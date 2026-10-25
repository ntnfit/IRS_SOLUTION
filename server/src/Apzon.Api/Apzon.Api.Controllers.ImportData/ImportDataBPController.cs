using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.ImportData;

[ValidateLoginSapExceptionHandling]
public class ImportDataBPController : BaseApiController
{
	[HttpGet]
	public DataTable GetTableColumns(string tableName)
	{
		return UnitOfWork.ImportDataBP.GetTableColumns(tableName);
	}

	[HttpPost]
	public DataTable CheckData([FromBody] Tuple<string, DataTable, List<Tuple<string, Type>>> tupleCheck)
	{
		return UnitOfWork.ImportDataBP.CheckData(tupleCheck);
	}

	[HttpPost]
	public string SaveData([FromBody] Tuple<string, DataTable, List<Tuple<string, Type>>> tupleSave)
	{
		return UnitOfWork.ImportDataBP.SaveData(tupleSave);
	}
}
