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
using Apzon.Forms.Commons;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.HumanResources.TimeSheeting;

public class frm_WorkCode : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	protected Enums.FormAuthorities Authoritues = Enums.FormAuthorities.Read;

	private bool _forceClose;

	private bool _isCancel;

	private IContainer components;

	private GridControlEx grd_WorkCode;

	private GridViewEx gv_WorkCode;

	private GridColumnEx cl_Code;

	private GridColumnEx cl_Name;

	private RepositoryItemLookUpEdit LkUp_Type;

	private RepositoryItemLookUpEdit LkUpSalary;

	public SimpleButtonEx btn_Save;

	public SimpleButtonEx btnCancel;

	private GridColumnEx cl_Rate;

	private GridColumnEx cl_WorkType;

	private GridColumnEx cl_Remarks;

	private GridColumnEx cl_Color;

	private GridColumnEx cl_Branch;

	private ContextMenuStrip contextMenuStrip_WorkCode;

	private ToolStripMenuItem toolStripMenuItem_Remove;

	private RepositoryItemCalcEdit CalcRate;

	private RepositoryItemLookUpEdit lukWorkType;

	private RepositoryItemLookUpEdit lukBranch;

	private RepositoryItemTextEdit resItemCode;

	private RepositoryItemColorPickEdit colorPick_Color;

	private RepositoryItemColorEdit repositoryItemColorEdit1;

	private RepositoryItemColorPickEdit repositoryItemColorPickEdit1;

	private GridColumnEx cl_Status;

	private RepositoryItemLookUpEdit lukStatus;

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
			case Enums.FormViewModeType.Update:
				btn_Save.Text = Resources.ButtonUpdate;
				break;
			case Enums.FormViewModeType.View:
				btn_Save.Text = Resources.ButtonOK;
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public frm_WorkCode()
	{
		InitializeComponent();
		FormId = "oWorkCode";
		gv_WorkCode.ItemId = "APZ_OHWC";
		gv_WorkCode.FormId = FormId;
		OnLoadAsync += frm_WorkCode_Load;
	}

	private async Task frm_WorkCode_Load(object sender, EventArgs e)
	{
		_ = 2;
		try
		{
			await LoadLookupWorkType();
			await LoadLookupBranches();
			LoadLookupStatus();
			Dictionary<string, GridViewEx> tblFormular = new Dictionary<string, GridViewEx> { ["APZ_OHWC"] = gv_WorkCode };
			WinFuntions.LoadUdfColumns(gv_WorkCode, "APZ_OHWC", tblFormular);
			await RefreshWorkCode();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public void AddNewRow()
	{
		try
		{
			gv_WorkCode.BeginInsertRow();
			DataTable dataSourceTable = grd_WorkCode.DataSourceTable;
			if (dataSourceTable.IsNotNull())
			{
				if (dataSourceTable.AsEnumerable().Any((DataRow s) => Function.ToString(s["Code"]) == ""))
				{
					return;
				}
				DataRow dataRow = dataSourceTable.NewRow();
				dataRow["Code"] = "";
				dataRow["Name"] = "";
				dataRow["Rate"] = 0.0;
				dataRow["WorkType"] = 0;
				dataRow["Remarks"] = "";
				dataRow["Color"] = 0;
				dataRow["Branch"] = 0;
				dataRow["IsNewRow"] = "Y";
				dataSourceTable.Rows.Add(dataRow);
				dataSourceTable.AcceptChanges();
				dataSourceTable.SetDefaultAndMaxLen(gv_WorkCode.ItemId);
				grd_WorkCode.DataSource = dataSourceTable;
				grd_WorkCode.RefreshDataSource();
			}
			else
			{
				DataTable tableStructure = WinFuntions.GetTableStructure("APZ_OHWC");
				if (!tableStructure.ExistsColumn("IsNewRow"))
				{
					tableStructure.Columns.Add("IsNewRow", typeof(string));
				}
				DataRow dataRow2 = tableStructure.NewRow();
				dataRow2["Code"] = "";
				dataRow2["Name"] = "";
				dataRow2["Rate"] = 0.0;
				dataRow2["WorkType"] = 0;
				dataRow2["Remarks"] = "";
				dataRow2["Color"] = 0;
				dataRow2["Branch"] = 0;
				dataRow2["IsNewRow"] = "Y";
				tableStructure.Rows.Add(dataRow2);
				tableStructure.AcceptChanges();
				tableStructure.SetDefaultAndMaxLen(gv_WorkCode.ItemId);
				grd_WorkCode.DataSource = tableStructure;
				grd_WorkCode.RefreshDataSource();
			}
			gv_WorkCode.EndInsertRow();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			gv_WorkCode.EndInsertRow();
		}
	}

	public async Task RefreshWorkCode()
	{
		try
		{
			DataTable dataTable = await GetWorkCode();
			dataTable.SetDefaultAndMaxLen(gv_WorkCode.ItemId);
			grd_WorkCode.DataSource = dataTable;
			grd_WorkCode.DataSourceTable.AcceptChanges();
			grd_WorkCode.RefreshDataSource();
			AddNewRow();
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task<DataTable> GetWorkCode()
	{
		try
		{
			DataTable dataTable = await _httpClient.TimeSheetingClient.LoadWorkCode();
			if (dataTable.IsNotNull() && !dataTable.ExistsColumn("IsNewRow"))
			{
				dataTable.Columns.Add("IsNewRow", typeof(string));
			}
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new DataTable();
	}

	private async Task LoadLookupBranches()
	{
		try
		{
			DataTable dataSource = await _httpClient.EmployeeMasterClient.LoadComboBranch();
			lukBranch.DataSource = dataSource;
			lukBranch.DisplayMember = "Name";
			lukBranch.ValueMember = "Code";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadLookupWorkType()
	{
		try
		{
			DataTable dataTable = await _httpClient.TimeSheetingClient.LoadWorkType();
			DataRow dataRow = dataTable.NewRow();
			dataRow["Id"] = -10000;
			dataRow["Name"] = "-Define New-";
			dataTable.Rows.Add(dataRow);
			lukWorkType.DataSource = dataTable;
			lukWorkType.DisplayMember = "Name";
			lukWorkType.ValueMember = "Id";
			lukWorkType.BestFitMode = BestFitMode.BestFitResizePopup;
			lukWorkType.ForceInitialize();
			lukWorkType.PopulateColumns();
			lukWorkType.Columns["Id"].Visible = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void LoadLookupStatus()
	{
		try
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("A", "Active");
			dictionary.Add("I", "Inactive");
			lukStatus.SetDataSource(dictionary);
			lukStatus.DisplayMember = "Name";
			lukStatus.ValueMember = "Code";
			lukStatus.BestFitMode = BestFitMode.BestFitResizePopup;
			lukStatus.ForceInitialize();
			lukStatus.PopulateColumns();
			lukStatus.Columns["Code"].Visible = false;
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
			if (FormMode != Enums.FormViewModeType.Update || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				await RefreshWorkCode();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_WorkCode_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
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

	private void lukWorkType_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (Function.ParseInt(e.NewValue) == -10000)
			{
				frm_WorkType frm_WorkType2 = new frm_WorkType
				{
					MdiParent = MdiParent
				};
				frm_WorkType2.refreshType = (Func<Task>)Delegate.Combine(frm_WorkType2.refreshType, new Func<Task>(LoadLookupWorkType));
				frm_WorkType2.Show();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_WorkCode_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			ColumnView columnView = (ColumnView)sender;
			if (Function.ToString(gv_WorkCode.GetFocusedRowCellValue("IsNewRow")) != "Y" && (columnView.FocusedColumn.FieldName == "Code" || columnView.FocusedColumn.FieldName == "Branch"))
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

	private void gv_WorkCode_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
	{
		try
		{
			if (sender is ColumnView columnView)
			{
				_ = (GridColumnEx)columnView.FocusedColumn;
				_ = Function.ToString(gv_WorkCode.GetFocusedRowCellValue("IsNewRow")) == "Y";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_WorkCode_InvalidValueException(object sender, InvalidValueExceptionEventArgs e)
	{
		try
		{
			if (sender is ColumnView columnView)
			{
				e.ExceptionMode = ExceptionMode.DisplayError;
				e.WindowCaption = "Input Error";
				columnView.HideEditor();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Save_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				_isCancel = true;
				Close();
				_isCancel = false;
			}
			else if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				btn_Save.Enabled = false;
				btnCancel.Enabled = false;
				await SaveWorkCode();
				btn_Save.Enabled = true;
				btnCancel.Enabled = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task<string> SaveWorkCode()
	{
		_ = 1;
		try
		{
			if (grd_WorkCode.DataSource is DataTable dataTable)
			{
				DataTable dataTable2 = dataTable.Copy();
				if (dataTable2.IsNotNull())
				{
					DataRow dataRow = dataTable2.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["Code"]) == "");
					if (dataRow != null && Function.ParseInt(dataRow["WorkType"]) == 0 && string.IsNullOrEmpty(Function.ToString(dataRow["Name"])))
					{
						dataTable2.Rows.Remove(dataRow);
						dataTable2.AcceptChanges();
					}
					if (dataTable2.IsNotNull())
					{
						dataTable2 = dataTable2.Copy();
						if (dataTable2.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["code"]).Length > 10) != null)
						{
							WinCommons.Warning(Enums.MessageCode.TheLengthOfStringEnteredIsLargerThanSpecified, "");
							return "Error!";
						}
						if (dataTable2.AsEnumerable().FirstOrDefault((DataRow d) => string.IsNullOrEmpty(Function.ToString(d["Code"]))) != null)
						{
							WinCommons.Error(Enums.MessageCode.CodeCannotBeEmpty, "");
							return "Error!";
						}
						if (dataTable2.AsEnumerable().FirstOrDefault((DataRow d) => string.IsNullOrEmpty(Function.ToString(d["Name"]))) != null)
						{
							WinCommons.Error(Enums.MessageCode.NameCannotBeEmpty, "");
							return "Error!";
						}
						if (dataTable2.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["WorkType"]) == 0) != null)
						{
							WinCommons.Error(Enums.MessageCode.WOrkTypeCannotBeEmpty, "");
							return "Error!";
						}
						if (dataTable2.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["Branch"]) == 0) != null)
						{
							WinCommons.Error(Enums.MessageCode.BranchCannotBeEmpty, "");
							return "Error!";
						}
						if ((from c in dataTable.AsEnumerable()
							group c by new
							{
								Code = c.Field<string>("Code"),
								Branch = c.Field<int>("Branch")
							} into g
							where g.Count() > 1
							select new { g.Key.Code }).ToList().Count > 0)
						{
							WinCommons.Error(Enums.MessageCode.ThisEntryAlreadyExistsInTable, "");
							return "Error!";
						}
						if (dataTable2.ExistsColumn("IsNewRow"))
						{
							dataTable2.Columns.Remove("IsNewRow");
						}
						for (int num = 0; num < dataTable2.Rows.Count; num++)
						{
							dataTable2.Rows[num]["UserSign"] = GlobalData.UserID;
							dataTable2.Rows[num]["UpdateDate"] = DateTime.Today;
						}
						dataTable2.AcceptChanges();
						WinCommons._OpenProcessing();
						string text = await _httpClient.TimeSheetingClient.SaveWorkCode(dataTable2);
						WinCommons._CloseProcessing();
						if (string.IsNullOrEmpty(text))
						{
							await RefreshWorkCode();
							WinCommons.Success(Enums.MessageCode.Success, "");
							FormMode = Enums.FormViewModeType.View;
						}
						else
						{
							WinCommons.Error(Enums.MessageCode.None, text);
						}
					}
				}
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return string.Empty;
	}

	private void toolStripMenuItem_Remove_Click(object sender, EventArgs e)
	{
		try
		{
			string text = Function.ToString(gv_WorkCode.GetFocusedRowCellValue("Code"));
			int branch = Function.ParseInt(gv_WorkCode.GetFocusedRowCellValue("Branch"));
			if (!string.IsNullOrEmpty(text) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmRemoveItem, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				RemoveWorkCode(text, branch);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void RemoveWorkCode(string Id, int branch)
	{
		_ = 1;
		try
		{
			string text = await _httpClient.TimeSheetingClient.RemoveWorkCode(Id, branch);
			if (string.IsNullOrWhiteSpace(text))
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
				gv_WorkCode.BeginUpdate();
				gv_WorkCode.DeleteSelectedRows();
				gv_WorkCode.EndUpdate();
				await RefreshWorkCode();
				AddNewRow();
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

	private void btnCancel_Click(object sender, EventArgs e)
	{
		try
		{
			_isCancel = true;
			Close();
			_isCancel = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void frm_WorkCode_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if (_isCancel)
			{
				return;
			}
			if (!_forceClose)
			{
				e.Cancel = true;
			}
			if (_forceClose)
			{
				return;
			}
			if (FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update)
			{
				switch (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSaveBeforeClose, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
				{
				case DialogResult.No:
					_forceClose = true;
					Close();
					break;
				case DialogResult.Yes:
					if (string.IsNullOrEmpty(await SaveWorkCode()))
					{
						_forceClose = true;
						Close();
					}
					break;
				}
			}
			else
			{
				_forceClose = true;
				Close();
			}
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.HumanResources.TimeSheeting.frm_WorkCode));
		this.LkUp_Type = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.LkUpSalary = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.grd_WorkCode = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip_WorkCode = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.toolStripMenuItem_Remove = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_WorkCode = new Apzon.Controls.Grid.GridViewEx();
		this.cl_Code = new Apzon.Controls.Grid.GridColumnEx();
		this.resItemCode = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.cl_Name = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_Rate = new Apzon.Controls.Grid.GridColumnEx();
		this.CalcRate = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.cl_WorkType = new Apzon.Controls.Grid.GridColumnEx();
		this.lukWorkType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.cl_Remarks = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_Color = new Apzon.Controls.Grid.GridColumnEx();
		this.colorPick_Color = new DevExpress.XtraEditors.Repository.RepositoryItemColorPickEdit();
		this.cl_Branch = new Apzon.Controls.Grid.GridColumnEx();
		this.lukBranch = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.cl_Status = new Apzon.Controls.Grid.GridColumnEx();
		this.lukStatus = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.repositoryItemColorEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemColorEdit();
		this.repositoryItemColorPickEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemColorPickEdit();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.LkUp_Type).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.LkUpSalary).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_WorkCode).BeginInit();
		this.contextMenuStrip_WorkCode.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_WorkCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.resItemCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.CalcRate).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lukWorkType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.colorPick_Color).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lukBranch).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lukStatus).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemColorEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemColorPickEdit1).BeginInit();
		base.SuspendLayout();
		this.LkUp_Type.AutoHeight = false;
		this.LkUp_Type.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.LkUp_Type.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.LkUp_Type.DisplayMember = "Name";
		this.LkUp_Type.Name = "LkUp_Type";
		this.LkUp_Type.ValueMember = "Code";
		this.LkUpSalary.AutoHeight = false;
		this.LkUpSalary.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.LkUpSalary.DisplayMember = "Name";
		this.LkUpSalary.Name = "LkUpSalary";
		this.LkUpSalary.ValueMember = "Code";
		this.grd_WorkCode.AllowPasteExcel = true;
		this.grd_WorkCode.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_WorkCode.ClipboardData = null;
		this.grd_WorkCode.ContextMenuStrip = this.contextMenuStrip_WorkCode;
		this.grd_WorkCode.Location = new System.Drawing.Point(3, 3);
		this.grd_WorkCode.MainView = this.gv_WorkCode;
		this.grd_WorkCode.Name = "grd_WorkCode";
		this.grd_WorkCode.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[8] { this.CalcRate, this.lukWorkType, this.lukBranch, this.resItemCode, this.colorPick_Color, this.repositoryItemColorEdit1, this.repositoryItemColorPickEdit1, this.lukStatus });
		this.grd_WorkCode.Size = new System.Drawing.Size(756, 391);
		this.grd_WorkCode.TabIndex = 77;
		this.grd_WorkCode.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_WorkCode });
		this.contextMenuStrip_WorkCode.ImageScalingSize = new System.Drawing.Size(20, 20);
		this.contextMenuStrip_WorkCode.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.toolStripMenuItem_Remove });
		this.contextMenuStrip_WorkCode.Name = "contextMenuStrip_WorkCode";
		this.contextMenuStrip_WorkCode.Size = new System.Drawing.Size(118, 26);
		this.toolStripMenuItem_Remove.Name = "toolStripMenuItem_Remove";
		this.toolStripMenuItem_Remove.Size = new System.Drawing.Size(117, 22);
		this.toolStripMenuItem_Remove.Text = "Remove";
		this.toolStripMenuItem_Remove.Click += new System.EventHandler(this.toolStripMenuItem_Remove_Click);
		this.gv_WorkCode.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clWorkCode;
		this.gv_WorkCode.ColumnKey = null;
		this.gv_WorkCode.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[8] { this.cl_Code, this.cl_Name, this.cl_Rate, this.cl_WorkType, this.cl_Remarks, this.cl_Color, this.cl_Branch, this.cl_Status });
		this.gv_WorkCode.DisableFormSetting = false;
		this.gv_WorkCode.DisplayException = false;
		this.gv_WorkCode.EditColumn = null;
		this.gv_WorkCode.FieldCurrency = null;
		this.gv_WorkCode.FieldRate = null;
		this.gv_WorkCode.FormId = "oWorkCode";
		this.gv_WorkCode.GridControl = this.grd_WorkCode;
		this.gv_WorkCode.IgnoreFilter = false;
		this.gv_WorkCode.IndexEditRow = -1;
		this.gv_WorkCode.IndicatorWidth = 30;
		this.gv_WorkCode.IsApzTable = true;
		this.gv_WorkCode.IsInsertNewRow = false;
		this.gv_WorkCode.IsLoadingData = false;
		this.gv_WorkCode.IsMappingContextAction = false;
		this.gv_WorkCode.ItemId = "APZ_OHWC";
		this.gv_WorkCode.LstColumnAllowBatch = null;
		this.gv_WorkCode.LstColumnNotAllowBatch = null;
		this.gv_WorkCode.LstColumnNotEditable = null;
		this.gv_WorkCode.LstColumNotSetting = null;
		this.gv_WorkCode.Name = "gv_WorkCode";
		this.gv_WorkCode.OptionsView.RowAutoHeight = true;
		this.gv_WorkCode.OptionsView.ShowGroupPanel = false;
		this.gv_WorkCode.UseBatchMod = false;
		this.gv_WorkCode.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_WorkCode_ShowingEditor);
		this.gv_WorkCode.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_WorkCode_CellValueChanged);
		this.gv_WorkCode.ValidatingEditor += new DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventHandler(this.gv_WorkCode_ValidatingEditor);
		this.gv_WorkCode.InvalidValueException += new DevExpress.XtraEditors.Controls.InvalidValueExceptionEventHandler(this.gv_WorkCode_InvalidValueException);
		this.cl_Code.Caption = "Code";
		this.cl_Code.ColumnEdit = this.resItemCode;
		this.cl_Code.CurrencyDateField = null;
		this.cl_Code.CurrencyField = null;
		this.cl_Code.CurrencyFixed = null;
		this.cl_Code.CurrencyFixedRate = 0.0;
		this.cl_Code.CurrencyRateField = null;
		this.cl_Code.FieldName = "Code";
		this.cl_Code.Name = "cl_Code";
		this.cl_Code.PrimaryKey = true;
		this.cl_Code.UnitField = null;
		this.cl_Code.Visible = true;
		this.cl_Code.VisibleIndex = 0;
		this.cl_Code.Width = 44;
		this.resItemCode.AutoHeight = false;
		this.resItemCode.MaxLength = 10;
		this.resItemCode.Name = "resItemCode";
		this.cl_Name.Caption = "Name";
		this.cl_Name.CurrencyDateField = null;
		this.cl_Name.CurrencyField = null;
		this.cl_Name.CurrencyFixed = null;
		this.cl_Name.CurrencyFixedRate = 0.0;
		this.cl_Name.CurrencyRateField = null;
		this.cl_Name.FieldName = "Name";
		this.cl_Name.Name = "cl_Name";
		this.cl_Name.PrimaryKey = false;
		this.cl_Name.UnitField = null;
		this.cl_Name.Visible = true;
		this.cl_Name.VisibleIndex = 1;
		this.cl_Name.Width = 112;
		this.cl_Rate.Caption = "Rate";
		this.cl_Rate.ColumnEdit = this.CalcRate;
		this.cl_Rate.CurrencyDateField = null;
		this.cl_Rate.CurrencyField = null;
		this.cl_Rate.CurrencyFixed = null;
		this.cl_Rate.CurrencyFixedRate = 0.0;
		this.cl_Rate.CurrencyRateField = null;
		this.cl_Rate.FieldName = "Rate";
		this.cl_Rate.Name = "cl_Rate";
		this.cl_Rate.PrimaryKey = false;
		this.cl_Rate.UnitField = null;
		this.cl_Rate.Visible = true;
		this.cl_Rate.VisibleIndex = 2;
		this.CalcRate.AutoHeight = false;
		this.CalcRate.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.CalcRate.Name = "CalcRate";
		this.cl_WorkType.Caption = "Work Type";
		this.cl_WorkType.ColumnEdit = this.lukWorkType;
		this.cl_WorkType.CurrencyDateField = null;
		this.cl_WorkType.CurrencyField = null;
		this.cl_WorkType.CurrencyFixed = null;
		this.cl_WorkType.CurrencyFixedRate = 0.0;
		this.cl_WorkType.CurrencyRateField = null;
		this.cl_WorkType.FieldName = "WorkType";
		this.cl_WorkType.Name = "cl_WorkType";
		this.cl_WorkType.PrimaryKey = false;
		this.cl_WorkType.UnitField = null;
		this.cl_WorkType.Visible = true;
		this.cl_WorkType.VisibleIndex = 3;
		this.lukWorkType.AutoHeight = false;
		this.lukWorkType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.lukWorkType.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Id", "Id", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.lukWorkType.DisplayMember = "Name";
		this.lukWorkType.Name = "lukWorkType";
		this.lukWorkType.NullText = "";
		this.lukWorkType.ValueMember = "Id";
		this.lukWorkType.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.lukWorkType_EditValueChanging);
		this.cl_Remarks.Caption = "Remarks";
		this.cl_Remarks.CurrencyDateField = null;
		this.cl_Remarks.CurrencyField = null;
		this.cl_Remarks.CurrencyFixed = null;
		this.cl_Remarks.CurrencyFixedRate = 0.0;
		this.cl_Remarks.CurrencyRateField = null;
		this.cl_Remarks.FieldName = "Remarks";
		this.cl_Remarks.Name = "cl_Remarks";
		this.cl_Remarks.PrimaryKey = false;
		this.cl_Remarks.UnitField = null;
		this.cl_Remarks.Visible = true;
		this.cl_Remarks.VisibleIndex = 4;
		this.cl_Color.Caption = "Color";
		this.cl_Color.ColumnEdit = this.colorPick_Color;
		this.cl_Color.CurrencyDateField = null;
		this.cl_Color.CurrencyField = null;
		this.cl_Color.CurrencyFixed = null;
		this.cl_Color.CurrencyFixedRate = 0.0;
		this.cl_Color.CurrencyRateField = null;
		this.cl_Color.FieldName = "Color";
		this.cl_Color.Name = "cl_Color";
		this.cl_Color.PrimaryKey = false;
		this.cl_Color.UnitField = null;
		this.cl_Color.Visible = true;
		this.cl_Color.VisibleIndex = 5;
		this.colorPick_Color.AutoHeight = false;
		this.colorPick_Color.AutomaticColor = System.Drawing.Color.Black;
		this.colorPick_Color.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.colorPick_Color.Name = "colorPick_Color";
		this.colorPick_Color.StoreColorAsInteger = true;
		this.cl_Branch.Caption = "Branch";
		this.cl_Branch.ColumnEdit = this.lukBranch;
		this.cl_Branch.CurrencyDateField = null;
		this.cl_Branch.CurrencyField = null;
		this.cl_Branch.CurrencyFixed = null;
		this.cl_Branch.CurrencyFixedRate = 0.0;
		this.cl_Branch.CurrencyRateField = null;
		this.cl_Branch.FieldName = "Branch";
		this.cl_Branch.Name = "cl_Branch";
		this.cl_Branch.PrimaryKey = false;
		this.cl_Branch.UnitField = null;
		this.cl_Branch.Visible = true;
		this.cl_Branch.VisibleIndex = 6;
		this.lukBranch.AutoHeight = false;
		this.lukBranch.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.lukBranch.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.lukBranch.DisplayMember = "Name";
		this.lukBranch.Name = "lukBranch";
		this.lukBranch.NullText = "";
		this.lukBranch.ValueMember = "Code";
		this.cl_Status.Caption = "Status";
		this.cl_Status.ColumnEdit = this.lukStatus;
		this.cl_Status.CurrencyDateField = null;
		this.cl_Status.CurrencyField = null;
		this.cl_Status.CurrencyFixed = null;
		this.cl_Status.CurrencyFixedRate = 0.0;
		this.cl_Status.CurrencyRateField = null;
		this.cl_Status.FieldName = "Status";
		this.cl_Status.Name = "cl_Status";
		this.cl_Status.PrimaryKey = false;
		this.cl_Status.UnitField = null;
		this.cl_Status.Visible = true;
		this.cl_Status.VisibleIndex = 7;
		this.lukStatus.AutoHeight = false;
		this.lukStatus.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.lukStatus.Name = "lukStatus";
		this.lukStatus.NullText = "";
		this.repositoryItemColorEdit1.AutoHeight = false;
		this.repositoryItemColorEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemColorEdit1.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.repositoryItemColorEdit1.Name = "repositoryItemColorEdit1";
		this.repositoryItemColorPickEdit1.AutoHeight = false;
		this.repositoryItemColorPickEdit1.AutomaticColor = System.Drawing.Color.Black;
		this.repositoryItemColorPickEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemColorPickEdit1.Name = "repositoryItemColorPickEdit1";
		this.repositoryItemColorPickEdit1.StoreColorAsInteger = true;
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Save.Appearance.Options.UseBackColor = true;
		this.btn_Save.EditControl = null;
		this.btn_Save.EditValue = "OK";
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(3, 400);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.ReadOnly = false;
		this.btn_Save.SettingInVisible = false;
		this.btn_Save.SettingReadonly = false;
		this.btn_Save.Size = new System.Drawing.Size(61, 20);
		this.btn_Save.TabIndex = 79;
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
		this.btnCancel.Location = new System.Drawing.Point(70, 400);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(93, 20);
		this.btnCancel.TabIndex = 78;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		base.AcceptButton = this.btn_Save;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btnCancel;
		base.ClientSize = new System.Drawing.Size(762, 422);
		base.Controls.Add(this.btn_Save);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.grd_WorkCode);
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_WorkCode";
		this.Text = "Work Code";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_WorkCode_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.LkUp_Type).EndInit();
		((System.ComponentModel.ISupportInitialize)this.LkUpSalary).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_WorkCode).EndInit();
		this.contextMenuStrip_WorkCode.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_WorkCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.resItemCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.CalcRate).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lukWorkType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.colorPick_Color).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lukBranch).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lukStatus).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemColorEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemColorPickEdit1).EndInit();
		base.ResumeLayout(false);
	}
}
