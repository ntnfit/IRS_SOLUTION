using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Grid;
using Apzon.Forms.Commons;
using Apzon.HttpClientHelper;
using DevExpress.XtraGrid.Columns;

namespace Apzon.Forms.SalesOppotunity.SetUp.Opportunity;

public class frm_SalesOpportunityRelationship : VirSubFormOpportunitySetup
{
	private IContainer components;

	public frm_SalesOpportunityRelationship()
	{
		InitializeComponent();
		FormController = "OpportunityRelationship";
		TableName = "OORL";
		FormId = "oOpportunityRelationship";
		gvDoc.FormId = FormId;
		gvDoc.ItemId = "OORL";
		Text = "Relationships - Setup";
	}

	protected override async void removeToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataTable = grdDoc.DataSource as DataTable;
			int[] selectedRows = gvDoc.GetSelectedRows();
			if (selectedRows != null && selectedRows.Length != 0)
			{
				if (Function.ParseInt(gvDoc.GetRowCellValue(selectedRows[0], "OrlCode")) > 0)
				{
					gvDoc.DeleteSelectedRows();
					if (dataTable != null)
					{
						dataTable.AcceptChanges();
						btOK.Text = Resources.ButtonUpdate;
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
			gridColumnEx.Caption = "OrlCode";
			gridColumnEx.FieldName = "OrlCode";
			gridColumnEx.Visible = false;
			gridColumnEx.OptionsColumn.ReadOnly = true;
			GridColumnEx gridColumnEx2 = new GridColumnEx();
			gridColumnEx2.Caption = "Relationship Description";
			gridColumnEx2.FieldName = "OrlDesc";
			gridColumnEx2.Visible = true;
			gridColumnEx.OptionsColumn.ReadOnly = false;
			GridColumnCollection columns = gvDoc.Columns;
			GridColumn[] columns2 = new GridColumnEx[2] { gridColumnEx, gridColumnEx2 };
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
			dtRet = dataTable.Copy();
			for (int num = dtRet.Rows.Count - 1; num >= 0; num--)
			{
				if (string.IsNullOrWhiteSpace(Function.ToString(dtRet.Rows[num]["OrlDesc"])))
				{
					dtRet.Rows.Remove(dtRet.Rows[num]);
				}
			}
			dtRet.AcceptChanges();
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
				dataTable = WinFuntions.GetTableStructure("OORL");
			}
			if (!dataTable.AsEnumerable().Any((DataRow r) => string.IsNullOrWhiteSpace(Function.ToString(r["OrlDesc"]))))
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["OrlCode"] = 0;
				dataRow["OrlDesc"] = "";
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
