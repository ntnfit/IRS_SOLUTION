using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.DataServices.Interfaces;
using Apzon.Entities.Models;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.UserFields;
using Apzon.Services;
using SAPbobsCOM;

namespace Apzon.DataServices.Implements;

public class EmpMasterDataRepository : Repository<DataTable>, IEmployeeMasterData, IRepository<DataTable>, IDisposable
{
	protected readonly IUnitOfWork _unitOfWork;

	protected readonly Apzon.Services.IUnitOfWork _servUnitOfWork;

	public EmpMasterDataRepository(Apzon.Services.IUnitOfWork serviUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviUnitOfWork, dataService)
	{
		_unitOfWork = unitOfWork;
		_servUnitOfWork = serviUnitOfWork;
	}

	public void Dispose()
	{
		Dispose();
	}

	protected virtual HttpResult SaveAttachment(DataTable dt, int absEntry, string sessionId, bool addNew)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("empID,int", dt.Rows[0]["empID"]);
		return ServiceUnitOfWork.Files.SaveAttachmentFiles(dt.ToAttachmentFile(), dictionary, "OHEM");
	}

	public HttpResult Save(DataSet data, string sessionId)
	{
		try
		{
			if (data != null && data.Tables.Count > 0)
			{
				int num = 0;
				bool flag = !Exist(Function.ParseInt(data.Tables[0].Rows[0]["empID"]), sessionId, ApzObjectTypes.oEmployeesInfo);
				if (data.Tables.Count > 7 && data.Tables[7].Rows.Count > 0)
				{
					HttpResult httpResult = SaveAttachment(data.Tables[7], Function.ParseInt(data.Tables[0].Rows[0]["AtcEntry"]), sessionId, flag);
					if (httpResult.MessageCode != Enums.MessageCode.Success)
					{
						return httpResult;
					}
					num = Function.ParseInt(httpResult.Content);
				}
				if (flag)
				{
					return AddNew(data, sessionId, ApzObjectTypes.oEmployeesInfo, num);
				}
				return UpdateData(data, sessionId, ApzObjectTypes.oEmployeesInfo, num);
			}
			return new HttpResult(Enums.MessageCode.Error, "Loi du lieu!");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public string SaveListHeader(DataTable dt, string sessionId)
	{
		try
		{
			List<DataSet> list = new List<DataSet>();
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				DataSet dataSet = new DataSet();
				DataTable dataTable = dt.Clone();
				dataTable.ImportRow(dt.Rows[i]);
				dataTable.TableName = "dtDoc";
				dataSet.Tables.Add(dataTable);
				list.Add(dataSet);
			}
			return AddNewList(list, sessionId, ApzObjectTypes.oEmployeesInfo).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public virtual HttpResult UpdateData(DataSet data, string sessionId, ApzObjectTypes objType, int absEnty)
	{
		try
		{
			string text = Mapping(data, sessionId, EnumCommandType.Update, objType, absEnty);
			string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?><env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <env:Header xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <SessionID>" + sessionId + "</SessionID>\r\n                        </env:Header><env:Body xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">" + UpdateApzTable(Function.ParseInt(data.Tables["dtDoc"].Rows[0]["EmpId"]), data.Tables["dtHealth"], data.Tables["dtContract"]) + UpdateTeamsMember(Function.ParseInt(data.Tables["dtDoc"].Rows[0]["EmpId"]), data.Tables["dtTeams"]) + "<dis:UpdateObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + text + "</dis:UpdateObject></env:Body></env:Envelopes>";
			XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				if (data.Tables.Count > 7)
				{
					UpdateFile(data.Tables[7], absEnty);
				}
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					return new HttpResult(Enums.MessageCode.Error, xElement2.Value);
				}
			}
			else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
			{
				if (data.Tables.Count > 7)
				{
					UpdateFile(data.Tables[7], absEnty);
				}
				XElement xElement3 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("RetKey"));
				if (xElement3 != null)
				{
					int num = Function.ParseInt(xElement3.Value);
					Logging.Write("WATCH", $"update document - {objType} - DocEntry - {num}");
				}
			}
			return new HttpResult(Enums.MessageCode.Success);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected string UpdateApzTable(int empId, DataTable dtHealth, DataTable dtContract)
	{
		try
		{
			string text = string.Format(" DELETE FROM APZ_OHEH WHERE \"empID\"={0};  DELETE FROM APZ_OHEC WHERE \"empID\"={0}; ", empId);
			for (int i = 0; i < dtHealth.Rows.Count; i++)
			{
				text += string.Format(" INSERT INTO APZ_OHEH (\"healthID\", \"empID\", \"medicalDate\", \"kind\", \"disease\", \"diseaseProfile\", \"note\") VALUES(N'{0}',N'{1}',{2},N'{3}',N'{4}',N'{5}',N'{6}'); ", dtHealth.Rows[i]["healthID"], empId, string.IsNullOrEmpty(Function.ToString(dtHealth.Rows[i]["medicalDate"])) ? "null" : ("'" + Function.ParseDateTimes(dtHealth.Rows[i]["medicalDate"]).ToString("yyyy-MM-dd HH:mm:ss") + "'"), dtHealth.Rows[i]["kind"], dtHealth.Rows[i]["disease"], dtHealth.Rows[i]["diseaseProfile"], dtHealth.Rows[i]["note"]);
			}
			for (int j = 0; j < dtContract.Rows.Count; j++)
			{
				text += string.Format(" INSERT INTO APZ_OHEC (\"contractID\", \"empID\", \"code\", \"type\", \"dept\", \"branch\", \"signer\", \"signDate\", \"startDate\", \"endDate\", \"ProbSalary\", \"basicSalary\", \"salary\", \"salaryCurr\", \"PymMeth\", \"note\", \"BPLId\")  VALUES({0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16}); ", j + 1, FormatValue(empId), FormatValue(dtContract.Rows[j]["code"]), FormatValue(dtContract.Rows[j]["type"]), FormatValue(dtContract.Rows[j]["dept"]), FormatValue(dtContract.Rows[j]["branch"]), FormatValue(dtContract.Rows[j]["signer"]), FormatValue(dtContract.Rows[j]["signDate"]), FormatValue(dtContract.Rows[j]["startDate"]), FormatValue(dtContract.Rows[j]["endDate"]), FormatValue(dtContract.Rows[j]["ProbSalary"], isDouble: true), FormatValue(dtContract.Rows[j]["basicSalary"], isDouble: true), FormatValue(dtContract.Rows[j]["salary"], isDouble: true), FormatValue(dtContract.Rows[j]["salaryCurr"]), FormatValue(dtContract.Rows[j]["PymMeth"]), FormatValue(dtContract.Rows[j]["note"]), FormatValue(dtContract.Rows[j]["BPLId"]));
			}
			return _unitOfWork.UDOs.GetXmlExecuteSQL(text, "", isGetBody: true);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return "";
	}

	protected string UpdateTeamsMember(int empId, DataTable dtTeams)
	{
		try
		{
			string text = $" DELETE FROM HTM1 WHERE \"empID\"={empId};";
			for (int i = 0; i < dtTeams.Rows.Count; i++)
			{
				text += string.Format(" INSERT INTO HTM1 (\"empID\", \"line\", \"teamID\", \"role\")  VALUES({0},{1},{2},{3});", FormatValue(empId), FormatValue(dtTeams.Rows[i]["line"]), FormatValue(dtTeams.Rows[i]["teamID"]), FormatValue(dtTeams.Rows[i]["role"]));
			}
			return _unitOfWork.UDOs.GetXmlExecuteSQL(text, "", isGetBody: true);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return "";
	}

	private string FormatValue(object obj, bool isDouble = false)
	{
		try
		{
			string text = Function.ToString(obj);
			if (string.IsNullOrEmpty(text))
			{
				return "null";
			}
			if (isDouble)
			{
				return "'" + text + "'";
			}
			if (Function.ParseInt(text) != 0)
			{
				return "'" + text + "'";
			}
			if (DateTime.TryParse(text, out var result))
			{
				return "'" + result.ToString("yyyy-MM-dd HH:mm:ss") + "'";
			}
			return "N'" + text + "'";
		}
		catch (Exception)
		{
		}
		return "null";
	}

	public virtual HttpResult AddNewList(List<DataSet> listDs, string sessionId, ApzObjectTypes objType)
	{
		try
		{
			string text = string.Empty;
			foreach (DataSet listD in listDs)
			{
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
				text = text + UpdateApzTable(Function.ParseInt(listD.Tables["dtDoc"].Rows[0]["EmpId"]), listD.Tables["dtHealth"], listD.Tables["dtContract"]) + UpdateTeamsMember(Function.ParseInt(listD.Tables["dtDoc"].Rows[0]["EmpId"]), listD.Tables["dtTeams"]) + " <dis:AddObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + Mapping(listD, sessionId, EnumCommandType.Add, objType, Function.ParseInt(listD.Tables["dtDoc"].Rows[0]["AtcEntry"])) + "</dis:AddObject>";
			}
			string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?><env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <env:Header xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <SessionID>" + sessionId + "</SessionID>\r\n                        </env:Header><env:Body xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\"> " + text + "</env:Body></env:Envelopes>";
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
					Logging.Write("WATCH", $"Add new document - {objType} - DocEntry - {num}");
					return new HttpResult(Enums.MessageCode.Success, "", num);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new HttpResult();
	}

	public virtual HttpResult AddNew(DataSet data, string sessionId, ApzObjectTypes objType, int absEntry)
	{
		try
		{
			string text = Mapping(data, sessionId, EnumCommandType.Add, objType, absEntry);
			string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?><env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <env:Header xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <SessionID>" + sessionId + "</SessionID>\r\n                        </env:Header><env:Body xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\"> " + UpdateApzTable(Function.ParseInt(data.Tables["dtDoc"].Rows[0]["EmpId"]), data.Tables["dtHealth"], data.Tables["dtContract"]) + UpdateTeamsMember(Function.ParseInt(data.Tables["dtDoc"].Rows[0]["EmpId"]), data.Tables["dtTeams"]) + " <dis:AddObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + text + "</dis:AddObject></env:Body></env:Envelopes>";
			XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					if (absEntry > 0)
					{
						DeleteFile(absEntry);
					}
					return new HttpResult(Enums.MessageCode.Error, xElement2.Value);
				}
			}
			else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
			{
				XElement xElement3 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("RetKey"));
				if (xElement3 != null)
				{
					int num = Function.ParseInt(xElement3.Value);
					Logging.Write("WATCH", $"Add new document - {objType} - DocEntry - {num}");
					return new HttpResult(Enums.MessageCode.Success, "", num);
				}
			}
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
		return new HttpResult();
	}

	public virtual string Mapping(DataSet data, string sessionId, EnumCommandType commandType, ApzObjectTypes objType, int absEntry)
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
		string text = "<BOM>\r\n                          <BO xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                          <AdmInfo>\r\n                            <Object>" + objType.ToString() + "</Object>\r\n                          </AdmInfo>";
		try
		{
			switch (commandType)
			{
			case EnumCommandType.Update:
			{
				text = text + "<QueryParams><EmployeeID>" + Function.ParseInt(dataTable7.Rows[0]["empID"]) + "</EmployeeID></QueryParams>";
				text += " <EmployeesInfo>\r\n                            <row>";
				if (dataTable7.Columns.Contains("Code") && !string.IsNullOrWhiteSpace(Function.ToString(dataTable7.Rows[0]["Code"])))
				{
					text = text + "<EmployeeCode>" + Function.RawText(dataTable7.Rows[0]["Code"]) + "</EmployeeCode>";
				}
				text = text + "<LastName>" + Function.RawText(dataTable7.Rows[0]["lastName"]) + "</LastName>";
				text = text + "<FirstName>" + Function.RawText(dataTable7.Rows[0]["firstName"]) + "</FirstName>";
				text = text + "<MiddleName>" + Function.RawText(dataTable7.Rows[0]["middleName"]) + "</MiddleName>";
				text = text + "<Gender>" + Function.RawText(dataTable7.Rows[0]["sex"]) + "</Gender>";
				text = text + "<JobTitle>" + Function.RawText(dataTable7.Rows[0]["jobTitle"]) + "</JobTitle>";
				int num25 = Function.ParseInt(dataTable7.Rows[0]["type"]);
				if (num25 != 0)
				{
					text = text + "<EmployeeType>" + num25 + "</EmployeeType>";
				}
				int num26 = Function.ParseInt(dataTable7.Rows[0]["dept"]);
				if (num26 != 0)
				{
					text = text + "<Department>" + num26 + "</Department>";
				}
				int num27 = Function.ParseInt(dataTable7.Rows[0]["branch"]);
				if (num27 != 0)
				{
					text = text + "<Branch>" + num27 + "</Branch>";
				}
				text = text + "<WorkStreet>" + Function.RawText(dataTable7.Rows[0]["workStreet"]) + "</WorkStreet>";
				text = text + "<WorkBlock>" + Function.RawText(dataTable7.Rows[0]["workBlock"]) + "</WorkBlock>";
				text = text + "<WorkZipCode>" + Function.RawText(dataTable7.Rows[0]["workZip"]) + "</WorkZipCode>";
				text = text + "<WorkCity>" + Function.RawText(dataTable7.Rows[0]["workCity"]) + "</WorkCity>";
				text = text + "<WorkCountryCode>" + Function.RawText(dataTable7.Rows[0]["workCountr"]) + "</WorkCountryCode>";
				text = text + "<WorkStateCode>" + Function.RawText(dataTable7.Rows[0]["workState"]) + "</WorkStateCode>";
				int num28 = Function.ParseInt(dataTable7.Rows[0]["manager"]);
				if (num28 != 0)
				{
					text = text + "<Manager>" + num28 + "</Manager>";
				}
				int num29 = Function.ParseInt(dataTable7.Rows[0]["userId"]);
				text = ((num29 == 0) ? (text + "<ApplicationUserID>000000</ApplicationUserID>") : (text + "<ApplicationUserID>" + num29 + "</ApplicationUserID>"));
				int num30 = Function.ParseInt(dataTable7.Rows[0]["salesPrson"]);
				text = ((num30 == 0) ? (text + "<SalesPersonCode>-1</SalesPersonCode>") : (text + "<SalesPersonCode>" + num30 + "</SalesPersonCode>"));
				text = text + "<OfficePhone>" + Function.RawText(dataTable7.Rows[0]["officeTel"]) + "</OfficePhone>";
				text = text + "<OfficeExtension>" + Function.RawText(dataTable7.Rows[0]["officeExt"]) + "</OfficeExtension>";
				text = text + "<MobilePhone>" + Function.RawText(dataTable7.Rows[0]["mobile"]) + "</MobilePhone>";
				text = text + "<Pager>" + Function.RawText(dataTable7.Rows[0]["pager"]) + "</Pager>";
				text = text + "<HomePhone>" + Function.RawText(dataTable7.Rows[0]["homeTel"]) + "</HomePhone>";
				text = text + "<Fax>" + Function.RawText(dataTable7.Rows[0]["fax"]) + "</Fax>";
				text = text + "<eMail>" + Function.RawText(dataTable7.Rows[0]["email"]) + "</eMail>";
				text = text + "<EmployeeCostUnit>" + Function.RawText(dataTable7.Rows[0]["empCostUnt"]) + "</EmployeeCostUnit>";
				text = (string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["startDate"])) ? (text + "<StartDate></StartDate>") : (text + "<StartDate>" + Function.ParseDateTimes(dataTable7.Rows[0]["startDate"]).ToString("yyyyMMdd") + "</StartDate>"));
				int num31 = Function.ParseInt(dataTable7.Rows[0]["status"]);
				if (num31 != 0)
				{
					text = text + "<StatusCode>" + num31 + "</StatusCode>";
				}
				text = text + "<Salary>" + Function.RawText(dataTable7.Rows[0]["salary"]) + "</Salary>";
				text = text + "<SalaryUnit>" + Function.RawText(dataTable7.Rows[0]["salaryUnit"]) + "</SalaryUnit>";
				text = text + "<EmployeeCosts>" + Function.ToString(dataTable7.Rows[0]["emplCost"]) + "</EmployeeCosts>";
				text = text + "<EmployeeCostUnit>" + Function.RawText(dataTable7.Rows[0]["empCostUnt"]) + "</EmployeeCostUnit>";
				text = (string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["termDate"])) ? (text + "<TerminationDate></TerminationDate>") : (text + "<TerminationDate>" + Function.ParseDateTimes(dataTable7.Rows[0]["termDate"]).ToString("yyyyMMdd") + "</TerminationDate>"));
				int num32 = Function.ParseInt(dataTable7.Rows[0]["termReason"]);
				if (num32 != 0)
				{
					text = text + "<TreminationReason>" + num32 + "</TreminationReason>";
				}
				text = text + "<BankCode>" + Function.RawText(dataTable7.Rows[0]["bankCode"]) + "</BankCode>";
				text = text + "<BankBranch>" + Function.RawText(dataTable7.Rows[0]["bankBranch"]) + "</BankBranch>";
				text = text + "<BankBranchNum>" + Function.RawText(dataTable7.Rows[0]["bankBranNo"]) + "</BankBranchNum>";
				text = text + "<BankAccount>" + Function.RawText(dataTable7.Rows[0]["bankAcount"]) + "</BankAccount>";
				text = text + "<HomeStreet>" + Function.RawText(dataTable7.Rows[0]["homeStreet"]) + "</HomeStreet>";
				text = text + "<HomeBlock>" + Function.RawText(dataTable7.Rows[0]["homeBlock"]) + "</HomeBlock>";
				text = text + "<HomeZipCode>" + Function.RawText(dataTable7.Rows[0]["homeZip"]) + "</HomeZipCode>";
				text = text + "<HomeCity>" + Function.RawText(dataTable7.Rows[0]["homeCity"]) + "</HomeCity>";
				text = text + "<HomeCounty>" + Function.RawText(dataTable7.Rows[0]["homeCounty"]) + "</HomeCounty>";
				text = text + "<HomeCountry>" + Function.RawText(dataTable7.Rows[0]["homeCountr"]) + "</HomeCountry>";
				text = text + "<HomeState>" + Function.RawText(dataTable7.Rows[0]["homeState"]) + "</HomeState>";
				text = (string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["birthDate"])) ? (text + "<DateOfBirth></DateOfBirth>") : (text + "<DateOfBirth>" + Function.ParseDateTimes(dataTable7.Rows[0]["birthDate"]).ToString("yyyyMMdd") + "</DateOfBirth>"));
				text = text + "<CountryOfBirth>" + Function.RawText(dataTable7.Rows[0]["brthCountr"]) + "</CountryOfBirth>";
				text = text + "<MartialStatus>" + Function.RawText(dataTable7.Rows[0]["martStatus"]) + "</MartialStatus>";
				text = text + "<NumOfChildren>" + Function.ParseInt(dataTable7.Rows[0]["nChildren"]) + "</NumOfChildren>";
				text = text + "<IdNumber>" + Function.RawText(dataTable7.Rows[0]["govID"]) + "</IdNumber>";
				text = text + "<CitizenshipCountryCode>" + Function.RawText(dataTable7.Rows[0]["citizenshp"]) + "</CitizenshipCountryCode>";
				text = text + "<PassportNumber>" + Function.RawText(dataTable7.Rows[0]["passportNo"]) + "</PassportNumber>";
				text = (string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["passportEx"])) ? (text + "<PassportExpirationDate></PassportExpirationDate>") : (text + "<PassportExpirationDate>" + Convert.ToDateTime(dataTable7.Rows[0]["passportEx"]).ToString("yyyyMMdd") + "</PassportExpirationDate>"));
				text = text + "<Picture>" + Function.RawText(dataTable7.Rows[0]["picture"]) + "</Picture>";
				text = text + "<Remarks>" + Function.RawText(dataTable7.Rows[0]["remark"]) + "</Remarks>";
				text = text + "<SalaryCurrency>" + Function.RawText(dataTable7.Rows[0]["salaryCurr"]) + "</SalaryCurrency>";
				text = text + "<EmployeeCostsCurrency>" + Function.RawText(dataTable7.Rows[0]["empCostCur"]) + "</EmployeeCostsCurrency>";
				text = text + "<WorkBuildingFloorRoom>" + Function.RawText(dataTable7.Rows[0]["WorkBuild"]) + "</WorkBuildingFloorRoom>";
				text = text + "<HomeBuildingFloorRoom>" + Function.RawText(dataTable7.Rows[0]["HomeBuild"]) + "</HomeBuildingFloorRoom>";
				int num33 = Function.ParseInt(dataTable7.Rows[0]["position"]);
				if (num33 != 0)
				{
					text = text + "<Position>" + num33 + "</Position>";
				}
				text = text + "<CostCenterCode>" + Function.RawText(dataTable7.Rows[0]["CostCenter"]) + "</CostCenterCode>";
				text = text + "<CompanyNumber>" + Function.RawText(dataTable7.Rows[0]["CompanyNum"]) + "</CompanyNumber>";
				text = text + "<VacationPreviousYear>" + Function.ParseInt(dataTable7.Rows[0]["VacPreYear"]) + "</VacationPreviousYear>";
				text = text + "<VacationCurrentYear>" + Function.ParseInt(dataTable7.Rows[0]["VacCurYear"]) + "</VacationCurrentYear>";
				text = text + "<MunicipalityKey>" + Function.RawText(dataTable7.Rows[0]["MunKey"]) + "</MunicipalityKey>";
				text = text + "<TaxClass>" + Function.RawText(dataTable7.Rows[0]["TaxClass"]) + "</TaxClass>";
				text = text + "<IncomeTaxLiability>" + Function.RawText(dataTable7.Rows[0]["InTaxLiabi"]) + "</IncomeTaxLiability>";
				text = text + "<Religion>" + Function.RawText(dataTable7.Rows[0]["EmTaxCCode"]) + "</Religion>";
				text = text + "<PartnerReligion>" + Function.RawText(dataTable7.Rows[0]["RelPartner"]) + "</PartnerReligion>";
				text = text + "<ExemptionAmount>" + Function.ToString(dataTable7.Rows[0]["ExemptAmnt"]) + "</ExemptionAmount>";
				text = ((!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["ExemptUnit"]))) ? (text + "<ExemptionUnit>" + Function.RawText(dataTable7.Rows[0]["ExemptUnit"]) + "</ExemptionUnit>") : (text + "<ExemptionUnit>0</ExemptionUnit>"));
				text = text + "<ExemptionCurrency>" + Function.RawText(dataTable7.Rows[0]["ExemptCurr"]) + "</ExemptionCurrency>";
				text = text + "<AdditionalAmount>" + Function.ToString(dataTable7.Rows[0]["AddiAmnt"]) + "</AdditionalAmount>";
				text = ((!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["AddiUnit"]))) ? (text + "<AdditionalUnit>" + Function.RawText(dataTable7.Rows[0]["AddiUnit"]) + "</AdditionalUnit>") : (text + "<AdditionalUnit>0</AdditionalUnit>"));
				text = text + "<AdditionalCurrency>" + Function.RawText(dataTable7.Rows[0]["AddiCurr"]) + "</AdditionalCurrency>";
				text = text + "<TaxOfficeName>" + Function.RawText(dataTable7.Rows[0]["TaxOName"]) + "</TaxOfficeName>";
				text = text + "<TaxOfficeNumber>" + Function.RawText(dataTable7.Rows[0]["TaxONum"]) + "</TaxOfficeNumber>";
				text = text + "<HealthInsuranceName>" + Function.RawText(dataTable7.Rows[0]["HeaInsName"]) + "</HealthInsuranceName>";
				text = text + "<HealthInsuranceCode>" + Function.RawText(dataTable7.Rows[0]["HeaInsCode"]) + "</HealthInsuranceCode>";
				text = text + "<HealthInsuranceType>" + Function.RawText(dataTable7.Rows[0]["HeaInsType"]) + "</HealthInsuranceType>";
				text = text + "<SocialInsuranceNumber>" + Function.RawText(dataTable7.Rows[0]["SInsurNum"]) + "</SocialInsuranceNumber>";
				text = text + "<ProfessionStatus>" + Function.RawText(dataTable7.Rows[0]["StatusOfP"]) + "</ProfessionStatus>";
				text = text + "<EducationStatus>" + Function.RawText(dataTable7.Rows[0]["StatusOfE"]) + "</EducationStatus>";
				text = text + "<PersonGroup>" + Function.RawText(dataTable7.Rows[0]["PersGroup"]) + "</PersonGroup>";
				text = text + "<JobTitleCode>" + Function.RawText(dataTable7.Rows[0]["JTCode"]) + "</JobTitleCode>";
				text = text + "<BankCodeForDATEV>" + Function.RawText(dataTable7.Rows[0]["BCodeDateV"]) + "</BankCodeForDATEV>";
				text = ((!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["DevBAOwner"]))) ? (text + "<DeviatingBankAccountOwner>" + Function.RawText(dataTable7.Rows[0]["DevBAOwner"]) + "</DeviatingBankAccountOwner>") : (text + "<DeviatingBankAccountOwner>N</DeviatingBankAccountOwner>"));
				text = text + "<SpouseFirstName>" + Function.RawText(dataTable7.Rows[0]["FNameSP"]) + "</SpouseFirstName>";
				text = text + "<SpouseSurname>" + Function.RawText(dataTable7.Rows[0]["SurnameSP"]) + "</SpouseSurname>";
				text = text + "<ExternalEmployeeNumber>" + Function.RawText(dataTable7.Rows[0]["ExtEmpNo"]) + "</ExternalEmployeeNumber>";
				text = text + "<BirthPlace>" + Function.RawText(dataTable7.Rows[0]["BirthPlace"]) + "</BirthPlace>";
				text = ((!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["PymMeth"]))) ? (text + "<PaymentMethod>" + Function.RawText(dataTable7.Rows[0]["PymMeth"]) + "</PaymentMethod>") : (text + "<PaymentMethod>05</PaymentMethod>"));
				int num34 = Function.ParseInt(dataTable7.Rows[0]["STDCode"]);
				if (num34 != 0)
				{
					text = text + "<STDCode>" + num34 + "</STDCode>";
				}
				text = ((!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["ContResp"]))) ? (text + "<AccountantResponsible>" + Function.RawText(dataTable7.Rows[0]["ContResp"]) + "</AccountantResponsible>") : (text + "<AccountantResponsible>N</AccountantResponsible>"));
				text = ((!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["RepLegal"]))) ? (text + "<LegalRepresentative>" + Function.RawText(dataTable7.Rows[0]["RepLegal"]) + "</LegalRepresentative>") : (text + "<LegalRepresentative>N</LegalRepresentative>"));
				text = ((!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["DirfDeclar"]))) ? (text + "<DIRFResponsible>" + Function.RawText(dataTable7.Rows[0]["DirfDeclar"]) + "</DIRFResponsible>") : (text + "<DIRFResponsible>N</DIRFResponsible>"));
				text = text + "<IDType>" + Function.RawText(dataTable7.Rows[0]["IDType"]) + "</IDType>";
				int num35 = Function.ParseInt(dataTable7.Rows[0]["BPLId"]);
				if (num35 != 0)
				{
					text = text + "<BPLID>" + num35 + "</BPLID>";
				}
				text = ((!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["QualCode"]))) ? (text + "<QualificationCode>" + Function.RawText(dataTable7.Rows[0]["QualCode"]) + "</QualificationCode>") : (text + "<QualificationCode>000</QualificationCode>"));
				text = ((!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["PRWebAccss"]))) ? (text + "<PRWebAccess>" + Function.RawText(dataTable7.Rows[0]["PRWebAccss"]) + "</PRWebAccess>") : (text + "<PRWebAccess>N</PRWebAccess>"));
				text = text + "<WorkStreetNumber>" + Function.RawText(dataTable7.Rows[0]["StreetNoW"]) + "</WorkStreetNumber>";
				text = text + "<HomeStreetNumber>" + Function.RawText(dataTable7.Rows[0]["StreetNoH"]) + "</HomeStreetNumber>";
				text = text + "<LinkedVendor>" + Function.RawText(dataTable7.Rows[0]["BPLink"]) + "</LinkedVendor>";
				text = text + "<Active>" + Function.RawText(dataTable7.Rows[0]["Active"]) + "</Active>";
				text = (string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["PassIssue"])) ? (text + "<PassportIssueDate></PassportIssueDate>") : (text + "<PassportIssueDate>" + Function.ParseDateTimes(dataTable7.Rows[0]["PassIssue"]).ToString("yyyyMMdd") + "</PassportIssueDate>"));
				text = text + "<PassportIssuer>" + Function.RawText(dataTable7.Rows[0]["PassIssuer"]) + "</PassportIssuer>";
				IEnumerable<string> enumerable2 = from t in (from DataColumn c in dataTable7.Columns
						select c.ColumnName).ToList()
					where t.StartsWith("U_")
					select t;
				if (enumerable2 != null && enumerable2.ToList() != null && enumerable2.ToList().Count > 0)
				{
					List<UserFieldsImpl> list2 = new List<UserFieldsImpl>();
					for (int num36 = 0; num36 < enumerable2.ToList().Count; num36++)
					{
						UserFieldsImpl userFieldsImpl2 = new UserFieldsImpl();
						if (dataTable7.Columns[enumerable2.ToList()[num36]].DataType == typeof(DateTime))
						{
							userFieldsImpl2.Value = Function.ToString(Function.ParseDateTime(dataTable7.Rows[0][enumerable2.ToList()[num36]]));
							userFieldsImpl2.Type = BoFieldTypes.db_Date;
						}
						else if (dataTable7.Columns[enumerable2.ToList()[num36]].DataType == typeof(double))
						{
							userFieldsImpl2.Value = Function.ToString(Function.ParseDouble(dataTable7.Rows[0][enumerable2.ToList()[num36]]));
							userFieldsImpl2.Type = BoFieldTypes.db_Numeric;
						}
						else
						{
							userFieldsImpl2.Value = Function.ToString(Function.ToString(dataTable7.Rows[0][enumerable2.ToList()[num36]]));
							userFieldsImpl2.Type = BoFieldTypes.db_Alpha;
						}
						userFieldsImpl2.Name = enumerable2.ToList()[num36];
						list2.Add(userFieldsImpl2);
					}
					text += DataService.MappingUdf(list2);
				}
				text += "</row></EmployeesInfo>";
				if (dataTable12 != null && dataTable12.Rows.Count > 0)
				{
					text += "<EmployeeRolesInfo>";
					for (int num37 = 0; num37 < dataTable12.Rows.Count; num37++)
					{
						text += "<row>";
						text = text + "<EmployeeID>" + Function.ParseInt(dataTable7.Rows[0]["empID"]) + "</EmployeeID>";
						if (Function.ParseInt(dataTable12.Rows[num37]["line"]) > 0)
						{
							text = text + "<LineNum>" + Function.ParseInt(dataTable12.Rows[num37]["line"]) + "</LineNum>";
						}
						int num38 = Function.ParseInt(dataTable12.Rows[num37]["roleID"]);
						if (num38 != 0)
						{
							text = text + "<RoleID>" + num38 + "</RoleID>";
						}
						text += DataService.MappingUdf(dataTable12.Rows[num37]);
						text += "</row>";
					}
					text += "</EmployeeRolesInfo>";
				}
				if (dataTable8 != null && dataTable8.Rows.Count > 0)
				{
					text += "<EmployeeAbsenceInfo>";
					for (int num39 = 0; num39 < dataTable8.Rows.Count; num39++)
					{
						text += "<row>";
						text = text + "<EmployeeID>" + Function.ParseInt(dataTable7.Rows[0]["empID"]) + "</EmployeeID>";
						if (Function.ParseInt(dataTable8.Rows[num39]["line"]) > 0)
						{
							text = text + "<LineNum>" + Function.ParseInt(dataTable8.Rows[num39]["line"]) + "</LineNum>";
						}
						text = text + "<FromDate>" + Function.ParseDateTimes(dataTable8.Rows[num39]["fromDate"]).ToString("yyyyMMdd") + "</FromDate>";
						text = text + "<ToDate>" + Function.ParseDateTimes(dataTable8.Rows[num39]["toDate"]).ToString("yyyyMMdd") + "</ToDate>";
						text = text + "<Reason>" + Function.RawText(dataTable8.Rows[num39]["reason"]) + "</Reason>";
						text = text + "<ApprovedBy>" + Function.RawText(dataTable8.Rows[num39]["approvedBy"]) + "</ApprovedBy>";
						int num40 = Function.ParseInt(dataTable8.Rows[num39]["cnfrmrNum"]);
						if (num40 != 0)
						{
							text = text + "<ConfirmerNumber>" + num40 + "</ConfirmerNumber>";
						}
						text += DataService.MappingUdf(dataTable8.Rows[num39]);
						text += "</row>";
					}
					text += "</EmployeeAbsenceInfo>";
				}
				if (dataTable9 != null && dataTable9.Rows.Count > 0)
				{
					text += "<EmployeeEducationInfo>";
					for (int num41 = 0; num41 < dataTable9.Rows.Count; num41++)
					{
						text += "<row>";
						text = text + "<EmployeeNo>" + Function.ParseInt(dataTable7.Rows[0]["empID"]) + "</EmployeeNo>";
						if (Function.ParseInt(dataTable9.Rows[num41]["line"]) > 0)
						{
							text = text + "<LineNum>" + Function.ParseInt(dataTable9.Rows[num41]["line"]) + "</LineNum>";
						}
						text = text + "<FromDate>" + Function.ParseDateTimes(dataTable9.Rows[num41]["fromDate"]).ToString("yyyyMMdd") + "</FromDate>";
						text = text + "<ToDate>" + Function.ParseDateTimes(dataTable9.Rows[num41]["toDate"]).ToString("yyyyMMdd") + "</ToDate>";
						text = text + "<Institute>" + Function.RawText(dataTable9.Rows[num41]["institute"]) + "</Institute>";
						text = text + "<Major>" + Function.RawText(dataTable9.Rows[num41]["major"]) + "</Major>";
						text = text + "<Diploma>" + Function.RawText(dataTable9.Rows[num41]["diploma"]) + "</Diploma>";
						int num42 = Function.ParseInt(dataTable9.Rows[num41]["type"]);
						if (num42 != 0)
						{
							text = text + "<EducationType>" + num42 + "</EducationType>";
						}
						text += DataService.MappingUdf(dataTable9.Rows[num41]);
						text += "</row>";
					}
					text += "</EmployeeEducationInfo>";
				}
				if (dataTable10 != null && dataTable10.Rows.Count > 0)
				{
					text += "<EmployeeReviewsInfo>";
					for (int num43 = 0; num43 < dataTable10.Rows.Count; num43++)
					{
						text += "<row>";
						text = text + "<EmployeeNo>" + Function.ParseInt(dataTable7.Rows[0]["empID"]) + "</EmployeeNo>";
						if (Function.ParseInt(dataTable10.Rows[num43]["line"]) > 0)
						{
							text = text + "<LineNum>" + Function.ParseInt(dataTable10.Rows[num43]["line"]) + "</LineNum>";
						}
						text = text + "<Date>" + Function.ParseDateTimes(dataTable10.Rows[num43]["date"]).ToString("yyyyMMdd") + "</Date>";
						text = text + "<ReviewDescription>" + Function.RawText(dataTable10.Rows[num43]["reviewDesc"]) + "</ReviewDescription>";
						text = text + "<Grade>" + Function.RawText(dataTable10.Rows[num43]["grade"]) + "</Grade>";
						text = text + "<Remarks>" + Function.RawText(dataTable10.Rows[num43]["remarks"]) + "</Remarks>";
						int num44 = Function.ParseInt(dataTable10.Rows[num43]["manager"]);
						if (num44 != 0)
						{
							text = text + "<Manager>" + num44 + "</Manager>";
						}
						text += DataService.MappingUdf(dataTable10.Rows[num43]);
						text += "</row>";
					}
					text += "</EmployeeReviewsInfo>";
				}
				if (dataTable11 != null && dataTable11.Rows.Count > 0)
				{
					text += "<EmployeePrevEmpoymentInfo>";
					for (int num45 = 0; num45 < dataTable11.Rows.Count; num45++)
					{
						text += "<row>";
						text = text + "<EmployeeNo>" + Function.ParseInt(dataTable7.Rows[0]["empID"]) + "</EmployeeNo>";
						text = text + "<LineNum>" + Function.ParseInt(dataTable11.Rows[num45]["line"]) + "</LineNum>";
						text = text + "<FromDtae>" + Function.ParseDateTimes(dataTable11.Rows[num45]["fromDate"]).ToString("yyyyMMdd") + "</FromDtae>";
						text = text + "<ToDate>" + Function.ParseDateTimes(dataTable11.Rows[num45]["toDate"]).ToString("yyyyMMdd") + "</ToDate>";
						text = text + "<Employer>" + Function.RawText(dataTable11.Rows[num45]["employer"]) + "</Employer>";
						text = text + "<Position>" + Function.RawText(dataTable11.Rows[num45]["position"]) + "</Position>";
						text = text + "<Remarks>" + Function.RawText(dataTable11.Rows[num45]["remarks"]) + "</Remarks>";
						text += DataService.MappingUdf(dataTable11.Rows[num45]);
						text += "</row>";
					}
					text += "</EmployeePrevEmpoymentInfo>";
				}
				break;
			}
			case EnumCommandType.Add:
			{
				text += " <EmployeesInfo>\r\n                            <row>";
				if (dataTable7.Columns.Contains("Code") && !string.IsNullOrWhiteSpace(Function.ToString(dataTable7.Rows[0]["Code"])))
				{
					text = text + "<EmployeeCode>" + Function.RawText(dataTable7.Rows[0]["Code"]) + "</EmployeeCode>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["lastName"])))
				{
					text = text + "<LastName>" + Function.RawText(dataTable7.Rows[0]["lastName"]) + "</LastName>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["firstName"])))
				{
					text = text + "<FirstName>" + Function.RawText(dataTable7.Rows[0]["firstName"]) + "</FirstName>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["middleName"])))
				{
					text = text + "<MiddleName>" + Function.RawText(dataTable7.Rows[0]["middleName"]) + "</MiddleName>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["sex"])))
				{
					text = text + "<Gender>" + Function.RawText(dataTable7.Rows[0]["sex"]) + "</Gender>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["jobTitle"])))
				{
					text = text + "<JobTitle>" + Function.RawText(dataTable7.Rows[0]["jobTitle"]) + "</JobTitle>";
				}
				int num = Function.ParseInt(dataTable7.Rows[0]["type"]);
				if (num != 0)
				{
					text = text + "<EmployeeType>" + num + "</EmployeeType>";
				}
				int num2 = Function.ParseInt(dataTable7.Rows[0]["dept"]);
				if (num2 != 0)
				{
					text = text + "<Department>" + num2 + "</Department>";
				}
				int num3 = Function.ParseInt(dataTable7.Rows[0]["branch"]);
				if (num3 != 0)
				{
					text = text + "<Branch>" + num3 + "</Branch>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["workStreet"])))
				{
					text = text + "<WorkStreet>" + Function.RawText(dataTable7.Rows[0]["workStreet"]) + "</WorkStreet>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["workBlock"])))
				{
					text = text + "<WorkBlock>" + Function.RawText(dataTable7.Rows[0]["workBlock"]) + "</WorkBlock>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["workZip"])))
				{
					text = text + "<WorkZipCode>" + Function.RawText(dataTable7.Rows[0]["workZip"]) + "</WorkZipCode>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["workCity"])))
				{
					text = text + "<WorkCity>" + Function.RawText(dataTable7.Rows[0]["workCity"]) + "</WorkCity>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["workCountr"])))
				{
					text = text + "<WorkCountryCode>" + Function.RawText(dataTable7.Rows[0]["workCountr"]) + "</WorkCountryCode>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["workState"])))
				{
					text = text + "<WorkStateCode>" + Function.RawText(dataTable7.Rows[0]["workState"]) + "</WorkStateCode>";
				}
				int num4 = Function.ParseInt(dataTable7.Rows[0]["manager"]);
				if (num4 != 0)
				{
					text = text + "<Manager>" + num4 + "</Manager>";
				}
				int num5 = Function.ParseInt(dataTable7.Rows[0]["userId"]);
				if (num5 != 0)
				{
					text = text + "<ApplicationUserID>" + num5 + "</ApplicationUserID>";
				}
				int num6 = Function.ParseInt(dataTable7.Rows[0]["salesPrson"]);
				text = ((num6 == 0) ? (text + "<SalesPersonCode>-1</SalesPersonCode>") : (text + "<SalesPersonCode>" + num6 + "</SalesPersonCode>"));
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["officeTel"])))
				{
					text = text + "<OfficePhone>" + Function.RawText(dataTable7.Rows[0]["officeTel"]) + "</OfficePhone>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["officeExt"])))
				{
					text = text + "<OfficeExtension>" + Function.RawText(dataTable7.Rows[0]["officeExt"]) + "</OfficeExtension>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["mobile"])))
				{
					text = text + "<MobilePhone>" + Function.RawText(dataTable7.Rows[0]["mobile"]) + "</MobilePhone>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["pager"])))
				{
					text = text + "<Pager>" + Function.RawText(dataTable7.Rows[0]["pager"]) + "</Pager>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["homeTel"])))
				{
					text = text + "<HomePhone>" + Function.RawText(dataTable7.Rows[0]["homeTel"]) + "</HomePhone>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["fax"])))
				{
					text = text + "<Fax>" + Function.RawText(dataTable7.Rows[0]["fax"]) + "</Fax>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["email"])))
				{
					text = text + "<eMail>" + Function.RawText(dataTable7.Rows[0]["email"]) + "</eMail>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["startDate"])) && !Function.ParseDateTimes(dataTable7.Rows[0]["startDate"]).ToString("yyyyMMdd").Equals("00010101") && Function.ParseDateTimes(dataTable7.Rows[0]["startDate"]) > new DateTime(1900, 1, 1))
				{
					text = text + "<StartDate>" + Function.ParseDateTimes(dataTable7.Rows[0]["startDate"]).ToString("yyyyMMdd") + "</StartDate>";
				}
				int num7 = Function.ParseInt(dataTable7.Rows[0]["status"]);
				if (num7 != 0)
				{
					text = text + "<StatusCode>" + num7 + "</StatusCode>";
				}
				if (Function.ParseDouble(dataTable7.Rows[0]["salary"]) > 0.0)
				{
					text = text + "<Salary>" + Function.ToString(dataTable7.Rows[0]["salary"]) + "</Salary>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["salaryUnit"])))
				{
					text = text + "<SalaryUnit>" + Function.RawText(dataTable7.Rows[0]["salaryUnit"]) + "</SalaryUnit>";
				}
				if (Function.ParseDouble(dataTable7.Rows[0]["emplCost"]) > 0.0)
				{
					text = text + "<EmployeeCosts>" + Function.ToString(dataTable7.Rows[0]["emplCost"]) + "</EmployeeCosts>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["empCostUnt"])))
				{
					text = text + "<EmployeeCostUnit>" + Function.RawText(dataTable7.Rows[0]["empCostUnt"]) + "</EmployeeCostUnit>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["termDate"])) && !Function.ParseDateTimes(dataTable7.Rows[0]["termDate"]).ToString("yyyyMMdd").Equals("00010101") && Function.ParseDateTimes(dataTable7.Rows[0]["termDate"]) > new DateTime(1900, 1, 1))
				{
					text = text + "<TerminationDate>" + Function.ParseDateTimes(dataTable7.Rows[0]["termDate"]).ToString("yyyyMMdd") + "</TerminationDate>";
				}
				int num8 = Function.ParseInt(dataTable7.Rows[0]["termReason"]);
				if (num8 != 0)
				{
					text = text + "<TreminationReason>" + num8 + "</TreminationReason>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["bankCode"])))
				{
					text = text + "<BankCode>" + Function.RawText(dataTable7.Rows[0]["bankCode"]) + "</BankCode>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["bankBranch"])))
				{
					text = text + "<BankBranch>" + Function.RawText(dataTable7.Rows[0]["bankBranch"]) + "</BankBranch>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["bankBranNo"])))
				{
					text = text + "<BankBranchNum>" + Function.RawText(dataTable7.Rows[0]["bankBranNo"]) + "</BankBranchNum>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["bankAcount"])))
				{
					text = text + "<BankAccount>" + Function.RawText(dataTable7.Rows[0]["bankAcount"]) + "</BankAccount>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["homeStreet"])))
				{
					text = text + "<HomeStreet>" + Function.RawText(dataTable7.Rows[0]["homeStreet"]) + "</HomeStreet>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["homeBlock"])))
				{
					text = text + "<HomeBlock>" + Function.RawText(dataTable7.Rows[0]["homeBlock"]) + "</HomeBlock>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["homeZip"])))
				{
					text = text + "<HomeZipCode>" + Function.RawText(dataTable7.Rows[0]["homeZip"]) + "</HomeZipCode>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["homeCity"])))
				{
					text = text + "<HomeCity>" + Function.RawText(dataTable7.Rows[0]["homeCity"]) + "</HomeCity>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["homeCounty"])))
				{
					text = text + "<HomeCounty>" + Function.RawText(dataTable7.Rows[0]["homeCounty"]) + "</HomeCounty>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["homeCountr"])))
				{
					text = text + "<HomeCountry>" + Function.RawText(dataTable7.Rows[0]["homeCountr"]) + "</HomeCountry>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["homeState"])))
				{
					text = text + "<HomeState>" + Function.RawText(dataTable7.Rows[0]["homeState"]) + "</HomeState>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["birthDate"])) && !Function.ParseDateTimes(dataTable7.Rows[0]["birthDate"]).ToString("yyyyMMdd").Equals("00010101") && Function.ParseDateTimes(dataTable7.Rows[0]["birthDate"]) > new DateTime(1900, 1, 1))
				{
					text = text + "<DateOfBirth>" + Function.ParseDateTimes(dataTable7.Rows[0]["birthDate"]).ToString("yyyyMMdd") + "</DateOfBirth>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["brthCountr"])))
				{
					text = text + "<CountryOfBirth>" + Function.RawText(dataTable7.Rows[0]["brthCountr"]) + "</CountryOfBirth>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["martStatus"])))
				{
					text = text + "<MartialStatus>" + Function.RawText(dataTable7.Rows[0]["martStatus"]) + "</MartialStatus>";
				}
				int num9 = Function.ParseInt(dataTable7.Rows[0]["nChildren"]);
				if (num9 > 0)
				{
					text = text + "<NumOfChildren>" + num9 + "</NumOfChildren>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["govID"])))
				{
					text = text + "<IdNumber>" + Function.RawText(dataTable7.Rows[0]["govID"]) + "</IdNumber>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["citizenshp"])))
				{
					text = text + "<CitizenshipCountryCode>" + Function.RawText(dataTable7.Rows[0]["citizenshp"]) + "</CitizenshipCountryCode>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["passportNo"])))
				{
					text = text + "<PassportNumber>" + Function.RawText(dataTable7.Rows[0]["passportNo"]) + "</PassportNumber>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["passportEx"])) && !Function.ParseDateTimes(dataTable7.Rows[0]["passportEx"]).ToString("yyyyMMdd").Equals("00010101") && Function.ParseDateTimes(dataTable7.Rows[0]["passportEx"]) > new DateTime(1900, 1, 1))
				{
					text = text + "<PassportExpirationDate>" + Convert.ToDateTime(dataTable7.Rows[0]["passportEx"]).ToString("yyyyMMdd") + "</PassportExpirationDate>";
				}
				text = text + "<Picture>" + Function.RawText(dataTable7.Rows[0]["picture"]) + "</Picture>";
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["remark"])))
				{
					text = text + "<Remarks>" + Function.RawText(dataTable7.Rows[0]["remark"]) + "</Remarks>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["salaryCurr"])))
				{
					text = text + "<SalaryCurrency>" + Function.RawText(dataTable7.Rows[0]["salaryCurr"]) + "</SalaryCurrency>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["empCostCur"])))
				{
					text = text + "<EmployeeCostsCurrency>" + Function.RawText(dataTable7.Rows[0]["empCostCur"]) + "</EmployeeCostsCurrency>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["WorkBuild"])))
				{
					text = text + "<WorkBuildingFloorRoom>" + Function.RawText(dataTable7.Rows[0]["WorkBuild"]) + "</WorkBuildingFloorRoom>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["HomeBuild"])))
				{
					text = text + "<HomeBuildingFloorRoom>" + Function.RawText(dataTable7.Rows[0]["HomeBuild"]) + "</HomeBuildingFloorRoom>";
				}
				int num10 = Function.ParseInt(dataTable7.Rows[0]["position"]);
				if (num10 != 0)
				{
					text = text + "<Position>" + num10 + "</Position>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["CostCenter"])))
				{
					text = text + "<CostCenterCode>" + Function.RawText(dataTable7.Rows[0]["CostCenter"]) + "</CostCenterCode>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["CompanyNum"])))
				{
					text = text + "<CompanyNumber>" + Function.RawText(dataTable7.Rows[0]["CompanyNum"]) + "</CompanyNumber>";
				}
				int num11 = Function.ParseInt(dataTable7.Rows[0]["VacPreYear"]);
				if (num11 > 0)
				{
					text = text + "<VacationPreviousYear>" + num11 + "</VacationPreviousYear>";
				}
				int num12 = Function.ParseInt(dataTable7.Rows[0]["VacCurYear"]);
				if (num12 > 0)
				{
					text = text + "<VacationCurrentYear>" + num12 + "</VacationCurrentYear>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["MunKey"])))
				{
					text = text + "<MunicipalityKey>" + Function.RawText(dataTable7.Rows[0]["MunKey"]) + "</MunicipalityKey>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["TaxClass"])))
				{
					text = text + "<TaxClass>" + Function.RawText(dataTable7.Rows[0]["TaxClass"]) + "</TaxClass>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["InTaxLiabi"])))
				{
					text = text + "<IncomeTaxLiability>" + Function.RawText(dataTable7.Rows[0]["InTaxLiabi"]) + "</IncomeTaxLiability>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["EmTaxCCode"])))
				{
					text = text + "<Religion>" + Function.RawText(dataTable7.Rows[0]["EmTaxCCode"]) + "</Religion>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["RelPartner"])))
				{
					text = text + "<PartnerReligion>" + Function.RawText(dataTable7.Rows[0]["RelPartner"]) + "</PartnerReligion>";
				}
				if (Function.ParseDouble(dataTable7.Rows[0]["ExemptAmnt"]) > 0.0)
				{
					text = text + "<ExemptionAmount>" + Function.ToString(dataTable7.Rows[0]["ExemptAmnt"]) + "</ExemptionAmount>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["ExemptUnit"])))
				{
					text = text + "<ExemptionUnit>" + Function.RawText(dataTable7.Rows[0]["ExemptUnit"]) + "</ExemptionUnit>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["ExemptCurr"])))
				{
					text = text + "<ExemptionCurrency>" + Function.RawText(dataTable7.Rows[0]["ExemptCurr"]) + "</ExemptionCurrency>";
				}
				if (Function.ParseDouble(dataTable7.Rows[0]["AddiAmnt"]) > 0.0)
				{
					text = text + "<AdditionalAmount>" + Function.ToString(dataTable7.Rows[0]["AddiAmnt"]) + "</AdditionalAmount>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["AddiUnit"])))
				{
					text = text + "<AdditionalUnit>" + Function.RawText(dataTable7.Rows[0]["AddiUnit"]) + "</AdditionalUnit>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["AddiCurr"])))
				{
					text = text + "<AdditionalCurrency>" + Function.RawText(dataTable7.Rows[0]["AddiCurr"]) + "</AdditionalCurrency>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["TaxOName"])))
				{
					text = text + "<TaxOfficeName>" + Function.RawText(dataTable7.Rows[0]["TaxOName"]) + "</TaxOfficeName>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["TaxONum"])))
				{
					text = text + "<TaxOfficeNumber>" + Function.RawText(dataTable7.Rows[0]["TaxONum"]) + "</TaxOfficeNumber>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["HeaInsName"])))
				{
					text = text + "<HealthInsuranceName>" + Function.RawText(dataTable7.Rows[0]["HeaInsName"]) + "</HealthInsuranceName>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["HeaInsCode"])))
				{
					text = text + "<HealthInsuranceCode>" + Function.RawText(dataTable7.Rows[0]["HeaInsCode"]) + "</HealthInsuranceCode>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["HeaInsType"])))
				{
					text = text + "<HealthInsuranceType>" + Function.RawText(dataTable7.Rows[0]["HeaInsType"]) + "</HealthInsuranceType>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["SInsurNum"])))
				{
					text = text + "<SocialInsuranceNumber>" + Function.RawText(dataTable7.Rows[0]["SInsurNum"]) + "</SocialInsuranceNumber>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["StatusOfP"])))
				{
					text = text + "<ProfessionStatus>" + Function.RawText(dataTable7.Rows[0]["StatusOfP"]) + "</ProfessionStatus>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["StatusOfE"])))
				{
					text = text + "<EducationStatus>" + Function.RawText(dataTable7.Rows[0]["StatusOfE"]) + "</EducationStatus>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["PersGroup"])))
				{
					text = text + "<PersonGroup>" + Function.RawText(dataTable7.Rows[0]["PersGroup"]) + "</PersonGroup>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["JTCode"])))
				{
					text = text + "<JobTitleCode>" + Function.RawText(dataTable7.Rows[0]["JTCode"]) + "</JobTitleCode>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["BCodeDateV"])))
				{
					text = text + "<BankCodeForDATEV>" + Function.RawText(dataTable7.Rows[0]["BCodeDateV"]) + "</BankCodeForDATEV>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["DevBAOwner"])))
				{
					text = text + "<DeviatingBankAccountOwner>" + Function.RawText(dataTable7.Rows[0]["DevBAOwner"]) + "</DeviatingBankAccountOwner>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["FNameSP"])))
				{
					text = text + "<SpouseFirstName>" + Function.RawText(dataTable7.Rows[0]["FNameSP"]) + "</SpouseFirstName>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["SurnameSP"])))
				{
					text = text + "<SpouseSurname>" + Function.RawText(dataTable7.Rows[0]["SurnameSP"]) + "</SpouseSurname>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["ExtEmpNo"])))
				{
					text = text + "<ExternalEmployeeNumber>" + Function.RawText(dataTable7.Rows[0]["ExtEmpNo"]) + "</ExternalEmployeeNumber>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["BirthPlace"])))
				{
					text = text + "<BirthPlace>" + Function.RawText(dataTable7.Rows[0]["BirthPlace"]) + "</BirthPlace>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["PymMeth"])))
				{
					text = text + "<PaymentMethod>" + Function.RawText(dataTable7.Rows[0]["PymMeth"]) + "</PaymentMethod>";
				}
				int num13 = Function.ParseInt(dataTable7.Rows[0]["STDCode"]);
				if (num13 != 0)
				{
					text = text + "<STDCode>" + num13 + "</STDCode>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["CPF"])))
				{
					text = text + "<CPF>" + Function.RawText(dataTable7.Rows[0]["CPF"]) + "</CPF>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["CRC"])))
				{
					text = text + "<CRCNumber>" + Function.RawText(dataTable7.Rows[0]["CRC"]) + "</CRCNumber>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["ContResp"])))
				{
					text = text + "<AccountantResponsible>" + Function.RawText(dataTable7.Rows[0]["ContResp"]) + "</AccountantResponsible>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["RepLegal"])))
				{
					text = text + "<LegalRepresentative>" + Function.RawText(dataTable7.Rows[0]["RepLegal"]) + "</LegalRepresentative>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["DirfDeclar"])))
				{
					text = text + "<DIRFResponsible>" + Function.RawText(dataTable7.Rows[0]["DirfDeclar"]) + "</DIRFResponsible>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["UF_CRC"])))
				{
					text = text + "<CRCState>" + Function.RawText(dataTable7.Rows[0]["UF_CRC"]) + "</CRCState>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["IDType"])))
				{
					text = text + "<IDType>" + Function.RawText(dataTable7.Rows[0]["IDType"]) + "</IDType>";
				}
				int num14 = Function.ParseInt(dataTable7.Rows[0]["BPLId"]);
				if (num14 != 0)
				{
					text = text + "<BPLID>" + num14 + "</BPLID>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["QualCode"])))
				{
					text = text + "<QualificationCode>" + Function.RawText(dataTable7.Rows[0]["QualCode"]) + "</QualificationCode>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["PRWebAccss"])))
				{
					text = text + "<PRWebAccess>" + Function.RawText(dataTable7.Rows[0]["PRWebAccss"]) + "</PRWebAccess>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["StreetNoW"])))
				{
					text = text + "<WorkStreetNumber>" + Function.RawText(dataTable7.Rows[0]["StreetNoW"]) + "</WorkStreetNumber>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["StreetNoH"])))
				{
					text = text + "<HomeStreetNumber>" + Function.RawText(dataTable7.Rows[0]["StreetNoH"]) + "</HomeStreetNumber>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["BPLink"])))
				{
					text = text + "<LinkedVendor>" + Function.RawText(dataTable7.Rows[0]["BPLink"]) + "</LinkedVendor>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["Active"])))
				{
					text = text + "<Active>" + Function.RawText(dataTable7.Rows[0]["Active"]) + "</Active>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["PassIssue"])) && Function.ParseDateTimes(dataTable7.Rows[0]["PassIssue"]) > new DateTime(1900, 1, 1))
				{
					text = text + "<PassportIssueDate>" + Function.ParseDateTimes(dataTable7.Rows[0]["PassIssue"]).ToString("yyyyMMdd") + "</PassportIssueDate>";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable7.Rows[0]["PassIssuer"])))
				{
					text = text + "<PassportIssuer>" + Function.RawText(dataTable7.Rows[0]["PassIssuer"]) + "</PassportIssuer>";
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
				text += "</row></EmployeesInfo>";
				if (dataTable12 != null && dataTable12.Rows.Count > 0)
				{
					text += "<EmployeeRolesInfo>";
					for (int num16 = 0; num16 < dataTable12.Rows.Count; num16++)
					{
						text += "<row>";
						text = text + "<EmployeeID>" + Function.ParseInt(dataTable7.Rows[0]["empID"]) + "</EmployeeID>";
						if (Function.ParseInt(dataTable12.Rows[num16]["line"]) > 0)
						{
							text = text + "<LineNum>" + Function.ParseInt(dataTable12.Rows[num16]["line"]) + "</LineNum>";
						}
						int num17 = Function.ParseInt(dataTable12.Rows[num16]["roleID"]);
						if (num17 != 0)
						{
							text = text + "<RoleID>" + num17 + "</RoleID>";
						}
						text += DataService.MappingUdf(dataTable12.Rows[num16]);
						text += "</row>";
					}
					text += "</EmployeeRolesInfo>";
				}
				if (dataTable8 != null && dataTable8.Rows.Count > 0)
				{
					text += "<EmployeeAbsenceInfo>";
					for (int num18 = 0; num18 < dataTable8.Rows.Count; num18++)
					{
						text += "<row>";
						text = text + "<EmployeeID>" + Function.ParseInt(dataTable7.Rows[0]["empID"]) + "</EmployeeID>";
						if (Function.ParseInt(dataTable8.Rows[num18]["line"]) > 0)
						{
							text = text + "<LineNum>" + Function.ParseInt(dataTable8.Rows[num18]["line"]) + "</LineNum>";
						}
						if (!string.IsNullOrEmpty(Function.ToString(dataTable8.Rows[num18]["fromDate"])) && !Function.ParseDateTimes(dataTable8.Rows[num18]["fromDate"]).ToString("yyyyMMdd").Equals("00010101") && Function.ParseDateTimes(dataTable8.Rows[num18]["fromDate"]) > new DateTime(1900, 1, 1))
						{
							text = text + "<FromDate>" + Function.ParseDateTimes(dataTable8.Rows[num18]["fromDate"]).ToString("yyyyMMdd") + "</FromDate>";
						}
						if (!string.IsNullOrEmpty(Function.ToString(dataTable8.Rows[num18]["toDate"])) && !Function.ParseDateTimes(dataTable8.Rows[num18]["toDate"]).ToString("yyyyMMdd").Equals("00010101") && Function.ParseDateTimes(dataTable8.Rows[num18]["toDate"]) > new DateTime(1900, 1, 1))
						{
							text = text + "<ToDate>" + Function.ParseDateTimes(dataTable8.Rows[num18]["toDate"]).ToString("yyyyMMdd") + "</ToDate>";
						}
						if (!string.IsNullOrEmpty(Function.ToString(dataTable8.Rows[num18]["reason"])))
						{
							text = text + "<Reason>" + Function.RawText(dataTable8.Rows[num18]["reason"]) + "</Reason>";
						}
						if (!string.IsNullOrEmpty(Function.ToString(dataTable8.Rows[num18]["approvedBy"])))
						{
							text = text + "<ApprovedBy>" + Function.RawText(dataTable8.Rows[num18]["approvedBy"]) + "</ApprovedBy>";
						}
						int num19 = Function.ParseInt(dataTable8.Rows[num18]["cnfrmrNum"]);
						if (num19 != 0)
						{
							text = text + "<ConfirmerNumber>" + num19 + "</ConfirmerNumber>";
						}
						text += DataService.MappingUdf(dataTable8.Rows[num18]);
						text += "</row>";
					}
					text += "</EmployeeAbsenceInfo>";
				}
				if (dataTable9 != null && dataTable9.Rows.Count > 0)
				{
					text += "<EmployeeEducationInfo>";
					for (int num20 = 0; num20 < dataTable9.Rows.Count; num20++)
					{
						text += "<row>";
						text = text + "<EmployeeNo>" + Function.ParseInt(dataTable7.Rows[0]["empID"]) + "</EmployeeNo>";
						if (Function.ParseInt(dataTable9.Rows[num20]["line"]) > 0)
						{
							text = text + "<LineNum>" + Function.ParseInt(dataTable9.Rows[num20]["line"]) + "</LineNum>";
						}
						if (!string.IsNullOrEmpty(Function.ToString(dataTable9.Rows[num20]["fromDate"])) && !Function.ParseDateTimes(dataTable9.Rows[num20]["fromDate"]).ToString("yyyyMMdd").Equals("00010101") && Function.ParseDateTimes(dataTable9.Rows[num20]["fromDate"]) > new DateTime(1900, 1, 1))
						{
							text = text + "<FromDate>" + Function.ParseDateTimes(dataTable9.Rows[num20]["fromDate"]).ToString("yyyyMMdd") + "</FromDate>";
						}
						if (!string.IsNullOrEmpty(Function.ToString(dataTable9.Rows[num20]["toDate"])) && !Function.ParseDateTimes(dataTable9.Rows[num20]["toDate"]).ToString("yyyyMMdd").Equals("00010101") && Function.ParseDateTimes(dataTable9.Rows[num20]["toDate"]) > new DateTime(1900, 1, 1))
						{
							text = text + "<ToDate>" + Function.ParseDateTimes(dataTable9.Rows[num20]["toDate"]).ToString("yyyyMMdd") + "</ToDate>";
						}
						if (!string.IsNullOrEmpty(Function.ToString(dataTable9.Rows[num20]["institute"])))
						{
							text = text + "<Institute>" + Function.RawText(dataTable9.Rows[num20]["institute"]) + "</Institute>";
						}
						if (!string.IsNullOrEmpty(Function.ToString(dataTable9.Rows[num20]["major"])))
						{
							text = text + "<Major>" + Function.RawText(dataTable9.Rows[num20]["major"]) + "</Major>";
						}
						if (!string.IsNullOrEmpty(Function.ToString(dataTable9.Rows[num20]["diploma"])))
						{
							text = text + "<Diploma>" + Function.RawText(dataTable9.Rows[num20]["diploma"]) + "</Diploma>";
						}
						int num21 = Function.ParseInt(dataTable9.Rows[num20]["type"]);
						if (num21 != 0)
						{
							text = text + "<EducationType>" + num21 + "</EducationType>";
						}
						text += DataService.MappingUdf(dataTable9.Rows[num20]);
						text += "</row>";
					}
					text += "</EmployeeEducationInfo>";
				}
				if (dataTable10 != null && dataTable10.Rows.Count > 0)
				{
					text += "<EmployeeReviewsInfo>";
					for (int num22 = 0; num22 < dataTable10.Rows.Count; num22++)
					{
						text += "<row>";
						text = text + "<EmployeeNo>" + Function.ParseInt(dataTable7.Rows[0]["empID"]) + "</EmployeeNo>";
						if (Function.ParseInt(dataTable10.Rows[num22]["line"]) > 0)
						{
							text = text + "<LineNum>" + Function.ParseInt(dataTable10.Rows[num22]["line"]) + "</LineNum>";
						}
						if (!string.IsNullOrEmpty(Function.ToString(dataTable10.Rows[num22]["date"])) && !Function.ParseDateTimes(dataTable10.Rows[num22]["date"]).ToString("yyyyMMdd").Equals("00010101") && Function.ParseDateTimes(dataTable10.Rows[num22]["date"]) > new DateTime(1900, 1, 1))
						{
							text = text + "<Date>" + Function.ParseDateTimes(dataTable10.Rows[num22]["date"]).ToString("yyyyMMdd") + "</Date>";
						}
						if (!string.IsNullOrEmpty(Function.ToString(dataTable10.Rows[num22]["reviewDesc"])))
						{
							text = text + "<ReviewDescription>" + Function.RawText(dataTable10.Rows[num22]["reviewDesc"]) + "</ReviewDescription>";
						}
						if (!string.IsNullOrEmpty(Function.ToString(dataTable10.Rows[num22]["grade"])))
						{
							text = text + "<Grade>" + Function.RawText(dataTable10.Rows[num22]["grade"]) + "</Grade>";
						}
						if (!string.IsNullOrEmpty(Function.ToString(dataTable10.Rows[num22]["remarks"])))
						{
							text = text + "<Remarks>" + Function.RawText(dataTable10.Rows[num22]["remarks"]) + "</Remarks>";
						}
						int num23 = Function.ParseInt(dataTable10.Rows[num22]["manager"]);
						if (num23 != 0)
						{
							text = text + "<Manager>" + num23 + "</Manager>";
						}
						text += DataService.MappingUdf(dataTable10.Rows[num22]);
						text += "</row>";
					}
					text += "</EmployeeReviewsInfo>";
				}
				if (dataTable11 == null || dataTable11.Rows.Count <= 0)
				{
					break;
				}
				text += "<EmployeePrevEmpoymentInfo>";
				for (int num24 = 0; num24 < dataTable11.Rows.Count; num24++)
				{
					text += "<row>";
					text = text + "<EmployeeNo>" + Function.ParseInt(dataTable7.Rows[0]["empID"]) + "</EmployeeNo>";
					if (Function.ParseInt(dataTable11.Rows[num24]["line"]) > 0)
					{
						text = text + "<LineNum>" + Function.ParseInt(dataTable11.Rows[num24]["line"]) + "</LineNum>";
					}
					if (!string.IsNullOrEmpty(Function.ToString(dataTable11.Rows[num24]["fromDate"])) && !Function.ParseDateTimes(dataTable11.Rows[num24]["fromDate"]).ToString("yyyyMMdd").Equals("00010101") && Function.ParseDateTimes(dataTable11.Rows[num24]["fromDate"]) > new DateTime(1900, 1, 1))
					{
						text = text + "<FromDtae>" + Function.ParseDateTimes(dataTable11.Rows[num24]["fromDate"]).ToString("yyyyMMdd") + "</FromDtae>";
					}
					if (!string.IsNullOrEmpty(Function.ToString(dataTable11.Rows[num24]["toDate"])) && !Function.ParseDateTimes(dataTable11.Rows[num24]["toDate"]).ToString("yyyyMMdd").Equals("00010101") && Function.ParseDateTimes(dataTable11.Rows[num24]["toDate"]) > new DateTime(1900, 1, 1))
					{
						text = text + "<ToDate>" + Function.ParseDateTimes(dataTable11.Rows[num24]["toDate"]).ToString("yyyyMMdd") + "</ToDate>";
					}
					if (!string.IsNullOrEmpty(Function.ToString(dataTable11.Rows[num24]["employer"])))
					{
						text = text + "<Employer>" + Function.RawText(dataTable11.Rows[num24]["employer"]) + "</Employer>";
					}
					if (!string.IsNullOrEmpty(Function.ToString(dataTable11.Rows[num24]["position"])))
					{
						text = text + "<Position>" + Function.RawText(dataTable11.Rows[num24]["position"]) + "</Position>";
					}
					if (!string.IsNullOrEmpty(Function.ToString(dataTable11.Rows[num24]["remarks"])))
					{
						text = text + "<Remarks>" + Function.RawText(dataTable11.Rows[num24]["remarks"]) + "</Remarks>";
					}
					text += DataService.MappingUdf(dataTable11.Rows[num24]);
					text += "</row>";
				}
				text += "</EmployeePrevEmpoymentInfo>";
				break;
			}
			}
			text += "</BO></BOM>";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return text;
	}

	public virtual bool Exist(int empID, string sessionId, ApzObjectTypes objType)
	{
		try
		{
			string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?>\r\n\r\n                            <env:Envelope xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n\r\n                             <env:Header>\r\n\r\n                              <SessionID>" + sessionId + "</SessionID> \r\n\r\n                             </env:Header>\r\n\r\n                             <env:Body>\r\n\r\n                              <dis:GetByKey xmlns:dis=\"http://www.sap.com/SBO/DIS\">\r\n\r\n                               <Object>" + objType.ToString() + "</Object> \r\n\r\n                               <EmployeeID>" + empID + "</EmployeeID> \r\n\r\n                              </dis:GetByKey>\r\n\r\n                             </env:Body>\r\n\r\n                            </env:Envelope>";
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
			return !empID.Equals(0);
		}
	}

	public virtual string Remove(int empID, string sessionId)
	{
		string result = "";
		try
		{
			string xmlObject = "<?xml version=\"1.0\" ?>  \r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>EmployeesInfoService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>\r\n                                    <Remove xmlns=\"EmployeesInfoService\">\r\n                                     <EmployeeInfoParams>   <EmployeeID>" + empID + "</EmployeeID></EmployeeInfoParams>\r\n                                    </Remove>  \r\n                                </soap:Body>  \r\n                            </soap:Envelope>  ";
			DataService.GetServiceMetaData(sessionId, "EmployeesInfoService");
			DataService.GetBusinessObjectTemplate(sessionId, "oEmployeesInfo");
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

	protected string UpdateApzTableQuery(int empId, DataTable dtHealth)
	{
		try
		{
			string text = $" DELETE FROM APZ_OHEH WHERE \"empID\"={empId}; ";
			for (int i = 0; i < dtHealth.Rows.Count; i++)
			{
				string text2 = string.Empty;
				string text3 = string.Empty;
				for (int j = 0; j < dtHealth.Columns.Count; j++)
				{
					text2 += ((j == dtHealth.Columns.Count - 1) ? Function.ToString("\"" + dtHealth.Columns[j].ColumnName + "\"") : Function.ToString("\"" + dtHealth.Columns[j].ColumnName + "\","));
					text3 += ((j == dtHealth.Columns.Count - 1) ? Function.ToString(FormatValue(dtHealth.Rows[i][Function.ToString(dtHealth.Columns[j].ColumnName)])) : Function.ToString(FormatValue(dtHealth.Rows[i][Function.ToString(dtHealth.Columns[j].ColumnName)]) + ","));
				}
				text += string.Format(" INSERT INTO APZ_OHEH (" + text2 + ") VALUES (" + text3 + ");");
			}
			return text;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return "";
	}

	protected string UpdateTeamsMemberQuery(int empId, DataTable dtTeams)
	{
		try
		{
			string text = $" DELETE FROM HTM1 WHERE \"empID\"={empId};";
			for (int i = 0; i < dtTeams.Rows.Count; i++)
			{
				text += string.Format(" INSERT INTO HTM1 (\"empID\", \"line\", \"teamID\", \"role\")  VALUES({0},{1},{2},{3});", FormatValue(empId), FormatValue(dtTeams.Rows[i]["line"]), FormatValue(dtTeams.Rows[i]["teamID"]), FormatValue(dtTeams.Rows[i]["role"]));
			}
			return text;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return "";
	}

	private int SaveFile()
	{
		try
		{
			string query = "select max(\"AbsEntry\") from oatc;";
			string obj = ExecuteScarlar(query, CommandType.Text);
			query = "insert into oatc (\"AbsEntry\") values (" + (Function.ParseInt(obj) + 1) + "); ";
			if (ExecuteNonQuery(query, CommandType.Text) > 0)
			{
				return Function.ParseInt(obj) + 1;
			}
			return 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return 0;
		}
	}

	protected string DeleteFile(int absEntry)
	{
		try
		{
			string query = "delete from ATC1 WHERE \"AbsEntry\" = @absEntry;";
			if (ExecuteNonQuery(query, CommandType.Text, new SqlParameter("@absEntry", absEntry)) >= 0)
			{
				query = "delete from OATC WHERE \"AbsEntry\" = @absEntry;";
				if (ExecuteNonQuery(query, CommandType.Text, new SqlParameter("@absEntry", absEntry)) >= 0)
				{
					return "";
				}
			}
			return "ERROR";
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
	}

	protected string UpdateFile(DataTable dt, int absEntry)
	{
		ITransaction transaction = new Transaction();
		try
		{
			string query = "delete from ATC1 where \"AbsEntry\"=@absEntry;";
			if (ExecuteNonQuery(query, CommandType.Text, new SqlParameter("@absEntry", absEntry)) > 0)
			{
				AddFile(dt, absEntry, transaction);
				transaction.Commit();
				return string.Empty;
			}
			transaction.Rollback();
			return "Eror";
		}
		catch (Exception ex)
		{
			transaction.Rollback();
			return ex.Message;
		}
	}

	private string AddFile(DataTable dt, int absEntry, ITransaction trans = null)
	{
		if (trans == null)
		{
			trans = new Transaction();
		}
		try
		{
			string empty = string.Empty;
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				List<SqlParameter> list = new List<SqlParameter>();
				empty = "INSERT INTO \"ATC1\" (\"AbsEntry\", \"Line\", \"srcPath\", \"trgtPath\", \"FileName\", \"FileExt\", \"Date\")                            \r\n                         VALUES (";
				empty += "@AbsEntry ";
				list.Add(new SqlParameter("@AbsEntry", absEntry));
				empty += ",@Line ";
				list.Add(new SqlParameter("@Line", i + 1));
				empty += ", @srcPath ";
				list.Add(new SqlParameter("@srcPath", Function.ToString(dt.Rows[i]["srcPath"])));
				empty += ",@trgtPath ";
				list.Add(new SqlParameter("@trgtPath", Function.ToString(dt.Rows[i]["trgtPath"])));
				empty += ",@FileName ";
				list.Add(new SqlParameter("@FileName", Function.ToString(dt.Rows[i]["FileName"])));
				empty += ", @FileExt ";
				list.Add(new SqlParameter("@FileExt", Function.ToString(dt.Rows[i]["FileExt"])));
				empty += ",@Date ";
				list.Add(new SqlParameter("@Date", Function.ParseDateTime(dt.Rows[i]["Date"])));
				empty += " );";
				if (ExecuteNonQuery(empty, CommandType.Text, list.ToArray()) <= 0)
				{
					trans.Rollback();
					return "eror";
				}
			}
			trans.Commit();
			return string.Empty;
		}
		catch (Exception ex)
		{
			trans.Rollback();
			return ex.Message;
		}
	}
}
