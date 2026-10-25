using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.DataServices.Interfaces;

public interface ICustomGroups
{
	bool CheckExists(string cstGroupCode);

	HttpResult Save(DataTable dt, string sessionId);

	HttpResult Remove(DataTable dt, string sessionId);
}
