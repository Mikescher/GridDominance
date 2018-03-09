using System.Collections.Generic;

namespace GridDominance.Shared.Network.Backend.QueryResult
{
#pragma warning disable 169
#pragma warning disable 649
	// ReSharper disable once ClassNeverInstantiated.Global
	// ReSharper disable InconsistentNaming
	// ReSharper disable once ArrangeTypeModifiers
	class QueryResultDownloadData
	{
		public string result;

		public int errorid;
		public string errormessage;

		public QueryResultUserData user;
		public List<QueryResultScoreData> scores;
	}
#pragma warning restore 169
#pragma warning restore 649
}
