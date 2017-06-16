using System;
using System.Collections.Generic;
using GridDominance.Shared.Screens.NormalGameScreen.Entities;
using MonoSAMFramework.Portable.GameMath.Geometry;

namespace GridDominance.Shared.Screens.NormalGameScreen.LaserNetwork
{
	public enum LaserRayTerminator { OOB, VoidObject, Portal, Glass, Mirror, Target, LaserDoubleTerm, LaserSelfTerm, LaserFaultTerm }
	
	public sealed class LaserRay
	{
		public FPoint Start;
		public FPoint End;

		public LaserRayTerminator Terminator;

		public Cannon TerminatorCannon;
		public Tuple<LaserRay, LaserSource> TerminatorRay;
		public List<LaserRay> SelfCollRays = new List<LaserRay>(); // Rays that [[LaserSelfTerm]] with this one

		public readonly LaserRay Source;
		public readonly int Depth;
		public readonly bool InGlass;
		public readonly object StartIgnoreObj;
		public readonly object EndIgnoreObj;
		public readonly float SourceDistance; // At [[Start]]

		public float Length => (End - Start).Length();

		public LaserRay(FPoint s, FPoint e, LaserRay src, LaserRayTerminator t, int d, bool g, object sign, object eign, float sd, Cannon tc, Tuple<LaserRay, LaserSource> tr)
		{
			Depth = d;
			InGlass = g;
			StartIgnoreObj = sign;
			EndIgnoreObj   = eign;
			Start = s;
			End = e;
			Source = src;
			Terminator = t;
			TerminatorCannon = tc;
			TerminatorRay = tr;
			SourceDistance = sd;
		}

		public void SetLaserIntersect(FPoint e, LaserRay otherRay, LaserSource otherSource, LaserRayTerminator t)
		{
			End = e;
			Terminator = t;
			TerminatorCannon = null;
			TerminatorRay = Tuple.Create(otherRay, otherSource);
		}
	}
}
