using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.DataServices.Interfaces;

public interface IRoles
{
	HttpResult Save(DataTable dtSource, string sessionId);

	HttpResult Delete(int id, string sessionId);
}
