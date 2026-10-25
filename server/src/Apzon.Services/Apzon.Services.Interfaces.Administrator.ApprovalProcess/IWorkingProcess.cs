using System;
using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Administrator.ApprovalProcess;

public interface IWorkingProcess
{
	DataSet GetWorkingProcessSettingData(int userId);

	DataSet GetSetting(int userId);

	HttpResult SaveWorkingProcessSetting(Tuple<DataSet> tupData);

	DataTable GetTemplateData(int userId, string status, string requestType);

	DataTable GetTemplateDetails(int wddCode);

	DataTable GetTooltipData(int wddCode, int wtmCode, int docEntry, int userId);

	DataTable GetTileViewSetting(int userId);

	bool CheckDocumentCompleteApproval(int wddCode);

	DataTable FilterMenu(Tuple<DataSet> tupFilter);

	HttpResult SaveMenu(Tuple<int, DataTable> tup);

	bool AllowLastGroupUserCreatedDoc(int wddCode, int userId);

	bool IsUserApproved(int wddCode, int userId);
}
