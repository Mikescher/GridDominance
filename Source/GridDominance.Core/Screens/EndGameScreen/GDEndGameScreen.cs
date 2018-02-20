using System;
using System.Collections.Generic;
using System.Text;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.EndGameScreen.Background;
using GridDominance.Shared.Screens.WorldMapScreen;
using GridDominance.Shared.Screens.WorldMapScreen.Background;
using GridDominance.Shared.Screens.WorldMapScreen.HUD;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.DebugTools;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Localization;
using MonoSAMFramework.Portable.Persistance;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Background;
using MonoSAMFramework.Portable.Screens.Entities;
using MonoSAMFramework.Portable.Screens.Entities.Particles;
using MonoSAMFramework.Portable.Screens.Entities.Special;
using MonoSAMFramework.Portable.Screens.HUD;
using MonoSAMFramework.Portable.Screens.ViewportAdapters;

namespace GridDominance.Shared.Screens.EndGameScreen
{
	public class GDEndGameScreen : GameScreen
	{
		public const int VIEW_WIDTH  = 16 * GDConstants.TILE_WIDTH;
		public const int VIEW_HEIGHT = 10 * GDConstants.TILE_WIDTH;

		protected override EntityManager CreateEntityManager() => new GDWorldMapEntityManager(this);
		protected override GameHUD CreateHUD() => new GDGameEndHUD(this);
		protected override GameBackground CreateBackground() => new EndGameBackground(this);
		protected override SAMViewportAdapter CreateViewport() => new TolerantBoxingViewportAdapter(Game.Window, Graphics, VIEW_WIDTH, VIEW_HEIGHT);
		protected override DebugMinimap CreateDebugMinimap() => new StandardDebugMinimapImplementation(this, 192, 32);
		protected override FRectangle CreateMapFullBounds() => new FRectangle(0, 0, 1, 1);
		protected override float GetBaseTextureScale() => Textures.DEFAULT_TEXTURE_SCALE_F;

		private float _lifetime;

		private readonly ParticleBanner _banner1;
		private readonly ParticleBanner _banner2;

		public GDEndGameScreen(MonoSAMGame game, GraphicsDeviceManager gdm) : base(game, gdm)
		{
			_banner1 = new ParticleBanner(this, Textures.TexParticle, GDConstants.ORDER_WORLD_LOGO);
			_banner2 = new ParticleBanner(this, Textures.TexParticle, GDConstants.ORDER_WORLD_LOGO);

			Initialize();
		}

		private void Initialize()
		{
#if DEBUG
			DebugUtils.CreateShortcuts(this);
			DebugDisp = DebugUtils.CreateDisplay(this);
#endif

			_banner1.TargetRect = new FRectangle(0, 2.5f, 16, 2).AsDeflated(0.25f).InReferenceRaster(1f/GDConstants.TILE_WIDTH);
			_banner1.Text = L10N.T(L10NImpl.STR_ENDGAME_1);
			_banner1.UseCPUParticles = false;
			_banner1.AnimationTime = 4f;
			_banner1.AnimationStartDelay = 5f;
			_banner1.CreateEntities(ParticlePresets.GetConfigLetterFlickerFire());

			_banner2.TargetRect = new FRectangle(0, 5.5f, 16, 2).AsDeflated(0.25f).InReferenceRaster(1f / GDConstants.TILE_WIDTH);
			_banner2.Text = L10N.T(L10NImpl.STR_ENDGAME_2);
			_banner2.UseCPUParticles = false;
			_banner2.AnimationTime = 4f;
			_banner2.AnimationStartDelay = 9f;
			_banner2.CreateEntities(ParticlePresets.GetConfigLetterFlickerFire());
			
			Entities.AddEntity(new MouseAreaEntity(this, new FPoint(VIEW_WIDTH/2f, VIEW_HEIGHT/2f), new FSize(VIEW_WIDTH * 2, VIEW_HEIGHT * 2), 0)
			{
				Click = LeaveScreen
			});
		}

		private void LeaveScreen()
		{
			if (_lifetime > 5) MainGame.Inst.SetOverworldScreen();
		}

		protected override void OnUpdate(SAMTime gameTime, InputState istate)
		{
			_lifetime += gameTime.ElapsedSeconds;

#if DEBUG
			DebugDisp.IsEnabled = DebugSettings.Get("DebugTextDisplay");
			DebugDisp.Scale = 0.75f;

			//if (MonoSAMGame.IsDesktop())
			//{
			//	_banner1.AnimationStartDelay = 0;
			//	_banner2.AnimationStartDelay = 0;
			//
			//	var xcfg = XConfigFile.LoadFromString(System.IO.File.ReadAllText(@"F:\Symlinks\GridDominance\Data\presets\auto.xconf"));
			//	var pcfg = ParticleEmitterConfig.ParticleEmitterConfigBuilder.LoadFromXConfig(xcfg);
			//	_banner1.CreateEntities(pcfg);
			//	_banner2.CreateEntities(pcfg);
			//}

#endif

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

			if (trigger) LeaveScreen();
		}

		protected override void OnDrawGame(IBatchRenderer sbatch)
		{
			//
		}

		protected override void OnDrawHUD(IBatchRenderer sbatch)
		{
			//
		}

		protected override void OnShow()
		{
			MainGame.Inst.GDSound.PlayMusicBackground();
			MainGame.Inst.GDSound.PlayEffectGameWon();
		}
	}
}
