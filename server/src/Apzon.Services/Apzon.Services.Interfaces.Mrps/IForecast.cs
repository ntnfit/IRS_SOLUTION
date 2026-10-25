using System;
using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Mrps;

public interface IForecast
{
	DataTable GetData(string code, string name, DateTime? fromdate, DateTime? todate);

	DataTable GetDataRow(int absId);

	DataTable GetForeCastFromOpportunity(int id);

	DataTable GetMaxAbsId();

	DataTable GetMaxLineIdByAbsId(int abs);

	string DeleteData(int abs);

	HttpResult SaveData(DataSet dsDataSet);

	string DeleteDataLine(Tuple<int, string, DateTime> tup);
}
