using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Properties;
using Apzon.Controls.Controls;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Entities.Models.Common;
using Apzon.Global;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Controls.Form.ExchangeRateAndIndex;

public class frm_SetRateForSelectionCriteria : XtraFormEx
{
	public delegate void OnCloseForm(List<Common.Table_Code> lstCurrency, int from, int to);

	public OnCloseForm ReLoadParent;

	public DateTime _FromDate = DateTime.Today;

	public DateTime _ToDate = DateTime.Today;

	private IContainer components;

	private LabelLine labelLine1;

	private LabelLine labelLine2;

	private DateEdit dateFrom;

	private DateEdit dateTo;

	private GridControlEx grdRates;

	private GridColumn gcCurrency;

	private GridColumn gcRate;

	private SimpleButton btCancel;

	private SimpleButton btOK;

	private GridViewEx gvRates;

	public frm_SetRateForSelectionCriteria()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_ExchangeRatesAndIndexs"))
		{
			InitializeComponent();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_ExchangeRatesAndIndexs" && x.authoriza == "F"))
			{
				btCancel.Enabled = true;
			}
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	private void LoadData()
	{
		try
		{
			List<CurrencyCodesImpl> list = new List<CurrencyCodesImpl>();
			foreach (CurrencyCodesImpl currencyCode in GlobalData.CurrencyCodes)
			{
				CurrencyCodesImpl currencyCodesImpl = new CurrencyCodesImpl();
				currencyCodesImpl.CurrCode = currencyCode.CurrCode;
				currencyCodesImpl.Rate = 0m;
				list.Add(currencyCodesImpl);
			}
			grdRates.DataSource = list;
			grdRates.RefreshDataSource();
			dateFrom.DateTime = _FromDate;
			dateTo.DateTime = _ToDate;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void frm_SetRateForSelectionCriteria_Load(object sender, EventArgs e)
	{
		try
		{
			dateFrom.Properties.Mask.MaskType = MaskType.DateTime;
			dateFrom.Properties.Mask.EditMask = GlobalData.DateTimeFormat;
			dateFrom.Properties.Mask.UseMaskAsDisplayFormat = true;
			dateTo.Properties.Mask.MaskType = MaskType.DateTime;
			dateTo.Properties.Mask.EditMask = GlobalData.DateTimeFormat;
			dateTo.Properties.Mask.UseMaskAsDisplayFormat = true;
			LoadData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btOK_Click(object sender, EventArgs e)
	{
		try
		{
			List<Common.Table_Code> list = new List<Common.Table_Code>();
			int[] selectedRows = gvRates.GetSelectedRows();
			for (int i = 0; i < selectedRows.Length; i++)
			{
				Common.Table_Code table_Code = new Common.Table_Code();
				table_Code.Code = gvRates.GetRowCellValue(selectedRows[i], gcCurrency).ToString();
				table_Code.Name = gvRates.GetRowCellValue(selectedRows[i], gcRate).ToString();
				list.Add(table_Code);
			}
			int day = dateFrom.DateTime.Day;
			int day2 = dateTo.DateTime.Day;
			ReLoadParent(list, day, day2);
			Close();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btCancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void dateFrom_Leave(object sender, EventArgs e)
	{
		try
		{
			CheckValidTime(dateFrom.EditValue, ref dateFrom);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void dateTo_Leave(object sender, EventArgs e)
	{
		try
		{
			CheckValidTime(dateTo.EditValue, ref dateTo);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void CheckValidTime(object value, ref DateEdit DateControl)
	{
		try
		{
			DateTime dateTime = dateFrom.DateTime;
			DateTime dateTime2 = dateTo.DateTime;
			if (dateTime > dateTime2 || dateTime < _FromDate || dateTime2 > _ToDate || dateTime > _ToDate || dateTime2 < _FromDate)
			{
				WinCommons.Error(Enums.MessageCode.DateBeweent, "", _FromDate.Date.ToShortDateString(), _ToDate.Date.ToShortDateString());
				DateControl.Focus();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Controls.Form.ExchangeRateAndIndex.frm_SetRateForSelectionCriteria));
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.dateFrom = new DevExpress.XtraEditors.DateEdit();
		this.dateTo = new DevExpress.XtraEditors.DateEdit();
		this.grdRates = new Apzon.Controls.Grid.GridControlEx();
		this.gvRates = new Apzon.Controls.Grid.GridViewEx();
		this.gcCurrency = new DevExpress.XtraGrid.Columns.GridColumn();
		this.gcRate = new DevExpress.XtraGrid.Columns.GridColumn();
		this.btCancel = new DevExpress.XtraEditors.SimpleButton();
		this.btOK = new DevExpress.XtraEditors.SimpleButton();
		((System.ComponentModel.ISupportInitialize)this.dateFrom.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dateFrom.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dateTo.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dateTo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grdRates).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gvRates).BeginInit();
		base.SuspendLayout();
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(4, 12);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(76, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 0;
		this.labelLine1.Utext = "Date From";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(183, 12);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(67, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 1;
		this.labelLine2.Utext = "To";
		this.dateFrom.EditValue = null;
		this.dateFrom.Location = new System.Drawing.Point(77, 9);
		this.dateFrom.Name = "dateFrom";
		this.dateFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dateFrom.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dateFrom.Size = new System.Drawing.Size(100, 20);
		this.dateFrom.TabIndex = 2;
		this.dateFrom.Leave += new System.EventHandler(this.dateFrom_Leave);
		this.dateTo.EditValue = null;
		this.dateTo.Location = new System.Drawing.Point(249, 9);
		this.dateTo.Name = "dateTo";
		this.dateTo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dateTo.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dateTo.Size = new System.Drawing.Size(100, 20);
		this.dateTo.TabIndex = 3;
		this.dateTo.Leave += new System.EventHandler(this.dateTo_Leave);
		this.grdRates.AllowPasteExcel = false;
		this.grdRates.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grdRates.ClipboardData = null;
		this.grdRates.Location = new System.Drawing.Point(4, 38);
		this.grdRates.MainView = this.gvRates;
		this.grdRates.Name = "grdRates";
		this.grdRates.Size = new System.Drawing.Size(345, 287);
		this.grdRates.TabIndex = 4;
		this.grdRates.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvRates });
		this.gvRates.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvRates.ColumnKey = null;
		this.gvRates.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[2] { this.gcCurrency, this.gcRate });
		this.gvRates.DisableFormSetting = false;
		this.gvRates.DisplayException = false;
		this.gvRates.EditColumn = null;
		this.gvRates.FieldCurrency = null;
		this.gvRates.FieldRate = null;
		this.gvRates.FormId = "oSetRateForSelectionCriteria";
		this.gvRates.GridControl = this.grdRates;
		this.gvRates.IndexEditRow = -1;
		this.gvRates.IndicatorWidth = 35;
		this.gvRates.IsApzTable = false;
		this.gvRates.IsInsertNewRow = false;
		this.gvRates.IsLoadingData = false;
		this.gvRates.ItemId = "ODATA";
		this.gvRates.LstColumnAllowBatch = null;
		this.gvRates.LstColumnNotAllowBatch = null;
		this.gvRates.LstColumNotSetting = null;
		this.gvRates.Name = "gvRates";
		this.gvRates.OptionsSelection.MultiSelect = true;
		this.gvRates.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
		this.gvRates.OptionsView.ShowDetailButtons = false;
		this.gvRates.OptionsView.ShowGroupPanel = false;
		this.gvRates.UseBatchMod = false;
		this.gcCurrency.Caption = "Currency";
		this.gcCurrency.FieldName = "CurrCode";
		this.gcCurrency.Name = "gcCurrency";
		this.gcCurrency.OptionsColumn.AllowEdit = false;
		this.gcCurrency.OptionsColumn.ReadOnly = true;
		this.gcCurrency.Visible = true;
		this.gcCurrency.VisibleIndex = 1;
		this.gcRate.Caption = "Rate";
		this.gcRate.FieldName = "Rate";
		this.gcRate.Name = "gcRate";
		this.gcRate.Visible = true;
		this.gcRate.VisibleIndex = 2;
		this.btCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btCancel.ImageOptions.Image");
		this.btCancel.Location = new System.Drawing.Point(85, 331);
		this.btCancel.Name = "btCancel";
		this.btCancel.Size = new System.Drawing.Size(75, 23);
		this.btCancel.TabIndex = 6;
		this.btCancel.Text = "Cancel";
		this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
		this.btOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btOK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btOK.ImageOptions.Image");
		this.btOK.Location = new System.Drawing.Point(4, 331);
		this.btOK.Name = "btOK";
		this.btOK.Size = new System.Drawing.Size(75, 23);
		this.btOK.TabIndex = 5;
		this.btOK.Text = "OK";
		this.btOK.Click += new System.EventHandler(this.btOK_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(352, 361);
		base.Controls.Add(this.btCancel);
		base.Controls.Add(this.btOK);
		base.Controls.Add(this.grdRates);
		base.Controls.Add(this.dateTo);
		base.Controls.Add(this.dateFrom);
		base.Controls.Add(this.labelLine2);
		base.Controls.Add(this.labelLine1);
		base.FormId = "oSetRateForSelectionCriteria";
		base.Name = "frm_SetRateForSelectionCriteria";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Set Rate For Selection Criteria";
		base.Load += new System.EventHandler(this.frm_SetRateForSelectionCriteria_Load);
		((System.ComponentModel.ISupportInitialize)this.dateFrom.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dateFrom.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dateTo.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dateTo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grdRates).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gvRates).EndInit();
		base.ResumeLayout(false);
	}
}
