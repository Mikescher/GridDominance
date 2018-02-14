using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.ViewportAdapters;
using MonoSAMFramework.Portable.UpdateAgents.Impl;

namespace GridDominance.Shared.Screens.WorldMapScreen.Agents
{
	public class ZoomOutAgent : DecayOperation<GDWorldMapScreen>
	{
		private const float DURATION = 1.0f; // sec

		private SAMViewportAdapter vp;

		private FRectangle rectStart;
		private FRectangle rectFinal;

		public override string Name => "ZoomOut";

		public ZoomOutAgent() : base(DURATION)
		{
			//
		}

		protected override void OnInit(GDWorldMapScreen screen)
		{
			vp = screen.VAdapterGame;

			rectStart = screen.GuaranteedMapViewport;
			rectFinal = screen.Graph.BoundingViewport;
		}

		protected override void OnDecayProgress(GDWorldMapScreen screen, float perc, SAMTime gameTime, InputState istate)
		{
			var bounds = FRectangle.Lerp(rectStart, rectFinal, FloatMath.FunctionEaseInOutCubic(perc));

			vp.ChangeVirtualSize(bounds.Width, bounds.Height);
			screen.MapViewportCenterX = bounds.CenterX;
			screen.MapViewportCenterY = bounds.CenterY;

			screen.GDBackground.GridLineAlpha = 1 - perc;
		}

		protected override void OnStart(GDWorldMapScreen screen)
		{
			screen.ZoomState = BistateProgress.Expanding;
			screen.GDBackground.GridLineAlpha = 1f;
		}

		protected override void OnEnd(GDWorldMapScreen screen)
		{
			screen.ZoomState = BistateProgress.Expanded;
			screen.GDBackground.GridLineAlpha = 0f;

			vp.ChangeVirtualSize(rectFinal.Width, rectFinal.Height);
			screen.MapViewportCenterX = rectFinal.CenterX;
			screen.MapViewportCenterY = rectFinal.CenterY;
		}
	}
}
