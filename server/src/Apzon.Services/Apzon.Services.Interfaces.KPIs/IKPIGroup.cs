using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.KPIs;

public interface IKPIGroup
{
	DataSet Search(DataTable dt);

	DataTable GetByCode(string code);

	DataSet GetLinesEmployee(int docentry);

	DataSet GetLinesIndicator(int docentry);

	DataTable GetLinesEmployeeByCode(string code);

	DataTable GetLinesIndicatorByCode(string code, string type);

	HttpResult Save(DataSet ds, string action);
}
