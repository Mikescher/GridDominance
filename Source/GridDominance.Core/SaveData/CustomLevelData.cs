using MonoSAMFramework.Portable.Persistance;
using MonoSAMFramework.Portable.Persistance.DataFile;

namespace GridDominance.Shared.SaveData
{
	public class CustomLevelData : BaseDataFile
	{
		public long id;
		public bool starred;

		public bool Diff0_HasCompleted;
		public int  Diff0_BestTime;

		public bool Diff1_HasCompleted;
		public int  Diff1_BestTime;

		public bool Diff2_HasCompleted;
		public int  Diff2_BestTime;

		public bool Diff3_HasCompleted;
		public int  Diff3_BestTime;

		protected override void Configure()
		{
			RegisterConstructor(() => new CustomLevelData());

			RegisterProperty<CustomLevelData>(SemVersion.VERSION_1_0_5, "id",                 o => o.id,                 (o, v) => o.id                 = v);
			RegisterProperty<CustomLevelData>(SemVersion.VERSION_1_0_5, "starred",            o => o.starred,            (o, v) => o.starred            = v);

			RegisterProperty<CustomLevelData>(SemVersion.VERSION_1_0_5, "Diff0_HasCompleted", o => o.Diff0_HasCompleted, (o, v) => o.Diff0_HasCompleted = v);
			RegisterProperty<CustomLevelData>(SemVersion.VERSION_1_0_5, "Diff0_BestTime",     o => o.Diff0_BestTime,     (o, v) => o.Diff0_BestTime     = v);
			
			RegisterProperty<CustomLevelData>(SemVersion.VERSION_1_0_5, "Diff1_HasCompleted", o => o.Diff1_HasCompleted, (o, v) => o.Diff1_HasCompleted = v);
			RegisterProperty<CustomLevelData>(SemVersion.VERSION_1_0_5, "Diff1_BestTime",     o => o.Diff1_BestTime,     (o, v) => o.Diff1_BestTime     = v);
			
			RegisterProperty<CustomLevelData>(SemVersion.VERSION_1_0_5, "Diff2_HasCompleted", o => o.Diff2_HasCompleted, (o, v) => o.Diff2_HasCompleted = v);
			RegisterProperty<CustomLevelData>(SemVersion.VERSION_1_0_5, "Diff2_BestTime",     o => o.Diff2_BestTime,     (o, v) => o.Diff2_BestTime     = v);
			
			RegisterProperty<CustomLevelData>(SemVersion.VERSION_1_0_5, "Diff3_HasCompleted", o => o.Diff3_HasCompleted, (o, v) => o.Diff3_HasCompleted = v);
			RegisterProperty<CustomLevelData>(SemVersion.VERSION_1_0_5, "Diff3_BestTime",     o => o.Diff3_BestTime,     (o, v) => o.Diff3_BestTime     = v);
		}

		protected override string GetTypeName()
		{
			return "PLAYER_PROFILE_CUSTOMLEVELDATA";
		}
	}
}
