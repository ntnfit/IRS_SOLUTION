using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.MobileApp;
using Apzon.Services.Interfaces.DMS;

namespace Apzon.Services.Repository.DMS;

public class TrackingUserLocationRepository : BaseService<DataTable>, ITrackingUserLocation
{
	private const string tempDoc = "#APZ_D_TKLC";

	public TrackingUserLocationRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetLastestAllEmployeeTrackingLocation(string userName)
	{
		try
		{
			string query = "APZ_D_GetLastestAllEmployeeTrackingLocation";
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter
				{
					ParameterName = "@userName",
					Value = userName
				}
			};
			return ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return null;
		}
	}

	public DataTable GetLastestEmployeeTrackingLocation(int empId)
	{
		try
		{
			string query = "APZ_D_GetLastestEmployeeTrackingLocation";
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter
				{
					ParameterName = "@empId",
					Value = empId
				}
			};
			return ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return null;
		}
	}

	public DataTable GetLastestUserTrackingLocation(int userId)
	{
		try
		{
			string query = "APZ_D_GetLastestUserTrackingLocation";
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter
				{
					ParameterName = "@userId",
					Value = userId
				}
			};
			return ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return null;
		}
	}

	public DataTable GetEmployeeRoute(DataTable data)
	{
		try
		{
			if (!data.IsNotNull())
			{
				return new DataTable();
			}
			DataRow dataRow = data.Rows[0];
			int num = Function.ParseInt(dataRow["empId"]);
			string query = "APZ_D_GetEmployeeRoute";
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter
				{
					ParameterName = "@empId",
					Value = num
				}
			};
			if (data.ExistsColumn("date"))
			{
				object obj = dataRow["date"];
				if (obj != null)
				{
					list.Add(new SqlParameter
					{
						ParameterName = "@date",
						Value = Function.ParseDateTimes(obj)
					});
				}
			}
			return ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return null;
		}
	}

	public HttpResult TrackingUserLocation(DataTable data)
	{
		try
		{
			return Process(ApzObjectTypes.oVisitStore, -1, Enums.TransactionType.U, data);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new HttpResult
			{
				Message = ex.ToString()
			};
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataTable data, ref string objId, ref string objNum)
	{
		HttpResult httpResult = new HttpResult();
		string message = "";
		switch (transtype)
		{
		case Enums.TransactionType.U:
			message = Update(objtype, bplId, ref transtype, data, ref objId, ref objNum).Message;
			break;
		}
		httpResult.Message = message;
		return httpResult;
	}

	public HttpResult Update(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataTable data, ref string objId, ref string objNum)
	{
		try
		{
			objId = "";
			objNum = "";
			HttpResult httpResult = new HttpResult();
			string query = "APZ_D_TrackingUserLocation";
			CreateTempTable(data, "#APZ_D_TKLC");
			BulkCopy(data, "#APZ_D_TKLC");
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure);
			httpResult.Message = Function.ToString(dataTable.Rows[0]["Message"]);
			return httpResult;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public ResponseResult SaveAppSetting(DataTable data)
	{
		try
		{
			DataTable dataTable = new DataTable();
			using (DatabaseService.OpenConnection())
			{
				CreateTempTable(data, "#APZ_D_ODAS");
				BulkCopy(data, "#APZ_D_ODAS");
				string query = "APZ_D_Save_AppSetting";
				dataTable = ExecuteDataTable(query, CommandType.StoredProcedure);
			}
			if (dataTable.IsNotNull() && Function.ToString(dataTable.Rows[0]["Code"]) != "200")
			{
				return new ResponseResult
				{
					Message = Function.ToString(dataTable.Rows[0]["Message"]),
					MessageCode = Function.ToString(dataTable.Rows[0]["Code"])
				};
			}
			return new ResponseResult
			{
				Message = "",
				MessageCode = "200"
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new ResponseResult
			{
				Message = ex.Message,
				MessageCode = "406"
			};
		}
	}
}
