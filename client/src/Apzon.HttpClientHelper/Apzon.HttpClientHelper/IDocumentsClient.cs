using System.Data;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Entities.Models;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;

namespace Apzon.HttpClientHelper;

public interface IDocumentsClient
{
	void SetObject(ApzObjectTypes type);

	DataTable GetPaymentTerms(int docEntry, ApzObjectTypes apzObjectTypes);

	Task<DataTable> LoadBatchNumberStruct();

	Task<DataTable> GetBatchSerialByCode(string itemcode, string whscode, int lineNum, string barcode);

	DataTable GetBatchSerialByDocEntry(int docEntry);

	DataTable CopyBatchSerial(string fromentry, ApzObjectTypes fromtype);

	Task<DocumentTable> CopyHeader(ApzObjectTypes fromType, ApzObjectTypes toType, string docEntry);

	Task<DocumentLineTable> Copy(ApzObjectTypes fromType, ApzObjectTypes toType, string docEntry);

	Task<DataTable> LoadSeries(string username);

	DataTable GetReferencedDocument(int docEntry);

	Task<int> GetDocumentNumber(int series);

	Task<DocumentLineTable> Duplicate(int docentry);

	DataTable GetExpnsByCode(string expnsCode);

	Task<DataTable> GetFreightCode();

	Task<DataTable> GetReferencedDocumentBy(int docEntry);

	Task<HttpResult> UpdateDataByService(DocumentsImpl doc);

	Task<HttpResult> SaveDraft(DataSet ds);

	Task<HttpResult> Remove(int docEntry);

	Task<HttpResult> Close(int docEntry);

	Task<HttpResult> Cancel(int docEntry);

	Task<HttpResult> SaveDoc(DocumentsImpl doc);

	Task<HttpResult> SaveDocument(DocumentsImpl doc);

	Task<DataTable> ListChooseDocuments(string from, string to);

	Task<HttpResult> SaveAsDraft(DocumentsImpl doc);

	Task<DataTable> GetLineShipTo(string cardCode);

	int GetDocNumByDocEntryType(int baseKeySearch, int baseTypeSearch);

	DataTable GetOwnerByBpUser(string cardCode, int userID);

	Task<DocumentTable> Search(DocumentsImpl value);

	Task<DocumentLineTable> GetLine(int id, string FilterLine = "");

	int GetDocTypeByDraftKey(int draftKey);

	string GetFormController();

	Task<DataTable> GetDownPaymentToDraw(ApzObjectTypes objType, string cardCode, string docCur);

	Task<DataTable> GetDownPaymentDrawed(ApzObjectTypes objType, int docEntry);
}
