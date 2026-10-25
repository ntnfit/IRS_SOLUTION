using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.DataServices.Interfaces;

public interface IGroupCustomerVender
{
	HttpResult Save(DataTable item, string sessionId, string type);

	HttpResult Remove(DataTable dt, string sessionId);
}
