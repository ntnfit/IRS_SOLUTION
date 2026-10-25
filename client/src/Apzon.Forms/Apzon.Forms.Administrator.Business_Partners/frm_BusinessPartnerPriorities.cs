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
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Helper;
using Apzon.Forms.Properties;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Administrator.Business_Partners;

public class frm_BusinessPartnerPriorities : XtraFormEx
{
	private const string FormController = "BusinessPartnerPriorities";

	private DataTable _dtRemove = new DataTable();

	private bool _isDelete;

	private DataTable _dtOld;

	private IContainer components;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem deleteRowToolStripMenuItem;

	private GridViewEx gvDoc;

	private GridControlEx grdDoc;

	public SimpleButtonEx btn_Save;

	public SimpleButtonEx btnCancel;

	private GridColumnEx gridColumn1;

	private GridColumnEx gridColumn2;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Update:
			btn_Save.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btn_Save.Text = Apzon.Commons.Properties.Resources.ButtonOK;
			break;
		}
	}

	public frm_BusinessPartnerPriorities()
	{
		InitializeComponent();
	}

	private async void frm_BusinessPartnerPriorities_Load(object sender, EventArgs e)
	{
		try
		{
			await LoadData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadData()
	{
		try
		{
			FormMode = Enums.FormViewModeType.View;
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "BusinessPartnerPriorities");
			DataTable dataTable = await helper.GetAsync("LoadDoc");
			grdDoc.DataSource = dataTable;
			grdDoc.RefreshDataSource();
			gvDoc.SetGridColumnMaxLength("OBPP");
			_dtOld = dataTable.Copy();
			_dtRemove = (grdDoc.DataSource as DataTable).Copy().Clone();
			AddNew();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task SaveData()
	{
		try
		{
			DataTable dataTable = grdDoc.DataSourceTable.Copy();
			EnumerableRowCollection<string> source = from r in dataTable.AsEnumerable()
				select Function.ToString(r["OldPrioCode"]);
			foreach (DataRow row in _dtOld.Rows)
			{
				if (!source.Contains(Function.ToString(row["PrioCode"])))
				{
					DataRow dataRow2 = dataTable.NewRow();
					dataRow2["OldPrioCode"] = Function.ParseInt(row["PrioCode"]);
					dataTable.Rows.Add(dataRow2);
				}
			}
			dataTable.AcceptChanges();
			using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "BusinessPartnerPriorities");
			HttpResult httpResult = helper.Post("Save", dataTable);
			if (httpResult != null && string.IsNullOrEmpty(httpResult.Message))
			{
				await LoadData();
				WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void AddNew()
	{
		try
		{
			DataTable dataSourceTable = grdDoc.DataSourceTable;
			dataSourceTable.Rows.Add(dataSourceTable.NewRow());
			dataSourceTable.AcceptChanges();
			grdDoc.DataSource = dataSourceTable;
			grdDoc.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataTable = grdDoc.DataSourceTable.Copy();
			int focusedRowHandle = gvDoc.FocusedRowHandle;
			dataTable.Rows.RemoveAt(focusedRowHandle);
			dataTable.AcceptChanges();
			grdDoc.DataSource = dataTable;
			grdDoc.RefreshDataSource();
			FormMode = Enums.FormViewModeType.Update;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gvDoc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		FormMode = Enums.FormViewModeType.Update;
		try
		{
			if (grdDoc.DataSource is DataTable dataTable && e.RowHandle == dataTable.Rows.Count - 1 && (e.Column.FieldName == "PrioCode" || e.Column.FieldName == "PrioDesc"))
			{
				AddNew();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Save_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Update)
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					WinCommons._OpenProcessing();
					btn_Save.Enabled = false;
					await SaveData();
					btn_Save.Enabled = true;
					WinCommons._CloseProcessing();
				}
			}
			else
			{
				Close();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btnCancel_Click(object sender, EventArgs e)
	{
		if (FormMode == Enums.FormViewModeType.Update && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSaveBeforeClose, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
		{
			WinCommons._OpenProcessing();
			await SaveData();
			WinCommons._CloseProcessing();
		}
		Close();
	}

	private void gvDoc_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	public async Task REFRESH_DATA()
	{
		frm_BusinessPartnerPriorities_Load(null, null);
	}

	private async void grdDoc_EditorKeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.V && e.Modifiers == Keys.Control)
			{
				ExtenseionMethods.PasteFromExcelToDataTable(grdDoc, gvDoc, new List<string> { "PrioCode" });
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvDoc_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
	{
		try
		{
			if (!(gvDoc.FocusedColumn.FieldName == "PrioCode"))
			{
				return;
			}
			string text = Function.ToString(e.Value);
			if (text.Any((char c) => !char.IsDigit(c) && c != '.'))
			{
				e.ErrorText = "Invalid numeric value";
				e.Valid = false;
				return;
			}
			string[] array = text.Split('.');
			if (array == null)
			{
				return;
			}
			if (array.Length == 2)
			{
				if (array[1].Any((char c) => c != '0'))
				{
					e.ErrorText = "Invalid numeric value";
					e.Valid = false;
				}
				e.Value = array[0];
			}
			else if (array.Length > 2)
			{
				e.ErrorText = "Invalid numeric value";
				e.Valid = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvDoc_InvalidValueException(object sender, InvalidValueExceptionEventArgs e)
	{
		e.ExceptionMode = ExceptionMode.DisplayError;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Administrator.Business_Partners.frm_BusinessPartnerPriorities));
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gvDoc = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn2 = new Apzon.Controls.Grid.GridColumnEx();
		this.grdDoc = new Apzon.Controls.Grid.GridControlEx();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).BeginInit();
		base.SuspendLayout();
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.deleteRowToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(134, 26);
		this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
		this.deleteRowToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
		this.deleteRowToolStripMenuItem.Text = "Delete Row";
		this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.deleteRowToolStripMenuItem_Click);
		this.gvDoc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvDoc.ColumnKey = null;
		this.gvDoc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[2] { this.gridColumn1, this.gridColumn2 });
		this.gvDoc.DisableFormSetting = false;
		this.gvDoc.DisplayException = false;
		this.gvDoc.EditColumn = null;
		this.gvDoc.FieldCurrency = null;
		this.gvDoc.FieldRate = null;
		this.gvDoc.FormId = "oBusinessPartnerPriorities";
		this.gvDoc.GridControl = this.grdDoc;
		this.gvDoc.IgnoreFilter = false;
		this.gvDoc.IndexEditRow = -1;
		this.gvDoc.IndicatorWidth = 40;
		this.gvDoc.IsApzTable = false;
		this.gvDoc.IsInsertNewRow = false;
		this.gvDoc.IsLoadingData = false;
		this.gvDoc.IsMappingContextAction = false;
		this.gvDoc.ItemId = "OBPP";
		this.gvDoc.LstColumnAllowBatch = null;
		this.gvDoc.LstColumnNotAllowBatch = null;
		this.gvDoc.LstColumnNotEditable = null;
		this.gvDoc.LstColumNotSetting = new System.Collections.Generic.List<string> { "OldPrioCode" };
		this.gvDoc.Name = "gvDoc";
		this.gvDoc.OptionsSelection.MultiSelect = true;
		this.gvDoc.OptionsView.ShowGroupPanel = false;
		this.gvDoc.UseBatchMod = false;
		this.gvDoc.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gvDoc_CustomDrawRowIndicator);
		this.gvDoc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvDoc_CellValueChanged);
		this.gvDoc.ValidatingEditor += new DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventHandler(this.gvDoc_ValidatingEditor);
		this.gvDoc.InvalidValueException += new DevExpress.XtraEditors.Controls.InvalidValueExceptionEventHandler(this.gvDoc_InvalidValueException);
		this.gridColumn1.Caption = "Priority";
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.DisplayFormat.FormatString = "n0";
		this.gridColumn1.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gridColumn1.FieldName = "PrioCode";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.PrimaryKey = true;
		this.gridColumn1.UnitField = null;
		this.gridColumn1.Visible = true;
		this.gridColumn1.VisibleIndex = 0;
		this.gridColumn1.Width = 147;
		this.gridColumn2.Caption = "Priority Description";
		this.gridColumn2.CurrencyDateField = null;
		this.gridColumn2.CurrencyField = null;
		this.gridColumn2.CurrencyFixed = null;
		this.gridColumn2.CurrencyFixedRate = 0.0;
		this.gridColumn2.CurrencyRateField = null;
		this.gridColumn2.FieldName = "PrioDesc";
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.PrimaryKey = false;
		this.gridColumn2.UnitField = null;
		this.gridColumn2.Visible = true;
		this.gridColumn2.VisibleIndex = 1;
		this.gridColumn2.Width = 325;
		this.grdDoc.AllowPasteExcel = true;
		this.grdDoc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grdDoc.ClipboardData = null;
		this.grdDoc.ContextMenuStrip = this.contextMenuStrip1;
		this.grdDoc.Location = new System.Drawing.Point(2, 2);
		this.grdDoc.MainView = this.gvDoc;
		this.grdDoc.Name = "grdDoc";
		this.grdDoc.Size = new System.Drawing.Size(516, 287);
		this.grdDoc.TabIndex = 68;
		this.grdDoc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvDoc });
		this.grdDoc.EditorKeyDown += new System.Windows.Forms.KeyEventHandler(this.grdDoc_EditorKeyDown);
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Save.Appearance.Options.UseBackColor = true;
		this.btn_Save.EditControl = null;
		this.btn_Save.EditValue = "OK";
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = Apzon.Forms.Properties.Resources.apply_16x16;
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(5, 297);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.ReadOnly = false;
		this.btn_Save.SettingInVisible = false;
		this.btn_Save.SettingReadonly = false;
		this.btn_Save.Size = new System.Drawing.Size(99, 20);
		this.btn_Save.TabIndex = 67;
		this.btn_Save.Text = "OK";
		this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnCancel.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btnCancel.Appearance.Options.UseBackColor = true;
		this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btnCancel.EditControl = null;
		this.btnCancel.EditValue = "Cancel";
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(106, 297);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(99, 20);
		this.btnCancel.TabIndex = 66;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(518, 324);
		base.Controls.Add(this.grdDoc);
		base.Controls.Add(this.btn_Save);
		base.Controls.Add(this.btnCancel);
		base.FormId = "oBusinessPartnerPriorities";
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_BusinessPartnerPriorities.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_BusinessPartnerPriorities";
		this.Text = "Business Partner Priorities";
		base.Load += new System.EventHandler(this.frm_BusinessPartnerPriorities_Load);
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gvDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).EndInit();
		base.ResumeLayout(false);
	}
}
