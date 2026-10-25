using System;

namespace Apzon.Entities.Models.Documents;

public class ExportProcessesImpl
{
	public DateTime ExportationDeclarationDate { get; set; }

	public int ExportationDeclarationNumber { get; set; }

	public int ExportationDocumentTypeCode { get; set; }

	public int ExportationNatureCode { get; set; }

	public DateTime ExportationRegistryDate { get; set; }

	public int ExportationRegistryNumber { get; set; }

	public DateTime LadingBillDate { get; set; }

	public string LadingBillNumber { get; set; }

	public int LadingBillTypeCode { get; set; }

	public DateTime MerchandiseLeftCustomsDate { get; set; }
}
