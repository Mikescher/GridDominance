using System;

namespace GridDominance.Graphfileformat.Parser
{
	public struct WGPipe
	{
		public enum Orientation
		{
			Auto = 0,
			Clockwise = 0x10,
			Counterclockwise = 0x20,
		}

		public readonly Guid Target;
		public readonly Orientation PipeOrientation;

		public WGPipe(Guid t, Orientation o)
		{
			Target = t;
			PipeOrientation = o;
		}
	}
}
