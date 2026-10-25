using System.Data;

namespace Apzon.Services.Interfaces.ProductionAddon;

public interface IRouting
{
	DataTable GetDocs();

	DataTable GetDoc(string roucode);

	string SaveDocs(DataSet ds);

	string SaveDoc(DataTable dt);

	int GetMaxLineNum(string roucode);

	DataTable SearchField(DataTable dts);

	DataTable GetListRouting();

	string DeleteDocs(string ids);

	string DeleteAllDocByKeyCode(string ids);

	string DeleteDoc(DataTable dts);

	string DeleteDocsAlone(string ids);
}
