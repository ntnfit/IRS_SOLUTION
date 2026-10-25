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
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Properties;
using Apzon.Forms.UserControls;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraLayout;
using DevExpress.XtraRichEdit;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Xml;
using DevExpress.XtraTab;

namespace Apzon.Forms.Administrator;

public class frm_NoteManagement : XtraFormEx
{
	public enum DateTimeInterval
	{
		Earlier,
		LastWeek,
		Yesterday,
		Today,
		Tomorrow,
		ThisWeek,
		Later
	}

	private GridView view;

	private RepositoryItemButtonEdit emptyEditor;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private bool _isChanging;

	private bool _FormLoading;

	private bool changingRow;

	private bool _isLoadingData;

	private bool _isChange;

	private bool commenting;

	private bool _forceClose;

	private bool _isCancel;

	private bool _isTextChanging;

	private bool isLoadFromOpportunity;

	private string _cardCodefrm_SalesNote;

	private bool _isDuplicate;

	private Dictionary<int, string> _dictionEmployee = new Dictionary<int, string>();

	private Dictionary<int, string> _dictionUser = new Dictionary<int, string>();

	public int ClgCode;

	public DataRow DataRowFromCal;

	public bool CallFromCal;

	public bool CallFromRelated;

	public Func<Task> ReloadCalendar;

	public Func<DataRow, Task> ReloadRelated;

	public Func<int, Task> ReloadAfterRemove;

	public int _owner;

	private DataTable _dtCurrentRow;

	private DataTable _dtActiveEmployee;

	private DataTable _dtSearch;

	public int _linkeDocType = -1;

	public int _linkDocNum = -1;

	private Enums.FormViewModeType _openFormViewMode = Enums.FormViewModeType.View;

	public Enums.ChangeLog ChangeLogType = Enums.ChangeLog.clNoteManagement;

	private IContainer components;

	private SplitContainerControl splitContainerControl1;

	private GridControlEx grd_Note;

	private GridViewEx gv_Note;

	private GridColumnEx cl_Details;

	private GridColumnEx cl_ClgCode;

	private LabelLine lb_BPCode;

	private BarManager barManager1;

	private BarDockControl barDockControlTop;

	private BarDockControl barDockControlBottom;

	private BarDockControl barDockControlLeft;

	private BarDockControl barDockControlRight;

	private ContextMenuStrip contextMenuStrip_Note;

	private ToolStripMenuItem toolStripMenuItem_Delete;

	private ucAttachment ucAttachment1;

	private ContextMenuStrip contextMenuStrip_Comment;

	private ToolStripMenuItem toolStripMenuItem_DeleteCM;

	private ComboBoxEditEx cbUser;

	private LabelLine lb_AssignedTo;

	private DateEditEx dStart_Recontact;

	private LabelLine lb_StartTime;

	private TextEditEx txtDetailNote;

	private GridColumnEx gc_AssignTo;

	private RepositoryItemLookUpEdit lk_AttendUser;

	private ButtonEditCFL txtBpCode;

	private ucComment ucComment1;

	private ucLinkedDocument ucLinkedDocument1;

	private TextEditEx txtClgCode;

	private LabelLine labelLine3;

	private LabelLine labelLine2;

	public SimpleButtonEx btnCancel;

	public SimpleButtonEx btn_Save;

	private ButtonEditCFL cfl_GroupEmployee;

	private ButtonEditCFL cfl_GroupUser;

	private ButtonEditCFL cfl_ProjectNew;

	private ButtonEditCFL cfl_ProjectWBS;

	private LabelLine labelLine4;

	private ButtonEditDistrRule bte_DistrRule;

	private LabelLine labelLine1;

	private LabelLine labelLine5;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem removeNoteToolStripMenuItem;

	private LabelLine lblCreatedBy;

	public SimpleButtonEx btnSearch;

	private DateEditEx date_To;

	private LabelLine lbl_ValidTo;

	private DateEditEx date_From;

	private LabelLine lbl_ValidFrom;

	private CheckEditEx chk_InviteAll;

	private GridColumnEx gridColumnEx1;

	private XtraTabControlEx xtraTabControl1;

	private XtraTabPageEx tab_Attachment;

	private XtraTabPageEx tab_Comment;

	private ButtonEditCFL cfl_Opportunity;

	private LabelLine labelLine6;

	private GridColumnEx gridColumnEx2;

	private RepositoryItemHourEdit repositoryItemHourEdit1;

	private WysiwygEdit wysiwygEdit1;

	public SimpleButtonEx btn_PushNotify;

	private ToolStripMenuItem toolStripMenuItem_Duplicate;

	private XtraTabPageEx xtraTabPageEx1;

	private LayoutUDF layout_UDF;

	private LayoutControlGroup Root;

	private ButtonEditCFL txtOwner;

	private LabelLine lbOwner;

	private SplitContainerControl splitContainerControl2;

	private ContextActionMappingImpl[] MappingDocument { get; set; }

	private DataTable OpenFormDocument { get; set; }

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
		case Enums.FormViewModeType.Add:
			btn_Save.Text = Apzon.Commons.Properties.Resources.ButtonAdd;
			break;
		case Enums.FormViewModeType.Update:
			btn_Save.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btn_Save.Text = Apzon.Commons.Properties.Resources.ButtonOK;
			break;
		}
	}

	public frm_NoteManagement()
	{
		Initialize();
	}

	public frm_NoteManagement(Enums.FormViewModeType openMode, ContextActionMappingImpl[] dataAction, DataTable dtMapping)
	{
		Initialize();
		_openFormViewMode = openMode;
		MappingDocument = dataAction;
		OpenFormDocument = dtMapping;
	}

	public void Initialize()
	{
		InitializeComponent();
		OnLoadAsync += frm_NoteManagement_Load;
		ucAttachment1.gv_File.BorderStyle = BorderStyles.NoBorder;
		ucAttachment1.gv_File.OptionsView.ShowColumnHeaders = false;
		ucAttachment1.btBrowse.BorderStyle = BorderStyles.Flat;
		ucAttachment1.btDelete.BorderStyle = BorderStyles.Flat;
		ucAttachment1.btDisplay.BorderStyle = BorderStyles.Flat;
		ucComment1.ObjectType = ApzObjectTypes.oNoteManagement;
		ucComment1.gv_Comment.Appearance.Empty.BackColor = Color.FromArgb(250, 250, 250);
		ucComment1.AutoGridSize = true;
		gc_AssignTo.OptionsColumn.ReadOnly = true;
		gc_AssignTo.OptionsColumn.AllowEdit = false;
		gc_AssignTo.OptionsColumn.AllowFocus = false;
		gc_AssignTo.OptionsColumn.TabStop = false;
		gc_AssignTo.Visible = true;
		splitContainerControl1.Collapsed = true;
		view = gv_Note;
		IsSetSuperTip = true;
		IsScrollable = true;
		FormId = "oNoteManagement";
		gv_Note.ItemId = "APZ_OCLG";
		gv_Note.FormId = FormId;
		RepositoryItemButtonEditCFL properties = cfl_GroupUser.Properties;
		properties.AfterShowCFL = (Action<frm_CFL>)Delegate.Combine(properties.AfterShowCFL, (Action<frm_CFL>)((frm_CFL frm) =>
		{
		}));
	}

	private async Task frm_NoteManagement_Load(object sender, EventArgs e)
	{
		_ = 2;
		try
		{
			if (CallFromCal || CallFromRelated)
			{
				splitContainerControl1.CollapsePanel = SplitCollapsePanel.Panel1;
				txtDetailNote.Enabled = true;
				AcceptButton = btn_Save;
				if (ClgCode == 0)
				{
					FormMode = Enums.FormViewModeType.Add;
					ucComment1.Enabled = false;
					ucAttachment1.Enabled = false;
				}
				else
				{
					ucComment1.Enabled = true;
					ucAttachment1.Enabled = true;
				}
				string text = "";
				if (DataRowFromCal.IsExistsColumn("Details"))
				{
					text = Function.ToString(DataRowFromCal["Details"]);
				}
				if (string.IsNullOrEmpty(text.Trim()))
				{
					Text = "Note - ...";
				}
				else
				{
					string[] array = text.Trim().Split(' ');
					if (array.Length >= 4)
					{
						Text = "Note - " + array[0] + " " + array[1] + " " + array[2] + " " + array[3];
						if (Text.Length > 30)
						{
							Text = Text.Substring(0, 30);
						}
						else
						{
							Text = Text;
						}
					}
					else
					{
						Text = "Note - " + text;
						if (Text.Length > 30)
						{
							Text = "Note - " + Text.Substring(0, 30);
						}
					}
				}
			}
			else
			{
				splitContainerControl1.CollapsePanel = SplitCollapsePanel.Panel2;
				splitContainerControl1.Collapsed = false;
				AcceptButton = null;
			}
			await LoadInitForm();
			await LoadData();
			if (_openFormViewMode != Enums.FormViewModeType.View && _openFormViewMode == Enums.FormViewModeType.Add)
			{
				await REFRESH();
				if (OpenFormDocument.IsNotNull())
				{
					ContextActionMappingImpl[] mappingDocument = MappingDocument;
					foreach (ContextActionMappingImpl contextActionMappingImpl in mappingDocument)
					{
						SetDataSourceControl(contextActionMappingImpl.TargetField, ContextActionMappingSource.GetValueSource(OpenFormDocument.Rows[0], contextActionMappingImpl));
					}
				}
				FormMode = Enums.FormViewModeType.Add;
			}
			SetMaxLengthForControls();
			SetEventControl(this);
			gv_Note.OptionsView.ColumnAutoWidth = true;
			if (CallFromCal || CallFromRelated)
			{
				txtDetailNote.Focus();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Note_DoubleClick(object sender, EventArgs e)
	{
		try
		{
			if (grd_Note.DataSourceTable.HasColumn())
			{
				GridView gridView = (GridView)sender;
				Point pt = gridView.GridControl.PointToClient(Control.MousePosition);
				GridHitInfo gridHitInfo = gridView.CalcHitInfo(pt);
				if (gridHitInfo.InRow || gridHitInfo.InRowCell)
				{
					splitContainerControl1.Collapsed = !splitContainerControl1.Collapsed;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void gv_Note_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			if (e.FocusedRowHandle < 0)
			{
				splitContainerControl1.Panel2.Controls.EnableDisableControl(isEnable: false);
				return;
			}
			splitContainerControl1.Panel2.Controls.EnableDisableControl(isEnable: true);
			_isLoadingData = true;
			grd_Note.DataSourceTable.AcceptChanges();
			string currentClgCode = Function.ToString(gv_Note.GetFocusedRowCellValue("ClgCode"));
			DataTable dataTable = grd_Note.DataSource as DataTable;
			if (dataTable.IsNotNull())
			{
				DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["ClgCode"]) == currentClgCode);
				if (dataRow != null)
				{
					DataSet dataSet = new DataSet();
					_dtCurrentRow = dataRow.ToArray().CopyToDataTable();
					dataSet.Tables.Add(_dtCurrentRow);
					await BindControl(dataSet);
				}
			}
			if (Function.ParseInt(currentClgCode) != 0)
			{
				FormMode = (_isDuplicate ? Enums.FormViewModeType.Add : Enums.FormViewModeType.View);
				_isDuplicate = false;
				if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_NoteManagement" && x.authoriza == "R"))
				{
					ucComment1.Enabled = false;
				}
				else
				{
					ucComment1.Enabled = true;
				}
			}
			else
			{
				ucComment1.Enabled = false;
				FormMode = Enums.FormViewModeType.Add;
			}
			if (Function.ToString(gv_Note.GetFocusedRowCellValue("OwnerAuth")).Equals("F"))
			{
				Controls.EnableDisableControl(isEnable: true);
				if (ContextMenuStrip != null)
				{
					ContextMenuStrip.Enabled = true;
				}
			}
			else
			{
				Controls.EnableDisableControl(isEnable: false);
				if (ContextMenuStrip != null)
				{
					ContextMenuStrip.Enabled = false;
				}
			}
			_isLoadingData = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void gv_Note_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
	{
	}

	private void gv_Note_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (Function.ParseInt(gv_Note.GetFocusedRowCellValue("ClgCode")) != 0)
			{
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Note_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
	{
		try
		{
			if (!(sender is GridView gridView) || e.Column.FieldName != "AssignTo" || !e.IsGetData)
			{
				return;
			}
			int listSourceRowIndex = e.ListSourceRowIndex;
			string text = Function.ToString(gridView.GetListSourceRowCellValue(listSourceRowIndex, "GroupUser"));
			string text2 = Function.ToString(gridView.GetListSourceRowCellValue(listSourceRowIndex, "GroupEmployee"));
			string text3 = Function.ToString(gridView.GetListSourceRowCellValue(listSourceRowIndex, "AllUserInvite"));
			string text4 = "";
			if (!string.IsNullOrEmpty(text))
			{
				foreach (string item in text.Split(new char[1] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList())
				{
					int key = Function.ParseInt(item);
					if (_dictionUser.ContainsKey(key))
					{
						text4 = text4 + _dictionUser[key] + "; ";
					}
				}
			}
			if (string.IsNullOrEmpty(text))
			{
				foreach (string item2 in text2.Split(new char[1] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList())
				{
					int key2 = Function.ParseInt(item2);
					if (_dictionEmployee.ContainsKey(key2))
					{
						text4 = text4 + _dictionEmployee[key2] + "; ";
					}
				}
			}
			if (text4.Length > 2)
			{
				text4 = text4.Remove(text4.Length - 2);
			}
			if (Function.ToString(text3) == "Y")
			{
				e.Value = "All Resources";
			}
			else
			{
				e.Value = text4;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Note_BeforeLeaveRow(object sender, RowAllowEventArgs e)
	{
		try
		{
			if (!_isChanging && !_isLoadingData && FormMode == Enums.FormViewModeType.Update)
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
				{
					e.Allow = false;
				}
				else
				{
					grd_Note.DataSourceTable.RejectChanges();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Note_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (!_isChanging)
			{
				_isTextChanging = true;
				if (e.Column.FieldName == "Details")
				{
					txtDetailNote.Text = Function.ToString(e.Value);
				}
				_isTextChanging = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Note_CustomColumnSort(object sender, CustomColumnSortEventArgs e)
	{
		if (!(sender is GridView gridView))
		{
			return;
		}
		try
		{
			e.Handled = true;
			int num = Function.ParseInt(gridView.GetListSourceRowCellValue(e.ListSourceRowIndex1, "ClgCode"));
			int num2 = Function.ParseInt(gridView.GetListSourceRowCellValue(e.ListSourceRowIndex2, "ClgCode"));
			e.Handled = true;
			if (num == 0)
			{
				e.Result = ((e.SortOrder != ColumnSortOrder.Ascending) ? 1 : (-1));
			}
			else if (num2 == 0)
			{
				e.Result = ((e.SortOrder == ColumnSortOrder.Ascending) ? 1 : (-1));
			}
			else
			{
				e.Handled = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Note_CustomColumnGroup(object sender, CustomColumnSortEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == "Recontact" || e.Column.FieldName == "endDate" || e.Column.FieldName == "CntctDate")
			{
				int interval = (int)GetInterval(((DateTime)e.Value1 - DateTime.Today).Days);
				int interval2 = (int)GetInterval(((DateTime)e.Value2 - DateTime.Today).Days);
				e.Result = ConvertResult(interval - interval2);
				e.Handled = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private int ConvertResult(int result)
	{
		if (result > 0)
		{
			return 1;
		}
		if (result < 0)
		{
			return -1;
		}
		return 0;
	}

	private DateTimeInterval GetInterval(int days)
	{
		if (days < -7)
		{
			return DateTimeInterval.Earlier;
		}
		if (days < -1)
		{
			return DateTimeInterval.LastWeek;
		}
		if (days == -1)
		{
			return DateTimeInterval.Yesterday;
		}
		if (days == 0)
		{
			return DateTimeInterval.Today;
		}
		if (days == 1)
		{
			return DateTimeInterval.Tomorrow;
		}
		if (days <= 7)
		{
			return DateTimeInterval.ThisWeek;
		}
		return DateTimeInterval.Later;
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		_isCancel = true;
		Close();
		_isCancel = false;
	}

	private async void btn_Save_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.View)
			{
				btn_Save.Enabled = false;
				btnCancel.Enabled = false;
				await SaveData();
				btn_Save.Enabled = true;
				btnCancel.Enabled = true;
			}
			else
			{
				_isCancel = true;
				Close();
				_isCancel = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<string> SaveData()
	{
		_ = 3;
		try
		{
			if (string.IsNullOrEmpty(Function.ToString(_dtCurrentRow.Rows[0]["Details"])))
			{
				WinCommons.Error(Enums.MessageCode.InfoSubjectCanNotEmpty, "");
				return "Error";
			}
			DataSet dataSet = GetControlsValueOCLG().Copy();
			if (ucAttachment1.GetAttachmentNoSAP(out var dtAtt))
			{
				dtAtt.TableName = "Attachment";
			}
			if (dtAtt != null)
			{
				if (!dtAtt.HasColumn())
				{
					dtAtt = WinFuntions.GetTableStructure("ATC1");
				}
				dataSet.Tables.Add(dtAtt);
			}
			int clgCode = Function.ParseInt(_dtCurrentRow.Rows[0]["ClgCode"]);
			WinCommons._OpenProcessing();
			if (dataSet.IsNotNull())
			{
				HttpResult httpResult = await _httpClient.SubActivities.SaveNote(dataSet);
				if (string.IsNullOrEmpty(httpResult.Message))
				{
					string[] array = Function.ToString(httpResult.Content).Split(',');
					WinCommons.Success(Enums.MessageCode.Success, "");
					ucComment1.Enabled = true;
					FormMode = Enums.FormViewModeType.View;
					if (array.Length > 1)
					{
						int num = Function.ParseInt(array[1]);
						if (dtAtt != null && num != 0)
						{
							_dtCurrentRow.Rows[0]["AtcEntry"] = num;
							_dtCurrentRow.AcceptChanges();
							foreach (DataRow item in ucAttachment1.grd_File.DataSourceTable.AsEnumerable())
							{
								item["AbsEntry"] = num;
							}
							ucAttachment1.grd_File.DataSourceTable.AcceptChanges();
						}
					}
					if (clgCode <= 0)
					{
						_dtCurrentRow.Rows[0]["ClgCode"] = Function.ParseInt(array[0]);
						if (!CallFromCal && !CallFromRelated)
						{
							_isChanging = true;
							gv_Note.SetFocusedRowCellValue("ClgCode", Function.ParseInt(array[0]));
							_isChanging = false;
							PostDataBackToGridSource(_dtCurrentRow);
							AddNewRow();
							int rowHandle = gv_Note.GetRowHandle(grd_Note.DataSourceTable.Rows.Count - 1);
							gv_Note.FocusedRowHandle = rowHandle;
						}
					}
					else if (!CallFromCal && !CallFromRelated)
					{
						PostDataBackToGridSource(_dtCurrentRow);
					}
					ClgCode = Function.ParseInt(array[0]);
					if (CallFromCal)
					{
						if (ReloadCalendar != null && !((XtraForm)ReloadCalendar.Target).IsDisposed)
						{
							await ReloadCalendar();
						}
						_isCancel = true;
						Close();
						_isCancel = false;
					}
					if (CallFromRelated)
					{
						if (ReloadRelated != null && !((XtraForm)ReloadRelated.Target).IsDisposed)
						{
							await ReloadRelated(_dtCurrentRow.Copy().Rows[0]);
						}
						_isCancel = true;
						Close();
						_isCancel = false;
					}
					await WinFuntions.RefreshVisibleCalendars();
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
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		return string.Empty;
	}

	private void toolStripMenuItem_Delete_Click(object sender, EventArgs e)
	{
		try
		{
			RemoveProcess();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task RemoveProcess(bool callFromCal = false)
	{
		try
		{
			if ((callFromCal || ((grd_Note.DataSource as DataTable).IsNotNull() && Function.ParseInt(gv_Note.GetFocusedRowCellValue("ClgCode")) != 0 && gv_Note.FocusedRowHandle >= 0)) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				await RemoveCurrentNote();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task RemoveCurrentNote()
	{
		_ = 3;
		try
		{
			if (!_dtCurrentRow.IsNotNull())
			{
				return;
			}
			WinCommons._OpenProcessing();
			DataTable dt = grd_Note.DataSource as DataTable;
			int code = Function.ParseInt(gv_Note.GetFocusedRowCellValue("ClgCode"));
			string text = await _httpClient.SubActivities.DeleteNote(_dtCurrentRow);
			if (string.IsNullOrEmpty(text))
			{
				if (dt.IsNotNull())
				{
					int dataSourceIndex = dt.Rows.IndexOf(dt.Rows.Find(code));
					int rowHandle = gv_Note.GetRowHandle(dataSourceIndex);
					if (rowHandle > 0)
					{
						gv_Note.DeleteRow(rowHandle);
					}
					dt.AcceptChanges();
					grd_Note.RefreshDataSource();
				}
				WinCommons.Success(Enums.MessageCode.DeleteSuccess, "");
				if (ReloadCalendar != null && !((XtraForm)ReloadCalendar.Target).IsDisposed)
				{
					await ReloadCalendar();
				}
				if (CallFromCal || CallFromRelated)
				{
					_isCancel = true;
					Close();
					_isCancel = false;
				}
				await WinFuntions.RefreshVisibleCalendars();
				if (ReloadAfterRemove != null && !((XtraForm)ReloadAfterRemove.Target).IsDisposed)
				{
					await ReloadAfterRemove(code);
				}
				WinCommons._CloseProcessing();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			WinCommons._CloseProcessing();
		}
	}

	private void cbUser_SelectedValueChanged(object sender, EventArgs e)
	{
		try
		{
			string text = Function.ToString(cbUser.EditValue);
			if (!(text == "User"))
			{
				if (!(text == "Employee"))
				{
					return;
				}
				cfl_GroupUser.Visible = false;
				cfl_GroupEmployee.Visible = true;
				if (string.IsNullOrEmpty(Function.ToString(cfl_GroupEmployee.EditValue)) && _dtActiveEmployee.IsNotNull())
				{
					DataRow dataRow = _dtActiveEmployee.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["userId"]) == GlobalData.UserID);
					if (dataRow != null)
					{
						cfl_GroupEmployee.EditValue = Function.ParseInt(dataRow["empID"]);
					}
				}
				if (!changingRow)
				{
					cfl_GroupUser.EditValue = null;
				}
			}
			else
			{
				cfl_GroupUser.Visible = true;
				cfl_GroupEmployee.Visible = false;
				if (!changingRow)
				{
					cfl_GroupEmployee.EditValue = null;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void splitContainerControl1_SplitGroupPanelCollapsing(object sender, SplitGroupPanelCollapsingEventArgs e)
	{
		try
		{
			if (CallFromCal || CallFromRelated)
			{
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void frm_NoteManagement_FormClosing(object sender, FormClosingEventArgs e)
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
					if (string.IsNullOrEmpty(await SaveData()))
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

	private void txtDetailNote_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (!_isTextChanging)
			{
				_isChanging = true;
				gv_Note.SetFocusedRowCellValue("Details", e.NewValue);
				_isChanging = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void removeNoteToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			RemoveProcess(callFromCal: true);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void grd_Note_ProcessGridKey(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.Return)
			{
				gv_Note.PostEditor();
				await SaveData();
				e.Handled = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void contextMenuStrip_Note_Opening(object sender, CancelEventArgs e)
	{
		try
		{
			e.Cancel = gv_Note.FocusedRowHandle < 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
	{
		try
		{
			if ((!CallFromCal && !CallFromRelated) || (FormMode == Enums.FormViewModeType.Add && (CallFromCal || CallFromRelated)))
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

	private void btnSearch_Click(object sender, EventArgs e)
	{
		try
		{
			if (Function.ParseDateTimes(date_To.EditValue) < Function.ParseDateTimes(date_From.EditValue) && date_To.EditValue != null)
			{
				WinCommons.Error(Enums.MessageCode.ToDateMustGreaterFromDate, "");
			}
			else if (FormMode != Enums.FormViewModeType.Update || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				LoadData();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void chk_InviteAll_CheckStateChanged(object sender, EventArgs e)
	{
		try
		{
			if (((CheckEdit)sender).CheckState == CheckState.Checked)
			{
				cfl_GroupUser.Enabled = false;
				cfl_GroupEmployee.Enabled = false;
			}
			else
			{
				cfl_GroupUser.Enabled = true;
				cfl_GroupEmployee.Enabled = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async void AddNewRow()
	{
		try
		{
			gv_Note.BeginInsertRow();
			DataTable dataTable = grd_Note.DataSourceTable;
			if (dataTable.IsNotNull())
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["ClgCode"] = 0;
				dataRow["GroupUser"] = Function.ToString(GlobalData.UserID);
				dataRow["GroupEmployee"] = "";
				dataRow["Action"] = 'E';
				dataRow["Recontact"] = DateTime.Today;
				dataRow["endDate"] = DateTime.Today;
				dataRow["AllUserInvite"] = "N";
				dataRow["AllEmplInvite"] = "N";
				dataRow["CntctDate"] = DateTime.Today;
				dataRow["CntctTime"] = Function.ParseTimeSpanToInt(DateTime.Now.TimeOfDay);
				dataRow["AssignedBy"] = GlobalData.UserID;
				dataRow["OwnerAuth"] = "F";
				if (_owner != 0)
				{
					dataRow["Owner"] = _owner;
				}
				foreach (Control control3 in layout_UDF.Controls)
				{
					if (control3 is BaseEdit && dataTable.Columns.Contains(control3.Name) && !string.IsNullOrEmpty(control3.AccessibleDescription))
					{
						dataRow[control3.Name] = control3.AccessibleDescription;
					}
				}
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
			}
			else
			{
				dataTable = WinFuntions.GetTableStructure("APZ_OCLG");
				DataRow dataRow2 = dataTable.NewRow();
				dataRow2["ClgCode"] = 0;
				dataRow2["Action"] = 'E';
				dataRow2["GroupUser"] = Function.ToString(GlobalData.UserID);
				dataRow2["GroupEmployee"] = "";
				dataRow2["Recontact"] = DateTime.Today;
				dataRow2["endDate"] = DateTime.Today;
				dataRow2["AllUserInvite"] = "N";
				dataRow2["AllEmplInvite"] = "N";
				dataRow2["CntctDate"] = DateTime.Today;
				dataRow2["CntctTime"] = Function.ParseTimeSpanToInt(DateTime.Now.TimeOfDay);
				dataRow2["AssignedBy"] = GlobalData.UserID;
				dataRow2["OwnerAuth"] = "F";
				if (_owner != 0)
				{
					dataRow2["Owner"] = _owner;
				}
				foreach (Control control4 in layout_UDF.Controls)
				{
					if (control4 is BaseEdit && dataTable.Columns.Contains(control4.Name) && !string.IsNullOrEmpty(control4.AccessibleDescription))
					{
						dataRow2[control4.Name] = control4.AccessibleDescription;
					}
				}
				dataTable.Rows.Add(dataRow2);
				dataTable.AcceptChanges();
			}
			dataTable.SetDefaultAndMaxLen(gv_Note.ItemId);
			grd_Note.DataSource = dataTable;
			dataTable.PrimaryKey = dataTable.Columns["ClgCode"].ToArray();
			grd_Note.RefreshDataSource();
			grd_Note.DataSourceTable.AcceptChanges();
			gv_Note.EndInsertRow();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task CalAddNew()
	{
		try
		{
			DataTable tableStructure = WinFuntions.GetTableStructure("APZ_OCLG");
			tableStructure.SetDefaultAndMaxLen(gv_Note.ItemId);
			if (DataRowFromCal != null)
			{
				DataRow dataRow = tableStructure.NewRow();
				dataRow["ClgCode"] = 0;
				ClgCode = 0;
				dataRow["BeginTime"] = DataRowFromCal["BeginTime"];
				dataRow["ENDTime"] = DataRowFromCal["ENDTime"];
				dataRow["Recontact"] = DataRowFromCal["Recontact"];
				dataRow["endDate"] = DataRowFromCal["endDate"];
				dataRow["GroupUser"] = DataRowFromCal["GroupUser"];
				dataRow["GroupEmployee"] = DataRowFromCal["GroupEmployee"];
				dataRow["ResourceIdsInfo"] = Function.ToString(DataRowFromCal["ResourceIdsInfo"]);
				dataRow["DocType"] = DataRowFromCal["DocType"];
				dataRow["DocEntry"] = DataRowFromCal["DocEntry"];
				dataRow["DocNum"] = DataRowFromCal["DocNum"];
				dataRow["CardCode"] = DataRowFromCal["CardCode"];
				dataRow["ProjectEntry"] = DataRowFromCal["ProjectEntry"];
				dataRow["ProjectWBS"] = DataRowFromCal["ProjectWBS"];
				dataRow["OprId"] = DataRowFromCal["OprId"];
				dataRow["OprLine"] = DataRowFromCal["OprLine"];
				dataRow["AllUserInvite"] = "N";
				dataRow["AllEmplInvite"] = "N";
				dataRow["CntctDate"] = DateTime.Today;
				dataRow["CntctTime"] = Function.ParseTimeSpanToInt(DateTime.Now.TimeOfDay);
				dataRow["AssignedBy"] = GlobalData.UserID;
				tableStructure.Rows.Add(dataRow);
				tableStructure.AcceptChanges();
			}
			DataSet dataSet = new DataSet();
			_dtCurrentRow = tableStructure.Copy();
			dataSet.Tables.Add(_dtCurrentRow);
			await BindControl(dataSet);
			ucComment1.Enabled = false;
			FormMode = Enums.FormViewModeType.Add;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private DataSet GetControlsValueOCLG()
	{
		DataSet dataSet = new DataSet();
		DataRow dataRow = _dtCurrentRow.Rows[0];
		try
		{
			string text = DateTime.Now.ToString("HH:mm");
			string text2 = DateTime.Now.AddMinutes(30.0).ToString("HH:mm");
			if (Function.ParseInt(dataRow["BeginTime"]) == 0)
			{
				dataRow["BeginTime"] = Function.ParseInt(text.Replace(":", ""));
			}
			if (Function.ParseInt(dataRow["ENDTime"]) == 0)
			{
				dataRow["ENDTime"] = Function.ParseInt(text2.Replace(":", ""));
			}
			string text3 = cbUser.SelectedItem.ToString();
			if (text3.Equals("User"))
			{
				dataRow["GroupUser"] = Function.ToString(cfl_GroupUser.EditValue);
				dataRow["GroupEmployee"] = DBNull.Value;
			}
			else if (text3.Equals("Employee"))
			{
				dataRow["GroupEmployee"] = Function.ToString(cfl_GroupEmployee.EditValue);
				dataRow["GroupUser"] = DBNull.Value;
			}
			dataRow["DocType"] = ucLinkedDocument1.DocType;
			dataRow["DocNum"] = ucLinkedDocument1.EditValue;
			dataRow["DocEntry"] = ucLinkedDocument1.DocEntry;
			dataRow["UpdateDate"] = DateTime.Today;
			dataRow["UpdateTime"] = Function.ParseTimeSpanToInt(DateTime.Now.TimeOfDay);
			dataRow["UserSign"] = GlobalData.UserID;
			dataRow["Closed"] = "N";
			dataRow["status"] = 0;
			dataRow["Action"] = 'E';
			dataRow["Notes"] = wysiwygEdit1.richEditor.Text;
			dataRow["HtmlNotes"] = wysiwygEdit1.richEditor.WordMLText;
			ResourceIdCollection resourceIdCollection = new ResourceIdCollection();
			SchedulerCompatibility.Base64XmlObjectSerialization = false;
			if (chk_InviteAll.CheckState == CheckState.Checked)
			{
				resourceIdCollection.Clear();
			}
			else if (Function.ToString(cbUser.EditValue) == "User")
			{
				string text4 = Function.ToString(cfl_GroupUser.EditValue);
				if (!string.IsNullOrEmpty(text4))
				{
					string[] array = Function.ToString(text4).Split(';');
					foreach (string text5 in array)
					{
						if (!string.IsNullOrWhiteSpace(text5))
						{
							resourceIdCollection.Add("U" + Function.ToString(text5));
						}
					}
				}
			}
			else if (Function.ToString(cbUser.EditValue) == "Employee")
			{
				string text4 = Function.ToString(cfl_GroupEmployee.EditValue);
				if (!string.IsNullOrEmpty(text4))
				{
					string[] array = Function.ToString(text4).Split(';');
					foreach (string text6 in array)
					{
						if (!string.IsNullOrWhiteSpace(text6))
						{
							resourceIdCollection.Add("E" + Function.ToString(text6));
						}
					}
				}
			}
			string value = new AppointmentResourceIdCollectionXmlPersistenceHelper(resourceIdCollection).ToXml();
			dataRow["ResourceIdsInfo"] = value;
			bte_DistrRule.AddValueToRow(dataRow);
			_dtCurrentRow.AcceptChanges();
			dataSet.Tables.Add(_dtCurrentRow.Copy());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return dataSet;
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			if (!CallFromCal && !CallFromRelated)
			{
				btnSearch_Click(null, null);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task REFRESH()
	{
		try
		{
			if (!CallFromCal && !CallFromRelated)
			{
				DataTable dataSourceTable = grd_Note.DataSourceTable;
				int dataSourceIndex = dataSourceTable.Rows.IndexOf(dataSourceTable.Rows.Find(0));
				gv_Note.FocusedRowHandle = gv_Note.GetRowHandle(dataSourceIndex);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadInitForm()
	{
		_ = 5;
		try
		{
			cfl_ProjectNew.Properties.ValueMember = "AbsEntry";
			cfl_ProjectNew.Properties.DisplayMember = "Code";
			cfl_ProjectNew.Properties.DisplayMember2 = "Name";
			cfl_ProjectNew.Properties.SeparateString = " - ";
			cfl_ProjectNew.Properties.HandleChooseItem = Project_HandleChooseItem;
			RepositoryItemButtonEditCFL properties = cfl_ProjectNew.Properties;
			properties.AfterShowCFL = (Action<frm_CFL>)Delegate.Combine(properties.AfterShowCFL, (Action<frm_CFL>)((frm_CFL frm) =>
			{
				try
				{
					DataTable dataTable2 = frm.gv_CFL.GridControl.DataSource as DataTable;
					if (dataTable2.IsNotNull())
					{
						EnumerableRowCollection<DataRow> source = from s in dataTable2.AsEnumerable()
							where Function.ToString(s["PrgStatus"]) == "S"
							select s;
						if (source.Any())
						{
							frm.gv_CFL.GridControl.DataSource = source.CopyToDataTable();
						}
						else
						{
							dataTable2.Clear();
							dataTable2.AcceptChanges();
						}
					}
				}
				catch (Exception ex2)
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
				}
			}));
			cfl_GroupEmployee.Properties.ValueMember = "EmpId";
			cfl_GroupEmployee.Properties.DisplayMember = "lastName";
			cfl_GroupEmployee.Properties.DisplayMember2 = "firstName";
			cfl_GroupEmployee.Properties.TypeCFL = Enums.TypeCFL.EmployeeListForSalesEmpSetup;
			cfl_GroupEmployee.Properties.TypeDrillDown = ApzObjectTypes.oEmployeesInfo;
			cfl_GroupEmployee.Properties.MultiSelectMode = true;
			cfl_GroupUser.Properties.ValueMember = "USERID";
			cfl_GroupUser.Properties.DisplayMember = "U_NAME";
			cfl_GroupUser.Properties.DisplayMember2 = "";
			cfl_GroupUser.Properties.TypeCFL = Enums.TypeCFL.GetUserList;
			cfl_GroupUser.Properties.TypeDrillDown = ApzObjectTypes.oNone;
			cfl_GroupUser.Properties.Buttons[1].Visible = false;
			cfl_GroupUser.Properties.MultiSelectMode = true;
			cfl_GroupEmployee.Properties.ValidateOnEnterKey = true;
			cfl_GroupUser.Properties.ValidateOnEnterKey = true;
			Task t2 = cfl_GroupUser.Properties.RefreshDataSourceAsync();
			Task t3 = cfl_GroupEmployee.Properties.RefreshDataSourceAsync();
			Task t5 = cfl_ProjectNew.Properties.RefreshDataSourceAsync();
			Task<int> t6 = layout_UDF.LoadControls("APZ_OCLG", ApzObjectTypes.oNoteManagement);
			DataTable dataTable = await _httpClient.SubActivities.GetEmployeeByUserId(GlobalData.UserID);
			if (dataTable.IsNotNull())
			{
				_owner = Function.ParseInt(dataTable.Rows[0][0]);
			}
			Task t7 = txtOwner.Properties.RefreshDataSourceAsync();
			await t2;
			await t3;
			await t5;
			await t6;
			await t7;
			cfl_GroupUser.EditValueChanged += cfl_GroupUser_EditValueChanged;
			cfl_GroupEmployee.EditValueChanged += cfl_GroupUser_EditValueChanged;
			_dictionUser.Clear();
			_dictionUser = cfl_GroupUser.Properties.dictionSource.ToDictionary((KeyValuePair<string, string> s) => Function.ParseInt(s.Key), (KeyValuePair<string, string> s) => s.Value);
			_dictionEmployee.Clear();
			_dictionEmployee = cfl_GroupEmployee.Properties.dictionSource.ToDictionary((KeyValuePair<string, string> s) => Function.ParseInt(s.Key), (KeyValuePair<string, string> s) => s.Value);
			_dtActiveEmployee = cfl_GroupEmployee.Properties.DataSource;
			cfl_ProjectWBS.Properties.ValueMember = "TaskCode";
			cfl_ProjectWBS.Properties.TypeCFL = Enums.TypeCFL.ProjectTask;
			cfl_ProjectWBS.Properties.TypeDrillDown = ApzObjectTypes.oProjectManager;
			RepositoryItemButtonEditCFL properties2 = cfl_ProjectWBS.Properties;
			properties2.BeforeShowCFL = (Action<frm_CFL>)Delegate.Combine(properties2.BeforeShowCFL, (Action<frm_CFL>)((frm_CFL frm) =>
			{
				frm.projectId = Function.ParseInt(cfl_ProjectNew.EditValue);
			}));
			cfl_ProjectWBS.Properties.LoadDataSourceWhenEnter = true;
			gc_AssignTo.OptionsColumn.ReadOnly = true;
			gc_AssignTo.OptionsColumn.AllowEdit = false;
			gc_AssignTo.OptionsColumn.AllowFocus = false;
			gc_AssignTo.OptionsColumn.TabStop = false;
			txtBpCode.Properties.LoadDataSourceWhenEnter = true;
			ucLinkedDocument1.ItemSelected += OnContentChanged;
			ucLinkedDocument1.cbDocNum.EditValueChanged += OnContentChanged;
			ucLinkedDocument1.BeforeButtonPressed += UcLinkedDocument1_BeforeButtonPressed;
			gv_Note.RowCellStyle += GvDocOnRowCellStyle;
			emptyEditor = new RepositoryItemButtonEdit();
			emptyEditor.Buttons.Clear();
			emptyEditor.TextEditStyle = TextEditStyles.HideTextEditor;
			grd_Note.RepositoryItems.Add(emptyEditor);
			ucAttachment ucAttachment2 = ucAttachment1;
			ucAttachment2.BrowsedFile = (Action<List<DataRow>>)Delegate.Combine(ucAttachment2.BrowsedFile, (Action<List<DataRow>>)((List<DataRow> s) =>
			{
				if (FormMode == Enums.FormViewModeType.View)
				{
					FormMode = Enums.FormViewModeType.Update;
				}
			}));
			ucAttachment1.ButtonDelete_Click += ucAttachment1_ButtonBrowse_Click;
			ucAttachment1.gv_File.BorderStyle = BorderStyles.NoBorder;
			ucAttachment1.gv_File.OptionsView.ShowColumnHeaders = false;
			ucAttachment1.btBrowse.BorderStyle = BorderStyles.Flat;
			ucAttachment1.btDelete.BorderStyle = BorderStyles.Flat;
			ucAttachment1.btDisplay.BorderStyle = BorderStyles.Flat;
			ucAttachment1.gv_File.Columns["FileName"].VisibleIndex = 0;
			ucAttachment1.gv_File.Columns["trgtPath"].VisibleIndex = 1;
			_isLoadingData = true;
			date_From.EditValue = DateTime.Today.AddDays(-Configuration.SearchDate);
			date_To.EditValue = DateTime.Today.AddDays(Configuration.SearchDate);
			_isLoadingData = false;
			cfl_Opportunity.Properties.ValueMember = "OprId";
			cfl_Opportunity.Properties.DisplayMember = "OprId";
			cfl_Opportunity.Properties.DisplayMember2 = "Name";
			cfl_Opportunity.Properties.SeparateString = " - ";
			cfl_Opportunity.Properties.TypeDrillDown = ApzObjectTypes.oSalesOpportunities;
			cfl_Opportunity.Properties.TypeCFL = Enums.TypeCFL.Opportunity;
			cfl_Opportunity.Properties.HandleChooseItem = Opportunity_HandleChooseItem;
			wysiwygEdit1.richEditor.AutoCorrect += RichEditor_AutoCorrect;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void Project_HandleChooseItem(DataRow dr)
	{
		try
		{
			cfl_ProjectNew.EditValue = dr["AbsEntry"];
			Function.ToString(txtBpCode.EditValue);
			txtBpCode.EditValue = dr["CardCode"];
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void Opportunity_HandleChooseItem(DataRow dr)
	{
		try
		{
			cfl_Opportunity.EditValue = Function.ParseInt(dr["OprId"]);
			if (_dtCurrentRow.IsNotNull())
			{
				_dtCurrentRow.Rows[0]["OprLine"] = Function.ParseInt(dr["OprLine"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void UcLinkedDocument1_BeforeButtonPressed(object sender, EventArgs e)
	{
		ucLinkedDocument1.FindCardCode = Function.ToString(txtBpCode.EditValue);
	}

	private void GvDocOnRowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (sender is GridView gridView && e.Column.FieldName == "AssignTo")
			{
				string value = Function.ToString(gridView.GetRowCellValue(e.RowHandle, "GroupUser"));
				string value2 = Function.ToString(gridView.GetRowCellValue(e.RowHandle, "GroupEmployee"));
				if (string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(value2))
				{
					e.Appearance.ForeColor = Color.FromArgb(125, 88, 0);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_PushNotify_Click(object sender, EventArgs e)
	{
		try
		{
			int num = Function.ParseInt(_dtCurrentRow.Rows[0]["ClgCode"]);
			if (num > 0 && XtraMessageBoxEx.Show(Enums.MessageCode.PushNotificationConfirm, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				string text = await _httpClient.SubActivities.PushAppNotification(Function.ToString(num));
				if (string.IsNullOrEmpty(text))
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, text);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
	}

	private void toolStripMenuItem_Duplicate_Click(object sender, EventArgs e)
	{
		try
		{
			if (gv_Note.FocusedRowHandle == 0 || (FormMode == Enums.FormViewModeType.Add && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No))
			{
				return;
			}
			WinCommons._OpenProcessing();
			DataTable dataTable = grd_Note.DataSourceTable.Copy();
			dataTable.PrimaryKey = null;
			dataTable.Columns["ClgCode"].Unique = false;
			DataRow focusedDataRow = gv_Note.GetFocusedDataRow();
			int num = Function.ParseInt(dataTable.AsEnumerable().Max((DataRow r) => Function.ParseInt(r["ClgCode"]))) + 1;
			DataRow dataRow = dataTable.NewRow();
			dataRow.ItemArray = focusedDataRow.ItemArray;
			dataRow["ClgCode"] = num;
			dataRow["Recontact"] = DateTime.Today;
			dataRow["endDate"] = DateTime.Today;
			dataRow["CntctDate"] = DateTime.Today;
			dataRow["CntctTime"] = Function.ParseTimeSpanToInt(DateTime.Now.TimeOfDay);
			dataRow["UserSign"] = GlobalData.UserID;
			dataRow["AssignedBy"] = GlobalData.UserID;
			dataRow["Recontact"] = DateTime.Today;
			dataRow["UpdateDate"] = DateTime.Today;
			dataTable.Rows.Add(dataRow.ItemArray);
			dataTable.AcceptChanges();
			grd_Note.DataSource = dataTable;
			grd_Note.RefreshDataSource();
			gv_Note.FocusedRowHandle = 1;
			FormMode = Enums.FormViewModeType.Add;
			_isDuplicate = true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private void wysiwygEdit1_Validating(object sender, CancelEventArgs e)
	{
		try
		{
			if (wysiwygEdit1.richEditor.Document.Text.Trim() == "" && wysiwygEdit1.richEditor.Document.Images.Count == 0 && (wysiwygEdit1.richEditor.Document.WordMLText.Contains("w:hdr") || wysiwygEdit1.richEditor.Document.WordMLText.Contains("w:ftr")) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmClearHeaderFooter, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				wysiwygEdit1.richEditor.ResetText();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void RichEditor_AutoCorrect(object sender, AutoCorrectEventArgs e)
	{
		if (FormMode == Enums.FormViewModeType.View)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	private void ucAttachment1_ButtonBrowse_Click(object sender, EventArgs e)
	{
		if (FormMode == Enums.FormViewModeType.View)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	private void cfl_GroupUser_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			_isChanging = true;
			DataTable dataSourceTable = grd_Note.DataSourceTable;
			int clgCode = Function.ParseInt(gv_Note.GetFocusedRowCellValue("ClgCode"));
			DataRow dataRow = dataSourceTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["ClgCode"]) == clgCode);
			if (dataRow != null)
			{
				if (((ButtonEditCFL)sender).Name == "cfl_GroupUser")
				{
					dataRow["GroupUser"] = Function.ToString(cfl_GroupUser.EditValue);
				}
				else if (((ButtonEditCFL)sender).Name == "cfl_GroupEmployee")
				{
					dataRow["GroupEmployee"] = Function.ToString(cfl_GroupEmployee.EditValue);
				}
			}
			gv_Note.RefreshData();
			_isChanging = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task SetMaxLengthForControls()
	{
		try
		{
			Dictionary<string, Control> dicControls = new Dictionary<string, Control>
			{
				{ "APZ_OCLG.Details", txtDetailNote },
				{ "APZ_OCLG.CardCode", txtBpCode }
			};
			await this.SetControlMaxLength(dicControls);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void SetEventControl(Control ctr)
	{
		foreach (Control control in ctr.Controls)
		{
			if (control is CheckEdit checkEdit)
			{
				checkEdit.CheckStateChanged += OnContentChanged;
			}
			else if (control is ComboBoxEdit comboBoxEdit)
			{
				comboBoxEdit.SelectedIndexChanged += OnContentChanged;
				comboBoxEdit.SelectedValueChanged += OnContentChanged;
				comboBoxEdit.EditValueChanged += OnContentChanged;
			}
			else if (control is LookUpEdit lookUpEdit)
			{
				lookUpEdit.EditValueChanged += OnContentChanged;
				lookUpEdit.Properties.ShowHeader = false;
				lookUpEdit.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
			}
			else if (control is DateEdit dateEdit)
			{
				dateEdit.EditValueChanged += OnContentChanged;
			}
			else if (control is TextEdit || control is MemoEdit)
			{
				control.TextChanged += OnContentChanged;
			}
			else if (control is SeparatorComboBox)
			{
				((SeparatorComboBox)control).SelectedIndexChanged += OnContentChanged;
			}
			else if (control is RadioButton)
			{
				((RadioButton)control).CheckedChanged += OnContentChanged;
			}
			else if (control is RichEditControl)
			{
				((RichEditControl)control).ModifiedChanged += OnContentChanged;
			}
			else
			{
				SetEventControl(control);
			}
		}
	}

	protected void OnContentChanged(object sender, EventArgs e)
	{
		try
		{
			if ((!(sender is DateEdit dateEdit) || (!(dateEdit.Name == "date_From") && !(dateEdit.Name == "date_To"))) && !(sender is BaseEdit { Name: "memoComment" }) && !_isLoadingData && (!CallFromCal || (CallFromCal && ClgCode != 0) || !CallFromRelated) && FormMode != Enums.FormViewModeType.Add)
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

	private async Task LoadData()
	{
		_ = 2;
		try
		{
			_isLoadingData = true;
			WinCommons._OpenProcessing();
			if (CallFromCal || CallFromRelated)
			{
				if (ClgCode == 0)
				{
					await CalAddNew();
				}
				else
				{
					await CalView();
				}
			}
			else
			{
				gv_Note.BeginUpdate();
				if (!_dtSearch.IsNotNull())
				{
					_dtSearch = new DataTable();
					_dtSearch.Columns.Add("userCode", typeof(string));
					_dtSearch.Columns.Add("startDate", typeof(DateTime));
					_dtSearch.Columns.Add("endDate", typeof(DateTime));
				}
				_dtSearch.Rows.Clear();
				_dtSearch.Rows.Add(GlobalData.UserCodeLogin, (date_From.EditValue == null) ? DBNull.Value : ((object)Function.ParseDateTimes(date_From.EditValue)), (date_To.EditValue == null) ? DBNull.Value : ((object)Function.ParseDateTimes(date_To.EditValue)));
				DataTable dataTable = await _httpClient.SubActivities.LoadNoteByInterval(_dtSearch);
				if (dataTable.IsNotNull())
				{
					dataTable.SetDefaultAndMaxLen(gv_Note.ItemId);
					grd_Note.DataSource = dataTable;
					grd_Note.RefreshDataSource();
					grd_Note.DataSourceTable.AcceptChanges();
					AddNewRow();
				}
				else if (!(grd_Note.DataSource as DataTable).IsNotNull())
				{
					AddNewRow();
				}
				gv_Note.ExpandAllGroups();
				gv_Note.EndUpdate();
				dataTable.PrimaryKey = dataTable.Columns["ClgCode"].ToArray();
			}
			WinCommons._CloseProcessing();
			_isLoadingData = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			WinCommons._CloseProcessing();
			_isLoadingData = false;
		}
	}

	private async Task CalView()
	{
		try
		{
			if (DataRowFromCal != null)
			{
				DataSet dataSet = new DataSet();
				_dtCurrentRow = DataRowFromCal.ToArray().CopyToDataTable();
				dataSet.Tables.Add(_dtCurrentRow);
				await BindControl(dataSet);
			}
			FormMode = Enums.FormViewModeType.View;
			ucComment1.Enabled = true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void PostDataBackToGridSource(DataTable dt)
	{
		try
		{
			if (!dt.IsNotNull())
			{
				return;
			}
			foreach (DataRow item in grd_Note.DataSourceTable.AsEnumerable())
			{
				if (item["ClgCode"].Equals(_dtCurrentRow.Rows[0]["ClgCode"]))
				{
					item.ItemArray = dt.Rows[0].ItemArray;
				}
			}
			grd_Note.DataSourceTable.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task BindControl(DataSet ds)
	{
		_ = 1;
		try
		{
			DataTable dt = ds.Tables[0];
			if (!dt.IsNotNull())
			{
				return;
			}
			DataRow dr = dt.Rows[0];
			dr.AcceptChanges();
			PostDataBackToGridSource(dt);
			Function.ToString(dr["GroupUser"]);
			if (!string.IsNullOrEmpty(Function.ToString(dr["GroupEmployee"])))
			{
				cbUser.EditValue = "Employee";
			}
			else
			{
				cbUser.EditValue = "User";
			}
			txtClgCode.DataBindings.Clear();
			txtClgCode.DataBindings.Add("EditValue", dt, "ClgCode", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
			txtDetailNote.DataBindings.Clear();
			txtDetailNote.DataBindings.Add("EditValue", dt, "Details", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
			bte_DistrRule.SetEditValue(dr);
			ucLinkedDocument1.EditValue = dr["DocNum"];
			ucLinkedDocument1.DocEntry = dr["DocEntry"];
			ucLinkedDocument1.DocType = Function.ParseInt(dr["DocType"]);
			txtBpCode.DataBindings.Clear();
			txtBpCode.DataBindings.Add("EditValue", dt, "CardCode", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
			dStart_Recontact.DataBindings.Clear();
			dStart_Recontact.DataBindings.Add("EditValue", dt, "Recontact", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
			cfl_ProjectWBS.DataBindings.Clear();
			cfl_ProjectWBS.DataBindings.Add("EditValue", dt, "ProjectWBS", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
			cfl_ProjectNew.DataBindings.Clear();
			cfl_ProjectNew.DataBindings.Add("EditValue", dt, "ProjectEntry", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
			cfl_Opportunity.DataBindings.Clear();
			cfl_Opportunity.DataBindings.Add("EditValue", dt, "OprId", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
			cfl_GroupUser.DataBindings.Clear();
			cfl_GroupUser.DataBindings.Add("EditValue", dt, "GroupUser", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
			cfl_GroupEmployee.DataBindings.Clear();
			cfl_GroupEmployee.DataBindings.Add("EditValue", dt, "GroupEmployee", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
			chk_InviteAll.DataBindings.Clear();
			chk_InviteAll.DataBindings.Add("EditValue", dt, "AllUserInvite", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
			txtOwner.DataBindings.Clear();
			txtOwner.DataBindings.Add("EditValue", dt, "Owner", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
			if (Function.ToString(dr["HtmlNotes"]).Contains("w:wordDocument"))
			{
				wysiwygEdit1.richEditor.WordMLText = Function.ToString(dr["HtmlNotes"]);
			}
			else
			{
				wysiwygEdit1.richEditor.HtmlText = Function.ToString(dr["HtmlNotes"]);
			}
			wysiwygEdit1.richEditor.Document.DefaultCharacterProperties.FontSize = 11f;
			int absEntry = Function.ParseInt(dt.Rows[0]["AtcEntry"]);
			await ucAttachment1.LoadAttachments(absEntry);
			await ucComment1.RefreshComment(Function.ParseInt(dt.Rows[0]["ClgCode"]));
			lblCreatedBy.Text = "";
			int key = Function.ParseInt(dr["AssignedBy"]);
			string text = Function.ToString(dr["CntctTime"]);
			DateTime dateTime = Function.ParseDateTimes(dr["CntctDate"]);
			string s = ((text.Length < 2) ? Function.ToString(dateTime.Date.Add(new TimeSpan(0, Function.ParseInt(text), 0))) : Function.ToString(dateTime.Date.Add(new TimeSpan(Function.ParseInt(text) / 100, Function.ParseInt(text.Substring(text.Length - 2, 2)), 0))));
			int num = Function.ParseInt(dr["ClgCode"]);
			if (_dictionUser.ContainsKey(key))
			{
				lblCreatedBy.Utext = "Created by: " + _dictionUser[key] + " (" + WinFuntions.FormatCreatedDate(s, shorten: true) + "). ID: " + num;
			}
			else
			{
				lblCreatedBy.Utext = "Created by:";
			}
			foreach (Control control in layout_UDF.Controls)
			{
				if (control is BaseEdit && dr.Table.Columns.Contains(control.Name))
				{
					control.DataBindings.Clear();
					control.DataBindings.Add("EditValue", dt, control.Name, formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected override void NoAuthSetControlSearch()
	{
		ucAttachment1.btDisplay.Enabled = true;
		btnSearch.Enabled = true;
		DateEditEx dateEditEx = date_To;
		bool readOnly = (date_From.ReadOnly = false);
		dateEditEx.ReadOnly = readOnly;
	}

	public override void CHANGELOG()
	{
		try
		{
			string text = "";
			if (FormMode == Enums.FormViewModeType.Add)
			{
				WinCommons.Error(Enums.MessageCode.NoMatchingRecordFound, "");
			}
			if (CallFromCal || CallFromRelated)
			{
				text = Function.ToString(DataRowFromCal["ClgCode"]);
				Enums.ChangeLog changeLogType = ChangeLogType;
				GetChangeLogParams getChangeLogParams = new GetChangeLogParams();
				getChangeLogParams.PrimaryKey = text;
				getChangeLogParams.Object = changeLogType;
				getChangeLogParams.IsApzTable = true;
				frm_ChangeLogList frm_ChangeLogList2 = new frm_ChangeLogList();
				frm_ChangeLogList2.GetChangeLogParams = getChangeLogParams;
				frm_ChangeLogList2.MdiParent = MdiParent;
				frm_ChangeLogList2.Show();
			}
			else
			{
				text = Function.ToString(gv_Note.GetFocusedRowCellValue("ClgCode"));
				Enums.ChangeLog changeLogType2 = ChangeLogType;
				GetChangeLogParams getChangeLogParams2 = new GetChangeLogParams();
				getChangeLogParams2.PrimaryKey = text;
				getChangeLogParams2.Object = changeLogType2;
				getChangeLogParams2.IsApzTable = true;
				frm_ChangeLogList frm_ChangeLogList3 = new frm_ChangeLogList();
				frm_ChangeLogList3.GetChangeLogParams = getChangeLogParams2;
				frm_ChangeLogList3.MdiParent = MdiParent;
				frm_ChangeLogList3.Show();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Administrator.frm_NoteManagement));
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions4 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions5 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions6 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions7 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled10 = new DevExpress.Utils.SerializableAppearanceObject();
		this.lk_AttendUser = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.btnSearch = new Apzon.Controls.Controls.SimpleButtonEx();
		this.date_To = new Apzon.Controls.Editor.DateEditEx();
		this.lbl_ValidTo = new Apzon.Controls.Controls.LabelLine();
		this.date_From = new Apzon.Controls.Editor.DateEditEx();
		this.lbl_ValidFrom = new Apzon.Controls.Controls.LabelLine();
		this.grd_Note = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip_Note = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.toolStripMenuItem_Duplicate = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripMenuItem_Delete = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Note = new Apzon.Controls.Grid.GridViewEx();
		this.cl_ClgCode = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_Details = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_AssignTo = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemHourEdit1 = new Apzon.Controls.Controls.RepositoryItemHourEdit();
		this.splitContainerControl2 = new DevExpress.XtraEditors.SplitContainerControl();
		this.wysiwygEdit1 = new Apzon.Forms.UserControls.WysiwygEdit();
		this.cfl_GroupUser = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.txtOwner = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.xtraTabControl1 = new Apzon.Controls.Controls.XtraTabControlEx();
		this.tab_Attachment = new Apzon.Controls.Controls.XtraTabPageEx();
		this.ucAttachment1 = new Apzon.Forms.UserControls.ucAttachment();
		this.tab_Comment = new Apzon.Controls.Controls.XtraTabPageEx();
		this.ucComment1 = new Apzon.Forms.UserControls.ucComment();
		this.xtraTabPageEx1 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.layout_UDF = new Apzon.Forms.UserControls.LayoutUDF();
		this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
		this.lb_BPCode = new Apzon.Controls.Controls.LabelLine();
		this.lbOwner = new Apzon.Controls.Controls.LabelLine();
		this.lb_AssignedTo = new Apzon.Controls.Controls.LabelLine();
		this.cbUser = new Apzon.Controls.Controls.ComboBoxEditEx();
		this.cfl_Opportunity = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.lb_StartTime = new Apzon.Controls.Controls.LabelLine();
		this.labelLine6 = new Apzon.Controls.Controls.LabelLine();
		this.dStart_Recontact = new Apzon.Controls.Editor.DateEditEx();
		this.txtBpCode = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.chk_InviteAll = new Apzon.Controls.Editor.CheckEditEx();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.cfl_ProjectNew = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.ucLinkedDocument1 = new Apzon.Forms.UserControls.ucLinkedDocument();
		this.cfl_ProjectWBS = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.cfl_GroupEmployee = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine5 = new Apzon.Controls.Controls.LabelLine();
		this.bte_DistrRule = new Apzon.Forms.UserControls.ButtonEditDistrRule();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.btn_PushNotify = new Apzon.Controls.Controls.SimpleButtonEx();
		this.lblCreatedBy = new Apzon.Controls.Controls.LabelLine();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.txtClgCode = new Apzon.Controls.Editor.TextEditEx();
		this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
		this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
		this.txtDetailNote = new Apzon.Controls.Editor.TextEditEx();
		this.contextMenuStrip_Comment = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.toolStripMenuItem_DeleteCM = new System.Windows.Forms.ToolStripMenuItem();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.removeNoteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		((System.ComponentModel.ISupportInitialize)this.lk_AttendUser).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.date_To.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_To.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_From.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_From.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Note).BeginInit();
		this.contextMenuStrip_Note.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Note).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemHourEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl2).BeginInit();
		this.splitContainerControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.cfl_GroupUser.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtOwner.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).BeginInit();
		this.xtraTabControl1.SuspendLayout();
		this.tab_Attachment.SuspendLayout();
		this.tab_Comment.SuspendLayout();
		this.xtraTabPageEx1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.layout_UDF).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.Root).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbUser.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cfl_Opportunity.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dStart_Recontact.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dStart_Recontact.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtBpCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_InviteAll.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cfl_ProjectNew.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cfl_ProjectWBS.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cfl_GroupEmployee.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bte_DistrRule.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtClgCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.barManager1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtDetailNote.Properties).BeginInit();
		this.contextMenuStrip_Comment.SuspendLayout();
		this.contextMenuStrip1.SuspendLayout();
		base.SuspendLayout();
		this.lk_AttendUser.AutoHeight = false;
		this.lk_AttendUser.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.lk_AttendUser.DisplayMember = "USERNAME";
		this.lk_AttendUser.Name = "lk_AttendUser";
		this.lk_AttendUser.ValueMember = "USERID";
		this.splitContainerControl1.CollapsePanel = DevExpress.XtraEditors.SplitCollapsePanel.Panel2;
		this.splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainerControl1.Location = new System.Drawing.Point(0, 0);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.btnSearch);
		this.splitContainerControl1.Panel1.Controls.Add(this.date_To);
		this.splitContainerControl1.Panel1.Controls.Add(this.lbl_ValidTo);
		this.splitContainerControl1.Panel1.Controls.Add(this.date_From);
		this.splitContainerControl1.Panel1.Controls.Add(this.lbl_ValidFrom);
		this.splitContainerControl1.Panel1.Controls.Add(this.grd_Note);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.splitContainerControl2);
		this.splitContainerControl1.Panel2.Controls.Add(this.btn_PushNotify);
		this.splitContainerControl1.Panel2.Controls.Add(this.lblCreatedBy);
		this.splitContainerControl1.Panel2.Controls.Add(this.btnCancel);
		this.splitContainerControl1.Panel2.Controls.Add(this.btn_Save);
		this.splitContainerControl1.Panel2.Controls.Add(this.txtClgCode);
		this.splitContainerControl1.Panel2.Controls.Add(this.txtDetailNote);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(1100, 665);
		this.splitContainerControl1.SplitterPosition = 408;
		this.splitContainerControl1.TabIndex = 0;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.splitContainerControl1.SplitGroupPanelCollapsing += new DevExpress.XtraEditors.SplitGroupPanelCollapsingEventHandler(this.splitContainerControl1_SplitGroupPanelCollapsing);
		this.btnSearch.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btnSearch.Appearance.Options.UseBackColor = true;
		this.btnSearch.EditControl = null;
		this.btnSearch.EditValue = "Search";
		this.btnSearch.FieldName = null;
		this.btnSearch.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnSearch.ImageOptions.Image");
		this.btnSearch.IsSearchControl = true;
		this.btnSearch.Location = new System.Drawing.Point(327, 3);
		this.btnSearch.Name = "btnSearch";
		this.btnSearch.ReadOnly = false;
		this.btnSearch.SettingInVisible = false;
		this.btnSearch.SettingReadonly = false;
		this.btnSearch.Size = new System.Drawing.Size(79, 20);
		this.btnSearch.TabIndex = 3;
		this.btnSearch.Text = "Search";
		this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
		this.date_To.EditControl = null;
		this.date_To.EditValue = null;
		this.date_To.FieldName = null;
		this.date_To.Location = new System.Drawing.Point(216, 3);
		this.date_To.Name = "date_To";
		this.date_To.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_To.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_To.Properties.IsSearchControl = true;
		this.date_To.SettingInVisible = false;
		this.date_To.SettingReadonly = false;
		this.date_To.Size = new System.Drawing.Size(100, 20);
		this.date_To.TabIndex = 2;
		this.lbl_ValidTo.EditControl = null;
		this.lbl_ValidTo.EditValue = "To";
		this.lbl_ValidTo.FieldName = null;
		this.lbl_ValidTo.HideWhenEmpty = false;
		this.lbl_ValidTo.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_ValidTo.Location = new System.Drawing.Point(175, 6);
		this.lbl_ValidTo.Name = "lbl_ValidTo";
		this.lbl_ValidTo.ReadOnly = false;
		this.lbl_ValidTo.SettingInVisible = false;
		this.lbl_ValidTo.SettingReadonly = false;
		this.lbl_ValidTo.ShowUnderLine = true;
		this.lbl_ValidTo.Size = new System.Drawing.Size(42, 20);
		this.lbl_ValidTo.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_ValidTo.TabIndex = 96;
		this.lbl_ValidTo.Utext = "To";
		this.date_From.EditControl = null;
		this.date_From.EditValue = null;
		this.date_From.FieldName = null;
		this.date_From.Location = new System.Drawing.Point(52, 3);
		this.date_From.Name = "date_From";
		this.date_From.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_From.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_From.Properties.IsSearchControl = true;
		this.date_From.SettingInVisible = false;
		this.date_From.SettingReadonly = false;
		this.date_From.Size = new System.Drawing.Size(100, 20);
		this.date_From.TabIndex = 1;
		this.lbl_ValidFrom.AllowDrop = true;
		this.lbl_ValidFrom.EditControl = null;
		this.lbl_ValidFrom.EditValue = "From";
		this.lbl_ValidFrom.FieldName = null;
		this.lbl_ValidFrom.HideWhenEmpty = false;
		this.lbl_ValidFrom.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_ValidFrom.Location = new System.Drawing.Point(3, 6);
		this.lbl_ValidFrom.Name = "lbl_ValidFrom";
		this.lbl_ValidFrom.ReadOnly = false;
		this.lbl_ValidFrom.SettingInVisible = false;
		this.lbl_ValidFrom.SettingReadonly = false;
		this.lbl_ValidFrom.ShowUnderLine = true;
		this.lbl_ValidFrom.Size = new System.Drawing.Size(52, 20);
		this.lbl_ValidFrom.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_ValidFrom.TabIndex = 94;
		this.lbl_ValidFrom.Utext = "From";
		this.grd_Note.AllowPasteExcel = true;
		this.grd_Note.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Note.ClipboardData = null;
		this.grd_Note.ContextMenuStrip = this.contextMenuStrip_Note;
		this.grd_Note.Location = new System.Drawing.Point(0, 28);
		this.grd_Note.MainView = this.gv_Note;
		this.grd_Note.Name = "grd_Note";
		this.grd_Note.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.repositoryItemHourEdit1 });
		this.grd_Note.Size = new System.Drawing.Size(406, 637);
		this.grd_Note.TabIndex = 4;
		this.grd_Note.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Note });
		this.grd_Note.ProcessGridKey += new System.Windows.Forms.KeyEventHandler(this.grd_Note_ProcessGridKey);
		this.contextMenuStrip_Note.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.toolStripMenuItem_Duplicate, this.toolStripMenuItem_Delete });
		this.contextMenuStrip_Note.Name = "contextMenuStrip_Note";
		this.contextMenuStrip_Note.Size = new System.Drawing.Size(125, 48);
		this.contextMenuStrip_Note.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Note_Opening);
		this.toolStripMenuItem_Duplicate.Name = "toolStripMenuItem_Duplicate";
		this.toolStripMenuItem_Duplicate.Size = new System.Drawing.Size(124, 22);
		this.toolStripMenuItem_Duplicate.Text = "Duplicate";
		this.toolStripMenuItem_Duplicate.Click += new System.EventHandler(this.toolStripMenuItem_Duplicate_Click);
		this.toolStripMenuItem_Delete.Name = "toolStripMenuItem_Delete";
		this.toolStripMenuItem_Delete.Size = new System.Drawing.Size(124, 22);
		this.toolStripMenuItem_Delete.Text = "Delete";
		this.toolStripMenuItem_Delete.Click += new System.EventHandler(this.toolStripMenuItem_Delete_Click);
		this.gv_Note.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.gv_Note.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Note.ColumnKey = null;
		this.gv_Note.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[5] { this.cl_ClgCode, this.cl_Details, this.gc_AssignTo, this.gridColumnEx1, this.gridColumnEx2 });
		this.gv_Note.DisableFormSetting = false;
		this.gv_Note.DisplayException = false;
		this.gv_Note.EditColumn = null;
		this.gv_Note.FieldCurrency = null;
		this.gv_Note.FieldRate = null;
		this.gv_Note.FormId = "oNoteManagement";
		this.gv_Note.GridControl = this.grd_Note;
		this.gv_Note.IgnoreFilter = false;
		this.gv_Note.IndexEditRow = -1;
		this.gv_Note.IndicatorWidth = 35;
		this.gv_Note.IsApzTable = false;
		this.gv_Note.IsInsertNewRow = false;
		this.gv_Note.IsLoadingData = false;
		this.gv_Note.IsMappingContextAction = false;
		this.gv_Note.ItemId = "ONOTE";
		this.gv_Note.LstColumnAllowBatch = null;
		this.gv_Note.LstColumnNotAllowBatch = null;
		this.gv_Note.LstColumnNotEditable = null;
		this.gv_Note.LstColumNotSetting = null;
		this.gv_Note.Name = "gv_Note";
		this.gv_Note.NewItemRowText = "New Note";
		this.gv_Note.OptionsBehavior.AutoExpandAllGroups = true;
		this.gv_Note.OptionsView.BestFitMaxRowCount = 300;
		this.gv_Note.OptionsView.RowAutoHeight = true;
		this.gv_Note.OptionsView.ShowGroupPanel = false;
		this.gv_Note.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[2]
		{
			new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.cl_ClgCode, DevExpress.Data.ColumnSortOrder.Descending),
			new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.gridColumnEx1, DevExpress.Data.ColumnSortOrder.Descending)
		});
		this.gv_Note.UseBatchMod = false;
		this.gv_Note.CustomColumnGroup += new DevExpress.XtraGrid.Views.Base.CustomColumnSortEventHandler(this.gv_Note_CustomColumnGroup);
		this.gv_Note.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_Note_ShowingEditor);
		this.gv_Note.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gv_Note_FocusedRowChanged);
		this.gv_Note.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_Note_FocusedRowObjectChanged);
		this.gv_Note.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Note_CellValueChanging);
		this.gv_Note.BeforeLeaveRow += new DevExpress.XtraGrid.Views.Base.RowAllowEventHandler(this.gv_Note_BeforeLeaveRow);
		this.gv_Note.CustomColumnSort += new DevExpress.XtraGrid.Views.Base.CustomColumnSortEventHandler(this.gv_Note_CustomColumnSort);
		this.gv_Note.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gv_Note_CustomUnboundColumnData);
		this.gv_Note.DoubleClick += new System.EventHandler(this.gv_Note_DoubleClick);
		this.cl_ClgCode.Caption = "ClgCode";
		this.cl_ClgCode.CurrencyDateField = null;
		this.cl_ClgCode.CurrencyField = null;
		this.cl_ClgCode.CurrencyFixed = null;
		this.cl_ClgCode.CurrencyFixedRate = 0.0;
		this.cl_ClgCode.CurrencyRateField = null;
		this.cl_ClgCode.FieldName = "ClgCode";
		this.cl_ClgCode.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.cl_ClgCode.Name = "cl_ClgCode";
		this.cl_ClgCode.OptionsColumn.ReadOnly = true;
		this.cl_ClgCode.PrimaryKey = false;
		this.cl_ClgCode.SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
		this.cl_ClgCode.UnitField = null;
		this.cl_ClgCode.Width = 100;
		this.cl_Details.Caption = "Add Note";
		this.cl_Details.CurrencyDateField = null;
		this.cl_Details.CurrencyField = null;
		this.cl_Details.CurrencyFixed = null;
		this.cl_Details.CurrencyFixedRate = 0.0;
		this.cl_Details.CurrencyRateField = null;
		this.cl_Details.FieldName = "Details";
		this.cl_Details.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.cl_Details.Name = "cl_Details";
		this.cl_Details.PrimaryKey = false;
		this.cl_Details.SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
		this.cl_Details.UnitField = null;
		this.cl_Details.Visible = true;
		this.cl_Details.VisibleIndex = 0;
		this.cl_Details.Width = 269;
		this.gc_AssignTo.Caption = "AssignTo";
		this.gc_AssignTo.CurrencyDateField = null;
		this.gc_AssignTo.CurrencyField = null;
		this.gc_AssignTo.CurrencyFixed = null;
		this.gc_AssignTo.CurrencyFixedRate = 0.0;
		this.gc_AssignTo.CurrencyRateField = null;
		this.gc_AssignTo.FieldName = "AssignTo";
		this.gc_AssignTo.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gc_AssignTo.Name = "gc_AssignTo";
		this.gc_AssignTo.PrimaryKey = false;
		this.gc_AssignTo.SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
		this.gc_AssignTo.UnboundType = DevExpress.Data.UnboundColumnType.String;
		this.gc_AssignTo.UnitField = null;
		this.gc_AssignTo.Visible = true;
		this.gc_AssignTo.VisibleIndex = 1;
		this.gc_AssignTo.Width = 95;
		this.gridColumnEx1.Caption = "Note Date";
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gridColumnEx1.FieldName = "Recontact";
		this.gridColumnEx1.GroupInterval = DevExpress.XtraGrid.ColumnGroupInterval.DateRange;
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.OptionsColumn.ReadOnly = true;
		this.gridColumnEx1.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.DateSmart;
		this.gridColumnEx1.PrimaryKey = false;
		this.gridColumnEx1.SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx1.Visible = true;
		this.gridColumnEx1.VisibleIndex = 2;
		this.gridColumnEx2.Caption = "Note Time";
		this.gridColumnEx2.ColumnEdit = this.repositoryItemHourEdit1;
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.FieldName = "BeginTime";
		this.gridColumnEx2.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.UnitField = null;
		this.repositoryItemHourEdit1.AutoHeight = false;
		this.repositoryItemHourEdit1.IsSearchControl = false;
		this.repositoryItemHourEdit1.MaxLength = 5;
		this.repositoryItemHourEdit1.Name = "repositoryItemHourEdit1";
		this.splitContainerControl2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl2.FixedPanel = DevExpress.XtraEditors.SplitFixedPanel.Panel2;
		this.splitContainerControl2.Horizontal = false;
		this.splitContainerControl2.Location = new System.Drawing.Point(3, 27);
		this.splitContainerControl2.Name = "splitContainerControl2";
		this.splitContainerControl2.Panel1.AutoScroll = true;
		this.splitContainerControl2.Panel1.Controls.Add(this.wysiwygEdit1);
		this.splitContainerControl2.Panel1.Text = "Panel1";
		this.splitContainerControl2.Panel2.AutoScroll = true;
		this.splitContainerControl2.Panel2.Controls.Add(this.cfl_GroupUser);
		this.splitContainerControl2.Panel2.Controls.Add(this.txtOwner);
		this.splitContainerControl2.Panel2.Controls.Add(this.xtraTabControl1);
		this.splitContainerControl2.Panel2.Controls.Add(this.lb_BPCode);
		this.splitContainerControl2.Panel2.Controls.Add(this.lbOwner);
		this.splitContainerControl2.Panel2.Controls.Add(this.lb_AssignedTo);
		this.splitContainerControl2.Panel2.Controls.Add(this.cbUser);
		this.splitContainerControl2.Panel2.Controls.Add(this.cfl_Opportunity);
		this.splitContainerControl2.Panel2.Controls.Add(this.lb_StartTime);
		this.splitContainerControl2.Panel2.Controls.Add(this.labelLine6);
		this.splitContainerControl2.Panel2.Controls.Add(this.dStart_Recontact);
		this.splitContainerControl2.Panel2.Controls.Add(this.txtBpCode);
		this.splitContainerControl2.Panel2.Controls.Add(this.chk_InviteAll);
		this.splitContainerControl2.Panel2.Controls.Add(this.labelLine2);
		this.splitContainerControl2.Panel2.Controls.Add(this.labelLine3);
		this.splitContainerControl2.Panel2.Controls.Add(this.cfl_ProjectNew);
		this.splitContainerControl2.Panel2.Controls.Add(this.ucLinkedDocument1);
		this.splitContainerControl2.Panel2.Controls.Add(this.cfl_ProjectWBS);
		this.splitContainerControl2.Panel2.Controls.Add(this.cfl_GroupEmployee);
		this.splitContainerControl2.Panel2.Controls.Add(this.labelLine4);
		this.splitContainerControl2.Panel2.Controls.Add(this.labelLine5);
		this.splitContainerControl2.Panel2.Controls.Add(this.bte_DistrRule);
		this.splitContainerControl2.Panel2.Controls.Add(this.labelLine1);
		this.splitContainerControl2.Panel2.Text = "Panel2";
		this.splitContainerControl2.Size = new System.Drawing.Size(675, 607);
		this.splitContainerControl2.SplitterPosition = 335;
		this.splitContainerControl2.TabIndex = 136;
		this.wysiwygEdit1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.wysiwygEdit1.FieldName = "HtmlNotes";
		this.wysiwygEdit1.Location = new System.Drawing.Point(0, 0);
		this.wysiwygEdit1.Name = "wysiwygEdit1";
		this.wysiwygEdit1.Size = new System.Drawing.Size(675, 262);
		this.wysiwygEdit1.TabIndex = 134;
		this.wysiwygEdit1.Validating += new System.ComponentModel.CancelEventHandler(this.wysiwygEdit1_Validating);
		this.cfl_GroupUser.EditControl = null;
		this.cfl_GroupUser.FieldName = "GroupUser";
		this.cfl_GroupUser.IsSearchControl = false;
		this.cfl_GroupUser.Location = new System.Drawing.Point(144, 5);
		this.cfl_GroupUser.Name = "cfl_GroupUser";
		this.cfl_GroupUser.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions1.Image");
		this.cfl_GroupUser.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, false, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.cfl_GroupUser.Properties.ContainerControl = null;
		this.cfl_GroupUser.Properties.DisplayMember = "";
		this.cfl_GroupUser.Properties.DisplayMember2 = "";
		this.cfl_GroupUser.Properties.LoadDataSourceWhenEnter = false;
		this.cfl_GroupUser.Properties.LookAndFeel.UseDefaultLookAndFeel = false;
		this.cfl_GroupUser.Properties.SeparateCharacterForMulti = ';';
		this.cfl_GroupUser.Properties.SeparateString = ", ";
		this.cfl_GroupUser.Properties.UsingAutoCompleteBox = false;
		this.cfl_GroupUser.Properties.UsingDataSource = true;
		this.cfl_GroupUser.Properties.ValidateWhenNoDataSource = false;
		this.cfl_GroupUser.Properties.ValueMember = "";
		this.cfl_GroupUser.SettingInVisible = false;
		this.cfl_GroupUser.SettingReadonly = false;
		this.cfl_GroupUser.Size = new System.Drawing.Size(178, 20);
		this.cfl_GroupUser.TabIndex = 4;
		this.txtOwner.EditControl = null;
		this.txtOwner.FieldName = "Owner";
		this.txtOwner.IsSearchControl = false;
		this.txtOwner.Location = new System.Drawing.Point(143, 31);
		this.txtOwner.Name = "txtOwner";
		this.txtOwner.Properties.AcceptNullValue.Add("0");
		this.txtOwner.Properties.AcceptNullValue.Add("-1");
		this.txtOwner.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtOwner.Properties.Appearance.Options.UseFont = true;
		this.txtOwner.Properties.AutoHeight = false;
		this.txtOwner.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions2.Image = Apzon.Forms.Properties.Resources.right;
		this.txtOwner.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.txtOwner.Properties.ContainerControl = null;
		this.txtOwner.Properties.DisplayMember = "lastName";
		this.txtOwner.Properties.DisplayMember2 = "firstName";
		this.txtOwner.Properties.LoadDataSourceWhenEnter = false;
		this.txtOwner.Properties.SeparateCharacterForMulti = ';';
		this.txtOwner.Properties.SeparateString = ", ";
		this.txtOwner.Properties.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.Employees;
		this.txtOwner.Properties.TypeDrillDown = Apzon.Commons.ApzObjectTypes.oEmployeesInfo;
		this.txtOwner.Properties.UsingAutoCompleteBox = false;
		this.txtOwner.Properties.UsingDataSource = true;
		this.txtOwner.Properties.ValidateWhenNoDataSource = false;
		this.txtOwner.Properties.ValueMember = "EmpId";
		this.txtOwner.SettingInVisible = false;
		this.txtOwner.SettingReadonly = false;
		this.txtOwner.Size = new System.Drawing.Size(178, 20);
		this.txtOwner.TabIndex = 6;
		this.xtraTabControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.xtraTabControl1.IsSearchControl = false;
		this.xtraTabControl1.Location = new System.Drawing.Point(2, 164);
		this.xtraTabControl1.Name = "xtraTabControl1";
		this.xtraTabControl1.SelectedTabPage = this.tab_Attachment;
		this.xtraTabControl1.Size = new System.Drawing.Size(673, 171);
		this.xtraTabControl1.TabIndex = 13;
		this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[3] { this.tab_Attachment, this.tab_Comment, this.xtraTabPageEx1 });
		this.tab_Attachment.Controls.Add(this.ucAttachment1);
		this.tab_Attachment.EditControl = null;
		this.tab_Attachment.EditValue = "Attachment";
		this.tab_Attachment.FieldName = null;
		this.tab_Attachment.IsSearchControl = false;
		this.tab_Attachment.Name = "tab_Attachment";
		this.tab_Attachment.ReadOnly = false;
		this.tab_Attachment.SettingInVisible = false;
		this.tab_Attachment.SettingReadonly = false;
		this.tab_Attachment.Size = new System.Drawing.Size(671, 146);
		this.tab_Attachment.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Attachment.Text = "Attachment";
		this.ucAttachment1.DataSource = null;
		this.ucAttachment1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.ucAttachment1.FormId = "oNoteManagement";
		this.ucAttachment1.ItemId = "ATC1";
		this.ucAttachment1.Location = new System.Drawing.Point(0, 0);
		this.ucAttachment1.Name = "ucAttachment1";
		this.ucAttachment1.Size = new System.Drawing.Size(671, 146);
		this.ucAttachment1.TabIndex = 3;
		this.tab_Comment.Controls.Add(this.ucComment1);
		this.tab_Comment.EditControl = null;
		this.tab_Comment.EditValue = "Comment";
		this.tab_Comment.FieldName = null;
		this.tab_Comment.IsSearchControl = false;
		this.tab_Comment.Name = "tab_Comment";
		this.tab_Comment.ReadOnly = false;
		this.tab_Comment.SettingInVisible = false;
		this.tab_Comment.SettingReadonly = false;
		this.tab_Comment.Size = new System.Drawing.Size(671, 146);
		this.tab_Comment.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Comment.Text = "Comment";
		this.ucComment1.AdditionNotifyUser = (System.Collections.Generic.List<int>)resources.GetObject("ucComment1.AdditionNotifyUser");
		this.ucComment1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.ucComment1.Location = new System.Drawing.Point(0, 0);
		this.ucComment1.Name = "ucComment1";
		this.ucComment1.ObjectType = Apzon.Commons.ApzObjectTypes.oNone;
		this.ucComment1.Size = new System.Drawing.Size(671, 146);
		this.ucComment1.TabIndex = 11;
		this.xtraTabPageEx1.Controls.Add(this.layout_UDF);
		this.xtraTabPageEx1.EditControl = null;
		this.xtraTabPageEx1.EditValue = "Additional Info.";
		this.xtraTabPageEx1.FieldName = "";
		this.xtraTabPageEx1.IsSearchControl = false;
		this.xtraTabPageEx1.Name = "xtraTabPageEx1";
		this.xtraTabPageEx1.ReadOnly = false;
		this.xtraTabPageEx1.SettingInVisible = false;
		this.xtraTabPageEx1.SettingReadonly = false;
		this.xtraTabPageEx1.Size = new System.Drawing.Size(671, 146);
		this.xtraTabPageEx1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPageEx1.Text = "Additional Info.";
		this.layout_UDF.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layout_UDF.Location = new System.Drawing.Point(0, 0);
		this.layout_UDF.Name = "layout_UDF";
		this.layout_UDF.OptionsView.DrawItemBorders = true;
		this.layout_UDF.Root = this.Root;
		this.layout_UDF.Size = new System.Drawing.Size(671, 146);
		this.layout_UDF.TabIndex = 18;
		this.layout_UDF.Text = "layoutControl1";
		this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.Root.GroupBordersVisible = false;
		this.Root.Name = "Root";
		this.Root.Size = new System.Drawing.Size(671, 146);
		this.Root.TextVisible = false;
		this.lb_BPCode.EditControl = null;
		this.lb_BPCode.EditValue = "BP Code";
		this.lb_BPCode.FieldName = null;
		this.lb_BPCode.HideWhenEmpty = false;
		this.lb_BPCode.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_BPCode.Location = new System.Drawing.Point(2, 62);
		this.lb_BPCode.Name = "lb_BPCode";
		this.lb_BPCode.ReadOnly = false;
		this.lb_BPCode.SettingInVisible = false;
		this.lb_BPCode.SettingReadonly = false;
		this.lb_BPCode.ShowUnderLine = true;
		this.lb_BPCode.Size = new System.Drawing.Size(140, 20);
		this.lb_BPCode.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_BPCode.TabIndex = 84;
		this.lb_BPCode.Utext = "BP Code";
		this.lbOwner.EditControl = null;
		this.lbOwner.EditValue = "Owner";
		this.lbOwner.FieldName = null;
		this.lbOwner.HideWhenEmpty = false;
		this.lbOwner.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbOwner.Location = new System.Drawing.Point(2, 34);
		this.lbOwner.Name = "lbOwner";
		this.lbOwner.ReadOnly = false;
		this.lbOwner.SettingInVisible = false;
		this.lbOwner.SettingReadonly = false;
		this.lbOwner.ShowUnderLine = true;
		this.lbOwner.Size = new System.Drawing.Size(140, 20);
		this.lbOwner.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbOwner.TabIndex = 0;
		this.lbOwner.Utext = "Owner";
		this.lb_AssignedTo.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lb_AssignedTo.Appearance.Options.UseFont = true;
		this.lb_AssignedTo.EditControl = null;
		this.lb_AssignedTo.EditValue = "Assigned To";
		this.lb_AssignedTo.FieldName = null;
		this.lb_AssignedTo.HideWhenEmpty = false;
		this.lb_AssignedTo.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_AssignedTo.Location = new System.Drawing.Point(2, 8);
		this.lb_AssignedTo.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.lb_AssignedTo.Name = "lb_AssignedTo";
		this.lb_AssignedTo.ReadOnly = false;
		this.lb_AssignedTo.SettingInVisible = false;
		this.lb_AssignedTo.SettingReadonly = false;
		this.lb_AssignedTo.ShowUnderLine = true;
		this.lb_AssignedTo.Size = new System.Drawing.Size(65, 19);
		this.lb_AssignedTo.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_AssignedTo.TabIndex = 104;
		this.lb_AssignedTo.Utext = "Assigned To";
		this.cbUser.EditControl = null;
		this.cbUser.EditValue = "User";
		this.cbUser.FieldName = "AssignedTo";
		this.cbUser.IsSearchControl = false;
		this.cbUser.Location = new System.Drawing.Point(67, 5);
		this.cbUser.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.cbUser.Name = "cbUser";
		this.cbUser.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.cbUser.Properties.Appearance.Options.UseFont = true;
		this.cbUser.Properties.AutoHeight = false;
		this.cbUser.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbUser.Properties.Items.AddRange(new object[2] { "User", "Employee" });
		this.cbUser.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.cbUser.SettingInVisible = false;
		this.cbUser.SettingReadonly = false;
		this.cbUser.Size = new System.Drawing.Size(75, 20);
		this.cbUser.TabIndex = 3;
		this.cbUser.SelectedValueChanged += new System.EventHandler(this.cbUser_SelectedValueChanged);
		this.cfl_Opportunity.EditControl = null;
		this.cfl_Opportunity.FieldName = "OprId";
		this.cfl_Opportunity.IsSearchControl = false;
		this.cfl_Opportunity.Location = new System.Drawing.Point(428, 83);
		this.cfl_Opportunity.Name = "cfl_Opportunity";
		this.cfl_Opportunity.Properties.Appearance.BackColor = System.Drawing.Color.White;
		this.cfl_Opportunity.Properties.Appearance.Options.UseBackColor = true;
		this.cfl_Opportunity.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions3.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions4.Image");
		this.cfl_Opportunity.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance4, appearanceHovered4, appearancePressed4, appearanceDisabled4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.cfl_Opportunity.Properties.ContainerControl = null;
		this.cfl_Opportunity.Properties.DisplayMember = "Code";
		this.cfl_Opportunity.Properties.DisplayMember2 = "";
		this.cfl_Opportunity.Properties.LoadDataSourceWhenEnter = false;
		this.cfl_Opportunity.Properties.ReadOnly = true;
		this.cfl_Opportunity.Properties.SeparateCharacterForMulti = ';';
		this.cfl_Opportunity.Properties.SeparateString = ", ";
		this.cfl_Opportunity.Properties.UsingAutoCompleteBox = false;
		this.cfl_Opportunity.Properties.UsingDataSource = true;
		this.cfl_Opportunity.Properties.ValidateWhenNoDataSource = false;
		this.cfl_Opportunity.Properties.ValueMember = "AbsEntry";
		this.cfl_Opportunity.SettingInVisible = false;
		this.cfl_Opportunity.SettingReadonly = false;
		this.cfl_Opportunity.Size = new System.Drawing.Size(179, 20);
		this.cfl_Opportunity.TabIndex = 12;
		this.cfl_Opportunity.TabStop = false;
		this.lb_StartTime.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lb_StartTime.Appearance.Options.UseFont = true;
		this.lb_StartTime.EditControl = null;
		this.lb_StartTime.EditValue = "Note Date";
		this.lb_StartTime.FieldName = null;
		this.lb_StartTime.HideWhenEmpty = false;
		this.lb_StartTime.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_StartTime.Location = new System.Drawing.Point(327, 111);
		this.lb_StartTime.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.lb_StartTime.Name = "lb_StartTime";
		this.lb_StartTime.ReadOnly = false;
		this.lb_StartTime.SettingInVisible = false;
		this.lb_StartTime.SettingReadonly = false;
		this.lb_StartTime.ShowUnderLine = true;
		this.lb_StartTime.Size = new System.Drawing.Size(100, 19);
		this.lb_StartTime.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_StartTime.TabIndex = 111;
		this.lb_StartTime.Utext = "Note Date";
		this.labelLine6.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.labelLine6.Appearance.Options.UseFont = true;
		this.labelLine6.EditControl = null;
		this.labelLine6.EditValue = "Linked Opportunity";
		this.labelLine6.FieldName = null;
		this.labelLine6.HideWhenEmpty = false;
		this.labelLine6.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine6.Location = new System.Drawing.Point(327, 85);
		this.labelLine6.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.labelLine6.Name = "labelLine6";
		this.labelLine6.ReadOnly = false;
		this.labelLine6.SettingInVisible = false;
		this.labelLine6.SettingReadonly = false;
		this.labelLine6.ShowUnderLine = true;
		this.labelLine6.Size = new System.Drawing.Size(100, 19);
		this.labelLine6.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine6.TabIndex = 133;
		this.labelLine6.Utext = "Linked Opportunity";
		this.dStart_Recontact.EditControl = null;
		this.dStart_Recontact.EditValue = null;
		this.dStart_Recontact.Enabled = false;
		this.dStart_Recontact.FieldName = "Recontact";
		this.dStart_Recontact.Location = new System.Drawing.Point(428, 108);
		this.dStart_Recontact.Name = "dStart_Recontact";
		this.dStart_Recontact.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
		this.dStart_Recontact.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.dStart_Recontact.Properties.Appearance.Options.UseFont = true;
		this.dStart_Recontact.Properties.AutoHeight = false;
		this.dStart_Recontact.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dStart_Recontact.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dStart_Recontact.Properties.IsSearchControl = false;
		this.dStart_Recontact.Properties.Mask.EditMask = "";
		this.dStart_Recontact.SettingInVisible = false;
		this.dStart_Recontact.SettingReadonly = false;
		this.dStart_Recontact.Size = new System.Drawing.Size(177, 20);
		this.dStart_Recontact.TabIndex = 13;
		this.txtBpCode.EditControl = null;
		this.txtBpCode.FieldName = "CardCode";
		this.txtBpCode.IsSearchControl = false;
		this.txtBpCode.Location = new System.Drawing.Point(144, 59);
		this.txtBpCode.Name = "txtBpCode";
		this.txtBpCode.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtBpCode.Properties.Appearance.Options.UseFont = true;
		this.txtBpCode.Properties.AutoHeight = false;
		this.txtBpCode.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions4.Image = Apzon.Forms.Properties.Resources.right;
		this.txtBpCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance5, appearanceHovered5, appearancePressed5, appearanceDisabled5, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.txtBpCode.Properties.ContainerControl = null;
		this.txtBpCode.Properties.DisplayMember = "";
		this.txtBpCode.Properties.DisplayMember2 = "";
		this.txtBpCode.Properties.LoadDataSourceWhenEnter = false;
		this.txtBpCode.Properties.SeparateCharacterForMulti = ';';
		this.txtBpCode.Properties.SeparateString = ", ";
		this.txtBpCode.Properties.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.Bp;
		this.txtBpCode.Properties.TypeDrillDown = Apzon.Commons.ApzObjectTypes.oBusinessPartners;
		this.txtBpCode.Properties.UsingAutoCompleteBox = false;
		this.txtBpCode.Properties.UsingDataSource = true;
		this.txtBpCode.Properties.ValidateWhenNoDataSource = false;
		this.txtBpCode.Properties.ValueMember = "CardCode";
		this.txtBpCode.SettingInVisible = false;
		this.txtBpCode.SettingReadonly = false;
		this.txtBpCode.Size = new System.Drawing.Size(178, 20);
		this.txtBpCode.TabIndex = 7;
		this.chk_InviteAll.EditControl = null;
		this.chk_InviteAll.FieldName = "AllUserInvite";
		this.chk_InviteAll.IsSearchControl = false;
		this.chk_InviteAll.Location = new System.Drawing.Point(328, 5);
		this.chk_InviteAll.Name = "chk_InviteAll";
		this.chk_InviteAll.Properties.Caption = "All";
		this.chk_InviteAll.Properties.ValueChecked = "Y";
		this.chk_InviteAll.Properties.ValueUnchecked = "N";
		this.chk_InviteAll.SettingInVisible = false;
		this.chk_InviteAll.SettingReadonly = false;
		this.chk_InviteAll.Size = new System.Drawing.Size(51, 20);
		this.chk_InviteAll.TabIndex = 5;
		this.chk_InviteAll.CheckStateChanged += new System.EventHandler(this.chk_InviteAll_CheckStateChanged);
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "Document Type";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(327, 34);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(100, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 118;
		this.labelLine2.Utext = "Document Type";
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "Document Num";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(327, 62);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(100, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 119;
		this.labelLine3.Utext = "Document Num";
		this.cfl_ProjectNew.EditControl = null;
		this.cfl_ProjectNew.FieldName = "ProjectEntry";
		this.cfl_ProjectNew.IsSearchControl = false;
		this.cfl_ProjectNew.Location = new System.Drawing.Point(144, 85);
		this.cfl_ProjectNew.Name = "cfl_ProjectNew";
		this.cfl_ProjectNew.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions5.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions7.Image");
		this.cfl_ProjectNew.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance6, appearanceHovered6, appearancePressed6, appearanceDisabled6, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions5, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance7, appearanceHovered7, appearancePressed7, appearanceDisabled7, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.cfl_ProjectNew.Properties.ContainerControl = null;
		this.cfl_ProjectNew.Properties.DisplayMember = "Code";
		this.cfl_ProjectNew.Properties.DisplayMember2 = "";
		this.cfl_ProjectNew.Properties.LoadDataSourceWhenEnter = false;
		this.cfl_ProjectNew.Properties.SeparateCharacterForMulti = ';';
		this.cfl_ProjectNew.Properties.SeparateString = ", ";
		this.cfl_ProjectNew.Properties.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.ProjectManager;
		this.cfl_ProjectNew.Properties.TypeDrillDown = Apzon.Commons.ApzObjectTypes.oProjectManager;
		this.cfl_ProjectNew.Properties.UsingAutoCompleteBox = false;
		this.cfl_ProjectNew.Properties.UsingDataSource = true;
		this.cfl_ProjectNew.Properties.ValidateWhenNoDataSource = false;
		this.cfl_ProjectNew.Properties.ValueMember = "AbsEntry";
		this.cfl_ProjectNew.SettingInVisible = false;
		this.cfl_ProjectNew.SettingReadonly = false;
		this.cfl_ProjectNew.Size = new System.Drawing.Size(178, 20);
		this.cfl_ProjectNew.TabIndex = 8;
		this.ucLinkedDocument1.CardCode = null;
		this.ucLinkedDocument1.DocEntry = null;
		this.ucLinkedDocument1.DocEntryFieldName = "DocEntry";
		this.ucLinkedDocument1.DocNum = 0;
		this.ucLinkedDocument1.DocNumFieldName = "DocNum";
		this.ucLinkedDocument1.DocType = -1;
		this.ucLinkedDocument1.DocTypeFieldName = "DocType";
		this.ucLinkedDocument1.EditValue = null;
		this.ucLinkedDocument1.FindCardCode = null;
		this.ucLinkedDocument1.IsLinkDraftFieldName = null;
		this.ucLinkedDocument1.IsLinked = false;
		this.ucLinkedDocument1.Location = new System.Drawing.Point(428, 8);
		this.ucLinkedDocument1.Name = "ucLinkedDocument1";
		this.ucLinkedDocument1.Size = new System.Drawing.Size(179, 77);
		this.ucLinkedDocument1.TabIndex = 11;
		this.ucLinkedDocument1.ValidateInfo = null;
		this.cfl_ProjectWBS.EditControl = null;
		this.cfl_ProjectWBS.FieldName = "ProjectWBS";
		this.cfl_ProjectWBS.IsSearchControl = false;
		this.cfl_ProjectWBS.Location = new System.Drawing.Point(144, 111);
		this.cfl_ProjectWBS.Name = "cfl_ProjectWBS";
		this.cfl_ProjectWBS.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions6.Image = Apzon.Forms.Properties.Resources.right;
		this.cfl_ProjectWBS.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, false, true, editorButtonImageOptions6, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance8, appearanceHovered8, appearancePressed8, appearanceDisabled8, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.cfl_ProjectWBS.Properties.ContainerControl = null;
		this.cfl_ProjectWBS.Properties.DisplayMember = "";
		this.cfl_ProjectWBS.Properties.DisplayMember2 = "";
		this.cfl_ProjectWBS.Properties.LoadDataSourceWhenEnter = false;
		this.cfl_ProjectWBS.Properties.SeparateCharacterForMulti = ';';
		this.cfl_ProjectWBS.Properties.SeparateString = ", ";
		this.cfl_ProjectWBS.Properties.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.Project;
		this.cfl_ProjectWBS.Properties.UsingAutoCompleteBox = false;
		this.cfl_ProjectWBS.Properties.UsingDataSource = true;
		this.cfl_ProjectWBS.Properties.ValidateWhenNoDataSource = false;
		this.cfl_ProjectWBS.Properties.ValueMember = "PrjCode";
		this.cfl_ProjectWBS.SettingInVisible = false;
		this.cfl_ProjectWBS.SettingReadonly = false;
		this.cfl_ProjectWBS.Size = new System.Drawing.Size(178, 20);
		this.cfl_ProjectWBS.TabIndex = 9;
		this.cfl_GroupEmployee.EditControl = null;
		this.cfl_GroupEmployee.FieldName = "GroupEmployee";
		this.cfl_GroupEmployee.IsSearchControl = false;
		this.cfl_GroupEmployee.Location = new System.Drawing.Point(144, 5);
		this.cfl_GroupEmployee.Name = "cfl_GroupEmployee";
		this.cfl_GroupEmployee.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions7.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions9.Image");
		this.cfl_GroupEmployee.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, false, true, editorButtonImageOptions7, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance9, appearanceHovered9, appearancePressed9, appearanceDisabled9, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.cfl_GroupEmployee.Properties.ContainerControl = null;
		this.cfl_GroupEmployee.Properties.DisplayMember = "";
		this.cfl_GroupEmployee.Properties.DisplayMember2 = "";
		this.cfl_GroupEmployee.Properties.LoadDataSourceWhenEnter = false;
		this.cfl_GroupEmployee.Properties.LookAndFeel.UseDefaultLookAndFeel = false;
		this.cfl_GroupEmployee.Properties.SeparateCharacterForMulti = ';';
		this.cfl_GroupEmployee.Properties.SeparateString = ", ";
		this.cfl_GroupEmployee.Properties.UsingAutoCompleteBox = false;
		this.cfl_GroupEmployee.Properties.UsingDataSource = true;
		this.cfl_GroupEmployee.Properties.ValidateWhenNoDataSource = false;
		this.cfl_GroupEmployee.Properties.ValueMember = "";
		this.cfl_GroupEmployee.SettingInVisible = false;
		this.cfl_GroupEmployee.SettingReadonly = false;
		this.cfl_GroupEmployee.Size = new System.Drawing.Size(178, 20);
		this.cfl_GroupEmployee.TabIndex = 122;
		this.cfl_GroupEmployee.Visible = false;
		this.labelLine4.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.labelLine4.Appearance.Options.UseFont = true;
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "WBS";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(2, 114);
		this.labelLine4.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(140, 19);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 129;
		this.labelLine4.Utext = "WBS";
		this.labelLine5.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.labelLine5.Appearance.Options.UseFont = true;
		this.labelLine5.EditControl = null;
		this.labelLine5.EditValue = "Distr. Rule";
		this.labelLine5.FieldName = null;
		this.labelLine5.HideWhenEmpty = false;
		this.labelLine5.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine5.Location = new System.Drawing.Point(2, 140);
		this.labelLine5.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.labelLine5.Name = "labelLine5";
		this.labelLine5.ReadOnly = false;
		this.labelLine5.SettingInVisible = false;
		this.labelLine5.SettingReadonly = false;
		this.labelLine5.ShowUnderLine = true;
		this.labelLine5.Size = new System.Drawing.Size(140, 19);
		this.labelLine5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine5.TabIndex = 127;
		this.labelLine5.Utext = "Distr. Rule";
		this.bte_DistrRule.EditControl = null;
		this.bte_DistrRule.FieldName = "";
		this.bte_DistrRule.IsSearchControl = false;
		this.bte_DistrRule.Location = new System.Drawing.Point(144, 137);
		this.bte_DistrRule.Name = "bte_DistrRule";
		this.bte_DistrRule.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "Search", -1, true, true, false, imageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance10, appearanceHovered10, appearancePressed10, appearanceDisabled10, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.bte_DistrRule.SettingInVisible = false;
		this.bte_DistrRule.SettingReadonly = false;
		this.bte_DistrRule.Size = new System.Drawing.Size(178, 20);
		this.bte_DistrRule.TabIndex = 10;
		this.labelLine1.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.labelLine1.Appearance.Options.UseFont = true;
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Project Code";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(2, 88);
		this.labelLine1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(140, 19);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 128;
		this.labelLine1.Utext = "Project Code";
		this.btn_PushNotify.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_PushNotify.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_PushNotify.Appearance.Options.UseBackColor = true;
		this.btn_PushNotify.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_PushNotify.EditControl = null;
		this.btn_PushNotify.EditValue = "Push Notification";
		this.btn_PushNotify.FieldName = null;
		this.btn_PushNotify.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_PushNotify.ImageOptions.Image");
		this.btn_PushNotify.IsSearchControl = false;
		this.btn_PushNotify.Location = new System.Drawing.Point(171, 642);
		this.btn_PushNotify.Name = "btn_PushNotify";
		this.btn_PushNotify.ReadOnly = false;
		this.btn_PushNotify.SettingInVisible = false;
		this.btn_PushNotify.SettingReadonly = false;
		this.btn_PushNotify.Size = new System.Drawing.Size(127, 20);
		this.btn_PushNotify.TabIndex = 135;
		this.btn_PushNotify.Text = "Push Notification";
		this.btn_PushNotify.Click += new System.EventHandler(this.btn_PushNotify_Click);
		this.lblCreatedBy.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.lblCreatedBy.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Italic);
		this.lblCreatedBy.Appearance.ForeColor = System.Drawing.Color.DarkGray;
		this.lblCreatedBy.Appearance.Options.UseFont = true;
		this.lblCreatedBy.Appearance.Options.UseForeColor = true;
		this.lblCreatedBy.Appearance.Options.UseTextOptions = true;
		this.lblCreatedBy.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
		this.lblCreatedBy.EditControl = null;
		this.lblCreatedBy.EditValue = "Created By:";
		this.lblCreatedBy.FieldName = null;
		this.lblCreatedBy.HideWhenEmpty = false;
		this.lblCreatedBy.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lblCreatedBy.Location = new System.Drawing.Point(304, 643);
		this.lblCreatedBy.Name = "lblCreatedBy";
		this.lblCreatedBy.ReadOnly = false;
		this.lblCreatedBy.SettingInVisible = false;
		this.lblCreatedBy.SettingReadonly = false;
		this.lblCreatedBy.ShowUnderLine = false;
		this.lblCreatedBy.Size = new System.Drawing.Size(295, 17);
		this.lblCreatedBy.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lblCreatedBy.TabIndex = 131;
		this.lblCreatedBy.Utext = "Created By:";
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnCancel.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btnCancel.Appearance.Options.UseBackColor = true;
		this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btnCancel.EditControl = null;
		this.btnCancel.EditValue = "Cancel";
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(72, 642);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(93, 20);
		this.btnCancel.TabIndex = 15;
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
		this.btn_Save.Location = new System.Drawing.Point(6, 642);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.ReadOnly = false;
		this.btn_Save.SettingInVisible = false;
		this.btn_Save.SettingReadonly = false;
		this.btn_Save.Size = new System.Drawing.Size(61, 20);
		this.btn_Save.TabIndex = 14;
		this.btn_Save.Text = "OK";
		this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
		this.txtClgCode.EditControl = null;
		this.txtClgCode.FieldName = null;
		this.txtClgCode.IsSearchControl = false;
		this.txtClgCode.Location = new System.Drawing.Point(64, 112);
		this.txtClgCode.MenuManager = this.barManager1;
		this.txtClgCode.Name = "txtClgCode";
		this.txtClgCode.SettingInVisible = false;
		this.txtClgCode.SettingReadonly = false;
		this.txtClgCode.Size = new System.Drawing.Size(41, 20);
		this.txtClgCode.TabIndex = 117;
		this.txtClgCode.Visible = false;
		this.barManager1.DockControls.Add(this.barDockControlTop);
		this.barManager1.DockControls.Add(this.barDockControlBottom);
		this.barManager1.DockControls.Add(this.barDockControlLeft);
		this.barManager1.DockControls.Add(this.barDockControlRight);
		this.barManager1.Form = this;
		this.barManager1.MaxItemId = 349;
		this.barDockControlTop.CausesValidation = false;
		this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
		this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
		this.barDockControlTop.Manager = this.barManager1;
		this.barDockControlTop.Size = new System.Drawing.Size(1100, 0);
		this.barDockControlBottom.CausesValidation = false;
		this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.barDockControlBottom.Location = new System.Drawing.Point(0, 665);
		this.barDockControlBottom.Manager = this.barManager1;
		this.barDockControlBottom.Size = new System.Drawing.Size(1100, 0);
		this.barDockControlLeft.CausesValidation = false;
		this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
		this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
		this.barDockControlLeft.Manager = this.barManager1;
		this.barDockControlLeft.Size = new System.Drawing.Size(0, 665);
		this.barDockControlRight.CausesValidation = false;
		this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
		this.barDockControlRight.Location = new System.Drawing.Point(1100, 0);
		this.barDockControlRight.Manager = this.barManager1;
		this.barDockControlRight.Size = new System.Drawing.Size(0, 665);
		this.txtDetailNote.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.txtDetailNote.EditControl = null;
		this.txtDetailNote.FieldName = "Details";
		this.txtDetailNote.IsSearchControl = false;
		this.txtDetailNote.Location = new System.Drawing.Point(3, 3);
		this.txtDetailNote.MenuManager = this.barManager1;
		this.txtDetailNote.Name = "txtDetailNote";
		this.txtDetailNote.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.txtDetailNote.Properties.Appearance.Options.UseFont = true;
		this.txtDetailNote.Properties.NullValuePrompt = "Subject...";
		this.txtDetailNote.SettingInVisible = false;
		this.txtDetailNote.SettingReadonly = false;
		this.txtDetailNote.Size = new System.Drawing.Size(676, 20);
		this.txtDetailNote.TabIndex = 1;
		this.txtDetailNote.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.txtDetailNote_EditValueChanging);
		this.contextMenuStrip_Comment.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.toolStripMenuItem_DeleteCM });
		this.contextMenuStrip_Comment.Name = "contextMenuStrip_Comment";
		this.contextMenuStrip_Comment.Size = new System.Drawing.Size(108, 26);
		this.toolStripMenuItem_DeleteCM.Name = "toolStripMenuItem_DeleteCM";
		this.toolStripMenuItem_DeleteCM.Size = new System.Drawing.Size(107, 22);
		this.toolStripMenuItem_DeleteCM.Text = "Delete";
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.removeNoteToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(147, 26);
		this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
		this.removeNoteToolStripMenuItem.Name = "removeNoteToolStripMenuItem";
		this.removeNoteToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
		this.removeNoteToolStripMenuItem.Text = "Remove Note";
		this.removeNoteToolStripMenuItem.Click += new System.EventHandler(this.removeNoteToolStripMenuItem_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1100, 665);
		this.ContextMenuStrip = this.contextMenuStrip1;
		base.Controls.Add(this.splitContainerControl1);
		base.Controls.Add(this.barDockControlLeft);
		base.Controls.Add(this.barDockControlRight);
		base.Controls.Add(this.barDockControlBottom);
		base.Controls.Add(this.barDockControlTop);
		base.FormId = "oNoteManagement";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_NoteManagement";
		base.ShowInTaskbar = false;
		this.Text = "Note Management";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_NoteManagement_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.lk_AttendUser).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.date_To.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_To.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_From.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_From.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Note).EndInit();
		this.contextMenuStrip_Note.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Note).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemHourEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl2).EndInit();
		this.splitContainerControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.cfl_GroupUser.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtOwner.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).EndInit();
		this.xtraTabControl1.ResumeLayout(false);
		this.tab_Attachment.ResumeLayout(false);
		this.tab_Comment.ResumeLayout(false);
		this.xtraTabPageEx1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.layout_UDF).EndInit();
		((System.ComponentModel.ISupportInitialize)this.Root).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbUser.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cfl_Opportunity.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dStart_Recontact.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dStart_Recontact.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtBpCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_InviteAll.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cfl_ProjectNew.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cfl_ProjectWBS.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cfl_GroupEmployee.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bte_DistrRule.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtClgCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.barManager1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtDetailNote.Properties).EndInit();
		this.contextMenuStrip_Comment.ResumeLayout(false);
		this.contextMenuStrip1.ResumeLayout(false);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
