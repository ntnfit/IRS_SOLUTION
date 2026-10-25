using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.UserFields;

namespace Apzon.DataServices;

public interface IDataService
{
	HttpResult GetBusinessObjectTemplate(string sessionId, string objType);

	HttpResult GetServiceMetaData(string sessionId, string servicename);

	HttpResult BatchProcess(string xmlObject, int trytime = 0);

	HttpResult Process(string xmlObject, int trytime = 0);

	HttpResult AddObject(ApzObjectTypes objectTypes, string sessionId, string xmlObject, int trytime = 0, bool isTryProcess = false);

	HttpResult UpdateObject(ApzObjectTypes objectTypes, string sessionId, string xmlObject, object objectKey = null, int trytime = 0, bool isTryProcess = false);

	HttpResult CloseObject(ApzObjectTypes objectType, string sessionId, object objectKey);

	HttpResult RemoveObject(ApzObjectTypes objectType, string sessionId, object objectKey);

	HttpResult AddServiceObject(ApzObjectTypes objectTypes, string sessionId, string xmlObject, int trytime = 0, bool isTryProcess = false);

	HttpResult UpdateServiceObject(ApzObjectTypes objectTypes, string sessionId, string xmlObject, object objectKey = null, int trytime = 0, bool isTryProcess = false);

	HttpResult RemoveServiceObject(ApzObjectTypes objectType, string sessionId, string jsonObject);

	HttpResult GetByKey(string sessionId, string xmlObject, int trytime = 0);

	HttpResult Login(string dataBaseServer, string dataBaseName, string dataBaseType, string dataBaseUserName, string dataBasePassword, string companyUserName, string companyPassword, string language, string licenseServer);

	object LogOut(string sSessionId);

	string GetCurrentSessionId(string xmlObject);

	string UpdateSessionId(string sessionId, string xmlObject);

	Task<HttpResult> ProcessAsync(string xmlObject, int trytime = 0);

	HttpResult ProcessMutiTransaction(string seddionId, List<BodyRequestObject> lstBodyRequestObjects, bool isTryProcess = false, bool replaceOnPatch = true);

	HttpResult ProcessJson(ApzObjectTypes objectType, string sessionId, string jsonObject, Enums.HttpAction action = Enums.HttpAction.AddNew, object objectKey = null, string udoCode = "", bool isTryProcess = false, bool replaceOnPatch = true);

	HttpResult ProcessServiceJson(ApzObjectTypes objectType, string sessionId, string jsonObject, Enums.HttpAction action, object objectKey = null, string udoCode = "", bool isTryProcess = false);

	HttpResult GetDataObjectFromService(ApzObjectTypes objectType, string sessionId, Enums.HttpAction action = Enums.HttpAction.Get, object objectKey = null, string bodyString = "", bool isServiceAction = false, bool isTryProcess = false);

	string MappingUdf(List<UserFieldsImpl> lst, EnumCommandType commandType = EnumCommandType.Add, bool ignoreUpdateEmptyValue = false, List<string> lstFieldNotValidated = null);

	string MappingUdf(DataRow dataRow, EnumCommandType commandType = EnumCommandType.Add, bool ignoreUpdateEmptyValue = false);
}
