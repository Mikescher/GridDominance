using System;

namespace GridDominance.Graphfileformat.Blueprint
{
	public struct PipeBlueprint
	{
		public enum Orientation
		{
			Auto = 0,
			Clockwise = 0x10,
			Counterclockwise = 0x20,
		}

		public readonly Guid Target;
		public readonly Orientation PipeOrientation;

		public PipeBlueprint(Guid t, Orientation o)
		{
			Target = t;
			PipeOrientation = o;
		}
	}
}
