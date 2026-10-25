using System;
using System.Collections.Generic;
using System.Data;
using Apzon.Entities.Models.PlanningProduction;

namespace Apzon.DataServices.Interfaces;

public interface IPrs : IRepository<Prs>
{
	string GetCodeMax();

	DataTable GetByCode(string id);

	DataTable SP_APZ_ProductionScheduling(string mrp);

	DataTable SP_APZ_GetScheduling(string code);

	DataTable SP_APZ_SchedulingCapacity(string mrp);

	DataTable GetData_PRS2ByCode_Update(string code);

	DataTable GetScheduleGenProduction(string psCode);

	string SaveData(Prs objOPRS, List<Prs1> lstPRS1, List<Prs2> lstPRS2);

	bool CheckPOInShift(string itemCode, DateTime date, string shift, string psCode);
}
