using System.Collections.Generic;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.OverworldScreen.Background;
using GridDominance.Shared.Screens.OverworldScreen.Entities;
using GridDominance.Shared.Screens.OverworldScreen.HUD;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.DebugTools;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Background;
using MonoSAMFramework.Portable.Screens.Entities;
using MonoSAMFramework.Portable.Screens.Entities.Particles;
using MonoSAMFramework.Portable.Screens.HUD;
using MonoSAMFramework.Portable.Screens.ViewportAdapters;
using GridDominance.Shared.SaveData;
using GridDominance.Shared.Screens.Common;
using GridDominance.Shared.Screens.Common.HUD.Dialogs;
using GridDominance.Shared.Screens.Common.Operations;
using GridDominance.Shared.Screens.OverworldScreen.Operations;
using GridDominance.Shared.Screens.WorldMapScreen.HUD;

namespace GridDominance.Shared.Screens.OverworldScreen
{
	public class GDOverworldScreen : GameScreen
	{
		public const int VIEW_WIDTH = 16 * GDConstants.TILE_WIDTH;
		public const int VIEW_HEIGHT = 10 * GDConstants.TILE_WIDTH;

		public OverworldHUD GDHUD => (OverworldHUD)HUD;

		protected override EntityManager CreateEntityManager() => new OverworldEntityManager(this);
		protected override GameHUD CreateHUD() => new OverworldHUD(this, true);
		protected override GameBackground CreateBackground() => new OverworldBackground(this);
		protected override SAMViewportAdapter CreateViewport() => new TolerantBoxingViewportAdapter(Game.Window, Graphics, VIEW_WIDTH, VIEW_HEIGHT);
		protected override DebugMinimap CreateDebugMinimap() => new StandardDebugMinimapImplementation(this, 192, 32);
		protected override FRectangle CreateMapFullBounds() => new FRectangle(0, 0, GDConstants.VIEW_WIDTH, GDConstants.VIEW_HEIGHT);
		protected override float GetBaseTextureScale() => Textures.DEFAULT_TEXTURE_SCALE_F;

		private bool _effectsEnabledCache = true;
		private readonly ParticleBanner _banner;
		public bool IsTransitioning = false;

		public OverworldScrollAgent ScrollAgent;

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

			if (!MonoSAMGame.IsIOS()) AddAgent(new ExitAgent());

			List<OverworldNode> nodesList = new List<OverworldNode>();

			nodesList.Add(new OverworldNode_Tutorial(this, FPoint.Zero));
			nodesList.Add(new OverworldNode_W1(this, FPoint.Zero));
			nodesList.Add(new OverworldNode_W2(this, FPoint.Zero));
			nodesList.Add(new OverworldNode_W3(this, FPoint.Zero));
			nodesList.Add(new OverworldNode_W4(this, FPoint.Zero));
			if (MainGame.Flavor != GDFlavor.FREE && MainGame.Flavor != GDFlavor.FULL_NOMP) nodesList.Add(new OverworldNode_MP(this, FPoint.Zero));
			if (MainGame.Flavor != GDFlavor.FREE) nodesList.Add(new OverworldNode_SCCM(this, FPoint.Zero));

			foreach (var node in nodesList) Entities.AddEntity(node);

			AddAgent(ScrollAgent = new OverworldScrollAgent(nodesList.ToArray()));

			_banner.TargetRect = new FRectangle(0 * GDConstants.TILE_WIDTH, 0.5f * GDConstants.TILE_WIDTH, 16 * GDConstants.TILE_WIDTH, 4 * GDConstants.TILE_WIDTH).AsDeflated(0.25f * GDConstants.TILE_WIDTH);
			_banner.Text = GDConstants.LOGO_STRING;
			_banner.UseCPUParticles = false;
			_banner.AnimationTime = 4f;
			_banner.AnimationStartDelay= 1f;

			if (!MainGame.IsShaderless()) _banner.CreateEntities(ParticlePresets.GetConfigLetterGreenGas());
		}
		
		protected override void OnUpdate(SAMTime gameTime, InputState istate)
		{
#if DEBUG
			DebugDisp.IsEnabled = DebugSettings.Get("DebugTextDisplay");
			DebugDisp.Scale = 0.75f;

			//if (MonoSAMGame.IsDesktop())
			//{
			//	if (istate.IsKeyExclusiveJustDown(SKeys.R))
			//	{
			//		var xcfg = XConfigFile.LoadFromString(System.IO.File.ReadAllText(@"F:\Symlinks\GridDominance\Data\presets\green_gas.xconf"));
			//		var pcfg = ParticleEmitterConfig.ParticleEmitterConfigBuilder.LoadFromXConfig(xcfg);
			//		_banner.CreateEntities(pcfg);
			//	}
			//}

#endif

			if (!MainGame.IsShaderless())
			{
				if (_effectsEnabledCache != MainGame.Inst.Profile.EffectsEnabled)
				{
					_effectsEnabledCache = MainGame.Inst.Profile.EffectsEnabled;

					if (MainGame.Inst.Profile.EffectsEnabled)
						_banner.CreateEntities(ParticlePresets.GetConfigLetterGreenGas());
					else
						_banner.RemoveEntities();
				}
			}
		}

		protected override void OnDrawGame(IBatchRenderer sbatch)
		{
			if (MainGame.IsShaderless())
			{
				var hh = 4.5f * GDConstants.TILE_WIDTH;
				sbatch.DrawCentered(Textures.TexLogo, _banner.TargetRect.Center, hh * Textures.TexLogo.Width / Textures.TexLogo.Height, hh, Color.White);
			}
			else
			{
				if (!MainGame.Inst.Profile.EffectsEnabled)
				{
					var hh = 4.5f * GDConstants.TILE_WIDTH;
					sbatch.DrawCentered(Textures.TexLogo, _banner.TargetRect.Center, hh * Textures.TexLogo.Width / Textures.TexLogo.Height, hh, Color.White);
				}
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

		public void TryShowAuthErrorPanel()
		{
			if (!MainGame.Inst.Profile.UnacknowledgedAuthError) return;
			if (HUD.GetCurrentModalDialog() != null) return;

			HUD.AddModal(new AuthErrorPanel(), false, 0.8f, 0f);
		}

		protected override void OnShow()
		{
			MainGame.Inst.GDSound.PlayMusicBackground();

			if (MainGame.Inst.Profile.UnacknowledgedAuthError)
			{
				HUD.AddModal(new AuthErrorPanel(), false, 0.8f, 0f);
			}
			else if (MainGame.Inst.Profile.AccountType == AccountType.Anonymous && MainGame.Inst.Profile.TotalPoints > 128 && !MainGame.Inst.Profile.AccountReminderShown)
			{
				HUD.AddModal(new AccountReminderPanel(), true, 0.8f, 1f);
			}
		}
	}
}
