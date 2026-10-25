using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Financials;

public interface ICurrencies
{
	DataTable GetData();

	HttpResult Save(DataTable dtObj);

	string ExistInDocument(string curr);

	HttpResult Remove(string currCode);
}
