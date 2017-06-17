using System;
using System.Collections.Generic;
using GridDominance.Shared.Screens.NormalGameScreen.Entities;
using MonoSAMFramework.Portable.GameMath.Geometry;

namespace GridDominance.Shared.Screens.NormalGameScreen.LaserNetwork
{
	public enum LaserRayTerminator { OOB, VoidObject, Portal, Glass, Mirror, Target, LaserMultiTerm, LaserSelfTerm, LaserFaultTerm, BulletTerm }
	
	public sealed class LaserRay
	{
		public FPoint Start;
		public FPoint End;

		public LaserRayTerminator Terminator;

		public Cannon TargetCannon;

		public List<Tuple<LaserRay, LaserSource>> TerminatorRays;         // Rays that directcollide with this one
		public List<LaserRay> SelfCollRays = new List<LaserRay>(); // Rays that [[LaserSelfTerm]] with this one

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

		public LaserRay(FPoint s, FPoint e, LaserRay src, LaserRayTerminator t, int d, bool g, object sign, object eign, float sd, Cannon tc)
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
		}

		public void SetLaserIntersect(FPoint e, LaserRay otherRay, LaserSource otherSource, LaserRayTerminator t)
		{
			End = e;
			Terminator = t;
			_length = null;
			_angle = null;

			TerminatorRays.Add(Tuple.Create(otherRay, otherSource));
		}

		public void SetLaserCollisionlessIntersect(FPoint e, LaserRay otherRay, LaserSource otherSource, LaserRayTerminator t)
		{
			End = e;
			Terminator = t;
			_length = null;
			_angle = null;

			TerminatorRays.Clear();
		}
	}
}
