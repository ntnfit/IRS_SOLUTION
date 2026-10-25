using System;
using System.Collections.Generic;
using System.Data;
using Apzon.Entities.Models.PlanningProduction;

namespace Apzon.DataServices.Interfaces;

public interface IMps : IRepository<Mps>
{
	string UpdateStatus(string id, string status);

	DataTable GetWorkday1(Mps model);

	DataTable GetWorkday2(Mps model);

	DataTable GetMPS1(Mps model);

	DataTable GetMPS2(Mps model);

	DataTable RunMps1(Mps model);

	DataTable RunMps2(Mps model);

	DataTable RunWorkday1(Mps model);

	DataTable RunWorkday2(Mps model);

	string Save(Mps oMps, DataTable dt1, DataTable dt2);

	List<Mps> GetListMps(string mpsCode, int status, DateTime bDate, DateTime eDate);

	string DeleteData(Mps model);

	DataTable GetMps(string mpsCode, int status, DateTime beginDate, DateTime endDate);
}
