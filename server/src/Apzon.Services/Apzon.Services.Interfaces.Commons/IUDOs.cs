using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Commons;

public interface IUDOs
{
	DataTable GetListUdo();

	DataTable GetUdoByCode(Common.Table_Code item);

	string GetTypeByCode(string code);

	DataTable GetUdo1ByCode(string code);

	int GetMaxId(string code);

	DataSet GetData(DataTable dt);

	HttpResult GetPage(DataTable dt);

	DataTable GetDataByDocEntry(Common.Table_Code item);

	HttpResult Save(DataSet ds);

	HttpResult Remove(DataSet ds);

	HttpResult Cancel(DataSet ds);

	HttpResult Close(DataSet ds);

	DataTable GetSeriesByUser(string udoCode, string userCode);

	string GetNextNumber(string udoCode, int series);
}
