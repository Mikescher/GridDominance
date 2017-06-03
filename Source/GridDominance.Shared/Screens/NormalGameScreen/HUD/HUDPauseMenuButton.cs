using System;
using GridDominance.Shared.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Button;

namespace GridDominance.Shared.Screens.NormalGameScreen.HUD
{
	class HUDPauseMenuButton : HUDButton
	{
		public const int MARKER_WIDTH = 33;
		public const int MARKER_HEIGHT = 17;

		private const int HEIGHT = HUDPauseButton.DIAMETER;
		private const int GAP = 10;

		private static readonly float fontScaleFactor = FontRenderHelper.GetFontScale(Textures.HUDFontBold, 40);

		private const float CLOSING_DELAY = 0.2f;

		public const float ROUNDNESS = 4f;

		private readonly float width;
		private readonly Vector2 relSpawnpos;
		private readonly HUDPauseButton baseButton;
		private readonly int btnIndex;
		private readonly int btnCount;
		private readonly string btnText;
		private readonly Action btnAction;

		private float openingProgress = 0f;
		public bool IsOpening = true;
		public bool IsClosing = false;

		public override int Depth { get; }

		public HUDPauseMenuButton(HUDPauseButton owner, string buttonText, float textWidth, int buttonDepth, int buttonIndex, int totalButtonCount, Action buttonAction)
		{
			baseButton = owner;
			btnIndex = buttonIndex;
			btnCount = totalButtonCount;
			btnText = buttonText;
			btnAction = buttonAction;
			width = textWidth + 24;
			relSpawnpos = new Vector2(width / 2 - HUDPauseButton.DIAMETER / 2, 64);

			Depth = buttonDepth;

			RelativePosition = new FPoint(12, 12);
			Size = new FSize(0, 0);
			Alignment = owner.Alignment;
		}

		public override void OnInitialize()
		{

		}

		public override void OnRemove()
		{
			// NOP
		}

		protected override void DoUpdate(SAMTime gameTime, InputState istate)
		{
			if (IsOpening && FloatMath.IsNotOne(openingProgress))
			{
				bool hasOpened;
				openingProgress = FloatMath.LimitedInc(openingProgress, gameTime.ElapsedSeconds / HUDPauseButton.ANIMATION_SPEED, 1f, out hasOpened);
				if (hasOpened)
				{
					IsOpening = false;
				}

				UpdateOpeningPosition();
			}
			else if (IsClosing)
			{
				bool hasClosed;
				openingProgress = FloatMath.LimitedDec(openingProgress, gameTime.ElapsedSeconds / HUDPauseButton.ANIMATION_SPEED, 0f, out hasClosed);
				if (hasClosed)
				{
					Remove();
				}

				UpdateClosingPosition();
			}
		}

		private void UpdateOpeningPosition()
		{
			if (openingProgress < 0.5f)
			{
				var stepProgress = openingProgress / 0.5f;

				RelativeCenter = baseButton.RelativeCenter + relSpawnpos * stepProgress;
				Size = new FSize(width * stepProgress, HEIGHT * stepProgress);
			}
			else if (openingProgress < 0.55f)
			{
				RelativeCenter = baseButton.RelativeCenter + relSpawnpos;
				Size = new FSize(width, HEIGHT);
			}
			else if (openingProgress < 1f)
			{
				var stepProgress = (openingProgress - 0.55f) / 0.45f;

				var posX = baseButton.RelativeCenter.X + relSpawnpos.X;
				var posY = baseButton.RelativeCenter.Y + relSpawnpos.Y + FloatMath.Min(stepProgress * (btnCount - 1) * (HEIGHT + GAP), btnIndex * (HEIGHT + GAP));

				RelativeCenter = new Vector2(posX, posY);

				Size = new FSize(width, HEIGHT);
			}
			else
			{
				RelativeCenter = baseButton.RelativeCenter + relSpawnpos + new Vector2(0, btnIndex * (HEIGHT + GAP));
				Size = new FSize(width, HEIGHT);
			}
		}

		private void UpdateClosingPosition()
		{
			var prog = 1 - openingProgress;
			prog -= (btnCount-btnIndex-1) * CLOSING_DELAY;
			if (prog <= 0) prog = 0;
			prog /= 1 - ((btnCount - 1) * CLOSING_DELAY);

			RelativeCenter = baseButton.RelativeCenter + relSpawnpos + new Vector2(-prog * width*2, btnIndex * (HEIGHT + GAP));
		}

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			var scale = Size.Width * 1f / width;

			FlatRenderHelper.DrawRoundedBlurPanelBackgroundPart(sbatch, bounds, ROUNDNESS);

			if (btnIndex == 0)
			{
				sbatch.DrawScaled(
					Textures.TexHUDButtonPauseMenuMarkerBackground,
					Center + new Vector2(width / 2f - HUDPauseButton.DIAMETER / 2f, -(HEIGHT / 2f + MARKER_HEIGHT / 2f)) * scale,
					scale,
					Color.White,
					0f);

				sbatch.DrawScaled(
					Textures.TexHUDButtonPauseMenuMarker,
					Center + new Vector2(width / 2f - HUDPauseButton.DIAMETER / 2f, -(HEIGHT / 2f + MARKER_HEIGHT / 2f)) * scale,
					scale,
					IsPressed ? FlatColors.Concrete : FlatColors.Silver,
					0f);
			}

			SimpleRenderHelper.DrawRoundedRect(sbatch, bounds, IsPressed ? FlatColors.Concrete : FlatColors.Silver, ROUNDNESS);

			var fontBounds = Textures.HUDFontBold.MeasureString(btnText);

			sbatch.DrawString(
				Textures.HUDFontBold, 
				btnText, 
				Center + new Vector2(-width/2f + 12 + fontScaleFactor * fontBounds.X/2f, 0f)*scale, 
				FlatColors.Foreground, 
				0f,
				fontBounds/2f,
				scale * fontScaleFactor, 
				SpriteEffects.None, 0f);
		}
		
		protected override void OnPress(InputState istate)
		{
			if (IsOpening) return;

			btnAction();
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
