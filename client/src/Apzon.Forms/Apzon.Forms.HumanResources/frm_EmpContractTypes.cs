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
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.HumanResources;

public class frm_EmpContractTypes : XtraFormEx
{
	private List<int> changedCells = new List<int>();

	private const string FormController = "EmpContract";

	private bool isLoading;

	private bool isCancel;

	private bool isInserted;

	private int focusedRow;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private int typeIDMax;

	public Action<int> childSend;

	private IContainer components;

	private GridControlEx gridControl1;

	private GridViewEx gridView1;

	private GridColumnEx typeID;

	private GridColumnEx name;

	private GridColumnEx descriptio;

	private GridColumnEx code;

	private GridColumnEx empStatus;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem removeRowToolStripMenuItem;

	private RepositoryItemLookUpEdit repEmpStatus;

	private RepositoryItemTextEdit repCode;

	private RepositoryItemTextEdit repName;

	private SimpleButtonEx btCancel;

	private SimpleButtonEx btnOK;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
		{
			WinFuntions.DisableAllControl(Controls);
			btCancel.Enabled = true;
			return;
		}
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Update:
			btnOK.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btnOK.Text = Resources.ButtonOK;
			break;
		}
	}

	public frm_EmpContractTypes()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_HumanResources"))
		{
			InitializeComponent();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_HumanResources" && x.authoriza == "F"))
			{
				WinFuntions.DisableAllControl(Controls);
				btCancel.Enabled = true;
			}
			FormMode = Enums.FormViewModeType.View;
			FormId = "oEmpContractTypes";
			gridView1.FormId = FormId;
			gridView1.ItemId = "APZ_OCTY";
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	public async Task<DataTable> GetStatus()
	{
		try
		{
			using HttpClientHelper<DataTable> help = new HttpClientHelper<DataTable>(Configuration.ApiServer, "States");
			DataTable dataTable = await help.GetAsync("GetEmployeeStatus");
			if (!dataTable.HasColumn())
			{
				dataTable = WinFuntions.GetTableStructure("OHST");
			}
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	private async Task LoadData()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "EmpContract");
			DataTable dataTable = await helper.GetAsync("LoadContractTypes");
			if (!dataTable.IsNotNull())
			{
				dataTable = WinFuntions.GetTableStructure("APZ_OCTY");
			}
			else
			{
				typeIDMax = Function.ParseInt((from s in dataTable.AsEnumerable()
					where Function.ToString(s.Field<int?>("typeID")) != ""
					select s).Max((DataRow s) => s.Field<int?>("typeID")));
			}
			gridControl1.DataSource = dataTable;
			AddNewLine();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void CheckSaveState()
	{
		if (!isLoading)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	private async Task<string> SaveData()
	{
		try
		{
			DataTable dataSourceTable = gridControl1.DataSourceTable;
			dataSourceTable.AcceptChanges();
			string requiredField = "code";
			if (dataSourceTable.IsNotNull())
			{
				foreach (DataRow item in dataSourceTable.AsEnumerable())
				{
					if (Function.ToString(item["typeID"]) == "")
					{
						item["typeID"] = 0;
						isInserted = true;
					}
				}
				DataRow[] array = (from s in dataSourceTable.AsEnumerable()
					where !string.IsNullOrEmpty(Function.ToString(s.Field<string>(requiredField)))
					select s).ToArray();
				if (array.Length == 0)
				{
					gridControl1.DataSource = dataSourceTable.Clone();
					dataSourceTable = dataSourceTable.Clone();
					dataSourceTable.AcceptChanges();
				}
				else
				{
					dataSourceTable = array.CopyToDataTable();
					dataSourceTable.AcceptChanges();
				}
				using (HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "EmpContract"))
				{
					HttpResult httpResult = helper.Post("Save", dataSourceTable);
					if (httpResult != null && string.IsNullOrEmpty(httpResult.Message))
					{
						focusedRow = gridView1.FocusedRowHandle;
						await LoadData();
						WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
						return "success";
					}
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				}
				return "";
			}
			WinCommons.Error(Enums.MessageCode.ThisEntryIsNull, "");
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return "";
		}
	}

	private async void btnOK_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
				return;
			}
			gridView1.ClearSorting();
			if (await SaveData() != "success")
			{
				return;
			}
			FormMode = Enums.FormViewModeType.View;
			gridView1.FocusedRowHandle = focusedRow;
			int obj;
			if (changedCells.Count > 0)
			{
				if (isInserted)
				{
					obj = typeIDMax;
					isInserted = false;
				}
				else
				{
					for (int i = 0; i < changedCells.Count; i++)
					{
						int value = Function.ParseInt(gridView1.GetRowCellValue(changedCells[i], "typeID"));
						changedCells[i] = value;
					}
					obj = changedCells.Max();
				}
			}
			else
			{
				obj = Function.ParseInt(gridView1.GetFocusedRowCellValue("typeID"));
			}
			if (childSend != null)
			{
				childSend(obj);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btCancel_Click(object sender, EventArgs e)
	{
		isCancel = true;
		Close();
	}

	private void frm_EmpContractTypes_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Update && !isCancel)
			{
				switch (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1))
				{
				case DialogResult.Cancel:
					e.Cancel = true;
					break;
				case DialogResult.Yes:
					btnOK_Click(null, null);
					break;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gridView1_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		if (!isLoading)
		{
			if (Function.ToString(gridView1.GetRowCellValue(e.RowHandle, "code")) != "")
			{
				changedCells.Add(e.RowHandle);
			}
			AddNewLine();
			CheckSaveState();
		}
	}

	private void gridView1_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		CheckSaveState();
	}

	private void gridView1_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator && e.RowHandle != -2147483647)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private async void removeRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (gridView1.DataRowCount <= 0)
			{
				return;
			}
			if ((gridControl1.DataSource as DataTable).IsNotNull())
			{
				int num = Function.ParseInt(gridView1.GetFocusedRowCellValue("typeID"));
				if (num > 0)
				{
					using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "EmpContract");
					if ((await helper.GetAsync("CheckContractType", new
					{
						typeID = num
					})).IsNotNull())
					{
						WinCommons.Error(Enums.MessageCode.CannotDeleteThisRecordIsBeingUsed, "");
						return;
					}
				}
			}
			gridView1.BeginUpdate();
			gridView1.DeleteSelectedRows();
			gridView1.EndUpdate();
			FormMode = Enums.FormViewModeType.Update;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadEmpStatus()
	{
		try
		{
			DataTable dataTable = await GetStatus();
			if (dataTable.HasColumn())
			{
				dataTable.NewRow();
				DataRow dataRow = dataTable.NewRow();
				dataRow["Code"] = -2;
				dataRow["Name"] = "Define New";
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
				repEmpStatus.DropDownRows = dataTable.Rows.Count;
				repEmpStatus.DataSource = dataTable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task REFRESH_DATA()
	{
		_ = 1;
		try
		{
			if (FormMode != Enums.FormViewModeType.Update || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) != DialogResult.No)
			{
				isLoading = true;
				await LoadData();
				await LoadEmpStatus();
				isLoading = false;
				FormMode = Enums.FormViewModeType.View;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void frm_EmpContractTypes_Load(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			isLoading = true;
			RepositoryItemTextEdit repositoryItemTextEdit = new RepositoryItemTextEdit();
			repositoryItemTextEdit.MaxLength = 20;
			gridView1.Columns["code"].ColumnEdit = repositoryItemTextEdit;
			RepositoryItemTextEdit repositoryItemTextEdit2 = new RepositoryItemTextEdit();
			repositoryItemTextEdit2.MaxLength = 100;
			gridView1.Columns["name"].ColumnEdit = repositoryItemTextEdit2;
			await LoadData();
			await LoadEmpStatus();
			repEmpStatus.ValueMember = "Code";
			repEmpStatus.DisplayMember = "Name";
			gridView1.OptionsBehavior.KeepFocusedRowOnUpdate = true;
			isLoading = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void LoadLookupStatus()
	{
		try
		{
			await LoadEmpStatus();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void repEmpStatus_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (!isLoading)
			{
				if (Function.ParseInt(e.NewValue) == -2)
				{
					frm_HREmpStatus frm_HREmpStatus2 = new frm_HREmpStatus
					{
						MdiParent = MdiParent
					};
					frm_HREmpStatus2.refreshStatus = (RefreshLookupStatus)Delegate.Combine(frm_HREmpStatus2.refreshStatus, new RefreshLookupStatus(LoadLookupStatus));
					frm_HREmpStatus2.StartPosition = FormStartPosition.CenterScreen;
					frm_HREmpStatus2.Show();
					e.Cancel = true;
				}
				else
				{
					CheckSaveState();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void AddNewLine()
	{
		DataTable dataSourceTable = gridControl1.DataSourceTable;
		if (dataSourceTable != null && (dataSourceTable.Rows.Count == 0 || dataSourceTable.AsEnumerable().All((DataRow t) => !string.IsNullOrEmpty(Function.ToString(t["Code"])))))
		{
			DataRow row = dataSourceTable.NewRow();
			dataSourceTable.Rows.Add(row);
			dataSourceTable.AcceptChanges();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.HumanResources.frm_EmpContractTypes));
		this.btCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnOK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.gridControl1 = new Apzon.Controls.Grid.GridControlEx();
		this.gridView1 = new Apzon.Controls.Grid.GridViewEx();
		this.typeID = new Apzon.Controls.Grid.GridColumnEx();
		this.code = new Apzon.Controls.Grid.GridColumnEx();
		this.name = new Apzon.Controls.Grid.GridColumnEx();
		this.descriptio = new Apzon.Controls.Grid.GridColumnEx();
		this.empStatus = new Apzon.Controls.Grid.GridColumnEx();
		this.repEmpStatus = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.repCode = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.repName = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.removeRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		((System.ComponentModel.ISupportInitialize)this.gridControl1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridView1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repEmpStatus).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repName).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		base.SuspendLayout();
		this.btCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btCancel.EditControl = null;
		this.btCancel.EditValue = "Cancel";
		this.btCancel.FieldName = null;
		this.btCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btCancel.ImageOptions.Image");
		this.btCancel.IsSearchControl = false;
		this.btCancel.Location = new System.Drawing.Point(81, 300);
		this.btCancel.Name = "btCancel";
		this.btCancel.ReadOnly = false;
		this.btCancel.SettingInVisible = false;
		this.btCancel.SettingReadonly = false;
		this.btCancel.Size = new System.Drawing.Size(75, 23);
		this.btCancel.TabIndex = 12;
		this.btCancel.Text = "Cancel";
		this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
		this.btnOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnOK.EditControl = null;
		this.btnOK.EditValue = "OK";
		this.btnOK.FieldName = null;
		this.btnOK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnOK.ImageOptions.Image");
		this.btnOK.IsSearchControl = false;
		this.btnOK.Location = new System.Drawing.Point(3, 300);
		this.btnOK.Name = "btnOK";
		this.btnOK.ReadOnly = false;
		this.btnOK.SettingInVisible = false;
		this.btnOK.SettingReadonly = false;
		this.btnOK.Size = new System.Drawing.Size(75, 23);
		this.btnOK.TabIndex = 11;
		this.btnOK.Text = "OK";
		this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
		this.gridControl1.AllowPasteExcel = true;
		this.gridControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.gridControl1.ClipboardData = null;
		this.gridControl1.Location = new System.Drawing.Point(3, 3);
		this.gridControl1.MainView = this.gridView1;
		this.gridControl1.Name = "gridControl1";
		this.gridControl1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.repEmpStatus, this.repCode, this.repName });
		this.gridControl1.Size = new System.Drawing.Size(585, 292);
		this.gridControl1.TabIndex = 10;
		this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gridView1 });
		this.gridView1.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gridView1.ColumnKey = null;
		this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[5] { this.typeID, this.code, this.name, this.descriptio, this.empStatus });
		this.gridView1.DisableFormSetting = false;
		this.gridView1.DisplayException = false;
		this.gridView1.EditColumn = null;
		this.gridView1.FieldCurrency = null;
		this.gridView1.FieldRate = null;
		this.gridView1.FormId = "oContractTypes";
		this.gridView1.GridControl = this.gridControl1;
		this.gridView1.IgnoreFilter = false;
		this.gridView1.IndexEditRow = -1;
		this.gridView1.IndicatorWidth = 30;
		this.gridView1.IsApzTable = false;
		this.gridView1.IsInsertNewRow = false;
		this.gridView1.IsLoadingData = false;
		this.gridView1.IsMappingContextAction = false;
		this.gridView1.ItemId = "APZ_OCTY";
		this.gridView1.LstColumnAllowBatch = null;
		this.gridView1.LstColumnNotAllowBatch = null;
		this.gridView1.LstColumnNotEditable = null;
		this.gridView1.LstColumNotSetting = null;
		this.gridView1.Name = "gridView1";
		this.gridView1.OptionsView.ShowGroupPanel = false;
		this.gridView1.UseBatchMod = false;
		this.gridView1.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gridView1_CustomDrawRowIndicator);
		this.gridView1.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridView1_CellValueChanged);
		this.gridView1.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridView1_CellValueChanging);
		this.typeID.Caption = "typeID";
		this.typeID.CurrencyDateField = null;
		this.typeID.CurrencyField = null;
		this.typeID.CurrencyFixed = null;
		this.typeID.CurrencyFixedRate = 0.0;
		this.typeID.CurrencyRateField = null;
		this.typeID.FieldName = "typeID";
		this.typeID.Name = "typeID";
		this.typeID.PrimaryKey = true;
		this.typeID.UnitField = null;
		this.code.Caption = "Code";
		this.code.CurrencyDateField = null;
		this.code.CurrencyField = null;
		this.code.CurrencyFixed = null;
		this.code.CurrencyFixedRate = 0.0;
		this.code.CurrencyRateField = null;
		this.code.FieldName = "code";
		this.code.Name = "code";
		this.code.PrimaryKey = false;
		this.code.UnitField = null;
		this.code.Visible = true;
		this.code.VisibleIndex = 0;
		this.code.Width = 115;
		this.name.Caption = "Name";
		this.name.CurrencyDateField = null;
		this.name.CurrencyField = null;
		this.name.CurrencyFixed = null;
		this.name.CurrencyFixedRate = 0.0;
		this.name.CurrencyRateField = null;
		this.name.FieldName = "name";
		this.name.Name = "name";
		this.name.PrimaryKey = false;
		this.name.UnitField = null;
		this.name.Visible = true;
		this.name.VisibleIndex = 1;
		this.name.Width = 137;
		this.descriptio.Caption = "Description";
		this.descriptio.CurrencyDateField = null;
		this.descriptio.CurrencyField = null;
		this.descriptio.CurrencyFixed = null;
		this.descriptio.CurrencyFixedRate = 0.0;
		this.descriptio.CurrencyRateField = null;
		this.descriptio.FieldName = "descriptio";
		this.descriptio.Name = "descriptio";
		this.descriptio.PrimaryKey = false;
		this.descriptio.UnitField = null;
		this.descriptio.Visible = true;
		this.descriptio.VisibleIndex = 3;
		this.descriptio.Width = 178;
		this.empStatus.Caption = "Emp Status";
		this.empStatus.ColumnEdit = this.repEmpStatus;
		this.empStatus.CurrencyDateField = null;
		this.empStatus.CurrencyField = null;
		this.empStatus.CurrencyFixed = null;
		this.empStatus.CurrencyFixedRate = 0.0;
		this.empStatus.CurrencyRateField = null;
		this.empStatus.FieldName = "empStatus";
		this.empStatus.Name = "empStatus";
		this.empStatus.PrimaryKey = false;
		this.empStatus.UnitField = null;
		this.empStatus.Visible = true;
		this.empStatus.VisibleIndex = 2;
		this.empStatus.Width = 105;
		this.repEmpStatus.AutoHeight = false;
		this.repEmpStatus.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repEmpStatus.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.repEmpStatus.Name = "repEmpStatus";
		this.repEmpStatus.NullText = "";
		this.repEmpStatus.ShowFooter = false;
		this.repEmpStatus.ShowHeader = false;
		this.repEmpStatus.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.repEmpStatus_EditValueChanging);
		this.repCode.AutoHeight = false;
		this.repCode.MaxLength = 20;
		this.repCode.Name = "repCode";
		this.repName.AutoHeight = false;
		this.repName.MaxLength = 100;
		this.repName.Name = "repName";
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.removeRowToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(144, 26);
		this.removeRowToolStripMenuItem.Name = "removeRowToolStripMenuItem";
		this.removeRowToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
		this.removeRowToolStripMenuItem.Text = "Remove Row";
		this.removeRowToolStripMenuItem.Click += new System.EventHandler(this.removeRowToolStripMenuItem_Click);
		base.AcceptButton = this.btnOK;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(591, 327);
		this.ContextMenuStrip = this.contextMenuStrip1;
		base.Controls.Add(this.btCancel);
		base.Controls.Add(this.btnOK);
		base.Controls.Add(this.gridControl1);
		base.FormId = "oContractTypes";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_EmpContractTypes";
		base.ShowInTaskbar = false;
		this.Text = "Contract Types";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_EmpContractTypes_FormClosing);
		base.Load += new System.EventHandler(this.frm_EmpContractTypes_Load);
		((System.ComponentModel.ISupportInitialize)this.gridControl1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridView1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repEmpStatus).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repName).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
