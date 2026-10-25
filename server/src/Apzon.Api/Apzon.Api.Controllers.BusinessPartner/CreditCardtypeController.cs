using System.Data;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.BusinessPartners;

namespace Apzon.Api.Controllers.BusinessPartner;

public class CreditCardtypeController : BaseApiController
{
	[HttpGet]
	public DataTable ShowCreditCardType()
	{
		return UnitOfWork.CreditCardType.ShowCreditCardType();
	}

	[HttpGet]
	public int GetMaxCode()
	{
		return UnitOfWork.CardGroup.GetMaxCode();
	}

	[HttpPost]
	[MappingSap(IsMapping = false)]
	public string SaveDoc([FromBody] CreditCardTypesImpl doc)
	{
		return UnitOfWork.CreditCardType.SaveDoc(doc);
	}

	[HttpPost]
	[MappingSap(IsMapping = false)]
	public string DeleteDoc([FromBody] int code)
	{
		return UnitOfWork.CreditCardType.DeleteDoc(code);
	}
}
