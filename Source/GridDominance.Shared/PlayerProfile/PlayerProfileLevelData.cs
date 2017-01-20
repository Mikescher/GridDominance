using System;
using System.Collections.Generic;
using System.Linq;
using GridDominance.Shared.Screens.ScreenGame.Fractions;
using MonoSAMFramework.Portable.Persistance;
using MonoSAMFramework.Portable.Persistance.DataFile;
using MonoSAMFramework.Portable.Persistance.DataFile.PrimitiveWrapper;

namespace GridDominance.Shared.PlayerProfile
{
	public class PlayerProfileLevelData : BaseDataFile
	{
		private HashSet<FractionDifficulty> completedDifficulties;
		private Dictionary<FractionDifficulty, DataFileFloatWrapper> bestTimes;

		public PlayerProfileLevelData()
		{
			completedDifficulties = new HashSet<FractionDifficulty>();
			bestTimes = new Dictionary<FractionDifficulty, DataFileFloatWrapper>();
		}

		public int TotalPoints => completedDifficulties.Sum(FractionDifficultyHelper.GetScore);

		public bool HasCompleted(FractionDifficulty d)
		{
			return completedDifficulties.Contains(d);
		}

		public void SetCompletedTrue(FractionDifficulty d, float time)
		{
			completedDifficulties.Add(d);
			if (!bestTimes.ContainsKey(d) || bestTimes[d].Value > time) bestTimes[d] = DataFileFloatWrapper.Create(time);
		}

		public void SetCompletedFalse(FractionDifficulty d)
		{
			completedDifficulties.Remove(d);
			bestTimes.Remove(d);
		}

		protected override void Configure()
		{
			RegisterConstructor(() => new PlayerProfileLevelData());

			RegisterPropertyEnumSet<PlayerProfileLevelData, FractionDifficulty>(SemVersion.VERSION_1_0_0, "competion_data", o => o.completedDifficulties, (o, v) => o.completedDifficulties = v);
			RegisterPropertyEnumDictionary<PlayerProfileLevelData, FractionDifficulty, DataFileFloatWrapper>(SemVersion.VERSION_1_0_0, "time_data", DataFileFloatWrapper.Create, o => o.bestTimes, (o, v) => o.bestTimes = v);
		}

		protected override string GetTypeName()
		{
			return "PLAYER_PROFILE_LEVEL_DATA";
		}
	}
}
