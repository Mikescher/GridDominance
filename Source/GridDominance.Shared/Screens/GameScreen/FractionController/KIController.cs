using System;
using System.Collections.Generic;
using System.Linq;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using GridDominance.Shared.Framework;
using GridDominance.Shared.Screens.GameScreen.Entities;
using Microsoft.Xna.Framework;

namespace GridDominance.Shared.Screens.GameScreen.FractionController
{
	abstract class KIController : AbstractFractionController
	{
		protected const float STANDARD_UPDATE_TIME = 1.666f;
		protected const float NEUTRAL_UPDATE_TIME  = 0.111f;

		private readonly ConstantRandom crng;
		
		protected KIController(float interval, GameScreen owner, Cannon cannon, Fraction fraction) 
			: base(interval, owner, cannon, fraction)
		{
			crng = new ConstantRandom(cannon);
		}

		protected bool CalculateKI(List<Func<GDEntity>> searchFunctions, bool idleRotate)
		{
			foreach (var sf in searchFunctions)
			{
				var target = sf();
				if (target != null)
				{
					Cannon.RotateTo(target);

					//Owner.PushNotification("Cannon :: KIController --> " + sf.Method.Name);
					return true;
				}
			}

			if (idleRotate) Cannon.Rotation.Set(FloatMath.GetRangedRandom(0, FloatMath.TAU));
			//Owner.PushNotification("Cannon :: KIController --> Idle");

			return false;
		}

		#region Target Finding

		protected Bullet FindTargetAttackingBullet()
		{
			return Owner
				.GetEntities<Bullet>()
				.Where(p => p.Fraction != Fraction)
				.Where(p => ! p.IsDying)
				.Where(IsHoming)
				.WhereSmallestBy(p => (p.Position - Cannon.Position).Length(), GameScreen.TILE_WIDTH / 2f)
				.RandomOrDefault(crng);
		}

		protected Cannon FindTargetSupportCannon()
		{
			return Owner
				.GetEntities<Cannon>()
				.Where(p => p.Fraction == Fraction)
				.Where(p => p != Cannon)
				.Where(p => p.CannonHealth.TargetValue < 0.5f)
				.Where(IsReachable)
				.WhereSmallestBy(p => (p.Position - Cannon.Position).Length(), GameScreen.TILE_WIDTH)
				.RandomOrDefault(crng);
		}

		protected Cannon FindTargetNeutralCannon()
		{
			return Owner
				.GetEntities<Cannon>()
				.Where(p => p.Fraction.IsNeutral)
				.Where(IsReachable)
				.WhereSmallestBy(p => (p.Position - Cannon.Position).Length(), 2 * GameScreen.TILE_WIDTH)
				.RandomOrDefault(crng);
		}

		protected Cannon FindTargetEnemyCannon()
		{
			return Owner
				.GetEntities<Cannon>()
				.Where(p => !p.Fraction.IsNeutral)
				.Where(p => p.Fraction != Fraction)
				.Where(IsReachable)
				.WhereSmallestBy(p => (p.Position - Cannon.Position).Length(), GameScreen.TILE_WIDTH)
				.RandomOrDefault(crng);
		}

		protected Cannon FindTargetFriendlyCannon()
		{
			return Owner
				.GetEntities<Cannon>()
				.Where(p => p.Fraction == Fraction)
				.Where(p => p != Cannon)
				.Where(IsFullyReachable)
				.WhereSmallestBy(p => (p.Position - Cannon.Position).Length(), GameScreen.TILE_WIDTH)
				.RandomOrDefault(crng);
		}

		protected Cannon FindTargetBlockedEnemyCannon()
		{
			return Owner
				.GetEntities<Cannon>()
				.Where(p => !p.Fraction.IsNeutral)
				.Where(p => p.Fraction != Fraction)
				.Where(IsBulletBlockedReachable)
				.WhereSmallestBy(p => (p.Position - Cannon.Position).Length(), GameScreen.TILE_WIDTH)
				.RandomOrDefault(crng);
		}

		protected Cannon FindTargetBlockedFriendlyCannon()
		{
			return Owner
				.GetEntities<Cannon>()
				.Where(p => p.Fraction == Fraction)
				.Where(p => p != Cannon)
				.Where(IsReachable)
				.WhereSmallestBy(p => (p.Position - Cannon.Position).Length(), GameScreen.TILE_WIDTH)
				.RandomOrDefault(crng);
		}

		protected Cannon FindNearestEnemyCannon()
		{
			return Owner
				.GetEntities<Cannon>()
				.Where(p => !p.Fraction.IsNeutral)
				.Where(p => p.Fraction != Fraction)
				.WhereSmallestBy(p => (p.Position - Cannon.Position).Length(), GameScreen.TILE_WIDTH)
				.RandomOrDefault(crng);
		}

		// ignore all bullets
		private bool IsHoming(Bullet b)
		{
			//TODO: Optimize IsHoming()
			// We could cache the target cannon in every bullet
			// and update it on collision or creation
			// would be faster (?) - optimization opportunity for later
			// i should measure how expensive ray tracing is

			GDEntity result = null;

			Func<Fixture, Vector2, Vector2, float, float> callback = (f, pos, normal, frac) =>
			{
				if (f.UserData is Bullet) // ignore _all_ Bullets
				{
					return -1; // ignore
				}

				result = (GDEntity)f.UserData;
				return frac; // limit to this length
			};

			var rayStart = b.PhysicsBody.Position;
			var rayEnd = rayStart + b.PhysicsBody.LinearVelocity * ConvertUnits.ToSimUnits(GameScreen.VIEW_WIDTH) / b.PhysicsBody.LinearVelocity.Length();

			Owner.GetPhysicsWorld().RayCast(callback, rayStart, rayEnd);

			return (result == Cannon);
		}

		// ignore own bullets
		private bool IsReachable(Cannon c)
		{
			GDEntity result = null;

			Func<Fixture, Vector2, Vector2, float, float> callback = (f, pos, normal, frac) =>
			{
				if (f.UserData == c) return frac; // limit

				var bulletData = f.UserData as Bullet;
				if ((bulletData != null) && bulletData.Source == Cannon) // ignore own Bullets
				{
					return -1; // ignore
				}

				result = (GDEntity) f.UserData;

				return 0; // terminate
			};

			var rayStart = Cannon.PhysicsBody.Position;
			var rayEnd = c.PhysicsBody.Position;

			Owner.GetPhysicsWorld().RayCast(callback, rayStart, rayEnd);

			return (result == null);
		}

		// ignore nothing
		private bool IsFullyReachable(Cannon c)
		{
			GDEntity result = null;

			Func<Fixture, Vector2, Vector2, float, float> callback = (f, pos, normal, frac) =>
			{
				if (f.UserData == c) return frac; // limit
			
				result = (GDEntity)f.UserData;

				return 0; // terminate
			};

			var rayStart = Cannon.PhysicsBody.Position;
			var rayEnd = c.PhysicsBody.Position;

			Owner.GetPhysicsWorld().RayCast(callback, rayStart, rayEnd);

			return (result == null);
		}

		// ignore all bullets
		private bool IsBulletBlockedReachable(Cannon c)
		{
			GDEntity result = null;

			Func<Fixture, Vector2, Vector2, float, float> callback = (f, pos, normal, frac) =>
			{
				if (f.UserData == c) return frac; // limit
				
				if (f.UserData is Bullet) // ignore _all_ Bullets
				{
					return -1; // ignore
				}

				result = (GDEntity)f.UserData;

				return 0; // terminate
			};

			var rayStart = Cannon.PhysicsBody.Position;
			var rayEnd = c.PhysicsBody.Position;

			Owner.GetPhysicsWorld().RayCast(callback, rayStart, rayEnd);

			return (result == null);
		}

		#endregion
	}
}
