using System;
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
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Administrator;
using Apzon.Forms.Commons;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Administrator;

public class frm_AuthorToken : XtraFormEx
{
	public frm_EmpToken frmEmp;

	private IContainer components;

	private GridControlEx grd_Doc;

	private GridViewEx gv_Doc;

	private SimpleButtonEx btn_Save;

	private GridColumnEx colEmpName;

	private GridColumnEx colAuthor;

	private RepositoryItemCheckEdit ce_Author;

	private SimpleButtonEx btn_Cancel;

	private GridColumnEx colEmpId;

	private GridColumnEx colEmpCode;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem selectAllToolStripMenuItem;

	private ToolStripMenuItem unSelectAllToolStripMenuItem;

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
			btn_Save.Text = Resources.ButtonSave;
			break;
		case Enums.FormViewModeType.View:
			btn_Save.Text = Resources.ButtonOK;
			break;
		}
	}

	public frm_AuthorToken()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_EmpToken"))
		{
			InitializeComponent();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_EmpToken" && x.authoriza == "F"))
			{
				Controls.EnableDisableControl(isEnable: false);
			}
			FormId = "oAuthorToken";
			gv_Doc.ItemId = "APZ_HET1";
			gv_Doc.FormId = FormId;
			OnLoadAsync += frm_AuthorToken_Load;
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			DataTable dataSource = await GetData(GlobalData.UserID);
			grd_Doc.DataSource = dataSource;
			grd_Doc.RefreshDataSource();
			gv_Doc.RefreshData();
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task SaveData(DataTable dt)
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("EmpId", typeof(int));
			dataTable.Columns.Add("UserId", typeof(int));
			dataTable.Columns.Add("DelAll", typeof(bool));
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["EmpId"] = Function.ParseInt(dt.Rows[i]["EmpId"]);
				dataRow["UserId"] = GlobalData.UserID;
				dataTable.Rows.Add(dataRow);
			}
			if (dt.Rows.Count == 0)
			{
				DataRow dataRow2 = dataTable.NewRow();
				dataRow2["DelAll"] = true;
				dataRow2["UserId"] = GlobalData.UserID;
				dataTable.Rows.Add(dataRow2);
			}
			using HttpClientHelper<string> hp = new HttpClientHelper<string>(Configuration.ApiServer, "AuthorToken");
			if (string.IsNullOrEmpty(await hp.PostAsync("Save", dataTable)))
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
				if (frmEmp != null)
				{
					frmEmp.btn_Refresh_Click(null, null);
				}
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.UpdateFailed, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<DataTable> GetData(int userId)
	{
		_ = 1;
		try
		{
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "AuthorToken");
			DataTable dt = await hp.GetAsync("GetAllEmployees");
			if (dt.Columns.Contains("U_OldCode"))
			{
				colEmpCode.Visible = true;
			}
			DataTable dataTable = await hp.GetAsync("GetAuthorizedEmp", new { userId });
			DataTable dataTable2 = new DataTable();
			dataTable2.Columns.Add("EmpId", typeof(int));
			dataTable2.Columns.Add("FullName", typeof(string));
			dataTable2.Columns.Add("Checked", typeof(bool));
			dataTable2.Columns.Add("U_OldCode", typeof(string));
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				int num = 0;
				for (int j = 0; j < dataTable.Rows.Count; j++)
				{
					if (Function.ToString(dt.Rows[i]["EmpId"]) == Function.ToString(dataTable.Rows[j]["EmpId"]))
					{
						num++;
					}
				}
				DataRow dataRow = dataTable2.NewRow();
				dataRow["EmpId"] = Function.ParseInt(dt.Rows[i]["EmpId"]);
				dataRow["FullName"] = Function.ToString(dt.Rows[i]["FullName"]);
				if (dt.Columns.Contains("U_OldCode"))
				{
					dataRow["U_OldCode"] = Function.ToString(dt.Rows[i]["U_OldCode"]);
				}
				if (num > 0)
				{
					dataRow["Checked"] = Function.ParseBool("true");
				}
				else
				{
					dataRow["Checked"] = Function.ParseBool("false");
				}
				dataTable2.Rows.Add(dataRow);
			}
			return dataTable2;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	private async Task frm_AuthorToken_Load(object sender, EventArgs e)
	{
		try
		{
			FormMode = Enums.FormViewModeType.View;
			WinCommons._OpenProcessing();
			DataTable dataSource = await GetData(GlobalData.UserID);
			grd_Doc.DataSource = dataSource;
			grd_Doc.RefreshDataSource();
			gv_Doc.RefreshData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private void gv_Doc_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator && e.RowHandle >= 0)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private async void btn_Save_Click(object sender, EventArgs e)
	{
		if (FormMode == Enums.FormViewModeType.View)
		{
			Close();
			return;
		}
		try
		{
			WinCommons._OpenProcessing();
			int userID = GlobalData.UserID;
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("EmpId", typeof(int));
			dataTable.Columns.Add("UserId", typeof(int));
			DataTable dataSourceTable = grd_Doc.DataSourceTable;
			for (int i = 0; i < dataSourceTable.Rows.Count; i++)
			{
				if (Function.ParseBool(dataSourceTable.Rows[i]["Checked"]))
				{
					DataRow dataRow = dataTable.NewRow();
					dataRow["EmpId"] = Function.ParseInt(dataSourceTable.Rows[i]["EmpId"]);
					dataRow["UserId"] = userID;
					dataTable.Rows.Add(dataRow);
				}
			}
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				btn_Save.Enabled = false;
				btn_Cancel.Enabled = false;
				await SaveData(dataTable);
				btn_Save.Enabled = true;
				btn_Cancel.Enabled = true;
				FormMode = Enums.FormViewModeType.View;
			}
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.SaveError, "");
		}
		WinCommons._CloseProcessing();
	}

	private void gv_Doc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		FormMode = Enums.FormViewModeType.Update;
	}

	private void gv_Doc_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		FormMode = Enums.FormViewModeType.Update;
	}

	private void gv_Doc_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		FormMode = Enums.FormViewModeType.Update;
	}

	private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataSourceTable = grd_Doc.DataSourceTable;
			for (int i = 0; i < dataSourceTable.Rows.Count; i++)
			{
				dataSourceTable.Rows[i]["Checked"] = "true";
			}
			grd_Doc.RefreshDataSource();
			FormMode = Enums.FormViewModeType.Update;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void unSelectAllToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataSourceTable = grd_Doc.DataSourceTable;
			for (int i = 0; i < dataSourceTable.Rows.Count; i++)
			{
				dataSourceTable.Rows[i]["Checked"] = "false";
			}
			grd_Doc.RefreshDataSource();
			FormMode = Enums.FormViewModeType.Update;
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
		this.components = new System.ComponentModel.Container();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Administrator.frm_AuthorToken));
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.unSelectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.colEmpId = new Apzon.Controls.Grid.GridColumnEx();
		this.colEmpCode = new Apzon.Controls.Grid.GridColumnEx();
		this.colEmpName = new Apzon.Controls.Grid.GridColumnEx();
		this.colAuthor = new Apzon.Controls.Grid.GridColumnEx();
		this.ce_Author = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ce_Author).BeginInit();
		base.SuspendLayout();
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_Doc.Location = new System.Drawing.Point(2, 2);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.ce_Author });
		this.grd_Doc.Size = new System.Drawing.Size(497, 281);
		this.grd_Doc.TabIndex = 0;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.selectAllToolStripMenuItem, this.unSelectAllToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(138, 48);
		this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
		this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
		this.selectAllToolStripMenuItem.Text = "Select All";
		this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.selectAllToolStripMenuItem_Click);
		this.unSelectAllToolStripMenuItem.Name = "unSelectAllToolStripMenuItem";
		this.unSelectAllToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
		this.unSelectAllToolStripMenuItem.Text = "UnSelect All";
		this.unSelectAllToolStripMenuItem.Click += new System.EventHandler(this.unSelectAllToolStripMenuItem_Click);
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = null;
		this.gv_Doc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[4] { this.colEmpId, this.colEmpCode, this.colEmpName, this.colAuthor });
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = "oAuthorizationToken";
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.IgnoreFilter = false;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 40;
		this.gv_Doc.IsApzTable = false;
		this.gv_Doc.IsInsertNewRow = false;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.IsMappingContextAction = false;
		this.gv_Doc.ItemId = "APZ_OATT";
		this.gv_Doc.LstColumnAllowBatch = null;
		this.gv_Doc.LstColumnNotAllowBatch = null;
		this.gv_Doc.LstColumnNotEditable = null;
		this.gv_Doc.LstColumNotSetting = null;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsFind.AllowFindPanel = false;
		this.gv_Doc.OptionsFind.AlwaysVisible = true;
		this.gv_Doc.OptionsFind.SearchInPreview = true;
		this.gv_Doc.OptionsFind.ShowClearButton = false;
		this.gv_Doc.OptionsFind.ShowFindButton = false;
		this.gv_Doc.OptionsView.ShowGroupPanel = false;
		this.gv_Doc.UseBatchMod = false;
		this.gv_Doc.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_Doc_CustomDrawRowIndicator);
		this.gv_Doc.SelectionChanged += new DevExpress.Data.SelectionChangedEventHandler(this.gv_Doc_SelectionChanged);
		this.gv_Doc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Doc_CellValueChanged);
		this.gv_Doc.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Doc_CellValueChanging);
		this.colEmpId.Caption = "Employee ID";
		this.colEmpId.CurrencyDateField = null;
		this.colEmpId.CurrencyField = null;
		this.colEmpId.CurrencyFixed = null;
		this.colEmpId.CurrencyFixedRate = 0.0;
		this.colEmpId.CurrencyRateField = null;
		this.colEmpId.FieldName = "EmpId";
		this.colEmpId.Name = "colEmpId";
		this.colEmpId.PrimaryKey = true;
		this.colEmpId.UnitField = null;
		this.colEmpId.Visible = true;
		this.colEmpId.VisibleIndex = 1;
		this.colEmpCode.Caption = "Employee Code";
		this.colEmpCode.CurrencyDateField = null;
		this.colEmpCode.CurrencyField = null;
		this.colEmpCode.CurrencyFixed = null;
		this.colEmpCode.CurrencyFixedRate = 0.0;
		this.colEmpCode.CurrencyRateField = null;
		this.colEmpCode.FieldName = "U_OldCode";
		this.colEmpCode.Name = "colEmpCode";
		this.colEmpCode.OptionsColumn.ReadOnly = true;
		this.colEmpCode.PrimaryKey = false;
		this.colEmpCode.UnitField = null;
		this.colEmpName.Caption = "Employee Name";
		this.colEmpName.CurrencyDateField = null;
		this.colEmpName.CurrencyField = null;
		this.colEmpName.CurrencyFixed = null;
		this.colEmpName.CurrencyFixedRate = 0.0;
		this.colEmpName.CurrencyRateField = null;
		this.colEmpName.FieldName = "FullName";
		this.colEmpName.Name = "colEmpName";
		this.colEmpName.OptionsColumn.ReadOnly = true;
		this.colEmpName.PrimaryKey = false;
		this.colEmpName.UnitField = null;
		this.colEmpName.Visible = true;
		this.colEmpName.VisibleIndex = 2;
		this.colAuthor.Caption = "Authorization";
		this.colAuthor.ColumnEdit = this.ce_Author;
		this.colAuthor.CurrencyDateField = null;
		this.colAuthor.CurrencyField = null;
		this.colAuthor.CurrencyFixed = null;
		this.colAuthor.CurrencyFixedRate = 0.0;
		this.colAuthor.CurrencyRateField = null;
		this.colAuthor.FieldName = "Checked";
		this.colAuthor.Name = "colAuthor";
		this.colAuthor.PrimaryKey = false;
		this.colAuthor.UnitField = null;
		this.colAuthor.Visible = true;
		this.colAuthor.VisibleIndex = 0;
		this.ce_Author.AutoHeight = false;
		this.ce_Author.ContextMenuStrip = this.contextMenuStrip1;
		this.ce_Author.FullFocusRect = true;
		this.ce_Author.Name = "ce_Author";
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.ButtonKind = Apzon.Commons.Enums.Enums.SimpleButtonType.Save;
		this.btn_Save.EditControl = null;
		this.btn_Save.EditValue = "Save";
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(2, 287);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.ReadOnly = false;
		this.btn_Save.SettingInVisible = false;
		this.btn_Save.SettingReadonly = false;
		this.btn_Save.Size = new System.Drawing.Size(75, 20);
		this.btn_Save.TabIndex = 17;
		this.btn_Save.Text = "Save";
		this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(80, 287);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 20);
		this.btn_Cancel.TabIndex = 18;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		base.AcceptButton = this.btn_Save;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btn_Cancel;
		base.ClientSize = new System.Drawing.Size(502, 310);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_Save);
		base.Controls.Add(this.grd_Doc);
		base.FormId = "oAuthorizationToken";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_AuthorToken";
		this.Text = "Authorization for employee token";
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ce_Author).EndInit();
		base.ResumeLayout(false);
	}
}
