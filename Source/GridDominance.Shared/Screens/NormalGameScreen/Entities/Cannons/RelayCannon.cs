using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.NormalGameScreen.EntityOperations;
using GridDominance.Shared.Screens.NormalGameScreen.FractionController;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using GridDominance.Shared.Screens.ScreenGame;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.DebugTools;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using System;
using System.Linq;
using MonoSAMFramework.Portable.Extensions;
using GridDominance.Shared.Screens.NormalGameScreen.LaserNetwork;
using FarseerPhysics.Factories;
using GridDominance.Shared.Screens.NormalGameScreen.Physics;
using FarseerPhysics.Dynamics;
using FarseerPhysics;
using MonoSAMFramework.Portable;

namespace GridDominance.Shared.Screens.NormalGameScreen.Entities
{
	public class RelayCannon : Cannon
	{
		private readonly RelayCannonBlueprint Blueprint;
		private readonly GDGameScreen _screen;

		private float barrelRecoil = 0f;

		public int BulletBuffer = 0;
		public float BarrelCharge = 0f;

		private readonly bool _muted;

		public override bool IsLaser => false;

		public RelayCannon(GDGameScreen scrn, RelayCannonBlueprint bp, Fraction[] fractions) :
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
			if (barrelRecoil < 1)
			{
				barrelRecoil = FloatMath.LimitedInc(barrelRecoil, BARREL_RECOIL_SPEED * Fraction.BulletMultiplicator * RealBoost * gameTime.ElapsedSeconds, 1f);
			}

			if (Fraction.IsNeutral || BulletBuffer<=0)
			{
				BulletBuffer = 0;
				BarrelCharge = 0;
			}
			else
			{
				BarrelCharge += gameTime.ElapsedSeconds;
				if (BarrelCharge > RELAY_BARREL_CHARGE_TIME)
				{
					BarrelCharge -= RELAY_BARREL_CHARGE_TIME;
					Shoot();
					BulletBuffer--;
				}
			}
		}

		private void Shoot()
		{
			var position = GetBulletSpawnPoint();
			var velocity = GetBulletVelocity();

			//Screen.PushNotification($"Cannon :: Shoot ({position.X:000.0}|{position.Y:000.0}) at {FloatMath.ToDegree(velocity.ToAngle()):000}°");

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
			var variance = FloatMath.GetRangedRandom(-BULLET_ANGLE_VARIANCE, +BULLET_ANGLE_VARIANCE);
			var angle = FloatMath.AddRads(Rotation.ActualValue, variance);

			return new Vector2(1, 0).Rotate(angle) * BULLET_INITIAL_SPEED;
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
			sbatch.DrawCentered(
				Textures.TexCircle,
				Position,
				Scale * CANNON_DIAMETER * 0.5f,
				Scale * CANNON_DIAMETER * 0.5f,
				Fraction.Color);
		}

		#endregion

		#region Change State

		public override void ResetChargeAndBooster()
		{
			BulletBuffer = 0;
			BarrelCharge = 0f;
			FinishAllOperations(o => o is CannonBooster);
		}

		public override void ForceResetBarrelCharge()
		{
			BulletBuffer = 0;
			BarrelCharge = 0f;
		}

		public override void TakeDamage(Fraction source, float sourceScale)
		{
			if (Fraction.IsNeutral && !source.IsNeutral)
			{
				CannonHealth.Set(1);
				SetFraction(source);
			}
			else if (!Fraction.IsNeutral && source != Fraction)
			{
				if (counterAttackingLasersFriends > 0)
				{
					// ignore 
				}
				else
				{
					CannonHealth.Set(0);
					SetFraction(Fraction.GetNeutral());
				}

			}
		}

		public override void TakeLaserDamage(Fraction source, LaserRay ray, float dmg)
		{
			if (dmg > 0f)
			{
				if (Fraction.IsNeutral && !source.IsNeutral)
				{
					CannonHealth.Set(1);
					SetFraction(source);
				}
				else if (!Fraction.IsNeutral && source != Fraction)
				{
					CannonHealth.Set(0);
					SetFraction(Fraction.GetNeutral());
				}
			}

			if (!source.IsNeutral && !Fraction.IsNeutral)
			{
				if (dmg > 0) counterAttackingLasersEnemy++;
				_attackingRaysCollector.Add(ray);
			}
		}

		public override void ApplyBoost()
		{
			if (Fraction.IsNeutral) return;

			BulletBuffer++;
		}

		public override void ApplyLaserBoost(LaserCannon src, float pwr)
		{
			if (Fraction.IsNeutral) return;

			if (pwr > 0)
			{
				counterAttackingLasersFriends++;

				if (Fraction.IsNeutral && !src.Fraction.IsNeutral)
				{
					CannonHealth.Set(1);
					SetFraction(src.Fraction);
				}
				else if (!Fraction.IsNeutral && src.Fraction != Fraction)
				{
					CannonHealth.Set(0);
					SetFraction(Fraction.GetNeutral());
				}
			}
		}

		#endregion

		public override KIController CreateKIController(GDGameScreen screen, Fraction fraction)
		{
			return new RelayKIController(screen, this, fraction);
		}

		public void RemoteUpdate(Fraction frac, float shield, float sendertime)
		{
			if (frac != Fraction) SetFraction(frac);

			ManualBoost = 0;

			var delta = GDOwner.LevelTime - sendertime;

			CannonHealth.Set(frac.IsNeutral ? 0 : 1);
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
	}
}
