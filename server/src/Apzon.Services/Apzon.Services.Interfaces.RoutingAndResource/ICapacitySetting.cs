using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.RoutingAndResource;

public interface ICapacitySetting
{
	DataTable GetColumnSetting();

	DataSet GetDataSetting();

	HttpResult SaveData(DataSet dsSave);
}
