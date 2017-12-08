using System;
using System.Collections.Generic;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

namespace MonoSAMFramework.Portable.Screens.HUD.Elements.Container
{
	public class HUDFixedUniformGrid : HUDLayoutContainer
	{
		private int _gridWidth = 1;
		public int GridWidth { get => _gridWidth; set { if (_gridWidth != value) { _gridWidth = value; InvalidatePosition(); } } }

		private int _gridHeight = 1;
		public int GridHeight { get => _gridHeight; set { if (_gridHeight != value) { _gridHeight = value; InvalidatePosition(); } } }

		private float rowHeight = 10;
		public float RowHeight { get => rowHeight; set { if (rowHeight != value) { rowHeight = value; InvalidatePosition(); } } }

		private float _columnWidth = 10;
		public float ColumnWidth { get => _columnWidth; set { if (_columnWidth != value) { _columnWidth = value; InvalidatePosition(); } } }

		private float _paddingX = 10;
		public float PaddingX { get => _paddingX; set { if (_paddingX != value) { _paddingX = value; InvalidatePosition(); } } }

		private float _paddingY = 10;
		public float PaddingY { get => _paddingY; set { if (_paddingY != value) { _paddingY = value; InvalidatePosition(); } } }

		public float Padding { set { PaddingX = value; PaddingY = value; } }

		private readonly List<Tuple<DPoint, HUDElement>> _layoutElements = new List<Tuple<DPoint, HUDElement>>();

		public HUDFixedUniformGrid(int depth = 0)
		{
			//
		}

		[Obsolete("Use layout specific add", true)]
		public new void AddElement(HUDElement e)
		{
			throw new NotSupportedException();
		}

		[Obsolete("Use layout specific add", true)]
		public new void AddElements(IEnumerable<HUDElement> es)
		{
			throw new NotSupportedException();
		}

		public void AddElement(int col, int row, HUDElement e)
		{
			base.AddElement(e);

			_layoutElements.Add(Tuple.Create(new DPoint(col, row), e));
		}

		protected override void OnBeforeRecalculatePosition()
		{
			base.OnBeforeRecalculatePosition();

			for (int i = _layoutElements.Count - 1; i >= 0; i--)
			{
				if (!_layoutElements[i].Item2.Alive) _layoutElements.RemoveAt(i);
			}

			Size = new FSize(GridWidth * ColumnWidth + (GridWidth - 1) * PaddingX, GridHeight * RowHeight + (GridHeight - 1) * PaddingY);

			foreach (var el in _layoutElements)
			{
				if (el.Item1.X < 0 || el.Item1.X >= GridWidth) continue;
				if (el.Item1.Y < 0 || el.Item1.Y >= GridHeight) continue;

				el.Item2.Size = new FSize(ColumnWidth, RowHeight);

				el.Item2.Alignment = HUDAlignment.TOPLEFT;
				el.Item2.RelativePosition = new FPoint(el.Item1.X * (ColumnWidth + PaddingX), el.Item1.Y * (RowHeight + PaddingY));
			}
		}

		protected override void OnChildrenChanged()
		{
			base.OnChildrenChanged();

			for (int i = _layoutElements.Count - 1; i >= 0; i--)
			{
				if (!_layoutElements[i].Item2.Alive) _layoutElements.RemoveAt(i);
			}
		}
	}
}
