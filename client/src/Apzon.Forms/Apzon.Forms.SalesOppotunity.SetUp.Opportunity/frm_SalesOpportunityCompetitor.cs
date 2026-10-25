using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Controls.Grid;
using Apzon.Forms.Commons;
using Apzon.HttpClientHelper;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;

namespace Apzon.Forms.SalesOppotunity.SetUp.Opportunity;

public class frm_SalesOpportunityCompetitor : VirSubFormOpportunitySetup
{
	private IContainer components;

	public frm_SalesOpportunityCompetitor()
	{
		InitializeComponent();
		FormController = "OpportunityCompetitor";
		TableName = "OCMT";
		FormId = "oOpportunityCompetitor";
		gvDoc.FormId = FormId;
		gvDoc.ItemId = "OCMT";
		Text = "Competitor - Setup";
	}

	protected override async void removeToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataTable = grdDoc.DataSource as DataTable;
			int[] selectedRows = gvDoc.GetSelectedRows();
			if (selectedRows != null && selectedRows.Length != 0 && Function.ParseInt(gvDoc.GetRowCellValue(selectedRows[0], "CompetId")) > 0)
			{
				gvDoc.DeleteSelectedRows();
				if (dataTable != null)
				{
					dataTable.AcceptChanges();
					btOK.Text = "Update";
				}
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
			gridColumnEx.Caption = "CompetId";
			gridColumnEx.FieldName = "CompetId";
			gridColumnEx.Visible = false;
			gridColumnEx.OptionsColumn.ReadOnly = true;
			GridColumnEx gridColumnEx2 = new GridColumnEx();
			gridColumnEx2.Caption = "Name";
			gridColumnEx2.FieldName = "Name";
			gridColumnEx2.Visible = true;
			gridColumnEx2.OptionsColumn.ReadOnly = false;
			GridColumnEx gridColumnEx3 = new GridColumnEx();
			gridColumnEx3.Caption = "Threat Level";
			gridColumnEx3.FieldName = "ThreatLevl";
			gridColumnEx3.Visible = true;
			gridColumnEx3.OptionsColumn.ReadOnly = false;
			GridColumnEx gridColumnEx4 = new GridColumnEx();
			gridColumnEx4.Caption = "Details";
			gridColumnEx4.FieldName = "Memo";
			gridColumnEx4.Visible = true;
			gridColumnEx4.OptionsColumn.ReadOnly = false;
			GridColumnEx gridColumnEx5 = new GridColumnEx();
			gridColumnEx5.Caption = "DataSource";
			gridColumnEx5.FieldName = "DataSource";
			gridColumnEx5.Visible = false;
			gridColumnEx5.OptionsColumn.ReadOnly = true;
			GridColumnEx gridColumnEx6 = new GridColumnEx();
			gridColumnEx6.Caption = "UserSign";
			gridColumnEx6.FieldName = "UserSign";
			gridColumnEx6.Visible = false;
			gridColumnEx6.OptionsColumn.ReadOnly = true;
			GridColumnCollection columns = gvDoc.Columns;
			GridColumn[] columns2 = new GridColumnEx[6] { gridColumnEx, gridColumnEx2, gridColumnEx3, gridColumnEx4, gridColumnEx5, gridColumnEx6 };
			columns.AddRange(columns2);
			RepositoryItemLookUpEdit repositoryItemLookUpEdit = new RepositoryItemLookUpEdit();
			repositoryItemLookUpEdit.AutoHeight = false;
			repositoryItemLookUpEdit.Columns.Clear();
			repositoryItemLookUpEdit.Columns.Add(new LookUpColumnInfo("Name", ""));
			repositoryItemLookUpEdit.DisplayMember = "Name";
			repositoryItemLookUpEdit.Name = "lkThreatLevel";
			repositoryItemLookUpEdit.ValueMember = "Code";
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add(new DataColumn("Name", typeof(string)));
			dataTable.Columns.Add(new DataColumn("Code", typeof(short)));
			DataRow dataRow = dataTable.NewRow();
			dataRow["Code"] = Function.ParseShort(1);
			dataRow["Name"] = "Low";
			dataTable.Rows.Add(dataRow);
			DataRow dataRow2 = dataTable.NewRow();
			dataRow2["Code"] = Function.ParseShort(2);
			dataRow2["Name"] = "Medium";
			dataTable.Rows.Add(dataRow2);
			DataRow dataRow3 = dataTable.NewRow();
			dataRow3["Code"] = Function.ParseShort(3);
			dataRow3["Name"] = "High";
			dataTable.Rows.Add(dataRow3);
			dataTable.AcceptChanges();
			repositoryItemLookUpEdit.DataSource = dataTable;
			gvDoc.Columns["ThreatLevl"].ColumnEdit = repositoryItemLookUpEdit;
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
				if (string.IsNullOrWhiteSpace(Function.ToString(dtRet.Rows[num]["Name"])))
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
				dataTable = WinFuntions.GetTableStructure("OCMT");
			}
			if (!dataTable.AsEnumerable().Any((DataRow r) => string.IsNullOrWhiteSpace(Function.ToString(r["Name"]))))
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["CompetId"] = 0;
				dataRow["Name"] = "";
				dataRow["ThreatLevl"] = 1;
				dataRow["Memo"] = "";
				dataRow["DataSource"] = "N";
				dataRow["UserSign"] = 1;
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
