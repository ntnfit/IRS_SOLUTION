using System;
using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IApprovalStages
{
	Task<DataTable> GetData();

	Task<DataTable> GetDataLines(int stageCode);

	Task<DataTable> SearchData(int stageCode, string stageName);

	int GetNewCode();

	Task<HttpResult> Save(Tuple<DataTable, DataTable, string> tuple);

	Task<DataTable> GetUserInfo(string userName);

	Task<HttpResult> Delete(Tuple<int> tuple);

	DataTable GetRecord(string position, string stageCode);
}
