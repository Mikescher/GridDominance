using MonoSAMFramework.Portable.DeviceBridge;
using System;
using System.Net.Http;

namespace MonoSAMFramework.Portable.Network.REST
{
	public abstract class SAMRestAPI
	{
		private readonly string serverbasepath;
		private readonly string secret;
		private readonly IRSAProvider parameterKey;

		private readonly HttpClient http;

		protected SAMRestAPI(string url, string signaturesecret, string pubkey)
		{
			serverbasepath = url;
			secret = signaturesecret;
			parameterKey = MonoSAMGame.CurrentInst.Bridge.CreateNewRSA().SetPublicKey(pubkey);

			http = new HttpClient();
			http.MaxResponseContentBufferSize = 256000; // 265 kB
			http.Timeout = TimeSpan.FromSeconds(45);
		}

		public void QuerySynchron(string apiEndPoint, RestParameterSet parameter)
		{
			string url = serverbasepath + "/" + apiEndPoint + ".php" + parameter.CreateParamString(secret, parameterKey);

			//TODO continue here
		}
	}
}
