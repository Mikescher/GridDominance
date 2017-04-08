using System;
using System.Collections.Generic;

namespace GridDominance.Graphfileformat.Parser
{
	public struct WGNode
	{
		public readonly float X;
		public readonly float Y;
		public readonly Guid LevelID;

		public readonly List<WGPipe> OutgoingPipes;

		public WGNode(float x, float y, Guid id)
		{
			X = x;
			Y = y;
			LevelID = id;
			OutgoingPipes = new List<WGPipe>();
		}
	}
}
