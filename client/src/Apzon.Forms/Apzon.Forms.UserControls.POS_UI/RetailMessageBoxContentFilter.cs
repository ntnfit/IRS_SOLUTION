using System;
using System.Windows.Forms;

namespace Apzon.Forms.UserControls.POS_UI;

public class RetailMessageBoxContentFilter : IMessageFilter
{
	private readonly RetailMessageBoxContent _frm;

	public RetailMessageBoxContentFilter(RetailMessageBoxContent frmMain)
	{
		_frm = frmMain;
	}

	public bool PreFilterMessage(ref Message m)
	{
		try
		{
			if (m.Msg == 256 && ((uint)(int)m.WParam | (uint)Control.ModifierKeys) == 13)
			{
				_frm.EnterKey();
			}
		}
		catch (Exception)
		{
			return false;
		}
		return false;
	}
}
