using System;
using System.Collections.Generic;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable InconsistentNaming
// ReSharper disable ArrangeTypeModifiers
namespace GridDominance.Shared.Network.Backend.QueryResult
{
#pragma warning disable 169
#pragma warning disable 649

	class QueryResultHighscores
	{
		public string result;

		public int errorid;
		public string errormessage;

		public List<QueryResultHighscoreData> highscores;
	}
	
	class QueryResultHighscoreData
	{
		public string levelid;
		public int difficulty;

		public int best_time;
		public int completion_count;

		public DateTimeOffset best_last_changed;
		public int best_userid;
	}

#pragma warning restore 169
#pragma warning restore 649
}
