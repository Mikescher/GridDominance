using System.Linq;
using GridDominance.Graphfileformat.Blueprint;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.OverworldScreen;
using GridDominance.Shared.Screens.OverworldScreen.Entities;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.ViewportAdapters;
using MonoSAMFramework.Portable.UpdateAgents.Impl;

namespace GridDominance.Shared.Screens.WorldMapScreen.Agents
{
	public class ReappearTransitionOperation : FixTimeOperation<GDOverworldScreen>
	{
		private const float DURATION = 0.5f; // sec

		private SAMViewportAdapter vp;

		private FRectangle rectStart;
		private FRectangle rectFinal;

		private OverworldNode _gdNode;

		private readonly GraphBlueprint _focus;

		public override string Name => "ReappearTransition";

		public ReappearTransitionOperation(GraphBlueprint g) : base(DURATION)
		{
			_focus = g;
		}

		protected override void OnInit(GDOverworldScreen screen)
		{
			_gdNode = screen.GetEntities<OverworldNode>().First(n => n.ContentID == _focus.ID);
			vp = screen.VAdapterGame;

			rectStart = FRectangle.CreateByCenter(_gdNode.Position, new FSize(1.8f * GDConstants.TILE_WIDTH, 1.8f * GDConstants.TILE_WIDTH))
				.SetRatioUnderfitKeepCenter(GDConstants.VIEW_WIDTH * 1f / GDConstants.VIEW_HEIGHT);

			rectFinal = screen.GuaranteedMapViewport;
		}

		protected override void OnProgress(GDOverworldScreen screen, float perc, SAMTime gameTime, InputState istate)
		{
			var bounds = FRectangle.Lerp(rectStart, rectFinal, FloatMath.FunctionEaseInSine(perc));

			vp.ChangeVirtualSize(bounds.Width, bounds.Height);
			screen.MapViewportCenterX = bounds.CenterX;
			screen.MapViewportCenterY = bounds.CenterY;

			_gdNode.AlphaOverride = FloatMath.FunctionEaseInExpo(perc, 10);
		}

		protected override void OnStart(GDOverworldScreen screen)
		{
			_gdNode.AlphaOverride = 0;

			foreach (var node in screen.GetEntities<OverworldNode>())
			{
				node.FlickerTime = OverworldNode.COLLAPSE_TIME * 10; // no flicker - for sure
			}
		}

		protected override void OnAbort(GDOverworldScreen owner)
		{
			_gdNode.AlphaOverride = 1;
		}
	}
}
