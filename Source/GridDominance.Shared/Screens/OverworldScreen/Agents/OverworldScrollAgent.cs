using MonoSAMFramework.Portable.Screens.Agents;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Input;
using GridDominance.Shared.Screens.OverworldScreen.Entities;
using GridDominance.Shared.Resources;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.GameMath;

namespace GridDominance.Shared.Screens.OverworldScreen.Agents
{
	class OverworldScrollAgent : GameScreenAgent //TODO OverworldScrollAgent
	{
		private const float POSITION_X = 1.5f * GDConstants.TILE_WIDTH;
		private const float POSITION_Y = 6.5f * GDConstants.TILE_WIDTH;

		private const float DIST_X     = 5.0f * GDConstants.TILE_WIDTH;
		private const float MIN_DIST_X = 3.5f * GDConstants.TILE_WIDTH;

		private const float FORCE      = 10f; // [(m/s²)/m] = [1/s²]
		private const float DRAG       = 0.8f; // %

		private enum DragMode { Global, Node }

		public override bool Alive => true;

		private readonly OverworldNode[] _nodes;
		private readonly AdaptionFloat[] _values;

		private bool isDragging = false;
		private float mouseStartPos;
		private float offsetStart;
		private int dragAnchor;
		private DragMode dragMode;

		public OverworldScrollAgent(GDOverworldScreen scrn, OverworldNode[] nodes) : base(scrn)
		{
			_nodes = nodes;
			_values = new AdaptionFloat[_nodes.Length];
			for (int i = 0; i < _nodes.Length; i++)
			{
				_values[i] = new AdaptionFloat(POSITION_X + i * DIST_X, FORCE, DRAG);
			}
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
				else if (istate.IsRealJustDown && istate.SwallowConsumer == InputConsumer.GameEntity)
				{
					StartDrag(istate);
				}
				else
				{
					UpdateIdle(gameTime, istate);
				}
			}
		}

		private void StartDrag(InputState istate)
		{
			for (int i = 0; i < _nodes.Length; i++)
			{
				if (_nodes[i].DrawingBoundingRect.Contains(istate.GamePointerPositionOnMap))
				{
					isDragging = true;
					mouseStartPos = istate.GamePointerPositionOnMap.X;
					offsetStart = _nodes[i].NodePos.X;
					dragAnchor = i;
					dragMode = DragMode.Node;

					return;
				}
			}

			isDragging = true;
			mouseStartPos = istate.GamePointerPositionOnMap.X;
			offsetStart = _nodes[0].Position.X;
			dragAnchor = -1;
			dragMode = DragMode.Global;
		}

		private void UpdateDrag(SAMTime gameTime, InputState istate)
		{
			if (dragMode == DragMode.Node)
			{
				_values[dragAnchor].SetDirect(offsetStart + (istate.GamePointerPositionOnMap.X - mouseStartPos));

				for (int i = 0; i < dragAnchor; i++)
				{
					_values[i].Set(_values[i+1].Value - DIST_X);
					_values[i].ValueMin = float.MinValue;
					_values[i].ValueMax = _values[i + 1].Value - MIN_DIST_X;
				}
				for (int i = dragAnchor+1; i < _nodes.Length; i++)
				{
					_values[i].Set(_values[i - 1].Value + DIST_X);
					_values[i].ValueMin = _values[i - 1].Value + MIN_DIST_X;
					_values[i].ValueMax = float.MaxValue;
				}
			}
			else if (dragMode == DragMode.Global)
			{
				var delta = (istate.GamePointerPositionOnMap.X - mouseStartPos);

				for (int i = 0; i < _nodes.Length; i++)
				{
					_values[i].Set(offsetStart + delta + i * DIST_X);
					_values[i].ValueMin = (i == 0)               ? float.MinValue : _values[i - 1].Value + MIN_DIST_X;
					_values[i].ValueMax = (i+1 == _nodes.Length) ? float.MaxValue : _values[i + 1].Value - MIN_DIST_X;
				}
			}
			
			UpdateOffsets(gameTime, istate);
		}

		private void EndDrag()
		{
			isDragging = false;
		}

		private void UpdateIdle(SAMTime gameTime, InputState istate)
		{
			UpdateOffsets(gameTime, istate);
		}

		private void UpdateOffsets(SAMTime gameTime, InputState istate)
		{
			for (int i = 0; i < _values.Length; i++) _values[i].Update(gameTime, istate);

			for (int i = 0; i < _values.Length; i++)
			{
				if (!FloatMath.EpsilonEquals(_nodes[i].NodePos.X, _values[i].Value, 0.01f))
				{
					_nodes[i].NodePos.X = _values[i].Value;
					_nodes[i].NodePos.Y = POSITION_Y;
				}
			}
		}
	}
}
