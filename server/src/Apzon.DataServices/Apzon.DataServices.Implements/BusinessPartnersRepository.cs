using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.DataServices.Interfaces;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.MasterData;
using Apzon.Services;
using SAPbobsCOM;

namespace Apzon.DataServices.Implements;

public class BusinessPartnersRepository : Repository<BPImpls>, Apzon.DataServices.Interfaces.IBusinessPartners, IRepository<BPImpls>, IDisposable
{
	protected readonly IUnitOfWork _unitOfWork;

	public BusinessPartnersRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, dataService)
	{
		_unitOfWork = unitOfWork;
	}

	public HttpResult Save(BPImpls doc, string sessionId, bool ignoreEmptyvalue)
	{
		try
		{
			bool flag = (!Exists(doc.CardCode, sessionId) && doc.OldCardCode == "") || (!Exists(doc.OldCardCode, sessionId) && doc.OldCardCode != "");
			if (doc.AttachmentFiles != null && doc.AttachmentFiles.Count != 0)
			{
				HttpResult httpResult = SaveAttachment(doc, sessionId, flag);
				if (httpResult.MessageCode != Enums.MessageCode.Success)
				{
					return httpResult;
				}
				doc.AtcEntry = Function.ParseInt(httpResult.Content);
			}
			if (flag)
			{
				return AddDis(doc, sessionId);
			}
			return UpdateDis(doc, sessionId);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected virtual HttpResult SaveAttachment(BPImpls doc, string sessionId, bool addNew)
	{
		try
		{
			return ServiceUnitOfWork.Files.SaveAttachmentFiles(doc.AttachmentFiles, new Dictionary<string, object> { { "CardCode,string", doc.CardCode } }, "OCRD");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public HttpResult AddOrUpdate(BPImpls doc, string sessionId, bool ignoreEmptyvalue)
	{
		try
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("CardCode,string", doc.CardCode);
			HttpResult httpResult = ServiceUnitOfWork.Files.SaveAttachmentFiles(doc.AttachmentFiles, dictionary, "OCRD");
			if (httpResult.MessageCode != Enums.MessageCode.Success)
			{
				return httpResult;
			}
			doc.AtcEntry = Function.ParseInt(httpResult.Content);
			if ((!Exists(doc.CardCode, sessionId) && doc.OldCardCode == "") || (!Exists(doc.OldCardCode, sessionId) && doc.OldCardCode != ""))
			{
				return AddDis(doc, sessionId);
			}
			return UpdateDis(doc, sessionId);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public virtual bool Exists(string p, string sessionId)
	{
		try
		{
			string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?>\r\n                            <env:Envelope xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                             <env:Header>\r\n                              <SessionID>" + sessionId + "</SessionID> \r\n                             </env:Header>\r\n                             <env:Body>\r\n                              <dis:GetByKey xmlns:dis=\"http://www.sap.com/SBO/DIS\">\r\n                               <Object>oBusinessPartners</Object> \r\n                               <CardCode>" + Function.RawText(p) + "</CardCode> \r\n                              </dis:GetByKey>\r\n                             </env:Body>\r\n                            </env:Envelope>";
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

	public virtual bool CheckExistsPriorities(int prioCode, string sessionId)
	{
		return false;
	}

	public void Dispose()
	{
	}

	public virtual HttpResult UpdateDis(BPImpls doc, string sessionId)
	{
		try
		{
			string message = "";
			StringBuilder stringBuilder = new StringBuilder();
			if (!doc.CardCode.Equals(doc.OldCardCode))
			{
				stringBuilder.Append("<dis:RemoveObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">\r\n                                    <BOM>\r\n                                        <BO>\r\n                                            <AdmInfo>\r\n                                                <Object>oBusinessPartners</Object>\r\n                                            </AdmInfo>\r\n                                            <QueryParams><CardCode>" + doc.OldCardCode + "</CardCode></QueryParams>\r\n                                        </BO>\r\n                                    </BOM>\r\n                                </dis:RemoveObject>");
			}
			StringBuilder stringBuilder2 = new StringBuilder("<?xml version=\"1.0\" encoding=\"UTF-16\"?>");
			stringBuilder2.Append("<env:Envelope xmlns:env = \"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                                <env:Header>\r\n                                 <SessionID>" + sessionId + "</SessionID></env:Header>\r\n                                <env:Body>" + stringBuilder);
			string value = MappingDis(doc, (!doc.CardCode.Equals(doc.OldCardCode)) ? EnumCommandType.Add : EnumCommandType.Update, sessionId);
			stringBuilder2.Append(value);
			stringBuilder2.Append("</env:Body></env:Envelope>");
			XmlElement xmlElement = DataService.Process(stringBuilder2.ToString()).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					message = xElement2.Value;
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), message);
					return new HttpResult(Enums.MessageCode.Error, message);
				}
			}
			else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
			{
				XElement xElement3 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("RetKey"));
				if (xElement3 != null)
				{
					UpdateBpAccounts(doc, sessionId);
					DeleteBpAddress(doc, sessionId);
					DeleteContactEmp(doc, sessionId);
					DeleteBpPaymentDate(doc, sessionId);
					DeleteBpBankAccount(doc, sessionId);
					DeleteBPBranchAssignment(doc, sessionId);
					string text = Function.ToString(xElement3.Value);
					Logging.Write("WATCH", $"update BP master - {text}");
					message = "";
					return new HttpResult(Enums.MessageCode.Success, "", text);
				}
			}
			return new HttpResult(Enums.MessageCode.Success, message);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public virtual HttpResult AddDis(BPImpls doc, string sessionId)
	{
		try
		{
			string value = MappingDis(doc, EnumCommandType.Add, sessionId);
			StringBuilder stringBuilder = new StringBuilder("<?xml version=\"1.0\" encoding=\"UTF-16\"?>");
			stringBuilder.Append("<env:Envelope xmlns:env = \"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                                <env:Header>\r\n                                 <SessionID>" + sessionId + "</SessionID></env:Header><env:Body>");
			stringBuilder.Append(value);
			stringBuilder.Append("</env:Body></env:Envelope>");
			XmlElement xmlElement = DataService.Process(stringBuilder.ToString()).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					Logging.Write("ERROR", "Add BP master error: " + xElement2.Value);
					return new HttpResult(Enums.MessageCode.Error, xElement2.Value);
				}
			}
			else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
			{
				XElement xElement3 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("RetKey"));
				if (xElement3 != null)
				{
					string text = Function.ToString(xElement3.Value);
					Logging.Write("WATCH", $"Add BP master - {text}");
					return new HttpResult(Enums.MessageCode.Success, "", text);
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

	public virtual string MappingDis(BPImpls doc, EnumCommandType commandType, string sessionId)
	{
		StringBuilder stringBuilder = new StringBuilder();
		try
		{
			if (commandType == EnumCommandType.Update)
			{
				stringBuilder.Append("<dis:UpdateObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">");
			}
			else
			{
				stringBuilder.Append("<dis:AddObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">");
			}
			stringBuilder.Append("<BOM>\r\n                          <BO xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                          <AdmInfo>\r\n                            <Object>oBusinessPartners</Object>\r\n                          </AdmInfo>");
			if (commandType == EnumCommandType.Update)
			{
				stringBuilder.Append("<QueryParams><CardCode>" + Function.RawText(doc.OldCardCode) + "</CardCode></QueryParams>");
			}
			stringBuilder.Append("<BusinessPartners>\r\n                            <row>");
			if (!doc.Series.Equals(0) && commandType == EnumCommandType.Add)
			{
				stringBuilder.Append("<Series>" + Function.ParseInt(doc.Series) + "</Series>");
			}
			if (!string.IsNullOrWhiteSpace(doc.Password))
			{
				stringBuilder.Append("<Password>" + Function.RawText(doc.Password) + "</Password>");
			}
			stringBuilder.Append("<Website>" + Function.RawText(doc.IntrntSite) + "</Website>");
			if (!string.IsNullOrWhiteSpace(doc.AddID))
			{
				stringBuilder.Append("<AdditionalID>" + Function.RawText(doc.AddID) + "</AdditionalID>");
			}
			if (!string.IsNullOrWhiteSpace(doc.VatIdUnCmp))
			{
				stringBuilder.Append("<UnifiedFederalTaxID>" + Function.RawText(doc.VatIdUnCmp) + "</UnifiedFederalTaxID>");
			}
			if (Function.ParseDecimal(doc.IntrstRate) > 0m)
			{
				stringBuilder.Append("<IntrestRatePercent>" + doc.IntrstRate + "</IntrestRatePercent>");
			}
			if (!string.IsNullOrWhiteSpace(doc.Address))
			{
				stringBuilder.Append("<Address>" + Function.RawText(doc.Address) + "</Address>");
			}
			if (!string.IsNullOrWhiteSpace(doc.ShipToDef))
			{
				stringBuilder.Append("<ShipToDefault>" + Function.RawText(doc.ShipToDef) + "</ShipToDefault>");
			}
			if (!string.IsNullOrWhiteSpace(doc.Building))
			{
				stringBuilder.Append("<BillToBuildingFloorRoom>" + Function.RawText(doc.Building) + "</BillToBuildingFloorRoom>");
			}
			if (!string.IsNullOrWhiteSpace(doc.MailBuildi))
			{
				stringBuilder.Append("<ShipToBuildingFloorRoom>" + Function.RawText(doc.MailBuildi) + "</ShipToBuildingFloorRoom>");
			}
			if (!string.IsNullOrWhiteSpace(doc.BillToDef))
			{
				stringBuilder.Append("<BilltoDefault>" + Function.RawText(doc.BillToDef) + "</BilltoDefault>");
			}
			if (!string.IsNullOrWhiteSpace(doc.DflAccount))
			{
				stringBuilder.Append("<DefaultAccount>" + Function.RawText(doc.DflAccount) + "</DefaultAccount>");
				stringBuilder.Append("<DefaultBankCode>" + Function.RawText(doc.BankCode) + "</DefaultBankCode>");
				stringBuilder.Append("<BankCountry>" + Function.RawText(doc.BankCountr) + "</BankCountry>");
			}
			if (!string.IsNullOrWhiteSpace(doc.CardCode) && commandType == EnumCommandType.Add)
			{
				stringBuilder.Append("<CardCode>" + Function.RawText(doc.CardCode) + "</CardCode>");
			}
			stringBuilder.Append("<CardName>" + Function.RawText(doc.CardName) + "</CardName>");
			stringBuilder.Append("<CardForeignName>" + Function.RawText(doc.CardFName) + "</CardForeignName>");
			stringBuilder.Append("<CardType>" + Function.RawText(doc.CardType switch
			{
				"C" => BoCardTypes.cCustomer, 
				"L" => BoCardTypes.cLid, 
				"S" => BoCardTypes.cSupplier, 
				_ => BoCardTypes.cCustomer, 
			}) + "</CardType>");
			if (!string.IsNullOrWhiteSpace(doc.CntctPrsn))
			{
				stringBuilder.Append("<ContactPerson>" + Function.RawText(doc.CntctPrsn) + "</ContactPerson>");
			}
			if (Function.ParseInt(doc.SlpCode) > 0)
			{
				stringBuilder.Append("<SalesPersonCode>" + doc.SlpCode + "</SalesPersonCode>");
			}
			else
			{
				stringBuilder.Append("<SalesPersonCode>-1</SalesPersonCode>");
			}
			if (!string.IsNullOrWhiteSpace(doc.Cellular))
			{
				stringBuilder.Append("<Cellular>" + Function.RawText(doc.Cellular) + "</Cellular>");
			}
			if (Function.ParseDecimal(doc.CreditCard) > 0m)
			{
				stringBuilder.Append("<CreditCardCode>" + Function.ToString(doc.CreditCard) + "</CreditCardCode>");
			}
			if (doc.CardValid.HasValue && doc.CardValid.Value.Year > 1900)
			{
				stringBuilder.Append("<CreditCardExpiration>" + Function.ParseDateTime(doc.CardValid).Value.ToString("yyyyMMdd") + "</CreditCardExpiration>");
			}
			if (Function.ParseDecimal(doc.CreditLine) > 0m)
			{
				stringBuilder.Append("<CreditLimit>" + Function.ToString(doc.CreditLine) + "</CreditLimit>");
			}
			if (Function.ParseDecimal(doc.DebtLine) > 0m)
			{
				stringBuilder.Append("<MaxCommitment>" + Function.ToString(doc.DebtLine) + "</MaxCommitment>");
			}
			if (!string.IsNullOrWhiteSpace(doc.Currency))
			{
				stringBuilder.Append("<Currency>" + Function.RawText(doc.Currency) + "</Currency>");
			}
			stringBuilder.Append("<DiscountPercent>" + Function.ToString(doc.Discount) + "</DiscountPercent>");
			stringBuilder.Append("<EmailAddress>" + Function.RawText(doc.E_Mail) + "</EmailAddress>");
			if (!string.IsNullOrWhiteSpace(doc.ExportCode))
			{
				stringBuilder.Append("<ExportCode>" + Function.RawText(doc.ExportCode) + "</ExportCode>");
			}
			stringBuilder.Append("<Fax>" + Function.RawText(doc.Fax) + "</Fax>");
			stringBuilder.Append("<FederalTaxID>" + Function.RawText(doc.LicTradNum) + "</FederalTaxID>");
			stringBuilder.Append("<Picture>" + Function.RawText(doc.Picture) + "</Picture>");
			stringBuilder.Append("<FreeText>" + Function.RawText(doc.Free_Text) + "</FreeText>");
			if (Function.ParseInt(doc.GroupCode) > 0)
			{
				stringBuilder.Append("<GroupCode>" + Function.ToString(doc.GroupCode) + "</GroupCode>");
			}
			if (!string.IsNullOrWhiteSpace(doc.MailBlock))
			{
				stringBuilder.Append("<MailAddress>" + Function.RawText(doc.MailBlock) + "</MailAddress>");
			}
			stringBuilder.Append("<Notes>" + Function.RawText(doc.Notes) + "</Notes>");
			if (!string.IsNullOrWhiteSpace(doc.OwnerIdNum))
			{
				stringBuilder.Append("<OwnerIDNumber>" + Function.RawText(doc.OwnerIdNum) + "</OwnerIDNumber>");
			}
			if (Function.ParseInt(doc.GroupNum) > 0)
			{
				stringBuilder.Append("<PayTermsGrpCode>" + Function.ToString(doc.GroupNum) + "</PayTermsGrpCode>");
			}
			stringBuilder.Append("<Phone1>" + Function.RawText(doc.Phone1) + "</Phone1>");
			stringBuilder.Append("<Phone2>" + Function.RawText(doc.Phone2) + "</Phone2>");
			if (Function.ParseInt(doc.ListNum) != 0)
			{
				stringBuilder.Append("<PriceListNum>" + Function.ToString(doc.ListNum) + "</PriceListNum>");
			}
			if (Function.ParseInt(doc.ShipType) > 0)
			{
				stringBuilder.Append("<ShippingType>" + Function.ToString(doc.ShipType) + "</ShippingType>");
			}
			if (doc.Priority > 0)
			{
				stringBuilder.Append("<Priority>" + Function.ToString(doc.Priority) + "</Priority>");
			}
			stringBuilder.Append("<IBAN>" + Function.RawText(doc.IBAN) + "</IBAN>");
			if (doc.Priority > 0)
			{
				stringBuilder.Append("<Priority>" + Function.ToString(doc.Priority) + "</Priority>");
			}
			string text = ((Function.ToString(doc.PartDelivr) == "Y") ? Function.ToString(BoYesNoEnum.tYES) : Function.ToString(BoYesNoEnum.tNO));
			stringBuilder.Append("<PartialDelivery>" + text + "</PartialDelivery>");
			string text2 = ((Function.ToString(doc.BackOrder) == "Y") ? Function.ToString(BoYesNoEnum.tYES) : Function.ToString(BoYesNoEnum.tNO));
			stringBuilder.Append("<BackOrder>" + text2 + "</BackOrder>");
			string text3 = ((Function.ToString(doc.NoDiscount) == "Y") ? Function.ToString(BoYesNoEnum.tYES) : Function.ToString(BoYesNoEnum.tNO));
			stringBuilder.Append("<NoDiscounts>" + text3 + "</NoDiscounts>");
			string text4 = ((Function.ToString(doc.EdrsFromBP) == "Y") ? Function.ToString(BoYesNoEnum.tYES) : Function.ToString(BoYesNoEnum.tNO));
			stringBuilder.Append("<EndorsableChecksFromBP>" + text4 + "</EndorsableChecksFromBP>");
			string text5 = ((Function.ToString(doc.EdrsToBP) == "Y") ? Function.ToString(BoYesNoEnum.tYES) : Function.ToString(BoYesNoEnum.tNO));
			stringBuilder.Append("<AcceptsEndorsedChecks>" + text5 + "</AcceptsEndorsedChecks>");
			stringBuilder.Append("<Indicator>" + Function.RawText(doc.Indicator) + "</Indicator>");
			stringBuilder.Append("<ProjectCode>" + Function.RawText(doc.ProjectCod) + "</ProjectCode>");
			if (Function.ParseInt(doc.IndustryC) > 0)
			{
				stringBuilder.Append("<Industry>" + Function.RawText(doc.IndustryC) + "</Industry>");
			}
			if (!string.IsNullOrEmpty(doc.CmpPrivate))
			{
				stringBuilder.Append("<CompanyPrivate>" + Function.RawText(doc.CmpPrivate) + "</CompanyPrivate>");
			}
			stringBuilder.Append("<AliasName>" + Function.RawText(doc.AliasName) + "</AliasName>");
			stringBuilder.Append("<ChannelBP>" + Function.RawText(doc.ChannlBP) + "</ChannelBP>");
			if (Function.ParseInt(doc.DfTcnician) > 0)
			{
				stringBuilder.Append("<DefaultTechnician>" + Function.ToString(doc.DfTcnician) + "</DefaultTechnician>");
			}
			if (Function.ParseInt(doc.Territory) > 0)
			{
				stringBuilder.Append("<Territory>" + Function.ToString(doc.Territory) + "</Territory>");
			}
			if (doc.LangCode > 0)
			{
				stringBuilder.Append("<LanguageCode>" + doc.LangCode + "</LanguageCode>");
			}
			stringBuilder.Append("<GlobalLocationNumber>" + Function.RawText(doc.GlblLocNum) + "</GlobalLocationNumber>");
			string text6 = ((Function.ToString(doc.validFor) == "Y") ? Function.ToString(BoYesNoEnum.tYES) : Function.ToString(BoYesNoEnum.tNO));
			stringBuilder.Append("<Valid>" + text6 + "</Valid>");
			string text7;
			if (Function.ToString(doc.validFor) == "Y")
			{
				if (doc.validFrom.HasValue)
				{
					_ = doc.validFrom.Value;
					if (doc.validFrom.Value.Year > 2000)
					{
						text7 = doc.validFrom.Value.ToString("yyyyMMdd");
						goto IL_0cc3;
					}
				}
				text7 = "";
				goto IL_0cc3;
			}
			goto IL_0d6e;
			IL_0ee6:
			if (doc.ItemProperties != null && doc.ItemProperties.Count > 0)
			{
				for (int i = 0; i < doc.ItemProperties.Count; i++)
				{
					BoYesNoEnum boYesNoEnum = ((doc.ItemProperties[i] == "Y") ? BoYesNoEnum.tYES : BoYesNoEnum.tNO);
					stringBuilder.Append("<Properties" + (i + 1) + ">" + boYesNoEnum.ToString() + "</Properties" + (i + 1) + ">");
				}
			}
			stringBuilder.Append("<FatherCard>" + doc.FatherCard + "</FatherCard>");
			if (!string.IsNullOrEmpty(doc.FatherType))
			{
				stringBuilder.Append("<FatherType>" + doc.FatherType + "</FatherType>");
			}
			stringBuilder.Append("<DebitorAccount>" + doc.DebPayAcct + "</DebitorAccount>");
			stringBuilder.Append("<DownPaymentClearAct>" + doc.DpmClear + "</DownPaymentClearAct>");
			stringBuilder.Append("<DownPaymentInterimAccount>" + doc.DpmIntAct + "</DownPaymentInterimAccount>");
			stringBuilder.Append("<LinkedBusinessPartner>" + doc.ConnBP + "</LinkedBusinessPartner>");
			stringBuilder.Append("<PlanningGroup>" + doc.PlngGroup + "</PlanningGroup>");
			if (string.Compare(doc.Affiliate, "y", ignoreCase: true) == 0)
			{
				stringBuilder.Append("<Affiliate>tYES</Affiliate>");
			}
			else
			{
				stringBuilder.Append("<Affiliate>tNO</Affiliate>");
			}
			stringBuilder.Append("<VatLiable>" + doc.VatStatus + "</VatLiable>");
			stringBuilder.Append("<VatGroup>" + doc.ECVatGroup + "</VatGroup>");
			if (!string.IsNullOrEmpty(doc.HousBnkCry))
			{
				stringBuilder.Append("<HouseBankCountry>" + doc.HousBnkCry + "</HouseBankCountry>");
				stringBuilder.Append("<HouseBank>" + doc.HouseBank + "</HouseBank>");
				stringBuilder.Append("<HouseBankAccount>" + doc.HousBnkAct + "</HouseBankAccount>");
				stringBuilder.Append("<ReferenceDetails>" + doc.RefDetails + "</ReferenceDetails>");
				if (string.Compare(doc.CollecAuth, "y", ignoreCase: true) == 0)
				{
					stringBuilder.Append("<CollectionAuthorization>tYES</CollectionAuthorization>");
				}
				else
				{
					stringBuilder.Append("<CollectionAuthorization>tNO</CollectionAuthorization>");
				}
				stringBuilder.Append("<BankChargesAllocationCode>" + Function.ToString(doc.BCACode) + "</BankChargesAllocationCode>");
			}
			stringBuilder.Append(DataService.MappingUdf(doc.ListUserFields));
			stringBuilder.Append("</row>\r\n                        </BusinessPartners>");
			stringBuilder.Append("<AccountRecivablePayables>");
			if (doc.AccountRecivablePayables != null && doc.AccountRecivablePayables.Count > 0)
			{
				for (int j = 0; j < doc.AccountRecivablePayables.Count; j++)
				{
					if (string.IsNullOrWhiteSpace(doc.AccountRecivablePayables[j].AcctCode))
					{
						continue;
					}
					stringBuilder.Append("<row>");
					stringBuilder.Append("<AccountCode>" + doc.AccountRecivablePayables[j].AcctCode + "</AccountCode>");
					string text8 = "";
					string acctType = doc.AccountRecivablePayables[j].AcctType;
					if (!(acctType == "D"))
					{
						if (acctType == "O")
						{
							text8 = "bpat_OpenDebts";
						}
					}
					else
					{
						text8 = "bpat_DownPayment";
					}
					stringBuilder.Append("<AccountType>" + text8 + "</AccountType>");
					stringBuilder.Append("<BPCode>" + Function.RawText(doc.AccountRecivablePayables[j].CardCode) + "</BPCode>");
					stringBuilder.Append("</row>");
				}
			}
			stringBuilder.Append("</AccountRecivablePayables>");
			if (doc.Addresses != null && doc.Addresses.Count > 0)
			{
				stringBuilder.Append("<BPAddresses>");
				for (int k = 0; k < doc.Addresses.Count; k++)
				{
					stringBuilder.Append("<row>");
					stringBuilder.Append("<AddressName>" + Function.RawText(doc.Addresses[k].Address) + "</AddressName>");
					BoAddressType boAddressType = ((Function.ToString(doc.Addresses[k].AdresType) == "B") ? BoAddressType.bo_BillTo : BoAddressType.bo_ShipTo);
					stringBuilder.Append("<AddressType>" + Function.RawText(boAddressType) + "</AddressType>");
					stringBuilder.Append("<Street>" + Function.RawText(doc.Addresses[k].Street) + "</Street>");
					stringBuilder.Append("<StreetNo>" + Function.RawText(doc.Addresses[k].StreetNo) + "</StreetNo>");
					stringBuilder.Append("<Block>" + Function.RawText(doc.Addresses[k].Block) + "</Block>");
					stringBuilder.Append("<City>" + Function.RawText(doc.Addresses[k].City) + "</City>");
					stringBuilder.Append("<ZipCode>" + Function.RawText(doc.Addresses[k].ZipCode) + "</ZipCode>");
					stringBuilder.Append("<County>" + Function.RawText(doc.Addresses[k].County) + "</County>");
					stringBuilder.Append("<State>" + Function.RawText(doc.Addresses[k].State) + "</State>");
					stringBuilder.Append("<Country>" + Function.RawText(doc.Addresses[k].Country) + "</Country>");
					stringBuilder.Append("<BuildingFloorRoom>" + Function.RawText(doc.Addresses[k].Building) + "</BuildingFloorRoom>");
					stringBuilder.Append("<GlobalLocationNumber>" + Function.RawText(doc.Addresses[k].GlblLocNum) + "</GlobalLocationNumber>");
					stringBuilder.Append(DataService.MappingUdf(doc.Addresses[k].UserFields));
					stringBuilder.Append("</row>");
				}
				stringBuilder.Append("</BPAddresses>");
			}
			stringBuilder.Append("<ContactEmployees>");
			if (doc.ListContactPerson != null && doc.ListContactPerson.Count > 0)
			{
				for (int l = 0; l < doc.ListContactPerson.Count; l++)
				{
					stringBuilder.Append("<row>");
					ContactPersonImpl contactPersonImpl = doc.ListContactPerson[l];
					stringBuilder.Append("<Active>" + ((contactPersonImpl.Active == "Y") ? "tYES" : "tNO") + "</Active>");
					if (!string.IsNullOrWhiteSpace(contactPersonImpl.Address))
					{
						stringBuilder.Append("<Address>" + Function.RawText(contactPersonImpl.Address) + "</Address>");
					}
					if (!string.IsNullOrWhiteSpace(contactPersonImpl.CardCode))
					{
						stringBuilder.Append("<CardCode>" + Function.RawText(contactPersonImpl.CardCode) + "</CardCode>");
					}
					if (!string.IsNullOrWhiteSpace(contactPersonImpl.BirthCity))
					{
						stringBuilder.Append("<CityOfBirth>" + Function.RawText(contactPersonImpl.BirthCity) + "</CityOfBirth>");
					}
					if (contactPersonImpl.BirthDate.HasValue && contactPersonImpl.BirthDate.Value.Year > 1900)
					{
						stringBuilder.Append("<DateOfBirth>" + Function.ParseDateTime(contactPersonImpl.BirthDate).Value.ToString("yyyyMMdd") + "</DateOfBirth>");
					}
					if (!string.IsNullOrWhiteSpace(contactPersonImpl.E_MailL))
					{
						stringBuilder.Append("<E_Mail>" + Function.RawText(contactPersonImpl.E_MailL) + "</E_Mail>");
					}
					if (!string.IsNullOrWhiteSpace(contactPersonImpl.Fax))
					{
						stringBuilder.Append("<Fax>" + Function.RawText(contactPersonImpl.Fax) + "</Fax>");
					}
					if (!string.IsNullOrWhiteSpace(contactPersonImpl.FirstName))
					{
						stringBuilder.Append("<FirstName>" + Function.RawText(contactPersonImpl.FirstName) + "</FirstName>");
					}
					stringBuilder.Append("<Gender>" + Function.RawText(contactPersonImpl.Gender switch
					{
						"M" => BoGenderTypes.gt_Male, 
						"F" => BoGenderTypes.gt_Female, 
						"E" => BoGenderTypes.gt_Undefined, 
						_ => BoGenderTypes.gt_Undefined, 
					}) + "</Gender>");
					if (!string.IsNullOrWhiteSpace(contactPersonImpl.Name))
					{
						stringBuilder.Append("<Name>" + Function.RawText(contactPersonImpl.Name) + "</Name>");
					}
					if (!string.IsNullOrWhiteSpace(contactPersonImpl.LastName))
					{
						stringBuilder.Append("<LastName>" + Function.RawText(contactPersonImpl.LastName) + "</LastName>");
					}
					if (!string.IsNullOrWhiteSpace(contactPersonImpl.Cellolar))
					{
						stringBuilder.Append("<MobilePhone>" + Function.RawText(contactPersonImpl.Cellolar) + "</MobilePhone>");
					}
					if (!string.IsNullOrWhiteSpace(contactPersonImpl.Notes1))
					{
						stringBuilder.Append("<Remarks1>" + Function.RawText(contactPersonImpl.Notes1) + "</Remarks1>");
					}
					if (!string.IsNullOrWhiteSpace(contactPersonImpl.Notes2))
					{
						stringBuilder.Append("<Remarks2>" + Function.RawText(contactPersonImpl.Notes2) + "</Remarks2>");
					}
					if (!string.IsNullOrWhiteSpace(contactPersonImpl.Tel1))
					{
						stringBuilder.Append("<Phone1>" + Function.RawText(contactPersonImpl.Tel1) + "</Phone1>");
					}
					if (!string.IsNullOrWhiteSpace(contactPersonImpl.Tel2))
					{
						stringBuilder.Append("<Phone2>" + Function.RawText(contactPersonImpl.Tel2) + "</Phone2>");
					}
					if (!string.IsNullOrWhiteSpace(contactPersonImpl.BirthPlace))
					{
						stringBuilder.Append("<PlaceOfBirth>" + Function.RawText(contactPersonImpl.BirthPlace) + "</PlaceOfBirth>");
					}
					if (!string.IsNullOrWhiteSpace(contactPersonImpl.Position))
					{
						stringBuilder.Append("<Position>" + Function.RawText(contactPersonImpl.Position) + "</Position>");
					}
					if (!string.IsNullOrWhiteSpace(contactPersonImpl.Title))
					{
						stringBuilder.Append("<Title>" + Function.RawText(contactPersonImpl.Title) + "</Title>");
					}
					if (!string.IsNullOrWhiteSpace(contactPersonImpl.Profession))
					{
						stringBuilder.Append("<Profession>" + Function.RawText(contactPersonImpl.Profession) + "</Profession>");
					}
					if (!string.IsNullOrWhiteSpace(contactPersonImpl.Password))
					{
						stringBuilder.Append("<Password>" + Function.RawText(contactPersonImpl.Password) + "</Password>");
					}
					if (!string.IsNullOrWhiteSpace(contactPersonImpl.Pager))
					{
						stringBuilder.Append("<Pager>" + Function.RawText(contactPersonImpl.Pager) + "</Pager>");
					}
					stringBuilder.Append(DataService.MappingUdf(contactPersonImpl.UserFields, commandType));
					stringBuilder.Append("</row>");
				}
			}
			stringBuilder.Append("</ContactEmployees>");
			if (doc.BPBankAccounts != null && doc.BPBankAccounts.Count > 0)
			{
				stringBuilder.Append("<BPBankAccounts>");
				for (int m = 0; m < doc.BPBankAccounts.Count; m++)
				{
					stringBuilder.Append("<row>");
					stringBuilder.Append("<AccountName>" + Function.RawText(doc.BPBankAccounts[m].AcctName) + "</AccountName>");
					stringBuilder.Append("<AccountNo>" + Function.RawText(doc.BPBankAccounts[m].Account) + "</AccountNo>");
					stringBuilder.Append("<BankCode>" + Function.RawText(doc.BPBankAccounts[m].BankCode) + "</BankCode>");
					stringBuilder.Append("<BICSwiftCode>" + Function.RawText(doc.BPBankAccounts[m].SwiftNum) + "</BICSwiftCode>");
					stringBuilder.Append("<Block>" + Function.RawText(doc.BPBankAccounts[m].Block) + "</Block>");
					stringBuilder.Append("<BPCode>" + Function.RawText(doc.CardCode) + "</BPCode>");
					stringBuilder.Append("<Branch>" + Function.RawText(doc.BPBankAccounts[m].Branch) + "</Branch>");
					stringBuilder.Append("<City>" + Function.RawText(doc.BPBankAccounts[m].City) + "</City>");
					stringBuilder.Append("<ControlKey>" + Function.RawText(doc.BPBankAccounts[m].ControlKey) + "</ControlKey>");
					stringBuilder.Append("<Country>" + Function.RawText(doc.BPBankAccounts[m].Country) + "</Country>");
					stringBuilder.Append("<County>" + Function.RawText(doc.BPBankAccounts[m].County) + "</County>");
					stringBuilder.Append("<IBAN>" + Function.RawText(doc.BPBankAccounts[m].IBAN) + "</IBAN>");
					stringBuilder.Append("<LogInstance>" + Function.RawText(doc.BPBankAccounts[m].LogInstanc) + "</LogInstance>");
					if (doc.BPBankAccounts[m].MandatDate.HasValue)
					{
						_ = doc.BPBankAccounts[m].MandatDate.Value;
						stringBuilder.Append("<SignatureDate>" + doc.BPBankAccounts[m].MandatDate.Value.ToString("yyyyMMdd") + "</SignatureDate>");
					}
					stringBuilder.Append("<MandateID>" + Function.RawText(doc.BPBankAccounts[m].MandateID) + "</MandateID>");
					stringBuilder.Append("<Phone>" + Function.RawText(doc.BPBankAccounts[m].Phone) + "</Phone>");
					if (doc.BPBankAccounts[m].SignDate.HasValue)
					{
						_ = doc.BPBankAccounts[m].SignDate.Value;
						stringBuilder.Append("<SignatureDate>" + doc.BPBankAccounts[m].SignDate.Value.ToString("yyyyMMdd") + "</SignatureDate>");
					}
					stringBuilder.Append("<State>" + Function.RawText(doc.BPBankAccounts[m].State) + "</State>");
					stringBuilder.Append("<Street>" + Function.RawText(doc.BPBankAccounts[m].Street) + "</Street>");
					stringBuilder.Append("<UserNo1>" + Function.RawText(doc.BPBankAccounts[m].UsrNumber1) + "</UserNo1>");
					stringBuilder.Append("<UserNo2>" + Function.RawText(doc.BPBankAccounts[m].UsrNumber2) + "</UserNo2>");
					stringBuilder.Append("<UserNo3>" + Function.RawText(doc.BPBankAccounts[m].UsrNumber3) + "</UserNo3>");
					stringBuilder.Append("<UserNo4>" + Function.RawText(doc.BPBankAccounts[m].UsrNumber4) + "</UserNo4>");
					stringBuilder.Append("<ZipCode>" + Function.RawText(doc.BPBankAccounts[m].ZipCode) + "</ZipCode>");
					stringBuilder.Append("</row>");
				}
				stringBuilder.Append("</BPBankAccounts>");
			}
			stringBuilder.Append("<BPPaymentDates>");
			if (doc.BPPaymentDates != null && doc.BPPaymentDates.Count > 0)
			{
				for (int n = 0; n < doc.BPPaymentDates.Count; n++)
				{
					stringBuilder.Append("<row>");
					stringBuilder.Append("<PaymentDate>" + Function.RawText(doc.BPPaymentDates[n].PmntDate) + "</PaymentDate>");
					stringBuilder.Append("</row>");
				}
			}
			stringBuilder.Append("</BPPaymentDates>");
			stringBuilder.Append("<BPBranchAssignment>");
			if (doc.BPBranchAssignment != null && doc.BPBranchAssignment.Count > 0)
			{
				for (int num = 0; num < doc.BPBranchAssignment.Count; num++)
				{
					stringBuilder.Append("<row>");
					stringBuilder.Append("<BPLID>" + doc.BPBranchAssignment[num].BPLId + "</BPLID>");
					stringBuilder.Append("</row>");
				}
			}
			stringBuilder.Append("</BPBranchAssignment>");
			stringBuilder.Append("</BO></BOM>");
			if (commandType == EnumCommandType.Update)
			{
				stringBuilder.Append("</dis:UpdateObject>");
			}
			else
			{
				stringBuilder.Append("</dis:AddObject>");
			}
			goto end_IL_0006;
			IL_0d3d:
			string text9;
			stringBuilder.Append("<ValidTo>" + text9 + "</ValidTo>");
			stringBuilder.Append("<ValidRemarks>" + Function.RawText(doc.ValidComm) + "</ValidRemarks>");
			goto IL_0d6e;
			IL_0eb5:
			string text10;
			stringBuilder.Append("<FrozenTo>" + text10 + "</FrozenTo>");
			stringBuilder.Append("<FrozenRemarks>" + Function.RawText(doc.FrozenComm) + "</FrozenRemarks>");
			goto IL_0ee6;
			IL_0cc3:
			stringBuilder.Append("<ValidFrom>" + text7 + "</ValidFrom>");
			if (doc.validTo.HasValue)
			{
				_ = doc.validTo.Value;
				if (doc.validTo.Value.Year > 2000)
				{
					text9 = doc.validTo.Value.ToString("yyyyMMdd");
					goto IL_0d3d;
				}
			}
			text9 = "";
			goto IL_0d3d;
			IL_0e3b:
			string text11;
			stringBuilder.Append("<FrozenFrom>" + text11 + "</FrozenFrom>");
			if (doc.frozenTo.HasValue)
			{
				_ = doc.frozenTo.Value;
				if (doc.frozenTo.Value.Year > 2000)
				{
					text10 = doc.frozenTo.Value.ToString("yyyyMMdd");
					goto IL_0eb5;
				}
			}
			text10 = "";
			goto IL_0eb5;
			IL_0d6e:
			string text12 = ((Function.ToString(doc.frozenFor) == "Y") ? Function.ToString(BoYesNoEnum.tYES) : Function.ToString(BoYesNoEnum.tNO));
			stringBuilder.Append("<Frozen>" + text12 + "</Frozen>");
			if (Function.ToString(doc.frozenFor) == "Y")
			{
				if (doc.frozenFrom.HasValue)
				{
					_ = doc.frozenFrom.Value;
					if (doc.frozenFrom.Value.Year > 2000)
					{
						text11 = doc.frozenFrom.Value.ToString("yyyyMMdd");
						goto IL_0e3b;
					}
				}
				text11 = "";
				goto IL_0e3b;
			}
			goto IL_0ee6;
			end_IL_0006:;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return stringBuilder.ToString();
	}

	public virtual string Remove(string cardCode, string sessionId)
	{
		string result = "";
		try
		{
			string xmlObject = "<?xml version=\"1.0\" ?>  \r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>BusinessPartnersService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>\r\n                                    <Remove xmlns=\"BusinessPartnersService\">\r\n                                     <BusinessPartner>   <CardCode>" + cardCode + "</CardCode></BusinessPartner>\r\n                                    </Remove>  \r\n                                </soap:Body>  \r\n                            </soap:Envelope>  ";
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

	public virtual string RemoveMulti(List<string> lstCardCode, string sessionId)
	{
		string result = "";
		try
		{
			string text = "";
			foreach (string item in lstCardCode)
			{
				text = text + "<Remove xmlns=\"BusinessPartnersService\">\r\n                                     <BusinessPartner><CardCode>" + item + "</CardCode></BusinessPartner>\r\n                              </Remove> ";
			}
			string xmlObject = "<?xml version=\"1.0\" ?>  \r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>BusinessPartnersService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>\r\n                                     " + text + "\r\n                                </soap:Body>  \r\n                            </soap:Envelope>  ";
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

	public virtual HttpResult SaveBPPriorities(DataTable dt, string sessionId)
	{
		return new HttpResult();
	}

	public virtual HttpResult RemoveBPPriorities(DataTable dt, string sessionId)
	{
		return new HttpResult();
	}

	public virtual HttpResult SaveBPProperties(DataTable dt, string sessionId)
	{
		HttpResult httpResult = new HttpResult();
		try
		{
			StringBuilder stringBuilder = new StringBuilder("<?xml version=\"1.0\"?>  \r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>BusinessPartnerPropertiesService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>");
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				stringBuilder.Append("<UpdateBusinessPartnerProperty xmlns=\"BusinessPartnerPropertiesService\">\r\n                                    <BusinessPartnerProperty>\r\n                                        <PropertyCode>" + Function.ParseInt(dt.Rows[i]["GroupCode"]) + "</PropertyCode>\r\n                                        <PropertyName>" + Function.ToString(dt.Rows[i]["GroupName"]) + "</PropertyName>\r\n                                    </BusinessPartnerProperty>\r\n                                </UpdateBusinessPartnerProperty>");
			}
			stringBuilder.Append("</soap:Body>  \r\n                    </soap:Envelope>");
			httpResult.Message = UpdateProperties(stringBuilder.ToString(), sessionId);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			httpResult.Message = ex.Message;
		}
		return httpResult;
	}

	public string UpdateProperties(string xml, string sessionId)
	{
		try
		{
			XmlElement xmlElement = DataService.Process(xml).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					throw new Exception(xElement2.Value);
				}
			}
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return string.Empty;
	}

	public string GetByParams(string sessionId, BPImpls doc)
	{
		try
		{
			string xmlObject = "<?xml version=\"1.0\" ?>  \r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>BusinessPartnersService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>\r\n                                    <GetByParams xmlns=\"BusinessPartnersService\">\r\n                                     <BusinessPartner>   <CardCode>" + doc.CardCode + "</CardCode></BusinessPartner>\r\n                                    </GetByParams>  \r\n                                </soap:Body>  \r\n                            </soap:Envelope>  ";
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

	protected void DeleteBpAddress(BPImpls doc, string sessionId)
	{
		try
		{
			string byParams = GetByParams(sessionId, doc);
			if (string.IsNullOrEmpty(byParams))
			{
				throw new Exception("[DI-SERVER] No matching record found.");
			}
			XElement xElement = XElement.Parse(byParams).Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("BusinessPartner"));
			DataSet dataSet = new DataSet();
			using (StringReader reader = new StringReader(Function.ToString(xElement)))
			{
				dataSet.ReadXml(reader);
			}
			if (!dataSet.Tables.Contains("BPAddress"))
			{
				return;
			}
			DataTable dataTable = dataSet.Tables["BPAddress"];
			var list = (from u in dataTable.AsEnumerable()
				let addressType = ((Function.ToString(u["AddressType"]) == "bo_BillTo") ? "B" : ((Function.ToString(u["AddressType"]) == "bo_ShipTo") ? "S" : ""))
				let addressName = Function.ToString(u["AddressName"])
				select new
				{
					addrName = Function.ToString(addressName),
					addrType = Function.ToString(addressType)
				} into line
				where !doc.Addresses.Exists((AddressesImpl t) => t.Address.ToUpper() == line.addrName.ToUpper() && t.AdresType.ToUpper() == line.addrType.ToUpper())
				select line).ToList();
			if (list.Count <= 0)
			{
				return;
			}
			for (int num = 0; num < dataTable.Rows.Count; num++)
			{
				if (Function.ToString(dataTable.Rows[num]["AddressType"]) == "bo_BillTo")
				{
					dataTable.Rows[num]["AddressType"] = "B";
				}
				else if (Function.ToString(dataTable.Rows[num]["AddressType"]) == "bo_ShipTo")
				{
					dataTable.Rows[num]["AddressType"] = "S";
				}
			}
			foreach (var i in list)
			{
				DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["AddressName"]) == i.addrName && Function.ToString(t["AddressType"]) == i.addrType);
				if (dataRow != null)
				{
					dataTable.Rows.Remove(dataRow);
				}
			}
			for (int num2 = 0; num2 < dataTable.Rows.Count; num2++)
			{
				if (Function.ToString(dataTable.Rows[num2]["AddressType"]) == "B")
				{
					dataTable.Rows[num2]["AddressType"] = "bo_BillTo";
				}
				else if (Function.ToString(dataTable.Rows[num2]["AddressType"]) == "S")
				{
					dataTable.Rows[num2]["AddressType"] = "bo_ShipTo";
				}
			}
			string xml = dataSet.GetXml();
			if (string.IsNullOrEmpty(xml))
			{
				return;
			}
			string xmlObject = "<?xml version=\"1.0\" ?>\r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>BusinessPartnersService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>\r\n                                    <Update xmlns=\"BusinessPartnersService\">" + xml + "</Update>\r\n                                </soap:Body>  \r\n                            </soap:Envelope> ";
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

	protected void DeleteBpBankAccount(BPImpls doc, string sessionId)
	{
		try
		{
			string byParams = GetByParams(sessionId, doc);
			if (string.IsNullOrEmpty(byParams))
			{
				throw new Exception("[DI-SERVER] No matching record found.");
			}
			XElement xElement = XElement.Parse(byParams).Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("BusinessPartner"));
			DataSet dataSet = new DataSet();
			using (StringReader reader = new StringReader(Function.ToString(xElement)))
			{
				dataSet.ReadXml(reader);
			}
			if (!dataSet.Tables.Contains("BPBankAccounts"))
			{
				return;
			}
			DataTable dataTable = dataSet.Tables["BPBankAccounts"];
			var list = (from u in dataTable.AsEnumerable()
				let vCountry = Function.ToString(u["Country"])
				let vBankCode = Function.ToString(u["BankCode"])
				let vAccountNo = Function.ToString(u["AccountNo"])
				select new
				{
					Country = Function.ToString(vCountry),
					BankCode = Function.ToString(vBankCode),
					AccountNo = Function.ToString(vAccountNo)
				} into line
				where !doc.BPBankAccounts.Exists((BankAccountsImpl t) => t.Country.ToUpper() == line.Country.ToUpper() && t.BankCode.ToUpper() == line.BankCode.ToUpper() && t.Account.ToUpper() == line.AccountNo.ToUpper())
				select line).ToList();
			if (list.Count <= 0)
			{
				return;
			}
			foreach (var i in list)
			{
				DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["Country"]).ToUpper() == i.Country.ToUpper() && Function.ToString(t["BankCode"]).ToUpper() == i.BankCode.ToUpper() && Function.ToString(t["AccountNo"]).ToUpper() == i.AccountNo.ToUpper());
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
			string xmlObject = "<?xml version=\"1.0\" ?>\r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>BusinessPartnersService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>\r\n                                    <Update xmlns=\"BusinessPartnersService\">" + xml + "</Update>\r\n                                </soap:Body>  \r\n                            </soap:Envelope> ";
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

	protected void DeleteContactEmp(BPImpls doc, string sessionId)
	{
		try
		{
			string byParams = GetByParams(sessionId, doc);
			if (string.IsNullOrEmpty(byParams))
			{
				throw new Exception("[DI-SERVER] No matching record found.");
			}
			XElement xElement = XElement.Parse(byParams).Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("BusinessPartner"));
			DataSet dataSet = new DataSet();
			using (StringReader reader = new StringReader(Function.ToString(xElement)))
			{
				dataSet.ReadXml(reader);
			}
			if (!dataSet.Tables.Contains("ContactEmployee"))
			{
				return;
			}
			DataTable dataTable = dataSet.Tables["ContactEmployee"];
			var list = (from row in dataTable.AsEnumerable()
				select new
				{
					Name = Function.ToString(row["Name"])
				} into line
				where !doc.ListContactPerson.Exists((ContactPersonImpl t) => t.Name.ToUpper() == line.Name.ToUpper())
				select line).ToList();
			if (list.Count <= 0)
			{
				return;
			}
			foreach (var i in list)
			{
				DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["Name"]) == i.Name);
				if (dataRow != null)
				{
					dataTable.Rows.Remove(dataRow);
				}
			}
			string xml = dataSet.GetXml();
			if (string.IsNullOrEmpty(xml))
			{
				return;
			}
			string xmlObject = "<?xml version=\"1.0\" ?>\r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>BusinessPartnersService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>\r\n                                    <Update xmlns=\"BusinessPartnersService\">" + xml + "</Update>\r\n                                </soap:Body>  \r\n                            </soap:Envelope> ";
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

	protected void DeleteBpPaymentDate(BPImpls doc, string sessionId)
	{
		try
		{
			string byParams = GetByParams(sessionId, doc);
			if (string.IsNullOrEmpty(byParams))
			{
				throw new Exception("[DI-SERVER] No matching record found.");
			}
			XElement xElement = XElement.Parse(byParams).Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("BusinessPartner"));
			DataSet dataSet = new DataSet();
			using (StringReader reader = new StringReader(Function.ToString(xElement)))
			{
				dataSet.ReadXml(reader);
			}
			if (!dataSet.Tables.Contains("BPPaymentDates"))
			{
				return;
			}
			DataTable dataTable = dataSet.Tables["BPPaymentDates"];
			var list = (from row in dataTable.AsEnumerable()
				select new
				{
					Date = Function.ToString(row["PaymentDate"])
				} into line
				where !doc.BPPaymentDates.Exists((PaymentDatesImpl t) => t.PmntDate.ToUpper() == line.Date.ToUpper())
				select line).ToList();
			if (list.Count <= 0)
			{
				return;
			}
			foreach (var i in list)
			{
				DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["PaymentDate"]) == i.Date);
				if (dataRow != null)
				{
					dataTable.Rows.Remove(dataRow);
				}
			}
			string xml = dataSet.GetXml();
			if (string.IsNullOrEmpty(xml))
			{
				return;
			}
			string xmlObject = "<?xml version=\"1.0\" ?>\r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>BusinessPartnersService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>\r\n                                    <Update xmlns=\"BusinessPartnersService\">" + xml + "</Update>\r\n                                </soap:Body>  \r\n                            </soap:Envelope> ";
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

	protected void DeleteBPBranchAssignment(BPImpls doc, string sessionId)
	{
		try
		{
			string byParams = GetByParams(sessionId, doc);
			if (string.IsNullOrEmpty(byParams))
			{
				throw new Exception("[DI-SERVER] No matching record found.");
			}
			XElement xElement = XElement.Parse(byParams).Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("BusinessPartner"));
			DataSet dataSet = new DataSet();
			using (StringReader reader = new StringReader(Function.ToString(xElement)))
			{
				dataSet.ReadXml(reader);
			}
			if (!dataSet.Tables.Contains("BPBranchAssignment"))
			{
				return;
			}
			DataTable dataTable = dataSet.Tables["BPBranchAssignmentItem"];
			var list = (from row in dataTable.AsEnumerable()
				select new
				{
					BPLID = Function.ParseInt(row["BPLID"])
				} into line
				where !doc.BPBranchAssignment.Exists((BPBranchAssignmentImpls t) => t.BPLId == line.BPLID)
				select line).ToList();
			if (list.Count <= 0)
			{
				return;
			}
			foreach (var i in list)
			{
				DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["BPLID"]) == i.BPLID);
				if (dataRow != null)
				{
					dataTable.Rows.Remove(dataRow);
				}
			}
			string xml = dataSet.GetXml();
			if (string.IsNullOrEmpty(xml))
			{
				return;
			}
			string xmlObject = "<?xml version=\"1.0\" ?>\r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>BusinessPartnersService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>\r\n                                    <Update xmlns=\"BusinessPartnersService\">" + xml + "</Update>\r\n                                </soap:Body>  \r\n                            </soap:Envelope> ";
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

	protected void UpdateBpAccounts(BPImpls doc, string sessionId)
	{
		try
		{
			if (doc.AccountRecivablePayables == null && doc.AccountRecivablePayables.Count != 2)
			{
				return;
			}
			BPAccountReceivablePaybleImpl bPAccountReceivablePaybleImpl = doc.AccountRecivablePayables.FirstOrDefault((BPAccountReceivablePaybleImpl a) => a.AcctType == "D");
			string value = ((bPAccountReceivablePaybleImpl == null) ? "" : bPAccountReceivablePaybleImpl.AcctCode);
			BPAccountReceivablePaybleImpl bPAccountReceivablePaybleImpl2 = doc.AccountRecivablePayables.FirstOrDefault((BPAccountReceivablePaybleImpl a) => a.AcctType == "O");
			string value2 = ((bPAccountReceivablePaybleImpl2 == null) ? "" : bPAccountReceivablePaybleImpl2.AcctCode);
			string byParams = GetByParams(sessionId, doc);
			if (string.IsNullOrEmpty(byParams))
			{
				throw new Exception("[DI-SERVER] No matching record found.");
			}
			XElement xElement = XElement.Parse(byParams).Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("BusinessPartner"));
			DataSet dataSet = new DataSet();
			using (StringReader reader = new StringReader(Function.ToString(xElement)))
			{
				dataSet.ReadXml(reader);
			}
			if (!dataSet.Tables.Contains("BPAccountReceivablePayble"))
			{
				return;
			}
			DataTable dataTable = dataSet.Tables["BPAccountReceivablePayble"];
			for (int num = 0; num < dataTable.Rows.Count; num++)
			{
				string text = Function.ToString(dataTable.Rows[num]["AccountType"]);
				if (text == "bpat_DownPayment")
				{
					dataTable.Rows[num]["AccountCode"] = value;
				}
				else if (text == "bpat_OpenDebts")
				{
					dataTable.Rows[num]["AccountCode"] = value2;
				}
			}
			dataTable.AcceptChanges();
			for (int num2 = dataTable.Rows.Count - 1; num2 >= 0; num2--)
			{
				if (string.IsNullOrWhiteSpace(Function.ToString(dataTable.Rows[num2]["AccountCode"])))
				{
					dataTable.Rows.Remove(dataTable.Rows[num2]);
				}
			}
			dataTable.AcceptChanges();
			string xml = dataSet.GetXml();
			if (string.IsNullOrEmpty(xml))
			{
				return;
			}
			string xmlObject = "<?xml version=\"1.0\" ?>\r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>BusinessPartnersService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>\r\n                                    <Update xmlns=\"BusinessPartnersService\">" + xml + "</Update>\r\n                                </soap:Body>  \r\n                            </soap:Envelope> ";
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
}
