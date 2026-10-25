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

public class frm_HREducationTypes : XtraFormEx
{
	private List<int> changedCells = new List<int>();

	private const string FormController = "HREducation";

	private bool isLoading;

	private bool isCancel;

	private bool isInserted;

	private int focusedRow;

	private int edTypeMax;

	public Action<int> childSend;

	private IContainer components;

	private GridControlEx gridControl1;

	private GridColumnEx name;

	private GridColumnEx descriptio;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem removeRowToolStripMenuItem;

	private GridColumnEx edType;

	private SimpleButtonEx btCancel;

	private SimpleButtonEx btnOK;

	private GridViewEx gridView1;

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

	public frm_HREducationTypes()
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
			FormId = "oHREducationTypes";
			gridView1.FormId = FormId;
			gridView1.ItemId = "OHED";
			OnLoadAsync += frm_HREducationTypes_Load;
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	private async Task LoadData()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "HREducation");
			DataTable dataTable = await helper.GetAsync("LoadDoc");
			if (!dataTable.IsNotNull())
			{
				dataTable = WinFuntions.GetTableStructure("OHED");
			}
			else
			{
				edTypeMax = Function.ParseInt((from s in dataTable.AsEnumerable()
					where Function.ToString(s.Field<int?>("edType")) != ""
					select s).Max((DataRow s) => s.Field<int?>("edType")));
			}
			gridControl1.DataSource = dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<string> SaveData()
	{
		try
		{
			DataTable dataTable = gridControl1.DataSource as DataTable;
			dataTable.AcceptChanges();
			string requiredField = "name";
			if (dataTable.IsNotNull())
			{
				foreach (DataRow item in dataTable.AsEnumerable())
				{
					if (Function.ToString(item["edType"]) == "")
					{
						item["edType"] = 0;
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
				using (HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "HREducation"))
				{
					HttpResult httpResult = helper.Post("Save", dataTable);
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

	private void CheckSaveState()
	{
		if (!isLoading)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Update || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) != DialogResult.No)
			{
				isLoading = true;
				await LoadData();
				gridView1.AddNewRow();
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

	private async Task frm_HREducationTypes_Load(object sender, EventArgs e)
	{
		try
		{
			isLoading = true;
			RepositoryItemTextEdit repositoryItemTextEdit = new RepositoryItemTextEdit();
			repositoryItemTextEdit.MaxLength = 20;
			gridView1.Columns["name"].ColumnEdit = repositoryItemTextEdit;
			await LoadData();
			gridView1.OptionsBehavior.KeepFocusedRowOnUpdate = true;
			gridView1.AddNewRow();
			gridView1.MakeRowVisible(gridView1.DataRowCount - 1);
			isLoading = false;
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
				return;
			}
			gridView1.ClearSorting();
			DataTable dataTable = gridControl1.DataSource as DataTable;
			dataTable.AcceptChanges();
			if (dataTable.IsNotNull())
			{
				foreach (DataRow item in dataTable.AsEnumerable())
				{
					if (Function.ToString(item["name"]) == "")
					{
						if (!(Function.ToString(item["edType"]) == "") && !(Function.ToString(item["edType"]) == "0"))
						{
							WinCommons.Error(Enums.MessageCode.NameCannotBeEmpty, "");
							return;
						}
						if (Function.ToString(item["descriptio"]) != "")
						{
							WinCommons.Error(Enums.MessageCode.NameCannotBeEmpty, "");
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
					obj = edTypeMax;
					isInserted = false;
				}
				else
				{
					for (int i = 0; i < changedCells.Count; i++)
					{
						int value = Function.ParseInt(gridView1.GetRowCellValue(changedCells[i], "edType"));
						changedCells[i] = value;
					}
					obj = changedCells.Max();
				}
			}
			else
			{
				obj = Function.ParseInt(gridView1.GetFocusedRowCellValue("edType"));
			}
			if (childSend != null)
			{
				childSend(obj);
			}
			gridView1.AddNewRow();
			gridView1.MakeRowVisible(gridView1.DataRowCount - 1);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void removeRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if ((gridControl1.DataSource as DataTable).IsNotNull())
			{
				int num = Function.ParseInt(gridView1.GetFocusedRowCellValue("edType"));
				if (num > 0)
				{
					using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "HREducation");
					if ((await helper.GetAsync("CheckEducationType", new
					{
						edType = num
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

	private void frm_HREducationTypes_FormClosing(object sender, FormClosingEventArgs e)
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

	private void btCancel_Click(object sender, EventArgs e)
	{
		isCancel = true;
		Close();
	}

	private void gridView1_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		if (!isLoading)
		{
			if (Function.ToString(gridView1.GetRowCellValue(e.RowHandle, "name")) != "")
			{
				changedCells.Add(e.RowHandle);
			}
			CheckSaveState();
		}
	}

	private void gridView1_ValidateRow(object sender, ValidateRowEventArgs e)
	{
	}

	private void gridView1_InvalidRowException(object sender, InvalidRowExceptionEventArgs e)
	{
		e.ExceptionMode = ExceptionMode.NoAction;
	}

	private void gridView1_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator && e.RowHandle != -2147483647)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void gridView1_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		CheckSaveState();
	}

	private void gridControl1_EditorKeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Return)
		{
			gridView1.PostEditor();
			btnOK_Click(null, null);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.HumanResources.frm_HREducationTypes));
		this.gridControl1 = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.removeRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gridView1 = new Apzon.Controls.Grid.GridViewEx();
		this.edType = new Apzon.Controls.Grid.GridColumnEx();
		this.name = new Apzon.Controls.Grid.GridColumnEx();
		this.descriptio = new Apzon.Controls.Grid.GridColumnEx();
		this.btCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnOK = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.gridControl1).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gridView1).BeginInit();
		base.SuspendLayout();
		this.gridControl1.AllowPasteExcel = true;
		this.gridControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.gridControl1.ClipboardData = null;
		this.gridControl1.ContextMenuStrip = this.contextMenuStrip1;
		this.gridControl1.Location = new System.Drawing.Point(0, 0);
		this.gridControl1.MainView = this.gridView1;
		this.gridControl1.Name = "gridControl1";
		this.gridControl1.Size = new System.Drawing.Size(608, 346);
		this.gridControl1.TabIndex = 0;
		this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gridView1 });
		this.gridControl1.EditorKeyDown += new System.Windows.Forms.KeyEventHandler(this.gridControl1_EditorKeyDown);
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.removeRowToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(144, 26);
		this.removeRowToolStripMenuItem.Name = "removeRowToolStripMenuItem";
		this.removeRowToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
		this.removeRowToolStripMenuItem.Text = "Remove Row";
		this.removeRowToolStripMenuItem.Click += new System.EventHandler(this.removeRowToolStripMenuItem_Click);
		this.gridView1.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gridView1.ColumnKey = null;
		this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.edType, this.name, this.descriptio });
		this.gridView1.DisableFormSetting = false;
		this.gridView1.DisplayException = false;
		this.gridView1.EditColumn = null;
		this.gridView1.FieldCurrency = null;
		this.gridView1.FieldRate = null;
		this.gridView1.FormId = "oEducationTypes";
		this.gridView1.GridControl = this.gridControl1;
		this.gridView1.IgnoreFilter = false;
		this.gridView1.IndexEditRow = -1;
		this.gridView1.IndicatorWidth = 30;
		this.gridView1.IsApzTable = false;
		this.gridView1.IsInsertNewRow = false;
		this.gridView1.IsLoadingData = false;
		this.gridView1.IsMappingContextAction = false;
		this.gridView1.ItemId = "OHED";
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
		this.gridView1.InvalidRowException += new DevExpress.XtraGrid.Views.Base.InvalidRowExceptionEventHandler(this.gridView1_InvalidRowException);
		this.gridView1.ValidateRow += new DevExpress.XtraGrid.Views.Base.ValidateRowEventHandler(this.gridView1_ValidateRow);
		this.edType.Caption = "edType";
		this.edType.CurrencyDateField = null;
		this.edType.CurrencyField = null;
		this.edType.CurrencyFixed = null;
		this.edType.CurrencyFixedRate = 0.0;
		this.edType.CurrencyRateField = null;
		this.edType.FieldName = "edType";
		this.edType.Name = "edType";
		this.edType.PrimaryKey = false;
		this.edType.UnitField = null;
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
		this.name.VisibleIndex = 0;
		this.name.Width = 212;
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
		this.descriptio.VisibleIndex = 1;
		this.descriptio.Width = 370;
		this.btCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btCancel.EditControl = null;
		this.btCancel.EditValue = "Cancel";
		this.btCancel.FieldName = null;
		this.btCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btCancel.ImageOptions.Image");
		this.btCancel.IsSearchControl = false;
		this.btCancel.Location = new System.Drawing.Point(82, 352);
		this.btCancel.Name = "btCancel";
		this.btCancel.ReadOnly = false;
		this.btCancel.SettingInVisible = false;
		this.btCancel.SettingReadonly = false;
		this.btCancel.Size = new System.Drawing.Size(75, 23);
		this.btCancel.TabIndex = 9;
		this.btCancel.Text = "Cancel";
		this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
		this.btnOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnOK.EditControl = null;
		this.btnOK.EditValue = "OK";
		this.btnOK.FieldName = null;
		this.btnOK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnOK.ImageOptions.Image");
		this.btnOK.IsSearchControl = false;
		this.btnOK.Location = new System.Drawing.Point(4, 352);
		this.btnOK.Name = "btnOK";
		this.btnOK.ReadOnly = false;
		this.btnOK.SettingInVisible = false;
		this.btnOK.SettingReadonly = false;
		this.btnOK.Size = new System.Drawing.Size(75, 23);
		this.btnOK.TabIndex = 8;
		this.btnOK.Text = "OK";
		this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
		base.AcceptButton = this.btnOK;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(608, 378);
		base.Controls.Add(this.btCancel);
		base.Controls.Add(this.btnOK);
		base.Controls.Add(this.gridControl1);
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_HREducationTypes";
		base.ShowInTaskbar = false;
		this.Text = "Education Types - Setup";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_HREducationTypes_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.gridControl1).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gridView1).EndInit();
		base.ResumeLayout(false);
	}
}
