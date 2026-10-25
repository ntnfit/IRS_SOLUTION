using System;
using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IWorkingProcessClient
{
	DataSet GetWorkingProcessSetting(int userId);

	DataSet GetSetting(int userId);

	Task<HttpResult> SaveWorkingProcessSetting(Tuple<DataSet> tupData);

	DataTable GetTemplateData(int userId, string status, string requestType);

	Task<DataTable> GetTemplateDetails(int wddCode);

	DataTable GetTooltipData(int wddCode, int wtmCode, int docEntry, int userId);

	DataTable GetTileViewSetting(int userId);

	bool CheckDocumentCompleteApproval(int wddCode);

	DataTable FilterMenu(Tuple<DataSet> tupFilter);

	HttpResult SaveMenu(Tuple<int, DataTable> tup);

	Task<bool> AllowLastGroupUserCreatedDoc(int wddCode, int userId);

	Task<bool> IsUserApproved(int wddCode, int userId);
}
