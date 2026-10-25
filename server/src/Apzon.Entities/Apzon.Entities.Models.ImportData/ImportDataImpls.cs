using System.Collections.Generic;
using Apzon.Commons.Enums;

namespace Apzon.Entities.Models.ImportData;

public class ImportDataImpls
{
	public Enums.ImportActionEnum ImportAction { get; set; }

	public List<ImportDTImpl> ListDaTaImport { get; set; }

	public int UserID { get; set; }
}
