using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.DataServices.Interfaces;

public interface IInventoryCycles
{
	HttpResult Add(DataTable dtSave, string sessionId);

	HttpResult Update(DataTable dtSave, string sessionId);

	HttpResult Delete(DataTable dtSave, string sessionId);
}
