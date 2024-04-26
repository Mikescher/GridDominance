using MonoSAMFramework.Portable.Input;
using System;

namespace MonoSAMFramework.Portable.Screens.Entities.MouseArea
{
	// ReSharper disable FieldCanBeMadeReadOnly.Global
	public class GameEntityMouseAreaLambdaAdapter : IGameEntityMouseAreaListener
	{
		public Action<GameEntityMouseArea, SAMTime, InputState> MouseEnter = null;
		public Action<GameEntityMouseArea, SAMTime, InputState> MouseLeave = null;
		public Action<GameEntityMouseArea, SAMTime, InputState> MouseDown  = null;
		public Action<GameEntityMouseArea, SAMTime, InputState> MouseUp    = null;
		public Action<GameEntityMouseArea, SAMTime, InputState> MouseMove  = null;
		public Action<GameEntityMouseArea, SAMTime, InputState> MouseClick = null;

		public void OnMouseEnter(GameEntityMouseArea sender, SAMTime gameTime, InputState istate) => MouseEnter?.Invoke(sender, gameTime, istate);
		public void OnMouseLeave(GameEntityMouseArea sender, SAMTime gameTime, InputState istate) => MouseLeave?.Invoke(sender, gameTime, istate);
		public void OnMouseDown( GameEntityMouseArea sender, SAMTime gameTime, InputState istate) => MouseDown?.Invoke( sender, gameTime, istate);
		public void OnMouseUp(   GameEntityMouseArea sender, SAMTime gameTime, InputState istate) => MouseUp?.Invoke(   sender, gameTime, istate);
		public void OnMouseMove( GameEntityMouseArea sender, SAMTime gameTime, InputState istate) => MouseMove?.Invoke( sender, gameTime, istate);
		public void OnMouseClick(GameEntityMouseArea sender, SAMTime gameTime, InputState istate) => MouseClick?.Invoke(sender, gameTime, istate);
	}
}
