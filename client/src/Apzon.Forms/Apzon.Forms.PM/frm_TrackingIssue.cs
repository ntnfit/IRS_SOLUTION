using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Editor.MyLookUpEdit;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.Properties;
using Apzon.Forms.SalesOppotunity;
using Apzon.Forms.UserControls;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;

namespace Apzon.Forms.PM;

public class frm_TrackingIssue : XtraFormEx
{
	private const string OkString = "OK";

	private const string UpdateString = "Update";

	private const string AddString = "Add";

	private DataTable _dtdoc;

	private DataTable _dtHistory;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private Enums.FormViewModeType _formView;

	private bool _isLoading;

	private bool isNewIssue;

	public DataRow DataRowFromCal;

	public bool CallFromCal;

	public bool _duplicate;

	public bool find;

	public bool _ischangeNote;

	public bool OkToClose;

	public Func<Task> RefreshSettings;

	public Func<int, Task> SendDataBackToMain;

	private int employeeId;

	private readonly Dictionary<string, string> _lstPriority = new Dictionary<string, string>
	{
		{ "4", "Critical" },
		{ "3", "High" },
		{ "2", "Medium" },
		{ "1", "Low" }
	};

	public Enums.FormViewModeType OpenFormViewMode = Enums.FormViewModeType.View;

	private bool _isMapping;

	private IContainer components;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem addToolStripMenuItem;

	private DateEditEx date_searchTD;

	private LabelLine labelLine26;

	private DateEditEx date_searchFD;

	private LabelLine labelLine25;

	private LabelLine labelLine24;

	private TextEditEx txt_noSearch;

	private SimpleButtonEx btn_Find;

	private ButtonEditCFL btn_ReportBy;

	private ButtonEditCFL btn_HandleBy;

	private ButtonEditCFL btn_Owner;

	private ButtonEditCFL btn_ProjectCode;

	private ButtonEditCFL btn_BpCode;

	private LabelLine labelLine6;

	private LabelLine labelLine5;

	private LabelLine lbl_ReportBy;

	private LabelLine labelLine3;

	private LabelLine labelLine2;

	private LabelLine labelLine1;

	private SplitContainerControl splitContainerControl1;

	private GridControlEx grd_TrackingIssue;

	public GridViewEx gv_TrackingIssue;

	private GridColumnEx gridColumnEx1;

	private GridColumnEx gridColumnEx2;

	private GridColumnEx gridColumnEx3;

	private RepositoryItemLookUpEditDefineNew luk_typeGrid;

	private GridColumnEx gc_Status;

	private RepositoryItemLookUpEditDefineNew luk_statusGrid;

	private RepositoryItemButtonEditCFL btn_gridReportBy;

	private GridColumnEx gridColumnEx7;

	private RepositoryItemButtonEditCFL btn_gridHandBy;

	private GridColumnEx gridColumnEx8;

	private GridColumnEx gridColumnEx9;

	private GridColumnEx gridColumnEx10;

	private RepositoryItemDateEditEx date_gridRes;

	private GridColumnEx gridColumnEx11;

	private GridColumnEx TaskCode;

	private GridColumnEx gridColumnEx13;

	private GridColumnEx gridColumnEx14;

	private RepositoryItemButtonEditCFL btn_gridProject;

	private GridColumnEx gridColumnEx15;

	private GridColumnEx gc_priority;

	private RepositoryItemLookUpEditSimple luk_priorityGrid;

	private GridViewEx gridViewEx2;

	private RepositoryItemButtonEditCFL btn_gridBpCode;

	private ToolStripMenuItem duplicateToolStripMenuItem;

	private SimpleButtonEx btn_ShowDetail;

	private FolderBrowserDialogEx folderBrowserDialogEx1;

	private GridColumnEx gridColumnEx12;

	private GridColumnEx gridColumnEx4;

	private GridColumnEx gridColumnEx24;

	private GridColumnEx gridColumnEx25;

	private GridColumnEx gridColumnEx26;

	private GridColumnEx gridColumnEx27;

	private GridColumnEx gridColumnEx6;

	private RepositoryItemDateEditEx repositoryItemDateEditEx1;

	private ToolStripSeparator toolStripSeparator1;

	private ToolStripMenuItem newTaskToolStripMenuItem;

	private ToolStripMenuItem relatedActivitiesToolStripMenuItem;

	private ucTrackingIssue ucTrackingIssue1;

	private SimpleButtonEx btn_Cancel;

	public LookUpEditDefineNew luk_statusFind;

	private ToolStripMenuItem removeToolStripMenuItem;

	public int id { get; set; }

	public DataTable MappingSource { get; set; }

	public ContextActionMappingImpl[] MappingDocument { get; set; }

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		try
		{
			_formMode = value;
			switch (value)
			{
			case Enums.FormViewModeType.Add:
				ucTrackingIssue1.btn_Ok.Text = "Add";
				ucTrackingIssue1.btn_ChangeStatus.Enabled = false;
				ucTrackingIssue1.luk_ChangeStatus.Enabled = false;
				ucTrackingIssue1.ucComment1.Enabled = false;
				break;
			case Enums.FormViewModeType.Update:
				ucTrackingIssue1.btn_Ok.Text = "Update";
				ucTrackingIssue1.btn_ChangeStatus.Enabled = true;
				ucTrackingIssue1.luk_ChangeStatus.Enabled = true;
				ucTrackingIssue1.ucComment1.Enabled = true;
				break;
			case Enums.FormViewModeType.View:
				ucTrackingIssue1.btn_Ok.Text = "OK";
				ucTrackingIssue1.btn_ChangeStatus.Enabled = true;
				ucTrackingIssue1.luk_ChangeStatus.Enabled = true;
				ucTrackingIssue1.ucComment1.Enabled = true;
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public frm_TrackingIssue()
	{
		InitForm();
	}

	public frm_TrackingIssue(bool isFocusCommentTab, int openId)
	{
		InitForm();
		id = openId;
		if (isFocusCommentTab)
		{
			splitContainerControl1.PanelVisibility = SplitPanelVisibility.Both;
			ucTrackingIssue1.xtraTabControl1.SelectedTabPage = ucTrackingIssue1.xtraTabPageComment;
		}
	}

	private void InitForm()
	{
		InitializeComponent();
		splitContainerControl1.PanelVisibility = SplitPanelVisibility.Panel1;
		ucTrackingIssue1.AutoScrollMinSize = new Size(561, 618);
		OnLoadAsync += frm_TrackingIssue_Load;
		ucAttachment ucAttachment2 = ucTrackingIssue1.ucAttachment1;
		ucAttachment2.BrowsedFile = (Action<List<DataRow>>)Delegate.Combine(ucAttachment2.BrowsedFile, (Action<List<DataRow>>)((List<DataRow> rowList) =>
		{
			foreach (DataRow row in rowList)
			{
				ucTrackingIssue1.FileNameAtc(row, 1);
			}
			if (ucTrackingIssue1.btn_Ok.Text != "Add")
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}));
		ucAttachment ucAttachment3 = ucTrackingIssue1.ucAttachment1;
		ucAttachment3.BeforeDelete = (Action<int>)Delegate.Combine(ucAttachment3.BeforeDelete, (Action<int>)((int i) =>
		{
			ucTrackingIssue1.FileNameAtc(ucTrackingIssue1.ucAttachment1.gv_File.GetFocusedDataRow(), 2);
		}));
		ucAttachment ucAttachment4 = ucTrackingIssue1.ucAttachment1;
		ucAttachment4.DeletedFile = (Action)Delegate.Combine(ucAttachment4.DeletedFile, (Action)(() =>
		{
			if (ucTrackingIssue1.btn_Ok.Text != "Add")
			{
				ucTrackingIssue1.btn_Ok.Text = "Update";
			}
		}));
		ucTrackingIssue1.ucComment1.BeforeSendComment += ucTrackingIssue1.UcComment1_BeforeSendComment;
		ucTrackingIssue1.ucAttachment1.gv_File.Columns["FileName"].VisibleIndex = 0;
		ucTrackingIssue1.ucAttachment1.gv_File.Columns["trgtPath"].VisibleIndex = 1;
		FormObjectType = ApzObjectTypes.oTrackingIssue;
		ucTrackingIssue ucTrackingIssue2 = ucTrackingIssue1;
		ucTrackingIssue2.SendDataBackToMain = (Func<int, Task>)Delegate.Combine(ucTrackingIssue2.SendDataBackToMain, new Func<int, Task>(ReloadAfterAddNew));
	}

	private async Task ReloadAfterAddNew(int issueId)
	{
		try
		{
			_isLoading = true;
			DataTable dt = grd_TrackingIssue.DataSourceTable.Copy();
			int num = (dt.IsNotNull() ? dt.AsEnumerable().Max((DataRow r) => Function.ParseInt(r["Id"])) : 0);
			if (issueId > num)
			{
				dt.ImportRow((await Search("", "", 0, "", "", issueId, "", "", "")).Rows[0]);
				dt.AcceptChanges();
				grd_TrackingIssue.DataSource = dt;
				grd_TrackingIssue.RefreshDataSource();
				gv_TrackingIssue.FocusedRowHandle = gv_TrackingIssue.LocateByValue("Id", issueId);
			}
			FormMode = Enums.FormViewModeType.View;
			_isLoading = false;
			ucTrackingIssue1._isSuccess = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.InfoCannotLoadData, "");
		}
	}

	private async Task frm_TrackingIssue_Load(object sender, EventArgs e)
	{
		_ = 2;
		try
		{
			if (OpenFormViewMode == Enums.FormViewModeType.Add)
			{
				_isMapping = true;
			}
			await LoadForm();
			await AfterLoad();
			if (OpenFormViewMode != Enums.FormViewModeType.Add)
			{
				return;
			}
			_isMapping = true;
			try
			{
				await REFRESH();
				ContextActionMappingImpl[] mappingDocument = MappingDocument;
				foreach (ContextActionMappingImpl contextActionMappingImpl in mappingDocument)
				{
					switch (contextActionMappingImpl.TargetField)
					{
					case "IssueDate":
						ucTrackingIssue1.date_IDate.EditValue = ContextActionMappingSource.GetValueSource(MappingSource.Rows[0], contextActionMappingImpl, typeof(DateTime));
						break;
					case "IssueType":
						ucTrackingIssue1.luk_Type.EditValue = ContextActionMappingSource.GetValueSource(MappingSource.Rows[0], contextActionMappingImpl, typeof(int));
						break;
					case "Priority":
						ucTrackingIssue1.luk_priority.EditValue = ContextActionMappingSource.GetValueSource(MappingSource.Rows[0], contextActionMappingImpl, typeof(int));
						break;
					case "Status":
						ucTrackingIssue1.luk_Status.EditValue = ContextActionMappingSource.GetValueSource(MappingSource.Rows[0], contextActionMappingImpl, typeof(int));
						break;
					case "Reporter":
						ucTrackingIssue1.btn_report.EditValue = ContextActionMappingSource.GetValueSource(MappingSource.Rows[0], contextActionMappingImpl, typeof(int));
						break;
					case "HandleBy":
						ucTrackingIssue1.btn_handle.EditValue = ContextActionMappingSource.GetValueSource(MappingSource.Rows[0], contextActionMappingImpl, typeof(int));
						break;
					case "SolutionId":
						ucTrackingIssue1.btn_solution.EditValue = ContextActionMappingSource.GetValueSource(MappingSource.Rows[0], contextActionMappingImpl, typeof(int));
						break;
					case "ResponseDate":
						ucTrackingIssue1.date_rDate.EditValue = ContextActionMappingSource.GetValueSource(MappingSource.Rows[0], contextActionMappingImpl, typeof(DateTime));
						break;
					case "BpCode":
						ucTrackingIssue1.btn_bp.EditValue = ContextActionMappingSource.GetValueSource(MappingSource.Rows[0], contextActionMappingImpl, typeof(string));
						break;
					case "ProjectCode":
						ucTrackingIssue1.btn_pr.EditValue = ContextActionMappingSource.GetValueSource(MappingSource.Rows[0], contextActionMappingImpl, typeof(int));
						break;
					case "SolutionBy":
						ucTrackingIssue1.btn_sBy.EditValue = ContextActionMappingSource.GetValueSource(MappingSource.Rows[0], contextActionMappingImpl, typeof(int));
						break;
					case "WbsCode":
						ucTrackingIssue1.btn_taskCode.EditValue = ContextActionMappingSource.GetValueSource(MappingSource.Rows[0], contextActionMappingImpl, typeof(string));
						break;
					case "Note":
						ucTrackingIssue1.txt_note.EditValue = ContextActionMappingSource.GetValueSource(MappingSource.Rows[0], contextActionMappingImpl, typeof(string));
						break;
					case "ProjectOwner":
						ucTrackingIssue1.btn_prOw.EditValue = ContextActionMappingSource.GetValueSource(MappingSource.Rows[0], contextActionMappingImpl, typeof(int));
						break;
					}
					ContextActionMappingImpl[] source = MappingDocument.Where((ContextActionMappingImpl t) => t.TargetField.StartsWith("U_")).ToArray();
					foreach (Control editor in ucTrackingIssue1.layout_UDF.Controls)
					{
						if (editor is BaseEdit)
						{
							BaseEdit baseEdit = editor as BaseEdit;
							ContextActionMappingImpl contextActionMappingImpl2 = source.FirstOrDefault((ContextActionMappingImpl t) => t.TargetField == editor.Name);
							if (contextActionMappingImpl2 != null)
							{
								baseEdit.EditValue = Function.ToString(ContextActionMappingSource.GetValueSource(MappingSource.Rows[0], contextActionMappingImpl2, typeof(string)));
							}
						}
					}
					FormMode = Enums.FormViewModeType.Add;
				}
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", ex);
				WinCommons.ErrorLineNumber(ex);
			}
			_isMapping = false;
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", ex2);
			WinCommons.ErrorLineNumber(ex2);
		}
	}

	private async Task AfterLoad()
	{
		if (DataRowFromCal != null)
		{
			isNewIssue = true;
			btn_ShowDetail.Text = "Hide Detail";
			FormMode = Enums.FormViewModeType.Add;
			await REFRESH();
			DataTable dt = DataRowFromCal.Table.Clone();
			dt.PrimaryKey = null;
			dt.ImportRow(DataRowFromCal);
			int num = await EmployeeId(GlobalData.UserID);
			dt.Rows[0]["Priority"] = 2;
			dt.Rows[0]["SolutionDesc"] = "";
			dt.Rows[0]["Reporter"] = num;
			dt.AcceptChanges();
			int num2 = Function.ParseInt(dt.Rows[0]["ProjectCode"]);
			btn_ProjectCode.EditValue = ((num2 > 0) ? dt.Rows[0]["ProjectCode"] : DBNull.Value);
			string bpCode = Function.ToString(btn_BpCode.EditValue);
			string projectCode = Function.ToString(btn_ProjectCode.EditValue);
			int projectOwner = Function.ParseInt(btn_Owner.EditValue);
			string reportBy = Function.ToString(btn_ReportBy.EditValue);
			string handleBy = Function.ToString(btn_HandleBy.EditValue);
			string status = Function.ToString(luk_statusFind.EditValue);
			string fromDate = Function.ToString(date_searchFD.EditValue);
			string toDate = Function.ToString(date_searchTD.EditValue);
			DataTable dataSource = await Search(bpCode, projectCode, projectOwner, reportBy, handleBy, id, status, fromDate, toDate);
			id = 0;
			grd_TrackingIssue.DataSource = dataSource;
			ucTrackingIssue1.SetDataTable(dt);
		}
		isNewIssue = false;
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCloseForm, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
		{
			Close();
		}
	}

	private async void btn_Find_Click(object sender, EventArgs e)
	{
		WinCommons._OpenProcessing();
		try
		{
			find = true;
			ucTrackingIssue1._isSearch = true;
			string bpCode = Function.ToString(btn_BpCode.EditValue);
			string projectCode = Function.ToString(btn_ProjectCode.EditValue);
			int projectOwner = Function.ParseInt(btn_Owner.EditValue);
			string reportBy = Function.ToString(btn_ReportBy.EditValue);
			string handleBy = Function.ToString(btn_HandleBy.EditValue);
			string status = Function.ToString(luk_statusFind.EditValue);
			string fromDate = ((date_searchFD.EditValue != null) ? date_searchFD.DateTime.ToString("ddMMyyyy") : "");
			string toDate = ((date_searchTD.EditValue != null) ? date_searchTD.DateTime.ToString("ddMMyyyy") : "");
			if (id == 0)
			{
				id = Function.ParseInt(txt_noSearch.EditValue);
			}
			DataTable dataTable = await Search(bpCode, projectCode, projectOwner, reportBy, handleBy, id, status, fromDate, toDate);
			id = 0;
			if (!ucTrackingIssue1.CallFromCal)
			{
				if ((ucTrackingIssue1.btn_Ok.Text == "Add" || ucTrackingIssue1.btn_Ok.Text == "Update") && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) != DialogResult.Yes)
				{
					return;
				}
				FormMode = Enums.FormViewModeType.View;
			}
			ucTrackingIssue1.CallFromCal = CallFromCal;
			grd_TrackingIssue.DataSource = dataTable;
			grd_TrackingIssue.RefreshDataSource();
			gridColumnEx1.SortOrder = ColumnSortOrder.Descending;
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				ucTrackingIssue1.ResetFormControl(null);
				ucTrackingIssue1.ResetReadOnly(value: false);
				gv_TrackingIssue.FocusedRowHandle = 0;
				int num = Function.ParseInt(gv_TrackingIssue.GetFocusedRowCellValue("Id"));
				await ucTrackingIssue1.GetDetailById(num);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		find = false;
		CallFromCal = false;
		ucTrackingIssue1._isSearch = false;
		WinCommons._CloseProcessing();
	}

	private async void addToolStripMenuItem_Click(object sender, EventArgs e)
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

	private async void removeToolStripMenuItem_Click(object sender, EventArgs e)
	{
		_ = 3;
		try
		{
			if (gv_TrackingIssue.GetSelectedRows() == null || gv_TrackingIssue.GetSelectedRows().Count() == 0)
			{
				WinCommons.Error(Enums.MessageCode.InfoNoRowSelected, "");
				return;
			}
			string Id = Function.ToString(gv_TrackingIssue.GetFocusedRowCellValue("Id"));
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmRemoveByNo, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, Id) != DialogResult.Yes || Id.Length <= 0)
			{
				return;
			}
			string tmp = await _httpClient.TrackingIssue.DeleteDocs(Id);
			await ucTrackingIssue1.ucComment1.RemoveCommentById(Function.ParseInt(Id));
			if (string.IsNullOrEmpty(tmp))
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
				if (SendDataBackToMain != null && !((XtraForm)SendDataBackToMain.Target).IsDisposed)
				{
					await SendDataBackToMain(Function.ParseInt(Id));
				}
				await ucTrackingIssue1.ucComment1.RefreshComment(0);
				ucTrackingIssue1.grd_History.DatasourceTable().Rows.Clear();
				btn_Find_Click(null, null);
				gv_TrackingIssue.FocusedRowHandle = 0;
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, tmp);
				WinCommons.Error(Enums.MessageCode.DeleteError, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_TrackingIssue_BeforeLeaveRow(object sender, RowAllowEventArgs e)
	{
		try
		{
			if (_isLoading || ucTrackingIssue1._isSuccess || (FormMode == Enums.FormViewModeType.View && ucTrackingIssue1.btn_Ok.Text == "OK"))
			{
				return;
			}
			if (CallFromCal || isNewIssue)
			{
				CallFromCal = false;
				return;
			}
			DialogResult dialogResult = XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
			if (dialogResult == DialogResult.Yes && dialogResult == DialogResult.Yes)
			{
				e.Allow = true;
				FormMode = Enums.FormViewModeType.View;
			}
			if (dialogResult == DialogResult.No)
			{
				e.Allow = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_TrackingIssue_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == "Priority")
			{
				e.DisplayText = "";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void gv_TrackingIssue_DoubleClick(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Add)
			{
				return;
			}
			GridView gridView = (GridView)sender;
			Point pt = gridView.GridControl.PointToClient(Control.MousePosition);
			GridHitInfo gridHitInfo = gridView.CalcHitInfo(pt);
			if ((gridHitInfo.InRow || gridHitInfo.InRowCell) && gridHitInfo.Column == null)
			{
				splitContainerControl1.PanelVisibility = SplitPanelVisibility.Both;
				btn_ShowDetail.Text = "Hide Detail";
				if (gv_TrackingIssue.FocusedRowHandle < 0)
				{
					return;
				}
				ucTrackingIssue1.ResetReadOnly(value: false);
				int num = Function.ParseInt(gv_TrackingIssue.GetFocusedRowCellValue("Id"));
				await ucTrackingIssue1.GetDetailById(num);
			}
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void duplicateToolStripMenuItem_Click(object sender, EventArgs e)
	{
		_ = 2;
		try
		{
			_duplicate = true;
			splitContainerControl1.PanelVisibility = SplitPanelVisibility.Both;
			_dtdoc = grd_TrackingIssue.DatasourceTable();
			if (_dtdoc == null || _dtdoc.Columns.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.InfoCannotLoadData, "");
			}
			int num = Function.ParseInt(gv_TrackingIssue.GetFocusedRowCellValue("Id"));
			if (num > 0)
			{
				DataSet ds = await _httpClient.TrackingIssue.GetDataById(num);
				if (ds.IsNotNull())
				{
					ds.Tables[0].Rows[0]["IssueDate"] = DateTime.Now.Date;
					ds.Tables[0].Rows[0]["Status"] = 1;
					DataRow dataRow = ds.Tables[0].Rows[0];
					dataRow["Reporter"] = await EmployeeId(GlobalData.UserID);
					ucTrackingIssue1.SetDataTable(ds.Tables[0]);
					if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
					{
						int absEntry = Function.ParseInt(ds.Tables[0].Rows[0]["Id"]);
						ucTrackingIssue1.ucAttachment1.LoadAttachmentsFile(absEntry);
					}
					await ucTrackingIssue1.ucComment1.RefreshComment(0);
					ucTrackingIssue1.grd_History.DataSourceTable.Rows.Clear();
				}
				FormMode = Enums.FormViewModeType.Add;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		_duplicate = false;
	}

	private async void gv_TrackingIssue_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Add || _isLoading || CallFromCal || _isMapping)
			{
				return;
			}
			if (e.FocusedRowHandle < 0)
			{
				_isLoading = true;
				ucTrackingIssue1._isLoading = true;
				ucTrackingIssue1.txt_Id.EditValue = null;
				ucTrackingIssue1.date_IDate.EditValue = null;
				ucTrackingIssue1.luk_Type.EditValue = null;
				ucTrackingIssue1.luk_priority.EditValue = null;
				ucTrackingIssue1.luk_Status.EditValue = null;
				ucTrackingIssue1.btn_report.EditValue = null;
				ucTrackingIssue1.btn_handle.EditValue = null;
				ucTrackingIssue1.btn_solution.EditValue = null;
				ucTrackingIssue1.date_rDate.EditValue = null;
				ucTrackingIssue1.btn_bp.EditValue = null;
				ucTrackingIssue1.btn_pr.EditValue = null;
				ucTrackingIssue1.btn_sBy.EditValue = null;
				ucTrackingIssue1.btn_taskCode.EditValue = null;
				ucTrackingIssue1.txt_attact.EditValue = null;
				ucTrackingIssue1.txt_note.EditValue = null;
				ucTrackingIssue1.txt_solutionDesc.EditValue = null;
				ucTrackingIssue1.txt_taskName.EditValue = null;
				ucTrackingIssue1.btn_user1.EditValue = null;
				ucTrackingIssue1.memo_Summary.EditValue = null;
				ucTrackingIssue1.btn_prOw.EditValue = null;
				if (ucTrackingIssue1.ucAttachment1.DataSource.IsNotNull())
				{
					ucTrackingIssue1.ucAttachment1.DataSource.Rows.Clear();
					ucTrackingIssue1.ucAttachment1.grd_File.RefreshDataSource();
				}
				if (ucTrackingIssue1.grd_History.DataSourceTable.IsNotNull())
				{
					ucTrackingIssue1.grd_History.DataSourceTable.Rows.Clear();
				}
				if (ucTrackingIssue1.ucComment1.grd_Comment.DatasourceTable().IsNotNull())
				{
					ucTrackingIssue1.ucComment1.grd_Comment.DataSourceTable().Rows.Clear();
				}
				ucTrackingIssue1.ResetReadOnly(value: true);
				FormMode = Enums.FormViewModeType.View;
				ucTrackingIssue1._isLoading = false;
				_isLoading = false;
				return;
			}
			foreach (Control control in ucTrackingIssue1.layout_UDF.Controls)
			{
				if (control is BaseEdit && FormMode == Enums.FormViewModeType.Add)
				{
					(control as BaseEdit).EditValue = control.AccessibleDescription;
				}
			}
			ucTrackingIssue1.ResetReadOnly(value: false);
			ucTrackingIssue1.luk_ChangeStatus.EditValue = null;
			int num = Function.ParseInt(gv_TrackingIssue.GetFocusedRowCellValue("Id"));
			await ucTrackingIssue1.GetDetailById(num);
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void BindData(Control control, DataView dv, string fieldName, DataSourceUpdateMode mode = DataSourceUpdateMode.OnValidation)
	{
		control.DataBindings.Clear();
		if (control is BaseEdit)
		{
			control.DataBindings.Add("EditValue", dv, fieldName, formattingEnabled: false, mode);
		}
		control.Tag = "APZ_OCLG, " + fieldName;
	}

	private void btn_ShowDetail_Click(object sender, EventArgs e)
	{
		try
		{
			if (splitContainerControl1.PanelVisibility == SplitPanelVisibility.Panel1)
			{
				splitContainerControl1.PanelVisibility = SplitPanelVisibility.Both;
				btn_ShowDetail.Text = "Hide Detail";
			}
			else if (splitContainerControl1.PanelVisibility == SplitPanelVisibility.Both)
			{
				splitContainerControl1.PanelVisibility = SplitPanelVisibility.Panel1;
				btn_ShowDetail.Text = "Show Detail";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void frm_TrackingIssue_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			DataSet dataSet = new DataSet();
			DataTable dt = new DataTable();
			GetDataTableSearch(ref dt);
			dataSet.Tables.Add(dt.Copy());
			await _httpClient.TrackingIssue.SaveSearch(dataSet, Function.ToString(GlobalData.UserID));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_TrackingIssue_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
	{
		try
		{
			int num = 16;
			e.Appearance.Options.UseTextOptions = true;
			e.DefaultDraw();
			if (e.Column.FieldName == "Status")
			{
				Color color = ColorTranslator.FromHtml(Function.ToString(gv_TrackingIssue.GetRowCellValue(e.RowHandle, "ColorStatus")));
				Color color2 = Color.FromArgb(color.R, color.G, color.B);
				if (!string.IsNullOrEmpty(Function.ToString(e.CellValue)))
				{
					Brush brush = new SolidBrush(color2);
					e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
					e.Graphics.FillRectangle(brush, new Rectangle(e.Bounds.Location, new Size(num, num)));
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<DataTable> Search(string bpCode, string projectCode, int projectOwner, string reportBy, string handleBy, int id, string status, string fromDate, string toDate)
	{
		_isLoading = true;
		try
		{
			DataTable dt = await _httpClient.TrackingIssue.Search(bpCode.Trim(), projectCode.Trim(), projectOwner, reportBy, handleBy, id, status, fromDate, toDate, GlobalData.UserName);
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				int num = Function.ParseInt(dt.Rows[i]["ProjectCode"]);
				if (ucTrackingIssue1.btn_pr.Properties.dictionSource.ContainsKey(Function.ToString(num)))
				{
					dt.Rows[i]["ProjectCode"] = ((num > 0) ? dt.Rows[i]["ProjectCode"] : DBNull.Value);
				}
				else
				{
					dt.Rows[i]["ProjectCode"] = DBNull.Value;
				}
				int num2 = Function.ParseInt(dt.Rows[i]["Reporter"]);
				if (ucTrackingIssue1.btn_report.Properties.dictionSource.ContainsKey(Function.ToString(num2)))
				{
					dt.Rows[i]["Reporter"] = ((num2 > 0) ? dt.Rows[i]["Reporter"] : DBNull.Value);
				}
				else
				{
					int num3 = await EmployeeId(num2);
					if (num3 > 0)
					{
						dt.Rows[i]["Reporter"] = num3;
					}
					else
					{
						dt.Rows[i]["Reporter"] = DBNull.Value;
					}
				}
				int num4 = Function.ParseInt(dt.Rows[i]["HandleBy"]);
				if (ucTrackingIssue1.btn_handle.Properties.dictionSource.ContainsKey(Function.ToString(num4)))
				{
					dt.Rows[i]["HandleBy"] = ((num4 > 0) ? dt.Rows[i]["HandleBy"] : DBNull.Value);
				}
				else
				{
					dt.Rows[i]["HandleBy"] = DBNull.Value;
				}
			}
			_isLoading = false;
			return dt;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return new DataTable();
		}
	}

	private async Task LoadForm()
	{
		_ = 11;
		try
		{
			_isLoading = true;
			FormMode = Enums.FormViewModeType.View;
			ucTrackingIssue1.LoadControl();
			btn_BpCode.Properties.ValueMember = "CardCode";
			btn_BpCode.Properties.TypeCFL = Enums.TypeCFL.Customer;
			btn_BpCode.Properties.LoadDataSourceWhenEnter = true;
			RepositoryItemButtonEditCFL properties = btn_BpCode.Properties;
			properties.DataSourceLoaded = (Action<DataTable>)Delegate.Combine(properties.DataSourceLoaded, (Action<DataTable>)((DataTable dt) =>
			{
				btn_gridBpCode.DataSource = dt.Copy();
			}));
			btn_BpCode.Properties.RefreshDataSource();
			btn_BpCode.Properties.TypeDrillDown = ApzObjectTypes.oBusinessPartners;
			btn_gridBpCode.ValueMember = "CardCode";
			btn_gridBpCode.TypeCFL = Enums.TypeCFL.Customer;
			btn_gridBpCode.LoadDataSourceWhenEnter = true;
			btn_gridBpCode.RefreshDataSource();
			btn_gridBpCode.TypeDrillDown = ApzObjectTypes.oBusinessPartners;
			btn_HandleBy.Properties.ValueMember = "EmpId";
			btn_HandleBy.Properties.DisplayMember = "lastName";
			btn_HandleBy.Properties.DisplayMember2 = "firstName";
			btn_HandleBy.Properties.TypeCFL = Enums.TypeCFL.EmployeeListForSalesEmpSetup;
			RepositoryItemButtonEditCFL properties2 = btn_HandleBy.Properties;
			properties2.DataSourceLoaded = (Action<DataTable>)Delegate.Combine(properties2.DataSourceLoaded, (Action<DataTable>)((DataTable dt) =>
			{
				btn_gridHandBy.DataSource = dt.Copy();
				btn_ReportBy.Properties.DataSource = dt.Copy();
				btn_gridReportBy.DataSource = dt.Copy();
				btn_Owner.Properties.DataSource = dt.Copy();
			}));
			btn_HandleBy.Properties.MultiSelectMode = true;
			await btn_HandleBy.Properties.RefreshDataSourceAsync();
			btn_HandleBy.Properties.TypeDrillDown = ApzObjectTypes.oEmployeesInfo;
			btn_Owner.Properties.ValueMember = "EmpId";
			btn_Owner.Properties.DisplayMember = "lastName";
			btn_Owner.Properties.DisplayMember2 = "firstName";
			btn_Owner.Properties.TypeCFL = Enums.TypeCFL.EmployeeListForSalesEmpSetup;
			await btn_Owner.Properties.RefreshDataSourceAsync();
			btn_Owner.Properties.TypeDrillDown = ApzObjectTypes.oEmployeesInfo;
			btn_gridHandBy.ValueMember = "EmpId";
			btn_gridHandBy.DisplayMember = "lastName";
			btn_gridHandBy.DisplayMember2 = "firstName";
			btn_gridHandBy.TypeCFL = Enums.TypeCFL.EmployeeListForSalesEmpSetup;
			await btn_gridHandBy.RefreshDataSourceAsync();
			btn_gridHandBy.TypeDrillDown = ApzObjectTypes.oEmployeesInfo;
			btn_ReportBy.Properties.ValueMember = "EmpId";
			btn_ReportBy.Properties.DisplayMember = "lastName";
			btn_ReportBy.Properties.DisplayMember2 = "firstName";
			btn_ReportBy.Properties.TypeCFL = Enums.TypeCFL.EmployeeListForSalesEmpSetup;
			btn_ReportBy.Properties.MultiSelectMode = true;
			await btn_ReportBy.Properties.RefreshDataSourceAsync();
			btn_ReportBy.Properties.TypeDrillDown = ApzObjectTypes.oEmployeesInfo;
			btn_gridReportBy.ValueMember = "EmpId";
			btn_gridReportBy.DisplayMember = "lastName";
			btn_gridReportBy.DisplayMember2 = "firstName";
			btn_gridReportBy.TypeCFL = Enums.TypeCFL.EmployeeListForSalesEmpSetup;
			await btn_gridReportBy.RefreshDataSourceAsync();
			btn_gridReportBy.TypeDrillDown = ApzObjectTypes.oEmployeesInfo;
			btn_ProjectCode.Properties.ValueMember = "AbsEntry";
			btn_ProjectCode.Properties.DisplayMember = "Code";
			btn_ProjectCode.Properties.TypeCFL = Enums.TypeCFL.ProjectManager;
			btn_ProjectCode.Properties.RefreshDataSource();
			btn_ProjectCode.Properties.TypeDrillDown = ApzObjectTypes.oProjectManager;
			btn_gridProject.ValueMember = "AbsEntry";
			btn_gridProject.DisplayMember = "Code";
			btn_gridProject.TypeCFL = Enums.TypeCFL.ProjectManager;
			btn_gridProject.LoadDataSourceWhenEnter = true;
			btn_gridProject.RefreshDataSource();
			btn_gridProject.TypeDrillDown = ApzObjectTypes.oProjectManager;
			luk_typeGrid.ValueMember = "Code";
			luk_typeGrid.DisplayMember = "Name";
			luk_typeGrid.FormProperties.TableName = "APZ_IETE";
			await luk_typeGrid.RefreshDataSourceAsync();
			luk_statusFind.Properties.ValueMember = "Code";
			luk_statusFind.Properties.DisplayMember = "Name";
			luk_statusFind.Properties.FormProperties.TableName = "APZ_OTIS";
			await luk_statusFind.Properties.RefreshDataSourceAsync();
			luk_statusGrid.ValueMember = "Code";
			luk_statusGrid.DisplayMember = "Name";
			luk_statusGrid.FormProperties.TableName = "APZ_OTIS";
			await luk_statusGrid.RefreshDataSourceAsync();
			luk_priorityGrid.SetDataSource(_lstPriority);
			date_searchFD.EditValue = DateTime.Today.AddDays(-Configuration.SearchDate);
			date_searchTD.EditValue = DateTime.Today.AddDays(Configuration.SearchDate);
			new DataTable();
			DataTable dataTable;
			if (await GetDataSearchByUserId(GlobalData.UserID) > 0)
			{
				string bpCode = Function.ToString(btn_BpCode.EditValue);
				string projectCode = Function.ToString(btn_ProjectCode.EditValue);
				int projectOwner = Function.ParseInt(btn_Owner.EditValue);
				string reportBy = Function.ToString(btn_ReportBy.EditValue);
				string handleBy = Function.ToString(btn_HandleBy.EditValue);
				string status = Function.ToString(luk_statusFind.EditValue);
				string fromDate = ((date_searchFD.EditValue != null) ? date_searchFD.DateTime.ToString("ddMMyyyy") : "");
				string toDate = ((date_searchTD.EditValue != null) ? date_searchTD.DateTime.ToString("ddMMyyyy") : "");
				if (id == 0)
				{
					id = Function.ParseInt(txt_noSearch.EditValue);
				}
				dataTable = await Search(bpCode, projectCode, projectOwner, reportBy, handleBy, id, status, fromDate, toDate);
			}
			else
			{
				string fromDate2 = DateTime.Today.AddDays(-Configuration.SearchDate).ToString("ddMMyyyy");
				string toDate2 = DateTime.Today.AddDays(Configuration.SearchDate).ToString("ddMMyyyy");
				dataTable = await Search("", "", 0, "", "", id, "", fromDate2, toDate2);
			}
			id = 0;
			gridColumnEx1.SortOrder = ColumnSortOrder.Descending;
			gridColumnEx2.SortOrder = ColumnSortOrder.Descending;
			grd_TrackingIssue.DataSource = dataTable;
			if (!dataTable.IsNotNull() || CallFromCal)
			{
				ucTrackingIssue1.CallFromCal = CallFromCal;
				await REFRESH();
				gv_TrackingIssue_FocusedRowChanged(null, null);
			}
			gv_TrackingIssue.FocusedRowHandle = 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		_isLoading = false;
	}

	private async Task<int> EmployeeId(int userId)
	{
		try
		{
			DataTable dataTable = await _httpClient.TrackingIssue.GetEmployeeByUserId(userId);
			if (dataTable.IsNotNull())
			{
				return Function.ParseInt(dataTable.Rows[0]["empId"]);
			}
			return 0;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return 0;
		}
	}

	public async Task REFRESH()
	{
		try
		{
			splitContainerControl1.PanelVisibility = SplitPanelVisibility.Both;
			FormMode = Enums.FormViewModeType.Add;
			int projectId = Function.ParseInt(btn_ProjectCode.EditValue);
			await ucTrackingIssue1.RefreshTracking(projectId);
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task REFRESH_DATA()
	{
		btn_Find_Click(null, null);
	}

	private void GetDataTableSearch(ref DataTable dt)
	{
		try
		{
			dt = new DataTable();
			dt.Columns.Add("FormId", typeof(string));
			dt.Columns.Add("ParamName", typeof(string));
			dt.Columns.Add("ParamType", typeof(string));
			dt.Columns.Add("Value", typeof(string));
			dt.Columns.Add("UserCode", typeof(int));
			DataRow dataRow = dt.NewRow();
			dataRow["FormId"] = "oTrackingIssue";
			dataRow["ParamName"] = "Id";
			dataRow["ParamType"] = "int";
			dataRow["Value"] = Function.ParseInt(txt_noSearch.EditValue);
			dataRow["UserCode"] = GlobalData.UserID;
			dt.Rows.Add(dataRow);
			DataRow dataRow2 = dt.NewRow();
			dataRow2["FormId"] = "oTrackingIssue";
			dataRow2["ParamName"] = "Status";
			dataRow2["ParamType"] = "string";
			dataRow2["Value"] = Function.ToString(luk_statusFind.EditValue);
			dataRow2["UserCode"] = GlobalData.UserID;
			dt.Rows.Add(dataRow2);
			DataRow dataRow3 = dt.NewRow();
			dataRow3["FormId"] = "oTrackingIssue";
			dataRow3["ParamName"] = "ReporterBy";
			dataRow3["ParamType"] = "string";
			dataRow3["Value"] = Function.ToString(btn_ReportBy.EditValue);
			dataRow3["UserCode"] = GlobalData.UserID;
			dt.Rows.Add(dataRow3);
			DataRow dataRow4 = dt.NewRow();
			dataRow4["FormId"] = "oTrackingIssue";
			dataRow4["ParamName"] = "HandleBy";
			dataRow4["ParamType"] = "string";
			dataRow4["Value"] = Function.ToString(btn_HandleBy.EditValue);
			dataRow4["UserCode"] = GlobalData.UserID;
			dt.Rows.Add(dataRow4);
			DataRow dataRow5 = dt.NewRow();
			dataRow5["FormId"] = "oTrackingIssue";
			dataRow5["ParamName"] = "ProjectCode";
			dataRow5["ParamType"] = "int";
			dataRow5["Value"] = Function.ParseInt(btn_ProjectCode.EditValue);
			dataRow5["UserCode"] = GlobalData.UserID;
			dt.Rows.Add(dataRow5);
			DataRow dataRow6 = dt.NewRow();
			dataRow6["FormId"] = "oTrackingIssue";
			dataRow6["ParamName"] = "ProjectOwner";
			dataRow6["ParamType"] = "int";
			dataRow6["Value"] = Function.ParseInt(btn_Owner.EditValue);
			dataRow6["UserCode"] = GlobalData.UserID;
			dt.Rows.Add(dataRow6);
			DataRow dataRow7 = dt.NewRow();
			dataRow7["FormId"] = "oTrackingIssue";
			dataRow7["ParamName"] = "BpCode";
			dataRow7["ParamType"] = "string";
			dataRow7["Value"] = Function.ToString(btn_BpCode.EditValue);
			dataRow7["UserCode"] = GlobalData.UserID;
			dt.Rows.Add(dataRow7);
			DataRow dataRow8 = dt.NewRow();
			dataRow8["FormId"] = "oTrackingIssue";
			dataRow8["ParamName"] = "FromDate";
			dataRow8["ParamType"] = "DateTime";
			if (!Function.ParseDateTime(date_searchFD.EditValue).HasValue)
			{
				dataRow8["Value"] = DBNull.Value;
			}
			else
			{
				dataRow8["Value"] = Function.ParseDateTime(date_searchFD.EditValue);
			}
			dataRow8["UserCode"] = GlobalData.UserID;
			dt.Rows.Add(dataRow8);
			DataRow dataRow9 = dt.NewRow();
			dataRow9["FormId"] = "oTrackingIssue";
			dataRow9["ParamName"] = "ToDate";
			dataRow9["ParamType"] = "DateTime";
			if (!Function.ParseDateTime(date_searchTD.EditValue).HasValue)
			{
				dataRow9["Value"] = DBNull.Value;
			}
			else
			{
				dataRow9["Value"] = Function.ParseDateTime(date_searchTD.EditValue);
			}
			dataRow9["UserCode"] = GlobalData.UserID;
			dt.Rows.Add(dataRow9);
			dt.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void SetDataTableSearch(DataTable dt)
	{
		try
		{
			if (dt == null && dt.Rows.Count != 0)
			{
				return;
			}
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				switch (Function.ToString(dt.Rows[i]["ParamName"]))
				{
				case "Id":
					txt_noSearch.EditValue = ((Function.ParseInt(dt.Rows[i]["Value"]) > 0) ? dt.Rows[i]["Value"] : "");
					break;
				case "Status":
					luk_statusFind.EditValue = Function.ToString(dt.Rows[i]["Value"]);
					break;
				case "ReporterBy":
					btn_ReportBy.EditValue = Function.ToString(dt.Rows[i]["Value"]);
					break;
				case "HandleBy":
					btn_HandleBy.EditValue = Function.ToString(dt.Rows[i]["Value"]);
					break;
				case "ProjectCode":
					btn_ProjectCode.EditValue = ((Function.ParseInt(dt.Rows[i]["Value"]) > 0) ? dt.Rows[i]["Value"] : "");
					break;
				case "ProjectOwner":
					btn_Owner.EditValue = ((Function.ParseInt(dt.Rows[i]["Value"]) > 0) ? dt.Rows[i]["Value"] : "");
					break;
				case "BpCode":
					btn_BpCode.EditValue = Function.ToString(dt.Rows[i]["Value"]);
					break;
				case "FromDate":
					date_searchFD.EditValue = Function.ParseDateTime(dt.Rows[i]["Value"]);
					break;
				case "ToDate":
					date_searchTD.EditValue = Function.ParseDateTime(dt.Rows[i]["Value"]);
					break;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<int> GetDataSearchByUserId(int userId)
	{
		try
		{
			string formId = "oTrackingIssue";
			DataTable dataTable = await _httpClient.TrackingIssue.GetDataSearchByUserId(userId, formId);
			if (dataTable == null || dataTable.Rows.Count == 0)
			{
				return 0;
			}
			SetDataTableSearch(dataTable);
			return 1;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return 0;
		}
	}

	private void newTaskToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			int formObjectType = (int)FormObjectType;
			int num = Function.ParseInt(gv_TrackingIssue.GetFocusedRowCellValue("Id"));
			string value = Function.ToString(gv_TrackingIssue.GetFocusedRowCellValue("BpCode"));
			string text = Function.ToString(gv_TrackingIssue.GetFocusedRowCellValue("Note"));
			frm_TaskManagement frm_TaskManagement2 = new frm_TaskManagement
			{
				ClgCode = 0,
				StartPosition = FormStartPosition.CenterScreen,
				MdiParent = MdiParent,
				CallFromCal = true
			};
			DataTable tableStructure = WinFuntions.GetTableStructure("APZ_OCLG");
			DataRow dataRow = tableStructure.NewRow();
			dataRow["GroupUser"] = GlobalData.UserID;
			if (Function.ToString(dataRow["Recontact"]) == "")
			{
				dataRow["Recontact"] = DateTime.Today;
				string text2 = DateTime.Now.Minute.ToString();
				dataRow["BeginTime"] = DateTime.Now.Hour + ((text2.Length == 2) ? text2 : ("0" + text2));
			}
			dataRow["Closed"] = "N";
			dataRow["DocType"] = formObjectType;
			if (num != 0)
			{
				dataRow["DocEntry"] = num;
				dataRow["DocNum"] = num;
			}
			dataRow["CardCode"] = value;
			int num2 = Function.ParseInt(gv_TrackingIssue.GetFocusedRowCellValue("ProjectCode"));
			string text3 = "";
			if (btn_ProjectCode.Properties.dictionSource.TryGetValue(Function.ToString(num2), out var value2))
			{
				text3 = Function.ToString(value2);
			}
			string text4 = Function.ToString(gv_TrackingIssue.GetFocusedRowCellValue("WbsCode"));
			string text5 = text3 + (string.IsNullOrEmpty(text4) ? "" : (" - " + text4));
			if (text.Length < 77)
			{
				text5 = text5 + " " + text;
				dataRow["Details"] = text5;
			}
			else
			{
				dataRow["Details"] = text5;
				dataRow["Notes"] = text;
			}
			if (num2 != 0)
			{
				dataRow["ProjectEntry"] = num2;
			}
			dataRow["ProjectWBS"] = text4;
			tableStructure.Rows.Add(dataRow);
			tableStructure.AcceptChanges();
			frm_TaskManagement2.DataRowFromCal = dataRow;
			frm_TaskManagement2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void relatedActivitiesToolStripMenuItem_Click_1(object sender, EventArgs e)
	{
		try
		{
			int num = Function.ParseInt(gv_TrackingIssue.GetFocusedRowCellValue("Id"));
			if (num > 0)
			{
				string cardCode = Function.ToString(gv_TrackingIssue.GetFocusedRowCellValue("BpCode"));
				string wbsCode = Function.ToString(gv_TrackingIssue.GetFocusedRowCellValue("WbsCode"));
				string projectCode = Function.ToString(gv_TrackingIssue.GetFocusedRowCellValue("ProjectCode"));
				frm_RelatedActivites frm_RelatedActivites2 = new frm_RelatedActivites(ApzObjectTypes.oTrackingIssue, Function.ToString(num));
				frm_RelatedActivites2.MdiParent = MdiParent;
				frm_RelatedActivites2.CardCode = cardCode;
				frm_RelatedActivites2.WbsCode = wbsCode;
				frm_RelatedActivites2.ProjectCode = projectCode;
				frm_RelatedActivites2.StartPosition = FormStartPosition.CenterScreen;
				frm_RelatedActivites2.Show();
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
		DevExpress.XtraGrid.GridFormatRule gridFormatRule = new DevExpress.XtraGrid.GridFormatRule();
		DevExpress.XtraEditors.FormatConditionRuleIconSet formatConditionRuleIconSet = new DevExpress.XtraEditors.FormatConditionRuleIconSet();
		DevExpress.XtraEditors.FormatConditionIconSet formatConditionIconSet = new DevExpress.XtraEditors.FormatConditionIconSet();
		DevExpress.XtraEditors.FormatConditionIconSetIcon formatConditionIconSetIcon = new DevExpress.XtraEditors.FormatConditionIconSetIcon();
		DevExpress.XtraEditors.FormatConditionIconSetIcon formatConditionIconSetIcon2 = new DevExpress.XtraEditors.FormatConditionIconSetIcon();
		DevExpress.XtraEditors.FormatConditionIconSetIcon formatConditionIconSetIcon3 = new DevExpress.XtraEditors.FormatConditionIconSetIcon();
		DevExpress.XtraEditors.FormatConditionIconSetIcon formatConditionIconSetIcon4 = new DevExpress.XtraEditors.FormatConditionIconSetIcon();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.PM.frm_TrackingIssue));
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions4 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions5 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions6 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions7 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions8 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions9 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled9 = new DevExpress.Utils.SerializableAppearanceObject();
		this.gc_priority = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Status = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_statusGrid = new Apzon.Forms.UserControls.RepositoryItemLookUpEditDefineNew();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.duplicateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
		this.newTaskToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.relatedActivitiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.grd_TrackingIssue = new Apzon.Controls.Grid.GridControlEx();
		this.gv_TrackingIssue = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx3 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_typeGrid = new Apzon.Forms.UserControls.RepositoryItemLookUpEditDefineNew();
		this.gridColumnEx6 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_gridReportBy = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx7 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_gridHandBy = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx8 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx9 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx10 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemDateEditEx1 = new Apzon.Controls.Editor.RepositoryItemDateEditEx();
		this.gridColumnEx11 = new Apzon.Controls.Grid.GridColumnEx();
		this.TaskCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx13 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_gridBpCode = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx14 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_gridProject = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx15 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx12 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx24 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx25 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx26 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx27 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_priorityGrid = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.date_gridRes = new Apzon.Controls.Editor.RepositoryItemDateEditEx();
		this.gridViewEx2 = new Apzon.Controls.Grid.GridViewEx();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.ucTrackingIssue1 = new Apzon.Forms.UserControls.ucTrackingIssue();
		this.btn_ShowDetail = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.lbl_ReportBy = new Apzon.Controls.Controls.LabelLine();
		this.labelLine5 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine6 = new Apzon.Controls.Controls.LabelLine();
		this.btn_BpCode = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.btn_ProjectCode = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.btn_Owner = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.btn_HandleBy = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.btn_ReportBy = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.btn_Find = new Apzon.Controls.Controls.SimpleButtonEx();
		this.txt_noSearch = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine24 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine25 = new Apzon.Controls.Controls.LabelLine();
		this.date_searchFD = new Apzon.Controls.Editor.DateEditEx();
		this.labelLine26 = new Apzon.Controls.Controls.LabelLine();
		this.date_searchTD = new Apzon.Controls.Editor.DateEditEx();
		this.folderBrowserDialogEx1 = new Apzon.Forms.Commons.FolderBrowserDialogEx();
		this.luk_statusFind = new Apzon.Forms.UserControls.LookUpEditDefineNew();
		((System.ComponentModel.ISupportInitialize)this.luk_statusGrid).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_TrackingIssue).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_TrackingIssue).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_typeGrid).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_gridReportBy).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_gridHandBy).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx1.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_gridBpCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_gridProject).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_priorityGrid).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_gridRes).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_gridRes.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridViewEx2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.btn_BpCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_ProjectCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_Owner.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_HandleBy.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_ReportBy.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_noSearch.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_searchFD.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_searchFD.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_searchTD.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_searchTD.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_statusFind.Properties).BeginInit();
		base.SuspendLayout();
		this.gc_priority.Caption = " ";
		this.gc_priority.CurrencyDateField = null;
		this.gc_priority.CurrencyField = null;
		this.gc_priority.CurrencyFixed = null;
		this.gc_priority.CurrencyFixedRate = 0.0;
		this.gc_priority.CurrencyRateField = null;
		this.gc_priority.FieldName = "Priority";
		this.gc_priority.MaxWidth = 24;
		this.gc_priority.MinWidth = 45;
		this.gc_priority.Name = "gc_priority";
		this.gc_priority.OptionsColumn.FixedWidth = true;
		this.gc_priority.OptionsColumn.ReadOnly = true;
		this.gc_priority.PrimaryKey = false;
		this.gc_priority.UnitField = null;
		this.gc_priority.Visible = true;
		this.gc_priority.VisibleIndex = 0;
		this.gc_priority.Width = 24;
		this.gc_Status.AppearanceCell.Options.UseTextOptions = true;
		this.gc_Status.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.gc_Status.Caption = "Status";
		this.gc_Status.ColumnEdit = this.luk_statusGrid;
		this.gc_Status.CurrencyDateField = null;
		this.gc_Status.CurrencyField = null;
		this.gc_Status.CurrencyFixed = null;
		this.gc_Status.CurrencyFixedRate = 0.0;
		this.gc_Status.CurrencyRateField = null;
		this.gc_Status.FieldName = "Status";
		this.gc_Status.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gc_Status.Name = "gc_Status";
		this.gc_Status.OptionsColumn.ReadOnly = true;
		this.gc_Status.PrimaryKey = false;
		this.gc_Status.UnitField = null;
		this.gc_Status.Visible = true;
		this.gc_Status.VisibleIndex = 2;
		this.gc_Status.Width = 125;
		this.luk_statusGrid.AutoHeight = false;
		this.luk_statusGrid.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_statusGrid.CheckFields = null;
		this.luk_statusGrid.CheckTables = null;
		this.luk_statusGrid.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.luk_statusGrid.DisplayMember = "Name";
		this.luk_statusGrid.Name = "luk_statusGrid";
		this.luk_statusGrid.NullText = "";
		this.luk_statusGrid.ShowFooter = false;
		this.luk_statusGrid.ShowHeader = false;
		this.luk_statusGrid.ValueMember = "Value";
		this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[6] { this.addToolStripMenuItem, this.removeToolStripMenuItem, this.duplicateToolStripMenuItem, this.toolStripSeparator1, this.newTaskToolStripMenuItem, this.relatedActivitiesToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(165, 120);
		this.addToolStripMenuItem.Name = "addToolStripMenuItem";
		this.addToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
		this.addToolStripMenuItem.Text = "Add";
		this.addToolStripMenuItem.Click += new System.EventHandler(this.addToolStripMenuItem_Click);
		this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
		this.removeToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
		this.removeToolStripMenuItem.Text = "Remove";
		this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
		this.duplicateToolStripMenuItem.Name = "duplicateToolStripMenuItem";
		this.duplicateToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
		this.duplicateToolStripMenuItem.Text = "Duplicate";
		this.duplicateToolStripMenuItem.Click += new System.EventHandler(this.duplicateToolStripMenuItem_Click);
		this.toolStripSeparator1.Name = "toolStripSeparator1";
		this.toolStripSeparator1.Size = new System.Drawing.Size(161, 6);
		this.newTaskToolStripMenuItem.Name = "newTaskToolStripMenuItem";
		this.newTaskToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
		this.newTaskToolStripMenuItem.Text = "New Task";
		this.newTaskToolStripMenuItem.Click += new System.EventHandler(this.newTaskToolStripMenuItem_Click);
		this.relatedActivitiesToolStripMenuItem.Name = "relatedActivitiesToolStripMenuItem";
		this.relatedActivitiesToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
		this.relatedActivitiesToolStripMenuItem.Text = "Related Activities";
		this.relatedActivitiesToolStripMenuItem.Click += new System.EventHandler(this.relatedActivitiesToolStripMenuItem_Click_1);
		this.grd_TrackingIssue.AllowPasteExcel = true;
		this.grd_TrackingIssue.ClipboardData = null;
		this.grd_TrackingIssue.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_TrackingIssue.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_TrackingIssue.Location = new System.Drawing.Point(0, 0);
		this.grd_TrackingIssue.MainView = this.gv_TrackingIssue;
		this.grd_TrackingIssue.Name = "grd_TrackingIssue";
		this.grd_TrackingIssue.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[9] { this.luk_typeGrid, this.luk_priorityGrid, this.luk_statusGrid, this.btn_gridReportBy, this.btn_gridHandBy, this.btn_gridProject, this.date_gridRes, this.btn_gridBpCode, this.repositoryItemDateEditEx1 });
		this.grd_TrackingIssue.Size = new System.Drawing.Size(469, 706);
		this.grd_TrackingIssue.TabIndex = 0;
		this.grd_TrackingIssue.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[2] { this.gv_TrackingIssue, this.gridViewEx2 });
		this.gv_TrackingIssue.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_TrackingIssue.ColumnKey = null;
		this.gv_TrackingIssue.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[21]
		{
			this.gridColumnEx1, this.gridColumnEx2, this.gridColumnEx3, this.gc_Status, this.gridColumnEx6, this.gridColumnEx7, this.gridColumnEx8, this.gridColumnEx9, this.gridColumnEx10, this.gridColumnEx11,
			this.TaskCode, this.gridColumnEx13, this.gridColumnEx14, this.gridColumnEx15, this.gc_priority, this.gridColumnEx12, this.gridColumnEx4, this.gridColumnEx24, this.gridColumnEx25, this.gridColumnEx26,
			this.gridColumnEx27
		});
		this.gv_TrackingIssue.DisableFormSetting = false;
		this.gv_TrackingIssue.DisplayException = false;
		this.gv_TrackingIssue.EditColumn = null;
		this.gv_TrackingIssue.FieldCurrency = null;
		this.gv_TrackingIssue.FieldRate = null;
		gridFormatRule.Column = this.gc_priority;
		gridFormatRule.ColumnApplyTo = this.gc_priority;
		gridFormatRule.Name = "Format0";
		formatConditionIconSet.CategoryName = "Shapes";
		formatConditionIconSetIcon.PredefinedName = "TrafficLights4_3.png";
		formatConditionIconSetIcon.Value = new decimal(new int[4] { 4, 0, 0, 0 });
		formatConditionIconSetIcon.ValueComparison = DevExpress.XtraEditors.FormatConditionComparisonType.GreaterOrEqual;
		formatConditionIconSetIcon2.PredefinedName = "TrafficLights23_2.png";
		formatConditionIconSetIcon2.Value = new decimal(new int[4] { 3, 0, 0, 0 });
		formatConditionIconSetIcon2.ValueComparison = DevExpress.XtraEditors.FormatConditionComparisonType.GreaterOrEqual;
		formatConditionIconSetIcon3.PredefinedName = "TrafficLights4_4.png";
		formatConditionIconSetIcon3.Value = new decimal(new int[4] { 2, 0, 0, 0 });
		formatConditionIconSetIcon3.ValueComparison = DevExpress.XtraEditors.FormatConditionComparisonType.GreaterOrEqual;
		formatConditionIconSetIcon4.PredefinedName = "TrafficLights23_1.png";
		formatConditionIconSetIcon4.Value = new decimal(new int[4] { 1, 0, 0, 0 });
		formatConditionIconSetIcon4.ValueComparison = DevExpress.XtraEditors.FormatConditionComparisonType.GreaterOrEqual;
		formatConditionIconSet.Icons.Add(formatConditionIconSetIcon);
		formatConditionIconSet.Icons.Add(formatConditionIconSetIcon2);
		formatConditionIconSet.Icons.Add(formatConditionIconSetIcon3);
		formatConditionIconSet.Icons.Add(formatConditionIconSetIcon4);
		formatConditionIconSet.Name = "RedToBlack";
		formatConditionIconSet.ValueType = DevExpress.XtraEditors.FormatConditionValueType.Number;
		formatConditionRuleIconSet.IconSet = formatConditionIconSet;
		gridFormatRule.Rule = formatConditionRuleIconSet;
		this.gv_TrackingIssue.FormatRules.Add(gridFormatRule);
		this.gv_TrackingIssue.FormId = "oTrackingIssue";
		this.gv_TrackingIssue.GridControl = this.grd_TrackingIssue;
		this.gv_TrackingIssue.GroupCount = 1;
		this.gv_TrackingIssue.IgnoreFilter = false;
		this.gv_TrackingIssue.IndexEditRow = -1;
		this.gv_TrackingIssue.IndicatorWidth = 35;
		this.gv_TrackingIssue.IsApzTable = false;
		this.gv_TrackingIssue.IsInsertNewRow = false;
		this.gv_TrackingIssue.IsLoadingData = false;
		this.gv_TrackingIssue.IsMappingContextAction = false;
		this.gv_TrackingIssue.ItemId = "APZ_TGIE";
		this.gv_TrackingIssue.LstColumnAllowBatch = null;
		this.gv_TrackingIssue.LstColumnNotAllowBatch = null;
		this.gv_TrackingIssue.LstColumnNotEditable = null;
		this.gv_TrackingIssue.LstColumNotSetting = null;
		this.gv_TrackingIssue.Name = "gv_TrackingIssue";
		this.gv_TrackingIssue.OptionsBehavior.AutoExpandAllGroups = true;
		this.gv_TrackingIssue.OptionsView.BestFitMaxRowCount = 100;
		this.gv_TrackingIssue.OptionsView.ColumnAutoWidth = false;
		this.gv_TrackingIssue.OptionsView.ShowGroupPanel = false;
		this.gv_TrackingIssue.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[1]
		{
			new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.gridColumnEx2, DevExpress.Data.ColumnSortOrder.Descending)
		});
		this.gv_TrackingIssue.UseBatchMod = false;
		this.gv_TrackingIssue.CustomDrawCell += new DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventHandler(this.gv_TrackingIssue_CustomDrawCell);
		this.gv_TrackingIssue.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gv_TrackingIssue_FocusedRowChanged);
		this.gv_TrackingIssue.BeforeLeaveRow += new DevExpress.XtraGrid.Views.Base.RowAllowEventHandler(this.gv_TrackingIssue_BeforeLeaveRow);
		this.gv_TrackingIssue.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.gv_TrackingIssue_CustomColumnDisplayText);
		this.gv_TrackingIssue.DoubleClick += new System.EventHandler(this.gv_TrackingIssue_DoubleClick);
		this.gridColumnEx1.Caption = "No.";
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "Id";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.OptionsColumn.ReadOnly = true;
		this.gridColumnEx1.PrimaryKey = false;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx1.Width = 77;
		this.gridColumnEx2.Caption = "Issue Date";
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.FieldName = "IssueDate";
		this.gridColumnEx2.GroupInterval = DevExpress.XtraGrid.ColumnGroupInterval.DateRange;
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.OptionsColumn.ReadOnly = true;
		this.gridColumnEx2.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.DateSmart;
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.UnitField = null;
		this.gridColumnEx2.Visible = true;
		this.gridColumnEx2.VisibleIndex = 1;
		this.gridColumnEx2.Width = 150;
		this.gridColumnEx3.Caption = "Issue Type";
		this.gridColumnEx3.ColumnEdit = this.luk_typeGrid;
		this.gridColumnEx3.CurrencyDateField = null;
		this.gridColumnEx3.CurrencyField = null;
		this.gridColumnEx3.CurrencyFixed = null;
		this.gridColumnEx3.CurrencyFixedRate = 0.0;
		this.gridColumnEx3.CurrencyRateField = null;
		this.gridColumnEx3.FieldName = "IssueType";
		this.gridColumnEx3.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx3.Name = "gridColumnEx3";
		this.gridColumnEx3.OptionsColumn.ReadOnly = true;
		this.gridColumnEx3.PrimaryKey = false;
		this.gridColumnEx3.UnitField = null;
		this.gridColumnEx3.Visible = true;
		this.gridColumnEx3.VisibleIndex = 1;
		this.gridColumnEx3.Width = 150;
		this.luk_typeGrid.AutoHeight = false;
		this.luk_typeGrid.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
		this.luk_typeGrid.CheckFields = null;
		this.luk_typeGrid.CheckTables = null;
		this.luk_typeGrid.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.luk_typeGrid.Name = "luk_typeGrid";
		this.luk_typeGrid.NullText = "";
		this.luk_typeGrid.ShowFooter = false;
		this.luk_typeGrid.ShowHeader = false;
		this.gridColumnEx6.Caption = "Reporter By";
		this.gridColumnEx6.ColumnEdit = this.btn_gridReportBy;
		this.gridColumnEx6.CurrencyDateField = null;
		this.gridColumnEx6.CurrencyField = null;
		this.gridColumnEx6.CurrencyFixed = null;
		this.gridColumnEx6.CurrencyFixedRate = 0.0;
		this.gridColumnEx6.CurrencyRateField = null;
		this.gridColumnEx6.FieldName = "Reporter";
		this.gridColumnEx6.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx6.Name = "gridColumnEx6";
		this.gridColumnEx6.PrimaryKey = false;
		this.gridColumnEx6.UnitField = null;
		this.gridColumnEx6.Visible = true;
		this.gridColumnEx6.VisibleIndex = 4;
		this.gridColumnEx6.Width = 129;
		this.btn_gridReportBy.AutoHeight = false;
		this.btn_gridReportBy.AutoLoadDataSource = true;
		editorButtonImageOptions.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions1.Image");
		this.btn_gridReportBy.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_gridReportBy.ContainerControl = this;
		this.btn_gridReportBy.DisplayMember = "lastName";
		this.btn_gridReportBy.DisplayMember2 = "firstName";
		this.btn_gridReportBy.LoadDataSourceWhenEnter = false;
		this.btn_gridReportBy.Name = "btn_gridReportBy";
		this.btn_gridReportBy.SeparateCharacterForMulti = ';';
		this.btn_gridReportBy.SeparateString = ", ";
		this.btn_gridReportBy.UsingAutoCompleteBox = false;
		this.btn_gridReportBy.UsingDataSource = true;
		this.btn_gridReportBy.ValidateWhenNoDataSource = false;
		this.btn_gridReportBy.ValueMember = "EmpId";
		this.gridColumnEx7.Caption = "Handle By";
		this.gridColumnEx7.ColumnEdit = this.btn_gridHandBy;
		this.gridColumnEx7.CurrencyDateField = null;
		this.gridColumnEx7.CurrencyField = null;
		this.gridColumnEx7.CurrencyFixed = null;
		this.gridColumnEx7.CurrencyFixedRate = 0.0;
		this.gridColumnEx7.CurrencyRateField = null;
		this.gridColumnEx7.FieldName = "HandleBy";
		this.gridColumnEx7.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx7.Name = "gridColumnEx7";
		this.gridColumnEx7.PrimaryKey = false;
		this.gridColumnEx7.UnitField = null;
		this.gridColumnEx7.Visible = true;
		this.gridColumnEx7.VisibleIndex = 5;
		this.gridColumnEx7.Width = 129;
		this.btn_gridHandBy.AutoHeight = false;
		this.btn_gridHandBy.AutoLoadDataSource = true;
		editorButtonImageOptions2.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions7.Image");
		this.btn_gridHandBy.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_gridHandBy.ContainerControl = this;
		this.btn_gridHandBy.DisplayMember = "lastName";
		this.btn_gridHandBy.DisplayMember2 = "firstName";
		this.btn_gridHandBy.LoadDataSourceWhenEnter = false;
		this.btn_gridHandBy.Name = "btn_gridHandBy";
		this.btn_gridHandBy.SeparateCharacterForMulti = ';';
		this.btn_gridHandBy.SeparateString = ", ";
		this.btn_gridHandBy.UsingAutoCompleteBox = false;
		this.btn_gridHandBy.UsingDataSource = true;
		this.btn_gridHandBy.ValidateWhenNoDataSource = false;
		this.btn_gridHandBy.ValueMember = "EmpId";
		this.gridColumnEx8.Caption = "Solution Id";
		this.gridColumnEx8.CurrencyDateField = null;
		this.gridColumnEx8.CurrencyField = null;
		this.gridColumnEx8.CurrencyFixed = null;
		this.gridColumnEx8.CurrencyFixedRate = 0.0;
		this.gridColumnEx8.CurrencyRateField = null;
		this.gridColumnEx8.FieldName = "SolutionId";
		this.gridColumnEx8.Name = "gridColumnEx8";
		this.gridColumnEx8.PrimaryKey = false;
		this.gridColumnEx8.UnitField = null;
		this.gridColumnEx9.Caption = "Solution Desc";
		this.gridColumnEx9.CurrencyDateField = null;
		this.gridColumnEx9.CurrencyField = null;
		this.gridColumnEx9.CurrencyFixed = null;
		this.gridColumnEx9.CurrencyFixedRate = 0.0;
		this.gridColumnEx9.CurrencyRateField = null;
		this.gridColumnEx9.FieldName = "SolutionDesc";
		this.gridColumnEx9.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx9.Name = "gridColumnEx9";
		this.gridColumnEx9.PrimaryKey = false;
		this.gridColumnEx9.UnitField = null;
		this.gridColumnEx9.Visible = true;
		this.gridColumnEx9.VisibleIndex = 8;
		this.gridColumnEx9.Width = 145;
		this.gridColumnEx10.Caption = "Response Date";
		this.gridColumnEx10.ColumnEdit = this.repositoryItemDateEditEx1;
		this.gridColumnEx10.CurrencyDateField = null;
		this.gridColumnEx10.CurrencyField = null;
		this.gridColumnEx10.CurrencyFixed = null;
		this.gridColumnEx10.CurrencyFixedRate = 0.0;
		this.gridColumnEx10.CurrencyRateField = null;
		this.gridColumnEx10.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gridColumnEx10.FieldName = "ResponseDate";
		this.gridColumnEx10.Name = "gridColumnEx10";
		this.gridColumnEx10.OptionsColumn.ReadOnly = true;
		this.gridColumnEx10.PrimaryKey = false;
		this.gridColumnEx10.UnitField = null;
		this.gridColumnEx10.Visible = true;
		this.gridColumnEx10.VisibleIndex = 3;
		this.gridColumnEx10.Width = 129;
		this.repositoryItemDateEditEx1.AutoHeight = false;
		this.repositoryItemDateEditEx1.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEditEx1.DisplayFormat.FormatString = "";
		this.repositoryItemDateEditEx1.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.repositoryItemDateEditEx1.EditFormat.FormatString = "";
		this.repositoryItemDateEditEx1.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.repositoryItemDateEditEx1.IsSearchControl = false;
		this.repositoryItemDateEditEx1.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.repositoryItemDateEditEx1.Name = "repositoryItemDateEditEx1";
		this.gridColumnEx11.Caption = "SolutionBy";
		this.gridColumnEx11.CurrencyDateField = null;
		this.gridColumnEx11.CurrencyField = null;
		this.gridColumnEx11.CurrencyFixed = null;
		this.gridColumnEx11.CurrencyFixedRate = 0.0;
		this.gridColumnEx11.CurrencyRateField = null;
		this.gridColumnEx11.FieldName = "SolutionBy";
		this.gridColumnEx11.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx11.Name = "gridColumnEx11";
		this.gridColumnEx11.PrimaryKey = false;
		this.gridColumnEx11.UnitField = null;
		this.TaskCode.Caption = "TaskCode";
		this.TaskCode.CurrencyDateField = null;
		this.TaskCode.CurrencyField = null;
		this.TaskCode.CurrencyFixed = null;
		this.TaskCode.CurrencyFixedRate = 0.0;
		this.TaskCode.CurrencyRateField = null;
		this.TaskCode.FieldName = "TaskCode";
		this.TaskCode.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.TaskCode.Name = "TaskCode";
		this.TaskCode.PrimaryKey = false;
		this.TaskCode.UnitField = null;
		this.gridColumnEx13.Caption = "Bp Code";
		this.gridColumnEx13.ColumnEdit = this.btn_gridBpCode;
		this.gridColumnEx13.CurrencyDateField = null;
		this.gridColumnEx13.CurrencyField = null;
		this.gridColumnEx13.CurrencyFixed = null;
		this.gridColumnEx13.CurrencyFixedRate = 0.0;
		this.gridColumnEx13.CurrencyRateField = null;
		this.gridColumnEx13.FieldName = "BpCode";
		this.gridColumnEx13.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx13.Name = "gridColumnEx13";
		this.gridColumnEx13.PrimaryKey = false;
		this.gridColumnEx13.UnitField = null;
		this.gridColumnEx13.Visible = true;
		this.gridColumnEx13.VisibleIndex = 6;
		this.gridColumnEx13.Width = 161;
		this.btn_gridBpCode.AutoHeight = false;
		this.btn_gridBpCode.AutoLoadDataSource = true;
		editorButtonImageOptions3.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions8.Image");
		this.btn_gridBpCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_gridBpCode.ContainerControl = this;
		this.btn_gridBpCode.DisplayMember = "";
		this.btn_gridBpCode.DisplayMember2 = "";
		this.btn_gridBpCode.LoadDataSourceWhenEnter = false;
		this.btn_gridBpCode.Name = "btn_gridBpCode";
		this.btn_gridBpCode.SeparateCharacterForMulti = ';';
		this.btn_gridBpCode.SeparateString = ", ";
		this.btn_gridBpCode.UsingAutoCompleteBox = false;
		this.btn_gridBpCode.UsingDataSource = true;
		this.btn_gridBpCode.ValidateWhenNoDataSource = false;
		this.btn_gridBpCode.ValueMember = "";
		this.gridColumnEx14.Caption = "Project Code";
		this.gridColumnEx14.ColumnEdit = this.btn_gridProject;
		this.gridColumnEx14.CurrencyDateField = null;
		this.gridColumnEx14.CurrencyField = null;
		this.gridColumnEx14.CurrencyFixed = null;
		this.gridColumnEx14.CurrencyFixedRate = 0.0;
		this.gridColumnEx14.CurrencyRateField = null;
		this.gridColumnEx14.FieldName = "ProjectCode";
		this.gridColumnEx14.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx14.Name = "gridColumnEx14";
		this.gridColumnEx14.PrimaryKey = false;
		this.gridColumnEx14.UnitField = null;
		this.gridColumnEx14.Visible = true;
		this.gridColumnEx14.VisibleIndex = 7;
		this.gridColumnEx14.Width = 141;
		this.btn_gridProject.AutoHeight = false;
		this.btn_gridProject.AutoLoadDataSource = true;
		editorButtonImageOptions4.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions9.Image");
		this.btn_gridProject.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance4, appearanceHovered4, appearancePressed4, appearanceDisabled4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_gridProject.ContainerControl = this;
		this.btn_gridProject.DisplayMember = "Code";
		this.btn_gridProject.DisplayMember2 = "";
		this.btn_gridProject.LoadDataSourceWhenEnter = false;
		this.btn_gridProject.Name = "btn_gridProject";
		this.btn_gridProject.SeparateCharacterForMulti = ';';
		this.btn_gridProject.SeparateString = ", ";
		this.btn_gridProject.UsingAutoCompleteBox = false;
		this.btn_gridProject.UsingDataSource = true;
		this.btn_gridProject.ValidateWhenNoDataSource = false;
		this.btn_gridProject.ValueMember = "AbsEntry";
		this.gridColumnEx15.Caption = "Note";
		this.gridColumnEx15.CurrencyDateField = null;
		this.gridColumnEx15.CurrencyField = null;
		this.gridColumnEx15.CurrencyFixed = null;
		this.gridColumnEx15.CurrencyFixedRate = 0.0;
		this.gridColumnEx15.CurrencyRateField = null;
		this.gridColumnEx15.FieldName = "Note";
		this.gridColumnEx15.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx15.Name = "gridColumnEx15";
		this.gridColumnEx15.PrimaryKey = false;
		this.gridColumnEx15.UnitField = null;
		this.gridColumnEx15.Visible = true;
		this.gridColumnEx15.VisibleIndex = 9;
		this.gridColumnEx15.Width = 270;
		this.gridColumnEx12.Caption = "Subject";
		this.gridColumnEx12.CurrencyDateField = null;
		this.gridColumnEx12.CurrencyField = null;
		this.gridColumnEx12.CurrencyFixed = null;
		this.gridColumnEx12.CurrencyFixedRate = 0.0;
		this.gridColumnEx12.CurrencyRateField = null;
		this.gridColumnEx12.FieldName = "Subject";
		this.gridColumnEx12.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx12.Name = "gridColumnEx12";
		this.gridColumnEx12.PrimaryKey = false;
		this.gridColumnEx12.UnitField = null;
		this.gridColumnEx4.Caption = "TaskName";
		this.gridColumnEx4.CurrencyDateField = null;
		this.gridColumnEx4.CurrencyField = null;
		this.gridColumnEx4.CurrencyFixed = null;
		this.gridColumnEx4.CurrencyFixedRate = 0.0;
		this.gridColumnEx4.CurrencyRateField = null;
		this.gridColumnEx4.FieldName = "TaskName";
		this.gridColumnEx4.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx4.Name = "gridColumnEx4";
		this.gridColumnEx4.PrimaryKey = false;
		this.gridColumnEx4.UnitField = null;
		this.gridColumnEx24.Caption = "AttachedFiled";
		this.gridColumnEx24.CurrencyDateField = null;
		this.gridColumnEx24.CurrencyField = null;
		this.gridColumnEx24.CurrencyFixed = null;
		this.gridColumnEx24.CurrencyFixedRate = 0.0;
		this.gridColumnEx24.CurrencyRateField = null;
		this.gridColumnEx24.FieldName = "AttachedFiled";
		this.gridColumnEx24.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx24.Name = "gridColumnEx24";
		this.gridColumnEx24.PrimaryKey = false;
		this.gridColumnEx24.UnitField = null;
		this.gridColumnEx25.Caption = "ProjectOwner";
		this.gridColumnEx25.CurrencyDateField = null;
		this.gridColumnEx25.CurrencyField = null;
		this.gridColumnEx25.CurrencyFixed = null;
		this.gridColumnEx25.CurrencyFixedRate = 0.0;
		this.gridColumnEx25.CurrencyRateField = null;
		this.gridColumnEx25.FieldName = "ProjectOwner";
		this.gridColumnEx25.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx25.Name = "gridColumnEx25";
		this.gridColumnEx25.PrimaryKey = false;
		this.gridColumnEx25.UnitField = null;
		this.gridColumnEx26.Caption = "UserSign";
		this.gridColumnEx26.CurrencyDateField = null;
		this.gridColumnEx26.CurrencyField = null;
		this.gridColumnEx26.CurrencyFixed = null;
		this.gridColumnEx26.CurrencyFixedRate = 0.0;
		this.gridColumnEx26.CurrencyRateField = null;
		this.gridColumnEx26.FieldName = "UserSign";
		this.gridColumnEx26.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx26.Name = "gridColumnEx26";
		this.gridColumnEx26.PrimaryKey = false;
		this.gridColumnEx26.UnitField = null;
		this.gridColumnEx27.Caption = "UserSign2";
		this.gridColumnEx27.CurrencyDateField = null;
		this.gridColumnEx27.CurrencyField = null;
		this.gridColumnEx27.CurrencyFixed = null;
		this.gridColumnEx27.CurrencyFixedRate = 0.0;
		this.gridColumnEx27.CurrencyRateField = null;
		this.gridColumnEx27.FieldName = "UserSign2";
		this.gridColumnEx27.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx27.Name = "gridColumnEx27";
		this.gridColumnEx27.PrimaryKey = false;
		this.gridColumnEx27.UnitField = null;
		this.luk_priorityGrid.AutoHeight = false;
		this.luk_priorityGrid.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_priorityGrid.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Value", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.luk_priorityGrid.DisplayMember = "Name";
		this.luk_priorityGrid.IsSearchControl = false;
		this.luk_priorityGrid.Name = "luk_priorityGrid";
		this.luk_priorityGrid.NullText = "";
		this.luk_priorityGrid.ShowFooter = false;
		this.luk_priorityGrid.ShowHeader = false;
		this.luk_priorityGrid.ValueMember = "Value";
		this.date_gridRes.AutoHeight = false;
		this.date_gridRes.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_gridRes.DisplayFormat.FormatString = "";
		this.date_gridRes.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_gridRes.EditFormat.FormatString = "";
		this.date_gridRes.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_gridRes.IsSearchControl = false;
		this.date_gridRes.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.date_gridRes.Name = "date_gridRes";
		this.gridViewEx2.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gridViewEx2.ColumnKey = null;
		this.gridViewEx2.DisableFormSetting = false;
		this.gridViewEx2.DisplayException = false;
		this.gridViewEx2.EditColumn = null;
		this.gridViewEx2.FieldCurrency = null;
		this.gridViewEx2.FieldRate = null;
		this.gridViewEx2.FormId = "oTrackingIssue";
		this.gridViewEx2.GridControl = this.grd_TrackingIssue;
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
		this.gridViewEx2.OptionsView.BestFitMaxRowCount = 100;
		this.gridViewEx2.OptionsView.ColumnAutoWidth = false;
		this.gridViewEx2.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridViewEx2.OptionsView.ShowGroupPanel = false;
		this.gridViewEx2.UseBatchMod = false;
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Location = new System.Drawing.Point(1, 76);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.grd_TrackingIssue);
		this.splitContainerControl1.Panel2.Controls.Add(this.ucTrackingIssue1);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(1118, 706);
		this.splitContainerControl1.SplitterPosition = 469;
		this.splitContainerControl1.TabIndex = 1;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.ucTrackingIssue1.AttachmentItemId = "ATC1";
		this.ucTrackingIssue1.AutoScroll = true;
		this.ucTrackingIssue1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		this.ucTrackingIssue1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.ucTrackingIssue1.FormId = "oTrackingIssue";
		this.ucTrackingIssue1.FormView = Apzon.Commons.Enums.Enums.FormViewModeType.View;
		this.ucTrackingIssue1.GrdTrackingIssue = this.grd_TrackingIssue;
		this.ucTrackingIssue1.GvTrackingIssue = this.gv_TrackingIssue;
		this.ucTrackingIssue1.HistoryItemId = "APZ_TGIE1";
		this.ucTrackingIssue1.Location = new System.Drawing.Point(0, 0);
		this.ucTrackingIssue1.Name = "ucTrackingIssue1";
		this.ucTrackingIssue1.Size = new System.Drawing.Size(639, 706);
		this.ucTrackingIssue1.TabIndex = 0;
		this.btn_ShowDetail.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_ShowDetail.EditControl = null;
		this.btn_ShowDetail.EditValue = "Show Detail";
		this.btn_ShowDetail.FieldName = null;
		this.btn_ShowDetail.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_ShowDetail.ImageOptions.Image");
		this.btn_ShowDetail.IsSearchControl = false;
		this.btn_ShowDetail.Location = new System.Drawing.Point(1, 788);
		this.btn_ShowDetail.Name = "btn_ShowDetail";
		this.btn_ShowDetail.ReadOnly = false;
		this.btn_ShowDetail.SettingInVisible = false;
		this.btn_ShowDetail.SettingReadonly = false;
		this.btn_ShowDetail.Size = new System.Drawing.Size(101, 23);
		this.btn_ShowDetail.TabIndex = 89;
		this.btn_ShowDetail.Text = "Show Detail";
		this.btn_ShowDetail.Click += new System.EventHandler(this.btn_ShowDetail_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(108, 788);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(69, 23);
		this.btn_Cancel.TabIndex = 90;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "BP Code";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(392, 37);
		this.labelLine1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(80, 19);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 2;
		this.labelLine1.Utext = "BP Code";
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "Project Code";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(1, 40);
		this.labelLine2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(80, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 3;
		this.labelLine2.Utext = "Project Code";
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "Project Owner";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(193, 38);
		this.labelLine3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(80, 19);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 4;
		this.labelLine3.Utext = "Project Owner";
		this.lbl_ReportBy.EditControl = null;
		this.lbl_ReportBy.EditValue = "Report By";
		this.lbl_ReportBy.FieldName = null;
		this.lbl_ReportBy.HideWhenEmpty = false;
		this.lbl_ReportBy.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_ReportBy.Location = new System.Drawing.Point(392, 3);
		this.lbl_ReportBy.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lbl_ReportBy.Name = "lbl_ReportBy";
		this.lbl_ReportBy.ReadOnly = false;
		this.lbl_ReportBy.SettingInVisible = false;
		this.lbl_ReportBy.SettingReadonly = false;
		this.lbl_ReportBy.ShowUnderLine = true;
		this.lbl_ReportBy.Size = new System.Drawing.Size(80, 19);
		this.lbl_ReportBy.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_ReportBy.TabIndex = 5;
		this.lbl_ReportBy.Utext = "Report By";
		this.labelLine5.EditControl = null;
		this.labelLine5.EditValue = "Handle By";
		this.labelLine5.FieldName = null;
		this.labelLine5.HideWhenEmpty = false;
		this.labelLine5.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine5.Location = new System.Drawing.Point(585, 2);
		this.labelLine5.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine5.Name = "labelLine5";
		this.labelLine5.ReadOnly = false;
		this.labelLine5.SettingInVisible = false;
		this.labelLine5.SettingReadonly = false;
		this.labelLine5.ShowUnderLine = true;
		this.labelLine5.Size = new System.Drawing.Size(80, 20);
		this.labelLine5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine5.TabIndex = 6;
		this.labelLine5.Utext = "Handle By";
		this.labelLine6.EditControl = null;
		this.labelLine6.EditValue = "No.";
		this.labelLine6.FieldName = null;
		this.labelLine6.HideWhenEmpty = false;
		this.labelLine6.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine6.Location = new System.Drawing.Point(1, 2);
		this.labelLine6.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine6.Name = "labelLine6";
		this.labelLine6.ReadOnly = false;
		this.labelLine6.SettingInVisible = false;
		this.labelLine6.SettingReadonly = false;
		this.labelLine6.ShowUnderLine = true;
		this.labelLine6.Size = new System.Drawing.Size(80, 20);
		this.labelLine6.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine6.TabIndex = 7;
		this.labelLine6.Utext = "No.";
		this.btn_BpCode.EditControl = null;
		this.btn_BpCode.FieldName = null;
		this.btn_BpCode.IsSearchControl = true;
		this.btn_BpCode.Location = new System.Drawing.Point(465, 36);
		this.btn_BpCode.Name = "btn_BpCode";
		this.btn_BpCode.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions5.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions6.Image");
		this.btn_BpCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions5, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance5, appearanceHovered5, appearancePressed5, appearanceDisabled5, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_BpCode.Properties.ContainerControl = null;
		this.btn_BpCode.Properties.DisplayMember = "CardCode";
		this.btn_BpCode.Properties.DisplayMember2 = "";
		this.btn_BpCode.Properties.LoadDataSourceWhenEnter = false;
		this.btn_BpCode.Properties.SeparateCharacterForMulti = ';';
		this.btn_BpCode.Properties.SeparateString = ", ";
		this.btn_BpCode.Properties.UsingAutoCompleteBox = false;
		this.btn_BpCode.Properties.UsingDataSource = true;
		this.btn_BpCode.Properties.ValidateWhenNoDataSource = false;
		this.btn_BpCode.Properties.ValueMember = "CardCode";
		this.btn_BpCode.SettingInVisible = false;
		this.btn_BpCode.SettingReadonly = false;
		this.btn_BpCode.Size = new System.Drawing.Size(100, 20);
		this.btn_BpCode.TabIndex = 8;
		this.btn_ProjectCode.EditControl = null;
		this.btn_ProjectCode.FieldName = null;
		this.btn_ProjectCode.IsSearchControl = true;
		this.btn_ProjectCode.Location = new System.Drawing.Point(79, 38);
		this.btn_ProjectCode.Name = "btn_ProjectCode";
		this.btn_ProjectCode.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions6.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions5.Image");
		this.btn_ProjectCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions6, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance6, appearanceHovered6, appearancePressed6, appearanceDisabled6, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_ProjectCode.Properties.ContainerControl = null;
		this.btn_ProjectCode.Properties.DisplayMember = "Code";
		this.btn_ProjectCode.Properties.DisplayMember2 = "";
		this.btn_ProjectCode.Properties.LoadDataSourceWhenEnter = false;
		this.btn_ProjectCode.Properties.SeparateCharacterForMulti = ';';
		this.btn_ProjectCode.Properties.SeparateString = ", ";
		this.btn_ProjectCode.Properties.UsingAutoCompleteBox = false;
		this.btn_ProjectCode.Properties.UsingDataSource = true;
		this.btn_ProjectCode.Properties.ValidateWhenNoDataSource = false;
		this.btn_ProjectCode.Properties.ValueMember = "AbsEntry";
		this.btn_ProjectCode.SettingInVisible = false;
		this.btn_ProjectCode.SettingReadonly = false;
		this.btn_ProjectCode.Size = new System.Drawing.Size(100, 20);
		this.btn_ProjectCode.TabIndex = 9;
		this.btn_Owner.EditControl = null;
		this.btn_Owner.FieldName = null;
		this.btn_Owner.IsSearchControl = true;
		this.btn_Owner.Location = new System.Drawing.Point(273, 37);
		this.btn_Owner.Name = "btn_Owner";
		this.btn_Owner.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions7.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions4.Image");
		this.btn_Owner.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions7, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance7, appearanceHovered7, appearancePressed7, appearanceDisabled7, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_Owner.Properties.ContainerControl = null;
		this.btn_Owner.Properties.DisplayMember = "lastName";
		this.btn_Owner.Properties.DisplayMember2 = "firstName";
		this.btn_Owner.Properties.LoadDataSourceWhenEnter = false;
		this.btn_Owner.Properties.SeparateCharacterForMulti = ';';
		this.btn_Owner.Properties.SeparateString = ", ";
		this.btn_Owner.Properties.UsingAutoCompleteBox = false;
		this.btn_Owner.Properties.UsingDataSource = true;
		this.btn_Owner.Properties.ValidateWhenNoDataSource = false;
		this.btn_Owner.Properties.ValueMember = "EmpId";
		this.btn_Owner.SettingInVisible = false;
		this.btn_Owner.SettingReadonly = false;
		this.btn_Owner.Size = new System.Drawing.Size(100, 20);
		this.btn_Owner.TabIndex = 10;
		this.btn_HandleBy.EditControl = null;
		this.btn_HandleBy.FieldName = null;
		this.btn_HandleBy.IsSearchControl = true;
		this.btn_HandleBy.Location = new System.Drawing.Point(664, 2);
		this.btn_HandleBy.Name = "btn_HandleBy";
		this.btn_HandleBy.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions8.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions3.Image");
		this.btn_HandleBy.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions8, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance8, appearanceHovered8, appearancePressed8, appearanceDisabled8, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_HandleBy.Properties.ContainerControl = null;
		this.btn_HandleBy.Properties.DisplayMember = "lastName";
		this.btn_HandleBy.Properties.DisplayMember2 = "firstName";
		this.btn_HandleBy.Properties.LoadDataSourceWhenEnter = false;
		this.btn_HandleBy.Properties.SeparateCharacterForMulti = ';';
		this.btn_HandleBy.Properties.SeparateString = ", ";
		this.btn_HandleBy.Properties.UsingAutoCompleteBox = false;
		this.btn_HandleBy.Properties.UsingDataSource = true;
		this.btn_HandleBy.Properties.ValidateWhenNoDataSource = false;
		this.btn_HandleBy.Properties.ValueMember = "EmpId";
		this.btn_HandleBy.SettingInVisible = false;
		this.btn_HandleBy.SettingReadonly = false;
		this.btn_HandleBy.Size = new System.Drawing.Size(100, 20);
		this.btn_HandleBy.TabIndex = 11;
		this.btn_ReportBy.EditControl = null;
		this.btn_ReportBy.FieldName = null;
		this.btn_ReportBy.IsSearchControl = true;
		this.btn_ReportBy.Location = new System.Drawing.Point(465, 2);
		this.btn_ReportBy.Name = "btn_ReportBy";
		this.btn_ReportBy.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions9.Image = Apzon.Forms.Properties.Resources.right;
		this.btn_ReportBy.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions9, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance9, appearanceHovered9, appearancePressed9, appearanceDisabled9, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_ReportBy.Properties.ContainerControl = null;
		this.btn_ReportBy.Properties.DisplayMember = "lastName";
		this.btn_ReportBy.Properties.DisplayMember2 = "firstName";
		this.btn_ReportBy.Properties.LoadDataSourceWhenEnter = false;
		this.btn_ReportBy.Properties.SeparateCharacterForMulti = ';';
		this.btn_ReportBy.Properties.SeparateString = ", ";
		this.btn_ReportBy.Properties.UsingAutoCompleteBox = false;
		this.btn_ReportBy.Properties.UsingDataSource = true;
		this.btn_ReportBy.Properties.ValidateWhenNoDataSource = false;
		this.btn_ReportBy.Properties.ValueMember = "EmpId";
		this.btn_ReportBy.SettingInVisible = false;
		this.btn_ReportBy.SettingReadonly = false;
		this.btn_ReportBy.Size = new System.Drawing.Size(100, 20);
		this.btn_ReportBy.TabIndex = 12;
		this.btn_Find.EditControl = null;
		this.btn_Find.EditValue = "Search";
		this.btn_Find.FieldName = null;
		this.btn_Find.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Find.ImageOptions.Image");
		this.btn_Find.IsSearchControl = true;
		this.btn_Find.Location = new System.Drawing.Point(952, 28);
		this.btn_Find.Name = "btn_Find";
		this.btn_Find.ReadOnly = false;
		this.btn_Find.SettingInVisible = false;
		this.btn_Find.SettingReadonly = false;
		this.btn_Find.Size = new System.Drawing.Size(75, 23);
		this.btn_Find.TabIndex = 14;
		this.btn_Find.Text = "Search";
		this.btn_Find.Click += new System.EventHandler(this.btn_Find_Click);
		this.txt_noSearch.EditControl = null;
		this.txt_noSearch.FieldName = null;
		this.txt_noSearch.IsSearchControl = true;
		this.txt_noSearch.Location = new System.Drawing.Point(77, 2);
		this.txt_noSearch.Name = "txt_noSearch";
		this.txt_noSearch.SettingInVisible = false;
		this.txt_noSearch.SettingReadonly = false;
		this.txt_noSearch.Size = new System.Drawing.Size(100, 20);
		this.txt_noSearch.TabIndex = 15;
		this.labelLine24.EditControl = null;
		this.labelLine24.EditValue = "Hide Status";
		this.labelLine24.FieldName = null;
		this.labelLine24.HideWhenEmpty = false;
		this.labelLine24.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine24.Location = new System.Drawing.Point(193, 2);
		this.labelLine24.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine24.Name = "labelLine24";
		this.labelLine24.ReadOnly = false;
		this.labelLine24.SettingInVisible = false;
		this.labelLine24.SettingReadonly = false;
		this.labelLine24.ShowUnderLine = true;
		this.labelLine24.Size = new System.Drawing.Size(80, 20);
		this.labelLine24.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine24.TabIndex = 36;
		this.labelLine24.Utext = "Hide Status";
		this.labelLine25.EditControl = null;
		this.labelLine25.EditValue = "From Date";
		this.labelLine25.FieldName = null;
		this.labelLine25.HideWhenEmpty = false;
		this.labelLine25.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine25.Location = new System.Drawing.Point(585, 36);
		this.labelLine25.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine25.Name = "labelLine25";
		this.labelLine25.ReadOnly = false;
		this.labelLine25.SettingInVisible = false;
		this.labelLine25.SettingReadonly = false;
		this.labelLine25.ShowUnderLine = true;
		this.labelLine25.Size = new System.Drawing.Size(80, 20);
		this.labelLine25.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine25.TabIndex = 37;
		this.labelLine25.Utext = "From Date";
		this.date_searchFD.EditControl = null;
		this.date_searchFD.EditValue = null;
		this.date_searchFD.FieldName = null;
		this.date_searchFD.Location = new System.Drawing.Point(663, 34);
		this.date_searchFD.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.date_searchFD.Name = "date_searchFD";
		this.date_searchFD.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_searchFD.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_searchFD.Properties.IsSearchControl = true;
		this.date_searchFD.SettingInVisible = false;
		this.date_searchFD.SettingReadonly = false;
		this.date_searchFD.Size = new System.Drawing.Size(100, 20);
		this.date_searchFD.TabIndex = 38;
		this.labelLine26.EditControl = null;
		this.labelLine26.EditValue = "To Date";
		this.labelLine26.FieldName = null;
		this.labelLine26.HideWhenEmpty = false;
		this.labelLine26.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine26.Location = new System.Drawing.Point(783, 32);
		this.labelLine26.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine26.Name = "labelLine26";
		this.labelLine26.ReadOnly = false;
		this.labelLine26.SettingInVisible = false;
		this.labelLine26.SettingReadonly = false;
		this.labelLine26.ShowUnderLine = true;
		this.labelLine26.Size = new System.Drawing.Size(66, 20);
		this.labelLine26.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine26.TabIndex = 39;
		this.labelLine26.Utext = "To Date";
		this.date_searchTD.EditControl = null;
		this.date_searchTD.EditValue = null;
		this.date_searchTD.FieldName = null;
		this.date_searchTD.Location = new System.Drawing.Point(847, 30);
		this.date_searchTD.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.date_searchTD.Name = "date_searchTD";
		this.date_searchTD.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_searchTD.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_searchTD.Properties.IsSearchControl = true;
		this.date_searchTD.SettingInVisible = false;
		this.date_searchTD.SettingReadonly = false;
		this.date_searchTD.Size = new System.Drawing.Size(100, 20);
		this.date_searchTD.TabIndex = 40;
		this.folderBrowserDialogEx1.Description = "";
		this.folderBrowserDialogEx1.DontIncludeNetworkFoldersBelowDomainLevel = false;
		this.folderBrowserDialogEx1.IsSearchControl = false;
		this.folderBrowserDialogEx1.NewStyle = true;
		this.folderBrowserDialogEx1.RootFolder = System.Environment.SpecialFolder.Desktop;
		this.folderBrowserDialogEx1.SelectedPath = "";
		this.folderBrowserDialogEx1.ShowBothFilesAndFolders = false;
		this.folderBrowserDialogEx1.ShowEditBox = true;
		this.folderBrowserDialogEx1.ShowFullPathInEditBox = true;
		this.folderBrowserDialogEx1.ShowNewFolderButton = true;
		this.luk_statusFind.EditControl = null;
		this.luk_statusFind.FieldName = null;
		this.luk_statusFind.IsSearchControl = false;
		this.luk_statusFind.Location = new System.Drawing.Point(273, 2);
		this.luk_statusFind.Name = "luk_statusFind";
		this.luk_statusFind.Properties.AutoHeight = false;
		this.luk_statusFind.Properties.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_statusFind.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_statusFind.Properties.CheckFields = null;
		this.luk_statusFind.Properties.CheckTables = null;
		this.luk_statusFind.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.luk_statusFind.Properties.NullText = "";
		this.luk_statusFind.Properties.ShowFooter = false;
		this.luk_statusFind.Properties.ShowHeader = false;
		this.luk_statusFind.SettingInVisible = false;
		this.luk_statusFind.SettingReadonly = false;
		this.luk_statusFind.Size = new System.Drawing.Size(100, 20);
		this.luk_statusFind.TabIndex = 99;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1142, 813);
		base.Controls.Add(this.luk_statusFind);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_ShowDetail);
		base.Controls.Add(this.date_searchTD);
		base.Controls.Add(this.labelLine26);
		base.Controls.Add(this.date_searchFD);
		base.Controls.Add(this.labelLine25);
		base.Controls.Add(this.labelLine24);
		base.Controls.Add(this.txt_noSearch);
		base.Controls.Add(this.btn_Find);
		base.Controls.Add(this.btn_ReportBy);
		base.Controls.Add(this.btn_HandleBy);
		base.Controls.Add(this.btn_Owner);
		base.Controls.Add(this.btn_ProjectCode);
		base.Controls.Add(this.btn_BpCode);
		base.Controls.Add(this.labelLine6);
		base.Controls.Add(this.labelLine5);
		base.Controls.Add(this.lbl_ReportBy);
		base.Controls.Add(this.labelLine3);
		base.Controls.Add(this.labelLine2);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.splitContainerControl1);
		base.FormId = "oTrackingIssue";
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_TrackingIssue.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_TrackingIssue";
		this.Text = "Tracking Issue";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_TrackingIssue_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.luk_statusGrid).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_TrackingIssue).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_TrackingIssue).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_typeGrid).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_gridReportBy).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_gridHandBy).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx1.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_gridBpCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_gridProject).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_priorityGrid).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_gridRes.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_gridRes).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridViewEx2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.btn_BpCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_ProjectCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_Owner.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_HandleBy.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_ReportBy.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_noSearch.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_searchFD.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_searchFD.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_searchTD.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_searchTD.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_statusFind.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
