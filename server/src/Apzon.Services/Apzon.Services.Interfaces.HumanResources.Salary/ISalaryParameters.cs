using System;
using System.Data;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.HumanResources.Salary;

public interface ISalaryParameters
{
	DataTable GetList();

	HttpResult Save(DataTable dt, string action);

	HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataTable dsSave, ref string objId, ref string objNum);

	HttpResult SaveData(DataTable st, Enums.TransactionType tranType, ref string objId, ref string objNum);

	HttpResult DeleteData(ref HttpResult ret, ref string objId, ref string objNum);

	string DeleteDocs(string ids);

	DataTable GetByDate(DateTime fromDate, DateTime toDate);
}
