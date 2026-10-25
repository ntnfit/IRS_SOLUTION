using System;
using System.Data;
using Apzon.Commons;
using Apzon.Services.Repository.Inventory.ItemManagement;

namespace Apzon.Services.SqlRepository.Inventory.ItemManagement;

public class SqlBusinessPartnerCatalogNumbersRespository : BusinessPartnerCatalogNumbersRespository
{
	public SqlBusinessPartnerCatalogNumbersRespository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public override DataTable GetExisted(DataTable model, ApzObjectTypes fatherObjectTypes)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				CreateTempTable(model, "#tbl");
				BulkCopy(model, "#tbl");
				return ExecuteDataTable("SELECT T0.*,T1.Substitute OldSub FROM #tbl T0 LEFT JOIN OSCN T1 ON T0.CardCode=T1.CardCode AND T0.ItemCode=T1.ItemCode AND T0.Substitute=T1.Substitute", CommandType.Text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		return new DataTable();
	}
}
