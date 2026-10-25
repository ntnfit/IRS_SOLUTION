using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Helper;
using Apzon.Services.Interfaces.Administrator.SystemInitialization;

namespace Apzon.Services.Repository.Administrator.SystemInitialization;

public class MenuStructureRepository : BaseService<IMenuStructure>, IMenuStructure
{
	public MenuStructureRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataSet GetListMenu(string lngCode = "")
	{
		try
		{
			string query = "Apz_MenuStructure_GetListMenu";
			return ExecuteData(query, CommandType.StoredProcedure, new SqlParameter("@lngCode", lngCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetFatherMenu  FAILED ", ex.Message);
			return new DataSet();
		}
	}

	public string AddNewFolder(DataTable dt)
	{
		try
		{
			DataRow dataRow = dt.Rows[0];
			int id = Function.ParseInt(dataRow["Id"]);
			int fatherId = Function.ParseInt(dataRow["FatherId"]);
			string code = Function.ToString(dataRow["Code"]);
			string name = Function.ToString(dataRow["Name"]);
			string language = Function.ToString(dataRow["LanguageKey"]);
			string menuType = Function.ToString(dataRow["MenuType"]);
			string typeCommand = "Add";
			return SaveData(typeCommand, id, fatherId, code, name, language, menuType);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public string RelocateFolder(DataTable dt)
	{
		try
		{
			DataRow dataRow = dt.Rows[0];
			int id = Function.ParseInt(dataRow["Id"]);
			int fatherId = Function.ParseInt(dataRow["FatherId"]);
			string code = Function.ToString(dataRow["Code"]);
			string name = Function.ToString(dataRow["Name"]);
			string language = Function.ToString(dataRow["LanguageKey"]);
			string menuType = Function.ToString(dataRow["MenuType"]);
			string typeCommand = "Relocate";
			return SaveData(typeCommand, id, fatherId, code, name, language, menuType);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public string Rename(DataTable dt)
	{
		try
		{
			DataRow dataRow = dt.Rows[0];
			int id = Function.ParseInt(dataRow["Id"]);
			int fatherId = Function.ParseInt(dataRow["FatherId"]);
			string code = Function.ToString(dataRow["Code"]);
			string name = Function.ToString(dataRow["Name"]);
			string language = Function.ToString(dataRow["LanguageKey"]);
			string menuType = Function.ToString(dataRow["MenuType"]);
			string typeCommand = "Rename";
			return SaveData(typeCommand, id, fatherId, code, name, language, menuType);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public string DeleteFolder(int id)
	{
		try
		{
			string typeCommand = "Delete";
			return SaveData(typeCommand, id);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private string SaveData(string typeCommand, int id = -1, int fatherId = -1, string code = "", string name = "", string language = "", string menuType = "")
	{
		try
		{
			string query = "Apz_MenuStructure_SaveData";
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@id", id),
				new SqlParameter("@fatherId", fatherId),
				new SqlParameter("@code", code),
				new SqlParameter("@name", name),
				new SqlParameter("@language", language),
				new SqlParameter("@menuType", menuType),
				new SqlParameter("@type", typeCommand)
			};
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable.IsNotNull())
			{
				if (Function.ParseInt(dataTable.Rows[0][0]) == 0)
				{
					return "";
				}
				return $"{Function.ParseInt(dataTable.Rows[0][0])}-{Function.ToString(dataTable.Rows[0][1])}";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SaveData  FAILED ", ex.Message);
			return ex.Message;
		}
		return "";
	}

	public DataTable GetListCustomizeMenu()
	{
		try
		{
			string query = "Apz_MenuStructure_GetListCustomizeMenu";
			return ExecuteDataTable(query, CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetFatherMenu  FAILED ", ex.Message);
			return new DataTable();
		}
	}
}
