using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.HumanResources;

namespace Apzon.Services.Repository.HumanResources;

public class EmpMasterDataRepository(IDatabaseService databaseService) : BaseService<DataSet>(databaseService), IEmpMasterData
{
	private string Temp_Doc = "#APZ_Emp_DOC";

	private string Temp_Atc = "#APZ_Emp_ATC";

	private string Temp_Role = "#APZ_Emp_HEM6";

	private string Temp_Team = "#APZ_Emp_HTM1";

	private string Temp_Absence = "#APZ_Emp_HEM1";

	private string Temp_Education = "#APZ_Emp_HEM2";

	private string Temp_Review = "#APZ_Emp_HEM3";

	private string Temp_PrevEmp = "#APZ_Emp_HEM4";

	private string Tem_Health = "#APZ_Emp_OHEH";

	private string Temp_Contract = "#APZ_Contract";

	public DataSet GetDetail(int empId)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@empID", empId)
			};
			DataSet dataSet = ExecuteData("apz_EmpData_GetDetail", CommandType.StoredProcedure, list.ToArray());
			if (dataSet != null && dataSet.Tables.Count > 0)
			{
				return dataSet;
			}
			return new DataSet();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDetail  FAILED ", ex.Message);
			return new DataSet();
		}
	}

	public DataTable Search(DataTable dtDoc, string userCode)
	{
		try
		{
			string text = "select \"empID\" from ohem where 1=1 ";
			try
			{
				if (Function.ToString(ExecuteScalar("Select \"IngoreActiveEmp\" From APZ_OADM;", CommandType.Text)).Equals("Y"))
				{
					dtDoc.Columns.Remove("Active");
				}
			}
			catch (Exception)
			{
			}
			List<SqlParameter> list = new List<SqlParameter>();
			for (int i = 0; i < dtDoc.Columns.Count; i++)
			{
				if (dtDoc.Rows[0][dtDoc.Columns[i].ColumnName] == null)
				{
					continue;
				}
				switch (dtDoc.Columns[i].DataType.Name.ToUpper())
				{
				case "DATETIME":
				{
					DateTime? dateTime = Function.ParseDateTime(dtDoc.Rows[0][dtDoc.Columns[i].ColumnName]);
					if (dateTime.HasValue && dateTime > new DateTime(1900, 1, 1))
					{
						list.Add(new SqlParameter("@" + dtDoc.Columns[i].ColumnName, dateTime));
						text = text + " and \"" + dtDoc.Columns[i].ColumnName + "\"=@" + dtDoc.Columns[i].ColumnName;
					}
					break;
				}
				case "INT64":
				case "INT32":
				case "INT16":
				{
					int num = Function.ParseInt(dtDoc.Rows[0][dtDoc.Columns[i].ColumnName]);
					if (num != 0)
					{
						list.Add(new SqlParameter("@" + dtDoc.Columns[i].ColumnName, num));
						text = text + " and \"" + dtDoc.Columns[i].ColumnName + "\"=@" + dtDoc.Columns[i].ColumnName;
					}
					break;
				}
				case "STRING":
				{
					string text2 = Function.ToString(dtDoc.Rows[0][dtDoc.Columns[i].ColumnName]);
					if (!string.IsNullOrEmpty(text2))
					{
						list.Add(new SqlParameter("@" + dtDoc.Columns[i].ColumnName, "%" + text2 + "%"));
						text = text + " and \"" + dtDoc.Columns[i].ColumnName + "\" like @" + dtDoc.Columns[i].ColumnName + SearchQuery();
					}
					break;
				}
				default:
				{
					string value = Function.ToString(dtDoc.Rows[0][dtDoc.Columns[i].ColumnName]);
					if (!string.IsNullOrEmpty(value))
					{
						list.Add(new SqlParameter("@" + dtDoc.Columns[i].ColumnName, value));
						text = text + " and \"" + dtDoc.Columns[i].ColumnName + "\"=@" + dtDoc.Columns[i].ColumnName;
					}
					break;
				}
				}
			}
			text = GetOwnerShipQuery(text);
			list.Add(new SqlParameter("@userCode", userCode));
			return ExecuteDataTable(text + ";", CommandType.Text, list.ToArray());
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Search  FAILED ", ex2.Message);
			return new DataTable();
		}
	}

	protected virtual string GetOwnerShipQuery(string query)
	{
		return "declare @usrId int = (select USERID from OUSR where USER_CODE = @userCode)\r\n                    " + query + " and (U_OwnerCode IN (select distinct OwnerId from apz_func_OwnerOverShip(@userCode, '171')) OR ISNULL(U_OwnerCode,-1)= -1 or UserSign = @usrId)";
	}

	protected virtual string SearchQuery()
	{
		return " COLLATE SQL_Latin1_General_Cp1_CI_AI ";
	}

	public string GetRecord(int empId, int type, string userCode)
	{
		int num = 0;
		try
		{
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@Current", empId),
				new SqlParameter("@Type", type),
				new SqlParameter("@userCode", userCode)
			};
			DataTable dataTable = ExecuteDataTable("apz_EmpData_navigation", CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				num = dataTable.Rows[0].Field<int>("empID");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetRecord ", ex.Message);
		}
		return num.ToString();
	}

	public DataTable GetTeamMembers()
	{
		try
		{
			string teamMembersQuery = GetTeamMembersQuery();
			DataTable dataTable = ExecuteDataTable(teamMembersQuery, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	protected virtual string GetTeamMembersQuery()
	{
		return "select teamID,name from OHTM";
	}

	public DataTable ShowBankAccByCountryCod(string countryCod)
	{
		try
		{
			string query = ShowBankAccByCountryCodQuery();
			return ExecuteDataTable(query, CommandType.Text, new SqlParameter("@CountryCod", countryCod ?? ""));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	protected virtual string ShowBankAccByCountryCodQuery()
	{
		return "select BankCode as Code,BankName as Name from ODSC WHERE CountryCod = @CountryCod";
	}

	public DataTable ExistFileInDoc(int empId, string fileName, string fileExt)
	{
		DataTable result = new DataTable();
		try
		{
			string query = ExistFileInDocQuery();
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@empID", empId),
				new SqlParameter("@fileExt", fileExt),
				new SqlParameter("@fileName", fileName)
			};
			result = ExecuteDataTable(query, CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	protected virtual string ExistFileInDocQuery()
	{
		return "select FileName from atc1 where AbsEntry=(select AtcEntry from ohem where empID=@empID) \r\n\t                and FileExt=@fileExt and FileName=@fileName and AbsEntry is not null";
	}

	public string Save(DataSet ds)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				DataTable dataTable = ds.Tables[0];
				int num = Function.ParseInt(dataTable.Rows[0]["empID"]);
				int num2 = Function.ParseInt(dataTable.Rows[0]["AtcEntry"]);
				if (ds.Tables.Count > 2 && ds.Tables[2].Rows.Count > 0 && num2 == 0)
				{
					num2 = SaveFile();
					dataTable.Rows[0]["AtcEntry"] = num2;
					dataTable.AcceptChanges();
				}
				string text;
				if (num > 0)
				{
					text = Update(dataTable);
					if (string.IsNullOrEmpty(text) && num2 > 0)
					{
						UpdateFile(ds.Tables[2], num2);
					}
				}
				else
				{
					num = GetId();
					dataTable.Rows[0]["empID"] = num;
					dataTable.AcceptChanges();
					text = Insert(dataTable);
					if (Function.ParseInt(text) > 0)
					{
						if (num2 > 0)
						{
							AddFile(ds.Tables[2], num2);
						}
						text = string.Empty;
					}
				}
				if (string.IsNullOrEmpty(text))
				{
					text = SaveLine(ds.Tables[1], num);
					if (string.IsNullOrEmpty(text))
					{
						dbTransaction.Commit();
					}
				}
				return text;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public string Insert(DataTable dtDoc)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			string text = string.Empty;
			string text2 = string.Empty;
			for (int i = 0; i < dtDoc.Columns.Count; i++)
			{
				if (dtDoc.Rows[0][dtDoc.Columns[i].ColumnName] == null)
				{
					continue;
				}
				switch (dtDoc.Columns[i].DataType.Name.ToUpper())
				{
				case "DATETIME":
				{
					DateTime? dateTime = Function.ParseDateTime(dtDoc.Rows[0][dtDoc.Columns[i].ColumnName]);
					if (dateTime.HasValue && dateTime > new DateTime(1900, 1, 1))
					{
						list.Add(new SqlParameter("@" + dtDoc.Columns[i].ColumnName, dateTime));
						text = text + dtDoc.Columns[i].ColumnName + ",";
						text2 = text2 + "@" + dtDoc.Columns[i].ColumnName + ",";
					}
					break;
				}
				case "INT64":
				case "INT32":
				case "INT16":
					list.Add(new SqlParameter("@" + dtDoc.Columns[i].ColumnName, Convert.ToInt16(dtDoc.Rows[0][dtDoc.Columns[i].ColumnName])));
					text = text + dtDoc.Columns[i].ColumnName + ",";
					text2 = text2 + "@" + dtDoc.Columns[i].ColumnName + ",";
					break;
				default:
				{
					string value = Function.ToString(dtDoc.Rows[0][dtDoc.Columns[i].ColumnName]);
					if (!string.IsNullOrEmpty(value))
					{
						list.Add(new SqlParameter("@" + dtDoc.Columns[i].ColumnName, value));
						text = text + dtDoc.Columns[i].ColumnName + ",";
						text2 = text2 + "@" + dtDoc.Columns[i].ColumnName + ",";
					}
					break;
				}
				}
			}
			text = text.Substring(0, text.Length - 1);
			text2 = text2.Substring(0, text2.Length - 1);
			string query = "insert ohem(" + text + ") values (" + text2 + ")";
			return Function.ToString(ExecuteNonQuery(query, CommandType.Text, list.ToArray()));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public string Update(DataTable dtDoc)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			string text = "UPDATE ohem SET ";
			for (int i = 0; i < dtDoc.Columns.Count; i++)
			{
				if (dtDoc.Rows[0][dtDoc.Columns[i].ColumnName] == null)
				{
					continue;
				}
				switch (dtDoc.Columns[i].DataType.Name.ToUpper())
				{
				case "DATETIME":
				{
					DateTime? dateTime = Function.ParseDateTime(dtDoc.Rows[0][dtDoc.Columns[i].ColumnName]);
					if (dateTime.HasValue && dateTime > new DateTime(1900, 1, 1))
					{
						list.Add(new SqlParameter("@" + dtDoc.Columns[i].ColumnName, dateTime));
						text = text + Environment.NewLine + dtDoc.Columns[i].ColumnName + "=@" + dtDoc.Columns[i].ColumnName + ",";
					}
					break;
				}
				case "INT64":
				case "INT32":
				case "INT16":
					list.Add(new SqlParameter("@" + dtDoc.Columns[i].ColumnName, Convert.ToInt16(dtDoc.Rows[0][dtDoc.Columns[i].ColumnName])));
					text = text + Environment.NewLine + dtDoc.Columns[i].ColumnName + "=@" + dtDoc.Columns[i].ColumnName + ",";
					break;
				default:
				{
					string value = Function.ToString(dtDoc.Rows[0][dtDoc.Columns[i].ColumnName]);
					if (!string.IsNullOrEmpty(value))
					{
						list.Add(new SqlParameter("@" + dtDoc.Columns[i].ColumnName, value));
						text = text + Environment.NewLine + dtDoc.Columns[i].ColumnName + "=@" + dtDoc.Columns[i].ColumnName + ",";
					}
					break;
				}
				}
			}
			text = text.Substring(0, text.Length - 1);
			text += " where empID=@empID";
			if (ExecuteNonQuery(text, CommandType.Text, list.ToArray()) > 0)
			{
				return string.Empty;
			}
			return "Eror";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private int GetId()
	{
		try
		{
			return Function.ParseInt(ExecuteScalar("select max(empID) from ohem", CommandType.Text).ToString()) + 1;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return 0;
		}
	}

	public string SaveLine(DataTable dt, int empId)
	{
		try
		{
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				if (Function.ParseInt(dt.Rows[i]["empID"]) == 0)
				{
					List<SqlParameter> list = new List<SqlParameter>
					{
						new SqlParameter("@empID", empId),
						new SqlParameter("@line", Function.ParseInt(dt.Rows[i]["LineNum"])),
						new SqlParameter("@roleID", Function.ParseInt(dt.Rows[i]["roleID"]))
					};
					if (ExecuteNonQuery("insert hem6(empID,line,roleID) values (@empID,@line,@roleID)", CommandType.Text, list.ToArray()) < 0)
					{
						return "Eror";
					}
				}
				else
				{
					List<SqlParameter> list2 = new List<SqlParameter>
					{
						new SqlParameter("@empID", empId),
						new SqlParameter("@line", Function.ParseInt(dt.Rows[i]["LineNum"])),
						new SqlParameter("@roleID", Function.ParseInt(dt.Rows[i]["roleID"]))
					};
					if (ExecuteNonQuery("update hem6 set roleID=@roleID where empID=@empID and line=@line", CommandType.Text, list2.ToArray()) < 0)
					{
						return "Eror";
					}
				}
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private int SaveFile()
	{
		try
		{
			string query = "select max(AbsEntry) from oatc";
			string obj = ExecuteScalar(query, CommandType.Text).ToString();
			query = "insert into oatc (absEntry) values (" + (Function.ParseInt(obj) + 1) + ") ";
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

	private string DeleteFile(int absEntry)
	{
		try
		{
			string query = "delete from ATC1 WHERE absEntry = @absEntry";
			if (ExecuteNonQuery(query, CommandType.Text, new SqlParameter("@absEntry", absEntry)) >= 0)
			{
				query = "delete from OATC WHERE absEntry = @absEntry";
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

	private string UpdateFile(DataTable dt, int absEntry)
	{
		try
		{
			string query = "delete from ATC1 where AbsEntry=@absEntry";
			if (ExecuteNonQuery(query, CommandType.Text, new SqlParameter("@absEntry", absEntry)) > 0)
			{
				AddFile(dt, absEntry);
				return string.Empty;
			}
			return "Eror";
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
	}

	private string AddFile(DataTable dt, int absEntry)
	{
		try
		{
			string empty = string.Empty;
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				List<SqlParameter> list = new List<SqlParameter>();
				empty = "INSERT INTO [dbo].[ATC1]\r\n                               ([AbsEntry]\r\n                               ,[Line]\r\n                               ,[srcPath]\r\n                               ,[trgtPath]\r\n                               ,[FileName]\r\n                               ,[FileExt]\r\n                               ,[Date])                             \r\n                         VALUES (";
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
				empty += " )";
				if (ExecuteNonQuery(empty, CommandType.Text, list.ToArray()) <= 0)
				{
					return "eror";
				}
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
	}

	public bool Check(string itemCode)
	{
		throw new NotImplementedException();
	}

	public HttpResult SaveDocDataSet(DataSet ds, string action)
	{
		try
		{
			Enums.TransactionType transactionType = Enums.TransactionType.A;
			switch (action)
			{
			case "A":
				transactionType = Enums.TransactionType.A;
				break;
			case "U":
				transactionType = Enums.TransactionType.U;
				break;
			case "D":
				transactionType = Enums.TransactionType.D;
				break;
			default:
				return new HttpResult
				{
					Message = "Employee Master data's Repository: Wrong action. Should be: 'A', 'U' OR 'D'"
				};
			}
			return Process(ApzObjectTypes.oEmployeesInfo, -1, transactionType, ds);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public string SaveWithRollBack(DataTable dt)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction(IsolationLevel.RepeatableRead);
				string text = "";
				int num = 0;
				for (int i = 0; i < dt.Rows.Count; i++)
				{
					DataSet dataSet = new DataSet();
					DataTable dataTable = dt.Clone();
					dataTable.ImportRow(dt.Rows[i]);
					dataTable.TableName = "dtDoc";
					dataSet.Tables.Add(dataTable);
					text = SaveDocDataSet(dataSet, "A").Message;
					if (string.IsNullOrEmpty(text))
					{
						break;
					}
					num++;
				}
				if (num == 0)
				{
					dbTransaction.Commit();
					return "";
				}
				return text;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public string DeleteDoc(int empId)
	{
		try
		{
			return Process(ApzObjectTypes.oEmployeesInfo, -1, Enums.TransactionType.D, new DataSet(), Function.ToString(empId)).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet ds, ref string objId, ref string objNum)
	{
		HttpResult ret = new HttpResult();
		switch (transtype)
		{
		case Enums.TransactionType.A:
			ProcessSave(ref transtype, ds, ref objId, ref objNum, ref ret);
			break;
		case Enums.TransactionType.U:
			ProcessSave(ref transtype, ds, ref objId, ref objNum, ref ret);
			break;
		case Enums.TransactionType.D:
			ProcessDelete(ref ret, ref objId, ref objNum);
			break;
		}
		return ret;
	}

	private void ProcessSave(ref Enums.TransactionType transtype, DataSet ds, ref string objId, ref string objNum, ref HttpResult ret)
	{
		try
		{
			if (ds == null || ds.Tables.Count < 1 || ds.Tables[0] == null || ds.Tables[0].Rows.Count == 0)
			{
				ret.Message = "No data to save";
				return;
			}
			string value = ((transtype == Enums.TransactionType.A) ? "A" : "U");
			CreateTempTable(ds.Tables["dtDoc"], Temp_Doc);
			BulkCopy(ds.Tables["dtDoc"], Temp_Doc);
			if (!ds.Tables.Contains("AttFile"))
			{
				DataTable tableStruct = GetTableStruct("ATC1");
				tableStruct.TableName = "AttFile";
				ds.Tables.Add(tableStruct);
			}
			if (!ds.Tables.Contains("dtRole"))
			{
				DataTable tableStruct2 = GetTableStruct("HEM6");
				tableStruct2.TableName = "dtRole";
				ds.Tables.Add(tableStruct2);
			}
			if (!ds.Tables.Contains("dtTeams"))
			{
				DataTable tableStruct3 = GetTableStruct("HTM1");
				tableStruct3.TableName = "dtTeams";
				ds.Tables.Add(tableStruct3);
			}
			if (!ds.Tables.Contains("dtAbsence"))
			{
				DataTable tableStruct4 = GetTableStruct("HEM1");
				tableStruct4.TableName = "dtAbsence";
				ds.Tables.Add(tableStruct4);
			}
			if (!ds.Tables.Contains("dtEducation"))
			{
				DataTable tableStruct5 = GetTableStruct("HEM2");
				tableStruct5.TableName = "dtEducation";
				ds.Tables.Add(tableStruct5);
			}
			if (!ds.Tables.Contains("dtReview"))
			{
				DataTable tableStruct6 = GetTableStruct("HEM3");
				tableStruct6.TableName = "dtReview";
				ds.Tables.Add(tableStruct6);
			}
			if (!ds.Tables.Contains("dtPrevEmp"))
			{
				DataTable tableStruct7 = GetTableStruct("HEM4");
				tableStruct7.TableName = "dtPrevEmp";
				ds.Tables.Add(tableStruct7);
			}
			if (!ds.Tables.Contains("dtHealth"))
			{
				DataTable tableStruct8 = GetTableStruct("APZ_OHEH");
				tableStruct8.TableName = "dtHealth";
				ds.Tables.Add(tableStruct8);
			}
			if (!ds.Tables.Contains("dtContract"))
			{
				DataTable tableStruct9 = GetTableStruct("APZ_OHEC");
				tableStruct9.TableName = "dtContract";
				ds.Tables.Add(tableStruct9);
			}
			CreateTempTable(ds.Tables["AttFile"], Temp_Atc);
			BulkCopy(ds.Tables["AttFile"], Temp_Atc);
			CreateTempTable(ds.Tables["dtRole"], Temp_Role);
			BulkCopy(ds.Tables["dtRole"], Temp_Role);
			CreateTempTable(ds.Tables["dtTeams"], Temp_Team);
			BulkCopy(ds.Tables["dtTeams"], Temp_Team);
			CreateTempTable(ds.Tables["dtAbsence"], Temp_Absence);
			BulkCopy(ds.Tables["dtAbsence"], Temp_Absence);
			CreateTempTable(ds.Tables["dtEducation"], Temp_Education);
			BulkCopy(ds.Tables["dtEducation"], Temp_Education);
			CreateTempTable(ds.Tables["dtReview"], Temp_Review);
			BulkCopy(ds.Tables["dtReview"], Temp_Review);
			CreateTempTable(ds.Tables["dtPrevEmp"], Temp_PrevEmp);
			BulkCopy(ds.Tables["dtPrevEmp"], Temp_PrevEmp);
			CreateTempTable(ds.Tables["dtHealth"], Tem_Health);
			BulkCopy(ds.Tables["dtHealth"], Tem_Health);
			CreateTempTable(ds.Tables["dtContract"], Temp_Contract);
			BulkCopy(ds.Tables["dtContract"], Temp_Contract);
			SqlParameter sqlParameter = new SqlParameter("@Typ", value);
			DataTable dataTable = ExecuteDataTable("apz_process_EmpData", CommandType.StoredProcedure, sqlParameter);
			if (dataTable == null || dataTable.Rows.Count <= 0)
			{
				return;
			}
			if (dataTable.Columns.Contains("ErrorMessage") || dataTable.Columns.Contains("ErrorNumber"))
			{
				ret.Message = Function.ToString(dataTable.Rows[0]["ErrorMessage"]);
				string text = Function.ToString(dataTable.Rows[0]["ErrorNumber"]);
				ret.Message = text + " - " + ret.Message;
				objId = "";
				objNum = "";
				return;
			}
			if (dataTable.Columns.Contains("EmpId"))
			{
				ret.Content = dataTable.Rows[0]["EmpId"];
				objId = Function.ToString(ret.Content);
				objNum = Function.ToString(ret.Content);
			}
			ret.Message = "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			ret.Message = ex.Message;
		}
	}

	private void ProcessDelete(ref HttpResult ret, ref string objId, ref string objNum)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@empId", Function.ParseInt(objId));
			DataTable dataTable = ExecuteDataTable("apz_process_Employee_Remove", CommandType.StoredProcedure, sqlParameter);
			if (dataTable == null || dataTable.Rows.Count == 0)
			{
				ret.Message = "";
			}
			else
			{
				ret.Message = Function.ToString(dataTable.Rows[0][0]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			ret.Message = ex.Message;
			ret.Message = "Delete error: " + ex.Message;
		}
	}

	public bool CheckDoc(int empId)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@empId", empId)
			};
			DataTable dataTable = ExecuteDataTable("SELECT empId\r\n                            FROM OHEM \r\n                            WHERE empId = @empId", CommandType.Text, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return true;
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return false;
		}
	}

	public string GetListUserOfEmp(string lstEmp)
	{
		try
		{
			return Function.ToString(ExecuteScalar(GetListUserOfEmpQuery(), CommandType.Text, new SqlParameter("@lstEmp", lstEmp)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return "";
	}

	protected virtual string GetListUserOfEmpQuery()
	{
		return "Select  convert(nvarchar,userId) + ';' FROM OHEM WHERE empId in (select data from split(@lstEmp, ';')) FOR XML PATH('')";
	}

	public DataTable GetOrgChartData(int empID)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@empID", empID)
			};
			DataTable dataTable = ExecuteDataTable("apz_EmpData_GetOrgChartData", CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetDataEmployeeDMS(int pageIndex, string searchKey, int empID, int userId)
	{
		try
		{
			string query = "APZ_D_Employees_SearchData";
			new DataTable();
			List<SqlParameter> list = new List<SqlParameter>();
			SqlParameter item = new SqlParameter("@searchKey", searchKey);
			SqlParameter item2 = new SqlParameter("@lowLimit", 10 * pageIndex - 10);
			SqlParameter item3 = new SqlParameter("@upLimit", pageIndex * 10);
			SqlParameter item4 = new SqlParameter("@empID", empID);
			SqlParameter item5 = new SqlParameter("@userId", userId);
			list.Add(item2);
			list.Add(item3);
			list.Add(item);
			list.Add(item4);
			list.Add(item5);
			return ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return null;
		}
	}

	public DataTable UploadAvatarEmployeeDMS(DataTable param)
	{
		try
		{
			DataTable dataTable = new DataTable();
			string text = Function.ToString(param.Rows[0]["fileName"]);
			int num = Function.ParseInt(param.Rows[0]["empID"]);
			string arg = Function.ToString(ExecuteDataTable("select BitmapPath from OADP", CommandType.Text).Rows[0][0]);
			string value = $"{arg}\\{text}";
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@fileName", text),
				new SqlParameter("@empID", num)
			};
			int num2 = ExecuteNonQuery("Update OHEM set picture=@fileName where empID=@empID", CommandType.Text, list.ToArray());
			dataTable.Columns.Add("Result", typeof(string));
			dataTable.Columns.Add("FilePath", typeof(string));
			DataRow dataRow = dataTable.NewRow();
			dataRow["Result"] = ((num2 > 0) ? "" : "Error");
			dataRow["FilePath"] = value;
			dataTable.Rows.Add(dataRow.ItemArray);
			dataTable.AcceptChanges();
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return null;
		}
	}

	public DataTableResult GetEmployeeInfo(int empId)
	{
		try
		{
			return new DataTableResult(ExecuteDataTable("select * from OHEM where \"empID\" = @empId;", CommandType.Text, new SqlParameter("@empId", empId)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTableResult(ex.Message);
		}
	}
}
