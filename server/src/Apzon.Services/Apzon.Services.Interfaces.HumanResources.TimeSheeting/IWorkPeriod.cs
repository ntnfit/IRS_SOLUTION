using System;
using System.Data;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.HumanResources.TimeSheeting;

public interface IWorkPeriod
{
	string Remove(string Code, int branch);

	HttpResult Save(DataTable dt, string action);

	HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataTable dtSave, ref string objId, ref string objNum);

	HttpResult SaveData(DataTable dt, Enums.TransactionType tranType, ref string objId, ref string objNum);

	DataTable FindDataNextLast(string id, int type);

	DataTable GetByDate(DateTime fromDate, DateTime toDate, string code);

	DataTable GetListWorkPeriod();

	DataTable GetWorkPeriodByCode(string code, int branch);
}
