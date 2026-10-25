using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Controls;
using Apzon.Controls.Grid;
using Apzon.Forms.Commons;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.XtraGrid.Columns;

namespace Apzon.Forms.SalesOppotunity.SetUp.Opportunity;

public class frm_InterestLevel : VirSubFormOpportunitySetup
{
	private IContainer components;

	public frm_InterestLevel()
	{
		InitializeComponent();
		FormController = "InterestLevel";
		TableName = "OOIR";
		FormId = "oInterestLevel";
		gvDoc.FormId = FormId;
		gvDoc.ItemId = "OOIR";
		Text = "Interest Level - Setup";
	}

	protected override async void removeToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataTable = grdDoc.DataSource as DataTable;
			int[] selectedRows = gvDoc.GetSelectedRows();
			if (selectedRows != null && selectedRows.Length != 0)
			{
				if (Function.ParseInt(gvDoc.GetRowCellValue(selectedRows[0], "Num")) > 0)
				{
					gvDoc.DeleteSelectedRows();
					if (dataTable != null)
					{
						dataTable.AcceptChanges();
						btOK.Text = "Update";
					}
				}
				return;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		AddNewRow();
	}

	protected override void CreateGridViewColumn()
	{
		try
		{
			gvDoc.Columns.Clear();
			GridColumnEx gridColumnEx = new GridColumnEx();
			gridColumnEx.Caption = "Sequence No.";
			gridColumnEx.FieldName = "Num";
			gridColumnEx.Visible = false;
			gridColumnEx.OptionsColumn.ReadOnly = true;
			GridColumnEx gridColumnEx2 = new GridColumnEx();
			gridColumnEx2.Caption = "Description";
			gridColumnEx2.FieldName = "Descript";
			gridColumnEx2.Visible = true;
			gridColumnEx2.OptionsColumn.ReadOnly = false;
			GridColumnEx gridColumnEx3 = new GridColumnEx();
			gridColumnEx3.Caption = "Sort";
			gridColumnEx3.FieldName = "SortOrder";
			gridColumnEx3.Visible = true;
			gridColumnEx3.OptionsColumn.ReadOnly = false;
			GridColumnEx gridColumnEx4 = new GridColumnEx();
			gridColumnEx4.Caption = "User Signature";
			gridColumnEx4.FieldName = "UserSign";
			gridColumnEx4.Visible = true;
			gridColumnEx4.OptionsColumn.ReadOnly = false;
			GridColumnCollection columns = gvDoc.Columns;
			GridColumn[] columns2 = new GridColumnEx[4] { gridColumnEx, gridColumnEx2, gridColumnEx3, gridColumnEx4 };
			columns.AddRange(columns2);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
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
				if (string.IsNullOrWhiteSpace(Function.ToString(dtRet.Rows[num]["Descript"])))
				{
					dtRet.Rows.Remove(dtRet.Rows[num]);
				}
			}
			dtRet.AcceptChanges();
			if (dtRet.Rows.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.NoDataToSave, "");
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
		dtRet = new DataTable();
		return false;
	}

	protected override async Task ShowData()
	{
		try
		{
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
			DataTable dataSource = await _helper.GetAsync("GetDocs");
			grdDoc.DataSource = dataSource;
			grdDoc.RefreshDataSource();
			btOK.Text = "OK";
			AddNewRow();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected override void AddNewRow()
	{
		try
		{
			DataTable dataTable = grdDoc.DataSource as DataTable;
			if (dataTable == null || dataTable.Rows.Count == 0)
			{
				dataTable = WinFuntions.GetTableStructure("OOIN");
			}
			if (!dataTable.AsEnumerable().Any((DataRow r) => string.IsNullOrWhiteSpace(Function.ToString(r["Descript"]))))
			{
				int num = 0;
				if (dataTable.IsNotNull())
				{
					num = dataTable.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["SortOrder"]));
				}
				num = ((num >= 100) ? (num + 1) : 100);
				DataRow dataRow = dataTable.NewRow();
				dataRow["Num"] = 0;
				dataRow["Descript"] = "";
				dataRow["SortOrder"] = num;
				dataRow["UserSign"] = GlobalData.UserID;
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
			}
			grdDoc.DataSource = dataTable;
			grdDoc.RefreshDataSource();
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
