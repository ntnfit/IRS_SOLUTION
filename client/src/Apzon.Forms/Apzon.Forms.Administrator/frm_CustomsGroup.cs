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
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Helper;
using Apzon.Forms.Properties;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Administrator;

public class frm_CustomsGroup : XtraFormEx
{
	private readonly string _Controller = "CustomsGroup";

	private DataTable _dtSelectRow = new DataTable();

	private bool _isDelete;

	private IContainer components;

	private PanelControl panelControl1;

	private SimpleButtonEx btn_Cancel;

	private SimpleButtonEx btn_OK;

	private GridControlEx grd_CustomGroup;

	private GridViewEx gv_CustomGroup;

	private GridColumnEx gridColumn1;

	private GridColumnEx gridColumn2;

	private GridColumnEx gridColumn3;

	private GridColumnEx gridColumn4;

	private GridColumnEx gridColumn5;

	private GridColumnEx gridColumn6;

	private GridColumnEx gridColumn7;

	private GridColumnEx gridColumn8;

	private GridColumnEx gridColumn9;

	private GridColumnEx gridColumn10;

	private GridColumnEx gridColumn11;

	private GridColumnEx gridColumn12;

	private GridColumnEx gridColumn13;

	private GridColumnEx gridColumn14;

	private GridColumnEx gridColumn15;

	private GridColumnEx gridColumn16;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem removeToolStripMenuItem;

	private RepositoryItemCalcEdit rep_Custom;

	private RepositoryItemCalcEdit rep_Purchase;

	private RepositoryItemCalcEdit rep_Other;

	private RepositoryItemCalcEdit rep_Total;

	private RepositoryItemButtonEdit rep_btnCAA;

	private RepositoryItemButtonEdit rep_btnCEA;

	public frm_CustomsGroup()
	{
		InitializeComponent();
	}

	private async void frm_CustomsGroup_Load(object sender, EventArgs e)
	{
		await LoadData();
	}

	private async Task LoadData()
	{
		_ = 1;
		try
		{
			using (HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, _Controller))
			{
				DataTable dataSource = await hp.GetAsync("LoadDoc");
				grd_CustomGroup.DataSource = dataSource;
				await AddNewRow();
				rep_Custom.Mask.MaskType = MaskType.Numeric;
				rep_Custom.Mask.EditMask = "n" + GlobalData.QuantitiesFormat.NumberDecimalDigits;
				rep_Custom.Mask.UseMaskAsDisplayFormat = true;
				rep_Purchase.Mask.MaskType = MaskType.Numeric;
				rep_Purchase.Mask.EditMask = "n" + GlobalData.QuantitiesFormat.NumberDecimalDigits;
				rep_Purchase.Mask.UseMaskAsDisplayFormat = true;
				rep_Other.Mask.MaskType = MaskType.Numeric;
				rep_Other.Mask.EditMask = "n" + GlobalData.QuantitiesFormat.NumberDecimalDigits;
				rep_Other.Mask.UseMaskAsDisplayFormat = true;
				rep_Total.Mask.MaskType = MaskType.Numeric;
				rep_Total.Mask.EditMask = "n" + GlobalData.QuantitiesFormat.NumberDecimalDigits;
				rep_Total.Mask.UseMaskAsDisplayFormat = true;
			}
			_dtSelectRow = grd_CustomGroup.DataSourceTable.Copy().Clone();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task AddNewRow()
	{
		_ = 1;
		try
		{
			DataTable dt = grd_CustomGroup.DataSource as DataTable;
			DataRow dr = dt.NewRow();
			DataRow dataRow = dr;
			dataRow["CstGrpCode"] = await GetMaxCstGrpCode() + 1;
			dr["CstGrpName"] = string.Empty;
			dr["GroupNum"] = string.Empty;
			dr["Custom"] = DBNull.Value;
			dr["BuyTax"] = DBNull.Value;
			dr["OtherTax"] = DBNull.Value;
			dr["TotalTax"] = DBNull.Value;
			dr["Locked"] = "N";
			dr["DataSource"] = "I";
			dataRow = dr;
			dataRow["UserSign"] = await GetUserSign();
			dr["cstAllcAcc"] = DBNull.Value;
			dr["cstExpAcc"] = DBNull.Value;
			dr["PortAddr"] = DBNull.Value;
			dr["PortState"] = DBNull.Value;
			dr["ExciExpAcc"] = DBNull.Value;
			dr["ExciAlcAcc"] = DBNull.Value;
			dt.Rows.Add(dr);
			dt.AcceptChanges();
			grd_CustomGroup.DataSource = dt;
			grd_CustomGroup.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<int> GetMaxCstGrpCode()
	{
		try
		{
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, _Controller);
			DataTable dataTable = await _helper.GetDataTableAsync("GetMaxCstGrpCode");
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ParseInt(dataTable.Rows[0][0]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return -1;
	}

	private async Task SaveData()
	{
		_ = 3;
		try
		{
			if (_isDelete)
			{
				using (HttpClientHelper<HttpResult> hr_1 = new HttpClientHelper<HttpResult>(Configuration.ApiServer, _Controller))
				{
					WinCommons._OpenProcessing();
					HttpResult httpResult = await hr_1.PostAsync("Remove", _dtSelectRow);
					WinCommons._CloseProcessing();
					if (!string.IsNullOrEmpty(httpResult.Message))
					{
						WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
						return;
					}
				}
				_dtSelectRow.Rows.Clear();
				_dtSelectRow.AcceptChanges();
				_isDelete = false;
			}
			DataTable dataTable = grd_CustomGroup.DataSource as DataTable;
			if (!dataTable.IsNotNull())
			{
				return;
			}
			DataTable dtNew = dataTable.Copy();
			DataRow[] array = (from t in dtNew.AsEnumerable()
				where !string.IsNullOrEmpty(Function.ToString(t["CstGrpName"]))
				select t).ToArray();
			if (array.Length != 0)
			{
				dtNew = array.CopyToDataTable();
			}
			List<IGrouping<string, DataRow>> list = (from i in dtNew.AsEnumerable()
				group i by Function.ToString(i.Field<string>("CstGrpName")).ToUpper() into gr
				where gr.Count() > 1
				select gr).ToList();
			if (list != null && list.Count > 0)
			{
				WinCommons.Error(Enums.MessageCode.ThisEntryAlreadyExistsInFollowingTableCustomerGroup, "");
				return;
			}
			DataTable dtAdd = dtNew.Clone();
			int code = await GetMaxCstGrpCode() + 1;
			foreach (DataRow row in dtNew.Rows)
			{
				DataRow dataRow2 = dtAdd.NewRow();
				if (Function.ParseInt(row["CstGrpCode"]) > 0 || Function.ParseInt(row["CstGrpCode"]) == -1)
				{
					dataRow2["CstGrpCode"] = row["CstGrpCode"];
					dataRow2["CstGrpName"] = row["CstGrpName"];
					dataRow2["GroupNum"] = row["GroupNum"];
					dataRow2["Custom"] = row["Custom"];
					dataRow2["BuyTax"] = row["BuyTax"];
					dataRow2["OtherTax"] = row["OtherTax"];
					dataRow2["TotalTax"] = row["TotalTax"];
					dataRow2["Locked"] = row["Locked"];
					dataRow2["DataSource"] = row["DataSource"];
					dataRow2["UserSign"] = row["UserSign"];
					dataRow2["cstAllcAcc"] = row["cstAllcAcc"];
					dataRow2["cstExpAcc"] = row["cstExpAcc"];
					dataRow2["PortAddr"] = row["PortAddr"];
					dataRow2["PortState"] = row["PortState"];
					dataRow2["ExciExpAcc"] = row["ExciExpAcc"];
					dataRow2["ExciAlcAcc"] = row["ExciAlcAcc"];
				}
				else
				{
					dataRow2["CstGrpCode"] = code;
					dataRow2["CstGrpName"] = row["CstGrpName"];
					dataRow2["DataSource"] = "I";
					dataRow2["GroupNum"] = row["GroupNum"];
					dataRow2["Custom"] = row["Custom"];
					dataRow2["BuyTax"] = row["BuyTax"];
					dataRow2["OtherTax"] = row["OtherTax"];
					dataRow2["TotalTax"] = row["TotalTax"];
					dataRow2["cstAllcAcc"] = row["cstAllcAcc"];
					dataRow2["cstExpAcc"] = row["cstExpAcc"];
					dataRow2["Locked"] = "N";
					DataRow dataRow3 = dataRow2;
					dataRow3["UserSign"] = await GetUserSign();
					code++;
				}
				dtAdd.Rows.Add(dataRow2);
			}
			using HttpClientHelper<HttpResult> hr = new HttpClientHelper<HttpResult>(Configuration.ApiServer, _Controller);
			WinCommons._OpenProcessing();
			HttpResult httpResult2 = hr.Post("SaveOARG", dtAdd);
			WinCommons._CloseProcessing();
			if (httpResult2 != null && string.IsNullOrEmpty(httpResult2.Message))
			{
				await LoadData();
				WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
				btn_OK.Text = Apzon.Commons.Properties.Resources.ButtonOK;
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, httpResult2.Message);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<int> GetUserSign()
	{
		string userCodeLogin = GlobalData.UserCodeLogin;
		try
		{
			using HttpClientHelper<UsersImpl> _helper = new HttpClientHelper<UsersImpl>(Configuration.ApiServer, "Users");
			DataTable dataTable = await _helper.GetDataTableAsync("GetUserSign", new
			{
				nameLogin = userCodeLogin
			});
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ParseInt(dataTable.Rows[0]["UserSign"].ToString());
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return -1;
	}

	private void gv_CustomGroup_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void removeToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			foreach (DataRow row in gv_CustomGroup.GetListDataSourceSelectedRow().CopyToDataTable().Rows)
			{
				if (!string.IsNullOrEmpty(row["CstGrpName"]?.ToString() ?? ""))
				{
					_dtSelectRow.Rows.Add(row.ItemArray);
				}
			}
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, _Controller);
			foreach (DataRow row2 in _dtSelectRow.Rows)
			{
				int code = Function.ParseInt(row2["CstGrpCode"]?.ToString() ?? "");
				if (httpClientHelper.GetDataTable("CheckExistCstGrpCode", new { code }).IsNotNull())
				{
					WinCommons.Error(Enums.MessageCode.CannotDeleteThisRecordIsBeingUsed, "");
					return;
				}
			}
			_isDelete = true;
			gv_CustomGroup.BeginUpdate();
			gv_CustomGroup.DeleteDataSourceRowSelected();
			gv_CustomGroup.EndUpdate();
			btn_OK.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Cancel_Click(object sender, EventArgs e)
	{
		if (btn_OK.Text == Apzon.Commons.Properties.Resources.ButtonUpdate && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCloseForm, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
		{
			await SaveData();
		}
		Close();
	}

	private async void btn_OK_Click(object sender, EventArgs e)
	{
		try
		{
			if (btn_OK.Text == Apzon.Commons.Properties.Resources.ButtonUpdate)
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					await SaveData();
				}
			}
			else
			{
				Close();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_CustomGroup_ShowingEditor(object sender, CancelEventArgs e)
	{
	}

	private void gv_CustomGroup_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (Function.ParseInt((sender as GridView).GetRowCellValue(e.RowHandle, "CstGrpCode")) == -1 && (e.Column.FieldName == "CstGrpName" || e.Column.FieldName == "GroupNum" || e.Column.FieldName == "Custom" || e.Column.FieldName == "BuyTax" || e.Column.FieldName == "OtherTax" || e.Column.FieldName == "TotalTax"))
			{
				e.Appearance.BackColor = Color.LightGray;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void gv_CustomGroup_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (e.RowHandle == gv_CustomGroup.RowCount - 1 && !string.IsNullOrEmpty(Function.ToString(gv_CustomGroup.GetRowCellValue(e.RowHandle, "CstGrpName"))))
			{
				await AddNewRow();
			}
			btn_OK.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task REFRESH_DATA()
	{
		await LoadData();
	}

	private void grd_CustomGroup_EditorKeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.V && e.Modifiers == Keys.Control)
			{
				ExtenseionMethods.PasteFromExcelToDataTable(grd_CustomGroup, gv_CustomGroup, new List<string> { "CstGrpName" });
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions4 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions5 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions6 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled6 = new DevExpress.Utils.SerializableAppearanceObject();
		this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_CustomGroup = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_CustomGroup = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn4 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Custom = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gridColumn5 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Purchase = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gridColumn6 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Other = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gridColumn7 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Total = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gridColumn8 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn9 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn10 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn11 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn12 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn13 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn14 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn15 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn16 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_btnCAA = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.rep_btnCEA = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		((System.ComponentModel.ISupportInitialize)this.panelControl1).BeginInit();
		this.panelControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_CustomGroup).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_CustomGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Custom).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Purchase).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Other).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Total).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_btnCAA).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_btnCEA).BeginInit();
		base.SuspendLayout();
		this.panelControl1.Controls.Add(this.btn_Cancel);
		this.panelControl1.Controls.Add(this.btn_OK);
		this.panelControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.panelControl1.Location = new System.Drawing.Point(0, 346);
		this.panelControl1.Name = "panelControl1";
		this.panelControl1.Size = new System.Drawing.Size(779, 36);
		this.panelControl1.TabIndex = 0;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = Apzon.Forms.Properties.Resources.cancel_16x16;
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(109, 6);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(98, 23);
		this.btn_Cancel.TabIndex = 0;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.btn_OK.EditControl = null;
		this.btn_OK.EditValue = "OK";
		this.btn_OK.FieldName = null;
		this.btn_OK.ImageOptions.Image = Apzon.Forms.Properties.Resources.apply_16x16;
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(5, 6);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.ReadOnly = false;
		this.btn_OK.SettingInVisible = false;
		this.btn_OK.SettingReadonly = false;
		this.btn_OK.Size = new System.Drawing.Size(98, 23);
		this.btn_OK.TabIndex = 0;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.grd_CustomGroup.AllowPasteExcel = true;
		this.grd_CustomGroup.ClipboardData = null;
		this.grd_CustomGroup.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_CustomGroup.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_CustomGroup.Location = new System.Drawing.Point(0, 0);
		this.grd_CustomGroup.MainView = this.gv_CustomGroup;
		this.grd_CustomGroup.Name = "grd_CustomGroup";
		this.grd_CustomGroup.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[6] { this.rep_Custom, this.rep_Purchase, this.rep_Other, this.rep_Total, this.rep_btnCAA, this.rep_btnCEA });
		this.grd_CustomGroup.Size = new System.Drawing.Size(779, 346);
		this.grd_CustomGroup.TabIndex = 1;
		this.grd_CustomGroup.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_CustomGroup });
		this.grd_CustomGroup.EditorKeyDown += new System.Windows.Forms.KeyEventHandler(this.grd_CustomGroup_EditorKeyDown);
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.removeToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(118, 26);
		this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
		this.removeToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
		this.removeToolStripMenuItem.Text = "Remove";
		this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
		this.gv_CustomGroup.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_CustomGroup.ColumnKey = null;
		this.gv_CustomGroup.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[16]
		{
			this.gridColumn1, this.gridColumn2, this.gridColumn3, this.gridColumn4, this.gridColumn5, this.gridColumn6, this.gridColumn7, this.gridColumn8, this.gridColumn9, this.gridColumn10,
			this.gridColumn11, this.gridColumn12, this.gridColumn13, this.gridColumn14, this.gridColumn15, this.gridColumn16
		});
		this.gv_CustomGroup.DisableFormSetting = false;
		this.gv_CustomGroup.DisplayException = false;
		this.gv_CustomGroup.EditColumn = null;
		this.gv_CustomGroup.FieldCurrency = null;
		this.gv_CustomGroup.FieldRate = null;
		this.gv_CustomGroup.FormId = "oCustomsGroup";
		this.gv_CustomGroup.GridControl = this.grd_CustomGroup;
		this.gv_CustomGroup.IgnoreFilter = false;
		this.gv_CustomGroup.IndexEditRow = -1;
		this.gv_CustomGroup.IndicatorWidth = 35;
		this.gv_CustomGroup.IsApzTable = false;
		this.gv_CustomGroup.IsInsertNewRow = false;
		this.gv_CustomGroup.IsLoadingData = false;
		this.gv_CustomGroup.IsMappingContextAction = false;
		this.gv_CustomGroup.ItemId = "OARG";
		this.gv_CustomGroup.LstColumnAllowBatch = null;
		this.gv_CustomGroup.LstColumnNotAllowBatch = null;
		this.gv_CustomGroup.LstColumnNotEditable = null;
		this.gv_CustomGroup.LstColumNotSetting = null;
		this.gv_CustomGroup.Name = "gv_CustomGroup";
		this.gv_CustomGroup.OptionsSelection.MultiSelect = true;
		this.gv_CustomGroup.OptionsView.ShowGroupPanel = false;
		this.gv_CustomGroup.UseBatchMod = false;
		this.gv_CustomGroup.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_CustomGroup_CustomDrawRowIndicator);
		this.gv_CustomGroup.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_CustomGroup_RowCellStyle);
		this.gv_CustomGroup.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_CustomGroup_ShowingEditor);
		this.gv_CustomGroup.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_CustomGroup_CellValueChanged);
		this.gridColumn1.Caption = "CstGrpCode";
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "CstGrpCode";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.PrimaryKey = true;
		this.gridColumn1.UnitField = null;
		this.gridColumn2.Caption = "Name";
		this.gridColumn2.CurrencyDateField = null;
		this.gridColumn2.CurrencyField = null;
		this.gridColumn2.CurrencyFixed = null;
		this.gridColumn2.CurrencyFixedRate = 0.0;
		this.gridColumn2.CurrencyRateField = null;
		this.gridColumn2.FieldName = "CstGrpName";
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.OptionsColumn.FixedWidth = true;
		this.gridColumn2.PrimaryKey = false;
		this.gridColumn2.UnitField = null;
		this.gridColumn2.Visible = true;
		this.gridColumn2.VisibleIndex = 0;
		this.gridColumn2.Width = 87;
		this.gridColumn3.Caption = "Number";
		this.gridColumn3.CurrencyDateField = null;
		this.gridColumn3.CurrencyField = null;
		this.gridColumn3.CurrencyFixed = null;
		this.gridColumn3.CurrencyFixedRate = 0.0;
		this.gridColumn3.CurrencyRateField = null;
		this.gridColumn3.FieldName = "GroupNum";
		this.gridColumn3.Name = "gridColumn3";
		this.gridColumn3.OptionsColumn.FixedWidth = true;
		this.gridColumn3.PrimaryKey = false;
		this.gridColumn3.UnitField = null;
		this.gridColumn3.Visible = true;
		this.gridColumn3.VisibleIndex = 1;
		this.gridColumn4.Caption = "Customs";
		this.gridColumn4.ColumnEdit = this.rep_Custom;
		this.gridColumn4.CurrencyDateField = null;
		this.gridColumn4.CurrencyField = null;
		this.gridColumn4.CurrencyFixed = null;
		this.gridColumn4.CurrencyFixedRate = 0.0;
		this.gridColumn4.CurrencyRateField = null;
		this.gridColumn4.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gridColumn4.FieldName = "Custom";
		this.gridColumn4.Name = "gridColumn4";
		this.gridColumn4.OptionsColumn.FixedWidth = true;
		this.gridColumn4.PrimaryKey = false;
		this.gridColumn4.UnitField = null;
		this.gridColumn4.Visible = true;
		this.gridColumn4.VisibleIndex = 2;
		this.gridColumn4.Width = 80;
		this.rep_Custom.AutoHeight = false;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.withtextwrapping_topright_16x16;
		this.rep_Custom.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo, "", -1, true, true, false, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_Custom.Name = "rep_Custom";
		this.gridColumn5.Caption = "Purchase";
		this.gridColumn5.ColumnEdit = this.rep_Purchase;
		this.gridColumn5.CurrencyDateField = null;
		this.gridColumn5.CurrencyField = null;
		this.gridColumn5.CurrencyFixed = null;
		this.gridColumn5.CurrencyFixedRate = 0.0;
		this.gridColumn5.CurrencyRateField = null;
		this.gridColumn5.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gridColumn5.FieldName = "BuyTax";
		this.gridColumn5.Name = "gridColumn5";
		this.gridColumn5.OptionsColumn.FixedWidth = true;
		this.gridColumn5.PrimaryKey = false;
		this.gridColumn5.UnitField = null;
		this.gridColumn5.Visible = true;
		this.gridColumn5.VisibleIndex = 3;
		this.rep_Purchase.AutoHeight = false;
		editorButtonImageOptions2.Image = Apzon.Forms.Properties.Resources.withtextwrapping_topright_16x16;
		this.rep_Purchase.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo, "", -1, true, true, false, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_Purchase.Name = "rep_Purchase";
		this.gridColumn6.Caption = "Other";
		this.gridColumn6.ColumnEdit = this.rep_Other;
		this.gridColumn6.CurrencyDateField = null;
		this.gridColumn6.CurrencyField = null;
		this.gridColumn6.CurrencyFixed = null;
		this.gridColumn6.CurrencyFixedRate = 0.0;
		this.gridColumn6.CurrencyRateField = null;
		this.gridColumn6.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gridColumn6.FieldName = "OtherTax";
		this.gridColumn6.Name = "gridColumn6";
		this.gridColumn6.OptionsColumn.FixedWidth = true;
		this.gridColumn6.PrimaryKey = false;
		this.gridColumn6.UnitField = null;
		this.gridColumn6.Visible = true;
		this.gridColumn6.VisibleIndex = 4;
		this.gridColumn6.Width = 50;
		this.rep_Other.AutoHeight = false;
		editorButtonImageOptions3.Image = Apzon.Forms.Properties.Resources.withtextwrapping_topright_16x16;
		this.rep_Other.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo, "", -1, true, true, false, editorButtonImageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_Other.Name = "rep_Other";
		this.gridColumn7.Caption = "Total";
		this.gridColumn7.ColumnEdit = this.rep_Total;
		this.gridColumn7.CurrencyDateField = null;
		this.gridColumn7.CurrencyField = null;
		this.gridColumn7.CurrencyFixed = null;
		this.gridColumn7.CurrencyFixedRate = 0.0;
		this.gridColumn7.CurrencyRateField = null;
		this.gridColumn7.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gridColumn7.FieldName = "TotalTax";
		this.gridColumn7.Name = "gridColumn7";
		this.gridColumn7.OptionsColumn.FixedWidth = true;
		this.gridColumn7.PrimaryKey = false;
		this.gridColumn7.UnitField = null;
		this.gridColumn7.Visible = true;
		this.gridColumn7.VisibleIndex = 5;
		this.gridColumn7.Width = 50;
		this.rep_Total.AutoHeight = false;
		editorButtonImageOptions4.Image = Apzon.Forms.Properties.Resources.withtextwrapping_topright_16x16;
		this.rep_Total.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo, "", -1, true, true, false, editorButtonImageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance4, appearanceHovered4, appearancePressed4, appearanceDisabled4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_Total.Name = "rep_Total";
		this.gridColumn8.Caption = "Locked";
		this.gridColumn8.CurrencyDateField = null;
		this.gridColumn8.CurrencyField = null;
		this.gridColumn8.CurrencyFixed = null;
		this.gridColumn8.CurrencyFixedRate = 0.0;
		this.gridColumn8.CurrencyRateField = null;
		this.gridColumn8.FieldName = "Locked";
		this.gridColumn8.Name = "gridColumn8";
		this.gridColumn8.PrimaryKey = false;
		this.gridColumn8.UnitField = null;
		this.gridColumn9.Caption = "DataSource";
		this.gridColumn9.CurrencyDateField = null;
		this.gridColumn9.CurrencyField = null;
		this.gridColumn9.CurrencyFixed = null;
		this.gridColumn9.CurrencyFixedRate = 0.0;
		this.gridColumn9.CurrencyRateField = null;
		this.gridColumn9.FieldName = "DataSource";
		this.gridColumn9.Name = "gridColumn9";
		this.gridColumn9.PrimaryKey = false;
		this.gridColumn9.UnitField = null;
		this.gridColumn10.Caption = "UserSign";
		this.gridColumn10.CurrencyDateField = null;
		this.gridColumn10.CurrencyField = null;
		this.gridColumn10.CurrencyFixed = null;
		this.gridColumn10.CurrencyFixedRate = 0.0;
		this.gridColumn10.CurrencyRateField = null;
		this.gridColumn10.FieldName = "UserSign";
		this.gridColumn10.Name = "gridColumn10";
		this.gridColumn10.PrimaryKey = false;
		this.gridColumn10.UnitField = null;
		this.gridColumn11.Caption = "Customs Allocation Account";
		this.gridColumn11.CurrencyDateField = null;
		this.gridColumn11.CurrencyField = null;
		this.gridColumn11.CurrencyFixed = null;
		this.gridColumn11.CurrencyFixedRate = 0.0;
		this.gridColumn11.CurrencyRateField = null;
		this.gridColumn11.FieldName = "cstAllcAcc";
		this.gridColumn11.Name = "gridColumn11";
		this.gridColumn11.OptionsColumn.FixedWidth = true;
		this.gridColumn11.PrimaryKey = false;
		this.gridColumn11.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridColumn11.UnitField = null;
		this.gridColumn11.Visible = true;
		this.gridColumn11.VisibleIndex = 6;
		this.gridColumn11.Width = 150;
		this.gridColumn12.Caption = "Customs Expense Account";
		this.gridColumn12.CurrencyDateField = null;
		this.gridColumn12.CurrencyField = null;
		this.gridColumn12.CurrencyFixed = null;
		this.gridColumn12.CurrencyFixedRate = 0.0;
		this.gridColumn12.CurrencyRateField = null;
		this.gridColumn12.FieldName = "cstExpAcc";
		this.gridColumn12.Name = "gridColumn12";
		this.gridColumn12.PrimaryKey = false;
		this.gridColumn12.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridColumn12.UnitField = null;
		this.gridColumn12.Visible = true;
		this.gridColumn12.VisibleIndex = 7;
		this.gridColumn12.Width = 175;
		this.gridColumn13.Caption = "PortAddr";
		this.gridColumn13.CurrencyDateField = null;
		this.gridColumn13.CurrencyField = null;
		this.gridColumn13.CurrencyFixed = null;
		this.gridColumn13.CurrencyFixedRate = 0.0;
		this.gridColumn13.CurrencyRateField = null;
		this.gridColumn13.FieldName = "PortAddr";
		this.gridColumn13.Name = "gridColumn13";
		this.gridColumn13.PrimaryKey = false;
		this.gridColumn13.UnitField = null;
		this.gridColumn14.Caption = "PortState";
		this.gridColumn14.CurrencyDateField = null;
		this.gridColumn14.CurrencyField = null;
		this.gridColumn14.CurrencyFixed = null;
		this.gridColumn14.CurrencyFixedRate = 0.0;
		this.gridColumn14.CurrencyRateField = null;
		this.gridColumn14.FieldName = "PortState";
		this.gridColumn14.Name = "gridColumn14";
		this.gridColumn14.PrimaryKey = false;
		this.gridColumn14.UnitField = null;
		this.gridColumn15.Caption = "ExciExpAcc";
		this.gridColumn15.CurrencyDateField = null;
		this.gridColumn15.CurrencyField = null;
		this.gridColumn15.CurrencyFixed = null;
		this.gridColumn15.CurrencyFixedRate = 0.0;
		this.gridColumn15.CurrencyRateField = null;
		this.gridColumn15.FieldName = "ExciExpAcc";
		this.gridColumn15.Name = "gridColumn15";
		this.gridColumn15.PrimaryKey = false;
		this.gridColumn15.UnitField = null;
		this.gridColumn16.Caption = "ExciAlcAcc";
		this.gridColumn16.CurrencyDateField = null;
		this.gridColumn16.CurrencyField = null;
		this.gridColumn16.CurrencyFixed = null;
		this.gridColumn16.CurrencyFixedRate = 0.0;
		this.gridColumn16.CurrencyRateField = null;
		this.gridColumn16.FieldName = "ExciAlcAcc";
		this.gridColumn16.Name = "gridColumn16";
		this.gridColumn16.PrimaryKey = false;
		this.gridColumn16.UnitField = null;
		this.rep_btnCAA.AutoHeight = false;
		editorButtonImageOptions5.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_btnCAA.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions5, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance5, appearanceHovered5, appearancePressed5, appearanceDisabled5, "", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rep_btnCAA.Name = "rep_btnCAA";
		this.rep_btnCEA.AutoHeight = false;
		editorButtonImageOptions6.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_btnCEA.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions6, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance6, appearanceHovered6, appearancePressed6, appearanceDisabled6, "", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rep_btnCEA.Name = "rep_btnCEA";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(779, 382);
		base.Controls.Add(this.grd_CustomGroup);
		base.Controls.Add(this.panelControl1);
		base.FormId = "oCustomsGroup";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_CustomsGroup";
		this.Text = "Customs Groups";
		base.Load += new System.EventHandler(this.frm_CustomsGroup_Load);
		((System.ComponentModel.ISupportInitialize)this.panelControl1).EndInit();
		this.panelControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_CustomGroup).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_CustomGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Custom).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Purchase).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Other).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Total).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_btnCAA).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_btnCEA).EndInit();
		base.ResumeLayout(false);
	}
}
