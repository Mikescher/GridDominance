using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GridDominance.Shared.Network.Backend;
using GridDominance.Shared.SaveData;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using MonoSAMFramework.Portable.LogProtocol;

namespace GridDominance.Shared.Network
{
	public interface IGDServerAPI
	{
		Task Ping(PlayerProfile profile);
		Task CreateUser(PlayerProfile profile);
		Task SetScore(PlayerProfile profile, Guid levelid, FractionDifficulty diff, int time);
		Task DownloadData(PlayerProfile profile);
		Task Reupload(PlayerProfile profile);
		Task LogClient(PlayerProfile profile, SAMLogEntry entry);
		Task DownloadHighscores(PlayerProfile profile);
		Task<Tuple<VerifyResult, int, string>> Verify(string username, string password);
		Task<Tuple<UpgradeResult, string>> UpgradeUser(PlayerProfile profile, string username, string password);
		Task<Tuple<ChangePasswordResult, string>> ChangePassword(PlayerProfile profile, string newPassword);
		Task<List<QueryResultRankingData>> GetRanking();
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

		public async Task DownloadData(PlayerProfile profile)
		{
			//
		}

		public async Task Reupload(PlayerProfile profile)
		{
			//
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

		public async Task<List<QueryResultRankingData>> GetRanking()
		{
			return new List<QueryResultRankingData>();
		}
	}
	#pragma warning restore 1998
}