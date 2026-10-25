using System.Data;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Production;

namespace Apzon.Services.Interfaces.Production;

public interface IBillOfMasterials
{
	DataTable ShowOITT();

	DataTable ShowITT1(string BoMCode);

	DataTable Search(DataTable doc, DataTable dtOCondition);

	DataTable SearchLine(BillOfMasterials_LineImpl_ITT1 line);

	DataTable GetItemCFL();

	HttpResult DeleteDoc(string code);

	DataTable GetFirstRecord();

	DataTable GetLastRecord();

	DataTable GetNextRecord(string value);

	DataTable GetPreviousRecord(string value);

	DataTable GetItemName(string itemCode);

	DataTable getBoMByItemCode(string code);

	DataTable GetList_BillOfMster();

	DataTable GetRawMaterial(string itemCode);

	DataTable GetChildItemsInBom(string code);

	DataTable GetLineByFather(string father, string username, int branch);

	DataTable GetRecord(string p, string c);

	DataTable SearchADoc(string code);

	DataTable CheckValidItemType(string code);

	DataTable GetItemPriceByPriceList(string itemCode, int priceList);

	string SaveDocNoSAP(DataSet ds, Enums.TransactionType type, string code);

	DataTable GetActiveDimensionCount();

	DataTable GetSearchTableLine();

	DataTable GetSearchTableDoc();

	DataTable GetItemDetails(string type, string code);

	DataTable IsValidCode(int type, string code);

	string SaveDocNoSAPUpdate(DataSet ds);

	bool IsABOM(string code);

	DataTable GetDataLineBomVersion(string fatherCode, string version);

	DataTable GetAllChilds();

	DataTable CopyChildItemsInBom(string itemCode, int baseType, int baseEntry, int baseRow);
}
