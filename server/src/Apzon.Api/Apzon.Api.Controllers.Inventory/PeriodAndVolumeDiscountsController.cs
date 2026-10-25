using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.Inventory;

public class PeriodAndVolumeDiscountsController : BaseApiController
{
	[HttpGet]
	public DataSet LoadData(string cardCode)
	{
		return UnitOfWork.PeriodAndVolumeDiscounts.LoadData(cardCode);
	}

	[HttpGet]
	public DataTable GetItemByItemCode(string itemCode, int listNum)
	{
		return UnitOfWork.PeriodAndVolumeDiscounts.GetItemByItemCode(itemCode, listNum);
	}

	[HttpPost]
	public string SaveDoc([FromBody] DataSet ds)
	{
		return UnitOfWork.PeriodAndVolumeDiscounts.SaveDoc(ds);
	}

	[HttpGet]
	public DataTable GetItems(string fCode, string tCode, string fVendor, string tVendor)
	{
		return UnitOfWork.PeriodAndVolumeDiscounts.GetItems(fCode, tCode, fVendor, tVendor);
	}
}
