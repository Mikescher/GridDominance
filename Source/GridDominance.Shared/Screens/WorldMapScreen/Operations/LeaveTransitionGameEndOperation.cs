using GridDominance.Shared.Screens.WorldMapScreen.Entities;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.ViewportAdapters;
using MonoSAMFramework.Portable.UpdateAgents.Impl;

namespace GridDominance.Shared.Screens.WorldMapScreen.Agents
{
	public class LeaveTransitionGameEndOperation : DecayOperation<GDWorldMapScreen>
	{
		private const float DURATION      = 1.00f; // sec
		private const float DURATION_SLOW = 1.60f; // sec

		private SAMViewportAdapter vp;

		private FRectangle rectStart;
		private FRectangle rectFinal;

		private readonly WarpGameEndNode _node;

		public override string Name => "LeaveTransitionGameEnd";

		public LeaveTransitionGameEndOperation(bool slower, WarpGameEndNode node) : base(slower ? DURATION_SLOW : DURATION)
		{
			_node = node;
		}

		protected override void OnInit(GDWorldMapScreen screen)
		{
			vp = screen.VAdapterGame;

			rectStart = screen.GuaranteedMapViewport;

			rectFinal = _node.DrawingBoundingRect.AsResized(0.5f, 0.5f);
		}

		protected override void OnStart(GDWorldMapScreen screen)
		{
			var bounds = rectStart;

			vp.ChangeVirtualSize(bounds.Width, bounds.Height);
			screen.MapViewportCenterX = bounds.CenterX;
			screen.MapViewportCenterY = bounds.CenterY;

			screen.ColorOverdraw = 0f;
		}

		protected override void OnDecayProgress(GDWorldMapScreen screen, float perc, SAMTime gameTime, InputState istate)
		{
			var bounds = FRectangle.Lerp(rectStart, rectFinal, FloatMath.FunctionEaseOutSine(perc));

			vp.ChangeVirtualSize(bounds.Width, bounds.Height);
			screen.MapViewportCenterX = bounds.CenterX;
			screen.MapViewportCenterY = bounds.CenterY;

			_node.ColorOverdraw = perc;
		}

		protected override void OnEnd(GDWorldMapScreen screen)
		{
			_node.ColorOverdraw = 1f;

			MainGame.Inst.SetGameEndScreen();
		}

		protected override void OnAbort(GDWorldMapScreen owner)
		{
			OnEnd(owner);
		}
	}
}
