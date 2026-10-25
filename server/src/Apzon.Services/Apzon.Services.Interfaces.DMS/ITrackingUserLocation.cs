using System.Data;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.MobileApp;

namespace Apzon.Services.Interfaces.DMS;

public interface ITrackingUserLocation
{
	DataTable GetLastestAllEmployeeTrackingLocation(string userName);

	HttpResult TrackingUserLocation(DataTable data);

	DataTable GetLastestEmployeeTrackingLocation(int empId);

	DataTable GetLastestUserTrackingLocation(int userId);

	ResponseResult SaveAppSetting(DataTable dt);

	DataTable GetEmployeeRoute(DataTable data);
}
