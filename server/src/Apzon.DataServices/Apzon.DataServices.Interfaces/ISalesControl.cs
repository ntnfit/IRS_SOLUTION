using System.Data;

namespace Apzon.DataServices.Interfaces;

public interface ISalesControl : IRepository<DataTable>
{
	string AddDocumentByBase(int docEntry, int fromObjType, int toObjType, string sessionId);
}
