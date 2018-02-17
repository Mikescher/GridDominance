using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.Leveleditor;
using GridDominance.Shared.Screens.ScreenGame;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.UpdateAgents;

namespace GridDominance.Shared.Screens.NormalGameScreen.Agents
{
	class LeveleditorDragAgent : SAMUpdateOp<LevelEditorScreen>
	{
		private const float RETURN_SPEED = 48 * GDConstants.TILE_WIDTH;

		private bool _isDragging = false;

		private FPoint _mouseStartPos;
		private FPoint _startOffset;

		private LevelEditorScreen _gdScreen;

		private FRectangle _boundsMap;
		private FRectangle _boundsWorkingArea;
		private DVector _oobForce;

		public override string Name => "LeveleditorDragAgent";

		public LeveleditorDragAgent()
		{
			//
		}

		protected override void OnInit(LevelEditorScreen screen)
		{
			_gdScreen = screen;
		}

		protected override void OnUpdate(LevelEditorScreen screen, SAMTime gameTime, InputState istate)
		{
			_boundsWorkingArea = _gdScreen.VAdapterGame.VirtualTotalBoundingBox.AsDeflated(0, 4 * GDConstants.TILE_WIDTH, 4 * GDConstants.TILE_WIDTH, 0);

			_boundsMap = FRectangle.CreateByTopLeft(
				_boundsWorkingArea.X + _gdScreen.MapOffsetX,
				_boundsWorkingArea.Y + _gdScreen.MapOffsetY,
				_gdScreen.LevelData.Width * GDConstants.TILE_WIDTH,
				_gdScreen.LevelData.Height * GDConstants.TILE_WIDTH);

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
				else if (!_oobForce.IsZero())
				{
					UpdateRestDrag(gameTime);
				}
			}
		}

		private void StartDrag(InputState istate)
		{
			_mouseStartPos = istate.GamePointerPosition;
			_startOffset = _gdScreen.MapOffset;

			_isDragging = true;
		}

		private void UpdateDrag(SAMTime gameTime, InputState istate)
		{
			var delta = istate.GamePointerPosition - _mouseStartPos;

			_gdScreen.MapOffsetX = _startOffset.X + delta.X;
			_gdScreen.MapOffsetY = _startOffset.Y + delta.Y;

			_oobForce = CalculateOOB();
		}

		private DVector CalculateOOB()
		{
			var v2 = new DVector();

			if (_boundsMap.Left < _boundsWorkingArea.Left && _boundsMap.Right  < _boundsWorkingArea.Right)  v2.X = +1;
			if (_boundsMap.Left > _boundsWorkingArea.Left && _boundsMap.Right  > _boundsWorkingArea.Right)  v2.X = -1;
			if (_boundsMap.Top  < _boundsWorkingArea.Top  && _boundsMap.Bottom < _boundsWorkingArea.Bottom) v2.Y = +1;
			if (_boundsMap.Top  > _boundsWorkingArea.Top  && _boundsMap.Bottom > _boundsWorkingArea.Bottom) v2.Y = -1;

			return v2;
		}

		private void UpdateRestDrag(SAMTime gameTime)
		{
			_gdScreen.MapOffsetX += _oobForce.X * RETURN_SPEED * gameTime.ElapsedSeconds;
			_gdScreen.MapOffsetY += _oobForce.Y * RETURN_SPEED * gameTime.ElapsedSeconds;

			var next = CalculateOOB();

			if (next.X == -1 && _oobForce.X == +1)
			{
				next.X = 0;
				_gdScreen.MapOffsetX = _boundsWorkingArea.Right - _boundsMap.Width;
			}
			if (next.X == +1 && _oobForce.X == -1)
			{
				next.X = 0;
				_gdScreen.MapOffsetX = _boundsWorkingArea.Left;
			}
			if (next.Y == -1 && _oobForce.Y == +1)
			{
				next.Y = 0;
				_gdScreen.MapOffsetY = _boundsWorkingArea.Bottom - _boundsMap.Height;
			}
			if (next.Y == +1 && _oobForce.Y == -1)
			{
				next.Y = 0;
				_gdScreen.MapOffsetY = _boundsWorkingArea.Top;
			}

			_oobForce = next;
		}

		private void EndDrag(SAMTime gameTime, InputState istate)
		{
			_oobForce = CalculateOOB();

			_isDragging = false;
		}
	}
}
