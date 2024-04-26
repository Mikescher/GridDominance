using System;
using System.Collections.Generic;
using System.Linq;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

namespace MonoSAMFramework.Portable.Screens.HUD.Elements.Container
{
	public class HUDWrapperContainer : HUDLayoutContainer
	{
		public HUDElement Child => Children.FirstOrDefault();

		public HUDWrapperContainer(int depth = 0)
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

		public void SetElement(HUDElement e)
		{
			ClearChildren();

			if (e != null)
			{
				e.Alignment = HUDAlignment.CENTER;
				e.RelativePosition = FPoint.Zero;
				e.Size = this.Size;
			}

			base.AddElement(e);

			var c = Child;
			if (c != null)
			{
				c.Alignment = HUDAlignment.CENTER;
				c.RelativePosition = FPoint.Zero;
				c.Size = this.Size;
			}
		}

		protected override void OnChildrenChanged()
		{
			while (ChildrenCount > 1) RemoveChild(Children.First());

			var c = Child;
			if (c != null)
			{
				c.Alignment = HUDAlignment.CENTER;
				c.RelativePosition = FPoint.Zero;
				c.Size = this.Size;
			}
		}

		protected override void OnAfterRecalculatePosition()
		{
			var c = Child;
			if (c != null)
			{
				c.Alignment = HUDAlignment.CENTER;
				c.RelativePosition = FPoint.Zero;
				c.Size = this.Size;
			}
		}
	}
}
