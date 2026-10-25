using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.KPIs;

public interface IKPIsSetting
{
	DataSetResult GetKPIsSetting();

	DataSetResult GetDetail(int absEntry);

	HttpResult Save(DataSet ds, string action);

	HttpResult SaveCopySetting(DataTable dt);

	DataTable LoadCopySetting(int absEntry);
}
