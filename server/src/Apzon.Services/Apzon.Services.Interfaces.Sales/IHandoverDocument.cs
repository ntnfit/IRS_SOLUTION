using System.Data;
using Apzon.Entities.Models.Documents;

namespace Apzon.Services.Interfaces.Sales;

public interface IHandoverDocument
{
	DataSet LoadDocuments(int id, string barcode);

	DataTable GetById(int id);

	int GetPage(int currentId, int type, string username, int branch);

	DataSet GetDocumentDetail(string empCode, string barcode, string userName, int branch);

	DataSet SearchDocuments(DocumentsImpl doc);
}
