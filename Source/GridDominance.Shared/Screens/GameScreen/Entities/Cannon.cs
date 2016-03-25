using System;
using System.Collections.Generic;
using System.Linq;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using GridDominance.Levelformat.Parser;
using GridDominance.Shared.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using GridDominance.Shared.Framework;
using GridDominance.Shared.Screens.GameScreen.Background;
using GridDominance.Shared.Screens.GameScreen.EntityOperations;
using GridDominance.Shared.Screens.GameScreen.FractionController;
using MonoGame.Extended.Shapes;
using MonoGame.Extended.TextureAtlases;

namespace GridDominance.Shared.Screens.GameScreen.Entities
{
	class Cannon : GDEntity
	{
		private const float ROTATION_SPEED = FloatMath.TAU / 2; // 3.141 rad/sec
		private const float HEALTH_PROGRESS_SPEED = 1f; // min 1sec for 360°
		private const float BASE_COG_ROTATION_SPEED = FloatMath.TAU / 3; // 2.1 rad/sec
		private const float BARREL_RECOIL_SPEED = 4; // 250ms for full recovery (on normal boost and normal fraction mult)
		private const float BARREL_RECOIL_LENGTH = 32;

		private const float BARREL_CHARGE_SPEED = 0.9f;
		public  const float CANNON_DIAMETER = 96;
		public  const float BARREL_HEIGHT = 32;
		public  const float BARREL_WIDTH = 64;
		private const float BULLET_ANGLE_VARIANCE = 0.035f; // ~ 2 degree
		private const float BULLET_INITIAL_SPEED = 100f;

		private const float START_HEALTH_REGEN = 0.015f; // Health per sec bei 0HP
		private const float END_HEALTH_REGEN   = 0.105f; // Health per sec bei 1HP

		private const float BOOSTER_POWER = 0.5f;
		private const float BOOSTER_LIFETIME_MULTIPLIER = 0.9f;

		private const float HEALTH_HIT_DROP = 0.20f; // on Hit
		private const float HEALTH_HIT_GEN  = 0.20f; // on Hit from own fraction

		public Fraction Fraction { get; private set; }
		private AbstractFractionController controller;
		public float TotalBoost = 0f;

		private float barrelCharge = 0f;
		private float barrelRecoil = 0f;
		public readonly DeltaLimitedFloat CannonHealth = new DeltaLimitedFloat(1f, HEALTH_PROGRESS_SPEED);

		public readonly DeltaLimitedModuloFloat Rotation;
		public readonly Vector2 Center;
		public readonly float Scale;
		public override Vector2 Position => Center;
		private float cannonCogRotation;
		private List<Vector2> GridSourcePoints;

		public Body PhysicsBody;
		public Fixture PhysicsFictureBase;
		public Fixture PhysicsFictureBarrel;

		public Cannon(GameScreen scrn, LPCannon blueprint, Fraction[] fractions) : base(scrn)
		{
			Fraction = fractions[blueprint.Player];

			Center = new Vector2(blueprint.X, blueprint.Y);
			Scale = blueprint.Scale;
			
			Rotation = new DeltaLimitedModuloFloat(FloatMath.ToRadians(blueprint.Rotation), ROTATION_SPEED, FloatMath.TAU);
			
			CannonHealth.SetForce(Fraction.IsNeutral ? 0f : 1f);

			FindGridSourcePoints();
		}

		#region Manage

		public override void OnInitialize()
		{
			controller = Fraction.CreateController(Owner, this);

			PhysicsBody = BodyFactory.CreateBody(Manager.PhysicsWorld, ConvertUnits.ToSimUnits(Center), 0, BodyType.Static);

			PhysicsFictureBase = FixtureFactory.AttachCircle(
				ConvertUnits.ToSimUnits(Scale * CANNON_DIAMETER / 2), 1,
				PhysicsBody,
				Vector2.Zero, this);

			PhysicsFictureBarrel = FixtureFactory.AttachRectangle(
				ConvertUnits.ToSimUnits(Scale * BARREL_WIDTH), ConvertUnits.ToSimUnits(Scale * BARREL_HEIGHT), 1, 
				new Vector2(ConvertUnits.ToSimUnits(Scale * CANNON_DIAMETER / 2), 0),
				PhysicsBody, this);
		}

		public override void OnRemove()
		{
			Manager.PhysicsWorld.RemoveBody(PhysicsBody);

			foreach (var source in GridSourcePoints)
			{
				Owner.Background.DeregisterBlockedSpawn(this, (int)source.X, (int)source.Y);
			}
		}

		private void FindGridSourcePoints()
		{
			GridSourcePoints = new List<Vector2>();

			var cx = Center.X/GameScreen.TILE_WIDTH;
			var cy = Center.Y/GameScreen.TILE_WIDTH;
			var cr = (CANNON_DIAMETER * Scale * 0.5f)/GameScreen.TILE_WIDTH;

			for (int x = FloatMath.Ceiling(cx - cr); x <= FloatMath.Floor(cx + cr); x++)
			{
				for (int y = FloatMath.Ceiling(cy - cr); y <= FloatMath.Floor(cy + cr); y++)
				{
					var d = (cx - x)*(cx - x) + (cy - y)*(cy - y);
					if (d <= cr*cr)
					{
						GridSourcePoints.Add(new Vector2(x, y));
						Owner.Background.RegisterBlockedSpawn(this, x, y);
					}
				}
			}
		}

		#endregion

		#region Update

		protected override void OnUpdate(GameTime gameTime, InputState istate)
		{
			controller.Update(gameTime, istate);

			Rotation.Update(gameTime);
			UpdatePhysicBodies();
			UpdateHealth(gameTime);
			UpdateBarrel(gameTime);
		}

		private void UpdateHealth(GameTime gameTime)
		{
			CannonHealth.Update(gameTime);

			if (CannonHealth.TargetValue < 1 && CannonHealth.TargetValue > 0)
			{
				var bonus = START_HEALTH_REGEN + (END_HEALTH_REGEN - START_HEALTH_REGEN) * CannonHealth.TargetValue;

				CannonHealth.Inc(bonus * gameTime.GetElapsedSeconds());
				CannonHealth.Limit(0f, 1f);
			}

			if (CannonHealth.ActualValue >= 1 || (CannonHealth.ActualValue <= 0 && Fraction.IsNeutral))
			{
				var rotInc = BASE_COG_ROTATION_SPEED * Fraction.Multiplicator * (1 + TotalBoost) * gameTime.GetElapsedSeconds();

				cannonCogRotation = (cannonCogRotation + rotInc) % (FloatMath.PI / 2);
			}
			else
			{
				if (FloatMath.FloatInequals(cannonCogRotation, FloatMath.PI / 2))
				{
					var rotInc = BASE_COG_ROTATION_SPEED * Fraction.GetNeutral().Multiplicator * (1 + TotalBoost) * gameTime.GetElapsedSeconds();

					bool isLimited;
					cannonCogRotation = FloatMath.LimitedInc(cannonCogRotation, rotInc, FloatMath.PI/2, out isLimited);
					if (isLimited) cannonCogRotation = FloatMath.PI / 2;
				}
			}
		}

		private void UpdateBarrel(GameTime gameTime)
		{
			if ((CannonHealth.TargetValue >= 1 || Fraction.IsNeutral) && controller.DoBarrelRecharge())
			{
				barrelCharge += BARREL_CHARGE_SPEED * Fraction.Multiplicator * (1 + TotalBoost) * gameTime.GetElapsedSeconds();

				if (barrelCharge >= 1f)
				{
					barrelCharge -= 1f;

					Shoot();
				}
			}

			if (barrelRecoil < 1)
			{
				barrelRecoil = FloatMath.LimitedInc(barrelRecoil, BARREL_RECOIL_SPEED * Fraction.Multiplicator * (1 + TotalBoost) * gameTime.GetElapsedSeconds(), 1f);
			}
		}

		private void Shoot()
		{
			var position = GetBulletSpawnPoint();
			var velocity = GetBulletVelocity();

			//Owner.PushNotification($"Cannon :: Shoot ({position.X:000.0}|{position.Y:000.0}) at {FloatMath.ToDegree(velocity.ToAngle()):000}°");

			barrelRecoil = 0f;

			Manager.AddEntity(new Bullet(Owner, this, position, velocity, Scale));
		}

		private void UpdatePhysicBodies()
		{
			PhysicsBody.Rotation = Rotation.ActualValue;
		}

		public Vector2 GetBulletSpawnPoint()
		{
			return Center + new Vector2(Scale * (CANNON_DIAMETER/2 + Bullet.BULLET_DIAMETER * 0.66f), 0).Rotate(Rotation.ActualValue);
		}

		public Vector2 GetBulletVelocity()
		{
			var variance = FloatMath.GetRangedRandom(-BULLET_ANGLE_VARIANCE, +BULLET_ANGLE_VARIANCE);
			var angle = FloatMath.AddRads(Rotation.ActualValue, variance);

			return new Vector2(1, 0).Rotate(angle) * BULLET_INITIAL_SPEED;
		}

		#endregion

		#region Draw

		public override void Draw(SpriteBatch sbatch)
		{
			DrawBodyAndBarrel(sbatch);
			DrawCog(sbatch);

#if DEBUG
			DrawDebugView(sbatch);

			// ASSERTION
			if (ActiveOperations.Count(p => p is CannonBooster) != FloatMath.Round(TotalBoost/BOOSTER_POWER)) throw new Exception("Assertion failed TotalBoost == Boosters");
#endif
		}

		private void DrawBodyAndBarrel(SpriteBatch sbatch)
		{
			var recoil = (1-barrelRecoil) * BARREL_RECOIL_LENGTH;

			sbatch.Draw(
				Textures.TexCannonBarrelShadow.Texture,
				Center,
				Textures.TexCannonBarrelShadow.Bounds,
				Color.White,
				Rotation.ActualValue,
				new Vector2(-16 + recoil, 48),
				Scale*Textures.DEFAULT_TEXTURE_SCALE,
				SpriteEffects.None,
				0);

			sbatch.Draw(
				Textures.TexCannonBodyShadow.Texture,
				Center,
				Textures.TexCannonBodyShadow.Bounds,
				Color.White,
				Rotation.ActualValue,
				new Vector2(Textures.TexCannonBodyShadow.Width/2f, Textures.TexCannonBodyShadow.Height/2f),
				Scale*Textures.DEFAULT_TEXTURE_SCALE,
				SpriteEffects.None,
				0);

			sbatch.Draw(
				Textures.TexCannonBarrel.Texture,
				Center,
				Textures.TexCannonBarrel.Bounds,
				Color.White,
				Rotation.ActualValue,
				new Vector2(-32 + recoil, 32),
				Scale*Textures.DEFAULT_TEXTURE_SCALE,
				SpriteEffects.None,
				0);

			sbatch.Draw(
				Textures.TexCannonBody.Texture,
				Center,
				Textures.TexCannonBody.Bounds,
				Color.White,
				Rotation.ActualValue,
				new Vector2(Textures.TexCannonBody.Width/2f, Textures.TexCannonBody.Height/2f),
				Scale*Textures.DEFAULT_TEXTURE_SCALE,
				SpriteEffects.None,
				0);
		}

		private void DrawCog(SpriteBatch sbatch)
		{
			TextureRegion2D texBack = Textures.AnimCannonCog[Textures.ANIMATION_CANNONCOG_SIZE - 1];
			TextureRegion2D texProg = Textures.AnimCannonCog[(int)(CannonHealth.ActualValue * (Textures.ANIMATION_CANNONCOG_SIZE - 1))];

			sbatch.Draw(
				texBack.Texture,
				Center,
				texBack.Bounds,
				FlatColors.Clouds,
				cannonCogRotation + 3 * (FloatMath.PI / 2),
				new Vector2(0.5f * texBack.Width, 0.5f * texBack.Height),
				Scale * Textures.DEFAULT_TEXTURE_SCALE,
				SpriteEffects.None,
				0);

			sbatch.Draw(
				texProg.Texture,
				Center,
				texProg.Bounds,
				Fraction.Color,
				cannonCogRotation + 3 * (FloatMath.PI / 2),
				new Vector2(0.5f * texProg.Width, 0.5f * texProg.Height),
				Scale * Textures.DEFAULT_TEXTURE_SCALE,
				SpriteEffects.None,
				0);
		}

#if DEBUG
		private void DrawDebugView(SpriteBatch sbatch)
		{
			var innerRadius = Scale*CANNON_DIAMETER/2;

			var rectChargeFull = new RectangleF(Center.X - innerRadius, Center.Y + innerRadius + (0 * 12) + 4, innerRadius * 2, 8);
			var rectChargeProg = new RectangleF(Center.X - innerRadius, Center.Y + innerRadius + (0 * 12) + 4, innerRadius * 2 * barrelCharge, 8);

			sbatch.FillRectangle(rectChargeFull, Color.White);
			sbatch.FillRectangle(rectChargeProg, Color.DarkGray);
			sbatch.DrawRectangle(rectChargeFull, Color.Black);

			var rectHealthFull = new RectangleF(Center.X - innerRadius, Center.Y + innerRadius + (1 * 12) + 4, innerRadius * 2, 8);
			var rectHealthProgT = new RectangleF(Center.X - innerRadius, Center.Y + innerRadius + (1 * 12) + 4, innerRadius * 2 * CannonHealth.TargetValue, 8);
			var rectHealthProgA = new RectangleF(Center.X - innerRadius, Center.Y + innerRadius + (1 * 12) + 4, innerRadius * 2 * CannonHealth.ActualValue, 8);

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
				var rectFull = new RectangleF(Center.X - innerRadius, Center.Y + innerRadius + ((i+2) * 12) + 16, innerRadius * 2, 8);
				var rectProg = new RectangleF(Center.X - innerRadius, Center.Y + innerRadius + ((i+2) * 12) + 16, innerRadius * 2 * (1- ActiveOperations[i].Progress), 8);

				sbatch.FillRectangle(rectFull, Color.White);
				sbatch.FillRectangle(rectProg, Color.Chocolate);
				sbatch.DrawRectangle(rectFull, Color.Black);
			}
		}
#endif

		#endregion

		#region Change State

		public void ResetChargeAndBooster()
		{
			barrelCharge = 0f;

			RemoveAllOperations(o => o is CannonBooster);
		}

		public void ApplyBoost()
		{
			if (Fraction.IsNeutral) return;

			CannonHealth.Inc(HEALTH_HIT_GEN);
			if (CannonHealth.Limit(0f, 1f) == 1)
			{
				AddEntityOperation(new CannonBooster(BOOSTER_POWER, 1/(BOOSTER_LIFETIME_MULTIPLIER * Fraction.Multiplicator))); //TODO This means the length goes up for lesser multiplicators? wtf
			}
		}

		public void TakeDamage(Fraction source)
		{
			if (source.IsNeutral)
			{
				ResetChargeAndBooster();
			}
			else if (Fraction.IsNeutral)
			{
				SetFraction(source);
				CannonHealth.Set(HEALTH_HIT_GEN);
			}
			else
			{
				CannonHealth.Dec(HEALTH_HIT_DROP);

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
			}
		}

		private void SetFraction(Fraction f)
		{
			Fraction = f;
			ResetChargeAndBooster();
			controller = Fraction.CreateController(Owner, this);
		}

		public void RotateTo(GDEntity target)
		{
			Rotation.Set(FloatMath.PositiveAtan2(target.Position.Y - Center.Y, target.Position.X - Center.X));
			//Owner.PushNotification($"Cannon :: target({FloatMath.ToDegree(Rotation.TargetValue):000}°)");
		}

		#endregion
	}
}
