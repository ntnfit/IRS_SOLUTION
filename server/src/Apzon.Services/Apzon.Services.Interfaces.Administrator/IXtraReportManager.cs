using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Administrator;

public interface IXtraReportManager
{
	DataSet LoadReport(string userName, int branch);

	DataSet LoadLayout(string userName, int branch);

	HttpResult SaveReport(DataTable dt);

	string GetDataReportLayoutById(int id);

	string RemoveReport(int id);
}
