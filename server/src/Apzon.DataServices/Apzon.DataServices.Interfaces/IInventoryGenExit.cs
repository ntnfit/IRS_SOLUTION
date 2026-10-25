using System.Threading.Tasks;
using Apzon.Entities.Models.Documents;

namespace Apzon.DataServices.Interfaces;

public interface IInventoryGenExit : IRepository<DocumentsImpl>
{
	Task<string> Save(DocumentsImpl doc, string sessionId);

	string SaveAsDraft(DocumentsImpl doc, string sessionId);

	string Close(int docEntry, string sessionId);

	string Reopen(int docEntry, string sessionId);

	string Cancel(int docEntry, string sessionId);

	string Remove(int docEntry, string sessionId);

	bool Exists(int docEntry, string sessionId);
}
