using System.Data;

namespace Apzon.Services.Interfaces.Financials;

public interface IDocumentConfirm
{
	DataTable SearchDataByBarCode(string barcode, string type);

	string GetRecord(int docEntry, int type);
}
