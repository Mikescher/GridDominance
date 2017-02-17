using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GridDominance.Shared.Network.Backend;
using GridDominance.Shared.Resources;
using GridDominance.Shared.SaveData;
using GridDominance.Shared.Screens.ScreenGame.Fractions;
using MonoSAMFramework.Portable;
using MonoSAMFramework.Portable.DeviceBridge;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.LogProtocol;
using MonoSAMFramework.Portable.Network.REST;

namespace GridDominance.Shared.Network
{
	public class GDServerAPI : SAMRestAPI
	{
		private const int RETRY_PING               = 2;
		private const int RETRY_SETSCORE           = 4;
		private const int RETRY_DOWNLOADDATA       = 4;
		private const int RETRY_LOGERROR           = 4;
		private const int RETRY_DOWNLOADHIGHSCORES = 4;
		private const int RETRY_CREATEUSER         = 6;

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

				var response = await QueryAsync<QueryResultPing>("ping", ps, RETRY_PING);

				DownloadHighscores(profile).EnsureNoError();

				if (response.result == "success")
				{
					if (response.user.RevID > profile.OnlineRevisionID)
					{
						await DownloadData(profile);

						if (profile.NeedsReupload) Reupload(profile);
					}
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
							SAMLog.Error("Backend", $"Local user cannot login on server ({response.errorid}:{response.errormessage}). Reset local user");

							// something went horribly wrong
							// create new user on next run
							profile.OnlineUserID = -1;

							MainGame.Inst.SaveProfile();
						});
					}
					else
					{
						SAMLog.Error("Backend", $"Ping: Error {response.errorid}: {response.errormessage}");
					}
				}
			}
			catch (RestConnectionException e)
			{
				SAMLog.Warning("Backend", e); // probably no internet
			}
			catch (Exception e)
			{
				SAMLog.Error("Backend", e);
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

				var response = await QueryAsync<QueryResultCreateUser>("create-user", ps, RETRY_CREATEUSER);

				if (response.result == "success")
				{
					MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
					{
						profile.OnlineUserID = response.user.ID;
						profile.OnlineRevisionID = response.user.RevID;
						profile.OnlinePasswordHash = pw;

						MainGame.Inst.SaveProfile();

						Reupload(profile);
					});

				}
				else if (response.result == "error")
				{
					//TODO react on duplicate username etc
					SAMLog.Error("Backend", $"CreateUser: Error {response.errorid}: {response.errormessage}");
				}
			}
			catch (RestConnectionException e)
			{
				SAMLog.Warning("Backend", e); // probably no internet
			}
			catch (Exception e)
			{
				SAMLog.Error("Backend", e);
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

				if (response.result == "success")
				{
					if (response.update)
					{
						MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
						{
							profile.OnlineRevisionID = response.user.RevID;

							MainGame.Inst.SaveProfile();

							if (profile.NeedsReupload) Reupload(profile);
						});
					}
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
							SAMLog.Error("Backend", $"Local user cannot login on server ({response.errorid}:{response.errormessage}). Reset local user");

							// something went horribly wrong
							// create new user on next run
							profile.OnlineUserID = -1;

							MainGame.Inst.SaveProfile();
						});
					}
					else
					{
						SAMLog.Error("Backend", $"SetScore: Error {response.errorid}: {response.errormessage}");
					}
				}
			}
			catch (RestConnectionException e)
			{
				SAMLog.Warning("Backend", e); // probably no internet

				MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
				{
					profile.NeedsReupload = true;

					MainGame.Inst.SaveProfile();
				});
			}
			catch (Exception e)
			{
				SAMLog.Error("Backend", e);

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

				if (response.result == "success")
				{
					MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
					{
						profile.OnlineRevisionID = response.user.RevID;

						foreach (var scdata in response.scores)
						{
							profile.SetCompleted(Guid.Parse(scdata.levelid), (FractionDifficulty)scdata.difficulty, scdata.best_time, false);
						}

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
							SAMLog.Error("Backend", $"Local user cannot login on server ({response.errorid}:{response.errormessage}). Reset local user");

							// something went horribly wrong
							// create new user on next run
							profile.OnlineUserID = -1;

							MainGame.Inst.SaveProfile();
						});
					}
					else
					{
						SAMLog.Error("Backend", $"SetScore: Error {response.errorid}: {response.errormessage}");
					}
				}
			}
			catch (RestConnectionException e)
			{
				SAMLog.Warning("Backend", e); // probably no internet
			}
			catch (Exception e)
			{
				SAMLog.Error("Backend", e);
			}
		}

		private async void Reupload(PlayerProfile profile)
		{
			profile.NeedsReupload = false;

			try
			{
				List<Task> scoretasks = new List<Task>();

				foreach (var lvldata in profile.LevelData)
				{
					foreach (var diff in lvldata.Value.Data.Where(p => p.Value.HasCompleted))
					{
						scoretasks.Add(SetScore(profile, lvldata.Key, diff.Key, diff.Value.BestTime));
					}
				}

				await Task.WhenAll(scoretasks);

				MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
				{
					profile.NeedsReupload = false;

					MainGame.Inst.SaveProfile();
				});
			}
			catch (RestConnectionException e)
			{
				SAMLog.Warning("Backend", e); // probably no internet
				MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
				{
					profile.NeedsReupload = false;

					MainGame.Inst.SaveProfile();
				});
			}
			catch (Exception e)
			{
				SAMLog.Error("Backend", e);
				MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
				{
					profile.NeedsReupload = false;

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
				ps.AddParameterString("screen_resolution", bridge.ScreenResolution, false);
				ps.AddParameterString("exception_id", entry.Type, false);
				ps.AddParameterCompressed("exception_message", entry.MessageShort, false);
				ps.AddParameterCompressed("exception_stacktrace", entry.MessageLong, false);
				ps.AddParameterCompressed("additional_info", bridge.FullDeviceInfoString, false);

				var response = await QueryAsync<QueryResultDownloadData>("log-client", ps, RETRY_LOGERROR);

				if (response.result == "error")
				{
					SAMLog.Warning("Log_Upload", response.errormessage);
				}
			}
			catch (RestConnectionException e)
			{
				// well, that sucks
				// probably no internet
				SAMLog.Warning("Backend", e); // probably no internet
			}
			catch (Exception e)
			{
				SAMLog.Error("Backend", e);
			}
		}
		
		public async Task DownloadHighscores(PlayerProfile profile)
		{
			try
			{
				var response = await QueryAsync<QueryResultHighscores>("get-highscores", new RestParameterSet(), RETRY_DOWNLOADHIGHSCORES);

				if (response.result == "success")
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
					SAMLog.Warning("Backend", $"DownloadHighscores: Error {response.errorid}: {response.errormessage}");
				}
			}
			catch (RestConnectionException e)
			{
				SAMLog.Warning("Backend", e); // probably no internet
			}
			catch (Exception e)
			{
				SAMLog.Error("Backend", e);
			}
		}
	}
}
