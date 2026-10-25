using System.Data;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.HumanResources;

public interface IJobVacancy
{
	DataTable GetData(string status, int docNo, string fromdate, string todate);

	DataTable FindDataNextLast(int id, int type);

	HttpResult Save(DataSet ds, string action);

	HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet dsSave, ref string objId, ref string objNum);

	HttpResult SaveData(DataSet ds, Enums.TransactionType tranType, ref string objId, ref string objNum);

	DataTable GetNo();

	DataSet GetDetail(int docId);

	DataTable SetDataLookUp(string db);

	DataTable ProfileMacthUp(string qualification);

	DataTable GetListEmpId();

	DataTable ListBenefits();

	HttpResult DeleteData(ref HttpResult ret, ref string objId, ref string objNum);

	string DeleteDocs(string ids);

	int GetMaxDocNum();

	DataTable GetQuaById(int id);

	DataTable GetEmpById(int id);

	DataTable GetAppById(string id);

	int GetDocMaxEntry();

	int GetNextNumber(int series);
}
