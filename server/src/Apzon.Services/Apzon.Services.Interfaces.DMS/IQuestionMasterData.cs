using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.DMS;

public interface IQuestionMasterData
{
	DataTable LoadData(int id, string fromDate, string toDate, string status);

	DataTable GetDataById(int Id);

	HttpResult Save(DataSet ds, string action);

	string DeleteDocs(string Id);

	int GetNo();

	DataTable FindNextLastFrist(int id, int type);
}
