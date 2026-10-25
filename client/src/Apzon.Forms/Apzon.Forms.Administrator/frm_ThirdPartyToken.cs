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
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Editor.MyLookUpEdit;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.Administrator;

public class frm_ThirdPartyToken : XtraFormEx
{
	private const string OkTitle = "Ok";

	private const string SaveTitle = "Save";

	private const string UpdateTitle = "Update";

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private IContainer components;

	private LabelLine lb_Status;

	private TextEditEx txtTokenName;

	private LabelLine lb_Remarks;

	private TextEditEx txtRemark;

	private SimpleButtonEx btnSearch;

	private GridControlEx grdDoc;

	private GridViewEx grdTpt;

	private GridColumnEx gcAbsEntry;

	private GridColumnEx gcTokenName;

	private GridColumnEx gcRemarks;

	private GridColumnEx gcStatus;

	private GridColumnEx gcToken;

	private GridColumnEx gcUserName;

	private GridColumnEx gcExpiredDate;

	private RepositoryItemDateEditEx rpidateExpiredDate;

	private RepositoryItemLookUpEditSimple rpiLkuStatus;

	private SimpleButtonEx btnSave;

	private SimpleButtonEx btnCancel;

	private GridColumnEx gcPassword;

	private GridColumnEx gcDatabaseName;

	private GridColumnEx gcDomainName;

	private RepositoryItemTextEdit rpitPassword;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem addNewToolStripMenuItem;

	private ToolStripMenuItem deleteRowToolStripMenuItem;

	private RepositoryItemTextEdit rpiTUsername;

	private LookUpEditEx lkUp_Status;

	private LabelLine lb_TokenName;

	private RepositoryItemButtonEdit rpibtnGenerateToken;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		try
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
			case Enums.FormViewModeType.Add:
				btnSave.Text = "Save";
				break;
			case Enums.FormViewModeType.Update:
				btnSave.Text = "Update";
				break;
			case Enums.FormViewModeType.View:
				btnSave.Text = "Ok";
				break;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public frm_ThirdPartyToken()
	{
		InitializeComponent();
		OnLoadAsync += frm_ThirdPartyToken_Load;
		SettingLanguage();
	}

	private void SettingLanguage()
	{
	}

	private void CustomizeGridview()
	{
		deleteRowToolStripMenuItem.Visible = true;
		deleteRowToolStripMenuItem.Enabled = true;
		addNewToolStripMenuItem.Visible = true;
		addNewToolStripMenuItem.Enabled = true;
	}

	public async Task REFRESH_DATA()
	{
		await LoadData(string.Empty, string.Empty, string.Empty);
	}

	private async Task LoadData(string username, string databaseName, string status)
	{
		try
		{
			FormMode = Enums.FormViewModeType.View;
			DataTable dataTable = await _httpClient.ThirdPartyToken.LoadTokens(username, databaseName, status);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				CustomizeGridview();
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					dataTable.Rows[i]["Password"] = Function.Decrypt(RSA.PrivateKey, Function.ToString(dataTable.Rows[i]["Password"]));
				}
			}
			grdDoc.DataSource = dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task Save()
	{
		_ = 1;
		try
		{
			if (FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update)
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					string action = string.Empty;
					DataTable dataTable = new DataTable();
					if (FormMode == Enums.FormViewModeType.Update)
					{
						dataTable = grdDoc.DataSourceTable.Clone();
						DataRow focusedDataRow = grdTpt.GetFocusedDataRow();
						if (focusedDataRow != null)
						{
							dataTable.ImportRow(focusedDataRow);
							action = "U";
						}
					}
					if (FormMode == Enums.FormViewModeType.Add)
					{
						dataTable = (DataTable)grdDoc.DataSource;
					}
					if (dataTable != null && dataTable.Rows.Count > 0)
					{
						for (int i = 0; i < dataTable.Rows.Count; i++)
						{
							if (Function.ToString(dataTable.Rows[0]["Token"]) == "")
							{
								WinCommons.Warning(Enums.MessageCode.TpTokenNotNull, "");
								return;
							}
							if (Function.ToString(dataTable.Rows[0]["Username"]) == "")
							{
								WinCommons.Warning(Enums.MessageCode.UserNameCannotBeNull, "");
								return;
							}
							if (Function.ToString(dataTable.Rows[0]["Password"]) == "")
							{
								WinCommons.Warning(Enums.MessageCode.PleaseEnterPassword, "");
								return;
							}
							if (Function.ToString(dataTable.Rows[0]["DatabaseName"]) == "")
							{
								WinCommons.Warning(Enums.MessageCode.DatabaseNameNotNull, "");
								return;
							}
							if (Function.ToString(dataTable.Rows[0]["DomainName"]) == "")
							{
								WinCommons.Warning(Enums.MessageCode.DomainNameNotNull, "");
								return;
							}
							if (Function.ToString(dataTable.Rows[0]["ExpiredDate"]) != "" && Convert.ToDateTime(Function.ToString(dataTable.Rows[0]["ExpiredDate"])).Date < DateTime.Now.Date)
							{
								WinCommons.Warning(Enums.MessageCode.ExpiredDateLowerThanNow, "");
								return;
							}
							dataTable.Rows[i]["Password"] = Function.Encrypt(RSA.PublicKey, Function.ToString(dataTable.Rows[i]["Password"]));
						}
					}
					WinCommons._OpenProcessing();
					HttpResult httpResult = await _httpClient.ThirdPartyToken.SaveTokens(dataTable, action);
					if (httpResult.MessageCode == Enums.MessageCode.SaveSuccess)
					{
						WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
					}
					else
					{
						WinCommons.Error(Enums.MessageCode.Error, "", httpResult.Message);
					}
					await LoadData(txtTokenName.Text, txtRemark.Text, Function.ToString(lkUp_Status.EditValue));
				}
			}
			else
			{
				Close();
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private void DataChanged()
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task AddNewToken()
	{
		try
		{
			new DataTable();
			FormMode = Enums.FormViewModeType.Add;
			deleteRowToolStripMenuItem.Visible = false;
			DataTable _dtOriginalTpt = grdDoc.DataSourceTable;
			_dtOriginalTpt.Rows.Clear();
			HttpResult httpResult = await _httpClient.ThirdPartyToken.GetMaxAbsEntry();
			DataRow dataRow = _dtOriginalTpt.NewRow();
			dataRow["AbsEntry"] = Convert.ToInt32(httpResult.Content) + 1;
			dataRow["Status"] = "Y";
			dataRow["DatabaseName"] = GlobalData.DatabaseName;
			dataRow["DomainName"] = GlobalData.Domain;
			dataRow["Token"] = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Replace("==", "") + Convert.ToBase64String(Guid.NewGuid().ToByteArray());
			_dtOriginalTpt.Rows.Add(dataRow);
			grdDoc.DataSource = _dtOriginalTpt;
			DataChanged();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
	}

	public async Task REFRESH()
	{
		_ = 1;
		try
		{
			if (FormMode == Enums.FormViewModeType.View || FormMode == Enums.FormViewModeType.Find)
			{
				await AddNewToken();
			}
			else if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
			{
				await AddNewToken();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
	}

	private async Task DeleteRow()
	{
		_ = 1;
		try
		{
			if (grdTpt.GetSelectedRows() == null || !grdTpt.GetSelectedRows().Any())
			{
				WinCommons.Error(Enums.MessageCode.InfoNoRowSelected, "");
				return;
			}
			DialogResult dialogResult = XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDeleteSelectedRow, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo);
			string value = Function.ToString(grdTpt.GetFocusedRowCellValue("AbsEntry"));
			if (dialogResult == DialogResult.Yes)
			{
				HttpResult httpResult = await _httpClient.ThirdPartyToken.DeleteToken(Convert.ToInt32(value));
				if (httpResult.MessageCode == Enums.MessageCode.DeleteSuccess)
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				}
			}
			await LoadData(txtTokenName.Text, txtRemark.Text, Function.ToString(lkUp_Status.EditValue));
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task Search()
	{
		try
		{
			string username = Function.ToString(txtTokenName.Text);
			string databaseName = Function.ToString(txtRemark.Text);
			await LoadData(username, databaseName, Function.ToString(lkUp_Status.EditValue));
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void ConfirmClose()
	{
		if (FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update)
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSaveBeforeClose, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				await LoadData("", "", Function.ToString(lkUp_Status.EditValue));
			}
		}
		else
		{
			Close();
		}
	}

	private async Task frm_ThirdPartyToken_Load(object sender, EventArgs e)
	{
		rpiLkuStatus.SetDataSource(new Dictionary<string, string>
		{
			{ "Y", "Active" },
			{ "N", "InActive" }
		});
		lkUp_Status.SetDataSource(new Dictionary<string, string>
		{
			{ "", "All" },
			{ "Y", "Active" },
			{ "N", "In Active" }
		});
		gcExpiredDate.DisplayFormatType = Enums.DisplayFormatType.DateTime;
		await LoadData("", "", Function.ToString(lkUp_Status.EditValue));
	}

	private async void btnSave_Click(object sender, EventArgs e)
	{
		await Save();
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		ConfirmClose();
	}

	private async void btnSearch_Click(object sender, EventArgs e)
	{
		await Search();
	}

	private void grdTpt_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		DataChanged();
	}

	private async void AddNewToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			await REFRESH();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void DeleteRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		await DeleteRow();
	}

	private void grdTpt_BeforeLeaveRow(object sender, RowAllowEventArgs e)
	{
		if (FormMode == Enums.FormViewModeType.Busy || FormMode == Enums.FormViewModeType.Add)
		{
			e.Allow = false;
		}
		else if (FormMode == Enums.FormViewModeType.Update)
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
			{
				e.Allow = false;
				return;
			}
			e.Allow = true;
			FormMode = Enums.FormViewModeType.View;
		}
	}

	private void frm_ThirdPartyToken_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if ((FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSaveBeforeClose, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
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

	private void grdTpt_ShowingEditor(object sender, CancelEventArgs e)
	{
		if (Function.ToString(grdTpt.GetFocusedRowCellValue("DomainName")) != GlobalData.Domain || Function.ToString(grdTpt.GetFocusedRowCellValue("DatabaseName")) != GlobalData.DatabaseName)
		{
			e.Cancel = true;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Administrator.frm_ThirdPartyToken));
		this.lb_Status = new Apzon.Controls.Controls.LabelLine();
		this.txtTokenName = new Apzon.Controls.Editor.TextEditEx();
		this.lb_Remarks = new Apzon.Controls.Controls.LabelLine();
		this.txtRemark = new Apzon.Controls.Editor.TextEditEx();
		this.btnSearch = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grdDoc = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addNewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.grdTpt = new Apzon.Controls.Grid.GridViewEx();
		this.gcAbsEntry = new Apzon.Controls.Grid.GridColumnEx();
		this.gcTokenName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcRemarks = new Apzon.Controls.Grid.GridColumnEx();
		this.gcStatus = new Apzon.Controls.Grid.GridColumnEx();
		this.rpiLkuStatus = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.gcToken = new Apzon.Controls.Grid.GridColumnEx();
		this.rpibtnGenerateToken = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcUserName = new Apzon.Controls.Grid.GridColumnEx();
		this.rpiTUsername = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.gcPassword = new Apzon.Controls.Grid.GridColumnEx();
		this.rpitPassword = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.gcDatabaseName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDomainName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcExpiredDate = new Apzon.Controls.Grid.GridColumnEx();
		this.rpidateExpiredDate = new Apzon.Controls.Editor.RepositoryItemDateEditEx();
		this.btnSave = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.lkUp_Status = new Apzon.Controls.Editor.LookUpEditEx();
		this.lb_TokenName = new Apzon.Controls.Controls.LabelLine();
		((System.ComponentModel.ISupportInitialize)this.txtTokenName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtRemark.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdTpt).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rpiLkuStatus).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rpibtnGenerateToken).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rpiTUsername).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rpitPassword).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rpidateExpiredDate).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rpidateExpiredDate.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lkUp_Status.Properties).BeginInit();
		base.SuspendLayout();
		this.lb_Status.EditControl = null;
		this.lb_Status.EditValue = "Status";
		this.lb_Status.FieldName = null;
		this.lb_Status.HideWhenEmpty = false;
		this.lb_Status.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Status.Location = new System.Drawing.Point(295, 35);
		this.lb_Status.Name = "lb_Status";
		this.lb_Status.ReadOnly = false;
		this.lb_Status.SettingInVisible = false;
		this.lb_Status.SettingReadonly = false;
		this.lb_Status.ShowUnderLine = true;
		this.lb_Status.Size = new System.Drawing.Size(43, 20);
		this.lb_Status.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Status.TabIndex = 4;
		this.lb_Status.Utext = "Status";
		this.txtTokenName.EditControl = null;
		this.txtTokenName.FieldName = null;
		this.txtTokenName.IsSearchControl = false;
		this.txtTokenName.Location = new System.Drawing.Point(99, 8);
		this.txtTokenName.Name = "txtTokenName";
		this.txtTokenName.SettingInVisible = false;
		this.txtTokenName.SettingReadonly = false;
		this.txtTokenName.Size = new System.Drawing.Size(190, 20);
		this.txtTokenName.TabIndex = 6;
		this.lb_Remarks.EditControl = null;
		this.lb_Remarks.EditValue = "Remarks";
		this.lb_Remarks.FieldName = null;
		this.lb_Remarks.HideWhenEmpty = false;
		this.lb_Remarks.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Remarks.Location = new System.Drawing.Point(13, 38);
		this.lb_Remarks.Name = "lb_Remarks";
		this.lb_Remarks.ReadOnly = false;
		this.lb_Remarks.SettingInVisible = false;
		this.lb_Remarks.SettingReadonly = false;
		this.lb_Remarks.ShowUnderLine = true;
		this.lb_Remarks.Size = new System.Drawing.Size(80, 20);
		this.lb_Remarks.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Remarks.TabIndex = 10;
		this.lb_Remarks.Utext = "Remarks";
		this.txtRemark.EditControl = null;
		this.txtRemark.FieldName = null;
		this.txtRemark.IsSearchControl = false;
		this.txtRemark.Location = new System.Drawing.Point(99, 35);
		this.txtRemark.Name = "txtRemark";
		this.txtRemark.SettingInVisible = false;
		this.txtRemark.SettingReadonly = false;
		this.txtRemark.Size = new System.Drawing.Size(190, 20);
		this.txtRemark.TabIndex = 13;
		this.btnSearch.EditControl = null;
		this.btnSearch.EditValue = "Search";
		this.btnSearch.FieldName = null;
		this.btnSearch.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnSearch.ImageOptions.Image");
		this.btnSearch.IsSearchControl = false;
		this.btnSearch.Location = new System.Drawing.Point(498, 32);
		this.btnSearch.Name = "btnSearch";
		this.btnSearch.ReadOnly = false;
		this.btnSearch.SettingInVisible = false;
		this.btnSearch.SettingReadonly = false;
		this.btnSearch.Size = new System.Drawing.Size(104, 23);
		this.btnSearch.TabIndex = 14;
		this.btnSearch.Text = "Search";
		this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
		this.grdDoc.AllowPasteExcel = true;
		this.grdDoc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grdDoc.ClipboardData = null;
		this.grdDoc.ContextMenuStrip = this.contextMenuStrip1;
		this.grdDoc.Location = new System.Drawing.Point(12, 64);
		this.grdDoc.MainView = this.grdTpt;
		this.grdDoc.Name = "grdDoc";
		this.grdDoc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[5] { this.rpidateExpiredDate, this.rpiLkuStatus, this.rpitPassword, this.rpiTUsername, this.rpibtnGenerateToken });
		this.grdDoc.Size = new System.Drawing.Size(786, 243);
		this.grdDoc.TabIndex = 0;
		this.grdDoc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.grdTpt });
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addNewToolStripMenuItem, this.deleteRowToolStripMenuItem });
		this.contextMenuStrip1.Name = "context_GridMenu";
		this.contextMenuStrip1.Size = new System.Drawing.Size(131, 48);
		this.addNewToolStripMenuItem.Name = "addNewToolStripMenuItem";
		this.addNewToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
		this.addNewToolStripMenuItem.Text = "Add new";
		this.addNewToolStripMenuItem.Click += new System.EventHandler(this.AddNewToolStripMenuItem_Click);
		this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
		this.deleteRowToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
		this.deleteRowToolStripMenuItem.Text = "Delete row";
		this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.DeleteRowToolStripMenuItem_Click);
		this.grdTpt.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.grdTpt.Appearance.FocusedRow.Options.UseBackColor = true;
		this.grdTpt.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.grdTpt.ColumnKey = null;
		this.grdTpt.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[10] { this.gcAbsEntry, this.gcTokenName, this.gcRemarks, this.gcStatus, this.gcToken, this.gcUserName, this.gcPassword, this.gcDatabaseName, this.gcDomainName, this.gcExpiredDate });
		this.grdTpt.DisableFormSetting = false;
		this.grdTpt.DisplayException = false;
		this.grdTpt.FieldCurrency = null;
		this.grdTpt.FieldRate = null;
		this.grdTpt.FormId = "oThirdPartyToken";
		this.grdTpt.GridControl = this.grdDoc;
		this.grdTpt.IndicatorWidth = 35;
		this.grdTpt.IsApzTable = false;
		this.grdTpt.IsInsertNewRow = false;
		this.grdTpt.IsLoadingData = false;
		this.grdTpt.ItemId = "Token";
		this.grdTpt.LstColumnAllowBatch = null;
		this.grdTpt.LstColumnNotAllowBatch = null;
		this.grdTpt.LstColumnNotEditable = null;
		this.grdTpt.LstColumNotSetting = null;
		this.grdTpt.Name = "grdTpt";
		this.grdTpt.NewItemRowText = "New Token";
		this.grdTpt.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.grdTpt.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.grdTpt.OptionsPrint.AutoWidth = false;
		this.grdTpt.OptionsPrint.UsePrintStyles = false;
		this.grdTpt.OptionsView.BestFitMaxRowCount = 100;
		this.grdTpt.OptionsView.ColumnAutoWidth = false;
		this.grdTpt.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.grdTpt.OptionsView.ShowGroupedColumns = true;
		this.grdTpt.OptionsView.ShowGroupPanel = false;
		this.grdTpt.UseBatchMod = false;
		this.grdTpt.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.grdTpt_ShowingEditor);
		this.grdTpt.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.grdTpt_CellValueChanged);
		this.grdTpt.BeforeLeaveRow += new DevExpress.XtraGrid.Views.Base.RowAllowEventHandler(this.grdTpt_BeforeLeaveRow);
		this.gcAbsEntry.Caption = "Entry";
		this.gcAbsEntry.CurrencyDateField = null;
		this.gcAbsEntry.CurrencyField = null;
		this.gcAbsEntry.CurrencyFixed = null;
		this.gcAbsEntry.CurrencyFixedRate = 0.0;
		this.gcAbsEntry.CurrencyRateField = null;
		this.gcAbsEntry.FieldName = "AbsEntry";
		this.gcAbsEntry.Name = "gcAbsEntry";
		this.gcAbsEntry.OptionsColumn.AllowEdit = false;
		this.gcAbsEntry.PrimaryKey = true;
		this.gcAbsEntry.UnitField = null;
		this.gcAbsEntry.Visible = true;
		this.gcAbsEntry.VisibleIndex = 0;
		this.gcTokenName.Caption = "Token Name";
		this.gcTokenName.CurrencyDateField = null;
		this.gcTokenName.CurrencyField = null;
		this.gcTokenName.CurrencyFixed = null;
		this.gcTokenName.CurrencyFixedRate = 0.0;
		this.gcTokenName.CurrencyRateField = null;
		this.gcTokenName.FieldName = "TokenName";
		this.gcTokenName.Name = "gcTokenName";
		this.gcTokenName.PrimaryKey = false;
		this.gcTokenName.UnitField = null;
		this.gcTokenName.Visible = true;
		this.gcTokenName.VisibleIndex = 1;
		this.gcRemarks.Caption = "Remarks";
		this.gcRemarks.CurrencyDateField = null;
		this.gcRemarks.CurrencyField = null;
		this.gcRemarks.CurrencyFixed = null;
		this.gcRemarks.CurrencyFixedRate = 0.0;
		this.gcRemarks.CurrencyRateField = null;
		this.gcRemarks.FieldName = "Remarks";
		this.gcRemarks.Name = "gcRemarks";
		this.gcRemarks.PrimaryKey = false;
		this.gcRemarks.UnitField = null;
		this.gcRemarks.Visible = true;
		this.gcRemarks.VisibleIndex = 2;
		this.gcStatus.Caption = "Status";
		this.gcStatus.ColumnEdit = this.rpiLkuStatus;
		this.gcStatus.CurrencyDateField = null;
		this.gcStatus.CurrencyField = null;
		this.gcStatus.CurrencyFixed = null;
		this.gcStatus.CurrencyFixedRate = 0.0;
		this.gcStatus.CurrencyRateField = null;
		this.gcStatus.FieldName = "Status";
		this.gcStatus.Name = "gcStatus";
		this.gcStatus.PrimaryKey = false;
		this.gcStatus.UnitField = null;
		this.gcStatus.Visible = true;
		this.gcStatus.VisibleIndex = 3;
		this.rpiLkuStatus.AutoHeight = false;
		this.rpiLkuStatus.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.rpiLkuStatus.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rpiLkuStatus.IsSearchControl = false;
		this.rpiLkuStatus.Name = "rpiLkuStatus";
		this.rpiLkuStatus.NullText = "";
		this.rpiLkuStatus.ShowFooter = false;
		this.rpiLkuStatus.ShowHeader = false;
		this.gcToken.Caption = "Token";
		this.gcToken.ColumnEdit = this.rpibtnGenerateToken;
		this.gcToken.CurrencyDateField = null;
		this.gcToken.CurrencyField = null;
		this.gcToken.CurrencyFixed = null;
		this.gcToken.CurrencyFixedRate = 0.0;
		this.gcToken.CurrencyRateField = null;
		this.gcToken.FieldName = "Token";
		this.gcToken.Name = "gcToken";
		this.gcToken.OptionsColumn.AllowEdit = false;
		this.gcToken.PrimaryKey = false;
		this.gcToken.UnitField = null;
		this.gcToken.Visible = true;
		this.gcToken.VisibleIndex = 4;
		this.rpibtnGenerateToken.AutoHeight = false;
		this.rpibtnGenerateToken.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.rpibtnGenerateToken.Name = "rpibtnGenerateToken";
		this.gcUserName.Caption = "Username";
		this.gcUserName.ColumnEdit = this.rpiTUsername;
		this.gcUserName.CurrencyDateField = null;
		this.gcUserName.CurrencyField = null;
		this.gcUserName.CurrencyFixed = null;
		this.gcUserName.CurrencyFixedRate = 0.0;
		this.gcUserName.CurrencyRateField = null;
		this.gcUserName.FieldName = "UserName";
		this.gcUserName.Name = "gcUserName";
		this.gcUserName.PrimaryKey = false;
		this.gcUserName.UnitField = null;
		this.gcUserName.Visible = true;
		this.gcUserName.VisibleIndex = 5;
		this.rpiTUsername.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
		this.rpiTUsername.AutoHeight = false;
		this.rpiTUsername.Name = "rpiTUsername";
		this.gcPassword.Caption = "Password";
		this.gcPassword.ColumnEdit = this.rpitPassword;
		this.gcPassword.CurrencyDateField = null;
		this.gcPassword.CurrencyField = null;
		this.gcPassword.CurrencyFixed = null;
		this.gcPassword.CurrencyFixedRate = 0.0;
		this.gcPassword.CurrencyRateField = null;
		this.gcPassword.FieldName = "Password";
		this.gcPassword.Name = "gcPassword";
		this.gcPassword.PrimaryKey = false;
		this.gcPassword.UnitField = null;
		this.gcPassword.Visible = true;
		this.gcPassword.VisibleIndex = 6;
		this.rpitPassword.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
		this.rpitPassword.AutoHeight = false;
		this.rpitPassword.Name = "rpitPassword";
		this.rpitPassword.PasswordChar = '*';
		this.gcDatabaseName.Caption = "Database Name";
		this.gcDatabaseName.CurrencyDateField = null;
		this.gcDatabaseName.CurrencyField = null;
		this.gcDatabaseName.CurrencyFixed = null;
		this.gcDatabaseName.CurrencyFixedRate = 0.0;
		this.gcDatabaseName.CurrencyRateField = null;
		this.gcDatabaseName.FieldName = "DatabaseName";
		this.gcDatabaseName.Name = "gcDatabaseName";
		this.gcDatabaseName.OptionsColumn.AllowEdit = false;
		this.gcDatabaseName.PrimaryKey = false;
		this.gcDatabaseName.UnitField = null;
		this.gcDatabaseName.Visible = true;
		this.gcDatabaseName.VisibleIndex = 7;
		this.gcDomainName.Caption = "Domain Name";
		this.gcDomainName.CurrencyDateField = null;
		this.gcDomainName.CurrencyField = null;
		this.gcDomainName.CurrencyFixed = null;
		this.gcDomainName.CurrencyFixedRate = 0.0;
		this.gcDomainName.CurrencyRateField = null;
		this.gcDomainName.FieldName = "DomainName";
		this.gcDomainName.Name = "gcDomainName";
		this.gcDomainName.OptionsColumn.AllowEdit = false;
		this.gcDomainName.PrimaryKey = false;
		this.gcDomainName.UnitField = null;
		this.gcDomainName.Visible = true;
		this.gcDomainName.VisibleIndex = 8;
		this.gcExpiredDate.Caption = "Expired Date";
		this.gcExpiredDate.ColumnEdit = this.rpidateExpiredDate;
		this.gcExpiredDate.CurrencyDateField = null;
		this.gcExpiredDate.CurrencyField = null;
		this.gcExpiredDate.CurrencyFixed = null;
		this.gcExpiredDate.CurrencyFixedRate = 0.0;
		this.gcExpiredDate.CurrencyRateField = null;
		this.gcExpiredDate.DisplayFormat.FormatString = "d";
		this.gcExpiredDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.gcExpiredDate.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gcExpiredDate.FieldName = "ExpiredDate";
		this.gcExpiredDate.Name = "gcExpiredDate";
		this.gcExpiredDate.PrimaryKey = false;
		this.gcExpiredDate.UnitField = null;
		this.gcExpiredDate.Visible = true;
		this.gcExpiredDate.VisibleIndex = 9;
		this.rpidateExpiredDate.AutoHeight = false;
		this.rpidateExpiredDate.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rpidateExpiredDate.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rpidateExpiredDate.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.rpidateExpiredDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.rpidateExpiredDate.EditFormat.FormatString = "dd/MM/yyyy";
		this.rpidateExpiredDate.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.rpidateExpiredDate.IsSearchControl = false;
		this.rpidateExpiredDate.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.rpidateExpiredDate.Name = "rpidateExpiredDate";
		this.btnSave.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnSave.ButtonKind = Apzon.Commons.Enums.Enums.SimpleButtonType.Save;
		this.btnSave.EditControl = null;
		this.btnSave.EditValue = "Save";
		this.btnSave.FieldName = null;
		this.btnSave.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnSave.ImageOptions.Image");
		this.btnSave.IsSearchControl = false;
		this.btnSave.Location = new System.Drawing.Point(12, 313);
		this.btnSave.Name = "btnSave";
		this.btnSave.ReadOnly = false;
		this.btnSave.SettingInVisible = false;
		this.btnSave.SettingReadonly = false;
		this.btnSave.Size = new System.Drawing.Size(75, 23);
		this.btnSave.TabIndex = 15;
		this.btnSave.Text = "Save";
		this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnCancel.ButtonKind = Apzon.Commons.Enums.Enums.SimpleButtonType.Save;
		this.btnCancel.EditControl = null;
		this.btnCancel.EditValue = "Cancel";
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(93, 313);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(75, 23);
		this.btnCancel.TabIndex = 16;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.lkUp_Status.EditControl = null;
		this.lkUp_Status.FieldName = null;
		this.lkUp_Status.IsSearchControl = false;
		this.lkUp_Status.Location = new System.Drawing.Point(345, 35);
		this.lkUp_Status.Name = "lkUp_Status";
		this.lkUp_Status.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.lkUp_Status.Properties.DropDownRows = 3;
		this.lkUp_Status.Properties.NullText = "";
		this.lkUp_Status.SettingInVisible = false;
		this.lkUp_Status.SettingReadonly = false;
		this.lkUp_Status.Size = new System.Drawing.Size(100, 20);
		this.lkUp_Status.TabIndex = 17;
		this.lb_TokenName.EditControl = null;
		this.lb_TokenName.EditValue = "Token Name";
		this.lb_TokenName.FieldName = null;
		this.lb_TokenName.HideWhenEmpty = false;
		this.lb_TokenName.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_TokenName.Location = new System.Drawing.Point(13, 8);
		this.lb_TokenName.Name = "lb_TokenName";
		this.lb_TokenName.ReadOnly = false;
		this.lb_TokenName.SettingInVisible = false;
		this.lb_TokenName.SettingReadonly = false;
		this.lb_TokenName.ShowUnderLine = true;
		this.lb_TokenName.Size = new System.Drawing.Size(80, 20);
		this.lb_TokenName.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_TokenName.TabIndex = 18;
		this.lb_TokenName.Utext = "Token Name";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(812, 341);
		base.Controls.Add(this.lb_TokenName);
		base.Controls.Add(this.lkUp_Status);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.btnSave);
		base.Controls.Add(this.grdDoc);
		base.Controls.Add(this.btnSearch);
		base.Controls.Add(this.txtRemark);
		base.Controls.Add(this.lb_Remarks);
		base.Controls.Add(this.txtTokenName);
		base.Controls.Add(this.lb_Status);
		base.DisableContextAction = true;
		base.DisableLoadScript = true;
		base.FormId = "oThirdPartyToken";
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_ThirdPartyToken.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_ThirdPartyToken";
		this.Text = "Third party token management";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_ThirdPartyToken_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.txtTokenName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtRemark.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdTpt).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rpiLkuStatus).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rpibtnGenerateToken).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rpiTUsername).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rpitPassword).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rpidateExpiredDate.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rpidateExpiredDate).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lkUp_Status.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
