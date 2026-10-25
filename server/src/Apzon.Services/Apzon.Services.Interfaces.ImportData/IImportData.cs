using System.Data;

namespace Apzon.Services.Interfaces.ImportData;

public interface IImportData
{
	bool CheckExistBP(string cardCode);

	DataTable GetABP(string cardCode);

	string AddBP(DataSet ds);

	string UpdateBP(DataSet ds);

	DataTable GetBPAddress(string cardCode);

	DataTable GetBPContact(string cardCode);

	DataTable GetBPBankAccount(string cardCode);

	DataTable GetBPPaymentDate(string cardCode);

	int GetBPManualSeries(string objectCode, string subType);

	DataTable GetAItem(string itemCode);

	DataTable GetItemWarehouse(string itemCode);

	DataTable GetItemPrice(string itemCode);

	DataTable GetItemUoM(string itemCode);

	DataTable GetWareHouse();

	string AddItem(DataSet ds);

	string UpdateItem(DataSet ds);

	bool CheckExistItem(string itemCode);

	DataTable GetDefaultItemPrice();

	int ItemGetDefaultInventoryUomEntry(int ugpEntry);

	string AddBOM(DataSet ds);

	string UpdateBOM(DataSet ds);

	DataSet GetBillOfMaterials(string code);

	bool ExistsBOM(string father);

	bool ExistsBOMChild(string father, int childNum, string code);

	DataSet GetAdvanceBillOfMaterials(string VerID, string code);

	bool ExistsAdvanceBOM(string VerID, string code);

	bool ExistsAdvanceBOMChild(string father, int childNum, string code);

	DataTable GetAEmployees(int empID);

	DataTable GetAEmployeesRelatives(int empId, int relaId);

	DataTable GetEmployeeAbsence(int empID);

	DataTable GetEmployeeEducation(int empID);

	DataTable GetEmployeeReview(int empID);

	DataTable GetEmployeePrevEmp(int empID);

	DataTable GetEmployeeRole(int empID);

	DataTable GetEmployeeTeams(int empID);

	DataTable GetEmployeeHealth(int empID);

	DataTable GetEmployeeContract(int empID);

	string AddEmployees(DataSet ds);

	string UpdateEmployees(DataSet ds);

	bool CheckExistEmployees(int empID);

	bool CheckExistEmployeesRelatives(int empID, int relativeId);

	DataSet GetAWBSTemplate(string code);

	bool CheckExistWBSTemplate(string code);

	DataTable GetAttachment(int atcEntry);

	int GetManualSeries(string objectCode);

	string SaveWithRollBack(DataSet ds, dynamic updateHb);

	string TestHub(int count, dynamic updateHb);
}
