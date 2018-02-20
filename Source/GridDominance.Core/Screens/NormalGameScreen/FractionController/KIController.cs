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
	public abstract class KIController : AbstractFractionController
	{
		protected class KIMethod
		{
			public readonly string Name;

			private readonly Func<GameEntity> _runDirect;
			private readonly Func<BulletPath> _runPrecalc;
			private readonly Func<LaserRay> _runAntiLaser;
			private readonly Action _runGeneric;
			private readonly Func<float?> _runCustom;

			private KIMethod(string n, Func<GameEntity> r1, Func<BulletPath> r2, Action r3, Func<LaserRay> r4, Func<float?> r5)
			{
				Name = n;
				_runDirect = r1;
				_runPrecalc = r2;
				_runGeneric = r3;
				_runAntiLaser = r4;
				_runCustom = r5;
			}

			public static KIMethod CreateRaycast(string n, Func<GameEntity> r) => new KIMethod(n, r,    null, null, null, null);
			public static KIMethod CreatePrecalc(string n, Func<BulletPath> r) => new KIMethod(n, null, r,    null, null, null);
			public static KIMethod CreateGeneric(string n, Action           r) => new KIMethod(n, null, null, r,    null, null);
			public static KIMethod CreateDefense(string n, Func<LaserRay>   r) => new KIMethod(n, null, null, null, r,    null);
			public static KIMethod CreateCustom( string n, Func<float?>     r) => new KIMethod(n, null, null, null, null, r   );

			public bool Run(KIController ki)
			{
				if (_runDirect != null)
				{
					var target = _runDirect();
					if (target != null)
					{
						var rot = FloatMath.PositiveAtan2(target.Position.Y - ki.Cannon.Position.Y, target.Position.X - ki.Cannon.Position.X);

						ki.Cannon.KITarget = target;

						if (ki.MinimumRotationalDelta > 0 && FloatMath.DiffRadiansAbs(rot, ki.Cannon.Rotation.TargetValue) < ki.MinimumRotationalDelta)
						{
							ki.LastKIFunction = "Ign["+Name+"]";
							return true;
						}

						ki.Cannon.Rotation.Set(rot);
						ki.LastKIFunction = Name;

						return true;
					}
				}
				else if (_runPrecalc != null)
				{
					var target = _runPrecalc();
					if (target != null)
					{
						ki.Cannon.KITarget = target.TargetCannon;

						if (ki.MinimumRotationalDelta > 0 && FloatMath.DiffRadiansAbs(target.CannonRotation, ki.Cannon.Rotation.TargetValue) < ki.MinimumRotationalDelta)
						{
							ki.LastKIFunction = "Ign[" + Name + "]";
							return true;
						}

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
						var rot = target.ToAngle(ki.Cannon.Position);

						ki.Cannon.KITarget = null;

						if (ki.MinimumRotationalDelta > 0 && FloatMath.DiffRadiansAbs(rot, ki.Cannon.Rotation.TargetValue) < ki.MinimumRotationalDelta/4f)
						{
							ki.LastKIFunction = "Ign[" + Name + "]";
							return true;
						}

						ki.Cannon.Rotation.Set(rot);
						ki.LastKIFunction = Name;

						return true;
					}
					return false;
				}
				else if (_runGeneric != null)
				{
					_runGeneric();
					return true;
				}
				else if (_runCustom != null)
				{
					var f =_runCustom();
					if (f != null)
					{
						var rot = f.Value;

						if (ki.MinimumRotationalDelta > 0 && FloatMath.DiffRadiansAbs(rot, ki.Cannon.Rotation.TargetValue) < ki.MinimumRotationalDelta / 4f)
						{
							ki.LastKIFunction = "Ign[" + Name + "]";
							return true;
						}

						ki.Cannon.Rotation.Set(rot);
						ki.LastKIFunction = Name;

						return true;
					}
					return false;
				}

				ki.Cannon.KITarget = null;
				return false;
			}
		}

		public    const float MAX_UPDATE_TIME      = 16.00f;
		protected const float STANDARD_UPDATE_TIME = 1.666f;
		protected const float RELAY_UPDATE_TIME    = 0.600f;
		protected const float LASER_UPDATE_TIME    = 0.400f;
		protected const float NEUTRAL_UPDATE_TIME  = 0.111f;
		protected const float MIN_LASER_ROT        = FloatMath.RAD_POS_002;
		public    const int   MAX_KI_RELAYCHAIN    = 8;

		private readonly ConstantRandom crng;
		public string LastKIFunction = "None";

		public readonly float MinimumRotationalDelta;
		
		protected KIController(float interval, GDGameScreen owner, Cannon cannon, Fraction fraction, float minRotDelta)
			: base(interval, owner, cannon, fraction, true)
		{
			MinimumRotationalDelta = minRotDelta;
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
			if (Cannon.IsShielded) return null;

			return Cannon
				.AttackingRays
				.RandomOrDefault(crng);
		}

		protected Bullet FindTargetAttackingBullet()
		{
			if (Cannon.IsShielded) return null;

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
				.Where(p => !p.IsShielded)
				.Where(p => p.Fraction == Fraction && p != Cannon)
				.Where(p => p != Cannon)
				.Where(p => p.CannonHealth.TargetValue < 0.5f)
				.Where(IsReachable)
				.WhereSmallestBy(p => (p.Position - Cannon.Position).Length(), GDConstants.TILE_WIDTH)
				.RandomOrDefault(crng);
		}

		protected Cannon FindTargetShieldCannon()
		{
			return Owner
				.GetEntities<Cannon>()
				.Where(p => p.Fraction == Fraction && p != Cannon)
				.Where(p => p != Cannon)
				.Where(p => p.CannonHealth.TargetValue > 0.8f)
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
				.Where(p => !p.IsShielded)
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
				.Where(p => !p.IsShielded)
				.Where(p => p.Fraction == Fraction)
				.Where(p => p != Cannon)
				.Where(p => !p.IsLaser && p.CannonHealth.TargetValue >= 1f)
				.Where(IsReachable)
				.WhereSmallestBy(p => (p.Position - Cannon.Position).Length(), GDConstants.TILE_WIDTH)
				.RandomOrDefault(crng);
		}

		protected Cannon FindTargetAnyFriendlyCannon()
		{
			return Owner
				.GetEntities<Cannon>()
				.Where(p => !p.IsShielded)
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
				.Where(p => !p.IsShielded)
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
				.Where(p => !p.IsShielded)
				.Where(p => p.Fraction == Fraction && p != Cannon)
				.Where(p => p != Cannon)
				.Where(p => !p.IsLaser && p.CannonHealth.TargetValue >= 1f)
				.Where(IsBulletBlockedReachable)
				.WhereSmallestBy(p => (p.Position - Cannon.Position).Length(), GDConstants.TILE_WIDTH)
				.RandomOrDefault(crng);
		}

		protected Cannon FindNearestEnemyCannon()
		{
			return Owner
				.GetEntities<Cannon>()
				.Where(p => !p.IsShielded)
				.Where(p => !p.Fraction.IsNeutral)
				.Where(p => p.Fraction != Fraction)
				.WhereSmallestBy(p => (p.Position - Cannon.Position).Length(), GDConstants.TILE_WIDTH)
				.RandomOrDefault(crng);
		}

		protected Cannon FindNearestFriendlyCannon()
		{
			return Owner
				.GetEntities<Cannon>()
				.Where(p => !p.IsShielded)
				.Where(p => p.Fraction == Fraction && p != Cannon)
				.Where(p => !p.IsLaser && p.CannonHealth.TargetValue >= 1f)
				.WhereSmallestBy(p => (p.Position - Cannon.Position).Length(), GDConstants.TILE_WIDTH)
				.RandomOrDefault(crng);
		}

		protected float? KeepAttackingEnemyCannon()
		{
			foreach (var ray in ((LaserCannon)Cannon).LaserSource.Lasers)
			{
				if (ray.Terminator == LaserRayTerminator.Target && ray.TargetCannon != null && ray.TargetCannon.Fraction != Cannon.Fraction && ray.TargetCannon.CannonHealth.TargetValue < 0.85f)
				{
					Cannon.KITarget = ray.TargetCannon;
					return Cannon.Rotation.TargetValue;
				}
			}
			return null;
		}

		protected float? KeepSupportingFriendlyCannon()
		{
			foreach (var ray in ((LaserCannon)Cannon).LaserSource.Lasers)
			{
				if (ray.Terminator == LaserRayTerminator.Target && ray.TargetCannon != null && ray.TargetCannon.Fraction == Cannon.Fraction && ray.TargetCannon.CannonHealth.TargetValue < 0.90f)
				{
					Cannon.KITarget = ray.TargetCannon;
					return Cannon.Rotation.TargetValue;
				}
			}
			return null;
		}

		protected Cannon FindTargetRelayChain()
		{
			return Owner
				.GetEntities<RelayCannon>()
				.Where(p => !p.IsShielded)
				.Where(p => !p.Fraction.IsNeutral)
				.Where(p => p.Fraction == Fraction && p != Cannon)
				.Where(IsReachable)
				.Where(p => p.IsSuccesfulRelayChaining())
				.WhereSmallestBy(p => (p.Position - Cannon.Position).Length(), GDConstants.TILE_WIDTH)
				.RandomOrDefault(crng);
		}

		#endregion

		#region Target Finding (Precalc)

		protected BulletPath FindTargetSupportCannonPrecalc()
		{
			return Cannon.BulletPaths
				.Where(p => !p.TargetCannon.IsShielded)
				.Where(p => p.TargetCannon.Fraction == Fraction)
				.Where(p => p.TargetCannon != Cannon)
				.Where(p => p.TargetCannon.CannonHealth.TargetValue < 0.5f)
				.Where(IsReachablePrecalc)
				.WhereSmallestBy(p => (p.TargetCannon.Position - Cannon.Position).Length(), GDConstants.TILE_WIDTH)
				.RandomOrDefault(crng);
		}

		protected BulletPath FindTargetShieldCannonPrecalc()
		{
			return Cannon.BulletPaths
				.Where(p => p.TargetCannon.Fraction == Fraction)
				.Where(p => p.TargetCannon != Cannon)
				.Where(p => p.TargetCannon.CannonHealth.TargetValue > 0.8f)
				.Where(IsReachablePrecalc)
				.WhereSmallestBy(p => (p.TargetCannon.Position - Cannon.Position).Length(), GDConstants.TILE_WIDTH)
				.RandomOrDefault(crng);
		}

		protected BulletPath FindTargetNeutralCannonPrecalc()
		{
			return Cannon.BulletPaths
				.Where(p => !p.TargetCannon.IsShielded)
				.Where(p => p.TargetCannon.Fraction.IsNeutral)
				.Where(IsReachablePrecalc)
				.WhereSmallestBy(p => (p.TargetCannon.Position - Cannon.Position).Length(), 2 * GDConstants.TILE_WIDTH)
				.RandomOrDefault(crng);
		}

		protected BulletPath FindTargetEnemyCannonPrecalc()
		{
			return Cannon.BulletPaths
				.Where(p => !p.TargetCannon.IsShielded)
				.Where(p => !p.TargetCannon.Fraction.IsNeutral)
				.Where(p => p.TargetCannon.Fraction != Fraction)
				.Where(IsReachablePrecalc)
				.WhereSmallestBy(p => (p.TargetCannon.Position - Cannon.Position).Length(), GDConstants.TILE_WIDTH)
				.RandomOrDefault(crng);
		}

		protected BulletPath FindTargetFriendlyCannonPrecalc()
		{
			return Cannon.BulletPaths
				.Where(p => !p.TargetCannon.IsShielded)
				.Where(p => p.TargetCannon.Fraction == Fraction)
				.Where(p => p.TargetCannon != Cannon)
				.Where(p => !p.TargetCannon.IsLaser && p.TargetCannon.CannonHealth.TargetValue >= 1f)
				.Where(IsReachablePrecalc)
				.WhereSmallestBy(p => (p.TargetCannon.Position - Cannon.Position).Length(), GDConstants.TILE_WIDTH)
				.RandomOrDefault(crng);
		}

		protected BulletPath FindTargetAnyFriendlyCannonPrecalc()
		{
			return Cannon.BulletPaths
				.Where(p => !p.TargetCannon.IsShielded)
				.Where(p => p.TargetCannon.Fraction == Fraction)
				.Where(p => p.TargetCannon != Cannon)
				.Where(IsReachablePrecalc)
				.WhereSmallestBy(p => (p.TargetCannon.Position - Cannon.Position).Length(), GDConstants.TILE_WIDTH)
				.RandomOrDefault(crng);
		}

		protected BulletPath FindTargetBlockedEnemyCannonPrecalc()
		{
			return Cannon.BulletPaths
				.Where(p => !p.TargetCannon.IsShielded)
				.Where(p => !p.TargetCannon.Fraction.IsNeutral)
				.Where(p => p.TargetCannon.Fraction != Fraction)
				.Where(IsBulletBlockedReachablePrecalc)
				.WhereSmallestBy(p => (p.TargetCannon.Position - Cannon.Position).Length(), GDConstants.TILE_WIDTH)
				.RandomOrDefault(crng);
		}

		protected BulletPath FindTargetBlockedFriendlyCannonPrecalc()
		{
			return Cannon.BulletPaths
				.Where(p => !p.TargetCannon.IsShielded)
				.Where(p => p.TargetCannon.Fraction == Fraction)
				.Where(p => p.TargetCannon != Cannon)
				.Where(p => !p.TargetCannon.IsLaser && p.TargetCannon.CannonHealth.TargetValue >= 1f)
				.Where(IsBulletBlockedReachablePrecalc)
				.WhereSmallestBy(p => (p.TargetCannon.Position - Cannon.Position).Length(), GDConstants.TILE_WIDTH)
				.RandomOrDefault(crng);
		}

		protected BulletPath FindTargetRelayChainPrecalc()
		{
			return Cannon.BulletPaths
				.Where(p => !p.TargetCannon.IsShielded)
				.Where(p => !p.TargetCannon.Fraction.IsNeutral)
				.Where(p => p.TargetCannon.Fraction == Fraction)
				.Where(p => p.TargetCannon is RelayCannon)
				.Where(IsReachablePrecalc)
				.Where(p => ((RelayCannon)p.TargetCannon).IsSuccesfulRelayChaining())
				.WhereSmallestBy(p => (p.TargetCannon.Position - Cannon.Position).Length(), GDConstants.TILE_WIDTH)
				.RandomOrDefault(crng);
		}

		#endregion

		#region Helper

		// ignore all bullets
		private bool IsHoming(Bullet b)
		{
			GameEntity result = null;

			float velo = b.PhysicsBody.LinearVelocity.Length();

			if (Math.Abs(velo) < FloatMath.EPSILON6) return false;

			float cbFunc(Fixture f, Vector2 pos, Vector2 normal, float frac)
			{
				if (f.UserData is Bullet) return -1; // ignore _all_ Bullets

				if (f.UserData is IPhysicsMarker) return -1; // ignore
				if (f.UserData is ShieldCollisionMarker) return -1; // ignore

				result = (GameEntity) f.UserData;
				return frac; // limit to this length
			}

			var rayStart = b.PhysicsBody.Position;
			var rayEnd = rayStart + b.PhysicsBody.LinearVelocity * ConvertUnits.ToSimUnits(GDConstants.VIEW_WIDTH) / b.PhysicsBody.LinearVelocity.Length();

			Owner.GetPhysicsWorld().RayCast(cbFunc, rayStart, rayEnd);

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

				var shieldData = f.UserData as ShieldCollisionMarker;
				if (shieldData != null && !shieldData.Active) return -1; // ignore deactivated shields

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

			float cbFunc(Fixture f, Vector2 pos, Vector2 normal, float frac)
			{
				if (f.UserData == Cannon) return -1; // ignore self;

				if (f.UserData == c) return frac; // limit

				if (f.UserData is Bullet) return -1; // ignore _all_ Bullets

				var shieldData = f.UserData as ShieldCollisionMarker;
				if (shieldData != null && !shieldData.Active) return -1; // ignore deactivated shields

				if (f.UserData is IPhysicsMarker) return -1; // ignore

				result = (GameEntity) f.UserData;

				return 0; // terminate
			}

			var rayStart = Cannon.PhysicsBody.Position;
			var rayEnd = c.PhysicsBody.Position;

			Owner.GetPhysicsWorld().RayCast(cbFunc, rayStart, rayEnd);

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

					var shieldData = f.UserData as ShieldCollisionMarker;
					if (shieldData != null && !shieldData.Active) return -1; // ignore deactivated shields

					if (f.UserData is IPhysicsMarker) return -1; // ignore

					result = (GameEntity)f.UserData;

					return 0; // terminate
				};
				
				Owner.GetPhysicsWorld().RayCast(callback, ray.Item1, ray.Item2);

				if (result != null) return false;

				foreach (var lsource in Owner.GetLaserNetwork().Sources)
				{
					if (lsource.UserData != Cannon)
					{
						foreach (var lray in lsource.Lasers)
						{
							if (Math2D.LineIntersection(lray.Start, lray.End, ray.Item1.ToFPoint(), ray.Item2.ToFPoint(), out _))
							{
								return false;
							}
						}
					}
				}
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

					var shieldData = f.UserData as ShieldCollisionMarker;
					if (shieldData != null && !shieldData.Active) return -1; // ignore deactivated shields

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
