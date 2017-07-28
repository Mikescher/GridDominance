using GridDominance.Graphfileformat.Blueprint;
using GridDominance.Shared.Screens.WorldMapScreen.Entities;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Screens.Agents;
using MonoSAMFramework.Portable.Screens.ViewportAdapters;

namespace GridDominance.Shared.Screens.WorldMapScreen.Agents
{
	public class LeaveTransitionWorldMapAgent : DecayGameScreenAgent
	{
		private const float DURATION      = 1.00f; // sec
		private const float DURATION_SLOW = 1.60f; // sec

		private readonly SAMViewportAdapter vp;

		private readonly FRectangle rectStart;
		private readonly FRectangle rectFinal;

		private readonly GDWorldMapScreen _gdScreen;
		private readonly WarpNode _node;
		private readonly GraphBlueprint _target;

		public LeaveTransitionWorldMapAgent(GDWorldMapScreen scrn, bool slower, WarpNode node, GraphBlueprint target) : base(scrn, slower ? DURATION_SLOW : DURATION)
		{
			_gdScreen = scrn;
			_node = node;
			_target = target;
			vp = scrn.VAdapterGame;

			rectStart = scrn.GuaranteedMapViewport;

			rectFinal = node.DrawingBoundingRect.AsResized(0.5f, 0.5f);
		}

		protected override void Run(float perc)
		{
			var bounds = FRectangle.Lerp(rectStart, rectFinal, FloatMath.FunctionEaseOutSine(perc));

			vp.ChangeVirtualSize(bounds.Width, bounds.Height);
			Screen.MapViewportCenterX = bounds.CenterX;
			Screen.MapViewportCenterY = bounds.CenterY;

			_node.ColorOverdraw = perc;
		}

		protected override void Start()
		{
			var bounds = rectStart;

			vp.ChangeVirtualSize(bounds.Width, bounds.Height);
			Screen.MapViewportCenterX = bounds.CenterX;
			Screen.MapViewportCenterY = bounds.CenterY;

			_gdScreen.ColorOverdraw = 0f;
		}

		protected override void End()
		{
			_node.ColorOverdraw = 1f;

			MainGame.Inst.SetWorldMapScreenWithTransition(_target);
		}
	}
}
