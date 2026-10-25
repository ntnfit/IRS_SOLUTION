using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Commons;

public interface IQueries
{
	DataSet GetListQueries(string userName, string branch, string filter = "");

	string DeleteLine(int cateId);

	string DeleteQuery(string queryName, string catalog);

	DataTable GetParameters(string qname);

	DataTable GetColumns(string qname, CommandType cmdType, DataTable dataTable);

	HttpResult SaveExtractQuery(DataSet ds);

	DataSet LoadParameters(int reportId);

	DataTable ListOfQueryReport(string username);

	DataSet SubmitQueryReport(DataTable dtPrms);

	string RemoveExtractQR(string id);

	DataTable GetQueryById(int queryId);

	DataTable GetAuthGroup();
}
