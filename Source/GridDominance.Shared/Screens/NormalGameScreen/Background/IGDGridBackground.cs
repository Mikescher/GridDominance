using GridDominance.Shared.Screens.NormalGameScreen.Entities;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.GameMath.Geometry;

namespace GridDominance.Shared.Screens.NormalGameScreen.Background
{
	public interface IGDGridBackground
	{
		int ParticleCount { get; }

		void RegisterBlockedSpawn(Cannon cannon, int i, int i1);
		void RegisterBlockedLine(Vector2 start, Vector2 end);
		void RegisterBlockedCircle(Vector2 pos, float r);

		void SpawnParticles(Fraction fraction, int spawnX, int spawnY);
		void RegisterBlockedBlock(FRectangle block);
	}
}
