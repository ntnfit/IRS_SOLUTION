using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Administrator;

namespace Apzon.Services.Repository.Administrator;

public class AlertOverviewRepository : BaseService<DataTable>, IAlertOverview
{
	private IUnitOfWork unit;

	public AlertOverviewRepository(IDatabaseService databaseService, IUnitOfWork unitOfWork)
		: base(databaseService)
	{
		unit = unitOfWork;
	}

	public DataTable GetAlert(string username, int mailtype)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("apz_alert_getbyuser", CommandType.StoredProcedure, new SqlParameter("@UserName", username), new SqlParameter("@MailType", mailtype));
			dataTable.Columns.Add("RowCount", typeof(int));
			DataTable dataTable2 = dataTable.Clone();
			int num = Function.ParseInt(ExecuteScalar("SELECT USERID FROM OUSR WHERE USER_CODE=@User;", CommandType.Text, new SqlParameter("@User", username)));
			if (dataTable.IsNotNull())
			{
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					string text = Function.ToString(dataTable.Rows[i]["QString"]);
					text = text.Replace("$[USER]", num.ToString());
					if (!string.IsNullOrEmpty(text))
					{
						string query = Function.ParseSqlToCountRowResult(text);
						if (!string.IsNullOrEmpty(text))
						{
							int num2;
							try
							{
								num2 = Function.ParseInt(ExecuteScalar(query, CommandType.Text));
							}
							catch (Exception ex)
							{
								try
								{
									num2 = Function.ParseInt(ExecuteDataTable(text, CommandType.Text)?.Rows.Count);
									Logging.Write("ERROR", ex);
								}
								catch (Exception ex2)
								{
									num2 = 0;
									Logging.Write("ERROR", ex2);
								}
							}
							if (num2 > 0)
							{
								DataRow dataRow = dataTable2.NewRow();
								dataRow.ItemArray = dataTable.Rows[i].ItemArray;
								dataRow["RowCount"] = num2;
								dataTable2.Rows.Add(dataRow);
							}
						}
						else
						{
							Logging.Write("WATCH", Function.ToString(dataTable.Rows[i]["Code"]) + " CountQString is empty " + text);
						}
					}
					else
					{
						Logging.Write("WATCH", Function.ToString(dataTable.Rows[i]["Code"]) + " QString is empty");
					}
				}
			}
			if (dataTable2.Columns.Count != 0)
			{
				DataTable dataTable3 = ExecuteDataTable("SELECT a.\"Code\", a.\"Subject\", d.U_NAME AS \"FromUser\", c.\"ObjType\"\r\n\t                                                FROM OALR a join OAIB b on a.\"Code\"=b.\"AlertCode\"\r\n\t                                                join ALR3 c on a.\"Code\"=c.\"Code\"\r\n\t                                                join OUSR d on d.USERID=a.\"UserSign\"\r\n\t                                                join OUSR e on e.USERID=b.\"UserSign\"\r\n\t                                                where  a.\"Type\"='M' AND b.\"WasRead\"='N'\r\n\t                                                and c.\"ObjType\" IN ('122','112') and e.USER_CODE=@UserCode\r\n\t                                                ORDER BY b.\"RecDate\" DESC;", CommandType.Text, new SqlParameter("@UserCode", username));
				if (dataTable3.IsNotNull())
				{
					DataRow dataRow2 = dataTable2.NewRow();
					dataRow2["Code"] = -1;
					dataRow2["Name"] = Function.ToString(dataTable3.Rows[0]["FromUser"]) + ": " + Function.ToString(dataTable3.Rows[0]["Subject"]);
					dataRow2["WasRead"] = "N";
					dataRow2["RowCount"] = dataTable3.Rows.Count;
					dataTable2.Rows.Add(dataRow2);
				}
			}
			return dataTable2;
		}
		catch (Exception ex3)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex3.Message);
		}
		return new DataTable();
	}

	public DataTable GetAlertUnread(string username, int mailtype)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("apz_alert_getbyuser", CommandType.StoredProcedure, new SqlParameter("@UserName", username), new SqlParameter("@MailType", mailtype));
			dataTable.Columns.Add("RowCount", typeof(int));
			if (dataTable.IsNotNull())
			{
				DataTable dataTable2 = dataTable.Clone();
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					string text = Function.ToString(dataTable.Rows[i]["QString"]);
					if (string.IsNullOrEmpty(text))
					{
						continue;
					}
					string query = Function.ParseSqlToCountRowResult(text);
					if (!string.IsNullOrEmpty(text))
					{
						int num;
						try
						{
							num = Function.ParseInt(ExecuteScalar(query, CommandType.Text));
						}
						catch (Exception ex)
						{
							num = Function.ParseInt(ExecuteDataTable(text, CommandType.Text)?.Rows.Count);
							Logging.Write("ERROR", ex);
						}
						if (num > 0)
						{
							DataRow dataRow = dataTable2.NewRow();
							dataRow.ItemArray = dataTable.Rows[i].ItemArray;
							dataRow["RowCount"] = num;
							dataTable2.Rows.Add(dataRow);
						}
					}
				}
				return dataTable2;
			}
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
		}
		return new DataTable();
	}

	public DataSet GetAlertData(string code, string userName)
	{
		try
		{
			string alertQuery = GetAlertQuery();
			DataTable dataTable = ExecuteDataTable(alertQuery, CommandType.Text, new SqlParameter("@Code", code));
			if (dataTable.IsNotNull())
			{
				string text = Function.ToString(dataTable.Rows[0][0]);
				if (text.ToUpper().Contains("$[USER]"))
				{
					int num = -1;
					DataTable userSign = unit.Users.GetUserSign(userName);
					if (userSign.IsNotNull())
					{
						num = Function.ParseInt(userSign.Rows[0]["USERID"]);
					}
					text = text.Replace("$[USER]", num.ToString()).Replace("$[user]", num.ToString());
				}
				DataTable dataTable2 = ExecuteDataTable(text, CommandType.Text);
				if (dataTable2.IsNotNull())
				{
					DataSet dataSet = new DataSet();
					dataTable2.TableName = "Data";
					dataSet.Tables.Add(dataTable2);
					DataSet dataSet2 = ExecuteData("SELECT T0.*\r\n                                FROM ALR2 T0\r\n                                INNER JOIN OALR T1 ON t0.\"Code\"=T1.\"Code\"\r\n                                INNER JOIN OALT T2 ON T1.\"TCode\"=T2.\"Code\" WHERE T2.\"Code\"=@Code;\r\n\r\n                                SELECT T0.* FROM ALR3 T0\r\n                                INNER JOIN OALR T1 ON T0.\"Code\"=T1.\"Code\"\r\n                                INNER JOIN oalt T2 ON t1.\"TCode\"=t2.\"Code\" WHERE T2.\"Code\"=@Code;", CommandType.Text, new SqlParameter("@Code", code));
					if (dataTable2.IsNotNull())
					{
						foreach (DataTable table in dataSet2.Tables)
						{
							dataSet.Tables.Add(table.Copy());
						}
					}
					return dataSet;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	protected virtual string GetAlertQuery()
	{
		return "select isNULL((SELECT T1.\"QString\" FROM oalt T0 INNER JOIN ouqr t1 ON t0.\"QueryId\" = t1.\"IntrnalKey\" WHERE t0.\"Code\" =@Code ),'')";
	}

	public virtual HttpResult SaveReadMessageChanged(int code, string wasread, string username)
	{
		try
		{
			if (ExecuteNonQuery("IF @Code = -1\r\n                BEGIN\r\n                    UPDATE T0 SET WasRead = 'Y' FROM OAIB T0 JOIN OUSR T1 ON T0.UserSign=T1.USERID JOIN ALR3 T2 ON T2.Code=T0.AlertCode\r\n                    WHERE T1.USER_CODE=@UserName AND T2.ObjType in ('112','122') AND T0.WasRead = 'N'\r\n                END\r\n                ELSE IF EXISTS(SELECT 1 FROM OAIB T0\r\n                INNER JOIN ousr t1 ON t0.UserSign=T1.USERID\r\n                WHERE AlertCode=@Code AND T1.USER_CODE=@UserName)\r\n\r\n                BEGIN\r\n\t                UPDATE T0 \r\n\t                SET WasRead=@WasRead\r\n\t                FROM OAIB T0\r\n\t                INNER JOIN ousr t1 ON t0.UserSign=T1.USERID\r\n\t                WHERE AlertCode=@Code AND T1.USER_CODE=@UserName\r\n                END\r\n\r\n                ELSE\r\n\r\n                BEGIN\r\n\t                DECLARE @RecTime int=CONVERT(int,(SELECT LEFT((SELECT CONVERT(VARCHAR,GETDATE(),108)),2)+SUBSTRING((SELECT CONVERT(VARCHAR,GETDATE(),108)),4,2)))\r\n\t                INSERT INTO dbo.OAIB\r\n\t                        ( AlertCode ,\r\n\t                          UserSign ,\r\n\t                          Opened ,\r\n\t                          RecDate ,\r\n\t                          RecTime ,\r\n\t                          WasRead ,\r\n\t                          Deleted ,\r\n\t                          Failed\r\n\t                        )\r\n\t                SELECT @Code,t1.USERID,'Y',GETDATE(),@RecTime,@WasRead,'N','N'\r\n\t                FROM OUSR T1 WHERE T1.USER_CODE=@UserName\r\n                END\t", CommandType.Text, new SqlParameter("@Code", code), new SqlParameter("@WasRead", wasread), new SqlParameter("@UserName", username)) <= 0)
			{
				return new HttpResult(Enums.MessageCode.Error, "No record effected");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new HttpResult();
	}

	public int CheckCountAlertOverview(string username)
	{
		try
		{
			int num = 0;
			DataTable dataTable = ExecuteDataTable("apz_alert_getbyuser", CommandType.StoredProcedure, new SqlParameter("@UserName", username), new SqlParameter("@MailType", 0.ToString()));
			if (dataTable.IsNotNull())
			{
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					string text = Function.ToString(dataTable.Rows[i]["QString"]);
					if (!string.IsNullOrEmpty(text))
					{
						text = Function.ParseSqlToCountRowResult(text);
						if (!string.IsNullOrEmpty(text) && Function.ParseInt(ExecuteScalar(text, CommandType.Text)) > 0)
						{
							num++;
						}
					}
				}
			}
			return num;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return 0;
	}
}
