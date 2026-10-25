using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Administrator;

namespace Apzon.HttpClientHelper;

public interface IGeneralSetting
{
	Task GetFormatString();

	GeneralSettingImpl GetAllGeneralSetting();

	Task<DataTable> GetLengthUnits();

	Task<DataTable> GetWeightUnits();
}
