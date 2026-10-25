using System.Data;
using System.Xml.Linq;
using Apzon.Entities.Models.Common;

namespace Apzon.DataServices.Interfaces;

public interface IUDOs : IRepository<DataTable>
{
	HttpResult Save(DataSet ds, string sessionId);

	XElement GetByParams(string udoCode, string udoName, int docEntry, string sessionId);

	string DeleteDoc(DataSet ds, string sessionId);

	string CancelDoc(DataSet ds, string sessionId);

	string CloseDoc(DataSet ds, string sessionId);

	string MappingXml(DataTable udoTable, DataTable dtFather, DataSet dsChild, string sessionId, string command);

	string GetXmlExecuteSQL(string sql, string sessionId, bool isGetBody = false);

	XElement ExecuteSQL(string sql, string sessionId);
}
