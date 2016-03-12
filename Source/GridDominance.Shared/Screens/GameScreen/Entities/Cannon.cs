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
		private const float CANNON_DIAMETER = 96;
		private const float BULLET_ANGLE_VARIANCE = 0.035f; // ~ 2 degree
		private const float BULLET_INITIAL_SPEED = 100f;

		private const float START_HEALTH_REGEN = 0.015f; // Health per sec bei 0HP
		private const float END_HEALTH_REGEN   = 0.105f; // Health per sec bei 1HP

		private const float BOOSTER_POWER = 0.5f;
		private const float BOOSTER_LIFETIME_MULTIPLIER = 0.9f;

		private const float HEALTH_HIT_DROP = 0.20f; // on Hit
		private const float HEALTH_HIT_GEN  = 0.20f; // on Hit from own fraction

		public Fraction Fraction;
		public float TotalBoost = 0f;

		private Sprite spriteBody;
		private Sprite spriteBarrel;
		private Sprite spriteBodyShadow;
		private Sprite spriteBarrelShadow;

		private bool isMouseDragging = false;

		private float barrelCharge = 0f;
		private readonly DeltaLimitedFloat cannonHealth = new DeltaLimitedFloat(1f, HEALTH_PROGRESS_SPEED);

		private readonly DeltaLimitedModuloFloat rotation;

		private readonly Vector2 center;
		private readonly CircleF innerBoundings;
		private readonly float scale;

		private readonly Vector2 cannonCogOrigin;
		private float cannonCogRotation;

		private Body body;

		public Cannon(GameScreen scrn, LPCannon blueprint, Fraction[] fractions) : base(scrn)
		{
			Fraction = fractions[blueprint.Player];

			center = new Vector2(blueprint.X, blueprint.Y);
			scale = blueprint.Scale;
			
			rotation = new DeltaLimitedModuloFloat(FloatMath.ToRadians(blueprint.Rotation), ROTATION_SPEED, FloatMath.TAU);

			innerBoundings = new CircleF(center, scale * CANNON_DIAMETER / 2);
			cannonHealth.SetForce(Fraction.IsNeutral ? 0f : 1f);

			cannonCogOrigin = new Vector2(0.5f * Textures.AnimCannonCog[0].Width, 0.5f * Textures.AnimCannonCog[0].Height); // better cache than sorry
		}

		public override void OnInitialize()
		{
			spriteBody = new Sprite(Textures.TexCannonBody)
			{
				Scale = scale * Textures.DEFAULT_TEXTURE_SCALE,
				Position = center,
				Color = FlatColors.Silver,
			};

			spriteBodyShadow = new Sprite(Textures.TexCannonBodyShadow)
			{
				Scale = scale * Textures.DEFAULT_TEXTURE_SCALE,
				Position = center,
			};

			spriteBarrel = new Sprite(Textures.TexCannonBarrel)
			{
				Scale = scale * Textures.DEFAULT_TEXTURE_SCALE,
				Position = center,
				Origin = new Vector2(-32, 32),
				Color = FlatColors.Silver,
			};

			spriteBarrelShadow = new Sprite(Textures.TexCannonBarrelShadow)
			{
				Scale = scale * Textures.DEFAULT_TEXTURE_SCALE,
				Position = center,
				Origin = new Vector2(-16, 48),
			};

			body = BodyFactory.CreateCircle(Manager.PhysicsWorld, ConvertUnits.ToSimUnits(scale * CANNON_DIAMETER / 2), 1, ConvertUnits.ToSimUnits(center), BodyType.Static, this);
		}

		public override void OnRemove()
		{
			Manager.PhysicsWorld.RemoveBody(body);
		}

		#region Update

		public override void OnUpdate(GameTime gameTime, InputState istate)
		{
			UpdateRotation(gameTime, istate);

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

			Manager.AddEntity(new Bullet(Owner, this, position, velocity, scale));
		}

		private void UpdateRotation(GameTime gameTime, InputState istate)
		{
			if (istate.IsJustDown && innerBoundings.Contains(istate.PointerPosition))
			{
				isMouseDragging = true;
			}
			else if (!istate.IsDown && isMouseDragging)
			{
				isMouseDragging = false;

				Owner.PushNotification($"Cannon :: target({FloatMath.ToDegree(rotation.TargetValue):000}°)");
			}
			else if (isMouseDragging && istate.IsDown && !innerBoundings.Contains(istate.PointerPosition))
			{
				rotation.Set(FloatMath.PositiveAtan2(istate.PointerPosition.Y - center.Y, istate.PointerPosition.X - center.X));
			}

			rotation.Update(gameTime);

			body.Rotation = rotation.ActualValue;

			spriteBody.Rotation = rotation.ActualValue;
			spriteBodyShadow.Rotation = rotation.ActualValue;

			spriteBarrel.Rotation = rotation.ActualValue;
			spriteBarrelShadow.Rotation = rotation.ActualValue;
		}

		public Vector2 GetBulletSpawnPoint()
		{
			return center + new Vector2(64, 0).Rotate(rotation.ActualValue);
		}

		public Vector2 GetBulletVelocity()
		{
			var variance = FloatMath.GetRangedRandom(-BULLET_ANGLE_VARIANCE, +BULLET_ANGLE_VARIANCE);
			var angle = FloatMath.AddRads(rotation.ActualValue, variance);

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
				center,
				texBack.Bounds,
				FlatColors.Clouds,
				cannonCogRotation + 3 * (FloatMath.PI / 2),
				cannonCogOrigin,
				scale * Textures.DEFAULT_TEXTURE_SCALE,
				SpriteEffects.None,
				0);

			sbatch.Draw(
				texProg.Texture,
				center,
				texProg.Bounds,
				Fraction.Color,
				cannonCogRotation + 3 * (FloatMath.PI / 2),
				cannonCogOrigin, 
				scale * Textures.DEFAULT_TEXTURE_SCALE,
				SpriteEffects.None,
				0);
		}

		private void DrawDebugView(SpriteBatch sbatch)
		{
			var rectChargeFull = new RectangleF(center.X - innerBoundings.Radius, center.Y + innerBoundings.Radius + (0 * 12) + 4, innerBoundings.Diameter, 8);
			var rectChargeProg = new RectangleF(center.X - innerBoundings.Radius, center.Y + innerBoundings.Radius + (0 * 12) + 4, innerBoundings.Diameter * barrelCharge, 8);

			sbatch.FillRectangle(rectChargeFull, Color.White);
			sbatch.FillRectangle(rectChargeProg, Color.DarkGray);
			sbatch.DrawRectangle(rectChargeFull, Color.Black);

			var rectHealthFull = new RectangleF(center.X - innerBoundings.Radius, center.Y + innerBoundings.Radius + (1 * 12) + 4, innerBoundings.Diameter, 8);
			var rectHealthProgT = new RectangleF(center.X - innerBoundings.Radius, center.Y + innerBoundings.Radius + (1 * 12) + 4, innerBoundings.Diameter * cannonHealth.TargetValue, 8);
			var rectHealthProgA = new RectangleF(center.X - innerBoundings.Radius, center.Y + innerBoundings.Radius + (1 * 12) + 4, innerBoundings.Diameter * cannonHealth.ActualValue, 8);

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
				var rectFull = new RectangleF(center.X - innerBoundings.Radius, center.Y + innerBoundings.Radius + ((i+2) * 12) + 16, innerBoundings.Diameter, 8);
				var rectProg = new RectangleF(center.X - innerBoundings.Radius, center.Y + innerBoundings.Radius + ((i+2) * 12) + 16, innerBoundings.Diameter * (1- ActiveOperations[i].Progress), 8);

				sbatch.FillRectangle(rectFull, Color.White);
				sbatch.FillRectangle(rectProg, Color.Chocolate);
				sbatch.DrawRectangle(rectFull, Color.Black);
			}
		}

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

				if (cannonHealth.TargetValue == 0)
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
		}

		#endregion
	}
}
