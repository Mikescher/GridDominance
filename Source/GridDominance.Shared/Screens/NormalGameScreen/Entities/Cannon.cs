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
	public abstract class Cannon : GameEntity
	{
		protected const float ROTATION_SPEED = FloatMath.TAU / 2; // 3.141 rad/sec
		protected const float HEALTH_PROGRESS_SPEED = 1f; // min 1sec for 360°
		protected const float BASE_COG_ROTATION_SPEED = FloatMath.TAU / 3; // 2.1 rad/sec
		protected const float BARREL_RECOIL_SPEED = 4; // 250ms for full recovery (on normal boost and normal fraction mult)
		protected const float BARREL_RECOIL_LENGTH = 32;

		protected const float BARREL_CHARGE_SPEED = 0.9f;
		public    const float CANNON_DIAMETER = 96;		 // only diameter of base circle
		public    const float CANNON_OUTER_DIAMETER = 160; // includes fully extended barrel
		public    const float BARREL_HEIGHT = 32;
		public    const float BARREL_WIDTH = 64;
		protected const float BULLET_ANGLE_VARIANCE = FloatMath.RAD_POS_002;
		public    const float BULLET_INITIAL_SPEED = 100f;

		protected const float START_HEALTH_REGEN = 0.015f; // Health per sec bei 0HP
		protected const float END_HEALTH_REGEN   = 0.105f; // Health per sec bei 1HP

		protected const float BOOSTER_POWER = 0.5f;
		protected const float BOOSTER_LIFETIME_MULTIPLIER = 0.9f;
		protected const float MAX_BOOST = BOOSTER_POWER * 6;

		protected const float HEALTH_HIT_DROP = 0.27f; // on Hit
		protected const float HEALTH_HIT_GEN  = 0.27f; // on Hit from own fraction

		protected const float CROSSHAIR_TRANSPARENCY = 0.5f;
		protected const float CROSSHAIR_GROW_SPEED = 3f;

		public  Fraction Fraction { get; private set; }
		protected AbstractFractionController controller;
		public  float TotalBoost = 0f;

		public float RealBoost => 1 + Math.Min(TotalBoost, MAX_BOOST);

		public readonly DeltaLimitedFloat CannonHealth = new DeltaLimitedFloat(1f, HEALTH_PROGRESS_SPEED);

		public readonly DeltaLimitedModuloFloat Rotation;
		public readonly DeltaLimitedFloat CrosshairSize = new DeltaLimitedFloat(0f, CROSSHAIR_GROW_SPEED);
		public readonly float Scale; // scale effects: Size, HEALTH_REGEN, HEALTH_HIT_GEN (this+target), HEALTH_HIT_DROP (this+target), BARREL_CHARGE_SPEED
		public override Vector2 Position { get; }
		public override FSize DrawingBoundingBox { get; }
		public override Color DebugIdentColor => Fraction.Color;
		public readonly int BlueprintCannonID;
		private readonly BulletPathBlueprint[] bulletPathBlueprints;
		public List<BulletPath> BulletPaths;

		public Body PhysicsBody;
		public Fixture PhysicsFixtureBase;
		public Fixture PhysicsFixtureBarrel;

		protected Cannon(GDGameScreen scrn, Fraction[] fractions, int player, float px, float py, float diam, int cid, float rotdeg, BulletPathBlueprint[] paths) : base(scrn, GDConstants.ORDER_GAME_CANNON)
		{
			Fraction = fractions[player];

			Position = new Vector2(px, py);
			Scale = diam / CANNON_DIAMETER;
			DrawingBoundingBox = new FSize(CANNON_OUTER_DIAMETER, CANNON_OUTER_DIAMETER) * Scale;
			BlueprintCannonID = cid;
			bulletPathBlueprints = paths;

			Rotation = new DeltaLimitedModuloFloat(FloatMath.ToRadians(rotdeg), ROTATION_SPEED, FloatMath.TAU);
			
			CannonHealth.SetForce(Fraction.IsNeutral ? 0f : 1f);

			FindParticleSpawns();
		}

		#region Manage

		public override void OnInitialize(EntityManager manager)
		{
			controller = this.GDOwner().CreateController(Fraction, this);

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

		protected bool IsMouseDownOnThis(InputState istate)
		{
			return istate.IsRealDown && (istate.GamePointerPosition - Position).Length() < CANNON_DIAMETER;
		}

		protected void UpdateHealth(SAMTime gameTime)
		{
			CannonHealth.Update(gameTime);

			if (CannonHealth.TargetValue < 1 && CannonHealth.TargetValue > 0)
			{
				var bonus = START_HEALTH_REGEN + (END_HEALTH_REGEN - START_HEALTH_REGEN) * CannonHealth.TargetValue;

				bonus /= Scale;

				CannonHealth.Inc(bonus * gameTime.ElapsedSeconds);
				CannonHealth.Limit(0f, 1f);
			}
		}

		protected void UpdatePhysicBodies()
		{
			PhysicsBody.Rotation = Rotation.ActualValue;
		}

		#endregion

		#region Change State

		public abstract void ResetChargeAndBooster();

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
#if DEBUG
			if (DebugSettings.Get("ImmortalCannons")) return;
#endif

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
			controller = this.GDOwner().CreateController(Fraction, this);
		}

		public void RotateTo(GameEntity target)
		{
			Rotation.Set(FloatMath.PositiveAtan2(target.Position.Y - Position.Y, target.Position.X - Position.X));
			//Screen.PushNotification($"Cannon :: target({FloatMath.ToDegree(Rotation.TargetValue):000}°)");
		}

		public void ForceUpdateController()
		{
			controller?.OnRemove();
			controller = this.GDOwner().CreateController(Fraction, this);
		}

		public void ForceSetController(AbstractFractionController ctrl)
		{
			controller = ctrl;
		}

		public abstract void ForceResetBarrelCharge();
 
		#endregion
	}
}
