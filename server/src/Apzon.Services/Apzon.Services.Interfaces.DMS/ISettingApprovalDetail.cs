using System.Data;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.MobileApp;

namespace Apzon.Services.Interfaces.DMS;

public interface ISettingApprovalDetail
{
	DataTable LoadData(int id, string fromDate, string toDate, string report);

	DataTable GetAllLineById(int Id);

	HttpResult Save(DataSet ds, string action);

	string DeleteDocs(string Id);

	ResponseResult GetDataById(int Id);

	DataTable GetDataHeaderById(int Id);

	ResponseResult GetDataSource(string docNo);
}
