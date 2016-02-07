using System;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using GridDominance.Shared.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using GridDominance.Shared.Framework;
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

		public readonly Fraction Fraction;

		private Sprite spriteBody;
		private Sprite spriteBarrel;

		private bool isMouseDragging = false;

		private float barrelCharge = 0f;

		private float actualRotation = 0;   // radians
		private float targetRotation = 0;   // radians

		private Vector2 center;
		private CircleF innerBoundings;

		private Body body;

		public Cannon(GameScreen scrn, int posX, float posY, Fraction cannonfraction)
			: base(scrn)
		{
			Fraction = cannonfraction;

			center = new Vector2(posX, posY);
			innerBoundings = new CircleF(center, CANNON_DIAMETER / 2);
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

			body = BodyFactory.CreateCircle(Manager.PhysicsWorld, ConvertUnits.ToSimUnits(CANNON_DIAMETER /2), 1, ConvertUnits.ToSimUnits(center), BodyType.Static, this);
		}

		public override void OnRemove()
		{
			Manager.PhysicsWorld.RemoveBody(body);
		}

		public override void OnUpdate(GameTime gameTime, InputState istate)
		{
			UpdateRotation(gameTime, istate);
			UpdateBarrel(gameTime);
		}

		private void UpdateBarrel(GameTime gameTime)
		{
			barrelCharge += BARREL_CHARGE_SPEED * gameTime.GetElapsedSeconds();

			if (barrelCharge >= 1f)
			{
				barrelCharge -= 1f;

				Shoot();
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

		public override void Draw(SpriteBatch sbatch)
		{
			sbatch.Draw(spriteBarrel);
			sbatch.Draw(spriteBody);
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
	}
}
