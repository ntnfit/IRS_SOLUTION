using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.Financials;

public class DocumentConfirnController : BaseApiController
{
	[HttpGet]
	public DataTable SearchDataByBarCode(string barcode, string type)
	{
		return UnitOfWork.DocumentConfirm.SearchDataByBarCode(barcode, type);
	}

	[HttpGet]
	public string GetRecord(int docEntry, int type)
	{
		return UnitOfWork.DocumentConfirm.GetRecord(docEntry, type);
	}
}
