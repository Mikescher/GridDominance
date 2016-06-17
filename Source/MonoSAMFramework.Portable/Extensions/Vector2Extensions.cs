using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.MathHelper.FloatClasses;

namespace MonoSAMFramework.Portable.Extensions
{
	public static class Vector2Extensions
	{
		public static FSize ToSize(this Vector2 p)
		{
			return new FSize(p.X, p.Y);
		}

		public static FPoint ToFPoint(this Vector2 p)
		{
			return new FPoint(p.X, p.Y);
		}
	}
}
