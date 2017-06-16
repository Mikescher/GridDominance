using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Agents;

namespace GridDominance.Shared.Screens.WorldMapScreen.Agents
{
	class WorldMapDragAgent : GameScreenAgent
	{
		private const int DRAGSPEED_RESOLUTION = 1;

		private const float SPEED_MIN = 24;

		private const int OUT_OF_BOUNDS_FORCE_BASE = 6;
		private const int OUT_OF_BOUNDS_FORCE_MULT = 12;

		private const float FRICTION = 6f;

		private bool isDragging = false;
		private Vector2 outOfBoundsForce = Vector2.Zero;

		private FPoint mouseStartPos;
		private FPoint startOffset;

		private FPoint lastMousePos;
		private float lastMousePosTime;
		private ulong lastMousePosTick;
		private Vector2 dragSpeed;

		private readonly GDWorldMapScreen _gdScreen;
		private readonly List<FPoint> _nodePositions;

		public override bool Alive => true;

		public WorldMapDragAgent(GDWorldMapScreen scrn, List<FPoint> nodePositions) : base(scrn)
		{
			_gdScreen = scrn;
			_nodePositions = nodePositions;
		}

		public override void Update(SAMTime gameTime, InputState istate)
		{
			_gdScreen.IsDragging = isDragging;

			if (_gdScreen.ZoomState != BistateProgress.Normal)
			{
				if (isDragging) EndDrag();
				return;
			}

			if (isDragging)
			{
				if (istate.IsRealDown)
				{
					UpdateDrag(gameTime, istate);

					_gdScreen.IsBackgroundPressed = true;
				}
				else
				{
					EndDrag();

					_gdScreen.IsBackgroundPressed = false;
				}
			}
			else
			{
				if (istate.IsExclusiveJustDown)
				{
					istate.Swallow(InputConsumer.GameBackground);
					StartDrag(istate);

					_gdScreen.IsBackgroundPressed = true;
				}
				else if (!dragSpeed.IsZero() || !outOfBoundsForce.IsZero())
				{
					UpdateRestDrag(gameTime);

					_gdScreen.IsBackgroundPressed = false;
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
			outOfBoundsForce = Vector2.Zero;

			var cmvp = Screen.CompleteMapViewport;

			var center = cmvp.Center;
			var maxDistSquared = FloatMath.PythSquared(cmvp.Width / 2, cmvp.Height / 2);

			float nearestDistSquared = float.MaxValue;
			FPoint nearestNode = FPoint.Zero;

			foreach (var np in _nodePositions)
			{
				var d = (np - center).LengthSquared();
				if (d < nearestDistSquared)
				{
					nearestDistSquared = d;
					nearestNode = np;
				}
			}

			if (nearestDistSquared <= maxDistSquared)
			{
				return;
			}

			outOfBoundsForce = (center - nearestNode).Normalized() * (OUT_OF_BOUNDS_FORCE_BASE + OUT_OF_BOUNDS_FORCE_MULT * ((nearestNode - center).Length() - FloatMath.Sqrt(maxDistSquared)));

		}

		private void UpdateRestDrag(SAMTime gameTime)
		{
			float dragX = dragSpeed.X + outOfBoundsForce.X;
			float dragY = dragSpeed.Y + outOfBoundsForce.Y;
			
			Screen.MapOffsetX = Screen.MapOffsetX + dragX * gameTime.ElapsedSeconds;
			Screen.MapOffsetY = Screen.MapOffsetY + dragY * gameTime.ElapsedSeconds;

			CalculateOOB();

			dragSpeed -= dragSpeed * FRICTION * gameTime.ElapsedSeconds;

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

			isDragging = false;
		}
	}
}
