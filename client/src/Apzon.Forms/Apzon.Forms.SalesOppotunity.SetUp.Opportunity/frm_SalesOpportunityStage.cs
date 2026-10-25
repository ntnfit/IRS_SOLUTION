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
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;

namespace Apzon.Forms.SalesOppotunity.SetUp.Opportunity;

public class frm_SalesOpportunityStage : VirSubFormOpportunitySetup
{
	private IContainer components;

	public frm_SalesOpportunityStage()
	{
		InitializeComponent();
		FormController = "OpportunityStage";
		TableName = "OOST";
		FormId = "oOpportunityStage";
		Text = "Opportunity Stage Setup";
		gvDoc.FormId = FormId;
		gvDoc.ItemId = "OOST";
		removeToolStripMenuItem.Visible = false;
	}

	protected override void CreateGridViewColumn()
	{
		try
		{
			gvDoc.Columns.Clear();
			GridColumnEx gridColumnEx = new GridColumnEx();
			gridColumnEx.Caption = "Num";
			gridColumnEx.FieldName = "Num";
			gridColumnEx.Visible = false;
			gridColumnEx.OptionsColumn.ReadOnly = true;
			GridColumnEx gridColumnEx2 = new GridColumnEx();
			gridColumnEx2.Caption = "Name";
			gridColumnEx2.FieldName = "Descript";
			gridColumnEx2.Visible = true;
			gridColumnEx2.OptionsColumn.ReadOnly = false;
			GridColumnEx gridColumnEx3 = new GridColumnEx();
			gridColumnEx3.Caption = "Stage No.";
			gridColumnEx3.FieldName = "StepId";
			gridColumnEx3.Visible = true;
			gridColumnEx3.OptionsColumn.ReadOnly = false;
			GridColumnEx gridColumnEx4 = new GridColumnEx();
			gridColumnEx4.Caption = "Closing Percentage";
			gridColumnEx4.FieldName = "CloPrcnt";
			gridColumnEx4.Visible = true;
			gridColumnEx4.OptionsColumn.ReadOnly = false;
			GridColumnEx gridColumnEx5 = new GridColumnEx();
			gridColumnEx5.Caption = "Canceled";
			gridColumnEx5.FieldName = "Canceled";
			gridColumnEx5.Visible = true;
			gridColumnEx5.OptionsColumn.ReadOnly = false;
			GridColumnEx gridColumnEx6 = new GridColumnEx();
			gridColumnEx6.Caption = "UserSign";
			gridColumnEx6.FieldName = "UserSign";
			gridColumnEx6.Visible = false;
			gridColumnEx6.OptionsColumn.ReadOnly = true;
			GridColumnEx gridColumnEx7 = new GridColumnEx();
			gridColumnEx7.Caption = "Sales";
			gridColumnEx7.FieldName = "SalesStage";
			gridColumnEx7.Visible = true;
			gridColumnEx7.OptionsColumn.ReadOnly = false;
			GridColumnEx gridColumnEx8 = new GridColumnEx();
			gridColumnEx8.Caption = "Purchasing";
			gridColumnEx8.FieldName = "PurStage";
			gridColumnEx8.Visible = true;
			gridColumnEx8.OptionsColumn.ReadOnly = false;
			GridColumnCollection columns = gvDoc.Columns;
			GridColumn[] columns2 = new GridColumnEx[8] { gridColumnEx, gridColumnEx2, gridColumnEx3, gridColumnEx4, gridColumnEx5, gridColumnEx6, gridColumnEx7, gridColumnEx8 };
			columns.AddRange(columns2);
			RepositoryItemCheckEdit repositoryItemCheckEdit = new RepositoryItemCheckEdit();
			repositoryItemCheckEdit.DisplayValueChecked = "Y";
			repositoryItemCheckEdit.DisplayValueGrayed = "N";
			repositoryItemCheckEdit.DisplayValueUnchecked = "N";
			repositoryItemCheckEdit.ValueChecked = "Y";
			repositoryItemCheckEdit.ValueGrayed = "N";
			repositoryItemCheckEdit.ValueUnchecked = "N";
			RepositoryItemCheckEdit repositoryItemCheckEdit2 = new RepositoryItemCheckEdit();
			repositoryItemCheckEdit2.DisplayValueChecked = "Y";
			repositoryItemCheckEdit2.DisplayValueGrayed = "N";
			repositoryItemCheckEdit2.DisplayValueUnchecked = "N";
			repositoryItemCheckEdit2.ValueChecked = "Y";
			repositoryItemCheckEdit2.ValueGrayed = "N";
			repositoryItemCheckEdit2.ValueUnchecked = "N";
			RepositoryItemCheckEdit repositoryItemCheckEdit3 = new RepositoryItemCheckEdit();
			repositoryItemCheckEdit3.DisplayValueChecked = "Y";
			repositoryItemCheckEdit3.DisplayValueGrayed = "N";
			repositoryItemCheckEdit3.DisplayValueUnchecked = "N";
			repositoryItemCheckEdit3.ValueChecked = "Y";
			repositoryItemCheckEdit3.ValueGrayed = "N";
			repositoryItemCheckEdit3.ValueUnchecked = "N";
			RepositoryItemCalcEdit columnEdit = new RepositoryItemCalcEdit();
			gvDoc.Columns["SalesStage"].ColumnEdit = repositoryItemCheckEdit;
			gvDoc.Columns["PurStage"].ColumnEdit = repositoryItemCheckEdit2;
			gvDoc.Columns["Canceled"].ColumnEdit = repositoryItemCheckEdit3;
			gvDoc.Columns["CloPrcnt"].ColumnEdit = columnEdit;
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

	private short FindMaxStepId(DataTable dt)
	{
		try
		{
			short value = Function.ParseShort(0).Value;
			if (dt.IsNotNull())
			{
				for (int i = 0; i < dt.Rows.Count; i++)
				{
					short? num = Function.ParseShort(dt.Rows[i]["StepId"]);
					if (num > value)
					{
						value = num.Value;
					}
				}
			}
			return value;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return 0;
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
				if (string.IsNullOrWhiteSpace(Function.ToString(dtRet.Rows[num]["Descript"])))
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
				dataTable = WinFuntions.GetTableStructure("OOST");
			}
			if (!dataTable.AsEnumerable().Any((DataRow r) => string.IsNullOrWhiteSpace(Function.ToString(r["Descript"]))))
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["Num"] = 0;
				dataRow["StepId"] = FindMaxStepId(dataTable) + 1;
				dataRow["Descript"] = "";
				dataRow["Cloprcnt"] = 0;
				dataRow["UserSign"] = GlobalData.UserID;
				dataRow["Canceled"] = "N";
				dataRow["SalesStage"] = "Y";
				dataRow["PurStage"] = "Y";
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
