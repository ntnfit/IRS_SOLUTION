using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.HumanResources;

public class EmpPenalizationController : BaseApiController
{
	[HttpGet]
	public DataTable GetData(int docNo)
	{
		return UnitOfWork.EmpPenalization.GetData(docNo);
	}

	[HttpGet]
	public DataTable GetPeriodRecords()
	{
		return UnitOfWork.EmpPenalization.GetPeriodRecords();
	}

	[HttpGet]
	public string Remove(int docNo)
	{
		return UnitOfWork.EmpPenalization.Remove(docNo);
	}

	[HttpPost]
	public string Save([FromBody] DataSet ds)
	{
		return UnitOfWork.EmpPenalization.Save(ds);
	}
}
