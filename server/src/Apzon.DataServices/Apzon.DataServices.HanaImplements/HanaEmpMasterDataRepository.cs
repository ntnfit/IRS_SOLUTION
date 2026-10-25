using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.DataServices.Implements;
using Apzon.Entities.Models;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.UserFields;
using Apzon.Services;
using SAPbobsCOM;

namespace Apzon.DataServices.HanaImplements;

public class HanaEmpMasterDataRepository : EmpMasterDataRepository
{
	public HanaEmpMasterDataRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, unitOfWork, dataService)
	{
	}

	protected override HttpResult SaveAttachment(DataTable dt, int absEntry, string sessionId, bool addNew)
	{
		if (addNew || absEntry <= 0)
		{
			return _unitOfWork.Attachment2.Add(dt.ToAttachmentFile(), sessionId);
		}
		return _unitOfWork.Attachment2.Update(absEntry, dt.ToAttachmentFile(), sessionId);
	}

	public override bool Exist(int empID, string sessionId, ApzObjectTypes objType)
	{
		try
		{
			List<string> tableName = Function.GetTableName(objType);
			if (empID <= 0)
			{
				return false;
			}
			if (tableName.Count == 0)
			{
				throw new Exception("Can not check exitsts data");
			}
			return !string.IsNullOrEmpty(ExecuteScarlar($"select 1 from {tableName[0]} where \"empID\" = empID;", CommandType.Text, new SqlParameter("@empID", empID)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return !empID.Equals(0);
		}
	}

	public override HttpResult AddNew(DataSet data, string sessionId, ApzObjectTypes objType, int absEntry)
	{
		try
		{
			List<BodyRequestObject> list = new List<BodyRequestObject>();
			string bodyObject = Mapping(data, sessionId, EnumCommandType.Add, objType, absEntry);
			list.Add(new BodyRequestObject
			{
				ObjectType = objType,
				OutObjectKey = true,
				Action = Enums.HttpAction.AddNew,
				BodyObject = bodyObject
			});
			HttpResult httpResult = DataService.ProcessMutiTransaction(sessionId, list);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				if (absEntry > 0)
				{
					DeleteFile(absEntry);
				}
				Logging.Write("ERROR", httpResult.Message);
				return httpResult;
			}
			string text = UpdateApzTableQuery(Function.ParseInt(data.Tables["dtDoc"].Rows[0]["EmpId"]), data.Tables["dtHealth"]);
			text += UpdateTeamsMemberQuery(Function.ParseInt(data.Tables["dtDoc"].Rows[0]["EmpId"]), data.Tables["dtTeams"]);
			if (!string.IsNullOrEmpty(text))
			{
				ExecuteNonQuery(text, CommandType.Text);
			}
			HttpResult httpResult2 = _servUnitOfWork.EmpContract.SaveData(data.Tables["dtContract"]);
			string message = ((!string.IsNullOrEmpty(httpResult2.Message)) ? ("APZ_OHEC:" + httpResult2.Message) : null);
			int num = Function.ParseInt(httpResult.Content);
			Logging.Write("WATCH", $"Add new document\t{objType}\t{num}");
			return new HttpResult(Enums.MessageCode.Success, message, num);
		}
		catch (Exception ex)
		{
			if (absEntry > 0)
			{
				DeleteFile(absEntry);
			}
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override HttpResult UpdateData(DataSet data, string sessionId, ApzObjectTypes objType, int absEnty)
	{
		try
		{
			List<BodyRequestObject> list = new List<BodyRequestObject>();
			string bodyObject = Mapping(data, sessionId, EnumCommandType.Update, objType, absEnty);
			list.Add(new BodyRequestObject
			{
				ObjectType = objType,
				OutObjectKey = true,
				Key = Function.ParseInt(data.Tables["dtDoc"].Rows[0]["EmpId"]),
				Action = Enums.HttpAction.Update,
				BodyObject = bodyObject
			});
			HttpResult httpResult = DataService.ProcessMutiTransaction(sessionId, list);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				if (data.Tables.Count > 7)
				{
					UpdateFile(data.Tables[7], absEnty);
				}
				Logging.Write("ERROR", httpResult.Message);
				return httpResult;
			}
			string text = UpdateApzTableQuery(Function.ParseInt(data.Tables["dtDoc"].Rows[0]["EmpId"]), data.Tables["dtHealth"]);
			text += UpdateTeamsMemberQuery(Function.ParseInt(data.Tables["dtDoc"].Rows[0]["EmpId"]), data.Tables["dtTeams"]);
			if (!string.IsNullOrEmpty(text))
			{
				ExecuteNonQuery(text, CommandType.Text);
			}
			HttpResult httpResult2 = _servUnitOfWork.EmpContract.SaveData(data.Tables["dtContract"], Enums.TransactionType.U);
			string message = ((!string.IsNullOrEmpty(httpResult2.Message)) ? ("APZ_OHEC:" + httpResult2.Message) : null);
			int num = Function.ParseInt(httpResult.Content);
			Logging.Write("WATCH", string.Format("Update document\t{0}\t{1}", objType, Function.ParseInt(data.Tables["dtDoc"].Rows[0]["EmpId"])));
			return new HttpResult(Enums.MessageCode.Success, message, num);
		}
		catch (Exception ex)
		{
			if (data.Tables.Count > 7)
			{
				UpdateFile(data.Tables[7], absEnty);
			}
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override string Mapping(DataSet data, string sessionId, EnumCommandType commandType, ApzObjectTypes objType, int absEntry)
	{
		if (!data.Tables.Contains("dtRole"))
		{
			DataTable dataTable = new DataTable();
			dataTable.TableName = "dtRole";
			data.Tables.Add(dataTable);
		}
		if (!data.Tables.Contains("dtTeams"))
		{
			DataTable dataTable2 = new DataTable();
			dataTable2.TableName = "dtTeams";
			data.Tables.Add(dataTable2);
		}
		if (!data.Tables.Contains("dtAbsence"))
		{
			DataTable dataTable3 = new DataTable();
			dataTable3.TableName = "dtAbsence";
			data.Tables.Add(dataTable3);
		}
		if (!data.Tables.Contains("dtEducation"))
		{
			DataTable dataTable4 = new DataTable();
			dataTable4.TableName = "dtEducation";
			data.Tables.Add(dataTable4);
		}
		if (!data.Tables.Contains("dtReview"))
		{
			DataTable dataTable5 = new DataTable();
			dataTable5.TableName = "dtReview";
			data.Tables.Add(dataTable5);
		}
		if (!data.Tables.Contains("dtPrevEmp"))
		{
			DataTable dataTable6 = new DataTable();
			dataTable6.TableName = "dtPrevEmp";
			data.Tables.Add(dataTable6);
		}
		DataTable dataTable7 = data.Tables["dtDoc"];
		DataTable dataTable8 = data.Tables["dtAbsence"];
		DataTable dataTable9 = data.Tables["dtEducation"];
		DataTable dataTable10 = data.Tables["dtReview"];
		DataTable dataTable11 = data.Tables["dtPrevEmp"];
		DataTable dataTable12 = data.Tables["dtRole"];
		string text = "{";
		try
		{
			switch (commandType)
			{
			case EnumCommandType.Update:
			{
				if (dataTable7.Columns.Contains("Code") && !string.IsNullOrWhiteSpace(Function.ToString(dataTable7.Rows[0]["Code"])))
				{
					text += Function.GetJsonString("EmployeeCode", Function.ToString(dataTable7.Rows[0]["Code"]));
				}
				text += Function.GetJsonString("LastName", Function.ToString(dataTable7.Rows[0]["lastName"]));
				text += Function.GetJsonString("FirstName", Function.ToString(dataTable7.Rows[0]["firstName"]));
				text += Function.GetJsonString("MiddleName", Function.ToString(dataTable7.Rows[0]["middleName"]));
				text += Function.GetJsonString("Gender", Function.ToString(dataTable7.Rows[0]["sex"]));
				text += Function.GetJsonString("JobTitle", Function.ToString(dataTable7.Rows[0]["jobTitle"]));
				int num25 = Function.ParseInt(dataTable7.Rows[0]["type"]);
				if (num25 != 0)
				{
					text += Function.GetJsonString("EmployeeType", num25);
				}
				int num26 = Function.ParseInt(dataTable7.Rows[0]["dept"]);
				if (num26 != 0)
				{
					text += Function.GetJsonString("Department", num26);
				}
				int num27 = Function.ParseInt(dataTable7.Rows[0]["branch"]);
				if (num27 != 0)
				{
					text += Function.GetJsonString("Branch", num27);
				}
				text += Function.GetJsonString("WorkStreet", Function.ToString(dataTable7.Rows[0]["workStreet"]));
				text += Function.GetJsonString("WorkBlock", Function.ToString(dataTable7.Rows[0]["workBlock"]));
				text += Function.GetJsonString("WorkZipCode", Function.ToString(dataTable7.Rows[0]["workZip"]));
				text += Function.GetJsonString("WorkCity", Function.ToString(dataTable7.Rows[0]["workCity"]));
				text += Function.GetJsonString("WorkCounty", Function.ToString(dataTable7.Rows[0]["workCounty"]));
				text += Function.GetJsonString("WorkCountryCode", Function.ToString(dataTable7.Rows[0]["workCountr"]));
				text += Function.GetJsonString("WorkStateCode", Function.ToString(dataTable7.Rows[0]["workState"]));
				int num28 = Function.ParseInt(dataTable7.Rows[0]["manager"]);
				text = ((num28 == 0) ? (text + Function.GetJsonString("Manager", null)) : (text + Function.GetJsonString("Manager", num28)));
				int num29 = Function.ParseInt(dataTable7.Rows[0]["userId"]);
				text = ((num29 == 0) ? (text + "ApplicationUserID:null,") : (text + Function.GetJsonString("ApplicationUserID", num29)));
				int num30 = Function.ParseInt(dataTable7.Rows[0]["salesPrson"]);
				text = ((num30 == 0) ? (text + Function.GetJsonString("SalesPersonCode", -1)) : (text + Function.GetJsonString("SalesPersonCode", num30)));
				text += Function.GetJsonString("OfficePhone", Function.ToString(dataTable7.Rows[0]["officeTel"]));
				text += Function.GetJsonString("OfficeExtension", Function.ToString(dataTable7.Rows[0]["officeExt"]));
				text += Function.GetJsonString("MobilePhone", Function.ToString(dataTable7.Rows[0]["mobile"]));
				text += Function.GetJsonString("Pager", Function.ToString(dataTable7.Rows[0]["pager"]));
				text += Function.GetJsonString("HomePhone", Function.ToString(dataTable7.Rows[0]["homeTel"]));
				text += Function.GetJsonString("Fax", Function.ToString(dataTable7.Rows[0]["fax"]));
				text += Function.GetJsonString("eMail", Function.ToString(dataTable7.Rows[0]["email"]));
				text = (string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["startDate"])) ? (text + Function.GetJsonString("StartDate", null)) : (text + Function.GetJsonString("StartDate", Function.ParseDateTimes(dataTable7.Rows[0]["startDate"]).ToString("yyyyMMdd"))));
				int num31 = Function.ParseInt(dataTable7.Rows[0]["status"]);
				if (num31 != 0)
				{
					text += Function.GetJsonString("StatusCode", num31);
				}
				text += Function.GetJsonString("Salary", Function.ToString(dataTable7.Rows[0]["salary"]));
				text += Function.GetJsonString("SalaryUnit", Function.ToString(dataTable7.Rows[0]["salaryUnit"]));
				text += Function.GetJsonString("EmployeeCosts", Function.ToString(dataTable7.Rows[0]["emplCost"]));
				text += Function.GetJsonString("EmployeeCostUnit", Function.ToString(dataTable7.Rows[0]["empCostUnt"]));
				text = (string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["termDate"])) ? (text + Function.GetJsonString("TerminationDate", null)) : (text + Function.GetJsonString("TerminationDate", Function.ParseDateTimes(dataTable7.Rows[0]["termDate"]).ToString("yyyyMMdd"))));
				int num32 = Function.ParseInt(dataTable7.Rows[0]["termReason"]);
				if (num32 != 0)
				{
					text += Function.GetJsonString("TreminationReason", num32);
				}
				text += Function.GetJsonString("BankCode", Function.ToString(dataTable7.Rows[0]["bankCode"]));
				text += Function.GetJsonString("BankBranch", Function.ToString(dataTable7.Rows[0]["bankBranch"]));
				text += Function.GetJsonString("BankBranchNum", Function.ToString(dataTable7.Rows[0]["bankBranNo"]));
				text += Function.GetJsonString("BankAccount", Function.ToString(dataTable7.Rows[0]["bankAcount"]));
				text += Function.GetJsonString("HomeStreet", Function.ToString(dataTable7.Rows[0]["homeStreet"]));
				text += Function.GetJsonString("HomeBlock", Function.ToString(dataTable7.Rows[0]["homeBlock"]));
				text += Function.GetJsonString("HomeZipCode", Function.ToString(dataTable7.Rows[0]["homeZip"]));
				text += Function.GetJsonString("HomeCity", Function.ToString(dataTable7.Rows[0]["homeCity"]));
				text += Function.GetJsonString("HomeCounty", Function.ToString(dataTable7.Rows[0]["homeCounty"]));
				text += Function.GetJsonString("HomeCountry", Function.ToString(dataTable7.Rows[0]["homeCountr"]));
				text += Function.GetJsonString("HomeState", Function.ToString(dataTable7.Rows[0]["homeState"]));
				text = (string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["birthDate"])) ? (text + Function.GetJsonString("DateOfBirth", null)) : (text + Function.GetJsonString("DateOfBirth", Function.ParseDateTimes(dataTable7.Rows[0]["birthDate"]).ToString("yyyyMMdd"))));
				text += Function.GetJsonString("CountryOfBirth", Function.ToString(dataTable7.Rows[0]["brthCountr"]));
				text += Function.GetJsonString("MartialStatus", Function.ToString(dataTable7.Rows[0]["martStatus"]));
				int num33 = Function.ParseInt(dataTable7.Rows[0]["nChildren"]);
				text += Function.GetJsonString("NumOfChildren", num33);
				text += Function.GetJsonString("IdNumber", Function.ToString(dataTable7.Rows[0]["govID"]));
				text += Function.GetJsonString("CitizenshipCountryCode", Function.ToString(dataTable7.Rows[0]["citizenshp"]));
				text += Function.GetJsonString("PassportNumber", Function.ToString(dataTable7.Rows[0]["passportNo"]));
				text = (string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["passportEx"])) ? (text + Function.GetJsonString("PassportExpirationDate", null)) : (text + Function.GetJsonString("PassportExpirationDate", Convert.ToDateTime(dataTable7.Rows[0]["passportEx"]).ToString("yyyyMMdd"))));
				text += Function.GetJsonString("Picture", Function.ToString(dataTable7.Rows[0]["picture"]));
				text += Function.GetJsonString("Remarks", Function.ToString(dataTable7.Rows[0]["remark"]));
				text += Function.GetJsonString("SalaryCurrency", Function.ToString(dataTable7.Rows[0]["salaryCurr"]));
				text += Function.GetJsonString("EmployeeCostsCurrency", Function.ToString(dataTable7.Rows[0]["empCostCur"]));
				text += Function.GetJsonString("WorkBuildingFloorRoom", Function.ToString(dataTable7.Rows[0]["WorkBuild"]));
				text += Function.GetJsonString("HomeBuildingFloorRoom", Function.ToString(dataTable7.Rows[0]["HomeBuild"]));
				int num34 = Function.ParseInt(dataTable7.Rows[0]["position"]);
				if (num34 != 0)
				{
					text += Function.GetJsonString("Position", num34);
				}
				text = ((absEntry <= 0) ? (text + Function.GetJsonString("AttachmentEntry", null)) : (text + Function.GetJsonString("AttachmentEntry", absEntry)));
				text += Function.GetJsonString("CostCenterCode", Function.ToString(dataTable7.Rows[0]["CostCenter"]));
				text += Function.GetJsonString("CompanyNumber", Function.ToString(dataTable7.Rows[0]["CompanyNum"]));
				int num35 = Function.ParseInt(dataTable7.Rows[0]["VacPreYear"]);
				text += Function.GetJsonString("VacationPreviousYear", num35);
				int num36 = Function.ParseInt(dataTable7.Rows[0]["VacCurYear"]);
				text += Function.GetJsonString("VacationCurrentYear", num36);
				text += Function.GetJsonString("MunicipalityKey", Function.ToString(dataTable7.Rows[0]["MunKey"]));
				text += Function.GetJsonString("TaxClass", Function.ToString(dataTable7.Rows[0]["TaxClass"]));
				text += Function.GetJsonString("IncomeTaxLiability", Function.ToString(dataTable7.Rows[0]["InTaxLiabi"]));
				text += Function.GetJsonString("Religion", Function.ToString(dataTable7.Rows[0]["EmTaxCCode"]));
				text += Function.GetJsonString("PartnerReligion", Function.ToString(dataTable7.Rows[0]["RelPartner"]));
				text += Function.GetJsonString("ExemptionAmount", Function.ToString(dataTable7.Rows[0]["ExemptAmnt"]));
				text = ((!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["ExemptUnit"]))) ? (text + Function.GetJsonString("ExemptionUnit", Function.ToString(dataTable7.Rows[0]["ExemptUnit"]))) : (text + Function.GetJsonString("ExemptionUnit", 0)));
				text += Function.GetJsonString("ExemptionCurrency", Function.ToString(dataTable7.Rows[0]["ExemptCurr"]));
				text += Function.GetJsonString("AdditionalAmount", Function.ToString(dataTable7.Rows[0]["AddiAmnt"]));
				text = ((!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["AddiUnit"]))) ? (text + Function.GetJsonString("AdditionalUnit", Function.ToString(dataTable7.Rows[0]["AddiUnit"]))) : (text + Function.GetJsonString("AdditionalUnit", 0)));
				text += Function.GetJsonString("AdditionalCurrency", Function.ToString(dataTable7.Rows[0]["AddiCurr"]));
				text += Function.GetJsonString("TaxOfficeName", Function.ToString(dataTable7.Rows[0]["TaxOName"]));
				text += Function.GetJsonString("TaxOfficeNumber", Function.ToString(dataTable7.Rows[0]["TaxONum"]));
				text += Function.GetJsonString("HealthInsuranceName", Function.ToString(dataTable7.Rows[0]["HeaInsName"]));
				text += Function.GetJsonString("HealthInsuranceCode", Function.ToString(dataTable7.Rows[0]["HeaInsCode"]));
				text += Function.GetJsonString("HealthInsuranceType", Function.ToString(dataTable7.Rows[0]["HeaInsType"]));
				text += Function.GetJsonString("SocialInsuranceNumber", Function.ToString(dataTable7.Rows[0]["SInsurNum"]));
				text += Function.GetJsonString("ProfessionStatus", Function.ToString(dataTable7.Rows[0]["StatusOfP"]));
				text += Function.GetJsonString("EducationStatus", Function.ToString(dataTable7.Rows[0]["StatusOfE"]));
				text += Function.GetJsonString("PersonGroup", Function.ToString(dataTable7.Rows[0]["PersGroup"]));
				text += Function.GetJsonString("JobTitleCode", Function.ToString(dataTable7.Rows[0]["JTCode"]));
				text += Function.GetJsonString("BankCodeForDATEV", Function.ToString(dataTable7.Rows[0]["BCodeDateV"]));
				text = ((!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["DevBAOwner"]))) ? (text + Function.GetJsonString("DeviatingBankAccountOwner", Function.ToString(dataTable7.Rows[0]["DevBAOwner"]))) : (text + Function.GetJsonString("DeviatingBankAccountOwner", "N")));
				text += Function.GetJsonString("SpouseFirstName", Function.ToString(dataTable7.Rows[0]["FNameSP"]));
				text += Function.GetJsonString("SpouseSurname", Function.ToString(dataTable7.Rows[0]["SurnameSP"]));
				text += Function.GetJsonString("ExternalEmployeeNumber", Function.ToString(dataTable7.Rows[0]["ExtEmpNo"]));
				text += Function.GetJsonString("BirthPlace", Function.ToString(dataTable7.Rows[0]["BirthPlace"]));
				text = ((!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["PymMeth"]))) ? (text + Function.GetJsonString("PaymentMethod", Function.ToString(dataTable7.Rows[0]["PymMeth"]))) : (text + Function.GetJsonString("PaymentMethod", "05")));
				int num37 = Function.ParseInt(dataTable7.Rows[0]["STDCode"]);
				if (num37 != 0)
				{
					text += Function.GetJsonString("STDCode", num37);
				}
				text = ((!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["ContResp"]))) ? (text + Function.GetJsonString("AccountantResponsible", Function.ToString(dataTable7.Rows[0]["ContResp"]))) : (text + Function.GetJsonString("AccountantResponsible", "N")));
				text = ((!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["RepLegal"]))) ? (text + Function.GetJsonString("LegalRepresentative", Function.ToString(dataTable7.Rows[0]["RepLegal"]))) : (text + Function.GetJsonString("LegalRepresentative", "N")));
				text = ((!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["DirfDeclar"]))) ? (text + Function.GetJsonString("DIRFResponsible", Function.ToString(dataTable7.Rows[0]["DirfDeclar"]))) : (text + Function.GetJsonString("DIRFResponsible", "N")));
				text += Function.GetJsonString("IDType", Function.ToString(dataTable7.Rows[0]["IDType"]));
				int num38 = Function.ParseInt(dataTable7.Rows[0]["BPLId"]);
				if (num38 != 0)
				{
					text += Function.GetJsonString("BPLID", num38);
				}
				text = ((!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["QualCode"]))) ? (text + Function.GetJsonString("QualificationCode", Function.ToString(dataTable7.Rows[0]["QualCode"]))) : (text + Function.GetJsonString("QualificationCode", "000")));
				text = ((!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["PRWebAccss"]))) ? (text + Function.GetJsonString("PRWebAccess", Function.ToString(dataTable7.Rows[0]["PRWebAccss"]))) : (text + Function.GetJsonString("PRWebAccess", "N")));
				text += Function.GetJsonString("WorkStreetNumber", Function.ToString(dataTable7.Rows[0]["StreetNoW"]));
				text += Function.GetJsonString("HomeStreetNumber", Function.ToString(dataTable7.Rows[0]["StreetNoH"]));
				text += Function.GetJsonString("LinkedVendor", Function.ToString(dataTable7.Rows[0]["BPLink"]));
				text += Function.GetJsonString("Active", Function.ToString(dataTable7.Rows[0]["Active"]));
				text = (string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["PassIssue"])) ? (text + Function.GetJsonString("PassportIssueDate", null)) : (text + Function.GetJsonString("PassportIssueDate", Function.ParseDateTimes(dataTable7.Rows[0]["PassIssue"]).ToString("yyyyMMdd"))));
				text += Function.GetJsonString("PassportIssuer", Function.ToString(dataTable7.Rows[0]["PassIssuer"]));
				IEnumerable<string> enumerable2 = from t in (from DataColumn c in dataTable7.Columns
						select c.ColumnName).ToList()
					where t.StartsWith("U_")
					select t;
				if (enumerable2 != null && enumerable2.ToList() != null && enumerable2.ToList().Count > 0)
				{
					List<UserFieldsImpl> list2 = new List<UserFieldsImpl>();
					for (int num39 = 0; num39 < enumerable2.ToList().Count; num39++)
					{
						UserFieldsImpl userFieldsImpl2 = new UserFieldsImpl();
						if (dataTable7.Columns[enumerable2.ToList()[num39]].DataType == typeof(DateTime))
						{
							userFieldsImpl2.Value = Function.ToString(Function.ParseDateTime(dataTable7.Rows[0][enumerable2.ToList()[num39]]));
							userFieldsImpl2.Type = BoFieldTypes.db_Date;
						}
						else if (dataTable7.Columns[enumerable2.ToList()[num39]].DataType == typeof(double))
						{
							userFieldsImpl2.Value = Function.ToString(Function.ParseDouble(dataTable7.Rows[0][enumerable2.ToList()[num39]]));
							userFieldsImpl2.Type = BoFieldTypes.db_Numeric;
						}
						else
						{
							userFieldsImpl2.Value = Function.ToString(Function.ToString(dataTable7.Rows[0][enumerable2.ToList()[num39]]));
							userFieldsImpl2.Type = BoFieldTypes.db_Alpha;
						}
						userFieldsImpl2.Name = enumerable2.ToList()[num39];
						list2.Add(userFieldsImpl2);
					}
					text += DataService.MappingUdf(list2);
				}
				text += "\"EmployeeRolesInfoLines\":[";
				if (dataTable12 != null && dataTable12.Rows.Count > 0)
				{
					for (int num40 = 0; num40 < dataTable12.Rows.Count; num40++)
					{
						text += "{";
						text += Function.GetJsonString("EmployeeID", Function.ToString(dataTable7.Rows[0]["empID"]));
						if (Function.ParseInt(dataTable12.Rows[num40]["line"]) > 0)
						{
							text += Function.GetJsonString("LineNum", Function.ParseInt(dataTable12.Rows[num40]["line"]));
						}
						int num41 = Function.ParseInt(dataTable12.Rows[num40]["roleID"]);
						if (num41 != 0)
						{
							text += Function.GetJsonString("RoleID", num41);
						}
						text += DataService.MappingUdf(dataTable12.Rows[num40]);
						text += "},";
					}
				}
				text += "],";
				text += "\"EmployeeAbsenceInfoLines\":[";
				if (dataTable8 != null && dataTable8.Rows.Count > 0)
				{
					for (int num42 = 0; num42 < dataTable8.Rows.Count; num42++)
					{
						text += "{";
						text += Function.GetJsonString("EmployeeID", Function.ToString(dataTable7.Rows[0]["empID"]));
						if (Function.ParseInt(dataTable8.Rows[num42]["line"]) > 0)
						{
							text += Function.GetJsonString("LineNum", Function.ParseInt(dataTable8.Rows[num42]["line"]));
						}
						text += Function.GetJsonString("FromDate", Function.ParseDateTimes(dataTable8.Rows[num42]["fromDate"]).ToString("yyyyMMdd"));
						text += Function.GetJsonString("ToDate", Function.ParseDateTimes(dataTable8.Rows[num42]["toDate"]).ToString("yyyyMMdd"));
						text += Function.GetJsonString("Reason", Function.ToString(dataTable8.Rows[num42]["reason"]));
						text += Function.GetJsonString("ApprovedBy", Function.ToString(dataTable8.Rows[num42]["approvedBy"]));
						int num43 = Function.ParseInt(dataTable8.Rows[num42]["cnfrmrNum"]);
						if (num43 != 0)
						{
							text += Function.GetJsonString("ConfirmerNumber", num43);
						}
						text += DataService.MappingUdf(dataTable8.Rows[num42]);
						text += "},";
					}
				}
				text += "],";
				text += "\"EmployeeEducationInfoLines\":[";
				if (dataTable9 != null && dataTable9.Rows.Count > 0)
				{
					for (int num44 = 0; num44 < dataTable9.Rows.Count; num44++)
					{
						text += "{";
						text += Function.GetJsonString("EmployeeNo", Function.ParseInt(dataTable7.Rows[0]["empID"]));
						if (Function.ParseInt(dataTable9.Rows[num44]["line"]) > 0)
						{
							text += Function.GetJsonString("LineNum", Function.ParseInt(dataTable9.Rows[num44]["line"]));
						}
						text += Function.GetJsonString("FromDate", Function.ParseDateTimes(dataTable9.Rows[num44]["fromDate"]).ToString("yyyyMMdd"));
						text += Function.GetJsonString("ToDate", Function.ParseDateTimes(dataTable9.Rows[num44]["toDate"]).ToString("yyyyMMdd"));
						text += Function.GetJsonString("Institute", Function.ToString(dataTable9.Rows[num44]["institute"]));
						text += Function.GetJsonString("Major", Function.ToString(dataTable9.Rows[num44]["major"]));
						text += Function.GetJsonString("Diploma", Function.ToString(dataTable9.Rows[num44]["diploma"]));
						int num45 = Function.ParseInt(dataTable9.Rows[num44]["type"]);
						if (num45 != 0)
						{
							text += Function.GetJsonString("EducationType", num45);
						}
						text += DataService.MappingUdf(dataTable9.Rows[num44]);
						text += "},";
					}
				}
				text += "],";
				text += "\"EmployeeReviewsInfoLines\":[";
				if (dataTable10 != null && dataTable10.Rows.Count > 0)
				{
					for (int num46 = 0; num46 < dataTable10.Rows.Count; num46++)
					{
						text += "{";
						text += Function.GetJsonString("EmployeeNo", Function.ParseInt(dataTable7.Rows[0]["empID"]));
						if (Function.ParseInt(dataTable10.Rows[num46]["line"]) > 0)
						{
							text += Function.GetJsonString("LineNum", Function.ParseInt(dataTable10.Rows[num46]["line"]));
						}
						text += Function.GetJsonString("Date", Function.ParseDateTimes(dataTable10.Rows[num46]["date"]).ToString("yyyyMMdd"));
						text += Function.GetJsonString("ReviewDescription", Function.ToString(dataTable10.Rows[num46]["reviewDesc"]));
						text += Function.GetJsonString("Grade", Function.ToString(dataTable10.Rows[num46]["grade"]));
						text += Function.GetJsonString("Remarks", Function.ToString(dataTable10.Rows[num46]["remarks"]));
						int num47 = Function.ParseInt(dataTable10.Rows[num46]["manager"]);
						if (num47 != 0)
						{
							text += Function.GetJsonString("Manager", num47);
						}
						text += DataService.MappingUdf(dataTable10.Rows[num46]);
						text += "},";
					}
				}
				text += "],";
				text += "\"EmployeePreviousEmpoymentInfoLines\":[";
				if (dataTable11 != null && dataTable11.Rows.Count > 0)
				{
					for (int num48 = 0; num48 < dataTable11.Rows.Count; num48++)
					{
						text += "{";
						text += Function.GetJsonString("EmployeeNo", Function.ParseInt(dataTable7.Rows[0]["empID"]));
						text += Function.GetJsonString("LineNum", Function.ParseInt(dataTable11.Rows[num48]["line"]));
						text += Function.GetJsonString("FromDtae", Function.ParseDateTimes(dataTable11.Rows[num48]["fromDate"]).ToString("yyyyMMdd"));
						text += Function.GetJsonString("ToDate", Function.ParseDateTimes(dataTable11.Rows[num48]["toDate"]).ToString("yyyyMMdd"));
						text += Function.GetJsonString("Employer", Function.ToString(dataTable11.Rows[num48]["employer"]));
						text += Function.GetJsonString("Position", Function.ToString(dataTable11.Rows[num48]["position"]));
						text += Function.GetJsonString("Remarks", Function.ToString(dataTable11.Rows[num48]["remarks"]));
						text += DataService.MappingUdf(dataTable11.Rows[num48]);
						text += "},";
					}
				}
				text += "],";
				break;
			}
			case EnumCommandType.Add:
			{
				if (dataTable7.Columns.Contains("Code") && !string.IsNullOrWhiteSpace(Function.ToString(dataTable7.Rows[0]["Code"])))
				{
					text += Function.GetJsonString("EmployeeCode", Function.ToString(dataTable7.Rows[0]["Code"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["lastName"])))
				{
					text += Function.GetJsonString("LastName", Function.ToString(dataTable7.Rows[0]["lastName"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["firstName"])))
				{
					text += Function.GetJsonString("FirstName", Function.ToString(dataTable7.Rows[0]["firstName"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["middleName"])))
				{
					text += Function.GetJsonString("MiddleName", Function.ToString(dataTable7.Rows[0]["middleName"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["sex"])))
				{
					text += Function.GetJsonString("Gender", Function.ToString(dataTable7.Rows[0]["sex"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["jobTitle"])))
				{
					text += Function.GetJsonString("JobTitle", Function.ToString(dataTable7.Rows[0]["jobTitle"]));
				}
				int num = Function.ParseInt(dataTable7.Rows[0]["type"]);
				if (num != 0)
				{
					text += Function.GetJsonString("EmployeeType", num);
				}
				int num2 = Function.ParseInt(dataTable7.Rows[0]["dept"]);
				if (num2 != 0)
				{
					text += Function.GetJsonString("Department", num2);
				}
				int num3 = Function.ParseInt(dataTable7.Rows[0]["branch"]);
				if (num3 != 0)
				{
					text += Function.GetJsonString("Branch", num3);
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["workStreet"])))
				{
					text += Function.GetJsonString("WorkStreet", Function.ToString(dataTable7.Rows[0]["workStreet"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["workBlock"])))
				{
					text += Function.GetJsonString("WorkBlock", Function.ToString(dataTable7.Rows[0]["workBlock"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["workZip"])))
				{
					text += Function.GetJsonString("WorkZipCode", Function.ToString(dataTable7.Rows[0]["workZip"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["workCity"])))
				{
					text += Function.GetJsonString("WorkCity", Function.ToString(dataTable7.Rows[0]["workCity"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["WorkCounty"])))
				{
					text += Function.GetJsonString("WorkCounty", Function.ToString(dataTable7.Rows[0]["WorkCounty"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["workCountr"])))
				{
					text += Function.GetJsonString("WorkCountryCode", Function.ToString(dataTable7.Rows[0]["workCountr"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["workState"])))
				{
					text += Function.GetJsonString("WorkStateCode", Function.ToString(dataTable7.Rows[0]["workState"]));
				}
				int num4 = Function.ParseInt(dataTable7.Rows[0]["manager"]);
				if (num4 != 0)
				{
					text += Function.GetJsonString("Manager", num4);
				}
				int num5 = Function.ParseInt(dataTable7.Rows[0]["userId"]);
				if (num5 != 0)
				{
					text += Function.GetJsonString("ApplicationUserID", num5);
				}
				int num6 = Function.ParseInt(dataTable7.Rows[0]["salesPrson"]);
				text = ((num6 == 0) ? (text + Function.GetJsonString("SalesPersonCode", -1)) : (text + Function.GetJsonString("SalesPersonCode", num6)));
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["officeTel"])))
				{
					text += Function.GetJsonString("OfficePhone", Function.ToString(dataTable7.Rows[0]["officeTel"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["officeExt"])))
				{
					text += Function.GetJsonString("OfficeExtension", Function.ToString(dataTable7.Rows[0]["officeExt"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["mobile"])))
				{
					text += Function.GetJsonString("MobilePhone", Function.ToString(dataTable7.Rows[0]["mobile"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["pager"])))
				{
					text += Function.GetJsonString("Pager", Function.ToString(dataTable7.Rows[0]["pager"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["homeTel"])))
				{
					text += Function.GetJsonString("HomePhone", Function.ToString(dataTable7.Rows[0]["homeTel"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["fax"])))
				{
					text += Function.GetJsonString("Fax", Function.ToString(dataTable7.Rows[0]["fax"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["email"])))
				{
					text += Function.GetJsonString("eMail", Function.ToString(dataTable7.Rows[0]["email"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["startDate"])) && !Function.ParseDateTimes(dataTable7.Rows[0]["startDate"]).ToString("yyyyMMdd").Equals("00010101") && Function.ParseDateTimes(dataTable7.Rows[0]["startDate"]) > new DateTime(1900, 1, 1))
				{
					text += Function.GetJsonString("StartDate", Function.ParseDateTimes(dataTable7.Rows[0]["startDate"]).ToString("yyyyMMdd"));
				}
				int num7 = Function.ParseInt(dataTable7.Rows[0]["status"]);
				if (num7 != 0)
				{
					text += Function.GetJsonString("StatusCode", num7);
				}
				if (Function.ParseDouble(dataTable7.Rows[0]["salary"]) > 0.0)
				{
					text += Function.GetJsonString("Salary", Function.ToString(dataTable7.Rows[0]["salary"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["salaryUnit"])))
				{
					text += Function.GetJsonString("SalaryUnit", Function.ToString(dataTable7.Rows[0]["salaryUnit"]));
				}
				if (Function.ParseDouble(dataTable7.Rows[0]["emplCost"]) > 0.0)
				{
					text += Function.GetJsonString("EmployeeCosts", Function.ToString(dataTable7.Rows[0]["emplCost"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["empCostUnt"])))
				{
					text += Function.GetJsonString("EmployeeCostUnit", Function.ToString(dataTable7.Rows[0]["empCostUnt"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["termDate"])) && !Function.ParseDateTimes(dataTable7.Rows[0]["termDate"]).ToString("yyyyMMdd").Equals("00010101") && Function.ParseDateTimes(dataTable7.Rows[0]["termDate"]) > new DateTime(1900, 1, 1))
				{
					text += Function.GetJsonString("TerminationDate", Function.ParseDateTimes(dataTable7.Rows[0]["termDate"]).ToString("yyyyMMdd"));
				}
				int num8 = Function.ParseInt(dataTable7.Rows[0]["termReason"]);
				if (num8 != 0)
				{
					text += Function.GetJsonString("TreminationReason", num8);
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["bankCode"])))
				{
					text += Function.GetJsonString("BankCode", Function.ToString(dataTable7.Rows[0]["bankCode"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["bankBranch"])))
				{
					text += Function.GetJsonString("BankBranch", Function.ToString(dataTable7.Rows[0]["bankBranch"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["bankBranNo"])))
				{
					text += Function.GetJsonString("BankBranchNum", Function.ToString(dataTable7.Rows[0]["bankBranNo"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["bankAcount"])))
				{
					text += Function.GetJsonString("BankAccount", Function.ToString(dataTable7.Rows[0]["bankAcount"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["homeStreet"])))
				{
					text += Function.GetJsonString("HomeStreet", Function.ToString(dataTable7.Rows[0]["homeStreet"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["homeBlock"])))
				{
					text += Function.GetJsonString("HomeBlock", Function.ToString(dataTable7.Rows[0]["homeBlock"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["homeZip"])))
				{
					text += Function.GetJsonString("HomeZipCode", Function.ToString(dataTable7.Rows[0]["homeZip"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["StreetNoH"])))
				{
					text += Function.GetJsonString("HomeCity", Function.ToString(dataTable7.Rows[0]["homeCity"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["homeCounty"])))
				{
					text += Function.GetJsonString("HomeCounty", Function.ToString(dataTable7.Rows[0]["homeCounty"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["homeCountr"])))
				{
					text += Function.GetJsonString("HomeCountry", Function.ToString(dataTable7.Rows[0]["homeCountr"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["homeState"])))
				{
					text += Function.GetJsonString("HomeState", Function.ToString(dataTable7.Rows[0]["homeState"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["birthDate"])) && !Function.ParseDateTimes(dataTable7.Rows[0]["birthDate"]).ToString("yyyyMMdd").Equals("00010101") && Function.ParseDateTimes(dataTable7.Rows[0]["birthDate"]) > new DateTime(1900, 1, 1))
				{
					text += Function.GetJsonString("DateOfBirth", Function.ParseDateTimes(dataTable7.Rows[0]["birthDate"]).ToString("yyyyMMdd"));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["brthCountr"])))
				{
					text += Function.GetJsonString("CountryOfBirth", Function.ToString(dataTable7.Rows[0]["brthCountr"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["martStatus"])))
				{
					text += Function.GetJsonString("MartialStatus", Function.ToString(dataTable7.Rows[0]["martStatus"]));
				}
				int num9 = Function.ParseInt(dataTable7.Rows[0]["nChildren"]);
				if (num9 >= 0)
				{
					text += Function.GetJsonString("NumOfChildren", num9);
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["govID"])))
				{
					text += Function.GetJsonString("IdNumber", Function.ToString(dataTable7.Rows[0]["govID"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["citizenshp"])))
				{
					text += Function.GetJsonString("CitizenshipCountryCode", Function.ToString(dataTable7.Rows[0]["citizenshp"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["passportNo"])))
				{
					text += Function.GetJsonString("PassportNumber", Function.ToString(dataTable7.Rows[0]["passportNo"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["passportEx"])) && !Function.ParseDateTimes(dataTable7.Rows[0]["passportEx"]).ToString("yyyyMMdd").Equals("00010101") && Function.ParseDateTimes(dataTable7.Rows[0]["passportEx"]) > new DateTime(1900, 1, 1))
				{
					text += Function.GetJsonString("PassportExpirationDate", Convert.ToDateTime(dataTable7.Rows[0]["passportEx"]).ToString("yyyyMMdd"));
				}
				text += Function.GetJsonString("Picture", Function.ToString(dataTable7.Rows[0]["picture"]));
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["remark"])))
				{
					text += Function.GetJsonString("Remarks", Function.ToString(dataTable7.Rows[0]["remark"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["salaryCurr"])))
				{
					text += Function.GetJsonString("SalaryCurrency", Function.ToString(dataTable7.Rows[0]["salaryCurr"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["empCostCur"])))
				{
					text += Function.GetJsonString("EmployeeCostsCurrency", Function.ToString(dataTable7.Rows[0]["empCostCur"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["WorkBuild"])))
				{
					text += Function.GetJsonString("WorkBuildingFloorRoom", Function.ToString(dataTable7.Rows[0]["WorkBuild"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["HomeBuild"])))
				{
					text += Function.GetJsonString("HomeBuildingFloorRoom", Function.ToString(dataTable7.Rows[0]["HomeBuild"]));
				}
				int num10 = Function.ParseInt(dataTable7.Rows[0]["position"]);
				if (num10 != 0)
				{
					text += Function.GetJsonString("Position", num10);
				}
				if (absEntry > 0)
				{
					text += Function.GetJsonString("AttachmentEntry", absEntry);
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["CostCenter"])))
				{
					text += Function.GetJsonString("CostCenterCode", Function.ToString(dataTable7.Rows[0]["CostCenter"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["CompanyNum"])))
				{
					text += Function.GetJsonString("CompanyNumber", Function.ToString(dataTable7.Rows[0]["CompanyNum"]));
				}
				int num11 = Function.ParseInt(dataTable7.Rows[0]["VacPreYear"]);
				if (num11 > 0)
				{
					text += Function.GetJsonString("VacationPreviousYear", num11);
				}
				int num12 = Function.ParseInt(dataTable7.Rows[0]["VacCurYear"]);
				if (num12 > 0)
				{
					text += Function.GetJsonString("VacationCurrentYear", num12);
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["MunKey"])))
				{
					text += Function.GetJsonString("MunicipalityKey", Function.ToString(dataTable7.Rows[0]["MunKey"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["TaxClass"])))
				{
					text += Function.GetJsonString("TaxClass", Function.ToString(dataTable7.Rows[0]["TaxClass"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["InTaxLiabi"])))
				{
					text += Function.GetJsonString("IncomeTaxLiability", Function.ToString(dataTable7.Rows[0]["InTaxLiabi"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["EmTaxCCode"])))
				{
					text += Function.GetJsonString("Religion", Function.ToString(dataTable7.Rows[0]["EmTaxCCode"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["RelPartner"])))
				{
					text += Function.GetJsonString("PartnerReligion", Function.ToString(dataTable7.Rows[0]["RelPartner"]));
				}
				if (Function.ParseDouble(dataTable7.Rows[0]["ExemptAmnt"]) > 0.0)
				{
					text += Function.GetJsonString("ExemptionAmount", Function.ToString(dataTable7.Rows[0]["ExemptAmnt"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["ExemptUnit"])))
				{
					text += Function.GetJsonString("ExemptionUnit", Function.ToString(dataTable7.Rows[0]["ExemptUnit"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["ExemptCurr"])))
				{
					text += Function.GetJsonString("ExemptionCurrency", Function.ToString(dataTable7.Rows[0]["ExemptCurr"]));
				}
				if (Function.ParseDouble(dataTable7.Rows[0]["AddiAmnt"]) > 0.0)
				{
					text += Function.GetJsonString("AdditionalAmount", Function.ToString(dataTable7.Rows[0]["AddiAmnt"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["AddiUnit"])))
				{
					text += Function.GetJsonString("AdditionalUnit", Function.ToString(dataTable7.Rows[0]["AddiUnit"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["AddiCurr"])))
				{
					text += Function.GetJsonString("AdditionalCurrency", Function.ToString(dataTable7.Rows[0]["AddiCurr"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["TaxOName"])))
				{
					text += Function.GetJsonString("TaxOfficeName", Function.ToString(dataTable7.Rows[0]["TaxOName"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["TaxONum"])))
				{
					text += Function.GetJsonString("TaxOfficeNumber", Function.ToString(dataTable7.Rows[0]["TaxONum"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["HeaInsName"])))
				{
					text += Function.GetJsonString("HealthInsuranceName", Function.ToString(dataTable7.Rows[0]["HeaInsName"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["HeaInsCode"])))
				{
					text += Function.GetJsonString("HealthInsuranceCode", Function.ToString(dataTable7.Rows[0]["HeaInsCode"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["HeaInsType"])))
				{
					text += Function.GetJsonString("HealthInsuranceType", Function.ToString(dataTable7.Rows[0]["HeaInsType"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["SInsurNum"])))
				{
					text += Function.GetJsonString("SocialInsuranceNumber", Function.ToString(dataTable7.Rows[0]["SInsurNum"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["StatusOfP"])))
				{
					text += Function.GetJsonString("ProfessionStatus", Function.ToString(dataTable7.Rows[0]["StatusOfP"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["StatusOfE"])))
				{
					text += Function.GetJsonString("EducationStatus", Function.ToString(dataTable7.Rows[0]["StatusOfE"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["PersGroup"])))
				{
					text += Function.GetJsonString("PersonGroup", Function.ToString(dataTable7.Rows[0]["PersGroup"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["JTCode"])))
				{
					text += Function.GetJsonString("JobTitleCode", Function.ToString(dataTable7.Rows[0]["JTCode"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["BCodeDateV"])))
				{
					text += Function.GetJsonString("BankCodeForDATEV", Function.ToString(dataTable7.Rows[0]["BCodeDateV"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["DevBAOwner"])))
				{
					text += Function.GetJsonString("DeviatingBankAccountOwner", Function.ToString(dataTable7.Rows[0]["DevBAOwner"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["FNameSP"])))
				{
					text += Function.GetJsonString("SpouseFirstName", Function.ToString(dataTable7.Rows[0]["FNameSP"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["SurnameSP"])))
				{
					text += Function.GetJsonString("SpouseSurname", Function.ToString(dataTable7.Rows[0]["SurnameSP"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["ExtEmpNo"])))
				{
					text += Function.GetJsonString("ExternalEmployeeNumber", Function.ToString(dataTable7.Rows[0]["ExtEmpNo"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["BirthPlace"])))
				{
					text += Function.GetJsonString("BirthPlace", Function.ToString(dataTable7.Rows[0]["BirthPlace"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["PymMeth"])))
				{
					text += Function.GetJsonString("PaymentMethod", Function.ToString(dataTable7.Rows[0]["PymMeth"]));
				}
				int num13 = Function.ParseInt(dataTable7.Rows[0]["STDCode"]);
				if (num13 != 0)
				{
					text += Function.GetJsonString("STDCode", num13);
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["CPF"])))
				{
					text += Function.GetJsonString("CPF", Function.ToString(dataTable7.Rows[0]["CPF"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["CRC"])))
				{
					text += Function.GetJsonString("CRCNumber", Function.ToString(dataTable7.Rows[0]["CRC"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["ContResp"])))
				{
					text += Function.GetJsonString("AccountantResponsible", Function.ToString(dataTable7.Rows[0]["ContResp"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["RepLegal"])))
				{
					text += Function.GetJsonString("LegalRepresentative", Function.ToString(dataTable7.Rows[0]["RepLegal"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["DirfDeclar"])))
				{
					text += Function.GetJsonString("DIRFResponsible", Function.ToString(dataTable7.Rows[0]["DirfDeclar"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["UF_CRC"])))
				{
					text += Function.GetJsonString("CRCState", Function.ToString(dataTable7.Rows[0]["UF_CRC"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["IDType"])))
				{
					text += Function.GetJsonString("IDType", Function.ToString(dataTable7.Rows[0]["IDType"]));
				}
				int num14 = Function.ParseInt(dataTable7.Rows[0]["BPLId"]);
				if (num14 != 0)
				{
					text += Function.GetJsonString("BPLID", num14);
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["QualCode"])))
				{
					text += Function.GetJsonString("QualificationCode", Function.ToString(dataTable7.Rows[0]["QualCode"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["PRWebAccss"])))
				{
					text += Function.GetJsonString("PRWebAccess", Function.ToString(dataTable7.Rows[0]["PRWebAccss"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["StreetNoW"])))
				{
					text += Function.GetJsonString("WorkStreetNumber", Function.ToString(dataTable7.Rows[0]["StreetNoW"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["StreetNoH"])))
				{
					text += Function.GetJsonString("HomeStreetNumber", Function.ToString(dataTable7.Rows[0]["StreetNoH"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["BPLink"])))
				{
					text += Function.GetJsonString("LinkedVendor", Function.ToString(dataTable7.Rows[0]["BPLink"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["Active"])))
				{
					text += Function.GetJsonString("Active", Function.ToString(dataTable7.Rows[0]["Active"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["PassIssue"])) && Function.ParseDateTimes(dataTable7.Rows[0]["PassIssue"]) > new DateTime(1900, 1, 1))
				{
					text += Function.GetJsonString("PassportIssueDate", Function.ParseDateTimes(dataTable7.Rows[0]["PassIssue"]).ToString("yyyyMMdd"));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["PassIssuer"])))
				{
					text += Function.GetJsonString("PassportIssuer", Function.ToString(dataTable7.Rows[0]["PassIssuer"]));
				}
				IEnumerable<string> enumerable = from t in (from DataColumn c in dataTable7.Columns
						select c.ColumnName).ToList()
					where t.StartsWith("U_")
					select t;
				if (enumerable != null && enumerable.ToList() != null && enumerable.ToList().Count > 0)
				{
					List<UserFieldsImpl> list = new List<UserFieldsImpl>();
					for (int num15 = 0; num15 < enumerable.ToList().Count; num15++)
					{
						UserFieldsImpl userFieldsImpl = new UserFieldsImpl();
						if (dataTable7.Columns[enumerable.ToList()[num15]].DataType == typeof(DateTime))
						{
							userFieldsImpl.Value = Function.ToString(Function.ParseDateTime(dataTable7.Rows[0][enumerable.ToList()[num15]]));
							userFieldsImpl.Type = BoFieldTypes.db_Date;
						}
						else if (dataTable7.Columns[enumerable.ToList()[num15]].DataType == typeof(double))
						{
							userFieldsImpl.Value = Function.ToString(Function.ParseDecimal(dataTable7.Rows[0][enumerable.ToList()[num15]]));
							userFieldsImpl.Type = BoFieldTypes.db_Numeric;
						}
						else
						{
							userFieldsImpl.Value = Function.ToString(Function.ToString(dataTable7.Rows[0][enumerable.ToList()[num15]]));
							userFieldsImpl.Type = BoFieldTypes.db_Alpha;
						}
						userFieldsImpl.Name = enumerable.ToList()[num15];
						list.Add(userFieldsImpl);
					}
					text += DataService.MappingUdf(list);
				}
				if (dataTable12 != null && dataTable12.Rows.Count > 0)
				{
					text += "\"EmployeeRolesInfoLines\":[";
					for (int num16 = 0; num16 < dataTable12.Rows.Count; num16++)
					{
						text += "{";
						text += Function.GetJsonString("EmployeeID", Function.ToString(dataTable7.Rows[0]["empID"]));
						if (Function.ParseInt(dataTable12.Rows[num16]["line"]) > 0)
						{
							text += Function.GetJsonString("LineNum", Function.ParseInt(dataTable12.Rows[num16]["line"]));
						}
						int num17 = Function.ParseInt(dataTable12.Rows[num16]["roleID"]);
						if (num17 != 0)
						{
							text += Function.GetJsonString("RoleID", num17);
						}
						text += DataService.MappingUdf(dataTable12.Rows[num16]);
						text += "},";
					}
					text += "],";
				}
				if (dataTable8 != null && dataTable8.Rows.Count > 0)
				{
					text += "\"EmployeeAbsenceInfoLines\":[";
					for (int num18 = 0; num18 < dataTable8.Rows.Count; num18++)
					{
						text += "{";
						text += Function.GetJsonString("EmployeeID", Function.ToString(dataTable7.Rows[0]["empID"]));
						if (Function.ParseInt(dataTable8.Rows[num18]["line"]) > 0)
						{
							text += Function.GetJsonString("LineNum", Function.ParseInt(dataTable8.Rows[num18]["line"]));
						}
						if (!string.IsNullOrEmpty(Function.ToString(dataTable8.Rows[num18]["fromDate"])) && !Function.ParseDateTimes(dataTable8.Rows[num18]["fromDate"]).ToString("yyyyMMdd").Equals("00010101") && Function.ParseDateTimes(dataTable8.Rows[num18]["fromDate"]) > new DateTime(1900, 1, 1))
						{
							text += Function.GetJsonString("FromDate", Function.ParseDateTimes(dataTable8.Rows[num18]["fromDate"]).ToString("yyyyMMdd"));
						}
						if (!string.IsNullOrEmpty(Function.ToString(dataTable8.Rows[num18]["toDate"])) && !Function.ParseDateTimes(dataTable8.Rows[num18]["toDate"]).ToString("yyyyMMdd").Equals("00010101") && Function.ParseDateTimes(dataTable8.Rows[num18]["toDate"]) > new DateTime(1900, 1, 1))
						{
							text += Function.GetJsonString("ToDate", Function.ParseDateTimes(dataTable8.Rows[num18]["toDate"]).ToString("yyyyMMdd"));
						}
						if (!string.IsNullOrEmpty(Function.ToString(dataTable8.Rows[num18]["reason"])))
						{
							text += Function.GetJsonString("Reason", Function.ToString(dataTable8.Rows[num18]["reason"]));
						}
						if (!string.IsNullOrEmpty(Function.ToString(dataTable8.Rows[num18]["approvedBy"])))
						{
							text += Function.GetJsonString("ApprovedBy", Function.ToString(dataTable8.Rows[num18]["approvedBy"]));
						}
						int num19 = Function.ParseInt(dataTable8.Rows[num18]["cnfrmrNum"]);
						if (num19 != 0)
						{
							text += Function.GetJsonString("ConfirmerNumber", num19);
						}
						text += DataService.MappingUdf(dataTable8.Rows[num18]);
						text += "},";
					}
					text += "],";
				}
				if (dataTable9 != null && dataTable9.Rows.Count > 0)
				{
					text += "\"EmployeeEducationInfoLines\":[";
					for (int num20 = 0; num20 < dataTable9.Rows.Count; num20++)
					{
						text += "{";
						text += Function.GetJsonString("EmployeeNo", Function.ParseInt(dataTable7.Rows[0]["empID"]));
						if (Function.ParseInt(dataTable9.Rows[num20]["line"]) > 0)
						{
							text += Function.GetJsonString("LineNum", Function.ParseInt(dataTable9.Rows[num20]["line"]));
						}
						if (!string.IsNullOrEmpty(Function.ToString(dataTable9.Rows[num20]["fromDate"])) && !Function.ParseDateTimes(dataTable9.Rows[num20]["fromDate"]).ToString("yyyyMMdd").Equals("00010101") && Function.ParseDateTimes(dataTable9.Rows[num20]["fromDate"]) > new DateTime(1900, 1, 1))
						{
							text += Function.GetJsonString("FromDate", Function.ParseDateTimes(dataTable9.Rows[num20]["fromDate"]).ToString("yyyyMMdd"));
						}
						if (!string.IsNullOrEmpty(Function.ToString(dataTable9.Rows[num20]["toDate"])) && !Function.ParseDateTimes(dataTable9.Rows[num20]["toDate"]).ToString("yyyyMMdd").Equals("00010101") && Function.ParseDateTimes(dataTable9.Rows[num20]["toDate"]) > new DateTime(1900, 1, 1))
						{
							text += Function.GetJsonString("ToDate", Function.ParseDateTimes(dataTable9.Rows[num20]["toDate"]).ToString("yyyyMMdd"));
						}
						if (!string.IsNullOrEmpty(Function.ToString(dataTable9.Rows[num20]["institute"])))
						{
							text += Function.GetJsonString("Institute", Function.ToString(dataTable9.Rows[num20]["institute"]));
						}
						if (!string.IsNullOrEmpty(Function.ToString(dataTable9.Rows[num20]["major"])))
						{
							text += Function.GetJsonString("Major", Function.ToString(dataTable9.Rows[num20]["major"]));
						}
						if (!string.IsNullOrEmpty(Function.ToString(dataTable9.Rows[num20]["diploma"])))
						{
							text += Function.GetJsonString("Diploma", Function.ToString(dataTable9.Rows[num20]["diploma"]));
						}
						int num21 = Function.ParseInt(dataTable9.Rows[num20]["type"]);
						if (num21 != 0)
						{
							text += Function.GetJsonString("EducationType", num21);
						}
						text += DataService.MappingUdf(dataTable9.Rows[num20]);
						text += "},";
					}
					text += "],";
				}
				if (dataTable10 != null && dataTable10.Rows.Count > 0)
				{
					text += "\"EmployeeReviewsInfoLines\":[";
					for (int num22 = 0; num22 < dataTable10.Rows.Count; num22++)
					{
						text += "{";
						text += Function.GetJsonString("EmployeeNo", Function.ParseInt(dataTable7.Rows[0]["empID"]));
						if (Function.ParseInt(dataTable10.Rows[num22]["line"]) > 0)
						{
							text += Function.GetJsonString("LineNum", Function.ParseInt(dataTable10.Rows[num22]["line"]));
						}
						if (!string.IsNullOrEmpty(Function.ToString(dataTable10.Rows[num22]["date"])) && !Function.ParseDateTimes(dataTable10.Rows[num22]["date"]).ToString("yyyyMMdd").Equals("00010101") && Function.ParseDateTimes(dataTable10.Rows[num22]["date"]) > new DateTime(1900, 1, 1))
						{
							text += Function.GetJsonString("Date", Function.ParseDateTimes(dataTable10.Rows[num22]["date"]).ToString("yyyyMMdd"));
						}
						if (!string.IsNullOrEmpty(Function.ToString(dataTable10.Rows[num22]["reviewDesc"])))
						{
							text += Function.GetJsonString("ReviewDescription", Function.ToString(dataTable10.Rows[num22]["reviewDesc"]));
						}
						if (!string.IsNullOrEmpty(Function.ToString(dataTable10.Rows[num22]["grade"])))
						{
							text += Function.GetJsonString("Grade", Function.ToString(dataTable10.Rows[num22]["grade"]));
						}
						if (!string.IsNullOrEmpty(Function.ToString(dataTable10.Rows[num22]["remarks"])))
						{
							text += Function.GetJsonString("Remarks", Function.ToString(dataTable10.Rows[num22]["remarks"]));
						}
						int num23 = Function.ParseInt(dataTable10.Rows[num22]["manager"]);
						if (num23 != 0)
						{
							text += Function.GetJsonString("Manager", num23);
						}
						text += DataService.MappingUdf(dataTable10.Rows[num22]);
						text += "},";
					}
					text += "],";
				}
				if (dataTable11 == null || dataTable11.Rows.Count <= 0)
				{
					break;
				}
				text += "\"EmployeePreviousEmpoymentInfoLines\":[";
				for (int num24 = 0; num24 < dataTable11.Rows.Count; num24++)
				{
					text += "{";
					text += Function.GetJsonString("EmployeeNo", Function.ParseInt(dataTable7.Rows[0]["empID"]));
					if (Function.ParseInt(dataTable11.Rows[num24]["line"]) > 0)
					{
						text += Function.GetJsonString("LineNum", Function.ParseInt(dataTable11.Rows[num24]["line"]));
					}
					if (!string.IsNullOrEmpty(Function.ToString(dataTable11.Rows[num24]["fromDate"])) && !Function.ParseDateTimes(dataTable11.Rows[num24]["fromDate"]).ToString("yyyyMMdd").Equals("00010101") && Function.ParseDateTimes(dataTable11.Rows[num24]["fromDate"]) > new DateTime(1900, 1, 1))
					{
						text += Function.GetJsonString("FromDtae", Function.ParseDateTimes(dataTable11.Rows[num24]["fromDate"]).ToString("yyyyMMdd"));
					}
					if (!string.IsNullOrEmpty(Function.ToString(dataTable11.Rows[num24]["toDate"])) && !Function.ParseDateTimes(dataTable11.Rows[num24]["toDate"]).ToString("yyyyMMdd").Equals("00010101") && Function.ParseDateTimes(dataTable11.Rows[num24]["toDate"]) > new DateTime(1900, 1, 1))
					{
						text += Function.GetJsonString("ToDate", Function.ParseDateTimes(dataTable11.Rows[num24]["toDate"]).ToString("yyyyMMdd"));
					}
					if (!string.IsNullOrEmpty(Function.ToString(dataTable11.Rows[num24]["employer"])))
					{
						text += Function.GetJsonString("Employer", Function.ToString(dataTable11.Rows[num24]["employer"]));
					}
					if (!string.IsNullOrEmpty(Function.ToString(dataTable11.Rows[num24]["position"])))
					{
						text += Function.GetJsonString("Position", Function.ToString(dataTable11.Rows[num24]["position"]));
					}
					if (!string.IsNullOrEmpty(Function.ToString(dataTable11.Rows[num24]["remarks"])))
					{
						text += Function.GetJsonString("Remarks", Function.ToString(dataTable11.Rows[num24]["remarks"]));
					}
					text += DataService.MappingUdf(dataTable11.Rows[num24]);
					text += "},";
				}
				text += "],";
				break;
			}
			}
			text += "}";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return text;
	}

	private string MappingTeam(DataRow rTeam)
	{
		string text = "{";
		try
		{
			text += "\"TeamMembers\":[";
			text += "{";
			text += Function.GetJsonString("EmployeeID", Function.ParseInt(rTeam["empID"]));
			text += Function.GetJsonString("TeamID", Function.ToString(rTeam["teamID"]));
			string text2 = Function.ToString(rTeam["role"]);
			BoRoleInTeam boRoleInTeam = ((!(text2 == "M") && text2 == "L") ? BoRoleInTeam.borit_Leader : BoRoleInTeam.borit_Member);
			text += Function.GetJsonString("RoleInTeam", Function.ToString(boRoleInTeam));
			text += "}";
			text += "],";
			text += "}";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return text;
	}

	public override HttpResult AddNewList(List<DataSet> listDs, string sessionId, ApzObjectTypes objType)
	{
		try
		{
			List<BodyRequestObject> list = new List<BodyRequestObject>();
			foreach (DataSet listD in listDs)
			{
				string empty = string.Empty;
				if (!listD.Tables.Contains("dtHealth"))
				{
					DataTable dataTable = new DataTable();
					dataTable.TableName = "dtHealth";
					listD.Tables.Add(dataTable);
				}
				if (!listD.Tables.Contains("dtContract"))
				{
					DataTable dataTable2 = new DataTable();
					dataTable2.TableName = "dtContract";
					listD.Tables.Add(dataTable2);
				}
				empty = Mapping(listD, sessionId, EnumCommandType.Add, objType, Function.ParseInt(listD.Tables["dtDoc"].Rows[0]["AtcEntry"]));
				list.Add(new BodyRequestObject
				{
					ObjectType = objType,
					OutObjectKey = true,
					Action = Enums.HttpAction.AddNew,
					BodyObject = empty
				});
			}
			HttpResult httpResult = DataService.ProcessMutiTransaction(sessionId, list);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				Logging.Write("ERROR", httpResult.Message);
				return httpResult;
			}
			foreach (DataSet listD2 in listDs)
			{
				UpdateApzTable(Function.ParseInt(listD2.Tables["dtDoc"].Rows[0]["EmpId"]), listD2.Tables["dtHealth"], listD2.Tables["dtContract"]);
				UpdateTeamsMember(Function.ParseInt(listD2.Tables["dtDoc"].Rows[0]["EmpId"]), listD2.Tables["dtTeams"]);
			}
			int num = Function.ParseInt(httpResult.Content);
			Logging.Write("WATCH", $"Add new document\t{objType}\t{num}");
			return new HttpResult(Enums.MessageCode.Success, "", num);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override string Remove(int empID, string sessionId)
	{
		try
		{
			List<BodyRequestObject> list = new List<BodyRequestObject>();
			list.Add(new BodyRequestObject
			{
				ObjectType = ApzObjectTypes.oEmployeesInfo,
				OutObjectKey = true,
				Key = empID,
				Action = Enums.HttpAction.Delete
			});
			HttpResult httpResult = DataService.ProcessMutiTransaction(sessionId, list);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				Logging.Write("ERROR", "Delete Employee error: " + httpResult.Message);
				return httpResult.Message;
			}
			Logging.Write("WATCH", "Delete Employee Success: " + Function.ToString(empID));
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}
}
