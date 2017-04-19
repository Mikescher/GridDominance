using System.Collections.Generic;

namespace GridDominance.Graphfileformat.Blueprint
{
	public struct RootNodeBlueprint : INodeBlueprint
	{
		public readonly float X;
		public readonly float Y;

		public readonly List<PipeBlueprint> OutgoingPipes;

		List<PipeBlueprint> INodeBlueprint.Pipes => OutgoingPipes;
		float INodeBlueprint.X => X;
		float INodeBlueprint.Y => Y;

		public RootNodeBlueprint(float x, float y)
		{
			X = x;
			Y = y;
			OutgoingPipes = new List<PipeBlueprint>();
		}

	}
}
