using System;
using System.Data;

namespace Apzon.Services.Interfaces.FacebookCRM;

public interface IFacebookCRM
{
	DataTable LoadMessages(string pageId);

	DataTable LoadMessagesForCustomer(string pageId, string customerId);

	DataTable LoadCustomers(string pageId);

	DataTable LoadPageInfo(string pageId);

	DataTable LoadPageInfoFromUser(string userId);

	DataTable LoadOrderAndItemCode();

	int GetDocNumByDocEntry(int docEntry);

	string SaveUser(DataTable dt);

	string SavePageList(DataTable dt);

	string SaveMessages(DataTable dt);

	string SaveCustomers(DataSet dt);

	string ActiveMessages(DataTable dt);

	string ChangePageStatus(string pageId, DateTime lastLog);
}
