using System;
using System.Linq;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using GridDominance.Levelformat.Parser;
using GridDominance.Shared.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using GridDominance.Shared.Framework;
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

		private const float BARREL_CHARGE_SPEED = 0.9f;
		public  const float CANNON_DIAMETER = 96;
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

		private Sprite spriteBody;
		private Sprite spriteBarrel;
		private Sprite spriteBodyShadow;
		private Sprite spriteBarrelShadow;

		private float barrelCharge = 0f;
		private readonly DeltaLimitedFloat cannonHealth = new DeltaLimitedFloat(1f, HEALTH_PROGRESS_SPEED);

		public readonly DeltaLimitedModuloFloat Rotation;

		public readonly Vector2 Center;
		public readonly float Scale;

		private readonly Vector2 cannonCogOrigin;
		private float cannonCogRotation;

		private Body body;

		public Cannon(GameScreen scrn, LPCannon blueprint, Fraction[] fractions) : base(scrn)
		{
			Fraction = fractions[blueprint.Player];

			Center = new Vector2(blueprint.X, blueprint.Y);
			Scale = blueprint.Scale;
			
			Rotation = new DeltaLimitedModuloFloat(FloatMath.ToRadians(blueprint.Rotation), ROTATION_SPEED, FloatMath.TAU);
			
			cannonHealth.SetForce(Fraction.IsNeutral ? 0f : 1f);

			cannonCogOrigin = new Vector2(0.5f * Textures.AnimCannonCog[0].Width, 0.5f * Textures.AnimCannonCog[0].Height); // better cache than sorry
		}

		public override void OnInitialize()
		{
			controller = Fraction.CreateController(Owner, this);

			spriteBody = new Sprite(Textures.TexCannonBody)
			{
				Scale = Scale * Textures.DEFAULT_TEXTURE_SCALE,
				Position = Center,
				Color = FlatColors.Silver,
			};

			spriteBodyShadow = new Sprite(Textures.TexCannonBodyShadow)
			{
				Scale = Scale * Textures.DEFAULT_TEXTURE_SCALE,
				Position = Center,
			};

			spriteBarrel = new Sprite(Textures.TexCannonBarrel)
			{
				Scale = Scale * Textures.DEFAULT_TEXTURE_SCALE,
				Position = Center,
				Origin = new Vector2(-32, 32),
				Color = FlatColors.Silver,
			};

			spriteBarrelShadow = new Sprite(Textures.TexCannonBarrelShadow)
			{
				Scale = Scale * Textures.DEFAULT_TEXTURE_SCALE,
				Position = Center,
				Origin = new Vector2(-16, 48),
			};

			body = BodyFactory.CreateCircle(Manager.PhysicsWorld, ConvertUnits.ToSimUnits(Scale * CANNON_DIAMETER / 2), 1, ConvertUnits.ToSimUnits(Center), BodyType.Static, this);
		}

		public override void OnRemove()
		{
			Manager.PhysicsWorld.RemoveBody(body);
		}

		#region Update

		public override void OnUpdate(GameTime gameTime, InputState istate)
		{
			controller.Update(gameTime, istate);

			UpdateRotation(gameTime);
			UpdateHealth(gameTime);
			UpdateBarrel(gameTime);
		}

		private void UpdateHealth(GameTime gameTime)
		{
			cannonHealth.Update(gameTime);

			if (cannonHealth.TargetValue < 1 && cannonHealth.TargetValue > 0)
			{
				var bonus = START_HEALTH_REGEN + (END_HEALTH_REGEN - START_HEALTH_REGEN) * cannonHealth.TargetValue;

				cannonHealth.Inc(bonus * gameTime.GetElapsedSeconds());
				cannonHealth.Limit(0f, 1f);
			}

			if (cannonHealth.ActualValue >= 1 || (cannonHealth.ActualValue <= 0 && Fraction.IsNeutral))
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
			if (cannonHealth.TargetValue >= 1)
			{
				barrelCharge += BARREL_CHARGE_SPEED * Fraction.Multiplicator * (1 + TotalBoost) * gameTime.GetElapsedSeconds();

				if (barrelCharge >= 1f)
				{
					barrelCharge -= 1f;

					Shoot();
				}
			}
		}

		private void Shoot()
		{
			var position = GetBulletSpawnPoint();
			var velocity = GetBulletVelocity();

			//Owner.PushNotification($"Cannon :: Shoot ({position.X:000.0}|{position.Y:000.0}) at {FloatMath.ToDegree(velocity.ToAngle()):000}°");

			Manager.AddEntity(new Bullet(Owner, this, position, velocity, Scale));
		}

		private void UpdateRotation(GameTime gameTime)
		{
			Rotation.Update(gameTime);

			body.Rotation = Rotation.ActualValue;

			spriteBody.Rotation = Rotation.ActualValue;
			spriteBodyShadow.Rotation = Rotation.ActualValue;

			spriteBarrel.Rotation = Rotation.ActualValue;
			spriteBarrelShadow.Rotation = Rotation.ActualValue;
		}

		public Vector2 GetBulletSpawnPoint()
		{
			return Center + new Vector2(64, 0).Rotate(Rotation.ActualValue);
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
			sbatch.Draw(spriteBarrelShadow);
			sbatch.Draw(spriteBodyShadow);

			sbatch.Draw(spriteBarrel);
			sbatch.Draw(spriteBody);

			DrawCog(sbatch);

#if DEBUG
			DrawDebugView(sbatch);

			// ASSERTION
			if (ActiveOperations.Count(p => p is CannonBooster) != FloatMath.Round(TotalBoost/BOOSTER_POWER)) throw new Exception("Assertion failed TotalBoost == Boosters");
#endif
		}

		private void DrawCog(SpriteBatch sbatch)
		{
			TextureRegion2D texBack = Textures.AnimCannonCog[Textures.ANIMATION_CANNONCOG_SIZE - 1];
			TextureRegion2D texProg = Textures.AnimCannonCog[(int)(cannonHealth.ActualValue * (Textures.ANIMATION_CANNONCOG_SIZE - 1))];

			sbatch.Draw(
				texBack.Texture,
				Center,
				texBack.Bounds,
				FlatColors.Clouds,
				cannonCogRotation + 3 * (FloatMath.PI / 2),
				cannonCogOrigin,
				Scale * Textures.DEFAULT_TEXTURE_SCALE,
				SpriteEffects.None,
				0);

			sbatch.Draw(
				texProg.Texture,
				Center,
				texProg.Bounds,
				Fraction.Color,
				cannonCogRotation + 3 * (FloatMath.PI / 2),
				cannonCogOrigin, 
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
			var rectHealthProgT = new RectangleF(Center.X - innerRadius, Center.Y + innerRadius + (1 * 12) + 4, innerRadius * 2 * cannonHealth.TargetValue, 8);
			var rectHealthProgA = new RectangleF(Center.X - innerRadius, Center.Y + innerRadius + (1 * 12) + 4, innerRadius * 2 * cannonHealth.ActualValue, 8);

			if (cannonHealth.IsDecreasing())
			{
				sbatch.FillRectangle(rectHealthFull, Color.White);
				sbatch.FillRectangle(rectHealthProgA, Fraction.Color.Lighten());
				sbatch.FillRectangle(rectHealthProgT, Fraction.Color);
				sbatch.DrawRectangle(rectHealthFull, Color.Black);
			}
			else if (cannonHealth.IsIncreasing())
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

			cannonHealth.Inc(HEALTH_HIT_GEN);
			if (cannonHealth.Limit(0f, 1f) == 1)
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
				ResetChargeAndBooster();
				cannonHealth.Set(HEALTH_HIT_GEN);
			}
			else
			{
				cannonHealth.Dec(HEALTH_HIT_DROP);

				if (FloatMath.IsZero(cannonHealth.TargetValue))
				{
					// Never tell me the odds

					SetFraction(Fraction.GetNeutral());
					ResetChargeAndBooster();
				}
				else if (cannonHealth.TargetValue < 0)
				{
					SetFraction(source);
					cannonHealth.Set(FloatMath.Abs(cannonHealth.TargetValue));
					ResetChargeAndBooster();
				}
			}
		}

		private void SetFraction(Fraction f)
		{
			Fraction = f;
			controller = Fraction.CreateController(Owner, this);
		}

		#endregion
	}
}
