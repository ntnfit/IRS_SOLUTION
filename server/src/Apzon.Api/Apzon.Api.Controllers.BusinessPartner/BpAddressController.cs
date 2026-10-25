using System.Data;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.MasterData;

namespace Apzon.Api.Controllers.BusinessPartner;

public class BpAddressController : BaseApiController
{
	[HttpGet]
	public DataTable ShowAddress(string code, string addrType)
	{
		return UnitOfWork.Addresses.ShowAddress(code, addrType);
	}

	[HttpPost]
	[MappingSap(IsMapping = false)]
	public string SaveDoc([FromBody] AddressesImpl doc)
	{
		return UnitOfWork.Addresses.SaveDoc(doc);
	}

	[HttpPost]
	[MappingSap(IsMapping = false)]
	public string DeleteDoc([FromBody] string addr, string code, string addrType)
	{
		return UnitOfWork.Addresses.DeleteDoc(addr, code, addrType);
	}
}
