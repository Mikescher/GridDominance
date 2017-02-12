using System;
using System.Collections.Generic;
using System.Linq;
using GridDominance.Shared.Screens.ScreenGame.Fractions;
using MonoSAMFramework.Portable.Persistance;
using MonoSAMFramework.Portable.Persistance.DataFile;
using MonoSAMFramework.Portable.Persistance.DataFile.PrimitiveWrapper;

namespace GridDominance.Shared.SaveData
{
	public class PlayerProfileLevelData : BaseDataFile
	{
		public Dictionary<FractionDifficulty, DataFileIntWrapper> BestTimes;

		public PlayerProfileLevelData()
		{
			BestTimes = new Dictionary<FractionDifficulty, DataFileIntWrapper>();
		}

		public int TotalPoints => BestTimes.Select(p => p.Key).Sum(FractionDifficultyHelper.GetScore);
		public int CompletionCount => BestTimes.Count;

		public bool HasCompleted(FractionDifficulty d)
		{
			return BestTimes.ContainsKey(d);
		}

		internal void SetBestTime(FractionDifficulty d, int? time)
		{
			if (time == null)
			{
				BestTimes.Remove(d);
			}
			else
			{
				BestTimes[d] = DataFileIntWrapper.Create(time.Value);
			}
		}

		protected override void Configure()
		{
			RegisterConstructor(() => new PlayerProfileLevelData());

			RegisterPropertyEnumDictionary<PlayerProfileLevelData, FractionDifficulty, DataFileIntWrapper>(SemVersion.VERSION_1_0_0, "data", DataFileIntWrapper.Create, o => o.BestTimes, (o, v) => o.BestTimes = v);
		}

		protected override string GetTypeName()
		{
			return "PLAYER_PROFILE_LEVEL_DATA";
		}

		public string GetTimeString(FractionDifficulty d)
		{
			DataFileIntWrapper time;
			if (!BestTimes.TryGetValue(d, out time)) return "";

			var minutes = (int)(time.Value / 1000f / 60f);
			var seconds = (int) ((time.Value - minutes * 1000 * 60) / 60f);
			var millis = time.Value - minutes * 1000 * 60 - seconds * 1000;

			return string.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, millis);
		}

		public int GetTime(FractionDifficulty d)
		{
			DataFileIntWrapper time;
			if (!BestTimes.TryGetValue(d, out time)) return 0;
			return time.Value;
		}
	}
}
