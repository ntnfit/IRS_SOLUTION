using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.DataServices.Implements;
using Apzon.Entities.Models.Common;
using Apzon.Services;

namespace Apzon.DataServices.HanaImplements;

public class HanaOpportunityReopository : OpportunityRepository
{
	public HanaOpportunityReopository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, unitOfWork, dataService)
	{
		_serviceUnitOfWork = serviceUnitOfWork;
	}

	protected override string Add(DataSet ds, string sessionId)
	{
		try
		{
			string sesB1Ws = sessionId.Split(',')[0];
			string sessionId2 = sessionId.Split(',')[1];
			DataTable dataTable = ds.Tables[5];
			string text = string.Empty;
			int refItemDetailId = 0;
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				text = AddItemDetail(dataTable, sessionId2, ref refItemDetailId);
			}
			if (!string.IsNullOrEmpty(text))
			{
				return text;
			}
			string text2 = string.Empty;
			long forecastId = 0L;
			bool flag = CheckConfigForecast();
			if ((dataTable != null && dataTable.Rows.Count > 0) & flag)
			{
				text2 = AddForecast(ds, sesB1Ws, refItemDetailId, ref forecastId);
			}
			if (!string.IsNullOrEmpty(text2))
			{
				RemoveItemDetail(refItemDetailId, sessionId2);
				return text2;
			}
			string xmlObject = Mapping(ds, isUpdate: false, refItemDetailId, forecastId);
			HttpResult httpResult = DataService.AddObject(ApzObjectTypes.oSalesOpportunities, sessionId2, xmlObject);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				RemoveItemDetail(refItemDetailId, sessionId2);
				RemoveForecast(forecastId, sesB1Ws);
				return httpResult.Message;
			}
			int oprId = Function.ParseInt(httpResult.Content);
			string text3 = DeleteLinkedAct(oprId);
			if (ds.Tables[6].Rows.Count > 0)
			{
				foreach (DataRow row in ds.Tables[6].Rows)
				{
					text3 = UpdateLinkedAct(row);
					if (!string.IsNullOrEmpty(text3))
					{
						Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), text3);
					}
				}
			}
			if (ds.Tables[8] != null && ds.Tables[8].Rows.Count > 0)
			{
				text3 = DeleteInterestRange(ds.Tables[8].Rows.Count, oprId);
				if (!string.IsNullOrEmpty(text3))
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), text3);
				}
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return ex.Message;
		}
	}

	protected override string AddItemDetail(DataTable dt, string sessionId, ref int refItemDetailId)
	{
		try
		{
			refItemDetailId = 0;
			string jsonObject = MappingItemDetail(dt, sessionId, "Add");
			HttpResult httpResult = DataService.ProcessJson(ApzObjectTypes.oNone, sessionId, jsonObject, Enums.HttpAction.AddNew, null, "OPR6");
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				return "Save Item Detail ERROR: " + httpResult.Message;
			}
			if (!string.IsNullOrEmpty(Function.ToString(httpResult.Content)))
			{
				refItemDetailId = Function.ParseInt(httpResult.Content);
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return ex.Message;
		}
	}

	protected override string RemoveItemDetail(int itemDetailEntry, string sessionId)
	{
		try
		{
			HttpResult httpResult = DataService.ProcessJson(ApzObjectTypes.oNone, sessionId, "", Enums.HttpAction.Delete, itemDetailEntry, "OPR6");
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				return httpResult.Message;
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return ex.Message;
		}
	}

	protected override string MappingItemDetail(DataTable dt, string sessionId, string command)
	{
		if (dt.Rows.Count == 0)
		{
			return "";
		}
		string text = "{\r\n";
		if (command.ToUpper().Equals("UPDATE") && Function.ParseInt(dt.Rows[0]["DocEntry"]) > 0)
		{
			text += Function.GetJsonString("DocEntry", Function.ToString(dt.Rows[0]["DocEntry"]));
			text += Function.GetJsonString("DocNum", Function.ToString(dt.Rows[0]["DocEntry"]));
			text += Function.GetJsonString("Series", -1);
		}
		text += "\"OPR62Collection\":[";
		List<DataColumn> list = (from DataColumn c in dt.Columns
			select (c) into t
			where t.ColumnName.StartsWith("U_")
			select t).ToList();
		foreach (DataRow row in dt.Rows)
		{
			text += "{";
			if (!string.IsNullOrEmpty(Function.ToString(row["ItemCode"])))
			{
				text += Function.GetJsonString("U_ItemCode", Function.ToString(row["ItemCode"]));
			}
			if (!string.IsNullOrEmpty(Function.ToString(row["ItemName"])))
			{
				text += Function.GetJsonString("U_ItemName", Function.ToString(row["ItemName"]));
			}
			if (!string.IsNullOrEmpty(Function.ToString(row["FrgnName"])))
			{
				text += Function.GetJsonString("U_FrgnName", Function.ToString(row["FrgnName"]));
			}
			if (!Function.ParseDouble(row["Quantity"]).Equals(0.0))
			{
				text += Function.GetJsonString("U_Quantity", Function.ParseDouble(row["Quantity"]));
			}
			if (!Function.ParseDouble(row["IPercent"]).Equals(0.0))
			{
				text += Function.GetJsonString("U_IPercent", Function.ParseDouble(row["IPercent"]));
			}
			if (!Function.ParseDouble(row["Price"]).Equals(0.0))
			{
				text += Function.GetJsonString("U_Price", Function.ParseDouble(row["Price"]));
			}
			if (!Function.ParseDouble(row["PotentAmount"]).Equals(0.0))
			{
				text += Function.GetJsonString("U_PotentAmount", Function.ParseDouble(row["PotentAmount"]));
			}
			if (Function.ParseDateTime(row["DeliveryDate"]).HasValue && Function.ParseDateTime(row["DeliveryDate"]) != new DateTime(1900, 1, 1))
			{
				text += Function.GetJsonString("U_DeliveryDate", Convert.ToDateTime(row["DeliveryDate"]).ToString("yyyyMMdd"));
			}
			if (!string.IsNullOrEmpty(Function.ToString(row["AttFile"])))
			{
				text += Function.GetJsonString("U_AttFile", Function.ToString(row["AttFile"]));
			}
			string itemDetailUdfColumn = _serviceUnitOfWork.Opportunity.GetItemDetailUdfColumn();
			DataTable source = ExecuteDataTable(itemDetailUdfColumn, CommandType.Text, new SqlParameter("@TableName", "@OPR62"));
			foreach (DataColumn col in list)
			{
				DataRow dataRow2 = source.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["name"]).ToUpper().Equals(col.ColumnName.ToUpper()));
				if (dataRow2 == null)
				{
					continue;
				}
				switch (Function.ToString(dataRow2["datatype"]).ToLower())
				{
				case "numeric":
					text += Function.GetJsonString(col.ColumnName, Function.ParseDouble(row[col]));
					break;
				case "datetime":
					if (Function.ParseDateTime(row[col]).HasValue)
					{
						text += Function.GetJsonString(col.ColumnName, Convert.ToDateTime(row[col]).ToString("yyyyMMdd"));
					}
					break;
				case "int":
				case "smallint":
				case "tinyint":
				case "bigint":
					text += Function.GetJsonString(col.ColumnName, Function.ParseInt(row[col]));
					break;
				case "bit":
					text += Function.GetJsonString(col.ColumnName, Function.ParseBool(row[col]));
					break;
				default:
					text += Function.GetJsonString(col.ColumnName, Function.ToString(row[col]));
					break;
				}
			}
			text += " },";
		}
		return text + "],\r\n                                            }";
	}

	protected override string Mapping(DataSet ds, bool isUpdate = false, int itemDetailEntry = 0, long forecastId = 0L)
	{
		try
		{
			DataRow dataRow = ds.Tables[0].Rows[0];
			string text = "{\r\n";
			if (!string.IsNullOrEmpty(Function.ToString(dataRow["CardCode"])))
			{
				text += Function.GetJsonString("CardCode", Function.ToString(dataRow["CardCode"]));
			}
			if (Function.ParseInt(dataRow["SlpCode"]) != 0)
			{
				text += Function.GetJsonString("SalesPerson", Function.ParseInt(dataRow["SlpCode"]));
			}
			if (Function.ParseInt(dataRow["CprCode"]) != 0)
			{
				text += Function.GetJsonString("ContactPerson", Function.ParseInt(dataRow["CprCode"]));
			}
			if (Function.ParseInt(dataRow["Source"]) != 0)
			{
				text += Function.GetJsonString("Source", Function.ParseInt(dataRow["Source"]));
			}
			if (Function.ParseInt(dataRow["IntCat1"]) != 0)
			{
				text += Function.GetJsonString("InterestField1", Function.ParseInt(dataRow["IntCat1"]));
			}
			if (Function.ParseInt(dataRow["IntCat2"]) != 0)
			{
				text += Function.GetJsonString("InterestField2", Function.ParseInt(dataRow["IntCat2"]));
			}
			if (Function.ParseInt(dataRow["IntCat3"]) != 0)
			{
				text += Function.GetJsonString("InterestField3", Function.ParseInt(dataRow["IntCat3"]));
			}
			if (Function.ParseInt(dataRow["IntRate"]) != 0)
			{
				text += Function.GetJsonString("InterestLevel", Function.ParseInt(dataRow["IntRate"]));
			}
			if (Function.ParseDateTime(dataRow["OpenDate"]).HasValue)
			{
				text += Function.GetJsonString("StartDate", Convert.ToDateTime(dataRow["OpenDate"]).ToString("yyyyMMdd"));
			}
			if (Function.ParseDateTime(dataRow["PredDate"]).HasValue)
			{
				text += Function.GetJsonString("PredictedClosingDate", Convert.ToDateTime(dataRow["PredDate"]).ToString("yyyyMMdd"));
			}
			if (!Function.ParseDouble(dataRow["PrcnProf"]).Equals(0.0))
			{
				text += Function.GetJsonString("GrossProfit", Function.ParseDouble(dataRow["PrcnProf"]));
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataRow["Memo"])))
			{
				text += Function.GetJsonString("Remarks", Function.ToString(dataRow["Memo"]));
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataRow["Status"])))
			{
				text += Function.GetJsonString("Status", Function.ToString(dataRow["Status"]));
			}
			if (!Function.ParseInt(dataRow["Reason"]).Equals(0))
			{
				text += Function.GetJsonString("ReasonForClosing>", Function.ToString(dataRow["Reason"]));
			}
			if (!Function.ParseDouble(dataRow["MaxSumLoc"]).Equals(0.0))
			{
				text += Function.GetJsonString("TotalAmountLocal", Function.ParseDouble(dataRow["MaxSumLoc"]));
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataRow["Name"])))
			{
				text += Function.GetJsonString("OpportunityName", Function.ToString(dataRow["Name"]));
			}
			if (!Function.ParseInt(dataRow["Industry"]).Equals(0))
			{
				text += Function.GetJsonString("Industry", Function.ParseInt(dataRow["Industry"]));
			}
			if (Function.ParseInt(dataRow["Owner"]) > 0)
			{
				text += Function.GetJsonString("DataOwnershipfield", Function.ParseInt(dataRow["Owner"]));
				if (dataRow.IsExistsColumn("U_DataOwner"))
				{
					text += Function.GetJsonString("U_DataOwner", Function.ParseInt(dataRow["Owner"]));
				}
			}
			else
			{
				text += "DataOwnershipfield: null,";
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataRow["StatusRem"])))
			{
				text += Function.GetJsonString("StatusRemarks", Function.ToString(dataRow["StatusRem"]));
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataRow["PrjCode"])))
			{
				text += Function.GetJsonString("ProjectCode", Function.ToString(dataRow["PrjCode"]));
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataRow["ChnCrdName"])))
			{
				text += Function.GetJsonString("BPChanelName", Function.ToString(dataRow["ChnCrdName"]));
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataRow["CardName"])))
			{
				text += Function.GetJsonString("CustomerName", Function.ToString(dataRow["CardName"]));
			}
			if (!Function.ParseInt(dataRow["Territory"]).Equals(0))
			{
				text += Function.GetJsonString("Territory", Function.ParseInt(dataRow["Territory"]));
			}
			if (Function.ParseDateTime(dataRow["CloseDate"]).HasValue)
			{
				text += Function.GetJsonString("ClosingDate", Convert.ToDateTime(dataRow["CloseDate"]).ToString("yyyyMMdd"));
			}
			if (!Function.ParseInt(dataRow["ChnCrdCon"]).Equals(0))
			{
				text += Function.GetJsonString("BPChannelContact", Function.ParseInt(dataRow["ChnCrdCon"]));
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataRow["ChnCrdCode"])))
			{
				text += Function.GetJsonString("BPChanelCode", Function.ToString(dataRow["ChnCrdCode"]));
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataRow["OpprType"])))
			{
				string value = (Function.ToString(dataRow["OpprType"]).Equals("R") ? "boOpSales" : "boOpPurchasing");
				text += Function.GetJsonString("OpportunityType", value);
			}
			if (dataRow.IsExistsColumn("U_Itemdetail"))
			{
				dataRow["U_Itemdetail"] = itemDetailEntry;
			}
			if (dataRow.IsExistsColumn("U_ForecastId"))
			{
				dataRow["U_ForecastId"] = forecastId;
			}
			text += DataService.MappingUdf(dataRow, (!isUpdate) ? EnumCommandType.Add : EnumCommandType.Update);
			if (ds.Tables[1].Rows.Count > 0)
			{
				text += "\"SalesOpportunitiesLines\":[";
				for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
				{
					DataRow dataRow2 = ds.Tables[1].Rows[i];
					if (ExistsStage(i, Function.ParseInt(dataRow["OpprId"])) && i != ds.Tables[1].Rows.Count - 1)
					{
						text += string.Format("{{{0}}},", Function.GetJsonString("LineNum", i));
						continue;
					}
					text += "{\r\n";
					text += Function.GetJsonString("LineNum", i);
					if (!Function.ParseInt(dataRow2["SlpCode"]).Equals(0))
					{
						text += Function.GetJsonString("SalesPerson", Function.ParseInt(dataRow2["SlpCode"]));
					}
					if (Function.ParseDateTime(dataRow2["OpenDate"]).HasValue)
					{
						text += Function.GetJsonString("StartDate", Convert.ToDateTime(dataRow2["OpenDate"]).ToString("yyyyMMdd"));
					}
					if (Function.ParseDateTime(dataRow2["CloseDate"]).HasValue)
					{
						text += Function.GetJsonString("ClosingDate", Convert.ToDateTime(dataRow2["CloseDate"]).ToString("yyyyMMdd"));
					}
					if (Function.ParseInt(dataRow2["Step_Id"]) > 0)
					{
						text += Function.GetJsonString("StageKey", Function.ParseInt(dataRow2["Step_Id"]));
					}
					if (!Function.ParseDouble(dataRow2["ClosePrcnt"]).Equals(0.0))
					{
						text += Function.GetJsonString("PercentageRate", Function.ParseInt(dataRow2["ClosePrcnt"]));
					}
					text += Function.GetJsonString("MaxLocalTotal", Function.ParseDouble(dataRow2["MaxSumLoc"]));
					if (!string.IsNullOrEmpty(Function.ToString(dataRow2["Memo"])))
					{
						text += Function.GetJsonString("Remarks", Function.ToString(dataRow2["Memo"]));
					}
					if (!string.IsNullOrEmpty(Function.ToString(dataRow2["ObjType"])))
					{
						string table = "";
						string text2 = Function.ToString(dataRow2["ObjType"]);
						string value2 = string.Empty;
						switch (text2)
						{
						case "13":
							value2 = "bodt_Invoice";
							table = "OINV";
							break;
						case "14":
							value2 = "bodt_CreditNote";
							table = "ORIN";
							break;
						case "15":
							value2 = "bodt_DeliveryNote";
							table = "ODLN";
							break;
						case "16":
							value2 = "bodt_Return";
							table = "ORDN";
							break;
						case "17":
							value2 = "bodt_Order";
							table = "ORDR";
							break;
						case "18":
							value2 = "bodt_PurchaseInvoice";
							table = "OPCH";
							break;
						case "19":
							value2 = "bodt_PurchaseCreditNote";
							table = "ORPC";
							break;
						case "20":
							value2 = "bodt_PurchaseDeliveryNote";
							table = "OPDN";
							break;
						case "21":
							value2 = "bodt_PurchaseReturn";
							table = "ORPD";
							break;
						case "22":
							value2 = "bodt_PurchaseOrder";
							table = "OPOR";
							break;
						case "23":
							value2 = "bodt_Quotation";
							table = "OQUT";
							break;
						case "163":
							value2 = "bodt_CorrectionAPInvoice";
							table = "OCPI";
							break;
						case "165":
							value2 = "bodt_CorrectionARInvoice";
							table = "OCSI";
							break;
						case "540000006":
							value2 = "bodt_PurchaseQutation";
							table = "OPQT";
							break;
						}
						if (!string.IsNullOrEmpty(value2))
						{
							text += Function.GetJsonString("DocumentType", value2);
						}
						string docEntry = GetDocEntry(Function.ParseInt(dataRow2["DocNumber"]), table);
						if (docEntry != "" && docEntry != "0")
						{
							text += Function.GetJsonString("DocumentNumber", docEntry);
						}
					}
					if (!string.IsNullOrEmpty(Function.ToString(dataRow2["DocChkbox"])))
					{
						string text3 = Function.ToString(dataRow2["DocChkbox"]);
						if (!(text3 == "N"))
						{
							if (text3 == "Y")
							{
								text += Function.GetJsonString("DocumentCheckbox", "Y");
							}
						}
						else
						{
							text += Function.GetJsonString("DocumentCheckbox", "N");
						}
					}
					if (!Function.ParseInt(dataRow2["CntctCode"]).Equals(0))
					{
						text += Function.GetJsonString("ContactPerson", Function.ParseInt(dataRow2["CntctCode"]));
					}
					if (!string.IsNullOrEmpty(Function.ToString(dataRow2["ChnCrdName"])))
					{
						text += Function.GetJsonString("BPChanelName", Function.ToString(dataRow2["ChnCrdName"]));
					}
					if (!string.IsNullOrEmpty(Function.ToString(dataRow2["ChnCrdCode"])))
					{
						text += Function.GetJsonString("BPChanelCode", Function.ToString(dataRow2["ChnCrdCode"]));
					}
					if (!Function.ParseInt(dataRow2["Owner"]).Equals(0) && !isUpdate)
					{
						text += Function.GetJsonString("DataOwnershipfield", Function.ParseInt(dataRow2["Owner"]));
					}
					if (!Function.ParseInt(dataRow2["ChnCrdCon"]).Equals(0))
					{
						text += Function.GetJsonString("BPChannelContact", Function.ParseInt(dataRow2["ChnCrdCon"]));
					}
					text += DataService.MappingUdf(dataRow2, (!isUpdate) ? EnumCommandType.Add : EnumCommandType.Update);
					text += "},";
				}
				text += "],\r\n";
			}
			if (ds.Tables[2].Rows.Count > 0)
			{
				text += "\"SalesOpportunitiesCompetition\":[";
				for (int j = 0; j < ds.Tables[2].Rows.Count; j++)
				{
					DataRow dataRow3 = ds.Tables[2].Rows[j];
					text += "{\r\n";
					if (!Function.ParseInt(dataRow3["CompetId"]).Equals(0))
					{
						text += Function.GetJsonString("Competition", Function.ParseInt(dataRow3["CompetId"]));
					}
					if (!string.IsNullOrEmpty(Function.ToString(dataRow3["Memo"])))
					{
						text += Function.GetJsonString("Details", Function.ToString(dataRow3["Memo"]));
					}
					if (!string.IsNullOrEmpty(Function.ToString(dataRow3["Won"])))
					{
						text += Function.GetJsonString("WonOrLost", Function.ToString(dataRow3["Won"]));
					}
					text += DataService.MappingUdf(dataRow3, (!isUpdate) ? EnumCommandType.Add : EnumCommandType.Update);
					text += "},\r\n";
				}
				text += "],\r\n";
			}
			if (ds.Tables[3].Rows.Count > 0)
			{
				text += "\"SalesOpportunitiesPartners\":[";
				for (int k = 0; k < ds.Tables[3].Rows.Count; k++)
				{
					DataRow dataRow4 = ds.Tables[3].Rows[k];
					text += "{";
					if (!Function.ParseInt(dataRow4["ParterId"]).Equals(0))
					{
						text += Function.GetJsonString("Partners", Function.ParseInt(dataRow4["ParterId"]));
					}
					if (!string.IsNullOrEmpty(Function.ToString(dataRow4["Memo"])))
					{
						text += Function.GetJsonString("Details", Function.ToString(dataRow4["Memo"]));
					}
					if (!Function.ParseInt(dataRow4["OrlCode"]).Equals(0))
					{
						text += Function.GetJsonString("RelationshipCode", Function.ParseInt(dataRow4["OrlCode"]));
					}
					text += DataService.MappingUdf(dataRow4, (!isUpdate) ? EnumCommandType.Add : EnumCommandType.Update);
					text += "},\r\n";
				}
				text += "],\r\n";
			}
			if (ds.Tables[8].Rows.Count > 0)
			{
				text += "\"SalesOpportunitiesInterests\":[";
				for (int l = 0; l < ds.Tables[8].Rows.Count; l++)
				{
					DataRow dataRow5 = ds.Tables[8].Rows[l];
					text += "{\r\n";
					if (!Function.ParseInt(dataRow5["IntId"]).Equals(0))
					{
						text += Function.GetJsonString("InterestId", Function.ParseInt(dataRow5["IntId"]));
					}
					text = ((!(Function.ToString(dataRow5["Prmry"]).ToUpper() == "Y")) ? (text + Function.GetJsonString("PrimaryInterest", "tNO")) : (text + Function.GetJsonString("PrimaryInterest", "tYES")));
					text += "},\r\n";
				}
				text += "]";
			}
			return text + "}";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return string.Empty;
		}
	}

	protected override string Update(DataSet ds, string sessionId)
	{
		try
		{
			string text = sessionId.Split(',')[1];
			object obj = ds.Tables[0].Rows[0]["U_ItemDetail"];
			int refItemDetailId = 0;
			if (obj == null || obj == DBNull.Value || string.IsNullOrWhiteSpace(obj.ToString()) || Function.ParseInt(obj) == 0)
			{
				AddItemDetail(ds.Tables[5], text, ref refItemDetailId);
			}
			else
			{
				refItemDetailId = Function.ParseInt(obj);
				MappingItemDetail(ds.Tables[5], text, "Update");
			}
			string text2 = string.Join(",", from t in ds.Tables[1].AsEnumerable()
				select Function.ParseInt(t["Line"]));
			if (string.IsNullOrEmpty(text2))
			{
				text2 = "-1";
			}
			DataTable dataTable = ExecuteDataTable("SELECT * FROM OPR1 WHERE \"OpprId\"=@DocEntry AND \"Line\" NOT IN (" + text2 + ");", CommandType.Text, new SqlParameter("@DocEntry", Function.ParseInt(ds.Tables[0].Rows[0]["OpprId"])));
			if (dataTable.IsNotNull())
			{
				DataTable dataTable2 = ds.Tables[1];
				for (int num = 0; num < dataTable.Rows.Count; num++)
				{
					dataTable2.ImportRow(dataTable.Rows[num]);
				}
				dataTable2 = (from t in dataTable2.AsEnumerable()
					orderby Function.ParseInt(t["Line"])
					select t).CopyToDataTable();
				dataTable2.AcceptChanges();
				List<DataTable> list = new List<DataTable>();
				list.Add(ds.Tables[0].Copy());
				list.Add(dataTable2);
				for (int num2 = 2; num2 < ds.Tables.Count; num2++)
				{
					list.Add(ds.Tables[num2].Copy());
				}
				ds = new DataSet();
				ds.Tables.AddRange(list.ToArray());
			}
			string bodyObject = Mapping(ds, isUpdate: true, refItemDetailId, 0L);
			int num3 = Function.ParseInt(ds.Tables[0].Rows[0]["OpprId"]);
			List<BodyRequestObject> list2 = new List<BodyRequestObject>();
			list2.Add(new BodyRequestObject
			{
				ObjectType = ApzObjectTypes.oSalesOpportunities,
				Action = Enums.HttpAction.Update,
				BodyObject = bodyObject,
				Key = num3
			});
			if (CheckConfigForecast())
			{
				string bodyObject2 = MappingForecast(ds, text, "UpdateObject");
				list2.Add(new BodyRequestObject
				{
					ObjectType = ApzObjectTypes.oSalesForecast,
					Action = Enums.HttpAction.Update,
					BodyObject = bodyObject2,
					Key = Function.ParseInt(ds.Tables[0].Rows[0]["U_ForecastId"])
				});
			}
			HttpResult httpResult = DataService.ProcessMutiTransaction(text, list2);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				return httpResult.Message;
			}
			string text3 = DeleteLinkedAct(num3);
			if (ds.Tables[6].Rows.Count > 0)
			{
				foreach (DataRow row in ds.Tables[6].Rows)
				{
					text3 = UpdateLinkedAct(row);
					if (!string.IsNullOrEmpty(text3))
					{
						Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), text3);
					}
				}
			}
			text3 = DeleteInterestRange(ds.Tables[8].Rows.Count, num3);
			if (!string.IsNullOrEmpty(text3))
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), text3);
			}
			text3 = ServiceUnitOfWork.Opportunity.ProcessDataReasons(ds.Tables[4], num3);
			if (!string.IsNullOrEmpty(text3))
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), text3);
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return ex.Message;
		}
	}

	protected override string MappingForecast(DataSet ds, string sessionId, string command)
	{
		try
		{
			DataTable dataTable = ds.Tables[5];
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				string value = "FCS-OOP-" + Function.ToString(dataTable.Rows[0]["DocEntry"]);
				string value2 = string.Empty;
				if (!string.IsNullOrEmpty(Function.ToString(ds.Tables[0].Rows[0]["Name"])))
				{
					value2 = "FCS-OOP-" + Function.ToString(ds.Tables[0].Rows[0]["Name"]);
				}
				DateTime? dateTime = Function.ParseDateTime(dataTable.Compute("min(DeliveryDate)", ""));
				DateTime? dateTime2 = Function.ParseDateTime(dataTable.Compute("max(DeliveryDate)", ""));
				string text = "{\r\n";
				if (dateTime.HasValue)
				{
					text += Function.GetJsonString("ForecastStartDate", Function.ParseDateTimes(dateTime).ToString(")yyyyMMdd"));
				}
				if (dateTime2.HasValue)
				{
					text += Function.GetJsonString("ForecastEndDate", Function.ParseDateTimes(dateTime2).ToString("yyyyMMdd"));
				}
				if (!string.IsNullOrEmpty(value))
				{
					text += Function.GetJsonString("ForecastCode", value);
				}
				if (!string.IsNullOrEmpty(value2))
				{
					text += Function.GetJsonString("ForecastName", value2);
				}
				text += "\"SalesForecastLines\": [";
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					text += "{";
					if (!Function.ParseDouble(dataTable.Rows[i]["Quantity"]).Equals(0.0))
					{
						text += Function.GetJsonString("Quantity", Function.ParseDouble(dataTable.Rows[i]["Quantity"]));
					}
					if (Function.ParseDateTime(dataTable.Rows[i]["DeliveryDate"]).HasValue)
					{
						text += Function.GetJsonString("ForecastedDay", Function.ParseDateTimes(dataTable.Rows[i]["DeliveryDate"]).ToString("yyyyMMdd"));
					}
					if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[i]["ItemCode"])))
					{
						text += Function.GetJsonString("ItemNo", Function.ToString(dataTable.Rows[i]["ItemCode"]));
					}
					text += "},";
				}
				return text + "]\r\n}";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return string.Empty;
		}
		return string.Empty;
	}

	public override string DeleteDocs(string ids, string sessionId)
	{
		try
		{
			string seddionId = sessionId.Split(',')[1];
			string[] array = ids.Split(',');
			List<BodyRequestObject> list = new List<BodyRequestObject>();
			if (array.Length != 0)
			{
				string[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					int num = Function.ParseInt(array2[i]);
					if (num != 0)
					{
						list.Add(new BodyRequestObject
						{
							ObjectType = ApzObjectTypes.oSalesOpportunities,
							Action = Enums.HttpAction.Delete,
							Key = num
						});
					}
				}
				HttpResult httpResult = DataService.ProcessMutiTransaction(seddionId, list);
				if (!string.IsNullOrEmpty(httpResult.Message))
				{
					return httpResult.Message;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return string.Empty;
		}
		return string.Empty;
	}
}
