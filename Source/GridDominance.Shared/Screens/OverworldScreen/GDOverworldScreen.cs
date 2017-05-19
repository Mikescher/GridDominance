using System.Collections.Generic;
using System.Linq;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.OverworldScreen.Background;
using GridDominance.Shared.Screens.OverworldScreen.Entities;
using GridDominance.Shared.Screens.OverworldScreen.HUD;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.DebugTools;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.GameMath.VectorPath;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.LogProtocol;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Background;
using MonoSAMFramework.Portable.Screens.Entities;
using MonoSAMFramework.Portable.Screens.Entities.Particles.CPUParticles;
using MonoSAMFramework.Portable.Screens.HUD;
using MonoSAMFramework.Portable.Screens.ViewportAdapters;

namespace GridDominance.Shared.Screens.OverworldScreen
{
	public class GDOverworldScreen : GameScreen
	{
		public const int VIEW_WIDTH = 16 * GDConstants.TILE_WIDTH;
		public const int VIEW_HEIGHT = 10 * GDConstants.TILE_WIDTH;

		protected OverworldHUD GDHUD => (OverworldHUD)HUD;

		protected override EntityManager CreateEntityManager() => new OverworldEntityManager(this);
		protected override GameHUD CreateHUD() => new OverworldHUD(this);
		protected override GameBackground CreateBackground() => new OverworldBackground(this);
		protected override SAMViewportAdapter CreateViewport() => new TolerantBoxingViewportAdapter(Game.Window, Graphics, VIEW_WIDTH, VIEW_HEIGHT);
		protected override DebugMinimap CreateDebugMinimap() => new StandardDebugMinimapImplementation(this, 192, 32);
		protected override FRectangle CreateMapFullBounds() => new FRectangle(0, 0, GDConstants.VIEW_WIDTH, GDConstants.VIEW_HEIGHT);
		protected override float GetBaseTextureScale() => Textures.DEFAULT_TEXTURE_SCALE_F;

		private bool _effectsEnabledCache = true;
		private readonly List<CPUParticleEmitter> logoEmitter = new List<CPUParticleEmitter>();
		public bool IsTransitioning = false;

		public GDOverworldScreen(MonoSAMGame game, GraphicsDeviceManager gdm) : base(game, gdm)
		{
			Initialize();
		}

		private void Initialize()
		{
#if DEBUG
			DebugUtils.CreateShortcuts(this);
			DebugDisp = DebugUtils.CreateDisplay(this);
#endif
			AddLetter('T', 1.2f, 100, 256, 1);
			AddLetter('I', 1.2f, 100, 256, 2);
			AddLetter('T', 1.2f, 100, 256, 3);
			AddLetter('L', 1.2f, 100, 256, 4);
			AddLetter('E', 1.2f, 100, 256, 5);

			Entities.AddEntity(new OverworldTutorialNode(this, new Vector2(3f  * GDConstants.TILE_WIDTH, 6.5f * GDConstants.TILE_WIDTH), "Tutorial"));
			Entities.AddEntity(new OverworldNode(this, new Vector2(8f  * GDConstants.TILE_WIDTH, 6.5f * GDConstants.TILE_WIDTH), "Basic", Levels.WORLD_001));
			Entities.AddEntity(new OverworldNode(this, new Vector2(13f * GDConstants.TILE_WIDTH, 6.5f * GDConstants.TILE_WIDTH), "Professional", Levels.WORLD_002));
		}

		protected override void OnUpdate(SAMTime gameTime, InputState istate)
		{
#if DEBUG
			DebugDisp.IsEnabled = DebugSettings.Get("DebugTextDisplay");
			DebugDisp.Scale = 0.75f;
#endif

			if (_effectsEnabledCache != MainGame.Inst.Profile.EffectsEnabled)
			{
				_effectsEnabledCache = MainGame.Inst.Profile.EffectsEnabled;

				foreach (var emitter in logoEmitter)
				{
					emitter.IsEnabled = _effectsEnabledCache;
				}
			}
		}

		private void AddLetter(char chr, float size, float x, float y, int index)
		{
			/*
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
				new Vector2(x + index * 140, y - (size * 150) / 2),
				PathPresets.LETTERS[chr].AsScaled(size * 150),
				ParticlePresets.GetConfigLetterFireRed(size, chr),
				0.5f + index * 0.3f,
				0.3f,
				GDConstants.ORDER_WORLD_LOGO);
			//*/

			logoEmitter.Add(em);

			Entities.AddEntity(em);
		}

		protected override void OnDrawGame(IBatchRenderer sbatch)
		{
			if (!MainGame.Inst.Profile.EffectsEnabled)
			{
				FontRenderHelper.DrawTextCentered(sbatch, Textures.HUDFontRegular, 5f * GDConstants.TILE_WIDTH, "TITLE", FlatColors.Orange, new Vector2(VIEW_WIDTH/2f, 2.5f * GDConstants.TILE_WIDTH));

			}
		}

		protected override void OnDrawHUD(IBatchRenderer sbatch)
		{

		}
	}
}
