using System;
using System.IO;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.NormalGameScreen;
using MonoSAMFramework.Portable.LogProtocol;
using MonoSAMFramework.Portable.Network.Multiplayer;

namespace GridDominance.Shared.Network.Multiplayer
{
	public class GDMultiplayerClient : SAMNetworkConnection
	{
		public Guid LevelID;
		public GameSpeedModes Speed;
		public int MusicIndex;

		public GDMultiplayerClient() 
			: base(new UDPNetworkMedium(GDConstants.MULTIPLAYER_SERVER_HOST, GDConstants.MULTIPLAYER_SERVER_PORT))
		{
			
		}

		protected override bool ProcessSpecificMessage(byte cmd, byte[] data)
		{
			if (cmd == CMD_FORWARDLOBBYSYNC)
			{
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
				answer[2] = (byte)((SessionID >> 8) & 0xFF);
				answer[3] = (byte)(SessionID & 0xFF);
				answer[4] = (byte)(((SessionUserID & 0xF) << 4) | ((SessionSecret >> 8) & 0x0F));
				answer[5] = (byte)(SessionSecret & 0xFF);
				answer[6] = (byte)SessionUserID;
				_medium.Send(answer);
				
				
				return true;
			}

			return false;
		}
	}
}
