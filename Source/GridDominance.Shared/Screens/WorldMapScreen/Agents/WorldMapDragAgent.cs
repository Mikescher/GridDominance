using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.MathHelper.FloatClasses;
using MonoSAMFramework.Portable.Screens.Agents;

namespace GridDominance.Shared.Screens.WorldMapScreen.Agents
{
	class WorldMapDragAgent : GameScreenAgent
	{
		private const float DRAGSPEED_RESOLUTION = 0.01f;

		private const float SPEED_MIN = 24;
		private const float SPEED_MAX = 128;

		private const float FRICTION = 4;


		private bool isDragging = false;

		private FPoint mouseStartPos;
		private Vector2 startOffset;

		private FPoint lastMousePos;
		private float lastMousePosTimer;
		private Vector2 dragSpeed;

		public WorldMapDragAgent(GDWorldMapScreen scrn) : base(scrn)
		{

		}

		public override void Update(GameTime gameTime, InputState istate)
		{
			if (isDragging)
			{
				if (istate.IsDown)
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
				if (istate.IsDown)
				{
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
			mouseStartPos = istate.PointerPosition;
			startOffset = Screen.MapOffset;

			dragSpeed = Vector2.Zero;
			lastMousePos = istate.PointerPosition;
			lastMousePosTimer = 0f;

			isDragging = true;
		}

		private void UpdateDrag(GameTime gameTime, InputState istate)
		{
			var delta = istate.PointerPosition - mouseStartPos;

			Screen.MapOffsetX = startOffset.X + delta.X;
			Screen.MapOffsetY = startOffset.Y + delta.Y;

			lastMousePosTimer += gameTime.GetElapsedSeconds();
			if (lastMousePosTimer > DRAGSPEED_RESOLUTION)
			{
				dragSpeed = (istate.PointerPosition - lastMousePos) / lastMousePosTimer;

				Debug.WriteLine(dragSpeed);

				lastMousePosTimer = 0f;
				lastMousePos = istate.PointerPosition;
			}
		}

		private void UpdateRestDrag(GameTime gameTime)
		{
			Screen.MapOffsetX = Screen.MapOffsetX + dragSpeed.X * gameTime.GetElapsedSeconds();
			Screen.MapOffsetY = Screen.MapOffsetY + dragSpeed.Y * gameTime.GetElapsedSeconds();

			dragSpeed -= dragSpeed * FRICTION * gameTime.GetElapsedSeconds();

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
