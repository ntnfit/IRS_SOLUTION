using System.Data;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Service;

public interface IServiceContractTemplate
{
	DataTable SearchData(DataTable dt);

	DataTable GetDataDoc(int id, Enums.NavigateType type);

	HttpResult SaveData(DataTable dt);

	HttpResult DeleteData(DataTable dt);
}
