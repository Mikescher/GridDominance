using System;
using System.Collections.Generic;
using System.Linq;
using GridDominance.Graphfileformat.Blueprint;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.Common.Agents;
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
using MonoSAMFramework.Portable.Localization;
using MonoSAMFramework.Portable.LogProtocol;
using MonoSAMFramework.Portable.Persistance;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Background;
using MonoSAMFramework.Portable.Screens.Entities;
using MonoSAMFramework.Portable.Screens.Entities.Particles;
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
		private readonly ParticleBanner _banner;
		public bool IsTransitioning = false;

		public GDOverworldScreen(MonoSAMGame game, GraphicsDeviceManager gdm) : base(game, gdm)
		{
			_banner = new ParticleBanner(this, Textures.TexParticle, GDConstants.ORDER_WORLD_LOGO);

			Initialize();
		}

		private void Initialize()
		{
#if DEBUG
			DebugUtils.CreateShortcuts(this);
			DebugDisp = DebugUtils.CreateDisplay(this);
#endif
			AddAgent(new ExitAgent(this));

			Entities.AddEntity(new OverworldNode_Tutorial(this, new Vector2( 3f * GDConstants.TILE_WIDTH, 6.5f * GDConstants.TILE_WIDTH)));
			Entities.AddEntity(new OverworldNode_W1(      this, new Vector2( 8f * GDConstants.TILE_WIDTH, 6.5f * GDConstants.TILE_WIDTH)));
			Entities.AddEntity(new OverworldNode_W2(      this, new Vector2(13f * GDConstants.TILE_WIDTH, 6.5f * GDConstants.TILE_WIDTH)));

			_banner.TargetRect = new FRectangle(0 * GDConstants.TILE_WIDTH, 0.5f * GDConstants.TILE_WIDTH, 16 * GDConstants.TILE_WIDTH, 4 * GDConstants.TILE_WIDTH).AsDeflated(0.25f * GDConstants.TILE_WIDTH);
			_banner.Text = "CANNON\nCONQUEST";
			_banner.UseCPUParticles = false;
			_banner.AnimationTime = 4f;
			_banner.CreateEntities(ParticlePresets.GetConfigLetterGreenGas()); //TODO delay by 1.5s , or only start when initial android lag finished
		}
		
		protected override void OnUpdate(SAMTime gameTime, InputState istate)
		{
#if DEBUG
			DebugDisp.IsEnabled = DebugSettings.Get("DebugTextDisplay");
			DebugDisp.Scale = 0.75f;
#endif

#if (DEBUG && __DESKTOP__)
			if (istate.IsKeyJustDown(SKeys.R))
			{
				var xcfg = XConfigFile.LoadFromString(System.IO.File.ReadAllText(@"F:\Symlinks\GridDominance\Data\presets\green_gas.xconf"));
				var pcfg = ParticleEmitterConfig.ParticleEmitterConfigBuilder.LoadFromXConfig(xcfg);
				_banner.CreateEntities(pcfg);
			}
#endif

			if (_effectsEnabledCache != MainGame.Inst.Profile.EffectsEnabled)
			{
				_effectsEnabledCache = MainGame.Inst.Profile.EffectsEnabled;

				if (MainGame.Inst.Profile.EffectsEnabled)
					_banner.CreateEntities(ParticlePresets.GetConfigLetterGreenGas());
				else
					_banner.RemoveEntities();
			}
		}

		protected override void OnDrawGame(IBatchRenderer sbatch)
		{
			if (!MainGame.Inst.Profile.EffectsEnabled)
			{
				var hh = 4.5f * GDConstants.TILE_WIDTH;
				sbatch.DrawCentered(Textures.TexLogo, _banner.TargetRect.VecCenter, hh * Textures.TexLogo.Width / Textures.TexLogo.Height, hh, Color.White);
			}

#if DEBUG
			if (DebugSettings.Get("DebugEntityBoundaries"))
			{
				sbatch.DrawRectangle(_banner.TargetRect, Color.DodgerBlue, 3f);
			}
#endif
		}

		protected override void OnDrawHUD(IBatchRenderer sbatch)
		{

		}
	}
}
