using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Service;

public interface IServiceLevelAgreement
{
	DataTable LoadData(int id, string SLANo, string proService);

	DataSet GetDataById(int Id);

	HttpResult Save(DataSet ds, string action);

	DataTable FindNextLastFrist(int id, int type);

	int GetMaxId();

	string DeleteDocs(string Id);
}
