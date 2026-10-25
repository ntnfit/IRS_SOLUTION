using System.Collections.Generic;
using System.Data;

namespace Apzon.Entities.Models.DMS;

public class FinishVisitStoreImpl
{
	public DataTable DataHeader { get; set; }

	public DataTable DataLine { get; set; }

	public DataTable DataUpdateInventory { get; set; }

	public List<CompetitorProductAvailableImpl> DataCompetitor { get; set; }
}
