using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.Screens;
using System;
using System.Collections.Generic;
using System.Linq;
using MonoSAMFramework.Portable.Interfaces;
using MonoSAMFramework.Portable.Font;

namespace MonoSAMFramework.Portable.DebugTools
{
	public class DebugTextDisplayLine : IDebugTextDisplayLineProvider
	{
		private static readonly Func<bool> ActionTrue = () => true;

		public readonly Func<List<string>> DisplayText;
		public readonly Func<bool> Active;

		public float InertiaPosition = -1;

		public bool IsAlive { get; set; } = true;
		public Color Color { get; private set; } = Color.Black;
		public float Decay { get; private set; } = 1f;
		public double Lifetime => lifetime;

		private double age = 0f;

		private double lifetime = double.MaxValue;
		private double decaytime = double.MinValue;
		private double spawntime = 0;

		public Color Background = Color.White;
		public ILifetimeObject Owner = null;

		public float PositionY = 0f;
		public bool IsDecaying => lifetime < 1000;

		public int Order => 0;
		
		public DebugTextDisplayLine(Func<string> text)
			: this(text, ActionTrue)
		{
			// 
		}

		public DebugTextDisplayLine(Func<string> text, Func<bool> active)
			: this(() => new List<string>{ text() }, active)
		{
		}

		public DebugTextDisplayLine(Func<List<string>> text, Func<bool> active)
		{
			DisplayText = text;
			Active = active;
		}

		public IEnumerable<DebugTextDisplayLine> GetLines()
		{
			yield return this;
		}

		public bool RemoveZombies()
		{
			return IsAlive;
		}

		public void Update()
		{
			if (Owner != null && !Owner.Alive) IsAlive = false;
		}

		public void UpdateDecay(SAMTime gameTime, bool first)
		{
			if (age < spawntime && spawntime > 0)
			{
				lifetime -= gameTime.ElapsedSeconds;
				age += gameTime.ElapsedSeconds;

				Decay = (float) (age / spawntime);
			}
			else if (lifetime < decaytime)
			{
				if (first)
				{
					lifetime -= gameTime.ElapsedSeconds;
					age += gameTime.ElapsedSeconds;

					Decay = (float) (lifetime / decaytime);
				}
				else
				{
					Decay = 1;
				}
			}
			else
			{
				lifetime -= gameTime.ElapsedSeconds;
				age += gameTime.ElapsedSeconds;

				Decay = 1;
			}

			if (lifetime < 0) IsAlive = false;

		}

		public void UpdatePosition(SAMTime gameTime, SAMFont font, int lineCount, ref float posY)
		{
			if (InertiaPosition < 0)
			{
				InertiaPosition = posY;
			}
			else if (posY < InertiaPosition)
			{
				var speed = gameTime.ElapsedSeconds * DebugTextDisplay.INERTIA_SPEED * FloatMath.Max(1, FloatMath.Round((InertiaPosition - posY) / font.LineSpacing()));

				if (lineCount > DebugTextDisplay.OVERFLOW_MAX) speed = 99999;

				InertiaPosition = FloatMath.LimitedDec(InertiaPosition, speed, posY);
				posY = InertiaPosition;
			}
			else if (posY > InertiaPosition)
			{
				// should never happen ^^
				InertiaPosition = posY;
			}

			PositionY = posY;
			
			posY += DisplayText().Max(l => font.MeasureString(l).Y) + DebugTextDisplay.TEXT_SPACING;
		}

		public DebugTextDisplayLine SetColor(Color c)
		{
			Color = c;
			return this;
		}

		public DebugTextDisplayLine SetBackground(Color c)
		{
			Background = c;
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
