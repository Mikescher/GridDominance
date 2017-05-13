using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.NormalGameScreen.EntityOperations;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.GameMath;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.GameMath.Geometry.Alignment;
using MonoSAMFramework.Portable.LogProtocol;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Entities;

namespace GridDominance.Shared.Screens.NormalGameScreen.Entities
{
	public class Bullet : GameEntity
	{
		public enum BulletCollisionType { None, VoidObject, GlassObject, FriendlyCannon, NeutralCannon, EnemyCannon, EnemyBullet, FriendlyBullet }

		public  const float BULLET_DIAMETER = 25;
		private const float MAXIMUM_LIEFTIME = 25;

		public readonly Fraction Fraction;

		public bool IsDying;

		public Vector2 BulletPosition;
		public float BulletRotation = 0f;
		public float BulletAlpha = 1f;
		public float BulletExtraScale = 1f;
		public BulletCollisionType LastCollision = BulletCollisionType.None;
		public GameEntity LastCollisionObject;

		public Body PhysicsBody;
		public readonly Cannon Source;
		public readonly float Scale;
		
		private readonly Vector2 initialVelocity;

		public override Vector2 Position => BulletPosition;
		public override FSize DrawingBoundingBox { get; }
		public override Color DebugIdentColor => Fraction.Color;

		public Bullet(GameScreen scrn, Cannon shooter, Vector2 pos, Vector2 velo, float entityScale) : base(scrn, GDConstants.ORDER_GAME_BULLETS)
		{
			BulletPosition = pos;
			initialVelocity = velo;
			Source = shooter;
			Fraction = Source.Fraction;
			Scale = entityScale;

			DrawingBoundingBox = new FSize(BULLET_DIAMETER, BULLET_DIAMETER);
		}

		public override void OnInitialize(EntityManager manager)
		{
			PhysicsBody = BodyFactory.CreateCircle(this.GDManager().PhysicsWorld, ConvertUnits.ToSimUnits(Scale * BULLET_DIAMETER / 2), 1, ConvertUnits.ToSimUnits(BulletPosition), BodyType.Dynamic, this);
			PhysicsBody.LinearVelocity = ConvertUnits.ToSimUnits(initialVelocity);
			PhysicsBody.CollidesWith = Category.All;
			PhysicsBody.IsBullet = true;               // Use CCD solver (prevents tunelling) - do we really need this / how much perf soes this cost ??
			PhysicsBody.Restitution = 1f;              // Bouncability, 1=bounce always elastic
			PhysicsBody.AngularDamping = 1f;           // Practically no angular rotation
			PhysicsBody.Friction = 0f;
			PhysicsBody.LinearDamping = 0f;            // no slowing down
			PhysicsBody.OnCollision += OnCollision;
			PhysicsBody.AngularVelocity = 0;
			//Body.Mass = Scale * Scale; // Weight dependent on size
		}

		private bool OnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
		{
			if (!Alive || IsDying) return false;

			var otherBullet = fixtureB.UserData as Bullet;
			if (otherBullet != null)
			{
				LastCollisionObject = otherBullet;

				if (otherBullet.Fraction == Fraction) { LastCollision = BulletCollisionType.FriendlyBullet; return true;}
				if (!otherBullet.Alive) return false;
				
				if (otherBullet.Scale / Scale >= 2f && !otherBullet.Fraction.IsNeutral)
				{
					// split other
					otherBullet.SplitDestruct();
					MutualDestruct();
					MainGame.Inst.GDSound.PlayEffectCollision();
					LastCollision = BulletCollisionType.EnemyBullet;
					return false;
				}
				else if (Scale / otherBullet.Scale >= 2f && Fraction.IsNeutral)
				{
					// split me
					otherBullet.MutualDestruct();
					SplitDestruct();
					MainGame.Inst.GDSound.PlayEffectCollision();
					LastCollision = BulletCollisionType.EnemyBullet;
					return false;
				}
				else
				{
					otherBullet.MutualDestruct();
					MutualDestruct();
					MainGame.Inst.GDSound.PlayEffectCollision();
					LastCollision = BulletCollisionType.EnemyBullet;
					return false;
				}
			}
			
			var otherCannon = fixtureB.UserData as Cannon;
			if (otherCannon != null)
			{
				LastCollisionObject = otherCannon;

				if (otherCannon.Fraction == Fraction)
				{
					// if Source barrel then ignore collision
					if (otherCannon != Source || fixtureB == otherCannon.PhysicsFixtureBase)
					{
						DisintegrateIntoFriend();
						otherCannon.ApplyBoost();
						MainGame.Inst.GDSound.PlayEffectBoost();
						LastCollision = BulletCollisionType.FriendlyCannon;
					}
				}
				else // if (otherCannon.Fraction != this.Fraction)
				{
					DisintegrateIntoEnemy();
					otherCannon.TakeDamage(Fraction, Scale);
					MainGame.Inst.GDSound.PlayEffectHit();
					LastCollision = BulletCollisionType.EnemyCannon;
				}

				return false;
			}

			var otherVoidWall = fixtureB.UserData as VoidWall;
			if (otherVoidWall != null)
			{
				LastCollisionObject = otherVoidWall;

				DisintegrateIntoVoidObject();
				MainGame.Inst.GDSound.PlayEffectCollision();
				LastCollision = BulletCollisionType.VoidObject;
				return false;
			}

			var otherVoidCircle = fixtureB.UserData as VoidCircle;
			if (otherVoidCircle != null)
			{
				LastCollisionObject = otherVoidCircle;

				DisintegrateIntoVoidObject();
				MainGame.Inst.GDSound.PlayEffectCollision();
				LastCollision = BulletCollisionType.VoidObject;
				return false;
			}

			var otherGlassBlock = fixtureB.UserData as GlassBlock;
			if (otherGlassBlock != null)
			{
				LastCollisionObject = otherGlassBlock;

				//TODO Glass Collision Soundeffect
				LastCollision = BulletCollisionType.GlassObject;
				return true;
			}

			// wud ???
			SAMLog.Error("Collision", string.Format("Bullet collided with unkown fixture: {0}", fixtureB.UserData ?? "<NULL>"));
			return false;
		}

		private void MutualDestruct()
		{
			// After Bullet-Bulllet Collision

			if (MainGame.Inst.Profile.EffectsEnabled)
			{
				for (int i = 0; i < 8; i++)
					Manager.AddEntity(new BulletSplitter(Owner, this, (FlatAlign8)i));
			}

			Alive = false;
		}

		private void SplitDestruct()
		{
			// After Bullet-Bulllet Collision

			Alive = false;

			float newScale = Scale / 2f;

			var v1 = ConvertUnits.ToDisplayUnits(PhysicsBody.LinearVelocity).Rotate(FloatMath.RAD_NEG_030);
			var v2 = ConvertUnits.ToDisplayUnits(PhysicsBody.LinearVelocity).Rotate(FloatMath.RAD_POS_030);

			var p1 = Position + v1.WithLength(BULLET_DIAMETER * newScale * 0.9f);
			var p2 = Position + v2.WithLength(BULLET_DIAMETER * newScale * 0.9f);


			Manager.AddEntity(new Bullet(Owner, Source, p1, v1, newScale));
			Manager.AddEntity(new Bullet(Owner, Source, p2, v2, newScale));
		}

		private void DisintegrateIntoVoidObject()
		{
			// After Bullet-Void Collision

			if (MainGame.Inst.Profile.EffectsEnabled)
			{
				for (int i = 0; i < 8; i++)
					Manager.AddEntity(new BulletSplitter(Owner, this, (FlatAlign8)i));
			}

			Alive = false;
		}

		private void DisintegrateIntoEnemy()
		{
			// After Bullet-Cannon Collision

			AddEntityOperation(new BulletFadeAndDieOperation(0.05f));
			IsDying = true;
		}

		public void DisintegrateIntoVortex()
		{
			// After Bullet-Vortex Collision

			if (IsDying) return;

			AddEntityOperation(new BulletShrinkAndDieOperation(0.35f));
			IsDying = true;
		}

		private void DisintegrateIntoFriend()
		{
			// After Bullet-Cannon Collision

			AddEntityOperation(new BulletShrinkAndDieOperation(0.15f));
			IsDying = true;
		}

		public override void OnRemove()
		{
			this.GDManager().PhysicsWorld.RemoveBody(PhysicsBody);
		}

		protected override void OnUpdate(SAMTime gameTime, InputState istate)
		{
			BulletPosition = ConvertUnits.ToDisplayUnits(PhysicsBody.Position);
			BulletRotation = PhysicsBody.Rotation;

			if (Lifetime > MAXIMUM_LIEFTIME) AddEntityOperation(new BulletFadeAndDieOperation(1.0f));

			if (!Manager.BoundingBox.Contains(BulletPosition)) Remove();
		}

		protected override void OnDraw(IBatchRenderer sbatch)
		{
			sbatch.DrawCentered(
				Textures.TexBullet, 
				BulletPosition, 
				Scale * BulletExtraScale * BULLET_DIAMETER,
				Scale * BulletExtraScale * BULLET_DIAMETER,
				Fraction.Color * BulletAlpha, BulletRotation);
		}
	}
}
