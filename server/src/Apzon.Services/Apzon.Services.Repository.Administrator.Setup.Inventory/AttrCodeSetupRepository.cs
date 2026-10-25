using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Services.Interfaces.Administrator.Setup.Inventory;

namespace Apzon.Services.Repository.Administrator.Setup.Inventory;

public class AttrCodeSetupRepository : BaseService<IAttrCodeSetup>, IAttrCodeSetup
{
	public AttrCodeSetupRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetAttribute()
	{
		try
		{
			string query = "select \"AbsEntry\", \"FldNum\", \"DispName\" from OBFC where \"FldType\"='A' and \"Activated\"='Y';";
			return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDocs  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetAttributeByFldNum(int fldNum)
	{
		try
		{
			string query = "select a.\"AbsEntry\", a.\"FldAbs\", a.\"AttrValue\", \r\n                                    nullif((select COUNT(1) from OBIN where \"Attr" + fldNum + "Val\" = a.\"AttrValue\"), 0) as \"NoOfBin\"\r\n                            from OBAT a inner join OBFC b on a.\"FldAbs\" = b.\"AbsEntry\" where b.\"FldNum\" = " + fldNum + " order by \"NoOfBin\" desc;";
			return ExecuteDataTable(query, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDocs  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public string CheckExistedInBinLoc(int fldAbs, string attrValue)
	{
		try
		{
			string query = "select top 1 1 from OBIN where \"Attr" + fldAbs + "Val\" = '" + attrValue + "';";
			return Function.ToString(ExecuteScalar(query, CommandType.Text));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Check  FAILED ", ex.Message);
			return ex.Message;
		}
	}
}
