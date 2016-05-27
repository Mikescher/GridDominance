using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace MonoSAMFramework.Portable.Extensions
{
	public static class Vector2Extensions
	{
		public static Size ToSize(this Vector2 p)
		{
			return new Size((int) p.X, (int) p.Y);
		}
	}
}
