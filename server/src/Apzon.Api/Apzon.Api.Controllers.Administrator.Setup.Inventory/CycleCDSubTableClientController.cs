using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.Administrator.Setup.Inventory;

public class CycleCDSubTableClientController : BaseApiController
{
	[HttpGet]
	public DataTable GetInventoryCycles()
	{
		return UnitOfWork.CycleCountDeterminationSubTable.GetInventoryCycles();
	}

	[HttpGet]
	public DataTable GetByCycles(int cyclesBy)
	{
		return UnitOfWork.CycleCountDeterminationSubTable.GetByCycles(cyclesBy);
	}

	[HttpGet]
	public DataTable GetUsers()
	{
		return UnitOfWork.CycleCountDeterminationSubTable.GetUsers();
	}

	[HttpGet]
	public DataTable GetByWhsCode(string whsCode)
	{
		return UnitOfWork.CycleCountDeterminationSubTable.GetByWhsCode(whsCode);
	}

	[HttpGet]
	public DataTable SearchDataBywhs(string whs, int cycleBy)
	{
		return UnitOfWork.CycleCountDeterminationSubTable.SearchDataBywhs(whs, cycleBy);
	}

	[HttpGet]
	public DataTable GetByCycleCode(int cycleCode)
	{
		return UnitOfWork.CycleCountDeterminationSubTable.GetByCycleCode(cycleCode);
	}

	[HttpPost]
	public HttpResult Saves([FromBody] DataTable dt)
	{
		return UnitOfWork.CycleCountDeterminationSubTable.Save(dt, "U");
	}
}
