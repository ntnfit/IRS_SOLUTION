using System.Data;

namespace Apzon.Services.Interfaces.PackingManagement;

public interface IPrintBillWrap
{
	DataTable GetCase(int POCode);

	DataTable GetItemByCaseCode(string CaseCode);

	string InsertCase(string CaseCode, int PO, int USERID);

	string InsertItem(string CaseCode, string BundleCode, int LineNumber);

	string DelCase(int LineNumber);

	string DelItem(string LineNum);
}
