using System.Data;
using Apzon.Entities.Models.MobileApp;

namespace Apzon.Services.Interfaces.DMS;

public interface IDMSApproved
{
	DataTable GetList(string userName);

	DataTable GetListV2(string userName, string fromDate, string toDate);

	ResponseResult GetListItem(string objType, string userCode, string fromDate, string toDate, int pageIndex, string typeFilter, string searchText, int wddCode);

	ResponseResult GetDetailItem(string userName, string status, string requestType, string wddCode);

	ResponseResult GetTemplateDetails(int wddCode, int wtmCode);
}
