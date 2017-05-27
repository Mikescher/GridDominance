using GridDominance.Shared.Screens.WorldMapScreen.Background;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Screens.Agents;
using MonoSAMFramework.Portable.Screens.ViewportAdapters;

namespace GridDominance.Shared.Screens.WorldMapScreen.Agents
{
	public class ZoomOutAgent : DecayGameScreenAgent
	{
		private const float DURATION = 1.0f; // sec

		private readonly TolerantBoxingViewportAdapter vp;

		private readonly FRectangle rectStart;
		private readonly FRectangle rectFinal;

		private readonly GDWorldMapScreen _gdScreen;

		public ZoomOutAgent(GDWorldMapScreen scrn) : base(scrn, DURATION)
		{
			_gdScreen = scrn;
			vp = (TolerantBoxingViewportAdapter) scrn.VAdapterGame;

			rectStart = scrn.GuaranteedMapViewport;

			rectFinal = scrn.Graph.BoundingViewport;
		}

		protected override void Run(float perc)
		{
			var bounds = FRectangle.Lerp(rectStart, rectFinal, FloatMath.FunctionEaseInOutCubic(perc));

			vp.ChangeVirtualSize(bounds.Width, bounds.Height);
			Screen.MapViewportCenterX = bounds.CenterX;
			Screen.MapViewportCenterY = bounds.CenterY;

			_gdScreen.GDBackground.GridLineAlpha = 1 - perc;
		}

		protected override void Start()
		{
			((GDWorldMapScreen)Screen).ZoomState = BistateProgress.Expanding;
			_gdScreen.GDBackground.GridLineAlpha = 1f;
		}

		protected override void End()
		{
			((GDWorldMapScreen)Screen).ZoomState = BistateProgress.Expanded;
			_gdScreen.GDBackground.GridLineAlpha = 0f;

			vp.ChangeVirtualSize(rectFinal.Width, rectFinal.Height);
			Screen.MapViewportCenterX = rectFinal.CenterX;
			Screen.MapViewportCenterY = rectFinal.CenterY;
		}
	}
}
