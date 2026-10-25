using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Administrator.Setup;

namespace Apzon.Services.Repository.Administrator.Setup;

public class ManufacturersRepository : BaseService<DataTable>, IManufacturers, IService<ManufacturersImpl>
{
	public ManufacturersRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public List<ManufacturersImpl> Cfl()
	{
		List<ManufacturersImpl> list = new List<ManufacturersImpl>();
		try
		{
			DataTable dataTable = ExecuteDataTable("SELECT \"FirmCode\" FIRMCODE, \"FirmName\" FIRMNAME FROM OMRC;", CommandType.Text);
			if (dataTable != null)
			{
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					list.Add(new ManufacturersImpl
					{
						Code = Function.ParseInt(dataTable.Rows[i]["FIRMCODE"]),
						ManufacturerName = Function.ToString(dataTable.Rows[i]["FIRMNAME"])
					});
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return list;
	}

	public string Insert(ManufacturersImpl entity)
	{
		throw new NotImplementedException();
	}

	public string Update(ManufacturersImpl entity)
	{
		throw new NotImplementedException();
	}

	public string Remove(ManufacturersImpl entity)
	{
		throw new NotImplementedException();
	}

	public ManufacturersImpl GetById(int id)
	{
		throw new NotImplementedException();
	}

	public DataTable GetManufacturers()
	{
		try
		{
			string query = "SELECT \"FirmCode\",\"FirmName\" FROM OMRC; ";
			return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception)
		{
			return new DataTable();
		}
	}

	public DataTable ShowManufacturers()
	{
		try
		{
			string query = "SELECT * FROM OMRC ;  ";
			return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception)
		{
			return new DataTable();
		}
	}

	public DataTable Search(string firmName)
	{
		DataTable result = new DataTable();
		try
		{
			string text = "SELECT * FROM  OMRC  \r\n                                Where 1=1 ";
			if (!string.IsNullOrEmpty(firmName))
			{
				text = text + " AND \"FirmName\" like N'%" + firmName + "%' ";
			}
			result = ExecuteDataTable(text + ";", CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get data Manufacturers Search - Doc ", ex.Message);
		}
		return result;
	}

	public int GetMaxCode()
	{
		try
		{
			string query = "SELECT ISNULL( MAX(FirmCode ),0) as FirmCode\r\n                            FROM OMRC ";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Convert.ToInt32(dataTable.Rows[0]["FirmCode"]);
			}
			return 0;
		}
		catch (Exception)
		{
			return 0;
		}
	}

	protected virtual string GetMaxCodeQuery()
	{
		return "SELECT ISNULL( MAX(FirmCode ),0) as FirmCode\r\n                            FROM OMRC ;";
	}

	public string DeleteDoc(int FIRMCODE)
	{
		try
		{
			string query = " delete from OMRC where \"FirmCode\" = @firm;\r\n                             UPDATE ONNM SET \"AutoKey\" = (Select MAX(\"FirmCode\") FROM OMRC) +1 WHERE \"ObjectCode\" = '43';";
			if (ExecuteNonQuery(query, CommandType.Text, new SqlParameter("@firm", FIRMCODE)) > 0)
			{
				return null;
			}
			return "ERROR";
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
	}

	public HttpResult SaveDoc(DataTable doc)
	{
		return Process(ApzObjectTypes.oManufacturers, -1, Enums.TransactionType.A, doc);
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataTable doc, ref string objId, ref string objNum)
	{
		HttpResult httpResult = new HttpResult();
		try
		{
			httpResult.Message = ProcessChildData(doc, "APZ_Manufacturer_SaveDoc", "#OMRC");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			httpResult.Message = ex.Message;
		}
		return httpResult;
	}

	private string ProcessChildData(DataTable dt, string store, string tempTable)
	{
		try
		{
			CreateTempTable(dt, tempTable);
			BulkCopy(dt, tempTable);
			DataTable dataTable = ExecuteDataTable(store, CommandType.StoredProcedure);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ToString(dataTable.Rows[0][0]);
			}
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public DataTable ShowCurrency()
	{
		try
		{
			string query = "select \"CurrCode\",\"CurrName\",\"Decimals\", \"RoundSys\"  from ocrn;";
			return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception)
		{
			return new DataTable();
		}
	}

	public DataTable CheckFirmCodeOITM(int firmCode)
	{
		try
		{
			return ExecuteDataTable("SELECT \"ItemCode\" FROM OITM WHERE \"FirmCode\"=@firmCode;", CommandType.Text, new SqlParameter("@firmCode", firmCode));
		}
		catch (Exception)
		{
			return new DataTable();
		}
	}

	public HttpResult Remove(DataTable dt)
	{
		try
		{
			string query = "delete from OMRC where \"FirmCode\" = @firmCode;";
			foreach (DataRow row in dt.Rows)
			{
				if (ExecuteNonQuery(query, CommandType.Text, new SqlParameter("@firmCode", row["firmCode"])) == 0)
				{
					return new HttpResult(Enums.MessageCode.Error, "Delete Error!");
				}
			}
			return new HttpResult();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}
}
