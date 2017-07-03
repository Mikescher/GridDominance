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

namespace GridDominance.Shared.Network.Multiplayer
{
	public class GDMultiplayerClient : SAMNetworkConnection
	{
		public Guid LevelID;
		public GameSpeedModes Speed;
		public int MusicIndex;

		public GDGameScreen_MPClient Screen;

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
			s.DebugDisp.AddLine(() => $"CONN(Ping={Ping.Value:0.0000} | Loss={PackageLossPerc * 100:00.00}% | State={ConnState} | Mode={Mode} | Packages={packageCount} (l={packageModSize}byte) | Ctr={msgIdWraps:00}:{msgId:000})");
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

			int p = 6;
			for (;;)
			{
				byte cmd = d[p];
				p++;

				if (p >= GDMultiplayerCodes.BYTE_SIZE)
				{
					SAMLog.Error("SNS-Client", "OOB: " + p);
					break;
				}
				else if (cmd == GDMultiplayerCodes.AREA_BCANNONS)
				{
					ProcessForwardBulletCannons(ref p, d);
				}
				else if (cmd == GDMultiplayerCodes.AREA_END)
				{
					break;
				}
				else
				{
					SAMLog.Error("SNS-Client", "Unknown AREA: " + cmd);
					break;
				}
			}


			RecieveMsg(msgUserID, seq);
		}

		private void ProcessForwardBulletCannons(ref int p, byte[] d)
		{
			int count = d[p];
			p++;

			for (int i = 0; i < count; i++)
			{
				var id = d[p];
				p++;
				var frac = Screen.GetFractionByID(d[p]);
				p++;
				var rot = (d[p] / 256f) * FloatMath.TAU;
				p++;
				var hp = (d[p] / 255f);
				p++;

				if (frac == Screen.LocalPlayerFraction) continue;

				Cannon c;
				if (Screen.CannonMap.TryGetValue(id, out c))
				{
					BulletCannon bc = c as BulletCannon;
					if (bc != null)
					{
						if (bc.Fraction != frac) bc.SetFraction(frac);
						bc.Rotation.Set(rot);
						bc.CannonHealth.Set(hp);
					}
				}
			}
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

		private void SendAndReset(ref int idx)
		{
			SetSequenceCounter(ref MSG_FORWARD[1]);
			MSG_FORWARD[idx] = GDMultiplayerCodes.AREA_END;
			idx++;
			
			_medium.Send(MSG_FORWARD, idx);
			packageModSize = idx;

			idx = 6;

			packageCount++;
		}

		private void SendForwardBulletCannons(ref int idx)
		{
			if (idx + 2 >= GDMultiplayerCodes.BYTE_SIZE) SendAndReset(ref idx);

			MSG_FORWARD[idx] = GDMultiplayerCodes.AREA_BCANNONS;
			idx++;

			byte arrsize = (byte) ((GDMultiplayerCodes.BYTE_SIZE - idx - 2) / GDMultiplayerCodes.SIZE_BCANNON_DEF);

			int posSize = idx;

			MSG_FORWARD[posSize] = 0xFF;
			idx++;

			int i = 0;
			foreach (var cannon in Screen.GetEntities<BulletCannon>())
			{
				if (cannon.Fraction != Screen.LocalPlayerFraction) continue;

				MSG_FORWARD[idx] = cannon.BlueprintCannonID;
				idx++;
				MSG_FORWARD[idx] = Screen.GetFractionID(cannon.Fraction);
				idx++;
				MSG_FORWARD[idx] = (byte) ((cannon.Rotation.TargetValue / FloatMath.TAU) * 256);
				idx++;
				MSG_FORWARD[idx] = (byte) (FloatMath.Clamp(cannon.CannonHealth.TargetValue, 0f, 1f) * 255);
				idx++;


				i++;
				if (i >= arrsize)
				{
					MSG_FORWARD[posSize] = (byte) i;
					SendAndReset(ref idx);
					MSG_FORWARD[idx] = GDMultiplayerCodes.AREA_BCANNONS;
					idx++;
					i -= arrsize;
					arrsize = (byte) ((GDMultiplayerCodes.BYTE_SIZE - idx - 2) / GDMultiplayerCodes.SIZE_BCANNON_DEF);
					posSize = idx;
					MSG_FORWARD[posSize] = 0xFF;
					idx++;
				}
			}
			MSG_FORWARD[posSize] = (byte) i;
		}
	}
}
