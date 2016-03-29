using GridDominance.Shared.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.TextureAtlases;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.MathHelper;
using MonoSAMFramework.Portable.Screens.HUD;

namespace GridDominance.Shared.Screens.ScreenGame.HUD
{
	class HUDPauseButton : GameHUDElement
	{
		private const float ANIMATION_SPEED = 5f;

		private bool isClicked = false;
		private float animationProgress = 0f;

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
			TextureRegion2D tex;

			if (FloatMath.IsZero(animationProgress))
			{
				tex = Textures.HUDButtonPause[0];
			}
			else if (FloatMath.IsOne(animationProgress))
			{
				tex = Textures.HUDButtonPause[Textures.ANIMATION_HUDBUTTONPAUSE_SIZE - 1];
			}
			else
			{
				tex = Textures.HUDButtonPause[(int)(Textures.ANIMATION_HUDBUTTONPAUSE_SIZE * animationProgress)];
			}

			sbatch.Draw(tex, bounds, Color.White);
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
				Owner.Owner.PushNotification("HUDPauseButton :: Game paused");
			}
			else
			{
				Owner.Owner.PushNotification("HUDPauseButton :: Game resumed");
			}
		}
	}
}
