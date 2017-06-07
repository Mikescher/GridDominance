using System.Linq;
using GridDominance.Graphfileformat.Blueprint;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.OverworldScreen;
using GridDominance.Shared.Screens.OverworldScreen.Entities;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Screens.Agents;
using MonoSAMFramework.Portable.Screens.ViewportAdapters;

namespace GridDominance.Shared.Screens.WorldMapScreen.Agents
{
	public class ReappearTransitionAgent : DecayGameScreenAgent
	{
		private const float DURATION = 0.5f; // sec

		private readonly TolerantBoxingViewportAdapter vp;

		private readonly FRectangle rectStart;
		private readonly FRectangle rectFinal;

		private readonly OverworldNode _gdNode;

		public ReappearTransitionAgent(GDOverworldScreen scrn, GraphBlueprint g) : base(scrn, DURATION)
		{
			_gdNode = scrn.GetEntities<OverworldNode>().First(n => n.ContentID == g.ID);
			vp = (TolerantBoxingViewportAdapter) scrn.VAdapterGame;

			rectStart = FRectangle.CreateByCenter(_gdNode.Position, new FSize(1.8f * GDConstants.TILE_WIDTH, 1.8f * GDConstants.TILE_WIDTH))
				.SetRatioUnderfitKeepCenter(GDConstants.VIEW_WIDTH * 1f / GDConstants.VIEW_HEIGHT);

			rectFinal = scrn.GuaranteedMapViewport;
		}

		protected override void Run(float perc)
		{
			var bounds = FRectangle.Lerp(rectStart, rectFinal, FloatMath.FunctionEaseInSine(perc));

			vp.ChangeVirtualSize(bounds.Width, bounds.Height);
			Screen.MapViewportCenterX = bounds.CenterX;
			Screen.MapViewportCenterY = bounds.CenterY;

			_gdNode.AlphaOverride = FloatMath.FunctionEaseInExpo(perc, 10);
		}

		protected override void Start()
		{
			_gdNode.AlphaOverride = 0;

			foreach (var node in Screen.GetEntities<OverworldNode>())
			{
				node.FlickerTime = OverworldNode.COLLAPSE_TIME * 10; // no flicker - for sure
			}
		}

		protected override void End()
		{
			_gdNode.AlphaOverride = 1;
		}
	}
}
