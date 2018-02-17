using System;
using System.Linq;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.Leveleditor.Entities;
using GridDominance.Shared.Screens.Leveleditor.HUD;
using GridDominance.Shared.Screens.NormalGameScreen.Agents;
using GridDominance.Shared.Screens.NormalGameScreen.Entities;
using GridDominance.Shared.SCCM;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.DebugTools;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.LogProtocol;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Background;
using MonoSAMFramework.Portable.Screens.Entities;
using MonoSAMFramework.Portable.Screens.HUD;
using MonoSAMFramework.Portable.Screens.ViewportAdapters;

namespace GridDominance.Shared.Screens.Leveleditor
{
	class LevelEditorScreen : GameScreen
	{
		public const int VIEW_WIDTH  = (16+4+1) * GDConstants.TILE_WIDTH;
		public const int VIEW_HEIGHT = (10+4+1) * GDConstants.TILE_WIDTH;

		public readonly SCCMLevelData LevelData;
		public LevelEditorMode Mode = LevelEditorMode.Mouse;

		public ILeveleditorStub Selection = null;

		public LevelEditorScreen(MonoSAMGame game, GraphicsDeviceManager gdm, SCCMLevelData dat) : base(game, gdm)
		{
			LevelData = dat;

			Initialize();
		}

		public LevelEditorHUD GDHUD => (LevelEditorHUD)HUD;

		protected override EntityManager CreateEntityManager() => new LevelEditorEntityManager(this);
		protected override GameHUD CreateHUD() => new LevelEditorHUD(this);
		protected override GameBackground CreateBackground() => new LevelEditorBackground(this);
		protected override SAMViewportAdapter CreateViewport() => new TolerantBoxingViewportAdapter(Game.Window, Graphics, VIEW_WIDTH, VIEW_HEIGHT);
		protected override DebugMinimap CreateDebugMinimap() => new StandardDebugMinimapImplementation(this, 192, 32);
		protected override FRectangle CreateMapFullBounds() => new FRectangle(0, 0, 1, 1);
		protected override float GetBaseTextureScale() => Textures.DEFAULT_TEXTURE_SCALE_F;

		public LeveleditorDragAgent DragAgent;
		public LeveleditorInsertAgent InsertAgent;

		private void Initialize()
		{
#if DEBUG
			DebugUtils.CreateShortcuts(this);
			DebugDisp = DebugUtils.CreateDisplay(this);
#endif

			var workingArea = VAdapterGame.VirtualTotalBoundingBox.AsDeflated(0, 4 * GDConstants.TILE_WIDTH, 4 * GDConstants.TILE_WIDTH, 0);

			MapOffsetX = workingArea.Left;
			MapOffsetY = workingArea.Top;

			AddAgent(DragAgent = new LeveleditorDragAgent());
			AddAgent(InsertAgent = new LeveleditorInsertAgent());

			//
		}

		protected override void OnUpdate(SAMTime gameTime, InputState istate)
		{
#if DEBUG
			DebugDisp.IsEnabled = DebugSettings.Get("DebugTextDisplay");
			DebugDisp.Scale = 0.75f;
#endif

			//

#if DEBUG
			if (DebugSettings.Get("LeaveScreen"))
			{
				MainGame.Inst.SetOverworldScreen();
			}
#endif
		}

		protected override void OnDrawGame(IBatchRenderer sbatch)
		{
			//
		}

		protected override void OnDrawHUD(IBatchRenderer sbatch)
		{
			//
		}

		public CannonStub CanInsertCannonStub(FPoint center, ILeveleditorStub ign)
		{
			if (center.X <= 0) return null;
			if (center.Y <= 0) return null;
			if (center.X >= LevelData.Width * GDConstants.TILE_WIDTH) return null;
			if (center.Y >= LevelData.Height * GDConstants.TILE_WIDTH) return null;

			var s1 = CanInsertCannonStub(center, CannonStub.SCALES[3], ign);
			if (s1 != null) return s1;

			var s2 = CanInsertCannonStub(center, CannonStub.SCALES[2], ign);
			if (s2 != null) return s2;

			var s3 = CanInsertCannonStub(center, CannonStub.SCALES[1], ign);
			if (s3 != null) return s3;

			var s4 = CanInsertCannonStub(center, CannonStub.SCALES[0], ign);
			if (s4 != null) return s4;

			return null;
		}

		public CannonStub CanInsertCannonStub(FPoint center, float scale, ILeveleditorStub ign)
		{
			CannonStub s = new CannonStub(this, center, scale);

			foreach (var stub in GetEntities<CannonStub>().Where(stub => stub.Alive && stub != ign))
			{
				if (stub.CollidesWith(s)) return null;
			}

			return s;
		}

		public void SetMode(LevelEditorMode m)
		{
			Mode = m;
			switch (m)
			{
				case LevelEditorMode.Mouse:        GDHUD.ModePanel.SetActiveButton(GDHUD.ModePanel.BtnMouse);    break;
				case LevelEditorMode.AddCannon:    GDHUD.ModePanel.SetActiveButton(GDHUD.ModePanel.BtnCannon);   break;
				case LevelEditorMode.AddWall:      GDHUD.ModePanel.SetActiveButton(GDHUD.ModePanel.BtnWall);     break;
				case LevelEditorMode.AddObstacle:  GDHUD.ModePanel.SetActiveButton(GDHUD.ModePanel.BtnObstacle); break;
				default: SAMLog.Error("LES::EnumSwitch_TICS", "Mode = " + m); break;
			}
		}

		public void SelectStub(ILeveleditorStub stub)
		{
			Selection = stub;
		}
	}
}
