using System.Data;

namespace Apzon.Services.Interfaces.Administrator.Setup.Inventory;

public interface IBinLocList
{
	DataTable GetBinLocListBySLCode(int fldAbs, string slCode);

	DataTable GetBinLocListByAttrVal(int fldAbs, string attrVal);
}
