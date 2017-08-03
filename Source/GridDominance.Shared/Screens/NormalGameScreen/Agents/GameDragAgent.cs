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
		private const float DRAGSPEED_RESOLUTION_TIME = 0.1f; // time

		private const float SPEED_MIN = 24;
		private const float SPEED_MAX = 2048;

		private const float DRAG_DURATION = 0.1f;

		private bool _isDragging = false;
		private bool _dragSpeedCalculated = false;

		private FPoint _mouseStartPos;
		private FPoint _startOffset;

		private FPoint _lastMousePos;
		private float _lastMousePosTime;
		private ulong _lastMousePosTick;

		private Vector2 _restDragStartSpeed;
		private FPoint _restDragStartPos;
		private float _restDragRemainingTime;

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
				else if (_restDragRemainingTime > 0)
				{
					UpdateRestDrag(gameTime);
				}
			}
		}

		private void StartDrag(InputState istate)
		{
			_mouseStartPos = istate.GamePointerPosition;
			_startOffset = Screen.MapOffset;

			_restDragStartSpeed = Vector2.Zero;
			_restDragRemainingTime = 0;
			_lastMousePos = istate.GamePointerPosition;
			_lastMousePosTick = MonoSAMGame.GameCycleCounter;
			_lastMousePosTime = MonoSAMGame.CurrentTime.TotalElapsedSeconds;

			_isDragging = true;

			_dragSpeedCalculated = true;
		}

		private void UpdateDrag(SAMTime gameTime, InputState istate)
		{
			var delta = istate.GamePointerPosition - _mouseStartPos;

			Screen.MapOffsetX = _startOffset.X + delta.X;
			Screen.MapOffsetY = _startOffset.Y + delta.Y;

			CalculateOOB();

			if (gameTime.TotalElapsedSeconds - _lastMousePosTime > DRAGSPEED_RESOLUTION_TIME)
			{
				UpdateRestDragSpeed(gameTime, istate);
			}
		}

		private void UpdateRestDragSpeed(SAMTime gameTime, InputState istate)
		{
			_restDragStartSpeed = (istate.GamePointerPosition - _lastMousePos) / (gameTime.TotalElapsedSeconds - _lastMousePosTime);

			_lastMousePosTick = MonoSAMGame.GameCycleCounter;
			_lastMousePosTime = gameTime.TotalElapsedSeconds;
			_lastMousePos = istate.GamePointerPosition;

			_dragSpeedCalculated = true;
		}

		private void CalculateOOB()
		{
			if (Screen.MapOffsetX > _bounds.X)
			{
				_restDragRemainingTime = 0;
				Screen.MapOffsetX = _bounds.X;
			}

			if (Screen.GuaranteedMapViewport.Right > _bounds.Right)
			{
				_restDragRemainingTime = 0;
				Screen.MapOffsetX = Screen.VAdapterGame.VirtualGuaranteedWidth - _bounds.Right;
			}

			if (Screen.MapOffsetY > _bounds.Y)
			{
				_restDragRemainingTime = 0;
				Screen.MapOffsetY = _bounds.Y;
			}

			if (Screen.GuaranteedMapViewport.Bottom > _bounds.Bottom)
			{
				_restDragRemainingTime = 0;
				Screen.MapOffsetY = Screen.VAdapterGame.VirtualGuaranteedHeight - _bounds.Bottom;
			}
		}

		private void UpdateRestDrag(SAMTime gameTime)
		{
			_restDragRemainingTime -= gameTime.ElapsedSeconds;

			var fp = (DRAG_DURATION - _restDragRemainingTime) / DRAG_DURATION;

			var sp = FloatMath.Sin(fp * FloatMath.HALF_PI);

			Screen.MapOffsetX = _restDragStartPos.X + sp * (DRAG_DURATION) * _restDragStartSpeed.X;
			Screen.MapOffsetY = _restDragStartPos.Y + sp * (DRAG_DURATION) * _restDragStartSpeed.Y;

			CalculateOOB();
		}

		private void EndDrag(SAMTime gameTime, InputState istate)
		{
			if (!_dragSpeedCalculated) UpdateRestDragSpeed(gameTime, istate);

			if (_restDragStartSpeed.LengthSquared() < SPEED_MIN * SPEED_MIN)
			{
				_restDragStartSpeed = Vector2.Zero;
				_restDragRemainingTime = 0;
				return;
			}

			if (_restDragStartSpeed.LengthSquared() > SPEED_MAX * SPEED_MAX)
			{
				_restDragStartSpeed = _restDragStartSpeed.WithLength(SPEED_MAX);
			}

			_restDragRemainingTime = DRAG_DURATION;
			_restDragStartPos = Screen.MapOffset;

			_isDragging = false;
		}
	}
}
