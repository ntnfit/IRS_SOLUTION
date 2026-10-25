using System.Data;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Commons;

public interface IFormSetting
{
	DataTable GetFormSetting(string formId, string itemId, string user, string lngCode);

	string SaveData(DataTable lst);

	string CopyFormSetting(DataTable dt);

	string CopyFormSettingToItem(int settingId);

	DataSet GetStructure(int id, string formId, string itemId, string userName, int branch, string type);

	DataTable GetListFormSetting(string formId, string itemId, string user, int id);

	DataTable GetValidData(string formId, string itemId, string user, int id);

	HttpResult Save(DataSet ds, string objId = "", Enums.TransactionType transactionType = Enums.TransactionType.A, Enums.DatabaseSystemType databaseSystemType = Enums.DatabaseSystemType.SQL);

	HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet dsSave, ref string objId, ref string objNum);

	HttpResult SaveData(DataSet ds, Enums.TransactionType tranType, ref string objId, ref string objNum);

	DataSet GetUserForCopy(string userName, int idSetting);

	HttpResult CopyReportFormSetting(string listUser, int idSetting);

	DataTable GetDataLanguage(string lngCode);

	DataTable GetDataSettingLanguage(string currLng);

	HttpResult RestoreDefault(string userCode, string formId, string itemId);

	HttpResult RestoreDefaultAllUser(string formId, string itemId);

	DataTable GetMessageDataCenter(string lngCode);
}
