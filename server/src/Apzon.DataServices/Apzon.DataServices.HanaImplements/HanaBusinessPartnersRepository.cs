using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.DataServices.Implements;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Entities.Models.Common;
using Apzon.Services;
using SAPbobsCOM;

namespace Apzon.DataServices.HanaImplements;

public class HanaBusinessPartnersRepository : BusinessPartnersRepository
{
	public HanaBusinessPartnersRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, unitOfWork, dataService)
	{
	}

	protected override HttpResult SaveAttachment(BPImpls doc, string sessionId, bool addNew)
	{
		try
		{
			return (!addNew && doc.AtcEntry > 0) ? _unitOfWork.Attachment2.Update(doc.AtcEntry, doc.AttachmentFiles, sessionId) : _unitOfWork.Attachment2.Add(doc.AttachmentFiles, sessionId);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override bool Exists(string p, string sessionId)
	{
		try
		{
			if (string.IsNullOrWhiteSpace(p))
			{
				return false;
			}
			return !string.IsNullOrEmpty(ExecuteScarlar("select 1 from OCRD where \"CardCode\" = @cardCode;", CommandType.Text, new SqlParameter("@cardCode", Function.ToString(p))));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return p != null && !p.Equals("");
		}
	}

	public override bool CheckExistsPriorities(int priorityCode, string sessionId)
	{
		try
		{
			string query = "SELECT 1 FROM OBPP WHERE \"PrioCode\"=@PrioCode;";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, new SqlParameter("@PrioCode", priorityCode));
			return (dataTable != null && dataTable.Rows.Count > 0) ? true : false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return false;
		}
	}

	public override HttpResult AddDis(BPImpls doc, string sessionId)
	{
		try
		{
			List<BodyRequestObject> list = new List<BodyRequestObject>();
			string text = "";
			string bodyObject = MappingDis(doc, EnumCommandType.Add, sessionId);
			ApzObjectTypes objectType = ApzObjectTypes.oBusinessPartners;
			list.Add(new BodyRequestObject
			{
				ObjectType = objectType,
				OutObjectKey = true,
				Key = Function.ToString(doc.CardCode),
				Action = Enums.HttpAction.AddNew,
				BodyObject = bodyObject
			});
			HttpResult httpResult = DataService.ProcessMutiTransaction(sessionId, list);
			string text2 = Function.ToString(httpResult.Content);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				text = httpResult.Message;
				Logging.Write("ERROR", "ADD BP master error: " + httpResult.Message);
			}
			else
			{
				Logging.Write("WATCH", $"ADD BP master - {text2}");
				text = "";
			}
			return new HttpResult(Enums.MessageCode.Success, text, text2);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override HttpResult UpdateDis(BPImpls doc, string sessionId)
	{
		try
		{
			List<BodyRequestObject> list = new List<BodyRequestObject>();
			string text = "";
			ApzObjectTypes objectType = ApzObjectTypes.oBusinessPartners;
			if (!doc.CardCode.Equals(doc.OldCardCode))
			{
				list.Add(new BodyRequestObject
				{
					Action = Enums.HttpAction.Delete,
					OutObjectKey = true,
					ObjectType = objectType,
					Key = Function.ToString(doc.OldCardCode)
				});
			}
			string bodyObject = MappingDis(doc, (!doc.CardCode.Equals(doc.OldCardCode)) ? EnumCommandType.Add : EnumCommandType.Update, sessionId);
			list.Add(new BodyRequestObject
			{
				ObjectType = objectType,
				OutObjectKey = true,
				Key = Function.ToString(doc.CardCode),
				Action = ((!doc.CardCode.Equals(doc.OldCardCode)) ? Enums.HttpAction.AddNew : Enums.HttpAction.Update),
				BodyObject = bodyObject
			});
			HttpResult httpResult = DataService.ProcessMutiTransaction(sessionId, list);
			string obj = Function.ToString(httpResult.Content);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				text = httpResult.Message;
				Logging.Write("ERROR", "UPDATE BP master error: " + httpResult.Message);
			}
			else
			{
				Logging.Write("WATCH", $"update BP master - {Function.ToString(doc.CardCode)}");
				text = "";
			}
			return new HttpResult(Enums.MessageCode.Success, text, obj);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override string Remove(string cardCode, string sessionId)
	{
		try
		{
			List<BodyRequestObject> list = new List<BodyRequestObject>();
			list.Add(new BodyRequestObject
			{
				ObjectType = ApzObjectTypes.oBusinessPartners,
				OutObjectKey = true,
				Key = cardCode,
				Action = Enums.HttpAction.Delete
			});
			HttpResult httpResult = DataService.ProcessMutiTransaction(sessionId, list);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				Logging.Write("ERROR", "Delete BP error: " + httpResult.Message);
				return httpResult.Message;
			}
			Logging.Write("WATCH", "Delete BP Success: " + Function.ToString(cardCode));
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public override string RemoveMulti(List<string> lstCardCode, string sessionId)
	{
		try
		{
			List<BodyRequestObject> list = new List<BodyRequestObject>();
			foreach (string item in lstCardCode)
			{
				list.Add(new BodyRequestObject
				{
					ObjectType = ApzObjectTypes.oBusinessPartners,
					OutObjectKey = true,
					Key = item,
					Action = Enums.HttpAction.Delete
				});
			}
			HttpResult httpResult = DataService.ProcessMutiTransaction(sessionId, list);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				Logging.Write("ERROR", "Delete BP error: " + httpResult.Message);
				return httpResult.Message;
			}
			Logging.Write("WATCH", "Delete BP Success: " + string.Join(", ", lstCardCode));
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public override string MappingDis(BPImpls doc, EnumCommandType commandType, string sessionId)
	{
		StringBuilder stringBuilder = new StringBuilder();
		try
		{
			stringBuilder.Append("{");
			if (!doc.Series.Equals(0) && commandType == EnumCommandType.Add)
			{
				stringBuilder.Append(Function.GetJsonString("Series", Function.ParseInt(doc.Series)));
			}
			if (!string.IsNullOrWhiteSpace(doc.Password))
			{
				stringBuilder.Append(Function.GetJsonString("Password", Function.ToString(doc.Password)));
			}
			stringBuilder.Append(Function.GetJsonString("Website", Function.ToString(doc.IntrntSite)));
			if (!string.IsNullOrWhiteSpace(doc.AddID))
			{
				stringBuilder.Append(Function.GetJsonString("AdditionalID", Function.ToString(doc.AddID)));
			}
			if (!string.IsNullOrWhiteSpace(doc.VatIdUnCmp))
			{
				stringBuilder.Append(Function.GetJsonString("UnifiedFederalTaxID", Function.ToString(doc.VatIdUnCmp)));
			}
			if (Function.ParseDecimal(doc.IntrstRate) > 0m)
			{
				stringBuilder.Append(Function.GetJsonString("IntrestRatePercent", doc.IntrstRate));
			}
			if (!string.IsNullOrWhiteSpace(doc.Address))
			{
				stringBuilder.Append(Function.GetJsonString("Address", Function.ToString(doc.Address)));
			}
			if (!string.IsNullOrWhiteSpace(doc.ShipToDef))
			{
				stringBuilder.Append(Function.GetJsonString("ShipToDefault", Function.ToString(doc.ShipToDef)));
			}
			if (!string.IsNullOrWhiteSpace(doc.Building))
			{
				stringBuilder.Append(Function.GetJsonString("BillToBuildingFloorRoom", Function.ToString(doc.Building)));
			}
			if (!string.IsNullOrWhiteSpace(doc.MailBuildi))
			{
				stringBuilder.Append(Function.GetJsonString("ShipToBuildingFloorRoom", Function.ToString(doc.MailBuildi)));
			}
			if (!string.IsNullOrWhiteSpace(doc.BillToDef))
			{
				stringBuilder.Append(Function.GetJsonString("BilltoDefault", Function.ToString(doc.BillToDef)));
			}
			if (!string.IsNullOrWhiteSpace(doc.DflAccount))
			{
				stringBuilder.Append(Function.GetJsonString("DefaultAccount", Function.ToString(doc.DflAccount)));
				stringBuilder.Append(Function.GetJsonString("DefaultBankCode", Function.ToString(doc.BankCode)));
				stringBuilder.Append(Function.GetJsonString("BankCountry", Function.ToString(doc.BankCountr)));
			}
			if (!string.IsNullOrWhiteSpace(doc.CardCode) && commandType == EnumCommandType.Add)
			{
				stringBuilder.Append(Function.GetJsonString("CardCode", Function.ToString(doc.CardCode)));
			}
			stringBuilder.Append(Function.GetJsonString("CardName", Function.ToString(doc.CardName)));
			stringBuilder.Append(Function.GetJsonString("CardForeignName", Function.ToString(doc.CardFName)));
			stringBuilder.Append(Function.GetJsonString("CardType", Function.ToString(doc.CardType switch
			{
				"C" => BoCardTypes.cCustomer, 
				"L" => BoCardTypes.cLid, 
				"S" => BoCardTypes.cSupplier, 
				_ => BoCardTypes.cCustomer, 
			})));
			if (!string.IsNullOrWhiteSpace(doc.CntctPrsn))
			{
				stringBuilder.Append(Function.GetJsonString("ContactPerson", Function.ToString(doc.CntctPrsn)));
			}
			if (Function.ParseInt(doc.SlpCode) > 0)
			{
				stringBuilder.Append(Function.GetJsonString("SalesPersonCode", doc.SlpCode));
			}
			else
			{
				stringBuilder.Append(Function.GetJsonString("SalesPersonCode", -1));
			}
			if (!string.IsNullOrWhiteSpace(doc.Cellular))
			{
				stringBuilder.Append(Function.GetJsonString("Cellular", Function.ToString(doc.Cellular)));
			}
			if (Function.ParseDecimal(doc.CreditCard) > 0m)
			{
				stringBuilder.Append(Function.GetJsonString("CreditCardCode", Function.ToString(doc.CreditCard)));
			}
			if (doc.CardValid.HasValue && doc.CardValid.Value.Year > 1900)
			{
				stringBuilder.Append(Function.GetJsonString("CreditCardExpiration", Function.ParseDateTime(doc.CardValid).Value.ToString("yyyyMMdd")));
			}
			if (Function.ParseDecimal(doc.CreditLine) > 0m)
			{
				stringBuilder.Append(Function.GetJsonString("CreditLimit", Function.ToString(doc.CreditLine)));
			}
			if (Function.ParseDecimal(doc.DebtLine) > 0m)
			{
				stringBuilder.Append(Function.GetJsonString("MaxCommitment", Function.ToString(doc.DebtLine)));
			}
			if (!string.IsNullOrWhiteSpace(doc.Currency))
			{
				stringBuilder.Append(Function.GetJsonString("Currency", Function.ToString(doc.Currency)));
			}
			stringBuilder.Append(Function.GetJsonString("DiscountPercent", Function.ToString(doc.Discount)));
			stringBuilder.Append(Function.GetJsonString("EmailAddress", Function.ToString(doc.E_Mail)));
			if (!string.IsNullOrWhiteSpace(doc.ExportCode))
			{
				stringBuilder.Append(Function.GetJsonString("ExportCode", Function.ToString(doc.ExportCode)));
			}
			stringBuilder.Append(Function.GetJsonString("Fax", Function.ToString(doc.Fax)));
			stringBuilder.Append(Function.GetJsonString("FederalTaxID", Function.ToString(doc.LicTradNum)));
			stringBuilder.Append(Function.GetJsonString("Picture", Function.ToString(doc.Picture)));
			stringBuilder.Append(Function.GetJsonString("FreeText", Function.ToString(doc.Free_Text)));
			if (Function.ParseInt(doc.GroupCode) > 0)
			{
				stringBuilder.Append(Function.GetJsonString("GroupCode", Function.ToString(doc.GroupCode)));
			}
			if (!string.IsNullOrWhiteSpace(doc.MailBlock))
			{
				stringBuilder.Append(Function.GetJsonString("MailAddress", Function.ToString(doc.MailBlock)));
			}
			stringBuilder.Append(Function.GetJsonString("Notes", Function.ToString(doc.Notes)));
			if (!string.IsNullOrWhiteSpace(doc.OwnerIdNum))
			{
				stringBuilder.Append(Function.GetJsonString("OwnerIDNumber", Function.ToString(doc.OwnerIdNum)));
			}
			if (Function.ParseInt(doc.GroupNum) > 0)
			{
				stringBuilder.Append(Function.GetJsonString("PayTermsGrpCode", Function.ToString(doc.GroupNum)));
			}
			stringBuilder.Append(Function.GetJsonString("Phone1", Function.ToString(doc.Phone1)));
			stringBuilder.Append(Function.GetJsonString("Phone2", Function.ToString(doc.Phone2)));
			if (Function.ParseInt(doc.ListNum) != 0)
			{
				stringBuilder.Append(Function.GetJsonString("PriceListNum", Function.ToString(doc.ListNum)));
			}
			if (Function.ParseInt(doc.ShipType) > 0)
			{
				stringBuilder.Append(Function.GetJsonString("ShippingType", Function.ToString(doc.ShipType)));
			}
			if (Function.ParseInt(doc.OwnerCode) > 0)
			{
				stringBuilder.Append(Function.GetJsonString("OwnerCode", Function.ToString(doc.OwnerCode)));
			}
			else if (commandType == EnumCommandType.Update)
			{
				stringBuilder.Append("OwnerCode: null,");
			}
			if (doc.Priority > 0)
			{
				stringBuilder.Append(Function.GetJsonString("Priority", Function.ToString(doc.Priority)));
			}
			stringBuilder.Append(Function.GetJsonString("IBAN", Function.ToString(doc.IBAN)));
			if (doc.Priority > 0)
			{
				stringBuilder.Append(Function.GetJsonString("Priority", Function.ToString(doc.Priority)));
			}
			string value = ((Function.ToString(doc.PartDelivr) == "N") ? Function.ToString(BoYesNoEnum.tNO) : Function.ToString(BoYesNoEnum.tYES));
			stringBuilder.Append(Function.GetJsonString("PartialDelivery", value));
			string value2 = ((Function.ToString(doc.BackOrder) == "N") ? Function.ToString(BoYesNoEnum.tNO) : Function.ToString(BoYesNoEnum.tYES));
			stringBuilder.Append(Function.GetJsonString("BackOrder", value2));
			string value3 = ((Function.ToString(doc.NoDiscount) == "Y") ? Function.ToString(BoYesNoEnum.tYES) : Function.ToString(BoYesNoEnum.tNO));
			stringBuilder.Append(Function.GetJsonString("NoDiscounts", value3));
			string value4 = ((Function.ToString(doc.EdrsFromBP) == "Y") ? Function.ToString(BoYesNoEnum.tYES) : Function.ToString(BoYesNoEnum.tNO));
			stringBuilder.Append(Function.GetJsonString("EndorsableChecksFromBP", value4));
			string value5 = ((Function.ToString(doc.EdrsToBP) == "Y") ? Function.ToString(BoYesNoEnum.tYES) : Function.ToString(BoYesNoEnum.tNO));
			stringBuilder.Append(Function.GetJsonString("AcceptsEndorsedChecks", value5));
			stringBuilder.Append(Function.GetJsonString("Indicator", Function.ToString(doc.Indicator)));
			stringBuilder.Append(Function.GetJsonString("ProjectCode", Function.ToString(doc.ProjectCod)));
			if (Function.ParseInt(doc.IndustryC) > 0)
			{
				stringBuilder.Append(Function.GetJsonString("Industry", Function.ToString(doc.IndustryC)));
			}
			if (!string.IsNullOrEmpty(doc.CmpPrivate))
			{
				stringBuilder.Append(Function.GetJsonString("CompanyPrivate", Function.ToString(doc.CmpPrivate)));
			}
			stringBuilder.Append(Function.GetJsonString("AliasName", Function.ToString(doc.AliasName)));
			stringBuilder.Append(Function.GetJsonString("ChannelBP", Function.ToString(doc.ChannlBP)));
			if (Function.ParseInt(doc.DfTcnician) > 0)
			{
				stringBuilder.Append(Function.GetJsonString("DefaultTechnician", Function.ToString(doc.DfTcnician)));
			}
			if (Function.ParseInt(doc.Territory) > 0)
			{
				stringBuilder.Append(Function.GetJsonString("Territory", Function.ToString(doc.Territory)));
			}
			if (doc.LangCode > 0)
			{
				stringBuilder.Append(Function.GetJsonString("LanguageCode", doc.LangCode));
			}
			stringBuilder.Append(Function.GetJsonString("GlobalLocationNumber", Function.ToString(doc.GlblLocNum)));
			string value6 = ((Function.ToString(doc.validFor) == "Y") ? Function.ToString(BoYesNoEnum.tYES) : Function.ToString(BoYesNoEnum.tNO));
			stringBuilder.Append(Function.GetJsonString("Valid", value6));
			string value7;
			if (Function.ToString(doc.validFor) == "Y")
			{
				if (doc.validFrom.HasValue)
				{
					_ = doc.validFrom.Value;
					if (doc.validFrom.Value.Year > 2000)
					{
						value7 = doc.validFrom.Value.ToString("yyyyMMdd");
						goto IL_0b84;
					}
				}
				value7 = "";
				goto IL_0b84;
			}
			goto IL_0c20;
			IL_0d5d:
			string value8;
			stringBuilder.Append(Function.GetJsonString("FrozenTo", value8));
			stringBuilder.Append(Function.GetJsonString("FrozenRemarks", Function.ToString(doc.FrozenComm)));
			goto IL_0d84;
			IL_0c20:
			string value9 = ((Function.ToString(doc.frozenFor) == "Y") ? Function.ToString(BoYesNoEnum.tYES) : Function.ToString(BoYesNoEnum.tNO));
			stringBuilder.Append(Function.GetJsonString("Frozen", value9));
			string value10;
			if (Function.ToString(doc.frozenFor) == "Y")
			{
				if (doc.frozenFrom.HasValue)
				{
					_ = doc.frozenFrom.Value;
					if (doc.frozenFrom.Value.Year > 2000)
					{
						value10 = doc.frozenFrom.Value.ToString("yyyyMMdd");
						goto IL_0ce8;
					}
				}
				value10 = "";
				goto IL_0ce8;
			}
			goto IL_0d84;
			IL_0bf9:
			string value11;
			stringBuilder.Append(Function.GetJsonString("ValidTo", value11));
			stringBuilder.Append(Function.GetJsonString("ValidRemarks", Function.ToString(doc.ValidComm)));
			goto IL_0c20;
			IL_0d84:
			if (doc.ItemProperties != null && doc.ItemProperties.Count > 0)
			{
				for (int i = 0; i < doc.ItemProperties.Count; i++)
				{
					BoYesNoEnum boYesNoEnum = ((doc.ItemProperties[i] == "Y") ? BoYesNoEnum.tYES : BoYesNoEnum.tNO);
					stringBuilder.Append(Function.GetJsonString("Properties" + (i + 1), boYesNoEnum.ToString()));
				}
			}
			stringBuilder.Append(Function.GetJsonString("FatherCard", doc.FatherCard));
			if (!string.IsNullOrEmpty(doc.FatherType))
			{
				stringBuilder.Append(Function.GetJsonString("FatherType", doc.FatherType));
			}
			stringBuilder.Append(Function.GetJsonString("DebitorAccount", doc.DebPayAcct));
			stringBuilder.Append(Function.GetJsonString("DownPaymentClearAct", doc.DpmClear));
			stringBuilder.Append(Function.GetJsonString("DownPaymentInterimAccount", doc.DpmIntAct));
			stringBuilder.Append(Function.GetJsonString("LinkedBusinessPartner", doc.ConnBP));
			stringBuilder.Append(Function.GetJsonString("PlanningGroup", doc.PlngGroup));
			if (string.Compare(doc.Affiliate, "y", ignoreCase: true) == 0)
			{
				stringBuilder.Append(Function.GetJsonString("Affiliate", "tYES"));
			}
			else
			{
				stringBuilder.Append(Function.GetJsonString("Affiliate", "tNO"));
			}
			stringBuilder.Append(Function.GetJsonString("VatLiable", doc.VatStatus));
			stringBuilder.Append(Function.GetJsonString("VatGroup", doc.ECVatGroup));
			if (!string.IsNullOrEmpty(doc.HousBnkCry))
			{
				stringBuilder.Append(Function.GetJsonString("HouseBankCountry", doc.HousBnkCry));
				stringBuilder.Append(Function.GetJsonString("HouseBank", doc.HouseBank));
				stringBuilder.Append(Function.GetJsonString("HouseBankAccount", doc.HousBnkAct));
				stringBuilder.Append(Function.GetJsonString("ReferenceDetails", doc.RefDetails));
				if (string.Compare(doc.CollecAuth, "y", ignoreCase: true) == 0)
				{
					stringBuilder.Append(Function.GetJsonString("CollectionAuthorization", "tYES"));
				}
				else
				{
					stringBuilder.Append(Function.GetJsonString("CollectionAuthorization", "tNO"));
				}
				stringBuilder.Append(Function.GetJsonString("BankChargesAllocationCode", Function.ToString(doc.BCACode)));
			}
			if (doc.AtcEntry > 0)
			{
				stringBuilder.Append(Function.GetJsonString("AttachmentEntry", doc.AtcEntry.ToString()));
			}
			else
			{
				stringBuilder.Append(Function.GetJsonString("AttachmentEntry", null));
			}
			stringBuilder.Append(DataService.MappingUdf(doc.ListUserFields));
			stringBuilder.Append("\"BPAccountReceivablePaybleCollection\":[");
			if (doc.AccountRecivablePayables != null && doc.AccountRecivablePayables.Count > 0)
			{
				for (int j = 0; j < doc.AccountRecivablePayables.Count; j++)
				{
					if (string.IsNullOrWhiteSpace(doc.AccountRecivablePayables[j].AcctCode))
					{
						continue;
					}
					stringBuilder.Append("{");
					stringBuilder.Append(Function.GetJsonString("AccountCode", doc.AccountRecivablePayables[j].AcctCode));
					string value12 = "";
					string acctType = doc.AccountRecivablePayables[j].AcctType;
					if (!(acctType == "D"))
					{
						if (acctType == "O")
						{
							value12 = "bpat_OpenDebts";
						}
					}
					else
					{
						value12 = "bpat_DownPayment";
					}
					stringBuilder.Append(Function.GetJsonString("AccountType", value12));
					stringBuilder.Append(Function.GetJsonString("BPCode", Function.ToString(doc.AccountRecivablePayables[j].CardCode)));
					stringBuilder.Append("},");
				}
			}
			stringBuilder.Append("],");
			if (doc.Addresses != null && doc.Addresses.Count > 0)
			{
				stringBuilder.Append("\"BPAddresses\":[");
				for (int k = 0; k < doc.Addresses.Count; k++)
				{
					stringBuilder.Append("{");
					stringBuilder.Append(Function.GetJsonString("BPCode", Function.ToString(doc.Addresses[k].CardCode)));
					if (Function.ParseInt(doc.Addresses[k].LineNum) >= 0)
					{
						stringBuilder.Append(Function.GetJsonString("RowNum", Function.ParseInt(doc.Addresses[k].LineNum)));
					}
					stringBuilder.Append(Function.GetJsonString("AddressName", Function.ToString(doc.Addresses[k].Address)));
					BoAddressType boAddressType = ((Function.ToString(doc.Addresses[k].AdresType) == "B") ? BoAddressType.bo_BillTo : BoAddressType.bo_ShipTo);
					stringBuilder.Append(Function.GetJsonString("AddressType", Function.ToString(boAddressType)));
					stringBuilder.Append(Function.GetJsonString("Street", Function.ToString(doc.Addresses[k].Street)));
					stringBuilder.Append(Function.GetJsonString("StreetNo", Function.ToString(doc.Addresses[k].StreetNo)));
					stringBuilder.Append(Function.GetJsonString("Block", Function.ToString(doc.Addresses[k].Block)));
					stringBuilder.Append(Function.GetJsonString("City", Function.ToString(doc.Addresses[k].City)));
					stringBuilder.Append(Function.GetJsonString("ZipCode", Function.ToString(doc.Addresses[k].ZipCode)));
					stringBuilder.Append(Function.GetJsonString("County", Function.ToString(doc.Addresses[k].County)));
					stringBuilder.Append(Function.GetJsonString("State", Function.ToString(doc.Addresses[k].State)));
					stringBuilder.Append(Function.GetJsonString("Country", Function.ToString(doc.Addresses[k].Country)));
					stringBuilder.Append(Function.GetJsonString("BuildingFloorRoom", Function.ToString(doc.Addresses[k].Building)));
					stringBuilder.Append(Function.GetJsonString("GlobalLocationNumber", Function.ToString(doc.Addresses[k].GlblLocNum)));
					stringBuilder.Append(DataService.MappingUdf(doc.Addresses[k].UserFields));
					stringBuilder.Append("},");
				}
				stringBuilder.Append("],");
			}
			stringBuilder.Append("\"ContactEmployees\":[");
			if (doc.ListContactPerson != null && doc.ListContactPerson.Count > 0)
			{
				for (int l = 0; l < doc.ListContactPerson.Count; l++)
				{
					stringBuilder.Append("{");
					ContactPersonImpl contactPersonImpl = doc.ListContactPerson[l];
					stringBuilder.Append(Function.GetJsonString("InternalCode", Function.ToString(contactPersonImpl.CntctCode)));
					stringBuilder.Append(Function.GetJsonString("Active", (contactPersonImpl.Active == "Y") ? "tYES" : "tNO"));
					if (!string.IsNullOrWhiteSpace(contactPersonImpl.Address))
					{
						stringBuilder.Append(Function.GetJsonString("Address", Function.ToString(contactPersonImpl.Address)));
					}
					if (!string.IsNullOrWhiteSpace(contactPersonImpl.CardCode))
					{
						stringBuilder.Append(Function.GetJsonString("CardCode", Function.ToString(contactPersonImpl.CardCode)));
					}
					if (!string.IsNullOrWhiteSpace(contactPersonImpl.BirthCity))
					{
						stringBuilder.Append(Function.GetJsonString("CityOfBirth", Function.ToString(contactPersonImpl.BirthCity)));
					}
					if (contactPersonImpl.BirthDate.HasValue && contactPersonImpl.BirthDate.Value.Year > 1900)
					{
						stringBuilder.Append(Function.GetJsonString("DateOfBirth", Function.ParseDateTime(contactPersonImpl.BirthDate).Value.ToString("yyyyMMdd")));
					}
					if (!string.IsNullOrWhiteSpace(contactPersonImpl.E_MailL))
					{
						stringBuilder.Append(Function.GetJsonString("E_Mail", Function.ToString(contactPersonImpl.E_MailL)));
					}
					if (!string.IsNullOrWhiteSpace(contactPersonImpl.Fax))
					{
						stringBuilder.Append(Function.GetJsonString("Fax", Function.ToString(contactPersonImpl.Fax)));
					}
					if (!string.IsNullOrWhiteSpace(contactPersonImpl.FirstName))
					{
						stringBuilder.Append(Function.GetJsonString("FirstName", Function.ToString(contactPersonImpl.FirstName)));
					}
					stringBuilder.Append(Function.GetJsonString("Gender", Function.ToString(contactPersonImpl.Gender switch
					{
						"M" => BoGenderTypes.gt_Male, 
						"F" => BoGenderTypes.gt_Female, 
						"E" => BoGenderTypes.gt_Undefined, 
						_ => BoGenderTypes.gt_Undefined, 
					})));
					if (!string.IsNullOrWhiteSpace(contactPersonImpl.Name))
					{
						stringBuilder.Append(Function.GetJsonString("Name", Function.ToString(contactPersonImpl.Name)));
					}
					if (!string.IsNullOrWhiteSpace(contactPersonImpl.LastName))
					{
						stringBuilder.Append(Function.GetJsonString("LastName", Function.ToString(contactPersonImpl.LastName)));
					}
					if (!string.IsNullOrWhiteSpace(contactPersonImpl.Cellolar))
					{
						stringBuilder.Append(Function.GetJsonString("MobilePhone", Function.ToString(contactPersonImpl.Cellolar)));
					}
					if (!string.IsNullOrWhiteSpace(contactPersonImpl.Notes1))
					{
						stringBuilder.Append(Function.GetJsonString("Remarks1", Function.ToString(contactPersonImpl.Notes1)));
					}
					if (!string.IsNullOrWhiteSpace(contactPersonImpl.Notes2))
					{
						stringBuilder.Append(Function.GetJsonString("Remarks2", Function.ToString(contactPersonImpl.Notes2)));
					}
					if (!string.IsNullOrWhiteSpace(contactPersonImpl.Tel1))
					{
						stringBuilder.Append(Function.GetJsonString("Phone1", Function.ToString(contactPersonImpl.Tel1)));
					}
					if (!string.IsNullOrWhiteSpace(contactPersonImpl.Tel2))
					{
						stringBuilder.Append(Function.GetJsonString("Phone2", Function.ToString(contactPersonImpl.Tel2)));
					}
					if (!string.IsNullOrWhiteSpace(contactPersonImpl.BirthPlace))
					{
						stringBuilder.Append(Function.GetJsonString("PlaceOfBirth", Function.ToString(contactPersonImpl.BirthPlace)));
					}
					if (!string.IsNullOrWhiteSpace(contactPersonImpl.Position))
					{
						stringBuilder.Append(Function.GetJsonString("Position", Function.ToString(contactPersonImpl.Position)));
					}
					if (!string.IsNullOrWhiteSpace(contactPersonImpl.Title))
					{
						stringBuilder.Append(Function.GetJsonString("Title", Function.ToString(contactPersonImpl.Title)));
					}
					if (!string.IsNullOrWhiteSpace(contactPersonImpl.Profession))
					{
						stringBuilder.Append(Function.GetJsonString("Profession", Function.ToString(contactPersonImpl.Profession)));
					}
					if (!string.IsNullOrWhiteSpace(contactPersonImpl.Password))
					{
						stringBuilder.Append(Function.GetJsonString("Password", Function.ToString(contactPersonImpl.Password)));
					}
					if (!string.IsNullOrWhiteSpace(contactPersonImpl.Pager))
					{
						stringBuilder.Append(Function.GetJsonString("Pager", Function.ToString(contactPersonImpl.Pager)));
					}
					stringBuilder.Append("},");
				}
			}
			stringBuilder.Append("],");
			stringBuilder.Append("\"BPBankAccounts\":[");
			if (doc.BPBankAccounts != null && doc.BPBankAccounts.Count > 0)
			{
				for (int m = 0; m < doc.BPBankAccounts.Count; m++)
				{
					stringBuilder.Append("{");
					stringBuilder.Append(Function.GetJsonString("AccountName", Function.ToString(doc.BPBankAccounts[m].AcctName)));
					stringBuilder.Append(Function.GetJsonString("AccountNo", Function.ToString(doc.BPBankAccounts[m].Account)));
					stringBuilder.Append(Function.GetJsonString("BankCode", Function.ToString(doc.BPBankAccounts[m].BankCode)));
					stringBuilder.Append(Function.GetJsonString("BICSwiftCode", Function.ToString(doc.BPBankAccounts[m].SwiftNum)));
					stringBuilder.Append(Function.GetJsonString("Block", Function.ToString(doc.BPBankAccounts[m].Block)));
					stringBuilder.Append(Function.GetJsonString("BPCode", Function.ToString(doc.CardCode)));
					stringBuilder.Append(Function.GetJsonString("Branch", Function.ToString(doc.BPBankAccounts[m].Branch)));
					stringBuilder.Append(Function.GetJsonString("City", Function.ToString(doc.BPBankAccounts[m].City)));
					stringBuilder.Append(Function.GetJsonString("ControlKey", Function.ToString(doc.BPBankAccounts[m].ControlKey)));
					stringBuilder.Append(Function.GetJsonString("Country", Function.ToString(doc.BPBankAccounts[m].Country)));
					stringBuilder.Append(Function.GetJsonString("County", Function.ToString(doc.BPBankAccounts[m].County)));
					stringBuilder.Append(Function.GetJsonString("IBAN", Function.ToString(doc.BPBankAccounts[m].IBAN)));
					stringBuilder.Append(Function.GetJsonString("LogInstance", Function.ToString(doc.BPBankAccounts[m].LogInstanc)));
					if (doc.BPBankAccounts[m].MandatDate.HasValue)
					{
						_ = doc.BPBankAccounts[m].MandatDate.Value;
						stringBuilder.Append(Function.GetJsonString("SignatureDate", doc.BPBankAccounts[m].MandatDate.Value.ToString("yyyyMMdd")));
					}
					stringBuilder.Append(Function.GetJsonString("MandateID", Function.ToString(doc.BPBankAccounts[m].MandateID)));
					stringBuilder.Append(Function.GetJsonString("Phone", Function.ToString(doc.BPBankAccounts[m].Phone)));
					if (doc.BPBankAccounts[m].SignDate.HasValue)
					{
						_ = doc.BPBankAccounts[m].SignDate.Value;
						stringBuilder.Append(Function.GetJsonString("SignatureDate", doc.BPBankAccounts[m].SignDate.Value.ToString("yyyyMMdd")));
					}
					stringBuilder.Append(Function.GetJsonString("State", Function.ToString(doc.BPBankAccounts[m].State)));
					stringBuilder.Append(Function.GetJsonString("Street", Function.ToString(doc.BPBankAccounts[m].Street)));
					stringBuilder.Append(Function.GetJsonString("UserNo1", Function.ToString(doc.BPBankAccounts[m].UsrNumber1)));
					stringBuilder.Append(Function.GetJsonString("UserNo2", Function.ToString(doc.BPBankAccounts[m].UsrNumber2)));
					stringBuilder.Append(Function.GetJsonString("UserNo3", Function.ToString(doc.BPBankAccounts[m].UsrNumber3)));
					stringBuilder.Append(Function.GetJsonString("UserNo4", Function.ToString(doc.BPBankAccounts[m].UsrNumber4)));
					stringBuilder.Append(Function.GetJsonString("ZipCode", Function.ToString(doc.BPBankAccounts[m].ZipCode)));
					stringBuilder.Append("},");
				}
			}
			stringBuilder.Append("],");
			stringBuilder.Append("\"BPPaymentDates\":[");
			if (doc.BPPaymentDates != null && doc.BPPaymentDates.Count > 0)
			{
				for (int n = 0; n < doc.BPPaymentDates.Count; n++)
				{
					stringBuilder.Append("{");
					stringBuilder.Append(Function.GetJsonString("PaymentDate", Function.ToString(doc.BPPaymentDates[n].PmntDate)));
					stringBuilder.Append(Function.GetJsonString("BPCode", Function.ToString(doc.BPPaymentDates[n].CardCode)));
					stringBuilder.Append("},");
				}
			}
			stringBuilder.Append("],");
			stringBuilder.Append("\"BPBranchAssignment\":[");
			if (doc.BPBranchAssignment != null && doc.BPBranchAssignment.Count > 0)
			{
				for (int num = 0; num < doc.BPBranchAssignment.Count; num++)
				{
					stringBuilder.Append("{");
					stringBuilder.Append(Function.GetJsonString("BPLID", doc.BPBranchAssignment[num].BPLId));
					stringBuilder.Append("},");
				}
			}
			stringBuilder.Append("],");
			stringBuilder.Append("}");
			goto end_IL_0006;
			IL_0ce8:
			stringBuilder.Append(Function.GetJsonString("FrozenFrom", value10));
			if (doc.frozenTo.HasValue)
			{
				_ = doc.frozenTo.Value;
				if (doc.frozenTo.Value.Year > 2000)
				{
					value8 = doc.frozenTo.Value.ToString("yyyyMMdd");
					goto IL_0d5d;
				}
			}
			value8 = "";
			goto IL_0d5d;
			IL_0b84:
			stringBuilder.Append(Function.GetJsonString("ValidFrom", value7));
			if (doc.validTo.HasValue)
			{
				_ = doc.validTo.Value;
				if (doc.validTo.Value.Year > 2000)
				{
					value11 = doc.validTo.Value.ToString("yyyyMMdd");
					goto IL_0bf9;
				}
			}
			value11 = "";
			goto IL_0bf9;
			end_IL_0006:;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return stringBuilder.ToString();
	}

	public override HttpResult SaveBPProperties(DataTable dt, string sessionId)
	{
		List<BodyRequestObject> list = new List<BodyRequestObject>();
		try
		{
			foreach (DataRow row in dt.Rows)
			{
				string text = "{";
				text += Function.GetJsonString("PropertyCode", row["GroupCode"]);
				text += Function.GetJsonString("PropertyName", row["GroupName"]);
				text += "}";
				list.Add(new BodyRequestObject
				{
					BodyObject = text,
					Action = Enums.HttpAction.Update,
					ObjectType = ApzObjectTypes.oBusinessPartnerProperties,
					Key = Function.ParseInt(row["GroupCode"]),
					OutObjectKey = true
				});
			}
			return DataService.ProcessMutiTransaction(sessionId, list);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override HttpResult SaveBPPriorities(DataTable dt, string sessionId)
	{
		List<BodyRequestObject> list = new List<BodyRequestObject>();
		try
		{
			foreach (DataRow row in dt.Rows)
			{
				if (!(Function.ToString(row["OldPrioCode"]).Trim() == "") || !(Function.ToString(row["PrioCode"]).Trim() == "") || !(Function.ToString(row["PrioDesc"]).Trim() == ""))
				{
					string text = "{";
					if (Function.ToString(row["PrioCode"]) != "")
					{
						text += Function.GetJsonString("Priority", Function.ParseInt(row["PrioCode"]));
					}
					text = text + Function.GetJsonString("PriorityDescription", Function.ToString(row["PrioDesc"])) + "}";
					Enums.HttpAction httpAction = (string.IsNullOrEmpty(Function.ToString(row["OldPrioCode"])) ? Enums.HttpAction.AddNew : (string.IsNullOrEmpty(Function.ToString(row["PrioCode"])) ? Enums.HttpAction.Delete : Enums.HttpAction.Update));
					if (httpAction == Enums.HttpAction.AddNew)
					{
						list.Add(new BodyRequestObject
						{
							Action = Enums.HttpAction.AddNew,
							BodyObject = text,
							ObjectType = ApzObjectTypes.oBusinessPartnerPriorities
						});
					}
					else
					{
						list.Add(new BodyRequestObject
						{
							Action = httpAction,
							BodyObject = text,
							ObjectType = ApzObjectTypes.oBusinessPartnerPriorities,
							Key = Function.ParseInt(row["OldPrioCode"]),
							OutObjectKey = true
						});
					}
				}
			}
			return DataService.ProcessMutiTransaction(sessionId, list);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override HttpResult RemoveBPPriorities(DataTable dt, string sessionId)
	{
		List<BodyRequestObject> list = new List<BodyRequestObject>();
		try
		{
			foreach (DataRow row in dt.Rows)
			{
				list.Add(new BodyRequestObject
				{
					Action = Enums.HttpAction.Delete,
					Key = Function.ParseInt(row["PrioCode"]),
					ObjectType = ApzObjectTypes.oBusinessPartnerPriorities,
					OutObjectKey = true
				});
			}
			return DataService.ProcessMutiTransaction(sessionId, list);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}
}
