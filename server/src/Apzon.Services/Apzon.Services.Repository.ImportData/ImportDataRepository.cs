using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.ImportData;
using Apzon.Services.Interfaces.ImportData;
using Apzon.Services.Repository.ImportData.BillOfMaterials;
using Apzon.Services.Repository.ImportData.BusinessPartners;
using Apzon.Services.Repository.ImportData.HumanResources;
using Apzon.Services.Repository.ImportData.Inventory;

namespace Apzon.Services.Repository.ImportData;

public class ImportDataRepository : BaseService<DataTable>, IImportData
{
	public ImportDataRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public string AddBP(DataSet ds)
	{
		try
		{
			return GetUnitOfWork().BusinessMasterData.SaveDocDataSet(ds, "A");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public string UpdateBP(DataSet ds)
	{
		try
		{
			return GetUnitOfWork().BusinessMasterData.SaveDocDataSet(ds, "U");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public DataTable GetABP(string cardCode)
	{
		return GetDataByTableCode("OCRD", "cardCode", cardCode);
	}

	public DataTable GetBPAddress(string cardCode)
	{
		return GetDataByTableCode("CRD1", "cardCode", cardCode);
	}

	public DataTable GetBPContact(string cardCode)
	{
		return GetDataByTableCode("OCPR", "cardCode", cardCode);
	}

	public DataTable GetBPBankAccount(string cardCode)
	{
		return GetDataByTableCode("OCRB", "cardCode", cardCode);
	}

	public DataTable GetBPPaymentDate(string cardCode)
	{
		return GetDataByTableCode("CRD5", "cardCode", cardCode);
	}

	public bool CheckExistBP(string cardCode)
	{
		return new ImportDataBPMasterRepository(DatabaseService).CheckExistBP(cardCode);
	}

	public int GetBPManualSeries(string objectCode, string subType)
	{
		return new ImportDataBPMasterRepository(DatabaseService).GetBPManualSeries(objectCode, subType);
	}

	public DataTable GetAItem(string itemCode)
	{
		return GetDataByTableCode("OITM", "itemCode", itemCode);
	}

	public DataTable GetItemWarehouse(string itemCode)
	{
		return GetDataByTableCode("OITW", "itemCode", itemCode);
	}

	public DataTable GetItemPrice(string itemCode)
	{
		return GetDataByTableCode("ITM1", "itemCode", itemCode);
	}

	public DataTable GetItemUoM(string itemCode)
	{
		return GetDataByTableCode("ITM12", "itemCode", itemCode);
	}

	public string AddItem(DataSet ds)
	{
		try
		{
			ds.Tables["OtherTable"].Rows.Clear();
			ds.Tables["OtherTable"].AcceptChanges();
			ds.AcceptChanges();
			return GetUnitOfWork().Items.SaveDocFromImport(ds, "A");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public string UpdateItem(DataSet ds)
	{
		try
		{
			ds.Tables["OtherTable"].Rows.Clear();
			ds.Tables["OtherTable"].AcceptChanges();
			ds.AcceptChanges();
			return GetUnitOfWork().Items.SaveDocFromImport(ds, "U");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public DataTable GetDefaultItemPrice()
	{
		return new ImportDataItemMasterRepository(DatabaseService).GetDefaultItemPrice();
	}

	public int ItemGetDefaultInventoryUomEntry(int ugpEntry)
	{
		return new ImportDataItemMasterRepository(DatabaseService).ItemGetDefaultInventoryUomEntry(ugpEntry);
	}

	public DataTable GetWareHouse()
	{
		return new ImportDataItemMasterRepository(DatabaseService).GetWareHouse();
	}

	public bool CheckExistItem(string itemCode)
	{
		return new ImportDataItemMasterRepository(DatabaseService).CheckExistItem(itemCode);
	}

	public string AddBOM(DataSet ds)
	{
		return GetUnitOfWork().BillOfMasterials.SaveDocNoSAP(ds, Enums.TransactionType.A, "");
	}

	public string UpdateBOM(DataSet ds)
	{
		return GetUnitOfWork().BillOfMasterials.SaveDocNoSAPUpdate(ds);
	}

	public DataSet GetBillOfMaterials(string code)
	{
		return new ImportDataBillOfMaterialsRepository(DatabaseService).GetBillOfMaterials(code);
	}

	public bool ExistsBOM(string father)
	{
		return new ImportDataBillOfMaterialsRepository(DatabaseService).ExistsBOM(father);
	}

	public bool ExistsBOMChild(string father, int childNum, string code)
	{
		return new ImportDataBillOfMaterialsRepository(DatabaseService).ExistsBOMChild(father, childNum, code);
	}

	public DataSet GetAdvanceBillOfMaterials(string VerID, string code)
	{
		string query = "Select * From APZ_OITT Where \"Code\"=@code And \"BomVerID\"=@verId;\r\n                           Select * From APZ_ITT1 Where \"Father\"=@verId;";
		return ExecuteData(query, CommandType.Text, new SqlParameter("@code", code), new SqlParameter("@verId", VerID));
	}

	public bool ExistsAdvanceBOM(string VerID, string code)
	{
		string query = "Select * From APZ_OITT Where \"Code\"=@code And \"BomVerID\"=@verId;";
		if (ExecuteDataTable(query, CommandType.Text, new SqlParameter("@code", code), new SqlParameter("@verId", VerID)).IsNotNull())
		{
			return true;
		}
		return false;
	}

	public bool ExistsAdvanceBOMChild(string father, int childNum, string code)
	{
		string query = "Select * From APZ_ITT1 Where \"Father\"=@father And \"Code\"=@code And \"ChildNum\"=@ChildNum;";
		if (ExecuteDataTable(query, CommandType.Text, new SqlParameter("@father", father), new SqlParameter("@code", code), new SqlParameter("@ChildNum", childNum)).IsNotNull())
		{
			return true;
		}
		return false;
	}

	public DataTable GetAEmployees(int empID)
	{
		return GetDataByTableCode("OHEM", "empID", empID);
	}

	public DataTable GetAEmployeesRelatives(int empId, int relaId)
	{
		return ExecuteDataTable("select * from APZ_OHER where \"empID\" = @empId and \"relaID\" = @relaID;", CommandType.Text, new SqlParameter("@empId", empId), new SqlParameter("@relaID", relaId));
	}

	public DataTable GetEmployeeAbsence(int empID)
	{
		return GetDataByTableCode("HEM1", "empID", empID);
	}

	public DataTable GetEmployeeEducation(int empID)
	{
		return GetDataByTableCode("HEM2", "empID", empID);
	}

	public DataTable GetEmployeeReview(int empID)
	{
		return GetDataByTableCode("HEM3", "empID", empID);
	}

	public DataTable GetEmployeePrevEmp(int empID)
	{
		return GetDataByTableCode("HEM4", "empID", empID);
	}

	public DataTable GetEmployeeRole(int empID)
	{
		return GetDataByTableCode("HEM6", "empID", empID);
	}

	public DataTable GetEmployeeTeams(int empID)
	{
		return GetDataByTableCode("HTM1", "empID", empID);
	}

	public DataTable GetEmployeeHealth(int empID)
	{
		return GetDataByTableCode("APZ_OHEH", "empID", empID);
	}

	public DataTable GetEmployeeContract(int empID)
	{
		return GetDataByTableCode("APZ_OHEC", "empID", empID);
	}

	public string AddEmployees(DataSet ds)
	{
		try
		{
			return GetUnitOfWork().EmpMasterData.SaveDocDataSet(ds, "A").Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public string UpdateEmployees(DataSet ds)
	{
		try
		{
			return GetUnitOfWork().EmpMasterData.SaveDocDataSet(ds, "U").Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public bool CheckExistEmployees(int empID)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("Select 1 from OHEM where \"empID\"=@empID;", CommandType.Text, new SqlParameter("@empID", empID));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return false;
	}

	public bool CheckExistEmployeesRelatives(int empID, int relativeId)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("Select 1 from APZ_OHER where \"empID\"=@empID and \"relaID\" = @relaId;", CommandType.Text, new SqlParameter("@empID", empID), new SqlParameter("@relaId", relativeId));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return false;
	}

	public DataSet GetAWBSTemplate(string code)
	{
		try
		{
			DataSet dataSet = new DataSet();
			DataTable dataByTableCode = GetDataByTableCode("APZ_OWBS", "Code", code);
			DataTable dataByTableCode2 = GetDataByTableCode("APZ_WBS1", "Code", code);
			DataTable dataByTableCode3 = GetDataByTableCode("APZ_TASK", "Code", code);
			dataSet.Tables.Add(dataByTableCode);
			dataSet.Tables.Add(dataByTableCode2);
			dataSet.Tables.Add(dataByTableCode3);
			dataSet.AcceptChanges();
			return dataSet;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public bool CheckExistWBSTemplate(string code)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("Select 1 from APZ_OWBS where \"Code\"=@Code;", CommandType.Text, new SqlParameter("@Code", code));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return false;
	}

	private DataTable GetDataByTableCode(string table, string columnName, object value)
	{
		try
		{
			return ExecuteDataTable("Select * From " + table + " where \"" + columnName + "\" = @value;", CommandType.Text, new SqlParameter("@value", value));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetAttachment(int atcEntry)
	{
		return GetDataByTableCode("ATC1", "AbsEntry", atcEntry);
	}

	public int GetManualSeries(string objectCode)
	{
		try
		{
			string query = "SELECT \"Series\" FROM nnm1 WHERE \"ObjectCode\"=@objectCode AND \"IsManual\"='Y';";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, new SqlParameter("@objectCode", objectCode));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ParseInt(dataTable.Rows[0][0]);
			}
			return 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return 0;
		}
	}

	public int GetUserID(string objectCode)
	{
		try
		{
			string query = "SELECT \"Series\" FROM nnm1 WHERE \"ObjectCode\"=@objectCode AND \"IsManual\"='Y';";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, new SqlParameter("@objectCode", objectCode));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ParseInt(dataTable.Rows[0][0]);
			}
			return 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return 0;
		}
	}

	public string SaveWithRollBack(DataSet ds, dynamic updateHb)
	{
		updateHb.UpdateStatus("START");
		try
		{
			if (ds.Tables.Count >= 3)
			{
				if (ds.Tables.Count - 2 == ds.Tables[1].Rows.Count)
				{
					using (DatabaseService.OpenConnection())
					{
						using DbTransaction dbTransaction = DatabaseService.BeginTransaction(IsolationLevel.RepeatableRead);
						IUnitOfWork unitOfWork = GetUnitOfWork();
						string text = "";
						int num = 0;
						ImportDataImpls importDataImpls = new ImportDataImpls();
						importDataImpls.ImportAction = (Enums.ImportActionEnum)Function.ParseInt(ds.Tables[0].Rows[0][0]);
						importDataImpls.UserID = 0;
						importDataImpls.ListDaTaImport = new List<ImportDTImpl>();
						for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
						{
							ImportDTImpl importDTImpl = new ImportDTImpl();
							importDTImpl.DTData = ds.Tables[i + 2];
							importDTImpl.ImportObjectCode = (Enums.ImportObjectEnum)Function.ParseInt(ds.Tables[1].Rows[i][0]);
							importDataImpls.ListDaTaImport.Add(importDTImpl);
						}
						for (int j = 0; j < importDataImpls.ListDaTaImport.Count; j++)
						{
							text = SaveImportObjectData(importDataImpls.ListDaTaImport[j], importDataImpls.ImportAction, importDataImpls.UserID, importDataImpls, unitOfWork, importDataImpls.ListDaTaImport, updateHb);
							if (!text.StartsWith("OK:"))
							{
								break;
							}
							num += Function.ParseInt(text.Remove(0, 3));
						}
						if (text.StartsWith("OK:"))
						{
							dbTransaction.Commit();
							updateHb.UpdateStatus("END");
							return "OK:" + num;
						}
						updateHb.UpdateStatus("END");
						return text;
					}
				}
				updateHb.UpdateStatus("END");
				return "Data Tables count in dataset not equal objects count in object table";
			}
			updateHb.UpdateStatus("END");
			return "No data table found";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			updateHb.UpdateStatus("END");
			return ex.Message;
		}
	}

	private string SaveImportObjectData(ImportDTImpl objImport, Enums.ImportActionEnum action, int userID, ImportDataImpls objectImport, IUnitOfWork unitOfWork, List<ImportDTImpl> ListDaTaImport, dynamic updateHb)
	{
		try
		{
			BaseImportData baseImportData = new BaseImportData();
			int num = 0;
			for (int i = 0; i < objImport.DTData.Rows.Count; i++)
			{
				string text = "";
				switch (objImport.ImportObjectCode)
				{
				case Enums.ImportObjectEnum.BusinessPartners:
					baseImportData = BaseImportDataFactory.GetBaseImportData("ImportDataBPMaster", DatabaseService.ConnectionString, userID, unitOfWork);
					text = ((ImportDataBPMaster)baseImportData).ImportBusinessPartners(objImport.DTData.Rows[i], action, ListDaTaImport);
					break;
				case Enums.ImportObjectEnum.BPAddresses:
					baseImportData = BaseImportDataFactory.GetBaseImportData("ImportDataBPMaster", DatabaseService.ConnectionString, userID, unitOfWork);
					text = ((ImportDataBPMaster)baseImportData).ImportBPChildObject(objImport.DTData.Rows[i], action, ListDaTaImport);
					break;
				case Enums.ImportObjectEnum.Contacts:
					baseImportData = BaseImportDataFactory.GetBaseImportData("ImportDataBPMaster", DatabaseService.ConnectionString, userID, unitOfWork);
					text = ((ImportDataBPMaster)baseImportData).ImportBPChildObject(objImport.DTData.Rows[i], action, ListDaTaImport);
					break;
				case Enums.ImportObjectEnum.BPBankAccounts:
					new ImportDataBPMaster(DatabaseService.ConnectionString, userID, unitOfWork);
					text = ((ImportDataBPMaster)baseImportData).ImportBPChildObject(objImport.DTData.Rows[i], action, ListDaTaImport);
					break;
				case Enums.ImportObjectEnum.BPPaymentDates:
					baseImportData = BaseImportDataFactory.GetBaseImportData("ImportDataBPMaster", DatabaseService.ConnectionString, userID, unitOfWork);
					text = ((ImportDataBPMaster)baseImportData).ImportBPChildObject(objImport.DTData.Rows[i], action, ListDaTaImport);
					break;
				case Enums.ImportObjectEnum.Items:
					baseImportData = BaseImportDataFactory.GetBaseImportData("ImportDataItemMaster", DatabaseService.ConnectionString, userID, unitOfWork);
					text = ((ImportDataItemMaster)baseImportData).ImportItems(objImport.DTData.Rows[i], action, ListDaTaImport);
					break;
				case Enums.ImportObjectEnum.ItemUnitOfMeasurement:
					baseImportData = BaseImportDataFactory.GetBaseImportData("ImportDataItemMaster", DatabaseService.ConnectionString, userID, unitOfWork);
					text = ((ImportDataItemMaster)baseImportData).ImportItemChildObject(objImport.DTData.Rows[i], action, ListDaTaImport);
					break;
				case Enums.ImportObjectEnum.ItemWarehouseInfo:
					baseImportData = BaseImportDataFactory.GetBaseImportData("ImportDataItemMaster", DatabaseService.ConnectionString, userID, unitOfWork);
					text = ((ImportDataItemMaster)baseImportData).ImportItemChildObject(objImport.DTData.Rows[i], action, ListDaTaImport);
					break;
				case Enums.ImportObjectEnum.Items_Prices:
					baseImportData = BaseImportDataFactory.GetBaseImportData("ImportDataItemMaster", DatabaseService.ConnectionString, userID, unitOfWork);
					text = ((ImportDataItemMaster)baseImportData).ImportItemChildObject(objImport.DTData.Rows[i], action, ListDaTaImport);
					break;
				case Enums.ImportObjectEnum.ProductTrees:
					baseImportData = BaseImportDataFactory.GetBaseImportData("ImportDataBillOfMaterials", DatabaseService.ConnectionString, userID, unitOfWork);
					text = ((ImportDataBillOfMaterials)baseImportData).SaveAProductTrees(objImport.DTData.Rows[i], action, objectImport);
					break;
				case Enums.ImportObjectEnum.ProductTrees_Lines:
					baseImportData = BaseImportDataFactory.GetBaseImportData("ImportDataBillOfMaterials", DatabaseService.ConnectionString, userID, unitOfWork);
					text = ((ImportDataBillOfMaterials)baseImportData).SaveAProductTrees_Lines(objImport.DTData.Rows[i], action, objectImport);
					break;
				case Enums.ImportObjectEnum.EmployeesInfo:
					baseImportData = BaseImportDataFactory.GetBaseImportData("ImportDataEmployees", DatabaseService.ConnectionString, userID, unitOfWork);
					text = ((ImportDataEmployees)baseImportData).ImportEmployees(objImport.DTData.Rows[i], action, ListDaTaImport);
					break;
				case Enums.ImportObjectEnum.EmployeesEducationInfo:
					baseImportData = BaseImportDataFactory.GetBaseImportData("ImportDataEmployees", DatabaseService.ConnectionString, userID, unitOfWork);
					text = ((ImportDataEmployees)baseImportData).ImportEmployeesChildObject(objImport.DTData.Rows[i], action, ListDaTaImport);
					break;
				case Enums.ImportObjectEnum.EmployeesReviewsInfo:
					baseImportData = BaseImportDataFactory.GetBaseImportData("ImportDataEmployees", DatabaseService.ConnectionString, userID, unitOfWork);
					text = ((ImportDataEmployees)baseImportData).ImportEmployeesChildObject(objImport.DTData.Rows[i], action, ListDaTaImport);
					break;
				case Enums.ImportObjectEnum.EmployeesPrevEmpoymentInfo:
					baseImportData = BaseImportDataFactory.GetBaseImportData("ImportDataEmployees", DatabaseService.ConnectionString, userID, unitOfWork);
					text = ((ImportDataEmployees)baseImportData).ImportEmployeesChildObject(objImport.DTData.Rows[i], action, ListDaTaImport);
					break;
				case Enums.ImportObjectEnum.EmployeesRolesInfo:
					baseImportData = BaseImportDataFactory.GetBaseImportData("ImportDataEmployees", DatabaseService.ConnectionString, userID, unitOfWork);
					text = ((ImportDataEmployees)baseImportData).ImportEmployeesChildObject(objImport.DTData.Rows[i], action, ListDaTaImport);
					break;
				case Enums.ImportObjectEnum.EmployeesContractsInfo:
					baseImportData = BaseImportDataFactory.GetBaseImportData("ImportDataEmployees", DatabaseService.ConnectionString, userID, unitOfWork);
					text = ((ImportDataEmployees)baseImportData).ImportEmployeesChildObject(objImport.DTData.Rows[i], action, ListDaTaImport);
					break;
				case Enums.ImportObjectEnum.EmployeesHealthInfo:
					baseImportData = BaseImportDataFactory.GetBaseImportData("ImportDataEmployees", DatabaseService.ConnectionString, userID, unitOfWork);
					text = ((ImportDataEmployees)baseImportData).ImportEmployeesChildObject(objImport.DTData.Rows[i], action, ListDaTaImport);
					break;
				}
				if (!string.IsNullOrEmpty(text))
				{
					string text2 = "ERR:" + (int)objImport.ImportObjectCode + ";" + text;
					updateHb.UpdateStatus(text2);
					return text2;
				}
				if (new Enums.ImportObjectEnum[14]
				{
					Enums.ImportObjectEnum.BPAddresses,
					Enums.ImportObjectEnum.Contacts,
					Enums.ImportObjectEnum.BPBankAccounts,
					Enums.ImportObjectEnum.BPPaymentDates,
					Enums.ImportObjectEnum.ItemUnitOfMeasurement,
					Enums.ImportObjectEnum.ItemWarehouseInfo,
					Enums.ImportObjectEnum.Items_Prices,
					Enums.ImportObjectEnum.ProductTrees_Lines,
					Enums.ImportObjectEnum.EmployeesContractsInfo,
					Enums.ImportObjectEnum.EmployeesHealthInfo,
					Enums.ImportObjectEnum.EmployeesPrevEmpoymentInfo,
					Enums.ImportObjectEnum.EmployeesReviewsInfo,
					Enums.ImportObjectEnum.EmployeesRolesInfo,
					Enums.ImportObjectEnum.EmployeesEducationInfo
				}.Contains(objImport.ImportObjectCode))
				{
					i--;
				}
				num++;
				updateHb.UpdateStatus("OK");
			}
			return "OK:" + num;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "ERROR:" + objImport.ImportObjectCode.ToString() + ";-1;" + ex.Message;
		}
	}

	public string TestHub(int count, dynamic updateHb)
	{
		updateHb.UpdateStatus("START");
		try
		{
			for (int i = 0; i < count; i++)
			{
				int num = 50000000;
				while (num > 2 && !IsPrime(num))
				{
					num--;
				}
				updateHb.UpdateStatus("OK");
			}
			updateHb.UpdateStatus("END");
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			updateHb.UpdateStatus("END");
			return ex.Message;
		}
	}

	private bool IsPrime(int num)
	{
		bool result = true;
		for (int i = 2; i < num / 2; i++)
		{
			if (num % i == 0)
			{
				result = false;
			}
		}
		return result;
	}
}
