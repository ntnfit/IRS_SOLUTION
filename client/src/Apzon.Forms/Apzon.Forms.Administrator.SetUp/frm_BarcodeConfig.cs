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
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.Administrator.SetUp;

public class frm_BarcodeConfig : XtraFormEx
{
	private Enums.FormAuthorities authorities;

	private const string Controller = "BarConfig";

	private IContainer components;

	private SimpleButtonEx btnCancel;

	private SimpleButtonEx btnSave;

	private GridControlEx grdDoc;

	private GridViewEx gvDoc;

	private GridColumnEx gcObj;

	private GridColumnEx gcObjType;

	private GridColumnEx gcDocEn;

	private GridColumnEx gcLineId;

	private GridColumnEx gcOrder;

	private RepositoryItemLookUpEdit rObj;

	private ContextMenuStrip contextMenuStrip;

	private ToolStripMenuItem insertConfigToolStripMenuItem;

	private RepositoryItemCalcEdit rcal;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Update:
			btnSave.Text = Resources.ButtonSave;
			break;
		case Enums.FormViewModeType.View:
			btnSave.Text = Resources.ButtonOK;
			break;
		}
	}

	public frm_BarcodeConfig()
	{
		try
		{
			if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_BarcodeConfig"))
			{
				InitializeComponent();
				if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_BarcodeConfig" && x.authoriza == "F"))
				{
					authorities = Enums.FormAuthorities.Read;
				}
				else
				{
					authorities = Enums.FormAuthorities.Full;
				}
				FormMode = Enums.FormViewModeType.View;
				return;
			}
			throw new Exception("Not access form!");
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	private async void frm_BarcodeConfig_Load(object sender, EventArgs e)
	{
		try
		{
			LoadOgjectType();
			FormMode = Enums.FormViewModeType.View;
			DataTable dataTable = await GetData();
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				grdDoc.DataSource = dataTable;
				grdDoc.RefreshDataSource();
			}
			else
			{
				EnableDisableControl(isEnable: false);
				WinCommons.Error(Enums.MessageCode.ErrorConnect2Server, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private async void btnSave_Click(object sender, EventArgs e)
	{
		Enums.FormViewModeType formMode = FormMode;
		WinCommons._OpenProcessing();
		EnableDisableControl(isEnable: false);
		try
		{
			if (formMode == Enums.FormViewModeType.View)
			{
				Close();
			}
			else if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				await Save();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		EnableDisableControl(isEnable: true);
		WinCommons._CloseProcessing();
	}

	private void insertConfigToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataTable = grdDoc.DataSourceTable.Clone();
			DataRow row = dataTable.NewRow();
			dataTable.Rows.Add(row);
			grdDoc.DataSource = dataTable;
			grdDoc.RefreshDataSource();
			FormMode = Enums.FormViewModeType.Update;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvDoc_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
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

	public async Task MODE_FIND()
	{
		try
		{
			DataTable dataTable = await GetData();
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				grdDoc.DataSource = dataTable;
				grdDoc.RefreshDataSource();
			}
			else
			{
				EnableDisableControl(isEnable: false);
				WinCommons.Error(Enums.MessageCode.ErrorConnect2Server, "");
			}
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task REFRESH()
	{
	}

	private void LoadOgjectType()
	{
		try
		{
			List<Common.Table_ID> dataSource = new List<Common.Table_ID>
			{
				new Common.Table_ID
				{
					Name = "Production order",
					ID = 202
				},
				new Common.Table_ID
				{
					Name = "Purchase order",
					ID = 22
				}
			};
			rObj.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task<DataTable> GetData()
	{
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "BarConfig");
		return await http.GetAsync("GetData");
	}

	private async Task Save()
	{
		DataTable dataSourceTable = grdDoc.DataSourceTable;
		if (dataSourceTable != null && dataSourceTable.Rows.Count > 0)
		{
			using HttpClientHelper<string> http = new HttpClientHelper<string>(Configuration.ApiServer, "BarConfig");
			string text = await http.PostAsync("Save", dataSourceTable);
			if (string.IsNullOrEmpty(text))
			{
				FormMode = Enums.FormViewModeType.View;
				dataSourceTable = await GetData();
				if (dataSourceTable != null && dataSourceTable.Rows.Count > 0)
				{
					grdDoc.DataSource = dataSourceTable;
					grdDoc.RefreshDataSource();
				}
				else
				{
					grdDoc.DataSource = null;
					grdDoc.RefreshDataSource();
				}
				WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, text);
			}
		}
		else
		{
			WinCommons.Error(Enums.MessageCode.NoDataToSave, "");
		}
	}

	private void EnableDisableControl(bool isEnable)
	{
		try
		{
			FormMode = (isEnable ? Enums.FormViewModeType.View : Enums.FormViewModeType.Busy);
			btnCancel.Enabled = isEnable;
			btnSave.Enabled = isEnable;
			gvDoc.ReadOnly = !isEnable;
			contextMenuStrip.Enabled = isEnable;
			if (isEnable && authorities == Enums.FormAuthorities.Read)
			{
				btnSave.Enabled = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void frm_BarcodeConfig_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Update && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
			{
				e.Cancel = true;
			}
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Administrator.SetUp.frm_BarcodeConfig));
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnSave = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grdDoc = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.insertConfigToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gvDoc = new Apzon.Controls.Grid.GridViewEx();
		this.gcObj = new Apzon.Controls.Grid.GridColumnEx();
		this.rObj = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcObjType = new Apzon.Controls.Grid.GridColumnEx();
		this.rcal = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gcDocEn = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineId = new Apzon.Controls.Grid.GridColumnEx();
		this.gcOrder = new Apzon.Controls.Grid.GridColumnEx();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).BeginInit();
		this.contextMenuStrip.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rObj).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rcal).BeginInit();
		base.SuspendLayout();
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnCancel.EditControl = null;
		this.btnCancel.EditValue = "Cancel";
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(448, 217);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(75, 20);
		this.btnCancel.TabIndex = 6;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.btnSave.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnSave.ButtonKind = Apzon.Commons.Enums.Enums.SimpleButtonType.Save;
		this.btnSave.EditControl = null;
		this.btnSave.EditValue = "Save";
		this.btnSave.FieldName = null;
		this.btnSave.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnSave.ImageOptions.Image");
		this.btnSave.IsSearchControl = false;
		this.btnSave.Location = new System.Drawing.Point(370, 217);
		this.btnSave.Name = "btnSave";
		this.btnSave.ReadOnly = false;
		this.btnSave.SettingInVisible = false;
		this.btnSave.SettingReadonly = false;
		this.btnSave.Size = new System.Drawing.Size(75, 20);
		this.btnSave.TabIndex = 5;
		this.btnSave.Text = "Save";
		this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
		this.grdDoc.AllowPasteExcel = true;
		this.grdDoc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grdDoc.ClipboardData = null;
		this.grdDoc.ContextMenuStrip = this.contextMenuStrip;
		this.grdDoc.Location = new System.Drawing.Point(0, 0);
		this.grdDoc.MainView = this.gvDoc;
		this.grdDoc.Name = "grdDoc";
		this.grdDoc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.rObj, this.rcal });
		this.grdDoc.Size = new System.Drawing.Size(527, 213);
		this.grdDoc.TabIndex = 7;
		this.grdDoc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvDoc });
		this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.insertConfigToolStripMenuItem });
		this.contextMenuStrip.Name = "contextMenuStrip";
		this.contextMenuStrip.Size = new System.Drawing.Size(141, 26);
		this.insertConfigToolStripMenuItem.Name = "insertConfigToolStripMenuItem";
		this.insertConfigToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
		this.insertConfigToolStripMenuItem.Text = "Insert config";
		this.insertConfigToolStripMenuItem.Visible = false;
		this.insertConfigToolStripMenuItem.Click += new System.EventHandler(this.insertConfigToolStripMenuItem_Click);
		this.gvDoc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvDoc.ColumnKey = null;
		this.gvDoc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[5] { this.gcObj, this.gcObjType, this.gcDocEn, this.gcLineId, this.gcOrder });
		this.gvDoc.DisableFormSetting = false;
		this.gvDoc.DisplayException = false;
		this.gvDoc.EditColumn = null;
		this.gvDoc.FieldCurrency = null;
		this.gvDoc.FieldRate = null;
		this.gvDoc.FormId = "oBarcodeConfig";
		this.gvDoc.GridControl = this.grdDoc;
		this.gvDoc.IgnoreFilter = false;
		this.gvDoc.IndexEditRow = -1;
		this.gvDoc.IndicatorWidth = 35;
		this.gvDoc.IsApzTable = false;
		this.gvDoc.IsInsertNewRow = false;
		this.gvDoc.IsLoadingData = false;
		this.gvDoc.IsMappingContextAction = false;
		this.gvDoc.ItemId = "APZ_OCPB";
		this.gvDoc.LstColumnAllowBatch = null;
		this.gvDoc.LstColumnNotAllowBatch = null;
		this.gvDoc.LstColumnNotEditable = null;
		this.gvDoc.LstColumNotSetting = null;
		this.gvDoc.Name = "gvDoc";
		this.gvDoc.OptionsView.ShowGroupPanel = false;
		this.gvDoc.UseBatchMod = false;
		this.gvDoc.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvDoc_CellValueChanging);
		this.gcObj.Caption = "Object";
		this.gcObj.ColumnEdit = this.rObj;
		this.gcObj.CurrencyDateField = null;
		this.gcObj.CurrencyField = null;
		this.gcObj.CurrencyFixed = null;
		this.gcObj.CurrencyFixedRate = 0.0;
		this.gcObj.CurrencyRateField = null;
		this.gcObj.FieldName = "ObjType";
		this.gcObj.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gcObj.Name = "gcObj";
		this.gcObj.OptionsColumn.ReadOnly = true;
		this.gcObj.PrimaryKey = true;
		this.gcObj.UnitField = null;
		this.gcObj.Visible = true;
		this.gcObj.VisibleIndex = 0;
		this.rObj.AutoHeight = false;
		this.rObj.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rObj.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name1"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ID", "Name2", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rObj.DisplayMember = "Name";
		this.rObj.Name = "rObj";
		this.rObj.NullText = "";
		this.rObj.ShowFooter = false;
		this.rObj.ShowHeader = false;
		this.rObj.ValueMember = "ID";
		this.gcObjType.Caption = "Barcode Type Length";
		this.gcObjType.ColumnEdit = this.rcal;
		this.gcObjType.CurrencyDateField = null;
		this.gcObjType.CurrencyField = null;
		this.gcObjType.CurrencyFixed = null;
		this.gcObjType.CurrencyFixedRate = 0.0;
		this.gcObjType.CurrencyRateField = null;
		this.gcObjType.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Integer;
		this.gcObjType.FieldName = "BarType";
		this.gcObjType.Name = "gcObjType";
		this.gcObjType.PrimaryKey = false;
		this.gcObjType.UnitField = null;
		this.gcObjType.Visible = true;
		this.gcObjType.VisibleIndex = 1;
		this.gcObjType.Width = 115;
		this.rcal.AutoHeight = false;
		this.rcal.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rcal.Name = "rcal";
		this.gcDocEn.Caption = "DocEntry length";
		this.gcDocEn.ColumnEdit = this.rcal;
		this.gcDocEn.CurrencyDateField = null;
		this.gcDocEn.CurrencyField = null;
		this.gcDocEn.CurrencyFixed = null;
		this.gcDocEn.CurrencyFixedRate = 0.0;
		this.gcDocEn.CurrencyRateField = null;
		this.gcDocEn.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Integer;
		this.gcDocEn.FieldName = "DocEn";
		this.gcDocEn.Name = "gcDocEn";
		this.gcDocEn.PrimaryKey = false;
		this.gcDocEn.UnitField = null;
		this.gcDocEn.Visible = true;
		this.gcDocEn.VisibleIndex = 2;
		this.gcDocEn.Width = 96;
		this.gcLineId.Caption = "Line Length";
		this.gcLineId.ColumnEdit = this.rcal;
		this.gcLineId.CurrencyDateField = null;
		this.gcLineId.CurrencyField = null;
		this.gcLineId.CurrencyFixed = null;
		this.gcLineId.CurrencyFixedRate = 0.0;
		this.gcLineId.CurrencyRateField = null;
		this.gcLineId.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Integer;
		this.gcLineId.FieldName = "LineId";
		this.gcLineId.Name = "gcLineId";
		this.gcLineId.PrimaryKey = false;
		this.gcLineId.UnitField = null;
		this.gcLineId.Visible = true;
		this.gcLineId.VisibleIndex = 3;
		this.gcLineId.Width = 96;
		this.gcOrder.Caption = "Order length";
		this.gcOrder.ColumnEdit = this.rcal;
		this.gcOrder.CurrencyDateField = null;
		this.gcOrder.CurrencyField = null;
		this.gcOrder.CurrencyFixed = null;
		this.gcOrder.CurrencyFixedRate = 0.0;
		this.gcOrder.CurrencyRateField = null;
		this.gcOrder.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Integer;
		this.gcOrder.FieldName = "Order";
		this.gcOrder.Name = "gcOrder";
		this.gcOrder.PrimaryKey = false;
		this.gcOrder.UnitField = null;
		this.gcOrder.Visible = true;
		this.gcOrder.VisibleIndex = 4;
		this.gcOrder.Width = 106;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(527, 241);
		base.Controls.Add(this.grdDoc);
		base.Controls.Add(this.btnSave);
		base.Controls.Add(this.btnCancel);
		base.FormId = "oBarcodeConfig";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_BarcodeConfig";
		this.Text = "Barcode Config";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_BarcodeConfig_FormClosing);
		base.Load += new System.EventHandler(this.frm_BarcodeConfig_Load);
		((System.ComponentModel.ISupportInitialize)this.grdDoc).EndInit();
		this.contextMenuStrip.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gvDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rObj).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rcal).EndInit();
		base.ResumeLayout(false);
	}
}
