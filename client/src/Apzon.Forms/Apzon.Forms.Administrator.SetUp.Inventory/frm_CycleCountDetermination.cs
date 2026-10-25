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
using Apzon.Entities.Models.Common;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;

namespace Apzon.Forms.Administrator.SetUp.Inventory;

public class frm_CycleCountDetermination : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private IContainer components;

	private GridControlEx grid_CylesCount;

	private GridViewEx gv_CyclesCount;

	private SimpleButtonEx btn_Ok;

	private SimpleButtonEx btn_Cancel;

	private GridColumnEx WhsCode;

	private GridColumnEx WhsName;

	private GridColumnEx CycleBy;

	private GridColumnEx BinActivat;

	private RepositoryItemLookUpEdit look_CyclesBy;

	private GridColumnEx CycleType;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Update:
			btn_Ok.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btn_Ok.Text = Resources.ButtonOK;
			break;
		}
	}

	public frm_CycleCountDetermination()
	{
		InitializeComponent();
	}

	private async Task frm_CycleCountDetermination_Load(object sender, EventArgs e)
	{
		try
		{
			SetDataCylesBy();
			GridControlEx gridControlEx = grid_CylesCount;
			gridControlEx.DataSource = await GetList();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private async void btn_Ok_Click(object sender, EventArgs e)
	{
		_ = FormMode;
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			if (FormMode == Enums.FormViewModeType.Update)
			{
				await Save();
			}
			else
			{
				Close();
				WinCommons._OpenProcessing();
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private void gv_CyclesCount_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		try
		{
			if (e.Info.IsRowIndicator && e.RowHandle >= 0)
			{
				e.Info.DisplayText = (e.RowHandle + 1).ToString();
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_CyclesCount_ShowingEditor(object sender, CancelEventArgs e)
	{
		if (Function.ToString(gv_CyclesCount.GetFocusedRowCellValue("BinActivat")) == "Y")
		{
			e.Cancel = false;
		}
		else
		{
			e.Cancel = true;
		}
	}

	private void look_CyclesBy_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			FormMode = Enums.FormViewModeType.Update;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_CyclesCount_DoubleClick(object sender, EventArgs e)
	{
		try
		{
			GridView gridView = (GridView)sender;
			Point pt = gridView.GridControl.PointToClient(Control.MousePosition);
			GridHitInfo gridHitInfo = gridView.CalcHitInfo(pt);
			if ((gridHitInfo.InRow || gridHitInfo.InRowCell) && gridHitInfo.Column == null)
			{
				WinCommons._OpenProcessing();
				string whsCode = Function.ToString(gv_CyclesCount.GetFocusedRowCellValue("WhsCode"));
				int cycleBy = Function.ParseInt(gv_CyclesCount.GetFocusedRowCellValue("CycleBy"));
				new frm_CycleCountDeterminationSubtable(whsCode, cycleBy).ShowDialog(this);
				WinCommons._CloseProcessing();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<DataTable> GetList()
	{
		return await _httpClient.CycleCountDetermination.GetList();
	}

	public void SetDataCylesBy()
	{
		List<Common.Table_Code> list = new List<Common.Table_Code>();
		list.Add(new Common.Table_Code
		{
			Name = "Item Group",
			Code = "0"
		});
		list.Add(new Common.Table_Code
		{
			Name = "Warehouse sublevel-Area",
			Code = "1"
		});
		list.Add(new Common.Table_Code
		{
			Name = "Warehouse sublevel-Row",
			Code = "2"
		});
		list.Add(new Common.Table_Code
		{
			Name = "Warehouse sublevel-Cell",
			Code = "3"
		});
		list.Add(new Common.Table_Code
		{
			Name = "Warehouse sublevel-Shelf",
			Code = "4"
		});
		look_CyclesBy.DataSource = list;
	}

	public void GetDataLine(ref DataTable dt)
	{
		try
		{
			DataTable dataSourceTable = grid_CylesCount.DataSourceTable;
			dt = new DataTable();
			dt.Columns.Add("WhsCode", typeof(string));
			dt.Columns.Add("CycleType", typeof(string));
			dt.Columns.Add("CycleBy", typeof(int));
			for (int i = 0; i < dataSourceTable.Rows.Count; i++)
			{
				DataRow dataRow = dt.NewRow();
				dataRow["WhsCode"] = Function.ToString(gv_CyclesCount.GetFocusedRowCellValue("WhsCode"));
				dataRow["CycleType"] = Function.ToString(gv_CyclesCount.GetFocusedRowCellValue("CycleType"));
				dataRow["CycleBy"] = Function.ParseInt(gv_CyclesCount.GetFocusedRowCellValue("CycleBy"));
				dt.Rows.Add(dataRow);
				dt.AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<int> Save()
	{
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				WinCommons._OpenProcessing();
				btn_Ok.Enabled = false;
				DataTable dataSourceTable = grid_CylesCount.DataSourceTable;
				HttpResult httpResult = await _httpClient.CycleCountDetermination.Saves(dataSourceTable);
				if (string.IsNullOrEmpty(httpResult.Message))
				{
					WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
					FormMode = Enums.FormViewModeType.View;
					btn_Ok.Enabled = true;
					WinCommons._CloseProcessing();
					return 0;
				}
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				return 1;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return 1;
		}
		WinCommons._CloseProcessing();
		return 1;
	}

	private void frm_CycleCountDetermination_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCloseForm, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
			{
				e.Cancel = true;
			}
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Administrator.SetUp.Inventory.frm_CycleCountDetermination));
		this.grid_CylesCount = new Apzon.Controls.Grid.GridControlEx();
		this.gv_CyclesCount = new Apzon.Controls.Grid.GridViewEx();
		this.WhsCode = new Apzon.Controls.Grid.GridColumnEx();
		this.WhsName = new Apzon.Controls.Grid.GridColumnEx();
		this.CycleBy = new Apzon.Controls.Grid.GridColumnEx();
		this.look_CyclesBy = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.BinActivat = new Apzon.Controls.Grid.GridColumnEx();
		this.CycleType = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_Ok = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.grid_CylesCount).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_CyclesCount).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.look_CyclesBy).BeginInit();
		base.SuspendLayout();
		this.grid_CylesCount.AllowPasteExcel = true;
		this.grid_CylesCount.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grid_CylesCount.ClipboardData = null;
		this.grid_CylesCount.Location = new System.Drawing.Point(1, 0);
		this.grid_CylesCount.MainView = this.gv_CyclesCount;
		this.grid_CylesCount.Name = "grid_CylesCount";
		this.grid_CylesCount.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.look_CyclesBy });
		this.grid_CylesCount.Size = new System.Drawing.Size(499, 342);
		this.grid_CylesCount.TabIndex = 0;
		this.grid_CylesCount.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_CyclesCount });
		this.gv_CyclesCount.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_CyclesCount.ColumnKey = null;
		this.gv_CyclesCount.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[5] { this.WhsCode, this.WhsName, this.CycleBy, this.BinActivat, this.CycleType });
		this.gv_CyclesCount.DisableFormSetting = false;
		this.gv_CyclesCount.DisplayException = false;
		this.gv_CyclesCount.EditColumn = null;
		this.gv_CyclesCount.FieldCurrency = null;
		this.gv_CyclesCount.FieldRate = null;
		this.gv_CyclesCount.FormId = "oCycleCountDetermination";
		this.gv_CyclesCount.GridControl = this.grid_CylesCount;
		this.gv_CyclesCount.IgnoreFilter = false;
		this.gv_CyclesCount.IndexEditRow = -1;
		this.gv_CyclesCount.IndicatorWidth = 30;
		this.gv_CyclesCount.IsApzTable = false;
		this.gv_CyclesCount.IsInsertNewRow = false;
		this.gv_CyclesCount.IsLoadingData = false;
		this.gv_CyclesCount.IsMappingContextAction = false;
		this.gv_CyclesCount.ItemId = "OCCS";
		this.gv_CyclesCount.LstColumnAllowBatch = null;
		this.gv_CyclesCount.LstColumnNotAllowBatch = null;
		this.gv_CyclesCount.LstColumnNotEditable = null;
		this.gv_CyclesCount.LstColumNotSetting = null;
		this.gv_CyclesCount.Name = "gv_CyclesCount";
		this.gv_CyclesCount.OptionsView.ShowGroupPanel = false;
		this.gv_CyclesCount.UseBatchMod = false;
		this.gv_CyclesCount.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_CyclesCount_CustomDrawRowIndicator);
		this.gv_CyclesCount.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_CyclesCount_ShowingEditor);
		this.gv_CyclesCount.DoubleClick += new System.EventHandler(this.gv_CyclesCount_DoubleClick);
		this.WhsCode.Caption = "Whse Code";
		this.WhsCode.CurrencyDateField = null;
		this.WhsCode.CurrencyField = null;
		this.WhsCode.CurrencyFixed = null;
		this.WhsCode.CurrencyFixedRate = 0.0;
		this.WhsCode.CurrencyRateField = null;
		this.WhsCode.FieldName = "WhsCode";
		this.WhsCode.Name = "WhsCode";
		this.WhsCode.PrimaryKey = true;
		this.WhsCode.UnitField = null;
		this.WhsCode.Visible = true;
		this.WhsCode.VisibleIndex = 0;
		this.WhsName.Caption = "Whse Name";
		this.WhsName.CurrencyDateField = null;
		this.WhsName.CurrencyField = null;
		this.WhsName.CurrencyFixed = null;
		this.WhsName.CurrencyFixedRate = 0.0;
		this.WhsName.CurrencyRateField = null;
		this.WhsName.FieldName = "WhsName";
		this.WhsName.Name = "WhsName";
		this.WhsName.PrimaryKey = false;
		this.WhsName.UnitField = null;
		this.WhsName.Visible = true;
		this.WhsName.VisibleIndex = 1;
		this.CycleBy.Caption = "Cycle By";
		this.CycleBy.ColumnEdit = this.look_CyclesBy;
		this.CycleBy.CurrencyDateField = null;
		this.CycleBy.CurrencyField = null;
		this.CycleBy.CurrencyFixed = null;
		this.CycleBy.CurrencyFixedRate = 0.0;
		this.CycleBy.CurrencyRateField = null;
		this.CycleBy.FieldName = "CycleBy";
		this.CycleBy.Name = "CycleBy";
		this.CycleBy.PrimaryKey = false;
		this.CycleBy.UnitField = null;
		this.CycleBy.Visible = true;
		this.CycleBy.VisibleIndex = 2;
		this.look_CyclesBy.AutoHeight = false;
		this.look_CyclesBy.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.look_CyclesBy.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.look_CyclesBy.DisplayMember = "Name";
		this.look_CyclesBy.Name = "look_CyclesBy";
		this.look_CyclesBy.NullText = "";
		this.look_CyclesBy.ValueMember = "Code";
		this.look_CyclesBy.EditValueChanged += new System.EventHandler(this.look_CyclesBy_EditValueChanged);
		this.BinActivat.Caption = "Bin Activat";
		this.BinActivat.CurrencyDateField = null;
		this.BinActivat.CurrencyField = null;
		this.BinActivat.CurrencyFixed = null;
		this.BinActivat.CurrencyFixedRate = 0.0;
		this.BinActivat.CurrencyRateField = null;
		this.BinActivat.FieldName = "BinActivat";
		this.BinActivat.Name = "BinActivat";
		this.BinActivat.PrimaryKey = false;
		this.BinActivat.UnitField = null;
		this.CycleType.Caption = "Cycle Type";
		this.CycleType.CurrencyDateField = null;
		this.CycleType.CurrencyField = null;
		this.CycleType.CurrencyFixed = null;
		this.CycleType.CurrencyFixedRate = 0.0;
		this.CycleType.CurrencyRateField = null;
		this.CycleType.FieldName = "CycleType";
		this.CycleType.Name = "CycleType";
		this.CycleType.PrimaryKey = false;
		this.CycleType.UnitField = null;
		this.btn_Ok.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Ok.EditControl = null;
		this.btn_Ok.EditValue = "OK";
		this.btn_Ok.FieldName = null;
		this.btn_Ok.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Ok.ImageOptions.Image");
		this.btn_Ok.IsSearchControl = false;
		this.btn_Ok.Location = new System.Drawing.Point(2, 345);
		this.btn_Ok.Name = "btn_Ok";
		this.btn_Ok.ReadOnly = false;
		this.btn_Ok.SettingInVisible = false;
		this.btn_Ok.SettingReadonly = false;
		this.btn_Ok.Size = new System.Drawing.Size(75, 23);
		this.btn_Ok.TabIndex = 1;
		this.btn_Ok.Text = "OK";
		this.btn_Ok.Click += new System.EventHandler(this.btn_Ok_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(79, 345);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 2;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		base.AcceptButton = this.btn_Ok;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btn_Cancel;
		base.ClientSize = new System.Drawing.Size(502, 371);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_Ok);
		base.Controls.Add(this.grid_CylesCount);
		base.FormId = "oCycleCountDetermination";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_CycleCountDetermination";
		this.Text = "Cycle Count Determination - Setup";
		base.OnLoadAsync += new Apzon.Controls.Form.LoadAsyncEventHandler(this.frm_CycleCountDetermination_Load);
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_CycleCountDetermination_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.grid_CylesCount).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_CyclesCount).EndInit();
		((System.ComponentModel.ISupportInitialize)this.look_CyclesBy).EndInit();
		base.ResumeLayout(false);
	}
}
