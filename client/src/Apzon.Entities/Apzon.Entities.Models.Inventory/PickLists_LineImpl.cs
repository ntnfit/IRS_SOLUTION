using System.Collections.Generic;
using Apzon.Entities.Models.Documents;
using Apzon.Entities.Models.UserFields;

namespace Apzon.Entities.Models.Inventory;

public class PickLists_LineImpl
{
	public int AbsEntry { get; set; }

	public int PickEntry { get; set; }

	public int OrderEntry { get; set; }

	public int OrderLine { get; set; }

	public decimal PickQtty { get; set; }

	public string PickStatus { get; set; }

	public decimal RelQtty { get; set; }

	public int LogInsac { get; set; }

	public decimal PrevReleas { get; set; }

	public int BaseObject { get; set; }

	public List<UserFieldsImpl> UserFields { get; set; }

	public List<BatchNumbersImpl> BatchNumbers { get; set; }

	public List<SerialNumbersImpl> SerialNumbers { get; set; }

	public List<DocumentLinesBinAllocationsImpl> BinAllocations { get; set; }
}
