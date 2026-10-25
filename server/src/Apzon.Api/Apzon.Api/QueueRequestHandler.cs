using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Apzon.Commons;

namespace Apzon.Api;

public class QueueRequestHandler : DelegatingHandler
{
	protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		HttpResponseMessage result = await base.SendAsync(request, cancellationToken);
		if (request.Headers.AsEnumerable().Any((KeyValuePair<string, IEnumerable<string>> t) => t.Key.Equals("ModifiedObject")) && request.Headers.Contains("SessionId"))
		{
			string text = Function.ToString(request.Headers.GetValues("SessionId").FirstOrDefault());
			if (Global.SessionUserInfo.ContainsKey(text))
			{
				string databaseName = Global.SessionUserInfo[text].DatabaseName;
				if (!string.IsNullOrEmpty(databaseName))
				{
					RequestDatabase requestDatabase = Global.QueueRequestModifiedData[databaseName];
					if (requestDatabase != null)
					{
						lock (requestDatabase)
						{
							requestDatabase.InProcessRequestCount--;
							if (requestDatabase.ListSessionInprocess.Contains(text))
							{
								requestDatabase.ListSessionInprocess.Remove(text);
							}
						}
					}
				}
			}
		}
		return result;
	}
}
