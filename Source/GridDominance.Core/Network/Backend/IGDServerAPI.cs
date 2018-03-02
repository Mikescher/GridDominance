using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GridDominance.Graphfileformat.Blueprint;
using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Shared.Network.Backend;
using GridDominance.Shared.SaveData;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using GridDominance.Shared.SCCM;
using MonoSAMFramework.Portable.LogProtocol;

namespace GridDominance.Shared.Network
{
	public interface IGDServerAPI
	{
		Task Ping(PlayerProfile profile);
		Task CreateUser(PlayerProfile profile);
		Task SetScore(PlayerProfile profile, Guid levelid, FractionDifficulty diff, int time);
		Task SetScoreAndTime(PlayerProfile profile);
		Task<bool> DownloadData(PlayerProfile profile);
		Task<bool> Reupload(PlayerProfile profile);
		Task LogClient(PlayerProfile profile, SAMLogEntry entry);
		Task DownloadHighscores(PlayerProfile profile);
		Task<Tuple<VerifyResult, int, string>> Verify(string username, string password);
		Task<Tuple<UpgradeResult, string>> UpgradeUser(PlayerProfile profile, string username, string password);
		Task<Tuple<ChangePasswordResult, string>> ChangePassword(PlayerProfile profile, string newPassword);
		Task<QueryResultRanking> GetRanking(PlayerProfile profile, GraphBlueprint limit, HighscoreCategory cat);
		Task<Tuple<VerifyResult, string>> MergeLogin(PlayerProfile profile, string username, string password);
		Task<Tuple<bool, Int64>> GetNewCustomLevelID(PlayerProfile profile);
		Task<UploadResult> UploadUserLevel(PlayerProfile profile, LevelBlueprint level, SCCMLevelData rawData, byte[] binary);
		Task<List<SCCMLevelMeta>> QueryUserLevel(PlayerProfile profile, QueryUserLevelCategory cat, string param, int pagination);
		Task<byte[]> DownloadUserLevel(PlayerProfile profile,long onlineID);
		Task<bool?> SetCustomLevelPlayed(PlayerProfile profile, long onlineID, FractionDifficulty d);
		Task<CustomLevelCompletionResult> SetCustomLevelCompleted(PlayerProfile profile, long onlineID, FractionDifficulty d, int time);
		Task<Tuple<int, bool>> SetCustomLevelStarred(PlayerProfile profile, long onlineID, bool star); // <level_starcount, isStarred>
	}

	#pragma warning disable 1998
	public class DummyGDServerAPI : IGDServerAPI
	{
		public async Task Ping(PlayerProfile profile)
		{
			//
		}

		public async Task CreateUser(PlayerProfile profile)
		{
			profile.AccountType = AccountType.Anonymous;
			profile.OnlineUserID = 77;
			profile.OnlineRevisionID = 1;
			profile.OnlinePasswordHash = "";
		}

		public async Task SetScore(PlayerProfile profile, Guid levelid, FractionDifficulty diff, int time)
		{
			//
		}

		public async Task SetScoreAndTime(PlayerProfile profile)
		{
			//
		}

		public async Task<bool> DownloadData(PlayerProfile profile)
		{
			return true;
		}

		public async Task<bool> Reupload(PlayerProfile profile)
		{
			return true;
		}

		public async Task LogClient(PlayerProfile profile, SAMLogEntry entry)
		{
			//
		}

		public async Task DownloadHighscores(PlayerProfile profile)
		{
			//
		}

		public async Task<Tuple<VerifyResult, int, string>> Verify(string username, string password)
		{
			return Tuple.Create(VerifyResult.Success, 77, string.Empty);
		}

		public async Task<Tuple<VerifyResult, string>> MergeLogin(PlayerProfile profile, string username, string password)
		{
			return Tuple.Create(VerifyResult.Success, string.Empty);
		}

		public async Task<Tuple<UpgradeResult, string>> UpgradeUser(PlayerProfile profile, string username, string password)
		{
			profile.AccountType = AccountType.Full;
			profile.OnlinePasswordHash = "";
			profile.OnlineRevisionID++;

			MainGame.Inst.SaveProfile();

			return Tuple.Create(UpgradeResult.Success, string.Empty);
		}

		public async Task<Tuple<ChangePasswordResult, string>> ChangePassword(PlayerProfile profile, string newPassword)
		{
			return Tuple.Create(ChangePasswordResult.Success, string.Empty);
		}

		public async Task<QueryResultRanking> GetRanking(PlayerProfile profile, GraphBlueprint limit, HighscoreCategory cat)
		{
			return new QueryResultRanking();
		}

		public async Task<Tuple<bool, Int64>> GetNewCustomLevelID(PlayerProfile profile)
		{
			return Tuple.Create(true, 999L);
		}

		public async Task<UploadResult> UploadUserLevel(PlayerProfile profile, LevelBlueprint level, SCCMLevelData rawData, byte[] binary)
		{
			return UploadResult.Success;
		}

		public async Task<List<SCCMLevelMeta>> QueryUserLevel(PlayerProfile profile, QueryUserLevelCategory cat, string param, int pagination)
		{
			return new List<SCCMLevelMeta>();
		}

		public async Task<byte[]> DownloadUserLevel(PlayerProfile profile,long onlineID)
		{
			return new byte[0];
		}

		public async Task<bool?> SetCustomLevelPlayed(PlayerProfile profile, long onlineID, FractionDifficulty d)
		{
			return true;
		}

		public async Task<CustomLevelCompletionResult> SetCustomLevelCompleted(PlayerProfile profile, long onlineID, FractionDifficulty d, int time)
		{
			return CustomLevelCompletionResult.PersonalBest;
		}

		public async Task<Tuple<int, bool>> SetCustomLevelStarred(PlayerProfile profile, long onlineID, bool star)
		{
			return Tuple.Create(star?1:0, star);
		}
	}
#pragma warning restore 1998
}