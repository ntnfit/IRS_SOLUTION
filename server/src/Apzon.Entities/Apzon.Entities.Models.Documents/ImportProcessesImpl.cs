using System;

namespace Apzon.Entities.Models.Documents;

public class ImportProcessesImpl
{
	public double AdditionalItemDiscountValue { get; set; }

	public string AdditionalNumber { get; set; }

	public DateTime CustomsClearanceDate { get; set; }

	public DateTime DateOfRegistry_DI_DSI_DA { get; set; }

	public string DrawbackRegimeConcessionAccountNumber { get; set; }

	public string ImportationDocumentNumber { get; set; }

	public string ImportationDocumentTypeCode { get; set; }
}
