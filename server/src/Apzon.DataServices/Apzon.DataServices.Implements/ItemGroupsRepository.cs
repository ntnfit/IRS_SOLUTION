using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.DataServices.Interfaces;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.UserFields;
using Apzon.Services;
using SAPbobsCOM;

namespace Apzon.DataServices.Implements;

public class ItemGroupsRepository : Repository<DataTable>, Apzon.DataServices.Interfaces.IItemGroups, IDisposable
{
	protected readonly IUnitOfWork _unitOfWork;

	public ItemGroupsRepository(Apzon.Services.IUnitOfWork unitOfWork, IDataService dataService, IUnitOfWork dataUnitOfWork)
		: base(unitOfWork, dataService)
	{
		_unitOfWork = dataUnitOfWork;
	}

	public virtual string Delete(int code, string sessionId)
	{
		try
		{
			string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?><env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <env:Header xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <SessionID>" + sessionId + "</SessionID>\r\n                        </env:Header><env:Body xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\"><dis:RemoveObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">\r\n                        <BOM>\r\n                          <BO>\r\n                            <AdmInfo>\r\n                                <Object>oItemGroups</Object>\r\n                            </AdmInfo>\r\n                            <QueryParams><Number>" + code + "</Number></QueryParams>\r\n                            </BO>\r\n                        </BOM>\r\n                        </dis:RemoveObject>\r\n                    </env:Body></env:Envelopes>";
			XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					Logging.Write("ERROR", "Delete item group error: " + xElement2.Value);
					return xElement2.Value;
				}
			}
			else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
			{
				List<XElement> list = (from t in xElement.Descendants()
					where t.Name.LocalName.Equals("RetKey")
					select t).ToList();
				if (list.Count > 0)
				{
					List<string> list2 = new List<string>();
					list2.AddRange(list.Select((XElement t) => Function.ToString(t.Value)));
					if (list2.Count > 0)
					{
						Logging.Write("WATCH", "Delete item group Success: " + string.Join(",", list2.ToArray()));
						return "";
					}
				}
			}
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public virtual HttpResult Save(DataSet ds, string sessionId)
	{
		try
		{
			string empty = string.Empty;
			if (ds != null && ds.Tables.Count == 2 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0 && ds.Tables[1] != null)
			{
				empty += Mapping(ds.Tables[0].Rows[0], (!Exists(Function.ToString(ds.Tables[0].Rows[0]["ItmsGrpCod"]), sessionId)) ? EnumCommandType.Add : EnumCommandType.Update, ds.Tables[1], sessionId);
				string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?><env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <env:Header xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <SessionID>" + sessionId + "</SessionID>\r\n                        </env:Header><env:Body xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">" + empty + "</env:Body></env:Envelopes>";
				XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
				XElement xElement = XElement.Parse(xmlElement.OuterXml);
				if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
				{
					XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
					if (xElement2 != null)
					{
						Logging.Write("ERROR", "Save item group error: " + xElement2.Value);
						return new HttpResult(Enums.MessageCode.Error, xElement2.Value);
					}
				}
				else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
				{
					List<XElement> list = (from t in xElement.Descendants()
						where t.Name.LocalName.Equals("RetKey")
						select t).ToList();
					if (list.Count > 0)
					{
						List<string> list2 = new List<string>();
						list2.AddRange(list.Select((XElement t) => Function.ToString(t.Value)));
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
							Logging.Write("WATCH", "Save item group Success: " + string.Join(",", list2.ToArray()));
							return new HttpResult(Enums.MessageCode.Success, "", string.Join(",", list2.ToArray()));
						}
					}
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

	protected virtual string Mapping(DataRow doc, EnumCommandType commandType, DataTable dtWhs, string sessionId)
	{
		string text = "";
		text = ((commandType != EnumCommandType.Update) ? (text + "<dis:AddObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">") : (text + "<dis:UpdateObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">"));
		text += "<BOM>\r\n                          <BO>\r\n                          <AdmInfo>\r\n                            <Object>oItemGroups</Object>\r\n                          </AdmInfo>";
		try
		{
			if (commandType == EnumCommandType.Update)
			{
				text = text + "<QueryParams><Number>" + Function.ParseInt(doc["ItmsGrpCod"]) + "</Number></QueryParams>";
			}
			text += "<ItemGroups>";
			text += "<row>";
			text = text + "<PriceDifferencesAccount>" + Function.ToString(doc["PriceDifAc"]) + "</PriceDifferencesAccount>";
			text = text + "<StockInflationAdjustAccount>" + Function.ToString(doc["IncreasAc"]) + "</StockInflationAdjustAccount>";
			text = text + "<MinimumOrderQuantity>" + Function.ToString(doc["MinOrdrQty"]) + "</MinimumOrderQuantity>";
			if (Function.ParseLong(doc["OrdrIntrvl"]) > 0)
			{
				text = text + "<OrderInterval>" + Function.ToString(doc["OrdrIntrvl"]) + "</OrderInterval>";
			}
			text = text + "<ExchangeRateDifferencesAccount>" + Function.ToString(doc["ExchangeAc"]) + "</ExchangeRateDifferencesAccount>";
			text = text + "<IncreasingAccount>" + Function.ToString(doc["IncresGlAc"]) + "</IncreasingAccount>";
			text = text + "<StockInflationOffsetAccount>" + Function.ToString(doc["DecreasAc"]) + "</StockInflationOffsetAccount>";
			text = ((!(Function.ToString(doc["PrcrmntMtd"]).ToUpper() == "M")) ? (text + "<ProcurementMethod>bom_Buy</ProcurementMethod>") : (text + "<ProcurementMethod>bom_Make</ProcurementMethod>"));
			text = text + "<PurchaseOffsetAccount>" + Function.ToString(doc["PurchOfsAc"]) + "</PurchaseOffsetAccount>";
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
				text = text + "<InventorySystem>" + text3 + "</InventorySystem>";
			}
			text = text + "<WIPMaterialVarianceAccount>" + Function.ToString(doc["WipVarAcct"]) + "</WIPMaterialVarianceAccount>";
			text = ((!(Function.ToString(doc["PlaningSys"]).ToUpper() == "M")) ? (text + "<PlanningSystem>bop_None</PlanningSystem>") : (text + "<PlanningSystem>bop_MRP</PlanningSystem>"));
			text = text + "<PurchaseAccount>" + Function.ToString(doc["PurchaseAc"]) + "</PurchaseAccount>";
			text = text + "<ReturningAccount>" + Function.ToString(doc["PAReturnAc"]) + "</ReturningAccount>";
			text = text + "<CostInflationAccount>" + Function.ToString(doc["CostRvlAct"]) + "</CostInflationAccount>";
			text = text + "<ExpensesAccount>" + Function.ToString(doc["ExpensesAc"]) + "</ExpensesAccount>";
			text = text + "<RevenuesAccount>" + Function.ToString(doc["RevenuesAc"]) + "</RevenuesAccount>";
			text = text + "<TransfersAccount>" + Function.ToString(doc["TransferAc"]) + "</TransfersAccount>";
			text = text + "<LeadTime>" + Function.ParseInt(doc["LeadTime"]) + "</LeadTime>";
			text = text + "<OrderMultiple>" + Function.ParseDecimal(doc["OrdrMulti"]) + "</OrderMultiple>";
			text = text + "<CostInflationOffsetAccount>" + Function.ToString(doc["CstOffsAct"]) + "</CostInflationOffsetAccount>";
			text = text + "<InventoryAccount>" + Function.ToString(doc["BalInvntAc"]) + "</InventoryAccount>";
			text = text + "<DecreaseGLAccount>" + Function.ToString(doc["DecresGlAc"]) + "</DecreaseGLAccount>";
			text = text + "<GoodsClearingAccount>" + Function.ToString(doc["BalanceAcc"]) + "</GoodsClearingAccount>";
			text = text + "<ForeignRevenuesAccount>" + Function.ToString(doc["FrRevenuAc"]) + "</ForeignRevenuesAccount>";
			text = ((!(Function.ToString(doc["Alert"]) == "Y")) ? (text + "<Alert>tNO</Alert>") : (text + "<Alert>tYES</Alert>"));
			text = text + "<WIPMaterialAccount>" + Function.ToString(doc["WipAcct"]) + "</WIPMaterialAccount>";
			text = text + "<ShippedGoodsAccount>" + Function.ToString(doc["ShpdGdsAct"]) + "</ShippedGoodsAccount>";
			text = text + "<ExemptRevenuesAccount>" + Function.ToString(doc["ExmptIncom"]) + "</ExemptRevenuesAccount>";
			text = text + "<DecreasingAccount>" + Function.ToString(doc["DecreasAc"]) + "</DecreasingAccount>";
			text = text + "<VATInRevenueAccount>" + Function.ToString(doc["VatRevAct"]) + "</VATInRevenueAccount>";
			text = text + "<VarianceAccount>" + Function.ToString(doc["VarianceAc"]) + "</VarianceAccount>";
			text = text + "<EUExpensesAccount>" + Function.ToString(doc["ExpensesAc"]) + "</EUExpensesAccount>";
			text = text + "<ForeignExpensesAccount>" + Function.ToString(doc["FrExpensAc"]) + "</ForeignExpensesAccount>";
			if (Function.ParseLong(doc["CycleCode"]) > 0)
			{
				text = text + "<CycleCode>" + Function.ParseLong(doc["CycleCode"]) + "</CycleCode>";
			}
			text = text + "<CostAccount>" + Function.ToString(doc["SaleCostAc"]) + "</CostAccount>";
			text = text + "<EURevenuesAccount>" + Function.ToString(doc["EURevenuAc"]) + "</EURevenuesAccount>";
			text = text + "<PAReturnAccount>" + Function.ToString(doc["PAReturnAc"]) + "</PAReturnAccount>";
			text = text + "<GroupName>" + Function.ToString(doc["ItmsGrpNam"]) + "</GroupName>";
			text = text + "<ExpenseClearingAct>" + Function.ToString(doc["ExpClrAct"]) + "</ExpenseClearingAct>";
			text = text + "<PurchaseCreditAcc>" + Function.ToString(doc["APCMAct"]) + "</PurchaseCreditAcc>";
			text = text + "<EUPurchaseCreditAcc>" + Function.ToString(doc["APCMEUAct"]) + "</EUPurchaseCreditAcc>";
			text = text + "<ForeignPurchaseCreditAcc>" + Function.ToString(doc["APCMFrnAct"]) + "</ForeignPurchaseCreditAcc>";
			text = text + "<SalesCreditAcc>" + Function.ToString(doc["ARCMAct"]) + "</SalesCreditAcc>";
			text = text + "<SalesCreditEUAcc>" + Function.ToString(doc["ARCMEUAct"]) + "</SalesCreditEUAcc>";
			text = text + "<ExemptedCredits>" + Function.ToString(doc["ARCMExpAct"]) + "</ExemptedCredits>";
			text = text + "<SalesCreditForeignAcc>" + Function.ToString(doc["ARCMFrnAct"]) + "</SalesCreditForeignAcc>";
			text = text + "<ExpenseOffsetAccount>" + Function.ToString(doc["ExpOfstAct"]) + "</ExpenseOffsetAccount>";
			text = text + "<NegativeInventoryAdjustmentAccount>" + Function.ToString(doc["NegStckAct"]) + "</NegativeInventoryAdjustmentAccount>";
			text = text + "<WHIncomingCenvatAccount>" + Function.ToString(doc["WhICenAct"]) + "</WHIncomingCenvatAccount>";
			text = text + "<WHOutgoingCenvatAccount>" + Function.ToString(doc["WhOCenAct"]) + "</WHOutgoingCenvatAccount>";
			text = text + "<StockInTransitAccount>" + Function.ToString(doc["StkInTnAct"]) + "</StockInTransitAccount>";
			text = text + "<WipOffsetProfitAndLossAccount>" + Function.ToString(doc["WipOffset"]) + "</WipOffsetProfitAndLossAccount>";
			text = text + "<InventoryOffsetProfitAndLossAccount>" + Function.ToString(doc["StockOffst"]) + "</InventoryOffsetProfitAndLossAccount>";
			text = text + "<ToleranceDays>" + Function.ParseInt(doc["ToleranDay"]) + "</ToleranceDays>";
			text = text + "<DefaultUoMGroup>" + Function.ParseInt(doc["UgpEntry"]) + "</DefaultUoMGroup>";
			text = text + "<DefaultInventoryUoM>" + Function.ParseInt(doc["IUomEntry"]) + "</DefaultInventoryUoM>";
			text = text + "<PurchaseBalanceAccount>" + Function.ToString(doc["PurBalAct"]) + "</PurchaseBalanceAccount>";
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
			text += "</row>";
			text += "</ItemGroups>";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		text += "</BO></BOM>";
		if (commandType == EnumCommandType.Update)
		{
			return text + "</dis:UpdateObject>";
		}
		return text + "</dis:AddObject>";
	}

	protected void ProcessDefaultBin(DataTable dt, int retKey)
	{
		try
		{
			using Transaction transaction = new Transaction();
			ExecuteNonQuery("DELETE FROM OIGW WHERE \"ItmsGrpCod\"=@ItmsGrpCod;", CommandType.Text, new SqlParameter("@ItmsGrpCod", retKey));
			int num = 0;
			DataTable dataTable = ExecuteDataTable(ProcessDefaultBinQuery(), CommandType.Text);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				num = Function.ParseInt(dataTable.Rows[0][0]);
			}
			if (dt != null && dt.Rows.Count > 0)
			{
				List<SqlParameter> list = new List<SqlParameter>();
				list.Add(new SqlParameter("@CreateDate", DateTime.Today));
				list.Add(new SqlParameter("@UpdateDate", DateTime.Today));
				DataTable dataTable2 = dt.Clone();
				DataTable dataTable3 = dt.Clone();
				for (int i = 0; i < dt.Rows.Count; i++)
				{
					if (Exists(retKey, Function.ToString(dt.Rows[i]["WhsCode"])))
					{
						dataTable2.ImportRow(dt.Rows[i]);
					}
					else
					{
						dataTable3.ImportRow(dt.Rows[i]);
					}
				}
				string text = "";
				int num2 = Function.ParseInt(dt.Rows[0]["UserSign"]);
				int num3 = 0;
				for (int j = 0; j < dataTable2.Rows.Count; j++)
				{
					text = text + Environment.NewLine + " UPDATE OIGW\r\n                                SET ";
					text = text + "\"DftBinAbs\"=@DftBinAbs" + num3;
					text = text + "\"DftBinEnfd\"=@DftBinEnfd" + num3;
					text += "\"UpdateDate\"=@UpdateDate";
					text = text + "\"UserSign2\"=" + num2;
					text = text + " WHERE \"WhsCode\"=@WhsCode" + num3 + " AND \"ItmsGrpCod\"=" + retKey + Environment.NewLine;
					text += "; ";
					list.AddRange(new SqlParameter[3]
					{
						new SqlParameter("@DftBinAbs" + num3, Function.ParseInt(dt.Rows[j]["DftBinAbs"])),
						new SqlParameter("@DftBinEnfd" + num3, Function.ToString(dt.Rows[j]["DftBinEnfd"])),
						new SqlParameter("@WhsCode" + num3, Function.ToString(dt.Rows[j]["WhsCode"]))
					});
					num3++;
				}
				if (dataTable3.Rows.Count > 0)
				{
					text = text + Environment.NewLine + "INSERT INTO OIGW\r\n                                       (\"AbsEntry\"\r\n                                       ,\"ItmsGrpCod\"\r\n                                       ,\"WhsCode\"\r\n                                       ,\"DftBinAbs\"\r\n                                       ,\"DftBinEnfd\"\r\n                                       ,\"DataSource\"\r\n                                       ,\"UserSign\"\r\n                                       ,\"LogInstanc\"\r\n                                       ,\"CreateDate\")\r\n                                 VALUES ";
					for (int k = 0; k < dataTable3.Rows.Count; k++)
					{
						num++;
						text = text + "(@AbsEntry" + num3 + "," + retKey + ",@WhsCode" + num3 + ",@DftBinAbs" + num3 + ",@DftBinEnfd" + num3 + ",'N'," + num2 + ",0,@CreateDate),";
						list.AddRange(new SqlParameter[4]
						{
							new SqlParameter("@AbsEntry" + num3, num),
							new SqlParameter("@WhsCode" + num3, Function.ToString(dt.Rows[k]["WhsCode"])),
							new SqlParameter("@DftBinAbs" + num3, Function.ParseInt(dt.Rows[k]["DftBinAbs"])),
							new SqlParameter("@DftBinEnfd" + num3, Function.ToString(dt.Rows[k]["DftBinEnfd"]))
						});
						num3++;
					}
				}
				if (text.EndsWith(","))
				{
					text = text.Remove(text.Length - 1);
				}
				if (!text.EndsWith(";"))
				{
					text += ";";
				}
				ExecuteNonQuery(text, CommandType.Text, list.ToArray());
			}
			transaction.Commit();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
	}

	protected virtual string ProcessDefaultBinQuery()
	{
		return "select max(isnull(absentry, 0)) from OIGW";
	}

	private bool Exists(int itmsGrpCod, string whsCode)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("select 1 from OIGW where \"ItmsGrpCod\"=@ItmsGrpCod and \"WhsCode\"=@whsCode;", CommandType.Text, new SqlParameter("@ItmsGrpCod", itmsGrpCod), new SqlParameter("@whsCode", whsCode));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return true;
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return false;
		}
	}

	protected virtual bool Exists(string p, string sessionId)
	{
		try
		{
			string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?>\r\n                            <env:Envelope xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                             <env:Header>\r\n                              <SessionID>" + sessionId + "</SessionID> \r\n                             </env:Header>\r\n                             <env:Body>\r\n                              <dis:GetByKey xmlns:dis=\"http://www.sap.com/SBO/DIS\">\r\n                               <Object>oItemGroups</Object> \r\n                               <Number>" + Function.RawText(p) + "</Number> \r\n                              </dis:GetByKey>\r\n                             </env:Body>\r\n                            </env:Envelope>";
			XmlElement xmlElement = DataService.GetByKey(sessionId, xmlObject).Content as XmlElement;
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				return false;
			}
			return true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return false;
	}

	public void Dispose()
	{
		throw new NotImplementedException();
	}
}
