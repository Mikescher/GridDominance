using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.RenderHelper;

namespace MonoSAMFramework.Portable.Screens.HUD.Elements.Other
{
	public class HUDScrollTableSingleRowPresenter : HUDElement
	{
		public override int Depth { get; }

		public Color Foreground;

		private readonly HUDScrollTable _colSource;
		private readonly List<string> _data = new List<string>();

		public HUDScrollTableSingleRowPresenter(HUDScrollTable columnSource, int depth = 0)
		{
			_colSource = columnSource;
			Foreground = _colSource.Foreground;
			Depth = depth;
		}

		public override void OnInitialize()
		{
			//
		}

		public void AddData(string text)
		{
			_data.Add(text);
		}

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			sbatch.FillRectangle(bounds, _colSource.Background);

			float rowHeight = _colSource.FontSize + 2 * _colSource.LineWidth;

			// rows
			{
				float x = 0;
				for (int ci = 0; ci < _data.Count; ci++)
				{
					if (FloatMath.IsZero(_colSource.GetColumnWidth(ci))) continue;

					FontRenderHelper.DrawTextVerticallyCentered(
						sbatch,
						_colSource.Font,
						_colSource.FontSize,
						_data[ci],
						Foreground,
						new Vector2(bounds.Left + x + _colSource.LineWidth * 2, bounds.Top + rowHeight / 2f));

					x += _colSource.GetColumnWidth(ci);
				}
			}

			// scroll
			{
				sbatch.FillRectangle(new FRectangle(bounds.Right - _colSource.ScrollWidth, bounds.Top, _colSource.ScrollWidth, bounds.Height), _colSource.ScrollThumbColor);
			}

			// Vert Lines
			{
				float x = 0;
				for (int i = 0; i < _data.Count; i++)
				{
					x += _colSource.GetColumnWidth(i);
					sbatch.DrawLine(bounds.Left + x, bounds.Top, bounds.Left + x, bounds.Bottom, _colSource.LineColor, _colSource.LineWidth);
				}
			}

			sbatch.DrawRectangle(bounds, _colSource.LineColor, _colSource.LineWidth);
		}

		public override void OnRemove()
		{
			//
		}

		protected override void DoUpdate(SAMTime gameTime, InputState istate)
		{
			//
		}
	}
}
