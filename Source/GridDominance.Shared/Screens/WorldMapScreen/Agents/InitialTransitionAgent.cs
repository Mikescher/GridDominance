using System.Linq;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.WorldMapScreen.Background;
using GridDominance.Shared.Screens.WorldMapScreen.Entities;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Screens.Agents;
using MonoSAMFramework.Portable.Screens.ViewportAdapters;

namespace GridDominance.Shared.Screens.WorldMapScreen.Agents
{
	public class InitialTransitionAgent : DecayGameScreenAgent
	{
		private const float DURATION = 1.0f; // sec

		private readonly TolerantBoxingViewportAdapter vp;

		private readonly FRectangle rectStart;
		private readonly FRectangle rectFinal;

		private readonly GDWorldMapScreen _gdScreen;

		public InitialTransitionAgent(GDWorldMapScreen scrn) : base(scrn, DURATION)
		{
			_gdScreen = scrn;
			vp = (TolerantBoxingViewportAdapter) scrn.VAdapterGame;

			rectStart = scrn.Graph.BoundingViewport;

			var pos = scrn.GetInitialNode().Position;

			rectFinal = FRectangle.CreateByCenter(pos, GDConstants.VIEW_WIDTH, GDConstants.VIEW_HEIGHT);
		}

		protected override void Run(float perc)
		{
			var bounds = FRectangle.Lerp(rectStart, rectFinal, FloatMath.FunctionEaseOutSine(perc));

			vp.ChangeVirtualSize(bounds.Width, bounds.Height);
			Screen.MapViewportCenterX = bounds.CenterX;
			Screen.MapViewportCenterY = bounds.CenterY;

			_gdScreen.ColorOverdraw = FloatMath.FunctionEaseOutExpo(1 - perc, 10);
		}

		protected override void Start()
		{
			var bounds = rectStart;

			vp.ChangeVirtualSize(bounds.Width, bounds.Height);
			Screen.MapViewportCenterX = bounds.CenterX;
			Screen.MapViewportCenterY = bounds.CenterY;

			_gdScreen.ZoomState = BistateProgress.Reverting;

			_gdScreen.ColorOverdraw = 1f;
		}

		protected override void End()
		{
			_gdScreen.ZoomState = BistateProgress.Normal;

			_gdScreen.ColorOverdraw = 0f;
		}
	}
}
