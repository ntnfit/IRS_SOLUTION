using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Administrator.Setup;

public interface IAdditionalGeneralSettings
{
	DataSet GetData();

	HttpResult Save(DataSet ds);

	string GetDefaultLng();
}
