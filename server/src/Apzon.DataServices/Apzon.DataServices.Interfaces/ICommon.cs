using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Production;
using Apzon.Entities.Models.UserFields;

namespace Apzon.DataServices.Interfaces;

public interface ICommon : IRepository<Common.Table_ID>
{
	bool GetIsMultiBranch();

	bool GetAvailableQty(string itemCode);

	bool GetSerialBatch(string itemCode);

	bool GetIsManualSerial(int series);

	string ExecuteBatFile(string command);

	Task<HttpResult> ProcessXmlRequest(string xmlRequest, string sessionId);

	DataTable GetTableStructure(string table);

	DataTable GetSeries(int ObjectCode);

	List<UserFieldsImpl> GetListField(string tableName);

	string SaveAttachmentFiles(List<AttachmentFile_Impl> lstFiles, Dictionary<string, string> code, string table);

	string SaveAttachmentFiles(DataTable dtFiles, Dictionary<string, string> code, string table, string sessionId, IUnitOfWork unitOfWork);

	bool ExistsSeries(ApzObjectTypes oItems, int seriesNum);

	HttpResult DoQuery(List<ContextActionDoQuery> doquery, string sessionId, string userExecute);

	HttpResult DoQueryWithTempTable(ContextActionDoQuery doquery, string sessionId, string userExecute);

	HttpResult HandleWorkflow(List<Workflow> processGroup, string sessionId);

	HttpResult HandleWorkflowProcess(List<IWorkflowProcess> processGroup, string sessionId);
}
