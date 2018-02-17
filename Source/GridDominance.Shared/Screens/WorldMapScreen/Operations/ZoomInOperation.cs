using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.WorldMapScreen.Background;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.ViewportAdapters;
using MonoSAMFramework.Portable.UpdateAgents.Impl;

namespace GridDominance.Shared.Screens.WorldMapScreen.Agents
{
	public class ZoomInOperation : DecayOperation<GDWorldMapScreen>
	{
		private const float DURATION = 0.6f; // sec

		private SAMViewportAdapter vp;

		private FPoint centerPos;

		private FRectangle rectStart;
		private FRectangle rectFinal;

		public override string Name => "ZoomIn";

		public ZoomInOperation(FPoint pos) : base( DURATION)
		{
			centerPos = pos;
		}

		protected override void OnInit(GDWorldMapScreen screen)
		{
			vp = screen.VAdapterGame;

			rectStart = screen.GuaranteedMapViewport;

			rectFinal = FRectangle.CreateByCenter(centerPos, GDConstants.VIEW_WIDTH, GDConstants.VIEW_HEIGHT);
		}

		protected override void OnDecayProgress(GDWorldMapScreen screen, float perc, SAMTime gameTime, InputState istate)
		{
			var bounds = FRectangle.Lerp(rectStart, rectFinal, FloatMath.FunctionEaseOutQuad(perc));

			vp.ChangeVirtualSize(bounds.Width, bounds.Height);
			screen.MapViewportCenterX = bounds.CenterX;
			screen.MapViewportCenterY = bounds.CenterY;

			screen.GDBackground.GridLineAlpha = perc;
		}

		protected override void OnStart(GDWorldMapScreen screen)
		{
			screen.ZoomState = BistateProgress.Reverting;
			screen.GDBackground.GridLineAlpha = 0f;
		}

		protected override void OnEnd(GDWorldMapScreen screen)
		{
			screen.ZoomState = BistateProgress.Normal;
			screen.GDBackground.GridLineAlpha = 1f;

			vp.ChangeVirtualSize(rectFinal.Width, rectFinal.Height);
			screen.MapViewportCenterX = rectFinal.CenterX;
			screen.MapViewportCenterY = rectFinal.CenterY;
		}

		protected override void OnAbort(GDWorldMapScreen owner)
		{
			OnEnd(owner);
		}
	}
}
