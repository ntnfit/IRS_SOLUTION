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

public class PrsRepository : Repository<Prs>, IPrs, IRepository<Prs>
{
	public PrsRepository(Apzon.Services.IUnitOfWork unitOfWork, IDataService dataService)
		: base(unitOfWork, dataService)
	{
	}

	public virtual string GetCodeMax()
	{
		string query = "SELECT (isnull(max(id),0) ) as Id FROM apz_OPRS";
		DataTable dataTable = ExecuteDataTable(query, CommandType.Text);
		int num = ((!string.IsNullOrEmpty(dataTable.Rows[0]["Id"].ToString())) ? int.Parse(dataTable.Rows[0]["Id"].ToString()) : 0);
		return "PS" + DateTime.Today.Year + DateTime.Today.Month + DateTime.Today.Day + (num + 101);
	}

	public DataTable GetByCode(string id)
	{
		return ExecuteDataTable("select * from apz_OPRS where \"PSCode\" = @PSCode;", CommandType.Text, new SqlParameter("@PSCode", id));
	}

	public DataTable SP_APZ_ProductionScheduling(string mrp)
	{
		string query = "SP_APZ_ProductionScheduling '" + mrp + "' ";
		return ExecuteDataTable(query, CommandType.Text);
	}

	public DataTable SP_APZ_GetScheduling(string code)
	{
		string query = "SP_APZ_GetScheduling '" + code + "'";
		return ExecuteDataTable(query, CommandType.Text);
	}

	public DataTable SP_APZ_SchedulingCapacity(string mrp)
	{
		string query = "SP_APZ_SchedulingCapacity '" + mrp + "'";
		return ExecuteDataTable(query, CommandType.Text);
	}

	public DataTable GetData_PRS2ByCode_Update(string code)
	{
		string query = "SP_APZ_GetSchedulingCapacity '" + code + "'";
		return ExecuteDataTable(query, CommandType.Text);
	}

	public DataTable GetScheduleGenProduction(string psCode)
	{
		return ExecuteDataTable("select * from apz_PRS1 where PSCode = @PsCode and ISNULL(ProductionOrder, '') <> '' and Type = 2", CommandType.Text, new SqlParameter("@PsCode", psCode));
	}

	public DataTable GetData_OPRSByID(string ID)
	{
		try
		{
			string query = "select * from apz_OPRS where Id = '" + ID + "'";
			return ExecuteDataTable(query, CommandType.Text);
		}
		catch (Exception)
		{
			return null;
		}
	}

	public string GetMaxCode()
	{
		string query = "SELECT PSCode FROM apz_OPRS where ID = (select max(Id) from apz_OPRS)";
		return ExecuteDataTable(query, CommandType.Text).Rows[0]["PSCode"].ToString();
	}

	public string SaveData(Prs objOPRS, List<Prs1> lstPRS1, List<Prs2> lstPRS2)
	{
		try
		{
			string text = "";
			DataTable data_OPRSByID = GetData_OPRSByID(objOPRS.Id.ToString());
			string text2 = null;
			if (data_OPRSByID != null && data_OPRSByID.Rows.Count > 0)
			{
				text += "Update apz_OPRS set ";
				text = text + "                BeginDate   = '" + objOPRS.BeginDate.ToString() + "', ";
				text = text + "                MrpCode     = N'" + objOPRS.MrpCode.Trim() + "', ";
				text = text + "                EndDate     = '" + objOPRS.EndDate.ToString() + "', ";
				text = text + "                Status      = '" + objOPRS.Status + "', ";
				text = text + "                Notes       = N'" + objOPRS.Notes.Trim() + "' ";
				text = text + " Where          Id          = " + objOPRS.Id;
				text += " \n ";
				ExecuteNonQuery(text, CommandType.Text);
				text2 = objOPRS.PSCode;
				if (lstPRS1 != null && lstPRS1.Count > 0)
				{
					int num = 0;
					text = "Delete apz_PRS1 where Type in (2,4) and PSCode ='" + text2 + "' and isnull(ProductionOrder, 0) = 0 \n ";
					for (int i = 0; i < lstPRS1.Count; i++)
					{
						num++;
						if (num == 2500)
						{
							text += " insert into apz_PRS1 with(rowlock) (Item, Type, Workday, Qty, Notes, Shift, PSCode, Line, MachineTime, NumMachine)";
							text = text + " values (N'" + lstPRS1[i].Item + "', '" + lstPRS1[i].Type + "', '" + lstPRS1[i].WorkDay.ToString() + "', '" + lstPRS1[i].Qty + "', N'" + lstPRS1[i].Notes + "', '" + lstPRS1[i].Shift + "', '" + text2 + "', '" + lstPRS1[i].Line + "', " + lstPRS1[i].MachineTime + ", " + lstPRS1[i].NumMachine + ") \n ";
							ExecuteNonQuery(text, CommandType.Text);
							text = null;
							num = 0;
						}
						else
						{
							text += " insert into apz_PRS1 with(rowlock) (Item, Type, Workday, Qty, Notes, Shift, PSCode, Line, MachineTime, NumMachine)";
							text = text + " values (N'" + lstPRS1[i].Item + "', '" + lstPRS1[i].Type + "', '" + lstPRS1[i].WorkDay.ToString() + "', '" + lstPRS1[i].Qty + "', N'" + lstPRS1[i].Notes + "', '" + lstPRS1[i].Shift + "', '" + text2 + "', '" + lstPRS1[i].Line + "', " + lstPRS1[i].MachineTime + ", " + lstPRS1[i].NumMachine + ") \n ";
						}
					}
				}
				if (ExecuteNonQuery(text, CommandType.Text) >= 0)
				{
					text = "Delete apz_PRS1 where Type = 3 and PSCode = '" + text2 + "' \n ";
					text += " insert into apz_PRS1 with(rowlock) (Item, Type, Workday, Qty, Notes, Shift, PSCode, Line, MachineTime, NumMachine) \n ";
					text = text + " select Item, 3, WorkDay, Qty, Notes, Shift, PSCode, Line, MachineTime, NumMachine from apz_PRS1 where Type = 2 and PSCode = '" + text2 + "'";
					ExecuteNonQuery(text, CommandType.Text);
					text = null;
					if (lstPRS2 != null && lstPRS2.Count > 0)
					{
						for (int j = 0; j < lstPRS2.Count; j++)
						{
							text = text + " Update apz_PRS2 set Amount = " + lstPRS2[j].Amount + " where Item = '" + lstPRS2[j].Item + "' and WorkDay ='" + lstPRS2[j].WorkDay.ToString() + "' and Shift = '" + lstPRS2[j].Shift + "' and Type= '" + lstPRS2[j].Type + "' and PSCode ='" + text2 + "' \n ";
						}
						if (ExecuteNonQuery(text, CommandType.Text) <= 0)
						{
							return "Error save data";
						}
					}
					return string.Empty;
				}
				return "Error save data";
			}
			text += " insert into apz_OPRS with(rowlock) (BeginDate, MrpCode, EndDate, Status, CreateDate, Notes, PSCode) ";
			text = text + " values ('" + objOPRS.BeginDate.ToString() + "', N'" + objOPRS.MrpCode + "', '" + objOPRS.EndDate.ToString() + "', '" + objOPRS.Status + "', (select getDate()), N'" + objOPRS.Notes + "', N'" + GetCodeMax() + "') \n ";
			if (ExecuteNonQuery(text, CommandType.Text) >= 0)
			{
				text = string.Empty;
				text2 = GetMaxCode();
				text = string.Empty;
				if (lstPRS1 != null && lstPRS1.Count > 0)
				{
					int num2 = 0;
					for (int k = 0; k < lstPRS1.Count; k++)
					{
						num2++;
						if (num2 == 2500)
						{
							text += " insert into apz_PRS1 with(rowlock) (Item, Type, Workday, Qty, Notes, Shift, PSCode, Line, MachineTime, NumMachine)";
							text = text + " values (N'" + lstPRS1[k].Item + "', '" + lstPRS1[k].Type + "', '" + lstPRS1[k].WorkDay.ToString() + "', '" + lstPRS1[k].Qty + "', N'" + lstPRS1[k].Notes + "', '" + lstPRS1[k].Shift + "', '" + text2 + "', '" + lstPRS1[k].Line + "', " + lstPRS1[k].MachineTime + ", " + lstPRS1[k].NumMachine + ") \n ";
							ExecuteNonQuery(text, CommandType.Text);
							text = null;
							num2 = 0;
						}
						else
						{
							text += " insert into apz_PRS1 with(rowlock) (Item, Type, Workday, Qty, Notes, Shift, PSCode, Line, MachineTime, NumMachine)";
							text = text + " values (N'" + lstPRS1[k].Item + "', '" + lstPRS1[k].Type + "', '" + lstPRS1[k].WorkDay.ToString() + "', '" + lstPRS1[k].Qty + "', N'" + lstPRS1[k].Notes + "', '" + lstPRS1[k].Shift + "', '" + text2 + "', '" + lstPRS1[k].Line + "', " + lstPRS1[k].MachineTime + ", " + lstPRS1[k].NumMachine + ") \n ";
						}
					}
				}
				if (ExecuteNonQuery(text, CommandType.Text) >= 0)
				{
					text = "insert into apz_PRS1 with(rowlock) (Item, Type, Workday, Qty, Notes, Shift, PSCode, Line, MachineTime, NumMachine) \n ";
					text = text + " select Item, 3, WorkDay, Qty, Notes, Shift, PSCode, Line, MachineTime, NumMachine from apz_PRS1 where Type = 2 and PSCode = '" + text2 + "'";
					ExecuteNonQuery(text, CommandType.Text);
					text = null;
					if (lstPRS2 != null && lstPRS2.Count > 0)
					{
						for (int l = 0; l < lstPRS2.Count; l++)
						{
							text += " insert into apz_PRS2 with(rowlock) (Item, Type, Workday, Amount, Notes, Shift, PSCode)";
							text = text + " values (N'" + lstPRS2[l].Item + "', '" + lstPRS2[l].Type + "', '" + lstPRS2[l].WorkDay.ToString() + "', '" + lstPRS2[l].Amount + "', N'" + lstPRS2[l].Notes + "', '" + lstPRS2[l].Shift + "', '" + text2 + "') \n ";
						}
						if (ExecuteNonQuery(text, CommandType.Text) <= 0)
						{
							return "Save data error";
						}
					}
					return null;
				}
				return "Save data error";
			}
			return "Save data error";
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
	}

	public bool CheckPOInShift(string itemCode, DateTime date, string shift, string psCode)
	{
		try
		{
			string query = "select ProductionOrder from apz_PRS1 where Item = '" + itemCode + "' and WorkDay = '" + date.ToString() + "' and Shift = '" + shift + "' and Type = 2 and PSCode = '" + psCode + "' ";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				if (string.IsNullOrEmpty(dataTable.Rows[0]["ProductionOrder"].ToString().Trim()))
				{
					return false;
				}
				return true;
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return false;
		}
	}
}
