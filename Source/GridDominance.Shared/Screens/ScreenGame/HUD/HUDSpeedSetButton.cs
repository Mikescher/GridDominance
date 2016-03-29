using System;
using GridDominance.Shared.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.Input;

namespace GridDominance.Shared.Screens.ScreenGame.HUD
{
	class HUDSpeedSetButton : GDGameHUDElement
	{
		private readonly GameSpeedModes speed;
		private readonly HUDSpeedBaseButton BaseButton;

		public bool IsOpened = false;
		public bool IsTranspositioning = false;

		public override int Depth => 0;
		public bool Highlighted = false;

		public HUDSpeedSetButton(HUDSpeedBaseButton baseButton, GameSpeedModes buttonSpeed)
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
				
				var center = new Vector2(Position.X + bounds.Width / 2f, Position.Y + bounds.Height / 2f);

				sbatch.Draw(
					Textures.TexHUDButtonBase.Texture,
					center,
					Textures.TexHUDButtonBase.Bounds,
					Highlighted ? FlatColors.Emerald : FlatColors.Flamingo,
					0f,
					Textures.TexHUDButtonBase.Center(),
					Textures.DEFAULT_TEXTURE_SCALE,
					SpriteEffects.None,
					0);

				sbatch.Draw(
					texture.Texture,
					center,
					texture.Bounds,
					(GDOwner.GDOwner.GameSpeedMode == speed) ? FlatColors.MidnightBlue : FlatColors.Clouds,
					0f,
					texture.Center(),
					Textures.DEFAULT_TEXTURE_SCALE,
					SpriteEffects.None,
					0);
			}
		}

		private TextureRegion2D GetTexture()
		{
			switch (speed)
			{
				case GameSpeedModes.SUPERSLOW:
					return Textures.TexHUDButtonSpeedSet0;
				case GameSpeedModes.SLOW:
					return Textures.TexHUDButtonSpeedSet1;
				case GameSpeedModes.NORMAL:
					return Textures.TexHUDButtonSpeedSet2;
				case GameSpeedModes.FAST:
					return Textures.TexHUDButtonSpeedSet3;
				case GameSpeedModes.SUPERFAST:
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

			Click();
		}

		public void Click()
		{
			Owner.Owner.PushNotification("HUDSpeedSetButton :: Set Speed := " + speed);
			GDOwner.GDOwner.GameSpeedMode = speed;
			
			BaseButton.Close();
		}
	}
}
