using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.Administrator.Setup.Inventory;

public class BinLocListController : BaseApiController
{
	[HttpGet]
	public DataTable GetBinLocListBySLCode(int fldAbs, string slCode)
	{
		return UnitOfWork.BinLocList.GetBinLocListBySLCode(fldAbs, slCode);
	}

	[HttpGet]
	public DataTable GetBinLocListByAttrVal(int fldAbs, string attrVal)
	{
		return UnitOfWork.BinLocList.GetBinLocListByAttrVal(fldAbs, attrVal);
	}
}
