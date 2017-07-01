using GridDominance.Shared.Resources;
using MonoSAMFramework.Portable.Network.Multiplayer;

namespace GridDominance.Shared.Network.Multiplayer
{
	class GDMultiplayerServer : SAMNetworkServer
	{
		
		public GDMultiplayerServer() 
			: base(new UDPNetworkMedium(GDConstants.MULTIPLAYER_SERVER_HOST, GDConstants.MULTIPLAYER_SERVER_PORT))
		{
			
		}
	}
}
