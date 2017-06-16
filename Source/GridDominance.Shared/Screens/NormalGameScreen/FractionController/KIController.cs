using System;
using System.Collections.Generic;
using System.Linq;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.NormalGameScreen.Entities;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using GridDominance.Shared.Screens.NormalGameScreen.LaserNetwork;
using GridDominance.Shared.Screens.NormalGameScreen.Physics;
using GridDominance.Shared.Screens.ScreenGame;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Screens.Entities;

namespace GridDominance.Shared.Screens.NormalGameScreen.FractionController
{
	abstract class KIController : AbstractFractionController
	{
		protected class KIMethod
		{
			public readonly string Name;

			private readonly Func<GameEntity> _runDirect;
			private readonly Func<BulletPath> _runPrecalc;
			private readonly Func<LaserRay> _runAntiLaser;
			private readonly Action<KIController> _runGeneric;

			private KIMethod(string n, Func<GameEntity> r1, Func<BulletPath> r2, Action<KIController> r3, Func<LaserRay> r4)
			{
				Name = n;
				_runDirect = r1;
				_runPrecalc = r2;
				_runGeneric = r3;
				_runAntiLaser = r4;
			}

			public static KIMethod CreateRaycast(string n, Func<GameEntity>     r) => new KIMethod(n, r,    null, null, null);
			public static KIMethod CreatePrecalc(string n, Func<BulletPath>     r) => new KIMethod(n, null, r,    null, null);
			public static KIMethod CreateGeneric(string n, Action<KIController> r) => new KIMethod(n, null, null, r,    null);
			public static KIMethod CreateDefense(string n, Func<LaserRay>       r) => new KIMethod(n, null, null, null, r);

			public bool Run(KIController ki)
			{
				if (_runDirect != null)
				{
					var target = _runDirect();
					if (target != null)
					{
						ki.Cannon.RotateTo(target);

						ki.LastKIFunction = Name;

						return true;
					}
				}
				else if (_runPrecalc != null)
				{
					var target = _runPrecalc();
					if (target != null)
					{
						ki.Cannon.Rotation.Set(target.CannonRotation);

						ki.LastKIFunction = Name;

						return true;
					}
				}
				else if (_runAntiLaser != null)
				{
					var ray = _runAntiLaser();
					if (ray != null)
					{
						var target = ki.Cannon.Position.MirrorAt(FPoint.MiddlePoint(ray.Start, ray.End));
						ki.Cannon.Rotation.Set(target.ToAngle(ki.Cannon.Position));
						ki.LastKIFunction = Name;

						return true;
					}
					return false;
				}
				else if (_runGeneric != null)
				{
					_runGeneric(ki);
					return true;
				}

				return false;
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

#if DEBUG
			try { DebugUtils.TIMING_KI.Start();
#endif
			
			foreach (var sf in searchFunctions)
			{
				var result = sf.Run(this);
				if (result) return true;
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

#if DEBUG
			} finally { DebugUtils.TIMING_KI.Stop(); }
#endif
		}

		#region Target Finding (Base)

		protected LaserRay FindTargetAttackingLaser()
		{
			return Cannon
				.AttackingRays
				.RandomOrDefault(crng);
		}

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

		protected Cannon FindNearestFriendlyCannon()
		{
			return Owner
				.GetEntities<Cannon>()
				.Where(p => p.Fraction == Fraction)
				.WhereSmallestBy(p => (p.Position - Cannon.Position).Length(), GDConstants.TILE_WIDTH)
				.RandomOrDefault(crng);
		}

		#endregion

		#region Target Finding (Precalc)

		protected BulletPath FindTargetSupportCannonPrecalc()
		{
			return Cannon.BulletPaths
				.Where(p => p.TargetCannon.Fraction == Fraction)
				.Where(p => p.TargetCannon != Cannon)
				.Where(p => p.TargetCannon.CannonHealth.TargetValue < 0.5f)
				.Where(IsReachablePrecalc)
				.WhereSmallestBy(p => (p.TargetCannon.Position - Cannon.Position).Length(), GDConstants.TILE_WIDTH)
				.RandomOrDefault(crng);
		}

		protected BulletPath FindTargetNeutralCannonPrecalc()
		{
			return Cannon.BulletPaths
				.Where(p => p.TargetCannon.Fraction.IsNeutral)
				.Where(IsReachablePrecalc)
				.WhereSmallestBy(p => (p.TargetCannon.Position - Cannon.Position).Length(), 2 * GDConstants.TILE_WIDTH)
				.RandomOrDefault(crng);
		}

		protected BulletPath FindTargetEnemyCannonPrecalc()
		{
			return Cannon.BulletPaths
				.Where(p => !p.TargetCannon.Fraction.IsNeutral)
				.Where(p => p.TargetCannon.Fraction != Fraction)
				.Where(IsReachablePrecalc)
				.WhereSmallestBy(p => (p.TargetCannon.Position - Cannon.Position).Length(), GDConstants.TILE_WIDTH)
				.RandomOrDefault(crng);
		}

		protected BulletPath FindTargetFriendlyCannonPrecalc()
		{
			return Cannon.BulletPaths
				.Where(p => p.TargetCannon.Fraction == Fraction)
				.Where(p => p.TargetCannon != Cannon)
				.Where(IsReachablePrecalc)
				.WhereSmallestBy(p => (p.TargetCannon.Position - Cannon.Position).Length(), GDConstants.TILE_WIDTH)
				.RandomOrDefault(crng);
		}

		protected BulletPath FindTargetBlockedEnemyCannonPrecalc()
		{
			return Cannon.BulletPaths
				.Where(p => !p.TargetCannon.Fraction.IsNeutral)
				.Where(p => p.TargetCannon.Fraction != Fraction)
				.Where(IsBulletBlockedReachablePrecalc)
				.WhereSmallestBy(p => (p.TargetCannon.Position - Cannon.Position).Length(), GDConstants.TILE_WIDTH)
				.RandomOrDefault(crng);
		}

		protected BulletPath FindTargetBlockedFriendlyCannonPrecalc()
		{
			return Cannon.BulletPaths
				.Where(p => p.TargetCannon.Fraction == Fraction)
				.Where(p => p.TargetCannon != Cannon)
				.Where(IsBulletBlockedReachablePrecalc)
				.WhereSmallestBy(p => (p.TargetCannon.Position - Cannon.Position).Length(), GDConstants.TILE_WIDTH)
				.RandomOrDefault(crng);
		}

		#endregion

		#region Helper

		// ignore all bullets
		private bool IsHoming(Bullet b)
		{
			//TODO: Optimize IsHoming()
			// We could cache the target cannon in every bullet
			// and update it on collision or creation
			// would be faster (?) - optimization opportunity for later
			// i should measure how expensive ray tracing is

			GameEntity result = null;

			float velo = b.PhysicsBody.LinearVelocity.Length();

			if (Math.Abs(velo) < FloatMath.EPSILON6) return false;

			Func<Fixture, Vector2, Vector2, float, float> callback = (f, pos, normal, frac) =>
			{
				if (f.UserData is Bullet) return -1; // ignore _all_ Bullets

				if (f.UserData is IPhysicsMarker) return -1; // ignore

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
				if (bulletData != null && bulletData.Source == Cannon) return -1; // ignore own Bullets

				if (f.UserData is IPhysicsMarker) return -1; // ignore

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
				
				if (f.UserData is Bullet) return -1;// ignore _all_ Bullets
				
				if (f.UserData is IPhysicsMarker) return -1; // ignore

				result = (GameEntity)f.UserData;

				return 0; // terminate
			};

			var rayStart = Cannon.PhysicsBody.Position;
			var rayEnd = c.PhysicsBody.Position;

			Owner.GetPhysicsWorld().RayCast(callback, rayStart, rayEnd);

			return (result == null);
		}

		// ignore own bullets
		private bool IsReachablePrecalc(BulletPath p)
		{
			foreach (var ray in p.Rays)
			{
				GameEntity result = null;
				Func<Fixture, Vector2, Vector2, float, float> callback = (f, pos, normal, frac) =>
				{
					if (f.UserData == Cannon) return -1; // ignore self;

					if (f.UserData == p.TargetCannon) return frac; // limit

					var bulletData = f.UserData as Bullet;
					if (bulletData != null && bulletData.Source == Cannon) return -1; // ignore own Bullets

					if (f.UserData is IPhysicsMarker) return -1; // ignore

					result = (GameEntity)f.UserData;

					return 0; // terminate
				};
				
				Owner.GetPhysicsWorld().RayCast(callback, ray.Item1, ray.Item2);

				if (result != null) return false;
			}

			return true;
		}

		// ignore all bullets
		private bool IsBulletBlockedReachablePrecalc(BulletPath p)
		{
			foreach (var ray in p.Rays)
			{
				GameEntity result = null;
				Func<Fixture, Vector2, Vector2, float, float> callback = (f, pos, normal, frac) =>
				{
					if (f.UserData == Cannon) return -1; // ignore self;

					if (f.UserData == p.TargetCannon) return frac; // limit

					if (f.UserData is Bullet) return -1;// ignore _all_ Bullets

					if (f.UserData is IPhysicsMarker) return -1; // ignore

					result = (GameEntity)f.UserData;

					return 0; // terminate
				};

				Owner.GetPhysicsWorld().RayCast(callback, ray.Item1, ray.Item2);

				if (result != null) return false;
			}

			return true;
		}

		#endregion
	}
}
