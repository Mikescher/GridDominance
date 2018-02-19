using System;
using System.Collections.Generic;
using GridDominance.Graphfileformat.Blueprint;
using MonoSAMFramework.Portable.GameMath.Geometry;

namespace GridDominance.Shared.Screens.WorldMapScreen.Entities
{
	public interface IWorldNode
	{
		FPoint Position { get; }
		IEnumerable<IWorldNode> NextLinkedNodes { get; }
		Guid ConnectionID { get; }

		bool NodeEnabled { get; set; }

		void CreatePipe(IWorldNode target, PipeBlueprint.Orientation orientation);
		bool HasAnyCompleted();
	}
}
