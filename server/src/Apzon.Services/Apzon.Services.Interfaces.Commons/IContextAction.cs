using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Commons;

public interface IContextAction
{
	DataTable GetListContextAction(string formId, string itemId);

	DataTable GetListParams(string formId, string itemId);

	DataTable GetListMapping(string formId, string itemId);

	HttpResult Save(string formId, string itemId, DataSet ds);

	DataSet GetData(string formId, string itemId);

	DataSet GetDataByEntry(int absEntry, string formId, string itemId);

	DataTable GetMappingUdo(string udocode, string actionFormId, string actionItemId, int absEntry, string sonname);

	DataTable GetTargetField(string udocode, string sonname);

	DataTable GetListAttachment(string formId, string itemId);

	DataTable GetListAttachParams(string formId, string itemId);

	HttpResult SavePoint(DataTable dt);

	DataTable GetProcessGroup(int absEntry, string formId, string itemId);

	DataTable GetListHttpRequestConfig(string formId, string itemId);

	DataTable GetHeaderByAlias(string alias, string formId, string itemId);

	DataTable GetColumnsFromPhysicalTableName(string physicalTableName);

	HttpResult SavePhysicalData(string tableName, DataSet dsAction, DataTable source);
}
