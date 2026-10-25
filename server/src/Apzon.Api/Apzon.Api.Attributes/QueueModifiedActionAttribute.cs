using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Apzon.Commons;

namespace Apzon.Api.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class QueueModifiedActionAttribute : ActionFilterAttribute
{
	public bool NextToProcess { get; set; }

	public DateTime RequestDate { get; set; }

	public override async Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
	{
		if (actionContext.Request.Headers.Contains("SessionId"))
		{
			RequestDate = DateTime.Now;
			string text = Function.ToString(actionContext.Request.Headers.GetValues("SessionId").FirstOrDefault());
			string databaseName = Global.SessionUserInfo[text].DatabaseName;
			if (string.IsNullOrEmpty(databaseName))
			{
				HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Forbidden)
				{
					ReasonPhrase = "INVALIDSESSION"
				};
				actionContext.Response = response;
				return;
			}
			RequestDatabase requestDatabase = Global.QueueRequestModifiedData[databaseName];
			if (requestDatabase == null)
			{
				HttpResponseMessage response2 = new HttpResponseMessage(HttpStatusCode.ExpectationFailed)
				{
					ReasonPhrase = "INVALIDSESSION"
				};
				actionContext.Response = response2;
				return;
			}
			lock (requestDatabase)
			{
				if (requestDatabase.QueueRequestModifiedData.Count >= Global.QueueRequestWait)
				{
					HttpResponseMessage response3 = new HttpResponseMessage(HttpStatusCode.GatewayTimeout)
					{
						ReasonPhrase = "SERVER_BUSY"
					};
					actionContext.Response = response3;
					return;
				}
				requestDatabase.QueueRequestModifiedData.Add(this);
				if (requestDatabase.QueueRequestModifiedData.Any())
				{
					requestDatabase.QueueRequestModifiedData[0].NextToProcess = true;
				}
			}
			actionContext.Request.Headers.Add("ModifiedObject", "Y");
			while (true)
			{
				if ((DateTime.Now - RequestDate).TotalMinutes > (double)Global.RequestTimeout)
				{
					HttpResponseMessage response4 = new HttpResponseMessage(HttpStatusCode.RequestTimeout)
					{
						ReasonPhrase = "Request Timeout"
					};
					actionContext.Response = response4;
					return;
				}
				if (NextToProcess && requestDatabase.InProcessRequestCount < Global.MaxRequestProcess)
				{
					break;
				}
				Thread.Sleep(50);
			}
			lock (requestDatabase)
			{
				requestDatabase.InProcessRequestCount++;
				requestDatabase.QueueRequestModifiedData.Remove(this);
				if (requestDatabase.QueueRequestModifiedData.Any())
				{
					requestDatabase.QueueRequestModifiedData[0].NextToProcess = true;
				}
			}
			while (true)
			{
				lock (requestDatabase)
				{
					if (!requestDatabase.ListSessionInprocess.Contains(text))
					{
						requestDatabase.ListSessionInprocess.Add(text);
						break;
					}
				}
				if ((DateTime.Now - RequestDate).TotalMinutes > (double)Global.RequestTimeout)
				{
					HttpResponseMessage response5 = new HttpResponseMessage(HttpStatusCode.RequestTimeout)
					{
						ReasonPhrase = "Request Timeout"
					};
					actionContext.Response = response5;
					return;
				}
				Thread.Sleep(50);
			}
		}
		await base.OnActionExecutingAsync(actionContext, cancellationToken);
	}
}
