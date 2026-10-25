using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.DMS;

public interface IAlertTemplate
{
	DataTable GetAllTemplate();

	HttpResult SaveData(DataTable dt, string type);

	HttpResult Remove(string code);
}
