using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.MathHelper.FloatClasses;

namespace MonoSAMFramework.Portable.Extensions
{
	public static class RectangleExtensions
	{
		public static FRectangle ToFRectangle(this Rectangle p)
		{
			return new FRectangle(p);
		}
	}
}
