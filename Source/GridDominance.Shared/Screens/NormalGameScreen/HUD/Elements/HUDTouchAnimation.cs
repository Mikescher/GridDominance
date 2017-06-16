using System;
using System.Collections.Generic;
using System.Text;
using GridDominance.Shared.Resources;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.HUD;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

namespace GridDominance.Shared.Screens.NormalGameScreen.HUD
{
	public class HUDTouchAnimation : HUDElement
	{
		public override int Depth { get; } = 0;

		private float TIME_APPEAR = 0.4f;
		private float TIME_STARTWAIT = 0.2f;
		private float TIME_MOVE = 1.8f;
		private float TIME_ENDWAIT = 0.2f;
		private float TIME_DISAPPEAR = 0.6f;
		private float TIME_DEAD = 0.5f;

		private int _mode = 0; // 0=Appear | 1=wait | 2=move | 3=wait | 4=dissapear | 5=dead
		private float _progress = 0f;

		private readonly FPoint _start;
		private readonly FPoint _end;

		public HUDTouchAnimation(FPoint p1, FPoint p2)
		{
			Alignment = HUDAlignment.ABSOLUTE;
			Size = new FSize(128, 128);

			_start = p1 + new Vector2(8, 30);
			_end   = p2 + new Vector2(8, 30);

			RelativePosition = _start;
		}

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			switch (_mode)
			{
				case 0:
					sbatch.DrawStretched(Textures.TexHUDIconTouchUp, bounds, Color.White * _progress);
					break;
				case 1:
					sbatch.DrawStretched(Textures.TexHUDIconTouchUp, bounds, Color.White);
					break;
				case 2:
					sbatch.DrawStretched(Textures.TexHUDIconTouchDown, bounds, Color.White);
					break;
				case 3:
					sbatch.DrawStretched(Textures.TexHUDIconTouchUp, bounds, Color.White);
					break;
				case 4:
					sbatch.DrawStretched(Textures.TexHUDIconTouchUp, bounds, Color.White * (1 - _progress));
					break;
				case 5:
					//
					break;
			}

		}

		public override void OnInitialize()
		{

		}

		public override void OnRemove()
		{

		}

		protected override void DoUpdate(SAMTime gameTime, InputState istate)
		{
			switch (_mode)
			{
				case 0:
					_progress += gameTime.ElapsedSeconds / TIME_APPEAR;
					RelativeCenter = _start;
					if (_progress >= 1) { _progress = 0; _mode++; }
					break;
				case 1:
					_progress += gameTime.ElapsedSeconds / TIME_STARTWAIT;
					RelativeCenter = _start;
					if (_progress >= 1) { _progress = 0; _mode++; }
					break;
				case 2:
					_progress += gameTime.ElapsedSeconds / TIME_MOVE;
					RelativeCenter = FPoint.Lerp(_start, _end, FloatMath.FunctionEaseInOutCubic(_progress));
					if (_progress >= 1) { _progress = 0; _mode++; }
					break;
				case 3:
					_progress += gameTime.ElapsedSeconds / TIME_ENDWAIT;
					RelativeCenter = _end;
					if (_progress >= 1) { _progress = 0; _mode++; }
					break;
				case 4:
					_progress += gameTime.ElapsedSeconds / TIME_DISAPPEAR;
					RelativeCenter = _end;
					if (_progress >= 1) { _progress = 0; _mode++; }
					break;
				case 5:
					_progress += gameTime.ElapsedSeconds / TIME_DEAD;
					if (_progress >= 1) { _progress = 0; _mode = 0; }
					break;
			}
		}
	}
}
