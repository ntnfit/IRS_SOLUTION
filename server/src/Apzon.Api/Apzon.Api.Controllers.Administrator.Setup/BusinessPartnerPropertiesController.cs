using System.Data;
using System.Web.Http;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.Administrator.Setup;

public class BusinessPartnerPropertiesController : BaseApiController
{
	[HttpGet]
	public DataTable LoadDoc()
	{
		return UnitOfWork.BusinessPartnerProperties.LoadDoc();
	}

	[HttpPost]
	public HttpResult Save([FromBody] DataTable dtDoc)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return new HttpResult(Enums.MessageCode.Error, "Invalid session");
			}
			return BrokerService.GetUnitOfWork(SessionId).BusinessPartners.SaveBPProperties(dtDoc, SessionUserInfo.SessionDiServer);
		}
		return UnitOfWork.BusinessPartnerProperties.Save(dtDoc);
	}
}
