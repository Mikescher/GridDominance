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

namespace GridDominance.Shared.Screens.GameScreen.Entities
{
	class Cannon : GDEntity
	{
		private const float ROTATION_SPEED = FloatMath.TAU / 2; // 3.141 rad/sec

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

		private bool isMouseDragging = false;

		private float barrelCharge = 0f;
		private float cannonHealth = 1f;

		private float actualRotation = 0;   // radians
		private float targetRotation = 0;   // radians

		private Vector2 center;
		private CircleF innerBoundings;

		private Body body;

		public Cannon(GameScreen scrn, float posX, float posY, Fraction cannonfraction) : base(scrn)
		{
			Fraction = cannonfraction;

			center = new Vector2(posX, posY);

			innerBoundings = new CircleF(center, CANNON_DIAMETER / 2);
			cannonHealth = Fraction.IsNeutral ? 0f : 1f;
		}

		public Cannon(GameScreen scrn, LPCannon blueprint, Fraction[] fractions) : base(scrn)
		{
			Fraction = fractions[blueprint.Player];

			center = new Vector2(blueprint.X, blueprint.Y);

			actualRotation = FloatMath.ToRadians(blueprint.Rotation);
			targetRotation = FloatMath.ToRadians(blueprint.Rotation);

			innerBoundings = new CircleF(center, CANNON_DIAMETER / 2);
			cannonHealth = Fraction.IsNeutral ? 0f : 1f;
		}

		public override void OnInitialize()
		{
			spriteBody = new Sprite(Textures.TexCannonBody)
			{
				Scale = Textures.DEFAULT_TEXTURE_SCALE,
				Position = center,
				Color = Fraction.Color,
			};

			spriteBarrel = new Sprite(Textures.TexCannonBarrel)
			{
				Scale = Textures.DEFAULT_TEXTURE_SCALE,
				Position = center,
				Origin = new Vector2(-32, 32),
				Color = Fraction.Color,
			};

			body = BodyFactory.CreateCircle(Manager.PhysicsWorld, ConvertUnits.ToSimUnits(CANNON_DIAMETER / 2), 1, ConvertUnits.ToSimUnits(center), BodyType.Static, this);
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
			if (cannonHealth < 1 && cannonHealth > 0)
			{
				var bonus = START_HEALTH_REGEN + (END_HEALTH_REGEN - START_HEALTH_REGEN) * cannonHealth;

				cannonHealth += bonus * gameTime.GetElapsedSeconds();
				if (cannonHealth > 1) cannonHealth = 1;
			}
		}

		private void UpdateBarrel(GameTime gameTime)
		{
			if (cannonHealth >= 1)
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

			Manager.AddEntity(new Bullet(Owner, this, position, velocity));
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

				Owner.PushNotification($"Cannon :: target({FloatMath.ToDegree(targetRotation):000}°)");
			}
			else if (isMouseDragging && istate.IsDown && !innerBoundings.Contains(istate.PointerPosition))
			{
				targetRotation = FloatMath.PositiveAtan2(istate.PointerPosition.Y - center.Y, istate.PointerPosition.X - center.X);
			}

			// ReSharper disable once CompareOfFloatsByEqualityOperator
			if (actualRotation != targetRotation)
			{
				var radSpeed = ROTATION_SPEED * gameTime.GetElapsedSeconds();
				var diff = FloatMath.DiffRadians(actualRotation, targetRotation);

				actualRotation = Math.Abs(diff) <= radSpeed ? targetRotation : FloatMath.AddRads(actualRotation, -FloatMath.Sign(diff) * radSpeed);
				body.Rotation = actualRotation;
			}

			spriteBody.Rotation = actualRotation;
			spriteBarrel.Rotation = actualRotation;
		}

		public Vector2 GetBulletSpawnPoint()
		{
			return center + new Vector2(64, 0).Rotate(actualRotation);
		}

		public Vector2 GetBulletVelocity()
		{
			var variance = FloatMath.GetRangedRandom(-BULLET_ANGLE_VARIANCE, +BULLET_ANGLE_VARIANCE);
			var angle = FloatMath.AddRads(actualRotation, variance);

			return new Vector2(1, 0).Rotate(angle) * BULLET_INITIAL_SPEED;
		}

		#endregion

		#region Draw

		public override void Draw(SpriteBatch sbatch)
		{
			sbatch.Draw(spriteBarrel);
			sbatch.Draw(spriteBody);

#if DEBUG
			DrawDebugView(sbatch);

			// ASSERTION
			if (ActiveOperations.Count(p => p is CannonBooster) != FloatMath.Round(TotalBoost/BOOSTER_POWER)) throw new Exception("Assertion failed TotalBoost == Boosters");
#endif
		}

		private void DrawDebugView(SpriteBatch sbatch)
		{
			var rectChargeFull = new RectangleF(center.X - CANNON_DIAMETER / 2, center.Y + CANNON_DIAMETER / 2 + (0 * 12) + 4, CANNON_DIAMETER * 1, 8);
			var rectChargeProg = new RectangleF(center.X - CANNON_DIAMETER / 2, center.Y + CANNON_DIAMETER / 2 + (0 * 12) + 4, CANNON_DIAMETER * barrelCharge, 8);

			sbatch.FillRectangle(rectChargeFull, Color.White);
			sbatch.FillRectangle(rectChargeProg, Color.DarkGray);
			sbatch.DrawRectangle(rectChargeFull, Color.Black);

			var rectHealthFull = new RectangleF(center.X - CANNON_DIAMETER / 2, center.Y + CANNON_DIAMETER / 2 + (1 * 12) + 4, CANNON_DIAMETER * 1, 8);
			var rectHealthProg = new RectangleF(center.X - CANNON_DIAMETER / 2, center.Y + CANNON_DIAMETER / 2 + (1 * 12) + 4, CANNON_DIAMETER * cannonHealth, 8);

			sbatch.FillRectangle(rectHealthFull, Color.White);
			sbatch.FillRectangle(rectHealthProg, Fraction.Color);
			sbatch.DrawRectangle(rectHealthFull, Color.Black);

			for (int i = 0; i < ActiveOperations.Count; i++)
			{
				var rectFull = new RectangleF(center.X - CANNON_DIAMETER / 2, center.Y + CANNON_DIAMETER / 2 + ((i+2) * 12) + 16, CANNON_DIAMETER * 1, 8);
				var rectProg = new RectangleF(center.X - CANNON_DIAMETER / 2, center.Y + CANNON_DIAMETER / 2 + ((i+2) * 12) + 16, CANNON_DIAMETER * (1- ActiveOperations[i].Progress), 8);

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

			cannonHealth += HEALTH_HIT_GEN;
			if (cannonHealth >= 1)
			{
				cannonHealth = 1;
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
				cannonHealth = HEALTH_HIT_GEN;
			}
			else
			{
				cannonHealth -= HEALTH_HIT_DROP;

				// ReSharper disable once CompareOfFloatsByEqualityOperator
				if (cannonHealth == 0)
				{
					// Never tell me the odds

					SetFraction(Fraction.GetNeutral());
					ResetChargeAndBooster();
				}
				else if (cannonHealth < 0)
				{
					SetFraction(source);
					cannonHealth = FloatMath.Abs(cannonHealth);
					ResetChargeAndBooster();
				}
			}
		}

		private void SetFraction(Fraction f)
		{
			Fraction = f;

			spriteBody.Color = f.Color;
			spriteBarrel.Color = f.Color;
		}

		#endregion
	}
}
