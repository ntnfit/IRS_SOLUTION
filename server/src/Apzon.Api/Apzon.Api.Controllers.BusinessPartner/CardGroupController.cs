using System.Data;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.BusinessPartners;

namespace Apzon.Api.Controllers.BusinessPartner;

public class CardGroupController : BaseApiController
{
	[HttpGet]
	public DataTable ShowCardGroup()
	{
		return UnitOfWork.CardGroup.ShowCardGroup();
	}

	[HttpGet]
	public int GetMaxCode()
	{
		return UnitOfWork.CardGroup.GetMaxCode();
	}

	[HttpPost]
	[MappingSap(IsMapping = false)]
	public string SaveDoc([FromBody] CardGroupsImpl doc)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "Invalid session";
			}
			BrokerService.GetUnitOfWork(SessionId);
			return "Chức năng đang được cập nhật!";
		}
		return UnitOfWork.CardGroup.SaveDoc(doc);
	}

	[HttpPost]
	[MappingSap(IsMapping = false)]
	public string DeleteDoc([FromBody] int code)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "Invalid session";
			}
			BrokerService.GetUnitOfWork(SessionId);
			return "Chức năng đang được cập nhật!";
		}
		return UnitOfWork.CardGroup.DeleteDoc(code);
	}
}
