using System;
using System.Data;

namespace Apzon.Services.Interfaces.ProductionAddon;

public interface IMasterOrderReplan
{
	DataTable Search(string moNo, DateTime fromDate, DateTime toDate);

	string Save(DataTable dt);
}
