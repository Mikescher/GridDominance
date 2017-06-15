using System;
using System.Collections.Generic;
using GridDominance.Shared.Screens.NormalGameScreen.Entities;
using Microsoft.Xna.Framework;

namespace GridDominance.Shared.Screens.NormalGameScreen.LaserNetwork
{
	public enum LaserRayTerminator { OOB, VoidObject, Portal, Glass, Mirror, Target, LaserDoubleTerm, LaserSelfTerm, LaserFaultTerm }
	
	public sealed class LaserRay
	{
		public Vector2 Start;
		public Vector2 End;

		public LaserRayTerminator Terminator;

		public Cannon TerminatorCannon;
		public Tuple<LaserRay, LaserSource> TerminatorRay;
		public List<LaserRay> SelfCollRays = new List<LaserRay>(); // Rays that [[LaserSelfTerm]] with this one

		public readonly LaserRay Source;
		public readonly int Depth;
		public readonly bool InGlass;
		public readonly object StartIgnoreObj;
		public readonly float SourceDistance; // At [[Start]]

		public float Length => (End - Start).Length();

		public LaserRay(Vector2 s, Vector2 e, LaserRay src, LaserRayTerminator t, int d, bool g, object ign, float sd, Cannon tc, Tuple<LaserRay, LaserSource> tr)
		{
			Depth = d;
			InGlass = g;
			StartIgnoreObj = ign;
			Start = s;
			End = e;
			Source = src;
			Terminator = t;
			TerminatorCannon = tc;
			TerminatorRay = tr;
			SourceDistance = sd;
		}

		public void SetLaserIntersect(Vector2 e, LaserRay otherRay, LaserSource otherSource, LaserRayTerminator t)
		{
			End = e;
			Terminator = t;
			TerminatorCannon = null;
			TerminatorRay = Tuple.Create(otherRay, otherSource);
		}
	}
}
