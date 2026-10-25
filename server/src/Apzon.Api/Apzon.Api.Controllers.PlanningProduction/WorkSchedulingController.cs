using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.PlanningProduction;

public class WorkSchedulingController : BaseApiController
{
	[HttpGet]
	public DataTable GetDataDoc(int branch)
	{
		return UnitOfWork.WorkScheduling.GetDataDoc(branch);
	}

	[HttpGet]
	public DataTable GetDataLine(int docId)
	{
		return UnitOfWork.WorkScheduling.GetDataLine(docId);
	}

	[HttpGet]
	public DataTable LoadShiftData()
	{
		return UnitOfWork.WorkScheduling.LoadShiftData();
	}

	[HttpPost]
	public HttpResult Save([FromBody] DataSet dsSave)
	{
		return UnitOfWork.WorkScheduling.Save(dsSave, Enums.TransactionType.A, "");
	}

	[HttpPost]
	public HttpResult Update([FromBody] DataSet dsSave)
	{
		return UnitOfWork.WorkScheduling.Save(dsSave, Enums.TransactionType.U, "");
	}

	[HttpPost]
	public HttpResult Delete([FromBody] int id)
	{
		return UnitOfWork.WorkScheduling.Save(null, Enums.TransactionType.D, Function.ToString(id));
	}

	[HttpGet]
	public DataTable GetColumnLine()
	{
		return UnitOfWork.WorkScheduling.GetColumnLine();
	}

	[HttpGet]
	public DataTable GetColumnDoc()
	{
		return UnitOfWork.WorkScheduling.GetColumnDoc();
	}
}
