using GridDominance.Shared.Resources;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.ViewportAdapters;
using MonoSAMFramework.Portable.UpdateAgents.Impl;

namespace GridDominance.Shared.Screens.WorldMapScreen.Agents
{
	public class InitialTransitionOperation : DecayOperation<GDWorldMapScreen>
	{
		private const float DURATION = 1.0f; // sec

		private SAMViewportAdapter vp;

		private FRectangle rectStart;
		private FRectangle rectFinal;

		public override string Name => "InitialTransition";

		public InitialTransitionOperation() : base(DURATION)
		{
		}

		protected override void OnInit(GDWorldMapScreen screen)
		{
			vp = screen.VAdapterGame;

			rectStart = screen.Graph.BoundingViewport;

			var pos = screen.GetInitialNode().Position;

			rectFinal = FRectangle.CreateByCenter(pos, GDConstants.VIEW_WIDTH, GDConstants.VIEW_HEIGHT);
		}

		protected override void OnStart(GDWorldMapScreen screen)
		{
			var bounds = rectStart;

			vp.ChangeVirtualSize(bounds.Width, bounds.Height);
			screen.MapViewportCenterX = bounds.CenterX;
			screen.MapViewportCenterY = bounds.CenterY;

			screen.ZoomState = BistateProgress.Reverting;

			screen.ColorOverdraw = 1f;
		}

		protected override void OnDecayProgress(GDWorldMapScreen screen, float perc, SAMTime gameTime, InputState istate)
		{
			var bounds = FRectangle.Lerp(rectStart, rectFinal, FloatMath.FunctionEaseOutSine(perc));

			vp.ChangeVirtualSize(bounds.Width, bounds.Height);
			screen.MapViewportCenterX = bounds.CenterX;
			screen.MapViewportCenterY = bounds.CenterY;

			screen.ColorOverdraw = FloatMath.FunctionEaseOutExpo(1 - perc, 10);
		}

		protected override void OnEnd(GDWorldMapScreen screen)
		{
			screen.ZoomState = BistateProgress.Normal;

			screen.ColorOverdraw = 0f;
		}

		protected override void OnAbort(GDWorldMapScreen owner)
		{
			OnEnd(owner);
		}
	}
}
