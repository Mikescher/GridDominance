using System.Collections.Generic;
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
using MonoSAMFramework.Portable.Localization;
using MonoSAMFramework.Portable.RenderHelper;

namespace GridDominance.Shared.Screens.NormalGameScreen.HUD
{
	public class HUDPauseButton : HUDEllipseButton
	{
		public const int DIAMETER = 48;

		public const float ANIMATION_SPEED = 0.5f;

		private GDGameScreen GDScreen => (GDGameScreen) HUD.Screen;

		private bool isOpened = false;
		private float animationProgress = 0f;

		public override int Depth => 1;

		private HUDPauseMenuButton[] subMenu = null;

		private readonly bool _showResume;
		private readonly bool _showRestart;
		private readonly bool _showExit;

		public HUDPauseButton(bool resume, bool restart, bool exit)
		{
			RelativePosition = new FPoint(12, 12);
			Size = new FSize(DIAMETER, DIAMETER);
			Alignment = HUDAlignment.TOPRIGHT;

			OverrideEllipseSize = new FSize(DIAMETER + 12 + 12, DIAMETER + 12 + 12);

			_showResume = resume;
			_showRestart = restart;
			_showExit = exit;

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
			if (!isOpened && FloatMath.IsZero(animationProgress) && !GDScreen.HasFinished)
			{
				bool trigger = false;
				if (istate.IsKeyExclusiveJustDown(SKeys.AndroidBack))
				{
					istate.SwallowKey(SKeys.AndroidBack, InputConsumer.ScreenAgent);
					trigger = true;
				}
				else if (istate.IsKeyExclusiveJustDown(SKeys.Backspace))
				{
					istate.SwallowKey(SKeys.Backspace, InputConsumer.ScreenAgent);
					trigger = true;
				}

				if (trigger) Open();
			}

			if (isOpened && FloatMath.IsNotOne(animationProgress))
			{
				animationProgress = FloatMath.LimitedInc(animationProgress, gameTime.ElapsedSeconds / ANIMATION_SPEED, 1f);
			}
			else if (!isOpened && FloatMath.IsNotZero(animationProgress))
			{
				animationProgress = FloatMath.LimitedDec(animationProgress, gameTime.ElapsedSeconds / ANIMATION_SPEED, 0f);
			}
		}

		private void Close()
		{
			isOpened = false;

			if (subMenu == null) return;

			if (this.GDHUD().GDOwner.CanPause) this.GDHUD().GDOwner.IsPaused = false;

			foreach (var button in subMenu)
			{
				button.IsClosing = true;
			}
			subMenu = null;
		}

		private void Open()
		{
			isOpened = true;

			if (this.GDHUD().GDOwner.CanPause) this.GDHUD().GDOwner.IsPaused = true;

			var t1 = L10N.T(L10NImpl.STR_PAUS_RESUME);
			var t2 = L10N.T(L10NImpl.STR_PAUS_RESTART);
			var t3 = L10N.T(L10NImpl.STR_PAUS_EXIT);

			var w1 = FontRenderHelper.MeasureStringCached(Textures.HUDFontBold, t1, 40f);
			var w2 = FontRenderHelper.MeasureStringCached(Textures.HUDFontBold, t2, 40f);
			var w3 = FontRenderHelper.MeasureStringCached(Textures.HUDFontBold, t3, 40f);

			var w = FloatMath.Max(w1.Width, w2.Width, w3.Width);

			int cnt = (_showResume ? 1 : 0) + (_showRestart ? 1 : 0) + (_showExit ? 1 : 0);

			var m  = new List<HUDPauseMenuButton>();
			int idx = 0;
			int dep = -1;
			
			if (_showResume)  m.Add(new HUDPauseMenuButton(this, t1, w, dep--, idx++, cnt, OnResume));
			if (_showRestart) m.Add(new HUDPauseMenuButton(this, t2, w, dep--, idx++, cnt, OnRestart));
			if (_showExit)    m.Add(new HUDPauseMenuButton(this, t3, w, dep--, idx++, cnt, OnExit));

			subMenu = m.ToArray();

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
			this.GDHUD().GDOwner.ExitToMap();
		}

		protected override void OnPress(InputState istate)
		{
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
