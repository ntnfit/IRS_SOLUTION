using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Production;

public interface IIssueForProduction
{
	DataTable Search(int docEntry);

	int GetMaxCode();

	int GetMaxFileEntry();

	DataTable LoadItemsByParentCode(int docEntry);

	DataTable LoadItemsByProdOrderEntry(int docEntry);

	DataTable LoadAttachFiles(int absEntry);

	DataTable GetFirstRecord();

	DataTable GetLastRecord();

	DataTable GetNextRecord(string value);

	DataTable GetPreviousRecord(string value);

	string DeleteIGE1(string docEntry, string lineNum);

	DataTableResult GetDataByPOEntry(string barcode);

	DataSetResult GetDataByBarcode(string barcode, int docEntry);
}
