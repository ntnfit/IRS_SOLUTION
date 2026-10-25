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
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Forms.Commons;
using Apzon.Forms.Properties;
using Apzon.Forms.UserControls;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.HumanResources.TimeSheeting;

public class frm_WorkScheduleRegister : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private bool _isLoading;

	private bool _isChanging;

	private DataTable _dtSchedules;

	private IContainer components;

	private LabelLine lbl_Department;

	private LabelLine lbl_Branch;

	private LabelLine labelLine3;

	private TextEditEx txt_EmpNo;

	private TextEditEx txt_EmpName;

	private LabelLine lbl_EmpNo;

	private LabelLine lbl_FirstName;

	public SimpleButtonEx btnSearch;

	public SimpleButtonEx btn_EditMain;

	public SimpleButtonEx btn_Save;

	public SimpleButtonEx btn_Cancel;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem removeRowToolStripMenuItem;

	private CheckedComboBoxEditEx luk_Department;

	private CheckedComboBoxEditEx luk_Status;

	private LookUpEditCombo luk_Branch;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx3;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx4;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx1;

	private GridControlEx grd_Details;

	private GridViewEx gv_Details;

	private GridColumnEx gridColumnEx2;

	private GridColumnEx gridColumnEx3;

	private GridColumnEx gridColumnEx4;

	private GridControlEx grd_Doc;

	private GridViewEx gv_Doc;

	private GridColumnEx gridColumnEx7;

	private RepositoryItemLookUpEdit rep_Branch2;

	private GridColumnEx gridColumnEx6;

	private RepositoryItemLookUpEdit rep_Status2;

	private GridColumnEx gridColumn10;

	private RepositoryItemLookUpEdit rep_deptAdvanced;

	private GridColumnEx gc_empNo3;

	private RepositoryItemButtonEdit rep_CFL2;

	private GridColumnEx gridColumnEx10;

	private GridColumnEx gridColumn12;

	private GridColumnEx gridColumnEx1;

	private GridViewEx gridViewEx2;

	private SplitContainerControl splitContainerControl1;

	private RepositoryItemLookUpEdit rep_ScheduleLine;

	private RepositoryItemLookUpEdit rep_Schedule;

	private RepositoryItemDateEditEx rep_ValidFrom;

	private RepositoryItemDateEditEx rep_ValidTo;

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

	public frm_WorkScheduleRegister()
	{
		InitializeComponent();
		FormId = "oWorkScheduleRegister";
		gv_Doc.FormId = FormId;
		gv_Doc.ItemId = "OHEM";
		gv_Details.FormId = FormId;
		gv_Details.ItemId = "APZ_OWSR";
		OnLoadAsync += frm_WorkScheduleRegister_Load;
		gv_Doc.ShowingEditor += GvDocOnShowingEditor;
		gv_Details.CellValueChanged += GvDetailsOnCellValueChanged;
		gv_Doc.BeforeLeaveRow += GvDocOnBeforeLeaveRow;
		gv_Doc.FocusedRowChanged += GvDocOnFocusedRowChanged;
	}

	private void GvDocOnFocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
	{
		if (gv_Doc.IsGroupRow(e.PrevFocusedRowHandle) || gv_Doc.IsGroupRow(e.FocusedRowHandle) || !gv_Doc.IsValidRowHandle(e.PrevFocusedRowHandle) || !gv_Doc.IsValidRowHandle(e.FocusedRowHandle))
		{
			gv_Doc_FocusedRowObjectChanged(null, null);
		}
	}

	private void GvDocOnBeforeLeaveRow(object sender, RowAllowEventArgs e)
	{
		try
		{
			if (_isLoading || (FormMode != Enums.FormViewModeType.Add && FormMode != Enums.FormViewModeType.Update))
			{
				return;
			}
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				e.Allow = false;
				return;
			}
			if (grd_Doc.DataSource is DataTable dataTable)
			{
				dataTable.RejectChanges();
			}
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void GvDetailsOnCellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			DataTable dataSourceTable = grd_Details.DataSourceTable;
			if (!_isChanging)
			{
				FormMode = Enums.FormViewModeType.Update;
				if (dataSourceTable.AsEnumerable().FirstOrDefault((DataRow t) => t["ValidFrom"] == DBNull.Value) == null)
				{
					DataRow dataRow = dataSourceTable.NewRow();
					dataRow["empID"] = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("empID"));
					dataSourceTable.Rows.Add(dataRow);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void GvDocOnShowingEditor(object sender, CancelEventArgs e)
	{
		if (gv_Doc.FocusedColumn.FieldName == "SchID")
		{
			e.Cancel = true;
		}
	}

	private async Task frm_WorkScheduleRegister_Load(object sender, EventArgs e)
	{
		try
		{
			await LoadInitForm();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadInitForm()
	{
		_ = 3;
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("branch", typeof(int));
			dataTable.Columns.Add("dept", typeof(string));
			dataTable.Columns.Add("empId", typeof(int));
			dataTable.Columns.Add("empName", typeof(string));
			dataTable.Columns.Add("status", typeof(string));
			dataTable.Columns.Add("validFrom", typeof(DateTime));
			dataTable.Columns.Add("validTo", typeof(DateTime));
			DataRow dataRow = dataTable.NewRow();
			dataRow["validFrom"] = DateTime.Today;
			dataRow["validTo"] = DateTime.Today;
			dataTable.Rows.Add(dataRow);
			DataSource = dataTable;
			DataTable dataTable2 = await _httpClient.EmployeeMasterClient.LoadComboDept();
			luk_Department.Properties.DataSource = dataTable2.Copy();
			luk_Department.Properties.DisplayMember = "Name";
			luk_Department.Properties.ValueMember = "Code";
			rep_deptAdvanced.DataSource = dataTable2.Copy();
			rep_deptAdvanced.DisplayMember = "Name";
			rep_deptAdvanced.ValueMember = "Code";
			rep_deptAdvanced.BestFitMode = BestFitMode.BestFitResizePopup;
			rep_deptAdvanced.ForceInitialize();
			rep_deptAdvanced.PopulateColumns();
			rep_deptAdvanced.Columns["Code"].Visible = false;
			rep_deptAdvanced.Columns["Remarks"].Visible = false;
			DataTable dataTable3 = await _httpClient.EmployeeMasterClient.GetEmployeeStatus();
			luk_Status.Properties.DataSource = dataTable3.Copy();
			luk_Status.Properties.DisplayMember = "Name";
			luk_Status.Properties.ValueMember = "Code";
			rep_Status2.DataSource = dataTable3.Copy();
			rep_Status2.DisplayMember = "Name";
			rep_Status2.ValueMember = "Code";
			rep_Status2.BestFitMode = BestFitMode.BestFitResizePopup;
			rep_Status2.ForceInitialize();
			rep_Status2.PopulateColumns();
			rep_Status2.Columns["Code"].Visible = false;
			DataTable dtBranches = await _httpClient.EmployeeMasterClient.LoadComboBranch();
			luk_Branch.Properties.DataSource = dtBranches.Copy();
			luk_Branch.Properties.DisplayMember = "Name";
			luk_Branch.Properties.ValueMember = "Code";
			luk_Branch.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
			luk_Branch.Properties.ForceInitialize();
			luk_Branch.Properties.PopulateColumns();
			luk_Branch.Properties.Columns["Code"].Visible = false;
			rep_Branch2.DataSource = dtBranches.Copy();
			rep_Branch2.DisplayMember = "Name";
			rep_Branch2.ValueMember = "Code";
			rep_Branch2.BestFitMode = BestFitMode.BestFitResizePopup;
			rep_Branch2.ForceInitialize();
			rep_Branch2.PopulateColumns();
			rep_Branch2.Columns["Code"].Visible = false;
			btn_EditMain.Visible = true;
			_dtSchedules = await _httpClient.WorkScheduleSetupClient.GetAll();
			LookUpEditEx lookUpEditEx = new LookUpEditEx();
			lookUpEditEx.SetDataSource(_dtSchedules, "SchName", "SchID", emptyRow: true, emptyTop: true, hideOtherColumns: true);
			lookUpEditEx.Properties.NullText = "";
			rep_Schedule.Assign(lookUpEditEx.Properties);
			rep_ScheduleLine.Assign(lookUpEditEx.Properties);
			if (dtBranches.Rows.Count == 1)
			{
				luk_Branch.ItemIndex = 0;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btnSearch_Click(object sender, EventArgs e)
	{
		if (string.IsNullOrWhiteSpace(Function.ToString(luk_Branch.EditValue)))
		{
			WinCommons.Warning(Enums.MessageCode.PleaseSelectPranchBeforeContinue, "");
		}
		else
		{
			await LoadData();
		}
	}

	private async Task LoadData()
	{
		try
		{
			DataTable dt = DataSource as DataTable;
			DataTable dataTable = await _httpClient.WorkScheduleRegisterClient.Search(dt);
			if (dataTable.HasColumn())
			{
				grd_Doc.DataSource = dataTable;
				dataTable.AcceptChanges();
				grd_Doc.RefreshDataSource();
				grd_Doc.DataSourceTable.AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_EditMain_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataTable = DataSource as DataTable;
			frm_EmployeeScheduleEdit frm_EmployeeScheduleEdit2 = new frm_EmployeeScheduleEdit
			{
				MdiParent = MdiParent
			};
			if (dataTable.IsNotNull())
			{
				frm_EmployeeScheduleEdit2.DtSearch = dataTable.Copy();
			}
			frm_EmployeeScheduleEdit2.StartPosition = FormStartPosition.CenterScreen;
			frm_EmployeeScheduleEdit2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Save_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Update)
			{
				btn_Save.Enabled = false;
				btn_Cancel.Enabled = false;
				await SaveData();
				btn_Save.Enabled = true;
				btn_Cancel.Enabled = true;
			}
			else
			{
				Close();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task SaveData()
	{
		try
		{
			DataTable dataSourceTable = grd_Details.DataSourceTable;
			if (dataSourceTable != null)
			{
				WinCommons._OpenProcessing();
				string text = await _httpClient.WorkScheduleRegisterClient.SaveDoc(dataSourceTable);
				WinCommons._CloseProcessing();
				if (string.IsNullOrWhiteSpace(text))
				{
					FormMode = Enums.FormViewModeType.View;
					WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, text);
				}
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.NoDataToSave, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btn_Cancel_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Update && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSaveBeforeClose, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				await SaveData();
			}
			Close();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void gv_Doc_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			int empID = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("empID"));
			await GetDetails(empID);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void removeRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dt = grd_Details.DataSourceTable;
			object focusedRowCellValue = gv_Details.GetFocusedRowCellValue("LineNum");
			object focusedRowCellValue2 = gv_Details.GetFocusedRowCellValue("ValidFrom");
			int empID = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("empID"));
			if (string.IsNullOrWhiteSpace(Function.ToString(focusedRowCellValue)) && string.IsNullOrWhiteSpace(Function.ToString(focusedRowCellValue2)))
			{
				return;
			}
			if (string.IsNullOrWhiteSpace(Function.ToString(focusedRowCellValue)))
			{
				gv_Details.BeginUpdate();
				gv_Details.DeleteSelectedRows();
				gv_Details.EndUpdate();
				dt.AcceptChanges();
				grd_Details.RefreshDataSource();
			}
			else if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDeleteThisRow, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				string text = await _httpClient.WorkScheduleRegisterClient.Remove(Function.ParseInt(focusedRowCellValue), empID);
				if (string.IsNullOrEmpty(text))
				{
					gv_Details.BeginUpdate();
					gv_Details.DeleteSelectedRows();
					gv_Details.EndUpdate();
					dt.AcceptChanges();
					grd_Details.RefreshDataSource();
					WinCommons.Success(Enums.MessageCode.DeleteSuccess, "");
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, text);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void luk_Branch_EditValueChanged(object sender, EventArgs e)
	{
		int selectedBranch = Function.ParseInt(luk_Branch.EditValue);
		EnumerableRowCollection<DataRow> source = from t in _dtSchedules.AsEnumerable()
			where Function.ParseInt(t["branch"]) == selectedBranch
			select t;
		DataTable dt = _dtSchedules.Clone();
		if (source.Any())
		{
			dt = source.CopyToDataTable();
		}
		LookUpEditEx lookUpEditEx = new LookUpEditEx();
		lookUpEditEx.SetDataSource(dt, "SchName", "SchID", emptyRow: true, emptyTop: true, hideOtherColumns: true);
		lookUpEditEx.Properties.NullText = "";
		rep_Schedule.Assign(lookUpEditEx.Properties);
		rep_ScheduleLine.Assign(lookUpEditEx.Properties);
	}

	private async Task GetDetails(int empID)
	{
		try
		{
			if (empID > 0)
			{
				DataTable dataTable = await _httpClient.WorkScheduleRegisterClient.GetDetail(empID);
				DataRow dataRow = dataTable.NewRow();
				dataRow["empID"] = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("empID"));
				dataTable.Rows.Add(dataRow);
				grd_Details.DataSource = dataTable;
				grd_Details.RefreshDataSource();
			}
			else
			{
				grd_Details.DataSource = new DataTable();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.HumanResources.TimeSheeting.frm_WorkScheduleRegister));
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		this.repositoryItemCalcEditEx3 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx4 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx1 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.removeRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.lbl_Department = new Apzon.Controls.Controls.LabelLine();
		this.lbl_Branch = new Apzon.Controls.Controls.LabelLine();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.txt_EmpNo = new Apzon.Controls.Editor.TextEditEx();
		this.txt_EmpName = new Apzon.Controls.Editor.TextEditEx();
		this.lbl_EmpNo = new Apzon.Controls.Controls.LabelLine();
		this.lbl_FirstName = new Apzon.Controls.Controls.LabelLine();
		this.btnSearch = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_EditMain = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.luk_Department = new Apzon.Controls.Editor.CheckedComboBoxEditEx();
		this.luk_Status = new Apzon.Controls.Editor.CheckedComboBoxEditEx();
		this.luk_Branch = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.grd_Details = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Details = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_ValidFrom = new Apzon.Controls.Editor.RepositoryItemDateEditEx();
		this.gridColumnEx3 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_ValidTo = new Apzon.Controls.Editor.RepositoryItemDateEditEx();
		this.gridColumnEx4 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_ScheduleLine = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx7 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Branch2 = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx6 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Status2 = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumn10 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_deptAdvanced = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gc_empNo3 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_CFL2 = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gridColumnEx10 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn12 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Schedule = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridViewEx2 = new Apzon.Controls.Grid.GridViewEx();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx1).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txt_EmpNo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_EmpName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Department.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Status.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Branch.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Details).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Details).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ValidFrom).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ValidFrom.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ValidTo).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ValidTo.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ScheduleLine).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Branch2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Status2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_deptAdvanced).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CFL2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Schedule).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridViewEx2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		base.SuspendLayout();
		this.repositoryItemCalcEditEx3.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx3.IsSearchControl = false;
		this.repositoryItemCalcEditEx3.Name = "repositoryItemCalcEditEx3";
		this.repositoryItemCalcEditEx4.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx4.IsSearchControl = false;
		this.repositoryItemCalcEditEx4.Name = "repositoryItemCalcEditEx4";
		this.repositoryItemCalcEditEx1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx1.IsSearchControl = false;
		this.repositoryItemCalcEditEx1.Name = "repositoryItemCalcEditEx1";
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.removeRowToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(144, 26);
		this.removeRowToolStripMenuItem.Name = "removeRowToolStripMenuItem";
		this.removeRowToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
		this.removeRowToolStripMenuItem.Text = "Remove Row";
		this.removeRowToolStripMenuItem.Click += new System.EventHandler(this.removeRowToolStripMenuItem_Click);
		this.lbl_Department.EditControl = null;
		this.lbl_Department.EditValue = "Department";
		this.lbl_Department.FieldName = null;
		this.lbl_Department.HideWhenEmpty = false;
		this.lbl_Department.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_Department.Location = new System.Drawing.Point(4, 39);
		this.lbl_Department.Name = "lbl_Department";
		this.lbl_Department.ReadOnly = false;
		this.lbl_Department.SettingInVisible = false;
		this.lbl_Department.SettingReadonly = false;
		this.lbl_Department.ShowUnderLine = true;
		this.lbl_Department.Size = new System.Drawing.Size(117, 20);
		this.lbl_Department.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_Department.TabIndex = 74;
		this.lbl_Department.Utext = "Department";
		this.lbl_Branch.EditControl = null;
		this.lbl_Branch.EditValue = "Branch";
		this.lbl_Branch.FieldName = null;
		this.lbl_Branch.HideWhenEmpty = false;
		this.lbl_Branch.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_Branch.Location = new System.Drawing.Point(4, 13);
		this.lbl_Branch.Name = "lbl_Branch";
		this.lbl_Branch.ReadOnly = false;
		this.lbl_Branch.SettingInVisible = false;
		this.lbl_Branch.SettingReadonly = false;
		this.lbl_Branch.ShowUnderLine = true;
		this.lbl_Branch.Size = new System.Drawing.Size(117, 20);
		this.lbl_Branch.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_Branch.TabIndex = 76;
		this.lbl_Branch.Utext = "Branch";
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "Status";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(4, 64);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(117, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 85;
		this.labelLine3.Utext = "Status";
		this.txt_EmpNo.EditControl = null;
		this.txt_EmpNo.FieldName = "empID";
		this.txt_EmpNo.IsSearchControl = true;
		this.txt_EmpNo.Location = new System.Drawing.Point(455, 10);
		this.txt_EmpNo.Name = "txt_EmpNo";
		this.txt_EmpNo.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.txt_EmpNo.SettingInVisible = false;
		this.txt_EmpNo.SettingReadonly = false;
		this.txt_EmpNo.Size = new System.Drawing.Size(167, 20);
		this.txt_EmpNo.TabIndex = 4;
		this.txt_EmpNo.Tag = " ";
		this.txt_EmpName.EditControl = null;
		this.txt_EmpName.FieldName = "empName";
		this.txt_EmpName.IsSearchControl = true;
		this.txt_EmpName.Location = new System.Drawing.Point(455, 36);
		this.txt_EmpName.Name = "txt_EmpName";
		this.txt_EmpName.SettingInVisible = false;
		this.txt_EmpName.SettingReadonly = false;
		this.txt_EmpName.Size = new System.Drawing.Size(167, 20);
		this.txt_EmpName.TabIndex = 5;
		this.lbl_EmpNo.EditControl = null;
		this.lbl_EmpNo.EditValue = "Employee No.";
		this.lbl_EmpNo.FieldName = null;
		this.lbl_EmpNo.HideWhenEmpty = false;
		this.lbl_EmpNo.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_EmpNo.Location = new System.Drawing.Point(330, 14);
		this.lbl_EmpNo.Name = "lbl_EmpNo";
		this.lbl_EmpNo.ReadOnly = false;
		this.lbl_EmpNo.SettingInVisible = false;
		this.lbl_EmpNo.SettingReadonly = false;
		this.lbl_EmpNo.ShowUnderLine = true;
		this.lbl_EmpNo.Size = new System.Drawing.Size(126, 20);
		this.lbl_EmpNo.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_EmpNo.TabIndex = 89;
		this.lbl_EmpNo.Utext = "Employee No.";
		this.lbl_FirstName.EditControl = null;
		this.lbl_FirstName.EditValue = "Employee's Name";
		this.lbl_FirstName.FieldName = null;
		this.lbl_FirstName.HideWhenEmpty = false;
		this.lbl_FirstName.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_FirstName.Location = new System.Drawing.Point(330, 39);
		this.lbl_FirstName.Name = "lbl_FirstName";
		this.lbl_FirstName.ReadOnly = false;
		this.lbl_FirstName.SettingInVisible = false;
		this.lbl_FirstName.SettingReadonly = false;
		this.lbl_FirstName.ShowUnderLine = true;
		this.lbl_FirstName.Size = new System.Drawing.Size(126, 20);
		this.lbl_FirstName.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_FirstName.TabIndex = 88;
		this.lbl_FirstName.Utext = "Employee's Name";
		this.btnSearch.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btnSearch.Appearance.Options.UseBackColor = true;
		this.btnSearch.EditControl = null;
		this.btnSearch.EditValue = "Search";
		this.btnSearch.FieldName = null;
		this.btnSearch.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnSearch.ImageOptions.Image");
		this.btnSearch.IsSearchControl = true;
		this.btnSearch.Location = new System.Drawing.Point(523, 62);
		this.btnSearch.Name = "btnSearch";
		this.btnSearch.ReadOnly = false;
		this.btnSearch.SettingInVisible = false;
		this.btnSearch.SettingReadonly = false;
		this.btnSearch.Size = new System.Drawing.Size(99, 20);
		this.btnSearch.TabIndex = 6;
		this.btnSearch.Text = "Search";
		this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
		this.btn_EditMain.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_EditMain.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_EditMain.Appearance.Options.UseBackColor = true;
		this.btn_EditMain.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_EditMain.EditControl = null;
		this.btn_EditMain.EditValue = "Edit Schedules";
		this.btn_EditMain.FieldName = null;
		this.btn_EditMain.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_EditMain.ImageOptions.Image");
		this.btn_EditMain.IsSearchControl = true;
		this.btn_EditMain.Location = new System.Drawing.Point(679, 465);
		this.btn_EditMain.Name = "btn_EditMain";
		this.btn_EditMain.ReadOnly = false;
		this.btn_EditMain.SettingInVisible = false;
		this.btn_EditMain.SettingReadonly = false;
		this.btn_EditMain.Size = new System.Drawing.Size(138, 20);
		this.btn_EditMain.TabIndex = 11;
		this.btn_EditMain.Text = "Edit Schedules";
		this.btn_EditMain.Click += new System.EventHandler(this.btn_EditMain_Click);
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Save.Appearance.Options.UseBackColor = true;
		this.btn_Save.EditControl = null;
		this.btn_Save.EditValue = "OK";
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(6, 465);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.ReadOnly = false;
		this.btn_Save.SettingInVisible = false;
		this.btn_Save.SettingReadonly = false;
		this.btn_Save.Size = new System.Drawing.Size(61, 20);
		this.btn_Save.TabIndex = 95;
		this.btn_Save.Text = "OK";
		this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Cancel.Appearance.Options.UseBackColor = true;
		this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(73, 465);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(93, 20);
		this.btn_Cancel.TabIndex = 94;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.luk_Department.EditControl = null;
		this.luk_Department.FieldName = "dept";
		this.luk_Department.IsSearchControl = true;
		this.luk_Department.Location = new System.Drawing.Point(120, 36);
		this.luk_Department.Name = "luk_Department";
		this.luk_Department.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Department.SettingInVisible = false;
		this.luk_Department.SettingReadonly = false;
		this.luk_Department.Size = new System.Drawing.Size(167, 20);
		this.luk_Department.TabIndex = 2;
		this.luk_Status.EditControl = null;
		this.luk_Status.FieldName = "status";
		this.luk_Status.IsSearchControl = true;
		this.luk_Status.Location = new System.Drawing.Point(120, 62);
		this.luk_Status.Name = "luk_Status";
		this.luk_Status.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Status.SettingInVisible = false;
		this.luk_Status.SettingReadonly = false;
		this.luk_Status.Size = new System.Drawing.Size(167, 20);
		this.luk_Status.TabIndex = 3;
		this.luk_Branch.EditControl = null;
		this.luk_Branch.FieldName = "branch";
		this.luk_Branch.IsSearchControl = true;
		this.luk_Branch.Location = new System.Drawing.Point(120, 10);
		this.luk_Branch.Name = "luk_Branch";
		this.luk_Branch.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Branch.Properties.IsSearchControl = false;
		this.luk_Branch.Properties.NullText = "";
		this.luk_Branch.SettingInVisible = false;
		this.luk_Branch.SettingReadonly = false;
		this.luk_Branch.Size = new System.Drawing.Size(167, 20);
		this.luk_Branch.TabIndex = 1;
		this.luk_Branch.EditValueChanged += new System.EventHandler(this.luk_Branch_EditValueChanged);
		this.grd_Details.AllowPasteExcel = false;
		this.grd_Details.ClipboardData = null;
		this.grd_Details.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_Details.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Details.Location = new System.Drawing.Point(0, 0);
		this.grd_Details.MainView = this.gv_Details;
		this.grd_Details.Name = "grd_Details";
		this.grd_Details.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.rep_ScheduleLine, this.rep_ValidFrom, this.rep_ValidTo });
		this.grd_Details.Size = new System.Drawing.Size(378, 369);
		this.grd_Details.TabIndex = 10;
		this.grd_Details.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Details });
		this.gv_Details.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Details.ColumnKey = null;
		this.gv_Details.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.gridColumnEx2, this.gridColumnEx3, this.gridColumnEx4 });
		this.gv_Details.DisableFormSetting = false;
		this.gv_Details.DisplayException = false;
		this.gv_Details.EditColumn = null;
		this.gv_Details.FieldCurrency = null;
		this.gv_Details.FieldRate = null;
		this.gv_Details.FormId = "oEmployeeScheduleRegister";
		this.gv_Details.GridControl = this.grd_Details;
		this.gv_Details.IgnoreFilter = false;
		this.gv_Details.IndexEditRow = -1;
		this.gv_Details.IndicatorWidth = 40;
		this.gv_Details.IsApzTable = false;
		this.gv_Details.IsInsertNewRow = false;
		this.gv_Details.IsLoadingData = false;
		this.gv_Details.IsMappingContextAction = false;
		this.gv_Details.ItemId = "ESR1";
		this.gv_Details.LstColumnAllowBatch = null;
		this.gv_Details.LstColumnNotAllowBatch = null;
		this.gv_Details.LstColumnNotEditable = null;
		this.gv_Details.LstColumNotSetting = null;
		this.gv_Details.Name = "gv_Details";
		this.gv_Details.OptionsView.ShowGroupPanel = false;
		this.gv_Details.UseBatchMod = false;
		this.gridColumnEx2.Caption = "From Date";
		this.gridColumnEx2.ColumnEdit = this.rep_ValidFrom;
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.FieldName = "ValidFrom";
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.UnitField = null;
		this.gridColumnEx2.Visible = true;
		this.gridColumnEx2.VisibleIndex = 0;
		this.rep_ValidFrom.AutoHeight = false;
		this.rep_ValidFrom.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_ValidFrom.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_ValidFrom.IsSearchControl = false;
		this.rep_ValidFrom.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.rep_ValidFrom.Name = "rep_ValidFrom";
		this.gridColumnEx3.Caption = "To Date";
		this.gridColumnEx3.ColumnEdit = this.rep_ValidTo;
		this.gridColumnEx3.CurrencyDateField = null;
		this.gridColumnEx3.CurrencyField = null;
		this.gridColumnEx3.CurrencyFixed = null;
		this.gridColumnEx3.CurrencyFixedRate = 0.0;
		this.gridColumnEx3.CurrencyRateField = null;
		this.gridColumnEx3.FieldName = "ValidTo";
		this.gridColumnEx3.Name = "gridColumnEx3";
		this.gridColumnEx3.PrimaryKey = false;
		this.gridColumnEx3.UnitField = null;
		this.gridColumnEx3.Visible = true;
		this.gridColumnEx3.VisibleIndex = 1;
		this.rep_ValidTo.AutoHeight = false;
		this.rep_ValidTo.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_ValidTo.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_ValidTo.IsSearchControl = false;
		this.rep_ValidTo.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.rep_ValidTo.Name = "rep_ValidTo";
		this.gridColumnEx4.Caption = "Schedule";
		this.gridColumnEx4.ColumnEdit = this.rep_ScheduleLine;
		this.gridColumnEx4.CurrencyDateField = null;
		this.gridColumnEx4.CurrencyField = null;
		this.gridColumnEx4.CurrencyFixed = null;
		this.gridColumnEx4.CurrencyFixedRate = 0.0;
		this.gridColumnEx4.CurrencyRateField = null;
		this.gridColumnEx4.FieldName = "SchID";
		this.gridColumnEx4.Name = "gridColumnEx4";
		this.gridColumnEx4.PrimaryKey = false;
		this.gridColumnEx4.UnitField = null;
		this.gridColumnEx4.Visible = true;
		this.gridColumnEx4.VisibleIndex = 2;
		this.rep_ScheduleLine.AutoHeight = false;
		this.rep_ScheduleLine.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_ScheduleLine.Name = "rep_ScheduleLine";
		this.rep_ScheduleLine.NullText = "";
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Doc.Location = new System.Drawing.Point(0, 0);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[5] { this.rep_deptAdvanced, this.rep_CFL2, this.rep_Branch2, this.rep_Status2, this.rep_Schedule });
		this.grd_Doc.Size = new System.Drawing.Size(425, 369);
		this.grd_Doc.TabIndex = 9;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[2] { this.gv_Doc, this.gridViewEx2 });
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = null;
		this.gv_Doc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[7] { this.gridColumnEx7, this.gridColumnEx6, this.gridColumn10, this.gc_empNo3, this.gridColumnEx10, this.gridColumn12, this.gridColumnEx1 });
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = "oEmployeeScheduleRegister";
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.GroupCount = 1;
		this.gv_Doc.IgnoreFilter = false;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 40;
		this.gv_Doc.IsApzTable = false;
		this.gv_Doc.IsInsertNewRow = false;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.IsMappingContextAction = false;
		this.gv_Doc.ItemId = "OESR";
		this.gv_Doc.LstColumnAllowBatch = null;
		this.gv_Doc.LstColumnNotAllowBatch = null;
		this.gv_Doc.LstColumnNotEditable = null;
		this.gv_Doc.LstColumNotSetting = null;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsBehavior.AutoExpandAllGroups = true;
		this.gv_Doc.OptionsView.ShowGroupPanel = false;
		this.gv_Doc.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[1]
		{
			new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.gridColumn10, DevExpress.Data.ColumnSortOrder.Ascending)
		});
		this.gv_Doc.UseBatchMod = false;
		this.gv_Doc.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_Doc_FocusedRowObjectChanged);
		this.gridColumnEx7.Caption = "Branch";
		this.gridColumnEx7.ColumnEdit = this.rep_Branch2;
		this.gridColumnEx7.CurrencyDateField = null;
		this.gridColumnEx7.CurrencyField = null;
		this.gridColumnEx7.CurrencyFixed = null;
		this.gridColumnEx7.CurrencyFixedRate = 0.0;
		this.gridColumnEx7.CurrencyRateField = null;
		this.gridColumnEx7.FieldName = "branch";
		this.gridColumnEx7.Name = "gridColumnEx7";
		this.gridColumnEx7.OptionsColumn.ReadOnly = true;
		this.gridColumnEx7.PrimaryKey = false;
		this.gridColumnEx7.UnitField = null;
		this.rep_Branch2.AutoHeight = false;
		this.rep_Branch2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Branch2.Name = "rep_Branch2";
		this.rep_Branch2.NullText = "";
		this.gridColumnEx6.Caption = "Status";
		this.gridColumnEx6.ColumnEdit = this.rep_Status2;
		this.gridColumnEx6.CurrencyDateField = null;
		this.gridColumnEx6.CurrencyField = null;
		this.gridColumnEx6.CurrencyFixed = null;
		this.gridColumnEx6.CurrencyFixedRate = 0.0;
		this.gridColumnEx6.CurrencyRateField = null;
		this.gridColumnEx6.FieldName = "status";
		this.gridColumnEx6.Name = "gridColumnEx6";
		this.gridColumnEx6.OptionsColumn.ReadOnly = true;
		this.gridColumnEx6.PrimaryKey = false;
		this.gridColumnEx6.UnitField = null;
		this.rep_Status2.AutoHeight = false;
		this.rep_Status2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Status2.Name = "rep_Status2";
		this.rep_Status2.NullText = "";
		this.gridColumn10.Caption = "Department";
		this.gridColumn10.ColumnEdit = this.rep_deptAdvanced;
		this.gridColumn10.CurrencyDateField = null;
		this.gridColumn10.CurrencyField = null;
		this.gridColumn10.CurrencyFixed = null;
		this.gridColumn10.CurrencyFixedRate = 0.0;
		this.gridColumn10.CurrencyRateField = null;
		this.gridColumn10.FieldName = "dept";
		this.gridColumn10.Name = "gridColumn10";
		this.gridColumn10.OptionsColumn.ReadOnly = true;
		this.gridColumn10.PrimaryKey = false;
		this.gridColumn10.UnitField = null;
		this.gridColumn10.Visible = true;
		this.gridColumn10.VisibleIndex = 0;
		this.rep_deptAdvanced.AutoHeight = false;
		this.rep_deptAdvanced.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_deptAdvanced.Name = "rep_deptAdvanced";
		this.rep_deptAdvanced.NullText = "";
		this.gc_empNo3.Caption = "Employee No.";
		this.gc_empNo3.ColumnEdit = this.rep_CFL2;
		this.gc_empNo3.CurrencyDateField = null;
		this.gc_empNo3.CurrencyField = null;
		this.gc_empNo3.CurrencyFixed = null;
		this.gc_empNo3.CurrencyFixedRate = 0.0;
		this.gc_empNo3.CurrencyRateField = null;
		this.gc_empNo3.FieldName = "empID";
		this.gc_empNo3.Name = "gc_empNo3";
		this.gc_empNo3.OptionsColumn.ReadOnly = true;
		this.gc_empNo3.PrimaryKey = true;
		this.gc_empNo3.UnitField = null;
		this.gc_empNo3.Visible = true;
		this.gc_empNo3.VisibleIndex = 0;
		this.rep_CFL2.AutoHeight = false;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_CFL2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_CFL2.Name = "rep_CFL2";
		this.rep_CFL2.ReadOnly = true;
		this.rep_CFL2.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.gridColumnEx10.Caption = "Extened Employee No";
		this.gridColumnEx10.CurrencyDateField = null;
		this.gridColumnEx10.CurrencyField = null;
		this.gridColumnEx10.CurrencyFixed = null;
		this.gridColumnEx10.CurrencyFixedRate = 0.0;
		this.gridColumnEx10.CurrencyRateField = null;
		this.gridColumnEx10.FieldName = "ExtEmpNo";
		this.gridColumnEx10.Name = "gridColumnEx10";
		this.gridColumnEx10.OptionsColumn.ReadOnly = true;
		this.gridColumnEx10.PrimaryKey = false;
		this.gridColumnEx10.UnitField = null;
		this.gridColumnEx10.Visible = true;
		this.gridColumnEx10.VisibleIndex = 2;
		this.gridColumn12.Caption = "Employee Name";
		this.gridColumn12.CurrencyDateField = null;
		this.gridColumn12.CurrencyField = null;
		this.gridColumn12.CurrencyFixed = null;
		this.gridColumn12.CurrencyFixedRate = 0.0;
		this.gridColumn12.CurrencyRateField = null;
		this.gridColumn12.FieldName = "empName";
		this.gridColumn12.Name = "gridColumn12";
		this.gridColumn12.OptionsColumn.ReadOnly = true;
		this.gridColumn12.PrimaryKey = false;
		this.gridColumn12.UnitField = null;
		this.gridColumn12.Visible = true;
		this.gridColumn12.VisibleIndex = 1;
		this.gridColumnEx1.Caption = "Current Schedule";
		this.gridColumnEx1.ColumnEdit = this.rep_Schedule;
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "SchID";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.PrimaryKey = false;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx1.Visible = true;
		this.gridColumnEx1.VisibleIndex = 3;
		this.rep_Schedule.AutoHeight = false;
		this.rep_Schedule.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Schedule.Name = "rep_Schedule";
		this.rep_Schedule.NullText = "";
		this.gridViewEx2.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gridViewEx2.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gridViewEx2.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gridViewEx2.ColumnKey = null;
		this.gridViewEx2.DisableFormSetting = false;
		this.gridViewEx2.DisplayException = false;
		this.gridViewEx2.EditColumn = null;
		this.gridViewEx2.FieldCurrency = null;
		this.gridViewEx2.FieldRate = null;
		this.gridViewEx2.FormId = "oEmployeeScheduleRegister";
		this.gridViewEx2.GridControl = this.grd_Doc;
		this.gridViewEx2.IgnoreFilter = false;
		this.gridViewEx2.IndexEditRow = -1;
		this.gridViewEx2.IndicatorWidth = 35;
		this.gridViewEx2.IsApzTable = false;
		this.gridViewEx2.IsInsertNewRow = false;
		this.gridViewEx2.IsLoadingData = false;
		this.gridViewEx2.IsMappingContextAction = false;
		this.gridViewEx2.ItemId = "OGRID2";
		this.gridViewEx2.LstColumnAllowBatch = null;
		this.gridViewEx2.LstColumnNotAllowBatch = null;
		this.gridViewEx2.LstColumnNotEditable = null;
		this.gridViewEx2.LstColumNotSetting = null;
		this.gridViewEx2.Name = "gridViewEx2";
		this.gridViewEx2.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gridViewEx2.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gridViewEx2.OptionsPrint.AutoWidth = false;
		this.gridViewEx2.OptionsPrint.UsePrintStyles = false;
		this.gridViewEx2.OptionsView.BestFitMaxRowCount = 100;
		this.gridViewEx2.OptionsView.ColumnAutoWidth = false;
		this.gridViewEx2.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridViewEx2.OptionsView.ShowGroupedColumns = true;
		this.gridViewEx2.OptionsView.ShowGroupPanel = false;
		this.gridViewEx2.UseBatchMod = false;
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Location = new System.Drawing.Point(4, 90);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.grd_Doc);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.grd_Details);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(813, 369);
		this.splitContainerControl1.SplitterPosition = 425;
		this.splitContainerControl1.TabIndex = 1;
		this.splitContainerControl1.Text = "splitContainerControl1";
		base.AcceptButton = this.btn_Save;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btn_Cancel;
		base.ClientSize = new System.Drawing.Size(827, 493);
		base.Controls.Add(this.splitContainerControl1);
		base.Controls.Add(this.luk_Status);
		base.Controls.Add(this.luk_Department);
		base.Controls.Add(this.luk_Branch);
		base.Controls.Add(this.btn_Save);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_EditMain);
		base.Controls.Add(this.btnSearch);
		base.Controls.Add(this.txt_EmpNo);
		base.Controls.Add(this.txt_EmpName);
		base.Controls.Add(this.lbl_EmpNo);
		base.Controls.Add(this.lbl_FirstName);
		base.Controls.Add(this.labelLine3);
		base.Controls.Add(this.lbl_Branch);
		base.Controls.Add(this.lbl_Department);
		base.FormId = "oEmployeeScheduleRegister";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_WorkScheduleRegister";
		base.ShowInTaskbar = false;
		this.Text = "Employee Schedule Register";
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx1).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txt_EmpNo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_EmpName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Department.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Status.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Branch.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Details).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Details).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ValidFrom.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ValidFrom).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ValidTo.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ValidTo).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ScheduleLine).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Branch2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Status2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_deptAdvanced).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CFL2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Schedule).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridViewEx2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
