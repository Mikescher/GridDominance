using Microsoft.Xna.Framework;

namespace MonoSAMFramework.Portable.Extensions
{
	public static class GameTimeExtensions
	{
		public static float GetElapsedSeconds(this GameTime gameTime)
		{
			return (float)gameTime.ElapsedGameTime.TotalSeconds;
		}

		public static float GetTotalSeconds(this GameTime gameTime)
		{
			return (float)gameTime.TotalGameTime.TotalSeconds;
		}
	}
}
