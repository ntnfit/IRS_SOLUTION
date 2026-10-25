using System;
using System.Collections.Generic;
using System.Data;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.DataServices.Implements;
using Apzon.Entities.Models.Common;
using Apzon.Services;

namespace Apzon.DataServices.HanaImplements;

public class HanaBranchesRepository : BranchesRepository
{
	public HanaBranchesRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, unitOfWork, dataService)
	{
	}

	public override HttpResult SaveData(string sessionId, DataTable doc)
	{
		try
		{
			List<BodyRequestObject> list = new List<BodyRequestObject>();
			foreach (DataRow row in doc.Rows)
			{
				int num = Function.ParseInt(row["BPLId"]);
				Enums.HttpAction action = ((num == 0) ? Enums.HttpAction.AddNew : ((num > 0) ? Enums.HttpAction.Update : Enums.HttpAction.Delete));
				list.Add(new BodyRequestObject
				{
					BodyObject = Mapping(row),
					Action = action,
					ObjectType = ApzObjectTypes.oBusinessPlaces,
					Key = Function.ParseInt(Math.Abs(num)),
					OutObjectKey = true
				});
			}
			return DataService.ProcessMutiTransaction(sessionId, list);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public string Mapping(DataRow row)
	{
		try
		{
			string text = "{";
			int num = Function.ParseInt(row["BPLId"]);
			if (num != 0)
			{
				text += Function.GetJsonString("BPLID", Function.ParseInt(Math.Abs(num)));
			}
			if (num >= 0)
			{
				text += Function.GetJsonString("AdditionalIdNumber", row["AddtnlId"]);
				text += Function.GetJsonString("Address", row["Address"]);
				text += Function.GetJsonString("Addressforeign", row["AddressFr"]);
				text += Function.GetJsonString("AddressType", row["AddrType"]);
				text += Function.GetJsonString("AliasName", row["AliasName"]);
				text += Function.GetJsonString("Block", row["Block"]);
				text += Function.GetJsonString("BPLName", row["BPLName"]);
				text += Function.GetJsonString("BPLNameForeign", row["BPLFrName"]);
				text += Function.GetJsonString("Building", row["Building"]);
				text += Function.GetJsonString("Business", row["Business"]);
				text += Function.GetJsonString("City", row["City"]);
				text += Function.GetJsonString("CompanyQualificationCode", row["CompQualif"]);
				text += Function.GetJsonString("CooperativeAssociationTypeCode", row["CoopAssocT"]);
				text += Function.GetJsonString("Country", row["Country"]);
				text += Function.GetJsonString("County", row["County"]);
				text += Function.GetJsonString("CreditContributionOriginCode", row["CredCOrig"]);
				text += Function.GetJsonString("DateOfIncorporation", row["DateOfInc"]);
				text += Function.GetJsonString("DeclarerTypeCode", row["DeclType"]);
				text += Function.GetJsonString("DefaultCustomerID", row["DflCust"]);
				text += Function.GetJsonString("DefaultResourceWarehouseID", row["DfltResWhs"]);
				text += Function.GetJsonString("DefaultTaxCode", row["DflTaxCode"]);
				text += Function.GetJsonString("DefaultVendorID", row["DflVendor"]);
				text += Function.GetJsonString("DefaultWarehouseID", row["DflWhs"]);
				text += Function.GetJsonString("Disabled", row["Disabled"]);
				text += Function.GetJsonString("EconomicActivityTypeCode", row["EconActT"]);
				text += Function.GetJsonString("EnvironmentType", row["EnvTypeNFe"]);
				text += Function.GetJsonString("FederalTaxID", row["TaxIdNum"]);
				text += Function.GetJsonString("FederalTaxID2", row["TaxIdNum2"]);
				text += Function.GetJsonString("FederalTaxID3", row["TaxIdNum3"]);
				text += Function.GetJsonString("GlobalLocationNumber", row["GlblLocNum"]);
				text += Function.GetJsonString("Industry", row["Industry"]);
				text += Function.GetJsonString("IPIPeriodCode", row["IPIPeriod"]);
				text += Function.GetJsonString("MainBPL", row["MainBPL"]);
				text += Function.GetJsonString("NatureOfCompanyCode", row["CompNature"]);
				text += Function.GetJsonString("Opting4ICMS", row["Opt4ICMS"]);
				text += Function.GetJsonString("PaymentClearingAccount", row["PmtClrAct"]);
				text += Function.GetJsonString("PreferredStateCode", row["PrefState"]);
				text += Function.GetJsonString("ProfitTaxationCode", row["ProfTax"]);
				text += Function.GetJsonString("RepName", row["RepName"]);
				text += Function.GetJsonString("SPEDProfile", row["SPEDProf"]);
				text += Function.GetJsonString("State", row["State"]);
				text += Function.GetJsonString("Street", row["Street"]);
				text += Function.GetJsonString("StreetNo", row["StreetNo"]);
				text += Function.GetJsonString("TaxOffice", row["RevOffice"]);
				text += Function.GetJsonString("TaxOfficeNo", row["TxOffcNo"]);
				text += Function.GetJsonString("VATRegNum", row["VATRegNum"]);
				text += Function.GetJsonString("ZipCode", row["ZipCode"]);
			}
			return text + "}";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return "";
		}
	}
}
