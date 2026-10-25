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
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.HumanResources;

public class frm_Qualifications : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private const string FormController = "HRMQualifications";

	protected Enums.FormAuthorities Authoritues = Enums.FormAuthorities.Read;

	private readonly List<int> _listQua = new List<int>();

	private string QuaCodes = string.Empty;

	public int QuaId;

	private IContainer components;

	public GridControlEx grd_Quanlifications;

	public GridViewEx gv_Qualifications;

	public GridColumnEx cl_QuaCode;

	public GridColumnEx cl_Name;

	public GridColumnEx cl_QuaGroup;

	public GridColumnEx cl_Description;

	protected RepositoryItemLookUpEdit rlup_Qua;

	public SimpleButtonEx btn_Save;

	public SimpleButtonEx btnCancel;

	private ContextMenuStrip contextMenuStrip_Qualifications;

	private ToolStripMenuItem toolStripMenuItem_Remove;

	private RepositoryItemTextEdit quaCodeRep;

	private RepositoryItemTextEdit repositoryItemTextEdit1;

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
		}
	}

	public frm_Qualifications()
	{
		InitializeComponent();
		FormId = "oQualifications";
		gv_Qualifications.FormId = FormId;
		gv_Qualifications.ItemId = "APZ_OHQU";
		OnLoadAsync += frm_Qualifications_Load;
	}

	private async Task frm_Qualifications_Load(object sender, EventArgs e)
	{
		_ = 2;
		try
		{
			await LoadLookUp();
			await RefreshQuanlifications();
			if (QuaId > 0)
			{
				await GetDetails(QuaId);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private async void btnCancel_Click(object sender, EventArgs e)
	{
		Close();
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
				WinCommons._OpenProcessing();
				btn_Save.Enabled = false;
				await SaveQuanlifications();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		btn_Save.Enabled = true;
		WinCommons._CloseProcessing();
	}

	private async void gv_Doc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			int num = 0;
			for (int i = 0; i < gv_Qualifications.RowCount; i++)
			{
				num = gv_Qualifications.GetVisibleRowHandle(i);
			}
			string text = Function.ToString(gv_Qualifications.GetRowCellValue(e.RowHandle, "quacode"));
			if (e.Column.FieldName == "quacode" && text != "" && e.RowHandle == num)
			{
				AddNewRowQuanlifications();
			}
			if (FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
			FormMode = Enums.FormViewModeType.Update;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator && e.RowHandle != -2147483647)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void toolStripMenuItem_Remove_Click(object sender, EventArgs e)
	{
		try
		{
			int num = Function.ParseInt(gv_Qualifications.GetFocusedRowCellValue("QuaCode"));
			if (num > 0 && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmRemoveItem, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				RemoveQuanlifications(num);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rlup_Qua_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (e.NewValue != null && Function.ParseInt(e.NewValue) == -1)
			{
				frm_QualificationsGroup frm_QualificationsGroup2 = new frm_QualificationsGroup
				{
					MdiParent = MdiParent
				};
				frm_QualificationsGroup2.RefreshQuaGroup = (RefreshLookupQuaGroup)Delegate.Combine(frm_QualificationsGroup2.RefreshQuaGroup, new RefreshLookupQuaGroup(LoadLookUpQua));
				frm_QualificationsGroup2.Show();
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Qualifications_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		if (FormMode == Enums.FormViewModeType.View)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			LoadLookUp();
			await RefreshQuanlifications();
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task SaveQuanlifications()
	{
		_ = 1;
		try
		{
			DataTable dataTable = (grd_Quanlifications.DataSource as DataTable).Copy();
			if (dataTable.IsNotNull())
			{
				DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["QuaCode"]) == 0);
				if (string.IsNullOrEmpty(Function.ToString(dataRow["name"])) && string.IsNullOrEmpty(Function.ToString(dataRow["Description"])))
				{
					dataTable.Rows.Remove(dataRow);
					dataTable.AcceptChanges();
				}
				if ((from t in dataTable.AsEnumerable()
					select Function.ToString(t["Name"])).Distinct().Count() != dataTable.Rows.Count)
				{
					WinCommons.Error(Enums.MessageCode.ThisEntryAlreadyExistsInTable, "");
				}
				else
				{
					if (!dataTable.IsNotNull())
					{
						return;
					}
					if (dataTable.AsEnumerable().FirstOrDefault((DataRow t) => string.IsNullOrEmpty(Function.ToString(t["Name"]))) != null)
					{
						WinCommons.Error(Enums.MessageCode.NameCannotBeEmpty, "");
						return;
					}
					dataTable.AcceptChanges();
					using HttpClientHelper<HttpResult> _dtHelp = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "HRMQualifications");
					HttpResult httpResult = await _dtHelp.PostAsync("SaveQualifications", dataTable);
					if (string.IsNullOrEmpty(Function.ToString(httpResult.Content)))
					{
						WinCommons.Success(Enums.MessageCode.Success, "");
						FormMode = Enums.FormViewModeType.View;
						await RefreshQuanlifications();
					}
					else
					{
						WinCommons.Error(Enums.MessageCode.None, Function.ToString(httpResult.Content));
					}
				}
			}
			else
			{
				grd_Quanlifications.DataSource = dataTable.Clone();
				grd_Quanlifications.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void RemoveQuanlifications(int QuaCodes)
	{
		try
		{
			using HttpClientHelper<string> hp = new HttpClientHelper<string>(Configuration.ApiServer, "HRMQualifications");
			string text = hp.Post("RemoveQualifications", QuaCodes);
			if (string.IsNullOrWhiteSpace(text))
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
				gv_Qualifications.DeleteSelectedRows();
				await RefreshQuanlifications();
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

	private async Task RefreshQuanlifications()
	{
		try
		{
			GridControlEx gridControlEx = grd_Quanlifications;
			gridControlEx.DataSource = await GetNewQuanlifications();
			grd_Quanlifications.DataSourceTable.AcceptChanges();
			grd_Quanlifications.RefreshDataSource();
			FormMode = Enums.FormViewModeType.View;
			AddNewRowQuanlifications();
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task<DataTable> GetNewQuanlifications()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "HRMQualifications");
			return await helper.GetAsync("GetQualifications");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	private void AddNewRowQuanlifications()
	{
		try
		{
			gv_Qualifications.BeginInsertRow();
			DataTable dataTable = grd_Quanlifications.DataSource as DataTable;
			if (dataTable.IsNotNull())
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["quacode"] = 0;
				dataRow["QuaGroup"] = 0;
				dataRow["Name"] = "";
				dataRow["Description"] = "";
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
				grd_Quanlifications.DataSource = dataTable;
				grd_Quanlifications.RefreshDataSource();
			}
			else
			{
				DataTable tableStructure = WinFuntions.GetTableStructure("APZ_OHQU");
				DataRow dataRow2 = tableStructure.NewRow();
				dataRow2["quacode"] = 0;
				dataRow2["QuaGroup"] = 0;
				dataRow2["Name"] = "";
				dataRow2["Description"] = "";
				tableStructure.Rows.Add(dataRow2);
				tableStructure.AcceptChanges();
				grd_Quanlifications.DataSource = tableStructure;
				grd_Quanlifications.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			gv_Qualifications.EndInsertRow();
		}
	}

	private async void LoadLookUpQua()
	{
		try
		{
			_ = grd_Quanlifications.DataSourceTable;
			DataTable dataTable = await _httpClient.EmployeeMasterClient.LoadLookUpQua();
			if (dataTable == null || dataTable.Columns.Count <= 0)
			{
				dataTable = new DataTable();
				dataTable.Columns.Add("Id", typeof(int));
				dataTable.Columns.Add("Name", typeof(string));
			}
			DataRow dataRow = dataTable.NewRow();
			dataRow["Id"] = -1;
			dataRow["Name"] = "Define New";
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			rlup_Qua.DataSource = dataTable.Copy();
			rlup_Qua.DisplayMember = "Name";
			rlup_Qua.ValueMember = "Id";
			rlup_Qua.ForceInitialize();
			rlup_Qua.PopulateColumns();
			rlup_Qua.Columns["Id"].Visible = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadLookUp()
	{
		try
		{
			LoadLookUpQua();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task GetDetails(int quaId)
	{
		try
		{
			GridControlEx gridControlEx = grd_Quanlifications;
			gridControlEx.DataSource = await GetQuaCode(quaId);
			grd_Quanlifications.DataSourceTable.AcceptChanges();
			grd_Quanlifications.RefreshDataSource();
			AddNewRowQuanlifications();
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<DataTable> GetQuaCode(int quaId)
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "HRMQualifications");
			return await helper.GetAsync("GetQuaCode", new { quaId });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	private async void frm_Qualifications_FormClosing(object sender, FormClosingEventArgs e)
	{
		if (FormMode == Enums.FormViewModeType.Update && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSaveBeforeClose, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
		{
			WinCommons._OpenProcessing();
			btn_Save.Enabled = false;
			await SaveQuanlifications();
			btn_Save.Enabled = true;
			WinCommons._CloseProcessing();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.HumanResources.frm_Qualifications));
		this.grd_Quanlifications = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip_Qualifications = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.toolStripMenuItem_Remove = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Qualifications = new Apzon.Controls.Grid.GridViewEx();
		this.cl_QuaCode = new Apzon.Controls.Grid.GridColumnEx();
		this.quaCodeRep = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.cl_Name = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_QuaGroup = new Apzon.Controls.Grid.GridColumnEx();
		this.rlup_Qua = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.cl_Description = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemTextEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.grd_Quanlifications).BeginInit();
		this.contextMenuStrip_Qualifications.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Qualifications).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.quaCodeRep).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rlup_Qua).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEdit1).BeginInit();
		base.SuspendLayout();
		this.grd_Quanlifications.AllowPasteExcel = true;
		this.grd_Quanlifications.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Quanlifications.ClipboardData = null;
		this.grd_Quanlifications.ContextMenuStrip = this.contextMenuStrip_Qualifications;
		this.grd_Quanlifications.Location = new System.Drawing.Point(4, 4);
		this.grd_Quanlifications.MainView = this.gv_Qualifications;
		this.grd_Quanlifications.Name = "grd_Quanlifications";
		this.grd_Quanlifications.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.rlup_Qua, this.quaCodeRep, this.repositoryItemTextEdit1 });
		this.grd_Quanlifications.Size = new System.Drawing.Size(514, 297);
		this.grd_Quanlifications.TabIndex = 3;
		this.grd_Quanlifications.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Qualifications });
		this.contextMenuStrip_Qualifications.ImageScalingSize = new System.Drawing.Size(20, 20);
		this.contextMenuStrip_Qualifications.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.toolStripMenuItem_Remove });
		this.contextMenuStrip_Qualifications.Name = "contextMenuStrip_Qualifications";
		this.contextMenuStrip_Qualifications.Size = new System.Drawing.Size(118, 26);
		this.toolStripMenuItem_Remove.Name = "toolStripMenuItem_Remove";
		this.toolStripMenuItem_Remove.Size = new System.Drawing.Size(117, 22);
		this.toolStripMenuItem_Remove.Text = "Remove";
		this.toolStripMenuItem_Remove.Click += new System.EventHandler(this.toolStripMenuItem_Remove_Click);
		this.gv_Qualifications.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Qualifications.ColumnKey = null;
		this.gv_Qualifications.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[4] { this.cl_QuaCode, this.cl_Name, this.cl_QuaGroup, this.cl_Description });
		this.gv_Qualifications.DisableFormSetting = false;
		this.gv_Qualifications.DisplayException = false;
		this.gv_Qualifications.EditColumn = null;
		this.gv_Qualifications.FieldCurrency = null;
		this.gv_Qualifications.FieldRate = null;
		this.gv_Qualifications.FormId = "oQualifications";
		this.gv_Qualifications.GridControl = this.grd_Quanlifications;
		this.gv_Qualifications.IgnoreFilter = false;
		this.gv_Qualifications.IndexEditRow = -1;
		this.gv_Qualifications.IndicatorWidth = 60;
		this.gv_Qualifications.IsApzTable = false;
		this.gv_Qualifications.IsInsertNewRow = false;
		this.gv_Qualifications.IsLoadingData = false;
		this.gv_Qualifications.IsMappingContextAction = false;
		this.gv_Qualifications.ItemId = "APZ_OHQU";
		this.gv_Qualifications.LstColumnAllowBatch = null;
		this.gv_Qualifications.LstColumnNotAllowBatch = null;
		this.gv_Qualifications.LstColumnNotEditable = null;
		this.gv_Qualifications.LstColumNotSetting = null;
		this.gv_Qualifications.Name = "gv_Qualifications";
		this.gv_Qualifications.OptionsView.ColumnAutoWidth = false;
		this.gv_Qualifications.OptionsView.ShowGroupPanel = false;
		this.gv_Qualifications.UseBatchMod = false;
		this.gv_Qualifications.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_Doc_CustomDrawRowIndicator);
		this.gv_Qualifications.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Doc_CellValueChanged);
		this.gv_Qualifications.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Qualifications_CellValueChanging);
		this.cl_QuaCode.Caption = "QuaCode";
		this.cl_QuaCode.ColumnEdit = this.quaCodeRep;
		this.cl_QuaCode.CurrencyDateField = null;
		this.cl_QuaCode.CurrencyField = null;
		this.cl_QuaCode.CurrencyFixed = null;
		this.cl_QuaCode.CurrencyFixedRate = 0.0;
		this.cl_QuaCode.CurrencyRateField = null;
		this.cl_QuaCode.FieldName = "QuaCode";
		this.cl_QuaCode.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.cl_QuaCode.Name = "cl_QuaCode";
		this.cl_QuaCode.OptionsColumn.ReadOnly = true;
		this.cl_QuaCode.PrimaryKey = true;
		this.cl_QuaCode.UnitField = null;
		this.cl_QuaCode.Width = 109;
		this.quaCodeRep.AutoHeight = false;
		this.quaCodeRep.Name = "quaCodeRep";
		this.cl_Name.Caption = "Name";
		this.cl_Name.CurrencyDateField = null;
		this.cl_Name.CurrencyField = null;
		this.cl_Name.CurrencyFixed = null;
		this.cl_Name.CurrencyFixedRate = 0.0;
		this.cl_Name.CurrencyRateField = null;
		this.cl_Name.FieldName = "Name";
		this.cl_Name.Name = "cl_Name";
		this.cl_Name.PrimaryKey = true;
		this.cl_Name.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.cl_Name.UnitField = null;
		this.cl_Name.Visible = true;
		this.cl_Name.VisibleIndex = 0;
		this.cl_Name.Width = 129;
		this.cl_QuaGroup.AppearanceHeader.Options.UseTextOptions = true;
		this.cl_QuaGroup.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.cl_QuaGroup.Caption = "Qualifications Group";
		this.cl_QuaGroup.ColumnEdit = this.rlup_Qua;
		this.cl_QuaGroup.CurrencyDateField = null;
		this.cl_QuaGroup.CurrencyField = null;
		this.cl_QuaGroup.CurrencyFixed = null;
		this.cl_QuaGroup.CurrencyFixedRate = 0.0;
		this.cl_QuaGroup.CurrencyRateField = null;
		this.cl_QuaGroup.FieldName = "QuaGroup";
		this.cl_QuaGroup.Name = "cl_QuaGroup";
		this.cl_QuaGroup.PrimaryKey = false;
		this.cl_QuaGroup.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.cl_QuaGroup.UnitField = null;
		this.cl_QuaGroup.Visible = true;
		this.cl_QuaGroup.VisibleIndex = 1;
		this.cl_QuaGroup.Width = 151;
		this.rlup_Qua.AcceptEditorTextAsNewValue = DevExpress.Utils.DefaultBoolean.True;
		this.rlup_Qua.AutoHeight = false;
		this.rlup_Qua.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rlup_Qua.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name1")
		});
		this.rlup_Qua.DisplayMember = "Name";
		this.rlup_Qua.Name = "rlup_Qua";
		this.rlup_Qua.NullText = "";
		this.rlup_Qua.ValueMember = "Id";
		this.rlup_Qua.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.rlup_Qua_EditValueChanging);
		this.cl_Description.Caption = "Description";
		this.cl_Description.CurrencyDateField = null;
		this.cl_Description.CurrencyField = null;
		this.cl_Description.CurrencyFixed = null;
		this.cl_Description.CurrencyFixedRate = 0.0;
		this.cl_Description.CurrencyRateField = null;
		this.cl_Description.FieldName = "Description";
		this.cl_Description.Name = "cl_Description";
		this.cl_Description.PrimaryKey = false;
		this.cl_Description.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.cl_Description.UnitField = null;
		this.cl_Description.Visible = true;
		this.cl_Description.VisibleIndex = 2;
		this.cl_Description.Width = 171;
		this.repositoryItemTextEdit1.AutoHeight = false;
		this.repositoryItemTextEdit1.Name = "repositoryItemTextEdit1";
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Save.Appearance.Options.UseBackColor = true;
		this.btn_Save.EditControl = null;
		this.btn_Save.EditValue = "OK";
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(4, 307);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.ReadOnly = false;
		this.btn_Save.SettingInVisible = false;
		this.btn_Save.SettingReadonly = false;
		this.btn_Save.Size = new System.Drawing.Size(61, 20);
		this.btn_Save.TabIndex = 76;
		this.btn_Save.Text = "OK";
		this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnCancel.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btnCancel.Appearance.Options.UseBackColor = true;
		this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btnCancel.EditControl = null;
		this.btnCancel.EditValue = "Cancel";
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(68, 307);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(93, 20);
		this.btnCancel.TabIndex = 75;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		base.AcceptButton = this.btn_Save;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btnCancel;
		base.ClientSize = new System.Drawing.Size(523, 333);
		base.Controls.Add(this.btn_Save);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.grd_Quanlifications);
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_Qualifications";
		this.Text = "Qualifications";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_Qualifications_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.grd_Quanlifications).EndInit();
		this.contextMenuStrip_Qualifications.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Qualifications).EndInit();
		((System.ComponentModel.ISupportInitialize)this.quaCodeRep).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rlup_Qua).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEdit1).EndInit();
		base.ResumeLayout(false);
	}
}
