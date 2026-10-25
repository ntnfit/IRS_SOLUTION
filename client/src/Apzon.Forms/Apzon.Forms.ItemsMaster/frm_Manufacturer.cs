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
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.Helper;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.ItemsMaster;

public class frm_Manufacturer : XtraFormEx
{
	private const string TableName = "OMRC";

	private DataTable _dtRemove = new DataTable();

	private bool _isDelete;

	private int newDocEntry;

	private int docEntry;

	private IContainer components;

	private PanelControl panelControl1;

	private PanelControl panelControl4;

	private GridControlEx grd_Manufacturer;

	private PanelControl panelControl3;

	private PanelControl panelControl2;

	private TextEditEx txtFind;

	private LabelLine lb_Find;

	private GridColumnEx STT;

	private GridColumnEx gcFIRMCODE;

	private GridColumnEx gcFIRMNAME;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem addNewToolStripMenuItem;

	private ToolStripMenuItem deleteToolStripMenuItem;

	private GridColumnEx gcDataSource;

	private RepositoryItemTextEdit rITxtE_FIRMNAME;

	private GridViewEx gv_Manufacturer;

	private GridViewEx tblViewManufacturer;

	private SimpleButtonEx btOK;

	private SimpleButtonEx btCancel;

	private GridColumnEx gcUserSign;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
		{
			WinFuntions.DisableAllControl(Controls);
			btCancel.Enabled = true;
			return;
		}
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Update:
			btOK.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btOK.Text = Resources.ButtonOK;
			break;
		}
	}

	private async void frm_Manufacturer_Load(object sender, EventArgs e)
	{
		await RefreshData();
	}

	private void gv_Manufacturer_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
	{
		contextMenuStrip1.Show(grd_Manufacturer, e.Point);
	}

	private async void addNewToolStripMenuItem_Click(object sender, EventArgs e)
	{
		await AddNewRow();
	}

	private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			foreach (DataRow row in gv_Manufacturer.GetListDataSourceSelectedRow().CopyToDataTable().Rows)
			{
				if (!string.IsNullOrEmpty(Function.ToString(row["FirmName"])))
				{
					_dtRemove.Rows.Add(row.ItemArray);
				}
			}
			if (!_dtRemove.IsNotNull() || _dtRemove.Rows.Count <= 0)
			{
				return;
			}
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Manufacturers");
			foreach (DataRow row2 in _dtRemove.Rows)
			{
				int firmCode = Function.ParseInt(row2["FirmCode"]?.ToString() ?? "");
				DataTable dataTable = httpClientHelper.Get("CheckFirmCodeOITM", new { firmCode });
				if (dataTable != null)
				{
					if (dataTable.Rows.Count == 0)
					{
						gv_Manufacturer.BeginUpdate();
						gv_Manufacturer.DeleteDataSourceRowSelected();
						gv_Manufacturer.EndUpdate();
						FormMode = Enums.FormViewModeType.Update;
						_isDelete = true;
					}
					else
					{
						WinCommons.Error(Enums.MessageCode.ManufacturerIsAssignedToItems, "");
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void gv_Manufacturer_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			DataTable dataTable = grd_Manufacturer.DataSource as DataTable;
			if (dataTable.IsNotNull() && e.Column.FieldName == "FirmName")
			{
				if (e.RowHandle == dataTable.Rows.Count - 1)
				{
					await AddNewRow();
				}
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btCancel_Click(object sender, EventArgs e)
	{
		if (FormMode == Enums.FormViewModeType.Update && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSaveBeforeClose, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
		{
			await SaveData();
		}
		Close();
	}

	private async void txtFind_TextChanged(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			string value = txtFind.Text.Trim();
			FormMode = Enums.FormViewModeType.View;
			if (string.IsNullOrEmpty(value))
			{
				await RefreshData();
			}
			else
			{
				await Search();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btOK_Click(object sender, EventArgs e)
	{
		btOK.Enabled = false;
		try
		{
			if (FormMode == Enums.FormViewModeType.Update)
			{
				await SaveData();
			}
			else
			{
				Close();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		btOK.Enabled = true;
	}

	private void gv_Manufacturer_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Return)
		{
			btOK_Click(null, null);
		}
	}

	private void grd_Manufacturer_EditorKeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.V && e.Modifiers == Keys.Control)
			{
				ExtenseionMethods.PasteFromExcelToDataTable(grd_Manufacturer, gv_Manufacturer, new List<string> { "FirmName" });
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Manufacturer_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == "FirmName" && Function.ParseInt(gv_Manufacturer.GetRowCellValue(e.RowHandle, "FirmCode")) < 0)
			{
				e.Appearance.BackColor = Color.LightGray;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Manufacturer_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (Function.ParseInt(gv_Manufacturer.GetFocusedRowCellValue("FirmCode")) < 0)
			{
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void gv_Manufacturer_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
	}

	private void gv_Manufacturer_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	public frm_Manufacturer()
	{
		InitializeComponent();
		SettingLanguage();
		FormId = "oManufacturer";
		gv_Manufacturer.ItemId = "OMRC";
		gv_Manufacturer.FormId = FormId;
	}

	private void SettingLanguage()
	{
		btCancel.Text = Resources.ButtonCancel;
		FormMode = Enums.FormViewModeType.View;
		lb_Find.Text = Resources.lb_Find;
		gcFIRMCODE.Caption = Resources.OMRC_FIRMCODE;
		gcFIRMNAME.Caption = Resources.OMRC_FIRMNAME;
		Text = Resources.OMRC_Title_frm_Manufacturer;
	}

	private async Task RefreshData()
	{
		await GetNewDoc();
		await ShowManufacturer();
	}

	private async Task GetNewDoc()
	{
		try
		{
			using HttpClientHelper<ManufacturersImpl> _help = new HttpClientHelper<ManufacturersImpl>(Configuration.ApiServer, "Manufacturers");
			newDocEntry = await _help.GetIntAsync("GetMaxCode") + 1;
			gv_Manufacturer.SetFocusedRowCellValue("statusID", newDocEntry);
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task ShowManufacturer()
	{
		_ = 1;
		try
		{
			using (HttpClientHelper<DataTable> _help = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Manufacturers"))
			{
				DataTable dataSource = await _help.GetAsync("ShowManufacturers");
				txtFind.Text = "";
				grd_Manufacturer.DataSource = dataSource;
				grd_Manufacturer.RefreshDataSource();
			}
			await AddNewRow();
			_dtRemove = (grd_Manufacturer.DataSource as DataTable).Copy().Clone();
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task Search()
	{
		try
		{
			string firmName = txtFind.Text;
			using HttpClientHelper<DataTable> _help = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Manufacturers");
			DataTable dataSource = await _help.GetDataTableAsync("Search", new { firmName });
			grd_Manufacturer.DataSource = dataSource;
			grd_Manufacturer.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private async Task SaveData()
	{
		_ = 1;
		try
		{
			WinCommons._OpenProcessing();
			if (_isDelete)
			{
				using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "Manufacturers");
				HttpResult httpResult = await helper.PostAsync("Delete", _dtRemove);
				if (!string.IsNullOrEmpty(httpResult.Message))
				{
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				}
			}
			DataTable dataTable = grd_Manufacturer.DataSource as DataTable;
			if (dataTable.IsNotNull())
			{
				List<IGrouping<string, DataRow>> list = (from i in dataTable.AsEnumerable()
					group i by Function.ToString(i.Field<string>("FirmName")).ToUpper() into gr
					where gr.Count() > 1
					select gr).ToList();
				if (list != null && list.Count > 0)
				{
					WinCommons.Error(Enums.MessageCode.ThisEntryExistsInFollowingTableManufacturers, "");
					return;
				}
				DataTable dataTable2 = dataTable.Copy();
				DataRow[] array = (from t in dataTable2.AsEnumerable()
					where !string.IsNullOrEmpty(Function.ToString(t["FirmName"]))
					select t).ToArray();
				if (array.Length != 0)
				{
					dataTable2 = array.CopyToDataTable();
				}
				DataTable dtAdd = dataTable2.Clone();
				int code = ((GetNewCode(dataTable2) <= 0) ? 1 : GetNewCode(dataTable2));
				foreach (DataRow row in dataTable2.Rows)
				{
					DataRow dataRow2 = dtAdd.NewRow();
					if (Function.ParseInt(row["FirmCode"]) > 0 || Function.ParseInt(row["FirmCode"]) == -1)
					{
						dataRow2["FirmCode"] = row["FirmCode"];
						dataRow2["FirmName"] = row["FirmName"];
						dataRow2["DataSource"] = row["DataSource"];
						dataRow2["UserSign"] = row["UserSign"];
					}
					else
					{
						dataRow2["FirmCode"] = code;
						dataRow2["FirmName"] = row["FirmName"];
						dataRow2["DataSource"] = "I";
						DataRow dataRow3 = dataRow2;
						dataRow3["UserSign"] = await GetUserSignByUserCode_tableOUSR();
						code++;
					}
					dtAdd.Rows.Add(dataRow2);
				}
				Save(dtAdd);
			}
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
		finally
		{
			WinCommons._CloseProcessing();
		}
	}

	private void Save(DataTable dt)
	{
		try
		{
			using HttpClientHelper<HttpResult> httpClientHelper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "Manufacturers");
			HttpResult httpResult = httpClientHelper.Post("SaveDoc", dt);
			if (httpResult != null && string.IsNullOrEmpty(httpResult.Message))
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
				WinCommons.writeStatus(Enums.MessageCode.Success, Enums.MessageType.Success, Msg_Time._TimeMedium, "");
				gv_Manufacturer.HideLoadingPanel();
				docEntry = 0;
				FormMode = Enums.FormViewModeType.View;
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private int GetNewCode(DataTable dt)
	{
		try
		{
			List<int> list = new List<int>();
			foreach (DataRow row in dt.Rows)
			{
				if (row["FirmCode"] != null)
				{
					list.Add(Function.ParseInt(row["FirmCode"].ToString()));
				}
			}
			if (list.Count > 0)
			{
				int? num = ((IEnumerable<int>)list).Max((Func<int, int?>)((int x) => x));
				if (num.HasValue)
				{
					return Function.ParseInt(num.ToString()) + 1;
				}
				return 0;
			}
			return 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return 0;
		}
	}

	private async Task<int> GetUserSignByUserCode_tableOUSR()
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

	private async Task AddNewRow()
	{
		try
		{
			DataTable dt = (DataTable)grd_Manufacturer.DataSource;
			DataTable tableStructure = WinFuntions.GetTableStructure("OMRC");
			if (dt.IsNotNull())
			{
				DataRow row = dt.NewRow();
				row["FirmCode"] = 0;
				row["DataSource"] = "I";
				DataRow dataRow = row;
				dataRow["UserSign"] = await GetUserSignByUserCode_tableOUSR();
				dt.Rows.Add(row);
				dt.AcceptChanges();
				grd_Manufacturer.DataSource = dt;
				grd_Manufacturer.RefreshDataSource();
			}
			else
			{
				grd_Manufacturer.DataSource = tableStructure;
				grd_Manufacturer.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task<DataTable> GetDtAddNew()
	{
		try
		{
			DataTable dataTable = grd_Manufacturer.DataSource as DataTable;
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Manufacturers");
			DataTable dataTable2 = await helper.GetAsync("GetAddNewRow", new
			{
				tableName = "OMRC",
				codeMax = dataTable.Rows[dataTable.Rows.Count - 1]["FirmCode"]
			});
			if (dataTable2.IsNotNull())
			{
				return dataTable2;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	public async Task REFRESH_DATA()
	{
		await RefreshData();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.ItemsMaster.frm_Manufacturer));
		this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
		this.panelControl4 = new DevExpress.XtraEditors.PanelControl();
		this.grd_Manufacturer = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addNewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Manufacturer = new Apzon.Controls.Grid.GridViewEx();
		this.gcFIRMCODE = new Apzon.Controls.Grid.GridColumnEx();
		this.gcFIRMNAME = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDataSource = new Apzon.Controls.Grid.GridColumnEx();
		this.gcUserSign = new Apzon.Controls.Grid.GridColumnEx();
		this.rITxtE_FIRMNAME = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.panelControl3 = new DevExpress.XtraEditors.PanelControl();
		this.btOK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
		this.txtFind = new Apzon.Controls.Editor.TextEditEx();
		this.lb_Find = new Apzon.Controls.Controls.LabelLine();
		this.tblViewManufacturer = new Apzon.Controls.Grid.GridViewEx();
		this.STT = new Apzon.Controls.Grid.GridColumnEx();
		((System.ComponentModel.ISupportInitialize)this.panelControl1).BeginInit();
		this.panelControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.panelControl4).BeginInit();
		this.panelControl4.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Manufacturer).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Manufacturer).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rITxtE_FIRMNAME).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl3).BeginInit();
		this.panelControl3.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.panelControl2).BeginInit();
		this.panelControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txtFind.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tblViewManufacturer).BeginInit();
		base.SuspendLayout();
		this.panelControl1.Controls.Add(this.panelControl4);
		this.panelControl1.Controls.Add(this.panelControl3);
		this.panelControl1.Controls.Add(this.panelControl2);
		this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panelControl1.Location = new System.Drawing.Point(0, 0);
		this.panelControl1.Name = "panelControl1";
		this.panelControl1.Size = new System.Drawing.Size(480, 376);
		this.panelControl1.TabIndex = 1;
		this.panelControl4.Controls.Add(this.grd_Manufacturer);
		this.panelControl4.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panelControl4.Location = new System.Drawing.Point(2, 34);
		this.panelControl4.Name = "panelControl4";
		this.panelControl4.Size = new System.Drawing.Size(476, 309);
		this.panelControl4.TabIndex = 2;
		this.grd_Manufacturer.AllowPasteExcel = true;
		this.grd_Manufacturer.ClipboardData = null;
		this.grd_Manufacturer.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_Manufacturer.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Manufacturer.Location = new System.Drawing.Point(2, 2);
		this.grd_Manufacturer.MainView = this.gv_Manufacturer;
		this.grd_Manufacturer.Name = "grd_Manufacturer";
		this.grd_Manufacturer.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.rITxtE_FIRMNAME });
		this.grd_Manufacturer.Size = new System.Drawing.Size(472, 305);
		this.grd_Manufacturer.TabIndex = 3;
		this.grd_Manufacturer.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Manufacturer });
		this.grd_Manufacturer.EditorKeyDown += new System.Windows.Forms.KeyEventHandler(this.grd_Manufacturer_EditorKeyDown);
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addNewToolStripMenuItem, this.deleteToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(124, 48);
		this.addNewToolStripMenuItem.Name = "addNewToolStripMenuItem";
		this.addNewToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
		this.addNewToolStripMenuItem.Text = "Add New";
		this.addNewToolStripMenuItem.Click += new System.EventHandler(this.addNewToolStripMenuItem_Click);
		this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
		this.deleteToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
		this.deleteToolStripMenuItem.Text = "Delete";
		this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
		this.gv_Manufacturer.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Manufacturer.ColumnKey = null;
		this.gv_Manufacturer.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[4] { this.gcFIRMCODE, this.gcFIRMNAME, this.gcDataSource, this.gcUserSign });
		this.gv_Manufacturer.DisableFormSetting = false;
		this.gv_Manufacturer.DisplayException = false;
		this.gv_Manufacturer.EditColumn = null;
		this.gv_Manufacturer.FieldCurrency = null;
		this.gv_Manufacturer.FieldRate = null;
		this.gv_Manufacturer.FormId = "oMenufacturer";
		this.gv_Manufacturer.GridControl = this.grd_Manufacturer;
		this.gv_Manufacturer.IgnoreFilter = false;
		this.gv_Manufacturer.IndexEditRow = -1;
		this.gv_Manufacturer.IndicatorWidth = 35;
		this.gv_Manufacturer.IsApzTable = false;
		this.gv_Manufacturer.IsInsertNewRow = false;
		this.gv_Manufacturer.IsLoadingData = false;
		this.gv_Manufacturer.IsMappingContextAction = false;
		this.gv_Manufacturer.ItemId = "ODATA";
		this.gv_Manufacturer.LstColumnAllowBatch = null;
		this.gv_Manufacturer.LstColumnNotAllowBatch = null;
		this.gv_Manufacturer.LstColumnNotEditable = null;
		this.gv_Manufacturer.LstColumNotSetting = null;
		this.gv_Manufacturer.Name = "gv_Manufacturer";
		this.gv_Manufacturer.OptionsSelection.MultiSelect = true;
		this.gv_Manufacturer.OptionsView.ShowDetailButtons = false;
		this.gv_Manufacturer.OptionsView.ShowGroupPanel = false;
		this.gv_Manufacturer.UseBatchMod = false;
		this.gv_Manufacturer.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_Manufacturer_CustomDrawRowIndicator);
		this.gv_Manufacturer.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Manufacturer_RowCellStyle);
		this.gv_Manufacturer.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(this.gv_Manufacturer_PopupMenuShowing);
		this.gv_Manufacturer.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_Manufacturer_ShowingEditor);
		this.gv_Manufacturer.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Manufacturer_CellValueChanged);
		this.gv_Manufacturer.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Manufacturer_CellValueChanging);
		this.gv_Manufacturer.KeyDown += new System.Windows.Forms.KeyEventHandler(this.gv_Manufacturer_KeyDown);
		this.gcFIRMCODE.Caption = "Code";
		this.gcFIRMCODE.CurrencyDateField = null;
		this.gcFIRMCODE.CurrencyField = null;
		this.gcFIRMCODE.CurrencyFixed = null;
		this.gcFIRMCODE.CurrencyFixedRate = 0.0;
		this.gcFIRMCODE.CurrencyRateField = null;
		this.gcFIRMCODE.FieldName = "FirmCode";
		this.gcFIRMCODE.Name = "gcFIRMCODE";
		this.gcFIRMCODE.OptionsColumn.AllowEdit = false;
		this.gcFIRMCODE.OptionsColumn.ReadOnly = true;
		this.gcFIRMCODE.OptionsColumn.TabStop = false;
		this.gcFIRMCODE.PrimaryKey = false;
		this.gcFIRMCODE.UnitField = null;
		this.gcFIRMNAME.Caption = "Manufacturer Name";
		this.gcFIRMNAME.CurrencyDateField = null;
		this.gcFIRMNAME.CurrencyField = null;
		this.gcFIRMNAME.CurrencyFixed = null;
		this.gcFIRMNAME.CurrencyFixedRate = 0.0;
		this.gcFIRMNAME.CurrencyRateField = null;
		this.gcFIRMNAME.FieldName = "FirmName";
		this.gcFIRMNAME.Name = "gcFIRMNAME";
		this.gcFIRMNAME.PrimaryKey = false;
		this.gcFIRMNAME.UnitField = null;
		this.gcFIRMNAME.Visible = true;
		this.gcFIRMNAME.VisibleIndex = 0;
		this.gcFIRMNAME.Width = 379;
		this.gcDataSource.Caption = "DataSource";
		this.gcDataSource.CurrencyDateField = null;
		this.gcDataSource.CurrencyField = null;
		this.gcDataSource.CurrencyFixed = null;
		this.gcDataSource.CurrencyFixedRate = 0.0;
		this.gcDataSource.CurrencyRateField = null;
		this.gcDataSource.FieldName = "DataSource";
		this.gcDataSource.Name = "gcDataSource";
		this.gcDataSource.OptionsColumn.AllowEdit = false;
		this.gcDataSource.PrimaryKey = false;
		this.gcDataSource.UnitField = null;
		this.gcUserSign.Caption = "UserSign";
		this.gcUserSign.CurrencyDateField = null;
		this.gcUserSign.CurrencyField = null;
		this.gcUserSign.CurrencyFixed = null;
		this.gcUserSign.CurrencyFixedRate = 0.0;
		this.gcUserSign.CurrencyRateField = null;
		this.gcUserSign.FieldName = "UserSign";
		this.gcUserSign.Name = "gcUserSign";
		this.gcUserSign.OptionsColumn.AllowEdit = false;
		this.gcUserSign.PrimaryKey = false;
		this.gcUserSign.UnitField = null;
		this.rITxtE_FIRMNAME.AutoHeight = false;
		this.rITxtE_FIRMNAME.Name = "rITxtE_FIRMNAME";
		this.panelControl3.Controls.Add(this.btOK);
		this.panelControl3.Controls.Add(this.btCancel);
		this.panelControl3.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.panelControl3.Location = new System.Drawing.Point(2, 343);
		this.panelControl3.Name = "panelControl3";
		this.panelControl3.Size = new System.Drawing.Size(476, 31);
		this.panelControl3.TabIndex = 1;
		this.btOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btOK.EditControl = null;
		this.btOK.EditValue = "OK";
		this.btOK.FieldName = null;
		this.btOK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btOK.ImageOptions.Image");
		this.btOK.IsSearchControl = false;
		this.btOK.Location = new System.Drawing.Point(3, 6);
		this.btOK.Name = "btOK";
		this.btOK.ReadOnly = false;
		this.btOK.SettingInVisible = false;
		this.btOK.SettingReadonly = false;
		this.btOK.Size = new System.Drawing.Size(99, 20);
		this.btOK.TabIndex = 36;
		this.btOK.Text = "OK";
		this.btOK.Click += new System.EventHandler(this.btOK_Click);
		this.btCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btCancel.EditControl = null;
		this.btCancel.EditValue = "Cancel";
		this.btCancel.FieldName = null;
		this.btCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btCancel.ImageOptions.Image");
		this.btCancel.IsSearchControl = false;
		this.btCancel.Location = new System.Drawing.Point(106, 6);
		this.btCancel.Name = "btCancel";
		this.btCancel.ReadOnly = false;
		this.btCancel.SettingInVisible = false;
		this.btCancel.SettingReadonly = false;
		this.btCancel.Size = new System.Drawing.Size(99, 20);
		this.btCancel.TabIndex = 37;
		this.btCancel.Text = "Cancel";
		this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
		this.panelControl2.Controls.Add(this.txtFind);
		this.panelControl2.Controls.Add(this.lb_Find);
		this.panelControl2.Dock = System.Windows.Forms.DockStyle.Top;
		this.panelControl2.Location = new System.Drawing.Point(2, 2);
		this.panelControl2.Name = "panelControl2";
		this.panelControl2.Size = new System.Drawing.Size(476, 32);
		this.panelControl2.TabIndex = 0;
		this.txtFind.EditControl = null;
		this.txtFind.FieldName = null;
		this.txtFind.IsSearchControl = false;
		this.txtFind.Location = new System.Drawing.Point(111, 7);
		this.txtFind.Name = "txtFind";
		this.txtFind.SettingInVisible = false;
		this.txtFind.SettingReadonly = false;
		this.txtFind.Size = new System.Drawing.Size(251, 20);
		this.txtFind.TabIndex = 1;
		this.txtFind.TextChanged += new System.EventHandler(this.txtFind_TextChanged);
		this.lb_Find.EditControl = null;
		this.lb_Find.EditValue = "Find";
		this.lb_Find.FieldName = null;
		this.lb_Find.HideWhenEmpty = false;
		this.lb_Find.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Find.Location = new System.Drawing.Point(36, 10);
		this.lb_Find.Name = "lb_Find";
		this.lb_Find.ReadOnly = false;
		this.lb_Find.SettingInVisible = false;
		this.lb_Find.SettingReadonly = false;
		this.lb_Find.ShowUnderLine = true;
		this.lb_Find.Size = new System.Drawing.Size(80, 20);
		this.lb_Find.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Find.TabIndex = 0;
		this.lb_Find.Utext = "Find";
		this.tblViewManufacturer.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.tblViewManufacturer.ColumnKey = null;
		this.tblViewManufacturer.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[1] { this.STT });
		this.tblViewManufacturer.DisableFormSetting = false;
		this.tblViewManufacturer.DisplayException = false;
		this.tblViewManufacturer.EditColumn = null;
		this.tblViewManufacturer.FieldCurrency = null;
		this.tblViewManufacturer.FieldRate = null;
		this.tblViewManufacturer.FormId = "oMenufacturer";
		this.tblViewManufacturer.IgnoreFilter = false;
		this.tblViewManufacturer.IndexEditRow = -1;
		this.tblViewManufacturer.IndicatorWidth = 35;
		this.tblViewManufacturer.IsApzTable = false;
		this.tblViewManufacturer.IsInsertNewRow = false;
		this.tblViewManufacturer.IsLoadingData = false;
		this.tblViewManufacturer.IsMappingContextAction = false;
		this.tblViewManufacturer.ItemId = "OFATURER";
		this.tblViewManufacturer.LstColumnAllowBatch = null;
		this.tblViewManufacturer.LstColumnNotAllowBatch = null;
		this.tblViewManufacturer.LstColumnNotEditable = null;
		this.tblViewManufacturer.LstColumNotSetting = null;
		this.tblViewManufacturer.Name = "tblViewManufacturer";
		this.tblViewManufacturer.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
		this.tblViewManufacturer.UseBatchMod = false;
		this.STT.Caption = "STT";
		this.STT.CurrencyDateField = null;
		this.STT.CurrencyField = null;
		this.STT.CurrencyFixed = null;
		this.STT.CurrencyFixedRate = 0.0;
		this.STT.CurrencyRateField = null;
		this.STT.Name = "STT";
		this.STT.PrimaryKey = false;
		this.STT.UnitField = null;
		this.STT.Visible = true;
		this.STT.VisibleIndex = 0;
		base.AcceptButton = this.btOK;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btCancel;
		base.ClientSize = new System.Drawing.Size(480, 376);
		base.Controls.Add(this.panelControl1);
		base.FormId = "oMenufacturer";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_Manufacturer";
		this.Text = "Manufacturer";
		base.Load += new System.EventHandler(this.frm_Manufacturer_Load);
		((System.ComponentModel.ISupportInitialize)this.panelControl1).EndInit();
		this.panelControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.panelControl4).EndInit();
		this.panelControl4.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Manufacturer).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Manufacturer).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rITxtE_FIRMNAME).EndInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl3).EndInit();
		this.panelControl3.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.panelControl2).EndInit();
		this.panelControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txtFind.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tblViewManufacturer).EndInit();
		base.ResumeLayout(false);
	}
}
