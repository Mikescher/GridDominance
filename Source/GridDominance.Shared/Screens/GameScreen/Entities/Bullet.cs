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
		public  const float BULLET_DIAMETER = 25;
		private const float MAXIMUM_LIEFTIME = 25;

		public readonly Fraction Fraction;

		public bool IsDying;

		public Vector2 BulletPosition;
		public float BulletRotation = 0f;
		public float BulletAlpha = 1f;

		public Body PhysicsBody;
		public readonly Cannon Source;
		private readonly float scale;
		
		private readonly Vector2 initialVelocity;

		public override Vector2 Position => BulletPosition;

		public Bullet(GameScreen scrn, Cannon shooter, Vector2 pos, Vector2 velo, float entityScale)
			: base(scrn)
		{
			BulletPosition = pos;
			initialVelocity = velo;
			Source = shooter;
			Fraction = Source.Fraction;
			scale = entityScale;
		}

		public override void OnInitialize()
		{
			PhysicsBody = BodyFactory.CreateCircle(Manager.PhysicsWorld, ConvertUnits.ToSimUnits(scale * BULLET_DIAMETER / 2), 1, ConvertUnits.ToSimUnits(BulletPosition), BodyType.Dynamic, this);
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

				if (otherCannon.Fraction == Fraction)
				{
					// if Source barrel then ignore collision
					if (otherCannon != Source || fixtureB == otherCannon.PhysicsFictureBase)
					{
						Disintegrate();
						otherCannon.ApplyBoost();
					}
				}
				else // if (otherCannon.Fraction != this.Fraction)
				{
					Disintegrate();
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
			BulletPosition = ConvertUnits.ToDisplayUnits(PhysicsBody.Position);
			BulletRotation = PhysicsBody.Rotation;

			if (Lifetime > MAXIMUM_LIEFTIME) AddEntityOperation(new BulletFadeAndDieOperation());

			if (!Manager.BoundingBox.Contains(BulletPosition)) Remove();
		}

		public override void Draw(SpriteBatch sbatch)
		{
			sbatch.Draw(
				Textures.TexBullet.Texture, 
				BulletPosition,
				Textures.TexBullet.Bounds, 
				Fraction.Color * BulletAlpha,
				BulletRotation, 
				new Vector2(Textures.TexBullet.Width/2f, Textures.TexBullet.Height/2f), 
				scale * Textures.DEFAULT_TEXTURE_SCALE, 
				SpriteEffects.None, 
				0);
		}
	}
}
