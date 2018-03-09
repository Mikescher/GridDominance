using System;
using System.Linq;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Shared.Screens.Common;
using GridDominance.Shared.Screens.NormalGameScreen.Entities.EntityOperations;
using GridDominance.Shared.Screens.NormalGameScreen.FractionController;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using GridDominance.Shared.Screens.NormalGameScreen.Physics;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.DebugTools;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;

namespace GridDominance.Shared.Screens.NormalGameScreen.Entities.Cannons
{
	public class MinigunCannon : Cannon
	{
		private readonly MinigunBlueprint Blueprint;
		private readonly GDGameScreen _screen;

		public float BarrelCharge = 0f;
		
		private float barrelRecoil = 0f;
		private float cannonCogRotation;

		private int _remainingBullets = 0;
		private float _nextBulletTime = 0f;

		private readonly bool _muted;

		public override bool IsLaser => false;

		public MinigunCannon(GDGameScreen scrn, MinigunBlueprint bp, Fraction[] fractions) : 
			base(scrn, fractions, bp.Player, bp.X, bp.Y, bp.Diameter, bp.CannonID, bp.Rotation, bp.PrecalculatedPaths)
		{
			Blueprint = bp;
			_screen = scrn;
			_muted = scrn.IsPreview;
		}

		protected override void CreatePhysics()
		{
			PhysicsBody = BodyFactory.CreateBody(this.GDManager().PhysicsWorld, ConvertUnits2.ToSimUnits(Position), 0, BodyType.Static);

			PhysicsFixtureBase = FixtureFactory.AttachCircle(
				ConvertUnits.ToSimUnits(Scale * CANNON_DIAMETER / 2), 1,
				PhysicsBody,
				Vector2.Zero, this);

			FixtureFactory.AttachRectangle(
				ConvertUnits.ToSimUnits(Scale * BARREL_WIDTH), ConvertUnits.ToSimUnits(Scale * BARREL_HEIGHT), 1,
				new Vector2(ConvertUnits.ToSimUnits(Scale * CANNON_DIAMETER / 2), 0),
				PhysicsBody, this);
		}

		#region Update

		protected override void OnUpdate(SAMTime gameTime, InputState istate)
		{
			controller.Update(gameTime, istate);

			bool change = Rotation.CUpdate(gameTime);
			if (change) _screen.LaserNetwork.SemiDirty = true;

			CrosshairSize.Update(gameTime);

			UpdatePhysicBodies();
			UpdateHealth(gameTime);
			UpdateBoost(gameTime);
			UpdateCog(gameTime);
			UpdateBarrel(gameTime);
			UpdateShield(gameTime);

#if DEBUG
			if (IsMouseDownOnThis(istate) && DebugSettings.Get("AssimilateCannon"))
			{
				var bckp = DebugSettings.Get("ImmortalCannons");
				DebugSettings.SetManual("ImmortalCannons", false);
				
				while (Fraction.Type != FractionType.PlayerFraction)
					TakeDamage(this.GDOwner().GetPlayerFraction(), 1);

				DebugSettings.SetManual("ImmortalCannons", bckp);

				CannonHealth.SetForce(1f);
			}
			if (IsMouseDownOnThis(istate) && DebugSettings.Get("LooseCannon"))
			{
				var bckp = DebugSettings.Get("ImmortalCannons");
				DebugSettings.SetManual("ImmortalCannons", false);

				while (Fraction.Type != FractionType.ComputerFraction)
					TakeDamage(this.GDOwner().GetComputerFraction(), 1);

				DebugSettings.SetManual("ImmortalCannons", bckp);

				CannonHealth.SetForce(1f);
			}
			if (IsMouseDownOnThis(istate) && DebugSettings.Get("AbandonCannon"))
			{
				CannonHealth.SetForce(0f);
				SetFraction(Fraction.GetNeutral());
			}
#endif
		}

		private void UpdateBarrel(SAMTime gameTime)
		{
			if (CannonHealth.TargetValue >= 1)
			{
				if (_remainingBullets > 0)
				{
					_nextBulletTime -= gameTime.ElapsedSeconds;

					if (_nextBulletTime <= 0)
					{
						if (controller.DoBarrelRecharge())
							Shoot();
						else if (controller.SimulateBarrelRecharge())
							barrelRecoil = 0f;

						_remainingBullets--;
						_nextBulletTime += MINIGUN_BULLET_DELAY;
					}

					if (_remainingBullets <= 0) _nextBulletTime = 0;
				}
				else
				{
					float chargeDelta = BARREL_CHARGE_SPEED_MINIGUN * Fraction.BulletMultiplicator * RealBoost * gameTime.ElapsedSeconds;
					if (Scale > 2.5f) chargeDelta /= Scale;

					BarrelCharge += chargeDelta;

					if (BarrelCharge >= 1f)
					{
						BarrelCharge -= 1f;

						_remainingBullets = MINIGUN_BULLET_COUNT;
					}
				}
			}
			else
			{
				_remainingBullets = 0;
			}

			if (barrelRecoil < 1)
			{
				barrelRecoil = FloatMath.LimitedInc(barrelRecoil, BARREL_RECOIL_SPEED * Fraction.BulletMultiplicator * RealBoost * gameTime.ElapsedSeconds, 1f);
			}
		}
		
		private void UpdateCog(SAMTime gameTime)
		{
			if (CannonHealth.ActualValue >= 1)
			{
				var rotInc = BASE_COG_ROTATION_SPEED * Fraction.BulletMultiplicator * RealBoost * gameTime.ElapsedSeconds;

				cannonCogRotation = (cannonCogRotation + rotInc) % (FloatMath.PI / 2);
			}
			else
			{
				if (FloatMath.FloatInequals(cannonCogRotation, FloatMath.PI / 2))
				{
					var rotInc = BASE_COG_ROTATION_SPEED * Fraction.GetNeutral().BulletMultiplicator * RealBoost * gameTime.ElapsedSeconds;

					bool isLimited;
					cannonCogRotation = FloatMath.LimitedInc(cannonCogRotation, rotInc, FloatMath.PI / 2, out isLimited);
					if (isLimited) cannonCogRotation = FloatMath.PI / 2;
				}
			}
		}

		private void Shoot()
		{
			var position = GetBulletSpawnPoint();
			var velocity = GetBulletVelocity();

			barrelRecoil = 0f;

			Manager.AddEntity(new Bullet(GDOwner, this, position, velocity, Scale, Fraction));
			if (!_muted) MainGame.Inst.GDSound.PlayEffectShoot();
		}

		public FPoint GetBulletSpawnPoint()
		{
			return Position + new Vector2(Scale * (CANNON_DIAMETER / 2 + Bullet.BULLET_DIAMETER * 0.66f), 0).Rotate(Rotation.ActualValue);
		}

		public Vector2 GetBulletVelocity()
		{
			var variance = FloatMath.GetRangedRandom(-MG_BULLET_ANGLE_VARIANCE, +MG_BULLET_ANGLE_VARIANCE);
			var angle = FloatMath.AddRads(Rotation.ActualValue, variance);

			return new Vector2(1, 0).Rotate(angle) * MG_BULLET_INITIAL_SPEED;
		}

		#endregion

		#region Draw

		protected override void OnDraw(IBatchRenderer sbatch)
		{
			CommonCannonRenderer.DrawMinigunCannon_BG(sbatch, Position, Scale, Rotation.ActualValue, barrelRecoil);
		}

		protected override void OnDrawOrderedForegroundLayer(IBatchRenderer sbatch)
		{
			DrawCrosshair(sbatch);

			CommonCannonRenderer.DrawMinigunCannon_FG(sbatch, Position, Scale, Rotation.ActualValue, barrelRecoil, cannonCogRotation, CannonHealth.ActualValue, Fraction.Color);

			DrawShield(sbatch);

#if DEBUG
			// ASSERTION
			if (ActiveOperations.Count(p => p is CannonBooster) != BulletBoostCount) throw new Exception("Assertion failed TotalBoost == Boosters");
#endif
		}

		#endregion

		#region Change State

		public override void ResetChargeAndBooster()
		{
			BarrelCharge = 0f;

			AbortAllOperations(o => o is CannonBooster);
		}

		public override void ForceResetBarrelCharge()
		{
			BarrelCharge = 0f;
		}

		public override void ApplyBoost()
		{
			if (Fraction.IsNeutral) return;

			CannonHealth.Inc(HEALTH_HIT_GEN);
			if (CannonHealth.Limit(0f, 1f) == 1)
			{
				AddOperation(new CannonBooster(1 / (BOOSTER_LIFETIME_MULTIPLIER * Fraction.BulletMultiplicator)));
			}
		}
		
		#endregion

		public override KIController CreateKIController(GDGameScreen screen, Fraction fraction)
		{
			return new BulletKIController(screen, this, fraction);
		}

		public void RemoteUpdate(Fraction frac, float hp, byte boost, float charge, float shield, float sendertime)
		{
			if (frac != Fraction) SetFraction(frac);

			ManualBoost = boost;
			BarrelCharge = charge;

			var delta = GDOwner.LevelTime - sendertime;

			CannonHealth.Set(hp);
			ShieldTime = shield;

			var ups = delta / (1 / 30f);

			if (ups > 1)
			{
				var iups = FloatMath.Min(FloatMath.Round(ups), 10);
				var gt30 = new SAMTime((delta / iups) * GDOwner.GameSpeed, MonoSAMGame.CurrentTime.TotalElapsedSeconds);

				for (int i = 0; i < iups; i++)
				{
					RemoteUpdateSim(gt30);
				}
			}
		}

		private void RemoteUpdateSim(SAMTime gameTime)
		{
			CannonHealth.Update(gameTime);

			if (CannonHealth.TargetValue < 1 && CannonHealth.TargetValue > MIN_REGEN_HEALTH && (LastAttackingLasersEnemy <= LastAttackingLasersFriends))
			{
				var bonus = START_HEALTH_REGEN + (END_HEALTH_REGEN - START_HEALTH_REGEN) * CannonHealth.TargetValue;

				bonus /= Scale;

				CannonHealth.Inc(bonus * gameTime.ElapsedSeconds);
				CannonHealth.Limit(0f, 1f);
			}

			if (CannonHealth.TargetValue >= 1)
			{
				float chargeDelta = BARREL_CHARGE_SPEED_MINIGUN * Fraction.BulletMultiplicator * RealBoost * gameTime.ElapsedSeconds;
				if (Scale > 2.5f) chargeDelta /= Scale;

				if (_remainingBullets > 0)
				{
					_nextBulletTime -= gameTime.ElapsedSeconds;

					if (_nextBulletTime <= 0)
					{
						barrelRecoil = 0f;

						_remainingBullets--;
						_nextBulletTime += MINIGUN_BULLET_DELAY;
					}

					if (_remainingBullets <= 0) _nextBulletTime = 0;
				}
				else
				{
					BarrelCharge += chargeDelta;

					if (BarrelCharge >= 1f)
					{
						BarrelCharge -= 1f;

						_remainingBullets = MINIGUN_BULLET_COUNT;
					}
				}
			}

			if (LastAttackingShieldLaser > 0)
			{
				ShieldTime += (SHIELD_CHARGE_SPEED) * gameTime.ElapsedSeconds;
				if (ShieldTime > MAX_SHIELD_TIME) ShieldTime = MAX_SHIELD_TIME;
			}
			else
			{
				ShieldTime -= (SHIELD_DISCHARGE_SPEED) * gameTime.ElapsedSeconds;
				if (ShieldTime < 0) ShieldTime = 0;
			}
		}

		public override void SetFractionAndHealth(Fraction fraction, float hp)
		{
			SetFraction(fraction);
			CannonHealth.Set(hp);
			CannonHealth.Limit(0f, 1f);
		}
	}
}
