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
using Apzon.Commons.Helper;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Forms.Helper;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraLayout;

namespace Apzon.Forms.Reports;

public class frm_ExtractQueryReportResult : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private List<Tuple<string, string, BaseEdit, bool>> _parameters = new List<Tuple<string, string, BaseEdit, bool>>();

	private IContainer components;

	private SplitContainerControl splitContainerControl1;

	private GridControlEx grd_Result;

	private GridViewEx gv_Result;

	private LayoutControl layoutControl_Udf;

	private LayoutControl layoutControl11;

	private LayoutControlGroup layout_UdfLeft;

	private LayoutControlGroup layout_Udf;

	private LayoutControlItemEx layoutControlItem18;

	private SimpleButtonEx btn_Submit;

	public int ReportId { get; set; }

	public frm_ExtractQueryReportResult()
	{
		InitializeComponent();
	}

	private async void frm_ExtractQueryReportResult_Load(object sender, EventArgs e)
	{
		try
		{
			await LoadParameters();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadParameters()
	{
		try
		{
			DataSet dataSet = await _httpClient.Queries.LoadParameters(ReportId);
			if (dataSet == null || dataSet.Tables.Count <= 0)
			{
				return;
			}
			DataTable dataTable = dataSet.Tables[0];
			if (!dataTable.IsNotNull())
			{
				return;
			}
			_parameters = new List<Tuple<string, string, BaseEdit, bool>>();
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				string text = Function.ToString(dataTable.Rows[i]["Name"]);
				string text2 = Function.ToString(dataTable.Rows[i]["DbType"]);
				if (Function.ToString(dataTable.Rows[i]["IsApply"]).Equals("N"))
				{
					_parameters.Add(new Tuple<string, string, BaseEdit, bool>(text, text2, new BaseEdit(), item4: false));
					continue;
				}
				LayoutControlItem layoutControlItem = layout_UdfLeft.AddItem();
				if (text2 == "datetime")
				{
					DateEdit dateEdit = new DateEdit
					{
						Visible = true,
						Name = "PR_" + text,
						EditValue = null,
						Size = new Size(380, 20)
					};
					dateEdit.SetFormatControl(Enums.DisplayFormatType.DateTime);
					dateEdit.StyleController = layoutControl_Udf;
					layoutControlItem.Control = dateEdit;
					dateEdit.StyleController = layoutControl_Udf;
					layoutControlItem.Control = dateEdit;
					_parameters.Add(new Tuple<string, string, BaseEdit, bool>(text, text2, dateEdit, item4: true));
				}
				layoutControlItem.Text = Function.ToString(dataTable.Rows[i]["Caption"]) ?? text;
				layoutControlItem.AppearanceItemCaption.Font = new Font(layoutControlItem.AppearanceItemCaption.Font.FontFamily, layoutControlItem.AppearanceItemCaption.Font.Size, FontStyle.Underline);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Submit_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Id", typeof(int));
			dataTable.Columns.Add("Name", typeof(string));
			dataTable.Columns.Add("Type", typeof(string));
			dataTable.Columns.Add("Value", typeof(object));
			dataTable.Columns.Add("IsApply", typeof(bool));
			foreach (Tuple<string, string, BaseEdit, bool> parameter in _parameters)
			{
				DataRow dataRow = dataTable.NewRow();
				object value = null;
				switch (parameter.Item2)
				{
				case "datetime":
					value = (parameter.Item4 ? Function.ParseDateTime(parameter.Item3.EditValue) : new DateTime?(new DateTime(1900, 1, 1)));
					break;
				case "varchar":
				case "nvarchar":
				case "char":
				case "nchar":
				case "text":
				case "ntext":
					value = (parameter.Item4 ? Function.ToString(parameter.Item3.EditValue) : string.Empty);
					break;
				}
				dataRow["Id"] = ReportId;
				dataRow["Name"] = parameter.Item1;
				dataRow["Type"] = parameter.Item2;
				dataRow["Value"] = value;
				dataTable.Rows.Add(dataRow);
			}
			if (!dataTable.IsNotNull())
			{
				DataRow dataRow2 = dataTable.NewRow();
				dataRow2["Id"] = ReportId;
				dataRow2["Name"] = "";
				dataRow2["Type"] = "";
				dataRow2["Value"] = "";
				dataTable.Rows.Add(dataRow2);
			}
			DataSet dataSet = await _httpClient.Queries.SubmitQueryReport(dataTable);
			if (dataSet.Tables.Count > 0)
			{
				DataTable dataSource = dataSet.Tables[0];
				grd_Result.DataSource = dataSource;
				grd_Result.RefreshDataSource();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Reports.frm_ExtractQueryReportResult));
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.btn_Submit = new Apzon.Controls.Controls.SimpleButtonEx();
		this.layoutControl_Udf = new DevExpress.XtraLayout.LayoutControl();
		this.layoutControl11 = new DevExpress.XtraLayout.LayoutControl();
		this.layout_UdfLeft = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layout_Udf = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem18 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.grd_Result = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Result = new Apzon.Controls.Grid.GridViewEx();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.layoutControl_Udf).BeginInit();
		this.layoutControl_Udf.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.layoutControl11).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layout_UdfLeft).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layout_Udf).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem18).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Result).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Result).BeginInit();
		base.SuspendLayout();
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(1, -1);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.btn_Submit);
		this.splitContainerControl1.Panel1.Controls.Add(this.layoutControl_Udf);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.grd_Result);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(902, 668);
		this.splitContainerControl1.SplitterPosition = 104;
		this.splitContainerControl1.TabIndex = 0;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.btn_Submit.EditControl = null;
		this.btn_Submit.EditValue = "Submit";
		this.btn_Submit.FieldName = null;
		this.btn_Submit.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Submit.ImageOptions.Image");
		this.btn_Submit.IsSearchControl = false;
		this.btn_Submit.Location = new System.Drawing.Point(462, 48);
		this.btn_Submit.Name = "btn_Submit";
		this.btn_Submit.ReadOnly = false;
		this.btn_Submit.SettingInVisible = false;
		this.btn_Submit.SettingReadonly = false;
		this.btn_Submit.Size = new System.Drawing.Size(75, 23);
		this.btn_Submit.TabIndex = 2;
		this.btn_Submit.Text = "Submit";
		this.btn_Submit.Click += new System.EventHandler(this.btn_Submit_Click);
		this.layoutControl_Udf.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.layoutControl_Udf.Controls.Add(this.layoutControl11);
		this.layoutControl_Udf.Location = new System.Drawing.Point(0, 0);
		this.layoutControl_Udf.Margin = new System.Windows.Forms.Padding(0);
		this.layoutControl_Udf.Name = "layoutControl_Udf";
		this.layoutControl_Udf.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(860, 449, 250, 350);
		this.layoutControl_Udf.Root = this.layout_Udf;
		this.layoutControl_Udf.Size = new System.Drawing.Size(459, 106);
		this.layoutControl_Udf.TabIndex = 1;
		this.layoutControl_Udf.Text = "layoutControl_Udf";
		this.layoutControl11.Location = new System.Drawing.Point(12, 12);
		this.layoutControl11.Margin = new System.Windows.Forms.Padding(0);
		this.layoutControl11.Name = "layoutControl11";
		this.layoutControl11.Root = this.layout_UdfLeft;
		this.layoutControl11.Size = new System.Drawing.Size(435, 82);
		this.layoutControl11.TabIndex = 4;
		this.layoutControl11.Text = "layoutControl11";
		this.layout_UdfLeft.CustomizationFormText = "layout_UdfLeft";
		this.layout_UdfLeft.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layout_UdfLeft.GroupBordersVisible = false;
		this.layout_UdfLeft.Name = "layout_UdfLeft";
		this.layout_UdfLeft.Size = new System.Drawing.Size(435, 82);
		this.layout_UdfLeft.TextVisible = false;
		this.layout_Udf.CustomizationFormText = "layout_Udf_Left";
		this.layout_Udf.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layout_Udf.GroupBordersVisible = false;
		this.layout_Udf.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[1] { this.layoutControlItem18 });
		this.layout_Udf.Name = "layout_Udf";
		this.layout_Udf.Size = new System.Drawing.Size(459, 106);
		this.layout_Udf.TextVisible = false;
		this.layoutControlItem18.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem18.Control = this.layoutControl11;
		this.layoutControlItem18.CustomizationFormText = "layoutControlItem18";
		this.layoutControlItem18.EditControl = null;
		this.layoutControlItem18.EditValue = "layoutControlItem18";
		this.layoutControlItem18.FieldName = null;
		this.layoutControlItem18.IsSearchControl = false;
		this.layoutControlItem18.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem18.Name = "layoutControlItem18";
		this.layoutControlItem18.ReadOnly = false;
		this.layoutControlItem18.SettingInVisible = false;
		this.layoutControlItem18.SettingReadonly = false;
		this.layoutControlItem18.Size = new System.Drawing.Size(439, 86);
		this.layoutControlItem18.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem18.TextVisible = false;
		this.grd_Result.AllowPasteExcel = true;
		this.grd_Result.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Result.ClipboardData = null;
		this.grd_Result.Location = new System.Drawing.Point(0, 0);
		this.grd_Result.MainView = this.gv_Result;
		this.grd_Result.Name = "grd_Result";
		this.grd_Result.Size = new System.Drawing.Size(902, 559);
		this.grd_Result.TabIndex = 0;
		this.grd_Result.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Result });
		this.gv_Result.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Result.ColumnKey = null;
		this.gv_Result.DisableFormSetting = false;
		this.gv_Result.DisplayException = false;
		this.gv_Result.EditColumn = null;
		this.gv_Result.FieldCurrency = null;
		this.gv_Result.FieldRate = null;
		this.gv_Result.FormId = "oExtractQueryReportResult";
		this.gv_Result.GridControl = this.grd_Result;
		this.gv_Result.IgnoreFilter = false;
		this.gv_Result.IndexEditRow = -1;
		this.gv_Result.IndicatorWidth = 35;
		this.gv_Result.IsApzTable = false;
		this.gv_Result.IsInsertNewRow = false;
		this.gv_Result.IsLoadingData = false;
		this.gv_Result.IsMappingContextAction = false;
		this.gv_Result.ItemId = "ORESULT";
		this.gv_Result.LanguageSource = Apzon.Commons.Enums.Enums.LanguageSource.Documents;
		this.gv_Result.LstColumnAllowBatch = null;
		this.gv_Result.LstColumnNotAllowBatch = null;
		this.gv_Result.LstColumnNotEditable = null;
		this.gv_Result.LstColumNotSetting = null;
		this.gv_Result.Name = "gv_Result";
		this.gv_Result.UseBatchMod = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(904, 709);
		base.Controls.Add(this.splitContainerControl1);
		base.FormId = "oExtractQueryReportResult";
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_ExtractQueryReportResult.IconOptions.Icon");
		base.Name = "frm_ExtractQueryReportResult";
		this.Text = "Extract Query Report Result";
		base.Load += new System.EventHandler(this.frm_ExtractQueryReportResult_Load);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.layoutControl_Udf).EndInit();
		this.layoutControl_Udf.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.layoutControl11).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layout_UdfLeft).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layout_Udf).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem18).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Result).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Result).EndInit();
		base.ResumeLayout(false);
	}
}
