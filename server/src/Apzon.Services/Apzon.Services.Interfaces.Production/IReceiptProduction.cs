using System.Data;
using Apzon.Entities.Models.Production;

namespace Apzon.Services.Interfaces.Production;

public interface IReceiptProduction
{
	string DeleteOIGN(int docEntry);

	string DeleteIGN1(int docEntry, int lineNum);

	DataTable Search(ReceiptProduction_Impl_OIGN doc);

	DataTable SearchLine(ReceiptProduction_LineImpl_IGN1 line);

	int GetMaxDocEntry();

	DataTable LoadItemsByProdOrderEntry(int docEntry);

	DataTable GetFirstRecord();

	DataTable GetLastRecord();

	DataTable GetNextRecord(string docEntry);

	DataTable GetPreviousRecord(string docEntry);

	string UpdateQtyProductionOrder(int orderEntry, decimal Qty, string tranType);
}
