using System.Data;

namespace Apzon.Services.Interfaces.Administrator.Setup.Inventory;

public interface IAttrCodeSetup
{
	DataTable GetAttribute();

	DataTable GetAttributeByFldNum(int fldNum);

	string CheckExistedInBinLoc(int fldAbs, string attrValue);
}
