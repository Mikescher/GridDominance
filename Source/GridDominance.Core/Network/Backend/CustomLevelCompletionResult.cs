using GridDominance.Shared.SCCM;

namespace GridDominance.Shared.Network.Backend
{
	public class CustomLevelCompletionResult
	{
		public readonly bool IsError;
		public readonly bool IsPersonalBest;
		public readonly bool IsFirstClear;
		public readonly bool IsWorldRecord;
		public readonly int ScoreGain;

		public readonly SCCMLevelMeta Metadata;

		private CustomLevelCompletionResult(bool e, bool pb, bool fc, bool wr, int g, SCCMLevelMeta m)
		{
			IsError = e;
			IsPersonalBest = pb;
			IsFirstClear = fc;
			IsWorldRecord = wr;
			ScoreGain = g;
			Metadata = m;
		}

		public static CustomLevelCompletionResult CreateError()
		{
			return new CustomLevelCompletionResult(true, false, false, false, 0, null);
		}

		public static CustomLevelCompletionResult Parse(QueryResultUpdateUserLevelCompleted r)
		{
			return new CustomLevelCompletionResult(false, r.inserted, r.firstclear, r.highscore, r.scoregain, SCCMLevelMeta.Parse(r.meta));
		}
	}
}
