using System;
using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.DMS;

public interface IStore
{
	string GetGroupUser(string groupUser);

	DataTable GetStore(string storeCode, string cardCode, string status);

	DataTable GetEvents(int clgCode);

	DataTable GetStoreEvents(string storeCode);

	DataTable GetGeography();

	DataTable GetRecord(string position, string storeCode);

	DataTable GetLastEvent(string storeCode);

	HttpResult Save(Tuple<DataSet, string> tuple);

	HttpResult CheckRemove(string storeCode);

	HttpResult Remove(Tuple<string> tuple);

	DataTable GetParentsGeography(string ggpCode);
}
