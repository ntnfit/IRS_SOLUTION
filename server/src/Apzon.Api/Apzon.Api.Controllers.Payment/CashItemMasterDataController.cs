using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.Payment;

public class CashItemMasterDataController : BaseApiController
{
	[HttpGet]
	public DataTable GetAllCash()
	{
		return UnitOfWork.CashItemMasterData.GetAllCash();
	}

	[HttpGet]
	public DataTable GetDetailByCode(string cashCode)
	{
		return UnitOfWork.CashItemMasterData.GetDetailByCode(cashCode);
	}

	[HttpGet]
	public DataTable FindDataNextLast(string id, int type)
	{
		return UnitOfWork.CashItemMasterData.FindDataNextLast(id, type);
	}

	[HttpPost]
	public HttpResult Add([FromBody] DataSet ds)
	{
		return UnitOfWork.CashItemMasterData.Save(ds, "A");
	}

	[HttpPost]
	public HttpResult Update([FromBody] DataSet ds)
	{
		return UnitOfWork.CashItemMasterData.Save(ds, "U");
	}

	[HttpPost]
	public string DeleteDocs([FromBody] string ids)
	{
		return UnitOfWork.CashItemMasterData.DeleteDocs(ids);
	}
}
