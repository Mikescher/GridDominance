using System;
using System.Collections.Generic;

namespace GridDominance.Graphfileformat.Blueprint
{
	public interface INodeBlueprint
	{
		Guid ConnectionID { get; }

		float X { get; }
		float Y { get; }
		List<PipeBlueprint> Pipes { get; }
	}
}
