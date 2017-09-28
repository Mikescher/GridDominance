using GridDominance.Shared.Resources;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.GameMath.VectorPath;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Button;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Container;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Primitives;
using MonoSAMFramework.Portable.Screens.HUD.Enums;
using MonoSAMFramework.Portable.Localization;
using MonoSAMFramework.Portable.Persistance;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Entities;
using MonoSAMFramework.Portable.Screens.Entities.Particles;
using MonoSAMFramework.Portable.Screens.Entities.Particles.GPUParticles;

namespace GridDominance.Shared.Screens.WorldMapScreen.HUD
{
	class AcknowledgementsPanel : HUDRoundedPanel
	{
		public const float WIDTH  = 10 * GDConstants.TILE_WIDTH;
		public const float HEIGHT = 9 * GDConstants.TILE_WIDTH;

		public const float LINE_WIDTH  = 4.5f * GDConstants.TILE_WIDTH;
		public const float LINE_OFFSET = (WIDTH - 2*LINE_WIDTH)/3;

		public override int Depth => 0;

		private PathGPUParticleEmitter _emitter;

		public AcknowledgementsPanel()
		{
			RelativePosition = FPoint.Zero;
			Size = new FSize(WIDTH, HEIGHT);
			Alignment = HUDAlignment.CENTER;
			Background = FlatColors.BackgroundHUD;
		}

		public override void OnInitialize()
		{
			base.OnInitialize();

			AddElement(new HUDLabel(1)
			{
				TextAlignment = HUDAlignment.CENTER,
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(0, 0),
				Size = new FSize(WIDTH, 96),

				Font = Textures.HUDFontBold,
				FontSize = 64,

				L10NText = L10NImpl.STR_ACKNOWLEDGEMENTS,
				TextColor = FlatColors.SunFlower,
			});

			for (int i = 0; i < Attributions.THANKS.Length; i++)
			{
				var dd = Attributions.THANKS[i];

				var btn = new HUDTextButton(1)
				{
					TextAlignment = HUDAlignment.CENTERLEFT,
					Alignment = HUDAlignment.TOPLEFT,
					RelativePosition = new FPoint(LINE_OFFSET, 96 + i * 56),
					Size = new FSize(LINE_WIDTH, 32),

					Font = Textures.HUDFontRegular,
					FontSize = 32,

					Text = dd.Item1,

					TextColor = FlatColors.Clouds,
					TextPadding = 16,

					BackgroundNormal  = HUDBackgroundDefinition.CreateRounded(FlatColors.MidnightBlue, 8),
					BackgroundPressed = HUDBackgroundDefinition.CreateRounded(FlatColors.Wisteria,     8),

					ClickMode = HUDButton.HUDButtonClickMode.Single,
				};

				var lbl = new HUDLabel(1)
				{
					TextAlignment = HUDAlignment.CENTERLEFT,
					Alignment = HUDAlignment.TOPRIGHT,
					RelativePosition = new FPoint(LINE_OFFSET, 96 + i * 56),
					Size = new FSize(LINE_WIDTH, 32),

					Font = Textures.HUDFontBold,
					FontSize = 32,

					Text = dd.Item3,

					TextColor = FlatColors.Clouds,
				};

				if (!string.IsNullOrWhiteSpace(dd.Item2))
				{
					btn.ButtonClick += (s, a) =>
					{
						HUD.ShowToast(null, dd.Item2, 32, FlatColors.Silver, FlatColors.Foreground, 3f);
						MainGame.Inst.Bridge.OpenURL(dd.Item2);
					};
				}

				AddElement(lbl);
				AddElement(btn);
			}


			if (MainGame.Inst.Profile.EffectsEnabled) AddParticles();
		}

		private void AddParticles()
		{
			var scrn = HUD.Screen;

			var path = VectorPathBuilder
				.Start()
				.MoveTo(scrn.TranslateHUDToGameCoordinates(-WIDTH/2f, -HEIGHT/2f))
				.LineTo(scrn.TranslateHUDToGameCoordinates(-WIDTH/2f, +HEIGHT/2f))
				.LineTo(scrn.TranslateHUDToGameCoordinates(+WIDTH/2f, +HEIGHT/2f))
				.LineTo(scrn.TranslateHUDToGameCoordinates(+WIDTH/2f, -HEIGHT/2f))
				.LineTo(scrn.TranslateHUDToGameCoordinates(-WIDTH/2f, -HEIGHT/2f))
				.Build();

			var cfg = ParticlePresets.GetConfigBubbleHighlight().Build(Textures.TexParticle, 2f, 3f);

			_emitter = new PathGPUParticleEmitter(scrn, scrn.MapViewportCenter, path, cfg, GDConstants.ORDER_WORLD_SUPEREFFECTS);
			_emitter.AlphaAppearTime = 2f;
			_emitter.FastForward();

			HUD.Screen.Entities.AddEntity(_emitter);
		}

#if DEBUG
		public override void Update(SAMTime gameTime, InputState istate)
		{
			base.Update(gameTime, istate);

#if (DEBUG && __DESKTOP__)
			if (istate.IsKeyExclusiveJustDown(SKeys.R))
			{
				var xcfg = XConfigFile.LoadFromString(System.IO.File.ReadAllText(@"F:\Symlinks\GridDominance\Data\presets\generic.xconf"));
				var pcfg = ParticleEmitterConfig.ParticleEmitterConfigBuilder.LoadFromXConfig(xcfg);
				_emitter.Alive = false;

			
				var scrn = HUD.Screen;

				var path = VectorPathBuilder
					.Start()
					.MoveTo(scrn.TranslateHUDToGameCoordinates(-WIDTH/2f, -HEIGHT/2f))
					.LineTo(scrn.TranslateHUDToGameCoordinates(-WIDTH/2f, +HEIGHT/2f))
					.LineTo(scrn.TranslateHUDToGameCoordinates(+WIDTH/2f, +HEIGHT/2f))
					.LineTo(scrn.TranslateHUDToGameCoordinates(+WIDTH/2f, -HEIGHT/2f))
					.LineTo(scrn.TranslateHUDToGameCoordinates(-WIDTH/2f, -HEIGHT/2f))
					.Build();

				var cfg = pcfg.Build(Textures.TexParticle, 2f, 3f);

				_emitter = new PathGPUParticleEmitter(scrn, scrn.MapViewportCenter, path, cfg, GDConstants.ORDER_WORLD_SUPEREFFECTS);

				HUD.Screen.Entities.AddEntity(_emitter);
			}
#endif
		}
#endif

		public override void OnRemove()
		{
			base.OnRemove();

			_emitter.Alive = false;
		}
	}
}