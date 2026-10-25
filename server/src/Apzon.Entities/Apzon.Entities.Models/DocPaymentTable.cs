using System.Data;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;

namespace Apzon.Entities.Models;

public class DocPaymentTable : Table<DocPayment>
{
	public event DocPaymentRowChangedDlgt DocPaymentRowChanged;

	protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
	{
		return new DocPaymentRow(builder);
	}

	protected override void OnRowChanged(DataRowChangeEventArgs e)
	{
		base.OnRowChanged(e);
		DocPaymentRowChangedEventArgs args = new DocPaymentRowChangedEventArgs(e.Action, (DocPaymentRow)e.Row);
		OnDocPaymentRowChanged(args);
	}

	protected virtual void OnDocPaymentRowChanged(DocPaymentRowChangedEventArgs args)
	{
		if (DocPaymentRowChanged != null)
		{
			DocPaymentRowChanged(this, args);
		}
	}
}
