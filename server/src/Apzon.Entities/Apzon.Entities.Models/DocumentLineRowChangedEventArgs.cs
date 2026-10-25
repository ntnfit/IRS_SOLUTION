using System.Data;

namespace Apzon.Entities.Models;

public class DocumentLineRowChangedEventArgs
{
	protected DataRowAction action;

	protected DocumentLineRow row;

	public DataRowAction Action => action;

	public DocumentLineRow Row => row;

	public DocumentLineRowChangedEventArgs(DataRowAction action, DocumentLineRow row)
	{
		this.action = action;
		this.row = row;
	}
}
