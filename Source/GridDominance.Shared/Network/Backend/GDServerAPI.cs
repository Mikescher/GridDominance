using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GridDominance.Shared.Network.Backend;
using GridDominance.Shared.Resources;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable;
using MonoSAMFramework.Portable.DebugTools;
using MonoSAMFramework.Portable.Network.REST;

namespace GridDominance.Shared.Network
{
	class GDServerAPI : SAMRestAPI
	{

		public GDServerAPI() : base(GDConstants.SERVER_URL, GDConstants.SERVER_SECRET, GDConstants.SERVER_PUBKEY)
		{

		}

		public async Task DoPing(IDebugTextDisplay disp, int userid, string password)
		{
			try
			{
				var ps = new RestParameterSet();
				ps.AddParameterInt("userid", userid);
				ps.AddParameterEncrypted("password", password);
				ps.AddParameterString("app_version", GDConstants.Version.ToString());

				var response = await QueryAsync<QueryResultPing>("ping", ps);

				if (response.result == "success")
				{
					disp.AddLine("Ping: OK", Color.Lime, Color.Black);
				}
				else if (response.result == "error")
				{
					disp.AddLine("Ping: Error", Color.DeepPink, Color.Black);
				}
				else
				{
					throw new Exception("Unknown server response.result: " + response.result);
				}
			}
			catch (Exception e)
			{
				disp.AddLine("Ping: Exception", Color.DeepPink, Color.Black);
			}
		}
	}
}
