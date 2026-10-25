using System.Data;

namespace Apzon.Entities.Models;

public class DocPaymentRowChangedEventArgs
{
	protected DataRowAction action;

	protected DocPaymentRow row;

	public DataRowAction Action => action;

	public DocPaymentRow Row => row;

	public DocPaymentRowChangedEventArgs(DataRowAction action, DocPaymentRow row)
	{
		this.action = action;
		this.row = row;
	}
}
