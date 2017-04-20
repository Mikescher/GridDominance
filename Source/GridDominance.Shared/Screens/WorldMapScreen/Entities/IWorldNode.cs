using System;
using System.Collections.Generic;
using System.Text;
using GridDominance.Graphfileformat.Blueprint;
using Microsoft.Xna.Framework;

namespace GridDominance.Shared.Screens.WorldMapScreen.Entities
{
	public interface IWorldNode
	{
		Vector2 Position { get; }
		IEnumerable<IWorldNode> NextLinkedNodes { get; }

		void CreatePipe(LevelNode target, PipeBlueprint.Orientation orientation);
	}
}
