using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
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
using Apzon.Forms.SalesOppotunity;
using Apzon.Forms.SalesOppotunity.SetUp.Activity;
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
using DevExpress.XtraLayout;
using DevExpress.XtraRichEdit;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Compatibility;
using DevExpress.XtraScheduler.Xml;
using DevExpress.XtraTab;

namespace Apzon.Forms.Service;

public class frm_EventManagement : XtraFormEx
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

	private enum ChangeType
	{
		ChangeThis,
		ChangeAll,
		Normal
	}

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private bool _isChanging;

	private bool _isTextChanging;

	private bool _isLoading;

	private bool _forceClose;

	private bool _isCancel;

	public int _linkeDocType = -1;

	public int _linkDocNum = -1;

	private int _weekOfMonth;

	private int _day;

	private int _dayOfMonth;

	private int _month;

	private DataTable _dtCurrentRow;

	private DataTable _dtUnlockUser;

	private DataTable _dtActiveEmployee;

	private DataTable _dtSearch;

	public DataRow DataRowFromCal;

	public bool CallFromCal;

	public bool CallFromRelated;

	public int ClgCode;

	public int RecurIndex;

	private string _recurPat;

	private bool _isDuplicate;

	public AppointmentType AptType;

	public string RecurXml;

	public string OrigRecurXml;

	public int _owner;

	private List<string> _listEmailAddr = new List<string>();

	private Dictionary<int, string> _dictionEmployee = new Dictionary<int, string>();

	private Dictionary<int, string> _dictionUser = new Dictionary<int, string>();

	public Func<Task> ReloadCalendar;

	public Func<DataRow, Task> ReloadRelated;

	public Func<int, Task> ReloadAfterRemove;

	private Enums.FormViewModeType _openFormViewMode = Enums.FormViewModeType.View;

	public Enums.ChangeLog ChangeLogType = Enums.ChangeLog.clEventManagement;

	private IContainer components;

	public SimpleButtonEx btn_Remove;

	private GridControlEx grd_Doc;

	private RepositoryItemLookUpEdit rep_Deparment;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem duplicateRowToolStripMenuItem;

	private SplitContainerControl splitContainerControl1;

	private LookUpEditCombo luk_Meetting;

	private LabelLine label_MettingLocation;

	public SimpleButtonEx btn_Save;

	private GridViewEx gv_Doc;

	private GridColumnEx gridColumnEx1;

	private GridColumnEx gridColumnEx7;

	private LabelLine lb_BPCode;

	private LabelLine lb_EndTime;

	private LabelLine lb_StartTime;

	private LabelLine lb_Invite;

	private TextEditEx txtDetails;

	private LabelLine lb_Remarks;

	private CheckEditEx chkAllday;

	private HourEdit txtEndTime;

	private HourEdit txtBeginTime;

	private DateEditEx dStart_Recontact;

	private DateEditEx dEnDate;

	public SimpleButtonEx btn_Cancel;

	private GridColumnEx gridColumnEx2;

	private GridColumnEx gridColumnEx3;

	private RepositoryItemDateEditEx repositoryItemDateEditEx1;

	private GridColumnEx gridColumnEx4;

	private GridColumnEx gridColumnEx5;

	private RepositoryItemCheckEdit repositoryItemCheckEdit1;

	private GridColumnEx gc_AssignTo;

	private RepositoryItemCheckedComboBoxEdit rep_Invite;

	private RepositoryItemDateEditEx repositoryItemDateEdit1;

	private RepositoryItemButtonEditCFL rep_Bp;

	private ToolStripMenuItem deleteRowToolStripMenuItem;

	private ContextMenuStrip contextMenuStrip2;

	private ToolStripMenuItem removeEventToolStripMenuItem;

	private ToolTipController toolTipController1;

	private ButtonEditCFL txtBpCode;

	private ucLinkedDocument ucLinkedDocument1;

	private LabelLine lb_linkedDoc;

	private LabelLine lb_DocumentNumber;

	private LabelLine lb_DocumentType;

	private ButtonEditDistrRule bte_DistrRule;

	private LabelLine labelLine2;

	private LabelLine labelLine1;

	private ComboBoxEditEx cbUser;

	public SimpleButtonEx btn_SendMail;

	private ButtonEditCFL cfl_GroupUser;

	private ButtonEditCFL cfl_GroupEmployee;

	private ButtonEditCFL cfl_ProjectNew;

	private ButtonEditCFL cfl_ProjectWBS;

	private LabelLine labelLine4;

	private LabelLine lblCreatedBy;

	private DateEditEx date_From;

	private LabelLine lbl_ValidFrom;

	private DateEditEx date_To;

	private LabelLine lbl_ValidTo;

	public SimpleButtonEx btnSearch;

	private XtraTabControlEx xtraTabControl1;

	private XtraTabPageEx tab_Att;

	private ucAttachment ucAttachment1;

	private XtraTabPageEx tab_Comment;

	private ucComment ucComment1;

	private XtraTabPageEx tab_AddInfo;

	private LayoutUDF layout_UDF;

	private XtraTabPageEx tab_Recur;

	private Panel panel_Range;

	private LabelLine lb_End;

	private LabelLine lb_Start;

	private LabelLine lb_Occurrence;

	private TextEditEx txtAfterMaxOccur;

	private RadioButton radio_By;

	private RadioButton radio_After;

	private RadioButton radio_NoEndDate;

	private LabelLine lb_Range;

	private LabelLine labelLine32;

	private DateEditEx dSeStartDat;

	private DateEditEx txtBySeEndDat;

	private Panel panel_Daily;

	private CheckEditEx chkSun;

	private ComboBoxEditEx cbMonth;

	private ComboBoxEditEx cbMonth1;

	private ComboBoxEditEx cbDayOfWeek;

	private ComboBoxEditEx cbWeek;

	private CheckEditEx chkSat;

	private TextEditEx txtDayInMonth;

	private RadioButton radio_SubOption2;

	private Label label_RepeatEvery;

	private CheckEditEx chkFri;

	private TextEditEx txtInterval;

	private RadioButton radio_SubOption1;

	private LabelLine lb_Of;

	private LabelLine lb_RepeatOn;

	private CheckEditEx chkThu;

	private LabelLine lb_RepeatEvery;

	private CheckEditEx chkTue;

	private CheckEditEx chkWed;

	private CheckEditEx chkMon;

	private LookUpEditCombo cbRecurPat;

	private ButtonEditCFL cfl_UserSharing;

	private ButtonEditCFL cfl_EmplSharing;

	private LabelLine lbl_Share;

	private CheckEditEx chk_ShareAll;

	private CheckEditEx chk_InviteAll;

	private SplitContainerControl splitContainerControl2;

	private CheckEditEx chk_ShareAllEmp;

	private CheckEditEx chk_InviteAllEmp;

	private GridColumnEx gridColumnEx6;

	private CheckEditEx chkReminder;

	private LookUpEditCombo luk_RemType;

	private CalcEditEx calc_RemQty;

	private TextEditEx txt_BPName;

	private TextEditEx txt_BPType;

	private ButtonEditCFL cfl_Opportunity;

	private LabelLine labelLine3;

	private GridColumnEx gridColumnEx8;

	private GridColumnEx gridColumnEx9;

	private GridColumnEx gridColumnEx10;

	private RepositoryItemHourEdit repositoryItemHourEdit1;

	private LayoutControlGroup Root;

	private WysiwygEdit wysiwygEdit1;

	private ButtonEditCFL txtOwner;

	private LabelLine lbOwner;

	private SplitContainerControl splitContainerControl3;

	private ContextActionMappingImpl[] MappingDocument { get; set; }

	private DataTable OpenFormDocument { get; set; }

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
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
		default:
			btn_Save.Text = Apzon.Commons.Properties.Resources.ButtonOK;
			break;
		}
	}

	public frm_EventManagement()
	{
		Initialize();
	}

	public frm_EventManagement(Enums.FormViewModeType openMode, ContextActionMappingImpl[] dataAction, DataTable dtMapping)
	{
		Initialize();
		_openFormViewMode = openMode;
		MappingDocument = dataAction;
		OpenFormDocument = dtMapping;
	}

	public void Initialize()
	{
		InitializeComponent();
		IsSetSuperTip = true;
		IsScrollable = true;
		FormId = "oEventManagement";
		gv_Doc.ItemId = "APZ_OCLG";
		gv_Doc.FormId = FormId;
		OnLoadAsync += frm_EventManagement_Load;
		splitContainerControl1.Collapsed = true;
		gv_Doc.OptionsView.ColumnAutoWidth = true;
		ucComment1.BeforeSendComment += UcComment1_BeforeSendComment;
	}

	private void UcComment1_BeforeSendComment(object sender, EventArgs e)
	{
		try
		{
			if (!_dtCurrentRow.EmptyData())
			{
				DataRow dataRow = _dtCurrentRow.Rows[0];
				List<int> collection = Function.ToString(dataRow["GroupUser"]).Split(new char[1] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList()
					.ConvertAll((string user) => Function.ParseInt(user));
				ucComment1.AdditionNotifyUser.AddRange(collection);
				int item = Function.ParseInt(dataRow["AssignedBy"]);
				ucComment1.AdditionNotifyUser.Add(item);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task frm_EventManagement_Load(object sender, EventArgs e)
	{
		_ = 3;
		try
		{
			if (CallFromCal || CallFromRelated)
			{
				splitContainerControl1.CollapsePanel = SplitCollapsePanel.Panel1;
				AcceptButton = btn_Save;
				if (ClgCode == 0)
				{
					FormMode = Enums.FormViewModeType.Add;
					ucComment1.Enabled = false;
					btn_SendMail.Enabled = false;
				}
				btn_Remove.Visible = false;
				string text = "";
				if (DataRowFromCal.IsExistsColumn("Details"))
				{
					text = Function.ToString(DataRowFromCal["Details"]);
				}
				if (string.IsNullOrEmpty(text.Trim()))
				{
					Text = "Event - ...";
				}
				else
				{
					string[] array = text.Trim().Split(' ');
					if (array.Length >= 4)
					{
						Text = "Event - " + array[0] + " " + array[1] + " " + array[2] + " " + array[3];
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
						Text = "Event - " + text;
						if (Text.Length > 30)
						{
							Text = "Event - " + Text.Substring(0, 30);
						}
					}
				}
			}
			else
			{
				splitContainerControl1.CollapsePanel = SplitCollapsePanel.Panel2;
				AcceptButton = null;
				btn_Remove.Visible = true;
				ucComment1.Enabled = true;
				ucAttachment1.Enabled = true;
			}
			await LoadUDF();
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
			if (CallFromCal || CallFromRelated)
			{
				txtDetails.Focus();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadUDF()
	{
		try
		{
			await WinFuntions.LoadUdfColumns(gv_Doc, "APZ_OCLG");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void frm_EventManagement_FormClosing(object sender, FormClosingEventArgs e)
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

	private async void btn_Save_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.View)
			{
				btn_Save.Enabled = false;
				btn_Cancel.Enabled = false;
				await SaveData();
				btn_Save.Enabled = true;
				btn_Cancel.Enabled = true;
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

	private void btn_Cancel_Click(object sender, EventArgs e)
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
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Remove_Click(object sender, EventArgs e)
	{
		try
		{
			RemoveProcess();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
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

	private void removeEventToolStripMenuItem_Click(object sender, EventArgs e)
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

	private async Task RemoveProcess()
	{
		_ = 4;
		try
		{
			if (CallFromCal || CallFromRelated)
			{
				if (!CallFromRelated && _recurPat != "N")
				{
					frm_ConfirmDialog frm_ConfirmDialog2 = new frm_ConfirmDialog(2)
					{
						EventSubject = Function.ToString(_dtCurrentRow.Rows[0]["Details"]),
						StartPosition = FormStartPosition.CenterScreen
					};
					if (frm_ConfirmDialog2.ShowDialog() == DialogResult.OK)
					{
						switch (frm_ConfirmDialog2.GetValue())
						{
						case 1:
							await RemoveOccur();
							break;
						case 3:
							await RemoveEvent(2);
							break;
						default:
							await RemoveEvent(1);
							break;
						}
					}
				}
				else if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDeleteThisRow, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					await RemoveEvent(1);
				}
			}
			else if ((grd_Doc.DataSource as DataTable).IsNotNull() && Function.ParseInt(gv_Doc.GetFocusedRowCellValue("ClgCode")) != 0 && gv_Doc.FocusedRowHandle >= 0)
			{
				DialogResult dialogResult = ((!(_recurPat == "N")) ? XtraMessageBoxEx.Show(Enums.MessageCode.EventManagementConfirmDeleteWithRecurrences, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) : XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDeleteThisRow, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question));
				if (dialogResult == DialogResult.Yes)
				{
					await RemoveEvent(1);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task RemoveEvent(int type)
	{
		_ = 4;
		try
		{
			if (!_dtCurrentRow.IsNotNull())
			{
				return;
			}
			WinCommons._OpenProcessing();
			DataTable dt = grd_Doc.DataSource as DataTable;
			int code = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("ClgCode"));
			string text = ((type != 1) ? (await _httpClient.SubActivities.RemoveUnchangedEvent(_dtCurrentRow)) : (await _httpClient.SubActivities.RemoveAllEvent(_dtCurrentRow)));
			if (string.IsNullOrEmpty(text))
			{
				if (dt.IsNotNull())
				{
					int dataSourceIndex = dt.Rows.IndexOf(dt.Rows.Find(code));
					int rowHandle = gv_Doc.GetRowHandle(dataSourceIndex);
					if (rowHandle > 0)
					{
						gv_Doc.DeleteRow(rowHandle);
					}
					dt.AcceptChanges();
					grd_Doc.RefreshDataSource();
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

	private async Task RemoveOccur()
	{
		try
		{
			SaveData(AppointmentType.DeletedOccurrence);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
	{
		try
		{
			e.Cancel = gv_Doc.FocusedRowHandle < 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void contextMenuStrip2_Opening(object sender, CancelEventArgs e)
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

	private void gv_Doc_DoubleClick(object sender, EventArgs e)
	{
		try
		{
			if (grd_Doc.DataSourceTable.HasColumn())
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

	private async void grd_Doc_ProcessGridKey(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.Return)
			{
				gv_Doc.PostEditor();
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

	private void gv_Doc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (_isChanging)
			{
				return;
			}
			_isTextChanging = true;
			if (e.Column.FieldName == "CardCode")
			{
				txtBpCode.EditValue = Function.ToString(e.Value);
			}
			if (e.Column.FieldName == "Recontact")
			{
				dStart_Recontact.EditValue = e.Value;
				gv_Doc.SetRowCellValue(e.RowHandle, "endDate", e.Value);
			}
			if (e.Column.FieldName == "endDate")
			{
				dEnDate.EditValue = e.Value;
			}
			if (e.Column.FieldName == "AllDayEvent")
			{
				chkAllday.EditValue = e.Value;
				if (!Function.ParseBool(e.Value))
				{
					if (string.IsNullOrEmpty(Function.ToString(txtBeginTime.EditValue)))
					{
						txtBeginTime.EditValue = 1000;
					}
					if (string.IsNullOrEmpty(Function.ToString(txtEndTime.EditValue)))
					{
						txtEndTime.EditValue = (string.IsNullOrEmpty(Function.ToString(txtBeginTime.EditValue)) ? 1100 : (Function.ParseInt(txtBeginTime.EditValue) + 100));
					}
					txtBeginTime.Visible = true;
					txtEndTime.Visible = true;
				}
				else
				{
					txtBeginTime.Visible = false;
					txtEndTime.Visible = false;
					txtBeginTime.EditValue = 0;
					txtEndTime.EditValue = 100;
				}
			}
			_isTextChanging = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void gv_Doc_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			if (e.FocusedRowHandle < 0)
			{
				splitContainerControl1.Panel2.Controls.EnableDisableControl(isEnable: false);
				return;
			}
			if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_EventManagement" && x.authoriza == "R"))
			{
				splitContainerControl1.Panel2.Controls.EnableDisableControl(isEnable: false);
			}
			else
			{
				splitContainerControl1.Panel2.Controls.EnableDisableControl(isEnable: true);
			}
			_isLoading = true;
			grd_Doc.DataSourceTable.AcceptChanges();
			string currentClgCode = Function.ToString(gv_Doc.GetFocusedRowCellValue("ClgCode"));
			DataTable dataTable = grd_Doc.DataSource as DataTable;
			if (dataTable.IsNotNull())
			{
				DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["ClgCode"]) == currentClgCode);
				if (dataRow != null)
				{
					DataSet dataSet = new DataSet();
					_dtCurrentRow = dataRow.ToArray().CopyToDataTable();
					_recurPat = Function.ToString(dataRow["RecurPat"]);
					dataSet.Tables.Add(_dtCurrentRow);
					await BindControl(dataSet);
				}
			}
			if (Function.ParseInt(currentClgCode) != 0)
			{
				FormMode = (_isDuplicate ? Enums.FormViewModeType.Add : Enums.FormViewModeType.View);
				_isDuplicate = false;
				if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_EventManagement" && x.authoriza == "R"))
				{
					ucAttachment1.EnableButtons(isEnabled: false);
					btn_SendMail.Enabled = false;
				}
				else
				{
					ucAttachment1.EnableButtons(isEnabled: true);
					btn_SendMail.Enabled = true;
				}
				ucComment1.Enabled = true;
			}
			else
			{
				FormMode = Enums.FormViewModeType.Add;
				ucComment1.Enabled = false;
				btn_SendMail.Enabled = false;
				foreach (Control control in layout_UDF.Controls)
				{
					if (control is BaseEdit)
					{
						(control as BaseEdit).EditValue = control.AccessibleDescription;
					}
				}
			}
			if (Function.ToString(gv_Doc.GetFocusedRowCellValue("OwnerAuth")).Equals("F"))
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
			_isLoading = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			_isLoading = false;
		}
	}

	private async void gv_Doc_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
	{
	}

	private void gv_Doc_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			ColumnView columnView = (ColumnView)sender;
			if (Function.ParseInt(gv_Doc.GetFocusedRowCellValue("ClgCode")) != 0)
			{
				if (columnView.FocusedColumn.FieldName == "CardCode" || columnView.FocusedColumn.FieldName == "GroupUser")
				{
					if (columnView.FocusedColumn.FieldName == "CardCode")
					{
						((RepositoryItemButtonEditCFL)gv_Doc.FocusedColumn.ColumnEdit).Buttons[0].Visible = false;
						((RepositoryItemButtonEditCFL)gv_Doc.FocusedColumn.ColumnEdit).TabWhenEmpty = false;
					}
					gv_Doc.FocusedColumn.OptionsColumn.ReadOnly = true;
				}
				else
				{
					e.Cancel = true;
				}
			}
			else if (columnView.FocusedColumn.FieldName == "CardCode" || columnView.FocusedColumn.FieldName == "GroupUser")
			{
				if (columnView.FocusedColumn.FieldName == "CardCode")
				{
					((RepositoryItemButtonEditCFL)gv_Doc.FocusedColumn.ColumnEdit).Buttons[0].Visible = true;
					((RepositoryItemButtonEditCFL)gv_Doc.FocusedColumn.ColumnEdit).TabWhenEmpty = true;
				}
				gv_Doc.FocusedColumn.OptionsColumn.ReadOnly = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Doc_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (!_isChanging)
			{
				_isTextChanging = true;
				if (e.Column.FieldName == "Details")
				{
					txtDetails.Text = Function.ToString(e.Value);
				}
				_isTextChanging = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Doc_BeforeLeaveRow(object sender, RowAllowEventArgs e)
	{
		try
		{
			if (!_isChanging && !_isLoading && FormMode == Enums.FormViewModeType.Update)
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
				{
					e.Allow = false;
				}
				else
				{
					grd_Doc.DataSourceTable.RejectChanges();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
	{
		try
		{
			if (!(sender is ColumnView columnView))
			{
				return;
			}
			GridColumnEx gridColumnEx = (GridColumnEx)columnView.FocusedColumn;
			if (gridColumnEx.FieldName == "Recontact")
			{
				if (string.IsNullOrEmpty(Function.ToString(e.Value)))
				{
					e.Valid = false;
					e.ErrorText = "Start Date must have a value!";
				}
			}
			else if (gridColumnEx.FieldName == "endDate")
			{
				DateTime dateTime = Function.ParseDateTimes(gv_Doc.GetFocusedRowCellValue("Recontact"));
				DateTime dateTime2 = Function.ParseDateTimes(e.Value);
				if (string.IsNullOrEmpty(Function.ToString(e.Value)))
				{
					e.Valid = false;
					e.ErrorText = "End Date must have a value!";
				}
				if (dateTime2 < dateTime)
				{
					e.Valid = false;
					e.ErrorText = "Invalid End Date!";
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_InvalidValueException(object sender, InvalidValueExceptionEventArgs e)
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

	private void gv_Doc_CustomColumnSort(object sender, CustomColumnSortEventArgs e)
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

	private void gv_Doc_CustomColumnGroup(object sender, CustomColumnSortEventArgs e)
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

	private void chkAllday_CheckStateChanged(object sender, EventArgs e)
	{
	}

	private void txtBpCode_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isTextChanging)
			{
				_isChanging = true;
				gv_Doc.SetFocusedRowCellValue("CardCode", txtBpCode.EditValue);
				RefreshCardTypeAndCardName();
				_isChanging = false;
				_isTextChanging = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void luk_Metting_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (Function.ParseInt(e.NewValue) == -10000)
			{
				frm_ActivityMeetingLocation frm_ActivityMeetingLocation2 = new frm_ActivityMeetingLocation
				{
					MdiParent = MdiParent
				};
				frm_ActivityMeetingLocation2.RefeshParentLookUpSource = (RefreshLookupType)Delegate.Combine(frm_ActivityMeetingLocation2.RefeshParentLookUpSource, new RefreshLookupType(LoadLookUpMeetting));
				frm_ActivityMeetingLocation2.Show();
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txtDetails_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (!_isTextChanging)
			{
				_isChanging = true;
				gv_Doc.SetFocusedRowCellValue("Details", e.NewValue);
				_isChanging = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void dStart_Recontact_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isTextChanging && DateTime.TryParse(Function.ToString(dStart_Recontact.EditValue), out var _))
			{
				DateTime dateTime = Function.ParseDateTime(dStart_Recontact.EditValue) ?? DateTime.MinValue;
				dSeStartDat.EditValue = dateTime;
				dEnDate.EditValue = dateTime;
				chkMon.Checked = false;
				chkTue.Checked = false;
				chkWed.Checked = false;
				chkThu.Checked = false;
				chkFri.Checked = false;
				chkSat.Checked = false;
				chkSun.Checked = false;
				GetDay(dateTime);
				GetDayOfMonth(dateTime);
				GetMonth(dateTime);
				GetWeekOfMonth(dateTime);
				_isChanging = true;
				gv_Doc.SetFocusedRowCellValue("Recontact", dStart_Recontact.EditValue);
				_isChanging = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void dStart_Recontact_Validating(object sender, CancelEventArgs e)
	{
		try
		{
			if (string.IsNullOrEmpty(Function.ToString(dStart_Recontact.EditValue)))
			{
				dStart_Recontact.ErrorText = "Start Date must have a value";
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void dEnDate_Validating(object sender, CancelEventArgs e)
	{
		try
		{
			if (string.IsNullOrEmpty(Function.ToString(dEnDate.EditValue)))
			{
				dEnDate.ErrorText = "End Date must have a value";
				e.Cancel = true;
			}
			else if (Function.ParseDateTimes(dEnDate.EditValue) < Function.ParseDateTimes(dStart_Recontact.EditValue))
			{
				dEnDate.ErrorText = "Invalid End Date!";
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void dEnDate_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isTextChanging)
			{
				_isChanging = true;
				if (DateTime.TryParse(Function.ToString(dEnDate.EditValue), out var _))
				{
					gv_Doc.SetFocusedRowCellValue("endDate", dEnDate.EditValue);
				}
				_isChanging = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void cbRecurPat_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			string text = Function.ToString(cbRecurPat.EditValue);
			switch (text)
			{
			case "N":
				panel_Daily.Visible = false;
				panel_Range.Visible = false;
				if (chkMon.CheckState == CheckState.Indeterminate)
				{
					chkMon.Checked = false;
				}
				if (chkTue.CheckState == CheckState.Indeterminate)
				{
					chkTue.Checked = false;
				}
				if (chkWed.CheckState == CheckState.Indeterminate)
				{
					chkWed.Checked = false;
				}
				if (chkThu.CheckState == CheckState.Indeterminate)
				{
					chkThu.Checked = false;
				}
				if (chkFri.CheckState == CheckState.Indeterminate)
				{
					chkFri.Checked = false;
				}
				if (chkSat.CheckState == CheckState.Indeterminate)
				{
					chkSat.Checked = false;
				}
				if (chkSun.CheckState == CheckState.Indeterminate)
				{
					chkSun.Checked = false;
				}
				chkMon.Visible = false;
				chkTue.Visible = false;
				chkWed.Visible = false;
				chkThu.Visible = false;
				chkFri.Visible = false;
				chkSat.Visible = false;
				chkSun.Visible = false;
				return;
			case "D":
				label_RepeatEvery.Text = "Day(s)";
				radio_SubOption1.Text = "";
				radio_SubOption2.Text = "Every Weekday";
				lb_RepeatEvery.Size = new Size(123, 18);
				radio_SubOption1.Location = new Point(207, 10);
				radio_SubOption1.Size = new Size(14, 13);
				radio_SubOption2.Location = new Point(207, 31);
				radio_SubOption2.Size = new Size(101, 17);
				radio_SubOption1.Visible = true;
				radio_SubOption2.Visible = true;
				lb_RepeatOn.Visible = false;
				txtDayInMonth.Visible = false;
				cbWeek.Visible = false;
				cbDayOfWeek.Visible = false;
				cbMonth.Visible = false;
				cbMonth1.Visible = false;
				lb_Of.Visible = false;
				chkMon.Visible = false;
				chkTue.Visible = false;
				chkWed.Visible = false;
				chkThu.Visible = false;
				chkFri.Visible = false;
				chkSat.Visible = false;
				chkSun.Visible = false;
				break;
			case "W":
				label_RepeatEvery.Text = "Week(s)";
				radio_SubOption1.Visible = false;
				radio_SubOption2.Visible = false;
				cbMonth.Visible = false;
				cbMonth1.Visible = false;
				txtDayInMonth.Visible = false;
				cbWeek.Visible = false;
				cbDayOfWeek.Visible = false;
				lb_Of.Visible = false;
				lb_RepeatEvery.Size = new Size(148, 18);
				lb_RepeatOn.Size = new Size(148, 18);
				chkMon.Location = new Point(226, 28);
				chkMon.Size = new Size(46, 19);
				chkMon.TabIndex = 33;
				chkTue.Location = new Point(278, 28);
				chkTue.Size = new Size(46, 19);
				chkTue.TabIndex = 34;
				chkWed.Location = new Point(325, 28);
				chkWed.Size = new Size(46, 19);
				chkWed.TabIndex = 35;
				chkThu.Location = new Point(378, 28);
				chkThu.Size = new Size(46, 19);
				chkThu.TabIndex = 36;
				chkFri.Location = new Point(429, 28);
				chkFri.Size = new Size(33, 19);
				chkFri.TabIndex = 37;
				chkSat.Location = new Point(468, 28);
				chkSat.Size = new Size(37, 19);
				chkSat.TabIndex = 38;
				chkSun.Location = new Point(510, 28);
				chkSun.Size = new Size(46, 19);
				chkSun.TabIndex = 39;
				chkMon.Visible = true;
				chkTue.Visible = true;
				chkWed.Visible = true;
				chkThu.Visible = true;
				chkFri.Visible = true;
				chkSat.Visible = true;
				chkSun.Visible = true;
				if (!_isLoading)
				{
					chkMon.CheckState = CheckState.Unchecked;
					chkTue.CheckState = CheckState.Unchecked;
					chkWed.CheckState = CheckState.Unchecked;
					chkThu.CheckState = CheckState.Unchecked;
					chkFri.CheckState = CheckState.Unchecked;
					chkSat.CheckState = CheckState.Unchecked;
					chkSun.CheckState = CheckState.Unchecked;
				}
				cbMonth.Visible = false;
				cbMonth1.Visible = false;
				lb_Of.Visible = false;
				goto default;
			default:
				if (text == "M")
				{
					lb_RepeatEvery.Size = new Size(148, 18);
					label_RepeatEvery.Text = "Month(s)";
					lb_RepeatOn.Size = new Size(123, 18);
					radio_SubOption1.Text = string.Empty;
					radio_SubOption2.Text = string.Empty;
					radio_SubOption1.Text = "Day";
					radio_SubOption1.Visible = true;
					radio_SubOption1.Location = new Point(207, 26);
					radio_SubOption1.Size = new Size(101, 17);
					radio_SubOption2.Location = new Point(207, 48);
					radio_SubOption2.Size = new Size(14, 13);
					txtDayInMonth.Visible = true;
					txtDayInMonth.Location = new Point(253, 24);
					txtDayInMonth.Size = new Size(37, 18);
					radio_SubOption2.Visible = true;
					cbWeek.Visible = true;
					cbDayOfWeek.Visible = true;
					cbMonth.Visible = false;
					cbMonth1.Visible = false;
					lb_Of.Visible = false;
					chkMon.Visible = false;
					chkTue.Visible = false;
					chkWed.Visible = false;
					chkThu.Visible = false;
					chkFri.Visible = false;
					chkSat.Visible = false;
					chkSun.Visible = false;
				}
				if (text == "A")
				{
					label_RepeatEvery.Text = "Year(s)";
					lb_RepeatOn.Size = new Size(123, 18);
					lb_RepeatEvery.Size = new Size(148, 18);
					radio_SubOption1.Text = "";
					radio_SubOption1.Visible = true;
					radio_SubOption2.Text = "";
					radio_SubOption2.Visible = true;
					cbMonth.Location = new Point(253, 24);
					cbMonth.Size = new Size(100, 18);
					txtDayInMonth.Visible = true;
					txtDayInMonth.Location = new Point(359, 24);
					txtDayInMonth.Size = new Size(37, 18);
					cbWeek.Visible = true;
					cbDayOfWeek.Visible = true;
					cbMonth.Visible = true;
					cbMonth1.Visible = true;
					lb_Of.Visible = true;
					chkMon.Visible = false;
					chkTue.Visible = false;
					chkWed.Visible = false;
					chkThu.Visible = false;
					chkFri.Visible = false;
					chkSat.Visible = false;
					chkSun.Visible = false;
				}
				lb_RepeatOn.Visible = true;
				break;
			}
			panel_Daily.Visible = true;
			panel_Range.Visible = true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void radio_SubOption1_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			if (radio_SubOption1.Checked)
			{
				cbWeek.ReadOnly = true;
				cbDayOfWeek.ReadOnly = true;
				cbMonth1.ReadOnly = true;
				txtDayInMonth.ReadOnly = false;
				cbMonth.ReadOnly = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void radio_SubOption2_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			if (radio_SubOption2.Checked)
			{
				txtDayInMonth.ReadOnly = true;
				cbMonth.ReadOnly = true;
				cbWeek.ReadOnly = false;
				cbDayOfWeek.ReadOnly = false;
				cbMonth1.ReadOnly = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void radio_NoEndDate_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			if (radio_NoEndDate.Checked)
			{
				txtAfterMaxOccur.ReadOnly = true;
				txtBySeEndDat.ReadOnly = true;
				txtBySeEndDat.Enabled = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void radio_After_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			if (radio_After.Checked)
			{
				txtAfterMaxOccur.ReadOnly = false;
				txtBySeEndDat.ReadOnly = true;
				txtBySeEndDat.Enabled = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void radio_By_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			if (radio_By.Checked)
			{
				txtAfterMaxOccur.ReadOnly = true;
				txtBySeEndDat.ReadOnly = false;
				txtBySeEndDat.Enabled = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void cbMonth_SelectedValueChanged(object sender, EventArgs e)
	{
		string selectedItem = cbMonth.EditValue.ToString();
		cbMonth1.SelectedItem = selectedItem;
	}

	private void cbMonth1_SelectedValueChanged(object sender, EventArgs e)
	{
		try
		{
			string selectedItem = cbMonth1.EditValue.ToString();
			cbMonth.SelectedItem = selectedItem;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txtInterval_Validating(object sender, CancelEventArgs e)
	{
		try
		{
			if (Function.ParseInt(txtInterval.EditValue) <= 0)
			{
				txtInterval.ErrorText = "The frequency range is not valid!";
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txtAfterMaxOccur_Validating(object sender, CancelEventArgs e)
	{
		try
		{
			if (Function.ParseInt(txtAfterMaxOccur.EditValue) <= 0)
			{
				txtAfterMaxOccur.ErrorText = "The recurrence range is not valid!";
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txtDayInMonth_Validating(object sender, CancelEventArgs e)
	{
		try
		{
			if (Function.ParseInt(txtDayInMonth.EditValue) <= 0)
			{
				txtAfterMaxOccur.ErrorText = "The recurrence frequency is not valid [Repeat Day in Month] ";
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txtBeginTime_Validating(object sender, CancelEventArgs e)
	{
		try
		{
			if (string.IsNullOrEmpty(Function.ToString(txtBeginTime.EditValue)))
			{
				txtBeginTime.ErrorText = "Start Time must have a value";
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txtEndTime_Validating(object sender, CancelEventArgs e)
	{
		try
		{
			if (string.IsNullOrEmpty(Function.ToString(txtEndTime.EditValue)))
			{
				txtEndTime.ErrorText = "End Time must have a value";
				e.Cancel = true;
			}
			else if (Function.ParseInt(txtEndTime.EditValue) < Function.ParseInt(txtBeginTime.EditValue) && Function.ParseDateTimes(dStart_Recontact.EditValue) == Function.ParseDateTimes(dEnDate.EditValue))
			{
				txtEndTime.ErrorText = "Invalid end time";
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txtBySeEndDat_Validating(object sender, CancelEventArgs e)
	{
		try
		{
			if (string.IsNullOrEmpty(Function.ToString(txtBySeEndDat.EditValue)))
			{
				txtBySeEndDat.ErrorText = "By End Date must have a value!";
				e.Cancel = true;
			}
			else if (Function.ParseDateTimes(txtBySeEndDat.EditValue) < Function.ParseDateTimes(dSeStartDat.EditValue))
			{
				txtBySeEndDat.ErrorText = "The recurrence range is not valid!";
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void frm_EventManagement_Shown(object sender, EventArgs e)
	{
	}

	private void cbUser_SelectedValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (_isChanging)
			{
				return;
			}
			string text = Function.ToString(cbUser.EditValue);
			if (!(text == "User"))
			{
				if (!(text == "Employee"))
				{
					return;
				}
				cfl_GroupUser.Visible = false;
				cfl_UserSharing.Visible = false;
				chk_InviteAll.Visible = false;
				chk_ShareAll.Visible = false;
				cfl_GroupEmployee.Visible = true;
				cfl_EmplSharing.Visible = true;
				chk_InviteAllEmp.Visible = true;
				_isChanging = true;
				cfl_GroupUser.EditValue = null;
				cfl_UserSharing.EditValue = null;
				chk_InviteAll.CheckState = CheckState.Unchecked;
				chk_ShareAll.CheckState = CheckState.Unchecked;
				chk_ShareAllEmp.Visible = true;
				if (!_isLoading && string.IsNullOrEmpty(Function.ToString(cfl_GroupEmployee.EditValue)) && _dtActiveEmployee.IsNotNull())
				{
					DataRow dataRow = _dtActiveEmployee.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["userId"]) == GlobalData.UserID);
					if (dataRow != null)
					{
						cfl_GroupEmployee.EditValue = Function.ParseInt(dataRow["empID"]);
					}
				}
				_isChanging = false;
			}
			else
			{
				cfl_GroupUser.Visible = true;
				cfl_UserSharing.Visible = true;
				chk_InviteAll.Visible = true;
				chk_ShareAll.Visible = true;
				cfl_GroupEmployee.Visible = false;
				cfl_EmplSharing.Visible = false;
				chk_InviteAllEmp.Visible = false;
				chk_ShareAllEmp.Visible = false;
				_isChanging = true;
				cfl_GroupEmployee.EditValue = null;
				cfl_EmplSharing.EditValue = null;
				if (!_isLoading && string.IsNullOrEmpty(Function.ToString(cfl_GroupUser.EditValue)))
				{
					cfl_GroupUser.EditValue = GlobalData.UserID;
				}
				chk_InviteAllEmp.CheckState = CheckState.Unchecked;
				chk_ShareAllEmp.CheckState = CheckState.Unchecked;
				_isChanging = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_SendMail_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (!_dtCurrentRow.IsNotNull())
			{
				return;
			}
			DataRow dataRow = _dtCurrentRow.Rows[0];
			int num = 0;
			_listEmailAddr.Clear();
			if (!string.IsNullOrEmpty(Function.ToString(_dtCurrentRow.Rows[0]["GroupUser"])))
			{
				string text = ((!string.IsNullOrEmpty(Function.ToString(cfl_GroupUser.EditValue))) ? (Function.ToString(cfl_GroupUser.EditValue) + ";" + Function.ToString(GlobalData.UserID)) : Function.ToString(GlobalData.UserID));
				if (!string.IsNullOrEmpty(text))
				{
					string[] array = Function.ToString(text).Split(';');
					num = array.Length;
					string[] array2 = array;
					foreach (string user in array2)
					{
						DataRow dataRow2 = _dtUnlockUser.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["USERID"]) == Function.ParseInt(user));
						if (dataRow2 != null && !string.IsNullOrEmpty(Function.ToString(dataRow2["E_Mail"])))
						{
							_listEmailAddr.Add(Function.ToString(dataRow2["E_Mail"]));
						}
					}
				}
			}
			else if (!string.IsNullOrEmpty(Function.ToString(_dtCurrentRow.Rows[0]["GroupEmployee"])))
			{
				string text = Function.ToString(cfl_GroupEmployee.EditValue);
				if (!string.IsNullOrEmpty(text))
				{
					string[] array3 = Function.ToString(text).Split(';');
					num = array3.Length;
					string[] array2 = array3;
					foreach (string user2 in array2)
					{
						DataRow dataRow3 = _dtActiveEmployee.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["empID"]) == Function.ParseInt(user2));
						if (dataRow3 != null && !string.IsNullOrEmpty(Function.ToString(dataRow3["email"])))
						{
							_listEmailAddr.Add(Function.ToString(dataRow3["email"]));
						}
					}
				}
			}
			if (_listEmailAddr.Count <= 0)
			{
				WinCommons.Error(Enums.MessageCode.InfoFillEmailForReceieve, "");
				return;
			}
			if (_listEmailAddr.Count < num)
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.EventManagementConfirmReceiveNotEmail, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
				{
					return;
				}
			}
			else if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSendMail, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				return;
			}
			string subject = Function.ToString(dataRow["Details"]);
			XtraInputBoxArgs xtraInputBoxArgs = new XtraInputBoxArgs
			{
				Caption = "Send options",
				Prompt = "Choose Email Template you want to send",
				DefaultButtonIndex = 0,
				LookAndFeel = LookAndFeel.ParentLookAndFeel
			};
			RadioGroup radioGroup = new RadioGroup();
			radioGroup.Properties.BorderStyle = BorderStyles.NoBorder;
			RadioGroupItem item = new RadioGroupItem(1, "Use Default Template");
			radioGroup.BackColor = BackColor;
			RadioGroupItem item2 = new RadioGroupItem(2, "Use Your Template");
			radioGroup.Properties.Items.Add(item);
			radioGroup.Properties.Items.Add(item2);
			xtraInputBoxArgs.Editor = radioGroup;
			xtraInputBoxArgs.DefaultResponse = 1;
			int sendType = Function.ParseInt(XtraInputBox.Show(xtraInputBoxArgs));
			string text2 = await PopulateBody(sendType);
			if (text2 == "Error")
			{
				WinCommons.Error(Enums.MessageCode.InfoNotSelectedTemplateSendMailSelectTemp, "");
			}
			else
			{
				if (text2 == "Exception")
				{
					return;
				}
				string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(string.Concat("<div style='border: medium solid grey; width: 500px; height: 266px;font-family: arial,sans-serif; font-size: 17px;'>" + "<h3 style='background-color: blueviolet; margin-top:0px;'>Aspen Reporting Tool</h3>", "<br />"), "Dear abc,"), "<br />"), "<p>"), "Thank you for registering </p>"), "<p><a href='apzon.com'>Click Here</a>To finalize the registration process</p>"), " <br />"), "Thanks,"), "<br />"), "<b>The Team</b>"), "</div>");
				List<string> list = new List<string>();
				DataTable dataTable = ucAttachment1.grd_File.DataSource as DataTable;
				if (dataTable.IsNotNull())
				{
					for (int num2 = 0; num2 < dataTable.Rows.Count; num2++)
					{
						DataRow dataRow4 = dataTable.Rows[num2];
						string path = (string.IsNullOrWhiteSpace(Function.ToString(dataRow4["srcPath"])) ? "" : Function.ToString(dataRow4["srcPath"]));
						string path2 = (string.IsNullOrWhiteSpace(Function.ToString(dataRow4["FileName"])) ? "" : Function.ToString(dataRow4["FileName"]));
						string text3 = (string.IsNullOrWhiteSpace(Function.ToString(dataRow4["FileExt"])) ? "" : Function.ToString(dataRow4["FileExt"]));
						string path3 = Function.ToString(dataRow4["trgtPath"]);
						_ = Path.Combine(path, path2) + "." + text3;
						string item3 = Path.Combine(path3, path2) + "." + text3;
						list.Add(item3);
					}
				}
				SmtpMailClient smtpMailClient = new SmtpMailClient();
				try
				{
					WinCommons._OpenProcessing();
					string text4 = await smtpMailClient.SendEmail(_listEmailAddr, subject, text2, null, null, null, list);
					if (!string.IsNullOrEmpty(text4))
					{
						WinCommons.Error(Enums.MessageCode.InfoSendMailFailedWithReason, "", text4);
						Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), text4.ToString());
						return;
					}
					WinCommons._CloseProcessing();
				}
				catch (Exception ex)
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
					WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
					return;
				}
				finally
				{
					WinCommons._CloseProcessing();
				}
				WinCommons.Success(Enums.MessageCode.InfoSendMailSuccess, "");
			}
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.ToString());
			WinCommons.ErrorLineNumber(ex2);
		}
	}

	private async Task<string> PopulateBody(int sendType)
	{
		try
		{
			string locationCode = "";
			DataRow dr = _dtCurrentRow.Rows[0];
			DataTable dataTable = await _httpClient.ActivityClient.ActivityMeetingLocation_GetAll();
			int location = Function.ParseInt(dr["Location"]);
			if (dataTable.IsNotNull())
			{
				DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["Code"]) == location);
				if (dataRow != null)
				{
					locationCode = Function.ToString(dataRow["Name"]);
				}
			}
			string text = Function.ToString(dr["Notes"]);
			DateTime date = Function.ParseDateTimes(dr["Recontact"]).Date;
			DateTime date2 = Function.ParseDateTimes(dr["EndDate"]).Date;
			bool flag = Function.ParseBool(dr["AllDayEvent"]);
			string text2 = "";
			string text3 = "";
			if (!flag)
			{
				TimeSpan value = Function.ParseIntToTimeSpan(Function.ParseInt(dr["BeginTime"]));
				TimeSpan value2 = Function.ParseIntToTimeSpan(Function.ParseInt(dr["ENDTime"]));
				text2 = DateTime.Today.Add(value).ToString("hh:mm tt");
				text3 = DateTime.Today.Add(value2).ToString("hh:mm tt");
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("{Location}", locationCode);
			dictionary.Add("{Content}", text);
			if (date != date2 || (date == date2 && date != DateTime.Today))
			{
				if (flag)
				{
					dictionary.Add("{Start}", date.ToShortDateString());
					dictionary.Add("{End}", date2.ToShortDateString());
				}
				else
				{
					dictionary.Add("{Start}", date.ToShortDateString() + " " + text2);
					dictionary.Add("{End}", date2.ToShortDateString() + " " + text3);
				}
			}
			else
			{
				dictionary.Add("{Start}", text2);
				dictionary.Add("{End}", text3);
			}
			string text4 = "";
			if (sendType == 2)
			{
				OpenFileDialog openFileDialog = new OpenFileDialog();
				openFileDialog.Multiselect = false;
				if (openFileDialog.ShowDialog() != DialogResult.OK)
				{
					return "Error";
				}
				string[] fileNames = openFileDialog.FileNames;
				if (fileNames.Length == 0)
				{
					return "Error";
				}
				using (StreamReader streamReader = new StreamReader(Path.GetFullPath(fileNames[0])))
				{
					text4 = streamReader.ReadToEnd();
				}
				if (!string.IsNullOrEmpty(text4))
				{
					foreach (KeyValuePair<string, string> item in dictionary)
					{
						text4 = text4.Replace(item.Key, item.Value);
					}
				}
			}
			else
			{
				if (!string.IsNullOrEmpty(locationCode))
				{
					text4 = text4 + "<H4>Location: </H4> " + locationCode;
				}
				text4 = text4 + "<H4>Content: </H4>  <p>" + text + "</p>";
				if (!(date != date2) && (!(date == date2) || !(date != DateTime.Today)))
				{
					text4 = text4 + "<H3>Time: </H3> &nbsp;" + text2 + " - " + text3;
				}
				else
				{
					text4 = ((!flag) ? (text4 + "<H3>Time: </H3> &nbsp;" + date.ToShortDateString() + " " + text2 + " - " + date2.ToShortDateString() + " " + text3) : (text4 + "<H3>Time: </H3> &nbsp;" + date.ToShortDateString() + " - " + date2.ToShortDateString()));
				}
			}
			return text4;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
			return "Exception";
		}
	}

	private void gv_Doc_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
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
			string text4 = Function.ToString(gridView.GetListSourceRowCellValue(listSourceRowIndex, "AllEmplInvite"));
			string text5 = "";
			if (!string.IsNullOrEmpty(text))
			{
				foreach (string item in text.Split(new char[1] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList())
				{
					int key = Function.ParseInt(item);
					if (_dictionUser.ContainsKey(key))
					{
						text5 = text5 + _dictionUser[key] + "; ";
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
						text5 = text5 + _dictionEmployee[key2] + "; ";
					}
				}
			}
			if (text5.Length > 2)
			{
				text5 = text5.Remove(text5.Length - 2);
			}
			if (Function.ToString(text3) == "Y" || Function.ToString(text4) == "Y")
			{
				e.Value = "All Resources";
			}
			else
			{
				e.Value = text5;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btnSearch_Click(object sender, EventArgs e)
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

	private void ucAttachment1_ButtonBrowse_Click(object sender, EventArgs e)
	{
		if (FormMode == Enums.FormViewModeType.View)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	private void xtraTabControl1_SelectedPageChanging(object sender, TabPageChangingEventArgs e)
	{
		_isLoading = true;
	}

	private void xtraTabControl1_SelectedPageChanged(object sender, TabPageChangedEventArgs e)
	{
		_isLoading = false;
	}

	private void chk_InviteAll_CheckStateChanged(object sender, EventArgs e)
	{
		try
		{
			CheckEdit checkEdit = (CheckEdit)sender;
			if (checkEdit.CheckState == CheckState.Checked)
			{
				cfl_UserSharing.Visible = false;
				cfl_EmplSharing.Visible = false;
				lbl_Share.Visible = false;
				cfl_GroupUser.Enabled = false;
				cfl_GroupEmployee.Enabled = false;
				chk_ShareAll.Visible = false;
				chk_ShareAll.Enabled = false;
				chk_ShareAllEmp.Visible = false;
				chk_ShareAllEmp.Enabled = false;
				_isChanging = true;
				if (checkEdit.Name == "chk_InviteAll")
				{
					chk_ShareAll.CheckState = CheckState.Unchecked;
				}
				else
				{
					chk_ShareAllEmp.CheckState = CheckState.Unchecked;
				}
				_isChanging = false;
			}
			else
			{
				cfl_UserSharing.Visible = true;
				cfl_EmplSharing.Visible = true;
				lbl_Share.Visible = true;
				cfl_GroupUser.Enabled = true;
				cfl_GroupEmployee.Enabled = true;
				chk_ShareAll.Visible = true;
				chk_ShareAll.Enabled = true;
				chk_ShareAllEmp.Visible = true;
				chk_ShareAllEmp.Enabled = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void chk_ShareAll_CheckStateChanged(object sender, EventArgs e)
	{
		try
		{
			CheckEdit checkEdit = (CheckEdit)sender;
			if (!_isChanging && chk_InviteAll.CheckState != CheckState.Checked)
			{
				if (checkEdit.CheckState == CheckState.Checked)
				{
					cfl_UserSharing.Enabled = false;
					cfl_EmplSharing.Enabled = false;
				}
				else
				{
					cfl_UserSharing.Enabled = true;
					cfl_EmplSharing.Enabled = true;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadData()
	{
		_ = 2;
		try
		{
			_isLoading = true;
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
				gv_Doc.BeginUpdate();
				if (!_dtSearch.IsNotNull())
				{
					_dtSearch = new DataTable();
					_dtSearch.Columns.Add("userCode", typeof(string));
					_dtSearch.Columns.Add("startDate", typeof(DateTime));
					_dtSearch.Columns.Add("endDate", typeof(DateTime));
				}
				_dtSearch.Rows.Clear();
				_dtSearch.Rows.Add(GlobalData.UserCodeLogin, (date_From.EditValue == null) ? DBNull.Value : ((object)Function.ParseDateTimes(date_From.EditValue)), (date_To.EditValue == null) ? DBNull.Value : ((object)Function.ParseDateTimes(date_To.EditValue)));
				DataTable dataTable = await _httpClient.SubActivities.LoadEventByInterval(_dtSearch);
				if (dataTable.IsNotNull())
				{
					dataTable.SetDefaultAndMaxLen(gv_Doc.ItemId);
					grd_Doc.DataSource = dataTable;
					grd_Doc.RefreshDataSource();
					grd_Doc.DataSourceTable.AcceptChanges();
					AddNewRow();
				}
				else
				{
					grd_Doc.DataSourceTable.Clear();
					AddNewRow();
				}
				gv_Doc.ExpandAllGroups();
				gv_Doc.EndUpdate();
				dataTable.PrimaryKey = dataTable.Columns["ClgCode"].ToArray();
			}
			WinCommons._CloseProcessing();
			_isLoading = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			WinCommons._CloseProcessing();
			_isLoading = false;
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
			ucAttachment1.EnableButtons(isEnabled: true);
			btn_SendMail.Enabled = true;
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
			tableStructure.SetDefaultAndMaxLen(gv_Doc.ItemId);
			if (DataRowFromCal != null)
			{
				DataRow dataRow = tableStructure.NewRow();
				dataRow["ClgCode"] = 0;
				ClgCode = 0;
				dataRow["RecurPat"] = "N";
				dataRow["SeStartDat"] = DataRowFromCal["Recontact"];
				dataRow["Interval"] = 1;
				dataRow["MaxOccur"] = 1;
				dataRow["Interval"] = 1;
				dataRow["BeginTime"] = DataRowFromCal["BeginTime"];
				dataRow["ENDTime"] = DataRowFromCal["ENDTime"];
				dataRow["Recontact"] = DataRowFromCal["Recontact"];
				dataRow["endDate"] = DataRowFromCal["endDate"];
				dataRow["AllDayEvent"] = DataRowFromCal["AllDayEvent"];
				dataRow["GroupUser"] = DataRowFromCal["GroupUser"];
				dataRow["GroupEmployee"] = DataRowFromCal["GroupEmployee"];
				dataRow["ResourceIdsInfo"] = Function.ToString(DataRowFromCal["ResourceIdsInfo"]);
				dataRow["Monday"] = "N";
				dataRow["Tuesday"] = "N";
				dataRow["Wednesday"] = "N";
				dataRow["Thursday"] = "N";
				dataRow["Friday"] = "N";
				dataRow["Saturday"] = "N";
				dataRow["Sunday"] = "N";
				dataRow["AllUserInvite"] = "N";
				dataRow["AllUserShare"] = "N";
				dataRow["AllEmplInvite"] = "N";
				dataRow["AllEmplShare"] = "N";
				dataRow["DocType"] = DataRowFromCal["DocType"];
				dataRow["DocEntry"] = DataRowFromCal["DocEntry"];
				dataRow["DocNum"] = DataRowFromCal["DocNum"];
				dataRow["CardCode"] = DataRowFromCal["CardCode"];
				dataRow["ProjectEntry"] = DataRowFromCal["ProjectEntry"];
				dataRow["ProjectWBS"] = DataRowFromCal["ProjectWBS"];
				dataRow["OprId"] = DataRowFromCal["OprId"];
				dataRow["OprLine"] = DataRowFromCal["OprLine"];
				dataRow["Reminder"] = "N";
				dataRow["RemQty"] = 15;
				dataRow["RemType"] = "M";
				dataRow["AssignedBy"] = GlobalData.UserID;
				tableStructure.Rows.Add(dataRow);
				tableStructure.AcceptChanges();
			}
			DataSet dataSet = new DataSet();
			_dtCurrentRow = tableStructure.Copy();
			dataSet.Tables.Add(_dtCurrentRow);
			await BindControl(dataSet);
			FormMode = Enums.FormViewModeType.Add;
			ucComment1.Enabled = false;
			btn_SendMail.Enabled = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadInitForm()
	{
		_ = 8;
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
			cfl_ProjectWBS.Properties.ValueMember = "TaskCode";
			cfl_ProjectWBS.Properties.DisplayMember = "TaskCode";
			cfl_ProjectWBS.Properties.DisplayMember2 = "TaskName";
			cfl_ProjectWBS.Properties.SeparateString = " - ";
			cfl_ProjectWBS.Properties.TypeCFL = Enums.TypeCFL.ProjectTask;
			cfl_ProjectWBS.Properties.TypeDrillDown = ApzObjectTypes.oProjectManager;
			RepositoryItemButtonEditCFL properties2 = cfl_ProjectWBS.Properties;
			properties2.BeforeShowCFL = (Action<frm_CFL>)Delegate.Combine(properties2.BeforeShowCFL, (Action<frm_CFL>)((frm_CFL frm) =>
			{
				frm.projectId = Function.ParseInt(cfl_ProjectNew.EditValue);
			}));
			cfl_Opportunity.Properties.ValueMember = "OprId";
			cfl_Opportunity.Properties.DisplayMember = "OprId";
			cfl_Opportunity.Properties.DisplayMember2 = "Name";
			cfl_Opportunity.Properties.SeparateString = " - ";
			cfl_Opportunity.Properties.TypeDrillDown = ApzObjectTypes.oSalesOpportunities;
			cfl_Opportunity.Properties.TypeCFL = Enums.TypeCFL.Opportunity;
			cfl_Opportunity.Properties.HandleChooseItem = Opportunity_HandleChooseItem;
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
			cfl_EmplSharing.Properties.ValueMember = "EmpId";
			cfl_EmplSharing.Properties.DisplayMember = "lastName";
			cfl_EmplSharing.Properties.DisplayMember2 = "firstName";
			cfl_EmplSharing.Properties.TypeCFL = Enums.TypeCFL.EmployeeListForSalesEmpSetup;
			cfl_EmplSharing.Properties.TypeDrillDown = ApzObjectTypes.oEmployeesInfo;
			cfl_EmplSharing.Properties.MultiSelectMode = true;
			cfl_UserSharing.Properties.ValueMember = "USERID";
			cfl_UserSharing.Properties.DisplayMember = "U_NAME";
			cfl_UserSharing.Properties.DisplayMember2 = "";
			cfl_UserSharing.Properties.TypeCFL = Enums.TypeCFL.GetUserList;
			cfl_UserSharing.Properties.TypeDrillDown = ApzObjectTypes.oNone;
			cfl_UserSharing.Properties.Buttons[1].Visible = false;
			cfl_UserSharing.Properties.MultiSelectMode = true;
			cfl_EmplSharing.Properties.ValidateOnEnterKey = true;
			cfl_UserSharing.Properties.ValidateOnEnterKey = true;
			Task t7 = cfl_UserSharing.Properties.RefreshDataSourceAsync();
			Task t8 = cfl_EmplSharing.Properties.RefreshDataSourceAsync();
			Task t9 = cfl_ProjectNew.Properties.RefreshDataSourceAsync();
			DataTable dataTable = await _httpClient.SubActivities.GetEmployeeByUserId(GlobalData.UserID);
			if (dataTable.IsNotNull())
			{
				_owner = Function.ParseInt(dataTable.Rows[0][0]);
			}
			Task t10 = txtOwner.Properties.RefreshDataSourceAsync();
			await t2;
			await t3;
			await t9;
			await t7;
			await t8;
			await t10;
			_dictionUser.Clear();
			_dictionUser = cfl_GroupUser.Properties.dictionSource.ToDictionary((KeyValuePair<string, string> s) => Function.ParseInt(s.Key), (KeyValuePair<string, string> s) => s.Value);
			_dtUnlockUser = cfl_GroupUser.Properties.DataSource;
			_dictionEmployee.Clear();
			_dictionEmployee = cfl_GroupEmployee.Properties.dictionSource.ToDictionary((KeyValuePair<string, string> s) => Function.ParseInt(s.Key), (KeyValuePair<string, string> s) => s.Value);
			_dtActiveEmployee = cfl_GroupEmployee.Properties.DataSource;
			cfl_GroupUser.EditValueChanged += cfl_GroupUser_EditValueChanged;
			cfl_GroupEmployee.EditValueChanged += cfl_GroupUser_EditValueChanged;
			cfl_UserSharing.EditValueChanged += cfl_UserSharing_EditValueChanged;
			cfl_EmplSharing.EditValueChanged += cfl_UserSharing_EditValueChanged;
			Task<int> t11 = layout_UDF.LoadControls("APZ_OCLG", ApzObjectTypes.oEventManagement);
			await LoadLookUpMeetting();
			await t11;
			LoadLookupRecur();
			gc_AssignTo.OptionsColumn.ReadOnly = true;
			gc_AssignTo.OptionsColumn.AllowEdit = false;
			gc_AssignTo.OptionsColumn.AllowFocus = false;
			gc_AssignTo.OptionsColumn.TabStop = false;
			rep_Bp.RefreshDataSource();
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL = rep_Bp;
			repositoryItemButtonEditCFL.DataSourceLoaded = (Action<DataTable>)Delegate.Combine(repositoryItemButtonEditCFL.DataSourceLoaded, (Action<DataTable>)((DataTable dtS) =>
			{
				txtBpCode.Properties.DataSource = dtS.Copy();
				txtBpCode.Refresh();
			}));
			txtBpCode.Properties.LoadDataSourceWhenEnter = true;
			ucLinkedDocument1.ItemSelected += OnContentChanged;
			ucLinkedDocument1.cbDocNum.EditValueChanged += OnContentChanged;
			ucLinkedDocument1.BeforeButtonPressed += UcLinkedDocument1_BeforeButtonPressed;
			gv_Doc.RowCellStyle += GvDocOnRowCellStyle;
			_isLoading = true;
			date_From.EditValue = DateTime.Today.AddDays(-Configuration.SearchDate);
			date_To.EditValue = DateTime.Today.AddDays(Configuration.SearchDate);
			_isLoading = false;
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
			ucAttachment1.gv_File.Columns["FileName"].VisibleIndex = 0;
			ucAttachment1.gv_File.Columns["trgtPath"].VisibleIndex = 1;
			ucComment1.ObjectType = ApzObjectTypes.oEventManagement;
			ucComment1.gv_Comment.Appearance.Empty.BackColor = Color.FromArgb(250, 250, 250);
			ucComment1.AutoGridSize = true;
			chk_InviteAllEmp.CheckStateChanged += chk_InviteAll_CheckStateChanged;
			chk_ShareAllEmp.CheckStateChanged += chk_ShareAll_CheckStateChanged;
			List<Common.Table_Code> list = new List<Common.Table_Code>
			{
				new Common.Table_Code
				{
					Code = "M",
					Name = "Minutes"
				},
				new Common.Table_Code
				{
					Code = "H",
					Name = "Hours"
				},
				new Common.Table_Code
				{
					Code = "D",
					Name = "Days"
				}
			};
			SetDataSourceByList(luk_RemType, list);
			foreach (Control control in layout_UDF.Controls)
			{
				if (control is BaseEdit && FormMode == Enums.FormViewModeType.Add)
				{
					(control as BaseEdit).EditValue = control.AccessibleDescription;
				}
			}
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

	private void RefreshCardTypeAndCardName()
	{
		try
		{
			string text = Function.ToString(txtBpCode.EditValue);
			if (text == "")
			{
				txt_BPType.EditValue = null;
				txt_BPName.EditValue = null;
				txt_BPType.Visible = false;
				txt_BPName.Visible = false;
			}
			if (text != "")
			{
				string cardTypeByCardCode = _httpClient.SubActivities.GetCardTypeByCardCode(text);
				txt_BPType.EditValue = cardTypeByCardCode;
				string cardNameByCode = GetCardNameByCode(text);
				txt_BPName.EditValue = cardNameByCode;
				txt_BPType.Visible = true;
				txt_BPName.Visible = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private string GetCardNameByCode(string cardCode)
	{
		try
		{
			using HttpClientHelper<string> httpClientHelper = new HttpClientHelper<string>(Configuration.ApiServer, "BusinessPartnerCatalogNumbers");
			return httpClientHelper.Get("GetCardNameByCode", new { cardCode });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		return "";
	}

	private void SetDataSourceByList(LookUpEdit luk, List<Common.Table_Code> list)
	{
		try
		{
			luk.Properties.DataSource = list;
			luk.Properties.DisplayMember = "Name";
			luk.Properties.ValueMember = "Code";
			luk.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
			luk.Properties.ForceInitialize();
			luk.Properties.PopulateColumns();
			luk.Properties.Columns["Code"].Visible = false;
			luk.Properties.DropDownRows = list.Count;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void cfl_UserSharing_EditValueChanged(object sender, EventArgs e)
	{
	}

	private void cfl_GroupUser_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (_isTextChanging)
			{
				return;
			}
			_isChanging = true;
			DataTable dataSourceTable = grd_Doc.DataSourceTable;
			string inviteRes = "";
			List<int> listUser = new List<int>();
			int clgCode = 0;
			DataRow dataRow;
			if (CallFromCal || CallFromRelated)
			{
				dataRow = DataRowFromCal;
			}
			else
			{
				clgCode = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("ClgCode"));
				dataRow = dataSourceTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["ClgCode"]) == clgCode);
			}
			if (dataRow != null)
			{
				if (((ButtonEditCFL)sender).Name == "cfl_GroupUser")
				{
					dataRow["GroupUser"] = Function.ToString(cfl_GroupUser.EditValue);
					RepositoryItemButtonEditCFL properties = cfl_UserSharing.Properties;
					properties.AfterShowCFL = (Action<frm_CFL>)Delegate.Combine(properties.AfterShowCFL, (Action<frm_CFL>)((frm_CFL frm) =>
					{
						try
						{
							DataTable dataTable = frm.gv_CFL.GridControl.DataSource as DataTable;
							if (dataTable.IsNotNull())
							{
								inviteRes = Function.ToString(cfl_GroupUser.EditValue);
								if (!string.IsNullOrEmpty(inviteRes))
								{
									string[] array = Function.ToString(inviteRes).Split(';');
									foreach (string obj in array)
									{
										listUser.Add(Function.ParseInt(obj));
									}
								}
								EnumerableRowCollection<DataRow> source = from p in dataTable.AsEnumerable()
									where !listUser.Contains(Function.ParseInt(p["USERID"]))
									select p;
								if (source.Any())
								{
									frm.gv_CFL.GridControl.DataSource = source.CopyToDataTable();
								}
								else
								{
									dataTable.Clear();
									dataTable.AcceptChanges();
								}
							}
						}
						catch (Exception ex2)
						{
							Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
							WinCommons.ErrorLineNumber(ex2);
						}
					}));
				}
				else if (((ButtonEditCFL)sender).Name == "cfl_GroupEmployee")
				{
					dataRow["GroupEmployee"] = Function.ToString(cfl_GroupEmployee.EditValue);
					RepositoryItemButtonEditCFL properties2 = cfl_EmplSharing.Properties;
					properties2.AfterShowCFL = (Action<frm_CFL>)Delegate.Combine(properties2.AfterShowCFL, (Action<frm_CFL>)((frm_CFL frm) =>
					{
						try
						{
							DataTable dataTable = frm.gv_CFL.GridControl.DataSource as DataTable;
							if (dataTable.IsNotNull())
							{
								inviteRes = Function.ToString(cfl_GroupEmployee.EditValue);
								if (!string.IsNullOrEmpty(inviteRes))
								{
									string[] array = Function.ToString(inviteRes).Split(';');
									foreach (string obj in array)
									{
										listUser.Add(Function.ParseInt(obj));
									}
								}
								EnumerableRowCollection<DataRow> source = from p in dataTable.AsEnumerable()
									where !listUser.Contains(Function.ParseInt(p["empID"]))
									select p;
								if (source.Any())
								{
									frm.gv_CFL.GridControl.DataSource = source.CopyToDataTable();
								}
								else
								{
									dataTable.Clear();
									dataTable.AcceptChanges();
								}
							}
						}
						catch (Exception ex2)
						{
							Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
							WinCommons.ErrorLineNumber(ex2);
						}
					}));
				}
			}
			gv_Doc.RefreshData();
			_isChanging = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void GvDocOnRowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (!(sender is GridView gridView))
			{
				return;
			}
			if (e.Column.FieldName == "endDate")
			{
				if (Function.ToString(e.CellValue) == "")
				{
					return;
				}
				Function.ParseDateTimes(e.CellValue);
			}
			if (e.Column.FieldName == "AssignTo")
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

	private async Task SetMaxLengthForControls()
	{
		try
		{
			Dictionary<string, Control> dicControls = new Dictionary<string, Control>
			{
				{ "APZ_OCLG.Details", txtDetails },
				{ "APZ_OCLG.Location", luk_Meetting },
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

	private async Task BindControl(DataSet ds)
	{
		_ = 1;
		try
		{
			DataTable dataTable = ds.Tables[0];
			if (!dataTable.IsNotNull())
			{
				return;
			}
			DataRow dataRow = dataTable.Rows[0];
			dataRow.AcceptChanges();
			PostDataBackToGridSource(dataTable);
			chkAllday.DataBindings.Clear();
			chkAllday.DataBindings.Add("EditValue", dataTable, "AllDayEvent", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
			chkAllday.Refresh();
			cfl_GroupUser.DataBindings.Clear();
			cfl_GroupUser.DataBindings.Add("EditValue", dataTable, "GroupUser", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
			cfl_GroupEmployee.DataBindings.Clear();
			cfl_GroupEmployee.DataBindings.Add("EditValue", dataTable, "GroupEmployee", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
			cfl_UserSharing.DataBindings.Clear();
			cfl_UserSharing.DataBindings.Add("EditValue", dataTable, "UserSharing", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
			cfl_EmplSharing.DataBindings.Clear();
			cfl_EmplSharing.DataBindings.Add("EditValue", dataTable, "EmplSharing", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
			chk_InviteAll.DataBindings.Clear();
			chk_InviteAll.DataBindings.Add("EditValue", dataTable, "AllUserInvite", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
			chk_ShareAll.DataBindings.Clear();
			chk_ShareAll.DataBindings.Add("EditValue", dataTable, "AllUserShare", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
			chk_InviteAllEmp.DataBindings.Clear();
			chk_InviteAllEmp.DataBindings.Add("EditValue", dataTable, "AllEmplInvite", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
			chk_ShareAllEmp.DataBindings.Clear();
			chk_ShareAllEmp.DataBindings.Add("EditValue", dataTable, "AllEmplShare", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
			cfl_ProjectWBS.DataBindings.Clear();
			cfl_ProjectWBS.DataBindings.Add("EditValue", dataTable, "ProjectWBS", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
			cfl_ProjectNew.DataBindings.Clear();
			cfl_ProjectNew.DataBindings.Add("EditValue", dataTable, "ProjectEntry", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
			cfl_Opportunity.DataBindings.Clear();
			cfl_Opportunity.DataBindings.Add("EditValue", dataTable, "OprId", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
			string value = Function.ToString(dataRow["GroupEmployee"]);
			string text = Function.ToString(dataRow["AllEmplInvite"]);
			string text2 = Function.ToString(dataRow["AllEmplShare"]);
			if (!string.IsNullOrEmpty(value) || text == "Y" || text2 == "Y")
			{
				cbUser.EditValue = "Employee";
			}
			else
			{
				cbUser.EditValue = "User";
			}
			txtDetails.DataBindings.Clear();
			txtDetails.DataBindings.Add("EditValue", dataTable, "Details", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
			luk_Meetting.DataBindings.Clear();
			luk_Meetting.DataBindings.Add("EditValue", dataTable, "Location", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
			wysiwygEdit1.richEditor.DataBindings.Clear();
			wysiwygEdit1.richEditor.DataBindings.Add("Text", dataTable, "Notes", formattingEnabled: false, DataSourceUpdateMode.OnValidation);
			wysiwygEdit1.richEditor.DataBindings.Add("HtmlText", dataTable, "HtmlNotes", formattingEnabled: false, DataSourceUpdateMode.OnValidation);
			cbRecurPat.DataBindings.Clear();
			cbRecurPat.DataBindings.Add("EditValue", dataTable, "RecurPat", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
			ucLinkedDocument1.EditValue = dataRow["DocNum"];
			ucLinkedDocument1.DocEntry = dataRow["DocEntry"];
			ucLinkedDocument1.DocType = Function.ParseInt(dataRow["DocType"]);
			bte_DistrRule.SetEditValue(dataRow);
			txtBpCode.DataBindings.Clear();
			txtBpCode.DataBindings.Add("EditValue", dataTable, "CardCode", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
			dStart_Recontact.DataBindings.Clear();
			dStart_Recontact.DataBindings.Add("EditValue", dataTable, "Recontact", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
			dEnDate.DataBindings.Clear();
			dEnDate.DataBindings.Add("EditValue", dataTable, "endDate", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
			txtBeginTime.DataBindings.Clear();
			txtBeginTime.DataBindings.Add("EditValue", dataTable, "BeginTime", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
			txtEndTime.DataBindings.Clear();
			txtEndTime.DataBindings.Add("EditValue", dataTable, "ENDTime", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
			txtInterval.DataBindings.Clear();
			txtInterval.DataBindings.Add("EditValue", dataTable, "Interval", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
			chkReminder.DataBindings.Clear();
			chkReminder.DataBindings.Add("EditValue", dataTable, "Reminder", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
			chkReminder.Refresh();
			calc_RemQty.DataBindings.Clear();
			calc_RemQty.DataBindings.Add("EditValue", dataTable, "RemQty", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
			luk_RemType.DataBindings.Clear();
			luk_RemType.DataBindings.Add("EditValue", dataTable, "RemType", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
			txtOwner.DataBindings.Clear();
			txtOwner.DataBindings.Add("EditValue", dataTable, "Owner", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
			string text3 = Function.ToString(dataRow["RecurPat"]);
			string text4 = dataRow["SubOption"].ToString();
			switch (text3)
			{
			case "D":
				if (!(text4 == "1"))
				{
					if (text4 == "2")
					{
						radio_SubOption2.Checked = true;
					}
					else
					{
						radio_SubOption1.Checked = true;
					}
				}
				else
				{
					radio_SubOption1.Checked = true;
				}
				break;
			case "M":
			case "A":
				if (!(text4 == "1"))
				{
					if (text4 == "2")
					{
						radio_SubOption2.Checked = true;
					}
					else
					{
						radio_SubOption1.Checked = true;
					}
				}
				else
				{
					radio_SubOption1.Checked = true;
				}
				break;
			default:
				radio_SubOption1.Checked = true;
				break;
			}
			chkMon.DataBindings.Clear();
			chkMon.DataBindings.Add("EditValue", dataTable, "Monday", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
			chkTue.DataBindings.Clear();
			chkTue.DataBindings.Add("EditValue", dataTable, "Tuesday", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
			chkWed.DataBindings.Clear();
			chkWed.DataBindings.Add("EditValue", dataTable, "Wednesday", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
			chkThu.DataBindings.Clear();
			chkThu.DataBindings.Add("EditValue", dataTable, "Thursday", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
			chkFri.DataBindings.Clear();
			chkFri.DataBindings.Add("EditValue", dataTable, "Friday", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
			chkSat.DataBindings.Clear();
			chkSat.DataBindings.Add("EditValue", dataTable, "Saturday", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
			chkSun.DataBindings.Clear();
			chkSun.DataBindings.Add("EditValue", dataTable, "Sunday", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
			cbMonth.SelectedItem = dataRow["Month"];
			cbMonth1.SelectedItem = dataRow["Month"];
			txtDayInMonth.DataBindings.Clear();
			txtDayInMonth.DataBindings.Add("EditValue", dataTable, "DayInMonth", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
			cbWeek.SelectedItem = Function.ToString(dataRow["Week"]);
			cbDayOfWeek.SelectedItem = Function.ToString(dataRow["DayOfWeek"]);
			dSeStartDat.DataBindings.Clear();
			dSeStartDat.DataBindings.Add("EditValue", dataTable, "SeStartDat", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
			switch (dataRow["EndType"].ToString())
			{
			case "N":
				radio_NoEndDate.Checked = true;
				break;
			case "C":
				radio_After.Checked = true;
				break;
			case "D":
				radio_By.Checked = true;
				break;
			default:
				radio_NoEndDate.Checked = true;
				break;
			}
			txtAfterMaxOccur.DataBindings.Clear();
			txtAfterMaxOccur.DataBindings.Add("EditValue", dataTable, "MaxOccur", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
			txtBySeEndDat.DataBindings.Clear();
			txtBySeEndDat.DataBindings.Add("EditValue", dataTable, "SeEndDat", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
			foreach (Control control in layout_UDF.Controls)
			{
				if (control is BaseEdit && dataRow.Table.Columns.Contains(control.Name))
				{
					control.DataBindings.Clear();
					control.DataBindings.Add("EditValue", dataTable, control.Name, formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
				}
			}
			dataTable.Columns.Cast<DataColumn>().Any((DataColumn c) => c.ColumnName.ToUpper().StartsWith("U_"));
			lblCreatedBy.Text = "";
			int key = Function.ParseInt(dataRow["AssignedBy"]);
			string text5 = Function.ToString(dataRow["CntctTime"]);
			DateTime dateTime = Function.ParseDateTimes(dataRow["CntctDate"]);
			string s = ((text5.Length < 2) ? Function.ToString(dateTime.Date.Add(new TimeSpan(0, Function.ParseInt(text5), 0))) : Function.ToString(dateTime.Date.Add(new TimeSpan(Function.ParseInt(text5) / 100, Function.ParseInt(text5.Substring(text5.Length - 2, 2)), 0))));
			int _code = Function.ParseInt(dataRow["ClgCode"]);
			if (_dictionUser.ContainsKey(key))
			{
				lblCreatedBy.Utext = "Created by: " + _dictionUser[key] + " (" + WinFuntions.FormatCreatedDate(s, shorten: true) + "). ID: " + _code;
			}
			else
			{
				lblCreatedBy.Utext = "Created by:";
			}
			int absEntry = Function.ParseInt(dataRow["AtcEntry"]);
			await ucAttachment1.LoadAttachments(absEntry);
			await ucComment1.RefreshComment(_code);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void GetMonth(DateTime dateChange)
	{
		try
		{
			if (dateChange == DateTime.Now.Date)
			{
				_month = DateTime.Now.Month;
			}
			else
			{
				_month = dateChange.Month;
			}
			switch (_month)
			{
			case 1:
				cbMonth.SelectedItem = "January";
				cbMonth1.SelectedItem = "January";
				break;
			case 2:
				cbMonth.SelectedItem = "February";
				cbMonth1.SelectedItem = "February";
				break;
			case 3:
				cbMonth.SelectedItem = "March";
				cbMonth1.SelectedItem = "March";
				break;
			case 4:
				cbMonth.SelectedItem = "April";
				cbMonth1.SelectedItem = "April";
				break;
			case 5:
				cbMonth.SelectedItem = "May";
				cbMonth1.SelectedItem = "May";
				break;
			case 6:
				cbMonth.SelectedItem = "June";
				cbMonth1.SelectedItem = "June";
				break;
			case 7:
				cbMonth.SelectedItem = "July";
				cbMonth1.SelectedItem = "July";
				break;
			case 8:
				cbMonth.SelectedItem = "August";
				cbMonth1.SelectedItem = "August";
				break;
			case 9:
				cbMonth.SelectedItem = "September";
				cbMonth1.SelectedItem = "September";
				break;
			case 10:
				cbMonth.SelectedItem = "October";
				cbMonth1.SelectedItem = "October";
				break;
			case 11:
				cbMonth.SelectedItem = "November";
				cbMonth1.SelectedItem = "November";
				break;
			case 12:
				cbMonth.SelectedItem = "December";
				cbMonth1.SelectedItem = "December";
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void GetDay(DateTime dateChange)
	{
		try
		{
			dateChange = Function.ParseDateTime(dStart_Recontact.EditValue) ?? DateTime.MinValue;
			DateTime date = DateTime.Now.Date;
			if (dateChange == date)
			{
				_day = (int)(date.DayOfWeek + 1);
			}
			else
			{
				_day = (int)(dateChange.DayOfWeek + 1);
			}
			switch (_day)
			{
			case 1:
				cbDayOfWeek.EditValue = "Sunday";
				chkSun.Checked = true;
				break;
			case 2:
				cbDayOfWeek.EditValue = "Monday";
				chkMon.Checked = true;
				break;
			case 3:
				cbDayOfWeek.EditValue = "Tuesday";
				chkTue.Checked = true;
				break;
			case 4:
				cbDayOfWeek.EditValue = "Wednesday";
				chkWed.Checked = true;
				break;
			case 5:
				cbDayOfWeek.EditValue = "Thursday";
				chkThu.Checked = true;
				break;
			case 6:
				cbDayOfWeek.EditValue = "Friday";
				chkFri.Checked = true;
				break;
			case 7:
				cbDayOfWeek.EditValue = "Saturday";
				chkSat.Checked = true;
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void GetDayOfMonth(DateTime dateChange)
	{
		try
		{
			dateChange = Function.ParseDateTime(dStart_Recontact.EditValue) ?? DateTime.MinValue;
			DateTime date = DateTime.Now.Date;
			if (dateChange == date)
			{
				_dayOfMonth = date.Day;
				txtDayInMonth.Text = Function.ToString(_dayOfMonth);
			}
			else
			{
				_dayOfMonth = dateChange.Day;
				txtDayInMonth.Text = Function.ToString(_dayOfMonth);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void GetWeekOfMonth(DateTime dateChange)
	{
		try
		{
			if (dateChange == DateTime.Now.Date)
			{
				DateTime date = DateTime.Now.Date;
				_weekOfMonth = (int)(date.Day + date.DayOfWeek) / 7 + 1;
			}
			else
			{
				_weekOfMonth = (int)(dateChange.Day + dateChange.DayOfWeek) / 7 + 1;
			}
			switch (_weekOfMonth)
			{
			case 1:
				cbWeek.SelectedItem = "first";
				break;
			case 2:
				cbWeek.SelectedItem = "second";
				break;
			case 3:
				cbWeek.SelectedItem = "third";
				break;
			case 4:
				cbWeek.SelectedItem = "fourth";
				break;
			case 5:
				cbWeek.SelectedItem = "last";
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadLookUpMeetting()
	{
		try
		{
			DataTable dataTable = await _httpClient.ActivityClient.ActivityMeetingLocation_GetAll();
			DataRow row = dataTable.NewRow();
			dataTable.Rows.InsertAt(row, 0);
			dataTable.AcceptChanges();
			DataRow dataRow = dataTable.NewRow();
			dataRow["Code"] = -10000;
			dataRow["Name"] = "-Define New-";
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			luk_Meetting.Properties.DataSource = null;
			luk_Meetting.Properties.DataSource = dataTable.Copy();
			luk_Meetting.Properties.DisplayMember = "Name";
			luk_Meetting.Properties.ValueMember = "Code";
			luk_Meetting.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
			luk_Meetting.Properties.DropDownRows = dataTable.Rows.Count;
			luk_Meetting.Properties.ForceInitialize();
			luk_Meetting.Properties.PopulateColumns();
			luk_Meetting.Properties.Columns["Code"].Visible = false;
			luk_Meetting.Properties.Columns.Clear();
			luk_Meetting.Properties.Columns.Add(new LookUpColumnInfo("Name", ""));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void duplicateRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (gv_Doc.FocusedRowHandle == 0 || (FormMode == Enums.FormViewModeType.Add && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No))
			{
				return;
			}
			WinCommons._OpenProcessing();
			DataTable dataTable = grd_Doc.DataSourceTable.Copy();
			dataTable.PrimaryKey = null;
			dataTable.Columns["ClgCode"].Unique = false;
			DataRow focusedDataRow = gv_Doc.GetFocusedDataRow();
			DataRow dataRow = dataTable.NewRow();
			int num = Function.ParseInt(grd_Doc.DataSourceTable.Copy().AsEnumerable().Max((DataRow r) => Function.ParseInt(r["ClgCode"]))) + 1;
			dataRow.ItemArray = focusedDataRow.ItemArray;
			dataRow["ClgCode"] = num;
			dataRow["Recontact"] = DateTime.Today;
			dataRow["endDate"] = DateTime.Today;
			dataRow["CntctDate"] = DateTime.Today;
			dataRow["CntctTime"] = Function.ParseTimeSpanToInt(DateTime.Now.TimeOfDay);
			dataRow["AssignedBy"] = GlobalData.UserID;
			dataRow["Recontact"] = DateTime.Today;
			dataRow["UpdateDate"] = DateTime.Today;
			dataRow["UserSign"] = GlobalData.UserID;
			dataTable.Rows.Add(dataRow.ItemArray);
			dataTable.AcceptChanges();
			grd_Doc.DataSource = dataTable;
			grd_Doc.RefreshDataSource();
			gv_Doc.FocusedRowHandle = 1;
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

	private void frm_EventManagement_OnControlValueChanged(object sender, ControlChangedEventArgs e)
	{
		try
		{
			if (_isTextChanging || !(e.FieldName == "AllDayEvent"))
			{
				return;
			}
			_isChanging = true;
			gv_Doc.SetFocusedRowCellValue("AllDayEvent", (Function.ParseInt(chkAllday.EditValue) == 1 || Function.ParseBool(chkAllday.EditValue)) ? 1 : 0);
			_isChanging = false;
			_isTextChanging = false;
			if (Function.ParseInt(chkAllday.EditValue) == 1 || Function.ParseBool(chkAllday.EditValue))
			{
				txtBeginTime.Visible = false;
				txtEndTime.Visible = false;
				txtBeginTime.EditValue = 0;
				txtEndTime.EditValue = 100;
				chkAllday.CheckState = CheckState.Checked;
				return;
			}
			if (string.IsNullOrEmpty(Function.ToString(txtBeginTime.EditValue)))
			{
				txtBeginTime.EditValue = 1000;
			}
			if (string.IsNullOrEmpty(Function.ToString(txtEndTime.EditValue)))
			{
				txtEndTime.EditValue = (string.IsNullOrEmpty(Function.ToString(txtBeginTime.EditValue)) ? 1100 : (Function.ParseInt(txtBeginTime.EditValue) + 100));
			}
			txtBeginTime.Visible = true;
			txtEndTime.Visible = true;
			chkAllday.CheckState = CheckState.Unchecked;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void LoadLookupRecur()
	{
		try
		{
			cbRecurPat.Properties.Columns.Clear();
			cbRecurPat.Properties.Columns.Add(new LookUpColumnInfo("Name", ""));
			cbRecurPat.Properties.NullText = "";
			cbRecurPat.Properties.DisplayMember = "Name";
			cbRecurPat.Properties.ValueMember = "Code";
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add(new DataColumn("Code", typeof(string)));
			dataTable.Columns.Add(new DataColumn("Name", typeof(string)));
			dataTable.AcceptChanges();
			string[,] array = new string[5, 2]
			{
				{ "N", "None" },
				{ "D", "Daily" },
				{ "W", "Weekly" },
				{ "M", "Monthly" },
				{ "A", "Annually" }
			};
			for (int i = 0; i < array.GetLength(0); i++)
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["Code"] = array[i, 0];
				dataRow["Name"] = array[i, 1];
				dataTable.Rows.Add(dataRow);
			}
			dataTable.AcceptChanges();
			cbRecurPat.Properties.DataSource = dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void AddNewRow()
	{
		try
		{
			gv_Doc.BeginInsertRow();
			DataTable dataTable = grd_Doc.DataSource as DataTable;
			if (!dataTable.IsNotNull())
			{
				dataTable = WinFuntions.GetTableStructure("APZ_OCLG");
				dataTable.SetDefaultAndMaxLen(gv_Doc.ItemId);
			}
			DataRow dataRow = dataTable.NewRow();
			dataRow["ClgCode"] = 0;
			dataRow["Recontact"] = DateTime.Today;
			dataRow["endDate"] = DateTime.Today;
			dataRow["BeginTime"] = 0;
			dataRow["ENDTime"] = 100;
			dataRow["RecurPat"] = "N";
			dataRow["SeStartDat"] = DateTime.Today;
			dataRow["Interval"] = 1;
			dataRow["MaxOccur"] = 1;
			dataRow["AllDayEvent"] = true;
			dataRow["GroupUser"] = Function.ToString(GlobalData.UserID);
			dataRow["GroupEmployee"] = "";
			dataRow["Monday"] = "N";
			dataRow["Tuesday"] = "N";
			dataRow["Wednesday"] = "N";
			dataRow["Thursday"] = "N";
			dataRow["Friday"] = "N";
			dataRow["Saturday"] = "N";
			dataRow["Sunday"] = "N";
			dataRow["AllUserInvite"] = "N";
			dataRow["AllUserShare"] = "N";
			dataRow["AllEmplInvite"] = "N";
			dataRow["AllEmplShare"] = "N";
			dataRow["CntctDate"] = DateTime.Today;
			dataRow["Reminder"] = "N";
			dataRow["RemQty"] = 15;
			dataRow["RemType"] = "M";
			dataRow["AssignedBy"] = GlobalData.UserID;
			dataRow["OwnerAuth"] = "F";
			if (_owner != 0)
			{
				dataRow["Owner"] = _owner;
			}
			foreach (Control control in layout_UDF.Controls)
			{
				if (control is BaseEdit && dataTable.Columns.Contains(control.Name) && !string.IsNullOrEmpty(control.AccessibleDescription))
				{
					dataRow[control.Name] = control.AccessibleDescription;
				}
			}
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			grd_Doc.DataSource = dataTable;
			dataTable.PrimaryKey = dataTable.Columns["ClgCode"].ToArray();
			grd_Doc.RefreshDataSource();
			grd_Doc.DataSourceTable.AcceptChanges();
			gv_Doc.EndInsertRow();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		gv_Doc.EndInsertRow();
	}

	private void PostDataBackToGridSource(DataTable dt)
	{
		try
		{
			if (!dt.IsNotNull())
			{
				return;
			}
			foreach (DataRow item in grd_Doc.DataSourceTable.AsEnumerable())
			{
				if (item["ClgCode"].Equals(_dtCurrentRow.Rows[0]["ClgCode"]))
				{
					item.ItemArray = dt.Rows[0].ItemArray;
				}
			}
			_recurPat = Function.ToString(dt.Rows[0]["RecurPat"]);
			grd_Doc.DataSourceTable.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private static void SetWeekDays(DataRow dr, RecurrenceInfo rInfo)
	{
		try
		{
			if (rInfo != null)
			{
				switch (Function.ParseInt(dr["DayOfWeek"]))
				{
				case 0:
					rInfo.WeekDays = WeekDays.WeekendDays;
					break;
				case 1:
					rInfo.WeekDays = WeekDays.Sunday;
					break;
				case 2:
					rInfo.WeekDays = WeekDays.Monday;
					break;
				case 3:
					rInfo.WeekDays = WeekDays.Tuesday;
					break;
				case 4:
					rInfo.WeekDays = WeekDays.Wednesday;
					break;
				case 5:
					rInfo.WeekDays = WeekDays.Thursday;
					break;
				case 6:
					rInfo.WeekDays = WeekDays.Friday;
					break;
				case 7:
					rInfo.WeekDays = WeekDays.Saturday;
					break;
				case 8:
					rInfo.WeekDays = WeekDays.EveryDay;
					break;
				case 9:
					rInfo.WeekDays = WeekDays.WorkDays;
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

	private async Task<string> SaveData(AppointmentType type = AppointmentType.Normal)
	{
		_ = 10;
		try
		{
			RecurrenceInfo rInfo = new RecurrenceInfo();
			RecurrenceInfo origInfo = new RecurrenceInfo();
			if (!string.IsNullOrEmpty(OrigRecurXml))
			{
				origInfo.FromXml(OrigRecurXml);
			}
			if (!string.IsNullOrEmpty(RecurXml))
			{
				rInfo.FromXml(RecurXml);
			}
			int saveType = 1;
			string recurPat = Function.ToString(cbRecurPat.EditValue);
			ChangeType changeType = ChangeType.Normal;
			if (type != AppointmentType.DeletedOccurrence)
			{
				if (AptType != AppointmentType.Normal || (_recurPat != "N" && recurPat != "N"))
				{
					if (CallFromCal)
					{
						frm_ConfirmDialog frm_ConfirmDialog2 = new frm_ConfirmDialog(1)
						{
							EventSubject = Function.ToString(_dtCurrentRow.Rows[0]["Details"]),
							StartPosition = FormStartPosition.CenterScreen
						};
						if (frm_ConfirmDialog2.ShowDialog() != DialogResult.OK)
						{
							return "error!";
						}
						if (frm_ConfirmDialog2.GetValue() == 1)
						{
							changeType = ChangeType.ChangeThis;
							saveType = 2;
						}
						else
						{
							changeType = ChangeType.ChangeAll;
							saveType = 1;
						}
					}
					else if (_recurPat != "N" && XtraMessageBoxEx.Show(Enums.MessageCode.EventManagementConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
					{
						return "error!";
					}
				}
				else if (_recurPat == "N" && recurPat != "N" && XtraMessageBoxEx.Show(Enums.MessageCode.EventManagementConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
				{
					return "error!";
				}
			}
			if (_dtCurrentRow.IsNotNull())
			{
				WinCommons._OpenProcessing();
				if (string.IsNullOrEmpty(Function.ToString(_dtCurrentRow.Rows[0]["Details"])))
				{
					WinCommons.Error(Enums.MessageCode.InfoSubjectCanNotEmpty, "");
					return "Error";
				}
				DataRow dr = _dtCurrentRow.Rows[0];
				int clgCode = 0;
				if (CallFromCal || CallFromRelated)
				{
					if (DataRowFromCal != null)
					{
						if (type == AppointmentType.DeletedOccurrence)
						{
							clgCode = ((AptType == AppointmentType.ChangedOccurrence || AptType == AppointmentType.Occurrence) ? ClgCode : 0);
						}
						else
						{
							switch (changeType)
							{
							case ChangeType.ChangeThis:
							{
								DateTime oldStart = default;
								DateTime seStart = default;
								DateTime seEnd = default;
								if (DataRowFromCal != null)
								{
									oldStart = Function.ParseDateTimes(DataRowFromCal["Recontact"]);
									seStart = Function.ParseDateTimes(DataRowFromCal["SeStartDat"]);
									seEnd = Function.ParseDateTimes(DataRowFromCal["SeEndDat"]);
								}
								else
								{
									DataTable dataTable = await _httpClient.SubActivities.GetActivityByCode(clgCode, GlobalData.UserCodeLogin);
									if (dataTable.IsNotNull())
									{
										oldStart = Function.ParseDateTimes(dataTable.Rows[0]["Recontact"]);
										seStart = Function.ParseDateTimes(dataTable.Rows[0]["SeStartDat"]);
										seEnd = Function.ParseDateTimes(dataTable.Rows[0]["SeEndDat"]);
									}
								}
								DateTime recontact = Function.ParseDateTimes(dr["Recontact"]);
								if (!(oldStart.Date != recontact.Date))
								{
									break;
								}
								if (recontact < seStart || (recontact > seEnd && !string.IsNullOrEmpty(Function.ToString(dr["SeEndDat"]))))
								{
									WinCommons.Error(Enums.MessageCode.YouCanNotScheduleAnEventSkipOverOtherEventSameRecurrence, "");
									return "Error";
								}
								int num = Function.ParseInt(DataRowFromCal["SeriesNum"]);
								if (num > 0)
								{
									DataTable dataTable2 = await _httpClient.SubActivities.GetActivityBySeriesNum(num, GlobalData.UserCodeLogin);
									List<DateTime> list = new List<DateTime>();
									if (dataTable2.IsNotNull())
									{
										for (int i = 0; i < dataTable2.Rows.Count; i++)
										{
											if (Function.ToString(dataTable2.Rows[i]["OrigDate"]) != "")
											{
												DateTime item = Function.ParseDateTimes(dataTable2.Rows[i]["OrigDate"]);
												list.Add(item);
											}
											if (Function.ParseInt(dataTable2.Rows[i]["ClgCode"]) != ClgCode && Function.ParseDateTimes(dataTable2.Rows[i]["Recontact"]).Date == recontact.Date)
											{
												WinCommons.Error(Enums.MessageCode.YouCanNotSchedule2InstanceOfSaveRecurrenceOnSameDay, "");
												return "Error";
											}
										}
									}
									OccurrenceCalculator occurrenceCalculator = OccurrenceCalculator.CreateInstance(origInfo);
									for (int j = 0; j <= origInfo.OccurrenceCount - 1; j++)
									{
										DateTime end = occurrenceCalculator.CalcOccurrenceStartTime(j);
										if ((list.Count <= 0 || !list.Any((DateTime date) => date.Date == end.Date)) && end.Date == recontact.Date)
										{
											WinCommons.Error(Enums.MessageCode.YouCanNotSchedule2InstanceOfSaveRecurrenceOnSameDay, "");
											return "Error";
										}
									}
								}
								type = AppointmentType.ChangedOccurrence;
								clgCode = ((Function.ParseInt(dr["SeriesNum"]) != 0) ? ClgCode : 0);
								break;
							}
							case ChangeType.ChangeAll:
								type = ((Function.ParseInt(dr["SeriesNum"]) == 0) ? AppointmentType.Pattern : AppointmentType.ChangedOccurrence);
								clgCode = ClgCode;
								break;
							default:
								if (recurPat != "N")
								{
									type = AppointmentType.Pattern;
								}
								clgCode = ClgCode;
								break;
							}
						}
					}
				}
				else
				{
					if (recurPat != "N")
					{
						type = AppointmentType.Pattern;
					}
					clgCode = Function.ParseInt(dr["ClgCode"]);
				}
				string value = string.Empty;
				if (radio_NoEndDate.Checked)
				{
					value = "N";
				}
				else if (radio_After.Checked)
				{
					value = "C";
				}
				else if (radio_By.Checked)
				{
					value = "D";
				}
				dr["EndType"] = value;
				dr["ClgCode"] = clgCode;
				dr["Action"] = "V";
				dr["UserSign"] = GlobalData.UserID;
				dr["UpdateDate"] = DateTime.Today;
				dr["UpdateTime"] = Function.ParseTimeSpanToInt(DateTime.Now.TimeOfDay);
				dr["Closed"] = "N";
				dr["status"] = 0;
				dr["Notes"] = wysiwygEdit1.richEditor.Text;
				dr["HtmlNotes"] = wysiwygEdit1.richEditor.HtmlText;
				dr["DocNum"] = ucLinkedDocument1.EditValue;
				dr["DocType"] = ucLinkedDocument1.DocType;
				dr["DocEntry"] = ucLinkedDocument1.DocEntry;
				switch (type)
				{
				case AppointmentType.Normal:
					if (string.IsNullOrEmpty(Function.ToString(dr["CntctDate"])))
					{
						dr["CntctDate"] = DateTime.Today;
					}
					if (string.IsNullOrEmpty(Function.ToString(dr["CntctTime"])))
					{
						dr["CntctTime"] = Function.ParseTimeSpanToInt(DateTime.Now.TimeOfDay);
					}
					dr["SeriesNum"] = DBNull.Value;
					dr["IsRemoved"] = "N";
					dr["OrigDate"] = DBNull.Value;
					break;
				case AppointmentType.ChangedOccurrence:
					dr["CntctDate"] = Function.ParseDateTimes(DataRowFromCal["CntctDate"]);
					dr["CntctTime"] = Function.ParseInt(DataRowFromCal["CntctTime"]);
					if (clgCode <= 0)
					{
						dr["SeriesNum"] = Function.ParseInt(DataRowFromCal["ClgCode"]);
					}
					else
					{
						dr["SeriesNum"] = Function.ParseInt(DataRowFromCal["SeriesNum"]);
					}
					dr["IsRemoved"] = "N";
					if (!string.IsNullOrEmpty(Function.ToString(DataRowFromCal["OrigDate"])) && changeType == ChangeType.ChangeThis)
					{
						dr["OrigDate"] = Function.ParseDateTimes(DataRowFromCal["OrigDate"]);
					}
					else
					{
						dr["OrigDate"] = Function.ParseDateTimes(DataRowFromCal["Recontact"]);
					}
					break;
				case AppointmentType.DeletedOccurrence:
					dr["CntctDate"] = Function.ParseDateTimes(DataRowFromCal["CntctDate"]);
					dr["CntctTime"] = Function.ParseInt(DataRowFromCal["CntctTime"]);
					if (clgCode <= 0)
					{
						dr["SeriesNum"] = Function.ParseInt(DataRowFromCal["ClgCode"]);
					}
					else
					{
						dr["SeriesNum"] = Function.ParseInt(DataRowFromCal["SeriesNum"]);
					}
					dr["IsRemoved"] = "Y";
					if (!string.IsNullOrEmpty(Function.ToString(DataRowFromCal["OrigDate"])) && changeType == ChangeType.ChangeThis)
					{
						dr["OrigDate"] = Function.ParseDateTimes(DataRowFromCal["OrigDate"]);
					}
					else
					{
						dr["OrigDate"] = Function.ParseDateTimes(DataRowFromCal["Recontact"]);
					}
					break;
				case AppointmentType.Pattern:
					if (string.IsNullOrEmpty(Function.ToString(dr["CntctDate"])))
					{
						dr["CntctDate"] = DateTime.Today;
					}
					if (string.IsNullOrEmpty(Function.ToString(dr["CntctTime"])))
					{
						dr["CntctTime"] = Function.ParseTimeSpanToInt(DateTime.Now.TimeOfDay);
					}
					dr["SeriesNum"] = DBNull.Value;
					dr["IsRemoved"] = "N";
					dr["OrigDate"] = DBNull.Value;
					break;
				}
				bte_DistrRule.AddValueToRow(dr);
				_dtCurrentRow.AcceptChanges();
				string value2 = string.Empty;
				dr["RecurPat"] = recurPat;
				if (radio_SubOption1.Checked)
				{
					value2 = "1";
				}
				else if (radio_SubOption2.Checked)
				{
					value2 = "2";
				}
				dr["SubOption"] = value2;
				if (recurPat == "N")
				{
					dr["Monday"] = "N";
					dr["Tuesday"] = "N";
					dr["Wednesday"] = "N";
					dr["Thursday"] = "N";
					dr["Friday"] = "N";
					dr["Saturday"] = "N";
					dr["Sunday"] = "N";
				}
				switch (Function.ToString(cbWeek.SelectedItem))
				{
				case "first":
					dr["Week"] = 1;
					break;
				case "second":
					dr["Week"] = 2;
					break;
				case "third":
					dr["Week"] = 3;
					break;
				case "fourth":
					dr["Week"] = 4;
					break;
				case "last":
					dr["Week"] = 5;
					break;
				}
				switch (Function.ToString(cbDayOfWeek.SelectedItem))
				{
				case "day":
					dr["DayOfWeek"] = 8;
					break;
				case "weekday":
					dr["DayOfWeek"] = 9;
					break;
				case "weekend day":
					dr["DayOfWeek"] = 0;
					break;
				case "Sunday":
					dr["DayOfWeek"] = 1;
					break;
				case "Monday":
					dr["DayOfWeek"] = 2;
					break;
				case "Tuesday":
					dr["DayOfWeek"] = 3;
					break;
				case "Wednesday":
					dr["DayOfWeek"] = 4;
					break;
				case "Thursday":
					dr["DayOfWeek"] = 5;
					break;
				case "Friday":
					dr["DayOfWeek"] = 6;
					break;
				case "Saturday":
					dr["DayOfWeek"] = 7;
					break;
				}
				switch (Function.ToString(cbMonth.SelectedItem))
				{
				case "January":
					dr["Month"] = 1;
					break;
				case "February":
					dr["Month"] = 2;
					break;
				case "March":
					dr["Month"] = 3;
					break;
				case "April":
					dr["Month"] = 4;
					break;
				case "May":
					dr["Month"] = 5;
					break;
				case "June":
					dr["Month"] = 6;
					break;
				case "July":
					dr["Month"] = 7;
					break;
				case "August":
					dr["Month"] = 8;
					break;
				case "September":
					dr["Month"] = 9;
					break;
				case "October":
					dr["Month"] = 10;
					break;
				case "November":
					dr["Month"] = 11;
					break;
				case "December":
					dr["Month"] = 12;
					break;
				}
				switch ((cbMonth1.SelectedItem != null) ? cbMonth.SelectedItem.ToString() : string.Empty)
				{
				case "January":
					dr["Month"] = 1;
					break;
				case "February":
					dr["Month"] = 2;
					break;
				case "March":
					dr["Month"] = 3;
					break;
				case "April":
					dr["Month"] = 4;
					break;
				case "May":
					dr["Month"] = 5;
					break;
				case "June":
					dr["Month"] = 6;
					break;
				case "July":
					dr["Month"] = 7;
					break;
				case "August":
					dr["Month"] = 8;
					break;
				case "September":
					dr["Month"] = 9;
					break;
				case "October":
					dr["Month"] = 10;
					break;
				case "November":
					dr["Month"] = 11;
					break;
				case "December":
					dr["Month"] = 12;
					break;
				}
				string strstartTime = Function.ToString(dr["BeginTime"]);
				int startTime = Function.ParseInt(dr["BeginTime"]);
				DateTime seStartTime = Function.ParseDateTimes(dr["SeStartDat"]);
				DataTable dtParam = new DataTable
				{
					Columns = 
					{
						{
							"DateChanged",
							typeof(string)
						},
						{
							"TimeChanged",
							typeof(string)
						},
						{
							"NewRecurrenceInfo",
							typeof(string)
						}
					}
				};
				if (CallFromCal && clgCode != 0 && saveType == 1 && recurPat != "N")
				{
					DataRow drow = dtParam.NewRow();
					if (Function.ParseInt(dr["SeriesNum"]) != 0 && (await _httpClient.SubActivities.GetActivityByCode(Function.ParseInt(dr["SeriesNum"]), GlobalData.UserCodeLogin)).IsNotNull())
					{
						RecurrenceInfo rInfoNew = new RecurrenceInfo();
						if (!string.IsNullOrEmpty(Function.ToString(dr["RecurrenceInfo"])))
						{
							rInfoNew.FromXml(Function.ToString(dr["RecurrenceInfo"]));
							if (Function.ParseBool(dr["AllDayEvent"]))
							{
								rInfoNew.Start = Function.ParseDateTimes(dr["SeStartDat"]).Date;
							}
							else if (strstartTime.Length >= 2)
							{
								rInfoNew.Start = seStartTime.Date.Add(new TimeSpan(startTime / 100, Function.ParseInt(strstartTime.Substring(strstartTime.Length - 2, 2)), 0));
							}
							else
							{
								rInfoNew.Start = seStartTime.Date.Add(new TimeSpan(0, Function.ParseInt(strstartTime), 0));
							}
						}
						DateTime recontact = default;
						DateTime seEnd = default;
						int oldStartTime = 0;
						int oldEndTime = 0;
						if (DataRowFromCal != null)
						{
							recontact = Function.ParseDateTimes(DataRowFromCal["Recontact"]);
							seEnd = Function.ParseDateTimes(DataRowFromCal["EndDate"]);
							oldStartTime = Function.ParseInt(DataRowFromCal["BeginTime"]);
							oldEndTime = Function.ParseInt(DataRowFromCal["ENDTime"]);
						}
						else
						{
							DataTable dataTable3 = await _httpClient.SubActivities.GetActivityByCode(clgCode, GlobalData.UserCodeLogin);
							if (dataTable3.IsNotNull())
							{
								recontact = Function.ParseDateTimes(dataTable3.Rows[0]["Recontact"]);
								seEnd = Function.ParseDateTimes(dataTable3.Rows[0]["EndDate"]);
								oldStartTime = Function.ParseInt(dataTable3.Rows[0]["BeginTime"]);
								oldEndTime = Function.ParseInt(dataTable3.Rows[0]["ENDTime"]);
							}
						}
						if (recontact.Date != Function.ParseDateTimes(dr["Recontact"]).Date || seEnd.Date != Function.ParseDateTimes(dr["EndDate"]).Date)
						{
							drow["DateChanged"] = "Y";
						}
						if (oldStartTime != Function.ParseInt(dr["BeginTime"]) || oldEndTime != Function.ParseInt(dr["ENDTime"]))
						{
							drow["TimeChanged"] = "Y";
						}
						if (Function.ToString(drow["DateChanged"]) == "Y" || Function.ToString(drow["TimeChanged"]) == "Y")
						{
							RecurrenceInfoXmlPersistenceHelper recurrenceInfoXmlPersistenceHelper = new RecurrenceInfoXmlPersistenceHelper(rInfoNew, DateTimeSavingMode.Appointment);
							drow["NewRecurrenceInfo"] = recurrenceInfoXmlPersistenceHelper.ToXml();
						}
						dtParam.Rows.Add(drow);
						dtParam.AcceptChanges();
					}
				}
				switch (type)
				{
				case AppointmentType.Pattern:
				case AppointmentType.Occurrence:
				{
					rInfo.AllDay = Function.ParseBool(dr["AllDayEvent"]);
					if (Function.ParseBool(dr["AllDayEvent"]))
					{
						rInfo.Start = Function.ParseDateTimes(dr["SeStartDat"]).Date;
					}
					else if (strstartTime.Length >= 2)
					{
						rInfo.Start = seStartTime.Date.Add(new TimeSpan(startTime / 100, Function.ParseInt(strstartTime.Substring(strstartTime.Length - 2, 2)), 0));
					}
					else
					{
						rInfo.Start = seStartTime.Date.Add(new TimeSpan(0, Function.ParseInt(strstartTime), 0));
					}
					if (Function.ParseInt(dr["MaxOccur"]) > 0)
					{
						rInfo.OccurrenceCount = Function.ParseInt(dr["MaxOccur"]);
					}
					if (Function.ParseInt(dr["Interval"]) > 0)
					{
						rInfo.Periodicity = Function.ParseInt(dr["Interval"]);
					}
					if (Function.ParseInt(dr["DayInMonth"]) > 0)
					{
						rInfo.DayNumber = Function.ParseInt(dr["DayInMonth"]);
					}
					if (Function.ParseInt(dr["Month"]) > 0)
					{
						rInfo.Month = Function.ParseInt(dr["Month"]);
					}
					switch (Function.ToString(dr["RecurPat"]))
					{
					case "D":
						rInfo.Type = RecurrenceType.Daily;
						switch (Function.ParseInt(dr["SubOption"]))
						{
						case 1:
							rInfo.WeekDays = WeekDays.EveryDay;
							break;
						case 2:
							rInfo.WeekDays = WeekDays.WorkDays;
							break;
						}
						break;
					case "W":
						rInfo.Type = RecurrenceType.Weekly;
						if (Function.ToString(dr["Sunday"]) == "Y")
						{
							rInfo.WeekDays |= WeekDays.Sunday;
						}
						if (Function.ToString(dr["Monday"]) == "Y")
						{
							rInfo.WeekDays |= WeekDays.Monday;
						}
						if (Function.ToString(dr["Tuesday"]) == "Y")
						{
							rInfo.WeekDays |= WeekDays.Tuesday;
						}
						if (Function.ToString(dr["Wednesday"]) == "Y")
						{
							rInfo.WeekDays |= WeekDays.Wednesday;
						}
						if (Function.ToString(dr["Thursday"]) == "Y")
						{
							rInfo.WeekDays |= WeekDays.Thursday;
						}
						if (Function.ToString(dr["Friday"]) == "Y")
						{
							rInfo.WeekDays |= WeekDays.Friday;
						}
						if (Function.ToString(dr["Saturday"]) == "Y")
						{
							rInfo.WeekDays |= WeekDays.Saturday;
						}
						break;
					case "M":
						rInfo.Type = RecurrenceType.Monthly;
						if (Function.ParseInt(dr["SubOption"]) == 2)
						{
							SetWeekDays(dr, rInfo);
						}
						break;
					case "A":
						rInfo.Type = RecurrenceType.Yearly;
						if (Function.ParseInt(dr["SubOption"]) == 2)
						{
							SetWeekDays(dr, rInfo);
						}
						break;
					default:
						rInfo.Type = RecurrenceType.Daily;
						break;
					case "N":
						break;
					}
					switch (Function.ToString(dr["EndType"]))
					{
					case "N":
						rInfo.Range = RecurrenceRange.NoEndDate;
						break;
					case "C":
						rInfo.Range = RecurrenceRange.OccurrenceCount;
						break;
					case "D":
					{
						rInfo.Range = RecurrenceRange.EndByDate;
						string text = Function.ToString(dr["ENDTime"]);
						if (text.Length >= 2)
						{
							rInfo.End = Function.ParseDateTimes(dr["SeEndDat"]).Date.Add(new TimeSpan(Function.ParseInt(dr["ENDTime"]) / 100, Function.ParseInt(text.Substring(text.Length - 2, 2)), 0));
						}
						else
						{
							rInfo.End = Function.ParseDateTimes(dr["SeEndDat"]).Date.Add(new TimeSpan(0, Function.ParseInt(text), 0));
						}
						break;
					}
					default:
						rInfo.Range = RecurrenceRange.NoEndDate;
						break;
					}
					switch (Function.ParseInt(dr["Week"]))
					{
					case 1:
						rInfo.WeekOfMonth = WeekOfMonth.First;
						break;
					case 2:
						rInfo.WeekOfMonth = WeekOfMonth.Second;
						break;
					case 3:
						rInfo.WeekOfMonth = WeekOfMonth.Third;
						break;
					case 4:
						rInfo.WeekOfMonth = WeekOfMonth.Fourth;
						break;
					case 5:
						rInfo.WeekOfMonth = WeekOfMonth.Last;
						break;
					default:
						rInfo.WeekOfMonth = WeekOfMonth.None;
						break;
					}
					if (rInfo.Range == RecurrenceRange.OccurrenceCount)
					{
						dr["SeEndDat"] = (rInfo.End = OccurrenceCalculator.CreateInstance(rInfo).CalcOccurrenceStartTime(rInfo.OccurrenceCount - 1)).Date;
					}
					else if (rInfo.Range == RecurrenceRange.NoEndDate)
					{
						dr["SeEndDat"] = DBNull.Value;
					}
					RecurrenceInfoXmlPersistenceHelper recurrenceInfoXmlPersistenceHelper2 = new RecurrenceInfoXmlPersistenceHelper(rInfo, DateTimeSavingMode.Appointment);
					dr["RecurrenceInfo"] = recurrenceInfoXmlPersistenceHelper2.ToXml();
					recurrenceInfoXmlPersistenceHelper2.ToXml();
					break;
				}
				case AppointmentType.ChangedOccurrence:
				case AppointmentType.DeletedOccurrence:
				{
					PatternReferenceXmlPersistenceHelper patternReferenceXmlPersistenceHelper = new PatternReferenceXmlPersistenceHelper(new PatternReference(rInfo.Id, RecurIndex));
					dr["RecurrenceInfo"] = patternReferenceXmlPersistenceHelper.ToXml();
					patternReferenceXmlPersistenceHelper.ToXml();
					break;
				}
				default:
					dr["RecurrenceInfo"] = "";
					break;
				}
				dr["CntctType"] = type;
				ResourceIdCollection resourceIdCollection = new ResourceIdCollection();
				SchedulerCompatibility.Base64XmlObjectSerialization = false;
				if (Function.ToString(cbUser.EditValue) == "User")
				{
					if (chk_InviteAll.CheckState == CheckState.Checked)
					{
						_dtCurrentRow.Rows[0]["GroupUser"] = DBNull.Value;
						resourceIdCollection.Clear();
					}
					else
					{
						string text2 = Function.ToString(cfl_GroupUser.EditValue);
						if (!string.IsNullOrEmpty(text2))
						{
							string[] array = Function.ToString(text2).Split(';');
							foreach (string text3 in array)
							{
								if (!string.IsNullOrWhiteSpace(text3))
								{
									resourceIdCollection.Add("U" + Function.ToString(text3));
								}
							}
						}
					}
					if (chk_ShareAll.CheckState == CheckState.Checked)
					{
						_dtCurrentRow.Rows[0]["UserSharing"] = DBNull.Value;
						resourceIdCollection.Clear();
					}
					else
					{
						string text4 = Function.ToString(cfl_UserSharing.EditValue);
						if (!string.IsNullOrEmpty(text4))
						{
							string[] array = Function.ToString(text4).Split(';');
							foreach (string text5 in array)
							{
								resourceIdCollection.Add("U" + Function.ToString(text5));
							}
						}
					}
				}
				else if (Function.ToString(cbUser.EditValue) == "Employee")
				{
					if (chk_InviteAllEmp.CheckState == CheckState.Checked)
					{
						_dtCurrentRow.Rows[0]["GroupEmployee"] = DBNull.Value;
						resourceIdCollection.Clear();
					}
					else
					{
						string text2 = Function.ToString(cfl_GroupEmployee.EditValue);
						if (!string.IsNullOrEmpty(text2))
						{
							string[] array = Function.ToString(text2).Split(';');
							foreach (string text6 in array)
							{
								if (!string.IsNullOrWhiteSpace(text6))
								{
									resourceIdCollection.Add("E" + Function.ToString(text6));
								}
							}
						}
					}
					if (chk_ShareAllEmp.CheckState == CheckState.Checked)
					{
						_dtCurrentRow.Rows[0]["EmplSharing"] = DBNull.Value;
						resourceIdCollection.Clear();
					}
					else
					{
						string text4 = Function.ToString(cfl_EmplSharing.EditValue);
						if (!string.IsNullOrEmpty(text4))
						{
							string[] array = Function.ToString(text4).Split(';');
							foreach (string text7 in array)
							{
								resourceIdCollection.Add("E" + Function.ToString(text7));
							}
						}
					}
				}
				string value3 = new AppointmentResourceIdCollectionXmlPersistenceHelper(resourceIdCollection).ToXml();
				_dtCurrentRow.Rows[0]["ResourceIdsInfo"] = value3;
				Appointment appointment = StaticAppointmentFactory.CreateAppointment(type);
				if (Function.ToString(_dtCurrentRow.Rows[0]["Reminder"]) == "Y")
				{
					appointment.HasReminder = true;
				}
				if (_dtCurrentRow.Rows[0]["Recontact"] != DBNull.Value)
				{
					appointment.Start = Function.ParseDateTimes(_dtCurrentRow.Rows[0]["Recontact"]);
				}
				if (_dtCurrentRow.Rows[0]["endDate"] != DBNull.Value)
				{
					appointment.End = Function.ParseDateTimes(_dtCurrentRow.Rows[0]["endDate"]);
				}
				if (appointment.IsRecurring)
				{
					appointment.RecurrenceInfo.Assign(rInfo);
				}
				if (appointment.HasReminder)
				{
					appointment.Reminders.Clear();
					Reminder reminder = appointment.CreateNewReminder();
					switch (Function.ToString(_dtCurrentRow.Rows[0]["RemType"]))
					{
					case "M":
						reminder.TimeBeforeStart = TimeSpan.FromMinutes(Function.ParseInt(_dtCurrentRow.Rows[0]["RemQty"]));
						break;
					case "D":
						reminder.TimeBeforeStart = TimeSpan.FromDays(Function.ParseInt(_dtCurrentRow.Rows[0]["RemQty"]));
						break;
					case "H":
						reminder.TimeBeforeStart = TimeSpan.FromHours(Function.ParseInt(_dtCurrentRow.Rows[0]["RemQty"]));
						break;
					}
					appointment.Reminders.Add(reminder);
					string value4 = ReminderCollectionXmlPersistenceHelper.CreateSaveInstance(appointment).ToXml();
					_dtCurrentRow.Rows[0]["ReminderInfo"] = value4;
				}
				DataSet dataSet = new DataSet();
				dataSet.Tables.Add(_dtCurrentRow.Copy());
				dataSet.Tables.Add(dtParam);
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
				HttpResult httpResult;
				if (clgCode <= 0)
				{
					httpResult = await _httpClient.SubActivities.AddEvent(dataSet);
				}
				else
				{
					httpResult = ((saveType != 1) ? (await _httpClient.SubActivities.SaveThisEvent(dataSet)) : (await _httpClient.SubActivities.SaveAllEvent(dataSet)));
				}
				if (string.IsNullOrEmpty(httpResult.Message))
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
					FormMode = Enums.FormViewModeType.View;
					_isLoading = true;
					ucComment1.Enabled = true;
					ucAttachment1.EnableButtons(isEnabled: true);
					string[] listContent = Function.ToString(httpResult.Content).Split(',');
					if (listContent.Length > 1)
					{
						int num3 = Function.ParseInt(listContent[1]);
						if (dtAtt != null && num3 != 0)
						{
							_dtCurrentRow.Rows[0]["AtcEntry"] = num3;
							_dtCurrentRow.AcceptChanges();
							foreach (DataRow item2 in ucAttachment1.grd_File.DataSourceTable.AsEnumerable())
							{
								item2["AbsEntry"] = num3;
							}
							ucAttachment1.grd_File.DataSourceTable.AcceptChanges();
						}
					}
					if (clgCode <= 0)
					{
						_dtCurrentRow.Rows[0]["ClgCode"] = Function.ParseInt(listContent[0]);
						if (!CallFromCal && !CallFromRelated)
						{
							_isChanging = true;
							gv_Doc.SetFocusedRowCellValue("ClgCode", Function.ParseInt(listContent[0]));
							_isChanging = false;
							PostDataBackToGridSource(_dtCurrentRow);
							AddNewRow();
							int rowHandle = gv_Doc.GetRowHandle(grd_Doc.DataSourceTable.Rows.Count - 1);
							gv_Doc.FocusedRowHandle = rowHandle;
						}
					}
					else
					{
						if (!CallFromCal && !CallFromRelated)
						{
							PostDataBackToGridSource(_dtCurrentRow);
						}
						btn_SendMail.Enabled = true;
					}
					if (type == AppointmentType.DeletedOccurrence)
					{
						await CalAddNew();
					}
					AptType = type;
					OrigRecurXml = "";
					ClgCode = Function.ParseInt(listContent[0]);
					if (CallFromCal)
					{
						DataRowFromCal["Recontact"] = Function.ParseDateTimes(_dtCurrentRow.Rows[0]["Recontact"]);
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
				WinCommons._CloseProcessing();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			_isLoading = false;
			WinCommons._CloseProcessing();
		}
		return string.Empty;
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

	private void SetEventControl(Control ctr)
	{
		foreach (Control control in ctr.Controls)
		{
			if (control is CheckEdit checkEdit)
			{
				checkEdit.CheckStateChanged += OnContentChanged;
				checkEdit.Validated += OnValidated;
			}
			else if (control is ComboBoxEdit comboBoxEdit)
			{
				comboBoxEdit.SelectedIndexChanged += OnContentChanged;
				comboBoxEdit.SelectedValueChanged += OnContentChanged;
				comboBoxEdit.EditValueChanged += OnContentChanged;
				comboBoxEdit.Validated += OnValidated;
			}
			else if (control is LookUpEdit lookUpEdit)
			{
				lookUpEdit.EditValueChanged += OnContentChanged;
				lookUpEdit.Properties.ShowHeader = false;
				lookUpEdit.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
				lookUpEdit.Validated += OnValidated;
			}
			else if (control is DateEdit dateEdit)
			{
				dateEdit.EditValueChanged += OnContentChanged;
				dateEdit.Validated += OnValidated;
			}
			else if (control is TextEdit || control is MemoEdit)
			{
				control.TextChanged += OnContentChanged;
				control.Validated += OnValidated;
			}
			else if (control is SeparatorComboBox)
			{
				((SeparatorComboBox)control).SelectedIndexChanged += OnContentChanged;
				((SeparatorComboBox)control).Validated += OnValidated;
			}
			else if (control is RadioButton)
			{
				((RadioButton)control).CheckedChanged += OnContentChanged;
				((RadioButton)control).Validated += OnValidated;
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
			if ((!(sender is DateEdit dateEdit) || (!(dateEdit.Name == "date_From") && !(dateEdit.Name == "date_To"))) && !(sender is BaseEdit { Name: "memoComment" }) && !_isLoading && (!CallFromCal || (CallFromCal && ClgCode != 0) || !CallFromRelated) && FormMode != Enums.FormViewModeType.Add)
			{
				FormMode = Enums.FormViewModeType.Update;
				btn_SendMail.Enabled = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected void OnValidated(object sender, EventArgs e)
	{
	}

	public async Task REFRESH()
	{
		try
		{
			if (!CallFromCal && !CallFromRelated)
			{
				DataTable dataSourceTable = grd_Doc.DataSourceTable;
				int dataSourceIndex = dataSourceTable.Rows.IndexOf(dataSourceTable.Rows.Find(0));
				gv_Doc.FocusedRowHandle = gv_Doc.GetRowHandle(dataSourceIndex);
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
		btnSearch.Enabled = true;
		DateEditEx dateEditEx = date_To;
		bool readOnly = (date_From.ReadOnly = false);
		dateEditEx.ReadOnly = readOnly;
		ucAttachment1.btDisplay.Enabled = true;
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
				text = Function.ToString(gv_Doc.GetFocusedRowCellValue("ClgCode"));
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Service.frm_EventManagement));
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions5 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions6 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions7 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions8 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions9 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance11 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered11 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed11 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled11 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions10 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance12 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered12 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed12 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled12 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance13 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered13 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed13 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled13 = new DevExpress.Utils.SerializableAppearanceObject();
		this.rep_Deparment = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.btn_Remove = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.duplicateRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx7 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Bp = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx3 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemDateEdit1 = new Apzon.Controls.Editor.RepositoryItemDateEditEx();
		this.gridColumnEx4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx5 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gc_AssignTo = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx6 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx8 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemHourEdit1 = new Apzon.Controls.Controls.RepositoryItemHourEdit();
		this.gridColumnEx9 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx10 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemDateEditEx1 = new Apzon.Controls.Editor.RepositoryItemDateEditEx();
		this.rep_Invite = new DevExpress.XtraEditors.Repository.RepositoryItemCheckedComboBoxEdit();
		this.toolTipController1 = new DevExpress.Utils.ToolTipController(this.components);
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.btnSearch = new Apzon.Controls.Controls.SimpleButtonEx();
		this.date_To = new Apzon.Controls.Editor.DateEditEx();
		this.lbl_ValidTo = new Apzon.Controls.Controls.LabelLine();
		this.date_From = new Apzon.Controls.Editor.DateEditEx();
		this.lbl_ValidFrom = new Apzon.Controls.Controls.LabelLine();
		this.splitContainerControl3 = new DevExpress.XtraEditors.SplitContainerControl();
		this.cfl_Opportunity = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.txtOwner = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.label_MettingLocation = new Apzon.Controls.Controls.LabelLine();
		this.lbOwner = new Apzon.Controls.Controls.LabelLine();
		this.luk_Meetting = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.cfl_UserSharing = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.lb_DocumentType = new Apzon.Controls.Controls.LabelLine();
		this.cfl_GroupEmployee = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.lb_DocumentNumber = new Apzon.Controls.Controls.LabelLine();
		this.lb_linkedDoc = new Apzon.Controls.Controls.LabelLine();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.lb_BPCode = new Apzon.Controls.Controls.LabelLine();
		this.txt_BPName = new Apzon.Controls.Editor.TextEditEx();
		this.lb_StartTime = new Apzon.Controls.Controls.LabelLine();
		this.txt_BPType = new Apzon.Controls.Editor.TextEditEx();
		this.lb_EndTime = new Apzon.Controls.Controls.LabelLine();
		this.chk_ShareAllEmp = new Apzon.Controls.Editor.CheckEditEx();
		this.lb_Invite = new Apzon.Controls.Controls.LabelLine();
		this.chk_InviteAllEmp = new Apzon.Controls.Editor.CheckEditEx();
		this.lb_Remarks = new Apzon.Controls.Controls.LabelLine();
		this.txtDetails = new Apzon.Controls.Editor.TextEditEx();
		this.chk_ShareAll = new Apzon.Controls.Editor.CheckEditEx();
		this.chkAllday = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_InviteAll = new Apzon.Controls.Editor.CheckEditEx();
		this.txtBeginTime = new Apzon.Controls.Controls.HourEdit();
		this.cfl_EmplSharing = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.txtEndTime = new Apzon.Controls.Controls.HourEdit();
		this.dStart_Recontact = new Apzon.Controls.Editor.DateEditEx();
		this.lbl_Share = new Apzon.Controls.Controls.LabelLine();
		this.dEnDate = new Apzon.Controls.Editor.DateEditEx();
		this.cfl_ProjectNew = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.txtBpCode = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.cfl_ProjectWBS = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.ucLinkedDocument1 = new Apzon.Forms.UserControls.ucLinkedDocument();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.cfl_GroupUser = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.bte_DistrRule = new Apzon.Forms.UserControls.ButtonEditDistrRule();
		this.cbUser = new Apzon.Controls.Controls.ComboBoxEditEx();
		this.splitContainerControl2 = new DevExpress.XtraEditors.SplitContainerControl();
		this.wysiwygEdit1 = new Apzon.Forms.UserControls.WysiwygEdit();
		this.xtraTabControl1 = new Apzon.Controls.Controls.XtraTabControlEx();
		this.tab_Recur = new Apzon.Controls.Controls.XtraTabPageEx();
		this.luk_RemType = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.calc_RemQty = new Apzon.Controls.Editor.CalcEditEx();
		this.chkReminder = new Apzon.Controls.Editor.CheckEditEx();
		this.panel_Range = new System.Windows.Forms.Panel();
		this.lb_End = new Apzon.Controls.Controls.LabelLine();
		this.lb_Start = new Apzon.Controls.Controls.LabelLine();
		this.lb_Occurrence = new Apzon.Controls.Controls.LabelLine();
		this.txtAfterMaxOccur = new Apzon.Controls.Editor.TextEditEx();
		this.radio_By = new System.Windows.Forms.RadioButton();
		this.radio_After = new System.Windows.Forms.RadioButton();
		this.radio_NoEndDate = new System.Windows.Forms.RadioButton();
		this.lb_Range = new Apzon.Controls.Controls.LabelLine();
		this.labelLine32 = new Apzon.Controls.Controls.LabelLine();
		this.dSeStartDat = new Apzon.Controls.Editor.DateEditEx();
		this.txtBySeEndDat = new Apzon.Controls.Editor.DateEditEx();
		this.panel_Daily = new System.Windows.Forms.Panel();
		this.chkSun = new Apzon.Controls.Editor.CheckEditEx();
		this.cbMonth = new Apzon.Controls.Controls.ComboBoxEditEx();
		this.cbMonth1 = new Apzon.Controls.Controls.ComboBoxEditEx();
		this.cbDayOfWeek = new Apzon.Controls.Controls.ComboBoxEditEx();
		this.cbWeek = new Apzon.Controls.Controls.ComboBoxEditEx();
		this.chkSat = new Apzon.Controls.Editor.CheckEditEx();
		this.txtDayInMonth = new Apzon.Controls.Editor.TextEditEx();
		this.radio_SubOption2 = new System.Windows.Forms.RadioButton();
		this.label_RepeatEvery = new System.Windows.Forms.Label();
		this.chkFri = new Apzon.Controls.Editor.CheckEditEx();
		this.txtInterval = new Apzon.Controls.Editor.TextEditEx();
		this.radio_SubOption1 = new System.Windows.Forms.RadioButton();
		this.lb_Of = new Apzon.Controls.Controls.LabelLine();
		this.lb_RepeatOn = new Apzon.Controls.Controls.LabelLine();
		this.chkThu = new Apzon.Controls.Editor.CheckEditEx();
		this.lb_RepeatEvery = new Apzon.Controls.Controls.LabelLine();
		this.chkTue = new Apzon.Controls.Editor.CheckEditEx();
		this.chkWed = new Apzon.Controls.Editor.CheckEditEx();
		this.chkMon = new Apzon.Controls.Editor.CheckEditEx();
		this.cbRecurPat = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.tab_AddInfo = new Apzon.Controls.Controls.XtraTabPageEx();
		this.layout_UDF = new Apzon.Forms.UserControls.LayoutUDF();
		this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
		this.tab_Att = new Apzon.Controls.Controls.XtraTabPageEx();
		this.ucAttachment1 = new Apzon.Forms.UserControls.ucAttachment();
		this.tab_Comment = new Apzon.Controls.Controls.XtraTabPageEx();
		this.ucComment1 = new Apzon.Forms.UserControls.ucComment();
		this.lblCreatedBy = new Apzon.Controls.Controls.LabelLine();
		this.btn_SendMail = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.removeEventToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		((System.ComponentModel.ISupportInitialize)this.rep_Deparment).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Bp).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit1.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemHourEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx1.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Invite).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.date_To.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_To.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_From.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_From.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl3).BeginInit();
		this.splitContainerControl3.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.cfl_Opportunity.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtOwner.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Meetting.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cfl_UserSharing.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cfl_GroupEmployee.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BPName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BPType.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_ShareAllEmp.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_InviteAllEmp.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtDetails.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_ShareAll.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chkAllday.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_InviteAll.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtBeginTime.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cfl_EmplSharing.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtEndTime.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dStart_Recontact.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dStart_Recontact.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dEnDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dEnDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cfl_ProjectNew.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtBpCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cfl_ProjectWBS.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cfl_GroupUser.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bte_DistrRule.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbUser.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl2).BeginInit();
		this.splitContainerControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).BeginInit();
		this.xtraTabControl1.SuspendLayout();
		this.tab_Recur.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.luk_RemType.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calc_RemQty.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chkReminder.Properties).BeginInit();
		this.panel_Range.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txtAfterMaxOccur.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dSeStartDat.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dSeStartDat.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtBySeEndDat.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtBySeEndDat.Properties).BeginInit();
		this.panel_Daily.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.chkSun.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbMonth.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbMonth1.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbDayOfWeek.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbWeek.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chkSat.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtDayInMonth.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chkFri.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtInterval.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chkThu.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chkTue.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chkWed.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chkMon.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbRecurPat.Properties).BeginInit();
		this.tab_AddInfo.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.layout_UDF).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.Root).BeginInit();
		this.tab_Att.SuspendLayout();
		this.tab_Comment.SuspendLayout();
		this.contextMenuStrip2.SuspendLayout();
		base.SuspendLayout();
		this.rep_Deparment.AutoHeight = false;
		this.rep_Deparment.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Deparment.Name = "rep_Deparment";
		this.rep_Deparment.NullText = "";
		this.btn_Remove.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Remove.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Remove.Appearance.Options.UseBackColor = true;
		this.btn_Remove.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Remove.EditControl = null;
		this.btn_Remove.EditValue = "";
		this.btn_Remove.FieldName = null;
		this.btn_Remove.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Remove.ImageOptions.Image");
		this.btn_Remove.IsSearchControl = false;
		this.btn_Remove.Location = new System.Drawing.Point(755, 675);
		this.btn_Remove.Name = "btn_Remove";
		this.btn_Remove.ReadOnly = false;
		this.btn_Remove.SettingInVisible = false;
		this.btn_Remove.SettingReadonly = false;
		this.btn_Remove.Size = new System.Drawing.Size(24, 30);
		this.btn_Remove.TabIndex = 29;
		this.btn_Remove.Click += new System.EventHandler(this.btn_Remove_Click);
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_Doc.Location = new System.Drawing.Point(0, 32);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[6] { this.repositoryItemDateEditEx1, this.repositoryItemCheckEdit1, this.rep_Invite, this.repositoryItemDateEdit1, this.rep_Bp, this.repositoryItemHourEdit1 });
		this.grd_Doc.Size = new System.Drawing.Size(317, 677);
		this.grd_Doc.TabIndex = 4;
		this.grd_Doc.ToolTipController = this.toolTipController1;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.grd_Doc.ProcessGridKey += new System.Windows.Forms.KeyEventHandler(this.grd_Doc_ProcessGridKey);
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.duplicateRowToolStripMenuItem, this.deleteRowToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(151, 48);
		this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
		this.duplicateRowToolStripMenuItem.Name = "duplicateRowToolStripMenuItem";
		this.duplicateRowToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
		this.duplicateRowToolStripMenuItem.Text = "Duplicate Row";
		this.duplicateRowToolStripMenuItem.Click += new System.EventHandler(this.duplicateRowToolStripMenuItem_Click);
		this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
		this.deleteRowToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
		this.deleteRowToolStripMenuItem.Text = "Delete Row";
		this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.deleteRowToolStripMenuItem_Click);
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = "ClgCode";
		this.gv_Doc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[11]
		{
			this.gridColumnEx7, this.gridColumnEx1, this.gridColumnEx2, this.gridColumnEx3, this.gridColumnEx4, this.gridColumnEx5, this.gc_AssignTo, this.gridColumnEx6, this.gridColumnEx8, this.gridColumnEx9,
			this.gridColumnEx10
		});
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = "oEventManagement";
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.IgnoreFilter = false;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 30;
		this.gv_Doc.IsApzTable = false;
		this.gv_Doc.IsInsertNewRow = true;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.IsMappingContextAction = false;
		this.gv_Doc.ItemId = "OEVENT";
		this.gv_Doc.LstColumnAllowBatch = null;
		this.gv_Doc.LstColumnNotAllowBatch = null;
		this.gv_Doc.LstColumnNotEditable = null;
		this.gv_Doc.LstColumNotSetting = null;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsBehavior.AutoExpandAllGroups = true;
		this.gv_Doc.OptionsView.ShowGroupPanel = false;
		this.gv_Doc.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[2]
		{
			new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.gridColumnEx7, DevExpress.Data.ColumnSortOrder.Descending),
			new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.gridColumnEx6, DevExpress.Data.ColumnSortOrder.Descending)
		});
		this.gv_Doc.UseBatchMod = false;
		this.gv_Doc.CustomColumnGroup += new DevExpress.XtraGrid.Views.Base.CustomColumnSortEventHandler(this.gv_Doc_CustomColumnGroup);
		this.gv_Doc.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_Doc_ShowingEditor);
		this.gv_Doc.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gv_Doc_FocusedRowChanged);
		this.gv_Doc.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_Doc_FocusedRowObjectChanged);
		this.gv_Doc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Doc_CellValueChanged);
		this.gv_Doc.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Doc_CellValueChanging);
		this.gv_Doc.BeforeLeaveRow += new DevExpress.XtraGrid.Views.Base.RowAllowEventHandler(this.gv_Doc_BeforeLeaveRow);
		this.gv_Doc.CustomColumnSort += new DevExpress.XtraGrid.Views.Base.CustomColumnSortEventHandler(this.gv_Doc_CustomColumnSort);
		this.gv_Doc.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gv_Doc_CustomUnboundColumnData);
		this.gv_Doc.DoubleClick += new System.EventHandler(this.gv_Doc_DoubleClick);
		this.gv_Doc.ValidatingEditor += new DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventHandler(this.gv_Doc_ValidatingEditor);
		this.gv_Doc.InvalidValueException += new DevExpress.XtraEditors.Controls.InvalidValueExceptionEventHandler(this.gv_Doc_InvalidValueException);
		this.gridColumnEx7.Caption = "ClgCode";
		this.gridColumnEx7.CurrencyDateField = null;
		this.gridColumnEx7.CurrencyField = null;
		this.gridColumnEx7.CurrencyFixed = null;
		this.gridColumnEx7.CurrencyFixedRate = 0.0;
		this.gridColumnEx7.CurrencyRateField = null;
		this.gridColumnEx7.FieldName = "ClgCode";
		this.gridColumnEx7.Name = "gridColumnEx7";
		this.gridColumnEx7.OptionsColumn.ReadOnly = true;
		this.gridColumnEx7.PrimaryKey = false;
		this.gridColumnEx7.SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
		this.gridColumnEx7.UnitField = null;
		this.gridColumnEx1.Caption = "Add Event";
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "Details";
		this.gridColumnEx1.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.PrimaryKey = false;
		this.gridColumnEx1.SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx1.Visible = true;
		this.gridColumnEx1.VisibleIndex = 0;
		this.gridColumnEx2.Caption = "BP Code";
		this.gridColumnEx2.ColumnEdit = this.rep_Bp;
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.FieldName = "CardCode";
		this.gridColumnEx2.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gridColumnEx2.SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
		this.gridColumnEx2.UnitField = null;
		this.gridColumnEx2.Visible = true;
		this.gridColumnEx2.VisibleIndex = 1;
		this.rep_Bp.AutoHeight = false;
		this.rep_Bp.AutoLoadDataSource = true;
		editorButtonImageOptions.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions1.Image");
		this.rep_Bp.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_Bp.ContainerControl = this;
		this.rep_Bp.DisplayMember = "CardCode";
		this.rep_Bp.DisplayMember2 = "";
		this.rep_Bp.LoadDataSourceWhenEnter = true;
		this.rep_Bp.Name = "rep_Bp";
		this.rep_Bp.SeparateCharacterForMulti = ';';
		this.rep_Bp.SeparateString = ", ";
		this.rep_Bp.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.Bp;
		this.rep_Bp.TypeDrillDown = Apzon.Commons.ApzObjectTypes.oBusinessPartners;
		this.rep_Bp.UsingAutoCompleteBox = false;
		this.rep_Bp.UsingDataSource = true;
		this.rep_Bp.ValidateWhenNoDataSource = false;
		this.rep_Bp.ValueMember = "CardCode";
		this.gridColumnEx3.Caption = "From Date";
		this.gridColumnEx3.ColumnEdit = this.repositoryItemDateEdit1;
		this.gridColumnEx3.CurrencyDateField = null;
		this.gridColumnEx3.CurrencyField = null;
		this.gridColumnEx3.CurrencyFixed = null;
		this.gridColumnEx3.CurrencyFixedRate = 0.0;
		this.gridColumnEx3.CurrencyRateField = null;
		this.gridColumnEx3.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gridColumnEx3.FieldName = "Recontact";
		this.gridColumnEx3.GroupInterval = DevExpress.XtraGrid.ColumnGroupInterval.DateRange;
		this.gridColumnEx3.Name = "gridColumnEx3";
		this.gridColumnEx3.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.DateSmart;
		this.gridColumnEx3.PrimaryKey = false;
		this.gridColumnEx3.SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
		this.gridColumnEx3.UnitField = null;
		this.gridColumnEx3.Visible = true;
		this.gridColumnEx3.VisibleIndex = 2;
		this.repositoryItemDateEdit1.AutoHeight = false;
		this.repositoryItemDateEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEdit1.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEdit1.IsSearchControl = false;
		this.repositoryItemDateEdit1.Name = "repositoryItemDateEdit1";
		this.gridColumnEx4.Caption = "To Date";
		this.gridColumnEx4.ColumnEdit = this.repositoryItemDateEdit1;
		this.gridColumnEx4.CurrencyDateField = null;
		this.gridColumnEx4.CurrencyField = null;
		this.gridColumnEx4.CurrencyFixed = null;
		this.gridColumnEx4.CurrencyFixedRate = 0.0;
		this.gridColumnEx4.CurrencyRateField = null;
		this.gridColumnEx4.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gridColumnEx4.FieldName = "endDate";
		this.gridColumnEx4.GroupInterval = DevExpress.XtraGrid.ColumnGroupInterval.DateRange;
		this.gridColumnEx4.Name = "gridColumnEx4";
		this.gridColumnEx4.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.DateSmart;
		this.gridColumnEx4.PrimaryKey = false;
		this.gridColumnEx4.SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
		this.gridColumnEx4.UnitField = null;
		this.gridColumnEx4.Visible = true;
		this.gridColumnEx4.VisibleIndex = 3;
		this.gridColumnEx5.Caption = "All Day";
		this.gridColumnEx5.ColumnEdit = this.repositoryItemCheckEdit1;
		this.gridColumnEx5.CurrencyDateField = null;
		this.gridColumnEx5.CurrencyField = null;
		this.gridColumnEx5.CurrencyFixed = null;
		this.gridColumnEx5.CurrencyFixedRate = 0.0;
		this.gridColumnEx5.CurrencyRateField = null;
		this.gridColumnEx5.FieldName = "AllDayEvent";
		this.gridColumnEx5.Name = "gridColumnEx5";
		this.gridColumnEx5.PrimaryKey = false;
		this.gridColumnEx5.SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
		this.gridColumnEx5.UnitField = null;
		this.gridColumnEx5.Visible = true;
		this.gridColumnEx5.VisibleIndex = 4;
		this.repositoryItemCheckEdit1.AutoHeight = false;
		this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
		this.gc_AssignTo.Caption = "Invite";
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
		this.gc_AssignTo.VisibleIndex = 5;
		this.gridColumnEx6.Caption = "Create Date";
		this.gridColumnEx6.CurrencyDateField = null;
		this.gridColumnEx6.CurrencyField = null;
		this.gridColumnEx6.CurrencyFixed = null;
		this.gridColumnEx6.CurrencyFixedRate = 0.0;
		this.gridColumnEx6.CurrencyRateField = null;
		this.gridColumnEx6.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gridColumnEx6.FieldName = "CntctDate";
		this.gridColumnEx6.GroupInterval = DevExpress.XtraGrid.ColumnGroupInterval.DateRange;
		this.gridColumnEx6.Name = "gridColumnEx6";
		this.gridColumnEx6.OptionsColumn.ReadOnly = true;
		this.gridColumnEx6.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.DateSmart;
		this.gridColumnEx6.PrimaryKey = false;
		this.gridColumnEx6.SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
		this.gridColumnEx6.UnitField = null;
		this.gridColumnEx8.Caption = "Begin Time";
		this.gridColumnEx8.ColumnEdit = this.repositoryItemHourEdit1;
		this.gridColumnEx8.CurrencyDateField = null;
		this.gridColumnEx8.CurrencyField = null;
		this.gridColumnEx8.CurrencyFixed = null;
		this.gridColumnEx8.CurrencyFixedRate = 0.0;
		this.gridColumnEx8.CurrencyRateField = null;
		this.gridColumnEx8.FieldName = "BeginTime";
		this.gridColumnEx8.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx8.Name = "gridColumnEx8";
		this.gridColumnEx8.OptionsColumn.ReadOnly = true;
		this.gridColumnEx8.PrimaryKey = false;
		this.gridColumnEx8.UnitField = null;
		this.repositoryItemHourEdit1.AutoHeight = false;
		this.repositoryItemHourEdit1.IsSearchControl = false;
		this.repositoryItemHourEdit1.MaxLength = 5;
		this.repositoryItemHourEdit1.Name = "repositoryItemHourEdit1";
		this.gridColumnEx9.Caption = "End Time";
		this.gridColumnEx9.ColumnEdit = this.repositoryItemHourEdit1;
		this.gridColumnEx9.CurrencyDateField = null;
		this.gridColumnEx9.CurrencyField = null;
		this.gridColumnEx9.CurrencyFixed = null;
		this.gridColumnEx9.CurrencyFixedRate = 0.0;
		this.gridColumnEx9.CurrencyRateField = null;
		this.gridColumnEx9.FieldName = "ENDTime";
		this.gridColumnEx9.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx9.Name = "gridColumnEx9";
		this.gridColumnEx9.OptionsColumn.ReadOnly = true;
		this.gridColumnEx9.PrimaryKey = false;
		this.gridColumnEx9.UnitField = null;
		this.gridColumnEx10.Caption = "Create Time";
		this.gridColumnEx10.ColumnEdit = this.repositoryItemHourEdit1;
		this.gridColumnEx10.CurrencyDateField = null;
		this.gridColumnEx10.CurrencyField = null;
		this.gridColumnEx10.CurrencyFixed = null;
		this.gridColumnEx10.CurrencyFixedRate = 0.0;
		this.gridColumnEx10.CurrencyRateField = null;
		this.gridColumnEx10.FieldName = "CntctTime";
		this.gridColumnEx10.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx10.Name = "gridColumnEx10";
		this.gridColumnEx10.OptionsColumn.ReadOnly = true;
		this.gridColumnEx10.PrimaryKey = false;
		this.gridColumnEx10.UnitField = null;
		this.repositoryItemDateEditEx1.AutoHeight = false;
		this.repositoryItemDateEditEx1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
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
		this.rep_Invite.AutoHeight = false;
		this.rep_Invite.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Invite.Name = "rep_Invite";
		this.labelLine4.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.labelLine4.Appearance.Options.UseFont = true;
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "WBS";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(366, 156);
		this.labelLine4.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(100, 19);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 114;
		this.toolTipController1.SetToolTip(this.labelLine4, "Work Breakdown Structure");
		this.labelLine4.Utext = "WBS";
		this.splitContainerControl1.CollapsePanel = DevExpress.XtraEditors.SplitCollapsePanel.Panel2;
		this.splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainerControl1.Location = new System.Drawing.Point(0, 0);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.btnSearch);
		this.splitContainerControl1.Panel1.Controls.Add(this.date_To);
		this.splitContainerControl1.Panel1.Controls.Add(this.lbl_ValidTo);
		this.splitContainerControl1.Panel1.Controls.Add(this.date_From);
		this.splitContainerControl1.Panel1.Controls.Add(this.lbl_ValidFrom);
		this.splitContainerControl1.Panel1.Controls.Add(this.grd_Doc);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.AutoScroll = true;
		this.splitContainerControl1.Panel2.Controls.Add(this.splitContainerControl3);
		this.splitContainerControl1.Panel2.Controls.Add(this.lblCreatedBy);
		this.splitContainerControl1.Panel2.Controls.Add(this.btn_SendMail);
		this.splitContainerControl1.Panel2.Controls.Add(this.btn_Remove);
		this.splitContainerControl1.Panel2.Controls.Add(this.btn_Cancel);
		this.splitContainerControl1.Panel2.Controls.Add(this.btn_Save);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(1110, 709);
		this.splitContainerControl1.SplitterPosition = 319;
		this.splitContainerControl1.TabIndex = 94;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.splitContainerControl1.SplitGroupPanelCollapsing += new DevExpress.XtraEditors.SplitGroupPanelCollapsingEventHandler(this.splitContainerControl1_SplitGroupPanelCollapsing);
		this.btnSearch.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btnSearch.Appearance.Options.UseBackColor = true;
		this.btnSearch.EditControl = null;
		this.btnSearch.EditValue = "Search";
		this.btnSearch.FieldName = null;
		this.btnSearch.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnSearch.ImageOptions.Image");
		this.btnSearch.IsSearchControl = true;
		this.btnSearch.Location = new System.Drawing.Point(329, 6);
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
		this.date_To.Location = new System.Drawing.Point(218, 6);
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
		this.lbl_ValidTo.Location = new System.Drawing.Point(177, 9);
		this.lbl_ValidTo.Name = "lbl_ValidTo";
		this.lbl_ValidTo.ReadOnly = false;
		this.lbl_ValidTo.SettingInVisible = false;
		this.lbl_ValidTo.SettingReadonly = false;
		this.lbl_ValidTo.ShowUnderLine = true;
		this.lbl_ValidTo.Size = new System.Drawing.Size(42, 20);
		this.lbl_ValidTo.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_ValidTo.TabIndex = 91;
		this.lbl_ValidTo.Utext = "To";
		this.date_From.EditControl = null;
		this.date_From.EditValue = null;
		this.date_From.FieldName = null;
		this.date_From.Location = new System.Drawing.Point(54, 6);
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
		this.lbl_ValidFrom.Location = new System.Drawing.Point(5, 9);
		this.lbl_ValidFrom.Name = "lbl_ValidFrom";
		this.lbl_ValidFrom.ReadOnly = false;
		this.lbl_ValidFrom.SettingInVisible = false;
		this.lbl_ValidFrom.SettingReadonly = false;
		this.lbl_ValidFrom.ShowUnderLine = true;
		this.lbl_ValidFrom.Size = new System.Drawing.Size(52, 20);
		this.lbl_ValidFrom.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_ValidFrom.TabIndex = 76;
		this.lbl_ValidFrom.Utext = "From";
		this.splitContainerControl3.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl3.Horizontal = false;
		this.splitContainerControl3.Location = new System.Drawing.Point(0, 0);
		this.splitContainerControl3.Name = "splitContainerControl3";
		this.splitContainerControl3.Panel1.AutoScroll = true;
		this.splitContainerControl3.Panel1.Controls.Add(this.cfl_Opportunity);
		this.splitContainerControl3.Panel1.Controls.Add(this.txtOwner);
		this.splitContainerControl3.Panel1.Controls.Add(this.label_MettingLocation);
		this.splitContainerControl3.Panel1.Controls.Add(this.lbOwner);
		this.splitContainerControl3.Panel1.Controls.Add(this.luk_Meetting);
		this.splitContainerControl3.Panel1.Controls.Add(this.cfl_UserSharing);
		this.splitContainerControl3.Panel1.Controls.Add(this.lb_DocumentType);
		this.splitContainerControl3.Panel1.Controls.Add(this.cfl_GroupEmployee);
		this.splitContainerControl3.Panel1.Controls.Add(this.lb_DocumentNumber);
		this.splitContainerControl3.Panel1.Controls.Add(this.lb_linkedDoc);
		this.splitContainerControl3.Panel1.Controls.Add(this.labelLine3);
		this.splitContainerControl3.Panel1.Controls.Add(this.lb_BPCode);
		this.splitContainerControl3.Panel1.Controls.Add(this.txt_BPName);
		this.splitContainerControl3.Panel1.Controls.Add(this.lb_StartTime);
		this.splitContainerControl3.Panel1.Controls.Add(this.txt_BPType);
		this.splitContainerControl3.Panel1.Controls.Add(this.lb_EndTime);
		this.splitContainerControl3.Panel1.Controls.Add(this.chk_ShareAllEmp);
		this.splitContainerControl3.Panel1.Controls.Add(this.lb_Invite);
		this.splitContainerControl3.Panel1.Controls.Add(this.chk_InviteAllEmp);
		this.splitContainerControl3.Panel1.Controls.Add(this.lb_Remarks);
		this.splitContainerControl3.Panel1.Controls.Add(this.txtDetails);
		this.splitContainerControl3.Panel1.Controls.Add(this.chk_ShareAll);
		this.splitContainerControl3.Panel1.Controls.Add(this.chkAllday);
		this.splitContainerControl3.Panel1.Controls.Add(this.chk_InviteAll);
		this.splitContainerControl3.Panel1.Controls.Add(this.txtBeginTime);
		this.splitContainerControl3.Panel1.Controls.Add(this.cfl_EmplSharing);
		this.splitContainerControl3.Panel1.Controls.Add(this.txtEndTime);
		this.splitContainerControl3.Panel1.Controls.Add(this.dStart_Recontact);
		this.splitContainerControl3.Panel1.Controls.Add(this.lbl_Share);
		this.splitContainerControl3.Panel1.Controls.Add(this.dEnDate);
		this.splitContainerControl3.Panel1.Controls.Add(this.cfl_ProjectNew);
		this.splitContainerControl3.Panel1.Controls.Add(this.txtBpCode);
		this.splitContainerControl3.Panel1.Controls.Add(this.cfl_ProjectWBS);
		this.splitContainerControl3.Panel1.Controls.Add(this.ucLinkedDocument1);
		this.splitContainerControl3.Panel1.Controls.Add(this.labelLine4);
		this.splitContainerControl3.Panel1.Controls.Add(this.labelLine1);
		this.splitContainerControl3.Panel1.Controls.Add(this.cfl_GroupUser);
		this.splitContainerControl3.Panel1.Controls.Add(this.labelLine2);
		this.splitContainerControl3.Panel1.Controls.Add(this.bte_DistrRule);
		this.splitContainerControl3.Panel1.Controls.Add(this.cbUser);
		this.splitContainerControl3.Panel1.Text = "Panel1";
		this.splitContainerControl3.Panel2.AutoScroll = true;
		this.splitContainerControl3.Panel2.Controls.Add(this.splitContainerControl2);
		this.splitContainerControl3.Panel2.Text = "Panel2";
		this.splitContainerControl3.Size = new System.Drawing.Size(780, 670);
		this.splitContainerControl3.SplitterPosition = 229;
		this.splitContainerControl3.TabIndex = 127;
		this.cfl_Opportunity.EditControl = null;
		this.cfl_Opportunity.FieldName = "OprId";
		this.cfl_Opportunity.IsSearchControl = false;
		this.cfl_Opportunity.Location = new System.Drawing.Point(466, 103);
		this.cfl_Opportunity.Name = "cfl_Opportunity";
		this.cfl_Opportunity.Properties.Appearance.BackColor = System.Drawing.Color.White;
		this.cfl_Opportunity.Properties.Appearance.Options.UseBackColor = true;
		this.cfl_Opportunity.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions2.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions3.Image");
		this.cfl_Opportunity.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.cfl_Opportunity.Properties.ContainerControl = null;
		this.cfl_Opportunity.Properties.DisplayMember = "Code";
		this.cfl_Opportunity.Properties.DisplayMember2 = "";
		this.cfl_Opportunity.Properties.LoadDataSourceWhenEnter = false;
		this.cfl_Opportunity.Properties.SeparateCharacterForMulti = ';';
		this.cfl_Opportunity.Properties.SeparateString = ", ";
		this.cfl_Opportunity.Properties.UsingAutoCompleteBox = false;
		this.cfl_Opportunity.Properties.UsingDataSource = true;
		this.cfl_Opportunity.Properties.ValidateWhenNoDataSource = false;
		this.cfl_Opportunity.Properties.ValueMember = "AbsEntry";
		this.cfl_Opportunity.SettingInVisible = false;
		this.cfl_Opportunity.SettingReadonly = false;
		this.cfl_Opportunity.Size = new System.Drawing.Size(178, 20);
		this.cfl_Opportunity.TabIndex = 15;
		this.cfl_Opportunity.TabStop = false;
		this.txtOwner.EditControl = null;
		this.txtOwner.FieldName = "Owner";
		this.txtOwner.IsSearchControl = false;
		this.txtOwner.Location = new System.Drawing.Point(119, 202);
		this.txtOwner.Name = "txtOwner";
		this.txtOwner.Properties.AcceptNullValue.Add("0");
		this.txtOwner.Properties.AcceptNullValue.Add("-1");
		this.txtOwner.Properties.AutoHeight = false;
		this.txtOwner.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions3.Image = Apzon.Forms.Properties.Resources.right;
		this.txtOwner.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance4, appearanceHovered4, appearancePressed4, appearanceDisabled4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.txtOwner.Properties.ContainerControl = this;
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
		this.txtOwner.Size = new System.Drawing.Size(185, 20);
		this.txtOwner.TabIndex = 13;
		this.label_MettingLocation.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label_MettingLocation.Appearance.Options.UseFont = true;
		this.label_MettingLocation.EditControl = null;
		this.label_MettingLocation.EditValue = "Metting Location";
		this.label_MettingLocation.FieldName = null;
		this.label_MettingLocation.HideWhenEmpty = false;
		this.label_MettingLocation.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.label_MettingLocation.Location = new System.Drawing.Point(8, 130);
		this.label_MettingLocation.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.label_MettingLocation.Name = "label_MettingLocation";
		this.label_MettingLocation.ReadOnly = false;
		this.label_MettingLocation.SettingInVisible = false;
		this.label_MettingLocation.SettingReadonly = false;
		this.label_MettingLocation.ShowUnderLine = true;
		this.label_MettingLocation.Size = new System.Drawing.Size(110, 19);
		this.label_MettingLocation.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.label_MettingLocation.TabIndex = 46;
		this.label_MettingLocation.Utext = "Metting Location";
		this.lbOwner.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lbOwner.Appearance.Options.UseFont = true;
		this.lbOwner.EditControl = null;
		this.lbOwner.EditValue = "Owner";
		this.lbOwner.FieldName = null;
		this.lbOwner.HideWhenEmpty = false;
		this.lbOwner.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbOwner.Location = new System.Drawing.Point(7, 205);
		this.lbOwner.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.lbOwner.Name = "lbOwner";
		this.lbOwner.ReadOnly = false;
		this.lbOwner.SettingInVisible = false;
		this.lbOwner.SettingReadonly = false;
		this.lbOwner.ShowUnderLine = true;
		this.lbOwner.Size = new System.Drawing.Size(110, 19);
		this.lbOwner.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbOwner.TabIndex = 0;
		this.lbOwner.Utext = "Owner";
		this.luk_Meetting.EditControl = null;
		this.luk_Meetting.FieldName = "Location";
		this.luk_Meetting.IsSearchControl = false;
		this.luk_Meetting.Location = new System.Drawing.Point(119, 127);
		this.luk_Meetting.Name = "luk_Meetting";
		this.luk_Meetting.Properties.AutoHeight = false;
		this.luk_Meetting.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Meetting.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.luk_Meetting.Properties.DisplayMember = "Name";
		this.luk_Meetting.Properties.IsSearchControl = false;
		this.luk_Meetting.Properties.NullText = "";
		this.luk_Meetting.Properties.ValueMember = "Code";
		this.luk_Meetting.SettingInVisible = false;
		this.luk_Meetting.SettingReadonly = false;
		this.luk_Meetting.Size = new System.Drawing.Size(185, 20);
		this.luk_Meetting.TabIndex = 8;
		this.luk_Meetting.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.luk_Metting_EditValueChanging);
		this.cfl_UserSharing.EditControl = null;
		this.cfl_UserSharing.FieldName = "UserSharing";
		this.cfl_UserSharing.IsSearchControl = false;
		this.cfl_UserSharing.Location = new System.Drawing.Point(119, 177);
		this.cfl_UserSharing.Name = "cfl_UserSharing";
		this.cfl_UserSharing.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions4.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions5.Image");
		this.cfl_UserSharing.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, false, true, editorButtonImageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance5, appearanceHovered5, appearancePressed5, appearanceDisabled5, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.cfl_UserSharing.Properties.ContainerControl = null;
		this.cfl_UserSharing.Properties.DisplayMember = "";
		this.cfl_UserSharing.Properties.DisplayMember2 = "";
		this.cfl_UserSharing.Properties.LoadDataSourceWhenEnter = false;
		this.cfl_UserSharing.Properties.LookAndFeel.UseDefaultLookAndFeel = false;
		this.cfl_UserSharing.Properties.SeparateCharacterForMulti = ';';
		this.cfl_UserSharing.Properties.SeparateString = ", ";
		this.cfl_UserSharing.Properties.UsingAutoCompleteBox = false;
		this.cfl_UserSharing.Properties.UsingDataSource = true;
		this.cfl_UserSharing.Properties.ValidateWhenNoDataSource = false;
		this.cfl_UserSharing.Properties.ValueMember = "";
		this.cfl_UserSharing.SettingInVisible = false;
		this.cfl_UserSharing.SettingReadonly = false;
		this.cfl_UserSharing.Size = new System.Drawing.Size(185, 20);
		this.cfl_UserSharing.TabIndex = 12;
		this.lb_DocumentType.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lb_DocumentType.Appearance.Options.UseFont = true;
		this.lb_DocumentType.EditControl = null;
		this.lb_DocumentType.EditValue = "Document Type";
		this.lb_DocumentType.FieldName = null;
		this.lb_DocumentType.HideWhenEmpty = false;
		this.lb_DocumentType.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_DocumentType.Location = new System.Drawing.Point(366, 52);
		this.lb_DocumentType.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.lb_DocumentType.Name = "lb_DocumentType";
		this.lb_DocumentType.ReadOnly = false;
		this.lb_DocumentType.SettingInVisible = false;
		this.lb_DocumentType.SettingReadonly = false;
		this.lb_DocumentType.ShowUnderLine = true;
		this.lb_DocumentType.Size = new System.Drawing.Size(100, 19);
		this.lb_DocumentType.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_DocumentType.TabIndex = 52;
		this.lb_DocumentType.Utext = "Document Type";
		this.cfl_GroupEmployee.EditControl = null;
		this.cfl_GroupEmployee.FieldName = "GroupEmployee";
		this.cfl_GroupEmployee.IsSearchControl = false;
		this.cfl_GroupEmployee.Location = new System.Drawing.Point(119, 153);
		this.cfl_GroupEmployee.Name = "cfl_GroupEmployee";
		this.cfl_GroupEmployee.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions5.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions6.Image");
		this.cfl_GroupEmployee.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, false, true, editorButtonImageOptions5, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance6, appearanceHovered6, appearancePressed6, appearanceDisabled6, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
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
		this.cfl_GroupEmployee.Size = new System.Drawing.Size(185, 20);
		this.cfl_GroupEmployee.TabIndex = 10;
		this.cfl_GroupEmployee.Visible = false;
		this.lb_DocumentNumber.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lb_DocumentNumber.Appearance.Options.UseFont = true;
		this.lb_DocumentNumber.EditControl = null;
		this.lb_DocumentNumber.EditValue = "Document Number";
		this.lb_DocumentNumber.FieldName = null;
		this.lb_DocumentNumber.HideWhenEmpty = false;
		this.lb_DocumentNumber.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_DocumentNumber.Location = new System.Drawing.Point(366, 80);
		this.lb_DocumentNumber.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.lb_DocumentNumber.Name = "lb_DocumentNumber";
		this.lb_DocumentNumber.ReadOnly = false;
		this.lb_DocumentNumber.SettingInVisible = false;
		this.lb_DocumentNumber.SettingReadonly = false;
		this.lb_DocumentNumber.ShowUnderLine = true;
		this.lb_DocumentNumber.Size = new System.Drawing.Size(100, 19);
		this.lb_DocumentNumber.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_DocumentNumber.TabIndex = 51;
		this.lb_DocumentNumber.Utext = "Document Number";
		this.lb_linkedDoc.EditControl = null;
		this.lb_linkedDoc.EditValue = "Linked Document";
		this.lb_linkedDoc.FieldName = null;
		this.lb_linkedDoc.HideWhenEmpty = false;
		this.lb_linkedDoc.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_linkedDoc.Location = new System.Drawing.Point(366, 30);
		this.lb_linkedDoc.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_linkedDoc.Name = "lb_linkedDoc";
		this.lb_linkedDoc.ReadOnly = false;
		this.lb_linkedDoc.SettingInVisible = false;
		this.lb_linkedDoc.SettingReadonly = false;
		this.lb_linkedDoc.ShowUnderLine = true;
		this.lb_linkedDoc.Size = new System.Drawing.Size(100, 19);
		this.lb_linkedDoc.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_linkedDoc.TabIndex = 55;
		this.lb_linkedDoc.Utext = "Linked Document";
		this.labelLine3.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.labelLine3.Appearance.Options.UseFont = true;
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "Linked Opportunity";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(366, 105);
		this.labelLine3.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(100, 19);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 126;
		this.labelLine3.Utext = "Linked Opportunity";
		this.lb_BPCode.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lb_BPCode.Appearance.Options.UseFont = true;
		this.lb_BPCode.EditControl = null;
		this.lb_BPCode.EditValue = "BP Code";
		this.lb_BPCode.FieldName = null;
		this.lb_BPCode.HideWhenEmpty = false;
		this.lb_BPCode.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_BPCode.Location = new System.Drawing.Point(7, 31);
		this.lb_BPCode.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.lb_BPCode.Name = "lb_BPCode";
		this.lb_BPCode.ReadOnly = false;
		this.lb_BPCode.SettingInVisible = false;
		this.lb_BPCode.SettingReadonly = false;
		this.lb_BPCode.ShowUnderLine = true;
		this.lb_BPCode.Size = new System.Drawing.Size(110, 19);
		this.lb_BPCode.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_BPCode.TabIndex = 95;
		this.lb_BPCode.Utext = "BP Code";
		this.txt_BPName.EditControl = null;
		this.txt_BPName.FieldName = null;
		this.txt_BPName.IsSearchControl = false;
		this.txt_BPName.Location = new System.Drawing.Point(119, 54);
		this.txt_BPName.Name = "txt_BPName";
		this.txt_BPName.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.txt_BPName.Properties.ReadOnly = true;
		this.txt_BPName.SettingInVisible = false;
		this.txt_BPName.SettingReadonly = false;
		this.txt_BPName.Size = new System.Drawing.Size(185, 18);
		this.txt_BPName.TabIndex = 124;
		this.txt_BPName.TabStop = false;
		this.lb_StartTime.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lb_StartTime.Appearance.Options.UseFont = true;
		this.lb_StartTime.EditControl = null;
		this.lb_StartTime.EditValue = "Start Time";
		this.lb_StartTime.FieldName = null;
		this.lb_StartTime.HideWhenEmpty = false;
		this.lb_StartTime.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_StartTime.Location = new System.Drawing.Point(8, 80);
		this.lb_StartTime.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.lb_StartTime.Name = "lb_StartTime";
		this.lb_StartTime.ReadOnly = false;
		this.lb_StartTime.SettingInVisible = false;
		this.lb_StartTime.SettingReadonly = false;
		this.lb_StartTime.ShowUnderLine = true;
		this.lb_StartTime.Size = new System.Drawing.Size(110, 19);
		this.lb_StartTime.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_StartTime.TabIndex = 102;
		this.lb_StartTime.Utext = "Start Time";
		this.txt_BPType.EditControl = null;
		this.txt_BPType.FieldName = null;
		this.txt_BPType.IsSearchControl = false;
		this.txt_BPType.Location = new System.Drawing.Point(230, 30);
		this.txt_BPType.Name = "txt_BPType";
		this.txt_BPType.Properties.AllowFocused = false;
		this.txt_BPType.Properties.Appearance.BackColor = System.Drawing.Color.WhiteSmoke;
		this.txt_BPType.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txt_BPType.Properties.Appearance.Options.UseBackColor = true;
		this.txt_BPType.Properties.Appearance.Options.UseFont = true;
		this.txt_BPType.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.txt_BPType.Properties.ReadOnly = true;
		this.txt_BPType.SettingInVisible = false;
		this.txt_BPType.SettingReadonly = false;
		this.txt_BPType.Size = new System.Drawing.Size(74, 16);
		this.txt_BPType.TabIndex = 123;
		this.txt_BPType.TabStop = false;
		this.lb_EndTime.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lb_EndTime.Appearance.Options.UseFont = true;
		this.lb_EndTime.EditControl = null;
		this.lb_EndTime.EditValue = "End Time";
		this.lb_EndTime.FieldName = null;
		this.lb_EndTime.HideWhenEmpty = false;
		this.lb_EndTime.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_EndTime.Location = new System.Drawing.Point(7, 106);
		this.lb_EndTime.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.lb_EndTime.Name = "lb_EndTime";
		this.lb_EndTime.ReadOnly = false;
		this.lb_EndTime.SettingInVisible = false;
		this.lb_EndTime.SettingReadonly = false;
		this.lb_EndTime.ShowUnderLine = true;
		this.lb_EndTime.Size = new System.Drawing.Size(110, 19);
		this.lb_EndTime.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_EndTime.TabIndex = 101;
		this.lb_EndTime.Utext = "End Time";
		this.chk_ShareAllEmp.EditControl = null;
		this.chk_ShareAllEmp.FieldName = "AllEmplShare";
		this.chk_ShareAllEmp.IsSearchControl = false;
		this.chk_ShareAllEmp.Location = new System.Drawing.Point(307, 177);
		this.chk_ShareAllEmp.Name = "chk_ShareAllEmp";
		this.chk_ShareAllEmp.Properties.Caption = "All";
		this.chk_ShareAllEmp.Properties.ValueChecked = "Y";
		this.chk_ShareAllEmp.Properties.ValueUnchecked = "N";
		this.chk_ShareAllEmp.SettingInVisible = false;
		this.chk_ShareAllEmp.SettingReadonly = false;
		this.chk_ShareAllEmp.Size = new System.Drawing.Size(51, 20);
		this.chk_ShareAllEmp.TabIndex = 13;
		this.chk_ShareAllEmp.Visible = false;
		this.lb_Invite.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lb_Invite.Appearance.Options.UseFont = true;
		this.lb_Invite.EditControl = null;
		this.lb_Invite.EditValue = "Invite";
		this.lb_Invite.FieldName = null;
		this.lb_Invite.HideWhenEmpty = false;
		this.lb_Invite.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Invite.Location = new System.Drawing.Point(8, 155);
		this.lb_Invite.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.lb_Invite.Name = "lb_Invite";
		this.lb_Invite.ReadOnly = false;
		this.lb_Invite.SettingInVisible = false;
		this.lb_Invite.SettingReadonly = false;
		this.lb_Invite.ShowUnderLine = true;
		this.lb_Invite.Size = new System.Drawing.Size(35, 19);
		this.lb_Invite.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Invite.TabIndex = 103;
		this.lb_Invite.Utext = "Invite";
		this.chk_InviteAllEmp.EditControl = null;
		this.chk_InviteAllEmp.FieldName = "AllEmplInvite";
		this.chk_InviteAllEmp.IsSearchControl = false;
		this.chk_InviteAllEmp.Location = new System.Drawing.Point(307, 152);
		this.chk_InviteAllEmp.Name = "chk_InviteAllEmp";
		this.chk_InviteAllEmp.Properties.Caption = "All";
		this.chk_InviteAllEmp.Properties.ValueChecked = "Y";
		this.chk_InviteAllEmp.Properties.ValueUnchecked = "N";
		this.chk_InviteAllEmp.SettingInVisible = false;
		this.chk_InviteAllEmp.SettingReadonly = false;
		this.chk_InviteAllEmp.Size = new System.Drawing.Size(51, 20);
		this.chk_InviteAllEmp.TabIndex = 11;
		this.chk_InviteAllEmp.Visible = false;
		this.lb_Remarks.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lb_Remarks.Appearance.Options.UseFont = true;
		this.lb_Remarks.EditControl = null;
		this.lb_Remarks.EditValue = "Subject";
		this.lb_Remarks.FieldName = null;
		this.lb_Remarks.HideWhenEmpty = false;
		this.lb_Remarks.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Remarks.Location = new System.Drawing.Point(7, 7);
		this.lb_Remarks.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.lb_Remarks.Name = "lb_Remarks";
		this.lb_Remarks.ReadOnly = false;
		this.lb_Remarks.SettingInVisible = false;
		this.lb_Remarks.SettingReadonly = false;
		this.lb_Remarks.ShowUnderLine = true;
		this.lb_Remarks.Size = new System.Drawing.Size(110, 19);
		this.lb_Remarks.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Remarks.TabIndex = 105;
		this.lb_Remarks.Utext = "Subject";
		this.txtDetails.EditControl = null;
		this.txtDetails.FieldName = "Details";
		this.txtDetails.IsSearchControl = false;
		this.txtDetails.Location = new System.Drawing.Point(119, 4);
		this.txtDetails.Name = "txtDetails";
		this.txtDetails.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.txtDetails.Properties.Appearance.Options.UseFont = true;
		this.txtDetails.Properties.AutoHeight = false;
		this.txtDetails.SettingInVisible = false;
		this.txtDetails.SettingReadonly = false;
		this.txtDetails.Size = new System.Drawing.Size(525, 20);
		this.txtDetails.TabIndex = 2;
		this.txtDetails.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.txtDetails_EditValueChanging);
		this.chk_ShareAll.EditControl = null;
		this.chk_ShareAll.FieldName = "AllUserShare";
		this.chk_ShareAll.IsSearchControl = false;
		this.chk_ShareAll.Location = new System.Drawing.Point(307, 177);
		this.chk_ShareAll.Name = "chk_ShareAll";
		this.chk_ShareAll.Properties.Caption = "All";
		this.chk_ShareAll.Properties.ValueChecked = "Y";
		this.chk_ShareAll.Properties.ValueUnchecked = "N";
		this.chk_ShareAll.SettingInVisible = false;
		this.chk_ShareAll.SettingReadonly = false;
		this.chk_ShareAll.Size = new System.Drawing.Size(51, 20);
		this.chk_ShareAll.TabIndex = 13;
		this.chk_ShareAll.CheckStateChanged += new System.EventHandler(this.chk_ShareAll_CheckStateChanged);
		this.chkAllday.EditControl = null;
		this.chkAllday.FieldName = "AllDayEvent";
		this.chkAllday.IsSearchControl = false;
		this.chkAllday.Location = new System.Drawing.Point(310, 77);
		this.chkAllday.Name = "chkAllday";
		this.chkAllday.Properties.Caption = "All day";
		this.chkAllday.SettingInVisible = false;
		this.chkAllday.SettingReadonly = false;
		this.chkAllday.Size = new System.Drawing.Size(55, 20);
		this.chkAllday.TabIndex = 8;
		this.chkAllday.CheckStateChanged += new System.EventHandler(this.chkAllday_CheckStateChanged);
		this.chk_InviteAll.EditControl = null;
		this.chk_InviteAll.FieldName = "AllUserInvite";
		this.chk_InviteAll.IsSearchControl = false;
		this.chk_InviteAll.Location = new System.Drawing.Point(307, 152);
		this.chk_InviteAll.Name = "chk_InviteAll";
		this.chk_InviteAll.Properties.Caption = "All";
		this.chk_InviteAll.Properties.ValueChecked = "Y";
		this.chk_InviteAll.Properties.ValueUnchecked = "N";
		this.chk_InviteAll.SettingInVisible = false;
		this.chk_InviteAll.SettingReadonly = false;
		this.chk_InviteAll.Size = new System.Drawing.Size(51, 20);
		this.chk_InviteAll.TabIndex = 11;
		this.chk_InviteAll.CheckStateChanged += new System.EventHandler(this.chk_InviteAll_CheckStateChanged);
		this.txtBeginTime.EditControl = null;
		this.txtBeginTime.FieldName = "BeginTime";
		this.txtBeginTime.IsSearchControl = false;
		this.txtBeginTime.Location = new System.Drawing.Point(232, 77);
		this.txtBeginTime.Name = "txtBeginTime";
		this.txtBeginTime.Properties.IsSearchControl = false;
		this.txtBeginTime.Properties.MaxLength = 5;
		this.txtBeginTime.SettingInVisible = false;
		this.txtBeginTime.SettingReadonly = false;
		this.txtBeginTime.Size = new System.Drawing.Size(72, 20);
		this.txtBeginTime.TabIndex = 5;
		this.txtBeginTime.Validating += new System.ComponentModel.CancelEventHandler(this.txtBeginTime_Validating);
		this.cfl_EmplSharing.EditControl = null;
		this.cfl_EmplSharing.FieldName = "EmplSharing";
		this.cfl_EmplSharing.IsSearchControl = false;
		this.cfl_EmplSharing.Location = new System.Drawing.Point(119, 177);
		this.cfl_EmplSharing.Name = "cfl_EmplSharing";
		this.cfl_EmplSharing.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions6.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions7.Image");
		this.cfl_EmplSharing.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, false, true, editorButtonImageOptions6, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance7, appearanceHovered7, appearancePressed7, appearanceDisabled7, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.cfl_EmplSharing.Properties.ContainerControl = null;
		this.cfl_EmplSharing.Properties.DisplayMember = "";
		this.cfl_EmplSharing.Properties.DisplayMember2 = "";
		this.cfl_EmplSharing.Properties.LoadDataSourceWhenEnter = false;
		this.cfl_EmplSharing.Properties.LookAndFeel.UseDefaultLookAndFeel = false;
		this.cfl_EmplSharing.Properties.SeparateCharacterForMulti = ';';
		this.cfl_EmplSharing.Properties.SeparateString = ", ";
		this.cfl_EmplSharing.Properties.UsingAutoCompleteBox = false;
		this.cfl_EmplSharing.Properties.UsingDataSource = true;
		this.cfl_EmplSharing.Properties.ValidateWhenNoDataSource = false;
		this.cfl_EmplSharing.Properties.ValueMember = "";
		this.cfl_EmplSharing.SettingInVisible = false;
		this.cfl_EmplSharing.SettingReadonly = false;
		this.cfl_EmplSharing.Size = new System.Drawing.Size(185, 20);
		this.cfl_EmplSharing.TabIndex = 12;
		this.cfl_EmplSharing.Visible = false;
		this.txtEndTime.EditControl = null;
		this.txtEndTime.FieldName = "ENDTime";
		this.txtEndTime.IsSearchControl = false;
		this.txtEndTime.Location = new System.Drawing.Point(232, 103);
		this.txtEndTime.Name = "txtEndTime";
		this.txtEndTime.Properties.IsSearchControl = false;
		this.txtEndTime.Properties.MaxLength = 5;
		this.txtEndTime.SettingInVisible = false;
		this.txtEndTime.SettingReadonly = false;
		this.txtEndTime.Size = new System.Drawing.Size(72, 20);
		this.txtEndTime.TabIndex = 7;
		this.txtEndTime.Validating += new System.ComponentModel.CancelEventHandler(this.txtEndTime_Validating);
		this.dStart_Recontact.EditControl = null;
		this.dStart_Recontact.EditValue = null;
		this.dStart_Recontact.FieldName = "Recontact";
		this.dStart_Recontact.Location = new System.Drawing.Point(119, 77);
		this.dStart_Recontact.Name = "dStart_Recontact";
		this.dStart_Recontact.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
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
		this.dStart_Recontact.Size = new System.Drawing.Size(107, 20);
		this.dStart_Recontact.TabIndex = 4;
		this.dStart_Recontact.EditValueChanged += new System.EventHandler(this.dStart_Recontact_EditValueChanged);
		this.dStart_Recontact.Validating += new System.ComponentModel.CancelEventHandler(this.dStart_Recontact_Validating);
		this.lbl_Share.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lbl_Share.Appearance.Options.UseFont = true;
		this.lbl_Share.EditControl = null;
		this.lbl_Share.EditValue = "Share with";
		this.lbl_Share.FieldName = null;
		this.lbl_Share.HideWhenEmpty = false;
		this.lbl_Share.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_Share.Location = new System.Drawing.Point(8, 180);
		this.lbl_Share.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.lbl_Share.Name = "lbl_Share";
		this.lbl_Share.ReadOnly = false;
		this.lbl_Share.SettingInVisible = false;
		this.lbl_Share.SettingReadonly = false;
		this.lbl_Share.ShowUnderLine = true;
		this.lbl_Share.Size = new System.Drawing.Size(110, 19);
		this.lbl_Share.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_Share.TabIndex = 115;
		this.lbl_Share.Utext = "Share with";
		this.dEnDate.EditControl = null;
		this.dEnDate.EditValue = null;
		this.dEnDate.FieldName = "endDate";
		this.dEnDate.Location = new System.Drawing.Point(119, 103);
		this.dEnDate.Name = "dEnDate";
		this.dEnDate.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
		this.dEnDate.Properties.AutoHeight = false;
		this.dEnDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dEnDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dEnDate.Properties.IsSearchControl = false;
		this.dEnDate.Properties.Mask.EditMask = "";
		this.dEnDate.SettingInVisible = false;
		this.dEnDate.SettingReadonly = false;
		this.dEnDate.Size = new System.Drawing.Size(107, 20);
		this.dEnDate.TabIndex = 6;
		this.dEnDate.EditValueChanged += new System.EventHandler(this.dEnDate_EditValueChanged);
		this.dEnDate.Validating += new System.ComponentModel.CancelEventHandler(this.dEnDate_Validating);
		this.cfl_ProjectNew.EditControl = null;
		this.cfl_ProjectNew.FieldName = "ProjectEntry";
		this.cfl_ProjectNew.IsSearchControl = false;
		this.cfl_ProjectNew.Location = new System.Drawing.Point(466, 128);
		this.cfl_ProjectNew.Name = "cfl_ProjectNew";
		this.cfl_ProjectNew.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions7.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions9.Image");
		this.cfl_ProjectNew.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance8, appearanceHovered8, appearancePressed8, appearanceDisabled8, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, false, true, editorButtonImageOptions7, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance9, appearanceHovered9, appearancePressed9, appearanceDisabled9, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.cfl_ProjectNew.Properties.ContainerControl = null;
		this.cfl_ProjectNew.Properties.DisplayMember = "Code";
		this.cfl_ProjectNew.Properties.DisplayMember2 = "";
		this.cfl_ProjectNew.Properties.LoadDataSourceWhenEnter = false;
		this.cfl_ProjectNew.Properties.SeparateCharacterForMulti = ';';
		this.cfl_ProjectNew.Properties.SeparateString = ", ";
		this.cfl_ProjectNew.Properties.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.ProjectManager;
		this.cfl_ProjectNew.Properties.UsingAutoCompleteBox = false;
		this.cfl_ProjectNew.Properties.UsingDataSource = true;
		this.cfl_ProjectNew.Properties.ValidateWhenNoDataSource = false;
		this.cfl_ProjectNew.Properties.ValueMember = "AbsEntry";
		this.cfl_ProjectNew.SettingInVisible = false;
		this.cfl_ProjectNew.SettingReadonly = false;
		this.cfl_ProjectNew.Size = new System.Drawing.Size(178, 20);
		this.cfl_ProjectNew.TabIndex = 16;
		this.txtBpCode.EditControl = null;
		this.txtBpCode.FieldName = "CardCode";
		this.txtBpCode.IsSearchControl = false;
		this.txtBpCode.Location = new System.Drawing.Point(119, 28);
		this.txtBpCode.Name = "txtBpCode";
		this.txtBpCode.Properties.AutoHeight = false;
		this.txtBpCode.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions8.Image = Apzon.Forms.Properties.Resources.right;
		this.txtBpCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions8, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance10, appearanceHovered10, appearancePressed10, appearanceDisabled10, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.txtBpCode.Properties.ContainerControl = this;
		this.txtBpCode.Properties.DisplayMember = "CardCode";
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
		this.txtBpCode.Size = new System.Drawing.Size(107, 20);
		this.txtBpCode.TabIndex = 3;
		this.txtBpCode.EditValueChanged += new System.EventHandler(this.txtBpCode_EditValueChanged);
		this.cfl_ProjectWBS.EditControl = null;
		this.cfl_ProjectWBS.FieldName = "ProjectWBS";
		this.cfl_ProjectWBS.IsSearchControl = false;
		this.cfl_ProjectWBS.Location = new System.Drawing.Point(466, 153);
		this.cfl_ProjectWBS.Name = "cfl_ProjectWBS";
		this.cfl_ProjectWBS.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions9.Image = Apzon.Forms.Properties.Resources.right;
		this.cfl_ProjectWBS.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, false, true, editorButtonImageOptions9, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance11, appearanceHovered11, appearancePressed11, appearanceDisabled11, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
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
		this.cfl_ProjectWBS.TabIndex = 17;
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
		this.ucLinkedDocument1.Location = new System.Drawing.Point(466, 26);
		this.ucLinkedDocument1.Name = "ucLinkedDocument1";
		this.ucLinkedDocument1.Size = new System.Drawing.Size(178, 77);
		this.ucLinkedDocument1.TabIndex = 14;
		this.ucLinkedDocument1.ValidateInfo = null;
		this.labelLine1.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.labelLine1.Appearance.Options.UseFont = true;
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Distr. Rule";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(366, 180);
		this.labelLine1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(100, 19);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 108;
		this.labelLine1.Utext = "Distr. Rule";
		this.cfl_GroupUser.EditControl = null;
		this.cfl_GroupUser.FieldName = "GroupUser";
		this.cfl_GroupUser.IsSearchControl = false;
		this.cfl_GroupUser.Location = new System.Drawing.Point(119, 153);
		this.cfl_GroupUser.Name = "cfl_GroupUser";
		this.cfl_GroupUser.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions10.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions12.Image");
		this.cfl_GroupUser.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, false, true, editorButtonImageOptions10, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance12, appearanceHovered12, appearancePressed12, appearanceDisabled12, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
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
		this.cfl_GroupUser.Size = new System.Drawing.Size(185, 20);
		this.cfl_GroupUser.TabIndex = 10;
		this.labelLine2.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.labelLine2.Appearance.Options.UseFont = true;
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "Project Code";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(366, 131);
		this.labelLine2.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(100, 19);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 109;
		this.labelLine2.Utext = "Project Code";
		this.bte_DistrRule.EditControl = null;
		this.bte_DistrRule.FieldName = null;
		this.bte_DistrRule.IsSearchControl = false;
		this.bte_DistrRule.Location = new System.Drawing.Point(466, 177);
		this.bte_DistrRule.Name = "bte_DistrRule";
		this.bte_DistrRule.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "Search", -1, true, true, false, imageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance13, appearanceHovered13, appearancePressed13, appearanceDisabled13, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.bte_DistrRule.SettingInVisible = false;
		this.bte_DistrRule.SettingReadonly = false;
		this.bte_DistrRule.Size = new System.Drawing.Size(178, 20);
		this.bte_DistrRule.TabIndex = 18;
		this.cbUser.EditControl = null;
		this.cbUser.EditValue = "User";
		this.cbUser.FieldName = null;
		this.cbUser.IsSearchControl = false;
		this.cbUser.Location = new System.Drawing.Point(45, 152);
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
		this.cbUser.Properties.LookAndFeel.UseDefaultLookAndFeel = false;
		this.cbUser.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.cbUser.SettingInVisible = false;
		this.cbUser.SettingReadonly = false;
		this.cbUser.Size = new System.Drawing.Size(71, 20);
		this.cbUser.TabIndex = 9;
		this.cbUser.SelectedValueChanged += new System.EventHandler(this.cbUser_SelectedValueChanged);
		this.splitContainerControl2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainerControl2.FixedPanel = DevExpress.XtraEditors.SplitFixedPanel.Panel2;
		this.splitContainerControl2.Horizontal = false;
		this.splitContainerControl2.Location = new System.Drawing.Point(0, 0);
		this.splitContainerControl2.Name = "splitContainerControl2";
		this.splitContainerControl2.Panel1.AutoScroll = true;
		this.splitContainerControl2.Panel1.Controls.Add(this.wysiwygEdit1);
		this.splitContainerControl2.Panel1.Text = "Panel1";
		this.splitContainerControl2.Panel2.AutoScroll = true;
		this.splitContainerControl2.Panel2.Controls.Add(this.xtraTabControl1);
		this.splitContainerControl2.Panel2.Text = "Panel2";
		this.splitContainerControl2.Size = new System.Drawing.Size(780, 431);
		this.splitContainerControl2.SplitterPosition = 221;
		this.splitContainerControl2.TabIndex = 122;
		this.splitContainerControl2.Text = "splitContainerControl2";
		this.wysiwygEdit1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.wysiwygEdit1.FieldName = "HtmlNotes";
		this.wysiwygEdit1.Location = new System.Drawing.Point(0, 0);
		this.wysiwygEdit1.Name = "wysiwygEdit1";
		this.wysiwygEdit1.Size = new System.Drawing.Size(780, 200);
		this.wysiwygEdit1.TabIndex = 0;
		this.xtraTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.xtraTabControl1.IsSearchControl = false;
		this.xtraTabControl1.Location = new System.Drawing.Point(0, 0);
		this.xtraTabControl1.Name = "xtraTabControl1";
		this.xtraTabControl1.SelectedTabPage = this.tab_Recur;
		this.xtraTabControl1.Size = new System.Drawing.Size(780, 221);
		this.xtraTabControl1.TabIndex = 19;
		this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[4] { this.tab_Recur, this.tab_AddInfo, this.tab_Att, this.tab_Comment });
		this.xtraTabControl1.SelectedPageChanged += new DevExpress.XtraTab.TabPageChangedEventHandler(this.xtraTabControl1_SelectedPageChanged);
		this.xtraTabControl1.SelectedPageChanging += new DevExpress.XtraTab.TabPageChangingEventHandler(this.xtraTabControl1_SelectedPageChanging);
		this.tab_Recur.Controls.Add(this.luk_RemType);
		this.tab_Recur.Controls.Add(this.calc_RemQty);
		this.tab_Recur.Controls.Add(this.chkReminder);
		this.tab_Recur.Controls.Add(this.panel_Range);
		this.tab_Recur.Controls.Add(this.panel_Daily);
		this.tab_Recur.Controls.Add(this.cbRecurPat);
		this.tab_Recur.EditControl = null;
		this.tab_Recur.EditValue = "Recurrence Info";
		this.tab_Recur.FieldName = null;
		this.tab_Recur.IsSearchControl = false;
		this.tab_Recur.Name = "tab_Recur";
		this.tab_Recur.ReadOnly = false;
		this.tab_Recur.SettingInVisible = false;
		this.tab_Recur.SettingReadonly = false;
		this.tab_Recur.Size = new System.Drawing.Size(778, 196);
		this.tab_Recur.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Recur.Text = "Recurrence Info";
		this.luk_RemType.EditControl = null;
		this.luk_RemType.FieldName = "RemType";
		this.luk_RemType.IsSearchControl = false;
		this.luk_RemType.Location = new System.Drawing.Point(371, 7);
		this.luk_RemType.Name = "luk_RemType";
		this.luk_RemType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_RemType.Properties.IsSearchControl = false;
		this.luk_RemType.Properties.NullText = "";
		this.luk_RemType.SettingInVisible = false;
		this.luk_RemType.SettingReadonly = false;
		this.luk_RemType.Size = new System.Drawing.Size(100, 20);
		this.luk_RemType.TabIndex = 23;
		this.calc_RemQty.EditControl = null;
		this.calc_RemQty.FieldName = "RemQty";
		this.calc_RemQty.IsSearchControl = false;
		this.calc_RemQty.Location = new System.Drawing.Point(284, 7);
		this.calc_RemQty.Name = "calc_RemQty";
		this.calc_RemQty.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.calc_RemQty.Properties.IsSearchControl = false;
		this.calc_RemQty.SettingInVisible = false;
		this.calc_RemQty.SettingReadonly = false;
		this.calc_RemQty.Size = new System.Drawing.Size(81, 20);
		this.calc_RemQty.TabIndex = 22;
		this.chkReminder.EditControl = null;
		this.chkReminder.EditValue = "N";
		this.chkReminder.FieldName = "Reminder";
		this.chkReminder.IsSearchControl = false;
		this.chkReminder.Location = new System.Drawing.Point(209, 7);
		this.chkReminder.Name = "chkReminder";
		this.chkReminder.Properties.Caption = "Reminder";
		this.chkReminder.Properties.ValueChecked = "Y";
		this.chkReminder.Properties.ValueUnchecked = "N";
		this.chkReminder.SettingInVisible = false;
		this.chkReminder.SettingReadonly = false;
		this.chkReminder.Size = new System.Drawing.Size(75, 20);
		this.chkReminder.TabIndex = 21;
		this.panel_Range.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.panel_Range.Controls.Add(this.lb_End);
		this.panel_Range.Controls.Add(this.lb_Start);
		this.panel_Range.Controls.Add(this.lb_Occurrence);
		this.panel_Range.Controls.Add(this.txtAfterMaxOccur);
		this.panel_Range.Controls.Add(this.radio_By);
		this.panel_Range.Controls.Add(this.radio_After);
		this.panel_Range.Controls.Add(this.radio_NoEndDate);
		this.panel_Range.Controls.Add(this.lb_Range);
		this.panel_Range.Controls.Add(this.labelLine32);
		this.panel_Range.Controls.Add(this.dSeStartDat);
		this.panel_Range.Controls.Add(this.txtBySeEndDat);
		this.panel_Range.Location = new System.Drawing.Point(0, 98);
		this.panel_Range.Name = "panel_Range";
		this.panel_Range.Size = new System.Drawing.Size(967, 93);
		this.panel_Range.TabIndex = 25;
		this.lb_End.EditControl = null;
		this.lb_End.EditValue = "End";
		this.lb_End.FieldName = null;
		this.lb_End.HideWhenEmpty = false;
		this.lb_End.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_End.Location = new System.Drawing.Point(75, 35);
		this.lb_End.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_End.Name = "lb_End";
		this.lb_End.ReadOnly = false;
		this.lb_End.SettingInVisible = false;
		this.lb_End.SettingReadonly = false;
		this.lb_End.ShowUnderLine = true;
		this.lb_End.Size = new System.Drawing.Size(94, 20);
		this.lb_End.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_End.TabIndex = 0;
		this.lb_End.Utext = "End";
		this.lb_Start.EditControl = null;
		this.lb_Start.EditValue = "Start";
		this.lb_Start.FieldName = null;
		this.lb_Start.HideWhenEmpty = false;
		this.lb_Start.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Start.Location = new System.Drawing.Point(75, 16);
		this.lb_Start.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_Start.Name = "lb_Start";
		this.lb_Start.ReadOnly = false;
		this.lb_Start.SettingInVisible = false;
		this.lb_Start.SettingReadonly = false;
		this.lb_Start.ShowUnderLine = true;
		this.lb_Start.Size = new System.Drawing.Size(94, 20);
		this.lb_Start.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Start.TabIndex = 0;
		this.lb_Start.Utext = "Start";
		this.lb_Occurrence.EditControl = null;
		this.lb_Occurrence.EditValue = "Occurrence(s)";
		this.lb_Occurrence.FieldName = null;
		this.lb_Occurrence.HideWhenEmpty = false;
		this.lb_Occurrence.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Occurrence.Location = new System.Drawing.Point(348, 58);
		this.lb_Occurrence.Name = "lb_Occurrence";
		this.lb_Occurrence.ReadOnly = false;
		this.lb_Occurrence.SettingInVisible = false;
		this.lb_Occurrence.SettingReadonly = false;
		this.lb_Occurrence.ShowUnderLine = false;
		this.lb_Occurrence.Size = new System.Drawing.Size(68, 13);
		this.lb_Occurrence.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Occurrence.TabIndex = 52;
		this.lb_Occurrence.Utext = "Occurrence(s)";
		this.txtAfterMaxOccur.EditControl = null;
		this.txtAfterMaxOccur.FieldName = "MaxOccur";
		this.txtAfterMaxOccur.IsSearchControl = false;
		this.txtAfterMaxOccur.Location = new System.Drawing.Point(242, 51);
		this.txtAfterMaxOccur.Name = "txtAfterMaxOccur";
		this.txtAfterMaxOccur.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 7f);
		this.txtAfterMaxOccur.Properties.Appearance.Options.UseFont = true;
		this.txtAfterMaxOccur.Properties.AutoHeight = false;
		this.txtAfterMaxOccur.SettingInVisible = false;
		this.txtAfterMaxOccur.SettingReadonly = false;
		this.txtAfterMaxOccur.Size = new System.Drawing.Size(100, 18);
		this.txtAfterMaxOccur.TabIndex = 51;
		this.txtAfterMaxOccur.Validating += new System.ComponentModel.CancelEventHandler(this.txtAfterMaxOccur_Validating);
		this.radio_By.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.radio_By.Location = new System.Drawing.Point(169, 66);
		this.radio_By.Name = "radio_By";
		this.radio_By.Size = new System.Drawing.Size(37, 16);
		this.radio_By.TabIndex = 34;
		this.radio_By.Text = "By";
		this.radio_By.UseVisualStyleBackColor = true;
		this.radio_By.CheckedChanged += new System.EventHandler(this.radio_By_CheckedChanged);
		this.radio_After.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.radio_After.Location = new System.Drawing.Point(169, 52);
		this.radio_After.Name = "radio_After";
		this.radio_After.Size = new System.Drawing.Size(43, 14);
		this.radio_After.TabIndex = 33;
		this.radio_After.Text = "After";
		this.radio_After.UseVisualStyleBackColor = true;
		this.radio_After.CheckedChanged += new System.EventHandler(this.radio_After_CheckedChanged);
		this.radio_NoEndDate.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.radio_NoEndDate.Location = new System.Drawing.Point(169, 37);
		this.radio_NoEndDate.Name = "radio_NoEndDate";
		this.radio_NoEndDate.Size = new System.Drawing.Size(85, 14);
		this.radio_NoEndDate.TabIndex = 32;
		this.radio_NoEndDate.Text = "No End Date";
		this.radio_NoEndDate.UseVisualStyleBackColor = true;
		this.radio_NoEndDate.CheckedChanged += new System.EventHandler(this.radio_NoEndDate_CheckedChanged);
		this.lb_Range.EditControl = null;
		this.lb_Range.EditValue = "Range";
		this.lb_Range.FieldName = null;
		this.lb_Range.HideWhenEmpty = false;
		this.lb_Range.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Range.Location = new System.Drawing.Point(75, 0);
		this.lb_Range.Name = "lb_Range";
		this.lb_Range.ReadOnly = false;
		this.lb_Range.SettingInVisible = false;
		this.lb_Range.SettingReadonly = false;
		this.lb_Range.ShowUnderLine = false;
		this.lb_Range.Size = new System.Drawing.Size(46, 18);
		this.lb_Range.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Range.TabIndex = 0;
		this.lb_Range.Utext = "Range";
		this.labelLine32.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.labelLine32.Appearance.Options.UseFont = true;
		this.labelLine32.EditControl = null;
		this.labelLine32.EditValue = "Start";
		this.labelLine32.FieldName = null;
		this.labelLine32.HideWhenEmpty = false;
		this.labelLine32.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine32.Location = new System.Drawing.Point(75, 19);
		this.labelLine32.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.labelLine32.Name = "labelLine32";
		this.labelLine32.ReadOnly = false;
		this.labelLine32.SettingInVisible = false;
		this.labelLine32.SettingReadonly = false;
		this.labelLine32.ShowUnderLine = true;
		this.labelLine32.Size = new System.Drawing.Size(94, 0);
		this.labelLine32.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine32.TabIndex = 45;
		this.labelLine32.Utext = "Start";
		this.dSeStartDat.AllowDrop = true;
		this.dSeStartDat.EditControl = null;
		this.dSeStartDat.EditValue = null;
		this.dSeStartDat.Enabled = false;
		this.dSeStartDat.FieldName = "SeStartDat";
		this.dSeStartDat.Location = new System.Drawing.Point(169, 16);
		this.dSeStartDat.Name = "dSeStartDat";
		this.dSeStartDat.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 7f);
		this.dSeStartDat.Properties.Appearance.Options.UseFont = true;
		this.dSeStartDat.Properties.AutoHeight = false;
		this.dSeStartDat.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dSeStartDat.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dSeStartDat.Properties.IsSearchControl = false;
		this.dSeStartDat.Properties.Mask.EditMask = "";
		this.dSeStartDat.Properties.Mask.UseMaskAsDisplayFormat = true;
		this.dSeStartDat.Properties.ReadOnly = true;
		this.dSeStartDat.SettingInVisible = false;
		this.dSeStartDat.SettingReadonly = false;
		this.dSeStartDat.Size = new System.Drawing.Size(100, 18);
		this.dSeStartDat.TabIndex = 31;
		this.txtBySeEndDat.EditControl = null;
		this.txtBySeEndDat.EditValue = null;
		this.txtBySeEndDat.FieldName = "SeEndDat";
		this.txtBySeEndDat.Location = new System.Drawing.Point(242, 71);
		this.txtBySeEndDat.Name = "txtBySeEndDat";
		this.txtBySeEndDat.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 7f);
		this.txtBySeEndDat.Properties.Appearance.Options.UseFont = true;
		this.txtBySeEndDat.Properties.AutoHeight = false;
		this.txtBySeEndDat.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtBySeEndDat.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtBySeEndDat.Properties.IsSearchControl = false;
		this.txtBySeEndDat.Properties.Mask.EditMask = "";
		this.txtBySeEndDat.Properties.Mask.UseMaskAsDisplayFormat = true;
		this.txtBySeEndDat.SettingInVisible = false;
		this.txtBySeEndDat.SettingReadonly = false;
		this.txtBySeEndDat.Size = new System.Drawing.Size(100, 18);
		this.txtBySeEndDat.TabIndex = 50;
		this.txtBySeEndDat.Validating += new System.ComponentModel.CancelEventHandler(this.txtBySeEndDat_Validating);
		this.panel_Daily.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.panel_Daily.Controls.Add(this.chkSun);
		this.panel_Daily.Controls.Add(this.cbMonth);
		this.panel_Daily.Controls.Add(this.cbMonth1);
		this.panel_Daily.Controls.Add(this.cbDayOfWeek);
		this.panel_Daily.Controls.Add(this.cbWeek);
		this.panel_Daily.Controls.Add(this.chkSat);
		this.panel_Daily.Controls.Add(this.txtDayInMonth);
		this.panel_Daily.Controls.Add(this.radio_SubOption2);
		this.panel_Daily.Controls.Add(this.label_RepeatEvery);
		this.panel_Daily.Controls.Add(this.chkFri);
		this.panel_Daily.Controls.Add(this.txtInterval);
		this.panel_Daily.Controls.Add(this.radio_SubOption1);
		this.panel_Daily.Controls.Add(this.lb_Of);
		this.panel_Daily.Controls.Add(this.lb_RepeatOn);
		this.panel_Daily.Controls.Add(this.chkThu);
		this.panel_Daily.Controls.Add(this.lb_RepeatEvery);
		this.panel_Daily.Controls.Add(this.chkTue);
		this.panel_Daily.Controls.Add(this.chkWed);
		this.panel_Daily.Controls.Add(this.chkMon);
		this.panel_Daily.Location = new System.Drawing.Point(1, 33);
		this.panel_Daily.Name = "panel_Daily";
		this.panel_Daily.Size = new System.Drawing.Size(966, 65);
		this.panel_Daily.TabIndex = 24;
		this.chkSun.EditControl = null;
		this.chkSun.EditValue = "N";
		this.chkSun.FieldName = "Sunday";
		this.chkSun.IsSearchControl = false;
		this.chkSun.Location = new System.Drawing.Point(638, 4);
		this.chkSun.Name = "chkSun";
		this.chkSun.Properties.Caption = "Sun";
		this.chkSun.Properties.ValueChecked = "Y";
		this.chkSun.Properties.ValueUnchecked = "N";
		this.chkSun.SettingInVisible = false;
		this.chkSun.SettingReadonly = false;
		this.chkSun.Size = new System.Drawing.Size(46, 20);
		this.chkSun.TabIndex = 33;
		this.cbMonth.EditControl = null;
		this.cbMonth.FieldName = "Month";
		this.cbMonth.IsSearchControl = false;
		this.cbMonth.Location = new System.Drawing.Point(321, 24);
		this.cbMonth.Name = "cbMonth";
		this.cbMonth.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 7f);
		this.cbMonth.Properties.Appearance.Options.UseFont = true;
		this.cbMonth.Properties.AutoHeight = false;
		this.cbMonth.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbMonth.Properties.Items.AddRange(new object[12]
		{
			"January", "February", "March", "April", "May", "June", "July", "August", "September", "October",
			"November", "December"
		});
		this.cbMonth.SettingInVisible = false;
		this.cbMonth.SettingReadonly = false;
		this.cbMonth.Size = new System.Drawing.Size(100, 18);
		this.cbMonth.TabIndex = 32;
		this.cbMonth.SelectedValueChanged += new System.EventHandler(this.cbMonth_SelectedValueChanged);
		this.cbMonth1.EditControl = null;
		this.cbMonth1.FieldName = "Month";
		this.cbMonth1.IsSearchControl = false;
		this.cbMonth1.Location = new System.Drawing.Point(489, 44);
		this.cbMonth1.Name = "cbMonth1";
		this.cbMonth1.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 7f);
		this.cbMonth1.Properties.Appearance.Options.UseFont = true;
		this.cbMonth1.Properties.AutoHeight = false;
		this.cbMonth1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbMonth1.Properties.Items.AddRange(new object[12]
		{
			"January", "February", "March", "April", "May", "June", "July", "August", "September", "October",
			"November", "December"
		});
		this.cbMonth1.SettingInVisible = false;
		this.cbMonth1.SettingReadonly = false;
		this.cbMonth1.Size = new System.Drawing.Size(100, 18);
		this.cbMonth1.TabIndex = 31;
		this.cbMonth1.SelectedValueChanged += new System.EventHandler(this.cbMonth1_SelectedValueChanged);
		this.cbDayOfWeek.EditControl = null;
		this.cbDayOfWeek.FieldName = "DayOfWeek";
		this.cbDayOfWeek.IsSearchControl = false;
		this.cbDayOfWeek.Location = new System.Drawing.Point(358, 44);
		this.cbDayOfWeek.Name = "cbDayOfWeek";
		this.cbDayOfWeek.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 7f);
		this.cbDayOfWeek.Properties.Appearance.Options.UseFont = true;
		this.cbDayOfWeek.Properties.AutoHeight = false;
		this.cbDayOfWeek.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbDayOfWeek.Properties.Items.AddRange(new object[10] { "day", "weekday", "weekend day", "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" });
		this.cbDayOfWeek.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.cbDayOfWeek.SettingInVisible = false;
		this.cbDayOfWeek.SettingReadonly = false;
		this.cbDayOfWeek.Size = new System.Drawing.Size(100, 18);
		this.cbDayOfWeek.TabIndex = 30;
		this.cbWeek.EditControl = null;
		this.cbWeek.FieldName = "Week";
		this.cbWeek.IsSearchControl = false;
		this.cbWeek.Location = new System.Drawing.Point(253, 44);
		this.cbWeek.Name = "cbWeek";
		this.cbWeek.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 7f);
		this.cbWeek.Properties.Appearance.Options.UseFont = true;
		this.cbWeek.Properties.AutoHeight = false;
		this.cbWeek.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbWeek.Properties.Items.AddRange(new object[5] { "first", "second", "third", "fourth", "last" });
		this.cbWeek.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.cbWeek.SettingInVisible = false;
		this.cbWeek.SettingReadonly = false;
		this.cbWeek.Size = new System.Drawing.Size(100, 18);
		this.cbWeek.TabIndex = 30;
		this.chkSat.EditControl = null;
		this.chkSat.EditValue = "N";
		this.chkSat.FieldName = "Saturday";
		this.chkSat.IsSearchControl = false;
		this.chkSat.Location = new System.Drawing.Point(597, 4);
		this.chkSat.Name = "chkSat";
		this.chkSat.Properties.Caption = "Sat";
		this.chkSat.Properties.ValueChecked = "Y";
		this.chkSat.Properties.ValueUnchecked = "N";
		this.chkSat.SettingInVisible = false;
		this.chkSat.SettingReadonly = false;
		this.chkSat.Size = new System.Drawing.Size(37, 20);
		this.chkSat.TabIndex = 33;
		this.txtDayInMonth.EditControl = null;
		this.txtDayInMonth.FieldName = "DayInMonth";
		this.txtDayInMonth.IsSearchControl = false;
		this.txtDayInMonth.Location = new System.Drawing.Point(427, 24);
		this.txtDayInMonth.Name = "txtDayInMonth";
		this.txtDayInMonth.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtDayInMonth.Properties.Appearance.Options.UseFont = true;
		this.txtDayInMonth.Properties.AutoHeight = false;
		this.txtDayInMonth.SettingInVisible = false;
		this.txtDayInMonth.SettingReadonly = false;
		this.txtDayInMonth.Size = new System.Drawing.Size(37, 18);
		this.txtDayInMonth.TabIndex = 29;
		this.txtDayInMonth.Validating += new System.ComponentModel.CancelEventHandler(this.txtDayInMonth_Validating);
		this.radio_SubOption2.AutoSize = true;
		this.radio_SubOption2.Location = new System.Drawing.Point(207, 48);
		this.radio_SubOption2.Name = "radio_SubOption2";
		this.radio_SubOption2.Size = new System.Drawing.Size(14, 13);
		this.radio_SubOption2.TabIndex = 30;
		this.radio_SubOption2.UseVisualStyleBackColor = true;
		this.radio_SubOption2.CheckedChanged += new System.EventHandler(this.radio_SubOption2_CheckedChanged);
		this.label_RepeatEvery.AutoSize = true;
		this.label_RepeatEvery.Location = new System.Drawing.Point(325, 10);
		this.label_RepeatEvery.Name = "label_RepeatEvery";
		this.label_RepeatEvery.Size = new System.Drawing.Size(39, 13);
		this.label_RepeatEvery.TabIndex = 27;
		this.label_RepeatEvery.Text = "Day(s)";
		this.chkFri.EditControl = null;
		this.chkFri.EditValue = "N";
		this.chkFri.FieldName = "Friday";
		this.chkFri.IsSearchControl = false;
		this.chkFri.Location = new System.Drawing.Point(557, 4);
		this.chkFri.Name = "chkFri";
		this.chkFri.Properties.Caption = "Fri";
		this.chkFri.Properties.ValueChecked = "Y";
		this.chkFri.Properties.ValueUnchecked = "N";
		this.chkFri.SettingInVisible = false;
		this.chkFri.SettingReadonly = false;
		this.chkFri.Size = new System.Drawing.Size(33, 20);
		this.chkFri.TabIndex = 33;
		this.txtInterval.EditControl = null;
		this.txtInterval.FieldName = "Interval";
		this.txtInterval.IsSearchControl = false;
		this.txtInterval.Location = new System.Drawing.Point(226, 5);
		this.txtInterval.Name = "txtInterval";
		this.txtInterval.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 7f);
		this.txtInterval.Properties.Appearance.Options.UseFont = true;
		this.txtInterval.Properties.AutoHeight = false;
		this.txtInterval.SettingInVisible = false;
		this.txtInterval.SettingReadonly = false;
		this.txtInterval.Size = new System.Drawing.Size(93, 18);
		this.txtInterval.TabIndex = 26;
		this.txtInterval.Validating += new System.ComponentModel.CancelEventHandler(this.txtInterval_Validating);
		this.radio_SubOption1.AutoSize = true;
		this.radio_SubOption1.Location = new System.Drawing.Point(207, 26);
		this.radio_SubOption1.Name = "radio_SubOption1";
		this.radio_SubOption1.Size = new System.Drawing.Size(101, 17);
		this.radio_SubOption1.TabIndex = 29;
		this.radio_SubOption1.Text = "Every Weekday";
		this.radio_SubOption1.UseVisualStyleBackColor = true;
		this.radio_SubOption1.CheckedChanged += new System.EventHandler(this.radio_SubOption1_CheckedChanged);
		this.lb_Of.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lb_Of.Appearance.Options.UseFont = true;
		this.lb_Of.EditControl = null;
		this.lb_Of.EditValue = "of";
		this.lb_Of.FieldName = null;
		this.lb_Of.HideWhenEmpty = false;
		this.lb_Of.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Of.Location = new System.Drawing.Point(466, 44);
		this.lb_Of.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.lb_Of.Name = "lb_Of";
		this.lb_Of.ReadOnly = false;
		this.lb_Of.SettingInVisible = false;
		this.lb_Of.SettingReadonly = false;
		this.lb_Of.ShowUnderLine = true;
		this.lb_Of.Size = new System.Drawing.Size(38, 16);
		this.lb_Of.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Of.TabIndex = 24;
		this.lb_Of.Utext = "of";
		this.lb_RepeatOn.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lb_RepeatOn.Appearance.Options.UseFont = true;
		this.lb_RepeatOn.EditControl = null;
		this.lb_RepeatOn.EditValue = "Repeat on";
		this.lb_RepeatOn.FieldName = null;
		this.lb_RepeatOn.HideWhenEmpty = false;
		this.lb_RepeatOn.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_RepeatOn.Location = new System.Drawing.Point(79, 23);
		this.lb_RepeatOn.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.lb_RepeatOn.Name = "lb_RepeatOn";
		this.lb_RepeatOn.ReadOnly = false;
		this.lb_RepeatOn.SettingInVisible = false;
		this.lb_RepeatOn.SettingReadonly = false;
		this.lb_RepeatOn.ShowUnderLine = true;
		this.lb_RepeatOn.Size = new System.Drawing.Size(123, 20);
		this.lb_RepeatOn.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_RepeatOn.TabIndex = 0;
		this.lb_RepeatOn.Utext = "Repeat on";
		this.chkThu.EditControl = null;
		this.chkThu.EditValue = "N";
		this.chkThu.FieldName = "Thursday";
		this.chkThu.IsSearchControl = false;
		this.chkThu.Location = new System.Drawing.Point(509, 4);
		this.chkThu.Name = "chkThu";
		this.chkThu.Properties.Caption = "Thu";
		this.chkThu.Properties.ValueChecked = "Y";
		this.chkThu.Properties.ValueUnchecked = "N";
		this.chkThu.SettingInVisible = false;
		this.chkThu.SettingReadonly = false;
		this.chkThu.Size = new System.Drawing.Size(46, 20);
		this.chkThu.TabIndex = 33;
		this.lb_RepeatEvery.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lb_RepeatEvery.Appearance.Options.UseFont = true;
		this.lb_RepeatEvery.EditControl = null;
		this.lb_RepeatEvery.EditValue = "Repeat Every";
		this.lb_RepeatEvery.FieldName = null;
		this.lb_RepeatEvery.HideWhenEmpty = false;
		this.lb_RepeatEvery.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_RepeatEvery.Location = new System.Drawing.Point(79, 4);
		this.lb_RepeatEvery.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.lb_RepeatEvery.Name = "lb_RepeatEvery";
		this.lb_RepeatEvery.ReadOnly = false;
		this.lb_RepeatEvery.SettingInVisible = false;
		this.lb_RepeatEvery.SettingReadonly = false;
		this.lb_RepeatEvery.ShowUnderLine = true;
		this.lb_RepeatEvery.Size = new System.Drawing.Size(123, 19);
		this.lb_RepeatEvery.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_RepeatEvery.TabIndex = 0;
		this.lb_RepeatEvery.Utext = "Repeat Every";
		this.chkTue.EditControl = null;
		this.chkTue.EditValue = "N";
		this.chkTue.FieldName = "Tuesday";
		this.chkTue.IsSearchControl = false;
		this.chkTue.Location = new System.Drawing.Point(412, 4);
		this.chkTue.Name = "chkTue";
		this.chkTue.Properties.Caption = "Tue";
		this.chkTue.Properties.ValueChecked = "Y";
		this.chkTue.Properties.ValueUnchecked = "N";
		this.chkTue.SettingInVisible = false;
		this.chkTue.SettingReadonly = false;
		this.chkTue.Size = new System.Drawing.Size(46, 20);
		this.chkTue.TabIndex = 33;
		this.chkWed.EditControl = null;
		this.chkWed.EditValue = "N";
		this.chkWed.FieldName = "Wednesday";
		this.chkWed.IsSearchControl = false;
		this.chkWed.Location = new System.Drawing.Point(458, 4);
		this.chkWed.Name = "chkWed";
		this.chkWed.Properties.Caption = "Wed";
		this.chkWed.Properties.ValueChecked = "Y";
		this.chkWed.Properties.ValueUnchecked = "N";
		this.chkWed.SettingInVisible = false;
		this.chkWed.SettingReadonly = false;
		this.chkWed.Size = new System.Drawing.Size(46, 20);
		this.chkWed.TabIndex = 33;
		this.chkMon.EditControl = null;
		this.chkMon.EditValue = "N";
		this.chkMon.FieldName = "Monday";
		this.chkMon.IsSearchControl = false;
		this.chkMon.Location = new System.Drawing.Point(364, 4);
		this.chkMon.Name = "chkMon";
		this.chkMon.Properties.Caption = "Mon";
		this.chkMon.Properties.ValueChecked = "Y";
		this.chkMon.Properties.ValueUnchecked = "N";
		this.chkMon.SettingInVisible = false;
		this.chkMon.SettingReadonly = false;
		this.chkMon.Size = new System.Drawing.Size(46, 20);
		this.chkMon.TabIndex = 33;
		this.cbRecurPat.EditControl = null;
		this.cbRecurPat.FieldName = "RecurPat";
		this.cbRecurPat.IsSearchControl = false;
		this.cbRecurPat.Location = new System.Drawing.Point(3, 7);
		this.cbRecurPat.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.cbRecurPat.Name = "cbRecurPat";
		this.cbRecurPat.Properties.AutoHeight = false;
		this.cbRecurPat.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbRecurPat.Properties.IsSearchControl = false;
		this.cbRecurPat.Properties.NullText = "";
		this.cbRecurPat.SettingInVisible = false;
		this.cbRecurPat.SettingReadonly = false;
		this.cbRecurPat.Size = new System.Drawing.Size(193, 20);
		this.cbRecurPat.TabIndex = 20;
		this.cbRecurPat.EditValueChanged += new System.EventHandler(this.cbRecurPat_EditValueChanged);
		this.tab_AddInfo.Controls.Add(this.layout_UDF);
		this.tab_AddInfo.EditControl = null;
		this.tab_AddInfo.EditValue = "Additional Info";
		this.tab_AddInfo.FieldName = null;
		this.tab_AddInfo.IsSearchControl = false;
		this.tab_AddInfo.Name = "tab_AddInfo";
		this.tab_AddInfo.ReadOnly = false;
		this.tab_AddInfo.SettingInVisible = false;
		this.tab_AddInfo.SettingReadonly = false;
		this.tab_AddInfo.Size = new System.Drawing.Size(778, 196);
		this.tab_AddInfo.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_AddInfo.Text = "Additional Info";
		this.layout_UDF.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layout_UDF.Location = new System.Drawing.Point(0, 0);
		this.layout_UDF.Name = "layout_UDF";
		this.layout_UDF.OptionsView.DrawItemBorders = true;
		this.layout_UDF.Root = this.Root;
		this.layout_UDF.Size = new System.Drawing.Size(778, 196);
		this.layout_UDF.TabIndex = 17;
		this.layout_UDF.Text = "layoutControl1";
		this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.Root.GroupBordersVisible = false;
		this.Root.Name = "Root";
		this.Root.Size = new System.Drawing.Size(778, 196);
		this.Root.TextVisible = false;
		this.tab_Att.Controls.Add(this.ucAttachment1);
		this.tab_Att.EditControl = null;
		this.tab_Att.EditValue = "Attachment";
		this.tab_Att.FieldName = null;
		this.tab_Att.IsSearchControl = false;
		this.tab_Att.Name = "tab_Att";
		this.tab_Att.ReadOnly = false;
		this.tab_Att.SettingInVisible = false;
		this.tab_Att.SettingReadonly = false;
		this.tab_Att.Size = new System.Drawing.Size(778, 196);
		this.tab_Att.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Att.Text = "Attachment";
		this.ucAttachment1.DataSource = null;
		this.ucAttachment1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.ucAttachment1.FormId = "oEventManagement";
		this.ucAttachment1.ItemId = "ATC1";
		this.ucAttachment1.Location = new System.Drawing.Point(0, 0);
		this.ucAttachment1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.ucAttachment1.Name = "ucAttachment1";
		this.ucAttachment1.Size = new System.Drawing.Size(778, 196);
		this.ucAttachment1.TabIndex = 78;
		this.tab_Comment.Controls.Add(this.ucComment1);
		this.tab_Comment.EditControl = null;
		this.tab_Comment.EditValue = "Comment";
		this.tab_Comment.FieldName = null;
		this.tab_Comment.IsSearchControl = false;
		this.tab_Comment.Name = "tab_Comment";
		this.tab_Comment.ReadOnly = false;
		this.tab_Comment.SettingInVisible = false;
		this.tab_Comment.SettingReadonly = false;
		this.tab_Comment.Size = new System.Drawing.Size(778, 196);
		this.tab_Comment.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Comment.Text = "Comment";
		this.ucComment1.AdditionNotifyUser = (System.Collections.Generic.List<int>)resources.GetObject("ucComment1.AdditionNotifyUser");
		this.ucComment1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.ucComment1.Location = new System.Drawing.Point(0, 0);
		this.ucComment1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.ucComment1.Name = "ucComment1";
		this.ucComment1.ObjectType = Apzon.Commons.ApzObjectTypes.oNone;
		this.ucComment1.Size = new System.Drawing.Size(778, 196);
		this.ucComment1.TabIndex = 0;
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
		this.lblCreatedBy.Location = new System.Drawing.Point(278, 683);
		this.lblCreatedBy.Name = "lblCreatedBy";
		this.lblCreatedBy.ReadOnly = false;
		this.lblCreatedBy.SettingInVisible = false;
		this.lblCreatedBy.SettingReadonly = false;
		this.lblCreatedBy.ShowUnderLine = false;
		this.lblCreatedBy.Size = new System.Drawing.Size(334, 17);
		this.lblCreatedBy.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lblCreatedBy.TabIndex = 23;
		this.lblCreatedBy.Utext = "Created By:";
		this.btn_SendMail.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_SendMail.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_SendMail.Appearance.Options.UseBackColor = true;
		this.btn_SendMail.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_SendMail.EditControl = null;
		this.btn_SendMail.EditValue = "Send E-mail";
		this.btn_SendMail.FieldName = null;
		this.btn_SendMail.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_SendMail.ImageOptions.Image");
		this.btn_SendMail.IsSearchControl = false;
		this.btn_SendMail.Location = new System.Drawing.Point(168, 682);
		this.btn_SendMail.Name = "btn_SendMail";
		this.btn_SendMail.ReadOnly = false;
		this.btn_SendMail.SettingInVisible = false;
		this.btn_SendMail.SettingReadonly = false;
		this.btn_SendMail.Size = new System.Drawing.Size(93, 20);
		this.btn_SendMail.TabIndex = 28;
		this.btn_SendMail.Text = "Send E-mail";
		this.btn_SendMail.Click += new System.EventHandler(this.btn_SendMail_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Cancel.Appearance.Options.UseBackColor = true;
		this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(70, 682);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(93, 20);
		this.btn_Cancel.TabIndex = 27;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Save.Appearance.Options.UseBackColor = true;
		this.btn_Save.EditControl = null;
		this.btn_Save.EditValue = "OK";
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(4, 682);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.ReadOnly = false;
		this.btn_Save.SettingInVisible = false;
		this.btn_Save.SettingReadonly = false;
		this.btn_Save.Size = new System.Drawing.Size(61, 20);
		this.btn_Save.TabIndex = 26;
		this.btn_Save.Text = "OK";
		this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
		this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.removeEventToolStripMenuItem });
		this.contextMenuStrip2.Name = "contextMenuStrip2";
		this.contextMenuStrip2.Size = new System.Drawing.Size(150, 26);
		this.contextMenuStrip2.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip2_Opening);
		this.removeEventToolStripMenuItem.Name = "removeEventToolStripMenuItem";
		this.removeEventToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
		this.removeEventToolStripMenuItem.Text = "Remove Event";
		this.removeEventToolStripMenuItem.Click += new System.EventHandler(this.removeEventToolStripMenuItem_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btn_Cancel;
		base.ClientSize = new System.Drawing.Size(1110, 709);
		this.ContextMenuStrip = this.contextMenuStrip2;
		base.Controls.Add(this.splitContainerControl1);
		base.FormId = "oEventManagement";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_EventManagement";
		base.ShowInTaskbar = false;
		this.Text = "Event Management";
		base.OnControlValueChanged += new Apzon.Controls.Form.ControlValueChanged(this.frm_EventManagement_OnControlValueChanged);
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_EventManagement_FormClosing);
		base.Shown += new System.EventHandler(this.frm_EventManagement_Shown);
		((System.ComponentModel.ISupportInitialize)this.rep_Deparment).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Bp).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit1.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemHourEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx1.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Invite).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.date_To.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_To.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_From.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_From.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl3).EndInit();
		this.splitContainerControl3.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.cfl_Opportunity.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtOwner.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Meetting.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cfl_UserSharing.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cfl_GroupEmployee.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BPName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BPType.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_ShareAllEmp.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_InviteAllEmp.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtDetails.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_ShareAll.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chkAllday.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_InviteAll.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtBeginTime.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cfl_EmplSharing.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtEndTime.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dStart_Recontact.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dStart_Recontact.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dEnDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dEnDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cfl_ProjectNew.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtBpCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cfl_ProjectWBS.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cfl_GroupUser.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bte_DistrRule.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbUser.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl2).EndInit();
		this.splitContainerControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).EndInit();
		this.xtraTabControl1.ResumeLayout(false);
		this.tab_Recur.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.luk_RemType.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calc_RemQty.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chkReminder.Properties).EndInit();
		this.panel_Range.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txtAfterMaxOccur.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dSeStartDat.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dSeStartDat.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtBySeEndDat.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtBySeEndDat.Properties).EndInit();
		this.panel_Daily.ResumeLayout(false);
		this.panel_Daily.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.chkSun.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbMonth.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbMonth1.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbDayOfWeek.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbWeek.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chkSat.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtDayInMonth.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chkFri.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtInterval.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chkThu.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chkTue.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chkWed.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chkMon.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbRecurPat.Properties).EndInit();
		this.tab_AddInfo.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.layout_UDF).EndInit();
		((System.ComponentModel.ISupportInitialize)this.Root).EndInit();
		this.tab_Att.ResumeLayout(false);
		this.tab_Comment.ResumeLayout(false);
		this.contextMenuStrip2.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
