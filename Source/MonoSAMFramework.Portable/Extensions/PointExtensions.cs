using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.MathHelper;
using MonoSAMFramework.Portable.MathHelper.FloatClasses;

namespace MonoSAMFramework.Portable.Extensions
{
	public static class PointExtensions
	{
		public static FPoint ToFPoint(this Point p)
		{
			return new FPoint(p);
		}
	}
}
