using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.Input;
using System;

namespace MonoSAMFramework.Portable.Screens.Entities.MouseArea
{
	// ReSharper disable FieldCanBeMadeReadOnly.Global
	public class GameEntityMouseAreaLambdaAdapter : IGameEntityMouseAreaListener
	{
		public Action<GameEntityMouseArea, GameTime, InputState> MouseEnter = null;
		public Action<GameEntityMouseArea, GameTime, InputState> MouseLeave = null;
		public Action<GameEntityMouseArea, GameTime, InputState> MouseDown  = null;
		public Action<GameEntityMouseArea, GameTime, InputState> MouseUp    = null;
		public Action<GameEntityMouseArea, GameTime, InputState> MouseMove  = null;
		public Action<GameEntityMouseArea, GameTime, InputState> MouseClick = null;

		public void OnMouseEnter(GameEntityMouseArea sender, GameTime gameTime, InputState istate) => MouseEnter?.Invoke(sender, gameTime, istate);
		public void OnMouseLeave(GameEntityMouseArea sender, GameTime gameTime, InputState istate) => MouseLeave?.Invoke(sender, gameTime, istate);
		public void OnMouseDown( GameEntityMouseArea sender, GameTime gameTime, InputState istate) => MouseDown?.Invoke( sender, gameTime, istate);
		public void OnMouseUp(   GameEntityMouseArea sender, GameTime gameTime, InputState istate) => MouseUp?.Invoke(   sender, gameTime, istate);
		public void OnMouseMove( GameEntityMouseArea sender, GameTime gameTime, InputState istate) => MouseMove?.Invoke( sender, gameTime, istate);
		public void OnMouseClick(GameEntityMouseArea sender, GameTime gameTime, InputState istate) => MouseClick?.Invoke(sender, gameTime, istate);
	}
}
