using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.DataServices.Implements;
using Apzon.Entities.Models.Common;
using Apzon.Services;

namespace Apzon.DataServices.HanaImplements;

public class HanaCountriesRepository : CountriesRepository
{
	public HanaCountriesRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, unitOfWork, dataService)
	{
	}

	public override bool CheckExists(string code)
	{
		string query = "SELECT * FROM OCRY where \"Code\"=@code;";
		try
		{
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, new SqlParameter("@code", code));
			return (dataTable.IsNotNull() && dataTable.Rows.Count > 0) ? true : false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return false;
		}
	}

	public override string Mapping(DataRow doc, string sessionId)
	{
		string text = "{";
		try
		{
			text += Function.GetJsonString("Code", doc["Code"]);
			text += Function.GetJsonString("Name", doc["Name"]);
			text += Function.GetJsonString("CodeForReports", Function.ToString(doc["ReportCode"]));
			text += Function.GetJsonString("AddressFormat", Function.ToString(doc["AddrFormat"]));
			text += Function.GetJsonString("NumberOfDigitsForTaxID", Function.ParseLong(doc["TaxIdDigts"]));
			text += Function.GetJsonString("BankCodeDigits", Function.ToString(doc["BnkCodDgts"]));
			text += Function.GetJsonString("BankBranchDigits", Function.ToString(doc["BnkBchDgts"]));
			text += Function.GetJsonString("BankAccountDigits", Function.ToString(doc["BnkActDgts"]));
			text += Function.GetJsonString("BankControlKeyDigits", Function.ToString(doc["BnkCtKDgts"]));
			if (Function.ToString(doc["ValDomAcct"]) == "XX")
			{
				text += Function.GetJsonString("DomesticAccountValidation", "dbavNone");
			}
			else if (Function.ToString(doc["ValDomAcct"]) == "BE")
			{
				text += Function.GetJsonString("DomesticAccountValidation", "dbavBelgium");
			}
			else if (Function.ToString(doc["ValDomAcct"]) == "ES")
			{
				text += Function.GetJsonString("DomesticAccountValidation", "dbavSpain");
			}
			else if (Function.ToString(doc["ValDomAcct"]) == "FR")
			{
				text += Function.GetJsonString("DomesticAccountValidation", "dbavFrance");
			}
			else if (Function.ToString(doc["ValDomAcct"]) == "IT")
			{
				text += Function.GetJsonString("DomesticAccountValidation", "dbavFrance");
			}
			else
			{
				text = ((!(Function.ToString(doc["ValDomAcct"]) == "NL")) ? (text + Function.GetJsonString("DomesticAccountValidation", "dbavPortugal")) : (text + Function.GetJsonString("DomesticAccountValidation", "dbavNetherlands")));
			}
			text = ((!(Function.ToString(doc["IsEC"]) == "Y")) ? (text + Function.GetJsonString("EU", "tNO")) : (text + Function.GetJsonString("EU", "tYES")));
			text = ((!(Function.ToString(doc["BnkCodDgts"]) == "Y")) ? (text + Function.GetJsonString("IbanValidation", "tNO")) : (text + Function.GetJsonString("IbanValidation", "tYES")));
			text = ((!(Function.ToString(doc["IsBlackLst"]) == "Y")) ? (text + Function.GetJsonString("Blacklisted", "tNO")) : (text + Function.GetJsonString("Blacklisted", "tYES")));
			text += Function.GetJsonString("UICCountryCode", Function.ToString(doc["UICCode"]));
			text = ((!(Function.ToString(doc["EAEU"]) == "Y")) ? (text + Function.GetJsonString("EAEU", "tNO")) : (text + Function.GetJsonString("EAEU", "tYES")));
			return text + "}";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return "";
		}
	}

	public override HttpResult Save(DataTable dt, string sessionId)
	{
		string text = "";
		List<BodyRequestObject> list = new List<BodyRequestObject>();
		try
		{
			foreach (DataRow row in dt.Rows)
			{
				if (CheckExists(Function.ToString(row["Code"])))
				{
					text = Mapping(row, sessionId);
					list.Add(new BodyRequestObject
					{
						Action = Enums.HttpAction.Update,
						BodyObject = text,
						Key = row["Code"],
						ObjectType = ApzObjectTypes.oCountries,
						OutObjectKey = true
					});
				}
				else
				{
					text = Mapping(row, sessionId);
					list.Add(new BodyRequestObject
					{
						Action = Enums.HttpAction.AddNew,
						BodyObject = text,
						ObjectType = ApzObjectTypes.oCountries
					});
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

	public override HttpResult Remove(string code, string sessionId)
	{
		List<BodyRequestObject> list = new List<BodyRequestObject>();
		try
		{
			list.Add(new BodyRequestObject
			{
				Action = Enums.HttpAction.Delete,
				ObjectType = ApzObjectTypes.oCountries,
				Key = Function.ToString(code),
				OutObjectKey = true
			});
			return DataService.ProcessMutiTransaction(sessionId, list);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}
}
