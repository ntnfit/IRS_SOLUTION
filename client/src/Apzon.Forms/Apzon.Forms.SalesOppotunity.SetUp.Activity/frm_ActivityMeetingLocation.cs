using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Controls;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Forms.Commons;
using Apzon.Global;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.SalesOppotunity.SetUp.Activity;

public class frm_ActivityMeetingLocation : VirSubFormActivitySetup
{
	public string madk = string.Empty;

	public string nameType = string.Empty;

	private IContainer components;

	public frm_ActivityMeetingLocation()
	{
		InitializeComponent();
		FormController = "ActivityMeetingLocation";
		TableName = "OCLO";
		FormId = "oActivityMeetingLocation";
		Text = "Activity Meeting Location Setup";
	}

	protected override void CreateGridViewColumn()
	{
		try
		{
			gvDoc.Columns.Clear();
			GridColumnEx gridColumnEx = new GridColumnEx();
			gridColumnEx.Caption = "Code";
			gridColumnEx.FieldName = "Code";
			gridColumnEx.Visible = false;
			gridColumnEx.OptionsColumn.ReadOnly = true;
			GridColumnEx gridColumnEx2 = new GridColumnEx();
			gridColumnEx2.Caption = "Name";
			gridColumnEx2.FieldName = "Name";
			gridColumnEx2.Visible = true;
			GridColumnEx gridColumnEx3 = new GridColumnEx();
			gridColumnEx3.Caption = "DataSource";
			gridColumnEx3.FieldName = "DataSource";
			gridColumnEx3.Visible = false;
			gridColumnEx3.OptionsColumn.ReadOnly = true;
			GridColumnEx gridColumnEx4 = new GridColumnEx();
			gridColumnEx4.Caption = "UserSign";
			gridColumnEx4.FieldName = "UserSign";
			gridColumnEx4.Visible = false;
			gridColumnEx4.OptionsColumn.ReadOnly = true;
			GridColumnEx gridColumnEx5 = new GridColumnEx();
			gridColumnEx5.Caption = "Locked";
			gridColumnEx5.FieldName = "Locked";
			gridColumnEx5.Visible = false;
			gridColumnEx5.OptionsColumn.ReadOnly = true;
			GridColumnCollection columns = gvDoc.Columns;
			GridColumn[] columns2 = new GridColumnEx[5] { gridColumnEx, gridColumnEx2, gridColumnEx3, gridColumnEx4, gridColumnEx5 };
			columns.AddRange(columns2);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected override async void removeToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dt = grdDoc.DataSource as DataTable;
			int[] selectedRows = gvDoc.GetSelectedRows();
			if (selectedRows != null && selectedRows.Length != 0)
			{
				int num = Function.ParseInt(gvDoc.GetRowCellValue(selectedRows[0], "Code"));
				if (num == -1)
				{
					return;
				}
				if (num > 0)
				{
					if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmRemoveMettingLocation, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes && string.IsNullOrEmpty(await Delete(num)))
					{
						gvDoc.DeleteSelectedRows();
					}
				}
				else
				{
					gvDoc.DeleteSelectedRows();
				}
				dt?.AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		AddNewRow();
	}

	protected override void gvDoc_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		string text = Function.ToString(gvDoc.GetRowCellValue(e.RowHandle, "name"));
		if (text == "Completed" || text == "Not Started")
		{
			e.Appearance.BackColor = Color.LightGray;
		}
	}

	protected override void gvDoc_ShowingEditor(object sender, CancelEventArgs e)
	{
		string text = Function.ToString(gvDoc.GetFocusedRowCellValue("name"));
		if (text == "Completed" || text == "Not Started")
		{
			e.Cancel = true;
		}
		else
		{
			e.Cancel = false;
		}
	}

	protected override bool CheckSave(out DataTable dtRet)
	{
		try
		{
			DataTable dataTable = grdDoc.DataSource as DataTable;
			if (!dataTable.IsNotNull())
			{
				WinCommons.Error(Enums.MessageCode.NoDataToSave, "");
				dtRet = null;
				return false;
			}
			dtRet = dataTable.Copy();
			for (int num = dtRet.Rows.Count - 1; num >= 0; num--)
			{
				if (string.IsNullOrWhiteSpace(Function.ToString(dtRet.Rows[num]["Name"])))
				{
					dtRet.Rows.Remove(dtRet.Rows[num]);
				}
			}
			dtRet.AcceptChanges();
			if (dtRet.Rows.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.NoDataToSave, "");
				return false;
			}
			List<string> list = (from dr in dtRet.AsEnumerable()
				select dr.Field<string>("Name") into x
				group x by x into g
				where g.Count() > 1
				select g.Key).ToList();
			if (list != null && list.Count > 0)
			{
				string text = list[0];
				WinCommons.Error(Enums.MessageCode.DuplicateName, "", text);
				dtRet = null;
				return false;
			}
			return true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		dtRet = null;
		return false;
	}

	protected override void AddNewRow()
	{
		try
		{
			DataTable dataTable = grdDoc.DataSource as DataTable;
			if (dataTable == null || dataTable.Columns.Count == 0)
			{
				dataTable = WinFuntions.GetTableStructure("OCLO");
			}
			if (!dataTable.AsEnumerable().Any((DataRow r) => string.IsNullOrWhiteSpace(Function.ToString(r["Name"]))))
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["Code"] = Function.ParseShort(0);
				dataRow["Name"] = "";
				dataRow["DataSource"] = "N";
				dataRow["UserSign"] = GlobalData.UserID;
				dataRow["Locked"] = "N";
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
				grdDoc.DataSource = dataTable;
				grdDoc.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
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
		this.components = new System.ComponentModel.Container();
	}
}
