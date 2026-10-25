using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Apzon.Api.Models;

public class ApplicationUser : IdentityUser
{
	public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
	{
		return await manager.CreateIdentityAsync(this, authenticationType);
	}
}
