using System;

namespace GridDominance.Graphfileformat.Blueprint
{
	public struct PipeBlueprint
	{
		public enum Orientation
		{
			Auto = 0,
			Clockwise = 0x11,
			Counterclockwise = 0x22,
			Direct = 0x33,
		}

		public readonly Guid Target;
		public readonly Orientation PipeOrientation;
		public readonly byte Priority; // lowest priority first

		public PipeBlueprint(Guid t, Orientation o, byte p)
		{
			Target = t;
			PipeOrientation = o;
			Priority = p;
		}
	}
}
