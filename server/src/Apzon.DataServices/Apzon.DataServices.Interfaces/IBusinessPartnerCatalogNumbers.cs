using System.Data;
using Apzon.Commons;
using Apzon.Entities.Models.Common;

namespace Apzon.DataServices.Interfaces;

public interface IBusinessPartnerCatalogNumbers
{
	HttpResult Save(DataTable model, ApzObjectTypes objtype, string sessionId);

	string Remove(string code, string subtitle, string item, ApzObjectTypes objtype, string sessionId);

	string CheckExist(string code, string subtitle, string item, ApzObjectTypes objtype, string sessionId);

	string RemoveItem(DataTable dt, ApzObjectTypes objtype, string sessionId);
}
