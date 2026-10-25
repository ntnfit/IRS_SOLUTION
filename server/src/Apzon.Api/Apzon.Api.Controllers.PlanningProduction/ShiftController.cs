using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.PlanningProduction;

public class ShiftController : BaseApiController
{
	[HttpGet]
	public DataTable GetData(int branch)
	{
		return UnitOfWork.Shift.GetData(branch);
	}

	[HttpGet]
	public DataTable GetColumn()
	{
		return UnitOfWork.Shift.GetColumn();
	}

	[HttpPost]
	public HttpResult Save([FromBody] DataTable dtSave)
	{
		return UnitOfWork.Shift.Save(dtSave, Enums.TransactionType.A, "");
	}

	[HttpPost]
	public HttpResult Update([FromBody] DataTable dtSave)
	{
		return UnitOfWork.Shift.Save(dtSave, Enums.TransactionType.U, "");
	}

	[HttpPost]
	public HttpResult Delete([FromBody] int shiftId)
	{
		return UnitOfWork.Shift.Save(null, Enums.TransactionType.D, Function.ToString(shiftId));
	}
}
