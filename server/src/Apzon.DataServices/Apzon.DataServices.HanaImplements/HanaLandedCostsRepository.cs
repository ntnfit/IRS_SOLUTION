using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.DataServices.Implements;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.LandedCosts;
using Apzon.Services;
using SAPbobsCOM;

namespace Apzon.DataServices.HanaImplements;

internal class HanaLandedCostsRepository : LandedCostsRepository
{
	private IUnitOfWork _unitOfWork;

	public HanaLandedCostsRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, unitOfWork, dataService)
	{
		_unitOfWork = unitOfWork;
	}

	protected override int GetMaxDoc()
	{
		try
		{
			string query = "select IFNULL(MAX(\"DocEntry\"), 0) from OPDN;";
			return Function.ParseInt(ExecuteDataTable(query, CommandType.Text).Rows[0][0]);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return -1;
		}
	}

	public override string MappingService(LandedCostImpl doc, EnumCommandType commandType)
	{
		try
		{
			string empty = string.Empty;
			empty = "{";
			if (doc.DocNum > 0)
			{
				empty += Function.GetJsonString("LandedCostNumber", doc.DocNum);
			}
			if (!string.IsNullOrEmpty(Function.ToString(doc.DocDate)) && doc.DocDate > new DateTime(1900, 1, 1))
			{
				empty += Function.GetJsonString("PostingDate", Convert.ToDateTime(doc.DocDate).ToString("yyyyMMdd"));
			}
			if (!string.IsNullOrEmpty(Function.ToString(doc.DocDueDate)) && doc.DocDueDate > new DateTime(1900, 1, 1))
			{
				empty += Function.GetJsonString("DueDate", Convert.ToDateTime(doc.DocDueDate).ToString("yyyyMMdd"));
			}
			if (!string.IsNullOrEmpty(doc.CardCode))
			{
				empty += Function.GetJsonString("VendorCode", Function.RawText(doc.CardCode));
			}
			if (!string.IsNullOrEmpty(doc.SuppName))
			{
				empty += Function.GetJsonString("VendorName", Function.RawText(doc.SuppName));
			}
			if (!string.IsNullOrEmpty(doc.AgentCode))
			{
				empty += Function.GetJsonString("Broker", Function.RawText(doc.AgentCode));
			}
			if (!string.IsNullOrEmpty(doc.AgentName))
			{
				empty += Function.GetJsonString("BrokerName", Function.RawText(doc.AgentName));
			}
			if (!string.IsNullOrEmpty(doc.Descr))
			{
				empty += Function.GetJsonString("Remarks", Function.RawText(doc.Descr));
			}
			if (!string.IsNullOrEmpty(doc.Ref1))
			{
				empty += Function.GetJsonString("Reference", Function.RawText(doc.Ref1));
			}
			if (!string.IsNullOrEmpty(doc.DocCur))
			{
				empty += Function.GetJsonString("DocumentCurrency", Function.RawText(doc.DocCur));
			}
			if (Math.Round(Function.ParseDecimal(doc.DocRate), 6) != 0m)
			{
				empty += Function.GetJsonString("DocumentRate", Function.ParseDecimal(doc.DocRate));
			}
			if (Math.Round(Function.ParseDecimal(doc.ExpCustom), 6) != 0m)
			{
				empty += Function.GetJsonString("ProjectedCustoms", Function.ParseDecimal(doc.ExpCustom));
			}
			if (Math.Round(Function.ParseDecimal(doc.ActCustom), 6) != 0m)
			{
				empty += Function.GetJsonString("ActualCustoms", Function.ParseDecimal(doc.ActCustom));
			}
			if (Math.Round(Function.ParseDecimal(doc.AcCustomFC), 6) != 0m)
			{
				empty += Function.GetJsonString("ActualCustomsFC", Function.ParseDecimal(doc.AcCustomFC));
			}
			if (Math.Round(Function.ParseDecimal(doc.Vat1), 6) != 0m)
			{
				empty += Function.GetJsonString("Tax1", Function.ParseDecimal(doc.Vat1));
			}
			if (Math.Round(Function.ParseDecimal(doc.Vat2), 6) != 0m)
			{
				empty += Function.GetJsonString("Tax2", Function.ParseDecimal(doc.Vat2));
			}
			if (Math.Round(Function.ParseDecimal(doc.BeforeVat), 6) != 0m)
			{
				empty += Function.GetJsonString("BeforeTax", Function.ParseDecimal(doc.BeforeVat));
			}
			if (Math.Round(Function.ParseDecimal(doc.DocTotal), 6) != 0m)
			{
				empty += Function.GetJsonString("Total", Function.ParseDecimal(doc.DocTotal));
			}
			if (Math.Round(Function.ParseDecimal(doc.CostSum), 6) != 0m)
			{
				empty += Function.GetJsonString("TotalFreightCharges", Function.ParseDecimal(doc.CostSum));
			}
			if (Math.Round(Function.ParseDecimal(doc.ExCustomFC), 6) != 0m)
			{
				empty += Function.GetJsonString("ProjectedCustomsFC", Function.ParseDecimal(doc.ExCustomFC));
			}
			if (Math.Round(Function.ParseDecimal(doc.Vat1FC), 6) != 0m)
			{
				empty += Function.GetJsonString("Tax1FC", Function.ParseDecimal(doc.Vat1FC));
			}
			if (Math.Round(Function.ParseDecimal(doc.Vat2FC), 6) != 0m)
			{
				empty += Function.GetJsonString("Tax2FC", Function.ParseDecimal(doc.Vat2FC));
			}
			if (Math.Round(Function.ParseDecimal(doc.BeforVatFC), 6) != 0m)
			{
				empty += Function.GetJsonString("BeforeTaxFC", Function.ParseDecimal(doc.BeforVatFC));
			}
			if (Math.Round(Function.ParseDecimal(doc.DocTotalFC), 6) != 0m)
			{
				empty += Function.GetJsonString("TotalFC", Function.ParseDecimal(doc.DocTotalFC));
			}
			if (Math.Round(Function.ParseDecimal(doc.CostSumFC), 6) != 0m)
			{
				empty += Function.GetJsonString("TotalFreightChargesFC", Function.ParseDecimal(doc.CostSumFC));
			}
			if (doc.Series > 0)
			{
				empty += Function.GetJsonString("Series", doc.Series);
			}
			if (!string.IsNullOrEmpty(doc.incCustom))
			{
				empty += Function.GetJsonString("CustomsAffectsInventory", doc.incCustom.Equals("Y") ? "tYES" : "tNO");
			}
			if (Math.Round(Function.ParseDecimal(doc.Cost_Match), 6) != 0m)
			{
				empty += Function.GetJsonString("AmountToBalance", Function.ParseDecimal(doc.Cost_Match));
			}
			if (Math.Round(Function.ParseDecimal(doc.C_Match_FC), 6) != 0m)
			{
				empty += Function.GetJsonString("AmountToBalanceFC", Function.ParseDecimal(doc.C_Match_FC));
			}
			if (!string.IsNullOrEmpty(doc.BillOfLad))
			{
				empty += Function.GetJsonString("BillofLadingNumber", Function.RawText(doc.BillOfLad));
			}
			if (doc.TrnspCode > 0)
			{
				empty += Function.GetJsonString("TransportType", doc.TrnspCode);
			}
			if (doc.JdtNum > 0)
			{
				empty += Function.GetJsonString("TransactionNumber", doc.JdtNum);
			}
			if (!string.IsNullOrEmpty(doc.JdtMemo))
			{
				empty += Function.GetJsonString("JournalRemarks", Function.RawText(doc.JdtMemo));
			}
			empty = ((doc.AtcEntry <= 0) ? (empty + Function.GetJsonString("AttachmentEntry", null)) : (empty + Function.GetJsonString("AttachmentEntry", doc.AtcEntry)));
			if (doc.UserFields != null)
			{
				_ = doc.UserFields.Count;
				_ = 0;
			}
			if (doc.ItemLines != null && doc.ItemLines.Count > 0)
			{
				empty += "LandedCost_ItemLines:[";
				foreach (LandedCost_ItemLineImpl itemLine in doc.ItemLines)
				{
					empty += "{";
					if (itemLine.LineNum >= 0)
					{
						empty += Function.GetJsonString("LineNumber", itemLine.LineNum);
					}
					if (itemLine.BaseType >= 0)
					{
						empty += Function.GetJsonString("BaseDocumentType", Function.ToString((LandedCostBaseDocumentTypeEnum)itemLine.BaseType));
					}
					if (itemLine.BaseEntry > 0)
					{
						empty += Function.GetJsonString("BaseEntry", itemLine.BaseEntry);
					}
					if (!string.IsNullOrEmpty(itemLine.ItemCode))
					{
						empty += Function.GetJsonString("Number", Function.RawText(itemLine.ItemCode));
					}
					if (!string.IsNullOrEmpty(itemLine.Dscription))
					{
						empty += Function.GetJsonString("ItemDescription", Function.RawText(itemLine.Dscription));
					}
					if (Math.Round(Function.ParseDecimal(itemLine.Quantity), 6) != 0m)
					{
						empty += Function.GetJsonString("Quantity", Function.ParseDecimal(itemLine.Quantity));
					}
					if (Math.Round(Function.ParseDecimal(itemLine.PriceFOB), 6) != 0m)
					{
						empty += Function.GetJsonString("BaseDocumentPrice", Function.ParseDecimal(itemLine.PriceFOB));
					}
					if (Math.Round(Function.ParseDecimal(itemLine.Rate), 6) != 0m)
					{
						empty += Function.GetJsonString("Rate", Function.ParseDecimal(itemLine.Rate));
					}
					if (Math.Round(Function.ParseDecimal(itemLine.Custom), 6) != 0m)
					{
						empty += Function.GetJsonString("ProjectedCustoms", Function.ParseDecimal(itemLine.Custom));
					}
					if (Math.Round(Function.ParseDecimal(itemLine.CustomFC), 6) != 0m)
					{
						empty += Function.GetJsonString("ProjectedCustomsFC", Function.ParseDecimal(itemLine.CustomFC));
					}
					if (Math.Round(Function.ParseDecimal(itemLine.Cost), 6) != 0m)
					{
						empty += Function.GetJsonString("ExpenditureFC", Function.ParseDecimal(itemLine.Cost));
					}
					if (Math.Round(Function.ParseDecimal(itemLine.PriceAtWH), 6) != 0m)
					{
						empty += Function.GetJsonString("WarehousePrice", Function.ParseDecimal(itemLine.PriceAtWH));
					}
					if (Math.Round(Function.ParseDecimal(itemLine.PricAtWHFC), 6) != 0m)
					{
						empty += Function.GetJsonString("WarehousePriceFC", Function.ParseDecimal(itemLine.PricAtWHFC));
					}
					if (Math.Round(Function.ParseDecimal(itemLine.LineTotal), 6) != 0m)
					{
						empty += Function.GetJsonString("LineTotal", Function.ParseDecimal(itemLine.LineTotal));
					}
					if (Math.Round(Function.ParseDecimal(itemLine.TotalFrgn), 6) != 0m)
					{
						empty += Function.GetJsonString("LineTotalFC", Function.ParseDecimal(itemLine.TotalFrgn));
					}
					if (Math.Round(Function.ParseDecimal(itemLine.Volume), 6) != 0m)
					{
						empty += Function.GetJsonString("Volume", Function.ParseDecimal(itemLine.Volume));
					}
					if (itemLine.UnitCode != 0)
					{
						empty += Function.GetJsonString("VolumeUoM", itemLine.UnitCode);
					}
					if (Math.Round(Function.ParseDecimal(itemLine.Weight1), 6) != 0m)
					{
						empty += Function.GetJsonString("Weight1", Function.ParseDecimal(itemLine.Weight1));
					}
					if (itemLine.UnitCode1 != 0)
					{
						empty += Function.GetJsonString("Weight1UnitCode", itemLine.UnitCode1);
					}
					if (Math.Round(Function.ParseDecimal(itemLine.Weight2), 6) != 0m)
					{
						empty += Function.GetJsonString("Weight2", Function.ParseDecimal(itemLine.Weight2));
					}
					if (itemLine.UnitCode2 != 0)
					{
						empty += Function.GetJsonString("Weight2UnitCode", itemLine.UnitCode2);
					}
					if (!string.IsNullOrEmpty(itemLine.Reference))
					{
						empty += Function.GetJsonString("Reference", Function.RawText(itemLine.Reference));
					}
					if (Math.Round(Function.ParseDecimal(itemLine.FactNoCust), 6) != 0m)
					{
						empty += Function.GetJsonString("FactorWithoutCustoms", Function.ParseDecimal(itemLine.FactNoCust));
					}
					if (Math.Round(Function.ParseDecimal(itemLine.FacWthCust), 6) != 0m)
					{
						empty += Function.GetJsonString("FactorWithCustoms", Function.ParseDecimal(itemLine.FacWthCust));
					}
					if (!string.IsNullOrEmpty(itemLine.UseBaseUn))
					{
						empty += Function.GetJsonString("InventoryUoM", Function.RawText(itemLine.UseBaseUn));
					}
					if (!string.IsNullOrEmpty(itemLine.BlockNum))
					{
						empty += Function.GetJsonString("BlockNumber", Function.RawText(itemLine.BlockNum));
					}
					if (!string.IsNullOrEmpty(itemLine.ImportLog))
					{
						empty += Function.GetJsonString("ImportLog", Function.RawText(itemLine.ImportLog));
					}
					if (!string.IsNullOrEmpty(itemLine.OrigWhs))
					{
						empty += Function.GetJsonString("OriginalWarehouse", Function.RawText(itemLine.OrigWhs));
					}
					if (!string.IsNullOrEmpty(itemLine.WhsCode))
					{
						empty += Function.GetJsonString("Warehouse", Function.RawText(itemLine.WhsCode));
					}
					if (itemLine.ReleaseNum > 0)
					{
						empty += Function.GetJsonString("ReleaseNumber", itemLine.ReleaseNum);
					}
					if (Math.Round(Function.ParseDecimal(itemLine.VarCosts), 6) != 0m)
					{
						empty += Function.GetJsonString("VariantCosts", Function.ParseDecimal(itemLine.VarCosts));
					}
					if (Math.Round(Function.ParseDecimal(itemLine.ConstCosts), 6) != 0m)
					{
						empty += Function.GetJsonString("FixCosts", Function.ParseDecimal(itemLine.ConstCosts));
					}
					if (Math.Round(Function.ParseDecimal(itemLine.VarCostsFR), 6) != 0m)
					{
						empty += Function.GetJsonString("VariantCostsFC", Function.ParseDecimal(itemLine.VarCostsFR));
					}
					if (Math.Round(Function.ParseDecimal(itemLine.CnstCostFR), 6) != 0m)
					{
						empty += Function.GetJsonString("FixCostsFC", Function.ParseDecimal(itemLine.CnstCostFR));
					}
					if (Math.Round(Function.ParseDecimal(itemLine.UserCustom), 6) != 0m)
					{
						empty += Function.GetJsonString("Customs", Function.ParseDecimal(itemLine.UserCustom));
					}
					if (Math.Round(Function.ParseDecimal(itemLine.UsrCusomFC), 6) != 0m)
					{
						empty += Function.GetJsonString("CustomsFC", Function.ParseDecimal(itemLine.UsrCusomFC));
					}
					if (Math.Round(Function.ParseDecimal(itemLine.FobValue), 6) != 0m)
					{
						empty += Function.GetJsonString("BaseDocumentValueLineTotal", Function.ParseDecimal(itemLine.FobValue));
					}
					if (Math.Round(Function.ParseDecimal(itemLine.FobValueFC), 6) != 0m)
					{
						empty += Function.GetJsonString("BaseDocumentValueLineTotalFC", Function.ParseDecimal(itemLine.FobValueFC));
					}
					if (Math.Round(Function.ParseDecimal(itemLine.TtlExpndLC), 6) != 0m)
					{
						empty += Function.GetJsonString("AllocatedUnitCostsLineTotal", Function.ParseDecimal(itemLine.TtlExpndLC));
					}
					if (Math.Round(Function.ParseDecimal(itemLine.TtlExpndFC), 6) != 0m)
					{
						empty += Function.GetJsonString("AllocatedUnitCostsLineTotalFC", Function.ParseDecimal(itemLine.TtlExpndFC));
					}
					if (Math.Round(Function.ParseDecimal(itemLine.TtlCustLC), 6) != 0m)
					{
						empty += Function.GetJsonString("CustomsValue", Function.ParseDecimal(itemLine.TtlCustLC));
					}
					if (Math.Round(Function.ParseDecimal(itemLine.TtlCustFC), 6) != 0m)
					{
						empty += Function.GetJsonString("CustomsValueFC", Function.ParseDecimal(itemLine.TtlCustFC));
					}
					if (Math.Round(Function.ParseDecimal(itemLine.TtlCostLC), 6) != 0m)
					{
						empty += Function.GetJsonString("TotalCosts", Function.ParseDecimal(itemLine.TtlCostLC));
					}
					if (Math.Round(Function.ParseDecimal(itemLine.TtlVolume), 6) != 0m)
					{
						empty += Function.GetJsonString("TotalVolume", Function.ParseDecimal(itemLine.TtlVolume));
					}
					if (itemLine.OrigLine != 0)
					{
						empty += Function.GetJsonString("BaseLine", itemLine.OrigLine);
					}
					if (Math.Round(Function.ParseDecimal(itemLine.TtlCustSC), 6) != 0m)
					{
						empty += Function.GetJsonString("TotalLineProjectedCustoms", Function.ParseDecimal(itemLine.TtlCustSC));
					}
					if (Math.Round(Function.ParseDecimal(itemLine.TtlExpndSC), 6) != 0m)
					{
						empty += Function.GetJsonString("AllocatedCostsLineTotal", Function.ParseDecimal(itemLine.TtlExpndSC));
					}
					if (Math.Round(Function.ParseDecimal(itemLine.FobnLaC), 6) != 0m)
					{
						empty += Function.GetJsonString("FOBandIncludedCosts", Function.ParseDecimal(itemLine.FobnLaC));
					}
					if (Math.Round(Function.ParseDecimal(itemLine.FobnLaCFC), 6) != 0m)
					{
						empty += Function.GetJsonString("FOBandIncludedCostsFC", Function.ParseDecimal(itemLine.FobnLaCFC));
					}
					if (!string.IsNullOrEmpty(itemLine.Project))
					{
						empty += Function.GetJsonString("Project", Function.RawText(itemLine.Project));
					}
					if (!string.IsNullOrEmpty(itemLine.OcrCode))
					{
						empty += Function.GetJsonString("DistributionRule", Function.RawText(itemLine.OcrCode));
					}
					if (!string.IsNullOrEmpty(itemLine.OcrCode2))
					{
						empty += Function.GetJsonString("DistributionRule2", Function.RawText(itemLine.OcrCode2));
					}
					if (!string.IsNullOrEmpty(itemLine.OcrCode3))
					{
						empty += Function.GetJsonString("DistributionRule3", Function.RawText(itemLine.OcrCode3));
					}
					if (!string.IsNullOrEmpty(itemLine.OcrCode4))
					{
						empty += Function.GetJsonString("DistributionRule4", Function.RawText(itemLine.OcrCode4));
					}
					if (!string.IsNullOrEmpty(itemLine.OcrCode5))
					{
						empty += Function.GetJsonString("DistributionRule5", Function.RawText(itemLine.OcrCode5));
					}
					if (itemLine.PriceList != 0)
					{
						empty += Function.GetJsonString("PriceList", itemLine.PriceList);
					}
					if (!string.IsNullOrEmpty(itemLine.CostOH))
					{
						empty += Function.GetJsonString("AutomaticExpenditure", itemLine.CostOH.Equals("Y") ? "tYES" : "tNO");
					}
					if (!string.IsNullOrEmpty(itemLine.StockEval))
					{
						empty += Function.GetJsonString("InventoryValuation", itemLine.StockEval.Equals("Y") ? "tYES" : "tNO");
					}
					if (itemLine.OrigRow != 0)
					{
						empty += Function.GetJsonString("OriginLine", itemLine.OrigRow);
					}
					if (!string.IsNullOrEmpty(itemLine.Currency))
					{
						empty += Function.GetJsonString("Currency", Function.RawText(itemLine.Currency));
					}
					if (Math.Round(Function.ParseDecimal(itemLine.CustRate), 6) != 0m)
					{
						empty += Function.GetJsonString("CustomsGroupRate", Function.RawText(itemLine.CustRate));
					}
					if (!string.IsNullOrEmpty(itemLine.VatGroup))
					{
						empty += Function.GetJsonString("VatGroup", Function.RawText(itemLine.VatGroup));
					}
					if (Math.Round(Function.ParseDecimal(itemLine.VatPrcnt), 6) != 0m)
					{
						empty += Function.GetJsonString("VatPercent", Function.ParseDecimal(itemLine.VatPrcnt));
					}
					if (Math.Round(Function.ParseDecimal(itemLine.ExciseSum), 6) != 0m)
					{
						empty += Function.GetJsonString("ExciseSum", Function.ParseDecimal(itemLine.ExciseSum));
					}
					if (Math.Round(Function.ParseDecimal(itemLine.ExcisSumFC), 6) != 0m)
					{
						empty += Function.GetJsonString("ExciseSumFC", Function.ParseDecimal(itemLine.ExcisSumFC));
					}
					if (!string.IsNullOrEmpty(itemLine.ExcInStk))
					{
						empty += Function.GetJsonString("ExciseAffectStock", itemLine.ExcInStk.Equals("Y") ? "tYES" : "tNO");
					}
					if (Math.Round(Function.ParseDecimal(itemLine.CustomSum), 6) != 0m)
					{
						empty += Function.GetJsonString("CustomsCost", Function.ParseDecimal(itemLine.CustomSum));
					}
					if (Math.Round(Function.ParseDecimal(itemLine.CustSumFC), 6) != 0m)
					{
						empty += Function.GetJsonString("CustomsCostFC", Function.ParseDecimal(itemLine.CustSumFC));
					}
					if (!string.IsNullOrEmpty(itemLine.CstmInStk))
					{
						empty += Function.GetJsonString("CustomsAffectStock", itemLine.CstmInStk.Equals("Y") ? "tYES" : "tNO");
					}
					if (Math.Round(Function.ParseDecimal(itemLine.CstmVatSum), 6) != 0m)
					{
						empty += Function.GetJsonString("CustomsVat", Function.ParseDecimal(itemLine.CstmVatSum));
					}
					if (Math.Round(Function.ParseDecimal(itemLine.CstmVatFC), 6) != 0m)
					{
						empty += Function.GetJsonString("CustomsVatFC", Function.ParseDecimal(itemLine.CstmVatFC));
					}
					if (!string.IsNullOrEmpty(itemLine.CstmVatStk))
					{
						empty += Function.GetJsonString("CustomsVatAffectStock", itemLine.CstmVatStk.Equals("Y") ? "tYES" : "tNO");
					}
					if (!string.IsNullOrEmpty(itemLine.CCDNumber))
					{
						empty += Function.GetJsonString("CCDNumber", Function.RawText(itemLine.CCDNumber));
					}
					if (Math.Round(Function.ParseDecimal(itemLine.FobVal2), 6) != 0m)
					{
						empty += Function.GetJsonString("CorrectedBaseDocumentValue", Function.ParseDecimal(itemLine.FobVal2));
					}
					if (Math.Round(Function.ParseDecimal(itemLine.FobVal2FC), 6) != 0m)
					{
						empty += Function.GetJsonString("CorrectedBaseDocumentValueFC", Function.ParseDecimal(itemLine.FobVal2FC));
					}
					empty += "},";
				}
				empty += "],";
			}
			if (doc.CostLines != null && doc.CostLines.Count > 0)
			{
				empty += "LandedCost_CostLines:[";
				foreach (LandedCost_CostLineImpl costLine in doc.CostLines)
				{
					empty += "{";
					if (!string.IsNullOrEmpty(costLine.AlcCode))
					{
						empty += Function.GetJsonString("LandedCostCode", Function.RawText(costLine.AlcCode));
					}
					if (!string.IsNullOrEmpty(costLine.OhType))
					{
						string value = "";
						switch (costLine.OhType)
						{
						case "F":
							value = Function.ToString(LandedCostAllocationByEnum.asCashValueBeforeCustoms);
							break;
						case "C":
							value = Function.ToString(LandedCostAllocationByEnum.asCashValueAfterCustoms);
							break;
						case "Q":
							value = Function.ToString(LandedCostAllocationByEnum.asQuantity);
							break;
						case "W":
							value = Function.ToString(LandedCostAllocationByEnum.asWeight);
							break;
						case "V":
							value = Function.ToString(LandedCostAllocationByEnum.asVolume);
							break;
						case "A":
							value = Function.ToString(LandedCostAllocationByEnum.asEqual);
							break;
						}
						if (!string.IsNullOrEmpty(value))
						{
							empty += Function.GetJsonString("AllocationBy", value);
						}
					}
					if (Math.Round(Function.ParseDecimal(costLine.CostSum), 6) != 0m)
					{
						empty += Function.GetJsonString("Amount", Function.ParseDecimal(costLine.CostSum));
					}
					if (Math.Round(Function.ParseDecimal(costLine.CostSumFC), 6) != 0m)
					{
						empty += Function.GetJsonString("AmountFC", Function.ParseDecimal(costLine.CostSumFC));
					}
					if (Math.Round(Function.ParseDecimal(costLine.Factor), 6) != 0m)
					{
						empty += Function.GetJsonString("Factor", Function.ParseDecimal(costLine.Factor));
					}
					if (!string.IsNullOrEmpty(costLine.CostType))
					{
						empty += Function.GetJsonString("CostType", costLine.CostType.Equals("F") ? "asFixedCosts" : "asVariableCosts");
					}
					if (!string.IsNullOrEmpty(costLine.InCustCalc))
					{
						empty += Function.GetJsonString("IncludeForCustoms", costLine.CostType.Equals("Y") ? "tYES" : "tNO");
					}
					if (Math.Round(Function.ParseDecimal(costLine.OpenCost), 6) != 0m)
					{
						empty += Function.GetJsonString("OpenAmount", Function.ParseDecimal(costLine.OpenCost));
					}
					if (Math.Round(Function.ParseDecimal(costLine.OpenCostFC), 6) != 0m)
					{
						empty += Function.GetJsonString("OpenAmountFC", Function.ParseDecimal(costLine.OpenCostFC));
					}
					if (!string.IsNullOrEmpty(costLine.AgentCode))
					{
						empty += Function.GetJsonString("Broker", costLine.AgentCode);
					}
					if (!string.IsNullOrEmpty(costLine.AgentName))
					{
						empty += Function.GetJsonString("BrokerName", costLine.AgentName);
					}
					empty += DataService.MappingUdf(costLine.UserFields, commandType);
					empty += "},";
				}
				empty += "]";
			}
			return empty + "}";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return string.Empty;
		}
	}

	public override HttpResult Save(LandedCostImpl doc, string sessionId)
	{
		try
		{
			string empty = string.Empty;
			ApzObjectTypes objectTypes = ApzObjectTypes.oPurchaseLandedCost;
			bool flag = Exists(doc.DocEntry, sessionId);
			HttpResult httpResult = new HttpResult();
			if (!flag)
			{
				empty = MappingService(doc, EnumCommandType.Add);
				httpResult = DataService.AddObject(objectTypes, sessionId, empty);
			}
			else
			{
				empty = MappingService(doc, EnumCommandType.Update);
				httpResult = DataService.UpdateObject(objectTypes, sessionId, empty);
			}
			return httpResult;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected override bool Exists(int docEntry, string sessionId)
	{
		try
		{
			string query = "select 1 from OIPF where \"DocEntry\" = @docEntry;";
			return ExecuteDataTable(query, CommandType.Text, new SqlParameter("@docEntry", docEntry)).IsNotNull();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return false;
		}
	}
}
