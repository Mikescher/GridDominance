using System;
using System.Linq;
using GridDominance.Levelformat.Parser;
using GridDominance.Shared.Resources;
using GridDominance.Shared.SaveData;
using GridDominance.Shared.Screens.WorldMapScreen.Agents;
using GridDominance.Shared.Screens.WorldMapScreen.Background;
using GridDominance.Shared.Screens.WorldMapScreen.Entities;
using GridDominance.Shared.Screens.WorldMapScreen.HUD;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable;
using MonoSAMFramework.Portable.BatchRenderer;
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
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens.Entities.Particles;
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

		public readonly LevelGraph Graph;

		public GDWorldMapScreen(MonoSAMGame game, GraphicsDeviceManager gdm) : base(game, gdm)
		{
			Graph = new LevelGraph(this);

			Initialize();
		}

		protected GDWorldHUD GDHUD => (GDWorldHUD) HUD;

		protected override EntityManager CreateEntityManager() => new GDWorldMapEntityManager(this);
		protected override GameHUD CreateHUD() => new GDWorldHUD(this);
		protected override GameBackground CreateBackground() => new WorldMapBackground(this);
		protected override SAMViewportAdapter CreateViewport() => new TolerantBoxingViewportAdapter(Game.Window, Graphics, VIEW_WIDTH, VIEW_HEIGHT);
		protected override DebugMinimap CreateDebugMinimap() => new GDWorldMapDebugMinimap(this);
		protected override FRectangle CreateMapFullBounds() => new FRectangle(-8, -8, 48, 48) * GDConstants.TILE_WIDTH;
		protected override float GetBaseTextureScale() => Textures.DEFAULT_TEXTURE_SCALE_F;

		private void Initialize()
		{
#if DEBUG
			DebugUtils.CreateShortcuts(this);
			DebugDisp = DebugUtils.CreateDisplay(this);
#endif
			//AddLetter('B', 1.0f, 100 + 20, 256, 1);
			//AddLetter('L', 0.5f, 100 + 120, 256, 2);
			//AddLetter('A', 0.5f, 100 + 190, 256, 3);
			//AddLetter('C', 0.5f, 100 + 260, 256, 4);
			//AddLetter('K', 0.5f, 100 + 330, 256, 5);
			//
			//AddLetter('F', 1.0f, 100 + 500, 256, 6);
			//AddLetter('O', 0.5f, 100 + 570, 256, 7);
			//AddLetter('R', 0.5f, 100 + 640, 256, 8);
			//AddLetter('E', 0.5f, 100 + 710, 256, 9);
			//AddLetter('S', 0.5f, 100 + 780, 256, 10);
			//AddLetter('T', 0.5f, 100 + 850, 256, 11);
			//
			//AddLetter('B', 1.0f, 100 + 260 + 20, 512, 12);
			//AddLetter('Y', 0.5f, 100 + 260 + 120, 512, 13);
			//AddLetter('T', 0.5f, 100 + 260 + 190, 512, 14);
			//AddLetter('E', 0.5f, 100 + 260 + 260, 512, 15);
			//AddLetter('S', 0.5f, 100 + 260 + 330, 512, 16);

			Graph.Init();

			AddAgent(new WorldMapDragAgent(this, GetEntities<LevelNode>().Select(n => n.Position).ToList()));
			MapOffsetY = VIEW_HEIGHT / -2f;

			((WorldMapBackground)Background).InitBackground(GetEntities<LevelNode>().ToList());
		}

		private void AddLetter(char chr, float size, float x, float y, int index)
		{
			//*
			var em = new AnimatedPathGPUParticleEmitter(
				this, 
				new Vector2(x, y - (size * 150) / 2), 
				PathPresets.LETTERS[chr].AsScaled(size * 150), 
				ParticlePresets.GetConfigLetterFireRed(size, chr), 
				0.5f + index * 0.3f, 
				0.3f);
			/*/

			var em = new AnimatedPathCPUParticleEmitter(
				this,
				new Vector2(x, y - (size * 150) / 2),
				PathPresets.LETTERS[chr].AsScaled(size * 150),
				ParticlePresets.GetConfigLetterFireRed(size, chr),
				0.5f + index * 0.3f,
				0.3f);
			//*/

			Entities.AddEntity(em);
		}

		protected override void OnUpdate(SAMTime gameTime, InputState istate)
		{
#if DEBUG
			DebugDisp.IsEnabled = DebugSettings.Get("DebugTextDisplay");
			DebugDisp.Scale = 0.75f;

			if (SAMLog.Entries.Any()) DebugSettings.SetManual("DebugTextDisplay", true);
#endif

			if ((ZoomState == BistateProgress.Expanding || ZoomState == BistateProgress.Expanded) && istate.IsRealJustDown && istate.SwallowConsumer != InputConsumer.HUDElement)
			{
				ZoomIn(istate.GamePointerPositionOnMap);
			}

			if ((ZoomState == BistateProgress.Reverting || ZoomState == BistateProgress.Normal) && istate.IsGesturePinchComplete && istate.LastPinchPower < -10)
			{
				ZoomOut();
			}
		}

		protected override void OnDrawGame(IBatchRenderer sbatch)
		{
#if DEBUG
			if (DebugSettings.Get("DebugBackground"))
			{
				sbatch.DrawRectangle(Graph.BoundingRect, Color.OrangeRed, 3f);
				sbatch.DrawRectangle(Graph.BoundingViewport, Color.OrangeRed, 3f);
			}
#endif
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

			AddAgent(new ZoomOutAgent(this));

			((GDWorldHUD) HUD).SelectedNode?.CloseNode();
		}

		private void ZoomIn(FPoint mapPosCenter)
		{
			if (ZoomState == BistateProgress.Reverting || ZoomState == BistateProgress.Normal) return;
			if (GetAgents<ZoomOutAgent>().Any()) return;
			if (GetAgents<ZoomInAgent>().Any()) return;

			AddAgent(new ZoomInAgent(this, mapPosCenter));
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
	}
}
