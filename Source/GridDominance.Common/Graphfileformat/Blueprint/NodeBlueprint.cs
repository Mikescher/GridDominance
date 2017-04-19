using System;
using System.Collections.Generic;

namespace GridDominance.Graphfileformat.Blueprint
{
	public struct NodeBlueprint : INodeBlueprint
	{
		public readonly float X;
		public readonly float Y;
		public readonly Guid LevelID;

		public readonly List<PipeBlueprint> OutgoingPipes;

		List<PipeBlueprint> INodeBlueprint.Pipes => OutgoingPipes;
		float INodeBlueprint.X => X;
		float INodeBlueprint.Y => Y;

		public NodeBlueprint(float x, float y, Guid id)
		{
			X = x;
			Y = y;
			LevelID = id;
			OutgoingPipes = new List<PipeBlueprint>();
		}

	}
}
