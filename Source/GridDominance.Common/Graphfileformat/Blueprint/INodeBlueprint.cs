using System.Collections.Generic;

namespace GridDominance.Graphfileformat.Blueprint
{
	public interface INodeBlueprint
	{
		float X { get; }
		float Y { get; }
		List<PipeBlueprint> Pipes { get; }
	}
}
