using System;
using System.Data;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.DataServices.Interfaces;
using Apzon.Entities.Models.Common;
using Apzon.Services;

namespace Apzon.DataServices.Implements;

public class BranchesRepository : Repository<DataTable>, IBranches
{
	public BranchesRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, dataService)
	{
	}

	public virtual HttpResult SaveData(string sessionId, DataTable doc)
	{
		try
		{
			string xmlObject = "<?xml version=\"1.0\" ?>  \r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>BusinessPlaces</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>" + Mapping(doc) + "</soap:Body>  \r\n                            </soap:Envelope>";
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
			else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
			{
				XElement xElement3 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("RetKey"));
				if (xElement3 != null)
				{
					int num = Function.ParseInt(xElement3.Value);
					Logging.Write("WATCH", $"Branches - {num}");
					return new HttpResult(Enums.MessageCode.Success, "", num);
				}
			}
			return new HttpResult();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public string Mapping(DataTable doc)
	{
		try
		{
			string text = "";
			foreach (DataRow row in doc.Rows)
			{
				string text2 = "";
				int num = Function.ParseInt(row["BPLId"]);
				if (num != 0)
				{
					string text3 = ((num > 0) ? "Update" : "Add");
					text2 = text2 + "<" + text3 + " xmlsn = \"BusinessPlaces\">\r\n                                 <BusinessPlaces>";
					if (text3 == "Update")
					{
						text2 = text2 + "<BPLId>" + num + "</BPLId>";
					}
					text2 = text2 + "<AdditionalIdNumber>" + Function.RawText(row["AddtnlId"]) + "</AdditionalIdNumber>";
					text2 = text2 + "<Address>" + Function.RawText(row["Address"]) + "</Address>";
					text2 = text2 + "<Addressforeign>" + Function.RawText(row["AddressFr"]) + "</Addressforeign>";
					text2 = text2 + "<AddressType>" + Function.RawText(row["AddrType"]) + "</AddressType>";
					text2 = text2 + "<AliasName>" + Function.RawText(row["AliasName"]) + "</AliasName>";
					text2 = text2 + "<Block>" + Function.RawText(row["Block"]) + "</Block>";
					text2 = text2 + "<BPLName>" + Function.RawText(row["BPLName"]) + "</BPLName>";
					text2 = text2 + "<BPLNameForeign>" + Function.RawText(row["BPLFrName"]) + "</BPLNameForeign>";
					text2 = text2 + "<Building>" + Function.RawText(row["Building"]) + "</Building>";
					text2 = text2 + "<Business>" + Function.RawText(row["Business"]) + "</Business>";
					text2 = text2 + "<City>" + Function.RawText(row["City"]) + "</City>";
					text2 = text2 + "<CompanyQualificationCode>" + Function.RawText(row["CompQualif"]) + "</CompanyQualificationCode>";
					text2 = text2 + "<CooperativeAssociationTypeCode>" + Function.RawText(row["CoopAssocT"]) + "</CooperativeAssociationTypeCode>";
					text2 = text2 + "<Country>" + Function.RawText(row["Country"]) + "</Country>";
					text2 = text2 + "<County>" + Function.RawText(row["County"]) + "</County>";
					text2 = text2 + "<CreditContributionOriginCode>" + Function.RawText(row["CredCOrig"]) + "</CreditContributionOriginCode>";
					text2 = text2 + "<DateOfIncorporation>" + Function.RawText(row["DateOfInc"]) + "</DateOfIncorporation>";
					text2 = text2 + "<DeclarerTypeCode>" + Function.RawText(row["DeclType"]) + "</DeclarerTypeCode>";
					text2 = text2 + "<DefaultCustomerID>" + Function.RawText(row["DflCust"]) + "</DefaultCustomerID>";
					text2 = text2 + "<DefaultResourceWarehouseID>" + Function.RawText(row["DfltResWhs"]) + "</DefaultResourceWarehouseID>";
					text2 = text2 + "<DefaultTaxCode>" + Function.RawText(row["DflTaxCode"]) + "</DefaultTaxCode>";
					text2 = text2 + "<DefaultVendorID>" + Function.RawText(row["DflVendor"]) + "</DefaultVendorID>";
					text2 = text2 + "<DefaultWarehouseID>" + Function.RawText(row["DflWhs"]) + "</DefaultWarehouseID>";
					text2 = text2 + "<Disabled>" + Function.RawText(row["Disabled"]) + "</Disabled>";
					text2 = text2 + "<EconomicActivityTypeCode>" + Function.RawText(row["EconActT"]) + "</EconomicActivityTypeCode>";
					text2 = text2 + "<EnvironmentType>" + Function.RawText(row["EnvTypeNFe"]) + "</EnvironmentType>";
					text2 = text2 + "<FederalTaxID>" + Function.RawText(row["TaxIdNum"]) + "</FederalTaxID>";
					text2 = text2 + "<FederalTaxID2>" + Function.RawText(row["TaxIdNum2"]) + "</FederalTaxID2>";
					text2 = text2 + "<FederalTaxID3>" + Function.RawText(row["TaxIdNum3"]) + "</FederalTaxID3>";
					text2 = text2 + "<GlobalLocationNumber>" + Function.RawText(row["GlblLocNum"]) + "</GlobalLocationNumber>";
					text2 = text2 + "<Industry>" + Function.RawText(row["Industry"]) + "</Industry>";
					text2 = text2 + "<IPIPeriodCode>" + Function.RawText(row["IPIPeriod"]) + "</IPIPeriodCode>";
					text2 = text2 + "<MainBPL>" + Function.RawText(row["MainBPL"]) + "</MainBPL>";
					text2 = text2 + "<NatureOfCompanyCode>" + Function.RawText(row["CompNature"]) + "</NatureOfCompanyCode>";
					text2 = text2 + "<Opting4ICMS>" + Function.RawText(row["Opt4ICMS"]) + "</Opting4ICMS>";
					text2 = text2 + "<PaymentClearingAccount>" + Function.RawText(row["PmtClrAct"]) + "</PaymentClearingAccount>";
					text2 = text2 + "<PreferredStateCode>" + Function.RawText(row["PrefState"]) + "</PreferredStateCode>";
					text2 = text2 + "<ProfitTaxationCode>" + Function.RawText(row["ProfTax"]) + "</ProfitTaxationCode>";
					text2 = text2 + "<RepName>" + Function.RawText(row["RepName"]) + "</RepName>";
					text2 = text2 + "<SPEDProfile>" + Function.RawText(row["SPEDProf"]) + "</SPEDProfile>";
					text2 = text2 + "<State>" + Function.RawText(row["State"]) + "</State>";
					text2 = text2 + "<Street>" + Function.RawText(row["Street"]) + "</Street>";
					text2 = text2 + "<StreetNo>" + Function.RawText(row["StreetNo"]) + "</StreetNo>";
					text2 = text2 + "<TaxOffice>" + Function.RawText(row["TaxOffice"]) + "</TaxOffice>";
					text2 = text2 + "<TaxOfficeNo>" + Function.RawText(row["TxOffcNo"]) + "</TaxOfficeNo>";
					text2 = text2 + "<VATRegNum>" + Function.RawText(row["VATRegNum"]) + "</VATRegNum>";
					text2 = text2 + "<ZipCode>" + Function.RawText(row["ZipCode"]) + "</ZipCode>";
				}
				else
				{
					text2 = text2 + "<Remove xmlsn = \"BusinessPlaces\"><BusinessPlaces><BPLId>" + Function.RawText(Math.Abs(num)) + "</BPLId></BusinessPlaces></Remove>";
				}
				text += text2;
			}
			return text;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return "";
		}
	}
}
