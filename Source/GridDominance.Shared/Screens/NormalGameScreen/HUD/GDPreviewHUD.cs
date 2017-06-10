using MonoSAMFramework.Portable.Screens.HUD;
using GridDominance.Shared.Resources;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Container;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Primitives;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

namespace GridDominance.Shared.Screens.NormalGameScreen.HUD
{
	class GDPreviewHUD : GameHUD
	{
		private HUDImage header1;
		private HUDImage header2;

		public GDPreviewHUD(GDGameScreen_Preview scrn) : base(scrn, Textures.HUDFontRegular)
		{
			AddElement(new HUDImage
			{
				Alignment = HUDAlignment.BOTTOMLEFT,
				Image = Textures.TexDescription2,
				Size = Textures.TexDescription2.Size().ScaleToHeight(4 * GDConstants.TILE_WIDTH),
			});

			AddElement(CreateHeader());
		}

		protected override void OnUpdate(SAMTime gameTime, InputState istate)
		{
			header1.Color = Color.White * FloatMath.PercSin(gameTime.TotalElapsedSeconds * 2);
			header2.Color = Color.White * FloatMath.PercSin(gameTime.TotalElapsedSeconds * 2);
		}

		private HUDLayoutContainer CreateHeader()
		{
			header1 = new HUDImage
			{
				Alignment = HUDAlignment.TOPLEFT,
				Image = Textures.TexGenericTitle,
				Size = Textures.TexGenericTitle.Size().ScaleToHeight(3 * GDConstants.TILE_WIDTH),
			};
			header2 = new HUDImage
			{
				Alignment = HUDAlignment.TOPRIGHT,
				Image = Textures.TexTitleNumber2,
				Size = Textures.TexTitleNumber2.Size().ScaleToHeight(3 * GDConstants.TILE_WIDTH),
			};

			float w = header1.Size.Width + header2.Size.Width;

			HUDLayoutContainer c = new HUDLayoutContainer()
			{
				Alignment = HUDAlignment.TOPCENTER,
				Size = new FSize(w, 4 * GDConstants.TILE_WIDTH),
			};

			c.AddElement(header1);
			c.AddElement(header2);

			return c;
		}
	}
}
