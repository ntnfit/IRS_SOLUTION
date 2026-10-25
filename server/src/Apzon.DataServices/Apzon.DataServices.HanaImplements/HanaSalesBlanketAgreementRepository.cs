using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.DataServices.Implements;
using Apzon.Entities.Models.Common;
using Apzon.Services;
using SAPbobsCOM;

namespace Apzon.DataServices.HanaImplements;

public class HanaSalesBlanketAgreementRepository : SalesBlanketAgreementRepository
{
	public HanaSalesBlanketAgreementRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, dataService)
	{
	}

	public override string Save(string sessionId, DataSet doc)
	{
		try
		{
			HttpResult httpResult = new HttpResult();
			int num = Function.ParseInt(doc.Tables[0].Rows[0]["AbsID"]);
			string jsonObject = MappingJson(doc.Tables[0], doc.Tables[1], doc.Tables[2]);
			if (num > 0)
			{
				httpResult = DataService.ProcessJson(ApzObjectTypes.oBlanketAgreement, sessionId, jsonObject, Enums.HttpAction.Update, num);
				if (!string.IsNullOrEmpty(httpResult.Message))
				{
					Logging.Write("WATCH", string.Format("Update BlanketAgreement Error \t" + httpResult.Message));
					return httpResult.Message;
				}
				Logging.Write("WATCH", string.Format("Update BlanketAgreement \t" + num));
			}
			else
			{
				httpResult = DataService.ProcessJson(ApzObjectTypes.oBlanketAgreement, sessionId, jsonObject);
				if (!string.IsNullOrEmpty(httpResult.Message))
				{
					Logging.Write("WATCH", string.Format("Add BlanketAgreement Error \t" + httpResult.Message));
					return httpResult.Message;
				}
				Logging.Write("WATCH", string.Format("Add BlanketAgreement \t" + httpResult.Content));
			}
			if (string.IsNullOrEmpty(httpResult.Message) && doc.Tables.Count > 3 && doc.Tables[3] != null && doc.Tables[3].Rows.Count > 0)
			{
				string text = SaveOAT3(doc.Tables[3]);
				if (!string.IsNullOrEmpty(text))
				{
					Logging.Write("WATCH", string.Format("SaveOAT3 Error \t", text));
				}
			}
			return httpResult.Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public override string Cancel(int docEntry, string sessionId)
	{
		try
		{
			HttpResult httpResult = new HttpResult();
			httpResult = DataService.ProcessJson(ApzObjectTypes.oBlanketAgreement, sessionId, "", Enums.HttpAction.Cancel, docEntry);
			if (string.IsNullOrEmpty(httpResult.Message))
			{
				Logging.Write("WATCH", string.Format("Cancel BlanketAgreement \t" + docEntry));
			}
			else
			{
				Logging.Write("WATCH", string.Format("Cancel BlanketAgreement Error \t" + httpResult.Message));
			}
			return httpResult.Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private string MappingJson(DataTable doc, DataTable dtLines, DataTable dtDetails)
	{
		try
		{
			string text = "{ ";
			if (ColumnExist(doc.Rows[0], "AbsID") && Function.ParseInt(Function.ParseInt(doc.Rows[0]["AbsID"])) > 0)
			{
				text += Function.GetJsonString("AgreementNo", Function.ParseInt(doc.Rows[0]["AbsID"]));
			}
			if (ColumnExist(doc.Rows[0], "BpCode") && !string.IsNullOrEmpty(Function.ToString(doc.Rows[0]["BpCode"])))
			{
				text += Function.GetJsonString("BPCode", Function.ToString(doc.Rows[0]["BpCode"]));
			}
			if (ColumnExist(doc.Rows[0], "BpName") && !string.IsNullOrEmpty(Function.ToString(doc.Rows[0]["BpName"])))
			{
				text += Function.GetJsonString("BPName", Function.ToString(doc.Rows[0]["BpName"]));
			}
			if (ColumnExist(doc.Rows[0], "CntctCode") && Function.ParseInt(doc.Rows[0]["CntctCode"]) > 0)
			{
				text += Function.GetJsonString("ContactPersonCode", Function.ParseInt(doc.Rows[0]["CntctCode"]));
			}
			if (ColumnExist(doc.Rows[0], "StartDate") && !string.IsNullOrEmpty(Function.ToString(doc.Rows[0]["StartDate"])))
			{
				text += Function.GetJsonString("StartDate", Convert.ToDateTime(doc.Rows[0]["StartDate"]).ToString("yyyy-MM-dd"));
			}
			if (ColumnExist(doc.Rows[0], "EndDate") && !string.IsNullOrEmpty(Function.ToString(doc.Rows[0]["EndDate"])))
			{
				text += Function.GetJsonString("EndDate", Convert.ToDateTime(doc.Rows[0]["EndDate"]).ToString("yyyy-MM-dd"));
			}
			if (ColumnExist(doc.Rows[0], "TermDate") && !string.IsNullOrEmpty(Function.ToString(doc.Rows[0]["TermDate"])))
			{
				text += Function.GetJsonString("TerminateDate", Convert.ToDateTime(doc.Rows[0]["TermDate"]).ToString("yyyy-MM-dd"));
			}
			if (ColumnExist(doc.Rows[0], "Descript") && !string.IsNullOrEmpty(Function.ToString(doc.Rows[0]["Descript"])))
			{
				text += Function.GetJsonString("Description", Function.ToString(doc.Rows[0]["Descript"]));
			}
			if (ColumnExist(doc.Rows[0], "Type") && !string.IsNullOrEmpty(Function.ToString(doc.Rows[0]["Type"])))
			{
				string text2 = Function.ToString(doc.Rows[0]["Type"]);
				if (!(text2 == "G"))
				{
					if (text2 == "S")
					{
						text += Function.GetJsonString("AgreementType", Function.ToString(BlanketAgreementTypeEnum.atSpecific));
					}
				}
				else
				{
					text += Function.GetJsonString("AgreementType", Function.ToString(BlanketAgreementTypeEnum.atGeneral));
				}
			}
			if (ColumnExist(doc.Rows[0], "Status") && !string.IsNullOrEmpty(Function.ToString(doc.Rows[0]["Status"])))
			{
				text += Function.GetJsonString("Status", Function.ToString(doc.Rows[0]["Status"]));
			}
			if (ColumnExist(doc.Rows[0], "Owner") && Function.ParseInt(doc.Rows[0]["Owner"]) > 0)
			{
				text += Function.GetJsonString("Owner", Function.ParseInt(doc.Rows[0]["Owner"]));
			}
			else if (Function.ParseInt(doc.Rows[0]["Owner"]) == 0)
			{
				text += Function.GetJsonString("Owner", -1);
			}
			if (ColumnExist(doc.Rows[0], "UseDiscnt") && !string.IsNullOrEmpty(Function.ToString(doc.Rows[0]["UseDiscnt"])))
			{
				string text2 = Function.ToString(doc.Rows[0]["UseDiscnt"]);
				if (!(text2 == "Y"))
				{
					if (text2 == "N")
					{
						text += Function.GetJsonString("IgnorePricesInAgreement", Function.ToString(BoYesNoEnum.tNO));
					}
				}
				else
				{
					text += Function.GetJsonString("IgnorePricesInAgreement", Function.ToString(BoYesNoEnum.tYES));
				}
			}
			if (ColumnExist(doc.Rows[0], "Renewal") && !string.IsNullOrEmpty(Function.ToString(doc.Rows[0]["Renewal"])))
			{
				string text2 = Function.ToString(doc.Rows[0]["Renewal"]);
				if (!(text2 == "Y"))
				{
					if (text2 == "N")
					{
						text += Function.GetJsonString("Renewal", Function.ToString(BoYesNoEnum.tNO));
					}
				}
				else
				{
					text += Function.GetJsonString("Renewal", Function.ToString(BoYesNoEnum.tYES));
				}
			}
			if (ColumnExist(doc.Rows[0], "RemindUnit") && !string.IsNullOrEmpty(Function.ToString(doc.Rows[0]["RemindUnit"])))
			{
				switch (Function.ToString(doc.Rows[0]["RemindUnit"]))
				{
				case "D":
					text += Function.GetJsonString("RemindUnit", Function.ToString(BoRemindUnits.reu_Days));
					break;
				case "W":
					text += Function.GetJsonString("RemindUnit", Function.ToString(BoRemindUnits.reu_Weeks));
					break;
				case "M":
					text += Function.GetJsonString("RemindUnit", Function.ToString(BoRemindUnits.reu_Month));
					break;
				}
			}
			if (ColumnExist(doc.Rows[0], "RemindVal") && Function.ParseInt(doc.Rows[0]["RemindVal"]) > 0)
			{
				text += Function.GetJsonString("RemindTime", Function.ParseInt(doc.Rows[0]["RemindVal"]));
			}
			if (ColumnExist(doc.Rows[0], "Remarks") && !string.IsNullOrEmpty(Function.ToString(doc.Rows[0]["Remarks"])))
			{
				text += Function.GetJsonString("Remarks", Function.ToString(doc.Rows[0]["Remarks"]));
			}
			if (ColumnExist(doc.Rows[0], "AtchEntry"))
			{
				text = ((Function.ParseInt(doc.Rows[0]["AtchEntry"]) <= 0) ? (text + Function.GetJsonString("AttachmentEntry", null)) : (text + Function.GetJsonString("AttachmentEntry", Function.ParseInt(doc.Rows[0]["AtchEntry"]))));
			}
			if (ColumnExist(doc.Rows[0], "SettleProb") && Function.ParseDecimal(doc.Rows[0]["SettleProb"]) > 0m)
			{
				text += Function.GetJsonString("SettlementProbability", Function.ParseDecimal(doc.Rows[0]["SettleProb"]));
			}
			if (ColumnExist(doc.Rows[0], "Method") && !string.IsNullOrEmpty(Function.ToString(doc.Rows[0]["Method"])))
			{
				string text2 = Function.ToString(doc.Rows[0]["Method"]);
				if (!(text2 == "I"))
				{
					if (text2 == "M")
					{
						text += Function.GetJsonString("AgreementMethod", Function.ToString(BlanketAgreementMethodEnum.amMonetary));
					}
				}
				else
				{
					text += Function.GetJsonString("AgreementMethod", Function.ToString(BlanketAgreementMethodEnum.amItem));
				}
			}
			if (ColumnExist(doc.Rows[0], "GroupNum") && Function.ParseInt(doc.Rows[0]["GroupNum"]) > 0)
			{
				text += Function.GetJsonString("PaymentTerms", Function.ParseInt(doc.Rows[0]["GroupNum"]));
			}
			if (ColumnExist(doc.Rows[0], "ListNum") && Function.ParseInt(doc.Rows[0]["ListNum"]) > 0)
			{
				text += Function.GetJsonString("PriceList", Function.ParseInt(doc.Rows[0]["ListNum"]));
			}
			if (ColumnExist(doc.Rows[0], "SignDate") && !string.IsNullOrEmpty(Function.ToString(doc.Rows[0]["SignDate"])))
			{
				text += Function.GetJsonString("SigningDate", Convert.ToDateTime(doc.Rows[0]["SignDate"]).ToString("yyyy-MM-dd"));
			}
			if (ColumnExist(doc.Rows[0], "AmendedTo") && Function.ParseInt(doc.Rows[0]["AmendedTo"]) > 0)
			{
				text += Function.GetJsonString("AmendmentTo", Function.ToString(doc.Rows[0]["AmendedTo"]));
			}
			if (ColumnExist(doc.Rows[0], "Series") && Function.ParseInt(doc.Rows[0]["Series"]) < 0)
			{
				text += Function.GetJsonString("Series", Function.ParseInt(doc.Rows[0]["Series"]));
			}
			if (ColumnExist(doc.Rows[0], "Number") && Function.ParseInt(doc.Rows[0]["Number"]) > 0)
			{
				text += Function.GetJsonString("DocNum", Function.ParseInt(doc.Rows[0]["Number"]));
			}
			if (ColumnExist(doc.Rows[0], "Handwrtten") && !string.IsNullOrEmpty(Function.ToString(doc.Rows[0]["Handwrtten"])))
			{
				string text2 = Function.ToString(doc.Rows[0]["Handwrtten"]);
				if (!(text2 == "Y"))
				{
					if (text2 == "N")
					{
						text += Function.GetJsonString("HandWritten", Function.ToString(BoYesNoEnum.tNO));
					}
				}
				else
				{
					text += Function.GetJsonString("HandWritten", Function.ToString(BoYesNoEnum.tYES));
				}
			}
			text += Function.GetJsonString("PeriodIndicator", "Default");
			if (ColumnExist(doc.Rows[0], "PayMethod") && !string.IsNullOrEmpty(Function.ToString(doc.Rows[0]["PayMethod"])))
			{
				text += Function.GetJsonString("PaymentMethod", Function.ToString(doc.Rows[0]["PayMethod"]));
			}
			if (ColumnExist(doc.Rows[0], "TrnspCode") && Function.ParseInt(doc.Rows[0]["TrnspCode"]) > 0)
			{
				text += Function.GetJsonString("ShippingType", Function.ParseInt(doc.Rows[0]["TrnspCode"]));
			}
			if (ColumnExist(doc.Rows[0], "NumAtCard") && !string.IsNullOrEmpty(Function.ToString(doc.Rows[0]["NumAtCard"])))
			{
				text += Function.GetJsonString("NumAtCard", Function.ToString(doc.Rows[0]["NumAtCard"]));
			}
			if (ColumnExist(doc.Rows[0], "Project") && !string.IsNullOrEmpty(Function.ToString(doc.Rows[0]["Project"])))
			{
				text += Function.GetJsonString("Project", Function.ToString(doc.Rows[0]["Project"]));
			}
			if (ColumnExist(doc.Rows[0], "BPCurr") && !string.IsNullOrEmpty(Function.ToString(doc.Rows[0]["BPCurr"])))
			{
				text += Function.GetJsonString("BPCurrency", Function.ToString(doc.Rows[0]["BPCurr"]));
			}
			text += Function.GetJsonString("PriceMode", "pmNet");
			text += DataService.MappingUdf(doc.Rows[0]);
			if (dtLines.IsNotNull())
			{
				text += "\"BlanketAgreements_ItemsLines\":[ ";
				foreach (DataRow dr in dtLines.Rows)
				{
					text += "{ ";
					if (ColumnExist(dr, "AgrNo") && Function.ParseInt(dr["AgrNo"]) > 0)
					{
						text += Function.GetJsonString("AgreementNo", Function.ParseInt(dr["AgrNo"]));
					}
					if (ColumnExist(dr, "AgrLineNum") && Function.ParseInt(dr["AgrLineNum"]) > 0)
					{
						text += Function.GetJsonString("AgreementRowNumber", Function.ParseInt(dr["AgrLineNum"]));
					}
					if (ColumnExist(dr, "ItemCode") && !string.IsNullOrEmpty(Function.ToString(dr["ItemCode"])))
					{
						text += Function.GetJsonString("ItemNo", Function.ToString(dr["ItemCode"]));
					}
					if (ColumnExist(dr, "ItemName") && !string.IsNullOrEmpty(Function.ToString(dr["ItemName"])))
					{
						text += Function.GetJsonString("ItemDescription", Function.ToString(dr["ItemName"]));
					}
					if (ColumnExist(dr, "ItemGroup") && Function.ParseInt(dr["ItemGroup"]) > 0)
					{
						text += Function.GetJsonString("ItemGroup", Function.ParseInt(dr["ItemGroup"]));
					}
					if (ColumnExist(dr, "PlanQty") && Function.ParseDecimal(dr["PlanQty"]) > 0m)
					{
						text += Function.GetJsonString("PlannedQuantity", Function.ParseDecimal(dr["PlanQty"]));
					}
					if (ColumnExist(dr, "UnitPrice") && Function.ParseDecimal(dr["UnitPrice"]) > 0m)
					{
						text += Function.GetJsonString("UnitPrice", Function.ParseDecimal(dr["UnitPrice"]));
					}
					if (ColumnExist(dr, "Currency") && !string.IsNullOrEmpty(Function.ToString(dr["Currency"])))
					{
						text += Function.GetJsonString("PriceCurrency", Function.ToString(dr["Currency"]));
					}
					if (ColumnExist(dr, "CumQty") && Function.ParseDecimal(dr["CumQty"]) > 0m)
					{
						text += Function.GetJsonString("CumulativeQuantity", Function.ParseDecimal(dr["CumQty"]));
					}
					if (ColumnExist(dr, "CumAmntFC") && Function.ParseDecimal(dr["CumAmntFC"]) > 0m)
					{
						text += Function.GetJsonString("CumulativeAmountFC", Function.ParseDecimal(dr["CumAmntFC"]));
					}
					if (ColumnExist(dr, "CumAmntLC") && Function.ParseDecimal(dr["CumAmntLC"]) > 0m)
					{
						text += Function.GetJsonString("CumulativeAmountLC", Function.ParseDecimal(dr["CumAmntLC"]));
					}
					if (ColumnExist(dr, "FreeTxt") && !string.IsNullOrEmpty(Function.ToString(dr["FreeTxt"])))
					{
						text += Function.GetJsonString("FreeText", Function.ToString(dr["FreeTxt"]));
					}
					if (ColumnExist(dr, "InvntryUom") && !string.IsNullOrEmpty(Function.ToString(dr["InvntryUom"])))
					{
						text += Function.GetJsonString("InventoryUoM", Function.ToString(dr["InvntryUom"]));
					}
					if (ColumnExist(dr, "RetPortion") && Function.ParseDecimal(dr["RetPortion"]) > 0m)
					{
						text += Function.GetJsonString("PortionOfReturns", Function.ParseDecimal(dr["RetPortion"]));
					}
					if (ColumnExist(dr, "WrrtyEnd") && !string.IsNullOrEmpty(Function.ToString(dr["WrrtyEnd"])))
					{
						text += Function.GetJsonString("EndOfWarranty", Convert.ToDateTime(dr["WrrtyEnd"]).ToString("yyyy-MM-dd"));
					}
					if (ColumnExist(dr, "PlanAmtLC") && Function.ParseDecimal(dr["PlanAmtLC"]) > 0m)
					{
						text += Function.GetJsonString("PlannedAmountLC", Function.ParseDecimal(dr["PlanAmtLC"]));
					}
					if (ColumnExist(dr, "PlanAmtFC") && Function.ParseDecimal(dr["PlanAmtFC"]) > 0m)
					{
						text += Function.GetJsonString("PlannedAmountFC", Function.ParseDecimal(dr["PlanAmtFC"]));
					}
					if (ColumnExist(dr, "Discount") && Function.ParseDecimal(dr["Discount"]) > 0m)
					{
						text += Function.GetJsonString("LineDiscount", Function.ParseDecimal(dr["Discount"]));
					}
					if (ColumnExist(dr, "UoMEntry") && Function.ParseInt(dr["UoMEntry"]) > 0)
					{
						text += Function.GetJsonString("UoMEntry", Function.ParseInt(dr["UoMEntry"]));
					}
					if (ColumnExist(dr, "UomCode") && !string.IsNullOrEmpty(Function.ToString(dr["UomCode"])))
					{
						text += Function.GetJsonString("UoMCode", Function.ToString(dr["UomCode"]));
					}
					if (ColumnExist(dr, "NumPerMsr") && Function.ParseDecimal(dr["NumPerMsr"]) > 0m)
					{
						text += Function.GetJsonString("UnitsOfMeasurement", Function.ParseDecimal(dr["NumPerMsr"]));
					}
					if (ColumnExist(dr, "UndlvQty") && Function.ParseDecimal(dr["UndlvQty"]) > 0m)
					{
						text += Function.GetJsonString("UndeliveredCumulativeQuantity", Function.ParseDecimal(dr["UndlvQty"]));
					}
					if (ColumnExist(dr, "UndlvAmntL") && Function.ParseDecimal(dr["UndlvAmntL"]) > 0m)
					{
						text += Function.GetJsonString("UndeliveredCumulativeAmountLC", Function.ParseDecimal(dr["UndlvAmntL"]));
					}
					if (ColumnExist(dr, "UndlvAmntF") && Function.ParseDecimal(dr["UndlvAmntF"]) > 0m)
					{
						text += Function.GetJsonString("UndeliveredCumulativeAmountFC", Function.ParseDecimal(dr["UndlvAmntF"]));
					}
					if (ColumnExist(dr, "TrnspCode") && Function.ParseInt(dr["TrnspCode"]) > 0)
					{
						text += Function.GetJsonString("ShippingType", Function.ParseInt(dr["TrnspCode"]));
					}
					if (ColumnExist(dr, "Project") && !string.IsNullOrEmpty(Function.ToString(dr["Project"])))
					{
						text += Function.GetJsonString("Project", Function.ToString(dr["Project"]));
					}
					if (ColumnExist(dr, "TaxCode") && !string.IsNullOrEmpty(Function.ToString(dr["TaxCode"])))
					{
						text += Function.GetJsonString("TaxCode", Function.ToString(dr["TaxCode"]));
					}
					if (ColumnExist(dr, "TAXRate") && Function.ParseDecimal(dr["TAXRate"]) > 0m)
					{
						text += Function.GetJsonString("TAXRate", Function.ParseDecimal(dr["TAXRate"]));
					}
					if (ColumnExist(dr, "PlVatAmtLC") && Function.ParseDecimal(dr["PlVatAmtLC"]) > 0m)
					{
						text += Function.GetJsonString("PlannedVATAmountLC", Function.ParseDecimal(dr["PlVatAmtLC"]));
					}
					if (ColumnExist(dr, "PlVatAmtFC") && Function.ParseDecimal(dr["PlVatAmtFC"]) > 0m)
					{
						text += Function.GetJsonString("PlannedVATAmountFC", Function.ParseDecimal(dr["PlVatAmtFC"]));
					}
					if (ColumnExist(dr, "CumVtAmtLC") && Function.ParseDecimal(dr["CumVtAmtLC"]) > 0m)
					{
						text += Function.GetJsonString("CumulativeVATAmountLC", Function.ParseDecimal(dr["CumVtAmtLC"]));
					}
					if (ColumnExist(dr, "CumVtAmtFC") && Function.ParseDecimal(dr["CumVtAmtFC"]) > 0m)
					{
						text += Function.GetJsonString("CumulativeVATAmountFC", Function.ParseDecimal(dr["CumVtAmtFC"]));
					}
					text += DataService.MappingUdf(dr);
					if (dtDetails != null && dtDetails.Rows.Count > 0)
					{
						List<DataRow> list = (from t in dtDetails.AsEnumerable()
							where Function.ParseInt(t["AgrNo"]) == Function.ParseInt(dr["AgrNo"]) && Function.ParseInt(t["AgrLnNum"]) == Function.ParseInt(dr["AgrLineNum"])
							select t).ToList();
						if (list != null && list.Count > 0)
						{
							text += "\"BlanketAgreements_DetailsLines\":[ ";
							foreach (DataRow item in list)
							{
								text += "{ ";
								if (ColumnExist(item, "AgrNo") && Function.ParseInt(item["AgrNo"]) > 0)
								{
									text += Function.GetJsonString("AgreementNo", Function.ParseInt(item["AgrNo"]));
								}
								if (ColumnExist(item, "AgrLnNum") && Function.ParseInt(item["AgrLnNum"]) > 0)
								{
									text += Function.GetJsonString("AgreementRowNumber", Function.ParseInt(item["AgrLnNum"]));
								}
								if (ColumnExist(item, "AgrEfctNum") && Function.ParseInt(item["AgrEfctNum"]) > 0)
								{
									text += Function.GetJsonString("AgreementEffectiveRowNumber", Function.ParseInt(item["AgrEfctNum"]));
								}
								if (ColumnExist(item, "DatePeriod") && !string.IsNullOrEmpty(Function.ToString(item["DatePeriod"])))
								{
									switch (Function.ToString(item["DatePeriod"]))
									{
									case "D":
										text += Function.GetJsonString("Frequency", Function.ToString(BlanketAgreementDatePeriodsEnum.Daily));
										break;
									case "W":
										text += Function.GetJsonString("Frequency", Function.ToString(BlanketAgreementDatePeriodsEnum.Weekly));
										break;
									case "M":
										text += Function.GetJsonString("Frequency", Function.ToString(BlanketAgreementDatePeriodsEnum.Monthly));
										break;
									case "Q":
										text += Function.GetJsonString("Frequency", Function.ToString(BlanketAgreementDatePeriodsEnum.Quarterly));
										break;
									case "S":
										text += Function.GetJsonString("Frequency", Function.ToString(BlanketAgreementDatePeriodsEnum.Semiannually));
										break;
									case "A":
										text += Function.GetJsonString("Frequency", Function.ToString(BlanketAgreementDatePeriodsEnum.Annually));
										break;
									case "O":
										text += Function.GetJsonString("Frequency", Function.ToString(BlanketAgreementDatePeriodsEnum.OneTime));
										break;
									}
								}
								if (ColumnExist(item, "FromDate") && !string.IsNullOrEmpty(Function.ToString(item["FromDate"])))
								{
									text += Function.GetJsonString("From", Convert.ToDateTime(item["FromDate"]).ToString("yyyy-MM-dd"));
								}
								if (ColumnExist(item, "ToDate") && !string.IsNullOrEmpty(Function.ToString(item["ToDate"])))
								{
									text += Function.GetJsonString("To", Convert.ToDateTime(item["ToDate"]).ToString("yyyy-MM-dd"));
								}
								if (ColumnExist(item, "CallUp") && !string.IsNullOrEmpty(Function.ToString(item["CallUp"])))
								{
									text += Function.GetJsonString("ReleaseInformation", Function.ToString(item["CallUp"]));
								}
								if (ColumnExist(item, "Quantity") && Function.ParseDecimal(item["Quantity"]) > 0m)
								{
									text += Function.GetJsonString("Quantity", Function.ParseDecimal(item["Quantity"]));
								}
								if (ColumnExist(item, "WhsCode") && !string.IsNullOrEmpty(Function.ToString(item["WhsCode"])))
								{
									text += Function.GetJsonString("Warehouse", Function.ToString(item["WhsCode"]));
								}
								if (ColumnExist(item, "FreeTxt") && !string.IsNullOrEmpty(Function.ToString(item["FreeTxt"])))
								{
									text += Function.GetJsonString("FreeText", Function.ToString(item["FreeTxt"]));
								}
								if (ColumnExist(item, "ConsumeFCT") && !string.IsNullOrEmpty(Function.ToString(item["ConsumeFCT"])))
								{
									string text2 = Function.ToString(item["ConsumeFCT"]);
									if (!(text2 == "Y"))
									{
										if (text2 == "N")
										{
											text += Function.GetJsonString("ConsumeSalesForecast", Function.ToString(BoYesNoEnum.tNO));
										}
									}
									else
									{
										text += Function.GetJsonString("ConsumeSalesForecast", Function.ToString(BoYesNoEnum.tYES));
									}
								}
								if (ColumnExist(item, "AmountLC") && Function.ParseDecimal(item["AmountLC"]) >= 0m)
								{
									text += Function.GetJsonString("PlannedAmountLC", Function.ParseDecimal(item["AmountLC"]));
								}
								if (ColumnExist(item, "AmountFC") && Function.ParseDecimal(item["AmountFC"]) >= 0m)
								{
									text += Function.GetJsonString("PlannedAmountFC", Function.ParseDecimal(item["AmountFC"]));
								}
								text += DataService.MappingUdf(item);
								text += "},";
							}
							text += "],";
						}
					}
					text += "},";
				}
				text += "],";
			}
			return text + "}";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return string.Empty;
	}
}
