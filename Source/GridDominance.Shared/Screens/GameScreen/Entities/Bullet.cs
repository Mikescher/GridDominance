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
		private const float SELF_PRESERVANCE_TIME = 0.5f;

		public readonly Fraction Fraction;

		public bool IsDying;

		public Sprite SpriteBullet;
		public Body PhysicsBody;
		public readonly Cannon Source;
		private readonly float scale;

		private readonly Vector2 initialPosition;
		private readonly Vector2 initialVelocity;

		public override Vector2 Position => SpriteBullet.Position;

		public Bullet(GameScreen scrn, Cannon shooter, Vector2 pos, Vector2 velo, float entityScale)
			: base(scrn)
		{
			initialPosition = pos;
			initialVelocity = velo;
			Source = shooter;
			Fraction = Source.Fraction;
			scale = entityScale;
		}

		public override void OnInitialize()
		{
			SpriteBullet = new Sprite(Textures.TexBullet)
			{
				Scale = scale * Textures.DEFAULT_TEXTURE_SCALE,
				Position = initialPosition,
				Color = Fraction.Color
			};

			PhysicsBody = BodyFactory.CreateCircle(Manager.PhysicsWorld, ConvertUnits.ToSimUnits(scale * BULLET_DIAMETER / 2), 1, ConvertUnits.ToSimUnits(initialPosition), BodyType.Dynamic, this);
			PhysicsBody.LinearVelocity = ConvertUnits.ToSimUnits(initialVelocity);
			PhysicsBody.CollidesWith = Category.All;
			PhysicsBody.IsBullet = true;
			PhysicsBody.Restitution = 0.95f;
			PhysicsBody.AngularDamping = 0.5f;
			PhysicsBody.Friction = 0.2f;
			PhysicsBody.LinearDamping = 0f;
			PhysicsBody.OnCollision += OnCollision;
			PhysicsBody.AngularVelocity = FloatMath.GetRangedRandom(-FloatMath.PI, +FloatMath.PI);
			//Body.Mass = Scale * Scale; // Weight dependent on size
		}

		private bool OnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
		{
			if (!Alive || IsDying) return false;

			var otherBullet = fixtureB.UserData as Bullet;
			if (otherBullet != null)
			{
				if (otherBullet.Fraction == Fraction) return true;

				otherBullet.MutualDestruct();
				MutualDestruct();
				return false;
			}

			var otherCannon = fixtureB.UserData as Cannon;
			if (otherCannon != null)
			{
				if (otherCannon == Source && Lifetime < SELF_PRESERVANCE_TIME)
				{
					return false;
				}

				Disintegrate();

				if (otherCannon.Fraction == Fraction)
				{
					otherCannon.ApplyBoost();
				}
				else // if (otherCannon.Fraction != this.Fraction)
				{
					otherCannon.TakeDamage(Fraction);
				}

				return false;
			}
			
			// wud ???
			Owner.PushErrorNotification(string.Format("Bullet collided with unkown fixture: {0}", fixtureB.UserData ?? "<NULL>"));
			return false;
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
			Manager.PhysicsWorld.RemoveBody(PhysicsBody);
		}

		protected override void OnUpdate(GameTime gameTime, InputState istate)
		{
			SpriteBullet.Position = ConvertUnits.ToDisplayUnits(PhysicsBody.Position);
			SpriteBullet.Rotation = PhysicsBody.Rotation;

			if (Lifetime > MAXIMUM_LIEFTIME) AddEntityOperation(new BulletFadeAndDieOperation());

			if (!Manager.BoundingBox.Contains(SpriteBullet.Position)) Remove();
		}

		public override void Draw(SpriteBatch sbatch)
		{
			sbatch.Draw(SpriteBullet);
		}
	}
}
