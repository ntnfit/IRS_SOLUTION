using System;
using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.KPIs;

public interface IListPointKPIs
{
	DataTable GetListPoint(string setCode);

	DataTable GetDetailScoring(int absEntry, string setCode, DateTime period);

	HttpResult SaveData(DataTable data);
}
