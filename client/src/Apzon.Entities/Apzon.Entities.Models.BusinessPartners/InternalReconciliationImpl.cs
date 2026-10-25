using System;
using System.Collections.Generic;

namespace Apzon.Entities.Models.BusinessPartners;

public class InternalReconciliationImpl
{
	public int ReconNum { get; set; }

	public DateTime ReconDate { get; set; }

	public string IsCard { get; set; }

	public string ReconType { get; set; }

	public decimal Total { get; set; }

	public List<InternalReconciliationLineImpl> InternalReconciliationRows { get; set; }
}
