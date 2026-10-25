using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;

namespace Apzon.DataServices.Interfaces;

public interface IStockTransferRequest : IRepository<DocumentsImpl>
{
	Task<HttpResult> Save(DocumentsImpl doc, bool isDraft, string sessionId);

	string MappingXml(ApzObjectTypes objectType, DocumentsImpl model, bool isDraft, EnumCommandType type = EnumCommandType.Add);

	string Close(int docEntry, string sessionId);
}
