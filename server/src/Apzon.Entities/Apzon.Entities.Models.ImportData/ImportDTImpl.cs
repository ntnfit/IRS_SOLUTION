using System.Data;
using Apzon.Commons.Enums;

namespace Apzon.Entities.Models.ImportData;

public class ImportDTImpl
{
	public Enums.ImportObjectEnum ImportObjectCode { get; set; }

	public DataTable DTData { get; set; }
}
