using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IContextActionClient
{
	Task<DataTable> GetListContextAction(string formId, string itemId);

	Task<DataTable> GetListParams(string formId, string itemId);

	Task<DataTable> GetListMapping(string formId, string itemId);

	Task<HttpResult> Save(string formId, string itemId, DataSet ds);

	DataSet GetData(string formId, string itemId);

	DataSet GetDataByEntry(int absEntry, string formId, string itemId);

	Task<DataTable> GetMappingUdo(string udocode, string actionFormId, string actionItemId, int absEntry, string sonname = "");

	Task<DataTable> GetTargetField(string udocode, string sonname = "");

	Task<DataTable> GetListAttachment(string actionFormId, string actionItemId);

	Task<DataTable> GetListAttachParams(string actionFormId, string actionItemId);

	Task<HttpResult> SavePoint(DataTable dt);

	Task<DataTable> GetProcessGroup(int absEntry, string formId, string itemId);

	Task<HttpResult> HandleWorkflow(List<Workflow> processGroup);

	Task<HttpResult> HandleWorkflowProcess(List<IWorkflowProcess> processGroup);

	Task<HttpResult> SavePublishService(DataSet ds);

	Task<DataTable> GetListHttpRequestConfig(string actionFormId, string actionItemId);

	DataTable GetHeaderByAlias(string alias, string formId, string itemId);

	Task<DataTable> GetColumnsFromPhysicalTableName(string physicalTableName);
}
