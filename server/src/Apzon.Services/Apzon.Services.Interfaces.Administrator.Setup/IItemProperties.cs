using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Administrator.Setup;

public interface IItemProperties : IService<DataTable>
{
	DataTable LoadDocItemProperties();

	HttpResult SaveItemProperties(DataTable dtDoc);
}
