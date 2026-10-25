using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Helper;
using Apzon.Forms.UserControls;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Utils.Behaviors;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.Packing_Management;

public class frm_Traceability : XtraFormEx
{
	private bool _isSaveSuccess;

	private List<Common.ParameterValue> TrangThai = new List<Common.ParameterValue>();

	private List<Common.ParameterValue> DonVi = new List<Common.ParameterValue>();

	private IContainer components;

	private LabelControl labelCtrl_CodeCase;

	private LabelControl labelControl2;

	private LabelControl labelControl4;

	private SimpleButton simpleBtn_OK;

	private SimpleButton simpleBtn_Cancel;

	private ButtonEditCFL bte_CaseCode;

	private BehaviorManager behaviorManager1;

	private ContextMenuStrip contextMenuStrip_Item;

	private ToolStripMenuItem addItemToolStripMenuItem;

	private ToolStripMenuItem deleteItemToolStripMenuItem;

	private GridViewEx gridViewEx1;

	private RepositoryItemLookUpEdit repositoryItemLookUpEdit1;

	private RepositoryItemLookUpEdit repositoryItemLookUpEdit2;

	private GridControlEx gridControl_Case;

	private GridViewEx gridView_Case;

	private GridColumnEx CaseCode;

	private GridColumnEx CaseName;

	private GridColumnEx TypeCode;

	private GridColumnEx Amount;

	private GridColumnEx Status;

	private RepositoryItemLookUpEdit repositoryLUE_Status;

	private GridColumnEx USERID;

	private GridColumnEx CreateDate;

	private GridColumnEx WhsCode;

	private GridColumnEx Unit;

	private RepositoryItemLookUpEdit repositoryLUE_Unit;

	private GridColumnEx Remark;

	private GridColumnEx POCode;

	private GridColumnEx PostingDate;

	private RepositoryItemLookUpEdit repositoryLUE_USERID;

	private GridControlEx gridControl_Item;

	private GridViewEx gridView_Item;

	private GridColumnEx gridColumnEx1;

	private GridColumnEx gridColumnEx2;

	private GridColumnEx gridColumnEx3;

	private GridColumnEx gridColumnEx4;

	private GridColumnEx gridColumnEx5;

	private GridColumnEx LineNum;

	private ContextMenuStrip contextMenuStrip_Case;

	private ToolStripMenuItem deleteCaseToolStripMenuItem;

	private SplitContainerControl splitContainerControl1;

	private SimpleButtonEx simpleBtn_Search;

	private ButtonEditCFL txtBox_typeCode;

	public DateEditEx txtBox_PostingDate;

	private GridColumnEx LineNumber;

	private string _CaseCode { get; set; }

	private int _LineNumber { get; set; }

	private string _BundleCode { get; set; }

	public frm_Traceability()
	{
		InitializeComponent();
	}

	private async void frm_Traceability_Load(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			txtBox_PostingDate.SetFormatControl(Enums.DisplayFormatType.DateTime);
			using (HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Traceability"))
			{
				DataTable dataSource = await http.GetAsync("GetCaseTracibility");
				gridControl_Case.DataSource = dataSource;
				DataTable dataSource2 = await http.GetAsync("GetUser");
				repositoryLUE_USERID.DataSource = dataSource2;
				repositoryLUE_USERID.DisplayMember = "U_NAME";
				repositoryLUE_USERID.ValueMember = "USERID";
			}
			TrangThai.Add(new Common.ParameterValue("Plan", "p"));
			TrangThai.Add(new Common.ParameterValue("Close", "c"));
			TrangThai.Add(new Common.ParameterValue("Cancel", "x"));
			repositoryLUE_Status.DataSource = TrangThai;
			repositoryLUE_Status.DisplayMember = "Name";
			repositoryLUE_Status.ValueMember = "Value";
			repositoryLUE_Status.ShowHeader = false;
			DonVi.Add(new Common.ParameterValue("Bundle", "b"));
			DonVi.Add(new Common.ParameterValue("Case", "c"));
			repositoryLUE_Unit.DataSource = DonVi;
			repositoryLUE_Unit.DisplayMember = "Name";
			repositoryLUE_Unit.ValueMember = "Value";
			repositoryLUE_Unit.ShowHeader = false;
			RepositoryItemButtonEditCFL properties = bte_CaseCode.Properties;
			properties.HandleChooseItem = (Action<DataRow>)Delegate.Combine(properties.HandleChooseItem, (Action<DataRow>)((DataRow chooseItem) =>
			{
				if (chooseItem != null)
				{
					FormMode = Enums.FormViewModeType.View;
					bte_CaseCode.EditValue = chooseItem["CaseCode"];
					_ = gridView_Case.FocusedRowHandle;
					SEARCH();
				}
			}));
			RepositoryItemButtonEditCFL properties2 = txtBox_typeCode.Properties;
			properties2.HandleChooseItem = (Action<DataRow>)Delegate.Combine(properties2.HandleChooseItem, (Action<DataRow>)((DataRow chooseItem) =>
			{
				if (chooseItem != null)
				{
					FormMode = Enums.FormViewModeType.View;
					txtBox_typeCode.EditValue = chooseItem["TypeCode"];
				}
			}));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.View;
	}

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.View:
			simpleBtn_OK.Text = "OK";
			break;
		case Enums.FormViewModeType.Update:
			simpleBtn_OK.Text = "Update";
			break;
		}
	}

	private async void Savedt()
	{
		try
		{
			DataTable dataTable = gridControl_Case.DataSourceTable.Clone();
			DataRow dataRow = gridView_Case.GetDataRow(gridView_Case.FocusedRowHandle);
			DataRow dataRow2 = dataTable.NewRow();
			dataRow2.ItemArray = dataRow.ItemArray;
			dataTable.Rows.Add(dataRow2);
			dataTable.TableName = "dtCase";
			DataTable dataTable2 = gridControl_Item.DataSource as DataTable;
			dataTable2.TableName = "dtItem";
			DataSet dataSet = new DataSet();
			dataSet.Tables.Add(dataTable);
			dataSet.Tables.Add(dataTable2);
			_CaseCode = Function.ToString(gridView_Case.GetRowCellValue(gridView_Case.FocusedRowHandle, "CaseCode"));
			if (gridView_Case.FocusedRowHandle.ToString() != null)
			{
				WinCommons._OpenProcessing();
				using HttpClientHelper<string> http = new HttpClientHelper<string>(Configuration.ApiServer, "Traceability");
				string text = await http.PostAsync("Update", dataSet);
				if (string.IsNullOrWhiteSpace(text))
				{
					WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, text);
				}
				WinCommons._CloseProcessing();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		FormMode = Enums.FormViewModeType.View;
	}

	private async void simpleBtn_Save_Click(object sender, EventArgs e)
	{
		if (FormMode == Enums.FormViewModeType.View)
		{
			Close();
		}
		else if (XtraMessageBox.Show("Save Change", "Confirmation", MessageBoxButtons.YesNo) != DialogResult.No)
		{
			Savedt();
		}
	}

	public async Task REFRESH()
	{
		try
		{
			Refresh();
			using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Traceability");
			DataTable dataSource = await http.GetAsync("GetCaseTracibility");
			gridControl_Case.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.View;
	}

	private async Task LoadData(Enums.NavigateType type)
	{
		string caseCode = (string)bte_CaseCode.EditValue;
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Traceability");
		try
		{
			DataTable dataSource = await http.GetAsync("CaseGetDataDoc", new
			{
				CaseCode = caseCode,
				type = type
			});
			gridControl_Case.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			await REFRESH();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task SEARCH()
	{
		try
		{
			if ((FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update) && XtraMessageBox.Show("Data were reset at all, do you want to continue?", "System message", MessageBoxButtons.YesNo) == DialogResult.No)
			{
				return;
			}
			WinCommons._OpenProcessing();
			FormMode = Enums.FormViewModeType.Busy;
			await LoadData(Enums.NavigateType.Current);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_PREVIOUS()
	{
		try
		{
			if ((FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update) && XtraMessageBox.Show("Data were reset at all, do you want to continue?", "System message", MessageBoxButtons.YesNo) == DialogResult.No)
			{
				return;
			}
			WinCommons._OpenProcessing();
			FormMode = Enums.FormViewModeType.Busy;
			await LoadData(Enums.NavigateType.Previous);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_LAST()
	{
		try
		{
			if ((FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update) && XtraMessageBox.Show("Data were reset at all, do you want to continue?", "System message", MessageBoxButtons.YesNo) == DialogResult.No)
			{
				return;
			}
			WinCommons._OpenProcessing();
			FormMode = Enums.FormViewModeType.Busy;
			await LoadData(Enums.NavigateType.Last);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_NEXT()
	{
		try
		{
			if ((FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update) && XtraMessageBox.Show("Data were reset at all, do you want to continue?", "System message", MessageBoxButtons.YesNo) == DialogResult.No)
			{
				return;
			}
			WinCommons._OpenProcessing();
			FormMode = Enums.FormViewModeType.Busy;
			await LoadData(Enums.NavigateType.Next);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_FIRST()
	{
		try
		{
			if ((FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update) && XtraMessageBox.Show("Data were reset at all, do you want to continue?", "System message", MessageBoxButtons.YesNo) == DialogResult.No)
			{
				return;
			}
			WinCommons._OpenProcessing();
			FormMode = Enums.FormViewModeType.Busy;
			await LoadData(Enums.NavigateType.First);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	private void simpleBtn_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void frm_Traceability_FormClosing(object sender, FormClosingEventArgs e)
	{
		if (FormMode == Enums.FormViewModeType.Update)
		{
			if (XtraMessageBox.Show("Do you want to save?", "System Message", MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				Savedt();
				e.Cancel = true;
			}
			else
			{
				e.Cancel = true;
			}
		}
		e.Cancel = false;
	}

	private async void addItemToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			int focusedRowHandle = gridView_Item.RowCount - 1;
			gridView_Item.FocusedRowHandle = focusedRowHandle;
			gridView_Item.AddNewRow();
			FormMode = Enums.FormViewModeType.Update;
			gridView_Item.SetFocusedRowCellValue("CaseCode", _CaseCode);
			using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Traceability");
			int num = Function.ParseInt((await http.GetAsync("GetLastLineNum")).Rows[0]["LineNum"]);
			gridView_Item.SetFocusedRowCellValue("LineNum", num);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void deleteItemToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (XtraMessageBox.Show("Do you want to Delete?", "System message", MessageBoxButtons.YesNo) == DialogResult.Yes)
		{
			gridView_Item.DeleteSelectedRows();
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	private async void gridView_Case_FocusedRowObjectChanged_1(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			_LineNumber = Function.ParseInt(gridView_Case.GetRowCellValue(gridView_Case.FocusedRowHandle, "LineNumber"));
			using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Traceability");
			DataTable dataSource = await http.GetAsync("GetItemByLineNumber", new
			{
				LineNumber = _LineNumber
			});
			gridControl_Item.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gridView_Case_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Update)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gridView_Item_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Update)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gridView_Case_BeforeLeaveRow_1(object sender, RowAllowEventArgs e)
	{
		if (FormMode == Enums.FormViewModeType.Update && XtraMessageBox.Show("Do you want to Save?", "System message", MessageBoxButtons.YesNo) == DialogResult.Yes)
		{
			Savedt();
		}
	}

	private async void deleteCaseToolStripMenuItem_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (XtraMessageBox.Show(Resources.xtraMsg_Delete, "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				_LineNumber = Function.ParseInt(gridView_Case.GetRowCellValue(gridView_Case.FocusedRowHandle, "LineNumber"));
				using HttpClientHelper<string> http = new HttpClientHelper<string>(Configuration.ApiServer, "PrintBillWrap");
				WinCommons._OpenProcessing();
				string text = await http.GetAsync("DelCase", new
				{
					LineNumber = _LineNumber
				});
				if (string.IsNullOrWhiteSpace(text))
				{
					WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, text);
				}
			}
			using (HttpClientHelper<DataTable> http2 = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Traceability"))
			{
				DataTable dataSource = await http2.GetAsync("GetCaseTracibility");
				gridControl_Case.DataSource = dataSource;
			}
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void simpleBtn_Search_Click(object sender, EventArgs e)
	{
		try
		{
			string text = bte_CaseCode.Text.Trim();
			if (text == "")
			{
				text = "-1";
			}
			string text2 = txtBox_typeCode.Text.Trim();
			if (text2 == "")
			{
				text2 = "-1";
			}
			DateTime? postingDate = Function.ParseDateTime(txtBox_PostingDate.EditValue);
			if (!postingDate.HasValue)
			{
				postingDate = Function.ParseDateTime("01/01/1990 12:00:00 AM");
			}
			using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Traceability");
			DataTable dataSource = await http.GetAsync("Search", new
			{
				CaseCode = text,
				TypeCode = text2,
				PostingDate = postingDate
			});
			gridControl_Case.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Packing_Management.frm_Traceability));
		DevExpress.Utils.SerializableAppearanceObject appearance2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled4 = new DevExpress.Utils.SerializableAppearanceObject();
		this.repositoryItemLookUpEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.repositoryItemLookUpEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.labelCtrl_CodeCase = new DevExpress.XtraEditors.LabelControl();
		this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
		this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
		this.simpleBtn_OK = new DevExpress.XtraEditors.SimpleButton();
		this.simpleBtn_Cancel = new DevExpress.XtraEditors.SimpleButton();
		this.contextMenuStrip_Item = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addItemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteItemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.bte_CaseCode = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.behaviorManager1 = new DevExpress.Utils.Behaviors.BehaviorManager(this.components);
		this.gridViewEx1 = new Apzon.Controls.Grid.GridViewEx();
		this.gridControl_Case = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip_Case = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.deleteCaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gridView_Case = new Apzon.Controls.Grid.GridViewEx();
		this.CaseCode = new Apzon.Controls.Grid.GridColumnEx();
		this.CaseName = new Apzon.Controls.Grid.GridColumnEx();
		this.TypeCode = new Apzon.Controls.Grid.GridColumnEx();
		this.Amount = new Apzon.Controls.Grid.GridColumnEx();
		this.Status = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryLUE_Status = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.USERID = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryLUE_USERID = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.CreateDate = new Apzon.Controls.Grid.GridColumnEx();
		this.WhsCode = new Apzon.Controls.Grid.GridColumnEx();
		this.Unit = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryLUE_Unit = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.Remark = new Apzon.Controls.Grid.GridColumnEx();
		this.POCode = new Apzon.Controls.Grid.GridColumnEx();
		this.PostingDate = new Apzon.Controls.Grid.GridColumnEx();
		this.LineNumber = new Apzon.Controls.Grid.GridColumnEx();
		this.gridControl_Item = new Apzon.Controls.Grid.GridControlEx();
		this.gridView_Item = new Apzon.Controls.Grid.GridViewEx();
		this.LineNum = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx5 = new Apzon.Controls.Grid.GridColumnEx();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.txtBox_PostingDate = new Apzon.Controls.Editor.DateEditEx();
		this.simpleBtn_Search = new Apzon.Controls.Controls.SimpleButtonEx();
		this.txtBox_typeCode = new Apzon.Forms.UserControls.ButtonEditCFL();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemLookUpEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemLookUpEdit2).BeginInit();
		this.contextMenuStrip_Item.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.bte_CaseCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.behaviorManager1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridViewEx1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridControl_Case).BeginInit();
		this.contextMenuStrip_Case.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gridView_Case).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryLUE_Status).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryLUE_USERID).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryLUE_Unit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridControl_Item).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridView_Item).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txtBox_PostingDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtBox_PostingDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtBox_typeCode.Properties).BeginInit();
		base.SuspendLayout();
		this.repositoryItemLookUpEdit1.AutoHeight = false;
		this.repositoryItemLookUpEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemLookUpEdit1.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.repositoryItemLookUpEdit1.Name = "repositoryItemLookUpEdit1";
		this.repositoryItemLookUpEdit1.NullText = "";
		this.repositoryItemLookUpEdit2.AutoHeight = false;
		this.repositoryItemLookUpEdit2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemLookUpEdit2.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.repositoryItemLookUpEdit2.Name = "repositoryItemLookUpEdit2";
		this.repositoryItemLookUpEdit2.NullText = "";
		this.labelCtrl_CodeCase.Location = new System.Drawing.Point(12, 11);
		this.labelCtrl_CodeCase.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.labelCtrl_CodeCase.Name = "labelCtrl_CodeCase";
		this.labelCtrl_CodeCase.Size = new System.Drawing.Size(52, 13);
		this.labelCtrl_CodeCase.TabIndex = 2;
		this.labelCtrl_CodeCase.Text = "Code Case";
		this.labelControl2.Location = new System.Drawing.Point(321, 11);
		this.labelControl2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.labelControl2.Name = "labelControl2";
		this.labelControl2.Size = new System.Drawing.Size(52, 13);
		this.labelControl2.TabIndex = 7;
		this.labelControl2.Text = "Type Code";
		this.labelControl4.Location = new System.Drawing.Point(635, 14);
		this.labelControl4.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.labelControl4.Name = "labelControl4";
		this.labelControl4.Size = new System.Drawing.Size(35, 13);
		this.labelControl4.TabIndex = 11;
		this.labelControl4.Text = "Posting";
		this.simpleBtn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.simpleBtn_OK.ImageOptions.ImageUri.Uri = "Apply;Size16x16";
		this.simpleBtn_OK.Location = new System.Drawing.Point(750, 488);
		this.simpleBtn_OK.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.simpleBtn_OK.Name = "simpleBtn_OK";
		this.simpleBtn_OK.Size = new System.Drawing.Size(92, 26);
		this.simpleBtn_OK.TabIndex = 14;
		this.simpleBtn_OK.Text = "OK";
		this.simpleBtn_OK.Click += new System.EventHandler(this.simpleBtn_Save_Click);
		this.simpleBtn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.simpleBtn_Cancel.ImageOptions.ImageUri.Uri = "Cancel;Size16x16";
		this.simpleBtn_Cancel.Location = new System.Drawing.Point(846, 488);
		this.simpleBtn_Cancel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.simpleBtn_Cancel.Name = "simpleBtn_Cancel";
		this.simpleBtn_Cancel.Size = new System.Drawing.Size(92, 26);
		this.simpleBtn_Cancel.TabIndex = 15;
		this.simpleBtn_Cancel.Text = "Cancel";
		this.simpleBtn_Cancel.Click += new System.EventHandler(this.simpleBtn_Cancel_Click);
		this.contextMenuStrip_Item.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addItemToolStripMenuItem, this.deleteItemToolStripMenuItem });
		this.contextMenuStrip_Item.Name = "contextMenuStrip_Item";
		this.contextMenuStrip_Item.Size = new System.Drawing.Size(135, 48);
		this.addItemToolStripMenuItem.Name = "addItemToolStripMenuItem";
		this.addItemToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
		this.addItemToolStripMenuItem.Text = "Add Item";
		this.addItemToolStripMenuItem.Click += new System.EventHandler(this.addItemToolStripMenuItem_Click);
		this.deleteItemToolStripMenuItem.Name = "deleteItemToolStripMenuItem";
		this.deleteItemToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
		this.deleteItemToolStripMenuItem.Text = "Delete Item";
		this.deleteItemToolStripMenuItem.Click += new System.EventHandler(this.deleteItemToolStripMenuItem_Click);
		this.bte_CaseCode.EditControl = null;
		this.bte_CaseCode.FieldName = null;
		this.bte_CaseCode.IsSearchControl = false;
		this.bte_CaseCode.Location = new System.Drawing.Point(71, 8);
		this.bte_CaseCode.Name = "bte_CaseCode";
		this.bte_CaseCode.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.bte_CaseCode.Properties.Appearance.Options.UseFont = true;
		this.bte_CaseCode.Properties.AutoLoadDataSource = true;
		this.bte_CaseCode.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		editorButtonImageOptions.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions2.Image");
		this.bte_CaseCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.bte_CaseCode.Properties.ContainerControl = this;
		this.bte_CaseCode.Properties.DisplayMember = "";
		this.bte_CaseCode.Properties.DisplayMember2 = "";
		this.bte_CaseCode.Properties.LoadDataSourceWhenEnter = false;
		this.bte_CaseCode.Properties.SeparateCharacterForMulti = ';';
		this.bte_CaseCode.Properties.SeparateString = ", ";
		this.bte_CaseCode.Properties.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.GetCase;
		this.bte_CaseCode.Properties.UsingAutoCompleteBox = false;
		this.bte_CaseCode.Properties.UsingDataSource = true;
		this.bte_CaseCode.Properties.ValidateWhenNoDataSource = false;
		this.bte_CaseCode.Properties.ValueMember = "";
		this.bte_CaseCode.SettingInVisible = false;
		this.bte_CaseCode.SettingReadonly = false;
		this.bte_CaseCode.Size = new System.Drawing.Size(111, 22);
		this.bte_CaseCode.TabIndex = 18;
		this.gridViewEx1.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gridViewEx1.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gridViewEx1.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gridViewEx1.ColumnKey = null;
		this.gridViewEx1.DisableFormSetting = false;
		this.gridViewEx1.DisplayException = false;
		this.gridViewEx1.EditColumn = null;
		this.gridViewEx1.FieldCurrency = null;
		this.gridViewEx1.FieldRate = null;
		this.gridViewEx1.FormId = null;
		this.gridViewEx1.IgnoreFilter = false;
		this.gridViewEx1.IndexEditRow = -1;
		this.gridViewEx1.IndicatorWidth = 35;
		this.gridViewEx1.IsApzTable = false;
		this.gridViewEx1.IsInsertNewRow = false;
		this.gridViewEx1.IsLoadingData = false;
		this.gridViewEx1.IsMappingContextAction = false;
		this.gridViewEx1.ItemId = null;
		this.gridViewEx1.LstColumnAllowBatch = null;
		this.gridViewEx1.LstColumnNotAllowBatch = null;
		this.gridViewEx1.LstColumnNotEditable = null;
		this.gridViewEx1.LstColumNotSetting = null;
		this.gridViewEx1.Name = "gridViewEx1";
		this.gridViewEx1.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gridViewEx1.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gridViewEx1.OptionsPrint.AutoWidth = false;
		this.gridViewEx1.OptionsPrint.UsePrintStyles = false;
		this.gridViewEx1.OptionsView.BestFitMaxRowCount = 100;
		this.gridViewEx1.OptionsView.ColumnAutoWidth = false;
		this.gridViewEx1.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridViewEx1.OptionsView.ShowGroupedColumns = true;
		this.gridViewEx1.OptionsView.ShowGroupPanel = false;
		this.gridViewEx1.UseBatchMod = false;
		this.gridControl_Case.AllowPasteExcel = true;
		this.gridControl_Case.ClipboardData = null;
		this.gridControl_Case.ContextMenuStrip = this.contextMenuStrip_Case;
		this.gridControl_Case.Dock = System.Windows.Forms.DockStyle.Fill;
		this.gridControl_Case.Location = new System.Drawing.Point(0, 0);
		this.gridControl_Case.MainView = this.gridView_Case;
		this.gridControl_Case.Name = "gridControl_Case";
		this.gridControl_Case.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.repositoryLUE_Status, this.repositoryLUE_Unit, this.repositoryLUE_USERID });
		this.gridControl_Case.Size = new System.Drawing.Size(926, 194);
		this.gridControl_Case.TabIndex = 27;
		this.gridControl_Case.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gridView_Case });
		this.contextMenuStrip_Case.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.deleteCaseToolStripMenuItem });
		this.contextMenuStrip_Case.Name = "contextMenuStrip_Case";
		this.contextMenuStrip_Case.Size = new System.Drawing.Size(136, 26);
		this.deleteCaseToolStripMenuItem.Name = "deleteCaseToolStripMenuItem";
		this.deleteCaseToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
		this.deleteCaseToolStripMenuItem.Text = "Delete Case";
		this.deleteCaseToolStripMenuItem.Click += new System.EventHandler(this.deleteCaseToolStripMenuItem_Click);
		this.gridView_Case.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gridView_Case.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gridView_Case.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gridView_Case.ColumnKey = null;
		this.gridView_Case.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[13]
		{
			this.CaseCode, this.CaseName, this.TypeCode, this.Amount, this.Status, this.USERID, this.CreateDate, this.WhsCode, this.Unit, this.Remark,
			this.POCode, this.PostingDate, this.LineNumber
		});
		this.gridView_Case.DisableFormSetting = false;
		this.gridView_Case.DisplayException = false;
		this.gridView_Case.EditColumn = null;
		this.gridView_Case.FieldCurrency = null;
		this.gridView_Case.FieldRate = null;
		this.gridView_Case.FormId = "oTraceability";
		this.gridView_Case.GridControl = this.gridControl_Case;
		this.gridView_Case.IgnoreFilter = false;
		this.gridView_Case.IndexEditRow = -1;
		this.gridView_Case.IndicatorWidth = 35;
		this.gridView_Case.IsApzTable = false;
		this.gridView_Case.IsInsertNewRow = false;
		this.gridView_Case.IsLoadingData = false;
		this.gridView_Case.IsMappingContextAction = false;
		this.gridView_Case.ItemId = "oTraceabilityCase";
		this.gridView_Case.LstColumnAllowBatch = null;
		this.gridView_Case.LstColumnNotAllowBatch = null;
		this.gridView_Case.LstColumnNotEditable = null;
		this.gridView_Case.LstColumNotSetting = null;
		this.gridView_Case.Name = "gridView_Case";
		this.gridView_Case.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gridView_Case.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gridView_Case.OptionsPrint.AutoWidth = false;
		this.gridView_Case.OptionsPrint.UsePrintStyles = false;
		this.gridView_Case.OptionsView.BestFitMaxRowCount = 100;
		this.gridView_Case.OptionsView.ColumnAutoWidth = false;
		this.gridView_Case.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridView_Case.OptionsView.ShowGroupedColumns = true;
		this.gridView_Case.OptionsView.ShowGroupPanel = false;
		this.gridView_Case.UseBatchMod = false;
		this.gridView_Case.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gridView_Case_FocusedRowObjectChanged_1);
		this.gridView_Case.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridView_Case_CellValueChanged);
		this.gridView_Case.BeforeLeaveRow += new DevExpress.XtraGrid.Views.Base.RowAllowEventHandler(this.gridView_Case_BeforeLeaveRow_1);
		this.CaseCode.Caption = "Case Code";
		this.CaseCode.CurrencyDateField = null;
		this.CaseCode.CurrencyField = null;
		this.CaseCode.CurrencyFixed = null;
		this.CaseCode.CurrencyFixedRate = 0.0;
		this.CaseCode.CurrencyRateField = null;
		this.CaseCode.FieldName = "CaseCode";
		this.CaseCode.Name = "CaseCode";
		this.CaseCode.OptionsColumn.FixedWidth = true;
		this.CaseCode.PrimaryKey = true;
		this.CaseCode.UnitField = null;
		this.CaseCode.Visible = true;
		this.CaseCode.VisibleIndex = 0;
		this.CaseName.Caption = "Case Name";
		this.CaseName.CurrencyDateField = null;
		this.CaseName.CurrencyField = null;
		this.CaseName.CurrencyFixed = null;
		this.CaseName.CurrencyFixedRate = 0.0;
		this.CaseName.CurrencyRateField = null;
		this.CaseName.FieldName = "CaseName";
		this.CaseName.Name = "CaseName";
		this.CaseName.OptionsColumn.FixedWidth = true;
		this.CaseName.PrimaryKey = false;
		this.CaseName.UnitField = null;
		this.CaseName.Visible = true;
		this.CaseName.VisibleIndex = 1;
		this.TypeCode.Caption = "Type Code";
		this.TypeCode.CurrencyDateField = null;
		this.TypeCode.CurrencyField = null;
		this.TypeCode.CurrencyFixed = null;
		this.TypeCode.CurrencyFixedRate = 0.0;
		this.TypeCode.CurrencyRateField = null;
		this.TypeCode.FieldName = "TypeCode";
		this.TypeCode.Name = "TypeCode";
		this.TypeCode.OptionsColumn.FixedWidth = true;
		this.TypeCode.PrimaryKey = false;
		this.TypeCode.UnitField = null;
		this.TypeCode.Visible = true;
		this.TypeCode.VisibleIndex = 2;
		this.Amount.Caption = "Amount";
		this.Amount.CurrencyDateField = null;
		this.Amount.CurrencyField = null;
		this.Amount.CurrencyFixed = null;
		this.Amount.CurrencyFixedRate = 0.0;
		this.Amount.CurrencyRateField = null;
		this.Amount.FieldName = "Amount";
		this.Amount.Name = "Amount";
		this.Amount.PrimaryKey = false;
		this.Amount.UnitField = null;
		this.Amount.Visible = true;
		this.Amount.VisibleIndex = 3;
		this.Amount.Width = 60;
		this.Status.Caption = "Status";
		this.Status.ColumnEdit = this.repositoryLUE_Status;
		this.Status.CurrencyDateField = null;
		this.Status.CurrencyField = null;
		this.Status.CurrencyFixed = null;
		this.Status.CurrencyFixedRate = 0.0;
		this.Status.CurrencyRateField = null;
		this.Status.FieldName = "Status";
		this.Status.Name = "Status";
		this.Status.PrimaryKey = false;
		this.Status.UnitField = null;
		this.Status.Visible = true;
		this.Status.VisibleIndex = 4;
		this.Status.Width = 60;
		this.repositoryLUE_Status.AutoHeight = false;
		this.repositoryLUE_Status.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryLUE_Status.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Value", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.repositoryLUE_Status.Name = "repositoryLUE_Status";
		this.repositoryLUE_Status.NullText = "";
		this.USERID.Caption = "User Name";
		this.USERID.ColumnEdit = this.repositoryLUE_USERID;
		this.USERID.CurrencyDateField = null;
		this.USERID.CurrencyField = null;
		this.USERID.CurrencyFixed = null;
		this.USERID.CurrencyFixedRate = 0.0;
		this.USERID.CurrencyRateField = null;
		this.USERID.FieldName = "USERID";
		this.USERID.Name = "USERID";
		this.USERID.OptionsColumn.AllowEdit = false;
		this.USERID.PrimaryKey = false;
		this.USERID.UnitField = null;
		this.USERID.Visible = true;
		this.USERID.VisibleIndex = 5;
		this.USERID.Width = 90;
		this.repositoryLUE_USERID.AutoHeight = false;
		this.repositoryLUE_USERID.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryLUE_USERID.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("U_NAME", "U_NAME"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("USERID", "USERID", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.repositoryLUE_USERID.Name = "repositoryLUE_USERID";
		this.repositoryLUE_USERID.NullText = "";
		this.CreateDate.Caption = "Create Date";
		this.CreateDate.CurrencyDateField = null;
		this.CreateDate.CurrencyField = null;
		this.CreateDate.CurrencyFixed = null;
		this.CreateDate.CurrencyFixedRate = 0.0;
		this.CreateDate.CurrencyRateField = null;
		this.CreateDate.FieldName = "CreateDate";
		this.CreateDate.Name = "CreateDate";
		this.CreateDate.OptionsColumn.AllowEdit = false;
		this.CreateDate.PrimaryKey = false;
		this.CreateDate.UnitField = null;
		this.WhsCode.Caption = "Whs Name";
		this.WhsCode.CurrencyDateField = null;
		this.WhsCode.CurrencyField = null;
		this.WhsCode.CurrencyFixed = null;
		this.WhsCode.CurrencyFixedRate = 0.0;
		this.WhsCode.CurrencyRateField = null;
		this.WhsCode.FieldName = "WhsCode";
		this.WhsCode.Name = "WhsCode";
		this.WhsCode.OptionsColumn.FixedWidth = true;
		this.WhsCode.PrimaryKey = false;
		this.WhsCode.UnitField = null;
		this.WhsCode.Visible = true;
		this.WhsCode.VisibleIndex = 6;
		this.WhsCode.Width = 120;
		this.Unit.Caption = "Unit";
		this.Unit.ColumnEdit = this.repositoryLUE_Unit;
		this.Unit.CurrencyDateField = null;
		this.Unit.CurrencyField = null;
		this.Unit.CurrencyFixed = null;
		this.Unit.CurrencyFixedRate = 0.0;
		this.Unit.CurrencyRateField = null;
		this.Unit.FieldName = "Unit";
		this.Unit.Name = "Unit";
		this.Unit.PrimaryKey = false;
		this.Unit.UnitField = null;
		this.Unit.Visible = true;
		this.Unit.VisibleIndex = 7;
		this.Unit.Width = 60;
		this.repositoryLUE_Unit.AutoHeight = false;
		this.repositoryLUE_Unit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryLUE_Unit.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Value", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.repositoryLUE_Unit.Name = "repositoryLUE_Unit";
		this.repositoryLUE_Unit.NullText = "";
		this.Remark.Caption = "Remark";
		this.Remark.CurrencyDateField = null;
		this.Remark.CurrencyField = null;
		this.Remark.CurrencyFixed = null;
		this.Remark.CurrencyFixedRate = 0.0;
		this.Remark.CurrencyRateField = null;
		this.Remark.FieldName = "Remark";
		this.Remark.Name = "Remark";
		this.Remark.PrimaryKey = false;
		this.Remark.UnitField = null;
		this.Remark.Visible = true;
		this.Remark.VisibleIndex = 8;
		this.POCode.Caption = "Packing Order";
		this.POCode.CurrencyDateField = null;
		this.POCode.CurrencyField = null;
		this.POCode.CurrencyFixed = null;
		this.POCode.CurrencyFixedRate = 0.0;
		this.POCode.CurrencyRateField = null;
		this.POCode.FieldName = "POCode";
		this.POCode.Name = "POCode";
		this.POCode.PrimaryKey = false;
		this.POCode.UnitField = null;
		this.POCode.Visible = true;
		this.POCode.VisibleIndex = 9;
		this.PostingDate.Caption = "Posting Date";
		this.PostingDate.CurrencyDateField = null;
		this.PostingDate.CurrencyField = null;
		this.PostingDate.CurrencyFixed = null;
		this.PostingDate.CurrencyFixedRate = 0.0;
		this.PostingDate.CurrencyRateField = null;
		this.PostingDate.FieldName = "PostingDate";
		this.PostingDate.Name = "PostingDate";
		this.PostingDate.PrimaryKey = false;
		this.PostingDate.UnitField = null;
		this.PostingDate.Visible = true;
		this.PostingDate.VisibleIndex = 10;
		this.LineNumber.Caption = "LineNumber";
		this.LineNumber.CurrencyDateField = null;
		this.LineNumber.CurrencyField = null;
		this.LineNumber.CurrencyFixed = null;
		this.LineNumber.CurrencyFixedRate = 0.0;
		this.LineNumber.CurrencyRateField = null;
		this.LineNumber.FieldName = "LineNumber";
		this.LineNumber.Name = "LineNumber";
		this.LineNumber.PrimaryKey = false;
		this.LineNumber.UnitField = null;
		this.gridControl_Item.AllowPasteExcel = true;
		this.gridControl_Item.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.gridControl_Item.ClipboardData = null;
		this.gridControl_Item.ContextMenuStrip = this.contextMenuStrip_Item;
		this.gridControl_Item.Location = new System.Drawing.Point(0, 0);
		this.gridControl_Item.MainView = this.gridView_Item;
		this.gridControl_Item.Name = "gridControl_Item";
		this.gridControl_Item.Size = new System.Drawing.Size(926, 245);
		this.gridControl_Item.TabIndex = 28;
		this.gridControl_Item.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gridView_Item });
		this.gridView_Item.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gridView_Item.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gridView_Item.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gridView_Item.ColumnKey = null;
		this.gridView_Item.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[6] { this.LineNum, this.gridColumnEx1, this.gridColumnEx2, this.gridColumnEx3, this.gridColumnEx4, this.gridColumnEx5 });
		this.gridView_Item.DisableFormSetting = false;
		this.gridView_Item.DisplayException = false;
		this.gridView_Item.EditColumn = null;
		this.gridView_Item.FieldCurrency = null;
		this.gridView_Item.FieldRate = null;
		this.gridView_Item.FormId = "oTraceabilitycase";
		this.gridView_Item.GridControl = this.gridControl_Item;
		this.gridView_Item.IgnoreFilter = false;
		this.gridView_Item.IndexEditRow = -1;
		this.gridView_Item.IndicatorWidth = 35;
		this.gridView_Item.IsApzTable = false;
		this.gridView_Item.IsInsertNewRow = false;
		this.gridView_Item.IsLoadingData = false;
		this.gridView_Item.IsMappingContextAction = false;
		this.gridView_Item.ItemId = "oTraceabilityItem";
		this.gridView_Item.LstColumnAllowBatch = null;
		this.gridView_Item.LstColumnNotAllowBatch = null;
		this.gridView_Item.LstColumnNotEditable = null;
		this.gridView_Item.LstColumNotSetting = null;
		this.gridView_Item.Name = "gridView_Item";
		this.gridView_Item.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gridView_Item.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gridView_Item.OptionsPrint.AutoWidth = false;
		this.gridView_Item.OptionsPrint.UsePrintStyles = false;
		this.gridView_Item.OptionsView.BestFitMaxRowCount = 100;
		this.gridView_Item.OptionsView.ColumnAutoWidth = false;
		this.gridView_Item.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridView_Item.OptionsView.ShowGroupedColumns = true;
		this.gridView_Item.OptionsView.ShowGroupPanel = false;
		this.gridView_Item.UseBatchMod = false;
		this.LineNum.Caption = "LineNum";
		this.LineNum.CurrencyDateField = null;
		this.LineNum.CurrencyField = null;
		this.LineNum.CurrencyFixed = null;
		this.LineNum.CurrencyFixedRate = 0.0;
		this.LineNum.CurrencyRateField = null;
		this.LineNum.FieldName = "LineNum";
		this.LineNum.Name = "LineNum";
		this.LineNum.PrimaryKey = true;
		this.LineNum.UnitField = null;
		this.gridColumnEx1.Caption = "Case Code";
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "CaseCode";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.PrimaryKey = false;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx2.Caption = "Item Code";
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.FieldName = "BundleCode";
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.UnitField = null;
		this.gridColumnEx2.Visible = true;
		this.gridColumnEx2.VisibleIndex = 0;
		this.gridColumnEx3.Caption = "Item Name";
		this.gridColumnEx3.CurrencyDateField = null;
		this.gridColumnEx3.CurrencyField = null;
		this.gridColumnEx3.CurrencyFixed = null;
		this.gridColumnEx3.CurrencyFixedRate = 0.0;
		this.gridColumnEx3.CurrencyRateField = null;
		this.gridColumnEx3.FieldName = "BundleName";
		this.gridColumnEx3.Name = "gridColumnEx3";
		this.gridColumnEx3.PrimaryKey = false;
		this.gridColumnEx3.UnitField = null;
		this.gridColumnEx3.Visible = true;
		this.gridColumnEx3.VisibleIndex = 1;
		this.gridColumnEx4.Caption = "Lot Number";
		this.gridColumnEx4.CurrencyDateField = null;
		this.gridColumnEx4.CurrencyField = null;
		this.gridColumnEx4.CurrencyFixed = null;
		this.gridColumnEx4.CurrencyFixedRate = 0.0;
		this.gridColumnEx4.CurrencyRateField = null;
		this.gridColumnEx4.FieldName = "LotNum";
		this.gridColumnEx4.Name = "gridColumnEx4";
		this.gridColumnEx4.PrimaryKey = false;
		this.gridColumnEx4.UnitField = null;
		this.gridColumnEx4.Visible = true;
		this.gridColumnEx4.VisibleIndex = 2;
		this.gridColumnEx5.Caption = "Whs Name";
		this.gridColumnEx5.CurrencyDateField = null;
		this.gridColumnEx5.CurrencyField = null;
		this.gridColumnEx5.CurrencyFixed = null;
		this.gridColumnEx5.CurrencyFixedRate = 0.0;
		this.gridColumnEx5.CurrencyRateField = null;
		this.gridColumnEx5.FieldName = "WhsCode";
		this.gridColumnEx5.Name = "gridColumnEx5";
		this.gridColumnEx5.PrimaryKey = false;
		this.gridColumnEx5.UnitField = null;
		this.gridColumnEx5.Visible = true;
		this.gridColumnEx5.VisibleIndex = 3;
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(12, 39);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.gridControl_Case);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.gridControl_Item);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(926, 444);
		this.splitContainerControl1.SplitterPosition = 194;
		this.splitContainerControl1.TabIndex = 29;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.txtBox_PostingDate.EditControl = null;
		this.txtBox_PostingDate.EditValue = new System.DateTime(2019, 11, 29, 12, 3, 7, 949);
		this.txtBox_PostingDate.FieldName = null;
		this.txtBox_PostingDate.Location = new System.Drawing.Point(676, 10);
		this.txtBox_PostingDate.Name = "txtBox_PostingDate";
		this.txtBox_PostingDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtBox_PostingDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtBox_PostingDate.Properties.IsSearchControl = false;
		this.txtBox_PostingDate.SettingInVisible = false;
		this.txtBox_PostingDate.SettingReadonly = false;
		this.txtBox_PostingDate.Size = new System.Drawing.Size(104, 20);
		this.txtBox_PostingDate.TabIndex = 19;
		this.simpleBtn_Search.EditControl = null;
		this.simpleBtn_Search.EditValue = "Search";
		this.simpleBtn_Search.FieldName = null;
		this.simpleBtn_Search.ImageOptions.ImageUri.Uri = "Zoom;Size16x16";
		this.simpleBtn_Search.IsSearchControl = false;
		this.simpleBtn_Search.Location = new System.Drawing.Point(863, 8);
		this.simpleBtn_Search.Name = "simpleBtn_Search";
		this.simpleBtn_Search.ReadOnly = false;
		this.simpleBtn_Search.SettingInVisible = false;
		this.simpleBtn_Search.SettingReadonly = false;
		this.simpleBtn_Search.Size = new System.Drawing.Size(75, 23);
		this.simpleBtn_Search.TabIndex = 30;
		this.simpleBtn_Search.Text = "Search";
		this.simpleBtn_Search.Click += new System.EventHandler(this.simpleBtn_Search_Click);
		this.txtBox_typeCode.EditControl = null;
		this.txtBox_typeCode.FieldName = null;
		this.txtBox_typeCode.IsSearchControl = false;
		this.txtBox_typeCode.Location = new System.Drawing.Point(379, 10);
		this.txtBox_typeCode.Name = "txtBox_typeCode";
		this.txtBox_typeCode.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtBox_typeCode.Properties.Appearance.Options.UseFont = true;
		this.txtBox_typeCode.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions2.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions4.Image");
		this.txtBox_typeCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance4, appearanceHovered4, appearancePressed4, appearanceDisabled4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.txtBox_typeCode.Properties.ContainerControl = this;
		this.txtBox_typeCode.Properties.DisplayMember = "";
		this.txtBox_typeCode.Properties.DisplayMember2 = "";
		this.txtBox_typeCode.Properties.LoadDataSourceWhenEnter = false;
		this.txtBox_typeCode.Properties.SeparateCharacterForMulti = ';';
		this.txtBox_typeCode.Properties.SeparateString = ", ";
		this.txtBox_typeCode.Properties.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.GetTypeCase;
		this.txtBox_typeCode.Properties.UsingAutoCompleteBox = false;
		this.txtBox_typeCode.Properties.UsingDataSource = true;
		this.txtBox_typeCode.Properties.ValidateWhenNoDataSource = false;
		this.txtBox_typeCode.Properties.ValueMember = "";
		this.txtBox_typeCode.SettingInVisible = false;
		this.txtBox_typeCode.SettingReadonly = false;
		this.txtBox_typeCode.Size = new System.Drawing.Size(100, 20);
		this.txtBox_typeCode.TabIndex = 31;
		base.Appearance.Options.UseFont = true;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(952, 525);
		base.Controls.Add(this.simpleBtn_Search);
		base.Controls.Add(this.splitContainerControl1);
		base.Controls.Add(this.bte_CaseCode);
		base.Controls.Add(this.simpleBtn_Cancel);
		base.Controls.Add(this.simpleBtn_OK);
		base.Controls.Add(this.labelControl4);
		base.Controls.Add(this.labelControl2);
		base.Controls.Add(this.labelCtrl_CodeCase);
		base.Controls.Add(this.txtBox_PostingDate);
		base.Controls.Add(this.txtBox_typeCode);
		this.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		base.FormId = "oTraceability";
		base.IconOptions.ShowIcon = false;
		base.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		base.Name = "frm_Traceability";
		this.Text = "Traceability";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_Traceability_FormClosing);
		base.Load += new System.EventHandler(this.frm_Traceability_Load);
		((System.ComponentModel.ISupportInitialize)this.repositoryItemLookUpEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemLookUpEdit2).EndInit();
		this.contextMenuStrip_Item.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.bte_CaseCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.behaviorManager1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridViewEx1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridControl_Case).EndInit();
		this.contextMenuStrip_Case.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gridView_Case).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryLUE_Status).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryLUE_USERID).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryLUE_Unit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridControl_Item).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridView_Item).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txtBox_PostingDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtBox_PostingDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtBox_typeCode.Properties).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
