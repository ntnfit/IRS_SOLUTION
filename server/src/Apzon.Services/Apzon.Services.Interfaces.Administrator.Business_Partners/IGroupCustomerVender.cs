using System;
using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Administrator.Business_Partners;

public interface IGroupCustomerVender
{
	DataTable GetPriceList();

	DataTable LoadDoc(string catelogy);

	HttpResult SaveDoc(Tuple<DataTable, string> tup);

	DataTable GetMaxCode();

	DataTable CheckExistGroupCode(int grCode);

	string CheckGroupName(string groupName);

	HttpResult Delete(string code);

	HttpResult Delete(DataTable dt);
}
