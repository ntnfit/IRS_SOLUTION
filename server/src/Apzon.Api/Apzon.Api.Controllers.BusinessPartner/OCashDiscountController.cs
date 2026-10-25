using System.Data;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.BusinessPartners;

namespace Apzon.Api.Controllers.BusinessPartner;

public class OCashDiscountController : BaseApiController
{
	[HttpGet]
	public DataTable ShowOCashDiscount()
	{
		return UnitOfWork.OCashDiscount.ShowOCashDiscount();
	}

	[HttpGet]
	public DataTable GetOCashDiscount(string iCode)
	{
		return UnitOfWork.OCashDiscount.GetOCashDiscount(iCode);
	}

	[HttpPost]
	[MappingSap(IsMapping = false)]
	public string SaveDoc([FromBody] OCashDiscountsImpl doc)
	{
		return UnitOfWork.OCashDiscount.SaveDoc(doc);
	}

	[HttpPost]
	[MappingSap(IsMapping = false)]
	public string DeleteDoc([FromBody] string code)
	{
		return UnitOfWork.OCashDiscount.DeleteDoc(code);
	}
}
