using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Controls.Grid;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Forms.Properties;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;

namespace Apzon.Forms.SalesOppotunity.SetUp.Opportunity;

public class frm_SalesOpportunityPartner : VirSubFormOpportunitySetup
{
	private IContainer components;

	public frm_SalesOpportunityPartner()
	{
		InitializeComponent();
		FormController = "OpportunityPartner";
		TableName = "OPRT";
		FormId = "oOpportunityPartner";
		gvDoc.FormId = FormId;
		gvDoc.ItemId = "OPRT";
		Text = "Partner - Setup";
	}

	protected override async void removeToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataTable = grdDoc.DataSource as DataTable;
			int[] selectedRows = gvDoc.GetSelectedRows();
			if (selectedRows != null && selectedRows.Length != 0)
			{
				if (Function.ParseInt(gvDoc.GetRowCellValue(selectedRows[0], "PrtId")) > 0)
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

	private void rep_RelatBP_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.GetSalesOppPartner, false, "", "", Enums.ItemObjectType.NoItemType, null, "");
				frm_CFL2.ShowDialog(this);
				if (frm_CFL2.ChooseItem != null)
				{
					_ = grdDoc.DataSourceTable;
					gvDoc.SetFocusedRowCellValue("RelatCard", Function.ToString(frm_CFL2.ChooseItem["CardCode"]));
					grdDoc.DataSourceTable.AcceptChanges();
					btOK.Text = "Update";
				}
			}
			if (e.Button.Index == 1)
			{
				string text = Function.ToString(gvDoc.GetFocusedRowCellValue("RelatCard"));
				if (!string.IsNullOrEmpty(text))
				{
					this.DrillDown(ApzObjectTypes.oBusinessPartners, ApzObjectTypes.oBusinessPartners, text);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected override void CreateGridViewColumn()
	{
		try
		{
			gvDoc.Columns.Clear();
			GridColumnEx gridColumnEx = new GridColumnEx();
			gridColumnEx.Caption = "PrtId";
			gridColumnEx.FieldName = "PrtId";
			gridColumnEx.Visible = false;
			gridColumnEx.OptionsColumn.ReadOnly = true;
			GridColumnEx gridColumnEx2 = new GridColumnEx();
			gridColumnEx2.Caption = "Name";
			gridColumnEx2.FieldName = "Name";
			gridColumnEx2.Visible = true;
			gridColumnEx.OptionsColumn.ReadOnly = false;
			GridColumnEx gridColumnEx3 = new GridColumnEx();
			gridColumnEx3.Caption = "Default Relationship";
			gridColumnEx3.FieldName = "DefaultORL";
			gridColumnEx3.Visible = true;
			gridColumnEx3.OptionsColumn.ReadOnly = false;
			GridColumnEx gridColumnEx4 = new GridColumnEx();
			gridColumnEx4.Caption = "RelatnType";
			gridColumnEx4.FieldName = "RelatnType";
			gridColumnEx4.Visible = false;
			gridColumnEx4.OptionsColumn.ReadOnly = true;
			GridColumnEx gridColumnEx5 = new GridColumnEx();
			gridColumnEx5.Caption = "Related BP";
			gridColumnEx5.FieldName = "RelatCard";
			gridColumnEx5.Visible = true;
			gridColumnEx4.OptionsColumn.ReadOnly = false;
			GridColumnEx gridColumnEx6 = new GridColumnEx();
			gridColumnEx6.Caption = "Details";
			gridColumnEx6.FieldName = "Memo";
			gridColumnEx6.Visible = true;
			gridColumnEx6.OptionsColumn.ReadOnly = false;
			GridColumnEx gridColumnEx7 = new GridColumnEx();
			gridColumnEx7.Caption = "DataSource";
			gridColumnEx7.FieldName = "DataSource";
			gridColumnEx7.Visible = false;
			gridColumnEx7.OptionsColumn.ReadOnly = true;
			GridColumnEx gridColumnEx8 = new GridColumnEx();
			gridColumnEx8.Caption = "UserSign";
			gridColumnEx8.FieldName = "UserSign";
			gridColumnEx8.Visible = false;
			gridColumnEx8.OptionsColumn.ReadOnly = true;
			GridColumnCollection columns = gvDoc.Columns;
			GridColumn[] columns2 = new GridColumnEx[8] { gridColumnEx, gridColumnEx2, gridColumnEx4, gridColumnEx5, gridColumnEx6, gridColumnEx7, gridColumnEx8, gridColumnEx3 };
			columns.AddRange(columns2);
			RepositoryItemLookUpEdit repositoryItemLookUpEdit = new RepositoryItemLookUpEdit();
			repositoryItemLookUpEdit.NullText = "";
			repositoryItemLookUpEdit.Columns.Clear();
			repositoryItemLookUpEdit.DisplayMember = "OrlDesc";
			repositoryItemLookUpEdit.ValueMember = "OrlCode";
			repositoryItemLookUpEdit.Columns.Add(new LookUpColumnInfo("OrlDesc", ""));
			using (HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Opportunity"))
			{
				DataTable dataTable = httpClientHelper.Get("GetListPartnerRelation");
				if (dataTable.Columns.Count == 0)
				{
					dataTable.Columns.Add(new DataColumn("OrlCode", typeof(int)));
					dataTable.Columns.Add(new DataColumn("OrlDesc", typeof(string)));
				}
				DataRow dataRow = dataTable.NewRow();
				dataRow["OrlCode"] = -1;
				dataRow["OrlDesc"] = "";
				dataTable.Rows.InsertAt(dataRow, 0);
				if (dataTable != null && dataTable.Rows.Count > 0)
				{
					repositoryItemLookUpEdit.DataSource = dataTable;
				}
			}
			gvDoc.Columns["DefaultORL"].ColumnEdit = repositoryItemLookUpEdit;
			SerializableAppearanceObject appearance = new SerializableAppearanceObject();
			RepositoryItemButtonEdit repositoryItemButtonEdit = new RepositoryItemButtonEdit();
			repositoryItemButtonEdit.Buttons.Clear();
			repositoryItemButtonEdit.AutoHeight = false;
			repositoryItemButtonEdit.Buttons.AddRange(new EditorButton[2]
			{
				new EditorButton(ButtonPredefines.Search),
				new EditorButton(ButtonPredefines.Glyph, "", -1, enabled: true, visible: true, isLeft: true, ImageLocation.MiddleCenter, Resources.right, new KeyShortcut(Keys.None), appearance, "", null, null, enableImageTransparency: true)
			});
			repositoryItemButtonEdit.Name = "rep_RelatBP";
			repositoryItemButtonEdit.ButtonClick += rep_RelatBP_ButtonClick;
			gvDoc.Columns["RelatCard"].ColumnEdit = repositoryItemButtonEdit;
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
		return true;
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
				dataTable = WinFuntions.GetTableStructure("OPRT");
			}
			if (!dataTable.AsEnumerable().Any((DataRow r) => string.IsNullOrWhiteSpace(Function.ToString(r["Name"]))))
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["PrtId"] = 0;
				dataRow["Name"] = "";
				dataRow["RelatnType"] = "";
				dataRow["RelatCard"] = "";
				dataRow["Memo"] = "";
				dataRow["DataSource"] = "N";
				dataRow["UserSign"] = GlobalData.UserID;
				dataRow["DefaultORL"] = -1;
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
