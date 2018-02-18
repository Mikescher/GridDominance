using System.Linq;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.Leveleditor;
using GridDominance.Shared.Screens.Leveleditor.Entities;
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

		private enum DMode { Nothing, MapDrag, CannonMove }

		private DMode _dragMode = DMode.Nothing;

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
			const float raster = (GDConstants.TILE_WIDTH / 2f);
			var rx = raster * FloatMath.Round(istate.GamePointerPositionOnMap.X / raster);
			var ry = raster * FloatMath.Round(istate.GamePointerPositionOnMap.Y / raster);

			_boundsWorkingArea = _gdScreen.VAdapterGame.VirtualTotalBoundingBox.AsDeflated(
				0, 
				4 * GDConstants.TILE_WIDTH, 
				_gdScreen.GDHUD.AttrPanel.IsVisible ? 4 * GDConstants.TILE_WIDTH : 0, 
				0);

			_boundsMap = FRectangle.CreateByTopLeft(
				_gdScreen.MapOffsetX,
				_gdScreen.MapOffsetY,
				_gdScreen.LevelData.Width * GDConstants.TILE_WIDTH,
				_gdScreen.LevelData.Height * GDConstants.TILE_WIDTH);

			if (_dragMode == DMode.MapDrag)
			{
				if (_gdScreen.Mode==LevelEditorMode.Mouse && istate.IsRealDown)
				{
					var delta = istate.GamePointerPosition - _mouseStartPos;
					_gdScreen.MapOffsetX = _startOffset.X + delta.X;
					_gdScreen.MapOffsetY = _startOffset.Y + delta.Y;
				}
				else
				{
					_dragMode = DMode.Nothing;
				}
			}
			else if (_dragMode == DMode.CannonMove)
			{
				if (_gdScreen.Mode == LevelEditorMode.Mouse && istate.IsRealDown && _gdScreen.Selection is CannonStub cs)
				{
					var ins = _gdScreen.CanInsertCannonStub(new FPoint(rx, ry), cs.Scale, cs);
					if (ins != null)
					{
						cs.Center = ins.Position;
					}
				}
				else
				{
					_dragMode = DMode.Nothing;
				}
			}
			else if (_dragMode == DMode.Nothing)
			{
				if (_gdScreen.Mode == LevelEditorMode.Mouse && istate.IsExclusiveJustDown)
				{
					var clickedCannon = _gdScreen.GetEntities<CannonStub>().FirstOrDefault(s => s.GetClickArea().Contains(istate.GamePointerPositionOnMap));
					if (clickedCannon != null)
					{
						istate.Swallow(InputConsumer.GameBackground);
						_gdScreen.SelectStub(clickedCannon);
						_mouseStartPos = istate.GamePointerPosition;
						_startOffset = _gdScreen.MapOffset;
						_dragMode = DMode.CannonMove;
					}
					else
					{
						istate.Swallow(InputConsumer.GameBackground);
						_gdScreen.SelectStub(null);
						_mouseStartPos = istate.GamePointerPosition;
						_startOffset = _gdScreen.MapOffset;
						_dragMode = DMode.MapDrag;
					}

				}
			}

			_oobForce = CalculateOOB();
			if (!_oobForce.IsZero() && _dragMode != DMode.MapDrag)
			{
				UpdateMapRestDrag(gameTime);
			}
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

		private void UpdateMapRestDrag(SAMTime gameTime)
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

		public void ManualStartCannonMove(InputState istate)
		{
			_gdScreen.SetMode(LevelEditorMode.Mouse);
			_mouseStartPos = istate.GamePointerPosition;
			_startOffset = _gdScreen.MapOffset;
			_dragMode = DMode.CannonMove;
		}
	}
}
