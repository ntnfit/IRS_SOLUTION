using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.DataServices.Interfaces;
using Apzon.Entities.Models.PlanningProduction;
using Apzon.Services;

namespace Apzon.DataServices.Implements;

public class MpsRepository : Repository<Mps>, IMps, IRepository<Mps>
{
	public MpsRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, dataService)
	{
	}

	public string UpdateStatus(string id, string status)
	{
		return "No matching record found: id=" + id;
	}

	public DataTable GetWorkday1(Mps model)
	{
		DataTable dataTable = new DataTable();
		dataTable.Columns.Add("WorkDay");
		try
		{
			string empty = string.Empty;
			empty = "select C.WorkDay from (SELECT distinct convert(varchar(10), A.WorkDay, 101) as WorkDay, A.WorkDay as DDate FROM apz_MPS1 A ";
			empty += " LEFT JOIN apz_OMPS B ON A.MpsCode = B.MpsCode ";
			if (model.EndDate.HasValue)
			{
				empty = empty + " WHERE 1= 1 AND A.WorkDay is not NUll  AND WorkDay <= '" + model.EndDate.Value.Date.ToString() + "'  ";
			}
			if (model != null && !string.IsNullOrEmpty(model.MpsCode))
			{
				empty = empty + "AND A.MpsCode =  '" + model.MpsCode + "' ";
			}
			empty += " ) C order by C.DDate";
			dataTable = ExecuteDataTable(empty, CommandType.Text);
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex.Message);
			return dataTable;
		}
	}

	public DataTable GetWorkday2(Mps model)
	{
		DataTable dataTable = new DataTable();
		dataTable.Columns.Add("WorkDay");
		try
		{
			string text = "select C.WorkDay from (SELECT distinct convert(varchar(10), A.WorkDay, 101) as WorkDay, A.WorkDay as DDate FROM apz_MPS2 A ";
			text += " LEFT JOIN apz_OMPS B ON A.MpsCode = B.MpsCode ";
			text += " WHERE 1= 1 AND A.WorkDay is not NUll ";
			if (model != null && !string.IsNullOrEmpty(model.MpsCode))
			{
				text = text + "AND A.MpsCode =  '" + model.MpsCode + "' ";
			}
			text += " ) C order by C.DDate";
			dataTable = ExecuteDataTable(text, CommandType.Text);
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex.Message);
			return dataTable;
		}
	}

	public DataTable GetMPS1(Mps model)
	{
		DataTable result = new DataTable();
		try
		{
			result = ExecuteDataTable("SP_APZ_GETMPS1 " + model.MpsCode, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex.Message);
		}
		return result;
	}

	public DataTable GetMPS2(Mps model)
	{
		DataTable result = new DataTable();
		try
		{
			result = ExecuteDataTable("SP_APZ_GETMPS2 " + model.MpsCode, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex.Message);
		}
		return result;
	}

	public DataTable RunMps1(Mps model)
	{
		DataTable result = new DataTable();
		try
		{
			result = ExecuteDataTable("SP_APZ_RUNMPS1", CommandType.StoredProcedure, new SqlParameter("@MpsCode", model.MpsCode), new SqlParameter("@Begindate", model.BeginDate), new SqlParameter("@Enddate", model.EndDate), new SqlParameter("@ForecastID", Function.ParseInt(model.ForCastID)));
			return result;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return result;
		}
	}

	public DataTable RunMps2(Mps model)
	{
		DataTable result = new DataTable();
		try
		{
			result = ExecuteDataTable("SP_APZ_RUNMPS2", CommandType.StoredProcedure, new SqlParameter("@MpsCode", model.MpsCode), new SqlParameter("@Begindate", model.BeginDate), new SqlParameter("@Enddate", model.EndDate));
			return result;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return result;
		}
	}

	public DataTable RunWorkday1(Mps model)
	{
		DataTable dataTable = new DataTable();
		dataTable.Columns.Add("WorkDay");
		try
		{
			string empty = string.Empty;
			empty = "select * from (SELECT distinct convert(varchar(10), A.WorkDay, 101) as WorkDay, A.WorkDay as DDate FROM apz_TMP_MPS1 A ";
			empty = empty + " WHERE A.WorkDay is not NUll AND WorkDay <= '" + model.EndDate.Value.Date.ToString() + "' ";
			if (model != null && !string.IsNullOrEmpty(model.MpsCode))
			{
				empty = empty + "AND A.MpsCode =  '" + model.MpsCode + "' ";
			}
			empty += " ) b ";
			empty += " order by b.DDate ";
			dataTable = ExecuteDataTable(empty, CommandType.Text);
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return dataTable;
		}
	}

	public DataTable RunWorkday2(Mps model)
	{
		DataTable dataTable = new DataTable();
		dataTable.Columns.Add("WorkDay");
		try
		{
			string empty = string.Empty;
			empty = "select * from (SELECT distinct convert(varchar(10), A.WorkDay, 101) as WorkDay, A.WorkDay as DDate FROM apz_TMP_MPS2 A ";
			empty += " WHERE 1= 1 AND A.WorkDay is not NUll ";
			if (model != null && !string.IsNullOrEmpty(model.MpsCode))
			{
				empty = empty + "AND A.MpsCode =  '" + model.MpsCode + "' ";
			}
			empty += " ) b ";
			empty += " order by b.DDate ";
			dataTable = ExecuteDataTable(empty, CommandType.Text);
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return dataTable;
		}
	}

	public string Save(Mps oMps, DataTable dtMps1, DataTable dtMps2)
	{
		try
		{
			string empty = string.Empty;
			int num = 0;
			if (oMps.Id == 0L)
			{
				empty = "INSERT INTO apz_OMPS(MpsCode,CreateDate,BeginDate,EndDate,Status,Notes, ForCastID, FixDate) ";
				empty += "VALUES ( ";
				empty = empty + "'" + oMps.MpsCode + "'";
				DateTime minValue = DateTime.MinValue;
				DateTime? createDate = oMps.CreateDate;
				if (minValue != createDate)
				{
					empty = empty + ", '" + oMps.CreateDate.Value.ToShortDateString() + "'";
				}
				minValue = DateTime.MinValue;
				createDate = oMps.BeginDate;
				if (minValue != createDate)
				{
					empty = empty + ", '" + oMps.BeginDate.Value.ToShortDateString() + "'";
				}
				minValue = DateTime.MinValue;
				createDate = oMps.EndDate;
				if (minValue != createDate)
				{
					empty = empty + ", '" + oMps.EndDate.Value.ToShortDateString() + "'";
				}
				empty = empty + ", '" + oMps.Status + "'";
				empty = empty + ", '" + oMps.Notes + "'";
				empty = empty + ", '" + oMps.ForCastID + "'";
				empty = empty + ", '" + oMps.FixDate.ToString() + "'";
				empty += " )";
			}
			else
			{
				empty = "UPDATE apz_OMPS ";
				empty += "SET ";
				empty = empty + "MpsCode = N'" + oMps.MpsCode + "', ";
				DateTime minValue = DateTime.MinValue;
				DateTime? createDate = oMps.CreateDate;
				if (minValue != createDate)
				{
					empty = empty + "CreateDate = N'" + oMps.CreateDate.Value.ToShortDateString() + "', ";
				}
				minValue = DateTime.MinValue;
				createDate = oMps.BeginDate;
				if (minValue != createDate)
				{
					empty = empty + "BeginDate = N'" + oMps.BeginDate.Value.ToShortDateString() + "', ";
				}
				minValue = DateTime.MinValue;
				createDate = oMps.EndDate;
				if (minValue != createDate)
				{
					empty = empty + "EndDate = N'" + oMps.EndDate.Value.ToShortDateString() + "', ";
				}
				minValue = DateTime.MinValue;
				createDate = oMps.FixDate;
				if (minValue != createDate)
				{
					empty = empty + "FixDate = N'" + oMps.FixDate.Value.ToShortDateString() + "', ";
				}
				empty = empty + "Status = N'" + oMps.Status + "', ";
				empty = empty + "Notes = N'" + oMps.Notes + "', ";
				empty = empty + "ForCastID = N'" + oMps.ForCastID + "' ";
				empty = empty + "WHERE Id = " + oMps.Id;
			}
			ExecuteNonQuery(empty, CommandType.Text);
			string text = null;
			if (oMps.Id == 0L)
			{
				string query = "select MpsCode from apz_OMPS where ID = (Select max(ID) from apz_OMPS)";
				DataTable dataTable = ExecuteDataTable(query, CommandType.Text);
				text = ((dataTable == null || dataTable.Rows.Count <= 0) ? "" : dataTable.Rows[0]["MpsCode"].ToString());
				empty = " INSERT apz_MPS1 (MpsCode,Item, Type,WorkDay,Qty, OpenBalance, Notes) \n ";
				empty = empty + " select '" + text + "', ItmCode, Pos, DDate, isnull(Amnt,0), isnull(OB,0), ''  from TBL_KMV_MPS where Pos in (1,2,3,6)";
				num = ExecuteNonQuery(empty, CommandType.Text);
				empty = "update apz_MPS1 set \r\n\t                            Line        = (select a.U_Line from OITM a where a.ItemCode = [apz_MPS1].Item ),\r\n\t                            MachineTime = (select isnull(a.U_TimeM,0) from OITM a where a.ItemCode = [apz_MPS1].Item ),\r\n                                NumMachine  = (select isnull(a.U_NumberM,0) from OITM a where a.ItemCode = [apz_MPS1].Item )\r\n                            where MpsCode = '" + text + "'";
				num = ExecuteNonQuery(empty, CommandType.Text);
			}
			else
			{
				text = oMps.MpsCode;
			}
			empty = string.Empty;
			if (dtMps1 != null && dtMps1.Rows.Count > 0)
			{
				empty = " Delete apz_MPS1 where MpsCode = '" + text + "' and [Type] in (4,5) \n ";
				dtMps1.BeginLoadData();
				for (int i = 0; i < dtMps1.Rows.Count; i++)
				{
					DataRow dataRow = dtMps1.Rows[i];
					empty += "INSERT INTO apz_MPS1 (MpsCode,Item, Type,WorkDay,Qty, OpenBalance, Notes, Line, NumMachine, MachineTime) ";
					empty += "VALUES ( ";
					empty = empty + "'" + dataRow["MpsCode"]?.ToString() + "'";
					empty = empty + ", '" + dataRow["Item"]?.ToString() + "'";
					empty = empty + ", '" + dataRow["Type"]?.ToString() + "'";
					empty = empty + ", '" + dataRow["WorkDay"]?.ToString() + "'";
					empty = empty + ", " + ((dataRow["Qty"] == null || string.IsNullOrEmpty(dataRow["Qty"].ToString())) ? "0" : dataRow["Qty"]);
					empty = empty + ", " + ((dataRow["OpenBalance"] == null || string.IsNullOrEmpty(dataRow["OpenBalance"].ToString())) ? "0" : dataRow["OpenBalance"]);
					empty = empty + ", '" + dataRow["Notes"]?.ToString() + "'";
					empty = empty + ", '" + dataRow["Line"]?.ToString() + "'";
					empty = empty + ", '" + ((dataRow["NumMachine"] == null || string.IsNullOrEmpty(dataRow["NumMachine"].ToString())) ? "0" : dataRow["NumMachine"])?.ToString() + "'";
					empty = empty + ", '" + ((dataRow["MachineTime"] == null || string.IsNullOrEmpty(dataRow["MachineTime"].ToString())) ? "0" : dataRow["MachineTime"])?.ToString() + "'";
					empty += " ) \n ";
					if (dataRow["Type"].ToString().Trim().Equals("5"))
					{
						empty = empty + " Update TBL_KMV_MPS set Amnt = " + ((dataRow["Qty"] == null || string.IsNullOrEmpty(dataRow["Qty"].ToString())) ? "0" : dataRow["Qty"])?.ToString() + " where ItmCode = '" + dataRow["Item"]?.ToString() + "' and DDate = '" + dataRow["WorkDay"]?.ToString() + "' and Pos = " + dataRow["Type"]?.ToString() + " \n ";
					}
				}
				dtMps1.EndLoadData();
				num = ExecuteNonQuery(empty, CommandType.Text);
			}
			if (dtMps2 != null && dtMps2.Rows.Count > 0)
			{
				empty = "delete apz_MPS2 where MpsCode = '" + text + "' \n ";
				dtMps2.BeginLoadData();
				for (int j = 0; j < dtMps2.Rows.Count; j++)
				{
					DataRow dataRow2 = dtMps2.Rows[j];
					empty += "INSERT INTO apz_MPS2 (MpsCode, Item, Type,WorkDay,Qty, Notes) ";
					empty += "VALUES ( ";
					empty = empty + "'" + dataRow2["MpsCode"]?.ToString() + "'";
					empty = empty + ", '" + dataRow2["Item"]?.ToString() + "'";
					empty = empty + ", '" + dataRow2["Type"]?.ToString() + "'";
					empty = empty + ", '" + dataRow2["WorkDay"]?.ToString() + "'";
					empty = empty + ", '" + ((dataRow2["Qty"] == null || string.IsNullOrEmpty(dataRow2["Qty"].ToString())) ? "0" : dataRow2["Qty"])?.ToString() + "'";
					empty = empty + ", '" + dataRow2["Notes"]?.ToString() + "'";
					empty += " ) \n ";
				}
				dtMps2.EndLoadData();
				num = ExecuteNonQuery(empty, CommandType.Text);
			}
			if (num > 0)
			{
				return null;
			}
			return "Error update";
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
	}

	public List<Mps> GetListMps(string mpsCode, int status, DateTime bDate, DateTime eDate)
	{
		return new List<Mps>();
	}

	public string DeleteData(Mps model)
	{
		try
		{
			return "No matching record found!";
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
	}

	public DataTable GetMps(string mpsCode, int status, DateTime beginDate, DateTime endDate)
	{
		DataTable result = new DataTable();
		try
		{
			string text = "SELECT Id, MpsCode,CreateDate, BeginDate, EndDate, Status, Notes ";
			text += "FROM apz_OMPS WHERE 1 = 1 ";
			if (!string.IsNullOrEmpty(mpsCode))
			{
				text = text + "AND MpsCode Like N'%" + mpsCode + "%' ";
			}
			if (DateTime.MinValue != beginDate.Date && DateTime.MinValue != endDate.Date)
			{
				text = text + " AND  '" + beginDate.ToShortDateString() + "' <= CreateDate AND CreateDate <= '" + endDate.ToShortDateString() + "' ";
			}
			if (status != 0)
			{
				text = text + "AND Status in ('" + status + "') ";
			}
			text += " Order by CreateDate desc, MpsCode desc ";
			result = ExecuteDataTable(text, CommandType.Text);
			return result;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return result;
		}
	}
}
