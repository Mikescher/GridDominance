using System;
using System.Collections.Generic;
using System.Linq;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Shared.NormalGameScreen;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.NormalGameScreen.EntityOperations;
using Microsoft.Xna.Framework;
using GridDominance.Shared.Screens.NormalGameScreen.FractionController;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.DebugTools;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.GameMath;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using GridDominance.Shared.Screens.ScreenGame;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Entities;

namespace GridDominance.Shared.Screens.NormalGameScreen.Entities
{
	public class Cannon : GameEntity
	{
		private const float ROTATION_SPEED = FloatMath.TAU / 2; // 3.141 rad/sec
		private const float HEALTH_PROGRESS_SPEED = 1f; // min 1sec for 360°
		private const float BASE_COG_ROTATION_SPEED = FloatMath.TAU / 3; // 2.1 rad/sec
		private const float BARREL_RECOIL_SPEED = 4; // 250ms for full recovery (on normal boost and normal fraction mult)
		private const float BARREL_RECOIL_LENGTH = 32;

		private const float BARREL_CHARGE_SPEED = 0.9f;
		public  const float CANNON_DIAMETER = 96;		 // only diameter of base circle
		public  const float CANNON_OUTER_DIAMETER = 160; // includes fully extended barrel
		public  const float BARREL_HEIGHT = 32;
		public  const float BARREL_WIDTH = 64;
		private const float BULLET_ANGLE_VARIANCE = FloatMath.RAD_POS_002;
		public  const float BULLET_INITIAL_SPEED = 100f;

		private const float START_HEALTH_REGEN = 0.015f; // Health per sec bei 0HP
		private const float END_HEALTH_REGEN   = 0.105f; // Health per sec bei 1HP

		private const float BOOSTER_POWER = 0.5f;
		private const float BOOSTER_LIFETIME_MULTIPLIER = 0.9f;
		private const float MAX_BOOST = BOOSTER_POWER * 6;

		private const float HEALTH_HIT_DROP = 0.27f; // on Hit
		private const float HEALTH_HIT_GEN  = 0.27f; // on Hit from own fraction

		private const float CROSSHAIR_TRANSPARENCY = 0.5f;
		private const float CROSSHAIR_GROW_SPEED = 3f;

		public Fraction Fraction { get; private set; }
		private AbstractFractionController controller;
		public float TotalBoost = 0f;

		public float RealBoost => 1 + Math.Min(TotalBoost, MAX_BOOST);

		private float barrelCharge = 0f;
		private float barrelRecoil = 0f;
		public readonly DeltaLimitedFloat CannonHealth = new DeltaLimitedFloat(1f, HEALTH_PROGRESS_SPEED);

		public readonly DeltaLimitedModuloFloat Rotation;
		public readonly DeltaLimitedFloat CrosshairSize = new DeltaLimitedFloat(0f, CROSSHAIR_GROW_SPEED);
		public readonly float Scale; // scale effects: Size, HEALTH_REGEN, HEALTH_HIT_GEN (this+target), HEALTH_HIT_DROP (this+target), BARREL_CHARGE_SPEED
		public override Vector2 Position { get; }
		public override FSize DrawingBoundingBox { get; }
		public override Color DebugIdentColor => Fraction.Color;
		public readonly int BlueprintCannonID;
		private readonly BulletPathBlueprint[] bulletPathBlueprints;
		private float cannonCogRotation;
		public List<BulletPath> BulletPaths;

		public Body PhysicsBody;
		public Fixture PhysicsFixtureBase;
		public Fixture PhysicsFixtureBarrel;

		public Cannon(GDGameScreen scrn, CannonBlueprint blueprint, Fraction[] fractions) : base(scrn, GDConstants.ORDER_GAME_CANNON)
		{
			Fraction = fractions[blueprint.Player];

			Position = new Vector2(blueprint.X, blueprint.Y);
			Scale = blueprint.Diameter / CANNON_DIAMETER;
			DrawingBoundingBox = new FSize(CANNON_OUTER_DIAMETER, CANNON_OUTER_DIAMETER) * Scale;
			BlueprintCannonID = blueprint.CannonID;
			bulletPathBlueprints = blueprint.PrecalculatedPaths;

			Rotation = new DeltaLimitedModuloFloat(FloatMath.ToRadians(blueprint.Rotation), ROTATION_SPEED, FloatMath.TAU);
			
			CannonHealth.SetForce(Fraction.IsNeutral ? 0f : 1f);

			FindParticleSpawns();
		}

		#region Manage

		public override void OnInitialize(EntityManager manager)
		{
			controller = Fraction.CreateController(this.GDOwner(), this);

			PhysicsBody = BodyFactory.CreateBody(this.GDManager().PhysicsWorld, ConvertUnits.ToSimUnits(Position), 0, BodyType.Static);

			PhysicsFixtureBase = FixtureFactory.AttachCircle(
				ConvertUnits.ToSimUnits(Scale * CANNON_DIAMETER / 2), 1,
				PhysicsBody,
				Vector2.Zero, this);

			PhysicsFixtureBarrel = FixtureFactory.AttachRectangle(
				ConvertUnits.ToSimUnits(Scale * BARREL_WIDTH), ConvertUnits.ToSimUnits(Scale * BARREL_HEIGHT), 1, 
				new Vector2(ConvertUnits.ToSimUnits(Scale * CANNON_DIAMETER / 2), 0),
				PhysicsBody, this);
		}

		public void OnAfterLevelLoad()
		{
			BulletPaths = new List<BulletPath>();
			foreach (var bp in bulletPathBlueprints)
			{
				var c = this.GDOwner().GetEntities<Cannon>().First(p => p.BlueprintCannonID == bp.TargetCannonID);
				var r = bp.CannonRotation;
				
				BulletPaths.Add(new BulletPath(c, r, bp.Rays.ToList()));
			}
		}

		public override void OnRemove()
		{
			this.GDManager().PhysicsWorld.RemoveBody(PhysicsBody);
		}

		private void FindParticleSpawns()
		{
			this.GDOwner().GDBackground.RegisterSpawn(this, new FCircle(Position, CANNON_DIAMETER * Scale * 0.5f));
		}

		#endregion

		#region Update

		protected override void OnUpdate(SAMTime gameTime, InputState istate)
		{
			controller.Update(gameTime, istate);

			Rotation.Update(gameTime);
			CrosshairSize.Update(gameTime);

			UpdatePhysicBodies();
			UpdateHealth(gameTime);
			UpdateBarrel(gameTime);

#if DEBUG
			if (IsMouseDownOnThis(istate) && DebugSettings.Get("AssimilateCannon"))
			{
				while (Fraction.Type != FractionType.PlayerFraction)
					TakeDamage(this.GDOwner().GetPlayerFraction(), 1);

				CannonHealth.SetForce(1f);
			}
			if (IsMouseDownOnThis(istate) && DebugSettings.Get("AbandonCannon"))
			{
				CannonHealth.SetForce(0f);
				SetFraction(Fraction.GetNeutral());
			}
#endif
		}

		private bool IsMouseDownOnThis(InputState istate)
		{
			return istate.IsRealDown && (istate.GamePointerPosition - Position).Length() < CANNON_DIAMETER;
		}

		private void UpdateHealth(SAMTime gameTime)
		{
			CannonHealth.Update(gameTime);

			if (CannonHealth.TargetValue < 1 && CannonHealth.TargetValue > 0)
			{
				var bonus = START_HEALTH_REGEN + (END_HEALTH_REGEN - START_HEALTH_REGEN) * CannonHealth.TargetValue;

				bonus /= Scale;

				CannonHealth.Inc(bonus * gameTime.ElapsedSeconds);
				CannonHealth.Limit(0f, 1f);
			}

			if (CannonHealth.ActualValue >= 1 || (CannonHealth.ActualValue <= 0 && Fraction.IsNeutral))
			{
				var rotInc = BASE_COG_ROTATION_SPEED * Fraction.Multiplicator * RealBoost * gameTime.ElapsedSeconds;

				cannonCogRotation = (cannonCogRotation + rotInc) % (FloatMath.PI / 2);
			}
			else
			{
				if (FloatMath.FloatInequals(cannonCogRotation, FloatMath.PI / 2))
				{
					var rotInc = BASE_COG_ROTATION_SPEED * Fraction.GetNeutral().Multiplicator * RealBoost * gameTime.ElapsedSeconds;

					bool isLimited;
					cannonCogRotation = FloatMath.LimitedInc(cannonCogRotation, rotInc, FloatMath.PI/2, out isLimited);
					if (isLimited) cannonCogRotation = FloatMath.PI / 2;
				}
			}
		}

		private void UpdateBarrel(SAMTime gameTime)
		{
			if ((CannonHealth.TargetValue >= 1 || Fraction.IsNeutral) && controller.DoBarrelRecharge())
			{
				float chargeDelta = BARREL_CHARGE_SPEED * Fraction.Multiplicator * RealBoost * gameTime.ElapsedSeconds;
				if (Scale > 2.5f) chargeDelta /= Scale;

				barrelCharge += chargeDelta;

				if (barrelCharge >= 1f)
				{
					barrelCharge -= 1f;

					Shoot();
				}
			}

			if (barrelRecoil < 1)
			{
				barrelRecoil = FloatMath.LimitedInc(barrelRecoil, BARREL_RECOIL_SPEED * Fraction.Multiplicator * RealBoost * gameTime.ElapsedSeconds, 1f);
			}
		}

		private void Shoot()
		{
			var position = GetBulletSpawnPoint();
			var velocity = GetBulletVelocity();

			//Screen.PushNotification($"Cannon :: Shoot ({position.X:000.0}|{position.Y:000.0}) at {FloatMath.ToDegree(velocity.ToAngle()):000}°");

			barrelRecoil = 0f;

			Manager.AddEntity(new Bullet(Owner, this, position, velocity, Scale, Fraction));
			MainGame.Inst.GDSound.PlayEffectShoot();
		}

		private void UpdatePhysicBodies()
		{
			PhysicsBody.Rotation = Rotation.ActualValue;
		}

		public Vector2 GetBulletSpawnPoint()
		{
			return Position + new Vector2(Scale * (CANNON_DIAMETER/2 + Bullet.BULLET_DIAMETER * 0.66f), 0).Rotate(Rotation.ActualValue);
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

#if DEBUG
			// ASSERTION
			if (ActiveOperations.Count(p => p is CannonBooster) != FloatMath.Round(TotalBoost / BOOSTER_POWER)) throw new Exception("Assertion failed TotalBoost == Boosters");
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
			sbatch.DrawScaled(
				Textures.AnimCannonCog[Textures.ANIMATION_CANNONCOG_SIZE - 1],
				Position,
				Scale,
				FlatColors.Clouds,
				cannonCogRotation + 3 * (FloatMath.PI / 2));

			sbatch.DrawScaled(
				Textures.AnimCannonCog[(int)(CannonHealth.ActualValue * (Textures.ANIMATION_CANNONCOG_SIZE - 1))],
				Position,
				Scale,
				Fraction.Color,
				cannonCogRotation + 3 * (FloatMath.PI / 2));
		}

#if DEBUG

		protected override void DrawDebugBorders(IBatchRenderer sbatch)
		{
			base.DrawDebugBorders(sbatch);

			DrawDebugView(sbatch);

			// ASSERTION
			if (ActiveOperations.Count(p => p is CannonBooster) != FloatMath.Round(TotalBoost / BOOSTER_POWER)) throw new Exception("Assertion failed TotalBoost == Boosters");
		}

		private void DrawDebugView(IBatchRenderer sbatch)
		{
			var innerRadius = Scale*CANNON_DIAMETER/2;

			var rectChargeFull = new FRectangle(Position.X - innerRadius, Position.Y + innerRadius + (0 * 12) + 4, innerRadius * 2, 8);
			var rectChargeProg = new FRectangle(Position.X - innerRadius, Position.Y + innerRadius + (0 * 12) + 4, innerRadius * 2 * barrelCharge, 8);

			sbatch.FillRectangle(rectChargeFull, Color.White);
			sbatch.FillRectangle(rectChargeProg, Color.DarkGray);
			sbatch.DrawRectangle(rectChargeFull, Color.Black);

			var rectHealthFull  = new FRectangle(Position.X - innerRadius, Position.Y + innerRadius + (1 * 12) + 4, innerRadius * 2, 8);
			var rectHealthProgT = new FRectangle(Position.X - innerRadius, Position.Y + innerRadius + (1 * 12) + 4, innerRadius * 2 * CannonHealth.TargetValue, 8);
			var rectHealthProgA = new FRectangle(Position.X - innerRadius, Position.Y + innerRadius + (1 * 12) + 4, innerRadius * 2 * CannonHealth.ActualValue, 8);

			if (CannonHealth.IsDecreasing())
			{
				sbatch.FillRectangle(rectHealthFull, Color.White);
				sbatch.FillRectangle(rectHealthProgA, Fraction.Color.Lighten());
				sbatch.FillRectangle(rectHealthProgT, Fraction.Color);
				sbatch.DrawRectangle(rectHealthFull, Color.Black);
			}
			else if (CannonHealth.IsIncreasing())
			{
				sbatch.FillRectangle(rectHealthFull, Color.White);
				sbatch.FillRectangle(rectHealthProgT, Fraction.Color.Lighten());
				sbatch.FillRectangle(rectHealthProgA, Fraction.Color);
				sbatch.DrawRectangle(rectHealthFull, Color.Black);
			}
			else
			{
				sbatch.FillRectangle(rectHealthFull, Color.White);
				sbatch.FillRectangle(rectHealthProgA, Fraction.Color);
				sbatch.DrawRectangle(rectHealthFull, Color.Black);
			}

			for (int i = 0; i < ActiveOperations.Count; i++)
			{
				var rectFull = new FRectangle(Position.X - innerRadius, Position.Y + innerRadius + ((i+2) * 12) + 16, innerRadius * 2, 8);
				var rectProg = new FRectangle(Position.X - innerRadius, Position.Y + innerRadius + ((i+2) * 12) + 16, innerRadius * 2 * (1- ActiveOperations[i].Progress), 8);

				sbatch.FillRectangle(rectFull, Color.White);
				sbatch.FillRectangle(rectProg, Color.Chocolate);
				sbatch.DrawRectangle(rectFull, Color.Black);
			}

			var kicontroller = controller as KIController;
			if (kicontroller != null)
			{
				var r = new FRectangle(Position.X - DrawingBoundingBox.Width * 0.5f, Position.Y - DrawingBoundingBox.Height / 2f, DrawingBoundingBox.Width, 12);

				sbatch.FillRectangle(r, Color.LightGray * 0.5f);
				FontRenderHelper.DrawSingleLineInBox(sbatch, Textures.DebugFontSmall, kicontroller.LastKIFunction, r, 1, true, Color.Black);
			}
		}
#endif

		#endregion

		#region Change State

		public void ResetChargeAndBooster()
		{
			barrelCharge = 0f;

			FinishAllOperations(o => o is CannonBooster);
		}

		public void ApplyBoost()
		{
			if (Fraction.IsNeutral) return;

			CannonHealth.Inc(HEALTH_HIT_GEN);
			if (CannonHealth.Limit(0f, 1f) == 1)
			{
				AddEntityOperation(new CannonBooster(BOOSTER_POWER, 1/(BOOSTER_LIFETIME_MULTIPLIER * Fraction.Multiplicator)));
			}
		}

		public void TakeDamage(Fraction source, float sourceScale)
		{
			if (source.IsNeutral)
			{
				ResetChargeAndBooster();
			}
			else if (Fraction.IsNeutral)
			{
				SetFraction(source);
				CannonHealth.Set((sourceScale * HEALTH_HIT_GEN) / Scale);
				CannonHealth.Limit(0f, 1f);
			}
			else
			{
				CannonHealth.Dec((sourceScale * HEALTH_HIT_DROP) / Scale);

				if (FloatMath.IsZero(CannonHealth.TargetValue))
				{
					// Never tell me the odds

					SetFraction(Fraction.GetNeutral());
				}
				else if (CannonHealth.TargetValue < 0)
				{
					SetFraction(source);
					CannonHealth.Set(FloatMath.Abs(CannonHealth.TargetValue));
				}

				CannonHealth.Limit(0f, 1f);
			}
		}

		public void SetFraction(Fraction f)
		{
			Fraction = f;
			ResetChargeAndBooster();
			controller?.OnRemove();
			controller = Fraction.CreateController(this.GDOwner(), this);
		}

		public void RotateTo(GameEntity target)
		{
			Rotation.Set(FloatMath.PositiveAtan2(target.Position.Y - Position.Y, target.Position.X - Position.X));
			//Screen.PushNotification($"Cannon :: target({FloatMath.ToDegree(Rotation.TargetValue):000}°)");
		}

		public void ForceUpdateController()
		{
			controller?.OnRemove();
			controller = Fraction.CreateController(this.GDOwner(), this);
		}

		public void ForceSetController(AbstractFractionController ctrl)
		{
			controller = ctrl;
		}

		public void ForceResetBarrelCharge()
		{
			barrelCharge = 0f;
		}
 
		#endregion
	}
}
