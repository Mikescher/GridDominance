using System;
using System.Collections.Generic;

namespace GridDominance.Graphfileformat.Blueprint
{
	public struct WarpNodeBlueprint : INodeBlueprint
	{
		public readonly float X;
		public readonly float Y;
		public readonly Guid TargetWorld;
		private readonly List<PipeBlueprint> _outgoingPipes;

		List<PipeBlueprint> INodeBlueprint.Pipes => _outgoingPipes;
		Guid INodeBlueprint.ConnectionID => TargetWorld;
		float INodeBlueprint.X => X;
		float INodeBlueprint.Y => Y;

		public WarpNodeBlueprint(float x, float y, Guid g)
		{
			_outgoingPipes = new List<PipeBlueprint>();

			X = x;
			Y = y;
			TargetWorld = g;
		}

	}
}
