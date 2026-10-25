using System.Data;

namespace Apzon.Services.Interfaces.Administrator.Setup.Inventory;

public interface IWHSubSetup
{
	DataTable GetAllWarehouseSublevel();

	DataTable GetWarehouseSublevelByFldAbs(int abs);

	string CheckCodeExist(int fldAbs, string subCode);

	DataTable CheckCodeExist(DataTable data);

	string IsUsedInBinLocCodes(int fldAbs, string slCode);

	DataTable IsUsedInBinLocCodes(DataTable data);

	string Save(int userSign, DataTable doc);

	DataTable GetRangeSlCode(int fldAbs, string from, string to, string descr);
}
