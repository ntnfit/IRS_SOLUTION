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
using Apzon.Commons.Constants;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Properties;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Administrator.SystemInitialization;

public class frm_DocumentNumbering : XtraFormEx
{
	private const string Controller = "DocumentNumbering";

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private IContainer components;

	private GridControlEx grd_Doc;

	private GridViewEx gv_Doc;

	private GridColumnEx gridColumn1;

	private GridColumnEx gridColumn2;

	private GridColumnEx gridColumn3;

	private GridColumnEx gridColumn4;

	private GridColumnEx gridColumn5;

	private GridColumnEx gridColumn6;

	private SimpleButtonEx btn_OK;

	private SimpleButtonEx btn_Cancel;

	private RepositoryItemLookUpEdit rep_ObCode;

	private GridColumnEx gridColumn7;

	private RepositoryItemSpinEdit rep_Enum;

	private GridColumnEx gridColumn8;

	private GridColumnEx gridColumn9;

	public DataTable DataSeries { get; set; }

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		try
		{
			_formMode = value;
			switch (value)
			{
			case Enums.FormViewModeType.Add:
			{
				SimpleButtonEx simpleButtonEx4 = btn_OK;
				bool enabled = (btn_Cancel.Enabled = true);
				simpleButtonEx4.Enabled = enabled;
				btn_OK.Text = Apzon.Commons.Properties.Resources.ButtonAdd;
				break;
			}
			case Enums.FormViewModeType.Update:
			{
				SimpleButtonEx simpleButtonEx3 = btn_OK;
				bool enabled = (btn_Cancel.Enabled = true);
				simpleButtonEx3.Enabled = enabled;
				btn_OK.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
				break;
			}
			case Enums.FormViewModeType.View:
			{
				SimpleButtonEx simpleButtonEx2 = btn_OK;
				bool enabled = (btn_Cancel.Enabled = true);
				simpleButtonEx2.Enabled = enabled;
				btn_OK.Text = Apzon.Commons.Properties.Resources.ButtonOK;
				break;
			}
			case Enums.FormViewModeType.Busy:
			{
				SimpleButtonEx simpleButtonEx = btn_OK;
				bool enabled = (btn_Cancel.Enabled = false);
				simpleButtonEx.Enabled = enabled;
				break;
			}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public frm_DocumentNumbering()
	{
		InitializeComponent();
	}

	private async void kethua_Load(object sender, EventArgs e)
	{
		try
		{
			LoadInItForm();
			await LoadData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void FreshLoad()
	{
		await LoadData();
		btn_OK.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
	}

	private async Task LoadData()
	{
		_ = 1;
		try
		{
			btn_OK.Text = Apzon.Commons.Properties.Resources.ButtonOK;
			using (HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "DocumentNumbering"))
			{
				DataTable dataSource = await helper.GetAsync("LoadDoc", new
				{
					userId = GlobalData.UserID
				});
				grd_Doc.DataSource = dataSource;
				grd_Doc.RefreshDataSource();
			}
			DataSeries = await _httpClient.DocumentNumering.GetDataSeries(GlobalData.UserID);
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void LoadInItForm()
	{
		try
		{
			List<Common.Table_Code> dataSource = CommonList.oDocumentNumberingCode.Select((KeyValuePair<string, string> item) => new Common.Table_Code
			{
				Code = item.Key,
				Name = item.Value
			}).ToList();
			rep_ObCode.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
	{
	}

	private void simpleButton2_Click(object sender, EventArgs e)
	{
		try
		{
			Close();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		btn_OK.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
	}

	private async void btn_OK_Click(object sender, EventArgs e)
	{
		Enums.FormViewModeType currentMode = FormMode;
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
				return;
			}
			if (!DataSeries.IsNotNull())
			{
				Close();
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "DataSeries: No data");
				return;
			}
			FormMode = Enums.FormViewModeType.Busy;
			WinCommons._OpenProcessing();
			using (new HttpClientHelper<string>(Configuration.ApiServer, "DocumentNumbering"))
			{
				DataSet dataSet = new DataSet();
				DataTable dataSourceTable = grd_Doc.DataSourceTable;
				dataSourceTable.TableName = "ONNM";
				dataSet.Tables.Add(dataSourceTable.Copy());
				DataSeries.TableName = "NNM1";
				dataSet.Tables.Add(DataSeries.Copy());
				HttpResult httpResult = await _httpClient.DocumentNumering.Save(dataSet);
				if (string.IsNullOrEmpty(httpResult.Message))
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
					await LoadData();
					WinCommons._CloseProcessing();
					return;
				}
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		FormMode = currentMode;
		WinCommons._CloseProcessing();
	}

	private void gv_Doc_DoubleClick(object sender, EventArgs e)
	{
		try
		{
			string obCode = Function.ToString(gv_Doc.GetFocusedRowCellValue("ObjectCode"));
			string docType = Function.ToString(gv_Doc.GetFocusedRowCellValue("DocSubType"));
			string seriesType = Function.ToString(gv_Doc.GetFocusedRowCellValue("SeriesType"));
			frm_Series frm_Series2 = new frm_Series(obCode, docType, seriesType);
			frm_Series2.Text = string.Format("Series - {0} - Setup", rep_ObCode.GetDisplayValueByKeyValue(gv_Doc.GetFocusedRowCellValue("Code")));
			frm_Series2.SetDefaultSeries = (Action<int>)Delegate.Combine(frm_Series2.SetDefaultSeries, (Action<int>)((int i) =>
			{
				SetDefaultSeries(i, obCode, docType);
			}));
			DataRow[] array = (from t in DataSeries.AsEnumerable()
				where Function.ToString(t["ObjectCode"]) == obCode && Function.ToString(t["DocSubType"]) == docType
				select t).ToArray();
			if (array.Length != 0)
			{
				frm_Series2.DataSeries = array.CopyToDataTable();
			}
			if (frm_Series2.ShowDialog(this) != DialogResult.Yes)
			{
				return;
			}
			DataTable dataSeries = frm_Series2.DataSeries;
			if (DataSeries.IsNotNull())
			{
				DataRow[] array2 = (from t in DataSeries.AsEnumerable()
					where Function.ToString(t["ObjectCode"]) == obCode && Function.ToString(t["DocSubType"]) == docType
					select t).ToArray();
				if (array2.Length != 0)
				{
					for (int num = 0; num < array2.Length; num++)
					{
						DataSeries.Rows.Remove(array2[num]);
					}
				}
				for (int num2 = 0; num2 < dataSeries.Rows.Count; num2++)
				{
					DataRow dataRow = DataSeries.NewRow();
					dataRow.ItemArray = dataSeries.Rows[num2].ItemArray;
					DataSeries.Rows.Add(dataRow);
				}
				DataSeries.AcceptChanges();
			}
			else
			{
				DataSeries = dataSeries.Copy();
			}
			FormMode = Enums.FormViewModeType.Update;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void SetDefaultSeries(int series, string objcode, string docsubtype)
	{
		try
		{
			DataTable dataSourceTable = grd_Doc.DataSourceTable;
			DataRow dataRow = dataSourceTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["ObjectCode"]) == objcode && Function.ToString(t["DocSubType"]) == docsubtype);
			if (dataRow != null)
			{
				dataRow["Series"] = series;
				dataSourceTable.AcceptChanges();
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (e.Column.OptionsColumn.ReadOnly)
			{
				e.Appearance.BackColor = GridColor.GridReadOnly;
			}
			if (Function.ToString(gv_Doc.GetRowCellValue(e.RowHandle, "SeriesType")) != "D")
			{
				e.Appearance.BackColor = GridColor.GridReadOnly;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Administrator.SystemInitialization.frm_DocumentNumbering));
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn9 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn7 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_ObCode = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumn2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn3 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Enum = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
		this.gridColumn4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn5 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn6 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn8 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ObCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Enum).BeginInit();
		base.SuspendLayout();
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.Location = new System.Drawing.Point(0, 0);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.rep_ObCode, this.rep_Enum });
		this.grd_Doc.Size = new System.Drawing.Size(946, 482);
		this.grd_Doc.TabIndex = 0;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = null;
		this.gv_Doc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[9] { this.gridColumn9, this.gridColumn7, this.gridColumn1, this.gridColumn2, this.gridColumn3, this.gridColumn4, this.gridColumn5, this.gridColumn6, this.gridColumn8 });
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = "oDocumentNumbering";
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.IgnoreFilter = false;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 35;
		this.gv_Doc.IsApzTable = false;
		this.gv_Doc.IsInsertNewRow = false;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.IsMappingContextAction = false;
		this.gv_Doc.ItemId = "NNM1";
		this.gv_Doc.LstColumnAllowBatch = null;
		this.gv_Doc.LstColumnNotAllowBatch = null;
		this.gv_Doc.LstColumnNotEditable = null;
		this.gv_Doc.LstColumNotSetting = null;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsView.ShowGroupPanel = false;
		this.gv_Doc.UseBatchMod = false;
		this.gv_Doc.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Doc_RowCellStyle);
		this.gv_Doc.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gv_Doc_CustomRowCellEdit);
		this.gv_Doc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Doc_CellValueChanged);
		this.gv_Doc.DoubleClick += new System.EventHandler(this.gv_Doc_DoubleClick);
		this.gridColumn9.Caption = "Series";
		this.gridColumn9.CurrencyDateField = null;
		this.gridColumn9.CurrencyField = null;
		this.gridColumn9.CurrencyFixed = null;
		this.gridColumn9.CurrencyFixedRate = 0.0;
		this.gridColumn9.CurrencyRateField = null;
		this.gridColumn9.FieldName = "Series";
		this.gridColumn9.Name = "gridColumn9";
		this.gridColumn9.OptionsColumn.ReadOnly = true;
		this.gridColumn9.PrimaryKey = true;
		this.gridColumn9.UnitField = null;
		this.gridColumn7.Caption = "DocSubType";
		this.gridColumn7.CurrencyDateField = null;
		this.gridColumn7.CurrencyField = null;
		this.gridColumn7.CurrencyFixed = null;
		this.gridColumn7.CurrencyFixedRate = 0.0;
		this.gridColumn7.CurrencyRateField = null;
		this.gridColumn7.FieldName = "DocSubType";
		this.gridColumn7.Name = "gridColumn7";
		this.gridColumn7.OptionsColumn.ReadOnly = true;
		this.gridColumn7.PrimaryKey = false;
		this.gridColumn7.UnitField = null;
		this.gridColumn1.Caption = "Document";
		this.gridColumn1.ColumnEdit = this.rep_ObCode;
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "Code";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.OptionsColumn.ReadOnly = true;
		this.gridColumn1.PrimaryKey = false;
		this.gridColumn1.UnitField = null;
		this.gridColumn1.Visible = true;
		this.gridColumn1.VisibleIndex = 0;
		this.gridColumn1.Width = 238;
		this.rep_ObCode.AutoHeight = false;
		this.rep_ObCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_ObCode.DisplayMember = "Name";
		this.rep_ObCode.Name = "rep_ObCode";
		this.rep_ObCode.NullText = "";
		this.rep_ObCode.ValueMember = "Code";
		this.gridColumn2.Caption = "Default Series";
		this.gridColumn2.CurrencyDateField = null;
		this.gridColumn2.CurrencyField = null;
		this.gridColumn2.CurrencyFixed = null;
		this.gridColumn2.CurrencyFixedRate = 0.0;
		this.gridColumn2.CurrencyRateField = null;
		this.gridColumn2.FieldName = "SeriesName";
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.PrimaryKey = false;
		this.gridColumn2.UnitField = null;
		this.gridColumn2.Visible = true;
		this.gridColumn2.VisibleIndex = 1;
		this.gridColumn2.Width = 164;
		this.gridColumn3.Caption = "First No.";
		this.gridColumn3.ColumnEdit = this.rep_Enum;
		this.gridColumn3.CurrencyDateField = null;
		this.gridColumn3.CurrencyField = null;
		this.gridColumn3.CurrencyFixed = null;
		this.gridColumn3.CurrencyFixedRate = 0.0;
		this.gridColumn3.CurrencyRateField = null;
		this.gridColumn3.DisplayFormat.FormatString = "n0";
		this.gridColumn3.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gridColumn3.FieldName = "InitialNum";
		this.gridColumn3.Name = "gridColumn3";
		this.gridColumn3.PrimaryKey = false;
		this.gridColumn3.UnitField = null;
		this.gridColumn3.Visible = true;
		this.gridColumn3.VisibleIndex = 2;
		this.gridColumn3.Width = 95;
		this.rep_Enum.AutoHeight = false;
		this.rep_Enum.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Enum.EditFormat.FormatString = "n0";
		this.rep_Enum.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.rep_Enum.MaxLength = 10;
		this.rep_Enum.MaxValue = new decimal(new int[4] { 2147483647, 0, 0, 0 });
		this.rep_Enum.Name = "rep_Enum";
		this.gridColumn4.Caption = "Next No.";
		this.gridColumn4.ColumnEdit = this.rep_Enum;
		this.gridColumn4.CurrencyDateField = null;
		this.gridColumn4.CurrencyField = null;
		this.gridColumn4.CurrencyFixed = null;
		this.gridColumn4.CurrencyFixedRate = 0.0;
		this.gridColumn4.CurrencyRateField = null;
		this.gridColumn4.DisplayFormat.FormatString = "n0";
		this.gridColumn4.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gridColumn4.FieldName = "NextNumber";
		this.gridColumn4.Name = "gridColumn4";
		this.gridColumn4.PrimaryKey = false;
		this.gridColumn4.UnitField = null;
		this.gridColumn4.Visible = true;
		this.gridColumn4.VisibleIndex = 3;
		this.gridColumn4.Width = 102;
		this.gridColumn5.Caption = "Last No.";
		this.gridColumn5.ColumnEdit = this.rep_Enum;
		this.gridColumn5.CurrencyDateField = null;
		this.gridColumn5.CurrencyField = null;
		this.gridColumn5.CurrencyFixed = null;
		this.gridColumn5.CurrencyFixedRate = 0.0;
		this.gridColumn5.CurrencyRateField = null;
		this.gridColumn5.DisplayFormat.FormatString = "n0";
		this.gridColumn5.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gridColumn5.FieldName = "LastNum";
		this.gridColumn5.Name = "gridColumn5";
		this.gridColumn5.PrimaryKey = false;
		this.gridColumn5.UnitField = null;
		this.gridColumn5.Visible = true;
		this.gridColumn5.VisibleIndex = 4;
		this.gridColumn5.Width = 104;
		this.gridColumn6.Caption = "Change Menu Names";
		this.gridColumn6.CurrencyDateField = null;
		this.gridColumn6.CurrencyField = null;
		this.gridColumn6.CurrencyFixed = null;
		this.gridColumn6.CurrencyFixedRate = 0.0;
		this.gridColumn6.CurrencyRateField = null;
		this.gridColumn6.FieldName = "DocAlias";
		this.gridColumn6.Name = "gridColumn6";
		this.gridColumn6.PrimaryKey = false;
		this.gridColumn6.UnitField = null;
		this.gridColumn6.Visible = true;
		this.gridColumn6.VisibleIndex = 5;
		this.gridColumn6.Width = 206;
		this.gridColumn8.Caption = "ObjectCode";
		this.gridColumn8.CurrencyDateField = null;
		this.gridColumn8.CurrencyField = null;
		this.gridColumn8.CurrencyFixed = null;
		this.gridColumn8.CurrencyFixedRate = 0.0;
		this.gridColumn8.CurrencyRateField = null;
		this.gridColumn8.FieldName = "ObjectCode";
		this.gridColumn8.Name = "gridColumn8";
		this.gridColumn8.OptionsColumn.ReadOnly = true;
		this.gridColumn8.PrimaryKey = false;
		this.gridColumn8.UnitField = null;
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.EditControl = null;
		this.btn_OK.EditValue = "OK";
		this.btn_OK.FieldName = null;
		this.btn_OK.ImageOptions.Image = Apzon.Forms.Properties.Resources.apply_16x16;
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(0, 488);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.ReadOnly = false;
		this.btn_OK.SettingInVisible = false;
		this.btn_OK.SettingReadonly = false;
		this.btn_OK.Size = new System.Drawing.Size(75, 23);
		this.btn_OK.TabIndex = 1;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = Apzon.Forms.Properties.Resources.cancel_16x16;
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(78, 488);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 2;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.simpleButton2_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(945, 516);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_OK);
		base.Controls.Add(this.grd_Doc);
		base.FormId = "oDocumentNumbering";
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_DocumentNumbering.IconOptions.Icon");
		base.Name = "frm_DocumentNumbering";
		this.Text = "Document Numbering";
		base.Load += new System.EventHandler(this.kethua_Load);
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ObCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Enum).EndInit();
		base.ResumeLayout(false);
	}
}
