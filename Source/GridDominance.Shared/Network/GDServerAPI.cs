using System;
using System.Collections.Generic;
using System.Text;
using GridDominance.Shared.Resources;
using MonoSAMFramework.Portable.Network.REST;

namespace GridDominance.Shared.Network
{
	class GDServerAPI : SAMRestAPI
	{

		public GDServerAPI() : base(GDConstants.SERVER_URL, GDConstants.SERVER_SECRET, GDConstants.SERVER_PUBKEY, GDConstants.SERVER_PUBKEYSIZE)
		{

		}
	}
}
