using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GridDominance.Graphfileformat.Blueprint;
using GridDominance.Shared.Network.Backend;
using GridDominance.Shared.Resources;
using GridDominance.Shared.SaveData;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using MonoSAMFramework.Portable;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.DeviceBridge;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.LogProtocol;
using MonoSAMFramework.Portable.Network.REST;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Localization;

namespace GridDominance.Shared.Network
{
	public class GDServerAPI : SAMRestAPI, IGDServerAPI
	{
		private const int RETRY_PING               = 2;
		private const int RETRY_SETSCORE           = 4;
		private const int RETRY_DOWNLOADDATA       = 4;
		private const int RETRY_LOGERROR           = 4;
		private const int RETRY_DOWNLOADHIGHSCORES = 4;
		private const int RETRY_CREATEUSER         = 6;
		private const int RETRY_VERIFY             = 6;
		private const int RETRY_CHANGE_PW          = 6;
		private const int RETRY_GETRANKING         = 6;

		private readonly IOperatingSystemBridge bridge;

		public GDServerAPI(IOperatingSystemBridge b) : base(GDConstants.SERVER_URL, GDConstants.SERVER_SECRET)
		{
			bridge = b;
		}

		public async Task Ping(PlayerProfile profile)
		{
			try
			{
				var ps = new RestParameterSet();
				ps.AddParameterInt("userid", profile.OnlineUserID);
				ps.AddParameterHash("password", profile.OnlinePasswordHash);
				ps.AddParameterString("app_version", GDConstants.Version.ToString());
				ps.AddParameterString("device_name", bridge.DeviceName);
				ps.AddParameterString("device_version", bridge.DeviceVersion);
				ps.AddParameterString("unlocked_worlds", profile.StrPurchasedWorlds);
				ps.AddParameterString("device_resolution", bridge.DeviceResolution.FormatAsResolution());

				var response = await QueryAsync<QueryResultPing>("ping", ps, RETRY_PING);

				if (response == null)
				{
					return; // meh - internal server error
				}
				else if (response.result == "success")
				{
					if (response.user.RevID > profile.OnlineRevisionID)
					{
						await DownloadData(profile);
					}

					if (profile.NeedsReupload) Reupload(profile).EnsureNoError();
				}
				else if (response.result == "error")
				{
					ShowErrorCommunication();

					if (response.errorid == BackendCodes.INTERNAL_EXCEPTION)
					{
						return; // meh
					}
					else if (response.errorid == BackendCodes.WRONG_PASSWORD || response.errorid == BackendCodes.USER_BY_ID_NOT_FOUND)
					{
						MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
						{
							SAMLog.Error("Backend::INVLOGIN", $"Local user cannot login on server ({response.errorid}:{response.errormessage}). Reset local user");

							// something went horribly wrong
							// create new user on next run
							profile.OnlineUserID = -1;
							profile.OnlineUsername = "anonymous";
							profile.AccountType = AccountType.Local;
							profile.OnlinePasswordHash = "";

							MainGame.Inst.SaveProfile();

							MainGame.Inst.Backend.CreateUser(MainGame.Inst.Profile).EnsureNoError();
						});
					}
					else
					{
						SAMLog.Error("Backend::PingError", $"Ping: Error {response.errorid}: {response.errormessage}");
					}
				}
			}
			catch (RestConnectionException e)
			{
				SAMLog.Warning("Backend::PingRCE", e); // probably no internet
				ShowErrorConnection();
			}
			catch (Exception e)
			{
				SAMLog.Error("Backend::PingE", e);
				ShowErrorCommunication();
			}
		}

		public async Task CreateUser(PlayerProfile profile)
		{
			try
			{
				var pw = bridge.DoSHA256(Guid.NewGuid().ToString("N"));

				var ps = new RestParameterSet();
				ps.AddParameterHash("password", pw);
				ps.AddParameterString("app_version", GDConstants.Version.ToString());
				ps.AddParameterString("device_name", bridge.DeviceName);
				ps.AddParameterString("device_version", bridge.DeviceVersion);
				ps.AddParameterString("unlocked_worlds", profile.StrPurchasedWorlds);
				ps.AddParameterString("device_resolution", bridge.DeviceResolution.FormatAsResolution());
				
				var response = await QueryAsync<QueryResultCreateUser>("create-user", ps, RETRY_CREATEUSER);

				if (response == null)
				{
					SAMLog.Error("Backend::CU_Null", "CreateUser returned NULL");
					ShowErrorCommunication();
				}
				else if (response.result == "success")
				{
					MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
					{
						profile.AccountType = AccountType.Anonymous;
						profile.OnlineUserID = response.user.ID;
						profile.OnlineRevisionID = response.user.RevID;
						profile.OnlinePasswordHash = pw;

						MainGame.Inst.SaveProfile();

						Reupload(profile).EnsureNoError();
					});

				}
				else if (response.result == "error")
				{
					SAMLog.Error("Backend::CU_Err", $"CreateUser: Error {response.errorid}: {response.errormessage}");
					ShowErrorCommunication();
				}
			}
			catch (RestConnectionException e)
			{
				SAMLog.Warning("Backend::CU_RCE", e); // probably no internet
				ShowErrorConnection();
			}
			catch (Exception e)
			{
				SAMLog.Error("Backend::CU_E", e);
				ShowErrorCommunication();
			}
		}

		public async Task SetScore(PlayerProfile profile, Guid levelid, FractionDifficulty diff, int time)
		{
			try
			{
				var ps = new RestParameterSet();
				ps.AddParameterInt("userid", profile.OnlineUserID);
				ps.AddParameterHash("password", profile.OnlinePasswordHash);
				ps.AddParameterString("app_version", GDConstants.Version.ToString());
				ps.AddParameterGuid("levelid", levelid); 
				ps.AddParameterInt("difficulty", (int)diff);
				ps.AddParameterInt("leveltime", time);
				ps.AddParameterInt("totalscore", profile.TotalPoints);

				var response = await QueryAsync<QueryResultSetScore>("set-score", ps, RETRY_SETSCORE);

				if (response == null)
				{
					return; // meh - internal server error
				}
				else if (response.result == "success")
				{
					if (response.update)
					{
						MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
						{
							profile.OnlineRevisionID = response.user.RevID;

							MainGame.Inst.SaveProfile();

							if (profile.NeedsReupload) Reupload(profile).EnsureNoError();
						});
					}
				}
				else if (response.result == "error")
				{
					ShowErrorCommunication();

					if (response.errorid == BackendCodes.INTERNAL_EXCEPTION)
					{
						return; // meh
					}
					else if (response.errorid == BackendCodes.WRONG_PASSWORD || response.errorid == BackendCodes.USER_BY_ID_NOT_FOUND)
					{
						MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
						{
							SAMLog.Error("Backend::SS_INVLOGIN", $"Local user cannot login on server ({response.errorid}:{response.errormessage}). Reset local user");

							// something went horribly wrong
							// create new user on next run
							profile.OnlineUserID = -1;

							MainGame.Inst.SaveProfile();
						});
					}
					else
					{
						SAMLog.Error("Backend::SS_ERR", $"SetScore: Error {response.errorid}: {response.errormessage}");
					}
				}
			}
			catch (RestConnectionException e)
			{
				SAMLog.Warning("Backend::SS_RCE", e); // probably no internet
				ShowErrorConnection();

				MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
				{
					profile.NeedsReupload = true;

					MainGame.Inst.SaveProfile();
				});
			}
			catch (Exception e)
			{
				SAMLog.Error("Backend::SS_E", e);
				ShowErrorCommunication();

				MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
				{
					profile.NeedsReupload = true;

					MainGame.Inst.SaveProfile();
				});
			}
		}

		public async Task SetMultiplayerScore(PlayerProfile profile, int score)
		{
			try
			{
				var ps = new RestParameterSet();
				ps.AddParameterInt("userid", profile.OnlineUserID);
				ps.AddParameterHash("password", profile.OnlinePasswordHash);
				ps.AddParameterString("app_version", GDConstants.Version.ToString());
				ps.AddParameterInt("mpscore", score);

				var response = await QueryAsync<QueryResultSetMPScore>("set-mpscore", ps, RETRY_SETSCORE);

				if (response == null)
				{
					return; // meh - internal server error
				}
				else if (response.result == "success")
				{
					MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
					{
						profile.OnlineRevisionID = response.user.RevID;

						MainGame.Inst.SaveProfile();

						if (profile.NeedsReupload) Reupload(profile).EnsureNoError();
					});
				}
				else if (response.result == "error")
				{
					ShowErrorCommunication();

					if (response.errorid == BackendCodes.INTERNAL_EXCEPTION)
					{
						return; // meh
					}
					else if (response.errorid == BackendCodes.WRONG_PASSWORD || response.errorid == BackendCodes.USER_BY_ID_NOT_FOUND)
					{
						MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
						{
							SAMLog.Error("Backend::SMPS_INVLOGIN", $"Local user cannot login on server ({response.errorid}:{response.errormessage}). Reset local user");

							// something went horribly wrong
							// create new user on next run
							profile.OnlineUserID = -1;

							MainGame.Inst.SaveProfile();
						});
					}
					else
					{
						SAMLog.Error("Backend::SMPS_ERR", $"SetScore: Error {response.errorid}: {response.errormessage}");
					}
				}
			}
			catch (RestConnectionException e)
			{
				SAMLog.Warning("Backend::SMPS_RCE", e); // probably no internet
				ShowErrorConnection();

				MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
				{
					profile.NeedsReupload = true;

					MainGame.Inst.SaveProfile();
				});
			}
			catch (Exception e)
			{
				SAMLog.Error("Backend::SMPS_E", e);
				ShowErrorCommunication();

				MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
				{
					profile.NeedsReupload = true;

					MainGame.Inst.SaveProfile();
				});
			}
		}

		public async Task DownloadData(PlayerProfile profile)
		{
			try
			{
				var ps = new RestParameterSet();
				ps.AddParameterInt("userid", profile.OnlineUserID);
				ps.AddParameterHash("password", profile.OnlinePasswordHash);
				ps.AddParameterString("app_version", GDConstants.Version.ToString());

				var response = await QueryAsync<QueryResultDownloadData>("download-data", ps, RETRY_DOWNLOADDATA);

				if (response == null)
				{
					return; // meh - internal server error
				}
				else if (response.result == "success")
				{
					MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
					{
						profile.OnlineRevisionID = response.user.RevID;

						foreach (var scdata in response.scores)
						{
							profile.SetCompleted(Guid.Parse(scdata.levelid), (FractionDifficulty)scdata.difficulty, scdata.best_time, false);
						}

						profile.MultiplayerPoints = response.user.MultiplayerScore;
						profile.HasMultiplayerGames |= response.user.MultiplayerScore>0;

						MainGame.Inst.SaveProfile();
					});
				}
				else if (response.result == "error")
				{
					if (response.errorid == BackendCodes.INTERNAL_EXCEPTION)
					{
						return; // meh
					}
					else if (response.errorid == BackendCodes.WRONG_PASSWORD || response.errorid == BackendCodes.USER_BY_ID_NOT_FOUND)
					{
						MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
						{
							SAMLog.Error("Backend::DD_INVLOGIN", $"Local user cannot login on server ({response.errorid}:{response.errormessage}). Reset local user");

							// something went horribly wrong
							// create new user on next run
							profile.OnlineUserID = -1;

							MainGame.Inst.SaveProfile();
						});
					}
					else
					{
						SAMLog.Error("Backend::DD_ERR", $"SetScore: Error {response.errorid}: {response.errormessage}");
						ShowErrorCommunication();
					}
				}
			}
			catch (RestConnectionException e)
			{
				SAMLog.Warning("Backend::DD_RCE", e); // probably no internet
				ShowErrorConnection();
			}
			catch (Exception e)
			{
				SAMLog.Error("Backend::DD_E", e);
				ShowErrorCommunication();
			}
		}

		public async Task Reupload(PlayerProfile profile)
		{
			profile.NeedsReupload = false;

			try
			{
				var ps = new RestParameterSet();
				ps.AddParameterInt("userid", profile.OnlineUserID);
				ps.AddParameterHash("password", profile.OnlinePasswordHash);
				ps.AddParameterString("app_version", GDConstants.Version.ToString());
				ps.AddParameterJson("data", CreateScoreArray(profile));

				var response = await QueryAsync<QueryResultSetMultiscore>("set-multiscore", ps, RETRY_DOWNLOADDATA);

				if (response == null)
				{
					return; // meh - internal server error
				}
				else if (response.result == "success")
				{
					if (response.update)
					{
						MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
						{
							profile.OnlineRevisionID = response.user.RevID;

							MainGame.Inst.SaveProfile();

							if (profile.NeedsReupload) Reupload(profile).EnsureNoError();
						});
					}
				}
				else if (response.result == "error")
				{
					ShowErrorCommunication();

					if (response.errorid == BackendCodes.INTERNAL_EXCEPTION)
					{
						return; // meh
					}
					else if (response.errorid == BackendCodes.WRONG_PASSWORD || response.errorid == BackendCodes.USER_BY_ID_NOT_FOUND)
					{
						MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
						{
							SAMLog.Error("Backend::RU_INVLOGIN", $"Local user cannot login on server ({response.errorid}:{response.errormessage}). Reset local user");

							// something went horribly wrong
							// create new user on next run
							profile.OnlineUserID = -1;

							MainGame.Inst.SaveProfile();
						});
					}
					else
					{
						SAMLog.Error("Backend::RU_ERR", $"SetScore: Error {response.errorid}: {response.errormessage}");
					}
				}
			}
			catch (RestConnectionException e)
			{
				SAMLog.Warning("Backend::RU_RCE", e); // probably no internet
				ShowErrorConnection();

				MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
				{
					profile.NeedsReupload = true;

					MainGame.Inst.SaveProfile();
				});
			}
			catch (Exception e)
			{
				SAMLog.Error("Backend::RU_E", e);
				ShowErrorCommunication();

				MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
				{
					profile.NeedsReupload = true;

					MainGame.Inst.SaveProfile();
				});
			}
		}

		public async Task LogClient(PlayerProfile profile, SAMLogEntry entry)
		{
			try
			{
				var ps = new RestParameterSet();
				ps.AddParameterInt("userid", profile.OnlineUserID, false);
				ps.AddParameterHash("password", profile.OnlinePasswordHash, false);
				ps.AddParameterString("app_version", GDConstants.Version.ToString(), false);
				ps.AddParameterString("screen_resolution", bridge.DeviceResolution.FormatAsResolution(), false);
				ps.AddParameterString("exception_id", entry.Type, false);
				ps.AddParameterCompressed("exception_message", entry.MessageShort, false);
				ps.AddParameterCompressed("exception_stacktrace", entry.MessageLong, false);
				ps.AddParameterCompressed("additional_info", bridge.FullDeviceInfoString, false);

				var response = await QueryAsync<QueryResultDownloadData>("log-client", ps, RETRY_LOGERROR);

				if (response == null)
				{
					SAMLog.Warning("Log_Upload_LC_NULL", "response == null");
				}
				else if (response.result == "error")
				{
					SAMLog.Warning("Log_Upload_LC_ERR", response.errormessage);
				}
			}
			catch (RestConnectionException e)
			{
				// well, that sucks
				// probably no internet
				SAMLog.Warning("Backend::LC_RCE", e); // probably no internet
			}
			catch (Exception e)
			{
				SAMLog.Error("Backend::LC_E", e);
			}
		}
		
		public async Task DownloadHighscores(PlayerProfile profile)
		{
			try
			{
				var response = await QueryAsync<QueryResultHighscores>("get-highscores", new RestParameterSet(), RETRY_DOWNLOADHIGHSCORES);

				if (response == null)
				{
					SAMLog.Warning("Backend::DH_NULL", $"DownloadHighscores: Error response == null");
					ShowErrorCommunication();
				}
				else if (response.result == "success")
				{
					MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
					{
						foreach (var hscore in response.highscores)
						{
							Guid id;
							if (Guid.TryParse(hscore.levelid, out id))
							{
								var d = profile.GetLevelData(id, (FractionDifficulty)hscore.difficulty);

								d.GlobalBestTime        = hscore.best_time;
								d.GlobalBestUserID      = hscore.best_userid;
								d.GlobalCompletionCount = hscore.completion_count;
							}
						}

						MainGame.Inst.SaveProfile();
					});
				}
				else if (response.result == "error")
				{
					SAMLog.Warning("Backend::DH_ERR", $"DownloadHighscores: Error {response.errorid}: {response.errormessage}");
					ShowErrorCommunication();
				}
			}
			catch (RestConnectionException e)
			{
				SAMLog.Warning("Backend::DH_RCE", e); // probably no internet
				ShowErrorConnection();
			}
			catch (Exception e)
			{
				SAMLog.Error("Backend::DH_E", e);
				ShowErrorCommunication();
			}
		}

		public async Task<Tuple<VerifyResult, int, string>> Verify(string username, string password)
		{
			try
			{
				var pwHash = bridge.DoSHA256(password);

				var ps = new RestParameterSet();
				ps.AddParameterString("username", username);
				ps.AddParameterHash("password", pwHash);
				ps.AddParameterString("app_version", GDConstants.Version.ToString());

				var response = await QueryAsync<QueryResultVerify>("verify", ps, RETRY_VERIFY);

				if (response == null)
				{
					return Tuple.Create(VerifyResult.InternalError, -1, "Internal server error");
				}
				else if (response.result == "success")
				{
					if (response.user.AutoUser) return Tuple.Create(VerifyResult.WrongUsername, -1, string.Empty);

					return Tuple.Create(VerifyResult.Success, response.user.ID, string.Empty);
				}
				else if (response.result == "error")
				{
					if (response.errorid == BackendCodes.INTERNAL_EXCEPTION)
					{
						return Tuple.Create(VerifyResult.InternalError, -1, response.errormessage);
					}
					else if (response.errorid == BackendCodes.WRONG_PASSWORD)
					{
						return Tuple.Create(VerifyResult.WrongPassword, -1, string.Empty);
					}
					else if (response.errorid == BackendCodes.USER_BY_NAME_NOT_FOUND)
					{
						return Tuple.Create(VerifyResult.WrongUsername, -1, string.Empty);
					}
					else
					{
						ShowErrorCommunication();
						SAMLog.Error("Backend::V_ERR", $"Verify: Error {response.errorid}: {response.errormessage}");
						return Tuple.Create(VerifyResult.InternalError, -1, response.errormessage);
					}
				}
				else
				{
					return Tuple.Create(VerifyResult.InternalError, -1, "Internal server exception");
				}
			}
			catch (RestConnectionException e)
			{
				SAMLog.Warning("Backend::V_RCE", e); // probably no internet
				ShowErrorConnection();
				return Tuple.Create(VerifyResult.NoConnection, -1, string.Empty);
			}
			catch (Exception e)
			{
				SAMLog.Error("Backend::V_E", e);
				ShowErrorCommunication();
				return Tuple.Create(VerifyResult.InternalError, -1, "Internal server exception");
			}
		}

		public async Task<Tuple<UpgradeResult, string>> UpgradeUser(PlayerProfile profile, string username, string password)
		{
			try
			{
				var pwHashOld = profile.OnlinePasswordHash;
				var pwHashNew = bridge.DoSHA256(password);

				var ps = new RestParameterSet();
				ps.AddParameterInt("userid", profile.OnlineUserID);
				ps.AddParameterHash("password_old", pwHashOld);
				ps.AddParameterString("app_version", GDConstants.Version.ToString());
				ps.AddParameterHash("password_new", pwHashNew);
				ps.AddParameterString("username_new", username);

				var response = await QueryAsync<QueryResultUpgradeUser>("upgrade-user", ps, RETRY_VERIFY);

				if (response == null)
				{
					ShowErrorCommunication();
					return Tuple.Create(UpgradeResult.InternalError, "Internal server error");
				}
				else if (response.result == "success")
				{
					MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
					{
						profile.AccountType = AccountType.Full;
						profile.OnlineUsername = response.user.Username;
						profile.OnlinePasswordHash = pwHashNew;
						profile.OnlineRevisionID = response.user.RevID;

						MainGame.Inst.SaveProfile();
					});


					return Tuple.Create(UpgradeResult.Success, string.Empty);
				}
				else if (response.result == "error")
				{
					if (response.errorid == BackendCodes.INTERNAL_EXCEPTION)
					{
						ShowErrorCommunication();
						return Tuple.Create(UpgradeResult.InternalError, response.errormessage);
					}

					if (response.errorid == BackendCodes.WRONG_PASSWORD)
						return Tuple.Create(UpgradeResult.AuthError, string.Empty);

					if (response.errorid == BackendCodes.UPGRADE_USER_ACCOUNT_ALREADY_SET)
						return Tuple.Create(UpgradeResult.AlreadyFullAcc, string.Empty);

					if (response.errorid == BackendCodes.UPGRADE_USER_DUPLICATE_USERNAME)
						return Tuple.Create(UpgradeResult.UsernameTaken, string.Empty);

					SAMLog.Error("Backend::UU_ERR", $"UpgradeUser: Error {response.errorid}: {response.errormessage}");
					ShowErrorCommunication();
					return Tuple.Create(UpgradeResult.InternalError, response.errormessage);

				}
				else
				{
					ShowErrorCommunication();
					return Tuple.Create(UpgradeResult.InternalError, "Internal server error");
				}
			}
			catch (RestConnectionException e)
			{
				SAMLog.Warning("Backend::UU_RCE", e); // probably no internet
				ShowErrorConnection();
				return Tuple.Create(UpgradeResult.NoConnection, string.Empty);
			}
			catch (Exception e)
			{
				SAMLog.Error("Backend::UU_E", e);
				ShowErrorCommunication();
				return Tuple.Create(UpgradeResult.InternalError, "Internal server error");
			}
		}

		public async Task<Tuple<ChangePasswordResult, string>> ChangePassword(PlayerProfile profile, string newPassword)
		{
			try
			{
				var pwHashOld = profile.OnlinePasswordHash;
				var pwHashNew = bridge.DoSHA256(newPassword);

				var ps = new RestParameterSet();
				ps.AddParameterInt("userid", profile.OnlineUserID);
				ps.AddParameterHash("password_old", pwHashOld);
				ps.AddParameterString("app_version", GDConstants.Version.ToString());
				ps.AddParameterHash("password_new", pwHashNew);

				var response = await QueryAsync<QueryResultChangePassword>("change-password", ps, RETRY_CHANGE_PW);

				if (response == null)
				{
					ShowErrorCommunication();
					return Tuple.Create(ChangePasswordResult.InternalError, "Internal server error");
				}
				else if (response.result == "success")
				{
					MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
					{
						profile.AccountType = AccountType.Full;
						profile.OnlinePasswordHash = pwHashNew;
						profile.OnlineRevisionID = response.user.RevID;

						MainGame.Inst.SaveProfile();
					});

					return Tuple.Create(ChangePasswordResult.Success, string.Empty);
				}
				else if (response.result == "error")
				{
					if (response.errorid == BackendCodes.INTERNAL_EXCEPTION)
					{
						ShowErrorCommunication();
						return Tuple.Create(ChangePasswordResult.InternalError, response.errormessage);
					}

					if (response.errorid == BackendCodes.WRONG_PASSWORD)
						return Tuple.Create(ChangePasswordResult.AuthError, string.Empty);
					
					SAMLog.Error("Backend::CP_ERR", $"ChangePassword: Error {response.errorid}: {response.errormessage}");
					ShowErrorCommunication();
					return Tuple.Create(ChangePasswordResult.InternalError, response.errormessage);

				}
				else
				{
					return Tuple.Create(ChangePasswordResult.InternalError, "Inetrnal server error");
				}
			}
			catch (RestConnectionException e)
			{
				SAMLog.Warning("Backend::CP_RCE", e); // probably no internet
				ShowErrorConnection();
				return Tuple.Create(ChangePasswordResult.NoConnection, string.Empty);
			}
			catch (Exception e)
			{
				SAMLog.Error("Backend::CP_E", e);
				ShowErrorCommunication();
				return Tuple.Create(ChangePasswordResult.InternalError, "Internal server error");
			}
		}

		public async Task<QueryResultRanking> GetRanking(PlayerProfile profile, GraphBlueprint limit, bool multiplayer)
		{
			try
			{
				var ps = new RestParameterSet();
				ps.AddParameterInt("userid", profile.OnlineUserID);

				if (multiplayer)
					ps.AddParameterString("world_id", "@");
				else if (limit == null)
					ps.AddParameterString("world_id", "*");
				else
					ps.AddParameterString("world_id", limit.ID.ToString("B"));

				var response = await QueryAsync<QueryResultRanking>("get-ranking", ps, RETRY_GETRANKING);

				if (response == null)
				{
					ShowErrorCommunication();
					return null;
				}
				else if (response.result == "success")
				{
					return response;
				}
				else
				{
					ShowErrorCommunication();
					return null;
				}
			}
			catch (RestConnectionException e)
			{
				SAMLog.Warning("Backend::GR_RCE", e); // probably no internet
				ShowErrorConnection();
				return null;
			}
			catch (Exception e)
			{
				SAMLog.Error("Backend::GR_E", e);
				ShowErrorCommunication();
				return null;
			}
		}

		public async Task<Tuple<VerifyResult, string>> MergeLogin(PlayerProfile profile, string username, string password)
		{
			try
			{
				var pwHash = bridge.DoSHA256(password);

				var ps = new RestParameterSet();
				ps.AddParameterInt("old_userid", profile.OnlineUserID);
				ps.AddParameterHash("old_password", profile.OnlinePasswordHash);
				ps.AddParameterString("app_version", GDConstants.Version.ToString());
				ps.AddParameterString("new_username", username);
				ps.AddParameterHash("new_password", pwHash);
				ps.AddParameterJson("merge_data", CreateScoreArray(profile));

				var response = await QueryAsync<QueryResultMergeLogin>("merge-login", ps, RETRY_CREATEUSER);

				if (response == null)
				{
					return Tuple.Create(VerifyResult.InternalError, "Internal server error");
				}
				else if (response.result == "success")
				{
					MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
					{
						profile.AccountType = AccountType.Full;
						profile.OnlineUsername = response.user.Username;
						profile.OnlineUserID = response.user.ID;
						profile.OnlineRevisionID = response.user.RevID;
						profile.OnlinePasswordHash = pwHash;

						foreach (var scdata in response.scores)
						{
							profile.SetCompleted(Guid.Parse(scdata.levelid), (FractionDifficulty)scdata.difficulty, scdata.best_time, false);
						}

						MainGame.Inst.SaveProfile();
					});
					
					return Tuple.Create(VerifyResult.Success, string.Empty);
				}
				else if (response.result == "error")
				{
					if (response.errorid == BackendCodes.INTERNAL_EXCEPTION)
					{
						return Tuple.Create(VerifyResult.InternalError, response.errormessage);
					}
					else if (response.errorid == BackendCodes.WRONG_PASSWORD)
					{
						return Tuple.Create(VerifyResult.WrongPassword, string.Empty);
					}
					else if (response.errorid == BackendCodes.USER_BY_NAME_NOT_FOUND)
					{
						return Tuple.Create(VerifyResult.WrongUsername, string.Empty);
					}
					else
					{
						ShowErrorCommunication();
						SAMLog.Error("Backend::ML_ERR", $"Verify: Error {response.errorid}: {response.errormessage}");
						return Tuple.Create(VerifyResult.InternalError, response.errormessage);
					}
				}
				else
				{
					return Tuple.Create(VerifyResult.InternalError, "Internal server exception");
				}
			}
			catch (RestConnectionException e)
			{
				SAMLog.Warning("Backend::ML_RCE", e); // probably no internet
				ShowErrorConnection();
				return Tuple.Create(VerifyResult.NoConnection, string.Empty);
			}
			catch (Exception e)
			{
				SAMLog.Error("Backend::ML_E", e);
				ShowErrorCommunication();
				return Tuple.Create(VerifyResult.InternalError, "Internal server exception");
			}
		}

		private void ShowErrorConnection()
		{
			MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
			{
				MainGame.Inst.ShowToast("SAPI::CONERR", L10N.T(L10NImpl.STR_API_CONERR), 40, FlatColors.Flamingo, FlatColors.Foreground, 3f);
			});
		}

		private void ShowErrorCommunication()
		{
			MainGame.Inst?.ShowToast("SAPI::COMERR", L10N.T(L10NImpl.STR_API_COMERR), 40, FlatColors.Flamingo, FlatColors.Foreground, 1.5f);
		}

		private object CreateScoreArray(PlayerProfile profile)
		{
			var d = new List<Tuple<Guid, FractionDifficulty, int>>();

			foreach (var ld in profile.LevelData)
			{
				foreach (var dd in ld.Value.Data)
				{
					if (dd.Value.HasCompleted && dd.Key >= FractionDifficulty.DIFF_0 && dd.Key <= FractionDifficulty.DIFF_3)
					
					d.Add(Tuple.Create(ld.Key, dd.Key, dd.Value.BestTime));
				}
			}

			return d.Select(p => (object)new
			{
				levelid = p.Item1.ToString("B"),
				difficulty = (int)p.Item2,
				leveltime = p.Item3,
			}).ToArray();
		}
	}
}
