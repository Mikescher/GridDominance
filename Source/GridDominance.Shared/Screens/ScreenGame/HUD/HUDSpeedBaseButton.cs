using GridDominance.Shared.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
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

		private static readonly Vector2[] OPENING_VECTORS =
		{
			new Vector2(OPENING_DISTANCE, 0).Rotate(FloatMath.ToRadians(00.0f)),
			new Vector2(OPENING_DISTANCE, 0).Rotate(FloatMath.ToRadians(22.5f)),
			new Vector2(OPENING_DISTANCE, 0).Rotate(FloatMath.ToRadians(45.0f)),
			new Vector2(OPENING_DISTANCE, 0).Rotate(FloatMath.ToRadians(67.5f)),
			new Vector2(OPENING_DISTANCE, 0).Rotate(FloatMath.ToRadians(90.0f)),
		};

		private bool isOpened = false;
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
			speedButtons = new[]
			{
				new HUDSpeedSetButton(this, GameSpeed.SUPERSLOW),
				new HUDSpeedSetButton(this, GameSpeed.SLOW),
				new HUDSpeedSetButton(this, GameSpeed.NORMAL),
				new HUDSpeedSetButton(this, GameSpeed.FAST),
				new HUDSpeedSetButton(this, GameSpeed.SUPERFAST),
			};

			Owner.AddElement(speedButtons[0]);
			Owner.AddElement(speedButtons[1]);
			Owner.AddElement(speedButtons[2]);
			Owner.AddElement(speedButtons[3]);
			Owner.AddElement(speedButtons[4]);
		}

		public override void OnRemove()
		{
			// NOP
		}

		protected override void DoDraw(SpriteBatch sbatch, Rectangle bounds)
		{
			var origin = new Vector2(Textures.TexHUDButtonSpeedHand.Width / 2f, Textures.TexHUDButtonSpeedHand.Height / 2f);
			var center = new Vector2(Position.X + bounds.Width / 2f, Position.Y + bounds.Height / 2f);

			if (isOpened)
			{
				sbatch.Draw(
					Textures.TexHUDButtonSpeedOpen.Texture,
					center,
					Textures.TexHUDButtonSpeedOpen.Bounds,
					Color.White,
					0f,
					origin,
					Textures.DEFAULT_TEXTURE_SCALE,
					SpriteEffects.None,
					0);
			}
			else
			{
				sbatch.Draw(
					Textures.TexHUDButtonSpeedBase.Texture,
					center,
					Textures.TexHUDButtonSpeedBase.Bounds,
					Color.White,
					0f,
					origin,
					Textures.DEFAULT_TEXTURE_SCALE,
					SpriteEffects.None,
					0);
			}

			sbatch.Draw(
				Textures.TexHUDButtonSpeedHand.Texture,
				center,
				Textures.TexHUDButtonSpeedHand.Bounds,
				Color.White,
				rotation,
				origin,
				Textures.DEFAULT_TEXTURE_SCALE,
				SpriteEffects.None,
				0);
		}

		protected override void DoUpdate(GameTime gameTime, InputState istate)
		{
			rotation = FloatMath.IncModulo(rotation, gameTime.GetElapsedSeconds() * HAND_ANIMATION_SPEED * GDOwner.GDOwner.RealGameSpeed, FloatMath.TAU);

			if (isOpened && FloatMath.IsNotOne(openingProgress))
			{
				UpdateClosing(gameTime);
			}
			else if (!isOpened && FloatMath.IsNotZero(openingProgress))
			{
				UpdateOpening(gameTime);
			}
		}

		private void UpdateClosing(GameTime gameTime)
		{
			bool finished;
			openingProgress = FloatMath.LimitedInc(openingProgress, gameTime.GetElapsedSeconds() * OPENING_ANIMATION_TOTALSPEED, 1f, out finished);

			for (int i = 0; i < 5; i++)
			{
				var progress = FloatMath.LimitedDec(openingProgress, i * OPENING_ANIMATION_DELAY, 0f) / OPENING_ANIMATION_SINGLESPEED;

				speedButtons[i].RelativePosition = (RelativePosition.ToVector2() + OPENING_VECTORS[i] * FloatMath.FunctionEaseOutElastic(progress)).ToPoint();
			}

			if (finished)
			{
				foreach (var btn in speedButtons)
				{
					btn.IsTranspositioning = false;
					btn.IsOpened = true;
				}
			}
		}

		private void UpdateOpening(GameTime gameTime)
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
					btn.IsTranspositioning = false;
					btn.IsOpened = false;
				}
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
			isOpened = true;

			foreach (var btn in speedButtons)
			{
				btn.IsTranspositioning = true;
				btn.IsOpened = false;
			}
		}

		public void Close()
		{
			isOpened = false;

			foreach (var btn in speedButtons)
			{
				btn.IsTranspositioning = true;
				btn.IsOpened = false;
			}
		}
	}
}
