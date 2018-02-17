using System.Collections.Generic;
using MonoSAMFramework.Portable.GameMath.Geometry;

namespace GridDominance.Shared.Screens.Leveleditor.Entities
{
	public interface ILeveleditorStub
	{
		IEnumerable<SingleAttrOption> AttrOptions { get; }

		void Kill();
	}
}
