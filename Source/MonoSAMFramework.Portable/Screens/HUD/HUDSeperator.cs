using MonoSAMFramework.Portable.MathHelper.FloatClasses;
using System;

namespace MonoSAMFramework.Portable.Screens.HUD
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
					Size = new FSize(Size.Width, SeperatorWidth ?? HUD.PixelWidth);
					break;
				case HUDOrientation.Vertical:
					Size = new FSize(SeperatorWidth ?? HUD.PixelWidth, Size.Height);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}
