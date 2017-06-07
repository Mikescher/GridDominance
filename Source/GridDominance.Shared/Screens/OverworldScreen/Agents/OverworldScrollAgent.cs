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
		private const float POSITION_Y = 6.5f * GDConstants.TILE_WIDTH;

		private const float DIST_X     = 5.0f * GDConstants.TILE_WIDTH;
		private const float MIN_DIST_X = 3.5f * GDConstants.TILE_WIDTH;

		private enum DragMode { Global, Node }

		public override bool Alive => true;

		private readonly OverworldNode[] _nodes;
		private readonly AccelerationFloat[] _values;

		private bool isDragging = false;
		private FPoint mouseStartPos;
		private FPoint offsetStart;
		private OverworldNode dragAnchor;
		private DragMode dragMode;

		public OverworldScrollAgent(GDOverworldScreen scrn, OverworldNode[] nodes) : base(scrn)
		{
			_nodes = nodes;
			_values = new AccelerationFloat[_nodes.Length];
			for (int i = 0; i < _nodes.Length; i++)
			{
				_values[i] = new AccelerationFloat();//TODO stuff
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

		}

		private void UpdateDrag(SAMTime gameTime, InputState istate)
		{

		}

		private void EndDrag()
		{

		}

		private void UpdateIdle(SAMTime gameTime, InputState istate)
		{

		}
	}
}
