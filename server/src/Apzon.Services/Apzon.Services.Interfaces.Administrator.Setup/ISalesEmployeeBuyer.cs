using System.Data;

namespace Apzon.Services.Interfaces.Administrator.Setup;

public interface ISalesEmployeeBuyer
{
	DataTable GetOCOG();

	DataTable GetListEmployee();

	DataTable GetOSLP();

	string Save(DataTable dt);

	string DeleteRow(int slpCode);

	string SetDefaultCode(int slpCode);

	int GetDefaultCode();

	bool IsExistsSlpCode(int slpCode);
}
