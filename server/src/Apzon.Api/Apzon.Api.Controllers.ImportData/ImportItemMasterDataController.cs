using System;
using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.ImportData;

public class ImportItemMasterDataController : BaseApiController
{
	[HttpGet]
	public DataTable GetTableColumns(string tableName)
	{
		return UnitOfWork.ImportItemMasterData.GetTableColumns(tableName);
	}

	[HttpGet]
	public DataTable GetColumnNam(string tableName)
	{
		return UnitOfWork.ImportItemMasterData.GetColumnNam(tableName);
	}

	[HttpPost]
	public DataTable CheckStatus([FromBody] Common.Table_Code tb)
	{
		return UnitOfWork.ImportItemMasterData.CheckStatus(tb);
	}

	[HttpPost]
	public string SaveAll([FromBody] Tuple<DataTable, string> tup)
	{
		return UnitOfWork.ImportItemMasterData.SaveAll(tup);
	}
}
