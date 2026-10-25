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

public class MrpRepository : Repository<Mrp>, IMrp, IRepository<Mrp>
{
	public MrpRepository(Apzon.Services.IUnitOfWork unitOfWork, IDataService dataService)
		: base(unitOfWork, dataService)
	{
	}

	public DataTable SP_APZ_GetMRP(long mrpId)
	{
		string query = "SP_APZ_GetMRP '" + mrpId + "'";
		return ExecuteDataTable(query, CommandType.Text);
	}

	public string GetItemByMpsCode(string mpsCode)
	{
		string text = string.Empty;
		try
		{
			string query = $"SELECT DISTINCT Item FROM apz_MPS1 WHERE Mpscode = '{mpsCode}'";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					text = ((i != dataTable.Rows.Count - 1) ? (text + dataTable.Rows[i]["Item"]?.ToString() + ",") : (text + dataTable.Rows[i]["Item"].ToString()));
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return text;
	}

	public DataTable MrpRun(Mrp obj, string items)
	{
		DataTable result = new DataTable();
		try
		{
			result = ExecuteDataTable("SP_APZ_MrpRun", CommandType.Text, new SqlParameter("@MpsCode", obj.MpsCode), new SqlParameter("@BeginDate", obj.BeginDate.Value.ToShortDateString()), new SqlParameter("@EndDate", obj.EndDate.Value.ToShortDateString()), new SqlParameter("@Whs", Function.ToString(obj.Whs)), new SqlParameter("@PoEntry", Function.ToString(obj.PoEntry)), new SqlParameter("@fgcode", items));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public DataTable GetOitmByItemCode(string itemCode)
	{
		try
		{
			(new SqlParameter[1])[0] = new SqlParameter("@ItemCode", itemCode);
			return ExecuteDataTable("Select * from OITM where ItemCode = @ItemCode", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable SP_APZ_GetMRPCapactity(long mrpId)
	{
		try
		{
			string query = "SP_APZ_GetMRPCapactity '" + mrpId + "'";
			return ExecuteDataTable(query, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable SP_APZ_MRPRun_Capacity(string mrpCode, DateTime fromDate, DateTime toDate)
	{
		try
		{
			string query = "SP_APZ_MRPRun_Capacity '" + mrpCode + "', '" + fromDate.Date.ToString() + "', '" + toDate.ToString() + "'";
			return ExecuteDataTable(query, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public string UpdateStatus(string id, string status)
	{
		SqlParameter[] parameters = new SqlParameter[2]
		{
			new SqlParameter("@Status", status),
			new SqlParameter("@Id", id)
		};
		string query = "Update apz_OMRP set [Status] =@Status where Id =@Id ";
		if (ExecuteNonQuery(query, CommandType.Text, parameters) <= 0)
		{
			return "No recored affected!";
		}
		return string.Empty;
	}

	public DataTable GetMrpByCode(string mrpCode)
	{
		try
		{
			SqlParameter[] parameters = new SqlParameter[1]
			{
				new SqlParameter("@MrpCode", mrpCode)
			};
			string query = $"SELECT * FROM apz_OMRP WHERE MrpCode = @MrpCode ";
			return ExecuteDataTable(query, CommandType.Text, parameters);
		}
		catch (Exception)
		{
			return new DataTable();
		}
	}

	public string Create(Mrp obj, List<Mrp1> lst1, List<Mrp2> lst2)
	{
		try
		{
			if (lst1 != null && lst1.Count > 0)
			{
				string query = string.Format("INSERT INTO apz_OMRP(MrpCode, MrpName, MpsCode, BeginDate, EndDate, Whs, Notes, Status, CreateDate, FixDate)  VALUES ('{0}', N'{1}' ,'{2}', '{3}', '{4}', '{5}', N'{6}', '{7}', (select GetDate()), '{8}') \n", obj.MrpCode, obj.MrpName, obj.MpsCode, obj.BeginDate.Value.ToString("MM/dd/yyyy"), obj.EndDate.Value.ToString("MM/dd/yyyy"), obj.Whs, obj.Notes, obj.Status, obj.FixDate);
				if (ExecuteNonQuery(query, CommandType.Text) <= 0)
				{
					return "Create not success!";
				}
				int maxId = GetMaxId("apz_OMRP", "Id");
				query = string.Empty;
				int num = 0;
				for (int i = 0; i < lst1.Count; i++)
				{
					num++;
					if (num == 2500)
					{
						query += $"INSERT INTO apz_MRP1(ItemCode, ItemName, Qty, ConfirmQty, DueDate, DocumentType, Notes, OMrpId, Type, OB, Line, LTime, NumMachine, MachineTime)  VALUES (N'{lst1[i].ItemCode}', N'{lst1[i].ItemName}' ,'{lst1[i].Qty}', '{lst1[i].ConfirmQty}', '{lst1[i].DueDate}', N'{lst1[i].DocumentType}', N'{lst1[i].Notes}', '{maxId}', '{lst1[i].Type}', {lst1[i].OB}, '{lst1[i].Line}', {lst1[i].LTime}, {lst1[i].NumMachine}, {lst1[i].MachineTime}) \n ";
						ExecuteNonQuery(query, CommandType.Text);
						query = null;
						num = 0;
					}
					else
					{
						query += $"INSERT INTO apz_MRP1(ItemCode, ItemName, Qty, ConfirmQty, DueDate, DocumentType, Notes, OMrpId, Type, OB, Line, LTime, NumMachine, MachineTime)  VALUES (N'{lst1[i].ItemCode}', N'{lst1[i].ItemName}' ,'{lst1[i].Qty}', '{lst1[i].ConfirmQty}', '{lst1[i].DueDate}', N'{lst1[i].DocumentType}', N'{lst1[i].Notes}', '{maxId}', '{lst1[i].Type}', {lst1[i].OB}, '{lst1[i].Line}', {lst1[i].LTime}, {lst1[i].NumMachine}, {lst1[i].MachineTime}) \n ";
					}
				}
				ExecuteNonQuery(query, CommandType.Text);
				query = null;
				for (int j = 0; j < lst2.Count; j++)
				{
					query += " INSERT INTO apz_MRP2 (MRPID, Item, Type, WorkDay, Qty, Notes, TypeName) ";
					query += "VALUES ( ";
					query = query + "'" + maxId + "'";
					query = query + ", '" + lst2[j].Item + "'";
					query = query + ", '" + lst2[j].Type + "'";
					query = query + ", '" + lst2[j].WorkDay.ToString() + "'";
					query = query + ", '" + lst2[j].Qty + "'";
					query = query + ", N'" + lst2[j].Notes + "' ";
					query = query + ", N'" + lst2[j].TypeName + "' ";
					query += " ) \n ";
				}
				if (ExecuteNonQuery(query, CommandType.Text) < 1)
				{
					ExecuteNonQuery("DELETE apz_OMRP WHERE Id = '" + maxId + "'", CommandType.Text);
					return "Create not success!";
				}
			}
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
		return null;
	}

	public string UpdateData(Mrp obj, List<Mrp1> lst1, List<Mrp2> lst2)
	{
		try
		{
			if (lst1 != null && lst1.Count > 0)
			{
				string text = string.Format("UPDATE apz_OMRP SET MrpCode = '{0}', MrpName = N'{1}', MpsCode = '{2}', BeginDate = '{3}', EndDate = '{4}', Whs = '{5}', Notes = N'{6}', Status = '{7}', FixDate = '{8}'  WHERE Id = '{9}'\n", obj.MrpCode, obj.MrpName, obj.MpsCode, obj.BeginDate.Value.ToString("MM/dd/yyyy"), obj.EndDate.Value.ToString("MM/dd/yyyy"), obj.Whs, obj.Notes, obj.Status, obj.FixDate.Value.ToString("MM/dd/yyyy"), obj.Id);
				text = text + "DELETE apz_MRP1 WHERE OMrpId = " + obj.Id + " \n ";
				text = text + "DELETE apz_MRP2 WHERE MRPID = " + obj.Id + " \n ";
				for (int i = 0; i < lst1.Count; i++)
				{
					text += $"INSERT INTO apz_MRP1(ItemCode, ItemName, Qty, ConfirmQty, DueDate, DocumentType, Notes, OMrpId, Type, OB, Line, LTime, NumMachine, MachineTime) VALUES (N'{lst1[i].ItemCode}', N'{lst1[i].ItemName}' ,'{lst1[i].Qty}', '{lst1[i].ConfirmQty}', '{lst1[i].DueDate}', N'{lst1[i].DocumentType}', N'{lst1[i].Notes}', '{obj.Id}', '{lst1[i].Type}', {lst1[i].OB}, '{lst1[i].Line}', {lst1[i].LTime}, {lst1[i].NumMachine}, {lst1[i].MachineTime}) \n";
				}
				for (int j = 0; j < lst2.Count; j++)
				{
					text += " INSERT INTO apz_MRP2 (MRPID, Item, Type, TypeName, WorkDay, Qty, Notes) ";
					text += "VALUES ( ";
					text = text + "'" + obj.Id + "'";
					text = text + ", '" + lst2[j].Item + "'";
					text = text + ", '" + lst2[j].Type + "'";
					text = text + ", N'" + lst2[j].TypeName + "'";
					text = text + ", '" + lst2[j].WorkDay.ToString() + "'";
					text = text + ", '" + lst2[j].Qty + "'";
					text = text + ", N'" + lst2[j].Notes + "' ";
					text += " ) \n ";
				}
				if (ExecuteNonQuery(text, CommandType.Text) < 0)
				{
					return "Update not success!";
				}
			}
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
		return null;
	}

	public string Create(Mrp obj, List<Mrp2> lst2)
	{
		try
		{
			string query = string.Format("INSERT INTO apz_OMRP(MrpCode, MrpName, MpsCode, BeginDate, EndDate, Whs, Notes, Status, CreateDate, FixDate) VALUES ('{0}', N'{1}' ,'{2}', '{3}', '{4}', '{5}', N'{6}', '{7}', (select GetDate()), '{8}') \n", obj.MrpCode, obj.MrpName, obj.MpsCode, obj.BeginDate.Value.ToString("MM/dd/yyyy"), obj.EndDate.Value.ToString("MM/dd/yyyy"), obj.Whs, obj.Notes, obj.Status, obj.FixDate);
			int num = ExecuteNonQuery(query, CommandType.Text);
			if (num > 0)
			{
				int maxId = GetMaxId("apz_OMRP", "Id");
				query = "insert apz_MRP1(ItemCode, Qty, ConfirmQty, DueDate, DocumentType, OB, Type)\r\n                            select ItmCode, sum(isnull(Amnt,0)), 0, DDate, PrcMethod, max(isnull(OB,0)), Pos from TBL_KMV_MRP \r\n                            group by ItmCode, DDate, PrcMethod, Pos";
				num = ExecuteNonQuery(query, CommandType.Text);
				query = "update apz_MRP1 set \r\n\t                            Line = (select a.U_Line from OITM a where a.ItemCode = [apz_MRP1].ItemCode ),\r\n\t                            LTime = (select isnull(a.LeadTime,0) from OITM a where a.ItemCode = [apz_MRP1].ItemCode ),\r\n\t                            MachineTime = (select isnull(a.U_TimeM,0) from OITM a where a.ItemCode = [apz_MRP1].ItemCode ),\r\n\t                            NumMachine = (select isnull(a.U_NumberM,0) from OITM a where a.ItemCode = [apz_MRP1].ItemCode ),\r\n\t                            OMrpId\t= (select MAX(Id) from apz_OMRP)\r\n                            where OMrpId is null";
				num = ExecuteNonQuery(query, CommandType.Text);
				if (lst2 != null && lst2.Count > 0)
				{
					for (int i = 0; i < lst2.Count; i++)
					{
						query += " INSERT INTO apz_MRP2 (MRPID, Item, Type, WorkDay, Qty, Notes, TypeName) ";
						query += "VALUES ( ";
						query = query + "'" + maxId + "'";
						query = query + ", '" + lst2[i].Item + "'";
						query = query + ", '" + lst2[i].Type + "'";
						query = query + ", '" + lst2[i].WorkDay.ToString() + "'";
						query = query + ", '" + lst2[i].Qty + "'";
						query = query + ", N'" + lst2[i].Notes + "' ";
						query = query + ", N'" + lst2[i].TypeName + "' ";
						query += " ) \n ";
					}
					num = ExecuteNonQuery(query, CommandType.Text);
				}
				if (num < 1)
				{
					query = "DELETE apz_OMRP WHERE Id = '" + maxId + "' \n ";
					query = query + " DELETE apz_MRP1 WHERE OMrpId = " + obj.Id + " \n ";
					ExecuteNonQuery(query, CommandType.Text);
				}
			}
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
		return string.Empty;
	}

	public string UpdateData(Mrp obj, List<Mrp2> lst2)
	{
		try
		{
			string text = string.Format("UPDATE apz_OMRP SET MrpCode = '{0}', MrpName = N'{1}', MpsCode = '{2}', BeginDate = '{3}', EndDate = '{4}', Whs = '{5}', Notes = N'{6}', Status = '{7}', FixDate = '{8}'  WHERE Id = '{9}'\n", obj.MrpCode, obj.MrpName, obj.MpsCode, obj.BeginDate.Value.ToString("MM/dd/yyyy"), obj.EndDate.Value.ToString("MM/dd/yyyy"), obj.Whs, obj.Notes, obj.Status, obj.FixDate.Value.ToString("MM/dd/yyyy"), obj.Id);
			text = text + "DELETE apz_MRP2 WHERE MRPID = " + obj.Id + " \n ";
			if (lst2 != null && lst2.Count > 0)
			{
				for (int i = 0; i < lst2.Count; i++)
				{
					text += " INSERT INTO apz_MRP2 (MRPID, Item, Type, TypeName, WorkDay, Qty, Notes) ";
					text += "VALUES ( ";
					text = text + "'" + obj.Id + "'";
					text = text + ", '" + lst2[i].Item + "'";
					text = text + ", '" + lst2[i].Type + "'";
					text = text + ", N'" + lst2[i].TypeName + "'";
					text = text + ", '" + lst2[i].WorkDay.ToString() + "'";
					text = text + ", '" + lst2[i].Qty + "'";
					text = text + ", N'" + lst2[i].Notes + "' ";
					text += " ) \n ";
				}
			}
			if (ExecuteNonQuery(text, CommandType.Text) < 0)
			{
				return "Update not success!";
			}
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
		return string.Empty;
	}

	public int GetMaxId(string tableName, string fieldId)
	{
		int result = 0;
		string text = null;
		try
		{
			text = "select max(" + fieldId + ") as MaxId from " + tableName;
			DataTable dataTable = ExecuteDataTable(text, CommandType.Text);
			if (dataTable != null)
			{
				string text2 = dataTable.Rows[0]["MaxId"].ToString();
				if (!string.IsNullOrEmpty(text2))
				{
					result = int.Parse(text2);
				}
			}
		}
		catch (Exception)
		{
		}
		return result;
	}

	public string GetCodeMrp()
	{
		DataTable dataTable = new DataTable();
		try
		{
			string query = "SELECT isnull(max(id),0) + 101 as Id FROM  apz_OMRP";
			dataTable = ExecuteDataTable(query, CommandType.Text);
			if (dataTable != null && dataTable.Rows.Count == 1)
			{
				return "MRP" + DateTime.Today.Year + DateTime.Today.Month + DateTime.Today.Day + dataTable.Rows[0]["Id"];
			}
		}
		catch (Exception)
		{
			return string.Empty;
		}
		return string.Empty;
	}

	public DataTable GetMrpHeader()
	{
		try
		{
			string text = "select a.AbsEntry, a.StartDate,MsnCode,Descr, c.FixDate, c.EndDate, 2 as Status, c.MpsCode as Code, '' as Whs ,'' as Notes ";
			text += "      from OMSN a left join OFCT b on a.FCTAbs=b.AbsID ";
			text += "                  left join apz_OMPS c on b.AbsID = c.ForCastID ";
			text += " where a.MsnCode not in(select MpsCode from apz_OMRP) order by a.AbsEntry desc";
			return ExecuteDataTable(text, CommandType.Text);
		}
		catch (Exception)
		{
			return new DataTable();
		}
	}

	public DataTable SP_APZ_MRPRun_SAP(string id, DateTime beginDate, DateTime endDate, string mpsCode)
	{
		try
		{
			string query = "SP_APZ_MRPRun_SAP '" + id + "', '" + beginDate.ToString() + "', '" + endDate.ToString() + "', '" + mpsCode + "'";
			return ExecuteDataTable(query, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable usp_BOM_GetChangedItemByCode(string itemCode, DateTime dateChanged, double qty)
	{
		try
		{
			string query = "usp_BOM_GetChangedItemByCode '" + itemCode + "', '" + dateChanged.ToString() + "', " + qty;
			return ExecuteDataTable(query, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetProductionOrder()
	{
		DataTable result = new DataTable();
		try
		{
			string query = $"SELECT a.*, b.ItemName FROM OWOR a LEFT JOIN OITM b ON a.ItemCode = b.ItemCode WHERE a.Status = 'R' ";
			result = ExecuteDataTable(query, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public DataTable GetMrp(Mrp obj)
	{
		DataTable result = new DataTable();
		try
		{
			string text = $"SELECT *, (case Status when 1 then N'Draft' when 3 then N'Reject' else 'Aprroved' end) as StatusName FROM apz_OMRP WHERE 1 = 1 ";
			if (!string.IsNullOrEmpty(obj.MrpCode))
			{
				text = text + "AND MrpCode = '" + obj.MrpCode + "' ";
			}
			if (!string.IsNullOrEmpty(obj.MrpName))
			{
				text = text + "AND MrpName = '" + obj.MrpName + "' ";
			}
			if (!string.IsNullOrEmpty(obj.MpsCode))
			{
				text = text + "AND MpsCode = '" + obj.MpsCode + "' ";
			}
			if (0 != obj.Status)
			{
				text = text + "AND Status = '" + obj.Status + "' ";
			}
			if (obj.CreateDate.HasValue)
			{
				text = text + "AND CreateDate = '" + obj.CreateDate.Value.ToShortDateString() + "' ";
			}
			if (obj.BeginDate.HasValue)
			{
				text = text + "AND BeginDate = '" + obj.BeginDate.Value.ToShortDateString() + "' ";
			}
			if (obj.EndDate.HasValue)
			{
				text = text + "AND EndDate = '" + obj.EndDate.Value.ToShortDateString() + "' ";
			}
			text += " ORDER BY CreateDate desc ";
			result = ExecuteDataTable(text, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public string DeleteMrp(Mrp mrp)
	{
		try
		{
			string text = "delete apz_OMRP where Id = " + mrp.Id + " \n ";
			text = text + " delete apz_MRP1 where OMrpId = " + mrp.Id + " \n ";
			text = text + " delete apz_MRP2 where MRPID = " + mrp.Id;
			ExecuteNonQuery(text, CommandType.Text);
			return string.Empty;
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
	}

	public DataTable GetMrpByCode_CFL(string mrpCode)
	{
		DataTable result = new DataTable();
		try
		{
			string query = string.Format("SELECT * FROM apz_OMRP WHERE MrpCode like N'%" + mrpCode + "%' ");
			result = ExecuteDataTable(query, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public DataTable GetData_CFL(string status)
	{
		string text = "select MrpCode, MrpName, BeginDate, EndDate, Whs, (case [status] when 1 then 'Draft' when 2 then 'Aprroved' else 'Reject' end) as Status from apz_OMRP ";
		text = text + " where Status in (" + status + ") order by MrpCode desc";
		return ExecuteDataTable(text, CommandType.Text);
	}
}
