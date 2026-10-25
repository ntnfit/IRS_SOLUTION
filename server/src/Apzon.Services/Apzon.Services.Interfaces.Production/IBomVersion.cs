using System.Data;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Production;

public interface IBomVersion
{
	DataTable GetDataDocBomVersion(string code, string version);

	DataTable GetDataLineBomVersion(int id);

	DataTable GetColumnsDocBomVersion();

	DataTable GetColumnsLineBomVersion();

	DataSet GetDataBomSap(string productionNo);

	HttpResult Save(DataSet dtset, Enums.TransactionType tranType, string objId, bool loginSap);

	string GetNewVersionBom(string itemcode);

	DataTable LoadDataNextLast(Enums.NavigateType type, string code);

	bool CheckBomSap(string code);
}
