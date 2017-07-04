using System;
using System.IO;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.NormalGameScreen;
using MonoSAMFramework.Portable.LogProtocol;
using MonoSAMFramework.Portable.Network.Multiplayer;
using MonoSAMFramework.Portable;
using GridDominance.Shared.Screens.NormalGameScreen.Entities;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.Screens;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;

namespace GridDominance.Shared.Network.Multiplayer
{
	public class GDMultiplayerClient : GDMultiplayerCommon
	{
		public Guid LevelID;
		public GameSpeedModes Speed;
		public int MusicIndex;

		private int packageCount = 0;
		private int packageModSize = 0;

		public GDMultiplayerClient()
			: base(new UDPNetworkMedium(GDConstants.MULTIPLAYER_SERVER_HOST, GDConstants.MULTIPLAYER_SERVER_PORT))
		{
#if DEBUG
			AddDebugLine(MonoSAMGame.CurrentInst.GetCurrentScreen());
#endif
		}

#if DEBUG
		public void AddDebugLine(GameScreen s)
		{
			s.DebugDisp.AddLine(() => $"CLIENT(Ping={Ping.Value:0.0000} | Loss={PackageLossPerc * 100:00.00}% | State={ConnState} | Mode={Mode} | Packages={packageCount} (l={packageModSize}byte) | Ctr={msgIdWraps:00}:{msgId:000})");
		}
#endif
		
		protected override bool ProcessSpecificMessage(byte cmd, byte[] data)
		{
			if (cmd == CMD_FORWARDLOBBYSYNC)
			{
				if (Mode == ServerMode.InLobby)
				{
					ProcessForwardLobbySync(data);
					return true;
				}
			}
			else if (cmd == CMD_FORWARD)
			{
				if (Mode == ServerMode.InLobby)
				{
					Mode = ServerMode.InGame;
					ProcessForward(data);
					return true;
				}
				else if (Mode == ServerMode.InGame)
				{
					ProcessForward(data);
					return true;
				}
			}

			return false;
		}

		private void ProcessForwardLobbySync(byte[] data)
		{
			// [8: CMD] [8:seq] [16: SessionID] [4: UserID] [12: SessionSecret]

			UserConn[0].LastResponse = MonoSAMGame.CurrentTime.TotalElapsedSeconds;
			
			using (BinaryReader reader = new BinaryReader(new MemoryStream(data)))
			{
				reader.ReadByte(); // CMD
				reader.ReadByte(); // SEQ
				reader.ReadByte(); // SID
				reader.ReadByte(); // SID
				reader.ReadByte(); // SSC
				reader.ReadByte(); // SSC
				LevelID = new Guid(reader.ReadBytes(16));
				Speed = (GameSpeedModes) reader.ReadByte();
				MusicIndex = reader.ReadByte();
				//TODO Level Checksum
			}

			SAMLog.Debug($"[[CMD_FORWARDLOBBYSYNC]]: {LevelID} | {Speed} | {MusicIndex}");

			byte[] answer = new byte[8];

			answer[0] = ANS_FORWARDLOBBYSYNC;
			SetSequenceCounter(ref answer[1]);
			answer[2] = (byte) ((SessionID >> 8) & 0xFF);
			answer[3] = (byte) (SessionID & 0xFF);
			answer[4] = (byte) (((SessionUserID & 0xF) << 4) | ((SessionSecret >> 8) & 0x0F));
			answer[5] = (byte) (SessionSecret & 0xFF);
			answer[6] = (byte) SessionUserID;
			_medium.Send(answer);
		}

		private void ProcessForward(byte[] d)
		{
			if (Screen == null) return;

			// [8: CMD] [8:seq] [16: SessionID] [4: UserID] [12: SessionSecret] [~: Payload]

			byte seq = d[1];
			ushort msgSessionID = (ushort) (((d[2] << 8) & 0xFF00) | (d[3] & 0xFF));
			ushort msgSessionSecret = (ushort) ((((d[4] << 8) & 0xFF00) | (d[5] & 0xFF)) & 0x0FFF);
			byte msgUserID = (byte) ((d[4] >> 4) & 0x0F);

			if (msgSessionID != SessionID || msgSessionSecret != SessionSecret || msgUserID != 0)
			{
				SAMLog.Warning("SNS-Client", $"Invalid server message: ({msgSessionID} != {SessionID} || {msgSessionSecret} != {SessionSecret} || {msgUserID} != {0})");
				return;
			}

			ProcessStateData(d, msgUserID);

			RecieveMsg(msgUserID, seq);
		}

		protected override void SendGameStateNow()
		{
			if (Screen == null) return;

			packageCount = 0;

			MSG_FORWARD[2] = (byte) ((SessionID >> 8) & 0xFF);
			MSG_FORWARD[3] = (byte) (SessionID & 0xFF);
			MSG_FORWARD[4] = (byte) (((SessionUserID & 0xF) << 4) | ((SessionSecret >> 8) & 0x0F));
			MSG_FORWARD[5] = (byte) (SessionSecret & 0xFF);

			int p = 6;
			SendForwardBulletCannons(ref p);
			SendAndReset(ref p);
		}

		protected override bool ShouldRecieveData(Fraction f, BulletCannon c) => true;
		protected override bool ShouldRecieveRotationData(Fraction f, BulletCannon c) => f != Screen.LocalPlayerFraction;
		protected override bool ShouldRecieveStateData(Fraction f, BulletCannon c) => true;

		protected override bool ShouldSendData(BulletCannon c) => c.Fraction == Screen.LocalPlayerFraction;
	}
}
