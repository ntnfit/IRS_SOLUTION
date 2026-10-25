using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.KPIs;

public interface ISetIndicatorKPIs
{
	DataTable LoadData(string code, string name);

	DataTable GetDataById(string code);

	HttpResult Save(DataSet ds, string action);

	string DeleteDocs(string code);
}
