using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GridDominance.Shared.Network.Backend;
using GridDominance.Shared.Resources;
using GridDominance.Shared.SaveData;
using GridDominance.Shared.Screens.ScreenGame.Fractions;
using MonoSAMFramework.Portable;
using MonoSAMFramework.Portable.DeviceBridge;
using MonoSAMFramework.Portable.LogProtocol;
using MonoSAMFramework.Portable.Network.REST;

namespace GridDominance.Shared.Network
{
	public class GDServerAPI : SAMRestAPI
	{
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

				var response = await QueryAsync<QueryResultPing>("ping", ps);

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
			catch (Exception e)
			{
				SAMLog.Warning("Backend", e); // probably no internet
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

				var response = await QueryAsync<QueryResultCreateUser>("create-user", ps);

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
			catch (Exception e)
			{
				SAMLog.Warning("Backend", e); // probably no internet
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

				var response = await QueryAsync<QueryResultSetScore>("set-score", ps);

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
			catch (Exception e)
			{
				SAMLog.Warning("Backend", e); // probably no internet

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

				var response = await QueryAsync<QueryResultDownloadData>("download-data", ps);

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
			catch (Exception e)
			{
				SAMLog.Warning("Backend", e); // probably no internet
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
					foreach (var diff in lvldata.Value.BestTimes.Keys)
					{
						scoretasks.Add(SetScore(profile, lvldata.Key, diff, lvldata.Value.GetTime(diff)));
					}
				}

				await Task.WhenAll(scoretasks);

				MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
				{
					profile.NeedsReupload = false;

					MainGame.Inst.SaveProfile();
				});
			}
			catch (Exception e)
			{
				SAMLog.Warning("Backend", e); // probably no internet
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

				var response = await QueryAsync<QueryResultDownloadData>("log-client", ps);

				if (response.result == "error")
				{
					SAMLog.Warning("Log_Upload", response.errormessage);
				}
			}
			catch (Exception e)
			{
				// well, that sucks
				// probably no internet
				SAMLog.Warning("Backend", e);
			}
		}
	}
}
