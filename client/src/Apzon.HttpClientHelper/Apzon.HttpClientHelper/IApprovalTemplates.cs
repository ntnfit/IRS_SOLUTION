using System;
using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IApprovalTemplates
{
	Task<DataTable> GetData();

	Task<DataSet> GetDataLines(int templateCode);

	Task<DataTable> SearchData(string templateName, string templateRemarks);

	int GetNewCode();

	Task<HttpResult> Save(Tuple<DataSet, string> tuple);

	Task<DataTable> GetUserInfo(string userName);

	Task<HttpResult> Delete(Tuple<int> tuple);

	DataTable GetRecord(string position, string templateCode);

	Task<DataTable> GetApprovalStageInfo(string templateName);

	int GetQueryId(string queryName);
}
