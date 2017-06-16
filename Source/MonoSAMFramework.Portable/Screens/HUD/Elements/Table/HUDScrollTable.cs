using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.RenderHelper;

namespace MonoSAMFramework.Portable.Screens.HUD.Elements.Table
{
	public class HUDScrollTable : HUDElement
	{
		private sealed class HSTColumn { public string Text; public float? Width; public float RealWidth; }
		private sealed class HSTRow { public string[] Data; public Color? ForegroundOverride; }

		private const int   DRAGSPEED_RESOLUTION = 1;
		private const float SPEED_MIN            = 24;
		private const float SPEED_MAX            = 128;
		private const float FRICTION             = 10;

		public override int Depth { get; }

		public Color Background       = Color.White;
		public Color Foreground       = Color.Black;
		public Color LineColor        = Color.Red;
		public Color HeaderBackground = Color.LightGray;
		public Color HeaderForeground = Color.DarkCyan;
		public Color ScrollThumbColor = Color.Magenta;
		public float ScrollWidth      = 16;
		public float ScrollHeight     = 64;
		public float LineWidth        = 2;
		public SpriteFont Font        = null;
		public float FontSize         = 16;

		public float ScrollPosition   = 0; // top row

		private bool isDragging = false;
		private float mouseStartPos;
		private float startOffset;
		private float lastMousePos;
		private float lastMousePosTime;
		private ulong lastMousePosTick;
		private float dragSpeed;

		private readonly List<HSTColumn> _columns = new List<HSTColumn>();
		private readonly List<HSTRow> _data = new List<HSTRow>();

		private bool _needsTabRecalc = true;
		private int _maxScrollPosition = 0;

		public HUDScrollTable(int depth = 0)
		{
			Depth = depth;
		}

		public override void OnInitialize()
		{
			if (Font == null) Font = HUD.DefaultFont;
		}

		public void AddColumn(string text, float? width)
		{
			_columns.Add(new HSTColumn { Text = text, Width = width });
			_needsTabRecalc = true;
		}

		public void AddRow(params string[] data)
		{
			_data.Add(new HSTRow{ ForegroundOverride = null, Data = data });
			_needsTabRecalc = true;
		}

		public void AddRowWithColor(Color c, params string[] data)
		{
			_data.Add(new HSTRow { ForegroundOverride = c, Data = data });
			_needsTabRecalc = true;
		}

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			if (_needsTabRecalc) RecalcTabData();

			sbatch.FillRectangle(bounds, Background);

			float rowHeight = FontSize + 2 * LineWidth;
			float py = 0;

			// Header
			{
				sbatch.FillRectangle(new FRectangle(bounds.X, bounds.Y, bounds.Width - ScrollWidth, rowHeight), HeaderBackground);
				float x = 0;
				for (int i = 0; i < _columns.Count; i++)
				{
					if (FloatMath.IsZero(_columns[i].RealWidth)) continue;

					FontRenderHelper.DrawTextCentered(
						sbatch, 
						Font, 
						FontSize, 
						_columns[i].Text, 
						HeaderForeground, 
						new FPoint(bounds.Left + x + _columns[i].RealWidth/2f, bounds.Top + rowHeight / 2f));

					x += _columns[i].RealWidth;
				}
				sbatch.DrawLine(bounds.Left, bounds.Top + rowHeight, bounds.Right - ScrollWidth, bounds.Top + rowHeight, LineColor, LineWidth);

				py += rowHeight;
			}

			// rows
			for (int di = (int)ScrollPosition; di < _data.Count; di++)
			{
				if (py + rowHeight > Height) break;

				float x = 0;
				for (int ci = 0; ci < _columns.Count; ci++)
				{
					if (FloatMath.IsZero(_columns[ci].RealWidth)) continue;

					FontRenderHelper.DrawTextVerticallyCentered(
						sbatch, 
						Font, 
						FontSize,
						_data[di].Data[ci],
						_data[di].ForegroundOverride ?? Foreground, 
						new FPoint(bounds.Left + x + LineWidth * 2, bounds.Top + py + rowHeight / 2f));

					x += _columns[ci].RealWidth;
				}
				sbatch.DrawLine(bounds.Left, bounds.Top + py + rowHeight, bounds.Right - ScrollWidth, bounds.Top + py + rowHeight, LineColor, LineWidth);

				py += rowHeight;
			}

			// scroll
			{
				var colPerPage = (int)(Height / rowHeight) - 1;
				var perc = FloatMath.Clamp(((int) ScrollPosition) / (1f*_data.Count - colPerPage), 0, 1);

				var pos = perc * (Height - ScrollHeight);

				sbatch.FillRectangle(new FRectangle(bounds.Right - ScrollWidth, bounds.Top + pos, ScrollWidth, ScrollHeight), ScrollThumbColor);
			}

			// Vert Lines
			{
				float x = 0;
				for (int i = 0; i < _columns.Count; i++)
				{
					x += _columns[i].RealWidth;
					sbatch.DrawLine(bounds.Left + x, bounds.Top, bounds.Left + x, bounds.Bottom, LineColor, LineWidth);
				}
			}

			sbatch.DrawRectangle(bounds, LineColor, LineWidth);
		}

		public override void OnRemove()
		{
			//
		}

		protected override void DoUpdate(SAMTime gameTime, InputState istate)
		{
			if (_needsTabRecalc) RecalcTabData();

			if (isDragging)
			{
				if (istate.IsRealDown)
					UpdateDrag(gameTime, istate);
				else
					EndDrag();
			}
			else if (FloatMath.IsNotZero(dragSpeed))
			{
				UpdateRestDrag(gameTime);
			}
		}

		protected override void OnAfterRecalculatePosition()
		{
			RecalcTabData();
		}

		private void RecalcTabData()
		{
			_needsTabRecalc = false;

			float rowHeight = FontSize + 2 * LineWidth;
			var colPerPage = Height / rowHeight - 1;
			_maxScrollPosition = _data.Count - (int) colPerPage;
			if (_maxScrollPosition < 0) _maxScrollPosition = 0;

			float remaining = Width - ScrollWidth;
			int freeSpaceCols = 0;

			for (int i = 0; i < _columns.Count; i++)
			{
				if (_columns[i].Width == null)
				{
					freeSpaceCols++;
				}
				else
				{
					var w = FloatMath.Min(remaining, _columns[i].Width.Value);
					_columns[i].RealWidth = w;
					remaining -= w;
					if (remaining < 0) remaining = 0;
				}
			}

			if (freeSpaceCols > 0)
			{
				for (int i = 0; i < _columns.Count; i++)
				{
					if (_columns[i].Width == null)
					{
						_columns[i].RealWidth = remaining / freeSpaceCols;
					}
				}
			}
		}

		protected override bool OnPointerDown(FPoint relPositionPoint, InputState istate)
		{
			StartDrag(istate);
			return true;
		}

		protected override bool OnPointerUp(FPoint relPositionPoint, InputState istate)
		{
			if (isDragging) EndDrag();
			return true;
		}

		private void StartDrag(InputState istate)
		{
			mouseStartPos = istate.GamePointerPosition.Y;
			startOffset = ScrollPosition;

			dragSpeed = 0;
			lastMousePos = istate.GamePointerPosition.Y;
			lastMousePosTick = MonoSAMGame.GameCycleCounter;
			lastMousePosTime = MonoSAMGame.CurrentTime.TotalElapsedSeconds;

			isDragging = true;
		}

		private void UpdateDrag(SAMTime gameTime, InputState istate)
		{
			float rowHeight = FontSize + 2 * LineWidth;

			var delta = istate.GamePointerPosition.Y - mouseStartPos;

			ScrollPosition = startOffset - delta / rowHeight;
			if (ScrollPosition < 0) ScrollPosition = 0;
			if (ScrollPosition > _maxScrollPosition) ScrollPosition = _maxScrollPosition;

			if (MonoSAMGame.GameCycleCounter - lastMousePosTick > DRAGSPEED_RESOLUTION)
			{
				dragSpeed = (istate.GamePointerPosition.Y - lastMousePos) / (gameTime.TotalElapsedSeconds - lastMousePosTime);

				lastMousePosTick = MonoSAMGame.GameCycleCounter;
				lastMousePosTime = gameTime.TotalElapsedSeconds;
				lastMousePos = istate.GamePointerPosition.Y;
			}
		}

		private void UpdateRestDrag(SAMTime gameTime)
		{
			float rowHeight = FontSize + 2 * LineWidth;

			ScrollPosition -= dragSpeed/rowHeight * gameTime.ElapsedSeconds;
			if (ScrollPosition < 0) ScrollPosition = 0;
			if (ScrollPosition > _maxScrollPosition) ScrollPosition = _maxScrollPosition;

			dragSpeed -= dragSpeed * FRICTION * gameTime.ElapsedSeconds;

			if (dragSpeed < SPEED_MIN)
			{
				dragSpeed = 0;
			}
		}

		private void EndDrag()
		{
			if (dragSpeed < SPEED_MIN)
			{
				dragSpeed = 0;
			}
			else if (dragSpeed > SPEED_MAX)
			{
				dragSpeed = SPEED_MAX;
			}

			isDragging = false;
		}

		public HUDScrollTableSingleRowPresenter CreateSingleRowPresenter(params string[] data)
		{
			var sp = new HUDScrollTableSingleRowPresenter(this, Depth);

			foreach (var d in data) sp.AddData(d);

			return sp;
		}

		public float GetColumnWidth(int idx)
		{
			if (_needsTabRecalc) RecalcTabData();

			return _columns[idx].RealWidth;
		}

		public float GetRowHeight()
		{
			return FontSize + 2 * LineWidth;
		}
	}
}
