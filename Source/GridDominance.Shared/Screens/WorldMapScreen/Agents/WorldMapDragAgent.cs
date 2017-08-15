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
		private const float CALCULATION_UPS = 60f;
		private const int MAX_UPDATES_PER_CALL = 8;
		private const float DRAGSPEED_RESOLUTION_TIME = 0.1f;

		private const float SPEED_MIN = 24;

		private const int OUT_OF_BOUNDS_FORCE_BASE = 6;
		private const int OUT_OF_BOUNDS_FORCE_MULT = 12;

		private const float FRICTION = 6f;

		private bool _isDragging = false;
		private bool _dragRestCalculated = false;
		private Vector2 _outOfBoundsForce = Vector2.Zero;

		private FPoint mouseStartPos;
		private FPoint startOffset;

		private FPoint _lastMousePos;
		private float _lastMousePosTime;
		private Vector2 _restDragSpeed;
		private float _timeSinceRestDragUpdate;

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
			_gdScreen.IsDragging = _isDragging;

			if (_gdScreen.ZoomState != BistateProgress.Normal)
			{
				if (_isDragging) EndDrag(gameTime, istate);
				_restDragSpeed = Vector2.Zero;
				return;
			}

			if (_isDragging)
			{
				if (istate.IsRealDown)
				{
					UpdateDrag(gameTime, istate);

					_gdScreen.IsBackgroundPressed = true;
				}
				else
				{
					EndDrag(gameTime, istate);

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
				else if (!_restDragSpeed.IsZero() || !_outOfBoundsForce.IsZero())
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

			_restDragSpeed = Vector2.Zero;
			_lastMousePos = istate.GamePointerPosition;
			_lastMousePosTime = MonoSAMGame.CurrentTime.TotalElapsedSeconds;

			_isDragging = true;
			_dragRestCalculated = false;
			_timeSinceRestDragUpdate = 0;
		}

		private void UpdateDrag(SAMTime gameTime, InputState istate)
		{
			if (istate.AllGamePointerPositions.Length > 1) return;

			var delta = istate.GamePointerPosition - mouseStartPos;

			Screen.MapOffsetX = startOffset.X + delta.X;
			Screen.MapOffsetY = startOffset.Y + delta.Y;

			CalculateOOB();

			if (gameTime.TotalElapsedSeconds - _lastMousePosTime > DRAGSPEED_RESOLUTION_TIME)
			{
				CalcRestDragSpeed(gameTime, istate);
			}
		}

		private void CalcRestDragSpeed(SAMTime gameTime, InputState istate)
		{
			if (istate.AllGamePointerPositions.Length > 1) return;

			_restDragSpeed = (istate.GamePointerPosition - _lastMousePos) / (gameTime.TotalElapsedSeconds - _lastMousePosTime);

			_lastMousePosTime = gameTime.TotalElapsedSeconds;
			_lastMousePos = istate.GamePointerPosition;

			_dragRestCalculated = true;
		}

		private void CalculateOOB()
		{
			_outOfBoundsForce = Vector2.Zero;

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

			_outOfBoundsForce = (center - nearestNode).Normalized() * (OUT_OF_BOUNDS_FORCE_BASE + OUT_OF_BOUNDS_FORCE_MULT * ((nearestNode - center).Length() - FloatMath.Sqrt(maxDistSquared)));

		}

		private void UpdateRestDrag(SAMTime gameTime)
		{
			_timeSinceRestDragUpdate += gameTime.RealtimeElapsedSeconds;

			for (int i = 0; _timeSinceRestDragUpdate > (1 / CALCULATION_UPS); i++)
			{
				if (i >= MAX_UPDATES_PER_CALL) { _timeSinceRestDragUpdate = 0; break; }

				RealRestDragUpdate(1 / CALCULATION_UPS);
				_timeSinceRestDragUpdate -= (1 / CALCULATION_UPS);
			}
		}

		private void RealRestDragUpdate(float delta)
		{
			float dragX = _restDragSpeed.X + _outOfBoundsForce.X;
			float dragY = _restDragSpeed.Y + _outOfBoundsForce.Y;

			Screen.MapOffsetX = Screen.MapOffsetX + dragX * delta;
			Screen.MapOffsetY = Screen.MapOffsetY + dragY * delta;

			CalculateOOB();

			_restDragSpeed -= _restDragSpeed * FRICTION * delta;

			if (_restDragSpeed.LengthSquared() < SPEED_MIN * SPEED_MIN)
			{
				_restDragSpeed = Vector2.Zero;
			}
		}

		private void EndDrag(SAMTime gameTime, InputState istate)
		{
			if (!_dragRestCalculated) CalcRestDragSpeed(gameTime, istate);

			var dragSpeedValue = _restDragSpeed.LengthSquared();

			if (dragSpeedValue < SPEED_MIN * SPEED_MIN)
			{
				_restDragSpeed = Vector2.Zero;
			}

			_isDragging = false;
		}
	}
}
