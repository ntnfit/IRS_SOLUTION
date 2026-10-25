using System;
using System.Diagnostics;
using Apzon.Commons;
using Microsoft.AspNet.SignalR.Hubs;

namespace Apzon.Api.Attributes;

public class ErrorHandlingPipelineModuleSocket : HubPipelineModule
{
	protected override void OnIncomingError(ExceptionContext exceptionContext, IHubIncomingInvokerContext invokerContext)
	{
		try
		{
			if (exceptionContext.Error.InnerException != null)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "=> Exception " + exceptionContext.Error.InnerException.Message);
			}
			base.OnIncomingError(exceptionContext, invokerContext);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}
}
