using GridDominance.Shared.Screens.NormalGameScreen.Entities;
using MonoSAMFramework.Portable.GameMath.Geometry;

namespace GridDominance.Shared.Screens.NormalGameScreen.Background
{
	public interface IGDGridBackground
	{
		void RegisterSpawn(Cannon cannon, FCircle circle);

		void RegisterBlockedLine(FPoint start, FPoint end);
		void RegisterBlockedCircle(FCircle circle);
		void RegisterBlockedBlock(FRectangle block, float rotation);
	}
}
