using System;
using System.Collections.Generic;
using System.Linq;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.ScreenGame.Entities;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath;
using GridDominance.Shared.Screens.ScreenGame.Fractions;
using MonoSAMFramework.Portable.Screens.Entities;

namespace GridDominance.Shared.Screens.ScreenGame.FractionController
{
	abstract class KIController : AbstractFractionController
	{
		public class KIMethod
		{
			public readonly string Name;
			public readonly Func<GameEntity> Run;

			public KIMethod(string n, Func<GameEntity> r)
			{
				Name = n;
				Run = r;
			}
		}

		protected const float STANDARD_UPDATE_TIME = 1.666f;
		protected const float NEUTRAL_UPDATE_TIME  = 0.111f;

		private readonly ConstantRandom crng;
		public string LastKIFunction = "None";

		protected KIController(float interval, GDGameScreen owner, Cannon cannon, Fraction fraction) 
			: base(interval, owner, cannon, fraction)
		{
			crng = new ConstantRandom(cannon);
		}

		protected bool CalculateKI(List<KIMethod> searchFunctions, bool idleRotate)
		{
			foreach (var sf in searchFunctions)
			{
				var target = sf.Run();
				if (target != null)
				{
					Cannon.RotateTo(target);

					LastKIFunction = sf.Name;

					return true;
				}
			}

			if (idleRotate)
			{
				LastKIFunction = "Random";
				Cannon.Rotation.Set(FloatMath.GetRangedRandom(0, FloatMath.TAU));
				return false;
			}
			else
			{
				LastKIFunction = "None";
				return false;
			}
		}

		#region Target Finding

		protected Bullet FindTargetAttackingBullet()
		{
			return Owner
				.GetEntities<Bullet>()
				.Where(p => p.Fraction != Fraction)
				.Where(p => ! p.IsDying)
				.Where(IsHoming)
				.WhereSmallestBy(p => (p.Position - Cannon.Position).Length(), GDConstants.TILE_WIDTH / 2f)
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
				.WhereSmallestBy(p => (p.Position - Cannon.Position).Length(), GDConstants.TILE_WIDTH)
				.RandomOrDefault(crng);
		}

		protected Cannon FindTargetNeutralCannon()
		{
			return Owner
				.GetEntities<Cannon>()
				.Where(p => p.Fraction.IsNeutral)
				.Where(IsReachable)
				.WhereSmallestBy(p => (p.Position - Cannon.Position).Length(), 2 * GDConstants.TILE_WIDTH)
				.RandomOrDefault(crng);
		}

		protected Cannon FindTargetEnemyCannon()
		{
			return Owner
				.GetEntities<Cannon>()
				.Where(p => !p.Fraction.IsNeutral)
				.Where(p => p.Fraction != Fraction)
				.Where(IsReachable)
				.WhereSmallestBy(p => (p.Position - Cannon.Position).Length(), GDConstants.TILE_WIDTH)
				.RandomOrDefault(crng);
		}

		protected Cannon FindTargetFriendlyCannon()
		{
			return Owner
				.GetEntities<Cannon>()
				.Where(p => p.Fraction == Fraction)
				.Where(p => p != Cannon)
				.Where(IsReachable)
				.WhereSmallestBy(p => (p.Position - Cannon.Position).Length(), GDConstants.TILE_WIDTH)
				.RandomOrDefault(crng);
		}

		protected Cannon FindTargetBlockedEnemyCannon()
		{
			return Owner
				.GetEntities<Cannon>()
				.Where(p => !p.Fraction.IsNeutral)
				.Where(p => p.Fraction != Fraction)
				.Where(IsBulletBlockedReachable)
				.WhereSmallestBy(p => (p.Position - Cannon.Position).Length(), GDConstants.TILE_WIDTH)
				.RandomOrDefault(crng);
		}

		protected Cannon FindTargetBlockedFriendlyCannon()
		{
			return Owner
				.GetEntities<Cannon>()
				.Where(p => p.Fraction == Fraction)
				.Where(p => p != Cannon)
				.Where(IsBulletBlockedReachable)
				.WhereSmallestBy(p => (p.Position - Cannon.Position).Length(), GDConstants.TILE_WIDTH)
				.RandomOrDefault(crng);
		}

		protected Cannon FindNearestEnemyCannon()
		{
			return Owner
				.GetEntities<Cannon>()
				.Where(p => !p.Fraction.IsNeutral)
				.Where(p => p.Fraction != Fraction)
				.WhereSmallestBy(p => (p.Position - Cannon.Position).Length(), GDConstants.TILE_WIDTH)
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

			GameEntity result = null;

			Func<Fixture, Vector2, Vector2, float, float> callback = (f, pos, normal, frac) =>
			{
				if (f.UserData is Bullet) // ignore _all_ Bullets
				{
					return -1; // ignore
				}

				result = (GameEntity)f.UserData;
				return frac; // limit to this length
			};

			var rayStart = b.PhysicsBody.Position;
			var rayEnd = rayStart + b.PhysicsBody.LinearVelocity * ConvertUnits.ToSimUnits(GDConstants.VIEW_WIDTH) / b.PhysicsBody.LinearVelocity.Length();

			Owner.GetPhysicsWorld().RayCast(callback, rayStart, rayEnd);

			return (result == Cannon);
		}

		// ignore own bullets
		private bool IsReachable(Cannon c)
		{
			GameEntity result = null;

			Func<Fixture, Vector2, Vector2, float, float> callback = (f, pos, normal, frac) =>
			{
				if (f.UserData == Cannon) return -1; // ignore self;

				if (f.UserData == c) return frac; // limit

				var bulletData = f.UserData as Bullet;
				if ((bulletData != null) && bulletData.Source == Cannon) // ignore own Bullets
				{
					return -1; // ignore
				}

				result = (GameEntity) f.UserData;

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
			GameEntity result = null;

			Func<Fixture, Vector2, Vector2, float, float> callback = (f, pos, normal, frac) =>
			{
				if (f.UserData == Cannon) return -1; // ignore self;

				if (f.UserData == c) return frac; // limit
				
				if (f.UserData is Bullet) // ignore _all_ Bullets
				{
					return -1; // ignore
				}

				result = (GameEntity)f.UserData;

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
