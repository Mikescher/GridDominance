namespace GridDominance.Shared.Network.Backend.QueryResult
{
#pragma warning disable 169
#pragma warning disable 649
	// ReSharper disable once ClassNeverInstantiated.Global
	// ReSharper disable InconsistentNaming
	// ReSharper disable once ArrangeTypeModifiers
	// ReSharper disable UnassignedField.Global
	public class QueryResultUpdateUserLevelCompleted
	{
		public string result;

		public int errorid;
		public string errormessage;

		public bool firstclear;
		public bool inserted;
		public bool highscore;
		public int scoregain;
		public int leveltime;
		public QueryResultQueryUserLevelData meta;

		public QueryResultUserData user;
	}
	// ReSharper restore UnassignedField.Global
#pragma warning restore 169
#pragma warning restore 649
}
