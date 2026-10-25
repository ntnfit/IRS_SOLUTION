using System.Data;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Production;

namespace Apzon.Services.Interfaces.Production;

public interface IMaterialShortageReport
{
	DataTable Run(MaterialShortageReport_Impl input);

	DataSet SearchData(Enums.NavigateType type, int code);

	HttpResult GetNewId();

	DataSet GetListMaterialShortageReport(int bplId, string type, string status);

	DataTable LoadAtpType();

	DataTable GetDocStructure();

	HttpResult Save(DataSet ds, Enums.TransactionType tranType, string code);

	DataTable GetStructureGenPurchase();

	DataTable GetDataGenPurchase(string input, int code, string user);

	string CheckInputData(string type, int code, string genType);
}
