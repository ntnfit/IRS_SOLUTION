using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.FacebookCRM;

public interface IFacebookManager
{
	DataTable GetDataAccessUser(string userCode);

	HttpResult SaveUserAccessToken(DataTable dtUserAccessToken);

	HttpResult SavePageUserInfo(DataTable dtPageInfo);

	DataTable GetListUserAccessPage(string userCode);

	DataSet GetDataConversationPage(string pageId);

	DataTable GetDataPageUser(string pageId);

	HttpResult SaveDataPageUser(string pageId, DataTable dtConver);

	DataTable GetDataItem(string itemCode, string bpCode);

	DataTable GetDataFacebookCustomer(string facebookUserId);
}
