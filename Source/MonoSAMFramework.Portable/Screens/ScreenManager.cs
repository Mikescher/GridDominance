﻿using Microsoft.Xna.Framework;
using System;

namespace MonoSAMFramework.Portable.Screens
{
	public class ScreenManager
	{
		private readonly Game _game;

		public ScreenManager(Game game)
		{
			_game = game;
			_game.Window.ClientSizeChanged += WindowOnClientSizeChanged;
			_game.Window.OrientationChanged += WindowOnClientSizeChanged;
			_game.Activated += (s, e) => Resume();
			_game.Deactivated += (s, e) => Pause();

			IsPaused = !_game.IsActive;
		}

		public bool IsPaused { get; private set; }

		private void WindowOnClientSizeChanged(object sender, EventArgs eventArgs)
		{
			if (_currentScreen != null)
			{
				var bounds = _game.Window.ClientBounds;
				_currentScreen.Resize(bounds.Width, bounds.Height);
			}
		}

		private GameScreen _currentScreen;
		public GameScreen CurrentScreen
		{
			get { return _currentScreen; }
			set
			{
				if (_currentScreen != null)
				{
					_currentScreen.Remove();
					var phud = _currentScreen.HUD;
					_currentScreen = value;
					_currentScreen?.Show();
					if (phud != null) _currentScreen?.HUD?.CopyToast(phud);
				}
				else
				{
					_currentScreen = value;
					_currentScreen?.Show();
				}
			}
		}

		public void Pause()
		{
			if (!IsPaused)
			{
				IsPaused = true;

				_currentScreen?.Pause();
			}
		}

		public void Resume()
		{
			if (IsPaused)
			{
				IsPaused = false;

				_currentScreen?.Resume();
			}
		}

		public void Draw(SAMTime gameTime)
		{
			_currentScreen?.Draw(gameTime);
		}

		public void Update(SAMTime gameTime)
		{
			_currentScreen?.Update(gameTime);
		}
	}
}
