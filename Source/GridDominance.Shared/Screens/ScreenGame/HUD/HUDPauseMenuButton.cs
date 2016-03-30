using GridDominance.Shared.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.TextureAtlases;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.MathHelper;
using MonoSAMFramework.Portable.Screens.HUD;

namespace GridDominance.Shared.Screens.ScreenGame.HUD
{
	class HUDPauseMenuButton : GDGameHUDElement
	{
		private const int WIDTH = 222;
		private const int HEIGHT = HUDPauseButton.DIAMETER;
		private const int GAP = 10;

		private static readonly Vector2 RELATIVE_SPAWNPOSITION = new Vector2(WIDTH/2 - HUDPauseButton.DIAMETER/2, 64);

		private readonly HUDPauseButton baseButton;
		private readonly int btnIndex;
		private readonly int btnCount;
		private readonly int btnDepth;

		public float openingProgress = 0f;
		public bool isOpening = true;
		public bool isClosing = false;

		public override int Depth => btnDepth;

		public HUDPauseMenuButton(HUDPauseButton owner, string buttonText, int buttonDepth, int buttonIndex, int totalButtonCount)
		{
			baseButton = owner;
			btnIndex = buttonIndex;
			btnCount = totalButtonCount;
			btnDepth = buttonDepth;

			RelativePosition = new Point(12, 12);
			Size = new Size(WIDTH, HEIGHT);
			Alignment = owner.Alignment;
		}

		public override void OnInitialize()
		{
			UpdatePosition();
		}

		public override void OnRemove()
		{
			// NOP
		}

		protected override void DoUpdate(GameTime gameTime, InputState istate)
		{
			if (isOpening && FloatMath.IsNotOne(openingProgress))
			{
				bool hasOpened;
				openingProgress = FloatMath.LimitedInc(openingProgress, gameTime.GetElapsedSeconds() * HUDPauseButton.ANIMATION_SPEED, 1f, out hasOpened);
				if (hasOpened)
				{
					isOpening = false;
				}

				UpdatePosition();
			}
			else if (isClosing)
			{
				bool hasClosed;
				openingProgress = FloatMath.LimitedDec(openingProgress, gameTime.GetElapsedSeconds() * HUDPauseButton.ANIMATION_SPEED, 1f, out hasClosed);
				if (hasClosed)
				{
					Remove();
				}

				UpdatePosition();
			}
		}

		private void UpdatePosition()
		{
			if (isOpening)
			{
				UpdateOpeningPosition();
			}
			else
			{
				UpdateClosingPosition();
			}
		}

		private void UpdateOpeningPosition()
		{
			if (openingProgress < 0.5f)
			{
				var stepProgress = openingProgress / 0.5f;

				RelativeCenter = baseButton.RelativeCenter + RELATIVE_SPAWNPOSITION * stepProgress;
				Size = new Size((int) (WIDTH * stepProgress), (int) (HEIGHT * stepProgress));
			}
			else if (openingProgress < 0.55f)
			{
				RelativeCenter = baseButton.RelativeCenter + RELATIVE_SPAWNPOSITION;
				Size = new Size(WIDTH, HEIGHT);
			}
			else if (openingProgress < 1f)
			{
				var stepProgress = (openingProgress - 0.55f) / 0.45f;

				var posX = baseButton.RelativeCenter.X + RELATIVE_SPAWNPOSITION.X;
				var posY = baseButton.RelativeCenter.Y + RELATIVE_SPAWNPOSITION.Y + FloatMath.Min(stepProgress * (btnCount - 1) * (HEIGHT + GAP), btnIndex * (HEIGHT + GAP));

				RelativeCenter = new Vector2(posX, posY);

				Size = new Size(WIDTH, HEIGHT);
			}
			else
			{
				RelativeCenter = baseButton.RelativeCenter + RELATIVE_SPAWNPOSITION + new Vector2(0, btnIndex * (HEIGHT + GAP));
				Size = new Size(WIDTH, HEIGHT);
			}
		}

		private void UpdateClosingPosition()
		{
			
		}

		protected override void DoDraw(SpriteBatch sbatch, Rectangle bounds)
		{
			var scale = (Size.Width * 1f / WIDTH);

			sbatch.Draw(
				Textures.TexHUDButtonPauseMenuBackground.Texture,
				Center,
				Textures.TexHUDButtonPauseMenuBackground.Bounds,
				Color.White,
				0f,
				Textures.TexHUDButtonPauseMenuBackground.Center(),
				scale * Textures.DEFAULT_TEXTURE_SCALE,
				SpriteEffects.None,
				0);

			if (btnIndex == 0)
			{
				sbatch.Draw(
					Textures.TexHUDButtonPauseMenuMarkerBackground.Texture,
					Center + new Vector2(87, -32) * scale,
					Textures.TexHUDButtonPauseMenuMarkerBackground.Bounds,
					Color.Silver,
					0f,
					Textures.TexHUDButtonPauseMenuMarkerBackground.Center(),
					scale * Textures.DEFAULT_TEXTURE_SCALE,
					SpriteEffects.None,
					0);

				sbatch.Draw(
					Textures.TexHUDButtonPauseMenuMarker.Texture,
					Center + new Vector2(87, -32) * scale,
					Textures.TexHUDButtonPauseMenuMarker.Bounds,
					Color.Silver,
					0f,
					Textures.TexHUDButtonPauseMenuMarker.Center(),
					scale * Textures.DEFAULT_TEXTURE_SCALE,
					SpriteEffects.None,
					0);
			}

			sbatch.Draw(Textures.TexPixel, bounds, FlatColors.Silver);
		}

		protected override void OnPointerClick(Point relPositionPoint, InputState istate)
		{
			if (isOpening) return;

			baseButton.Close();
		}
	}
}
