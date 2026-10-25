using System;
using System.Collections.Generic;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.DataServices.Implements;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.MasterData;
using Apzon.Services;
using SAPbobsCOM;

namespace Apzon.DataServices.HanaImplements;

public class HanaWarehousesRepository : WarehousesRepository
{
	public HanaWarehousesRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, unitOfWork, dataService)
	{
	}

	public override string AddDIS(WarehouseImpl doc, string sessionId)
	{
		try
		{
			string xmlObject = MappingDIS(doc, EnumCommandType.Add, sessionId);
			HttpResult httpResult = DataService.AddObject(ApzObjectTypes.oWarehouses, sessionId, xmlObject);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				return httpResult.Message;
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	protected override string MappingDIS(WarehouseImpl doc, EnumCommandType commandType, string sessionId)
	{
		string text = "";
		try
		{
			text = "{";
			if (!string.IsNullOrEmpty(doc.WhsCode) && commandType == EnumCommandType.Add)
			{
				text += Function.GetJsonString("WarehouseCode", doc.WhsCode);
			}
			if (!string.IsNullOrEmpty(doc.WhsName))
			{
				text += Function.GetJsonString("WarehouseName", Function.ToString(doc.WhsName));
			}
			if (!string.IsNullOrEmpty(doc.BalInvntAc))
			{
				text += Function.GetJsonString("StockAccount", Function.ToString(doc.BalInvntAc));
			}
			if (!string.IsNullOrEmpty(doc.SaleCostAc))
			{
				text += Function.GetJsonString("CostOfGoodsSold", Function.ToString(doc.SaleCostAc));
			}
			if (!string.IsNullOrEmpty(doc.TransferAc))
			{
				text += Function.GetJsonString("TransfersAcc", Function.ToString(doc.TransferAc));
			}
			if (!string.IsNullOrEmpty(doc.RevenuesAc))
			{
				text += Function.GetJsonString("RevenuesAccount", Function.ToString(doc.RevenuesAc));
			}
			if (!string.IsNullOrEmpty(doc.VarianceAc))
			{
				text += Function.GetJsonString("VarianceAccount", Function.ToString(doc.VarianceAc));
			}
			if (!string.IsNullOrEmpty(doc.DecreasAc))
			{
				text += Function.GetJsonString("DecreasingAccount", Function.ToString(doc.DecreasAc));
			}
			if (!string.IsNullOrEmpty(doc.IncresGlAc))
			{
				text += Function.GetJsonString("IncreasingAcc", Function.ToString(doc.IncresGlAc));
			}
			if (!string.IsNullOrEmpty(doc.PAReturnAc))
			{
				text += Function.GetJsonString("ReturningAccount", Function.ToString(doc.PAReturnAc));
			}
			if (!string.IsNullOrEmpty(doc.ExpensesAc))
			{
				text += Function.GetJsonString("ExpenseAccount", Function.ToString(doc.ExpensesAc));
			}
			if (!string.IsNullOrEmpty(doc.EURevenuAc))
			{
				text += Function.GetJsonString("EURevenuesAccount", Function.ToString(doc.EURevenuAc));
			}
			if (!string.IsNullOrEmpty(doc.ExpensesAc))
			{
				text += Function.GetJsonString("EUExpensesAccount", Function.ToString(doc.ExpensesAc));
			}
			if (!string.IsNullOrEmpty(doc.FrRevenuAc))
			{
				text += Function.GetJsonString("ForeignRevenuesAcc", Function.ToString(doc.FrRevenuAc));
			}
			if (!string.IsNullOrEmpty(doc.FrExpensAc))
			{
				text += Function.GetJsonString("ForeignExpensesAccount", Function.ToString(doc.FrExpensAc));
			}
			if (!string.IsNullOrEmpty(doc.VatGroup))
			{
				text += Function.GetJsonString("TaxGroup", Function.ToString(doc.VatGroup));
			}
			if (!string.IsNullOrEmpty(doc.Street))
			{
				text += Function.GetJsonString("Street", Function.ToString(doc.Street));
			}
			if (!string.IsNullOrEmpty(doc.Block))
			{
				text += Function.GetJsonString("Block", Function.ToString(doc.Block));
			}
			if (!string.IsNullOrEmpty(doc.ZipCode))
			{
				text += Function.GetJsonString("ZipCode", Function.ToString(doc.ZipCode));
			}
			if (!string.IsNullOrEmpty(doc.City))
			{
				text += Function.GetJsonString("City", Function.ToString(doc.City));
			}
			if (!string.IsNullOrEmpty(doc.County))
			{
				text += Function.GetJsonString("County", Function.ToString(doc.County));
			}
			if (!string.IsNullOrEmpty(doc.Country))
			{
				text += Function.GetJsonString("Country", Function.ToString(doc.Country));
			}
			if (!string.IsNullOrEmpty(doc.State))
			{
				text += Function.GetJsonString("State", Function.ToString(doc.State));
			}
			if (Function.ParseInt(doc.Location) > 0)
			{
				text += Function.GetJsonString("Location", Function.ToString(doc.Location));
			}
			if (!string.IsNullOrEmpty(doc.DropShip))
			{
				text += Function.GetJsonString("DropShip", Function.ToString(doc.DropShip));
			}
			if (!string.IsNullOrEmpty(doc.ExmptIncom))
			{
				text += Function.GetJsonString("ExemptRevenuesAccount", Function.ToString(doc.ExmptIncom));
			}
			if (!string.IsNullOrEmpty(doc.UseTax))
			{
				text += Function.GetJsonString("AllowUseTax", Function.ToString(doc.UseTax));
			}
			if (!string.IsNullOrEmpty(doc.PriceDifAc))
			{
				text += Function.GetJsonString("PriceDifferencesAccount", Function.ToString(doc.PriceDifAc));
			}
			if (!string.IsNullOrEmpty(doc.ExchangeAc))
			{
				text += Function.GetJsonString("ExchangeRateDifferencesAccount", Function.ToString(doc.ExchangeAc));
			}
			if (!string.IsNullOrEmpty(doc.BalanceAcc))
			{
				text += Function.GetJsonString("GoodsClearingAcc", Function.ToString(doc.BalanceAcc));
			}
			if (!string.IsNullOrEmpty(doc.PurchaseAc))
			{
				text += Function.GetJsonString("PurchaseAccount", Function.ToString(doc.PurchaseAc));
			}
			if (!string.IsNullOrEmpty(doc.PAReturnAc))
			{
				text += Function.GetJsonString("PurchaseReturningAccount", Function.ToString(doc.PAReturnAc));
			}
			if (!string.IsNullOrEmpty(doc.PurchOfsAc))
			{
				text += Function.GetJsonString("PurchaseOffsetAccount", Function.ToString(doc.PurchOfsAc));
			}
			if (!string.IsNullOrEmpty(doc.FedTaxID))
			{
				text += Function.GetJsonString("FederalTaxID", Function.ToString(doc.FedTaxID));
			}
			if (!string.IsNullOrEmpty(doc.Building))
			{
				text += Function.GetJsonString("BuildingFloorRoom", Function.ToString(doc.Building));
			}
			if (!string.IsNullOrEmpty(doc.ShpdGdsAct))
			{
				text += Function.GetJsonString("ShippedGoodsAccount", Function.ToString(doc.ShpdGdsAct));
			}
			if (!string.IsNullOrEmpty(doc.VatRevAct))
			{
				text += Function.GetJsonString("VATInRevenueAccount", Function.ToString(doc.VatRevAct));
			}
			if (!string.IsNullOrEmpty(doc.DecresGlAc))
			{
				text += Function.GetJsonString("DecreaseGLAccount", Function.ToString(doc.DecresGlAc));
			}
			if (!string.IsNullOrEmpty(doc.IncresGlAc))
			{
				text += Function.GetJsonString("IncreaseGLAccount", Function.ToString(doc.IncresGlAc));
			}
			if (!string.IsNullOrEmpty(doc.Nettable))
			{
				BoYesNoEnum boYesNoEnum = ((doc.Nettable == "Y") ? BoYesNoEnum.tYES : BoYesNoEnum.tNO);
				text += Function.GetJsonString("Nettable", Function.ToString(boYesNoEnum));
			}
			if (!string.IsNullOrEmpty(doc.IncreasAc))
			{
				text += Function.GetJsonString("StockInflationAdjustAccount", Function.ToString(doc.IncreasAc));
			}
			if (!string.IsNullOrEmpty(doc.DecreasAc))
			{
				text += Function.GetJsonString("StockInflationOffsetAccount", Function.ToString(doc.DecreasAc));
			}
			if (!string.IsNullOrEmpty(doc.WipAcct))
			{
				text += Function.GetJsonString("WIPMaterialAccount", Function.ToString(doc.WipAcct));
			}
			if (!string.IsNullOrEmpty(doc.WipVarAcct))
			{
				text += Function.GetJsonString("WIPMaterialVarianceAccount", Function.ToString(doc.WipVarAcct));
			}
			if (!string.IsNullOrEmpty(doc.CostRvlAct))
			{
				text += Function.GetJsonString("CostInflationAccount", Function.ToString(doc.CostRvlAct));
			}
			if (!string.IsNullOrEmpty(doc.CstOffsAct))
			{
				text += Function.GetJsonString("CostInflationOffsetAccount", Function.ToString(doc.CstOffsAct));
			}
			if (!string.IsNullOrEmpty(doc.ExpClrAct))
			{
				text += Function.GetJsonString("ExpensesClearingAccount", Function.ToString(doc.ExpClrAct));
			}
			if (!string.IsNullOrEmpty(doc.ExpOfstAct))
			{
				text += Function.GetJsonString("ExpenseOffsetingAct", Function.ToString(doc.ExpOfstAct));
			}
			if (!string.IsNullOrEmpty(doc.ARCMEUAct))
			{
				text += Function.GetJsonString("SalesCreditAcc", Function.ToString(doc.ARCMEUAct));
			}
			if (!string.IsNullOrEmpty(doc.ARCMFrnAct))
			{
				text += Function.GetJsonString("SalesCreditForeignAcc", Function.ToString(doc.ARCMFrnAct));
			}
			if (!string.IsNullOrEmpty(doc.ARCMEUAct))
			{
				text += Function.GetJsonString("SalesCreditEUAcc", Function.ToString(doc.ARCMEUAct));
			}
			if (!string.IsNullOrEmpty(doc.ARCMExpAct))
			{
				text += Function.GetJsonString("ExemptedCredits", Function.ToString(doc.ARCMExpAct));
			}
			if (!string.IsNullOrEmpty(doc.APCMAct))
			{
				text += Function.GetJsonString("PurchaseCreditAcc", Function.ToString(doc.APCMAct));
			}
			if (!string.IsNullOrEmpty(doc.APCMFrnAct))
			{
				text += Function.GetJsonString("ForeignPurchaseCreditAcc", Function.ToString(doc.APCMFrnAct));
			}
			if (!string.IsNullOrEmpty(doc.APCMEUAct))
			{
				text += Function.GetJsonString("EUPurchaseCreditAcc", Function.ToString(doc.APCMEUAct));
			}
			if (Function.ParseInt(doc.BPLid) > 0)
			{
				text += Function.GetJsonString("BusinessPlaceID", Function.ToString(doc.BPLid));
			}
			if (!string.IsNullOrEmpty(doc.NegStckAct))
			{
				text += Function.GetJsonString("NegativeInventoryAdjustmentAccount", Function.ToString(doc.NegStckAct));
			}
			if (!string.IsNullOrEmpty(doc.StkInTnAct))
			{
				text += Function.GetJsonString("StockInTransitAccount", Function.ToString(doc.StkInTnAct));
			}
			if (!string.IsNullOrEmpty(doc.AddrType))
			{
				text += Function.GetJsonString("AddressType", Function.ToString(doc.AddrType));
			}
			if (!string.IsNullOrEmpty(doc.StreetNo))
			{
				text += Function.GetJsonString("StreetNo", Function.ToString(doc.StreetNo));
			}
			if (!string.IsNullOrEmpty(doc.PurBalAct))
			{
				text += Function.GetJsonString("PurchaseBalanceAccount", Function.ToString(doc.PurBalAct));
			}
			if (!string.IsNullOrEmpty(doc.Excisable))
			{
				BoYesNoEnum boYesNoEnum2 = ((doc.Excisable == "Y") ? BoYesNoEnum.tYES : BoYesNoEnum.tNO);
				text += Function.GetJsonString("Excisable", Function.ToString(boYesNoEnum2));
			}
			if (!string.IsNullOrEmpty(doc.WhICenAct))
			{
				text += Function.GetJsonString("WHIncomingCenvatAccount", Function.ToString(doc.WhICenAct));
			}
			if (!string.IsNullOrEmpty(doc.WhOCenAct))
			{
				text += Function.GetJsonString("WHOutgoingCenvatAccount", Function.ToString(doc.WhOCenAct));
			}
			if (!string.IsNullOrEmpty(doc.WhShipTo))
			{
				text += Function.GetJsonString("WHShipToName", Function.ToString(doc.WhShipTo));
			}
			if (!string.IsNullOrEmpty(doc.Shipper))
			{
				text += Function.GetJsonString("Shipper", Function.ToString(doc.Shipper));
			}
			if (!string.IsNullOrEmpty(doc.BinActivat))
			{
				BoYesNoEnum boYesNoEnum3 = ((doc.BinActivat == "Y") ? BoYesNoEnum.tYES : BoYesNoEnum.tNO);
				text += Function.GetJsonString("EnableBinLocations", Function.ToString(boYesNoEnum3));
			}
			if (!string.IsNullOrEmpty(doc.BinSeptor))
			{
				text += Function.GetJsonString("BinLocCodeSeparator", Function.ToString(doc.BinSeptor));
			}
			if (Function.ParseInt(doc.DftBinAbs) > 0)
			{
				text += Function.GetJsonString("DefaultBin", Function.ToString(doc.DftBinAbs));
			}
			if (!string.IsNullOrEmpty(doc.DftBinEnfd))
			{
				BoYesNoEnum boYesNoEnum4 = ((doc.DftBinEnfd == "Y") ? BoYesNoEnum.tYES : BoYesNoEnum.tNO);
				text += Function.GetJsonString("DefaultBinEnforced", Function.ToString(boYesNoEnum4));
			}
			if (Function.ParseInt(doc.AutoIssMtd) > -1 && doc.BinActivat == "Y")
			{
				string text2 = "";
				switch (doc.AutoIssMtd)
				{
				case 0:
					text2 = "whsBinSingleChoiceOnly";
					break;
				case 1:
					text2 = "whsBinBinCodeOrder";
					break;
				case 2:
					text2 = "whsBinAltSortCodeOrder";
					break;
				case 3:
					text2 = "whsBinQtyDescendingOrder";
					break;
				case 4:
					text2 = "whsBinQtyAscendingOrder";
					break;
				case 5:
					text2 = "whsBinFIFO";
					break;
				case 6:
					text2 = "whsBinLIFO";
					break;
				case 7:
					text2 = "whsBinSingleBinPreferred";
					break;
				}
				text += Function.GetJsonString("AutoAllocOnIssue", Function.ToString(text2));
			}
			if (!string.IsNullOrEmpty(doc.ManageSnB))
			{
				BoYesNoEnum boYesNoEnum5 = ((doc.ManageSnB == "Y") ? BoYesNoEnum.tYES : BoYesNoEnum.tNO);
				text += Function.GetJsonString("ManageSerialAndBatchNumbers", Function.ToString(boYesNoEnum5));
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
				text += Function.GetJsonString("ReceivingBinLocationsBy", Function.ToString(receivingBinLocationsMethodEnum));
			}
			if (!string.IsNullOrEmpty(doc.RecBinEnab))
			{
				BoYesNoEnum boYesNoEnum6 = ((doc.RecBinEnab == "Y") ? BoYesNoEnum.tYES : BoYesNoEnum.tNO);
				text += Function.GetJsonString("EnableReceivingBinLocations", Function.ToString(boYesNoEnum6));
			}
			if (!string.IsNullOrEmpty(doc.GlblLocNum))
			{
				text += Function.GetJsonString("GlobalLocationNumber", Function.ToString(doc.GlblLocNum));
			}
			if (!string.IsNullOrEmpty(doc.RecvEmpBin))
			{
				BoYesNoEnum boYesNoEnum7 = ((doc.RecvEmpBin == "Y") ? BoYesNoEnum.tYES : BoYesNoEnum.tNO);
				text += Function.GetJsonString("RestrictReceiptToEmptyBinLocation", Function.ToString(boYesNoEnum7));
			}
			if (!string.IsNullOrEmpty(doc.Inactive))
			{
				BoYesNoEnum boYesNoEnum8 = ((doc.Inactive == "Y") ? BoYesNoEnum.tYES : BoYesNoEnum.tNO);
				text += Function.GetJsonString("Inactive", Function.ToString(boYesNoEnum8));
			}
			if (!string.IsNullOrEmpty(doc.RecvMaxQty))
			{
				BoYesNoEnum boYesNoEnum9 = ((doc.RecvMaxQty == "Y") ? BoYesNoEnum.tYES : BoYesNoEnum.tNO);
				text += Function.GetJsonString("ReceiveUpToMaxQuantity", Function.ToString(boYesNoEnum9));
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
				text += Function.GetJsonString("AutoAllocOnReceipt", Function.ToString(autoAllocOnReceiptMethodEnum));
			}
			if (!string.IsNullOrEmpty(doc.RecvMaxWT))
			{
				BoYesNoEnum boYesNoEnum10 = ((doc.RecvMaxWT == "Y") ? BoYesNoEnum.tYES : BoYesNoEnum.tNO);
				text += Function.GetJsonString("ReceiveUpToMaxWeight", Function.ToString(boYesNoEnum10));
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
				text += Function.GetJsonString("ReceiveUpToMethod", Function.ToString(receivingUpToMethodEnum));
			}
			if (!string.IsNullOrEmpty(doc.TaxOffice))
			{
				text += Function.GetJsonString("TaxOffice", Function.ToString(doc.TaxOffice));
			}
			text += DataService.MappingUdf(doc.UserFields, commandType);
			text += "}";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return text;
	}

	public override string UpdateDIS(WarehouseImpl doc, string sessionId)
	{
		try
		{
			string bodyObject = MappingDIS(doc, EnumCommandType.Update, sessionId);
			List<BodyRequestObject> list = new List<BodyRequestObject>();
			list.Add(new BodyRequestObject
			{
				Action = Enums.HttpAction.Update,
				BodyObject = bodyObject,
				Key = doc.WhsCode,
				ObjectType = ApzObjectTypes.oWarehouses,
				OutObjectKey = true
			});
			HttpResult httpResult = DataService.ProcessMutiTransaction(sessionId, list);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				return httpResult.Message;
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return string.Empty;
		}
	}
}
