using System;
using System.Collections.Generic;
using System.Text;
using GridDominance.Shared.Screens.ScreenGame.Entities;
using GridDominance.Shared.Screens.ScreenGame.Fractions;

namespace GridDominance.Shared.Screens.ScreenGame.Background
{
	interface IGDGridBackground
	{
		int ParticleCount { get; }
		void RegisterBlockedSpawn(Cannon cannon, int i, int i1);
		void DeregisterBlockedSpawn(Cannon cannon, int spawnX, int spawnY);
		void SpawnParticles(Fraction fraction, int spawnX, int spawnY);
	}
}
