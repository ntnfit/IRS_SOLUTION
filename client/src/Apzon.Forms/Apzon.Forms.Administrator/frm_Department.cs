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
using Apzon.HttpClientHelper;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Administrator;

public class frm_Department : XtraFormEx
{
	private const string Controller = "Department";

	private int newCodeDepartment;

	private IContainer components;

	private PanelControl panelControl1;

	private TextEditEx txtFind;

	private LabelLine lb_Find;

	private PanelControl panelControl2;

	private GridControlEx grdDoc;

	private GridViewEx gvDoc;

	private PanelControl panelControl3;

	private GridColumnEx gcCode;

	private GridColumnEx gcName;

	private GridColumnEx gcDescription;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem addNewRowToolStripMenuItem;

	private ToolStripMenuItem deleteRowToolStripMenuItem;

	private SimpleButtonEx btCancel;

	private SimpleButtonEx btOk;

	private SimpleButtonEx btDelete;

	public frm_Department()
	{
		InitializeComponent();
		SettingLanguage();
	}

	private void SettingLanguage()
	{
		Text = Resources.Titel_frmDepartment;
		gcCode.Caption = Resources.OUDP_Code;
		gcName.Caption = Resources.OUDP_Name;
		gcDescription.Caption = Resources.OUDP_Remarks;
		btOk.Text = Resources.ButtonOK;
		btDelete.Text = Resources.ButtonDelete;
		btCancel.Text = Resources.ButtonCancel;
	}

	private void btCancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void frm_Department_Load(object sender, EventArgs e)
	{
		try
		{
			RefreshData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void RefreshData()
	{
		try
		{
			GetNewDoc();
			AddNewRow();
			LoadData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void GetNewDoc()
	{
		using HttpClientHelper<string> http = new HttpClientHelper<string>(Configuration.ApiServer, "Department");
		newCodeDepartment = await http.GetIntAsync("GetMaxCode") + 1;
	}

	private async void LoadData()
	{
		using HttpClientHelper<List<DepartmentImpl>> http = new HttpClientHelper<List<DepartmentImpl>>(Configuration.ApiServer, "Department");
		List<DepartmentImpl> lst = await http.GetAsync("ShowDepartment");
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

	private void btDelete_Click(object sender, EventArgs e)
	{
		try
		{
			Delete();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void Delete()
	{
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				int code = Convert.ToInt32(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, "Code"));
				DeleteDoc(code);
				gvDoc.DeleteSelectedRows();
				RefreshData();
				newCodeDepartment = 0;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeShort, ex.Message);
		}
	}

	private void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		Delete();
	}

	private async void DeleteDoc(int code)
	{
		using HttpClientHelper<string> http = new HttpClientHelper<string>(Configuration.ApiServer, "Department");
		string text = await http.PostAsync("DeleteDoc", code);
		if (text == "null")
		{
			WinCommons.writeStatus(Enums.MessageCode.DeleteSuccess, Enums.MessageType.Success, Msg_Time._TimeShort, "");
			gvDoc.HideLoadingPanel();
			newCodeDepartment = 0;
		}
		else
		{
			WinCommons.writeStatus(Enums.MessageCode.None, Enums.MessageType.Warning, Msg_Time._TimeShort, text);
		}
	}

	private void btOk_Click(object sender, EventArgs e)
	{
		try
		{
			if (btOk.Text.Equals("Update") || btOk.Text == Resources.ButtonUpdate)
			{
				Save();
				btOk.Text = Resources.ButtonOK;
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

	private async void Save()
	{
		try
		{
			Function.ParseInt(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, "Code"));
			List<DepartmentImpl> source = (List<DepartmentImpl>)grdDoc.DataSource;
			new DepartmentImpl();
			Function.ToString(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, "Name"));
			if (XtraMessageBox.Show(Resources.xtraMsg_Save, "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				return;
			}
			source = source.Where((DepartmentImpl x) => x.Name != null && x.Name.Length > 0).ToList();
			foreach (DepartmentImpl item in source)
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

	private async Task<string> Save(DepartmentImpl doc)
	{
		using HttpClientHelper<string> http = new HttpClientHelper<string>(Configuration.ApiServer, "Department");
		string text = await http.PostAsync("SaveDoc", doc);
		if (string.IsNullOrEmpty(text))
		{
			WinCommons.writeStatus(Enums.MessageCode.Success, Enums.MessageType.Success, Msg_Time._TimeMedium, "");
			gvDoc.HideLoadingPanel();
			newCodeDepartment = 0;
			return "";
		}
		return text;
	}

	private void gvDoc_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode != Keys.Tab)
		{
			return;
		}
		List<DepartmentImpl> list = new List<DepartmentImpl>();
		if (gvDoc.DataSource != null)
		{
			list = gvDoc.DataSource as List<DepartmentImpl>;
			DepartmentImpl departmentImpl = new DepartmentImpl();
			int num = ((list.Count == 0) ? 1 : (list.Max((DepartmentImpl x) => x.Code) + 1));
			newCodeDepartment = ((newCodeDepartment > num) ? newCodeDepartment : num);
			departmentImpl.Code = newCodeDepartment;
			if (list.Where((DepartmentImpl x) => !string.IsNullOrEmpty(x.Name)).ToList().Count == 1)
			{
				list.Add(departmentImpl);
			}
		}
	}

	private void gvDoc_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
	{
		contextMenuStrip1.Show(grdDoc, e.Point);
	}

	private void txtFind_TextChanged(object sender, EventArgs e)
	{
		string text = txtFind.Text.ToString().Trim();
		if (text == null || text == "")
		{
			RefreshData();
		}
		else
		{
			Search();
		}
	}

	private void Search()
	{
		try
		{
			DepartmentImpl departmentImpl = new DepartmentImpl();
			departmentImpl.Name = txtFind.Text;
			using HttpClientHelper<List<DepartmentImpl>> httpClientHelper = new HttpClientHelper<List<DepartmentImpl>>(Configuration.ApiServer, "Department");
			List<DepartmentImpl> lst = httpClientHelper.Post("Search", departmentImpl);
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

	private void gvDoc_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void addNewRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		AddNewRow();
	}

	private void AddNewRow()
	{
		List<DepartmentImpl> lst = new List<DepartmentImpl>();
		if (gvDoc.DataSource != null)
		{
			lst = gvDoc.DataSource as List<DepartmentImpl>;
			if (lst.Where((DepartmentImpl x) => x.Name == null).ToList().Count == 0)
			{
				DepartmentImpl departmentImpl = new DepartmentImpl();
				int num = ((lst.Count == 0) ? 1 : (lst.Max((DepartmentImpl x) => x.Code) + 1));
				newCodeDepartment = ((newCodeDepartment > num) ? newCodeDepartment : num);
				departmentImpl.Code = newCodeDepartment;
				lst.Add(departmentImpl);
			}
		}
		else
		{
			DepartmentImpl departmentImpl2 = new DepartmentImpl();
			int num2 = ((lst.Count == 0) ? 1 : (lst.Max((DepartmentImpl x) => x.Code) + 1));
			newCodeDepartment = ((newCodeDepartment > num2) ? newCodeDepartment : num2);
			departmentImpl2.Code = newCodeDepartment;
			lst.Add(departmentImpl2);
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

	private void gvDoc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		string value = txtFind.Text;
		Function.ToString(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, "Name"));
		if (e.Column == gcName || e.Column == gcDescription)
		{
			if (!string.IsNullOrEmpty(value))
			{
				btOk.Text = "Update";
				return;
			}
			AddNewRow();
			btOk.Text = "Update";
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Administrator.frm_Department));
		this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
		this.txtFind = new Apzon.Controls.Editor.TextEditEx();
		this.lb_Find = new Apzon.Controls.Controls.LabelLine();
		this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
		this.grdDoc = new Apzon.Controls.Grid.GridControlEx();
		this.gvDoc = new Apzon.Controls.Grid.GridViewEx();
		this.gcCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gcName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDescription = new Apzon.Controls.Grid.GridColumnEx();
		this.panelControl3 = new DevExpress.XtraEditors.PanelControl();
		this.btCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btOk = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btDelete = new Apzon.Controls.Controls.SimpleButtonEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addNewRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		((System.ComponentModel.ISupportInitialize)this.panelControl1).BeginInit();
		this.panelControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txtFind.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl2).BeginInit();
		this.panelControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl3).BeginInit();
		this.panelControl3.SuspendLayout();
		this.contextMenuStrip1.SuspendLayout();
		base.SuspendLayout();
		this.panelControl1.Controls.Add(this.txtFind);
		this.panelControl1.Controls.Add(this.lb_Find);
		this.panelControl1.Dock = System.Windows.Forms.DockStyle.Top;
		this.panelControl1.Location = new System.Drawing.Point(0, 0);
		this.panelControl1.Name = "panelControl1";
		this.panelControl1.Size = new System.Drawing.Size(518, 37);
		this.panelControl1.TabIndex = 0;
		this.txtFind.EditControl = null;
		this.txtFind.FieldName = null;
		this.txtFind.IsSearchControl = false;
		this.txtFind.Location = new System.Drawing.Point(123, 8);
		this.txtFind.Name = "txtFind";
		this.txtFind.SettingInVisible = false;
		this.txtFind.SettingReadonly = false;
		this.txtFind.Size = new System.Drawing.Size(214, 20);
		this.txtFind.TabIndex = 1;
		this.txtFind.TextChanged += new System.EventHandler(this.txtFind_TextChanged);
		this.lb_Find.EditControl = null;
		this.lb_Find.EditValue = "Find";
		this.lb_Find.FieldName = null;
		this.lb_Find.HideWhenEmpty = false;
		this.lb_Find.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Find.Location = new System.Drawing.Point(47, 11);
		this.lb_Find.Name = "lb_Find";
		this.lb_Find.ReadOnly = false;
		this.lb_Find.SettingInVisible = false;
		this.lb_Find.SettingReadonly = false;
		this.lb_Find.ShowUnderLine = true;
		this.lb_Find.Size = new System.Drawing.Size(80, 20);
		this.lb_Find.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Find.TabIndex = 0;
		this.lb_Find.Utext = "Find";
		this.panelControl2.Controls.Add(this.grdDoc);
		this.panelControl2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panelControl2.Location = new System.Drawing.Point(0, 37);
		this.panelControl2.Name = "panelControl2";
		this.panelControl2.Size = new System.Drawing.Size(518, 266);
		this.panelControl2.TabIndex = 1;
		this.grdDoc.AllowPasteExcel = true;
		this.grdDoc.ClipboardData = null;
		this.grdDoc.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grdDoc.Location = new System.Drawing.Point(2, 2);
		this.grdDoc.MainView = this.gvDoc;
		this.grdDoc.Name = "grdDoc";
		this.grdDoc.Size = new System.Drawing.Size(514, 262);
		this.grdDoc.TabIndex = 0;
		this.grdDoc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvDoc });
		this.gvDoc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvDoc.ColumnKey = null;
		this.gvDoc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.gcCode, this.gcName, this.gcDescription });
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
		this.gvDoc.OptionsView.ShowGroupPanel = false;
		this.gvDoc.UseBatchMod = false;
		this.gvDoc.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gvDoc_CustomDrawRowIndicator);
		this.gvDoc.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(this.gvDoc_PopupMenuShowing);
		this.gvDoc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvDoc_CellValueChanged);
		this.gvDoc.KeyDown += new System.Windows.Forms.KeyEventHandler(this.gvDoc_KeyDown);
		this.gcCode.Caption = "Code";
		this.gcCode.CurrencyDateField = null;
		this.gcCode.CurrencyField = null;
		this.gcCode.CurrencyFixed = null;
		this.gcCode.CurrencyFixedRate = 0.0;
		this.gcCode.CurrencyRateField = null;
		this.gcCode.FieldName = "Code";
		this.gcCode.Name = "gcCode";
		this.gcCode.PrimaryKey = true;
		this.gcCode.UnitField = null;
		this.gcCode.Visible = true;
		this.gcCode.VisibleIndex = 0;
		this.gcName.Caption = "Name";
		this.gcName.CurrencyDateField = null;
		this.gcName.CurrencyField = null;
		this.gcName.CurrencyFixed = null;
		this.gcName.CurrencyFixedRate = 0.0;
		this.gcName.CurrencyRateField = null;
		this.gcName.FieldName = "Name";
		this.gcName.Name = "gcName";
		this.gcName.PrimaryKey = false;
		this.gcName.UnitField = null;
		this.gcName.Visible = true;
		this.gcName.VisibleIndex = 1;
		this.gcDescription.Caption = "Description";
		this.gcDescription.CurrencyDateField = null;
		this.gcDescription.CurrencyField = null;
		this.gcDescription.CurrencyFixed = null;
		this.gcDescription.CurrencyFixedRate = 0.0;
		this.gcDescription.CurrencyRateField = null;
		this.gcDescription.FieldName = "Remarks";
		this.gcDescription.Name = "gcDescription";
		this.gcDescription.PrimaryKey = false;
		this.gcDescription.UnitField = null;
		this.gcDescription.Visible = true;
		this.gcDescription.VisibleIndex = 2;
		this.panelControl3.Controls.Add(this.btCancel);
		this.panelControl3.Controls.Add(this.btOk);
		this.panelControl3.Controls.Add(this.btDelete);
		this.panelControl3.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.panelControl3.Location = new System.Drawing.Point(0, 268);
		this.panelControl3.Name = "panelControl3";
		this.panelControl3.Size = new System.Drawing.Size(518, 35);
		this.panelControl3.TabIndex = 2;
		this.btCancel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btCancel.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btCancel.Appearance.Options.UseBackColor = true;
		this.btCancel.EditControl = null;
		this.btCancel.EditValue = "Cancel";
		this.btCancel.FieldName = null;
		this.btCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btCancel.ImageOptions.Image");
		this.btCancel.IsSearchControl = false;
		this.btCancel.Location = new System.Drawing.Point(162, 5);
		this.btCancel.Name = "btCancel";
		this.btCancel.ReadOnly = false;
		this.btCancel.SettingInVisible = false;
		this.btCancel.SettingReadonly = false;
		this.btCancel.Size = new System.Drawing.Size(75, 25);
		this.btCancel.TabIndex = 18;
		this.btCancel.Text = "Cancel";
		this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
		this.btOk.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btOk.EditControl = null;
		this.btOk.EditValue = "Ok";
		this.btOk.FieldName = null;
		this.btOk.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btOk.ImageOptions.Image");
		this.btOk.IsSearchControl = false;
		this.btOk.Location = new System.Drawing.Point(0, 5);
		this.btOk.Name = "btOk";
		this.btOk.ReadOnly = false;
		this.btOk.SettingInVisible = false;
		this.btOk.SettingReadonly = false;
		this.btOk.Size = new System.Drawing.Size(75, 25);
		this.btOk.TabIndex = 16;
		this.btOk.Text = "Ok";
		this.btOk.Click += new System.EventHandler(this.btOk_Click);
		this.btDelete.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btDelete.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btDelete.Appearance.Options.UseBackColor = true;
		this.btDelete.EditControl = null;
		this.btDelete.EditValue = "Delete";
		this.btDelete.FieldName = null;
		this.btDelete.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btDelete.ImageOptions.Image");
		this.btDelete.IsSearchControl = false;
		this.btDelete.Location = new System.Drawing.Point(81, 5);
		this.btDelete.Name = "btDelete";
		this.btDelete.ReadOnly = false;
		this.btDelete.SettingInVisible = false;
		this.btDelete.SettingReadonly = false;
		this.btDelete.Size = new System.Drawing.Size(75, 25);
		this.btDelete.TabIndex = 17;
		this.btDelete.Text = "Delete";
		this.btDelete.Click += new System.EventHandler(this.btDelete_Click);
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addNewRowToolStripMenuItem, this.deleteRowToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(150, 48);
		this.addNewRowToolStripMenuItem.Name = "addNewRowToolStripMenuItem";
		this.addNewRowToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
		this.addNewRowToolStripMenuItem.Text = "Add New Row";
		this.addNewRowToolStripMenuItem.Click += new System.EventHandler(this.addNewRowToolStripMenuItem_Click);
		this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
		this.deleteRowToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
		this.deleteRowToolStripMenuItem.Text = "Delete Row";
		this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.deleteRowToolStripMenuItem_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(518, 303);
		base.Controls.Add(this.panelControl3);
		base.Controls.Add(this.panelControl2);
		base.Controls.Add(this.panelControl1);
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_Department";
		base.ShowInTaskbar = false;
		this.Text = "Department";
		base.Load += new System.EventHandler(this.frm_Department_Load);
		((System.ComponentModel.ISupportInitialize)this.panelControl1).EndInit();
		this.panelControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txtFind.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl2).EndInit();
		this.panelControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl3).EndInit();
		this.panelControl3.ResumeLayout(false);
		this.contextMenuStrip1.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
