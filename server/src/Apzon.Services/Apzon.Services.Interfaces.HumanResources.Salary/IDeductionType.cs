using System;
using System.Data;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.HumanResources.Salary;

public interface IDeductionType
{
	DataTable GetList();

	DataTable FindDataNextLast(int id, int type);

	DataTable GetDetail(int code);

	int GetMaxCode();

	DataTable GetByDate(DateTime fromDate, DateTime toDate);

	HttpResult Save(DataSet ds, string action);

	HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet dsSave, ref string objId, ref string objNum);

	HttpResult SaveData(DataSet ds, Enums.TransactionType tranType, ref string objId, ref string objNum);

	HttpResult DeleteData(ref HttpResult ret, ref string objId, ref string objNum);

	string DeleteDocs(string ids);
}
