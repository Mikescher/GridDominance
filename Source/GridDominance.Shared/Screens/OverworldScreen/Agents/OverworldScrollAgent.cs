using MonoSAMFramework.Portable.Screens.Agents;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Input;
using GridDominance.Shared.Screens.OverworldScreen.Entities;
using GridDominance.Shared.Resources;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using GridDominance.Graphfileformat.Blueprint;
using System;

namespace GridDominance.Shared.Screens.OverworldScreen.Agents
{
	public class OverworldScrollAgent : GameScreenAgent
	{
		private const float MIN_DRAG   = 0.2f * GDConstants.TILE_WIDTH;
		
		private const float PADDING_X  = 2.5f * GDConstants.TILE_WIDTH;
		private const float POSITION_Y = 6.5f * GDConstants.TILE_WIDTH;

		private const float DIST_X     = 5.0f * GDConstants.TILE_WIDTH;
		private const float MIN_DIST_X = 3.5f * GDConstants.TILE_WIDTH;

		private const float FORCE      = 75f;  // [(m/s²)/m] = [1/s²]
		private const float DRAG       = 0.7f; // %
		private const float MIN_SPEED  = 32f;  // %

		public const float CLICK_CANCEL_TIME = 0.5f;
		public const float CLICK_CANCEL_DIST = 0.2f * GDConstants.TILE_WIDTH;

		private enum DragMode { Global, Node }

		public override bool Alive => true;

		private readonly OverworldNode[] _nodes;
		private readonly AdaptionFloat[] _values;

		private float dragStartTime = 0f;
		private bool isDragging = false;
		private bool isActiveDragging = false;
		private float mouseStartPos;
		private float offsetStart;
		private int dragAnchor;
		private DragMode dragMode;

		private bool _sleep = true;
		
		public OverworldScrollAgent(GDOverworldScreen scrn, OverworldNode[] nodes) : base(scrn)
		{
			_nodes = nodes;
			_values = new AdaptionFloat[_nodes.Length];

			int focus = 0;
			switch (GDConstants.FLAVOR)
			{
				case GDFlavor.FREE:
				case GDFlavor.IAB:
					for (int i = 0; i < nodes.Length; i++)
					{
						if (!nodes[i].IsNodeEnabled) break;
						focus = i;
					}
					break;

				default:
				case GDFlavor.FULL:
				case GDFlavor.FULL_NOMP:
                    focus = 1;
					break;
			}

			var offset0 = GDConstants.VIEW_WIDTH/2f - focus * DIST_X;
			for (int i = 0; i < _nodes.Length; i++)
			{
				_values[i] = new AdaptionFloat(offset0 + i * DIST_X, FORCE, DRAG, MIN_SPEED);
				_nodes[i].NodePos = new FPoint(_values[i].Value, POSITION_Y);
			}

			CleanUpPositions(true);
		}

		public void ScrollTo(GraphBlueprint bp)
		{
			int focus = -1;
			for (int i = 0; i < _nodes.Length; i++)
			{
				if (_nodes[i].ContentID == bp.ID) focus = i;
			}
			if (focus == -1) return;

			var offset0 = GDConstants.VIEW_WIDTH / 2f - focus * DIST_X;
			for (int i = 0; i < _nodes.Length; i++)
			{
				_values[i] = new AdaptionFloat(offset0 + i * DIST_X, FORCE, DRAG, MIN_SPEED);
				_nodes[i].NodePos = new FPoint(_values[i].Value, POSITION_Y);
			}

			CleanUpPositions(true);
		}

		public void CopyState(OverworldScrollAgent other)
		{
			if (this._nodes.Length != other._nodes.Length) return;

			for (int i = 0; i < _nodes.Length; i++)
			{
				_values[i].SetFull(other._values[i]);
				_nodes[i].NodePos = other._nodes[i].NodePos;
			}

			dragStartTime = other.dragStartTime;
			isDragging = other.isDragging;
			isActiveDragging = other.isActiveDragging;
			mouseStartPos = other.mouseStartPos;
			offsetStart = other.offsetStart;
			dragAnchor = other.dragAnchor;
			dragMode = other.dragMode;
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
					StartDrag(gameTime, istate);
				}
				else if (istate.IsRealJustDown && istate.SwallowConsumer == InputConsumer.GameEntity)
				{
					StartDrag(gameTime, istate);
				}
				else
				{
					UpdateIdle(gameTime, istate);
				}
			}
		}

		private void StartDrag(SAMTime gameTime, InputState istate)
		{
			for (int i = 0; i < _nodes.Length; i++)
			{
				if (_nodes[i].DrawingBoundingRect.Contains(istate.GamePointerPositionOnMap))
				{
					isDragging = true;
					isActiveDragging = false;
					mouseStartPos = istate.GamePointerPositionOnMap.X;
					offsetStart = _nodes[i].NodePos.X;
					dragAnchor = i;
					dragMode = DragMode.Node;
					dragStartTime = gameTime.TotalElapsedSeconds;

					return;
				}
			}

			isDragging = true;
			mouseStartPos = istate.GamePointerPositionOnMap.X;
			offsetStart = _nodes[0].Position.X;
			dragAnchor = -1;
			dragMode = DragMode.Global;
			dragStartTime = gameTime.TotalElapsedSeconds;
		}

		private void UpdateDrag(SAMTime gameTime, InputState istate)
		{
			if (dragMode == DragMode.Node)
			{
				var delta = (istate.GamePointerPositionOnMap.X - mouseStartPos);

				isActiveDragging |= FloatMath.Abs(delta) > MIN_DRAG;
				if (!isActiveDragging) return;

				_values[dragAnchor].SetDirect(offsetStart + delta);

				for (int i = dragAnchor-1; i >= 0; i--)
				{
					_values[i].Set(_values[i+1].TargetValue - DIST_X);
					_values[i].ValueMin = float.MinValue;
					_values[i].ValueMax = _values[i + 1].Value - MIN_DIST_X;
				}
				for (int i = dragAnchor+1; i < _nodes.Length; i++)
				{
					_values[i].Set(_values[i - 1].TargetValue + DIST_X);
					_values[i].ValueMin = _values[i - 1].Value + MIN_DIST_X;
					_values[i].ValueMax = float.MaxValue;
				}

				if (gameTime.TotalElapsedSeconds - dragStartTime > CLICK_CANCEL_TIME || FloatMath.Abs(delta) > CLICK_CANCEL_DIST)
				{
					_nodes[dragAnchor].CancelClick();
				}

				dragStartTime = gameTime.TotalElapsedSeconds;
			}
			else if (dragMode == DragMode.Global)
			{
				var delta = (istate.GamePointerPositionOnMap.X - mouseStartPos);

				isActiveDragging |= FloatMath.Abs(delta) > MIN_DRAG;
				if (!isActiveDragging) return;
				
				for (int i = 0; i < _nodes.Length; i++)
				{
					_values[i].Set(offsetStart + delta + i * DIST_X);
					_values[i].ValueMin = (i == 0)               ? float.MinValue : _values[i - 1].Value + MIN_DIST_X;
					_values[i].ValueMax = (i+1 == _nodes.Length) ? float.MaxValue : _values[i + 1].Value - MIN_DIST_X;
				}
			}
			
			UpdateOffsets(gameTime, istate);
			_sleep = false;
		}

		private void EndDrag()
		{
			isDragging = false;

			for (int i = 0; i < _nodes.Length; i++)
			{
				_values[i].ValueMin = float.MinValue;
				_values[i].ValueMax = float.MaxValue;
			}

			CleanUpPositions(false);
		}

		private void CleanUpPositions(bool direct)
		{
			if (_values[0].TargetValue > PADDING_X)
			{
				for (int i = 0; i < _nodes.Length; i++)
				{
					_values[i].Set(PADDING_X + i * DIST_X, direct);
				}
				_sleep = false;
			}
			else if (_values[_values.Length - 1].TargetValue < GDConstants.VIEW_WIDTH - PADDING_X)
			{
				var n0 = GDConstants.VIEW_WIDTH - PADDING_X - ((_values.Length - 1) * DIST_X);

				for (int i = 0; i < _nodes.Length; i++)
				{
					_values[i].Set(n0 + i * DIST_X, direct);
				}
				_sleep = false;
			}

			if (direct)
			{
				for (int i = 0; i < _values.Length; i++)
				{
					_nodes[i].NodePos = new FPoint(_values[i].Value, POSITION_Y);
				}
				_sleep = true;
			}
		}

		private void UpdateIdle(SAMTime gameTime, InputState istate)
		{
			if (_sleep) return;
			
			UpdateOffsets(gameTime, istate);
		}

		private void UpdateOffsets(SAMTime gameTime, InputState istate)
		{
			for (int i = 0; i < _values.Length; i++)
			{
				if (_nodes[i].HasActiveOperation("OverworldNode::Shake")) continue;
				_values[i].Update(gameTime, istate);
			}

			_sleep = true;
			for (int i = 0; i < _values.Length; i++)
			{
				if (FloatMath.EpsilonEquals(_values[i].TargetValue, _values[i].Value, 0.01f) && FloatMath.EpsilonEquals(_values[i].Value, _nodes[i].NodePos.X, 0.01f))
					continue;
				
				_sleep = false;

				if (_nodes[i].HasActiveOperation("OverworldNode::Shake")) continue;
					
				_nodes[i].NodePos = new FPoint(_values[i].Value, POSITION_Y);

#if DEBUG
				_nodes[i].TargetNodePos = new FPoint(_values[i].TargetValue, POSITION_Y);
#endif
			}
		}
	}
}
