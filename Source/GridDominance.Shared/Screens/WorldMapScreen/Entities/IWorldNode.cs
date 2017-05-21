using System;
using System.Collections.Generic;
using GridDominance.Graphfileformat.Blueprint;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.GameMath.Geometry;

namespace GridDominance.Shared.Screens.WorldMapScreen.Entities
{
	public interface IWorldNode
	{
		Vector2 Position { get; }
		IEnumerable<IWorldNode> NextLinkedNodes { get; }
		Guid ConnectionID { get; }

		bool NodeEnabled { get; set; }

		void CreatePipe(IWorldNode target, PipeBlueprint.Orientation orientation);
		bool HasAnyCompleted();
	}
}
