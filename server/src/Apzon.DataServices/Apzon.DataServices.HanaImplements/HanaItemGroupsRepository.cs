using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.DataServices.Implements;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.UserFields;
using Apzon.Services;
using SAPbobsCOM;

namespace Apzon.DataServices.HanaImplements;

public class HanaItemGroupsRepository : ItemGroupsRepository
{
	public HanaItemGroupsRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IDataService dataService, IUnitOfWork datasUnitOfWork)
		: base(serviceUnitOfWork, dataService, datasUnitOfWork)
	{
	}

	protected override string ProcessDefaultBinQuery()
	{
		return "select max(ifnull(\"AbsEntry\",0)) from OIGW;";
	}

	public override string Delete(int code, string sessionId)
	{
		try
		{
			List<BodyRequestObject> list = new List<BodyRequestObject>();
			list.Add(new BodyRequestObject
			{
				ObjectType = ApzObjectTypes.oItemGroups,
				OutObjectKey = true,
				Key = code,
				Action = Enums.HttpAction.Delete
			});
			HttpResult httpResult = DataService.ProcessMutiTransaction(sessionId, list);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				Logging.Write("ERROR", "Delete item group error: " + httpResult.Message);
				return httpResult.Message;
			}
			Logging.Write("WATCH", "Delete item group Success: " + Function.ToString(code));
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public override HttpResult Save(DataSet ds, string sessionId)
	{
		try
		{
			string empty = string.Empty;
			if (ds != null && ds.Tables.Count == 2 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0 && ds.Tables[1] != null)
			{
				empty += Mapping(ds.Tables[0].Rows[0], (!Exists(Function.ToString(ds.Tables[0].Rows[0]["ItmsGrpCod"]), sessionId)) ? EnumCommandType.Add : EnumCommandType.Update, ds.Tables[1], sessionId);
				ApzObjectTypes objectType = ApzObjectTypes.oItemGroups;
				List<BodyRequestObject> list = new List<BodyRequestObject>();
				list.Add(new BodyRequestObject
				{
					ObjectType = objectType,
					OutObjectKey = true,
					Key = Function.ParseInt(ds.Tables[0].Rows[0]["ItmsGrpCod"]),
					Action = ((!Exists(Function.ToString(ds.Tables[0].Rows[0]["ItmsGrpCod"]), sessionId)) ? Enums.HttpAction.AddNew : Enums.HttpAction.Update),
					BodyObject = empty
				});
				HttpResult httpResult = DataService.ProcessMutiTransaction(sessionId, list);
				if (!string.IsNullOrEmpty(httpResult.Message))
				{
					Logging.Write("ERROR", "Save item group error: " + httpResult.Message);
					return new HttpResult(Enums.MessageCode.Error, httpResult.Message);
				}
				string text = Function.ToString(httpResult.Content);
				List<string> list2 = new List<string>();
				if (list2.Count > 0)
				{
					if (ds.Tables[1] != null)
					{
						ProcessDefaultBin(ds.Tables[1], Function.ParseInt(list2[0]));
					}
					else
					{
						ProcessDefaultBin(null, Function.ParseInt(list2[0]));
					}
					Logging.Write("WATCH", "Save item group Success: " + string.Join(",", text));
					return new HttpResult(Enums.MessageCode.Success, "", string.Join(",", text));
				}
			}
			return new HttpResult();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected override string Mapping(DataRow doc, EnumCommandType commandType, DataTable dtWhs, string sessionId)
	{
		string text = "";
		try
		{
			text += "{";
			text += Function.GetJsonString("Number", Function.ToString(doc["ItmsGrpCod"]));
			text += Function.GetJsonString("PriceDifferencesAccount", Function.ToString(doc["PriceDifAc"]));
			text += Function.GetJsonString("StockInflationAdjustAccount", Function.ToString(doc["IncreasAc"]));
			text += Function.GetJsonString("MinimumOrderQuantity", Function.ToString(doc["MinOrdrQty"]));
			if (Function.ParseLong(doc["OrdrIntrvl"]) > 0)
			{
				text += Function.GetJsonString("OrderInterval", Function.ToString(doc["OrdrIntrvl"]));
			}
			text += Function.GetJsonString("ExchangeRateDifferencesAccount", Function.ToString(doc["ExchangeAc"]));
			text += Function.GetJsonString("IncreasingAccount", Function.ToString(doc["IncresGlAc"]));
			text += Function.GetJsonString("StockInflationOffsetAccount", Function.ToString(doc["DecreasAc"]));
			text = ((!(Function.ToString(doc["PrcrmntMtd"]).ToUpper() == "M")) ? (text + Function.GetJsonString("ProcurementMethod", "bom_Buy")) : (text + Function.GetJsonString("ProcurementMethod", "bom_Make")));
			text += Function.GetJsonString("PurchaseOffsetAccount", Function.ToString(doc["PurchOfsAc"]));
			string text2 = Function.ToString(doc["InvntSys"]);
			string text3 = "";
			switch (text2)
			{
			case "A":
				text3 = "bis_MovingAverage";
				break;
			case "S":
				text3 = "bis_Standard";
				break;
			case "F":
				text3 = "bis_FIFO";
				break;
			}
			if (text3 != "")
			{
				text += Function.GetJsonString("InventorySystem", text3);
			}
			text += Function.GetJsonString("WIPMaterialVarianceAccount", Function.ToString(doc["WipVarAcct"]));
			text = ((!(Function.ToString(doc["PlaningSys"]).ToUpper() == "M")) ? (text + Function.GetJsonString("PlanningSystem", "bop_None")) : (text + Function.GetJsonString("PlanningSystem", "bop_MRP")));
			text += Function.GetJsonString("PurchaseAccount", Function.ToString(doc["PurchaseAc"]));
			text += Function.GetJsonString("ReturningAccount", Function.ToString(doc["PAReturnAc"]));
			text += Function.GetJsonString("CostInflationAccount", Function.ToString(doc["CostRvlAct"]));
			text += Function.GetJsonString("ExpensesAccount", Function.ToString(doc["ExpensesAc"]));
			text += Function.GetJsonString("RevenuesAccount", Function.ToString(doc["RevenuesAc"]));
			text += Function.GetJsonString("TransfersAccount", Function.ToString(doc["TransferAc"]));
			text += Function.GetJsonString("LeadTime", Function.ParseInt(doc["LeadTime"]));
			text += Function.GetJsonString("OrderMultiple", Function.ParseDecimal(doc["OrdrMulti"]));
			text += Function.GetJsonString("CostInflationOffsetAccount", Function.ToString(doc["CstOffsAct"]));
			text += Function.GetJsonString("InventoryAccount", Function.ToString(doc["BalInvntAc"]));
			text += Function.GetJsonString("DecreaseGLAccount", Function.ToString(doc["DecresGlAc"]));
			text += Function.GetJsonString("GoodsClearingAccount", Function.ToString(doc["BalanceAcc"]));
			text += Function.GetJsonString("ForeignRevenuesAccount", Function.ToString(doc["FrRevenuAc"]));
			text = ((!(Function.ToString(doc["Alert"]) == "Y")) ? (text + Function.GetJsonString("Alert", "tNO")) : (text + Function.GetJsonString("Alert", "tYES")));
			text += Function.GetJsonString("WIPMaterialAccount", Function.ToString(doc["WipAcct"]));
			text += Function.GetJsonString("ShippedGoodsAccount", Function.ToString(doc["ShpdGdsAct"]));
			text += Function.GetJsonString("ExemptRevenuesAccount", Function.ToString(doc["ExmptIncom"]));
			text += Function.GetJsonString("DecreasingAccount", Function.ToString(doc["DecreasAc"]));
			text += Function.GetJsonString("VATInRevenueAccount", Function.ToString(doc["VatRevAct"]));
			text += Function.GetJsonString("VarianceAccount", Function.ToString(doc["VarianceAc"]));
			text += Function.GetJsonString("EUExpensesAccount", Function.ToString(doc["ExpensesAc"]));
			text += Function.GetJsonString("ForeignExpensesAccount", Function.ToString(doc["FrExpensAc"]));
			if (Function.ParseLong(doc["CycleCode"]) > 0)
			{
				text += Function.GetJsonString("CycleCode", Function.ParseLong(doc["CycleCode"]));
			}
			text += Function.GetJsonString("CostAccount", Function.ToString(doc["SaleCostAc"]));
			text += Function.GetJsonString("EURevenuesAccount", Function.ToString(doc["EURevenuAc"]));
			text += Function.GetJsonString("PAReturnAccount", Function.ToString(doc["PAReturnAc"]));
			text += Function.GetJsonString("GroupName", Function.ToString(doc["ItmsGrpNam"]));
			text += Function.GetJsonString("ExpenseClearingAct", Function.ToString(doc["ExpClrAct"]));
			text += Function.GetJsonString("PurchaseCreditAcc", Function.ToString(doc["APCMAct"]));
			text += Function.GetJsonString("EUPurchaseCreditAcc", Function.ToString(doc["APCMEUAct"]));
			text += Function.GetJsonString("ForeignPurchaseCreditAcc", Function.ToString(doc["APCMFrnAct"]));
			text += Function.GetJsonString("SalesCreditAcc", Function.ToString(doc["ARCMAct"]));
			text += Function.GetJsonString("SalesCreditEUAcc", Function.ToString(doc["ARCMEUAct"]));
			text += Function.GetJsonString("ExemptedCredits", Function.ToString(doc["ARCMExpAct"]));
			text += Function.GetJsonString("SalesCreditForeignAcc", Function.ToString(doc["ARCMFrnAct"]));
			text += Function.GetJsonString("ExpenseOffsetAccount", Function.ToString(doc["ExpOfstAct"]));
			text += Function.GetJsonString("NegativeInventoryAdjustmentAccount", Function.ToString(doc["NegStckAct"]));
			text += Function.GetJsonString("WHIncomingCenvatAccount", Function.ToString(doc["WhICenAct"]));
			text += Function.GetJsonString("WHOutgoingCenvatAccount", Function.ToString(doc["WhOCenAct"]));
			text += Function.GetJsonString("StockInTransitAccount", Function.ToString(doc["StkInTnAct"]));
			text += Function.GetJsonString("WipOffsetProfitAndLossAccount", Function.ToString(doc["WipOffset"]));
			text += Function.GetJsonString("InventoryOffsetProfitAndLossAccount", Function.ToString(doc["StockOffst"]));
			text += Function.GetJsonString("ToleranceDays", Function.ParseInt(doc["ToleranDay"]));
			text += Function.GetJsonString("DefaultUoMGroup", Function.ParseInt(doc["UgpEntry"]));
			text += Function.GetJsonString("DefaultInventoryUoM", Function.ParseInt(doc["IUomEntry"]));
			text += Function.GetJsonString("PurchaseBalanceAccount", Function.ToString(doc["PurBalAct"]));
			IEnumerable<string> enumerable = from t in (from DataColumn c in doc.Table.Columns
					select c.ColumnName).ToList()
				where t.StartsWith("U_")
				select t;
			if (enumerable != null && enumerable.ToList() != null && enumerable.ToList().Count > 0)
			{
				List<UserFieldsImpl> list = new List<UserFieldsImpl>();
				for (int num = 0; num < enumerable.ToList().Count; num++)
				{
					UserFieldsImpl userFieldsImpl = new UserFieldsImpl();
					if (doc.Table.Columns[enumerable.ToList()[num]].DataType == typeof(DateTime))
					{
						userFieldsImpl.Value = Function.ToString(Function.ParseDateTime(doc[enumerable.ToList()[num]]));
						userFieldsImpl.Type = BoFieldTypes.db_Date;
					}
					else if (doc.Table.Columns[enumerable.ToList()[num]].DataType == typeof(double))
					{
						userFieldsImpl.Value = Function.ToString(Function.ParseDecimal(doc[enumerable.ToList()[num]]));
						userFieldsImpl.Type = BoFieldTypes.db_Numeric;
					}
					else
					{
						userFieldsImpl.Value = Function.ToString(Function.ToString(doc[enumerable.ToList()[num]]));
						userFieldsImpl.Type = BoFieldTypes.db_Alpha;
					}
					userFieldsImpl.Name = enumerable.ToList()[num];
					list.Add(userFieldsImpl);
				}
				text += DataService.MappingUdf(list);
			}
			text += "}";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return text;
	}

	protected override bool Exists(string p, string sessionId)
	{
		try
		{
			if (string.IsNullOrWhiteSpace(p))
			{
				return false;
			}
			return !string.IsNullOrEmpty(ExecuteScarlar($"select 1 from OITB where \"ItmsGrpCod\" = docEntry;", CommandType.Text, new SqlParameter("@docEntry", p)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return false;
	}
}
