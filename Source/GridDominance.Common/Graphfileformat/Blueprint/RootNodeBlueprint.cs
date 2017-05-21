using System;
using System.Collections.Generic;

namespace GridDominance.Graphfileformat.Blueprint
{
	public struct RootNodeBlueprint : INodeBlueprint
	{
		public readonly float X;
		public readonly float Y;
		public readonly List<PipeBlueprint> OutgoingPipes;
		public readonly Guid WorldID;
		public readonly string WorldName;

		List<PipeBlueprint> INodeBlueprint.Pipes => OutgoingPipes;
		Guid INodeBlueprint.ConnectionID => WorldID;
		float INodeBlueprint.X => X;
		float INodeBlueprint.Y => Y;

		public RootNodeBlueprint(float x, float y, Guid g, string n)
		{
			X = x;
			Y = y;
			OutgoingPipes = new List<PipeBlueprint>();
			WorldID = g;
			WorldName = n;
		}

	}
}
