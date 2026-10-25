using System;
using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.ImportData;

public class ImportBOMController : BaseApiController
{
	[HttpGet]
	public DataTable GetTableColumns(string tableName)
	{
		return UnitOfWork.ImportBom.GetTableColumns(tableName);
	}

	[HttpGet]
	public DataTable GetColumnNam(string tableName)
	{
		return UnitOfWork.ImportBom.GetColumnNam(tableName);
	}

	[HttpPost]
	public DataTable CheckStatus(Tuple<string, string, int> tup)
	{
		return UnitOfWork.ImportBom.CheckStatus(tup);
	}

	[HttpPost]
	public DataTable CheckStatus2(Tuple<string, string> tup)
	{
		return UnitOfWork.ImportBom.CheckStatus2(tup);
	}

	[HttpPost]
	public string SaveAll([FromBody] Tuple<DataTable, string> tup)
	{
		return UnitOfWork.ImportBom.SaveAll(tup);
	}
}
