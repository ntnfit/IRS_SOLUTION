using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;

namespace Apzon.DataServices.Interfaces;

public interface IDocuments : IRepository<DocumentsImpl>
{
	Task<HttpResult> AddApprovalRequest(DocumentsImpl doc, string sessionId, ApzObjectTypes objType, string serverPath = "");

	DataTable GetApprovalTemplate(DocumentsImpl doc, string sessionId, string userName, string viewMode, ApzObjectTypes type);

	Task<HttpResult> Save(DocumentsImpl doc, string sessionId, ApzObjectTypes objType, ApzObjectTypes docObjectCode = ApzObjectTypes.oDrafts, bool ignoreAttach = false, bool ignoreUpdateEmptyValue = false, bool ignoreCheckexists = false, string serverPath = "");

	Task<HttpResult> AddNew(DocumentsImpl doc, string sessionId, ApzObjectTypes objType, ApzObjectTypes docObjectCode = ApzObjectTypes.oDrafts);

	string Mapping(DocumentsImpl doc, EnumCommandType commandType, ApzObjectTypes objType, ApzObjectTypes docObjectCode = ApzObjectTypes.oDrafts, bool ignoreUpdateEmptyValue = false);

	string ApproveDraft(string sessionId, string username, DataTable dt, string serverPath = "");

	Task<HttpResult> SaveDraftToDocumentApproval(DocumentsImpl doc, string sessionId, string userName = "");

	HttpResult SaveDraftToDocument(DocumentsImpl doc, string sessionId);

	HttpResult CancelDoc(int docEntry, string sessionId, ApzObjectTypes objType);

	HttpResult CloseDoc(int docEntry, string sessionId, ApzObjectTypes objType);

	string RemoveAndCloseDocumentDraft(DataTable dt, string sessionId);

	Task<string> SaveTestScriptAsync(string sessionId);

	string RemoveDoc(int idDocEntry, string sessionId, ApzObjectTypes apzObjectTypes);

	HttpResult AddListDocument(List<DocumentsImpl> docs, string sessionId, ApzObjectTypes objType);

	HttpResult UpdateListDocument(List<DocumentsImpl> docs, string sessionId);

	HttpResult AddListDocument(List<DocumentsImpl> docs, string sessionId);

	HttpResult UpdateDataByService(DocumentsImpl doc, string sessionId, ApzObjectTypes objType, ApzObjectTypes docObjectCode, string xml, DataTable removedBatchSerial = null);

	Task<HttpResult> SaveDocument(DocumentsImpl doc, string sessionId, ApzObjectTypes objType, ApzObjectTypes docObjectCode = ApzObjectTypes.oDrafts, bool ignoreAttach = false, bool ignoreUpdateEmptyValue = false, bool ignoreCheckexists = false, string serverPath = "");

	HttpResult SaveData(DocumentsImpl doc, string sessionId);

	string SaveFacebookOrder(DocumentsImpl doc, BPImpls dtBp, string sessionId);

	HttpResult AddDocWithUDO(DocumentsImpl doc, DataSet dsUdo, string sessionId, ApzObjectTypes objType, ApzObjectTypes docObjectCode = ApzObjectTypes.oDrafts);
}
