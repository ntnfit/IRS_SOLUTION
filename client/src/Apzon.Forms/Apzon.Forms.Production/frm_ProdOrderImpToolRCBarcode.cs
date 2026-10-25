using System;
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
using Apzon.Forms.Commons;
using Apzon.HttpClientHelper;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.Production;

public class frm_ProdOrderImpToolRCBarcode : XtraFormEx
{
	private IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private bool _isLoading;

	private IContainer components;

	private TextEditEx txt_Barcode;

	private LabelLine lb_Name;

	private SimpleButtonEx btn_Save;

	private GridControlEx grdEx_Doc;

	private GridViewEx gvEx_Doc;

	private SimpleButtonEx btn_Close;

	private GridColumnEx gridColumnEx6;

	private GridColumnEx gridColumnEx1;

	private GridColumnEx gridColumnEx2;

	private GridColumnEx gridColumnEx3;

	private GridColumnEx gridColumnEx8;

	private GridColumnEx gridColumnEx7;

	private GridColumnEx gridColumnEx4;

	private GridColumnEx gridColumnEx5;

	private GridColumnEx gridColumnEx9;

	private GridColumnEx gridColumnEx10;

	public bool isSaved { get; set; }

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
		{
			WinFuntions.DisableAllControl(Controls);
			btn_Close.Enabled = true;
			return;
		}
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Update:
			btn_Save.Text = Resources.ButtonSave;
			break;
		case Enums.FormViewModeType.View:
			btn_Save.Text = Resources.ButtonOK;
			break;
		}
	}

	public frm_ProdOrderImpToolRCBarcode()
	{
		InitializeComponent();
	}

	private void frm_ProdOrderImpToolRCBarcode_Load(object sender, EventArgs e)
	{
		try
		{
			FormId = "PrdOrderImportTool";
			gvEx_Doc.ItemId = "POIT_RCBarcode";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btn_Save_Click(object sender, EventArgs e)
	{
		EnableDisableControls(isEnable: false);
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
			}
			else
			{
				WinCommons._OpenProcessing();
				await Save();
				WinCommons._CloseProcessing();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		EnableDisableControls(isEnable: true);
	}

	private void btn_Close_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void txt_Barcode_KeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode != Keys.Return)
			{
				return;
			}
			string barCode = txt_Barcode.Text;
			DataTable dataTable = grdEx_Doc.DataSource as DataTable;
			string value = "";
			if (!dataTable.IsNotNull())
			{
				dataTable = _httpClient.ProductionOrderImportTool.RCGetProductionDetails("-1");
				DataTable dataTable2 = _httpClient.ProductionOrderImportTool.RCGetProductionDetails("-2");
				if (dataTable2.IsNotNull())
				{
					value = Function.ToString(dataTable2.Rows[0][0]);
				}
			}
			DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow r) => Function.ToString(r["Barcode"]) == barCode);
			if (dataRow != null)
			{
				dataRow["CmpQuantiy"] = Function.ParseInt(dataRow["CmpQuantiy"]) + 1;
				FormMode = Enums.FormViewModeType.Update;
			}
			else
			{
				DataTable dataTable3 = _httpClient.ProductionOrderImportTool.RCGetProductionDetails(barCode);
				if (dataTable3.IsNotNull())
				{
					dataRow = dataTable.NewRow();
					dataRow["Barcode"] = barCode;
					dataRow["U_PlnCode"] = Function.ToString(dataTable3.Rows[0]["U_PlnCode"]);
					dataRow["U_LotNo"] = Function.ToString(dataTable3.Rows[0]["U_LotNo"]);
					dataRow["DocEntry"] = Function.ParseInt(dataTable3.Rows[0]["DocEntry"]);
					dataRow["ItemCode"] = Function.ToString(dataTable3.Rows[0]["ItemCode"]);
					dataRow["ItemName"] = Function.ToString(dataTable3.Rows[0]["ItemName"]);
					dataRow["CmpQuantiy"] = 1;
					dataRow["AcctCode"] = value;
					dataRow["PostingDate"] = DateTime.Today;
					dataRow["DocNum"] = Function.ParseInt(dataTable3.Rows[0]["DocNum"]);
					dataRow["Warehouse"] = Function.ToString(dataTable3.Rows[0]["Warehouse"]);
					dataTable.Rows.Add(dataRow);
					FormMode = Enums.FormViewModeType.Update;
				}
			}
			dataTable.AcceptChanges();
			grdEx_Doc.DataSource = dataTable;
			grdEx_Doc.RefreshDataSource();
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
			DataTable dataTable = (grdEx_Doc.DataSource as DataTable).Copy();
			if (dataTable.IsNotNull())
			{
				string text = await _httpClient.ProductionOrderImportTool.SaveRCFromPC(dataTable);
				if (string.IsNullOrEmpty(text))
				{
					WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
					FormMode = Enums.FormViewModeType.View;
					grdEx_Doc.DataSource = null;
					grdEx_Doc.RefreshDataSource();
					isSaved = true;
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, text);
					btn_Save.Enabled = true;
					_isLoading = false;
				}
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.NoDataToSave, "");
				btn_Save.Enabled = true;
				_isLoading = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void EnableDisableControls(bool isEnable)
	{
		try
		{
			btn_Save.Enabled = isEnable;
			btn_Close.Enabled = isEnable;
			txt_Barcode.Enabled = isEnable;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Production.frm_ProdOrderImpToolRCBarcode));
		this.txt_Barcode = new Apzon.Controls.Editor.TextEditEx();
		this.lb_Name = new Apzon.Controls.Controls.LabelLine();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grdEx_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.gvEx_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx6 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx8 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx7 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx10 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx5 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx9 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_Close = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.txt_Barcode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grdEx_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gvEx_Doc).BeginInit();
		base.SuspendLayout();
		this.txt_Barcode.EditControl = null;
		this.txt_Barcode.FieldName = null;
		this.txt_Barcode.IsSearchControl = false;
		this.txt_Barcode.Location = new System.Drawing.Point(103, 8);
		this.txt_Barcode.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.txt_Barcode.Name = "txt_Barcode";
		this.txt_Barcode.SettingInVisible = false;
		this.txt_Barcode.SettingReadonly = false;
		this.txt_Barcode.Size = new System.Drawing.Size(170, 20);
		this.txt_Barcode.TabIndex = 2;
		this.txt_Barcode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_Barcode_KeyDown);
		this.lb_Name.EditControl = null;
		this.lb_Name.EditValue = "Barcode";
		this.lb_Name.FieldName = null;
		this.lb_Name.HideWhenEmpty = false;
		this.lb_Name.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Name.Location = new System.Drawing.Point(15, 11);
		this.lb_Name.Margin = new System.Windows.Forms.Padding(4);
		this.lb_Name.Name = "lb_Name";
		this.lb_Name.ReadOnly = false;
		this.lb_Name.SettingInVisible = false;
		this.lb_Name.SettingReadonly = false;
		this.lb_Name.ShowUnderLine = true;
		this.lb_Name.Size = new System.Drawing.Size(99, 20);
		this.lb_Name.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Name.TabIndex = 3;
		this.lb_Name.Utext = "Barcode";
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.EditControl = null;
		this.btn_Save.EditValue = "OK";
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(15, 352);
		this.btn_Save.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.ReadOnly = false;
		this.btn_Save.SettingInVisible = false;
		this.btn_Save.SettingReadonly = false;
		this.btn_Save.Size = new System.Drawing.Size(75, 24);
		this.btn_Save.TabIndex = 12;
		this.btn_Save.Text = "OK";
		this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
		this.grdEx_Doc.AllowPasteExcel = true;
		this.grdEx_Doc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grdEx_Doc.ClipboardData = null;
		this.grdEx_Doc.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grdEx_Doc.Location = new System.Drawing.Point(15, 35);
		this.grdEx_Doc.MainView = this.gvEx_Doc;
		this.grdEx_Doc.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grdEx_Doc.Name = "grdEx_Doc";
		this.grdEx_Doc.Size = new System.Drawing.Size(867, 309);
		this.grdEx_Doc.TabIndex = 13;
		this.grdEx_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvEx_Doc });
		this.gvEx_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvEx_Doc.ColumnKey = null;
		this.gvEx_Doc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[10] { this.gridColumnEx6, this.gridColumnEx1, this.gridColumnEx2, this.gridColumnEx3, this.gridColumnEx8, this.gridColumnEx7, this.gridColumnEx4, this.gridColumnEx10, this.gridColumnEx5, this.gridColumnEx9 });
		this.gvEx_Doc.DisableFormSetting = false;
		this.gvEx_Doc.DisplayException = false;
		this.gvEx_Doc.EditColumn = null;
		this.gvEx_Doc.FieldCurrency = null;
		this.gvEx_Doc.FieldRate = null;
		this.gvEx_Doc.FormId = "oProdOrderImpToolRCBarcode";
		this.gvEx_Doc.GridControl = this.grdEx_Doc;
		this.gvEx_Doc.IgnoreFilter = false;
		this.gvEx_Doc.IndexEditRow = -1;
		this.gvEx_Doc.IndicatorWidth = 70;
		this.gvEx_Doc.IsApzTable = false;
		this.gvEx_Doc.IsInsertNewRow = false;
		this.gvEx_Doc.IsLoadingData = false;
		this.gvEx_Doc.IsMappingContextAction = false;
		this.gvEx_Doc.ItemId = "POIT_RCBarcode";
		this.gvEx_Doc.LstColumnAllowBatch = null;
		this.gvEx_Doc.LstColumnNotAllowBatch = null;
		this.gvEx_Doc.LstColumnNotEditable = null;
		this.gvEx_Doc.LstColumNotSetting = null;
		this.gvEx_Doc.Name = "gvEx_Doc";
		this.gvEx_Doc.OptionsView.ColumnAutoWidth = false;
		this.gvEx_Doc.OptionsView.ShowGroupPanel = false;
		this.gvEx_Doc.UseBatchMod = false;
		this.gridColumnEx6.Caption = "Barcode";
		this.gridColumnEx6.CurrencyDateField = null;
		this.gridColumnEx6.CurrencyField = null;
		this.gridColumnEx6.CurrencyFixed = null;
		this.gridColumnEx6.CurrencyFixedRate = 0.0;
		this.gridColumnEx6.CurrencyRateField = null;
		this.gridColumnEx6.FieldName = "Barcode";
		this.gridColumnEx6.Name = "gridColumnEx6";
		this.gridColumnEx6.PrimaryKey = true;
		this.gridColumnEx6.UnitField = null;
		this.gridColumnEx6.Visible = true;
		this.gridColumnEx6.VisibleIndex = 7;
		this.gridColumnEx1.Caption = "Week No.";
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "U_PlnCode";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.PrimaryKey = false;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx1.Visible = true;
		this.gridColumnEx1.VisibleIndex = 0;
		this.gridColumnEx2.Caption = "Lot No";
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.FieldName = "U_LotNo";
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.UnitField = null;
		this.gridColumnEx2.Visible = true;
		this.gridColumnEx2.VisibleIndex = 1;
		this.gridColumnEx3.Caption = "Production Order ID";
		this.gridColumnEx3.CurrencyDateField = null;
		this.gridColumnEx3.CurrencyField = null;
		this.gridColumnEx3.CurrencyFixed = null;
		this.gridColumnEx3.CurrencyFixedRate = 0.0;
		this.gridColumnEx3.CurrencyRateField = null;
		this.gridColumnEx3.FieldName = "DocEntry";
		this.gridColumnEx3.Name = "gridColumnEx3";
		this.gridColumnEx3.PrimaryKey = false;
		this.gridColumnEx3.UnitField = null;
		this.gridColumnEx3.Visible = true;
		this.gridColumnEx3.VisibleIndex = 2;
		this.gridColumnEx3.Width = 125;
		this.gridColumnEx8.Caption = "Product Code";
		this.gridColumnEx8.CurrencyDateField = null;
		this.gridColumnEx8.CurrencyField = null;
		this.gridColumnEx8.CurrencyFixed = null;
		this.gridColumnEx8.CurrencyFixedRate = 0.0;
		this.gridColumnEx8.CurrencyRateField = null;
		this.gridColumnEx8.FieldName = "ItemCode";
		this.gridColumnEx8.Name = "gridColumnEx8";
		this.gridColumnEx8.PrimaryKey = false;
		this.gridColumnEx8.UnitField = null;
		this.gridColumnEx8.Visible = true;
		this.gridColumnEx8.VisibleIndex = 3;
		this.gridColumnEx8.Width = 88;
		this.gridColumnEx7.Caption = "Product Name";
		this.gridColumnEx7.CurrencyDateField = null;
		this.gridColumnEx7.CurrencyField = null;
		this.gridColumnEx7.CurrencyFixed = null;
		this.gridColumnEx7.CurrencyFixedRate = 0.0;
		this.gridColumnEx7.CurrencyRateField = null;
		this.gridColumnEx7.FieldName = "ItemName";
		this.gridColumnEx7.Name = "gridColumnEx7";
		this.gridColumnEx7.PrimaryKey = false;
		this.gridColumnEx7.UnitField = null;
		this.gridColumnEx7.Visible = true;
		this.gridColumnEx7.VisibleIndex = 4;
		this.gridColumnEx7.Width = 92;
		this.gridColumnEx4.Caption = "Complete Qty.";
		this.gridColumnEx4.CurrencyDateField = null;
		this.gridColumnEx4.CurrencyField = null;
		this.gridColumnEx4.CurrencyFixed = null;
		this.gridColumnEx4.CurrencyFixedRate = 0.0;
		this.gridColumnEx4.CurrencyRateField = null;
		this.gridColumnEx4.FieldName = "CmpQuantiy";
		this.gridColumnEx4.Name = "gridColumnEx4";
		this.gridColumnEx4.PrimaryKey = false;
		this.gridColumnEx4.UnitField = null;
		this.gridColumnEx4.Visible = true;
		this.gridColumnEx4.VisibleIndex = 5;
		this.gridColumnEx10.Caption = "Reject Qty.";
		this.gridColumnEx10.CurrencyDateField = null;
		this.gridColumnEx10.CurrencyField = null;
		this.gridColumnEx10.CurrencyFixed = null;
		this.gridColumnEx10.CurrencyFixedRate = 0.0;
		this.gridColumnEx10.CurrencyRateField = null;
		this.gridColumnEx10.FieldName = "RjQuantity";
		this.gridColumnEx10.Name = "gridColumnEx10";
		this.gridColumnEx10.PrimaryKey = false;
		this.gridColumnEx10.UnitField = null;
		this.gridColumnEx10.Visible = true;
		this.gridColumnEx10.VisibleIndex = 9;
		this.gridColumnEx5.Caption = "Date";
		this.gridColumnEx5.CurrencyDateField = null;
		this.gridColumnEx5.CurrencyField = null;
		this.gridColumnEx5.CurrencyFixed = null;
		this.gridColumnEx5.CurrencyFixedRate = 0.0;
		this.gridColumnEx5.CurrencyRateField = null;
		this.gridColumnEx5.FieldName = "PostingDate";
		this.gridColumnEx5.Name = "gridColumnEx5";
		this.gridColumnEx5.PrimaryKey = false;
		this.gridColumnEx5.UnitField = null;
		this.gridColumnEx5.Visible = true;
		this.gridColumnEx5.VisibleIndex = 6;
		this.gridColumnEx9.Caption = "Warehouse";
		this.gridColumnEx9.CurrencyDateField = null;
		this.gridColumnEx9.CurrencyField = null;
		this.gridColumnEx9.CurrencyFixed = null;
		this.gridColumnEx9.CurrencyFixedRate = 0.0;
		this.gridColumnEx9.CurrencyRateField = null;
		this.gridColumnEx9.FieldName = "Warehouse";
		this.gridColumnEx9.Name = "gridColumnEx9";
		this.gridColumnEx9.PrimaryKey = false;
		this.gridColumnEx9.UnitField = null;
		this.gridColumnEx9.Visible = true;
		this.gridColumnEx9.VisibleIndex = 8;
		this.btn_Close.EditControl = null;
		this.btn_Close.EditValue = "Close";
		this.btn_Close.FieldName = null;
		this.btn_Close.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Close.ImageOptions.Image");
		this.btn_Close.IsSearchControl = false;
		this.btn_Close.Location = new System.Drawing.Point(93, 352);
		this.btn_Close.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.btn_Close.Name = "btn_Close";
		this.btn_Close.ReadOnly = false;
		this.btn_Close.SettingInVisible = false;
		this.btn_Close.SettingReadonly = false;
		this.btn_Close.Size = new System.Drawing.Size(75, 24);
		this.btn_Close.TabIndex = 14;
		this.btn_Close.Text = "Close";
		this.btn_Close.Click += new System.EventHandler(this.btn_Close_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(902, 384);
		base.Controls.Add(this.btn_Close);
		base.Controls.Add(this.grdEx_Doc);
		base.Controls.Add(this.btn_Save);
		base.Controls.Add(this.txt_Barcode);
		base.Controls.Add(this.lb_Name);
		base.FormId = "oProdOrderImpToolRCBarcode";
		base.IconOptions.ShowIcon = false;
		base.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		base.Name = "frm_ProdOrderImpToolRCBarcode";
		this.Text = "Production Order Import Tool Report Complete Barcode";
		base.Load += new System.EventHandler(this.frm_ProdOrderImpToolRCBarcode_Load);
		((System.ComponentModel.ISupportInitialize)this.txt_Barcode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grdEx_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gvEx_Doc).EndInit();
		base.ResumeLayout(false);
	}
}
