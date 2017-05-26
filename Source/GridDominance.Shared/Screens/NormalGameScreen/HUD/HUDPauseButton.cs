using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.ScreenGame;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.BatchRenderer.TextureAtlases;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Button;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

namespace GridDominance.Shared.Screens.NormalGameScreen.HUD
{
	public class HUDPauseButton : HUDEllipseButton
	{
		public const int DIAMETER = 48;

		public const float ANIMATION_SPEED = 2.5f;

		private GDGameScreen GDScreen => (GDGameScreen) HUD.Screen;

		private bool isOpened = false;
		private float animationProgress = 0f;

		public override int Depth => 1;

		private HUDPauseMenuButton[] subMenu = null;

		public HUDPauseButton()
		{
			RelativePosition = new FPoint(12, 12);
			Size = new FSize(DIAMETER, DIAMETER);
			Alignment = HUDAlignment.TOPRIGHT;

			OverrideEllipseSize = new FSize(DIAMETER + 12 + 12, DIAMETER + 12 + 12);

#if DEBUG
			ClickMode = HUDButtonClickMode.Single | HUDButtonClickMode.InstantHold;
#else
			ClickMode = HUDButtonClickMode.Single;
#endif

		}

		public override void OnInitialize()
		{
			// NOP
		}

		public override void OnRemove()
		{
			// NOP
		}

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
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
			
			sbatch.DrawScaled(Textures.TexHUDButtonBase, Center, texScale, FlatColors.Asbestos, 0f);
			sbatch.DrawScaled(texIcon, Center, texScale, IsPressed ? FlatColors.WetAsphalt : FlatColors.Clouds, 0f);
		}

		protected override void DoUpdate(SAMTime gameTime, InputState istate)
		{
			if (isOpened && FloatMath.IsNotOne(animationProgress))
			{
				animationProgress = FloatMath.LimitedInc(animationProgress, gameTime.ElapsedSeconds * ANIMATION_SPEED, 1f);
			}
			else if (!isOpened && FloatMath.IsNotZero(animationProgress))
			{
				animationProgress = FloatMath.LimitedDec(animationProgress, gameTime.ElapsedSeconds * ANIMATION_SPEED, 0f);
			}
		}

		private void Close()
		{
			isOpened = false;

			this.GDHUD().GDOwner.IsPaused = false;

			foreach (var button in subMenu)
			{
				button.IsClosing = true;
			}
			subMenu = null;

			HUD.Screen.PushNotification("HUDPauseButton :: Game resumed");
		}

		private void Open()
		{
			isOpened = true;

			this.GDHUD().GDOwner.IsPaused = true;

			HUD.Screen.PushNotification("HUDPauseButton :: Game paused");

			subMenu = new[]
			{
				new HUDPauseMenuButton(this, "RESUME", -1, 0, 3, OnResume),
				new HUDPauseMenuButton(this, "RESTART", -2, 1, 3, OnRestart),
				new HUDPauseMenuButton(this, "EXIT", -3, 2, 3, OnExit),
			};

			Owner.AddElements(subMenu);
		}

		private void OnResume()
		{
			Close();
		}

		private void OnRestart()
		{
			this.GDHUD().GDOwner.RestartLevel();
		}

		private void OnExit()
		{
			if (GDScreen.IsTutorial)
				MainGame.Inst.SetOverworldScreen();
			else
				MainGame.Inst.SetWorldMapScreenZoomedOut(GDScreen.WorldBlueprint, GDScreen.Blueprint.UniqueID);
		}

		protected override void OnPress(InputState istate)
		{
			this.GDHUD().GDOwner.PushNotification("Single Press");

			if (!isOpened)
			{
				Open();
			}
			else
			{
				Close();
			}
		}

		protected override void OnDoublePress(InputState istate)
		{
			// Not Available
		}

		protected override void OnTriplePress(InputState istate)
		{
			// Not Available
		}

		protected override void OnHold(InputState istate, float holdTime)
		{
			// Not Available
		}

		protected override void OnEnabledChanged(bool newValue)
		{
			base.OnEnabledChanged(newValue);

			if (!newValue)
			{
				isOpened = false;
				animationProgress = 0;
			}
		}
	}
}
