using System.Data;
using Apzon.Entities.Models.Documents;

namespace Apzon.Api.Controllers.Inventory.InventoryTransaction;

public class DocInput
{
	public DocumentsImpl Doc { get; set; }

	public DataSet DataUdo { get; set; }
}
