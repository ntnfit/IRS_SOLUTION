using System.Collections.Generic;
using System.Data;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;

namespace Apzon.Services.Interfaces.Purchase;

public interface IILandedCost
{
	int GetMaxDocEntry();

	string SaveDoc(DocumentsImpl doc);

	string DeleteDoc(string docEntry);

	DataTable GetLandedCosts(string docEntry);

	DataTable LoadUDFData(string docentry);

	string SaveUDF(List<Common.Table_ID_Code> lst);

	DataTable GetLandedCostItem(string tblLine, string codes);

	DataTable GetLandedCostItemByDocEntry(string docEntry);

	string SaveDocItemLine(Document_LinesImpl doc);

	DataTable GetLandedCostLineByDocEntry(string docEntry);

	string SaveDocCostLine(Document_LinesImpl doc);

	DataTable GetLandedCostCodeFormMain();

	DataTable GetLandedCostCode();

	int GetMaxDocEntryCostCode();

	string SaveDocCode(Document_LinesImpl doc);

	string DeleteDocCode(string docEntry);

	string SaveDocCostGRPO(Document_LinesImpl doc);

	string SaveDocCostGRPODetail(Document_LinesImpl doc);

	string Savesp_InsertWhseJournal(DocumentsImpl doc);

	DataTable GetNext(string docEntry);

	DataTable GetPrevious(string docEntry);

	DataTable GetFirst();

	DataTable GetLast();

	DataTable Search(DocumentsImpl doc);
}
