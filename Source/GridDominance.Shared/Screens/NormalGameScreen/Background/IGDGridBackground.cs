using GridDominance.Shared.Screens.NormalGameScreen.Entities;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.GameMath.Geometry;

namespace GridDominance.Shared.Screens.NormalGameScreen.Background
{
	public interface IGDGridBackground
	{
		void RegisterSpawn(Cannon cannon, FCircle circle);

		void RegisterBlockedLine(Vector2 start, Vector2 end);
		void RegisterBlockedCircle(FCircle circle);
		void RegisterBlockedBlock(FRectangle block);
	}
}
