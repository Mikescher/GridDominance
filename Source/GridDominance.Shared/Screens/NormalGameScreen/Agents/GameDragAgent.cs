using System;
using System.Collections.Generic;
using System.Text;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.OverworldScreen;
using GridDominance.Shared.Screens.OverworldScreen.Agents;
using GridDominance.Shared.Screens.OverworldScreen.Entities;
using GridDominance.Shared.Screens.ScreenGame;
using GridDominance.Shared.Screens.WorldMapScreen;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Agents;

namespace GridDominance.Shared.Screens.NormalGameScreen.Agents
{
    class GameDragAgent : GameScreenAgent
	{
		private const int DRAGSPEED_RESOLUTION = 1;

		private const float SPEED_MIN = 24;
		private const float SPEED_MAX = 128;

		private const float FRICTION = 12f;

		private bool isDragging = false;

		private FPoint mouseStartPos;
		private Vector2 startOffset;

		private FPoint lastMousePos;
		private float lastMousePosTime;
		private ulong lastMousePosTick;
		private Vector2 dragSpeed;

		private readonly GDGameScreen _gdScreen;
		private readonly FRectangle _bounds;

		public override bool Alive => true;

		public GameDragAgent(GDGameScreen scrn) : base(scrn)
		{
			_gdScreen = scrn;
			_bounds = _gdScreen.MapFullBounds;
		}

		public override void Update(SAMTime gameTime, InputState istate)
		{
			if (isDragging)
			{
				if (istate.IsRealDown)
				{
					UpdateDrag(gameTime, istate);
				}
				else
				{
					EndDrag();
				}
			}
			else
			{
				if (istate.IsExclusiveJustDown)
				{
					istate.Swallow(InputConsumer.GameBackground);
					StartDrag(istate);
				}
				else if (!dragSpeed.IsZero())
				{
					UpdateRestDrag(gameTime);
				}
			}
		}

		private void StartDrag(InputState istate)
		{
			mouseStartPos = istate.GamePointerPosition;
			startOffset = Screen.MapOffset;

			dragSpeed = Vector2.Zero;
			lastMousePos = istate.GamePointerPosition;
			lastMousePosTick = MonoSAMGame.GameCycleCounter;
			lastMousePosTime = MonoSAMGame.CurrentTime.TotalElapsedSeconds;

			isDragging = true;
		}

		private void UpdateDrag(SAMTime gameTime, InputState istate)
		{
			var delta = istate.GamePointerPosition - mouseStartPos;

			Screen.MapOffsetX = startOffset.X + delta.X;
			Screen.MapOffsetY = startOffset.Y + delta.Y;

			CalculateOOB();

			if (MonoSAMGame.GameCycleCounter - lastMousePosTick > DRAGSPEED_RESOLUTION)
			{
				dragSpeed = (istate.GamePointerPosition - lastMousePos) / (gameTime.TotalElapsedSeconds - lastMousePosTime);

				lastMousePosTick = MonoSAMGame.GameCycleCounter;
				lastMousePosTime = gameTime.TotalElapsedSeconds;
				lastMousePos = istate.GamePointerPosition;
			}
		}

		private void CalculateOOB()
		{
			if (Screen.MapOffsetX > _bounds.X)
			{
				dragSpeed.X = 0;
				Screen.MapOffsetX = _bounds.X;
			}

			if (Screen.GuaranteedMapViewport.Right > _bounds.Right)
			{
				dragSpeed.X = 0;
				Screen.MapOffsetX = Screen.VAdapterGame.VirtualGuaranteedWidth - _bounds.Right;
			}

			if (Screen.MapOffsetY > _bounds.Y)
			{
				dragSpeed.Y = 0;
				Screen.MapOffsetY = _bounds.Y;
			}

			if (Screen.GuaranteedMapViewport.Bottom > _bounds.Bottom)
			{
				dragSpeed.Y = 0;
				Screen.MapOffsetY = Screen.VAdapterGame.VirtualGuaranteedHeight - _bounds.Bottom;
			}
		}

		private void UpdateRestDrag(SAMTime gameTime)
		{
			float dragX = dragSpeed.X;
			float dragY = dragSpeed.Y;

			Screen.MapOffsetX = Screen.MapOffsetX + dragX * gameTime.ElapsedSeconds;
			Screen.MapOffsetY = Screen.MapOffsetY + dragY * gameTime.ElapsedSeconds;

			CalculateOOB();

			dragSpeed -= dragSpeed * FloatMath.Min(FRICTION * gameTime.ElapsedSeconds, 1);

			if (dragSpeed.LengthSquared() < SPEED_MIN * SPEED_MIN)
			{
				dragSpeed = Vector2.Zero;
			}
		}

		private void EndDrag()
		{
			var dragSpeedValue = dragSpeed.LengthSquared();

			if (dragSpeedValue < SPEED_MIN * SPEED_MIN)
			{
				dragSpeed = Vector2.Zero;
			}
			else if (dragSpeedValue > SPEED_MAX * SPEED_MAX)
			{
				dragSpeed.Truncate(SPEED_MAX);
			}

			isDragging = false;
		}
	}
}
