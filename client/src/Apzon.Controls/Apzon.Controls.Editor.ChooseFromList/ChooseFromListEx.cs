using System.ComponentModel;
using Apzon.Commons.Enums;
using DevExpress.XtraEditors.Controls;

namespace Apzon.Controls.Editor.ChooseFromList;

[ToolboxItem(true)]
public class ChooseFromListEx : ButtonEditEx
{
	public Enums.TypeCFL TypeCfl;

	public bool MultiSelect;

	public ChooseFromListEx()
	{
		Properties.Buttons[0].Kind = ButtonPredefines.Search;
	}
}
