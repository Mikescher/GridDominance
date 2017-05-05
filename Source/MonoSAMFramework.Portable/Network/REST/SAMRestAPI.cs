using MonoSAMFramework.Portable.DebugTools;
using MonoSAMFramework.Portable.LogProtocol;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace MonoSAMFramework.Portable.Network.REST
{
	public abstract class SAMRestAPI
	{
		private const int RETRY_SLEEP_TIME = 128;//ms

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
		
		protected async Task<TReturn> QueryAsync<TReturn>(string apiEndPoint, RestParameterSet parameter, int maxTries)
		{
			string url = serverbasepath + "/" + apiEndPoint + ".php" + parameter.CreateParamString(secret, MonoSAMGame.CurrentInst.Bridge);

#if DEBUG
			System.Diagnostics.Debug.WriteLine($"QueryAsync('{apiEndPoint}', '{url}', {maxTries})");
#endif

			for (;;)
			{
				string content;

				try
				{
					var response = await http.GetAsync(url);
					response.EnsureSuccessStatusCode();
					content = await response.Content.ReadAsStringAsync();
				}
				catch (Exception e)
				{
					if (maxTries > 0)
					{
						maxTries--;
						SAMLog.Info("QueryAsync", $"Retry query '{url}'. {maxTries}remaining", e.Message);
						await Task.Delay(RETRY_SLEEP_TIME);
						continue;
					}
					else
					{
						throw new RestConnectionException(e); // return to sender
					}
				}

#if DEBUG
				SAMLog.Debug($"Query '{apiEndPoint}' returned \r\n" + CompactJsonFormatter.CompressJson(content, 1));
#endif

				return JsonConvert.DeserializeObject<TReturn>(content);
			}

		}
	}
}
