using System;
using System.Collections.Generic;
using System.Linq;
using GridDominance.Graphfileformat.Blueprint;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.Common.Agents;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using GridDominance.Shared.Screens.WorldMapScreen.Agents;
using GridDominance.Shared.Screens.WorldMapScreen.Background;
using GridDominance.Shared.Screens.WorldMapScreen.Entities;
using GridDominance.Shared.Screens.WorldMapScreen.HUD;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Background;
using MonoSAMFramework.Portable.Screens.Entities;
using MonoSAMFramework.Portable.Screens.HUD;
using MonoSAMFramework.Portable.DebugTools;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.GameMath.VectorPath;
using MonoSAMFramework.Portable.LogProtocol;
using MonoSAMFramework.Portable.Screens.Entities.Particles.GPUParticles;
using MonoSAMFramework.Portable.Screens.ViewportAdapters;

namespace GridDominance.Shared.Screens.WorldMapScreen
{
	public class GDWorldMapScreen : GameScreen
	{
		public const int VIEW_WIDTH = 16 * GDConstants.TILE_WIDTH;
		public const int VIEW_HEIGHT = 10 * GDConstants.TILE_WIDTH;

		public bool IsBackgroundPressed = false;
		public BistateProgress ZoomState = BistateProgress.Normal;
		public bool IsDragging = false;

		public readonly LevelGraph Graph;
		public readonly GraphBlueprint GraphBlueprint;
		public float ColorOverdraw = 0f;

		public GDWorldMapScreen(MonoSAMGame game, GraphicsDeviceManager gdm, GraphBlueprint g, Guid? initialFocus) : base(game, gdm)
		{
			Graph = new LevelGraph(this);
			GraphBlueprint = g;

			Initialize(g, initialFocus);
		}

		public GDWorldHUD GDHUD => (GDWorldHUD) HUD;
		public WorldMapBackground GDBackground => (WorldMapBackground) Background;

		protected override EntityManager CreateEntityManager() => new GDWorldMapEntityManager(this);
		protected override GameHUD CreateHUD() => new GDWorldHUD(this);
		protected override GameBackground CreateBackground() => new WorldMapBackground(this);
		protected override SAMViewportAdapter CreateViewport() => new TolerantBoxingViewportAdapter(Game.Window, Graphics, VIEW_WIDTH, VIEW_HEIGHT);
		protected override DebugMinimap CreateDebugMinimap() => new StandardDebugMinimapImplementation(this, 192, 32);
		protected override FRectangle CreateMapFullBounds() => new FRectangle(0, 0, 1, 1);
		protected override float GetBaseTextureScale() => Textures.DEFAULT_TEXTURE_SCALE_F;

		public ulong PreventZoomInCtr = 0;

		private void Initialize(GraphBlueprint g, Guid? initialFocus)
		{
#if DEBUG
			DebugUtils.CreateShortcuts(this);
			DebugDisp = DebugUtils.CreateDisplay(this);
#endif
			Graph.Init(g);
			MapFullBounds = Graph.BoundingRect.AsInflated(2 * GDConstants.TILE_WIDTH, 2 * GDConstants.TILE_WIDTH);

			AddAgent(new WorldMapDragAgent(GetEntities<LevelNode>().Select(n => n.Position).ToList()));

			if (initialFocus == null)
			{
				MapViewportCenterX = Graph.BoundingRect.CenterX;
				MapViewportCenterY = Graph.BoundingRect.CenterY;
			}
			else
			{
				var nd = Graph.Nodes.FirstOrDefault(n => n.ConnectionID == initialFocus);
				if (nd != null)
				{
					MapViewportCenterX = nd.Position.X;
					MapViewportCenterY = nd.Position.Y;
				}

			}

			GDBackground.InitBackground(GetEntities<LevelNode>().ToList(), MapFullBounds.InReferenceRaster(GDConstants.TILE_WIDTH).AsInflated(16,16).Truncate());
		}

		protected override void OnUpdate(SAMTime gameTime, InputState istate)
		{
#if DEBUG
			DebugDisp.IsEnabled = DebugSettings.Get("DebugTextDisplay");
			DebugDisp.Scale = 0.75f;
#endif

			if (ZoomState == BistateProgress.Expanded && istate.IsRealJustDown && PreventZoomInCtr != MonoSAMGame.GameCycleCounter && istate.SwallowConsumer != InputConsumer.HUDElement)
			{
				var zoompos = istate.GamePointerPositionOnMap;


				foreach (var n in GetEntities<LevelNode>())
				{
					if (n.NodeEnabled && n.StateSum == BistateProgress.Closed && n.CenterContains(istate.GamePointerPositionOnMap))
					{
						n.OpenNode();
						zoompos = n.Position;
						break;
					}
				}

				ZoomIn(zoompos);

			}

			if (ZoomState == BistateProgress.Normal && istate.IsGesturePinchComplete && istate.LastPinchPower < -10)
			{
				ZoomOut();
			}

			if (ZoomState == BistateProgress.Expanded && istate.IsGesturePinchComplete && istate.LastPinchPower > +10)
			{
				ZoomIn(istate.GamePointerPositionOnMap);
			}

			bool trigger = false;
			if (istate.IsKeyExclusiveJustDown(SKeys.AndroidBack))
			{
				istate.SwallowKey(SKeys.AndroidBack, InputConsumer.ScreenAgent);
				trigger = true;
			}
			else if (istate.IsKeyExclusiveJustDown(SKeys.Backspace))
			{
				istate.SwallowKey(SKeys.Backspace, InputConsumer.ScreenAgent);
				trigger = true;
			}

			if (trigger) MainGame.Inst.SetOverworldScreen();

#if DEBUG
			if (DebugSettings.Get("LeaveScreen"))
			{
				MainGame.Inst.SetOverworldScreen();
			}
#endif
		}

		protected override void OnDrawGame(IBatchRenderer sbatch)
		{
#if DEBUG
			if (DebugSettings.Get("DebugBackground"))
			{
				sbatch.DrawRectangle(Graph.BoundingRect, Color.OrangeRed, 3f);
				sbatch.DrawRectangle(Graph.BoundingViewport, Color.Purple, 3f);
			}
#endif

			if (ColorOverdraw > 0)
			{
				sbatch.FillRectangle(CompleteMapViewport, FlatColors.Background * ColorOverdraw);
			}
		}

		protected override void OnDrawHUD(IBatchRenderer sbatch)
		{
			//
		}

		public void ZoomOut()
		{
			if (ZoomState == BistateProgress.Expanding || ZoomState == BistateProgress.Expanded) return;
			if (GetAgents<ZoomOutAgent>().Any()) return;
			if (GetAgents<ZoomInAgent>().Any()) return;

			AddAgent(new ZoomOutAgent());

			GDHUD.SelectedNode?.CloseNode();
		}

		public void ZoomInstantOut()
		{
			if (ZoomState == BistateProgress.Expanding || ZoomState == BistateProgress.Expanded) return;
			if (GetAgents<ZoomOutAgent>().Any()) return;
			if (GetAgents<ZoomInAgent>().Any()) return;

			ZoomState = BistateProgress.Expanded;
			GDBackground.GridLineAlpha = 0f;

			var bounds = Graph.BoundingViewport;
			VAdapterGame.ChangeVirtualSize(bounds.Width, bounds.Height);
			MapViewportCenterX = bounds.CenterX;
			MapViewportCenterY = bounds.CenterY;

			GDHUD.SelectedNode?.CloseNode();
		}

		private void ZoomIn(FPoint mapPosCenter)
		{
			if (ZoomState == BistateProgress.Reverting || ZoomState == BistateProgress.Normal) return;
			if (GetAgents<ZoomOutAgent>().Any()) return;
			if (GetAgents<ZoomInAgent>().Any()) return;

			AddAgent(new ZoomInAgent(mapPosCenter));
		}

		public override void Resize(int width, int height)
		{
			base.Resize(width, height);

#if DEBUG
			var newQuality = Textures.GetPreferredQuality(Game.GraphicsDevice);
			if (newQuality != Textures.TEXTURE_QUALITY)
			{
				Textures.ChangeQuality(Game.Content, newQuality);
			}
#endif
		}
		
		public IWorldNode GetInitialNode()
		{
			var n = BlueprintAnalyzer.FindInitialNode(GraphBlueprint);
			if (n != null)
			{
				var l = Graph.FindNode(n);
				if (l != null) return l;
			}

			return Graph.InitialNode; // fallback
		}

		protected override void OnShow()
		{
			MainGame.Inst.GDSound.PlayMusicBackground();
		}
	}
}
