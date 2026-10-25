using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.HumanResources.TimeSheeting;

public class InOutMethodController : BaseApiController
{
	[HttpGet]
	public DataTable GetAll()
	{
		return UnitOfWork.InOutMethod.GetAll();
	}

	[HttpGet]
	public string Remove(int methodId)
	{
		return UnitOfWork.InOutMethod.Remove(methodId);
	}

	[HttpPost]
	public string Save([FromBody] DataSet ds)
	{
		return UnitOfWork.InOutMethod.Save(ds);
	}
}
