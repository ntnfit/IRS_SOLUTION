using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
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
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Inventory.InventoryTransaction;

public class frm_Location : XtraFormEx
{
	public delegate void ShowForms(XtraFormEx frm);

	private int newDocEntry;

	private int docEntry;

	public ShowForms frmShowChildrenForm;

	private IContainer components;

	private ToolStripMenuItem deleteToolStripMenuItem;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem addNewToolStripMenuItem;

	private GridColumnEx STT;

	private GridViewEx tblViewManufacturer;

	private TextEditEx txtFind;

	private LabelLine lb_Find;

	private PanelControl panelControl2;

	private SimpleButtonEx btOK;

	private SimpleButtonEx btCancel;

	private GridColumnEx gcLocation;

	private GridColumnEx gcCode;

	private GridViewEx gvDoc;

	private GridControlEx grdDoc;

	private PanelControl panelControl4;

	private PanelControl panelControl1;

	private PanelControl panelControl3;

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
		case Enums.FormViewModeType.Add:
			btOK.Text = Resources.ButtonAdd;
			break;
		case Enums.FormViewModeType.Update:
			btOK.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btOK.Text = Resources.ButtonOK;
			break;
		case Enums.FormViewModeType.Find:
			btOK.Text = Resources.ButtonFind;
			break;
		}
	}

	public frm_Location()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_Location"))
		{
			InitializeComponent();
			SettingLanguage();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_Location" && x.authoriza == "F"))
			{
				WinFuntions.DisableAllControl(Controls);
				btCancel.Enabled = true;
			}
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	private void SettingLanguage()
	{
		Text = Resources.Title_frmLocation;
		lb_Find.Utext = Resources.ButtonFind;
		btCancel.Text = Resources.ButtonCancel;
		FormMode = Enums.FormViewModeType.View;
		gcCode.Caption = Resources.lb_LocationCode;
		gcLocation.Caption = Resources.lb_Name;
	}

	private void frm_Location_Load(object sender, EventArgs e)
	{
		RefreshData();
	}

	private void RefreshData()
	{
		GetNewDoc();
		LoadData();
	}

	private async void GetNewDoc()
	{
		try
		{
			using HttpClientHelper<LocationImpl> _hlep = new HttpClientHelper<LocationImpl>(Configuration.ApiServer, "Location");
			WinCommons._OpenProcessing();
			newDocEntry = await _hlep.GetIntAsync("GetMaxCode") + 1;
			gvDoc.SetFocusedRowCellValue("Code", newDocEntry);
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void LoadData()
	{
		ShowLocation();
	}

	private async void ShowLocation()
	{
		try
		{
			using HttpClientHelper<LocationImpl> _help = new HttpClientHelper<LocationImpl>(Configuration.ApiServer, "Location");
			List<LocationImpl> lst = await _help.GetListAsync("ShowLocation");
			if (grdDoc.InvokeRequired)
			{
				BeginInvoke((MethodInvoker)(() =>
				{
					grdDoc.DataSource = lst;
					grdDoc.RefreshDataSource();
					txtFind.Text = string.Empty;
				}));
			}
			else
			{
				grdDoc.DataSource = lst;
				grdDoc.RefreshDataSource();
				txtFind.Text = string.Empty;
			}
			AddNewRow();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				int code = Convert.ToInt32(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, "Code"));
				DeleteDoc(code);
				gvDoc.DeleteSelectedRows();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeShort, ex.Message);
		}
	}

	private async void DeleteDoc(int code)
	{
		_ = string.Empty;
		try
		{
			using HttpClientHelper<LocationImpl> _hel = new HttpClientHelper<LocationImpl>(Configuration.ApiServer, "Location");
			string text = await _hel.PostParamsAsJsonAsync("DeleteDoc", code);
			if (string.IsNullOrEmpty(text))
			{
				WinCommons.writeStatus(Enums.MessageCode.DeleteSuccess, Enums.MessageType.Success, Msg_Time._TimeShort, "");
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, text);
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void addNewToolStripMenuItem_Click(object sender, EventArgs e)
	{
		AddNewRow();
	}

	private void AddNewRow()
	{
		List<LocationImpl> lst = new List<LocationImpl>();
		if (gvDoc.DataSource != null)
		{
			lst = gvDoc.DataSource as List<LocationImpl>;
			if (lst.Where((LocationImpl x) => x.Location == null).ToList().Count == 0)
			{
				LocationImpl locationImpl = new LocationImpl();
				int num = ((lst.Count == 0) ? 1 : (lst.Max((LocationImpl x) => x.Code) + 1));
				newDocEntry = ((newDocEntry > num) ? newDocEntry : num);
				locationImpl.Code = newDocEntry;
				lst.Add(locationImpl);
			}
		}
		else
		{
			LocationImpl locationImpl2 = new LocationImpl();
			int num2 = ((lst.Count == 0) ? 1 : (lst.Max((LocationImpl x) => x.Code) + 1));
			newDocEntry = ((newDocEntry > num2) ? newDocEntry : num2);
			locationImpl2.Code = newDocEntry;
			lst.Add(locationImpl2);
		}
		if (grdDoc.InvokeRequired)
		{
			BeginInvoke((MethodInvoker)(() =>
			{
				grdDoc.DataSource = lst;
				grdDoc.RefreshDataSource();
			}));
		}
		else
		{
			grdDoc.DataSource = lst;
			grdDoc.RefreshDataSource();
		}
	}

	private void gvDoc_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
	{
		contextMenuStrip1.Show(grdDoc, e.Point);
	}

	private void gvDoc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		if (e.Column == gcLocation)
		{
			AddNewRow();
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	private void btCancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private async void txtFind_TextChanged(object sender, EventArgs e)
	{
		string text = txtFind.Text.ToString().Trim();
		if (text == null || text == "")
		{
			RefreshData();
		}
		else
		{
			await Search();
		}
	}

	private async Task Search()
	{
		try
		{
			LocationImpl locationImpl = new LocationImpl();
			locationImpl.Code = Function.ParseInt(txtFind.Text.ToString());
			using HttpClientHelper<LocationImpl> _hlep = new HttpClientHelper<LocationImpl>(Configuration.ApiServer, "Location");
			List<LocationImpl> lst = await _hlep.PostListParameter("Search", locationImpl);
			if (grdDoc.InvokeRequired)
			{
				BeginInvoke((MethodInvoker)(() =>
				{
					grdDoc.DataSource = lst;
					grdDoc.RefreshDataSource();
				}));
			}
			else
			{
				grdDoc.DataSource = lst;
				grdDoc.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private void btOK_Click(object sender, EventArgs e)
	{
		if (FormMode == Enums.FormViewModeType.Update)
		{
			Add();
			FormMode = Enums.FormViewModeType.View;
		}
		else
		{
			Close();
		}
	}

	private async void Add()
	{
		try
		{
			Function.ParseInt(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, gcCode));
			List<LocationImpl> source = (List<LocationImpl>)grdDoc.DataSource;
			new LocationImpl();
			Function.ToString(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, gcLocation));
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				return;
			}
			source = source.Where((LocationImpl x) => x.Location != null && x.Location.Length > 0).ToList();
			foreach (LocationImpl item in source)
			{
				await Save(item);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private async Task<string> Save(LocationImpl doc)
	{
		string result = string.Empty;
		try
		{
			using HttpClientHelper<LocationImpl> _help = new HttpClientHelper<LocationImpl>(Configuration.ApiServer, "Location");
			result = await _help.PostAsJsonAsync("SaveDoc", doc);
			if (string.IsNullOrEmpty(result))
			{
				WinCommons.writeStatus(Enums.MessageCode.Success, Enums.MessageType.Success, Msg_Time._TimeMedium, "");
				gvDoc.HideLoadingPanel();
				docEntry = 0;
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, result);
			}
			return result;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return result;
		}
	}

	private void gvDoc_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void gvDoc_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode != Keys.Tab)
		{
			return;
		}
		List<LocationImpl> list = new List<LocationImpl>();
		if (gvDoc.DataSource != null)
		{
			list = gvDoc.DataSource as List<LocationImpl>;
			LocationImpl locationImpl = new LocationImpl();
			int num = ((list.Count == 0) ? 1 : (list.Max((LocationImpl x) => x.Code) + 1));
			newDocEntry = ((newDocEntry > num) ? newDocEntry : num);
			locationImpl.Code = newDocEntry;
			if (list.Where((LocationImpl x) => string.IsNullOrEmpty(x.Location)).ToList().Count == 1)
			{
				list.Add(locationImpl);
			}
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Inventory.InventoryTransaction.frm_Location));
		this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addNewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.STT = new Apzon.Controls.Grid.GridColumnEx();
		this.tblViewManufacturer = new Apzon.Controls.Grid.GridViewEx();
		this.txtFind = new Apzon.Controls.Editor.TextEditEx();
		this.lb_Find = new Apzon.Controls.Controls.LabelLine();
		this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
		this.btOK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.gcLocation = new Apzon.Controls.Grid.GridColumnEx();
		this.gcCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gvDoc = new Apzon.Controls.Grid.GridViewEx();
		this.grdDoc = new Apzon.Controls.Grid.GridControlEx();
		this.panelControl4 = new DevExpress.XtraEditors.PanelControl();
		this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
		this.panelControl3 = new DevExpress.XtraEditors.PanelControl();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.tblViewManufacturer).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtFind.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl2).BeginInit();
		this.panelControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl4).BeginInit();
		this.panelControl4.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.panelControl1).BeginInit();
		this.panelControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.panelControl3).BeginInit();
		this.panelControl3.SuspendLayout();
		base.SuspendLayout();
		this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
		this.deleteToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
		this.deleteToolStripMenuItem.Text = "Delete";
		this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addNewToolStripMenuItem, this.deleteToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(124, 48);
		this.addNewToolStripMenuItem.Name = "addNewToolStripMenuItem";
		this.addNewToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
		this.addNewToolStripMenuItem.Text = "Add New";
		this.addNewToolStripMenuItem.Click += new System.EventHandler(this.addNewToolStripMenuItem_Click);
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
		this.tblViewManufacturer.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.tblViewManufacturer.ColumnKey = null;
		this.tblViewManufacturer.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[1] { this.STT });
		this.tblViewManufacturer.DisableFormSetting = false;
		this.tblViewManufacturer.DisplayException = false;
		this.tblViewManufacturer.EditColumn = null;
		this.tblViewManufacturer.FieldCurrency = null;
		this.tblViewManufacturer.FieldRate = null;
		this.tblViewManufacturer.FormId = null;
		this.tblViewManufacturer.IgnoreFilter = false;
		this.tblViewManufacturer.IndexEditRow = -1;
		this.tblViewManufacturer.IndicatorWidth = 35;
		this.tblViewManufacturer.IsApzTable = false;
		this.tblViewManufacturer.IsInsertNewRow = false;
		this.tblViewManufacturer.IsLoadingData = false;
		this.tblViewManufacturer.IsMappingContextAction = false;
		this.tblViewManufacturer.ItemId = null;
		this.tblViewManufacturer.LstColumnAllowBatch = null;
		this.tblViewManufacturer.LstColumnNotAllowBatch = null;
		this.tblViewManufacturer.LstColumnNotEditable = null;
		this.tblViewManufacturer.LstColumNotSetting = null;
		this.tblViewManufacturer.Name = "tblViewManufacturer";
		this.tblViewManufacturer.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
		this.tblViewManufacturer.UseBatchMod = false;
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
		this.panelControl2.Controls.Add(this.txtFind);
		this.panelControl2.Controls.Add(this.lb_Find);
		this.panelControl2.Dock = System.Windows.Forms.DockStyle.Top;
		this.panelControl2.Location = new System.Drawing.Point(2, 2);
		this.panelControl2.Name = "panelControl2";
		this.panelControl2.Size = new System.Drawing.Size(423, 32);
		this.panelControl2.TabIndex = 0;
		this.btOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btOK.EditControl = null;
		this.btOK.EditValue = "OK";
		this.btOK.FieldName = null;
		this.btOK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btOK.ImageOptions.Image");
		this.btOK.IsSearchControl = false;
		this.btOK.Location = new System.Drawing.Point(3, 12);
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
		this.btCancel.Location = new System.Drawing.Point(105, 12);
		this.btCancel.Name = "btCancel";
		this.btCancel.ReadOnly = false;
		this.btCancel.SettingInVisible = false;
		this.btCancel.SettingReadonly = false;
		this.btCancel.Size = new System.Drawing.Size(99, 20);
		this.btCancel.TabIndex = 37;
		this.btCancel.Text = "Cancel";
		this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
		this.gcLocation.Caption = "Name";
		this.gcLocation.CurrencyDateField = null;
		this.gcLocation.CurrencyField = null;
		this.gcLocation.CurrencyFixed = null;
		this.gcLocation.CurrencyFixedRate = 0.0;
		this.gcLocation.CurrencyRateField = null;
		this.gcLocation.FieldName = "Location";
		this.gcLocation.Name = "gcLocation";
		this.gcLocation.PrimaryKey = false;
		this.gcLocation.UnitField = null;
		this.gcLocation.Visible = true;
		this.gcLocation.VisibleIndex = 1;
		this.gcLocation.Width = 747;
		this.gcCode.Caption = "Location Code";
		this.gcCode.CurrencyDateField = null;
		this.gcCode.CurrencyField = null;
		this.gcCode.CurrencyFixed = null;
		this.gcCode.CurrencyFixedRate = 0.0;
		this.gcCode.CurrencyRateField = null;
		this.gcCode.FieldName = "Code";
		this.gcCode.Name = "gcCode";
		this.gcCode.OptionsColumn.AllowEdit = false;
		this.gcCode.OptionsColumn.ReadOnly = true;
		this.gcCode.OptionsColumn.TabStop = false;
		this.gcCode.PrimaryKey = false;
		this.gcCode.UnitField = null;
		this.gcCode.Visible = true;
		this.gcCode.VisibleIndex = 0;
		this.gcCode.Width = 315;
		this.gvDoc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvDoc.ColumnKey = null;
		this.gvDoc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[2] { this.gcCode, this.gcLocation });
		this.gvDoc.DisableFormSetting = false;
		this.gvDoc.DisplayException = false;
		this.gvDoc.EditColumn = null;
		this.gvDoc.FieldCurrency = null;
		this.gvDoc.FieldRate = null;
		this.gvDoc.FormId = null;
		this.gvDoc.GridControl = this.grdDoc;
		this.gvDoc.IgnoreFilter = false;
		this.gvDoc.IndexEditRow = -1;
		this.gvDoc.IndicatorWidth = 35;
		this.gvDoc.IsApzTable = false;
		this.gvDoc.IsInsertNewRow = false;
		this.gvDoc.IsLoadingData = false;
		this.gvDoc.IsMappingContextAction = false;
		this.gvDoc.ItemId = null;
		this.gvDoc.LstColumnAllowBatch = null;
		this.gvDoc.LstColumnNotAllowBatch = null;
		this.gvDoc.LstColumnNotEditable = null;
		this.gvDoc.LstColumNotSetting = null;
		this.gvDoc.Name = "gvDoc";
		this.gvDoc.OptionsView.ShowDetailButtons = false;
		this.gvDoc.OptionsView.ShowGroupPanel = false;
		this.gvDoc.UseBatchMod = false;
		this.gvDoc.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gvDoc_CustomDrawRowIndicator);
		this.gvDoc.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(this.gvDoc_PopupMenuShowing);
		this.gvDoc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvDoc_CellValueChanged);
		this.gvDoc.KeyDown += new System.Windows.Forms.KeyEventHandler(this.gvDoc_KeyDown);
		this.grdDoc.AllowPasteExcel = true;
		this.grdDoc.ClipboardData = null;
		this.grdDoc.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grdDoc.Location = new System.Drawing.Point(2, 2);
		this.grdDoc.MainView = this.gvDoc;
		this.grdDoc.Name = "grdDoc";
		this.grdDoc.Size = new System.Drawing.Size(419, 256);
		this.grdDoc.TabIndex = 3;
		this.grdDoc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvDoc });
		this.panelControl4.Controls.Add(this.grdDoc);
		this.panelControl4.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panelControl4.Location = new System.Drawing.Point(2, 34);
		this.panelControl4.Name = "panelControl4";
		this.panelControl4.Size = new System.Drawing.Size(423, 260);
		this.panelControl4.TabIndex = 2;
		this.panelControl1.Controls.Add(this.panelControl4);
		this.panelControl1.Controls.Add(this.panelControl3);
		this.panelControl1.Controls.Add(this.panelControl2);
		this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panelControl1.Location = new System.Drawing.Point(0, 0);
		this.panelControl1.Name = "panelControl1";
		this.panelControl1.Size = new System.Drawing.Size(427, 331);
		this.panelControl1.TabIndex = 2;
		this.panelControl3.Controls.Add(this.btOK);
		this.panelControl3.Controls.Add(this.btCancel);
		this.panelControl3.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.panelControl3.Location = new System.Drawing.Point(2, 294);
		this.panelControl3.Name = "panelControl3";
		this.panelControl3.Size = new System.Drawing.Size(423, 35);
		this.panelControl3.TabIndex = 1;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btCancel;
		base.ClientSize = new System.Drawing.Size(427, 331);
		base.Controls.Add(this.panelControl1);
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_Location";
		this.Text = "Location";
		base.Load += new System.EventHandler(this.frm_Location_Load);
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.tblViewManufacturer).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtFind.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl2).EndInit();
		this.panelControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gvDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl4).EndInit();
		this.panelControl4.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.panelControl1).EndInit();
		this.panelControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.panelControl3).EndInit();
		this.panelControl3.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
