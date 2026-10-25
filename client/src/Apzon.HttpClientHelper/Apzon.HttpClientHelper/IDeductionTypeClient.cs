using System;
using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IDeductionTypeClient
{
	Task<DataTable> GetList();

	Task<DataTable> FindDataNextLast(int id, int type);

	Task<HttpResult> Saves(DataSet ds);

	Task<HttpResult> Update(DataSet ds);

	Task<DataTable> GetDetail(int code);

	Task<string> DeleteDocs(string ids);

	Task<int> GetMaxCode();

	Task<DataTable> GetByDate(DateTime fromDate, DateTime toDate);
}
