using System;
using System.Data;
using System.Web.Http;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.DataServices;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;

namespace Apzon.Api.Controllers.FacebookCRM;

public class FacebookManagerController : BaseApiController
{
	[HttpGet]
	public DataTable GetDataAccessUser(string userCode)
	{
		return UnitOfWork.FacebookManager.GetDataAccessUser(userCode);
	}

	[HttpPost]
	public HttpResult SaveUserAccessToken([FromBody] DataTable dataAccessTokenUser)
	{
		return UnitOfWork.FacebookManager.SaveUserAccessToken(dataAccessTokenUser);
	}

	[HttpPost]
	public HttpResult SavePageUserInfo([FromBody] DataTable dtPageInfo)
	{
		return UnitOfWork.FacebookManager.SavePageUserInfo(dtPageInfo);
	}

	[HttpGet]
	public DataTable GetListUserAccessPage(string userCode)
	{
		return UnitOfWork.FacebookManager.GetListUserAccessPage(userCode);
	}

	[HttpGet]
	public DataSet GetDataConversationPage(string pageId)
	{
		return UnitOfWork.FacebookManager.GetDataConversationPage(pageId);
	}

	[HttpGet]
	public DataTable GetDataPageUser(string pageId)
	{
		return UnitOfWork.FacebookManager.GetDataPageUser(pageId);
	}

	[HttpPost]
	public HttpResult SaveDataPageUser([FromBody] Tuple<string, DataTable> dataConver)
	{
		return UnitOfWork.FacebookManager.SaveDataPageUser(dataConver.Item1, dataConver.Item2);
	}

	[HttpGet]
	public DataTable GetDataItem(string itemCode, string bpCode)
	{
		return UnitOfWork.FacebookManager.GetDataItem(itemCode, bpCode);
	}

	[HttpGet]
	public DataTable GetDataFacebookCustomer(string facebookUserId)
	{
		return UnitOfWork.FacebookManager.GetDataFacebookCustomer(facebookUserId);
	}

	[HttpPost]
	public string SaveOrder([FromBody] Tuple<DocumentsImpl, BPImpls> doc)
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
				return unitOfWork.Documents.SaveFacebookOrder(doc.Item1, doc.Item2, SessionUserInfo.SessionDiServer);
			}
			return "INVALIDSESSION";
		}
		return "This function is not supported";
	}
}
