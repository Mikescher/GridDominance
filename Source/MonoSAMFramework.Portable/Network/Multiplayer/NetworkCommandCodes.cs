using System;

namespace MonoSAMFramework.Portable.Network.Multiplayer
{
	public enum NetworkCommandCodes
	{
		CMD_USERPING           = SAMNetworkConnection.CMD_USERPING,
		CMD_USERPING_ACK       = SAMNetworkConnection.CMD_USERPING_ACK,
		CMD_CREATESESSION      = SAMNetworkConnection.CMD_CREATESESSION,
		CMD_JOINSESSION        = SAMNetworkConnection.CMD_JOINSESSION,
		CMD_QUITSESSION        = SAMNetworkConnection.CMD_QUITSESSION,
		CMD_QUERYSESSION       = SAMNetworkConnection.CMD_QUERYSESSION,
		CMD_PING               = SAMNetworkConnection.CMD_PING,
		CMD_AFTERGAME          = SAMNetworkConnection.CMD_AFTERGAME,
		CMD_NEWGAME            = SAMNetworkConnection.CMD_NEWGAME,
		CMD_FORWARD            = SAMNetworkConnection.CMD_FORWARD,
		CMD_FORWARDLOBBYSYNC   = SAMNetworkConnection.CMD_FORWARDLOBBYSYNC,
		CMD_FORWARDHOSTINFO    = SAMNetworkConnection.CMD_FORWARDHOSTINFO,
		CMD_AUTOJOINSESSION    = SAMNetworkConnection.CMD_AUTOJOINSESSION,
		ACK_SESSIONCREATED     = SAMNetworkConnection.ACK_SESSIONCREATED,
		ACK_SESSIONJOINED      = SAMNetworkConnection.ACK_SESSIONJOINED,
		ACK_QUERYANSWER        = SAMNetworkConnection.ACK_QUERYANSWER,
		ACK_SESSIONQUITSUCCESS = SAMNetworkConnection.ACK_SESSIONQUITSUCCESS,
		ACK_PINGRESULT         = SAMNetworkConnection.ACK_PINGRESULT,
		ACK_SESSIONNOTFOUND    = SAMNetworkConnection.ACK_SESSIONNOTFOUND,
		ACK_SESSIONFULL        = SAMNetworkConnection.ACK_SESSIONFULL,
		ACK_SESSIONSECRETWRONG = SAMNetworkConnection.ACK_SESSIONSECRETWRONG,
		MSG_SESSIONTERMINATED  = SAMNetworkConnection.MSG_SESSIONTERMINATED,
		ANS_FORWARDLOBBYSYNC   = SAMNetworkConnection.ANS_FORWARDLOBBYSYNC,
		ANS_NEWGAME            = SAMNetworkConnection.ANS_NEWGAME,
	}

	public static class NetworkCommandCodesHelper
	{
		public static string CodeToString(byte b)
		{
			var ncc = (NetworkCommandCodes) b;

			if (Enum.IsDefined(typeof(NetworkCommandCodes), ncc)) return ncc.ToString();

			return $"UNDEFINED[{b:X2}]";
		}
	}
}
