using System.Data;

namespace Apzon.Services.Interfaces.ProductionAddon;

public interface IControlKey
{
	DataTable GetDocs();

	DataTable GetDoc();

	string SaveDocs(DataSet ds);

	string SaveDoc(DataTable dt);

	DataTable GetListKeyCode();

	int GetMaxIndicatorID(string keycode);

	string DeleteDoc(DataTable dts);

	string DeleteDocs(string ids);

	string DeleteDocsAlone(string ids);

	DataTable GetDocSaved(string keycode);

	DataTable CheckExistKeyCode(string keycode);
}
