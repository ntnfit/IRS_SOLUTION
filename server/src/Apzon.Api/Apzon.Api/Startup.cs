using System;
using Apzon.Api.Attributes;
using Apzon.Api.Models;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Security.Cookies;
using Owin;

namespace Apzon.Api;

public class Startup
{
	public void ConfigureAuth(IAppBuilder app)
	{
		app.CreatePerOwinContext(ApplicationDbContext.Create);
		app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
		app.UseCookieAuthentication(new CookieAuthenticationOptions());
		app.UseExternalSignInCookie("ExternalCookie");
	}

	public void Configuration(IAppBuilder app)
	{
		GlobalHost.HubPipeline.AddModule(new ErrorHandlingPipelineModuleSocket());
		ConfigureAuth(app);
		GlobalHost.Configuration.ConnectionTimeout = new TimeSpan(0, 0, 15, 0);
		GlobalHost.Configuration.DisconnectTimeout = new TimeSpan(0, 0, 15, 0);
		GlobalHost.Configuration.KeepAlive = new TimeSpan(0, 0, 5, 0);
		app.MapSignalR("/api/SignalR", new HubConfiguration
		{
			EnableDetailedErrors = true,
			EnableJavaScriptProxies = false
		});
	}
}
