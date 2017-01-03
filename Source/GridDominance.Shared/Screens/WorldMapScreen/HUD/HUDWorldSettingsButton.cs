using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GridDominance.Shared.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Button;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

namespace GridDominance.Shared.Screens.WorldMapScreen.HUD
{
	class HUDWorldSettingsButton : HUDEllipseButton
	{
		public const float DIAMETER = 124;
		private const float SIZE_ICON = 72;

		public override int Depth => 1;

		private float rotation = 0f;

		public float rotationSpeed = 1f;
		public float openingProgress = 0f;
		public HUDWorldSubSettingButton[] subButtons;

		public HUDWorldSettingsButton()
		{
			RelativePosition = new FPoint(8, 8);
			Size = new FSize(DIAMETER, DIAMETER);
			Alignment = HUDAlignment.BOTTOMLEFT;
			ClickMode = HUDButtonClickMode.Single;
		}

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			sbatch.DrawSimple(Textures.TexHUDButtonBase, Center, DIAMETER, DIAMETER, ColorMath.Blend(FlatColors.Alizarin, FlatColors.Asbestos, openingProgress), 0f);

			sbatch.DrawSimple(Textures.TexHUDButtonIconSettings, Center, SIZE_ICON, SIZE_ICON, IsPressed ? FlatColors.WetAsphalt : FlatColors.Clouds, rotation);
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
			rotation += gameTime.GetElapsedSeconds() * rotationSpeed;

			if (istate.IsRealJustDown && FloatMath.IsOne(openingProgress) && !IsPressed && subButtons != null && !subButtons.Any(p => p.IsPressed))
			{
				// Close when clicked somewhere else
				AddHUDOperation(new HUDSettingsCloseOperation());
			}
		}

		protected override void OnPress(InputState istate)
		{
			if (FloatMath.IsZero(openingProgress))
			{
				AddHUDOperationSequence<HUDWorldSettingsButton>(
					new HUDSettingsOpenOperation(), 
					new HUDSettingsFontAppearOperation(0),
					new HUDSettingsFontAppearOperation(1),
					new HUDSettingsFontAppearOperation(2),
					new HUDSettingsFontAppearOperation(3),
					new HUDSettingsFontAppearOperation(4)
					);
			}
			else if (FloatMath.IsOne(openingProgress))
			{
				AddHUDOperation(new HUDSettingsCloseOperation());
			}
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
