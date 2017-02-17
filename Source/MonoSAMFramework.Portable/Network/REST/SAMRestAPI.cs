using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace MonoSAMFramework.Portable.Network.REST
{
	public abstract class SAMRestAPI
	{
		private readonly string serverbasepath;
		private readonly string secret;

		private readonly HttpClient http;

		protected SAMRestAPI(string url, string signaturesecret)
		{
			serverbasepath = url;
			secret = signaturesecret;

			http = new HttpClient();
			http.MaxResponseContentBufferSize = 256000; // 265 kB
			http.Timeout = TimeSpan.FromSeconds(45);
		}
		
		protected async Task<TReturn> QueryAsync<TReturn>(string apiEndPoint, RestParameterSet parameter)
		{
			string url = serverbasepath + "/" + apiEndPoint + ".php" + parameter.CreateParamString(secret, MonoSAMGame.CurrentInst.Bridge);

			var response = await http.GetAsync(url);

			response.EnsureSuccessStatusCode();

			var content = await response.Content.ReadAsStringAsync();

			return JsonConvert.DeserializeObject<TReturn>(content);
		}
	}
}
