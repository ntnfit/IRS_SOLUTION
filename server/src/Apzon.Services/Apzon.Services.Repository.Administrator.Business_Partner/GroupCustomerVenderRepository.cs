using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Administrator.Business_Partners;

namespace Apzon.Services.Repository.Administrator.Business_Partner;

public class GroupCustomerVenderRepository : BaseService<Tuple<DataTable, string>>, IGroupCustomerVender
{
	public GroupCustomerVenderRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetPriceList()
	{
		try
		{
			return ExecuteDataTable("SELECT \"ListNum\" AS \"Code\",\"ListName\" AS \"Name\" FROM OPLN;", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable LoadDoc(string category)
	{
		try
		{
			string query = "SELECT \"GroupCode\", \"GroupName\", \"GroupType\", \"Locked\", \"DataSource\", \"UserSign\", \"PriceList\", \"DiscRel\" FROM OCRG WHERE \"GroupType\" = @catelogy;";
			return ExecuteDataTable(query, CommandType.Text, new SqlParameter("@catelogy", category));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetMaxCode()
	{
		try
		{
			return ExecuteDataTable("SELECT MAX(\"GroupCode\") FROM OCRG;", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get Routing  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable CheckExistGroupCode(int grCode)
	{
		try
		{
			string query = "SELECT \"GroupCode\" FROM OCRD WHERE \"GroupCode\"=@GroupCode;";
			return ExecuteDataTable(query, CommandType.Text, new SqlParameter("@GroupCode", grCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "CheckExistGroupCode  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public string CheckGroupName(string groupName)
	{
		try
		{
			if (ExecuteDataTable("SELECT \"GroupCode\" FROM OCRG WHERE \"GroupName\" = @name", CommandType.Text, new SqlParameter("@name", groupName)).IsNotNull())
			{
				return "";
			}
			return "ERROR";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "CheckGroupName  FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public HttpResult SaveDoc(Tuple<DataTable, string> tup)
	{
		return Process(ApzObjectTypes.oBusinessPartnerGroups, -1, Enums.TransactionType.A, tup);
	}

	public HttpResult Delete(Tuple<DataTable, string> tup)
	{
		return Process(ApzObjectTypes.oBusinessPartnerGroups, -1, Enums.TransactionType.D, tup);
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, Tuple<DataTable, string> tup, ref string objId, ref string objNum)
	{
		try
		{
			objId = "";
			objNum = "";
			string text = Function.ToString(tup.Item1.Rows[0]["GroupType"]);
			DataTable dataTable = ExecuteDataTable("Select * from OCRG where \"GroupType\" = @groupType", CommandType.Text, new SqlParameter("@groupType", (text == "S") ? "C" : "S"));
			if (dataTable.IsNotNull())
			{
				for (int i = 0; i < tup.Item1.Rows.Count; i++)
				{
					string groupName = Function.ToString(tup.Item1.Rows[i]["GroupName"]);
					if (!string.IsNullOrWhiteSpace(groupName) && dataTable.AsEnumerable().Any((DataRow r) => string.Compare(Function.ToString(r["GroupName"]), groupName, ignoreCase: true) == 0))
					{
						return new HttpResult(Enums.MessageCode.Error, "[OCRG].[GroupName] This entry already exists '" + groupName + "'");
					}
				}
			}
			ExecuteNonQuery("DELETE FROM OCRG WHERE \"GroupType\" = @GType", CommandType.Text, new SqlParameter("@GType", tup.Item2));
			DataTable dataTable2 = ExecuteDataTable("Select \"AutoKey\" From ONNM WHERE \"ObjectCode\" = '10'", CommandType.Text);
			if (dataTable2 == null || dataTable2.Rows.Count <= 0)
			{
				return new HttpResult(Enums.MessageCode.Error, "Cannot get [ONNM].[AutoKey]");
			}
			short? num = Function.ParseShort(dataTable2.Rows[0][0]);
			if (num <= 0)
			{
				return new HttpResult(Enums.MessageCode.Error, "[ONNM].[AutoKey] (" + (num - 1) + "). New key must greater than zero (0)");
			}
			for (int num2 = 0; num2 < tup.Item1.Rows.Count; num2++)
			{
				if (Function.ParseShort(tup.Item1.Rows[num2]["GroupCode"]) == 0)
				{
					tup.Item1.Rows[num2]["GroupCode"] = num;
					num++;
				}
			}
			tup.Item1.AcceptChanges();
			string empty = string.Empty;
			int num3 = 0;
			foreach (DataRow row in tup.Item1.Rows)
			{
				empty = "INSERT INTO OCRG (\"GroupCode\", \"GroupName\", \"GroupType\", \"Locked\", \"DataSource\", \"UserSign\", \"PriceList\", \"DiscRel\")\r\n                                 VALUES (@code, @name, @type, @lock, @source, @user, @price, @disc)";
				int num4 = ExecuteNonQuery(empty, CommandType.Text, new SqlParameter[8]
				{
					new SqlParameter("@code", Function.ParseInt(row["GroupCode"])),
					new SqlParameter("@name", Function.ToString(row["GroupName"])),
					new SqlParameter("@type", Function.ToString(row["GroupType"])),
					new SqlParameter("@lock", Function.ToString(row["Locked"])),
					new SqlParameter("@source", Function.ToString(row["DataSource"])),
					new SqlParameter("@user", Function.ParseInt(row["UserSign"])),
					new SqlParameter("@price", Function.ParseInt(row["PriceList"])),
					new SqlParameter("@disc", Function.ToString(row["DiscRel"]))
				}.ToArray());
				num3 += num4;
			}
			num3 += ExecuteNonQuery("UPDATE ONNM SET \"AutoKey\" = (Select MAX(\"GroupCode\") FROM OCRG) + 1 WHERE \"ObjectCode\" = '10'", CommandType.Text);
			if (num3 == tup.Item1.Rows.Count + 1)
			{
				return new HttpResult();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new HttpResult();
	}

	public HttpResult Delete(DataTable dt)
	{
		string query = "delete from OCRG where \"GroupCode\" = @code;";
		HttpResult httpResult = new HttpResult();
		try
		{
			foreach (DataRow row in dt.Rows)
			{
				if (ExecuteNonQuery(query, CommandType.Text, new SqlParameter("@code", row["GroupCode"])) != 1)
				{
					Logging.Write("ERROR", "Delete Failed");
					httpResult.Message = "Delete Failed";
				}
			}
			return httpResult;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public virtual HttpResult Delete(string code)
	{
		string query = "delete from OCRG where \"GroupCode\" = @code;";
		try
		{
			if (ExecuteNonQuery(query, CommandType.Text, new SqlParameter("@code", code)) == 1)
			{
				return new HttpResult();
			}
			Logging.Write("ERROR", "Delete Failed");
			return new HttpResult(Enums.MessageCode.Error, "Delete Failed");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}
}
