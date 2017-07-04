using System;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.NormalGameScreen;
using GridDominance.Shared.Screens.NormalGameScreen.Entities;
using MonoSAMFramework.Portable;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.LogProtocol;
using MonoSAMFramework.Portable.Network.Multiplayer;
using MonoSAMFramework.Portable.Screens;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;

namespace GridDominance.Shared.Network.Multiplayer
{
	public class GDMultiplayerServer : GDMultiplayerCommon
	{

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
			s.DebugDisp.AddLine(() => $"SERVER(Ping={Ping.Value:0.0000} | Loss={PackageLossPerc * 100:00.00}% | State={ConnState} | Mode={Mode} | Packages={packageCount} (l={packageModSize}byte) | Ctr={msgIdWraps:00}:{msgId:000})");
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
			

			byte seq = d[1];
			ushort msgSessionID = (ushort)(((d[2] << 8) & 0xFF00) | (d[3] & 0xFF));
			ushort msgSessionSecret = (ushort)((((d[4] << 8) & 0xFF00) | (d[5] & 0xFF)) & 0x0FFF);
			byte msgUserID = (byte)((d[4] >> 4) & 0x0F);

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

			MSG_FORWARD[2] = (byte)((SessionID >> 8) & 0xFF);
			MSG_FORWARD[3] = (byte)(SessionID & 0xFF);
			MSG_FORWARD[4] = (byte)(((SessionUserID & 0xF) << 4) | ((SessionSecret >> 8) & 0x0F));
			MSG_FORWARD[5] = (byte)(SessionSecret & 0xFF);

			int p = 6;
			SendForwardBulletCannons(ref p);
			SendForwardBullets(ref p);
			SendAndReset(ref p);
		}

		protected override bool ShouldRecieveData(Fraction f, BulletCannon c) => !f.IsNeutral && f != Screen.LocalPlayerFraction;
		protected override bool ShouldRecieveRotationData(Fraction f, BulletCannon c) => !f.IsNeutral && f != Screen.LocalPlayerFraction;
		protected override bool ShouldRecieveStateData(Fraction f, BulletCannon c) => false;

		protected override bool ShouldSendData(BulletCannon c) => true;

	}
}
