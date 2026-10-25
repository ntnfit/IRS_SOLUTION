using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.DataServices.Implements;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.MasterData;
using Apzon.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SAPbobsCOM;

namespace Apzon.DataServices.HanaImplements;

public class HanaItemsRepository : ItemsRepository
{
	public HanaItemsRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, unitOfWork, dataService)
	{
	}

	private HttpResult SaveAttachment(ItemsImpl doc, string sessionId, bool addNew)
	{
		if (addNew || doc.AtcEntry <= 0)
		{
			return _unitOfWork.Attachment2.Add(doc.AttachmentFiles, sessionId);
		}
		return _unitOfWork.Attachment2.Update(doc.AtcEntry, doc.AttachmentFiles, sessionId);
	}

	public override string Save(ItemsImpl doc, string sessionId, bool ignoreEmptyValue = false)
	{
		try
		{
			List<BodyRequestObject> list = new List<BodyRequestObject>();
			EnumCommandType enumCommandType = ((doc.APZ_SAVE_ACTION.ToUpper() == "A") ? EnumCommandType.Add : EnumCommandType.Update);
			if (doc.AttachmentFiles != null && doc.AttachmentFiles.Count != 0)
			{
				HttpResult httpResult = SaveAttachment(doc, sessionId, enumCommandType == EnumCommandType.Add);
				if (httpResult.MessageCode != Enums.MessageCode.Success)
				{
					return httpResult.Message;
				}
				doc.AtcEntry = Function.ParseInt(httpResult.Content);
			}
			string text = "";
			if (doc.APZ_SAVE_ACTION.ToUpper() != "A" && doc.APZ_SAVE_ACTION.ToUpper() != "U")
			{
				return "Invalid APZ_SAVE_ACTION. Valid value is 'A'-Add, 'U'-Update";
			}
			string text2 = ((doc.APZ_SAVE_ACTION.ToUpper() == "A") ? "AddObject" : "UpdateObject");
			ApzObjectTypes objectType = ApzObjectTypes.oItems;
			if (enumCommandType == EnumCommandType.Update && !doc.ItemCode.Equals(doc.OldItemCode))
			{
				enumCommandType = EnumCommandType.Add;
				text2 = "AddObject";
				list.Add(new BodyRequestObject
				{
					Action = Enums.HttpAction.Delete,
					OutObjectKey = true,
					ObjectType = objectType,
					Key = Function.ToString(doc.OldItemCode)
				});
			}
			string value = MappingDIS(doc, enumCommandType, sessionId, ignoreEmptyValue);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(value);
			list.Add(new BodyRequestObject
			{
				ObjectType = objectType,
				OutObjectKey = true,
				Key = Function.ToString(doc.ItemCode),
				Action = ((text2 == "AddObject") ? Enums.HttpAction.AddNew : ((!(text2 == "UpdateObject")) ? Enums.HttpAction.AddNew : Enums.HttpAction.Update)),
				BodyObject = stringBuilder.ToString()
			});
			HttpResult httpResult2 = DataService.ProcessMutiTransaction(sessionId, list);
			string text3 = Function.ToString(httpResult2.Content);
			if (!string.IsNullOrEmpty(httpResult2.Message))
			{
				text = httpResult2.Message;
				Logging.Write("ERROR", text2 + " Item master error: " + httpResult2.Message);
			}
			else
			{
				if (doc.AtcEntry > 0)
				{
					text = UpdateAtcEntry(text3, doc.AtcEntry);
				}
				if (!string.IsNullOrWhiteSpace(text))
				{
					Logging.Write("ERROR", string.Format(text2 + " Item master-[{0}] success. But update AtcEntry Error-[{1}]", (text2 == "AddObject") ? text3 : Function.ToString(doc.ItemCode), text));
				}
				else
				{
					Logging.Write("WATCH", string.Format(text2 + " Item master - {0}", (text2 == "AddObject") ? text3 : Function.ToString(doc.ItemCode)));
				}
				text = "";
			}
			return text;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	protected override string MappingDIS(ItemsImpl doc, EnumCommandType commandType, string sessionId = "", bool ignoreEmptyValue = false)
	{
		StringBuilder stringBuilder = new StringBuilder();
		try
		{
			stringBuilder.Append("{");
			if (!string.IsNullOrWhiteSpace(doc.ItemCode) && commandType == EnumCommandType.Add && (_unitOfWork.Common.GetIsManualSerial(doc.Series) || !_unitOfWork.Common.ExistsSeries(ApzObjectTypes.oItems, doc.Series)))
			{
				stringBuilder.Append(Function.GetJsonString("ItemCode", Function.ToString(doc.ItemCode)));
			}
			if (!string.IsNullOrWhiteSpace(doc.ItemName))
			{
				stringBuilder.Append(Function.GetJsonString("ItemName", Function.ToString(doc.ItemName)));
			}
			stringBuilder.Append(Function.GetJsonString("ForeignName", Function.ToString(doc.FrgnName)));
			if (!string.IsNullOrEmpty(doc.ItemType))
			{
				ItemTypeEnum itemTypeEnum = ItemTypeEnum.itItems;
				stringBuilder.Append(Function.GetJsonString("ItemType", doc.ItemType switch
				{
					"I" => ItemTypeEnum.itItems, 
					"F" => ItemTypeEnum.itFixedAssets, 
					"L" => ItemTypeEnum.itLabor, 
					"T" => ItemTypeEnum.itTravel, 
					_ => ItemTypeEnum.itItems, 
				}));
			}
			if (!string.IsNullOrWhiteSpace(doc.PrchseItem))
			{
				string value = ((doc.PrchseItem == "Y") ? "tYES" : "tNO");
				stringBuilder.Append(Function.GetJsonString("PurchaseItem", value));
			}
			if (!string.IsNullOrWhiteSpace(doc.InvntItem))
			{
				string value2 = ((doc.InvntItem == "Y") ? "tYES" : "tNO");
				stringBuilder.Append(Function.GetJsonString("InventoryItem", value2));
			}
			if (!string.IsNullOrWhiteSpace(doc.SellItem))
			{
				string value3 = ((doc.SellItem == "Y") ? "tYES" : "tNO");
				stringBuilder.Append(Function.GetJsonString("SalesItem", value3));
			}
			if (Function.ParseInt(doc.ToleranDay) >= 0 || (!ignoreEmptyValue && commandType == EnumCommandType.Update))
			{
				stringBuilder.Append(Function.GetJsonString("ToleranceDays", Function.ToString(doc.ToleranDay)));
			}
			if (Function.ParseInt(doc.ItmsGrpCod) > 0)
			{
				stringBuilder.Append(Function.GetJsonString("ItemsGroupCode", Function.ToString(doc.ItmsGrpCod)));
			}
			if (doc.CstGrpCode != 0)
			{
				stringBuilder.Append(Function.GetJsonString("CustomsGroupCode", Function.ToString(doc.CstGrpCode)));
			}
			if (!string.IsNullOrWhiteSpace(doc.VatGourpSa))
			{
				stringBuilder.Append(Function.GetJsonString("SalesVATGroup", Function.ToString(doc.VatGourpSa)));
			}
			if (!string.IsNullOrWhiteSpace(doc.CodeBars) || (!ignoreEmptyValue && commandType == EnumCommandType.Update))
			{
				stringBuilder.Append(Function.GetJsonString("BarCode", Function.ToString(doc.CodeBars)));
			}
			if (doc.VATLiable != null)
			{
				if (!string.IsNullOrWhiteSpace(doc.VATLiable) && doc.VATLiable.ToLower() == "n")
				{
					stringBuilder.Append(Function.GetJsonString("VatLiable", "tNO"));
				}
				else
				{
					stringBuilder.Append(Function.GetJsonString("VatLiable", "tYES"));
				}
			}
			if (!string.IsNullOrWhiteSpace(doc.CardCode) || (!ignoreEmptyValue && commandType == EnumCommandType.Update))
			{
				stringBuilder.Append(Function.GetJsonString("Mainsupplier", Function.ToString(doc.CardCode)));
			}
			if (!string.IsNullOrWhiteSpace(doc.SuppCatNum) || (!ignoreEmptyValue && commandType == EnumCommandType.Update))
			{
				stringBuilder.Append(Function.GetJsonString("SupplierCatalogNo", Function.ToString(doc.SuppCatNum)));
			}
			if (!string.IsNullOrEmpty(doc.PicturName) || (!ignoreEmptyValue && commandType == EnumCommandType.Update))
			{
				stringBuilder.Append(Function.GetJsonString("Picture", Function.ToString(doc.PicturName)));
			}
			if (!string.IsNullOrWhiteSpace(doc.SerialNum) || (!ignoreEmptyValue && commandType == EnumCommandType.Update))
			{
				stringBuilder.Append(Function.GetJsonString("SerialNum", Function.ToString(doc.SerialNum)));
			}
			if (Function.ParseDouble(doc.CommisPcnt) > 0.0 || (!ignoreEmptyValue && commandType == EnumCommandType.Update))
			{
				stringBuilder.Append(Function.GetJsonString("CommissionPercent", Function.ParseDecimal(doc.CommisPcnt)));
			}
			if (Function.ParseDouble(doc.CommisSum) > 0.0 || (!ignoreEmptyValue && commandType == EnumCommandType.Update))
			{
				stringBuilder.Append(Function.GetJsonString("CommissionSum", Function.ParseDecimal(doc.CommisSum)));
			}
			stringBuilder.Append(Function.GetJsonString("CommissionGroup", 0));
			if (doc.AssetItem != null)
			{
				if (!string.IsNullOrWhiteSpace(doc.AssetItem) && doc.AssetItem.ToLower() == "y")
				{
					stringBuilder.Append(Function.GetJsonString("AssetItem", "tYES"));
				}
				else
				{
					stringBuilder.Append(Function.GetJsonString("AssetItem", "tNO"));
				}
			}
			if (!string.IsNullOrWhiteSpace(doc.ExportCode) || (!ignoreEmptyValue && commandType == EnumCommandType.Update))
			{
				stringBuilder.Append(Function.GetJsonString("DataExportCode", Function.ToString(doc.ExportCode)));
			}
			if (doc.FirmCode <= 0)
			{
				stringBuilder.Append(Function.GetJsonString("Manufacturer", -1));
			}
			else if (doc.FirmCode > 0 || (!ignoreEmptyValue && commandType == EnumCommandType.Update))
			{
				stringBuilder.Append(Function.GetJsonString("Manufacturer", Function.ToString(doc.FirmCode)));
			}
			string value4 = ((doc.ManBtchNum == "Y") ? "tYES" : "tNO");
			stringBuilder.Append(Function.GetJsonString("ManageBatchNumbers", value4));
			string value5 = ((doc.ManSerNum == "Y") ? "tYES" : "tNO");
			stringBuilder.Append(Function.GetJsonString("ManageSerialNumbers", value5));
			if (doc.ManSerNum == "Y" || doc.ManBtchNum == "Y")
			{
				switch (doc.IssuePriBy)
				{
				case 0:
					stringBuilder.Append(Function.GetJsonString("IssuePrimarilyBy", "ipbSerialAndBatchNumbers"));
					break;
				case 1:
					stringBuilder.Append(Function.GetJsonString("IssuePrimarilyBy", "ipbBinLocations"));
					break;
				}
				if (doc.MngMethod == "A" || doc.MngMethod == "R")
				{
					BoManageMethod boManageMethod = ((!(doc.MngMethod == "A")) ? BoManageMethod.bomm_OnReleaseOnly : BoManageMethod.bomm_OnEveryTransaction);
					stringBuilder.Append(Function.GetJsonString("SRIAndBatchManageMethod", Function.ToString(boManageMethod)));
					if (doc.MngMethod == "R" && doc.ManSerNum == "Y")
					{
						if (doc.ManOutOnly == "Y")
						{
							stringBuilder.Append(Function.GetJsonString("ManageSerialNumbersOnReleaseOnly", "tYes"));
						}
						else
						{
							stringBuilder.Append(Function.GetJsonString("ManageSerialNumbersOnReleaseOnly", "tNO"));
						}
					}
				}
			}
			BoYesNoEnum boYesNoEnum = ((doc.validFor == "Y") ? BoYesNoEnum.tYES : BoYesNoEnum.tNO);
			stringBuilder.Append(Function.GetJsonString("Valid", Function.ToString(boYesNoEnum)));
			string value6;
			if (doc.validFor == "Y")
			{
				if (doc.validFrom.HasValue)
				{
					_ = doc.validFrom.Value;
					if (doc.validFrom.Value.Year > 2000)
					{
						value6 = doc.validFrom.Value.ToString("yyyyMMdd");
						goto IL_07cb;
					}
				}
				value6 = "";
				goto IL_07cb;
			}
			goto IL_0867;
			IL_0990:
			string value7;
			stringBuilder.Append(Function.GetJsonString("FrozenTo", value7));
			stringBuilder.Append(Function.GetJsonString("FrozenRemarks", Function.ToString(doc.FrozenComm)));
			goto IL_09b7;
			IL_09b7:
			if (!string.IsNullOrWhiteSpace(doc.BuyUnitMsr) || (!ignoreEmptyValue && commandType == EnumCommandType.Update))
			{
				stringBuilder.Append(Function.GetJsonString("PurchaseUnit", Function.ToString(doc.BuyUnitMsr)));
			}
			if (!string.IsNullOrWhiteSpace(doc.VatGroupPu) || (!ignoreEmptyValue && commandType == EnumCommandType.Update))
			{
				stringBuilder.Append(Function.GetJsonString("PurchaseVATGroup", Function.ToString(doc.VatGroupPu)));
			}
			if (Function.ParseDouble(doc.SalFactor1) > 0.0)
			{
				stringBuilder.Append(Function.GetJsonString("SalesFactor1", Function.ParseDecimal(doc.SalFactor1)));
			}
			if (Function.ParseDouble(doc.SalFactor2) > 0.0)
			{
				stringBuilder.Append(Function.GetJsonString("SalesFactor2", Function.ParseDecimal(doc.SalFactor2)));
			}
			if (Function.ParseDouble(doc.SalFactor3) > 0.0)
			{
				stringBuilder.Append(Function.GetJsonString("SalesFactor3", Function.ParseDecimal(doc.SalFactor3)));
			}
			if (Function.ParseDouble(doc.SalFactor4) > 0.0)
			{
				stringBuilder.Append(Function.GetJsonString("SalesFactor4", Function.ParseDecimal(doc.SalFactor4)));
			}
			if (Function.ParseDouble(doc.PurFactor1) > 0.0)
			{
				stringBuilder.Append(Function.GetJsonString("PurchaseFactor1", Function.ParseDecimal(doc.PurFactor1)));
			}
			if (Function.ParseDouble(doc.PurFactor2) > 0.0)
			{
				stringBuilder.Append(Function.GetJsonString("PurchaseFactor2", Function.ParseDecimal(doc.PurFactor2)));
			}
			if (Function.ParseDouble(doc.PurFactor3) > 0.0)
			{
				stringBuilder.Append(Function.GetJsonString("PurchaseFactor3", Function.ParseDecimal(doc.PurFactor3)));
			}
			if (Function.ParseDouble(doc.PurFactor4) > 0.0)
			{
				stringBuilder.Append(Function.GetJsonString("PurchaseFactor4", Function.ParseDecimal(doc.PurFactor4)));
			}
			if (!string.IsNullOrWhiteSpace(doc.FrgnInAcct) || (!ignoreEmptyValue && commandType == EnumCommandType.Update))
			{
				stringBuilder.Append(Function.GetJsonString("ForeignRevenuesAccount", Function.ToString(doc.FrgnInAcct)));
			}
			if (!string.IsNullOrWhiteSpace(doc.ECInAcct) || (!ignoreEmptyValue && commandType == EnumCommandType.Update))
			{
				stringBuilder.Append(Function.GetJsonString("ECRevenuesAccount", Function.ToString(doc.ECInAcct)));
			}
			if (!string.IsNullOrWhiteSpace(doc.FrgnExpAcc) || (!ignoreEmptyValue && commandType == EnumCommandType.Update))
			{
				stringBuilder.Append(Function.GetJsonString("ForeignExpensesAccount", Function.ToString(doc.FrgnExpAcc)));
			}
			if (!string.IsNullOrWhiteSpace(doc.ECExpAcc) || (!ignoreEmptyValue && commandType == EnumCommandType.Update))
			{
				stringBuilder.Append(Function.GetJsonString("ECExpensesAccount", Function.ToString(doc.ECExpAcc)));
			}
			if (Function.ParseDouble(doc.AvgPrice) > 0.0 || (!ignoreEmptyValue && commandType == EnumCommandType.Update))
			{
				stringBuilder.Append(Function.GetJsonString("AvgStdPrice", Function.ParseDecimal(doc.AvgPrice)));
			}
			if (!string.IsNullOrWhiteSpace(doc.DfltWH) || (!ignoreEmptyValue && commandType == EnumCommandType.Update))
			{
				stringBuilder.Append(Function.GetJsonString("DefaultWarehouse", Function.ToString(doc.DfltWH)));
			}
			if (Function.ParseInt(doc.ShipType) > 0)
			{
				stringBuilder.Append(Function.GetJsonString("ShipType", Function.ParseInt(doc.ShipType)));
			}
			if (doc.TaxType != null)
			{
				switch (Function.ToString(doc.TaxType).ToUpper())
				{
				case "Y":
					stringBuilder.Append(Function.GetJsonString("TaxType", "tt_Yes"));
					break;
				case "U":
					stringBuilder.Append(Function.GetJsonString("TaxType", "tt_UseTax"));
					break;
				case "N":
					stringBuilder.Append(Function.GetJsonString("TaxType", "tt_No"));
					break;
				default:
					stringBuilder.Append(Function.GetJsonString("TaxType", "tt_Yes"));
					break;
				}
			}
			if (Function.ParseDouble(doc.MaxLevel) > 0.0)
			{
				stringBuilder.Append(Function.GetJsonString("MaxInventory", Function.ParseDecimal(doc.MaxLevel)));
			}
			if (!_unitOfWork.Common.GetIsMultiBranch() && !_unitOfWork.Common.GetAvailableQty(doc.ItemCode) && !_unitOfWork.Common.GetSerialBatch(doc.ItemCode))
			{
				if (string.Compare(doc.ByWh, "y", ignoreCase: true) == 0)
				{
					stringBuilder.Append(Function.GetJsonString("ManageStockByWarehouse", "tYES"));
				}
				else
				{
					stringBuilder.Append(Function.GetJsonString("ManageStockByWarehouse", "tNO"));
				}
			}
			if (doc.ByWh != "Y")
			{
				if (doc.ReorderQty > 0.0 || (!ignoreEmptyValue && commandType == EnumCommandType.Update))
				{
					stringBuilder.Append(Function.GetJsonString("DesiredInventory", Function.ParseDecimal(doc.ReorderQty)));
				}
				if (doc.MinLevel > 0.0 || (!ignoreEmptyValue && commandType == EnumCommandType.Update))
				{
					stringBuilder.Append(Function.GetJsonString("MinInventory", Function.ParseDecimal(doc.MinLevel)));
				}
				if (doc.MaxLevel > 0.0 || (!ignoreEmptyValue && commandType == EnumCommandType.Update))
				{
					stringBuilder.Append(Function.GetJsonString("MaxInventory", Function.ParseDecimal(doc.MaxLevel)));
				}
			}
			if (Function.ParseInt(doc.BHght2Unit) > 0)
			{
				stringBuilder.Append(Function.GetJsonString("PurchaseHeightUnit1", Function.ParseInt(doc.BHght2Unit)));
			}
			if (Function.ParseDouble(doc.BHeight2) > 0.0)
			{
				stringBuilder.Append(Function.GetJsonString("PurchaseUnitHeight1", Function.ParseDecimal(doc.BHeight2)));
			}
			if (Function.ParseInt(doc.BLen2Unit) > 0)
			{
				stringBuilder.Append(Function.GetJsonString("PurchaseLengthUnit1", Function.ParseInt(doc.BLen2Unit)));
			}
			if (Function.ParseDecimal(doc.Blength2) > 0m)
			{
				stringBuilder.Append(Function.GetJsonString("PurchaseUnitLength1", Function.ParseDecimal(doc.Blength2)));
			}
			if (Function.ParseInt(doc.BWght2Unit) > 0)
			{
				stringBuilder.Append(Function.GetJsonString("PurchaseWeightUnit1", Function.ParseInt(doc.BWght2Unit)));
			}
			if (Function.ParseDecimal(doc.BWeight2) > 0m)
			{
				stringBuilder.Append(Function.GetJsonString("PurchaseUnitWeight1", Function.ParseDecimal(doc.BWeight2)));
			}
			if (Function.ParseInt(doc.BWdth2Unit) > 0)
			{
				stringBuilder.Append(Function.GetJsonString("PurchaseWidthUnit1", Function.ParseInt(doc.BWdth2Unit)));
			}
			if (Function.ParseDecimal(doc.BWidth2) > 0m)
			{
				stringBuilder.Append(Function.GetJsonString("PurchaseUnitWidth1", Function.ParseDecimal(doc.BWidth2)));
			}
			if (Function.ParseInt(doc.SHght2Unit) > 0)
			{
				stringBuilder.Append(Function.GetJsonString("SalesHeightUnit1", Function.ParseInt(doc.SHght2Unit)));
			}
			if (Function.ParseDecimal(doc.SHeight2) > 0m)
			{
				stringBuilder.Append(Function.GetJsonString("SalesUnitHeight1", Function.ParseDecimal(doc.SHeight2)));
			}
			if (Function.ParseInt(doc.SLen2Unit) > 0)
			{
				stringBuilder.Append(Function.GetJsonString("SalesLengthUnit1", Function.ParseInt(doc.SLen2Unit)));
			}
			if (Function.ParseDecimal(doc.Slength2) > 0m)
			{
				stringBuilder.Append(Function.GetJsonString("SalesUnitLength1", Function.ParseDecimal(doc.Slength2)));
			}
			if (Function.ParseInt(doc.SWght2Unit) > 0)
			{
				stringBuilder.Append(Function.GetJsonString("SalesWeightUnit1", Function.ParseInt(doc.SWght2Unit)));
			}
			if (Function.ParseDecimal(doc.SWeight2) > 0m)
			{
				stringBuilder.Append(Function.GetJsonString("SalesUnitWeight1", Function.ParseDecimal(doc.SWeight2)));
			}
			if (Function.ParseInt(doc.SWdth2Unit) > 0)
			{
				stringBuilder.Append(Function.GetJsonString("SalesWidthUnit1", Function.ToString(doc.SWdth2Unit)));
			}
			if (Function.ParseDecimal(doc.SWidth2) > 0m)
			{
				stringBuilder.Append(Function.GetJsonString("SalesUnitWidth1", Function.ParseDecimal(doc.SWidth2)));
			}
			if (doc.BlockOut != null)
			{
				if (!string.IsNullOrWhiteSpace(doc.BlockOut) && doc.BlockOut.ToLower() == "n")
				{
					stringBuilder.Append(Function.GetJsonString("ForceSelectionOfSerialNumber", "tNO"));
				}
				else
				{
					stringBuilder.Append(Function.GetJsonString("ForceSelectionOfSerialNumber", "tYES"));
				}
			}
			if (doc.WTLiable != null || (!ignoreEmptyValue && commandType == EnumCommandType.Update))
			{
				if (!string.IsNullOrWhiteSpace(doc.WTLiable) && doc.WTLiable.ToLower() == "n")
				{
					stringBuilder.Append(Function.GetJsonString("WTLiable", "tNO"));
				}
				else
				{
					stringBuilder.Append(Function.GetJsonString("WTLiable", "tYES"));
				}
			}
			if (doc.EvalSystem != null)
			{
				BoInventorySystem boInventorySystem = BoInventorySystem.bis_MovingAverage;
				switch (doc.EvalSystem)
				{
				case "A":
					boInventorySystem = BoInventorySystem.bis_MovingAverage;
					break;
				case "S":
					boInventorySystem = BoInventorySystem.bis_Standard;
					break;
				case "F":
					boInventorySystem = BoInventorySystem.bis_FIFO;
					break;
				case "B":
					boInventorySystem = BoInventorySystem.bis_SNB;
					break;
				}
				stringBuilder.Append(Function.GetJsonString("CostAccountingMethod", Function.ToString(boInventorySystem)));
			}
			if (!string.IsNullOrWhiteSpace(doc.SWW))
			{
				stringBuilder.Append(Function.GetJsonString("SWW", Function.ToString(doc.SWW)));
			}
			if (!string.IsNullOrWhiteSpace(doc.WarrntTmpl))
			{
				stringBuilder.Append(Function.GetJsonString("WarrantyTemplate", Function.ToString(doc.WarrntTmpl)));
			}
			if (doc.IndirctTax != null)
			{
				if (!string.IsNullOrWhiteSpace(doc.IndirctTax) && doc.IndirctTax.ToLower() == "y")
				{
					stringBuilder.Append(Function.GetJsonString("IndirectTax", "tYES"));
				}
				else
				{
					stringBuilder.Append(Function.GetJsonString("IndirectTax", "tNO"));
				}
			}
			if (!string.IsNullOrWhiteSpace(doc.TaxCodeAR) || (!ignoreEmptyValue && commandType == EnumCommandType.Update))
			{
				stringBuilder.Append(Function.GetJsonString("ArTaxCode", Function.ToString(doc.TaxCodeAR)));
			}
			if (!string.IsNullOrWhiteSpace(doc.TaxCodeAP) || (!ignoreEmptyValue && commandType == EnumCommandType.Update))
			{
				stringBuilder.Append(Function.GetJsonString("ApTaxCode", Function.ToString(doc.TaxCodeAP)));
			}
			if (!string.IsNullOrWhiteSpace(doc.BaseUnit) || (!ignoreEmptyValue && commandType == EnumCommandType.Update))
			{
				stringBuilder.Append(Function.GetJsonString("BaseUnitName", Function.ToString(doc.BaseUnit)));
			}
			if (!string.IsNullOrWhiteSpace(doc.CountryOrg) || (!ignoreEmptyValue && commandType == EnumCommandType.Update))
			{
				stringBuilder.Append(Function.GetJsonString("ItemCountryOrg", Function.ToString(doc.CountryOrg)));
			}
			if (!string.IsNullOrWhiteSpace(doc.IssueMthd) || (!ignoreEmptyValue && commandType == EnumCommandType.Update))
			{
				BoIssueMethod boIssueMethod = ((doc.IssueMthd == "M") ? BoIssueMethod.im_Manual : BoIssueMethod.im_Backflush);
				stringBuilder.Append(Function.GetJsonString("IssueMethod", Function.ToString(boIssueMethod)));
			}
			if (!string.IsNullOrWhiteSpace(doc.Phantom) || (!ignoreEmptyValue && commandType == EnumCommandType.Update))
			{
				if (doc.Phantom == "Y")
				{
					stringBuilder.Append(Function.GetJsonString("IsPhantom", "tYES"));
				}
				else
				{
					stringBuilder.Append(Function.GetJsonString("IsPhantom", "tNO"));
				}
			}
			if (!string.IsNullOrWhiteSpace(doc.InvntryUom) || (!ignoreEmptyValue && commandType == EnumCommandType.Update))
			{
				stringBuilder.Append(Function.GetJsonString("InventoryUOM", Function.ToString(doc.InvntryUom)));
			}
			if (!string.IsNullOrEmpty(doc.PlaningSys) || (!ignoreEmptyValue && commandType == EnumCommandType.Update))
			{
				BoPlanningSystem boPlanningSystem = BoPlanningSystem.bop_None;
				string planingSys = doc.PlaningSys;
				if (!(planingSys == "M"))
				{
					if (planingSys == "N")
					{
						boPlanningSystem = BoPlanningSystem.bop_None;
					}
				}
				else
				{
					boPlanningSystem = BoPlanningSystem.bop_MRP;
				}
				stringBuilder.Append(Function.GetJsonString("PlanningSystem", boPlanningSystem));
			}
			if (!string.IsNullOrEmpty(doc.PrcrmntMtd))
			{
				BoProcurementMethod boProcurementMethod = BoProcurementMethod.bom_Buy;
				string planingSys = doc.PrcrmntMtd;
				if (!(planingSys == "B"))
				{
					if (planingSys == "M")
					{
						boProcurementMethod = BoProcurementMethod.bom_Make;
					}
				}
				else
				{
					boProcurementMethod = BoProcurementMethod.bom_Buy;
				}
				stringBuilder.Append(Function.GetJsonString("ProcurementMethod", boProcurementMethod));
				if (boProcurementMethod == BoProcurementMethod.bom_Make && doc.CompoWH != null)
				{
					BoMRPComponentWarehouse boMRPComponentWarehouse = BoMRPComponentWarehouse.bomcw_BOM;
					planingSys = doc.PrcrmntMtd;
					if (!(planingSys == "B"))
					{
						if (planingSys == "P")
						{
							boMRPComponentWarehouse = BoMRPComponentWarehouse.bomcw_Parent;
						}
					}
					else
					{
						boMRPComponentWarehouse = BoMRPComponentWarehouse.bomcw_BOM;
					}
					stringBuilder.Append(Function.GetJsonString("ComponentWarehouse", boMRPComponentWarehouse));
				}
			}
			if (doc.OrdrIntrvl > 0)
			{
				stringBuilder.Append(Function.GetJsonString("OrderIntervals", Function.ParseInt(doc.OrdrIntrvl)));
			}
			if (doc.OrdrMulti > 0.0)
			{
				stringBuilder.Append(Function.GetJsonString("OrderMultiple", Function.ParseDecimal(doc.OrdrMulti)));
			}
			if (doc.LeadTime != 0 || (!ignoreEmptyValue && commandType == EnumCommandType.Update))
			{
				stringBuilder.Append(Function.GetJsonString("LeadTime", Function.ParseInt(doc.LeadTime)));
			}
			if (Function.ParseDecimal(doc.MinOrdrQty) > 0m || (!ignoreEmptyValue && commandType == EnumCommandType.Update))
			{
				stringBuilder.Append(Function.GetJsonString("MinOrderQuantity", Function.ParseDecimal(doc.MinOrdrQty)));
			}
			ItemClassEnum itemClassEnum = ItemClassEnum.itcMaterial;
			if (!string.IsNullOrWhiteSpace(doc.ItemClass))
			{
				string planingSys = doc.ItemClass;
				if (!(planingSys == "2"))
				{
					if (planingSys == "1")
					{
						itemClassEnum = ItemClassEnum.itcService;
					}
				}
				else
				{
					itemClassEnum = ItemClassEnum.itcMaterial;
				}
				if (doc.ItemClass == "1" || doc.ItemClass == "2")
				{
					stringBuilder.Append(Function.GetJsonString("ItemClass", Function.ToString(itemClassEnum)));
				}
			}
			if (Function.ParseInt(doc.OSvcCode) > 0)
			{
				stringBuilder.Append(Function.GetJsonString("OutgoingServiceCode", Function.ParseInt(doc.OSvcCode)));
			}
			if (Function.ParseInt(doc.ISvcCode) > 0)
			{
				stringBuilder.Append(Function.GetJsonString("IncomingServiceCode", Function.ParseInt(doc.ISvcCode)));
			}
			if (Function.ParseInt(doc.ServiceGrp) > 0)
			{
				stringBuilder.Append(Function.GetJsonString("ServiceGroup", Function.ParseInt(doc.ServiceGrp)));
			}
			if (Function.ParseInt(doc.NCMCode) > 0)
			{
				stringBuilder.Append(Function.GetJsonString("NCMCode", Function.ParseInt(doc.NCMCode)));
			}
			if (doc.MatType != null)
			{
				stringBuilder.Append(Function.GetJsonString("MaterialType", Function.ToString(doc.MatType)));
			}
			if (Function.ParseInt(doc.MatGrp) > 0)
			{
				stringBuilder.Append(Function.GetJsonString("MaterialGroup", Function.ParseInt(doc.MatGrp)));
			}
			if (Function.ParseInt(doc.ProductSrc) > 0)
			{
				stringBuilder.Append(Function.GetJsonString("ProductSource", Function.ParseInt(doc.ProductSrc)));
			}
			if (Function.ParseInt(doc.DNFEntry) > 0)
			{
				stringBuilder.Append(Function.GetJsonString("DNFEntry", Function.ParseInt(doc.DNFEntry)));
			}
			if (!string.IsNullOrWhiteSpace(doc.Spec))
			{
				stringBuilder.Append(Function.GetJsonString("GTSItemSpec", Function.ToString(doc.Spec)));
			}
			if (!string.IsNullOrWhiteSpace(doc.TaxCtg))
			{
				stringBuilder.Append(Function.GetJsonString("GTSItemTaxCategory", Function.ToString(doc.TaxCtg)));
			}
			if (Function.ParseInt(doc.FuelCode) > 0)
			{
				stringBuilder.Append(Function.GetJsonString("FuelID", Function.ParseInt(doc.FuelCode)));
			}
			if (!string.IsNullOrWhiteSpace(doc.BeverTblC))
			{
				stringBuilder.Append(Function.GetJsonString("BeverageTableCode", Function.ToString(doc.BeverTblC)));
			}
			if (!string.IsNullOrWhiteSpace(doc.BeverGrpC))
			{
				stringBuilder.Append(Function.GetJsonString("BeverageGroupCode", Function.ToString(doc.BeverGrpC)));
			}
			if (Function.ParseInt(doc.BeverTM) > 0)
			{
				stringBuilder.Append(Function.GetJsonString("BeverageCommercialBrandCode", Function.ParseInt(doc.BeverTM)));
			}
			if (Function.ParseInt(doc.Series) > 0 && commandType == EnumCommandType.Add)
			{
				stringBuilder.Append(Function.GetJsonString("Series", Function.ToString(doc.Series)));
			}
			if (doc.ToleranDay != 0 || (!ignoreEmptyValue && commandType == EnumCommandType.Update))
			{
				stringBuilder.Append(Function.GetJsonString("ToleranceDays", Function.ParseInt(doc.ToleranDay)));
			}
			_ = doc.GLPickMeth;
			if (!string.IsNullOrWhiteSpace(doc.NoDiscount) || (!ignoreEmptyValue && commandType == EnumCommandType.Update))
			{
				stringBuilder.Append((doc.NoDiscount == "Y") ? Function.GetJsonString("NoDiscounts", "tYES") : Function.GetJsonString("NoDiscounts", "tNO"));
			}
			if (!string.IsNullOrWhiteSpace(doc.AssetClass))
			{
				stringBuilder.Append(Function.GetJsonString("AssetClass", Function.ToString(doc.AssetClass)));
			}
			if (!string.IsNullOrWhiteSpace(doc.AssetGroup))
			{
				stringBuilder.Append(Function.GetJsonString("AssetGroup", Function.ToString(doc.AssetGroup)));
			}
			if (!string.IsNullOrWhiteSpace(doc.InventryNo))
			{
				stringBuilder.Append(Function.GetJsonString("InventoryNumber", Function.ToString(doc.InventryNo)));
			}
			if (Function.ParseInt(doc.Technician) > 0)
			{
				stringBuilder.Append(Function.GetJsonString("Technician", Function.ParseInt(doc.Technician)));
			}
			if (Function.ParseInt(doc.Technician) > 0)
			{
				stringBuilder.Append(Function.GetJsonString("Employee", Function.ToString(doc.Technician)));
			}
			if (Function.ParseInt(doc.Location) > 0)
			{
				stringBuilder.Append(Function.GetJsonString("Location", Function.ToString(doc.Location)));
			}
			if (doc.CapDate.HasValue)
			{
				_ = doc.CapDate.Value;
				if (doc.CapDate.Value.Year > 2000)
				{
					stringBuilder.Append(Function.GetJsonString("CapitalizationDate", doc.CapDate.Value.ToString("yyyyMMdd")));
				}
			}
			if (doc.StatAsset != null)
			{
				if (!string.IsNullOrWhiteSpace(doc.StatAsset) && doc.StatAsset.ToLower() == "y")
				{
					stringBuilder.Append(Function.GetJsonString("StatisticalAsset", "tYES"));
				}
				else
				{
					stringBuilder.Append(Function.GetJsonString("StatisticalAsset", "tNO"));
				}
			}
			if (doc.Cession != null)
			{
				if (!string.IsNullOrWhiteSpace(doc.Cession) && doc.Cession.ToLower() == "y")
				{
					stringBuilder.Append(Function.GetJsonString("Cession", "tYES"));
				}
				else
				{
					stringBuilder.Append(Function.GetJsonString("Cession", "tNO"));
				}
			}
			if (doc.DeacAftUL != null)
			{
				if (!string.IsNullOrWhiteSpace(doc.DeacAftUL) && doc.DeacAftUL.ToLower() == "y")
				{
					stringBuilder.Append(Function.GetJsonString("DeactivateAfterUsefulLife", "tYES"));
				}
				else
				{
					stringBuilder.Append(Function.GetJsonString("DeactivateAfterUsefulLife", "tNO"));
				}
			}
			if (Function.ParseInt(doc.SVolUnit) > 0)
			{
				stringBuilder.Append(Function.GetJsonString("SalesVolumeUnit", Function.ToString(doc.SVolUnit)));
			}
			if (Function.ParseInt(doc.BVolUnit) > 0)
			{
				stringBuilder.Append(Function.GetJsonString("PurchaseVolumeUnit", Function.ToString(doc.BVolUnit)));
			}
			if (Function.ParseInt(doc.UgpEntry) > 0)
			{
				stringBuilder.Append(Function.GetJsonString("UoMGroupEntry", Function.ToString(doc.UgpEntry)));
				if (Function.ParseInt(doc.IUoMEntry) > 0)
				{
					stringBuilder.Append(Function.GetJsonString("InventoryUoMEntry", Function.ToString(doc.IUoMEntry)));
				}
				if (Function.ParseInt(doc.SUoMEntry) > 0)
				{
					stringBuilder.Append(Function.GetJsonString("DefaultSalesUoMEntry", Function.ToString(doc.SUoMEntry)));
					if (!string.IsNullOrWhiteSpace(doc.SalUnitMsr))
					{
						stringBuilder.Append(Function.GetJsonString("SalesUnit", Function.ToString(doc.SalUnitMsr)));
					}
					if (Function.ParseDecimal(doc.NumInSale) > 0m)
					{
						stringBuilder.Append(Function.GetJsonString("SalesItemsPerUnit", Function.ParseDecimal(doc.NumInSale)));
					}
					if (Function.ParseDecimal(doc.SLength1) >= 0m)
					{
						stringBuilder.Append(Function.GetJsonString("SalesUnitLength", Function.ParseDecimal(doc.SLength1)));
					}
					if (Function.ParseInt(doc.SLen1Unit) > 0)
					{
						stringBuilder.Append(Function.GetJsonString("SalesLengthUnit", Function.ToString(doc.SLen1Unit)));
					}
					if (Function.ParseDecimal(doc.SWidth1) >= 0m)
					{
						stringBuilder.Append(Function.GetJsonString("SalesUnitWidth", Function.ParseDecimal(doc.SWidth1)));
					}
					if (Function.ParseInt(doc.SWdth1Unit) > 0)
					{
						stringBuilder.Append(Function.GetJsonString("SalesWidthUnit", Function.ToString(doc.SWdth1Unit)));
					}
					if (Function.ParseDecimal(doc.SHeight1) >= 0m)
					{
						stringBuilder.Append(Function.GetJsonString("SalesUnitHeight", Function.ParseDecimal(doc.SHeight1)));
					}
					if (Function.ParseInt(doc.SHght1Unit) > 0)
					{
						stringBuilder.Append(Function.GetJsonString("SalesHeightUnit", Function.ToString(doc.SHght1Unit)));
					}
					if (Function.ParseDecimal(doc.SWeight1) >= 0m)
					{
						stringBuilder.Append(Function.GetJsonString("SalesUnitWeight", Function.ParseDecimal(doc.SWeight1)));
					}
					if (Function.ParseInt(doc.SWght1Unit) > 0)
					{
						stringBuilder.Append(Function.GetJsonString("SalesWeightUnit", Function.ToString(doc.SWght1Unit)));
					}
					if (Function.ParseDecimal(doc.SVolume) >= 0m)
					{
						stringBuilder.Append(Function.GetJsonString("SalesUnitVolume", Function.ParseDecimal(doc.SVolume)));
					}
				}
				if (Function.ParseInt(doc.PUoMEntry) > 0)
				{
					stringBuilder.Append(Function.GetJsonString("DefaultPurchasingUoMEntry", Function.ToString(doc.PUoMEntry)));
					if (!string.IsNullOrWhiteSpace(doc.BuyUnitMsr))
					{
						stringBuilder.Append(Function.GetJsonString("PurchaseUnit", Function.ToString(doc.BuyUnitMsr)));
					}
					if (Function.ParseDecimal(doc.BVolume) >= 0m)
					{
						stringBuilder.Append(Function.GetJsonString("PurchaseUnitVolume", Function.ParseDecimal(doc.BVolume)));
					}
					if (Function.ParseDecimal(doc.BLength1) >= 0m)
					{
						stringBuilder.Append(Function.GetJsonString("PurchaseUnitLength", Function.ParseDecimal(doc.BLength1)));
					}
					if (Function.ParseInt(doc.BLen1Unit) > 0)
					{
						stringBuilder.Append(Function.GetJsonString("PurchaseLengthUnit", Function.ToString(doc.BLen1Unit)));
					}
					if (Function.ParseDecimal(doc.BWidth1) >= 0m)
					{
						stringBuilder.Append(Function.GetJsonString("PurchaseUnitWidth", Function.ParseDecimal(doc.BWidth1)));
					}
					if (Function.ParseInt(doc.BWdth1Unit) > 0)
					{
						stringBuilder.Append(Function.GetJsonString("PurchaseWidthUnit", Function.ToString(doc.BWdth1Unit)));
					}
					if (Function.ParseDecimal(doc.BHeight1) >= 0m)
					{
						stringBuilder.Append(Function.GetJsonString("PurchaseUnitHeight", Function.ParseDecimal(doc.BHeight1)));
					}
					if (Function.ParseInt(doc.BHght1Unit) > 0)
					{
						stringBuilder.Append(Function.GetJsonString("PurchaseHeightUnit", Function.ToString(doc.BHght1Unit)));
					}
					if (Function.ParseDecimal(doc.BWeight1) >= 0m)
					{
						stringBuilder.Append(Function.GetJsonString("PurchaseUnitWeight", Function.ParseDecimal(doc.BWeight1)));
					}
					if (Function.ParseInt(doc.BWght1Unit) > 0)
					{
						stringBuilder.Append(Function.GetJsonString("PurchaseWeightUnit", Function.ToString(doc.BWght1Unit)));
					}
					if (Function.ParseDecimal(doc.NumInBuy) > 0m)
					{
						stringBuilder.Append(Function.GetJsonString("PurchaseItemsPerUnit", Function.ParseDecimal(doc.NumInBuy)));
					}
				}
			}
			else if (doc.UgpEntry != 0)
			{
				if (!string.IsNullOrWhiteSpace(doc.SalUnitMsr))
				{
					stringBuilder.Append(Function.GetJsonString("SalesUnit", Function.ToString(doc.SalUnitMsr)));
				}
				if (Function.ParseDecimal(doc.NumInSale) > 0m)
				{
					stringBuilder.Append(Function.GetJsonString("SalesItemsPerUnit", Function.ParseDecimal(doc.NumInSale)));
				}
				if (Function.ParseDecimal(doc.SVolume) >= 0m)
				{
					stringBuilder.Append(Function.GetJsonString("SalesUnitVolume", Function.ParseDecimal(doc.SVolume)));
				}
				if (Function.ParseDecimal(doc.SLength1) >= 0m)
				{
					stringBuilder.Append(Function.GetJsonString("SalesUnitLength", Function.ParseDecimal(doc.SLength1)));
				}
				if (Function.ParseInt(doc.SLen1Unit) > 0)
				{
					stringBuilder.Append(Function.GetJsonString("SalesLengthUnit", Function.ToString(doc.SLen1Unit)));
				}
				if (Function.ParseDecimal(doc.SWidth1) >= 0m)
				{
					stringBuilder.Append(Function.GetJsonString("SalesUnitWidth", Function.ParseDecimal(doc.SWidth1)));
				}
				if (Function.ParseInt(doc.SWdth1Unit) > 0)
				{
					stringBuilder.Append(Function.GetJsonString("SalesWidthUnit", Function.ToString(doc.SWdth1Unit)));
				}
				if (Function.ParseDecimal(doc.SHeight1) >= 0m)
				{
					stringBuilder.Append(Function.GetJsonString("SalesUnitHeight", Function.ParseDecimal(doc.SHeight1)));
				}
				if (Function.ParseInt(doc.SHght1Unit) > 0)
				{
					stringBuilder.Append(Function.GetJsonString("SalesHeightUnit", Function.ToString(doc.SHght1Unit)));
				}
				if (Function.ParseInt(doc.SVolUnit) > 0)
				{
					stringBuilder.Append(Function.GetJsonString("SalesVolumeUnit", Function.ToString(doc.SVolUnit)));
				}
				if (Function.ParseDecimal(doc.SWeight1) >= 0m)
				{
					stringBuilder.Append(Function.GetJsonString("SalesUnitWeight", Function.ParseDecimal(doc.SWeight1)));
				}
				if (Function.ParseInt(doc.SWght1Unit) > 0)
				{
					stringBuilder.Append(Function.GetJsonString("SalesWeightUnit", Function.ToString(doc.SWght1Unit)));
				}
				if (!string.IsNullOrWhiteSpace(doc.BuyUnitMsr))
				{
					stringBuilder.Append(Function.GetJsonString("PurchaseUnit", Function.ToString(doc.BuyUnitMsr)));
				}
				if (Function.ParseDecimal(doc.NumInBuy) > 0m)
				{
					stringBuilder.Append(Function.GetJsonString("PurchaseItemsPerUnit", Function.ParseDecimal(doc.NumInBuy)));
				}
				if (Function.ParseDecimal(doc.BVolume) > 0m)
				{
					stringBuilder.Append(Function.GetJsonString("PurchaseUnitVolume", Function.ParseDecimal(doc.BVolume)));
				}
				if (Function.ParseDecimal(doc.BLength1) >= 0m)
				{
					stringBuilder.Append(Function.GetJsonString("PurchaseUnitLength", Function.ParseDecimal(doc.BLength1)));
				}
				if (Function.ParseInt(doc.BLen1Unit) > 0)
				{
					stringBuilder.Append(Function.GetJsonString("PurchaseLengthUnit", Function.ToString(doc.BLen1Unit)));
				}
				if (Function.ParseDecimal(doc.BWidth1) >= 0m)
				{
					stringBuilder.Append(Function.GetJsonString("PurchaseUnitWidth", Function.ParseDecimal(doc.BWidth1)));
				}
				if (Function.ParseInt(doc.BWdth1Unit) > 0)
				{
					stringBuilder.Append(Function.GetJsonString("PurchaseWidthUnit", Function.ToString(doc.BWdth1Unit)));
				}
				if (Function.ParseDecimal(doc.BHeight1) >= 0m)
				{
					stringBuilder.Append(Function.GetJsonString("PurchaseUnitHeight", Function.ParseDecimal(doc.BHeight1)));
				}
				if (Function.ParseInt(doc.BHght1Unit) > 0)
				{
					stringBuilder.Append(Function.GetJsonString("PurchaseHeightUnit", Function.ToString(doc.BHght1Unit)));
				}
				if (Function.ParseDecimal(doc.BWeight1) >= 0m)
				{
					stringBuilder.Append(Function.GetJsonString("PurchaseUnitWeight", Function.ParseDecimal(doc.BWeight1)));
				}
				if (Function.ParseInt(doc.BWght1Unit) > 0)
				{
					stringBuilder.Append(Function.GetJsonString("PurchaseWeightUnit", Function.ToString(doc.BWght1Unit)));
				}
				stringBuilder.Append(Function.GetJsonString("UoMGroupEntry", -1));
			}
			if (!string.IsNullOrWhiteSpace(doc.DeprGroup))
			{
				stringBuilder.Append(Function.GetJsonString("DepreciationGroup", Function.ToString(doc.DeprGroup)));
			}
			if (!string.IsNullOrWhiteSpace(doc.AssetSerNo))
			{
				stringBuilder.Append(Function.GetJsonString("AssetSerialNumber", Function.ToString(doc.AssetSerNo)));
			}
			if (Function.ParseDecimal(doc.IWeight1) >= 0m)
			{
				stringBuilder.Append(Function.GetJsonString("InventoryWeight", Function.ParseDecimal(doc.IWeight1)));
			}
			if (Function.ParseInt(doc.IWght1Unit) > 0)
			{
				stringBuilder.Append(Function.GetJsonString("InventoryWeightUnit", Function.ToString(doc.IWght1Unit)));
			}
			if (Function.ParseDecimal(doc.IWeight2) >= 0m)
			{
				stringBuilder.Append(Function.GetJsonString("InventoryWeight1", Function.ParseDecimal(doc.IWeight2)));
			}
			if (Function.ParseInt(doc.IWght2Unit) > 0)
			{
				stringBuilder.Append(Function.GetJsonString("InventoryWeightUnit1", Function.ToString(doc.IWght2Unit)));
			}
			if (!string.IsNullOrWhiteSpace(doc.CntUnitMsr))
			{
				stringBuilder.Append(Function.GetJsonString("DefaultCountingUnit", Function.ToString(doc.CntUnitMsr)));
			}
			if (Function.ParseInt(doc.INUoMEntry) > 0)
			{
				stringBuilder.Append(Function.GetJsonString("DefaultCountingUoMEntry", Function.ToString(doc.INUoMEntry)));
			}
			if (doc.Excisable != null)
			{
				if (!string.IsNullOrWhiteSpace(doc.Excisable) && doc.Excisable.ToLower() == "y")
				{
					stringBuilder.Append(Function.GetJsonString("Excisable", "tYES"));
				}
				else
				{
					stringBuilder.Append(Function.GetJsonString("Excisable", "tNO"));
				}
			}
			if (Function.ParseInt(doc.ChapterID) > 0)
			{
				stringBuilder.Append(Function.GetJsonString("ChapterID", Function.ToString(doc.ChapterID)));
			}
			if (!string.IsNullOrWhiteSpace(doc.ScsCode))
			{
				stringBuilder.Append(Function.GetJsonString("ScsCode", Function.ToString(doc.ScsCode)));
			}
			if (!string.IsNullOrEmpty(doc.SpProdType))
			{
				SpecialProductTypeEnum specialProductTypeEnum = SpecialProductTypeEnum.sptMT;
				string planingSys = doc.SpProdType;
				if (!(planingSys == "MT"))
				{
					if (planingSys == "IO")
					{
						specialProductTypeEnum = SpecialProductTypeEnum.sptIO;
					}
				}
				else
				{
					specialProductTypeEnum = SpecialProductTypeEnum.sptMT;
				}
				stringBuilder.Append(Function.GetJsonString("SpProdType", specialProductTypeEnum));
			}
			if (!string.IsNullOrWhiteSpace(doc.CodeBars))
			{
				stringBuilder.Append(Function.GetJsonString("BarCode", Function.ToString(doc.CodeBars)));
			}
			if (!string.IsNullOrWhiteSpace(doc.SuppCatNum) || (!ignoreEmptyValue && commandType == EnumCommandType.Update))
			{
				stringBuilder.Append(Function.GetJsonString("SupplierCatalogNo", Function.ToString(doc.SuppCatNum)));
			}
			if (Function.ParseDecimal(doc.ReorderQty) > 0m)
			{
				stringBuilder.Append(Function.GetJsonString("PurchaseQtyPerPackUnit", Function.ParseDecimal(doc.ReorderQty)));
			}
			if (Function.ParseDecimal(doc.MinLevel) > 0m)
			{
				stringBuilder.Append(Function.GetJsonString("MinInventory", Function.ParseDecimal(doc.MinLevel)));
			}
			stringBuilder.Append(Function.GetJsonString("User_Text", Function.ToString(doc.UserText)));
			if (!string.IsNullOrEmpty(doc.TreeType))
			{
				BoItemTreeTypes boItemTreeTypes = BoItemTreeTypes.iNotATree;
				stringBuilder.Append(Function.GetJsonString("TreeType", Function.ToString(doc.TreeType switch
				{
					"N" => BoItemTreeTypes.iNotATree, 
					"A" => BoItemTreeTypes.iAssemblyTree, 
					"S" => BoItemTreeTypes.iSalesTree, 
					"P" => BoItemTreeTypes.iProductionTree, 
					"T" => BoItemTreeTypes.iTemplateTree, 
					"I" => BoItemTreeTypes.iIngredient, 
					_ => BoItemTreeTypes.iNotATree, 
				})));
			}
			else
			{
				stringBuilder.Append(Function.GetJsonString("TreeType", "iNotATree"));
			}
			if (Function.ParseDecimal(doc.MaxLevel) > 0m)
			{
				stringBuilder.Append(Function.GetJsonString("MaxInventory", Function.ParseDecimal(doc.MaxLevel)));
			}
			if (Function.ParseDecimal(doc.AvgPrice) > 0m)
			{
				stringBuilder.Append(Function.GetJsonString("AvgStdPrice", Function.ParseDecimal(doc.AvgPrice)));
			}
			if (!string.IsNullOrWhiteSpace(doc.PurPackMsr))
			{
				stringBuilder.Append(Function.GetJsonString("PurchasePackagingUnit", Function.ToString(doc.PurPackMsr)));
			}
			if (!string.IsNullOrWhiteSpace(doc.SalPackMsr))
			{
				stringBuilder.Append(Function.GetJsonString("SalesPackagingUnit", Function.ToString(doc.SalPackMsr)));
			}
			if (Function.ParseDecimal(doc.SalPackUn) > 0m)
			{
				stringBuilder.Append(Function.GetJsonString("SalesQtyPerPackUnit", Function.ToString(doc.SalPackUn)));
			}
			if (!string.IsNullOrEmpty(doc.GLMethod))
			{
				BoGLMethods boGLMethods = BoGLMethods.glm_WH;
				stringBuilder.Append(Function.GetJsonString("GLMethod", Function.ToString(doc.GLMethod switch
				{
					"W" => BoGLMethods.glm_WH, 
					"C" => BoGLMethods.glm_ItemClass, 
					"L" => BoGLMethods.glm_ItemLevel, 
					_ => BoGLMethods.glm_WH, 
				})));
			}
			if (doc.ItemProperties != null && doc.ItemProperties.Count > 0)
			{
				for (int i = 0; i < doc.ItemProperties.Count; i++)
				{
					BoYesNoEnum boYesNoEnum2 = ((doc.ItemProperties[i] == "Y") ? BoYesNoEnum.tYES : BoYesNoEnum.tNO);
					stringBuilder.Append(Function.GetJsonString("Properties" + (i + 1), boYesNoEnum2));
				}
			}
			if (Function.ParseInt(doc.AtcEntry) > 0)
			{
				stringBuilder.Append(Function.GetJsonString("AttachmentEntry", doc.AtcEntry));
			}
			else
			{
				stringBuilder.Append(Function.GetJsonString("AttachmentEntry", null));
			}
			stringBuilder.Append(DataService.MappingUdf(doc.ListUserFields, commandType));
			if (commandType == EnumCommandType.Update)
			{
				stringBuilder.Append("\"ItemPrices\":[");
				if (doc.ListITM1 != null)
				{
					for (int j = 0; j < doc.ListITM1.Count; j++)
					{
						stringBuilder.Append("{");
						if (doc.ListITM1[j].PriceList > 0)
						{
							stringBuilder.Append(Function.GetJsonString("PriceList", Function.ToString(doc.ListITM1[j].PriceList)));
						}
						stringBuilder.Append(Function.GetJsonString("Price", Function.ParseDecimal(doc.ListITM1[j].Price)));
						if (!string.IsNullOrWhiteSpace(doc.ListITM1[j].Currency) && Function.ParseDouble(doc.ListITM1[j].Price) > 0.0)
						{
							stringBuilder.Append(Function.GetJsonString("Currency", Function.ToString(doc.ListITM1[j].Currency)));
						}
						stringBuilder.Append(Function.GetJsonString("AdditionalPrice1", Function.ParseDecimal(doc.ListITM1[j].AddPrice1)));
						if (!string.IsNullOrWhiteSpace(doc.ListITM1[j].Currency1) && Function.ParseDecimal(doc.ListITM1[j].AddPrice1) > 0m)
						{
							stringBuilder.Append(Function.GetJsonString("AdditionalCurrency1", Function.ToString(doc.ListITM1[j].Currency1)));
						}
						stringBuilder.Append(Function.GetJsonString("AdditionalPrice2", Function.ParseDecimal(doc.ListITM1[j].AddPrice2)));
						if (!string.IsNullOrWhiteSpace(doc.ListITM1[j].Currency2) && Function.ParseDouble(doc.ListITM1[j].AddPrice2) > 0.0)
						{
							stringBuilder.Append(Function.GetJsonString("AdditionalCurrency2", Function.ToString(doc.ListITM1[j].Currency2)));
						}
						if (doc.ListITM1[j].BasePLNum > 0)
						{
							stringBuilder.Append(Function.GetJsonString("BasePriceList", Function.ToString(doc.ListITM1[j].BasePLNum)));
						}
						if (doc.ListITM1[j].Factor > 0.0)
						{
							stringBuilder.Append(Function.GetJsonString("Factor", Function.ToString(doc.ListITM1[j].Factor)));
						}
						stringBuilder.Append("},");
					}
				}
				stringBuilder.Append("],");
			}
			if ((commandType == EnumCommandType.Update || !IsAutoAddWarehouse() || (commandType == EnumCommandType.Add && Function.ToString(doc.GLMethod).ToUpper() == "L")) && doc.OitwLines != null && doc.OitwLines.Count > 0 && doc.OitwLines.Any((OITW_Impl t) => !string.IsNullOrWhiteSpace(t.WhsCode)))
			{
				stringBuilder.Append("\"ItemWarehouseInfoCollection\":[");
				for (int num = 0; num < doc.OitwLines.Count; num++)
				{
					stringBuilder.Append("{");
					stringBuilder.Append(Function.GetJsonString("WarehouseCode", Function.ToString(doc.OitwLines[num].WhsCode)));
					BoYesNoEnum boYesNoEnum3 = ((Function.ToString(doc.OitwLines[num].Locked) == "Y") ? BoYesNoEnum.tYES : BoYesNoEnum.tNO);
					stringBuilder.Append(Function.GetJsonString("Locked", boYesNoEnum3));
					if (doc.ByWh == "Y")
					{
						stringBuilder.Append(Function.GetJsonString("MaximalStock", doc.OitwLines[num].MaxStock));
						stringBuilder.Append(Function.GetJsonString("MinimalOrder", doc.OitwLines[num].MinOrder));
						stringBuilder.Append(Function.GetJsonString("MinimalStock", doc.OitwLines[num].MinStock));
					}
					if (doc.OitwLines[num].DftBinAbs != 0)
					{
						stringBuilder.Append(Function.GetJsonString("DefaultBin", doc.OitwLines[num].DftBinAbs));
					}
					if (commandType == EnumCommandType.Add && doc.GLMethod.ToUpper() == "L")
					{
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].SaleCostAc))
						{
							stringBuilder.Append(Function.GetJsonString("CostAccount", doc.OitwLines[num].SaleCostAc));
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].CostRvlAct))
						{
							stringBuilder.Append(Function.GetJsonString("CostInflationAccount", doc.OitwLines[num].CostRvlAct));
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].CstOffsAct))
						{
							stringBuilder.Append(Function.GetJsonString("CostInflationOffsetAccount", doc.OitwLines[num].CstOffsAct));
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].DecreasAc))
						{
							stringBuilder.Append(Function.GetJsonString("DecreasingAccount", doc.OitwLines[num].DecreasAc));
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].EUExpensAc))
						{
							stringBuilder.Append(Function.GetJsonString("EUExpensesAccount", doc.OitwLines[num].EUExpensAc));
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].APCMEUAct))
						{
							stringBuilder.Append(Function.GetJsonString("EUPurchaseCreditAcc", doc.OitwLines[num].APCMEUAct));
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].EURevenuAc))
						{
							stringBuilder.Append(Function.GetJsonString("EURevenuesAccount", doc.OitwLines[num].EURevenuAc));
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].ExchangeAc))
						{
							stringBuilder.Append(Function.GetJsonString("ExchangeRateDifferencesAcct", doc.OitwLines[num].ExchangeAc));
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].ARCMExpAct))
						{
							stringBuilder.Append(Function.GetJsonString("ExemptedCredits", doc.OitwLines[num].ARCMExpAct));
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].ExmptIncom))
						{
							stringBuilder.Append(Function.GetJsonString("ExemptIncomeAcc", doc.OitwLines[num].ExmptIncom));
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].ExpClrAct))
						{
							stringBuilder.Append(Function.GetJsonString("ExpenseClearingAct", doc.OitwLines[num].ExpClrAct));
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].ARCMExpAct))
						{
							stringBuilder.Append(Function.GetJsonString("ExemptedCredits", doc.OitwLines[num].ARCMExpAct));
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].ExmptIncom))
						{
							stringBuilder.Append(Function.GetJsonString("ExemptIncomeAcc", doc.OitwLines[num].ExmptIncom));
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].ExpClrAct))
						{
							stringBuilder.Append(Function.GetJsonString("ExpenseClearingAct", doc.OitwLines[num].ExpClrAct));
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].ExpOfstAct))
						{
							stringBuilder.Append(Function.GetJsonString("ExpenseOffsettingAccount", doc.OitwLines[num].ExpOfstAct));
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].ExpensesAc))
						{
							stringBuilder.Append(Function.GetJsonString("ExpensesAccount", doc.OitwLines[num].ExpensesAc));
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].FrExpensAc))
						{
							stringBuilder.Append(Function.GetJsonString("ForeignExpensAcc", doc.OitwLines[num].FrExpensAc));
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].APCMFrnAct))
						{
							stringBuilder.Append(Function.GetJsonString("ForeignPurchaseCreditAcc", doc.OitwLines[num].APCMFrnAct));
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].FrRevenuAc))
						{
							stringBuilder.Append(Function.GetJsonString("ForeignRevenueAcc", doc.OitwLines[num].FrRevenuAc));
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].DecresGlAc))
						{
							stringBuilder.Append(Function.GetJsonString("GLDecreaseAcct", doc.OitwLines[num].DecresGlAc));
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].IncresGlAc))
						{
							stringBuilder.Append(Function.GetJsonString("GLIncreaseAcct", doc.OitwLines[num].IncresGlAc));
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].BalanceAcc))
						{
							stringBuilder.Append(Function.GetJsonString("GoodsClearingAcct", doc.OitwLines[num].BalanceAcc));
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].IncreasAc))
						{
							stringBuilder.Append(Function.GetJsonString("IncreasingAccount", doc.OitwLines[num].IncreasAc));
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].BalInvntAc))
						{
							stringBuilder.Append(Function.GetJsonString("InventoryAccount", doc.OitwLines[num].BalInvntAc));
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].StockOffst))
						{
							stringBuilder.Append(Function.GetJsonString("InventoryOffsetProfitAndLossAccount", doc.OitwLines[num].StockOffst));
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].NegStckAct))
						{
							stringBuilder.Append(Function.GetJsonString("NegativeInventoryAdjustmentAccount", doc.OitwLines[num].NegStckAct));
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].PAReturnAc))
						{
							stringBuilder.Append(Function.GetJsonString("PAReturnAcct", doc.OitwLines[num].PAReturnAc));
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].PriceDifAc))
						{
							stringBuilder.Append(Function.GetJsonString("PriceDifferenceAcc", doc.OitwLines[num].PriceDifAc));
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].PurchaseAc))
						{
							stringBuilder.Append(Function.GetJsonString("PurchaseAcct", doc.OitwLines[num].PurchaseAc));
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].PurBalAct))
						{
							stringBuilder.Append(Function.GetJsonString("PurchaseBalanceAccount", doc.OitwLines[num].PurBalAct));
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].APCMAct))
						{
							stringBuilder.Append(Function.GetJsonString("PurchaseCreditAcc", doc.OitwLines[num].APCMAct));
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].PurchOfsAc))
						{
							stringBuilder.Append(Function.GetJsonString("PurchaseOffsetAcct", doc.OitwLines[num].PurchOfsAc));
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].ReturnAc))
						{
							stringBuilder.Append(Function.GetJsonString("ReturningAccount", doc.OitwLines[num].ReturnAc));
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].RevenuesAc))
						{
							stringBuilder.Append(Function.GetJsonString("RevenuesAccount", doc.OitwLines[num].RevenuesAc));
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].ARCMAct))
						{
							stringBuilder.Append(Function.GetJsonString("SalesCreditAcc", doc.OitwLines[num].ARCMAct));
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].ARCMEUAct))
						{
							stringBuilder.Append(Function.GetJsonString("SalesCreditEUAcc", doc.OitwLines[num].ARCMEUAct));
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].ARCMFrnAct))
						{
							stringBuilder.Append(Function.GetJsonString("SalesCreditForeignAcc", doc.OitwLines[num].ARCMFrnAct));
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].ShpdGdsAct))
						{
							stringBuilder.Append(Function.GetJsonString("ShippedGoodsAccount", doc.OitwLines[num].ShpdGdsAct));
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].StokRvlAct))
						{
							stringBuilder.Append(Function.GetJsonString("StockInflationAdjustAccount", doc.OitwLines[num].StokRvlAct));
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].StkOffsAct))
						{
							stringBuilder.Append(Function.GetJsonString("StockInflationOffsetAccount", doc.OitwLines[num].StkOffsAct));
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].StkInTnAct))
						{
							stringBuilder.Append(Function.GetJsonString("StockInTransitAccount", doc.OitwLines[num].StkInTnAct));
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].TransferAc))
						{
							stringBuilder.Append(Function.GetJsonString("TransferAccount", doc.OitwLines[num].TransferAc));
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].VarianceAc))
						{
							stringBuilder.Append(Function.GetJsonString("VarienceAccount", doc.OitwLines[num].VarianceAc));
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].VatRevAct))
						{
							stringBuilder.Append(Function.GetJsonString("VATInRevenueAccount", doc.OitwLines[num].VatRevAct));
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].WhICenAct))
						{
							stringBuilder.Append(Function.GetJsonString("WHIncomingCenvatAccount", doc.OitwLines[num].WhICenAct));
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].WhOCenAct))
						{
							stringBuilder.Append(Function.GetJsonString("WHOutgoingCenvatAccount", doc.OitwLines[num].WhOCenAct));
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].WipAcct))
						{
							stringBuilder.Append(Function.GetJsonString("WipAccount", doc.OitwLines[num].WipAcct));
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].WipOffset))
						{
							stringBuilder.Append(Function.GetJsonString("WipOffsetProfitAndLossAccount", doc.OitwLines[num].WipOffset));
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].WipVarAcct))
						{
							stringBuilder.Append(Function.GetJsonString("WipVarianceAccount", doc.OitwLines[num].WipVarAcct));
						}
					}
					stringBuilder.Append("},");
				}
				stringBuilder.Append("],");
			}
			if (doc.ListITM12 != null && doc.ListITM12.Count > 0)
			{
				stringBuilder.Append("\"ItemUnitOfMeasurementCollection\":[");
				for (int num2 = 0; num2 < doc.ListITM12.Count; num2++)
				{
					stringBuilder.Append("{");
					ItemUoMTypeEnum itemUoMTypeEnum = ItemUoMTypeEnum.iutPurchasing;
					switch (doc.ListITM12[num2].UomType)
					{
					case "P":
						itemUoMTypeEnum = ItemUoMTypeEnum.iutPurchasing;
						break;
					case "S":
						itemUoMTypeEnum = ItemUoMTypeEnum.iutSales;
						break;
					case "I":
						itemUoMTypeEnum = ItemUoMTypeEnum.iutInventory;
						break;
					}
					stringBuilder.Append(Function.GetJsonString("UoMType", Function.ToString(itemUoMTypeEnum)));
					stringBuilder.Append(Function.GetJsonString("UoMEntry", Function.ToString(doc.ListITM12[num2].UomEntry)));
					if (doc.ListITM12[num2].UomType == "I")
					{
						stringBuilder.Append(Function.GetJsonString("Length1", 0.0));
						stringBuilder.Append(Function.GetJsonString("Length2", 0.0));
						stringBuilder.Append(Function.GetJsonString("Width1", 0.0));
						stringBuilder.Append(Function.GetJsonString("Height1", 0.0));
						stringBuilder.Append(Function.GetJsonString("Height2", 0.0));
						stringBuilder.Append(Function.GetJsonString("Volume", 0.0));
						stringBuilder.Append(Function.GetJsonString("Weight1", 0.0));
						stringBuilder.Append(Function.GetJsonString("Weight2", 0.0));
					}
					else
					{
						stringBuilder.Append(Function.GetJsonString("Length1", Function.ParseDecimal(doc.ListITM12[num2].Length1)));
						stringBuilder.Append(Function.GetJsonString("Length1Unit", Function.ToString(doc.ListITM12[num2].Len1Unit)));
						stringBuilder.Append(Function.GetJsonString("Length2", Function.ParseDecimal(doc.ListITM12[num2].Length2)));
						stringBuilder.Append(Function.GetJsonString("Width1", Function.ParseDecimal(doc.ListITM12[num2].Width1)));
						stringBuilder.Append(Function.GetJsonString("Width1Unit", Function.ToString(doc.ListITM12[num2].Wdth1Unit)));
						stringBuilder.Append(Function.GetJsonString("Width2", Function.ParseDecimal(doc.ListITM12[num2].Width2)));
						stringBuilder.Append(Function.GetJsonString("Height1", Function.ParseDecimal(doc.ListITM12[num2].Height1)));
						stringBuilder.Append(Function.GetJsonString("Height1Unit", Function.ToString(doc.ListITM12[num2].Hght1Unit)));
						stringBuilder.Append(Function.GetJsonString("Height2", Function.ParseDecimal(doc.ListITM12[num2].Height2)));
						stringBuilder.Append(Function.GetJsonString("Volume", Function.ParseDecimal(doc.ListITM12[num2].Volume)));
						stringBuilder.Append(Function.GetJsonString("VolumeUnit", Function.ToString(doc.ListITM12[num2].VolUnit)));
						stringBuilder.Append(Function.GetJsonString("Weight1", Function.ParseDecimal(doc.ListITM12[num2].Weight1)));
						stringBuilder.Append(Function.GetJsonString("Weight1Unit", Function.ToString(doc.ListITM12[num2].Wght1Unit)));
						stringBuilder.Append(Function.GetJsonString("Weight2", Function.ParseDecimal(doc.ListITM12[num2].Weight2)));
					}
					stringBuilder.Append("},");
				}
				stringBuilder.Append("],");
			}
			if (doc.ListItemPreferredVendors != null && doc.ListItemPreferredVendors.Count > 0)
			{
				stringBuilder.Append("\"ItemPreferredVendors\":[");
				for (int num3 = 0; num3 < doc.ListItemPreferredVendors.Count; num3++)
				{
					stringBuilder.Append("{");
					stringBuilder.Append(Function.GetJsonString("BPCode", Function.ToString(doc.ListItemPreferredVendors[num3].VendorCode)));
					stringBuilder.Append("},");
				}
				stringBuilder.Append("],");
			}
			stringBuilder.Append("}");
			goto end_IL_0006;
			IL_07cb:
			stringBuilder.Append(Function.GetJsonString("ValidFrom", value6));
			string value8;
			if (doc.validTo.HasValue)
			{
				_ = doc.validTo.Value;
				if (doc.validTo.Value.Year > 2000)
				{
					value8 = doc.validTo.Value.ToString("yyyyMMdd");
					goto IL_0840;
				}
			}
			value8 = "";
			goto IL_0840;
			IL_091b:
			string value9;
			stringBuilder.Append(Function.GetJsonString("FrozenFrom", value9));
			if (doc.frozenTo.HasValue)
			{
				_ = doc.frozenTo.Value;
				if (doc.frozenTo.Value.Year > 2000)
				{
					value7 = doc.frozenTo.Value.ToString("yyyyMMdd");
					goto IL_0990;
				}
			}
			value7 = "";
			goto IL_0990;
			IL_0840:
			stringBuilder.Append(Function.GetJsonString("ValidTo", value8));
			stringBuilder.Append(Function.GetJsonString("ValidRemarks", Function.ToString(doc.ValidComm)));
			goto IL_0867;
			IL_0867:
			BoYesNoEnum boYesNoEnum4 = ((doc.frozenFor == "Y") ? BoYesNoEnum.tYES : BoYesNoEnum.tNO);
			stringBuilder.Append(Function.GetJsonString("Frozen", Function.ToString(boYesNoEnum4)));
			if (doc.frozenFor == "Y")
			{
				if (doc.frozenFrom.HasValue)
				{
					_ = doc.frozenFrom.Value;
					if (doc.frozenFrom.Value.Year > 2000)
					{
						value9 = doc.frozenFrom.Value.ToString("yyyyMMdd");
						goto IL_091b;
					}
				}
				value9 = "";
				goto IL_091b;
			}
			goto IL_09b7;
			end_IL_0006:;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		return stringBuilder.ToString();
	}

	public override string UpdateUoMPrices(List<Items_UoMPricesImpl> lst, string sessionId)
	{
		try
		{
			short priceList = lst[0].PriceList;
			string itemCode = lst[0].ItemCode;
			if (string.IsNullOrEmpty(itemCode) || priceList < 1)
			{
				return "Not enough information";
			}
			HttpResult dataObjectFromService = DataService.GetDataObjectFromService(ApzObjectTypes.oItems, sessionId, Enums.HttpAction.Get, lst[0].ItemCode);
			if (!string.IsNullOrEmpty(dataObjectFromService.Message))
			{
				return dataObjectFromService.Message;
			}
			if (string.IsNullOrEmpty(Function.ToString(dataObjectFromService.Content)))
			{
				return "[UpdateUoMPrices].[GetByPr] No matching record found.";
			}
			dynamic val = JsonConvert.DeserializeObject(Function.ToString(dataObjectFromService.Content));
			List<JObject> list = new List<JObject>();
			if (val.ItemPrices != null)
			{
				for (int i = 0; i < val.ItemPrices.Count; i++)
				{
					list.Add(val.ItemPrices[i]);
				}
			}
			dynamic val2 = list.FirstOrDefault((JObject t) => Function.ParseInt(t["PriceList"]) == Function.ParseInt(priceList));
			if (val2 == null)
			{
				return " Can not update Uom Price";
			}
			List<JObject> uomPrices = new List<JObject>();
			if (val2.UoMPrices != null)
			{
				for (int num = 0; num < val2.UoMPrices.Count; num++)
				{
					uomPrices.Add(val2.UoMPrices[num]);
				}
			}
			int i2;
			for (i2 = uomPrices.Count - 1; i2 > -1; i2--)
			{
				if (!lst.Exists((Items_UoMPricesImpl t) => t.UomEntry == Function.ParseInt(uomPrices[i2]["UoMEntry"])))
				{
					uomPrices.RemoveAt(i2);
				}
			}
			int i3;
			for (i3 = 0; i3 < lst.Count; i3++)
			{
				if (lst[i3].UomEntry > 0)
				{
					dynamic val3 = uomPrices.FirstOrDefault((JObject t) => Function.ParseInt(t["UoMEntry"]) == lst[i3].UomEntry);
					if (val3 == null)
					{
						val3 = new JObject();
						uomPrices.Add(val3);
					}
					val3.PriceList = lst[i3].PriceList;
					val3.UoMEntry = lst[i3].UomEntry;
					val3.Price = lst[i3].Price;
					if (!string.IsNullOrWhiteSpace(lst[i3].Currency))
					{
						val3.Currency = lst[i3].Currency;
					}
					val3.AdditionalPrice1 = lst[i3].AddPrice1;
					if (!string.IsNullOrEmpty(lst[i3].Currency1))
					{
						val3.AdditionalCurrency1 = lst[i3].Currency1;
					}
					val3.AdditionalPrice2 = lst[i3].AddPrice2;
					if (!string.IsNullOrEmpty(lst[i3].Currency2))
					{
						val3.AdditionalCurrency2 = lst[i3].Currency2;
					}
					string text = ((Function.ToString(lst[i3].AutoUpdate) == "Y") ? "tYES" : "tNO");
					val3.Auto = text;
				}
			}
			val2.UoMPrices = new JArray(uomPrices);
			string xmlObject = "{\"ItemPrices\":" + JsonConvert.SerializeObject(list) + "}";
			HttpResult httpResult = DataService.UpdateObject(ApzObjectTypes.oItems, sessionId, xmlObject, itemCode);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				return httpResult.Message;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
		return string.Empty;
	}

	public override string Remove(string itemCode, string sessionId)
	{
		try
		{
			List<BodyRequestObject> list = new List<BodyRequestObject>();
			list.Add(new BodyRequestObject
			{
				ObjectType = ApzObjectTypes.oItems,
				OutObjectKey = true,
				Key = itemCode,
				Action = Enums.HttpAction.Delete
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
			return ex.Message;
		}
	}
}
