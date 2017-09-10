using MonoSAMFramework.Portable.DebugTools;
using MonoSAMFramework.Portable.LogProtocol;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MonoSAMFramework.Portable.Language;

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
				string content;

				HttpResponseMessage response;
				try
				{
					if (para.Item2.Any())
						response = http.PostAsync(url, para.Item2).Result;
					else
						response = http.GetAsync(url).Result;

					response.EnsureSuccessStatusCode();
					content = response.Content.ReadAsStringAsync().Result;
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

#if DEBUG
				if (DebugSettings.Get("DebugNetwork"))
				{
					var json = CompactJsonFormatter.CompressJson(content, 1);
					var jlines = json.Replace("\r\n", "\n").Split('\n');
					if (jlines.Length > 7) json = string.Join("\n", jlines.Take(3).Concat(new[] {"..."}).Concat(jlines.Reverse().Take(3).Reverse()));
					SAMLog.Debug($"Query '{apiEndPoint}' returned \r\n" + json);
				}
#endif
				try
				{
					return JsonConvert.DeserializeObject<TReturn>(content);
				}
				catch (JsonReaderException e)
				{
					var headers1 = string.Join("\n", response.Headers.Select(p => p.Key + " = " + String.Join(" & ", p.Value)));
					var headers2 = string.Join("\n", response.Content.Headers.Select(p => p.Key + " = " + String.Join(" & ", p.Value)));
					var data0 = ByteUtils.CompressStringForStorage(content);

					throw new Exception($"JsonReaderException {e.Message} for (len={content?.Length}):\n\nresponse.Header:\n{headers1}\n\nresponse.Content.Header:\n{headers2}\n\ncontent=\n{data0}");
				}
			}

		}
	}
}
