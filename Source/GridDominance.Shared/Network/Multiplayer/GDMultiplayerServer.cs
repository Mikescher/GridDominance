using System;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.NormalGameScreen;
using GridDominance.Shared.Screens.NormalGameScreen.Entities;
using MonoSAMFramework.Portable;
using MonoSAMFramework.Portable.LogProtocol;
using MonoSAMFramework.Portable.Network.Multiplayer;
using MonoSAMFramework.Portable.Screens;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using System.IO;
using System.Linq;

namespace GridDominance.Shared.Network.Multiplayer
{
	public class GDMultiplayerServer : GDMultiplayerCommon
	{
		public Guid LevelID = Levels.LEVELID_1_1;
		public GameSpeedModes Speed = GameSpeedModes.NORMAL;
		public int MusicIndex = 0;

		public GDMultiplayerServer(MultiplayerConnectionType t) : base(t)
		{
#if DEBUG
			AddDebugLine(MonoSAMGame.CurrentInst.GetCurrentScreen());
#endif
		}

#if DEBUG
		public void AddDebugLine(GameScreen s)
		{
			s.DebugDisp.AddLine("DebugMultiplayer", () => 
				$"SERVER(Ping={ProxyPing.Value:0.0000} | Loss={ProxyPackageLossPerc * 100:00.00}% | State={ConnState} | Mode={Mode} | Packages={packageCount} (l={packageModSize}byte) | Ctr={msgIdWraps:00}:{msgId:000})\n" + 
				$"      (LagBehind={lagBehindTime} | SendFreq={SendFreq.Frequency:00.0} | UserPings=[{string.Join(",", UserConn.Take(SessionCapacity).Select(u => (u.InGamePing.Value*1000).ToString("F0")))}])\n" +
				$"      (Packages[Out]={SendFreq.HistoryStr})\n" +
				$"      (Packages[In]={RecieveFreq.HistoryStr})\n" +
				$"      {_medium.DebugDisplayString}", this);
		}
#endif

		protected override void ProcessForwardData(byte[] d)
		{
			if (Screen == null) return;

			var seq = NetworkDataTools.GetByte(d[1]);
			var msgSessionID = NetworkDataTools.GetSplitBits(d[2], d[3], 8, 8);
			var msgUserID = NetworkDataTools.GetHighBits(d[4], 4);
			var msgSessionSecret = NetworkDataTools.GetSplitBits(d[4], d[5], 4, 8);
			var msgTime = NetworkDataTools.GetSingle(d[6], d[7], d[8], d[9]);

			if (msgSessionID != SessionID || msgSessionSecret != SessionSecret || msgUserID == 0)
			{
				SAMLog.Warning("SNS-Server", $"Invalid server message: ({msgSessionID} != {SessionID} || {msgSessionSecret} != {SessionSecret} || {msgUserID} == {0})");
				return;
			}

			if (IsSeqGreater(UserConn[msgUserID].LastRecievedSeq, seq))
			{
				ProcessStateData(d, msgUserID);
			}
			else
			{
				SAMLog.Debug("Ignore Out-Of-Order Message");
			}

			
			RecieveMsg(msgUserID, seq);
		}

		protected override void SendGameStateNow()
		{
			if (Screen == null) return;

			packageCount = 0;

			NetworkDataTools.SetByteWithHighBits(out MSG_FORWARD[2], SessionID, 16);
			NetworkDataTools.SetByteWithLowBits(out MSG_FORWARD[3], SessionID, 16);
			NetworkDataTools.SetSplitByte(out MSG_FORWARD[4], SessionUserID, SessionSecret, 4, 12, 4, 4);
			NetworkDataTools.SetByteWithLowBits(out MSG_FORWARD[5], SessionSecret, 12);
			NetworkDataTools.SetSingle(out MSG_FORWARD[6], out MSG_FORWARD[7], out MSG_FORWARD[8], out MSG_FORWARD[9], Screen.LevelTime);

			int p = PACKAGE_FORWARD_HEADER_SIZE;
			SendForwardBulletCannons(ref p);
			SendForwardLaserCannons(ref p);
			SendForwardMiniguns(ref p);
			SendForwardShieldProjectors(ref p);
			SendForwardRelayCannons(ref p);
			SendForwardTrishotCannons(ref p);
			SendForwardBullets(ref p);
			SendAndReset(ref p);
		}

		public byte[] GetLobbySyncData()
		{
			using (var ms = new MemoryStream())
			using (var bw = new BinaryWriter(ms))
			{
				bw.Write(LevelID.ToByteArray());
				bw.Write(GameSpeedModeHelper.ToByte(Speed));
				bw.Write((byte)MusicIndex);

				bw.Write(GDConstants.IntVersion);
				bw.Write(Levels.LEVELS[LevelID].CalcCheckSum());

				return ms.ToArray();
			}
		}

		protected override byte[] GetHostInfoData()
		{
			using (var ms = new MemoryStream())
			using (var bw = new BinaryWriter(ms))
			{
				bw.Write(LevelID.ToByteArray());
				bw.Write((byte)Speed);
				bw.Write((byte)MusicIndex);

				bw.Write(GDConstants.IntVersion);
				bw.Write(Levels.LEVELS[LevelID].CalcCheckSum());

				return ms.ToArray();
			}
		}

		protected override bool ShouldRecieveData(Fraction f, Cannon c) => !f.IsNeutral && f != Screen.LocalPlayerFraction && !Screen.HasFinished;
		protected override bool ShouldRecieveRotationData(Fraction f, Cannon c) => !f.IsNeutral && f != Screen.LocalPlayerFraction;
		protected override bool ShouldRecieveStateData(Fraction f, Cannon c) => false;
		protected override bool ShouldSendData(Cannon c) => true;

		protected override void ProcessForwardLobbySync(byte[] data)
		{
			SAMLog.Warning("GDMS::PFLS", "ProcessForwardLobbySync called for server");
		}

		protected override void ProcessForwardHostData(byte[] data)
		{
			SAMLog.Warning("GDMS::PFHD", "ProcessForwardHostData called for server");
		}

		protected override void ProcessAfterGameData(byte[] data)
		{
			SAMLog.Warning("GDMS::PAGD", "ProcessAfterGameData called for server");
		}
	}
}
