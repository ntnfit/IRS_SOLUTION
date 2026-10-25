using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.DataServices.Interfaces;

public interface ICurrencies
{
	HttpResult Save(string sessionId, DataTable dt);

	HttpResult Remove(string currCode, string sessionId);
}
