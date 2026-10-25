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
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Mrps;

public class frm_SalesForeCastQuickAnalys : XtraFormEx
{
	private DataTable dt = new DataTable();

	private DataTable dtQuickAnalysis = new DataTable();

	private DataTable table = new DataTable();

	private List<Common.Table_Code> lstSaleEmp = new List<Common.Table_Code>();

	private List<int> lstYear = new List<int>();

	private bool status;

	private IContainer components;

	private SimpleButtonEx btnSearch;

	private LabelLine labelLine2;

	private LabelLine labelLine6;

	private LabelLine labelLine7;

	private LabelLine labelLine8;

	private TextEditEx txtBPName;

	private LabelLine labelLine5;

	private LabelLine labelLine4;

	private LabelLine labelLine3;

	private LabelLine labelLine1;

	private ButtonEditEx btnBPCode;

	private ComboBoxEditEx cbType;

	private ButtonEditEx btnWareHouse;

	private ComboBoxEditEx cbStatus;

	private ButtonEditEx btnDivision;

	private GridControlEx gridQuickAnalysis;

	private GridViewEx gvQuickAnalysis;

	private RepositoryItemLookUpEdit rILSalesEmp;

	private RepositoryItemLookUpEdit rILStaSalesEmpl;

	private RepositoryItemLookUpEdit repYear;

	private ComboBoxEditEx dtYear;

	private GridLookUpEdit cbEmployee;

	private GridViewEx gridLookUpEdit1View;

	private RepositoryItemComboBox repStatus;

	public frm_SalesForeCastQuickAnalys()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "SalesForeCastQuickAnalys"))
		{
			InitializeComponent();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "SalesForeCastQuickAnalys" && x.authoriza == "F"))
			{
				WinFuntions.DisableAllControl(Controls);
				btnSearch.Enabled = true;
			}
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	private async Task LoadComboEmployee()
	{
		try
		{
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "SaleEmployee");
			dt = await _helper.GetDataTableAsync("ShowSaleEmployee");
			if (dt == null || dt.Rows.Count <= 0)
			{
				return;
			}
			for (int i = 0; i < dt.Columns.Count; i++)
			{
				if (!dt.Columns[i].ColumnName.Equals("SlpCode") && !dt.Columns[i].ColumnName.Equals("SlpName"))
				{
					dt.Columns.RemoveAt(i);
					i--;
				}
			}
			cbEmployee.Properties.DataSource = dt;
			rILSalesEmp.DataSource = dt;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadGrid(DataTable dt)
	{
		try
		{
			WinCommons._OpenProcessing();
			using (HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "SalesForeCast"))
			{
				gvQuickAnalysis.Columns.Clear();
				gridQuickAnalysis.DataSource = null;
				GridColumn gridColumn = new GridColumn();
				gridColumn.Caption = "Item Code";
				gridColumn.FieldName = "ItemCode";
				gridColumn.Visible = true;
				gridColumn.Fixed = FixedStyle.Left;
				gridColumn.Width = 100;
				gvQuickAnalysis.Columns.Add(gridColumn);
				GridColumn gridColumn2 = new GridColumn();
				gridColumn2.Caption = "Item Name";
				gridColumn2.FieldName = "ItemName";
				gridColumn2.Fixed = FixedStyle.Left;
				gridColumn2.Visible = true;
				gridColumn2.Width = 150;
				gvQuickAnalysis.Columns.Add(gridColumn2);
				GridColumn gridColumn3 = new GridColumn();
				gridColumn3.Caption = "Jan " + dtYear.Text;
				gridColumn3.FieldName = "1";
				gridColumn3.Visible = true;
				gridColumn3.Width = 75;
				gvQuickAnalysis.Columns.Add(gridColumn3);
				GridColumn gridColumn4 = new GridColumn();
				gridColumn4.Caption = "Feb " + dtYear.Text;
				gridColumn4.FieldName = "2";
				gridColumn4.Width = 75;
				gridColumn4.Visible = true;
				gvQuickAnalysis.Columns.Add(gridColumn4);
				GridColumn gridColumn5 = new GridColumn();
				gridColumn5.Visible = true;
				gridColumn5.Width = 75;
				gridColumn5.Caption = "Mar " + dtYear.Text;
				gridColumn5.FieldName = "3";
				gvQuickAnalysis.Columns.Add(gridColumn5);
				GridColumn gridColumn6 = new GridColumn();
				gridColumn6.Caption = "Apr " + dtYear.Text;
				gridColumn6.FieldName = "4";
				gridColumn6.Width = 75;
				gridColumn6.Visible = true;
				gvQuickAnalysis.Columns.Add(gridColumn6);
				GridColumn gridColumn7 = new GridColumn();
				gridColumn7.Caption = "May " + dtYear.Text;
				gridColumn7.FieldName = "5";
				gridColumn7.Width = 75;
				gridColumn7.Visible = true;
				gvQuickAnalysis.Columns.Add(gridColumn7);
				GridColumn gridColumn8 = new GridColumn();
				gridColumn8.Caption = "Jun " + dtYear.Text;
				gridColumn8.FieldName = "6";
				gridColumn8.Width = 75;
				gvQuickAnalysis.Columns.Add(gridColumn8);
				GridColumn gridColumn9 = new GridColumn();
				gridColumn9.Caption = "Jul " + dtYear.Text;
				gridColumn9.FieldName = "7";
				gridColumn9.Width = 75;
				gridColumn9.Visible = true;
				gvQuickAnalysis.Columns.Add(gridColumn9);
				GridColumn gridColumn10 = new GridColumn();
				gridColumn10.Caption = "Aug " + dtYear.Text;
				gridColumn10.FieldName = "8";
				gridColumn10.Width = 75;
				gridColumn10.Visible = true;
				gvQuickAnalysis.Columns.Add(gridColumn10);
				GridColumn gridColumn11 = new GridColumn();
				gridColumn11.Caption = "Sep " + dtYear.Text;
				gridColumn11.FieldName = "9";
				gridColumn11.Visible = true;
				gridColumn11.Width = 75;
				gvQuickAnalysis.Columns.Add(gridColumn11);
				GridColumn gridColumn12 = new GridColumn();
				gridColumn12.Caption = "Oct " + dtYear.Text;
				gridColumn12.Visible = true;
				gridColumn12.FieldName = "10";
				gridColumn12.Width = 75;
				gvQuickAnalysis.Columns.Add(gridColumn12);
				GridColumn gridColumn13 = new GridColumn();
				gridColumn13.Caption = "Nov " + dtYear.Text;
				gridColumn13.FieldName = "11";
				gridColumn13.Visible = true;
				gridColumn13.Width = 75;
				gvQuickAnalysis.Columns.Add(gridColumn13);
				GridColumn gridColumn14 = new GridColumn();
				gridColumn14.Caption = "Dec " + dtYear.Text;
				gridColumn14.FieldName = "12";
				gridColumn14.Width = 75;
				gridColumn14.Visible = true;
				gvQuickAnalysis.Columns.Add(gridColumn14);
				GridColumn gridColumn15 = new GridColumn();
				gridColumn15.Caption = "Forecast Type";
				gridColumn15.Visible = true;
				gridColumn15.Width = 75;
				gridColumn15.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Far;
				gridColumn15.FieldName = "ForecastType";
				gvQuickAnalysis.Columns.Add(gridColumn15);
				GridColumn gridColumn16 = new GridColumn();
				gridColumn16.Caption = "BP Code";
				gridColumn16.Visible = true;
				gridColumn16.Width = 100;
				gridColumn16.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Far;
				gridColumn16.FieldName = "BpCode";
				gvQuickAnalysis.Columns.Add(gridColumn16);
				GridColumn gridColumn17 = new GridColumn();
				gridColumn17.Caption = "BP Name";
				gridColumn17.Visible = true;
				gridColumn17.Width = 100;
				gridColumn17.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Far;
				gridColumn17.FieldName = "BPName";
				gvQuickAnalysis.Columns.Add(gridColumn17);
				GridColumn gridColumn18 = new GridColumn();
				gridColumn18.Caption = "Employee";
				gridColumn18.FieldName = "Employee";
				gridColumn18.Visible = true;
				gridColumn18.ColumnEdit = rILSalesEmp;
				gridColumn18.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Far;
				gridColumn18.Width = 100;
				gvQuickAnalysis.Columns.Add(gridColumn18);
				GridColumn gridColumn19 = new GridColumn();
				gridColumn19.Caption = "Status";
				gridColumn19.FieldName = "Status";
				gridColumn19.Visible = true;
				gridColumn19.ColumnEdit = repStatus;
				gridColumn19.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Far;
				gridColumn19.Width = 100;
				gvQuickAnalysis.Columns.Add(gridColumn19);
				GridColumn gridColumn20 = new GridColumn();
				gridColumn20.Caption = "Div";
				gridColumn20.Visible = true;
				gridColumn20.Width = 100;
				gridColumn20.FieldName = "Division";
				gridColumn20.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Far;
				gvQuickAnalysis.Columns.Add(gridColumn20);
				GridColumn gridColumn21 = new GridColumn();
				gridColumn21.Caption = "Whse";
				gridColumn21.Visible = true;
				gridColumn21.Width = 50;
				gridColumn21.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Far;
				gridColumn21.FieldName = "Whse";
				gvQuickAnalysis.Columns.Add(gridColumn21);
				dtQuickAnalysis = await _helper.PostAsync("GetQuickAnalysis", dt);
				if (gridQuickAnalysis.DataSource != null)
				{
					gridQuickAnalysis.DataSource = null;
				}
				gridQuickAnalysis.DataSource = dtQuickAnalysis;
				gridQuickAnalysis.RefreshDataSource();
			}
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void SalesForeCastQuickAnalys_Load(object sender, EventArgs e)
	{
		try
		{
			await LoadComboEmployee();
			GetYearForComboBox();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private DataTable GetDataControl()
	{
		try
		{
			if (!status)
			{
				table.Columns.Add("BPCode");
				table.Columns.Add("ForecastType");
				table.Columns.Add("Year");
				table.Columns.Add("BPName");
				table.Columns.Add("Employee");
				table.Columns.Add("Whse");
				table.Columns.Add("Division");
				table.Columns.Add("Status");
				status = true;
			}
			table.Rows.Clear();
			DataRow dataRow = table.NewRow();
			dataRow["BPCode"] = ((btnBPCode.EditValue == null) ? "" : btnBPCode.EditValue);
			dataRow["ForecastType"] = ((cbType.EditValue == null) ? "" : cbType.EditValue.ToString());
			dataRow["Year"] = ((dtYear.EditValue == null) ? "" : dtYear.EditValue.ToString());
			dataRow["BPName"] = ((txtBPName.EditValue == null) ? "" : txtBPName.EditValue.ToString());
			dataRow["Employee"] = ((cbEmployee.EditValue == null) ? "" : cbEmployee.EditValue.ToString());
			dataRow["Whse"] = ((btnWareHouse.EditValue == null) ? "" : btnWareHouse.EditValue.ToString());
			dataRow["Division"] = ((btnDivision.EditValue == null) ? "" : btnDivision.EditValue.ToString());
			dataRow["Status"] = ((cbStatus.EditValue == null) ? "" : cbStatus.EditValue.ToString());
			table.Rows.Add(dataRow);
			return table;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	private async void GetYearForComboBox()
	{
		try
		{
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "SalesForeCast");
			dt = await _helper.GetDataTableAsync("GetYearForQuickAnalysis");
			if (dt != null && dt.Rows.Count > 0)
			{
				repYear.DataSource = dt;
				repYear.PopulateColumns();
			}
			foreach (DataRow row in dt.Rows)
			{
				lstYear.Add(Function.ParseInt(row["Year"]));
			}
			WinFuntions._addComboBox(dtYear, lstYear, p: false);
			int num = Function.ParseInt(DateTime.Today.ToString("yyyy"));
			foreach (object item in dtYear.Properties.Items)
			{
				if (Function.ParseInt(item) == num)
				{
					dtYear.SelectedItem = item;
				}
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gvQuickAnalysis_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void textEdit1_Properties_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
	}

	private void btnBPCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.Customer, multiSelect: false, Function.ToString(btnBPCode.Text));
			frm_ChooseFromList2.ShowDialog(this);
			if (frm_ChooseFromList2.ChooseItem != null)
			{
				btnBPCode.Text = frm_ChooseFromList2.ChooseItem.CardCode;
				txtBPName.Text = frm_ChooseFromList2.ChooseItem.CardName;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Warning, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private void btnWareHouse_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.Warehouse, multiSelect: false, Function.ToString(btnWareHouse.Text));
			frm_ChooseFromList2.ShowDialog(this);
			if (frm_ChooseFromList2.ChooseItem != null)
			{
				btnWareHouse.Text = Function.ToString(frm_ChooseFromList2.ChooseItem["WhsCode"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Warning, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private async void btnSearch_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataControl = GetDataControl();
			await LoadGrid(dataControl);
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void cbEmployee_EditValueChanged(object sender, EventArgs e)
	{
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
		this.btnSearch = new Apzon.Controls.Controls.SimpleButtonEx();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine6 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine7 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine8 = new Apzon.Controls.Controls.LabelLine();
		this.txtBPName = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine5 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.btnBPCode = new Apzon.Controls.Editor.ButtonEditEx();
		this.cbType = new Apzon.Controls.Controls.ComboBoxEditEx();
		this.btnWareHouse = new Apzon.Controls.Editor.ButtonEditEx();
		this.cbStatus = new Apzon.Controls.Controls.ComboBoxEditEx();
		this.btnDivision = new Apzon.Controls.Editor.ButtonEditEx();
		this.gridQuickAnalysis = new Apzon.Controls.Grid.GridControlEx();
		this.gvQuickAnalysis = new Apzon.Controls.Grid.GridViewEx();
		this.rILSalesEmp = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.rILStaSalesEmpl = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.repYear = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.repStatus = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
		this.dtYear = new Apzon.Controls.Controls.ComboBoxEditEx();
		this.cbEmployee = new DevExpress.XtraEditors.GridLookUpEdit();
		this.gridLookUpEdit1View = new Apzon.Controls.Grid.GridViewEx();
		((System.ComponentModel.ISupportInitialize)this.txtBPName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btnBPCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbType.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btnWareHouse.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbStatus.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btnDivision.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridQuickAnalysis).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gvQuickAnalysis).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rILSalesEmp).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rILStaSalesEmpl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repYear).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repStatus).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dtYear.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbEmployee.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridLookUpEdit1View).BeginInit();
		base.SuspendLayout();
		this.btnSearch.EditControl = null;
		this.btnSearch.EditValue = "Search";
		this.btnSearch.FieldName = null;
		this.btnSearch.IsSearchControl = false;
		this.btnSearch.Location = new System.Drawing.Point(771, 22);
		this.btnSearch.Name = "btnSearch";
		this.btnSearch.ReadOnly = false;
		this.btnSearch.SettingInVisible = false;
		this.btnSearch.SettingReadonly = false;
		this.btnSearch.Size = new System.Drawing.Size(75, 23);
		this.btnSearch.TabIndex = 44;
		this.btnSearch.Text = "Search";
		this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "Employee";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(182, 53);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(72, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 43;
		this.labelLine2.Utext = "Employee";
		this.labelLine6.EditControl = null;
		this.labelLine6.EditValue = "Division";
		this.labelLine6.FieldName = null;
		this.labelLine6.HideWhenEmpty = false;
		this.labelLine6.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine6.Location = new System.Drawing.Point(375, 53);
		this.labelLine6.Name = "labelLine6";
		this.labelLine6.ReadOnly = false;
		this.labelLine6.SettingInVisible = false;
		this.labelLine6.SettingReadonly = false;
		this.labelLine6.ShowUnderLine = true;
		this.labelLine6.Size = new System.Drawing.Size(71, 20);
		this.labelLine6.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine6.TabIndex = 42;
		this.labelLine6.Utext = "Division";
		this.labelLine7.EditControl = null;
		this.labelLine7.EditValue = "Status";
		this.labelLine7.FieldName = null;
		this.labelLine7.HideWhenEmpty = false;
		this.labelLine7.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine7.Location = new System.Drawing.Point(581, 49);
		this.labelLine7.Name = "labelLine7";
		this.labelLine7.ReadOnly = false;
		this.labelLine7.SettingInVisible = false;
		this.labelLine7.SettingReadonly = false;
		this.labelLine7.ShowUnderLine = true;
		this.labelLine7.Size = new System.Drawing.Size(63, 20);
		this.labelLine7.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine7.TabIndex = 41;
		this.labelLine7.Utext = "Status";
		this.labelLine8.EditControl = null;
		this.labelLine8.EditValue = "BP Name";
		this.labelLine8.FieldName = null;
		this.labelLine8.HideWhenEmpty = false;
		this.labelLine8.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine8.Location = new System.Drawing.Point(7, 52);
		this.labelLine8.Name = "labelLine8";
		this.labelLine8.ReadOnly = false;
		this.labelLine8.SettingInVisible = false;
		this.labelLine8.SettingReadonly = false;
		this.labelLine8.ShowUnderLine = true;
		this.labelLine8.Size = new System.Drawing.Size(58, 20);
		this.labelLine8.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine8.TabIndex = 40;
		this.labelLine8.Utext = "BP Name";
		this.txtBPName.EditControl = null;
		this.txtBPName.FieldName = null;
		this.txtBPName.IsSearchControl = false;
		this.txtBPName.Location = new System.Drawing.Point(67, 51);
		this.txtBPName.Name = "txtBPName";
		this.txtBPName.SettingInVisible = false;
		this.txtBPName.SettingReadonly = false;
		this.txtBPName.Size = new System.Drawing.Size(110, 20);
		this.txtBPName.TabIndex = 36;
		this.labelLine5.EditControl = null;
		this.labelLine5.EditValue = "Type";
		this.labelLine5.FieldName = null;
		this.labelLine5.HideWhenEmpty = false;
		this.labelLine5.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine5.Location = new System.Drawing.Point(182, 26);
		this.labelLine5.Name = "labelLine5";
		this.labelLine5.ReadOnly = false;
		this.labelLine5.SettingInVisible = false;
		this.labelLine5.SettingReadonly = false;
		this.labelLine5.ShowUnderLine = true;
		this.labelLine5.Size = new System.Drawing.Size(72, 20);
		this.labelLine5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine5.TabIndex = 35;
		this.labelLine5.Utext = "Type";
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "WareHouse";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(375, 26);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(71, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 34;
		this.labelLine4.Utext = "WareHouse";
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "Year";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(581, 26);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(63, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 33;
		this.labelLine3.Utext = "Year";
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "BP Code";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(7, 26);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(58, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 32;
		this.labelLine1.Utext = "BP Code";
		this.btnBPCode.EditControl = null;
		this.btnBPCode.FieldName = null;
		this.btnBPCode.IsSearchControl = false;
		this.btnBPCode.Location = new System.Drawing.Point(65, 23);
		this.btnBPCode.Name = "btnBPCode";
		this.btnBPCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.btnBPCode.Properties.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.textEdit1_Properties_ButtonPressed);
		this.btnBPCode.SettingInVisible = false;
		this.btnBPCode.SettingReadonly = false;
		this.btnBPCode.Size = new System.Drawing.Size(110, 20);
		this.btnBPCode.TabIndex = 28;
		this.btnBPCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btnBPCode_ButtonClick);
		this.cbType.EditControl = null;
		this.cbType.FieldName = null;
		this.cbType.IsSearchControl = false;
		this.cbType.Location = new System.Drawing.Point(255, 23);
		this.cbType.Name = "cbType";
		this.cbType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbType.Properties.ImmediatePopup = true;
		this.cbType.Properties.Items.AddRange(new object[3] { "Production", "Sales", "     " });
		this.cbType.SettingInVisible = false;
		this.cbType.SettingReadonly = false;
		this.cbType.Size = new System.Drawing.Size(111, 20);
		this.cbType.TabIndex = 30;
		this.btnWareHouse.EditControl = null;
		this.btnWareHouse.FieldName = null;
		this.btnWareHouse.IsSearchControl = false;
		this.btnWareHouse.Location = new System.Drawing.Point(446, 23);
		this.btnWareHouse.Name = "btnWareHouse";
		this.btnWareHouse.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.btnWareHouse.SettingInVisible = false;
		this.btnWareHouse.SettingReadonly = false;
		this.btnWareHouse.Size = new System.Drawing.Size(128, 20);
		this.btnWareHouse.TabIndex = 31;
		this.btnWareHouse.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btnWareHouse_ButtonClick);
		this.cbStatus.EditControl = null;
		this.cbStatus.FieldName = null;
		this.cbStatus.IsSearchControl = false;
		this.cbStatus.Location = new System.Drawing.Point(644, 49);
		this.cbStatus.Name = "cbStatus";
		this.cbStatus.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbStatus.Properties.Items.AddRange(new object[3] { "Draft", "Request", "Approved" });
		this.cbStatus.SettingInVisible = false;
		this.cbStatus.SettingReadonly = false;
		this.cbStatus.Size = new System.Drawing.Size(121, 20);
		this.cbStatus.TabIndex = 37;
		this.btnDivision.EditControl = null;
		this.btnDivision.FieldName = null;
		this.btnDivision.IsSearchControl = false;
		this.btnDivision.Location = new System.Drawing.Point(446, 50);
		this.btnDivision.Name = "btnDivision";
		this.btnDivision.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.btnDivision.SettingInVisible = false;
		this.btnDivision.SettingReadonly = false;
		this.btnDivision.Size = new System.Drawing.Size(128, 20);
		this.btnDivision.TabIndex = 39;
		this.gridQuickAnalysis.AllowPasteExcel = true;
		this.gridQuickAnalysis.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.gridQuickAnalysis.ClipboardData = null;
		this.gridQuickAnalysis.Location = new System.Drawing.Point(-3, 79);
		this.gridQuickAnalysis.MainView = this.gvQuickAnalysis;
		this.gridQuickAnalysis.Name = "gridQuickAnalysis";
		this.gridQuickAnalysis.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[4] { this.rILSalesEmp, this.rILStaSalesEmpl, this.repYear, this.repStatus });
		this.gridQuickAnalysis.Size = new System.Drawing.Size(970, 307);
		this.gridQuickAnalysis.TabIndex = 45;
		this.gridQuickAnalysis.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvQuickAnalysis });
		this.gvQuickAnalysis.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvQuickAnalysis.ColumnKey = null;
		this.gvQuickAnalysis.DisableFormSetting = false;
		this.gvQuickAnalysis.DisplayException = false;
		this.gvQuickAnalysis.EditColumn = null;
		this.gvQuickAnalysis.FieldCurrency = null;
		this.gvQuickAnalysis.FieldRate = null;
		this.gvQuickAnalysis.FormId = null;
		this.gvQuickAnalysis.GridControl = this.gridQuickAnalysis;
		this.gvQuickAnalysis.IgnoreFilter = false;
		this.gvQuickAnalysis.IndexEditRow = -1;
		this.gvQuickAnalysis.IndicatorWidth = 30;
		this.gvQuickAnalysis.IsApzTable = false;
		this.gvQuickAnalysis.IsInsertNewRow = false;
		this.gvQuickAnalysis.IsLoadingData = false;
		this.gvQuickAnalysis.IsMappingContextAction = false;
		this.gvQuickAnalysis.ItemId = null;
		this.gvQuickAnalysis.LstColumnAllowBatch = null;
		this.gvQuickAnalysis.LstColumnNotAllowBatch = null;
		this.gvQuickAnalysis.LstColumnNotEditable = null;
		this.gvQuickAnalysis.LstColumNotSetting = null;
		this.gvQuickAnalysis.Name = "gvQuickAnalysis";
		this.gvQuickAnalysis.OptionsView.ColumnAutoWidth = false;
		this.gvQuickAnalysis.UseBatchMod = false;
		this.gvQuickAnalysis.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gvQuickAnalysis_CustomDrawRowIndicator);
		this.rILSalesEmp.AutoHeight = false;
		this.rILSalesEmp.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rILSalesEmp.DisplayMember = "SlpName";
		this.rILSalesEmp.Name = "rILSalesEmp";
		this.rILSalesEmp.ValueMember = "SlpCode";
		this.rILStaSalesEmpl.AutoHeight = false;
		this.rILStaSalesEmpl.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rILStaSalesEmpl.DisplayMember = "SlpName";
		this.rILStaSalesEmpl.Name = "rILStaSalesEmpl";
		this.rILStaSalesEmpl.ValueMember = "SlpCode";
		this.repYear.AutoHeight = false;
		this.repYear.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repYear.DisplayMember = "Year";
		this.repYear.Name = "repYear";
		this.repYear.ValueMember = "Year";
		this.repStatus.AutoHeight = false;
		this.repStatus.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repStatus.Items.AddRange(new object[3] { "Draft", "Request", "Approved" });
		this.repStatus.Name = "repStatus";
		this.dtYear.EditControl = null;
		this.dtYear.FieldName = null;
		this.dtYear.IsSearchControl = false;
		this.dtYear.Location = new System.Drawing.Point(644, 23);
		this.dtYear.Name = "dtYear";
		this.dtYear.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dtYear.Properties.PopupSizeable = true;
		this.dtYear.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.dtYear.SettingInVisible = false;
		this.dtYear.SettingReadonly = false;
		this.dtYear.Size = new System.Drawing.Size(121, 20);
		this.dtYear.TabIndex = 29;
		this.cbEmployee.Location = new System.Drawing.Point(255, 53);
		this.cbEmployee.Name = "cbEmployee";
		this.cbEmployee.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbEmployee.Properties.DisplayMember = "SlpName";
		this.cbEmployee.Properties.NullText = "";
		this.cbEmployee.Properties.PopupSizeable = false;
		this.cbEmployee.Properties.PopupView = this.gridLookUpEdit1View;
		this.cbEmployee.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
		this.cbEmployee.Properties.ValueMember = "SlpCode";
		this.cbEmployee.Size = new System.Drawing.Size(111, 20);
		this.cbEmployee.TabIndex = 38;
		this.cbEmployee.EditValueChanged += new System.EventHandler(this.cbEmployee_EditValueChanged);
		this.gridLookUpEdit1View.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gridLookUpEdit1View.ColumnKey = null;
		this.gridLookUpEdit1View.DisableFormSetting = false;
		this.gridLookUpEdit1View.DisplayException = false;
		this.gridLookUpEdit1View.EditColumn = null;
		this.gridLookUpEdit1View.FieldCurrency = null;
		this.gridLookUpEdit1View.FieldRate = null;
		this.gridLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
		this.gridLookUpEdit1View.FormId = null;
		this.gridLookUpEdit1View.IgnoreFilter = false;
		this.gridLookUpEdit1View.IndexEditRow = -1;
		this.gridLookUpEdit1View.IndicatorWidth = 35;
		this.gridLookUpEdit1View.IsApzTable = false;
		this.gridLookUpEdit1View.IsInsertNewRow = false;
		this.gridLookUpEdit1View.IsLoadingData = false;
		this.gridLookUpEdit1View.IsMappingContextAction = false;
		this.gridLookUpEdit1View.ItemId = null;
		this.gridLookUpEdit1View.LstColumnAllowBatch = null;
		this.gridLookUpEdit1View.LstColumnNotAllowBatch = null;
		this.gridLookUpEdit1View.LstColumnNotEditable = null;
		this.gridLookUpEdit1View.LstColumNotSetting = null;
		this.gridLookUpEdit1View.Name = "gridLookUpEdit1View";
		this.gridLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.gridLookUpEdit1View.OptionsView.ShowGroupPanel = false;
		this.gridLookUpEdit1View.UseBatchMod = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(967, 386);
		base.Controls.Add(this.gridQuickAnalysis);
		base.Controls.Add(this.btnSearch);
		base.Controls.Add(this.labelLine2);
		base.Controls.Add(this.labelLine6);
		base.Controls.Add(this.labelLine7);
		base.Controls.Add(this.labelLine8);
		base.Controls.Add(this.txtBPName);
		base.Controls.Add(this.labelLine5);
		base.Controls.Add(this.labelLine4);
		base.Controls.Add(this.labelLine3);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.btnBPCode);
		base.Controls.Add(this.cbType);
		base.Controls.Add(this.btnWareHouse);
		base.Controls.Add(this.cbStatus);
		base.Controls.Add(this.btnDivision);
		base.Controls.Add(this.dtYear);
		base.Controls.Add(this.cbEmployee);
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_SalesForeCastQuickAnalys";
		base.ShowInTaskbar = false;
		this.Text = "Sales ForeCast Quick Analysis";
		base.Load += new System.EventHandler(this.SalesForeCastQuickAnalys_Load);
		((System.ComponentModel.ISupportInitialize)this.txtBPName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btnBPCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbType.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btnWareHouse.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbStatus.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btnDivision.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridQuickAnalysis).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gvQuickAnalysis).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rILSalesEmp).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rILStaSalesEmpl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repYear).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repStatus).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dtYear.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbEmployee.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridLookUpEdit1View).EndInit();
		base.ResumeLayout(false);
	}
}
