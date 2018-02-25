using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GridDominance.Shared.Network.Backend;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using MonoSAMFramework.Portable.GameMath.Geometry;

namespace GridDominance.Shared.SCCM
{
	public class SCCMLevelMeta
	{
		public long OnlineID;
		public string LevelName;

		public int UserID;
		public string Username;

		public DateTimeOffset UploadTime;
		public string Hash;
		public Version MinimumVersion;
		public DSize GridSize;

		public int Stars;
		public SCCMLevelDifficultyMeta[] Highscores;

		public static SCCMLevelMeta Parse(QueryResultQueryUserLevelData dat)
		{
			return new SCCMLevelMeta
			{
				OnlineID = dat.id,
				LevelName = dat.name,

				UserID = dat.userid,
				Username = dat.username,

				Stars = dat.stars,
				UploadTime = dat.upload_timestamp,
				Hash = dat.datahash,
				MinimumVersion = dat.upload_version,
				GridSize = new DSize(dat.grid_width, dat.grid_height),

				Highscores = new[]
				{
					new SCCMLevelDifficultyMeta
					{
						Difficulty           = FractionDifficulty.DIFF_0,
						TotalCompleted       = dat.d0_completed,
						TotalPlayed          = dat.d0_played,
						HighscoreUserID      = dat.d0_bestuserid,
						HighscoreUsername    = dat.d0_bestusername,
						HighscoreTime        = dat.d0_besttime,
						HighhhscoreTimestamp = dat.d0_besttimestamp,
					},
					new SCCMLevelDifficultyMeta
					{
						Difficulty = FractionDifficulty.DIFF_1,
						TotalCompleted       = dat.d1_completed,
						TotalPlayed          = dat.d1_played,
						HighscoreUserID      = dat.d1_bestuserid,
						HighscoreUsername    = dat.d1_bestusername,
						HighscoreTime        = dat.d1_besttime,
						HighhhscoreTimestamp = dat.d1_besttimestamp,
					},
					new SCCMLevelDifficultyMeta
					{
						Difficulty = FractionDifficulty.DIFF_2,
						TotalCompleted       = dat.d2_completed,
						TotalPlayed          = dat.d2_played,
						HighscoreUserID      = dat.d2_bestuserid,
						HighscoreUsername    = dat.d2_bestusername,
						HighscoreTime        = dat.d2_besttime,
						HighhhscoreTimestamp = dat.d2_besttimestamp,
					},
					new SCCMLevelDifficultyMeta
					{
						Difficulty = FractionDifficulty.DIFF_3,
						TotalCompleted       = dat.d3_completed,
						TotalPlayed          = dat.d3_played,
						HighscoreUserID      = dat.d3_bestuserid,
						HighscoreUsername    = dat.d3_bestusername,
						HighscoreTime        = dat.d3_besttime,
						HighhhscoreTimestamp = dat.d3_besttimestamp,
					},
				}
			};
		}
	}

	public class SCCMLevelDifficultyMeta
	{
		public FractionDifficulty Difficulty;

		public int TotalCompleted;
		public int TotalPlayed;

		public int? HighscoreUserID;
		public string HighscoreUsername;
		public int? HighscoreTime;
		public DateTimeOffset? HighhhscoreTimestamp;
	}
}
