using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using GridDominance.Shared.Framework;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.GameScreen.EntityOperations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;

namespace GridDominance.Shared.Screens.GameScreen.Entities
{
	class Bullet : GDEntity
	{
		private const float BULLET_DIAMETER = 25;
		private const float MAXIMUM_LIEFTIME = 25;

		public readonly Fraction Fraction;

		public bool IsDying = false;

		public  Sprite SpriteBullet;
		private Body body;
		private readonly float scale;

		private readonly Vector2 initial_position;
		private readonly Vector2 initial_velocity;


		public Bullet(GameScreen scrn, Cannon shooter, Vector2 pos, Vector2 velo, float _scale)
			: base(scrn)
		{
			initial_position = pos;
			initial_velocity = velo;
			Fraction = shooter.Fraction;
			scale = _scale;
		}

		public override void OnInitialize()
		{
			SpriteBullet = new Sprite(Textures.TexBullet)
			{
				Scale = scale * Textures.DEFAULT_TEXTURE_SCALE,
				Position = initial_position,
				Color = Fraction.Color,
			};

			body = BodyFactory.CreateCircle(Manager.PhysicsWorld, ConvertUnits.ToSimUnits(scale * BULLET_DIAMETER / 2), 1, ConvertUnits.ToSimUnits(initial_position), BodyType.Dynamic, this);
			body.LinearVelocity = ConvertUnits.ToSimUnits(initial_velocity);
			body.CollidesWith = Category.All;
			body.IsBullet = true;
			body.Restitution = 0.95f;
			body.AngularDamping = 0.5f;
			body.Friction = 0.2f;
			body.LinearDamping = 0f;
			body.OnCollision += OnCollision;
			body.AngularVelocity = FloatMath.GetRangedRandom(-FloatMath.PI, +FloatMath.PI);
			//body.Mass = scale * scale; // Weight dependent on size
		}

		private bool OnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
		{
			if (!Alive || IsDying) return false;

			var otherBullet = fixtureB.UserData as Bullet;
			if (otherBullet != null)
			{
				if (otherBullet.Fraction == this.Fraction) return true;

				otherBullet.MutualDestruct();
				this.MutualDestruct();
				return false;
			}

			var otherCannon = fixtureB.UserData as Cannon;
			if (otherCannon != null)
			{
				this.Disintegrate();

				if (otherCannon.Fraction == this.Fraction)
				{
					otherCannon.ApplyBoost();
				}
				else // if (otherCannon.Fraction != this.Fraction)
				{
					otherCannon.TakeDamage(this.Fraction);
				}

				return false;
			}

			// wud ???
			return true;
		}

		private void MutualDestruct()
		{
			// After Bullet-Bulllet Collision

			AddEntityOperation(new BulletSplatterAndDieOperation());
			IsDying = true;
		}

		private void Disintegrate()
		{
			// After Bullet-Cannon Collision

			AddEntityOperation(new BulletConsumeAndDieOperation());
			IsDying = true;
		}

		public override void OnRemove()
		{
			Manager.PhysicsWorld.RemoveBody(body);
		}

		public override void OnUpdate(GameTime gameTime, InputState istate)
		{
			SpriteBullet.Position = ConvertUnits.ToDisplayUnits(body.Position);
			SpriteBullet.Rotation = body.Rotation;

			if (Lifetime > MAXIMUM_LIEFTIME) AddEntityOperation(new BulletFadeAndDieOperation());

			if (!Manager.BoundingBox.Contains(SpriteBullet.Position)) Remove();
		}

		public override void Draw(SpriteBatch sbatch)
		{
			sbatch.Draw(SpriteBullet);
		}
	}
}
