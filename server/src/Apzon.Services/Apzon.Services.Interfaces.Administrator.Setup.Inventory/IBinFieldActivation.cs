using System.Data;

namespace Apzon.Services.Interfaces.Administrator.Setup.Inventory;

public interface IBinFieldActivation
{
	DataTable GetObfcData();

	string Save(DataTable dt);

	string IsFieldExist(int absEntry, string fldType);
}
