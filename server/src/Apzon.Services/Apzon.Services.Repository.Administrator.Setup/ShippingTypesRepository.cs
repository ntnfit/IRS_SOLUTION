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

public class ShippingTypesRepository : BaseService<DataTable>, IShippingTypes, IService<ShippingTypesImpl>
{
	public ShippingTypesRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public List<ShippingTypesImpl> Cfl()
	{
		try
		{
			List<ShippingTypesImpl> list = new List<ShippingTypesImpl>();
			DataTable dataTable = ExecuteDataTable("SELECT \"TrnspCode\", \"TrnspName\", \"WebSite\" FROM OSHP;", CommandType.Text);
			if (dataTable != null)
			{
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					list.Add(new ShippingTypesImpl
					{
						Code = Function.ParseInt(dataTable.Rows[i]["TrnspCode"]),
						Name = Function.ToString(dataTable.Rows[i]["TrnspName"]),
						WebSite = Function.ToString(dataTable.Rows[i]["WebSite"])
					});
				}
			}
			return list;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get List ShippingType Search - Doc ", ex.Message);
			return new List<ShippingTypesImpl>();
		}
	}

	public string Insert(ShippingTypesImpl entity)
	{
		throw new NotImplementedException();
	}

	public string Update(ShippingTypesImpl entity)
	{
		throw new NotImplementedException();
	}

	public string Remove(ShippingTypesImpl entity)
	{
		throw new NotImplementedException();
	}

	public ShippingTypesImpl GetById(int id)
	{
		throw new NotImplementedException();
	}

	public DataTable GetShippingTypes()
	{
		try
		{
			string query = "SELECT \"TrnspCode\", \"TrnspName\" FROM OSHP; ";
			return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get data ShippingType Search - Doc ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable ShowShipping()
	{
		try
		{
			string query = ShowShippingQueryString();
			return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "ShowShipping - Doc ", ex.Message);
			return new DataTable();
		}
	}

	protected virtual string ShowShippingQueryString()
	{
		return "SELECT * FROM OSHP";
	}

	public DataTable Search(string name)
	{
		DataTable result = new DataTable();
		try
		{
			string text = "SELECT \"TrnspCode\", \"TrnspName\", IFNULL(\"WebSite\", '') AS \"WebSite\", \"DataSource\", \"UserSign\" FROM OSHP Where 1 =1 ";
			if (!string.IsNullOrEmpty(name))
			{
				text = text + " AND \"TrnspName\" like '%" + name + "%' ";
			}
			result = ExecuteDataTable(text + ";", CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get data ShippingType Search - Doc ", ex.Message);
		}
		return result;
	}

	public int GetMaxCode()
	{
		try
		{
			string query = "SELECT ISNULL( MAX(TrnspCode ),0) as TrnspCode\r\n                                    FROM OSHP ";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Convert.ToInt32(dataTable.Rows[0]["TrnspCode"]);
			}
			return 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetMaxCode - Doc ", ex.Message);
			return 0;
		}
	}

	protected virtual string GetMaxCodeQuery()
	{
		return "SELECT ISNULL( MAX(TrnspCode ),0) as TrnspCode\r\n                                    FROM OSHP; ";
	}

	public bool Check(int idTrnspCode)
	{
		try
		{
			string query = "SELECT   A.\"TrnspCode\"\r\n                                    FROM OSHP A \r\n                                    WHERE A.\"TrnspCode\" = @idTrnspCode ;";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, new SqlParameter("@idTrnspCode", idTrnspCode));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return true;
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Check - Doc ", ex.Message);
			return false;
		}
	}

	public HttpResult SaveDoc(DataTable dt)
	{
		return Process(ApzObjectTypes.oShippingTypes, -1, Enums.TransactionType.U, dt);
	}

	public HttpResult DeleteDoc(int idTrnspCode)
	{
		return Process(ApzObjectTypes.oShippingTypes, -1, Enums.TransactionType.D, null, idTrnspCode.ToString());
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataTable dt, ref string objId, ref string objNum)
	{
		HttpResult ret = new HttpResult();
		switch (transtype)
		{
		case Enums.TransactionType.A:
		case Enums.TransactionType.U:
			ProcessSaveDoc(ref transtype, dt, ref objId, ref objNum, ref ret);
			break;
		case Enums.TransactionType.D:
			ProcessDeleteDoc(ref ret, ref objId, ref objNum);
			break;
		}
		return ret;
	}

	private void ProcessSaveDoc(ref Enums.TransactionType transtype, DataTable document, ref string objId, ref string objNum, ref HttpResult ret)
	{
		try
		{
			string overSizeColumns = GetUnitOfWork().Commons.GetOverSizeColumns(document, "OSHP");
			if (overSizeColumns != "")
			{
				ret.Message = overSizeColumns;
				return;
			}
			CreateTempTable(document, "#dtOSHP");
			BulkCopy(document, "#dtOSHP");
			ExecuteNonQuery("apz_process_ShipType", CommandType.StoredProcedure);
			ret.Message = "";
			objId = ret.Message;
			objNum = objId;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			ret.Message = ex.Message;
		}
	}

	private void ProcessDeleteDoc(ref HttpResult ret, ref string objId, ref string objNum)
	{
		try
		{
			string query = " delete from OSHP where \"TrnspCode\" = @objId; \r\n                             UPDATE ONNM SET \"AutoKey\" = (Select MAX(\"TrnspCode\") FROM OSHP) +1 WHERE \"ObjectCode\" = '49';";
			if (ExecuteNonQuery(query, CommandType.Text, new SqlParameter("@objId", objId)) > 0)
			{
				ret.Message = "";
			}
			else
			{
				ret.Message = " Delete error!";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			ret.Message = ex.Message;
		}
	}

	public DataTable CheckShippingOITM(int trnspCode)
	{
		try
		{
			string query = "SELECT TOP 1 1 FROM OITM WHERE \"ShipType\" = @trnspCode UNION ALL SELECT TOP 1 1 FROM OCRD WHERE \"ShipType\" = @trnspCode;";
			return ExecuteDataTable(query, CommandType.Text, new SqlParameter("@trnspCode", trnspCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}
}
