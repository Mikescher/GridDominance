using MonoSAMFramework.Portable.Persistance;
using MonoSAMFramework.Portable.Persistance.DataFile;

namespace GridDominance.Shared.SaveData
{
	public class LevelDiffData : BaseDataFile
	{
		public bool HasCompleted;
		public int BestTime;

		public int GlobalBestTime;
		public int GlobalBestUserID;
		public int GlobalCompletionCount;

		public LevelDiffData()
		{
			HasCompleted = false;

			BestTime = -1;

			GlobalBestTime = -1;
			GlobalBestUserID = -1;
			GlobalCompletionCount = -1;
		}
		
		protected override void Configure()
		{
			RegisterConstructor(() => new LevelDiffData());

			RegisterProperty<LevelDiffData>(SemVersion.VERSION_1_0_0, "HasCompleted",          o => o.HasCompleted,          (o, v) => o.HasCompleted          = v);
			RegisterProperty<LevelDiffData>(SemVersion.VERSION_1_0_0, "BestTime",              o => o.BestTime,              (o, v) => o.BestTime              = v);
			RegisterProperty<LevelDiffData>(SemVersion.VERSION_1_0_0, "GlobalBestTime",        o => o.GlobalBestTime,        (o, v) => o.GlobalBestTime        = v);
			RegisterProperty<LevelDiffData>(SemVersion.VERSION_1_0_0, "GlobalBestUserID",      o => o.GlobalBestUserID,      (o, v) => o.GlobalBestUserID      = v);
			RegisterProperty<LevelDiffData>(SemVersion.VERSION_1_0_0, "GlobalCompletionCount", o => o.GlobalCompletionCount, (o, v) => o.GlobalCompletionCount = v);
		}

		protected override string GetTypeName()
		{
			return "PLAYER_PROFILE_LEVEL_DIFFDATA";
		}
	}
}
