using System;
using System.Collections.Generic;
using System.Text;
using GridDominance.Shared.Resources;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.BatchRenderer.TextureAtlases;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Button;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

namespace GridDominance.Shared.Screens.WorldMapScreen.HUD
{
	abstract class HUDWorldSubSettingButton : HUDEllipseButton
	{
		private const float DIAMETER = 96;
		private const float SIZE_ICON = 56;
		private const float MARGIN_X = 0;
		private const float MARGIN_Y = 6;

		public override int Depth => 1;

		private readonly HUDWorldSettingsButton master;
		private readonly int position;

		public float offsetProgress = 0;
		public float scaleProgress = 1;
		public float fontProgress = 0;

		public HUDWorldSubSettingButton(HUDWorldSettingsButton master, int position)
		{
			this.master = master;
			this.position = position;

			RelativePosition = new FPoint(8, 8);
			Size = new FSize(DIAMETER, DIAMETER);
			Alignment = HUDAlignment.BOTTOMLEFT;
		}

		protected abstract TextureRegion2D GetIcon();
		protected abstract string GetText();

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			sbatch.DrawCentered(Textures.TexHUDButtonBase, Center, DIAMETER * scaleProgress, DIAMETER * scaleProgress, ColorMath.Blend(FlatColors.Asbestos, FlatColors.Alizarin, offsetProgress*scaleProgress));

			sbatch.DrawCentered(GetIcon(), Center, SIZE_ICON * scaleProgress, SIZE_ICON * scaleProgress, IsPressed ? FlatColors.WetAsphalt : FlatColors.Clouds);

			FontRenderHelper.DrawTextVerticallyCentered(sbatch, Textures.HUDFontRegular, SIZE_ICON, GetText(), FlatColors.Clouds * fontProgress, new Vector2(CenterX + SIZE_ICON, CenterY));
		}

		public override void OnInitialize()
		{
			//
		}

		public override void OnRemove()
		{
			//
		}

		protected override void DoUpdate(GameTime gameTime, InputState istate)
		{
			var px = master.RelativeCenter.X - DIAMETER / 2;
			var py = master.RelativeCenter.Y + HUDWorldSettingsButton.DIAMETER / 2 - DIAMETER;

			py += MARGIN_Y;
			py += (position + 1) * (DIAMETER + MARGIN_X / 2) * offsetProgress;

			RelativePosition = new FPoint(px, py);
		}

		protected override void OnDoublePress(InputState istate)
		{
			// Not Available
		}

		protected override void OnTriplePress(InputState istate)
		{
			// Not Available
		}

		protected override void OnHold(InputState istate, float holdTime)
		{
			// Not Available
		}
	}
}
