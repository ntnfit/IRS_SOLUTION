using System.Collections.Generic;
using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.DashBoard;

public class DashBoardAuthController : BaseApiController
{
	[HttpGet]
	public DataTable GetAllAuth(string user)
	{
		return UnitOfWork.DashboardAuthorization.GetAllAuth(user);
	}

	[HttpGet]
	public DataTable GetAllAuthForm(string user)
	{
		return UnitOfWork.DashboardAuthorization.GetAllAuthForm(user);
	}

	[HttpGet]
	public DataTable GetFormAuth(string user, string form)
	{
		return UnitOfWork.DashboardAuthorization.GetFormAuth(user, form);
	}

	[HttpPost]
	public HttpResult SaveDoc([FromBody] DataTable dt)
	{
		return UnitOfWork.DashboardAuthorization.SaveDoc(dt);
	}

	[HttpGet]
	public DataTable GetUserForCopy(string user)
	{
		return UnitOfWork.DashboardAuthorization.GetUserForCopy(user);
	}

	[HttpPost]
	public string CopyAuth([FromBody] List<string> lstUser)
	{
		return UnitOfWork.DashboardAuthorization.CopyAuth(lstUser);
	}
}
