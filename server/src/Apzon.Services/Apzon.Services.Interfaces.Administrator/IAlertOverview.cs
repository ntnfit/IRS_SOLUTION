using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Administrator;

public interface IAlertOverview
{
	DataTable GetAlert(string username, int mailtype);

	DataTable GetAlertUnread(string username, int mailtype);

	DataSet GetAlertData(string code, string userName);

	HttpResult SaveReadMessageChanged(int code, string wasread, string username);

	int CheckCountAlertOverview(string username);
}
