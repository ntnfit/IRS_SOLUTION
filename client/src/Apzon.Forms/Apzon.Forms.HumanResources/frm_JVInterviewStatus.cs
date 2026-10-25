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
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.HttpClientHelper;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.HumanResources;

public class frm_JVInterviewStatus : XtraFormEx
{
	private List<int> changedCells = new List<int>();

	public string FormController = "JVInterview";

	private bool isLoading;

	private bool isCancel;

	private bool isInserted;

	private int focusedRow;

	private int idMax;

	public string tableName = "";

	public string idField = "Id";

	public string firstField = "";

	public string secondField = "";

	public Action<int> childSend;

	private IContainer components;

	public GridControlEx gridControl1;

	public GridViewEx gridView1;

	public GridColumnEx gcId;

	public GridColumnEx gcRemark;

	public GridColumnEx gcName;

	public RepositoryItemLookUpEdit repEmpStatus;

	public RepositoryItemTextEdit repCode;

	public RepositoryItemTextEdit repName;

	public ContextMenuStrip contextMenuStrip1;

	public ToolStripMenuItem removeRowToolStripMenuItem;

	public SimpleButtonEx btCancel;

	public SimpleButtonEx btnOK;

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

	public frm_JVInterviewStatus()
	{
		InitializeComponent();
		if (FormAuth == Enums.FormAuthorities.Read)
		{
			WinFuntions.DisableAllControl(Controls);
			btCancel.Enabled = true;
		}
		tableName = "APZ_OHIS";
		firstField = "Name";
		secondField = "Remark";
		gridView1.ItemId = "APZ_OHIS";
		gridView1.FormId = "oInterviewStatus";
	}

	protected virtual void SetupGridView()
	{
	}

	protected virtual void LoadResult(ref DataTable dt)
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
			dt = httpClientHelper.Get("LoadInterviewStatus");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected virtual DataTable DeleteResult(int _ID)
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
			return httpClientHelper.Get("CheckInterviewStatus", new
			{
				ID = _ID
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	protected virtual HttpResult SaveResult(Tuple<DataTable, string> dt)
	{
		try
		{
			WinCommons._OpenProcessing();
			using HttpClientHelper<HttpResult> httpClientHelper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, FormController);
			return httpClientHelper.Post("Save", dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return new HttpResult(Enums.MessageCode.Error, "Error");
		}
	}

	protected virtual void LoadData()
	{
		try
		{
			DataTable dt = new DataTable();
			LoadResult(ref dt);
			if (!dt.IsNotNull())
			{
				dt = WinFuntions.GetTableStructure(tableName);
			}
			else
			{
				idMax = Function.ParseInt((from s in dt.AsEnumerable()
					where Function.ToString(s.Field<int?>(idField)) != ""
					select s).Max((DataRow s) => s.Field<int?>(idField)));
			}
			gridControl1.DataSource = dt;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected virtual void AddNewRow()
	{
		try
		{
			DataTable dataTable = gridControl1.DataSource as DataTable;
			if (!dataTable.IsNotNull())
			{
				dataTable = WinFuntions.GetTableStructure(tableName);
			}
			dataTable.Rows.Add(dataTable.NewRow());
			dataTable.AcceptChanges();
			gridControl1.DataSource = dataTable;
			gridControl1.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	protected virtual async Task<string> SaveToDB(DataTable dtNew)
	{
		try
		{
			Tuple<DataTable, string> dt = new Tuple<DataTable, string>(dtNew, tableName);
			HttpResult httpResult = SaveResult(dt);
			if (httpResult != null && string.IsNullOrEmpty(httpResult.Message))
			{
				focusedRow = gridView1.FocusedRowHandle;
				LoadData();
				WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
				return "success";
			}
			WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return "";
		}
	}

	protected virtual async Task<string> SaveData()
	{
		try
		{
			DataTable dataTable = gridControl1.DataSource as DataTable;
			dataTable.AcceptChanges();
			string requiredField = firstField;
			if (dataTable.IsNotNull())
			{
				foreach (DataRow item in dataTable.AsEnumerable())
				{
					if (Function.ToString(item[idField]) == "")
					{
						item[idField] = 0;
						isInserted = true;
					}
				}
				DataRow[] array = (from s in dataTable.AsEnumerable()
					where Function.ToString(s.Field<string>(requiredField)) != ""
					select s).ToArray();
				if (array.Length == 0)
				{
					gridControl1.DataSource = dataTable.Clone();
					dataTable = dataTable.Clone();
					dataTable.AcceptChanges();
				}
				else
				{
					dataTable = array.CopyToDataTable();
					dataTable.AcceptChanges();
				}
				return await SaveToDB(dataTable);
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

	private async void UpdateData()
	{
		try
		{
			gridView1.ClearSorting();
			DataTable dataTable = gridControl1.DataSource as DataTable;
			dataTable.AcceptChanges();
			if (dataTable.IsNotNull())
			{
				foreach (DataRow item in dataTable.AsEnumerable())
				{
					if (Function.ToString(item[firstField]) == "")
					{
						if (!(Function.ToString(item[idField]) == "") && !(Function.ToString(item[idField]) == "0"))
						{
							WinCommons.Error(Enums.MessageCode.FieldCannotBeNullWithFieldName, "", firstField);
							return;
						}
						if (Function.ToString(item[secondField]) != "")
						{
							WinCommons.Error(Enums.MessageCode.FieldCannotBeNullWithFieldName, "", firstField);
							return;
						}
					}
				}
			}
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
					obj = idMax;
					isInserted = false;
				}
				else
				{
					for (int i = 0; i < changedCells.Count; i++)
					{
						int value = Function.ParseInt(gridView1.GetRowCellValue(changedCells[i], idField));
						changedCells[i] = value;
					}
					obj = changedCells.Max();
				}
			}
			else
			{
				obj = Function.ParseInt(gridView1.GetFocusedRowCellValue(idField));
			}
			if (childSend != null)
			{
				childSend(obj);
			}
			AddNewRow();
			gridView1.MakeRowVisible(gridView1.DataRowCount - 1);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btnOK_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
			}
			else
			{
				UpdateData();
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

	private void frm_JVInterviewStatus_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Update && !isCancel)
			{
				switch (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSaveBeforeClose, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1))
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

	private void CheckSaveState()
	{
		if (!isLoading)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	private void gridView1_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		if (!isLoading)
		{
			if (Function.ToString(gridView1.GetRowCellValue(e.RowHandle, firstField)) != "")
			{
				changedCells.Add(e.RowHandle);
			}
			CheckSaveState();
		}
	}

	private void gridView1_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
	}

	private void gridView1_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Update || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) != DialogResult.No)
			{
				isLoading = true;
				LoadData();
				AddNewRow();
				gridView1.MakeRowVisible(gridView1.DataRowCount - 1);
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

	private async void frm_JVInterviewStatus_Load(object sender, EventArgs e)
	{
		try
		{
			gcId.FieldName = idField;
			gcName.FieldName = firstField;
			gcRemark.FieldName = secondField;
			if (!isLoading)
			{
				isLoading = true;
				SetupGridView();
				RepositoryItemTextEdit repositoryItemTextEdit = new RepositoryItemTextEdit();
				repositoryItemTextEdit.MaxLength = 254;
				gridView1.Columns[firstField].ColumnEdit = repositoryItemTextEdit;
				LoadData();
				AddNewRow();
				isLoading = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected virtual bool DeleteRow(int _ID)
	{
		try
		{
			if (DeleteResult(_ID).IsNotNull())
			{
				WinCommons.Error(Enums.MessageCode.CannotDeleteThisRecordIsBeingUsed, "");
				return false;
			}
			return true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
			return false;
		}
	}

	public virtual async void removeRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (gridView1.DataRowCount <= 0)
			{
				return;
			}
			DataTable dataTable = gridControl1.DataSource as DataTable;
			if (dataTable.IsNotNull())
			{
				int focusedRowHandle = gridView1.FocusedRowHandle;
				dataTable.Rows.RemoveAt(focusedRowHandle);
				if (dataTable.AsEnumerable().All((DataRow r) => Function.ToString(r["Name"]).Trim() != ""))
				{
					AddNewRow();
				}
				gridControl1.RefreshDataSource();
			}
			FormMode = Enums.FormViewModeType.Update;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gridControl1_EditorKeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Return)
		{
			gridView1.PostEditor();
			UpdateData();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.HumanResources.frm_JVInterviewStatus));
		this.btCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnOK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.gridControl1 = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.removeRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gridView1 = new Apzon.Controls.Grid.GridViewEx();
		this.gcId = new Apzon.Controls.Grid.GridColumnEx();
		this.gcName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcRemark = new Apzon.Controls.Grid.GridColumnEx();
		this.repEmpStatus = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.repCode = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.repName = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		((System.ComponentModel.ISupportInitialize)this.gridControl1).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gridView1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repEmpStatus).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repName).BeginInit();
		base.SuspendLayout();
		this.btCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btCancel.EditControl = null;
		this.btCancel.EditValue = "Cancel";
		this.btCancel.FieldName = null;
		this.btCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btCancel.ImageOptions.Image");
		this.btCancel.IsSearchControl = false;
		this.btCancel.Location = new System.Drawing.Point(84, 247);
		this.btCancel.Name = "btCancel";
		this.btCancel.ReadOnly = false;
		this.btCancel.SettingInVisible = false;
		this.btCancel.SettingReadonly = false;
		this.btCancel.Size = new System.Drawing.Size(75, 23);
		this.btCancel.TabIndex = 15;
		this.btCancel.Text = "Cancel";
		this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
		this.btnOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnOK.EditControl = null;
		this.btnOK.EditValue = "OK";
		this.btnOK.FieldName = null;
		this.btnOK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnOK.ImageOptions.Image");
		this.btnOK.IsSearchControl = false;
		this.btnOK.Location = new System.Drawing.Point(5, 247);
		this.btnOK.Name = "btnOK";
		this.btnOK.ReadOnly = false;
		this.btnOK.SettingInVisible = false;
		this.btnOK.SettingReadonly = false;
		this.btnOK.Size = new System.Drawing.Size(75, 23);
		this.btnOK.TabIndex = 14;
		this.btnOK.Text = "OK";
		this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
		this.gridControl1.AllowPasteExcel = true;
		this.gridControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.gridControl1.ClipboardData = null;
		this.gridControl1.ContextMenuStrip = this.contextMenuStrip1;
		this.gridControl1.Location = new System.Drawing.Point(0, 0);
		this.gridControl1.MainView = this.gridView1;
		this.gridControl1.Name = "gridControl1";
		this.gridControl1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.repEmpStatus, this.repCode, this.repName });
		this.gridControl1.Size = new System.Drawing.Size(512, 241);
		this.gridControl1.TabIndex = 13;
		this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gridView1 });
		this.gridControl1.EditorKeyDown += new System.Windows.Forms.KeyEventHandler(this.gridControl1_EditorKeyDown);
		this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.removeRowToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(144, 26);
		this.removeRowToolStripMenuItem.Name = "removeRowToolStripMenuItem";
		this.removeRowToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
		this.removeRowToolStripMenuItem.Text = "Remove Row";
		this.removeRowToolStripMenuItem.Click += new System.EventHandler(this.removeRowToolStripMenuItem_Click);
		this.gridView1.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gridView1.ColumnKey = null;
		this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.gcId, this.gcName, this.gcRemark });
		this.gridView1.DisableFormSetting = false;
		this.gridView1.DisplayException = false;
		this.gridView1.EditColumn = null;
		this.gridView1.FieldCurrency = null;
		this.gridView1.FieldRate = null;
		this.gridView1.FormId = "oInterviewStatus";
		this.gridView1.GridControl = this.gridControl1;
		this.gridView1.IgnoreFilter = false;
		this.gridView1.IndexEditRow = -1;
		this.gridView1.IndicatorWidth = 30;
		this.gridView1.IsApzTable = false;
		this.gridView1.IsInsertNewRow = false;
		this.gridView1.IsLoadingData = false;
		this.gridView1.IsMappingContextAction = false;
		this.gridView1.ItemId = "APZ_OHIS";
		this.gridView1.LstColumnAllowBatch = null;
		this.gridView1.LstColumnNotAllowBatch = null;
		this.gridView1.LstColumnNotEditable = null;
		this.gridView1.LstColumNotSetting = null;
		this.gridView1.Name = "gridView1";
		this.gridView1.OptionsSelection.MultiSelect = true;
		this.gridView1.OptionsView.ShowGroupPanel = false;
		this.gridView1.UseBatchMod = false;
		this.gridView1.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gridView1_CustomDrawRowIndicator);
		this.gridView1.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridView1_CellValueChanged);
		this.gridView1.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridView1_CellValueChanging);
		this.gcId.Caption = "Id";
		this.gcId.CurrencyDateField = null;
		this.gcId.CurrencyField = null;
		this.gcId.CurrencyFixed = null;
		this.gcId.CurrencyFixedRate = 0.0;
		this.gcId.CurrencyRateField = null;
		this.gcId.FieldName = "Id";
		this.gcId.Name = "gcId";
		this.gcId.PrimaryKey = false;
		this.gcId.UnitField = null;
		this.gcName.Caption = "Name";
		this.gcName.CurrencyDateField = null;
		this.gcName.CurrencyField = null;
		this.gcName.CurrencyFixed = null;
		this.gcName.CurrencyFixedRate = 0.0;
		this.gcName.CurrencyRateField = null;
		this.gcName.FieldName = "Name";
		this.gcName.Name = "gcName";
		this.gcName.PrimaryKey = true;
		this.gcName.UnitField = null;
		this.gcName.Visible = true;
		this.gcName.VisibleIndex = 0;
		this.gcName.Width = 179;
		this.gcRemark.Caption = "Remark";
		this.gcRemark.CurrencyDateField = null;
		this.gcRemark.CurrencyField = null;
		this.gcRemark.CurrencyFixed = null;
		this.gcRemark.CurrencyFixedRate = 0.0;
		this.gcRemark.CurrencyRateField = null;
		this.gcRemark.FieldName = "Remark";
		this.gcRemark.Name = "gcRemark";
		this.gcRemark.PrimaryKey = false;
		this.gcRemark.UnitField = null;
		this.gcRemark.Visible = true;
		this.gcRemark.VisibleIndex = 1;
		this.gcRemark.Width = 288;
		this.repEmpStatus.AutoHeight = false;
		this.repEmpStatus.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repEmpStatus.Name = "repEmpStatus";
		this.repCode.AutoHeight = false;
		this.repCode.MaxLength = 20;
		this.repCode.Name = "repCode";
		this.repName.AutoHeight = false;
		this.repName.MaxLength = 100;
		this.repName.Name = "repName";
		base.AcceptButton = this.btnOK;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(512, 274);
		base.Controls.Add(this.btCancel);
		base.Controls.Add(this.btnOK);
		base.Controls.Add(this.gridControl1);
		base.FormId = "oInterviewStatus";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_JVInterviewStatus";
		base.ShowInTaskbar = false;
		this.Text = "Interview Status";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_JVInterviewStatus_FormClosing);
		base.Load += new System.EventHandler(this.frm_JVInterviewStatus_Load);
		((System.ComponentModel.ISupportInitialize)this.gridControl1).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gridView1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repEmpStatus).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repName).EndInit();
		base.ResumeLayout(false);
	}
}
