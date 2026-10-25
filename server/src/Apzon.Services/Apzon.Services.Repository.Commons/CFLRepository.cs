using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Apzon.Commons;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Commons;

namespace Apzon.Services.Repository.Commons;

public class CFLRepository : BaseService<DataTable>, ICFL
{
	public CFLRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataSet LoadBusinessPartner(string cardType, string userName, string branch, string bpType = "")
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@cardType", cardType));
			list.Add(new SqlParameter("@userName", userName));
			list.Add(new SqlParameter("@branch", branch));
			list.Add(new SqlParameter("@bpType", bpType));
			return ExecuteData("Apz_Cfl_LoadBusinessPartner", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataSet GetListItem(string itemCode, string itemType, string userName, string branch, string pr, string listField = "")
	{
		try
		{
			string query = "Apz_Cfl_GetListItem";
			List<SqlParameter> list = new List<SqlParameter>();
			if (string.IsNullOrEmpty(itemCode))
			{
				itemCode = "";
			}
			if (string.IsNullOrEmpty(itemType))
			{
				itemType = "";
			}
			if (string.IsNullOrEmpty(userName))
			{
				userName = "";
			}
			if (string.IsNullOrEmpty(branch))
			{
				branch = "";
			}
			if (string.IsNullOrEmpty(pr))
			{
				pr = "";
			}
			list.Add(new SqlParameter("@itemCode", itemCode));
			list.Add(new SqlParameter("@itemType", itemType));
			list.Add(new SqlParameter("@userName", userName));
			list.Add(new SqlParameter("@branch", branch));
			list.Add(new SqlParameter("@CardCode", pr));
			list.Add(new SqlParameter("@DocDate", null));
			list.Add(new SqlParameter("@Quantity", 1));
			list.Add(new SqlParameter("@subCatNum", ""));
			list.Add(new SqlParameter("@listField", listField));
			return ExecuteData(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataSet GetListItemFilter(string itemCode, string itemType, string userName, string branch, string pr)
	{
		try
		{
			string query = "Apz_Cfl_GetListItemFilter";
			List<SqlParameter> list = new List<SqlParameter>();
			if (!string.IsNullOrEmpty(itemCode))
			{
				list.Add(new SqlParameter("@itemCode", itemCode));
			}
			if (!string.IsNullOrEmpty(itemType))
			{
				list.Add(new SqlParameter("@itemType", itemType));
			}
			if (!string.IsNullOrEmpty(userName))
			{
				list.Add(new SqlParameter("@userName", userName));
			}
			if (!string.IsNullOrEmpty(branch))
			{
				list.Add(new SqlParameter("@branch", branch));
			}
			if (!string.IsNullOrEmpty(pr))
			{
				list.Add(new SqlParameter("@CardCode", pr));
			}
			return ExecuteData(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataSet GetDataByStore(string storeName, string value, string userName, string branch)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@value", value));
			list.Add(new SqlParameter("@userName", userName));
			list.Add(new SqlParameter("@branch", branch));
			return ExecuteData(storeName, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message, storeName);
		}
		return new DataSet();
	}

	public DataSet GetProject(string prjCode, string userName, string branch)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			if (!string.IsNullOrEmpty(prjCode))
			{
				list.Add(new SqlParameter("@ProjectCode", prjCode));
			}
			if (!string.IsNullOrEmpty(userName))
			{
				list.Add(new SqlParameter("@UserName", userName));
			}
			if (!string.IsNullOrEmpty(userName))
			{
				list.Add(new SqlParameter("@Branch", branch));
			}
			return ExecuteData("Apz_Cfl_GetProject", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataSet GetListMasterResource(string resType)
	{
		try
		{
			return ExecuteData("Apz_Cfl_GetListMasterResource", CommandType.StoredProcedure, new SqlParameter("@ResType", resType));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataTable GetListEmployees()
	{
		try
		{
			return ExecuteDataTable("Apz_Cfl_GetListEmployees", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataSet GetResource(string rescode, string userName, string branch)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			if (!string.IsNullOrEmpty(rescode))
			{
				list.Add(new SqlParameter("@Resource", rescode));
			}
			if (!string.IsNullOrEmpty(userName))
			{
				list.Add(new SqlParameter("@userName", userName));
			}
			if (!string.IsNullOrEmpty(branch))
			{
				list.Add(new SqlParameter("@branch", branch));
			}
			return ExecuteData("Apz_Cfl_GetResource", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public string ValidateDistrRule(string value)
	{
		try
		{
			string text = "";
			string query = "APZ_ValidateDistrRule";
			List<SqlParameter> list = new List<SqlParameter>();
			value = Function.ToString(value);
			string[] array = value.Split(';');
			for (int i = 0; i < array.Length; i++)
			{
				string value2 = array[i];
				list.Add(new SqlParameter("@OcrCode" + (i + 1), value2));
			}
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				foreach (DataRow item in dataTable.AsEnumerable())
				{
					text = text + Function.ToString(item[0]) + ";";
				}
				while (text.Length > 0 && text[text.Length - 1].Equals(';'))
				{
					text = text.Remove(text.Length - 1);
				}
			}
			return text;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), $"ValidateDistrRule FAILED ", ex.Message);
			return "error";
		}
	}

	public DataSet GetListOfInstallBase(string customer, string itemCode, string userName, string branch)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			if (!string.IsNullOrEmpty(customer))
			{
				list.Add(new SqlParameter("@customer", customer));
			}
			if (!string.IsNullOrEmpty(itemCode))
			{
				list.Add(new SqlParameter("@itemCode", itemCode));
			}
			if (!string.IsNullOrEmpty(userName))
			{
				list.Add(new SqlParameter("@userName", userName));
			}
			if (!string.IsNullOrEmpty(branch))
			{
				list.Add(new SqlParameter("@branch", branch));
			}
			return ExecuteData("Apz_Cfl_LoadListOfInstalBill", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataSet GetListWareHouse(string wareHouseCode, string wareHouseName, string branch)
	{
		try
		{
			string query = "Apz_Cfl_GetListWarehouse";
			List<SqlParameter> list = new List<SqlParameter>();
			if (!string.IsNullOrEmpty(wareHouseCode))
			{
				list.Add(new SqlParameter("@WareHouseCode", wareHouseCode));
			}
			else
			{
				list.Add(new SqlParameter("@WareHouseCode", string.Empty));
			}
			if (!string.IsNullOrEmpty(wareHouseName))
			{
				list.Add(new SqlParameter("@WareHouseName", wareHouseName));
			}
			else
			{
				list.Add(new SqlParameter("@WareHouseName", string.Empty));
			}
			if (!string.IsNullOrEmpty(branch))
			{
				list.Add(new SqlParameter("@branch", branch));
			}
			return ExecuteData(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataSet GetListCostCenter()
	{
		try
		{
			string query = "APZ_GetListOfCostCenter";
			return ExecuteData(query, CommandType.StoredProcedure, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataTable GetAllOcrByDistRule()
	{
		try
		{
			string allOcrByDistRuleQuery = GetAllOcrByDistRuleQuery();
			return ExecuteDataTable(allOcrByDistRuleQuery, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	protected virtual string GetAllOcrByDistRuleQuery()
	{
		return "SELECT DimCode, DimDesc, '' OcrCode FROM ODIM WHERE  DimActive='Y' order by DimCode";
	}

	public DataSet GetListDefinedShift(int branch)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@branch", branch);
			return ExecuteData("APZ_CFL_GetListDefinedShift", CommandType.StoredProcedure, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetListAdvanceForecast()
	{
		try
		{
			return ExecuteData("APZ_CFL_GetListAdvanceForecast", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataSet GetListMrp(string mrpCode, string mrpStatus, string forecast, string mrpType)
	{
		try
		{
			return ExecuteData("APZ_CFL_GetListMRP", CommandType.StoredProcedure, new SqlParameter("@code", mrpCode), new SqlParameter("@status", mrpStatus), new SqlParameter("@type", mrpType), new SqlParameter("@forecast", forecast));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataSet GetListSchedule(string scheduleCode, string mrpCode, string status, string mrpType, string scheduleType, string isGenCapacity)
	{
		try
		{
			return ExecuteData("APZ_CFL_GetListSchedule", CommandType.StoredProcedure, new SqlParameter("@code", scheduleCode), new SqlParameter("@MRPCode", mrpCode), new SqlParameter("@status", status), new SqlParameter("@mrpType", mrpType), new SqlParameter("@scheduleType", scheduleType), new SqlParameter("@isGenCapacity", isGenCapacity));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataTable GetListItemsMRPItemCode(int bplId)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@BPLId", bplId));
			return ExecuteDataTable("select \"ItemCode\" from APZ_LIN1 a where \r\n                                    exists(select \"LineID\" From APZ_OLIN t where t.\"LineID\" = a.\"LineID\" and t.\"Branch\" = @BPLId);", CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataSet GetListMRPUsers()
	{
		try
		{
			return ExecuteData("APZ_CFL_GetListMRPUsers", CommandType.StoredProcedure, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataSet GetListDataLines(int branch)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@branch", branch);
			return ExecuteData("APZ_CFL_GetListDataLines", CommandType.StoredProcedure, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return null;
		}
	}

	public DataTable LoadDataAdvaceForecast(string lstForecast)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@Forecast", lstForecast);
			return ExecuteDataTable("Select Code, Name from APZ_OFCT where Code in (select Data from Split(@Forecast,','))", CommandType.Text, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataSet GetListBomItem(string code)
	{
		try
		{
			return ExecuteData("APZ_CFL_GetListBomItem", CommandType.StoredProcedure, new SqlParameter("@Code", code));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetBatchSerialBinNumbers(string itemCode, string whsCode, string objtype)
	{
		try
		{
			return ExecuteData("apz_cfl_batchserialBin", CommandType.StoredProcedure, new SqlParameter("@ItemCode", itemCode), new SqlParameter("@WhsCode", whsCode), new SqlParameter("@ObjType", objtype));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetBatchSerialNumbers(string itemCode, string whsCode, string objtype, string bpCode)
	{
		try
		{
			return ExecuteData("apz_cfl_batchserial", CommandType.StoredProcedure, new SqlParameter("@ItemCode", itemCode), new SqlParameter("@WhsCode", whsCode), new SqlParameter("@ObjType", objtype), new SqlParameter("@bpCode", bpCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetBatchSerialForReturnComponents(int baseEntry, int baseLine, string whsCode)
	{
		try
		{
			return ExecuteData("apz_cfl_BatchSerialForReturnComponents", CommandType.StoredProcedure, new SqlParameter("@baseEntry", baseEntry), new SqlParameter("@baseLine", baseLine), new SqlParameter("@whsCode", whsCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataTable GetInfoColum(DataTable dt, string userCode)
	{
		try
		{
			if (dt == null || dt.Rows.Count == 0)
			{
				return new DataTable();
			}
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				CreateTempTable(dt, "#tempTable");
				BulkCopy(dt, "#tempTable");
				DataTable result = ExecuteDataTable("APZ_CFL_GetInfoColum", CommandType.StoredProcedure, new SqlParameter("@userCode", userCode));
				dbTransaction.Rollback();
				return result;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataSet GetDashBoardController()
	{
		try
		{
			new List<SqlParameter>();
			return ExecuteData("Apz_CFL_GetControllers", CommandType.StoredProcedure, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataSet GetDataTransferRequestCopyFrom(string fromWhsCode, string toWhsCode)
	{
		try
		{
			return ExecuteData("APZ_CFL_GetDataTransferRequestCopyFrom", CommandType.StoredProcedure, new SqlParameter("@fromWhsCode", fromWhsCode), new SqlParameter("@toWhsCode", toWhsCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetDataByQuery(string query)
	{
		try
		{
			return ExecuteData(query, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message, query);
		}
		return new DataSet();
	}

	public DataSet GetDataByProcedure(string storeName)
	{
		try
		{
			return ExecuteData(storeName, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message, storeName);
		}
		return new DataSet();
	}

	public DataSet GetDataBanks()
	{
		try
		{
			new List<SqlParameter>();
			return ExecuteData("Apz_CFL_GetDataBanks", CommandType.StoredProcedure, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataSet GetDataSalesOder()
	{
		try
		{
			return ExecuteData("APZ_CFL_GetDataSalesOder", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetDataFactoringIndicator()
	{
		try
		{
			return ExecuteData("APZ_CFL_GetDataFactoringIndicator", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetDataEmployeesTechnician()
	{
		try
		{
			return ExecuteData("APZ_CFL_GetDataEmployeesTechnician", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetDataTerritory()
	{
		try
		{
			return ExecuteData("APZ_CFL_GetDataTerritory", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataTable LoadDataMrpSalesOrder(string lstMrpSalesOrder)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@lstMrpSalesOrder", lstMrpSalesOrder);
			return ExecuteDataTable("Select DocEntry, DocNum, CardCode, CardName, DocDate from ORDR where DocNum in (select Data from Split(@lstMrpSalesOrder,','))", CommandType.Text, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataSet GetDataListUoMByUoMGroup(int entry)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@Entry", entry));
			return ExecuteData("APZ_CFL_GetDataListUoMByUoMGroup", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetDataListUoMNameItemMaster()
	{
		try
		{
			return ExecuteData("APZ_CFL_GetDataListUoMNameItemMaster", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetDataListUoMs()
	{
		try
		{
			return ExecuteData("APZ_CFL_GetDataListUoMs", CommandType.StoredProcedure, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetDataListAlterUoMs(string lstUomEntry)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@UomEntry", lstUomEntry);
			return ExecuteData("APZ_CFL_GetDataListAlterUoMs", CommandType.StoredProcedure, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetDataListUoMForUoMPrices(string itemCode, int priceList, int ugpEntry)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@ItemCode", itemCode));
			list.Add(new SqlParameter("@PriceList", priceList));
			list.Add(new SqlParameter("@ugpEntry", ugpEntry));
			return ExecuteData("APZ_CFL_GetDataListUoMForUoMPrices", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetDataChartOfAccount()
	{
		try
		{
			return ExecuteData("APZ_CFL_GetDataChartOfAccount", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetDataUomCode(int uomGroup)
	{
		try
		{
			return ExecuteData("APZ_Cfl_GetDataUomCode", CommandType.StoredProcedure, new SqlParameter("@uomGroup", uomGroup));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet ListOfBinLocationByWareHouse(string whsCode)
	{
		try
		{
			return ExecuteData("APZ_Cfl_ListOfBinLocationByWareHouse", CommandType.StoredProcedure, new SqlParameter("@WhsCode", whsCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetProductionOrder(string type)
	{
		try
		{
			return ExecuteData("apz_cfl_productionorder", CommandType.StoredProcedure, new SqlParameter("@Type", type));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetListSalesPerson()
	{
		try
		{
			return ExecuteData("APZ_Cfl_GetDataListSalesEmployee", CommandType.StoredProcedure, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetListOfCostCenterByDimCode(int dimCode)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@DimCode", dimCode);
			return ExecuteData("APZ_Cfl_GetListOfCostCenterByDimCode", CommandType.StoredProcedure, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetEmployeeListForSalesEmpSetup()
	{
		try
		{
			return ExecuteData("APZ_Cfl_GetEmployeeListForSalesEmpSetup", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetDataBpCatalogItem(string bpCode, string subCatNum, string userName, string docdate, string branch, string itemCode)
	{
		try
		{
			return ExecuteData("APZ_CFL_GetDataBpCatalogItem", CommandType.StoredProcedure, new SqlParameter("@bpCode", bpCode), new SqlParameter("@subCatNum", subCatNum), new SqlParameter("@userName", userName), new SqlParameter("@DocDate", Function.ParseDateTime(docdate, "yyyyMMdd")), new SqlParameter("@branch", branch), new SqlParameter("@itemCode", itemCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetListInventoryCounting(string countDate, int countTime, string status, int series, string ref2, int docNum, string wareHouse)
	{
		try
		{
			return ExecuteData("APZ_InventoryCounting_GetListInventoryCounting", CommandType.StoredProcedure, new SqlParameter("@countDate", countDate), new SqlParameter("@countTime", countTime), new SqlParameter("@status", status), new SqlParameter("@series", series), new SqlParameter("@ref2", ref2), new SqlParameter("@docNum", docNum), new SqlParameter("@wareHouse", wareHouse));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataSet GetListInventoryCyles(DataTable search)
	{
		try
		{
			return ExecuteData("APZ_InventoryCyles_GetListInventoryCyles", CommandType.StoredProcedure, new SqlParameter("@name", search.Rows[0]["Name"]), new SqlParameter("@frequency", search.Rows[0]["Frequency"]), new SqlParameter("@subOption", search.Rows[0]["SubOption"]), new SqlParameter("@interval", search.Rows[0]["Interval"]), new SqlParameter("@nextDate", search.Rows[0]["NextDate"]), new SqlParameter("@enType", search.Rows[0]["EndType"]), new SqlParameter("@maxOccur", search.Rows[0]["MaxOccur"]), new SqlParameter("@seEndDate", search.Rows[0]["SeEndDat"]), new SqlParameter("@dayInMonth", search.Rows[0]["DayInMonth"]), new SqlParameter("@week", search.Rows[0]["Week"]), new SqlParameter("@dayOfWeek", search.Rows[0]["DayOfWeek"]), new SqlParameter("@month", search.Rows[0]["Month"]));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataSet GetDataBOMCFLOfProduct()
	{
		try
		{
			return ExecuteData("APZ_CFL_GetDataBOMCFLOfProduct", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetQualificationList()
	{
		try
		{
			return ExecuteData("APZ_Cfl_GetQualificationList", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetApplicantList()
	{
		try
		{
			return ExecuteData("APZ_Cfl_GetApplicantList", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetListQuestion()
	{
		try
		{
			return ExecuteData("APZ_JobVacancy_GetListQuestion", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetListAtp(string type, string status, string outputType)
	{
		try
		{
			return ExecuteData("APZ_CFL_GetListAtp", CommandType.StoredProcedure, new SqlParameter("@bplId", -1), new SqlParameter("@type", type), new SqlParameter("@status", status), new SqlParameter("@outputType", outputType));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet FindWorkPeriod(string status, string code, string name)
	{
		try
		{
			return ExecuteData("APZ_WorkPeriod_FindWorkPeriod", CommandType.StoredProcedure, new SqlParameter("@status", status), new SqlParameter("@code", code), new SqlParameter("@name", name));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataSet GetListScheduleAtp(string atpCode, string atpScheduleType, string atpScheduleStatus, string atpScheduleCode)
	{
		try
		{
			return ExecuteData("APZ_CFL_GetListScheduleAtp", CommandType.StoredProcedure, new SqlParameter("@atpCode", Function.ParseInt(atpCode)), new SqlParameter("@atpScheduleType", atpScheduleType), new SqlParameter("@atpScheduleStatus", atpScheduleStatus), new SqlParameter("@atpScheduleCode", Function.ParseInt(atpScheduleCode)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataTable LoadDataProductionMaterialShortageReport(string listXcludeOrder)
	{
		try
		{
			return ExecuteDataTable("APZ_CFL_LoadDataProductionMaterialShortageReport", CommandType.StoredProcedure, new SqlParameter("@orderNo", listXcludeOrder));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataSet GetLanedCostsList()
	{
		try
		{
			return ExecuteData("APZ_CFL_GetLandedCostList", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet LandedCostCopyFrom(string vendor)
	{
		try
		{
			return ExecuteData("APZ_Cfl_LandedCostCopy", CommandType.StoredProcedure, new SqlParameter("@vendor", vendor));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet LandedCostGodInvoice(string vendor, int objectType)
	{
		try
		{
			return ExecuteData("APZ_Cfl_LandedCostGodInvoice", CommandType.StoredProcedure, new SqlParameter("@vendor", vendor), new SqlParameter("@objectType", objectType));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetTableNameORW(string objtype)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@type", objtype);
			return ExecuteData("APZ_CFL_GetTableNameODO", CommandType.StoredProcedure, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetSalesOppPartner()
	{
		try
		{
			return ExecuteData("APZ_CFL_GetSalesOppPartner", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetEmployees(int branch, string value)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@value", value));
			list.Add(new SqlParameter("@empBranch", branch));
			return ExecuteData("Apz_Cfl_EmployleeData", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetDataListBOMActive()
	{
		try
		{
			return ExecuteData("APZ_CFL_ListBOM", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetEmployeeContract(int empID)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@empID", empID);
			return ExecuteData("APZ_Cfl_GetEmployeeContract", CommandType.StoredProcedure, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetDataBPControlAccounts(string cardType, string acctType)
	{
		try
		{
			return ExecuteData("APZ_Cfl_GetDataBPControlAccounts", CommandType.StoredProcedure, new SqlParameter("@cardType", cardType), new SqlParameter("@acctType", acctType));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetDataBOMForProduction()
	{
		try
		{
			return ExecuteData("APZ_Cfl_GetDataBOMForProduction", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetItemWarrantyTemplates(string type)
	{
		try
		{
			return ExecuteData("APZ_Cfl_GetItemWarrantyTemplates", CommandType.StoredProcedure, new SqlParameter("@CntrctType", type));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetUserList()
	{
		try
		{
			return ExecuteData("APZ_Cfl_GetUserList", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetSolution()
	{
		try
		{
			return ExecuteData("APZ_Cfl_GetSolution", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet UserDefinedTexts()
	{
		try
		{
			return ExecuteData("DECLARE @tblStructure AS TABLE\r\n                                            (\r\n                                             ColumnName NVARCHAR(50) NOT NULL\r\n                                            , Caption NVARCHAR(100) NULL\r\n                                            , DataType VARCHAR(20) NULL DEFAULT 'nvarchar' \r\n                                            , SubType VARCHAR(20) NULL  \r\n                                            , Width INT NULL DEFAULT 75\r\n                                            , GroupIndex INT NULL DEFAULT - 1 \r\n                                            , Visible BIT NULL DEFAULT 1\r\n                                            , FormatType NVARCHAR(20) NULL \r\n                                            , DisplayFormat NVARCHAR(20) NULL \r\n                                            ,[ReadOnly] BIT NULL DEFAULT 0\r\n                                            , AllowEdit BIT NULL DEFAULT 1\r\n                                            , AllowMove BIT NULL DEFAULT 1\r\n                                            , AllowSize BIT NULL DEFAULT 1\r\n                                            , AllowSort BIT NULL DEFAULT 1\r\n                                            , Fixed VARCHAR(1) NULL \r\n                                            -- Summary Item\r\n                                            , SummaryType VARCHAR(10) NULL \r\n\r\n                                          -- Group Summary\r\n                                          , GroupSummaryType VARCHAR(10) NULL \r\n\r\n                                         --,Parent BIT NULL \r\n                                        --,[Key] BIT NULL \r\n                                            )\r\n                                            INSERT INTO @tblStructure\r\n                                                        (ColumnName, Caption, DataType, Width, GroupIndex, Visible, FormatType, DisplayFormat, ReadOnly, AllowEdit, AllowMove, AllowSize, AllowSort, Fixed, SummaryType, GroupSummaryType)\r\n                                                VALUES  (N'TextCode',\t\t\tN'TextCode',\t\t\t'nvarchar',\t\t\t150,\t-1,\t\t\t1,\t\t\tNULL,\t\tNULL,\t\t\t1,\t\t 0,\t\t\t1,\t\t\t1,\t\t\t1,\t\t 'L',\tNULL,\t\t NULL)\r\n\t\t\t\t                                       ,(N'Text',\t\t\tN'Text',\t\t\t'nvarchar',\t\t\t200,\t-1,\t\t\t1,\t\t\tNULL,\t\tNULL,\t\t\t1,\t\t 0,\t\t\t1,\t\t\t1,\t\t\t1,\t\t 'L',\tNULL,\t\t NULL)\r\n\t\t\t\r\n                                        SELECT*\r\n                                        FROM @tblStructure\r\n\r\n\r\n\r\n                                        SELECT*\r\n                                        FROM OPDT", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetDataProjectTask(int projectId)
	{
		try
		{
			return ExecuteData("APZ_CFL_GetDataProjectTask", CommandType.StoredProcedure, new SqlParameter("@projectId", projectId));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetDataProjectManager(int projectid)
	{
		try
		{
			return ExecuteData("APZ_CFL_GetDataProjectManager", CommandType.StoredProcedure, new SqlParameter("@ProjectId", projectid));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetItemBySaleOrder(int orderId)
	{
		try
		{
			return ExecuteData("APZ_CFL_GetItemBySaleOrder", CommandType.StoredProcedure, new SqlParameter("@orderId", orderId));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetBOMTemplateBySaleOrder()
	{
		try
		{
			return ExecuteData("APZ_CFL_GetBOMTemplateBySaleOrder", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetDataTrackingIssue()
	{
		try
		{
			return ExecuteData("APZ_CFL_GetDataTrackingIssue", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetDataProjectReportCompletion(int projectId)
	{
		try
		{
			return ExecuteData("APZ_Cfl_ProjectReportCompletion", CommandType.StoredProcedure, new SqlParameter("@projectId", projectId));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetDataWBSInTabItem(int projectId)
	{
		try
		{
			return ExecuteData("APZ_CFL_GetWbsInBudgetItem", CommandType.StoredProcedure, new SqlParameter("@projectId", projectId));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetDataItemInTabItem(int projectId, string wbs)
	{
		try
		{
			return ExecuteData("APZ_CFL_GetItemInWbs", CommandType.StoredProcedure, new SqlParameter("@projectId", projectId), new SqlParameter("@wbs", wbs));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataTable GetAllDocumentByWbs(int objectType, int absEntry, int lineNum)
	{
		try
		{
			return ExecuteDataTable("APZ_Cfl_ProjectManagerItemDocument", CommandType.StoredProcedure, new SqlParameter("@objectType", objectType), new SqlParameter("@absEntry", absEntry), new SqlParameter("@lineNum", lineNum));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataSet GetAllProjectManager()
	{
		try
		{
			return ExecuteData("APZ_CFL_GetAllProjectManager", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet UserDefinedReport()
	{
		try
		{
			return ExecuteData("APZ_CFL_UserDefinedReport", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetDataDeliveryTypes()
	{
		try
		{
			return ExecuteData("APZ_CFL_GetDataDeliveryTypes", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetDataLanguages()
	{
		try
		{
			return ExecuteData("APZ_CFL_GetDataLanguages", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetBinOfItemGroupsWarehouse(string whsCode)
	{
		try
		{
			return ExecuteData("APZ_CFL_GetBinOfItemGroupsWarehouse", CommandType.StoredProcedure, new SqlParameter("@whsCode", whsCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetDataDefaultForUser()
	{
		try
		{
			return ExecuteData("APZ_CFL_GetDataDefaultForUser", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetBatchSerial(string itemCode, string whsCode)
	{
		try
		{
			return ExecuteData("APZ_CFl_InventoryBatchSerial", CommandType.StoredProcedure, new SqlParameter("@ItemCode", itemCode), new SqlParameter("@WhsCode", whsCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataSet GetDataInventoryCountingRequest()
	{
		try
		{
			return ExecuteData("APZ_CFL_InventoryCountingRequest", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetDataBin(string whsCode, string itemCode, int tranType)
	{
		try
		{
			return ExecuteData("APZ_CFl_GetDataBin", CommandType.StoredProcedure, new SqlParameter("@WhsCode", whsCode), new SqlParameter("@itemCode", itemCode), new SqlParameter("@tranType", tranType));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataTable LoadDataLstSchedule(string LstSchedule)
	{
		try
		{
			return ExecuteDataTable("APZ_CFL_LoadDataLstSchedule", CommandType.StoredProcedure, new SqlParameter("@lstSche", LstSchedule));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataSet GetListOperation()
	{
		try
		{
			return ExecuteData("APZ_CFL_GetListOperation", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetListSemiOfProduct(string producCode)
	{
		try
		{
			return ExecuteData("APZ_CFL_GetListSemiOfProduct", CommandType.StoredProcedure, new SqlParameter("@producCode", producCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataTable LoadDataWorkSchedule()
	{
		try
		{
			return ExecuteDataTable("select \"Id\" as \"CalCode\",\"Name\",\"FromDate\",\"ToDate\",\"Notes\" from apz_OWSH;", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataSet GetListWorkCenter()
	{
		try
		{
			return ExecuteData("APZ_CFL_GetListWorkCenter", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetListAdvanceRouting(string proCode)
	{
		try
		{
			return ExecuteData("APZ_CFL_GetListAdvanceRouting", CommandType.StoredProcedure, new SqlParameter("@ProCode", Function.ToString(proCode)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetListTask()
	{
		try
		{
			return ExecuteData("APZ_CFL_GetListTask", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet ListItemByCardCode(string cardCode, string type)
	{
		try
		{
			return ExecuteData("APZ_CFL_ItemByCardCodeWarranty", CommandType.StoredProcedure, new SqlParameter("@cardCode", cardCode), new SqlParameter("@type", type));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataSet ListServiceContractTemplate(string type)
	{
		try
		{
			return ExecuteData("APZ_CFL_ListServiceContractTemplate", CommandType.StoredProcedure, new SqlParameter("@type", type));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataTable GetDataServiceLevelAgreement()
	{
		try
		{
			return ExecuteDataTable("select SLAID, \"SLACode\", \"SLAName\" ,\"Remark\" from APZ_OSLA;", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public DataSet GetListItemForServiceCall(string cardCode)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@CardCode", cardCode);
			return ExecuteData("Apz_Cfl_GetListItemForServiceCall", CommandType.StoredProcedure, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataSet();
		}
	}

	public DataSet GetListSerialNum(string itemCode)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@ItemCode", itemCode);
			return ExecuteData("Apz_Cfl_GetSerialNum", CommandType.StoredProcedure, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataSet();
		}
	}

	public DataSet GetMarketingCampaign(int campaignId)
	{
		try
		{
			return ExecuteData("APZ_CFL_GetMarketingCampaign", CommandType.StoredProcedure, new SqlParameter("@campaignId", campaignId));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetWbsTemplate()
	{
		try
		{
			return ExecuteData("APZ_CFL_GetWbsTemplate", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetAllOpportunities()
	{
		try
		{
			return ExecuteData("APZ_CFL_GetAllOpportunities", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetListCapacity(string capacityStatus)
	{
		try
		{
			return ExecuteData("APZ_CFL_GetListCapacity", CommandType.StoredProcedure, new SqlParameter("@capacityStatus", capacityStatus));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetListResourceWorkSchedule(string status)
	{
		try
		{
			return ExecuteData("APZ_CFL_GetListResourceWorkSchedule", CommandType.StoredProcedure, new SqlParameter("@status", status));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetBioDataList(int branch, string period)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@period", period));
			list.Add(new SqlParameter("@branch", branch));
			return ExecuteData("Apz_Cfl_GetBioDataList", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetBioMapping(int machineNo)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@machine", machineNo));
			return ExecuteData("Apz_Cfl_GetBioMapping", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetAllTerritory()
	{
		try
		{
			return ExecuteData("APZ_CFL_GetAllTerritory", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetAllGeography()
	{
		try
		{
			return ExecuteData("APZ_CFL_GetAllGeography", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetAllEvents()
	{
		try
		{
			return ExecuteData("APZ_CFL_GetAllEvents", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetGeographyDetails(string countryCode, string fatherCode)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@countryCode", Function.ToString(countryCode)));
			list.Add(new SqlParameter("@fatherCode", Function.ToString(fatherCode)));
			return ExecuteData("APZ_CFL_GetGeographyDetails", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetStore(string ggpCode)
	{
		try
		{
			return ExecuteData("APZ_CFL_GetStore", CommandType.StoredProcedure, new SqlParameter("@ggpCode", Function.ToString(ggpCode)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetStoreGeography(string parentCode)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@parentCode", Function.ToString(parentCode)));
			return ExecuteData("APZ_CFL_StoreGeography", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new DataSet();
		}
	}

	public DataSet GetPO()
	{
		try
		{
			return ExecuteData("Apz_Cfl_GetPO", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetPOforCopy()
	{
		try
		{
			return ExecuteData("Apz_Cfl_GetPOforCopy", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetPOforWrap()
	{
		try
		{
			return ExecuteData("Apz_Cfl_GetPOforWrap", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetCase()
	{
		try
		{
			return ExecuteData("Apz_Cfl_GetCase", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetTypeCase()
	{
		try
		{
			return ExecuteData("Apz_Cfl_GetTypeCase", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetTC()
	{
		try
		{
			return ExecuteData("Apz_Cfl_GetTC", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetItem()
	{
		try
		{
			return ExecuteData("Apz_Cfl_GetItem", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetProductionOrder()
	{
		try
		{
			return ExecuteData("Apz_Cfl_GetProductionOrder", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetUsers()
	{
		try
		{
			return ExecuteData("APZ_CFL_Users", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetDraftNo()
	{
		try
		{
			return ExecuteData("APZ_CFL_DraftNo", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetDraftType()
	{
		try
		{
			return ExecuteData("APZ_CFL_DraftType", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetWorkingProcessRequest(int userId)
	{
		try
		{
			return ExecuteData("APZ_CFL_WorkingProcessRequest", CommandType.StoredProcedure, new SqlParameter("@userId", userId));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetBinLocAttribute(int fldAbs)
	{
		try
		{
			string query = "APZ_CFL_GetBinLocAttribute";
			return ExecuteData(query, CommandType.StoredProcedure, new SqlParameter("@fldAbs", fldAbs));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetWarehouseSublevel(int fldAbs)
	{
		try
		{
			string query = "APZ_CFL_GetWarehouseSublevelCode";
			return ExecuteData(query, CommandType.StoredProcedure, new SqlParameter("@fldAbs", fldAbs));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetWarehouseBinActivated()
	{
		try
		{
			string query = "APZ_CFL_GetWarehouseBinActivated";
			return ExecuteData(query, CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetAllReportDMS()
	{
		try
		{
			string query = "APZ_D_CFL_GetAllReportDMS";
			return ExecuteData(query, CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetChannel()
	{
		try
		{
			string query = "APZ_CFL_Channel";
			return ExecuteData(query, CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetSalesRoute()
	{
		try
		{
			string query = "APZ_CFL_GetSalesRoute";
			return ExecuteData(query, CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetSalesRouteEmployees()
	{
		try
		{
			string query = "APZ_CFL_GetSalesRouteEmployees";
			return ExecuteData(query, CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetSeriesOutlet()
	{
		try
		{
			string query = "APZ_CFL_GetSeriesOutlet";
			return ExecuteData(query, CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetBusinessPartner()
	{
		try
		{
			string query = "APZ_D_CFL_GetBusinessPartner";
			return ExecuteData(query, CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetDistributors()
	{
		try
		{
			return ExecuteData("APZ_D_CFL_GetDistributors", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetWorkingProcessTooltips()
	{
		try
		{
			return ExecuteData("APZ_CFL_WorkingProcessTooltips", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetListContractTemplate()
	{
		try
		{
			string query = "APZ_CFL_GetListContractTemplate";
			return ExecuteData(query, CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetKPIsMaster()
	{
		try
		{
			string query = "APZ_CFL_GetKPIsMaster";
			return ExecuteData(query, CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetApprovalUsers(int departmentCode)
	{
		try
		{
			string query = "APZ_CFL_ApprovalUsers";
			return ExecuteData(query, CommandType.StoredProcedure, new SqlParameter("@departmntCode", departmentCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetDepartments()
	{
		try
		{
			return ExecuteData("APZ_CFL_Departments", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetApprovalStages()
	{
		try
		{
			return ExecuteData("APZ_CFL_ApprovalStages", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetApprovalTemplateStages()
	{
		try
		{
			return ExecuteData("APZ_CFL_ApprovalTemplatesStages", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetListKPIs()
	{
		try
		{
			string query = "APZ_D_CFL_GetListKPIs";
			return ExecuteData(query, CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetPromotionCondition(string conditionType)
	{
		try
		{
			return ExecuteData("APZ_D_CFL_GetPromotionCondition", CommandType.StoredProcedure, new SqlParameter("@ConditionType", conditionType ?? ""));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		return new DataSet();
	}

	public DataSet GetPromotionBenefit(string benefitType)
	{
		try
		{
			string query = "APZ_D_CFL_GetPromotionBenefit";
			return ExecuteData(query, CommandType.StoredProcedure, new SqlParameter("@BenefitType", benefitType ?? ""));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet GetKPIsSettings()
	{
		try
		{
			string query = "APZ_CFL_GetKPIsSettings";
			return ExecuteData(query, CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataSet PreDecisionTask(int clgCode)
	{
		try
		{
			string query = "APZ_CFL_GetPreDecisionTask";
			return ExecuteData(query, CommandType.StoredProcedure, new SqlParameter("@clgCode", clgCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new DataSet();
		}
	}

	public DataSetResult GetCflStructure(ApzObjectTypes objType)
	{
		try
		{
			string text = "";
			switch (objType)
			{
			case ApzObjectTypes.oItems:
				text = "APZ_ITEM_GetSearchStructure";
				break;
			case ApzObjectTypes.oBusinessPartners:
				text = "APZ_BP_GetSearchStructure";
				break;
			case ApzObjectTypes.oProductTrees:
				text = "APZ_BOM_GetSearchStructure";
				break;
			case ApzObjectTypes.oProductionOrders:
				text = "APZ_ProductionOrder_GetSearchStructure";
				break;
			}
			if (string.IsNullOrEmpty(text))
			{
				return new DataSetResult("Cannot get CFL structure");
			}
			return new DataSetResult(ExecuteData(text, CommandType.StoredProcedure));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSetResult(ex.Message);
		}
	}

	public DataSet GetPriceList()
	{
		try
		{
			string query = "apz_cfl_getpricelist";
			return ExecuteData(query, CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new DataSet();
		}
	}

	public DataSet GetGoodsIssueOpen()
	{
		try
		{
			string query = "apz_cfl_getgoodsisssueopen";
			return ExecuteData(query, CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new DataSet();
		}
	}

	public List<string> GetListCFLItemField()
	{
		try
		{
			return (from r in ExecuteDataTable("select \"FieldName\" from APZ_Item_ListCFLField;", CommandType.Text).AsEnumerable()
				select Function.ToString(r[0])).ToList();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return null;
	}
}
