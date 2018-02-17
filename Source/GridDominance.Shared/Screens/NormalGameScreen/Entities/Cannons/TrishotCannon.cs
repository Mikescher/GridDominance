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
using MonoSAMFramework.Portable.Screens;
using System;
using System.Linq;
using MonoSAMFramework.Portable;
using MonoSAMFramework.Portable.Extensions;
using FarseerPhysics.Factories;
using FarseerPhysics;
using GridDominance.Shared.Screens.NormalGameScreen.Physics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Common;
using FarseerPhysics.Collision.Shapes;

namespace GridDominance.Shared.Screens.NormalGameScreen.Entities
{
	public class TrishotCannon : Cannon
	{
		private readonly TrishotCannonBlueprint Blueprint;
		private readonly GDGameScreen _screen;

		public float BarrelCharge = 0f;
		
		private float barrelRecoil = 0f;
		private float cannonCogRotation;

		private readonly bool _muted;

		public override bool IsLaser => false;

		public TrishotCannon(GDGameScreen scrn, TrishotCannonBlueprint bp, Fraction[] fractions) : 
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

			{
				var offset = new Vector2(ConvertUnits.ToSimUnits(Scale * CANNON_DIAMETER / 2), 0);
				var density = 1;
				var width = ConvertUnits.ToSimUnits(Scale * BARREL_WIDTH);
				var height = ConvertUnits.ToSimUnits(Scale * BARREL_HEIGHT);

				Vertices rectangleVertices = PolygonTools.CreateRectangle(width / 2, height / 2, Vector2.Zero, -TRISHOT_BARREL_ANGLE);
				var r = offset.Rotate(-TRISHOT_BARREL_ANGLE);
				rectangleVertices.Translate(ref r);
				PolygonShape rectangleShape = new PolygonShape(rectangleVertices, density);

				PhysicsBody.CreateFixture(rectangleShape, this);
			}

			{
				var offset = new Vector2(ConvertUnits.ToSimUnits(Scale * CANNON_DIAMETER / 2), 0);
				var density = 1;
				var width = ConvertUnits.ToSimUnits(Scale * BARREL_WIDTH);
				var height = ConvertUnits.ToSimUnits(Scale * BARREL_HEIGHT);

				Vertices rectangleVertices = PolygonTools.CreateRectangle(width / 2, height / 2, Vector2.Zero, 0);
				var r = offset;
				rectangleVertices.Translate(ref r);
				PolygonShape rectangleShape = new PolygonShape(rectangleVertices, density);

				PhysicsBody.CreateFixture(rectangleShape, this);
			}

			{
				var offset = new Vector2(ConvertUnits.ToSimUnits(Scale * CANNON_DIAMETER / 2), 0);
				var density = 1;
				var width = ConvertUnits.ToSimUnits(Scale * BARREL_WIDTH);
				var height = ConvertUnits.ToSimUnits(Scale * BARREL_HEIGHT);

				Vertices rectangleVertices = PolygonTools.CreateRectangle(width / 2, height / 2, Vector2.Zero, +TRISHOT_BARREL_ANGLE);
				var r = offset.Rotate(+TRISHOT_BARREL_ANGLE);
				rectangleVertices.Translate(ref r);
				PolygonShape rectangleShape = new PolygonShape(rectangleVertices, density);

				PhysicsBody.CreateFixture(rectangleShape, this);
			}
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
			if ((CannonHealth.TargetValue >= 1 || Fraction.IsNeutral))
			{
				if (controller.DoBarrelRecharge())
				{
					float chargeDelta = BARREL_CHARGE_SPEED_TRISHOT * Fraction.BulletMultiplicator * RealBoost * gameTime.ElapsedSeconds;
					if (Scale > 2.5f) chargeDelta /= Scale;

					BarrelCharge += chargeDelta;

					if (BarrelCharge >= 1f)
					{
						BarrelCharge -= 1f;

						Shoot();
					}
				}
				else if (controller.SimulateBarrelRecharge())
				{
					float chargeDelta = BARREL_CHARGE_SPEED_TRISHOT * Fraction.BulletMultiplicator * RealBoost * gameTime.ElapsedSeconds;
					if (Scale > 2.5f) chargeDelta /= Scale;

					BarrelCharge += chargeDelta;

					if (BarrelCharge >= 1f)
					{
						BarrelCharge -= 1f;

						barrelRecoil = 0f;
					}
				}

			}

			if (barrelRecoil < 1)
			{
				barrelRecoil = FloatMath.LimitedInc(barrelRecoil, BARREL_RECOIL_SPEED * Fraction.BulletMultiplicator * RealBoost * gameTime.ElapsedSeconds, 1f);
			}
		}
		
		private void UpdateCog(SAMTime gameTime)
		{
			if (CannonHealth.ActualValue >= 1 || (CannonHealth.ActualValue <= 0 && Fraction.IsNeutral))
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
			var position1 = GetBulletSpawnPoint(-1);
			var velocity1 = GetBulletVelocity(-1);
			Manager.AddEntity(new Bullet(GDOwner, this, position1, velocity1, Scale, Fraction));

			var position2 = GetBulletSpawnPoint(00);
			var velocity2 = GetBulletVelocity(00);
			Manager.AddEntity(new Bullet(GDOwner, this, position2, velocity2, Scale, Fraction));

			var position3 = GetBulletSpawnPoint(+1);
			var velocity3 = GetBulletVelocity(+1);
			Manager.AddEntity(new Bullet(GDOwner, this, position3, velocity3, Scale, Fraction));

			barrelRecoil = 0f;

			if (!_muted) MainGame.Inst.GDSound.PlayEffectShoot();
		}

		public FPoint GetBulletSpawnPoint(int i)
		{
			return Position + new Vector2(Scale * (CANNON_DIAMETER / 2 + Bullet.BULLET_DIAMETER * 0.66f), 0).Rotate(Rotation.ActualValue + i * TRISHOT_BARREL_ANGLE);
		}

		public Vector2 GetBulletVelocity(int i)
		{
			var variance = FloatMath.GetRangedRandom(-BULLET_ANGLE_VARIANCE, +BULLET_ANGLE_VARIANCE);
			var angle = FloatMath.AddRads(Rotation.ActualValue + i * TRISHOT_BARREL_ANGLE, variance);

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

		private void DrawBodyAndBarrel_BG(IBatchRenderer sbatch)
		{
			var recoil = (1 - barrelRecoil) * BARREL_RECOIL_LENGTH;

			var barrelCenter1 = Position + new Vector2(Scale * (CANNON_DIAMETER / 2f - recoil), 0).Rotate(Rotation.ActualValue - TRISHOT_BARREL_ANGLE);
			var barrelCenter2 = Position + new Vector2(Scale * (CANNON_DIAMETER / 2f - recoil), 0).Rotate(Rotation.ActualValue);
			var barrelCenter3 = Position + new Vector2(Scale * (CANNON_DIAMETER / 2f - recoil), 0).Rotate(Rotation.ActualValue + TRISHOT_BARREL_ANGLE);

			sbatch.DrawScaled(
				Textures.TexCannonBarrelShadow,
				barrelCenter1,
				Scale,
				Color.White,
				Rotation.ActualValue - TRISHOT_BARREL_ANGLE);

			sbatch.DrawScaled(
				Textures.TexCannonBarrelShadow,
				barrelCenter2,
				Scale,
				Color.White,
				Rotation.ActualValue);

			sbatch.DrawScaled(
				Textures.TexCannonBarrelShadow,
				barrelCenter3,
				Scale,
				Color.White,
				Rotation.ActualValue + TRISHOT_BARREL_ANGLE);

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

			var barrelCenter1 = Position + new Vector2(Scale * (CANNON_DIAMETER / 2f - recoil), 0).Rotate(Rotation.ActualValue - TRISHOT_BARREL_ANGLE);
			var barrelCenter2 = Position + new Vector2(Scale * (CANNON_DIAMETER / 2f - recoil), 0).Rotate(Rotation.ActualValue);
			var barrelCenter3 = Position + new Vector2(Scale * (CANNON_DIAMETER / 2f - recoil), 0).Rotate(Rotation.ActualValue + TRISHOT_BARREL_ANGLE);

			sbatch.DrawScaled(
				Textures.TexCannonBarrel,
				barrelCenter1,
				Scale,
				Color.White,
				Rotation.ActualValue - TRISHOT_BARREL_ANGLE);

			sbatch.DrawScaled(
				Textures.TexCannonBarrel,
				barrelCenter2,
				Scale,
				Color.White,
				Rotation.ActualValue);

			sbatch.DrawScaled(
				Textures.TexCannonBarrel,
				barrelCenter3,
				Scale,
				Color.White,
				Rotation.ActualValue + TRISHOT_BARREL_ANGLE);

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

			sbatch.DrawScaled(
				Textures.CannonCog,
				Position,
				Scale,
				FlatColors.Clouds,
				cannonCogRotation + FloatMath.RAD_POS_270);

			int aidx = (int) (health * (Textures.ANIMATION_CANNONCOG_SIZE - 1));

			if (aidx == Textures.ANIMATION_CANNONCOG_SIZE - 1)
			{
				sbatch.DrawScaled(
					Textures.CannonCog,
					Position,
					Scale,
					Fraction.Color,
					cannonCogRotation + FloatMath.RAD_POS_270);
			}
			else
			{
				int aniperseg = Textures.ANIMATION_CANNONCOG_SIZE / Textures.ANIMATION_CANNONCOG_SEGMENTS;
				float radpersegm = (FloatMath.RAD_POS_360 * 1f / Textures.ANIMATION_CANNONCOG_SEGMENTS);
				for (int i = 0; i < Textures.ANIMATION_CANNONCOG_SEGMENTS; i++)
				{
					if (aidx >= aniperseg * i)
					{
						var iidx = aidx - aniperseg * i;
						if (iidx > aniperseg + Textures.ANIMATION_CANNONCOG_OVERLAP) iidx = aniperseg + Textures.ANIMATION_CANNONCOG_OVERLAP;

						sbatch.DrawScaled(
							Textures.AnimCannonCog[iidx],
							Position,
							Scale,
							Fraction.Color,
							cannonCogRotation + FloatMath.RAD_POS_270 + i * radpersegm);
					}
				}
			}
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

			if ((CannonHealth.TargetValue >= 1 || Fraction.IsNeutral))
			{
				float chargeDelta = BARREL_CHARGE_SPEED_TRISHOT * Fraction.BulletMultiplicator * RealBoost * gameTime.ElapsedSeconds;
				if (Scale > 2.5f) chargeDelta /= Scale;

				BarrelCharge += chargeDelta;

				if (BarrelCharge >= 1f)
				{
					BarrelCharge -= 1f;

					barrelRecoil = 0f;
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
			CannonHealth.Set(1);
		}
	}
}
