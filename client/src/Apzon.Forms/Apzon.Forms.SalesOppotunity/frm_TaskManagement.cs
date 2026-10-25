using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Editor.MyLookUpEdit;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Forms.Properties;
using Apzon.Forms.Service;
using Apzon.Forms.UserControls;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.XtraBars.Navigation;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Drawing;
using DevExpress.XtraGrid.Scrolling;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraLayout;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Xml;

namespace Apzon.Forms.SalesOppotunity;

public class frm_TaskManagement : XtraFormEx
{
	private enum FormModeType
	{
		Normal,
		SingleView,
		SingleUpdate,
		SingleAdd
	}

	private enum TaskFilterType
	{
		None,
		Overdue,
		Today,
		ThisWeek,
		LastWeek,
		Users,
		Employees,
		ProjectNew,
		Unassigned
	}

	public enum SaveType
	{
		GridAdd,
		ValidateControl,
		UDF,
		ChangeStatus,
		ChangePosition,
		SingleAdd,
		SingleUpdate,
		Attachment,
		SubAdd,
		Copy
	}

	private enum DueDateOptions
	{
		Recalculate,
		RecalculateKeepStartTime,
		KeepDueDate,
		ResetDueDate
	}

	private enum GridViewFocus
	{
		NoFocus,
		Completed,
		NotCompleted
	}

	private DataTable dtAPZ_OCLG;

	private DataTable dtRow;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private GridView focusedView;

	private RepositoryItemButtonEdit emptyEditor;

	private bool _isLoading;

	private bool changingRowObject;

	private bool isValidate;

	private bool acceptChange;

	private bool changingRowFocus;

	private bool isModified;

	private bool changeRowStatus;

	private bool isFill;

	private bool deleteTask;

	private bool loadSingleTask;

	private bool readOnly;

	private bool queryChildNode;

	private bool reorderRow;

	private bool processKey;

	private bool modifyInAddMode;

	private bool openSubTask;

	private bool openInformationSplit;

	private bool openSubTaskSplit;

	private bool openAttachmentSplit;

	private bool openUDFSplit;

	private bool udfCellValueChanged;

	private bool rowClick;

	private bool saveTask;

	private bool addMainTask;

	private bool addSubTask;

	private bool changeTaskStatus;

	private bool rowUpdated;

	private bool rowUpdatedSub;

	private bool isSort;

	private bool saveSubTaskDetails;

	private bool saveSubTaskAssigner;

	private bool txtDetailsAutoHeight;

	private bool saveTaskPosition;

	private bool memoNotesAutoHeight;

	private bool loadTaskManagementSetting;

	private bool copyTask;

	private bool clickCheckChangeTask;

	private bool loadCompletedTask;

	private GridHitInfo downHitInfo;

	private string OrderFieldName = "OrderLine";

	private Dictionary<int, string> dictionEmployee = new Dictionary<int, string>();

	private Dictionary<int, string> dictionUser = new Dictionary<int, string>();

	private int GridMinWidth = 100;

	private int GridMinHeight = 24;

	private int rowHandleGridSub = -1;

	private int rowHandleGridList = -1;

	private int rowHandleGridCompleted = -1;

	private Color elementColor = Color.FromArgb(47, 46, 68);

	private Dictionary<int, string> dictionNodes = new Dictionary<int, string>();

	private string pathTask = "";

	private BaseEdit inplaceEditor;

	private List<int> listDocType = new List<int>();

	private List<int> listAssignedUser = new List<int>();

	private List<int> listAssignedEmployee = new List<int>();

	private List<int> listAssignedProjectNew = new List<int>();

	private Size singleModeSize = new Size(373, 530);

	private DataTable dtSort = new DataTable();

	private DataTable dtAPZ_CLG2 = new DataTable();

	private const string sortNone = "(None)";

	private const string sortPriority = "Priority";

	private const string sortDueDate = "Due Date";

	private const string sortAssignee = "Assignee";

	private const string sortAlpha = "Alphabetically";

	public bool IsSubTask;

	public bool CallFromCal;

	public int ClgCode;

	public DateTime StartDate;

	public DateTime EndDate;

	public string StartTime = "";

	public string EndTime = "";

	public int UserID;

	public DataRow DataRowFromCal;

	public Func<Task> ReloadCalendar;

	public Func<DataRow, Task> ReloadRelated;

	public Func<int, Task> ReloadAfterRemove;

	public int _owner;

	private new FormModeType _formMode;

	private TaskFilterType TaskFilter;

	private GridViewFocus _gridFocus;

	public Enums.ChangeLog ChangeLogType = Enums.ChangeLog.clTaskManagement;

	private IContainer components;

	private SplitContainerControl splitListTask;

	private ButtonEditCFL cfl_ProjectWBS;

	private SimpleButtonEx btnAttachment;

	private ComboBoxEditEx cbUser;

	private GridControlEx grd_List;

	private GridViewEx gv_List;

	private GridColumnEx gc_Code;

	private GridColumnEx gc_Task;

	private ButtonEditCFL cfl_GroupUser;

	private GridColumnEx gridColumnEx2;

	private GridColumnEx gc_Closed;

	private GridColumnEx gc_DueDate;

	private SimpleButtonEx btnSubTask;

	private SimpleButtonEx btnDelete;

	private GridColumnEx gc_ProjectWBS;

	private ButtonEditCFL cfl_GroupEmployee;

	private TextEditEx txtClgCode;

	private DateEditEx date_DueDate;

	private PanelControl panelControl1;

	private SplitContainerControl splitSubTask;

	private SplitContainerControl splitAttachment;

	private RepositoryItemButtonEdit rep_bte_Closed;

	private SplitContainerControl splitMoreInformation;

	private SimpleButtonEx btnDocument;

	private LabelControl labelControl5;

	private LabelControl labelControl4;

	private LabelControl labelControl6;

	private LabelControl labelControl3;

	private LabelControl labelControl2;

	private ucAttachment ucAttachment1;

	private LabelControl lblCreatedAt;

	private HourEdit date_DueTime;

	private ContextMenuStrip contextMenuStripSub;

	private ToolStripMenuItem toolStripMenuItem_DeleteCM;

	private GridColumnEx gcOrderLine;

	public SimpleButtonEx btnOK;

	private HourEdit date_StartTime;

	private LabelControl labelControl7;

	private DateEditEx date_StartDate;

	private LookUpEditSimple luk_Sort;

	private GridColumnEx gc_EndTime;

	private LabelControl labelControl8;

	private GridColumnEx gc_CreateDate;

	private GridColumnEx gc_AssignTo;

	private XtraScrollableControl xtraScrollableControl1;

	private TableLayoutPanel tableLayoutPanel1;

	private GridControlEx grd_ListCompleted;

	private GridViewEx gv_ListCompleted;

	private GridColumnEx gridColumnEx5;

	private GridColumnEx gc_Details;

	private GridColumnEx gc_Closed_Completed;

	private GridColumnEx gc_DueDate_Completed;

	private GridColumnEx gc_ProjectWBS_Completed;

	private GridColumnEx gridColumnEx13;

	private GridColumnEx gridColumnEx14;

	private GridColumnEx gridColumnEx15;

	private GridColumnEx gc_AssignTo_Completed;

	private SimpleButtonEx btnComplete;

	private RepositoryItemButtonEdit repositoryItemButtonEdit1;

	private CheckEditEx chk_Closed;

	private ToolTipController toolTipController1;

	private GridViewEx gv_ListSub;

	private GridColumnEx gridColumnEx8;

	private GridColumnEx gridColumnEx9;

	private GridColumnEx gridColumnEx10;

	private GridColumnEx gridColumnEx16;

	private RepositoryItemButtonEdit rep_bte_DeleteSubTask;

	private GridColumnEx gridColumnEx17;

	private GridColumnEx gridColumnEx18;

	private GridColumnEx gridColumnEx19;

	private GridColumnEx gridColumnEx20;

	private GridColumnEx gridColumnEx21;

	private GridColumnEx gridColumnEx22;

	private GridColumnEx gridColumnEx23;

	private GridColumnEx gridColumnEx24;

	private RepositoryItemButtonEdit rep_bte_Open;

	private GridColumnEx gridColumnEx25;

	private TableLayoutPanel tblLayoutInfo;

	private BreadCrumbEditEx brc_ParentTask;

	private ContextMenuStrip contextMenuList;

	private ToolStripMenuItem removeToolStripMenuItem;

	private ButtonEditDistrRule bte_DistrRule;

	private ucComment ucComment1;

	private ucLinkedDocument ucLinkedDocument1;

	private PanelControl panelFilter;

	private AccordionControl accordionControl1;

	private AccordionControlElement eleGeneral;

	private ToolStripMenuItem detailsToolStripMenuItem;

	private AccordionControlElement eleAll;

	private AccordionControlElement eleOverdue;

	private AccordionControlElement eleUser;

	private AccordionControlElement eleToday;

	private SplitContainerControl splitAccordion;

	private ButtonEditCFL cfl_BPCode;

	private LabelControl labelControl9;

	private ButtonEditCFL cfl_ProjectNew;

	private LabelControl labelControl10;

	private AccordionControlElement eleThisWeek;

	private AccordionControlElement eleLastWeek;

	private CheckedComboBoxEdit ccb_GroupUser;

	private CheckedComboBoxEdit ccb_GroupEmployee;

	private TextEditEx txt_BPType;

	private AccordionControlElement eleEmployee;

	private AccordionControlElement eleProjectNew;

	private GridColumnEx gc_ProjectEntry;

	private GridColumnEx gc_ProjectEntry_Completed;

	private SplitContainerControl splitUDF;

	private SimpleButtonEx btnUDF;

	private GridColumnEx gc_CloseDate_Completed;

	private LookUpEditSimple luk_Priority;

	private LabelControl labelControl11;

	private GridColumnEx gc_Priority;

	private RepositoryItemLookUpEditSimple rep_luk_Priority;

	private RepositoryItemTextEdit repositoryItemTextEdit1;

	private LabelControl labelControl1;

	private RepositoryItemButtonEditCFL rep_cfl_ProjectEntry;

	private GridColumnEx gc_Priority_Completed;

	private LayoutUDF layout_UDF;

	private RepositoryItemMemoEdit rep_memo_DetailsSub;

	private ToolStripMenuItem ctm_MainTaskAssigner;

	private ToolTipController toolTipGridSub;

	private TextBoxEx txtDetails;

	private PanelControl panelControl2;

	private GridControlEx grd_ListSub;

	private TextEditEx txt_BPName;

	private RepositoryItemButtonEditCFL rep_cfl_ProjectWBS;

	private GridColumnEx gc_CardCode;

	private TextBoxEx memoNotes;

	private ContextMenuStrip contextMenuInfoPanel;

	private ToolStripMenuItem copyTaskToolStripMenuItem;

	private ToolStripMenuItem copyTaskAndToolStripMenuItem;

	private ToolStripMenuItem copyTaskToolStripMenuItem1;

	private ToolStripMenuItem copyTaskAndToolStripMenuItem1;

	private ToolStripMenuItem removeToolStripMenuItem1;

	private ToolStripMenuItem copyTaskToolStripMenuItem2;

	private ToolStripMenuItem copyTaskAndToolStripMenuItem2;

	private ToolStripMenuItem ctm_JumpTo;

	private ToolStripSeparator toolStripSeparator2;

	private ToolStripSeparator toolStripSeparator1;

	private ToolStripSeparator toolStripSeparator5;

	private ToolStripSeparator toolStripSeparator4;

	private ToolStripSeparator toolStripSeparator3;

	private ContextMenuStrip contextMenuStripBottomText;

	private ToolStripMenuItem copyTaskIDToolStripMenuItem;

	private ButtonEditCFL cfl_Opportunity;

	private LabelControl labelControl12;

	private GridColumnEx gridColumnEx1;

	private RepositoryItemHourEdit repositoryItemHourEdit1;

	private GridColumnEx gridColumnEx4;

	private RepositoryItemHourEdit repositoryItemHourEdit2;

	private GridColumnEx gridColumnEx7;

	private GridColumnEx gridColumnEx11;

	private RepositoryItemButtonEditCFL btn_PrjGrid;

	private LayoutControlGroup layoutControlGroup1;

	private LabelControl labelControl13;

	private ButtonEditCFL cfl_Task;

	private LookUpEditSimple luk_TaskMode;

	private LabelControl labelControl14;

	private LabelControl lbOwner;

	private ButtonEditCFL txtOwner;

	private GridColumnEx gridColumnEx6;

	private new FormModeType FormMode
	{
		get
		{
			return _formMode;
		}
		set
		{
			_formMode = value;
			try
			{
				switch (_formMode)
				{
				case FormModeType.Normal:
					brc_ParentTask.Visible = false;
					break;
				case FormModeType.SingleView:
					btnOK.Text = Apzon.Commons.Properties.Resources.ButtonOK;
					FormModeViewUpdate();
					break;
				case FormModeType.SingleUpdate:
					btnOK.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
					FormModeViewUpdate();
					break;
				case FormModeType.SingleAdd:
					btnOK.Text = Apzon.Commons.Properties.Resources.ButtonAdd;
					splitMoreInformation.Collapsed = false;
					ucComment1.Visible = false;
					chk_Closed.Enabled = false;
					gv_ListSub.OptionsView.NewItemRowPosition = NewItemRowPosition.None;
					brc_ParentTask.Visible = false;
					modifyInAddMode = false;
					splitAccordion.PanelVisibility = SplitPanelVisibility.Panel2;
					break;
				}
			}
			catch (Exception)
			{
			}
		}
	}

	private void FormModeViewUpdate()
	{
		ucComment1.Visible = true;
		chk_Closed.Enabled = true;
		gv_ListSub.OptionsView.NewItemRowPosition = NewItemRowPosition.Top;
		GridSubAutoRowHeight();
		modifyInAddMode = false;
		if (IsSubTask)
		{
			brc_ParentTask.Visible = true;
		}
		else
		{
			brc_ParentTask.Visible = false;
		}
		splitAccordion.PanelVisibility = SplitPanelVisibility.Panel2;
	}

	public frm_TaskManagement()
	{
		InitializeComponent();
		FormMode = FormModeType.Normal;
		OnLoadAsync += frm_TaskManagement_Load;
		SetupForm();
		SetupSort();
		IsSetSuperTip = true;
		IsScrollable = true;
		RepositoryItemButtonEditCFL properties = cfl_ProjectNew.Properties;
		properties.AfterShowCFL = (Action<frm_CFL>)Delegate.Combine(properties.AfterShowCFL, (Action<frm_CFL>)((frm_CFL frm) =>
		{
			try
			{
				DataTable dataTable = frm.gv_CFL.GridControl.DataSource as DataTable;
				if (dataTable.Rows.Count > 0 && dataTable != null)
				{
					DataRow[] source = (from s in dataTable.AsEnumerable()
						where Function.ToString(s["PrgStatus"]) == "S"
						select s).ToArray();
					if (source.Count() > 0)
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
			catch (Exception ex)
			{
				WinCommons.ErrorLineNumber(ex);
			}
		}));
	}

	protected override void NoAuthSetControlSearch()
	{
		readOnly = true;
		btnComplete.Enabled = true;
		btnSubTask.Enabled = true;
		btnAttachment.Enabled = true;
		btnUDF.Enabled = true;
		btnDocument.Enabled = true;
		rep_bte_Closed.Buttons[0].Visible = true;
		rep_bte_Open.Buttons[0].Visible = true;
		rep_bte_DeleteSubTask.Buttons[0].Visible = true;
		rep_bte_DeleteSubTask.Buttons[0].Enabled = false;
		ucAttachment1.btDisplay.Enabled = true;
		luk_Sort.ReadOnly = false;
	}

	private void SetupForm()
	{
		try
		{
			new DataTable().Clear();
			FormId = "oTaskManagement";
			gv_List.FormId = FormId;
			gv_List.ItemId = "APZ_OCLG";
			gv_ListCompleted.FormId = FormId;
			gv_ListCompleted.ItemId = "APZ_OCLG";
			focusedView = gv_List;
			gv_ListSub.BorderStyle = BorderStyles.NoBorder;
			grd_ListCompleted.Visible = false;
			gv_List.FocusRectStyle = DrawFocusRectStyle.None;
			gv_ListCompleted.FocusRectStyle = DrawFocusRectStyle.None;
			gv_ListSub.FocusRectStyle = DrawFocusRectStyle.None;
			gv_ListSub.MouseMove += gridView1_MouseMove;
			gv_List.OptionsBehavior.KeepFocusedRowOnUpdate = true;
			gv_ListCompleted.OptionsBehavior.KeepFocusedRowOnUpdate = true;
			gv_List.OptionsCustomization.AllowSort = false;
			gv_List.Columns["OrderLine"].SortOrder = ColumnSortOrder.Descending;
			gv_List.Columns["endDate"].SortMode = ColumnSortMode.Custom;
			gv_ListCompleted.OptionsCustomization.AllowSort = false;
			gv_ListCompleted.Columns["CloseDate"].SortOrder = ColumnSortOrder.Descending;
			gv_ListCompleted.Columns["endDate"].SortMode = ColumnSortMode.Custom;
			gv_List.OptionsBehavior.ImmediateUpdateRowPosition = true;
			gv_ListSub.Columns["OrderLine"].SortOrder = ColumnSortOrder.Ascending;
			gc_Priority.SortMode = ColumnSortMode.Value;
			splitListTask.Collapsed = true;
			splitSubTask.Collapsed = true;
			splitAttachment.Collapsed = true;
			splitMoreInformation.Collapsed = true;
			splitUDF.Collapsed = true;
			date_DueTime.Properties.ValidateOnEnterKey = true;
			cfl_GroupEmployee.Properties.ValidateOnEnterKey = true;
			cfl_GroupUser.Properties.ValidateOnEnterKey = true;
			bte_DistrRule.Properties.ValidateOnEnterKey = true;
			cfl_ProjectWBS.Properties.ValidateOnEnterKey = true;
			SetColumnProperties(gc_AssignTo);
			SetColumnProperties(gc_AssignTo_Completed);
			SetColumnProperties(gc_ProjectWBS);
			SetColumnProperties(gc_ProjectWBS_Completed);
			SetColumnProperties(gc_ProjectEntry);
			SetColumnProperties(gc_ProjectEntry_Completed);
			SetColumnProperties(gc_Priority);
			SetColumnProperties(gc_Priority_Completed);
			SetColumnProperties(gc_CardCode);
			ucAttachment ucAttachment2 = ucAttachment1;
			ucAttachment2.BrowsedFile = (Action<List<DataRow>>)Delegate.Combine(ucAttachment2.BrowsedFile, (Action<List<DataRow>>)((List<DataRow> s) =>
			{
				if (FormMode == FormModeType.SingleView)
				{
					FormMode = FormModeType.SingleUpdate;
				}
			}));
			ucAttachment1.ButtonDelete_Click += ucAttachment1_ButtonBrowse_Click;
			ucAttachment ucAttachment3 = ucAttachment1;
			ucAttachment3.BrowsedFile = (Action<List<DataRow>>)Delegate.Combine(ucAttachment3.BrowsedFile, (Action<List<DataRow>>)((List<DataRow> dr) =>
			{
				AutoUploadAfterBrowse();
			}));
			ucAttachment ucAttachment4 = ucAttachment1;
			ucAttachment4.DeletedFile = (Action)Delegate.Combine(ucAttachment4.DeletedFile, new Action(AutoUploadAfterBrowse));
			ucAttachment1.gv_File.OptionsView.ShowColumnHeaders = false;
			ucAttachment1.gv_File.Columns["FileName"].VisibleIndex = 0;
			ucAttachment1.gv_File.Columns["trgtPath"].VisibleIndex = 1;
			ucComment1.ObjectType = ApzObjectTypes.oTaskManagement;
			ucComment1.gv_Comment.Appearance.Empty.BackColor = Color.FromArgb(250, 250, 250);
			ucComment1.AutoGridSize = true;
			ucComment1.BeforeSendComment += UcComment1_BeforeSendComment;
			ucLinkedDocument1.ItemSelected += ucLinkedDocument1_ItemSelected;
			ucLinkedDocument1.AfterValidating += ucLinkedDocument1_AfterValidating;
			ucLinkedDocument1.cbDocNum.BorderStyle = BorderStyles.NoBorder;
			ucLinkedDocument1.luk_DocType.BorderStyle = BorderStyles.NoBorder;
			ucLinkedDocument1.luk_DocType.Validating += Control_Validating;
			ucLinkedDocument1.luk_DocType.Validated += Control_Validated;
			ucLinkedDocument1.cbDocNum.Validated += Control_Validated;
			ucLinkedDocument1.cbDocNum.EditValueChanged += cbDocNum_EditValueChanged;
			ucLinkedDocument1.chkLinkDraft.LookAndFeel.SkinName = "Office 2016 Colorful";
			ucLinkedDocument1.chkLinkDraft.LookAndFeel.UseDefaultLookAndFeel = false;
			ucLinkedDocument1.luk_DocType.Properties.NullValuePrompt = "Document Type...";
			ucLinkedDocument1.luk_DocType.Properties.NullValuePromptShowForEmptyValue = true;
			ucLinkedDocument1.cbDocNum.Properties.NullValuePrompt = "Document Number...";
			ucLinkedDocument1.cbDocNum.Properties.NullValuePromptShowForEmptyValue = true;
			ucLinkedDocument1.chkLinkDraft.Font = new Font("Tahoma", 7.25f);
			ucLinkedDocument1.chkLinkDraft.ForeColor = Color.DarkGray;
			ucLinkedDocument1.luk_DocType.Tag = "APZ_OCLG, DocType";
			ucLinkedDocument1.cbDocNum.Tag = "APZ_OCLG, DocNum";
			ucLinkedDocument1.BeforeButtonPressed += UcLinkedDocument1_BeforeButtonPressed;
			SetupRepository();
			gv_List.GotFocus += (object sender, EventArgs args) =>
			{
				_gridFocus = GridViewFocus.NotCompleted;
			};
			gv_ListCompleted.GotFocus += (object sender, EventArgs args) =>
			{
				_gridFocus = GridViewFocus.Completed;
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void UcLinkedDocument1_BeforeButtonPressed(object sender, EventArgs e)
	{
		ucLinkedDocument1.FindCardCode = Function.ToString(cfl_BPCode.EditValue);
	}

	private void UcComment1_BeforeSendComment(object sender, EventArgs e)
	{
		try
		{
			if (!dtRow.EmptyData())
			{
				DataRow dataRow = dtRow.Rows[0];
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

	private void SetupRepository()
	{
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
		emptyEditor = new RepositoryItemButtonEdit();
		emptyEditor.Buttons.Clear();
		emptyEditor.TextEditStyle = TextEditStyles.HideTextEditor;
		grd_List.RepositoryItems.Add(emptyEditor);
		cfl_BPCode.Properties.LoadDataSourceWhenEnter = true;
		cfl_BPCode.Validated += cfl_BPCode_Validated;
		ccb_GroupUser.Properties.ValueMember = "USERID";
		ccb_GroupUser.Properties.DisplayMember = "U_NAME";
		ccb_GroupUser.Properties.SeparatorChar = ';';
		ccb_GroupEmployee.Properties.ValueMember = "empID";
		ccb_GroupEmployee.Properties.DisplayMember = "empName";
		ccb_GroupEmployee.Properties.SeparatorChar = ';';
		DataTable dataTable = new DataTable();
		dataTable.Columns.Add("Code", typeof(int));
		dataTable.Columns.Add("Name", typeof(string));
		dataTable.Rows.Add(3, "Emergency");
		dataTable.Rows.Add(2, "High");
		dataTable.Rows.Add(1, "Normal");
		dataTable.Rows.Add(0, "Low");
		dataTable.AcceptChanges();
		luk_Priority.Properties.DataSource = dataTable;
		cfl_ProjectWBS.Properties.ValueMember = "TaskCode";
		cfl_ProjectWBS.Properties.DisplayMember = "TaskCode";
		cfl_ProjectWBS.Properties.DisplayMember2 = "TaskName";
		cfl_ProjectWBS.Properties.SeparateString = " - ";
		cfl_ProjectWBS.Properties.TypeCFL = Enums.TypeCFL.ProjectTask;
		cfl_ProjectWBS.Properties.TypeDrillDown = ApzObjectTypes.oProjectManager;
		RepositoryItemButtonEditCFL properties = cfl_ProjectWBS.Properties;
		properties.BeforeShowCFL = (Action<frm_CFL>)Delegate.Combine(properties.BeforeShowCFL, (Action<frm_CFL>)((frm_CFL frm) =>
		{
			frm.projectId = Function.ParseInt(cfl_ProjectNew.EditValue);
		}));
		cfl_Opportunity.Properties.ValueMember = "OprId";
		cfl_Opportunity.Properties.DisplayMember = "OprId";
		cfl_Opportunity.Properties.DisplayMember2 = "Name";
		cfl_Opportunity.Properties.SeparateString = " - ";
		cfl_Opportunity.Properties.TypeDrillDown = ApzObjectTypes.oSalesOpportunities;
		cfl_Opportunity.Properties.HandleChooseItem = Opportunity_HandleChooseItem;
		gc_Closed_Completed.ColumnEdit = rep_bte_Closed;
		grd_List.RepositoryItems.Add(rep_bte_Open);
		grd_ListCompleted.RepositoryItems.Add(rep_bte_Closed);
		grd_ListSub.RepositoryItems.Add(rep_bte_Closed);
		grd_ListSub.RepositoryItems.Add(rep_bte_DeleteSubTask);
		gv_ListSub.VertScrollVisibility = ScrollVisibility.Never;
	}

	private void Opportunity_HandleChooseItem(DataRow dr)
	{
		cfl_Opportunity.EditValue = Function.ParseInt(dr["OprId"]);
		if (dtRow.IsNotNull())
		{
			dtRow.Rows[0]["OprLine"] = Function.ParseInt(dr["OprLine"]);
		}
	}

	private void ProjCode_HandleChooseItem(DataRow dr)
	{
		try
		{
			cfl_ProjectNew.EditValue = dr["AbsEntry"];
			Function.ToString(cfl_BPCode.EditValue);
			isFill = true;
			cfl_BPCode.EditValue = dr["CardCode"];
			isFill = false;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void SetColumnProperties(GridColumn column)
	{
		column.OptionsColumn.ReadOnly = true;
		column.OptionsColumn.AllowEdit = false;
		column.OptionsColumn.AllowFocus = false;
		column.OptionsColumn.TabStop = false;
	}

	private async Task frm_TaskManagement_Load(object sender, EventArgs e)
	{
		if (_isLoading)
		{
			return;
		}
		try
		{
			_isLoading = true;
			if (CallFromCal)
			{
				splitListTask.CollapsePanel = SplitCollapsePanel.Panel1;
				btnOK.Visible = true;
				Size = singleModeSize;
				SetMinScrollSize(singleModeSize);
				int num = 0;
				if (DataRowFromCal.IsExistsColumn("ClgCode"))
				{
					num = Function.ParseInt(DataRowFromCal["ClgCode"]);
				}
				if (num <= 0)
				{
					FormMode = FormModeType.SingleAdd;
				}
				else
				{
					FormMode = FormModeType.SingleView;
				}
				string text = "";
				if (DataRowFromCal.IsExistsColumn("Details"))
				{
					text = Function.ToString(DataRowFromCal["Details"]);
				}
				if (string.IsNullOrEmpty(text.Trim()))
				{
					Text = "Task - ...";
				}
				else
				{
					string[] array = text.Trim().Split(' ');
					if (array.Length >= 4)
					{
						Text = "Task - " + array[0] + " " + array[1] + " " + array[2] + " " + array[3];
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
						Text = "Task - " + text;
						if (Text.Length > 30)
						{
							Text = "Task - " + Text.Substring(0, 30);
						}
					}
				}
			}
			else
			{
				splitListTask.CollapsePanel = SplitCollapsePanel.Panel2;
				btnOK.Visible = false;
			}
			await LoadForm();
			_isLoading = false;
			await AfterLoad();
			gv_List.OptionsView.ColumnAutoWidth = true;
			gv_ListCompleted.OptionsView.ColumnAutoWidth = true;
			gv_ListSub.OptionsView.ColumnAutoWidth = true;
			UpdateGridSize(gv_List);
			UpdateGridSize(gv_ListCompleted);
			CheckValueChanging(this);
			await Task.WhenAll(WinFuntions.LoadUdfColumns(gv_List, "APZ_OCLG"), WinFuntions.LoadUdfColumns(gv_ListCompleted, "APZ_OCLG"));
			await LoadTaskManagementSetting(Function.ToString(GlobalData.UserID));
			gv_List.TopRowIndex = 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			_isLoading = false;
		}
	}

	private async Task LoadForm()
	{
		_ = 7;
		try
		{
			Task t2 = cfl_GroupUser.Properties.RefreshDataSourceAsync();
			Task t3 = cfl_GroupEmployee.Properties.RefreshDataSourceAsync();
			Task<int> t6 = layout_UDF.LoadControls("APZ_OCLG", ApzObjectTypes.oTaskManagement);
			Task task = LoadGridData();
			Task t7 = cfl_ProjectWBS.Properties.RefreshDataSourceAsync();
			Task t8 = txtOwner.Properties.RefreshDataSourceAsync();
			await task;
			CreateAssignedList();
			await t2;
			dictionUser.Clear();
			dictionUser = cfl_GroupUser.Properties.dictionSource.ToDictionary((KeyValuePair<string, string> s) => Function.ParseInt(s.Key), (KeyValuePair<string, string> s) => s.Value);
			AddAccordionElementUser();
			await t3;
			dictionEmployee.Clear();
			dictionEmployee = cfl_GroupEmployee.Properties.dictionSource.ToDictionary((KeyValuePair<string, string> s) => Function.ParseInt(s.Key), (KeyValuePair<string, string> s) => s.Value);
			AddAccordionElementEmployee();
			rep_cfl_ProjectEntry.Assign(btn_PrjGrid);
			btn_PrjGrid.ValueMember = "AbsEntry";
			btn_PrjGrid.DisplayMember = "Code";
			btn_PrjGrid.DisplayMember2 = "Name";
			btn_PrjGrid.SeparateString = " - ";
			btn_PrjGrid.TypeCFL = Enums.TypeCFL.ProjectManagerAll;
			btn_PrjGrid.RefreshDataSource();
			cfl_ProjectNew.Properties.ValueMember = "AbsEntry";
			cfl_ProjectNew.Properties.DisplayMember = "Code";
			cfl_ProjectNew.Properties.DisplayMember2 = "Name";
			cfl_ProjectNew.Properties.SeparateString = " - ";
			cfl_ProjectNew.Properties.TypeCFL = Enums.TypeCFL.ProjectManagerAll;
			RepositoryItemButtonEditCFL properties = cfl_ProjectNew.Properties;
			properties.HandleChooseItem = (Action<DataRow>)Delegate.Combine(properties.HandleChooseItem, new Action<DataRow>(ProjCode_HandleChooseItem));
			cfl_ProjectNew.Properties.TypeDrillDown = ApzObjectTypes.oProjectManager;
			await cfl_ProjectNew.Properties.RefreshDataSourceAsync();
			AddAccordionElementProjectNew();
			gv_List.RefreshData();
			await t6;
			foreach (object control in layout_UDF.Controls)
			{
				if (control is BaseEdit baseEdit)
				{
					baseEdit.BorderStyle = BorderStyles.NoBorder;
					baseEdit.Validating += Control_Validating;
					baseEdit.Validated += Control_Validated;
				}
			}
			await t7;
			DataTable dataTable = await _httpClient.SubActivities.GetEmployeeByUserId(GlobalData.UserID);
			if (dataTable.IsNotNull())
			{
				_owner = Function.ParseInt(dataTable.Rows[0][0]);
				txtOwner.EditValue = _owner;
			}
			await t8;
			RepositoryItemButtonEditCFL properties2 = cfl_Task.Properties;
			properties2.BeforeShowCFL = (Action<frm_CFL>)Delegate.Combine(properties2.BeforeShowCFL, new Action<frm_CFL>(Predecision_BeforeShowCfl));
			luk_TaskMode.SetDataSource(new Dictionary<string, string>
			{
				{ "M", "Manually Scheduled" },
				{ "A", "Auto Scheduled" }
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void Predecision_BeforeShowCfl(frm_CFL obj)
	{
		try
		{
			obj.orderId = ClgCode;
			obj.CflMutiSelectMode = true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void CreateAssignedList()
	{
		listAssignedUser.Clear();
		listAssignedEmployee.Clear();
		listAssignedProjectNew.Clear();
		foreach (DataRow item in grd_List.DataSourceTable.AsEnumerable())
		{
			AddUserToListAssignedUser(item);
			AddEmployeeToListAssignedEmployee(item);
			AddProjectNewToListAssignedProjectNew(item);
		}
	}

	private void AddProjectNewToListAssignedProjectNew(DataRow dr)
	{
		int num = Function.ParseInt(dr["ProjectEntry"]);
		if (num != 0 && !listAssignedProjectNew.Contains(num))
		{
			listAssignedProjectNew.Add(num);
		}
	}

	private void AddEmployeeToListAssignedEmployee(DataRow dr)
	{
		string text = Function.ToString(dr["GroupEmployee"]);
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		string[] array = text.Split(new char[1] { ';' }, StringSplitOptions.RemoveEmptyEntries);
		for (int i = 0; i < array.Length; i++)
		{
			int item = Function.ParseInt(array[i]);
			if (!listAssignedEmployee.Contains(item))
			{
				listAssignedEmployee.Add(item);
			}
		}
	}

	private void AddUserToListAssignedUser(DataRow dr)
	{
		string text = Function.ToString(dr["GroupUser"]);
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		string[] array = text.Split(new char[1] { ';' }, StringSplitOptions.RemoveEmptyEntries);
		for (int i = 0; i < array.Length; i++)
		{
			int item = Function.ParseInt(array[i]);
			if (!listAssignedUser.Contains(item))
			{
				listAssignedUser.Add(item);
			}
		}
	}

	private void AddAccordionElementProjectNew()
	{
		eleProjectNew.Elements.Clear();
		foreach (KeyValuePair<string, string> item in cfl_ProjectNew.Properties.dictionSource.OrderBy((KeyValuePair<string, string> s) => s.Value))
		{
			string key = item.Key;
			string text = Function.ToString(item.Value);
			if (listAssignedProjectNew.Contains(Function.ParseInt(key)))
			{
				AccordionControlElement accordionControlElement = new AccordionControlElement();
				accordionControlElement.Text = text;
				accordionControlElement.Tag = key;
				accordionControlElement.Style = ElementStyle.Item;
				accordionControlElement.Appearance.Normal.Font = new Font("Verdana", 8.25f);
				accordionControlElement.Appearance.Normal.ForeColor = elementColor;
				eleProjectNew.Elements.Add(accordionControlElement);
			}
		}
	}

	private void AddAccordionElementUser()
	{
		eleUser.Elements.Clear();
		AccordionControlElement accordionControlElement = new AccordionControlElement();
		accordionControlElement.Text = "(Unassigned)";
		accordionControlElement.Tag = "Unassigned";
		accordionControlElement.Style = ElementStyle.Item;
		accordionControlElement.Appearance.Normal.Font = new Font("Verdana", 8.25f);
		accordionControlElement.Appearance.Normal.ForeColor = elementColor;
		eleUser.Elements.Add(accordionControlElement);
		foreach (KeyValuePair<int, string> item in dictionUser.OrderBy((KeyValuePair<int, string> s) => s.Value))
		{
			int key = item.Key;
			string text = Function.ToString(item.Value);
			if (listAssignedUser.Contains(key))
			{
				AccordionControlElement accordionControlElement2 = new AccordionControlElement();
				accordionControlElement2.Text = text;
				accordionControlElement2.Tag = key;
				accordionControlElement2.Style = ElementStyle.Item;
				accordionControlElement2.Appearance.Normal.Font = new Font("Verdana", 8.25f);
				accordionControlElement2.Appearance.Normal.ForeColor = elementColor;
				eleUser.Elements.Add(accordionControlElement2);
			}
		}
	}

	private void AddAccordionElementEmployee()
	{
		eleEmployee.Elements.Clear();
		AccordionControlElement accordionControlElement = new AccordionControlElement();
		accordionControlElement.Text = "(Unassigned)";
		accordionControlElement.Tag = "Unassigned";
		accordionControlElement.Style = ElementStyle.Item;
		accordionControlElement.Appearance.Normal.Font = new Font("Verdana", 8.25f);
		accordionControlElement.Appearance.Pressed.Font = new Font("Verdana", 8.25f);
		accordionControlElement.Appearance.Normal.ForeColor = elementColor;
		eleEmployee.Elements.Add(accordionControlElement);
		foreach (KeyValuePair<int, string> item in dictionEmployee.OrderBy((KeyValuePair<int, string> s) => s.Value))
		{
			int key = item.Key;
			Function.ToString(item.Value);
			if (listAssignedEmployee.Contains(key))
			{
				AccordionControlElement accordionControlElement2 = new AccordionControlElement();
				accordionControlElement2.Text = Function.ToString(item.Value);
				accordionControlElement2.Tag = item.Key;
				accordionControlElement2.Style = ElementStyle.Item;
				accordionControlElement2.Appearance.Normal.Font = new Font("Verdana", 8.25f);
				accordionControlElement2.Appearance.Normal.ForeColor = elementColor;
				eleEmployee.Elements.Add(accordionControlElement2);
			}
		}
	}

	private void frm_TaskManagement_Shown(object sender, EventArgs e)
	{
	}

	private async Task RepositoryLoading()
	{
	}

	private async Task AfterLoad()
	{
		if (DataRowFromCal == null)
		{
			return;
		}
		if (DataRowFromCal.Table != null)
		{
			DataTable dataTable = DataRowFromCal.Table.Clone();
			dataTable.PrimaryKey = null;
			dataTable.ImportRow(DataRowFromCal);
			if (dataTable.ExistsColumn("Priority"))
			{
				dataTable.Rows[0]["Priority"] = 1;
			}
			try
			{
				if (dataTable.ExistsColumn("ResName"))
				{
					dataTable.Columns.Remove("ResName");
				}
				if (dataTable.ExistsColumn("ResourceId"))
				{
					dataTable.Columns.Remove("ResourceId");
				}
				if (dataTable.ExistsColumn("startDateTime"))
				{
					dataTable.Columns.Remove("startDateTime");
				}
				if (dataTable.ExistsColumn("endDateTime"))
				{
					dataTable.Columns.Remove("endDateTime");
				}
				if (dataTable.ExistsColumn("LocationCode"))
				{
					dataTable.Columns.Remove("LocationCode");
				}
				if (dataTable.ExistsColumn("ProjectCode"))
				{
					dataTable.Columns.Remove("ProjectCode");
				}
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			}
			dataTable.AcceptChanges();
			await FillInfo(dataTable.Rows[0]);
		}
		else
		{
			await FillInfo(DataRowFromCal);
		}
	}

	private async Task LoadGridData()
	{
		try
		{
			dtAPZ_OCLG = await LoadTask();
			grd_List.DataSource = dtAPZ_OCLG.Copy();
			grd_ListCompleted.DataSource = dtAPZ_OCLG.Copy();
			GridListAcceptChanges();
			GridListCompletedAcceptChanges();
			_ = gv_List.DataRowCount;
			_ = 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task RefreshSubTask()
	{
		try
		{
			int code = Function.ParseInt(txtClgCode.EditValue);
			DataTable dataTable = await LoadSubTask(code);
			if (dataTable.HasColumn())
			{
				grd_ListSub.DataSource = dataTable;
			}
			else
			{
				grd_ListSub.DataSource = dtAPZ_OCLG.Clone();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<DataTable> LoadSubTask(int _code)
	{
		try
		{
			DataTable dataTable = new DataTable();
			if (_code != 0)
			{
				string userCodeLogin = GlobalData.UserCodeLogin;
				dataTable = await _httpClient.SubActivities.LoadSubTask(_code, userCodeLogin);
			}
			if (!dataTable.HasColumn())
			{
				dataTable = ((!dtAPZ_OCLG.HasColumn()) ? WinFuntions.GetTableStructure(gv_List.ItemId) : dtAPZ_OCLG.Clone());
			}
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	private async Task<DataTable> LoadSubNode(int _code)
	{
		try
		{
			DataTable dataTable = new DataTable();
			if (_code != 0)
			{
				string userCodeLogin = GlobalData.UserCodeLogin;
				dataTable = await _httpClient.SubActivities.LoadSubNode(_code, userCodeLogin);
			}
			if (!dataTable.HasColumn())
			{
				dataTable = ((!dtAPZ_OCLG.HasColumn()) ? WinFuntions.GetTableStructure(gv_List.ItemId) : dtAPZ_OCLG.Clone());
			}
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	private async Task LoadParentTask(int parentTask)
	{
		try
		{
			DataTable dataTable = await _httpClient.SubActivities.LoadParentTask(parentTask);
			if (!dataTable.HasColumn())
			{
				brc_ParentTask.Visible = false;
				brc_ParentTask.Path = "";
				return;
			}
			brc_ParentTask.Visible = true;
			dictionNodes.Clear();
			foreach (DataRow item in dataTable.AsEnumerable())
			{
				int key = Function.ParseInt(item["ClgCode"]);
				string value = Function.ToString(item["Details"]);
				dictionNodes.Add(key, value);
			}
			dictionNodes.Add(Function.ParseInt(txtClgCode.EditValue), Function.ToString(txtDetails.Text));
			string[] value2 = dictionNodes.Select((KeyValuePair<int, string> t) => t.Key.ToString()).ToArray();
			pathTask = string.Join(brc_ParentTask.Properties.PathSeparator, value2);
			brc_ParentTask.Path = pathTask;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadTaskManagementSetting(string userSign)
	{
		if (CallFromCal)
		{
			return;
		}
		try
		{
			loadTaskManagementSetting = true;
			dtAPZ_CLG2 = await _httpClient.SubActivities.LoadTaskManagementSetting(userSign);
			if (!dtAPZ_CLG2.HasColumn())
			{
				dtAPZ_CLG2 = await WinFuntions.GetTableStructureAsync("APZ_CLG2");
			}
			else
			{
				if (!dtAPZ_CLG2.IsNotNull())
				{
					return;
				}
				string text = Function.ToString(dtAPZ_CLG2.Rows[0]["filterBy"]);
				if (text.StartsWithInvariantCulture("U_"))
				{
					string text2 = Function.ToString(text.Substring(2));
					foreach (AccordionControlElement element in eleUser.Elements)
					{
						string value = Function.ToString(element.Tag);
						if (text2.Equals(value))
						{
							eleUser.Expanded = true;
							accordionControl1.SelectedElement = element;
							break;
						}
					}
				}
				if (text.StartsWithInvariantCulture("E_"))
				{
					string text3 = Function.ToString(text.Substring(2));
					foreach (AccordionControlElement element2 in eleEmployee.Elements)
					{
						string value2 = Function.ToString(element2.Tag);
						if (text3.Equals(value2))
						{
							eleEmployee.Expanded = true;
							accordionControl1.SelectedElement = element2;
							break;
						}
					}
				}
				if (text.StartsWithInvariantCulture("P_"))
				{
					string text4 = Function.ToString(text.Substring(2));
					foreach (AccordionControlElement element3 in eleProjectNew.Elements)
					{
						string value3 = Function.ToString(element3.Tag);
						if (text4.Equals(value3))
						{
							eleProjectNew.Expanded = true;
							accordionControl1.SelectedElement = element3;
							break;
						}
					}
				}
				if (!text.StartsWithInvariantCulture("U_") && !text.StartsWithInvariantCulture("E_") && !text.StartsWithInvariantCulture("P_"))
				{
					foreach (AccordionControlElement element4 in eleGeneral.Elements)
					{
						string value4 = Function.ToString(element4.Tag);
						if (text.Equals(value4))
						{
							eleGeneral.Expanded = true;
							accordionControl1.SelectedElement = element4;
							break;
						}
					}
				}
				string sortBy = Function.ToString(dtAPZ_CLG2.Rows[0]["sortBy"]);
				if (dtSort.AsEnumerable().Any((DataRow dr) => Function.ToString(dr["Code"]).Equals(sortBy)))
				{
					luk_Sort.EditValue = sortBy;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		finally
		{
			loadTaskManagementSetting = false;
		}
	}

	public async Task REFRESH_DATA()
	{
		splitListTask.Collapsed = true;
		loadCompletedTask = false;
		grd_ListCompleted.Visible = false;
		await frm_TaskManagement_Load(null, null);
	}

	public async Task<DataTable> LoadTask()
	{
		try
		{
			DataTable dataTable = null;
			if (!CallFromCal)
			{
				string userCodeLogin = GlobalData.UserCodeLogin;
				dataTable = await _httpClient.SubActivities.LoadTask(userCodeLogin, "N");
			}
			if (!dataTable.HasColumn())
			{
				dataTable = WinFuntions.GetTableStructure(gv_List.ItemId);
			}
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	public async Task<DataTable> LoadCompletedTask()
	{
		try
		{
			DataTable dataTable = null;
			if (!CallFromCal)
			{
				string userCodeLogin = GlobalData.UserCodeLogin;
				dataTable = await _httpClient.SubActivities.LoadTask(userCodeLogin, "Y");
			}
			if (!dataTable.HasColumn())
			{
				dataTable = WinFuntions.GetTableStructure(gv_List.ItemId);
			}
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	private async void gv_List_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			if (!(e.Row is DataRowView dataRowView) || CallFromCal)
			{
				return;
			}
			GridView obj = sender as GridView;
			GridHitInfo gridHitInfo = obj.CalcHitInfo(obj.GridControl.PointToClient(Cursor.Position));
			if (!gridHitInfo.InRowCell || gridHitInfo.Column == null || !(gridHitInfo.Column.FieldName == "Closed") || !(Function.ToString(gv_List.GetFocusedRowCellValue("OwnerAuth")) == "F"))
			{
				changingRowObject = true;
				if (Function.ToString(dataRowView["ClgCode"]) != Function.ToString(txtClgCode.EditValue))
				{
					await FillInfo(dataRowView.Row);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			changingRowObject = false;
		}
	}

	private async void gv_List_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
	{
		try
		{
			if (FormAuth == Enums.FormAuthorities.Read)
			{
				removeToolStripMenuItem.Enabled = false;
				toolStripMenuItem_DeleteCM.Enabled = false;
				removeToolStripMenuItem1.Enabled = false;
			}
			else
			{
				removeToolStripMenuItem.Enabled = true;
				toolStripMenuItem_DeleteCM.Enabled = true;
				removeToolStripMenuItem1.Enabled = true;
			}
			if (CallFromCal)
			{
				return;
			}
			GridView gridView = sender as GridView;
			GridHitInfo gridHitInfo = gridView.CalcHitInfo(gridView.GridControl.PointToClient(Cursor.Position));
			if (!gridHitInfo.InRowCell || gridHitInfo.Column == null || !(gridHitInfo.Column.FieldName == "Closed"))
			{
				changingRowFocus = true;
				DataRow dataRow;
				if (e.FocusedRowHandle == -2147483647 || e.FocusedRowHandle == int.MinValue)
				{
					dataRow = (gridView.GridControl.DataSource as DataTable).NewRow();
					dataRow["Owner"] = _owner;
				}
				else
				{
					dataRow = gridView.GetDataRow(e.FocusedRowHandle);
				}
				Function.ToString(dataRow["ClgCode"]);
				if (focusedView == gv_List && e.FocusedRowHandle == -2147483647)
				{
					splitListTask.Collapsed = true;
				}
				if (Function.ToString(dataRow["ClgCode"]) != Function.ToString(txtClgCode.EditValue))
				{
					await FillInfo(dataRow);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			changingRowFocus = false;
		}
	}

	private async void gv_List_RowClick(object sender, RowClickEventArgs e)
	{
		if (rowClick)
		{
			return;
		}
		try
		{
			rowClick = true;
			GridView gridView = sender as GridView;
			if (!Function.ToString(gridView.GetFocusedRowCellValue("ClgCode")).Equals(txtClgCode.Text))
			{
				DataRow focusedDataRow = gridView.GetFocusedDataRow();
				await FillInfo(focusedDataRow);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			rowClick = false;
		}
	}

	private async void ctm_JumpTo_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			XtraInputBoxArgs xtraInputBoxArgs = new XtraInputBoxArgs();
			xtraInputBoxArgs.LookAndFeel = new UserLookAndFeel(xtraInputBoxArgs);
			xtraInputBoxArgs.LookAndFeel.UseDefaultLookAndFeel = false;
			xtraInputBoxArgs.LookAndFeel.SkinName = "Office 2016 Colorful";
			xtraInputBoxArgs.Caption = "Jump To";
			TextEdit textEdit = new TextEdit();
			textEdit.Properties.NullValuePrompt = "Task ID...";
			textEdit.Properties.ShowNullValuePromptWhenFocused = true;
			textEdit.Properties.NullValuePromptShowForEmptyValue = true;
			xtraInputBoxArgs.Editor = textEdit;
			string taskString = Function.ToString(XtraInputBox.Show(xtraInputBoxArgs));
			if (!(taskString == ""))
			{
				int clgCode = Function.ParseInt(taskString);
				string userNameLogin = GlobalData.UserNameLogin;
				DataTable dataTable = await _httpClient.SubActivities.LoadSingleTask(clgCode, userNameLogin);
				if (dataTable.EmptyData())
				{
					WinCommons.Error(Enums.MessageCode.TaskDoesNotExistsOrNotAccess, "", taskString);
				}
				else
				{
					await FillInfo(dataTable.Rows[0]);
					splitListTask.Collapsed = false;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task FillInfo(int clgCode)
	{
		try
		{
			if (dtAPZ_OCLG.HasColumn())
			{
				DataRow dataRow = dtAPZ_OCLG.NewRow();
				if (clgCode != 0)
				{
					dataRow["ClgCode"] = clgCode;
				}
				await FillInfo(dataRow);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task FillInfo(DataRow focusedRowFromGrid)
	{
		if (isFill || _isLoading || acceptChange || changeRowStatus || isSort || copyTask || focusedRowFromGrid == null || StayToSave())
		{
			return;
		}
		try
		{
			isFill = true;
			dtRow = dtAPZ_OCLG.Clone();
			DataRow dr = dtRow.NewRow();
			_ = FormAuth;
			_ = 2;
			int num = Function.ParseInt(focusedRowFromGrid["ClgCode"]);
			if (num != 0)
			{
				string userCodeLogin = GlobalData.UserCodeLogin;
				DataTable dataTable = await _httpClient.SubActivities.LoadSingleTask(num, userCodeLogin);
				if (dataTable.IsNotNull())
				{
					dr.ItemArray = dataTable.Rows[0].ItemArray;
				}
			}
			else
			{
				dr.ItemArray = focusedRowFromGrid.ItemArray;
				dr["OwnerAuth"] = "F";
			}
			if (dr["ClgCode"] == null || dr["ClgCode"] == DBNull.Value)
			{
				dr["ClgCode"] = 0;
			}
			dtRow.Rows.Add(dr);
			dtRow.AcceptChanges();
			if (dtRow.IsNotNull())
			{
				Function.ToString(dtRow.Rows[0]["GroupUser"]);
				if (!string.IsNullOrEmpty(Function.ToString(dtRow.Rows[0]["GroupEmployee"])))
				{
					cbUser.EditValue = "Employee";
				}
				else
				{
					cbUser.EditValue = "User";
				}
				ucLinkedDocument1.EditValue = dr["DocNum"];
				ucLinkedDocument1.DocEntry = dr["DocEntry"];
				ucLinkedDocument1.DocType = Function.ParseInt(dr["DocType"]);
				SetBottomLabelText(dr);
				DataView defaultView = dtRow.DefaultView;
				txtDetails.DataBindings.Clear();
				txtDetails.DataBindings.Add("Text", defaultView, "Details");
				BindData(txtClgCode, defaultView, "ClgCode");
				BindData(cfl_GroupUser, defaultView, "GroupUser", DataSourceUpdateMode.OnPropertyChanged);
				bte_DistrRule.SetEditValue(dtRow.Rows[0]);
				BindData(date_DueDate, defaultView, "endDate");
				memoNotes.DataBindings.Clear();
				memoNotes.DataBindings.Add("Text", defaultView, "Notes");
				BindData(date_DueTime, defaultView, "ENDTime", DataSourceUpdateMode.OnPropertyChanged);
				BindData(date_StartDate, defaultView, "Recontact");
				BindData(date_StartTime, defaultView, "BeginTime");
				BindData(chk_Closed, defaultView, "Closed");
				BindData(cfl_BPCode, defaultView, "CardCode", DataSourceUpdateMode.OnPropertyChanged);
				BindData(cfl_ProjectNew, defaultView, "ProjectEntry");
				BindData(cfl_ProjectWBS, defaultView, "ProjectWBS", DataSourceUpdateMode.OnPropertyChanged);
				BindData(luk_Priority, defaultView, "Priority", DataSourceUpdateMode.OnPropertyChanged);
				BindData(cfl_GroupEmployee, defaultView, "GroupEmployee", DataSourceUpdateMode.OnPropertyChanged);
				BindData(cfl_GroupUser, defaultView, "GroupUser", DataSourceUpdateMode.OnPropertyChanged);
				BindData(cfl_Opportunity, defaultView, "OprId", DataSourceUpdateMode.OnPropertyChanged);
				BindData(cfl_Task, defaultView, "Predecessors", DataSourceUpdateMode.OnPropertyChanged);
				BindData(luk_TaskMode, defaultView, "TaskMode", DataSourceUpdateMode.OnPropertyChanged);
				BindData(txtOwner, defaultView, "Owner", DataSourceUpdateMode.OnPropertyChanged);
				foreach (Control control in layout_UDF.Controls)
				{
					if (control is BaseEdit && dr.Table.Columns.Contains(control.Name))
					{
						BindData(control, defaultView, control.Name, DataSourceUpdateMode.OnPropertyChanged);
						if (FormMode == FormModeType.SingleAdd)
						{
							(control as BaseEdit).EditValue = control.AccessibleDescription;
						}
					}
				}
				List<Task> list = new List<Task>();
				int absEntry = Function.ParseInt(dtRow.Rows[0]["AtcEntry"]);
				Task item = ucAttachment1.LoadAttachmentsAsync(absEntry);
				Task item2 = RefreshSubTask();
				int id = Function.ParseInt(txtClgCode.Text);
				Task item3 = ucComment1.RefreshComment(id);
				Task item4 = cfl_ProjectWBS.Properties.RefreshDataSourceAsync();
				int num2 = Function.ParseInt(dtRow.Rows[0]["parentTask"]);
				if (num2 != 0)
				{
					IsSubTask = true;
					Task item5 = LoadParentTask(num2);
					list.Add(item5);
				}
				else
				{
					IsSubTask = false;
					brc_ParentTask.Visible = false;
				}
				list.Add(item);
				list.Add(item2);
				list.Add(item3);
				list.Add(item4);
				await Task.WhenAll(list);
				if (!string.IsNullOrEmpty(Function.ToString(cfl_BPCode.EditValue)) || !string.IsNullOrEmpty(Function.ToString(bte_DistrRule.EditValue)) || !string.IsNullOrEmpty(Function.ToString(ucLinkedDocument1.cbDocNum.EditValue)) || !string.IsNullOrEmpty(Function.ToString(cfl_ProjectWBS.EditValue)) || !string.IsNullOrEmpty(Function.ToString(cfl_ProjectNew.EditValue)))
				{
					splitMoreInformation.Collapsed = false;
				}
				else if (!openInformationSplit && FormMode != FormModeType.SingleAdd)
				{
					splitMoreInformation.Collapsed = true;
				}
				if (gv_ListSub.DataRowCount > 0)
				{
					splitSubTask.Collapsed = false;
				}
				if (!openSubTaskSplit && gv_ListSub.DataRowCount <= 0)
				{
					splitSubTask.Collapsed = true;
				}
				if (ucAttachment1.gv_File.DataRowCount > 0)
				{
					splitAttachment.Collapsed = false;
				}
				if (!openAttachmentSplit && ucAttachment1.gv_File.DataRowCount <= 0)
				{
					splitAttachment.Collapsed = true;
				}
				if (FormMode == FormModeType.SingleView)
				{
					int num3 = 0;
					if (!splitMoreInformation.Collapsed)
					{
						singleModeSize.Height += 120;
						num3++;
					}
					if (!splitSubTask.Collapsed)
					{
						singleModeSize.Height += 120;
					}
					if (!splitAttachment.Collapsed)
					{
					}
					Size = singleModeSize;
					SetMinScrollSize(singleModeSize);
				}
			}
			if (!Function.ToString(dr["OwnerAuth"]).Equals("F"))
			{
				readOnly = true;
				Controls.EnableDisableControl(isEnable: false);
				if (ContextMenuStrip != null)
				{
					ContextMenuStrip.Enabled = false;
				}
			}
			else
			{
				readOnly = false;
				Controls.EnableDisableControl(isEnable: true);
				if (ContextMenuStrip != null)
				{
					ContextMenuStrip.Enabled = true;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			isFill = false;
			modifyInAddMode = false;
		}
	}

	private void SetBottomLabelText(DataRow dr)
	{
		int num = Function.ParseInt(dr["AssignedBy"]);
		int num2 = Function.ParseInt(dr["GroupUser"]);
		int num3 = Function.ParseInt(dr["ClgCode"]);
		lblCreatedAt.Text = "";
		lblCreatedAt.ToolTip = "";
		string text = Function.ToString(dr["Closed"]);
		DateTime dateTime = Function.ParseDateTimes(dr["CntctDate"]);
		string text2 = Function.ToString(dr["CntctTime"]);
		string text3 = Function.ToString(dr["CloseDate"]);
		int key = Function.ParseInt(dr["AttendUser"]);
		string text4 = "";
		string text5 = ((text2.Length < 2) ? Function.ToString(dateTime.Date.Add(new TimeSpan(0, Function.ParseInt(text2), 0))) : Function.ToString(dateTime.Date.Add(new TimeSpan(Function.ParseInt(text2) / 100, Function.ParseInt(text2.Substring(text2.Length - 2, 2)), 0))));
		if (num == num2 && text == "Y")
		{
			if (dictionUser.ContainsKey(key))
			{
				text4 = dictionUser[key];
			}
			if (!string.IsNullOrEmpty(text3))
			{
				lblCreatedAt.Text = "Completed " + WinFuntions.FormatCreatedDate(text3, shorten: true, "on ");
			}
			if (dictionUser.ContainsKey(num))
			{
				string text6 = dictionUser[num];
				lblCreatedAt.ToolTip = "Completed by " + text4 + Environment.NewLine + "Created by " + text6 + Environment.NewLine + "ID: " + num3;
			}
		}
		if (num == num2 && text != "Y")
		{
			if (!string.IsNullOrEmpty(text5))
			{
				lblCreatedAt.Text = "Created " + WinFuntions.FormatCreatedDate(text5, shorten: true, "on ");
			}
			if (dictionUser.ContainsKey(num))
			{
				string text7 = dictionUser[num];
				lblCreatedAt.ToolTip = "Created by " + text7 + Environment.NewLine + "ID: " + num3;
			}
		}
		if (num != num2 && text == "Y")
		{
			if (dictionUser.ContainsKey(key))
			{
				text4 = dictionUser[key];
			}
			if (!string.IsNullOrEmpty(text3))
			{
				lblCreatedAt.Text = "Completed " + WinFuntions.FormatCreatedDate(text3, shorten: true, "on ") + ", by " + text4;
			}
			if (dictionUser.ContainsKey(num))
			{
				string text8 = dictionUser[num];
				lblCreatedAt.ToolTip = "Created by " + text8 + Environment.NewLine + "ID: " + num3;
			}
		}
		if (num != num2 && text != "Y" && dictionUser.ContainsKey(num))
		{
			string text9 = dictionUser[num];
			if (!string.IsNullOrEmpty(text5))
			{
				lblCreatedAt.Text = "Created " + WinFuntions.FormatCreatedDate(text5, shorten: true, "on ") + ", by " + text9;
			}
			lblCreatedAt.ToolTip = "ID: " + num3;
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

	public static string FormatCreatedDate(string s, bool shorten = false)
	{
		try
		{
			if (!DateTime.TryParse(s, out var result))
			{
				return "";
			}
			TimeSpan timeSpan = DateTime.Now - result;
			if (timeSpan < new TimeSpan(0, 0, 10))
			{
				return shorten ? "few seconds ago" : "few seconds ago";
			}
			if (timeSpan < new TimeSpan(0, 0, 60))
			{
				return timeSpan.Seconds + " seconds ago";
			}
			if (timeSpan < new TimeSpan(0, 60, 0))
			{
				string text = ((timeSpan.Minutes > 1) ? "s" : "");
				return timeSpan.Minutes + " minute" + text + " ago";
			}
			if (timeSpan < new TimeSpan(24, 0, 0))
			{
				string text2 = ((timeSpan.Hours > 1) ? "s" : "");
				return timeSpan.Hours + " hour" + text2 + " ago";
			}
			if (timeSpan < new TimeSpan(48, 0, 0))
			{
				return "Yesterday";
			}
			if (timeSpan < new TimeSpan(168, 0, 0))
			{
				string text3 = ((timeSpan.Days > 1) ? "s" : "");
				return timeSpan.Days + " day" + text3 + " ago";
			}
			return shorten ? result.Date.ToShortDateString() : result.Date.ToShortDateString();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return "";
	}

	private async void gridControl1_ProcessGridKey(object sender, KeyEventArgs e)
	{
		if (processKey)
		{
			return;
		}
		try
		{
			_ = (sender as GridControl).MainView;
			if (e.Modifiers == Keys.Control && e.KeyCode == Keys.V)
			{
				string text = Clipboard.GetText();
				if (!text.Contains(Environment.NewLine))
				{
					return;
				}
				processKey = true;
				string[] splitTextList = text.Split(new string[1] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
				DataTable dt = grd_List.DataSourceTable;
				double maxOrderLine = Function.ParseDouble(dt.Compute("MAX(OrderLine)", ""));
				for (int i = splitTextList.Length - 1; i >= 0; i--)
				{
					DataRow dataRow = dt.NewRow();
					dataRow["ClgCode"] = 0;
					dataRow["Details"] = splitTextList[i];
					dataRow["OrderLine"] = maxOrderLine + 1.0;
					maxOrderLine++;
					dt.Rows.InsertAt(dataRow, 0);
					await AddMainTask(dataRow);
				}
				dt.AcceptChanges();
				e.SuppressKeyPress = true;
			}
			if (e.KeyCode == Keys.Return && gv_List.FocusedRowHandle == -2147483647)
			{
				processKey = true;
				gv_List.SetFocusedRowCellValue("ClgCode", 0);
				gv_List.FocusedRowHandle++;
				e.Handled = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			processKey = false;
		}
	}

	private void grd_List_KeyPress(object sender, KeyPressEventArgs e)
	{
		if (e.KeyChar == '\u0016')
		{
			e.Handled = true;
		}
	}

	private void chk_Closed_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading && !changingRowFocus && !changingRowObject && !changeRowStatus && FormAuth != Enums.FormAuthorities.Read)
			{
				isModified = chk_Closed.IsModified;
				BeginInvoke((MethodInvoker)(() =>
				{
					chk_Closed.DoValidate();
				}));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void txtDetails_KeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.Return)
			{
				e.SuppressKeyPress = true;
				(sender as Control).Parent.SelectNextControl(ActiveControl, forward: true, tabStopOnly: true, nested: true, wrap: true);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ucLinkedDocument1_AfterValidating(object sender, EventArgs e)
	{
		try
		{
			ucLinkedDocument ucLinkedDocument2 = sender as ucLinkedDocument;
			if (ucLinkedDocument2.cbDocNum.IsModified)
			{
				isModified = true;
				string cardCode = ucLinkedDocument2.CardCode;
				if (cardCode != "")
				{
					cfl_BPCode.EditValue = cardCode;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ucLinkedDocument1_ItemSelected(object sender, EventArgs e)
	{
		isModified = true;
	}

	private async void cfl_ProjectNew_Validating(object sender, CancelEventArgs e)
	{
		try
		{
			ButtonEditCFL buttonEditCFL = sender as ButtonEditCFL;
			if (buttonEditCFL.IsModified)
			{
				CheckModified(buttonEditCFL);
				cfl_ProjectWBS.EditValue = null;
				await cfl_ProjectWBS.Properties.RefreshDataSourceAsync();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void GetAutoWBS()
	{
		try
		{
			DateTime? startDate = Function.ParseDateTime(date_StartDate.EditValue);
			if (!startDate.HasValue)
			{
				return;
			}
			DateTime? dueDate = Function.ParseDateTime(date_DueDate.EditValue);
			if (!dueDate.HasValue)
			{
				return;
			}
			int projectID = Function.ParseInt(cfl_ProjectNew.EditValue);
			if (projectID == 0 || Function.ToString(cfl_ProjectWBS.EditValue) != "")
			{
				return;
			}
			DataTable dataSource = cfl_ProjectWBS.Properties.DataSource;
			if (dataSource.EmptyData())
			{
				return;
			}
			DataRow[] array = dataSource.AsEnumerable().Where((DataRow dr) =>
			{
				if (Function.ParseInt(dr["AbsEntry"]) == projectID)
				{
					DateTime nonNullDateTime = GetNonNullDateTime(dr["FromDate"]);
					DateTime? dateTime = startDate;
					if (nonNullDateTime >= dateTime)
					{
						nonNullDateTime = GetNonNullDateTime(dr["ToDate"]);
						dateTime = dueDate;
						return nonNullDateTime <= dateTime;
					}
				}
				return false;
			}).ToArray();
			if (array.Count() > 0 && array.Count() <= 1)
			{
				object editValue = array[0]["TaskCode"];
				cfl_ProjectWBS.EditValue = editValue;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private DateTime GetFullStartDate()
	{
		DateTime nonNullDateTime = GetNonNullDateTime(date_StartDate.EditValue);
		TimeSpan timeSpanValue = date_StartTime.TimeSpanValue;
		return nonNullDateTime.Add(timeSpanValue);
	}

	private DateTime GetFullDueDate()
	{
		DateTime nonNullDateTime = GetNonNullDateTime(date_DueDate.EditValue);
		if (nonNullDateTime == DateTime.MinValue)
		{
			return DateTime.MinValue;
		}
		TimeSpan timeSpanValue = date_DueTime.TimeSpanValue;
		return nonNullDateTime.Add(timeSpanValue);
	}

	private DateTime GetNonNullDateTime(object value)
	{
		DateTime? dateTime = Function.ParseDateTime(value);
		if (dateTime.HasValue)
		{
			return dateTime.Value;
		}
		return DateTime.MinValue;
	}

	private void date_DueDate_Validating(object sender, CancelEventArgs e)
	{
		try
		{
			DateEdit dateEdit = sender as DateEdit;
			CheckDueDate(e);
			if (!e.Cancel && dateEdit.IsModified)
			{
				CheckModified(dateEdit);
				GetAutoWBS();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void date_StartDate_Validating(object sender, CancelEventArgs e)
	{
		try
		{
			DateEdit dateEdit = sender as DateEdit;
			if (Function.ToString(dateEdit.EditValue) == "")
			{
				WinCommons.Error(Enums.MessageCode.StartDateIsRequired, "");
				e.Cancel = true;
				return;
			}
			CheckDueDate(e);
			if (!e.Cancel && dateEdit.IsModified)
			{
				CheckModified(dateEdit);
				GetAutoWBS();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void date_DueTime_Validating(object sender, CancelEventArgs e)
	{
		try
		{
			BaseEdit editor = sender as BaseEdit;
			CheckDueTime(e);
			if (!e.Cancel)
			{
				CheckModified(editor);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void date_StartTime_Validating(object sender, CancelEventArgs e)
	{
		try
		{
			BaseEdit editor = sender as BaseEdit;
			CheckDueTime(e);
			if (!e.Cancel)
			{
				CheckModified(editor);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void bte_DistrRule_Validating(object sender, CancelEventArgs e)
	{
		try
		{
			ButtonEditDistrRule buttonEditDistrRule = sender as ButtonEditDistrRule;
			if (buttonEditDistrRule.FromCFL)
			{
				isModified = true;
			}
			else
			{
				CheckModified(buttonEditDistrRule);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void txtDetails_Validating(object sender, CancelEventArgs e)
	{
		try
		{
			if ((sender as TextBox).Modified)
			{
				isModified = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void Control_Validating(object sender, CancelEventArgs e)
	{
		try
		{
			if (!(sender is BaseEdit baseEdit))
			{
				isModified = true;
			}
			else if (baseEdit is ButtonEditCFL && (baseEdit as ButtonEditCFL).Properties.fromCFL)
			{
				isModified = true;
			}
			else
			{
				CheckModified(baseEdit);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void CheckModified(BaseEdit editor)
	{
		if (editor.IsModified)
		{
			isModified = true;
		}
	}

	private void CheckDueTime(CancelEventArgs e)
	{
		string text = Function.ToString(date_DueDate.EditValue);
		if (!(Function.ToString(text) != ""))
		{
			return;
		}
		DateTime dateTime = Function.ParseDateTimes(text);
		DateTime dateTime2 = Function.ParseDateTimes(Function.ToString(date_StartDate.EditValue));
		if (dateTime.Date == dateTime2.Date)
		{
			TimeSpan value = Function.IntSAPToTimeSpan(date_DueTime.EditValue);
			dateTime = dateTime.Add(value);
			TimeSpan value2 = Function.IntSAPToTimeSpan(date_StartTime.EditValue);
			dateTime2 = dateTime2.Add(value2);
			if (dateTime <= dateTime2)
			{
				WinCommons.Error(Enums.MessageCode.DueDateMustBeAfterStartDate, "");
				e.Cancel = true;
			}
		}
	}

	private void CheckDueDate(CancelEventArgs e)
	{
		string text = Function.ToString(date_DueDate.EditValue);
		if (Function.ToString(text) == "")
		{
			return;
		}
		DateTime dateTime = Function.ParseDateTimes(text);
		DateTime dateTime2 = Function.ParseDateTimes(Function.ToString(date_StartDate.EditValue));
		if (dateTime.Date < dateTime2.Date)
		{
			WinCommons.Error(Enums.MessageCode.DueDateMustBeAfterStartDate, "");
			e.Cancel = true;
		}
		if (dateTime.Date == dateTime2.Date)
		{
			TimeSpan timeSpan = Function.IntSAPToTimeSpan(date_StartTime.EditValue);
			dateTime2 = dateTime2.Add(timeSpan);
			TimeSpan timeSpan2 = Function.IntSAPToTimeSpan(date_DueTime.EditValue);
			if (timeSpan2 == new TimeSpan(0, 0, 0))
			{
				timeSpan2 = ((!(timeSpan < new TimeSpan(23, 44, 0))) ? new TimeSpan(23, 59, 0) : timeSpan.Add(new TimeSpan(0, 15, 0)));
				date_DueTime.EditValue = Function.ParseTimeSpanToInt(timeSpan2);
			}
			dateTime = dateTime.Add(timeSpan2);
			if (dateTime <= dateTime2)
			{
				WinCommons.Error(Enums.MessageCode.DueDateMustBeAfterStartDate, "");
				e.Cancel = true;
			}
		}
	}

	private async void Control_Validated(object sender, EventArgs e)
	{
		try
		{
			if (!changingRowObject && !changingRowFocus && !CallFromCal && isModified)
			{
				Control editor = sender as Control;
				isValidate = true;
				DataRow dr = dtRow.Rows[0];
				if (editor == chk_Closed)
				{
					ChangeTaskStatusByClickOnCheck(dr);
					return;
				}
				await SaveCurrentTask(dr);
				RefreshAccordionAfterValidate(editor);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			isModified = false;
			isValidate = false;
			txtDetails.Modified = false;
			memoNotes.Modified = false;
		}
	}

	private void RefreshAccordionAfterValidate(Control editor)
	{
		if (editor == cfl_ProjectNew)
		{
			if (accordionControl1.SelectedElement != null && accordionControl1.SelectedElement.OwnerElement == eleProjectNew)
			{
				return;
			}
			int num = Function.ParseInt(cfl_ProjectNew.EditValue);
			if (listAssignedProjectNew.Contains(num) || !cfl_ProjectNew.Properties.dictionSource.ContainsKey(Function.ToString(num)))
			{
				return;
			}
			string text = cfl_ProjectNew.Properties.dictionSource[Function.ToString(num)];
			AccordionControlElement accordionControlElement = new AccordionControlElement();
			accordionControlElement.Text = text;
			accordionControlElement.Tag = num;
			accordionControlElement.Style = ElementStyle.Item;
			accordionControlElement.Appearance.Normal.Font = new Font("Verdana", 8.25f);
			eleProjectNew.Elements.Add(accordionControlElement);
			listAssignedProjectNew.Add(num);
		}
		string[] array;
		if (editor == cfl_GroupUser)
		{
			if (accordionControl1.SelectedElement != null && accordionControl1.SelectedElement.OwnerElement == eleUser)
			{
				return;
			}
			array = Function.ToString(cfl_GroupUser.EditValue).Split(new char[1] { ';' }, StringSplitOptions.RemoveEmptyEntries);
			foreach (string text2 in array)
			{
				int num2 = Function.ParseInt(text2);
				if (!listAssignedUser.Contains(num2) && cfl_GroupUser.Properties.dictionSource.ContainsKey(text2))
				{
					string text3 = cfl_GroupUser.Properties.dictionSource[text2];
					AccordionControlElement accordionControlElement2 = new AccordionControlElement();
					accordionControlElement2.Text = text3;
					accordionControlElement2.Tag = num2;
					accordionControlElement2.Style = ElementStyle.Item;
					accordionControlElement2.Appearance.Normal.Font = new Font("Verdana", 8.25f);
					eleUser.Elements.Add(accordionControlElement2);
					listAssignedUser.Add(num2);
				}
			}
		}
		if (editor != cfl_GroupEmployee || (accordionControl1.SelectedElement != null && accordionControl1.SelectedElement.OwnerElement == eleEmployee))
		{
			return;
		}
		array = Function.ToString(cfl_GroupEmployee.EditValue).Split(new char[1] { ';' }, StringSplitOptions.RemoveEmptyEntries);
		foreach (string text4 in array)
		{
			int num3 = Function.ParseInt(text4);
			if (!listAssignedEmployee.Contains(num3) && cfl_GroupEmployee.Properties.dictionSource.ContainsKey(text4))
			{
				string text5 = cfl_GroupEmployee.Properties.dictionSource[text4];
				AccordionControlElement accordionControlElement3 = new AccordionControlElement();
				accordionControlElement3.Text = text5;
				accordionControlElement3.Tag = num3;
				accordionControlElement3.Style = ElementStyle.Item;
				accordionControlElement3.Appearance.Normal.Font = new Font("Verdana", 8.25f);
				eleEmployee.Elements.Add(accordionControlElement3);
				listAssignedEmployee.Add(num3);
			}
		}
	}

	private async void gv_List_RowUpdated(object sender, RowObjectEventArgs e)
	{
		if (rowUpdated || FormAuth == Enums.FormAuthorities.Read)
		{
			return;
		}
		try
		{
			if (_isLoading || changeRowStatus || !(e.Row is DataRowView dataRowView))
			{
				return;
			}
			GridView view = sender as GridView;
			if (e.RowHandle != -2147483647)
			{
				return;
			}
			rowUpdated = true;
			view.OptionsBehavior.ReadOnly = true;
			if (Function.ParseInt(dataRowView.Row["ClgCode"]) == 0)
			{
				if (string.IsNullOrWhiteSpace(Function.ToString(txtOwner.EditValue)) && _owner != 0)
				{
					dataRowView.Row["Owner"] = _owner;
				}
				await AddMainTask(dataRowView.Row);
			}
			view.OptionsBehavior.ReadOnly = false;
			view.ShowEditor();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			rowUpdated = false;
		}
	}

	private void rep_bte_Closed_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			GridView view = ((sender as ButtonEdit).Parent as GridControl).MainView as GridView;
			RepositoryClick(view);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rep_bte_Open_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			GridView view = ((sender as ButtonEdit).Parent as GridControl).MainView as GridView;
			RepositoryClick(view);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void RepositoryClick(GridView view)
	{
		try
		{
			if (!readOnly && view != null && !(Function.ToString(view.GetFocusedRowCellValue("OwnerAuth")) != "F"))
			{
				ChangeTaskStatusInGrid(view);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_ListCompleted_MouseDown(object sender, MouseEventArgs e)
	{
		try
		{
			GridView gridView = sender as GridView;
			GridHitInfo gridHitInfo = gridView.CalcHitInfo(new Point(e.X, e.Y));
			if (gridHitInfo.InRowCell && gridHitInfo.Column != null)
			{
				rowHandleGridCompleted = gridView.FocusedRowHandle;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ChangeTaskStatusInGrid(GridView view)
	{
		try
		{
			if (view == null)
			{
				return;
			}
			changeRowStatus = true;
			DataRow focusedDataRow = view.GetFocusedDataRow();
			if (Function.ToString(focusedDataRow["Closed"]) == "Y")
			{
				view.SetFocusedRowCellValue("Closed", "N");
			}
			else
			{
				view.SetFocusedRowCellValue("Closed", "Y");
			}
			int rowHandle = ((view == gv_List) ? rowHandleGridList : rowHandleGridCompleted);
			object rowCellValue = view.GetRowCellValue(rowHandle, "ClgCode");
			ChangeTaskStatus(focusedDataRow);
			view.CloseEditor();
			if (view != gv_ListSub)
			{
				int num = Function.ParseInt(txtClgCode.EditValue);
				int num2 = Function.ParseInt(focusedDataRow["ClgCode"]);
				if (num2 == num)
				{
					KeepFocusedRowAfterChangeStatus(view, num2);
					chk_Closed.EditValue = focusedDataRow["Closed"];
					SetBottomLabelText(focusedDataRow);
				}
				if (num2 != num)
				{
					int focusedRowHandle = view.LocateByValue("ClgCode", rowCellValue);
					view.FocusedRowHandle = focusedRowHandle;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			changeRowStatus = false;
		}
	}

	private void ChangeTaskStatusByClickOnCheck(DataRow dr)
	{
		try
		{
			changeRowStatus = true;
			int clgCode = Function.ParseInt(focusedView.GetFocusedRowCellValue("ClgCode"));
			clickCheckChangeTask = true;
			ChangeTaskStatus(dr);
			clickCheckChangeTask = false;
			KeepFocusedRowAfterChangeStatus(focusedView, clgCode);
			SetBottomLabelText(dr);
			changeRowStatus = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			changeRowStatus = false;
		}
	}

	private void KeepFocusedRowAfterChangeStatus(GridView view, int _clgCode)
	{
		int num = Function.ParseInt(txtClgCode.EditValue);
		if (_clgCode == num)
		{
			if (view == gv_List)
			{
				int focusedRowHandle = gv_ListCompleted.LocateByValue("ClgCode", _clgCode);
				gv_ListCompleted.FocusedRowHandle = focusedRowHandle;
				gv_ListCompleted.Focus();
			}
			if (view == gv_ListCompleted)
			{
				int focusedRowHandle2 = gv_List.LocateByValue("ClgCode", _clgCode);
				gv_List.FocusedRowHandle = focusedRowHandle2;
				gv_List.Focus();
			}
		}
	}

	private void ChangeTaskStatus(DataRow dr)
	{
		if (changeTaskStatus)
		{
			return;
		}
		try
		{
			if (dr == null || readOnly || string.IsNullOrEmpty(Function.ToString(dr["Details"])))
			{
				return;
			}
			DataTable dataTable = grd_ListSub.DataSourceTable.Clone();
			if (dataTable.HasColumn() && Function.ParseInt(dr["ClgCode"]) > 0)
			{
				changeTaskStatus = true;
				if (Function.ToString(dr["Closed"]) == "Y")
				{
					dr["AttendUser"] = GlobalData.UserID;
					dr["CloseDate"] = DateTime.Now;
					dr["status"] = 2;
				}
				else
				{
					dr["status"] = 1;
				}
				DataRow dataRow = dataTable.NewRow();
				dataRow.ItemArray = dr.ItemArray;
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
				DataSet dataSet = new DataSet();
				dataSet.Tables.Add(dataTable);
				_httpClient.SubActivities.ChangeTaskStatus(dataSet);
				RefreshMainGridsAfterChangingData(dr);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			changeTaskStatus = false;
		}
	}

	private void RevertCheckValue(CheckEdit editor)
	{
		string text = Function.ToString(editor.EditValue);
		if (text == "Y")
		{
			editor.EditValue = "N";
		}
		if (text == "N")
		{
			editor.EditValue = "Y";
		}
	}

	private async Task SaveSubTaskDetails(DataRow dr)
	{
		if (saveSubTaskDetails || FormAuth == Enums.FormAuthorities.Read)
		{
			return;
		}
		try
		{
			if (dr == null || readOnly || string.IsNullOrEmpty(Function.ToString(dr["Details"])))
			{
				return;
			}
			DataTable dataTable = grd_ListSub.DataSourceTable.Clone();
			if (!dataTable.HasColumn() || Function.ParseInt(dr["ClgCode"]) <= 0)
			{
				return;
			}
			saveSubTaskDetails = true;
			DataRow dataRow = dataTable.NewRow();
			dataRow.ItemArray = dr.ItemArray;
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			DataSet dataSet = new DataSet();
			dataSet.Tables.Add(dataTable);
			HttpResult httpResult = await _httpClient.SubActivities.SaveSubTaskDetails(dataSet);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				grd_ListSub.DataSourceTable.RejectChanges();
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				return;
			}
			GridAcceptChanges(gv_ListSub);
			if (ReloadCalendar != null && !((XtraForm)ReloadCalendar.Target).IsDisposed)
			{
				await ReloadCalendar();
			}
			await WinFuntions.RefreshVisibleCalendars();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			grd_ListSub.DataSourceTable.RejectChanges();
		}
		finally
		{
			saveSubTaskDetails = false;
		}
	}

	private async Task SaveSubTaskAssigner(DataRow dr)
	{
		if (saveSubTaskAssigner || FormAuth == Enums.FormAuthorities.Read)
		{
			return;
		}
		try
		{
			if (dr == null || readOnly || string.IsNullOrEmpty(Function.ToString(dr["Details"])))
			{
				return;
			}
			DataTable dataTable = grd_ListSub.DataSourceTable.Clone();
			if (dataTable.HasColumn() && Function.ParseInt(dr["ClgCode"]) > 0)
			{
				saveSubTaskAssigner = true;
				DataRow dataRow = dataTable.NewRow();
				dataRow.ItemArray = dr.ItemArray;
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
				DataSet dataSet = new DataSet();
				dataSet.Tables.Add(dataTable);
				HttpResult httpResult = await _httpClient.SubActivities.SaveSubTaskAssigner(dataSet);
				if (!string.IsNullOrEmpty(httpResult.Message))
				{
					grd_ListSub.DataSourceTable.RejectChanges();
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				}
				else
				{
					WinCommons.Success(Enums.MessageCode.Assigned, Msg_Time._TimeTiny, "");
					GridAcceptChanges(gv_ListSub);
					await WinFuntions.RefreshVisibleCalendars();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			grd_ListSub.DataSourceTable.RejectChanges();
		}
		finally
		{
			saveSubTaskAssigner = false;
		}
	}

	private async Task SaveTaskPosition(DataRow dr)
	{
		if (saveTaskPosition || FormAuth != Enums.FormAuthorities.Read)
		{
			return;
		}
		try
		{
			if (dr == null || readOnly || string.IsNullOrEmpty(Function.ToString(dr["Details"])))
			{
				return;
			}
			DataTable dataTable = grd_List.DataSourceTable.Clone();
			if (dataTable.HasColumn() && Function.ParseInt(dr["ClgCode"]) > 0)
			{
				saveTaskPosition = true;
				DataRow dataRow = dataTable.NewRow();
				dataRow.ItemArray = dr.ItemArray;
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
				DataSet dataSet = new DataSet();
				dataSet.Tables.Add(dataTable);
				HttpResult httpResult = await _httpClient.SubActivities.SaveTaskPosition(dataSet);
				if (!string.IsNullOrEmpty(httpResult.Message))
				{
					grd_List.DataSourceTable.RejectChanges();
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				}
				else
				{
					GridAcceptChanges(gv_List);
					await WinFuntions.RefreshVisibleCalendars();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			grd_List.DataSourceTable.RejectChanges();
		}
		finally
		{
			saveTaskPosition = false;
		}
	}

	private async Task SaveTaskManagementSetting()
	{
		try
		{
			if (CallFromCal)
			{
				return;
			}
			string value = Function.ToString(luk_Sort.EditValue);
			string value2 = "";
			AccordionControlElement selectedElement = accordionControl1.SelectedElement;
			if (selectedElement != null)
			{
				AccordionControlElement ownerElement = selectedElement.OwnerElement;
				string text = Function.ToString(selectedElement.Tag);
				if (ownerElement == eleGeneral)
				{
					value2 = text;
				}
				if (ownerElement == eleUser)
				{
					value2 = "U_" + text;
				}
				if (ownerElement == eleEmployee)
				{
					value2 = "E_" + text;
				}
				if (ownerElement == eleProjectNew)
				{
					value2 = "P_" + text;
				}
			}
			DataTable dataTable = dtAPZ_CLG2.Clone();
			DataRow dataRow = dataTable.NewRow();
			dataRow["UserSign"] = GlobalData.UserID;
			dataRow["sortBy"] = value;
			dataRow["filterBy"] = value2;
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			DataSet dataSet = new DataSet();
			dataSet.Tables.Add(dataTable);
			await _httpClient.SubActivities.SaveTaskManagementSetting(dataSet);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void frm_TaskManagement_FormClosed(object sender, FormClosedEventArgs e)
	{
		SaveTaskManagementSetting();
	}

	private async void AutoUploadAfterBrowse()
	{
		try
		{
			if (!CallFromCal && Function.ParseInt(txtClgCode.EditValue) != 0 && ucAttachment1.GetAttachmentNoSAP(out var dt))
			{
				DataRow dr = dtRow.Rows[0];
				dt.TableName = "Attachment";
				await SaveCurrentTask(dr, SaveType.Attachment, dt);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void gv_Udf_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		if (udfCellValueChanged)
		{
			return;
		}
		try
		{
			if (!isFill && !_isLoading)
			{
				if (FormMode == FormModeType.SingleView || FormMode == FormModeType.SingleUpdate)
				{
					CheckSaveState();
					return;
				}
				udfCellValueChanged = true;
				DataRow dr = dtRow.Rows[0];
				await SaveCurrentTask(dr, SaveType.UDF);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			udfCellValueChanged = false;
		}
	}

	private async Task SaveCurrentTask(DataRow dr, SaveType saveType = SaveType.ValidateControl, DataTable dtAtt = null)
	{
		if (saveTask || FormAuth == Enums.FormAuthorities.Read)
		{
			return;
		}
		try
		{
			if (dr == null || readOnly || Function.ParseInt(dr["ClgCode"]) <= 0)
			{
				return;
			}
			if (string.IsNullOrEmpty(Function.ToString(dr["Details"])))
			{
				grd_List.DataSourceTable.RejectChanges();
				return;
			}
			DataTable dt = grd_List.DataSourceTable.Clone();
			if (!dt.HasColumn())
			{
				return;
			}
			saveTask = true;
			SetAutoValueForTask(dr);
			if (saveType == SaveType.ValidateControl || saveType == SaveType.SingleUpdate)
			{
				dr["DocNum"] = ucLinkedDocument1.EditValue;
				dr["DocType"] = ucLinkedDocument1.DocType;
				dr["DocEntry"] = ucLinkedDocument1.DocEntry;
				bte_DistrRule.AddValueToRow(dr);
			}
			if (saveType != SaveType.UDF)
			{
				_ = saveType;
				_ = 6;
			}
			DataRow dataRow = dt.NewRow();
			dataRow.ItemArray = dr.ItemArray;
			dt.Rows.Add(dataRow);
			dataRow["UserSign"] = GlobalData.UserID;
			dataRow["UpdateDate"] = DateTime.Today;
			dataRow["UpdateTime"] = Function.ParseTimeSpanToInt(DateTime.Now.TimeOfDay);
			dt.AcceptChanges();
			DataSet dataSet = new DataSet();
			dataSet.Tables.Add(dt);
			if (dtAtt != null && (saveType == SaveType.Attachment || saveType == SaveType.SingleUpdate))
			{
				dataSet.Tables.Add(dtAtt);
			}
			HttpResult httpResult = await _httpClient.SubActivities.SaveTask(dataSet);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				grd_List.DataSourceTable.RejectChanges();
				saveTask = false;
				return;
			}
			string[] array = Function.ToString(httpResult.Content).Split(',');
			if (array.Length > 1)
			{
				int num = Function.ParseInt(array[1]);
				if (dtAtt != null && num != 0)
				{
					dr["AtcEntry"] = num;
					dr.AcceptChanges();
					foreach (DataRow item in ucAttachment1.grd_File.DataSourceTable.AsEnumerable())
					{
						item["AbsEntry"] = num;
					}
					ucAttachment1.grd_File.DataSourceTable.AcceptChanges();
				}
			}
			if (ReloadCalendar != null && !((XtraForm)ReloadCalendar.Target).IsDisposed)
			{
				await ReloadCalendar();
			}
			if (ReloadRelated != null && !((XtraForm)ReloadRelated.Target).IsDisposed)
			{
				await ReloadRelated(dt.Copy().Rows[0]);
			}
			if (FormMode == FormModeType.Normal)
			{
				RefreshMainGridsAfterChangingData(dr);
			}
			else
			{
				WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
			}
			await WinFuntions.RefreshVisibleCalendars();
			if (saveType == SaveType.SingleUpdate)
			{
				FormMode = FormModeType.SingleView;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			grd_List.DataSourceTable.RejectChanges();
			saveTask = false;
		}
		finally
		{
			saveTask = false;
		}
	}

	public async Task AddMainTask(DataRow dr, SaveType saveType = SaveType.GridAdd, DataTable dtAtt = null)
	{
		if (FormAuth == Enums.FormAuthorities.Read || addMainTask)
		{
			return;
		}
		try
		{
			if (dr == null || readOnly || Function.ParseInt(dr["ClgCode"]) > 0)
			{
				return;
			}
			if (string.IsNullOrEmpty(Function.ToString(dr["Details"])))
			{
				grd_List.DataSourceTable.RejectChanges();
				return;
			}
			DataTable dt = grd_List.DataSourceTable.Clone();
			if (!dt.HasColumn())
			{
				return;
			}
			dr["OwnerAuth"] = "F";
			addMainTask = true;
			SetDefaultValueForNewTask(dr, saveType);
			if (saveType == SaveType.GridAdd)
			{
				if (TaskFilter == TaskFilterType.ProjectNew)
				{
					object tag = accordionControl1.SelectedElement.Tag;
					dr["ProjectEntry"] = tag;
				}
				if (TaskFilter == TaskFilterType.Users)
				{
					object tag2 = accordionControl1.SelectedElement.Tag;
					dr["GroupUser"] = tag2;
				}
				if (TaskFilter == TaskFilterType.Employees)
				{
					object tag3 = accordionControl1.SelectedElement.Tag;
					dr["GroupEmployee"] = tag3;
					dr["GroupUser"] = null;
				}
				if (TaskFilter == TaskFilterType.Unassigned)
				{
					dr["GroupUser"] = null;
					dr["GroupEmployee"] = null;
				}
				foreach (Control control in layout_UDF.Controls)
				{
					if (control is BaseEdit && dr.Table.Columns.Contains(control.Name) && !string.IsNullOrEmpty(control.AccessibleDescription))
					{
						SetDefaultValue(dr, control);
					}
				}
			}
			SetAutoValueForTask(dr);
			if (saveType == SaveType.SingleAdd)
			{
				dr["DocNum"] = ucLinkedDocument1.EditValue;
				dr["DocType"] = ucLinkedDocument1.DocType;
				dr["DocEntry"] = ucLinkedDocument1.DocEntry;
				bte_DistrRule.AddValueToRow(dr);
			}
			DataRow row = dt.NewRow();
			row.ItemArray = dr.ItemArray;
			row["CntctDate"] = DateTime.Today;
			row["CntctTime"] = Function.ParseTimeSpanToInt(DateTime.Now.TimeOfDay);
			row["UpdateDate"] = DateTime.Today;
			row["UpdateTime"] = Function.ParseTimeSpanToInt(DateTime.Now.TimeOfDay);
			dt.Rows.Add(row);
			dt.AcceptChanges();
			DataSet dataSet = new DataSet();
			dataSet.Tables.Add(dt);
			if (dtAtt != null && (saveType == SaveType.SingleAdd || saveType == SaveType.Copy))
			{
				dataSet.Tables.Add(dtAtt);
			}
			HttpResult httpResult = await _httpClient.SubActivities.AddTask(dataSet);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				grd_List.DataSourceTable.RejectChanges();
				saveTask = false;
				return;
			}
			string[] array = Function.ToString(httpResult.Content).Split(',');
			int num = ((array.Length <= 1) ? Function.ParseInt(httpResult.Content) : Function.ParseInt(array[0]));
			dr["ClgCode"] = num;
			row["ClgCode"] = num;
			if (FormMode == FormModeType.SingleAdd)
			{
				ClgCode = num;
				if (ReloadCalendar != null && !((XtraForm)ReloadCalendar.Target).IsDisposed)
				{
					await ReloadCalendar();
				}
				if (ReloadRelated != null && !((XtraForm)ReloadRelated.Target).IsDisposed)
				{
					await ReloadRelated(dt.Copy().Rows[0]);
				}
			}
			if (FormMode == FormModeType.Normal)
			{
				if ((saveType == SaveType.GridAdd || saveType == SaveType.Copy) && Function.ToString(dr["parentTask"]) == "")
				{
					DataRow dataRow = grd_ListCompleted.DataSourceTable.NewRow();
					dataRow.ItemArray = dr.ItemArray;
					grd_ListCompleted.DataSourceTable.Rows.InsertAt(dataRow, 0);
				}
				RefreshMainGridsAfterChangingData(dr);
			}
			else
			{
				WinCommons.Success(Enums.MessageCode.AddedSuccessfully, "");
			}
			await WinFuntions.RefreshVisibleCalendars();
			if (saveType == SaveType.SingleAdd)
			{
				FormMode = FormModeType.SingleView;
				Close();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			grd_List.DataSourceTable.RejectChanges();
			saveTask = false;
		}
		finally
		{
			modifyInAddMode = false;
			addMainTask = false;
		}
	}

	private static void SetDefaultValue(DataRow dr, Control editor)
	{
		Type dataType = dr.Table.Columns[editor.Name].DataType;
		if (dataType == typeof(DateTime))
		{
			DateTime? dateTime = Function.ParseDateTime(editor.AccessibleDescription);
			if (dateTime.HasValue)
			{
				dr[editor.Name] = dateTime.Value;
			}
		}
		else if (dataType == typeof(int) || dataType == typeof(short) || dataType == typeof(long))
		{
			dr[editor.Name] = Function.ParseInt(editor.AccessibleDescription);
		}
		else if (dataType == typeof(float) || dataType == typeof(double) || dataType == typeof(decimal))
		{
			dr[editor.Name] = Function.ParseDouble(editor.AccessibleDescription);
		}
		else
		{
			dr[editor.Name] = editor.AccessibleDescription;
		}
	}

	private async Task AddSubTask(DataRow dr)
	{
		if (addSubTask || FormAuth == Enums.FormAuthorities.Read)
		{
			return;
		}
		try
		{
			if (dr == null || readOnly)
			{
				return;
			}
			if (string.IsNullOrEmpty(Function.ToString(dr["Details"])))
			{
				grd_ListSub.DataSourceTable.RejectChanges();
				return;
			}
			int num = Function.ParseInt(txtClgCode.EditValue);
			if (num == 0)
			{
				return;
			}
			DataTable dataTable = grd_ListSub.DataSourceTable.Clone();
			if (!dataTable.HasColumn() || saveTask)
			{
				return;
			}
			addSubTask = true;
			if (Function.ParseInt(dr["ClgCode"]) > 0)
			{
				return;
			}
			SetDefaultValueForNewTask(dr, SaveType.SubAdd);
			dr["parentTask"] = num;
			foreach (Control control in layout_UDF.Controls)
			{
				if (control is BaseEdit && dr.Table.Columns.Contains(control.Name))
				{
					SetDefaultValue(dr, control);
				}
			}
			SetAutoValueForTask(dr);
			DataRow dataRow = dataTable.NewRow();
			dataRow.ItemArray = dr.ItemArray;
			dataRow["CntctDate"] = DateTime.Today;
			dataRow["CntctTime"] = Function.ParseTimeSpanToInt(DateTime.Now.TimeOfDay);
			dataRow["UpdateDate"] = DateTime.Today;
			dataRow["UpdateTime"] = Function.ParseTimeSpanToInt(DateTime.Now.TimeOfDay);
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			DataSet dataSet = new DataSet();
			dataSet.Tables.Add(dataTable);
			HttpResult httpResult = await _httpClient.SubActivities.AddTask(dataSet);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				grd_ListSub.DataSourceTable.RejectChanges();
				return;
			}
			string[] array = Function.ToString(httpResult.Content).Split(',');
			int num2 = ((array.Length <= 1) ? Function.ParseInt(httpResult.Content) : Function.ParseInt(array[0]));
			dr["ClgCode"] = num2;
			dr.AcceptChanges();
			if (FormMode == FormModeType.SingleAdd)
			{
				ClgCode = num2;
			}
			if (FormMode == FormModeType.Normal)
			{
				foreach (DataRow item in grd_ListSub.DataSourceTable.AsEnumerable())
				{
					if (item["ClgCode"].Equals(dr["ClgCode"]))
					{
						item.ItemArray = dr.ItemArray;
					}
				}
				GridAcceptChanges(gv_ListSub);
			}
			if (ReloadCalendar != null && !((XtraForm)ReloadCalendar.Target).IsDisposed)
			{
				await ReloadCalendar();
			}
			await WinFuntions.RefreshVisibleCalendars();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			grd_ListSub.DataSourceTable.RejectChanges();
		}
		finally
		{
			addSubTask = false;
		}
	}

	private void RefreshMainGridsAfterChangingData(DataRow dr)
	{
		foreach (DataRow item in grd_List.DataSourceTable.AsEnumerable())
		{
			if (item["ClgCode"].Equals(dr["ClgCode"]))
			{
				item.ItemArray = dr.ItemArray;
				break;
			}
		}
		GridListAcceptChanges();
		foreach (DataRow item2 in grd_ListCompleted.DataSourceTable.AsEnumerable())
		{
			if (item2["ClgCode"].Equals(dr["ClgCode"]))
			{
				item2.ItemArray = dr.ItemArray;
				break;
			}
		}
		GridListCompletedAcceptChanges();
	}

	public static void SetAutoValueForTask(DataRow dr)
	{
		if (Function.ToString(dr["Closed"]) == "Y")
		{
			dr["CloseDate"] = DateTime.Now;
			dr["status"] = 2;
		}
		else
		{
			dr["status"] = 1;
		}
		string value = ResMapping(dr);
		dr["ResourceIdsInfo"] = value;
	}

	public static string ResMapping(DataRow dr)
	{
		ResourceIdCollection resourceIdCollection = new ResourceIdCollection();
		SchedulerCompatibility.Base64XmlObjectSerialization = false;
		string text = Function.ToString(dr["GroupUser"]);
		string text2 = Function.ToString(dr["GroupEmployee"]);
		if (text != "")
		{
			string text3 = text;
			if (!string.IsNullOrWhiteSpace(text3))
			{
				string[] array = Function.ToString(text3).Split(';');
				foreach (string text4 in array)
				{
					if (!string.IsNullOrWhiteSpace(text4))
					{
						resourceIdCollection.Add("U" + Function.ToString(text4));
					}
				}
			}
		}
		if (text == "")
		{
			string text3 = text2;
			if (!string.IsNullOrWhiteSpace(text3))
			{
				string[] array = Function.ToString(text3).Split(';');
				foreach (string text5 in array)
				{
					if (!string.IsNullOrWhiteSpace(text5))
					{
						resourceIdCollection.Add("E" + Function.ToString(text5));
					}
				}
			}
		}
		return new AppointmentResourceIdCollectionXmlPersistenceHelper(resourceIdCollection).ToXml();
	}

	public void SetDefaultValueForNewTask(DataRow dr, SaveType saveType)
	{
		if (Function.ToString(dr["GroupUser"]) == "" && Function.ToString(dr["GroupEmployee"]) == "")
		{
			dr["GroupUser"] = GlobalData.UserID;
		}
		if (Function.ToString(dr["Recontact"]) == "")
		{
			dr["Recontact"] = DateTime.Today;
			dr["BeginTime"] = Function.ParseTimeSpanToInt(DateTime.Now.TimeOfDay);
		}
		dr["Action"] = "T";
		dr["CntctDate"] = DateTime.Today;
		dr["AssignedBy"] = GlobalData.UserID;
		dr["UserSign"] = GlobalData.UserID;
		dr["Closed"] = "N";
		if (saveType == SaveType.GridAdd || saveType == SaveType.SubAdd)
		{
			if (Function.ToString(dr["Priority"]) == "")
			{
				dr["Priority"] = "1";
			}
			if (Function.ToString(dr["DocType"]) == "")
			{
				dr["DocType"] = -1;
			}
		}
	}

	private void GridListAcceptChanges()
	{
		try
		{
			acceptChange = true;
			int focusedRowHandle = gv_List.FocusedRowHandle;
			grd_List.DataSourceTable.AcceptChanges();
			gv_List.FocusedRowHandle = focusedRowHandle;
			acceptChange = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			acceptChange = false;
		}
	}

	private void GridAcceptChanges(GridView view)
	{
		try
		{
			acceptChange = true;
			int focusedRowHandle = view.FocusedRowHandle;
			(view.GridControl.DataSource as DataTable).AcceptChanges();
			view.FocusedRowHandle = focusedRowHandle;
			acceptChange = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			acceptChange = false;
		}
	}

	private void GridListCompletedAcceptChanges()
	{
		try
		{
			acceptChange = true;
			int focusedRowHandle = gv_ListCompleted.FocusedRowHandle;
			grd_ListCompleted.DataSourceTable.AcceptChanges();
			gv_ListCompleted.FocusedRowHandle = focusedRowHandle;
			acceptChange = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			acceptChange = false;
		}
	}

	private async void btnOK_Click(object sender, EventArgs e)
	{
		_ = 2;
		try
		{
			btnOK.Enabled = false;
			WinCommons._OpenProcessing();
			switch (FormMode)
			{
			case FormModeType.Normal:
			case FormModeType.SingleView:
				Close();
				break;
			case FormModeType.SingleUpdate:
			{
				if (dtRow.IsNotNull() && ucAttachment1.GetAttachmentNoSAP(out var dt2))
				{
					DataRow dr2 = dtRow.Rows[0];
					dt2.TableName = "Attachment";
					await SaveCurrentTask(dr2, SaveType.SingleUpdate, dt2);
				}
				break;
			}
			case FormModeType.SingleAdd:
			{
				DataTable dt;
				if (Function.ToString(txtDetails.Text) == "")
				{
					WinCommons.Error(Enums.MessageCode.TaskNameIsRequired, "");
					txtDetails.Focus();
				}
				else if (dtRow.IsNotNull() && ucAttachment1.GetAttachmentNoSAP(out dt))
				{
					DataRow dr = dtRow.Rows[0];
					dt.TableName = "Attachment";
					await AddMainTask(dr, SaveType.SingleAdd, dt);
					SetBottomLabelText(dr);
					await FillInfo(dr);
				}
				break;
			}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			btnOK.Enabled = true;
			WinCommons._CloseProcessing();
		}
	}

	private async void btnDelete_Click(object sender, EventArgs e)
	{
	}

	private async void removeToolStripMenuItem1_Click(object sender, EventArgs e)
	{
		if (deleteTask)
		{
			return;
		}
		try
		{
			deleteTask = true;
			DataRow dr;
			if (!CallFromCal)
			{
				if (focusedView == null || focusedView.DataRowCount == 0)
				{
					return;
				}
				GridView view = focusedView;
				if (!dtRow.IsNotNull())
				{
					return;
				}
				dr = dtRow.Rows[0];
				if (Function.ParseInt(dr["ClgCode"]) == 0)
				{
					return;
				}
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
				{
					await RemoveCurrentTask(dr);
					int num = Function.ParseInt(dr["parentTask"]);
					bool flag = num != 0;
					if (!flag)
					{
						view.DeleteRow(focusedView.FocusedRowHandle);
						GridControl gridControl = view.GridControl;
						DataTable dataTable = gridControl.DataSource as DataTable;
						dataTable.AcceptChanges();
						if (gridControl == grd_List)
						{
							grd_ListCompleted.DataSource = dataTable.Copy();
							GridAcceptChanges(gv_ListCompleted);
						}
						if (gridControl == grd_ListCompleted)
						{
							grd_List.DataSource = dataTable.Copy();
							GridAcceptChanges(gv_List);
						}
					}
					if (flag)
					{
						await FillInfo(num);
					}
				}
			}
			if (!CallFromCal || !dtRow.IsNotNull())
			{
				return;
			}
			dr = dtRow.Rows[0];
			if (Function.ParseInt(dr["ClgCode"]) != 0 && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				await RemoveCurrentTask(dr);
				if (ReloadCalendar != null && !((XtraForm)ReloadCalendar.Target).IsDisposed)
				{
					await ReloadCalendar();
				}
				int num2 = Function.ParseInt(dr["parentTask"]);
				bool flag2 = num2 != 0;
				if (!flag2)
				{
					Close();
				}
				if (flag2)
				{
					await FillInfo(num2);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			deleteTask = false;
		}
	}

	private async void removeToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (deleteTask)
		{
			return;
		}
		try
		{
			GridControl gridControl = ((sender as ToolStripMenuItem).Owner as ContextMenuStrip).SourceControl as GridControl;
			if (!(gridControl.MainView is GridView view))
			{
				return;
			}
			deleteTask = true;
			if (view.DataRowCount != 0 && view.GetFocusedRow() is DataRowView { Row: not null } dataRowView && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				Task task = RemoveCurrentTask(dataRowView);
				view.DeleteRow(view.FocusedRowHandle);
				await task;
				GridControl gridControl2 = view.GridControl;
				DataTable dataTable = gridControl2.DataSource as DataTable;
				dataTable.AcceptChanges();
				if (gridControl2 == grd_List)
				{
					grd_ListCompleted.DataSource = dataTable.Copy();
					GridAcceptChanges(gv_ListCompleted);
				}
				else
				{
					grd_List.DataSource = dataTable.Copy();
					GridAcceptChanges(gv_List);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			deleteTask = false;
		}
	}

	private async void gv_List_KeyDown(object sender, KeyEventArgs e)
	{
		if (processKey)
		{
			return;
		}
		try
		{
			if (!(sender is GridView view) || e.KeyCode != Keys.Delete || view.DataRowCount == 0 || !(view.GetFocusedRow() is DataRowView { Row: not null } dataRowView))
			{
				return;
			}
			processKey = true;
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				Task task = RemoveCurrentTask(dataRowView);
				view.DeleteRow(view.FocusedRowHandle);
				await task;
				GridControl gridControl = view.GridControl;
				DataTable dataTable = gridControl.DataSource as DataTable;
				dataTable.AcceptChanges();
				if (gridControl == grd_List)
				{
					grd_ListCompleted.DataSource = dataTable.Copy();
					GridAcceptChanges(gv_ListCompleted);
				}
				if (gridControl == grd_ListCompleted)
				{
					grd_List.DataSource = dataTable.Copy();
					GridAcceptChanges(gv_List);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			processKey = false;
		}
	}

	private async Task RemoveCurrentTask(DataRow dr)
	{
		_ = 3;
		try
		{
			if (dr == null)
			{
				return;
			}
			DataTable dataTable = (focusedView.GridControl.DataSource as DataTable).Clone();
			if (!dataTable.HasColumn())
			{
				return;
			}
			DataRow dataRow = dataTable.NewRow();
			dataRow.ItemArray = dr.ItemArray;
			int clgCode = Function.ParseInt(dataRow["ClgCode"]);
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			string text = await _httpClient.SubActivities.RemoveTask(dataTable);
			if (!string.IsNullOrEmpty(text))
			{
				WinCommons.Error(Enums.MessageCode.None, text);
				(focusedView.GridControl.DataSource as DataTable).RejectChanges();
				return;
			}
			GridAcceptChanges(focusedView);
			dtAPZ_OCLG = (focusedView.GridControl.DataSource as DataTable).Copy();
			await WinFuntions.RefreshVisibleCalendars();
			if (ReloadCalendar != null && !((XtraForm)ReloadCalendar.Target).IsDisposed)
			{
				await ReloadCalendar();
			}
			if (ReloadAfterRemove != null && !((XtraForm)ReloadAfterRemove.Target).IsDisposed)
			{
				await ReloadAfterRemove(clgCode);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			(focusedView.GridControl.DataSource as DataTable).RejectChanges();
		}
	}

	private async Task RemoveCurrentTask(DataRowView dr)
	{
		if (dr != null && dr.Row != null)
		{
			await RemoveCurrentTask(dr.Row);
		}
	}

	private void gridView1_MouseDown(object sender, MouseEventArgs e)
	{
		try
		{
			GridView gridView = sender as GridView;
			GridHitInfo gridHitInfo = gridView.CalcHitInfo(new Point(e.X, e.Y));
			if (gridHitInfo.InRowCell && gridHitInfo.Column != null)
			{
				rowHandleGridList = gridView.FocusedRowHandle;
			}
			downHitInfo = null;
			if (Control.ModifierKeys == Keys.None && e.Button == MouseButtons.Left && gridHitInfo.InRow && gridHitInfo.RowHandle != -2147483647)
			{
				downHitInfo = gridHitInfo;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gridView1_MouseMove(object sender, MouseEventArgs e)
	{
		try
		{
			GridView gridView = sender as GridView;
			if (e.Button == MouseButtons.Left && downHitInfo != null)
			{
				Size dragSize = SystemInformation.DragSize;
				Rectangle rectangle = new Rectangle(new Point(downHitInfo.HitPoint.X - dragSize.Width / 2, downHitInfo.HitPoint.Y - dragSize.Height / 2), dragSize);
				if (!rectangle.Contains(new Point(e.X, e.Y)))
				{
					gridView.GridControl.DoDragDrop(downHitInfo, DragDropEffects.All);
					downHitInfo = null;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task MoveRow(int sourceRowHandle, int targetRowHandle)
	{
		if (reorderRow)
		{
			return;
		}
		try
		{
			if (sourceRowHandle == targetRowHandle || sourceRowHandle == targetRowHandle + 1 || (Function.ToString(luk_Sort.EditValue) != "(None)" && Function.ToString(luk_Sort.EditValue) != ""))
			{
				return;
			}
			reorderRow = true;
			GridView gridView = WinFuntions.GetActiveGridControl(ActiveControl).MainView as GridView;
			DataRow dataRow = gridView.GetDataRow(targetRowHandle);
			if (dataRow != null)
			{
				DataRow dataRow2 = gridView.GetDataRow(targetRowHandle + 1);
				DataRow dataRow3 = gridView.GetDataRow(sourceRowHandle);
				decimal num = Function.ParseDecimal(dataRow["OrderLine"]);
				if (dataRow2 != null)
				{
					decimal num2 = Function.ParseDecimal(dataRow2["OrderLine"]);
					dataRow3["OrderLine"] = (num + num2) / 2m;
				}
				if (dataRow2 == null)
				{
					dataRow3["OrderLine"] = num + 1m;
				}
				await SaveTaskPosition(dataRow3);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			reorderRow = false;
		}
	}

	private void grd_List_DragOver(object sender, DragEventArgs e)
	{
		try
		{
			if (e.Data.GetDataPresent(typeof(GridHitInfo)) && e.Data.GetData(typeof(GridHitInfo)) is GridHitInfo gridHitInfo)
			{
				GridControl gridControl = sender as GridControl;
				GridHitInfo gridHitInfo2 = (gridControl.MainView as GridView).CalcHitInfo(gridControl.PointToClient(new Point(e.X, e.Y)));
				if (gridHitInfo2.InRow && gridHitInfo2.RowHandle != gridHitInfo.RowHandle && gridHitInfo2.RowHandle != -2147483647)
				{
					e.Effect = DragDropEffects.Move;
				}
				else
				{
					e.Effect = DragDropEffects.None;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void grd_List_DragDrop(object sender, DragEventArgs e)
	{
		try
		{
			GridControl gridControl = sender as GridControl;
			GridView obj = gridControl.MainView as GridView;
			GridHitInfo gridHitInfo = e.Data.GetData(typeof(GridHitInfo)) as GridHitInfo;
			GridHitInfo gridHitInfo2 = obj.CalcHitInfo(gridControl.PointToClient(new Point(e.X, e.Y)));
			int rowHandle = gridHitInfo.RowHandle;
			int rowHandle2 = gridHitInfo2.RowHandle;
			MoveRow(rowHandle, rowHandle2);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void splitContainerControl1_SplitGroupPanelCollapsing(object sender, SplitGroupPanelCollapsingEventArgs e)
	{
		if (CallFromCal)
		{
			e.Cancel = true;
		}
	}

	private void frm_TaskManagement_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if (btnOK.Text == Apzon.Commons.Properties.Resources.ButtonUpdate || (btnOK.Text == Apzon.Commons.Properties.Resources.ButtonAdd && modifyInAddMode))
			{
				switch (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSaveBeforeClose, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1))
				{
				case DialogResult.Cancel:
					e.Cancel = true;
					break;
				case DialogResult.Yes:
					btnOK_Click(null, null);
					break;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, Keys keyData)
	{
		switch (keyData)
		{
		case Keys.D1 | Keys.Control:
			btnDocument_Click(null, null);
			return true;
		case Keys.D2 | Keys.Control:
			btnSubTask_Click(null, null);
			return true;
		case Keys.D3 | Keys.Control:
			btnAttachment_Click(null, null);
			return true;
		case Keys.D4 | Keys.Control:
			btnUDF_Click(null, null);
			return true;
		default:
			return base.ProcessCmdKey(ref msg, keyData);
		}
	}

	private void btnSubTask_Click(object sender, EventArgs e)
	{
		if (splitSubTask.Collapsed)
		{
			splitSubTask.Collapsed = false;
			openSubTaskSplit = true;
		}
		else
		{
			splitSubTask.Collapsed = true;
			openSubTaskSplit = false;
		}
	}

	private void ucAttachment1_ButtonBrowse_Click(object sender, EventArgs e)
	{
		if (FormMode == FormModeType.SingleView)
		{
			FormMode = FormModeType.SingleUpdate;
		}
	}

	private void btnAttachment_Click(object sender, EventArgs e)
	{
		if (splitAttachment.Collapsed)
		{
			splitAttachment.Collapsed = false;
			openAttachmentSplit = true;
		}
		else
		{
			splitAttachment.Collapsed = true;
			openAttachmentSplit = false;
		}
	}

	private void btnDocument_Click(object sender, EventArgs e)
	{
		if (splitMoreInformation.Collapsed)
		{
			splitMoreInformation.Collapsed = false;
			openInformationSplit = true;
		}
		else
		{
			splitMoreInformation.Collapsed = true;
			openInformationSplit = false;
		}
	}

	private void btnUDF_Click(object sender, EventArgs e)
	{
		if (splitUDF.Collapsed)
		{
			splitUDF.Collapsed = false;
			openUDFSplit = true;
		}
		else
		{
			splitUDF.Collapsed = true;
			openUDFSplit = false;
		}
	}

	private void gv_List_GotFocus(object sender, EventArgs e)
	{
		try
		{
			_ = gv_List;
			focusedView = gv_List;
			gv_ListCompleted.RefreshRow(gv_ListCompleted.FocusedRowHandle);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_ListCompleted_GotFocus(object sender, EventArgs e)
	{
		try
		{
			focusedView = gv_ListCompleted;
			gv_List.RefreshRow(gv_List.FocusedRowHandle);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void toolTipController1_BeforeShow(object sender, ToolTipControllerShowEventArgs e)
	{
		try
		{
			string text = Function.ToString((e.SelectedControl as CheckEdit).EditValue);
			if (!(text == "Y"))
			{
				if (text == "N")
				{
					e.ToolTip = "Not Completed";
				}
			}
			else
			{
				e.ToolTip = "Completed";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private bool StayToSave()
	{
		if (FormMode == FormModeType.SingleUpdate || (FormMode == FormModeType.SingleAdd && modifyInAddMode))
		{
			switch (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1))
			{
			case DialogResult.Yes:
				return false;
			case DialogResult.No:
				return true;
			case DialogResult.Cancel:
				return true;
			}
		}
		return false;
	}

	private void ccb_GroupUser_KeyPress(object sender, KeyPressEventArgs e)
	{
		try
		{
			CheckedComboBoxEdit checkedComboBoxEdit = sender as CheckedComboBoxEdit;
			if (!checkedComboBoxEdit.IsPopupOpen)
			{
				checkedComboBoxEdit.ShowPopup();
				SendKeys.Send(e.KeyChar.ToString());
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void copyTaskIDToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (!dtRow.EmptyData())
			{
				string text = Function.ToString(dtRow.Rows[0]["ClgCode"]);
				if (!(text == "0"))
				{
					Clipboard.SetText(text);
					WinCommons.Success(Enums.MessageCode.Copied, Msg_Time._TimeTiny, "");
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void cbUser_SelectedValueChanged(object sender, EventArgs e)
	{
		try
		{
			string text = Function.ToString(cbUser.EditValue);
			if (!(text == "User"))
			{
				if (text == "Employee")
				{
					cfl_GroupUser.Visible = false;
					cfl_GroupEmployee.Visible = true;
					if (!changingRowObject && !changingRowFocus)
					{
						cfl_GroupUser.EditValue = null;
					}
				}
			}
			else
			{
				cfl_GroupUser.Visible = true;
				cfl_GroupEmployee.Visible = false;
				if (!changingRowObject && !changingRowFocus)
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

	private void cfl_BPCode_EditValueChanged(object sender, EventArgs e)
	{
		if (isFill || isModified)
		{
			RefreshCardTypeAndCardName();
		}
	}

	private void cfl_BPCode_Validated(object sender, EventArgs e)
	{
		if (isModified)
		{
			RefreshCardTypeAndCardName();
		}
	}

	private void RefreshCardTypeAndCardName()
	{
		try
		{
			string text = Function.ToString(cfl_BPCode.EditValue);
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

	private void txtDetails_TextChanged(object sender, EventArgs e)
	{
		txtDetails_Resize(sender, e);
	}

	private void txtDetails_Resize(object sender, EventArgs e)
	{
		TextBoxDetailsAutoHeight(sender);
	}

	private void memoNotes_TextChanged(object sender, EventArgs e)
	{
		TextBoxNotesAutoHeight(sender);
	}

	private void memoNotes_Resize(object sender, EventArgs e)
	{
		TextBoxNotesAutoHeight(sender);
	}

	private void TextBoxDetailsAutoHeight(object sender)
	{
		if (txtDetailsAutoHeight)
		{
			return;
		}
		try
		{
			TextBox textBox = sender as TextBox;
			int num = textBox.Width;
			if (num <= textBox.MinimumSize.Width)
			{
				num = 287;
			}
			using Graphics graphics = textBox.CreateGraphics();
			int num2 = (int)graphics.MeasureString(textBox.Text, textBox.Font, num).Height + 4;
			if (num2 < textBox.MinimumSize.Height)
			{
				textBox.Height = textBox.MinimumSize.Height;
				textBox.ScrollBars = ScrollBars.None;
			}
			if (num2 >= textBox.MinimumSize.Height && num2 < textBox.MaximumSize.Height)
			{
				textBox.Height = num2;
				textBox.ScrollBars = ScrollBars.None;
			}
			if (num2 >= textBox.MaximumSize.Height)
			{
				textBox.Height = textBox.MaximumSize.Height;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void TextBoxNotesAutoHeight(object sender)
	{
		if (memoNotesAutoHeight)
		{
			return;
		}
		try
		{
			memoNotesAutoHeight = true;
			TextBox textBox = sender as TextBox;
			int num = textBox.Width;
			if (num <= textBox.MinimumSize.Width)
			{
				num = 341;
			}
			int num2 = (int)textBox.CreateGraphics().MeasureString(textBox.Text, textBox.Font, num).Height + 4;
			if (num2 < textBox.MinimumSize.Height)
			{
				textBox.Height = textBox.MinimumSize.Height;
				textBox.ScrollBars = ScrollBars.None;
			}
			if (num2 >= textBox.MinimumSize.Height && num2 < textBox.MaximumSize.Height)
			{
				textBox.Height = num2;
				textBox.ScrollBars = ScrollBars.None;
			}
			if (num2 >= textBox.MaximumSize.Height)
			{
				textBox.Height = textBox.MaximumSize.Height;
				textBox.ScrollBars = ScrollBars.Vertical;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			memoNotesAutoHeight = false;
		}
	}

	private void TextBoxAutoHeight(object sender)
	{
		try
		{
			TextBox textBox = sender as TextBox;
			int num = textBox.Width;
			if (num <= textBox.MinimumSize.Width)
			{
				num = 287;
			}
			int num2 = (int)textBox.CreateGraphics().MeasureString(textBox.Text, textBox.Font, num).Height + 4;
			if (num2 < textBox.MaximumSize.Height)
			{
				textBox.Height = num2;
				textBox.ScrollBars = ScrollBars.None;
			}
			else
			{
				textBox.Height = textBox.MaximumSize.Height;
				textBox.ScrollBars = ScrollBars.Vertical;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_List_DoubleClick(object sender, EventArgs e)
	{
		try
		{
			GridView view = sender as GridView;
			DoubleClickMainTask(view);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void DoubleClickMainTask(GridView view)
	{
		GridHitInfo gridHitInfo = view.CalcHitInfo(view.GridControl.PointToClient(Control.MousePosition));
		if (gridHitInfo.InRow && !view.IsGroupRow(gridHitInfo.RowHandle) && gridHitInfo.RowHandle != -2147483647 && gridHitInfo.RowHandle != int.MinValue)
		{
			splitListTask.Collapsed = false;
			FillInfo(view.GetFocusedDataRow());
		}
	}

	private void gv_List_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
	{
		try
		{
			GridView gridView = sender as GridView;
			string fieldName = e.Column.FieldName;
			if (!(fieldName == "Closed"))
			{
				if (fieldName == "Delete" && e.RowHandle == -2147483647)
				{
					e.RepositoryItem = emptyEditor;
				}
				return;
			}
			if (e.RowHandle == -2147483647)
			{
				e.RepositoryItem = emptyEditor;
				return;
			}
			string text = Function.ToString(gridView.GetRowCellValue(e.RowHandle, e.Column));
			if (text == "Y")
			{
				e.RepositoryItem = rep_bte_Closed;
			}
			else if (text == "N")
			{
				e.RepositoryItem = rep_bte_Open;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_List_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			GridView gridView = sender as GridView;
			if (e.RowHandle == int.MinValue)
			{
				return;
			}
			if (e.RowHandle == -2147483647)
			{
				e.Appearance.BackColor = Color.White;
				return;
			}
			string text = Function.ToString(gridView.GetRowCellValue(e.RowHandle, "Closed"));
			if (e.RowHandle == gridView.FocusedRowHandle && gridView == focusedView)
			{
				e.Appearance.BackColor = Color.FromArgb(206, 213, 233);
			}
			if (e.RowHandle == gridView.FocusedRowHandle && gridView != focusedView && gridView != gv_ListSub && text == "Y")
			{
				e.Appearance.BackColor = Color.FromArgb(229, 229, 229);
				e.Appearance.ForeColor = Color.FromArgb(115, 115, 115);
			}
			if (e.RowHandle == gridView.FocusedRowHandle && gridView != focusedView && gridView != gv_ListSub && text != "Y")
			{
				e.Appearance.BackColor = Color.White;
			}
			if (e.RowHandle != gridView.FocusedRowHandle && text == "Y")
			{
				e.Appearance.BackColor = Color.FromArgb(229, 229, 229);
				e.Appearance.ForeColor = Color.FromArgb(115, 115, 115);
			}
			if (e.Column.FieldName == "endDate")
			{
				if (Function.ToString(e.CellValue) == "")
				{
					return;
				}
				DateTime dateTime = Function.ParseDateTimes(e.CellValue);
				TimeSpan value = Function.IntSAPToTimeSpan(gridView.GetRowCellValue(e.RowHandle, "ENDTime"));
				dateTime = dateTime.Add(value);
				if (gridView == gv_List)
				{
					if (dateTime < DateTime.Now)
					{
						Color color = Color.FromArgb(199, 63, 74);
						color = Color.FromArgb(199, 90, 92);
						color = Color.FromArgb(199, 43, 66);
						color = Color.FromArgb(199, 75, 92);
						color = Color.FromArgb(204, 59, 74);
						e.Appearance.ForeColor = color;
					}
					if (dateTime >= DateTime.Now)
					{
						Color color2 = Color.FromArgb(20, 137, 196);
						color2 = Color.FromArgb(0, 153, 204);
						e.Appearance.ForeColor = color2;
					}
				}
				if (gridView == gv_ListCompleted)
				{
					if (dateTime < DateTime.Now)
					{
						e.Appearance.ForeColor = Color.FromArgb(212, 115, 101);
					}
					if (dateTime >= DateTime.Now)
					{
						e.Appearance.ForeColor = Color.FromArgb(121, 171, 204);
					}
				}
			}
			if (e.Column.FieldName == "AssignTo" && !(Function.ToString(gridView.GetRowCellValue(e.RowHandle, "Closed")) == "Y"))
			{
				string value2 = Function.ToString(gridView.GetRowCellValue(e.RowHandle, "GroupUser"));
				string value3 = Function.ToString(gridView.GetRowCellValue(e.RowHandle, "GroupEmployee"));
				if (string.IsNullOrEmpty(value2) && !string.IsNullOrEmpty(value3))
				{
					e.Appearance.ForeColor = Color.FromArgb(125, 88, 0);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_List_BeforeLeaveRow(object sender, RowAllowEventArgs e)
	{
		try
		{
			if (changingRowObject || changingRowFocus)
			{
				e.Allow = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_List_FocusedColumnChanged(object sender, FocusedColumnChangedEventArgs e)
	{
	}

	private void gv_List_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			GridView gridView = sender as GridView;
			e.Cancel = true;
			if (FormAuth == Enums.FormAuthorities.Read)
			{
				if (gridView.FocusedColumn.FieldName == "Details")
				{
					gv_List.FocusedColumn.OptionsColumn.ReadOnly = true;
				}
				else
				{
					gv_List.FocusedColumn.OptionsColumn.ReadOnly = false;
				}
			}
			else if (gridView.FocusedRowHandle == -2147483647)
			{
				if (gridView.FocusedColumn.FieldName == "Details")
				{
					e.Cancel = false;
				}
			}
			else if (gridView.FocusedColumn.FieldName == "Closed")
			{
				e.Cancel = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_List_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == "endDate" && e.Column.ColumnType == typeof(DateTime))
			{
				e.DisplayText = $"{e.Value:dd.MM} ";
			}
			if (e.Column.FieldName == "Priority")
			{
				e.DisplayText = "";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void CheckValueChanging(Control control)
	{
		if (!CallFromCal)
		{
			return;
		}
		try
		{
			foreach (Control control2 in control.Controls)
			{
				if (!(control2 is BreadCrumbEdit) && !(control2 is ucLinkedDocument) && !(control2 is ucComment))
				{
					if (control2 is BaseEdit && control2.Name != "memoComment")
					{
						(control2 as BaseEdit).EditValueChanging -= frm_TaskManagement_EditValueChanging;
						(control2 as BaseEdit).EditValueChanging += frm_TaskManagement_EditValueChanging;
						(control2 as BaseEdit).EditValueChanged -= frm_TaskManagement_EditValueChanged;
						(control2 as BaseEdit).EditValueChanged += frm_TaskManagement_EditValueChanged;
					}
					if (control2 is TextBox && !(control2 is TextBoxMaskBox))
					{
						(control2 as TextBox).TextChanged += textBox_TextChanged;
					}
					CheckValueChanging(control2);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void textBox_TextChanged(object sender, EventArgs e)
	{
		try
		{
			CheckSaveState();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void frm_TaskManagement_SelectedIndexChanged(object sender, EventArgs e)
	{
		CheckSaveState();
	}

	private void cbDocNum_EditValueChanged(object sender, EventArgs e)
	{
		CheckSaveState();
	}

	private void frm_TaskManagement_EditValueChanged(object sender, EventArgs e)
	{
		CheckSaveState();
	}

	private void frm_TaskManagement_EditValueChanging(object sender, ChangingEventArgs e)
	{
		CheckSaveState();
	}

	private void CheckSaveState()
	{
		if (!_isLoading && !isFill && CallFromCal)
		{
			if (FormMode == FormModeType.SingleView)
			{
				FormMode = FormModeType.SingleUpdate;
			}
			if (FormMode == FormModeType.SingleAdd)
			{
				modifyInAddMode = true;
			}
		}
	}

	private void SetupSort()
	{
		dtSort = new DataTable();
		dtSort.Columns.Add("Code", typeof(string));
		dtSort.Columns.Add("Name", typeof(string));
		dtSort.Rows.Add("(None)", "");
		dtSort.Rows.Add("Priority", "Priority");
		dtSort.Rows.Add("Due Date", "Due Date");
		dtSort.Rows.Add("Assignee", "Assignee");
		dtSort.Rows.Add("Alphabetically", "Alphabetically");
		dtSort.AcceptChanges();
		luk_Sort.Properties.ValueMember = "Code";
		luk_Sort.Properties.DisplayMember = "Name";
		luk_Sort.Properties.DataSource = dtSort;
	}

	private void SortGridList()
	{
		try
		{
			GridViewEx gridViewEx = gv_List;
			gridViewEx.ClearSorting();
			gridViewEx.Columns["Priority"].Visible = false;
			gridViewEx.OptionsBehavior.ImmediateUpdateRowPosition = false;
			switch (Function.ToString(luk_Sort.EditValue))
			{
			case "(None)":
				gridViewEx.Columns[OrderFieldName].SortOrder = ColumnSortOrder.Descending;
				gridViewEx.OptionsBehavior.ImmediateUpdateRowPosition = true;
				break;
			case "Alphabetically":
				gridViewEx.Columns["Details"].SortOrder = ColumnSortOrder.Ascending;
				break;
			case "Due Date":
				gridViewEx.Columns["endDate"].SortOrder = ColumnSortOrder.Ascending;
				gridViewEx.Columns["ENDTime"].SortOrder = ColumnSortOrder.Ascending;
				break;
			case "Priority":
				gridViewEx.Columns["Priority"].SortOrder = ColumnSortOrder.Descending;
				gridViewEx.Columns["Priority"].Visible = true;
				break;
			case "Creation Date":
				gridViewEx.Columns["CntctDate"].SortOrder = ColumnSortOrder.Ascending;
				break;
			case "Assignee":
				gridViewEx.Columns["AssignTo"].SortOrder = ColumnSortOrder.Ascending;
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void SortGridListCompleted()
	{
		try
		{
			GridViewEx gridViewEx = gv_ListCompleted;
			gridViewEx.ClearSorting();
			gridViewEx.Columns["Priority"].Visible = false;
			gridViewEx.OptionsBehavior.ImmediateUpdateRowPosition = false;
			switch (Function.ToString(luk_Sort.EditValue))
			{
			case "(None)":
				gridViewEx.Columns["CloseDate"].SortOrder = ColumnSortOrder.Descending;
				gridViewEx.OptionsBehavior.ImmediateUpdateRowPosition = true;
				break;
			case "Alphabetically":
				gridViewEx.Columns["Details"].SortOrder = ColumnSortOrder.Ascending;
				break;
			case "Due Date":
				gridViewEx.Columns["endDate"].SortOrder = ColumnSortOrder.Ascending;
				gridViewEx.Columns["ENDTime"].SortOrder = ColumnSortOrder.Ascending;
				break;
			case "Priority":
				gridViewEx.Columns["Priority"].SortOrder = ColumnSortOrder.Descending;
				gridViewEx.Columns["Priority"].Visible = true;
				break;
			case "Creation Date":
				gridViewEx.Columns["CntctDate"].SortOrder = ColumnSortOrder.Ascending;
				break;
			case "Assignee":
				gridViewEx.Columns["AssignTo"].SortOrder = ColumnSortOrder.Ascending;
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void lookUpEditSimple1_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			isSort = true;
			SortGridList();
			SortGridListCompleted();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			isSort = false;
		}
	}

	private void gv_List_CustomColumnSort(object sender, CustomColumnSortEventArgs e)
	{
		try
		{
			if (!(sender is GridView gridView))
			{
				return;
			}
			string fieldName = e.Column.FieldName;
			if (fieldName == "endDate" || fieldName == "AssignTo")
			{
				string value = Function.ToString(gridView.GetListSourceRowCellValue(e.ListSourceRowIndex1, fieldName));
				string value2 = Function.ToString(gridView.GetListSourceRowCellValue(e.ListSourceRowIndex2, fieldName));
				if (string.IsNullOrEmpty(value))
				{
					e.Result = 1;
					e.Handled = true;
				}
				if (!string.IsNullOrEmpty(value) && string.IsNullOrEmpty(value2))
				{
					e.Result = -1;
					e.Handled = true;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_List_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
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
			string text3 = "";
			if (!string.IsNullOrEmpty(text) && string.IsNullOrEmpty(text2))
			{
				foreach (string item in text.Split(new char[1] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList())
				{
					int key = Function.ParseInt(item);
					if (dictionUser.ContainsKey(key))
					{
						text3 = text3 + dictionUser[key] + "; ";
					}
				}
			}
			if (!string.IsNullOrEmpty(text2))
			{
				foreach (string item2 in text2.Split(new char[1] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList())
				{
					int key2 = Function.ParseInt(item2);
					if (dictionEmployee.ContainsKey(key2))
					{
						text3 = text3 + dictionEmployee[key2] + "; ";
					}
				}
			}
			if (text3.Length > 2)
			{
				text3 = text3.Remove(text3.Length - 2);
			}
			e.Value = text3;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btnComplete_Click(object sender, EventArgs e)
	{
		try
		{
			btnComplete.Enabled = false;
			grd_ListCompleted.Visible = !grd_ListCompleted.Visible;
			if (grd_ListCompleted.Visible)
			{
				if (!loadCompletedTask)
				{
					DataTable dataTable = await LoadCompletedTask();
					if (dataTable.IsNotNull())
					{
						dtAPZ_OCLG = grd_List.DatasourceTable().Copy();
						dtAPZ_OCLG.SetPrimaryKey("ClgCode");
						dataTable.SetPrimaryKey("ClgCode");
						dtAPZ_OCLG.Merge(dataTable);
						dtAPZ_OCLG.PrimaryKey = null;
						grd_List.DataSource = dtAPZ_OCLG.Copy();
						grd_ListCompleted.DataSource = dtAPZ_OCLG.Copy();
						GridListAcceptChanges();
						GridListCompletedAcceptChanges();
						loadCompletedTask = true;
					}
				}
				UpdateGridSize(gv_ListCompleted);
			}
			else
			{
				focusedView = gv_List;
				grd_ListCompleted.Height = GridMinHeight;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			btnComplete.Enabled = true;
		}
	}

	private void UpdateGridSize(GridView view)
	{
		try
		{
			if (CallFromCal)
			{
				return;
			}
			GridViewInfo gridViewInfo = (GridViewInfo)view.GetViewInfo();
			int num = gridViewInfo.ViewRects.IndicatorWidth;
			if (view.VisibleColumns.Count <= 0)
			{
				return;
			}
			if (gridViewInfo.ColumnsInfo.Count <= 0)
			{
				view.GridControl.Refresh();
				if (gridViewInfo.ColumnsInfo.Count <= 0)
				{
					return;
				}
			}
			foreach (GridColumn visibleColumn in view.VisibleColumns)
			{
				if (gridViewInfo.GetColumnLeftCoord(visibleColumn) < gridViewInfo.ViewRects.ColumnPanelWidth)
				{
					view.LeftCoord = num;
				}
				GridColumnInfoArgs gridColumnInfoArgs = gridViewInfo.ColumnsInfo[visibleColumn];
				num += gridColumnInfoArgs.Bounds.Width;
			}
			ScrollInfo scrollInfo = (ScrollInfo)typeof(GridView).GetField("scrollInfo", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(view);
			if (scrollInfo.VScrollVisible)
			{
				num += scrollInfo.VScrollSize;
			}
			int num2 = gridViewInfo.CalcRealViewHeight(new Rectangle(0, 0, ClientSize.Width, ClientSize.Height));
			if (scrollInfo.HScrollVisible)
			{
				num2 += scrollInfo.HScrollSize;
			}
			num = Math.Max(GridMinWidth, num);
			num = Math.Min(ClientSize.Width - view.GridControl.Location.X, num);
			num2 = Math.Max(GridMinHeight, num2);
			if (num2 > Height - 100)
			{
				num2 = Height - 100;
			}
			view.GridControl.Size = new Size(num, num2);
			view.LayoutChanged();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_List_RowCountChanged(object sender, EventArgs e)
	{
		UpdateGridSize(sender as GridView);
	}

	private void gv_ListCompleted_RowCountChanged(object sender, EventArgs e)
	{
		GridView view = sender as GridView;
		UpdateGridSize(view);
	}

	private void grd_List_SizeChanged(object sender, EventArgs e)
	{
		if (grd_ListCompleted.Visible)
		{
			tableLayoutPanel1.Height = grd_List.Height + 40 + grd_ListCompleted.Height;
		}
		else
		{
			tableLayoutPanel1.Height = grd_List.Height + 40;
		}
	}

	private void grd_ListCompleted_SizeChanged(object sender, EventArgs e)
	{
		if (grd_ListCompleted.Visible)
		{
			tableLayoutPanel1.Height = grd_List.Height + 40 + grd_ListCompleted.Height;
		}
		else
		{
			tableLayoutPanel1.Height = grd_List.Height + 40;
		}
	}

	private void accordionControl1_SelectedElementChanged(object sender, SelectedElementChangedEventArgs e)
	{
		try
		{
			if (e.Element.OwnerElement == null)
			{
				return;
			}
			string text = Function.ToString(e.Element.Tag);
			switch (Function.ToString(e.Element.OwnerElement.Tag))
			{
			case "General":
				gv_List.NewItemRowText = "Add Task";
				switch (text)
				{
				case "All":
					TaskFilter = TaskFilterType.None;
					break;
				case "Overdue":
					TaskFilter = TaskFilterType.Overdue;
					break;
				case "Today":
					TaskFilter = TaskFilterType.Today;
					break;
				case "ThisWeek":
					TaskFilter = TaskFilterType.ThisWeek;
					break;
				case "LastWeek":
					TaskFilter = TaskFilterType.LastWeek;
					break;
				}
				break;
			case "Users":
				if (text == "Unassigned")
				{
					gv_List.NewItemRowText = "Add Unassigned Task";
					TaskFilter = TaskFilterType.Unassigned;
				}
				else
				{
					string text3 = e.Element.Text;
					gv_List.NewItemRowText = "Add Task for " + text3;
					TaskFilter = TaskFilterType.Users;
				}
				break;
			case "Employees":
				if (text == "Unassigned")
				{
					gv_List.NewItemRowText = "Add Unassigned Task ";
					TaskFilter = TaskFilterType.Unassigned;
				}
				else
				{
					string text2 = e.Element.Text;
					gv_List.NewItemRowText = "Add Task for " + text2;
					TaskFilter = TaskFilterType.Employees;
				}
				break;
			case "Project New":
			{
				string str = e.Element.Text;
				str = str.GetCharsBefore(" - ");
				gv_List.NewItemRowText = "Add Task for " + str;
				TaskFilter = TaskFilterType.ProjectNew;
				break;
			}
			}
			grd_List.RefreshDataSource();
			grd_ListCompleted.RefreshDataSource();
			splitListTask.Collapsed = true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_List_CustomRowFilter(object sender, RowFilterEventArgs e)
	{
		try
		{
			ColumnView columnView = sender as ColumnView;
			if (Function.ToString(columnView.GetListSourceRowCellValue(e.ListSourceRow, "Closed")) == "Y")
			{
				e.Visible = false;
				e.Handled = true;
			}
			else
			{
				AccordionFilter(e, columnView);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_ListCompleted_CustomRowFilter(object sender, RowFilterEventArgs e)
	{
		try
		{
			ColumnView columnView = sender as ColumnView;
			string text = Function.ToString(columnView.GetListSourceRowCellValue(e.ListSourceRow, "Closed"));
			if (text == "N" || text == "")
			{
				e.Visible = false;
				e.Handled = true;
			}
			else
			{
				AccordionFilter(e, columnView);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void AccordionFilter(RowFilterEventArgs e, ColumnView view)
	{
		try
		{
			switch (TaskFilter)
			{
			case TaskFilterType.None:
				e.Handled = true;
				return;
			case TaskFilterType.Overdue:
			{
				if (Function.ToString(view.GetListSourceRowCellValue(e.ListSourceRow, "endDate")) == "")
				{
					e.Visible = false;
					e.Handled = true;
					return;
				}
				DateTime dateTime = Function.ParseDateTimes(view.GetListSourceRowCellValue(e.ListSourceRow, "endDate"));
				TimeSpan value = Function.IntSAPToTimeSpan(view.GetListSourceRowCellValue(e.ListSourceRow, "ENDTime"));
				dateTime = dateTime.Add(value);
				if (dateTime >= DateTime.Now)
				{
					e.Visible = false;
					e.Handled = true;
				}
				return;
			}
			case TaskFilterType.Users:
			{
				string text3 = Function.ToString(view.GetListSourceRowCellValue(e.ListSourceRow, "GroupUser"));
				if (string.IsNullOrEmpty(text3))
				{
					e.Visible = false;
					e.Handled = true;
					return;
				}
				if (!string.IsNullOrEmpty(Function.ToString(view.GetListSourceRowCellValue(e.ListSourceRow, "GroupEmployee"))))
				{
					e.Visible = false;
					e.Handled = true;
					return;
				}
				List<string> list2 = text3.Split(new char[1] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
				for (int j = 0; j < list2.Count; j++)
				{
					list2[j] = list2[j].Trim();
				}
				string item2 = Function.ToString(accordionControl1.SelectedElement.Tag);
				if (!list2.Contains(item2))
				{
					e.Visible = false;
					e.Handled = true;
				}
				return;
			}
			case TaskFilterType.Employees:
			{
				string text2 = Function.ToString(view.GetListSourceRowCellValue(e.ListSourceRow, "GroupEmployee"));
				if (string.IsNullOrEmpty(text2))
				{
					e.Visible = false;
					e.Handled = true;
					return;
				}
				List<string> list = text2.Split(new char[1] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
				for (int i = 0; i < list.Count; i++)
				{
					list[i] = list[i].Trim();
				}
				string item = Function.ToString(accordionControl1.SelectedElement.Tag);
				if (!list.Contains(item))
				{
					e.Visible = false;
					e.Handled = true;
				}
				return;
			}
			case TaskFilterType.ProjectNew:
			{
				string text = Function.ToString(view.GetListSourceRowCellValue(e.ListSourceRow, "ProjectEntry"));
				if (string.IsNullOrEmpty(text))
				{
					e.Visible = false;
					e.Handled = true;
					return;
				}
				string value2 = Function.ToString(accordionControl1.SelectedElement.Tag);
				if (!text.Equals(value2))
				{
					e.Visible = false;
					e.Handled = true;
				}
				return;
			}
			case TaskFilterType.Unassigned:
			{
				string value3 = Function.ToString(view.GetListSourceRowCellValue(e.ListSourceRow, "GroupUser"));
				string value4 = Function.ToString(view.GetListSourceRowCellValue(e.ListSourceRow, "GroupEmployee"));
				if (!string.IsNullOrEmpty(value3) || !string.IsNullOrEmpty(value4))
				{
					e.Visible = false;
					e.Handled = true;
				}
				return;
			}
			}
			DateTime? dateTime2 = Function.ParseDateTime(view.GetListSourceRowCellValue(e.ListSourceRow, "Recontact"));
			DateTime dateTime3 = (dateTime2.HasValue ? dateTime2.Value : DateTime.MinValue);
			TimeSpan value5 = Function.IntSAPToTimeSpan(view.GetListSourceRowCellValue(e.ListSourceRow, "BeginTime"));
			dateTime3 = dateTime3.Add(value5);
			switch (TaskFilter)
			{
			case TaskFilterType.Today:
				if (dateTime3.Date != DateTime.Today)
				{
					e.Visible = false;
					e.Handled = true;
				}
				break;
			case TaskFilterType.ThisWeek:
			{
				TimeSpan timeSpan2 = dateTime3.Date - DateTime.Today;
				if (timeSpan2.Days < 0 || timeSpan2.Days >= 7)
				{
					e.Visible = false;
					e.Handled = true;
				}
				break;
			}
			case TaskFilterType.LastWeek:
			{
				TimeSpan timeSpan = dateTime3.Date - DateTime.Today;
				if (timeSpan.Days <= -7 || timeSpan.Days >= 0)
				{
					e.Visible = false;
					e.Handled = true;
				}
				break;
			}
			case TaskFilterType.None:
			case TaskFilterType.Overdue:
			case TaskFilterType.Users:
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_ListSub_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			GridView gridView = sender as GridView;
			if (gridView.FocusedRowHandle == -2147483647)
			{
				_ = gridView.FocusedColumn.FieldName != "Details";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void gv_ListSub_RowUpdated(object sender, RowObjectEventArgs e)
	{
		if (rowUpdatedSub)
		{
			return;
		}
		try
		{
			if (!_isLoading && !changeRowStatus && e.Row is DataRowView dr)
			{
				GridView view = sender as GridView;
				int _clgCode = Function.ParseInt(dr["ClgCode"]);
				rowUpdatedSub = true;
				view.OptionsBehavior.ReadOnly = true;
				if (_clgCode == 0 && e.RowHandle == -2147483647)
				{
					await AddSubTask(dr.Row);
				}
				if (_clgCode != 0 && view.FocusedColumn.FieldName != "Closed")
				{
					await SaveSubTaskDetails(dr.Row);
				}
				view.OptionsBehavior.ReadOnly = false;
				view.ShowEditor();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			rowUpdatedSub = false;
		}
	}

	private void gv_List_InitNewRow(object sender, InitNewRowEventArgs e)
	{
		try
		{
			GridView obj = sender as GridView;
			obj.SetRowCellValue(_value: Function.ParseDouble((obj.GridControl.DataSource as DataTable).Compute("MAX(OrderLine)", "")) + 1.0, rowHandle: e.RowHandle, fieldName: "OrderLine");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void grd_ListSub_ProcessGridKey(object sender, KeyEventArgs e)
	{
		if (processKey)
		{
			return;
		}
		try
		{
			_ = (sender as GridControl).MainView;
			if (e.Modifiers == Keys.Control && e.KeyCode == Keys.V)
			{
				string text = Clipboard.GetText();
				if (!text.Contains(Environment.NewLine))
				{
					return;
				}
				processKey = true;
				string[] splitTextList = text.Split(new string[1] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
				DataTable dt = grd_ListSub.DataSourceTable;
				double maxOrderLine = Function.ParseDouble(dt.Compute("MAX(OrderLine)", ""));
				for (int i = 0; i < splitTextList.Length; i++)
				{
					DataRow dataRow = dt.NewRow();
					dataRow["ClgCode"] = 0;
					dataRow["Details"] = splitTextList[i];
					dataRow["OrderLine"] = maxOrderLine + 1.0;
					maxOrderLine++;
					dt.Rows.Add(dataRow);
					await AddSubTask(dataRow);
					gv_ListSub.MoveLast();
				}
				dt.AcceptChanges();
				e.SuppressKeyPress = true;
			}
			if (e.KeyCode == Keys.Return && gv_ListSub.FocusedRowHandle == -2147483647)
			{
				processKey = true;
				gv_ListSub.FocusedRowHandle++;
				e.Handled = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			processKey = false;
		}
	}

	private void gv_ListSub_FocusedColumnChanged(object sender, FocusedColumnChangedEventArgs e)
	{
	}

	private void gv_ListSub_ShownEditor(object sender, EventArgs e)
	{
		try
		{
			GridView gridView = sender as GridView;
			if (!(gridView.FocusedColumn.FieldName != "Details"))
			{
				inplaceEditor = gridView.ActiveEditor;
				if (inplaceEditor != null)
				{
					inplaceEditor.DoubleClick += inplaceEditor_DoubleClick;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void inplaceEditor_DoubleClick(object sender, EventArgs e)
	{
		try
		{
			GridControl gridControl = (GridControl)((BaseEdit)sender).Parent;
			Point pt = gridControl.PointToClient(Control.MousePosition);
			GridView view = (GridView)gridControl.FocusedView;
			DoubleClickListSub(view, pt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void panelControl1_Paint(object sender, PaintEventArgs e)
	{
	}

	private void gv_ListSub_HiddenEditor(object sender, EventArgs e)
	{
		try
		{
			if (inplaceEditor != null)
			{
				inplaceEditor.DoubleClick -= inplaceEditor_DoubleClick;
				inplaceEditor = null;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void DoubleClickListSub(GridView view, Point pt)
	{
		GridHitInfo gridHitInfo = view.CalcHitInfo(pt);
		if (gridHitInfo.InRow && !view.IsGroupRow(gridHitInfo.RowHandle) && gridHitInfo.RowHandle != -2147483647 && gridHitInfo.RowHandle != int.MinValue)
		{
			FillInfo(view.GetFocusedDataRow());
		}
	}

	private void cfl_ProjectNew_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
	{
	}

	private async void rep_bte_DeleteSubTask_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			GridViewEx gridViewFromEditor = (sender as BaseEdit).GetGridViewFromEditor();
			await DeleteSub(gridViewFromEditor);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void toolStripMenuItem_DeleteCM_Click(object sender, EventArgs e)
	{
		try
		{
			GridView view = (((sender as ToolStripMenuItem).GetCurrentParent() as ContextMenuStrip).SourceControl as GridControl).MainView as GridView;
			await DeleteSub(view);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			deleteTask = false;
		}
	}

	private async Task DeleteSub(GridView view)
	{
		if (deleteTask)
		{
			return;
		}
		try
		{
			if (view != null)
			{
				deleteTask = true;
				if (view.GetFocusedRow() is DataRowView { Row: not null } dataRowView && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					Task task = DeleteSubTask(dataRowView);
					view.DeleteRow(view.FocusedRowHandle);
					await task;
					view.FocusedColumn = view.Columns["Details"];
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			deleteTask = false;
		}
	}

	private async Task DeleteSubTask(DataRowView dr)
	{
		_ = 3;
		try
		{
			if (dr == null)
			{
				return;
			}
			DataTable dataTable = grd_ListSub.DataSourceTable().Clone();
			if (!dataTable.HasColumn())
			{
				return;
			}
			DataRow dataRow = dataTable.NewRow();
			dataRow.ItemArray = dr.Row.ItemArray;
			int clgCode = Function.ParseInt(dataRow["ClgCode"]);
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			string text = await _httpClient.SubActivities.RemoveTask(dataTable);
			if (!string.IsNullOrEmpty(text))
			{
				WinCommons.Error(Enums.MessageCode.None, text);
				grd_ListSub.DataSourceTable().RejectChanges();
				return;
			}
			GridAcceptChanges(gv_ListSub);
			await WinFuntions.RefreshVisibleCalendars();
			if (ReloadCalendar != null && !((XtraForm)ReloadCalendar.Target).IsDisposed)
			{
				await ReloadCalendar();
			}
			if (ReloadAfterRemove != null && !((XtraForm)ReloadAfterRemove.Target).IsDisposed)
			{
				await ReloadAfterRemove(clgCode);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			grd_ListSub.DataSourceTable().RejectChanges();
		}
	}

	private async void gv_ListSub_RowCellClick(object sender, RowCellClickEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == "Delete")
			{
				await DeleteSub(sender as GridView);
			}
			if (e.Column.FieldName == "Closed")
			{
				GridView gridView = sender as GridView;
				DataRow focusedDataRow = gridView.GetFocusedDataRow();
				if (Function.ToString(focusedDataRow["Closed"]) == "Y")
				{
					gridView.SetFocusedRowCellValue("Closed", "N");
				}
				else
				{
					gridView.SetFocusedRowCellValue("Closed", "Y");
				}
				ChangeTaskStatus(focusedDataRow);
				gridView.CloseEditor();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void detailsToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			await FillInfo(gv_ListSub.GetFocusedDataRow());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_ListSub_CustomRowCellEditForEditing(object sender, CustomRowCellEditEventArgs e)
	{
		try
		{
			if (!(e.Column.FieldName != "Delete") && e.RowHandle == -2147483647)
			{
				e.RepositoryItem = emptyEditor;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_ListSub_MouseMove(object sender, MouseEventArgs e)
	{
		try
		{
			GridView obj = sender as GridView;
			GridHitInfo gridHitInfo = obj.CalcHitInfo(e.Location);
			if (gridHitInfo.InRowCell)
			{
				rowHandleGridSub = gridHitInfo.RowHandle;
			}
			else
			{
				rowHandleGridSub = -1;
			}
			obj.Invalidate();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_ListSub_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
	{
		try
		{
			if (e.RowHandle != -2147483647 && e.RowHandle != int.MinValue && e.Column.FieldName == "Delete" && e.RowHandle == rowHandleGridSub)
			{
				ButtonEditViewInfo buttonEditViewInfo = (e.Cell as GridCellInfo).ViewInfo as ButtonEditViewInfo;
				if (e.Column.ColumnEdit is RepositoryItemButtonEdit repositoryItemButtonEdit)
				{
					EditorButton button = repositoryItemButtonEdit.Buttons[0];
					EditorButtonObjectInfoArgs editorButtonObjectInfoArgs = new EditorButtonObjectInfoArgs(e.Cache, button, e.Appearance);
					e.Appearance.BackColor = Color.White;
					editorButtonObjectInfoArgs.Bounds = new Rectangle(e.Bounds.Right - 17, e.Bounds.Y, 17, e.Bounds.Height);
					buttonEditViewInfo.EditorButtonPainter.DrawObject(editorButtonObjectInfoArgs);
					Rectangle bounds = e.Bounds;
					bounds.Offset(2, 0);
					e.Appearance.DrawString(e.Cache, e.DisplayText, bounds);
					e.Handled = true;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void ctm_MainTaskAssigner_Click(object sender, EventArgs e)
	{
		try
		{
			GridViewEx gridViewEx = gv_ListSub;
			DataRow dataRow = gridViewEx.GetDataRow(gridViewEx.FocusedRowHandle);
			if (dataRow != null && dtRow.Rows.Count >= 0)
			{
				object value = dtRow.Rows[0]["GroupUser"];
				object value2 = dtRow.Rows[0]["GroupEmployee"];
				dataRow["GroupUser"] = value;
				dataRow["GroupEmployee"] = value2;
				await SaveSubTaskAssigner(dataRow);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void toolTipGridSub_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
	{
		try
		{
			if (e.SelectedControl != grd_ListSub)
			{
				return;
			}
			GridHitInfo gridHitInfo = gv_ListSub.CalcHitInfo(e.ControlMousePosition);
			if (!gridHitInfo.InRow || gridHitInfo.RowHandle == -2147483647)
			{
				return;
			}
			new SuperToolTipSetupArgs();
			ToolTipControlInfo toolTipControlInfo = new ToolTipControlInfo();
			string text = "";
			DataRow dataRow = gv_ListSub.GetDataRow(gridHitInfo.RowHandle);
			if (dataRow != null)
			{
				string text2 = Function.ToString(dataRow["GroupUser"]);
				string text3 = Function.ToString(dataRow["GroupEmployee"]);
				List<string> list = new List<string>();
				if (!string.IsNullOrEmpty(text3))
				{
					string[] array = text3.Split(new char[1] { ';' }, StringSplitOptions.RemoveEmptyEntries);
					for (int i = 0; i < array.Length; i++)
					{
						int key = Function.ParseInt(array[i]);
						if (dictionEmployee.ContainsKey(key))
						{
							string item = dictionEmployee[key];
							list.Add(item);
						}
					}
					if (list.Count == 1)
					{
						text = "Assign to employee: " + string.Join(" ;", list);
					}
					if (list.Count > 1)
					{
						text = "Assign to employees: " + string.Join(" ;", list);
					}
				}
				if (string.IsNullOrEmpty(text3) && !string.IsNullOrEmpty(text2))
				{
					string[] array = text2.Split(new char[1] { ';' }, StringSplitOptions.RemoveEmptyEntries);
					for (int i = 0; i < array.Length; i++)
					{
						int key2 = Function.ParseInt(array[i]);
						if (dictionUser.ContainsKey(key2))
						{
							string item2 = dictionUser[key2];
							list.Add(item2);
						}
					}
					if (list.Count == 1)
					{
						text = "Assign to user: " + string.Join(" ;", list);
					}
					if (list.Count > 1)
					{
						text = "Assign to users: " + string.Join(" ;", list);
					}
				}
			}
			toolTipControlInfo.Object = gridHitInfo.HitTest.ToString() + gridHitInfo.RowHandle;
			toolTipControlInfo.Text = text;
			e.Info = toolTipControlInfo;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void GridSubAutoRowHeight()
	{
		try
		{
			grd_ListSub.Height = 2000;
			GridViewInfo gridViewInfo = (grd_ListSub.MainView as GridView).GetViewInfo() as GridViewInfo;
			int num = 0;
			foreach (GridDataRowInfo item in gridViewInfo.RowsInfo)
			{
				num += item.Bounds.Height;
			}
			typeof(GridView).GetField("scrollInfo", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(grd_ListSub.MainView as GridView);
			int num2 = 3;
			int num3 = num + gridViewInfo.ViewRects.ColumnPanel.Height + gridViewInfo.ViewRects.Footer.Height + gridViewInfo.ViewRects.GroupPanel.Height + num2;
			if (num3 > Height - 100)
			{
				num3 = Height - 100;
			}
			grd_ListSub.Height = num3;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_ListSub_RowCountChanged(object sender, EventArgs e)
	{
		GridSubAutoRowHeight();
	}

	private void gv_ListSub_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == "Closed")
			{
				e.DisplayText = "";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void breadCrumbEdit1_Properties_PropertiesChanged(object sender, EventArgs e)
	{
		try
		{
			if (brc_ParentTask.Properties.BreadCrumbMode == BreadCrumbMode.Edit)
			{
				brc_ParentTask.Properties.BreadCrumbMode = BreadCrumbMode.Select;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void breadCrumbEdit1_Properties_ValidatePath(object sender, BreadCrumbValidatePathEventArgs e)
	{
		try
		{
			if (e.Path == pathTask)
			{
				e.ValidationResult = BreadCrumbValidatePathResult.CreateNodes;
			}
			else
			{
				e.ValidationResult = BreadCrumbValidatePathResult.Cancel;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void breadCrumbEdit1_Properties_NewNodeAdding(object sender, BreadCrumbNewNodeAddingEventArgs e)
	{
		try
		{
			if (dictionNodes.ContainsKey(Convert.ToInt32(e.Node.Value)))
			{
				string text = dictionNodes[Convert.ToInt32(e.Node.Value)];
				if (text.Length > 30)
				{
					text = text.Substring(0, 30) + "...";
				}
				e.Node.Caption = text;
				e.Node.PopulateOnDemand = true;
				LoadChildNodes(e.Node);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void breadCrumbEdit1_Properties_NodeClick(object sender, BreadCrumbNodeClickEventArgs e)
	{
		try
		{
			int clgCode = Function.ParseInt(e.Node.Value);
			await LoadTaskByNode(clgCode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void brc_ParentTask_Properties_SelectedNodeChanged(object sender, BreadCrumbSelectedNodeChangedEventArgs e)
	{
		try
		{
			if (e.NewNode.Parent != null)
			{
				int clgCode = Function.ParseInt(e.NewNode.Value);
				await LoadTaskByNode(clgCode);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadTaskByNode(int clgCode)
	{
		if (loadSingleTask)
		{
			return;
		}
		try
		{
			if (clgCode != 0 && clgCode != Function.ParseInt(txtClgCode.EditValue))
			{
				loadSingleTask = true;
				await FillInfo(clgCode);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			loadSingleTask = false;
		}
	}

	private async void brc_ParentTask_Properties_QueryChildNodes(object sender, BreadCrumbQueryChildNodesEventArgs e)
	{
		if (queryChildNode)
		{
			return;
		}
		try
		{
			if (e.Node.ChildNodes.Count <= 1)
			{
				queryChildNode = true;
				await LoadChildNodes(e.Node);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			queryChildNode = false;
		}
	}

	private async Task LoadChildNodes(BreadCrumbNode node)
	{
		try
		{
			int num = Function.ParseInt(node.Value);
			if (num == 0)
			{
				return;
			}
			DataTable dataTable = await LoadSubNode(num);
			if (!dataTable.IsNotNull())
			{
				return;
			}
			foreach (DataRow item in dataTable.AsEnumerable())
			{
				int num2 = Function.ParseInt(item["ClgCode"]);
				if (num2 == 0)
				{
					continue;
				}
				bool flag = false;
				foreach (BreadCrumbNode childNode in node.ChildNodes)
				{
					if (Function.ParseInt(childNode.Value) == num2)
					{
						flag = true;
					}
				}
				if (!flag)
				{
					BreadCrumbNode node2 = new BreadCrumbNode(Function.ToString(item["Details"]), num2, populateOnDemand: true);
					node.ChildNodes.Add(node2);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void copyTaskToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (!copyTask && dtRow.IsNotNull())
			{
				await CopyTask(dtRow.Rows[0]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void copyTaskAndToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (!copyTask && dtRow.IsNotNull())
			{
				await AdvancedCopyTask(dtRow.Rows[0]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task AdvancedCopyTask(DataRow copyDataRow)
	{
		try
		{
			DataRow dataRow = dtAPZ_OCLG.NewRow();
			dataRow.ItemArray = copyDataRow.ItemArray;
			frm_CopyTaskSetting frm_CopyTaskSetting2 = new frm_CopyTaskSetting(dataRow);
			frm_CopyTaskSetting2.StartPosition = FormStartPosition.CenterScreen;
			frm_CopyTaskSetting2.ShowDialog(this);
			if (!frm_CopyTaskSetting2.CopyTask)
			{
				return;
			}
			dataRow["Details"] = frm_CopyTaskSetting2.txtDetails.Text;
			int num = Function.ParseInt(frm_CopyTaskSetting2.rgp_DueDate.EditValue);
			DueDateOptions dueDateOptions = DueDateOptions.ResetDueDate;
			switch (num)
			{
			case 0:
				dueDateOptions = DueDateOptions.Recalculate;
				break;
			case 1:
				dueDateOptions = DueDateOptions.RecalculateKeepStartTime;
				break;
			case 2:
				dueDateOptions = DueDateOptions.KeepDueDate;
				break;
			case 3:
				dueDateOptions = DueDateOptions.ResetDueDate;
				break;
			}
			if (!frm_CopyTaskSetting2.chk_CopyAssignee.Checked)
			{
				dataRow["GroupUser"] = DBNull.Value;
				dataRow["GroupEmployee"] = DBNull.Value;
			}
			if (!frm_CopyTaskSetting2.chk_CopyProject.Checked)
			{
				dataRow["ProjectEntry"] = DBNull.Value;
				dataRow["ProjectWBS"] = DBNull.Value;
			}
			if (!frm_CopyTaskSetting2.chk_CopyAttachment.Checked)
			{
				dataRow["AtcEntry"] = DBNull.Value;
			}
			Function.ParseInt(dataRow["ClgCode"]);
			if (frm_CopyTaskSetting2.chk_IsSubTask.Checked)
			{
				int num2 = Function.ParseInt(frm_CopyTaskSetting2.txtParentTask.Text);
				if (num2 != 0)
				{
					dataRow["parentTask"] = num2;
				}
				if (num2 == 0)
				{
					dataRow["parentTask"] = DBNull.Value;
				}
			}
			if (!frm_CopyTaskSetting2.chk_IsSubTask.Checked)
			{
				dataRow["parentTask"] = DBNull.Value;
			}
			bool keepSubTask = frm_CopyTaskSetting2.chk_CopySubTask.Checked;
			bool keepSubTaskOfSubTask = frm_CopyTaskSetting2.chk_CopySubTaskOfSubTask.Checked;
			bool copyCompletedSubTask = frm_CopyTaskSetting2.chk_CopyCompletedSubTask.Checked;
			await CopyTask(dataRow, advancedCopy: true, keepSubTask, keepSubTaskOfSubTask, dueDateOptions, copyCompletedSubTask);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task CopyTask(DataRow copyDataRow, bool advancedCopy = false, bool keepSubTask = true, bool keepSubTaskOfSubTask = true, DueDateOptions dueDateOptions = DueDateOptions.ResetDueDate, bool copyCompletedSubTask = true)
	{
		_ = 2;
		try
		{
			DataTable dtGridList = grd_List.DataSourceTable;
			DataRow dr = dtGridList.NewRow();
			dr.ItemArray = copyDataRow.ItemArray;
			int clgCode = Function.ParseInt(dr["ClgCode"]);
			if (clgCode == 0)
			{
				return;
			}
			if (Function.ToString(dr["Details"]) == "")
			{
				WinCommons.Error(Enums.MessageCode.TaskNameIsRequired4Copy, "");
				return;
			}
			dr["prevActvty"] = dr["ClgCode"];
			dr["ClgCode"] = 0;
			dr["CntctDate"] = DBNull.Value;
			dr["AssignedBy"] = DBNull.Value;
			if (advancedCopy)
			{
				switch (dueDateOptions)
				{
				case DueDateOptions.Recalculate:
					RecalculateDueDateAndDueTime(dr);
					dr["Recontact"] = DBNull.Value;
					dr["BeginTime"] = DBNull.Value;
					break;
				case DueDateOptions.RecalculateKeepStartTime:
					RecalculateDueDateOnly(dr);
					dr["Recontact"] = DateTime.Today;
					break;
				case DueDateOptions.KeepDueDate:
					if (Function.ToString(dr["endDate"]) != "")
					{
						DateTime dateTime = Function.ParseDateTimes(dr["endDate"]);
						TimeSpan value = Function.IntSAPToTimeSpan(dr["ENDtime"]);
						dateTime = dateTime.Add(value);
						if (dateTime <= DateTime.Now)
						{
							dr["endDate"] = DBNull.Value;
							dr["ENDtime"] = DBNull.Value;
						}
					}
					dr["Recontact"] = DBNull.Value;
					dr["BeginTime"] = DBNull.Value;
					break;
				case DueDateOptions.ResetDueDate:
					dr["endDate"] = DBNull.Value;
					dr["ENDtime"] = DBNull.Value;
					dr["Recontact"] = DBNull.Value;
					dr["BeginTime"] = DBNull.Value;
					break;
				}
			}
			if (!advancedCopy)
			{
				dr["Recontact"] = DBNull.Value;
				dr["BeginTime"] = DBNull.Value;
				DataRow dataRow = dr;
				dataRow["Details"] = dataRow["Details"]?.ToString() + " (Copy)";
				dr["endDate"] = DBNull.Value;
				dr["ENDTime"] = DBNull.Value;
			}
			copyTask = true;
			await AddMainTask(dr, SaveType.Copy);
			string text = Function.ToString(dr["parentTask"]);
			int num = Function.ParseInt(dr["ClgCode"]);
			if (text == "")
			{
				DataRow dataRow2 = dtGridList.NewRow();
				dataRow2.ItemArray = dr.ItemArray;
				dataRow2["OrderLine"] = num;
				dtGridList.Rows.Add(dataRow2);
				dtGridList.AcceptChanges();
				int focusedRowHandle = gv_List.LocateByValue("ClgCode", num);
				gv_List.FocusedRowHandle = focusedRowHandle;
			}
			splitListTask.Collapsed = false;
			int copiedTaskClgCode = Function.ParseInt(dr["ClgCode"]);
			if (keepSubTask)
			{
				await CopyingSubTask(keepSubTaskOfSubTask, dueDateOptions, copyCompletedSubTask, clgCode, copiedTaskClgCode);
			}
			copyTask = false;
			await FillInfo(copiedTaskClgCode);
			if (ReloadCalendar != null && !((XtraForm)ReloadCalendar.Target).IsDisposed)
			{
				ReloadCalendar();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			copyTask = false;
		}
	}

	private async Task CopySubTaskForNewCopiedTask(DataRow copyDataRow, int copyParentTask, bool keepSubTaskOfSubTask = true, DueDateOptions dueDateOptions = DueDateOptions.ResetDueDate, bool copyCompletedSubTask = true)
	{
		_ = 1;
		try
		{
			_ = grd_List.DataSourceTable;
			int clgCode = Function.ParseInt(copyDataRow["ClgCode"]);
			if (clgCode != 0)
			{
				copyDataRow["prevActvty"] = copyDataRow["ClgCode"];
				copyDataRow["ClgCode"] = 0;
				switch (dueDateOptions)
				{
				case DueDateOptions.Recalculate:
					RecalculateDueDateAndDueTime(copyDataRow);
					copyDataRow["Recontact"] = DBNull.Value;
					copyDataRow["BeginTime"] = DBNull.Value;
					break;
				case DueDateOptions.RecalculateKeepStartTime:
					RecalculateDueDateOnly(copyDataRow);
					copyDataRow["Recontact"] = DateTime.Today;
					break;
				case DueDateOptions.KeepDueDate:
					copyDataRow["endDate"] = DBNull.Value;
					copyDataRow["ENDTime"] = DBNull.Value;
					copyDataRow["Recontact"] = DBNull.Value;
					copyDataRow["BeginTime"] = DBNull.Value;
					break;
				case DueDateOptions.ResetDueDate:
					copyDataRow["endDate"] = DBNull.Value;
					copyDataRow["ENDTime"] = DBNull.Value;
					copyDataRow["Recontact"] = DBNull.Value;
					copyDataRow["BeginTime"] = DBNull.Value;
					break;
				}
				copyDataRow["CntctDate"] = DBNull.Value;
				copyDataRow["AssignedBy"] = DBNull.Value;
				copyDataRow["parentTask"] = copyParentTask;
				await AddMainTask(copyDataRow, SaveType.Copy);
				int copyParentTask2 = Function.ParseInt(copyDataRow["ClgCode"]);
				if (keepSubTaskOfSubTask)
				{
					await CopyingSubTask(keepSubTaskOfSubTask, dueDateOptions, copyCompletedSubTask, clgCode, copyParentTask2);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task CopyingSubTask(bool keepSubTaskOfSubTask, DueDateOptions dueDateOptions, bool copyCompletedSubTask, int clgCode, int copyParentTask)
	{
		string userNameLogin = GlobalData.UserNameLogin;
		DataTable dataTable = await _httpClient.SubActivities.LoadSubTask(clgCode, userNameLogin);
		if (!dataTable.IsNotNull())
		{
			return;
		}
		foreach (DataRow item in dataTable.AsEnumerable())
		{
			if (!(Function.ToString(item["Closed"]) == "Y") || copyCompletedSubTask)
			{
				int num = Function.ParseInt(item["ClgCode"]);
				if (num != clgCode && num != copyParentTask)
				{
					await CopySubTaskForNewCopiedTask(item, copyParentTask, keepSubTaskOfSubTask, dueDateOptions, copyCompletedSubTask);
				}
			}
		}
	}

	private void RecalculateDueDateAndDueTime(DataRow dr)
	{
		if (Function.ToString(dr["endDate"]) == "")
		{
			return;
		}
		DateTime dateTime = Function.ParseDateTimes(dr["endDate"]);
		TimeSpan value = Function.IntSAPToTimeSpan(dr["ENDtime"]);
		dateTime = dateTime.Add(value);
		if (!(Function.ToString(dr["Recontact"]) == ""))
		{
			DateTime dateTime2 = Function.ParseDateTimes(dr["Recontact"]);
			TimeSpan value2 = Function.IntSAPToTimeSpan(dr["BeginTime"]);
			dateTime2 = dateTime2.Add(value2);
			if (!(dateTime < dateTime2))
			{
				TimeSpan value3 = dateTime - dateTime2;
				DateTime dateTime3 = DateTime.Now.Add(value3);
				DateTime date = dateTime3.Date;
				int num = Function.ParseTimeSpanToInt(dateTime3.TimeOfDay);
				dr["endDate"] = date;
				dr["ENDTime"] = num;
			}
		}
	}

	private void RecalculateDueDateOnly(DataRow dr)
	{
		if (Function.ToString(dr["endDate"]) == "")
		{
			return;
		}
		DateTime dateTime = Function.ParseDateTimes(dr["endDate"]);
		if (!(Function.ToString(dr["Recontact"]) == ""))
		{
			DateTime dateTime2 = Function.ParseDateTimes(dr["Recontact"]);
			Function.IntSAPToTimeSpan(dr["BeginTime"]);
			if (!(dateTime < dateTime2))
			{
				TimeSpan timeSpan = dateTime.Date - dateTime2.Date;
				DateTime dateTime3 = DateTime.Today + timeSpan;
				dr["endDate"] = dateTime3;
			}
		}
	}

	private async void copyTaskToolStripMenuItem1_Click(object sender, EventArgs e)
	{
		try
		{
			if (!copyTask && (((sender as ToolStripMenuItem).Owner as ContextMenuStrip).SourceControl as GridControl).MainView is GridView { DataRowCount: not 0 } gridView && gridView.GetFocusedRow() is DataRowView { Row: not null } dataRowView)
			{
				await CopyTask(dataRowView.Row);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void copyTaskAndToolStripMenuItem1_Click(object sender, EventArgs e)
	{
		try
		{
			if (!copyTask && (((sender as ToolStripMenuItem).Owner as ContextMenuStrip).SourceControl as GridControl).MainView is GridView { DataRowCount: not 0 } gridView && gridView.GetFocusedRow() is DataRowView { Row: not null } dataRowView)
			{
				await AdvancedCopyTask(dataRowView.Row);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void copyTaskToolStripMenuItem2_Click(object sender, EventArgs e)
	{
		try
		{
			if (!copyTask && (((sender as ToolStripMenuItem).Owner as ContextMenuStrip).SourceControl as GridControl).MainView is GridView { DataRowCount: not 0 } gridView && gridView.GetFocusedRow() is DataRowView { Row: not null } dataRowView)
			{
				await CopyTask(dataRowView.Row);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void copyTaskAndToolStripMenuItem2_Click(object sender, EventArgs e)
	{
		if (!copyTask && (((sender as ToolStripMenuItem).Owner as ContextMenuStrip).SourceControl as GridControl).MainView is GridView { DataRowCount: not 0 } gridView && gridView.GetFocusedRow() is DataRowView { Row: not null } dataRowView)
		{
			await AdvancedCopyTask(dataRowView.Row);
		}
	}

	public override void CHANGELOG()
	{
		try
		{
			string primaryKey = "";
			if (FormMode == FormModeType.SingleView || FormMode == FormModeType.SingleUpdate)
			{
				primaryKey = Function.ToString(DataRowFromCal["ClgCode"]);
				Enums.ChangeLog changeLogType = ChangeLogType;
				GetChangeLogParams getChangeLogParams = new GetChangeLogParams();
				getChangeLogParams.PrimaryKey = primaryKey;
				getChangeLogParams.Object = changeLogType;
				getChangeLogParams.IsApzTable = true;
				frm_ChangeLogList frm_ChangeLogList2 = new frm_ChangeLogList();
				frm_ChangeLogList2.GetChangeLogParams = getChangeLogParams;
				frm_ChangeLogList2.MdiParent = MdiParent;
				frm_ChangeLogList2.Show();
			}
			else if (FormMode == FormModeType.Normal)
			{
				if (_gridFocus == GridViewFocus.NotCompleted)
				{
					primaryKey = Function.ParseInt(gv_List.GetFocusedRowCellValue("ClgCode")).ToString();
				}
				else if (_gridFocus == GridViewFocus.Completed)
				{
					primaryKey = Function.ParseInt(gv_ListCompleted.GetFocusedRowCellValue("ClgCode")).ToString();
				}
				Enums.ChangeLog changeLogType2 = ChangeLogType;
				GetChangeLogParams getChangeLogParams2 = new GetChangeLogParams();
				getChangeLogParams2.PrimaryKey = primaryKey;
				getChangeLogParams2.Object = changeLogType2;
				getChangeLogParams2.IsApzTable = true;
				frm_ChangeLogList frm_ChangeLogList3 = new frm_ChangeLogList();
				frm_ChangeLogList3.GetChangeLogParams = getChangeLogParams2;
				frm_ChangeLogList3.MdiParent = MdiParent;
				frm_ChangeLogList3.Show();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.NoMatchingRecordFound, "");
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraGrid.GridFormatRule gridFormatRule = new DevExpress.XtraGrid.GridFormatRule();
		DevExpress.XtraEditors.FormatConditionRuleIconSet formatConditionRuleIconSet = new DevExpress.XtraEditors.FormatConditionRuleIconSet();
		DevExpress.XtraEditors.FormatConditionIconSet formatConditionIconSet = new DevExpress.XtraEditors.FormatConditionIconSet();
		DevExpress.XtraEditors.FormatConditionIconSetIcon formatConditionIconSetIcon = new DevExpress.XtraEditors.FormatConditionIconSetIcon();
		DevExpress.XtraEditors.FormatConditionIconSetIcon formatConditionIconSetIcon2 = new DevExpress.XtraEditors.FormatConditionIconSetIcon();
		DevExpress.XtraEditors.FormatConditionIconSetIcon formatConditionIconSetIcon3 = new DevExpress.XtraEditors.FormatConditionIconSetIcon();
		DevExpress.XtraEditors.FormatConditionIconSetIcon formatConditionIconSetIcon4 = new DevExpress.XtraEditors.FormatConditionIconSetIcon();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled4 = new DevExpress.Utils.SerializableAppearanceObject();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.SalesOppotunity.frm_TaskManagement));
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions4 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraGrid.GridFormatRule gridFormatRule2 = new DevExpress.XtraGrid.GridFormatRule();
		DevExpress.XtraEditors.FormatConditionRuleIconSet formatConditionRuleIconSet2 = new DevExpress.XtraEditors.FormatConditionRuleIconSet();
		DevExpress.XtraEditors.FormatConditionIconSet formatConditionIconSet2 = new DevExpress.XtraEditors.FormatConditionIconSet();
		DevExpress.XtraEditors.FormatConditionIconSetIcon formatConditionIconSetIcon5 = new DevExpress.XtraEditors.FormatConditionIconSetIcon();
		DevExpress.XtraEditors.FormatConditionIconSetIcon formatConditionIconSetIcon6 = new DevExpress.XtraEditors.FormatConditionIconSetIcon();
		DevExpress.XtraEditors.FormatConditionIconSetIcon formatConditionIconSetIcon7 = new DevExpress.XtraEditors.FormatConditionIconSetIcon();
		DevExpress.XtraEditors.FormatConditionIconSetIcon formatConditionIconSetIcon8 = new DevExpress.XtraEditors.FormatConditionIconSetIcon();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions5 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions4 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions6 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.BreadCrumbNode breadCrumbNode = new DevExpress.XtraEditors.BreadCrumbNode();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions5 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance11 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed11 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled11 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions7 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance12 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered11 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed12 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled12 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions6 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance13 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered12 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed13 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled13 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions8 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance14 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered13 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed14 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled14 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions7 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance15 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered14 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed15 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled15 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions8 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance16 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered15 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed16 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled16 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions9 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance17 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered16 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed17 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled17 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions9 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance18 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered17 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed18 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled18 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions10 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance19 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered18 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed19 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled19 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions10 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance20 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered19 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed20 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled20 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions11 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance21 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered20 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed21 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled21 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions12 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance22 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered21 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed22 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled22 = new DevExpress.Utils.SerializableAppearanceObject();
		this.gc_Priority = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Priority_Completed = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_memo_DetailsSub = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
		this.rep_bte_DeleteSubTask = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.rep_luk_Priority = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.repositoryItemButtonEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gc_DueDate = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Task = new Apzon.Controls.Grid.GridColumnEx();
		this.splitListTask = new DevExpress.XtraEditors.SplitContainerControl();
		this.xtraScrollableControl1 = new DevExpress.XtraEditors.XtraScrollableControl();
		this.ccb_GroupEmployee = new DevExpress.XtraEditors.CheckedComboBoxEdit();
		this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
		this.grd_List = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuList = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.ctm_JumpTo = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
		this.copyTaskToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.copyTaskAndToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
		this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_List = new Apzon.Controls.Grid.GridViewEx();
		this.gc_Code = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Closed = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_bte_Closed = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gc_ProjectEntry = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_cfl_ProjectEntry = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gc_ProjectWBS = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_cfl_ProjectWBS = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gcOrderLine = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_EndTime = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemHourEdit2 = new Apzon.Controls.Controls.RepositoryItemHourEdit();
		this.gc_CreateDate = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_AssignTo = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_CardCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx7 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx11 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemTextEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.grd_ListCompleted = new Apzon.Controls.Grid.GridControlEx();
		this.gv_ListCompleted = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx5 = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Details = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Closed_Completed = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_bte_Open = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gc_DueDate_Completed = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_ProjectEntry_Completed = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_PrjGrid = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gc_ProjectWBS_Completed = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx13 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx14 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemHourEdit1 = new Apzon.Controls.Controls.RepositoryItemHourEdit();
		this.gridColumnEx15 = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_AssignTo_Completed = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_CloseDate_Completed = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx4 = new Apzon.Controls.Grid.GridColumnEx();
		this.btnComplete = new Apzon.Controls.Controls.SimpleButtonEx();
		this.ccb_GroupUser = new DevExpress.XtraEditors.CheckedComboBoxEdit();
		this.btnDelete = new Apzon.Controls.Controls.SimpleButtonEx();
		this.labelControl8 = new DevExpress.XtraEditors.LabelControl();
		this.luk_Sort = new Apzon.Controls.Editor.MyLookUpEdit.LookUpEditSimple();
		this.tblLayoutInfo = new System.Windows.Forms.TableLayoutPanel();
		this.contextMenuInfoPanel = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.copyTaskToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.copyTaskAndToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
		this.removeToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.brc_ParentTask = new Apzon.Controls.Editor.BreadCrumbEditEx();
		this.btnOK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.splitMoreInformation = new DevExpress.XtraEditors.SplitContainerControl();
		this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
		this.luk_TaskMode = new Apzon.Controls.Editor.MyLookUpEdit.LookUpEditSimple();
		this.labelControl14 = new DevExpress.XtraEditors.LabelControl();
		this.labelControl13 = new DevExpress.XtraEditors.LabelControl();
		this.cfl_Task = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.txt_BPName = new Apzon.Controls.Editor.TextEditEx();
		this.luk_Priority = new Apzon.Controls.Editor.MyLookUpEdit.LookUpEditSimple();
		this.txt_BPType = new Apzon.Controls.Editor.TextEditEx();
		this.cfl_Opportunity = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.cfl_ProjectNew = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.labelControl9 = new DevExpress.XtraEditors.LabelControl();
		this.cfl_BPCode = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.ucLinkedDocument1 = new Apzon.Forms.UserControls.ucLinkedDocument();
		this.bte_DistrRule = new Apzon.Forms.UserControls.ButtonEditDistrRule();
		this.date_StartTime = new Apzon.Controls.Controls.HourEdit();
		this.labelControl6 = new DevExpress.XtraEditors.LabelControl();
		this.labelControl7 = new DevExpress.XtraEditors.LabelControl();
		this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
		this.date_StartDate = new Apzon.Controls.Editor.DateEditEx();
		this.labelControl11 = new DevExpress.XtraEditors.LabelControl();
		this.labelControl12 = new DevExpress.XtraEditors.LabelControl();
		this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
		this.labelControl10 = new DevExpress.XtraEditors.LabelControl();
		this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
		this.cfl_ProjectWBS = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.splitSubTask = new DevExpress.XtraEditors.SplitContainerControl();
		this.grd_ListSub = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStripSub = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.detailsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
		this.copyTaskToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
		this.copyTaskAndToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
		this.ctm_MainTaskAssigner = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
		this.toolStripMenuItem_DeleteCM = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_ListSub = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx8 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx9 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx10 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx16 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx17 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx18 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx19 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx20 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx21 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx22 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx23 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx24 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx25 = new Apzon.Controls.Grid.GridColumnEx();
		this.toolTipGridSub = new DevExpress.Utils.ToolTipController(this.components);
		this.splitAttachment = new DevExpress.XtraEditors.SplitContainerControl();
		this.ucAttachment1 = new Apzon.Forms.UserControls.ucAttachment();
		this.splitUDF = new DevExpress.XtraEditors.SplitContainerControl();
		this.layout_UDF = new Apzon.Forms.UserControls.LayoutUDF();
		this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.ucComment1 = new Apzon.Forms.UserControls.ucComment();
		this.lblCreatedAt = new DevExpress.XtraEditors.LabelControl();
		this.contextMenuStripBottomText = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.copyTaskIDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.txtDetails = new Apzon.Controls.Editor.TextBoxEx();
		this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
		this.txtOwner = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.lbOwner = new DevExpress.XtraEditors.LabelControl();
		this.date_DueTime = new Apzon.Controls.Controls.HourEdit();
		this.date_DueDate = new Apzon.Controls.Editor.DateEditEx();
		this.btnAttachment = new Apzon.Controls.Controls.SimpleButtonEx();
		this.labelControl5 = new DevExpress.XtraEditors.LabelControl();
		this.cfl_GroupUser = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.btnUDF = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnSubTask = new Apzon.Controls.Controls.SimpleButtonEx();
		this.cfl_GroupEmployee = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.btnDocument = new Apzon.Controls.Controls.SimpleButtonEx();
		this.cbUser = new Apzon.Controls.Controls.ComboBoxEditEx();
		this.txtClgCode = new Apzon.Controls.Editor.TextEditEx();
		this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
		this.memoNotes = new Apzon.Controls.Editor.TextBoxEx();
		this.chk_Closed = new Apzon.Controls.Editor.CheckEditEx();
		this.toolTipController1 = new DevExpress.Utils.ToolTipController(this.components);
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.panelFilter = new DevExpress.XtraEditors.PanelControl();
		this.accordionControl1 = new DevExpress.XtraBars.Navigation.AccordionControl();
		this.eleGeneral = new DevExpress.XtraBars.Navigation.AccordionControlElement();
		this.eleAll = new DevExpress.XtraBars.Navigation.AccordionControlElement();
		this.eleOverdue = new DevExpress.XtraBars.Navigation.AccordionControlElement();
		this.eleToday = new DevExpress.XtraBars.Navigation.AccordionControlElement();
		this.eleThisWeek = new DevExpress.XtraBars.Navigation.AccordionControlElement();
		this.eleLastWeek = new DevExpress.XtraBars.Navigation.AccordionControlElement();
		this.eleUser = new DevExpress.XtraBars.Navigation.AccordionControlElement();
		this.eleEmployee = new DevExpress.XtraBars.Navigation.AccordionControlElement();
		this.eleProjectNew = new DevExpress.XtraBars.Navigation.AccordionControlElement();
		this.splitAccordion = new DevExpress.XtraEditors.SplitContainerControl();
		this.gridColumnEx6 = new Apzon.Controls.Grid.GridColumnEx();
		((System.ComponentModel.ISupportInitialize)this.rep_memo_DetailsSub).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_bte_DeleteSubTask).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_luk_Priority).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemButtonEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitListTask).BeginInit();
		this.splitListTask.SuspendLayout();
		this.xtraScrollableControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.ccb_GroupEmployee.Properties).BeginInit();
		this.tableLayoutPanel1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_List).BeginInit();
		this.contextMenuList.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_List).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_bte_Closed).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_cfl_ProjectEntry).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_cfl_ProjectWBS).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemHourEdit2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_ListCompleted).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_ListCompleted).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_bte_Open).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_PrjGrid).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemHourEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ccb_GroupUser.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Sort.Properties).BeginInit();
		this.tblLayoutInfo.SuspendLayout();
		this.contextMenuInfoPanel.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.brc_ParentTask.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitMoreInformation).BeginInit();
		this.splitMoreInformation.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.panelControl1).BeginInit();
		this.panelControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.luk_TaskMode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cfl_Task.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BPName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Priority.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BPType.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cfl_Opportunity.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cfl_ProjectNew.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cfl_BPCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bte_DistrRule.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_StartTime.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_StartDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_StartDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cfl_ProjectWBS.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitSubTask).BeginInit();
		this.splitSubTask.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_ListSub).BeginInit();
		this.contextMenuStripSub.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_ListSub).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitAttachment).BeginInit();
		this.splitAttachment.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.splitUDF).BeginInit();
		this.splitUDF.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.layout_UDF).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).BeginInit();
		this.contextMenuStripBottomText.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.panelControl2).BeginInit();
		this.panelControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txtOwner.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_DueTime.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_DueDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_DueDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cfl_GroupUser.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cfl_GroupEmployee.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbUser.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtClgCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Closed.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.panelFilter).BeginInit();
		this.panelFilter.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.accordionControl1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitAccordion).BeginInit();
		this.splitAccordion.SuspendLayout();
		base.SuspendLayout();
		this.gc_Priority.AppearanceCell.Options.UseTextOptions = true;
		this.gc_Priority.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.gc_Priority.Caption = "Priority";
		this.gc_Priority.CurrencyDateField = null;
		this.gc_Priority.CurrencyField = null;
		this.gc_Priority.CurrencyFixed = null;
		this.gc_Priority.CurrencyFixedRate = 0.0;
		this.gc_Priority.CurrencyRateField = null;
		this.gc_Priority.FieldName = "Priority";
		this.gc_Priority.Name = "gc_Priority";
		this.gc_Priority.OptionsColumn.FixedWidth = true;
		this.gc_Priority.PrimaryKey = false;
		this.gc_Priority.UnitField = null;
		this.gc_Priority.Width = 24;
		this.gc_Priority_Completed.Caption = " ";
		this.gc_Priority_Completed.CurrencyDateField = null;
		this.gc_Priority_Completed.CurrencyField = null;
		this.gc_Priority_Completed.CurrencyFixed = null;
		this.gc_Priority_Completed.CurrencyFixedRate = 0.0;
		this.gc_Priority_Completed.CurrencyRateField = null;
		this.gc_Priority_Completed.FieldName = "Priority";
		this.gc_Priority_Completed.Name = "gc_Priority_Completed";
		this.gc_Priority_Completed.OptionsColumn.FixedWidth = true;
		this.gc_Priority_Completed.PrimaryKey = false;
		this.gc_Priority_Completed.UnitField = null;
		this.gc_Priority_Completed.Width = 24;
		this.rep_memo_DetailsSub.AcceptsReturn = false;
		this.rep_memo_DetailsSub.Name = "rep_memo_DetailsSub";
		this.rep_memo_DetailsSub.ScrollBars = System.Windows.Forms.ScrollBars.None;
		this.rep_memo_DetailsSub.ValidateOnEnterKey = true;
		this.rep_bte_DeleteSubTask.AutoHeight = false;
		serializableAppearanceObject.BackColor = System.Drawing.Color.White;
		serializableAppearanceObject.Options.UseBackColor = true;
		this.rep_bte_DeleteSubTask.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Clear, "", -1, true, true, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, serializableAppearanceObject, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_bte_DeleteSubTask.Name = "rep_bte_DeleteSubTask";
		this.rep_bte_DeleteSubTask.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.rep_luk_Priority.AutoHeight = false;
		this.rep_luk_Priority.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.rep_luk_Priority.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_luk_Priority.IsSearchControl = false;
		this.rep_luk_Priority.Name = "rep_luk_Priority";
		this.rep_luk_Priority.NullText = "";
		this.rep_luk_Priority.ShowFooter = false;
		this.rep_luk_Priority.ShowHeader = false;
		this.repositoryItemButtonEdit1.AutoHeight = false;
		this.repositoryItemButtonEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph)
		});
		this.repositoryItemButtonEdit1.Name = "repositoryItemButtonEdit1";
		this.gc_DueDate.AppearanceCell.Options.UseTextOptions = true;
		this.gc_DueDate.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.gc_DueDate.Caption = "Due Date";
		this.gc_DueDate.CurrencyDateField = null;
		this.gc_DueDate.CurrencyField = null;
		this.gc_DueDate.CurrencyFixed = null;
		this.gc_DueDate.CurrencyFixedRate = 0.0;
		this.gc_DueDate.CurrencyRateField = null;
		this.gc_DueDate.FieldName = "endDate";
		this.gc_DueDate.Name = "gc_DueDate";
		this.gc_DueDate.OptionsColumn.AllowEdit = false;
		this.gc_DueDate.OptionsColumn.AllowFocus = false;
		this.gc_DueDate.OptionsColumn.FixedWidth = true;
		this.gc_DueDate.OptionsColumn.ReadOnly = true;
		this.gc_DueDate.OptionsColumn.TabStop = false;
		this.gc_DueDate.PrimaryKey = false;
		this.gc_DueDate.UnitField = null;
		this.gc_DueDate.Visible = true;
		this.gc_DueDate.VisibleIndex = 5;
		this.gc_DueDate.Width = 60;
		this.gc_Task.Caption = "Task";
		this.gc_Task.CurrencyDateField = null;
		this.gc_Task.CurrencyField = null;
		this.gc_Task.CurrencyFixed = null;
		this.gc_Task.CurrencyFixedRate = 0.0;
		this.gc_Task.CurrencyRateField = null;
		this.gc_Task.FieldName = "Details";
		this.gc_Task.Name = "gc_Task";
		this.gc_Task.PrimaryKey = false;
		this.gc_Task.UnitField = null;
		this.gc_Task.Visible = true;
		this.gc_Task.VisibleIndex = 1;
		this.gc_Task.Width = 316;
		this.splitListTask.Appearance.ForeColor = System.Drawing.Color.FromArgb(38, 38, 38);
		this.splitListTask.Appearance.Options.UseForeColor = true;
		this.splitListTask.CollapsePanel = DevExpress.XtraEditors.SplitCollapsePanel.Panel2;
		this.splitListTask.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitListTask.FixedPanel = DevExpress.XtraEditors.SplitFixedPanel.Panel2;
		this.splitListTask.Location = new System.Drawing.Point(0, 0);
		this.splitListTask.LookAndFeel.SkinName = "Office 2016 Colorful";
		this.splitListTask.LookAndFeel.UseDefaultLookAndFeel = false;
		this.splitListTask.Name = "splitListTask";
		this.splitListTask.Panel1.AutoScroll = true;
		this.splitListTask.Panel1.Controls.Add(this.xtraScrollableControl1);
		this.splitListTask.Panel1.Controls.Add(this.labelControl8);
		this.splitListTask.Panel1.Controls.Add(this.luk_Sort);
		this.splitListTask.Panel1.Text = "Panel1";
		this.splitListTask.Panel2.AutoScroll = true;
		this.splitListTask.Panel2.Controls.Add(this.tblLayoutInfo);
		this.splitListTask.Panel2.Text = "Panel2";
		this.splitListTask.Size = new System.Drawing.Size(995, 753);
		this.splitListTask.SplitterPosition = 347;
		this.splitListTask.TabIndex = 17;
		this.splitListTask.Text = "splitContainerControl1";
		this.splitListTask.SplitGroupPanelCollapsing += new DevExpress.XtraEditors.SplitGroupPanelCollapsingEventHandler(this.splitContainerControl1_SplitGroupPanelCollapsing);
		this.xtraScrollableControl1.AlwaysScrollActiveControlIntoView = false;
		this.xtraScrollableControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.xtraScrollableControl1.Controls.Add(this.ccb_GroupEmployee);
		this.xtraScrollableControl1.Controls.Add(this.tableLayoutPanel1);
		this.xtraScrollableControl1.Controls.Add(this.ccb_GroupUser);
		this.xtraScrollableControl1.Controls.Add(this.btnDelete);
		this.xtraScrollableControl1.Location = new System.Drawing.Point(0, 26);
		this.xtraScrollableControl1.Name = "xtraScrollableControl1";
		this.xtraScrollableControl1.Size = new System.Drawing.Size(636, 731);
		this.xtraScrollableControl1.TabIndex = 19;
		this.ccb_GroupEmployee.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.ccb_GroupEmployee.Location = new System.Drawing.Point(107, 696);
		this.ccb_GroupEmployee.Name = "ccb_GroupEmployee";
		this.ccb_GroupEmployee.Properties.AutoHeight = false;
		this.ccb_GroupEmployee.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.ccb_GroupEmployee.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.ccb_GroupEmployee.Properties.IncrementalSearch = true;
		this.ccb_GroupEmployee.Properties.LookAndFeel.UseDefaultLookAndFeel = false;
		this.ccb_GroupEmployee.Properties.Mask.SaveLiteral = false;
		this.ccb_GroupEmployee.Properties.Mask.ShowPlaceHolders = false;
		this.ccb_GroupEmployee.Properties.Mask.UseMaskAsDisplayFormat = true;
		this.ccb_GroupEmployee.Properties.NullValuePrompt = "Assign To...";
		this.ccb_GroupEmployee.Size = new System.Drawing.Size(101, 21);
		this.ccb_GroupEmployee.TabIndex = 3;
		this.ccb_GroupEmployee.Visible = false;
		this.ccb_GroupEmployee.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ccb_GroupUser_KeyPress);
		this.tableLayoutPanel1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.tableLayoutPanel1.AutoSize = true;
		this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
		this.tableLayoutPanel1.ColumnCount = 1;
		this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100f));
		this.tableLayoutPanel1.Controls.Add(this.grd_List, 0, 0);
		this.tableLayoutPanel1.Controls.Add(this.grd_ListCompleted, 0, 2);
		this.tableLayoutPanel1.Controls.Add(this.btnComplete, 0, 1);
		this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
		this.tableLayoutPanel1.Name = "tableLayoutPanel1";
		this.tableLayoutPanel1.RowCount = 3;
		this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
		this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29f));
		this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
		this.tableLayoutPanel1.Size = new System.Drawing.Size(641, 522);
		this.tableLayoutPanel1.TabIndex = 0;
		this.grd_List.AllowDrop = true;
		this.grd_List.AllowPasteExcel = false;
		this.grd_List.ClipboardData = null;
		this.grd_List.ContextMenuStrip = this.contextMenuList;
		this.grd_List.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_List.Location = new System.Drawing.Point(3, 3);
		this.grd_List.LookAndFeel.UseDefaultLookAndFeel = false;
		this.grd_List.MainView = this.gv_List;
		this.grd_List.Name = "grd_List";
		this.grd_List.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[6] { this.rep_bte_Closed, this.rep_luk_Priority, this.repositoryItemTextEdit1, this.rep_cfl_ProjectEntry, this.rep_cfl_ProjectWBS, this.repositoryItemHourEdit2 });
		this.grd_List.RightToLeft = System.Windows.Forms.RightToLeft.No;
		this.grd_List.Size = new System.Drawing.Size(635, 50);
		this.grd_List.TabIndex = 16;
		this.grd_List.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_List });
		this.grd_List.ProcessGridKey += new System.Windows.Forms.KeyEventHandler(this.gridControl1_ProcessGridKey);
		this.grd_List.SizeChanged += new System.EventHandler(this.grd_List_SizeChanged);
		this.grd_List.DragDrop += new System.Windows.Forms.DragEventHandler(this.grd_List_DragDrop);
		this.grd_List.DragOver += new System.Windows.Forms.DragEventHandler(this.grd_List_DragOver);
		this.grd_List.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.grd_List_KeyPress);
		this.contextMenuList.Items.AddRange(new System.Windows.Forms.ToolStripItem[6] { this.ctm_JumpTo, this.toolStripSeparator2, this.copyTaskToolStripMenuItem1, this.copyTaskAndToolStripMenuItem1, this.toolStripSeparator1, this.removeToolStripMenuItem });
		this.contextMenuList.Name = "contextMenuStrip1";
		this.contextMenuList.Size = new System.Drawing.Size(162, 104);
		this.ctm_JumpTo.Name = "ctm_JumpTo";
		this.ctm_JumpTo.Size = new System.Drawing.Size(161, 22);
		this.ctm_JumpTo.Text = "Jump To";
		this.ctm_JumpTo.Click += new System.EventHandler(this.ctm_JumpTo_Click);
		this.toolStripSeparator2.Name = "toolStripSeparator2";
		this.toolStripSeparator2.Size = new System.Drawing.Size(158, 6);
		this.copyTaskToolStripMenuItem1.Name = "copyTaskToolStripMenuItem1";
		this.copyTaskToolStripMenuItem1.Size = new System.Drawing.Size(161, 22);
		this.copyTaskToolStripMenuItem1.Text = "Copy Task";
		this.copyTaskToolStripMenuItem1.Click += new System.EventHandler(this.copyTaskToolStripMenuItem1_Click);
		this.copyTaskAndToolStripMenuItem1.Name = "copyTaskAndToolStripMenuItem1";
		this.copyTaskAndToolStripMenuItem1.Size = new System.Drawing.Size(161, 22);
		this.copyTaskAndToolStripMenuItem1.Text = "Copy Task And...";
		this.copyTaskAndToolStripMenuItem1.Click += new System.EventHandler(this.copyTaskAndToolStripMenuItem1_Click);
		this.toolStripSeparator1.Name = "toolStripSeparator1";
		this.toolStripSeparator1.Size = new System.Drawing.Size(158, 6);
		this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
		this.removeToolStripMenuItem.ShortcutKeyDisplayString = "";
		this.removeToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
		this.removeToolStripMenuItem.Text = "Remove";
		this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
		this.gv_List.Appearance.Row.ForeColor = System.Drawing.Color.FromArgb(47, 46, 68);
		this.gv_List.Appearance.Row.Options.UseForeColor = true;
		this.gv_List.AutoFillColumn = this.gc_Task;
		this.gv_List.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.gv_List.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_List.ColumnKey = null;
		this.gv_List.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[14]
		{
			this.gc_Code, this.gc_Task, this.gc_Closed, this.gc_DueDate, this.gc_ProjectEntry, this.gc_ProjectWBS, this.gcOrderLine, this.gc_EndTime, this.gc_CreateDate, this.gc_AssignTo,
			this.gc_Priority, this.gc_CardCode, this.gridColumnEx7, this.gridColumnEx11
		});
		this.gv_List.DisableFormSetting = false;
		this.gv_List.DisplayException = false;
		this.gv_List.EditColumn = null;
		this.gv_List.FieldCurrency = null;
		this.gv_List.FieldRate = null;
		gridFormatRule.Column = this.gc_Priority;
		gridFormatRule.ColumnApplyTo = this.gc_Priority;
		gridFormatRule.Name = "Format0";
		formatConditionIconSet.CategoryName = "Shapes";
		formatConditionIconSetIcon.PredefinedName = "RedToBlack4_1.png";
		formatConditionIconSetIcon.Value = new decimal(new int[4] { 3, 0, 0, 0 });
		formatConditionIconSetIcon.ValueComparison = DevExpress.XtraEditors.FormatConditionComparisonType.GreaterOrEqual;
		formatConditionIconSetIcon2.PredefinedName = "TrafficLights23_2.png";
		formatConditionIconSetIcon2.Value = new decimal(new int[4] { 2, 0, 0, 0 });
		formatConditionIconSetIcon2.ValueComparison = DevExpress.XtraEditors.FormatConditionComparisonType.GreaterOrEqual;
		formatConditionIconSetIcon3.PredefinedName = "RedToBlack4_3.png";
		formatConditionIconSetIcon3.Value = new decimal(new int[4] { 1, 0, 0, 0 });
		formatConditionIconSetIcon3.ValueComparison = DevExpress.XtraEditors.FormatConditionComparisonType.GreaterOrEqual;
		formatConditionIconSetIcon4.PredefinedName = "TrafficLights23_1.png";
		formatConditionIconSetIcon4.ValueComparison = DevExpress.XtraEditors.FormatConditionComparisonType.GreaterOrEqual;
		formatConditionIconSet.Icons.Add(formatConditionIconSetIcon);
		formatConditionIconSet.Icons.Add(formatConditionIconSetIcon2);
		formatConditionIconSet.Icons.Add(formatConditionIconSetIcon3);
		formatConditionIconSet.Icons.Add(formatConditionIconSetIcon4);
		formatConditionIconSet.Name = "RedToBlack";
		formatConditionIconSet.ValueType = DevExpress.XtraEditors.FormatConditionValueType.Number;
		formatConditionRuleIconSet.IconSet = formatConditionIconSet;
		gridFormatRule.Rule = formatConditionRuleIconSet;
		this.gv_List.FormatRules.Add(gridFormatRule);
		this.gv_List.FormId = "oTaskManagement";
		this.gv_List.GridControl = this.grd_List;
		this.gv_List.IgnoreFilter = false;
		this.gv_List.IndexEditRow = -1;
		this.gv_List.IndicatorWidth = 35;
		this.gv_List.IsApzTable = false;
		this.gv_List.IsInsertNewRow = false;
		this.gv_List.IsLoadingData = false;
		this.gv_List.IsMappingContextAction = false;
		this.gv_List.ItemId = "OLOT";
		this.gv_List.LstColumnAllowBatch = null;
		this.gv_List.LstColumnNotAllowBatch = null;
		this.gv_List.LstColumnNotEditable = new string[1] { "ClgCode" };
		this.gv_List.LstColumNotSetting = null;
		this.gv_List.Name = "gv_List";
		this.gv_List.NewItemRowText = "Add Task";
		this.gv_List.OptionsView.BestFitMaxRowCount = 100;
		this.gv_List.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
		this.gv_List.OptionsView.ShowGroupPanel = false;
		this.gv_List.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.True;
		this.gv_List.OptionsView.ShowIndicator = false;
		this.gv_List.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.False;
		this.gv_List.UseBatchMod = false;
		this.gv_List.RowClick += new DevExpress.XtraGrid.Views.Grid.RowClickEventHandler(this.gv_List_RowClick);
		this.gv_List.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_List_RowCellStyle);
		this.gv_List.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gv_List_CustomRowCellEdit);
		this.gv_List.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_List_ShowingEditor);
		this.gv_List.InitNewRow += new DevExpress.XtraGrid.Views.Grid.InitNewRowEventHandler(this.gv_List_InitNewRow);
		this.gv_List.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gv_List_FocusedRowChanged);
		this.gv_List.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_List_FocusedRowObjectChanged);
		this.gv_List.FocusedColumnChanged += new DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventHandler(this.gv_List_FocusedColumnChanged);
		this.gv_List.BeforeLeaveRow += new DevExpress.XtraGrid.Views.Base.RowAllowEventHandler(this.gv_List_BeforeLeaveRow);
		this.gv_List.RowUpdated += new DevExpress.XtraGrid.Views.Base.RowObjectEventHandler(this.gv_List_RowUpdated);
		this.gv_List.CustomColumnSort += new DevExpress.XtraGrid.Views.Base.CustomColumnSortEventHandler(this.gv_List_CustomColumnSort);
		this.gv_List.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gv_List_CustomUnboundColumnData);
		this.gv_List.CustomRowFilter += new DevExpress.XtraGrid.Views.Base.RowFilterEventHandler(this.gv_List_CustomRowFilter);
		this.gv_List.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.gv_List_CustomColumnDisplayText);
		this.gv_List.KeyDown += new System.Windows.Forms.KeyEventHandler(this.gv_List_KeyDown);
		this.gv_List.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gridView1_MouseDown);
		this.gv_List.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gridView1_MouseMove);
		this.gv_List.DoubleClick += new System.EventHandler(this.gv_List_DoubleClick);
		this.gv_List.GotFocus += new System.EventHandler(this.gv_List_GotFocus);
		this.gv_List.RowCountChanged += new System.EventHandler(this.gv_List_RowCountChanged);
		this.gc_Code.Caption = "Code";
		this.gc_Code.CurrencyDateField = null;
		this.gc_Code.CurrencyField = null;
		this.gc_Code.CurrencyFixed = null;
		this.gc_Code.CurrencyFixedRate = 0.0;
		this.gc_Code.CurrencyRateField = null;
		this.gc_Code.FieldName = "ClgCode";
		this.gc_Code.Name = "gc_Code";
		this.gc_Code.PrimaryKey = false;
		this.gc_Code.UnitField = null;
		this.gc_Closed.AllowChSet = false;
		this.gc_Closed.Caption = " ";
		this.gc_Closed.ColumnEdit = this.rep_bte_Closed;
		this.gc_Closed.CurrencyDateField = null;
		this.gc_Closed.CurrencyField = null;
		this.gc_Closed.CurrencyFixed = null;
		this.gc_Closed.CurrencyFixedRate = 0.0;
		this.gc_Closed.CurrencyRateField = null;
		this.gc_Closed.FieldName = "Closed";
		this.gc_Closed.Name = "gc_Closed";
		this.gc_Closed.OptionsColumn.AllowSize = false;
		this.gc_Closed.OptionsColumn.FixedWidth = true;
		this.gc_Closed.OptionsColumn.TabStop = false;
		this.gc_Closed.PrimaryKey = false;
		this.gc_Closed.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gc_Closed.UnitField = null;
		this.gc_Closed.Visible = true;
		this.gc_Closed.VisibleIndex = 0;
		this.gc_Closed.Width = 24;
		this.rep_bte_Closed.AutoHeight = false;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.completed;
		this.rep_bte_Closed.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered, appearancePressed2, appearanceDisabled2, "Mark as Not Completed", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_bte_Closed.Name = "rep_bte_Closed";
		this.rep_bte_Closed.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
		this.rep_bte_Closed.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_bte_Closed_ButtonPressed);
		this.gc_ProjectEntry.AppearanceCell.Options.UseTextOptions = true;
		this.gc_ProjectEntry.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.gc_ProjectEntry.Caption = "Project Entry";
		this.gc_ProjectEntry.ColumnEdit = this.rep_cfl_ProjectEntry;
		this.gc_ProjectEntry.CurrencyDateField = null;
		this.gc_ProjectEntry.CurrencyField = null;
		this.gc_ProjectEntry.CurrencyFixed = null;
		this.gc_ProjectEntry.CurrencyFixedRate = 0.0;
		this.gc_ProjectEntry.CurrencyRateField = null;
		this.gc_ProjectEntry.FieldName = "ProjectEntry";
		this.gc_ProjectEntry.Name = "gc_ProjectEntry";
		this.gc_ProjectEntry.OptionsColumn.FixedWidth = true;
		this.gc_ProjectEntry.PrimaryKey = false;
		this.gc_ProjectEntry.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gc_ProjectEntry.UnitField = null;
		this.gc_ProjectEntry.Visible = true;
		this.gc_ProjectEntry.VisibleIndex = 2;
		this.rep_cfl_ProjectEntry.AutoHeight = false;
		this.rep_cfl_ProjectEntry.AutoLoadDataSource = true;
		this.rep_cfl_ProjectEntry.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered2, appearancePressed3, appearanceDisabled3, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, imageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance4, appearanceHovered3, appearancePressed4, appearanceDisabled4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_cfl_ProjectEntry.ContainerControl = this;
		this.rep_cfl_ProjectEntry.DisplayMember = "";
		this.rep_cfl_ProjectEntry.DisplayMember2 = "";
		this.rep_cfl_ProjectEntry.LoadDataSourceWhenEnter = false;
		this.rep_cfl_ProjectEntry.Name = "rep_cfl_ProjectEntry";
		this.rep_cfl_ProjectEntry.SeparateCharacterForMulti = ';';
		this.rep_cfl_ProjectEntry.SeparateString = ", ";
		this.rep_cfl_ProjectEntry.UsingAutoCompleteBox = false;
		this.rep_cfl_ProjectEntry.UsingDataSource = true;
		this.rep_cfl_ProjectEntry.ValidateWhenNoDataSource = false;
		this.rep_cfl_ProjectEntry.ValueMember = "";
		this.gc_ProjectWBS.AppearanceCell.Image = (System.Drawing.Image)resources.GetObject("gc_ProjectWBS.AppearanceCell.Image");
		this.gc_ProjectWBS.AppearanceCell.Options.UseTextOptions = true;
		this.gc_ProjectWBS.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.gc_ProjectWBS.Caption = "PrjWBS";
		this.gc_ProjectWBS.ColumnEdit = this.rep_cfl_ProjectWBS;
		this.gc_ProjectWBS.CurrencyDateField = null;
		this.gc_ProjectWBS.CurrencyField = null;
		this.gc_ProjectWBS.CurrencyFixed = null;
		this.gc_ProjectWBS.CurrencyFixedRate = 0.0;
		this.gc_ProjectWBS.CurrencyRateField = null;
		this.gc_ProjectWBS.FieldName = "ProjectWBS";
		this.gc_ProjectWBS.Name = "gc_ProjectWBS";
		this.gc_ProjectWBS.OptionsColumn.FixedWidth = true;
		this.gc_ProjectWBS.PrimaryKey = false;
		this.gc_ProjectWBS.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gc_ProjectWBS.UnitField = null;
		this.gc_ProjectWBS.Visible = true;
		this.gc_ProjectWBS.VisibleIndex = 3;
		this.gc_ProjectWBS.Width = 70;
		this.rep_cfl_ProjectWBS.AutoHeight = false;
		this.rep_cfl_ProjectWBS.AutoLoadDataSource = true;
		editorButtonImageOptions2.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions6.Image");
		this.rep_cfl_ProjectWBS.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance5, appearanceHovered4, appearancePressed5, appearanceDisabled5, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance6, appearanceHovered5, appearancePressed6, appearanceDisabled6, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_cfl_ProjectWBS.ContainerControl = this;
		this.rep_cfl_ProjectWBS.DisplayMember = "";
		this.rep_cfl_ProjectWBS.DisplayMember2 = "";
		this.rep_cfl_ProjectWBS.LoadDataSourceWhenEnter = false;
		this.rep_cfl_ProjectWBS.Name = "rep_cfl_ProjectWBS";
		this.rep_cfl_ProjectWBS.SeparateCharacterForMulti = ';';
		this.rep_cfl_ProjectWBS.SeparateString = ", ";
		this.rep_cfl_ProjectWBS.UsingAutoCompleteBox = false;
		this.rep_cfl_ProjectWBS.UsingDataSource = true;
		this.rep_cfl_ProjectWBS.ValidateWhenNoDataSource = false;
		this.rep_cfl_ProjectWBS.ValueMember = "";
		this.gcOrderLine.Caption = "OrderLine";
		this.gcOrderLine.CurrencyDateField = null;
		this.gcOrderLine.CurrencyField = null;
		this.gcOrderLine.CurrencyFixed = null;
		this.gcOrderLine.CurrencyFixedRate = 0.0;
		this.gcOrderLine.CurrencyRateField = null;
		this.gcOrderLine.FieldName = "OrderLine";
		this.gcOrderLine.Name = "gcOrderLine";
		this.gcOrderLine.PrimaryKey = false;
		this.gcOrderLine.UnitField = null;
		this.gc_EndTime.Caption = "endTime";
		this.gc_EndTime.ColumnEdit = this.repositoryItemHourEdit2;
		this.gc_EndTime.CurrencyDateField = null;
		this.gc_EndTime.CurrencyField = null;
		this.gc_EndTime.CurrencyFixed = null;
		this.gc_EndTime.CurrencyFixedRate = 0.0;
		this.gc_EndTime.CurrencyRateField = null;
		this.gc_EndTime.FieldName = "ENDTime";
		this.gc_EndTime.Name = "gc_EndTime";
		this.gc_EndTime.PrimaryKey = false;
		this.gc_EndTime.UnitField = null;
		this.repositoryItemHourEdit2.AutoHeight = false;
		this.repositoryItemHourEdit2.IsSearchControl = false;
		this.repositoryItemHourEdit2.MaxLength = 5;
		this.repositoryItemHourEdit2.Name = "repositoryItemHourEdit2";
		this.gc_CreateDate.Caption = "Create Date";
		this.gc_CreateDate.CurrencyDateField = null;
		this.gc_CreateDate.CurrencyField = null;
		this.gc_CreateDate.CurrencyFixed = null;
		this.gc_CreateDate.CurrencyFixedRate = 0.0;
		this.gc_CreateDate.CurrencyRateField = null;
		this.gc_CreateDate.FieldName = "CntctDate";
		this.gc_CreateDate.Name = "gc_CreateDate";
		this.gc_CreateDate.PrimaryKey = false;
		this.gc_CreateDate.UnitField = null;
		this.gc_AssignTo.Caption = "AssignTo";
		this.gc_AssignTo.CurrencyDateField = null;
		this.gc_AssignTo.CurrencyField = null;
		this.gc_AssignTo.CurrencyFixed = null;
		this.gc_AssignTo.CurrencyFixedRate = 0.0;
		this.gc_AssignTo.CurrencyRateField = null;
		this.gc_AssignTo.FieldName = "AssignTo";
		this.gc_AssignTo.Name = "gc_AssignTo";
		this.gc_AssignTo.OptionsColumn.FixedWidth = true;
		this.gc_AssignTo.PrimaryKey = false;
		this.gc_AssignTo.UnboundType = DevExpress.Data.UnboundColumnType.String;
		this.gc_AssignTo.UnitField = null;
		this.gc_AssignTo.Visible = true;
		this.gc_AssignTo.VisibleIndex = 4;
		this.gc_AssignTo.Width = 120;
		this.gc_CardCode.Caption = "BP Code";
		this.gc_CardCode.CurrencyDateField = null;
		this.gc_CardCode.CurrencyField = null;
		this.gc_CardCode.CurrencyFixed = null;
		this.gc_CardCode.CurrencyFixedRate = 0.0;
		this.gc_CardCode.CurrencyRateField = null;
		this.gc_CardCode.FieldName = "CardCode";
		this.gc_CardCode.Name = "gc_CardCode";
		this.gc_CardCode.OptionsColumn.FixedWidth = true;
		this.gc_CardCode.PrimaryKey = false;
		this.gc_CardCode.UnitField = null;
		this.gc_CardCode.Width = 70;
		this.gridColumnEx7.Caption = "Begin Time";
		this.gridColumnEx7.ColumnEdit = this.repositoryItemHourEdit2;
		this.gridColumnEx7.CurrencyDateField = null;
		this.gridColumnEx7.CurrencyField = null;
		this.gridColumnEx7.CurrencyFixed = null;
		this.gridColumnEx7.CurrencyFixedRate = 0.0;
		this.gridColumnEx7.CurrencyRateField = null;
		this.gridColumnEx7.FieldName = "BeginTime";
		this.gridColumnEx7.Name = "gridColumnEx7";
		this.gridColumnEx7.PrimaryKey = false;
		this.gridColumnEx7.UnitField = null;
		this.gridColumnEx11.Caption = "Create Time";
		this.gridColumnEx11.ColumnEdit = this.repositoryItemHourEdit2;
		this.gridColumnEx11.CurrencyDateField = null;
		this.gridColumnEx11.CurrencyField = null;
		this.gridColumnEx11.CurrencyFixed = null;
		this.gridColumnEx11.CurrencyFixedRate = 0.0;
		this.gridColumnEx11.CurrencyRateField = null;
		this.gridColumnEx11.FieldName = "CntctTime";
		this.gridColumnEx11.Name = "gridColumnEx11";
		this.gridColumnEx11.PrimaryKey = false;
		this.gridColumnEx11.UnitField = null;
		this.repositoryItemTextEdit1.AutoHeight = false;
		this.repositoryItemTextEdit1.Name = "repositoryItemTextEdit1";
		this.grd_ListCompleted.AllowPasteExcel = true;
		this.grd_ListCompleted.ClipboardData = null;
		this.grd_ListCompleted.ContextMenuStrip = this.contextMenuList;
		this.grd_ListCompleted.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_ListCompleted.Location = new System.Drawing.Point(3, 88);
		this.grd_ListCompleted.LookAndFeel.UseDefaultLookAndFeel = false;
		this.grd_ListCompleted.MainView = this.gv_ListCompleted;
		this.grd_ListCompleted.Name = "grd_ListCompleted";
		this.grd_ListCompleted.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.rep_bte_Open, this.repositoryItemHourEdit1, this.btn_PrjGrid });
		this.grd_ListCompleted.Size = new System.Drawing.Size(635, 431);
		this.grd_ListCompleted.TabIndex = 18;
		this.grd_ListCompleted.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_ListCompleted });
		this.grd_ListCompleted.SizeChanged += new System.EventHandler(this.grd_ListCompleted_SizeChanged);
		this.gv_ListCompleted.Appearance.FocusedRow.BackColor = System.Drawing.Color.DarkSeaGreen;
		this.gv_ListCompleted.Appearance.FocusedRow.BorderColor = System.Drawing.Color.DarkSeaGreen;
		this.gv_ListCompleted.Appearance.Row.ForeColor = System.Drawing.Color.FromArgb(47, 46, 68);
		this.gv_ListCompleted.Appearance.Row.Options.UseForeColor = true;
		this.gv_ListCompleted.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.gv_ListCompleted.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_ListCompleted.ColumnKey = null;
		this.gv_ListCompleted.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[14]
		{
			this.gridColumnEx5, this.gc_Details, this.gc_Closed_Completed, this.gc_DueDate_Completed, this.gc_ProjectEntry_Completed, this.gc_ProjectWBS_Completed, this.gridColumnEx13, this.gridColumnEx14, this.gridColumnEx15, this.gc_AssignTo_Completed,
			this.gc_CloseDate_Completed, this.gc_Priority_Completed, this.gridColumnEx1, this.gridColumnEx4
		});
		this.gv_ListCompleted.DisableFormSetting = false;
		this.gv_ListCompleted.DisplayException = false;
		this.gv_ListCompleted.EditColumn = null;
		this.gv_ListCompleted.FieldCurrency = null;
		this.gv_ListCompleted.FieldRate = null;
		gridFormatRule2.Column = this.gc_Priority_Completed;
		gridFormatRule2.ColumnApplyTo = this.gc_Priority_Completed;
		gridFormatRule2.Name = "Format0";
		formatConditionIconSet2.CategoryName = "Shapes";
		formatConditionIconSetIcon5.PredefinedName = "RedToBlack4_1.png";
		formatConditionIconSetIcon5.Value = new decimal(new int[4] { 3, 0, 0, 0 });
		formatConditionIconSetIcon5.ValueComparison = DevExpress.XtraEditors.FormatConditionComparisonType.GreaterOrEqual;
		formatConditionIconSetIcon6.PredefinedName = "TrafficLights23_2.png";
		formatConditionIconSetIcon6.Value = new decimal(new int[4] { 2, 0, 0, 0 });
		formatConditionIconSetIcon6.ValueComparison = DevExpress.XtraEditors.FormatConditionComparisonType.GreaterOrEqual;
		formatConditionIconSetIcon7.PredefinedName = "RedToBlack4_3.png";
		formatConditionIconSetIcon7.Value = new decimal(new int[4] { 1, 0, 0, 0 });
		formatConditionIconSetIcon7.ValueComparison = DevExpress.XtraEditors.FormatConditionComparisonType.GreaterOrEqual;
		formatConditionIconSetIcon8.PredefinedName = "Signs3_1.png";
		formatConditionIconSetIcon8.ValueComparison = DevExpress.XtraEditors.FormatConditionComparisonType.GreaterOrEqual;
		formatConditionIconSet2.Icons.Add(formatConditionIconSetIcon5);
		formatConditionIconSet2.Icons.Add(formatConditionIconSetIcon6);
		formatConditionIconSet2.Icons.Add(formatConditionIconSetIcon7);
		formatConditionIconSet2.Icons.Add(formatConditionIconSetIcon8);
		formatConditionIconSet2.Name = "RedToBlack";
		formatConditionRuleIconSet2.IconSet = formatConditionIconSet2;
		gridFormatRule2.Rule = formatConditionRuleIconSet2;
		this.gv_ListCompleted.FormatRules.Add(gridFormatRule2);
		this.gv_ListCompleted.FormId = "oTaskManagement";
		this.gv_ListCompleted.GridControl = this.grd_ListCompleted;
		this.gv_ListCompleted.IgnoreFilter = false;
		this.gv_ListCompleted.IndexEditRow = -1;
		this.gv_ListCompleted.IndicatorWidth = 35;
		this.gv_ListCompleted.IsApzTable = false;
		this.gv_ListCompleted.IsInsertNewRow = false;
		this.gv_ListCompleted.IsLoadingData = false;
		this.gv_ListCompleted.IsMappingContextAction = false;
		this.gv_ListCompleted.ItemId = "OLCT";
		this.gv_ListCompleted.LstColumnAllowBatch = null;
		this.gv_ListCompleted.LstColumnNotAllowBatch = null;
		this.gv_ListCompleted.LstColumnNotEditable = new string[1] { "ClgCode" };
		this.gv_ListCompleted.LstColumNotSetting = null;
		this.gv_ListCompleted.Name = "gv_ListCompleted";
		this.gv_ListCompleted.NewItemRowText = "New Task";
		this.gv_ListCompleted.OptionsView.BestFitMaxRowCount = 100;
		this.gv_ListCompleted.OptionsView.ShowColumnHeaders = false;
		this.gv_ListCompleted.OptionsView.ShowGroupPanel = false;
		this.gv_ListCompleted.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.True;
		this.gv_ListCompleted.OptionsView.ShowIndicator = false;
		this.gv_ListCompleted.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.False;
		this.gv_ListCompleted.UseBatchMod = false;
		this.gv_ListCompleted.RowClick += new DevExpress.XtraGrid.Views.Grid.RowClickEventHandler(this.gv_List_RowClick);
		this.gv_ListCompleted.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_List_RowCellStyle);
		this.gv_ListCompleted.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gv_List_CustomRowCellEdit);
		this.gv_ListCompleted.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_List_ShowingEditor);
		this.gv_ListCompleted.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gv_List_FocusedRowChanged);
		this.gv_ListCompleted.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_List_FocusedRowObjectChanged);
		this.gv_ListCompleted.BeforeLeaveRow += new DevExpress.XtraGrid.Views.Base.RowAllowEventHandler(this.gv_List_BeforeLeaveRow);
		this.gv_ListCompleted.CustomColumnSort += new DevExpress.XtraGrid.Views.Base.CustomColumnSortEventHandler(this.gv_List_CustomColumnSort);
		this.gv_ListCompleted.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gv_List_CustomUnboundColumnData);
		this.gv_ListCompleted.CustomRowFilter += new DevExpress.XtraGrid.Views.Base.RowFilterEventHandler(this.gv_ListCompleted_CustomRowFilter);
		this.gv_ListCompleted.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.gv_List_CustomColumnDisplayText);
		this.gv_ListCompleted.KeyDown += new System.Windows.Forms.KeyEventHandler(this.gv_List_KeyDown);
		this.gv_ListCompleted.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gv_ListCompleted_MouseDown);
		this.gv_ListCompleted.DoubleClick += new System.EventHandler(this.gv_List_DoubleClick);
		this.gv_ListCompleted.GotFocus += new System.EventHandler(this.gv_ListCompleted_GotFocus);
		this.gv_ListCompleted.RowCountChanged += new System.EventHandler(this.gv_ListCompleted_RowCountChanged);
		this.gridColumnEx5.Caption = "Code";
		this.gridColumnEx5.CurrencyDateField = null;
		this.gridColumnEx5.CurrencyField = null;
		this.gridColumnEx5.CurrencyFixed = null;
		this.gridColumnEx5.CurrencyFixedRate = 0.0;
		this.gridColumnEx5.CurrencyRateField = null;
		this.gridColumnEx5.FieldName = "ClgCode";
		this.gridColumnEx5.Name = "gridColumnEx5";
		this.gridColumnEx5.PrimaryKey = true;
		this.gridColumnEx5.UnitField = null;
		this.gc_Details.Caption = "Task";
		this.gc_Details.CurrencyDateField = null;
		this.gc_Details.CurrencyField = null;
		this.gc_Details.CurrencyFixed = null;
		this.gc_Details.CurrencyFixedRate = 0.0;
		this.gc_Details.CurrencyRateField = null;
		this.gc_Details.FieldName = "Details";
		this.gc_Details.Name = "gc_Details";
		this.gc_Details.PrimaryKey = false;
		this.gc_Details.UnitField = null;
		this.gc_Details.Visible = true;
		this.gc_Details.VisibleIndex = 1;
		this.gc_Details.Width = 237;
		this.gc_Closed_Completed.Caption = " Closed";
		this.gc_Closed_Completed.ColumnEdit = this.rep_bte_Open;
		this.gc_Closed_Completed.CurrencyDateField = null;
		this.gc_Closed_Completed.CurrencyField = null;
		this.gc_Closed_Completed.CurrencyFixed = null;
		this.gc_Closed_Completed.CurrencyFixedRate = 0.0;
		this.gc_Closed_Completed.CurrencyRateField = null;
		this.gc_Closed_Completed.FieldName = "Closed";
		this.gc_Closed_Completed.Name = "gc_Closed_Completed";
		this.gc_Closed_Completed.OptionsColumn.AllowSize = false;
		this.gc_Closed_Completed.OptionsColumn.FixedWidth = true;
		this.gc_Closed_Completed.OptionsColumn.TabStop = false;
		this.gc_Closed_Completed.PrimaryKey = false;
		this.gc_Closed_Completed.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gc_Closed_Completed.UnitField = null;
		this.gc_Closed_Completed.Visible = true;
		this.gc_Closed_Completed.VisibleIndex = 0;
		this.gc_Closed_Completed.Width = 24;
		this.rep_bte_Open.AutoHeight = false;
		editorButtonImageOptions3.Image = Apzon.Forms.Properties.Resources.incompleted;
		this.rep_bte_Open.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, editorButtonImageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance7, appearanceHovered6, appearancePressed7, appearanceDisabled7, "Marked as Completed", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_bte_Open.Name = "rep_bte_Open";
		this.rep_bte_Open.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
		this.rep_bte_Open.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_bte_Open_ButtonPressed);
		this.gc_DueDate_Completed.AppearanceCell.Options.UseTextOptions = true;
		this.gc_DueDate_Completed.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.gc_DueDate_Completed.Caption = "Due Date";
		this.gc_DueDate_Completed.CurrencyDateField = null;
		this.gc_DueDate_Completed.CurrencyField = null;
		this.gc_DueDate_Completed.CurrencyFixed = null;
		this.gc_DueDate_Completed.CurrencyFixedRate = 0.0;
		this.gc_DueDate_Completed.CurrencyRateField = null;
		this.gc_DueDate_Completed.FieldName = "endDate";
		this.gc_DueDate_Completed.Name = "gc_DueDate_Completed";
		this.gc_DueDate_Completed.OptionsColumn.AllowEdit = false;
		this.gc_DueDate_Completed.OptionsColumn.AllowFocus = false;
		this.gc_DueDate_Completed.OptionsColumn.FixedWidth = true;
		this.gc_DueDate_Completed.OptionsColumn.ReadOnly = true;
		this.gc_DueDate_Completed.OptionsColumn.TabStop = false;
		this.gc_DueDate_Completed.PrimaryKey = false;
		this.gc_DueDate_Completed.UnitField = null;
		this.gc_DueDate_Completed.Visible = true;
		this.gc_DueDate_Completed.VisibleIndex = 5;
		this.gc_DueDate_Completed.Width = 60;
		this.gc_ProjectEntry_Completed.AppearanceCell.Options.UseTextOptions = true;
		this.gc_ProjectEntry_Completed.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.gc_ProjectEntry_Completed.Caption = "Project Entry";
		this.gc_ProjectEntry_Completed.ColumnEdit = this.btn_PrjGrid;
		this.gc_ProjectEntry_Completed.CurrencyDateField = null;
		this.gc_ProjectEntry_Completed.CurrencyField = null;
		this.gc_ProjectEntry_Completed.CurrencyFixed = null;
		this.gc_ProjectEntry_Completed.CurrencyFixedRate = 0.0;
		this.gc_ProjectEntry_Completed.CurrencyRateField = null;
		this.gc_ProjectEntry_Completed.FieldName = "ProjectEntry";
		this.gc_ProjectEntry_Completed.Name = "gc_ProjectEntry_Completed";
		this.gc_ProjectEntry_Completed.OptionsColumn.FixedWidth = true;
		this.gc_ProjectEntry_Completed.PrimaryKey = false;
		this.gc_ProjectEntry_Completed.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gc_ProjectEntry_Completed.UnitField = null;
		this.gc_ProjectEntry_Completed.Visible = true;
		this.gc_ProjectEntry_Completed.VisibleIndex = 2;
		this.btn_PrjGrid.AutoHeight = false;
		this.btn_PrjGrid.AutoLoadDataSource = true;
		editorButtonImageOptions4.Image = Apzon.Forms.Properties.Resources.right;
		this.btn_PrjGrid.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions5, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance8, appearanceHovered7, appearancePressed8, appearanceDisabled8, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance9, appearanceHovered8, appearancePressed9, appearanceDisabled9, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_PrjGrid.ContainerControl = this;
		this.btn_PrjGrid.DisplayMember = "";
		this.btn_PrjGrid.DisplayMember2 = "";
		this.btn_PrjGrid.LoadDataSourceWhenEnter = false;
		this.btn_PrjGrid.Name = "btn_PrjGrid";
		this.btn_PrjGrid.SeparateCharacterForMulti = ';';
		this.btn_PrjGrid.SeparateString = ", ";
		this.btn_PrjGrid.UsingAutoCompleteBox = false;
		this.btn_PrjGrid.UsingDataSource = true;
		this.btn_PrjGrid.ValidateWhenNoDataSource = false;
		this.btn_PrjGrid.ValueMember = "";
		this.gc_ProjectWBS_Completed.AppearanceCell.Options.UseTextOptions = true;
		this.gc_ProjectWBS_Completed.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.gc_ProjectWBS_Completed.Caption = "PrjCode";
		this.gc_ProjectWBS_Completed.CurrencyDateField = null;
		this.gc_ProjectWBS_Completed.CurrencyField = null;
		this.gc_ProjectWBS_Completed.CurrencyFixed = null;
		this.gc_ProjectWBS_Completed.CurrencyFixedRate = 0.0;
		this.gc_ProjectWBS_Completed.CurrencyRateField = null;
		this.gc_ProjectWBS_Completed.FieldName = "ProjectWBS";
		this.gc_ProjectWBS_Completed.Name = "gc_ProjectWBS_Completed";
		this.gc_ProjectWBS_Completed.OptionsColumn.FixedWidth = true;
		this.gc_ProjectWBS_Completed.PrimaryKey = false;
		this.gc_ProjectWBS_Completed.UnitField = null;
		this.gc_ProjectWBS_Completed.Visible = true;
		this.gc_ProjectWBS_Completed.VisibleIndex = 3;
		this.gc_ProjectWBS_Completed.Width = 63;
		this.gridColumnEx13.Caption = "OrderLine";
		this.gridColumnEx13.CurrencyDateField = null;
		this.gridColumnEx13.CurrencyField = null;
		this.gridColumnEx13.CurrencyFixed = null;
		this.gridColumnEx13.CurrencyFixedRate = 0.0;
		this.gridColumnEx13.CurrencyRateField = null;
		this.gridColumnEx13.FieldName = "OrderLine";
		this.gridColumnEx13.Name = "gridColumnEx13";
		this.gridColumnEx13.PrimaryKey = false;
		this.gridColumnEx13.UnitField = null;
		this.gridColumnEx14.Caption = "End Time";
		this.gridColumnEx14.ColumnEdit = this.repositoryItemHourEdit1;
		this.gridColumnEx14.CurrencyDateField = null;
		this.gridColumnEx14.CurrencyField = null;
		this.gridColumnEx14.CurrencyFixed = null;
		this.gridColumnEx14.CurrencyFixedRate = 0.0;
		this.gridColumnEx14.CurrencyRateField = null;
		this.gridColumnEx14.FieldName = "ENDTime";
		this.gridColumnEx14.Name = "gridColumnEx14";
		this.gridColumnEx14.PrimaryKey = false;
		this.gridColumnEx14.UnitField = null;
		this.repositoryItemHourEdit1.AutoHeight = false;
		this.repositoryItemHourEdit1.IsSearchControl = false;
		this.repositoryItemHourEdit1.MaxLength = 5;
		this.repositoryItemHourEdit1.Name = "repositoryItemHourEdit1";
		this.gridColumnEx15.Caption = "Create Date";
		this.gridColumnEx15.CurrencyDateField = null;
		this.gridColumnEx15.CurrencyField = null;
		this.gridColumnEx15.CurrencyFixed = null;
		this.gridColumnEx15.CurrencyFixedRate = 0.0;
		this.gridColumnEx15.CurrencyRateField = null;
		this.gridColumnEx15.FieldName = "CntctDate";
		this.gridColumnEx15.Name = "gridColumnEx15";
		this.gridColumnEx15.PrimaryKey = false;
		this.gridColumnEx15.UnitField = null;
		this.gc_AssignTo_Completed.Caption = "AssignTo";
		this.gc_AssignTo_Completed.CurrencyDateField = null;
		this.gc_AssignTo_Completed.CurrencyField = null;
		this.gc_AssignTo_Completed.CurrencyFixed = null;
		this.gc_AssignTo_Completed.CurrencyFixedRate = 0.0;
		this.gc_AssignTo_Completed.CurrencyRateField = null;
		this.gc_AssignTo_Completed.FieldName = "AssignTo";
		this.gc_AssignTo_Completed.Name = "gc_AssignTo_Completed";
		this.gc_AssignTo_Completed.OptionsColumn.FixedWidth = true;
		this.gc_AssignTo_Completed.PrimaryKey = false;
		this.gc_AssignTo_Completed.UnboundType = DevExpress.Data.UnboundColumnType.String;
		this.gc_AssignTo_Completed.UnitField = null;
		this.gc_AssignTo_Completed.Visible = true;
		this.gc_AssignTo_Completed.VisibleIndex = 4;
		this.gc_AssignTo_Completed.Width = 120;
		this.gc_CloseDate_Completed.Caption = "gridColumnEx27";
		this.gc_CloseDate_Completed.CurrencyDateField = null;
		this.gc_CloseDate_Completed.CurrencyField = null;
		this.gc_CloseDate_Completed.CurrencyFixed = null;
		this.gc_CloseDate_Completed.CurrencyFixedRate = 0.0;
		this.gc_CloseDate_Completed.CurrencyRateField = null;
		this.gc_CloseDate_Completed.FieldName = "CloseDate";
		this.gc_CloseDate_Completed.Name = "gc_CloseDate_Completed";
		this.gc_CloseDate_Completed.PrimaryKey = false;
		this.gc_CloseDate_Completed.UnitField = null;
		this.gridColumnEx1.Caption = "Begin Time";
		this.gridColumnEx1.ColumnEdit = this.repositoryItemHourEdit1;
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "BeginTime";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.PrimaryKey = false;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx4.Caption = "Create Time";
		this.gridColumnEx4.ColumnEdit = this.repositoryItemHourEdit1;
		this.gridColumnEx4.CurrencyDateField = null;
		this.gridColumnEx4.CurrencyField = null;
		this.gridColumnEx4.CurrencyFixed = null;
		this.gridColumnEx4.CurrencyFixedRate = 0.0;
		this.gridColumnEx4.CurrencyRateField = null;
		this.gridColumnEx4.FieldName = "CntctTime";
		this.gridColumnEx4.Name = "gridColumnEx4";
		this.gridColumnEx4.PrimaryKey = false;
		this.gridColumnEx4.UnitField = null;
		this.btnComplete.Appearance.ForeColor = System.Drawing.Color.DarkGray;
		this.btnComplete.Appearance.Options.UseForeColor = true;
		this.btnComplete.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btnComplete.EditControl = null;
		this.btnComplete.EditValue = "Show Completed Tasks";
		this.btnComplete.FieldName = null;
		this.btnComplete.IsSearchControl = false;
		this.btnComplete.Location = new System.Drawing.Point(3, 59);
		this.btnComplete.Name = "btnComplete";
		this.btnComplete.ReadOnly = false;
		this.btnComplete.SettingInVisible = false;
		this.btnComplete.SettingReadonly = false;
		this.btnComplete.Size = new System.Drawing.Size(121, 23);
		this.btnComplete.TabIndex = 17;
		this.btnComplete.Text = "Show Completed Tasks";
		this.btnComplete.Click += new System.EventHandler(this.btnComplete_Click);
		this.ccb_GroupUser.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.ccb_GroupUser.Location = new System.Drawing.Point(-1, 696);
		this.ccb_GroupUser.Name = "ccb_GroupUser";
		this.ccb_GroupUser.Properties.AutoHeight = false;
		this.ccb_GroupUser.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.ccb_GroupUser.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.ccb_GroupUser.Properties.IncrementalSearch = true;
		this.ccb_GroupUser.Properties.LookAndFeel.UseDefaultLookAndFeel = false;
		this.ccb_GroupUser.Properties.Mask.SaveLiteral = false;
		this.ccb_GroupUser.Properties.Mask.ShowPlaceHolders = false;
		this.ccb_GroupUser.Properties.Mask.UseMaskAsDisplayFormat = true;
		this.ccb_GroupUser.Properties.NullValuePrompt = "Assign To...";
		this.ccb_GroupUser.Size = new System.Drawing.Size(102, 21);
		this.ccb_GroupUser.TabIndex = 2;
		this.ccb_GroupUser.Visible = false;
		this.ccb_GroupUser.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ccb_GroupUser_KeyPress);
		this.ccb_GroupUser.Validating += new System.ComponentModel.CancelEventHandler(this.Control_Validating);
		this.ccb_GroupUser.Validated += new System.EventHandler(this.Control_Validated);
		this.btnDelete.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
		this.btnDelete.EditControl = null;
		this.btnDelete.EditValue = "";
		this.btnDelete.FieldName = null;
		this.btnDelete.ImageOptions.Image = Apzon.Forms.Properties.Resources.round_delete_button;
		this.btnDelete.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
		this.btnDelete.IsSearchControl = false;
		this.btnDelete.Location = new System.Drawing.Point(214, 696);
		this.btnDelete.Name = "btnDelete";
		this.btnDelete.ReadOnly = false;
		this.btnDelete.SettingInVisible = false;
		this.btnDelete.SettingReadonly = false;
		this.btnDelete.Size = new System.Drawing.Size(21, 20);
		this.btnDelete.TabIndex = 61;
		this.btnDelete.TabStop = false;
		this.btnDelete.ToolTip = "Remove task";
		this.btnDelete.Visible = false;
		this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
		this.labelControl8.Appearance.Font = new System.Drawing.Font("Tahoma", 7.25f);
		this.labelControl8.Appearance.ForeColor = System.Drawing.Color.DarkGray;
		this.labelControl8.Appearance.Options.UseFont = true;
		this.labelControl8.Appearance.Options.UseForeColor = true;
		this.labelControl8.Location = new System.Drawing.Point(8, 7);
		this.labelControl8.Name = "labelControl8";
		this.labelControl8.Size = new System.Drawing.Size(19, 12);
		this.labelControl8.TabIndex = 18;
		this.labelControl8.Text = "Sort";
		this.luk_Sort.EditControl = null;
		this.luk_Sort.FieldName = null;
		this.luk_Sort.IsSearchControl = false;
		this.luk_Sort.Location = new System.Drawing.Point(33, 5);
		this.luk_Sort.Name = "luk_Sort";
		this.luk_Sort.Properties.AutoHeight = false;
		this.luk_Sort.Properties.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_Sort.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.luk_Sort.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Sort.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code")
		});
		this.luk_Sort.Properties.IsSearchControl = false;
		this.luk_Sort.Properties.LookAndFeel.UseDefaultLookAndFeel = false;
		this.luk_Sort.Properties.NullText = "";
		this.luk_Sort.Properties.ShowFooter = false;
		this.luk_Sort.Properties.ShowHeader = false;
		this.luk_Sort.SettingInVisible = false;
		this.luk_Sort.SettingReadonly = false;
		this.luk_Sort.Size = new System.Drawing.Size(113, 17);
		this.luk_Sort.TabIndex = 17;
		this.luk_Sort.EditValueChanged += new System.EventHandler(this.lookUpEditSimple1_EditValueChanged);
		this.tblLayoutInfo.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.tblLayoutInfo.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
		this.tblLayoutInfo.ColumnCount = 3;
		this.tblLayoutInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100f));
		this.tblLayoutInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 27f));
		this.tblLayoutInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 27f));
		this.tblLayoutInfo.ContextMenuStrip = this.contextMenuInfoPanel;
		this.tblLayoutInfo.Controls.Add(this.brc_ParentTask, 0, 0);
		this.tblLayoutInfo.Controls.Add(this.btnOK, 0, 5);
		this.tblLayoutInfo.Controls.Add(this.splitMoreInformation, 0, 4);
		this.tblLayoutInfo.Controls.Add(this.txtDetails, 0, 1);
		this.tblLayoutInfo.Controls.Add(this.panelControl2, 0, 3);
		this.tblLayoutInfo.Controls.Add(this.memoNotes, 0, 2);
		this.tblLayoutInfo.Controls.Add(this.chk_Closed, 2, 1);
		this.tblLayoutInfo.Location = new System.Drawing.Point(0, 3);
		this.tblLayoutInfo.Name = "tblLayoutInfo";
		this.tblLayoutInfo.RowCount = 6;
		this.tblLayoutInfo.RowStyles.Add(new System.Windows.Forms.RowStyle());
		this.tblLayoutInfo.RowStyles.Add(new System.Windows.Forms.RowStyle());
		this.tblLayoutInfo.RowStyles.Add(new System.Windows.Forms.RowStyle());
		this.tblLayoutInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 87f));
		this.tblLayoutInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100f));
		this.tblLayoutInfo.RowStyles.Add(new System.Windows.Forms.RowStyle());
		this.tblLayoutInfo.Size = new System.Drawing.Size(347, 736);
		this.tblLayoutInfo.TabIndex = 0;
		this.contextMenuInfoPanel.Items.AddRange(new System.Windows.Forms.ToolStripItem[4] { this.copyTaskToolStripMenuItem, this.copyTaskAndToolStripMenuItem, this.toolStripSeparator5, this.removeToolStripMenuItem1 });
		this.contextMenuInfoPanel.Name = "contextMenuInfoPanel";
		this.contextMenuInfoPanel.Size = new System.Drawing.Size(162, 76);
		this.copyTaskToolStripMenuItem.Name = "copyTaskToolStripMenuItem";
		this.copyTaskToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
		this.copyTaskToolStripMenuItem.Text = "Copy Task";
		this.copyTaskToolStripMenuItem.Click += new System.EventHandler(this.copyTaskToolStripMenuItem_Click);
		this.copyTaskAndToolStripMenuItem.Name = "copyTaskAndToolStripMenuItem";
		this.copyTaskAndToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
		this.copyTaskAndToolStripMenuItem.Text = "Copy Task And...";
		this.copyTaskAndToolStripMenuItem.Click += new System.EventHandler(this.copyTaskAndToolStripMenuItem_Click);
		this.toolStripSeparator5.Name = "toolStripSeparator5";
		this.toolStripSeparator5.Size = new System.Drawing.Size(158, 6);
		this.removeToolStripMenuItem1.Name = "removeToolStripMenuItem1";
		this.removeToolStripMenuItem1.Size = new System.Drawing.Size(161, 22);
		this.removeToolStripMenuItem1.Text = "Remove";
		this.removeToolStripMenuItem1.Click += new System.EventHandler(this.removeToolStripMenuItem1_Click);
		this.brc_ParentTask.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.tblLayoutInfo.SetColumnSpan(this.brc_ParentTask, 3);
		this.brc_ParentTask.EditControl = null;
		this.brc_ParentTask.FieldName = "ParentTask";
		this.brc_ParentTask.IsSearchControl = false;
		this.brc_ParentTask.Location = new System.Drawing.Point(3, 3);
		this.brc_ParentTask.Name = "brc_ParentTask";
		this.brc_ParentTask.Properties.Appearance.BackColor = System.Drawing.Color.White;
		this.brc_ParentTask.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 7.25f);
		this.brc_ParentTask.Properties.Appearance.ForeColor = System.Drawing.Color.FromArgb(32, 31, 52);
		this.brc_ParentTask.Properties.Appearance.Options.UseBackColor = true;
		this.brc_ParentTask.Properties.Appearance.Options.UseFont = true;
		this.brc_ParentTask.Properties.Appearance.Options.UseForeColor = true;
		this.brc_ParentTask.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.brc_ParentTask.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo, "", -1, true, false, false, imageOptions6, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance10, appearanceHovered9, appearancePressed10, appearanceDisabled10, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.brc_ParentTask.Properties.ButtonsStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.brc_ParentTask.Properties.LookAndFeel.UseDefaultLookAndFeel = false;
		breadCrumbNode.Caption = "Root";
		breadCrumbNode.Value = "Root";
		this.brc_ParentTask.Properties.Nodes.AddRange(new DevExpress.XtraEditors.BreadCrumbNode[1] { breadCrumbNode });
		this.brc_ParentTask.Properties.NodeClick += new DevExpress.XtraEditors.BreadCrumbNodeClickEventHandler(this.breadCrumbEdit1_Properties_NodeClick);
		this.brc_ParentTask.Properties.SelectedNodeChanged += new DevExpress.XtraEditors.BreadCrumbSelectedNodeChangedEventHandler(this.brc_ParentTask_Properties_SelectedNodeChanged);
		this.brc_ParentTask.Properties.QueryChildNodes += new DevExpress.XtraEditors.BreadCrumbQueryChildNodesEventHandler(this.brc_ParentTask_Properties_QueryChildNodes);
		this.brc_ParentTask.Properties.ValidatePath += new DevExpress.XtraEditors.BreadCrumbValidatePathEventHandler(this.breadCrumbEdit1_Properties_ValidatePath);
		this.brc_ParentTask.Properties.NewNodeAdding += new DevExpress.XtraEditors.BreadCrumbNewNodeAddingEventHandler(this.breadCrumbEdit1_Properties_NewNodeAdding);
		this.brc_ParentTask.Properties.PropertiesChanged += new System.EventHandler(this.breadCrumbEdit1_Properties_PropertiesChanged);
		this.brc_ParentTask.SelectedNode = breadCrumbNode;
		this.brc_ParentTask.SettingInVisible = false;
		this.brc_ParentTask.SettingReadonly = false;
		this.brc_ParentTask.Size = new System.Drawing.Size(341, 18);
		this.brc_ParentTask.TabIndex = 0;
		this.brc_ParentTask.TabStop = false;
		this.brc_ParentTask.Tag = "APZ_OCLG, parentTask";
		this.btnOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnOK.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btnOK.EditControl = null;
		this.btnOK.EditValue = "OK";
		this.btnOK.FieldName = null;
		this.btnOK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnOK.ImageOptions.Image");
		this.btnOK.IsSearchControl = false;
		this.btnOK.Location = new System.Drawing.Point(3, 714);
		this.btnOK.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
		this.btnOK.Name = "btnOK";
		this.btnOK.ReadOnly = false;
		this.btnOK.SettingInVisible = false;
		this.btnOK.SettingReadonly = false;
		this.btnOK.Size = new System.Drawing.Size(75, 22);
		this.btnOK.TabIndex = 63;
		this.btnOK.Text = "OK";
		this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
		this.splitMoreInformation.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitMoreInformation.CollapsePanel = DevExpress.XtraEditors.SplitCollapsePanel.Panel1;
		this.tblLayoutInfo.SetColumnSpan(this.splitMoreInformation, 3);
		this.splitMoreInformation.Horizontal = false;
		this.splitMoreInformation.IsSplitterFixed = true;
		this.splitMoreInformation.Location = new System.Drawing.Point(3, 187);
		this.splitMoreInformation.LookAndFeel.SkinName = "Office 2016 Colorful";
		this.splitMoreInformation.LookAndFeel.UseDefaultLookAndFeel = false;
		this.splitMoreInformation.Name = "splitMoreInformation";
		this.splitMoreInformation.Panel1.Controls.Add(this.panelControl1);
		this.splitMoreInformation.Panel1.Text = "Panel1";
		this.splitMoreInformation.Panel2.AutoScroll = true;
		this.splitMoreInformation.Panel2.Controls.Add(this.splitSubTask);
		this.splitMoreInformation.Panel2.Text = "Panel2";
		this.splitMoreInformation.Size = new System.Drawing.Size(341, 524);
		this.splitMoreInformation.SplitterPosition = 190;
		this.splitMoreInformation.TabIndex = 73;
		this.splitMoreInformation.Text = "splitContainerControl4";
		this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.panelControl1.Controls.Add(this.luk_TaskMode);
		this.panelControl1.Controls.Add(this.labelControl14);
		this.panelControl1.Controls.Add(this.labelControl13);
		this.panelControl1.Controls.Add(this.cfl_Task);
		this.panelControl1.Controls.Add(this.txt_BPName);
		this.panelControl1.Controls.Add(this.luk_Priority);
		this.panelControl1.Controls.Add(this.txt_BPType);
		this.panelControl1.Controls.Add(this.cfl_Opportunity);
		this.panelControl1.Controls.Add(this.cfl_ProjectNew);
		this.panelControl1.Controls.Add(this.labelControl9);
		this.panelControl1.Controls.Add(this.cfl_BPCode);
		this.panelControl1.Controls.Add(this.ucLinkedDocument1);
		this.panelControl1.Controls.Add(this.bte_DistrRule);
		this.panelControl1.Controls.Add(this.date_StartTime);
		this.panelControl1.Controls.Add(this.labelControl6);
		this.panelControl1.Controls.Add(this.labelControl7);
		this.panelControl1.Controls.Add(this.labelControl3);
		this.panelControl1.Controls.Add(this.date_StartDate);
		this.panelControl1.Controls.Add(this.labelControl11);
		this.panelControl1.Controls.Add(this.labelControl12);
		this.panelControl1.Controls.Add(this.labelControl2);
		this.panelControl1.Controls.Add(this.labelControl10);
		this.panelControl1.Controls.Add(this.labelControl1);
		this.panelControl1.Controls.Add(this.cfl_ProjectWBS);
		this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panelControl1.Location = new System.Drawing.Point(0, 0);
		this.panelControl1.LookAndFeel.UseDefaultLookAndFeel = false;
		this.panelControl1.Name = "panelControl1";
		this.panelControl1.Size = new System.Drawing.Size(341, 190);
		this.panelControl1.TabIndex = 5;
		this.panelControl1.Paint += new System.Windows.Forms.PaintEventHandler(this.panelControl1_Paint);
		this.luk_TaskMode.EditControl = null;
		this.luk_TaskMode.FieldName = "TaskMode";
		this.luk_TaskMode.IsSearchControl = false;
		this.luk_TaskMode.Location = new System.Drawing.Point(25, 157);
		this.luk_TaskMode.Name = "luk_TaskMode";
		this.luk_TaskMode.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.luk_TaskMode.Properties.Appearance.Options.UseFont = true;
		this.luk_TaskMode.Properties.AutoHeight = false;
		this.luk_TaskMode.Properties.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_TaskMode.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.luk_TaskMode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_TaskMode.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.luk_TaskMode.Properties.DisplayMember = "Name";
		this.luk_TaskMode.Properties.IsSearchControl = false;
		this.luk_TaskMode.Properties.NullText = "";
		this.luk_TaskMode.Properties.NullValuePrompt = "Task mode";
		this.luk_TaskMode.Properties.ShowFooter = false;
		this.luk_TaskMode.Properties.ShowHeader = false;
		this.luk_TaskMode.Properties.ValueMember = "Code";
		this.luk_TaskMode.SettingInVisible = false;
		this.luk_TaskMode.SettingReadonly = false;
		this.luk_TaskMode.Size = new System.Drawing.Size(134, 18);
		this.luk_TaskMode.TabIndex = 82;
		this.labelControl14.Appearance.Image = (System.Drawing.Image)resources.GetObject("labelControl14.Appearance.Image");
		this.labelControl14.Appearance.Options.UseImage = true;
		this.labelControl14.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("labelControl14.ImageOptions.Image");
		this.labelControl14.Location = new System.Drawing.Point(4, 158);
		this.labelControl14.Name = "labelControl14";
		this.labelControl14.Size = new System.Drawing.Size(16, 16);
		this.labelControl14.TabIndex = 83;
		this.labelControl14.ToolTip = "Task mode";
		this.labelControl13.Appearance.Image = (System.Drawing.Image)resources.GetObject("labelControl13.Appearance.Image");
		this.labelControl13.Appearance.Options.UseImage = true;
		this.labelControl13.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("labelControl13.ImageOptions.Image");
		this.labelControl13.Location = new System.Drawing.Point(4, 136);
		this.labelControl13.Name = "labelControl13";
		this.labelControl13.Size = new System.Drawing.Size(16, 16);
		this.labelControl13.TabIndex = 81;
		this.labelControl13.ToolTip = "Predecessors";
		this.cfl_Task.EditControl = null;
		this.cfl_Task.FieldName = "Predecessors";
		this.cfl_Task.IsSearchControl = false;
		this.cfl_Task.Location = new System.Drawing.Point(25, 135);
		this.cfl_Task.Name = "cfl_Task";
		this.cfl_Task.Properties.AutoHeight = false;
		this.cfl_Task.Properties.AutoLoadDataSource = true;
		this.cfl_Task.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		editorButtonImageOptions5.Image = Apzon.Forms.Properties.Resources.right;
		this.cfl_Task.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, false, true, editorButtonImageOptions5, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance11, appearanceHovered10, appearancePressed11, appearanceDisabled11, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.cfl_Task.Properties.ContainerControl = null;
		this.cfl_Task.Properties.DisplayMember = "ClgCode";
		this.cfl_Task.Properties.DisplayMember2 = "";
		this.cfl_Task.Properties.LoadDataSourceWhenEnter = false;
		this.cfl_Task.Properties.LookAndFeel.UseDefaultLookAndFeel = false;
		this.cfl_Task.Properties.MultiSelectMode = true;
		this.cfl_Task.Properties.SeparateCharacterForMulti = ';';
		this.cfl_Task.Properties.SeparateString = ", ";
		this.cfl_Task.Properties.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.PreDecisionTask;
		this.cfl_Task.Properties.TypeDrillDown = Apzon.Commons.ApzObjectTypes.oTaskManagement;
		this.cfl_Task.Properties.UsingAutoCompleteBox = false;
		this.cfl_Task.Properties.UsingDataSource = true;
		this.cfl_Task.Properties.ValidateWhenNoDataSource = false;
		this.cfl_Task.Properties.ValueMember = "ClgCode";
		this.cfl_Task.SettingInVisible = false;
		this.cfl_Task.SettingReadonly = false;
		this.cfl_Task.Size = new System.Drawing.Size(134, 18);
		this.cfl_Task.TabIndex = 80;
		this.txt_BPName.EditControl = null;
		this.txt_BPName.FieldName = "BPName";
		this.txt_BPName.IsSearchControl = false;
		this.txt_BPName.Location = new System.Drawing.Point(197, 136);
		this.txt_BPName.Name = "txt_BPName";
		this.txt_BPName.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.txt_BPName.Properties.ReadOnly = true;
		this.txt_BPName.SettingInVisible = false;
		this.txt_BPName.SettingReadonly = false;
		this.txt_BPName.Size = new System.Drawing.Size(141, 18);
		this.txt_BPName.TabIndex = 79;
		this.txt_BPName.TabStop = false;
		this.luk_Priority.EditControl = null;
		this.luk_Priority.FieldName = "Priority";
		this.luk_Priority.IsSearchControl = false;
		this.luk_Priority.Location = new System.Drawing.Point(25, 111);
		this.luk_Priority.Name = "luk_Priority";
		this.luk_Priority.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.luk_Priority.Properties.Appearance.Options.UseFont = true;
		this.luk_Priority.Properties.AutoHeight = false;
		this.luk_Priority.Properties.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_Priority.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.luk_Priority.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Priority.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.luk_Priority.Properties.DisplayMember = "Name";
		this.luk_Priority.Properties.IsSearchControl = false;
		this.luk_Priority.Properties.NullText = "";
		this.luk_Priority.Properties.NullValuePrompt = "Priority...";
		this.luk_Priority.Properties.ShowFooter = false;
		this.luk_Priority.Properties.ShowHeader = false;
		this.luk_Priority.Properties.ValueMember = "Code";
		this.luk_Priority.SettingInVisible = false;
		this.luk_Priority.SettingReadonly = false;
		this.luk_Priority.Size = new System.Drawing.Size(134, 18);
		this.luk_Priority.TabIndex = 5;
		this.luk_Priority.Validating += new System.ComponentModel.CancelEventHandler(this.Control_Validating);
		this.luk_Priority.Validated += new System.EventHandler(this.Control_Validated);
		this.txt_BPType.EditControl = null;
		this.txt_BPType.FieldName = null;
		this.txt_BPType.IsSearchControl = false;
		this.txt_BPType.Location = new System.Drawing.Point(291, 111);
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
		this.txt_BPType.Size = new System.Drawing.Size(47, 16);
		this.txt_BPType.TabIndex = 78;
		this.txt_BPType.TabStop = false;
		this.cfl_Opportunity.EditControl = null;
		this.cfl_Opportunity.FieldName = "Opportunity";
		this.cfl_Opportunity.IsSearchControl = false;
		this.cfl_Opportunity.Location = new System.Drawing.Point(197, 84);
		this.cfl_Opportunity.Name = "cfl_Opportunity";
		this.cfl_Opportunity.Properties.AutoLoadDataSource = true;
		this.cfl_Opportunity.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		editorButtonImageOptions6.Image = Apzon.Forms.Properties.Resources.right;
		this.cfl_Opportunity.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions7, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance12, appearanceHovered11, appearancePressed12, appearanceDisabled12, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions6, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance13, appearanceHovered12, appearancePressed13, appearanceDisabled13, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.cfl_Opportunity.Properties.ContainerControl = null;
		this.cfl_Opportunity.Properties.DisplayMember = "Code";
		this.cfl_Opportunity.Properties.DisplayMember2 = "";
		this.cfl_Opportunity.Properties.LoadDataSourceWhenEnter = false;
		this.cfl_Opportunity.Properties.NullValuePrompt = "Opportunity...";
		this.cfl_Opportunity.Properties.SeparateCharacterForMulti = ';';
		this.cfl_Opportunity.Properties.SeparateString = ", ";
		this.cfl_Opportunity.Properties.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.Opportunity;
		this.cfl_Opportunity.Properties.TypeDrillDown = Apzon.Commons.ApzObjectTypes.oSalesOpportunities;
		this.cfl_Opportunity.Properties.UsingAutoCompleteBox = false;
		this.cfl_Opportunity.Properties.UsingDataSource = true;
		this.cfl_Opportunity.Properties.ValidateWhenNoDataSource = false;
		this.cfl_Opportunity.Properties.ValueMember = "AbsEntry";
		this.cfl_Opportunity.SettingInVisible = false;
		this.cfl_Opportunity.SettingReadonly = false;
		this.cfl_Opportunity.Size = new System.Drawing.Size(141, 18);
		this.cfl_Opportunity.TabIndex = 2;
		this.cfl_Opportunity.TabStop = false;
		this.cfl_Opportunity.Validating += new System.ComponentModel.CancelEventHandler(this.Control_Validating);
		this.cfl_Opportunity.Validated += new System.EventHandler(this.Control_Validated);
		this.cfl_ProjectNew.EditControl = null;
		this.cfl_ProjectNew.FieldName = "ProjectNew";
		this.cfl_ProjectNew.IsSearchControl = false;
		this.cfl_ProjectNew.Location = new System.Drawing.Point(25, 31);
		this.cfl_ProjectNew.Name = "cfl_ProjectNew";
		this.cfl_ProjectNew.Properties.AutoLoadDataSource = true;
		this.cfl_ProjectNew.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		editorButtonImageOptions7.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions15.Image");
		this.cfl_ProjectNew.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions8, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance14, appearanceHovered13, appearancePressed14, appearanceDisabled14, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions7, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance15, appearanceHovered14, appearancePressed15, appearanceDisabled15, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.cfl_ProjectNew.Properties.ContainerControl = this;
		this.cfl_ProjectNew.Properties.DisplayMember = "Code";
		this.cfl_ProjectNew.Properties.DisplayMember2 = "";
		this.cfl_ProjectNew.Properties.LoadDataSourceWhenEnter = false;
		this.cfl_ProjectNew.Properties.NullValuePrompt = "Project...";
		this.cfl_ProjectNew.Properties.SeparateCharacterForMulti = ';';
		this.cfl_ProjectNew.Properties.SeparateString = ", ";
		this.cfl_ProjectNew.Properties.UsingAutoCompleteBox = false;
		this.cfl_ProjectNew.Properties.UsingDataSource = true;
		this.cfl_ProjectNew.Properties.ValidateWhenNoDataSource = false;
		this.cfl_ProjectNew.Properties.ValueMember = "AbsEntry";
		this.cfl_ProjectNew.SettingInVisible = false;
		this.cfl_ProjectNew.SettingReadonly = false;
		this.cfl_ProjectNew.Size = new System.Drawing.Size(134, 18);
		this.cfl_ProjectNew.TabIndex = 2;
		this.cfl_ProjectNew.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(this.cfl_ProjectNew_CustomDisplayText);
		this.cfl_ProjectNew.Validating += new System.ComponentModel.CancelEventHandler(this.cfl_ProjectNew_Validating);
		this.cfl_ProjectNew.Validated += new System.EventHandler(this.Control_Validated);
		this.labelControl9.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("labelControl9.ImageOptions.Image");
		this.labelControl9.Location = new System.Drawing.Point(177, 110);
		this.labelControl9.Name = "labelControl9";
		this.labelControl9.Size = new System.Drawing.Size(16, 16);
		this.labelControl9.TabIndex = 77;
		this.labelControl9.ToolTip = "Business Partner";
		this.cfl_BPCode.EditControl = null;
		this.cfl_BPCode.FieldName = "BPCode";
		this.cfl_BPCode.IsSearchControl = false;
		this.cfl_BPCode.Location = new System.Drawing.Point(197, 110);
		this.cfl_BPCode.Name = "cfl_BPCode";
		this.cfl_BPCode.Properties.AutoHeight = false;
		this.cfl_BPCode.Properties.AutoLoadDataSource = true;
		this.cfl_BPCode.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		editorButtonImageOptions8.Image = Apzon.Forms.Properties.Resources.right;
		this.cfl_BPCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions8, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance16, appearanceHovered15, appearancePressed16, appearanceDisabled16, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.cfl_BPCode.Properties.ContainerControl = null;
		this.cfl_BPCode.Properties.DisplayMember = "CardCode";
		this.cfl_BPCode.Properties.DisplayMember2 = "";
		this.cfl_BPCode.Properties.LoadDataSourceWhenEnter = false;
		this.cfl_BPCode.Properties.LookAndFeel.UseDefaultLookAndFeel = false;
		this.cfl_BPCode.Properties.NullValuePrompt = "Partner...";
		this.cfl_BPCode.Properties.SeparateCharacterForMulti = ';';
		this.cfl_BPCode.Properties.SeparateString = ", ";
		this.cfl_BPCode.Properties.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.Bp;
		this.cfl_BPCode.Properties.TypeDrillDown = Apzon.Commons.ApzObjectTypes.oBusinessPartners;
		this.cfl_BPCode.Properties.UsingAutoCompleteBox = false;
		this.cfl_BPCode.Properties.UsingDataSource = true;
		this.cfl_BPCode.Properties.ValidateWhenNoDataSource = false;
		this.cfl_BPCode.Properties.ValueMember = "CardCode";
		this.cfl_BPCode.SettingInVisible = false;
		this.cfl_BPCode.SettingReadonly = false;
		this.cfl_BPCode.Size = new System.Drawing.Size(88, 18);
		this.cfl_BPCode.TabIndex = 7;
		this.cfl_BPCode.EditValueChanged += new System.EventHandler(this.cfl_BPCode_EditValueChanged);
		this.cfl_BPCode.Validating += new System.ComponentModel.CancelEventHandler(this.Control_Validating);
		this.cfl_BPCode.Validated += new System.EventHandler(this.Control_Validated);
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
		this.ucLinkedDocument1.Location = new System.Drawing.Point(196, 7);
		this.ucLinkedDocument1.LookAndFeel.UseDefaultLookAndFeel = false;
		this.ucLinkedDocument1.Name = "ucLinkedDocument1";
		this.ucLinkedDocument1.Size = new System.Drawing.Size(141, 75);
		this.ucLinkedDocument1.TabIndex = 6;
		this.ucLinkedDocument1.ValidateInfo = null;
		this.bte_DistrRule.EditControl = null;
		this.bte_DistrRule.FieldName = "DistrRule";
		this.bte_DistrRule.IsSearchControl = false;
		this.bte_DistrRule.Location = new System.Drawing.Point(25, 85);
		this.bte_DistrRule.Name = "bte_DistrRule";
		this.bte_DistrRule.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.bte_DistrRule.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "Search", -1, true, true, false, imageOptions9, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance17, appearanceHovered16, appearancePressed17, appearanceDisabled17, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.bte_DistrRule.Properties.LookAndFeel.UseDefaultLookAndFeel = false;
		this.bte_DistrRule.Properties.NullValuePrompt = "Distribution Rule...";
		this.bte_DistrRule.SettingInVisible = false;
		this.bte_DistrRule.SettingReadonly = false;
		this.bte_DistrRule.Size = new System.Drawing.Size(134, 18);
		this.bte_DistrRule.TabIndex = 4;
		this.bte_DistrRule.Tag = "APZ_OCLG, OcrCode";
		this.bte_DistrRule.Validating += new System.ComponentModel.CancelEventHandler(this.bte_DistrRule_Validating);
		this.bte_DistrRule.Validated += new System.EventHandler(this.Control_Validated);
		this.date_StartTime.EditControl = null;
		this.date_StartTime.FieldName = "StartTime";
		this.date_StartTime.IsSearchControl = false;
		this.date_StartTime.Location = new System.Drawing.Point(118, 2);
		this.date_StartTime.Name = "date_StartTime";
		this.date_StartTime.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.date_StartTime.Properties.IsSearchControl = false;
		this.date_StartTime.Properties.LookAndFeel.UseDefaultLookAndFeel = false;
		this.date_StartTime.Properties.MaxLength = 5;
		this.date_StartTime.SettingInVisible = false;
		this.date_StartTime.SettingReadonly = false;
		this.date_StartTime.Size = new System.Drawing.Size(41, 18);
		this.date_StartTime.TabIndex = 1;
		this.date_StartTime.Tag = "APZ_OCLG, BeginTime";
		this.date_StartTime.ToolTip = "Start Time";
		this.date_StartTime.Validating += new System.ComponentModel.CancelEventHandler(this.date_StartTime_Validating);
		this.date_StartTime.Validated += new System.EventHandler(this.Control_Validated);
		this.labelControl6.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("labelControl6.ImageOptions.Image");
		this.labelControl6.Location = new System.Drawing.Point(176, 32);
		this.labelControl6.Name = "labelControl6";
		this.labelControl6.Size = new System.Drawing.Size(16, 16);
		this.labelControl6.TabIndex = 63;
		this.labelControl6.ToolTip = "Document Type";
		this.labelControl7.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("labelControl7.ImageOptions.Image");
		this.labelControl7.Location = new System.Drawing.Point(4, 2);
		this.labelControl7.Name = "labelControl7";
		this.labelControl7.Size = new System.Drawing.Size(16, 16);
		this.labelControl7.TabIndex = 76;
		this.labelControl7.ToolTip = "Start Date";
		this.labelControl3.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("labelControl3.ImageOptions.Image");
		this.labelControl3.Location = new System.Drawing.Point(176, 60);
		this.labelControl3.Name = "labelControl3";
		this.labelControl3.Size = new System.Drawing.Size(16, 16);
		this.labelControl3.TabIndex = 62;
		this.labelControl3.ToolTip = "Document Number";
		this.date_StartDate.EditControl = null;
		this.date_StartDate.EditValue = null;
		this.date_StartDate.FieldName = "StartDate";
		this.date_StartDate.Location = new System.Drawing.Point(25, 2);
		this.date_StartDate.Name = "date_StartDate";
		this.date_StartDate.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.date_StartDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_StartDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_StartDate.Properties.IsSearchControl = false;
		this.date_StartDate.Properties.LookAndFeel.UseDefaultLookAndFeel = false;
		this.date_StartDate.Properties.NullValuePrompt = "Start Date...";
		this.date_StartDate.SettingInVisible = false;
		this.date_StartDate.SettingReadonly = false;
		this.date_StartDate.Size = new System.Drawing.Size(87, 18);
		this.date_StartDate.TabIndex = 0;
		this.date_StartDate.Tag = "APZ_OCLG, Recontact";
		this.date_StartDate.Validating += new System.ComponentModel.CancelEventHandler(this.date_StartDate_Validating);
		this.date_StartDate.Validated += new System.EventHandler(this.Control_Validated);
		this.labelControl11.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("labelControl11.ImageOptions.Image");
		this.labelControl11.Location = new System.Drawing.Point(4, 112);
		this.labelControl11.Name = "labelControl11";
		this.labelControl11.Size = new System.Drawing.Size(16, 16);
		this.labelControl11.TabIndex = 61;
		this.labelControl11.ToolTip = "Priority";
		this.labelControl12.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("labelControl12.ImageOptions.Image");
		this.labelControl12.Location = new System.Drawing.Point(176, 84);
		this.labelControl12.Name = "labelControl12";
		this.labelControl12.Size = new System.Drawing.Size(16, 16);
		this.labelControl12.TabIndex = 60;
		this.labelControl12.ToolTip = "Opportunity";
		this.labelControl2.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("labelControl2.ImageOptions.Image");
		this.labelControl2.Location = new System.Drawing.Point(4, 86);
		this.labelControl2.Name = "labelControl2";
		this.labelControl2.Size = new System.Drawing.Size(16, 16);
		this.labelControl2.TabIndex = 61;
		this.labelControl2.ToolTip = "Distribution Rule";
		this.labelControl10.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("labelControl10.ImageOptions.Image");
		this.labelControl10.Location = new System.Drawing.Point(4, 30);
		this.labelControl10.Name = "labelControl10";
		this.labelControl10.Size = new System.Drawing.Size(16, 16);
		this.labelControl10.TabIndex = 60;
		this.labelControl10.ToolTip = "Project";
		this.labelControl1.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("labelControl1.ImageOptions.Image");
		this.labelControl1.Location = new System.Drawing.Point(4, 60);
		this.labelControl1.Name = "labelControl1";
		this.labelControl1.Size = new System.Drawing.Size(16, 16);
		this.labelControl1.TabIndex = 60;
		this.labelControl1.ToolTip = "Work Breakdown Structure";
		this.cfl_ProjectWBS.EditControl = null;
		this.cfl_ProjectWBS.FieldName = "ProjectWBS";
		this.cfl_ProjectWBS.IsSearchControl = false;
		this.cfl_ProjectWBS.Location = new System.Drawing.Point(25, 58);
		this.cfl_ProjectWBS.Name = "cfl_ProjectWBS";
		this.cfl_ProjectWBS.Properties.AutoLoadDataSource = true;
		this.cfl_ProjectWBS.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		editorButtonImageOptions9.Image = Apzon.Forms.Properties.Resources.right;
		this.cfl_ProjectWBS.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, false, true, editorButtonImageOptions9, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance18, appearanceHovered17, appearancePressed18, appearanceDisabled18, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.cfl_ProjectWBS.Properties.ContainerControl = null;
		this.cfl_ProjectWBS.Properties.DisplayMember = "";
		this.cfl_ProjectWBS.Properties.DisplayMember2 = "";
		this.cfl_ProjectWBS.Properties.LoadDataSourceWhenEnter = false;
		this.cfl_ProjectWBS.Properties.LookAndFeel.UseDefaultLookAndFeel = false;
		this.cfl_ProjectWBS.Properties.NullValuePrompt = "Project WBS...";
		this.cfl_ProjectWBS.Properties.SeparateCharacterForMulti = ';';
		this.cfl_ProjectWBS.Properties.SeparateString = ", ";
		this.cfl_ProjectWBS.Properties.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.Project;
		this.cfl_ProjectWBS.Properties.UsingAutoCompleteBox = false;
		this.cfl_ProjectWBS.Properties.UsingDataSource = true;
		this.cfl_ProjectWBS.Properties.ValidateWhenNoDataSource = false;
		this.cfl_ProjectWBS.Properties.ValueMember = "PrjCode";
		this.cfl_ProjectWBS.SettingInVisible = false;
		this.cfl_ProjectWBS.SettingReadonly = false;
		this.cfl_ProjectWBS.Size = new System.Drawing.Size(134, 18);
		this.cfl_ProjectWBS.TabIndex = 3;
		this.cfl_ProjectWBS.Validating += new System.ComponentModel.CancelEventHandler(this.Control_Validating);
		this.cfl_ProjectWBS.Validated += new System.EventHandler(this.Control_Validated);
		this.splitSubTask.Collapsed = true;
		this.splitSubTask.CollapsePanel = DevExpress.XtraEditors.SplitCollapsePanel.Panel1;
		this.splitSubTask.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitSubTask.Horizontal = false;
		this.splitSubTask.Location = new System.Drawing.Point(0, 0);
		this.splitSubTask.Name = "splitSubTask";
		this.splitSubTask.Panel1.AutoScroll = true;
		this.splitSubTask.Panel1.Controls.Add(this.grd_ListSub);
		this.splitSubTask.Panel1.Text = "Panel1";
		this.splitSubTask.Panel2.Controls.Add(this.splitAttachment);
		this.splitSubTask.Panel2.Text = "Panel2";
		this.splitSubTask.Size = new System.Drawing.Size(341, 322);
		this.splitSubTask.SplitterPosition = 164;
		this.splitSubTask.TabIndex = 71;
		this.splitSubTask.Text = "splitContainerControl2";
		this.grd_ListSub.AllowDrop = true;
		this.grd_ListSub.AllowPasteExcel = false;
		this.grd_ListSub.ClipboardData = null;
		this.grd_ListSub.ContextMenuStrip = this.contextMenuStripSub;
		this.grd_ListSub.Dock = System.Windows.Forms.DockStyle.Top;
		this.grd_ListSub.Location = new System.Drawing.Point(0, 0);
		this.grd_ListSub.LookAndFeel.UseDefaultLookAndFeel = false;
		this.grd_ListSub.MainView = this.gv_ListSub;
		this.grd_ListSub.Name = "grd_ListSub";
		this.grd_ListSub.Size = new System.Drawing.Size(0, 0);
		this.grd_ListSub.TabIndex = 6;
		this.grd_ListSub.ToolTipController = this.toolTipGridSub;
		this.grd_ListSub.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_ListSub });
		this.grd_ListSub.ProcessGridKey += new System.Windows.Forms.KeyEventHandler(this.grd_ListSub_ProcessGridKey);
		this.grd_ListSub.DragDrop += new System.Windows.Forms.DragEventHandler(this.grd_List_DragDrop);
		this.grd_ListSub.DragOver += new System.Windows.Forms.DragEventHandler(this.grd_List_DragOver);
		this.grd_ListSub.KeyUp += new System.Windows.Forms.KeyEventHandler(this.grd_ListSub_ProcessGridKey);
		this.contextMenuStripSub.Items.AddRange(new System.Windows.Forms.ToolStripItem[7] { this.detailsToolStripMenuItem, this.toolStripSeparator4, this.copyTaskToolStripMenuItem2, this.copyTaskAndToolStripMenuItem2, this.ctm_MainTaskAssigner, this.toolStripSeparator3, this.toolStripMenuItem_DeleteCM });
		this.contextMenuStripSub.Name = "contextMenuStrip_Comment";
		this.contextMenuStripSub.Size = new System.Drawing.Size(208, 126);
		this.detailsToolStripMenuItem.Name = "detailsToolStripMenuItem";
		this.detailsToolStripMenuItem.ShortcutKeyDisplayString = "";
		this.detailsToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
		this.detailsToolStripMenuItem.Text = "Details";
		this.detailsToolStripMenuItem.Click += new System.EventHandler(this.detailsToolStripMenuItem_Click);
		this.toolStripSeparator4.Name = "toolStripSeparator4";
		this.toolStripSeparator4.Size = new System.Drawing.Size(204, 6);
		this.copyTaskToolStripMenuItem2.Name = "copyTaskToolStripMenuItem2";
		this.copyTaskToolStripMenuItem2.Size = new System.Drawing.Size(207, 22);
		this.copyTaskToolStripMenuItem2.Text = "Copy Task";
		this.copyTaskToolStripMenuItem2.Click += new System.EventHandler(this.copyTaskToolStripMenuItem2_Click);
		this.copyTaskAndToolStripMenuItem2.Name = "copyTaskAndToolStripMenuItem2";
		this.copyTaskAndToolStripMenuItem2.Size = new System.Drawing.Size(207, 22);
		this.copyTaskAndToolStripMenuItem2.Text = "Copy Task And...";
		this.copyTaskAndToolStripMenuItem2.Click += new System.EventHandler(this.copyTaskAndToolStripMenuItem2_Click);
		this.ctm_MainTaskAssigner.Name = "ctm_MainTaskAssigner";
		this.ctm_MainTaskAssigner.Size = new System.Drawing.Size(207, 22);
		this.ctm_MainTaskAssigner.Text = "Assign to other members";
		this.ctm_MainTaskAssigner.Visible = false;
		this.ctm_MainTaskAssigner.Click += new System.EventHandler(this.ctm_MainTaskAssigner_Click);
		this.toolStripSeparator3.Name = "toolStripSeparator3";
		this.toolStripSeparator3.Size = new System.Drawing.Size(204, 6);
		this.toolStripMenuItem_DeleteCM.Name = "toolStripMenuItem_DeleteCM";
		this.toolStripMenuItem_DeleteCM.ShortcutKeyDisplayString = "";
		this.toolStripMenuItem_DeleteCM.Size = new System.Drawing.Size(207, 22);
		this.toolStripMenuItem_DeleteCM.Text = "Remove";
		this.toolStripMenuItem_DeleteCM.Click += new System.EventHandler(this.toolStripMenuItem_DeleteCM_Click);
		this.gv_ListSub.Appearance.Row.ForeColor = System.Drawing.Color.FromArgb(32, 31, 53);
		this.gv_ListSub.Appearance.Row.Options.UseForeColor = true;
		this.gv_ListSub.AutoFillColumn = this.gridColumnEx8;
		this.gv_ListSub.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.gv_ListSub.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_ListSub.ColumnKey = null;
		this.gv_ListSub.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[13]
		{
			this.gridColumnEx9, this.gridColumnEx8, this.gridColumnEx10, this.gridColumnEx16, this.gridColumnEx17, this.gridColumnEx18, this.gridColumnEx19, this.gridColumnEx20, this.gridColumnEx21, this.gridColumnEx22,
			this.gridColumnEx23, this.gridColumnEx24, this.gridColumnEx25
		});
		this.gv_ListSub.DisableFormSetting = false;
		this.gv_ListSub.DisplayException = false;
		this.gv_ListSub.EditColumn = null;
		this.gv_ListSub.FieldCurrency = null;
		this.gv_ListSub.FieldRate = null;
		this.gv_ListSub.FormId = "oTaskManagement";
		this.gv_ListSub.GridControl = this.grd_ListSub;
		this.gv_ListSub.IgnoreFilter = false;
		this.gv_ListSub.IndexEditRow = -1;
		this.gv_ListSub.IndicatorWidth = 35;
		this.gv_ListSub.IsApzTable = false;
		this.gv_ListSub.IsInsertNewRow = false;
		this.gv_ListSub.IsLoadingData = false;
		this.gv_ListSub.IsMappingContextAction = false;
		this.gv_ListSub.ItemId = "OLOS";
		this.gv_ListSub.LstColumnAllowBatch = null;
		this.gv_ListSub.LstColumnNotAllowBatch = null;
		this.gv_ListSub.LstColumnNotEditable = null;
		this.gv_ListSub.LstColumNotSetting = null;
		this.gv_ListSub.Name = "gv_ListSub";
		this.gv_ListSub.NewItemRowText = "Add Sub Task";
		this.gv_ListSub.OptionsBehavior.AutoSelectAllInEditor = false;
		this.gv_ListSub.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseUp;
		this.gv_ListSub.OptionsView.BestFitMaxRowCount = 100;
		this.gv_ListSub.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
		this.gv_ListSub.OptionsView.RowAutoHeight = true;
		this.gv_ListSub.OptionsView.ShowColumnHeaders = false;
		this.gv_ListSub.OptionsView.ShowGroupPanel = false;
		this.gv_ListSub.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.True;
		this.gv_ListSub.OptionsView.ShowIndicator = false;
		this.gv_ListSub.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.False;
		this.gv_ListSub.UseBatchMod = false;
		this.gv_ListSub.RowCellClick += new DevExpress.XtraGrid.Views.Grid.RowCellClickEventHandler(this.gv_ListSub_RowCellClick);
		this.gv_ListSub.CustomDrawCell += new DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventHandler(this.gv_ListSub_CustomDrawCell);
		this.gv_ListSub.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_List_RowCellStyle);
		this.gv_ListSub.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gv_List_CustomRowCellEdit);
		this.gv_ListSub.CustomRowCellEditForEditing += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gv_ListSub_CustomRowCellEditForEditing);
		this.gv_ListSub.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_ListSub_ShowingEditor);
		this.gv_ListSub.InitNewRow += new DevExpress.XtraGrid.Views.Grid.InitNewRowEventHandler(this.gv_List_InitNewRow);
		this.gv_ListSub.HiddenEditor += new System.EventHandler(this.gv_ListSub_HiddenEditor);
		this.gv_ListSub.ShownEditor += new System.EventHandler(this.gv_ListSub_ShownEditor);
		this.gv_ListSub.FocusedColumnChanged += new DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventHandler(this.gv_ListSub_FocusedColumnChanged);
		this.gv_ListSub.RowUpdated += new DevExpress.XtraGrid.Views.Base.RowObjectEventHandler(this.gv_ListSub_RowUpdated);
		this.gv_ListSub.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.gv_ListSub_CustomColumnDisplayText);
		this.gv_ListSub.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gridView1_MouseDown);
		this.gv_ListSub.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gv_ListSub_MouseMove);
		this.gv_ListSub.RowCountChanged += new System.EventHandler(this.gv_ListSub_RowCountChanged);
		this.gridColumnEx8.Caption = "Details";
		this.gridColumnEx8.ColumnEdit = this.rep_memo_DetailsSub;
		this.gridColumnEx8.CurrencyDateField = null;
		this.gridColumnEx8.CurrencyField = null;
		this.gridColumnEx8.CurrencyFixed = null;
		this.gridColumnEx8.CurrencyFixedRate = 0.0;
		this.gridColumnEx8.CurrencyRateField = null;
		this.gridColumnEx8.FieldName = "Details";
		this.gridColumnEx8.Name = "gridColumnEx8";
		this.gridColumnEx8.PrimaryKey = false;
		this.gridColumnEx8.UnitField = null;
		this.gridColumnEx8.Visible = true;
		this.gridColumnEx8.VisibleIndex = 1;
		this.gridColumnEx8.Width = 227;
		this.gridColumnEx9.Caption = "Code";
		this.gridColumnEx9.CurrencyDateField = null;
		this.gridColumnEx9.CurrencyField = null;
		this.gridColumnEx9.CurrencyFixed = null;
		this.gridColumnEx9.CurrencyFixedRate = 0.0;
		this.gridColumnEx9.CurrencyRateField = null;
		this.gridColumnEx9.FieldName = "ClgCode";
		this.gridColumnEx9.Name = "gridColumnEx9";
		this.gridColumnEx9.PrimaryKey = false;
		this.gridColumnEx9.UnitField = null;
		this.gridColumnEx10.Caption = "Action";
		this.gridColumnEx10.CurrencyDateField = null;
		this.gridColumnEx10.CurrencyField = null;
		this.gridColumnEx10.CurrencyFixed = null;
		this.gridColumnEx10.CurrencyFixedRate = 0.0;
		this.gridColumnEx10.CurrencyRateField = null;
		this.gridColumnEx10.FieldName = "Action";
		this.gridColumnEx10.Name = "gridColumnEx10";
		this.gridColumnEx10.PrimaryKey = false;
		this.gridColumnEx10.UnitField = null;
		this.gridColumnEx16.Caption = " Closed";
		this.gridColumnEx16.CurrencyDateField = null;
		this.gridColumnEx16.CurrencyField = null;
		this.gridColumnEx16.CurrencyFixed = null;
		this.gridColumnEx16.CurrencyFixedRate = 0.0;
		this.gridColumnEx16.CurrencyRateField = null;
		this.gridColumnEx16.FieldName = "Closed";
		this.gridColumnEx16.Name = "gridColumnEx16";
		this.gridColumnEx16.OptionsColumn.AllowSize = false;
		this.gridColumnEx16.OptionsColumn.FixedWidth = true;
		this.gridColumnEx16.OptionsColumn.TabStop = false;
		this.gridColumnEx16.PrimaryKey = false;
		this.gridColumnEx16.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridColumnEx16.UnitField = null;
		this.gridColumnEx16.Visible = true;
		this.gridColumnEx16.VisibleIndex = 0;
		this.gridColumnEx16.Width = 24;
		this.gridColumnEx17.AppearanceCell.Options.UseTextOptions = true;
		this.gridColumnEx17.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.gridColumnEx17.Caption = "Due Date";
		this.gridColumnEx17.CurrencyDateField = null;
		this.gridColumnEx17.CurrencyField = null;
		this.gridColumnEx17.CurrencyFixed = null;
		this.gridColumnEx17.CurrencyFixedRate = 0.0;
		this.gridColumnEx17.CurrencyRateField = null;
		this.gridColumnEx17.FieldName = "endDate";
		this.gridColumnEx17.Name = "gridColumnEx17";
		this.gridColumnEx17.OptionsColumn.AllowEdit = false;
		this.gridColumnEx17.OptionsColumn.AllowFocus = false;
		this.gridColumnEx17.OptionsColumn.FixedWidth = true;
		this.gridColumnEx17.OptionsColumn.ReadOnly = true;
		this.gridColumnEx17.OptionsColumn.TabStop = false;
		this.gridColumnEx17.PrimaryKey = false;
		this.gridColumnEx17.UnitField = null;
		this.gridColumnEx17.Width = 60;
		this.gridColumnEx18.AppearanceCell.Image = (System.Drawing.Image)resources.GetObject("gridColumnEx18.AppearanceCell.Image");
		this.gridColumnEx18.Caption = "PrjCode";
		this.gridColumnEx18.CurrencyDateField = null;
		this.gridColumnEx18.CurrencyField = null;
		this.gridColumnEx18.CurrencyFixed = null;
		this.gridColumnEx18.CurrencyFixedRate = 0.0;
		this.gridColumnEx18.CurrencyRateField = null;
		this.gridColumnEx18.FieldName = "PrjCode";
		this.gridColumnEx18.Name = "gridColumnEx18";
		this.gridColumnEx18.OptionsColumn.FixedWidth = true;
		this.gridColumnEx18.PrimaryKey = false;
		this.gridColumnEx18.UnitField = null;
		this.gridColumnEx18.Width = 70;
		this.gridColumnEx19.Caption = "AttendUser";
		this.gridColumnEx19.CurrencyDateField = null;
		this.gridColumnEx19.CurrencyField = null;
		this.gridColumnEx19.CurrencyFixed = null;
		this.gridColumnEx19.CurrencyFixedRate = 0.0;
		this.gridColumnEx19.CurrencyRateField = null;
		this.gridColumnEx19.FieldName = "AttendUser";
		this.gridColumnEx19.Name = "gridColumnEx19";
		this.gridColumnEx19.PrimaryKey = false;
		this.gridColumnEx19.UnitField = null;
		this.gridColumnEx20.Caption = "AttendEmpl";
		this.gridColumnEx20.CurrencyDateField = null;
		this.gridColumnEx20.CurrencyField = null;
		this.gridColumnEx20.CurrencyFixed = null;
		this.gridColumnEx20.CurrencyFixedRate = 0.0;
		this.gridColumnEx20.CurrencyRateField = null;
		this.gridColumnEx20.FieldName = "AttendEmpl";
		this.gridColumnEx20.Name = "gridColumnEx20";
		this.gridColumnEx20.PrimaryKey = false;
		this.gridColumnEx20.UnitField = null;
		this.gridColumnEx21.Caption = "OrderLine";
		this.gridColumnEx21.CurrencyDateField = null;
		this.gridColumnEx21.CurrencyField = null;
		this.gridColumnEx21.CurrencyFixed = null;
		this.gridColumnEx21.CurrencyFixedRate = 0.0;
		this.gridColumnEx21.CurrencyRateField = null;
		this.gridColumnEx21.FieldName = "OrderLine";
		this.gridColumnEx21.Name = "gridColumnEx21";
		this.gridColumnEx21.PrimaryKey = false;
		this.gridColumnEx21.UnitField = null;
		this.gridColumnEx22.Caption = "gridColumnEx4";
		this.gridColumnEx22.CurrencyDateField = null;
		this.gridColumnEx22.CurrencyField = null;
		this.gridColumnEx22.CurrencyFixed = null;
		this.gridColumnEx22.CurrencyFixedRate = 0.0;
		this.gridColumnEx22.CurrencyRateField = null;
		this.gridColumnEx22.FieldName = "ENDTime";
		this.gridColumnEx22.Name = "gridColumnEx22";
		this.gridColumnEx22.PrimaryKey = false;
		this.gridColumnEx22.UnitField = null;
		this.gridColumnEx23.Caption = "Create Date";
		this.gridColumnEx23.CurrencyDateField = null;
		this.gridColumnEx23.CurrencyField = null;
		this.gridColumnEx23.CurrencyFixed = null;
		this.gridColumnEx23.CurrencyFixedRate = 0.0;
		this.gridColumnEx23.CurrencyRateField = null;
		this.gridColumnEx23.FieldName = "CntctDate";
		this.gridColumnEx23.Name = "gridColumnEx23";
		this.gridColumnEx23.PrimaryKey = false;
		this.gridColumnEx23.UnitField = null;
		this.gridColumnEx24.Caption = "AssignTo";
		this.gridColumnEx24.CurrencyDateField = null;
		this.gridColumnEx24.CurrencyField = null;
		this.gridColumnEx24.CurrencyFixed = null;
		this.gridColumnEx24.CurrencyFixedRate = 0.0;
		this.gridColumnEx24.CurrencyRateField = null;
		this.gridColumnEx24.FieldName = "AssignTo";
		this.gridColumnEx24.Name = "gridColumnEx24";
		this.gridColumnEx24.OptionsColumn.FixedWidth = true;
		this.gridColumnEx24.PrimaryKey = false;
		this.gridColumnEx24.UnboundType = DevExpress.Data.UnboundColumnType.String;
		this.gridColumnEx24.UnitField = null;
		this.gridColumnEx24.Width = 90;
		this.gridColumnEx25.Caption = "Delete";
		this.gridColumnEx25.ColumnEdit = this.rep_bte_DeleteSubTask;
		this.gridColumnEx25.CurrencyDateField = null;
		this.gridColumnEx25.CurrencyField = null;
		this.gridColumnEx25.CurrencyFixed = null;
		this.gridColumnEx25.CurrencyFixedRate = 0.0;
		this.gridColumnEx25.CurrencyRateField = null;
		this.gridColumnEx25.FieldName = "Delete";
		this.gridColumnEx25.Name = "gridColumnEx25";
		this.gridColumnEx25.OptionsColumn.FixedWidth = true;
		this.gridColumnEx25.PrimaryKey = false;
		this.gridColumnEx25.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowForFocusedCell;
		this.gridColumnEx25.UnboundType = DevExpress.Data.UnboundColumnType.Object;
		this.gridColumnEx25.UnitField = null;
		this.gridColumnEx25.Width = 23;
		this.toolTipGridSub.GetActiveObjectInfo += new DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventHandler(this.toolTipGridSub_GetActiveObjectInfo);
		this.splitAttachment.CollapsePanel = DevExpress.XtraEditors.SplitCollapsePanel.Panel1;
		this.splitAttachment.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitAttachment.Horizontal = false;
		this.splitAttachment.Location = new System.Drawing.Point(0, 0);
		this.splitAttachment.Name = "splitAttachment";
		this.splitAttachment.Panel1.Controls.Add(this.ucAttachment1);
		this.splitAttachment.Panel1.Text = "Panel1";
		this.splitAttachment.Panel2.Controls.Add(this.splitUDF);
		this.splitAttachment.Panel2.Text = "Panel2";
		this.splitAttachment.Size = new System.Drawing.Size(341, 310);
		this.splitAttachment.SplitterPosition = 99;
		this.splitAttachment.TabIndex = 72;
		this.splitAttachment.Text = "splitContainerControl3";
		this.ucAttachment1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.ucAttachment1.DataSource = null;
		this.ucAttachment1.FormId = "oTaskManagement";
		this.ucAttachment1.ItemId = "ATC1";
		this.ucAttachment1.Location = new System.Drawing.Point(0, 0);
		this.ucAttachment1.Name = "ucAttachment1";
		this.ucAttachment1.RightToLeft = System.Windows.Forms.RightToLeft.No;
		this.ucAttachment1.Size = new System.Drawing.Size(341, 99);
		this.ucAttachment1.TabIndex = 7;
		this.splitUDF.CollapsePanel = DevExpress.XtraEditors.SplitCollapsePanel.Panel1;
		this.splitUDF.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitUDF.Horizontal = false;
		this.splitUDF.Location = new System.Drawing.Point(0, 0);
		this.splitUDF.Name = "splitUDF";
		this.splitUDF.Panel1.Controls.Add(this.layout_UDF);
		this.splitUDF.Panel1.Text = "Panel1";
		this.splitUDF.Panel2.Controls.Add(this.ucComment1);
		this.splitUDF.Panel2.Controls.Add(this.lblCreatedAt);
		this.splitUDF.Panel2.MinSize = 83;
		this.splitUDF.Panel2.Text = "Panel2";
		this.splitUDF.Size = new System.Drawing.Size(341, 199);
		this.splitUDF.SplitterPosition = 119;
		this.splitUDF.TabIndex = 5;
		this.splitUDF.Text = "splitContainerControl1";
		this.layout_UDF.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layout_UDF.Location = new System.Drawing.Point(0, 0);
		this.layout_UDF.LookAndFeel.UseDefaultLookAndFeel = false;
		this.layout_UDF.Name = "layout_UDF";
		this.layout_UDF.Root = this.layoutControlGroup1;
		this.layout_UDF.Size = new System.Drawing.Size(341, 104);
		this.layout_UDF.TabIndex = 2;
		this.layout_UDF.Text = "layoutControl1";
		this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup1.GroupBordersVisible = false;
		this.layoutControlGroup1.Name = "layoutControlGroup1";
		this.layoutControlGroup1.Size = new System.Drawing.Size(341, 104);
		this.layoutControlGroup1.TextVisible = false;
		this.ucComment1.AdditionNotifyUser = (System.Collections.Generic.List<int>)resources.GetObject("ucComment1.AdditionNotifyUser");
		this.ucComment1.Appearance.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.ucComment1.Appearance.Options.UseBackColor = true;
		this.ucComment1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.ucComment1.Location = new System.Drawing.Point(0, 0);
		this.ucComment1.LookAndFeel.UseDefaultLookAndFeel = false;
		this.ucComment1.Name = "ucComment1";
		this.ucComment1.ObjectType = Apzon.Commons.ApzObjectTypes.oNone;
		this.ucComment1.Size = new System.Drawing.Size(341, 62);
		this.ucComment1.TabIndex = 4;
		this.lblCreatedAt.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lblCreatedAt.Appearance.ForeColor = System.Drawing.Color.FromArgb(184, 184, 184);
		this.lblCreatedAt.Appearance.Options.UseFont = true;
		this.lblCreatedAt.Appearance.Options.UseForeColor = true;
		this.lblCreatedAt.Appearance.Options.UseTextOptions = true;
		this.lblCreatedAt.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.lblCreatedAt.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
		this.lblCreatedAt.ContextMenuStrip = this.contextMenuStripBottomText;
		this.lblCreatedAt.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.lblCreatedAt.Location = new System.Drawing.Point(0, 62);
		this.lblCreatedAt.Name = "lblCreatedAt";
		this.lblCreatedAt.Size = new System.Drawing.Size(341, 21);
		this.lblCreatedAt.TabIndex = 3;
		this.lblCreatedAt.Tag = "APZ_OCLG, CntctDate";
		this.lblCreatedAt.Text = "Created 20 hours ago, by Team Leader";
		this.contextMenuStripBottomText.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.copyTaskIDToolStripMenuItem });
		this.contextMenuStripBottomText.Name = "contextMenuStripBottomText";
		this.contextMenuStripBottomText.Size = new System.Drawing.Size(142, 26);
		this.copyTaskIDToolStripMenuItem.Name = "copyTaskIDToolStripMenuItem";
		this.copyTaskIDToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
		this.copyTaskIDToolStripMenuItem.Text = "Copy Task ID";
		this.copyTaskIDToolStripMenuItem.Click += new System.EventHandler(this.copyTaskIDToolStripMenuItem_Click);
		this.txtDetails.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.txtDetails.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.tblLayoutInfo.SetColumnSpan(this.txtDetails, 2);
		this.txtDetails.EditControl = null;
		this.txtDetails.EditValue = "";
		this.txtDetails.FieldName = "Details";
		this.txtDetails.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.txtDetails.ForeColor = System.Drawing.Color.FromArgb(32, 31, 53);
		this.txtDetails.IsSearchControl = false;
		this.txtDetails.Location = new System.Drawing.Point(3, 27);
		this.txtDetails.MaximumSize = new System.Drawing.Size(5000, 82);
		this.txtDetails.MaxLength = 300;
		this.txtDetails.MinimumSize = new System.Drawing.Size(10, 17);
		this.txtDetails.Multiline = true;
		this.txtDetails.Name = "txtDetails";
		this.txtDetails.SettingInVisible = false;
		this.txtDetails.SettingReadonly = false;
		this.txtDetails.Size = new System.Drawing.Size(314, 20);
		this.txtDetails.TabIndex = 1;
		this.txtDetails.Tag = "APZ_OCLG, Details";
		this.txtDetails.TextChanged += new System.EventHandler(this.txtDetails_TextChanged);
		this.txtDetails.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtDetails_KeyDown);
		this.txtDetails.Resize += new System.EventHandler(this.txtDetails_Resize);
		this.txtDetails.Validating += new System.ComponentModel.CancelEventHandler(this.txtDetails_Validating);
		this.txtDetails.Validated += new System.EventHandler(this.Control_Validated);
		this.panelControl2.Appearance.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.panelControl2.Appearance.Options.UseBackColor = true;
		this.panelControl2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.tblLayoutInfo.SetColumnSpan(this.panelControl2, 3);
		this.panelControl2.Controls.Add(this.txtOwner);
		this.panelControl2.Controls.Add(this.lbOwner);
		this.panelControl2.Controls.Add(this.date_DueTime);
		this.panelControl2.Controls.Add(this.date_DueDate);
		this.panelControl2.Controls.Add(this.btnAttachment);
		this.panelControl2.Controls.Add(this.labelControl5);
		this.panelControl2.Controls.Add(this.cfl_GroupUser);
		this.panelControl2.Controls.Add(this.btnUDF);
		this.panelControl2.Controls.Add(this.btnSubTask);
		this.panelControl2.Controls.Add(this.cfl_GroupEmployee);
		this.panelControl2.Controls.Add(this.btnDocument);
		this.panelControl2.Controls.Add(this.cbUser);
		this.panelControl2.Controls.Add(this.txtClgCode);
		this.panelControl2.Controls.Add(this.labelControl4);
		this.panelControl2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panelControl2.Location = new System.Drawing.Point(3, 100);
		this.panelControl2.Name = "panelControl2";
		this.panelControl2.Size = new System.Drawing.Size(341, 81);
		this.panelControl2.TabIndex = 63;
		this.txtOwner.EditControl = null;
		this.txtOwner.FieldName = "Owner";
		this.txtOwner.IsSearchControl = false;
		this.txtOwner.Location = new System.Drawing.Point(25, 28);
		this.txtOwner.Name = "txtOwner";
		this.txtOwner.Properties.AcceptNullValue.Add("0");
		this.txtOwner.Properties.AcceptNullValue.Add("-1");
		this.txtOwner.Properties.AutoLoadDataSource = true;
		this.txtOwner.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		editorButtonImageOptions10.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions20.Image");
		this.txtOwner.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions10, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance19, appearanceHovered18, appearancePressed19, appearanceDisabled19, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions10, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance20, appearanceHovered19, appearancePressed20, appearanceDisabled20, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.txtOwner.Properties.ContainerControl = this;
		this.txtOwner.Properties.DisplayMember = "lastName";
		this.txtOwner.Properties.DisplayMember2 = "firstName";
		this.txtOwner.Properties.LoadDataSourceWhenEnter = false;
		this.txtOwner.Properties.LookAndFeel.UseDefaultLookAndFeel = false;
		this.txtOwner.Properties.NullValuePrompt = "Owner...";
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
		this.txtOwner.Size = new System.Drawing.Size(134, 18);
		this.txtOwner.TabIndex = 3;
		this.txtOwner.Validating += new System.ComponentModel.CancelEventHandler(this.Control_Validating);
		this.txtOwner.Validated += new System.EventHandler(this.Control_Validated);
		this.lbOwner.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("lbOwner.ImageOptions.Image");
		this.lbOwner.Location = new System.Drawing.Point(4, 29);
		this.lbOwner.Name = "lbOwner";
		this.lbOwner.Size = new System.Drawing.Size(16, 16);
		this.lbOwner.TabIndex = 0;
		this.lbOwner.ToolTip = "Owner";
		this.date_DueTime.EditControl = null;
		this.date_DueTime.FieldName = "DueTime";
		this.date_DueTime.IsSearchControl = false;
		this.date_DueTime.Location = new System.Drawing.Point(118, 53);
		this.date_DueTime.Name = "date_DueTime";
		this.date_DueTime.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.date_DueTime.Properties.IsSearchControl = false;
		this.date_DueTime.Properties.LookAndFeel.UseDefaultLookAndFeel = false;
		this.date_DueTime.Properties.MaxLength = 5;
		this.date_DueTime.SettingInVisible = false;
		this.date_DueTime.SettingReadonly = false;
		this.date_DueTime.Size = new System.Drawing.Size(41, 18);
		this.date_DueTime.TabIndex = 5;
		this.date_DueTime.Tag = "APZ_OCLG, ENDTime";
		this.date_DueTime.ToolTip = "End Time";
		this.date_DueTime.Validating += new System.ComponentModel.CancelEventHandler(this.date_DueTime_Validating);
		this.date_DueTime.Validated += new System.EventHandler(this.Control_Validated);
		this.date_DueDate.EditControl = null;
		this.date_DueDate.EditValue = null;
		this.date_DueDate.FieldName = "DueDate";
		this.date_DueDate.Location = new System.Drawing.Point(25, 53);
		this.date_DueDate.Name = "date_DueDate";
		this.date_DueDate.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.date_DueDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_DueDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_DueDate.Properties.IsSearchControl = false;
		this.date_DueDate.Properties.LookAndFeel.UseDefaultLookAndFeel = false;
		this.date_DueDate.Properties.NullValuePrompt = "Due Date...";
		this.date_DueDate.SettingInVisible = false;
		this.date_DueDate.SettingReadonly = false;
		this.date_DueDate.Size = new System.Drawing.Size(87, 18);
		this.date_DueDate.TabIndex = 4;
		this.date_DueDate.Tag = "APZ_OCLG, endDate";
		this.date_DueDate.Validating += new System.ComponentModel.CancelEventHandler(this.date_DueDate_Validating);
		this.date_DueDate.Validated += new System.EventHandler(this.Control_Validated);
		this.btnAttachment.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btnAttachment.Appearance.Image = (System.Drawing.Image)resources.GetObject("btnAttachment.Appearance.Image");
		this.btnAttachment.Appearance.Options.UseImage = true;
		this.btnAttachment.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btnAttachment.EditControl = null;
		this.btnAttachment.EditValue = "";
		this.btnAttachment.FieldName = null;
		this.btnAttachment.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnAttachment.ImageOptions.Image");
		this.btnAttachment.IsSearchControl = false;
		this.btnAttachment.Location = new System.Drawing.Point(289, 51);
		this.btnAttachment.Name = "btnAttachment";
		this.btnAttachment.ReadOnly = false;
		this.btnAttachment.SettingInVisible = false;
		this.btnAttachment.SettingReadonly = false;
		this.btnAttachment.ShowFocusRectangle = DevExpress.Utils.DefaultBoolean.False;
		this.btnAttachment.Size = new System.Drawing.Size(23, 20);
		this.btnAttachment.TabIndex = 49;
		this.btnAttachment.TabStop = false;
		this.btnAttachment.ToolTip = "Attachments (Ctrl+3)";
		this.btnAttachment.Click += new System.EventHandler(this.btnAttachment_Click);
		this.labelControl5.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("labelControl5.ImageOptions.Image");
		this.labelControl5.Location = new System.Drawing.Point(4, 3);
		this.labelControl5.Name = "labelControl5";
		this.labelControl5.Size = new System.Drawing.Size(16, 16);
		this.labelControl5.TabIndex = 74;
		this.labelControl5.ToolTip = "Assignee";
		this.cfl_GroupUser.EditControl = null;
		this.cfl_GroupUser.FieldName = "GroupUser";
		this.cfl_GroupUser.IsSearchControl = false;
		this.cfl_GroupUser.Location = new System.Drawing.Point(99, 3);
		this.cfl_GroupUser.Name = "cfl_GroupUser";
		this.cfl_GroupUser.Properties.AutoLoadDataSource = true;
		this.cfl_GroupUser.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.cfl_GroupUser.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, false, true, imageOptions11, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance21, appearanceHovered20, appearancePressed21, appearanceDisabled21, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.cfl_GroupUser.Properties.ContainerControl = this;
		this.cfl_GroupUser.Properties.DisplayMember = "";
		this.cfl_GroupUser.Properties.DisplayMember2 = "";
		this.cfl_GroupUser.Properties.LoadDataSourceWhenEnter = false;
		this.cfl_GroupUser.Properties.LookAndFeel.UseDefaultLookAndFeel = false;
		this.cfl_GroupUser.Properties.NullValuePrompt = "Assignee...";
		this.cfl_GroupUser.Properties.SeparateCharacterForMulti = ';';
		this.cfl_GroupUser.Properties.SeparateString = ", ";
		this.cfl_GroupUser.Properties.UsingAutoCompleteBox = false;
		this.cfl_GroupUser.Properties.UsingDataSource = true;
		this.cfl_GroupUser.Properties.ValidateWhenNoDataSource = false;
		this.cfl_GroupUser.Properties.ValueMember = "";
		this.cfl_GroupUser.SettingInVisible = false;
		this.cfl_GroupUser.SettingReadonly = false;
		this.cfl_GroupUser.Size = new System.Drawing.Size(123, 18);
		this.cfl_GroupUser.TabIndex = 2;
		this.cfl_GroupUser.Tag = "APZ_OCLG, GroupUser";
		this.cfl_GroupUser.Validating += new System.ComponentModel.CancelEventHandler(this.Control_Validating);
		this.cfl_GroupUser.Validated += new System.EventHandler(this.Control_Validated);
		this.btnUDF.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btnUDF.Appearance.Image = (System.Drawing.Image)resources.GetObject("btnUDF.Appearance.Image");
		this.btnUDF.Appearance.Options.UseImage = true;
		this.btnUDF.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btnUDF.EditControl = null;
		this.btnUDF.EditValue = "";
		this.btnUDF.FieldName = null;
		this.btnUDF.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnUDF.ImageOptions.Image");
		this.btnUDF.IsSearchControl = false;
		this.btnUDF.Location = new System.Drawing.Point(316, 51);
		this.btnUDF.Name = "btnUDF";
		this.btnUDF.ReadOnly = false;
		this.btnUDF.SettingInVisible = false;
		this.btnUDF.SettingReadonly = false;
		this.btnUDF.ShowFocusRectangle = DevExpress.Utils.DefaultBoolean.False;
		this.btnUDF.Size = new System.Drawing.Size(23, 20);
		this.btnUDF.TabIndex = 49;
		this.btnUDF.TabStop = false;
		this.btnUDF.ToolTip = "Additional Informations (Ctrl+4)";
		this.btnUDF.Click += new System.EventHandler(this.btnUDF_Click);
		this.btnSubTask.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btnSubTask.Appearance.Image = (System.Drawing.Image)resources.GetObject("btnSubTask.Appearance.Image");
		this.btnSubTask.Appearance.Options.UseImage = true;
		this.btnSubTask.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btnSubTask.EditControl = null;
		this.btnSubTask.EditValue = "";
		this.btnSubTask.FieldName = null;
		this.btnSubTask.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnSubTask.ImageOptions.Image");
		this.btnSubTask.IsSearchControl = false;
		this.btnSubTask.Location = new System.Drawing.Point(262, 51);
		this.btnSubTask.Name = "btnSubTask";
		this.btnSubTask.ReadOnly = false;
		this.btnSubTask.SettingInVisible = false;
		this.btnSubTask.SettingReadonly = false;
		this.btnSubTask.ShowFocusRectangle = DevExpress.Utils.DefaultBoolean.False;
		this.btnSubTask.Size = new System.Drawing.Size(23, 20);
		this.btnSubTask.TabIndex = 60;
		this.btnSubTask.TabStop = false;
		this.btnSubTask.ToolTip = "Sub tasks (Ctrl+2)";
		this.btnSubTask.Click += new System.EventHandler(this.btnSubTask_Click);
		this.cfl_GroupEmployee.EditControl = null;
		this.cfl_GroupEmployee.FieldName = null;
		this.cfl_GroupEmployee.IsSearchControl = false;
		this.cfl_GroupEmployee.Location = new System.Drawing.Point(99, 3);
		this.cfl_GroupEmployee.Name = "cfl_GroupEmployee";
		this.cfl_GroupEmployee.Properties.AutoLoadDataSource = true;
		this.cfl_GroupEmployee.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.cfl_GroupEmployee.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, false, true, imageOptions12, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance22, appearanceHovered21, appearancePressed22, appearanceDisabled22, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.cfl_GroupEmployee.Properties.ContainerControl = null;
		this.cfl_GroupEmployee.Properties.DisplayMember = "";
		this.cfl_GroupEmployee.Properties.DisplayMember2 = "";
		this.cfl_GroupEmployee.Properties.LoadDataSourceWhenEnter = false;
		this.cfl_GroupEmployee.Properties.LookAndFeel.UseDefaultLookAndFeel = false;
		this.cfl_GroupEmployee.Properties.NullValuePrompt = "Assignee...";
		this.cfl_GroupEmployee.Properties.SeparateCharacterForMulti = ';';
		this.cfl_GroupEmployee.Properties.SeparateString = ", ";
		this.cfl_GroupEmployee.Properties.UsingAutoCompleteBox = false;
		this.cfl_GroupEmployee.Properties.UsingDataSource = true;
		this.cfl_GroupEmployee.Properties.ValidateWhenNoDataSource = false;
		this.cfl_GroupEmployee.Properties.ValueMember = "";
		this.cfl_GroupEmployee.SettingInVisible = false;
		this.cfl_GroupEmployee.SettingReadonly = false;
		this.cfl_GroupEmployee.Size = new System.Drawing.Size(123, 18);
		this.cfl_GroupEmployee.TabIndex = 3;
		this.cfl_GroupEmployee.Tag = "APZ_OCLG, GroupEmployee";
		this.cfl_GroupEmployee.Visible = false;
		this.cfl_GroupEmployee.Validating += new System.ComponentModel.CancelEventHandler(this.Control_Validating);
		this.cfl_GroupEmployee.Validated += new System.EventHandler(this.Control_Validated);
		this.btnDocument.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btnDocument.Appearance.Image = (System.Drawing.Image)resources.GetObject("btnDocument.Appearance.Image");
		this.btnDocument.Appearance.Options.UseImage = true;
		this.btnDocument.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btnDocument.EditControl = null;
		this.btnDocument.EditValue = "";
		this.btnDocument.FieldName = null;
		this.btnDocument.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnDocument.ImageOptions.Image");
		this.btnDocument.IsSearchControl = false;
		this.btnDocument.Location = new System.Drawing.Point(235, 51);
		this.btnDocument.Name = "btnDocument";
		this.btnDocument.ReadOnly = false;
		this.btnDocument.SettingInVisible = false;
		this.btnDocument.SettingReadonly = false;
		this.btnDocument.ShowFocusRectangle = DevExpress.Utils.DefaultBoolean.False;
		this.btnDocument.Size = new System.Drawing.Size(23, 20);
		this.btnDocument.TabIndex = 72;
		this.btnDocument.TabStop = false;
		this.btnDocument.ToolTip = "Details (Ctrl+1)";
		this.btnDocument.Click += new System.EventHandler(this.btnDocument_Click);
		this.cbUser.EditControl = null;
		this.cbUser.EditValue = "User";
		this.cbUser.FieldName = "User";
		this.cbUser.IsSearchControl = false;
		this.cbUser.Location = new System.Drawing.Point(25, 3);
		this.cbUser.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.cbUser.Name = "cbUser";
		this.cbUser.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.cbUser.Properties.Appearance.ForeColor = System.Drawing.Color.FromArgb(32, 31, 53);
		this.cbUser.Properties.Appearance.Options.UseFont = true;
		this.cbUser.Properties.Appearance.Options.UseForeColor = true;
		this.cbUser.Properties.AutoHeight = false;
		this.cbUser.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.cbUser.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbUser.Properties.Items.AddRange(new object[2] { "User", "Employee" });
		this.cbUser.Properties.LookAndFeel.UseDefaultLookAndFeel = false;
		this.cbUser.SettingInVisible = false;
		this.cbUser.SettingReadonly = false;
		this.cbUser.Size = new System.Drawing.Size(68, 18);
		this.cbUser.TabIndex = 1;
		this.cbUser.SelectedValueChanged += new System.EventHandler(this.cbUser_SelectedValueChanged);
		this.txtClgCode.EditControl = null;
		this.txtClgCode.FieldName = null;
		this.txtClgCode.IsSearchControl = false;
		this.txtClgCode.Location = new System.Drawing.Point(118, 53);
		this.txtClgCode.Name = "txtClgCode";
		this.txtClgCode.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.txtClgCode.SettingInVisible = false;
		this.txtClgCode.SettingReadonly = false;
		this.txtClgCode.Size = new System.Drawing.Size(14, 18);
		this.txtClgCode.TabIndex = 61;
		this.labelControl4.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("labelControl4.ImageOptions.Image");
		this.labelControl4.Location = new System.Drawing.Point(4, 54);
		this.labelControl4.Name = "labelControl4";
		this.labelControl4.Size = new System.Drawing.Size(16, 16);
		this.labelControl4.TabIndex = 63;
		this.labelControl4.ToolTip = "Due Date";
		this.memoNotes.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.memoNotes.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.tblLayoutInfo.SetColumnSpan(this.memoNotes, 3);
		this.memoNotes.EditControl = null;
		this.memoNotes.EditValue = "";
		this.memoNotes.FieldName = "Notes";
		this.memoNotes.IsSearchControl = false;
		this.memoNotes.Location = new System.Drawing.Point(3, 53);
		this.memoNotes.MaximumSize = new System.Drawing.Size(5000, 250);
		this.memoNotes.MinimumSize = new System.Drawing.Size(10, 41);
		this.memoNotes.Multiline = true;
		this.memoNotes.Name = "memoNotes";
		this.memoNotes.SettingInVisible = false;
		this.memoNotes.SettingReadonly = false;
		this.memoNotes.Size = new System.Drawing.Size(341, 41);
		this.memoNotes.TabIndex = 2;
		this.memoNotes.TextChanged += new System.EventHandler(this.memoNotes_TextChanged);
		this.memoNotes.Resize += new System.EventHandler(this.memoNotes_Resize);
		this.memoNotes.Validating += new System.ComponentModel.CancelEventHandler(this.txtDetails_Validating);
		this.memoNotes.Validated += new System.EventHandler(this.Control_Validated);
		this.chk_Closed.CausesValidation = false;
		this.chk_Closed.EditControl = null;
		this.chk_Closed.FieldName = null;
		this.chk_Closed.IsSearchControl = false;
		this.chk_Closed.Location = new System.Drawing.Point(323, 27);
		this.chk_Closed.Name = "chk_Closed";
		this.chk_Closed.Properties.Caption = "";
		this.chk_Closed.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.UserDefined;
		this.chk_Closed.Properties.GlyphAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.chk_Closed.Properties.ImageOptions.ImageChecked = Apzon.Forms.Properties.Resources.completed;
		this.chk_Closed.Properties.ImageOptions.ImageUnchecked = Apzon.Forms.Properties.Resources.incompleted;
		this.chk_Closed.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.chk_Closed.Properties.ValueChecked = "Y";
		this.chk_Closed.Properties.ValueUnchecked = "N";
		this.chk_Closed.SettingInVisible = false;
		this.chk_Closed.SettingReadonly = false;
		this.chk_Closed.Size = new System.Drawing.Size(21, 20);
		this.chk_Closed.TabIndex = 1;
		this.chk_Closed.TabStop = false;
		this.chk_Closed.Tag = "APZ_OCLG, Closed";
		this.chk_Closed.ToolTipController = this.toolTipController1;
		this.chk_Closed.CheckedChanged += new System.EventHandler(this.chk_Closed_CheckedChanged);
		this.chk_Closed.Validating += new System.ComponentModel.CancelEventHandler(this.Control_Validating);
		this.chk_Closed.Validated += new System.EventHandler(this.Control_Validated);
		this.toolTipController1.BeforeShow += new DevExpress.Utils.ToolTipControllerBeforeShowEventHandler(this.toolTipController1_BeforeShow);
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.UnitField = null;
		this.panelFilter.Controls.Add(this.accordionControl1);
		this.panelFilter.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panelFilter.Location = new System.Drawing.Point(0, 0);
		this.panelFilter.Name = "panelFilter";
		this.panelFilter.Size = new System.Drawing.Size(195, 753);
		this.panelFilter.TabIndex = 64;
		this.accordionControl1.AllowItemSelection = true;
		this.accordionControl1.Cursor = System.Windows.Forms.Cursors.Default;
		this.accordionControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.accordionControl1.Elements.AddRange(new DevExpress.XtraBars.Navigation.AccordionControlElement[4] { this.eleGeneral, this.eleUser, this.eleEmployee, this.eleProjectNew });
		this.accordionControl1.Location = new System.Drawing.Point(2, 2);
		this.accordionControl1.Name = "accordionControl1";
		this.accordionControl1.ShowFilterControl = DevExpress.XtraBars.Navigation.ShowFilterControl.Always;
		this.accordionControl1.Size = new System.Drawing.Size(191, 749);
		this.accordionControl1.TabIndex = 0;
		this.accordionControl1.Text = "accordionControl1";
		this.accordionControl1.SelectedElementChanged += new DevExpress.XtraBars.Navigation.SelectedElementChangedEventHandler(this.accordionControl1_SelectedElementChanged);
		this.eleGeneral.Elements.AddRange(new DevExpress.XtraBars.Navigation.AccordionControlElement[5] { this.eleAll, this.eleOverdue, this.eleToday, this.eleThisWeek, this.eleLastWeek });
		this.eleGeneral.Expanded = true;
		this.eleGeneral.Name = "eleGeneral";
		this.eleGeneral.Tag = "General";
		this.eleGeneral.Text = "General";
		this.eleAll.Appearance.Normal.Font = new System.Drawing.Font("Verdana", 8.25f);
		this.eleAll.Appearance.Normal.ForeColor = System.Drawing.Color.FromArgb(47, 46, 68);
		this.eleAll.Appearance.Normal.Options.UseFont = true;
		this.eleAll.Appearance.Normal.Options.UseForeColor = true;
		this.eleAll.Name = "eleAll";
		this.eleAll.Style = DevExpress.XtraBars.Navigation.ElementStyle.Item;
		this.eleAll.Tag = "All";
		this.eleAll.Text = "All";
		this.eleOverdue.Appearance.Normal.Font = new System.Drawing.Font("Verdana", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.eleOverdue.Appearance.Normal.ForeColor = System.Drawing.Color.FromArgb(47, 46, 68);
		this.eleOverdue.Appearance.Normal.Options.UseFont = true;
		this.eleOverdue.Appearance.Normal.Options.UseForeColor = true;
		this.eleOverdue.Name = "eleOverdue";
		this.eleOverdue.Style = DevExpress.XtraBars.Navigation.ElementStyle.Item;
		this.eleOverdue.Tag = "Overdue";
		this.eleOverdue.Text = "Overdue";
		this.eleToday.Appearance.Normal.Font = new System.Drawing.Font("Verdana", 8.25f);
		this.eleToday.Appearance.Normal.ForeColor = System.Drawing.Color.FromArgb(47, 46, 68);
		this.eleToday.Appearance.Normal.Options.UseFont = true;
		this.eleToday.Appearance.Normal.Options.UseForeColor = true;
		this.eleToday.HeaderTemplate.AddRange(new DevExpress.XtraBars.Navigation.HeaderElementInfo[4]
		{
			new DevExpress.XtraBars.Navigation.HeaderElementInfo(DevExpress.XtraBars.Navigation.HeaderElementType.Text),
			new DevExpress.XtraBars.Navigation.HeaderElementInfo(DevExpress.XtraBars.Navigation.HeaderElementType.Image),
			new DevExpress.XtraBars.Navigation.HeaderElementInfo(DevExpress.XtraBars.Navigation.HeaderElementType.HeaderControl),
			new DevExpress.XtraBars.Navigation.HeaderElementInfo(DevExpress.XtraBars.Navigation.HeaderElementType.ContextButtons)
		});
		this.eleToday.Name = "eleToday";
		this.eleToday.Style = DevExpress.XtraBars.Navigation.ElementStyle.Item;
		this.eleToday.Tag = "Today";
		this.eleToday.Text = "Today";
		this.eleThisWeek.Appearance.Normal.Font = new System.Drawing.Font("Verdana", 8.25f);
		this.eleThisWeek.Appearance.Normal.ForeColor = System.Drawing.Color.FromArgb(47, 46, 68);
		this.eleThisWeek.Appearance.Normal.Options.UseFont = true;
		this.eleThisWeek.Appearance.Normal.Options.UseForeColor = true;
		this.eleThisWeek.Name = "eleThisWeek";
		this.eleThisWeek.Style = DevExpress.XtraBars.Navigation.ElementStyle.Item;
		this.eleThisWeek.Tag = "ThisWeek";
		this.eleThisWeek.Text = "This Week";
		this.eleLastWeek.Appearance.Normal.Font = new System.Drawing.Font("Verdana", 8.25f);
		this.eleLastWeek.Appearance.Normal.ForeColor = System.Drawing.Color.FromArgb(47, 46, 68);
		this.eleLastWeek.Appearance.Normal.Options.UseFont = true;
		this.eleLastWeek.Appearance.Normal.Options.UseForeColor = true;
		this.eleLastWeek.Name = "eleLastWeek";
		this.eleLastWeek.Style = DevExpress.XtraBars.Navigation.ElementStyle.Item;
		this.eleLastWeek.Tag = "LastWeek";
		this.eleLastWeek.Text = "Last Week";
		this.eleUser.Name = "eleUser";
		this.eleUser.Tag = "Users";
		this.eleUser.Text = "Users";
		this.eleEmployee.Name = "eleEmployee";
		this.eleEmployee.Tag = "Employees";
		this.eleEmployee.Text = "Employees";
		this.eleProjectNew.Name = "eleProjectNew";
		this.eleProjectNew.Tag = "Project New";
		this.eleProjectNew.Text = "Projects";
		this.splitAccordion.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitAccordion.IsSplitterFixed = true;
		this.splitAccordion.Location = new System.Drawing.Point(0, 0);
		this.splitAccordion.Name = "splitAccordion";
		this.splitAccordion.Panel1.Controls.Add(this.panelFilter);
		this.splitAccordion.Panel1.Text = "Panel1";
		this.splitAccordion.Panel2.Controls.Add(this.splitListTask);
		this.splitAccordion.Panel2.Text = "Panel2";
		this.splitAccordion.Size = new System.Drawing.Size(1200, 753);
		this.splitAccordion.SplitterPosition = 195;
		this.splitAccordion.TabIndex = 65;
		this.splitAccordion.Text = "splitContainerControl5";
		this.gridColumnEx6.Caption = "Task";
		this.gridColumnEx6.CurrencyDateField = null;
		this.gridColumnEx6.CurrencyField = null;
		this.gridColumnEx6.CurrencyFixed = null;
		this.gridColumnEx6.CurrencyFixedRate = 0.0;
		this.gridColumnEx6.CurrencyRateField = null;
		this.gridColumnEx6.FieldName = "Details";
		this.gridColumnEx6.Name = "gridColumnEx6";
		this.gridColumnEx6.PrimaryKey = false;
		this.gridColumnEx6.UnitField = null;
		this.gridColumnEx6.Visible = true;
		this.gridColumnEx6.VisibleIndex = 1;
		this.gridColumnEx6.Width = 237;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1200, 753);
		base.Controls.Add(this.splitAccordion);
		base.FormId = "oTaskManagement";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_TaskManagement";
		base.ShowInTaskbar = false;
		this.Text = "Task Management";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_TaskManagement_FormClosing);
		base.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frm_TaskManagement_FormClosed);
		base.Shown += new System.EventHandler(this.frm_TaskManagement_Shown);
		((System.ComponentModel.ISupportInitialize)this.rep_memo_DetailsSub).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_bte_DeleteSubTask).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_luk_Priority).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemButtonEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitListTask).EndInit();
		this.splitListTask.ResumeLayout(false);
		this.xtraScrollableControl1.ResumeLayout(false);
		this.xtraScrollableControl1.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.ccb_GroupEmployee.Properties).EndInit();
		this.tableLayoutPanel1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_List).EndInit();
		this.contextMenuList.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_List).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_bte_Closed).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_cfl_ProjectEntry).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_cfl_ProjectWBS).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemHourEdit2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_ListCompleted).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_ListCompleted).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_bte_Open).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_PrjGrid).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemHourEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ccb_GroupUser.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Sort.Properties).EndInit();
		this.tblLayoutInfo.ResumeLayout(false);
		this.tblLayoutInfo.PerformLayout();
		this.contextMenuInfoPanel.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.brc_ParentTask.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitMoreInformation).EndInit();
		this.splitMoreInformation.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.panelControl1).EndInit();
		this.panelControl1.ResumeLayout(false);
		this.panelControl1.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.luk_TaskMode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cfl_Task.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BPName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Priority.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BPType.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cfl_Opportunity.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cfl_ProjectNew.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cfl_BPCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bte_DistrRule.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_StartTime.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_StartDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_StartDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cfl_ProjectWBS.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitSubTask).EndInit();
		this.splitSubTask.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_ListSub).EndInit();
		this.contextMenuStripSub.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_ListSub).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitAttachment).EndInit();
		this.splitAttachment.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.splitUDF).EndInit();
		this.splitUDF.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.layout_UDF).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).EndInit();
		this.contextMenuStripBottomText.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.panelControl2).EndInit();
		this.panelControl2.ResumeLayout(false);
		this.panelControl2.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.txtOwner.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_DueTime.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_DueDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_DueDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cfl_GroupUser.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cfl_GroupEmployee.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbUser.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtClgCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Closed.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.panelFilter).EndInit();
		this.panelFilter.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.accordionControl1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitAccordion).EndInit();
		this.splitAccordion.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
