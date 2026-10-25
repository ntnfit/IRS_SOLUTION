using System.Collections.Generic;
using System.Data;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.DataServices.Interfaces;

public interface IMaster : IRepository<DataTable>
{
	HttpResult Save(DataTable dt, string sessionId, ApzObjectTypes objType, string keyFieldName, Dictionary<string, string> dicFieldTags, bool deleteOldRows = false, Enums.ImportActionEnum actionType = Enums.ImportActionEnum.AddUpdate, string tableName = "", string rowTag = "", string keyTag = "", object parameter = null);

	HttpResult Remove(object value, string sessionId, ApzObjectTypes objType, string keyTag = "");

	HttpResult SaveService(DataTable dt, string sessionId, ApzObjectTypes objType, string serviceName, string keyFieldName, Dictionary<string, string> dicFieldTags, bool deleteOldRows = false, Enums.ImportActionEnum actionType = Enums.ImportActionEnum.AddUpdate, string tableName = "", string getMethodTag = "", string addMethodTag = "", string updateMethodTag = "", string deleteMethodTag = "", string paramsTag = "", string keyTag = "", string rowTag = "");

	HttpResult RemoveService(int code, string sessionId, ApzObjectTypes objType, string serviceName, string deleteMethodTag = "", string paramsTag = "", string keyTag = "");
}
