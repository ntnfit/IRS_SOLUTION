using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.DataServices.Interfaces;

public interface IShippingType
{
	HttpResult Save(DataTable dt, string sessionId);

	string Delete(int trnspCode, string sessionId);
}
