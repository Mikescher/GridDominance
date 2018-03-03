using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
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
using System.Text;
using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Shared.SCCM;
using MonoSAMFramework.Portable.Language;

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
		private const int RETRY_GETNEWLEVELID      = 6;
		private const int RETRY_LEVELUPLOAD        = 6;
		private const int RETRY_LEVELQUERY         = 6;
		private const int RETRY_LEVELDOWNLOAD      = 6;
		private const int RETRY_CUSTOMLEVELUPDATE  = 6;
		private const int RETRY_QUERYMETA          = 6;

		private const int MULTISCORE_PARTITION_SIZE = 64;

		private readonly IGDOperatingSystemBridge bridge;

		public GDServerAPI(IGDOperatingSystemBridge b) : base(GDConstants.SERVER_URL, GDConstants.SERVER_SECRET)
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
				ps.AddParameterString("app_type", bridge.AppType, false);

				var response = await QueryAsync<QueryResultPing>("ping", ps, RETRY_PING);

				if (response == null)
				{
					return; // meh - internal server error
				}
				else if (response.result == "success")
				{
					MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
					{
						var chngd = profile.UpdateSCCMData(response.user);
						if (chngd) MainGame.Inst.SaveProfile();
					});

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
					else if (response.errorid == BackendCodes.PARAMETER_HASH_MISMATCH)
					{
						SAMLog.Error("Backend::PING_SIGERR", $"Error {response.errorid}: {response.errormessage}\nSigInfo: {ps.GetDebugInfo(GetSigSecret())}");
					}
					else if (response.errorid == BackendCodes.WRONG_PASSWORD || response.errorid == BackendCodes.USER_BY_ID_NOT_FOUND)
					{
						MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
						{
							SAMLog.Error("Backend::PING_INVLOGIN", $"Local user cannot login on server ({response.errorid}:{response.errormessage}). (Type={profile.AccountType} | UID={profile.OnlineUserID} | Hash={profile.OnlinePasswordHash}) Reset local user");

							// something went horribly wrong
							// create new user on next run
							profile.ResetUserOnError();
							MainGame.Inst.SaveProfile();

							MainGame.Inst.Backend.CreateUser(MainGame.Inst.Profile).EnsureNoError();
						});
					}
					else
					{
						SAMLog.Error("Backend::PingError", $"Ping: Error {response.errorid}: {response.errormessage}");
					}
				}
				else
				{
					SAMLog.Error("Backend::PING_IRC", $"Ping: Invalid Result Code [{response.result}] {response.errorid}: {response.errormessage}");
					ShowErrorCommunication();
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
				ps.AddParameterString("app_type", bridge.AppType, false);

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
						profile.UpdateSCCMData(response.user);

						MainGame.Inst.SaveProfile();

						Reupload(profile).EnsureNoError();
					});

				}
				else if (response.result == "error")
				{
					SAMLog.Error("Backend::CU_Err", $"CreateUser: Error {response.errorid}: {response.errormessage}");
					ShowErrorCommunication();
				}
				else
				{
					SAMLog.Error("Backend::CU_IRC", $"CreateUser: Invalid Result Code [{response.result}] {response.errorid}: {response.errormessage}");
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

				ps.AddParameterInt("s0", profile.TotalPoints);
				ps.AddParameterInt("s1", profile.GetWorldPoints(Levels.WORLD_001));
				ps.AddParameterInt("s2", profile.GetWorldPoints(Levels.WORLD_002));
				ps.AddParameterInt("s3", profile.GetWorldPoints(Levels.WORLD_003));
				ps.AddParameterInt("s4", profile.GetWorldPoints(Levels.WORLD_004));
				ps.AddParameterInt("t0", profile.HighscoreTime);
				ps.AddParameterInt("t1", profile.GetWorldHighscoreTime(Levels.WORLD_001));
				ps.AddParameterInt("t2", profile.GetWorldHighscoreTime(Levels.WORLD_002));
				ps.AddParameterInt("t3", profile.GetWorldHighscoreTime(Levels.WORLD_003));
				ps.AddParameterInt("t4", profile.GetWorldHighscoreTime(Levels.WORLD_004));
				ps.AddParameterInt("sx", profile.MultiplayerPoints);

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
							profile.UpdateSCCMData(response.user);

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
					else if (response.errorid == BackendCodes.PARAMETER_HASH_MISMATCH)
					{
						SAMLog.Error("Backend::SS_SIGERR", $"Error {response.errorid}: {response.errormessage}\nSigInfo: {ps.GetDebugInfo(GetSigSecret())}");
					}
					else if (response.errorid == BackendCodes.WRONG_PASSWORD || response.errorid == BackendCodes.USER_BY_ID_NOT_FOUND)
					{
						MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
						{
							SAMLog.Error("Backend::SS_INVLOGIN", $"Local user cannot login on server ({response.errorid}:{response.errormessage}). (Type={profile.AccountType} | UID={profile.OnlineUserID} | Hash={profile.OnlinePasswordHash})  Reset local user");

							// something went horribly wrong
							// create new user on next run
							profile.ResetUserOnError();
							MainGame.Inst.SaveProfile();
						});
					}
					else
					{
						SAMLog.Error("Backend::SS_ERR", $"SetScore: Error {response.errorid}: {response.errormessage}");
					}
				}
				else
				{
					SAMLog.Error("Backend::SS_IRC", $"SetScore: Invalid Result Code [{response.result}] {response.errorid}: {response.errormessage}");
					ShowErrorCommunication();
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

		public async Task SetScoreAndTime(PlayerProfile profile)
		{
			try
			{
				var ps = new RestParameterSet();
				ps.AddParameterInt("userid", profile.OnlineUserID);
				ps.AddParameterHash("password", profile.OnlinePasswordHash);
				ps.AddParameterString("app_version", GDConstants.Version.ToString());

				ps.AddParameterInt("s0", profile.TotalPoints);
				ps.AddParameterInt("s1", profile.GetWorldPoints(Levels.WORLD_001));
				ps.AddParameterInt("s2", profile.GetWorldPoints(Levels.WORLD_002));
				ps.AddParameterInt("s3", profile.GetWorldPoints(Levels.WORLD_003));
				ps.AddParameterInt("s4", profile.GetWorldPoints(Levels.WORLD_004));
				ps.AddParameterInt("t0", profile.HighscoreTime);
				ps.AddParameterInt("t1", profile.GetWorldHighscoreTime(Levels.WORLD_001));
				ps.AddParameterInt("t2", profile.GetWorldHighscoreTime(Levels.WORLD_002));
				ps.AddParameterInt("t3", profile.GetWorldHighscoreTime(Levels.WORLD_003));
				ps.AddParameterInt("t4", profile.GetWorldHighscoreTime(Levels.WORLD_004));
				ps.AddParameterInt("sx", profile.MultiplayerPoints);

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
						profile.UpdateSCCMData(response.user);

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
					else if (response.errorid == BackendCodes.PARAMETER_HASH_MISMATCH)
					{
						SAMLog.Error("Backend::SMPS_SIGERR", $"Error {response.errorid}: {response.errormessage}\nSigInfo: {ps.GetDebugInfo(GetSigSecret())}");
					}
					else if (response.errorid == BackendCodes.WRONG_PASSWORD || response.errorid == BackendCodes.USER_BY_ID_NOT_FOUND)
					{
						MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
						{
							SAMLog.Error("Backend::SMPS_INVLOGIN", $"Local user cannot login on server ({response.errorid}:{response.errormessage}). (Type={profile.AccountType} | UID={profile.OnlineUserID} | Hash={profile.OnlinePasswordHash})  Reset local user");

							// something went horribly wrong
							// create new user on next run
							profile.ResetUserOnError();
							MainGame.Inst.SaveProfile();
						});
					}
					else
					{
						SAMLog.Error("Backend::SMPS_ERR", $"SetScoreAndTime: Error {response.errorid}: {response.errormessage}");
					}
				}
				else
				{
					SAMLog.Error("Backend::SMPS_IRC", $"SetScoreAndTime: Invalid Result Code [{response.result}] {response.errorid}: {response.errormessage}");
					ShowErrorCommunication();
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

		public async Task<bool> DownloadData(PlayerProfile profile)
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
					return false; // meh - internal server error
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

						profile.UpdateSCCMData(response.user);

						MainGame.Inst.SaveProfile();
					});

					return true;
				}
				else if (response.result == "error")
				{
					if (response.errorid == BackendCodes.INTERNAL_EXCEPTION)
					{
						return false; // meh
					}
					else if (response.errorid == BackendCodes.PARAMETER_HASH_MISMATCH)
					{
						SAMLog.Error("Backend::DD_SIGERR", $"Error {response.errorid}: {response.errormessage}\nSigInfo: {ps.GetDebugInfo(GetSigSecret())}");
					}
					else if (response.errorid == BackendCodes.WRONG_PASSWORD || response.errorid == BackendCodes.USER_BY_ID_NOT_FOUND)
					{
						MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
						{
							SAMLog.Error("Backend::DD_INVLOGIN", $"Local user cannot login on server ({response.errorid}:{response.errormessage}). (Type={profile.AccountType} | UID={profile.OnlineUserID} | Hash={profile.OnlinePasswordHash})  Reset local user");

							// something went horribly wrong
							// create new user on next run
							profile.ResetUserOnError();
							MainGame.Inst.SaveProfile();
						});
					}
					else
					{
						SAMLog.Error("Backend::DD_ERR", $"DownloadData: Error {response.errorid}: {response.errormessage}");
						ShowErrorCommunication();
					}

					return false;
				}
				else
				{
					SAMLog.Error("Backend::DD_IRC", $"DownloadData: Invalid Result Code [{response.result}] {response.errorid}: {response.errormessage}");
					ShowErrorCommunication();

					return false;
				}
			}
			catch (RestConnectionException e)
			{
				SAMLog.Warning("Backend::DD_RCE", e); // probably no internet
				ShowErrorConnection();

				return false;
			}
			catch (Exception e)
			{
				SAMLog.Error("Backend::DD_E", e);
				ShowErrorCommunication();

				return false;
			}
		}

		public async Task<bool> Reupload(PlayerProfile profile)
		{
			profile.NeedsReupload = false;

			var sarray = CreateScoreStrings(profile, MULTISCORE_PARTITION_SIZE).ToList();

			bool ok = true;

			foreach (var sarr in sarray)
			{
				bool r = await ReuploadMulti(profile, sarr);
				if (!r) ok = false;
			}
			
			return ok;
		}

		private async Task<bool> ReuploadMulti(PlayerProfile profile, string data)
		{
			try
			{
				var ps = new RestParameterSet();
				ps.AddParameterInt("userid", profile.OnlineUserID);
				ps.AddParameterHash("password", profile.OnlinePasswordHash);
				ps.AddParameterString("app_version", GDConstants.Version.ToString());

				ps.AddParameterBigCompressed("data", data);

				ps.AddParameterJson("data-length", data.Length, false);

				ps.AddParameterInt("s0", profile.TotalPoints);
				ps.AddParameterInt("s1", profile.GetWorldPoints(Levels.WORLD_001));
				ps.AddParameterInt("s2", profile.GetWorldPoints(Levels.WORLD_002));
				ps.AddParameterInt("s3", profile.GetWorldPoints(Levels.WORLD_003));
				ps.AddParameterInt("s4", profile.GetWorldPoints(Levels.WORLD_004));
				ps.AddParameterInt("t0", profile.HighscoreTime);
				ps.AddParameterInt("t1", profile.GetWorldHighscoreTime(Levels.WORLD_001));
				ps.AddParameterInt("t2", profile.GetWorldHighscoreTime(Levels.WORLD_002));
				ps.AddParameterInt("t3", profile.GetWorldHighscoreTime(Levels.WORLD_003));
				ps.AddParameterInt("t4", profile.GetWorldHighscoreTime(Levels.WORLD_004));
				ps.AddParameterInt("sx", profile.MultiplayerPoints);

				var response = await QueryAsync<QueryResultSetMultiscore>("set-multiscore-2", ps, RETRY_DOWNLOADDATA);

				if (response == null)
				{
					MonoSAMGame.CurrentInst.DispatchBeginInvoke(() => { profile.NeedsReupload = true; MainGame.Inst.SaveProfile(); });

					return false; // meh - internal server error
				}
				else if (response.result == "success")
				{
					if (response.update)
					{
						MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
						{
							profile.OnlineRevisionID = response.user.RevID;
							profile.UpdateSCCMData(response.user);

							MainGame.Inst.SaveProfile();

							if (profile.NeedsReupload) Reupload(profile).EnsureNoError();
						});
					}

					return true;
				}
				else if (response.result == "error")
				{
					ShowErrorCommunication();

					MonoSAMGame.CurrentInst.DispatchBeginInvoke(() => { profile.NeedsReupload = true; MainGame.Inst.SaveProfile(); });

					if (response.errorid == BackendCodes.INTERNAL_EXCEPTION)
					{
						return false; // meh
					}
					else if (response.errorid == BackendCodes.PARAMETER_HASH_MISMATCH)
					{
						SAMLog.Error("Backend::RU_SIGERR", $"Error {response.errorid}: {response.errormessage}\nSigInfo: {ps.GetDebugInfo(GetSigSecret())}");
					}
					else if (response.errorid == BackendCodes.WRONG_PASSWORD || response.errorid == BackendCodes.USER_BY_ID_NOT_FOUND)
					{
						MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
						{
							SAMLog.Error("Backend::RU_INVLOGIN", $"Local user cannot login on server ({response.errorid}:{response.errormessage}). (Type={profile.AccountType} | UID={profile.OnlineUserID} | Hash={profile.OnlinePasswordHash})  Reset local user");

							// something went horribly wrong
							// create new user on next run
							profile.ResetUserOnError();
							MainGame.Inst.SaveProfile();
						});
					}
					else
					{
						SAMLog.Error("Backend::RU_ERR", $"Reupload: Error {response.errorid}: {response.errormessage}");
					}

					return false;
				}
				else
				{
					SAMLog.Error("Backend::RU_IRC", $"Reupload: Invalid Result Code [{response.result}] {response.errorid}: {response.errormessage}");
					ShowErrorCommunication();

					MonoSAMGame.CurrentInst.DispatchBeginInvoke(() => { profile.NeedsReupload = true; MainGame.Inst.SaveProfile(); });

					return false;
				}
			}
			catch (RestConnectionException e)
			{
				SAMLog.Warning("Backend::RU_RCE", e); // probably no internet
				ShowErrorConnection();

				MonoSAMGame.CurrentInst.DispatchBeginInvoke(() => { profile.NeedsReupload = true; MainGame.Inst.SaveProfile(); });

				return false;
			}
			catch (Exception e)
			{
				SAMLog.Error("Backend::RU_E", e);
				ShowErrorCommunication();

				MonoSAMGame.CurrentInst.DispatchBeginInvoke(() => { profile.NeedsReupload = true; MainGame.Inst.SaveProfile(); });

				return false;
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

								if (d.HasCompleted && d.BestTime < d.GlobalBestTime && profile.AccountType != AccountType.Local)
								{
									// Fake local data until server updates (cronjob)

									d.GlobalBestTime = d.BestTime;
									d.GlobalBestUserID = profile.OnlineUserID;
									d.GlobalCompletionCount = hscore.completion_count + 1;
								}
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
				else
				{
					SAMLog.Error("Backend::DH_IRC", $"DownloadHighscore: Invalid Result Code [{response.result}] {response.errorid}: {response.errormessage}");
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
					else if (response.errorid == BackendCodes.PARAMETER_HASH_MISMATCH)
					{
						ShowErrorCommunication();
						SAMLog.Error("Backend::V_SIGERR", $"Error {response.errorid}: {response.errormessage}\nSigInfo: {ps.GetDebugInfo(GetSigSecret())}");
						return Tuple.Create(VerifyResult.InternalError, -1, response.errormessage);
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
					SAMLog.Error("Backend::V_IRC", $"Verify: Invalid Result Code [{response.result}] {response.errorid}: {response.errormessage}");
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
						profile.AccountReminderShown = true;
						profile.UpdateSCCMData(response.user);

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
					SAMLog.Error("Backend::UU_IRC", $"UpgradeUser: Invalid Result Code [{response.result}] {response.errorid}: {response.errormessage}");
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
						profile.UpdateSCCMData(response.user);

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
					SAMLog.Error("Backend::CP_IRC", $"ChangePassword: Invalid Result Code [{response.result}] {response.errorid}: {response.errormessage}");
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

		public async Task<QueryResultRanking> GetRanking(PlayerProfile profile, GraphBlueprint limit, HighscoreCategory cat)
		{
			try
			{
				var ps = new RestParameterSet();
				ps.AddParameterInt("userid", profile.OnlineUserID);

				switch (cat)
				{
					case HighscoreCategory.GlobalPoints:
						ps.AddParameterString("world_id", "*");
						break;

					case HighscoreCategory.WorldPoints:
						ps.AddParameterString("world_id", limit.ID.ToString("B"));
						break;

					case HighscoreCategory.MultiplayerPoints:
						ps.AddParameterString("world_id", "@");
						break;

					case HighscoreCategory.CustomLevelStars:
						ps.AddParameterString("world_id", "$");
						break;

					case HighscoreCategory.CustomLevelPoints:
						ps.AddParameterString("world_id", "#");
						break;

					default:
						SAMLog.Error("Backend::EnumSwitch_GR", $"cat = {cat}");
						break;
				}
				
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
					SAMLog.Error("Backend::GR_IRC", $"GetRanking: Invalid Result Code [{response.result}] {response.errorid}: {response.errormessage}");
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
				ps.AddParameterJsonBigCompressed("merge_data", CreateScoreArray(profile, null));

				ps.AddParameterInt("s0", profile.TotalPoints);
				ps.AddParameterInt("s1", profile.GetWorldPoints(Levels.WORLD_001));
				ps.AddParameterInt("s2", profile.GetWorldPoints(Levels.WORLD_002));
				ps.AddParameterInt("s3", profile.GetWorldPoints(Levels.WORLD_003));
				ps.AddParameterInt("s4", profile.GetWorldPoints(Levels.WORLD_004));
				ps.AddParameterInt("t0", profile.HighscoreTime);
				ps.AddParameterInt("t1", profile.GetWorldHighscoreTime(Levels.WORLD_001));
				ps.AddParameterInt("t2", profile.GetWorldHighscoreTime(Levels.WORLD_002));
				ps.AddParameterInt("t3", profile.GetWorldHighscoreTime(Levels.WORLD_003));
				ps.AddParameterInt("t4", profile.GetWorldHighscoreTime(Levels.WORLD_004));
				ps.AddParameterInt("sx", profile.MultiplayerPoints);

				var response = await QueryAsync<QueryResultMergeLogin>("merge-login-2", ps, RETRY_CREATEUSER);

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
						profile.AccountReminderShown = true;

						foreach (var scdata in response.scores)
						{
							profile.SetCompleted(Guid.Parse(scdata.levelid), (FractionDifficulty)scdata.difficulty, scdata.best_time, false);
						}

						profile.UpdateSCCMData(response.user);

						MainGame.Inst.SaveProfile();

						SetScoreAndTime(profile).EnsureNoError(); //score could have been changed - reupload
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
						SAMLog.Error("Backend::ML_ERR", $"MergeLogin: Error {response.errorid}: {response.errormessage}");
						return Tuple.Create(VerifyResult.InternalError, response.errormessage);
					}
				}
				else
				{
					SAMLog.Error("Backend::ML_IRC", $"MergeLogin: Invalid Result Code [{response.result}] {response.errorid}: {response.errormessage}");
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

		public async Task<Tuple<bool, long>> GetNewCustomLevelID(PlayerProfile profile)
		{
			try
			{
				var ps = new RestParameterSet();
				ps.AddParameterInt("userid", profile.OnlineUserID);
				ps.AddParameterHash("password", profile.OnlinePasswordHash);
				ps.AddParameterString("app_version", GDConstants.Version.ToString());

				var response = await QueryAsync<QueryResultNewLevelID>("get-newlevelid", ps, RETRY_GETNEWLEVELID);

				if (response == null)
				{
					ShowErrorCommunication();
					return Tuple.Create(false, -1L);
				}
				else if (response.result == "success")
				{
					return Tuple.Create(true, response.levelid);
				}
				else
				{
					SAMLog.Error("Backend::GNCLI_IRC", $"GetNewCustomLevelID: Invalid Result Code [{response.result}] {response.errorid}: {response.errormessage}");
					ShowErrorCommunication();
					return Tuple.Create(false, -1L);
				}
			}
			catch (RestConnectionException e)
			{
				SAMLog.Warning("Backend::GNCLI_RCE", e); // probably no internet
				ShowErrorConnection();
				return Tuple.Create(false, -1L);
			}
			catch (Exception e)
			{
				SAMLog.Error("Backend::GNCLI_E", e);
				ShowErrorCommunication();
				return Tuple.Create(false, -1L);
			}
		}

		public async Task<UploadResult> UploadUserLevel(PlayerProfile profile, LevelBlueprint level, SCCMLevelData rawData, byte[] binary, int time)
		{
			try
			{
				var ps = new RestParameterSet();
				ps.AddParameterInt("userid", profile.OnlineUserID);
				ps.AddParameterHash("password", profile.OnlinePasswordHash);
				ps.AddParameterString("app_version", GDConstants.Version.ToString());
				ps.AddParameterULong("app_version_dec", GDConstants.LevelIntVersion);
				ps.AddParameterLong("levelid", rawData.OnlineID);
				ps.AddParameterString("name", level.FullName);
				ps.AddParameterInt("gwidth", rawData.Size.Width);
				ps.AddParameterInt("gheight", rawData.Size.Height);
				ps.AddParameterInt("authortime", time);
				ps.AddParameterString("binhash", ByteUtils.ByteToHexBitFiddle(MonoSAMGame.CurrentInst.Bridge.DoSHA256(binary)));
				ps.AddParameterCompressedBinary("bindata", binary, false, true);

				var response = await QueryAsync<QueryResultUploadUserLevel>("upload-userlevel", ps, RETRY_LEVELUPLOAD);

				if (response == null)
				{
					ShowErrorCommunication();
					return UploadResult.NoConnection;
				}
				else if (response.result == "error")
				{
					if (response.errorid == BackendCodes.INTERNAL_EXCEPTION)
					{
						ShowErrorCommunication();
						return UploadResult.InternalError;
					}

					if (response.errorid == BackendCodes.LEVELUPLOAD_ALREADY_UPLOADED) return UploadResult.AlreadyUploaded;
					if (response.errorid == BackendCodes.LEVELUPLOAD_FILE_TOO_BIG) return UploadResult.FileTooBig;
					if (response.errorid == BackendCodes.LEVELUPLOAD_HASH_MISMATCH) return UploadResult.InternalError;
					if (response.errorid == BackendCodes.LEVELUPLOAD_INVALID_NAME) return UploadResult.InvalidName;
					if (response.errorid == BackendCodes.LEVELUPLOAD_LEVELID_NOT_FOUND) return UploadResult.LevelIDNotFound;
					if (response.errorid == BackendCodes.LEVELUPLOAD_WRONG_USERID) return UploadResult.WrongUserID;
					if (response.errorid == BackendCodes.LEVELUPLOAD_DUPLICATENAME) return UploadResult.DuplicateName;

					SAMLog.Error("Backend::UUL_ERR", $"UploadUserLevel: Error {response.errorid}: {response.errormessage}");
					ShowErrorCommunication();
					return UploadResult.InternalError;
				}
				else if (response.result == "success")
				{
					return UploadResult.Success;
				}
				else
				{
					SAMLog.Error("Backend::UUL_IRC", $"UploadUserLevel: Invalid Result Code [{response.result}] {response.errorid}: {response.errormessage}");
					ShowErrorCommunication();
					return UploadResult.InternalError;
				}
			}
			catch (RestConnectionException e)
			{
				SAMLog.Warning("Backend::UUL_RCE", e); // probably no internet
				ShowErrorConnection();
				return UploadResult.NoConnection;
			}
			catch (Exception e)
			{
				SAMLog.Error("Backend::UUL_E", e);
				ShowErrorCommunication();
				return UploadResult.InternalError;
			}
		}

		public async Task<List<SCCMLevelMeta>> QueryUserLevel(PlayerProfile profile, QueryUserLevelCategory cat, string param, int pagination)
		{
			try
			{
				var ps = new RestParameterSet();
				ps.AddParameterInt("userid", profile.OnlineUserID);
				ps.AddParameterString("category", QueryUserLevelCategoryHelper.EnumToString(cat));
				ps.AddParameterString("param", param);
				ps.AddParameterInt("pagination", pagination);
				ps.AddParameterString("app_version", GDConstants.Version.ToString());
				ps.AddParameterULong("app_version_dec", GDConstants.LevelIntVersion);

				var response = await QueryAsync<QueryResultQueryUserLevel>("query-userlevels", ps, RETRY_LEVELQUERY);

				if (response == null)
				{
					ShowErrorCommunication();
					return null;
				}
				else if (response.result == "error")
				{
					if (response.errorid == BackendCodes.INTERNAL_EXCEPTION)
					{
						ShowErrorCommunication();
						return null;
					}

					SAMLog.Error("Backend::QUL_ERR", $"QueryUserLevel: Error {response.errorid}: {response.errormessage}");
					ShowErrorCommunication();
					return null;
				}
				else if (response.result == "success")
				{

					return response.data.Select(SCCMLevelMeta.Parse).ToList();
				}
				else
				{
					SAMLog.Error("Backend::QUL_IRC", $"QueryUserLevel: Invalid Result Code [{response.result}] {response.errorid}: {response.errormessage}");
					ShowErrorCommunication();
					return null;
				}
			}
			catch (RestConnectionException e)
			{
				SAMLog.Warning("Backend::QUL_RCE", e); // probably no internet
				ShowErrorConnection();
				return null;
			}
			catch (Exception e)
			{
				SAMLog.Error("Backend::QUL_E", e);
				ShowErrorCommunication();
				return null;
			}
		}

		public async Task<byte[]> DownloadUserLevel(PlayerProfile profile, long onlineID)
		{
			try
			{
				var ps = new RestParameterSet();
				ps.AddParameterInt("userid", profile.OnlineUserID);
				ps.AddParameterHash("password", profile.OnlinePasswordHash);
				ps.AddParameterLong("levelid", onlineID);
				ps.AddParameterString("app_version", GDConstants.Version.ToString());
				ps.AddParameterULong("app_version_dec", GDConstants.LevelIntVersion);

				var response = await QueryAsync<QueryResultDownloadUserLevel>("download-userlevel", ps, RETRY_LEVELDOWNLOAD);

				if (response == null)
				{
					ShowErrorCommunication();
					return null;
				}
				else if (response.result == "error")
				{
					if (response.errorid == BackendCodes.INTERNAL_EXCEPTION)
					{
						ShowErrorCommunication();
						return null;
					}

					SAMLog.Error("Backend::DUL_ERR", $"QueryUserLevel: Error {response.errorid}: {response.errormessage}");
					ShowErrorCommunication();
					return null;
				}
				else if (response.result == "success")
				{
					return Convert.FromBase64String(response.content);
				}
				else
				{
					SAMLog.Error("Backend::DUL_IRC", $"DownloadUserLevel: Invalid Result Code [{response.result}] {response.errorid}: {response.errormessage}");
					ShowErrorCommunication();
					return null;
				}
			}
			catch (RestConnectionException e)
			{
				SAMLog.Warning("Backend::DUL_RCE", e); // probably no internet
				ShowErrorConnection();
				return null;
			}
			catch (Exception e)
			{
				SAMLog.Error("Backend::DUL_E", e);
				ShowErrorCommunication();
				return null;
			}
		}

		public async Task<bool?> SetCustomLevelPlayed(PlayerProfile profile, long onlineID, FractionDifficulty diff)
		{
			try
			{
				var ps = new RestParameterSet();
				ps.AddParameterInt("userid", profile.OnlineUserID);
				ps.AddParameterHash("password", profile.OnlinePasswordHash);
				ps.AddParameterString("app_version", GDConstants.Version.ToString());
				ps.AddParameterLong("levelid", onlineID);
				ps.AddParameterLong("difficulty", (int)diff);

				var response = await QueryAsync<QueryResultUpdateUserLevelPlayed>("update-userlevel-played", ps, RETRY_CUSTOMLEVELUPDATE);

				if (response == null)
				{
					ShowErrorCommunication();
					return null;
				}
				else if (response.result == "error")
				{
					SAMLog.Error("Backend::SCLP_ERR", $"SetCustomLevelPlayed: Error {response.errorid}: {response.errormessage}");
					ShowErrorCommunication();
					return null;
				}
				else if (response.result == "success")
				{
					MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
					{
						var chngd = profile.UpdateSCCMData(response.user);
						if (chngd) MainGame.Inst.SaveProfile();
					});

					return response.updated;
				}
				else
				{
					SAMLog.Error("Backend::SCLP_IRC", $"SetCustomLevelPlayed: Invalid Result Code [{response.result}] {response.errorid}: {response.errormessage}");
					ShowErrorCommunication();
					return null;
				}
			}
			catch (RestConnectionException e)
			{
				SAMLog.Warning("Backend::SCLP_RCE", e); // probably no internet
				ShowErrorConnection();
				return null;
			}
			catch (Exception e)
			{
				SAMLog.Error("Backend::SCLP_E", e);
				ShowErrorCommunication();
				return null;
			}
		}

		public async Task<CustomLevelCompletionResult> SetCustomLevelCompleted(PlayerProfile profile, long onlineID, FractionDifficulty diff, int time)
		{
			try
			{
				var ps = new RestParameterSet();
				ps.AddParameterInt("userid", profile.OnlineUserID);
				ps.AddParameterHash("password", profile.OnlinePasswordHash);
				ps.AddParameterString("app_version", GDConstants.Version.ToString());
				ps.AddParameterLong("levelid", onlineID);
				ps.AddParameterLong("difficulty", (int)diff);
				ps.AddParameterLong("time", time);

				var response = await QueryAsync<QueryResultUpdateUserLevelCompleted>("update-userlevel-completed", ps, RETRY_CUSTOMLEVELUPDATE);

				if (response == null)
				{
					ShowErrorCommunication();
					return CustomLevelCompletionResult.CreateError();
				}
				else if (response.result == "error")
				{
					SAMLog.Error("Backend::SCLC_ERR", $"SetCustomLevelPlayed: Error {response.errorid}: {response.errormessage}");
					ShowErrorCommunication();
					return CustomLevelCompletionResult.CreateError();
				}
				else if (response.result == "success")
				{
					MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
					{
						profile.UpdateSCCMData(response.user);

						var realTime = response.leveltime;
						profile.SetCustomLevelCompleted(onlineID, diff, realTime);

						MainGame.Inst.SaveProfile();
					});

					return CustomLevelCompletionResult.Parse(response);
				}
				else
				{
					SAMLog.Error("Backend::SCLC_IRC", $"SetCustomLevelCompleted: Invalid Result Code [{response.result}] {response.errorid}: {response.errormessage}");
					ShowErrorCommunication();
					return CustomLevelCompletionResult.CreateError();
				}
			}
			catch (RestConnectionException e)
			{
				SAMLog.Warning("Backend::SCLC_RCE", e); // probably no internet
				ShowErrorConnection();
				return CustomLevelCompletionResult.CreateError();
			}
			catch (Exception e)
			{
				SAMLog.Error("Backend::SCLC_E", e);
				ShowErrorCommunication();
				return CustomLevelCompletionResult.CreateError();
			}
		}
		
		public async Task<Tuple<int, bool>> SetCustomLevelStarred(PlayerProfile profile, long onlineID, bool star) // <level_starcount, isStarred>
		{
			try
			{
				var ps = new RestParameterSet();
				ps.AddParameterInt("userid", profile.OnlineUserID);
				ps.AddParameterHash("password", profile.OnlinePasswordHash);
				ps.AddParameterString("app_version", GDConstants.Version.ToString());
				ps.AddParameterLong("levelid", onlineID);
				ps.AddParameterBool("star", star);

				var response = await QueryAsync<QueryResultUpdateUserLevelStarred>("update-userlevel-starred", ps, RETRY_CUSTOMLEVELUPDATE);

				if (response == null)
				{
					ShowErrorCommunication();
					return null;
				}
				else if (response.result == "error")
				{
					SAMLog.Error("Backend::SCLS_ERR", $"SetCustomLevelPlayed: Error {response.errorid}: {response.errormessage}");
					ShowErrorCommunication();
					return null;
				}
				else if (response.result == "success")
				{
					MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
					{
						profile.UpdateSCCMData(response.user);
						profile.SetCustomLevelStarred(onlineID, response.value);

						MainGame.Inst.SaveProfile();
					});

					return Tuple.Create(response.stars, response.value);
				}
				else
				{
					SAMLog.Error("Backend::SCLS_IRC", $"SetCustomLevelStarred: Invalid Result Code [{response.result}] {response.errorid}: {response.errormessage}");
					ShowErrorCommunication();
					return null;
				}
			}
			catch (RestConnectionException e)
			{
				SAMLog.Warning("Backend::SCLS_RCE", e); // probably no internet
				ShowErrorConnection();
				return null;
			}
			catch (Exception e)
			{
				SAMLog.Error("Backend::SCLS_E", e);
				ShowErrorCommunication();
				return null;
			}
		}
		
		public async Task<SCCMLevelMeta> QueryUserLevelMeta(PlayerProfile profile, long onlineID)
		{
			try
			{
				var ps = new RestParameterSet();
				ps.AddParameterInt("userid", profile.OnlineUserID);
				ps.AddParameterHash("password", profile.OnlinePasswordHash);
				ps.AddParameterString("app_version", GDConstants.Version.ToString());
				ps.AddParameterLong("levelid", onlineID);

				var response = await QueryAsync<QueryResultQueryUserLevelMeta>("query-userlevel-meta", ps, RETRY_QUERYMETA);

				if (response == null)
				{
					ShowErrorCommunication();
					return null;
				}
				else if (response.result == "error")
				{
					SAMLog.Error("Backend::QULM_ERR", $"QueryUserLevelMeta: Error {response.errorid}: {response.errormessage}");
					ShowErrorCommunication();
					return null;
				}
				else if (response.result == "success")
				{
					return SCCMLevelMeta.Parse(response.data);
				}
				else
				{
					SAMLog.Error("Backend::QULM_IRC", $"QueryUserLevelMeta: Invalid Result Code [{response.result}] {response.errorid}: {response.errormessage}");
					ShowErrorCommunication();
					return null;
				}
			}
			catch (RestConnectionException e)
			{
				SAMLog.Warning("Backend::QULM_RCE", e); // probably no internet
				ShowErrorConnection();
				return null;
			}
			catch (Exception e)
			{
				SAMLog.Error("Backend::QULM_E", e);
				ShowErrorCommunication();
				return null;
			}
		}

		private void ShowErrorConnection()
		{
			MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
			{
				MainGame.Inst?.ShowToast("SAPI::CONERR", L10N.T(L10NImpl.STR_API_CONERR), 40, FlatColors.Flamingo, FlatColors.Foreground, 3f);
			});
		}

		private void ShowErrorCommunication()
		{
			MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
			{
				MainGame.Inst?.ShowToast("SAPI::COMERR", L10N.T(L10NImpl.STR_API_COMERR), 40, FlatColors.Flamingo, FlatColors.Foreground, 1.5f);
			});
		}

		private IEnumerable<string> CreateScoreStrings(PlayerProfile profile, int partitionSize)
		{
			var d = new StringBuilder();

			int c = 0;

			foreach (var ld in profile.LevelData)
			{
				if (!ld.Value.HasAnyCompleted()) continue;

				d.Append(ld.Key.ToString("N"));
				d.Append('@');

				if (ld.Value.HasCompletedExact(FractionDifficulty.DIFF_0)) { d.Append(ld.Value.Data[FractionDifficulty.DIFF_0].BestTime); c++; }
				d.Append(';');
				if (ld.Value.HasCompletedExact(FractionDifficulty.DIFF_1)) { d.Append(ld.Value.Data[FractionDifficulty.DIFF_1].BestTime); c++; }
				d.Append(';');
				if (ld.Value.HasCompletedExact(FractionDifficulty.DIFF_2)) { d.Append(ld.Value.Data[FractionDifficulty.DIFF_2].BestTime); c++; }
				d.Append(';');
				if (ld.Value.HasCompletedExact(FractionDifficulty.DIFF_3)) { d.Append(ld.Value.Data[FractionDifficulty.DIFF_3].BestTime); c++; }

				d.Append('\n');

				if (c > partitionSize)
				{
					yield return d.ToString();
					d.Clear();
					c = 0;
				}
			}

			if (c > 0)
			{
				yield return d.ToString();
			}
		}

		private object[] CreateScoreArray(PlayerProfile profile, Guid? world)
		{
			var d = new List<Tuple<Guid, FractionDifficulty, int>>();

			foreach (var ld in profile.LevelData)
			{
				if (world != null && Levels.MAP_LEVELS_WORLDS.TryGetValue(ld.Key, out var levelworld) && levelworld != world.Value) continue;

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
