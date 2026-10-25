using System.Data;

namespace Apzon.Entities.Models;

public class DocumentRowChangedEventArgs
{
	protected DataRowAction action;

	protected DocumentRow row;

	public DataRowAction Action => action;

	public DocumentRow Row => row;

	public DocumentRowChangedEventArgs(DataRowAction action, DocumentRow row)
	{
		this.action = action;
		this.row = row;
	}
}
