using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
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
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.HttpClientHelper;
using DevExpress.Data.Filtering;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.Administrator;

public class frm_UpdateFilter : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private IContainer components;

	private LabelLine labelLine1;

	private TextEditEx txt_Code;

	private LabelLine labelLine2;

	private LabelLine labelLine3;

	private LookUpEdit look_Category;

	private SimpleButtonEx btn_OK;

	private SimpleButtonEx btn_EditFilter;

	private MemoEditEx memo_FilterText;

	private TextEditEx txt_Name;

	private LabelLine labelLine4;

	private GridControlEx grd_Filter;

	private GridViewEx gv_Filter;

	public string Code { get; set; }

	public string Category { get; set; }

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
		{
			WinFuntions.DisableAllControl(Controls);
			return;
		}
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Add:
			btn_OK.Text = Resources.ButtonAdd;
			break;
		case Enums.FormViewModeType.Update:
			btn_OK.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btn_OK.Text = Resources.ButtonOK;
			break;
		}
	}

	public frm_UpdateFilter()
	{
		InitializeComponent();
	}

	private async void btn_OK_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			WinCommons._OpenProcessing();
			DataTable dataTable = (await WinFuntions.CreateCloneTabl("APZ_FILTER")).Clone();
			DataRow dataRow = dataTable.NewRow();
			if (dataRow.IsExistsColumn("Code"))
			{
				dataRow["Code"] = Function.ToString(txt_Code.EditValue);
			}
			if (dataRow.IsExistsColumn("Name"))
			{
				dataRow["Name"] = Function.ToString(txt_Name.EditValue);
			}
			if (dataRow.IsExistsColumn("ObjType"))
			{
				dataRow["ObjType"] = Function.ToString(look_Category.EditValue);
			}
			if (dataRow.IsExistsColumn("FilterText"))
			{
				dataRow["FilterText"] = Function.ToString(memo_FilterText.EditValue);
			}
			dataTable.Rows.Add(dataRow);
			HttpResult httpResult = await _httpClient.FilterSetting.Save(dataTable);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
			}
			else
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
				Close();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private async void frm_UpdateFilter_Load(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			DataSet dataSet = await _httpClient.FilterSetting.GetCategory();
			if (dataSet.IsHasStructure())
			{
				look_Category.Properties.DataSource = dataSet.Tables[1];
				look_Category.Properties.DisplayMember = "ObjDesc";
				look_Category.Properties.ValueMember = "ObjType";
			}
			if (!string.IsNullOrEmpty(Code))
			{
				DataTable dataTable = await _httpClient.FilterSetting.GetFilterByCode(Code);
				if (dataTable.IsNotNull())
				{
					txt_Code.EditValue = Function.ToString(dataTable.Rows[0]["Code"]);
					txt_Name.EditValue = Function.ToString(dataTable.Rows[0]["Name"]);
					memo_FilterText.EditValue = Function.ToString(dataTable.Rows[0]["FilterText"]);
					look_Category.EditValue = Function.ToString(dataTable.Rows[0]["ObjType"]);
				}
				FormMode = Enums.FormViewModeType.Update;
				txt_Code.Properties.ReadOnly = true;
			}
			else
			{
				look_Category.EditValue = Category;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_EditFilter_Click(object sender, EventArgs e)
	{
		try
		{
			DataRowView dataRowView = (DataRowView)look_Category.GetSelectedDataRow();
			if (dataRowView != null)
			{
				string datafilter = Function.ToString(dataRowView["DataFilter"]);
				DataTable dataSource = await _httpClient.FilterSetting.GetDataFilter(datafilter);
				grd_Filter.DataSource = dataSource;
				grd_Filter.RefreshDataSource();
				gv_Filter.PopulateColumns();
				gv_Filter.ActiveFilterString = Function.ToString(memo_FilterText.EditValue);
				if (gv_Filter.Columns.Count > 0)
				{
					gv_Filter.ShowFilterEditor(gv_Filter.Columns[0]);
				}
				string dataSetWhere = CriteriaToWhereClauseHelper.GetDataSetWhere(gv_Filter.ActiveFilterCriteria);
				memo_FilterText.EditValue = dataSetWhere;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void txt_Code_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Add)
			{
				FormMode = Enums.FormViewModeType.Update;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Administrator.frm_UpdateFilter));
		this.txt_Code = new Apzon.Controls.Editor.TextEditEx();
		this.look_Category = new DevExpress.XtraEditors.LookUpEdit();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_EditFilter = new Apzon.Controls.Controls.SimpleButtonEx();
		this.memo_FilterText = new Apzon.Controls.Editor.MemoEditEx();
		this.txt_Name = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.grd_Filter = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Filter = new Apzon.Controls.Grid.GridViewEx();
		((System.ComponentModel.ISupportInitialize)this.txt_Code.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.look_Category.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.memo_FilterText.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Name.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Filter).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Filter).BeginInit();
		base.SuspendLayout();
		this.txt_Code.Location = new System.Drawing.Point(90, 10);
		this.txt_Code.Name = "txt_Code";
		this.txt_Code.Size = new System.Drawing.Size(366, 20);
		this.txt_Code.TabIndex = 1;
		this.txt_Code.EditValueChanged += new System.EventHandler(this.txt_Code_EditValueChanged);
		this.look_Category.Location = new System.Drawing.Point(90, 58);
		this.look_Category.Name = "look_Category";
		this.look_Category.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.look_Category.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[3]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ObjType", "Code", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ObjDesc", 100, "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("DataFilter", "DataFilter", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default)
		});
		this.look_Category.Properties.NullText = "";
		this.look_Category.Size = new System.Drawing.Size(366, 20);
		this.look_Category.TabIndex = 3;
		this.look_Category.EditValueChanged += new System.EventHandler(this.txt_Code_EditValueChanged);
		this.btn_OK.Image = (System.Drawing.Image)resources.GetObject("btn_OK.Image");
		this.btn_OK.Location = new System.Drawing.Point(13, 275);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.Size = new System.Drawing.Size(80, 20);
		this.btn_OK.TabIndex = 6;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.btn_EditFilter.Image = (System.Drawing.Image)resources.GetObject("btn_EditFilter.Image");
		this.btn_EditFilter.Location = new System.Drawing.Point(357, 275);
		this.btn_EditFilter.Name = "btn_EditFilter";
		this.btn_EditFilter.Size = new System.Drawing.Size(98, 20);
		this.btn_EditFilter.TabIndex = 5;
		this.btn_EditFilter.Text = "Edit Filter Text";
		this.btn_EditFilter.Click += new System.EventHandler(this.btn_EditFilter_Click);
		this.memo_FilterText.Location = new System.Drawing.Point(90, 84);
		this.memo_FilterText.Name = "memo_FilterText";
		this.memo_FilterText.Size = new System.Drawing.Size(365, 185);
		this.memo_FilterText.TabIndex = 4;
		this.memo_FilterText.EditValueChanged += new System.EventHandler(this.txt_Code_EditValueChanged);
		this.txt_Name.Location = new System.Drawing.Point(89, 33);
		this.txt_Name.Name = "txt_Name";
		this.txt_Name.Size = new System.Drawing.Size(366, 20);
		this.txt_Name.TabIndex = 2;
		this.txt_Name.EditValueChanged += new System.EventHandler(this.txt_Code_EditValueChanged);
		this.labelLine4.Location = new System.Drawing.Point(12, 36);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.Size = new System.Drawing.Size(80, 20);
		this.labelLine4.TabIndex = 10;
		this.labelLine4.Utext = "Name";
		this.labelLine3.Location = new System.Drawing.Point(13, 60);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.Size = new System.Drawing.Size(80, 20);
		this.labelLine3.TabIndex = 4;
		this.labelLine3.Utext = "Category";
		this.labelLine2.Location = new System.Drawing.Point(13, 252);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.Size = new System.Drawing.Size(80, 20);
		this.labelLine2.TabIndex = 2;
		this.labelLine2.Utext = "Filter Text";
		this.labelLine1.Location = new System.Drawing.Point(13, 13);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.Size = new System.Drawing.Size(80, 20);
		this.labelLine1.TabIndex = 0;
		this.labelLine1.Utext = "Code";
		this.grd_Filter.Location = new System.Drawing.Point(12, 86);
		this.grd_Filter.MainView = this.gv_Filter;
		this.grd_Filter.Name = "grd_Filter";
		this.grd_Filter.Size = new System.Drawing.Size(72, 160);
		this.grd_Filter.TabIndex = 11;
		this.grd_Filter.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Filter });
		this.grd_Filter.Visible = false;
		this.gv_Filter.GridControl = this.grd_Filter;
		this.gv_Filter.Name = "gv_Filter";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(468, 307);
		base.Controls.Add(this.grd_Filter);
		base.Controls.Add(this.txt_Name);
		base.Controls.Add(this.labelLine4);
		base.Controls.Add(this.memo_FilterText);
		base.Controls.Add(this.btn_EditFilter);
		base.Controls.Add(this.btn_OK);
		base.Controls.Add(this.look_Category);
		base.Controls.Add(this.labelLine3);
		base.Controls.Add(this.labelLine2);
		base.Controls.Add(this.txt_Code);
		base.Controls.Add(this.labelLine1);
		base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
		base.Name = "frm_UpdateFilter";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Filter";
		base.Load += new System.EventHandler(this.frm_UpdateFilter_Load);
		((System.ComponentModel.ISupportInitialize)this.txt_Code.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.look_Category.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.memo_FilterText.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Name.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Filter).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Filter).EndInit();
		base.ResumeLayout(false);
	}
}
