using GridDominance.Shared.Resources;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.HUD;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

namespace GridDominance.Shared.Screens.NormalGameScreen.HUD.Elements
{
	public class HUDSpeedBaseButton : HUDElement
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
			RelativePosition = new FPoint(8, 8);
			Size = new FSize(62, 62);
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

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			sbatch.DrawScaled(Textures.TexHUDButtonBase,       Center, 1f, ColorMath.Blend(FlatColors.Flamingo, FlatColors.Asbestos, openingProgress), 0f);
			sbatch.DrawScaled(Textures.TexHUDButtonSpeedClock, Center, 1f, FlatColors.Clouds,                                                          0f);
			sbatch.DrawScaled(Textures.TexHUDButtonSpeedHand,  Center, 1f, FlatColors.Clouds,                                                          rotation);
		}

		protected override void DoUpdate(SAMTime gameTime, InputState istate)
		{
			rotation = FloatMath.IncModulo(rotation, gameTime.ElapsedSeconds * HAND_ANIMATION_SPEED * this.GDHUD().GDOwner.GameSpeed, FloatMath.TAU);

			int choosen = -1;

			if (isOpened && isDragging)
			{
				var center = new Vector2(Position.X + Size.Width / 2f, Position.Y + Size.Height / 2f);
				
				var delta = istate.HUDPointerPosition - center;
				if (delta.LengthSquared() >= DRAG_MINIMUMDISTANCE * DRAG_MINIMUMDISTANCE)
				{
					var angle = delta.ToAngle();

					if (angle >= FloatMath.RAD_POS_180 && angle <= FloatMath.RAD_POS_288)
						choosen = 0;
					else if (angle >= FloatMath.RAD_POS_288 && angle <= FloatMath.RAD_POS_306)
						choosen = 1;
					else if (angle >= FloatMath.RAD_POS_306 && angle <= FloatMath.RAD_POS_324)
						choosen = 2;
					else if (angle >= FloatMath.RAD_POS_324 && angle <= FloatMath.RAD_POS_342)
						choosen = 3;
					else if ((angle >= FloatMath.RAD_POS_342 && angle <= FloatMath.RAD_POS_360) || (angle > FloatMath.RAD_POS_000 && angle <= FloatMath.RAD_POS_090))
						choosen = 4;
					}

				if (istate.IsRealDown)
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

		private void UpdateOpening(SAMTime gameTime, bool force)
		{
			bool finished;
			openingProgress = FloatMath.LimitedInc(openingProgress, gameTime.ElapsedSeconds * OPENING_ANIMATION_TOTALSPEED, 1f, out finished);

			if (force)
			{
				openingProgress = 1f;
				finished = true;
			}

			for (int i = 0; i < 5; i++)
			{
				var progress = FloatMath.LimitedDec(openingProgress, i * OPENING_ANIMATION_DELAY, 0f) / OPENING_ANIMATION_SINGLESPEED;

				speedButtons[i].RelativePosition = RelativePosition + OPENING_VECTORS[i] * FloatMath.FunctionEaseOutElastic(progress);
			}

			if (finished || openingProgress > 0.5f) 
			{
				foreach (var btn in speedButtons)
				{
					btn.IsClickable = true;
				}
			}

			if (finished)
			{
				foreach (var btn in speedButtons)
				{
					btn.IsOpened = true;
					btn.IsClickable = true;
				}
			}
		}

		private void UpdateClosing(SAMTime gameTime)
		{
			bool finished;
			openingProgress = FloatMath.LimitedDec(openingProgress, gameTime.ElapsedSeconds * OPENING_ANIMATION_CLOSINGSPEED, 0f, out finished);

			for (int i = 0; i < 5; i++)
			{
				speedButtons[i].RelativePosition = RelativePosition + OPENING_VECTORS[i] * openingProgress;
				speedButtons[i].IsClickable = false;
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

		protected override bool OnPointerDown(FPoint relPositionPoint, InputState istate)
		{
			HUD.Screen.PushNotification("HUDSpeedBaseButton :: Click");
			
			if (!isOpened)
			{
				HUD.Screen.PushNotification("HUDSpeedBaseButton :: opening menu");

				Open();
			}
			else
			{
				HUD.Screen.PushNotification("HUDSpeedBaseButton :: closing menu");

				Close();
			}

			return true;
		}

		protected override bool OnPointerUp(FPoint relPositionPoint, InputState istate) => true;

		private void Open()
		{
			if (isOpened) return;
			if (!IsEnabled) return;

			isOpened = true;
			isDragging = true;

			if (speedButtons != null) 
			{
				foreach (var btn in speedButtons)
				{
					btn.Remove();
					btn.IsOpened = false;
					btn.IsClickable = false;
				}
				speedButtons = null;
			}

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
			if (!isOpened) return;

			isOpened = false;

			foreach (var btn in speedButtons)
			{
				btn.IsOpened = false;
				btn.IsClickable = false;
			}
		}

		protected override void OnEnabledChanged(bool newValue)
		{
			base.OnEnabledChanged(newValue);

			if (!newValue)
			{
				Close();
			}
		}
	}
}
