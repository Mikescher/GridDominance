using System;
using System.Collections.Generic;

namespace GridDominance.Shared.Network.Backend
{
#pragma warning disable 169
#pragma warning disable 649
	// ReSharper disable ClassNeverInstantiated.Global
	// ReSharper disable InconsistentNaming
	// ReSharper disable once ArrangeTypeModifiers
	class QueryResultQueryUserLevel
	{
		public string result;

		public int errorid;
		public string errormessage;

		public List<QueryResultQueryUserLevelData> data;
	}

	public class QueryResultQueryUserLevelData
	{
		public long id;
		public string name;
		public int userid;
		public string username;

		public DateTimeOffset upload_timestamp;
		public string datahash;
		public Version upload_version;
		public int stars;
		public int grid_width;
		public int grid_height;

		public int d0_completed;
		public int d0_played;
		public int? d0_bestuserid;
		public string d0_bestusername;
		public int? d0_besttime;
		public DateTimeOffset? d0_besttimestamp;

		public int d1_completed;
		public int d1_played;
		public int? d1_bestuserid;
		public string d1_bestusername;
		public int? d1_besttime;
		public DateTimeOffset? d1_besttimestamp;

		public int d2_completed;
		public int d2_played;
		public int? d2_bestuserid;
		public string d2_bestusername;
		public int? d2_besttime;
		public DateTimeOffset? d2_besttimestamp;

		public int d3_completed;
		public int d3_played;
		public int? d3_bestuserid;
		public string d3_bestusername;
		public int? d3_besttime;
		public DateTimeOffset? d3_besttimestamp;
	}

	// ReSharper restore ClassNeverInstantiated.Global
#pragma warning restore 169
#pragma warning restore 649
}
