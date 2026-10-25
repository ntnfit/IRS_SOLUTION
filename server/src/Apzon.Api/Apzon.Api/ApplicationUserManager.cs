using Apzon.Api.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.DataProtection;

namespace Apzon.Api;

public class ApplicationUserManager : UserManager<ApplicationUser>
{
	public ApplicationUserManager(IUserStore<ApplicationUser> store)
		: base(store)
	{
	}

	public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
	{
		ApplicationUserManager applicationUserManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
		applicationUserManager.UserValidator = new UserValidator<ApplicationUser>(applicationUserManager)
		{
			AllowOnlyAlphanumericUserNames = false,
			RequireUniqueEmail = true
		};
		applicationUserManager.PasswordValidator = new PasswordValidator
		{
			RequiredLength = 6,
			RequireNonLetterOrDigit = true,
			RequireDigit = true,
			RequireLowercase = true,
			RequireUppercase = true
		};
		IDataProtectionProvider dataProtectionProvider = options.DataProtectionProvider;
		if (dataProtectionProvider != null)
		{
			applicationUserManager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
		}
		return applicationUserManager;
	}
}
