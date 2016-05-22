using Microsoft.Xna.Framework;

namespace MonoSAMFramework.Portable.Extensions
{
	public static class SAMGameTimeExtensions
	{
		public static float GetTotalElapsedSeconds(this GameTime gameTime)
		{
			return (float)gameTime.TotalGameTime.TotalSeconds;
		}
	}
}
