using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.Inventory.ItemManagement;

public class SerialNumberTransactionController : BaseApiController
{
	[HttpGet]
	public DataTable GetSerialNumbers(string itemCode, int sysNumber)
	{
		return UnitOfWork.SerialNumberTransactions.GetSerialNumbers(itemCode, sysNumber);
	}

	[HttpGet]
	public DataTable GetTransactions(string itemCode, int sysNumber, string whsCode)
	{
		return UnitOfWork.SerialNumberTransactions.GetTransactions(itemCode, sysNumber, whsCode);
	}

	[HttpGet]
	public DataTable GetDocs(string distNumber, string itemfrom, string itemto, string status)
	{
		return UnitOfWork.SerialNumberTransactions.GetDocs(distNumber, itemfrom, itemto, status);
	}
}
