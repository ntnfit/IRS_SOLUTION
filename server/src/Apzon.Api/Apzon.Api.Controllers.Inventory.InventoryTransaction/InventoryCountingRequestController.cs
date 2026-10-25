using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.Inventory.InventoryTransaction;

public class InventoryCountingRequestController : BaseApiController
{
	[HttpGet]
	public DataTable LoadData(int id, string whs, string startFrom, string startTo)
	{
		return UnitOfWork.InventoryCountingRequest.LoadData(id, whs, startFrom, startTo);
	}

	[HttpPost]
	public HttpResult Saves([FromBody] DataTable dt)
	{
		return UnitOfWork.InventoryCountingRequest.Saves(dt);
	}

	[HttpPost]
	public string DeleteDocs([FromBody] string ids)
	{
		return UnitOfWork.InventoryCountingRequest.DeleteDocs(ids);
	}

	[HttpGet]
	public DataTable FindNextLastFrist(int docId, int type)
	{
		return UnitOfWork.InventoryCountingRequest.FindNextLastFrist(docId, type);
	}
}
