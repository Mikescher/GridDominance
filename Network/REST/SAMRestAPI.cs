using MonoSAMFramework.Portable.DebugTools;
using MonoSAMFramework.Portable.LogProtocol;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
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

			http = new HttpClient(new HttpClientHandler{ AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate });
			http.MaxResponseContentBufferSize = 256000; // 265 kB
			http.Timeout = TimeSpan.FromSeconds(45);
		}

		protected Task<TReturn> QueryAsync<TReturn>(string apiEndPoint, RestParameterSet parameter, int maxTries)
		{
			return Task.Run(() => Query<TReturn>(apiEndPoint, parameter, maxTries));
		}

		protected string GetSigSecret() => secret;

		private TReturn Query<TReturn>(string apiEndPoint, RestParameterSet parameter, int maxTries)
		{
			var para = parameter.CreateParamString(secret);


			string url = serverbasepath + "/" + apiEndPoint + ".php" + para.Item1;

#if DEBUG
			if (DebugSettings.Get("DebugNetwork")) SAMLog.Debug($"QueryAsync('{apiEndPoint}', '{url}', {maxTries})");
#endif

			for (;;)
			{
				try
				{
					HttpResponseMessage response;
					if (para.Item2 != null)
						response = http.PostAsync(url, para.Item2).Result;
					else
						response = http.GetAsync(url).Result;

					response.EnsureSuccessStatusCode();
					var content = response.Content.ReadAsStringAsync().Result;
#if DEBUG
					if (DebugSettings.Get("DebugNetwork"))
					{
						var json = CompactJsonFormatter.CompressJson(content, 1);
						var jlines = json.Replace("\r\n", "\n").Split('\n');
						if (jlines.Length > 7) json = string.Join("\n", jlines.Take(3).Concat(new[] { "..." }).Concat(jlines.Reverse().Take(3).Reverse()));
						SAMLog.Debug($"Query '{apiEndPoint}' returned \r\n" + json);
					}
#endif
					return JsonConvert.DeserializeObject<TReturn>(content);
				}
				catch (Exception e)
				{
					if (maxTries > 0)
					{
						maxTries--;
						SAMLog.Info("QueryAsync", $"Retry query '{url}'. {maxTries}remaining", e.Message);
						MonoSAMGame.CurrentInst.Bridge.Sleep(RETRY_SLEEP_TIME);
						continue;
					}
					else
					{
						throw new RestConnectionException(e); // return to sender
					}
				}
			}

		}
	}
}
