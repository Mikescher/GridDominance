using GridDominance.Graphfileformat.Blueprint;
using GridDominance.Shared.Screens.WorldMapScreen.Entities;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Screens.Agents;
using MonoSAMFramework.Portable.Screens.ViewportAdapters;

namespace GridDominance.Shared.Screens.WorldMapScreen.Agents
{
	public class LeaveTransitionGameEndAgent : DecayGameScreenAgent
	{
		private const float DURATION      = 1.00f; // sec
		private const float DURATION_SLOW = 1.60f; // sec

		private readonly SAMViewportAdapter vp;

		private readonly FRectangle rectStart;
		private readonly FRectangle rectFinal;

		private readonly GDWorldMapScreen _gdScreen;
		private readonly WarpGameEndNode _node;

		public LeaveTransitionGameEndAgent(GDWorldMapScreen scrn, bool slower, WarpGameEndNode node) : base(scrn, slower ? DURATION_SLOW : DURATION)
		{
			_gdScreen = scrn;
			_node = node;
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

			MainGame.Inst.SetGameEndScreen();
		}
	}
}
