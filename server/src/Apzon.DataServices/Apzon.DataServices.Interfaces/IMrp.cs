using System;
using System.Collections.Generic;
using System.Data;
using Apzon.Entities.Models.PlanningProduction;

namespace Apzon.DataServices.Interfaces;

public interface IMrp : IRepository<Mrp>
{
	DataTable SP_APZ_GetMRP(long mrpId);

	string GetItemByMpsCode(string mpsCode);

	DataTable MrpRun(Mrp obj, string items);

	DataTable GetOitmByItemCode(string itemCode);

	DataTable SP_APZ_GetMRPCapactity(long mrpId);

	DataTable SP_APZ_MRPRun_Capacity(string mrpCode, DateTime fromDate, DateTime toDate);

	string UpdateStatus(string id, string status);

	DataTable GetMrpByCode(string mrpCode);

	string Create(Mrp obj, List<Mrp1> lst1, List<Mrp2> lst2);

	string UpdateData(Mrp obj, List<Mrp1> lst1, List<Mrp2> lst2);

	string Create(Mrp obj, List<Mrp2> lst1);

	string UpdateData(Mrp obj, List<Mrp2> lst1);

	int GetMaxId(string tableName, string fieldId);

	string GetCodeMrp();

	DataTable GetMrpHeader();

	DataTable SP_APZ_MRPRun_SAP(string id, DateTime beginDate, DateTime endDate, string mpsCode);

	DataTable usp_BOM_GetChangedItemByCode(string itemCode, DateTime dateChanged, double qty);

	DataTable GetProductionOrder();

	DataTable GetMrp(Mrp mrp);

	string DeleteMrp(Mrp mrp);

	DataTable GetMrpByCode_CFL(string mrpCode);

	DataTable GetData_CFL(string status);
}
