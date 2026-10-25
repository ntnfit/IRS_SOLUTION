using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.HumanResources.Salary;

public class LeaveParameterController : BaseApiController
{
	[HttpPost]
	public string SaveDoc([FromBody] DataTable dt)
	{
		return UnitOfWork.LeaveParameter.SaveDoc(dt);
	}

	[HttpGet]
	public DataTable GetDoc()
	{
		return UnitOfWork.LeaveParameter.GetDoc();
	}

	[HttpGet]
	public string DeleteDoc(int code)
	{
		return UnitOfWork.LeaveParameter.DeleteDoc(code);
	}
}
