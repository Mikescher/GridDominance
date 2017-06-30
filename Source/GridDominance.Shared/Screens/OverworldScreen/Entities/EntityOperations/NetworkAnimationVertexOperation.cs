using System;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Entities.Operation;

namespace GridDominance.Shared.Screens.OverworldScreen.Entities.EntityOperations
{
	class NetworkAnimationVertexOperation : GameEntityOperation<OverworldNode_MP>
	{
		private readonly int _index;
		private readonly float _angleStart;
		private readonly float _angleEnd;

		public NetworkAnimationVertexOperation(int index, float start, float end) : base("NetworkAnimationVertex", NetworkAnimationTriggerOperation.TRANSITION_TIME)
		{
			_index = index;
			_angleStart = start;
			_angleEnd = FloatMath.GetNearestAngleRepresentation(start, end);
		}

		protected override void OnStart(OverworldNode_MP node)
		{
			
		}

		protected override void OnProgress(OverworldNode_MP node, float progress, SAMTime gameTime, InputState istate)
		{
			node.VertexRotations[_index] = _angleStart + FloatMath.FunctionCustomBounce(progress) * (_angleEnd - _angleStart);
		}

		protected override void OnEnd(OverworldNode_MP node)
		{
			node.VertexRotations[_index] = FloatMath.NormalizeAngle(_angleEnd);
		}

		protected override void OnAbort(OverworldNode_MP entity)
		{
			//
		}
	}
}
