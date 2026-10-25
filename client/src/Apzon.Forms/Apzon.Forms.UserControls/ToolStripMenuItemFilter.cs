using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Controls;
using Apzon.Controls.Pivot;
using Apzon.Forms.Mrps;

namespace Apzon.Forms.UserControls;

[ToolboxItem(true)]
public class ToolStripMenuItemFilter : ToolStripMenuItem
{
	private IContainer components;

	public ApzObjectTypes objType { get; set; }

	public PivotGridControlEx pivod { get; set; }

	public DataTable DataSource { get; set; }

	public ToolStripMenuItemFilter()
	{
		Text = "Filter Setting";
		InitializeComponent();
	}

	public ToolStripMenuItemFilter(IContainer container)
	{
		Text = "Filter Setting";
		container.Add(this);
		InitializeComponent();
	}

	public void FilterSettingClick()
	{
		try
		{
			DataTable dataTable = null;
			if (pivod != null)
			{
				dataTable = pivod.DataSource as DataTable;
			}
			else
			{
				if (!DataSource.HasColumn())
				{
					WinCommons.Error(Enums.MessageCode.DatasourceIsNull, "");
					return;
				}
				dataTable = DataSource.Copy();
			}
			if (dataTable.HasColumn())
			{
				frm_FormFilterSetting frm_FormFilterSetting2 = new frm_FormFilterSetting();
				frm_FormFilterSetting2.dtPgcMrps = dataTable;
				frm_FormFilterSetting2.objType = objType;
				frm_FormFilterSetting2.MdiParent = ((ContextMenuStrip)Parent).SourceControl.FindForm().MdiParent;
				frm_FormFilterSetting2.Show();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.DatasourceIsNull, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected override void OnClick(EventArgs e)
	{
		base.OnClick(e);
		FilterSettingClick();
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
	}
}
