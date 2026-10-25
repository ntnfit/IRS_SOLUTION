using System.Data;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.DataServices;
using Apzon.Entities.Models.BusinessPartners;

namespace Apzon.Api.Controllers.Intergrated;

[ValidateLoginSapExceptionHandling]
public class IntegratedBPController : BaseApiController
{
	[HttpPost]
	public DataSet Search([FromBody] DataTable doc)
	{
		return UnitOfWork.IntegratedBP.Search(doc);
	}

	[HttpGet]
	public DataSet GetContactEmps(string cardCode)
	{
		return UnitOfWork.IntegratedBP.GetContactEmps(cardCode);
	}

	[HttpGet]
	public DataSet GetAddresses(string cardCode)
	{
		return UnitOfWork.IntegratedBP.GetAddresses(cardCode);
	}

	[HttpPost]
	public DataTable CheckExists([FromBody] DataTable dt)
	{
		return UnitOfWork.IntegratedBP.CheckExists(dt);
	}

	[HttpPost]
	public string SaveADoc([FromBody] BPImpls doc)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "INVALIDSESSION";
			}
			IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
			if (!string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
			{
				return unitOfWork.BusinessPartners.Save(doc, SessionUserInfo.SessionDiServer).Message;
			}
			return "INVALIDSESSION";
		}
		return "This function is not support.";
	}
}
