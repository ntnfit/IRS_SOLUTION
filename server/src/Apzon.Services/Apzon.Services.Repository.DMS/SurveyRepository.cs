using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.DMS;
using Apzon.Services.Interfaces.DMS;

namespace Apzon.Services.Repository.DMS;

public class SurveyRepository : BaseService<SurveyImpl>, ISurvey
{
	private const string temp = "#APZ_D_OSUV";

	private const string tempLine = "#APZ_D_SUV1";

	public SurveyRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public HttpResult Save(SurveyImpl entities, string action)
	{
		try
		{
			Enums.TransactionType transtype = Enums.TransactionType.A;
			switch (action)
			{
			case "A":
				transtype = Enums.TransactionType.A;
				break;
			case "U":
				transtype = Enums.TransactionType.U;
				break;
			case "D":
				transtype = Enums.TransactionType.D;
				break;
			}
			return Process(ApzObjectTypes.oSurvey, -1, transtype, entities);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, SurveyImpl entities, ref string objId, ref string objNum)
	{
		new HttpResult();
		switch (transtype)
		{
		case Enums.TransactionType.A:
		case Enums.TransactionType.U:
			return SaveData(entities, transtype, ref objId, ref objNum);
		case Enums.TransactionType.D:
			return DeleteData(entities, ref objId, ref objNum);
		default:
			return new HttpResult(Enums.MessageCode.Error, "Could not execute function");
		}
	}

	public HttpResult SaveData(SurveyImpl entites, Enums.TransactionType tranType, ref string objId, ref string objNum)
	{
		try
		{
			string value = Function.ToString(tranType);
			DataTable tableStruct = GetTableStruct("APZ_D_OSUV");
			if (!tableStruct.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			DataTable dataTable = tableStruct.Clone();
			DataRow dataRow = dataTable.NewRow();
			dataRow["DocEntry"] = entites.DocEntry;
			dataRow["PersonName"] = entites.PersonName;
			dataRow["Address"] = entites.Address;
			dataRow["Email"] = entites.Email;
			dataRow["Phone"] = entites.Phone;
			dataRow["Notes"] = entites.Notes;
			dataRow["Rate"] = entites.Rate;
			dataRow["SurveyQuessionId"] = entites.SurveyQuessionId;
			dataRow["ClgCode"] = entites.ClgCode;
			dataRow["CreateTime"] = entites.CreateTime;
			dataRow["UpdateTime"] = entites.UpdateTime;
			dataTable.Rows.Add(dataRow);
			CreateTempTable(dataTable, "#APZ_D_OSUV");
			BulkCopy(dataTable, "#APZ_D_OSUV");
			DataTable tableStruct2 = GetTableStruct("APZ_D_SUV1");
			if (!tableStruct2.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			DataTable dataTable2 = tableStruct2.Clone();
			List<SurveyLineImpl> surveyLine = entites.SurveyLine;
			if (surveyLine != null && surveyLine.Count > 0)
			{
				for (int i = 0; i < surveyLine.Count; i++)
				{
					DataRow dataRow2 = dataTable2.Rows.Add();
					dataRow2["DocEntry"] = surveyLine[i].DocEntry;
					dataRow2["LineNum"] = surveyLine[i].LineNum;
					dataRow2["VisOrder"] = surveyLine[i].VisOrder;
					dataRow2["Question"] = surveyLine[i].Question;
					dataRow2["Answer"] = surveyLine[i].Answer;
				}
			}
			CreateTempTable(dataTable2, "#APZ_D_SUV1");
			BulkCopy(dataTable2, "#APZ_D_SUV1");
			SqlParameter sqlParameter = new SqlParameter("@type", value);
			DataTable dataTable3 = ExecuteDataTable("APZ_D_SurveyDMS_Save", CommandType.StoredProcedure, sqlParameter);
			if (dataTable3 != null && dataTable3.Rows.Count > 0)
			{
				if (string.IsNullOrEmpty(Function.ToString(dataTable3.Rows[0][1])))
				{
					objId = Function.ToString(dataTable3.Rows[0][0]);
					objNum = objId;
					return new HttpResult(Enums.MessageCode.Success, "", objId);
				}
				return new HttpResult(Enums.MessageCode.Error, $"{Function.ParseInt(dataTable3.Rows[0][0])}-{Function.ToString(dataTable3.Rows[0][1])}");
			}
			return new HttpResult(Enums.MessageCode.Error, "Data is not valid");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public DataTable GetAllSurvey(int clgCode)
	{
		DataTable result = new DataTable();
		try
		{
			string query = "SELECT *\r\n                              FROM APZ_D_OSUV where \"ClgCode\"=@clgCode  order by \"CreateTime\" Desc;";
			return result = ExecuteDataTable(query, CommandType.Text, new SqlParameter("@clgCode", clgCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "ShowAddress  FAILED ", ex.Message);
			return result;
		}
	}

	public DataTable GetQuestionById(int docentry)
	{
		DataTable result = new DataTable();
		try
		{
			string query = "SELECT *\r\n                              FROM APZ_D_CHM1 where \"DocEntry\"=@docentry  order by \"VisOrder\" Asc;";
			return result = ExecuteDataTable(query, CommandType.Text, new SqlParameter("docentry", docentry));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "ShowAddress  FAILED ", ex.Message);
			return result;
		}
	}

	public DataSet GetSurveyById(int docEntry)
	{
		DataSet result = new DataSet();
		try
		{
			string query = "APZ_D_SurveyLineById";
			return result = ExecuteData(query, CommandType.StoredProcedure, new SqlParameter("docEntry", docEntry));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "ShowAddress  FAILED ", ex.Message);
			return result;
		}
	}

	public HttpResult DeleteData(SurveyImpl entities, ref string objId, ref string objNum)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@docEntry", entities.DocEntry));
			DataTable dataTable = ExecuteDataTable("APZ_D_SurveyRemove", CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				if (string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0][1])))
				{
					objId = Function.ToString(dataTable.Rows[0][0]);
					objNum = objId;
					return new HttpResult(Enums.MessageCode.Success, "", objId);
				}
				return new HttpResult(Enums.MessageCode.Error, $"{Function.ParseInt(dataTable.Rows[0][0])}-{Function.ToString(dataTable.Rows[0][1])}");
			}
			return new HttpResult(Enums.MessageCode.Error, $"{Function.ParseInt(dataTable.Rows[0][0])}-{Function.ToString(dataTable.Rows[0][1])}");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}
}
