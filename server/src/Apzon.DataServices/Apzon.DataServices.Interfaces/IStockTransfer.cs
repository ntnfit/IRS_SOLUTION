using System;
using System.Data;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;

namespace Apzon.DataServices.Interfaces;

public interface IStockTransfer : IRepository<DocumentsImpl>
{
	HttpResult Save(DocumentsImpl doc, string sessionId, bool isDraft);

	string Close(int docEntry, string sessionId);

	string Cancel(int docEntry, string sessionId);

	bool Exists(int docEntry, string sessionId);

	string MappingXml(ApzObjectTypes objectType, DocumentsImpl model, bool isDraft, EnumCommandType type = EnumCommandType.Add);

	Task<HttpResult> SaveTransferBin(Tuple<DocumentsImpl, DataSet> model, string sessionId);

	HttpResult SaveDraftToDocument(int docEntry, string sessionId);

	HttpResult SaveDraftToDocument(DocumentsImpl doc, string sessionId);

	HttpResult Posting(Tuple<DocumentsImpl, DocumentsImpl, DocumentsImpl> model, string p);

	HttpResult AddTransferMobileApp(DataTable dt, string sessionId);
}
