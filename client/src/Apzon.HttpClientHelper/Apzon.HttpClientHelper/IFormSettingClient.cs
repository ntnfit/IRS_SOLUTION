using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IFormSettingClient
{
	Task<DataTable> GetFormSetting(string formId, string itemId, string user, string lngCode);

	Task<string> SaveData(List<FormSetting> lst);

	Task<string> CopyFormSetting(DataTable dt);

	Task<string> CopyFormSettingToItem(int settingId);

	Task<DataSet> GetStructure(int id, string formId, string itemId, string userName, int branch, string type);

	Task<DataTable> GetListFormSetting(string formId, string itemId, string user, int id = 0);

	Task<DataTable> GetValidData(string formId, string itemId, string user, int id = 0);

	Task<HttpResult> Save(DataSet ds);

	Task<HttpResult> Delete(int id);

	Task<DataSet> GetUserForCopy(string userName, int idSetting);

	Task<HttpResult> CopyReportFormSetting(string listUser, int idSetting);

	Task<DataTable> GetDataLanguage(string lngCode);

	Task<DataTable> GetDataSettingLanguage(string currLng);

	Task<HttpResult> RestoreDefault(string userCode, string formId, string itemId);

	Task<HttpResult> RestoreDefaultAllUser(string formId, string itemId);

	Task<DataTable> GetMessageDataCenter(string lngCode);
}
