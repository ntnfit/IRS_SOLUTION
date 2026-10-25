using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.Inventory.Batches;

public class BatchTransactionController : BaseApiController
{
	[HttpGet]
	public DataTable GetDocs(string distNumber, string itemfrom, string itemto, string status)
	{
		return UnitOfWork.BatchTransaction.GetDocs(distNumber, itemfrom, itemto, status);
	}

	[HttpGet]
	public DataTable GetDoc(string itemCode, string sysNumber, string locCode)
	{
		return UnitOfWork.BatchTransaction.GetDoc(itemCode, sysNumber, locCode);
	}
}
