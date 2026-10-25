using System;
using System.Data;
using System.Threading.Tasks;

namespace Apzon.HttpClientHelper;

public interface IListPointKPIsClient
{
	Task<DataTable> GetListPoint(string setCode = "");

	Task<DataTable> GetDetailScoring(int absEntry, string setCode, DateTime period);

	Task<string> SaveListPointKPI(DataTable dt);

	Task<string> SaveDetailScoring(DataTable dt);
}
