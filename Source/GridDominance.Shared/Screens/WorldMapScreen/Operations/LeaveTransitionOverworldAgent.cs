using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.ViewportAdapters;
using MonoSAMFramework.Portable.UpdateAgents.Impl;

namespace GridDominance.Shared.Screens.WorldMapScreen.Agents
{
	public class LeaveTransitionOverworldAgent : DecayOperation<GDWorldMapScreen>
	{
		private const float DURATION      = 1.00f; // sec
		private const float DURATION_FAST = 0.25f; // sec

		private SAMViewportAdapter vp;

		private FRectangle rectStart;
		private FRectangle rectFinal;

		public override string Name => "LeaveTransitionOverworld";

		public LeaveTransitionOverworldAgent(bool faster) : base(faster ? DURATION_FAST : DURATION)
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
			var bounds = FRectangle.Lerp(rectStart, rectFinal, FloatMath.FunctionEaseOutSine(perc));

			vp.ChangeVirtualSize(bounds.Width, bounds.Height);
			screen.MapViewportCenterX = bounds.CenterX;
			screen.MapViewportCenterY = bounds.CenterY;

			screen.ColorOverdraw = FloatMath.FunctionEaseOutExpo(perc, 10);
		}

		protected override void OnStart(GDWorldMapScreen screen)
		{
			var bounds = rectStart;

			vp.ChangeVirtualSize(bounds.Width, bounds.Height);
			screen.MapViewportCenterX = bounds.CenterX;
			screen.MapViewportCenterY = bounds.CenterY;

			screen.ZoomState = BistateProgress.Expanding;

			screen.ColorOverdraw = 0f;
		}

		protected override void OnEnd(GDWorldMapScreen screen)
		{
			screen.ZoomState = BistateProgress.Expanded;

			screen.ColorOverdraw = 1f;

			MainGame.Inst.SetOverworldScreenWithTransition(screen.GraphBlueprint);
		}

		protected override void OnAbort(GDWorldMapScreen owner)
		{
			OnEnd(owner);
		}
	}
}
