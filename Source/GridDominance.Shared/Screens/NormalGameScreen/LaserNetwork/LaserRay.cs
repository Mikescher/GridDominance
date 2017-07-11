using System;
using System.Collections.Generic;
using GridDominance.Shared.Screens.NormalGameScreen.Entities;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.LogProtocol;

namespace GridDominance.Shared.Screens.NormalGameScreen.LaserNetwork
{
	public enum LaserRayTerminator { OOB, VoidObject, Portal, Glass, Mirror, Target, LaserMultiTerm, LaserSelfTerm, LaserFaultTerm, BulletTerm }

	public enum RayType { Laser, Shield }

	public sealed class LaserRay
	{
		public FPoint Start;
		public FPoint End;

		public LaserRayTerminator Terminator;

		public Cannon TargetCannon;

		public List<Tuple<LaserRay, LaserSource>> TerminatorRays;         // Rays that directcollide with this one
		public List<LaserRay> SelfCollRays = new List<LaserRay>(); // Rays that [[LaserSelfTerm]] with this one
		public Bullet TerminatorBullet;

		public readonly RayType RayType;
		public readonly LaserRay Source;
		public readonly int Depth;
		public readonly bool InGlass;
		public readonly object StartIgnoreObj;
		public readonly object EndIgnoreObj;
		public readonly float SourceDistance; // At [[Start]]

		private float? _length = null;
		public float Length => _length ?? (_length = (End - Start).Length()) ?? 0;

		private float? _angle = null;
		public float Angle => _angle ?? (_angle = End.ToAngle(Start)) ?? 0;

		public LaserRay(FPoint s, FPoint e, LaserRay src, LaserRayTerminator t, int d, bool g, object sign, object eign, float sd, Cannon tc, RayType rt)
		{
			Depth = d;
			InGlass = g;
			StartIgnoreObj = sign;
			EndIgnoreObj   = eign;
			Start = s;
			End = e;
			Source = src;
			Terminator = t;
			TargetCannon = tc;
			TerminatorRays = new List<Tuple<LaserRay, LaserSource>>();
			SourceDistance = sd;
			RayType = rt;

#if DEBUG
			if (!Start.IsValid) SAMLog.Error("LASER::Assert_1-SV", "!Start.IsValid");
			if (!End.IsValid) SAMLog.Error("LASER::Assert_1-EV", "!End.IsValid");
			if ((End - Start).LengthSquared() < FloatMath.EPSILON7 * FloatMath.EPSILON7) SAMLog.Error("LASER::Assert_1-ESV", "(End - Start).LengthSquared() < FloatMath.EPSILON7 * FloatMath.EPSILON7");
#endif
		}

		public LaserRay SetLaserIntersect(LaserSource src, FPoint e, LaserRay otherRay, LaserSource otherSource, LaserRayTerminator t)
		{
			if ((Start - e).LengthSquared() < FloatMath.EPSILON4)
			{
				src.Lasers.Remove(this);
				return Source?.SetLaserIntersect(src, Source.End, otherRay, otherSource, t);
			}
			
			End = e;
			Terminator = t;
			_length = null;
			_angle = null;

			TerminatorRays.Add(Tuple.Create(otherRay, otherSource));

#if DEBUG
			if (!Start.IsValid) SAMLog.Error("LASER::Assert_2-SV", "!Start.IsValid");
			if (!End.IsValid) SAMLog.Error("LASER::Assert_2-EV", "!End.IsValid");
			if ((End - Start).LengthSquared() < FloatMath.EPSILON7 * FloatMath.EPSILON7) SAMLog.Error("LASER::Assert_2-ESV", "(End - Start).LengthSquared() < FloatMath.EPSILON7 * FloatMath.EPSILON7");
#endif

			return this;
		}

		public LaserRay SetLaserCollisionlessIntersect(LaserSource src, FPoint e, LaserRayTerminator t)
		{
			if ((Start - e).LengthSquared() < FloatMath.EPSILON4)
			{
				src.Lasers.Remove(this);

				return Source?.SetLaserCollisionlessIntersect(src, Source.End, t);
			}
			
			End = e;
			Terminator = t;
			_length = null;
			_angle = null;

			TerminatorRays.Clear();

#if DEBUG
			if (!Start.IsValid) SAMLog.Error("LASER::Assert_3-SV", "!Start.IsValid");
			if (!End.IsValid) SAMLog.Error("LASER::Assert_3-EV", "!End.IsValid");
			if ((End - Start).LengthSquared() < FloatMath.EPSILON7 * FloatMath.EPSILON7) SAMLog.Error("LASER::Assert_3-ESV", "(End - Start).LengthSquared() < FloatMath.EPSILON7 * FloatMath.EPSILON7");
#endif

			return this;
		}
	}
}
