using System;
using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IWorkPeriodClient
{
	Task<DataTable> FindDataNextLast(string id, int type);

	Task<HttpResult> Add(DataTable dt);

	Task<HttpResult> Update(DataTable dt);

	Task<string> Remove(string Code, int branch);

	Task<DataTable> GetListWorkPeriod();

	Task<DataTable> GetWorkPeriodByCode(string code, int branch);

	Task<DataTable> GetByDate(DateTime fromDate, DateTime toDate, string code);
}
