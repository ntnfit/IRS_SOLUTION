using System;
using System.Data;
using System.Data.SqlClient;
using Apzon.Entities.Models.PlanningProduction;
using Apzon.Services.Interfaces.PlanningProduction;

namespace Apzon.Services.Repository.PlanningProduction;

public class MpsRepository : BaseService<Mps>, IMps
{
	public MpsRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public string GetCodeMps()
	{
		string query = "SELECT ISNULL(max(id),0) + 101 as Id FROM  apz_OMPS";
		DataTable dataTable = ExecuteDataTable(query, CommandType.Text);
		if (dataTable == null || dataTable.Rows.Count == 0)
		{
			return "MPS" + DateTime.Today.Year + DateTime.Today.Month + DateTime.Today.Day;
		}
		return "MPS" + DateTime.Today.Year + DateTime.Today.Month + DateTime.Today.Day + dataTable.Rows[0]["Id"];
	}

	public string UpdateApprovalStatus(string id, string status)
	{
		try
		{
			ExecuteNonQuery("Update apz_OMPS set [Status] =@Status where ID = @Id", CommandType.Text, new SqlParameter("@Id", id), new SqlParameter("@Status", status));
			return string.Empty;
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
	}
}
