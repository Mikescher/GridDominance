using MonoSAMFramework.Portable.Localization;
using MonoSAMFramework.Portable.LogProtocol;
using MonoSAMFramework.Portable.Network.Multiplayer;

namespace GridDominance.Shared.Resources
{
	public static class L10NImplHelper
	{
		public static string FormatNetworkErrorMessage(SAMNetworkConnection.ErrorType type, object data)
		{
			switch (type)
			{
				case SAMNetworkConnection.ErrorType.None:
					return string.Empty;

				case SAMNetworkConnection.ErrorType.ProxyServerTimeout:
					return L10N.T(L10NImpl.STR_MP_TIMEOUT);

				case SAMNetworkConnection.ErrorType.ServerUserTimeout:
					return L10N.T(L10NImpl.STR_MP_TIMEOUT);

				case SAMNetworkConnection.ErrorType.UserTimeout:
					return L10N.TF(L10NImpl.STR_MP_TIMEOUT_USER, data);

				case SAMNetworkConnection.ErrorType.NotInLobby:
					return L10N.T(L10NImpl.STR_MP_NOTINLOBBY);

				case SAMNetworkConnection.ErrorType.SessionNotFound:
					return L10N.T(L10NImpl.STR_MP_SESSIONNOTFOUND);

				case SAMNetworkConnection.ErrorType.AuthentificationFailed:
					return L10N.T(L10NImpl.STR_MP_AUTHFAILED);

				case SAMNetworkConnection.ErrorType.LobbyFull:
					return L10N.T(L10NImpl.STR_MP_LOBBYFULL);

				case SAMNetworkConnection.ErrorType.GameVersionMismatch:
					return L10N.TF(L10NImpl.STR_MP_VERSIONMISMATCH, GDConstants.Version.ToString());

				case SAMNetworkConnection.ErrorType.LevelNotFound:
					return L10N.T(L10NImpl.STR_MP_LEVELNOTFOUND);

				case SAMNetworkConnection.ErrorType.LevelVersionMismatch:
					return L10N.T(L10NImpl.STR_MP_LEVELMISMATCH);

				case SAMNetworkConnection.ErrorType.UserDisconnect:
					return L10N.TF(L10NImpl.STR_MP_USERDISCONNECT, data);

				case SAMNetworkConnection.ErrorType.ServerDisconnect:
					return L10N.T(L10NImpl.STR_MP_SERVERDISCONNECT);

				case SAMNetworkConnection.ErrorType.BluetoothAdapterNotFound:
					return L10N.T(L10NImpl.STR_MP_BTADAPTERNULL);

				case SAMNetworkConnection.ErrorType.BluetoothAdapterNoPermission:
					return L10N.T(L10NImpl.STR_MP_BTADAPTERPERMDENIED);

				case SAMNetworkConnection.ErrorType.NetworkMediumInternalError:
					return L10N.T(L10NImpl.STR_MP_INTERNAL);

				case SAMNetworkConnection.ErrorType.BluetoothNotEnabled:
					return L10N.T(L10NImpl.STR_MP_BTDISABLED);

				case SAMNetworkConnection.ErrorType.P2PConnectionFailed:
					return L10N.T(L10NImpl.STR_MP_DIRECTCONNFAIL);

				case SAMNetworkConnection.ErrorType.P2PConnectionLost:
					return L10N.T(L10NImpl.STR_MP_DIRECTCONNLOST);

				case SAMNetworkConnection.ErrorType.P2PNoServerConnection:
					return L10N.T(L10NImpl.STR_MP_NOSERVERCONN);

				default:
					SAMLog.Error("L10NIH::EnumSwitch_FNEM", "type = "+ type);
					return string.Empty;
			}
		}
	}
}
