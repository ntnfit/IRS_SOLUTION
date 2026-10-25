using System;
using System.ComponentModel;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Controls;
using Apzon.Controls.Editor.ChooseFromList;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons.CFL;
using DevExpress.XtraEditors.Controls;

namespace Apzon.Forms.Interfaces.ChooseFromList.RepositoryItem;

public class VirRepChooseFromList : RepChooseFromList
{
	public VirRepChooseFromList()
	{
		ButtonClick += VirRepChooseFromList_ButtonClick;
	}

	public virtual void VirRepChooseFromList_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			string txtText = string.Empty;
			if (GridView != null)
			{
				txtText = Function.ToString(GridView.GetFocusedRowCellValue(FieldName));
			}
			else if (TreeList != null)
			{
				txtText = Function.ToString(TreeList.FocusedNode.GetValue(FieldName));
			}
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(TypeCfl, MultiSelect, txtText);
			frm_ChooseFromList2.Closing += FrmChooseFromList_Closing;
			frm_ChooseFromList2.ShowDialog();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public virtual void FrmChooseFromList_Closing(object sender, CancelEventArgs e)
	{
		try
		{
			if (sender is frm_ChooseFromList frm)
			{
				BindData(frm);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public virtual void BindData(frm_ChooseFromList frm)
	{
		try
		{
			Common.ListItems listItems = (Common.ListItems)frm.ChooseItem;
			if (GridView != null)
			{
				GridView.SetFocusedRowCellValue(FieldName, listItems.Value);
			}
			else if (TreeList != null)
			{
				TreeList.FocusedNode.SetValue(FieldName, listItems.Value);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}
}
