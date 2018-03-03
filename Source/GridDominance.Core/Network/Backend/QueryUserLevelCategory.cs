using MonoSAMFramework.Portable.LogProtocol;

namespace GridDominance.Shared.Network.Backend
{
	public enum QueryUserLevelCategory
	{
		AllLevelsOfUserid,
		TopLevelsAllTime,
		HotLevels,
		NewLevels,
		Search,
	}

	public static class QueryUserLevelCategoryHelper
	{
		public static string EnumToString(QueryUserLevelCategory c)
		{
			switch (c)
			{
				case QueryUserLevelCategory.AllLevelsOfUserid: return "@user";
				case QueryUserLevelCategory.TopLevelsAllTime:  return "@top";
				case QueryUserLevelCategory.HotLevels:         return "@hot";
				case QueryUserLevelCategory.NewLevels:         return "@new";
				case QueryUserLevelCategory.Search:            return "@search";

				default:
					SAMLog.Error("QULCH::EnumSwitch_ETS", "c = " + c);
					return "?";
			}
		}
	}
}
