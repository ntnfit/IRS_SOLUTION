using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.DataServices.Interfaces;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.MasterData;
using Apzon.Services;
using SAPbobsCOM;

namespace Apzon.DataServices.Implements;

public class ItemsRepository : Repository<ItemsImpl>, Apzon.DataServices.Interfaces.IItems, IRepository<ItemsImpl>, IDisposable
{
	protected readonly IUnitOfWork _unitOfWork;

	protected readonly Apzon.Services.IUnitOfWork _servUnitOfWork;

	public ItemsRepository(Apzon.Services.IUnitOfWork servUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(servUnitOfWork, dataService)
	{
		_unitOfWork = unitOfWork;
		_servUnitOfWork = servUnitOfWork;
	}

	public virtual string Save(ItemsImpl doc, string sessionId, bool ignoreEmptyValue = false)
	{
		try
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("ItemCode,string", doc.ItemCode);
			HttpResult httpResult = ServiceUnitOfWork.Files.SaveAttachmentFiles(doc.AttachmentFiles, dictionary, "OITM");
			if (httpResult.MessageCode != Enums.MessageCode.Success)
			{
				return httpResult.Message;
			}
			doc.AtcEntry = Function.ParseInt(httpResult.Content);
			string text = "";
			if (doc.APZ_SAVE_ACTION.ToUpper() != "A" && doc.APZ_SAVE_ACTION.ToUpper() != "U" && doc.APZ_SAVE_ACTION.ToUpper() != "S")
			{
				return "Invalid APZ_SAVE_ACTION. Valid value is 'A'-Add, 'U'-Update, 'S'-Add Or Update";
			}
			EnumCommandType enumCommandType = ((doc.APZ_SAVE_ACTION.ToUpper() == "A") ? EnumCommandType.Add : EnumCommandType.Update);
			string text2 = ((doc.APZ_SAVE_ACTION.ToUpper() == "A") ? "AddObject" : "UpdateObject");
			if (doc.APZ_SAVE_ACTION.ToUpper() == "S")
			{
				if (Exists(doc.ItemCode))
				{
					enumCommandType = EnumCommandType.Update;
					text2 = "UpdateObject";
				}
				else
				{
					enumCommandType = EnumCommandType.Add;
					text2 = "AddObject";
				}
			}
			string value = MappingDIS(doc, enumCommandType, sessionId, ignoreEmptyValue);
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			if (enumCommandType == EnumCommandType.Update && !doc.ItemCode.Equals(doc.OldItemCode))
			{
				text2 = "AddObject";
				enumCommandType = EnumCommandType.Add;
				stringBuilder2.Append("<dis:RemoveObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">\r\n                                                <BOM>\r\n                                                  <BO>\r\n                                                    <AdmInfo>`\r\n                                                        <Object>oItems</Object>\r\n                                                    </AdmInfo>\r\n                                                    <QueryParams><ItemCode>" + doc.OldItemCode + "</ItemCode></QueryParams>\r\n                                                  </BO>\r\n                                                </BOM>\r\n                                                </dis:RemoveObject>");
			}
			stringBuilder.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n                            <env:Envelope xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                            <env:Header>\r\n                            <SessionID>" + sessionId + "</SessionID>\r\n                            </env:Header>\r\n                            <env:Body>" + stringBuilder2?.ToString() + "<dis:" + text2 + " xmlns:dis=\"http://www.sap.com/SBO/DIS\">");
			stringBuilder.Append(value);
			stringBuilder.Append("</dis:" + text2 + " >\r\n                            </env:Body>\r\n                            </env:Envelope>");
			XmlElement xmlElement = DataService.Process(stringBuilder.ToString()).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					text = xElement2.Value;
					Logging.Write("ERROR", text2 + " Item master error: " + xElement2.Value);
				}
			}
			else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
			{
				XElement xElement3 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("RetKey"));
				if (xElement3 != null)
				{
					string text3 = Function.ToString(xElement3.Value);
					if (doc.AtcEntry > 0)
					{
						text = UpdateAtcEntry(text3, doc.AtcEntry);
					}
					if (!string.IsNullOrWhiteSpace(text))
					{
						Logging.Write("ERROR", string.Format(text2 + " Item master-[{0}] success. But update AtcEntry Error-[{1}]", text3, text));
					}
					else
					{
						Logging.Write("WATCH", string.Format(text2 + " Item master - {0}", text3));
					}
					if (!ignoreEmptyValue && enumCommandType == EnumCommandType.Update)
					{
						DeleteUoMInItem(doc.ListITM12, sessionId, doc.ItemCode);
					}
					if (!ignoreEmptyValue && enumCommandType == EnumCommandType.Update)
					{
						DeletePreferredVendors(doc.ListItemPreferredVendors, sessionId, doc.ItemCode);
					}
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

	public bool Exists(string itemcode)
	{
		try
		{
			string query = "SELECT 1 FROM OITM WHERE \"ItemCode\"=@ItemCode;";
			return ExecuteDataTable(query, CommandType.Text, new SqlParameter("@ItemCode", itemcode)).IsNotNull();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		return false;
	}

	protected virtual string MappingDIS(ItemsImpl doc, EnumCommandType commandType, string sessionId = "", bool ignoreEmptyValue = false)
	{
		StringBuilder stringBuilder = new StringBuilder();
		try
		{
			stringBuilder.Append("<BOM>\r\n                          <BO xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                          <AdmInfo>\r\n                            <Object>oItems</Object>\r\n                          </AdmInfo>");
			if (commandType == EnumCommandType.Update)
			{
				stringBuilder.Append("<QueryParams><ItemCode>" + Function.RawText(doc.ItemCode) + "</ItemCode></QueryParams>");
			}
			stringBuilder.Append("<Items>\r\n                            <row>");
			if (commandType == EnumCommandType.Update || (!string.IsNullOrWhiteSpace(doc.ItemCode) && commandType == EnumCommandType.Add && (_unitOfWork.Common.GetIsManualSerial(doc.Series) || !_unitOfWork.Common.ExistsSeries(ApzObjectTypes.oItems, doc.Series))))
			{
				stringBuilder.Append("<ItemCode>" + Function.RawText(doc.ItemCode) + "</ItemCode>");
			}
			if (!string.IsNullOrWhiteSpace(doc.ItemName))
			{
				stringBuilder.Append("<ItemName>" + Function.RawText(doc.ItemName) + "</ItemName>");
			}
			stringBuilder.Append("<ForeignName>" + Function.RawText(doc.FrgnName) + "</ForeignName>");
			if (!string.IsNullOrEmpty(doc.ItemType))
			{
				ItemTypeEnum itemTypeEnum = ItemTypeEnum.itItems;
				stringBuilder.Append("<ItemType>" + (doc.ItemType switch
				{
					"I" => ItemTypeEnum.itItems, 
					"F" => ItemTypeEnum.itFixedAssets, 
					"L" => ItemTypeEnum.itLabor, 
					"T" => ItemTypeEnum.itTravel, 
					_ => ItemTypeEnum.itItems, 
				}).ToString() + "</ItemType>");
			}
			if (!string.IsNullOrWhiteSpace(doc.PrchseItem))
			{
				string text = ((doc.PrchseItem == "Y") ? "tYES" : "tNO");
				stringBuilder.Append("<PurchaseItem>" + text + "</PurchaseItem>");
			}
			if (!string.IsNullOrWhiteSpace(doc.InvntItem))
			{
				string text2 = ((doc.InvntItem == "Y") ? "tYES" : "tNO");
				stringBuilder.Append("<InventoryItem>" + text2 + "</InventoryItem>");
			}
			if (!string.IsNullOrWhiteSpace(doc.SellItem))
			{
				string text3 = ((doc.SellItem == "Y") ? "tYES" : "tNO");
				stringBuilder.Append("<SalesItem>" + text3 + "</SalesItem>");
			}
			if (Function.ParseInt(doc.ToleranDay) >= 0 || (!ignoreEmptyValue && commandType == EnumCommandType.Update))
			{
				stringBuilder.Append("<ToleranceDays>" + Function.ToString(doc.ToleranDay) + "</ToleranceDays>");
			}
			if (Function.ParseInt(doc.ItmsGrpCod) > 0)
			{
				stringBuilder.Append("<ItemsGroupCode>" + Function.ToString(doc.ItmsGrpCod) + "</ItemsGroupCode>");
			}
			if (doc.CstGrpCode != 0)
			{
				stringBuilder.Append("<CustomsGroupCode>" + Function.ToString(doc.CstGrpCode) + "</CustomsGroupCode>");
			}
			if (!string.IsNullOrWhiteSpace(doc.VatGourpSa))
			{
				stringBuilder.Append("<SalesVATGroup>" + Function.RawText(doc.VatGourpSa) + "</SalesVATGroup>");
			}
			if (!string.IsNullOrWhiteSpace(doc.CodeBars) || (!ignoreEmptyValue && commandType == EnumCommandType.Update))
			{
				stringBuilder.Append("<BarCode>" + Function.RawText(doc.CodeBars) + "</BarCode>");
			}
			if (doc.VATLiable != null)
			{
				if (!string.IsNullOrWhiteSpace(doc.VATLiable) && doc.VATLiable.ToLower() == "n")
				{
					stringBuilder.Append("<VatLiable>tNO</VatLiable>");
				}
				else
				{
					stringBuilder.Append("<VatLiable>tYES</VatLiable>");
				}
			}
			if (!string.IsNullOrWhiteSpace(doc.CardCode) || (!ignoreEmptyValue && commandType == EnumCommandType.Update))
			{
				stringBuilder.Append("<Mainsupplier>" + Function.RawText(doc.CardCode) + "</Mainsupplier>");
			}
			if (!string.IsNullOrWhiteSpace(doc.SuppCatNum) || (!ignoreEmptyValue && commandType == EnumCommandType.Update))
			{
				stringBuilder.Append("<SupplierCatalogNo>" + Function.RawText(doc.SuppCatNum) + "</SupplierCatalogNo>");
			}
			if (!string.IsNullOrEmpty(doc.PicturName) || (!ignoreEmptyValue && commandType == EnumCommandType.Update))
			{
				stringBuilder.Append("<Picture>" + Function.RawText(doc.PicturName) + "</Picture>");
			}
			if (!string.IsNullOrWhiteSpace(doc.SerialNum) || (!ignoreEmptyValue && commandType == EnumCommandType.Update))
			{
				stringBuilder.Append("<SerialNum>" + Function.RawText(doc.SerialNum) + "</SerialNum>");
			}
			if (Function.ParseDouble(doc.CommisPcnt) > 0.0 || (!ignoreEmptyValue && commandType == EnumCommandType.Update))
			{
				stringBuilder.Append("<CommissionPercent>" + Function.ParseDecimal(doc.CommisPcnt) + "</CommissionPercent>");
			}
			if (Function.ParseDouble(doc.CommisSum) > 0.0 || (!ignoreEmptyValue && commandType == EnumCommandType.Update))
			{
				stringBuilder.Append("<CommissionSum>" + Function.ParseDecimal(doc.CommisSum) + "</CommissionSum>");
			}
			stringBuilder.Append("<CommissionGroup>0</CommissionGroup>");
			if (doc.AssetItem != null)
			{
				if (!string.IsNullOrWhiteSpace(doc.AssetItem) && doc.AssetItem.ToLower() == "y")
				{
					stringBuilder.Append("<AssetItem>tYES</AssetItem>");
				}
				else
				{
					stringBuilder.Append("<AssetItem>tNO</AssetItem>");
				}
			}
			if (!string.IsNullOrWhiteSpace(doc.ExportCode) || (!ignoreEmptyValue && commandType == EnumCommandType.Update))
			{
				stringBuilder.Append("<DataExportCode>" + Function.RawText(doc.ExportCode) + "</DataExportCode>");
			}
			if (doc.FirmCode <= 0)
			{
				stringBuilder.Append("<Manufacturer>-1</Manufacturer>");
			}
			else if (doc.FirmCode > 0 || (!ignoreEmptyValue && commandType == EnumCommandType.Update))
			{
				stringBuilder.Append("<Manufacturer>" + Function.RawText(doc.FirmCode) + "</Manufacturer>");
			}
			if (!string.IsNullOrEmpty(doc.ManBtchNum) || (!ignoreEmptyValue && commandType == EnumCommandType.Update))
			{
				string text4 = ((doc.ManBtchNum == "Y") ? "tYES" : "tNO");
				stringBuilder.Append("<ManageBatchNumbers>" + text4 + "</ManageBatchNumbers>");
			}
			if (!string.IsNullOrEmpty(doc.ManSerNum) || (!ignoreEmptyValue && commandType == EnumCommandType.Update))
			{
				string text5 = ((doc.ManSerNum == "Y") ? "tYES" : "tNO");
				stringBuilder.Append("<ManageSerialNumbers>" + text5 + "</ManageSerialNumbers>");
			}
			if (doc.ManSerNum == "Y" || doc.ManBtchNum == "Y")
			{
				switch (doc.IssuePriBy)
				{
				case 0:
					stringBuilder.Append("<IssuePrimarilyBy>ipbSerialAndBatchNumbers</IssuePrimarilyBy>");
					break;
				case 1:
					stringBuilder.Append("<IssuePrimarilyBy>ipbBinLocations</IssuePrimarilyBy>");
					break;
				}
				if (doc.MngMethod == "A" || doc.MngMethod == "R")
				{
					BoManageMethod boManageMethod = ((!(doc.MngMethod == "A")) ? BoManageMethod.bomm_OnReleaseOnly : BoManageMethod.bomm_OnEveryTransaction);
					stringBuilder.Append("<SRIAndBatchManageMethod>" + Function.ToString(boManageMethod) + "</SRIAndBatchManageMethod>");
					if (doc.MngMethod == "R" && doc.ManSerNum == "Y")
					{
						if (doc.ManOutOnly == "Y")
						{
							stringBuilder.Append("<ManageSerialNumbersOnReleaseOnly>tYES</ManageSerialNumbersOnReleaseOnly>");
						}
						else
						{
							stringBuilder.Append("<ManageSerialNumbersOnReleaseOnly>tNO</ManageSerialNumbersOnReleaseOnly>");
						}
					}
				}
			}
			BoYesNoEnum boYesNoEnum = ((doc.validFor == "Y") ? BoYesNoEnum.tYES : BoYesNoEnum.tNO);
			stringBuilder.Append("<Valid>" + Function.ToString(boYesNoEnum) + "</Valid>");
			string text6;
			if (doc.validFor == "Y")
			{
				if (doc.validFrom.HasValue)
				{
					_ = doc.validFrom.Value;
					if (doc.validFrom.Value.Year > 2000)
					{
						text6 = doc.validFrom.Value.ToString("yyyyMMdd");
						goto IL_084e;
					}
				}
				text6 = "";
				goto IL_084e;
			}
			goto IL_08f9;
			IL_084e:
			stringBuilder.Append("<ValidFrom>" + text6 + "</ValidFrom>");
			string text7;
			if (doc.validTo.HasValue)
			{
				_ = doc.validTo.Value;
				if (doc.validTo.Value.Year > 2000)
				{
					text7 = doc.validTo.Value.ToString("yyyyMMdd");
					goto IL_08c8;
				}
			}
			text7 = "";
			goto IL_08c8;
			IL_0a5b:
			if (!string.IsNullOrWhiteSpace(doc.BuyUnitMsr) || (!ignoreEmptyValue && commandType == EnumCommandType.Update))
			{
				stringBuilder.Append("<PurchaseUnit>" + Function.RawText(doc.BuyUnitMsr) + "</PurchaseUnit>");
			}
			if (!string.IsNullOrWhiteSpace(doc.VatGroupPu) || (!ignoreEmptyValue && commandType == EnumCommandType.Update))
			{
				stringBuilder.Append("<PurchaseVATGroup>" + Function.RawText(doc.VatGroupPu) + "</PurchaseVATGroup>");
			}
			if (Function.ParseDouble(doc.SalFactor1) > 0.0)
			{
				stringBuilder.Append("<SalesFactor1>" + Function.ParseDecimal(doc.SalFactor1) + "</SalesFactor1>");
			}
			if (Function.ParseDouble(doc.SalFactor2) > 0.0)
			{
				stringBuilder.Append("<SalesFactor2>" + Function.ParseDecimal(doc.SalFactor2) + "</SalesFactor2>");
			}
			if (Function.ParseDouble(doc.SalFactor3) > 0.0)
			{
				stringBuilder.Append("<SalesFactor3>" + Function.ParseDecimal(doc.SalFactor3) + "</SalesFactor3>");
			}
			if (Function.ParseDouble(doc.SalFactor4) > 0.0)
			{
				stringBuilder.Append("<SalesFactor4>" + Function.ParseDecimal(doc.SalFactor4) + "</SalesFactor4>");
			}
			if (Function.ParseDouble(doc.PurFactor1) > 0.0)
			{
				stringBuilder.Append("<PurchaseFactor1>" + Function.ParseDecimal(doc.PurFactor1) + "</PurchaseFactor1>");
			}
			if (Function.ParseDouble(doc.PurFactor2) > 0.0)
			{
				stringBuilder.Append("<PurchaseFactor2>" + Function.ParseDecimal(doc.PurFactor2) + "</PurchaseFactor2>");
			}
			if (Function.ParseDouble(doc.PurFactor3) > 0.0)
			{
				stringBuilder.Append("<PurchaseFactor3>" + Function.ParseDecimal(doc.PurFactor3) + "</PurchaseFactor3>");
			}
			if (Function.ParseDouble(doc.PurFactor4) > 0.0)
			{
				stringBuilder.Append("<PurchaseFactor4>" + Function.ParseDecimal(doc.PurFactor4) + "</PurchaseFactor4>");
			}
			if (!string.IsNullOrWhiteSpace(doc.FrgnInAcct) || (!ignoreEmptyValue && commandType == EnumCommandType.Update))
			{
				stringBuilder.Append("<ForeignRevenuesAccount>" + Function.RawText(doc.FrgnInAcct) + "</ForeignRevenuesAccount>");
			}
			if (!string.IsNullOrWhiteSpace(doc.ECInAcct) || (!ignoreEmptyValue && commandType == EnumCommandType.Update))
			{
				stringBuilder.Append("<ECRevenuesAccount>" + Function.RawText(doc.ECInAcct) + "</ECRevenuesAccount>");
			}
			if (!string.IsNullOrWhiteSpace(doc.FrgnExpAcc) || (!ignoreEmptyValue && commandType == EnumCommandType.Update))
			{
				stringBuilder.Append("<ForeignExpensesAccount>" + Function.RawText(doc.FrgnExpAcc) + "</ForeignExpensesAccount>");
			}
			if (!string.IsNullOrWhiteSpace(doc.ECExpAcc) || (!ignoreEmptyValue && commandType == EnumCommandType.Update))
			{
				stringBuilder.Append("<ECExpensesAccount>" + Function.RawText(doc.ECExpAcc) + "</ECExpensesAccount>");
			}
			if (Function.ParseDouble(doc.AvgPrice) > 0.0 || (!ignoreEmptyValue && commandType == EnumCommandType.Update))
			{
				stringBuilder.Append("<AvgStdPrice>" + Function.ParseDecimal(doc.AvgPrice) + "</AvgStdPrice>");
			}
			if (!string.IsNullOrWhiteSpace(doc.DfltWH) || (!ignoreEmptyValue && commandType == EnumCommandType.Update))
			{
				stringBuilder.Append("<DefaultWarehouse>" + Function.RawText(doc.DfltWH) + "</DefaultWarehouse>");
			}
			if (Function.ParseInt(doc.ShipType) > 0)
			{
				stringBuilder.Append("<ShipType>" + Function.ParseInt(doc.ShipType) + "</ShipType>");
			}
			if (doc.TaxType != null)
			{
				switch (Function.ToString(doc.TaxType).ToUpper())
				{
				case "Y":
					stringBuilder.Append("<TaxType>tt_Yes</TaxType>");
					break;
				case "U":
					stringBuilder.Append("<TaxType>tt_UseTax</TaxType>");
					break;
				case "N":
					stringBuilder.Append("<TaxType>tt_No</TaxType>");
					break;
				default:
					stringBuilder.Append("<TaxType>tt_Yes</TaxType>");
					break;
				}
			}
			if (Function.ParseDouble(doc.MaxLevel) > 0.0)
			{
				stringBuilder.Append("<MaxInventory>" + Function.ParseDecimal(doc.MaxLevel) + "</MaxInventory>");
			}
			if ((!_unitOfWork.Common.GetIsMultiBranch() && !_unitOfWork.Common.GetAvailableQty(doc.ItemCode) && !_unitOfWork.Common.GetSerialBatch(doc.ItemCode)) || (!ignoreEmptyValue && commandType == EnumCommandType.Update))
			{
				if (string.Compare(doc.ByWh, "y", ignoreCase: true) == 0)
				{
					stringBuilder.Append("<ManageStockByWarehouse>tYES</ManageStockByWarehouse>");
				}
				else
				{
					stringBuilder.Append("<ManageStockByWarehouse>tNO</ManageStockByWarehouse>");
				}
			}
			if (doc.ByWh != "Y")
			{
				if (doc.ReorderQty > 0.0 || (!ignoreEmptyValue && commandType == EnumCommandType.Update))
				{
					stringBuilder.Append("<DesiredInventory>" + Function.ParseDecimal(doc.ReorderQty) + "</DesiredInventory>");
				}
				if (doc.MinLevel > 0.0 || (!ignoreEmptyValue && commandType == EnumCommandType.Update))
				{
					stringBuilder.Append("<MinInventory>" + Function.ParseDecimal(doc.MinLevel) + "</MinInventory>");
				}
				if (doc.MaxLevel > 0.0 || (!ignoreEmptyValue && commandType == EnumCommandType.Update))
				{
					stringBuilder.Append("<MaxInventory>" + Function.ParseDecimal(doc.MaxLevel) + "</MaxInventory>");
				}
			}
			if (Function.ParseInt(doc.BHght2Unit) > 0)
			{
				stringBuilder.Append("<PurchaseHeightUnit1>" + Function.ParseInt(doc.BHght2Unit) + "</PurchaseHeightUnit1>");
			}
			if (Function.ParseDouble(doc.BHeight2) > 0.0)
			{
				stringBuilder.Append("<PurchaseUnitHeight1>" + Function.ParseDecimal(doc.BHeight2) + "</PurchaseUnitHeight1>");
			}
			if (Function.ParseInt(doc.BLen2Unit) > 0)
			{
				stringBuilder.Append("<PurchaseLengthUnit1>" + Function.ParseInt(doc.BLen2Unit) + "</PurchaseLengthUnit1>");
			}
			if (Function.ParseDecimal(doc.Blength2) > 0m)
			{
				stringBuilder.Append("<PurchaseUnitLength1>" + Function.ParseDecimal(doc.Blength2) + "</PurchaseUnitLength1>");
			}
			if (Function.ParseInt(doc.BWght2Unit) > 0)
			{
				stringBuilder.Append("<PurchaseWeightUnit1>" + Function.ParseInt(doc.BWght2Unit) + "</PurchaseWeightUnit1>");
			}
			if (Function.ParseDecimal(doc.BWeight2) > 0m)
			{
				stringBuilder.Append("<PurchaseUnitWeight1>" + Function.ParseDecimal(doc.BWeight2) + "</PurchaseUnitWeight1>");
			}
			if (Function.ParseInt(doc.BWdth2Unit) > 0)
			{
				stringBuilder.Append("<PurchaseWidthUnit1>" + Function.ParseInt(doc.BWdth2Unit) + "</PurchaseWidthUnit1>");
			}
			if (Function.ParseDecimal(doc.BWidth2) > 0m)
			{
				stringBuilder.Append("<PurchaseUnitWidth1>" + Function.ParseDecimal(doc.BWidth2) + "</PurchaseUnitWidth1>");
			}
			if (Function.ParseInt(doc.SHght2Unit) > 0)
			{
				stringBuilder.Append("<SalesHeightUnit1>" + Function.ParseInt(doc.SHght2Unit) + "</SalesHeightUnit1>");
			}
			if (Function.ParseDecimal(doc.SHeight2) > 0m)
			{
				stringBuilder.Append("<SalesUnitHeight1>" + Function.ParseDecimal(doc.SHeight2) + "</SalesUnitHeight1>");
			}
			if (Function.ParseInt(doc.SLen2Unit) > 0)
			{
				stringBuilder.Append("<SalesLengthUnit1>" + Function.ParseInt(doc.SLen2Unit) + "</SalesLengthUnit1>");
			}
			if (Function.ParseDecimal(doc.Slength2) > 0m)
			{
				stringBuilder.Append("<SalesUnitLength1>" + Function.ParseDecimal(doc.Slength2) + "</SalesUnitLength1>");
			}
			if (Function.ParseInt(doc.SWght2Unit) > 0)
			{
				stringBuilder.Append("<SalesWeightUnit1>" + Function.ParseInt(doc.SWght2Unit) + "</SalesWeightUnit1>");
			}
			if (Function.ParseDecimal(doc.SWeight2) > 0m)
			{
				stringBuilder.Append("<SalesUnitWeight1>" + Function.ParseDecimal(doc.SWeight2) + "</SalesUnitWeight1>");
			}
			if (Function.ParseInt(doc.SWdth2Unit) > 0)
			{
				stringBuilder.Append("<SalesWidthUnit1>" + Function.ToString(doc.SWdth2Unit) + "</SalesWidthUnit1>");
			}
			if (Function.ParseDecimal(doc.SWidth2) > 0m)
			{
				stringBuilder.Append("<SalesUnitWidth1>" + Function.ParseDecimal(doc.SWidth2) + "</SalesUnitWidth1>");
			}
			if (doc.BlockOut != null)
			{
				if (!string.IsNullOrWhiteSpace(doc.BlockOut) && doc.BlockOut.ToLower() == "n")
				{
					stringBuilder.Append("<ForceSelectionOfSerialNumber>tNO</ForceSelectionOfSerialNumber>");
				}
				else
				{
					stringBuilder.Append("<ForceSelectionOfSerialNumber>tYES</ForceSelectionOfSerialNumber>");
				}
			}
			if (doc.WTLiable != null || (!ignoreEmptyValue && commandType == EnumCommandType.Update))
			{
				if (!string.IsNullOrWhiteSpace(doc.WTLiable) && doc.WTLiable.ToLower() == "n")
				{
					stringBuilder.Append("<WTLiable>tNO</WTLiable>");
				}
				else
				{
					stringBuilder.Append("<WTLiable>tYES</WTLiable>");
				}
			}
			if (doc.EvalSystem != null || (!ignoreEmptyValue && commandType == EnumCommandType.Update))
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
				stringBuilder.Append("<CostAccountingMethod>" + Function.ToString(boInventorySystem) + "</CostAccountingMethod>");
			}
			if (!string.IsNullOrWhiteSpace(doc.SWW) || (!ignoreEmptyValue && commandType == EnumCommandType.Update))
			{
				stringBuilder.Append("<SWW>" + Function.RawText(doc.SWW) + "</SWW>");
			}
			if (!string.IsNullOrWhiteSpace(doc.WarrntTmpl) || (!ignoreEmptyValue && commandType == EnumCommandType.Update))
			{
				stringBuilder.Append("<WarrantyTemplate>" + Function.RawText(doc.WarrntTmpl) + "</WarrantyTemplate>");
			}
			if (doc.IndirctTax != null || (!ignoreEmptyValue && commandType == EnumCommandType.Update))
			{
				if (!string.IsNullOrWhiteSpace(doc.IndirctTax) && doc.IndirctTax.ToLower() == "y")
				{
					stringBuilder.Append("<IndirectTax>tYES</IndirectTax>");
				}
				else
				{
					stringBuilder.Append("<IndirectTax>tNO</IndirectTax>");
				}
			}
			if (!string.IsNullOrWhiteSpace(doc.TaxCodeAR) || (!ignoreEmptyValue && commandType == EnumCommandType.Update))
			{
				stringBuilder.Append("<ArTaxCode>" + Function.RawText(doc.TaxCodeAR) + "</ArTaxCode>");
			}
			if (!string.IsNullOrWhiteSpace(doc.TaxCodeAP) || (!ignoreEmptyValue && commandType == EnumCommandType.Update))
			{
				stringBuilder.Append("<ApTaxCode>" + Function.RawText(doc.TaxCodeAP) + "</ApTaxCode>");
			}
			if (!string.IsNullOrWhiteSpace(doc.BaseUnit) || (!ignoreEmptyValue && commandType == EnumCommandType.Update))
			{
				stringBuilder.Append("<BaseUnitName>" + Function.RawText(doc.BaseUnit) + "</BaseUnitName>");
			}
			if (!string.IsNullOrWhiteSpace(doc.CountryOrg) || (!ignoreEmptyValue && commandType == EnumCommandType.Update))
			{
				stringBuilder.Append("<ItemCountryOrg>" + Function.RawText(doc.CountryOrg) + "</ItemCountryOrg>");
			}
			if (!string.IsNullOrWhiteSpace(doc.IssueMthd) || (!ignoreEmptyValue && commandType == EnumCommandType.Update))
			{
				BoIssueMethod boIssueMethod = ((doc.IssueMthd == "M") ? BoIssueMethod.im_Manual : BoIssueMethod.im_Backflush);
				stringBuilder.Append("<IssueMethod>" + Function.ToString(boIssueMethod) + "</IssueMethod>");
			}
			if (!string.IsNullOrWhiteSpace(doc.Phantom) || (!ignoreEmptyValue && commandType == EnumCommandType.Update))
			{
				if (doc.Phantom == "Y")
				{
					stringBuilder.Append("<IsPhantom>tYES</IsPhantom>");
				}
				else
				{
					stringBuilder.Append("<IsPhantom>tNO</IsPhantom>");
				}
			}
			if (!string.IsNullOrWhiteSpace(doc.InvntryUom) || (!ignoreEmptyValue && commandType == EnumCommandType.Update))
			{
				stringBuilder.Append("<InventoryUOM>" + Function.RawText(doc.InvntryUom) + "</InventoryUOM>");
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
				stringBuilder.Append("<PlanningSystem>" + boPlanningSystem.ToString() + "</PlanningSystem>");
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
				stringBuilder.Append("<ProcurementMethod>" + boProcurementMethod.ToString() + "</ProcurementMethod>");
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
					stringBuilder.Append("<ComponentWarehouse>" + boMRPComponentWarehouse.ToString() + "</ComponentWarehouse>");
				}
			}
			if (doc.OrdrIntrvl > 0)
			{
				stringBuilder.Append("<OrderIntervals>" + Function.ParseInt(doc.OrdrIntrvl) + "</OrderIntervals>");
			}
			if (doc.OrdrMulti > 0.0)
			{
				stringBuilder.Append("<OrderMultiple>" + Function.ParseDecimal(doc.OrdrMulti) + "</OrderMultiple>");
			}
			if (doc.LeadTime != 0 || (!ignoreEmptyValue && commandType == EnumCommandType.Update))
			{
				stringBuilder.Append("<LeadTime>" + Function.ParseInt(doc.LeadTime) + "</LeadTime>");
			}
			if (Function.ParseDecimal(doc.MinOrdrQty) > 0m || (!ignoreEmptyValue && commandType == EnumCommandType.Update))
			{
				stringBuilder.Append("<MinOrderQuantity>" + Function.ParseDecimal(doc.MinOrdrQty) + "</MinOrderQuantity>");
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
					stringBuilder.Append("<ItemClass>" + Function.ToString(itemClassEnum) + "</ItemClass>");
				}
			}
			if (Function.ParseInt(doc.OSvcCode) > 0)
			{
				stringBuilder.Append("<OutgoingServiceCode>" + Function.ParseInt(doc.OSvcCode) + "</OutgoingServiceCode>");
			}
			if (Function.ParseInt(doc.ISvcCode) > 0)
			{
				stringBuilder.Append("<IncomingServiceCode>" + Function.ParseInt(doc.ISvcCode) + "</IncomingServiceCode>");
			}
			if (Function.ParseInt(doc.ServiceGrp) > 0)
			{
				stringBuilder.Append("<ServiceGroup>" + Function.ParseInt(doc.ServiceGrp) + "</ServiceGroup>");
			}
			if (Function.ParseInt(doc.NCMCode) > 0)
			{
				stringBuilder.Append("<NCMCode>" + Function.ParseInt(doc.NCMCode) + "</NCMCode>");
			}
			if (doc.MatType != null)
			{
				stringBuilder.Append("<MaterialType>" + Function.RawText(doc.MatType) + "</MaterialType>");
			}
			if (Function.ParseInt(doc.MatGrp) > 0)
			{
				stringBuilder.Append("<MaterialGroup>" + Function.ParseInt(doc.MatGrp) + "</MaterialGroup>");
			}
			if (Function.ParseInt(doc.ProductSrc) > 0)
			{
				stringBuilder.Append("<ProductSource>" + Function.ParseInt(doc.ProductSrc) + "</ProductSource>");
			}
			if (Function.ParseInt(doc.DNFEntry) > 0)
			{
				stringBuilder.Append("<DNFEntry>" + Function.ParseInt(doc.DNFEntry) + "</DNFEntry>");
			}
			if (!string.IsNullOrWhiteSpace(doc.Spec))
			{
				stringBuilder.Append("<GTSItemSpec>" + Function.RawText(doc.Spec) + "</GTSItemSpec>");
			}
			if (!string.IsNullOrWhiteSpace(doc.TaxCtg))
			{
				stringBuilder.Append("<GTSItemTaxCategory>" + Function.RawText(doc.TaxCtg) + "</GTSItemTaxCategory>");
			}
			if (Function.ParseInt(doc.FuelCode) > 0)
			{
				stringBuilder.Append("<FuelID>" + Function.ParseInt(doc.FuelCode) + "</FuelID>");
			}
			if (!string.IsNullOrWhiteSpace(doc.BeverTblC))
			{
				stringBuilder.Append("<BeverageTableCode>" + Function.RawText(doc.BeverTblC) + "</BeverageTableCode>");
			}
			if (!string.IsNullOrWhiteSpace(doc.BeverGrpC))
			{
				stringBuilder.Append("<BeverageGroupCode>" + Function.RawText(doc.BeverGrpC) + "</BeverageGroupCode>");
			}
			if (Function.ParseInt(doc.BeverTM) > 0)
			{
				stringBuilder.Append("<BeverageCommercialBrandCode>" + Function.ParseInt(doc.BeverTM) + "</BeverageCommercialBrandCode>");
			}
			if (Function.ParseInt(doc.Series) > 0 && commandType == EnumCommandType.Add)
			{
				stringBuilder.Append("<Series>" + Function.ToString(doc.Series) + "</Series>");
			}
			if (doc.ToleranDay != 0 || (!ignoreEmptyValue && commandType == EnumCommandType.Update))
			{
				stringBuilder.Append("<ToleranceDays>" + Function.ParseInt(doc.ToleranDay) + "</ToleranceDays>");
			}
			_ = doc.GLPickMeth;
			if (!string.IsNullOrWhiteSpace(doc.NoDiscount) || (!ignoreEmptyValue && commandType == EnumCommandType.Update))
			{
				stringBuilder.Append((doc.NoDiscount == "Y") ? "<NoDiscounts>tYES</NoDiscounts>" : "<NoDiscounts>tNO</NoDiscounts>");
			}
			if (!string.IsNullOrWhiteSpace(doc.AssetClass))
			{
				stringBuilder.Append("<AssetClass>" + Function.RawText(doc.AssetClass) + "</AssetClass>");
			}
			if (!string.IsNullOrWhiteSpace(doc.AssetGroup))
			{
				stringBuilder.Append("<AssetGroup>" + Function.RawText(doc.AssetGroup) + "</AssetGroup>");
			}
			if (!string.IsNullOrWhiteSpace(doc.InventryNo))
			{
				stringBuilder.Append("<InventoryNumber>" + Function.RawText(doc.InventryNo) + "</InventoryNumber>");
			}
			if (Function.ParseInt(doc.Technician) > 0)
			{
				stringBuilder.Append("<Technician>" + Function.ParseInt(doc.Technician) + "</Technician>");
			}
			if (Function.ParseInt(doc.Technician) > 0)
			{
				stringBuilder.Append("<Employee>" + Function.ToString(doc.Technician) + "</Employee>");
			}
			if (Function.ParseInt(doc.Location) > 0)
			{
				stringBuilder.Append("<Location>" + Function.ToString(doc.Location) + "</Location>");
			}
			if (doc.CapDate.HasValue)
			{
				_ = doc.CapDate.Value;
				if (doc.CapDate.Value.Year > 2000)
				{
					stringBuilder.Append("<CapitalizationDate>" + doc.CapDate.Value.ToString("yyyyMMdd") + "</CapitalizationDate>");
				}
			}
			if (doc.StatAsset != null)
			{
				if (!string.IsNullOrWhiteSpace(doc.StatAsset) && doc.StatAsset.ToLower() == "y")
				{
					stringBuilder.Append("<StatisticalAsset>tYES</StatisticalAsset>");
				}
				else
				{
					stringBuilder.Append("<StatisticalAsset>tNO</StatisticalAsset>");
				}
			}
			if (doc.Cession != null)
			{
				if (!string.IsNullOrWhiteSpace(doc.Cession) && doc.Cession.ToLower() == "y")
				{
					stringBuilder.Append("<Cession>tYES</Cession>");
				}
				else
				{
					stringBuilder.Append("<Cession>tNO</Cession>");
				}
			}
			if (doc.DeacAftUL != null)
			{
				if (!string.IsNullOrWhiteSpace(doc.DeacAftUL) && doc.DeacAftUL.ToLower() == "y")
				{
					stringBuilder.Append("<DeactivateAfterUsefulLife>tYES</DeactivateAfterUsefulLife>");
				}
				else
				{
					stringBuilder.Append("<DeactivateAfterUsefulLife>tNO</DeactivateAfterUsefulLife>");
				}
			}
			if (Function.ParseInt(doc.SVolUnit) > 0)
			{
				stringBuilder.Append("<SalesVolumeUnit>" + Function.ToString(doc.SVolUnit) + "</SalesVolumeUnit>");
			}
			if (Function.ParseInt(doc.BVolUnit) > 0)
			{
				stringBuilder.Append("<PurchaseVolumeUnit>" + Function.ToString(doc.BVolUnit) + "</PurchaseVolumeUnit>");
			}
			if (Function.ParseInt(doc.UgpEntry) > 0)
			{
				stringBuilder.Append("<UoMGroupEntry>" + Function.ToString(doc.UgpEntry) + "</UoMGroupEntry>");
				if (Function.ParseInt(doc.IUoMEntry) > 0)
				{
					stringBuilder.Append("<InventoryUoMEntry>" + Function.ToString(doc.IUoMEntry) + "</InventoryUoMEntry>");
				}
				if (Function.ParseInt(doc.SUoMEntry) > 0)
				{
					stringBuilder.Append("<DefaultSalesUoMEntry>" + Function.ToString(doc.SUoMEntry) + "</DefaultSalesUoMEntry>");
					if (!string.IsNullOrWhiteSpace(doc.SalUnitMsr))
					{
						stringBuilder.Append("<SalesUnit>" + Function.RawText(doc.SalUnitMsr) + "</SalesUnit>");
					}
					if (Function.ParseDecimal(doc.NumInSale) > 0m)
					{
						stringBuilder.Append("<SalesItemsPerUnit>" + Function.ParseDecimal(doc.NumInSale) + "</SalesItemsPerUnit>");
					}
					if (Function.ParseDecimal(doc.SLength1) > 0m)
					{
						stringBuilder.Append("<SalesUnitLength>" + Function.ParseDecimal(doc.SLength1) + "</SalesUnitLength>");
					}
					if (Function.ParseInt(doc.SLen1Unit) > 0)
					{
						stringBuilder.Append("<SalesLengthUnit>" + Function.ToString(doc.SLen1Unit) + "</SalesLengthUnit>");
					}
					if (Function.ParseDecimal(doc.SWidth1) > 0m)
					{
						stringBuilder.Append("<SalesUnitWidth>" + Function.ParseDecimal(doc.SWidth1) + "</SalesUnitWidth>");
					}
					if (Function.ParseInt(doc.SWdth1Unit) > 0)
					{
						stringBuilder.Append("<SalesWidthUnit>" + Function.ToString(doc.SWdth1Unit) + "</SalesWidthUnit>");
					}
					if (Function.ParseDecimal(doc.SHeight1) > 0m)
					{
						stringBuilder.Append("<SalesUnitHeight>" + Function.ParseDecimal(doc.SHeight1) + "</SalesUnitHeight>");
					}
					if (Function.ParseInt(doc.SHght1Unit) > 0)
					{
						stringBuilder.Append("<SalesHeightUnit>" + Function.ToString(doc.SHght1Unit) + "</SalesHeightUnit>");
					}
					if (Function.ParseDecimal(doc.SWeight1) > 0m)
					{
						stringBuilder.Append("<SalesUnitWeight>" + Function.ParseDecimal(doc.SWeight1) + "</SalesUnitWeight>");
					}
					if (Function.ParseInt(doc.SWght1Unit) > 0)
					{
						stringBuilder.Append("<SalesWeightUnit>" + Function.ToString(doc.SWght1Unit) + "</SalesWeightUnit>");
					}
					if (Function.ParseDecimal(doc.SVolume) > 0m)
					{
						stringBuilder.Append("<SalesUnitVolume>" + Function.ParseDecimal(doc.SVolume) + "</SalesUnitVolume>");
					}
				}
				if (Function.ParseInt(doc.PUoMEntry) > 0)
				{
					stringBuilder.Append("<DefaultPurchasingUoMEntry>" + Function.ToString(doc.PUoMEntry) + "</DefaultPurchasingUoMEntry>");
					if (!string.IsNullOrWhiteSpace(doc.BuyUnitMsr))
					{
						stringBuilder.Append("<PurchaseUnit>" + Function.RawText(doc.BuyUnitMsr) + "</PurchaseUnit>");
					}
					if (Function.ParseDecimal(doc.BVolume) > 0m)
					{
						stringBuilder.Append("<PurchaseUnitVolume>" + Function.ParseDecimal(doc.BVolume) + "</PurchaseUnitVolume>");
					}
					if (Function.ParseDecimal(doc.BLength1) > 0m)
					{
						stringBuilder.Append("<PurchaseUnitLength>" + Function.ParseDecimal(doc.BLength1) + "</PurchaseUnitLength>");
					}
					if (Function.ParseInt(doc.BLen1Unit) > 0)
					{
						stringBuilder.Append("<PurchaseLengthUnit>" + Function.ToString(doc.BLen1Unit) + "</PurchaseLengthUnit>");
					}
					if (Function.ParseDecimal(doc.BWidth1) > 0m)
					{
						stringBuilder.Append("<PurchaseUnitWidth>" + Function.ParseDecimal(doc.BWidth1) + "</PurchaseUnitWidth>");
					}
					if (Function.ParseInt(doc.BWdth1Unit) > 0)
					{
						stringBuilder.Append("<PurchaseWidthUnit>" + Function.ToString(doc.BWdth1Unit) + "</PurchaseWidthUnit>");
					}
					if (Function.ParseDecimal(doc.BHeight1) > 0m)
					{
						stringBuilder.Append("<PurchaseUnitHeight>" + Function.ParseDecimal(doc.BHeight1) + "</PurchaseUnitHeight>");
					}
					if (Function.ParseInt(doc.BHght1Unit) > 0)
					{
						stringBuilder.Append("<PurchaseHeightUnit>" + Function.ToString(doc.BHght1Unit) + "</PurchaseHeightUnit>");
					}
					if (Function.ParseDecimal(doc.BWeight1) > 0m)
					{
						stringBuilder.Append("<PurchaseUnitWeight>" + Function.ParseDecimal(doc.BWeight1) + "</PurchaseUnitWeight>");
					}
					if (Function.ParseInt(doc.BWght1Unit) > 0)
					{
						stringBuilder.Append("<PurchaseWeightUnit>" + Function.ToString(doc.BWght1Unit) + "</PurchaseWeightUnit>");
					}
					if (Function.ParseDecimal(doc.NumInBuy) > 0m)
					{
						stringBuilder.Append("<PurchaseItemsPerUnit>" + Function.ParseDecimal(doc.NumInBuy) + "</PurchaseItemsPerUnit>");
					}
				}
			}
			else if (doc.UgpEntry != 0)
			{
				stringBuilder.Append("<SalesUnit>" + Function.RawText(doc.SalUnitMsr) + "</SalesUnit>");
				if (Function.ParseDecimal(doc.NumInSale) > 0m)
				{
					stringBuilder.Append("<SalesItemsPerUnit>" + Function.ParseDecimal(doc.NumInSale) + "</SalesItemsPerUnit>");
				}
				if (Function.ParseDecimal(doc.SVolume) > 0m)
				{
					stringBuilder.Append("<SalesUnitVolume>" + Function.ParseDecimal(doc.SVolume) + "</SalesUnitVolume>");
				}
				if (Function.ParseDecimal(doc.SLength1) > 0m)
				{
					stringBuilder.Append("<SalesUnitLength>" + Function.ParseDecimal(doc.SLength1) + "</SalesUnitLength>");
				}
				if (Function.ParseInt(doc.SLen1Unit) > 0)
				{
					stringBuilder.Append("<SalesLengthUnit>" + Function.ToString(doc.SLen1Unit) + "</SalesLengthUnit>");
				}
				if (Function.ParseDecimal(doc.SWidth1) > 0m)
				{
					stringBuilder.Append("<SalesUnitWidth>" + Function.ParseDecimal(doc.SWidth1) + "</SalesUnitWidth>");
				}
				if (Function.ParseInt(doc.SWdth1Unit) > 0)
				{
					stringBuilder.Append("<SalesWidthUnit>" + Function.ToString(doc.SWdth1Unit) + "</SalesWidthUnit>");
				}
				if (Function.ParseDecimal(doc.SHeight1) > 0m)
				{
					stringBuilder.Append("<SalesUnitHeight>" + Function.ParseDecimal(doc.SHeight1) + "</SalesUnitHeight>");
				}
				if (Function.ParseInt(doc.SHght1Unit) > 0)
				{
					stringBuilder.Append("<SalesHeightUnit>" + Function.ToString(doc.SHght1Unit) + "</SalesHeightUnit>");
				}
				if (Function.ParseInt(doc.SVolUnit) > 0)
				{
					stringBuilder.Append("<SalesVolumeUnit>" + Function.ToString(doc.SVolUnit) + "</SalesVolumeUnit>");
				}
				if (Function.ParseDecimal(doc.SWeight1) > 0m)
				{
					stringBuilder.Append("<SalesUnitWeight>" + Function.ParseDecimal(doc.SWeight1) + "</SalesUnitWeight>");
				}
				if (Function.ParseInt(doc.SWght1Unit) > 0)
				{
					stringBuilder.Append("<SalesWeightUnit>" + Function.ToString(doc.SWght1Unit) + "</SalesWeightUnit>");
				}
				stringBuilder.Append("<PurchaseUnit>" + Function.RawText(doc.BuyUnitMsr) + "</PurchaseUnit>");
				if (Function.ParseDecimal(doc.NumInBuy) > 0m)
				{
					stringBuilder.Append("<PurchaseItemsPerUnit>" + Function.ParseDecimal(doc.NumInBuy) + "</PurchaseItemsPerUnit>");
				}
				if (Function.ParseDecimal(doc.BVolume) > 0m)
				{
					stringBuilder.Append("<PurchaseUnitVolume>" + Function.ParseDecimal(doc.BVolume) + "</PurchaseUnitVolume>");
				}
				if (Function.ParseDecimal(doc.BLength1) > 0m)
				{
					stringBuilder.Append("<PurchaseUnitLength>" + Function.ParseDecimal(doc.BLength1) + "</PurchaseUnitLength>");
				}
				if (Function.ParseInt(doc.BLen1Unit) > 0)
				{
					stringBuilder.Append("<PurchaseLengthUnit>" + Function.ToString(doc.BLen1Unit) + "</PurchaseLengthUnit>");
				}
				if (Function.ParseDecimal(doc.BWidth1) > 0m)
				{
					stringBuilder.Append("<PurchaseUnitWidth>" + Function.ParseDecimal(doc.BWidth1) + "</PurchaseUnitWidth>");
				}
				if (Function.ParseInt(doc.BWdth1Unit) > 0)
				{
					stringBuilder.Append("<PurchaseWidthUnit>" + Function.ToString(doc.BWdth1Unit) + "</PurchaseWidthUnit>");
				}
				if (Function.ParseDecimal(doc.BHeight1) > 0m)
				{
					stringBuilder.Append("<PurchaseUnitHeight>" + Function.ParseDecimal(doc.BHeight1) + "</PurchaseUnitHeight>");
				}
				if (Function.ParseInt(doc.BHght1Unit) > 0)
				{
					stringBuilder.Append("<PurchaseHeightUnit>" + Function.ToString(doc.BHght1Unit) + "</PurchaseHeightUnit>");
				}
				if (Function.ParseDecimal(doc.BWeight1) > 0m)
				{
					stringBuilder.Append("<PurchaseUnitWeight>" + Function.ParseDecimal(doc.BWeight1) + "</PurchaseUnitWeight>");
				}
				if (Function.ParseInt(doc.BWght1Unit) > 0)
				{
					stringBuilder.Append("<PurchaseWeightUnit>" + Function.ToString(doc.BWght1Unit) + "</PurchaseWeightUnit>");
				}
				stringBuilder.Append("<UoMGroupEntry>-1</UoMGroupEntry>");
			}
			if (!string.IsNullOrWhiteSpace(doc.DeprGroup))
			{
				stringBuilder.Append("<DepreciationGroup>" + Function.RawText(doc.DeprGroup) + "</DepreciationGroup>");
			}
			if (!string.IsNullOrWhiteSpace(doc.AssetSerNo))
			{
				stringBuilder.Append("<AssetSerialNumber>" + Function.RawText(doc.AssetSerNo) + "</AssetSerialNumber>");
			}
			if (Function.ParseDecimal(doc.IWeight1) > 0m)
			{
				stringBuilder.Append("<InventoryWeight>" + Function.ParseDecimal(doc.IWeight1) + "</InventoryWeight>");
			}
			if (Function.ParseInt(doc.IWght1Unit) > 0)
			{
				stringBuilder.Append("<InventoryWeightUnit>" + Function.ToString(doc.IWght1Unit) + "</InventoryWeightUnit>");
			}
			if (Function.ParseDecimal(doc.IWeight2) > 0m)
			{
				stringBuilder.Append("<InventoryWeight1>" + Function.ParseDecimal(doc.IWeight2) + "</InventoryWeight1>");
			}
			if (Function.ParseInt(doc.IWght2Unit) > 0)
			{
				stringBuilder.Append("<InventoryWeightUnit1>" + Function.ToString(doc.IWght2Unit) + "</InventoryWeightUnit1>");
			}
			if (!string.IsNullOrWhiteSpace(doc.CntUnitMsr))
			{
				stringBuilder.Append("<DefaultCountingUnit>" + Function.RawText(doc.CntUnitMsr) + "</DefaultCountingUnit>");
			}
			if (Function.ParseInt(doc.INUoMEntry) > 0)
			{
				stringBuilder.Append("<DefaultCountingUoMEntry>" + Function.ToString(doc.INUoMEntry) + "</DefaultCountingUoMEntry>");
			}
			if (doc.Excisable != null)
			{
				if (!string.IsNullOrWhiteSpace(doc.Excisable) && doc.Excisable.ToLower() == "y")
				{
					stringBuilder.Append("<Excisable>tYES</Excisable>");
				}
				else
				{
					stringBuilder.Append("<Excisable>tNO</Excisable>");
				}
			}
			if (Function.ParseInt(doc.ChapterID) > 0)
			{
				stringBuilder.Append("<ChapterID>" + Function.ToString(doc.ChapterID) + "</ChapterID>");
			}
			if (!string.IsNullOrWhiteSpace(doc.ScsCode))
			{
				stringBuilder.Append("<ScsCode>" + Function.RawText(doc.ScsCode) + "</ScsCode>");
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
				stringBuilder.Append("<SpProdType>" + specialProductTypeEnum.ToString() + "</SpProdType>");
			}
			if (!string.IsNullOrWhiteSpace(doc.CodeBars))
			{
				stringBuilder.Append("<BarCode>" + Function.RawText(doc.CodeBars) + "</BarCode>");
			}
			if (!string.IsNullOrWhiteSpace(doc.SuppCatNum) || (!ignoreEmptyValue && commandType == EnumCommandType.Update))
			{
				stringBuilder.Append("<SupplierCatalogNo>" + Function.RawText(doc.SuppCatNum) + "</SupplierCatalogNo>");
			}
			if (Function.ParseDecimal(doc.PurPackUn) > 0m)
			{
				stringBuilder.Append("<PurchaseQtyPerPackUnit>" + Function.ParseDecimal(doc.PurPackUn) + "</PurchaseQtyPerPackUnit>");
			}
			if (Function.ParseDecimal(doc.MinLevel) > 0m)
			{
				stringBuilder.Append("<MinInventory>" + Function.ParseDecimal(doc.MinLevel) + "</MinInventory>");
			}
			stringBuilder.Append("<User_Text>" + Function.RawText(doc.UserText) + "</User_Text>");
			if (!string.IsNullOrEmpty(doc.TreeType))
			{
				BoItemTreeTypes boItemTreeTypes = BoItemTreeTypes.iNotATree;
				stringBuilder.Append("<TreeType>" + Function.ToString(doc.TreeType switch
				{
					"N" => BoItemTreeTypes.iNotATree, 
					"A" => BoItemTreeTypes.iAssemblyTree, 
					"S" => BoItemTreeTypes.iSalesTree, 
					"P" => BoItemTreeTypes.iProductionTree, 
					"T" => BoItemTreeTypes.iTemplateTree, 
					"I" => BoItemTreeTypes.iIngredient, 
					_ => BoItemTreeTypes.iNotATree, 
				}) + "</TreeType>");
			}
			else
			{
				stringBuilder.Append("<TreeType>iNotATree</TreeType>");
			}
			if (Function.ParseDecimal(doc.MaxLevel) > 0m)
			{
				stringBuilder.Append("<MaxInventory>" + Function.ParseDecimal(doc.MaxLevel) + "</MaxInventory>");
			}
			if (Function.ParseDecimal(doc.AvgPrice) > 0m)
			{
				stringBuilder.Append("<AvgStdPrice>" + Function.ParseDecimal(doc.AvgPrice) + "</AvgStdPrice>");
			}
			if (!string.IsNullOrWhiteSpace(doc.PurPackMsr))
			{
				stringBuilder.Append("<PurchasePackagingUnit>" + Function.RawText(doc.PurPackMsr) + "</PurchasePackagingUnit>");
			}
			if (!string.IsNullOrWhiteSpace(doc.SalPackMsr))
			{
				stringBuilder.Append("<SalesPackagingUnit>" + Function.RawText(doc.SalPackMsr) + "</SalesPackagingUnit>");
			}
			if (Function.ParseDecimal(doc.SalPackUn) > 0m)
			{
				stringBuilder.Append("<SalesQtyPerPackUnit>" + Function.ParseDecimal(doc.SalPackUn) + "</SalesQtyPerPackUnit>");
			}
			if (!string.IsNullOrEmpty(doc.GLMethod))
			{
				BoGLMethods boGLMethods = BoGLMethods.glm_WH;
				stringBuilder.Append("<GLMethod>" + Function.ToString(doc.GLMethod switch
				{
					"W" => BoGLMethods.glm_WH, 
					"C" => BoGLMethods.glm_ItemClass, 
					"L" => BoGLMethods.glm_ItemLevel, 
					_ => BoGLMethods.glm_WH, 
				}) + "</GLMethod>");
			}
			if (doc.ItemProperties != null && doc.ItemProperties.Count > 0)
			{
				for (int i = 0; i < doc.ItemProperties.Count; i++)
				{
					BoYesNoEnum boYesNoEnum2 = ((doc.ItemProperties[i] == "Y") ? BoYesNoEnum.tYES : BoYesNoEnum.tNO);
					stringBuilder.Append("<Properties" + (i + 1) + ">" + boYesNoEnum2.ToString() + "</Properties" + (i + 1) + ">");
				}
			}
			if (Function.ParseInt(doc.AtcEntry) > 0)
			{
				stringBuilder.Append("<AttachmentEntry>" + doc.AtcEntry + "</AttachmentEntry>");
			}
			else
			{
				stringBuilder.Append("<AttachmentEntry>-1</AttachmentEntry>");
			}
			stringBuilder.Append(DataService.MappingUdf(doc.ListUserFields, commandType, ignoreEmptyValue));
			stringBuilder.Append("</row>\r\n                        </Items>");
			if (commandType == EnumCommandType.Update)
			{
				stringBuilder.Append("<Items_Prices>");
				if (doc.ListITM1 != null)
				{
					for (int j = 0; j < doc.ListITM1.Count; j++)
					{
						stringBuilder.Append("<row>");
						if (doc.ListITM1[j].PriceList > 0)
						{
							stringBuilder.Append("<PriceList>" + Function.ToString(doc.ListITM1[j].PriceList) + "</PriceList>");
						}
						stringBuilder.Append("<Price>" + Function.ParseDecimal(doc.ListITM1[j].Price) + "</Price>");
						if (!string.IsNullOrWhiteSpace(doc.ListITM1[j].Currency) && Function.ParseDouble(doc.ListITM1[j].Price) > 0.0)
						{
							stringBuilder.Append("<Currency>" + Function.RawText(doc.ListITM1[j].Currency) + "</Currency>");
						}
						stringBuilder.Append("<AdditionalPrice1>" + Function.ParseDecimal(doc.ListITM1[j].AddPrice1) + "</AdditionalPrice1>");
						if (!string.IsNullOrWhiteSpace(doc.ListITM1[j].Currency1) && Function.ParseDecimal(doc.ListITM1[j].AddPrice1) > 0m)
						{
							stringBuilder.Append("<AdditionalCurrency1>" + Function.RawText(doc.ListITM1[j].Currency1) + "</AdditionalCurrency1>");
						}
						stringBuilder.Append("<AdditionalPrice2>" + Function.ParseDecimal(doc.ListITM1[j].AddPrice2) + "</AdditionalPrice2>");
						if (!string.IsNullOrWhiteSpace(doc.ListITM1[j].Currency2) && Function.ParseDouble(doc.ListITM1[j].AddPrice2) > 0.0)
						{
							stringBuilder.Append("<AdditionalCurrency2>" + Function.RawText(doc.ListITM1[j].Currency2) + "</AdditionalCurrency2>");
						}
						if (doc.ListITM1[j].BasePLNum > 0)
						{
							stringBuilder.Append("<BasePriceList>" + Function.ToString(doc.ListITM1[j].BasePLNum) + "</BasePriceList>");
						}
						else
						{
							stringBuilder.Append("<BasePriceList>" + Function.ToString(doc.ListITM1[j].PriceList) + "</BasePriceList>");
						}
						if (doc.ListITM1[j].Factor > 0.0)
						{
							stringBuilder.Append("<Factor>" + Function.ToString(doc.ListITM1[j].Factor) + "</Factor>");
						}
						else
						{
							stringBuilder.Append("<Factor>1</Factor>");
						}
						stringBuilder.Append("</row>");
					}
				}
				stringBuilder.Append("</Items_Prices>");
			}
			if ((commandType == EnumCommandType.Update || !IsAutoAddWarehouse() || (commandType == EnumCommandType.Add && Function.ToString(doc.GLMethod).ToUpper() == "L")) && doc.OitwLines != null && doc.OitwLines.Count > 0 && doc.OitwLines.Any((OITW_Impl t) => !string.IsNullOrWhiteSpace(t.WhsCode)))
			{
				stringBuilder.Append("<ItemWarehouseInfo>");
				for (int num = 0; num < doc.OitwLines.Count; num++)
				{
					stringBuilder.Append("<row>");
					stringBuilder.Append("<WarehouseCode>" + Function.RawText(doc.OitwLines[num].WhsCode) + "</WarehouseCode>");
					stringBuilder.Append("<Locked>" + ((Function.ToString(doc.OitwLines[num].Locked) == "Y") ? BoYesNoEnum.tYES : BoYesNoEnum.tNO).ToString() + "</Locked>");
					if (doc.ByWh == "Y")
					{
						stringBuilder.Append("<MaximalStock>" + doc.OitwLines[num].MaxStock + "</MaximalStock>");
						stringBuilder.Append("<MinimalOrder>" + doc.OitwLines[num].MinOrder + "</MinimalOrder>");
						stringBuilder.Append("<MinimalStock>" + doc.OitwLines[num].MinStock + "</MinimalStock>");
					}
					if (doc.OitwLines[num].DftBinAbs != 0)
					{
						stringBuilder.Append("<DefaultBin>" + doc.OitwLines[num].DftBinAbs + "</DefaultBin>");
					}
					if (commandType == EnumCommandType.Add && doc.GLMethod.ToUpper() == "L")
					{
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].SaleCostAc))
						{
							stringBuilder.Append("<CostAccount>" + doc.OitwLines[num].SaleCostAc + "</CostAccount>");
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].CostRvlAct))
						{
							stringBuilder.Append("<CostInflationAccount>" + doc.OitwLines[num].CostRvlAct + "</CostInflationAccount>");
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].CstOffsAct))
						{
							stringBuilder.Append("<CostInflationOffsetAccount>" + doc.OitwLines[num].CstOffsAct + "</CostInflationOffsetAccount>");
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].DecreasAc))
						{
							stringBuilder.Append("<DecreasingAccount>" + doc.OitwLines[num].DecreasAc + "</DecreasingAccount>");
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].EUExpensAc))
						{
							stringBuilder.Append("<EUExpensesAccount>" + doc.OitwLines[num].EUExpensAc + "</EUExpensesAccount>");
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].APCMEUAct))
						{
							stringBuilder.Append("<EUPurchaseCreditAcc>" + doc.OitwLines[num].APCMEUAct + "</EUPurchaseCreditAcc>");
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].EURevenuAc))
						{
							stringBuilder.Append("<EURevenuesAccount>" + doc.OitwLines[num].EURevenuAc + "</EURevenuesAccount>");
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].ExchangeAc))
						{
							stringBuilder.Append("<ExchangeRateDifferencesAcct>" + doc.OitwLines[num].ExchangeAc + "</ExchangeRateDifferencesAcct>");
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].ARCMExpAct))
						{
							stringBuilder.Append("<ExemptedCredits>" + doc.OitwLines[num].ARCMExpAct + "</ExemptedCredits>");
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].ExmptIncom))
						{
							stringBuilder.Append("<ExemptIncomeAcc>" + doc.OitwLines[num].ExmptIncom + "</ExemptIncomeAcc>");
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].ExpClrAct))
						{
							stringBuilder.Append("<ExpenseClearingAct>" + doc.OitwLines[num].ExpClrAct + "</ExpenseClearingAct>");
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].ExpOfstAct))
						{
							stringBuilder.Append("<ExpenseOffsettingAccount>" + doc.OitwLines[num].ExpOfstAct + "</ExpenseOffsettingAccount>");
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].ExpensesAc))
						{
							stringBuilder.Append("<ExpensesAccount>" + doc.OitwLines[num].ExpensesAc + "</ExpensesAccount>");
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].FrExpensAc))
						{
							stringBuilder.Append("<ForeignExpensAcc>" + doc.OitwLines[num].FrExpensAc + "</ForeignExpensAcc>");
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].APCMFrnAct))
						{
							stringBuilder.Append("<ForeignPurchaseCreditAcc>" + doc.OitwLines[num].APCMFrnAct + "</ForeignPurchaseCreditAcc>");
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].FrRevenuAc))
						{
							stringBuilder.Append("<ForeignRevenueAcc>" + doc.OitwLines[num].FrRevenuAc + "</ForeignRevenueAcc>");
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].DecresGlAc))
						{
							stringBuilder.Append("<GLDecreaseAcct>" + doc.OitwLines[num].DecresGlAc + "</GLDecreaseAcct>");
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].IncresGlAc))
						{
							stringBuilder.Append("<GLIncreaseAcct>" + doc.OitwLines[num].IncresGlAc + "</GLIncreaseAcct>");
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].BalanceAcc))
						{
							stringBuilder.Append("<GoodsClearingAcct>" + doc.OitwLines[num].BalanceAcc + "</GoodsClearingAcct>");
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].IncreasAc))
						{
							stringBuilder.Append("<IncreasingAccount>" + doc.OitwLines[num].IncreasAc + "</IncreasingAccount>");
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].BalInvntAc))
						{
							stringBuilder.Append("<InventoryAccount>" + doc.OitwLines[num].BalInvntAc + "</InventoryAccount>");
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].StockOffst))
						{
							stringBuilder.Append("<InventoryOffsetProfitAndLossAccount>" + doc.OitwLines[num].StockOffst + "</InventoryOffsetProfitAndLossAccount>");
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].NegStckAct))
						{
							stringBuilder.Append("<NegativeInventoryAdjustmentAccount>" + doc.OitwLines[num].NegStckAct + "</NegativeInventoryAdjustmentAccount>");
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].PAReturnAc))
						{
							stringBuilder.Append("<PAReturnAcct>" + doc.OitwLines[num].PAReturnAc + "</PAReturnAcct>");
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].PriceDifAc))
						{
							stringBuilder.Append("<PriceDifferenceAcc>" + doc.OitwLines[num].PriceDifAc + "</PriceDifferenceAcc>");
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].PurchaseAc))
						{
							stringBuilder.Append("<PurchaseAcct>" + doc.OitwLines[num].PurchaseAc + "</PurchaseAcct>");
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].PurBalAct))
						{
							stringBuilder.Append("<PurchaseBalanceAccount>" + doc.OitwLines[num].PurBalAct + "</PurchaseBalanceAccount>");
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].APCMAct))
						{
							stringBuilder.Append("<PurchaseCreditAcc>" + doc.OitwLines[num].APCMAct + "</PurchaseCreditAcc>");
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].PurchOfsAc))
						{
							stringBuilder.Append("<PurchaseOffsetAcct>" + doc.OitwLines[num].PurchOfsAc + "</PurchaseOffsetAcct>");
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].ReturnAc))
						{
							stringBuilder.Append("<ReturningAccount>" + doc.OitwLines[num].ReturnAc + "</ReturningAccount>");
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].RevenuesAc))
						{
							stringBuilder.Append("<RevenuesAccount>" + doc.OitwLines[num].RevenuesAc + "</RevenuesAccount>");
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].ARCMAct))
						{
							stringBuilder.Append("<SalesCreditAcc>" + doc.OitwLines[num].ARCMAct + "</SalesCreditAcc>");
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].ARCMEUAct))
						{
							stringBuilder.Append("<SalesCreditEUAcc>" + doc.OitwLines[num].ARCMEUAct + "</SalesCreditEUAcc>");
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].ARCMFrnAct))
						{
							stringBuilder.Append("<SalesCreditForeignAcc>" + doc.OitwLines[num].ARCMFrnAct + "</SalesCreditForeignAcc>");
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].ShpdGdsAct))
						{
							stringBuilder.Append("<ShippedGoodsAccount>" + doc.OitwLines[num].ShpdGdsAct + "</ShippedGoodsAccount>");
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].StokRvlAct))
						{
							stringBuilder.Append("<StockInflationAdjustAccount>" + doc.OitwLines[num].StokRvlAct + "</StockInflationAdjustAccount>");
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].StkOffsAct))
						{
							stringBuilder.Append("<StockInflationOffsetAccount>" + doc.OitwLines[num].StkOffsAct + "</StockInflationOffsetAccount>");
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].StkInTnAct))
						{
							stringBuilder.Append("<StockInTransitAccount>" + doc.OitwLines[num].StkInTnAct + "</StockInTransitAccount>");
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].TransferAc))
						{
							stringBuilder.Append("<TransferAccount>" + doc.OitwLines[num].TransferAc + "</TransferAccount>");
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].VarianceAc))
						{
							stringBuilder.Append("<VarienceAccount>" + doc.OitwLines[num].VarianceAc + "</VarienceAccount>");
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].VatRevAct))
						{
							stringBuilder.Append("<VATInRevenueAccount>" + doc.OitwLines[num].VatRevAct + "</VATInRevenueAccount>");
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].WhICenAct))
						{
							stringBuilder.Append("<WHIncomingCenvatAccount>" + doc.OitwLines[num].WhICenAct + "</WHIncomingCenvatAccount>");
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].WhOCenAct))
						{
							stringBuilder.Append("<WHOutgoingCenvatAccount>" + doc.OitwLines[num].WhOCenAct + "</WHOutgoingCenvatAccount>");
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].WipAcct))
						{
							stringBuilder.Append("<WipAccount>" + doc.OitwLines[num].WipAcct + "</WipAccount>");
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].WipOffset))
						{
							stringBuilder.Append("<WipOffsetProfitAndLossAccount>" + doc.OitwLines[num].WipOffset + "</WipOffsetProfitAndLossAccount>");
						}
						if (!string.IsNullOrWhiteSpace(doc.OitwLines[num].WipVarAcct))
						{
							stringBuilder.Append("<WipVarianceAccount>" + doc.OitwLines[num].WipVarAcct + "</WipVarianceAccount>");
						}
					}
					stringBuilder.Append(DataService.MappingUdf(doc.OitwLines[num].UserFields, commandType, ignoreEmptyValue));
					stringBuilder.Append("</row>");
				}
				stringBuilder.Append("</ItemWarehouseInfo>");
			}
			if (doc.ListITM12 != null && doc.ListITM12.Count > 0)
			{
				stringBuilder.Append("<ItemUnitOfMeasurement>");
				for (int num2 = 0; num2 < doc.ListITM12.Count; num2++)
				{
					stringBuilder.Append("<row>");
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
					stringBuilder.Append("<UoMType>" + Function.ToString(itemUoMTypeEnum) + "</UoMType>");
					stringBuilder.Append("<UoMEntry>" + Function.ToString(doc.ListITM12[num2].UomEntry) + "</UoMEntry>");
					if (doc.ListITM12[num2].UomType == "I")
					{
						stringBuilder.Append("<Length1>0.000000</Length1>\r\n                            <Length2>0.000000</Length2>\r\n                            <Width1>0.000000</Width1>\r\n                            <Width2>0.000000</Width2>\r\n                            <Height1>0.000000</Height1>\r\n                            <Height2>0.000000</Height2>\r\n                            <Volume>0.000000</Volume>\r\n                            <Weight1>0.000000</Weight1\r\n                            ><Weight2>0.000000</Weight2>");
					}
					else
					{
						stringBuilder.Append("<Length1>" + Function.ParseDecimal(doc.ListITM12[num2].Length1) + "</Length1>");
						stringBuilder.Append("<Length1Unit>" + Function.RawText(doc.ListITM12[num2].Len1Unit) + "</Length1Unit>");
						stringBuilder.Append("<Length2>" + Function.ParseDecimal(doc.ListITM12[num2].Length2) + "</Length2>");
						stringBuilder.Append("<Width1>" + Function.ParseDecimal(doc.ListITM12[num2].Width1) + "</Width1>");
						stringBuilder.Append("<Width1Unit>" + Function.RawText(doc.ListITM12[num2].Wdth1Unit) + "</Width1Unit>");
						stringBuilder.Append("<Width2>" + Function.ParseDecimal(doc.ListITM12[num2].Width2) + "</Width2>");
						stringBuilder.Append("<Height1>" + Function.ParseDecimal(doc.ListITM12[num2].Height1) + "</Height1>");
						stringBuilder.Append("<Height1Unit>" + Function.RawText(doc.ListITM12[num2].Hght1Unit) + "</Height1Unit>");
						stringBuilder.Append("<Height2>" + Function.ParseDecimal(doc.ListITM12[num2].Height2) + "</Height2>");
						stringBuilder.Append("<Volume>" + Function.ParseDecimal(doc.ListITM12[num2].Volume) + "</Volume>");
						stringBuilder.Append("<VolumeUnit>" + Function.RawText(doc.ListITM12[num2].VolUnit) + "</VolumeUnit>");
						stringBuilder.Append("<Weight1>" + Function.ParseDecimal(doc.ListITM12[num2].Weight1) + "</Weight1>");
						stringBuilder.Append("<Weight1Unit>" + Function.RawText(doc.ListITM12[num2].Wght1Unit) + "</Weight1Unit>");
						stringBuilder.Append("<Weight2>" + Function.ParseDecimal(doc.ListITM12[num2].Weight2) + "</Weight2>");
					}
					stringBuilder.Append("</row>");
				}
				stringBuilder.Append("</ItemUnitOfMeasurement>");
			}
			if (doc.ListItemPreferredVendors != null && doc.ListItemPreferredVendors.Count > 0)
			{
				stringBuilder.Append("<ItemPreferredVendors>");
				for (int num3 = 0; num3 < doc.ListItemPreferredVendors.Count; num3++)
				{
					stringBuilder.Append("<row>");
					stringBuilder.Append("<BPCode>" + Function.RawText(doc.ListItemPreferredVendors[num3].VendorCode) + "</BPCode>");
					stringBuilder.Append("</row>");
				}
				stringBuilder.Append("</ItemPreferredVendors>");
			}
			stringBuilder.Append("</BO></BOM>");
			goto end_IL_0006;
			IL_09b0:
			string text8;
			stringBuilder.Append("<FrozenFrom>" + text8 + "</FrozenFrom>");
			string text9;
			if (doc.frozenTo.HasValue)
			{
				_ = doc.frozenTo.Value;
				if (doc.frozenTo.Value.Year > 2000)
				{
					text9 = doc.frozenTo.Value.ToString("yyyyMMdd");
					goto IL_0a2a;
				}
			}
			text9 = "";
			goto IL_0a2a;
			IL_08f9:
			BoYesNoEnum boYesNoEnum3 = ((doc.frozenFor == "Y") ? BoYesNoEnum.tYES : BoYesNoEnum.tNO);
			stringBuilder.Append("<Frozen>" + Function.ToString(boYesNoEnum3) + "</Frozen>");
			if (doc.frozenFor == "Y")
			{
				if (doc.frozenFrom.HasValue)
				{
					_ = doc.frozenFrom.Value;
					if (doc.frozenFrom.Value.Year > 2000)
					{
						text8 = doc.frozenFrom.Value.ToString("yyyyMMdd");
						goto IL_09b0;
					}
				}
				text8 = "";
				goto IL_09b0;
			}
			goto IL_0a5b;
			IL_0a2a:
			stringBuilder.Append("<FrozenTo>" + text9 + "</FrozenTo>");
			stringBuilder.Append("<FrozenRemarks>" + Function.RawText(doc.FrozenComm) + "</FrozenRemarks>");
			goto IL_0a5b;
			IL_08c8:
			stringBuilder.Append("<ValidTo>" + text7 + "</ValidTo>");
			stringBuilder.Append("<ValidRemarks>" + Function.RawText(doc.ValidComm) + "</ValidRemarks>");
			goto IL_08f9;
			end_IL_0006:;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		return stringBuilder.ToString();
	}

	protected bool IsAutoAddWarehouse()
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("Select top 1 \"AutoITW\" from OADM;", CommandType.Text);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ToString(dataTable.Rows[0][0]) == "Y";
			}
			return true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return true;
		}
	}

	protected string UpdateAtcEntry(string itemCode, int atcEntry)
	{
		string empty = string.Empty;
		try
		{
			string query = "UPDATE OITM SET \"AtcEntry\"=@AtcEntry  WHERE \"ItemCode\"=@ItemCode;";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@AtcEntry", atcEntry));
			list.Add(new SqlParameter("@ItemCode", itemCode));
			if (ExecuteNonQuery(query, CommandType.Text, list.ToArray()) > 0)
			{
				return null;
			}
			return "ERROR";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public string GetByParams(string sessionId, string itemCode)
	{
		try
		{
			string xmlObject = "<?xml version=\"1.0\" ?>  \r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>ItemsService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>\r\n                                    <GetByParams xmlns=\"ItemsService\">\r\n                                     <Item>   <ItemCode>" + itemCode + "</ItemCode></Item>\r\n                                    </GetByParams>  \r\n                                </soap:Body>  \r\n                            </soap:Envelope>  ";
			XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					throw new Exception(xElement2.Value);
				}
			}
			return xmlElement.OuterXml;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return string.Empty;
	}

	public virtual string UpdateUoMPrices(List<Items_UoMPricesImpl> lst, string sessionId)
	{
		try
		{
			string byParams = GetByParams(sessionId, lst[0].ItemCode);
			if (string.IsNullOrEmpty(byParams))
			{
				throw new Exception("[UpdateUoMPrices].[GetByPr] No matching record found.");
			}
			XElement xElement = XElement.Parse(byParams).Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Item"));
			DeleteUoMPrices(lst, sessionId, lst[0].ItemCode);
			if (!string.IsNullOrEmpty(Function.ToString(xElement)))
			{
				string text = ProcessUoMPrices(xElement, lst);
				if (!string.IsNullOrEmpty(text))
				{
					text = text.Replace("xmlns=\"\"", "");
					string xmlObject = "<?xml version=\"1.0\" ?>  \r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>ItemsService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>\r\n                                    <Update xmlns=\"ItemsService\">" + Function.ToString(text) + "</Update>\r\n                                </soap:Body>  \r\n                            </soap:Envelope> ";
					XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
					XElement xElement2 = XElement.Parse(xmlElement.OuterXml);
					if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
					{
						XElement xElement3 = xElement2.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
						if (xElement3 != null)
						{
							throw new Exception(xElement3.Value);
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
		return string.Empty;
	}

	private string ProcessUoMPrices(XElement itemXml, List<Items_UoMPricesImpl> lst)
	{
		try
		{
			XElement xElement = itemXml.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("ItemPrices"));
			if (xElement != null && xElement.Descendants().Any((XElement t) => t.Name.LocalName.Equals("ItemPrice")))
			{
				IEnumerable<XElement> enumerable = from t in xElement.Descendants()
					where t.Name.LocalName.Equals("ItemPrice")
					select t;
				XElement xElement2 = null;
				foreach (XElement item in enumerable)
				{
					if (item.Descendants().Any((XElement t) => t.Name.LocalName.Equals("PriceList") && Function.ParseShort(t.Value) == lst[0].PriceList))
					{
						xElement2 = item;
						break;
					}
				}
				if (xElement2 != null && xElement2.Descendants().Any((XElement t) => t.Name.LocalName.Equals("UoMPrices")))
				{
					XElement xElement3 = xElement2.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("UoMPrices"));
					xElement3.Descendants().Remove();
					foreach (Items_UoMPricesImpl item2 in lst)
					{
						string text = "<UoMPrice>";
						text = text + "<PriceList>" + Function.ToString(item2.PriceList) + "</PriceList>";
						text = text + "<UoMEntry>" + Function.ToString(item2.UomEntry) + "</UoMEntry>";
						text = text + "<ReduceBy>" + Function.ToString(item2.Factor) + "</ReduceBy>";
						text = text + "<Price>" + Function.ToString(item2.Price) + "</Price>";
						text = (string.IsNullOrWhiteSpace(item2.Currency) ? (text + "<Currency/>") : (text + "<Currency>" + item2.Currency + "</Currency>"));
						text = text + "<AdditionalReduceBy1>" + Function.ToString(item2.Factor1) + "</AdditionalReduceBy1>";
						text = text + "<AdditionalPrice1>" + Function.ToString(item2.AddPrice1) + "</AdditionalPrice1>";
						text = (string.IsNullOrWhiteSpace(item2.Currency1) ? (text + "<AdditionalCurrency1/>") : (text + "<AdditionalCurrency1>" + item2.Currency1 + "</AdditionalCurrency1>"));
						text = text + "<AdditionalReduceBy2>" + Function.ToString(item2.Factor2) + "</AdditionalReduceBy2>";
						text = text + "<AdditionalPrice2>" + Function.ToString(item2.AddPrice2) + "</AdditionalPrice2>";
						text = (string.IsNullOrWhiteSpace(item2.Currency2) ? (text + "<AdditionalCurrency2/>") : (text + "<AdditionalCurrency2>" + item2.Currency2 + "</AdditionalCurrency2>"));
						string text2 = ((Function.ToString(item2.AutoUpdate) == "Y") ? "tYES" : "tNO");
						text = text + "<Auto>" + text2 + "</Auto>";
						text += "</UoMPrice>";
						int num = text.IndexOf('<');
						if (num > 0)
						{
							text = text.Remove(0, num);
						}
						xElement3.Add(XElement.Parse(text));
					}
					return Function.ToString(itemXml);
				}
			}
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "";
		}
	}

	private string DeleteUoMPrices(List<Items_UoMPricesImpl> lst, string sessionId, string itemCode)
	{
		try
		{
			string byParams = GetByParams(sessionId, itemCode);
			if (string.IsNullOrEmpty(byParams))
			{
				throw new Exception("[DI-SERVER] No matching record found.");
			}
			XElement xElement = XElement.Parse(byParams).Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Item"));
			DataSet dataSet = new DataSet();
			using (StringReader reader = new StringReader(Function.ToString(xElement)))
			{
				dataSet.ReadXml(reader);
			}
			if (dataSet.Tables.Contains("UoMPrice"))
			{
				DataTable dataTable = dataSet.Tables["UoMPrice"];
				var list = (from u in dataTable.AsEnumerable()
					let PriceList = Function.ParseInt(u["PriceList"])
					let UomEntry = Function.ParseInt(u["UomEntry"])
					select new
					{
						priceList = Function.ParseInt(PriceList),
						uomEntry = Function.ParseInt(UomEntry)
					} into line
					where !lst.Exists((Items_UoMPricesImpl t) => t.PriceList == line.priceList && t.UomEntry == line.uomEntry)
					select line).ToList();
				if (list.Count <= 0)
				{
					return string.Empty;
				}
				for (int num = 0; num < dataTable.Rows.Count; num++)
				{
					string text = Function.ToString(dataTable.Rows[num]["Auto"]);
					if (!(text == "tYES"))
					{
						if (text == "tNO")
						{
							dataTable.Rows[num]["Auto"] = "N";
						}
					}
					else
					{
						dataTable.Rows[num]["Auto"] = "Y";
					}
				}
				foreach (var i in list)
				{
					DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["PriceList"]) == i.priceList && Function.ParseInt(t["UomEntry"]) == i.uomEntry);
					if (dataRow != null)
					{
						dataTable.Rows.Remove(dataRow);
					}
				}
				dataTable.AcceptChanges();
				for (int num2 = 0; num2 < dataTable.Rows.Count; num2++)
				{
					string text = Function.ToString(dataTable.Rows[num2]["Auto"]);
					if (!(text == "Y"))
					{
						if (text == "N")
						{
							dataTable.Rows[num2]["Auto"] = "tNO";
						}
					}
					else
					{
						dataTable.Rows[num2]["Auto"] = "tYES";
					}
				}
				string xml = dataSet.GetXml();
				if (!string.IsNullOrEmpty(xml))
				{
					string xmlObject = "<?xml version=\"1.0\" ?>\r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>ItemsService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>\r\n                                    <Update xmlns=\"ItemsService\">" + xml + "</Update>\r\n                                </soap:Body>  \r\n                            </soap:Envelope> ";
					XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
					XElement xElement2 = XElement.Parse(xmlElement.OuterXml);
					if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
					{
						XElement xElement3 = xElement2.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
						if (xElement3 != null)
						{
							throw new Exception(xElement3.Value);
						}
					}
					return string.Empty;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
		return string.Empty;
	}

	protected string DeleteUoMInItem(List<Items_UoMImpl> lst, string sessionId, string itemCode)
	{
		try
		{
			string byParams = GetByParams(sessionId, itemCode);
			if (string.IsNullOrEmpty(byParams))
			{
				throw new Exception("[DI-SERVER] No matching record found.");
			}
			XElement xElement = XElement.Parse(byParams).Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Item"));
			DataSet dataSet = new DataSet();
			using (StringReader reader = new StringReader(Function.ToString(xElement)))
			{
				dataSet.ReadXml(reader);
			}
			if (dataSet.Tables.Contains("ItemUnitOfMeasurement"))
			{
				DataTable dataTable = dataSet.Tables["ItemUnitOfMeasurement"];
				var list = (from u in dataTable.AsEnumerable()
					let UomType = ((Function.ToString(u["UomType"]) == "iutPurchasing") ? "P" : ((Function.ToString(u["UomType"]) == "iutSales") ? "S" : ((Function.ToString(u["UomType"]) == "iutInventory") ? "I" : "")))
					let UomEntry = Function.ParseInt(u["UomEntry"])
					select new
					{
						uomType = Function.ToString(UomType),
						uomEntry = Function.ParseInt(UomEntry)
					} into line
					where !lst.Exists((Items_UoMImpl t) => t.UomType.ToUpper() == line.uomType.ToUpper() && t.UomEntry == line.uomEntry)
					select line).ToList();
				if (list.Count <= 0)
				{
					return string.Empty;
				}
				for (int num = 0; num < dataTable.Rows.Count; num++)
				{
					switch (Function.ToString(dataTable.Rows[num]["UomType"]))
					{
					case "iutPurchasing":
						dataTable.Rows[num]["UomType"] = "P";
						break;
					case "iutSales":
						dataTable.Rows[num]["UomType"] = "S";
						break;
					case "iutInventory":
						dataTable.Rows[num]["UomType"] = "I";
						break;
					}
				}
				foreach (var i in list)
				{
					DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["UomType"]) == i.uomType && Function.ParseInt(t["UomEntry"]) == i.uomEntry);
					if (dataRow != null)
					{
						dataTable.Rows.Remove(dataRow);
					}
				}
				dataTable.AcceptChanges();
				for (int num2 = 0; num2 < dataTable.Rows.Count; num2++)
				{
					switch (Function.ToString(dataTable.Rows[num2]["UomType"]))
					{
					case "P":
						dataTable.Rows[num2]["UomType"] = "iutPurchasing";
						break;
					case "S":
						dataTable.Rows[num2]["UomType"] = "iutSales";
						break;
					case "I":
						dataTable.Rows[num2]["UomType"] = "iutInventory";
						break;
					}
				}
				string xml = dataSet.GetXml();
				if (!string.IsNullOrEmpty(xml))
				{
					string xmlObject = "<?xml version=\"1.0\" ?>\r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>ItemsService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>\r\n                                    <Update xmlns=\"ItemsService\">" + xml + "</Update>\r\n                                </soap:Body>  \r\n                            </soap:Envelope> ";
					XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
					XElement xElement2 = XElement.Parse(xmlElement.OuterXml);
					if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
					{
						XElement xElement3 = xElement2.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
						if (xElement3 != null)
						{
							throw new Exception(xElement3.Value);
						}
					}
					return string.Empty;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
		return string.Empty;
	}

	protected void DeletePreferredVendors(List<ItemPreferredVendorsImpl> lst, string sessionId, string itemCode)
	{
		try
		{
			string byParams = GetByParams(sessionId, itemCode);
			if (string.IsNullOrEmpty(byParams))
			{
				throw new Exception("[DI-SERVER] No matching record found.");
			}
			XElement xElement = XElement.Parse(byParams).Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Item"));
			DataSet dataSet = new DataSet();
			using (StringReader reader = new StringReader(Function.ToString(xElement)))
			{
				dataSet.ReadXml(reader);
			}
			if (!dataSet.Tables.Contains("ItemPreferredVendor"))
			{
				return;
			}
			DataTable dataTable = dataSet.Tables["ItemPreferredVendor"];
			List<DataRow> list = (from dataRow2 in dataTable.Copy().AsEnumerable()
				where !lst.Exists((ItemPreferredVendorsImpl t) => string.Compare(t.VendorCode, Function.ToString(dataRow2["BPCode"]), ignoreCase: true) == 0)
				select dataRow2).ToList();
			if (list.Count <= 0)
			{
				return;
			}
			foreach (DataRow r in list)
			{
				DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => string.Compare(Function.ToString(t["BPCode"]), Function.ToString(r["BPCode"]), ignoreCase: true) == 0);
				if (dataRow != null)
				{
					dataTable.Rows.Remove(dataRow);
				}
			}
			dataTable.AcceptChanges();
			string xml = dataSet.GetXml();
			if (string.IsNullOrEmpty(xml))
			{
				return;
			}
			string xmlObject = "<?xml version=\"1.0\" ?>\r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>ItemsService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>\r\n                                    <Update xmlns=\"ItemsService\">" + xml + "</Update>\r\n                                </soap:Body>  \r\n                            </soap:Envelope> ";
			XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
			XElement xElement2 = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement3 = xElement2.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement3 != null)
				{
					throw new Exception(xElement3.Value);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	public void Dispose()
	{
	}

	public virtual string Remove(string itemCode, string sessionId)
	{
		string result = "";
		try
		{
			string xmlObject = "<?xml version=\"1.0\" ?>  \r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>ItemsService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>\r\n                                    <Remove xmlns=\"ItemsService\">\r\n                                     <Item>   <ItemCode>" + itemCode + "</ItemCode></Item>\r\n                                    </Remove>  \r\n                                </soap:Body>  \r\n                            </soap:Envelope>  ";
			XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					return xElement2.Value;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			result = ex.Message;
		}
		return result;
	}
}
