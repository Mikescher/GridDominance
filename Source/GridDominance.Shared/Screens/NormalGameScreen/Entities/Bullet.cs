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
using FarseerPhysics.Common;
using System.Collections.Generic;
using MonoSAMFramework.Portable;
using GridDominance.Shared.Screens.NormalGameScreen.Physics;
using GridDominance.Shared.Screens.ScreenGame;

namespace GridDominance.Shared.Screens.NormalGameScreen.Entities
{
	public class Bullet : BaseBullet
	{
		private sealed class CollisionIgnorePortal { public Portal Entity; public ulong LastCollidedCycle; }
		private const int CollisionIgnoreObjectDecayCycles = 16;

		public Vector2 Velocity => ConvertUnits.ToDisplayUnits(PhysicsBody.LinearVelocity);

		public readonly Cannon Source;
		public readonly GDGameScreen GDOwner;

		private readonly Vector2 initialVelocity;

		public override FPoint Position => BulletPosition;
		public override FSize DrawingBoundingBox { get; }
		public override Color DebugIdentColor => Fraction.Color;

		private List<CollisionIgnorePortal> _ignoredPortals = new List<CollisionIgnorePortal>();

		public Bullet(GDGameScreen scrn, Cannon shooter, FPoint pos, Vector2 velo, float entityScale, Fraction frac) : base(scrn, frac, entityScale)
		{
			BulletPosition = pos;
			initialVelocity = velo;
			Source = shooter;
			GDOwner = scrn;
			BulletID = scrn.AssignBulletID(this);

			DrawingBoundingBox = new FSize(Scale * BULLET_DIAMETER, Scale * BULLET_DIAMETER);
		}

		public override void OnInitialize(EntityManager manager)
		{
			PhysicsBody = BodyFactory.CreateCircle(this.GDManager().PhysicsWorld, ConvertUnits.ToSimUnits(Scale * BULLET_DIAMETER / 2), 1, ConvertUnits2.ToSimUnits(BulletPosition), BodyType.Dynamic, this);
			PhysicsBody.LinearVelocity = ConvertUnits.ToSimUnits(initialVelocity);
			PhysicsBody.CollidesWith = Category.All;
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

			#region Bullet
			var otherBullet = fixtureB.UserData as Bullet;
			if (otherBullet != null)
			{

				if (otherBullet.Fraction == Fraction) return true;
				if (!otherBullet.Alive) return false;
				
				if (otherBullet.Scale / Scale >= 2f && !otherBullet.Fraction.IsNeutral)
				{
					// split other
					otherBullet.SplitDestruct();
					MutualDestruct();
					MainGame.Inst.GDSound.PlayEffectCollision();
					return false;
				}
				else if (Scale / otherBullet.Scale >= 2f && Fraction.IsNeutral)
				{
					// split me
					otherBullet.MutualDestruct();
					SplitDestruct();
					MainGame.Inst.GDSound.PlayEffectCollision();
					return false;
				}
				else
				{
					otherBullet.MutualDestruct();
					MutualDestruct();
					MainGame.Inst.GDSound.PlayEffectCollision();
					return false;
				}
			}
			#endregion

			#region Cannon
			var otherCannon = fixtureB.UserData as Cannon;
			if (otherCannon != null)
			{
				if (otherCannon.Fraction == Fraction)
				{
					// if Source barrel then ignore collision
					if (otherCannon != Source || fixtureB == otherCannon.PhysicsFixtureBase)
					{
						DisintegrateIntoFriend();
						otherCannon.ApplyBoost();
						MainGame.Inst.GDSound.PlayEffectBoost();
					}
				}
				else // if (otherCannon.Fraction != this.Fraction)
				{
					DisintegrateIntoEnemy();
					otherCannon.TakeDamage(Fraction, Scale);
					MainGame.Inst.GDSound.PlayEffectHit();
				}

				return false;
			}
			#endregion

			#region VoidWall
			var otherVoidWall = fixtureB.UserData as VoidWall;
			if (otherVoidWall != null)
			{
				DisintegrateIntoVoidObject();
				MainGame.Inst.GDSound.PlayEffectCollision();
				return false;
			}
			#endregion

			#region VoidCircle
			var otherVoidCircle = fixtureB.UserData as VoidCircle;
			if (otherVoidCircle != null)
			{
				DisintegrateIntoVoidObject();
				MainGame.Inst.GDSound.PlayEffectCollision();
				return false;
			}
			#endregion

			#region GlassBlock
			var otherGlassBlock = fixtureB.UserData as GlassBlock;
			if (otherGlassBlock != null)
			{
				MainGame.Inst.GDSound.PlayEffectReflect();
				return true;
			}
			#endregion

			#region Portal
			var otherPortal = fixtureB.UserData as Portal;
			if (otherPortal != null)
			{
				var inPortal = otherPortal;

				Vector2 normal;
				FixedArray2<Vector2> t;
				contact.GetWorldManifold(out normal, out t);

				bool hit = FloatMath.DiffRadiansAbs(normal.ToAngle(), inPortal.Normal) < FloatMath.RAD_POS_001;

				if (!hit)
				{
					// back-side hit
					DisintegrateIntoVoidObject();
					return false;
				}

				if (inPortal.Links.Count == 0)
				{
					// void portal
					DisintegrateIntoPortal();
					return false;
				}

				var velocity = ConvertUnits.ToDisplayUnits(PhysicsBody.LinearVelocity);

				for (int i = 0; i < _ignoredPortals.Count; i++)
				{
					if (_ignoredPortals[i].Entity == inPortal)
					{
						_ignoredPortals[i].LastCollidedCycle = MonoSAMGame.GameCycleCounter;

						if (FloatMath.DiffRadiansAbs(velocity.ToAngle(), inPortal.Normal) > FloatMath.RAD_POS_090)
						{
							// prevent tunneling
							Alive = false;
							return false;
						}

						return false;
					}
				}

				foreach (var outportal in inPortal.Links)
				{
					var stretch = outportal.Length / inPortal.Length;

					var rot = outportal.Normal - inPortal.Normal + FloatMath.RAD_POS_180;
					var projec = ConvertUnits.ToDisplayUnits(PhysicsBody.Position).ProjectOntoLine(inPortal.Position, inPortal.VecDirection);

					var newVelocity = velocity.Rotate(rot);
					var newStart = outportal.Position + outportal.VecDirection * (-projec) + outportal.VecNormal * (Portal.WIDTH / 2f);

					var b = new Bullet(GDOwner, Source, newStart, newVelocity, Scale * stretch, Fraction) { Lifetime = Lifetime };
					b._ignoredPortals.Add(new CollisionIgnorePortal() { Entity = outportal, LastCollidedCycle = MonoSAMGame.GameCycleCounter});
					b.AddEntityOperation(new BulletGrowOperation(0.15f));
					Owner.Entities.AddEntity(b);
				}
				DisintegrateIntoPortal();
				return false;
			}
			#endregion

			#region MirrorBlock
			var otherMirrorBlock = fixtureB.UserData as MirrorBlock;
			if (otherMirrorBlock != null)
			{
				MainGame.Inst.GDSound.PlayEffectReflect(); //TODO evtl other sound?
				return true;
			}
			#endregion

			#region MirrorCircle
			var otherMirrorCircle = fixtureB.UserData as MirrorCircle;
			if (otherMirrorCircle != null)
			{
				MainGame.Inst.GDSound.PlayEffectReflect(); //TODO evtl other sound?
				return true;
			}
			#endregion

			#region RefractionMarker
			var otherRefractionMarker1 = fixtureB.UserData as MarkerRefractionEdge;
			if (otherRefractionMarker1 != null) return false;
			var otherRefractionMarker2 = fixtureB.UserData as MarkerRefractionCorner;
			if (otherRefractionMarker2 != null) return false;
			#endregion

			#region BorderMarker
			var otherBorderMarker = fixtureB.UserData as MarkerCollisionBorder;
			if (otherBorderMarker != null)
			{
				if (GDOwner.WrapMode == GameWrapMode.Reflect) return true;
				return false;
			}
			#endregion

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


			Manager.AddEntity(new Bullet(GDOwner, Source, p1, v1, newScale, Fraction));
			Manager.AddEntity(new Bullet(GDOwner, Source, p2, v2, newScale, Fraction));

			GDOwner.ChangeBulletID(RemoteBullet.RemoteBulletState.Dying_Instant, BulletID, this);
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

			GDOwner.ChangeBulletID(RemoteBullet.RemoteBulletState.Dying_Explosion, BulletID, this);
		}

		private void VanishOutOfBounds()
		{
			Alive = false;
			GDOwner.ChangeBulletID(RemoteBullet.RemoteBulletState.Dying_Instant, BulletID, this);
		}

		private void DisintegrateIntoEnemy()
		{
			// After Bullet-Cannon Collision

			AddEntityOperation(new BulletFadeAndDieOperation(0.05f));
			IsDying = true;

			GDOwner.ChangeBulletID(RemoteBullet.RemoteBulletState.Dying_Fade, BulletID, this);
		}

		public void DisintegrateIntoVortex()
		{
			// After Bullet-Vortex Collision

			if (IsDying) return;

			AddEntityOperation(new BulletShrinkAndDieOperation(0.35f));
			IsDying = true;

			GDOwner.ChangeBulletID(RemoteBullet.RemoteBulletState.Dying_ShrinkSlow, BulletID, this);
		}

		private void DisintegrateIntoFriend()
		{
			// After Bullet-Cannon Collision

			AddEntityOperation(new BulletShrinkAndDieOperation(0.15f));
			IsDying = true;

			GDOwner.ChangeBulletID(RemoteBullet.RemoteBulletState.Dying_ShrinkFast, BulletID, this);
		}

		private void DisintegrateIntoPortal()
		{
			// After Bullet-Portal Collision

			AddEntityOperation(new BulletShrinkAndDieOperation(0.15f));
			IsDying = true;

			GDOwner.ChangeBulletID(RemoteBullet.RemoteBulletState.Dying_ShrinkFast, BulletID, this);
		}

		public override void OnRemove()
		{
			this.GDManager().PhysicsWorld.RemoveBody(PhysicsBody);
			GDOwner.UnassignBulletID(BulletID, this);
		}

		protected override void OnUpdate(SAMTime gameTime, InputState istate)
		{
			BulletPosition = ConvertUnits2.ToDisplayUnitsPoint(PhysicsBody.Position);
			BulletRotation = PhysicsBody.Rotation;

			for (int i = _ignoredPortals.Count - 1; i >= 0; i--)
			{
				if (MonoSAMGame.GameCycleCounter - _ignoredPortals[i].LastCollidedCycle > 16)
				{
					_ignoredPortals[i].LastCollidedCycle = CollisionIgnoreObjectDecayCycles;
					_ignoredPortals.RemoveAt(i);
				}
			}

			if (Lifetime > MAXIMUM_LIFETIME) AddEntityOperation(new BulletFadeAndDieOperation(1.0f));

			if (GDOwner.WrapMode == GameWrapMode.Death && !Manager.BoundingBox.Contains(BulletPosition))
			{
				if (!GDOwner.MapFullBounds.Contains(BulletPosition))
				{
					VanishOutOfBounds();
				}
			}

			if (GDOwner.WrapMode == GameWrapMode.Donut && !GDOwner.MapFullBounds.Contains(BulletPosition))
			{
				DonutWrap();
			}

			this.GDOwner().LaserNetwork.SemiDirty = true;
		}

		private void DonutWrap()
		{
			BulletPosition = BulletPosition.ModuloToToSize(GDOwner.MapFullBounds.Size);

			PhysicsBody.Position = ConvertUnits2.ToSimUnits(BulletPosition);
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
