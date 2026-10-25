using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Services.Repository.Inventory;

namespace Apzon.Services.HanaRepository.Inventory.PriceLists;

public class HanaSTDPriceListsRepository : STDPriceListsRepository
{
	public HanaSTDPriceListsRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public override string RestoreNummeringQuery()
	{
		return "DECLARE maxListNum INT := 0;\r\n                    SELECT IFNULL((SELECT MAX(IFNULL(\"ListNum\", 0)) FROM OPLN), 0) INTO maxListNum FROM DUMMY;\r\n                    SELECT: maxListNum FROM DUMMY;\r\n                    UPDATE ONNM SET \"AutoKey\" = :maxListNum + 1 WHERE \"ObjectCode\" = '6'; ";
	}

	protected override string DetailsLoadIsGrossQuery()
	{
		return "SELECT IFNULL(\"IsGrossPrc\", 'N') FROM OPLN WHERE \"ListNum\" = @listNum;";
	}

	public override DataTable GetPriceForListItem(DataTable lstItemCode, int priceList)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				CreateTempTable(lstItemCode, "#tempItem");
				BulkCopy(lstItemCode, "#tempItem");
				return ExecuteDataTable("APZ_PriceList_GetPriceForListItem", CommandType.StoredProcedure, new SqlParameter("@priceList", priceList));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}
}
