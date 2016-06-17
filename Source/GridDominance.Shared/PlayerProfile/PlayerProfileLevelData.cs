using System.Collections.Generic;
using GridDominance.Shared.Screens.ScreenGame.Fractions;
using MonoSAMFramework.Portable.Persistance;
using MonoSAMFramework.Portable.Persistance.DataFile;

namespace GridDominance.Shared.PlayerProfile
{
	public class PlayerProfileLevelData : BaseDataFile
	{
		private HashSet<FractionDifficulty> completedDifficulties;

		public PlayerProfileLevelData()
		{
			completedDifficulties = new HashSet<FractionDifficulty>();
		}

		public bool HasCompleted(FractionDifficulty d)
		{
			return completedDifficulties.Contains(d);
		}

		public void SetCompleted(FractionDifficulty d)
		{
			completedDifficulties.Add(d);
		}

		protected override void Configure()
		{
			RegisterConstructor(() => new PlayerProfileLevelData());

			RegisterPropertyEnumSet<PlayerProfileLevelData, FractionDifficulty>(SemVersion.VERSION_1_0_0, "cdata", o => o.completedDifficulties, (o, v) => o.completedDifficulties = v);
		}

		protected override string GetTypeName()
		{
			return "PLAYER_PROFILE_LEVEL_DATA";
		}
	}
}
