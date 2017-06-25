using System.Collections.Generic;
using System.Linq;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.Persistance;
using MonoSAMFramework.Portable.Persistance.DataFile;

namespace GridDominance.Shared.SaveData
{
	public class LevelData : BaseDataFile
	{
		public Dictionary<FractionDifficulty, LevelDiffData> Data;

		public LevelData()
		{
			Data = new Dictionary<FractionDifficulty, LevelDiffData>();

			Data.Add(FractionDifficulty.DIFF_0, new LevelDiffData());
			Data.Add(FractionDifficulty.DIFF_1, new LevelDiffData());
			Data.Add(FractionDifficulty.DIFF_2, new LevelDiffData());
			Data.Add(FractionDifficulty.DIFF_3, new LevelDiffData());
		}

		protected override void OnAfterDeserialize()
		{
			if (! Data.ContainsKey(FractionDifficulty.DIFF_0)) Data.Add(FractionDifficulty.DIFF_0, new LevelDiffData());
			if (! Data.ContainsKey(FractionDifficulty.DIFF_1)) Data.Add(FractionDifficulty.DIFF_1, new LevelDiffData());
			if (! Data.ContainsKey(FractionDifficulty.DIFF_2)) Data.Add(FractionDifficulty.DIFF_2, new LevelDiffData());
			if (! Data.ContainsKey(FractionDifficulty.DIFF_3)) Data.Add(FractionDifficulty.DIFF_3, new LevelDiffData());
		}

		public int TotalPoints => Data.Where(p => p.Value.HasCompleted).Select(p => p.Key).Sum(FractionDifficultyHelper.GetScore);
		public int CompletionCount => Data.Count(p => p.Value.HasCompleted);

		public bool HasCompleted(FractionDifficulty d)
		{
			return Data[d].HasCompleted;
		}

		public bool HasAnyCompleted()
		{
			return
				Data[FractionDifficulty.DIFF_0].HasCompleted ||
				Data[FractionDifficulty.DIFF_1].HasCompleted ||
				Data[FractionDifficulty.DIFF_2].HasCompleted ||
				Data[FractionDifficulty.DIFF_3].HasCompleted;
		}

		public bool HasAllCompleted()
		{
			return
				Data[FractionDifficulty.DIFF_0].HasCompleted &&
				Data[FractionDifficulty.DIFF_1].HasCompleted &&
				Data[FractionDifficulty.DIFF_2].HasCompleted &&
				Data[FractionDifficulty.DIFF_3].HasCompleted;
		}

		public void SetBestTime(FractionDifficulty d, int? time)
		{
			if (time == null)
			{
				Data[d].HasCompleted = false;
			}
			else
			{
				Data[d].HasCompleted = true;
				Data[d].BestTime = time.Value;
			}
		}

		protected override void Configure()
		{
			RegisterConstructor(() => new LevelData());

			RegisterPropertyEnumDictionary<LevelData, FractionDifficulty, LevelDiffData>(SemVersion.VERSION_1_0_0, "data", () => new LevelDiffData(), o => o.Data, (o, v) => o.Data = v);
		}

		protected override string GetTypeName()
		{
			return "PLAYER_PROFILE_LEVEL_DATA";
		}

		public string GetTimeString(FractionDifficulty d)
		{
			var v = Data[d];

			if (!v.HasCompleted) return string.Empty;

			return TimeExtension.FormatMilliseconds(v.BestTime);
		}

		public int GetTime(FractionDifficulty d)
		{
			if (!Data[d].HasCompleted) return 0;
			return Data[d].BestTime;
		}
	}
}
