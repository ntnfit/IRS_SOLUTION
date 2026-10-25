using System;
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
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.HttpClientHelper;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.HumanResources;

public class frm_HRTeamsSetup : XtraFormEx
{
	public RefreshLookupTeam refreshTeam;

	protected Enums.FormAuthorities Authoritues = Enums.FormAuthorities.Read;

	private IContainer components;

	private GridControlEx grd_Team;

	private GridViewEx gv_Team;

	private GridColumnEx cl_teamID;

	private GridColumnEx cl_name;

	private GridColumnEx cl_descriptio;

	public SimpleButtonEx btnCancel;

	private ContextMenuStrip contextMenuStrip_Team;

	private ToolStripMenuItem toolStripMenuItem_Remove;

	private ToolStripMenuItem toolStripMenuItem_Refresh;

	public SimpleButtonEx btn_Save;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
		{
			WinFuntions.DisableAllControl(Controls);
			btnCancel.Enabled = true;
			return;
		}
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Update:
			btn_Save.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btn_Save.Text = Resources.ButtonOK;
			break;
		case Enums.FormViewModeType.Busy:
			btn_Save.Enabled = false;
			btnCancel.Enabled = false;
			break;
		case Enums.FormViewModeType.Add:
			break;
		}
	}

	public frm_HRTeamsSetup()
	{
		InitializeComponent();
		FormId = "oHRTeamsSetup";
		gv_Team.FormId = FormId;
		gv_Team.ItemId = "OHTM";
		OnLoadAsync += frm_HRTeamsSetup_Load;
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		try
		{
			Close();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private async void btn_Save_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
			}
			else
			{
				await SaveTeams();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			await RefreshData();
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task SaveTeams()
	{
		try
		{
			DataTable dataTable = (grd_Team.DataSource as DataTable).Copy();
			if (dataTable.IsNotNull())
			{
				DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["teamID"]) == 0);
				if (string.IsNullOrEmpty(Function.ToString(dataRow["name"])) && string.IsNullOrEmpty(Function.ToString(dataRow["descriptio"])))
				{
					dataTable.Rows.Remove(dataRow);
					dataTable.AcceptChanges();
				}
				if ((from t in dataTable.AsEnumerable()
					select Function.ToString(t["name"])).Distinct().Count() != dataTable.Rows.Count)
				{
					WinCommons.Error(Enums.MessageCode.ThisEntryAlreadyExistsInTable, "");
				}
				else
				{
					if (!dataTable.IsNotNull())
					{
						return;
					}
					dataTable = dataTable.Copy();
					if (dataTable.AsEnumerable().FirstOrDefault((DataRow r) => Function.ToString(r["name"]).Length > 20) != null)
					{
						WinCommons.Warning(Enums.MessageCode.TheLengthOfStringEnteredIsLargerThanSpecified, "");
						return;
					}
					if (dataTable.AsEnumerable().FirstOrDefault((DataRow t) => string.IsNullOrEmpty(Function.ToString(t["name"]))) != null)
					{
						WinCommons.Error(Enums.MessageCode.NameCannotBeEmpty, "");
						return;
					}
					dataTable.AcceptChanges();
					using HttpClientHelper<HttpResult> _dtHelp = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "States");
					HttpResult httpResult = _dtHelp.Post("SaveTeamSetup", dataTable);
					if (string.IsNullOrEmpty(httpResult.Message))
					{
						WinCommons.Success(Enums.MessageCode.Success, "");
						if (refreshTeam != null && !((XtraForm)refreshTeam.Target).IsDisposed)
						{
							refreshTeam();
						}
						FormMode = Enums.FormViewModeType.View;
						await RefreshData();
					}
					else
					{
						WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
					}
				}
			}
			else
			{
				grd_Team.DataSource = dataTable.Clone();
				grd_Team.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void RemoveTeams(int objID)
	{
		try
		{
			using HttpClientHelper<string> hp = new HttpClientHelper<string>(Configuration.ApiServer, "States");
			string text = hp.Post("RemoveTeamSetup", objID);
			if (string.IsNullOrWhiteSpace(text))
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
				gv_Team.BeginUpdate();
				gv_Team.DeleteSelectedRows();
				gv_Team.EndUpdate();
				if (refreshTeam != null && !((XtraForm)refreshTeam.Target).IsDisposed)
				{
					refreshTeam();
				}
				await RefreshData();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task RefreshData()
	{
		try
		{
			GridControlEx gridControlEx = grd_Team;
			gridControlEx.DataSource = await GetNewTeam();
			grd_Team.DataSourceTable.AcceptChanges();
			grd_Team.RefreshDataSource();
			FormMode = Enums.FormViewModeType.View;
			AddNewRow();
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private async Task<DataTable> GetNewTeam()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "States");
			return await helper.GetAsync("GetTeamSetup");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	private async void toolStripMenuItem_Remove_Click(object sender, EventArgs e)
	{
		try
		{
			int num = Function.ParseInt(gv_Team.GetFocusedRowCellValue("teamID"));
			if (num > 0 && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				RemoveTeams(num);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void toolStripMenuItem_Refresh_Click(object sender, EventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			FormMode = Enums.FormViewModeType.Busy;
			await RefreshData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	private void AddNewRow()
	{
		try
		{
			gv_Team.BeginInsertRow();
			DataTable dataTable = grd_Team.DataSource as DataTable;
			if (dataTable.IsNotNull())
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["teamID"] = 0;
				dataRow["name"] = "";
				dataRow["descriptio"] = "";
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
				grd_Team.DataSource = dataTable;
				grd_Team.RefreshDataSource();
			}
			else
			{
				DataTable tableStructure = WinFuntions.GetTableStructure("OHTM");
				DataRow dataRow2 = tableStructure.NewRow();
				dataRow2["teamID"] = 0;
				dataRow2["name"] = "";
				dataRow2["descriptio"] = "";
				tableStructure.Rows.Add(dataRow2);
				tableStructure.AcceptChanges();
				grd_Team.DataSource = tableStructure;
				grd_Team.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			gv_Team.EndInsertRow();
		}
	}

	private void gv_Team_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			int num = 0;
			for (int i = 0; i < gv_Team.RowCount; i++)
			{
				num = gv_Team.GetVisibleRowHandle(i);
			}
			string text = Function.ToString(gv_Team.GetRowCellValue(e.RowHandle, "Name"));
			if (e.Column.FieldName == "Name" && text != "" && e.RowHandle == num)
			{
				AddNewRow();
			}
			if (FormMode == Enums.FormViewModeType.View)
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

	private void gv_Team_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator && e.RowHandle != -2147483647)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private async Task frm_HRTeamsSetup_Load(object sender, EventArgs e)
	{
		try
		{
			await RefreshData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void gv_Team_MouseUp(object sender, MouseEventArgs e)
	{
		if (gv_Team.GetDataRow(gv_Team.FocusedRowHandle) == null)
		{
			toolStripMenuItem_Remove.Visible = false;
		}
		else
		{
			toolStripMenuItem_Remove.Visible = true;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.HumanResources.frm_HRTeamsSetup));
		this.grd_Team = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip_Team = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.toolStripMenuItem_Remove = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripMenuItem_Refresh = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Team = new Apzon.Controls.Grid.GridViewEx();
		this.cl_teamID = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_name = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_descriptio = new Apzon.Controls.Grid.GridColumnEx();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.grd_Team).BeginInit();
		this.contextMenuStrip_Team.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Team).BeginInit();
		base.SuspendLayout();
		this.grd_Team.AllowPasteExcel = true;
		this.grd_Team.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Team.ClipboardData = null;
		this.grd_Team.ContextMenuStrip = this.contextMenuStrip_Team;
		this.grd_Team.Location = new System.Drawing.Point(4, 0);
		this.grd_Team.MainView = this.gv_Team;
		this.grd_Team.Name = "grd_Team";
		this.grd_Team.Size = new System.Drawing.Size(580, 304);
		this.grd_Team.TabIndex = 69;
		this.grd_Team.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Team });
		this.contextMenuStrip_Team.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.toolStripMenuItem_Remove, this.toolStripMenuItem_Refresh });
		this.contextMenuStrip_Team.Name = "contextMenuStrip_Team";
		this.contextMenuStrip_Team.Size = new System.Drawing.Size(118, 48);
		this.toolStripMenuItem_Remove.Name = "toolStripMenuItem_Remove";
		this.toolStripMenuItem_Remove.Size = new System.Drawing.Size(117, 22);
		this.toolStripMenuItem_Remove.Text = "Remove";
		this.toolStripMenuItem_Remove.Click += new System.EventHandler(this.toolStripMenuItem_Remove_Click);
		this.toolStripMenuItem_Refresh.Name = "toolStripMenuItem_Refresh";
		this.toolStripMenuItem_Refresh.Size = new System.Drawing.Size(117, 22);
		this.toolStripMenuItem_Refresh.Text = "Refresh";
		this.toolStripMenuItem_Refresh.Click += new System.EventHandler(this.toolStripMenuItem_Refresh_Click);
		this.gv_Team.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Team.ColumnKey = null;
		this.gv_Team.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.cl_teamID, this.cl_name, this.cl_descriptio });
		this.gv_Team.DisableFormSetting = false;
		this.gv_Team.DisplayException = false;
		this.gv_Team.EditColumn = null;
		this.gv_Team.FieldCurrency = null;
		this.gv_Team.FieldRate = null;
		this.gv_Team.FormId = "oHRTeamsSetup";
		this.gv_Team.GridControl = this.grd_Team;
		this.gv_Team.IgnoreFilter = false;
		this.gv_Team.IndexEditRow = -1;
		this.gv_Team.IndicatorWidth = 30;
		this.gv_Team.IsApzTable = false;
		this.gv_Team.IsInsertNewRow = false;
		this.gv_Team.IsLoadingData = false;
		this.gv_Team.IsMappingContextAction = false;
		this.gv_Team.ItemId = "OHTM";
		this.gv_Team.LstColumnAllowBatch = null;
		this.gv_Team.LstColumnNotAllowBatch = null;
		this.gv_Team.LstColumnNotEditable = null;
		this.gv_Team.LstColumNotSetting = null;
		this.gv_Team.Name = "gv_Team";
		this.gv_Team.OptionsView.RowAutoHeight = true;
		this.gv_Team.OptionsView.ShowGroupPanel = false;
		this.gv_Team.UseBatchMod = false;
		this.gv_Team.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_Team_CustomDrawRowIndicator);
		this.gv_Team.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Team_CellValueChanged);
		this.gv_Team.MouseUp += new System.Windows.Forms.MouseEventHandler(this.gv_Team_MouseUp);
		this.cl_teamID.Caption = "teamID";
		this.cl_teamID.CurrencyDateField = null;
		this.cl_teamID.CurrencyField = null;
		this.cl_teamID.CurrencyFixed = null;
		this.cl_teamID.CurrencyFixedRate = 0.0;
		this.cl_teamID.CurrencyRateField = null;
		this.cl_teamID.FieldName = "teamID";
		this.cl_teamID.Name = "cl_teamID";
		this.cl_teamID.OptionsColumn.ReadOnly = true;
		this.cl_teamID.PrimaryKey = false;
		this.cl_teamID.UnitField = null;
		this.cl_name.Caption = "Team Name";
		this.cl_name.CurrencyDateField = null;
		this.cl_name.CurrencyField = null;
		this.cl_name.CurrencyFixed = null;
		this.cl_name.CurrencyFixedRate = 0.0;
		this.cl_name.CurrencyRateField = null;
		this.cl_name.FieldName = "name";
		this.cl_name.Name = "cl_name";
		this.cl_name.PrimaryKey = false;
		this.cl_name.UnitField = null;
		this.cl_name.Visible = true;
		this.cl_name.VisibleIndex = 0;
		this.cl_name.Width = 403;
		this.cl_descriptio.Caption = "Description";
		this.cl_descriptio.CurrencyDateField = null;
		this.cl_descriptio.CurrencyField = null;
		this.cl_descriptio.CurrencyFixed = null;
		this.cl_descriptio.CurrencyFixedRate = 0.0;
		this.cl_descriptio.CurrencyRateField = null;
		this.cl_descriptio.FieldName = "descriptio";
		this.cl_descriptio.Name = "cl_descriptio";
		this.cl_descriptio.PrimaryKey = false;
		this.cl_descriptio.UnitField = null;
		this.cl_descriptio.Visible = true;
		this.cl_descriptio.VisibleIndex = 1;
		this.cl_descriptio.Width = 661;
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnCancel.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btnCancel.Appearance.Options.UseBackColor = true;
		this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btnCancel.EditControl = null;
		this.btnCancel.EditValue = "Cancel";
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(71, 310);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(93, 20);
		this.btnCancel.TabIndex = 70;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Save.Appearance.Options.UseBackColor = true;
		this.btn_Save.EditControl = null;
		this.btn_Save.EditValue = "OK";
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(4, 310);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.ReadOnly = false;
		this.btn_Save.SettingInVisible = false;
		this.btn_Save.SettingReadonly = false;
		this.btn_Save.Size = new System.Drawing.Size(61, 20);
		this.btn_Save.TabIndex = 71;
		this.btn_Save.Text = "OK";
		this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
		base.AcceptButton = this.btn_Save;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btnCancel;
		base.ClientSize = new System.Drawing.Size(587, 335);
		base.Controls.Add(this.btn_Save);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.grd_Team);
		base.FormId = "oHRTeamsSetup";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_HRTeamsSetup";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Teams - Setup";
		((System.ComponentModel.ISupportInitialize)this.grd_Team).EndInit();
		this.contextMenuStrip_Team.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Team).EndInit();
		base.ResumeLayout(false);
	}
}
