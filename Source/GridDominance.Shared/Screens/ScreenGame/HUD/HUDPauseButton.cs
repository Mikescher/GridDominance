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
		private const float ANIMATION_SPEED = 5f;

		private bool isClicked = false;
		private float animationProgress = 0f;

		public override int Depth => 1;

		public HUDPauseButton()
		{
			RelativePosition = new Point(12, 12);
			Size = new Size(48, 48);
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
			var center = new Vector2(Position.X + bounds.Width / 2f, Position.Y + bounds.Height / 2f);

			TextureRegion2D tex;

			if (FloatMath.IsZero(animationProgress))
			{
				tex = Textures.TexHUDButtonPause[0];
			}
			else if (FloatMath.IsOne(animationProgress))
			{
				tex = Textures.TexHUDButtonPause[Textures.ANIMATION_HUDBUTTONPAUSE_SIZE - 1];
			}
			else
			{
				tex = Textures.TexHUDButtonPause[(int)(Textures.ANIMATION_HUDBUTTONPAUSE_SIZE * animationProgress)];
			}
			
			sbatch.Draw(
				Textures.TexHUDButtonBase.Texture,
				center,
				Textures.TexHUDButtonBase.Bounds,
				FlatColors.Asbestos,
				0f,
				Textures.TexHUDButtonBase.Center(),
				Textures.DEFAULT_TEXTURE_SCALE,
				SpriteEffects.None,
				0);

			sbatch.Draw(
				tex.Texture,
				center,
				tex.Bounds,
				FlatColors.Clouds,
				0f,
				tex.Center(),
				Textures.DEFAULT_TEXTURE_SCALE,
				SpriteEffects.None,
				0);
		}

		protected override void DoUpdate(GameTime gameTime, InputState istate)
		{
			if (isClicked && FloatMath.IsNotOne(animationProgress))
			{
				animationProgress = FloatMath.LimitedInc(animationProgress, gameTime.GetElapsedSeconds() * ANIMATION_SPEED, 1f);
			}
			else if (!isClicked && FloatMath.IsNotZero(animationProgress))
			{
				animationProgress = FloatMath.LimitedDec(animationProgress, gameTime.GetElapsedSeconds() * ANIMATION_SPEED, 0f);
			}
		}

		protected override void OnPointerClick(Point relPositionPoint, InputState istate)
		{
			isClicked = !isClicked;

			if (isClicked)
			{
				GDOwner.GDOwner.IsPaused = true;

				Owner.Owner.PushNotification("HUDPauseButton :: Game paused");
			}
			else
			{
				GDOwner.GDOwner.IsPaused = false;

				Owner.Owner.PushNotification("HUDPauseButton :: Game resumed");
			}
		}
	}
}
