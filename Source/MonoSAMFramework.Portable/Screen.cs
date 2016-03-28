using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace MonoSAMFramework.Portable
{
	//TODO Use MonoGame.Xetended version once its merged into Main
	// 532bb549f7f15b4e25ac3ea936a05154ed5e35fd 
	public abstract class Screen : IDraw, IUpdate
	{
		public virtual void Show() { }
		public virtual void Hide() { }
		public virtual void Resize(int width, int height) { }
		public virtual void Pause() { }
		public virtual void Resume() { }
		public abstract void Update(GameTime gameTime);
		public abstract void Draw(GameTime gameTime);
	}
}
