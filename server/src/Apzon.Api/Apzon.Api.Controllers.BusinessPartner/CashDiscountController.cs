using System.Data;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.BusinessPartners;

namespace Apzon.Api.Controllers.BusinessPartner;

public class CashDiscountController : BaseApiController
{
	[HttpGet]
	public DataTable ShowCashDiscount()
	{
		return UnitOfWork.CashDiscount.ShowCashDiscount();
	}

	[HttpGet]
	public DataTable GetCashDiscount(string iCode)
	{
		return UnitOfWork.CashDiscount.GetCashDiscount(iCode);
	}

	[HttpGet]
	public int GetMaxLineId()
	{
		return UnitOfWork.CashDiscount.GetMaxLineId();
	}

	[HttpPost]
	[MappingSap(IsMapping = false)]
	public string SaveDoc([FromBody] CashDiscountsImpl doc)
	{
		return UnitOfWork.CashDiscount.SaveDoc(doc);
	}

	[HttpPost]
	[MappingSap(IsMapping = false)]
	public string DeleteDoc([FromBody] string code)
	{
		return UnitOfWork.CashDiscount.DeleteDoc(code);
	}
}
