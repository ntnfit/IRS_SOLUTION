using System;
using System.Data;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.DMS;

namespace Apzon.Services.Repository.DMS;

internal class DMSReportRepository : BaseService<DataSet>, IDMSReport
{
	public DMSReportRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetMarkers()
	{
		return ExecuteDataTable("SELECT * FROM APZ_D_MARKER;", CommandType.Text);
	}

	public HttpResult SavesCoverage()
	{
		try
		{
			DataTable obj = ExecuteDataTable("APZ_D_RPT_SalesCoverage", CommandType.StoredProcedure);
			return new HttpResult(Enums.MessageCode.Success, "", obj);
		}
		catch (Exception ex)
		{
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}
}
