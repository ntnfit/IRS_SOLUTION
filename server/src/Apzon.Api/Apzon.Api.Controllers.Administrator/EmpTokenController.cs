using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.Administrator;

public class EmpTokenController : BaseApiController
{
	[HttpPost]
	public string Save([FromBody] DataTable dtObj)
	{
		return UnitOfWork.EmpToken.Save(dtObj);
	}

	[HttpGet]
	public DataTable GetEmpTokens(int userId)
	{
		return UnitOfWork.EmpToken.GetEmpTokens(userId);
	}

	[HttpGet]
	public string CheckToken(int empId, string token)
	{
		return UnitOfWork.EmpToken.CheckToken(empId, token);
	}
}
