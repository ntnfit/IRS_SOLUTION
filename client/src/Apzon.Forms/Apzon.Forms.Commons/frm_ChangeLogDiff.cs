using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Controls;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.Commons;

public class frm_ChangeLogDiff : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private IContainer components;

	private GridControlEx grd_Diff;

	private GridViewEx gv_Diff;

	private SimpleButton btn_Close;

	private GridColumnEx gridColumnEx1;

	private GridColumnEx gridColumnEx2;

	private GridColumnEx gridColumnEx3;

	private GridColumnEx gridColumnEx4;

	private GridColumnEx gridColumnEx5;

	private GridColumnEx gridColumnEx6;

	private GridColumnEx gridColumnEx7;

	public GetChangeLogParams GetChangeLogParams { get; set; }

	public string LogFormId { get; set; }

	public frm_ChangeLogDiff()
	{
		InitializeComponent();
	}

	private async Task frm_ChangeLogDiff_Load(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			DataTable dtFormSettingAll = null;
			if (GetChangeLogParams == null)
			{
				return;
			}
			DataTable dt = await _httpClient.ChangeLogClient.GetChangeLogDifferences(GetChangeLogParams);
			if (GetChangeLogParams.IsApzTable)
			{
				List<string> list = (from t in dt.AsEnumerable()
					select Function.ToString(t["ArrayOffset"])).Distinct().ToList();
				foreach (string item in list)
				{
					if (string.IsNullOrWhiteSpace(LogFormId) || string.IsNullOrWhiteSpace(item))
					{
						continue;
					}
					DataTable dataTable = await _httpClient.FormSettingClient.GetFormSetting(LogFormId, item, GlobalData.UserName, GlobalData.CURRENTLANGUAGE);
					if (dataTable == null || dataTable.Rows.Count <= 0)
					{
						continue;
					}
					foreach (DataRow row in dataTable.Rows)
					{
						if (!dtFormSettingAll.IsNotNull())
						{
							dtFormSettingAll = dataTable.Copy();
						}
						else
						{
							dtFormSettingAll?.Rows.Add(row.ItemArray);
						}
					}
				}
				for (int num = 0; num < dt.Rows.Count; num++)
				{
					string colName = Function.ToString(dt.Rows[num]["ChangedField"]);
					string table = Function.ToString(dt.Rows[num]["ArrayOffset"]);
					DataRow dataRow2 = dtFormSettingAll?.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["ColumnId"]) == colName && Function.ToString(t["ItemId"]) == table);
					if (dataRow2 != null)
					{
						dt.Rows[num]["ChangedField"] = Function.ToString(dataRow2["ColumnName"]);
					}
				}
			}
			grd_Diff.DataSource = dt;
			grd_Diff.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Close_Click(object sender, EventArgs e)
	{
		Close();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Commons.frm_ChangeLogDiff));
		this.grd_Diff = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Diff = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx5 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx6 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx7 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_Close = new DevExpress.XtraEditors.SimpleButton();
		((System.ComponentModel.ISupportInitialize)this.grd_Diff).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Diff).BeginInit();
		base.SuspendLayout();
		this.grd_Diff.AllowPasteExcel = true;
		this.grd_Diff.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Diff.ClipboardData = null;
		this.grd_Diff.Location = new System.Drawing.Point(0, -1);
		this.grd_Diff.MainView = this.gv_Diff;
		this.grd_Diff.Name = "grd_Diff";
		this.grd_Diff.Size = new System.Drawing.Size(745, 565);
		this.grd_Diff.TabIndex = 0;
		this.grd_Diff.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Diff });
		this.gv_Diff.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Diff.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Diff.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Diff.ColumnKey = null;
		this.gv_Diff.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[7] { this.gridColumnEx1, this.gridColumnEx2, this.gridColumnEx3, this.gridColumnEx4, this.gridColumnEx5, this.gridColumnEx6, this.gridColumnEx7 });
		this.gv_Diff.DisableFormSetting = false;
		this.gv_Diff.DisplayException = false;
		this.gv_Diff.EditColumn = null;
		this.gv_Diff.FieldCurrency = null;
		this.gv_Diff.FieldRate = null;
		this.gv_Diff.FormId = "oChangeLogDriff";
		this.gv_Diff.GridControl = this.grd_Diff;
		this.gv_Diff.IgnoreFilter = false;
		this.gv_Diff.IndexEditRow = -1;
		this.gv_Diff.IndicatorWidth = 35;
		this.gv_Diff.IsApzTable = false;
		this.gv_Diff.IsInsertNewRow = false;
		this.gv_Diff.IsLoadingData = false;
		this.gv_Diff.IsMappingContextAction = false;
		this.gv_Diff.ItemId = "ODATALOG";
		this.gv_Diff.LstColumnAllowBatch = null;
		this.gv_Diff.LstColumnNotAllowBatch = null;
		this.gv_Diff.LstColumnNotEditable = null;
		this.gv_Diff.LstColumNotSetting = null;
		this.gv_Diff.Name = "gv_Diff";
		this.gv_Diff.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Diff.OptionsPrint.AutoWidth = false;
		this.gv_Diff.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Diff.OptionsView.ColumnAutoWidth = false;
		this.gv_Diff.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Diff.OptionsView.ShowGroupPanel = false;
		this.gv_Diff.UseBatchMod = false;
		this.gridColumnEx1.Caption = "#";
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "LineNumber";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.PrimaryKey = true;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx1.Visible = true;
		this.gridColumnEx1.VisibleIndex = 0;
		this.gridColumnEx1.Width = 100;
		this.gridColumnEx2.Caption = "ArrayOffset";
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.FieldName = "ArrayOffset";
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.UnitField = null;
		this.gridColumnEx2.Visible = true;
		this.gridColumnEx2.VisibleIndex = 1;
		this.gridColumnEx2.Width = 100;
		this.gridColumnEx3.Caption = "Date";
		this.gridColumnEx3.CurrencyDateField = null;
		this.gridColumnEx3.CurrencyField = null;
		this.gridColumnEx3.CurrencyFixed = null;
		this.gridColumnEx3.CurrencyFixedRate = 0.0;
		this.gridColumnEx3.CurrencyRateField = null;
		this.gridColumnEx3.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gridColumnEx3.FieldName = "Date";
		this.gridColumnEx3.Name = "gridColumnEx3";
		this.gridColumnEx3.PrimaryKey = false;
		this.gridColumnEx3.UnitField = null;
		this.gridColumnEx3.Visible = true;
		this.gridColumnEx3.VisibleIndex = 2;
		this.gridColumnEx3.Width = 100;
		this.gridColumnEx4.Caption = "Changed Field";
		this.gridColumnEx4.CurrencyDateField = null;
		this.gridColumnEx4.CurrencyField = null;
		this.gridColumnEx4.CurrencyFixed = null;
		this.gridColumnEx4.CurrencyFixedRate = 0.0;
		this.gridColumnEx4.CurrencyRateField = null;
		this.gridColumnEx4.FieldName = "ChangedField";
		this.gridColumnEx4.Name = "gridColumnEx4";
		this.gridColumnEx4.PrimaryKey = false;
		this.gridColumnEx4.UnitField = null;
		this.gridColumnEx4.Visible = true;
		this.gridColumnEx4.VisibleIndex = 3;
		this.gridColumnEx4.Width = 100;
		this.gridColumnEx5.Caption = "Old Value";
		this.gridColumnEx5.CurrencyDateField = null;
		this.gridColumnEx5.CurrencyField = null;
		this.gridColumnEx5.CurrencyFixed = null;
		this.gridColumnEx5.CurrencyFixedRate = 0.0;
		this.gridColumnEx5.CurrencyRateField = null;
		this.gridColumnEx5.FieldName = "OldValue";
		this.gridColumnEx5.Name = "gridColumnEx5";
		this.gridColumnEx5.PrimaryKey = false;
		this.gridColumnEx5.UnitField = null;
		this.gridColumnEx5.Visible = true;
		this.gridColumnEx5.VisibleIndex = 4;
		this.gridColumnEx5.Width = 100;
		this.gridColumnEx6.Caption = "New Value";
		this.gridColumnEx6.CurrencyDateField = null;
		this.gridColumnEx6.CurrencyField = null;
		this.gridColumnEx6.CurrencyFixed = null;
		this.gridColumnEx6.CurrencyFixedRate = 0.0;
		this.gridColumnEx6.CurrencyRateField = null;
		this.gridColumnEx6.FieldName = "NewValue";
		this.gridColumnEx6.Name = "gridColumnEx6";
		this.gridColumnEx6.PrimaryKey = false;
		this.gridColumnEx6.UnitField = null;
		this.gridColumnEx6.Visible = true;
		this.gridColumnEx6.VisibleIndex = 5;
		this.gridColumnEx6.Width = 100;
		this.gridColumnEx7.Caption = "User Name";
		this.gridColumnEx7.CurrencyDateField = null;
		this.gridColumnEx7.CurrencyField = null;
		this.gridColumnEx7.CurrencyFixed = null;
		this.gridColumnEx7.CurrencyFixedRate = 0.0;
		this.gridColumnEx7.CurrencyRateField = null;
		this.gridColumnEx7.FieldName = "UserName";
		this.gridColumnEx7.Name = "gridColumnEx7";
		this.gridColumnEx7.PrimaryKey = false;
		this.gridColumnEx7.UnitField = null;
		this.gridColumnEx7.Visible = true;
		this.gridColumnEx7.VisibleIndex = 6;
		this.gridColumnEx7.Width = 100;
		this.btn_Close.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Close.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Close.ImageOptions.Image");
		this.btn_Close.Location = new System.Drawing.Point(9, 569);
		this.btn_Close.Name = "btn_Close";
		this.btn_Close.Size = new System.Drawing.Size(75, 23);
		this.btn_Close.TabIndex = 1;
		this.btn_Close.Text = "OK";
		this.btn_Close.Click += new System.EventHandler(this.btn_Close_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(746, 597);
		base.Controls.Add(this.btn_Close);
		base.Controls.Add(this.grd_Diff);
		base.FormId = "oChangeLogDriff";
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_ChangeLogDiff.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_ChangeLogDiff";
		base.ShowInTaskbar = false;
		this.Text = "Change Log Differences";
		base.OnLoadAsync += new Apzon.Controls.Form.LoadAsyncEventHandler(this.frm_ChangeLogDiff_Load);
		((System.ComponentModel.ISupportInitialize)this.grd_Diff).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Diff).EndInit();
		base.ResumeLayout(false);
	}
}
