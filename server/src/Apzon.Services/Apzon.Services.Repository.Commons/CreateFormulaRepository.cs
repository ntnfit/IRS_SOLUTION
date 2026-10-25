using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Apzon.Services.Interfaces.Commons;

namespace Apzon.Services.Repository.Commons;

public class CreateFormulaRepository : BaseService<DataTable>, ICreateFormula
{
	public CreateFormulaRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetListForm()
	{
		using (DatabaseService.OpenConnection())
		{
			using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
			try
			{
				new DataTable();
				string query = "SELECT convert(nvarchar,ROW_NUMBER() OVER( ORDER BY FMenuName )) AS ID\r\n                                    ,menucode\r\n\t                                ,FMenuName\r\n\t                                ,MenuName \r\n                                   FROM OMNI WHERE  Enbled=N'Y'";
				DataTable result = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
				dbTransaction.Rollback();
				return result;
			}
			catch (Exception)
			{
				dbTransaction.Rollback();
				return new DataTable();
			}
		}
	}
}
