using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.UpdateAgents.Impl;

namespace GridDominance.Shared.Screens.OverworldScreen.Entities.EntityOperations
{
	class NetworkAnimationVertexOperation : FixTimeOperation<OverworldNode_MP>
	{
		private readonly int _index;
		private readonly float _angleStart;
		private readonly float _angleEnd;

		public override string Name => "NetworkAnimationVertex";

		public NetworkAnimationVertexOperation(int index, float start, float end) : base(NetworkAnimationTriggerOperation.TRANSITION_TIME)
		{
			_index = index;
			_angleStart = start;
			_angleEnd = FloatMath.GetNearestAngleRepresentation(start, end);
		}

		protected override void OnProgress(OverworldNode_MP node, float progress, SAMTime gameTime, InputState istate)
		{
			node.VertexRotations[_index] = _angleStart + FloatMath.FunctionCustomBounce(progress) * (_angleEnd - _angleStart);
		}

		protected override void OnEnd(OverworldNode_MP node)
		{
			node.VertexRotations[_index] = FloatMath.NormalizeAngle(_angleEnd);
		}
	}
}
