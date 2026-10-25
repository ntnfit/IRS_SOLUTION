using System.Data;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Commons;

public interface IUDRs
{
	DataTable GetListUDR();

	DataTable GetUDRByCode(Common.Table_Code item);

	DataSetResult GetParamByStore(string storeName, string code, string userName, int branch);

	DataSetResult GetListReport(string userName, int branch);

	HttpResult Save(DataSet ds, string action);

	HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet dsSave, ref string objId, ref string objNum);

	HttpResult SaveData(DataSet ds, Enums.TransactionType tranType, ref string objId, ref string objNum);

	DataTable GetParamsByCode(string code);

	DataTable GetQueryByCode(string code);
}
