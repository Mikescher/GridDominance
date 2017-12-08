﻿namespace MonoSAMFramework.Portable.Screens.HUD.Elements.Keyboard
{
	public interface IKeyboardListener
	{
		void PressChar(char chr);
		void PressBackspace();
		void KeyboardClosed();

		string GetPreviewText();
	}

	public class DummyKeyboardListener : IKeyboardListener
	{
		public void PressChar(char chr) { }
		public void PressBackspace() { }
		public void KeyboardClosed() { }
		public string GetPreviewText() => "";
	}
}
