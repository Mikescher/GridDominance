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
	class HUDPauseButton : GDGameHUDElement
	{
		public const int DIAMETER = 48;

		public const float ANIMATION_SPEED = 2.5f;

		private bool isOpened = false;
		private float animationProgress = 0f;

		public override int Depth => 1;

		private HUDPauseMenuButton[] subMenu = null;

		public HUDPauseButton()
		{
			RelativePosition = new Point(12, 12);
			Size = new Size(DIAMETER, DIAMETER);
			Alignment = HUDAlignment.TOPRIGHT;
		}

		public override void OnInitialize()
		{
			// NOP
		}

		public override void OnRemove()
		{
			// NOP
		}

		protected override void DoDraw(SpriteBatch sbatch, Rectangle bounds)
		{
			var texScale = bounds.Width / (Textures.TexHUDButtonBase.Width * Textures.DEFAULT_TEXTURE_SCALE.X);

			TextureRegion2D texIcon;

			if (FloatMath.IsZero(animationProgress))
			{
				texIcon = Textures.TexHUDButtonPause[0];
			}
			else if (FloatMath.IsOne(animationProgress))
			{
				texIcon = Textures.TexHUDButtonPause[Textures.ANIMATION_HUDBUTTONPAUSE_SIZE - 1];
			}
			else
			{
				texIcon = Textures.TexHUDButtonPause[(int)(Textures.ANIMATION_HUDBUTTONPAUSE_SIZE * animationProgress)];
			}
			
			sbatch.Draw(
				Textures.TexHUDButtonBase.Texture,
				Center,
				Textures.TexHUDButtonBase.Bounds,
				FlatColors.Asbestos,
				0f,
				Textures.TexHUDButtonBase.Center(),
				texScale * Textures.DEFAULT_TEXTURE_SCALE,
				SpriteEffects.None,
				0);

			sbatch.Draw(
				texIcon.Texture,
				Center,
				texIcon.Bounds,
				FlatColors.Clouds,
				0f,
				texIcon.Center(),
				texScale * Textures.DEFAULT_TEXTURE_SCALE,
				SpriteEffects.None,
				0);
		}

		protected override void DoUpdate(GameTime gameTime, InputState istate)
		{
			if (isOpened && FloatMath.IsNotOne(animationProgress))
			{
				animationProgress = FloatMath.LimitedInc(animationProgress, gameTime.GetElapsedSeconds() * ANIMATION_SPEED, 1f);
			}
			else if (!isOpened && FloatMath.IsNotZero(animationProgress))
			{
				animationProgress = FloatMath.LimitedDec(animationProgress, gameTime.GetElapsedSeconds() * ANIMATION_SPEED, 0f);
			}
		}

		protected override void OnPointerClick(Point relPositionPoint, InputState istate)
		{
			if (! isOpened)
			{
				Open();
			}
			else
			{
				Close();
			}
		}

		public void Close()
		{
			isOpened = false;

			GDOwner.GDOwner.IsPaused = false;

			foreach (var button in subMenu)
			{
				button.isClosing = true;
			}
			subMenu = null;

			Owner.Owner.PushNotification("HUDPauseButton :: Game resumed");
		}

		private void Open()
		{
			isOpened = true;

			GDOwner.GDOwner.IsPaused = true;

			Owner.Owner.PushNotification("HUDPauseButton :: Game paused");

			subMenu = new[]
			{
				new HUDPauseMenuButton(this, "RESUME", -1, 0, 3),
				new HUDPauseMenuButton(this, "RESTART", -2, 1, 3),
				new HUDPauseMenuButton(this, "EXIT", -3, 2, 3),
			};

			Owner.AddElements(subMenu);
		}
	}
}
