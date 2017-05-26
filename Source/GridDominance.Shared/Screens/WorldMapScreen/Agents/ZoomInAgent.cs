using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.WorldMapScreen.Background;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Screens.Agents;
using MonoSAMFramework.Portable.Screens.ViewportAdapters;

namespace GridDominance.Shared.Screens.WorldMapScreen.Agents
{
	public class ZoomInAgent : DecayGameScreenAgent
	{
		private const float DURATION = 0.6f; // sec

		private readonly TolerantBoxingViewportAdapter vp;

		private readonly FRectangle rectStart;
		private readonly FRectangle rectFinal;

		private readonly GDWorldMapScreen _gdScreen;

		public ZoomInAgent(GDWorldMapScreen scrn, FPoint pos) : base(scrn, DURATION)
		{
			_gdScreen = scrn;
			vp = (TolerantBoxingViewportAdapter) scrn.VAdapterGame;

			rectStart = scrn.GuaranteedMapViewport;

			rectFinal = FRectangle.CreateByCenter(pos, GDConstants.VIEW_WIDTH, GDConstants.VIEW_HEIGHT);
		}

		protected override void Run(float perc)
		{
			var bounds = FRectangle.Lerp(rectStart, rectFinal, FloatMath.FunctionEaseOutQuad(perc));

			vp.ChangeVirtualSize(bounds.Width, bounds.Height);
			Screen.MapViewportCenterX = bounds.CenterX;
			Screen.MapViewportCenterY = bounds.CenterY;

			((WorldMapBackground)_gdScreen.Background).GridLineAlpha = perc;
		}

		protected override void Start()
		{
			_gdScreen.ZoomState = BistateProgress.Reverting;
			((WorldMapBackground) _gdScreen.Background).GridLineAlpha = 0f;
		}

		protected override void End()
		{
			_gdScreen.ZoomState = BistateProgress.Normal;
			((WorldMapBackground)_gdScreen.Background).GridLineAlpha = 1f;

			vp.ChangeVirtualSize(rectFinal.Width, rectFinal.Height);
			Screen.MapViewportCenterX = rectFinal.CenterX;
			Screen.MapViewportCenterY = rectFinal.CenterY;
		}
	}
}
