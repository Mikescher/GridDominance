using System;
using GridDominance.Shared.Screens.NormalGameScreen.Entities;
using Microsoft.Xna.Framework;

namespace GridDominance.Shared.Screens.NormalGameScreen.LaserNetwork
{
	public enum LaserRayTerminator { OOB, VoidObject, Portal, Glass, Mirror, Target, LaserDoubleTerm, LaserSelfTerm }
	
	public sealed class LaserRay
	{
		public Vector2 Start;
		public Vector2 End;

		public LaserRay Source;
		public LaserRayTerminator Terminator;

		public Cannon TerminatorCannon;
		public Tuple<LaserRay, LaserSource> TerminatorRay;

		public int Depth;
		public bool InGlass;
		public object StartIgnoreObj;

		public LaserRay(Vector2 s, Vector2 e, LaserRay src, LaserRayTerminator t, int d, bool g, object ign, Cannon tc, Tuple<LaserRay, LaserSource> tr)
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
		}

		public void SetLaserIntersect(Vector2 e, LaserRay otherRay, LaserSource otherSource)
		{
			End = e;
			Terminator = LaserRayTerminator.LaserDoubleTerm;
			TerminatorCannon = null;
			TerminatorRay = Tuple.Create(otherRay, otherSource);
		}

		public void SetSelfLaserIntersect(Vector2 e, LaserRay otherRay, LaserSource otherSource)
		{
			End = e;
			Terminator = LaserRayTerminator.LaserSelfTerm;
			TerminatorCannon = null;
			TerminatorRay = Tuple.Create(otherRay, otherSource);
		}
	}
}
