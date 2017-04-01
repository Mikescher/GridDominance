using System.Collections.Generic;
using GridDominance.Shared.Screens.WorldMapScreen.Entities;
using Microsoft.Xna.Framework;
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
		private const float DRAGSPEED_RESOLUTION = 0.01f;

		private const float SPEED_MIN = 24;
		private const float SPEED_MAX = 128;

		private const int OUT_OF_BOUNDS_FORCE_BASE = 64;
		private const int OUT_OF_BOUNDS_FORCE_MULT = 32;

		private const float FRICTION = 10;

		private readonly FRectangle bounding;

		private bool isDragging = false;
		private Vector2 outOfBoundsForce = Vector2.Zero;

		private FPoint mouseStartPos;
		private Vector2 startOffset;

		private FPoint lastMousePos;
		private float lastMousePosTimer;
		private Vector2 dragSpeed;

		private readonly GDWorldMapScreen _gdScreen;
		private readonly List<Vector2> _nodePositions;

		public override bool Alive => true;

		public WorldMapDragAgent(GDWorldMapScreen scrn, List<Vector2> nodePositions) : base(scrn)
		{
			bounding = scrn.MapFullBounds;

			_gdScreen = scrn;
			_nodePositions = nodePositions;
		}

		public override void Update(SAMTime gameTime, InputState istate)
		{
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
			lastMousePosTimer = 0f;

			isDragging = true;
		}

		private void UpdateDrag(SAMTime gameTime, InputState istate)
		{
			var delta = istate.GamePointerPosition - mouseStartPos;

			Screen.MapOffsetX = startOffset.X + delta.X;
			Screen.MapOffsetY = startOffset.Y + delta.Y;

			CalculateOOB();

			lastMousePosTimer += gameTime.ElapsedSeconds;
			if (lastMousePosTimer > DRAGSPEED_RESOLUTION)
			{
				dragSpeed = (istate.GamePointerPosition - lastMousePos) / lastMousePosTimer;

				//Debug.WriteLine(dragSpeed);

				lastMousePosTimer = 0f;
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
			Vector2 nearestNode = Vector2.Zero;

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
			else if (dragSpeedValue > SPEED_MAX * SPEED_MAX)
			{
				dragSpeed.Truncate(SPEED_MAX);
			}

			isDragging = false;
		}
	}
}
