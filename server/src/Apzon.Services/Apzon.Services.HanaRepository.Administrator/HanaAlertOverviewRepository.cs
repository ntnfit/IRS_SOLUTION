using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Services.Repository.Administrator;

namespace Apzon.Services.HanaRepository.Administrator;

public class HanaAlertOverviewRepository : AlertOverviewRepository
{
	public HanaAlertOverviewRepository(IDatabaseService databaseService, IUnitOfWork unitOfWork)
		: base(databaseService, unitOfWork)
	{
	}

	public override HttpResult SaveReadMessageChanged(int code, string wasread, string username)
	{
		try
		{
			if (ExecuteNonQuery("apz_alert_SaveReadMessageChanged", CommandType.StoredProcedure, new SqlParameter("@Code", code), new SqlParameter("@WasRead", wasread), new SqlParameter("@UserName", username)) <= 0)
			{
				return new HttpResult(Enums.MessageCode.Error, "No record effected");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new HttpResult();
	}

	protected override string GetAlertQuery()
	{
		return "select ifNULL((SELECT T1.\"QString\" FROM oalt T0 INNER JOIN ouqr t1 ON t0.\"QueryId\" = t1.\"IntrnalKey\" WHERE t0.\"Code\" =@Code ),'') from dummy;";
	}
}
