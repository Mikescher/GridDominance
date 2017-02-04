using System;
using System.Net.Http;

namespace MonoSAMFramework.Portable.Network.REST
{
	public class SAMRestAPI
	{
		private readonly string serverbasepath;
		private readonly string secret;
		private readonly string publicParameterkey;
		private readonly int publicParameterkeySize;

		private readonly HttpClient http;

		public SAMRestAPI(string url, string signaturesecret, string pubkey, int pubsize)
		{
			serverbasepath = url;
			secret = signaturesecret;
			publicParameterkey = pubkey;
			publicParameterkeySize = pubsize;

			http = new HttpClient();
			http.MaxResponseContentBufferSize = 256000; // 265 kB
			http.Timeout = TimeSpan.FromSeconds(45);
		}

		public void QuerySynchron(string apiEndPoint, RestParameterSet parameter)
		{
			string url = serverbasepath + "/" + apiEndPoint + ".php" + parameter.CreateParamString(secret, publicParameterkey, publicParameterkeySize);


			var x = url;
			var y = x;
			var z = y;

		}
	}
}
