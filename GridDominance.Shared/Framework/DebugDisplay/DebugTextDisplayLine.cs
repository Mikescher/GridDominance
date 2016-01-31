using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace GridDominance.Shared.Framework.DebugDisplay
{
	class DebugTextDisplayLine
	{
		private static readonly Func<bool> ActionTrue = () => true;

		public readonly Func<string> DisplayText;
		public readonly Func<bool> Active;

		public float inertiaPosition = -1;

		public bool IsAlive { get; private set; } = true;
		public Color Color { get; private set; } = Color.Black;
		public float Decay { get; private set; } = 1f;

		private double age = 0f;

		private double lifetime = double.MaxValue;
		private double decaytime = double.MinValue;
		private double spawntime = 0;

		public DebugTextDisplayLine(Func<string> text)
			: this(text, ActionTrue)
		{
			// 
		}

		public DebugTextDisplayLine(Func<string> text, Func<bool> active)
		{
			DisplayText = text;
			Active = active;
		}

		public void Update(GameTime gameTime, InputState istate)
		{
			lifetime -= gameTime.GetElapsedSeconds();
			age += gameTime.GetElapsedSeconds();

			if (lifetime < 0) IsAlive = false;

			if (age < spawntime && spawntime > 0)
				Decay = (float) (age/spawntime);
			else if (lifetime < decaytime)
				Decay = (float) (lifetime/decaytime);
			else
				Decay = 1;
		}

		public DebugTextDisplayLine SetColor(Color c)
		{
			Color = c;
			return this;
		}

		public DebugTextDisplayLine SetLifetime(double l)
		{
			lifetime = l;
			return this;
		}

		public DebugTextDisplayLine SetDecaytime(double l)
		{
			decaytime = l;
			return this;
		}

		public DebugTextDisplayLine SetSpawntime(double s)
		{
			spawntime = s;
			return this;
		}
	}
}
