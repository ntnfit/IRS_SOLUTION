using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Api.Hubs;

namespace Apzon.Api.Controllers.ImportData;

public class ImportDataController : BaseApiController
{
	[HttpGet]
	public bool CheckExistBP(string cardCode)
	{
		return UnitOfWork.ImportData.CheckExistBP(cardCode);
	}

	[HttpGet]
	public DataTable GetABP(string cardCode)
	{
		return UnitOfWork.ImportData.GetABP(cardCode);
	}

	[HttpGet]
	public DataTable GetBPAddress(string cardCode)
	{
		return UnitOfWork.ImportData.GetBPAddress(cardCode);
	}

	[HttpGet]
	public DataTable GetBPContact(string cardCode)
	{
		return UnitOfWork.ImportData.GetBPContact(cardCode);
	}

	[HttpGet]
	public DataTable GetBPBankAccount(string cardCode)
	{
		return UnitOfWork.ImportData.GetBPBankAccount(cardCode);
	}

	[HttpGet]
	public DataTable GetBPPaymentDate(string cardCode)
	{
		return UnitOfWork.ImportData.GetBPPaymentDate(cardCode);
	}

	[HttpGet]
	public int GetBPManualSeries(string objectCode, string subType)
	{
		return UnitOfWork.ImportData.GetBPManualSeries(objectCode, subType);
	}

	[HttpPost]
	public string AddBP([FromBody] DataSet ds)
	{
		return UnitOfWork.ImportData.AddBP(ds);
	}

	[HttpPost]
	public string UpdateBP([FromBody] DataSet ds)
	{
		return UnitOfWork.ImportData.UpdateBP(ds);
	}

	[HttpGet]
	public DataTable GetAItem(string itemCode)
	{
		return UnitOfWork.ImportData.GetAItem(itemCode);
	}

	[HttpGet]
	public bool CheckExistItem(string itemCode)
	{
		return UnitOfWork.ImportData.CheckExistItem(itemCode);
	}

	[HttpGet]
	public DataTable GetItemWarehouse(string itemCode)
	{
		return UnitOfWork.ImportData.GetItemWarehouse(itemCode);
	}

	[HttpGet]
	public DataTable GetItemPrice(string itemCode)
	{
		return UnitOfWork.ImportData.GetItemPrice(itemCode);
	}

	[HttpGet]
	public DataTable GetItemUoM(string itemCode)
	{
		return UnitOfWork.ImportData.GetItemUoM(itemCode);
	}

	[HttpPost]
	public string AddItem([FromBody] DataSet ds)
	{
		return UnitOfWork.ImportData.AddItem(ds);
	}

	[HttpPost]
	public string UpdateItem([FromBody] DataSet ds)
	{
		return UnitOfWork.ImportData.UpdateItem(ds);
	}

	[HttpGet]
	public DataTable GetWareHouse()
	{
		return UnitOfWork.ImportData.GetWareHouse();
	}

	[HttpGet]
	public DataTable GetDefaultItemPrice()
	{
		return UnitOfWork.ImportData.GetDefaultItemPrice();
	}

	[HttpGet]
	public int ItemGetDefaultInventoryUomEntry(int ugpEntry)
	{
		return UnitOfWork.ImportData.ItemGetDefaultInventoryUomEntry(ugpEntry);
	}

	[HttpPost]
	public string AddBOM([FromBody] DataSet ds)
	{
		return UnitOfWork.ImportData.AddBOM(ds);
	}

	[HttpPost]
	public string UpdateBOM([FromBody] DataSet ds)
	{
		return UnitOfWork.ImportData.UpdateBOM(ds);
	}

	[HttpGet]
	public DataSet GetBillOfMaterials(string code)
	{
		return UnitOfWork.ImportData.GetBillOfMaterials(code);
	}

	[HttpGet]
	public bool ExistsBOM(string father)
	{
		return UnitOfWork.ImportData.ExistsBOM(father);
	}

	[HttpGet]
	public bool ExistsBOMChild(string father, int childNum, string code)
	{
		return UnitOfWork.ImportData.ExistsBOMChild(father, childNum, code);
	}

	[HttpGet]
	public DataSet GetAdvanceBillOfMaterials(string VerID, string code)
	{
		return UnitOfWork.ImportData.GetAdvanceBillOfMaterials(VerID, code);
	}

	[HttpGet]
	public bool ExistsAdvanceBOM(string VerID, string code)
	{
		return UnitOfWork.ImportData.ExistsAdvanceBOM(VerID, code);
	}

	[HttpGet]
	public bool ExistsAdvanceBOMChild(string father, int childNum, string code)
	{
		return UnitOfWork.ImportData.ExistsAdvanceBOMChild(father, childNum, code);
	}

	[HttpGet]
	public DataTable GetAEmployees(int empID)
	{
		return UnitOfWork.ImportData.GetAEmployees(empID);
	}

	[HttpGet]
	public DataTable GetAEmployeesRelatives(int empId, int relaId)
	{
		return UnitOfWork.ImportData.GetAEmployeesRelatives(empId, relaId);
	}

	[HttpGet]
	public DataTable GetEmployeeAbsence(int empID)
	{
		return UnitOfWork.ImportData.GetEmployeeAbsence(empID);
	}

	[HttpGet]
	public DataTable GetEmployeeEducation(int empID)
	{
		return UnitOfWork.ImportData.GetEmployeeEducation(empID);
	}

	[HttpGet]
	public DataTable GetEmployeeReview(int empID)
	{
		return UnitOfWork.ImportData.GetEmployeeReview(empID);
	}

	[HttpGet]
	public DataTable GetEmployeePrevEmp(int empID)
	{
		return UnitOfWork.ImportData.GetEmployeePrevEmp(empID);
	}

	[HttpGet]
	public DataTable GetEmployeeRole(int empID)
	{
		return UnitOfWork.ImportData.GetEmployeeRole(empID);
	}

	[HttpGet]
	public DataTable GetEmployeeTeams(int empID)
	{
		return UnitOfWork.ImportData.GetEmployeeTeams(empID);
	}

	[HttpGet]
	public DataTable GetEmployeeHealth(int empID)
	{
		return UnitOfWork.ImportData.GetEmployeeHealth(empID);
	}

	[HttpGet]
	public DataTable GetEmployeeContract(int empID)
	{
		return UnitOfWork.ImportData.GetEmployeeContract(empID);
	}

	[HttpPost]
	public string UpdateEmployees([FromBody] DataSet ds)
	{
		return UnitOfWork.ImportData.UpdateEmployees(ds);
	}

	[HttpPost]
	public string AddEmployees([FromBody] DataSet ds)
	{
		return UnitOfWork.ImportData.AddEmployees(ds);
	}

	[HttpGet]
	public bool CheckExistEmployees(int empID)
	{
		return UnitOfWork.ImportData.CheckExistEmployees(empID);
	}

	[HttpGet]
	public bool CheckExistEmployeesRelatives(int empID, int relativeId)
	{
		return UnitOfWork.ImportData.CheckExistEmployeesRelatives(empID, relativeId);
	}

	[HttpGet]
	public DataTable GetAttachment(int atcEntry)
	{
		return UnitOfWork.ImportData.GetAttachment(atcEntry);
	}

	public int GetManualSeries(string objectCode)
	{
		return UnitOfWork.ImportData.GetManualSeries(objectCode);
	}

	[HttpPost]
	public string SaveWithRollBack([FromBody] DataSet ds)
	{
		ImportDataHub updateHb = new ImportDataHub();
		return UnitOfWork.ImportData.SaveWithRollBack(ds, updateHb);
	}

	[HttpGet]
	public DataSet GetAWBSTemplate(string code)
	{
		return UnitOfWork.ImportData.GetAWBSTemplate(code);
	}

	[HttpGet]
	public bool CheckExistWBSTemplate(string code)
	{
		return UnitOfWork.ImportData.CheckExistWBSTemplate(code);
	}

	[HttpPost]
	public string TestHub([FromBody] int count)
	{
		ImportDataHub updateHb = new ImportDataHub();
		return UnitOfWork.ImportData.TestHub(count, updateHb);
	}
}
