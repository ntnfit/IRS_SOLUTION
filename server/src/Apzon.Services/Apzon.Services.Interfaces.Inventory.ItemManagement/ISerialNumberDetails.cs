using System.Data;

namespace Apzon.Services.Interfaces.Inventory.ItemManagement;

public interface ISerialNumberDetails
{
	DataTable Search(string itemCode, string itemName, string mnfSerial, string serialNum, string lotNum);

	DataTable GetSearialNumbers(string itemCode);

	DataTable GetQuantity(int absEntry);

	string SaveSerialNumberDetail(DataTable dtSave);

	string GetUniqueSerialNo();

	DataTable GetByAbsEntry(int absEntry);

	DataTable LoadAnEntry(int absEntry);

	int GetPage(int current, int type, string username, int branch);

	DataSet GetSerialDetail(int absEntry, string itemCode, string status, string distNumber, string mnfSerial, string lotNumber);
}
