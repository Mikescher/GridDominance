using GridDominance.Shared.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.MathHelper;
using MonoSAMFramework.Portable.Screens.HUD;

namespace GridDominance.Shared.Screens.ScreenGame.HUD
{
	class HUDSpeedBaseButton : GDGameHUDElement
	{
		private const float HAND_ANIMATION_SPEED = 1f;

		private const float OPENING_ANIMATION_TOTALSPEED = 1f;
		private const float OPENING_ANIMATION_CLOSINGSPEED = 3f;
		private const float OPENING_ANIMATION_DELAY = 0.05f;
		private const float OPENING_ANIMATION_SINGLESPEED = 1 - OPENING_ANIMATION_DELAY*5;

		private const float OPENING_DISTANCE = 128;

		private const float DRAG_MINIMUMDISTANCE = 94;

		private static readonly Vector2[] OPENING_VECTORS =
		{
			new Vector2(0, OPENING_DISTANCE).Rotate(-FloatMath.ToRadians(00.0f)),
			new Vector2(0, OPENING_DISTANCE).Rotate(-FloatMath.ToRadians(22.5f)),
			new Vector2(0, OPENING_DISTANCE).Rotate(-FloatMath.ToRadians(45.0f)),
			new Vector2(0, OPENING_DISTANCE).Rotate(-FloatMath.ToRadians(67.5f)),
			new Vector2(0, OPENING_DISTANCE).Rotate(-FloatMath.ToRadians(90.0f)),
		};

		private bool isOpened = false;
		private bool isDragging = false;
		private float openingProgress = 0f;

		private float rotation = 0f;

		private HUDSpeedSetButton[] speedButtons;

		public override int Depth => 1;

		public HUDSpeedBaseButton()
		{
			RelativePosition = new Point(8, 8);
			Size = new Size(62, 62);
			Alignment = HUDAlignment.BOTTOMLEFT;
		}

		public override void OnInitialize()
		{
			speedButtons = null;
		}

		public override void OnRemove()
		{
			// NOP
		}

		protected override void DoDraw(SpriteBatch sbatch, Rectangle bounds)
		{
			sbatch.Draw(
				Textures.TexHUDButtonBase.Texture,
				Center,
				Textures.TexHUDButtonBase.Bounds,
				ColorMath.Blend(FlatColors.Flamingo, FlatColors.Asbestos, openingProgress),
				0f,
				Textures.TexHUDButtonBase.Center(),
				Textures.DEFAULT_TEXTURE_SCALE,
				SpriteEffects.None,
				0);
			
			sbatch.Draw(
				Textures.TexHUDButtonSpeedClock.Texture,
				Center,
				Textures.TexHUDButtonSpeedClock.Bounds,
				FlatColors.Clouds,
				0f,
				Textures.TexHUDButtonSpeedClock.Center(),
				Textures.DEFAULT_TEXTURE_SCALE,
				SpriteEffects.None,
				0);

			sbatch.Draw(
				Textures.TexHUDButtonSpeedHand.Texture,
				Center,
				Textures.TexHUDButtonSpeedHand.Bounds,
				FlatColors.Clouds,
				rotation,
				Textures.TexHUDButtonSpeedHand.Center(),
				Textures.DEFAULT_TEXTURE_SCALE,
				SpriteEffects.None,
				0);
		}

		protected override void DoUpdate(GameTime gameTime, InputState istate)
		{
			rotation = FloatMath.IncModulo(rotation, gameTime.GetElapsedSeconds() * HAND_ANIMATION_SPEED * GDOwner.GDOwner.GameSpeed, FloatMath.TAU);

			int choosen = -1;

			if (isOpened && isDragging)
			{
				var center = new Vector2(Position.X + Size.Width / 2f, Position.Y + Size.Height / 2f);
				
				var delta = istate.PointerPosition.ToVector2() - center;
				if (delta.LengthSquared() >= DRAG_MINIMUMDISTANCE * DRAG_MINIMUMDISTANCE)
				{
					var angle = delta.ToAngle();

					if (angle > -FloatMath.PI && angle < FloatMath.PI / 16f)
						choosen = 0;
					else if (angle > FloatMath.PI / 16f && angle < 3 * FloatMath.PI / 16f)
						choosen = 1;
					else if (angle > 3 * FloatMath.PI / 16f && angle < 5 * FloatMath.PI / 16f)
						choosen = 2;
					else if (angle > 5 * FloatMath.PI / 16f && angle < 7 * FloatMath.PI / 16f)
						choosen = 3;
					else if (angle > 7 * FloatMath.PI / 16f && angle < FloatMath.PI)
						choosen = 4;
				}

				if (istate.IsDown)
				{
					for (int i = 0; i < 5; i++)
					{
						speedButtons[i].Highlighted = (i == choosen);
					}
				}
				else
				{
					if (choosen != -1)
					{
						speedButtons[choosen].Click();
					}

					isDragging = false;
				}
			}

			if (isOpened && FloatMath.IsNotOne(openingProgress))
			{
				UpdateOpening(gameTime, choosen != -1);
			}
			else if (!isOpened && FloatMath.IsNotZero(openingProgress))
			{
				UpdateClosing(gameTime);
			}
		}

		private void UpdateOpening(GameTime gameTime, bool force)
		{
			bool finished;
			openingProgress = FloatMath.LimitedInc(openingProgress, gameTime.GetElapsedSeconds() * OPENING_ANIMATION_TOTALSPEED, 1f, out finished);

			if (force)
			{
				openingProgress = 1f;
				finished = true;
			}

			for (int i = 0; i < 5; i++)
			{
				var progress = FloatMath.LimitedDec(openingProgress, i * OPENING_ANIMATION_DELAY, 0f) / OPENING_ANIMATION_SINGLESPEED;

				speedButtons[i].RelativePosition = (RelativePosition.ToVector2() + OPENING_VECTORS[i] * FloatMath.FunctionEaseOutElastic(progress)).ToPoint();
			}

			if (finished)
			{
				foreach (var btn in speedButtons)
				{
					btn.IsOpened = true;
				}
			}
		}

		private void UpdateClosing(GameTime gameTime)
		{
			bool finished;
			openingProgress = FloatMath.LimitedDec(openingProgress, gameTime.GetElapsedSeconds() * OPENING_ANIMATION_CLOSINGSPEED, 0f, out finished);

			for (int i = 0; i < 5; i++)
			{
				speedButtons[i].RelativePosition = (RelativePosition.ToVector2() + OPENING_VECTORS[i] * openingProgress).ToPoint();
			}

			if (finished)
			{
				foreach (var btn in speedButtons)
				{
					btn.Remove();

					btn.IsOpened = false;
				}
				speedButtons = null;
			}
		}

		protected override void OnPointerDown(Point relPositionPoint, InputState istate)
		{
			Owner.Owner.PushNotification("HUDSpeedBaseButton :: Click");
			
			if (!isOpened)
			{
				Owner.Owner.PushNotification("HUDSpeedBaseButton :: opening menu");

				Open();
			}
			else
			{
				Owner.Owner.PushNotification("HUDSpeedBaseButton :: closing menu");

				Close();
			}
		}

		private void Open()
		{
			if (isOpened) return;

			isOpened = true;
			isDragging = true;

			speedButtons = new[]
			{
				new HUDSpeedSetButton(this, GameSpeedModes.SUPERSLOW),
				new HUDSpeedSetButton(this, GameSpeedModes.SLOW),
				new HUDSpeedSetButton(this, GameSpeedModes.NORMAL),
				new HUDSpeedSetButton(this, GameSpeedModes.FAST),
				new HUDSpeedSetButton(this, GameSpeedModes.SUPERFAST),
			};

			Owner.AddElements(speedButtons);
		}

		public void Close()
		{
			isOpened = false;

			foreach (var btn in speedButtons)
			{
				btn.IsOpened = false;
			}
		}
	}
}
