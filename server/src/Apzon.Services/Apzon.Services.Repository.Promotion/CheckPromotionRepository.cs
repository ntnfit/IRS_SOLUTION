using System;
using System.Data;
using System.Data.SqlClient;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Promotion;

namespace Apzon.Services.Repository.Promotion;

internal class CheckPromotionRepository : BaseService<DataTable>, ICheckPromotion
{
	public CheckPromotionRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public HttpResult Save(DataTable model)
	{
		try
		{
			return Process(ApzObjectTypes.oCheckPromotion, -1, Enums.TransactionType.A, model);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataTable document, ref string objId, ref string objNum)
	{
		if (!document.IsNotNull())
		{
			return new HttpResult(Enums.MessageCode.Error, "Invalid data");
		}
		BulkToTempTable(document, "APZ_D_PMO4", "#APZ_D_PMO4");
		string value = Function.ToString(document.Rows[0]["GUID"]);
		ExecuteNonQuery(GetSaveQuery(), CommandType.Text, new SqlParameter("@GUID", value));
		return new HttpResult();
	}

	protected virtual string GetSaveQuery()
	{
		return "DELETE APZ_D_PMO4 WHERE GUID=@GUID  INSERT INTO APZ_D_PMO4 SELECT * FROM #APZ_D_PMO4";
	}

	public DataTable GetExistDocPromotion(string guid)
	{
		string query = "SELECT * FROM APZ_D_PMO4 WHERE GUID=@GUID;";
		return ExecuteDataTable(query, CommandType.Text, new SqlParameter("@GUID", guid));
	}
}
