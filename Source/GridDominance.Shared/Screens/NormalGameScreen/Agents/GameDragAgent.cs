using GridDominance.Shared.Screens.ScreenGame;
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
		private const float CALCULATION_UPS = 60f;
		private const int MAX_UPDATES_PER_CALL = 8;
		private const float DRAGSPEED_RESOLUTION_TIME = 0.1f;

		private const float SPEED_MIN = 24;

		private const float FRICTION = 12f;

		private bool _isDragging = false;
		private bool _dragRestCalculated = false;

		private FPoint _mouseStartPos;
		private FPoint _startOffset;

		private FPoint _lastMousePos;
		private float _lastMousePosTime;
		private Vector2 _dragSpeed;
		private float _timeSinceRestDragUpdate;

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
			if (_isDragging)
			{
				if (istate.IsRealDown)
				{
					UpdateDrag(gameTime, istate);
				}
				else
				{
					EndDrag(gameTime, istate);
				}
			}
			else
			{
				if (istate.IsExclusiveJustDown)
				{
					istate.Swallow(InputConsumer.GameBackground);
					StartDrag(istate);
				}
				else if (!_dragSpeed.IsZero())
				{
					UpdateRestDrag(gameTime);
				}
			}
		}

		private void StartDrag(InputState istate)
		{
			_mouseStartPos = istate.GamePointerPosition;
			_startOffset = Screen.MapOffset;

			_dragSpeed = Vector2.Zero;
			_lastMousePos = istate.GamePointerPosition;
			_lastMousePosTime = MonoSAMGame.CurrentTime.TotalElapsedSeconds;

			_isDragging = true;
			_dragRestCalculated = false;
			_timeSinceRestDragUpdate = 0;
		}

		private void UpdateDrag(SAMTime gameTime, InputState istate)
		{
			var delta = istate.GamePointerPosition - _mouseStartPos;

			Screen.MapOffsetX = _startOffset.X + delta.X;
			Screen.MapOffsetY = _startOffset.Y + delta.Y;

			CalculateOOB();

			if (gameTime.TotalElapsedSeconds - _lastMousePosTime > DRAGSPEED_RESOLUTION_TIME)
			{
				CalcRestDragSpeed(gameTime, istate);
			}
		}

		private void CalcRestDragSpeed(SAMTime gameTime, InputState istate)
		{
			_dragSpeed = (istate.GamePointerPosition - _lastMousePos) / (gameTime.TotalElapsedSeconds - _lastMousePosTime);

			_lastMousePosTime = gameTime.TotalElapsedSeconds;
			_lastMousePos = istate.GamePointerPosition;

			_dragRestCalculated = true;
		}

		private void CalculateOOB()
		{
			if (Screen.MapOffsetX > _bounds.X)
			{
				_dragSpeed.X = 0;
				Screen.MapOffsetX = _bounds.X;
			}

			if (Screen.GuaranteedMapViewport.Right > _bounds.Right)
			{
				_dragSpeed.X = 0;
				Screen.MapOffsetX = Screen.VAdapterGame.VirtualGuaranteedWidth - _bounds.Right;
			}

			if (Screen.MapOffsetY > _bounds.Y)
			{
				_dragSpeed.Y = 0;
				Screen.MapOffsetY = _bounds.Y;
			}

			if (Screen.GuaranteedMapViewport.Bottom > _bounds.Bottom)
			{
				_dragSpeed.Y = 0;
				Screen.MapOffsetY = Screen.VAdapterGame.VirtualGuaranteedHeight - _bounds.Bottom;
			}
		}

		private void UpdateRestDrag(SAMTime gameTime)
		{
			_timeSinceRestDragUpdate += gameTime.RealtimeElapsedSeconds;

			for (int i = 0; _timeSinceRestDragUpdate > (1 / CALCULATION_UPS); i++)
			{
				if (i >= MAX_UPDATES_PER_CALL) { _timeSinceRestDragUpdate = 0; break; }

				UpdateRealRestDrag(1 / CALCULATION_UPS);
				_timeSinceRestDragUpdate -= (1 / CALCULATION_UPS);
			}
		}

		private void UpdateRealRestDrag(float delta)
		{
			float dragX = _dragSpeed.X;
			float dragY = _dragSpeed.Y;

			Screen.MapOffsetX = Screen.MapOffsetX + dragX * delta;
			Screen.MapOffsetY = Screen.MapOffsetY + dragY * delta;

			CalculateOOB();

			_dragSpeed -= _dragSpeed * FloatMath.Min(FRICTION * delta, 1);

			if (_dragSpeed.LengthSquared() < SPEED_MIN * SPEED_MIN)
			{
				_dragSpeed = Vector2.Zero;
			}
		}

		private void EndDrag(SAMTime gameTime, InputState istate)
		{
			if (!_dragRestCalculated) CalcRestDragSpeed(gameTime, istate);

			var dragSpeedValue = _dragSpeed.LengthSquared();

			if (dragSpeedValue < SPEED_MIN * SPEED_MIN)
			{
				_dragSpeed = Vector2.Zero;
			}
			
			_isDragging = false;
		}
	}
}
