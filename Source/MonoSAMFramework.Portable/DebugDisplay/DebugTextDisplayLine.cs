using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoSAMFramework.Portable.MathHelper;
using System;

namespace MonoSAMFramework.Portable.DebugDisplay
{
	public class DebugTextDisplayLine
	{
		private static readonly Func<bool> ActionTrue = () => true;

		public readonly Func<string> DisplayText;
		public readonly Func<bool> Active;

		public float InertiaPosition = -1;

		public bool IsAlive { get; private set; } = true;
		public Color Color { get; private set; } = Color.Black;
		public float Decay { get; private set; } = 1f;

		private double age = 0f;

		private double lifetime = double.MaxValue;
		private double decaytime = double.MinValue;
		private double spawntime = 0;

		public Color Background = Color.White;

		public float PositionY = 0f;
		public bool IsDecaying => lifetime < 1000;

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

		public void UpdateDecay(GameTime gameTime, bool first)
		{
			if (age < spawntime && spawntime > 0)
			{
				lifetime -= gameTime.GetElapsedSeconds();
				age += gameTime.GetElapsedSeconds();

				Decay = (float) (age / spawntime);
			}
			else if (lifetime < decaytime)
			{
				if (first)
				{
					lifetime -= gameTime.GetElapsedSeconds();
					age += gameTime.GetElapsedSeconds();

					Decay = (float) (lifetime / decaytime);
				}
				else
				{
					Decay = 1;
				}
			}
			else
			{
				lifetime -= gameTime.GetElapsedSeconds();
				age += gameTime.GetElapsedSeconds();

				Decay = 1;
			}

			if (lifetime < 0) IsAlive = false;

		}

		public void UpdatePosition(GameTime gameTime, SpriteFont font, int lineCount, ref float posY)
		{
			if (InertiaPosition < 0)
			{
				InertiaPosition = posY;
			}
			else if (posY < InertiaPosition)
			{
				var speed = gameTime.GetElapsedSeconds() * DebugTextDisplay.INERTIA_SPEED * FloatMath.Max(1, FloatMath.Round((InertiaPosition - posY) / font.LineSpacing));

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
			
			posY += font.MeasureString(DisplayText()).Y * DebugTextDisplay.TEXT_SPACING;
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
