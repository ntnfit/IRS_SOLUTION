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
using Apzon.Forms.Commons;
using Apzon.Forms.Helper;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Administrator;

public class frm_Project : XtraFormEx
{
	public string ProjectCode = "";

	private List<string> _oldProjectCode = new List<string>();

	private IContainer components;

	private GridControlEx grdDoc;

	private GridColumnEx gcProjectCode;

	private GridColumnEx gcProjectName;

	private GridColumnEx gcValidFrom;

	private GridColumnEx gcValidTo;

	private GridColumnEx gcActive;

	private RepositoryItemCheckEdit rItemChk_Active;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem deleteToolStripMenuItem;

	private RepositoryItemDateEdit rep_From;

	private GridViewEx gvDoc;

	private SimpleButtonEx btCancel;

	private SimpleButtonEx btOK;

	private GridColumnEx gcLocked;

	private GridColumnEx gcDataSource;

	private GridColumnEx gcUserSign;

	private GridColumnEx gcLogInstanc;

	private GridColumnEx gcUserSign2;

	private GridColumnEx gcUpdateDate;

	private RepositoryItemDateEdit rep_To;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
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

	public frm_Project()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_Project"))
		{
			InitializeComponent();
			SettingLanguage();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_Project" && x.authoriza == "F"))
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
		Text = Resources.Title_frmProject;
		gcProjectCode.Caption = Resources.lb_ProjectCode;
		gcProjectName.Caption = Resources.lb_ProjectName;
		gcValidFrom.Caption = Resources.lb_ValidFrom;
		gcValidTo.Caption = Resources.lb_ValidTo;
		gcActive.Caption = Resources.lb_Active;
		deleteToolStripMenuItem.Text = Resources.ctx_Remove;
	}

	private async void frm_Project_Load(object sender, EventArgs e)
	{
		_ = 2;
		try
		{
			Dictionary<string, GridViewEx> tblFormular = new Dictionary<string, GridViewEx> { ["OPRJ"] = gvDoc };
			await WinFuntions.LoadUdfColumns(gvDoc, "OPRJ", tblFormular);
			List<Formular> formulars = await WinFuntions.GetFormulas(63);
			gvDoc.InitFormular(formulars, tblFormular);
			await ShowData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task ShowData()
	{
		try
		{
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Project");
			DataTable dataTable = await _helper.GetDataTableAsync("GetData");
			grdDoc.DataSource = dataTable;
			grdDoc.RefreshDataSource();
			_oldProjectCode.Clear();
			if (dataTable.IsNotNull())
			{
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					_oldProjectCode.Add(Function.ToString(dataTable.Rows[i]["PrjCode"]));
				}
			}
			AddNewRow();
			rep_From.Mask.MaskType = MaskType.DateTime;
			rep_From.Mask.EditMask = GlobalData.DateTimeFormat;
			rep_From.DisplayFormat.FormatType = FormatType.DateTime;
			rep_From.DisplayFormat.FormatString = GlobalData.DateTimeFormat;
			rep_To.Mask.MaskType = MaskType.DateTime;
			rep_To.Mask.EditMask = GlobalData.DateTimeFormat;
			rep_To.DisplayFormat.FormatType = FormatType.DateTime;
			rep_To.DisplayFormat.FormatString = GlobalData.DateTimeFormat;
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void AddNewRow()
	{
		try
		{
			DataTable dataTable = grdDoc.DataSource as DataTable;
			if (!dataTable.IsNotNull())
			{
				dataTable = GetDtAddNew();
			}
			dataTable.AsEnumerable().Any((DataRow r) => string.IsNullOrEmpty(Function.ToString(r["PrjCode"])));
			DataRow dataRow = dataTable.NewRow();
			dataRow["Locked"] = "N";
			dataRow["DataSource"] = "N";
			dataRow["UserSign"] = GlobalData.UserID;
			dataRow["Active"] = "Y";
			dataRow["LogInstanc"] = 0;
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			grdDoc.DataSource = dataTable;
			grdDoc.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private DataTable GetDtAddNew()
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons");
			DataTable dataTable = httpClientHelper.Get("GetAddNewRow", new
			{
				tableName = "OPRJ",
				codeMax = 0
			});
			if (dataTable.IsNotNull())
			{
				return dataTable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	private async Task<int> GetUserSignByUserCodeOUSR()
	{
		string userCodeLogin = GlobalData.UserCodeLogin;
		try
		{
			using HttpClientHelper<UsersImpl> _helper = new HttpClientHelper<UsersImpl>(Configuration.ApiServer, "Project");
			DataTable dataTable = await _helper.GetDataTableAsync("GetUserSignByUserCodeOUSR", new
			{
				madk = userCodeLogin
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

	private void btCancel_Click(object sender, EventArgs e)
	{
		ProjectCode = "";
		Close();
	}

	private async void btOK_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
				return;
			}
			btOK.Enabled = false;
			await Save();
			btOK.Enabled = true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task Save()
	{
		_ = 1;
		try
		{
			WinCommons._OpenProcessing();
			DataTable dataTable = grdDoc.DataSource as DataTable;
			if (dataTable.IsNotNull())
			{
				DataTable dataTable2 = dataTable.Copy();
				DataRow[] array = (from t in dataTable2.AsEnumerable()
					where !string.IsNullOrEmpty(Function.ToString(t["PrjCode"]))
					select t).ToArray();
				if (array.Length != 0)
				{
					dataTable2 = array.CopyToDataTable();
				}
				List<IGrouping<string, DataRow>> list = (from i in dataTable2.AsEnumerable()
					group i by Function.ToString(i.Field<string>("PrjCode")).ToUpper() into gr
					where gr.Count() > 1
					select gr).ToList();
				if (list != null && list.Count > 0)
				{
					WinCommons.Error(Enums.MessageCode.ThisEntryAlreadyExistsInTable, "");
					return;
				}
				if ((from s in dataTable2.AsEnumerable()
					where Function.ParseDateTime(s["ValidFrom"]) > Function.ParseDateTime(s["ValidTo"])
					select s).ToArray().Length != 0)
				{
					WinCommons.Error(Enums.MessageCode.SpecifyValidateToDateThatIsSameOrLaterThanValidFromDate, "");
					return;
				}
				using HttpClientHelper<HttpResult> _helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "Project");
				HttpResult httpResult = await _helper.PostAsync("SaveDoc", dataTable2);
				if (string.IsNullOrEmpty(httpResult.Message))
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
					await ShowData();
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				}
			}
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			WinCommons._CloseProcessing();
		}
	}

	private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			string text = Function.ToString(gvDoc.GetFocusedRowCellValue("PrjCode"));
			using (HttpClientHelper<string> httpClientHelper = new HttpClientHelper<string>(Configuration.ApiServer, "Project"))
			{
				if (!httpClientHelper.GetBool("ExistProject", new
				{
					code = text
				}))
				{
					gvDoc.BeginUpdate();
					gvDoc.DeleteSelectedRows();
					gvDoc.EndUpdate();
					DataTable dataTable = (DataTable)grdDoc.DataSource;
					dataTable.AcceptChanges();
					grdDoc.DataSource = dataTable;
					grdDoc.RefreshDataSource();
					return;
				}
			}
			using HttpClientHelper<string> httpClientHelper2 = new HttpClientHelper<string>(Configuration.ApiServer, "Project");
			string text2 = httpClientHelper2.Get("CheckExistCode", new
			{
				code = text
			});
			if (string.IsNullOrEmpty(text2))
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmRemoveProject, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes && string.IsNullOrEmpty(httpClientHelper2.Post("DeleteProject", text)))
				{
					gvDoc.BeginUpdate();
					gvDoc.DeleteSelectedRows();
					gvDoc.EndUpdate();
					DataTable dataTable2 = (DataTable)grdDoc.DataSource;
					dataTable2.AcceptChanges();
					grdDoc.DataSource = dataTable2;
					grdDoc.RefreshDataSource();
					WinCommons.Success(Enums.MessageCode.RemoveSuccess, "");
				}
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, text2);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		if ((msg.Msg == 256 || msg.Msg == 260) && keyData == Keys.Return)
		{
			btOK.Focus();
			btOK_Click(null, null);
		}
		return base.ProcessCmdKey(ref msg, keyData);
	}

	private void gvDoc_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void gvDoc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			FormMode = Enums.FormViewModeType.Update;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gvDoc_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			ProjectCode = ((gvDoc.GetFocusedRowCellValue("PrjCode") == DBNull.Value || gvDoc.GetFocusedRowCellValue("PrjCode") == null) ? "" : gvDoc.GetFocusedRowCellValue("PrjCode").ToString());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task REFRESH_DATA()
	{
		await ShowData();
	}

	private async void grdDoc_EditorKeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.V && e.Modifiers == Keys.Control)
			{
				ExtenseionMethods.PasteFromExcelToDataTable(grdDoc, gvDoc, new List<string> { "PrjCode" });
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvDoc_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (gvDoc.FocusedColumn.FieldName == "PrjCode")
			{
				string item = Function.ToString(gvDoc.GetFocusedRowCellValue("PrjCode")).Trim();
				if (_oldProjectCode.Contains(item))
				{
					e.Cancel = true;
				}
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Administrator.frm_Project));
		this.grdDoc = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gvDoc = new Apzon.Controls.Grid.GridViewEx();
		this.gcProjectCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gcProjectName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcValidFrom = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_From = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.gcValidTo = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_To = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.gcActive = new Apzon.Controls.Grid.GridColumnEx();
		this.rItemChk_Active = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gcLocked = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDataSource = new Apzon.Controls.Grid.GridColumnEx();
		this.gcUserSign = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLogInstanc = new Apzon.Controls.Grid.GridColumnEx();
		this.gcUserSign2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gcUpdateDate = new Apzon.Controls.Grid.GridColumnEx();
		this.btCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btOK = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_From).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_From.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_To).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_To.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rItemChk_Active).BeginInit();
		base.SuspendLayout();
		this.grdDoc.AllowPasteExcel = true;
		this.grdDoc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grdDoc.ClipboardData = null;
		this.grdDoc.ContextMenuStrip = this.contextMenuStrip1;
		this.grdDoc.Location = new System.Drawing.Point(2, 2);
		this.grdDoc.MainView = this.gvDoc;
		this.grdDoc.Name = "grdDoc";
		this.grdDoc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.rItemChk_Active, this.rep_From, this.rep_To });
		this.grdDoc.Size = new System.Drawing.Size(519, 264);
		this.grdDoc.TabIndex = 7;
		this.grdDoc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvDoc });
		this.grdDoc.EditorKeyDown += new System.Windows.Forms.KeyEventHandler(this.grdDoc_EditorKeyDown);
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.deleteToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(118, 26);
		this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
		this.deleteToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
		this.deleteToolStripMenuItem.Text = "Remove";
		this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
		this.gvDoc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvDoc.ColumnKey = null;
		this.gvDoc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[11]
		{
			this.gcProjectCode, this.gcProjectName, this.gcValidFrom, this.gcValidTo, this.gcActive, this.gcLocked, this.gcDataSource, this.gcUserSign, this.gcLogInstanc, this.gcUserSign2,
			this.gcUpdateDate
		});
		this.gvDoc.DisableFormSetting = false;
		this.gvDoc.DisplayException = false;
		this.gvDoc.EditColumn = null;
		this.gvDoc.FieldCurrency = null;
		this.gvDoc.FieldRate = null;
		this.gvDoc.FormId = "oProject";
		this.gvDoc.GridControl = this.grdDoc;
		this.gvDoc.IgnoreFilter = false;
		this.gvDoc.IndexEditRow = -1;
		this.gvDoc.IndicatorWidth = 30;
		this.gvDoc.IsApzTable = false;
		this.gvDoc.IsInsertNewRow = false;
		this.gvDoc.IsLoadingData = false;
		this.gvDoc.IsMappingContextAction = false;
		this.gvDoc.ItemId = "OPRJ";
		this.gvDoc.LstColumnAllowBatch = null;
		this.gvDoc.LstColumnNotAllowBatch = null;
		this.gvDoc.LstColumnNotEditable = null;
		this.gvDoc.LstColumNotSetting = null;
		this.gvDoc.Name = "gvDoc";
		this.gvDoc.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowForFocusedCell;
		this.gvDoc.OptionsView.ShowGroupPanel = false;
		this.gvDoc.UseBatchMod = false;
		this.gvDoc.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gvDoc_CustomDrawRowIndicator);
		this.gvDoc.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gvDoc_ShowingEditor);
		this.gvDoc.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gvDoc_FocusedRowObjectChanged);
		this.gvDoc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvDoc_CellValueChanged);
		this.gcProjectCode.Caption = "Project Code";
		this.gcProjectCode.CurrencyDateField = null;
		this.gcProjectCode.CurrencyField = null;
		this.gcProjectCode.CurrencyFixed = null;
		this.gcProjectCode.CurrencyFixedRate = 0.0;
		this.gcProjectCode.CurrencyRateField = null;
		this.gcProjectCode.FieldName = "PrjCode";
		this.gcProjectCode.Name = "gcProjectCode";
		this.gcProjectCode.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
		this.gcProjectCode.PrimaryKey = true;
		this.gcProjectCode.UnitField = null;
		this.gcProjectCode.Visible = true;
		this.gcProjectCode.VisibleIndex = 0;
		this.gcProjectCode.Width = 131;
		this.gcProjectName.Caption = "Project Name";
		this.gcProjectName.CurrencyDateField = null;
		this.gcProjectName.CurrencyField = null;
		this.gcProjectName.CurrencyFixed = null;
		this.gcProjectName.CurrencyFixedRate = 0.0;
		this.gcProjectName.CurrencyRateField = null;
		this.gcProjectName.FieldName = "PrjName";
		this.gcProjectName.Name = "gcProjectName";
		this.gcProjectName.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
		this.gcProjectName.PrimaryKey = false;
		this.gcProjectName.UnitField = null;
		this.gcProjectName.Visible = true;
		this.gcProjectName.VisibleIndex = 1;
		this.gcProjectName.Width = 129;
		this.gcValidFrom.Caption = "Valid From";
		this.gcValidFrom.ColumnEdit = this.rep_From;
		this.gcValidFrom.CurrencyDateField = null;
		this.gcValidFrom.CurrencyField = null;
		this.gcValidFrom.CurrencyFixed = null;
		this.gcValidFrom.CurrencyFixedRate = 0.0;
		this.gcValidFrom.CurrencyRateField = null;
		this.gcValidFrom.FieldName = "ValidFrom";
		this.gcValidFrom.Name = "gcValidFrom";
		this.gcValidFrom.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
		this.gcValidFrom.PrimaryKey = false;
		this.gcValidFrom.UnitField = null;
		this.gcValidFrom.Visible = true;
		this.gcValidFrom.VisibleIndex = 2;
		this.gcValidFrom.Width = 106;
		this.rep_From.AutoHeight = false;
		this.rep_From.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_From.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_From.Name = "rep_From";
		this.gcValidTo.Caption = "Valid To";
		this.gcValidTo.ColumnEdit = this.rep_To;
		this.gcValidTo.CurrencyDateField = null;
		this.gcValidTo.CurrencyField = null;
		this.gcValidTo.CurrencyFixed = null;
		this.gcValidTo.CurrencyFixedRate = 0.0;
		this.gcValidTo.CurrencyRateField = null;
		this.gcValidTo.FieldName = "ValidTo";
		this.gcValidTo.Name = "gcValidTo";
		this.gcValidTo.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
		this.gcValidTo.PrimaryKey = false;
		this.gcValidTo.UnitField = null;
		this.gcValidTo.Visible = true;
		this.gcValidTo.VisibleIndex = 3;
		this.gcValidTo.Width = 112;
		this.rep_To.AutoHeight = false;
		this.rep_To.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_To.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_To.Name = "rep_To";
		this.gcActive.Caption = "Active";
		this.gcActive.ColumnEdit = this.rItemChk_Active;
		this.gcActive.CurrencyDateField = null;
		this.gcActive.CurrencyField = null;
		this.gcActive.CurrencyFixed = null;
		this.gcActive.CurrencyFixedRate = 0.0;
		this.gcActive.CurrencyRateField = null;
		this.gcActive.FieldName = "Active";
		this.gcActive.Name = "gcActive";
		this.gcActive.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
		this.gcActive.PrimaryKey = false;
		this.gcActive.UnitField = null;
		this.gcActive.Visible = true;
		this.gcActive.VisibleIndex = 4;
		this.gcActive.Width = 43;
		this.rItemChk_Active.AutoHeight = false;
		this.rItemChk_Active.Name = "rItemChk_Active";
		this.rItemChk_Active.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Inactive;
		this.rItemChk_Active.ValueChecked = "Y";
		this.rItemChk_Active.ValueUnchecked = "N";
		this.gcLocked.Caption = "Locked";
		this.gcLocked.CurrencyDateField = null;
		this.gcLocked.CurrencyField = null;
		this.gcLocked.CurrencyFixed = null;
		this.gcLocked.CurrencyFixedRate = 0.0;
		this.gcLocked.CurrencyRateField = null;
		this.gcLocked.FieldName = "Locked";
		this.gcLocked.Name = "gcLocked";
		this.gcLocked.PrimaryKey = false;
		this.gcLocked.UnitField = null;
		this.gcDataSource.Caption = "DataSource";
		this.gcDataSource.CurrencyDateField = null;
		this.gcDataSource.CurrencyField = null;
		this.gcDataSource.CurrencyFixed = null;
		this.gcDataSource.CurrencyFixedRate = 0.0;
		this.gcDataSource.CurrencyRateField = null;
		this.gcDataSource.FieldName = "DataSource";
		this.gcDataSource.Name = "gcDataSource";
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
		this.gcUserSign.PrimaryKey = false;
		this.gcUserSign.UnitField = null;
		this.gcLogInstanc.Caption = "LogInstanc";
		this.gcLogInstanc.CurrencyDateField = null;
		this.gcLogInstanc.CurrencyField = null;
		this.gcLogInstanc.CurrencyFixed = null;
		this.gcLogInstanc.CurrencyFixedRate = 0.0;
		this.gcLogInstanc.CurrencyRateField = null;
		this.gcLogInstanc.FieldName = "LogInstanc";
		this.gcLogInstanc.Name = "gcLogInstanc";
		this.gcLogInstanc.PrimaryKey = false;
		this.gcLogInstanc.UnitField = null;
		this.gcUserSign2.Caption = "UserSign2";
		this.gcUserSign2.CurrencyDateField = null;
		this.gcUserSign2.CurrencyField = null;
		this.gcUserSign2.CurrencyFixed = null;
		this.gcUserSign2.CurrencyFixedRate = 0.0;
		this.gcUserSign2.CurrencyRateField = null;
		this.gcUserSign2.FieldName = "UserSign2";
		this.gcUserSign2.Name = "gcUserSign2";
		this.gcUserSign2.PrimaryKey = false;
		this.gcUserSign2.UnitField = null;
		this.gcUpdateDate.Caption = "UpdateDate";
		this.gcUpdateDate.CurrencyDateField = null;
		this.gcUpdateDate.CurrencyField = null;
		this.gcUpdateDate.CurrencyFixed = null;
		this.gcUpdateDate.CurrencyFixedRate = 0.0;
		this.gcUpdateDate.CurrencyRateField = null;
		this.gcUpdateDate.FieldName = "UpdateDate";
		this.gcUpdateDate.Name = "gcUpdateDate";
		this.gcUpdateDate.PrimaryKey = false;
		this.gcUpdateDate.UnitField = null;
		this.btCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btCancel.EditControl = null;
		this.btCancel.EditValue = "Cancel";
		this.btCancel.FieldName = null;
		this.btCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btCancel.ImageOptions.Image");
		this.btCancel.IsSearchControl = false;
		this.btCancel.Location = new System.Drawing.Point(80, 270);
		this.btCancel.Name = "btCancel";
		this.btCancel.ReadOnly = false;
		this.btCancel.SettingInVisible = false;
		this.btCancel.SettingReadonly = false;
		this.btCancel.Size = new System.Drawing.Size(75, 23);
		this.btCancel.TabIndex = 6;
		this.btCancel.Text = "Cancel";
		this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
		this.btOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btOK.EditControl = null;
		this.btOK.EditValue = "OK";
		this.btOK.FieldName = null;
		this.btOK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btOK.ImageOptions.Image");
		this.btOK.IsSearchControl = false;
		this.btOK.Location = new System.Drawing.Point(2, 270);
		this.btOK.Name = "btOK";
		this.btOK.ReadOnly = false;
		this.btOK.SettingInVisible = false;
		this.btOK.SettingReadonly = false;
		this.btOK.Size = new System.Drawing.Size(75, 23);
		this.btOK.TabIndex = 5;
		this.btOK.Text = "OK";
		this.btOK.Click += new System.EventHandler(this.btOK_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(523, 296);
		this.ContextMenuStrip = this.contextMenuStrip1;
		base.Controls.Add(this.grdDoc);
		base.Controls.Add(this.btCancel);
		base.Controls.Add(this.btOK);
		base.FormId = "oProject";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_Project";
		this.Text = "Project - Setup";
		base.Load += new System.EventHandler(this.frm_Project_Load);
		((System.ComponentModel.ISupportInitialize)this.grdDoc).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gvDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_From.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_From).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_To.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_To).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rItemChk_Active).EndInit();
		base.ResumeLayout(false);
	}
}
