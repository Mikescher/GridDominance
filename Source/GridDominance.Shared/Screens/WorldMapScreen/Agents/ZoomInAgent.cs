using System.Linq;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.WorldMapScreen.Entities;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Agents;
using MonoSAMFramework.Portable.Screens.ViewportAdapters;

namespace GridDominance.Shared.Screens.WorldMapScreen.Agents
{
	public class ZoomInAgent : DecayGameScreenAgent
	{
		private const float DURATION = 1.8f; // sec

		private readonly TolerantBoxingViewportAdapter vp;

		private readonly FRectangle rectStart;
		private readonly FRectangle rectFinal;

		public ZoomInAgent(GameScreen scrn, FPoint pos) : base(scrn, DURATION)
		{
			vp = (TolerantBoxingViewportAdapter) scrn.VAdapterGame;

			rectStart = scrn.GuaranteedMapViewport;

			rectFinal = FRectangle.CreateByCenter(pos, GDConstants.VIEW_WIDTH, GDConstants.VIEW_HEIGHT);
		}

		protected override void Run(float perc)
		{
			var bounds = FRectangle.Lerp(rectStart, rectFinal, FloatMath.FunctionEaseLinear(perc));

			vp.ChangeVirtualSize(bounds.Width, bounds.Height);
			Screen.MapViewportCenterX = bounds.CenterX;
			Screen.MapViewportCenterY = bounds.CenterY;

			Screen.DebugDisp.AddDecayLine(">" + perc, 0.5f, 0, 0);
		}
	}
}
