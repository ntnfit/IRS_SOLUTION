using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.Retail;

public class RetailChooseFromListController : BaseApiController
{
	[HttpGet]
	public DataSet ListItems(string username, string storecode, string itemcode, string cardcode, string barcode)
	{
		return UnitOfWork.RetailChooseFromList.ListItems(username, storecode, itemcode, cardcode, barcode);
	}

	[HttpGet]
	public DataSet Customer(string username, string storecode, string cardcode)
	{
		return UnitOfWork.RetailChooseFromList.Customer(username, storecode, cardcode);
	}
}
