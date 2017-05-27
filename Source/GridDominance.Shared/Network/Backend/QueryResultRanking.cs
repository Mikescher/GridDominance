using System;
using System.Collections.Generic;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable InconsistentNaming
// ReSharper disable ArrangeTypeModifiers
namespace GridDominance.Shared.Network.Backend
{
#pragma warning disable 169
#pragma warning disable 649

	class QueryResultRanking
	{
		public string result;

		public int errorid;
		public string errormessage;

		public List<QueryResultRankingData> ranking;
	}

	public class QueryResultRankingData
	{
		public int userid;
		public string username;

		public int totalscore;
		public int totaltime;
	}

#pragma warning restore 169
#pragma warning restore 649
}
