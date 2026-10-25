using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Entities.Models.Administrator;
using Apzon.Services.Interfaces.Administrator.Setup;

namespace Apzon.Services.Repository.Administrator.Setup;

public class HolidayRepository : BaseService<HolidayImpl>, IHoliday, IService<HolidayImpl>
{
	public HolidayRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable ShowHoliday()
	{
		try
		{
			string query = "select \"HldCode\" from OHLD;";
			new DataTable();
			return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "ShowHoliday  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public string SaveDoc(HolidayImpl doc)
	{
		string text = string.Empty;
		List<SqlParameter> list = new List<SqlParameter>();
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				bool flag = CheckHldCode(doc.HldCode);
				string empty = string.Empty;
				if (!flag)
				{
					empty = "INSERT OHLD(\"HldCode\",\"WndFrm\",\"WndTo\",\"isCurYear\",\"ignrWnd\",\"WeekNoRule\")\r\n                            VALUES \r\n                                        ( ";
					empty += " @HldCode ";
					empty += ", @WndFrm ";
					empty += ", @WndTo ";
					empty += ", @isCurYear ";
					empty += ", @ignrWnd ";
					empty += ", @WeekNoRule ";
					empty += " );";
				}
				else
				{
					list = new List<SqlParameter>();
					empty = "UPDATE OHLD  \r\n                            SET ";
					empty += "\"WndFrm\"= @WndFrm ";
					empty += ",\"WndTo\"= @WndTo ";
					empty += ",\"isCurYear\"= @isCurYear ";
					empty += ",\"ignrWnd\"= @ignrWnd ";
					empty += ",\"WeekNoRule\"= @WeekNoRule ";
					empty += " WHERE \"HldCode\" =@HldCode;";
				}
				list.Add(new SqlParameter("@WndFrm", Function.ToString(doc.WndFrm)));
				list.Add(new SqlParameter("@WndTo", Function.ToString(doc.WndTo)));
				list.Add(new SqlParameter("@isCurYear", Function.ToString(doc.isCurYear)));
				list.Add(new SqlParameter("@ignrWnd", Function.ToString(doc.ignrWnd)));
				list.Add(new SqlParameter("@WeekNoRule", Function.ToString(doc.WeekNoRule)));
				list.Add(new SqlParameter("@HldCode", Function.ToString(doc.HldCode)));
				empty = "delete from HLD1 where \"HldCode\"=@HldCode;" + empty;
				if (ExecuteNonQuery(empty, CommandType.Text, list.ToArray()) > 0 && doc.Lines != null)
				{
					foreach (Holiday_LineImpl line in doc.Lines)
					{
						string text2 = SaveLine(line);
						if (!string.IsNullOrEmpty(text2))
						{
							text = text2;
							break;
						}
					}
				}
				if (string.IsNullOrEmpty(text))
				{
					dbTransaction.Commit();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SAVE Line  FAILED ", ex.Message);
			text = ex.Message;
		}
		return text;
	}

	public DataTable GetMaxCode()
	{
		DataTable dataTable = new DataTable();
		try
		{
			string query = "SELECT ISNULL( MAX(\"HldCode\"),0) as Code FROM HLD1;";
			dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetMaxCode FAILED ", ex.Message);
			return dataTable;
		}
	}

	public bool CheckHldCode(string hldCode)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@HldCode", hldCode));
			string query = "SELECT   A.HldCode \r\n                            FROM OHLD A \r\n                            WHERE A.HldCode = @HldCode";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return true;
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "CheckHldCode FAILED ", ex.Message);
			return false;
		}
	}

	public string DeleteDoc(string hldCode)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				List<SqlParameter> list = new List<SqlParameter>();
				list.Add(new SqlParameter("@HldCode", hldCode));
				string query = "delete HLD1 where HldCode = @HldCode;delete OHLD where HldCode = @HldCode";
				if (ExecuteNonQuery(query, CommandType.Text, list.ToArray()) > 0)
				{
					dbTransaction.Commit();
					return null;
				}
				return "ERROR";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public DataTable Search()
	{
		DataTable result = new DataTable();
		try
		{
			string query = "select * from OHLD;";
			result = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Search Holiday setup ", ex.Message);
		}
		return result;
	}

	public DataTable SearchLine(string hldCode)
	{
		DataTable result = new DataTable();
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@HldCode", hldCode));
			string query = "select * from HLD1 where \"HldCode\"=@HldCode;";
			result = ExecuteDataTable(query, CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SearchLine Holiday setup  ", ex.Message);
		}
		return result;
	}

	public string SaveLine(Holiday_LineImpl line)
	{
		string result = string.Empty;
		List<SqlParameter> list = new List<SqlParameter>();
		try
		{
			string empty = string.Empty;
			empty = "INSERT INTO HLD1  ( HldCode,StrDate,EndDate,Rmrks)\r\n                                                     VALUES \r\n                                        ( ";
			empty += " @HldCode ";
			empty += ", @StrDate ";
			empty += ", @EndDate ";
			empty += ", @Rmrks ";
			empty += " )";
			list.Add(new SqlParameter("@HldCode", Function.ToString(line.HldCode)));
			list.Add(new SqlParameter("@StrDate", Function.ParseDateTime(line.StrDate)));
			list.Add(new SqlParameter("@EndDate", Function.ParseDateTime(line.EndDate)));
			list.Add(new SqlParameter("@Rmrks", Function.ToString(line.Rmrks)));
			if (ExecuteNonQuery(empty, CommandType.Text, list.ToArray()) <= 0)
			{
				result = "ERROR";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SAVE Line Holiday  FAILED ", ex.Message);
			result = ex.Message;
		}
		return result;
	}

	public string Insert(HolidayImpl entity)
	{
		throw new NotImplementedException();
	}

	public string Update(HolidayImpl entity)
	{
		throw new NotImplementedException();
	}

	public string Remove(HolidayImpl entity)
	{
		throw new NotImplementedException();
	}

	public HolidayImpl GetById(int id)
	{
		throw new NotImplementedException();
	}

	public DataTable GetHoliday(HolidayImpl _docSearch)
	{
		DataTable result = new DataTable();
		try
		{
			string text = "select a.\"HldCode\",\r\n                                    case when a.\"WndFrm\" = '0' then 'Sunday'\r\n\t                                     when a.\"WndFrm\" = '1' then 'Monday'\r\n\t                                    when a.\"WndFrm\" = '2' then 'Tuesday'\r\n\t                                    when a.\"WndFrm\" = '3' then 'Wednesday'\r\n\t                                    when a.\"WndFrm\" = '4' then 'Thursday'\r\n\t                                    when a.\"WndFrm\" = '5' then 'Friday'\r\n\t                                    when a.\"WndFrm\" = '6' then 'Saturday'\r\n                                    end WndFrm,\r\n                                    case when a.\"WndTo\" = '0' then 'Sunday'\r\n\t                                     when a.\"WndTo\" = '1' then 'Monday'\r\n\t                                    when a.\"WndTo\" = '2' then 'Tuesday'\r\n\t                                    when a.\"WndTo\" = '3' then 'Wednesday'\r\n\t                                    when a.\"WndTo\" = '4' then 'Thursday'\r\n\t                                    when a.\"WndTo\" = '5' then 'Friday'\r\n\t                                    when a.\"WndTo\" = '6' then 'Saturday'\r\n                                    end WndTo,\r\n                                    a.\"isCurYear\",a.\"ignrWnd\" ,\"WeekNoRule\"\r\n                             from OHLD a                                             \r\n                                Where 1=1";
			if (!string.IsNullOrEmpty(_docSearch.HldCode))
			{
				text = text + " AND a.\"HldCode\" like '%" + _docSearch.HldCode + "%' ";
			}
			if (!string.IsNullOrEmpty(_docSearch.WeekNoRule))
			{
				text = text + " AND a.\"WeekNoRule\" ='" + _docSearch.WeekNoRule + "';";
			}
			result = ExecuteData(text, CommandType.Text, (SqlParameter[])null).Tables[0];
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get Holiday ", ex.Message);
		}
		return result;
	}

	protected virtual string GetMaxCodeQuery()
	{
		return "SELECT ISNULL(MAX(HLDCODE),0) as Code FROM HLD1";
	}
}
