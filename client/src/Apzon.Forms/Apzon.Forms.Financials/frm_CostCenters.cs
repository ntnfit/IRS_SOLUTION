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
using Apzon.Forms.Commons;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Financials;

public class frm_CostCenters : XtraFormEx
{
	private bool _isLoading;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private string _privilege = "";

	private List<int> _lstRowChangedIdx = new List<int>();

	private IContainer components;

	private LabelLine lb_Code;

	private LabelLine labelLine1;

	private LabelLine labelLine2;

	private LabelLine labelLine3;

	private TextEditEx txt_OPRC_PrcCode;

	private TextEditEx txt_OPRC_PrcName;

	private LookUpEdit luk_OPRC_DimCode;

	private LookUpEdit luk_OPRC_CCTypeCode;

	private LabelLine labelLine4;

	private TextEditEx txt_OPRC_GrpCode;

	private LabelLine labelLine5;

	private LabelLine labelLine6;

	private DateEditEx date_ValidFrom;

	private DateEditEx date_ValidTo;

	private CheckEditEx chk_Active;

	private SimpleButtonEx btn_Add;

	private SimpleButtonEx btn_Cancel;

	private SimpleButtonEx btn_Search;

	public GridControlEx grd_Doc;

	public GridViewEx gv_Doc;

	public GridViewEx gridView2;

	private GridColumnEx gridColumn1;

	private GridColumnEx gridColumn2;

	private GridColumnEx gridColumn3;

	private GridColumnEx gridColumn4;

	private RepositoryItemLookUpEdit rep_luk_DimCode;

	private GridColumnEx gridColumn5;

	private RepositoryItemLookUpEdit rep_luk_CCTypeCode;

	private GridColumnEx gridColumn6;

	private RepositoryItemDateEdit rep_Date;

	private GridColumnEx gridColumn7;

	private GridColumnEx gridColumn8;

	private RepositoryItemCheckEdit rep_chk_Active;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem removeToolStripMenuItem;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
		{
			WinFuntions.DisableAllControl(Controls);
			btn_Cancel.Enabled = true;
			return;
		}
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Add:
			btn_Add.Text = Resources.ButtonAdd;
			break;
		case Enums.FormViewModeType.Update:
			btn_Add.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btn_Add.Text = Resources.ButtonOK;
			break;
		}
	}

	private void frm_CostCenters_Load(object sender, EventArgs e)
	{
		try
		{
			SetFormatControlValue();
			chk_Active.EditValue = "Y";
			LoadComboBox();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadComboBox()
	{
		try
		{
			DataSet dataSet = _httpClient.CostCentersClient.LoadComboBox();
			if (dataSet != null)
			{
				if (dataSet.Tables.Count > 0)
				{
					luk_OPRC_DimCode.Properties.DataSource = dataSet.Tables[0];
					rep_luk_DimCode.DataSource = dataSet.Tables[0];
				}
				if (dataSet.Tables.Count > 1)
				{
					luk_OPRC_CCTypeCode.Properties.DataSource = dataSet.Tables[1];
					rep_luk_CCTypeCode.DataSource = dataSet.Tables[1];
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Doc_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == "PrcCode" && FormMode != Enums.FormViewModeType.Add)
			{
				e.Appearance.BackColor = Color.FromArgb(231, 231, 231);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Doc_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (gv_Doc.FocusedColumn.FieldName == "PrcCode" && FormMode != Enums.FormViewModeType.Add)
			{
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void removeToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Add && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmRemoveCostCenter, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				string prcCode = Function.ToString(gv_Doc.GetFocusedRowCellValue("PrcCode"));
				string text = _httpClient.CostCentersClient.DeleteCostCenters(prcCode);
				if (string.IsNullOrEmpty(text))
				{
					WinCommons.Success(Enums.MessageCode.RemoveSuccess, "");
					FormMode = Enums.FormViewModeType.View;
					Search();
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, text);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_Add_Click(object sender, EventArgs e)
	{
		EnableDisableControls(p: false);
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
			}
			else if (ValidateData() && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				DataTable dataTable = grd_Doc.DataSource as DataTable;
				DataTable dataTable2 = dataTable.Clone();
				foreach (int item in _lstRowChangedIdx)
				{
					DataRow dataRow = dataTable.Rows[item];
					dataRow["Locked"] = "N";
					string text = Function.ToString(dataRow["Active"]);
					if (text != "Y" && text != "N")
					{
						dataRow["Active"] = "N";
					}
					DateTime? dateTime = Function.ParseDateTime(dataRow["ValidFrom"]);
					if (dateTime.HasValue)
					{
						_ = dateTime.Value;
					}
					else
					{
						dataRow["ValidFrom"] = DateTime.Today;
					}
					if (FormMode == Enums.FormViewModeType.Add)
					{
						dataRow["UserSign"] = GlobalData.UserID;
					}
					else
					{
						dataRow["UserSign2"] = GlobalData.UserID;
					}
					dataRow["Locked"] = "N";
					dataTable2.ImportRow(dataRow);
				}
				dataTable2.AcceptChanges();
				_lstRowChangedIdx = new List<int>();
				string text2 = _httpClient.CostCentersClient.Save(dataTable2);
				if (string.IsNullOrEmpty(text2))
				{
					WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
					FormMode = Enums.FormViewModeType.View;
					Search();
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, text2);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		EnableDisableControls(p: true);
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void btn_OpenTable_Click(object sender, EventArgs e)
	{
	}

	private void btn_Search_Click(object sender, EventArgs e)
	{
		try
		{
			if (ShowChangedConfirm())
			{
				FormMode = Enums.FormViewModeType.View;
				Search();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public frm_CostCenters()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_CostCenters"))
		{
			InitializeComponent();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_CostCenters" && x.authoriza == "F"))
			{
				WinFuntions.DisableAllControl(Controls);
				_privilege = "R";
			}
			else
			{
				_privilege = "F";
			}
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	private bool ValidateData()
	{
		try
		{
			if (!(grd_Doc.DataSource is DataTable dataTable) || dataTable.Rows.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.NoDataToSave, "");
				return false;
			}
			DataRow dataRow = dataTable.Rows[0];
			if (_httpClient.CostCentersClient.IsExistsCostCenters(Function.ToString(dataRow["PrcCode"])) && FormMode == Enums.FormViewModeType.Add)
			{
				WinCommons.Error(Enums.MessageCode.CostCenterAlreadyExists, "");
				return false;
			}
			if (string.IsNullOrWhiteSpace(Function.ToString(dataRow["PrcCode"])))
			{
				WinCommons.Error(Enums.MessageCode.EnterCostCenterField, "");
				return false;
			}
			DateTime? dateTime = Function.ParseDateTime(dataRow["ValidFrom"]);
			DateTime? dateTime2 = Function.ParseDateTime(dataRow["ValidTo"]);
			if (dateTime.HasValue)
			{
				_ = dateTime.Value;
				if (dateTime2.HasValue)
				{
					_ = dateTime2.Value;
					if (dateTime.Value.CompareTo(dateTime2.Value) > 0)
					{
						WinCommons.Error(Enums.MessageCode.SpecifyEffectiveToDateThatIsSameOrLaterThanEffectiveFrom, "");
						return false;
					}
				}
			}
			return true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return false;
		}
	}

	private void EnableDisableControls(bool p)
	{
		try
		{
			btn_Add.Enabled = p;
			btn_Cancel.Enabled = p;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private bool ShowChangedConfirm()
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update)
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					return true;
				}
				return false;
			}
			return true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return false;
		}
	}

	private void gv_Doc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			DataTable dataTable = grd_Doc.DataSource as DataTable;
			int dataSourceRowIndex = gv_Doc.GetDataSourceRowIndex(e.RowHandle);
			if (dataSourceRowIndex > -1 && !_lstRowChangedIdx.Contains(dataSourceRowIndex))
			{
				_lstRowChangedIdx.Add(dataSourceRowIndex);
			}
			if (!_isLoading && FormMode == Enums.FormViewModeType.View && dataTable != null && dataTable.Rows.Count > 0)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void Search()
	{
		try
		{
			_lstRowChangedIdx.Clear();
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("PrcCode", typeof(string));
			dataTable.Columns.Add("PrcName", typeof(string));
			dataTable.Columns.Add("GrpCode", typeof(string));
			dataTable.Columns.Add("DimCode", typeof(string));
			dataTable.Columns.Add("CCTypeCode", typeof(string));
			dataTable.Columns.Add("ValidFrom", typeof(string));
			dataTable.Columns.Add("ValidTo", typeof(string));
			dataTable.Columns.Add("Active", typeof(string));
			DataRow dataRow = dataTable.NewRow();
			dataRow["PrcCode"] = Function.ToString(txt_OPRC_PrcCode.Text).Trim();
			dataRow["PrcName"] = Function.ToString(txt_OPRC_PrcName.Text).Trim();
			dataRow["GrpCode"] = Function.ToString(txt_OPRC_GrpCode.Text).Trim();
			dataRow["DimCode"] = Function.ParseInt(luk_OPRC_DimCode.EditValue);
			dataRow["CCTypeCode"] = Function.ToString(luk_OPRC_CCTypeCode.EditValue);
			DateTime? dateTime = Function.ParseDateTime(date_ValidFrom.EditValue);
			string value;
			if (dateTime.HasValue)
			{
				_ = dateTime.Value;
				value = dateTime.Value.ToString("yyyyMMdd");
			}
			else
			{
				value = "19000101";
			}
			dataRow["ValidFrom"] = value;
			DateTime? dateTime2 = Function.ParseDateTime(date_ValidTo.EditValue);
			string value2;
			if (dateTime2.HasValue)
			{
				_ = dateTime2.Value;
				value2 = dateTime2.Value.ToString("yyyyMMdd");
			}
			else
			{
				value2 = "19000101";
			}
			dataRow["ValidTo"] = value2;
			dataRow["Active"] = ((Function.ToString(chk_Active.EditValue) == "Y") ? "Y" : "N");
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			DataTable dataSource = _httpClient.CostCentersClient.Search(dataTable);
			grd_Doc.DataSource = dataSource;
			grd_Doc.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void SetFormatControlValue()
	{
		try
		{
			rep_Date.Mask.MaskType = MaskType.DateTime;
			rep_Date.Mask.EditMask = GlobalData.DateTimeFormat;
			rep_Date.Mask.UseMaskAsDisplayFormat = true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task REFRESH_DATA()
	{
		FormMode = Enums.FormViewModeType.View;
		Search();
	}

	public async Task REFRESH()
	{
		try
		{
			_lstRowChangedIdx.Clear();
			FormMode = Enums.FormViewModeType.Add;
			DataTable tableStructure = WinFuntions.GetTableStructure("OPRC");
			DataRow dataRow = tableStructure.NewRow();
			dataRow["ValidFrom"] = DateTime.Today;
			dataRow["DimCode"] = 1;
			dataRow["Active"] = "Y";
			tableStructure.Rows.Add(dataRow);
			tableStructure.AcceptChanges();
			grd_Doc.DataSource = tableStructure;
			grd_Doc.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Doc_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
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
		this.components = new System.ComponentModel.Container();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Financials.frm_CostCenters));
		this.lb_Code = new Apzon.Controls.Controls.LabelLine();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.txt_OPRC_PrcCode = new Apzon.Controls.Editor.TextEditEx();
		this.txt_OPRC_PrcName = new Apzon.Controls.Editor.TextEditEx();
		this.luk_OPRC_DimCode = new DevExpress.XtraEditors.LookUpEdit();
		this.luk_OPRC_CCTypeCode = new DevExpress.XtraEditors.LookUpEdit();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.txt_OPRC_GrpCode = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine5 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine6 = new Apzon.Controls.Controls.LabelLine();
		this.date_ValidFrom = new Apzon.Controls.Editor.DateEditEx();
		this.date_ValidTo = new Apzon.Controls.Editor.DateEditEx();
		this.chk_Active = new Apzon.Controls.Editor.CheckEditEx();
		this.btn_Add = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Search = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn4 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_luk_DimCode = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumn5 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_luk_CCTypeCode = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumn6 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Date = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.gridColumn7 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn8 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_chk_Active = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gridView2 = new Apzon.Controls.Grid.GridViewEx();
		((System.ComponentModel.ISupportInitialize)this.txt_OPRC_PrcCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_OPRC_PrcName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_OPRC_DimCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_OPRC_CCTypeCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_OPRC_GrpCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_ValidFrom.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_ValidFrom.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_ValidTo.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_ValidTo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Active.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_luk_DimCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_luk_CCTypeCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Date).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Date.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_chk_Active).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridView2).BeginInit();
		base.SuspendLayout();
		this.lb_Code.EditControl = null;
		this.lb_Code.EditValue = "Cost Center";
		this.lb_Code.FieldName = null;
		this.lb_Code.HideWhenEmpty = false;
		this.lb_Code.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Code.Location = new System.Drawing.Point(7, 6);
		this.lb_Code.Name = "lb_Code";
		this.lb_Code.ReadOnly = false;
		this.lb_Code.SettingInVisible = false;
		this.lb_Code.SettingReadonly = false;
		this.lb_Code.ShowUnderLine = true;
		this.lb_Code.Size = new System.Drawing.Size(85, 20);
		this.lb_Code.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Code.TabIndex = 5;
		this.lb_Code.Utext = "Cost Center";
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Name";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(244, 6);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(100, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 6;
		this.labelLine1.Utext = "Name";
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "Sort Code";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(531, 6);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(77, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 7;
		this.labelLine2.Utext = "Sort Code";
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "Dimension";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(7, 32);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(85, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 8;
		this.labelLine3.Utext = "Dimension";
		this.txt_OPRC_PrcCode.EditControl = null;
		this.txt_OPRC_PrcCode.FieldName = null;
		this.txt_OPRC_PrcCode.IsSearchControl = false;
		this.txt_OPRC_PrcCode.Location = new System.Drawing.Point(91, 3);
		this.txt_OPRC_PrcCode.Name = "txt_OPRC_PrcCode";
		this.txt_OPRC_PrcCode.SettingInVisible = false;
		this.txt_OPRC_PrcCode.SettingReadonly = false;
		this.txt_OPRC_PrcCode.Size = new System.Drawing.Size(118, 20);
		this.txt_OPRC_PrcCode.TabIndex = 9;
		this.txt_OPRC_PrcName.EditControl = null;
		this.txt_OPRC_PrcName.FieldName = null;
		this.txt_OPRC_PrcName.IsSearchControl = false;
		this.txt_OPRC_PrcName.Location = new System.Drawing.Point(340, 3);
		this.txt_OPRC_PrcName.Name = "txt_OPRC_PrcName";
		this.txt_OPRC_PrcName.SettingInVisible = false;
		this.txt_OPRC_PrcName.SettingReadonly = false;
		this.txt_OPRC_PrcName.Size = new System.Drawing.Size(159, 20);
		this.txt_OPRC_PrcName.TabIndex = 10;
		this.luk_OPRC_DimCode.Location = new System.Drawing.Point(91, 29);
		this.luk_OPRC_DimCode.Name = "luk_OPRC_DimCode";
		this.luk_OPRC_DimCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_OPRC_DimCode.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.luk_OPRC_DimCode.Properties.DisplayMember = "Name";
		this.luk_OPRC_DimCode.Properties.NullText = "";
		this.luk_OPRC_DimCode.Properties.ValueMember = "Code";
		this.luk_OPRC_DimCode.Size = new System.Drawing.Size(118, 20);
		this.luk_OPRC_DimCode.TabIndex = 11;
		this.luk_OPRC_CCTypeCode.Location = new System.Drawing.Point(340, 29);
		this.luk_OPRC_CCTypeCode.Name = "luk_OPRC_CCTypeCode";
		this.luk_OPRC_CCTypeCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_OPRC_CCTypeCode.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.luk_OPRC_CCTypeCode.Properties.DisplayMember = "Name";
		this.luk_OPRC_CCTypeCode.Properties.NullText = "";
		this.luk_OPRC_CCTypeCode.Properties.ValueMember = "Code";
		this.luk_OPRC_CCTypeCode.Size = new System.Drawing.Size(106, 20);
		this.luk_OPRC_CCTypeCode.TabIndex = 13;
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "Cost Center Type";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(244, 32);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(104, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 12;
		this.labelLine4.Utext = "Cost Center Type";
		this.txt_OPRC_GrpCode.EditControl = null;
		this.txt_OPRC_GrpCode.FieldName = null;
		this.txt_OPRC_GrpCode.IsSearchControl = false;
		this.txt_OPRC_GrpCode.Location = new System.Drawing.Point(600, 3);
		this.txt_OPRC_GrpCode.Name = "txt_OPRC_GrpCode";
		this.txt_OPRC_GrpCode.SettingInVisible = false;
		this.txt_OPRC_GrpCode.SettingReadonly = false;
		this.txt_OPRC_GrpCode.Size = new System.Drawing.Size(76, 20);
		this.txt_OPRC_GrpCode.TabIndex = 14;
		this.labelLine5.EditControl = null;
		this.labelLine5.EditValue = "Effective From";
		this.labelLine5.FieldName = null;
		this.labelLine5.HideWhenEmpty = false;
		this.labelLine5.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine5.Location = new System.Drawing.Point(7, 59);
		this.labelLine5.Name = "labelLine5";
		this.labelLine5.ReadOnly = false;
		this.labelLine5.SettingInVisible = false;
		this.labelLine5.SettingReadonly = false;
		this.labelLine5.ShowUnderLine = true;
		this.labelLine5.Size = new System.Drawing.Size(85, 20);
		this.labelLine5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine5.TabIndex = 15;
		this.labelLine5.Utext = "Effective From";
		this.labelLine6.EditControl = null;
		this.labelLine6.EditValue = "To";
		this.labelLine6.FieldName = null;
		this.labelLine6.HideWhenEmpty = false;
		this.labelLine6.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine6.Location = new System.Drawing.Point(244, 57);
		this.labelLine6.Name = "labelLine6";
		this.labelLine6.ReadOnly = false;
		this.labelLine6.SettingInVisible = false;
		this.labelLine6.SettingReadonly = false;
		this.labelLine6.ShowUnderLine = true;
		this.labelLine6.Size = new System.Drawing.Size(100, 20);
		this.labelLine6.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine6.TabIndex = 16;
		this.labelLine6.Utext = "To";
		this.date_ValidFrom.EditControl = null;
		this.date_ValidFrom.EditValue = null;
		this.date_ValidFrom.FieldName = null;
		this.date_ValidFrom.Location = new System.Drawing.Point(91, 56);
		this.date_ValidFrom.Name = "date_ValidFrom";
		this.date_ValidFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_ValidFrom.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_ValidFrom.Properties.IsSearchControl = false;
		this.date_ValidFrom.SettingInVisible = false;
		this.date_ValidFrom.SettingReadonly = false;
		this.date_ValidFrom.Size = new System.Drawing.Size(118, 20);
		this.date_ValidFrom.TabIndex = 17;
		this.date_ValidTo.EditControl = null;
		this.date_ValidTo.EditValue = null;
		this.date_ValidTo.FieldName = null;
		this.date_ValidTo.Location = new System.Drawing.Point(340, 55);
		this.date_ValidTo.Name = "date_ValidTo";
		this.date_ValidTo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_ValidTo.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_ValidTo.Properties.IsSearchControl = false;
		this.date_ValidTo.SettingInVisible = false;
		this.date_ValidTo.SettingReadonly = false;
		this.date_ValidTo.Size = new System.Drawing.Size(106, 20);
		this.date_ValidTo.TabIndex = 18;
		this.chk_Active.EditControl = null;
		this.chk_Active.FieldName = null;
		this.chk_Active.IsSearchControl = false;
		this.chk_Active.Location = new System.Drawing.Point(533, 30);
		this.chk_Active.Name = "chk_Active";
		this.chk_Active.Properties.Caption = "&Active";
		this.chk_Active.Properties.DisplayValueChecked = "Y";
		this.chk_Active.Properties.DisplayValueGrayed = "N";
		this.chk_Active.Properties.DisplayValueUnchecked = "N";
		this.chk_Active.Properties.ValueChecked = "Y";
		this.chk_Active.Properties.ValueGrayed = "N";
		this.chk_Active.Properties.ValueUnchecked = "N";
		this.chk_Active.SettingInVisible = false;
		this.chk_Active.SettingReadonly = false;
		this.chk_Active.Size = new System.Drawing.Size(75, 20);
		this.chk_Active.TabIndex = 20;
		this.btn_Add.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Add.EditControl = null;
		this.btn_Add.EditValue = "OK";
		this.btn_Add.FieldName = null;
		this.btn_Add.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Add.ImageOptions.Image");
		this.btn_Add.IsSearchControl = false;
		this.btn_Add.Location = new System.Drawing.Point(7, 390);
		this.btn_Add.Name = "btn_Add";
		this.btn_Add.ReadOnly = false;
		this.btn_Add.SettingInVisible = false;
		this.btn_Add.SettingReadonly = false;
		this.btn_Add.Size = new System.Drawing.Size(75, 23);
		this.btn_Add.TabIndex = 21;
		this.btn_Add.Text = "OK";
		this.btn_Add.Click += new System.EventHandler(this.btn_Add_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(85, 390);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 22;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.btn_Search.EditControl = null;
		this.btn_Search.EditValue = "Search";
		this.btn_Search.FieldName = null;
		this.btn_Search.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Search.ImageOptions.Image");
		this.btn_Search.IsSearchControl = false;
		this.btn_Search.Location = new System.Drawing.Point(533, 52);
		this.btn_Search.Name = "btn_Search";
		this.btn_Search.ReadOnly = false;
		this.btn_Search.SettingInVisible = false;
		this.btn_Search.SettingReadonly = false;
		this.btn_Search.Size = new System.Drawing.Size(90, 23);
		this.btn_Search.TabIndex = 24;
		this.btn_Search.Text = "Search";
		this.btn_Search.Click += new System.EventHandler(this.btn_Search_Click);
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_Doc.ImeMode = System.Windows.Forms.ImeMode.On;
		this.grd_Doc.Location = new System.Drawing.Point(7, 84);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[4] { this.rep_Date, this.rep_chk_Active, this.rep_luk_DimCode, this.rep_luk_CCTypeCode });
		this.grd_Doc.Size = new System.Drawing.Size(676, 299);
		this.grd_Doc.TabIndex = 25;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[2] { this.gv_Doc, this.gridView2 });
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.removeToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(118, 26);
		this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
		this.removeToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
		this.removeToolStripMenuItem.Text = "Remove";
		this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = null;
		this.gv_Doc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[8] { this.gridColumn1, this.gridColumn2, this.gridColumn3, this.gridColumn4, this.gridColumn5, this.gridColumn6, this.gridColumn7, this.gridColumn8 });
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = "oCostCenter";
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.IgnoreFilter = false;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 35;
		this.gv_Doc.IsApzTable = false;
		this.gv_Doc.IsInsertNewRow = false;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.IsMappingContextAction = false;
		this.gv_Doc.ItemId = "OPRC";
		this.gv_Doc.LstColumnAllowBatch = null;
		this.gv_Doc.LstColumnNotAllowBatch = null;
		this.gv_Doc.LstColumnNotEditable = null;
		this.gv_Doc.LstColumNotSetting = null;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsDetail.EnableMasterViewMode = false;
		this.gv_Doc.OptionsSelection.MultiSelect = true;
		this.gv_Doc.OptionsView.ColumnAutoWidth = false;
		this.gv_Doc.OptionsView.ShowGroupPanel = false;
		this.gv_Doc.UseBatchMod = false;
		this.gv_Doc.ViewCaption = "Header";
		this.gv_Doc.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Doc_RowCellStyle);
		this.gv_Doc.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_Doc_ShowingEditor);
		this.gv_Doc.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gv_Doc_FocusedRowChanged);
		this.gv_Doc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Doc_CellValueChanged);
		this.gridColumn1.Caption = "Cost Center";
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "PrcCode";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.PrimaryKey = true;
		this.gridColumn1.UnitField = null;
		this.gridColumn1.Visible = true;
		this.gridColumn1.VisibleIndex = 0;
		this.gridColumn2.Caption = "Name";
		this.gridColumn2.CurrencyDateField = null;
		this.gridColumn2.CurrencyField = null;
		this.gridColumn2.CurrencyFixed = null;
		this.gridColumn2.CurrencyFixedRate = 0.0;
		this.gridColumn2.CurrencyRateField = null;
		this.gridColumn2.FieldName = "PrcName";
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.PrimaryKey = false;
		this.gridColumn2.UnitField = null;
		this.gridColumn2.Visible = true;
		this.gridColumn2.VisibleIndex = 1;
		this.gridColumn3.Caption = "Sort Code";
		this.gridColumn3.CurrencyDateField = null;
		this.gridColumn3.CurrencyField = null;
		this.gridColumn3.CurrencyFixed = null;
		this.gridColumn3.CurrencyFixedRate = 0.0;
		this.gridColumn3.CurrencyRateField = null;
		this.gridColumn3.FieldName = "GrpCode";
		this.gridColumn3.Name = "gridColumn3";
		this.gridColumn3.PrimaryKey = false;
		this.gridColumn3.UnitField = null;
		this.gridColumn3.Visible = true;
		this.gridColumn3.VisibleIndex = 2;
		this.gridColumn4.Caption = "Dimension";
		this.gridColumn4.ColumnEdit = this.rep_luk_DimCode;
		this.gridColumn4.CurrencyDateField = null;
		this.gridColumn4.CurrencyField = null;
		this.gridColumn4.CurrencyFixed = null;
		this.gridColumn4.CurrencyFixedRate = 0.0;
		this.gridColumn4.CurrencyRateField = null;
		this.gridColumn4.FieldName = "DimCode";
		this.gridColumn4.Name = "gridColumn4";
		this.gridColumn4.PrimaryKey = false;
		this.gridColumn4.UnitField = null;
		this.gridColumn4.Visible = true;
		this.gridColumn4.VisibleIndex = 3;
		this.rep_luk_DimCode.AutoHeight = false;
		this.rep_luk_DimCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_luk_DimCode.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.rep_luk_DimCode.DisplayMember = "Name";
		this.rep_luk_DimCode.Name = "rep_luk_DimCode";
		this.rep_luk_DimCode.NullText = "";
		this.rep_luk_DimCode.ValueMember = "Code";
		this.gridColumn5.Caption = "Cost Center Type";
		this.gridColumn5.ColumnEdit = this.rep_luk_CCTypeCode;
		this.gridColumn5.CurrencyDateField = null;
		this.gridColumn5.CurrencyField = null;
		this.gridColumn5.CurrencyFixed = null;
		this.gridColumn5.CurrencyFixedRate = 0.0;
		this.gridColumn5.CurrencyRateField = null;
		this.gridColumn5.FieldName = "CCTypeCode";
		this.gridColumn5.Name = "gridColumn5";
		this.gridColumn5.PrimaryKey = false;
		this.gridColumn5.UnitField = null;
		this.gridColumn5.Visible = true;
		this.gridColumn5.VisibleIndex = 4;
		this.rep_luk_CCTypeCode.AutoHeight = false;
		this.rep_luk_CCTypeCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_luk_CCTypeCode.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.rep_luk_CCTypeCode.DisplayMember = "Name";
		this.rep_luk_CCTypeCode.Name = "rep_luk_CCTypeCode";
		this.rep_luk_CCTypeCode.NullText = "";
		this.rep_luk_CCTypeCode.ValueMember = "Code";
		this.gridColumn6.Caption = "Effective From";
		this.gridColumn6.ColumnEdit = this.rep_Date;
		this.gridColumn6.CurrencyDateField = null;
		this.gridColumn6.CurrencyField = null;
		this.gridColumn6.CurrencyFixed = null;
		this.gridColumn6.CurrencyFixedRate = 0.0;
		this.gridColumn6.CurrencyRateField = null;
		this.gridColumn6.FieldName = "ValidFrom";
		this.gridColumn6.Name = "gridColumn6";
		this.gridColumn6.PrimaryKey = false;
		this.gridColumn6.UnitField = null;
		this.gridColumn6.Visible = true;
		this.gridColumn6.VisibleIndex = 5;
		this.rep_Date.AutoHeight = false;
		this.rep_Date.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Date.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Date.Name = "rep_Date";
		this.gridColumn7.Caption = "To";
		this.gridColumn7.ColumnEdit = this.rep_Date;
		this.gridColumn7.CurrencyDateField = null;
		this.gridColumn7.CurrencyField = null;
		this.gridColumn7.CurrencyFixed = null;
		this.gridColumn7.CurrencyFixedRate = 0.0;
		this.gridColumn7.CurrencyRateField = null;
		this.gridColumn7.FieldName = "ValidTo";
		this.gridColumn7.Name = "gridColumn7";
		this.gridColumn7.PrimaryKey = false;
		this.gridColumn7.UnitField = null;
		this.gridColumn7.Visible = true;
		this.gridColumn7.VisibleIndex = 6;
		this.gridColumn8.Caption = "Active";
		this.gridColumn8.ColumnEdit = this.rep_chk_Active;
		this.gridColumn8.CurrencyDateField = null;
		this.gridColumn8.CurrencyField = null;
		this.gridColumn8.CurrencyFixed = null;
		this.gridColumn8.CurrencyFixedRate = 0.0;
		this.gridColumn8.CurrencyRateField = null;
		this.gridColumn8.FieldName = "Active";
		this.gridColumn8.Name = "gridColumn8";
		this.gridColumn8.PrimaryKey = false;
		this.gridColumn8.UnitField = null;
		this.gridColumn8.Visible = true;
		this.gridColumn8.VisibleIndex = 7;
		this.rep_chk_Active.AutoHeight = false;
		this.rep_chk_Active.DisplayValueChecked = "Y";
		this.rep_chk_Active.DisplayValueUnchecked = "N";
		this.rep_chk_Active.Name = "rep_chk_Active";
		this.rep_chk_Active.ValueChecked = "Y";
		this.rep_chk_Active.ValueGrayed = "";
		this.rep_chk_Active.ValueUnchecked = "N";
		this.gridView2.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gridView2.ColumnKey = null;
		this.gridView2.DisableFormSetting = false;
		this.gridView2.DisplayException = false;
		this.gridView2.EditColumn = null;
		this.gridView2.FieldCurrency = null;
		this.gridView2.FieldRate = null;
		this.gridView2.FormId = "oCostCenter";
		this.gridView2.GridControl = this.grd_Doc;
		this.gridView2.IgnoreFilter = false;
		this.gridView2.IndexEditRow = -1;
		this.gridView2.IndicatorWidth = 35;
		this.gridView2.IsApzTable = false;
		this.gridView2.IsInsertNewRow = false;
		this.gridView2.IsLoadingData = false;
		this.gridView2.IsMappingContextAction = false;
		this.gridView2.ItemId = "OGRID2";
		this.gridView2.LstColumnAllowBatch = null;
		this.gridView2.LstColumnNotAllowBatch = null;
		this.gridView2.LstColumnNotEditable = null;
		this.gridView2.LstColumNotSetting = null;
		this.gridView2.Name = "gridView2";
		this.gridView2.UseBatchMod = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(686, 419);
		base.Controls.Add(this.grd_Doc);
		base.Controls.Add(this.btn_Search);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_Add);
		base.Controls.Add(this.chk_Active);
		base.Controls.Add(this.date_ValidTo);
		base.Controls.Add(this.date_ValidFrom);
		base.Controls.Add(this.labelLine6);
		base.Controls.Add(this.labelLine5);
		base.Controls.Add(this.txt_OPRC_GrpCode);
		base.Controls.Add(this.luk_OPRC_CCTypeCode);
		base.Controls.Add(this.luk_OPRC_DimCode);
		base.Controls.Add(this.txt_OPRC_PrcName);
		base.Controls.Add(this.txt_OPRC_PrcCode);
		base.Controls.Add(this.labelLine4);
		base.Controls.Add(this.labelLine3);
		base.Controls.Add(this.labelLine2);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.lb_Code);
		base.FormId = "oCostCenter";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_CostCenters";
		this.Text = "Cost Centers - Setup";
		base.Load += new System.EventHandler(this.frm_CostCenters_Load);
		((System.ComponentModel.ISupportInitialize)this.txt_OPRC_PrcCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_OPRC_PrcName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_OPRC_DimCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_OPRC_CCTypeCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_OPRC_GrpCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_ValidFrom.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_ValidFrom.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_ValidTo.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_ValidTo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Active.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_luk_DimCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_luk_CCTypeCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Date.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Date).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_chk_Active).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridView2).EndInit();
		base.ResumeLayout(false);
	}
}
