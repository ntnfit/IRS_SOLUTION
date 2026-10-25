using System.Data;

namespace Apzon.HttpClientHelper;

public interface ISalesEmployeeBuyerClient
{
	DataTable GetListEmployee();

	string Save(DataTable dt);

	string DeleteRow(int slpCode);

	DataTable GetOCOG();

	DataTable GetOSLP();

	string SetDefaultCode(int slpCode);

	int GetDefaultCode();

	bool IsExistsSlpCode(int slpCode);

	DataTable GetStructure();
}
