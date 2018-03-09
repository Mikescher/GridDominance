using System.Collections.Generic;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable InconsistentNaming
// ReSharper disable ArrangeTypeModifiers
namespace GridDominance.Shared.Network.Backend.QueryResult
{
#pragma warning disable 169
#pragma warning disable 649

	public class QueryResultRanking
	{
		public string result;

		public int errorid;
		public string errormessage;

		public List<QueryResultRankingData> ranking;
		public List<QueryResultUserRanking> personal;
	}

	public class QueryResultRankingData
	{
		public int userid;
		public string username;

		public int totalscore;
		public int totaltime;
	}

	public class QueryResultUserRanking
	{
		public int rank;
		public int userid;
		public string username;

		public int totalscore;
		public int totaltime;
	}

#pragma warning restore 169
#pragma warning restore 649
}
