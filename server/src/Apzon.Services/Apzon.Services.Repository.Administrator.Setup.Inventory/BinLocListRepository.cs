using System;
using System.Data;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Services.Interfaces.Administrator.Setup.Inventory;

namespace Apzon.Services.Repository.Administrator.Setup.Inventory;

public class BinLocListRepository : BaseService<IBinLocList>, IBinLocList
{
	public BinLocListRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetBinLocListBySLCode(int fldAbs, string slCode)
	{
		try
		{
			string query = "select 'N' \"Select\"\r\n                                    ,a.\"AbsEntry\"\r\n\t\t                            ,a.\"BinCode\"\r\n\t\t                            ,(select SUM(\"OnHandQty\") from OIBQ where \"BinAbs\" = a.\"AbsEntry\") \"ItemQty\"\r\n\t\t                            ,nullif((select COUNT(1) from OIBQ where \"BinAbs\" = a.\"AbsEntry\"), 0) \"NoOfItems\"\r\n\t\t                            ,null \"CCDNo\"\r\n                            from OBIN a where a.\"SL" + fldAbs + "Code\" = '" + slCode + "';";
			return ExecuteDataTable(query, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDocs  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetBinLocListByAttrVal(int fldAbs, string attrVal)
	{
		try
		{
			string query = "select 'N' \"Select\"\r\n                                    ,a.\"AbsEntry\"\r\n\t\t                            ,a.\"BinCode\"\r\n\t\t                            ,(select SUM(\"OnHandQty\") from OIBQ where \"BinAbs\" = a.\"AbsEntry\") \"ItemQty\"\r\n\t\t                            ,nullif((select COUNT(1) from OIBQ where \"BinAbs\" = a.\"AbsEntry\"), 0) \"NoOfItems\"\r\n\t\t                            ,null \"CCDNo\"\r\n                            from OBIN a where a.\"Attr" + fldAbs + "Val\" = '" + attrVal + "';";
			return ExecuteDataTable(query, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDocs  FAILED ", ex.Message);
			return new DataTable();
		}
	}
}
