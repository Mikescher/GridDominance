using System;
using GridDominance.Shared.Resources;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.BatchRenderer.TextureAtlases;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Button;

namespace GridDominance.Shared.Screens.NormalGameScreen.HUD
{
	class HUDSpeedSetButton : HUDEllipseButton
	{
		private const int REAL_DIAMETER = 48;

		private readonly GameSpeedModes speed;
		private readonly HUDSpeedBaseButton baseButton;

		public bool IsOpened = false;

		public override int Depth => 0;
		public bool Highlighted = false;

		public HUDSpeedSetButton(HUDSpeedBaseButton baseBtn, GameSpeedModes buttonSpeed)
		{
			speed = buttonSpeed;
			baseButton = baseBtn;

			RelativePosition = baseButton.RelativePosition;
			Size = baseButton.Size;
			Alignment = baseButton.Alignment;

			OverrideEllipseSize = new FSize(REAL_DIAMETER, REAL_DIAMETER);
		}

		public override void OnInitialize()
		{
			// NOP
		}

		public override void OnRemove()
		{
			// NOP
		}

		protected override void DoDrawBackground(IBatchRenderer sbatch, FRectangle bounds)
		{
			sbatch.DrawScaled(Textures.TexHUDButtonBase, Center, 1f, Highlighted ? FlatColors.Emerald : FlatColors.Flamingo, 0f);
		}

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			var c = FlatColors.Clouds;
			if (this.GDHUD().GDOwner.GameSpeedMode == speed)
				c = FlatColors.MidnightBlue;
			else if (IsPressed)
				c = FlatColors.WetAsphalt;

			sbatch.DrawScaled(GetTexture(), Center, 1f, c, 0f);
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

		protected override void DoUpdate(SAMTime gameTime, InputState istate)
		{
			// NOP
		}

		public void Click()
		{
			HUD.Screen.PushNotification("HUDSpeedSetButton :: Set Speed := " + speed);
			this.GDHUD().GDOwner.GameSpeedMode = speed;

			baseButton.Close();
		}

		protected override void OnPress(InputState istate)
		{
			if (IsOpened) Click();
		}

		protected override void OnDoublePress(InputState istate)
		{
			// NOP
		}

		protected override void OnTriplePress(InputState istate)
		{
			// NOP
		}

		protected override void OnHold(InputState istate, float holdTime)
		{
			// NOP
		}
	}
}
