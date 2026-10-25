using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.Administrator;

public class AuthorTokenController : BaseApiController
{
	[HttpPost]
	public string Save(DataTable dtObj)
	{
		return UnitOfWork.AuthorToken.Save(dtObj);
	}

	[HttpGet]
	public DataTable GetAllEmployees()
	{
		return UnitOfWork.AuthorToken.GetAllEmployees();
	}

	[HttpGet]
	public DataTable GetAuthorizedEmp(int UserId)
	{
		return UnitOfWork.AuthorToken.GetAuthorizedEmp(UserId);
	}
}
