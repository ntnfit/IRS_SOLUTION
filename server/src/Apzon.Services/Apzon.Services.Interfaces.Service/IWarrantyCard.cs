using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Service;

public interface IWarrantyCard
{
	DataTable LoadData(int id, string warrantyNo, string itemCode, string bpCode, string status);

	DataTable GetDataById(int Id);

	HttpResult Save(DataSet ds, string action);

	string DeleteDocs(string Id);

	int GetMaxId();

	DataTable FindNextLastFrist(int id, int type);
}
