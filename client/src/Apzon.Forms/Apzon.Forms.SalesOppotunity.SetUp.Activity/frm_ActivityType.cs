using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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

namespace Apzon.Forms.SalesOppotunity.SetUp.Activity;

public class frm_ActivityType : VirSubFormActivitySetup
{
	private IContainer components;

	public frm_ActivityType()
	{
		InitializeComponent();
		FormController = "ActivityType";
		TableName = "OCLT";
		FormId = "oActivityType";
		Text = "Activity Type Setup";
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
			gridColumnEx5.Caption = "Active";
			gridColumnEx5.FieldName = "Active";
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
					if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmRemoveActivityType, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes && string.IsNullOrEmpty(await Delete(num)))
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
			for (int i = 0; i < dtRet.Rows.Count; i++)
			{
				if (dtRet.Rows[i]["UserSign"] == DBNull.Value || Function.ParseInt(dtRet.Rows[i]["UserSign"]) == 0)
				{
					dtRet.Rows[i]["UserSign"] = GlobalData.UserID;
				}
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
			dtRet.AcceptChanges();
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
			if (dataTable == null || dataTable.Rows.Count == 0)
			{
				dataTable = WinFuntions.GetTableStructure("OCLT");
			}
			if (!dataTable.AsEnumerable().Any((DataRow r) => string.IsNullOrWhiteSpace(Function.ToString(r["Name"]))))
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["Code"] = Function.ParseShort(0);
				dataRow["Name"] = "";
				dataRow["DataSource"] = "N";
				dataRow["Active"] = "Y";
				dataRow["UserSign"] = GlobalData.UserID;
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
