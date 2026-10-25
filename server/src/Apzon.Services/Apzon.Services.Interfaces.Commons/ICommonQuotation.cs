using System.Collections.Generic;
using System.Data;
using Apzon.Commons;
using Apzon.Entities.Models.Documents;

namespace Apzon.Services.Interfaces.Commons;

public interface ICommonQuotation
{
	DataTable GetDocuments(DocumentsImpl doc, string tblDoc);

	DataTable GetDocLines(int docEntry, string tblLine);

	DataTable ContactPerson(string cardCode);

	DataTable TaxCode();

	int GetMaxDocEntry(string tblDoc);

	DataTable Search(DocumentsImpl doc, string tblDoc, string tblLine);

	bool CheckDoc(int docEntry, string tblDoc);

	bool CheckDocLine(int docEntry, int lineNum, string tblLine);

	string SaveDoc(DocumentsImpl doc, string tblDoc, string tblLine);

	string SaveDocLine(Document_LinesImpl line, DocumentsImpl doc, string tblLine);

	string DeleteDoc(int docEntry, string tblDoc, string tblLine);

	string DeleteLine(int docEntry, int lineNum, string tblLine);

	string Canncel(int docEntry, string tblDoc, string tblLine);

	string Close(int docEntry, string tblDoc, string tblLine);

	string CloseLine(int lineNum, int docEntry, string tblLine);

	int GetQuantityBase(string tbl, string tblLine, int baseEntry, int baseLine);

	int GetOpenQuantity(string tbl, string tblLine, int docEntry, int lineNum);

	int DealDocOrgin(ApzObjectTypes typeFrom, int baseEntry);

	int DealLineOrgin(ApzObjectTypes typeTo, ApzObjectTypes typeFrom, int baseEntry, int baseLine);

	List<string> GetTableName(ApzObjectTypes type);
}
