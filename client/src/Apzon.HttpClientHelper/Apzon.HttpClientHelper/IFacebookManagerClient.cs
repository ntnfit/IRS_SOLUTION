using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;

namespace Apzon.HttpClientHelper;

public interface IFacebookManagerClient
{
	DataTable GetDataAccessUser(string userCode);

	Task<HttpResult> SaveUserAccessToken(DataTable dtUserToken);

	Task<HttpResult> SavePageUserInfo(DataTable dtPageInfo);

	Task<DataTable> GetListUserAccessPage(string userCode);

	Task<DataSet> GetDataConversationPage(string pageId);

	DataTable GetDataPageUser(string pageId);

	Task<HttpResult> SaveDataPageUser(string pageId, DataTable dtConver);

	DataTable GetDataItem(string itemCode, string bpCode);

	Task<DataTable> GetDataFacebookCustomer(string facebookUserId);

	Task<string> SaveFacebookOrder(DocumentsImpl doc, BPImpls dtBp);
}
