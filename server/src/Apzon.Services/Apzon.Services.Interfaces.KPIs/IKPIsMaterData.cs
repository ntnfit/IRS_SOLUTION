using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.KPIs;

public interface IKPIsMaterData
{
	DataTable LoadData(string code, string name, string status);

	HttpResult Saves(DataTable dt);

	string DeleteDocs(string code);

	DataTable LoadDataFormulaCode();
}
