using System;
using GridDominance.Shared.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;
using MonoSAMFramework.Portable.Input;

namespace GridDominance.Shared.Screens.ScreenGame.HUD
{
	class HUDSpeedSetButton : GDGameHUDElement
	{
		private readonly GameSpeed speed;
		private readonly HUDSpeedBaseButton BaseButton;

		public bool IsOpened = false;
		public bool IsTranspositioning = false;

		public override int Depth => 0;

		public HUDSpeedSetButton(HUDSpeedBaseButton baseButton, GameSpeed buttonSpeed)
		{
			speed = buttonSpeed;
			BaseButton = baseButton;

			RelativePosition = baseButton.RelativePosition;
			Size = baseButton.Size;
			Alignment = baseButton.Alignment;
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
			if (IsOpened || IsTranspositioning)
			{
				var texture = GetTexture();

				var origin = new Vector2(texture.Width / 2f, texture.Height / 2f);
				var center = new Vector2(Position.X + bounds.Width / 2f, Position.Y + bounds.Height / 2f);

				sbatch.Draw(
					texture.Texture,
					center,
					texture.Bounds,
					Color.LightBlue,
					0f,
					origin,
					Textures.DEFAULT_TEXTURE_SCALE,
					SpriteEffects.None,
					0);
			}
		}

		private TextureRegion2D GetTexture()
		{
			switch (speed)
			{
				case GameSpeed.SUPERSLOW:
					return Textures.TexHUDButtonSpeedSet0;
				case GameSpeed.SLOW:
					return Textures.TexHUDButtonSpeedSet1;
				case GameSpeed.NORMAL:
					return Textures.TexHUDButtonSpeedSet2;
				case GameSpeed.FAST:
					return Textures.TexHUDButtonSpeedSet3;
				case GameSpeed.SUPERFAST:
					return Textures.TexHUDButtonSpeedSet4;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		protected override void DoUpdate(GameTime gameTime, InputState istate)
		{
			//
		}

		protected override void OnPointerClick(Point relPositionPoint, InputState istate)
		{
			if (!IsOpened || IsTranspositioning) return;

			Owner.Owner.PushNotification("HUDSpeedSetButton :: Set Speed := " + speed);

			BaseButton.Close();
		}
	}
}
