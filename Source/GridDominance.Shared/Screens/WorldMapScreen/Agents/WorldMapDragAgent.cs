using System.Diagnostics;
using GridDominance.Shared.Screens.ScreenGame;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.GameMath.FloatClasses;
using MonoSAMFramework.Portable.Screens.Agents;

namespace GridDominance.Shared.Screens.WorldMapScreen.Agents
{
	class WorldMapDragAgent : GameScreenAgent
	{
		private const float DRAGSPEED_RESOLUTION = 0.01f;

		private const float SPEED_MIN = 24;
		private const float SPEED_MAX = 128;

		private const int OUT_OF_BOUNDS_FORCE_BASE = 64;
		private const int OUT_OF_BOUNDS_FORCE_MULT = 32;

		private const float FRICTION = 10;

		public static readonly FRectangle BOUNDING = new FRectangle(-16 * GDGameScreen.TILE_WIDTH, -8 * GDGameScreen.TILE_WIDTH, 128 * GDGameScreen.TILE_WIDTH, 32 * GDGameScreen.TILE_WIDTH);

		private bool isDragging = false;
		private Vector2 outOfBoundsForce = Vector2.Zero;

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
				else if (!dragSpeed.IsZero() || !outOfBoundsForce.IsZero())
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

			CalculateOOB();

			lastMousePosTimer += gameTime.GetElapsedSeconds();
			if (lastMousePosTimer > DRAGSPEED_RESOLUTION)
			{
				dragSpeed = (istate.PointerPosition - lastMousePos) / lastMousePosTimer;

				Debug.WriteLine(dragSpeed);

				lastMousePosTimer = 0f;
				lastMousePos = istate.PointerPosition;
			}
		}

		private void CalculateOOB()
		{
			outOfBoundsForce = Vector2.Zero;

			if (Screen.CompleteMapViewport.Left < BOUNDING.Left)
			{
				var force = OUT_OF_BOUNDS_FORCE_BASE + OUT_OF_BOUNDS_FORCE_MULT * (BOUNDING.Left - Screen.CompleteMapViewport.Left);

				outOfBoundsForce.X -= force;
			}
			else if (Screen.CompleteMapViewport.Right > BOUNDING.Right)
			{
				var force = OUT_OF_BOUNDS_FORCE_BASE + OUT_OF_BOUNDS_FORCE_MULT * (BOUNDING.Right - Screen.CompleteMapViewport.Right);

				outOfBoundsForce.X -= force;
			}

			if (Screen.CompleteMapViewport.Top < BOUNDING.Top)
			{
				var force = OUT_OF_BOUNDS_FORCE_BASE + OUT_OF_BOUNDS_FORCE_MULT * (BOUNDING.Top - Screen.CompleteMapViewport.Top);

				outOfBoundsForce.Y -= force;
			}
			else if (Screen.CompleteMapViewport.Bottom > BOUNDING.Bottom)
			{
				var force = OUT_OF_BOUNDS_FORCE_BASE + OUT_OF_BOUNDS_FORCE_MULT * (BOUNDING.Bottom - Screen.CompleteMapViewport.Bottom);

				outOfBoundsForce.Y -= force;
			}
		}

		private void UpdateRestDrag(GameTime gameTime)
		{
			float dragX = dragSpeed.X + outOfBoundsForce.X;
			float dragY = dragSpeed.Y + outOfBoundsForce.Y;
			
			Screen.MapOffsetX = Screen.MapOffsetX + dragX * gameTime.GetElapsedSeconds();
			Screen.MapOffsetY = Screen.MapOffsetY + dragY * gameTime.GetElapsedSeconds();

			CalculateOOB();

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
