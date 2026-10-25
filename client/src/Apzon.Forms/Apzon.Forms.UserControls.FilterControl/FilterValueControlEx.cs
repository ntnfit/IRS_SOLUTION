using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Forms.Commons.CFL;
using DevExpress.XtraEditors.Drawing;

namespace Apzon.Forms.UserControls.FilterControl;

[ToolboxItem(true)]
public class FilterValueControlEx : FilterValueControl
{
	protected override void OnClickButton(EditorButtonObjectInfoArgs buttonInfo)
	{
		try
		{
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.ChooseFilterSourceValue, multiSelect: true, "");
			frm_ChooseFromList2.FilterSourceValue = SourceFilter;
			frm_ChooseFromList2.ValueFilterSource = Function.ToString(EditValue);
			frm_ChooseFromList2.ShowDialog(this);
			if (frm_ChooseFromList2.ChooseItems != null && frm_ChooseFromList2.ChooseItems.Count > 0)
			{
				EditValue = string.Join(";", frm_ChooseFromList2.ChooseItems.Select((dynamic t) => Function.ToString(t["Code"])));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}
}
