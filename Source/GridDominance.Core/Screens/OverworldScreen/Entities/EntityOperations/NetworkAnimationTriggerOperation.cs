using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.UpdateAgents.Impl;

namespace GridDominance.Shared.Screens.OverworldScreen.Entities.EntityOperations
{
	class NetworkAnimationTriggerOperation : CyclicOperation<OverworldNode_MP>
	{
		public const float CYCLE_TIME      = 1.75f;
		public const float TRANSITION_TIME = 1.25f;
		public const float INITIAL_DELAY   = 2.00f;

		public readonly int[] _shuffle = {0, 1, 2, 3, 4};

		public override string Name => "NetworkAnimationTrigger";

		public NetworkAnimationTriggerOperation() : base(CYCLE_TIME, false)
		{
			
		}

		protected override void OnCycle(OverworldNode_MP node, int counter)
		{
			if (node.HasActiveOperation("NetworkAnimationVertex")) return;

			float w1 = FloatMath.GetRangedRandom(0.75f, 2.25f);
			float w2 = FloatMath.GetRangedRandom(0.75f, 2.25f);
			float w3 = FloatMath.GetRangedRandom(0.75f, 2.25f);
			float w4 = FloatMath.GetRangedRandom(0.75f, 2.25f);
			float w5 = FloatMath.GetRangedRandom(0.75f, 2.25f);

			var ws = w1 + w2 + w3 + w4 + w5;

			var br = FloatMath.GetRangedRandom(FloatMath.TAU);

			float a1 = br + ((w1                    ) / ws) * FloatMath.TAU;
			float a2 = br + ((w1 + w2               ) / ws) * FloatMath.TAU;
			float a3 = br + ((w1 + w2 + w3          ) / ws) * FloatMath.TAU;
			float a4 = br + ((w1 + w2 + w3 + w4     ) / ws) * FloatMath.TAU;
			float a5 = br + ((w1 + w2 + w3 + w4 + w5) / ws) * FloatMath.TAU;

			ShuffleIndizies();
			
			node.AddOperation(new NetworkAnimationVertexOperation(_shuffle[0], node.VertexRotations[_shuffle[0]], a1));
			node.AddOperation(new NetworkAnimationVertexOperation(_shuffle[1], node.VertexRotations[_shuffle[1]], a2));
			node.AddOperation(new NetworkAnimationVertexOperation(_shuffle[2], node.VertexRotations[_shuffle[2]], a3));
			node.AddOperation(new NetworkAnimationVertexOperation(_shuffle[3], node.VertexRotations[_shuffle[3]], a4));
			node.AddOperation(new NetworkAnimationVertexOperation(_shuffle[4], node.VertexRotations[_shuffle[4]], a5));
		}

		private void ShuffleIndizies() // Fisher-Yates shuffle
		{
			int n = 5;
			while (n > 1)
			{
				n--;
				FloatMath.Swap(ref _shuffle[FloatMath.GetRangedIntRandom(n + 1)], ref _shuffle[n]);
			}
		}
	}
}
