using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.PM;

public interface IWBSTemplate
{
	DataTable LoadData(string code, string fromDate, string toDate);

	DataSet GetDataById(string code);

	HttpResult Save(DataSet ds, string action);

	string DeleteDocs(string code);

	DataTable FindNextLastFrist(int id, int type);
}
