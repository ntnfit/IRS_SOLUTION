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
using Apzon.Forms.Helper;
using Apzon.HttpClientHelper;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.ItemsMaster;

public class frm_Shipping : XtraFormEx
{
	private const string FormController = "ShippingTypes";

	private DataTable _dtStruct;

	public string _comboValue = "";

	private IContainer components;

	private GridColumnEx STT;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem addNewToolStripMenuItem;

	private ToolStripMenuItem deleteToolStripMenuItem;

	private GridViewEx tblViewManufacturer;

	private SimpleButtonEx btOK;

	private SimpleButtonEx btCancel;

	private GridControlEx grdDoc;

	private GridViewEx gvDoc;

	private GridColumnEx gcTrnspCode;

	private GridColumnEx gcTrnspName;

	private GridColumnEx gcWebSite;

	private GridColumnEx UserSign;

	private GridColumnEx gridColumn1;

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
		case Enums.FormViewModeType.Add:
			btOK.Text = Resources.ButtonAdd;
			break;
		case Enums.FormViewModeType.Update:
			btOK.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btOK.Text = Resources.ButtonOK;
			break;
		}
	}

	public frm_Shipping()
	{
		InitializeComponent();
		OnLoadAsync += frmShipping_Load;
	}

	public async Task REFRESH_DATA()
	{
		if ((FormMode != Enums.FormViewModeType.Add && FormMode != Enums.FormViewModeType.Update) || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
		{
			FormMode = Enums.FormViewModeType.View;
			frmShipping_Load(null, null);
		}
	}

	private async Task frmShipping_Load(object sender, EventArgs e)
	{
		try
		{
			_dtStruct = WinFuntions.GetTableStructure("OSHP");
			await RefreshData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void grdDoc_EditorKeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.V && e.Modifiers == Keys.Control)
			{
				ExtenseionMethods.PasteFromExcelToDataTable(grdDoc, gvDoc, new List<string> { "TrnspName" });
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			int num = Function.ParseInt(gvDoc.GetFocusedRowCellValue("TrnspCode"));
			if (num <= 0)
			{
				return;
			}
			using (HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ShippingTypes"))
			{
				DataTable dataTable = httpClientHelper.Get("CheckShippingOITM", new
				{
					trnspCode = num
				});
				if (dataTable != null && dataTable.Rows.Count > 0)
				{
					WinCommons.Error(Enums.MessageCode.CannotRemoveObjectInUseInDocuments, "");
					return;
				}
			}
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				return;
			}
			using HttpClientHelper<HttpResult> httpClientHelper2 = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "ShippingTypes");
			WinCommons._OpenProcessing();
			HttpResult httpResult = httpClientHelper2.Post("DeleteDoc", num);
			WinCommons._CloseProcessing();
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				return;
			}
			WinCommons.Success(Enums.MessageCode.RemoveSuccess, "");
			gvDoc.BeginUpdate();
			gvDoc.DeleteSelectedRows();
			gvDoc.EndUpdate();
			DataTable dataTable2 = (DataTable)grdDoc.DataSource;
			dataTable2.AcceptChanges();
			grdDoc.DataSource = dataTable2;
			grdDoc.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void addNewToolStripMenuItem_Click(object sender, EventArgs e)
	{
		AddNew();
	}

	private void gvDoc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if ((grdDoc.DataSource as DataTable).IsNotNull() && (e.Column.FieldName == "WebSite" || e.Column.FieldName == "TrnspName") && FormMode != Enums.FormViewModeType.Update)
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

	private async void btCancel_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Update && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSaveBeforeClose, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				await Save();
			}
			Close();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btOK_Click(object sender, EventArgs e)
	{
		DisableButtons(disable: true);
		try
		{
			if (FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update)
			{
				WinCommons._OpenProcessing();
				await Save();
				WinCommons._CloseProcessing();
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
		DisableButtons(disable: false);
	}

	private void DisableButtons(bool disable)
	{
		try
		{
			btOK.Enabled = !disable;
			btCancel.Enabled = !disable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task Save()
	{
		try
		{
			DataTable dataTable = grdDoc.DataSource as DataTable;
			if (!dataTable.IsNotNull())
			{
				return;
			}
			if (dataTable.AsEnumerable().Any((DataRow r) => string.IsNullOrEmpty(Function.ToString(r["TrnspName"])) && Function.ParseInt(r["TrnspCode"]) > 0))
			{
				WinCommons.Error(Enums.MessageCode.NotEnoughDataName, "");
				return;
			}
			EnumerableRowCollection<DataRow> source = from r in dataTable.AsEnumerable()
				where !string.IsNullOrEmpty(Function.ToString(r["TrnspName"]))
				select r;
			if (!source.Any())
			{
				return;
			}
			DataTable dataTable2 = source.CopyToDataTable();
			List<IGrouping<string, DataRow>> list = (from i in dataTable2.AsEnumerable()
				group i by Function.ToString(i.Field<string>("TrnspName")).ToUpper() into gr
				where gr.Count() > 1
				select gr).ToList();
			if (list != null && list.Count > 0)
			{
				WinCommons.Error(Enums.MessageCode.ThisEntryAlreadyExistsInFollwingTableDeliveryType, "");
				return;
			}
			using HttpClientHelper<HttpResult> _help = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "ShippingTypes");
			HttpResult httpResult = await _help.PostAsync("SaveDoc", dataTable2);
			if (httpResult != null && string.IsNullOrEmpty(httpResult.Message))
			{
				WinCommons.Success(Enums.MessageCode.OperationCompletedSuccess, "");
				RefreshData();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void AddNew()
	{
		try
		{
			DataTable dataTable = grdDoc.DataSource as DataTable;
			if (dataTable == null || dataTable.Columns.Count == 0)
			{
				dataTable = _dtStruct.Clone();
			}
			DataRow dataRow = dataTable.NewRow();
			dataRow["TrnspCode"] = 0;
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			grdDoc.DataSource = dataTable;
			grdDoc.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task RefreshData()
	{
		try
		{
			using HttpClientHelper<DataTable> _help = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ShippingTypes");
			DataTable dataSource = await _help.GetAsync("ShowShippingTypes");
			grdDoc.DataSource = dataSource;
			grdDoc.RefreshDataSource();
			AddNew();
			FormMode = Enums.FormViewModeType.View;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.ItemsMaster.frm_Shipping));
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addNewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.tblViewManufacturer = new Apzon.Controls.Grid.GridViewEx();
		this.STT = new Apzon.Controls.Grid.GridColumnEx();
		this.btOK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grdDoc = new Apzon.Controls.Grid.GridControlEx();
		this.gvDoc = new Apzon.Controls.Grid.GridViewEx();
		this.gcTrnspCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gcTrnspName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcWebSite = new Apzon.Controls.Grid.GridColumnEx();
		this.UserSign = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.tblViewManufacturer).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).BeginInit();
		base.SuspendLayout();
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addNewToolStripMenuItem, this.deleteToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(124, 48);
		this.addNewToolStripMenuItem.Name = "addNewToolStripMenuItem";
		this.addNewToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
		this.addNewToolStripMenuItem.Text = "Add New";
		this.addNewToolStripMenuItem.Click += new System.EventHandler(this.addNewToolStripMenuItem_Click);
		this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
		this.deleteToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
		this.deleteToolStripMenuItem.Text = "Delete";
		this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
		this.tblViewManufacturer.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.tblViewManufacturer.ColumnKey = null;
		this.tblViewManufacturer.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[1] { this.STT });
		this.tblViewManufacturer.DisableFormSetting = false;
		this.tblViewManufacturer.DisplayException = false;
		this.tblViewManufacturer.EditColumn = null;
		this.tblViewManufacturer.FieldCurrency = null;
		this.tblViewManufacturer.FieldRate = null;
		this.tblViewManufacturer.FormId = "oShipping";
		this.tblViewManufacturer.IgnoreFilter = false;
		this.tblViewManufacturer.IndexEditRow = -1;
		this.tblViewManufacturer.IndicatorWidth = 35;
		this.tblViewManufacturer.IsApzTable = false;
		this.tblViewManufacturer.IsInsertNewRow = false;
		this.tblViewManufacturer.IsLoadingData = false;
		this.tblViewManufacturer.IsMappingContextAction = false;
		this.tblViewManufacturer.ItemId = "OFACTURE";
		this.tblViewManufacturer.LstColumnAllowBatch = null;
		this.tblViewManufacturer.LstColumnNotAllowBatch = null;
		this.tblViewManufacturer.LstColumnNotEditable = null;
		this.tblViewManufacturer.LstColumNotSetting = null;
		this.tblViewManufacturer.Name = "tblViewManufacturer";
		this.tblViewManufacturer.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
		this.tblViewManufacturer.UseBatchMod = false;
		this.STT.Caption = "STT";
		this.STT.CurrencyDateField = null;
		this.STT.CurrencyField = null;
		this.STT.CurrencyFixed = null;
		this.STT.CurrencyFixedRate = 0.0;
		this.STT.CurrencyRateField = null;
		this.STT.Name = "STT";
		this.STT.PrimaryKey = false;
		this.STT.UnitField = null;
		this.STT.Visible = true;
		this.STT.VisibleIndex = 0;
		this.btOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btOK.EditControl = null;
		this.btOK.EditValue = "OK";
		this.btOK.FieldName = null;
		this.btOK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btOK.ImageOptions.Image");
		this.btOK.IsSearchControl = false;
		this.btOK.Location = new System.Drawing.Point(8, 253);
		this.btOK.Name = "btOK";
		this.btOK.ReadOnly = false;
		this.btOK.SettingInVisible = false;
		this.btOK.SettingReadonly = false;
		this.btOK.Size = new System.Drawing.Size(63, 22);
		this.btOK.TabIndex = 34;
		this.btOK.Text = "OK";
		this.btOK.Click += new System.EventHandler(this.btOK_Click);
		this.btCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btCancel.EditControl = null;
		this.btCancel.EditValue = "Cancel";
		this.btCancel.FieldName = null;
		this.btCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btCancel.ImageOptions.Image");
		this.btCancel.IsSearchControl = false;
		this.btCancel.Location = new System.Drawing.Point(74, 253);
		this.btCancel.Name = "btCancel";
		this.btCancel.ReadOnly = false;
		this.btCancel.SettingInVisible = false;
		this.btCancel.SettingReadonly = false;
		this.btCancel.Size = new System.Drawing.Size(62, 22);
		this.btCancel.TabIndex = 35;
		this.btCancel.Text = "Cancel";
		this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
		this.grdDoc.AllowPasteExcel = true;
		this.grdDoc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grdDoc.ClipboardData = null;
		this.grdDoc.ContextMenuStrip = this.contextMenuStrip1;
		this.grdDoc.Location = new System.Drawing.Point(8, 8);
		this.grdDoc.MainView = this.gvDoc;
		this.grdDoc.Name = "grdDoc";
		this.grdDoc.Size = new System.Drawing.Size(450, 239);
		this.grdDoc.TabIndex = 3;
		this.grdDoc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvDoc });
		this.grdDoc.EditorKeyDown += new System.Windows.Forms.KeyEventHandler(this.grdDoc_EditorKeyDown);
		this.gvDoc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvDoc.ColumnKey = null;
		this.gvDoc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[5] { this.gcTrnspCode, this.gcTrnspName, this.gcWebSite, this.UserSign, this.gridColumn1 });
		this.gvDoc.DisableFormSetting = false;
		this.gvDoc.DisplayException = false;
		this.gvDoc.EditColumn = null;
		this.gvDoc.FieldCurrency = null;
		this.gvDoc.FieldRate = null;
		this.gvDoc.FormId = "oShipping";
		this.gvDoc.GridControl = this.grdDoc;
		this.gvDoc.IgnoreFilter = false;
		this.gvDoc.IndexEditRow = -1;
		this.gvDoc.IndicatorWidth = 35;
		this.gvDoc.IsApzTable = false;
		this.gvDoc.IsInsertNewRow = false;
		this.gvDoc.IsLoadingData = false;
		this.gvDoc.IsMappingContextAction = false;
		this.gvDoc.ItemId = "OSHP";
		this.gvDoc.LstColumnAllowBatch = null;
		this.gvDoc.LstColumnNotAllowBatch = null;
		this.gvDoc.LstColumnNotEditable = null;
		this.gvDoc.LstColumNotSetting = null;
		this.gvDoc.Name = "gvDoc";
		this.gvDoc.OptionsView.ColumnAutoWidth = false;
		this.gvDoc.OptionsView.ShowDetailButtons = false;
		this.gvDoc.OptionsView.ShowGroupPanel = false;
		this.gvDoc.UseBatchMod = false;
		this.gvDoc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvDoc_CellValueChanged);
		this.gcTrnspCode.Caption = "Code";
		this.gcTrnspCode.CurrencyDateField = null;
		this.gcTrnspCode.CurrencyField = null;
		this.gcTrnspCode.CurrencyFixed = null;
		this.gcTrnspCode.CurrencyFixedRate = 0.0;
		this.gcTrnspCode.CurrencyRateField = null;
		this.gcTrnspCode.FieldName = "TrnspCode";
		this.gcTrnspCode.Name = "gcTrnspCode";
		this.gcTrnspCode.OptionsColumn.AllowEdit = false;
		this.gcTrnspCode.OptionsColumn.ReadOnly = true;
		this.gcTrnspCode.PrimaryKey = false;
		this.gcTrnspCode.UnitField = null;
		this.gcTrnspCode.Width = 131;
		this.gcTrnspName.Caption = "Name";
		this.gcTrnspName.CurrencyDateField = null;
		this.gcTrnspName.CurrencyField = null;
		this.gcTrnspName.CurrencyFixed = null;
		this.gcTrnspName.CurrencyFixedRate = 0.0;
		this.gcTrnspName.CurrencyRateField = null;
		this.gcTrnspName.FieldName = "TrnspName";
		this.gcTrnspName.Name = "gcTrnspName";
		this.gcTrnspName.PrimaryKey = false;
		this.gcTrnspName.UnitField = null;
		this.gcTrnspName.Visible = true;
		this.gcTrnspName.VisibleIndex = 0;
		this.gcTrnspName.Width = 200;
		this.gcWebSite.Caption = "Web Site";
		this.gcWebSite.CurrencyDateField = null;
		this.gcWebSite.CurrencyField = null;
		this.gcWebSite.CurrencyFixed = null;
		this.gcWebSite.CurrencyFixedRate = 0.0;
		this.gcWebSite.CurrencyRateField = null;
		this.gcWebSite.FieldName = "WebSite";
		this.gcWebSite.Name = "gcWebSite";
		this.gcWebSite.PrimaryKey = false;
		this.gcWebSite.UnitField = null;
		this.gcWebSite.Visible = true;
		this.gcWebSite.VisibleIndex = 1;
		this.gcWebSite.Width = 209;
		this.UserSign.Caption = "UserSign";
		this.UserSign.CurrencyDateField = null;
		this.UserSign.CurrencyField = null;
		this.UserSign.CurrencyFixed = null;
		this.UserSign.CurrencyFixedRate = 0.0;
		this.UserSign.CurrencyRateField = null;
		this.UserSign.FieldName = "UserSign";
		this.UserSign.Name = "UserSign";
		this.UserSign.PrimaryKey = false;
		this.UserSign.UnitField = null;
		this.UserSign.Width = 271;
		this.gridColumn1.Caption = "DataSource";
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "DataSource";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.PrimaryKey = false;
		this.gridColumn1.UnitField = null;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(466, 284);
		base.Controls.Add(this.btCancel);
		base.Controls.Add(this.btOK);
		base.Controls.Add(this.grdDoc);
		base.FormId = "oShipping";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_Shipping";
		this.Text = "Shipping";
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.tblViewManufacturer).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).EndInit();
		base.ResumeLayout(false);
	}
}
