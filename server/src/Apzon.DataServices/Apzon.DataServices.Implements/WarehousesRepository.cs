using System;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.DataServices.Interfaces;
using Apzon.Entities.Models.MasterData;
using Apzon.Services;
using SAPbobsCOM;

namespace Apzon.DataServices.Implements;

public class WarehousesRepository : Repository<WarehouseImpl>, Apzon.DataServices.Interfaces.IWarehouses, IRepository<WarehouseImpl>, IDisposable
{
	protected readonly IUnitOfWork _unitOfWork;

	public WarehousesRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, dataService)
	{
		_unitOfWork = unitOfWork;
	}

	public void Dispose()
	{
	}

	public string Save(WarehouseImpl doc, string sessionId, Enums.TransactionType tranType)
	{
		try
		{
			if (tranType == Enums.TransactionType.A)
			{
				return AddDIS(doc, sessionId);
			}
			return UpdateDIS(doc, sessionId);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public string Remove(string itemCode, string sessionId)
	{
		throw new NotImplementedException();
	}

	public virtual string UpdateDIS(WarehouseImpl doc, string sessionId)
	{
		try
		{
			string text = "";
			string xmlObject = MappingDIS(doc, EnumCommandType.Update, sessionId);
			XmlElement xmlElement = DataService.UpdateObject(ApzObjectTypes.oWarehouses, sessionId, xmlObject).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					text = xElement2.Value;
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), text);
				}
			}
			else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
			{
				XElement xElement3 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("RetKey"));
				if (xElement3 != null)
				{
					int num = Function.ParseInt(xElement3.Value);
					Logging.Write("WATCH", $"update warehouse - {num}");
					text = "";
				}
			}
			return text;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public virtual string AddDIS(WarehouseImpl doc, string sessionId)
	{
		try
		{
			string xmlObject = MappingDIS(doc, EnumCommandType.Add, sessionId);
			XmlElement xmlElement = DataService.AddObject(ApzObjectTypes.oWarehouses, sessionId, xmlObject).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					Logging.Write("ERROR", "Add Warehouse error: " + xElement2.Value);
					return xElement2.Value;
				}
			}
			else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
			{
				XElement xElement3 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("RetKey"));
				if (xElement3 != null)
				{
					int num = Function.ParseInt(xElement3.Value);
					Logging.Write("WATCH", $"Add BP master - {num}");
					return string.Empty;
				}
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	protected virtual string MappingDIS(WarehouseImpl doc, EnumCommandType commandType, string sessionId)
	{
		string text = "";
		try
		{
			text = "<BOM>\r\n                          <BO>\r\n                          <AdmInfo>\r\n                            <Object>oWarehouses</Object>\r\n                          </AdmInfo>";
			if (commandType == EnumCommandType.Update)
			{
				text = text + "<QueryParams><WarehouseCode>" + Function.ToString(doc.WhsCode) + "</WarehouseCode></QueryParams>";
			}
			text += "<Warehouses>\r\n                            <row>";
			if (!string.IsNullOrEmpty(doc.WhsCode) && commandType == EnumCommandType.Add)
			{
				text = text + "<WarehouseCode>" + doc.WhsCode + "</WarehouseCode>";
			}
			if (!string.IsNullOrEmpty(doc.WhsName))
			{
				text = text + "<WarehouseName>" + Function.RawText(doc.WhsName) + "</WarehouseName>";
			}
			if (!string.IsNullOrEmpty(doc.BalInvntAc))
			{
				text = text + "<StockAccount>" + Function.RawText(doc.BalInvntAc) + "</StockAccount>";
			}
			if (!string.IsNullOrEmpty(doc.SaleCostAc))
			{
				text = text + "<CostOfGoodsSold>" + Function.RawText(doc.SaleCostAc) + "</CostOfGoodsSold>";
			}
			if (!string.IsNullOrEmpty(doc.TransferAc))
			{
				text = text + "<TransfersAcc>" + Function.RawText(doc.TransferAc) + "</TransfersAcc>";
			}
			if (!string.IsNullOrEmpty(doc.RevenuesAc))
			{
				text = text + "<RevenuesAccount>" + Function.RawText(doc.RevenuesAc) + "</RevenuesAccount>";
			}
			if (!string.IsNullOrEmpty(doc.VarianceAc))
			{
				text = text + "<VarianceAccount>" + Function.RawText(doc.VarianceAc) + "</VarianceAccount>";
			}
			if (!string.IsNullOrEmpty(doc.DecreasAc))
			{
				text = text + "<DecreasingAccount>" + Function.RawText(doc.DecreasAc) + "</DecreasingAccount>";
			}
			if (!string.IsNullOrEmpty(doc.IncresGlAc))
			{
				text = text + "<IncreasingAcc>" + Function.RawText(doc.IncresGlAc) + "</IncreasingAcc>";
			}
			if (!string.IsNullOrEmpty(doc.PAReturnAc))
			{
				text = text + "<ReturningAccount>" + Function.RawText(doc.PAReturnAc) + "</ReturningAccount>";
			}
			if (!string.IsNullOrEmpty(doc.ExpensesAc))
			{
				text = text + "<ExpenseAccount>" + Function.RawText(doc.ExpensesAc) + "</ExpenseAccount>";
			}
			if (!string.IsNullOrEmpty(doc.EURevenuAc))
			{
				text = text + "<EURevenuesAccount>" + Function.RawText(doc.EURevenuAc) + "</EURevenuesAccount>";
			}
			if (!string.IsNullOrEmpty(doc.ExpensesAc))
			{
				text = text + "<EUExpensesAccount>" + Function.RawText(doc.ExpensesAc) + "</EUExpensesAccount>";
			}
			if (!string.IsNullOrEmpty(doc.FrRevenuAc))
			{
				text = text + "<ForeignRevenuesAcc>" + Function.RawText(doc.FrRevenuAc) + "</ForeignRevenuesAcc>";
			}
			if (!string.IsNullOrEmpty(doc.FrExpensAc))
			{
				text = text + "<ForeignExpensesAccount>" + Function.RawText(doc.FrExpensAc) + "</ForeignExpensesAccount>";
			}
			if (!string.IsNullOrEmpty(doc.VatGroup))
			{
				text = text + "<TaxGroup>" + Function.RawText(doc.VatGroup) + "</TaxGroup>";
			}
			if (!string.IsNullOrEmpty(doc.Street))
			{
				text = text + "<Street>" + Function.RawText(doc.Street) + "</Street>";
			}
			if (!string.IsNullOrEmpty(doc.Block))
			{
				text = text + "<Block>" + Function.RawText(doc.Block) + "</Block>";
			}
			if (!string.IsNullOrEmpty(doc.ZipCode))
			{
				text = text + "<ZipCode>" + Function.RawText(doc.ZipCode) + "</ZipCode>";
			}
			if (!string.IsNullOrEmpty(doc.City))
			{
				text = text + "<City>" + Function.RawText(doc.City) + "</City>";
			}
			if (!string.IsNullOrEmpty(doc.County))
			{
				text = text + "<County>" + Function.RawText(doc.County) + "</County>";
			}
			if (!string.IsNullOrEmpty(doc.Country))
			{
				text = text + "<Country>" + Function.RawText(doc.Country) + "</Country>";
			}
			if (!string.IsNullOrEmpty(doc.State))
			{
				text = text + "<State>" + Function.RawText(doc.State) + "</State>";
			}
			if (Function.ParseInt(doc.Location) > 0)
			{
				text = text + "<Location>" + Function.ToString(doc.Location) + "</Location>";
			}
			if (!string.IsNullOrEmpty(doc.DropShip))
			{
				text = text + "<DropShip>" + Function.RawText(doc.DropShip) + "</DropShip>";
			}
			if (!string.IsNullOrEmpty(doc.ExmptIncom))
			{
				text = text + "<ExemptRevenuesAccount>" + Function.RawText(doc.ExmptIncom) + "</ExemptRevenuesAccount>";
			}
			if (!string.IsNullOrEmpty(doc.UseTax))
			{
				text = text + "<AllowUseTax>" + Function.RawText(doc.UseTax) + "</AllowUseTax>";
			}
			if (!string.IsNullOrEmpty(doc.PriceDifAc))
			{
				text = text + "<PriceDifferencesAccount>" + Function.RawText(doc.PriceDifAc) + "</PriceDifferencesAccount>";
			}
			if (!string.IsNullOrEmpty(doc.ExchangeAc))
			{
				text = text + "<ExchangeRateDifferencesAccount>" + Function.RawText(doc.ExchangeAc) + "</ExchangeRateDifferencesAccount>";
			}
			if (!string.IsNullOrEmpty(doc.BalanceAcc))
			{
				text = text + "<GoodsClearingAcc>" + Function.RawText(doc.BalanceAcc) + "</GoodsClearingAcc>";
			}
			if (!string.IsNullOrEmpty(doc.PurchaseAc))
			{
				text = text + "<PurchaseAccount>" + Function.RawText(doc.PurchaseAc) + "</PurchaseAccount>";
			}
			if (!string.IsNullOrEmpty(doc.PAReturnAc))
			{
				text = text + "<PurchaseReturningAccount>" + Function.RawText(doc.PAReturnAc) + "</PurchaseReturningAccount>";
			}
			if (!string.IsNullOrEmpty(doc.PurchOfsAc))
			{
				text = text + "<PurchaseOffsetAccount>" + Function.RawText(doc.PurchOfsAc) + "</PurchaseOffsetAccount>";
			}
			if (!string.IsNullOrEmpty(doc.FedTaxID))
			{
				text = text + "<FederalTaxID>" + Function.RawText(doc.FedTaxID) + "</FederalTaxID>";
			}
			if (!string.IsNullOrEmpty(doc.Building))
			{
				text = text + "<BuildingFloorRoom>" + Function.RawText(doc.Building) + "</BuildingFloorRoom>";
			}
			if (!string.IsNullOrEmpty(doc.ShpdGdsAct))
			{
				text = text + "<ShippedGoodsAccount>" + Function.RawText(doc.ShpdGdsAct) + "</ShippedGoodsAccount>";
			}
			if (!string.IsNullOrEmpty(doc.VatRevAct))
			{
				text = text + "<VATInRevenueAccount>" + Function.RawText(doc.VatRevAct) + "</VATInRevenueAccount>";
			}
			if (!string.IsNullOrEmpty(doc.DecresGlAc))
			{
				text = text + "<DecreaseGLAccount>" + Function.RawText(doc.DecresGlAc) + "</DecreaseGLAccount>";
			}
			if (!string.IsNullOrEmpty(doc.IncresGlAc))
			{
				text = text + "<IncreaseGLAccount>" + Function.RawText(doc.IncresGlAc) + "</IncreaseGLAccount>";
			}
			if (!string.IsNullOrEmpty(doc.Nettable))
			{
				BoYesNoEnum boYesNoEnum = ((doc.Nettable == "Y") ? BoYesNoEnum.tYES : BoYesNoEnum.tNO);
				text = text + "<Nettable>" + Function.RawText(boYesNoEnum) + "</Nettable>";
			}
			if (!string.IsNullOrEmpty(doc.IncreasAc))
			{
				text = text + "<StockInflationAdjustAccount>" + Function.RawText(doc.IncreasAc) + "</StockInflationAdjustAccount>";
			}
			if (!string.IsNullOrEmpty(doc.DecreasAc))
			{
				text = text + "<StockInflationOffsetAccount>" + Function.RawText(doc.DecreasAc) + "</StockInflationOffsetAccount>";
			}
			if (!string.IsNullOrEmpty(doc.WipAcct))
			{
				text = text + "<WIPMaterialAccount>" + Function.RawText(doc.WipAcct) + "</WIPMaterialAccount>";
			}
			if (!string.IsNullOrEmpty(doc.WipVarAcct))
			{
				text = text + "<WIPMaterialVarianceAccount>" + Function.RawText(doc.WipVarAcct) + "</WIPMaterialVarianceAccount>";
			}
			if (!string.IsNullOrEmpty(doc.CostRvlAct))
			{
				text = text + "<CostInflationAccount>" + Function.RawText(doc.CostRvlAct) + "</CostInflationAccount>";
			}
			if (!string.IsNullOrEmpty(doc.CstOffsAct))
			{
				text = text + "<CostInflationOffsetAccount>" + Function.RawText(doc.CstOffsAct) + "</CostInflationOffsetAccount>";
			}
			if (!string.IsNullOrEmpty(doc.ExpClrAct))
			{
				text = text + "<ExpensesClearingAccount>" + Function.RawText(doc.ExpClrAct) + "</ExpensesClearingAccount>";
			}
			if (!string.IsNullOrEmpty(doc.ExpOfstAct))
			{
				text = text + "<ExpenseOffsetingAct>" + Function.RawText(doc.ExpOfstAct) + "</ExpenseOffsetingAct>";
			}
			if (!string.IsNullOrEmpty(doc.ARCMEUAct))
			{
				text = text + "<SalesCreditAcc>" + Function.RawText(doc.ARCMEUAct) + "</SalesCreditAcc>";
			}
			if (!string.IsNullOrEmpty(doc.ARCMFrnAct))
			{
				text = text + "<SalesCreditForeignAcc>" + Function.RawText(doc.ARCMFrnAct) + "</SalesCreditForeignAcc>";
			}
			if (!string.IsNullOrEmpty(doc.ARCMEUAct))
			{
				text = text + "<SalesCreditEUAcc>" + Function.RawText(doc.ARCMEUAct) + "</SalesCreditEUAcc>";
			}
			if (!string.IsNullOrEmpty(doc.ARCMExpAct))
			{
				text = text + "<ExemptedCredits>" + Function.RawText(doc.ARCMExpAct) + "</ExemptedCredits>";
			}
			if (!string.IsNullOrEmpty(doc.APCMAct))
			{
				text = text + "<PurchaseCreditAcc>" + Function.RawText(doc.APCMAct) + "</PurchaseCreditAcc>";
			}
			if (!string.IsNullOrEmpty(doc.APCMFrnAct))
			{
				text = text + "<ForeignPurchaseCreditAcc>" + Function.RawText(doc.APCMFrnAct) + "</ForeignPurchaseCreditAcc>";
			}
			if (!string.IsNullOrEmpty(doc.APCMEUAct))
			{
				text = text + "<EUPurchaseCreditAcc>" + Function.RawText(doc.APCMEUAct) + "</EUPurchaseCreditAcc>";
			}
			if (Function.ParseInt(doc.BPLid) > 0)
			{
				text = text + "<BusinessPlaceID>" + Function.RawText(doc.BPLid) + "</BusinessPlaceID>";
			}
			if (!string.IsNullOrEmpty(doc.NegStckAct))
			{
				text = text + "<NegativeInventoryAdjustmentAccount>" + Function.RawText(doc.NegStckAct) + "</NegativeInventoryAdjustmentAccount>";
			}
			if (!string.IsNullOrEmpty(doc.StkInTnAct))
			{
				text = text + "<StockInTransitAccount>" + Function.RawText(doc.StkInTnAct) + "</StockInTransitAccount>";
			}
			if (!string.IsNullOrEmpty(doc.AddrType))
			{
				text = text + "<AddressType>" + Function.RawText(doc.AddrType) + "</AddressType>";
			}
			if (!string.IsNullOrEmpty(doc.StreetNo))
			{
				text = text + "<StreetNo>" + Function.RawText(doc.StreetNo) + "</StreetNo>";
			}
			if (!string.IsNullOrEmpty(doc.PurBalAct))
			{
				text = text + "<PurchaseBalanceAccount>" + Function.RawText(doc.PurBalAct) + "</PurchaseBalanceAccount>";
			}
			if (!string.IsNullOrEmpty(doc.Excisable))
			{
				BoYesNoEnum boYesNoEnum2 = ((doc.Excisable == "Y") ? BoYesNoEnum.tYES : BoYesNoEnum.tNO);
				text = text + "<Excisable>" + Function.RawText(boYesNoEnum2) + "</Excisable>";
			}
			if (!string.IsNullOrEmpty(doc.WhICenAct))
			{
				text = text + "<WHIncomingCenvatAccount>" + Function.RawText(doc.WhICenAct) + "</WHIncomingCenvatAccount>";
			}
			if (!string.IsNullOrEmpty(doc.WhOCenAct))
			{
				text = text + "<WHOutgoingCenvatAccount>" + Function.RawText(doc.WhOCenAct) + "</WHOutgoingCenvatAccount>";
			}
			if (!string.IsNullOrEmpty(doc.WhShipTo))
			{
				text = text + "<WHShipToName>" + Function.RawText(doc.WhShipTo) + "</WHShipToName>";
			}
			if (!string.IsNullOrEmpty(doc.Shipper))
			{
				text = text + "<Shipper>" + Function.RawText(doc.Shipper) + "</Shipper>";
			}
			if (!string.IsNullOrEmpty(doc.BinActivat))
			{
				BoYesNoEnum boYesNoEnum3 = ((doc.BinActivat == "Y") ? BoYesNoEnum.tYES : BoYesNoEnum.tNO);
				text = text + "<EnableBinLocations>" + Function.RawText(boYesNoEnum3) + "</EnableBinLocations>";
			}
			if (!string.IsNullOrEmpty(doc.BinSeptor))
			{
				text = text + "<BinLocCodeSeparator>" + Function.RawText(doc.BinSeptor) + "</BinLocCodeSeparator>";
			}
			if (Function.ParseInt(doc.DftBinAbs) > 0)
			{
				text = text + "<DefaultBin>" + Function.RawText(doc.DftBinAbs) + "</DefaultBin>";
			}
			if (!string.IsNullOrEmpty(doc.DftBinEnfd))
			{
				BoYesNoEnum boYesNoEnum4 = ((doc.DftBinEnfd == "Y") ? BoYesNoEnum.tYES : BoYesNoEnum.tNO);
				text = text + "<DefaultBinEnforced>" + Function.RawText(boYesNoEnum4) + "</DefaultBinEnforced>";
			}
			if (Function.ParseInt(doc.AutoIssMtd) > -1)
			{
				BoDocWhsAutoIssueMethod boDocWhsAutoIssueMethod = BoDocWhsAutoIssueMethod.aimSingleChoiceOnly;
				switch (doc.AutoIssMtd)
				{
				case 0:
					boDocWhsAutoIssueMethod = BoDocWhsAutoIssueMethod.aimSingleChoiceOnly;
					break;
				case 1:
					boDocWhsAutoIssueMethod = BoDocWhsAutoIssueMethod.aimBinCodeOrder;
					break;
				case 2:
					boDocWhsAutoIssueMethod = BoDocWhsAutoIssueMethod.aimAlternativeSortCodeOrder;
					break;
				case 3:
					boDocWhsAutoIssueMethod = BoDocWhsAutoIssueMethod.aimQtyDescendingOrder;
					break;
				case 4:
					boDocWhsAutoIssueMethod = BoDocWhsAutoIssueMethod.aimQtyAscendingOrder;
					break;
				case 5:
					boDocWhsAutoIssueMethod = BoDocWhsAutoIssueMethod.aimBinFIFO;
					break;
				case 6:
					boDocWhsAutoIssueMethod = BoDocWhsAutoIssueMethod.aimBinLIFO;
					break;
				}
				text = text + "<AutoAllocOnIssue>" + Function.RawText(boDocWhsAutoIssueMethod) + "</AutoAllocOnIssue>";
			}
			if (!string.IsNullOrEmpty(doc.ManageSnB))
			{
				BoYesNoEnum boYesNoEnum5 = ((doc.ManageSnB == "Y") ? BoYesNoEnum.tYES : BoYesNoEnum.tNO);
				text = text + "<ManageSerialAndBatchNumbers>" + Function.RawText(boYesNoEnum5) + "</ManageSerialAndBatchNumbers>";
			}
			if (Function.ParseInt(doc.RecItemsBy) > -1)
			{
				ReceivingBinLocationsMethodEnum receivingBinLocationsMethodEnum = ReceivingBinLocationsMethodEnum.rblmBinLocationCodeOrder;
				switch (doc.RecItemsBy)
				{
				case 0:
					receivingBinLocationsMethodEnum = ReceivingBinLocationsMethodEnum.rblmBinLocationCodeOrder;
					break;
				case 1:
					receivingBinLocationsMethodEnum = ReceivingBinLocationsMethodEnum.rblmAlternativeSortCodeOrder;
					break;
				}
				text = text + "<ReceivingBinLocationsBy>" + Function.RawText(receivingBinLocationsMethodEnum) + "</ReceivingBinLocationsBy>";
			}
			if (!string.IsNullOrEmpty(doc.RecBinEnab))
			{
				BoYesNoEnum boYesNoEnum6 = ((doc.RecBinEnab == "Y") ? BoYesNoEnum.tYES : BoYesNoEnum.tNO);
				text = text + "<EnableReceivingBinLocations>" + Function.RawText(boYesNoEnum6) + "</EnableReceivingBinLocations>";
			}
			if (!string.IsNullOrEmpty(doc.GlblLocNum))
			{
				text = text + "<GlobalLocationNumber>" + Function.RawText(doc.GlblLocNum) + "</GlobalLocationNumber>";
			}
			if (!string.IsNullOrEmpty(doc.RecvEmpBin))
			{
				BoYesNoEnum boYesNoEnum7 = ((doc.RecvEmpBin == "Y") ? BoYesNoEnum.tYES : BoYesNoEnum.tNO);
				text = text + "<RestrictReceiptToEmptyBinLocation>" + Function.RawText(boYesNoEnum7) + "</RestrictReceiptToEmptyBinLocation>";
			}
			if (!string.IsNullOrEmpty(doc.Inactive))
			{
				BoYesNoEnum boYesNoEnum8 = ((doc.Inactive == "Y") ? BoYesNoEnum.tYES : BoYesNoEnum.tNO);
				text = text + "<Inactive>" + Function.RawText(boYesNoEnum8) + "</Inactive>";
			}
			if (!string.IsNullOrEmpty(doc.RecvMaxQty))
			{
				BoYesNoEnum boYesNoEnum9 = ((doc.RecvMaxQty == "Y") ? BoYesNoEnum.tYES : BoYesNoEnum.tNO);
				text = text + "<ReceiveUpToMaxQuantity>" + Function.RawText(boYesNoEnum9) + "</ReceiveUpToMaxQuantity>";
			}
			if (Function.ParseInt(doc.AutoRecvMd) > -1)
			{
				AutoAllocOnReceiptMethodEnum autoAllocOnReceiptMethodEnum = AutoAllocOnReceiptMethodEnum.aaormDefaultBin;
				switch (doc.AutoRecvMd)
				{
				case 0:
					autoAllocOnReceiptMethodEnum = AutoAllocOnReceiptMethodEnum.aaormDefaultBin;
					break;
				case 1:
					autoAllocOnReceiptMethodEnum = AutoAllocOnReceiptMethodEnum.aaormLastBinReceivedItem;
					break;
				case 2:
					autoAllocOnReceiptMethodEnum = AutoAllocOnReceiptMethodEnum.aaormItemCurrentBins;
					break;
				case 3:
					autoAllocOnReceiptMethodEnum = AutoAllocOnReceiptMethodEnum.aaormItemCurrentAndHistoricalBins;
					break;
				}
				text = text + "<AutoAllocOnReceipt>" + Function.RawText(autoAllocOnReceiptMethodEnum) + "</AutoAllocOnReceipt>";
			}
			if (!string.IsNullOrEmpty(doc.RecvMaxWT))
			{
				BoYesNoEnum boYesNoEnum10 = ((doc.RecvMaxWT == "Y") ? BoYesNoEnum.tYES : BoYesNoEnum.tNO);
				text = text + "<ReceiveUpToMaxWeight>" + Function.RawText(boYesNoEnum10) + "</ReceiveUpToMaxWeight>";
			}
			if (Function.ParseInt(doc.RecvUpTo) > -1)
			{
				ReceivingUpToMethodEnum receivingUpToMethodEnum = ReceivingUpToMethodEnum.rutmMaximumQty;
				switch (doc.RecvUpTo)
				{
				case "0":
					receivingUpToMethodEnum = ReceivingUpToMethodEnum.rutmMaximumQty;
					break;
				case "1":
					receivingUpToMethodEnum = ReceivingUpToMethodEnum.rutmMaximumWeight;
					break;
				case "2":
					receivingUpToMethodEnum = ReceivingUpToMethodEnum.rutmBothMaxQtyAndWeight;
					break;
				}
				text = text + "<ReceiveUpToMethod>" + Function.RawText(receivingUpToMethodEnum) + "</ReceiveUpToMethod>";
			}
			if (!string.IsNullOrEmpty(doc.TaxOffice))
			{
				text = text + "<TaxOffice>" + Function.RawText(doc.TaxOffice) + "</TaxOffice>";
			}
			text += DataService.MappingUdf(doc.UserFields, commandType);
			text += "</row>\r\n                        </Warehouses>";
			text += "</BO></BOM>";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return text;
	}
}
