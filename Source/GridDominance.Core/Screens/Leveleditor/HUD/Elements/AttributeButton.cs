using System;
using System.Collections.Generic;
using System.Text;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.Leveleditor.Entities;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.BatchRenderer.TextureAtlases;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Localization;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Button;

namespace GridDominance.Shared.Screens.Leveleditor.HUD.Elements
{
	public class AttributeButton : HUDButton
	{
		public override int Depth => 0;

		public SingleAttrOption Data;

		public override void OnInitialize()
		{
			ClickMode = HUDButtonClickMode.Single;
		}

		public override void OnRemove()
		{
			//
		}

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			var icontex = Data.Icon();
			var iconscl = Data.IconScale;
			var iconcol = Data.IconColor();

			sbatch.DrawCentered(Textures.TexCircle, bounds.Center, bounds.Width, bounds.Height, IsPressed ? FlatColors.ButtonPressedHUD : FlatColors.ButtonHUD);

			sbatch.DrawCentered(Textures.TexCircleEmpty, bounds.Center, bounds.Width, bounds.Height, FlatColors.SeperatorHUD);

			if (icontex != null) sbatch.DrawCentered(icontex, bounds.Center, bounds.Width * iconscl, bounds.Height * iconscl, iconcol);

			FontRenderHelper.DrawTextCentered(
				sbatch, 
				Textures.HUDFontBold, 
				48, 
				L10N.T(Data.Description), 
				FlatColors.TextHUD,
				bounds.Center - new Vector2(0, bounds.Height/2 + 32));

			var txt = Data.Text();
			var tcc = Data.TextColor();
			if (!string.IsNullOrWhiteSpace(txt))
			{
				FontRenderHelper.DrawTextCentered(
					sbatch,
					Textures.HUDFontBold,
					64,
					txt,
					tcc,
					bounds.Center);
			}

		}

		protected override void DoUpdate(SAMTime gameTime, InputState istate)
		{
			//
		}

		protected override void OnPress(InputState istate)
		{
			Data.Action();
		}

		protected override void OnDoublePress(InputState istate) { }

		protected override void OnTriplePress(InputState istate) { }

		protected override void OnHold(InputState istate, float holdTime) { }
	}
}
