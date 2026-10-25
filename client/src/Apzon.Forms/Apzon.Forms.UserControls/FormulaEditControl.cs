using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.XtraRichEdit;

namespace Apzon.Forms.UserControls;

[ToolboxItem(true)]
public class FormulaEditControl : RichEditControl, IMessageFilter
{
	public Control AutoCompleteControl { get; set; }

	public FormulaEditControl()
	{
		Application.AddMessageFilter(this);
	}

	public bool PreFilterMessage(ref Message m)
	{
		if (m.Msg == 513 && !ClientRectangle.Contains(PointToClient(Control.MousePosition)) && AutoCompleteControl != null)
		{
			AutoCompleteControl.Visible = false;
		}
		return false;
	}

	protected override void Dispose(bool disposing)
	{
		Application.RemoveMessageFilter(this);
		base.Dispose(disposing);
	}
}
