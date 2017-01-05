using System.Collections.Generic;

namespace MonoSAMFramework.Portable.DebugTools
{
	interface IDebugTextDisplayLineProvider
	{
		IEnumerable<DebugTextDisplayLine> GetLines();

		void Update();
		bool RemoveZombies();
	}
}
