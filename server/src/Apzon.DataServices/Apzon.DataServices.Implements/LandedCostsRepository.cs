using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.DataServices.Interfaces;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.LandedCosts;
using Apzon.Services;
using SAPbobsCOM;

namespace Apzon.DataServices.Implements;

internal class LandedCostsRepository : Repository<DataTable>, ILandedCosts, IDisposable
{
	private IUnitOfWork _unitOfWork;

	public LandedCostsRepository(Apzon.Services.IUnitOfWork serviceUOW, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUOW, dataService)
	{
		_unitOfWork = unitOfWork;
	}

	public void Dispose()
	{
	}

	protected virtual int GetMaxDoc()
	{
		try
		{
			string query = "select ISNULL(MAX(DocEntry), 0) from OPDN";
			return Function.ParseInt(ExecuteDataTable(query, CommandType.Text).Rows[0][0]);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return -1;
		}
	}

	public virtual HttpResult Save(LandedCostImpl doc, string sessionId)
	{
		try
		{
			string text = "";
			bool flag = Exists(doc.DocEntry, sessionId);
			text = (flag ? ("<UpdateLandedCost  xmlns=\"LandedCostsService\">" + MappingService(doc, EnumCommandType.Update) + "</UpdateLandedCost>") : ("<AddLandedCost  xmlns=\"LandedCostsService\">" + MappingService(doc, EnumCommandType.Add) + "</AddLandedCost>"));
			string xmlObject = "<?xml version=\"1.0\" ?>  \r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>LandedCostsService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>" + Function.ToString(text) + "\r\n                                </soap:Body>  \r\n                            </soap:Envelope> ";
			XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					return new HttpResult(Enums.MessageCode.Error, xElement2.Value);
				}
			}
			else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("LandedCostNumber"))
			{
				XElement xElement3 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("LandedCostNumber"));
				if (xElement3 != null)
				{
					string text2 = Function.ToString(xElement3.Value);
					if (flag)
					{
						Logging.Write("WATCH", $"Update Landed Costs - {text2}");
					}
					else
					{
						Logging.Write("WATCH", $"Add new Landed Costs - {text2}");
					}
					return new HttpResult(Enums.MessageCode.Success, "", text2);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("WATCH", ex);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new HttpResult();
	}

	public virtual string MappingService(LandedCostImpl doc, EnumCommandType commandType)
	{
		string text = "";
		try
		{
			text += "<LandedCost>";
			if (commandType == EnumCommandType.Update)
			{
				text = text + "<DocEntry>" + doc.DocEntry + "</DocEntry>";
			}
			if (doc.DocNum > 0)
			{
				text = text + "<LandedCostNumber>" + doc.DocNum + "</LandedCostNumber>";
			}
			if (doc.DocDate > new DateTime(1900, 1, 1))
			{
				text = text + "<PostingDate>" + doc.DocDate.ToString("yyyy-MM-dd") + "</PostingDate>";
			}
			if (doc.DocDueDate > new DateTime(1900, 1, 1))
			{
				text = text + "<DueDate>" + doc.DocDueDate.ToString("yyyy-MM-dd") + "</DueDate>";
			}
			if (!string.IsNullOrEmpty(doc.CardCode))
			{
				text = text + "<VendorCode>" + Function.RawText(doc.CardCode) + "</VendorCode>";
			}
			if (!string.IsNullOrEmpty(doc.SuppName))
			{
				text = text + "<VendorName>" + Function.RawText(doc.SuppName) + "</VendorName>";
			}
			if (!string.IsNullOrEmpty(doc.AgentCode))
			{
				text = text + "<Broker>" + Function.RawText(doc.AgentCode) + "</Broker>";
			}
			if (!string.IsNullOrEmpty(doc.AgentName))
			{
				text = text + "<BrokerName>" + Function.RawText(doc.AgentName) + "</BrokerName>";
			}
			if (!string.IsNullOrEmpty(doc.Descr))
			{
				text = text + "<Remarks>" + Function.RawText(doc.Descr) + "</Remarks>";
			}
			if (!string.IsNullOrEmpty(doc.Ref1))
			{
				text = text + "<Reference>" + Function.RawText(doc.Ref1) + "</Reference>";
			}
			if (!string.IsNullOrEmpty(doc.DocCur))
			{
				text = text + "<DocumentCurrency>" + Function.RawText(doc.DocCur) + "</DocumentCurrency>";
			}
			if (Math.Round(Function.ParseDecimal(doc.DocRate), 6) != 0m)
			{
				text = text + "<DocumentRate>" + Function.ParseDecimal(doc.DocRate) + "</DocumentRate>";
			}
			if (Math.Round(Function.ParseDecimal(doc.ExpCustom), 6) != 0m)
			{
				text = text + "<ProjectedCustoms>" + Function.ParseDecimal(doc.ExpCustom) + "</ProjectedCustoms>";
			}
			if (Math.Round(Function.ParseDecimal(doc.ActCustom), 6) != 0m)
			{
				text = text + "<ActualCustoms>" + Function.ParseDecimal(doc.ActCustom) + "</ActualCustoms>";
			}
			if (Math.Round(Function.ParseDecimal(doc.AcCustomFC), 6) != 0m)
			{
				text = text + "<ActualCustomsFC>" + Function.ParseDecimal(doc.AcCustomFC) + "</ActualCustomsFC>";
			}
			if (Math.Round(Function.ParseDecimal(doc.Vat1), 6) != 0m)
			{
				text = text + "<Tax1>" + Function.ParseDecimal(doc.Vat1) + "</Tax1>";
			}
			if (Math.Round(Function.ParseDecimal(doc.Vat2), 6) != 0m)
			{
				text = text + "<Tax2>" + Function.ParseDecimal(doc.Vat2) + "</Tax2>";
			}
			if (Math.Round(Function.ParseDecimal(doc.BeforeVat), 6) != 0m)
			{
				text = text + "<BeforeTax>" + Function.ParseDecimal(doc.BeforeVat) + "</BeforeTax>";
			}
			if (Math.Round(Function.ParseDecimal(doc.DocTotal), 6) != 0m)
			{
				text = text + "<Total>" + Function.ParseDecimal(doc.DocTotal) + "</Total>";
			}
			if (Math.Round(Function.ParseDecimal(doc.CostSum), 6) != 0m)
			{
				text = text + "<TotalFreightCharges>" + Function.ParseDecimal(doc.CostSum) + "</TotalFreightCharges>";
			}
			if (Math.Round(Function.ParseDecimal(doc.ExCustomFC), 6) != 0m)
			{
				text = text + "<ProjectedCustomsFC>" + Function.ParseDecimal(doc.ExCustomFC) + "</ProjectedCustomsFC>";
			}
			if (Math.Round(Function.ParseDecimal(doc.Vat1FC), 6) != 0m)
			{
				text = text + "<Tax1FC>" + Function.ParseDecimal(doc.Vat1FC) + "</Tax1FC>";
			}
			if (Math.Round(Function.ParseDecimal(doc.Vat2FC), 6) != 0m)
			{
				text = text + "<Tax2FC>" + Function.ParseDecimal(doc.Vat2FC) + "</Tax2FC>";
			}
			if (Math.Round(Function.ParseDecimal(doc.BeforVatFC), 6) != 0m)
			{
				text = text + "<BeforeTaxFC>" + Function.ParseDecimal(doc.BeforVatFC) + "</BeforeTaxFC>";
			}
			if (Math.Round(Function.ParseDecimal(doc.DocTotalFC), 6) != 0m)
			{
				text = text + "<TotalFC>" + Function.ParseDecimal(doc.DocTotalFC) + "</TotalFC>";
			}
			if (Math.Round(Function.ParseDecimal(doc.CostSumFC), 6) != 0m)
			{
				text = text + "<TotalFreightChargesFC>" + Function.ParseDecimal(doc.CostSumFC) + "</TotalFreightChargesFC>";
			}
			if (doc.Series > 0)
			{
				text = text + "<Series>" + doc.Series + "</Series>";
			}
			if (!string.IsNullOrEmpty(doc.incCustom))
			{
				string incCustom = doc.incCustom;
				if (!(incCustom == "Y"))
				{
					if (incCustom == "N")
					{
						text += "<CustomsAffectsInventory>tNO</CustomsAffectsInventory>";
					}
				}
				else
				{
					text += "<CustomsAffectsInventory>tYES</CustomsAffectsInventory>";
				}
			}
			if (Math.Round(Function.ParseDecimal(doc.Cost_Match), 6) != 0m)
			{
				text = text + "<AmountToBalance>" + Function.ParseDecimal(doc.Cost_Match) + "</AmountToBalance>";
			}
			if (Math.Round(Function.ParseDecimal(doc.C_Match_FC), 6) != 0m)
			{
				text = text + "<AmountToBalanceFC>" + Function.ParseDecimal(doc.C_Match_FC) + "</AmountToBalanceFC>";
			}
			if (!string.IsNullOrEmpty(doc.BillOfLad))
			{
				text = text + "<BillofLadingNumber>" + Function.RawText(doc.BillOfLad) + "</BillofLadingNumber>";
			}
			if (doc.TrnspCode > 0)
			{
				text = text + "<TransportType>" + doc.TrnspCode + "</TransportType>";
			}
			if (doc.JdtNum > 0)
			{
				text = text + "<TransactionNumber>" + doc.JdtNum + "</TransactionNumber>";
			}
			if (!string.IsNullOrEmpty(doc.JdtMemo))
			{
				text = text + "<JournalRemarks>" + Function.RawText(doc.JdtMemo) + "</JournalRemarks>";
			}
			if (doc.UserFields != null && doc.UserFields.Count > 0)
			{
				text += DataService.MappingUdf(doc.UserFields, commandType);
			}
			if (doc.ItemLines != null && doc.ItemLines.Count > 0)
			{
				text += "<LandedCost_ItemLines>";
				foreach (LandedCost_ItemLineImpl itemLine in doc.ItemLines)
				{
					text += "<LandedCost_ItemLine>";
					if (itemLine.LineNum >= 0)
					{
						text = text + "<LineNumber>" + itemLine.LineNum + "</LineNumber>";
					}
					if (itemLine.BaseType >= 0)
					{
						text = text + "<BaseDocumentType>" + ((LandedCostBaseDocumentTypeEnum)itemLine.BaseType/*cast due to constrained. prefix*/).ToString() + "</BaseDocumentType>";
					}
					if (itemLine.BaseEntry > 0)
					{
						text = text + "<BaseEntry>" + itemLine.BaseEntry + "</BaseEntry>";
					}
					if (!string.IsNullOrEmpty(itemLine.ItemCode))
					{
						text = text + "<Number>" + Function.RawText(itemLine.ItemCode) + "</Number>";
					}
					if (!string.IsNullOrEmpty(itemLine.Dscription))
					{
						text = text + "<ItemDescription>" + Function.RawText(itemLine.Dscription) + "</ItemDescription>";
					}
					if (Math.Round(Function.ParseDecimal(itemLine.Quantity), 6) != 0m)
					{
						text = text + "<Quantity>" + Function.ParseDecimal(itemLine.Quantity) + "</Quantity>";
					}
					if (Math.Round(Function.ParseDecimal(itemLine.PriceFOB), 6) != 0m)
					{
						text = text + "<BaseDocumentPrice>" + Function.ParseDecimal(itemLine.PriceFOB) + "</BaseDocumentPrice>";
					}
					if (Math.Round(Function.ParseDecimal(itemLine.Rate), 6) != 0m)
					{
						text = text + "<Rate>" + Function.ParseDecimal(itemLine.Rate) + "</Rate>";
					}
					if (Math.Round(Function.ParseDecimal(itemLine.Custom), 6) != 0m)
					{
						text = text + "<ProjectedCustoms>" + Function.ParseDecimal(itemLine.Custom) + "</ProjectedCustoms>";
					}
					if (Math.Round(Function.ParseDecimal(itemLine.CustomFC), 6) != 0m)
					{
						text = text + "<ProjectedCustomsFC>" + Function.ParseDecimal(itemLine.CustomFC) + "</ProjectedCustomsFC>";
					}
					if (Math.Round(Function.ParseDecimal(itemLine.Cost), 6) != 0m)
					{
						text = text + "<ExpenditureFC>" + Function.ParseDecimal(itemLine.Cost) + "</ExpenditureFC>";
					}
					if (Math.Round(Function.ParseDecimal(itemLine.PriceAtWH), 6) != 0m)
					{
						text = text + "<WarehousePrice>" + Function.ParseDecimal(itemLine.PriceAtWH) + "</WarehousePrice>";
					}
					if (Math.Round(Function.ParseDecimal(itemLine.PricAtWHFC), 6) != 0m)
					{
						text = text + "<WarehousePriceFC>" + Function.ParseDecimal(itemLine.PricAtWHFC) + "</WarehousePriceFC>";
					}
					if (Math.Round(Function.ParseDecimal(itemLine.LineTotal), 6) != 0m)
					{
						text = text + "<LineTotal>" + Function.ParseDecimal(itemLine.LineTotal) + "</LineTotal>";
					}
					if (Math.Round(Function.ParseDecimal(itemLine.TotalFrgn), 6) != 0m)
					{
						text = text + "<LineTotalFC>" + Function.ParseDecimal(itemLine.TotalFrgn) + "</LineTotalFC>";
					}
					if (Math.Round(Function.ParseDecimal(itemLine.Volume), 6) != 0m)
					{
						text = text + "<Volume>" + Function.ParseDecimal(itemLine.Volume) + "</Volume>";
					}
					if (itemLine.UnitCode != 0)
					{
						text = text + "<VolumeUoM>" + itemLine.UnitCode + "</VolumeUoM>";
					}
					if (Math.Round(Function.ParseDecimal(itemLine.Weight1), 6) != 0m)
					{
						text = text + "<Weight1>" + Function.ParseDecimal(itemLine.Weight1) + "</Weight1>";
					}
					if (itemLine.UnitCode1 != 0)
					{
						text = text + "<Weight1UnitCode>" + itemLine.UnitCode1 + "</Weight1UnitCode>";
					}
					if (Math.Round(Function.ParseDecimal(itemLine.Weight2), 6) != 0m)
					{
						text = text + "<Weight2>" + Function.ParseDecimal(itemLine.Weight2) + "</Weight2>";
					}
					if (itemLine.UnitCode2 != 0)
					{
						text = text + "<Weight2UnitCode>" + itemLine.UnitCode2 + "</Weight2UnitCode>";
					}
					if (!string.IsNullOrEmpty(itemLine.CardCode))
					{
						text = text + "<VendorCode>" + Function.RawText(itemLine.CardCode) + "</VendorCode>";
					}
					if (!string.IsNullOrEmpty(itemLine.Reference))
					{
						text = text + "<Reference>" + Function.RawText(itemLine.Reference) + "</Reference>";
					}
					if (Math.Round(Function.ParseDecimal(itemLine.FactNoCust), 6) != 0m)
					{
						text = text + "<FactorWithoutCustoms>" + Function.ParseDecimal(itemLine.FactNoCust) + "</FactorWithoutCustoms>";
					}
					if (Math.Round(Function.ParseDecimal(itemLine.FacWthCust), 6) != 0m)
					{
						text = text + "<FactorWithCustoms>" + Function.ParseDecimal(itemLine.FacWthCust) + "</FactorWithCustoms>";
					}
					if (!string.IsNullOrEmpty(itemLine.UseBaseUn))
					{
						text = text + "<InventoryUoM>" + Function.RawText(itemLine.UseBaseUn) + "</InventoryUoM>";
					}
					if (!string.IsNullOrEmpty(itemLine.BlockNum))
					{
						text = text + "<BlockNumber>" + Function.RawText(itemLine.BlockNum) + "</BlockNumber>";
					}
					if (!string.IsNullOrEmpty(itemLine.ImportLog))
					{
						text = text + "<ImportLog>" + Function.RawText(itemLine.ImportLog) + "</ImportLog>";
					}
					if (!string.IsNullOrEmpty(itemLine.OrigWhs))
					{
						text = text + "<OriginalWarehouse>" + Function.RawText(itemLine.OrigWhs) + "</OriginalWarehouse>";
					}
					if (!string.IsNullOrEmpty(itemLine.WhsCode))
					{
						text = text + "<Warehouse>" + Function.RawText(itemLine.WhsCode) + "</Warehouse>";
					}
					if (itemLine.ReleaseNum > 0)
					{
						text = text + "<ReleaseNumber>" + itemLine.ReleaseNum + "</ReleaseNumber>";
					}
					if (Math.Round(Function.ParseDecimal(itemLine.VarCosts), 6) != 0m)
					{
						text = text + "<VariantCosts>" + Function.ParseDecimal(itemLine.VarCosts) + "</VariantCosts>";
					}
					if (Math.Round(Function.ParseDecimal(itemLine.ConstCosts), 6) != 0m)
					{
						text = text + "<FixCosts>" + Function.ParseDecimal(itemLine.ConstCosts) + "</FixCosts>";
					}
					if (Math.Round(Function.ParseDecimal(itemLine.VarCostsFR), 6) != 0m)
					{
						text = text + "<VariantCostsFC>" + Function.ParseDecimal(itemLine.VarCostsFR) + "</VariantCostsFC>";
					}
					if (Math.Round(Function.ParseDecimal(itemLine.CnstCostFR), 6) != 0m)
					{
						text = text + "<FixCostsFC>" + Function.ParseDecimal(itemLine.CnstCostFR) + "</FixCostsFC>";
					}
					if (Math.Round(Function.ParseDecimal(itemLine.UserCustom), 6) != 0m)
					{
						text = text + "<Customs>" + Function.ParseDecimal(itemLine.UserCustom) + "</Customs>";
					}
					if (Math.Round(Function.ParseDecimal(itemLine.UsrCusomFC), 6) != 0m)
					{
						text = text + "<CustomsFC>" + Function.ParseDecimal(itemLine.UsrCusomFC) + "</CustomsFC>";
					}
					if (Math.Round(Function.ParseDecimal(itemLine.FobValue), 6) != 0m)
					{
						text = text + "<BaseDocumentValueLineTotal>" + Function.ParseDecimal(itemLine.FobValue) + "</BaseDocumentValueLineTotal>";
					}
					if (Math.Round(Function.ParseDecimal(itemLine.FobValueFC), 6) != 0m)
					{
						text = text + "<BaseDocumentValueLineTotalFC>" + Function.ParseDecimal(itemLine.FobValueFC) + "</BaseDocumentValueLineTotalFC>";
					}
					if (Math.Round(Function.ParseDecimal(itemLine.TtlExpndLC), 6) != 0m)
					{
						text = text + "<AllocatedUnitCostsLineTotal>" + Function.ParseDecimal(itemLine.TtlExpndLC) + "</AllocatedUnitCostsLineTotal>";
					}
					if (Math.Round(Function.ParseDecimal(itemLine.TtlExpndFC), 6) != 0m)
					{
						text = text + "<AllocatedUnitCostsLineTotalFC>" + Function.ParseDecimal(itemLine.TtlExpndFC) + "</AllocatedUnitCostsLineTotalFC>";
					}
					if (Math.Round(Function.ParseDecimal(itemLine.TtlCustLC), 6) != 0m)
					{
						text = text + "<CustomsValue>" + Function.ParseDecimal(itemLine.TtlCustLC) + "</CustomsValue>";
					}
					if (Math.Round(Function.ParseDecimal(itemLine.TtlCustFC), 6) != 0m)
					{
						text = text + "<CustomsValueFC>" + Function.ParseDecimal(itemLine.TtlCustFC) + "</CustomsValueFC>";
					}
					if (Math.Round(Function.ParseDecimal(itemLine.TtlCostLC), 6) != 0m)
					{
						text = text + "<TotalCosts>" + Function.ParseDecimal(itemLine.TtlCostLC) + "</TotalCosts>";
					}
					if (Math.Round(Function.ParseDecimal(itemLine.TtlVolume), 6) != 0m)
					{
						text = text + "<TotalVolume>" + Function.ParseDecimal(itemLine.TtlVolume) + "</TotalVolume>";
					}
					if (itemLine.OrigLine != 0)
					{
						text = text + "<BaseLine>" + itemLine.OrigLine + "</BaseLine>";
					}
					if (Math.Round(Function.ParseDecimal(itemLine.TtlCustSC), 6) != 0m)
					{
						text = text + "<TotalLineProjectedCustoms>" + Function.ParseDecimal(itemLine.TtlCustSC) + "</TotalLineProjectedCustoms>";
					}
					if (Math.Round(Function.ParseDecimal(itemLine.TtlExpndSC), 6) != 0m)
					{
						text = text + "<AllocatedCostsLineTotal>" + Function.ParseDecimal(itemLine.TtlExpndSC) + "</AllocatedCostsLineTotal>";
					}
					if (Math.Round(Function.ParseDecimal(itemLine.FobnLaC), 6) != 0m)
					{
						text = text + "<FOBandIncludedCosts>" + Function.ParseDecimal(itemLine.FobnLaC) + "</FOBandIncludedCosts>";
					}
					if (Math.Round(Function.ParseDecimal(itemLine.FobnLaCFC), 6) != 0m)
					{
						text = text + "<FOBandIncludedCostsFC>" + Function.ParseDecimal(itemLine.FobnLaCFC) + "</FOBandIncludedCostsFC>";
					}
					if (!string.IsNullOrEmpty(itemLine.Project))
					{
						text = text + "<Project>" + Function.RawText(itemLine.Project) + "</Project>";
					}
					if (!string.IsNullOrEmpty(itemLine.OcrCode))
					{
						text = text + "<DistributionRule>" + Function.RawText(itemLine.OcrCode) + "</DistributionRule>";
					}
					if (!string.IsNullOrEmpty(itemLine.OcrCode2))
					{
						text = text + "<DistributionRule2>" + Function.RawText(itemLine.OcrCode2) + "</DistributionRule2>";
					}
					if (!string.IsNullOrEmpty(itemLine.OcrCode3))
					{
						text = text + "<DistributionRule3>" + Function.RawText(itemLine.OcrCode3) + "</DistributionRule3>";
					}
					if (!string.IsNullOrEmpty(itemLine.OcrCode4))
					{
						text = text + "<DistributionRule4>" + Function.RawText(itemLine.OcrCode4) + "</DistributionRule4>";
					}
					if (!string.IsNullOrEmpty(itemLine.OcrCode5))
					{
						text = text + "<DistributionRule5>" + Function.RawText(itemLine.OcrCode5) + "</DistributionRule5>";
					}
					if (itemLine.PriceList != 0)
					{
						text = text + "<PriceList>" + itemLine.PriceList + "</PriceList>";
					}
					if (!string.IsNullOrEmpty(itemLine.CostOH))
					{
						string incCustom = itemLine.CostOH;
						if (!(incCustom == "Y"))
						{
							if (incCustom == "N")
							{
								text += "<AutomaticExpenditure>tNO</AutomaticExpenditure>";
							}
						}
						else
						{
							text += "<AutomaticExpenditure>tYES</AutomaticExpenditure>";
						}
					}
					if (!string.IsNullOrEmpty(itemLine.StockEval))
					{
						string incCustom = itemLine.StockEval;
						if (!(incCustom == "Y"))
						{
							if (incCustom == "N")
							{
								text += "<InventoryValuation>tNO</InventoryValuation>";
							}
						}
						else
						{
							text += "<InventoryValuation>tYES</InventoryValuation>";
						}
					}
					if (itemLine.OrigRow != 0)
					{
						text = text + "<OriginLine>" + itemLine.OrigRow + "</OriginLine>";
					}
					if (!string.IsNullOrEmpty(itemLine.Currency))
					{
						text = text + "<Currency>" + Function.RawText(itemLine.Currency) + "</Currency>";
					}
					if (Math.Round(Function.ParseDecimal(itemLine.CustRate), 6) != 0m)
					{
						text = text + "<CustomsGroupRate>" + Function.ParseDecimal(itemLine.CustRate) + "</CustomsGroupRate>";
					}
					if (!string.IsNullOrEmpty(itemLine.VatGroup))
					{
						text = text + "<VatGroup>" + Function.RawText(itemLine.VatGroup) + "</VatGroup>";
					}
					if (Math.Round(Function.ParseDecimal(itemLine.VatPrcnt), 6) != 0m)
					{
						text = text + "<VatPercent>" + Function.ParseDecimal(itemLine.VatPrcnt) + "</VatPercent>";
					}
					if (Math.Round(Function.ParseDecimal(itemLine.ExciseSum), 6) != 0m)
					{
						text = text + "<ExciseSum>" + Function.ParseDecimal(itemLine.ExciseSum) + "</ExciseSum>";
					}
					if (Math.Round(Function.ParseDecimal(itemLine.ExcisSumFC), 6) != 0m)
					{
						text = text + "<ExciseSumFC>" + Function.ParseDecimal(itemLine.ExcisSumFC) + "</ExciseSumFC>";
					}
					if (!string.IsNullOrEmpty(itemLine.ExcInStk))
					{
						string incCustom = itemLine.ExcInStk;
						if (!(incCustom == "Y"))
						{
							if (incCustom == "N")
							{
								text += "<ExciseAffectStock>tNO</ExciseAffectStock>";
							}
						}
						else
						{
							text += "<ExciseAffectStock>tYES</ExciseAffectStock>";
						}
					}
					if (Math.Round(Function.ParseDecimal(itemLine.CustomSum), 6) != 0m)
					{
						text = text + "<CustomsCost>" + Function.ParseDecimal(itemLine.CustomSum) + "</CustomsCost>";
					}
					if (Math.Round(Function.ParseDecimal(itemLine.CustSumFC), 6) != 0m)
					{
						text = text + "<CustomsCostFC>" + Function.ParseDecimal(itemLine.CustSumFC) + "</CustomsCostFC>";
					}
					if (!string.IsNullOrEmpty(itemLine.CstmInStk))
					{
						string incCustom = itemLine.CstmInStk;
						if (!(incCustom == "Y"))
						{
							if (incCustom == "N")
							{
								text += "<CustomsAffectStock>tNO</CustomsAffectStock>";
							}
						}
						else
						{
							text += "<CustomsAffectStock>tYES</CustomsAffectStock>";
						}
					}
					if (Math.Round(Function.ParseDecimal(itemLine.CstmVatSum), 6) != 0m)
					{
						text = text + "<CustomsVat>" + Function.ParseDecimal(itemLine.CstmVatSum) + "</CustomsVat>";
					}
					if (Math.Round(Function.ParseDecimal(itemLine.CstmVatFC), 6) != 0m)
					{
						text = text + "<CustomsVatFC>" + Function.ParseDecimal(itemLine.CstmVatFC) + "</CustomsVatFC>";
					}
					if (!string.IsNullOrEmpty(itemLine.CstmVatStk))
					{
						string incCustom = itemLine.CstmVatStk;
						if (!(incCustom == "Y"))
						{
							if (incCustom == "N")
							{
								text += "<CustomsVatAffectStock>tNO</CustomsVatAffectStock>";
							}
						}
						else
						{
							text += "<CustomsVatAffectStock>tYES</CustomsVatAffectStock>";
						}
					}
					if (!string.IsNullOrEmpty(itemLine.CCDNumber))
					{
						text = text + "<CCDNumber>" + Function.RawText(itemLine.CCDNumber) + "</CCDNumber>";
					}
					if (Math.Round(Function.ParseDecimal(itemLine.FobVal2), 6) != 0m)
					{
						text = text + "<CorrectedBaseDocumentValue>" + Function.ParseDecimal(itemLine.FobVal2) + "</CorrectedBaseDocumentValue>";
					}
					if (Math.Round(Function.ParseDecimal(itemLine.FobVal2FC), 6) != 0m)
					{
						text = text + "<CorrectedBaseDocumentValueFC>" + Function.ParseDecimal(itemLine.FobVal2FC) + "</CorrectedBaseDocumentValueFC>";
					}
					text += DataService.MappingUdf(itemLine.UserFields, commandType);
					text += "</LandedCost_ItemLine>";
				}
				text += "</LandedCost_ItemLines>";
			}
			if (doc.CostLines != null && doc.CostLines.Count > 0)
			{
				text += "<LandedCost_CostLines>";
				foreach (LandedCost_CostLineImpl costLine in doc.CostLines)
				{
					text += "<LandedCost_CostLine>";
					if (!string.IsNullOrEmpty(costLine.AlcCode))
					{
						text = text + "<LandedCostCode>" + Function.RawText(costLine.AlcCode) + "</LandedCostCode>";
					}
					if (!string.IsNullOrEmpty(costLine.OhType))
					{
						string text2 = "";
						switch (costLine.OhType)
						{
						case "F":
							text2 = LandedCostAllocationByEnum.asCashValueBeforeCustoms.ToString();
							break;
						case "C":
							text2 = LandedCostAllocationByEnum.asCashValueAfterCustoms.ToString();
							break;
						case "Q":
							text2 = LandedCostAllocationByEnum.asQuantity.ToString();
							break;
						case "W":
							text2 = LandedCostAllocationByEnum.asWeight.ToString();
							break;
						case "V":
							text2 = LandedCostAllocationByEnum.asVolume.ToString();
							break;
						case "A":
							text2 = LandedCostAllocationByEnum.asEqual.ToString();
							break;
						}
						if (!string.IsNullOrEmpty(text2))
						{
							text = text + "<AllocationBy>" + text2 + "</AllocationBy>";
						}
					}
					if (Math.Round(Function.ParseDecimal(costLine.CostSum), 6) != 0m)
					{
						text = text + "<Amount>" + Function.ParseDecimal(costLine.CostSum) + "</Amount>";
					}
					if (Math.Round(Function.ParseDecimal(costLine.CostSumFC), 6) != 0m)
					{
						text = text + "<AmountFC>" + Function.ParseDecimal(costLine.CostSumFC) + "</AmountFC>";
					}
					if (Math.Round(Function.ParseDecimal(costLine.Factor), 6) != 0m)
					{
						text = text + "<Factor>" + Function.ParseDecimal(costLine.Factor) + "</Factor>";
					}
					if (!string.IsNullOrEmpty(costLine.CostType))
					{
						string incCustom = costLine.CostType;
						if (!(incCustom == "F"))
						{
							if (incCustom == "V")
							{
								text += "<CostType>asVariableCosts</CostType>";
							}
						}
						else
						{
							text += "<CostType>asFixedCosts</CostType>";
						}
					}
					if (!string.IsNullOrEmpty(costLine.InCustCalc))
					{
						string incCustom = costLine.InCustCalc;
						if (!(incCustom == "Y"))
						{
							if (incCustom == "N")
							{
								text += "<IncludeForCustoms>tNO</IncludeForCustoms>";
							}
						}
						else
						{
							text += "<IncludeForCustoms>tYES</IncludeForCustoms>";
						}
					}
					if (Math.Round(Function.ParseDecimal(costLine.OpenCost), 6) != 0m)
					{
						text = text + "<OpenAmount>" + Function.ParseDecimal(costLine.OpenCost) + "</OpenAmount>";
					}
					if (Math.Round(Function.ParseDecimal(costLine.OpenCostFC), 6) != 0m)
					{
						text = text + "<OpenAmountFC>" + Function.ParseDecimal(costLine.OpenCostFC) + "</OpenAmountFC>";
					}
					if (!string.IsNullOrEmpty(costLine.AgentCode))
					{
						text = text + "<Broker>" + Function.RawText(costLine.AgentCode) + "</Broker>";
					}
					if (!string.IsNullOrEmpty(costLine.AgentName))
					{
						text = text + "<BrokerName>" + Function.RawText(costLine.AgentName) + "</BrokerName>";
					}
					text += DataService.MappingUdf(costLine.UserFields, commandType);
					text += "</LandedCost_CostLine>";
				}
				text += "</LandedCost_CostLines>";
			}
			return text + "</LandedCost>";
		}
		catch (Exception ex)
		{
			Logging.Write("WATCH", ex);
			return "";
		}
	}

	protected virtual bool Exists(int docEntry, string sessionId)
	{
		if (docEntry <= 0)
		{
			return false;
		}
		string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?>\r\n                                    <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>LandedCostsService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>\r\n                                    <GetLandedCost xmlns=\"LandedCostsService\">\r\n                                            <LandedCostParams>   \r\n                                                <LandedCostNumber>" + docEntry + "</LandedCostNumber>\r\n                                            </LandedCostParams>\r\n                                    </GetLandedCost>  \r\n                                </soap:Body>  \r\n                            </soap:Envelope>  ";
		XmlElement xmlElement = DataService.GetByKey(sessionId, xmlObject).Content as XmlElement;
		if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
		{
			return false;
		}
		if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("DocEntry"))
		{
			return true;
		}
		return false;
	}

	public HttpResult Close(int docEntry, string sessionId)
	{
		if (docEntry <= 0)
		{
			return new HttpResult(Enums.MessageCode.Error, "No matching record found");
		}
		string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?>\r\n                                    <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>LandedCostsService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>\r\n                                    <CloseLandedCost xmlns=\"LandedCostsService\">\r\n                                            <LandedCostParams>   \r\n                                                <LandedCostNumber>" + docEntry + "</LandedCostNumber>\r\n                                            </LandedCostParams>\r\n                                    </CloseLandedCost>  \r\n                                </soap:Body>  \r\n                            </soap:Envelope>  ";
		XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
		XElement xElement = XElement.Parse(xmlElement.OuterXml);
		if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
		{
			XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
			if (xElement2 != null)
			{
				return new HttpResult(Enums.MessageCode.Error, xElement2.Value);
			}
		}
		else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("LandedCostNumber"))
		{
			XElement xElement3 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("LandedCostNumber"));
			if (xElement3 != null)
			{
				string obj = Function.ToString(xElement3.Value);
				return new HttpResult(Enums.MessageCode.Success, "", obj);
			}
		}
		return new HttpResult();
	}
}
