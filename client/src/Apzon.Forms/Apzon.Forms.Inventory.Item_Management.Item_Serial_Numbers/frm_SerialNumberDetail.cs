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
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Administrator;
using Apzon.Forms.Administrator;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Forms.ItemsMaster;
using Apzon.Forms.Properties;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Inventory.Item_Management.Item_Serial_Numbers;

public class frm_SerialNumberDetail : XtraFormEx
{
	private const string FormController = "SerialNumberDetails";

	private string _privilege = "";

	private bool _isLoading;

	private List<DataRow> _lstChanged = new List<DataRow>();

	public int _drillDownAbsEntry = -1;

	private IContainer components;

	private RepositoryItemDateEdit repositoryItemDateEdit1;

	private RepositoryItemLookUpEdit repositoryItemLookUpEdit1;

	private GridViewEx gridView1;

	private RepositoryItemButtonEdit repositoryItemButtonEdit1;

	private GridViewEx gridView2;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem serialNumberTransactionsReportToolStripMenuItem;

	private GridControlEx grd_SerialNumber;

	private GridViewEx gv_SerialNumber;

	private SimpleButtonEx btn_Cancel;

	private SimpleButtonEx btn_Save;

	private TextEditEx txt_Number;

	private LabelLine labelLine2;

	private SimpleButtonEx btn_Search;

	private LabelLine labelLine3;

	private LabelLine labelLine1;

	private ButtonEditEx btn_ItemCode;

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

	private RepositoryItemDateEdit rep_Date;

	private GridColumnEx gridColumn11;

	private GridColumnEx gridColumn12;

	private GridColumnEx gridColumn13;

	private GridColumnEx gridColumn14;

	private GridColumnEx gridColumn15;

	private GridColumnEx gridColumn16;

	private RepositoryItemButtonEdit rep_ItemCode;

	private RepositoryItemButtonEdit rep_Whs;

	private GridColumnEx gridColumn18;

	private GridColumnEx gridColumn17;

	private RepositoryItemLookUpEdit rep_luk_Status;

	private TextEditEx txt_SerialNum;

	private LabelLine labelLine4;

	private TextEditEx txt_LotNum;

	private LabelLine labelLine5;

	private TextEditEx txt_ItemName;

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
		case Enums.FormViewModeType.Update:
			btn_Save.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btn_Save.Text = Apzon.Commons.Properties.Resources.ButtonOK;
			break;
		}
	}

	private async void frm_SerialNumberDetail_Load(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			_isLoading = true;
			await WinFuntions.LoadUdfColumns(gv_SerialNumber, "OSRN");
			SetFormatControls();
			SetDisableColumn();
			LoadCombo();
			if (_drillDownAbsEntry != -1)
			{
				await LoadAnEntry(_drillDownAbsEntry);
			}
			_isLoading = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_SerialNumber_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void gv_SerialNumber_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				if (!_lstChanged.Contains(gv_SerialNumber.GetDataRow(e.RowHandle)))
				{
					_lstChanged.Add(gv_SerialNumber.GetDataRow(e.RowHandle));
				}
				if (FormMode == Enums.FormViewModeType.View)
				{
					FormMode = Enums.FormViewModeType.Update;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_SerialNumber_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (e.Column.OptionsColumn.ReadOnly)
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

	private void gv_SerialNumber_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (gv_SerialNumber.FocusedColumn.OptionsColumn.ReadOnly && gv_SerialNumber.FocusedColumn.FieldName != "ItemCode" && gv_SerialNumber.FocusedColumn.FieldName != "WhsCode")
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

	private async void serialNumberTransactionsReportToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			int absEntry = Function.ParseInt(gv_SerialNumber.GetFocusedRowCellValue("AbsEntry"));
			using (HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "SerialNumberDetails"))
			{
				DataTable dataTable = await _helper.GetDataTableAsync("GetQuantity", new { absEntry });
				if (dataTable.IsNotNull() && Function.ParseDecimal(dataTable.Rows[0]["Quantity"]) < 0m)
				{
					WinCommons.Warning(Enums.MessageCode.NoSerialNumberTransactionFound, "");
					return;
				}
			}
			string id = Function.ToString(gv_SerialNumber.GetFocusedRowCellValue("ItemCode")) + ";" + Function.ParseInt(gv_SerialNumber.GetFocusedRowCellValue("SysNumber"));
			this.DrillDown(ApzObjectTypes.oSerialNumberTransactionReport, ApzObjectTypes.oSerialNumberTransactionReport, id);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Save_Click(object sender, EventArgs e)
	{
		EnableDisableControls(p: false);
		WinCommons._OpenProcessing();
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
			}
			else if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				DataTable dataTable = grd_SerialNumber.DataSource as DataTable;
				if (dataTable.IsNotNull())
				{
					dataTable.AcceptChanges();
					DataTable dataTable2 = dataTable.Clone();
					for (int i = 0; i < _lstChanged.Count; i++)
					{
						dataTable2.ImportRow(_lstChanged[i]);
					}
					dataTable2.AcceptChanges();
					using HttpClientHelper<string> httpClientHelper = new HttpClientHelper<string>(Configuration.ApiServer, "SerialNumberDetails");
					string text = httpClientHelper.Post("SaveSerialNumberDetail", dataTable2);
					if (string.IsNullOrEmpty(text))
					{
						WinCommons.Success(Enums.MessageCode.Success, "");
						btn_Search_Click(null, null);
						_lstChanged.Clear();
					}
					else
					{
						WinCommons.Error(Enums.MessageCode.None, text);
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
		EnableDisableControls(p: true);
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void btn_ItemCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.Items, false, string.Empty, "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem != null)
			{
				btn_ItemCode.EditValue = frm_CFL2.ChooseItem["ItemCode"];
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Search_Click(object sender, EventArgs e)
	{
		_isLoading = true;
		_drillDownAbsEntry = -1;
		EnableDisableControls(p: false);
		WinCommons._OpenProcessing();
		try
		{
			FormMode = Enums.FormViewModeType.View;
			_lstChanged.Clear();
			grd_SerialNumber.DataSource = null;
			grd_SerialNumber.RefreshDataSource();
			string itemCode = Function.ToString(btn_ItemCode.Text);
			string itemName = Function.ToString(txt_ItemName.Text);
			string mnfSerial = Function.ToString(txt_Number.Text);
			string serialNum = Function.ToString(txt_SerialNum.Text);
			string lotNum = Function.ToString(txt_LotNum.Text);
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "SerialNumberDetails");
			DataTable dataSource = await _helper.GetAsync("Search", new { itemCode, itemName, mnfSerial, serialNum, lotNum });
			grd_SerialNumber.DataSource = dataSource;
			grd_SerialNumber.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
		EnableDisableControls(p: true);
		_isLoading = false;
	}

	private void rep_ItemCode_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			string text = Function.ToString(gv_SerialNumber.GetFocusedRowCellValue("ItemCode"));
			if (!string.IsNullOrWhiteSpace(text))
			{
				frm_ItemMasterData frm_ItemMasterData2 = new frm_ItemMasterData(flag: false, flagSearchFrm: true, text);
				frm_ItemMasterData2.MdiParent = MdiParent;
				frm_ItemMasterData2.Show();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rep_Whs_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			string text = Function.ToString(gv_SerialNumber.GetFocusedRowCellValue("WhsCode"));
			if (!string.IsNullOrWhiteSpace(text))
			{
				frm_WareHouses frm_WareHouses2 = new frm_WareHouses();
				frm_WareHouses2._whsSearchCode = text;
				frm_WareHouses2.MdiParent = MdiParent;
				frm_WareHouses2.Show();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public frm_SerialNumberDetail()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_SerialNumberDetail"))
		{
			InitializeComponent();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_SerialNumberDetail" && x.authoriza == "F"))
			{
				_privilege = "R";
				WinFuntions.DisableAllControl(Controls);
				btn_Cancel.Enabled = true;
			}
			else
			{
				_privilege = "F";
			}
			return;
		}
		throw new Exception(Apzon.Commons.Properties.Resources.Msg_FormAccessDenied);
	}

	private void SetFormatControls()
	{
		try
		{
			rep_Date.Mask.MaskType = MaskType.DateTime;
			rep_Date.Mask.EditMask = GlobalData.DateTimeFormat;
			rep_Date.Mask.UseMaskAsDisplayFormat = true;
			gv_SerialNumber.Columns["Quantity"].DisplayFormat.FormatType = FormatType.Numeric;
			gv_SerialNumber.Columns["Quantity"].DisplayFormat.FormatString = "n" + GlobalData.QuantitiesFormat.NumberDecimalDigits;
			gv_SerialNumber.Columns["CommitQty"].DisplayFormat.FormatType = FormatType.Numeric;
			gv_SerialNumber.Columns["CommitQty"].DisplayFormat.FormatString = "n" + GlobalData.QuantitiesFormat.NumberDecimalDigits;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadCombo()
	{
		try
		{
			rep_luk_Status.DisplayMember = "Name";
			rep_luk_Status.ValueMember = "Code";
			rep_luk_Status.Columns.Clear();
			rep_luk_Status.Columns.AddRange(new LookUpColumnInfo[1]
			{
				new LookUpColumnInfo("Name", "Name")
			});
			rep_luk_Status.NullText = "";
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add(new DataColumn("Name", typeof(string)));
			dataTable.Columns.Add(new DataColumn("Code", typeof(string)));
			dataTable.AcceptChanges();
			DataRow dataRow = dataTable.NewRow();
			dataRow["Name"] = "Available";
			dataRow["Code"] = "0";
			dataTable.Rows.Add(dataRow);
			DataRow dataRow2 = dataTable.NewRow();
			dataRow2["Name"] = "Unavailable";
			dataRow2["Code"] = "1";
			dataTable.Rows.Add(dataRow2);
			DataRow dataRow3 = dataTable.NewRow();
			dataRow3["Name"] = "Allocated";
			dataRow3["Code"] = "2";
			dataTable.Rows.Add(dataRow3);
			dataTable.AcceptChanges();
			rep_luk_Status.DataSource = dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void SetDisableColumn()
	{
		try
		{
			using HttpClientHelper<string> httpClientHelper = new HttpClientHelper<string>(Configuration.ApiServer, "SerialNumberDetails");
			switch (httpClientHelper.Get("GetUniqueSerialNo"))
			{
			case "0":
				gv_SerialNumber.Columns["DistNumber"].OptionsColumn.ReadOnly = false;
				gv_SerialNumber.Columns["MnfSerial"].OptionsColumn.ReadOnly = false;
				gv_SerialNumber.Columns["LotNumber"].OptionsColumn.ReadOnly = false;
				break;
			case "2":
				gv_SerialNumber.Columns["DistNumber"].OptionsColumn.ReadOnly = false;
				gv_SerialNumber.Columns["MnfSerial"].OptionsColumn.ReadOnly = true;
				gv_SerialNumber.Columns["LotNumber"].OptionsColumn.ReadOnly = false;
				break;
			case "3":
				gv_SerialNumber.Columns["DistNumber"].OptionsColumn.ReadOnly = true;
				gv_SerialNumber.Columns["MnfSerial"].OptionsColumn.ReadOnly = false;
				gv_SerialNumber.Columns["LotNumber"].OptionsColumn.ReadOnly = false;
				break;
			case "4":
				gv_SerialNumber.Columns["DistNumber"].OptionsColumn.ReadOnly = false;
				gv_SerialNumber.Columns["MnfSerial"].OptionsColumn.ReadOnly = false;
				gv_SerialNumber.Columns["LotNumber"].OptionsColumn.ReadOnly = true;
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void EnableDisableControls(bool p)
	{
		try
		{
			btn_Cancel.Enabled = p;
			btn_Save.Enabled = p;
			btn_Search.Enabled = p;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadAnEntry(int absEntry)
	{
		try
		{
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "SerialNumberDetails");
			DataTable dataSource = await _helper.GetAsync("LoadAnEntry", new { absEntry });
			grd_SerialNumber.DataSource = dataSource;
			grd_SerialNumber.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Inventory.Item_Management.Item_Serial_Numbers.frm_SerialNumberDetail));
		this.repositoryItemDateEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.repositoryItemLookUpEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridView1 = new Apzon.Controls.Grid.GridViewEx();
		this.repositoryItemButtonEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gridView2 = new Apzon.Controls.Grid.GridViewEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.serialNumberTransactionsReportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.grd_SerialNumber = new Apzon.Controls.Grid.GridControlEx();
		this.gv_SerialNumber = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_ItemCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gridColumn2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn3 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Whs = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gridColumn18 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn17 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn4 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_luk_Status = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumn5 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn6 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn7 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn8 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn9 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn10 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Date = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.gridColumn11 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn12 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn13 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn14 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn15 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn16 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.txt_Number = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.btn_Search = new Apzon.Controls.Controls.SimpleButtonEx();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.btn_ItemCode = new Apzon.Controls.Editor.ButtonEditEx();
		this.txt_SerialNum = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.txt_LotNum = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine5 = new Apzon.Controls.Controls.LabelLine();
		this.txt_ItemName = new Apzon.Controls.Editor.TextEditEx();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit1.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemLookUpEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridView1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemButtonEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridView2).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_SerialNumber).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_SerialNumber).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ItemCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Whs).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_luk_Status).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Date).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Date.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Number.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_ItemCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_SerialNum.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_LotNum.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ItemName.Properties).BeginInit();
		base.SuspendLayout();
		this.repositoryItemDateEdit1.AutoHeight = false;
		this.repositoryItemDateEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEdit1.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEdit1.Name = "repositoryItemDateEdit1";
		this.repositoryItemLookUpEdit1.AutoHeight = false;
		this.repositoryItemLookUpEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemLookUpEdit1.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.repositoryItemLookUpEdit1.DisplayMember = "Name";
		this.repositoryItemLookUpEdit1.Name = "repositoryItemLookUpEdit1";
		this.repositoryItemLookUpEdit1.NullText = "";
		this.repositoryItemLookUpEdit1.ShowFooter = false;
		this.repositoryItemLookUpEdit1.ShowHeader = false;
		this.repositoryItemLookUpEdit1.ShowLines = false;
		this.repositoryItemLookUpEdit1.ValueMember = "Code";
		this.gridView1.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gridView1.ColumnKey = null;
		this.gridView1.DisableFormSetting = false;
		this.gridView1.DisplayException = false;
		this.gridView1.EditColumn = null;
		this.gridView1.FieldCurrency = null;
		this.gridView1.FieldRate = null;
		this.gridView1.FormId = "oSerialNumberDetails";
		this.gridView1.IgnoreFilter = false;
		this.gridView1.IndexEditRow = -1;
		this.gridView1.IndicatorWidth = 35;
		this.gridView1.IsApzTable = false;
		this.gridView1.IsInsertNewRow = false;
		this.gridView1.IsLoadingData = false;
		this.gridView1.IsMappingContextAction = false;
		this.gridView1.ItemId = "OGRID1";
		this.gridView1.LstColumnAllowBatch = null;
		this.gridView1.LstColumnNotAllowBatch = null;
		this.gridView1.LstColumnNotEditable = null;
		this.gridView1.LstColumNotSetting = null;
		this.gridView1.Name = "gridView1";
		this.gridView1.UseBatchMod = false;
		this.repositoryItemButtonEdit1.AutoHeight = false;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.repositoryItemButtonEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.repositoryItemButtonEdit1.Name = "repositoryItemButtonEdit1";
		this.gridView2.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gridView2.ColumnKey = null;
		this.gridView2.DisableFormSetting = false;
		this.gridView2.DisplayException = false;
		this.gridView2.EditColumn = null;
		this.gridView2.FieldCurrency = null;
		this.gridView2.FieldRate = null;
		this.gridView2.FormId = "oSerialNumberDetails";
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
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.serialNumberTransactionsReportToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(256, 26);
		this.serialNumberTransactionsReportToolStripMenuItem.Name = "serialNumberTransactionsReportToolStripMenuItem";
		this.serialNumberTransactionsReportToolStripMenuItem.Size = new System.Drawing.Size(255, 22);
		this.serialNumberTransactionsReportToolStripMenuItem.Text = "Serial Number Transactions Report";
		this.serialNumberTransactionsReportToolStripMenuItem.Click += new System.EventHandler(this.serialNumberTransactionsReportToolStripMenuItem_Click);
		this.grd_SerialNumber.AllowPasteExcel = true;
		this.grd_SerialNumber.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_SerialNumber.ClipboardData = null;
		this.grd_SerialNumber.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_SerialNumber.Location = new System.Drawing.Point(6, 67);
		this.grd_SerialNumber.MainView = this.gv_SerialNumber;
		this.grd_SerialNumber.Name = "grd_SerialNumber";
		this.grd_SerialNumber.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[4] { this.rep_Date, this.rep_ItemCode, this.rep_Whs, this.rep_luk_Status });
		this.grd_SerialNumber.Size = new System.Drawing.Size(943, 481);
		this.grd_SerialNumber.TabIndex = 14;
		this.grd_SerialNumber.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_SerialNumber });
		this.gv_SerialNumber.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_SerialNumber.ColumnKey = null;
		this.gv_SerialNumber.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[18]
		{
			this.gridColumn1, this.gridColumn2, this.gridColumn3, this.gridColumn18, this.gridColumn17, this.gridColumn4, this.gridColumn5, this.gridColumn6, this.gridColumn7, this.gridColumn8,
			this.gridColumn9, this.gridColumn10, this.gridColumn11, this.gridColumn12, this.gridColumn13, this.gridColumn14, this.gridColumn15, this.gridColumn16
		});
		this.gv_SerialNumber.DisableFormSetting = false;
		this.gv_SerialNumber.DisplayException = false;
		this.gv_SerialNumber.EditColumn = null;
		this.gv_SerialNumber.FieldCurrency = null;
		this.gv_SerialNumber.FieldRate = null;
		this.gv_SerialNumber.FormId = "oSerialNumberDetails";
		this.gv_SerialNumber.GridControl = this.grd_SerialNumber;
		this.gv_SerialNumber.IgnoreFilter = false;
		this.gv_SerialNumber.IndexEditRow = -1;
		this.gv_SerialNumber.IndicatorWidth = 80;
		this.gv_SerialNumber.IsApzTable = false;
		this.gv_SerialNumber.IsInsertNewRow = false;
		this.gv_SerialNumber.IsLoadingData = false;
		this.gv_SerialNumber.IsMappingContextAction = false;
		this.gv_SerialNumber.ItemId = "OSRN";
		this.gv_SerialNumber.LstColumnAllowBatch = null;
		this.gv_SerialNumber.LstColumnNotAllowBatch = null;
		this.gv_SerialNumber.LstColumnNotEditable = null;
		this.gv_SerialNumber.LstColumNotSetting = null;
		this.gv_SerialNumber.Name = "gv_SerialNumber";
		this.gv_SerialNumber.OptionsView.ColumnAutoWidth = false;
		this.gv_SerialNumber.OptionsView.ShowGroupPanel = false;
		this.gv_SerialNumber.UseBatchMod = false;
		this.gv_SerialNumber.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_SerialNumber_CustomDrawRowIndicator);
		this.gv_SerialNumber.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_SerialNumber_RowCellStyle);
		this.gv_SerialNumber.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_SerialNumber_ShowingEditor);
		this.gv_SerialNumber.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_SerialNumber_CellValueChanged);
		this.gridColumn1.Caption = "Item Number";
		this.gridColumn1.ColumnEdit = this.rep_ItemCode;
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "ItemCode";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.OptionsColumn.ReadOnly = true;
		this.gridColumn1.PrimaryKey = true;
		this.gridColumn1.UnitField = null;
		this.gridColumn1.Visible = true;
		this.gridColumn1.VisibleIndex = 0;
		this.rep_ItemCode.AutoHeight = false;
		editorButtonImageOptions2.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_ItemCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_ItemCode.Name = "rep_ItemCode";
		this.rep_ItemCode.ReadOnly = true;
		this.rep_ItemCode.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_ItemCode_ButtonPressed);
		this.gridColumn2.Caption = "Item Description";
		this.gridColumn2.CurrencyDateField = null;
		this.gridColumn2.CurrencyField = null;
		this.gridColumn2.CurrencyFixed = null;
		this.gridColumn2.CurrencyFixedRate = 0.0;
		this.gridColumn2.CurrencyRateField = null;
		this.gridColumn2.FieldName = "itemName";
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.OptionsColumn.ReadOnly = true;
		this.gridColumn2.PrimaryKey = false;
		this.gridColumn2.UnitField = null;
		this.gridColumn2.Visible = true;
		this.gridColumn2.VisibleIndex = 1;
		this.gridColumn3.Caption = "Warehouse";
		this.gridColumn3.ColumnEdit = this.rep_Whs;
		this.gridColumn3.CurrencyDateField = null;
		this.gridColumn3.CurrencyField = null;
		this.gridColumn3.CurrencyFixed = null;
		this.gridColumn3.CurrencyFixedRate = 0.0;
		this.gridColumn3.CurrencyRateField = null;
		this.gridColumn3.FieldName = "WhsCode";
		this.gridColumn3.Name = "gridColumn3";
		this.gridColumn3.OptionsColumn.ReadOnly = true;
		this.gridColumn3.PrimaryKey = false;
		this.gridColumn3.UnitField = null;
		this.gridColumn3.Visible = true;
		this.gridColumn3.VisibleIndex = 4;
		this.rep_Whs.AutoHeight = false;
		editorButtonImageOptions3.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_Whs.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_Whs.Name = "rep_Whs";
		this.rep_Whs.ReadOnly = true;
		this.rep_Whs.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_Whs_ButtonPressed);
		this.gridColumn18.Caption = "In Stock";
		this.gridColumn18.CurrencyDateField = null;
		this.gridColumn18.CurrencyField = null;
		this.gridColumn18.CurrencyFixed = null;
		this.gridColumn18.CurrencyFixedRate = 0.0;
		this.gridColumn18.CurrencyRateField = null;
		this.gridColumn18.FieldName = "Quantity";
		this.gridColumn18.Name = "gridColumn18";
		this.gridColumn18.OptionsColumn.ReadOnly = true;
		this.gridColumn18.PrimaryKey = false;
		this.gridColumn18.UnitField = null;
		this.gridColumn18.Visible = true;
		this.gridColumn18.VisibleIndex = 2;
		this.gridColumn17.Caption = "Commited";
		this.gridColumn17.CurrencyDateField = null;
		this.gridColumn17.CurrencyField = null;
		this.gridColumn17.CurrencyFixed = null;
		this.gridColumn17.CurrencyFixedRate = 0.0;
		this.gridColumn17.CurrencyRateField = null;
		this.gridColumn17.FieldName = "CommitQty";
		this.gridColumn17.Name = "gridColumn17";
		this.gridColumn17.OptionsColumn.ReadOnly = true;
		this.gridColumn17.PrimaryKey = false;
		this.gridColumn17.UnitField = null;
		this.gridColumn17.Visible = true;
		this.gridColumn17.VisibleIndex = 3;
		this.gridColumn4.Caption = "Status";
		this.gridColumn4.ColumnEdit = this.rep_luk_Status;
		this.gridColumn4.CurrencyDateField = null;
		this.gridColumn4.CurrencyField = null;
		this.gridColumn4.CurrencyFixed = null;
		this.gridColumn4.CurrencyFixedRate = 0.0;
		this.gridColumn4.CurrencyRateField = null;
		this.gridColumn4.FieldName = "Status1";
		this.gridColumn4.Name = "gridColumn4";
		this.gridColumn4.OptionsColumn.ReadOnly = true;
		this.gridColumn4.PrimaryKey = false;
		this.gridColumn4.UnitField = null;
		this.gridColumn4.Visible = true;
		this.gridColumn4.VisibleIndex = 5;
		this.rep_luk_Status.AutoHeight = false;
		this.rep_luk_Status.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_luk_Status.Name = "rep_luk_Status";
		this.gridColumn5.Caption = "Bin Location";
		this.gridColumn5.CurrencyDateField = null;
		this.gridColumn5.CurrencyField = null;
		this.gridColumn5.CurrencyFixed = null;
		this.gridColumn5.CurrencyFixedRate = 0.0;
		this.gridColumn5.CurrencyRateField = null;
		this.gridColumn5.FieldName = "BinLoc";
		this.gridColumn5.Name = "gridColumn5";
		this.gridColumn5.OptionsColumn.ReadOnly = true;
		this.gridColumn5.PrimaryKey = false;
		this.gridColumn5.UnitField = null;
		this.gridColumn5.Visible = true;
		this.gridColumn5.VisibleIndex = 6;
		this.gridColumn6.Caption = "Mfr Serial No.";
		this.gridColumn6.CurrencyDateField = null;
		this.gridColumn6.CurrencyField = null;
		this.gridColumn6.CurrencyFixed = null;
		this.gridColumn6.CurrencyFixedRate = 0.0;
		this.gridColumn6.CurrencyRateField = null;
		this.gridColumn6.FieldName = "MnfSerial";
		this.gridColumn6.Name = "gridColumn6";
		this.gridColumn6.PrimaryKey = false;
		this.gridColumn6.UnitField = null;
		this.gridColumn6.Visible = true;
		this.gridColumn6.VisibleIndex = 7;
		this.gridColumn7.Caption = "Serial Number";
		this.gridColumn7.CurrencyDateField = null;
		this.gridColumn7.CurrencyField = null;
		this.gridColumn7.CurrencyFixed = null;
		this.gridColumn7.CurrencyFixedRate = 0.0;
		this.gridColumn7.CurrencyRateField = null;
		this.gridColumn7.FieldName = "DistNumber";
		this.gridColumn7.Name = "gridColumn7";
		this.gridColumn7.PrimaryKey = false;
		this.gridColumn7.UnitField = null;
		this.gridColumn7.Visible = true;
		this.gridColumn7.VisibleIndex = 8;
		this.gridColumn8.Caption = "Lot Number";
		this.gridColumn8.CurrencyDateField = null;
		this.gridColumn8.CurrencyField = null;
		this.gridColumn8.CurrencyFixed = null;
		this.gridColumn8.CurrencyFixedRate = 0.0;
		this.gridColumn8.CurrencyRateField = null;
		this.gridColumn8.FieldName = "LotNumber";
		this.gridColumn8.Name = "gridColumn8";
		this.gridColumn8.PrimaryKey = false;
		this.gridColumn8.UnitField = null;
		this.gridColumn8.Visible = true;
		this.gridColumn8.VisibleIndex = 9;
		this.gridColumn9.Caption = "System No.";
		this.gridColumn9.CurrencyDateField = null;
		this.gridColumn9.CurrencyField = null;
		this.gridColumn9.CurrencyFixed = null;
		this.gridColumn9.CurrencyFixedRate = 0.0;
		this.gridColumn9.CurrencyRateField = null;
		this.gridColumn9.FieldName = "SysNumber";
		this.gridColumn9.Name = "gridColumn9";
		this.gridColumn9.OptionsColumn.ReadOnly = true;
		this.gridColumn9.PrimaryKey = false;
		this.gridColumn9.UnitField = null;
		this.gridColumn9.Visible = true;
		this.gridColumn9.VisibleIndex = 10;
		this.gridColumn10.Caption = "Admission Date";
		this.gridColumn10.ColumnEdit = this.rep_Date;
		this.gridColumn10.CurrencyDateField = null;
		this.gridColumn10.CurrencyField = null;
		this.gridColumn10.CurrencyFixed = null;
		this.gridColumn10.CurrencyFixedRate = 0.0;
		this.gridColumn10.CurrencyRateField = null;
		this.gridColumn10.FieldName = "InDate";
		this.gridColumn10.Name = "gridColumn10";
		this.gridColumn10.PrimaryKey = false;
		this.gridColumn10.UnitField = null;
		this.gridColumn10.Visible = true;
		this.gridColumn10.VisibleIndex = 11;
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
		this.gridColumn11.Caption = "Manufacturing Date";
		this.gridColumn11.ColumnEdit = this.rep_Date;
		this.gridColumn11.CurrencyDateField = null;
		this.gridColumn11.CurrencyField = null;
		this.gridColumn11.CurrencyFixed = null;
		this.gridColumn11.CurrencyFixedRate = 0.0;
		this.gridColumn11.CurrencyRateField = null;
		this.gridColumn11.FieldName = "MnfDate";
		this.gridColumn11.Name = "gridColumn11";
		this.gridColumn11.PrimaryKey = false;
		this.gridColumn11.UnitField = null;
		this.gridColumn11.Visible = true;
		this.gridColumn11.VisibleIndex = 12;
		this.gridColumn12.Caption = "Expiration Date";
		this.gridColumn12.ColumnEdit = this.rep_Date;
		this.gridColumn12.CurrencyDateField = null;
		this.gridColumn12.CurrencyField = null;
		this.gridColumn12.CurrencyFixed = null;
		this.gridColumn12.CurrencyFixedRate = 0.0;
		this.gridColumn12.CurrencyRateField = null;
		this.gridColumn12.FieldName = "ExpDate";
		this.gridColumn12.Name = "gridColumn12";
		this.gridColumn12.PrimaryKey = false;
		this.gridColumn12.UnitField = null;
		this.gridColumn12.Visible = true;
		this.gridColumn12.VisibleIndex = 13;
		this.gridColumn13.Caption = "Mfr Warranty Start";
		this.gridColumn13.ColumnEdit = this.rep_Date;
		this.gridColumn13.CurrencyDateField = null;
		this.gridColumn13.CurrencyField = null;
		this.gridColumn13.CurrencyFixed = null;
		this.gridColumn13.CurrencyFixedRate = 0.0;
		this.gridColumn13.CurrencyRateField = null;
		this.gridColumn13.FieldName = "GrntStart";
		this.gridColumn13.Name = "gridColumn13";
		this.gridColumn13.PrimaryKey = false;
		this.gridColumn13.UnitField = null;
		this.gridColumn13.Visible = true;
		this.gridColumn13.VisibleIndex = 14;
		this.gridColumn14.Caption = "Mfr Warranty End";
		this.gridColumn14.ColumnEdit = this.rep_Date;
		this.gridColumn14.CurrencyDateField = null;
		this.gridColumn14.CurrencyField = null;
		this.gridColumn14.CurrencyFixed = null;
		this.gridColumn14.CurrencyFixedRate = 0.0;
		this.gridColumn14.CurrencyRateField = null;
		this.gridColumn14.FieldName = "GrntExp";
		this.gridColumn14.Name = "gridColumn14";
		this.gridColumn14.PrimaryKey = false;
		this.gridColumn14.UnitField = null;
		this.gridColumn14.Visible = true;
		this.gridColumn14.VisibleIndex = 15;
		this.gridColumn15.Caption = "Location";
		this.gridColumn15.CurrencyDateField = null;
		this.gridColumn15.CurrencyField = null;
		this.gridColumn15.CurrencyFixed = null;
		this.gridColumn15.CurrencyFixedRate = 0.0;
		this.gridColumn15.CurrencyRateField = null;
		this.gridColumn15.FieldName = "Location";
		this.gridColumn15.Name = "gridColumn15";
		this.gridColumn15.PrimaryKey = false;
		this.gridColumn15.UnitField = null;
		this.gridColumn15.Visible = true;
		this.gridColumn15.VisibleIndex = 16;
		this.gridColumn16.Caption = "Details";
		this.gridColumn16.CurrencyDateField = null;
		this.gridColumn16.CurrencyField = null;
		this.gridColumn16.CurrencyFixed = null;
		this.gridColumn16.CurrencyFixedRate = 0.0;
		this.gridColumn16.CurrencyRateField = null;
		this.gridColumn16.FieldName = "Notes";
		this.gridColumn16.Name = "gridColumn16";
		this.gridColumn16.PrimaryKey = false;
		this.gridColumn16.UnitField = null;
		this.gridColumn16.Visible = true;
		this.gridColumn16.VisibleIndex = 17;
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = Apzon.Forms.Properties.Resources.cancel_16x16;
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(90, 554);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(87, 23);
		this.btn_Cancel.TabIndex = 16;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.EditControl = null;
		this.btn_Save.EditValue = "OK";
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = Apzon.Forms.Properties.Resources.apply_16x16;
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(6, 554);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.ReadOnly = false;
		this.btn_Save.SettingInVisible = false;
		this.btn_Save.SettingReadonly = false;
		this.btn_Save.Size = new System.Drawing.Size(81, 23);
		this.btn_Save.TabIndex = 15;
		this.btn_Save.Text = "OK";
		this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
		this.txt_Number.EditControl = null;
		this.txt_Number.FieldName = null;
		this.txt_Number.IsSearchControl = false;
		this.txt_Number.Location = new System.Drawing.Point(556, 6);
		this.txt_Number.Name = "txt_Number";
		this.txt_Number.SettingInVisible = false;
		this.txt_Number.SettingReadonly = false;
		this.txt_Number.Size = new System.Drawing.Size(139, 20);
		this.txt_Number.TabIndex = 38;
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "Mfr Serial No.";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(480, 9);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(79, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 37;
		this.labelLine2.Utext = "Mfr Serial No.";
		this.btn_Search.EditControl = null;
		this.btn_Search.EditValue = "Search";
		this.btn_Search.FieldName = null;
		this.btn_Search.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Search.ImageOptions.Image");
		this.btn_Search.IsSearchControl = false;
		this.btn_Search.Location = new System.Drawing.Point(710, 6);
		this.btn_Search.Name = "btn_Search";
		this.btn_Search.ReadOnly = false;
		this.btn_Search.SettingInVisible = false;
		this.btn_Search.SettingReadonly = false;
		this.btn_Search.Size = new System.Drawing.Size(81, 20);
		this.btn_Search.TabIndex = 36;
		this.btn_Search.Text = "Search";
		this.btn_Search.Click += new System.EventHandler(this.btn_Search_Click);
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "Item Descripton";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(218, 9);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(82, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 34;
		this.labelLine3.Utext = "Item Descripton";
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Item Number";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(7, 9);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(73, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 32;
		this.labelLine1.Utext = "Item Number";
		this.btn_ItemCode.EditControl = null;
		this.btn_ItemCode.FieldName = null;
		this.btn_ItemCode.IsSearchControl = false;
		this.btn_ItemCode.Location = new System.Drawing.Point(80, 6);
		this.btn_ItemCode.Name = "btn_ItemCode";
		this.btn_ItemCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.btn_ItemCode.SettingInVisible = false;
		this.btn_ItemCode.SettingReadonly = false;
		this.btn_ItemCode.Size = new System.Drawing.Size(128, 20);
		this.btn_ItemCode.TabIndex = 33;
		this.btn_ItemCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btn_ItemCode_ButtonClick);
		this.txt_SerialNum.EditControl = null;
		this.txt_SerialNum.FieldName = null;
		this.txt_SerialNum.IsSearchControl = false;
		this.txt_SerialNum.Location = new System.Drawing.Point(80, 32);
		this.txt_SerialNum.Name = "txt_SerialNum";
		this.txt_SerialNum.SettingInVisible = false;
		this.txt_SerialNum.SettingReadonly = false;
		this.txt_SerialNum.Size = new System.Drawing.Size(128, 20);
		this.txt_SerialNum.TabIndex = 40;
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "Serial Number";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(7, 35);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(80, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 39;
		this.labelLine4.Utext = "Serial Number";
		this.txt_LotNum.EditControl = null;
		this.txt_LotNum.FieldName = null;
		this.txt_LotNum.IsSearchControl = false;
		this.txt_LotNum.Location = new System.Drawing.Point(300, 32);
		this.txt_LotNum.Name = "txt_LotNum";
		this.txt_LotNum.SettingInVisible = false;
		this.txt_LotNum.SettingReadonly = false;
		this.txt_LotNum.Size = new System.Drawing.Size(139, 20);
		this.txt_LotNum.TabIndex = 42;
		this.labelLine5.EditControl = null;
		this.labelLine5.EditValue = "Lot number";
		this.labelLine5.FieldName = null;
		this.labelLine5.HideWhenEmpty = false;
		this.labelLine5.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine5.Location = new System.Drawing.Point(220, 35);
		this.labelLine5.Name = "labelLine5";
		this.labelLine5.ReadOnly = false;
		this.labelLine5.SettingInVisible = false;
		this.labelLine5.SettingReadonly = false;
		this.labelLine5.ShowUnderLine = true;
		this.labelLine5.Size = new System.Drawing.Size(80, 20);
		this.labelLine5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine5.TabIndex = 41;
		this.labelLine5.Utext = "Lot number";
		this.txt_ItemName.EditControl = null;
		this.txt_ItemName.FieldName = null;
		this.txt_ItemName.IsSearchControl = false;
		this.txt_ItemName.Location = new System.Drawing.Point(300, 6);
		this.txt_ItemName.Name = "txt_ItemName";
		this.txt_ItemName.SettingInVisible = false;
		this.txt_ItemName.SettingReadonly = false;
		this.txt_ItemName.Size = new System.Drawing.Size(139, 20);
		this.txt_ItemName.TabIndex = 43;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(955, 582);
		base.Controls.Add(this.txt_ItemName);
		base.Controls.Add(this.txt_LotNum);
		base.Controls.Add(this.labelLine5);
		base.Controls.Add(this.txt_SerialNum);
		base.Controls.Add(this.labelLine4);
		base.Controls.Add(this.txt_Number);
		base.Controls.Add(this.labelLine2);
		base.Controls.Add(this.btn_Search);
		base.Controls.Add(this.labelLine3);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.btn_ItemCode);
		base.Controls.Add(this.grd_SerialNumber);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_Save);
		base.FormId = "oSerialNumberDetails";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_SerialNumberDetail";
		this.Text = "Serial Number Details";
		base.Load += new System.EventHandler(this.frm_SerialNumberDetail_Load);
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit1.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemLookUpEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridView1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemButtonEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridView2).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_SerialNumber).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_SerialNumber).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ItemCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Whs).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_luk_Status).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Date.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Date).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Number.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_ItemCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_SerialNum.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_LotNum.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ItemName.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
