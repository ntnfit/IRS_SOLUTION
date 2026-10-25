using System.Data;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Sales;

public interface IAtp
{
	DataSet Search(DataTable dtSearch);

	DataTable GetDataAtpType();

	HttpResult GetNewIdAtp();

	DataTable GetStructureInput();

	DataTable GetDocStructure();

	DataTable RunAtp(DataSet ds);

	HttpResult Save(DataSet ds, Enums.TransactionType tranType, string code);

	DataSet SearchData(Enums.NavigateType type, int code);

	DataTable GetDataGenPurchase(string input, int code, string user);

	DataTable GetStructureGenPurchase();

	HttpResult GetNewIdScheduleForAtp();

	DataSet GetDataAtpForSchedle(int code);

	DataSet RunSchedule(string code, string type);

	DataSet GetDataLineSchedule(int id);

	DataTable GetDocScheduleStructure();

	HttpResult SaveSchedule(DataSet dtSet, Enums.TransactionType tranType, string code);

	DataTable GetDataDocSchedule(int code, Enums.NavigateType type);

	DataSet GetDataStructureGenProduction();

	DataTable GetDataGenProduction(string id, string type);

	string CheckInputData(string type, int code, string genType);
}
