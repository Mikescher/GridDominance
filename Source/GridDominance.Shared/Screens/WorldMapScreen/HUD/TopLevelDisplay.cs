using System;
using System.Collections.Generic;
using System.Text;
using GridDominance.Shared.Resources;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Container;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Primitives;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

namespace GridDominance.Shared.Screens.WorldMapScreen.HUD
{
	class TopLevelDisplay : HUDContainer
	{
		public override int Depth => 0;

		private readonly HUDRawText text;

		public TopLevelDisplay()
		{
			text = new HUDRawText
			{
				Alignment = HUDAlignment.CENTER,
				Text = "Level 1 - 1: Introduction",
				TextColor = FlatColors.TextHUD,
				FontSize = 40f,
			};

			Alignment = HUDAlignment.TOPCENTER;
			RelativePosition = FPoint.Zero;
			Size = new FSize(300, 60);
		}

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			SimpleRenderHelper.DrawSimpleRect(sbatch, bounds, Color.Black * 0.6f);
		}

		public override void OnInitialize()
		{
			AddElement(text);
		}

		public override void OnRemove()
		{
			//
		}

		protected override void DoUpdate(SAMTime gameTime, InputState istate)
		{
			if (!FloatMath.FloatEquals(Size.Width, HUD.Width)) Size = new FSize(HUD.Width, 60);
		}
	}
}
