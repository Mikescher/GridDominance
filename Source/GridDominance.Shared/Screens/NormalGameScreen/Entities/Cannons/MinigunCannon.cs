using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.NormalGameScreen.EntityOperations;
using GridDominance.Shared.Screens.NormalGameScreen.FractionController;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using GridDominance.Shared.Screens.ScreenGame;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.DebugTools;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens;
using System;
using System.Linq;
using MonoSAMFramework.Portable;
using MonoSAMFramework.Portable.Extensions;
using FarseerPhysics.Factories;
using FarseerPhysics;
using GridDominance.Shared.Screens.NormalGameScreen.Physics;
using FarseerPhysics.Dynamics;

namespace GridDominance.Shared.Screens.NormalGameScreen.Entities
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
						Shoot();
						_remainingBullets--;
						_nextBulletTime += MINIGUN_BULLET_DELAY;
					}

					if (_remainingBullets <= 0) _nextBulletTime = 0;
				}
				else
				{
					if (controller.DoBarrelRecharge())
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
					else if (controller.SimulateBarrelRecharge())
					{
						float chargeDelta = BARREL_CHARGE_SPEED_MINIGUN * Fraction.BulletMultiplicator * RealBoost * gameTime.ElapsedSeconds;
						if (Scale > 2.5f) chargeDelta /= Scale;

						BarrelCharge += chargeDelta;

						if (BarrelCharge >= 1f)
						{
							BarrelCharge -= 1f;

							barrelRecoil = 0f;
						}
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
			DrawBodyAndBarrel_BG(sbatch);
		}

		protected override void OnDrawOrderedForegroundLayer(IBatchRenderer sbatch)
		{
			DrawCrosshair(sbatch);
			DrawBodyAndBarrel_FG(sbatch);
			DrawCog(sbatch);

			DrawShield(sbatch);

#if DEBUG
			// ASSERTION
			if (ActiveOperations.Count(p => p is CannonBooster) != BulletBoostCount) throw new Exception("Assertion failed TotalBoost == Boosters");
#endif
		}

		private void DrawCrosshair(IBatchRenderer sbatch)
		{
			if (FloatMath.IsNotZero(CrosshairSize.ActualValue))
			{
				sbatch.DrawScaled(
					Textures.TexCannonCrosshair,
					Position,
					Scale * CrosshairSize.ActualValue,
					Color.White * (CROSSHAIR_TRANSPARENCY * CrosshairSize.ActualValue),
					Rotation.TargetValue);
			}
		}

		private void DrawBodyAndBarrel_BG(IBatchRenderer sbatch)
		{
			var recoil = (1 - barrelRecoil) * BARREL_RECOIL_LENGTH;

			var barrelCenter = Position + new Vector2(Scale * (CANNON_DIAMETER / 2f - recoil), 0).Rotate(Rotation.ActualValue);

			sbatch.DrawScaled(
				Textures.TexCannonBarrelShadow,
				barrelCenter,
				Scale,
				Color.White,
				Rotation.ActualValue);

			sbatch.DrawScaled(
				Textures.TexCannonBodyShadow,
				Position,
				Scale,
				Color.White,
				Rotation.ActualValue);
		}

		private void DrawBodyAndBarrel_FG(IBatchRenderer sbatch)
		{
			var recoil = (1 - barrelRecoil) * BARREL_RECOIL_LENGTH;

			var barrelCenter = Position + new Vector2(Scale * (CANNON_DIAMETER / 2f - recoil), 0).Rotate(Rotation.ActualValue);

			sbatch.DrawScaled(
				Textures.TexCannonBarrel,
				barrelCenter,
				Scale,
				Color.White,
				Rotation.ActualValue);

			sbatch.DrawScaled(
				Textures.TexCannonBody,
				Position,
				Scale,
				Color.White,
				Rotation.ActualValue);
		}

		private void DrawCog(IBatchRenderer sbatch)
		{
			var health = CannonHealth.ActualValue;
			if (health > 0.99) health = 1f;

			sbatch.DrawCentered(
				Textures.TexPixel, 
				Position,
				Scale * MINIGUNSTRUCT_DIAMETER,
				Scale * MINIGUNSTRUCT_DIAMETER,
				FlatColors.Clouds,
				FloatMath.RAD_POS_000 - cannonCogRotation);

			sbatch.DrawCentered(
				Textures.TexPixel,
				Position,
				Scale * MINIGUNSTRUCT_DIAMETER,
				Scale * MINIGUNSTRUCT_DIAMETER,
				FlatColors.Clouds,
				FloatMath.RAD_POS_000 + cannonCogRotation);

			if (health < 0.01)
			{
				// nothing
			}
			else if (health < 1)
			{
				var r = FRectangle.CreateByCenter(Position, health * Scale * MINIGUNSTRUCT_DIAMETER, health * Scale * MINIGUNSTRUCT_DIAMETER);

				sbatch.FillRectangleRot(r, Fraction.Color * (1 - health), FloatMath.RAD_POS_000 + cannonCogRotation);
				sbatch.FillRectangleRot(r, Fraction.Color * (1 - health), FloatMath.RAD_POS_000 - cannonCogRotation);

				sbatch.DrawRectangleRot(r, Fraction.Color, FloatMath.RAD_POS_000 + cannonCogRotation, 2f);
				sbatch.DrawRectangleRot(r, Fraction.Color, FloatMath.RAD_POS_000 - cannonCogRotation, 2f);
			}
			else
			{
				var r = FRectangle.CreateByCenter(Position, Scale * MINIGUNSTRUCT_DIAMETER, Scale * MINIGUNSTRUCT_DIAMETER);

				sbatch.DrawRectangleRot(r, Fraction.Color, FloatMath.RAD_POS_000 + cannonCogRotation, 2f);
				sbatch.DrawRectangleRot(r, Fraction.Color, FloatMath.RAD_POS_000 - cannonCogRotation, 2f);
			}
		}

		#endregion

		#region Change State

		public override void ResetChargeAndBooster()
		{
			BarrelCharge = 0f;

			FinishAllOperations(o => o is CannonBooster);
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
				AddEntityOperation(new CannonBooster(1 / (BOOSTER_LIFETIME_MULTIPLIER * Fraction.BulletMultiplicator)));
			}
		}

		#endregion

		public override KIController CreateKIController(GDGameScreen screen, Fraction fraction)
		{
			return new BulletKIController(screen, this, fraction);
		}

		public void RemoteUpdate(Fraction frac, float hp, byte boost, float charge, float sendertime)
		{
			if (frac != Fraction) SetFraction(frac);

			ManualBoost = boost;
			BarrelCharge = charge;

			var delta = GDOwner.LevelTime - sendertime;

			CannonHealth.Set(hp);

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
		}
	}
}
