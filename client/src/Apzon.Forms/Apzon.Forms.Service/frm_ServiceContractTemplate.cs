using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using Apzon.Forms.UserControls;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.Service;

public class frm_ServiceContractTemplate : XtraFormEx
{
	private bool _isAddNew;

	private bool _isSearch;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private IContainer components;

	private SimpleButtonEx btn_OK;

	private SimpleButtonEx btn_Cancel;

	private GridControlEx grd_ServiceContractTemplate;

	private GridViewEx gv_ServiceContractTemplate;

	private GridColumnEx gc_Type;

	private RepositoryItemLookUpEdit lookup_Type;

	private GridColumnEx gc_ContractTemplateNo;

	private GridColumnEx gc_Desc;

	private GridColumnEx gc_ContractCycle;

	private GridColumnEx gc_TolerantTime;

	private GridColumnEx gc_Status;

	private RepositoryItemLookUpEdit lookup_Status;

	private GridColumnEx gc_SLA;

	private GridColumnEx gc_Remark;

	private GridColumnEx gc_CycleUnit;

	private RepositoryItemLookUpEdit lookup_CycleUnit;

	private GridColumnEx gc_TolerantUnit;

	private RepositoryItemLookUpEdit lookup_TolerantUnit;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx1;

	private RepositoryItemButtonEditCFL bte_SLA;

	private RepositoryItemTextEdit txt_ContractCycle;

	private RepositoryItemTextEdit txt_TolerantTime;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem addToolStripMenuItem;

	private ToolStripMenuItem removeToolStripMenuItem;

	private LabelLine labelLine5;

	private LabelLine labelLine3;

	private LabelLine labelLine2;

	private LabelLine lb_Type;

	private ButtonEditCFL txt_CTNum;

	private LookUpEditEx txt_Status;

	private ButtonEditCFL txt_SLA;

	private LookUpEditEx txt_Type;

	private SimpleButtonEx btSearch;

	private int contractID { get; set; }

	public string contractNo { get; set; }

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.View:
			btn_OK.Text = "OK";
			break;
		case Enums.FormViewModeType.Update:
			btn_OK.Text = "Update";
			break;
		}
	}

	public frm_ServiceContractTemplate()
	{
		InitializeComponent();
		OnLoadAsync += frm_ServiceContractTemplate_Load;
	}

	private async Task frm_ServiceContractTemplate_Load(object sender, EventArgs e)
	{
		try
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>
			{
				{ "S", "Sales" },
				{ "P", "Purchase" }
			};
			lookup_Type.DataSource = dictionary.Select((KeyValuePair<string, string> r) => new
			{
				ID = r.Key,
				Name = r.Value
			});
			txt_Type.Properties.DataSource = dictionary;
			Dictionary<string, string> dictionary2 = new Dictionary<string, string>
			{
				{ "D", "Draft" },
				{ "A", "Active" },
				{ "T", "Terminated" },
				{ "O", "On Hold" }
			};
			lookup_Status.DataSource = dictionary2.Select((KeyValuePair<string, string> r) => new
			{
				ID = r.Key,
				Name = r.Value
			});
			txt_Status.Properties.DataSource = dictionary2;
			Dictionary<string, string> source = new Dictionary<string, string>
			{
				{ "D", "Day" },
				{ "M", "Month" },
				{ "Y", "Year" }
			};
			RepositoryItemLookUpEdit repositoryItemLookUpEdit = lookup_TolerantUnit;
			object dataSource = (lookup_CycleUnit.DataSource = source.Select((KeyValuePair<string, string> r) => new
			{
				ID = r.Key,
				Name = r.Value
			}));
			repositoryItemLookUpEdit.DataSource = dataSource;
			await LoadServiceContractTemplate();
			gv_ServiceContractTemplate.OptionsView.ColumnAutoWidth = true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadServiceContractTemplate()
	{
		try
		{
			txt_Type.EditValue = null;
			txt_CTNum.ResetText();
			txt_Status.EditValue = null;
			txt_SLA.ResetText();
			Search();
			FormMode = Enums.FormViewModeType.View;
			_isAddNew = false;
			_isSearch = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public void AddNewRow()
	{
		try
		{
			if (!_isAddNew)
			{
				FormMode = Enums.FormViewModeType.Update;
				gv_ServiceContractTemplate.AddNewRow();
				int focusedRowHandle = gv_ServiceContractTemplate.FocusedRowHandle;
				if (gv_ServiceContractTemplate.IsNewItemRow(focusedRowHandle))
				{
					gv_ServiceContractTemplate.SetRowCellValue(focusedRowHandle, gv_ServiceContractTemplate.Columns["Type"], "S");
					gv_ServiceContractTemplate.SetRowCellValue(focusedRowHandle, gv_ServiceContractTemplate.Columns["Status"], "A");
				}
			}
			_isAddNew = true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void Search()
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Type");
			dataTable.Columns.Add("ContractTemplateNum");
			dataTable.Columns.Add("SLACode");
			dataTable.Columns.Add("Status");
			DataRow dataRow = dataTable.NewRow();
			dataRow["Type"] = Function.ToString(txt_Type.EditValue);
			if (!string.IsNullOrEmpty(contractNo))
			{
				txt_CTNum.EditValue = contractNo;
			}
			dataRow["ContractTemplateNum"] = Function.ToString(txt_CTNum.EditValue);
			dataRow["SLACode"] = Function.ToString(txt_SLA.EditValue);
			dataRow["Status"] = Function.ToString(txt_Status.EditValue);
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			DataTable dataSource = await _httpClient.ServiceContractTemplateClient.SearchData(dataTable.Copy());
			contractNo = "";
			grd_ServiceContractTemplate.DataSource = dataSource;
			_isSearch = true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadData(Enums.NavigateType type)
	{
		try
		{
			if ((grd_ServiceContractTemplate.DataSource as DataTable).IsNotNull())
			{
				int focusedRowHandle = gv_ServiceContractTemplate.FocusedRowHandle;
				DataRow row = ((DataRowView)gv_ServiceContractTemplate.GetRow(focusedRowHandle)).Row;
				contractID = Function.ParseInt(row["ContractID"]);
			}
			else
			{
				contractID = 0;
			}
			DataTable dataSource = await _httpClient.ServiceContractTemplateClient.GetDataDoc(contractID, type);
			grd_ServiceContractTemplate.DataSource = dataSource;
			grd_ServiceContractTemplate.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_ServiceContractTemplate_CellValueChanged(object sender, CellValueChangedEventArgs e)
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

	private async void btn_OK_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (FormMode == Enums.FormViewModeType.Update)
			{
				DataTable dataTable = grd_ServiceContractTemplate.DataSource as DataTable;
				HttpResult httpResult = await _httpClient.ServiceContractTemplateClient.SaveData(dataTable.Copy());
				if (httpResult.Message == "")
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
					if (_isSearch)
					{
						Search();
						_isAddNew = false;
						FormMode = Enums.FormViewModeType.View;
					}
					else
					{
						await LoadServiceContractTemplate();
					}
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				}
			}
			else
			{
				Close();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void addToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			AddNewRow();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void removeToolStripMenuItem_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) != DialogResult.No)
			{
				DataTable dataTable = gv_ServiceContractTemplate.GetListDataSourceSelectedRow().CopyToDataTable();
				HttpResult httpResult = await _httpClient.ServiceContractTemplateClient.DeleteData(dataTable.Copy());
				if (httpResult.Message == "")
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				}
				await LoadServiceContractTemplate();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btSearch_Click(object sender, EventArgs e)
	{
		try
		{
			Search();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void lookup_Status_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			string text = Function.ToString((sender as LookUpEdit).EditValue);
			object rowCellValue = gv_ServiceContractTemplate.GetRowCellValue(gv_ServiceContractTemplate.FocusedRowHandle, gc_Status);
			if (text == "T" && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmChangeStatusTerminated, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
			{
				gv_ServiceContractTemplate.SetRowCellValue(gv_ServiceContractTemplate.FocusedRowHandle, gc_Status, rowCellValue);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task MODE_FIND()
	{
		try
		{
			if ((FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
			{
				return;
			}
			WinCommons._OpenProcessing();
			FormMode = Enums.FormViewModeType.Busy;
			Search();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	public async Task REFRESH()
	{
		try
		{
			AddNewRow();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task MOVE_PREVIOUS()
	{
		try
		{
			if ((FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
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
			if ((FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
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
			if ((FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
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
			if ((FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
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

	public async Task REFRESH_DATA()
	{
		try
		{
			await LoadServiceContractTemplate();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void frm_ServiceContractTemplate_FormClosing(object sender, FormClosingEventArgs e)
	{
		if (btn_OK.Text == Resources.ButtonUpdate && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
		{
			btn_OK_Click(null, null);
		}
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		Close();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Service.frm_ServiceContractTemplate));
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled6 = new DevExpress.Utils.SerializableAppearanceObject();
		this.repositoryItemCalcEditEx1 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_ServiceContractTemplate = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_ServiceContractTemplate = new Apzon.Controls.Grid.GridViewEx();
		this.gc_Type = new Apzon.Controls.Grid.GridColumnEx();
		this.lookup_Type = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gc_ContractTemplateNo = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Desc = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_ContractCycle = new Apzon.Controls.Grid.GridColumnEx();
		this.txt_ContractCycle = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.gc_CycleUnit = new Apzon.Controls.Grid.GridColumnEx();
		this.lookup_CycleUnit = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gc_TolerantTime = new Apzon.Controls.Grid.GridColumnEx();
		this.txt_TolerantTime = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.gc_TolerantUnit = new Apzon.Controls.Grid.GridColumnEx();
		this.lookup_TolerantUnit = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gc_Status = new Apzon.Controls.Grid.GridColumnEx();
		this.lookup_Status = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gc_SLA = new Apzon.Controls.Grid.GridColumnEx();
		this.bte_SLA = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gc_Remark = new Apzon.Controls.Grid.GridColumnEx();
		this.lb_Type = new Apzon.Controls.Controls.LabelLine();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine5 = new Apzon.Controls.Controls.LabelLine();
		this.txt_CTNum = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.txt_Type = new Apzon.Controls.Editor.LookUpEditEx();
		this.txt_SLA = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.txt_Status = new Apzon.Controls.Editor.LookUpEditEx();
		this.btSearch = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_ServiceContractTemplate).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_ServiceContractTemplate).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lookup_Type).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ContractCycle).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lookup_CycleUnit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_TolerantTime).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lookup_TolerantUnit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lookup_Status).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bte_SLA).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_CTNum.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Type.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_SLA.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Status.Properties).BeginInit();
		base.SuspendLayout();
		this.repositoryItemCalcEditEx1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx1.IsSearchControl = false;
		this.repositoryItemCalcEditEx1.Name = "repositoryItemCalcEditEx1";
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.EditControl = null;
		this.btn_OK.EditValue = "OK";
		this.btn_OK.FieldName = null;
		this.btn_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_OK.ImageOptions.Image");
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(9, 356);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.ReadOnly = false;
		this.btn_OK.SettingInVisible = false;
		this.btn_OK.SettingReadonly = false;
		this.btn_OK.Size = new System.Drawing.Size(75, 23);
		this.btn_OK.TabIndex = 9;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(90, 356);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 10;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.grd_ServiceContractTemplate.AllowPasteExcel = true;
		this.grd_ServiceContractTemplate.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_ServiceContractTemplate.ClipboardData = null;
		this.grd_ServiceContractTemplate.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_ServiceContractTemplate.Location = new System.Drawing.Point(0, 63);
		this.grd_ServiceContractTemplate.MainView = this.gv_ServiceContractTemplate;
		this.grd_ServiceContractTemplate.Name = "grd_ServiceContractTemplate";
		this.grd_ServiceContractTemplate.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[7] { this.lookup_Type, this.lookup_Status, this.lookup_CycleUnit, this.lookup_TolerantUnit, this.bte_SLA, this.txt_ContractCycle, this.txt_TolerantTime });
		this.grd_ServiceContractTemplate.Size = new System.Drawing.Size(768, 287);
		this.grd_ServiceContractTemplate.TabIndex = 11;
		this.grd_ServiceContractTemplate.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_ServiceContractTemplate });
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addToolStripMenuItem, this.removeToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(118, 48);
		this.addToolStripMenuItem.Name = "addToolStripMenuItem";
		this.addToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
		this.addToolStripMenuItem.Text = "Add";
		this.addToolStripMenuItem.Click += new System.EventHandler(this.addToolStripMenuItem_Click);
		this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
		this.removeToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
		this.removeToolStripMenuItem.Text = "Remove";
		this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
		this.gv_ServiceContractTemplate.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_ServiceContractTemplate.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_ServiceContractTemplate.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_ServiceContractTemplate.ColumnKey = null;
		this.gv_ServiceContractTemplate.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[10] { this.gc_Type, this.gc_ContractTemplateNo, this.gc_Desc, this.gc_ContractCycle, this.gc_CycleUnit, this.gc_TolerantTime, this.gc_TolerantUnit, this.gc_Status, this.gc_SLA, this.gc_Remark });
		this.gv_ServiceContractTemplate.DisableFormSetting = false;
		this.gv_ServiceContractTemplate.DisplayException = false;
		this.gv_ServiceContractTemplate.EditColumn = null;
		this.gv_ServiceContractTemplate.FieldCurrency = null;
		this.gv_ServiceContractTemplate.FieldRate = null;
		this.gv_ServiceContractTemplate.FormId = "oServiceContractTemplate";
		this.gv_ServiceContractTemplate.GridControl = this.grd_ServiceContractTemplate;
		this.gv_ServiceContractTemplate.IgnoreFilter = false;
		this.gv_ServiceContractTemplate.IndexEditRow = -1;
		this.gv_ServiceContractTemplate.IndicatorWidth = 35;
		this.gv_ServiceContractTemplate.IsApzTable = false;
		this.gv_ServiceContractTemplate.IsInsertNewRow = false;
		this.gv_ServiceContractTemplate.IsLoadingData = false;
		this.gv_ServiceContractTemplate.IsMappingContextAction = false;
		this.gv_ServiceContractTemplate.ItemId = "APZ_OCTT";
		this.gv_ServiceContractTemplate.LstColumnAllowBatch = null;
		this.gv_ServiceContractTemplate.LstColumnNotAllowBatch = null;
		this.gv_ServiceContractTemplate.LstColumnNotEditable = null;
		this.gv_ServiceContractTemplate.LstColumNotSetting = null;
		this.gv_ServiceContractTemplate.Name = "gv_ServiceContractTemplate";
		this.gv_ServiceContractTemplate.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_ServiceContractTemplate.OptionsSelection.MultiSelect = true;
		this.gv_ServiceContractTemplate.OptionsView.BestFitMaxRowCount = 100;
		this.gv_ServiceContractTemplate.OptionsView.ColumnAutoWidth = false;
		this.gv_ServiceContractTemplate.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_ServiceContractTemplate.OptionsView.ShowGroupPanel = false;
		this.gv_ServiceContractTemplate.UseBatchMod = false;
		this.gv_ServiceContractTemplate.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_ServiceContractTemplate_CellValueChanged);
		this.gc_Type.Caption = "Type";
		this.gc_Type.ColumnEdit = this.lookup_Type;
		this.gc_Type.CurrencyDateField = null;
		this.gc_Type.CurrencyField = null;
		this.gc_Type.CurrencyFixed = null;
		this.gc_Type.CurrencyFixedRate = 0.0;
		this.gc_Type.CurrencyRateField = null;
		this.gc_Type.FieldName = "Type";
		this.gc_Type.Name = "gc_Type";
		this.gc_Type.PrimaryKey = true;
		this.gc_Type.UnitField = null;
		this.gc_Type.Visible = true;
		this.gc_Type.VisibleIndex = 0;
		this.lookup_Type.AutoHeight = false;
		this.lookup_Type.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.lookup_Type.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ID", "Name5"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name6")
		});
		this.lookup_Type.DisplayMember = "Name";
		this.lookup_Type.Name = "lookup_Type";
		this.lookup_Type.NullText = "";
		this.lookup_Type.ValueMember = "ID";
		this.gc_ContractTemplateNo.Caption = "Contract Template No";
		this.gc_ContractTemplateNo.CurrencyDateField = null;
		this.gc_ContractTemplateNo.CurrencyField = null;
		this.gc_ContractTemplateNo.CurrencyFixed = null;
		this.gc_ContractTemplateNo.CurrencyFixedRate = 0.0;
		this.gc_ContractTemplateNo.CurrencyRateField = null;
		this.gc_ContractTemplateNo.FieldName = "ContractTemplateNum";
		this.gc_ContractTemplateNo.Name = "gc_ContractTemplateNo";
		this.gc_ContractTemplateNo.PrimaryKey = false;
		this.gc_ContractTemplateNo.UnitField = null;
		this.gc_ContractTemplateNo.Visible = true;
		this.gc_ContractTemplateNo.VisibleIndex = 1;
		this.gc_Desc.Caption = "Desc";
		this.gc_Desc.CurrencyDateField = null;
		this.gc_Desc.CurrencyField = null;
		this.gc_Desc.CurrencyFixed = null;
		this.gc_Desc.CurrencyFixedRate = 0.0;
		this.gc_Desc.CurrencyRateField = null;
		this.gc_Desc.FieldName = "Description";
		this.gc_Desc.Name = "gc_Desc";
		this.gc_Desc.PrimaryKey = false;
		this.gc_Desc.UnitField = null;
		this.gc_Desc.Visible = true;
		this.gc_Desc.VisibleIndex = 2;
		this.gc_ContractCycle.Caption = "Contract Cycle";
		this.gc_ContractCycle.ColumnEdit = this.txt_ContractCycle;
		this.gc_ContractCycle.CurrencyDateField = null;
		this.gc_ContractCycle.CurrencyField = null;
		this.gc_ContractCycle.CurrencyFixed = null;
		this.gc_ContractCycle.CurrencyFixedRate = 0.0;
		this.gc_ContractCycle.CurrencyRateField = null;
		this.gc_ContractCycle.FieldName = "ContractCycle";
		this.gc_ContractCycle.Name = "gc_ContractCycle";
		this.gc_ContractCycle.PrimaryKey = false;
		this.gc_ContractCycle.UnitField = null;
		this.gc_ContractCycle.Visible = true;
		this.gc_ContractCycle.VisibleIndex = 3;
		this.txt_ContractCycle.AutoHeight = false;
		this.txt_ContractCycle.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
		this.txt_ContractCycle.Name = "txt_ContractCycle";
		this.gc_CycleUnit.Caption = "Cycle Unit";
		this.gc_CycleUnit.ColumnEdit = this.lookup_CycleUnit;
		this.gc_CycleUnit.CurrencyDateField = null;
		this.gc_CycleUnit.CurrencyField = null;
		this.gc_CycleUnit.CurrencyFixed = null;
		this.gc_CycleUnit.CurrencyFixedRate = 0.0;
		this.gc_CycleUnit.CurrencyRateField = null;
		this.gc_CycleUnit.FieldName = "CycleUnit";
		this.gc_CycleUnit.Name = "gc_CycleUnit";
		this.gc_CycleUnit.PrimaryKey = false;
		this.gc_CycleUnit.UnitField = null;
		this.gc_CycleUnit.Visible = true;
		this.gc_CycleUnit.VisibleIndex = 4;
		this.lookup_CycleUnit.AutoHeight = false;
		this.lookup_CycleUnit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.lookup_CycleUnit.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ID", "Name9"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name10")
		});
		this.lookup_CycleUnit.DisplayMember = "Name";
		this.lookup_CycleUnit.Name = "lookup_CycleUnit";
		this.lookup_CycleUnit.NullText = "";
		this.lookup_CycleUnit.ValueMember = "ID";
		this.gc_TolerantTime.Caption = "Tolerant Time";
		this.gc_TolerantTime.ColumnEdit = this.txt_TolerantTime;
		this.gc_TolerantTime.CurrencyDateField = null;
		this.gc_TolerantTime.CurrencyField = null;
		this.gc_TolerantTime.CurrencyFixed = null;
		this.gc_TolerantTime.CurrencyFixedRate = 0.0;
		this.gc_TolerantTime.CurrencyRateField = null;
		this.gc_TolerantTime.FieldName = "TolerantTime";
		this.gc_TolerantTime.Name = "gc_TolerantTime";
		this.gc_TolerantTime.PrimaryKey = false;
		this.gc_TolerantTime.UnitField = null;
		this.gc_TolerantTime.Visible = true;
		this.gc_TolerantTime.VisibleIndex = 5;
		this.txt_TolerantTime.AutoHeight = false;
		this.txt_TolerantTime.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
		this.txt_TolerantTime.Name = "txt_TolerantTime";
		this.gc_TolerantUnit.Caption = "Tolerant Unit";
		this.gc_TolerantUnit.ColumnEdit = this.lookup_TolerantUnit;
		this.gc_TolerantUnit.CurrencyDateField = null;
		this.gc_TolerantUnit.CurrencyField = null;
		this.gc_TolerantUnit.CurrencyFixed = null;
		this.gc_TolerantUnit.CurrencyFixedRate = 0.0;
		this.gc_TolerantUnit.CurrencyRateField = null;
		this.gc_TolerantUnit.FieldName = "TolerantUnit";
		this.gc_TolerantUnit.Name = "gc_TolerantUnit";
		this.gc_TolerantUnit.PrimaryKey = false;
		this.gc_TolerantUnit.UnitField = null;
		this.gc_TolerantUnit.Visible = true;
		this.gc_TolerantUnit.VisibleIndex = 6;
		this.lookup_TolerantUnit.AutoHeight = false;
		this.lookup_TolerantUnit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.lookup_TolerantUnit.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ID", "Name11"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name12")
		});
		this.lookup_TolerantUnit.DisplayMember = "Name";
		this.lookup_TolerantUnit.Name = "lookup_TolerantUnit";
		this.lookup_TolerantUnit.NullText = "";
		this.lookup_TolerantUnit.ValueMember = "ID";
		this.gc_Status.Caption = "Status";
		this.gc_Status.ColumnEdit = this.lookup_Status;
		this.gc_Status.CurrencyDateField = null;
		this.gc_Status.CurrencyField = null;
		this.gc_Status.CurrencyFixed = null;
		this.gc_Status.CurrencyFixedRate = 0.0;
		this.gc_Status.CurrencyRateField = null;
		this.gc_Status.FieldName = "Status";
		this.gc_Status.Name = "gc_Status";
		this.gc_Status.PrimaryKey = false;
		this.gc_Status.UnitField = null;
		this.gc_Status.Visible = true;
		this.gc_Status.VisibleIndex = 7;
		this.lookup_Status.AutoHeight = false;
		this.lookup_Status.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.lookup_Status.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ID", "Name7"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name8")
		});
		this.lookup_Status.DisplayMember = "Name";
		this.lookup_Status.Name = "lookup_Status";
		this.lookup_Status.NullText = "";
		this.lookup_Status.ValueMember = "ID";
		this.lookup_Status.EditValueChanged += new System.EventHandler(this.lookup_Status_EditValueChanged);
		this.gc_SLA.Caption = "SLA";
		this.gc_SLA.ColumnEdit = this.bte_SLA;
		this.gc_SLA.CurrencyDateField = null;
		this.gc_SLA.CurrencyField = null;
		this.gc_SLA.CurrencyFixed = null;
		this.gc_SLA.CurrencyFixedRate = 0.0;
		this.gc_SLA.CurrencyRateField = null;
		this.gc_SLA.FieldName = "SLACode";
		this.gc_SLA.Name = "gc_SLA";
		this.gc_SLA.PrimaryKey = false;
		this.gc_SLA.UnitField = null;
		this.gc_SLA.Visible = true;
		this.gc_SLA.VisibleIndex = 8;
		this.bte_SLA.AutoHeight = false;
		this.bte_SLA.AutoLoadDataSource = true;
		editorButtonImageOptions.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions2.Image");
		this.bte_SLA.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.bte_SLA.ContainerControl = this;
		this.bte_SLA.DisplayMember = "SLAID";
		this.bte_SLA.DisplayMember2 = "";
		this.bte_SLA.LoadDataSourceWhenEnter = false;
		this.bte_SLA.Name = "bte_SLA";
		this.bte_SLA.SeparateCharacterForMulti = ';';
		this.bte_SLA.SeparateString = ", ";
		this.bte_SLA.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.ServiceLevelAgreement;
		this.bte_SLA.TypeDrillDown = Apzon.Commons.ApzObjectTypes.oServiceLevelAgreement;
		this.bte_SLA.UsingAutoCompleteBox = false;
		this.bte_SLA.UsingDataSource = true;
		this.bte_SLA.ValidateWhenNoDataSource = false;
		this.bte_SLA.ValueMember = "SLAID";
		this.gc_Remark.Caption = "Remark";
		this.gc_Remark.CurrencyDateField = null;
		this.gc_Remark.CurrencyField = null;
		this.gc_Remark.CurrencyFixed = null;
		this.gc_Remark.CurrencyFixedRate = 0.0;
		this.gc_Remark.CurrencyRateField = null;
		this.gc_Remark.FieldName = "Remark";
		this.gc_Remark.Name = "gc_Remark";
		this.gc_Remark.PrimaryKey = false;
		this.gc_Remark.UnitField = null;
		this.gc_Remark.Visible = true;
		this.gc_Remark.VisibleIndex = 9;
		this.lb_Type.EditControl = null;
		this.lb_Type.EditValue = "Type";
		this.lb_Type.FieldName = null;
		this.lb_Type.HideWhenEmpty = false;
		this.lb_Type.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Type.Location = new System.Drawing.Point(12, 11);
		this.lb_Type.Name = "lb_Type";
		this.lb_Type.ReadOnly = false;
		this.lb_Type.SettingInVisible = false;
		this.lb_Type.SettingReadonly = false;
		this.lb_Type.ShowUnderLine = true;
		this.lb_Type.Size = new System.Drawing.Size(119, 20);
		this.lb_Type.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Type.TabIndex = 12;
		this.lb_Type.Utext = "Type";
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "Contract Template Num";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(12, 37);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(119, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 13;
		this.labelLine2.Utext = "Contract Template Num";
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "Status";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(258, 11);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(80, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 14;
		this.labelLine3.Utext = "Status";
		this.labelLine5.EditControl = null;
		this.labelLine5.EditValue = "SLA";
		this.labelLine5.FieldName = null;
		this.labelLine5.HideWhenEmpty = false;
		this.labelLine5.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine5.Location = new System.Drawing.Point(258, 37);
		this.labelLine5.Name = "labelLine5";
		this.labelLine5.ReadOnly = false;
		this.labelLine5.SettingInVisible = false;
		this.labelLine5.SettingReadonly = false;
		this.labelLine5.ShowUnderLine = true;
		this.labelLine5.Size = new System.Drawing.Size(80, 20);
		this.labelLine5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine5.TabIndex = 16;
		this.labelLine5.Utext = "SLA";
		this.txt_CTNum.EditControl = null;
		this.txt_CTNum.FieldName = null;
		this.txt_CTNum.IsSearchControl = false;
		this.txt_CTNum.Location = new System.Drawing.Point(134, 34);
		this.txt_CTNum.Name = "txt_CTNum";
		this.txt_CTNum.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions2.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions6.Image");
		this.txt_CTNum.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, false, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance4, appearanceHovered4, appearancePressed4, appearanceDisabled4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.txt_CTNum.Properties.ContainerControl = null;
		this.txt_CTNum.Properties.DisplayMember = "ContractTemplateNum";
		this.txt_CTNum.Properties.DisplayMember2 = "";
		this.txt_CTNum.Properties.LoadDataSourceWhenEnter = false;
		this.txt_CTNum.Properties.SeparateCharacterForMulti = ';';
		this.txt_CTNum.Properties.SeparateString = ", ";
		this.txt_CTNum.Properties.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.ListServiceContractTemplate;
		this.txt_CTNum.Properties.UsingAutoCompleteBox = false;
		this.txt_CTNum.Properties.UsingDataSource = true;
		this.txt_CTNum.Properties.ValidateWhenNoDataSource = false;
		this.txt_CTNum.Properties.ValueMember = "ContractTemplateNum";
		this.txt_CTNum.SettingInVisible = false;
		this.txt_CTNum.SettingReadonly = false;
		this.txt_CTNum.Size = new System.Drawing.Size(100, 20);
		this.txt_CTNum.TabIndex = 17;
		this.txt_Type.EditControl = null;
		this.txt_Type.FieldName = null;
		this.txt_Type.IsSearchControl = false;
		this.txt_Type.Location = new System.Drawing.Point(134, 8);
		this.txt_Type.Name = "txt_Type";
		this.txt_Type.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_Type.Properties.NullText = "";
		this.txt_Type.SettingInVisible = false;
		this.txt_Type.SettingReadonly = false;
		this.txt_Type.Size = new System.Drawing.Size(100, 20);
		this.txt_Type.TabIndex = 18;
		this.txt_SLA.EditControl = null;
		this.txt_SLA.FieldName = null;
		this.txt_SLA.IsSearchControl = false;
		this.txt_SLA.Location = new System.Drawing.Point(341, 34);
		this.txt_SLA.Name = "txt_SLA";
		this.txt_SLA.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions3.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions4.Image");
		this.txt_SLA.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance5, appearanceHovered5, appearancePressed5, appearanceDisabled5, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, false, true, editorButtonImageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance6, appearanceHovered6, appearancePressed6, appearanceDisabled6, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.txt_SLA.Properties.ContainerControl = null;
		this.txt_SLA.Properties.DisplayMember = "SLAID";
		this.txt_SLA.Properties.DisplayMember2 = "";
		this.txt_SLA.Properties.LoadDataSourceWhenEnter = false;
		this.txt_SLA.Properties.SeparateCharacterForMulti = ';';
		this.txt_SLA.Properties.SeparateString = ", ";
		this.txt_SLA.Properties.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.ServiceLevelAgreement;
		this.txt_SLA.Properties.UsingAutoCompleteBox = false;
		this.txt_SLA.Properties.UsingDataSource = true;
		this.txt_SLA.Properties.ValidateWhenNoDataSource = false;
		this.txt_SLA.Properties.ValueMember = "SLAID";
		this.txt_SLA.SettingInVisible = false;
		this.txt_SLA.SettingReadonly = false;
		this.txt_SLA.Size = new System.Drawing.Size(100, 20);
		this.txt_SLA.TabIndex = 19;
		this.txt_Status.EditControl = null;
		this.txt_Status.FieldName = null;
		this.txt_Status.IsSearchControl = false;
		this.txt_Status.Location = new System.Drawing.Point(341, 8);
		this.txt_Status.Name = "txt_Status";
		this.txt_Status.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_Status.Properties.NullText = "";
		this.txt_Status.SettingInVisible = false;
		this.txt_Status.SettingReadonly = false;
		this.txt_Status.Size = new System.Drawing.Size(100, 20);
		this.txt_Status.TabIndex = 20;
		this.btSearch.Appearance.Font = new System.Drawing.Font("Tahoma", 7.5f);
		this.btSearch.Appearance.Options.UseFont = true;
		this.btSearch.EditControl = null;
		this.btSearch.EditValue = "Search";
		this.btSearch.FieldName = null;
		this.btSearch.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btSearch.ImageOptions.Image");
		this.btSearch.IsSearchControl = false;
		this.btSearch.Location = new System.Drawing.Point(459, 35);
		this.btSearch.Name = "btSearch";
		this.btSearch.ReadOnly = false;
		this.btSearch.SettingInVisible = false;
		this.btSearch.SettingReadonly = false;
		this.btSearch.Size = new System.Drawing.Size(93, 19);
		this.btSearch.TabIndex = 60;
		this.btSearch.Text = "Search";
		this.btSearch.Click += new System.EventHandler(this.btSearch_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(768, 387);
		this.ContextMenuStrip = this.contextMenuStrip1;
		base.Controls.Add(this.btSearch);
		base.Controls.Add(this.txt_Status);
		base.Controls.Add(this.txt_SLA);
		base.Controls.Add(this.txt_Type);
		base.Controls.Add(this.txt_CTNum);
		base.Controls.Add(this.labelLine5);
		base.Controls.Add(this.labelLine3);
		base.Controls.Add(this.labelLine2);
		base.Controls.Add(this.lb_Type);
		base.Controls.Add(this.grd_ServiceContractTemplate);
		base.Controls.Add(this.btn_OK);
		base.Controls.Add(this.btn_Cancel);
		base.FormId = "oServiceContractTemplate";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_ServiceContractTemplate";
		this.Text = "Service Contract Template";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_ServiceContractTemplate_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_ServiceContractTemplate).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_ServiceContractTemplate).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lookup_Type).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ContractCycle).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lookup_CycleUnit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_TolerantTime).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lookup_TolerantUnit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lookup_Status).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bte_SLA).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_CTNum.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Type.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_SLA.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Status.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
