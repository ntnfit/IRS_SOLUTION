using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface ICapacitySettingClient
{
	DataTable GetColumnSetting();

	DataSet GetDataSetting();

	HttpResult SaveData(DataSet dsSave);
}
