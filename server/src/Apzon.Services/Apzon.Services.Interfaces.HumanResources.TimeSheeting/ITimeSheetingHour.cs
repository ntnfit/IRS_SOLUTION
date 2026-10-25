using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.HumanResources.TimeSheeting;

public interface ITimeSheetingHour
{
	DataTable LoadTimeSheetingHour(int branch, string dept, int empID, string empName, string status, string period, string workcode, int baseEntry);

	DataSet LoadTimeSheetingHourCopyFrom(int baseEntry, string workcode);

	DataTable LoadTimeSheetingHourSetting(string userSign);

	HttpResult SaveTimeSheetingHour(DataTable dt);

	string SaveTimeSheetingHourSetting(DataTable dt);
}
