using System.Collections.Generic;
using MonoSAMFramework.Portable.GameMath.Geometry;

namespace GridDominance.Shared.Screens.LevelEditorScreen.Entities
{
	public interface ILeveleditorStub
	{
		IEnumerable<SingleAttrOption> AttrOptions { get; }

		IFShape GetClickArea();

		void Kill();

		bool CollidesWith(CannonStub other);
		bool CollidesWith(ObstacleStub other);
		bool CollidesWith(WallStub other);
		bool CollidesWith(PortalStub other);
	}
}
