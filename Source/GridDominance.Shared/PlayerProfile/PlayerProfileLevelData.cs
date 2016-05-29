using System;
using System.Collections.Generic;
using System.Text;
using GridDominance.Shared.Screens.ScreenGame.Fractions;

namespace GridDominance.Shared.PlayerProfile
{
	public class PlayerProfileLevelData
	{
		public HashSet<FractionDifficulty> CompletDifficulties = new HashSet<FractionDifficulty>();

		public PlayerProfileLevelData()
		{
			
		}

		public bool HasCompleted(FractionDifficulty d)
		{
			return CompletDifficulties.Contains(d);
		}

		public void SetCompleted(FractionDifficulty d)
		{
			CompletDifficulties.Add(d);
		}
	}
}
