using System;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.NormalGameScreen;
using GridDominance.Shared.Screens.NormalGameScreen.Entities;
using MonoSAMFramework.Portable;
using MonoSAMFramework.Portable.LogProtocol;
using MonoSAMFramework.Portable.Network.Multiplayer;
using MonoSAMFramework.Portable.Screens;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using GridDominance.Levelfileformat.Blueprint;
using System.IO;

namespace GridDominance.Shared.Network.Multiplayer
{
	public class GDMultiplayerServer : GDMultiplayerCommon
	{
		public Guid LevelID = Levels.LEVELID_1_1;
		public GameSpeedModes Speed = GameSpeedModes.NORMAL;
		public int MusicIndex = 0;

		public GDMultiplayerServer() 
			: base(new UDPNetworkMedium(GDConstants.MULTIPLAYER_SERVER_HOST, GDConstants.MULTIPLAYER_SERVER_PORT))
		{
#if DEBUG
			AddDebugLine(MonoSAMGame.CurrentInst.GetCurrentScreen());
#endif
		}

#if DEBUG
		public void AddDebugLine(GameScreen s)
		{
			s.DebugDisp.AddLine("DebugMultiplayer", () => 
				$"SERVER(Ping={Ping.Value:0.0000} | Loss={PackageLossPerc * 100:00.00}% | State={ConnState} | Mode={Mode} | Packages={packageCount} (l={packageModSize}byte) | Ctr={msgIdWraps:00}:{msgId:000})\n" + 
				$"LagBehind={lagBehindTime})");
		}
#endif

		protected override bool ProcessSpecificMessage(byte cmd, byte[] data)
		{
			if (cmd == CMD_FORWARD)
			{
				if (Mode == ServerMode.InGame)
				{
					ProcessForward(data);
					return true;
				}
			}

			return false;
		}

		private void ProcessForward(byte[] d)
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
			SendForwardBullets(ref p);
			SendAndReset(ref p);
		}

		public byte[] GetLobbySyncData()
		{
			using (var ms = new MemoryStream())
			using (var bw = new BinaryWriter(ms))
			{
				bw.Write(LevelID.ToByteArray());
				bw.Write((byte)Speed);
				bw.Write((byte)MusicIndex);

				bw.Write(GDConstants.IVersion);
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

				bw.Write(GDConstants.IVersion);
				bw.Write(Levels.LEVELS[LevelID].CalcCheckSum());

				return ms.ToArray();
			}
		}

		protected override bool ShouldRecieveData(Fraction f, Cannon c) => !f.IsNeutral && f != Screen.LocalPlayerFraction && !Screen.HasFinished;
		protected override bool ShouldRecieveRotationData(Fraction f, Cannon c) => !f.IsNeutral && f != Screen.LocalPlayerFraction;
		protected override bool ShouldRecieveStateData(Fraction f, Cannon c) => false;

		protected override bool ShouldSendData(BulletCannon c) => true;
		protected override bool ShouldSendData(LaserCannon c) => true;
	}
}
