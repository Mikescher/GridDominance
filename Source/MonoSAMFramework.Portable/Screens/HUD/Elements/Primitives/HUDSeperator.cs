using System;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

namespace MonoSAMFramework.Portable.Screens.HUD.Elements.Primitives
{
	public class HUDSeperator : HUDRectangle
	{
		private HUDOrientation _orientation;
		public HUDOrientation Orientation
		{
			get { return _orientation; }
			set { _orientation = value; InvalidatePosition(); }
		}

		public int? SeperatorWidth = null;

		public Color Color { set => Definition = HUDBackgroundDefinition.CreateSimple(value); }

		public HUDSeperator(HUDOrientation orientation, int depth = 0)
			: base(depth)
		{
			_orientation = orientation;
		}

		protected override void OnBeforeRecalculatePosition()
		{
			switch (Orientation)
			{
				case HUDOrientation.Horizontal:
					Size = new FSize(Size.Width, (SeperatorWidth ?? 1) * HUD.PixelWidth);
					break;
				case HUDOrientation.Vertical:
					Size = new FSize((SeperatorWidth ?? 1) * HUD.PixelWidth, Size.Height);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}
