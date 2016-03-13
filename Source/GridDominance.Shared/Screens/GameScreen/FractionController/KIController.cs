using System;
using System.Linq;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using GridDominance.Shared.Framework;
using GridDominance.Shared.Screens.GameScreen.Entities;
using Microsoft.Xna.Framework;

namespace GridDominance.Shared.Screens.GameScreen.FractionController
{
	class KIController : AbstractFractionController
	{
		private readonly ConstantRandom crng;
		
		public KIController(GameScreen owner, Cannon cannon, Fraction fraction) 
			: base(COMPUTER_UPDATE_TIME, owner, cannon, fraction)
		{
			crng = new ConstantRandom(cannon);
		}

		protected override void Calculate(InputState istate)
		{
			// ------------------------ Dodge incoming missiles ------------------------

			var attackBullet = FindTargetAttackingBullet();
			if (attackBullet != null)
			{
				Cannon.RotateTo(attackBullet);

				Owner.PushNotification("Cannon :: KIController --> AttackingBullet");
				return;
			}

			// -------------------------- Help low HP friends --------------------------

			var supportCannon = FindTargetSupportCannon();
			if (supportCannon != null)
			{
				Cannon.RotateTo(supportCannon);

				Owner.PushNotification("Cannon :: KIController --> SupportFriendlyCannon");
				return;
			}

			// ------------------------ Conquer neutral cannons ------------------------

			var neutralCannon = FindTargetNeutralCannon();
			if (neutralCannon != null)
			{
				Cannon.RotateTo(neutralCannon);

				Owner.PushNotification("Cannon :: KIController --> ConquerNeutralCannon");
				return;
			}

			// ---------------------------- Attack opponents ---------------------------

			var enemyCannon = FindTargetEnemyCannon();
			if (enemyCannon != null)
			{
				Cannon.RotateTo(enemyCannon);

				Owner.PushNotification("Cannon :: KIController --> ConquerNeutralCannon");
				return;
			}

			// -------------------------- Support teamplayers --------------------------

			var boostCannon = FindTargetFriendlyCannon();
			if (boostCannon != null)
			{
				Cannon.RotateTo(boostCannon);

				Owner.PushNotification("Cannon :: KIController --> BoostTeamplayer");
				return;
			}

			// --------------------------- Attack who you can --------------------------

			var blockedEnemyCannon = FindTargetBlockedEnemyCannon();
			if (blockedEnemyCannon != null)
			{
				Cannon.RotateTo(blockedEnemyCannon);

				Owner.PushNotification("Cannon :: KIController --> AttackBlockedCannon");
				return;
			}

			// -------------------------- Support who you can --------------------------

			var blockedFriendlyCannon = FindTargetBlockedFriendlyCannon();
			if (blockedFriendlyCannon != null)
			{
				Cannon.RotateTo(blockedFriendlyCannon);

				Owner.PushNotification("Cannon :: KIController --> SupportBlockedCannon");
				return;
			}

			// ------------------------------- Just guess ------------------------------

			var nearestEnemyCannon = FindNearestEnemyCannon();
			if (nearestEnemyCannon != null)
			{
				Cannon.RotateTo(nearestEnemyCannon);

				Owner.PushNotification("Cannon :: KIController --> AttackNearestCannon");
				return;
			}

			Cannon.Rotation.Set(FloatMath.GetRangedRandom(0, FloatMath.TAU));
			Owner.PushNotification("Cannon :: KIController --> Idle");
		}

		#region Target Finding

		private Bullet FindTargetAttackingBullet()
		{
			return Owner
				.GetEntities<Bullet>()
				.Where(p => p.Fraction != Fraction)
				.Where(p => ! p.IsDying)
				.Where(IsHoming)
				.RandomOrDefault(crng);
		}

		private Cannon FindTargetSupportCannon()
		{
			return Owner
				.GetEntities<Cannon>()
				.Where(p => p.Fraction == Fraction)
				.Where(p => p != Cannon)
				.Where(p => p.CannonHealth.TargetValue < 0.5f)
				.Where(IsReachable)
				.WhereSmallestBy(p => (p.Position - Cannon.Position).LengthSquared(), GameScreen.TILE_WIDTH_SQUARED)
				.RandomOrDefault(crng);
		}

		private Cannon FindTargetNeutralCannon()
		{
			return Owner
				.GetEntities<Cannon>()
				.Where(p => p.Fraction.IsNeutral)
				.Where(IsReachable)
				.WhereSmallestBy(p => (p.Position - Cannon.Position).LengthSquared(), GameScreen.TILE_WIDTH_SQUARED)
				.RandomOrDefault(crng);
		}

		private Cannon FindTargetEnemyCannon()
		{
			return Owner
				.GetEntities<Cannon>()
				.Where(p => !p.Fraction.IsNeutral)
				.Where(p => p.Fraction != Fraction)
				.Where(IsReachable)
				.WhereSmallestBy(p => (p.Position - Cannon.Position).LengthSquared(), GameScreen.TILE_WIDTH_SQUARED)
				.RandomOrDefault(crng);
		}

		private Cannon FindTargetFriendlyCannon()
		{
			return Owner
				.GetEntities<Cannon>()
				.Where(p => p.Fraction == Fraction)
				.Where(p => p != Cannon)
				.Where(IsFullyReachable)
				.WhereSmallestBy(p => (p.Position - Cannon.Position).LengthSquared(), GameScreen.TILE_WIDTH_SQUARED)
				.RandomOrDefault(crng);
		}

		private Cannon FindTargetBlockedEnemyCannon()
		{
			return Owner
				.GetEntities<Cannon>()
				.Where(p => !p.Fraction.IsNeutral)
				.Where(p => p.Fraction != Fraction)
				.Where(IsBulletBlockedReachable)
				.WhereSmallestBy(p => (p.Position - Cannon.Position).LengthSquared(), GameScreen.TILE_WIDTH_SQUARED)
				.RandomOrDefault(crng);
		}

		private Cannon FindTargetBlockedFriendlyCannon()
		{
			return Owner
				.GetEntities<Cannon>()
				.Where(p => p.Fraction == Fraction)
				.Where(p => p != Cannon)
				.Where(IsReachable)
				.WhereSmallestBy(p => (p.Position - Cannon.Position).LengthSquared(), GameScreen.TILE_WIDTH_SQUARED)
				.RandomOrDefault(crng);
		}

		private Cannon FindNearestEnemyCannon()
		{
			return Owner
				.GetEntities<Cannon>()
				.Where(p => !p.Fraction.IsNeutral)
				.Where(p => p.Fraction != Fraction)
				.WhereSmallestBy(p => (p.Position - Cannon.Position).LengthSquared(), GameScreen.TILE_WIDTH_SQUARED)
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
