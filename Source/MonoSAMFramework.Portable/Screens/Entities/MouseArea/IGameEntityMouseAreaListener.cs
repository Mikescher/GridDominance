using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.Input;

namespace MonoSAMFramework.Portable.Screens.Entities.MouseArea
{
	public interface IGameEntityMouseAreaListener
	{
		void OnMouseEnter(GameEntityMouseArea sender, SAMTime gameTime, InputState istate);
		void OnMouseLeave(GameEntityMouseArea sender, SAMTime gameTime, InputState istate);
		void OnMouseDown(GameEntityMouseArea sender, SAMTime gameTime, InputState istate);
		void OnMouseUp(GameEntityMouseArea sender, SAMTime gameTime, InputState istate);
		void OnMouseMove(GameEntityMouseArea sender, SAMTime gameTime, InputState istate);
		void OnMouseClick(GameEntityMouseArea sender, SAMTime gameTime, InputState istate);
	}
}
