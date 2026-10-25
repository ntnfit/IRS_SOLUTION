using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.DataServices.Interfaces;

public interface IItemGroups
{
	HttpResult Save(DataSet ds, string sessionId);

	string Delete(int code, string sessionId);
}
