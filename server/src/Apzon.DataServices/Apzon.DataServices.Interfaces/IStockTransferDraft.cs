using Apzon.Entities.Models.Documents;

namespace Apzon.DataServices.Interfaces;

public interface IStockTransferDraft : IRepository<DocumentsImpl>
{
	string Close(int docEntry, string sessionId);
}
