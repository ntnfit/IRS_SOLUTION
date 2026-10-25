using System;
using System.Data;
using Apzon.DataServices.Implements;
using Apzon.Services;

namespace Apzon.DataServices.HanaImplements;

public class HanaPrsRepository : PrsRepository
{
	public HanaPrsRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, dataService)
	{
	}

	public override string GetCodeMax()
	{
		string query = "SELECT (ifnull(max(\"PSCode\"),0) ) \"Id\" FROM apz_OPRS;";
		DataTable dataTable = ExecuteDataTable(query, CommandType.Text);
		int num = ((!string.IsNullOrEmpty(dataTable.Rows[0]["Id"].ToString())) ? int.Parse(dataTable.Rows[0]["Id"].ToString()) : 0);
		return "PS" + DateTime.Today.Year + DateTime.Today.Month + DateTime.Today.Day + (num + 101);
	}
}
