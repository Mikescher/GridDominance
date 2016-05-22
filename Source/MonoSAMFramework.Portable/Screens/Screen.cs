using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.Interfaces;
using System;

namespace MonoSAMFramework.Portable.Screens
{
	public abstract class Screen : ILifetimeObject
	{
		public bool IsShown { get; private set; }
		public bool IsRemoved { get; private set; }

		public bool Alive => !IsRemoved;

		public void Show()
		{
#if DEBUG
			if (IsRemoved) throw new Exception("You cannot reuse screens");
#endif

			IsShown = true;

			OnShow();
		}

		public void Remove()
		{
			IsRemoved = true;
			IsShown = false;

			OnRemove();
		}

		protected virtual void OnShow() { }
		protected virtual void OnRemove() { }

		public virtual void Resize(int width, int height) { }

		public virtual void Pause() { }
		public virtual void Resume() { }

		public abstract void Update(GameTime gameTime);
		public abstract void Draw(GameTime gameTime);
	}
}
