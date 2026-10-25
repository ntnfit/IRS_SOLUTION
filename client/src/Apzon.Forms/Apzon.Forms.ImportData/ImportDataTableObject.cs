using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Forms.ImportData;

public class ImportDataTableObject
{
	public DataTable ImportData { get; set; }

	public DataTable MappingData { get; set; }

	public ImportObjectImpls ImportObject { get; set; }

	public DataTable TableFields { get; set; }

	public ImportDataTableObject(DataTable importData, DataTable mappingData, ImportObjectImpls importObject)
	{
		ImportData = importData;
		MappingData = mappingData;
		ImportObject = importObject;
	}
}
