using GridDominance.Graphfileformat.Blueprint;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.OverworldScreen;
using GridDominance.Shared.Screens.OverworldScreen.Entities;
using GridDominance.Shared.Screens.WorldMapScreen.Background;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Screens.Agents;
using MonoSAMFramework.Portable.Screens.ViewportAdapters;

namespace GridDominance.Shared.Screens.WorldMapScreen.Agents
{
	public class TransitionZoomInToTutorialAgent : DecayGameScreenAgent
	{
		private const float DURATION = 0.5f; // sec

		private readonly TolerantBoxingViewportAdapter vp;

		private readonly FRectangle rectStart;
		private readonly FRectangle rectFinal;

		private readonly OverworldNode_Tutorial _gdNode;

		public TransitionZoomInToTutorialAgent(GDOverworldScreen scrn, OverworldNode_Tutorial node) : base(scrn, DURATION)
		{
			_gdNode = node;
			vp = (TolerantBoxingViewportAdapter) scrn.VAdapterGame;

			rectStart = scrn.GuaranteedMapViewport;

			rectFinal = FRectangle.CreateByCenter(node.Position, new FSize(1.8f * GDConstants.TILE_WIDTH, 1.8f * GDConstants.TILE_WIDTH))
				.SetRatioUnderfitKeepCenter(GDConstants.VIEW_WIDTH * 1f / GDConstants.VIEW_HEIGHT);
		}

		protected override void Run(float perc)
		{
			var bounds = FRectangle.Lerp(rectStart, rectFinal, FloatMath.FunctionEaseInSine(perc));

			vp.ChangeVirtualSize(bounds.Width, bounds.Height);
			Screen.MapViewportCenterX = bounds.CenterX;
			Screen.MapViewportCenterY = bounds.CenterY;

			_gdNode.AlphaOverride = 1 - FloatMath.FunctionEaseOutExpo(perc, 10);
		}

		protected override void Start()
		{
			_gdNode.AlphaOverride = 1;
		}

		protected override void End()
		{
			_gdNode.AlphaOverride = 0;

			MainGame.Inst.SetTutorialLevelScreen();
		}
	}
}
