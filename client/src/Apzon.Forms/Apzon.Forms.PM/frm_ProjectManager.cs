using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Commons;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Editor.MyLookUpEdit;
using Apzon.Controls.Editor.TextEditExPrice;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Controls.Tree;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Administrator;
using Apzon.Forms.BusinessMaster;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Forms.Inventory.InventoryTransaction;
using Apzon.Forms.Properties;
using Apzon.Forms.Purchase;
using Apzon.Forms.Sales;
using Apzon.Forms.SalesOppotunity;
using Apzon.Forms.Service;
using Apzon.Forms.UserControls;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler.UI;
using DevExpress.XtraScheduler.Xml;
using DevExpress.XtraTab;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Data;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraTreeList.StyleFormatConditions;

namespace Apzon.Forms.PM;

public class frm_ProjectManager : XtraFormEx
{
	private enum FormModeType
	{
		Show,
		Hide
	}

	private const string OkString = "OK";

	private const string UpdateString = "Update";

	private const string AddString = "Add";

	private const string ShowString = "Show Gantt Chart";

	private const string HideString = "Hide Gantt Chart";

	private const string SelectString = "Select All";

	private const string UnSelectString = "UnSelect All";

	public Func<int, Task> SendDataBackToMain;

	private DataTable _dtDoc;

	private DataTable _dtTeam;

	private DataTable _dtDepen;

	private DataTable _dtItem;

	private DataTable _dtPayment;

	private DataTable _dtDocument;

	private DataTable _dtAddTeam;

	private DataTable _dtSub;

	private bool success;

	private bool _isUDF;

	private bool _isCpyWbs;

	private bool isChooseBom;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private bool _isLoading;

	private bool _isinout;

	private bool _isDb;

	private bool _addsub;

	private bool _isChangeItem;

	private bool _isOrder;

	private DataTable _dtOriginal = new DataTable();

	private Dictionary<int, string> dictionEmployee = new Dictionary<int, string>();

	private Dictionary<int, string> dictionUser = new Dictionary<int, string>();

	public List<Formular> Formulars = new List<Formular>();

	public static Dictionary<string, GridViewEx> TblFormular = new Dictionary<string, GridViewEx>();

	private List<CurrencyCodesImpl> CurrData;

	private readonly RepositoryItemLookUpEdit _typeD = new RepositoryItemLookUpEdit();

	private readonly RepositoryItemLookUpEdit _typeS = new RepositoryItemLookUpEdit();

	private readonly RepositoryItemLookUpEdit _typeC = new RepositoryItemLookUpEdit();

	private readonly RepositoryItemLookUpEdit _typeL = new RepositoryItemLookUpEdit();

	private readonly Dictionary<string, string> _lstValidD = new Dictionary<string, string>
	{
		{ "D", "Draft" },
		{ "S", "Started" }
	};

	private readonly Dictionary<string, string> _lstValidS = new Dictionary<string, string>
	{
		{ "S", "Started" },
		{ "P", "Pending" },
		{ "F", "Finished" }
	};

	private readonly Dictionary<string, string> _lstValidC = new Dictionary<string, string> { { "C", "Canceled" } };

	private readonly Dictionary<string, string> _lstValidL = new Dictionary<string, string> { { "L", "Closed" } };

	private readonly Dictionary<string, string> _lstStatus = new Dictionary<string, string>
	{
		{ "D", "Draft" },
		{ "S", "Started" },
		{ "P", "Pending" },
		{ "F", "Finished" },
		{ "C", "Canceled" },
		{ "L", "Closed" }
	};

	private readonly Dictionary<string, string> _lstPrio = new Dictionary<string, string>
	{
		{ "4", "Critical" },
		{ "3", "High" },
		{ "2", "Medium" },
		{ "1", "Low" }
	};

	private bool CancelItemChangeEvent;

	private FormModeType _formModeType;

	private bool isTemplateWbs;

	private int _hotTrackRow = int.MinValue;

	public Enums.ChangeLog ChangeLogType = Enums.ChangeLog.clProjectManager;

	private IContainer components;

	private SplitContainerControl splitContainerControl1;

	private GridControlEx grd_Doc;

	private GridViewEx gv_Doc;

	private GridColumnEx gridColumnEx1;

	private GridColumnEx gridColumnEx2;

	private GridColumnEx gridColumnEx3;

	private GridColumnEx gridColumnEx4;

	private GridColumnEx gridColumnEx5;

	private GridColumnEx gridColumnEx6;

	private GridColumnEx gridColumnEx7;

	private GridColumnEx gridColumnEx8;

	private GridColumnEx gridColumnEx9;

	private GridColumnEx gridColumnEx10;

	private GridColumnEx gridColumnEx11;

	private GridColumnEx gc_priority;

	private GridColumnEx gridColumnEx14;

	private GridColumnEx gridColumnEx15;

	private GridColumnEx gridColumnEx16;

	private GridColumnEx gridColumnEx17;

	private GridColumnEx gridColumnEx18;

	private GridColumnEx gridColumnEx19;

	private XtraTabControlEx xtra_Line;

	private XtraTabPageEx tab_Task;

	private XtraTabPageEx xtra_Team;

	private XtraTabPageEx xtra_Budget;

	private XtraTabPageEx xtra_Document;

	private XtraTabPageEx xtra_Sub;

	private SimpleButtonEx btn_Ok;

	private SimpleButtonEx btn_Cancel;

	private GridControlEx grd_LineTeam;

	private GridViewEx gv_LineTeam;

	private GridControlEx grd_LineDocument;

	private GridViewEx gv_LineDocument;

	private XtraTabControlEx xtra_LineBudget;

	private XtraTabPageEx xtra_LineItem;

	private GridColumnEx gridColumnEx47;

	private GridColumnEx gridColumnEx48;

	private GridColumnEx gridColumnEx49;

	private RepositoryItemLookUpEditSimple luk_Type;

	private RepositoryItemLookUpEditSimple luk_Status;

	private RepositoryItemButtonEditCFL btn_BpCode;

	private RepositoryItemButtonEditCFL btn_Manager;

	private RepositoryItemButtonEditCFL btn_Project;

	private RepositoryItemButtonEditCFL btn_Owner;

	private RepositoryItemButtonEditCFL btn_SaleOder;

	private RepositoryItemLookUpEditCombo luk_Industry;

	private RepositoryItemLookUpEditSimple luk_Priority;

	private GridColumnEx gridColumnEx50;

	private GridColumnEx gridColumnEx51;

	private GridColumnEx gridColumnEx52;

	private GridColumnEx gridColumnEx53;

	private GridColumnEx gridColumnEx54;

	private GridColumnEx gridColumnEx55;

	private GridColumnEx gridColumnEx56;

	private GridColumnEx gridColumnEx60;

	private GridColumnEx gridColumnEx62;

	private GridColumnEx gridColumnEx63;

	private SimpleButtonEx btn_Search;

	private ButtonEditCFL btn_Bp;

	private LookUpEditEx luk_Sta;

	private TextEditEx txt_Code;

	private LabelLine labelLine3;

	private LabelLine labelLine2;

	private LabelLine labelLine1;

	private RepositoryItemLookUpEditDefineNew luk_branch;

	private RepositoryItemLookUpEditDefineNew luk_dept;

	private RepositoryItemLookUpEditDefineNew luk_role;

	private ContextMenuStrip ctm_Doc;

	private ToolStripMenuItem addNewToolStripMenuItem;

	private ToolStripMenuItem deleteToolStripMenuItem;

	private ContextMenuStrip ctm_Rows;

	private ToolStripMenuItem addRowToolStripMenuItem;

	private ToolStripMenuItem deleteRowToolStripMenuItem;

	private ToolStripMenuItem addSubProjectToolStripMenuItem;

	private ToolStripMenuItem addSubProjectToolStripMenuItem1;

	private ToolStripMenuItem insertRowToolStripMenuItem;

	private ToolStripMenuItem duplicateRowToolStripMenuItem;

	private XtraTabPageEx xtra_Issue;

	private GridControlEx grd_lineissue;

	private GridViewEx gv_lineIssue;

	private GridColumnEx gridColumnEx13;

	private GridColumnEx gridColumnEx28;

	private GridColumnEx gridColumnEx69;

	private GridColumnEx gc_priorityy;

	private GridColumnEx gridColumnEx71;

	private GridColumnEx gridColumnEx72;

	private GridColumnEx gridColumnEx74;

	private GridColumnEx gridColumnEx78;

	private GridColumnEx gridColumnEx79;

	private GridColumnEx gridColumnEx80;

	private GridColumnEx gridColumnEx81;

	private GridColumnEx gridColumnEx82;

	private GridColumnEx gridColumnEx84;

	private GridColumnEx gridColumnEx85;

	private GridColumnEx gridColumnEx86;

	private GridColumnEx gridColumnEx87;

	private GridColumnEx gridColumnEx88;

	private RepositoryItemButtonEditCFL btn_Id;

	private RepositoryItemLookUpEditDefineNew luk_priorityGrid;

	private RepositoryItemButtonEditCFL btn_report;

	private RepositoryItemButtonEditCFL btn_handle;

	private RepositoryItemButtonEditCFL btn_solution;

	private RepositoryItemLookUpEditSimple luk_statusGrid;

	private RepositoryItemLookUpEditDefineNew luk_TypeGrid;

	private GridColumnEx gridColumnEx89;

	private GridColumnEx gridColumnEx90;

	private RepositoryItemButtonEditCFL btn_FinProject;

	private SimpleButtonEx btn_ShowGantt;

	private CheckedListBoxControl chk_Action;

	private ResourcesCheckedListBoxControl chk_Users;

	private LabelControl labelControl1;

	private DateNavigator dateNavigator1;

	private SchedulerStorage schedulerStorage1;

	private SchedulerControl schedulerControl1;

	private ToolTipController toolTipController1;

	private ToolStripMenuItem newTaskInDoc;

	private ToolStripMenuItem newEvenToolStripMenuItem;

	private ToolStripMenuItem newTaskToolStripMenuItem1;

	private ToolStripMenuItem newEventToolStripMenuItem;

	private RepositoryItemTextEdit txt_prjCode;

	private TreeEx tv_Wbs;

	private TreeListColumnEx tl_Entry;

	private TreeListColumnEx tl_Wbs;

	private TreeListColumnEx tl_Code;

	private TreeListColumnEx tl_Name;

	private TreeListColumnEx tl_FromDate;

	private TreeListColumnEx tl_ToDate;

	private TreeListColumnEx tl_Priority;

	private TreeListColumnEx tl_Milestone;

	private TreeListColumnEx tl_Notes;

	private TreeListColumnEx tl_Status;

	private TreeListColumnEx tl_Dependent;

	private TreeListColumnEx tl_Resource;

	private RepositoryItemButtonEditCFL btn_resource;

	private RepositoryItemLookUpEditSimple luk_taskPriority;

	private RepositoryItemLookUpEditSimple luk_Milestone;

	private RepositoryItemLookUpEditSimple luk_taskStatus;

	private TreeListColumnEx tl_LineNum;

	private TreeListColumnEx tl_IsExist;

	private RepositoryItemButtonEdit btn_NewTask;

	private TreeListColumnEx tl_FatherNum;

	private DateEditEx date_StartFrom;

	private DateEditEx date_StartTo;

	private LabelLine labelLine5;

	private LabelLine labelLine4;

	private ToolStripMenuItem newTrackingIssueToolStripMenuItem;

	private ToolStripMenuItem newTrackingIssueToolStripMenuItem1;

	private RepositoryItemButtonEdit txt_TaskCode;

	private RepositoryItemDateEditEx date_fromdate;

	private RepositoryItemDateEditEx date_todate;

	private ToolStripSeparator toolStripSeparator2;

	private ToolStripSeparator toolStripSeparator1;

	private ToolStripSeparator toolStripSeparator3;

	private ToolStripSeparator toolStripSeparator5;

	private ToolStripSeparator toolStripSeparator4;

	private ToolStripSeparator toolStripSeparator6;

	private ToolStripMenuItem toolStripMenuItemIndent;

	private ToolStripMenuItem toolStripMenuItemOutdent;

	private ToolStripSeparator toolStripSeparator7;

	private ToolStripSeparator toolStripSeparator8;

	private ToolStripMenuItem viewOrgChartToolStripMenuItem;

	private SplitContainerControl splitContainerControl4;

	private SplitContainerControl splitContainerControl5;

	private ContextMenuStrip ctm_BudgetItem;

	private ToolStripMenuItem addRowToolStripMenuItem1;

	private ToolStripMenuItem toolStripMenuItemInsertRowBudget;

	private ToolStripSeparator toolStripSeparator10;

	private ToolStripMenuItem toolStripMenuItemDuplicateRowBudget;

	private ToolStripSeparator toolStripSeparator9;

	private ToolStripMenuItem removeRowToolStripMenuItem;

	private ContextMenuStrip ctm_Sub;

	private ToolStripMenuItem toolStripMenuItemAddSub;

	private ContextMenuStrip ctm_Issues;

	private ToolStripMenuItem toolStripMenuItemAddNew;

	private XtraTabPageEx tab_TaskList;

	private ContextMenuStrip ctm_TaskList;

	private ToolStripMenuItem toolStripMenuItemNewTask;

	private RepositoryItemButtonEditCFL btn_IssueBp;

	private RepositoryItemButtonEditCFL btn_IssuePr;

	private RepositoryItemButtonEditCFL btn_IssueSLBy;

	private RepositoryItemButtonEditCFL btn_IssueTaskCode;

	private RepositoryItemButtonEditCFL btn_IssuwPrOwner;

	private ToolStripMenuItem relatedActivitiesToolStripMenuItem;

	private TreeListColumnEx tl_visOrder;

	private TreeListColumnEx tb_AbsEntry;

	private TreeListColumnEx tb_LineNum;

	private TreeListColumnEx tb_ItemCode;

	private RepositoryItemButtonEditCFL btn_ItemCode;

	private TreeListColumnEx tb_ItemName;

	private TreeListColumnEx tb_TaskCode;

	private RepositoryItemButtonEditCFL btn_TaskCode;

	private TreeListColumnEx tb_TaskName;

	private TreeListColumnEx tb_plQty;

	private TreeListColumnEx tb_RefPrice;

	private TreeListColumnEx tb_Amount;

	private TreeListColumnEx tb_Branch;

	private RepositoryItemLookUpEditDefineNew luk_ItemBranch;

	private TreeListColumnEx tb_ActualQty;

	private TreeListColumnEx tb_QtyDiff;

	private TreeListColumnEx tb_Notes;

	private TreeListColumnEx tb_FatherNum;

	private GridColumnEx gridColumnEx24;

	private RepositoryItemButtonEdit btn_AbsEntry;

	private RepositoryItemTextEdit txt_budgetLevel;

	private TreeListColumnEx tb_IsNew;

	private RepositoryItemTextEditExPrice txt_RefPrice;

	private TreeListColumnEx tb_ActualAmount;

	private TreeListColumnEx tb_FromDate;

	private TreeListColumnEx tb_ToDate;

	private RepositoryItemDateEditEx dat_IFromDate;

	private RepositoryItemDateEditEx dat_IToDate;

	private DropDownButton drp_CrDocumnet;

	private XtraTabPageEx xtra_Payment;

	private TreeEx tv_Payments;

	private TreeListColumnEx tp_AbsEntry;

	private TreeListColumnEx tp_TaskCode;

	private TreeListColumnEx tp_TaskName;

	private TreeListColumnEx tp_InvoicePlan;

	private TreeListColumnEx tp_IncomingPayment;

	private TreeListColumnEx tp_Payments;

	private TreeListColumnEx tp_Duedate;

	private TreeListColumnEx tp_Remarks;

	private TreeListColumnEx tp_LineNum;

	private TreeListColumnEx tp_FatherNum;

	private TreeListColumnEx tb_BomItem;

	private RepositoryItemButtonEditCFL btn_BOM;

	private TreeListColumnEx tb_OrderItem;

	private RepositoryItemButtonEditCFL btn_OrderItem;

	private TreeListColumnEx tb_PR;

	private TreeListColumnEx tb_PO;

	private TreeListColumnEx tb_GRPO;

	private TreeListColumnEx tb_Delivery;

	private TreeListColumnEx tb_Invoice;

	private TreeListColumnEx tb_UoM;

	private RepositoryItemLookUpEditDefineNew luk_UoM;

	private ContextMenuStrip ctm_Payments;

	private ToolStripMenuItem toolStripMenuItem2;

	private ToolStripMenuItem toolStripMenuItem3;

	private ToolStripSeparator toolStripSeparator11;

	private ToolStripMenuItem toolStripMenuItem4;

	private ToolStripSeparator toolStripSeparator12;

	private ToolStripMenuItem toolStripMenuItem5;

	private TreeListColumnEx tp_IsNew;

	private ToolStripMenuItem addItemRowToolStripMenuItem;

	private ToolStripMenuItem insertItemRowToolStripMenuItem;

	private TreeListColumnEx tl_BaseLine;

	private TreeListColumnEx tb_BaseLine;

	private TreeListColumnEx tp_BaseLine;

	private TreeListColumnEx tb_AR;

	private RepositoryItemButtonEdit btn_PR;

	private RepositoryItemButtonEdit btn_PO;

	private RepositoryItemButtonEdit btn_GRPO;

	private RepositoryItemButtonEdit btn_Delivery;

	private RepositoryItemButtonEdit btn_APInvoice;

	private RepositoryItemButtonEdit btn_ARInvoice;

	private TreeListColumnEx tb_BaseQty;

	private TreeListColumnEx tb_ItmsGrpCod;

	private RepositoryItemLookUpEdit luk_ItemGroup;

	private RepositoryItemLookUpEditDefineNew luk_ItmsGrpCod;

	private GridColumnEx gridColumnEx25;

	private RepositoryItemButtonEditCFL btn_DocNumDo;

	private GridColumnEx gridColumnEx26;

	private GridColumnEx gridColumnEx27;

	private GridColumnEx gridColumnEx29;

	private ToolStripMenuItem canceledToolStripMenuItem;

	private ToolStripMenuItem closedToolStripMenuItem;

	private TreeEx tv_budget;

	private SimpleButtonEx btn_Select;

	private RepositoryItemMemoExEdit repositoryItemMemoExEdit1;

	private RepositoryItemMemoExEdit repositoryItemMemoExEdit2;

	private RepositoryItemMemoExEdit repositoryItemMemoExEdit3;

	private GridColumnEx gridColumnEx30;

	private RepositoryItemButtonEditCFL btn_Whs;

	private TreeListColumnEx tb_DefaultWhs;

	private TreeListColumnEx tb_VisOrder;

	private TreeListColumnEx tp_VisOrder;

	private RepositoryItemButtonEditCFL btn_WbsPayment;

	private ToolStripMenuItem viewDiagramToolStripMenuItem;

	private LabelLine labelLine6;

	private TextEditEx txt_No;

	private RepositoryItemCalcEdit cal_PlanQty;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx1;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx2;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx3;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx4;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx5;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx6;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx7;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx8;

	private RepositoryItemButtonEditCFL btn_ResourceEmployee;

	private RepositoryItemButtonEditCFL btn_WarehouseItem;

	private GridColumnEx gridColumnEx31;

	private TreeListColumnEx tb_Currency;

	private RepositoryItemLookUpEdit luk_DocCurr;

	private RepositoryItemLookUpEdit luk_Currency;

	private GridColumnEx gridColumnEx32;

	private RepositoryItemLookUpEdit luk_CurrencyLineDu;

	private TreeListColumnEx tb_PQ;

	protected DataTable dataTable12;

	private RepositoryItemButtonEdit btn_PQ;

	private ButtonEditCFL btn_OwnerSearch;

	private LabelLine labelLine7;

	private TreeListColumnEx tb_ARDR;

	private TreeListColumnEx tb_APDR;

	private RepositoryItemButtonEditCFL btn_WBSTemplate;

	private ToolStripMenuItem genTasksToolStripMenuItem;

	private TreeListColumnEx tl_WbsTemplate;

	private GridColumnEx gridColumnEx33;

	private GridColumnEx gridColumnEx34;

	private GridColumnEx gridColumnEx35;

	private RepositoryItemDateEditEx repositoryItemDateEditEx1;

	private GridColumnEx gridColumnEx36;

	private RepositoryItemTimeSpanEdit repositoryItemTimeSpanEdit1;

	private GridColumnEx gridColumnEx37;

	private RepositoryItemDateEditEx repositoryItemDateEditEx2;

	private GridColumnEx gridColumnEx38;

	private RepositoryItemTimeSpanEdit repositoryItemTimeSpanEdit2;

	private GridControlEx grd_LineSub;

	private GridViewEx gv_LineSub;

	private GridColumnEx gridColumnEx39;

	private GridColumnEx gridColumnEx40;

	private RepositoryItemTextEdit repositoryItemTextEdit1;

	private GridColumnEx gridColumnEx41;

	private GridColumnEx gridColumnEx42;

	private RepositoryItemLookUpEditSimple luk_SubType;

	private GridColumnEx gridColumnEx43;

	private RepositoryItemLookUpEditSimple luk_SubStatus;

	private GridColumnEx gridColumnEx44;

	private RepositoryItemButtonEditCFL btn_SubBpCode;

	private GridColumnEx gridColumnEx45;

	private GridColumnEx gridColumnEx46;

	private RepositoryItemButtonEditCFL btn_SubPM;

	private GridColumnEx gridColumnEx57;

	private GridColumnEx gridColumnEx58;

	private GridColumnEx gridColumnEx59;

	private GridColumnEx gridColumnEx61;

	private RepositoryItemLookUpEditSimple luk_SubPriority;

	private GridColumnEx gridColumnEx64;

	private RepositoryItemButtonEditCFL btn_SubMasterProject;

	private GridColumnEx gridColumnEx65;

	private GridColumnEx gridColumnEx66;

	private RepositoryItemLookUpEditCombo repositoryItemLookUpEditCombo1;

	private GridColumnEx gridColumnEx67;

	private RepositoryItemButtonEditCFL btn_SubOwner;

	private GridColumnEx gridColumnEx68;

	private RepositoryItemButtonEditCFL btn_SubSalesOrder;

	private GridColumnEx gridColumnEx70;

	private RepositoryItemMemoExEdit repositoryItemMemoExEdit4;

	private GridColumnEx gridColumnEx73;

	private GridColumnEx gridColumnEx75;

	private GridColumnEx gridColumnEx76;

	private GridColumnEx gridColumnEx77;

	private RepositoryItemButtonEditCFL btn_SubFinalProject;

	private GridColumnEx gridColumnEx83;

	private RepositoryItemTextEdit repositoryItemTextEdit2;

	private GridColumnEx gridColumnEx91;

	private RepositoryItemButtonEditCFL btn_SubWhs;

	private GridColumnEx gridColumnEx92;

	private RepositoryItemLookUpEdit luk_SubCurrency;

	private GridColumnEx gridColumnEx93;

	private GridColumnEx gridColumnEx94;

	private GridColumnEx gridColumnEx95;

	private RepositoryItemDateEditEx repositoryItemDateEditEx3;

	private GridColumnEx gridColumnEx96;

	private RepositoryItemTimeSpanEdit repositoryItemTimeSpanEdit3;

	private GridColumnEx gridColumnEx97;

	private RepositoryItemDateEditEx repositoryItemDateEditEx4;

	private GridColumnEx gridColumnEx98;

	private RepositoryItemTimeSpanEdit repositoryItemTimeSpanEdit4;

	private GridColumnEx gridColumnEx99;

	private RepositoryItemButtonEditCFL btn_Code;

	private GridColumnEx gridColumnEx100;

	private TreeListColumnEx tb_IGE;

	private RepositoryItemButtonEditCFL btn_IGE;

	private SimpleButtonEx btn_GanttView;

	private XtraTabPageEx xtra_NoteList;

	private XtraTabPageEx xtra_EventList;

	private GridControlEx grd_NoteList;

	private GridViewEx gv_NoteList;

	private GridControlEx grd_EventList;

	private GridViewEx gv_EventList;

	private GridColumnEx cl_ClgCode;

	private GridColumnEx cl_Details;

	private GridColumnEx gc_AssignTo;

	private GridColumnEx NoteDate;

	private GridColumnEx NoteTime;

	private GridColumnEx ClgCode;

	private GridColumnEx Details;

	private GridColumnEx CardCode;

	private GridColumnEx Recontact;

	private GridColumnEx endDate;

	private GridColumnEx AllDayEvent;

	private GridColumnEx AssignTo;

	private GridColumnEx CntctDate;

	private GridColumnEx BeginTime;

	private GridColumnEx ENDTime;

	private GridColumnEx gridColumnEx103;

	private TreeEx tv_ListCompleted;

	private TreeListColumnEx tl_RowId;

	private TreeListColumnEx tl_ClgCode;

	private TreeListColumnEx tl_Closed_Completed;

	private TreeListColumnEx tl_Details;

	private TreeListColumnEx tl_ProjectWBS_Completed;

	private TreeListColumnEx tl_AssignTo_Completed;

	private TreeListColumnEx tl_DueDate_Completed;

	private TreeListColumnEx tl_ProjectEntry_Completed;

	private TreeListColumnEx tl_OrderLine;

	private TreeListColumnEx tl_CntctDate;

	private TreeListColumnEx tl_ENDTime;

	private TreeListColumnEx tl_CloseDate_Completed;

	private TreeListColumnEx tl_Priority_Completed;

	private RepositoryItemButtonEdit rep_btn_Closed;

	private RepositoryItemButtonEdit rep_btn_Open;

	private RepositoryItemButtonEdit rep_btn_0;

	private RepositoryItemButtonEdit rep_btn_1;

	private RepositoryItemButtonEdit rep_btn_2;

	private RepositoryItemButtonEdit rep_btn_3;

	private RepositoryItemLookUpEdit luk_prioritylistcompleted;

	private TreeListColumnEx tl_WBSMode;

	private RepositoryItemLookUpEditSimple luk_WBSMode;

	private TreeListColumnEx tl_Duration;

	private ToolStripMenuItem duplicateToolStripMenuItem;

	private GridColumnEx gridColumnEx12;

	private RepositoryItemLookUpEditSimple luk_WddStatus;

	private GridColumnEx gridColumnEx20;

	private SimpleButtonEx btn_Reject;

	private SimpleButtonEx btn_Approval;

	private ToolStripMenuItem duplicateDoc;

	private GridColumnEx gridColumnEx21;

	private TreeListColumnEx treeListColumnEx1;

	private RepositoryItemButtonEditCFL btn_WOR;

	private GridColumnEx gv_LoadBom;

	private RepositoryItemLookUpEditSimple look_LoadBom;

	private GridColumnEx gridColumnEx22;

	private RepositoryItemButtonEdit rep_CommentsCount;

	private TreeListColumnEx treeListColumnEx2;

	private RepositoryItemButtonEdit rep_WbsComments;

	private TreeListColumnEx treeListColumnEx3;

	private RepositoryItemButtonEdit rep_BudgetComments;

	private GridColumnEx gridColumnEx23;

	private GridColumnEx gridColumnEx101;

	private RepositoryItemButtonEditCFL btn_BaseProject;

	private GridColumnEx gridColumnEx102;

	private RepositoryItemLookUpEditSimple luk_BaseLine;

	private GridColumnEx gridColumnEx104;

	private RepositoryItemLookUpEditSimple luk_BaseLineActive;

	private SimpleButtonEx btn_UndoApproval;

	public List<ApzObjectTypes> PopCopyFrom { get; set; }

	public int ProjectId { get; set; }

	private FormModeType FormMode_Type
	{
		get
		{
			return _formModeType;
		}
		set
		{
			_formModeType = value;
			try
			{
				switch (_formModeType)
				{
				case FormModeType.Show:
					btn_ShowGantt.Text = "Show Gantt Chart";
					break;
				case FormModeType.Hide:
					btn_ShowGantt.Text = "Hide Gantt Chart";
					break;
				}
			}
			catch (Exception ex)
			{
				WinCommons.ErrorLineNumber(ex);
			}
		}
	}

	private int HotTrackRow
	{
		get
		{
			return _hotTrackRow;
		}
		set
		{
			try
			{
				if (_hotTrackRow != value)
				{
					int hotTrackRow = _hotTrackRow;
					_hotTrackRow = value;
					if (gv_Doc.ActiveEditor != null)
					{
						gv_Doc.PostEditor();
					}
					gv_Doc.RefreshRow(hotTrackRow);
					gv_Doc.RefreshRow(_hotTrackRow);
					if (_hotTrackRow >= 0)
					{
						grd_Doc.Cursor = Cursors.Hand;
					}
					else
					{
						grd_Doc.Cursor = Cursors.Default;
					}
				}
			}
			catch (Exception)
			{
			}
		}
	}

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		try
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
			case Enums.FormViewModeType.Add:
				btn_Ok.Text = "Add";
				duplicateRowToolStripMenuItem.Enabled = true;
				addNewToolStripMenuItem.Enabled = false;
				deleteToolStripMenuItem.Enabled = false;
				txt_prjCode.ReadOnly = false;
				newTaskToolStripMenuItem1.Enabled = false;
				newEventToolStripMenuItem.Enabled = false;
				newTrackingIssueToolStripMenuItem1.Enabled = false;
				toolStripMenuItemAddNew.Enabled = false;
				toolStripMenuItemNewTask.Enabled = false;
				newEvenToolStripMenuItem.Enabled = false;
				newTaskInDoc.Enabled = false;
				newTrackingIssueToolStripMenuItem.Enabled = false;
				relatedActivitiesToolStripMenuItem.Enabled = false;
				genTasksToolStripMenuItem.Enabled = false;
				duplicateToolStripMenuItem.Enabled = false;
				break;
			case Enums.FormViewModeType.Update:
				btn_Ok.Text = "Update";
				duplicateRowToolStripMenuItem.Enabled = true;
				addNewToolStripMenuItem.Enabled = true;
				deleteToolStripMenuItem.Enabled = true;
				txt_prjCode.ReadOnly = true;
				newTaskToolStripMenuItem1.Enabled = true;
				newEventToolStripMenuItem.Enabled = true;
				newTrackingIssueToolStripMenuItem1.Enabled = true;
				toolStripMenuItemAddNew.Enabled = true;
				toolStripMenuItemNewTask.Enabled = true;
				newEvenToolStripMenuItem.Enabled = true;
				newTaskInDoc.Enabled = true;
				newTrackingIssueToolStripMenuItem.Enabled = true;
				relatedActivitiesToolStripMenuItem.Enabled = true;
				genTasksToolStripMenuItem.Enabled = false;
				duplicateToolStripMenuItem.Enabled = false;
				break;
			case Enums.FormViewModeType.View:
				duplicateRowToolStripMenuItem.Enabled = true;
				addNewToolStripMenuItem.Enabled = true;
				deleteToolStripMenuItem.Enabled = true;
				btn_Ok.Text = "OK";
				txt_prjCode.ReadOnly = true;
				newTaskToolStripMenuItem1.Enabled = true;
				newEventToolStripMenuItem.Enabled = true;
				newTrackingIssueToolStripMenuItem1.Enabled = true;
				toolStripMenuItemAddNew.Enabled = true;
				toolStripMenuItemNewTask.Enabled = true;
				newEvenToolStripMenuItem.Enabled = true;
				newTaskInDoc.Enabled = true;
				newTrackingIssueToolStripMenuItem.Enabled = true;
				relatedActivitiesToolStripMenuItem.Enabled = true;
				genTasksToolStripMenuItem.Enabled = true;
				duplicateToolStripMenuItem.Enabled = true;
				break;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public frm_ProjectManager()
	{
		InitializeComponent();
		try
		{
			tv_Wbs.ItemId = "APZ_PRG1";
			splitContainerControl4.PanelVisibility = SplitPanelVisibility.Panel1;
			schedulerControl1.BeginUpdate();
			schedulerControl1.Start = DateTime.Now;
			schedulerControl1.ResourceColorSchemas.Clear();
			SchedulerColorSchema value = new SchedulerColorSchema(Color.White);
			schedulerControl1.ResourceColorSchemas.Add(value);
			toolTipController1.ToolTipType = ToolTipType.SuperTip;
			schedulerControl1.EndUpdate();
			chk_Action.ValueMember = "Action";
			chk_Action.DisplayMember = "ActionName";
			chk_Action.CheckMember = "IsChecked";
			OnLoadAsync += frm_ProjectManager_Load;
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL = btn_Code;
			repositoryItemButtonEditCFL.AfterShowCFL = (Action<frm_CFL>)Delegate.Combine(repositoryItemButtonEditCFL.AfterShowCFL, (Action<frm_CFL>)((frm_CFL frm) =>
			{
				try
				{
					DataTable dataTable = frm.gv_CFL.GridControl.DataSource as DataTable;
					string type = Function.ToString(gv_Doc.GetFocusedRowCellValue("ProjectType"));
					string code = Function.ToString(gv_Doc.GetFocusedRowCellValue("Code"));
					if (dataTable.Rows.Count > 0 && dataTable != null)
					{
						DataRow[] source = (from s in dataTable.AsEnumerable()
							where Function.ToString(s["ProjectType"]) == type && Function.ToString(s["Code"]) != code
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
				catch (Exception ex2)
				{
					Logging.Write("ERROR", ex2);
					WinCommons.ErrorLineNumber(ex2);
				}
			}));
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL2 = btn_Project;
			repositoryItemButtonEditCFL2.AfterShowCFL = (Action<frm_CFL>)Delegate.Combine(repositoryItemButtonEditCFL2.AfterShowCFL, (Action<frm_CFL>)((frm_CFL frm) =>
			{
				try
				{
					DataTable dataTable = frm.gv_CFL.GridControl.DataSource as DataTable;
					string type = Function.ToString(gv_Doc.GetFocusedRowCellValue("ProjectType"));
					string code = Function.ToString(gv_Doc.GetFocusedRowCellValue("Code"));
					if (dataTable.Rows.Count > 0 && dataTable != null)
					{
						DataRow[] source = (from s in dataTable.AsEnumerable()
							where Function.ToString(s["ProjectType"]) == type && Function.ToString(s["Code"]) != code
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
				catch (Exception ex2)
				{
					Logging.Write("ERROR", ex2);
					WinCommons.ErrorLineNumber(ex2);
				}
			}));
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL3 = btn_SaleOder;
			repositoryItemButtonEditCFL3.AfterShowCFL = (Action<frm_CFL>)Delegate.Combine(repositoryItemButtonEditCFL3.AfterShowCFL, (Action<frm_CFL>)((frm_CFL frm) =>
			{
				try
				{
					DataTable dataTable = frm.gv_CFL.GridControl.DataSource as DataTable;
					string bpCode = Function.ToString(gv_Doc.GetFocusedRowCellValue("CardCode"));
					if (dataTable.Rows.Count > 0 && dataTable != null)
					{
						DataRow[] source = (from s in dataTable.AsEnumerable()
							where Function.ToString(s["CardCode"]) == bpCode && Function.ToString(s["DocStatus"]) == "O"
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
				catch (Exception ex2)
				{
					WinCommons.ErrorLineNumber(ex2);
				}
			}));
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL4 = btn_TaskCode;
			repositoryItemButtonEditCFL4.AfterShowCFL = (Action<frm_CFL>)Delegate.Combine(repositoryItemButtonEditCFL4.AfterShowCFL, (Action<frm_CFL>)((frm_CFL frm) =>
			{
				try
				{
					DataRow[] source = (from s in tv_Wbs.DataSourceTable().AsEnumerable()
						where Function.ToString(s["TaskCode"]) != ""
						select s).ToArray();
					if (source.Count() > 0)
					{
						frm.gv_CFL.GridControl.DataSource = source.CopyToDataTable();
					}
				}
				catch (Exception ex2)
				{
					Logging.Write("ERROR", ex2);
					WinCommons.ErrorLineNumber(ex2);
				}
			}));
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL5 = btn_WbsPayment;
			repositoryItemButtonEditCFL5.AfterShowCFL = (Action<frm_CFL>)Delegate.Combine(repositoryItemButtonEditCFL5.AfterShowCFL, (Action<frm_CFL>)((frm_CFL frm) =>
			{
				try
				{
					DataRow[] source = (from s in tv_Wbs.DataSourceTable().AsEnumerable()
						where Function.ToString(s["TaskCode"]) != ""
						select s).ToArray();
					if (source.Count() > 0)
					{
						frm.gv_CFL.GridControl.DataSource = source.CopyToDataTable();
					}
				}
				catch (Exception ex2)
				{
					Logging.Write("ERROR", ex2);
					WinCommons.ErrorLineNumber(ex2);
				}
			}));
			btn_resource.CustomHandleChooseItem += btn_resource_CustomHandleChooseItem;
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_ProjectManager" && x.authoriza == "F"))
			{
				deleteToolStripMenuItem.Enabled = false;
				canceledToolStripMenuItem.Enabled = false;
				closedToolStripMenuItem.Enabled = false;
			}
			else
			{
				deleteToolStripMenuItem.Enabled = true;
				canceledToolStripMenuItem.Enabled = true;
				closedToolStripMenuItem.Enabled = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_resource_CustomHandleChooseItem(object sender, ButtonEditCFLEventArgs e)
	{
		try
		{
			if (!(sender is ButtonEditCFL buttonEditCFL) || e == null)
			{
				return;
			}
			List<DataRow> chooseItems = e.ChooseItems;
			List<string> list = new List<string>();
			if (chooseItems != null)
			{
				foreach (DataRow item2 in chooseItems)
				{
					string item = Function.ToString(item2["EmpId"]);
					list.Add(item);
				}
				buttonEditCFL.EditValue = string.Join(";", list);
			}
			await SetDataTeamByResource(Function.ToString(buttonEditCFL.EditValue));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public frm_ProjectManager(DataTable doc, DataTable task, DataTable team, DataTable item, DataTable payment, DataTable document, DataTable sub, DataTable pen, DataTable newteam, int id)
		: this()
	{
		try
		{
			_addsub = true;
			grd_Doc.DataSource = doc;
			tv_Wbs.DataSource = task;
			tv_budget.DataSource = item;
			tv_Payments.DataSource = payment;
			grd_LineDocument.DataSource = document;
			grd_LineSub.DataSource = sub;
			grd_LineTeam.DataSource = team;
			_dtDepen = pen;
			_dtAddTeam = newteam;
			base.FormMode = Enums.FormViewModeType.Add;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task frm_ProjectManager_Load(object sender, EventArgs e)
	{
		_ = 6;
		try
		{
			_isLoading = true;
			base.FormMode = Enums.FormViewModeType.View;
			await LoadForm();
			SetDataCurrency();
			LoadInItForm();
			LoadCopyMenu();
			luk_TypeGrid.ValueMember = "Code";
			luk_TypeGrid.DisplayMember = "Name";
			luk_TypeGrid.FormProperties.TableName = "APZ_IETE";
			await luk_TypeGrid.RefreshDataSourceAsync();
			luk_branch.ValueMember = "Code";
			luk_branch.DisplayMember = "Name";
			luk_branch.FormProperties.TableName = "OUBR";
			luk_ItemBranch.ValueMember = "Code";
			luk_ItemBranch.DisplayMember = "Name";
			luk_ItemBranch.FormProperties.TableName = "OUBR";
			luk_dept.ValueMember = "Code";
			luk_dept.DisplayMember = "Name";
			luk_dept.FormProperties.TableName = "OUDP";
			luk_role.ValueMember = "typeID";
			luk_role.DisplayMember = "name";
			luk_role.FormProperties.TableName = "ohty";
			luk_ItmsGrpCod.ValueMember = "ItmsGrpCod";
			luk_ItmsGrpCod.DisplayMember = "ItmsGrpNam";
			luk_ItmsGrpCod.FormProperties.TableName = "OITB";
			await Task.WhenAll(luk_branch.RefreshDataSourceAsync(), luk_ItemBranch.RefreshDataSourceAsync(), luk_dept.RefreshDataSourceAsync(), luk_role.RefreshDataSourceAsync(), luk_ItmsGrpCod.RefreshDataSourceAsync());
			luk_statusGrid.SetDataSource(new Dictionary<string, string>
			{
				{ "1", "New" },
				{ "2", "Assigned" },
				{ "3", "In Progress" },
				{ "4", "FeedBack" },
				{ "5", "Confirmed" },
				{ "6", "Resolved" },
				{ "7", "Rejected" },
				{ "8", "Closed" }
			});
			luk_Type.SetDataSource(new Dictionary<string, string>
			{
				{ "I", "Internal " },
				{ "E", "External " }
			});
			luk_SubType.SetDataSource(new Dictionary<string, string>
			{
				{ "I", "Internal " },
				{ "E", "External " }
			});
			luk_Status.SetDataSource(_lstStatus);
			luk_SubStatus.SetDataSource(_lstStatus);
			luk_taskStatus.SetDataSource(_lstStatus);
			luk_SubStatus.SetDataSource(_lstStatus);
			luk_Sta.Properties.SetDataSource(new Dictionary<string, string>
			{
				{ "", "" },
				{ "D", "Draft" },
				{ "S", "Started" },
				{ "P", "Pending" },
				{ "F", "Finished" },
				{ "C", "Canceled" },
				{ "L", "Closed" }
			});
			luk_Priority.SetDataSource(_lstPrio);
			luk_SubPriority.SetDataSource(_lstPrio);
			luk_taskPriority.SetDataSource(_lstPrio);
			luk_priorityGrid.SetDataSource(_lstPrio);
			luk_Milestone.SetDataSource(new Dictionary<string, string>
			{
				{ "Y", "Yes" },
				{ "N", "No" }
			});
			luk_WBSMode.SetDataSource(new Dictionary<string, string>
			{
				{ "A", "Auto" },
				{ "M", "Manual" }
			});
			luk_WddStatus.SetDataSource(new Dictionary<string, string>
			{
				{ "-", "Không cần duyệt" },
				{ "W", "Chờ duyệt" },
				{ "P", "Đã duyệt" },
				{ "N", "Từ chối" }
			});
			look_LoadBom.SetDataSource(new Dictionary<string, string>
			{
				{ "Y", "Yes" },
				{ "N", "No" }
			});
			Formulars = await WinFuntions.GetFormulas((int)FormObjectType);
			Dictionary<string, GridViewEx> tblFormular = new Dictionary<string, GridViewEx>
			{
				["APZ_OPRG"] = gv_Doc,
				["APZ_TGIE"] = gv_lineIssue
			};
			Dictionary<string, ViewEditModel> formulaList = new Dictionary<string, ViewEditModel>
			{
				["APZ_OPRG"] = new ViewEditModel(gv_Doc),
				["APZ_TGIE"] = new ViewEditModel(gv_lineIssue),
				["APZ_PRG1"] = new ViewEditModel(tv_Wbs),
				["APZ_PRG4"] = new ViewEditModel(tv_budget),
				["APZ_PRG6"] = new ViewEditModel(tv_Payments)
			};
			await Task.WhenAll(WinFuntions.LoadUdfColumns(gv_Doc, "APZ_OPRG", tblFormular), WinFuntions.LoadUdfColumns(gv_LineSub, "APZ_OPRG"), WinFuntions.LoadUdfColumns(gv_lineIssue, "APZ_TGIE", tblFormular), tv_Wbs.LoadUdfColumns("APZ_PRG1", formulaList), tv_budget.LoadUdfColumns("APZ_PRG4", formulaList), tv_Payments.LoadUdfColumns("APZ_PRG6", formulaList));
			if (_addsub)
			{
				base.FormMode = Enums.FormViewModeType.Add;
				return;
			}
			DataSet dataSet = await _httpClient.Cfl.GetUserList();
			new DataTable();
			DataTable dataTable = ((!dataSet.IsHasStructure()) ? dataSet.Tables[0] : dataSet.Tables[1]);
			if (dataTable.IsNotNull() && dataTable.ExistsColumn("USERID") && dataTable.ExistsColumn("U_NAME"))
			{
				dictionUser = dataTable.AsEnumerable().ToDictionary((DataRow s) => Function.ParseInt(s["USERID"]), (DataRow s) => Function.ToString(s["U_NAME"]));
			}
			DataSet employeeListForSalesEmpSetup = _httpClient.Cfl.GetEmployeeListForSalesEmpSetup();
			new DataTable();
			DataTable dataTable2 = ((!employeeListForSalesEmpSetup.IsHasStructure()) ? employeeListForSalesEmpSetup.Tables[0] : employeeListForSalesEmpSetup.Tables[1]);
			if (dataTable2.IsNotNull() && dataTable2.ExistsColumn("empID") && dataTable2.ExistsColumn("lastName") && dataTable2.ExistsColumn("firstName"))
			{
				dictionEmployee = dataTable2.AsEnumerable().ToDictionary((DataRow s) => Function.ParseInt(s["empID"]), (DataRow s) => Function.ToString(s["firstName"]) + "," + Function.ToString(s["lastName"]));
			}
			date_StartFrom.EditValue = DateTime.Today.AddDays(-Configuration.SearchDate);
			date_StartTo.EditValue = DateTime.Today.AddDays(Configuration.SearchDate);
			string startFrom = (Function.ParseDateTime(date_StartFrom.EditValue).HasValue ? ((DateTime)date_StartFrom.EditValue).ToString("yyyyMMdd") : "");
			string startTo = (Function.ParseDateTime(date_StartTo.EditValue).HasValue ? ((DateTime)date_StartTo.EditValue).ToString("yyyyMMdd") : "");
			if (ProjectId > 0)
			{
				txt_No.EditValue = ProjectId;
			}
			DataTable dataTable3 = await LoadData(Function.ParseInt(txt_No.EditValue), "", "", "", startFrom, startTo);
			ProjectId = 0;
			grd_Doc.DataSource = dataTable3;
			grd_Doc.DatasourceTable().AcceptChanges();
			if (!dataTable3.IsNotNull())
			{
				gv_Doc_FocusedRowObjectChanged(null, null);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		_isLoading = false;
	}

	private void LoadInItForm()
	{
		try
		{
			List<Common.Table_Code> dataSource = _lstValidD.Select((KeyValuePair<string, string> item) => new Common.Table_Code
			{
				Code = item.Key,
				Name = item.Value
			}).ToList();
			_typeD.DataSource = dataSource;
			_typeD.DisplayMember = "Name";
			_typeD.ValueMember = "Code";
			_typeD.NullText = string.Empty;
			List<Common.Table_Code> dataSource2 = _lstValidS.Select((KeyValuePair<string, string> item) => new Common.Table_Code
			{
				Code = item.Key,
				Name = item.Value
			}).ToList();
			_typeS.DataSource = dataSource2;
			_typeS.DisplayMember = "Name";
			_typeS.ValueMember = "Code";
			_typeS.NullText = string.Empty;
			List<Common.Table_Code> dataSource3 = _lstValidC.Select((KeyValuePair<string, string> item) => new Common.Table_Code
			{
				Code = item.Key,
				Name = item.Value
			}).ToList();
			_typeC.DataSource = dataSource3;
			_typeC.DisplayMember = "Name";
			_typeC.ValueMember = "Code";
			_typeC.NullText = string.Empty;
			List<Common.Table_Code> dataSource4 = _lstValidL.Select((KeyValuePair<string, string> item) => new Common.Table_Code
			{
				Code = item.Key,
				Name = item.Value
			}).ToList();
			_typeL.DataSource = dataSource4;
			_typeL.DisplayMember = "Name";
			_typeL.ValueMember = "Code";
			_typeL.NullText = string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Ok_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			switch (base.FormMode)
			{
			case Enums.FormViewModeType.Add:
				await Save(1);
				break;
			case Enums.FormViewModeType.Update:
				await Save(2);
				break;
			case Enums.FormViewModeType.View:
				Close();
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private async void btn_Search_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			string bpCode = Function.ToString(btn_Bp.EditValue);
			Function.ToString(btn_OwnerSearch.EditValue);
			string code = Function.ToString(txt_Code.EditValue);
			string status = Function.ToString(luk_Sta.EditValue);
			string startFrom = (Function.ParseDateTime(date_StartFrom.EditValue).HasValue ? ((DateTime)date_StartFrom.EditValue).ToString("yyyyMMdd") : "");
			string startTo = (Function.ParseDateTime(date_StartTo.EditValue).HasValue ? ((DateTime)date_StartTo.EditValue).ToString("yyyyMMdd") : "");
			if (ProjectId > 0)
			{
				txt_No.EditValue = ProjectId;
			}
			DataTable dataSource = await LoadData(Function.ParseInt(txt_No.EditValue), code, status, bpCode, startFrom, startTo);
			ProjectId = 0;
			if (!_isLoading && ((base.FormMode != Enums.FormViewModeType.Add && base.FormMode != Enums.FormViewModeType.Update) || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes))
			{
				base.FormMode = Enums.FormViewModeType.View;
				tv_budget.DataSourceTable().Rows.Clear();
				tv_Wbs.DataSourceTable().Rows.Clear();
				grd_LineDocument.DataSourceTable.Rows.Clear();
				grd_LineTeam.DataSourceTable.Rows.Clear();
				grd_LineSub.DataSourceTable.Rows.Clear();
				grd_Doc.DataSource = dataSource;
				grd_Doc.DatasourceTable().AcceptChanges();
				int id = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("AbsEntry"));
				await LoadLineById(id);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Select_Click(object sender, EventArgs e)
	{
		try
		{
			string a = btn_Select.Text;
			List<TreeListNode> lst = new List<TreeListNode>();
			if (tv_budget.Nodes.Count > 0)
			{
				for (int i = 0; i < tv_budget.Nodes.Count; i++)
				{
					GetLevelNodesItem(tv_budget.Nodes[i], ref lst);
				}
			}
			foreach (TreeListNode item in lst)
			{
				if (!item.Visible)
				{
					continue;
				}
				if (string.Equals(a, "Select All"))
				{
					if (!string.IsNullOrEmpty(Function.ToString(item["AbsEntry"])))
					{
						item.CheckState = CheckState.Checked;
					}
				}
				else if (string.Equals(a, "UnSelect All") && !string.IsNullOrEmpty(Function.ToString(item["AbsEntry"])))
				{
					item.CheckState = CheckState.Unchecked;
				}
			}
			List<TreeListNode> list = lst.Where((TreeListNode n) => n.CheckState == CheckState.Checked).ToList();
			if (list != null && list.Count > 0)
			{
				btn_Select.Text = "UnSelect All";
			}
			else
			{
				btn_Select.Text = "Select All";
			}
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
			if (base.FormMode == Enums.FormViewModeType.Add || _addsub)
			{
				btn_Ok.Enabled = true;
				return;
			}
			int id = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("AbsEntry"));
			await LoadLineById(id);
			btn_Code.RefreshDataSource();
			string text = Function.ToString(gv_Doc.GetFocusedRowCellValue("PrgStatus"));
			switch (text)
			{
			case "D":
				addSubProjectToolStripMenuItem.Enabled = true;
				newTaskInDoc.Enabled = true;
				newEvenToolStripMenuItem.Enabled = true;
				relatedActivitiesToolStripMenuItem.Enabled = true;
				newTrackingIssueToolStripMenuItem.Enabled = true;
				drp_CrDocumnet.Enabled = false;
				break;
			case "S":
				addSubProjectToolStripMenuItem.Enabled = true;
				newTaskInDoc.Enabled = true;
				newEvenToolStripMenuItem.Enabled = true;
				relatedActivitiesToolStripMenuItem.Enabled = true;
				newTrackingIssueToolStripMenuItem.Enabled = true;
				drp_CrDocumnet.Enabled = true;
				break;
			case "P":
				addSubProjectToolStripMenuItem.Enabled = true;
				newTaskInDoc.Enabled = true;
				newEvenToolStripMenuItem.Enabled = true;
				relatedActivitiesToolStripMenuItem.Enabled = true;
				newTrackingIssueToolStripMenuItem.Enabled = true;
				drp_CrDocumnet.Enabled = true;
				break;
			case "F":
				addSubProjectToolStripMenuItem.Enabled = true;
				newTaskInDoc.Enabled = true;
				newEvenToolStripMenuItem.Enabled = true;
				relatedActivitiesToolStripMenuItem.Enabled = true;
				newTrackingIssueToolStripMenuItem.Enabled = true;
				drp_CrDocumnet.Enabled = true;
				break;
			case "C":
				addSubProjectToolStripMenuItem.Enabled = false;
				newTaskInDoc.Enabled = false;
				newEvenToolStripMenuItem.Enabled = false;
				relatedActivitiesToolStripMenuItem.Enabled = false;
				newTrackingIssueToolStripMenuItem.Enabled = false;
				drp_CrDocumnet.Enabled = false;
				break;
			case "L":
				addSubProjectToolStripMenuItem.Enabled = false;
				newTaskInDoc.Enabled = false;
				newEvenToolStripMenuItem.Enabled = false;
				relatedActivitiesToolStripMenuItem.Enabled = false;
				newTrackingIssueToolStripMenuItem.Enabled = false;
				drp_CrDocumnet.Enabled = false;
				break;
			}
			int num = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("BaseProject"));
			string text2 = Function.ToString(gv_Doc.GetFocusedRowCellValue("OwnerAuth"));
			if (num == 0 && text == "S" && text2 == "F")
			{
				duplicateToolStripMenuItem.Enabled = true;
			}
			else
			{
				duplicateToolStripMenuItem.Enabled = false;
			}
			string text3 = Function.ToString(gv_Doc.GetFocusedRowCellValue("WddStatus"));
			string text4 = Function.ToString(gv_Doc.GetFocusedRowCellValue("NextApprover"));
			string text5 = Function.ToString(gv_Doc.GetFocusedRowCellValue("Approver"));
			if (!string.IsNullOrEmpty(text5))
			{
				btn_UndoApproval.Visible = text5.Split(',').Contains(GlobalData.UserCodeLogin) && !text3.Equals("N");
			}
			else if (!string.IsNullOrEmpty(text4))
			{
				bool flag = text4.Split(',').Contains(GlobalData.UserCodeLogin);
				bool flag2 = Function.ToString(gv_Doc.GetFocusedRowCellValue("BaseLineActive")).Equals("Y");
				bool flag3 = text3.Equals("P");
				if (flag & flag2 & flag3)
				{
					btn_UndoApproval.Visible = true;
				}
				else
				{
					btn_UndoApproval.Visible = false;
				}
			}
			SimpleButtonEx simpleButtonEx = btn_Approval;
			bool visible = (btn_Reject.Visible = text4.Split(',').Contains(GlobalData.UserCodeLogin) && text3 == "W");
			simpleButtonEx.Visible = visible;
			string a = Function.ToString(gv_Doc.GetFocusedRowCellValue("BaseLine"));
			if (!text3.Equals("-") || string.Equals(a, "Y"))
			{
				btn_Ok.Enabled = false;
			}
			else
			{
				btn_Ok.Enabled = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (Function.ToString(gv_Doc.GetFocusedRowCellValue("OwnerAuth")) != "F")
			{
				if (gv_Doc.FocusedColumn.FieldName == "CommentsCount")
				{
					e.Cancel = false;
				}
				else
				{
					e.Cancel = true;
				}
				return;
			}
			if (gv_Doc.FocusedColumn.FieldName == "WddStatus")
			{
				e.Cancel = true;
				return;
			}
			string text = Function.ToString(gv_Doc.GetFocusedRowCellValue("ProjectType"));
			string text2 = Function.ToString(gv_Doc.GetFocusedRowCellValue("BaseLine"));
			if (gv_Doc.FocusedColumn.FieldName == "CardCode" && text.Equals("I"))
			{
				e.Cancel = true;
			}
			if (gv_Doc.FocusedColumn.FieldName == "CardName" && text.Equals("I"))
			{
				e.Cancel = true;
			}
			if (text2.Equals("Y"))
			{
				e.Cancel = true;
			}
			string a = Function.ToString(gv_Doc.GetFocusedRowCellValue("PrgStatus"));
			if (!(gv_Doc.FocusedColumn.FieldName == "Remark") && (string.Equals(a, "C") || string.Equals(a, "L")))
			{
				gv_Doc.FocusedColumn.OptionsColumn.ReadOnly = true;
				gv_Doc.Columns["Remark"].OptionsColumn.ReadOnly = false;
			}
			else
			{
				gv_Doc.Columns["AbsEntry"].OptionsColumn.ReadOnly = true;
				gv_Doc.Columns["UserCodeSign2"].OptionsColumn.ReadOnly = true;
				gv_Doc.Columns["UserCodeSign"].OptionsColumn.ReadOnly = true;
				gv_Doc.Columns["CreateDate"].OptionsColumn.ReadOnly = true;
				gv_Doc.Columns["CreateTime"].OptionsColumn.ReadOnly = true;
				gv_Doc.Columns["UpdateDate"].OptionsColumn.ReadOnly = true;
				gv_Doc.Columns["UpdateTime"].OptionsColumn.ReadOnly = true;
			}
			if (!Function.ToString(gv_Doc.GetFocusedRowCellValue("WddStatus")).Equals("-") || string.Equals(text2, "Y"))
			{
				e.Cancel = true;
			}
			if (gv_Doc.FocusedColumn.FieldName == "CommentsCount")
			{
				e.Cancel = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void gv_Doc_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
	{
		_ = 4;
		try
		{
			_dtItem = tv_budget.DataSourceTable();
			List<string> list = new List<string> { "OcrCode", "OcrCode2", "OcrCode3", "OcrCode4", "OcrCode5" };
			if (e.Column.FieldName == "BudgetLevel")
			{
				_isChangeItem = true;
				_dtPayment = tv_Payments.DataSourceTable();
				int num = ((gv_Doc.ActiveEditor != null) ? Function.ParseInt(gv_Doc.ActiveEditor.OldEditValue) : 0);
				int num2 = Function.ParseInt(e.Value);
				if (num2 < num)
				{
					for (int i = 0; i < _dtItem.Rows.Count; i++)
					{
						int id = Function.ParseInt(_dtItem.Rows[i]["AbsEntry"]);
						int lineNum = Function.ParseInt(_dtItem.Rows[i]["LineNum"]);
						string text = Function.ToString(_dtItem.Rows[i]["ItemCode"]);
						if (!string.IsNullOrEmpty(_httpClient.ProjectManager.CheckItemInDocument(id, lineNum)))
						{
							WinCommons.Warning(Enums.MessageCode.ItemCodeIsBeingLinked2Document, "", text);
							return;
						}
					}
					if (_dtItem != null && _dtItem.Rows.Count > 0)
					{
						List<DataRow> list2 = (from t in _dtItem.AsEnumerable()
							where Function.ParseInt(t["IsNew"]) == 0 && Function.ParseInt(t["FatherNum"]) > 0
							select t).ToList();
						if (list2.Count > 0)
						{
							for (int num3 = 0; num3 < list2.Count; num3++)
							{
								list2[num3].Delete();
							}
							_dtItem.AcceptChanges();
						}
					}
				}
				if (num2 > 0)
				{
					SetDataByLevelItem();
					SetDataByLevelPayments();
				}
				else
				{
					if (_dtItem != null && _dtItem.Rows.Count > 0)
					{
						List<DataRow> list3 = (from t in _dtItem.AsEnumerable()
							where Function.ParseInt(t["BaseLine"]) > 0
							select t).ToList();
						if (list3.Count > 0)
						{
							for (int num4 = 0; num4 < list3.Count; num4++)
							{
								list3[num4].Delete();
							}
							_dtItem.AcceptChanges();
						}
					}
					if (_dtPayment != null && _dtPayment.Rows.Count > 0)
					{
						List<DataRow> list4 = (from t in _dtPayment.AsEnumerable()
							where Function.ParseInt(t["BaseLine"]) > 0
							select t).ToList();
						if (list4.Count > 0)
						{
							for (int num5 = 0; num5 < list4.Count; num5++)
							{
								list4[num5].Delete();
							}
							_dtPayment.AcceptChanges();
						}
					}
				}
			}
			else if (e.Column.FieldName == "DefaultWhs")
			{
				string value = Function.ToString(e.Value);
				if (!string.IsNullOrEmpty(value) && _dtItem != null && _dtItem.Rows.Count > 0)
				{
					for (int num6 = 0; num6 < _dtItem.Rows.Count; num6++)
					{
						Function.ParseInt(_dtItem.Rows[num6]["FatherNum"]);
						if (string.IsNullOrEmpty(Function.ToString(_dtItem.Rows[num6]["TaskCode"])))
						{
							_dtItem.Rows[num6]["DefaultWhs"] = value;
						}
					}
				}
			}
			else if (e.Column.FieldName == "CardCode")
			{
				gv_Doc.SetFocusedRowCellValue("Order", null);
				if (_dtItem != null && _dtItem.Rows.Count > 0)
				{
					for (int num7 = 0; num7 < _dtItem.Rows.Count; num7++)
					{
						_dtItem.Rows[num7]["OrderItem"] = -1;
					}
				}
			}
			else if (e.Column.FieldName == "Order")
			{
				if (_dtItem != null && _dtItem.Rows.Count > 0)
				{
					for (int num8 = 0; num8 < _dtItem.Rows.Count; num8++)
					{
						_dtItem.Rows[num8]["OrderItem"] = -1;
					}
				}
			}
			else if (e.Column.FieldName == "DocCur")
			{
				string value2 = Function.ToString(e.Value);
				_dtItem = tv_budget.DataSourceTable();
				if (!string.IsNullOrEmpty(value2) && _dtItem != null && _dtItem.Rows.Count > 0)
				{
					for (int num9 = 0; num9 < _dtItem.Rows.Count; num9++)
					{
						_dtItem.Rows[num9]["Currency"] = value2;
					}
				}
				_dtDocument = grd_LineDocument.DataSourceTable();
				if (!string.IsNullOrEmpty(value2) && _dtDocument != null && _dtDocument.Rows.Count > 0)
				{
					for (int num10 = 0; num10 < _dtDocument.Rows.Count; num10++)
					{
						_dtDocument.Rows[num10]["Currency"] = value2;
					}
				}
			}
			else if (e.Column.FieldName == "StartDate")
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmChangeFromDate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
				{
					_dtItem = tv_Wbs.DataSourceTable();
					if (_dtItem != null && _dtItem.Rows.Count > 0)
					{
						for (int num11 = 0; num11 < _dtItem.Rows.Count; num11++)
						{
							_dtItem.Rows[num11]["FromDate"] = e.Value;
						}
					}
				}
			}
			else if (e.Column.FieldName == "DueDate")
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmChangeToDate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
				{
					_dtItem = tv_Wbs.DataSourceTable();
					if (_dtItem != null && _dtItem.Rows.Count > 0)
					{
						for (int num12 = 0; num12 < _dtItem.Rows.Count; num12++)
						{
							_dtItem.Rows[num12]["ToDate"] = e.Value;
						}
					}
				}
			}
			else if (e.Column.FieldName == "PM")
			{
				string value3 = Function.ToString(e.Value);
				_dtItem = tv_Wbs.DataSourceTable();
				if (!string.IsNullOrEmpty(value3) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmApplyToAllLine, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes && _dtItem != null && _dtItem.Rows.Count > 0)
				{
					for (int num13 = 0; num13 < _dtItem.Rows.Count; num13++)
					{
						_dtItem.Rows[num13]["Resource"] = value3;
					}
				}
			}
			else if (list.Contains(e.Column.FieldName))
			{
				string ocrCode = Function.ToString(e.Value);
				DataTable dtTask = tv_Wbs.DataSourceTable();
				if (e.Column.FieldName == "OcrCode")
				{
					await SetOrcCodeGrid(ocrCode, e, "OcrCode", 1);
				}
				else if (e.Column.FieldName == "OcrCode2")
				{
					await SetOrcCodeGrid(ocrCode, e, "OcrCode2", 2);
				}
				else if (e.Column.FieldName == "OcrCode3")
				{
					await SetOrcCodeGrid(ocrCode, e, "OcrCode3", 3);
				}
				else if (e.Column.FieldName == "OcrCode4")
				{
					await SetOrcCodeGrid(ocrCode, e, "OcrCode4", 4);
				}
				else if (e.Column.FieldName == "OcrCode5")
				{
					await SetOrcCodeGrid(ocrCode, e, "OcrCode5", 5);
				}
				if (!string.IsNullOrEmpty(ocrCode) && dtTask != null && dtTask.Rows.Count > 0)
				{
					for (int num14 = 0; num14 < dtTask.Rows.Count; num14++)
					{
						dtTask.Rows[num14][e.Column.FieldName] = ocrCode;
					}
				}
			}
			tv_Wbs.DataSourceTable().AcceptChanges();
			tv_budget.DataSourceTable().AcceptChanges();
			tv_Payments.DataSourceTable().AcceptChanges();
			if (!_isLoading)
			{
				DataChanged();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
	{
		if (e.Column.FieldName == "CreateTime")
		{
			e.DisplayText = Function.ParseIntToTimeSpanString(Function.ParseInt(gv_Doc.GetRowCellValue(e.ListSourceRowIndex, "CreateTime")));
		}
		if (e.Column.FieldName == "UpdateTime")
		{
			e.DisplayText = Function.ParseIntToTimeSpanString(Function.ParseInt(gv_Doc.GetRowCellValue(e.ListSourceRowIndex, "UpdateTime")));
		}
	}

	private void luk_Industry_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 1)
			{
				frm_IndustrySetup frm_IndustrySetup2 = new frm_IndustrySetup();
				frm_IndustrySetup2.IsOpenFromBP = true;
				frm_IndustrySetup2.FormClosing += (object s, FormClosingEventArgs e2) =>
				{
					LoadComboIndustry();
				};
				frm_IndustrySetup2.ShowDialog(this);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_BeforeLeaveRow(object sender, RowAllowEventArgs e)
	{
		try
		{
			if (!_isLoading && base.FormMode == Enums.FormViewModeType.Update)
			{
				DialogResult dialogResult = XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
				if (dialogResult == DialogResult.Yes && dialogResult == DialogResult.Yes)
				{
					grd_Doc.DataSourceTable.RejectChanges();
					base.FormMode = Enums.FormViewModeType.View;
				}
				if (dialogResult == DialogResult.No)
				{
					e.Allow = false;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			string text = Function.ToString(gv_Doc.GetRowCellValue(e.RowHandle, "ProjectType"));
			if ((e.Column.FieldName == "CardCode" || e.Column.FieldName == "CardName") && text.Equals("I"))
			{
				e.Appearance.BackColor = GridColor.GridReadOnly;
			}
			if (base.FormMode != Enums.FormViewModeType.Add && e.Column.FieldName == "Code")
			{
				e.Appearance.BackColor = GridColor.GridReadOnly;
			}
			if (e.Column.FieldName == "BudgetAmount" || e.Column.FieldName == "ActualAmount" || e.Column.FieldName == "AmountDiff" || e.Column.FieldName == "UserCodeSign2" || e.Column.FieldName == "UserCodeSign")
			{
				e.Appearance.BackColor = GridColor.GridReadOnly;
			}
			string a = Function.ToString(gv_Doc.GetRowCellValue(e.RowHandle, "PrgStatus"));
			if (!(e.Column.FieldName == "Remark") && (string.Equals(a, "C") || string.Equals(a, "L")))
			{
				e.Appearance.BackColor = GridColor.GridReadOnly;
			}
			string a2 = Function.ToString(gv_Doc.GetFocusedRowCellValue("BaseLine"));
			if (!string.Equals(Function.ToString(gv_Doc.GetFocusedRowCellValue("WddStatus")), "-") || string.Equals(a2, "Y"))
			{
				e.Appearance.BackColor = GridColor.GridReadOnly;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
	{
		try
		{
			if (base.FormMode != Enums.FormViewModeType.Add && gv_Doc.FocusedColumn.FieldName.Equals("MasterEntry"))
			{
				int num = Function.ParseInt(e.Value);
				if (num != 0 && num == Function.ParseInt(gv_Doc.GetFocusedRowCellValue("AbsEntry")))
				{
					e.Valid = false;
					WinCommons.Error(Enums.MessageCode.YouCannotMakeSubProjectOfItself, "");
					gv_Doc.DisplayException = true;
					return;
				}
			}
			gv_Doc.DisplayException = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_CustomRowCellEditForEditing(object sender, CustomRowCellEditEventArgs e)
	{
		try
		{
			if (e.Column.FieldName != "PrgStatus")
			{
				return;
			}
			string text = Function.ToString(gv_Doc.GetFocusedRowCellValue("PrgStatus"));
			if (!string.IsNullOrWhiteSpace(text))
			{
				switch (text)
				{
				case "D":
					e.RepositoryItem = _typeD;
					break;
				case "S":
					e.RepositoryItem = _typeS;
					break;
				case "P":
					e.RepositoryItem = _typeS;
					break;
				case "F":
					e.RepositoryItem = _typeS;
					break;
				case "C":
					e.RepositoryItem = _typeC;
					break;
				case "L":
					e.RepositoryItem = _typeL;
					break;
				}
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void addNewToolStripMenuItem_Click(object sender, EventArgs e)
	{
		await REFRESH();
	}

	private async void deleteToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (gv_Doc.GetSelectedRows() == null && gv_Doc.GetSelectedRows().Count() == 0)
			{
				WinCommons.Error(Enums.MessageCode.InfoNoRowSelected, "");
				return;
			}
			Function.ParseInt(gv_Doc.GetFocusedRowCellValue("AbsEntry"));
			string text = Function.ToString(gv_Doc.GetFocusedRowCellValue("Code"));
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmRemoveByCode, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, text) == DialogResult.Yes)
			{
				string id = Function.ToString(gv_Doc.GetFocusedRowCellValue("AbsEntry"));
				string text2 = await _httpClient.ProjectManager.DeleteDocs(id);
				if (string.IsNullOrEmpty(text2))
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
					btn_Search_Click(null, null);
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, text2);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void addRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (xtra_Line.SelectedTabPage == tab_Task)
		{
			NewTask();
		}
	}

	private void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (xtra_Line.SelectedTabPage != tab_Task)
			{
				return;
			}
			DeleteTeam();
			DataTable dataTable = tv_Wbs.DataSourceTable();
			_dtItem = tv_budget.DataSourceTable();
			_dtDoc = grd_Doc.DataSourceTable();
			if (dataTable == null || dataTable.Columns.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.InfoCannotLoadData, "");
			}
			TreeListNode focusedNode = tv_Wbs.FocusedNode;
			string text = Function.ToString(focusedNode.GetValue(tl_Wbs));
			int level = focusedNode.Level;
			int[] array = (from t in text.Split('.')
				select Function.ParseInt(t)).ToArray();
			string nextwbs = "";
			string text2 = Function.ToString(focusedNode.GetValue(tl_Code));
			if (string.IsNullOrEmpty(text2))
			{
				tv_Wbs.Nodes.Remove(focusedNode);
			}
			else
			{
				DataRow[] array2 = _dtItem.Select($"TaskCode = '{text2}'");
				if (array2.Count() > 0)
				{
					if (XtraMessageBoxEx.Show(Enums.MessageCode.ProjectManagerConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, text2) != DialogResult.Yes)
					{
						return;
					}
					DataRow[] array3 = array2;
					foreach (DataRow dataRow in array3)
					{
						TreeListNode focusedNode2 = tv_budget.FindNodeByKeyID(Function.ParseInt(dataRow["LineNum"]));
						tv_budget.FocusedNode = focusedNode2;
						tv_budget.DeleteSelectedNodes();
						tv_budget.DataSourceTable().AcceptChanges();
					}
					for (int num2 = 0; num2 < _dtItem.Rows.Count; num2++)
					{
						int fatherId = Function.ParseInt(_dtItem.Rows[num2]["LineNum"]);
						if (!string.IsNullOrEmpty(Function.ToString(_dtItem.Rows[num2]["TaskCode"])))
						{
							AmountFather(_dtItem, fatherId);
							ActualAmountFather(_dtItem, fatherId);
							SumAmountHeader(_dtItem, _dtDoc);
						}
					}
				}
				if (array.Length > level)
				{
					array[level]++;
					nextwbs = string.Join(".", array);
				}
				if (focusedNode.ParentNode != null)
				{
					focusedNode.ParentNode.Nodes.Remove(focusedNode);
				}
				else
				{
					tv_Wbs.Nodes.Remove(focusedNode);
				}
			}
			dataTable.AcceptChanges();
			DataRow dataRow2 = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["Wbs"]) == nextwbs);
			if (dataRow2 != null)
			{
				int num3 = Function.ParseInt(dataRow2["LineNum"]);
				TreeListNode treeListNode = tv_Wbs.FindNodeByKeyID(num3);
				if (treeListNode != null)
				{
					treeListNode.SetValue("Wbs", text);
					RefWbs(treeListNode, dataTable, Function.ParseInt(focusedNode.GetValue("FatherNum")), text, "Y");
				}
			}
			RefreshVisOrder();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_LineTeam_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
	{
		if (!_isLoading)
		{
			DataChanged();
		}
	}

	private void addSubProjectToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			int focusedRowHandle = gv_Doc.FocusedRowHandle;
			int id = Function.ParseInt(gv_Doc.GetRowCellValue(focusedRowHandle, "AbsEntry"));
			AddSub(id);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_LineTeam_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == "Resource" || e.Column.FieldName == "EmployeeName" || e.Column.FieldName == "Branch" || e.Column.FieldName == "Department")
			{
				e.Appearance.BackColor = GridColor.GridReadOnly;
			}
			string a = Function.ToString(gv_Doc.GetFocusedRowCellValue("PrgStatus"));
			if (string.Equals(a, "C") || string.Equals(a, "L"))
			{
				e.Appearance.BackColor = GridColor.GridReadOnly;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void addSubProjectToolStripMenuItem1_Click(object sender, EventArgs e)
	{
		try
		{
			int focusedRowHandle = gv_Doc.FocusedRowHandle;
			int id = Function.ParseInt(gv_Doc.GetRowCellValue(focusedRowHandle, "AbsEntry"));
			AddSub(id);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void insertRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (xtra_Line.SelectedTabPage == tab_Task)
			{
				DataTable dataSourceTable = tv_Wbs.DataSourceTable;
				_dtItem = tv_budget.DataSourceTable();
				if (dataSourceTable == null || dataSourceTable.Columns.Count == 0)
				{
					WinCommons.Error(Enums.MessageCode.CannotGetData, "");
					return;
				}
				int lineNum = Function.ParseInt(tv_Wbs.FocusedNode.GetValue(tl_LineNum));
				NewTask(lineNum, Function.ToString(lineNum));
				tv_Wbs.FocusedNode.Expand();
				DataRow dataRow = (from t in dataSourceTable.AsEnumerable()
					where Function.ParseInt(t["FatherNum"]) == lineNum
					select t).ToArray().ToList().FirstOrDefault((DataRow t) => Function.ToString(t["TaskCode"]) == "");
				if (dataRow != null)
				{
					string cellValue = Function.ToString(dataRow["Wbs"]);
					tv_Wbs.FocusedNode = tv_Wbs.FindNodeByFieldValue("Wbs", cellValue);
				}
			}
			else if (xtra_Line.SelectedTabPage == xtra_Document)
			{
				InsertRow(_dtDocument, gv_LineDocument, grd_LineDocument);
			}
			else if (xtra_Line.SelectedTabPage == xtra_Sub)
			{
				InsertRow(_dtSub, gv_LineSub, grd_LineSub);
			}
			DataChanged();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void duplicateRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (xtra_Line.SelectedTabPage != tab_Task)
			{
				return;
			}
			DataTable dataSourceTable = tv_Wbs.DataSourceTable;
			_dtItem = tv_budget.DataSourceTable();
			if (dataSourceTable == null || dataSourceTable.Columns.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.CannotGetData, "");
				return;
			}
			DataRow focusedDataRow = tv_Wbs.GetFocusedDataRow();
			if (focusedDataRow != null)
			{
				DataRow dataRow = dataSourceTable.NewRow();
				int num = Function.ParseInt(dataRow["LineNum"]);
				dataRow.ItemArray = focusedDataRow.ItemArray;
				if (_dtItem != null && _dtItem.Rows.Count > 0)
				{
					for (int i = 0; i < dataSourceTable.Rows.Count; i++)
					{
						if (Function.ToString(dataSourceTable.Rows[i]["TaskCode"]) == "")
						{
							dataSourceTable.Rows.RemoveAt(i--);
						}
					}
					int num2 = Function.ParseInt(dataSourceTable.Compute("MAX(LineNum)", ""));
					dataRow["LineNum"] = num2 + 1;
				}
				dataRow["BaseLine"] = Function.ParseInt(dataRow["LineNum"]);
				if (tv_Wbs.FocusedNode.Id + 1 >= dataSourceTable.Rows.Count)
				{
					dataSourceTable.Rows.Add(dataRow);
				}
				else
				{
					dataSourceTable.Rows.InsertAt(dataRow, tv_Wbs.FocusedNode.Id + 1);
				}
				dataSourceTable.AcceptChanges();
				RefreshWbs(tv_Wbs.FocusedNode, dataSourceTable, "Y");
				RefreshVisOrder();
				TreeListNode focusedNode = tv_Wbs.FindNodeByKeyID(num);
				tv_Wbs.FocusedNode = focusedNode;
			}
			DataChanged();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void frm_ProjectManager_Shown(object sender, EventArgs e)
	{
		if (ContextMenuStrip != null)
		{
			ContextMenuStrip.Opening += ContextMenuStripOnOpening;
		}
	}

	private void ContextMenuStripOnOpening(object sender, CancelEventArgs e)
	{
		try
		{
			e.Cancel = schedulerControl1.Bounds.Contains(schedulerControl1.PointToClient(Control.MousePosition));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_ShowGantt_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode_Type == FormModeType.Hide)
			{
				splitContainerControl4.PanelVisibility = SplitPanelVisibility.Panel1;
				FormMode_Type = FormModeType.Show;
			}
			else if (FormMode_Type == FormModeType.Show)
			{
				splitContainerControl4.PanelVisibility = SplitPanelVisibility.Both;
				FormMode_Type = FormModeType.Hide;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void newTaskToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			int num = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("AbsEntry"));
			Function.ToString(tv_Wbs.GetFocusedRowCellValue(tl_Code));
			string value = Function.ToString(gv_Doc.GetFocusedRowCellValue("CardCode"));
			frm_TaskManagement frm_TaskManagement2 = new frm_TaskManagement
			{
				ClgCode = 0,
				StartPosition = FormStartPosition.CenterScreen,
				MdiParent = MdiParent,
				CallFromCal = true
			};
			string value2 = "";
			string text = Function.ToString(tv_Wbs.GetFocusedRowCellValue(tl_Resource));
			if (text != null)
			{
				value2 = text;
			}
			DataTable tableStructure = WinFuntions.GetTableStructure("APZ_OCLG");
			DataRow dataRow = tableStructure.NewRow();
			if (!string.IsNullOrEmpty(value2))
			{
				dataRow["GroupEmployee"] = value2;
			}
			else
			{
				dataRow["GroupUser"] = GlobalData.UserID;
			}
			if (Function.ToString(dataRow["Recontact"]) == "")
			{
				dataRow["Recontact"] = DateTime.Today;
				string text2 = DateTime.Now.Minute.ToString();
				dataRow["BeginTime"] = DateTime.Now.Hour + ((text2.Length == 2) ? text2 : ("0" + text2));
			}
			dataRow["Closed"] = "N";
			dataRow["ProjectEntry"] = num;
			dataRow["CardCode"] = value;
			tableStructure.Rows.Add(dataRow);
			tableStructure.AcceptChanges();
			frm_TaskManagement2.DataRowFromCal = dataRow;
			frm_TaskManagement2.ReloadCalendar = (Func<Task>)Delegate.Combine(frm_TaskManagement2.ReloadCalendar, new Func<Task>(ReloadTaskList));
			frm_TaskManagement2.ReloadAfterRemove = (Func<int, Task>)Delegate.Combine(frm_TaskManagement2.ReloadAfterRemove, new Func<int, Task>(ReloadTaskList));
			frm_TaskManagement2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void newEventToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			SetDataEvent();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void newEvenToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			SetDataEvent();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void SetDataEvent()
	{
		try
		{
			int num = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("AbsEntry"));
			string value = Function.ToString(tv_Wbs.GetFocusedRowCellValue(tl_Code));
			string value2 = Function.ToString(gv_Doc.GetFocusedRowCellValue("CardCode"));
			int userID = GlobalData.UserID;
			DataTable tableStructure = WinFuntions.GetTableStructure("APZ_OCLG");
			DataRow dataRow = tableStructure.NewRow();
			dataRow["ClgCode"] = 0;
			dataRow["Recontact"] = DateTime.Today;
			string text = DateTime.Now.ToString("HH:mm");
			dataRow["BeginTime"] = Function.ParseInt(text.Replace(":", ""));
			dataRow["EndDate"] = DateTime.Today;
			string text2 = DateTime.Now.AddMinutes(30.0).ToString("HH:mm");
			dataRow["ENDTime"] = Function.ParseInt(text2.Replace(":", ""));
			if (Function.ParseInt(dataRow["BeginTime"]) == 0 && Function.ParseInt(dataRow["ENDTime"]) == 0)
			{
				dataRow["AllDay"] = "Y";
			}
			else
			{
				dataRow["AllDay"] = "N";
			}
			dataRow["GroupUser"] = Function.ToString(userID);
			dataRow["Closed"] = "N";
			dataRow["ProjectEntry"] = num;
			dataRow["ProjectWBS"] = value;
			dataRow["CardCode"] = value2;
			tableStructure.Rows.Add(dataRow);
			tableStructure.AcceptChanges();
			frm_EventManagement frm_EventManagement2 = new frm_EventManagement();
			frm_EventManagement2.MdiParent = MdiParent;
			frm_EventManagement2.CallFromRelated = true;
			frm_EventManagement2.DataRowFromCal = dataRow;
			frm_EventManagement2.StartPosition = FormStartPosition.CenterScreen;
			frm_EventManagement2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void schedulerControl1_CustomDrawDependency(object sender, DevExpress.XtraScheduler.CustomDrawObjectEventArgs e)
	{
		try
		{
			DependencyViewInfo dependencyViewInfo = (DependencyViewInfo)e.ObjectInfo;
			bool flag = dependencyViewInfo.Items.OfType<DependencyViewInfoArrowItem>().Any();
			if (dependencyViewInfo.Dependencies[0].Type == AppointmentDependencyType.FinishToStart)
			{
				Pen pen = new Pen(Color.DimGray, 2f);
				if (flag)
				{
					AdjustableArrowCap customEndCap = new AdjustableArrowCap(5f, 5f);
					pen.EndCap = LineCap.ArrowAnchor;
					pen.CustomEndCap = customEndCap;
				}
				else
				{
					pen.DashStyle = DashStyle.Solid;
				}
				dependencyViewInfo.Graphics.DrawLine(pen, dependencyViewInfo.Start, dependencyViewInfo.End);
			}
			e.Handled = true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void treeList1_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (Function.ToString(gv_Doc.GetFocusedRowCellValue("OwnerAuth")) != "F")
			{
				if (tv_Wbs.FocusedColumn.FieldName == "CommentsCount")
				{
					e.Cancel = false;
				}
				else
				{
					e.Cancel = true;
				}
				return;
			}
			if (tv_Wbs.FocusedColumn.FieldName == "CommentsCount")
			{
				e.Cancel = false;
				return;
			}
			string text = Function.ToString(tv_Wbs.FocusedNode.GetValue(tl_IsExist));
			if (base.FormMode != Enums.FormViewModeType.Add && text == "Y" && tv_Wbs.FocusedColumn.FieldName == "TaskCode")
			{
				tv_Wbs.FocusedColumn.OptionsColumn.ReadOnly = true;
			}
			else
			{
				tv_Wbs.FocusedColumn.OptionsColumn.ReadOnly = false;
				tv_Wbs.Columns["Wbs"].OptionsColumn.ReadOnly = true;
				tv_Wbs.Columns["LineNum"].OptionsColumn.ReadOnly = true;
				tv_Wbs.Columns["AbsEntry"].OptionsColumn.ReadOnly = true;
			}
			string a = Function.ToString(gv_Doc.GetFocusedRowCellValue("PrgStatus"));
			if ((tv_Wbs.FocusedColumn.FieldName == "TaskCode" || tv_Wbs.FocusedColumn.FieldName == "TaskName" || tv_Wbs.FocusedColumn.FieldName == "FromDate" || tv_Wbs.FocusedColumn.FieldName == "ToDate" || tv_Wbs.FocusedColumn.FieldName == "OcrCode" || tv_Wbs.FocusedColumn.FieldName == "OcrCode2" || tv_Wbs.FocusedColumn.FieldName == "OcrCode3" || tv_Wbs.FocusedColumn.FieldName == "OcrCode4" || tv_Wbs.FocusedColumn.FieldName == "OcrCode5" || tv_Wbs.FocusedColumn.FieldName == "Priority" || tv_Wbs.FocusedColumn.FieldName == "Milestone" || tv_Wbs.FocusedColumn.FieldName == "Notes" || tv_Wbs.FocusedColumn.FieldName == "Status") && (string.Equals(a, "C") || string.Equals(a, "L")))
			{
				e.Cancel = true;
			}
			string a2 = Function.ToString(gv_Doc.GetFocusedRowCellValue("BaseLine"));
			if (!string.Equals(Function.ToString(gv_Doc.GetFocusedRowCellValue("WddStatus")), "-") || string.Equals(a2, "Y"))
			{
				tv_Wbs.FocusedColumn.OptionsColumn.ReadOnly = true;
			}
			if (tv_Wbs.FocusedColumn.FieldName == "CommentsCount")
			{
				e.Cancel = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void treeList1_NodeCellStyle(object sender, GetCustomNodeCellStyleEventArgs e)
	{
		try
		{
			string text = Function.ToString(tv_Wbs.GetRowCellValue(e.Node, tl_Milestone));
			string text2 = Function.ToString(tv_Wbs.GetRowCellValue(e.Node, tl_IsExist));
			if (text.Equals("Y"))
			{
				e.Appearance.Font = new Font(e.Appearance.Font.FontFamily, e.Appearance.Font.Size, FontStyle.Bold);
			}
			if (base.FormMode != Enums.FormViewModeType.Add && text2 == "Y" && e.Column.FieldName == "TaskCode")
			{
				e.Appearance.BackColor = GridColor.GridReadOnly;
			}
			if (new string[2] { "LineNum", "Wbs" }.Contains(e.Column.FieldName))
			{
				e.Appearance.BackColor = GridColor.GridReadOnly;
			}
			string a = Function.ToString(gv_Doc.GetFocusedRowCellValue("PrgStatus"));
			if (string.Equals(a, "C") || string.Equals(a, "L"))
			{
				e.Appearance.BackColor = GridColor.GridReadOnly;
			}
			string a2 = Function.ToString(gv_Doc.GetFocusedRowCellValue("BaseLine"));
			if (!string.Equals(Function.ToString(gv_Doc.GetFocusedRowCellValue("WddStatus")), "-") || string.Equals(a2, "Y"))
			{
				e.Appearance.BackColor = GridColor.GridReadOnly;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void treeList1_CellValueChanged(object sender, DevExpress.XtraTreeList.CellValueChangedEventArgs e)
	{
		try
		{
			DataTable dtTask = tv_Wbs.DataSourceTable();
			int linenum = Function.ParseInt(e.Node.GetValue("LineNum"));
			int fatherNum = Function.ParseInt(e.Node.GetValue("FatherNum"));
			TreeListNode node = e.Node;
			if (e.Column.FieldName == "Resource")
			{
				await SetDataTeamByResource(Function.ToString(e.Value));
			}
			else if (e.Column.FieldName == "OcrCode")
			{
				string ocrCode = Function.ToString(e.Value);
				await SetOrcCode(ocrCode, e, "OcrCode", 1, tv_Wbs);
			}
			else if (e.Column.FieldName == "OcrCode2")
			{
				string ocrCode2 = Function.ToString(e.Value);
				await SetOrcCode(ocrCode2, e, "OcrCode2", 2, tv_Wbs);
			}
			else if (e.Column.FieldName == "OcrCode3")
			{
				string ocrCode3 = Function.ToString(e.Value);
				await SetOrcCode(ocrCode3, e, "OcrCode3", 3, tv_Wbs);
			}
			else if (e.Column.FieldName == "OcrCode4")
			{
				string ocrCode4 = Function.ToString(e.Value);
				await SetOrcCode(ocrCode4, e, "OcrCode4", 4, tv_Wbs);
			}
			else if (e.Column.FieldName == "OcrCode5")
			{
				string ocrCode5 = Function.ToString(e.Value);
				await SetOrcCode(ocrCode5, e, "OcrCode5", 5, tv_Wbs);
			}
			else if (e.Column.FieldName == "Dependent")
			{
				if (_dtDepen == null)
				{
					WinCommons.Error(Enums.MessageCode.CannotGetDataPredecessor, "");
					return;
				}
				int num = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("AbsEntry"));
				if (_dtDepen.IsNotNull())
				{
					for (int i = 0; i < _dtDepen.Rows.Count; i++)
					{
						if (Function.ParseInt(_dtDepen.Rows[i]["LineNum"]) == linenum)
						{
							_dtDepen.Rows.Remove(_dtDepen.Rows[i]);
							i--;
						}
					}
				}
				_dtDepen.AcceptChanges();
				int[] array = (from t in Function.ToString(e.Value).Split(',')
					select Function.ParseInt(t)).ToArray();
				if (array.Length >= 1)
				{
					for (int num2 = 0; num2 < array.Length; num2++)
					{
						DataRow dataRow = _dtDepen.NewRow();
						dataRow["AbsEntry"] = num;
						dataRow["LineNum"] = Function.ParseInt(e.Node.GetValue("LineNum"));
						dataRow["VisOrder"] = num2;
						dataRow["Dependent"] = array[num2];
						_dtDepen.Rows.Add(dataRow);
					}
					_dtDepen.AcceptChanges();
					int lineId = array.Max();
					DataRow dataRow2 = (from t in dtTask.AsEnumerable()
						where Function.ParseInt(t["LineNum"]) == Function.ParseInt(lineId)
						select t).FirstOrDefault();
					if (dataRow2 != null)
					{
						DateTime dateTime = Function.ParseDateTimes(dataRow2["ToDate"]).AddDays(1.0);
						tv_Wbs.DataSourceTable().Rows[e.Node.Id]["FromDate"] = dateTime;
						DateTime dateTime2 = Function.ParseDateTimes(tv_Wbs.GetRowCellValue(e.Node, tl_ToDate));
						int num3 = Function.ParseInt(tv_Wbs.GetRowCellValue(e.Node, tl_Duration));
						if (!string.IsNullOrEmpty(Function.ToString(dateTime2)))
						{
							tv_Wbs.DataSourceTable().Rows[e.Node.Id]["ToDate"] = dateTime.AddDays(num3 - 1);
							dateTime2 = dateTime;
							SetToDate(fatherNum, dtTask, linenum, dateTime2, Function.ToString(tv_Wbs.GetRowCellValue(e.Node, tl_WBSMode)));
						}
					}
				}
			}
			else if (e.Column.FieldName == "TaskCode")
			{
				_isChangeItem = true;
				if (string.IsNullOrEmpty(Function.ToString(e.Node.GetValue("Wbs"))))
				{
					List<DataRow> list = (from t in dtTask.AsEnumerable()
						where Function.ParseInt(t["FatherNum"]) == 0 || Function.ParseInt(t["FatherNum"]) == -1
						select t).ToList();
					int num4 = 0;
					if (list.Count > 0)
					{
						num4 = Function.ParseInt(list.CopyToDataTable().AsEnumerable().Max((DataRow t) => Function.ParseInt(t["Wbs"])));
					}
					if (num4 > 0)
					{
						node.SetValue("Wbs", num4 + 1);
					}
					else
					{
						node.SetValue("Wbs", 1);
					}
					if (dtTask.Rows.Count > 0)
					{
						node.SetValue("VisOrder", Function.ParseInt(dtTask.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["VisOrder"]))) + 1);
					}
					else
					{
						node.SetValue("VisOrder", 1);
					}
					if (dtTask.Rows.Count > 0)
					{
						node.SetValue("LineNum", Function.ParseInt(dtTask.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["LineNum"]))) + 1);
					}
					else
					{
						node.SetValue("LineNum", 1);
					}
				}
			}
			else if (e.Column.FieldName == "WbsTemplate")
			{
				if (base.FormMode != Enums.FormViewModeType.Add && tv_Wbs.ActiveEditor != null)
				{
					string wbsCode = Function.ToString(tv_Wbs.ActiveEditor.OldEditValue);
					DataTable dataTable = await _httpClient.ProjectManager.GetTaskByWbsCode("1470000132", Function.ParseInt(gv_Doc.GetFocusedRowCellValue("AbsEntry")), linenum, wbsCode);
					if (dataTable != null && dataTable.Rows.Count > 0 && XtraMessageBoxEx.Show(Enums.MessageCode.ProjectManagerConfirmChange, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
					{
						return;
					}
				}
				DataRow dataRow3 = (from t in btn_WBSTemplate.DataSource.AsEnumerable()
					where Function.ToString(t["Code"]) == Function.ToString(e.Value)
					select t).FirstOrDefault();
				if (dtTask != null && dtTask.Rows.Count > 0 && dataRow3 != null)
				{
					string text = Function.ToString(dataRow3["Code"]);
					if (!string.IsNullOrEmpty(text))
					{
						tv_Wbs.SetRowCellValue(e.Node, tl_Name, Function.ToString(dataRow3["Name"]));
						DataSet dataSet = await _httpClient.ProjectManager.GetDataLineByWbsTemplate(text);
						if (dataSet != null)
						{
							tv_Wbs.ShowLoadingPanel();
							DataTable dataTable2 = dataSet.Tables[0];
							if (dataTable2 != null && dataTable2.Rows.Count > 0)
							{
								List<DataRow> list2 = (from t in dataTable2.AsEnumerable()
									where Function.ParseInt(t["FatherCode"]) == -1 || Function.ParseInt(t["FatherCode"]) == 0
									select t).ToList();
								if (list2 != null && list2.Count > 0)
								{
									for (int num5 = 0; num5 < list2.Count; num5++)
									{
										NewTask(linenum, Function.ToString(linenum));
										AddWbsDetailTemplate(dataTable2, list2[num5], linenum, Function.ToString(list2[num5]["Code"]));
									}
								}
							}
							tv_Wbs.HideLoadingPanel();
						}
					}
				}
			}
			else if (e.Column.FieldName == "Dependent" || e.Column.FieldName == "Wbs" || e.Column.FieldName == "Notes")
			{
				await InitAppointments();
			}
			else if (e.Column.FieldName == "FromDate" || e.Column.FieldName == "ToDate" || e.Column.FieldName == "TaskName")
			{
				_ = e.Node.Id;
				if (!Function.ParseDateTime(e.Value).HasValue)
				{
					return;
				}
				_dtItem = tv_Wbs.DataSourceTable();
				new List<DataRow>();
				(from DataRow dr in _dtItem.Rows
					select (dr)).ToList();
				if (e.Column.FieldName == "FromDate")
				{
					Function.ParseDateTimes(e.Value);
					double num6 = Function.ParseDouble(tv_Wbs.GetRowCellValue(e.Node, tl_Duration));
					Function.ParseDateTimes(tv_Wbs.GetRowCellValue(e.Node, tl_ToDate));
					if (num6 > 0.0)
					{
						tv_Wbs.DataSourceTable().Rows[e.Node.Id]["ToDate"] = Function.ParseDateTimes(tv_Wbs.GetRowCellValue(e.Node, tl_FromDate)).AddDays(num6 - 1.0);
						DateTime toDate = Function.ParseDateTimes(tv_Wbs.GetRowCellValue(e.Node, tl_FromDate)).AddDays(num6 - 1.0);
						SetToDate(fatherNum, dtTask, linenum, toDate, Function.ToString(tv_Wbs.GetRowCellValue(e.Node, tl_WBSMode)));
					}
					SetFromDate(fatherNum, dtTask);
				}
				else if (e.Column.FieldName == "ToDate")
				{
					DateTime dateTime3 = Function.ParseDateTimes(tv_Wbs.GetRowCellValue(e.Node, tl_FromDate));
					DateTime toDate2 = Function.ParseDateTimes(e.Value);
					Function.ParseDouble(tv_Wbs.GetRowCellValue(e.Node, tl_Duration));
					if (!string.IsNullOrEmpty(Function.ToString(dateTime3)))
					{
						int num7 = Math.Abs(toDate2.Subtract(dateTime3).Days);
						tv_Wbs.DataSourceTable().Rows[e.Node.Id]["Duration"] = num7 + 1;
					}
					SetToDate(fatherNum, dtTask, linenum, toDate2, Function.ToString(tv_Wbs.GetRowCellValue(e.Node, tl_WBSMode)));
					SetFromDate(fatherNum, dtTask);
				}
				tv_Wbs.DataSourceTable().AcceptChanges();
				await InitAppointments();
				_isChangeItem = true;
			}
			else if (e.Column.FieldName == "Duration")
			{
				Function.ParseDateTimes(tv_Wbs.GetRowCellValue(e.Node, tl_Duration));
				double num8 = Function.ParseDouble(e.Value);
				DateTime dateTime4 = Function.ParseDateTimes(tv_Wbs.GetRowCellValue(e.Node, tl_FromDate)).AddDays(num8 - 1.0);
				tv_Wbs.DataSourceTable().Rows[e.Node.Id]["ToDate"] = dateTime4;
				SetToDate(fatherNum, dtTask, linenum, dateTime4, Function.ToString(tv_Wbs.GetRowCellValue(e.Node, tl_WBSMode)));
			}
			else if (e.Column.FieldName == "WBSMode")
			{
				string text2 = Function.ToString(e.Value);
				string text3 = Function.ToString(tv_Wbs.GetRowCellValue(e.Node, tl_Dependent));
				if (string.Equals(text2, "A"))
				{
					DataRow[] array2 = (from t in dtTask.AsEnumerable()
						where Function.ParseInt(t["FatherNum"]) == linenum && !string.IsNullOrEmpty(Function.ToString(t["TaskCode"]))
						select t).ToArray();
					if (array2.Length != 0)
					{
						DateTime dateTime5 = array2.Select((DataRow t) => Function.ParseDateTimes(t["FromDate"])).ToArray().Min();
						DateTime dateTime6 = array2.Select((DataRow t) => Function.ParseDateTimes(t["ToDate"])).ToArray().Max();
						tv_Wbs.DataSourceTable().Rows[e.Node.Id]["ToDate"] = dateTime6;
						tv_Wbs.DataSourceTable().Rows[e.Node.Id]["FromDate"] = dateTime5;
						tv_Wbs.DataSourceTable().Rows[e.Node.Id]["Duration"] = Function.ParseDecimal(Function.ParseDateTimes(dateTime6).Subtract(Function.ParseDateTimes(dateTime5)).Days);
						SetFromDate(fatherNum, dtTask);
						SetToDate(fatherNum, dtTask, linenum, dateTime6, text2);
					}
					else if (!string.IsNullOrEmpty(text3))
					{
						string lineId2 = text3.Split(new char[1] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToArray().Max();
						DataRow dataRow4 = (from t in dtTask.AsEnumerable()
							where Function.ParseInt(t["LineNum"]) == Function.ParseInt(lineId2)
							select t).FirstOrDefault();
						if (dataRow4 != null)
						{
							tv_Wbs.DataSourceTable().Rows[e.Node.Id]["FromDate"] = Function.ParseDateTimes(dataRow4["ToDate"]).AddDays(1.0);
							SetFromDate(fatherNum, dtTask);
						}
					}
				}
			}
			DataRow[] source = tv_Wbs.DataSourceTable().Select(" Isnull(TaskCode,'') <> ''").ToArray();
			if (source.Count() > 0)
			{
				btn_TaskCode.DataSource = source.CopyToDataTable();
			}
			int fatherId = Function.ParseInt(tv_Wbs.GetRowCellValue(e.Node, tl_FatherNum));
			string line = Function.ToString(tv_Wbs.GetRowCellValue(e.Node, tl_LineNum));
			NewTask(fatherId, line);
			if (!_isLoading)
			{
				DataChanged();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void SetToDate(int fatherNum, DataTable dtTask, int lineNum, DateTime toDate, string wbsMode)
	{
		try
		{
			DataRow[] array = (from t in dtTask.AsEnumerable()
				where Function.ParseInt(t["Dependent"]) == lineNum
				select t).ToArray();
			if (array.Length != 0 && string.Equals(wbsMode, "A"))
			{
				for (int num = 0; num < array.Length; num++)
				{
					DateTime dateTime = toDate.AddDays(1.0);
					int fatherNum2 = Function.ParseInt(array[num]["FatherNum"]);
					int lineNum2 = Function.ParseInt(array[num]["LineNum"]);
					string wbsMode2 = Function.ToString(array[num]["WBSMode"]);
					int num2 = Function.ParseInt(array[num]["Duration"]);
					array[num]["FromDate"] = dateTime;
					array[num]["ToDate"] = dateTime.AddDays(num2 - 1);
					SetToDate(fatherNum2, dtTask, lineNum2, dateTime.AddDays(num2 - 1), wbsMode2);
				}
			}
			DataRow dataRow = (from t in dtTask.AsEnumerable()
				where Function.ParseInt(t["LineNum"]) == fatherNum
				select t).FirstOrDefault();
			if (dataRow == null || !string.Equals(Function.ToString(dataRow["WBSMode"]), "A"))
			{
				return;
			}
			DateTime value = Function.ParseDateTimes(dataRow["FromDate"]);
			DataRow[] array2 = (from t in dtTask.AsEnumerable()
				where Function.ParseInt(t["FatherNum"]) == fatherNum && !string.IsNullOrEmpty(Function.ToString(t["TaskCode"]))
				select t).ToArray();
			if (array2.Length != 0)
			{
				DateTime dateTime2 = array2.Select((DataRow t) => Function.ParseDateTimes(t["ToDate"])).ToArray().Max();
				dataRow["ToDate"] = dateTime2;
				dataRow["Duration"] = Function.ParseDecimal(dateTime2.Subtract(value).Days + 1);
			}
		}
		catch (Exception)
		{
			throw;
		}
	}

	private void SetFromDate(int fatherNum, DataTable dtTask)
	{
		try
		{
			DataRow dataRow = (from t in dtTask.AsEnumerable()
				where Function.ParseInt(t["LineNum"]) == fatherNum
				select t).FirstOrDefault();
			if (dataRow == null || !string.Equals(Function.ToString(dataRow["WBSMode"]), "A"))
			{
				return;
			}
			DateTime dateTime = Function.ParseDateTimes(dataRow["ToDate"]);
			DataRow[] array = (from t in dtTask.AsEnumerable()
				where Function.ParseInt(t["FatherNum"]) == fatherNum && !string.IsNullOrEmpty(Function.ToString(t["TaskCode"]))
				select t).ToArray();
			if (array.Length != 0)
			{
				DateTime dateTime2 = array.Select((DataRow t) => Function.ParseDateTimes(t["FromDate"])).ToArray().Min();
				dataRow["FromDate"] = dateTime2;
				int days = dateTime.Subtract(dateTime2).Days;
				dataRow["Duration"] = Function.ParseDecimal(days + 1);
			}
		}
		catch (Exception)
		{
			throw;
		}
	}

	private void UpdateFromDate(List<DataRow> lstItem, int fathernum, DateTime? value)
	{
		if (fathernum != 0)
		{
			IEnumerable<DataRow> source = from myRow in lstItem.AsEnumerable()
				where myRow.Field<int>("LineNum") == fathernum
				select myRow;
			if (!(Function.ParseDateTime(source.FirstOrDefault().Field<DateTime>("FromDate")) < value))
			{
				source.FirstOrDefault().SetField("FromDate", value);
				fathernum = Function.ParseInt(source.FirstOrDefault().Field<int>("FatherNum"));
				UpdateFromDate(lstItem, fathernum, value);
			}
		}
	}

	private void UpdateToDate(List<DataRow> lstItem, int fathernum, DateTime? value)
	{
		if (fathernum != 0)
		{
			IEnumerable<DataRow> source = from myRow in lstItem.AsEnumerable()
				where myRow.Field<int>("LineNum") == fathernum
				select myRow;
			if (!(Function.ParseDateTime(source.FirstOrDefault().Field<DateTime>("ToDate")) > value))
			{
				source.FirstOrDefault().SetField("ToDate", value);
				fathernum = Function.ParseInt(source.FirstOrDefault().Field<int>("FatherNum"));
				UpdateToDate(lstItem, fathernum, value);
			}
		}
	}

	private void tv_Wbs_FocusedNodeChanged(object sender, FocusedNodeChangedEventArgs e)
	{
		try
		{
			if (base.FormMode == Enums.FormViewModeType.Add || e.Node == null)
			{
				return;
			}
			DataRow dataRow = tv_Wbs.GetDataRow(e.Node.Id);
			if (dataRow != null)
			{
				if (string.IsNullOrEmpty(Function.ToString(dataRow["WbsTemplate"])))
				{
					genTasksToolStripMenuItem.Enabled = false;
				}
				else
				{
					genTasksToolStripMenuItem.Enabled = true;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void genTasksToolStripMenuItem_Click(object sender, EventArgs e)
	{
		_ = 4;
		try
		{
			DataTable dtTask = tv_Wbs.DataSourceTable();
			DataRow node = tv_Wbs.GetDataRow(tv_Wbs.FocusedNode.Id);
			if (node != null)
			{
				string wbsCode = Function.ToString(node["WbsTemplate"]);
				int baseLine = Function.ParseInt(node["LineNum"]);
				if (!string.IsNullOrEmpty(wbsCode))
				{
					DataTable dataTable = await _httpClient.ProjectManager.GetTaskByWbsCode("1470000132", Function.ParseInt(gv_Doc.GetFocusedRowCellValue("AbsEntry")), baseLine, wbsCode);
					if (dataTable != null && dataTable.Rows.Count > 0 && XtraMessageBoxEx.Show(Enums.MessageCode.ProjectManagerConfirmGenTaskWhenGenerated, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
					{
						return;
					}
					string[] source = Function.ToString(node["Resource"]).Trim().Split(';');
					source = source.Where((string c) => Function.ParseInt(c) > 0).ToArray();
					if (source.Length > 1)
					{
						WinCommons.Warning(Enums.MessageCode.TaskIsOnlyGenFor1Employee, "");
						return;
					}
					int num = 0;
					string[] array = source;
					foreach (string text in array)
					{
						if (!string.IsNullOrWhiteSpace(text))
						{
							num = Function.ParseInt(text);
						}
					}
					if (string.Equals(_httpClient.ProjectManager.CheckMltpBrnchs(), "Y"))
					{
						DataTable brachByEmpID = _httpClient.ProjectManager.GetBrachByEmpID(num);
						if (brachByEmpID == null || brachByEmpID.Rows.Count == 0)
						{
							WinCommons.Warning(Enums.MessageCode.EmployeeDoesNotHaveAWorkSchedule, "", Function.ToString(num));
							return;
						}
						int num3 = Function.ParseInt(brachByEmpID.Rows[0]["BPLId"]);
						DateTime dateTime = Function.ParseDateTimes(tv_Wbs.GetFocusedRowCellValue(tl_FromDate));
						if (string.IsNullOrEmpty(Function.ToString(dateTime)))
						{
							WinCommons.Warning(Enums.MessageCode.FromDateIsNull, "");
							return;
						}
						int num4 = Function.ParseInt(_httpClient.ProjectManager.GetIdWorkScheduling(dateTime.Year).Rows[0]["BplID"]);
						if (num3 != num4)
						{
							WinCommons.Warning(Enums.MessageCode.EmployeeDoesNotHaveAWorkSchedule, "", Function.ToString(num));
							return;
						}
					}
					if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmGenTask, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
					{
						WinCommons._OpenProcessing();
						DataTable dtnewTask = await _httpClient.ProjectManager.GetTaskTemplateByCode(wbsCode);
						DataTable dtTaskTime = new DataTable
						{
							Columns = 
							{
								{
									"LineId",
									typeof(int)
								},
								{
									"StartDate",
									typeof(DateTime)
								},
								{
									"StartTime",
									typeof(int)
								},
								{
									"EndDate",
									typeof(DateTime)
								},
								{
									"EndTime",
									typeof(int)
								}
							}
						};
						if (dtnewTask == null || dtnewTask.Rows.Count <= 0)
						{
							WinCommons.Warning(Enums.MessageCode.None, "Not data Task");
							return;
						}
						DateTime startDate = Function.ParseDateTimes(tv_Wbs.GetFocusedRowCellValue(tl_FromDate));
						DataTable dataTable2 = await _httpClient.ProjectManager.GetScheduleFromDate(startDate.ToString("yyyy-MM-dd"));
						if (dataTable2 == null || dataTable2.Rows.Count <= 0)
						{
							WinCommons.Warning(Enums.MessageCode.None, "Not data Schedule");
							return;
						}
						for (int num5 = 0; num5 < dtnewTask.Rows.Count; num5++)
						{
							int lineId = Function.ParseInt(dtnewTask.Rows[num5]["LineId"]);
							int leadTime = Function.ParseInt(dtnewTask.Rows[num5]["Leadtime"]);
							int startTime = Function.ParseInt(dataTable2.Rows[0]["Start1"]);
							GenerateTaskTime(dtnewTask, dtTaskTime, dataTable2, lineId, startTime, startDate, leadTime, 0);
						}
						string value = Function.ToString(GlobalData.UserID);
						DataTable tableStructure = WinFuntions.GetTableStructure("APZ_OCLG");
						tableStructure.Columns.Remove("OwnerAuth");
						int linetask = 0;
						for (int num6 = 0; num6 < dtnewTask.Rows.Count; num6++)
						{
							int num7 = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("AbsEntry"));
							string taskCode = ((!string.IsNullOrEmpty(Function.ToString(dtnewTask.Rows[num6]["WBSCode"]))) ? (Function.ToString(dtnewTask.Rows[num6]["Code"]) + "." + Function.ToString(dtnewTask.Rows[num6]["WBSCode"])) : Function.ToString(dtnewTask.Rows[num6]["Code"]));
							string value2 = Function.ToString(gv_Doc.GetFocusedRowCellValue("CardCode"));
							Function.ParseInt(dtnewTask.Rows[num6]["Leadtime"]);
							string value3 = "";
							DataRow dataRow = (from t in dtTask.AsEnumerable()
								where Function.ToString(t["TaskCode"]) == taskCode
								select t).FirstOrDefault();
							if (dataRow != null && !string.IsNullOrEmpty(Function.ToString(dataRow["Resource"])))
							{
								value3 = Function.ToString(dataRow["Resource"]);
							}
							linetask = Function.ParseInt(dtnewTask.Rows[num6]["LineId"]);
							DataRow dataRow2 = dtTaskTime.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["LineId"]) == linetask);
							if (dataRow2 != null)
							{
								DataRow dataRow3 = tableStructure.NewRow();
								if (string.IsNullOrEmpty(value3) && !string.IsNullOrEmpty(Function.ToString(dtnewTask.Rows[num6]["Employee"])))
								{
									value3 = Function.ToString(dtnewTask.Rows[num6]["Employee"]);
								}
								if (!string.IsNullOrEmpty(Function.ToString(dtnewTask.Rows[num6]["User"])))
								{
									value = Function.ToString(dtnewTask.Rows[num6]["User"]);
								}
								if (!string.IsNullOrEmpty(value3))
								{
									dataRow3["GroupEmployee"] = value3;
								}
								else
								{
									dataRow3["GroupUser"] = value;
								}
								dataRow3["Recontact"] = Function.ParseDateTime(dataRow2["StartDate"]);
								dataRow3["BeginTime"] = Function.ParseInt(dataRow2["StartTime"]);
								dataRow3["endDate"] = Function.ParseDateTime(dataRow2["EndDate"]);
								dataRow3["ENDTime"] = Function.ParseInt(dataRow2["EndTime"]);
								dataRow3["Closed"] = "N";
								dataRow3["ProjectEntry"] = num7;
								dataRow3["ProjectWBS"] = taskCode;
								dataRow3["CardCode"] = value2;
								if (string.IsNullOrEmpty(Function.ToString(dtnewTask.Rows[num6]["TaskName"])))
								{
									WinCommons.Warning(Enums.MessageCode.TaskNameIsRequired, "");
									return;
								}
								dataRow3["Details"] = Function.ToString(dtnewTask.Rows[num6]["TaskName"]);
								dataRow3["Action"] = "T";
								dataRow3["CntctDate"] = DateTime.Today;
								dataRow3["CntctTime"] = Function.ParseTimeSpanToInt(DateTime.Now.TimeOfDay);
								dataRow3["UpdateDate"] = DateTime.Today;
								dataRow3["UpdateTime"] = Function.ParseTimeSpanToInt(DateTime.Now.TimeOfDay);
								dataRow3["Notes"] = Function.ToString(dtnewTask.Rows[num6]["Note"]);
								dataRow3["AssignedBy"] = GlobalData.UserID;
								dataRow3["UserSign"] = GlobalData.UserID;
								dataRow3["BaseType"] = "1470000132";
								dataRow3["BaseEntry"] = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("AbsEntry"));
								dataRow3["BaseLine"] = Function.ParseInt(tv_Wbs.GetFocusedRowCellValue(tl_LineNum));
								dataRow3["BaseCode"] = wbsCode;
								dataRow3["status"] = 1;
								string value4 = ResMapping(dataRow3);
								dataRow3["ResourceIdsInfo"] = value4;
								dataRow3["OcrCode"] = Function.ToString(dtnewTask.Rows[num6]["OcrCode"]);
								dataRow3["OcrCode2"] = Function.ToString(dtnewTask.Rows[num6]["OcrCode2"]);
								dataRow3["OcrCode3"] = Function.ToString(dtnewTask.Rows[num6]["OcrCode3"]);
								dataRow3["OcrCode4"] = Function.ToString(dtnewTask.Rows[num6]["OcrCode4"]);
								dataRow3["OcrCode5"] = Function.ToString(dtnewTask.Rows[num6]["OcrCode5"]);
								dataRow3["Owner"] = GlobalData.UserID;
								tableStructure.Rows.Add(dataRow3);
								tableStructure.AcceptChanges();
							}
						}
						if (!tableStructure.IsNotNull())
						{
							WinCommons.Warning(Enums.MessageCode.FromDateDoesNotHaveWorkScheduleYet, "");
							return;
						}
						HttpResult httpResult = await _httpClient.SubActivities.AddMuntiTask(tableStructure);
						if (!string.IsNullOrEmpty(httpResult.Message))
						{
							WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
							return;
						}
						WinCommons.Success(Enums.MessageCode.GenTaskSuccessfully, "");
						await ReloadTaskList(0);
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private void GenerateTaskTime(DataTable dtNewTask, DataTable dtTaskTime, DataTable dtschedult, int lineId, int startTime, DateTime? startDate, int leadTime, int startIndex)
	{
		double num = 0.0;
		for (int i = startIndex; i < dtschedult.Rows.Count; i++)
		{
			DateTime? dateTime = Function.ParseDateTime(dtschedult.Rows[i]["WorkDate"]);
			int num2 = Function.ParseInt(dtschedult.Rows[i]["Start1"]);
			int num3 = Function.ParseInt(dtschedult.Rows[i]["End1"]);
			int num4 = Function.ParseInt(dtschedult.Rows[i]["Start2"]);
			int num5 = Function.ParseInt(dtschedult.Rows[i]["End2"]);
			int val = num2 % 100;
			int val2 = num3 % 100;
			int val3 = num4 % 100;
			int num6 = Math.Min(val2: Math.Min(val3, num5 % 100), val1: Math.Min(val, val2));
			int num7 = 100;
			num7 = ((num6 == 0) ? 100 : num6);
			int num8 = num2;
			if (dateTime == startDate)
			{
				num8 = ((startTime > num2) ? startTime : num2);
			}
			else
			{
				num8 = num2;
			}
			int num9 = num8;
			for (int j = num8; j < num3; j += num7)
			{
				num9 += num7;
				num = ((leadTime != 0) ? (num + 60.0 / (double)((num7 == 100) ? 60 : num7)) : 0.0);
				if (num >= (double)leadTime)
				{
					break;
				}
			}
			if (num < (double)leadTime)
			{
				if (dateTime == startDate)
				{
					num8 = ((startTime > num4) ? startTime : num4);
				}
				else
				{
					num8 = num4;
				}
				num9 = num8;
				for (int k = num8; k < num5; k += num7)
				{
					num9 += num7;
					num += 60.0 / (double)((num7 == 100) ? 60 : num7);
					if (num >= (double)leadTime)
					{
						break;
					}
				}
			}
			if (num == (double)leadTime)
			{
				DataRow dataRow = dtTaskTime.NewRow();
				dataRow["StartTime"] = startTime;
				dataRow["EndTime"] = num9;
				dataRow["LineId"] = lineId;
				dataRow["StartDate"] = startDate;
				dataRow["EndDate"] = dateTime;
				dtTaskTime.Rows.Add(dataRow);
				DataRow[] array = (from t in dtNewTask.AsEnumerable()
					where (from x in Function.ToString(t["Dependence"]).Split(',')
						select Function.ParseInt(x)).Contains(lineId)
					select t).ToArray();
				if (array.Length == 0)
				{
					break;
				}
				DataTable dataTable = array.CopyToDataTable();
				for (int num10 = 0; num10 < dataTable.Rows.Count; num10++)
				{
					int num11 = num9 + num7;
					DateTime? startDate2 = dateTime;
					int startIndex2 = i;
					if (!Enumerable.Range(num2, num3 - num2).Contains(num11) && !Enumerable.Range(num4, num5 - num4).Contains(num11))
					{
						startDate2 = dateTime.Value.AddDays(1.0);
						startIndex2 = i + 1;
						num11 = Function.ParseInt(dtschedult.Rows[i]["Start1"]);
					}
					GenerateTaskTime(dtNewTask, dtTaskTime, dtschedult, Function.ParseInt(dataTable.Rows[num10]["LineId"]), num11, startDate2, Function.ParseInt(dataTable.Rows[num10]["LeadTime"]), startIndex2);
				}
				break;
			}
			_ = (double)leadTime;
		}
	}

	private void setTime(int hour, DataTable dtShift, DataRow RowShift, DataRow dr, ref DateTime startDate, ref int endTime)
	{
		try
		{
			DataTable dataWorkSchedule = GetDataWorkSchedule(startDate.Year);
			if (dataWorkSchedule == null)
			{
				return;
			}
			if (dtShift != null && dtShift.Rows.Count > 0)
			{
				int num = Function.ParseInt(RowShift["Start1"]);
				if (endTime == 0)
				{
					endTime = num;
				}
				int num2 = Function.ParseInt(RowShift["End1"]);
				int num3 = num2 - num;
				if (hour < num3)
				{
					if (endTime + hour > num2)
					{
						startDate = startDate.AddDays(1.0);
						int hourValue = hour - (num2 - endTime);
						endTime = -1;
						DataTable dataWorkSchedule2 = GetDataWorkSchedule(startDate.Year);
						CheckDateShift(hourValue, dtShift, dataWorkSchedule2, dr, ref startDate, ref endTime);
					}
					if (endTime + hour == num2)
					{
						dr["endDate"] = startDate;
						dr["ENDTime"] = num2;
						startDate = startDate.AddDays(1.0);
						endTime = -1;
						GetDataWorkSchedule(startDate.Year);
						CheckShift(dtShift, dataWorkSchedule, ref startDate, ref endTime);
					}
					else
					{
						dr["endDate"] = startDate;
						dr["ENDTime"] = endTime + hour;
						endTime = Function.ParseInt(dr["ENDTime"]);
					}
				}
				else if (hour == num3)
				{
					dr["endDate"] = startDate;
					dr["ENDTime"] = num2;
					if (dtShift.Rows.Count > 1)
					{
						DataRow dataRow = (from t in dtShift.AsEnumerable()
							where Function.ParseInt(t["ID"]) == Function.ParseInt(RowShift["ID"]) + 1
							select t).FirstOrDefault();
						if (dataRow != null)
						{
							endTime = Function.ParseInt(dataRow["Start1"]);
							return;
						}
						startDate = startDate.AddDays(1.0);
						endTime = -1;
						GetDataWorkSchedule(startDate.Year);
						CheckShift(dtShift, dataWorkSchedule, ref startDate, ref endTime);
					}
					else
					{
						startDate = startDate.AddDays(1.0);
						endTime = -1;
						GetDataWorkSchedule(startDate.Year);
						CheckShift(dtShift, dataWorkSchedule, ref startDate, ref endTime);
					}
				}
				else
				{
					if (hour <= num3)
					{
						return;
					}
					if (dtShift.Rows.Count > 1)
					{
						DataRow dataRow2 = (from t in dtShift.AsEnumerable()
							where Function.ParseInt(t["ID"]) == Function.ParseInt(RowShift["ID"]) + 1
							select t).FirstOrDefault();
						if (dataRow2 != null)
						{
							int hour2 = hour - (num2 - endTime);
							endTime = Function.ParseInt(dataRow2["Start1"]);
							setTime(hour2, dtShift, dataRow2, dr, ref startDate, ref endTime);
						}
						else
						{
							startDate = startDate.AddDays(1.0);
							int hourValue2 = hour - (num2 - endTime);
							endTime = -1;
							DataTable dataWorkSchedule3 = GetDataWorkSchedule(startDate.Year);
							CheckDateShift(hourValue2, dtShift, dataWorkSchedule3, dr, ref startDate, ref endTime);
						}
					}
					else
					{
						int hourValue3 = hour - (num2 - endTime);
						startDate = startDate.AddDays(1.0);
						endTime = -1;
						DataTable dataWorkSchedule4 = GetDataWorkSchedule(startDate.Year);
						CheckDateShift(hourValue3, dtShift, dataWorkSchedule4, dr, ref startDate, ref endTime);
					}
				}
			}
			else
			{
				startDate = startDate.AddDays(1.0);
				DataTable dataWorkSchedule5 = GetDataWorkSchedule(startDate.Year);
				CheckDateShift(hour, dtShift, dataWorkSchedule5, dr, ref startDate, ref endTime);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void CheckDateShift(int hourValue, DataTable dtShift, DataTable dtWorkSchedure, DataRow dr, ref DateTime startDate, ref int endtime)
	{
		try
		{
			DateTime date = startDate;
			List<DataRow> list = (from t in dtWorkSchedure.AsEnumerable()
				where Function.ParseDateTimes(t["WorkDate"]) == date
				select t).ToList();
			if (list == null || list.Count <= 0)
			{
				return;
			}
			DataTable dataTable = list.CopyToDataTable();
			DataTable shifOrderByTime = GetShifOrderByTime(Function.ToString(dataTable.Rows[0]["Shift"]));
			if (shifOrderByTime != null && shifOrderByTime.Rows.Count > 0)
			{
				if (endtime == -1)
				{
					endtime = Function.ParseInt(shifOrderByTime.Rows[0]["Start1"]);
				}
				setTime(hourValue, dtShift, shifOrderByTime.Rows[0], dr, ref startDate, ref endtime);
			}
			else
			{
				startDate = startDate.AddDays(1.0);
				CheckDateShift(hourValue, dtShift, dtWorkSchedure, dr, ref startDate, ref endtime);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void CheckShift(DataTable dtShift, DataTable dtWorkSchedure, ref DateTime startDate, ref int endtime)
	{
		try
		{
			DateTime date = startDate;
			List<DataRow> list = (from t in dtWorkSchedure.AsEnumerable()
				where Function.ParseDateTimes(t["WorkDate"]) == date
				select t).ToList();
			if (list == null || list.Count <= 0)
			{
				return;
			}
			DataTable dataTable = list.CopyToDataTable();
			DataTable shifOrderByTime = GetShifOrderByTime(Function.ToString(dataTable.Rows[0]["Shift"]));
			if (shifOrderByTime != null && shifOrderByTime.Rows.Count > 0)
			{
				if (endtime == -1)
				{
					endtime = Function.ParseInt(shifOrderByTime.Rows[0]["Start1"]);
				}
			}
			else
			{
				startDate = startDate.AddDays(1.0);
				CheckShift(dtShift, dtWorkSchedure, ref startDate, ref endtime);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public static string ResMapping(DataRow dr)
	{
		try
		{
			ResourceIdCollection resourceIdCollection = new ResourceIdCollection();
			SchedulerCompatibility.Base64XmlObjectSerialization = false;
			string text = Function.ToString(dr["GroupUser"]);
			string text2 = Function.ToString(dr["GroupEmployee"]);
			if (text != "")
			{
				string text3 = text;
				if (!string.IsNullOrEmpty(text3))
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
				if (!string.IsNullOrEmpty(text3))
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
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
			return "";
		}
	}

	private void treeList1_CustomColumnDisplayText(object sender, DevExpress.XtraTreeList.CustomColumnDisplayTextEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == "Resource")
			{
				string[] array = Function.ToString(tv_Wbs.GetRowCellValue(e.Node, tl_Resource)).Split(';');
				if (array.Length != 0)
				{
					List<string> list = new List<string>();
					string[] array2 = array;
					foreach (string text in array2)
					{
						string item = (btn_Owner.dictionSource.ContainsKey(Function.ToString(text)) ? btn_Owner.dictionSource[Function.ToString(text)] : "");
						list.Add(item);
					}
					e.DisplayText = string.Join(";", list);
				}
			}
			if (e.Column.FieldName == "FromDate")
			{
				e.DisplayText = string.Format("{0:" + GlobalData.DateTimeFormat + "} ", e.Value);
			}
			else if (e.Column.FieldName == "ToDate")
			{
				e.DisplayText = string.Format("{0:" + GlobalData.DateTimeFormat + "} ", e.Value);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_NewTask_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			int fatherId = Function.ParseInt(tv_Wbs.FocusedNode.GetValue("LineNum"));
			NewTask(fatherId);
			tv_Wbs.FocusedNode.Expand();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void newTrackingIssueToolStripMenuItem_Click(object sender, EventArgs e)
	{
		newTrackingIssueToolStripMenuItem1_Click(sender, e);
	}

	private void newTrackingIssueToolStripMenuItem1_Click(object sender, EventArgs e)
	{
		try
		{
			int num = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("AbsEntry"));
			string value = Function.ToString(tv_Wbs.GetFocusedRowCellValue(tl_Code));
			int num2 = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("Owner"));
			string value2 = Function.ToString(tv_Wbs.GetFocusedRowCellValue(tl_Name));
			string value3 = Function.ToString(gv_Doc.GetFocusedRowCellValue("CardCode"));
			int userID = GlobalData.UserID;
			DataTable tableStructure = WinFuntions.GetTableStructure("APZ_TGIE");
			tableStructure.Columns.Add("SolutionDesc");
			tableStructure.Columns.Add("TaskName");
			DataRow dataRow = tableStructure.NewRow();
			dataRow["Priority"] = "2";
			dataRow["Status"] = "1";
			dataRow["IssueDate"] = DateTime.Today;
			dataRow["Reporter"] = userID;
			dataRow["ProjectCode"] = num;
			dataRow["ProjectOwner"] = num2;
			dataRow["WbsCode"] = value;
			dataRow["TaskName"] = value2;
			dataRow["BpCode"] = value3;
			tableStructure.Rows.Add(dataRow);
			tableStructure.AcceptChanges();
			frm_TrackingIssue frm_TrackingIssue2 = new frm_TrackingIssue();
			frm_TrackingIssue2.MdiParent = MdiParent;
			frm_TrackingIssue2.CallFromCal = true;
			frm_TrackingIssue2.DataRowFromCal = dataRow;
			frm_TrackingIssue2.StartPosition = FormStartPosition.CenterScreen;
			frm_TrackingIssue2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void treeList1_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
	{
		try
		{
			int result = 0;
			if (!(sender is TreeList treeList))
			{
				return;
			}
			if (treeList.FocusedColumn.FieldName == "Dependent" && !string.IsNullOrEmpty(Function.ToString(e.Value)))
			{
				int[] array = (from t in Function.ToString(e.Value).Split(',')
					select Function.ParseInt(t)).ToArray();
				int lineNum = Function.ParseInt(tv_Wbs.GetFocusedRowCellValue(tl_LineNum));
				int num = Function.ParseInt(tv_Wbs.GetFocusedRowCellValue(tl_visOrder));
				if (array.Length != 0)
				{
					List<int> lst = new List<int>();
					GetAllFatherId(lineNum, ref lst);
					List<int> list = new List<int>();
					GetAllChildId(lineNum, ref lst);
					int[] array2 = array;
					foreach (int num3 in array2)
					{
						if (num3 <= 0)
						{
							e.Valid = false;
							e.ErrorText = "";
							WinCommons.Error(Enums.MessageCode.TypeOnlyPositiveWholeNumber4SimpleRelationshipBetweenWbs, "");
							return;
						}
						if (num3 == num)
						{
							e.Valid = false;
							e.ErrorText = "";
							WinCommons.Error(Enums.MessageCode.YouCannotMakeWbsPredecessorOfItself, "");
							return;
						}
						if (lst.Contains(num3))
						{
							e.Valid = false;
							e.ErrorText = "";
							WinCommons.Error(Enums.MessageCode.ProjectCannotLinkSummaryWbs2ItsSub, "");
							return;
						}
						if (list.Contains(num3))
						{
							e.Valid = false;
							e.ErrorText = "";
							WinCommons.Error(Enums.MessageCode.ProjectCannotLinkSummaryWbs2ItsSub, "");
							return;
						}
						if (Function.ParseInt(num3) > 0 && !int.TryParse(Function.ToString(num3), out result))
						{
							e.Valid = false;
							e.ErrorText = "";
							WinCommons.Error(Enums.MessageCode.PredecessorOnlyInputFromLineNum, "");
							return;
						}
					}
				}
			}
			if (treeList.FocusedColumn.FieldName == "FromDate")
			{
				DateTime? dateTime = Function.ParseDateTime(gv_Doc.GetFocusedRowCellValue("DueDate"));
				DateTime? dateTime2 = Function.ParseDateTime(e.Value);
				if (dateTime.HasValue && dateTime2.HasValue && dateTime2 > dateTime)
				{
					e.Valid = false;
					e.ErrorText = "";
					WinCommons.Error(Enums.MessageCode.WbsFromDateShouldNotGreaterThanProjectDueDate, "");
					return;
				}
				DateTime? dateTime3 = Function.ParseDateTime(tv_Wbs.FocusedNode.GetValue(tl_ToDate));
				if (dateTime3.HasValue && dateTime2.HasValue && dateTime2 > dateTime3)
				{
					e.Valid = false;
					e.ErrorText = "";
					WinCommons.Error(Enums.MessageCode.WbsFromDateShouldNotGreaterThanWbsDueDate, "");
					return;
				}
			}
			if (treeList.FocusedColumn.FieldName == "ToDate")
			{
				DateTime? dateTime4 = Function.ParseDateTime(e.Value);
				DateTime? dateTime5 = Function.ParseDateTime(tv_Wbs.FocusedNode.GetValue(tl_FromDate));
				if (dateTime4.HasValue && dateTime5.HasValue && dateTime5 > dateTime4)
				{
					e.Valid = false;
					e.ErrorText = "";
					WinCommons.Error(Enums.MessageCode.WbsDueDateShouldNotLessWbsFromDate, "");
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void newTaskToolStripMenuItem1_Click(object sender, EventArgs e)
	{
		try
		{
			int num = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("AbsEntry"));
			string value = Function.ToString(tv_Wbs.GetFocusedRowCellValue(tl_Code));
			frm_TaskManagement frm_TaskManagement2 = new frm_TaskManagement
			{
				ClgCode = 0,
				StartPosition = FormStartPosition.CenterScreen,
				MdiParent = MdiParent,
				CallFromCal = true
			};
			DataTable tableStructure = WinFuntions.GetTableStructure("APZ_OCLG");
			DataRow dataRow = tableStructure.NewRow();
			string value2 = "";
			string text = Function.ToString(tv_Wbs.GetFocusedRowCellValue(tl_Resource));
			if (text != null)
			{
				value2 = text;
			}
			if (!string.IsNullOrEmpty(value2))
			{
				dataRow["GroupEmployee"] = value2;
			}
			else
			{
				dataRow["GroupUser"] = GlobalData.UserID;
			}
			if (Function.ToString(dataRow["Recontact"]) == "")
			{
				dataRow["Recontact"] = DateTime.Today;
				string text2 = DateTime.Now.Minute.ToString();
				dataRow["BeginTime"] = DateTime.Now.Hour + ((text2.Length == 2) ? text2 : ("0" + text2));
			}
			dataRow["Closed"] = "N";
			dataRow["ProjectEntry"] = num;
			dataRow["ProjectWBS"] = value;
			tableStructure.Rows.Add(dataRow);
			tableStructure.AcceptChanges();
			frm_TaskManagement2.DataRowFromCal = dataRow;
			frm_TaskManagement2.ReloadCalendar = (Func<Task>)Delegate.Combine(frm_TaskManagement2.ReloadCalendar, new Func<Task>(ReloadTaskList));
			frm_TaskManagement2.ReloadAfterRemove = (Func<int, Task>)Delegate.Combine(frm_TaskManagement2.ReloadAfterRemove, new Func<int, Task>(ReloadTaskList));
			frm_TaskManagement2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void txt_TaskCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			int fatherId = Function.ParseInt(tv_Wbs.FocusedNode.GetValue("LineNum"));
			NewTask(fatherId);
			tv_Wbs.FocusedNode.Expand();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void treeList1_InvalidValueException(object sender, InvalidValueExceptionEventArgs e)
	{
		if (sender is TreeList)
		{
			e.ExceptionMode = ExceptionMode.DisplayError;
			e.WindowCaption = "Input Error";
		}
	}

	private void treeList1_CustomDrawNodeIndicator(object sender, CustomDrawNodeIndicatorEventArgs e)
	{
		(e.ObjectArgs as IndicatorObjectInfoArgs).DisplayText = Function.ToString(Function.ParseInt(e.Node.GetValue("VisOrder")));
	}

	private void toolStripMenuItemIndent_Click(object sender, EventArgs e)
	{
		try
		{
			_isinout = true;
			int previousNodeSameLevel = GetPreviousNodeSameLevel(tv_Wbs.FocusedNode);
			if (previousNodeSameLevel != -1)
			{
				string wbs = Function.ToString(tv_Wbs.FindNodeByKeyID(previousNodeSameLevel).GetValue("Wbs"));
				tv_Wbs.FocusedNode.SetValue("FatherNum", previousNodeSameLevel);
				tv_Wbs.DataSourceTable.AcceptChanges();
				RefWbs(tv_Wbs.FindNodeByKeyID(previousNodeSameLevel), tv_Wbs.DataSourceTable(), previousNodeSameLevel, wbs, "Y");
				RefreshVisOrder();
				tv_Wbs.DataSourceTable.AcceptChanges();
				DataChanged();
			}
			_isinout = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void toolStripMenuItemOutdent_Click(object sender, EventArgs e)
	{
		try
		{
			_isinout = true;
			TreeListNode parentNode = tv_Wbs.FocusedNode.ParentNode;
			TreeListNode treeListNode = tv_Wbs.FocusedNode.ParentNode;
			if (parentNode != null)
			{
				tv_Wbs.FocusedNode.SetValue("FatherNum", parentNode.GetValue("FatherNum"));
				if (Function.ParseInt(parentNode.GetValue("FatherNum")) > 0)
				{
					treeListNode = tv_Wbs.FindNodeByKeyID(parentNode.GetValue("FatherNum"));
				}
				tv_Wbs.DataSourceTable.AcceptChanges();
				RefWbs(parentNode, tv_Wbs.DataSourceTable(), Function.ParseInt(parentNode.GetValue("FatherNum")), Function.ToString(treeListNode.GetValue("Wbs")), "Y");
				RefreshVisOrder();
				tv_Wbs.DataSourceTable.AcceptChanges();
				DataChanged();
			}
			_isinout = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void tv_Wbs_CustomColumnSort(object sender, DevExpress.XtraTreeList.CustomColumnSortEventArgs e)
	{
		try
		{
			if (e.Column.FieldName != "Wbs")
			{
				return;
			}
			try
			{
				string text = Function.ToString(e.NodeValue1);
				string text2 = Function.ToString(e.NodeValue2);
				e.Result = Function.CompareString(text, text2);
			}
			catch
			{
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ctm_Rows_Opening(object sender, CancelEventArgs e)
	{
		try
		{
			TreeListNode focusedNode = tv_Wbs.FocusedNode;
			int previousNodeSameLevel = GetPreviousNodeSameLevel(tv_Wbs.FocusedNode);
			toolStripMenuItemOutdent.Enabled = focusedNode != null && focusedNode.ParentNode != null;
			toolStripMenuItemIndent.Enabled = focusedNode != null && previousNodeSameLevel != -1;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void viewOrgChartToolStripMenuItem_Click(object sender, EventArgs e)
	{
		WinCommons._OpenProcessing();
		try
		{
			frm_ProjectManagerOrgChart frm_ProjectManagerOrgChart2 = new frm_ProjectManagerOrgChart();
			frm_ProjectManagerOrgChart2.Source = tv_Wbs.DataSourceTable();
			frm_ProjectManagerOrgChart2.Project = gv_Doc.GetFocusedDataRow();
			frm_ProjectManagerOrgChart2.MdiParent = MdiParent;
			frm_ProjectManagerOrgChart2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private void addRowToolStripMenuItem1_Click(object sender, EventArgs e)
	{
		NewItem();
	}

	private void toolStripMenuItemInsertRowBudget_Click(object sender, EventArgs e)
	{
		try
		{
			int id = tv_budget.FocusedNode.Id;
			_dtItem = tv_budget.DataSourceTable();
			if (_dtItem == null || _dtItem.Columns.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.InfoCannotLoadData, "");
				return;
			}
			int num = Function.ParseInt(_dtItem.Compute("MAX(LineNum)", ""));
			int num2 = Function.ParseInt(tv_budget.GetRowCellValue(tv_budget.FocusedNode, tb_FatherNum));
			int num3 = Function.ParseInt(tv_budget.GetRowCellValue(tv_budget.FocusedNode, tb_BaseLine));
			string text = Function.ToString(tv_budget.FocusedNode.GetValue(tl_Wbs));
			DataRow dataRow = _dtItem.NewRow();
			dataRow["FatherNum"] = num2;
			dataRow["LineNum"] = num + 1;
			dataRow["Wbs"] = text + 1;
			dataRow["Currency"] = Function.ToString(gv_Doc.GetFocusedRowCellValue("DocCur"));
			if (num2 > 0 && num3 > 0)
			{
				dataRow["IsNew"] = false;
				dataRow["BaseLine"] = num3;
			}
			else
			{
				dataRow["IsNew"] = true;
				dataRow["BaseLine"] = 0;
			}
			_dtItem.Rows.InsertAt(dataRow, id);
			_dtItem.AcceptChanges();
			TreeListNode focusedNode = tv_budget.FindNodeByKeyID(num + 1);
			tv_budget.FocusedNode = focusedNode;
			RefreshWbs(tv_budget.FocusedNode, tv_budget.DataSourceTable, "Y");
			RefreshVisItem(_dtItem);
			DataChanged();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void removeRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			int id = Function.ParseInt(tv_budget.FocusedNode.GetValue("AbsEntry"));
			int num = Function.ParseInt(tv_budget.FocusedNode.GetValue("LineNum"));
			string text = Function.ToString(tv_budget.FocusedNode.GetValue("ItemCode"));
			string value = _httpClient.ProjectManager.CheckItemInDocument(id, num);
			int fatherId = Function.ParseInt(tv_budget.FocusedNode.GetValue("FatherNum"));
			if (!string.IsNullOrEmpty(value))
			{
				WinCommons.Warning(Enums.MessageCode.ItemCodeIsBeingLinked2Document, "", text);
			}
			else
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDeleteByLineNum, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, Function.ToString(num)) != DialogResult.Yes)
				{
					return;
				}
				TreeListNode focusedNode = tv_budget.FocusedNode;
				Function.ToString(focusedNode.GetValue(tb_TaskCode));
				_dtItem = tv_budget.DataSourceTable();
				_dtDoc = grd_Doc.DataSourceTable;
				tv_Wbs.DataSourceTable();
				tv_budget.DeleteSelectedNodes();
				tv_budget.DataSourceTable().AcceptChanges();
				string text2 = Function.ToString(focusedNode.GetValue(tl_Wbs));
				_ = focusedNode.Level;
				(from t in text2.Split('.')
					select Function.ParseInt(t)).ToArray();
				string nextwbs = "";
				DataRow dataRow = _dtItem.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["Wbs"]) == nextwbs);
				if (dataRow != null)
				{
					int num2 = Function.ParseInt(dataRow["LineNum"]);
					TreeListNode treeListNode = tv_budget.FindNodeByKeyID(num2);
					if (treeListNode != null)
					{
						treeListNode.SetValue("Wbs", text2);
						RefreshWbs(treeListNode, _dtItem, "Y");
					}
				}
				RefreshVisItem(tv_budget.DataSourceTable);
				AmountFather(_dtItem, fatherId);
				ActualAmountFather(_dtItem, fatherId);
				SumAmountHeader(_dtItem, _dtDoc);
				DataChanged();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void toolStripMenuItemDuplicateRowBudget_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataTable = tv_budget.DataSourceTable();
			_dtDoc = grd_Doc.DataSourceTable;
			tv_Wbs.DataSourceTable();
			int num = Function.ParseInt(dataTable.Compute("MAX(LineNum)", ""));
			if (dataTable == null || dataTable.Columns.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.CannotGetData, "");
				return;
			}
			DataRow focusedDataRow = tv_budget.GetFocusedDataRow();
			if (focusedDataRow != null)
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow.ItemArray = focusedDataRow.ItemArray;
				dataRow["LineNum"] = num + 1;
				string text = Function.ToString(tv_budget.FocusedNode.GetValue(tl_Wbs));
				dataRow["Wbs"] = text + 1;
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
				int fatherId = Function.ParseInt(tv_budget.FocusedNode.GetValue("FatherNum"));
				Function.ToString(tv_budget.FocusedNode.GetValue("TaskCode"));
				AmountFather(dataTable, fatherId);
				ActualAmountFather(dataTable, fatherId);
				SumAmountHeader(dataTable, _dtDoc);
				TreeListNode focusedNode = tv_budget.FindNodeByKeyID(num + 1);
				tv_budget.FocusedNode = focusedNode;
				RefreshWbs(tv_budget.FocusedNode, tv_budget.DataSourceTable, "Y");
				RefreshVisItem(tv_budget.DataSourceTable);
				DataChanged();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void addItemRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			int fatherId = Function.ParseInt(tv_budget.FocusedNode.GetValue("FatherNum"));
			string line = Function.ToString(tv_budget.FocusedNode.GetValue("LineNum"));
			NewItemTree(fatherId, line);
			tv_budget.FocusedNode.Expand();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void insertItemRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			int num = Function.ParseInt(tv_budget.FocusedNode.GetValue("LineNum"));
			NewItemTree(num, Function.ToString(num));
			tv_budget.FocusedNode.Expand();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void toolStripMenuItemAddSub_Click(object sender, EventArgs e)
	{
		addSubProjectToolStripMenuItem_Click(null, null);
	}

	private void toolStripMenuItemAddNew_Click(object sender, EventArgs e)
	{
		newTrackingIssueToolStripMenuItem_Click(null, null);
	}

	private void toolStripMenuItemNewTask_Click(object sender, EventArgs e)
	{
		newTaskToolStripMenuItem_Click(sender, e);
	}

	private void relatedActivitiesToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			int num = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("AbsEntry"));
			if (num > 0)
			{
				string cardCode = Function.ToString(gv_Doc.GetFocusedRowCellValue("CardCode"));
				string wbsCode = Function.ToString(tv_Wbs.GetFocusedRowCellValue(tl_Code));
				frm_RelatedActivites frm_RelatedActivites2 = new frm_RelatedActivites(ApzObjectTypes.oProjectManager, Function.ToString(num));
				frm_RelatedActivites2.MdiParent = MdiParent;
				frm_RelatedActivites2.CardCode = cardCode;
				frm_RelatedActivites2.WbsCode = wbsCode;
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

	private void gv_lineIssue_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
	{
		try
		{
			Brush brush = new SolidBrush(Color.FromArgb(252, 189, 189));
			Brush brush2 = new SolidBrush(Color.FromArgb(194, 223, 255));
			Brush brush3 = new SolidBrush(Color.FromArgb(255, 205, 133));
			Brush brush4 = new SolidBrush(Color.FromArgb(227, 183, 235));
			Brush brush5 = new SolidBrush(Color.FromArgb(255, 244, 148));
			Brush brush6 = new SolidBrush(Color.FromArgb(210, 245, 176));
			Brush brush7 = new SolidBrush(Color.FromArgb(255, 0, 0));
			Brush brush8 = new SolidBrush(Color.FromArgb(201, 204, 196));
			int num = 16;
			e.Appearance.Options.UseTextOptions = true;
			e.DefaultDraw();
			if (e.Column.FieldName == "Status" && !string.IsNullOrEmpty(Function.ToString(e.CellValue)))
			{
				e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
				int num2 = Convert.ToInt32(e.CellValue);
				e.Graphics.FillRectangle(num2 switch
				{
					1 => brush, 
					2 => brush2, 
					3 => brush3, 
					4 => brush4, 
					5 => brush5, 
					6 => brush6, 
					7 => brush7, 
					_ => brush8, 
				}, new Rectangle(e.Bounds.Location, new Size(num, num)));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void tv_Wbs_EditorKeyDown(object sender, KeyEventArgs e)
	{
		KeysEvent(sender, e);
	}

	private void tv_Wbs_KeyDown(object sender, KeyEventArgs e)
	{
		KeysEvent(sender, e);
	}

	private void KeysEvent(object sender, KeyEventArgs e)
	{
		try
		{
			DataTable dataTable = tv_Wbs.DataSourceTable();
			if (!(sender is TreeList treeList))
			{
				return;
			}
			if (e.KeyCode == Keys.Return)
			{
				tv_Wbs.SetFocusedRowCellValue(treeList.FocusedColumn.FieldName, treeList.EditingValue);
				int lineNum = Function.ParseInt(tv_Wbs.FocusedNode.GetValue("FatherNum"));
				string line = Function.ToString(tv_Wbs.FocusedNode.GetValue("LineNum"));
				NewTask(lineNum, line);
				tv_Wbs.FocusedNode.Expand();
				DataRow dataRow = (from t in dataTable.AsEnumerable()
					where Function.ParseInt(t["FatherNum"]) == lineNum
					select t).ToArray().ToList().FirstOrDefault((DataRow t) => Function.ToString(t["TaskCode"]) == "");
				if (dataRow != null)
				{
					string cellValue = Function.ToString(dataRow["Wbs"]);
					tv_Wbs.FocusedNode = tv_Wbs.FindNodeByFieldValue("Wbs", cellValue);
				}
			}
			if (e.KeyCode == Keys.Insert)
			{
				int lineNum2 = Function.ParseInt(tv_Wbs.FocusedNode.GetValue("LineNum"));
				NewTask(lineNum2, Function.ToString(lineNum2));
				tv_Wbs.FocusedNode.Expand();
				DataRow dataRow2 = (from t in dataTable.AsEnumerable()
					where Function.ParseInt(t["FatherNum"]) == lineNum2
					select t).ToArray().ToList().FirstOrDefault((DataRow t) => Function.ToString(t["TaskCode"]) == "");
				if (dataRow2 != null)
				{
					string cellValue2 = Function.ToString(dataRow2["Wbs"]);
					tv_Wbs.FocusedNode = tv_Wbs.FindNodeByFieldValue("Wbs", cellValue2);
				}
			}
			if (e.Modifiers == (Keys.Shift | Keys.Alt) && e.KeyCode == Keys.Right)
			{
				_isinout = true;
				int previousNodeSameLevel = GetPreviousNodeSameLevel(tv_Wbs.FocusedNode);
				if (previousNodeSameLevel != -1)
				{
					string wbs = Function.ToString(tv_Wbs.FindNodeByKeyID(previousNodeSameLevel).GetValue("Wbs"));
					tv_Wbs.FocusedNode.SetValue("FatherNum", previousNodeSameLevel);
					tv_Wbs.DataSourceTable.AcceptChanges();
					RefWbs(tv_Wbs.FindNodeByKeyID(previousNodeSameLevel), tv_Wbs.DataSourceTable(), previousNodeSameLevel, wbs, "Y");
					RefreshVisOrder();
					tv_Wbs.DataSourceTable.AcceptChanges();
					DataChanged();
				}
				_isinout = false;
			}
			if (e.Modifiers == (Keys.Shift | Keys.Alt) && e.KeyCode == Keys.Left)
			{
				_isinout = true;
				TreeListNode parentNode = tv_Wbs.FocusedNode.ParentNode;
				TreeListNode treeListNode = tv_Wbs.FocusedNode.ParentNode;
				if (parentNode != null)
				{
					tv_Wbs.FocusedNode.SetValue("FatherNum", parentNode.GetValue("FatherNum"));
					if (Function.ParseInt(parentNode.GetValue("FatherNum")) > 0)
					{
						treeListNode = tv_Wbs.FindNodeByKeyID(parentNode.GetValue("FatherNum"));
					}
					tv_Wbs.DataSourceTable.AcceptChanges();
					RefWbs(parentNode, tv_Wbs.DataSourceTable(), Function.ParseInt(parentNode.GetValue("FatherNum")), Function.ToString(treeListNode.GetValue("Wbs")), "Y");
					RefreshVisOrder();
					tv_Wbs.DataSourceTable.AcceptChanges();
					DataChanged();
				}
				_isinout = false;
			}
			if (e.KeyCode != Keys.Delete || e.Modifiers != Keys.Shift)
			{
				return;
			}
			DeleteTeam();
			if (dataTable == null || dataTable.Columns.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.InfoCannotLoadData, "");
			}
			TreeListNode focusedNode = tv_Wbs.FocusedNode;
			string text = Function.ToString(focusedNode.GetValue(tl_Wbs));
			int level = focusedNode.Level;
			int[] array = (from t in text.Split('.')
				select Function.ParseInt(t)).ToArray();
			string nextwbs = "";
			if (array.Length > level)
			{
				array[level]++;
				nextwbs = string.Join(".", array);
			}
			if (focusedNode.ParentNode != null)
			{
				focusedNode.ParentNode.Nodes.Remove(focusedNode);
			}
			else
			{
				tv_Wbs.Nodes.Remove(focusedNode);
			}
			dataTable.AcceptChanges();
			DataRow dataRow3 = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["Wbs"]) == nextwbs);
			if (dataRow3 != null)
			{
				int num = Function.ParseInt(dataRow3["LineNum"]);
				TreeListNode treeListNode2 = tv_Wbs.FindNodeByKeyID(num);
				if (treeListNode2 != null)
				{
					treeListNode2.SetValue("Wbs", text);
					RefreshWbs(treeListNode2, dataTable, "Y");
				}
			}
			RefreshVisOrder();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void KeysBudgetItem(object sender, KeyEventArgs e)
	{
		try
		{
			_dtDoc = grd_Doc.DataSourceTable;
			_dtItem = tv_budget.DataSourceTable();
			if (sender is TreeList)
			{
				Function.ParseInt(_dtItem.Compute("MAX(LineNum)", ""));
				if (e.KeyCode == Keys.Insert)
				{
					string text = Function.ToString(tv_budget.FocusedNode.GetValue("LineNum"));
					NewItemTree(Function.ParseInt(text), text);
					tv_budget.FocusedNode.Expand();
				}
				if (e.Modifiers == Keys.Shift && e.KeyCode == Keys.Delete)
				{
					int fatherId = Function.ParseInt(tv_budget.FocusedNode.GetValue("FatherNum"));
					tv_budget.DeleteSelectedNodes();
					tv_budget.DataSourceTable().AcceptChanges();
					AmountFather(_dtItem, fatherId);
					ActualAmountFather(_dtItem, fatherId);
					SumAmountHeader(_dtItem, _dtDoc);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void tv_budget_CellValueChanged(object sender, DevExpress.XtraTreeList.CellValueChangedEventArgs e)
	{
		_ = 5;
		try
		{
			_dtDoc = grd_Doc.DataSourceTable;
			_dtItem = tv_budget.DataSourceTable();
			string whs = Function.ToString(gv_Doc.GetFocusedRowCellValue("DefaultWhs"));
			int fatherId = Function.ParseInt(tv_budget.GetRowCellValue(e.Node, tb_FatherNum));
			int lineNum = Function.ParseInt(tv_budget.GetRowCellValue(e.Node, tb_LineNum));
			string currencyDoc = Function.ToString(gv_Doc.GetFocusedRowCellValue("DocCur"));
			if (GlobalData.CurrencyCodes != null && GlobalData.CurrencyCodes.Count > 0)
			{
				_ = GlobalData.CurrencyCodes.FirstOrDefault((CurrencyCodesImpl t) => t.CurrCode.ToUpper().Equals(currencyDoc.ToUpper()))?.Decimals;
			}
			if (e.Column.FieldName == "PlanQty")
			{
				double num = Function.ParseDouble(tv_budget.GetRowCellValue(e.Node, tb_plQty));
				double num2 = Function.ParseDouble(tv_budget.GetRowCellValue(e.Node, tb_RefPrice));
				double num3 = Function.ParseDouble(tv_budget.GetRowCellValue(e.Node, tb_ActualQty)) - num;
				double num4 = num * num2;
				string bom = Function.ToString(tv_budget.GetRowCellValue(e.Node, tb_BomItem));
				double num5 = Function.ParseDouble(tv_budget.GetRowCellValue(e.Node, tb_BaseQty));
				double num6 = ((tv_budget.ActiveEditor != null) ? Function.ParseDouble(tv_budget.ActiveEditor.OldEditValue) : 0.0);
				double num7 = num5;
				if (fatherId > 0)
				{
					TreeListNode treeListNode = tv_budget.FindNodeByKeyID(fatherId);
					if (treeListNode != null)
					{
						double num8 = Function.ParseDouble(treeListNode.GetValue(tb_plQty));
						double num9 = Function.ParseDouble(treeListNode.GetValue(tb_BaseQty));
						if (num8 != 0.0 && num9 != 0.0 && !string.IsNullOrEmpty(Function.ToString(treeListNode.GetValue(tb_BomItem))))
						{
							num7 = num * num9 / num8;
							tv_budget.DataSourceTable().Rows[e.Node.Id]["BaseQty"] = num7;
						}
					}
				}
				if (num5 != 0.0 && num7 != 0.0 && num6 != 0.0 && num != 0.0)
				{
					double num10 = SetPlanQty(bom, lineNum, num7 / num5, num / num6);
					if (num2 == 0.0)
					{
						num4 = num10;
					}
				}
				tv_budget.DataSourceTable().Rows[e.Node.Id]["Amount"] = num4;
				AmountFather(_dtItem, fatherId);
				if (_dtItem != null && _dtItem.Rows.Count > 0)
				{
					tv_budget.DataSourceTable().Rows[e.Node.Id]["QtyDiff"] = num3;
				}
			}
			else if (e.Column.FieldName == "RefPrice")
			{
				double num11 = Function.ParseDouble(tv_budget.GetRowCellValue(e.Node, tb_plQty));
				double num12 = Function.ParseDouble(e.Value);
				double num13 = num11 * num12;
				tv_budget.DataSourceTable().Rows[e.Node.Id]["Amount"] = num13;
				AmountFather(_dtItem, fatherId);
				ActualAmountFather(_dtItem, fatherId);
			}
			else if (e.Column.FieldName == "ActualQty")
			{
				double num14 = Function.ParseDouble(tv_budget.GetRowCellValue(e.Node, tb_plQty));
				double num15 = Function.ParseDouble(tv_budget.GetRowCellValue(e.Node, tb_ActualQty));
				double num16 = Function.ParseDouble(tv_budget.GetRowCellValue(e.Node, tb_RefPrice));
				double num17 = num15 - num14;
				if (_dtItem != null && _dtItem.Rows.Count > 0)
				{
					tv_budget.DataSourceTable().Rows[e.Node.Id]["QtyDiff"] = num17;
				}
				double num18 = num16 * num15;
				if (_dtItem != null && _dtItem.Rows.Count > 0)
				{
					tv_budget.SetRowCellValue(e.Node, "ActualAmount", num18);
					ActualAmountFather(_dtItem, fatherId);
				}
			}
			else if (e.Column.FieldName == "OcrCode")
			{
				string ocrCode = Function.ToString(e.Value);
				await SetOrcCode(ocrCode, e, "OcrCode", 1, tv_budget);
			}
			else if (e.Column.FieldName == "OcrCode2")
			{
				string ocrCode2 = Function.ToString(e.Value);
				await SetOrcCode(ocrCode2, e, "OcrCode2", 2, tv_budget);
			}
			else if (e.Column.FieldName == "OcrCode3")
			{
				string ocrCode3 = Function.ToString(e.Value);
				await SetOrcCode(ocrCode3, e, "OcrCode3", 3, tv_budget);
			}
			else if (e.Column.FieldName == "OcrCode4")
			{
				string ocrCode4 = Function.ToString(e.Value);
				await SetOrcCode(ocrCode4, e, "OcrCode4", 4, tv_budget);
			}
			else if (e.Column.FieldName == "OcrCode5")
			{
				string ocrCode5 = Function.ToString(e.Value);
				await SetOrcCode(ocrCode5, e, "OcrCode5", 5, tv_budget);
			}
			else if (e.Column.FieldName == "Amount")
			{
				double num19 = Function.ParseDouble(tv_budget.GetRowCellValue(e.Node, tb_plQty));
				double num20 = Function.ParseDouble(tv_budget.GetRowCellValue(e.Node, tb_RefPrice));
				AmountFather(_dtItem, fatherId);
				double num21 = Function.ParseDouble(e.Value);
				if (num19 != 0.0 && (!e.Node.Nodes.Any() || num20 != 0.0))
				{
					tv_budget.DataSourceTable().Rows[e.Node.Id]["RefPrice"] = num21 / num19;
				}
			}
			else if (e.Column.FieldName == "ActualAmount")
			{
				ActualAmountFather(_dtItem, fatherId);
			}
			else if (e.Column.FieldName == "BomItem")
			{
				if (isChooseBom)
				{
					return;
				}
				string bomItem = Function.ToString(e.Value);
				DataRow dataRow = (from t in btn_BOM.DataSource.AsEnumerable()
					where Function.ToString(t["Code"]) == bomItem
					select t).FirstOrDefault();
				if (dataRow != null)
				{
					string value = Function.ToString(dataRow["Code"]);
					if (!Function.ToString(tv_budget.GetRowCellValue(e.Node, tb_ItemCode)).Equals(bomItem))
					{
						tv_budget.SetRowCellValue(e.Node, "ItemCode", value);
					}
				}
			}
			else if (e.Column.FieldName == "ItemCode")
			{
				string itemCode = Function.ToString(e.Value);
				if (itemCode != "" && !HttpClientHelperFactory.GetHttpClient().BillOfMasterials.IsValidCode(4, itemCode).IsNotNull())
				{
					WinCommons.Warning(Enums.MessageCode.ItemCodeDoesNotExistsWithCode, "", itemCode);
					tv_budget.SetRowCellValue(e.Node, "ItemCode", DBNull.Value);
					return;
				}
				if (CancelItemChangeEvent)
				{
					return;
				}
				Function.ToString(tv_budget.GetRowCellValue(e.Node, tb_TaskCode));
				tv_Wbs.DataSourceTable();
				DataRow itemInfor = (from t in btn_ItemCode.DataSource.AsEnumerable()
					where Function.ToString(t["ItemCode"]) == itemCode
					select t).FirstOrDefault();
				if (string.IsNullOrEmpty(itemCode))
				{
					tv_budget.SetRowCellValue(e.Node, "Dscription", DBNull.Value);
					tv_budget.SetRowCellValue(e.Node, "PlanQty", DBNull.Value);
					tv_budget.SetRowCellValue(e.Node, "ItmsGrpCod", DBNull.Value);
					tv_budget.SetRowCellValue(e.Node, "DefaultWhs", DBNull.Value);
				}
				if (itemInfor != null)
				{
					DataTable dataTable = await _httpClient.ProjectManager.GetTreeData(itemCode);
					if (dataTable.IsNotNull())
					{
						CancelItemChangeEvent = true;
						WinCommons._OpenProcessing();
						DataRow dataRow2 = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["ItemCode"]).Equals(itemCode));
						double num22 = Function.ParseDouble(dataRow2["Quantity"]);
						tv_budget.SetRowCellValue(e.Node, tb_IsNew, true);
						tv_budget.SetRowCellValue(e.Node, tb_ItemName, Function.ToString(itemInfor["ItemName"]));
						tv_budget.SetRowCellValue(e.Node, tb_UoM, Function.ToString(itemInfor["InvntryUom"]));
						if (!Function.ToString(tv_budget.GetRowCellValue(e.Node, tb_BomItem)).Equals(itemCode))
						{
							tv_budget.SetRowCellValue(e.Node, tb_BomItem, itemCode);
						}
						if (!_isOrder)
						{
							tv_budget.SetRowCellValue(e.Node, tb_plQty, 1);
						}
						tv_budget.SetRowCellValue(e.Node, tb_BaseQty, num22);
						tv_budget.SetRowCellValue(e.Node, tb_ItmsGrpCod, Function.ParseInt(itemInfor["ItmsGrpCod"]));
						tv_budget.SetRowCellValue(e.Node, tb_Amount, Function.ParseDouble(itemInfor["Price"]));
						if (string.IsNullOrEmpty(whs))
						{
							tv_budget.SetRowCellValue(e.Node, tb_DefaultWhs, Function.ToString(dataRow2["WhsCode"]));
						}
						else
						{
							tv_budget.SetRowCellValue(e.Node, tb_DefaultWhs, whs);
						}
						if (Function.ToString(gv_Doc.GetFocusedRowCellValue("LoadBom")) == "Y")
						{
							SetTreeDataToView(e.Node, dataTable.AsEnumerable().ToList(), Function.ParseInt(dataRow2["ID"]), 1.0, num22, whs);
						}
						AmountFather(_dtItem, Function.ParseInt(e.Node.GetValue("FatherNum")));
						SumAmountHeader(tv_budget.DataSourceTable, grd_Doc.DataSourceTable);
						CancelItemChangeEvent = false;
						WinCommons._CloseProcessing();
					}
					else
					{
						tv_budget.SetRowCellValue(e.Node, tb_ItemName.FieldName, Function.ToString(itemInfor["ItemName"]));
						tv_budget.SetRowCellValue(e.Node, tb_UoM, Function.ToString(itemInfor["InvntryUom"]));
						tv_budget.SetRowCellValue(e.Node, tb_ItmsGrpCod, Function.ParseInt(itemInfor["ItmsGrpCod"]));
						if (!string.IsNullOrEmpty(whs))
						{
							tv_budget.SetRowCellValue(e.Node, tb_DefaultWhs, whs);
						}
						else
						{
							tv_budget.SetRowCellValue(e.Node, tb_DefaultWhs, Function.ToString(itemInfor["dfltWH"]));
						}
						tv_budget.SetRowCellValue(e.Node, tb_IsNew, true);
					}
				}
				NewItemTree(fatherId, Function.ToString(lineNum));
				tv_budget.DataSourceTable().AcceptChanges();
			}
			else if (e.Column.FieldName == "TaskCode" && Function.ParseInt(tv_budget.GetRowCellValue(e.Node, tb_BaseLine)) > 0)
			{
				return;
			}
			if (e.Column.FieldName == "PlanQty" || e.Column.FieldName == "RefPrice" || e.Column.FieldName == "ActualQty" || e.Column.FieldName == "Amount")
			{
				SumAmountHeader(_dtItem, _dtDoc);
			}
			if (!_isCpyWbs)
			{
				tv_budget.DataSourceTable().AcceptChanges();
				if (!_isLoading)
				{
					DataChanged();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void SetTreeDataToView(TreeListNode fatherNode, List<DataRow> dtTree, int fatherId, double fatherPlan, double fatherBase, string defWhs)
	{
		List<DataRow> list = dtTree.Where((DataRow t) => Function.ParseInt(t["FatherId"]) == fatherId).ToList();
		if (!list.Any())
		{
			return;
		}
		string currencyDoc = Function.ToString(gv_Doc.GetFocusedRowCellValue("DocCur"));
		if (GlobalData.CurrencyCodes != null && GlobalData.CurrencyCodes.Count > 0)
		{
			_ = GlobalData.CurrencyCodes.FirstOrDefault((CurrencyCodesImpl t) => t.CurrCode.ToUpper().Equals(currencyDoc.ToUpper()))?.Decimals;
		}
		fatherNode.SetValue(tb_BomItem, fatherNode.GetValue(tb_ItemCode));
		double num = 0.0;
		int num2 = Function.ParseInt(fatherNode.GetValue("LineNum"));
		AddListTreeItem(num2, list.Count, Function.ToString(fatherNode.GetValue("Wbs")), Function.ParseInt(fatherNode.GetValue("BaseLine")));
		fatherNode = tv_budget.FindNodeByKeyID(num2);
		for (int num3 = 0; num3 < list.Count; num3++)
		{
			DataRow dataRow = list[num3];
			TreeListNode treeListNode = fatherNode.Nodes[num3];
			DataRow dataRow2 = tv_budget.GetDataRow(treeListNode.Id);
			double num4 = Function.ParseDouble(dataRow["Quantity"]) * fatherPlan / Function.ParseDouble(dataRow["FatherQty"]);
			double num5 = Function.ParseDouble(dataRow["Quantity"]) * fatherBase / Function.ParseDouble(dataRow["FatherQty"]);
			dataRow2["Dscription"] = dataRow["ItemName"];
			dataRow2["UoM"] = dataRow["InvntryUom"];
			dataRow2["PlanQty"] = num4;
			dataRow2["RefPrice"] = dataRow["Price"];
			dataRow2["Amount"] = num4 * Function.ParseDouble(dataRow["Price"]);
			dataRow2["BaseQty"] = num5;
			dataRow2["ItmsGrpCod"] = dataRow["ItmsGrpCod"];
			dataRow2["IsNew"] = true;
			if (string.IsNullOrEmpty(defWhs))
			{
				dataRow2["DefaultWhs"] = dataRow["WhsCode"];
			}
			else
			{
				dataRow2["DefaultWhs"] = defWhs;
			}
			tv_budget.SetRowCellValue(treeListNode, "ItemCode", dataRow["ItemCode"]);
			SetTreeDataToView(treeListNode, dtTree, Function.ParseInt(dataRow["ID"]), num4, num5, defWhs);
			num += Function.ParseDouble(treeListNode.GetValue("Amount"));
			dtTree.Remove(dataRow);
		}
		if (Function.ParseDecimal(fatherNode.GetValue("RefPrice")) == 0m)
		{
			fatherNode.SetValue("Amount", num);
		}
	}

	private void AddListTreeItem(int fatherId, int ChildCount, string wbs, int baseLine)
	{
		if (Function.ParseInt(gv_Doc.GetFocusedRowCellValue("BudgetLevel")) <= 0)
		{
			return;
		}
		DataTable dataSourceTable = tv_budget.DataSourceTable;
		if (dataSourceTable == null)
		{
			return;
		}
		int num = Function.ParseInt(dataSourceTable.Compute("MAX(LineNum)", ""));
		string value = Function.ToString(gv_Doc.GetFocusedRowCellValue("DocCur"));
		for (int i = 0; i < ChildCount; i++)
		{
			DataRow dataRow = dataSourceTable.NewRow();
			dataRow["FatherNum"] = fatherId;
			dataRow["LineNum"] = ++num;
			dataRow["Currency"] = value;
			dataRow["OrderItem"] = -1;
			if (baseLine > 0)
			{
				dataRow["BaseLine"] = baseLine;
				dataRow["IsNew"] = false;
			}
			else
			{
				dataRow["IsNew"] = true;
			}
			dataRow["Wbs"] = string.Join(".", wbs, i + 1);
			dataSourceTable.Rows.Add(dataRow);
		}
		dataSourceTable.AcceptChanges();
	}

	private double SetPlanQty(string bom, int lineNum, double changeRateBase, double changeRateLplan)
	{
		double num = 0.0;
		if (!string.IsNullOrEmpty(bom))
		{
			List<DataRow> list = (from t in _dtItem.AsEnumerable()
				where Function.ParseInt(t["FatherNum"]) == lineNum
				select t).ToList();
			if (list != null && list.Count > 0)
			{
				for (int num2 = 0; num2 < list.Count; num2++)
				{
					list[num2]["BaseQty"] = Function.ParseDouble(list[num2]["BaseQty"]) * changeRateBase;
					list[num2]["PlanQty"] = Function.ParseDouble(list[num2]["PlanQty"]) * changeRateLplan;
					double num3 = Function.ParseDouble(list[num2]["RefPrice"]);
					string bom2 = Function.ToString(list[num2]["BomItem"]);
					int lineNum2 = Function.ParseInt(list[num2]["LineNum"]);
					double num4 = SetPlanQty(bom2, lineNum2, changeRateBase, changeRateLplan);
					double num5 = ((num3 != 0.0) ? (Function.ParseDouble(list[num2]["PlanQty"]) * num3) : num4);
					list[num2]["Amount"] = num5;
					num += num5;
				}
			}
		}
		return num;
	}

	private async Task SetOrcCode(string ocrCode, DevExpress.XtraTreeList.CellValueChangedEventArgs e, string colunm, int p, TreeEx tree)
	{
		try
		{
			if (ocrCode != "" && (from t in (await LoadDataOrcCode(p)).AsEnumerable()
				where Function.ToString(t["OcrCode"]) == ocrCode
				select t).FirstOrDefault() == null)
			{
				WinCommons.Warning(Enums.MessageCode.ThisItemDoesNotExistsInList, "", ocrCode);
				tree.SetRowCellValue(e.Node, colunm, DBNull.Value);
			}
		}
		catch (Exception)
		{
			throw;
		}
	}

	private async Task SetOrcCodeGrid(string ocrCode, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e, string colunm, int p)
	{
		try
		{
			if (ocrCode != "" && (from t in (await LoadDataOrcCode(p)).AsEnumerable()
				where Function.ToString(t["OcrCode"]) == ocrCode
				select t).FirstOrDefault() == null)
			{
				WinCommons.Warning(Enums.MessageCode.ThisItemDoesNotExistsInList, "", ocrCode);
				gv_Doc.SetRowCellValue(e.RowHandle, colunm, DBNull.Value);
			}
		}
		catch (Exception)
		{
			throw;
		}
	}

	private void btn_AbsEntry_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			int num = Function.ParseInt(tv_budget.FocusedNode.GetValue("LineNum"));
			NewItemTree(num, Function.ToString(num));
			tv_budget.FocusedNode.Expand();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void tv_budget_EditorKeyDown(object sender, KeyEventArgs e)
	{
		KeysBudgetItem(sender, e);
	}

	private void tv_budget_KeyDown(object sender, KeyEventArgs e)
	{
		KeysBudgetItem(sender, e);
	}

	private void tv_budget_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (Function.ToString(gv_Doc.GetFocusedRowCellValue("OwnerAuth")) != "F")
			{
				e.Cancel = true;
				return;
			}
			if (Function.ToString(gv_Doc.GetFocusedRowCellValue("OwnerAuth")) == "R")
			{
				tv_budget.Columns["Notes"].OptionsColumn.ReadOnly = false;
				return;
			}
			string value = Function.ToString(tv_budget.GetFocusedRowCellValue(tb_TaskCode));
			int num = Function.ParseInt(tv_budget.GetFocusedRowCellValue(tb_OrderItem));
			bool flag = Function.ParseBool(tv_budget.GetFocusedRowCellValue(tb_IsNew));
			int num2 = Function.ParseInt(tv_budget.GetFocusedRowCellValue(tb_BaseLine));
			if (new List<string>
			{
				"ItemCode", "Dscription", "PlanQty", "RefPrice", "Amount", "BplId", "QtyDiff", "FromDate", "ToDate", "OrderItem",
				"PR", "PO", "GRPO", "IGE", "AR", "Delivery", "Invoice", "UoM", "BomItem", "DefaultWhs"
			}.Contains(tv_budget.FocusedColumn.FieldName) && !string.IsNullOrEmpty(value) && !flag)
			{
				tv_budget.Columns["Notes"].OptionsColumn.ReadOnly = false;
				e.Cancel = true;
			}
			else
			{
				tv_budget.Columns["LineNum"].OptionsColumn.ReadOnly = true;
				tv_budget.Columns["AbsEntry"].OptionsColumn.ReadOnly = true;
				tv_budget.Columns["ActualQty"].OptionsColumn.ReadOnly = true;
				tv_budget.Columns["ActualAmount"].OptionsColumn.ReadOnly = true;
				tv_budget.Columns["ItmsGrpCod"].OptionsColumn.ReadOnly = true;
				tv_budget.Columns["BaseQty"].OptionsColumn.ReadOnly = true;
			}
			if (tv_budget.FocusedColumn.FieldName == "TaskCode" && num2 == 0)
			{
				tv_budget.Columns["TaskCode"].OptionsColumn.ReadOnly = false;
			}
			if (tv_budget.FocusedColumn.FieldName == "TaskCode" && num2 > 0)
			{
				tv_budget.Columns["TaskCode"].OptionsColumn.ReadOnly = true;
			}
			if (tv_budget.FocusedColumn.FieldName == "TaskName" && num2 == 0)
			{
				tv_budget.Columns["TaskName"].OptionsColumn.ReadOnly = false;
			}
			if (num > -1 && (tv_budget.FocusedColumn.FieldName == "ItemCode" || tv_budget.FocusedColumn.FieldName == "Dscription" || tv_budget.FocusedColumn.FieldName == "UoM" || tv_budget.FocusedColumn.FieldName == "PlanQty" || tv_budget.FocusedColumn.FieldName == "BaseQty" || tv_budget.FocusedColumn.FieldName == "RefPrice" || tv_budget.FocusedColumn.FieldName == "ItmsGrpCod" || tv_budget.FocusedColumn.FieldName == "Amount"))
			{
				e.Cancel = true;
			}
			string a = Function.ToString(gv_Doc.GetFocusedRowCellValue("PrgStatus"));
			if (string.Equals(a, "C") || string.Equals(a, "L"))
			{
				e.Cancel = true;
			}
			string a2 = Function.ToString(gv_Doc.GetFocusedRowCellValue("BaseLine"));
			if (!string.Equals(Function.ToString(gv_Doc.GetFocusedRowCellValue("WddStatus")), "-") || string.Equals(a2, "Y"))
			{
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void tv_budget_NodeCellStyle(object sender, GetCustomNodeCellStyleEventArgs e)
	{
		try
		{
			if (!CancelItemChangeEvent)
			{
				string value = Function.ToString(tv_budget.GetRowCellValue(e.Node, tb_TaskCode));
				bool flag = Function.ParseBool(tv_budget.GetRowCellValue(e.Node, tb_IsNew));
				List<string> list = new List<string>
				{
					"PR", "PO", "GRPO", "IGE", "Delivery", "Invoice", "AR", "PQ", "ActualQty", "ActualAmount",
					"ItmsGrpCod", "BaseQty", "ARDR", "APDR"
				};
				List<string> list2 = new List<string>
				{
					"TaskCode", "TaskName", "ItemCode", "Dscription", "PlanQty", "Dscription", "RefPrice", "Amount", "BplId", "QtyDiff",
					"FromDate", "ToDate", "OrderItem", "UoM", "BomItem", "DefaultWhs"
				};
				string a = Function.ToString(gv_Doc.GetFocusedRowCellValue("PrgStatus"));
				if (string.Equals(a, "C") || string.Equals(a, "L"))
				{
					e.Appearance.BackColor = GridColor.GridReadOnly;
				}
				if (!flag && e.Column.FieldName == "TaskCode")
				{
					e.Appearance.BackColor = GridColor.GridReadOnly;
				}
				if (!flag && e.Column.FieldName == "TaskName")
				{
					e.Appearance.BackColor = GridColor.GridReadOnly;
				}
				if (list.Contains(e.Column.FieldName))
				{
					e.Appearance.BackColor = GridColor.GridReadOnly;
				}
				if (!string.IsNullOrEmpty(value) && list2.Contains(e.Column.FieldName) && !flag)
				{
					e.Appearance.BackColor = GridColor.GridReadOnly;
				}
				if (!string.IsNullOrEmpty(value) && e.Column.FieldName == "Dscription")
				{
					e.Appearance.BackColor = GridColor.GridReadOnly;
				}
				string a2 = Function.ToString(gv_Doc.GetFocusedRowCellValue("BaseLine"));
				if (!string.Equals(Function.ToString(gv_Doc.GetFocusedRowCellValue("WddStatus")), "-") || string.Equals(a2, "Y"))
				{
					e.Appearance.BackColor = GridColor.GridReadOnly;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void tv_budget_FocusedNodeChanged(object sender, FocusedNodeChangedEventArgs e)
	{
		try
		{
			Function.ParseBool(tv_budget.GetFocusedRowCellValue(tb_IsNew));
			string value = Function.ToString(tv_budget.GetRowCellValue(e.Node, tb_TaskCode));
			int num = Function.ParseInt(tv_budget.GetRowCellValue(e.Node, tb_BaseLine));
			if (!string.IsNullOrEmpty(value) && num > 0)
			{
				toolStripMenuItemDuplicateRowBudget.Enabled = false;
				removeRowToolStripMenuItem.Enabled = false;
			}
			else
			{
				toolStripMenuItemDuplicateRowBudget.Enabled = true;
				removeRowToolStripMenuItem.Enabled = true;
			}
			string a = Function.ToString(gv_Doc.GetFocusedRowCellValue("BaseLine"));
			if (!string.Equals(Function.ToString(gv_Doc.GetFocusedRowCellValue("WddStatus")), "-") || string.Equals(a, "Y"))
			{
				removeRowToolStripMenuItem.Enabled = false;
				addRowToolStripMenuItem1.Enabled = false;
				toolStripMenuItemInsertRowBudget.Enabled = false;
				addItemRowToolStripMenuItem.Enabled = false;
				insertItemRowToolStripMenuItem.Enabled = false;
			}
			else
			{
				removeRowToolStripMenuItem.Enabled = true;
				addRowToolStripMenuItem1.Enabled = true;
				toolStripMenuItemInsertRowBudget.Enabled = true;
				addItemRowToolStripMenuItem.Enabled = true;
				insertItemRowToolStripMenuItem.Enabled = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void toolStripMenuItem2_Click(object sender, EventArgs e)
	{
		NewPayment();
	}

	private void toolStripMenuItem3_Click(object sender, EventArgs e)
	{
		try
		{
			int id = tv_Payments.FocusedNode.Id;
			_dtPayment = tv_Payments.DataSourceTable();
			if (_dtPayment == null || _dtPayment.Columns.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.InfoCannotLoadData, "");
				return;
			}
			int num = Function.ParseInt(tv_Payments.GetRowCellValue(tv_Payments.FocusedNode, tp_FatherNum));
			int num2 = Function.ParseInt(tv_Payments.GetRowCellValue(tv_Payments.FocusedNode, tp_BaseLine));
			int num3 = Function.ParseInt(_dtPayment.Compute("MAX(LineNum)", ""));
			DataRow dataRow = _dtPayment.NewRow();
			dataRow["FatherNum"] = num;
			dataRow["LineNum"] = num3 + 1;
			if (num > 0 && num2 > 0)
			{
				dataRow["IsNew"] = false;
				dataRow["BaseLine"] = num2;
			}
			else
			{
				dataRow["IsNew"] = true;
				dataRow["BaseLine"] = 0;
			}
			_dtPayment.Rows.InsertAt(dataRow, id);
			_dtPayment.AcceptChanges();
			RefreshVisOrderPayment();
			DataChanged();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void RefreshVisOrderPayment()
	{
		DataTable dataSourceTable = tv_Payments.DataSourceTable;
		if (dataSourceTable.IsNotNull())
		{
			int num = 1;
			foreach (DataRow item in (from t in dataSourceTable.AsEnumerable()
				select (t)).ToList())
			{
				item["VisOrder"] = num;
				num++;
			}
		}
		DataChanged();
	}

	private void toolStripMenuItem4_Click(object sender, EventArgs e)
	{
		try
		{
			_dtPayment = tv_Payments.DataSourceTable();
			_dtDoc = grd_Doc.DataSourceTable;
			tv_Wbs.DataSourceTable();
			int num = Function.ParseInt(_dtPayment.Compute("MAX(LineNum)", ""));
			if (_dtPayment == null || _dtPayment.Columns.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.CannotGetData, "");
				return;
			}
			DataRow focusedDataRow = tv_Payments.GetFocusedDataRow();
			if (focusedDataRow != null)
			{
				DataRow dataRow = _dtPayment.NewRow();
				dataRow.ItemArray = focusedDataRow.ItemArray;
				dataRow["LineNum"] = num + 1;
				_dtPayment.Rows.Add(dataRow);
				_dtPayment.AcceptChanges();
				int fatherId = Function.ParseInt(tv_Payments.FocusedNode.GetValue("FatherNum"));
				InvoicePlanFather(_dtPayment, fatherId);
				IncomingPaymentFather(_dtPayment, fatherId);
				TreeListNode focusedNode = tv_Payments.FindNodeByKeyID(num + 1);
				tv_Payments.FocusedNode = focusedNode;
				DataChanged();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void toolStripMenuItem5_Click(object sender, EventArgs e)
	{
		try
		{
			Function.ToString(tv_Payments.FocusedNode.GetValue(tb_TaskCode));
			_dtPayment = tv_Payments.DataSourceTable();
			_dtDoc = grd_Doc.DataSourceTable;
			tv_Wbs.DataSourceTable();
			Function.ParseInt(tv_Payments.FocusedNode.GetValue("AbsEntry"));
			tv_Payments.DeleteSelectedNodes();
			tv_Payments.DataSourceTable().AcceptChanges();
			int fatherId = Function.ParseInt(tv_Payments.FocusedNode.GetValue("FatherNum"));
			InvoicePlanFather(_dtPayment, fatherId);
			IncomingPaymentFather(_dtPayment, fatherId);
			DataChanged();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void tv_Payments_CellValueChanged(object sender, DevExpress.XtraTreeList.CellValueChangedEventArgs e)
	{
		try
		{
			_dtDoc = grd_Doc.DataSourceTable;
			_dtPayment = tv_Payments.DataSourceTable();
			if (e.Column.FieldName == "InvoicePlan")
			{
				int fatherId = Function.ParseInt(tv_budget.GetRowCellValue(e.Node, tb_FatherNum));
				InvoicePlanFather(_dtPayment, fatherId);
			}
			if (e.Column.FieldName == "IncomingPayment")
			{
				int fatherId2 = Function.ParseInt(tv_budget.GetRowCellValue(e.Node, tb_FatherNum));
				IncomingPaymentFather(_dtPayment, fatherId2);
			}
			if ((!(e.Column.FieldName == "TaskCode") || Function.ParseInt(tv_Payments.GetRowCellValue(e.Node, tb_BaseLine)) <= 0) && !_isLoading)
			{
				DataChanged();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void tv_Payments_FocusedNodeChanged(object sender, FocusedNodeChangedEventArgs e)
	{
		try
		{
			string value = Function.ToString(tv_Payments.GetRowCellValue(e.Node, tp_TaskCode));
			int num = Function.ParseInt(tv_Payments.GetRowCellValue(e.Node, tp_BaseLine));
			if (!string.IsNullOrEmpty(value) && num > 0)
			{
				toolStripMenuItem4.Enabled = false;
				toolStripMenuItem5.Enabled = false;
			}
			else
			{
				toolStripMenuItem4.Enabled = true;
				toolStripMenuItem5.Enabled = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void tv_Payments_EditorKeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.Return)
			{
				int fatherId = Function.ParseInt(tv_Payments.FocusedNode.GetValue("FatherNum"));
				string line = Function.ToString(tv_Payments.FocusedNode.GetValue("LineNum"));
				NewPaymentTree(fatherId, line);
				tv_Payments.FocusedNode.Expand();
			}
			if (e.KeyCode == Keys.Insert)
			{
				int num = Function.ParseInt(tv_Payments.FocusedNode.GetValue("LineNum"));
				NewPaymentTree(num, Function.ToString(num));
				tv_Payments.FocusedNode.Expand();
			}
			if (e.Modifiers == Keys.Shift && e.KeyCode == Keys.Delete)
			{
				Function.ParseInt(tv_budget.FocusedNode.GetValue("LineNum"));
				tv_Payments.DeleteSelectedNodes();
				tv_Payments.DataSourceTable().AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void tv_Payments_NodeCellStyle(object sender, GetCustomNodeCellStyleEventArgs e)
	{
		try
		{
			string value = Function.ToString(tv_Payments.GetRowCellValue(e.Node, tp_TaskCode));
			bool flag = Function.ParseBool(tv_Payments.GetRowCellValue(e.Node, tp_IsNew));
			string a = Function.ToString(gv_Doc.GetFocusedRowCellValue("PrgStatus"));
			if (string.Equals(a, "C") || string.Equals(a, "L"))
			{
				e.Appearance.BackColor = GridColor.GridReadOnly;
			}
			if (!flag && e.Column.FieldName == "TaskCode")
			{
				e.Appearance.BackColor = GridColor.GridReadOnly;
			}
			if (!flag && e.Column.FieldName == "TaskName")
			{
				e.Appearance.BackColor = GridColor.GridReadOnly;
			}
			if (!string.IsNullOrEmpty(value) && (e.Column.FieldName == "TaskCode" || e.Column.FieldName == "TaskName" || e.Column.FieldName == "InvoicePlan" || e.Column.FieldName == "IncomingPayment" || e.Column.FieldName == "Payments" || e.Column.FieldName == "Duedate" || e.Column.FieldName == "FatherNum") && !flag)
			{
				e.Appearance.BackColor = GridColor.GridReadOnly;
			}
			string a2 = Function.ToString(gv_Doc.GetFocusedRowCellValue("BaseLine"));
			if (!string.Equals(Function.ToString(gv_Doc.GetFocusedRowCellValue("WddStatus")), "-") || string.Equals(a2, "Y"))
			{
				e.Appearance.BackColor = GridColor.GridReadOnly;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void tv_Payments_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (Function.ToString(gv_Doc.GetFocusedRowCellValue("OwnerAuth")) != "F")
			{
				e.Cancel = true;
				return;
			}
			string value = Function.ToString(tv_Payments.GetFocusedRowCellValue(tp_TaskCode));
			int num = Function.ParseInt(tv_Payments.GetFocusedRowCellValue(tp_BaseLine));
			bool flag = Function.ParseBool(tv_Payments.GetFocusedRowCellValue(tp_IsNew));
			if ((tv_Payments.FocusedColumn.FieldName == "InvoicePlan" || tv_Payments.FocusedColumn.FieldName == "IncomingPayment" || tv_Payments.FocusedColumn.FieldName == "Payments" || tv_Payments.FocusedColumn.FieldName == "Duedate" || tv_Payments.FocusedColumn.FieldName == "FatherNum") && !string.IsNullOrEmpty(value) && !flag)
			{
				tv_Payments.FocusedColumn.OptionsColumn.ReadOnly = true;
				tv_Payments.Columns["Remarks"].OptionsColumn.ReadOnly = false;
			}
			else
			{
				tv_Payments.FocusedColumn.OptionsColumn.ReadOnly = false;
				tv_Payments.Columns["LineNum"].OptionsColumn.ReadOnly = true;
				tv_Payments.Columns["AbsEntry"].OptionsColumn.ReadOnly = true;
			}
			if (tv_Payments.FocusedColumn.FieldName == "TaskCode" && num == 0)
			{
				tv_Payments.Columns["TaskCode"].OptionsColumn.ReadOnly = false;
			}
			if (tv_Payments.FocusedColumn.FieldName == "TaskCode" && num > 0)
			{
				tv_Payments.Columns["TaskCode"].OptionsColumn.ReadOnly = true;
			}
			if (tv_Payments.FocusedColumn.FieldName == "TaskName" && num == 0)
			{
				tv_Payments.Columns["TaskName"].OptionsColumn.ReadOnly = false;
			}
			if (tv_Payments.FocusedColumn.FieldName == "TaskName" && num > 0)
			{
				tv_Payments.Columns["TaskName"].OptionsColumn.ReadOnly = true;
			}
			string a = Function.ToString(gv_Doc.GetFocusedRowCellValue("PrgStatus"));
			if (string.Equals(a, "C") || string.Equals(a, "L"))
			{
				e.Cancel = true;
			}
			string a2 = Function.ToString(gv_Doc.GetFocusedRowCellValue("BaseLine"));
			if (!string.Equals(Function.ToString(gv_Doc.GetFocusedRowCellValue("WddStatus")), "-") || string.Equals(a2, "Y"))
			{
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void xtra_Line_SelectedPageChanged(object sender, TabPageChangedEventArgs e)
	{
		try
		{
			if (_isChangeItem && e.Page == xtra_Budget)
			{
				_isLoading = true;
				_isCpyWbs = true;
				SetDataByLevelItem();
				_isCpyWbs = false;
				_isLoading = false;
			}
			btn_GanttView.Visible = xtra_Line.SelectedTabPage == tab_TaskList;
			btn_ShowGantt.Visible = xtra_Line.SelectedTabPage == tab_Task;
			SimpleButtonEx simpleButtonEx = btn_Select;
			bool visible = (drp_CrDocumnet.Visible = xtra_Line.SelectedTabPage == xtra_Budget);
			simpleButtonEx.Visible = visible;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		_isChangeItem = false;
	}

	private void xtra_LineBudget_SelectedPageChanged(object sender, TabPageChangedEventArgs e)
	{
		try
		{
			if (_isChangeItem && e.Page == xtra_Payment)
			{
				_isLoading = true;
				_isCpyWbs = true;
				SetDataByLevelPayments();
				_isCpyWbs = false;
				_isLoading = false;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		_isChangeItem = false;
	}

	private void btn_ARInvoice_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		CallChooseFromList(13);
	}

	private void btn_PR_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		CallChooseFromList(1470000113);
	}

	private void btn_PO_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		CallChooseFromList(22);
	}

	private void btn_GRPO_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		CallChooseFromList(20);
	}

	private void btn_Delivery_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		CallChooseFromList(15);
	}

	private void btn_APInvoice_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		CallChooseFromList(18);
	}

	private void btn_PQ_ButtonClick_1(object sender, ButtonPressedEventArgs e)
	{
		CallChooseFromList(540000006);
	}

	private void Btn_IGE_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		CallChooseFromList(60);
	}

	private void btn_WOR_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		CallChooseFromList(202);
	}

	private void CallChooseFromList(int type)
	{
		try
		{
			int absEntry = Function.ParseInt(tv_budget.GetFocusedRowCellValue(tb_AbsEntry));
			int lineNum = Function.ParseInt(tv_budget.GetFocusedRowCellValue(tb_LineNum));
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.DocumentByWbs, multiSelect: false, "");
			frm_ChooseFromList2._objectType = type;
			frm_ChooseFromList2._absEntry = absEntry;
			frm_ChooseFromList2._lineNum = lineNum;
			frm_ChooseFromList2.ShowDialog(this);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void canceledToolStripMenuItem_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			int docEntry = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("AbsEntry"));
			string value = "C";
			if (docEntry <= 0 || string.IsNullOrEmpty(value) || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCancel, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) != DialogResult.Yes)
			{
				return;
			}
			DataRow[] array = (from t in grd_Doc.DataSourceTable.AsEnumerable()
				where Function.ParseInt(t["AbsEntry"]) == docEntry
				select t).ToArray();
			if (array != null && array.Length != 0)
			{
				array[0]["PrgStatus"] = "C";
				DataTable dtCopy = array.CopyToDataTable();
				DataSet ds = await GetTableSave(dtCopy);
				if (string.IsNullOrEmpty((await _httpClient.ProjectManager.Update(ds)).Message))
				{
					btn_Search_Click(null, null);
				}
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void closedToolStripMenuItem_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			int docEntry = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("AbsEntry"));
			string value = "L";
			if (docEntry <= 0 || string.IsNullOrEmpty(value) || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmClose, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) != DialogResult.Yes)
			{
				return;
			}
			DataRow[] array = (from t in grd_Doc.DataSourceTable.AsEnumerable()
				where Function.ParseInt(t["AbsEntry"]) == docEntry
				select t).ToArray();
			if (array != null && array.Length != 0)
			{
				array[0]["PrgStatus"] = "L";
				DataTable dtCopy = array.CopyToDataTable();
				DataSet ds = await GetTableSave(dtCopy);
				if (string.IsNullOrEmpty((await _httpClient.ProjectManager.Update(ds)).Message))
				{
					btn_Search_Click(null, null);
				}
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_LineDocument_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			string a = Function.ToString(gv_Doc.GetFocusedRowCellValue("PrgStatus"));
			if (string.Equals(a, "C") || string.Equals(a, "L"))
			{
				e.Appearance.BackColor = GridColor.GridReadOnly;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_LineSub_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			string a = Function.ToString(gv_Doc.GetFocusedRowCellValue("PrgStatus"));
			if (string.Equals(a, "C") || string.Equals(a, "L"))
			{
				e.Appearance.BackColor = GridColor.GridReadOnly;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_lineIssue_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			string a = Function.ToString(gv_Doc.GetFocusedRowCellValue("PrgStatus"));
			if (string.Equals(a, "C") || string.Equals(a, "L"))
			{
				e.Appearance.BackColor = GridColor.GridReadOnly;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_LineTeam_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (Function.ToString(gv_Doc.GetFocusedRowCellValue("OwnerAuth")) != "F")
			{
				e.Cancel = true;
				return;
			}
			string a = Function.ToString(gv_Doc.GetFocusedRowCellValue("PrgStatus"));
			if (string.Equals(a, "C") || string.Equals(a, "L"))
			{
				gv_LineTeam.FocusedColumn.OptionsColumn.ReadOnly = true;
			}
			else
			{
				gv_LineTeam.Columns["Role"].OptionsColumn.ReadOnly = true;
			}
			string a2 = Function.ToString(gv_Doc.GetFocusedRowCellValue("BaseLine"));
			if (!string.Equals(Function.ToString(gv_Doc.GetFocusedRowCellValue("WddStatus")), "-") || string.Equals(a2, "Y"))
			{
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task MOVE_FIRST()
	{
		WinCommons._OpenProcessing();
		base.FormMode = Enums.FormViewModeType.Busy;
		try
		{
			if (await FindNextLastFrist(Enums.NavigateType.First) == 0)
			{
				base.FormMode = Enums.FormViewModeType.View;
				WinCommons._CloseProcessing();
				return;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_NEXT()
	{
		WinCommons._OpenProcessing();
		base.FormMode = Enums.FormViewModeType.Busy;
		try
		{
			if (await FindNextLastFrist(Enums.NavigateType.Next) == 0)
			{
				base.FormMode = Enums.FormViewModeType.View;
				WinCommons._CloseProcessing();
				return;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_LAST()
	{
		WinCommons._OpenProcessing();
		base.FormMode = Enums.FormViewModeType.Busy;
		try
		{
			if (await FindNextLastFrist(Enums.NavigateType.Last) == 0)
			{
				base.FormMode = Enums.FormViewModeType.View;
				WinCommons._CloseProcessing();
				return;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_PREVIOUS()
	{
		WinCommons._OpenProcessing();
		base.FormMode = Enums.FormViewModeType.Busy;
		try
		{
			if (await FindNextLastFrist(Enums.NavigateType.Previous) == 0)
			{
				base.FormMode = Enums.FormViewModeType.View;
				WinCommons._CloseProcessing();
				return;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	public async Task<int> FindNextLastFrist(Enums.NavigateType type)
	{
		_ = 1;
		try
		{
			DataTable dataTable = await _httpClient.ProjectManager.FindNextLastFrist(Function.ParseInt(txt_No.EditValue), (int)type);
			if (dataTable.IsNotNull())
			{
				grd_Doc.DataSource = dataTable;
				grd_Doc.DatasourceTable().AcceptChanges();
				int num = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("AbsEntry"));
				txt_No.EditValue = num;
				await LoadLineById(num);
				base.FormMode = Enums.FormViewModeType.View;
				return 1;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		return 0;
	}

	private async Task LoadForm()
	{
		_ = 1;
		try
		{
			btn_BpCode.ValueMember = "CardCode";
			btn_BpCode.FieldNameInDataRow("CardCode", "CardName");
			btn_BpCode.FieldNameInGrid("CardCode", "CardName");
			btn_BpCode.TypeCFL = Enums.TypeCFL.CustomerLead;
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL = btn_BpCode;
			repositoryItemButtonEditCFL.DataSourceLoaded = (Action<DataTable>)Delegate.Combine(repositoryItemButtonEditCFL.DataSourceLoaded, (Action<DataTable>)((DataTable dtData) =>
			{
				btn_Bp.Properties.DataSource = dtData.Copy();
				btn_IssueBp.DataSource = dtData.Copy();
				btn_SubBpCode.DataSource = dtData.Copy();
			}));
			btn_BpCode.LoadDataSourceWhenEnter = true;
			btn_BpCode.RefreshDataSource();
			btn_BpCode.TypeDrillDown = ApzObjectTypes.oBusinessPartners;
			btn_Bp.Properties.ValueMember = "CardCode";
			btn_Bp.Properties.TypeCFL = Enums.TypeCFL.Customer;
			btn_Bp.Properties.LoadDataSourceWhenEnter = true;
			btn_Bp.Properties.RefreshDataSource();
			btn_Bp.Properties.TypeDrillDown = ApzObjectTypes.oBusinessPartners;
			btn_SubBpCode.ValueMember = "CardCode";
			btn_SubBpCode.TypeCFL = Enums.TypeCFL.Customer;
			btn_SubBpCode.LoadDataSourceWhenEnter = true;
			btn_SubBpCode.RefreshDataSource();
			btn_SubBpCode.TypeDrillDown = ApzObjectTypes.oBusinessPartners;
			btn_IssueBp.ValueMember = "CardCode";
			btn_IssueBp.TypeCFL = Enums.TypeCFL.Customer;
			btn_IssueBp.LoadDataSourceWhenEnter = true;
			btn_IssueBp.RefreshDataSource();
			btn_IssueBp.TypeDrillDown = ApzObjectTypes.oBusinessPartners;
			btn_Manager.ValueMember = "EmpId";
			btn_Manager.DisplayMember = "lastName";
			btn_Manager.DisplayMember2 = "firstName";
			btn_Manager.SeparateString = " ";
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL2 = btn_Manager;
			repositoryItemButtonEditCFL2.DataSourceLoaded = (Action<DataTable>)Delegate.Combine(repositoryItemButtonEditCFL2.DataSourceLoaded, (Action<DataTable>)((DataTable dtData) =>
			{
				btn_Owner.DataSource = dtData.Copy();
			}));
			btn_Manager.TypeCFL = Enums.TypeCFL.EmployeeListForSalesEmpSetup;
			btn_Manager.RefreshDataSource();
			btn_Manager.TypeDrillDown = ApzObjectTypes.oEmployeesInfo;
			btn_SubPM.ValueMember = "EmpId";
			btn_SubPM.DisplayMember = "lastName";
			btn_SubPM.DisplayMember2 = "firstName";
			btn_SubPM.SeparateString = " ";
			btn_SubPM.TypeCFL = Enums.TypeCFL.EmployeeListForSalesEmpSetup;
			btn_SubPM.RefreshDataSource();
			btn_SubPM.TypeDrillDown = ApzObjectTypes.oEmployeesInfo;
			btn_Project.ValueMember = "AbsEntry";
			btn_Project.DisplayMember = "Code";
			btn_Project.TypeCFL = Enums.TypeCFL.ProjectManagerAll;
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL3 = btn_Project;
			repositoryItemButtonEditCFL3.DataSourceLoaded = (Action<DataTable>)Delegate.Combine(repositoryItemButtonEditCFL3.DataSourceLoaded, (Action<DataTable>)((DataTable dtData) =>
			{
				btn_IssuePr.DataSource = dtData.Copy();
				btn_SubMasterProject.DataSource = dtData.Copy();
			}));
			btn_Project.RefreshDataSource();
			btn_Project.TypeDrillDown = ApzObjectTypes.oProjectManager;
			btn_SubMasterProject.ValueMember = "AbsEntry";
			btn_SubMasterProject.DisplayMember = "Code";
			btn_SubMasterProject.TypeCFL = Enums.TypeCFL.ProjectManagerAll;
			btn_SubMasterProject.RefreshDataSource();
			btn_SubMasterProject.TypeDrillDown = ApzObjectTypes.oProjectManager;
			btn_IssuePr.ValueMember = "AbsEntry";
			btn_IssuePr.DisplayMember = "Code";
			btn_IssuePr.TypeCFL = Enums.TypeCFL.ProjectManagerAll;
			btn_IssuePr.RefreshDataSource();
			btn_FinProject.ValueMember = "PrjCode";
			btn_FinProject.TypeCFL = Enums.TypeCFL.Project;
			btn_FinProject.RefreshDataSource();
			btn_FinProject.TypeDrillDown = ApzObjectTypes.oProjectCodes;
			btn_SubFinalProject.ValueMember = "PrjCode";
			btn_SubFinalProject.TypeCFL = Enums.TypeCFL.Project;
			btn_SubFinalProject.RefreshDataSource();
			btn_SubFinalProject.TypeDrillDown = ApzObjectTypes.oProjectCodes;
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL4 = btn_SubOwner;
			string valueMember = (btn_Owner.ValueMember = "EmpId");
			repositoryItemButtonEditCFL4.ValueMember = valueMember;
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL5 = btn_SubOwner;
			valueMember = (btn_Owner.DisplayMember = "lastName");
			repositoryItemButtonEditCFL5.DisplayMember = valueMember;
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL6 = btn_SubOwner;
			valueMember = (btn_Owner.DisplayMember2 = "firstName");
			repositoryItemButtonEditCFL6.DisplayMember2 = valueMember;
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL7 = btn_SubOwner;
			valueMember = (btn_Owner.SeparateString = " ");
			repositoryItemButtonEditCFL7.SeparateString = valueMember;
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL8 = btn_SubOwner;
			Enums.TypeCFL typeCFL = (btn_Owner.TypeCFL = Enums.TypeCFL.EmployeeListForSalesEmpSetup);
			repositoryItemButtonEditCFL8.TypeCFL = typeCFL;
			btn_SubOwner.RefreshDataSource();
			btn_SubOwner.TypeDrillDown = ApzObjectTypes.oEmployeesInfo;
			btn_Owner.RefreshDataSource();
			btn_Owner.TypeDrillDown = ApzObjectTypes.oEmployeesInfo;
			btn_OwnerSearch.Properties.ValueMember = "EmpId";
			btn_OwnerSearch.Properties.DisplayMember = "lastName";
			btn_OwnerSearch.Properties.DisplayMember2 = "firstName";
			btn_OwnerSearch.Properties.SeparateString = " ";
			btn_OwnerSearch.Properties.TypeCFL = Enums.TypeCFL.EmployeeListForSalesEmpSetup;
			btn_OwnerSearch.Properties.RefreshDataSource();
			btn_OwnerSearch.Properties.TypeDrillDown = ApzObjectTypes.oEmployeesInfo;
			btn_SaleOder.ValueMember = "DocNum";
			btn_SaleOder.TypeCFL = Enums.TypeCFL.SalesOder;
			btn_SaleOder.RefreshDataSource();
			btn_SubSalesOrder.ValueMember = "DocNum";
			btn_SubSalesOrder.TypeCFL = Enums.TypeCFL.SalesOder;
			btn_SubSalesOrder.RefreshDataSource();
			btn_ItemCode.ValueMember = "ItemCode";
			btn_ItemCode.FieldNameInDataRow("ItemCode", "ItemName");
			btn_ItemCode.ListMandatoryField = "ItemName,InvntryUom,ItmsGrpCod,Price,dfltWH";
			btn_ItemCode.FieldNameInGrid("ItemCode", "Dscription");
			btn_ItemCode.FindCFLBy(Enums.TypeCFL.ListItems, true, "", Enums.ItemObjectType.NoItemType, null);
			btn_ItemCode.TypeDrillDown = ApzObjectTypes.oItems;
			btn_ItemCode.HandleChooseItemMulti = ChooseItemReceiptMulti;
			btn_ItemCode.RefreshDataSource();
			btn_Code.ValueMember = "AbsEntry";
			btn_Code.TypeCFL = Enums.TypeCFL.ProjectManagerAll;
			btn_Code.RefreshDataSource();
			btn_Code.TypeDrillDown = ApzObjectTypes.oProjectManager;
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL9 = btn_Code;
			repositoryItemButtonEditCFL9.HandleChooseItem = (Action<DataRow>)Delegate.Combine(repositoryItemButtonEditCFL9.HandleChooseItem, new Action<DataRow>(ProjCode_HandleChooseItem));
			btn_resource.ValueMember = "empId";
			btn_resource.DisplayMember = "empId";
			btn_resource.AutoLoadDataSource = false;
			btn_resource.AcceptFalseInput = true;
			btn_resource.FindCFLBy(Enums.TypeCFL.EmployeeListForSalesEmpSetup, true, "", Enums.ItemObjectType.NoItemType, null);
			btn_report.ValueMember = "EmpId";
			btn_report.DisplayMember = "lastName";
			btn_report.DisplayMember2 = "firstName";
			btn_report.TypeCFL = Enums.TypeCFL.EmployeeListForSalesEmpSetup;
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL10 = btn_report;
			repositoryItemButtonEditCFL10.DataSourceLoaded = (Action<DataTable>)Delegate.Combine(repositoryItemButtonEditCFL10.DataSourceLoaded, (Action<DataTable>)((DataTable dt) =>
			{
				btn_handle.DataSource = dt.Copy();
				btn_IssuwPrOwner.DataSource = dt.Copy();
				btn_ResourceEmployee.DataSource = dt.Copy();
			}));
			btn_report.RefreshDataSource();
			btn_report.TypeDrillDown = ApzObjectTypes.oEmployeesInfo;
			btn_handle.ValueMember = "EmpId";
			btn_handle.DisplayMember = "lastName";
			btn_handle.DisplayMember2 = "firstName";
			btn_handle.TypeCFL = Enums.TypeCFL.EmployeeListForSalesEmpSetup;
			btn_handle.RefreshDataSource();
			btn_handle.TypeDrillDown = ApzObjectTypes.oEmployeesInfo;
			btn_IssuwPrOwner.ValueMember = "EmpId";
			btn_IssuwPrOwner.DisplayMember = "lastName";
			btn_IssuwPrOwner.DisplayMember2 = "firstName";
			btn_IssuwPrOwner.TypeCFL = Enums.TypeCFL.EmployeeListForSalesEmpSetup;
			btn_IssuwPrOwner.RefreshDataSource();
			btn_IssuwPrOwner.TypeDrillDown = ApzObjectTypes.oEmployeesInfo;
			btn_ResourceEmployee.ValueMember = "EmpId";
			btn_ResourceEmployee.TypeCFL = Enums.TypeCFL.EmployeeListForSalesEmpSetup;
			btn_ResourceEmployee.RefreshDataSource();
			btn_ResourceEmployee.TypeDrillDown = ApzObjectTypes.oEmployeesInfo;
			btn_Id.ValueMember = "Id";
			btn_Id.DisplayMember = "";
			btn_Id.TypeCFL = Enums.TypeCFL.TrackingIssue;
			btn_Id.RefreshDataSource();
			btn_Id.TypeDrillDown = ApzObjectTypes.oTrackingIssue;
			btn_solution.ValueMember = "SltCode";
			btn_solution.DisplayMember = "Subject";
			btn_solution.TypeCFL = Enums.TypeCFL.KnowledgeBaseSolutions;
			btn_solution.RefreshDataSource();
			btn_solution.TypeDrillDown = ApzObjectTypes.oKnowledgeBaseSolutions;
			btn_TaskCode.ValueMember = "TaskCode";
			btn_TaskCode.TypeCFL = Enums.TypeCFL.ProjectTask;
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL11 = btn_TaskCode;
			repositoryItemButtonEditCFL11.HandleChooseItem = (Action<DataRow>)Delegate.Combine(repositoryItemButtonEditCFL11.HandleChooseItem, new Action<DataRow>(TaskCode_HandleChooseItem));
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL12 = btn_TaskCode;
			repositoryItemButtonEditCFL12.DataSourceLoaded = (Action<DataTable>)Delegate.Combine(repositoryItemButtonEditCFL12.DataSourceLoaded, (Action<DataTable>)((DataTable dtData) =>
			{
				btn_WbsPayment.DataSource = dtData.Copy();
				btn_IssueTaskCode.DataSource = dtData.Copy();
			}));
			btn_TaskCode.RefreshDataSource();
			btn_IssueTaskCode.ValueMember = "TaskCode";
			btn_IssueTaskCode.FieldNameInDataRow("TaskCode", "TaskName");
			btn_IssueTaskCode.FieldNameInGrid("TaskCode", "TaskName");
			btn_IssueTaskCode.TypeCFL = Enums.TypeCFL.ProjectTask;
			btn_IssueTaskCode.RefreshDataSource();
			btn_WbsPayment.ValueMember = "TaskCode";
			btn_WbsPayment.TypeCFL = Enums.TypeCFL.ProjectTask;
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL13 = btn_WbsPayment;
			repositoryItemButtonEditCFL13.HandleChooseItem = (Action<DataRow>)Delegate.Combine(repositoryItemButtonEditCFL13.HandleChooseItem, new Action<DataRow>(WbsPayment_HandleChooseItem));
			btn_WbsPayment.RefreshDataSource();
			btn_IssueSLBy.ValueMember = "USERID";
			btn_IssueSLBy.DisplayMember = "U_NAME";
			btn_IssueSLBy.TypeCFL = Enums.TypeCFL.GetUserList;
			btn_IssueSLBy.TypeDrillDown = ApzObjectTypes.oNone;
			btn_IssueSLBy.RefreshDataSource();
			btn_OrderItem.ValueMember = "LineNum";
			btn_OrderItem.FindCFLBy(Enums.TypeCFL.ItemForSaleOrder, true, "", Enums.ItemObjectType.NoItemType, null);
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL14 = btn_OrderItem;
			repositoryItemButtonEditCFL14.BeforeShowCFL = (Action<frm_CFL>)Delegate.Combine(repositoryItemButtonEditCFL14.BeforeShowCFL, (Action<frm_CFL>)((frm_CFL frm) =>
			{
				frm.orderId = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("Order"));
			}));
			btn_OrderItem.RefreshDataSource();
			btn_OrderItem.TypeDrillDown = ApzObjectTypes.oOrders;
			btn_OrderItem.HandleChooseItemMulti = ChooseOrderItemReceiptMulti;
			btn_OrderItem.TextEditStyle = TextEditStyles.DisableTextEditor;
			btn_Whs.ValueMember = "WhsCode";
			btn_Whs.TypeCFL = Enums.TypeCFL.Warehouse;
			btn_Whs.RefreshDataSource();
			btn_Whs.TypeDrillDown = ApzObjectTypes.oWarehouses;
			btn_SubWhs.ValueMember = "WhsCode";
			btn_SubWhs.TypeCFL = Enums.TypeCFL.Warehouse;
			btn_SubWhs.RefreshDataSource();
			btn_SubWhs.TypeDrillDown = ApzObjectTypes.oWarehouses;
			btn_WarehouseItem.ValueMember = "WhsCode";
			btn_WarehouseItem.TypeCFL = Enums.TypeCFL.Warehouse;
			btn_WarehouseItem.RefreshDataSource();
			btn_WarehouseItem.TypeDrillDown = ApzObjectTypes.oWarehouses;
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL15 = btn_WarehouseItem;
			repositoryItemButtonEditCFL15.HandleChooseItem = (Action<DataRow>)Delegate.Combine(repositoryItemButtonEditCFL15.HandleChooseItem, new Action<DataRow>(WhsItem_HandleChooseItem));
			btn_BOM.ValueMember = "Code";
			btn_BOM.FindCFLBy(Enums.TypeCFL.BOMSaleOrder, true, "", Enums.ItemObjectType.NoItemType, null);
			btn_BOM.RefreshDataSource();
			btn_BOM.TypeDrillDown = ApzObjectTypes.oItems;
			btn_BOM.HandleChooseItemMulti = ChooseBomReceiptMulti;
			btn_DocNumDo.ValueMember = "DocEntry";
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL16 = btn_DocNumDo;
			repositoryItemButtonEditCFL16.DrillDownMethod = (Action<string>)Delegate.Combine(repositoryItemButtonEditCFL16.DrillDownMethod, (Action<string>)((string editorValue) =>
			{
				string text5 = Function.ToString(gv_LineDocument.GetFocusedRowCellValue("ObjType"));
				ApzObjectTypes type = ApzObjectTypes.oNone;
				switch (text5)
				{
				case "1470000113":
					type = ApzObjectTypes.oPurchaseRequest;
					break;
				case "20":
					type = ApzObjectTypes.oPurchaseDeliveryNotes;
					break;
				case "22":
					type = ApzObjectTypes.oPurchaseOrders;
					break;
				case "15":
					type = ApzObjectTypes.oDeliveryNotes;
					break;
				case "18":
					type = ApzObjectTypes.oPurchaseInvoices;
					break;
				case "13":
					type = ApzObjectTypes.oInvoices;
					break;
				case "540000006":
					type = ApzObjectTypes.oPurchaseQuotations;
					break;
				case "203":
					type = ApzObjectTypes.oDownPayments;
					break;
				case "204":
					type = ApzObjectTypes.oPurchaseDownPayments;
					break;
				case "60":
					type = ApzObjectTypes.oInventoryGenExit;
					break;
				case "202":
					type = ApzObjectTypes.oProductionOrders;
					break;
				}
				this.DrillDown(type, editorValue);
			}));
			btn_DocNumDo.RefreshDataSource();
			btn_WBSTemplate.ValueMember = "Code";
			btn_WBSTemplate.TypeCFL = Enums.TypeCFL.WbsTemplate;
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL17 = btn_WBSTemplate;
			repositoryItemButtonEditCFL17.HandleChooseItem = (Action<DataRow>)Delegate.Combine(repositoryItemButtonEditCFL17.HandleChooseItem, new Action<DataRow>(WBSTemplate_HandleChooseItem));
			btn_WBSTemplate.RefreshDataSource();
			btn_WBSTemplate.TypeDrillDown = ApzObjectTypes.oWBSTemplate;
			LoadComboIndustry();
			await WinFuntions.CreateColumnsDistRule(gv_Doc);
			CreateTreeDistRule(tv_Wbs);
			CreateTreeDistRule(tv_budget);
			CreateTreeDistRule(tv_Payments);
			await InitCalendar();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public void SetDataCurrency()
	{
		try
		{
			CurrData = GlobalData.CurrencyCodes;
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("CurrCode", typeof(string));
			dataTable.Columns.Add("CurrName", typeof(string));
			foreach (CurrencyCodesImpl currDatum in CurrData)
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["CurrCode"] = currDatum.CurrCode;
				dataRow["CurrName"] = currDatum.CurrName;
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
			}
			luk_DocCurr.DataSource = dataTable.Copy();
			luk_DocCurr.NullText = "";
			luk_DocCurr.ValueMember = "CurrCode";
			luk_DocCurr.DisplayMember = "CurrCode";
			luk_Currency.DataSource = dataTable.Copy();
			luk_Currency.NullText = "";
			luk_Currency.ValueMember = "CurrCode";
			luk_Currency.DisplayMember = "CurrCode";
			luk_CurrencyLineDu.DataSource = dataTable.Copy();
			luk_CurrencyLineDu.NullText = "";
			luk_CurrencyLineDu.ValueMember = "CurrCode";
			luk_CurrencyLineDu.DisplayMember = "CurrCode";
			luk_SubCurrency.DataSource = dataTable.Copy();
			luk_SubCurrency.NullText = "";
			luk_SubCurrency.ValueMember = "CurrCode";
			luk_SubCurrency.DisplayMember = "CurrCode";
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void WBSTemplate_HandleChooseItem(DataRow dr)
	{
		WinCommons._OpenProcessing();
		try
		{
			TreeListNode focusedNode = tv_Wbs.FocusedNode;
			if (dr != null)
			{
				tv_Wbs.SetRowCellValue(focusedNode, tl_WbsTemplate, Function.ToString(dr["Code"]));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private void TaskCode_HandleChooseItem(DataRow dr)
	{
		try
		{
			if (!tv_budget.Columns["TaskCode"].OptionsColumn.ReadOnly)
			{
				TreeListNode focusedNode = tv_budget.FocusedNode;
				if (dr != null)
				{
					tv_budget.SetRowCellValue(focusedNode, tb_TaskCode, Function.ToString(dr["TaskCode"]));
					tv_budget.SetRowCellValue(focusedNode, tb_TaskName, Function.ToString(dr["TaskName"]));
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void WbsPayment_HandleChooseItem(DataRow dr)
	{
		try
		{
			if (!tv_Payments.Columns["TaskCode"].OptionsColumn.ReadOnly)
			{
				TreeListNode focusedNode = tv_Payments.FocusedNode;
				if (dr != null)
				{
					tv_Payments.SetRowCellValue(focusedNode, tp_TaskCode, Function.ToString(dr["TaskCode"]));
					tv_Payments.SetRowCellValue(focusedNode, tp_TaskName, Function.ToString(dr["TaskName"]));
				}
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void ChooseOrderItemReceiptMulti(List<DataRow> rows)
	{
		try
		{
			WinCommons._OpenProcessing();
			_isOrder = true;
			_dtItem = tv_budget.DataSourceTable();
			_dtDoc = grd_Doc.DatasourceTable();
			int fatherId = Function.ParseInt(tv_budget.GetFocusedRowCellValue(tb_FatherNum));
			int num = Function.ParseInt(tv_budget.FocusedNode.GetValue("LineNum"));
			Function.ToString(gv_Doc.GetFocusedRowCellValue("DefaultWhs"));
			TreeListNode focusedNode = tv_budget.FocusedNode;
			if (_dtItem != null && _dtItem.Rows.Count > 0 && rows.Count > 0)
			{
				string value = Function.ToString(rows[0]["ItemCode"]);
				if (!string.IsNullOrEmpty(value))
				{
					tv_budget.SetRowCellValue(focusedNode, tb_ItemCode, value);
					tv_budget.SetRowCellValue(focusedNode, tb_plQty, Function.ParseDouble(rows[0]["Quantity"]));
					for (int i = 1; i < rows.Count; i++)
					{
						int num2 = NewItemTree(fatherId, Function.ToString(num));
						TreeListNode treeListNode = tv_budget.FindNodeByFieldValue("LineNum", num2);
						if (treeListNode != null)
						{
							string value2 = Function.ToString(rows[i]["ItemCode"]);
							if (!string.IsNullOrEmpty(value2))
							{
								tv_budget.SetRowCellValue(treeListNode, "ItemCode", value2);
								tv_budget.SetRowCellValue(treeListNode, tb_plQty, Function.ParseDouble(rows[i]["Quantity"]));
							}
							NewItemTree(fatherId, Function.ToString(num));
						}
					}
					_dtItem.AcceptChanges();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		_isOrder = false;
		WinCommons._CloseProcessing();
	}

	private async void ChooseBomReceiptMulti(List<DataRow> rows)
	{
		try
		{
			WinCommons._OpenProcessing();
			isChooseBom = true;
			_dtItem = tv_budget.DataSourceTable();
			_dtDoc = grd_Doc.DatasourceTable();
			int fatherId = Function.ParseInt(tv_budget.GetFocusedRowCellValue(tb_FatherNum));
			int num = Function.ParseInt(tv_budget.FocusedNode.GetValue("LineNum"));
			Function.ToString(gv_Doc.GetFocusedRowCellValue("DefaultWhs"));
			TreeListNode focusedNode = tv_budget.FocusedNode;
			if (_dtItem != null && _dtItem.Rows.Count > 0 && rows.Count > 0)
			{
				string value = Function.ToString(rows[0]["Code"]);
				if (!string.IsNullOrEmpty(value))
				{
					tv_budget.SetRowCellValue(focusedNode, tb_ItemCode, value);
					for (int i = 1; i < rows.Count; i++)
					{
						int num2 = NewItemTree(fatherId, Function.ToString(num));
						TreeListNode treeListNode = tv_budget.FindNodeByFieldValue("LineNum", num2);
						if (treeListNode != null)
						{
							string value2 = Function.ToString(rows[i]["Code"]);
							if (!string.IsNullOrEmpty(value2))
							{
								tv_budget.SetRowCellValue(treeListNode, "ItemCode", value2);
							}
							NewItemTree(fatherId, Function.ToString(num));
						}
					}
					_dtItem.AcceptChanges();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		isChooseBom = false;
		WinCommons._CloseProcessing();
	}

	private async void ChooseItemReceiptMulti(List<DataRow> rows)
	{
		try
		{
			WinCommons._OpenProcessing();
			isChooseBom = true;
			_dtItem = tv_budget.DataSourceTable();
			_dtDoc = grd_Doc.DatasourceTable();
			int fatherId = Function.ParseInt(tv_budget.GetFocusedRowCellValue(tb_FatherNum));
			int num = Function.ParseInt(tv_budget.FocusedNode.GetValue("LineNum"));
			Function.ToString(gv_Doc.GetFocusedRowCellValue("DefaultWhs"));
			TreeListNode focusedNode = tv_budget.FocusedNode;
			if (_dtItem != null && _dtItem.Rows.Count > 0 && rows.Count > 0)
			{
				string value = Function.ToString(rows[0]["ItemCode"]);
				tv_budget.SetRowCellValue(focusedNode, tb_ItemCode, value);
				for (int i = 1; i < rows.Count; i++)
				{
					int num2 = NewItemTree(fatherId, Function.ToString(num));
					TreeListNode treeListNode = tv_budget.FindNodeByFieldValue("LineNum", num2);
					if (treeListNode != null)
					{
						string value2 = Function.ToString(rows[i]["ItemCode"]);
						if (!string.IsNullOrEmpty(value2))
						{
							tv_budget.SetRowCellValue(treeListNode, "ItemCode", value2);
						}
						NewItemTree(fatherId, Function.ToString(num));
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		isChooseBom = false;
		WinCommons._CloseProcessing();
	}

	private void ProjCode_HandleChooseItem(DataRow dr)
	{
		try
		{
			gv_LineSub.GetFocusedDataRow().ItemArray = dr.ItemArray;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void WhsItem_HandleChooseItem(DataRow dr)
	{
		try
		{
			if (dr != null)
			{
				TreeListNode focusedNode = tv_budget.FocusedNode;
				tv_budget.SetRowCellValue(focusedNode, tb_DefaultWhs, Function.ToString(dr["WhsCode"]));
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private DataTable GetDataWorkSchedule(int year)
	{
		try
		{
			DataTable idWorkScheduling = _httpClient.ProjectManager.GetIdWorkScheduling(year);
			if (idWorkScheduling != null && idWorkScheduling.Rows.Count > 0)
			{
				int docId = Function.ParseInt(idWorkScheduling.Rows[0]["Id"]);
				using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "WorkScheduling");
				return httpClientHelper.Get("GetDataLine", new { docId });
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	private DataTable GetShifOrderByTime(string code)
	{
		try
		{
			return _httpClient.ProjectManager.GetShifOrderByTime(code);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	private async void AddWbsDetailTemplate(DataTable dt, DataRow row, int line, string code)
	{
		try
		{
			DataTable dtTask = tv_Wbs.DataSourceTable();
			DataRow[] source = (from t in dtTask.AsEnumerable()
				where Function.ParseInt(t["FatherNum"]) == line
				select t).ToArray();
			DataRow rowfocused = source.ToList().FirstOrDefault((DataRow t) => Function.ToString(t["TaskCode"]) == "");
			double value = Function.ParseDouble(row["Duration"]);
			if (rowfocused != null)
			{
				rowfocused["TaskCode"] = code + "." + Function.ToString(row["LineId"]);
				rowfocused["TaskName"] = Function.ToString(row["WBSName"]);
				rowfocused["WBSMode"] = Function.ToString(row["WBSMode"]);
				rowfocused["Resource"] = Function.ToString(row["Resource"]);
				rowfocused["Dependent"] = Function.ToString(row["Predecessors"]);
				rowfocused["Milestone"] = Function.ToString(row["Milestone"]);
				rowfocused["Notes"] = Function.ToString(row["Notes"]);
				rowfocused["OcrCode"] = Function.ToString(row["OcrCode"]);
				rowfocused["OcrCode2"] = Function.ToString(row["OcrCode2"]);
				rowfocused["OcrCode3"] = Function.ToString(row["OcrCode3"]);
				rowfocused["OcrCode4"] = Function.ToString(row["OcrCode4"]);
				rowfocused["OcrCode5"] = Function.ToString(row["OcrCode5"]);
				rowfocused["Duration"] = Function.ParseDecimal(row["Duration"]);
				string text = Function.ToString(rowfocused["Dependent"]);
				if (!string.IsNullOrEmpty(text))
				{
					string lineId = text.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToArray().Max();
					DataRow dataRow = (from t in dtTask.AsEnumerable()
						where Function.ParseInt(t["LineNum"]) == Function.ParseInt(lineId)
						select t).FirstOrDefault();
					if (dataRow != null)
					{
						rowfocused["FromDate"] = Function.ParseDateTimes(dataRow["ToDate"]).AddDays(value);
					}
				}
				else
				{
					rowfocused["FromDate"] = Function.ParseDateTimes(gv_Doc.GetFocusedRowCellValue("StartDate")).AddDays(value);
				}
				rowfocused["ToDate"] = Function.ParseDateTimes(rowfocused["FromDate"]).AddDays(value);
				DateTime value2 = Function.ParseDateTimes(rowfocused["FromDate"]);
				DateTime tr = Function.ParseDateTimes(rowfocused["ToDate"]);
				int num = Math.Abs(tr.Subtract(value2).Days);
				rowfocused["Duration"] = Function.ParseDecimal(num + 1);
				await SetDataTeamByResource(Function.ToString(row["Resource"]));
				SetFromDate(line, dtTask);
				SetToDate(line, dtTask, line, tr, Function.ToString(row["WBSMode"]));
			}
			List<DataRow> list = (from t in dt.AsEnumerable()
				where Function.ParseInt(t["FatherCode"]) == Function.ParseInt(row["LineId"])
				select t).ToList();
			if (list.Count > 0)
			{
				for (int num2 = 0; num2 < list.Count; num2++)
				{
					NewTask(Function.ParseInt(rowfocused["LineNum"]), Function.ToString(rowfocused["LineNum"]));
					AddWbsDetailTemplate(dt, list[num2], Function.ParseInt(rowfocused["LineNum"]), Function.ToString(list[num2]["Code"]));
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<DataTable> LoadData(int absEntry, string code, string status, string bpCode, string startFrom, string startTo)
	{
		try
		{
			_isLoading = true;
			DataTable dataTable = await _httpClient.ProjectManager.LoadData(absEntry, code.Trim(), status, bpCode.Trim(), startFrom, startTo, GlobalData.UserName);
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				int num = Function.ParseInt(dataTable.Rows[i]["MasterEntry"]);
				dataTable.Rows[i]["MasterEntry"] = ((num > 0) ? dataTable.Rows[i]["MasterEntry"] : DBNull.Value);
			}
			_isLoading = false;
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	private async Task LoadLineById(int id)
	{
		_ = 5;
		try
		{
			tv_budget.DataSourceTable().Rows.Clear();
			tv_Wbs.DataSourceTable().Rows.Clear();
			grd_LineDocument.DataSourceTable.Rows.Clear();
			grd_LineTeam.DataSourceTable.Rows.Clear();
			grd_lineissue.DatasourceTable().Rows.Clear();
			grd_LineSub.DatasourceTable().Rows.Clear();
			grd_NoteList.DataSourceTable().Rows.Clear();
			grd_EventList.DataSourceTable().Rows.Clear();
			_isDb = true;
			_isLoading = true;
			DataSet dataSet = await _httpClient.ProjectManager.GetDataById(id);
			if (dataSet.IsNotNull() && dataSet.Tables.Count > 0)
			{
				DataTable dataTable = dataSet.Tables[0];
				dataTable.Columns["LineNum"].AutoIncrement = true;
				dataTable.Columns["LineNum"].AutoIncrementSeed = (dataTable.IsNotNull() ? (dataTable.AsEnumerable().Max((DataRow t) => Function.ParseLong(t["LineNum"])) + 1) : 1);
				dataTable.Columns["LineNum"].AutoIncrementStep = 1L;
				dataTable.Columns["Status"].DefaultValue = "D";
				dataTable.Columns["Priority"].DefaultValue = "2";
				dataTable.Columns["Milestone"].DefaultValue = "N";
				dataTable.Columns["IsExist"].DefaultValue = "N";
				tv_Wbs.DataSource = dataTable;
				if (tv_Wbs.Columns["Wbs"] != null && tv_Wbs.Columns["Wbs"].SortOrder == SortOrder.None)
				{
					tv_Wbs.BeginSort();
					tv_Wbs.Columns["Wbs"].SortOrder = SortOrder.Ascending;
					tv_Wbs.EndSort();
				}
				btn_TaskCode.DataSource = dataTable.Copy();
				DataTable dataTable2 = dataSet.Tables[1];
				for (int num = 0; num < dataTable2.Rows.Count; num++)
				{
					int num2 = Function.ParseInt(dataTable2.Rows[num]["Resource"]);
					DataRow[] array = dataTable2.Select($"Resource={num2}").ToArray();
					if (array.Count() > 0)
					{
						for (int num3 = 1; num3 < array.Count(); num3++)
						{
							array[num3].Delete();
						}
						dataTable2.AcceptChanges();
					}
				}
				grd_LineTeam.DataSource = dataTable2;
				_dtItem = dataSet.Tables[2];
				_dtItem.Columns.Add("Wbs", typeof(string));
				_dtDoc = grd_Doc.DatasourceTable();
				if ((_dtItem == null || _dtItem.Rows.Count == 0) && Function.ParseInt(gv_Doc.GetFocusedRowCellValue("BudgetLevel")) > 0)
				{
					tv_budget.DataSource = _dtItem;
					SetDataByLevelItem();
				}
				for (int num4 = 0; num4 < _dtItem.Rows.Count; num4++)
				{
					int fatherId = Function.ParseInt(_dtItem.Rows[num4]["LineNum"]);
					string value = Function.ToString(_dtItem.Rows[num4]["TaskCode"]);
					double num5 = Function.ParseDouble(_dtItem.Rows[num4]["PlanQty"]);
					double num6 = Function.ParseDouble(_dtItem.Rows[num4]["ActualQty"]);
					_dtItem.Rows[num4]["QtyDiff"] = num6 - num5;
					_dtItem.Rows[num4]["Wbs"] = Function.ToString(_dtItem.Rows[num4]["VisOrder"]);
					_dtItem.Rows[num4]["ItemCode"] = Function.ToString(_dtItem.Rows[num4]["ItemCode"]).Trim();
					if (!string.IsNullOrEmpty(value))
					{
						ActualAmountFather(_dtItem, fatherId);
						SumAmountHeader(_dtItem, _dtDoc);
					}
				}
				tv_budget.DataSource = _dtItem;
				_dtPayment = dataSet.Tables[7];
				if ((_dtPayment == null || _dtPayment.Rows.Count == 0) && Function.ParseInt(gv_Doc.GetFocusedRowCellValue("BudgetLevel")) > 0)
				{
					tv_Payments.DataSource = _dtPayment;
					SetDataByLevelPayments();
				}
				tv_Payments.DataSource = _dtPayment;
				_dtDocument = dataSet.Tables[3];
				grd_LineDocument.DataSource = dataSet.Tables[3];
				grd_LineSub.DataSource = dataSet.Tables[4];
				_dtDepen = dataSet.Tables[5];
				_dtAddTeam = dataSet.Tables[1];
				DataTable dtAtt = dataSet.Tables[6];
				for (int i = 0; i < dtAtt.Rows.Count; i++)
				{
					int num7 = Function.ParseInt(dtAtt.Rows[i]["Reporter"]);
					if (btn_report.dictionSource.ContainsKey(Function.ToString(num7)))
					{
						dtAtt.Rows[i]["Reporter"] = ((num7 > 0) ? dtAtt.Rows[i]["Reporter"] : DBNull.Value);
					}
					else
					{
						int num8 = await EmployeeId(num7);
						if (num8 > 0)
						{
							dtAtt.Rows[i]["Reporter"] = num8;
						}
						else
						{
							dtAtt.Rows[i]["Reporter"] = DBNull.Value;
						}
					}
					int num9 = Function.ParseInt(dtAtt.Rows[i]["HandleBy"]);
					if (btn_handle.dictionSource.ContainsKey(Function.ToString(num9)))
					{
						dtAtt.Rows[i]["HandleBy"] = ((num9 > 0) ? dtAtt.Rows[i]["HandleBy"] : DBNull.Value);
					}
					else
					{
						dtAtt.Rows[i]["HandleBy"] = DBNull.Value;
					}
					int num10 = Function.ParseInt(dtAtt.Rows[i]["ProjectOwner"]);
					if (btn_handle.dictionSource.ContainsKey(Function.ToString(num9)))
					{
						dtAtt.Rows[i]["ProjectOwner"] = ((num10 > 0) ? dtAtt.Rows[i]["ProjectOwner"] : DBNull.Value);
					}
					else
					{
						dtAtt.Rows[i]["ProjectOwner"] = DBNull.Value;
					}
					int num11 = Function.ParseInt(dtAtt.Rows[i]["SolutionBy"]);
					if (btn_IssueSLBy.dictionSource.ContainsKey(Function.ToString(num11)))
					{
						dtAtt.Rows[i]["SolutionBy"] = ((num10 > 0) ? dtAtt.Rows[i]["SolutionBy"] : DBNull.Value);
					}
					else
					{
						dtAtt.Rows[i]["SolutionBy"] = DBNull.Value;
					}
					int num12 = Function.ParseInt(dtAtt.Rows[i]["SolutionId"]);
					if (btn_solution.dictionSource.ContainsKey(Function.ToString(num12)))
					{
						dtAtt.Rows[i]["SolutionId"] = ((num10 > 0) ? dtAtt.Rows[i]["SolutionId"] : DBNull.Value);
					}
					else
					{
						dtAtt.Rows[i]["SolutionId"] = DBNull.Value;
					}
				}
				grd_lineissue.DataSource = dtAtt;
				await ReloadTaskList();
				await ReloadEventList();
				await ReloadNoteList();
			}
			await LoadCalendarData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
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
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
			return 0;
		}
	}

	public async Task REFRESH()
	{
		try
		{
			if (base.FormMode != Enums.FormViewModeType.View && (base.FormMode == Enums.FormViewModeType.Update || base.FormMode == Enums.FormViewModeType.Add) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.No)
			{
				return;
			}
			base.FormMode = Enums.FormViewModeType.Add;
			grd_Doc.DataSourceTable.Rows.Clear();
			tv_Wbs.DataSourceTable().Rows.Clear();
			grd_LineTeam.DataSourceTable.Rows.Clear();
			tv_budget.DataSourceTable().Rows.Clear();
			tv_Payments.DataSourceTable().Rows.Clear();
			grd_LineDocument.DataSourceTable.Rows.Clear();
			grd_LineSub.DataSourceTable.Rows.Clear();
			grd_EventList.DataSourceTable.Rows.Clear();
			grd_NoteList.DataSourceTable.Rows.Clear();
			tv_ListCompleted.DataSourceTable().Rows.Clear();
			_dtDepen.Rows.Clear();
			_dtDoc = grd_Doc.DataSourceTable;
			if (_dtDoc == null || _dtDoc.Columns.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.InfoCannotLoadData, "");
			}
			DataRow dataRow = _dtDoc.NewRow();
			dataRow["ProjectType"] = "I";
			dataRow["StartDate"] = DateTime.Now.Date;
			dataRow["PrgStatus"] = "D";
			dataRow["Priority"] = "2";
			dataRow["BudgetLevel"] = 1;
			dataRow["BaseLine"] = "N";
			dataRow["BaseLineActive"] = "N";
			dataRow["WddStatus"] = "-";
			dataRow["LoadBom"] = "N";
			dataRow["OwnerAuth"] = "F";
			DataRow dataRow2 = btn_Owner.DataSource.Select($"userId='{GlobalData.UserID}'").FirstOrDefault();
			if (dataRow2 != null)
			{
				dataRow["Owner"] = Function.ParseInt(dataRow2["empID"]);
			}
			dataRow["DocCur"] = GlobalData.MainCurrency;
			_dtDoc.Rows.Add(dataRow);
			grd_Doc.DataSource = _dtDoc;
			DataTable dataTable = tv_Wbs.DataSourceTable();
			dataTable.Columns["LineNum"].AutoIncrement = true;
			if (dataTable == null || dataTable.Rows.Count == 0)
			{
				dataTable.Columns["LineNum"].AutoIncrementStep = -1L;
				dataTable.Columns["LineNum"].AutoIncrementSeed = -1L;
				dataTable.Columns["LineNum"].AutoIncrementStep = 1L;
				dataTable.Columns["LineNum"].AutoIncrementSeed = 1L;
			}
			else
			{
				dataTable.Columns["LineNum"].AutoIncrementSeed = dataTable.AsEnumerable().Max((DataRow t) => Function.ParseLong(t["LineNum"])) + 1;
				dataTable.Columns["LineNum"].AutoIncrementStep = 1L;
			}
			NewTask();
			NewItem();
			NewDocument();
			await LoadCalendarData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void NewTask()
	{
		try
		{
			DataTable dataTable = tv_Wbs.DataSourceTable();
			if (dataTable == null || dataTable.Columns.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.InfoCannotLoadData, "");
				return;
			}
			_dtItem = tv_budget.DataSourceTable();
			List<DataRow> list = (from t in dataTable.AsEnumerable()
				where Function.ParseInt(t["FatherNum"]) == 0 || Function.ParseInt(t["FatherNum"]) == -1
				select t).ToList();
			int num = 0;
			if (list.Count > 0)
			{
				num = Function.ParseInt(list.CopyToDataTable().AsEnumerable().Max((DataRow t) => Function.ParseInt(t["Wbs"])));
			}
			DataRow dataRow = dataTable.NewRow();
			dataRow["Wbs"] = ((num <= 0) ? 1 : (num + 1));
			dataRow["TaskCode"] = ((num <= 0) ? 1 : (num + 1));
			if (dataTable.Rows.Count > 0)
			{
				dataRow["VisOrder"] = Function.ParseInt(dataTable.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["VisOrder"]))) + 1;
			}
			else
			{
				dataRow["VisOrder"] = 1;
			}
			dataRow["FatherNum"] = 0;
			dataRow["BaseLine"] = Function.ParseInt(dataRow["LineNum"]);
			dataRow["FromDate"] = Function.ParseDateTimes(gv_Doc.GetFocusedRowCellValue("StartDate"));
			dataRow["ToDate"] = Function.ParseDateTimes(gv_Doc.GetFocusedRowCellValue("StartDate"));
			dataRow["WBSMode"] = "M";
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			if (tv_Wbs.Columns["Wbs"].SortOrder == SortOrder.None)
			{
				tv_Wbs.BeginSort();
				tv_Wbs.Columns["Wbs"].SortOrder = SortOrder.Ascending;
				tv_Wbs.EndSort();
			}
			DataChanged();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void NewItem()
	{
		try
		{
			int num = 0;
			_dtItem = tv_budget.DataSourceTable();
			if (_dtItem == null || _dtItem.Columns.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.InfoCannotLoadData, "");
				return;
			}
			int num2 = Function.ParseInt(_dtItem.Compute("MAX(LineNum)", ""));
			DataRow dataRow = _dtItem.NewRow();
			dataRow["LineNum"] = num2 + 1;
			dataRow["IsNew"] = true;
			dataRow["OrderItem"] = -1;
			dataRow["FatherNum"] = -1;
			dataRow["Currency"] = Function.ToString(gv_Doc.GetFocusedRowCellValue("DocCur"));
			List<DataRow> list = (from t in _dtItem.AsEnumerable()
				where Function.ParseInt(t["FatherNum"]) == 0 || Function.ParseInt(t["FatherNum"]) == -1
				select t).ToList();
			if (list.Count > 0)
			{
				num = Function.ParseInt(list.CopyToDataTable().AsEnumerable().Max((DataRow t) => Function.ParseInt(t["Wbs"])));
			}
			dataRow["Wbs"] = ((num <= 0) ? 1 : (num + 1));
			if (_dtItem.Rows.Count > 0)
			{
				dataRow["VisOrder"] = Function.ParseInt(_dtItem.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["VisOrder"]))) + 1;
			}
			else
			{
				dataRow["VisOrder"] = 1;
			}
			_dtItem.Rows.Add(dataRow);
			_dtItem.AcceptChanges();
			DataChanged();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void NewPayment()
	{
		try
		{
			_dtPayment = tv_Payments.DataSourceTable();
			tv_Wbs.DataSourceTable();
			if (_dtPayment == null || _dtPayment.Columns.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.InfoCannotLoadData, "");
				return;
			}
			int num = Function.ParseInt(_dtPayment.Compute("MAX(LineNum)", ""));
			DataRow dataRow = _dtPayment.NewRow();
			dataRow["LineNum"] = num + 1;
			dataRow["IsNew"] = true;
			dataRow["FatherNum"] = -1;
			if (_dtPayment.Rows.Count > 0)
			{
				dataRow["VisOrder"] = Function.ParseInt(_dtPayment.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["VisOrder"]))) + 1;
			}
			else
			{
				dataRow["VisOrder"] = 1;
			}
			_dtPayment.Rows.Add(dataRow);
			_dtPayment.AcceptChanges();
			DataChanged();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void NewDocument()
	{
		try
		{
			_dtDocument = grd_LineDocument.DataSourceTable;
			if (_dtDocument == null || _dtDocument.Columns.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.CannotGetData, "");
				return;
			}
			DataRow row = _dtDocument.NewRow();
			_dtDocument.Rows.Add(row);
			_dtDocument.AcceptChanges();
			DataChanged();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task REFRESH_DATA()
	{
		btn_Search_Click(null, null);
	}

	private void LoadComboIndustry()
	{
		try
		{
			luk_Industry.DisplayMember = "IndName";
			luk_Industry.ValueMember = "IndCode";
			DataTable dataTable = _httpClient.BusinessMaster.LoadComboIndustry();
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				luk_Industry.DataSource = dataTable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<int> Save(int type)
	{
		_ = 3;
		try
		{
			DataTable dtTask = tv_Wbs.DataSourceTable().Copy();
			for (int i = 0; i < dtTask.Rows.Count; i++)
			{
				if (Function.ToString(dtTask.Rows[i]["TaskCode"]) == "" && !string.IsNullOrEmpty(Function.ToString(dtTask.Rows[i]["TaskName"])))
				{
					WinCommons.Warning(Enums.MessageCode.TaskCodeIsNullAtWbs, "", Function.ToString(dtTask.Rows[i]["Wbs"]));
					return 1;
				}
				if (Function.ToString(dtTask.Rows[i]["TaskCode"]) == "" && string.IsNullOrEmpty(Function.ToString(dtTask.Rows[i]["TaskName"])))
				{
					dtTask.Rows.RemoveAt(i--);
				}
			}
			grd_Doc.DatasourceTable().Copy();
			List<string> list = (from dr in dtTask.Copy().AsEnumerable()
				select dr.Field<string>("TaskCode") into x
				group x by x into g
				where g.Count() > 1
				select g.Key).ToList();
			if (list.Count() > 0)
			{
				WinCommons.Error(Enums.MessageCode.TaskCodeExistsInTableWithId, "", Function.ToString(list[0]));
				return 1;
			}
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				WinCommons._OpenProcessing();
				DataSet ds = new DataSet();
				if (type == 2)
				{
					DataTable dataTable = grd_Doc.DataSourceTable.Copy();
					dataTable.AcceptChanges();
					DataRow[] array = dataTable.Select("AbsEntry=" + Function.ParseInt(gv_Doc.GetFocusedRowCellValue("AbsEntry")));
					if (array.Length != 0)
					{
						_dtDoc = array.CopyToDataTable();
					}
				}
				else
				{
					_dtDoc = grd_Doc.DataSourceTable.Copy();
				}
				if (Function.ToString(_dtDoc.Rows[0]["MasterEntry"]) == "")
				{
					_dtDoc.Rows[0]["MasterEntry"] = 0;
				}
				if (string.IsNullOrEmpty(Function.ToString(_dtDoc.Rows[0]["Owner"])))
				{
					DataRow dataRow = btn_Owner.DataSource.Select($"userId='{GlobalData.UserID}'").FirstOrDefault();
					if (dataRow != null)
					{
						_dtDoc.Rows[0]["Owner"] = Function.ParseInt(dataRow["empID"]);
					}
				}
				DataTable dtts = dtTask.Copy();
				(from DataRow t in dtts.Rows
					where string.IsNullOrEmpty(Function.ToString(t["TaskCode"]))
					select t).ToList().ForEach((DataRow r) =>
				{
					r.Delete();
				});
				_dtTeam = grd_LineTeam.DataSourceTable.Copy();
				_dtItem = tv_budget.DataSourceTable().Copy();
				if (_dtItem != null && _dtItem.Rows.Count > 0)
				{
					for (int num = 0; num < _dtItem.Rows.Count; num++)
					{
						if (!string.IsNullOrEmpty(Function.ToString(_dtItem.Rows[num]["ItemCode"])))
						{
							_dtItem.Rows[num]["ItemCode"] = Function.ToString(_dtItem.Rows[num]["ItemCode"]).Trim();
						}
						if (Function.ToString(_dtItem.Rows[num]["OrderItem"]) == "")
						{
							_dtItem.Rows[num]["OrderItem"] = -1;
						}
						if (Function.ToString(_dtItem.Rows[num]["ItemCode"]) == "" && Function.ToString(_dtItem.Rows[num]["TaskCode"]) == "")
						{
							_dtItem.Rows.RemoveAt(num--);
						}
					}
				}
				else
				{
					gv_Doc.SetFocusedRowCellValue("BudgetLevel", DBNull.Value);
				}
				_dtItem.AcceptChanges();
				_dtDocument = grd_LineDocument.DataSourceTable.Copy();
				_dtSub = grd_LineSub.DataSourceTable.Copy();
				_dtAddTeam = await AddTeam();
				_dtPayment = tv_Payments.DataSourceTable().Copy();
				if (_dtPayment != null && _dtPayment.Rows.Count > 0)
				{
					for (int num2 = 0; num2 < _dtPayment.Rows.Count; num2++)
					{
						if (Function.ToString(_dtPayment.Rows[num2]["InvoicePlan"]) == "" && Function.ToString(_dtPayment.Rows[num2]["IncomingPayment"]) == "" && Function.ToString(_dtPayment.Rows[num2]["TaskCode"]) == "")
						{
							_dtPayment.Rows.RemoveAt(num2--);
						}
					}
				}
				_dtDoc.TableName = "dtDoc";
				dtTask.TableName = "dtTask";
				_dtItem.TableName = "dtItem";
				_dtAddTeam.TableName = "dtTeam";
				_dtDocument.TableName = "dtDocment";
				_dtDepen.TableName = "dtDepen";
				_dtSub.TableName = "dtSub";
				_dtPayment.TableName = "dtPayment";
				ds.Tables.Add(_dtDoc.Copy());
				ds.Tables.Add(dtts.Copy());
				ds.Tables.Add(_dtDepen.Copy());
				ds.Tables.Add(_dtAddTeam.Copy());
				ds.Tables.Add(_dtItem.Copy());
				ds.Tables.Add(_dtDocument.Copy());
				ds.Tables.Add(_dtSub.Copy());
				ds.Tables.Add(_dtPayment.Copy());
				HttpResult resp = ((type != 2) ? (await _httpClient.ProjectManager.Add(ds)) : (await _httpClient.ProjectManager.Update(ds)));
				if (string.IsNullOrEmpty(resp.Message))
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
					if (_addsub)
					{
						int arg = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("MasterEntry"));
						_isLoading = true;
						if (SendDataBackToMain != null && !((XtraForm)SendDataBackToMain.Target).IsDisposed)
						{
							await SendDataBackToMain(arg);
							btn_Code.RefreshDataSource();
							txt_No.EditValue = Function.ParseInt(resp.Content);
							btn_Search_Click(null, null);
							base.FormMode = Enums.FormViewModeType.View;
						}
						_isLoading = false;
						_addsub = false;
						return 0;
					}
					_isLoading = true;
					base.FormMode = Enums.FormViewModeType.View;
					if (type == 2)
					{
						grd_Doc.DatasourceTable().AcceptChanges();
						tv_Wbs.DataSourceTable().AcceptChanges();
						grd_LineTeam.DataSourceTable().AcceptChanges();
						tv_budget.DataSourceTable().AcceptChanges();
					}
					else
					{
						btn_Search_Click(null, null);
					}
					_isLoading = false;
					return 0;
				}
				WinCommons.Error(Enums.MessageCode.None, resp.Message);
				return 1;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return 1;
		}
		WinCommons._CloseProcessing();
		return 1;
	}

	private async Task<DataTable> AddTeam()
	{
		try
		{
			DataTable dtTask = tv_Wbs.DataSourceTable;
			DataTable dt = grd_LineTeam.DataSourceTable.Copy();
			dt.Rows.Clear();
			for (int i = 0; i < dtTask.Rows.Count; i++)
			{
				string[] array = Function.ToString(dtTask.Rows[i]["Resource"]).Split(';');
				if (array.Length == 0)
				{
					continue;
				}
				string[] array2 = array;
				foreach (string d in array2)
				{
					if (Function.ParseInt(d) <= 0)
					{
						continue;
					}
					DataSet dataSet = await _httpClient.EmployeeMasterClient.GetDetail(Function.ParseInt(d));
					DataRow dataRow = dt.NewRow();
					int num = 0;
					DataRow[] array3 = _dtTeam.Select($"Resource = '{Function.ParseInt(d)}'");
					if (array3.Length != 0)
					{
						for (int k = 0; k < array3.Count(); k++)
						{
							num = Function.ParseInt(array3[k]["Role"]);
						}
					}
					string value = (btn_Owner.dictionSource.ContainsKey(Function.ToString(Function.ParseInt(d))) ? btn_Owner.dictionSource[Function.ToString(d)] : "");
					dataRow["AbsEntry"] = Function.ParseInt(dtTask.Rows[i]["AbsEntry"]);
					dataRow["LineNum"] = Function.ParseInt(dtTask.Rows[i]["LineNum"]);
					dataRow["Resource"] = Function.ParseInt(d);
					dataRow["EmployeeName"] = value;
					if (dataSet.Tables[0] != null)
					{
						if (dataSet.Tables[0].Rows.Count > 0)
						{
							dataRow["Branch"] = dataSet.Tables[0].Rows[0]["branch"];
						}
					}
					else
					{
						dataRow["Branch"] = 0;
					}
					if (dataSet.Tables[0] != null)
					{
						if (dataSet.Tables[0].Rows.Count > 0)
						{
							dataRow["Department"] = dataSet.Tables[0].Rows[0]["dept"];
						}
					}
					else
					{
						dataRow["Department"] = 0;
					}
					dataRow["Role"] = num;
					dt.Rows.Add(dataRow);
					dt.AcceptChanges();
				}
			}
			return dt;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	private void RefreshVisOrder()
	{
		try
		{
			DataTable dataSourceTable = tv_Wbs.DataSourceTable;
			if (dataSourceTable.IsNotNull())
			{
				int num = 1;
				List<DataRow> list = (from t in dataSourceTable.AsEnumerable()
					select (t)).ToList();
				list.Sort(new WbsComparer());
				foreach (DataRow item in list)
				{
					Function.ParseInt(item["LineNum"]);
					item["VisOrder"] = num;
					num++;
				}
			}
			if (tv_Wbs.Columns["Wbs"].SortOrder == SortOrder.None)
			{
				tv_Wbs.BeginSort();
				tv_Wbs.Columns["Wbs"].SortOrder = SortOrder.Ascending;
				tv_Wbs.EndSort();
				tv_Wbs.FocusedNode.Expand();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void RefreshWbs(TreeListNode node, DataTable dtTask, string isF)
	{
		try
		{
			if (node == null)
			{
				return;
			}
			int fatherId = Function.ParseInt(node.GetValue("FatherNum"));
			string wbs = Function.ToString(node.GetValue("Wbs"));
			int[] lstwbs = (from t in wbs.Split('.')
				select Function.ParseInt(t)).ToArray();
			int level = node.Level;
			int linenum = Function.ParseInt(node.GetValue("LineNum"));
			if (isF == "Y" && fatherId > 0 && level > 1)
			{
				List<DataRow> list = (from t in dtTask.AsEnumerable()
					where Function.ParseInt(t["FatherNum"]) == fatherId && Function.ParseInt(t["LineNum"]) != linenum && GetWbsByLevel(Function.ToString(t["Wbs"]), level) >= GetWbsByLevel(wbs, level)
					select t).ToList();
				list.Sort(new WbsComparer());
				if (list.Count > 0)
				{
					for (int num = 0; num < list.Count; num++)
					{
						list[num]["Wbs"] = GetNextWbs(lstwbs, level);
						int num2 = Function.ParseInt(list[num]["LineNum"]);
						TreeListNode node2 = tv_Wbs.FindNodeByKeyID(num2);
						RefreshWbs(node2, dtTask, "N");
					}
				}
			}
			List<DataRow> list2 = (from t in dtTask.AsEnumerable()
				where Function.ParseInt(t["FatherNum"]) == linenum
				select t).ToList();
			list2.Sort(new WbsComparer());
			if (list2.Count > 0)
			{
				for (int num3 = 0; num3 < list2.Count; num3++)
				{
					list2[num3]["Wbs"] = string.Join(".", wbs, num3 + 1);
					int num4 = Function.ParseInt(list2[num3]["LineNum"]);
					TreeListNode node3 = tv_Wbs.FindNodeByKeyID(num4);
					RefreshWbs(node3, dtTask, "N");
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private string GetNextWbs(int[] lstwbs, int level)
	{
		if (lstwbs.Length > level)
		{
			lstwbs[level]++;
			return string.Join(".", lstwbs);
		}
		return "";
	}

	private int GetWbsByLevel(string wbs, int level)
	{
		string[] array = wbs.Split('.');
		if (array.Length > level)
		{
			return Function.ParseInt(array[level]);
		}
		return 0;
	}

	private void InsertRow(DataTable dt, GridViewEx gv, GridControlEx grd)
	{
		try
		{
			int focusedRowHandle = gv.FocusedRowHandle;
			if (focusedRowHandle > 0)
			{
				dt = grd.DataSourceTable;
				if (dt == null || dt.Columns.Count == 0)
				{
					WinCommons.Error(Enums.MessageCode.InfoCannotLoadData, "");
					return;
				}
				DataRow row = dt.NewRow();
				dt.Rows.InsertAt(row, focusedRowHandle);
				dt.AcceptChanges();
				DataChanged();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void DeleteTeam()
	{
		try
		{
			string[] array = Function.ToString(tv_Wbs.GetFocusedRowCellValue(tl_Resource)).Split(';');
			int num = 0;
			DataTable dataTable = tv_Wbs.DataSourceTable();
			_dtTeam = grd_LineTeam.DataSourceTable;
			if (array.Count() <= 0)
			{
				return;
			}
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				int num2 = Function.ParseInt(array2[i]);
				if (num2 <= 0)
				{
					continue;
				}
				for (int j = 0; j < dataTable.Rows.Count; j++)
				{
					string[] array3 = Function.ToString(dataTable.Rows[j]["Resource"]).Split(';');
					foreach (string obj in array3)
					{
						if (num2 == Function.ParseInt(obj))
						{
							num++;
						}
					}
				}
				if (num == 1)
				{
					grd_LineTeam.DataSourceTable.Select($"Resource='{num2}'").FirstOrDefault()?.Delete();
				}
				num = 0;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void AddSub(int id)
	{
		try
		{
			string value = Function.ToString(gv_Doc.GetFocusedRowCellValue("ProjectType"));
			_dtDoc = grd_Doc.DataSourceTable.Copy();
			DataTable dataTable = tv_Wbs.DataSourceTable().Copy();
			_dtItem = tv_budget.DataSourceTable().Copy();
			_dtTeam = grd_LineTeam.DataSourceTable.Copy();
			_dtDocument = grd_LineDocument.DataSourceTable.Copy();
			_dtSub = grd_LineSub.DataSourceTable.Copy();
			_dtPayment = tv_Payments.DataSourceTable().Copy();
			_dtDoc.Rows.Clear();
			dataTable.Rows.Clear();
			_dtTeam.Rows.Clear();
			_dtItem.Rows.Clear();
			_dtDocument.Rows.Clear();
			_dtSub.Rows.Clear();
			_dtPayment.Clear();
			DataTable dataTable2 = _dtDepen.Copy();
			dataTable2.Rows.Clear();
			DataTable dataTable3 = _dtAddTeam.Copy();
			dataTable3.Rows.Clear();
			int ID = Function.ParseInt(gv_Doc.GetRowCellValue(gv_Doc.FocusedRowHandle, "AbsEntry"));
			DataRow[] array = (from t in grd_Doc.DataSourceTable.AsEnumerable()
				where Function.ParseInt(t["AbsEntry"]) == ID
				select t).ToArray();
			array[0]["AbsEntry"] = 0;
			array[0]["Code"] = "";
			array[0]["Name"] = "";
			array[0]["StartDate"] = DateTime.Now.Date;
			array[0]["PrgStatus"] = "D";
			array[0]["Priority"] = "2";
			array[0]["ProjectType"] = value;
			array[0]["MasterEntry"] = id;
			array[0]["BudgetLevel"] = 1;
			array[0]["BaseLine"] = "N";
			array[0]["BaseLineActive"] = "N";
			array[0]["WddStatus"] = "-";
			array[0]["LoadBom"] = "N";
			array[0]["UserCodeSign"] = GlobalData.UserCodeLogin;
			DataRow dataRow = btn_Owner.DataSource.Select($"userId='{GlobalData.UserID}'").FirstOrDefault();
			if (dataRow != null)
			{
				array[0]["Owner"] = Function.ParseInt(dataRow["empID"]);
			}
			array[0]["DocCur"] = GlobalData.MainCurrency;
			array[0]["OwnerAuth"] = "F";
			DataTable doc = array.CopyToDataTable();
			int num = Function.ParseInt(dataTable.Compute("MAX(Wbs)", ""));
			DataRow dataRow2 = dataTable.NewRow();
			dataRow2["Wbs"] = ((num <= 0) ? 1 : (num + 1));
			dataRow2["TaskCode"] = ((num <= 0) ? 1 : (num + 1));
			if (dataTable.Rows.Count > 0)
			{
				dataRow2["VisOrder"] = Function.ParseInt(dataTable.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["VisOrder"]))) + 1;
			}
			else
			{
				dataRow2["VisOrder"] = 1;
			}
			dataRow2["FatherNum"] = 0;
			dataRow2["BaseLine"] = Function.ParseInt(dataRow2["LineNum"]);
			dataRow2["FromDate"] = Function.ParseDateTimes(gv_Doc.GetFocusedRowCellValue("StartDate"));
			dataRow2["ToDate"] = Function.ParseDateTimes(gv_Doc.GetFocusedRowCellValue("StartDate"));
			dataRow2["WBSMode"] = "M";
			dataTable.Rows.Add(dataRow2);
			dataTable.AcceptChanges();
			DataRow dataRow3 = _dtItem.NewRow();
			dataRow3["IsNew"] = true;
			dataRow3["OrderItem"] = -1;
			dataRow3["FatherNum"] = -1;
			dataRow3["Currency"] = Function.ToString(gv_Doc.GetFocusedRowCellValue("DocCur"));
			int num2 = Function.ParseInt(_dtItem.Compute("MAX(Wbs)", ""));
			List<DataRow> list = (from t in _dtItem.AsEnumerable()
				where Function.ParseInt(t["FatherNum"]) == 0 || Function.ParseInt(t["FatherNum"]) == -1
				select t).ToList();
			if (list.Count > 0)
			{
				num2 = Function.ParseInt(list.CopyToDataTable().AsEnumerable().Max((DataRow t) => Function.ParseInt(t["Wbs"])));
			}
			dataRow3["Wbs"] = ((num2 <= 0) ? 1 : (num2 + 1));
			if (_dtItem.Rows.Count > 0)
			{
				dataRow3["VisOrder"] = Function.ParseInt(_dtItem.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["VisOrder"]))) + 1;
			}
			else
			{
				dataRow3["VisOrder"] = 1;
			}
			_dtItem.Rows.Add(dataRow3);
			_dtItem.AcceptChanges();
			DataRow row = _dtPayment.NewRow();
			_dtPayment.Rows.Add(row);
			_dtPayment.AcceptChanges();
			DataRow row2 = _dtDocument.NewRow();
			_dtDocument.Rows.Add(row2);
			_dtDocument.AcceptChanges();
			DataRow dataRow4 = _dtSub.NewRow();
			dataRow4["StartDate"] = DateTime.Now.Date;
			dataRow4["PrgStatus"] = "D";
			dataRow4["Priority"] = "2";
			_dtSub.Rows.Add(dataRow4);
			_addsub = true;
			frm_ProjectManager frm_ProjectManager2 = new frm_ProjectManager(doc, dataTable, _dtTeam, _dtItem, _dtPayment, _dtDocument, _dtSub, dataTable2, dataTable3, id)
			{
				MdiParent = MdiParent,
				StartPosition = FormStartPosition.CenterScreen
			};
			frm_ProjectManager2.SendDataBackToMain = (Func<int, Task>)Delegate.Combine(frm_ProjectManager2.SendDataBackToMain, new Func<int, Task>(ReloadSubProject));
			frm_ProjectManager2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		_addsub = false;
	}

	public static void CreateColumnsDistRule(GridViewEx gv)
	{
		try
		{
			if (GlobalData.Dimensions == null || GlobalData.Dimensions.Rows.Count <= 0)
			{
				return;
			}
			foreach (DataRow row in GlobalData.Dimensions.Rows)
			{
				AddDistRule(gv, row);
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private static void AddDistRule(GridViewEx gv, DataRow r)
	{
		try
		{
			string code = ((r["DimCode"] == null || r["DimCode"] == DBNull.Value) ? "" : r["DimCode"].ToString());
			if (string.IsNullOrWhiteSpace(code))
			{
				return;
			}
			if (code == "1")
			{
				code = "";
			}
			string fieldName = "OcrCode" + code.ToString();
			RepositoryItemButtonEdit repositoryItemButtonEdit = new RepositoryItemButtonEdit();
			repositoryItemButtonEdit.Name = "rep_line_" + fieldName;
			repositoryItemButtonEdit.Buttons[0].Kind = ButtonPredefines.Search;
			repositoryItemButtonEdit.ButtonClick += (object s, ButtonPressedEventArgs e) =>
			{
				try
				{
					Enums.TypeCFL type = Enums.TypeCFL.OrcCode;
					switch (code)
					{
					case "2":
						type = Enums.TypeCFL.OrcCode2;
						break;
					case "3":
						type = Enums.TypeCFL.OrcCode3;
						break;
					case "4":
						type = Enums.TypeCFL.OrcCode4;
						break;
					case "5":
						type = Enums.TypeCFL.OrcCode5;
						break;
					}
					frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(type, multiSelect: false, "");
					frm_ChooseFromList2.ShowDialog(gv.GridControl.FindForm());
					if (frm_ChooseFromList2.ChooseItem != null)
					{
						DataRow row = ((DataRowView)frm_ChooseFromList2.ChooseItem).Row;
						gv.SetRowCellValue(gv.FocusedRowHandle, fieldName, row[0]);
					}
				}
				catch (Exception ex2)
				{
					WinCommons.ErrorLineNumber(ex2);
				}
			};
			GridColumnEx gridColumnEx = new GridColumnEx();
			gridColumnEx.FieldName = fieldName;
			gridColumnEx.Caption = ((r["DimDesc"] == null || r["DimDesc"] == DBNull.Value) ? "" : r["DimDesc"].ToString());
			gridColumnEx.Visible = true;
			gridColumnEx.Width = 100;
			gridColumnEx.ColumnEdit = repositoryItemButtonEdit;
			gv.Columns.Add(gridColumnEx);
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public static void CreateTreeDistRule(TreeList gv)
	{
		try
		{
			if (GlobalData.Dimensions == null || GlobalData.Dimensions.Rows.Count <= 0)
			{
				return;
			}
			foreach (DataRow row in GlobalData.Dimensions.Rows)
			{
				AddDistRuleInTree(gv, row);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private static void AddDistRuleInTree(TreeList gv, DataRow r)
	{
		try
		{
			string code = ((r["DimCode"] == null || r["DimCode"] == DBNull.Value) ? "" : r["DimCode"].ToString());
			if (string.IsNullOrWhiteSpace(code))
			{
				return;
			}
			if (code == "1")
			{
				code = "";
			}
			string fieldName = "OcrCode" + code.ToString();
			RepositoryItemButtonEdit repositoryItemButtonEdit = new RepositoryItemButtonEdit();
			repositoryItemButtonEdit.Name = "rep_line_" + fieldName;
			repositoryItemButtonEdit.Buttons[0].Kind = ButtonPredefines.Search;
			repositoryItemButtonEdit.Buttons.Add(new EditorButton(ButtonPredefines.Glyph));
			repositoryItemButtonEdit.Buttons[1].ImageOptions.Image = Apzon.Forms.Properties.Resources.right;
			repositoryItemButtonEdit.Buttons[1].IsLeft = true;
			repositoryItemButtonEdit.Buttons[1].Caption = "DrillDown";
			repositoryItemButtonEdit.ButtonClick += (object s, ButtonPressedEventArgs e) =>
			{
				try
				{
					Enums.TypeCFL type = Enums.TypeCFL.OrcCode;
					switch (code)
					{
					case "2":
						type = Enums.TypeCFL.OrcCode2;
						break;
					case "3":
						type = Enums.TypeCFL.OrcCode3;
						break;
					case "4":
						type = Enums.TypeCFL.OrcCode4;
						break;
					case "5":
						type = Enums.TypeCFL.OrcCode5;
						break;
					}
					frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(type, multiSelect: false, "");
					frm_ChooseFromList2.ShowDialog();
					if (frm_ChooseFromList2.ChooseItem != null)
					{
						DataRow row = ((DataRowView)frm_ChooseFromList2.ChooseItem).Row;
						gv.SetFocusedRowCellValue(fieldName, row[0]);
						gv.CloseEditor();
					}
				}
				catch (Exception ex2)
				{
					WinCommons.ErrorLineNumber(ex2);
				}
			};
			TreeListColumn treeListColumn = gv.Columns[fieldName];
			if (treeListColumn == null)
			{
				treeListColumn = new TreeListColumnEx();
				treeListColumn.FieldName = fieldName;
				treeListColumn.Visible = false;
				treeListColumn.Width = 100;
				gv.Columns.Add(treeListColumn);
			}
			treeListColumn.Caption = ((r["DimDesc"] == null || r["DimDesc"] == DBNull.Value) ? "" : r["DimDesc"].ToString());
			gv.RepositoryItems.Add(repositoryItemButtonEdit);
			treeListColumn.ColumnEdit = repositoryItemButtonEdit;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private async Task ReloadTaskList()
	{
		try
		{
			DataTable dataSource = await _httpClient.SubActivities.LoadTaskAndSubTask(GlobalData.UserCodeLogin, ApzObjectTypes.oProjectManager, Function.ToString(gv_Doc.GetFocusedRowCellValue("AbsEntry")));
			tv_ListCompleted.DataSource = dataSource;
			tv_ListCompleted.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task ReloadTaskList(int clgCode)
	{
		try
		{
			DataTable dataSource = await _httpClient.SubActivities.LoadTaskAndSubTask(GlobalData.UserCodeLogin, ApzObjectTypes.oProjectManager, Function.ToString(gv_Doc.GetFocusedRowCellValue("AbsEntry")));
			tv_ListCompleted.DataSource = dataSource;
			tv_ListCompleted.RefreshDataSource();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task ReloadSubProject(int id)
	{
		try
		{
			DataTable dataSource = await _httpClient.ProjectManager.LoadSubProjectByMasterId(id);
			grd_LineSub.DataSource = dataSource;
			grd_LineSub.RefreshDataSource();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void CheckWbsTemplate(DataTable dt, int fatherId)
	{
		if (fatherId > 0)
		{
			List<DataRow> list = (from x in dt.AsEnumerable()
				where Function.ParseInt(x["LineNum"]) == fatherId
				select x).ToList();
			if (!string.IsNullOrEmpty(Function.ToString(list[0]["WbsTemplate"])))
			{
				isTemplateWbs = true;
				return;
			}
			int fatherId2 = Function.ParseInt(list[0]["FatherNum"]);
			isTemplateWbs = false;
			CheckWbsTemplate(dt, fatherId2);
		}
	}

	private void NewTask(int fatherId = -1, string line = "")
	{
		try
		{
			if (_isinout)
			{
				return;
			}
			DataTable dataTable = tv_Wbs.DataSourceTable();
			CheckWbsTemplate(dataTable, fatherId);
			if (dataTable == null || fatherId == 0 || dataTable.AsEnumerable().FirstOrDefault((DataRow t) => string.IsNullOrEmpty(Function.ToString(t["Wbs"])) && Function.ParseInt(t["FatherNum"]) == fatherId) != null)
			{
				return;
			}
			TreeListNode treeListNode = tv_Wbs.FindNodeByKeyID(fatherId);
			int num = 0;
			if (treeListNode != null)
			{
				num = treeListNode.Level + 1;
			}
			DataRow[] array = (from t in dataTable.AsEnumerable()
				where Function.ParseInt(t["FatherNum"]) == fatherId
				select t).ToArray();
			string value = "";
			if (array.Length != 0)
			{
				string text = array.Select((DataRow t) => Function.ToString(t["Wbs"])).ToList().SortAsc()
					.LastOrDefault();
				if (text != null)
				{
					int[] array2 = (from t in text.Split('.')
						select Function.ParseInt(t)).ToArray();
					if (array2.Length > num)
					{
						array2[num]++;
						value = string.Join(".", array2);
					}
				}
			}
			else
			{
				value = string.Join(".", Function.ToString(treeListNode.GetValue("Wbs")), 1);
			}
			if (array.Length != 0)
			{
				if (!string.IsNullOrEmpty(line))
				{
					DataRow dataRow = (from t in dataTable.AsEnumerable()
						where Function.ParseInt(t["FatherNum"]) == fatherId
						select t).ToArray().ToList().FirstOrDefault((DataRow t) => Function.ToString(t["TaskCode"]) == "" || Function.ToString(t["TaskName"]) == "");
					if (dataRow != null)
					{
						Function.ToString(dataRow["Wbs"]);
					}
					else
					{
						DataRow dataRow2 = dataTable.NewRow();
						dataRow2["FatherNum"] = fatherId;
						dataRow2["Wbs"] = value;
						dataRow2["WBSMode"] = "M";
						if (!isTemplateWbs)
						{
							dataRow2["TaskCode"] = value;
						}
						dataTable.Rows.Add(dataRow2);
						dataTable.AcceptChanges();
						RefreshVisOrder();
					}
				}
			}
			else
			{
				DataRow dataRow3 = dataTable.NewRow();
				dataRow3["FatherNum"] = fatherId;
				dataRow3["Wbs"] = value;
				if (!isTemplateWbs)
				{
					dataRow3["TaskCode"] = value;
				}
				dataRow3["WBSMode"] = "M";
				dataTable.Rows.Add(dataRow3);
				dataTable.AcceptChanges();
				RefreshVisOrder();
			}
			SetDataByLevelItem();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void GetAllFatherId(int lineNum, ref List<int> lst)
	{
		try
		{
			DataRow dataRow = tv_Wbs.DataSourceTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["LineNum"]) == lineNum);
			if (dataRow != null)
			{
				int num = Function.ParseInt(dataRow["FatherNum"]);
				lst.Add(num);
				GetAllFatherId(num, ref lst);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void GetAllChildId(int lineNum, ref List<int> lst)
	{
		try
		{
			DataRow[] array = (from t in tv_Wbs.DataSourceTable.AsEnumerable()
				where Function.ParseInt(t["FatherNum"]) == lineNum
				select t).ToArray();
			if (array.Length != 0)
			{
				DataRow[] array2 = array;
				for (int num = 0; num < array2.Length; num++)
				{
					int num2 = Function.ParseInt(array2[num]["LineNum"]);
					lst.Add(num2);
					GetAllChildId(num2, ref lst);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private int GetPreviousNodeSameLevel(TreeListNode node)
	{
		try
		{
			if (node != null)
			{
				int fatherNum = Function.ParseInt(node.GetValue("FatherNum"));
				int linenum = Function.ParseInt(node.GetValue("LineNum"));
				string wbs = Function.ToString(node.GetValue("Wbs"));
				int level = node.Level;
				DataRow dataRow = tv_Wbs.DataSourceTable.AsEnumerable().LastOrDefault((DataRow t) => Function.ParseInt(t["FatherNum"]) == fatherNum && Function.ParseInt(t["LineNum"]) != linenum && GetWbsByLevel(Function.ToString(t["Wbs"]), level) <= GetWbsByLevel(wbs, level));
				if (dataRow != null)
				{
					return Function.ParseInt(dataRow["LineNum"]);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		return -1;
	}

	private void DataChanged()
	{
		if (!_isLoading && base.FormMode == Enums.FormViewModeType.View)
		{
			base.FormMode = Enums.FormViewModeType.Update;
		}
	}

	private void SumAmountHeader(DataTable dt, DataTable dtDoc)
	{
		try
		{
			double num = 0.0;
			double num2 = 0.0;
			List<DataRow> list = (from t in dt.AsEnumerable()
				where Function.ParseInt(t["FatherNum"]) == 0 || Function.ParseInt(t["FatherNum"]) == -1
				select t).ToList();
			if (list != null && list.Count > 0)
			{
				for (int num3 = 0; num3 < list.Count; num3++)
				{
					Function.ParseDouble(list[num3]["RefPrice"]);
					Function.ParseDouble(list[num3]["ActualQty"]);
					num += Function.ParseDouble(list[num3]["Amount"]);
					num2 += Function.ParseDouble(list[num3]["ActualAmount"]);
				}
				double num4 = num2 - num;
				if (dt != null && dt.Rows.Count > 0 && dtDoc != null && dtDoc.Rows.Count > 0)
				{
					dtDoc.Rows[gv_Doc.GetFocusedDataSourceRowIndex()]["BudgetAmount"] = num;
					dtDoc.Rows[gv_Doc.GetFocusedDataSourceRowIndex()]["ActualAmount"] = num2;
					dtDoc.Rows[gv_Doc.GetFocusedDataSourceRowIndex()]["AmountDiff"] = num4;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void SetDataByLevelItem()
	{
		try
		{
			WinCommons._OpenProcessing();
			tv_Wbs.DataSourceTable().AcceptChanges();
			tv_budget.DataSourceTable().AcceptChanges();
			DataTable dataTable = tv_Wbs.DataSourceTable();
			_dtItem = tv_budget.DataSourceTable();
			_dtDoc = grd_Doc.DataSourceTable();
			if (_dtItem != null && _dtItem.Rows.Count > 0)
			{
				for (int i = 0; i < _dtItem.Rows.Count; i++)
				{
					if (Function.ToString(_dtItem.Rows[i]["ItemCode"]) == "" && Function.ToString(_dtItem.Rows[i]["TaskCode"]) == "")
					{
						_dtItem.Rows.RemoveAt(i--);
					}
				}
			}
			_dtItem.AcceptChanges();
			int num = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("BudgetLevel"));
			Function.ParseInt(tv_Wbs.GetFocusedRowCellValue(tl_Code));
			if (num > 0)
			{
				int level = num - 1;
				if (dataTable != null && dataTable.Rows.Count > 0)
				{
					List<TreeListNode> lst = new List<TreeListNode>();
					if (tv_Wbs.Nodes.Count > 0)
					{
						for (int j = 0; j < tv_Wbs.Nodes.Count; j++)
						{
							GetLevelNodes(tv_Wbs.Nodes[j], ref lst, level);
						}
					}
					lst.Where((TreeListNode t) => Function.ParseInt(t["Level"]) == 0).ToList();
					DataTable dataTable2 = dataTable.Copy();
					DataTable dtClone = dataTable2.Clone();
					foreach (TreeListNode node in lst)
					{
						DataRow dataRow = (from t in dataTable2.AsEnumerable()
							where Function.ParseInt(t["LineNum"]) == Function.ParseInt(node["LineNum"])
							select t).FirstOrDefault();
						dataRow["BaseLine"] = Function.ParseInt(dataRow["LineNum"]);
						dtClone.ImportRow(dataRow);
						dtClone.AcceptChanges();
					}
					if (_dtItem != null && _dtItem.Rows.Count > 0)
					{
						if (dtClone.Rows.Count > 0 && dtClone != null)
						{
							int item;
							for (item = 0; item < dtClone.Rows.Count; item++)
							{
								int num2 = Function.ParseInt(_dtItem.Compute("MAX(LineNum)", ""));
								if (string.IsNullOrEmpty(Function.ToString(dtClone.Rows[item]["TaskCode"])))
								{
									continue;
								}
								DataRow[] array = _dtItem.Select(string.Format("BaseLine={0}", Function.ParseInt(dtClone.Rows[item]["BaseLine"])));
								if (array.Count() == 0)
								{
									List<DataRow> list = (from t in dtClone.AsEnumerable()
										where Function.ParseInt(t["FatherNum"]) == Function.ParseInt(dtClone.Rows[item]["LineNum"])
										select t).ToList();
									DataRow dataRow2 = _dtItem.NewRow();
									dataRow2["LineNum"] = num2 + 1;
									if (list.Count > 0)
									{
										for (int num3 = 0; num3 < list.Count; num3++)
										{
											list[num3]["FatherNum"] = num2 + 1;
										}
									}
									DataRow dataRow3 = (from t in _dtItem.AsEnumerable()
										where Function.ParseInt(t["BaseLine"]) == Function.ParseInt(dtClone.Rows[item]["FatherNum"])
										select t).FirstOrDefault();
									if (Function.ParseInt(dtClone.Rows[item]["FatherNum"]) == 0)
									{
										dataRow2["FatherNum"] = 0;
									}
									else if (dataRow3 != null)
									{
										dataRow2["FatherNum"] = Function.ParseInt(dataRow3["LineNum"]);
									}
									dataRow2["TaskCode"] = Function.ToString(dtClone.Rows[item]["TaskCode"]);
									dataRow2["TaskName"] = Function.ToString(dtClone.Rows[item]["TaskName"]);
									if (Function.ParseDateTime(dtClone.Rows[item]["FromDate"]).HasValue)
									{
										dataRow2["FromDate"] = Function.ParseDateTime(dtClone.Rows[item]["FromDate"]);
									}
									else
									{
										dataRow2["FromDate"] = DBNull.Value;
									}
									if (Function.ParseDateTime(dtClone.Rows[item]["ToDate"]).HasValue)
									{
										dataRow2["ToDate"] = Function.ParseDateTime(dtClone.Rows[item]["ToDate"]);
									}
									else
									{
										dataRow2["FromDate"] = DBNull.Value;
									}
									dataRow2["BaseLine"] = Function.ParseInt(dtClone.Rows[item]["BaseLine"]);
									dataRow2["IsNew"] = false;
									dataRow2["Wbs"] = Function.ToString(dtClone.Rows[item]["Wbs"]);
									_dtItem.Rows.Add(dataRow2);
									_dtItem.AcceptChanges();
									tv_budget.DataSource = _dtItem;
									tv_budget.DataSourceTable().AcceptChanges();
									RefreshVisItem(_dtItem);
									continue;
								}
								DataRow[] array2 = array;
								foreach (DataRow dataRow4 in array2)
								{
									if (!string.IsNullOrEmpty(Function.ToString(dataRow4["TaskCode"])))
									{
										dataRow4["TaskName"] = Function.ToString(dtClone.Rows[item]["TaskName"]);
										if (Function.ParseDateTime(dtClone.Rows[item]["FromDate"]).HasValue)
										{
											dataRow4["FromDate"] = Function.ParseDateTime(dtClone.Rows[item]["FromDate"]);
										}
										else
										{
											dataRow4["FromDate"] = DBNull.Value;
										}
										if (Function.ParseDateTime(dtClone.Rows[item]["ToDate"]).HasValue)
										{
											dataRow4["ToDate"] = Function.ParseDateTime(dtClone.Rows[item]["ToDate"]);
										}
										else
										{
											dataRow4["ToDate"] = DBNull.Value;
										}
										_dtItem.AcceptChanges();
									}
								}
							}
						}
					}
					else
					{
						for (int num5 = 0; num5 < dtClone.Rows.Count; num5++)
						{
							DataRow dataRow5 = _dtItem.NewRow();
							dataRow5["FatherNum"] = Function.ParseInt(dtClone.Rows[num5]["FatherNum"]);
							dataRow5["TaskCode"] = Function.ToString(dtClone.Rows[num5]["TaskCode"]);
							dataRow5["TaskName"] = Function.ToString(dtClone.Rows[num5]["TaskName"]);
							dataRow5["LineNum"] = Function.ParseInt(dtClone.Rows[num5]["LineNum"]);
							dataRow5["BaseLine"] = Function.ParseInt(dtClone.Rows[num5]["BaseLine"]);
							if (Function.ParseDateTime(dtClone.Rows[num5]["FromDate"]).HasValue)
							{
								dataRow5["FromDate"] = Function.ParseDateTime(dtClone.Rows[num5]["FromDate"]);
							}
							else
							{
								dataRow5["FromDate"] = DBNull.Value;
							}
							if (Function.ParseDateTime(dtClone.Rows[num5]["ToDate"]).HasValue)
							{
								dataRow5["ToDate"] = Function.ParseDateTime(dtClone.Rows[num5]["ToDate"]);
							}
							else
							{
								dataRow5["ToDate"] = DBNull.Value;
							}
							dataRow5["IsNew"] = false;
							dataRow5["Wbs"] = Function.ToString(dtClone.Rows[num5]["Wbs"]);
							_dtItem.Rows.Add(dataRow5);
							_dtItem.AcceptChanges();
							tv_budget.DataSource = _dtItem;
							tv_budget.DataSourceTable().AcceptChanges();
							RefreshVisItem(_dtItem);
						}
					}
					for (int num6 = 0; num6 < _dtItem.Rows.Count; num6++)
					{
						int fatherId = Function.ParseInt(_dtItem.Rows[num6]["FatherNum"]);
						if (!string.IsNullOrEmpty(Function.ToString(_dtItem.Rows[num6]["TaskCode"])))
						{
							AmountFather(_dtItem, fatherId);
							ActualAmountFather(_dtItem, fatherId);
							SumAmountHeader(_dtItem, _dtDoc);
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private void SetDataByLevelPayments()
	{
		try
		{
			WinCommons._OpenProcessing();
			tv_Payments.DataSourceTable().AcceptChanges();
			DataTable dataTable = tv_Wbs.DataSourceTable();
			_dtPayment = tv_Payments.DataSourceTable();
			_dtDoc = grd_Doc.DataSourceTable();
			if (_dtPayment != null && _dtPayment.Rows.Count > 0)
			{
				for (int i = 0; i < _dtPayment.Rows.Count; i++)
				{
					if (Function.ToString(_dtPayment.Rows[i]["InvoicePlan"]) == "" && Function.ToString(_dtPayment.Rows[i]["IncomingPayment"]) == "" && Function.ToString(_dtPayment.Rows[i]["TaskCode"]) == "")
					{
						_dtPayment.Rows.RemoveAt(i--);
					}
				}
			}
			_dtPayment.AcceptChanges();
			int num = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("BudgetLevel"));
			Function.ParseInt(tv_Wbs.GetFocusedRowCellValue(tl_Code));
			Function.ParseInt(tv_Wbs.GetFocusedRowCellValue(tl_LineNum));
			if (num > 0)
			{
				int level = num - 1;
				if (dataTable != null && dataTable.Rows.Count > 0)
				{
					List<TreeListNode> lst = new List<TreeListNode>();
					if (tv_Wbs.Nodes.Count > 0)
					{
						for (int j = 0; j < tv_Wbs.Nodes.Count; j++)
						{
							GetLevelNodes(tv_Wbs.Nodes[j], ref lst, level);
						}
					}
					lst.Where((TreeListNode t) => Function.ParseInt(t["Level"]) == 0).ToList();
					DataTable dataTable2 = tv_Wbs.DataSourceTable.Copy();
					DataTable dtClone = dataTable2.Clone();
					foreach (TreeListNode node in lst)
					{
						DataRow dataRow = (from t in dataTable2.AsEnumerable()
							where Function.ParseInt(t["LineNum"]) == Function.ParseInt(node["LineNum"])
							select t).FirstOrDefault();
						dataRow["BaseLine"] = Function.ParseInt(dataRow["LineNum"]);
						dtClone.ImportRow(dataRow);
						dtClone.AcceptChanges();
					}
					if (_dtPayment != null && _dtPayment.Rows.Count > 0)
					{
						if (dtClone.Rows.Count > 0 && dtClone != null)
						{
							int item;
							for (item = 0; item < dtClone.Rows.Count; item++)
							{
								int num2 = Function.ParseInt(_dtPayment.Compute("MAX(LineNum)", ""));
								if (string.IsNullOrEmpty(Function.ToString(dtClone.Rows[item]["TaskCode"])))
								{
									continue;
								}
								DataRow[] array = _dtPayment.Select(string.Format("BaseLine={0}", Function.ParseInt(dtClone.Rows[item]["BaseLine"])));
								if (array.Count() == 0)
								{
									List<DataRow> list = (from t in dtClone.AsEnumerable()
										where Function.ParseInt(t["FatherNum"]) == Function.ParseInt(dtClone.Rows[item]["LineNum"])
										select t).ToList();
									DataRow dataRow2 = _dtPayment.NewRow();
									dataRow2["LineNum"] = num2 + 1;
									if (list.Count > 0)
									{
										for (int num3 = 0; num3 < list.Count; num3++)
										{
											list[num3]["FatherNum"] = num2 + 1;
										}
									}
									dataRow2["FatherNum"] = Function.ParseInt(dtClone.Rows[item]["FatherNum"]);
									dataRow2["TaskCode"] = Function.ToString(dtClone.Rows[item]["TaskCode"]);
									dataRow2["TaskName"] = Function.ToString(dtClone.Rows[item]["TaskName"]);
									dataRow2["BaseLine"] = Function.ParseInt(dtClone.Rows[item]["BaseLine"]);
									dataRow2["IsNew"] = false;
									dataRow2["VisOrder"] = Function.ParseInt(_dtPayment.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["VisOrder"]))) + 1;
									_dtPayment.Rows.Add(dataRow2);
									_dtPayment.AcceptChanges();
									continue;
								}
								DataRow[] array2 = array;
								foreach (DataRow dataRow3 in array2)
								{
									if (!string.IsNullOrEmpty(Function.ToString(dataRow3["TaskCode"])))
									{
										dataRow3["FatherNum"] = Function.ParseInt(dtClone.Rows[item]["FatherNum"]);
										dataRow3["TaskName"] = Function.ToString(dtClone.Rows[item]["TaskName"]);
										_dtPayment.AcceptChanges();
									}
								}
							}
							List<TreeListNode> lst2 = new List<TreeListNode>();
							if (tv_Payments.Nodes.Count > 0)
							{
								for (int num5 = 0; num5 < tv_Payments.Nodes.Count; num5++)
								{
									GetLevelNodesItem(tv_Payments.Nodes[num5], ref lst2);
								}
							}
						}
					}
					else
					{
						for (int num6 = 0; num6 < dtClone.Rows.Count; num6++)
						{
							DataRow dataRow4 = _dtPayment.NewRow();
							dataRow4["FatherNum"] = Function.ParseInt(dtClone.Rows[num6]["FatherNum"]);
							dataRow4["TaskCode"] = Function.ToString(dtClone.Rows[num6]["TaskCode"]);
							dataRow4["TaskName"] = Function.ToString(dtClone.Rows[num6]["TaskName"]);
							dataRow4["LineNum"] = Function.ParseInt(dtClone.Rows[num6]["LineNum"]);
							dataRow4["BaseLine"] = Function.ParseInt(dtClone.Rows[num6]["BaseLine"]);
							dataRow4["IsNew"] = false;
							dataRow4["VisOrder"] = 1;
							_dtPayment.Rows.Add(dataRow4);
							_dtPayment.AcceptChanges();
						}
					}
					tv_Payments.DataSource = _dtPayment;
					tv_Payments.DataSourceTable().AcceptChanges();
					for (int num7 = 0; num7 < _dtPayment.Rows.Count; num7++)
					{
						int fatherId = Function.ParseInt(Function.ToString(_dtPayment.Rows[num7]["FatherNum"]));
						InvoicePlanFather(_dtPayment, fatherId);
						IncomingPaymentFather(_dtPayment, fatherId);
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private void RefreshVisItem(DataTable dt)
	{
		try
		{
			if (dt.IsNotNull())
			{
				int num = 1;
				List<DataRow> list = (from t in dt.AsEnumerable()
					select (t)).ToList();
				list.Sort(new WbsComparer());
				if (list.Count > 0)
				{
					foreach (DataRow item in list)
					{
						item["VisOrder"] = num;
						if (string.IsNullOrEmpty(Function.ToString(item["OrderItem"])))
						{
							item["OrderItem"] = -1;
						}
						num++;
					}
				}
			}
			if (tv_budget.Columns["VisOrder"].SortOrder == SortOrder.None)
			{
				tv_budget.BeginSort();
				tv_budget.Columns["VisOrder"].SortOrder = SortOrder.Ascending;
				tv_budget.EndSort();
				tv_budget.FocusedNode.Expand();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void GetLevelNodes(TreeListNode node, ref List<TreeListNode> lst, int level)
	{
		try
		{
			if (node != null)
			{
				if (node.Level <= level)
				{
					lst.Add(node);
				}
				for (int i = 0; i < node.Nodes.Count; i++)
				{
					GetLevelNodes(node.Nodes[i], ref lst, level);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void GetLevelNodesItem(TreeListNode node, ref List<TreeListNode> lst)
	{
		try
		{
			if (node != null)
			{
				lst.Add(node);
				for (int i = 0; i < node.Nodes.Count; i++)
				{
					GetLevelNodesItem(node.Nodes[i], ref lst);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private int NewItemTree(int fatherId = -1, string line = "")
	{
		try
		{
			int num = -2;
			if (Function.ParseInt(gv_Doc.GetFocusedRowCellValue("BudgetLevel")) > 0)
			{
				DataTable dataTable = tv_budget.DataSourceTable();
				if (dataTable != null)
				{
					TreeListNode treeListNode = tv_budget.FindNodeByKeyID(fatherId);
					int num2 = 0;
					if (treeListNode != null)
					{
						num2 = treeListNode.Level + 1;
					}
					DataRow[] array = (from t in dataTable.AsEnumerable()
						where Function.ParseInt(t["FatherNum"]) == fatherId
						select t).ToArray();
					string value = "";
					if (array.Length != 0)
					{
						string text = (from t in array.AsEnumerable()
							where !string.IsNullOrEmpty(Function.ToString(t["Wbs"]))
							select Function.ToString(t["Wbs"])).ToList().SortAsc().LastOrDefault();
						if (text != null)
						{
							int[] array2 = (from t in text.Split('.')
								select Function.ParseInt(t)).ToArray();
							if (array2.Length > num2)
							{
								array2[num2]++;
								value = string.Join(".", array2);
							}
							else
							{
								value = Function.ToString(Function.ParseInt(array2.Max()) + 1);
							}
						}
					}
					else
					{
						value = string.Join(".", Function.ToString(treeListNode.GetValue("Wbs")), 1);
					}
					if (array.Length != 0)
					{
						num = Function.ParseInt(dataTable.Compute("MAX(LineNum)", ""));
						if (Function.ParseInt(line) > -1)
						{
							DataRow dataRow = array.ToList().FirstOrDefault((DataRow t) => Function.ToString(t["ItemCode"]) == "");
							if (dataRow != null)
							{
								string text2 = Function.ToString(dataRow["LineNum"]);
								tv_budget.FocusedNode = tv_budget.FindNodeByFieldValue("LineNum", text2);
								num = Function.ParseInt(text2) - 1;
							}
							else
							{
								DataRow dataRow2 = (from t in dataTable.AsEnumerable()
									where Function.ParseInt(t["LineNum"]) == fatherId
									select t).FirstOrDefault();
								DataRow dataRow3 = dataTable.NewRow();
								dataRow3["FatherNum"] = fatherId;
								dataRow3["LineNum"] = num + 1;
								dataRow3["Currency"] = Function.ToString(gv_Doc.GetFocusedRowCellValue("DocCur"));
								dataRow3["Dscription"] = "";
								dataRow3["OrderItem"] = -1;
								if (dataRow2 != null)
								{
									int num3 = Function.ParseInt(dataRow2["BaseLine"]);
									if (num3 > 0)
									{
										dataRow3["BaseLine"] = num3;
									}
									dataRow3["IsNew"] = true;
								}
								dataRow3["Wbs"] = value;
								dataTable.Rows.Add(dataRow3);
								dataTable.AcceptChanges();
								RefreshVisItem(dataTable);
							}
						}
					}
					else
					{
						DataRow dataRow4 = (from t in dataTable.AsEnumerable()
							where Function.ParseInt(t["LineNum"]) == Function.ParseInt(line)
							select t).FirstOrDefault();
						num = Function.ParseInt(dataTable.Compute("MAX(LineNum)", "")) + 1;
						if (dataRow4 != null)
						{
							Function.ToString(dataRow4["TaskCode"]);
							Function.ToString(dataRow4["TaskName"]);
							DataRow dataRow5 = dataTable.NewRow();
							dataRow5["FatherNum"] = fatherId;
							dataRow5["LineNum"] = num + 1;
							dataRow5["Currency"] = Function.ToString(gv_Doc.GetFocusedRowCellValue("DocCur"));
							int num4 = Function.ParseInt(dataRow4["BaseLine"]);
							dataRow5["OrderItem"] = -1;
							if (num4 > 0)
							{
								dataRow5["BaseLine"] = num4;
							}
							dataRow5["IsNew"] = true;
							dataRow5["Dscription"] = "";
							dataRow5["Wbs"] = value;
							dataTable.Rows.Add(dataRow5);
							dataTable.AcceptChanges();
							RefreshVisItem(dataTable);
						}
					}
					DataChanged();
				}
			}
			return num + 1;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
			return -1;
		}
	}

	private void AmountFather(DataTable dtItem, int fatherId = -1)
	{
		try
		{
			TreeListNode treeListNode = tv_budget.FindNodeByKeyID(fatherId);
			if (fatherId >= 0 && treeListNode != null && !(Function.ParseDecimal(treeListNode.GetValue("RefPrice")) != 0m))
			{
				double num = (from t in dtItem.AsEnumerable()
					where Function.ParseInt(t["FatherNum"]) == fatherId
					select t).Sum((DataRow t) => Function.ParseDouble(t["Amount"]));
				dtItem.Rows[treeListNode.Id]["Amount"] = num;
				if (fatherId != Function.ParseInt(treeListNode.GetValue("FatherNum")))
				{
					AmountFather(dtItem, Function.ParseInt(treeListNode.GetValue("FatherNum")));
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ActualAmountFather(DataTable dtItem, int fatherId = -1)
	{
		try
		{
			double num = (from t in dtItem.AsEnumerable()
				where Function.ParseInt(t["FatherNum"]) == fatherId
				select t).ToList().Sum((DataRow t) => Function.ParseDouble(t["ActualAmount"]));
			DataRow dataRow = dtItem.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["LineNum"]) == fatherId);
			if (num != 0.0 && dataRow != null)
			{
				dataRow["ActualAmount"] = num;
				dtItem.AcceptChanges();
				if (Function.ParseInt(dataRow["FatherNum"]) != Function.ParseInt(dataRow["LineNum"]))
				{
					ActualAmountFather(dtItem, Function.ParseInt(dataRow["FatherNum"]));
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void NewPaymentTree(int fatherId = -1, string line = "")
	{
		try
		{
			_dtPayment = tv_Payments.DataSourceTable();
			if (_dtPayment == null)
			{
				return;
			}
			if ((from t in _dtPayment.AsEnumerable()
				where Function.ParseInt(t["FatherNum"]) == fatherId
				select t).ToArray().Length != 0)
			{
				int num = Function.ParseInt(_dtPayment.Compute("MAX(LineNum)", ""));
				if (Function.ParseInt(line) > -1)
				{
					DataRow dataRow = (from t in _dtPayment.AsEnumerable()
						where Function.ParseInt(t["FatherNum"]) == fatherId
						select t).ToArray().ToList().FirstOrDefault((DataRow t) => Function.ToString(t["InvoicePlan"]) == "");
					if (dataRow != null)
					{
						string cellValue = Function.ToString(dataRow["LineNum"]);
						tv_Payments.FocusedNode = tv_Payments.FindNodeByFieldValue("LineNum", cellValue);
					}
					else
					{
						(from t in _dtPayment.AsEnumerable()
							where Function.ParseInt(t["LineNum"]) == Function.ParseInt(line)
							select t).FirstOrDefault();
						DataRow dataRow2 = (from t in _dtPayment.AsEnumerable()
							where Function.ParseInt(t["LineNum"]) == fatherId
							select t).FirstOrDefault();
						DataRow dataRow3 = _dtPayment.NewRow();
						dataRow3["FatherNum"] = fatherId;
						dataRow3["LineNum"] = num + 1;
						if (dataRow2 != null)
						{
							int num2 = Function.ParseInt(dataRow2["BaseLine"]);
							if (num2 > 0)
							{
								dataRow3["BaseLine"] = num2;
								dataRow3["IsNew"] = false;
							}
							else
							{
								dataRow3["IsNew"] = true;
							}
						}
						dataRow3["VisOrder"] = Function.ParseInt(_dtPayment.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["VisOrder"]))) + 1;
						_dtPayment.Rows.Add(dataRow3);
						_dtPayment.AcceptChanges();
						tv_Payments.FocusedNode = tv_Payments.FindNodeByFieldValue("LineNum", num + 1);
					}
				}
			}
			else
			{
				DataRow dataRow4 = (from t in _dtPayment.AsEnumerable()
					where Function.ParseInt(t["LineNum"]) == Function.ParseInt(line)
					select t).FirstOrDefault();
				int num3 = Function.ParseInt(_dtPayment.Compute("MAX(LineNum)", ""));
				if (dataRow4 != null)
				{
					DataRow dataRow5 = _dtPayment.NewRow();
					dataRow5["FatherNum"] = fatherId;
					dataRow5["LineNum"] = num3 + 1;
					int num4 = Function.ParseInt(dataRow4["BaseLine"]);
					if (num4 > 0)
					{
						dataRow5["BaseLine"] = num4;
						dataRow5["IsNew"] = false;
					}
					else
					{
						dataRow5["IsNew"] = true;
					}
					dataRow5["VisOrder"] = Function.ParseInt(_dtPayment.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["VisOrder"]))) + 1;
					_dtPayment.Rows.Add(dataRow5);
					_dtPayment.AcceptChanges();
					tv_Payments.FocusedNode = tv_Payments.FindNodeByFieldValue("LineNum", num3 + 1);
				}
			}
			tv_Payments.FocusedColumn = tp_InvoicePlan;
			DataChanged();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void InvoicePlanFather(DataTable dt, int fatherId = -1)
	{
		try
		{
			double num = (from t in dt.AsEnumerable()
				where Function.ParseInt(t["FatherNum"]) == fatherId
				select t).ToList().Sum((DataRow t) => Function.ParseDouble(t["InvoicePlan"]));
			DataRow dataRow = dt.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["LineNum"]) == fatherId);
			if (dataRow != null)
			{
				dataRow["InvoicePlan"] = num;
				dt.AcceptChanges();
				if (Function.ParseInt(dataRow["FatherNum"]) != Function.ParseInt(dataRow["LineNum"]))
				{
					InvoicePlanFather(dt, Function.ParseInt(dataRow["FatherNum"]));
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void IncomingPaymentFather(DataTable dt, int fatherId = -1)
	{
		try
		{
			double num = (from t in dt.AsEnumerable()
				where Function.ParseInt(t["FatherNum"]) == fatherId
				select t).ToList().Sum((DataRow t) => Function.ParseDouble(t["IncomingPayment"]));
			DataRow dataRow = dt.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["LineNum"]) == fatherId);
			if (dataRow != null)
			{
				dataRow["IncomingPayment"] = num;
				dt.AcceptChanges();
				if (Function.ParseInt(dataRow["FatherNum"]) != Function.ParseInt(dataRow["LineNum"]))
				{
					IncomingPaymentFather(dt, Function.ParseInt(dataRow["FatherNum"]));
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public void LoadCopyMenu()
	{
		try
		{
			PopCopyFrom = new List<ApzObjectTypes>
			{
				ApzObjectTypes.oPurchaseRequest,
				ApzObjectTypes.oPurchaseOrders,
				ApzObjectTypes.oPurchaseQuotations,
				ApzObjectTypes.oPurchaseDeliveryNotes,
				ApzObjectTypes.oDeliveryNotes,
				ApzObjectTypes.oInvoices,
				ApzObjectTypes.oPurchaseInvoices,
				ApzObjectTypes.oDownPayments,
				ApzObjectTypes.oPurchaseDownPayments,
				ApzObjectTypes.oInventoryGenExit,
				ApzObjectTypes.oReportCompletion
			};
			DXPopupMenu dXPopupMenu = new DXPopupMenu();
			foreach (ApzObjectTypes pop in PopCopyFrom)
			{
				DXMenuItem dXMenuItem = new DXMenuItem();
				switch (pop)
				{
				case ApzObjectTypes.oPurchaseRequest:
					dXMenuItem.Caption = MenuCaption.PurchaseRequest;
					break;
				case ApzObjectTypes.oPurchaseOrders:
					dXMenuItem.Caption = MenuCaption.PurchaseOder;
					break;
				case ApzObjectTypes.oPurchaseQuotations:
					dXMenuItem.Caption = MenuCaption.PurchaseQuotation;
					break;
				case ApzObjectTypes.oPurchaseDeliveryNotes:
					dXMenuItem.Caption = MenuCaption.GoodReceiptPO;
					break;
				case ApzObjectTypes.oDeliveryNotes:
					dXMenuItem.Caption = MenuCaption.Delivery;
					break;
				case ApzObjectTypes.oInvoices:
					dXMenuItem.Caption = MenuCaption.ARInvoices;
					break;
				case ApzObjectTypes.oPurchaseInvoices:
					dXMenuItem.Caption = MenuCaption.APInvoices;
					break;
				case ApzObjectTypes.oDownPayments:
					dXMenuItem.Caption = MenuCaption.ARDownpaymentRequest;
					break;
				case ApzObjectTypes.oPurchaseDownPayments:
					dXMenuItem.Caption = MenuCaption.APDownpaymentRequest;
					break;
				case ApzObjectTypes.oInventoryGenExit:
					dXMenuItem.Caption = MenuCaption.GoodsIssue;
					break;
				case ApzObjectTypes.oReportCompletion:
					dXMenuItem.Caption = MenuCaption.ReportCompletion;
					break;
				}
				dXMenuItem.Click += (object s, EventArgs e) =>
				{
					WinCommons._OpenProcessing();
					try
					{
						DataTable dataTable = new DataTable();
						DataTable dataTable2 = new DataTable();
						dataTable2 = SetDataHeaderDocument();
						ContextActionMappingImpl[] mappingLines = MappingLine();
						ContextActionMappingImpl[] mappingDoc = MappingDoc();
						switch (pop)
						{
						case ApzObjectTypes.oPurchaseRequest:
						{
							dataTable = SetDataLineDocument(1470000113);
							frm_PurchaseRequest frm_PurchaseRequest2 = new frm_PurchaseRequest();
							frm_PurchaseRequest2.SetMapping(dataTable2, dataTable, mappingDoc, mappingLines, null, null);
							frm_PurchaseRequest2.MdiParent = MdiParent;
							frm_PurchaseRequest2.Show();
							break;
						}
						case ApzObjectTypes.oPurchaseOrders:
						{
							dataTable = SetDataLineDocument(22);
							frm_PurchaseOder frm_PurchaseOder2 = new frm_PurchaseOder();
							frm_PurchaseOder2.SetMapping(dataTable2, dataTable, mappingDoc, mappingLines, null, null);
							frm_PurchaseOder2.MdiParent = MdiParent;
							frm_PurchaseOder2.Show();
							break;
						}
						case ApzObjectTypes.oPurchaseQuotations:
						{
							dataTable = SetDataLineDocument(540000006);
							frm_PurchaseQuotation frm_PurchaseQuotation2 = new frm_PurchaseQuotation();
							frm_PurchaseQuotation2.SetMapping(dataTable2, dataTable, mappingDoc, mappingLines, null, null);
							frm_PurchaseQuotation2.MdiParent = MdiParent;
							frm_PurchaseQuotation2.Show();
							break;
						}
						case ApzObjectTypes.oPurchaseDeliveryNotes:
						{
							dataTable = SetDataLineDocument(20);
							frm_GoodReceiptPO frm_GoodReceiptPO2 = new frm_GoodReceiptPO();
							frm_GoodReceiptPO2.SetMapping(dataTable2, dataTable, mappingDoc, mappingLines, null, null);
							frm_GoodReceiptPO2.MdiParent = MdiParent;
							frm_GoodReceiptPO2.Show();
							break;
						}
						case ApzObjectTypes.oDeliveryNotes:
						{
							dataTable = SetDataLineDocument(15);
							frm_Delivery frm_Delivery2 = new frm_Delivery();
							frm_Delivery2.SetMapping(dataTable2, dataTable, mappingDoc, mappingLines, null, null);
							frm_Delivery2.MdiParent = MdiParent;
							frm_Delivery2.Show();
							break;
						}
						case ApzObjectTypes.oInvoices:
						{
							dataTable = SetDataLineDocument(13);
							frm_ARInvoices frm_ARInvoices2 = new frm_ARInvoices();
							frm_ARInvoices2.SetMapping(dataTable2, dataTable, mappingDoc, mappingLines, null, null);
							frm_ARInvoices2.MdiParent = MdiParent;
							frm_ARInvoices2.Show();
							break;
						}
						case ApzObjectTypes.oPurchaseInvoices:
						{
							dataTable = SetDataLineDocument(18);
							frm_APInvoices frm_APInvoices2 = new frm_APInvoices();
							frm_APInvoices2.SetMapping(dataTable2, dataTable, mappingDoc, mappingLines, null, null);
							frm_APInvoices2.MdiParent = MdiParent;
							frm_APInvoices2.Show();
							break;
						}
						case ApzObjectTypes.oDownPayments:
						{
							dataTable = SetDataLineDocument(203);
							frm_DownPaymentRequest frm_DownPaymentRequest2 = new frm_DownPaymentRequest();
							frm_DownPaymentRequest2.SetMapping(dataTable2, dataTable, mappingDoc, mappingLines, null, null);
							frm_DownPaymentRequest2.MdiParent = MdiParent;
							frm_DownPaymentRequest2.Show();
							break;
						}
						case ApzObjectTypes.oPurchaseDownPayments:
						{
							dataTable = SetDataLineDocument(204);
							frm_PurchaseDownPaymentRequest frm_PurchaseDownPaymentRequest2 = new frm_PurchaseDownPaymentRequest();
							frm_PurchaseDownPaymentRequest2.SetMapping(dataTable2, dataTable, mappingDoc, mappingLines, null, null);
							frm_PurchaseDownPaymentRequest2.MdiParent = MdiParent;
							frm_PurchaseDownPaymentRequest2.Show();
							break;
						}
						case ApzObjectTypes.oInventoryGenExit:
						{
							dataTable = SetDataLineDocument(60);
							frm_GoodsIssue frm_GoodsIssue2 = new frm_GoodsIssue();
							frm_GoodsIssue2.SetMapping(dataTable2, dataTable, mappingDoc, mappingLines, null, null);
							frm_GoodsIssue2.MdiParent = MdiParent;
							frm_GoodsIssue2.Show();
							break;
						}
						case ApzObjectTypes.oReportCompletion:
							SetDataReportCompletion();
							break;
						}
					}
					catch (Exception ex2)
					{
						Logging.Write("ERROR", ex2);
						WinCommons.ErrorLineNumber(ex2);
					}
					WinCommons._CloseProcessing();
				};
				dXPopupMenu.Items.Add(dXMenuItem);
			}
			drp_CrDocumnet.DropDownControl = dXPopupMenu;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void RefWbs(TreeListNode node, DataTable dtTask, int fatherId, string wbs, string isF)
	{
		try
		{
			if (node == null)
			{
				return;
			}
			int[] array = (from t in wbs.Split('.')
				select Function.ParseInt(t)).ToArray();
			int level = node.Level;
			Function.ParseInt(node.GetValue("LineNum"));
			if (isF == "Y" && fatherId > 0 && level > 1)
			{
				TreeListNode parent = tv_Wbs.FindNodeByKeyID(fatherId);
				TreeListNode treeListNode = tv_Wbs.FindNodeByKeyID(parent.GetValue("FatherNum"));
				List<DataRow> list = (from t in dtTask.AsEnumerable()
					where Function.ParseInt(t["FatherNum"]) == Function.ParseInt(parent.GetValue("FatherNum"))
					select t).ToList();
				list.Sort(new WbsComparer());
				if (list.Count > 0)
				{
					string text = Function.ToString(treeListNode.GetValue("Wbs"));
					for (int num = 0; num < list.Count; num++)
					{
						list[num]["Wbs"] = string.Join(".", text, num + 1);
						int num2 = Function.ParseInt(list[num]["LineNum"]);
						TreeListNode treeListNode2 = tv_Wbs.FindNodeByKeyID(num2);
						RefWbs(treeListNode2, dtTask, Function.ParseInt(treeListNode2.GetValue("LineNum")), string.Join(".", text, num + 1), "N");
					}
				}
			}
			List<DataRow> list2 = (from t in dtTask.AsEnumerable()
				where Function.ParseInt(t["FatherNum"]) == fatherId
				select t).ToList();
			list2.Sort(new WbsComparer());
			if (list2.Count <= 0)
			{
				return;
			}
			for (int num3 = 0; num3 < list2.Count; num3++)
			{
				if (fatherId > 0)
				{
					list2[num3]["Wbs"] = string.Join(".", wbs, num3 + 1);
				}
				else
				{
					list2[num3]["Wbs"] = string.Join(".", Function.ParseInt(array[level]) + num3);
				}
				int num4 = Function.ParseInt(list2[num3]["LineNum"]);
				TreeListNode node2 = tv_Wbs.FindNodeByKeyID(num4);
				RefWbs(node2, dtTask, num4, Function.ToString(list2[num3]["Wbs"]), "N");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void GetNodeChecked(TreeListNode node, ref List<TreeListNode> lst)
	{
		try
		{
			if (node != null)
			{
				if (node.Checked)
				{
					lst.Add(node);
				}
				for (int i = 0; i < node.Nodes.Count; i++)
				{
					GetNodeChecked(node.Nodes[i], ref lst);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void SetDataReportCompletion()
	{
		try
		{
			List<TreeListNode> lst = new List<TreeListNode>();
			if (tv_budget.Nodes.Count > 0)
			{
				for (int i = 0; i < tv_budget.Nodes.Count; i++)
				{
					GetNodeChecked(tv_budget.Nodes[i], ref lst);
				}
			}
			DataTable dataTable = tv_budget.DataSourceTable.Copy();
			DataTable dataTable2 = dataTable.Clone();
			foreach (TreeListNode node in lst)
			{
				DataRow dataRow = (from t in dataTable.AsEnumerable()
					where Function.ParseInt(t["LineNum"]) == Function.ParseInt(node["FatherNum"])
					select t).FirstOrDefault();
				DataRow dataRow2 = (from t in dataTable.AsEnumerable()
					where Function.ParseInt(t["LineNum"]) == Function.ParseInt(node["LineNum"])
					select t).FirstOrDefault();
				if (dataRow != null)
				{
					dataRow2["TaskCode"] = Function.ToString(dataRow["TaskCode"]);
					dataRow2["TaskName"] = Function.ToString(dataRow["TaskName"]);
				}
				dataTable2.ImportRow(dataRow2);
				dataTable2.AcceptChanges();
			}
			DataTable dt = WinFuntions.GetTableStructure("APZ_RTC1");
			if (dataTable2.Rows.Count > 0 && dataTable2 != null)
			{
				for (int num = 0; num < dataTable2.Rows.Count; num++)
				{
					DataRow dataRow3 = dt.NewRow();
					dataRow3["DocEntry"] = 0;
					dataRow3["LineNum"] = 0;
					dataRow3["TaskCode"] = Function.ToString(dataTable2.Rows[num]["TaskCode"]);
					dataRow3["TaskName"] = Function.ToString(dataTable2.Rows[num]["TaskName"]);
					dataRow3["ItemCode"] = Function.ToString(dataTable2.Rows[num]["ItemCode"]);
					dataRow3["ItemName"] = Function.ToString(dataTable2.Rows[num]["Dscription"]);
					dataRow3["Quantity"] = Function.ParseDouble(dataTable2.Rows[num]["PlanQty"]);
					dataRow3["RefPrice"] = Function.ParseDouble(dataTable2.Rows[num]["RefPrice"]);
					dataRow3["Total"] = Function.ParseDouble(dataTable2.Rows[num]["Amount"]);
					dataRow3["ActualQty"] = Function.ParseDouble(dataTable2.Rows[num]["ActualQty"]);
					dataRow3["ActualAmount"] = Function.ParseDouble(dataTable2.Rows[num]["ActualAmount"]);
					dataRow3["OpenQty"] = Function.ParseDouble(dataTable2.Rows[num]["ActualQty"]);
					dataRow3["UoM"] = Function.ToString(dataTable2.Rows[num]["UoM"]);
					dataRow3["PostingDate"] = DateTime.Now.Date;
					dataRow3["BaseLine"] = Function.ParseInt(dataTable2.Rows[num]["LineNum"]);
					dataRow3["BaseEntry"] = Function.ParseInt(dataTable2.Rows[num]["AbsEntry"]);
					dataRow3["Currency"] = Function.ToString(dataTable2.Rows[num]["Currency"]);
					dataRow3["LineStatus"] = "O";
					dataRow3["Remarks"] = "";
					dt.Rows.Add(dataRow3);
					dt.AcceptChanges();
				}
				int projectId = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("AbsEntry"));
				string docCurr = Function.ToString(gv_Doc.GetFocusedRowCellValue("DocCur"));
				double total = dataTable2.AsEnumerable().Sum((DataRow t) => Function.ParseDouble(t["Amount"]));
				double totalActual = dataTable2.AsEnumerable().Sum((DataRow t) => Function.ParseDouble(t["ActualAmount"]));
				DataTable dtDoc = WinFuntions.GetTableStructure("APZ_ORTC");
				DataRow rowDoc = dtDoc.NewRow();
				DataRow dataRow4 = rowDoc;
				dataRow4["DocEntry"] = await SetDataNo();
				rowDoc["ProjectId"] = projectId;
				rowDoc["Status"] = "P";
				rowDoc["PostingDate"] = DateTime.Now.Date;
				rowDoc["Total"] = total;
				rowDoc["TotalActual"] = totalActual;
				rowDoc["Comment"] = "";
				rowDoc["DocCur"] = docCurr;
				dtDoc.Rows.Add(rowDoc);
				dtDoc.AcceptChanges();
				frm_ProjectReportCompletion frm_ProjectReportCompletion2 = new frm_ProjectReportCompletion(dt, dtDoc);
				frm_ProjectReportCompletion2.MdiParent = MdiParent;
				frm_ProjectReportCompletion2.Show();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.ItemCodeCannotBeNull, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<int> SetDataNo()
	{
		try
		{
			return await _httpClient.ReportCompletion.GetNo();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return 0;
		}
	}

	private DataTable SetDataLineDocument(int type)
	{
		try
		{
			List<TreeListNode> lst = new List<TreeListNode>();
			if (tv_budget.Nodes.Count > 0)
			{
				for (int i = 0; i < tv_budget.Nodes.Count; i++)
				{
					GetNodeChecked(tv_budget.Nodes[i], ref lst);
				}
			}
			DataTable dataTable = tv_budget.DataSourceTable.Copy();
			DataTable dataTable2 = dataTable.Clone();
			foreach (TreeListNode node in lst)
			{
				DataRow row = (from t in dataTable.AsEnumerable()
					where Function.ParseInt(t["LineNum"]) == Function.ParseInt(node["LineNum"])
					select t).FirstOrDefault();
				dataTable2.ImportRow(row);
				dataTable2.AcceptChanges();
			}
			DataTable dataTable3 = new DataTable();
			dataTable3.Columns.Add("DocEntry", typeof(int));
			dataTable3.Columns.Add("LineNum", typeof(int));
			dataTable3.Columns.Add("TargetType", typeof(int));
			dataTable3.Columns.Add("ItemCode", typeof(string));
			dataTable3.Columns.Add("Dscription", typeof(string));
			dataTable3.Columns.Add("Quantity", typeof(double));
			dataTable3.Columns.Add("Currency", typeof(string));
			dataTable3.Columns.Add("PriceBefDi", typeof(double));
			dataTable3.Columns.Add("Project", typeof(string));
			dataTable3.Columns.Add("U_OriginType", typeof(string));
			dataTable3.Columns.Add("U_OriginCode", typeof(int));
			dataTable3.Columns.Add("U_OriginLine", typeof(int));
			dataTable3.Columns.Add("WhsCode", typeof(string));
			dataTable3.Columns.Add("OcrCode", typeof(string));
			dataTable3.Columns.Add("OcrCode2", typeof(string));
			dataTable3.Columns.Add("OcrCode3", typeof(string));
			dataTable3.Columns.Add("OcrCode4", typeof(string));
			dataTable3.Columns.Add("OcrCode5", typeof(string));
			if (dataTable2.Rows.Count > 0 && dataTable2 != null)
			{
				for (int num = 0; num < dataTable2.Rows.Count; num++)
				{
					DataRow dataRow = dataTable3.NewRow();
					dataRow["DocEntry"] = 0;
					dataRow["LineNum"] = 0;
					dataRow["TargetType"] = -1;
					dataRow["ItemCode"] = Function.ToString(dataTable2.Rows[num]["ItemCode"]);
					dataRow["Dscription"] = Function.ToString(dataTable2.Rows[num]["Dscription"]);
					switch (type)
					{
					case 1470000113:
						dataRow["Quantity"] = ((Function.ParseDouble(dataTable2.Rows[num]["PlanQty"]) - Function.ParseDouble(dataTable2.Rows[num]["PR"]) > 0.0) ? (Function.ParseDouble(dataTable2.Rows[num]["PlanQty"]) - Function.ParseDouble(dataTable2.Rows[num]["PR"])) : 0.0);
						break;
					case 22:
						dataRow["Quantity"] = ((Function.ParseDouble(dataTable2.Rows[num]["PlanQty"]) - Function.ParseDouble(dataTable2.Rows[num]["PO"]) > 0.0) ? (Function.ParseDouble(dataTable2.Rows[num]["PlanQty"]) - Function.ParseDouble(dataTable2.Rows[num]["PO"])) : 0.0);
						break;
					default:
						dataRow["Quantity"] = Function.ParseDouble(dataTable2.Rows[num]["PlanQty"]);
						break;
					}
					dataRow["Currency"] = Function.ToString(dataTable2.Rows[num]["Currency"]);
					dataRow["PriceBefDi"] = Function.ParseDouble(dataTable2.Rows[num]["RefPrice"]);
					dataRow["Project"] = Function.ToString(gv_Doc.GetFocusedRowCellValue("FinProject"));
					dataRow["U_OriginType"] = "1470000132";
					dataRow["U_OriginCode"] = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("AbsEntry"));
					dataRow["U_OriginLine"] = Function.ParseInt(dataTable2.Rows[num]["LineNum"]);
					dataRow["WhsCode"] = Function.ToString(gv_Doc.GetFocusedRowCellValue("DefaultWhs"));
					dataRow["OcrCode"] = Function.ToString(dataTable2.Rows[num]["OcrCode"]);
					dataRow["OcrCode2"] = Function.ToString(dataTable2.Rows[num]["OcrCode2"]);
					dataRow["OcrCode3"] = Function.ToString(dataTable2.Rows[num]["OcrCode3"]);
					dataRow["OcrCode4"] = Function.ToString(dataTable2.Rows[num]["OcrCode4"]);
					dataRow["OcrCode5"] = Function.ToString(dataTable2.Rows[num]["OcrCode5"]);
					dataTable3.Rows.Add(dataRow);
					dataTable3.AcceptChanges();
				}
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.ItemCodeCannotBeNull, "");
			}
			return dataTable3;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	private DataTable SetDataHeaderDocument()
	{
		try
		{
			DataTable dataTable = new DataTable
			{
				Columns = 
				{
					{
						"Project",
						typeof(string)
					},
					{
						"DocCur",
						typeof(string)
					}
				}
			};
			DataRow dataRow = dataTable.NewRow();
			dataRow["Project"] = Function.ToString(gv_Doc.GetFocusedRowCellValue("FinProject"));
			dataRow["DocCur"] = Function.ToString(gv_Doc.GetFocusedRowCellValue("DocCur"));
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	private ContextActionMappingImpl[] MappingLine()
	{
		try
		{
			return new ContextActionMappingImpl[15]
			{
				new ContextActionMappingImpl
				{
					TargetField = "ItemCode",
					SourceField = "ItemCode"
				},
				new ContextActionMappingImpl
				{
					TargetField = "Dscription",
					SourceField = "Dscription"
				},
				new ContextActionMappingImpl
				{
					TargetField = "Project",
					SourceField = "Project"
				},
				new ContextActionMappingImpl
				{
					TargetField = "WhsCode",
					SourceField = "WhsCode"
				},
				new ContextActionMappingImpl
				{
					TargetField = "U_OriginType",
					SourceField = "U_OriginType"
				},
				new ContextActionMappingImpl
				{
					TargetField = "U_OriginCode",
					SourceField = "U_OriginCode"
				},
				new ContextActionMappingImpl
				{
					TargetField = "Quantity",
					SourceField = "Quantity"
				},
				new ContextActionMappingImpl
				{
					TargetField = "Currency",
					SourceField = "Currency"
				},
				new ContextActionMappingImpl
				{
					TargetField = "PriceBefDi",
					SourceField = "PriceBefDi"
				},
				new ContextActionMappingImpl
				{
					TargetField = "U_OriginLine",
					SourceField = "U_OriginLine"
				},
				new ContextActionMappingImpl
				{
					TargetField = "OcrCode",
					SourceField = "OcrCode"
				},
				new ContextActionMappingImpl
				{
					TargetField = "OcrCode2",
					SourceField = "OcrCode2"
				},
				new ContextActionMappingImpl
				{
					TargetField = "OcrCode3",
					SourceField = "OcrCode3"
				},
				new ContextActionMappingImpl
				{
					TargetField = "OcrCode4",
					SourceField = "OcrCode4"
				},
				new ContextActionMappingImpl
				{
					TargetField = "OcrCode5",
					SourceField = "OcrCode5"
				}
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
			return new ContextActionMappingImpl[0];
		}
	}

	private ContextActionMappingImpl[] MappingDoc()
	{
		try
		{
			return new ContextActionMappingImpl[2]
			{
				new ContextActionMappingImpl
				{
					TargetField = "Project",
					SourceField = "Project"
				},
				new ContextActionMappingImpl
				{
					TargetField = "DocCur",
					SourceField = "DocCur"
				}
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
			return new ContextActionMappingImpl[0];
		}
	}

	private async Task<DataTable> LoadDataOrcCode(int p)
	{
		try
		{
			DataTable result = new DataTable();
			using (HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Sales"))
			{
				result = await helper.GetDataTableAsync("GetListOrcCode", new
				{
					p = p.ToString()
				});
			}
			return result;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return new DataTable();
		}
	}

	private void treeList1_DragDrop(object sender, DragEventArgs e)
	{
		try
		{
			_isinout = true;
			if (sender is TreeList treeList)
			{
				Point pt = treeList.PointToClient(new Point(e.X, e.Y));
				TreeListNode treeListNode = e.Data.GetData(typeof(TreeListNode)) as TreeListNode;
				TreeListNode node = treeList.CalcHitInfo(pt).Node;
				if (node != null && treeListNode != null)
				{
					treeListNode.SetValue("FatherNum", node.GetValue("FatherNum"));
					tv_Wbs.DataSourceTable.AcceptChanges();
					string text = Function.ToString(node.GetValue("Wbs"));
					int[] array = (from t in text.Split('.')
						select Function.ParseInt(t)).ToArray();
					if (node.Level < array.Length)
					{
						array[node.Level]--;
						treeListNode.SetValue("Wbs", string.Join(".", array));
					}
					int linenum = Function.ParseInt(treeListNode.GetValue("LineNum"));
					string wbs = "";
					string text2 = Function.ToString(treeListNode.GetValue("Wbs"));
					wbs = ((Function.CompareString(text, text2) > 0) ? text2 : text);
					DataRow dataRow = tv_Wbs.DataSourceTable.AsEnumerable().LastOrDefault((DataRow t) => (Function.ParseInt(t["FatherNum"]) == 0 || Function.ParseInt(t["FatherNum"]) == -1) && Function.ParseInt(t["LineNum"]) != linenum && GetWbsByLevel(Function.ToString(t["Wbs"]), 0) <= GetWbsByLevel(wbs, 0));
					if (dataRow != null)
					{
						RefWbs(tv_Wbs.FindNodeByKeyID(Function.ParseInt(dataRow["LineNum"])), tv_Wbs.DataSourceTable, Function.ParseInt(dataRow["LineNum"]), wbs, "Y");
					}
					RefreshVisOrder();
					tv_Wbs.DataSourceTable.AcceptChanges();
				}
			}
			e.Effect = DragDropEffects.None;
			SetDefaultCursor();
			_isinout = false;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void treeList1_DragLeave(object sender, EventArgs e)
	{
		SetDefaultCursor();
	}

	private void treeList1_DragOver(object sender, DragEventArgs e)
	{
		try
		{
			if (!(e.Data.GetData(typeof(IDragNodesProvider)) is IDragNodesProvider provider))
			{
				e.Effect = DragDropEffects.None;
			}
			else
			{
				DXDragEventArgs dXDragEventArgs = e.GetDXDragEventArgs((TreeList)sender);
				e.Effect = GetDragDropEffect(dXDragEventArgs, provider);
			}
			SetDragCursor(e.Effect);
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void SetDragCursor(DragDropEffects e)
	{
		try
		{
			if (e == DragDropEffects.Move)
			{
				Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Apzon.Forms.Resources.move.ico");
				if (manifestResourceStream != null)
				{
					Cursor = new Cursor(manifestResourceStream);
				}
			}
			if (e == DragDropEffects.Copy)
			{
				Stream manifestResourceStream2 = Assembly.GetExecutingAssembly().GetManifestResourceStream("Apzon.Forms.Resources.copy.ico");
				if (manifestResourceStream2 != null)
				{
					Cursor = new Cursor(manifestResourceStream2);
				}
			}
			if (e == DragDropEffects.None)
			{
				Cursor = Cursors.No;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void SetDefaultCursor()
	{
		Cursor = Cursors.Default;
	}

	private DragDropEffects GetDragDropEffect(DXDragEventArgs e, IDragNodesProvider provider)
	{
		if (!AllowDropNodes(e, provider))
		{
			return DragDropEffects.None;
		}
		return e.Effect;
	}

	private void treeList1_CalcNodeDragImageIndex(object sender, CalcNodeDragImageIndexEventArgs e)
	{
		try
		{
			DXDragEventArgs dXDragEventArgs = e.DragArgs.GetDXDragEventArgs(sender as TreeList);
			if (dXDragEventArgs.Data.GetData(typeof(IDragNodesProvider)) is IDragNodesProvider provider && !AllowDropNodes(dXDragEventArgs, provider))
			{
				e.ImageIndex = -1;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private bool AllowDropNodes(DXDragEventArgs e, IDragNodesProvider provider)
	{
		try
		{
			if (e.HitInfo.HitTest.RowInfo == null)
			{
				return true;
			}
			TreeListNode node = e.HitInfo.HitTest.RowInfo.Node;
			if (node == null)
			{
				return true;
			}
			if (e.DragInsertPosition == DragInsertPosition.None)
			{
				return true;
			}
			if (e.DragInsertPosition != DragInsertPosition.AsChild)
			{
				_ = node.Level;
			}
			else
			{
				_ = node.Level;
			}
			foreach (TreeListNode dragNode in provider.DragNodes)
			{
				if (dragNode.GetValue("LineNum") == node.GetValue("LineNum"))
				{
					return false;
				}
			}
			return true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return false;
	}

	private TreeListNode HasAsParent(TreeListNode node1, TreeListNode node2)
	{
		try
		{
			if (node1.Level > node2.Level)
			{
				return node1.HasAsParent(node2) ? node1 : null;
			}
			if (node1.Level < node2.Level)
			{
				return node2.HasAsParent(node1) ? node2 : null;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		return null;
	}

	private void viewDiagramToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			frm_Diagram frm_Diagram2 = new frm_Diagram();
			frm_Diagram2.ObjectType = ApzObjectTypes.oProjectManager;
			frm_Diagram2.ObjectId = Function.ToString(gv_Doc.GetFocusedRowCellValue("AbsEntry"));
			frm_Diagram2.MdiParent = MdiParent;
			frm_Diagram2.Show();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void tv_Wbs_GiveFeedback(object sender, GiveFeedbackEventArgs e)
	{
		e.UseDefaultCursors = false;
	}

	private void tv_Wbs_CustomizeNewNodeFromOuterData(object sender, CustomizeNewNodeFromOuterDataEventArgs e)
	{
		try
		{
			if (sender is TreeList treeList)
			{
				e.NewData[treeList.ParentFieldName] = ((e.DestinationNode == null) ? ((object)0) : e.DestinationNode[treeList.KeyFieldName]);
				e.NewData[treeList.KeyFieldName] = treeList.AllNodesCount;
				e.NewData[treeList.ImageIndexFieldName] = e.SourceNode[treeList.ImageIndexFieldName];
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void tv_Wbs_BeforeDropNode(object sender, BeforeDropNodeEventArgs e)
	{
		try
		{
			if (!e.IsCopy)
			{
				TreeListNodes nodes = ((e.DestinationNode == null) ? ((TreeList)sender).Nodes : e.DestinationNode.Nodes);
				e.Cancel = AllowDropNode(e.SourceNode, nodes);
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private bool AllowDropNode(TreeListNode sourceNode, TreeListNodes nodes)
	{
		try
		{
			foreach (TreeListNode node in nodes)
			{
				object value = sourceNode.GetValue("LineNum");
				object value2 = node.GetValue("LineNum");
				if (object.Equals(value, value2))
				{
					return true;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return false;
	}

	private void tv_Wbs_BeforeDragNode(object sender, BeforeDragNodeEventArgs e)
	{
		try
		{
			if (e.Nodes.Count <= 1)
			{
				return;
			}
			int num = 0;
			int num2 = num + 1;
			List<TreeListNode> list = new List<TreeListNode>();
			while (num != e.Nodes.Count - 1)
			{
				TreeListNode treeListNode = HasAsParent(e.Nodes[num], e.Nodes[num2]);
				if (treeListNode != null)
				{
					list.Add(treeListNode);
				}
				if (num2 == e.Nodes.Count - 1)
				{
					num++;
					num2 = num + 1;
				}
				else
				{
					num2++;
				}
			}
			if (list.Count == 0)
			{
				return;
			}
			foreach (TreeListNode item in list)
			{
				e.Nodes.Remove(item);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_MouseMove(object sender, MouseEventArgs e)
	{
		try
		{
			GridHitInfo gridHitInfo = (sender as GridViewEx).CalcHitInfo(new Point(e.X, e.Y));
			if (gridHitInfo.InRowCell)
			{
				HotTrackRow = gridHitInfo.RowHandle;
			}
			else
			{
				HotTrackRow = int.MinValue;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void tv_budget_AfterCheckNode(object sender, NodeEventArgs e)
	{
		TreeListNode focusedNode = tv_budget.FocusedNode;
		if (string.IsNullOrEmpty(Function.ToString(tv_budget.GetRowCellValue(e.Node, tb_AbsEntry))))
		{
			WinCommons.Warning(Enums.MessageCode.YouNeedToSaveRowInfomationBeforeCreatingTheDocument, "", Function.ToString(tv_budget.GetRowCellValue(e.Node, tb_ItemCode)));
			focusedNode.CheckState = CheckState.Unchecked;
		}
	}

	private void gv_Doc_MouseLeave(object sender, EventArgs e)
	{
		HotTrackRow = int.MinValue;
	}

	private void tv_Wbs_CellValueChanging(object sender, DevExpress.XtraTreeList.CellValueChangedEventArgs e)
	{
	}

	private async Task InitCalendar()
	{
		try
		{
			chk_Action.BackColor = BackColor;
			chk_Action.BorderStyle = BorderStyles.NoBorder;
			chk_Users.BackColor = BackColor;
			chk_Users.BorderStyle = BorderStyles.NoBorder;
			ArrayList dataSource = new ArrayList();
			schedulerControl1.BeginUpdate();
			double value = 8.0;
			double value2 = 17.0;
			int resourcesPerPage = 3;
			int argb = -4980813;
			int argb2 = -14669;
			int argb3 = -77;
			int argb4 = -1;
			int argb5 = -7156260;
			int argb6 = -3883625;
			schedulerControl1.GroupType = SchedulerGroupType.Resource;
			schedulerControl1.ActiveViewType = SchedulerViewType.Gantt;
			schedulerControl1.OptionsView.ToolTipVisibility = ToolTipVisibility.Never;
			chk_Action.DataSource = dataSource;
			schedulerControl1.DayView.ResourcesPerPage = resourcesPerPage;
			schedulerControl1.WeekView.ResourcesPerPage = resourcesPerPage;
			schedulerControl1.WorkWeekView.ResourcesPerPage = resourcesPerPage;
			schedulerControl1.FullWeekView.ResourcesPerPage = resourcesPerPage;
			schedulerControl1.MonthView.ResourcesPerPage = resourcesPerPage;
			schedulerControl1.TimelineView.ResourcesPerPage = resourcesPerPage;
			schedulerControl1.GanttView.ResourcesPerPage = resourcesPerPage;
			schedulerControl1.WorkWeekView.WorkTime = new TimeOfDayInterval(TimeSpan.FromHours(value), TimeSpan.FromHours(value2));
			schedulerControl1.DayView.WorkTime = new TimeOfDayInterval(TimeSpan.FromHours(value), TimeSpan.FromHours(value2));
			schedulerControl1.FullWeekView.WorkTime = new TimeOfDayInterval(TimeSpan.FromHours(value), TimeSpan.FromHours(value2));
			schedulerControl1.TimelineView.WorkTime = new TimeOfDayInterval(TimeSpan.FromHours(value), TimeSpan.FromHours(value2));
			schedulerControl1.GanttView.WorkTime = new TimeOfDayInterval(TimeSpan.FromHours(value), TimeSpan.FromHours(value2));
			schedulerControl1.WorkDays.BeginUpdate();
			if (Function.ToString("N") == "Y")
			{
				schedulerControl1.WorkDays.Add(WeekDays.Saturday);
			}
			else
			{
				schedulerControl1.WorkDays.Remove(new WeekDaysWorkDay(WeekDays.Saturday));
			}
			schedulerControl1.WorkDays.EndUpdate();
			AppointmentLabelCollection labels = schedulerStorage1.Appointments.Labels;
			if (labels.Count > 0)
			{
				labels.BeginUpdate();
				AppointmentLabel byId = labels.GetById("T");
				if (byId != null)
				{
					byId.Color = Color.FromArgb(argb);
				}
				AppointmentLabel byId2 = labels.GetById("V");
				if (byId2 != null)
				{
					byId2.Color = Color.FromArgb(argb2);
				}
				AppointmentLabel byId3 = labels.GetById("E");
				if (byId3 != null)
				{
					byId3.Color = Color.FromArgb(argb3);
				}
				labels.EndUpdate();
			}
			AppointmentStatusCollection statuses = schedulerStorage1.Appointments.Statuses;
			if (statuses.Count > 0)
			{
				statuses.BeginUpdate();
				AppointmentStatus byId4 = statuses.GetById(0);
				if (byId4 != null)
				{
					byId4.Brush = new SolidBrush(Color.FromArgb(argb4));
				}
				AppointmentStatus byId5 = statuses.GetById(1);
				if (byId5 != null)
				{
					byId5.Brush = new SolidBrush(Color.FromArgb(argb6));
				}
				AppointmentStatus byId6 = statuses.GetById(2);
				if (byId6 != null)
				{
					byId6.Brush = new SolidBrush(Color.FromArgb(argb5));
				}
				statuses.EndUpdate();
			}
			schedulerStorage1.Appointments.CustomFieldMappings.Clear();
			AppointmentMappingInfo mappings = schedulerStorage1.Appointments.Mappings;
			mappings.AppointmentId = "LineNum";
			mappings.Start = "FromDate";
			mappings.End = "ToDate";
			mappings.Subject = "TaskName";
			mappings.ResourceId = "ResourceIdsInfo";
			mappings.Description = "Notes";
			mappings.AllDay = null;
			ResourceMappingInfo mappings2 = schedulerStorage1.Resources.Mappings;
			mappings2.Caption = "EmployeeName";
			mappings2.Id = "Resource";
			schedulerStorage1.FilterResource += schedulerStorage1_FilterResource;
			schedulerControl1.EndUpdate();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadCalendarData()
	{
		_ = 1;
		try
		{
			schedulerControl1.BeginUpdate();
			await InitResources();
			await InitAppointments();
			schedulerControl1.EndUpdate();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task InitResources()
	{
		try
		{
			if (grd_LineTeam.DataSource is DataTable dataTable)
			{
				schedulerStorage1.BeginUpdate();
				schedulerStorage1.Resources.DataSource = dataTable.Copy();
				schedulerStorage1.EndUpdate();
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task InitAppointments()
	{
		try
		{
			WinCommons._OpenProcessing();
			DataTable dataTable = tv_Wbs.DataSourceTable().Copy();
			dataTable.Columns.Add("ResourceIdsInfo", typeof(string));
			SchedulerCompatibility.Base64XmlObjectSerialization = false;
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				DataRow dataRow = dataTable.Rows[i];
				ResourceIdCollection resourceIdCollection = new ResourceIdCollection();
				string text = Function.ToString(dataRow["Resource"]);
				if (!string.IsNullOrEmpty(text))
				{
					string[] array = Function.ToString(text).Split(';');
					foreach (string text2 in array)
					{
						if (!string.IsNullOrEmpty(text2))
						{
							resourceIdCollection.Add(Function.ParseInt(text2));
						}
					}
				}
				AppointmentResourceIdCollectionXmlPersistenceHelper appointmentResourceIdCollectionXmlPersistenceHelper = new AppointmentResourceIdCollectionXmlPersistenceHelper(resourceIdCollection);
				dataRow["ResourceIdsInfo"] = appointmentResourceIdCollectionXmlPersistenceHelper.ToXml();
				if (!string.IsNullOrEmpty(Function.ToString(dataRow["ToDate"])))
				{
					dataRow["ToDate"] = Function.ParseDateTimes(dataRow["ToDate"]).Date.Add(new TimeSpan(0, 23, 59));
				}
			}
			if (_dtDepen != null)
			{
				schedulerStorage1.BeginUpdate();
				DataTable dataTable2 = _dtDepen.Copy();
				dataTable2.Columns.Add("DepenType", typeof(int));
				for (int k = 0; k < dataTable2.Rows.Count; k++)
				{
					dataTable2.Rows[k]["DepenType"] = 10;
				}
				schedulerStorage1.Appointments.DataSource = dataTable;
				AppointmentDependencyMappingInfo mappings = schedulerStorage1.AppointmentDependencies.Mappings;
				mappings.ParentId = "Dependent";
				mappings.DependentId = "LineNum";
				mappings.Type = "DepenType";
				schedulerStorage1.AppointmentDependencies.DataSource = dataTable2;
				schedulerStorage1.EndUpdate();
			}
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private async Task ReloadNoteList()
	{
		try
		{
			DataTable dataSource = await _httpClient.SubActivities.LoadNote(GlobalData.UserCodeLogin, Function.ParseInt(gv_Doc.GetFocusedRowCellValue("AbsEntry")));
			grd_NoteList.DataSource = dataSource;
			grd_NoteList.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task ReloadNoteList(int clgCode)
	{
		try
		{
			DataTable dataSource = await _httpClient.SubActivities.LoadNote(GlobalData.UserCodeLogin, Function.ParseInt(gv_Doc.GetFocusedRowCellValue("AbsEntry")));
			grd_NoteList.DataSource = dataSource;
			grd_NoteList.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task ReloadEventList()
	{
		try
		{
			DataTable dataSource = await _httpClient.SubActivities.LoadEventByObjectType(GlobalData.UserCodeLogin, ApzObjectTypes.oEventManagement, ApzObjectTypes.oProjectManager, Function.ToString(gv_Doc.GetFocusedRowCellValue("AbsEntry")));
			grd_EventList.DataSource = dataSource;
			grd_EventList.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task ReloadEventList(int clgCode)
	{
		try
		{
			DataTable dataSource = await _httpClient.SubActivities.LoadEventByObjectType(GlobalData.UserCodeLogin, ApzObjectTypes.oEventManagement, ApzObjectTypes.oProjectManager, Function.ToString(gv_Doc.GetFocusedRowCellValue("AbsEntry")));
			grd_EventList.DataSource = dataSource;
			grd_EventList.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_GanttView_Click(object sender, EventArgs e)
	{
		try
		{
			if (xtra_Line.SelectedTabPage == tab_TaskList && new frm_GanttControl
			{
				ProjectAbsEntry = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("AbsEntry"))
			}.ShowDialog(this) == DialogResult.OK)
			{
				await ReloadTaskList();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_ItemCode_KeyDown(object sender, KeyEventArgs e)
	{
		KeysBudgetItem(sender, e);
	}

	private void gv_NoteList_DoubleClick(object sender, EventArgs e)
	{
		try
		{
			if (gv_NoteList.FocusedRowHandle >= 0)
			{
				if (Function.ParseInt(gv_NoteList.GetFocusedRowCellValue("ClgCode")) > 0)
				{
					DataTable tableStructure = WinFuntions.GetTableStructure("APZ_OCLG");
					tableStructure.ImportRow(gv_NoteList.GetFocusedDataRow());
					DataRow dataRow = tableStructure.Rows[0];
					frm_NoteManagement frm_NoteManagement2 = new frm_NoteManagement
					{
						ClgCode = Function.ParseInt(dataRow["ClgCode"]),
						CallFromCal = true,
						MdiParent = MdiParent,
						DataRowFromCal = dataRow
					};
					frm_NoteManagement2.ReloadAfterRemove = (Func<int, Task>)Delegate.Combine(frm_NoteManagement2.ReloadAfterRemove, new Func<int, Task>(ReloadNoteList));
					frm_NoteManagement2.ReloadCalendar = (Func<Task>)Delegate.Combine(frm_NoteManagement2.ReloadCalendar, new Func<Task>(ReloadNoteList));
					frm_NoteManagement2.Show();
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.NoMatchingRecordFound, "");
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_NoteList_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
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
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_EventList_DoubleClick(object sender, EventArgs e)
	{
		try
		{
			if (gv_EventList.FocusedRowHandle >= 0)
			{
				if (Function.ParseInt(gv_EventList.GetFocusedRowCellValue("ClgCode")) > 0)
				{
					DataTable tableStructure = WinFuntions.GetTableStructure("APZ_OCLG");
					tableStructure.ImportRow(gv_EventList.GetFocusedDataRow());
					DataRow dataRow = tableStructure.Rows[0];
					frm_EventManagement frm_EventManagement2 = new frm_EventManagement
					{
						ClgCode = Function.ParseInt(dataRow["ClgCode"]),
						MdiParent = MdiParent,
						CallFromCal = true,
						DataRowFromCal = dataRow
					};
					frm_EventManagement2.ReloadAfterRemove = (Func<int, Task>)Delegate.Combine(frm_EventManagement2.ReloadAfterRemove, new Func<int, Task>(ReloadEventList));
					frm_EventManagement2.ReloadCalendar = (Func<Task>)Delegate.Combine(frm_EventManagement2.ReloadCalendar, new Func<Task>(ReloadEventList));
					frm_EventManagement2.Show();
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.NoMatchingRecordFound, "");
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_EventList_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
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
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_NoteList_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == "AssignTo")
			{
				string value = Function.ToString(gv_NoteList.GetRowCellValue(e.RowHandle, "GroupUser"));
				string value2 = Function.ToString(gv_NoteList.GetRowCellValue(e.RowHandle, "GroupEmployee"));
				if (string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(value2))
				{
					e.Appearance.ForeColor = Color.FromArgb(125, 88, 0);
				}
			}
			string a = Function.ToString(gv_Doc.GetFocusedRowCellValue("PrgStatus"));
			if (string.Equals(a, "C") || string.Equals(a, "L"))
			{
				e.Appearance.BackColor = GridColor.GridReadOnly;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_EventList_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == "AssignTo")
			{
				string value = Function.ToString(gv_EventList.GetRowCellValue(e.RowHandle, "GroupUser"));
				string value2 = Function.ToString(gv_EventList.GetRowCellValue(e.RowHandle, "GroupEmployee"));
				if (string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(value2))
				{
					e.Appearance.ForeColor = Color.FromArgb(125, 88, 0);
				}
			}
			string a = Function.ToString(gv_Doc.GetFocusedRowCellValue("PrgStatus"));
			if (string.Equals(a, "C") || string.Equals(a, "L"))
			{
				e.Appearance.BackColor = GridColor.GridReadOnly;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void tv_ListCompleted_NodeCellStyle(object sender, GetCustomNodeCellStyleEventArgs e)
	{
		try
		{
			try
			{
				if (e.Column.FieldName == "AssignTo")
				{
					string value = Function.ToString(tv_ListCompleted.DataSourceTable().Rows[e.Node.Id]["GroupUser"]);
					string value2 = Function.ToString(tv_ListCompleted.DataSourceTable().Rows[e.Node.Id]["GroupEmployee"]);
					if (string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(value2))
					{
						e.Appearance.ForeColor = Color.FromArgb(125, 88, 0);
					}
				}
				string a = Function.ToString(gv_Doc.GetFocusedRowCellValue("PrgStatus"));
				if (string.Equals(a, "C") || string.Equals(a, "L"))
				{
					e.Appearance.BackColor = GridColor.GridReadOnly;
				}
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", ex);
				WinCommons.ErrorLineNumber(ex);
			}
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", ex2);
			WinCommons.ErrorLineNumber(ex2);
		}
	}

	private void tv_ListCompleted_CustomNodeCellEdit(object sender, GetCustomNodeCellEditEventArgs e)
	{
		try
		{
			string fieldName = e.Column.FieldName;
			if (!(fieldName == "Closed"))
			{
				if (fieldName == "Priority")
				{
					_ = tv_ListCompleted.FormatRules.IsLoading;
				}
				return;
			}
			string text = Function.ToString(tv_ListCompleted.GetRowCellValue(tv_ListCompleted.GetNodeByVisibleIndex(e.Node.Id), tl_Closed_Completed));
			if (text == "Y")
			{
				e.RepositoryItem = rep_btn_Closed;
			}
			else if (text == "N")
			{
				e.RepositoryItem = rep_btn_Open;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void tv_ListCompleted_CustomColumnDisplayText(object sender, DevExpress.XtraTreeList.CustomColumnDisplayTextEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == "endDate")
			{
				e.DisplayText = string.Format("{0:" + GlobalData.DateTimeFormat + "} ", e.Value);
			}
			else if (e.Column.FieldName == "CntctDate")
			{
				e.DisplayText = string.Format("{0:" + GlobalData.DateTimeFormat + "} ", e.Value);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void tv_ListCompleted_DoubleClick(object sender, EventArgs e)
	{
		try
		{
			if (tv_ListCompleted.FocusedNode.Id >= 0)
			{
				if (Function.ParseInt(tv_ListCompleted.GetRowCellValue(tv_ListCompleted.FocusedNode, tl_ClgCode)) > 0)
				{
					DataTable tableStructure = WinFuntions.GetTableStructure("APZ_OCLG");
					tableStructure.ImportRow(tv_ListCompleted.GetFocusedDataRow());
					DataRow dataRow = tableStructure.Rows[0];
					frm_TaskManagement frm_TaskManagement2 = new frm_TaskManagement
					{
						ClgCode = Function.ParseInt(dataRow["ClgCode"]),
						CallFromCal = true,
						MdiParent = MdiParent,
						DataRowFromCal = dataRow
					};
					frm_TaskManagement2.ReloadCalendar = (Func<Task>)Delegate.Combine(frm_TaskManagement2.ReloadCalendar, new Func<Task>(ReloadTaskList));
					frm_TaskManagement2.ReloadAfterRemove = (Func<int, Task>)Delegate.Combine(frm_TaskManagement2.ReloadAfterRemove, new Func<int, Task>(ReloadTaskList));
					frm_TaskManagement2.Show();
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.NoMatchingRecordFound, "");
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void tv_ListCompleted_CustomUnboundColumnData(object sender, TreeListCustomColumnDataEventArgs e)
	{
		try
		{
			if (!(sender is TreeEx treeEx) || e == null || e.Column.FieldName != "AssignTo")
			{
				return;
			}
			int id = e.Node.Id;
			string text = Function.ToString(treeEx.DataSourceTable.Rows[id]["GroupUser"]);
			string text2 = Function.ToString(treeEx.DataSourceTable.Rows[id]["GroupEmployee"]);
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
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void tv_ListCompleted_CustomDrawNodeIndicator(object sender, CustomDrawNodeIndicatorEventArgs e)
	{
		(e.ObjectArgs as IndicatorObjectInfoArgs).DisplayText = Function.ToString(Function.ParseInt(e.Node.GetValue("RowId")));
	}

	private async Task SetDataTeamByResource(string resource)
	{
		_ = 1;
		try
		{
			DataTable dtTask = tv_Wbs.DataSourceTable();
			int entry = Function.ParseInt(tv_Wbs.GetFocusedRowCellValue(tl_Entry));
			int lineNum = Function.ParseInt(tv_Wbs.GetFocusedRowCellValue(tl_LineNum));
			DataTable dt = grd_LineTeam.DataSourceTable;
			string[] array = resource.Split(';');
			if (array.Count() > 0)
			{
				string[] array2 = array;
				foreach (string d in array2)
				{
					if (Function.ParseInt(d) <= 0 || dt.Select($"Resource = '{Function.ParseInt(d)}'").Count() != 0)
					{
						continue;
					}
					DataSet dataSet = await _httpClient.EmployeeMasterClient.GetDetail(Function.ParseInt(d));
					DataRow dataRow = dt.NewRow();
					string value = (btn_Owner.dictionSource.ContainsKey(Function.ToString(Function.ParseInt(d))) ? btn_Owner.dictionSource[Function.ToString(d)] : "");
					dataRow["AbsEntry"] = entry;
					dataRow["LineNum"] = lineNum;
					dataRow["Resource"] = Function.ParseInt(d);
					dataRow["EmployeeName"] = value;
					if (dataSet.Tables[0] != null)
					{
						if (dataSet.Tables[0].Rows.Count > 0)
						{
							dataRow["Branch"] = dataSet.Tables[0].Rows[0]["branch"];
						}
					}
					else
					{
						dataRow["Branch"] = 0;
					}
					if (dataSet.Tables[0] != null)
					{
						if (dataSet.Tables[0].Rows.Count > 0)
						{
							dataRow["Department"] = dataSet.Tables[0].Rows[0]["dept"];
						}
					}
					else
					{
						dataRow["Department"] = 0;
					}
					if (dataSet.Tables[1] != null)
					{
						if (dataSet.Tables[1].Rows.Count > 0)
						{
							dataRow["Role"] = dataSet.Tables[1].Rows[0]["roleID"];
						}
					}
					else
					{
						dataRow["Role"] = 0;
					}
					dt.Rows.Add(dataRow);
					dt.AcceptChanges();
				}
				grd_LineTeam.DataSource = dt;
				grd_LineTeam.DatasourceTable().AcceptChanges();
			}
			for (int j = 0; j < dt.Rows.Count; j++)
			{
				int id = Function.ParseInt(dt.Rows[j]["Resource"]);
				if (dtTask != null && dtTask.Rows.Count > 0 && (from map in dtTask.AsEnumerable()
					where Function.ToString(map["Resource"]).Split(';').Contains(id.ToString())
					select map["Wbs"]).Count() == 0)
				{
					dt.Rows[j].Delete();
					grd_LineTeam.DataSource = dt;
					grd_LineTeam.DatasourceTable().AcceptChanges();
				}
			}
			await LoadCalendarData();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Approval_Click(object sender, EventArgs e)
	{
		try
		{
			HttpResult httpResult = await HttpClient.ProjectManager.SetApproval((btn_Approval.Text == "Pending") ? "W" : "P", Function.ParseInt(gv_Doc.GetFocusedRowCellValue("AbsEntry")));
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				WinCommons.Error(Enums.MessageCode.Error, httpResult.Message);
				return;
			}
			WinCommons.Success(Enums.MessageCode.Success, "");
			btn_Search_Click(null, null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Reject_Click(object sender, EventArgs e)
	{
		try
		{
			HttpResult httpResult = await HttpClient.ProjectManager.SetApproval("N", Function.ParseInt(gv_Doc.GetFocusedRowCellValue("AbsEntry")));
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				WinCommons.Error(Enums.MessageCode.Error, httpResult.Message);
				return;
			}
			WinCommons.Success(Enums.MessageCode.Success, "");
			btn_Search_Click(null, null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void schedulerStorage1_FilterResource(object sender, PersistentObjectCancelEventArgs e)
	{
		try
		{
			if (!((Resource)e.Object).Visible)
			{
				e.Cancel = !((Resource)e.Object).Visible;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void schedulerControl1_PopupMenuShowing(object sender, DevExpress.XtraScheduler.PopupMenuShowingEventArgs e)
	{
		try
		{
			if (e.Menu.Id == SchedulerMenuItemId.AppointmentMenu)
			{
				e.Menu.RemoveMenuItem(SchedulerMenuItemId.OpenAppointment);
				e.Menu.RemoveMenuItem(SchedulerMenuItemId.EditSeries);
				e.Menu.RemoveMenuItem(SchedulerMenuItemId.DeleteAppointment);
				e.Menu.RemoveMenuItem(SchedulerMenuItemId.LabelSubMenu);
				e.Menu.RemoveMenuItem(SchedulerMenuItemId.StatusSubMenu);
				e.Menu.RemoveMenuItem(SchedulerMenuItemId.RestoreOccurrence);
			}
			if (e.Menu.Id == SchedulerMenuItemId.DefaultMenu)
			{
				e.Menu.RemoveMenuItem(SchedulerMenuItemId.SwitchViewMenu);
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_LineSub_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
	{
	}

	private void rep_CommentsCount_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_Comments frm_Comments2 = new frm_Comments();
			frm_Comments2.ObjectType = 1470000132.ToString();
			frm_Comments2.ObjectCode = Function.ToString(gv_Doc.GetFocusedRowCellValue("AbsEntry"));
			frm_Comments2.ShowDialog();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rep_WbsComments_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_Comments frm_Comments2 = new frm_Comments();
			frm_Comments2.ObjectType = 1470000132 + "WBS";
			frm_Comments2.ObjectCode = Function.ToString(gv_Doc.GetFocusedRowCellValue("AbsEntry"));
			frm_Comments2.ObjectDetail = Function.ToString(tv_Wbs.FocusedNode.GetValue("Wbs"));
			frm_Comments2.ShowDialog();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_UndoApproval_Click(object sender, EventArgs e)
	{
		try
		{
			HttpResult httpResult = await HttpClient.ProjectManager.UndoApproval(Function.ParseInt(gv_Doc.GetFocusedRowCellValue("AbsEntry")));
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				WinCommons.Error(Enums.MessageCode.Error, httpResult.Message);
				return;
			}
			WinCommons.Success(Enums.MessageCode.Success, "");
			btn_Search_Click(null, null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rep_BudgetComments_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_Comments frm_Comments2 = new frm_Comments();
			frm_Comments2.ObjectType = 1470000132 + "BUDGET";
			frm_Comments2.ObjectCode = Function.ToString(gv_Doc.GetFocusedRowCellValue("AbsEntry"));
			frm_Comments2.ObjectDetail = Function.ToString(tv_budget.FocusedNode.GetValue("LineNum"));
			frm_Comments2.ShowDialog();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_SubSalesOrder_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			int docNum = Function.ParseInt(gv_LineSub.GetFocusedRowCellValue("Order"));
			GetDataSalesOder(docNum);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_SaleOder_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			int docNum = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("Order"));
			GetDataSalesOder(docNum);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void GetDataSalesOder(int docNum)
	{
		try
		{
			DataTable dataTable = await ExtenseionMethods.GetDocumentHeader(docNum, ApzObjectTypes.oOrders);
			if (dataTable != null)
			{
				string text = Function.ToString((from s in dataTable.AsEnumerable()
					where Function.ParseInt(s["DocNum"]) == docNum
					select s).FirstOrDefault()["DocEntry"]);
				if (!string.IsNullOrEmpty(text))
				{
					this.DrillDown(ApzObjectTypes.oOrders, ApzObjectTypes.oOrders, text);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void Frm_ProjectManager_FormClosing(object sender, FormClosingEventArgs e)
	{
		new frm_ProjectManager();
		if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCloseForm, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) != DialogResult.Yes)
		{
			e.Cancel = true;
		}
	}

	private async void DuplicateToolStripMenuItem_Click(object sender, EventArgs e)
	{
		_ = 2;
		try
		{
			WinCommons._OpenProcessing();
			string code = Function.ToString(gv_Doc.GetRowCellValue(gv_Doc.FocusedRowHandle, "Code"));
			int absEntry = Function.ParseInt(gv_Doc.GetRowCellValue(gv_Doc.FocusedRowHandle, "AbsEntry"));
			int budgetLevel = Function.ParseInt(gv_Doc.GetRowCellValue(gv_Doc.FocusedRowHandle, "BudgetLevel"));
			DataTable dataSourceTable = grd_Doc.DataSourceTable;
			DataRow[] dr = (from t in dataSourceTable.AsEnumerable()
				where Function.ToString(t["Code"]) == code
				select t).ToArray();
			DataTable dataTable = await _httpClient.ProjectManager.GetAllBaseLine(absEntry);
			int num = 0;
			new DataTable("dtTask");
			if (dataTable.Rows.Count > 0)
			{
				num = dataTable.AsEnumerable().Max((DataRow r) => Function.ParseInt(Function.ToString(r["Code"]).Substring(Function.ToString(r["Code"]).LastIndexOf("_") + 1)));
			}
			if (dr != null && dr.Length != 0)
			{
				dr[0]["Code"] = code + "_" + (num + 1);
				dr[0]["BaseLine"] = "Y";
				dr[0]["BaseProject"] = absEntry;
				dr[0]["BaseLineActive"] = "N";
				dr[0]["PrgStatus"] = "D";
				dr[0]["WddStatus"] = "W";
				dr[0]["BudgetLevel"] = ((budgetLevel == 0) ? 1 : budgetLevel);
				dr[0]["LoadBom"] = "N";
				dr[0]["OwnerAuth"] = "F";
				DataTable dtCopy = dr.CopyToDataTable();
				DataSet ds = await GetTableSave(dtCopy);
				HttpResult httpResult = await _httpClient.ProjectManager.Add(ds);
				if (string.IsNullOrEmpty(httpResult.Message))
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
					btn_Search_Click(null, null);
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
					btn_Search_Click(null, null);
				}
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private async void duplicateDoc_Click(object sender, EventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			new DataSet();
			string code = Function.ToString(gv_Doc.GetRowCellValue(gv_Doc.FocusedRowHandle, "Code"));
			int absEntry = Function.ParseInt(gv_Doc.GetRowCellValue(gv_Doc.FocusedRowHandle, "AbsEntry"));
			int budgetLevel = Function.ParseInt(gv_Doc.GetRowCellValue(gv_Doc.FocusedRowHandle, "BudgetLevel"));
			DataTable dataSourceTable = grd_Doc.DataSourceTable;
			DataRow[] dr = (from t in dataSourceTable.AsEnumerable()
				where Function.ToString(t["Code"]) == code
				select t).ToArray();
			await _httpClient.ProjectManager.GetAllBaseLine(absEntry);
			new DataTable("dtTask");
			if (dr != null && dr.Length != 0)
			{
				dr[0]["Code"] = "";
				dr[0]["Name"] = "";
				dr[0]["BaseLine"] = "N";
				dr[0]["BaseProject"] = absEntry;
				dr[0]["BaseLineActive"] = "N";
				dr[0]["PrgStatus"] = "D";
				dr[0]["BudgetLevel"] = ((budgetLevel == 0) ? 1 : budgetLevel);
				dr[0]["WddStatus"] = "-";
				dr[0]["LoadBom"] = "N";
				DataTable dataSource = dr.CopyToDataTable();
				grd_Doc.DataSource = dataSource;
				tv_Wbs.DataSource = tv_Wbs.DataSourceTable().Copy();
				tv_budget.DataSource = tv_budget.DataSourceTable();
				grd_LineTeam.DataSource = grd_LineTeam.DataSourceTable();
				tv_Payments.DataSource = tv_Payments.DataSourceTable();
				base.FormMode = Enums.FormViewModeType.Add;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	public override void CHANGELOG()
	{
		try
		{
			if (base.FormMode == Enums.FormViewModeType.View || base.FormMode == Enums.FormViewModeType.Update)
			{
				string primaryKey = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("AbsEntry")).ToString();
				Enums.ChangeLog changeLogType = ChangeLogType;
				GetChangeLogParams getChangeLogParams = new GetChangeLogParams();
				getChangeLogParams.PrimaryKey = primaryKey;
				getChangeLogParams.Object = changeLogType;
				getChangeLogParams.IsApzTable = true;
				frm_ChangeLogList frm_ChangeLogList2 = new frm_ChangeLogList
				{
					GetChangeLogParams = getChangeLogParams,
					MdiParent = MdiParent
				};
				frm_ChangeLogList2.Text = frm_ChangeLogList2.Text + " - " + base.Text + (string.IsNullOrWhiteSpace(getChangeLogParams.PrimaryKey) ? "" : (" (" + getChangeLogParams.PrimaryKey + ")"));
				frm_ChangeLogList2.Show();
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

	private async Task<DataSet> GetTableSave(DataTable dtCopy)
	{
		try
		{
			DataSet ds = new DataSet();
			_dtDoc = dtCopy;
			DataTable dtWbs = tv_Wbs.DataSourceTable().Copy();
			_dtTeam = grd_LineTeam.DataSourceTable.Copy();
			_dtItem = tv_budget.DataSourceTable();
			_dtAddTeam = await AddTeam();
			_dtDocument = grd_LineDocument.DataSourceTable();
			_dtSub = grd_LineSub.DataSourceTable();
			_dtPayment = tv_Payments.DataSourceTable();
			_dtDoc.TableName = "dtDoc";
			_dtItem.TableName = "dtItem";
			_dtAddTeam.TableName = "dtTeam";
			_dtDocument.TableName = "dtDocment";
			_dtDepen.TableName = "dtDepen";
			_dtSub.TableName = "dtSub";
			_dtPayment.TableName = "dtPayment";
			ds.Tables.Add(_dtDoc.Copy());
			ds.Tables.Add(dtWbs.Copy());
			ds.Tables.Add(_dtDepen.Copy());
			ds.Tables.Add(_dtAddTeam.Copy());
			ds.Tables.Add(_dtItem.Copy());
			ds.Tables.Add(_dtDocument.Copy());
			ds.Tables.Add(_dtSub.Copy());
			ds.Tables.Add(_dtPayment.Copy());
			return ds;
		}
		catch (Exception)
		{
			throw;
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions4 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions4 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions5 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions5 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions6 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance11 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered11 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed11 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled11 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions6 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance12 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered12 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed12 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled12 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions7 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance13 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered13 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed13 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled13 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraTreeList.StyleFormatConditions.TreeListFormatRule treeListFormatRule = new DevExpress.XtraTreeList.StyleFormatConditions.TreeListFormatRule();
		DevExpress.XtraEditors.FormatConditionRuleIconSet formatConditionRuleIconSet2 = new DevExpress.XtraEditors.FormatConditionRuleIconSet();
		DevExpress.XtraEditors.FormatConditionIconSet formatConditionIconSet2 = new DevExpress.XtraEditors.FormatConditionIconSet();
		DevExpress.XtraEditors.FormatConditionIconSetIcon formatConditionIconSetIcon5 = new DevExpress.XtraEditors.FormatConditionIconSetIcon();
		DevExpress.XtraEditors.FormatConditionIconSetIcon formatConditionIconSetIcon6 = new DevExpress.XtraEditors.FormatConditionIconSetIcon();
		DevExpress.XtraEditors.FormatConditionIconSetIcon formatConditionIconSetIcon7 = new DevExpress.XtraEditors.FormatConditionIconSetIcon();
		DevExpress.XtraEditors.FormatConditionIconSetIcon formatConditionIconSetIcon8 = new DevExpress.XtraEditors.FormatConditionIconSetIcon();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions7 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance14 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered14 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed14 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled14 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions8 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance15 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered15 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed15 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled15 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions9 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance16 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered16 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed16 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled16 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions8 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance17 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered17 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed17 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled17 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions9 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance18 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered18 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed18 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled18 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraScheduler.TimeRuler value = new DevExpress.XtraScheduler.TimeRuler();
		DevExpress.XtraScheduler.TimeRuler value2 = new DevExpress.XtraScheduler.TimeRuler();
		DevExpress.XtraScheduler.TimeRuler value3 = new DevExpress.XtraScheduler.TimeRuler();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions10 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance19 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered19 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed19 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled19 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions10 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance20 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered20 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed20 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled20 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions11 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance21 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered21 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed21 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled21 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions11 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance22 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered22 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed22 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled22 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions12 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance23 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered23 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed23 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled23 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions12 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance24 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered24 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed24 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled24 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions13 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance25 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered25 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed25 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled25 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions13 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance26 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered26 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed26 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled26 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions14 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance27 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered27 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed27 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled27 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions15 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance28 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered28 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed28 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled28 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions16 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance29 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered29 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed29 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled29 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions17 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance30 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered30 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed30 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled30 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions18 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance31 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered31 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed31 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled31 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions19 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance32 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered32 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed32 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled32 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions14 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance33 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered33 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed33 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled33 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions20 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance34 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered34 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed34 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled34 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions21 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance35 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered35 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed35 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled35 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions22 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance36 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered36 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed36 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled36 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions15 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance37 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered37 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed37 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled37 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions23 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance38 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered38 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed38 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled38 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions24 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance39 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered39 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed39 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled39 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions25 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance40 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered40 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed40 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled40 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions16 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance41 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered41 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed41 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled41 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions26 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance42 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered42 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed42 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled42 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions27 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance43 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered43 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed43 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled43 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraGrid.GridFormatRule gridFormatRule2 = new DevExpress.XtraGrid.GridFormatRule();
		DevExpress.XtraEditors.FormatConditionRuleIconSet formatConditionRuleIconSet3 = new DevExpress.XtraEditors.FormatConditionRuleIconSet();
		DevExpress.XtraEditors.FormatConditionIconSet formatConditionIconSet3 = new DevExpress.XtraEditors.FormatConditionIconSet();
		DevExpress.XtraEditors.FormatConditionIconSetIcon formatConditionIconSetIcon9 = new DevExpress.XtraEditors.FormatConditionIconSetIcon();
		DevExpress.XtraEditors.FormatConditionIconSetIcon formatConditionIconSetIcon10 = new DevExpress.XtraEditors.FormatConditionIconSetIcon();
		DevExpress.XtraEditors.FormatConditionIconSetIcon formatConditionIconSetIcon11 = new DevExpress.XtraEditors.FormatConditionIconSetIcon();
		DevExpress.XtraEditors.FormatConditionIconSetIcon formatConditionIconSetIcon12 = new DevExpress.XtraEditors.FormatConditionIconSetIcon();
		new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		new DevExpress.Utils.SerializableAppearanceObject();
		new DevExpress.Utils.SerializableAppearanceObject();
		new DevExpress.Utils.SerializableAppearanceObject();
		new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions28 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance44 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered44 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed44 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled44 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions29 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance45 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered45 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed45 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled45 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions17 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance46 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered46 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed46 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled46 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions30 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance47 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered47 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed47 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled47 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions18 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance48 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered48 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed48 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled48 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions31 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance49 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered49 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed49 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled49 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions19 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance50 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered50 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed50 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled50 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions20 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance51 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered51 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed51 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled51 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions32 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance52 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered52 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed52 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled52 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions21 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance53 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered53 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed53 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled53 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions33 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance54 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered54 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed54 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled54 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraGrid.GridFormatRule gridFormatRule3 = new DevExpress.XtraGrid.GridFormatRule();
		DevExpress.XtraEditors.FormatConditionRuleIconSet formatConditionRuleIconSet4 = new DevExpress.XtraEditors.FormatConditionRuleIconSet();
		DevExpress.XtraEditors.FormatConditionIconSet formatConditionIconSet4 = new DevExpress.XtraEditors.FormatConditionIconSet();
		DevExpress.XtraEditors.FormatConditionIconSetIcon formatConditionIconSetIcon13 = new DevExpress.XtraEditors.FormatConditionIconSetIcon();
		DevExpress.XtraEditors.FormatConditionIconSetIcon formatConditionIconSetIcon14 = new DevExpress.XtraEditors.FormatConditionIconSetIcon();
		DevExpress.XtraEditors.FormatConditionIconSetIcon formatConditionIconSetIcon15 = new DevExpress.XtraEditors.FormatConditionIconSetIcon();
		DevExpress.XtraEditors.FormatConditionIconSetIcon formatConditionIconSetIcon16 = new DevExpress.XtraEditors.FormatConditionIconSetIcon();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions34 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance55 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered55 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed55 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled55 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions35 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance56 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered56 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed56 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled56 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions36 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance57 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered57 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed57 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled57 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions37 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance58 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered58 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed58 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled58 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions38 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance59 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered59 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed59 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled59 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions22 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance60 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered60 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed60 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled60 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions23 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance61 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered61 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed61 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled61 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions24 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance62 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered62 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed62 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled62 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions25 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance63 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered63 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed63 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled63 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraTreeList.StyleFormatConditions.TreeListFormatRule treeListFormatRule2 = new DevExpress.XtraTreeList.StyleFormatConditions.TreeListFormatRule();
		DevExpress.XtraEditors.FormatConditionRuleIconSet formatConditionRuleIconSet5 = new DevExpress.XtraEditors.FormatConditionRuleIconSet();
		DevExpress.XtraEditors.FormatConditionIconSet formatConditionIconSet5 = new DevExpress.XtraEditors.FormatConditionIconSet();
		DevExpress.XtraEditors.FormatConditionIconSetIcon formatConditionIconSetIcon17 = new DevExpress.XtraEditors.FormatConditionIconSetIcon();
		DevExpress.XtraEditors.FormatConditionIconSetIcon formatConditionIconSetIcon18 = new DevExpress.XtraEditors.FormatConditionIconSetIcon();
		DevExpress.XtraEditors.FormatConditionIconSetIcon formatConditionIconSetIcon19 = new DevExpress.XtraEditors.FormatConditionIconSetIcon();
		DevExpress.XtraEditors.FormatConditionIconSetIcon formatConditionIconSetIcon20 = new DevExpress.XtraEditors.FormatConditionIconSetIcon();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions39 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance64 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered64 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed64 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled64 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions40 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance65 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered65 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed65 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled65 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions41 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance66 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered66 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed66 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled66 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions42 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance67 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered67 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed67 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled67 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions43 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance68 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered68 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed68 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled68 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions44 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance69 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered69 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed69 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled69 = new DevExpress.Utils.SerializableAppearanceObject();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.PM.frm_ProjectManager));
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions26 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance70 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered70 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed70 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled70 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions45 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance71 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered71 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed71 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled71 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions27 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance72 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered72 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed72 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled72 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions46 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance73 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered73 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed73 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled73 = new DevExpress.Utils.SerializableAppearanceObject();
		this.gc_priority = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_Priority = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.tl_Priority = new Apzon.Controls.Tree.TreeListColumnEx();
		this.luk_taskPriority = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.gridColumnEx61 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_SubPriority = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.gc_priorityy = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_priorityGrid = new Apzon.Forms.UserControls.RepositoryItemLookUpEditDefineNew();
		this.tl_Priority_Completed = new Apzon.Controls.Tree.TreeListColumnEx();
		this.luk_prioritylistcompleted = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.repositoryItemCalcEditEx1 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx2 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx3 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx4 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx5 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx6 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx7 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx8 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.ctm_Doc = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addNewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.duplicateDoc = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
		this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
		this.addSubProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
		this.newTaskInDoc = new System.Windows.Forms.ToolStripMenuItem();
		this.newEvenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.newTrackingIssueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.relatedActivitiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.canceledToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.closedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.viewDiagramToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.duplicateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx23 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.txt_prjCode = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.gridColumnEx3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx4 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_Type = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.gridColumnEx5 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_Status = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.gridColumnEx6 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_BpCode = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx7 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx8 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_Manager = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx9 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx10 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx11 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx14 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_Project = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx15 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx16 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_Industry = new Apzon.Forms.UserControls.RepositoryItemLookUpEditCombo();
		this.gridColumnEx17 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_Owner = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx18 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_SaleOder = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx19 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemMemoExEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit();
		this.gridColumnEx60 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx62 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx63 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx90 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_FinProject = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx24 = new Apzon.Controls.Grid.GridColumnEx();
		this.txt_budgetLevel = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.gridColumnEx30 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_Whs = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx31 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_DocCurr = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx33 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx34 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx35 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemDateEditEx1 = new Apzon.Controls.Editor.RepositoryItemDateEditEx();
		this.gridColumnEx36 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemTimeSpanEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemTimeSpanEdit();
		this.gridColumnEx37 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemDateEditEx2 = new Apzon.Controls.Editor.RepositoryItemDateEditEx();
		this.gridColumnEx38 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemTimeSpanEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemTimeSpanEdit();
		this.gridColumnEx12 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_WddStatus = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.gridColumnEx20 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx21 = new Apzon.Controls.Grid.GridColumnEx();
		this.gv_LoadBom = new Apzon.Controls.Grid.GridColumnEx();
		this.look_LoadBom = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.gridColumnEx22 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_CommentsCount = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gridColumnEx101 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx102 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx104 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_BaseProject = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.luk_BaseLine = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.luk_BaseLineActive = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.xtra_Line = new Apzon.Controls.Controls.XtraTabControlEx();
		this.tab_Task = new Apzon.Controls.Controls.XtraTabPageEx();
		this.splitContainerControl4 = new DevExpress.XtraEditors.SplitContainerControl();
		this.tv_Wbs = new Apzon.Controls.Tree.TreeEx();
		this.tl_Entry = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tl_Wbs = new Apzon.Controls.Tree.TreeListColumnEx();
		this.btn_NewTask = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.tl_Code = new Apzon.Controls.Tree.TreeListColumnEx();
		this.txt_TaskCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.tl_Name = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tl_FromDate = new Apzon.Controls.Tree.TreeListColumnEx();
		this.date_fromdate = new Apzon.Controls.Editor.RepositoryItemDateEditEx();
		this.tl_ToDate = new Apzon.Controls.Tree.TreeListColumnEx();
		this.date_todate = new Apzon.Controls.Editor.RepositoryItemDateEditEx();
		this.tl_Milestone = new Apzon.Controls.Tree.TreeListColumnEx();
		this.luk_Milestone = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.tl_Notes = new Apzon.Controls.Tree.TreeListColumnEx();
		this.repositoryItemMemoExEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit();
		this.tl_Status = new Apzon.Controls.Tree.TreeListColumnEx();
		this.luk_taskStatus = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.tl_Dependent = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tl_Resource = new Apzon.Controls.Tree.TreeListColumnEx();
		this.btn_resource = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.tl_LineNum = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tl_IsExist = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tl_FatherNum = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tl_visOrder = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tl_BaseLine = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tl_WbsTemplate = new Apzon.Controls.Tree.TreeListColumnEx();
		this.btn_WBSTemplate = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.tl_WBSMode = new Apzon.Controls.Tree.TreeListColumnEx();
		this.luk_WBSMode = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.tl_Duration = new Apzon.Controls.Tree.TreeListColumnEx();
		this.treeListColumnEx2 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.rep_WbsComments = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.ctm_Rows = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.toolStripMenuItemIndent = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripMenuItemOutdent = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
		this.addRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.insertRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.duplicateRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
		this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
		this.addSubProjectToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
		this.newTaskToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.newTrackingIssueToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.newEventToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.genTasksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
		this.viewOrgChartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.splitContainerControl5 = new DevExpress.XtraEditors.SplitContainerControl();
		this.schedulerControl1 = new DevExpress.XtraScheduler.SchedulerControl();
		this.schedulerStorage1 = new DevExpress.XtraScheduler.SchedulerStorage(this.components);
		this.chk_Action = new DevExpress.XtraEditors.CheckedListBoxControl();
		this.chk_Users = new DevExpress.XtraScheduler.UI.ResourcesCheckedListBoxControl();
		this.dateNavigator1 = new DevExpress.XtraScheduler.DateNavigator();
		this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
		this.xtra_Team = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_LineTeam = new Apzon.Controls.Grid.GridControlEx();
		this.gv_LineTeam = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx50 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx51 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx52 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_ResourceEmployee = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx53 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx54 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_branch = new Apzon.Forms.UserControls.RepositoryItemLookUpEditDefineNew();
		this.gridColumnEx55 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_dept = new Apzon.Forms.UserControls.RepositoryItemLookUpEditDefineNew();
		this.gridColumnEx56 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_role = new Apzon.Forms.UserControls.RepositoryItemLookUpEditDefineNew();
		this.xtra_Budget = new Apzon.Controls.Controls.XtraTabPageEx();
		this.xtra_LineBudget = new Apzon.Controls.Controls.XtraTabControlEx();
		this.xtra_LineItem = new Apzon.Controls.Controls.XtraTabPageEx();
		this.tv_budget = new Apzon.Controls.Tree.TreeEx();
		this.tb_AbsEntry = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tb_LineNum = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tb_ItemCode = new Apzon.Controls.Tree.TreeListColumnEx();
		this.btn_ItemCode = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.tb_ItemName = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tb_TaskCode = new Apzon.Controls.Tree.TreeListColumnEx();
		this.btn_TaskCode = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.tb_TaskName = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tb_plQty = new Apzon.Controls.Tree.TreeListColumnEx();
		this.cal_PlanQty = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.tb_RefPrice = new Apzon.Controls.Tree.TreeListColumnEx();
		this.txt_RefPrice = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.tb_Amount = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tb_Branch = new Apzon.Controls.Tree.TreeListColumnEx();
		this.luk_ItemBranch = new Apzon.Forms.UserControls.RepositoryItemLookUpEditDefineNew();
		this.tb_ActualQty = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tb_QtyDiff = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tb_Notes = new Apzon.Controls.Tree.TreeListColumnEx();
		this.repositoryItemMemoExEdit3 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit();
		this.tb_FatherNum = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tb_IsNew = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tb_ActualAmount = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tb_FromDate = new Apzon.Controls.Tree.TreeListColumnEx();
		this.dat_IFromDate = new Apzon.Controls.Editor.RepositoryItemDateEditEx();
		this.tb_ToDate = new Apzon.Controls.Tree.TreeListColumnEx();
		this.dat_IToDate = new Apzon.Controls.Editor.RepositoryItemDateEditEx();
		this.tb_BomItem = new Apzon.Controls.Tree.TreeListColumnEx();
		this.btn_BOM = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.tb_OrderItem = new Apzon.Controls.Tree.TreeListColumnEx();
		this.btn_OrderItem = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.tb_PR = new Apzon.Controls.Tree.TreeListColumnEx();
		this.btn_PR = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.tb_PO = new Apzon.Controls.Tree.TreeListColumnEx();
		this.btn_PO = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.tb_GRPO = new Apzon.Controls.Tree.TreeListColumnEx();
		this.btn_GRPO = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.tb_Delivery = new Apzon.Controls.Tree.TreeListColumnEx();
		this.btn_Delivery = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.tb_Invoice = new Apzon.Controls.Tree.TreeListColumnEx();
		this.btn_APInvoice = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.tb_UoM = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tb_BaseLine = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tb_AR = new Apzon.Controls.Tree.TreeListColumnEx();
		this.btn_ARInvoice = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.tb_BaseQty = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tb_ItmsGrpCod = new Apzon.Controls.Tree.TreeListColumnEx();
		this.luk_ItmsGrpCod = new Apzon.Forms.UserControls.RepositoryItemLookUpEditDefineNew();
		this.tb_DefaultWhs = new Apzon.Controls.Tree.TreeListColumnEx();
		this.btn_WarehouseItem = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.tb_VisOrder = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tb_Currency = new Apzon.Controls.Tree.TreeListColumnEx();
		this.luk_Currency = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.tb_PQ = new Apzon.Controls.Tree.TreeListColumnEx();
		this.btn_PQ = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.tb_ARDR = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tb_APDR = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tb_IGE = new Apzon.Controls.Tree.TreeListColumnEx();
		this.btn_IGE = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.treeListColumnEx1 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.btn_WOR = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.treeListColumnEx3 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.rep_BudgetComments = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.ctm_BudgetItem = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addRowToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripMenuItemInsertRowBudget = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
		this.toolStripMenuItemDuplicateRowBudget = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
		this.removeRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.addItemRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.insertItemRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.btn_AbsEntry = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.luk_UoM = new Apzon.Forms.UserControls.RepositoryItemLookUpEditDefineNew();
		this.luk_ItemGroup = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.xtra_Payment = new Apzon.Controls.Controls.XtraTabPageEx();
		this.tv_Payments = new Apzon.Controls.Tree.TreeEx();
		this.tp_AbsEntry = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tp_TaskCode = new Apzon.Controls.Tree.TreeListColumnEx();
		this.btn_WbsPayment = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.tp_TaskName = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tp_InvoicePlan = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tp_IncomingPayment = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tp_Payments = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tp_Duedate = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tp_Remarks = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tp_LineNum = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tp_FatherNum = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tp_IsNew = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tp_BaseLine = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tp_VisOrder = new Apzon.Controls.Tree.TreeListColumnEx();
		this.ctm_Payments = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
		this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
		this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
		this.xtra_Document = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_LineDocument = new Apzon.Controls.Grid.GridControlEx();
		this.gv_LineDocument = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx47 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx48 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx49 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_DocNumDo = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx25 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx26 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx27 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx29 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx32 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_CurrencyLineDu = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.xtra_Sub = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_LineSub = new Apzon.Controls.Grid.GridControlEx();
		this.gv_LineSub = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx39 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_Code = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx40 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemTextEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.gridColumnEx41 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx42 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_SubType = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.gridColumnEx43 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_SubStatus = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.gridColumnEx44 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_SubBpCode = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx45 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx46 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_SubPM = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx57 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx58 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx59 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx64 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_SubMasterProject = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx65 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx66 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemLookUpEditCombo1 = new Apzon.Forms.UserControls.RepositoryItemLookUpEditCombo();
		this.gridColumnEx67 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_SubOwner = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx68 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_SubSalesOrder = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx70 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemMemoExEdit4 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit();
		this.gridColumnEx73 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx75 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx76 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx77 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_SubFinalProject = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx83 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemTextEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.gridColumnEx91 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_SubWhs = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx92 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_SubCurrency = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx93 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx94 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx95 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemDateEditEx3 = new Apzon.Controls.Editor.RepositoryItemDateEditEx();
		this.gridColumnEx96 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemTimeSpanEdit3 = new DevExpress.XtraEditors.Repository.RepositoryItemTimeSpanEdit();
		this.gridColumnEx97 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemDateEditEx4 = new Apzon.Controls.Editor.RepositoryItemDateEditEx();
		this.gridColumnEx98 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemTimeSpanEdit4 = new DevExpress.XtraEditors.Repository.RepositoryItemTimeSpanEdit();
		this.gridColumnEx99 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx100 = new Apzon.Controls.Grid.GridColumnEx();
		this.xtra_Issue = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_lineissue = new Apzon.Controls.Grid.GridControlEx();
		this.ctm_Issues = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.toolStripMenuItemAddNew = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_lineIssue = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx13 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_Id = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx28 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx69 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_TypeGrid = new Apzon.Forms.UserControls.RepositoryItemLookUpEditDefineNew();
		this.gridColumnEx71 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_statusGrid = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.gridColumnEx72 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_report = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx74 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_handle = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx78 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_solution = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx79 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx80 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_IssueBp = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx81 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx82 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_IssueSLBy = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx84 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx85 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx86 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_IssuwPrOwner = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx87 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx88 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx89 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_IssuePr = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.btn_IssueTaskCode = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.tab_TaskList = new Apzon.Controls.Controls.XtraTabPageEx();
		this.tv_ListCompleted = new Apzon.Controls.Tree.TreeEx();
		this.tl_RowId = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tl_ClgCode = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tl_Closed_Completed = new Apzon.Controls.Tree.TreeListColumnEx();
		this.rep_btn_Closed = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.tl_Details = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tl_ProjectWBS_Completed = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tl_AssignTo_Completed = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tl_DueDate_Completed = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tl_ProjectEntry_Completed = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tl_OrderLine = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tl_CntctDate = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tl_ENDTime = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tl_CloseDate_Completed = new Apzon.Controls.Tree.TreeListColumnEx();
		this.ctm_TaskList = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.toolStripMenuItemNewTask = new System.Windows.Forms.ToolStripMenuItem();
		this.rep_btn_Open = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.rep_btn_0 = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.rep_btn_1 = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.rep_btn_2 = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.rep_btn_3 = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.xtra_NoteList = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_NoteList = new Apzon.Controls.Grid.GridControlEx();
		this.gv_NoteList = new Apzon.Controls.Grid.GridViewEx();
		this.cl_ClgCode = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_Details = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_AssignTo = new Apzon.Controls.Grid.GridColumnEx();
		this.NoteDate = new Apzon.Controls.Grid.GridColumnEx();
		this.NoteTime = new Apzon.Controls.Grid.GridColumnEx();
		this.xtra_EventList = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_EventList = new Apzon.Controls.Grid.GridControlEx();
		this.gv_EventList = new Apzon.Controls.Grid.GridViewEx();
		this.ClgCode = new Apzon.Controls.Grid.GridColumnEx();
		this.Details = new Apzon.Controls.Grid.GridColumnEx();
		this.CardCode = new Apzon.Controls.Grid.GridColumnEx();
		this.Recontact = new Apzon.Controls.Grid.GridColumnEx();
		this.endDate = new Apzon.Controls.Grid.GridColumnEx();
		this.AllDayEvent = new Apzon.Controls.Grid.GridColumnEx();
		this.AssignTo = new Apzon.Controls.Grid.GridColumnEx();
		this.CntctDate = new Apzon.Controls.Grid.GridColumnEx();
		this.BeginTime = new Apzon.Controls.Grid.GridColumnEx();
		this.ENDTime = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx103 = new Apzon.Controls.Grid.GridColumnEx();
		this.ctm_Sub = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.toolStripMenuItemAddSub = new System.Windows.Forms.ToolStripMenuItem();
		this.btn_Ok = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.txt_Code = new Apzon.Controls.Editor.TextEditEx();
		this.luk_Sta = new Apzon.Controls.Editor.LookUpEditEx();
		this.btn_Bp = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.btn_Search = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_ShowGantt = new Apzon.Controls.Controls.SimpleButtonEx();
		this.toolTipController1 = new DevExpress.Utils.ToolTipController(this.components);
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine5 = new Apzon.Controls.Controls.LabelLine();
		this.date_StartTo = new Apzon.Controls.Editor.DateEditEx();
		this.date_StartFrom = new Apzon.Controls.Editor.DateEditEx();
		this.drp_CrDocumnet = new DevExpress.XtraEditors.DropDownButton();
		this.btn_Select = new Apzon.Controls.Controls.SimpleButtonEx();
		this.txt_No = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine6 = new Apzon.Controls.Controls.LabelLine();
		this.btn_OwnerSearch = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.labelLine7 = new Apzon.Controls.Controls.LabelLine();
		this.btn_GanttView = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Approval = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Reject = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_UndoApproval = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.luk_Priority).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_taskPriority).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_SubPriority).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_priorityGrid).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_prioritylistcompleted).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx6).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx7).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx8).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		this.ctm_Doc.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_prjCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Type).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Status).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_BpCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_Manager).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_Project).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Industry).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_Owner).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_SaleOder).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_FinProject).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_budgetLevel).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_Whs).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_DocCurr).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx1.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTimeSpanEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx2.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTimeSpanEdit2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_WddStatus).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.look_LoadBom).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CommentsCount).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_BaseProject).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_BaseLine).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_BaseLineActive).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.xtra_Line).BeginInit();
		this.xtra_Line.SuspendLayout();
		this.tab_Task.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl4).BeginInit();
		this.splitContainerControl4.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.tv_Wbs).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_NewTask).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_TaskCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_fromdate).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_fromdate.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_todate).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_todate.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Milestone).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_taskStatus).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_resource).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_WBSTemplate).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_WBSMode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_WbsComments).BeginInit();
		this.ctm_Rows.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl5).BeginInit();
		this.splitContainerControl5.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.schedulerControl1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.schedulerStorage1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Action).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Users).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dateNavigator1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dateNavigator1.CalendarTimeProperties).BeginInit();
		this.xtra_Team.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_LineTeam).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_LineTeam).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_ResourceEmployee).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_branch).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_dept).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_role).BeginInit();
		this.xtra_Budget.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.xtra_LineBudget).BeginInit();
		this.xtra_LineBudget.SuspendLayout();
		this.xtra_LineItem.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.tv_budget).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_ItemCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_TaskCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cal_PlanQty).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_RefPrice).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_ItemBranch).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dat_IFromDate).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dat_IFromDate.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dat_IToDate).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dat_IToDate.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_BOM).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_OrderItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_PR).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_PO).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_GRPO).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_Delivery).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_APInvoice).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_ARInvoice).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_ItmsGrpCod).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_WarehouseItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Currency).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_PQ).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_IGE).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_WOR).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_BudgetComments).BeginInit();
		this.ctm_BudgetItem.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.btn_AbsEntry).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_UoM).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_ItemGroup).BeginInit();
		this.xtra_Payment.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.tv_Payments).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_WbsPayment).BeginInit();
		this.ctm_Payments.SuspendLayout();
		this.xtra_Document.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_LineDocument).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_LineDocument).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_DocNumDo).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_CurrencyLineDu).BeginInit();
		this.xtra_Sub.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_LineSub).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_LineSub).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_Code).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_SubType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_SubStatus).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_SubBpCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_SubPM).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_SubMasterProject).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemLookUpEditCombo1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_SubOwner).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_SubSalesOrder).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_SubFinalProject).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEdit2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_SubWhs).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_SubCurrency).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx3.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTimeSpanEdit3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx4.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTimeSpanEdit4).BeginInit();
		this.xtra_Issue.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_lineissue).BeginInit();
		this.ctm_Issues.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_lineIssue).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_Id).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_TypeGrid).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_statusGrid).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_report).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_handle).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_solution).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_IssueBp).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_IssueSLBy).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_IssuwPrOwner).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_IssuePr).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_IssueTaskCode).BeginInit();
		this.tab_TaskList.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.tv_ListCompleted).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_btn_Closed).BeginInit();
		this.ctm_TaskList.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.rep_btn_Open).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_btn_0).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_btn_1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_btn_2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_btn_3).BeginInit();
		this.xtra_NoteList.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_NoteList).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_NoteList).BeginInit();
		this.xtra_EventList.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_EventList).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_EventList).BeginInit();
		this.ctm_Sub.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txt_Code.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Sta.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_Bp.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_StartTo.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_StartTo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_StartFrom.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_StartFrom.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_No.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_OwnerSearch.Properties).BeginInit();
		base.SuspendLayout();
		this.gc_priority.ColumnEdit = this.luk_Priority;
		this.gc_priority.CurrencyDateField = null;
		this.gc_priority.CurrencyField = null;
		this.gc_priority.CurrencyFixed = null;
		this.gc_priority.CurrencyFixedRate = 0.0;
		this.gc_priority.CurrencyRateField = null;
		this.gc_priority.FieldName = "Priority";
		this.gc_priority.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gc_priority.Name = "gc_priority";
		this.gc_priority.PrimaryKey = false;
		this.gc_priority.UnitField = null;
		this.gc_priority.Width = 52;
		this.luk_Priority.AutoHeight = false;
		this.luk_Priority.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_Priority.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Priority.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.luk_Priority.IsSearchControl = false;
		this.luk_Priority.Name = "luk_Priority";
		this.luk_Priority.NullText = "";
		this.luk_Priority.ShowFooter = false;
		this.luk_Priority.ShowHeader = false;
		this.tl_Priority.ColumnEdit = this.luk_taskPriority;
		this.tl_Priority.CurrencyDateField = null;
		this.tl_Priority.CurrencyField = null;
		this.tl_Priority.CurrencyFixed = null;
		this.tl_Priority.CurrencyFixedRate = 0.0;
		this.tl_Priority.CurrencyRateField = null;
		this.tl_Priority.FieldName = "Priority";
		this.tl_Priority.Name = "tl_Priority";
		this.tl_Priority.Width = 52;
		this.luk_taskPriority.AutoHeight = false;
		this.luk_taskPriority.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_taskPriority.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_taskPriority.IsSearchControl = false;
		this.luk_taskPriority.Name = "luk_taskPriority";
		this.luk_taskPriority.NullText = "";
		this.luk_taskPriority.ShowFooter = false;
		this.luk_taskPriority.ShowHeader = false;
		this.gridColumnEx61.ColumnEdit = this.luk_SubPriority;
		this.gridColumnEx61.CurrencyDateField = null;
		this.gridColumnEx61.CurrencyField = null;
		this.gridColumnEx61.CurrencyFixed = null;
		this.gridColumnEx61.CurrencyFixedRate = 0.0;
		this.gridColumnEx61.CurrencyRateField = null;
		this.gridColumnEx61.FieldName = "Priority";
		this.gridColumnEx61.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx61.Name = "gridColumnEx61";
		this.gridColumnEx61.PrimaryKey = false;
		this.gridColumnEx61.UnitField = null;
		this.gridColumnEx61.Width = 52;
		this.luk_SubPriority.AutoHeight = false;
		this.luk_SubPriority.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_SubPriority.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_SubPriority.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.luk_SubPriority.IsSearchControl = false;
		this.luk_SubPriority.Name = "luk_SubPriority";
		this.luk_SubPriority.NullText = "";
		this.luk_SubPriority.ShowFooter = false;
		this.luk_SubPriority.ShowHeader = false;
		this.gc_priorityy.Caption = "Priority";
		this.gc_priorityy.ColumnEdit = this.luk_priorityGrid;
		this.gc_priorityy.CurrencyDateField = null;
		this.gc_priorityy.CurrencyField = null;
		this.gc_priorityy.CurrencyFixed = null;
		this.gc_priorityy.CurrencyFixedRate = 0.0;
		this.gc_priorityy.CurrencyRateField = null;
		this.gc_priorityy.FieldName = "Priority";
		this.gc_priorityy.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gc_priorityy.Name = "gc_priorityy";
		this.gc_priorityy.OptionsColumn.ReadOnly = true;
		this.gc_priorityy.PrimaryKey = false;
		this.gc_priorityy.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gc_priorityy.UnitField = null;
		this.gc_priorityy.Width = 150;
		this.luk_priorityGrid.AutoHeight = false;
		this.luk_priorityGrid.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
		this.luk_priorityGrid.CheckFields = null;
		this.luk_priorityGrid.CheckTables = null;
		this.luk_priorityGrid.Name = "luk_priorityGrid";
		this.luk_priorityGrid.NullText = "";
		this.luk_priorityGrid.ShowFooter = false;
		this.luk_priorityGrid.ShowHeader = false;
		this.tl_Priority_Completed.Caption = "Priority";
		this.tl_Priority_Completed.CurrencyDateField = null;
		this.tl_Priority_Completed.CurrencyField = null;
		this.tl_Priority_Completed.CurrencyFixed = null;
		this.tl_Priority_Completed.CurrencyFixedRate = 0.0;
		this.tl_Priority_Completed.CurrencyRateField = null;
		this.tl_Priority_Completed.FieldName = "Priority";
		this.tl_Priority_Completed.Name = "tl_Priority_Completed";
		this.tl_Priority_Completed.OptionsColumn.AllowEdit = false;
		this.tl_Priority_Completed.OptionsColumn.FixedWidth = true;
		this.tl_Priority_Completed.Width = 22;
		this.luk_prioritylistcompleted.AutoHeight = false;
		this.luk_prioritylistcompleted.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_prioritylistcompleted.Name = "luk_prioritylistcompleted";
		this.repositoryItemCalcEditEx1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx1.IsSearchControl = false;
		this.repositoryItemCalcEditEx1.Name = "repositoryItemCalcEditEx1";
		this.repositoryItemCalcEditEx2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx2.IsSearchControl = false;
		this.repositoryItemCalcEditEx2.Name = "repositoryItemCalcEditEx2";
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
		this.repositoryItemCalcEditEx5.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx5.IsSearchControl = false;
		this.repositoryItemCalcEditEx5.Name = "repositoryItemCalcEditEx5";
		this.repositoryItemCalcEditEx6.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx6.IsSearchControl = false;
		this.repositoryItemCalcEditEx6.Name = "repositoryItemCalcEditEx6";
		this.repositoryItemCalcEditEx7.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx7.IsSearchControl = false;
		this.repositoryItemCalcEditEx7.Name = "repositoryItemCalcEditEx7";
		this.repositoryItemCalcEditEx8.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx8.IsSearchControl = false;
		this.repositoryItemCalcEditEx8.Name = "repositoryItemCalcEditEx8";
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(10, 68);
		this.splitContainerControl1.Margin = new System.Windows.Forms.Padding(2);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.grd_Doc);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.xtra_Line);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(882, 449);
		this.splitContainerControl1.SplitterPosition = 206;
		this.splitContainerControl1.TabIndex = 0;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.ContextMenuStrip = this.ctm_Doc;
		this.grd_Doc.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Doc.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(2);
		this.grd_Doc.Location = new System.Drawing.Point(0, 0);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Margin = new System.Windows.Forms.Padding(2);
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[25]
		{
			this.btn_BpCode, this.btn_Manager, this.btn_Owner, this.btn_SaleOder, this.btn_Project, this.luk_Type, this.luk_Status, this.luk_Industry, this.luk_Priority, this.btn_FinProject,
			this.txt_prjCode, this.txt_budgetLevel, this.repositoryItemMemoExEdit1, this.btn_Whs, this.luk_DocCurr, this.repositoryItemDateEditEx1, this.repositoryItemTimeSpanEdit1, this.repositoryItemDateEditEx2, this.repositoryItemTimeSpanEdit2, this.luk_WddStatus,
			this.look_LoadBom, this.rep_CommentsCount, this.btn_BaseProject, this.luk_BaseLine, this.luk_BaseLineActive
		});
		this.grd_Doc.Size = new System.Drawing.Size(882, 206);
		this.grd_Doc.TabIndex = 0;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.ctm_Doc.ImageScalingSize = new System.Drawing.Size(18, 18);
		this.ctm_Doc.Items.AddRange(new System.Windows.Forms.ToolStripItem[15]
		{
			this.addNewToolStripMenuItem, this.duplicateDoc, this.toolStripSeparator5, this.deleteToolStripMenuItem, this.toolStripSeparator4, this.addSubProjectToolStripMenuItem, this.toolStripSeparator6, this.newTaskInDoc, this.newEvenToolStripMenuItem, this.newTrackingIssueToolStripMenuItem,
			this.relatedActivitiesToolStripMenuItem, this.canceledToolStripMenuItem, this.closedToolStripMenuItem, this.viewDiagramToolStripMenuItem, this.duplicateToolStripMenuItem
		});
		this.ctm_Doc.Name = "contextMenuStrip1";
		this.ctm_Doc.Size = new System.Drawing.Size(202, 286);
		this.addNewToolStripMenuItem.Name = "addNewToolStripMenuItem";
		this.addNewToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
		this.addNewToolStripMenuItem.Text = "Add New";
		this.addNewToolStripMenuItem.Click += new System.EventHandler(this.addNewToolStripMenuItem_Click);
		this.duplicateDoc.Name = "duplicateDoc";
		this.duplicateDoc.Size = new System.Drawing.Size(201, 22);
		this.duplicateDoc.Text = "Duplicate";
		this.duplicateDoc.Click += new System.EventHandler(this.duplicateDoc_Click);
		this.toolStripSeparator5.Name = "toolStripSeparator5";
		this.toolStripSeparator5.Size = new System.Drawing.Size(198, 6);
		this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
		this.deleteToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
		this.deleteToolStripMenuItem.Text = "Remove";
		this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
		this.toolStripSeparator4.Name = "toolStripSeparator4";
		this.toolStripSeparator4.Size = new System.Drawing.Size(198, 6);
		this.addSubProjectToolStripMenuItem.Name = "addSubProjectToolStripMenuItem";
		this.addSubProjectToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
		this.addSubProjectToolStripMenuItem.Text = "Define New Sub. Project";
		this.addSubProjectToolStripMenuItem.Click += new System.EventHandler(this.addSubProjectToolStripMenuItem_Click);
		this.toolStripSeparator6.Name = "toolStripSeparator6";
		this.toolStripSeparator6.Size = new System.Drawing.Size(198, 6);
		this.newTaskInDoc.Name = "newTaskInDoc";
		this.newTaskInDoc.Size = new System.Drawing.Size(201, 22);
		this.newTaskInDoc.Text = "New Task";
		this.newTaskInDoc.Click += new System.EventHandler(this.newTaskToolStripMenuItem_Click);
		this.newEvenToolStripMenuItem.Name = "newEvenToolStripMenuItem";
		this.newEvenToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
		this.newEvenToolStripMenuItem.Text = "New Event";
		this.newEvenToolStripMenuItem.Click += new System.EventHandler(this.newEvenToolStripMenuItem_Click);
		this.newTrackingIssueToolStripMenuItem.Name = "newTrackingIssueToolStripMenuItem";
		this.newTrackingIssueToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
		this.newTrackingIssueToolStripMenuItem.Text = "New Issue";
		this.newTrackingIssueToolStripMenuItem.Click += new System.EventHandler(this.newTrackingIssueToolStripMenuItem_Click);
		this.relatedActivitiesToolStripMenuItem.Name = "relatedActivitiesToolStripMenuItem";
		this.relatedActivitiesToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
		this.relatedActivitiesToolStripMenuItem.Text = "Related Activities";
		this.relatedActivitiesToolStripMenuItem.Click += new System.EventHandler(this.relatedActivitiesToolStripMenuItem_Click);
		this.canceledToolStripMenuItem.Name = "canceledToolStripMenuItem";
		this.canceledToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
		this.canceledToolStripMenuItem.Text = "Cancel";
		this.canceledToolStripMenuItem.Click += new System.EventHandler(this.canceledToolStripMenuItem_Click);
		this.closedToolStripMenuItem.Name = "closedToolStripMenuItem";
		this.closedToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
		this.closedToolStripMenuItem.Text = "Close";
		this.closedToolStripMenuItem.Click += new System.EventHandler(this.closedToolStripMenuItem_Click);
		this.viewDiagramToolStripMenuItem.Name = "viewDiagramToolStripMenuItem";
		this.viewDiagramToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
		this.viewDiagramToolStripMenuItem.Text = "View Diagram";
		this.viewDiagramToolStripMenuItem.Click += new System.EventHandler(this.viewDiagramToolStripMenuItem_Click);
		this.duplicateToolStripMenuItem.Name = "duplicateToolStripMenuItem";
		this.duplicateToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
		this.duplicateToolStripMenuItem.Text = "Creat baseline";
		this.duplicateToolStripMenuItem.Click += new System.EventHandler(this.DuplicateToolStripMenuItem_Click);
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = null;
		this.gv_Doc.ColumnPanelRowHeight = 0;
		this.gv_Doc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[40]
		{
			this.gridColumnEx23, this.gridColumnEx1, this.gridColumnEx2, this.gridColumnEx3, this.gridColumnEx4, this.gridColumnEx5, this.gridColumnEx6, this.gridColumnEx7, this.gridColumnEx8, this.gridColumnEx9,
			this.gridColumnEx10, this.gridColumnEx11, this.gc_priority, this.gridColumnEx14, this.gridColumnEx15, this.gridColumnEx16, this.gridColumnEx17, this.gridColumnEx18, this.gridColumnEx19, this.gridColumnEx60,
			this.gridColumnEx62, this.gridColumnEx63, this.gridColumnEx90, this.gridColumnEx24, this.gridColumnEx30, this.gridColumnEx31, this.gridColumnEx33, this.gridColumnEx34, this.gridColumnEx35, this.gridColumnEx36,
			this.gridColumnEx37, this.gridColumnEx38, this.gridColumnEx12, this.gridColumnEx20, this.gridColumnEx21, this.gv_LoadBom, this.gridColumnEx22, this.gridColumnEx101, this.gridColumnEx102, this.gridColumnEx104
		});
		this.gv_Doc.DetailHeight = 364;
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FooterPanelHeight = 0;
		gridFormatRule.Column = this.gc_priority;
		gridFormatRule.ColumnApplyTo = this.gc_priority;
		gridFormatRule.Name = "Format0";
		formatConditionIconSet.CategoryName = "Shapes";
		formatConditionIconSetIcon.PredefinedName = "RedToBlack4_1.png";
		formatConditionIconSetIcon.Value = new decimal(new int[4] { 4, 0, 0, 0 });
		formatConditionIconSetIcon.ValueComparison = DevExpress.XtraEditors.FormatConditionComparisonType.GreaterOrEqual;
		formatConditionIconSetIcon2.PredefinedName = "TrafficLights4_2.png";
		formatConditionIconSetIcon2.Value = new decimal(new int[4] { 3, 0, 0, 0 });
		formatConditionIconSetIcon2.ValueComparison = DevExpress.XtraEditors.FormatConditionComparisonType.GreaterOrEqual;
		formatConditionIconSetIcon3.PredefinedName = "TrafficLights4_4.png";
		formatConditionIconSetIcon3.Value = new decimal(new int[4] { 2, 0, 0, 0 });
		formatConditionIconSetIcon3.ValueComparison = DevExpress.XtraEditors.FormatConditionComparisonType.GreaterOrEqual;
		formatConditionIconSetIcon4.PredefinedName = "TrafficLights4_1.png";
		formatConditionIconSetIcon4.Value = new decimal(new int[4] { 1, 0, 0, 0 });
		formatConditionIconSetIcon4.ValueComparison = DevExpress.XtraEditors.FormatConditionComparisonType.GreaterOrEqual;
		formatConditionIconSet.Icons.Add(formatConditionIconSetIcon);
		formatConditionIconSet.Icons.Add(formatConditionIconSetIcon2);
		formatConditionIconSet.Icons.Add(formatConditionIconSetIcon3);
		formatConditionIconSet.Icons.Add(formatConditionIconSetIcon4);
		formatConditionIconSet.Name = "RedToBlack";
		formatConditionRuleIconSet.IconSet = formatConditionIconSet;
		gridFormatRule.Rule = formatConditionRuleIconSet;
		this.gv_Doc.FormatRules.Add(gridFormatRule);
		this.gv_Doc.FormId = "oProjectManagement";
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.GroupRowHeight = 0;
		this.gv_Doc.HasFormSetting = true;
		this.gv_Doc.IgnoreFilter = false;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 35;
		this.gv_Doc.IsApzTable = false;
		this.gv_Doc.IsInsertNewRow = false;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.IsMappingContextAction = false;
		this.gv_Doc.ItemId = "APZ_OPRG";
		this.gv_Doc.LevelIndent = 0;
		this.gv_Doc.LstColumnAllowBatch = null;
		this.gv_Doc.LstColumnNotAllowBatch = null;
		this.gv_Doc.LstColumnNotEditable = null;
		this.gv_Doc.LstColumNotSetting = new System.Collections.Generic.List<string> { "OwnerAuth" };
		this.gv_Doc.LstOldUDF = null;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Doc.OptionsView.ColumnAutoWidth = false;
		this.gv_Doc.OptionsView.ShowGroupPanel = false;
		this.gv_Doc.PreviewIndent = 0;
		this.gv_Doc.ReadOnly = false;
		this.gv_Doc.RowHeight = 0;
		this.gv_Doc.SettingReadonly = false;
		this.gv_Doc.UseBatchMod = false;
		this.gv_Doc.ViewCaptionHeight = 0;
		this.gv_Doc.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Doc_RowCellStyle);
		this.gv_Doc.CustomRowCellEditForEditing += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gv_Doc_CustomRowCellEditForEditing);
		this.gv_Doc.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_Doc_ShowingEditor);
		this.gv_Doc.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_Doc_FocusedRowObjectChanged);
		this.gv_Doc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Doc_CellValueChanged);
		this.gv_Doc.BeforeLeaveRow += new DevExpress.XtraGrid.Views.Base.RowAllowEventHandler(this.gv_Doc_BeforeLeaveRow);
		this.gv_Doc.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.gv_Doc_CustomColumnDisplayText);
		this.gv_Doc.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gv_Doc_MouseMove);
		this.gv_Doc.MouseLeave += new System.EventHandler(this.gv_Doc_MouseLeave);
		this.gv_Doc.ValidatingEditor += new DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventHandler(this.gv_Doc_ValidatingEditor);
		this.gridColumnEx23.Caption = "gridColumnEx23";
		this.gridColumnEx23.CurrencyDateField = null;
		this.gridColumnEx23.CurrencyField = null;
		this.gridColumnEx23.CurrencyFixed = null;
		this.gridColumnEx23.CurrencyFixedRate = 0.0;
		this.gridColumnEx23.CurrencyRateField = null;
		this.gridColumnEx23.FieldName = "OwnerAuth";
		this.gridColumnEx23.Name = "gridColumnEx23";
		this.gridColumnEx23.PrimaryKey = false;
		this.gridColumnEx23.UnitField = null;
		this.gridColumnEx1.Caption = "AbsEntry";
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "AbsEntry";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.OptionsColumn.AllowEdit = false;
		this.gridColumnEx1.OptionsColumn.ReadOnly = true;
		this.gridColumnEx1.PrimaryKey = false;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx2.Caption = "Project Code";
		this.gridColumnEx2.ColumnEdit = this.txt_prjCode;
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.FieldName = "Code";
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.UnitField = null;
		this.gridColumnEx2.Width = 100;
		this.txt_prjCode.AutoHeight = false;
		this.txt_prjCode.Name = "txt_prjCode";
		this.gridColumnEx3.Caption = "Project Name";
		this.gridColumnEx3.CurrencyDateField = null;
		this.gridColumnEx3.CurrencyField = null;
		this.gridColumnEx3.CurrencyFixed = null;
		this.gridColumnEx3.CurrencyFixedRate = 0.0;
		this.gridColumnEx3.CurrencyRateField = null;
		this.gridColumnEx3.FieldName = "Name";
		this.gridColumnEx3.Name = "gridColumnEx3";
		this.gridColumnEx3.PrimaryKey = false;
		this.gridColumnEx3.UnitField = null;
		this.gridColumnEx3.Width = 110;
		this.gridColumnEx4.Caption = "Type";
		this.gridColumnEx4.ColumnEdit = this.luk_Type;
		this.gridColumnEx4.CurrencyDateField = null;
		this.gridColumnEx4.CurrencyField = null;
		this.gridColumnEx4.CurrencyFixed = null;
		this.gridColumnEx4.CurrencyFixedRate = 0.0;
		this.gridColumnEx4.CurrencyRateField = null;
		this.gridColumnEx4.FieldName = "ProjectType";
		this.gridColumnEx4.Name = "gridColumnEx4";
		this.gridColumnEx4.PrimaryKey = false;
		this.gridColumnEx4.UnitField = null;
		this.luk_Type.AutoHeight = false;
		this.luk_Type.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_Type.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Type.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.luk_Type.IsSearchControl = false;
		this.luk_Type.Name = "luk_Type";
		this.luk_Type.NullText = "";
		this.luk_Type.ShowFooter = false;
		this.luk_Type.ShowHeader = false;
		this.gridColumnEx5.Caption = "Status";
		this.gridColumnEx5.ColumnEdit = this.luk_Status;
		this.gridColumnEx5.CurrencyDateField = null;
		this.gridColumnEx5.CurrencyField = null;
		this.gridColumnEx5.CurrencyFixed = null;
		this.gridColumnEx5.CurrencyFixedRate = 0.0;
		this.gridColumnEx5.CurrencyRateField = null;
		this.gridColumnEx5.FieldName = "PrgStatus";
		this.gridColumnEx5.Name = "gridColumnEx5";
		this.gridColumnEx5.PrimaryKey = false;
		this.gridColumnEx5.UnitField = null;
		this.luk_Status.AutoHeight = false;
		this.luk_Status.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_Status.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Status.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.luk_Status.IsSearchControl = false;
		this.luk_Status.Name = "luk_Status";
		this.luk_Status.NullText = "";
		this.luk_Status.ShowFooter = false;
		this.luk_Status.ShowHeader = false;
		this.gridColumnEx6.Caption = "BP Code";
		this.gridColumnEx6.ColumnEdit = this.btn_BpCode;
		this.gridColumnEx6.CurrencyDateField = null;
		this.gridColumnEx6.CurrencyField = null;
		this.gridColumnEx6.CurrencyFixed = null;
		this.gridColumnEx6.CurrencyFixedRate = 0.0;
		this.gridColumnEx6.CurrencyRateField = null;
		this.gridColumnEx6.FieldName = "CardCode";
		this.gridColumnEx6.Name = "gridColumnEx6";
		this.gridColumnEx6.PrimaryKey = false;
		this.gridColumnEx6.UnitField = null;
		this.btn_BpCode.AutoHeight = false;
		this.btn_BpCode.AutoLoadDataSource = true;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.btn_BpCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_BpCode.ContainerControl = this;
		this.btn_BpCode.DisplayMember = "CardCode";
		this.btn_BpCode.DisplayMember2 = "";
		this.btn_BpCode.ListMandatoryField = null;
		this.btn_BpCode.LoadDataSourceWhenEnter = false;
		this.btn_BpCode.Name = "btn_BpCode";
		this.btn_BpCode.SeparateCharacterForMulti = ';';
		this.btn_BpCode.SeparateString = ", ";
		this.btn_BpCode.UsingAutoCompleteBox = false;
		this.btn_BpCode.UsingDataSource = true;
		this.btn_BpCode.ValidateWhenNoDataSource = false;
		this.btn_BpCode.ValueMember = "CardCode";
		this.gridColumnEx7.Caption = "BP Name";
		this.gridColumnEx7.CurrencyDateField = null;
		this.gridColumnEx7.CurrencyField = null;
		this.gridColumnEx7.CurrencyFixed = null;
		this.gridColumnEx7.CurrencyFixedRate = 0.0;
		this.gridColumnEx7.CurrencyRateField = null;
		this.gridColumnEx7.FieldName = "CardName";
		this.gridColumnEx7.Name = "gridColumnEx7";
		this.gridColumnEx7.PrimaryKey = false;
		this.gridColumnEx7.UnitField = null;
		this.gridColumnEx8.Caption = "Project Manager";
		this.gridColumnEx8.ColumnEdit = this.btn_Manager;
		this.gridColumnEx8.CurrencyDateField = null;
		this.gridColumnEx8.CurrencyField = null;
		this.gridColumnEx8.CurrencyFixed = null;
		this.gridColumnEx8.CurrencyFixedRate = 0.0;
		this.gridColumnEx8.CurrencyRateField = null;
		this.gridColumnEx8.FieldName = "PM";
		this.gridColumnEx8.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx8.Name = "gridColumnEx8";
		this.gridColumnEx8.PrimaryKey = false;
		this.gridColumnEx8.UnitField = null;
		this.gridColumnEx8.Width = 120;
		this.btn_Manager.AutoHeight = false;
		this.btn_Manager.AutoLoadDataSource = true;
		this.btn_Manager.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_Manager.ContainerControl = this;
		this.btn_Manager.DisplayMember = "lastName";
		this.btn_Manager.DisplayMember2 = "firstName";
		this.btn_Manager.ListMandatoryField = null;
		this.btn_Manager.LoadDataSourceWhenEnter = false;
		this.btn_Manager.Name = "btn_Manager";
		this.btn_Manager.SeparateCharacterForMulti = ';';
		this.btn_Manager.SeparateString = " ";
		this.btn_Manager.UsingAutoCompleteBox = false;
		this.btn_Manager.UsingDataSource = true;
		this.btn_Manager.ValidateWhenNoDataSource = false;
		this.btn_Manager.ValueMember = "EmpId";
		this.gridColumnEx9.Caption = "Start Date";
		this.gridColumnEx9.CurrencyDateField = null;
		this.gridColumnEx9.CurrencyField = null;
		this.gridColumnEx9.CurrencyFixed = null;
		this.gridColumnEx9.CurrencyFixedRate = 0.0;
		this.gridColumnEx9.CurrencyRateField = null;
		this.gridColumnEx9.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gridColumnEx9.FieldName = "StartDate";
		this.gridColumnEx9.Name = "gridColumnEx9";
		this.gridColumnEx9.PrimaryKey = false;
		this.gridColumnEx9.UnitField = null;
		this.gridColumnEx9.Width = 100;
		this.gridColumnEx10.Caption = "Due Date";
		this.gridColumnEx10.CurrencyDateField = null;
		this.gridColumnEx10.CurrencyField = null;
		this.gridColumnEx10.CurrencyFixed = null;
		this.gridColumnEx10.CurrencyFixedRate = 0.0;
		this.gridColumnEx10.CurrencyRateField = null;
		this.gridColumnEx10.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gridColumnEx10.FieldName = "DueDate";
		this.gridColumnEx10.Name = "gridColumnEx10";
		this.gridColumnEx10.PrimaryKey = false;
		this.gridColumnEx10.UnitField = null;
		this.gridColumnEx11.Caption = "Actual Closed Date";
		this.gridColumnEx11.CurrencyDateField = null;
		this.gridColumnEx11.CurrencyField = null;
		this.gridColumnEx11.CurrencyFixed = null;
		this.gridColumnEx11.CurrencyFixedRate = 0.0;
		this.gridColumnEx11.CurrencyRateField = null;
		this.gridColumnEx11.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gridColumnEx11.FieldName = "ActualDate";
		this.gridColumnEx11.Name = "gridColumnEx11";
		this.gridColumnEx11.PrimaryKey = false;
		this.gridColumnEx11.UnitField = null;
		this.gridColumnEx11.Width = 150;
		this.gridColumnEx14.Caption = "Master Project";
		this.gridColumnEx14.ColumnEdit = this.btn_Project;
		this.gridColumnEx14.CurrencyDateField = null;
		this.gridColumnEx14.CurrencyField = null;
		this.gridColumnEx14.CurrencyFixed = null;
		this.gridColumnEx14.CurrencyFixedRate = 0.0;
		this.gridColumnEx14.CurrencyRateField = null;
		this.gridColumnEx14.FieldName = "MasterEntry";
		this.gridColumnEx14.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx14.Name = "gridColumnEx14";
		this.gridColumnEx14.PrimaryKey = false;
		this.gridColumnEx14.UnitField = null;
		this.gridColumnEx14.Width = 100;
		this.btn_Project.AutoHeight = false;
		this.btn_Project.AutoLoadDataSource = true;
		editorButtonImageOptions2.Image = Apzon.Forms.Properties.Resources.right;
		this.btn_Project.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_Project.ContainerControl = this;
		this.btn_Project.DisplayMember = "Name";
		this.btn_Project.DisplayMember2 = "";
		this.btn_Project.ListMandatoryField = null;
		this.btn_Project.LoadDataSourceWhenEnter = false;
		this.btn_Project.Name = "btn_Project";
		this.btn_Project.SeparateCharacterForMulti = ';';
		this.btn_Project.SeparateString = ", ";
		this.btn_Project.UsingAutoCompleteBox = false;
		this.btn_Project.UsingDataSource = true;
		this.btn_Project.ValidateWhenNoDataSource = false;
		this.btn_Project.ValueMember = "AbsEntry";
		this.gridColumnEx15.Caption = "Risk Level";
		this.gridColumnEx15.CurrencyDateField = null;
		this.gridColumnEx15.CurrencyField = null;
		this.gridColumnEx15.CurrencyFixed = null;
		this.gridColumnEx15.CurrencyFixedRate = 0.0;
		this.gridColumnEx15.CurrencyRateField = null;
		this.gridColumnEx15.FieldName = "RiskLvl";
		this.gridColumnEx15.Name = "gridColumnEx15";
		this.gridColumnEx15.PrimaryKey = false;
		this.gridColumnEx15.UnitField = null;
		this.gridColumnEx15.Width = 100;
		this.gridColumnEx16.Caption = "Industry";
		this.gridColumnEx16.ColumnEdit = this.luk_Industry;
		this.gridColumnEx16.CurrencyDateField = null;
		this.gridColumnEx16.CurrencyField = null;
		this.gridColumnEx16.CurrencyFixed = null;
		this.gridColumnEx16.CurrencyFixedRate = 0.0;
		this.gridColumnEx16.CurrencyRateField = null;
		this.gridColumnEx16.FieldName = "IndCode";
		this.gridColumnEx16.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx16.Name = "gridColumnEx16";
		this.gridColumnEx16.PrimaryKey = false;
		this.gridColumnEx16.UnitField = null;
		this.luk_Industry.AutoHeight = false;
		this.luk_Industry.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_Industry.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, imageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance4, appearanceHovered4, appearancePressed4, appearanceDisabled4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.luk_Industry.IsSearchControl = false;
		this.luk_Industry.Name = "luk_Industry";
		this.luk_Industry.NullText = "";
		this.luk_Industry.ShowFooter = false;
		this.luk_Industry.ShowHeader = false;
		this.luk_Industry.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.luk_Industry_ButtonPressed);
		this.gridColumnEx17.Caption = "Project Owner";
		this.gridColumnEx17.ColumnEdit = this.btn_Owner;
		this.gridColumnEx17.CurrencyDateField = null;
		this.gridColumnEx17.CurrencyField = null;
		this.gridColumnEx17.CurrencyFixed = null;
		this.gridColumnEx17.CurrencyFixedRate = 0.0;
		this.gridColumnEx17.CurrencyRateField = null;
		this.gridColumnEx17.FieldName = "Owner";
		this.gridColumnEx17.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx17.Name = "gridColumnEx17";
		this.gridColumnEx17.PrimaryKey = false;
		this.gridColumnEx17.UnitField = null;
		this.gridColumnEx17.Width = 100;
		this.btn_Owner.AutoHeight = false;
		this.btn_Owner.AutoLoadDataSource = true;
		editorButtonImageOptions3.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions9.Image");
		this.btn_Owner.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance5, appearanceHovered5, appearancePressed5, appearanceDisabled5, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_Owner.ContainerControl = this;
		this.btn_Owner.DisplayMember = "lastName";
		this.btn_Owner.DisplayMember2 = "firstName";
		this.btn_Owner.ListMandatoryField = null;
		this.btn_Owner.LoadDataSourceWhenEnter = false;
		this.btn_Owner.Name = "btn_Owner";
		this.btn_Owner.SeparateCharacterForMulti = ';';
		this.btn_Owner.SeparateString = " ";
		this.btn_Owner.UsingAutoCompleteBox = false;
		this.btn_Owner.UsingDataSource = true;
		this.btn_Owner.ValidateWhenNoDataSource = false;
		this.btn_Owner.ValueMember = "EmpId";
		this.gridColumnEx18.Caption = "Sales Order";
		this.gridColumnEx18.ColumnEdit = this.btn_SaleOder;
		this.gridColumnEx18.CurrencyDateField = null;
		this.gridColumnEx18.CurrencyField = null;
		this.gridColumnEx18.CurrencyFixed = null;
		this.gridColumnEx18.CurrencyFixedRate = 0.0;
		this.gridColumnEx18.CurrencyRateField = null;
		this.gridColumnEx18.FieldName = "Order";
		this.gridColumnEx18.Name = "gridColumnEx18";
		this.gridColumnEx18.PrimaryKey = false;
		this.gridColumnEx18.UnitField = null;
		this.gridColumnEx18.Width = 100;
		this.btn_SaleOder.AutoHeight = false;
		this.btn_SaleOder.AutoLoadDataSource = true;
		this.btn_SaleOder.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, imageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance6, appearanceHovered6, appearancePressed6, appearanceDisabled6, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_SaleOder.ContainerControl = this;
		this.btn_SaleOder.DisplayMember = "DocNum";
		this.btn_SaleOder.DisplayMember2 = "";
		this.btn_SaleOder.ListMandatoryField = null;
		this.btn_SaleOder.LoadDataSourceWhenEnter = false;
		this.btn_SaleOder.Name = "btn_SaleOder";
		this.btn_SaleOder.SeparateCharacterForMulti = ';';
		this.btn_SaleOder.SeparateString = ", ";
		this.btn_SaleOder.UsingAutoCompleteBox = false;
		this.btn_SaleOder.UsingDataSource = true;
		this.btn_SaleOder.ValidateWhenNoDataSource = false;
		this.btn_SaleOder.ValueMember = "DocEntry";
		this.btn_SaleOder.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btn_SaleOder_ButtonClick);
		this.gridColumnEx19.Caption = "Remark";
		this.gridColumnEx19.ColumnEdit = this.repositoryItemMemoExEdit1;
		this.gridColumnEx19.CurrencyDateField = null;
		this.gridColumnEx19.CurrencyField = null;
		this.gridColumnEx19.CurrencyFixed = null;
		this.gridColumnEx19.CurrencyFixedRate = 0.0;
		this.gridColumnEx19.CurrencyRateField = null;
		this.gridColumnEx19.FieldName = "Remark";
		this.gridColumnEx19.Name = "gridColumnEx19";
		this.gridColumnEx19.PrimaryKey = false;
		this.gridColumnEx19.UnitField = null;
		this.repositoryItemMemoExEdit1.AutoHeight = false;
		this.repositoryItemMemoExEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemMemoExEdit1.Name = "repositoryItemMemoExEdit1";
		this.repositoryItemMemoExEdit1.ShowIcon = false;
		this.gridColumnEx60.Caption = "Budget Amount";
		this.gridColumnEx60.ColumnEdit = this.repositoryItemCalcEditEx1;
		this.gridColumnEx60.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gridColumnEx60.CurrencyDateField = null;
		this.gridColumnEx60.CurrencyField = "DocCur";
		this.gridColumnEx60.CurrencyFixed = null;
		this.gridColumnEx60.CurrencyFixedRate = 0.0;
		this.gridColumnEx60.CurrencyRateField = null;
		this.gridColumnEx60.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gridColumnEx60.FieldName = "BudgetAmount";
		this.gridColumnEx60.Name = "gridColumnEx60";
		this.gridColumnEx60.OptionsColumn.ReadOnly = true;
		this.gridColumnEx60.PrimaryKey = false;
		this.gridColumnEx60.UnitField = null;
		this.gridColumnEx60.Width = 100;
		this.gridColumnEx62.Caption = "Actual Amount";
		this.gridColumnEx62.ColumnEdit = this.repositoryItemCalcEditEx2;
		this.gridColumnEx62.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gridColumnEx62.CurrencyDateField = null;
		this.gridColumnEx62.CurrencyField = "DocCur";
		this.gridColumnEx62.CurrencyFixed = null;
		this.gridColumnEx62.CurrencyFixedRate = 0.0;
		this.gridColumnEx62.CurrencyRateField = null;
		this.gridColumnEx62.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gridColumnEx62.FieldName = "ActualAmount";
		this.gridColumnEx62.Name = "gridColumnEx62";
		this.gridColumnEx62.OptionsColumn.ReadOnly = true;
		this.gridColumnEx62.PrimaryKey = false;
		this.gridColumnEx62.UnitField = null;
		this.gridColumnEx62.Width = 100;
		this.gridColumnEx63.Caption = "Amount Diff ";
		this.gridColumnEx63.ColumnEdit = this.repositoryItemCalcEditEx3;
		this.gridColumnEx63.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gridColumnEx63.CurrencyDateField = null;
		this.gridColumnEx63.CurrencyField = "DocCur";
		this.gridColumnEx63.CurrencyFixed = null;
		this.gridColumnEx63.CurrencyFixedRate = 0.0;
		this.gridColumnEx63.CurrencyRateField = null;
		this.gridColumnEx63.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gridColumnEx63.FieldName = "AmountDiff";
		this.gridColumnEx63.Name = "gridColumnEx63";
		this.gridColumnEx63.OptionsColumn.ReadOnly = true;
		this.gridColumnEx63.PrimaryKey = false;
		this.gridColumnEx63.UnitField = null;
		this.gridColumnEx63.Width = 100;
		this.gridColumnEx90.Caption = "Financial Project";
		this.gridColumnEx90.ColumnEdit = this.btn_FinProject;
		this.gridColumnEx90.CurrencyDateField = null;
		this.gridColumnEx90.CurrencyField = null;
		this.gridColumnEx90.CurrencyFixed = null;
		this.gridColumnEx90.CurrencyFixedRate = 0.0;
		this.gridColumnEx90.CurrencyRateField = null;
		this.gridColumnEx90.FieldName = "FinProject";
		this.gridColumnEx90.Name = "gridColumnEx90";
		this.gridColumnEx90.PrimaryKey = false;
		this.gridColumnEx90.UnitField = null;
		this.gridColumnEx90.Width = 100;
		this.btn_FinProject.AutoHeight = false;
		this.btn_FinProject.AutoLoadDataSource = true;
		editorButtonImageOptions4.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions12.Image");
		this.btn_FinProject.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance7, appearanceHovered7, appearancePressed7, appearanceDisabled7, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance8, appearanceHovered8, appearancePressed8, appearanceDisabled8, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_FinProject.ContainerControl = this;
		this.btn_FinProject.DisplayMember = "";
		this.btn_FinProject.DisplayMember2 = "";
		this.btn_FinProject.ListMandatoryField = null;
		this.btn_FinProject.LoadDataSourceWhenEnter = false;
		this.btn_FinProject.Name = "btn_FinProject";
		this.btn_FinProject.SeparateCharacterForMulti = ';';
		this.btn_FinProject.SeparateString = ", ";
		this.btn_FinProject.UsingAutoCompleteBox = false;
		this.btn_FinProject.UsingDataSource = true;
		this.btn_FinProject.ValidateWhenNoDataSource = false;
		this.btn_FinProject.ValueMember = "PrjCode";
		this.gridColumnEx24.Caption = "Budget Level";
		this.gridColumnEx24.ColumnEdit = this.txt_budgetLevel;
		this.gridColumnEx24.CurrencyDateField = null;
		this.gridColumnEx24.CurrencyField = null;
		this.gridColumnEx24.CurrencyFixed = null;
		this.gridColumnEx24.CurrencyFixedRate = 0.0;
		this.gridColumnEx24.CurrencyRateField = null;
		this.gridColumnEx24.FieldName = "BudgetLevel";
		this.gridColumnEx24.Name = "gridColumnEx24";
		this.gridColumnEx24.PrimaryKey = false;
		this.gridColumnEx24.UnitField = null;
		this.txt_budgetLevel.AutoHeight = false;
		this.txt_budgetLevel.Name = "txt_budgetLevel";
		this.gridColumnEx30.Caption = "Default Whs";
		this.gridColumnEx30.ColumnEdit = this.btn_Whs;
		this.gridColumnEx30.CurrencyDateField = null;
		this.gridColumnEx30.CurrencyField = null;
		this.gridColumnEx30.CurrencyFixed = null;
		this.gridColumnEx30.CurrencyFixedRate = 0.0;
		this.gridColumnEx30.CurrencyRateField = null;
		this.gridColumnEx30.FieldName = "DefaultWhs";
		this.gridColumnEx30.Name = "gridColumnEx30";
		this.gridColumnEx30.PrimaryKey = false;
		this.gridColumnEx30.UnitField = null;
		this.btn_Whs.AutoHeight = false;
		this.btn_Whs.AutoLoadDataSource = true;
		editorButtonImageOptions5.Image = Apzon.Forms.Properties.Resources.right;
		this.btn_Whs.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions5, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance9, appearanceHovered9, appearancePressed9, appearanceDisabled9, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions5, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance10, appearanceHovered10, appearancePressed10, appearanceDisabled10, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_Whs.ContainerControl = this;
		this.btn_Whs.DisplayMember = "WhsCode";
		this.btn_Whs.DisplayMember2 = "";
		this.btn_Whs.ListMandatoryField = null;
		this.btn_Whs.LoadDataSourceWhenEnter = false;
		this.btn_Whs.Name = "btn_Whs";
		this.btn_Whs.SeparateCharacterForMulti = ';';
		this.btn_Whs.SeparateString = ", ";
		this.btn_Whs.UsingAutoCompleteBox = false;
		this.btn_Whs.UsingDataSource = true;
		this.btn_Whs.ValidateWhenNoDataSource = false;
		this.btn_Whs.ValueMember = "WhsCode";
		this.gridColumnEx31.Caption = "Doc Currency";
		this.gridColumnEx31.ColumnEdit = this.luk_DocCurr;
		this.gridColumnEx31.CurrencyDateField = null;
		this.gridColumnEx31.CurrencyField = null;
		this.gridColumnEx31.CurrencyFixed = null;
		this.gridColumnEx31.CurrencyFixedRate = 0.0;
		this.gridColumnEx31.CurrencyRateField = null;
		this.gridColumnEx31.FieldName = "DocCur";
		this.gridColumnEx31.Name = "gridColumnEx31";
		this.gridColumnEx31.PrimaryKey = false;
		this.gridColumnEx31.UnitField = null;
		this.luk_DocCurr.AutoHeight = false;
		this.luk_DocCurr.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_DocCurr.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("CurrCode", "Code")
		});
		this.luk_DocCurr.DisplayMember = "CurrCode";
		this.luk_DocCurr.Name = "luk_DocCurr";
		this.luk_DocCurr.NullText = "";
		this.luk_DocCurr.ValueMember = "CurrCode";
		this.gridColumnEx33.Caption = "UserCodeSign";
		this.gridColumnEx33.CurrencyDateField = null;
		this.gridColumnEx33.CurrencyField = null;
		this.gridColumnEx33.CurrencyFixed = null;
		this.gridColumnEx33.CurrencyFixedRate = 0.0;
		this.gridColumnEx33.CurrencyRateField = null;
		this.gridColumnEx33.FieldName = "UserCodeSign";
		this.gridColumnEx33.Name = "gridColumnEx33";
		this.gridColumnEx33.OptionsColumn.ReadOnly = true;
		this.gridColumnEx33.PrimaryKey = false;
		this.gridColumnEx33.UnitField = null;
		this.gridColumnEx34.Caption = "UserCodeSign2";
		this.gridColumnEx34.CurrencyDateField = null;
		this.gridColumnEx34.CurrencyField = null;
		this.gridColumnEx34.CurrencyFixed = null;
		this.gridColumnEx34.CurrencyFixedRate = 0.0;
		this.gridColumnEx34.CurrencyRateField = null;
		this.gridColumnEx34.FieldName = "UserCodeSign2";
		this.gridColumnEx34.Name = "gridColumnEx34";
		this.gridColumnEx34.OptionsColumn.ReadOnly = true;
		this.gridColumnEx34.PrimaryKey = false;
		this.gridColumnEx34.UnitField = null;
		this.gridColumnEx35.Caption = "CreateDate";
		this.gridColumnEx35.ColumnEdit = this.repositoryItemDateEditEx1;
		this.gridColumnEx35.CurrencyDateField = null;
		this.gridColumnEx35.CurrencyField = null;
		this.gridColumnEx35.CurrencyFixed = null;
		this.gridColumnEx35.CurrencyFixedRate = 0.0;
		this.gridColumnEx35.CurrencyRateField = null;
		this.gridColumnEx35.FieldName = "CreateDate";
		this.gridColumnEx35.Name = "gridColumnEx35";
		this.gridColumnEx35.OptionsColumn.ReadOnly = true;
		this.gridColumnEx35.PrimaryKey = false;
		this.gridColumnEx35.UnitField = null;
		this.repositoryItemDateEditEx1.AutoHeight = false;
		this.repositoryItemDateEditEx1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEditEx1.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEditEx1.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.repositoryItemDateEditEx1.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.repositoryItemDateEditEx1.EditFormat.FormatString = "dd/MM/yyyy";
		this.repositoryItemDateEditEx1.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.repositoryItemDateEditEx1.IsSearchControl = false;
		this.repositoryItemDateEditEx1.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.repositoryItemDateEditEx1.Name = "repositoryItemDateEditEx1";
		this.gridColumnEx36.Caption = "CreateTime";
		this.gridColumnEx36.ColumnEdit = this.repositoryItemTimeSpanEdit1;
		this.gridColumnEx36.CurrencyDateField = null;
		this.gridColumnEx36.CurrencyField = null;
		this.gridColumnEx36.CurrencyFixed = null;
		this.gridColumnEx36.CurrencyFixedRate = 0.0;
		this.gridColumnEx36.CurrencyRateField = null;
		this.gridColumnEx36.FieldName = "CreateTime";
		this.gridColumnEx36.Name = "gridColumnEx36";
		this.gridColumnEx36.OptionsColumn.ReadOnly = true;
		this.gridColumnEx36.PrimaryKey = false;
		this.gridColumnEx36.UnitField = null;
		this.repositoryItemTimeSpanEdit1.AutoHeight = false;
		this.repositoryItemTimeSpanEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemTimeSpanEdit1.Mask.EditMask = "dd.HH:mm:ss";
		this.repositoryItemTimeSpanEdit1.Name = "repositoryItemTimeSpanEdit1";
		this.gridColumnEx37.Caption = "UpdateDate";
		this.gridColumnEx37.ColumnEdit = this.repositoryItemDateEditEx2;
		this.gridColumnEx37.CurrencyDateField = null;
		this.gridColumnEx37.CurrencyField = null;
		this.gridColumnEx37.CurrencyFixed = null;
		this.gridColumnEx37.CurrencyFixedRate = 0.0;
		this.gridColumnEx37.CurrencyRateField = null;
		this.gridColumnEx37.FieldName = "UpdateDate";
		this.gridColumnEx37.Name = "gridColumnEx37";
		this.gridColumnEx37.OptionsColumn.ReadOnly = true;
		this.gridColumnEx37.PrimaryKey = false;
		this.gridColumnEx37.UnitField = null;
		this.repositoryItemDateEditEx2.AutoHeight = false;
		this.repositoryItemDateEditEx2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEditEx2.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEditEx2.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.repositoryItemDateEditEx2.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.repositoryItemDateEditEx2.EditFormat.FormatString = "dd/MM/yyyy";
		this.repositoryItemDateEditEx2.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.repositoryItemDateEditEx2.IsSearchControl = false;
		this.repositoryItemDateEditEx2.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.repositoryItemDateEditEx2.Name = "repositoryItemDateEditEx2";
		this.gridColumnEx38.Caption = "UpdateTime";
		this.gridColumnEx38.ColumnEdit = this.repositoryItemTimeSpanEdit2;
		this.gridColumnEx38.CurrencyDateField = null;
		this.gridColumnEx38.CurrencyField = null;
		this.gridColumnEx38.CurrencyFixed = null;
		this.gridColumnEx38.CurrencyFixedRate = 0.0;
		this.gridColumnEx38.CurrencyRateField = null;
		this.gridColumnEx38.FieldName = "UpdateTime";
		this.gridColumnEx38.Name = "gridColumnEx38";
		this.gridColumnEx38.OptionsColumn.ReadOnly = true;
		this.gridColumnEx38.PrimaryKey = false;
		this.gridColumnEx38.UnitField = null;
		this.repositoryItemTimeSpanEdit2.AutoHeight = false;
		this.repositoryItemTimeSpanEdit2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemTimeSpanEdit2.Mask.EditMask = "dd.HH:mm:ss";
		this.repositoryItemTimeSpanEdit2.Name = "repositoryItemTimeSpanEdit2";
		this.gridColumnEx12.Caption = "Trạng thái phê duyệt";
		this.gridColumnEx12.ColumnEdit = this.luk_WddStatus;
		this.gridColumnEx12.CurrencyDateField = null;
		this.gridColumnEx12.CurrencyField = null;
		this.gridColumnEx12.CurrencyFixed = null;
		this.gridColumnEx12.CurrencyFixedRate = 0.0;
		this.gridColumnEx12.CurrencyRateField = null;
		this.gridColumnEx12.FieldName = "WddStatus";
		this.gridColumnEx12.Name = "gridColumnEx12";
		this.gridColumnEx12.PrimaryKey = false;
		this.gridColumnEx12.UnitField = null;
		this.luk_WddStatus.AutoHeight = false;
		this.luk_WddStatus.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_WddStatus.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_WddStatus.IsSearchControl = false;
		this.luk_WddStatus.Name = "luk_WddStatus";
		this.luk_WddStatus.NullText = "";
		this.luk_WddStatus.ShowFooter = false;
		this.luk_WddStatus.ShowHeader = false;
		this.gridColumnEx20.Caption = "Next approver";
		this.gridColumnEx20.CurrencyDateField = null;
		this.gridColumnEx20.CurrencyField = null;
		this.gridColumnEx20.CurrencyFixed = null;
		this.gridColumnEx20.CurrencyFixedRate = 0.0;
		this.gridColumnEx20.CurrencyRateField = null;
		this.gridColumnEx20.FieldName = "NextApprover";
		this.gridColumnEx20.Name = "gridColumnEx20";
		this.gridColumnEx20.PrimaryKey = false;
		this.gridColumnEx20.UnitField = null;
		this.gridColumnEx20.Width = 37;
		this.gridColumnEx21.Caption = "Approver";
		this.gridColumnEx21.CurrencyDateField = null;
		this.gridColumnEx21.CurrencyField = null;
		this.gridColumnEx21.CurrencyFixed = null;
		this.gridColumnEx21.CurrencyFixedRate = 0.0;
		this.gridColumnEx21.CurrencyRateField = null;
		this.gridColumnEx21.FieldName = "Approver";
		this.gridColumnEx21.Name = "gridColumnEx21";
		this.gridColumnEx21.PrimaryKey = false;
		this.gridColumnEx21.UnitField = null;
		this.gv_LoadBom.Caption = "Load Bom";
		this.gv_LoadBom.ColumnEdit = this.look_LoadBom;
		this.gv_LoadBom.CurrencyDateField = null;
		this.gv_LoadBom.CurrencyField = null;
		this.gv_LoadBom.CurrencyFixed = null;
		this.gv_LoadBom.CurrencyFixedRate = 0.0;
		this.gv_LoadBom.CurrencyRateField = null;
		this.gv_LoadBom.FieldName = "LoadBom";
		this.gv_LoadBom.Name = "gv_LoadBom";
		this.gv_LoadBom.PrimaryKey = false;
		this.gv_LoadBom.UnitField = null;
		this.look_LoadBom.AutoHeight = false;
		this.look_LoadBom.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.look_LoadBom.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.look_LoadBom.IsSearchControl = false;
		this.look_LoadBom.Name = "look_LoadBom";
		this.look_LoadBom.NullText = "";
		this.look_LoadBom.ShowFooter = false;
		this.look_LoadBom.ShowHeader = false;
		this.gridColumnEx22.Caption = "Comments";
		this.gridColumnEx22.ColumnEdit = this.rep_CommentsCount;
		this.gridColumnEx22.CurrencyDateField = null;
		this.gridColumnEx22.CurrencyField = null;
		this.gridColumnEx22.CurrencyFixed = null;
		this.gridColumnEx22.CurrencyFixedRate = 0.0;
		this.gridColumnEx22.CurrencyRateField = null;
		this.gridColumnEx22.FieldName = "CommentsCount";
		this.gridColumnEx22.Name = "gridColumnEx22";
		this.gridColumnEx22.PrimaryKey = false;
		this.gridColumnEx22.UnitField = null;
		this.rep_CommentsCount.AutoHeight = false;
		editorButtonImageOptions6.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("editorButtonImageOptions15.SvgImage");
		editorButtonImageOptions6.SvgImageSize = new System.Drawing.Size(16, 16);
		this.rep_CommentsCount.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, editorButtonImageOptions6, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance11, appearanceHovered11, appearancePressed11, appearanceDisabled11, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_CommentsCount.Name = "rep_CommentsCount";
		this.rep_CommentsCount.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_CommentsCount_ButtonClick);
		this.gridColumnEx101.Caption = "BaseProject";
		this.gridColumnEx101.CurrencyDateField = null;
		this.gridColumnEx101.CurrencyField = null;
		this.gridColumnEx101.CurrencyFixed = null;
		this.gridColumnEx101.CurrencyFixedRate = 0.0;
		this.gridColumnEx101.CurrencyRateField = null;
		this.gridColumnEx101.FieldName = "BaseProject";
		this.gridColumnEx101.Name = "gridColumnEx101";
		this.gridColumnEx101.OptionsColumn.AllowEdit = false;
		this.gridColumnEx101.OptionsColumn.ReadOnly = true;
		this.gridColumnEx101.PrimaryKey = false;
		this.gridColumnEx101.UnitField = null;
		this.gridColumnEx102.Caption = "BaseLine";
		this.gridColumnEx102.CurrencyDateField = null;
		this.gridColumnEx102.CurrencyField = null;
		this.gridColumnEx102.CurrencyFixed = null;
		this.gridColumnEx102.CurrencyFixedRate = 0.0;
		this.gridColumnEx102.CurrencyRateField = null;
		this.gridColumnEx102.FieldName = "BaseLine";
		this.gridColumnEx102.Name = "gridColumnEx102";
		this.gridColumnEx102.OptionsColumn.AllowEdit = false;
		this.gridColumnEx102.OptionsColumn.ReadOnly = true;
		this.gridColumnEx102.PrimaryKey = false;
		this.gridColumnEx102.UnitField = null;
		this.gridColumnEx104.Caption = "BaseLineActive";
		this.gridColumnEx104.CurrencyDateField = null;
		this.gridColumnEx104.CurrencyField = null;
		this.gridColumnEx104.CurrencyFixed = null;
		this.gridColumnEx104.CurrencyFixedRate = 0.0;
		this.gridColumnEx104.CurrencyRateField = null;
		this.gridColumnEx104.FieldName = "BaseLineActive";
		this.gridColumnEx104.Name = "gridColumnEx104";
		this.gridColumnEx104.OptionsColumn.AllowEdit = false;
		this.gridColumnEx104.OptionsColumn.ReadOnly = true;
		this.gridColumnEx104.PrimaryKey = false;
		this.gridColumnEx104.UnitField = null;
		this.btn_BaseProject.AutoHeight = false;
		this.btn_BaseProject.AutoLoadDataSource = true;
		editorButtonImageOptions7.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions17.Image");
		this.btn_BaseProject.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions6, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance12, appearanceHovered12, appearancePressed12, appearanceDisabled12, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions7, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance13, appearanceHovered13, appearancePressed13, appearanceDisabled13, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_BaseProject.ContainerControl = this;
		this.btn_BaseProject.DisplayMember = "";
		this.btn_BaseProject.DisplayMember2 = "";
		this.btn_BaseProject.ListMandatoryField = null;
		this.btn_BaseProject.LoadDataSourceWhenEnter = false;
		this.btn_BaseProject.Name = "btn_BaseProject";
		this.btn_BaseProject.SeparateCharacterForMulti = ';';
		this.btn_BaseProject.SeparateString = ", ";
		this.btn_BaseProject.UsingAutoCompleteBox = false;
		this.btn_BaseProject.UsingDataSource = true;
		this.btn_BaseProject.ValidateWhenNoDataSource = false;
		this.btn_BaseProject.ValueMember = "";
		this.luk_BaseLine.AutoHeight = false;
		this.luk_BaseLine.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_BaseLine.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_BaseLine.IsSearchControl = false;
		this.luk_BaseLine.Name = "luk_BaseLine";
		this.luk_BaseLine.NullText = "";
		this.luk_BaseLine.ShowFooter = false;
		this.luk_BaseLine.ShowHeader = false;
		this.luk_BaseLineActive.AutoHeight = false;
		this.luk_BaseLineActive.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_BaseLineActive.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_BaseLineActive.IsSearchControl = false;
		this.luk_BaseLineActive.Name = "luk_BaseLineActive";
		this.luk_BaseLineActive.NullText = "";
		this.luk_BaseLineActive.ShowFooter = false;
		this.luk_BaseLineActive.ShowHeader = false;
		this.xtra_Line.Dock = System.Windows.Forms.DockStyle.Fill;
		this.xtra_Line.IsSearchControl = false;
		this.xtra_Line.Location = new System.Drawing.Point(0, 0);
		this.xtra_Line.Margin = new System.Windows.Forms.Padding(2);
		this.xtra_Line.Name = "xtra_Line";
		this.xtra_Line.SelectedTabPage = this.tab_Task;
		this.xtra_Line.Size = new System.Drawing.Size(882, 233);
		this.xtra_Line.TabIndex = 0;
		this.xtra_Line.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[9] { this.tab_Task, this.xtra_Team, this.xtra_Budget, this.xtra_Document, this.xtra_Sub, this.xtra_Issue, this.tab_TaskList, this.xtra_NoteList, this.xtra_EventList });
		this.xtra_Line.SelectedPageChanged += new DevExpress.XtraTab.TabPageChangedEventHandler(this.xtra_Line_SelectedPageChanged);
		this.tab_Task.Controls.Add(this.splitContainerControl4);
		this.tab_Task.EditControl = null;
		this.tab_Task.EditValue = "Work Breakdown Structure";
		this.tab_Task.FieldName = "tab_Task";
		this.tab_Task.IsSearchControl = false;
		this.tab_Task.Margin = new System.Windows.Forms.Padding(2);
		this.tab_Task.Name = "tab_Task";
		this.tab_Task.ReadOnly = false;
		this.tab_Task.SettingInVisible = false;
		this.tab_Task.SettingReadonly = false;
		this.tab_Task.Size = new System.Drawing.Size(880, 208);
		this.tab_Task.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Task.Text = "Work Breakdown Structure";
		this.splitContainerControl4.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainerControl4.Location = new System.Drawing.Point(0, 0);
		this.splitContainerControl4.Margin = new System.Windows.Forms.Padding(2);
		this.splitContainerControl4.Name = "splitContainerControl4";
		this.splitContainerControl4.Panel1.Controls.Add(this.tv_Wbs);
		this.splitContainerControl4.Panel1.Text = "Panel1";
		this.splitContainerControl4.Panel2.Controls.Add(this.splitContainerControl5);
		this.splitContainerControl4.Panel2.Text = "Panel2";
		this.splitContainerControl4.Size = new System.Drawing.Size(880, 208);
		this.splitContainerControl4.SplitterPosition = 762;
		this.splitContainerControl4.TabIndex = 1;
		this.splitContainerControl4.Text = "splitContainerControl4";
		this.tv_Wbs.AllowDrop = true;
		this.tv_Wbs.AllowPasteExcel = true;
		this.tv_Wbs.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.tv_Wbs.ColumnKey = null;
		this.tv_Wbs.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[21]
		{
			this.tl_Entry, this.tl_Wbs, this.tl_Code, this.tl_Name, this.tl_FromDate, this.tl_ToDate, this.tl_Priority, this.tl_Milestone, this.tl_Notes, this.tl_Status,
			this.tl_Dependent, this.tl_Resource, this.tl_LineNum, this.tl_IsExist, this.tl_FatherNum, this.tl_visOrder, this.tl_BaseLine, this.tl_WbsTemplate, this.tl_WBSMode, this.tl_Duration,
			this.treeListColumnEx2
		});
		this.tv_Wbs.ContextMenuStrip = this.ctm_Rows;
		this.tv_Wbs.Cursor = System.Windows.Forms.Cursors.Default;
		this.tv_Wbs.DisableFormSetting = false;
		this.tv_Wbs.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tv_Wbs.EditColumn = null;
		this.tv_Wbs.FieldCurrency = null;
		this.tv_Wbs.FieldRate = null;
		treeListFormatRule.Column = this.tl_Priority;
		treeListFormatRule.ColumnApplyTo = this.tl_Priority;
		treeListFormatRule.Name = "Format0";
		formatConditionIconSet2.CategoryName = "Shapes";
		formatConditionIconSetIcon5.PredefinedName = "TrafficLights4_3.png";
		formatConditionIconSetIcon5.Value = new decimal(new int[4] { 4, 0, 0, 0 });
		formatConditionIconSetIcon5.ValueComparison = DevExpress.XtraEditors.FormatConditionComparisonType.GreaterOrEqual;
		formatConditionIconSetIcon6.PredefinedName = "TrafficLights23_2.png";
		formatConditionIconSetIcon6.Value = new decimal(new int[4] { 3, 0, 0, 0 });
		formatConditionIconSetIcon6.ValueComparison = DevExpress.XtraEditors.FormatConditionComparisonType.GreaterOrEqual;
		formatConditionIconSetIcon7.PredefinedName = "TrafficLights4_4.png";
		formatConditionIconSetIcon7.Value = new decimal(new int[4] { 2, 0, 0, 0 });
		formatConditionIconSetIcon7.ValueComparison = DevExpress.XtraEditors.FormatConditionComparisonType.GreaterOrEqual;
		formatConditionIconSetIcon8.PredefinedName = "TrafficLights4_1.png";
		formatConditionIconSetIcon8.ValueComparison = DevExpress.XtraEditors.FormatConditionComparisonType.GreaterOrEqual;
		formatConditionIconSet2.Icons.Add(formatConditionIconSetIcon5);
		formatConditionIconSet2.Icons.Add(formatConditionIconSetIcon6);
		formatConditionIconSet2.Icons.Add(formatConditionIconSetIcon7);
		formatConditionIconSet2.Icons.Add(formatConditionIconSetIcon8);
		formatConditionIconSet2.Name = "RedToBlack";
		formatConditionRuleIconSet2.IconSet = formatConditionIconSet2;
		treeListFormatRule.Rule = formatConditionRuleIconSet2;
		this.tv_Wbs.FormatRules.Add(treeListFormatRule);
		this.tv_Wbs.FormId = "oProjectManagement";
		this.tv_Wbs.HasFormSetting = true;
		this.tv_Wbs.IndexEditRow = 0;
		this.tv_Wbs.IndicatorWidth = 40;
		this.tv_Wbs.IsApzTable = false;
		this.tv_Wbs.IsLoadingData = false;
		this.tv_Wbs.ItemId = "APZ_PRG1";
		this.tv_Wbs.KeyFieldName = "LineNum";
		this.tv_Wbs.Location = new System.Drawing.Point(0, 0);
		this.tv_Wbs.LstColumnNotEditable = null;
		this.tv_Wbs.LstColumNotSetting = null;
		this.tv_Wbs.LstOldUDF = null;
		this.tv_Wbs.Margin = new System.Windows.Forms.Padding(2);
		this.tv_Wbs.Name = "tv_Wbs";
		this.tv_Wbs.OptionsBehavior.AutoChangeParent = false;
		this.tv_Wbs.OptionsBehavior.CloseEditorOnLostFocus = false;
		this.tv_Wbs.OptionsBehavior.PopulateServiceColumns = true;
		this.tv_Wbs.OptionsBehavior.ShowToolTips = false;
		this.tv_Wbs.OptionsBehavior.SmartMouseHover = false;
		this.tv_Wbs.OptionsDragAndDrop.AcceptOuterNodes = true;
		this.tv_Wbs.OptionsDragAndDrop.CanCloneNodesOnDrop = true;
		this.tv_Wbs.OptionsDragAndDrop.DragNodesMode = DevExpress.XtraTreeList.DragNodesMode.Multiple;
		this.tv_Wbs.OptionsDragAndDrop.DropNodesMode = DevExpress.XtraTreeList.DropNodesMode.Advanced;
		this.tv_Wbs.OptionsSelection.KeepSelectedOnClick = false;
		this.tv_Wbs.OptionsSelection.SelectNodesOnRightClick = true;
		this.tv_Wbs.OptionsView.AutoWidth = false;
		this.tv_Wbs.OptionsView.EnableAppearanceEvenRow = true;
		this.tv_Wbs.OptionsView.EnableAppearanceOddRow = true;
		this.tv_Wbs.ParentFieldName = "FatherNum";
		this.tv_Wbs.ReadOnly = false;
		this.tv_Wbs.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[12]
		{
			this.btn_resource, this.luk_taskStatus, this.luk_taskPriority, this.luk_Milestone, this.btn_NewTask, this.txt_TaskCode, this.date_fromdate, this.date_todate, this.repositoryItemMemoExEdit2, this.btn_WBSTemplate,
			this.luk_WBSMode, this.rep_WbsComments
		});
		this.tv_Wbs.SettingReadonly = false;
		this.tv_Wbs.Size = new System.Drawing.Size(762, 208);
		this.tv_Wbs.TabIndex = 0;
		this.tv_Wbs.CellValueChanged += new DevExpress.XtraTreeList.CellValueChangedEventHandler(this.treeList1_CellValueChanged);
		this.tv_Wbs.NodeCellStyle += new DevExpress.XtraTreeList.GetCustomNodeCellStyleEventHandler(this.treeList1_NodeCellStyle);
		this.tv_Wbs.BeforeDragNode += new DevExpress.XtraTreeList.BeforeDragNodeEventHandler(this.tv_Wbs_BeforeDragNode);
		this.tv_Wbs.FocusedNodeChanged += new DevExpress.XtraTreeList.FocusedNodeChangedEventHandler(this.tv_Wbs_FocusedNodeChanged);
		this.tv_Wbs.CustomColumnSort += new DevExpress.XtraTreeList.CustomColumnSortEventHandler(this.tv_Wbs_CustomColumnSort);
		this.tv_Wbs.CustomColumnDisplayText += new DevExpress.XtraTreeList.CustomColumnDisplayTextEventHandler(this.treeList1_CustomColumnDisplayText);
		this.tv_Wbs.ValidatingEditor += new DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventHandler(this.treeList1_ValidatingEditor);
		this.tv_Wbs.InvalidValueException += new DevExpress.XtraEditors.Controls.InvalidValueExceptionEventHandler(this.treeList1_InvalidValueException);
		this.tv_Wbs.BeforeDropNode += new DevExpress.XtraTreeList.BeforeDropNodeEventHandler(this.tv_Wbs_BeforeDropNode);
		this.tv_Wbs.CustomizeNewNodeFromOuterData += new DevExpress.XtraTreeList.CustomizeNewNodeFromOuterDataEventHandler(this.tv_Wbs_CustomizeNewNodeFromOuterData);
		this.tv_Wbs.CalcNodeDragImageIndex += new DevExpress.XtraTreeList.CalcNodeDragImageIndexEventHandler(this.treeList1_CalcNodeDragImageIndex);
		this.tv_Wbs.CustomDrawNodeIndicator += new DevExpress.XtraTreeList.CustomDrawNodeIndicatorEventHandler(this.treeList1_CustomDrawNodeIndicator);
		this.tv_Wbs.CellValueChanging += new DevExpress.XtraTreeList.CellValueChangedEventHandler(this.tv_Wbs_CellValueChanging);
		this.tv_Wbs.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.treeList1_ShowingEditor);
		this.tv_Wbs.EditorKeyDown += new System.Windows.Forms.KeyEventHandler(this.tv_Wbs_EditorKeyDown);
		this.tv_Wbs.DragDrop += new System.Windows.Forms.DragEventHandler(this.treeList1_DragDrop);
		this.tv_Wbs.DragOver += new System.Windows.Forms.DragEventHandler(this.treeList1_DragOver);
		this.tv_Wbs.DragLeave += new System.EventHandler(this.treeList1_DragLeave);
		this.tv_Wbs.GiveFeedback += new System.Windows.Forms.GiveFeedbackEventHandler(this.tv_Wbs_GiveFeedback);
		this.tv_Wbs.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tv_Wbs_KeyDown);
		this.tl_Entry.Caption = "AbsEntry";
		this.tl_Entry.CurrencyDateField = null;
		this.tl_Entry.CurrencyField = null;
		this.tl_Entry.CurrencyFixed = null;
		this.tl_Entry.CurrencyFixedRate = 0.0;
		this.tl_Entry.CurrencyRateField = null;
		this.tl_Entry.FieldName = "AbsEntry";
		this.tl_Entry.Name = "tl_Entry";
		this.tl_Entry.OptionsColumn.AllowEdit = false;
		this.tl_Entry.OptionsColumn.ReadOnly = true;
		this.tl_Wbs.Caption = "WBS";
		this.tl_Wbs.ColumnEdit = this.btn_NewTask;
		this.tl_Wbs.CurrencyDateField = null;
		this.tl_Wbs.CurrencyField = null;
		this.tl_Wbs.CurrencyFixed = null;
		this.tl_Wbs.CurrencyFixedRate = 0.0;
		this.tl_Wbs.CurrencyRateField = null;
		this.tl_Wbs.FieldName = "Wbs";
		this.tl_Wbs.Name = "tl_Wbs";
		this.tl_Wbs.OptionsColumn.AllowSort = false;
		this.tl_Wbs.OptionsColumn.ReadOnly = true;
		this.tl_Wbs.ShowButtonMode = DevExpress.XtraTreeList.ShowButtonModeEnum.ShowAlways;
		this.tl_Wbs.SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
		this.tl_Wbs.Width = 100;
		this.btn_NewTask.AutoHeight = false;
		this.btn_NewTask.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Plus)
		});
		this.btn_NewTask.Name = "btn_NewTask";
		this.btn_NewTask.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btn_NewTask_ButtonClick);
		this.tl_Code.Caption = "Code";
		this.tl_Code.ColumnEdit = this.txt_TaskCode;
		this.tl_Code.CurrencyDateField = null;
		this.tl_Code.CurrencyField = null;
		this.tl_Code.CurrencyFixed = null;
		this.tl_Code.CurrencyFixedRate = 0.0;
		this.tl_Code.CurrencyRateField = null;
		this.tl_Code.FieldName = "TaskCode";
		this.tl_Code.Name = "tl_Code";
		this.tl_Code.ShowButtonMode = DevExpress.XtraTreeList.ShowButtonModeEnum.ShowAlways;
		this.tl_Code.Width = 150;
		this.txt_TaskCode.AutoHeight = false;
		this.txt_TaskCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Plus)
		});
		this.txt_TaskCode.Name = "txt_TaskCode";
		this.txt_TaskCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txt_TaskCode_ButtonClick);
		this.tl_Name.Caption = "Name";
		this.tl_Name.CurrencyDateField = null;
		this.tl_Name.CurrencyField = null;
		this.tl_Name.CurrencyFixed = null;
		this.tl_Name.CurrencyFixedRate = 0.0;
		this.tl_Name.CurrencyRateField = null;
		this.tl_Name.FieldName = "TaskName";
		this.tl_Name.Name = "tl_Name";
		this.tl_Name.Width = 150;
		this.tl_FromDate.Caption = "FromDate";
		this.tl_FromDate.ColumnEdit = this.date_fromdate;
		this.tl_FromDate.CurrencyDateField = null;
		this.tl_FromDate.CurrencyField = null;
		this.tl_FromDate.CurrencyFixed = null;
		this.tl_FromDate.CurrencyFixedRate = 0.0;
		this.tl_FromDate.CurrencyRateField = null;
		this.tl_FromDate.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.tl_FromDate.FieldName = "FromDate";
		this.tl_FromDate.Name = "tl_FromDate";
		this.date_fromdate.AutoHeight = false;
		this.date_fromdate.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_fromdate.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_fromdate.IsSearchControl = false;
		this.date_fromdate.Name = "date_fromdate";
		this.tl_ToDate.Caption = "ToDate";
		this.tl_ToDate.ColumnEdit = this.date_todate;
		this.tl_ToDate.CurrencyDateField = null;
		this.tl_ToDate.CurrencyField = null;
		this.tl_ToDate.CurrencyFixed = null;
		this.tl_ToDate.CurrencyFixedRate = 0.0;
		this.tl_ToDate.CurrencyRateField = null;
		this.tl_ToDate.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.tl_ToDate.FieldName = "ToDate";
		this.tl_ToDate.Name = "tl_ToDate";
		this.date_todate.AutoHeight = false;
		this.date_todate.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_todate.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_todate.IsSearchControl = false;
		this.date_todate.Name = "date_todate";
		this.tl_Milestone.Caption = "Milestone";
		this.tl_Milestone.ColumnEdit = this.luk_Milestone;
		this.tl_Milestone.CurrencyDateField = null;
		this.tl_Milestone.CurrencyField = null;
		this.tl_Milestone.CurrencyFixed = null;
		this.tl_Milestone.CurrencyFixedRate = 0.0;
		this.tl_Milestone.CurrencyRateField = null;
		this.tl_Milestone.FieldName = "Milestone";
		this.tl_Milestone.Name = "tl_Milestone";
		this.luk_Milestone.AutoHeight = false;
		this.luk_Milestone.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_Milestone.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Milestone.IsSearchControl = false;
		this.luk_Milestone.Name = "luk_Milestone";
		this.luk_Milestone.NullText = "";
		this.luk_Milestone.ShowFooter = false;
		this.luk_Milestone.ShowHeader = false;
		this.tl_Notes.Caption = "Notes";
		this.tl_Notes.ColumnEdit = this.repositoryItemMemoExEdit2;
		this.tl_Notes.CurrencyDateField = null;
		this.tl_Notes.CurrencyField = null;
		this.tl_Notes.CurrencyFixed = null;
		this.tl_Notes.CurrencyFixedRate = 0.0;
		this.tl_Notes.CurrencyRateField = null;
		this.tl_Notes.FieldName = "Notes";
		this.tl_Notes.Name = "tl_Notes";
		this.repositoryItemMemoExEdit2.AutoHeight = false;
		this.repositoryItemMemoExEdit2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemMemoExEdit2.Name = "repositoryItemMemoExEdit2";
		this.repositoryItemMemoExEdit2.ShowIcon = false;
		this.tl_Status.Caption = "Status";
		this.tl_Status.ColumnEdit = this.luk_taskStatus;
		this.tl_Status.CurrencyDateField = null;
		this.tl_Status.CurrencyField = null;
		this.tl_Status.CurrencyFixed = null;
		this.tl_Status.CurrencyFixedRate = 0.0;
		this.tl_Status.CurrencyRateField = null;
		this.tl_Status.FieldName = "Status";
		this.tl_Status.Name = "tl_Status";
		this.luk_taskStatus.AutoHeight = false;
		this.luk_taskStatus.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_taskStatus.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_taskStatus.IsSearchControl = false;
		this.luk_taskStatus.Name = "luk_taskStatus";
		this.luk_taskStatus.NullText = "";
		this.luk_taskStatus.ShowFooter = false;
		this.luk_taskStatus.ShowHeader = false;
		this.tl_Dependent.Caption = "Predecessors";
		this.tl_Dependent.CurrencyDateField = null;
		this.tl_Dependent.CurrencyField = null;
		this.tl_Dependent.CurrencyFixed = null;
		this.tl_Dependent.CurrencyFixedRate = 0.0;
		this.tl_Dependent.CurrencyRateField = null;
		this.tl_Dependent.FieldName = "Dependent";
		this.tl_Dependent.Name = "tl_Dependent";
		this.tl_Dependent.Width = 100;
		this.tl_Resource.Caption = "Resources";
		this.tl_Resource.ColumnEdit = this.btn_resource;
		this.tl_Resource.CurrencyDateField = null;
		this.tl_Resource.CurrencyField = null;
		this.tl_Resource.CurrencyFixed = null;
		this.tl_Resource.CurrencyFixedRate = 0.0;
		this.tl_Resource.CurrencyRateField = null;
		this.tl_Resource.FieldName = "Resource";
		this.tl_Resource.Name = "tl_Resource";
		this.btn_resource.AutoHeight = false;
		this.btn_resource.AutoLoadDataSource = true;
		this.btn_resource.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions7, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance14, appearanceHovered14, appearancePressed14, appearanceDisabled14, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, false, true, imageOptions8, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance15, appearanceHovered15, appearancePressed15, appearanceDisabled15, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_resource.ContainerControl = this;
		this.btn_resource.DisplayMember = "";
		this.btn_resource.DisplayMember2 = "";
		this.btn_resource.ListMandatoryField = null;
		this.btn_resource.LoadDataSourceWhenEnter = false;
		this.btn_resource.Name = "btn_resource";
		this.btn_resource.SeparateCharacterForMulti = ';';
		this.btn_resource.SeparateString = ", ";
		this.btn_resource.UsingAutoCompleteBox = false;
		this.btn_resource.UsingDataSource = true;
		this.btn_resource.ValidateWhenNoDataSource = false;
		this.btn_resource.ValueMember = "EmpId";
		this.tl_LineNum.Caption = "LineNum";
		this.tl_LineNum.CurrencyDateField = null;
		this.tl_LineNum.CurrencyField = null;
		this.tl_LineNum.CurrencyFixed = null;
		this.tl_LineNum.CurrencyFixedRate = 0.0;
		this.tl_LineNum.CurrencyRateField = null;
		this.tl_LineNum.FieldName = "LineNum";
		this.tl_LineNum.Name = "tl_LineNum";
		this.tl_LineNum.OptionsColumn.ReadOnly = true;
		this.tl_IsExist.Caption = "IsExist";
		this.tl_IsExist.CurrencyDateField = null;
		this.tl_IsExist.CurrencyField = null;
		this.tl_IsExist.CurrencyFixed = null;
		this.tl_IsExist.CurrencyFixedRate = 0.0;
		this.tl_IsExist.CurrencyRateField = null;
		this.tl_IsExist.FieldName = "IsExist";
		this.tl_IsExist.Name = "tl_IsExist";
		this.tl_IsExist.OptionsColumn.AllowEdit = false;
		this.tl_IsExist.OptionsColumn.ReadOnly = true;
		this.tl_FatherNum.Caption = "FatherNum";
		this.tl_FatherNum.CurrencyDateField = null;
		this.tl_FatherNum.CurrencyField = null;
		this.tl_FatherNum.CurrencyFixed = null;
		this.tl_FatherNum.CurrencyFixedRate = 0.0;
		this.tl_FatherNum.CurrencyRateField = null;
		this.tl_FatherNum.FieldName = "FatherNum";
		this.tl_FatherNum.Name = "tl_FatherNum";
		this.tl_FatherNum.OptionsColumn.AllowEdit = false;
		this.tl_FatherNum.OptionsColumn.ReadOnly = true;
		this.tl_visOrder.Caption = "VisOrder";
		this.tl_visOrder.CurrencyDateField = null;
		this.tl_visOrder.CurrencyField = null;
		this.tl_visOrder.CurrencyFixed = null;
		this.tl_visOrder.CurrencyFixedRate = 0.0;
		this.tl_visOrder.CurrencyRateField = null;
		this.tl_visOrder.FieldName = "VisOrder";
		this.tl_visOrder.Name = "tl_visOrder";
		this.tl_visOrder.OptionsColumn.ReadOnly = true;
		this.tl_BaseLine.Caption = "BaseLine";
		this.tl_BaseLine.CurrencyDateField = null;
		this.tl_BaseLine.CurrencyField = null;
		this.tl_BaseLine.CurrencyFixed = null;
		this.tl_BaseLine.CurrencyFixedRate = 0.0;
		this.tl_BaseLine.CurrencyRateField = null;
		this.tl_BaseLine.FieldName = "BaseLine";
		this.tl_BaseLine.Name = "tl_BaseLine";
		this.tl_BaseLine.OptionsColumn.AllowEdit = false;
		this.tl_BaseLine.OptionsColumn.ReadOnly = true;
		this.tl_WbsTemplate.Caption = "Wbs Template";
		this.tl_WbsTemplate.ColumnEdit = this.btn_WBSTemplate;
		this.tl_WbsTemplate.CurrencyDateField = null;
		this.tl_WbsTemplate.CurrencyField = null;
		this.tl_WbsTemplate.CurrencyFixed = null;
		this.tl_WbsTemplate.CurrencyFixedRate = 0.0;
		this.tl_WbsTemplate.CurrencyRateField = null;
		this.tl_WbsTemplate.FieldName = "WbsTemplate";
		this.tl_WbsTemplate.Name = "tl_WbsTemplate";
		this.tl_WbsTemplate.ShowButtonMode = DevExpress.XtraTreeList.ShowButtonModeEnum.ShowAlways;
		this.btn_WBSTemplate.AutoHeight = false;
		this.btn_WBSTemplate.AutoLoadDataSource = true;
		editorButtonImageOptions8.Image = Apzon.Forms.Properties.Resources.right;
		this.btn_WBSTemplate.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions9, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance16, appearanceHovered16, appearancePressed16, appearanceDisabled16, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions8, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance17, appearanceHovered17, appearancePressed17, appearanceDisabled17, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_WBSTemplate.ContainerControl = this;
		this.btn_WBSTemplate.DisplayMember = "";
		this.btn_WBSTemplate.DisplayMember2 = "";
		this.btn_WBSTemplate.ListMandatoryField = null;
		this.btn_WBSTemplate.LoadDataSourceWhenEnter = false;
		this.btn_WBSTemplate.Name = "btn_WBSTemplate";
		this.btn_WBSTemplate.SeparateCharacterForMulti = ';';
		this.btn_WBSTemplate.SeparateString = ", ";
		this.btn_WBSTemplate.UsingAutoCompleteBox = false;
		this.btn_WBSTemplate.UsingDataSource = true;
		this.btn_WBSTemplate.ValidateWhenNoDataSource = false;
		this.btn_WBSTemplate.ValueMember = "";
		this.tl_WBSMode.Caption = "WBSMode";
		this.tl_WBSMode.ColumnEdit = this.luk_WBSMode;
		this.tl_WBSMode.CurrencyDateField = null;
		this.tl_WBSMode.CurrencyField = null;
		this.tl_WBSMode.CurrencyFixed = null;
		this.tl_WBSMode.CurrencyFixedRate = 0.0;
		this.tl_WBSMode.CurrencyRateField = null;
		this.tl_WBSMode.FieldName = "WBSMode";
		this.tl_WBSMode.Name = "tl_WBSMode";
		this.luk_WBSMode.AutoHeight = false;
		this.luk_WBSMode.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_WBSMode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_WBSMode.IsSearchControl = false;
		this.luk_WBSMode.Name = "luk_WBSMode";
		this.luk_WBSMode.NullText = "";
		this.luk_WBSMode.ShowFooter = false;
		this.luk_WBSMode.ShowHeader = false;
		this.tl_Duration.Caption = "Duration";
		this.tl_Duration.CurrencyDateField = null;
		this.tl_Duration.CurrencyField = null;
		this.tl_Duration.CurrencyFixed = null;
		this.tl_Duration.CurrencyFixedRate = 0.0;
		this.tl_Duration.CurrencyRateField = null;
		this.tl_Duration.FieldName = "Duration";
		this.tl_Duration.Name = "tl_Duration";
		this.treeListColumnEx2.Caption = "Comments";
		this.treeListColumnEx2.ColumnEdit = this.rep_WbsComments;
		this.treeListColumnEx2.CurrencyDateField = null;
		this.treeListColumnEx2.CurrencyField = null;
		this.treeListColumnEx2.CurrencyFixed = null;
		this.treeListColumnEx2.CurrencyFixedRate = 0.0;
		this.treeListColumnEx2.CurrencyRateField = null;
		this.treeListColumnEx2.FieldName = "CommentsCount";
		this.treeListColumnEx2.Name = "treeListColumnEx2";
		this.treeListColumnEx2.ShowButtonMode = DevExpress.XtraTreeList.ShowButtonModeEnum.ShowAlways;
		this.rep_WbsComments.AutoHeight = false;
		editorButtonImageOptions9.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("editorButtonImageOptions22.SvgImage");
		editorButtonImageOptions9.SvgImageSize = new System.Drawing.Size(16, 16);
		this.rep_WbsComments.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, editorButtonImageOptions9, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance18, appearanceHovered18, appearancePressed18, appearanceDisabled18, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_WbsComments.Name = "rep_WbsComments";
		this.rep_WbsComments.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_WbsComments_ButtonClick);
		this.ctm_Rows.ImageScalingSize = new System.Drawing.Size(18, 18);
		this.ctm_Rows.Items.AddRange(new System.Windows.Forms.ToolStripItem[17]
		{
			this.toolStripMenuItemIndent, this.toolStripMenuItemOutdent, this.toolStripSeparator7, this.addRowToolStripMenuItem, this.insertRowToolStripMenuItem, this.duplicateRowToolStripMenuItem, this.toolStripSeparator2, this.deleteRowToolStripMenuItem, this.toolStripSeparator1, this.addSubProjectToolStripMenuItem1,
			this.toolStripSeparator3, this.newTaskToolStripMenuItem1, this.newTrackingIssueToolStripMenuItem1, this.newEventToolStripMenuItem, this.genTasksToolStripMenuItem, this.toolStripSeparator8, this.viewOrgChartToolStripMenuItem
		});
		this.ctm_Rows.Name = "contextMenuStrip2";
		this.ctm_Rows.Size = new System.Drawing.Size(209, 322);
		this.ctm_Rows.Opening += new System.ComponentModel.CancelEventHandler(this.ctm_Rows_Opening);
		this.toolStripMenuItemIndent.Image = (System.Drawing.Image)resources.GetObject("toolStripMenuItemIndent.Image");
		this.toolStripMenuItemIndent.Name = "toolStripMenuItemIndent";
		this.toolStripMenuItemIndent.Size = new System.Drawing.Size(208, 24);
		this.toolStripMenuItemIndent.Text = "Indent(Alt+Shift+Right)";
		this.toolStripMenuItemIndent.Click += new System.EventHandler(this.toolStripMenuItemIndent_Click);
		this.toolStripMenuItemOutdent.Image = (System.Drawing.Image)resources.GetObject("toolStripMenuItemOutdent.Image");
		this.toolStripMenuItemOutdent.Name = "toolStripMenuItemOutdent";
		this.toolStripMenuItemOutdent.Size = new System.Drawing.Size(208, 24);
		this.toolStripMenuItemOutdent.Text = "Outdent(Alt+Shift+Left)";
		this.toolStripMenuItemOutdent.Click += new System.EventHandler(this.toolStripMenuItemOutdent_Click);
		this.toolStripSeparator7.Name = "toolStripSeparator7";
		this.toolStripSeparator7.Size = new System.Drawing.Size(205, 6);
		this.addRowToolStripMenuItem.Name = "addRowToolStripMenuItem";
		this.addRowToolStripMenuItem.Size = new System.Drawing.Size(208, 24);
		this.addRowToolStripMenuItem.Text = "Add Row";
		this.addRowToolStripMenuItem.Click += new System.EventHandler(this.addRowToolStripMenuItem_Click);
		this.insertRowToolStripMenuItem.Name = "insertRowToolStripMenuItem";
		this.insertRowToolStripMenuItem.Size = new System.Drawing.Size(208, 24);
		this.insertRowToolStripMenuItem.Text = "Insert Row";
		this.insertRowToolStripMenuItem.Click += new System.EventHandler(this.insertRowToolStripMenuItem_Click);
		this.duplicateRowToolStripMenuItem.Name = "duplicateRowToolStripMenuItem";
		this.duplicateRowToolStripMenuItem.Size = new System.Drawing.Size(208, 24);
		this.duplicateRowToolStripMenuItem.Text = "Duplicate Row";
		this.duplicateRowToolStripMenuItem.Click += new System.EventHandler(this.duplicateRowToolStripMenuItem_Click);
		this.toolStripSeparator2.Name = "toolStripSeparator2";
		this.toolStripSeparator2.Size = new System.Drawing.Size(205, 6);
		this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
		this.deleteRowToolStripMenuItem.Size = new System.Drawing.Size(208, 24);
		this.deleteRowToolStripMenuItem.Text = "Delete Row(Shift+Delete)";
		this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.deleteRowToolStripMenuItem_Click);
		this.toolStripSeparator1.Name = "toolStripSeparator1";
		this.toolStripSeparator1.Size = new System.Drawing.Size(205, 6);
		this.addSubProjectToolStripMenuItem1.Name = "addSubProjectToolStripMenuItem1";
		this.addSubProjectToolStripMenuItem1.Size = new System.Drawing.Size(208, 24);
		this.addSubProjectToolStripMenuItem1.Text = "Define New Sub. Project";
		this.addSubProjectToolStripMenuItem1.Click += new System.EventHandler(this.addSubProjectToolStripMenuItem1_Click);
		this.toolStripSeparator3.Name = "toolStripSeparator3";
		this.toolStripSeparator3.Size = new System.Drawing.Size(205, 6);
		this.newTaskToolStripMenuItem1.Name = "newTaskToolStripMenuItem1";
		this.newTaskToolStripMenuItem1.Size = new System.Drawing.Size(208, 24);
		this.newTaskToolStripMenuItem1.Text = "New Task";
		this.newTaskToolStripMenuItem1.Click += new System.EventHandler(this.newTaskToolStripMenuItem1_Click);
		this.newTrackingIssueToolStripMenuItem1.Name = "newTrackingIssueToolStripMenuItem1";
		this.newTrackingIssueToolStripMenuItem1.Size = new System.Drawing.Size(208, 24);
		this.newTrackingIssueToolStripMenuItem1.Text = "New Issue";
		this.newTrackingIssueToolStripMenuItem1.Click += new System.EventHandler(this.newTrackingIssueToolStripMenuItem1_Click);
		this.newEventToolStripMenuItem.Name = "newEventToolStripMenuItem";
		this.newEventToolStripMenuItem.Size = new System.Drawing.Size(208, 24);
		this.newEventToolStripMenuItem.Text = "New Event";
		this.newEventToolStripMenuItem.Click += new System.EventHandler(this.newEventToolStripMenuItem_Click);
		this.genTasksToolStripMenuItem.Name = "genTasksToolStripMenuItem";
		this.genTasksToolStripMenuItem.Size = new System.Drawing.Size(208, 24);
		this.genTasksToolStripMenuItem.Text = "Generate Task";
		this.genTasksToolStripMenuItem.Click += new System.EventHandler(this.genTasksToolStripMenuItem_Click);
		this.toolStripSeparator8.Name = "toolStripSeparator8";
		this.toolStripSeparator8.Size = new System.Drawing.Size(205, 6);
		this.viewOrgChartToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("viewOrgChartToolStripMenuItem.Image");
		this.viewOrgChartToolStripMenuItem.Name = "viewOrgChartToolStripMenuItem";
		this.viewOrgChartToolStripMenuItem.Size = new System.Drawing.Size(208, 24);
		this.viewOrgChartToolStripMenuItem.Text = "View Org Chart";
		this.viewOrgChartToolStripMenuItem.Click += new System.EventHandler(this.viewOrgChartToolStripMenuItem_Click);
		this.splitContainerControl5.Collapsed = true;
		this.splitContainerControl5.CollapsePanel = DevExpress.XtraEditors.SplitCollapsePanel.Panel2;
		this.splitContainerControl5.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainerControl5.Location = new System.Drawing.Point(0, 0);
		this.splitContainerControl5.Margin = new System.Windows.Forms.Padding(2);
		this.splitContainerControl5.Name = "splitContainerControl5";
		this.splitContainerControl5.Panel1.Controls.Add(this.schedulerControl1);
		this.splitContainerControl5.Panel1.Text = "Panel1";
		this.splitContainerControl5.Panel2.Controls.Add(this.chk_Action);
		this.splitContainerControl5.Panel2.Controls.Add(this.chk_Users);
		this.splitContainerControl5.Panel2.Controls.Add(this.dateNavigator1);
		this.splitContainerControl5.Panel2.Controls.Add(this.labelControl1);
		this.splitContainerControl5.Panel2.Text = "Panel2";
		this.splitContainerControl5.Size = new System.Drawing.Size(108, 208);
		this.splitContainerControl5.SplitterPosition = 236;
		this.splitContainerControl5.TabIndex = 0;
		this.splitContainerControl5.Text = "splitContainerControl5";
		this.schedulerControl1.Appearance.HeaderCaption.BackColor = System.Drawing.Color.Aqua;
		this.schedulerControl1.Appearance.HeaderCaption.Options.UseBackColor = true;
		this.schedulerControl1.DataStorage = this.schedulerStorage1;
		this.schedulerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.schedulerControl1.Location = new System.Drawing.Point(0, 0);
		this.schedulerControl1.Margin = new System.Windows.Forms.Padding(2);
		this.schedulerControl1.Name = "schedulerControl1";
		this.schedulerControl1.OptionsBehavior.SelectOnRightClick = true;
		this.schedulerControl1.OptionsCustomization.AllowAppointmentCopy = DevExpress.XtraScheduler.UsedAppointmentType.None;
		this.schedulerControl1.OptionsCustomization.AllowAppointmentCreate = DevExpress.XtraScheduler.UsedAppointmentType.None;
		this.schedulerControl1.OptionsCustomization.AllowAppointmentDelete = DevExpress.XtraScheduler.UsedAppointmentType.None;
		this.schedulerControl1.OptionsCustomization.AllowAppointmentDrag = DevExpress.XtraScheduler.UsedAppointmentType.None;
		this.schedulerControl1.OptionsCustomization.AllowAppointmentDragBetweenResources = DevExpress.XtraScheduler.UsedAppointmentType.None;
		this.schedulerControl1.OptionsCustomization.AllowAppointmentEdit = DevExpress.XtraScheduler.UsedAppointmentType.None;
		this.schedulerControl1.OptionsCustomization.AllowAppointmentMultiSelect = false;
		this.schedulerControl1.OptionsCustomization.AllowAppointmentResize = DevExpress.XtraScheduler.UsedAppointmentType.None;
		this.schedulerControl1.OptionsCustomization.AllowDisplayAppointmentDependencyForm = DevExpress.XtraScheduler.AllowDisplayAppointmentDependencyForm.Never;
		this.schedulerControl1.OptionsCustomization.AllowDisplayAppointmentFlyout = false;
		this.schedulerControl1.OptionsCustomization.AllowDisplayAppointmentForm = DevExpress.XtraScheduler.AllowDisplayAppointmentForm.Never;
		this.schedulerControl1.OptionsCustomization.AllowInplaceEditor = DevExpress.XtraScheduler.UsedAppointmentType.None;
		this.schedulerControl1.OptionsView.NavigationButtons.Visibility = DevExpress.XtraScheduler.NavigationButtonVisibility.Never;
		this.schedulerControl1.OptionsView.ShowOnlyResourceAppointments = true;
		this.schedulerControl1.Size = new System.Drawing.Size(98, 208);
		this.schedulerControl1.Start = new System.DateTime(2016, 10, 31, 0, 0, 0, 0);
		this.schedulerControl1.TabIndex = 1;
		this.schedulerControl1.Text = "schedulerControl1";
		this.schedulerControl1.Views.AgendaView.AppointmentDisplayOptions.ShowResource = true;
		this.schedulerControl1.Views.DayView.AllDayAreaScrollBarVisible = true;
		this.schedulerControl1.Views.DayView.Appearance.AdditionalHeaderCaption.Options.UseBackColor = true;
		this.schedulerControl1.Views.DayView.AppointmentDisplayOptions.StatusDisplayType = DevExpress.XtraScheduler.AppointmentStatusDisplayType.Bounds;
		this.schedulerControl1.Views.DayView.ResourcesPerPage = 3;
		this.schedulerControl1.Views.DayView.TimeRulers.Add(value);
		this.schedulerControl1.Views.FullWeekView.Enabled = true;
		this.schedulerControl1.Views.FullWeekView.TimeRulers.Add(value2);
		this.schedulerControl1.Views.GanttView.AppointmentDisplayOptions.PercentCompleteDisplayType = DevExpress.XtraScheduler.PercentCompleteDisplayType.None;
		this.schedulerControl1.Views.WorkWeekView.TimeRulers.Add(value3);
		this.schedulerControl1.PopupMenuShowing += new DevExpress.XtraScheduler.PopupMenuShowingEventHandler(this.schedulerControl1_PopupMenuShowing);
		this.schedulerControl1.CustomDrawDependency += new DevExpress.XtraScheduler.CustomDrawObjectEventHandler(this.schedulerControl1_CustomDrawDependency);
		this.schedulerStorage1.Appointments.CustomFieldMappings.Add(new DevExpress.XtraScheduler.AppointmentCustomFieldMapping("AddrName", "AddrName"));
		this.schedulerStorage1.Appointments.CustomFieldMappings.Add(new DevExpress.XtraScheduler.AppointmentCustomFieldMapping("AddrType", "AddrType"));
		this.schedulerStorage1.Appointments.ResourceSharing = true;
		this.schedulerStorage1.Resources.ColorSaving = DevExpress.XtraScheduler.ColorSavingType.ArgbColor;
		this.chk_Action.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.chk_Action.CheckOnClick = true;
		this.chk_Action.Cursor = System.Windows.Forms.Cursors.Default;
		this.chk_Action.HotTrackSelectMode = DevExpress.XtraEditors.HotTrackSelectMode.SelectItemOnClick;
		this.chk_Action.Location = new System.Drawing.Point(1, 3);
		this.chk_Action.Margin = new System.Windows.Forms.Padding(2);
		this.chk_Action.Name = "chk_Action";
		this.chk_Action.Size = new System.Drawing.Size(211, 54);
		this.chk_Action.TabIndex = 22;
		this.chk_Users.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.chk_Users.CheckOnClick = true;
		this.chk_Users.Cursor = System.Windows.Forms.Cursors.Default;
		this.chk_Users.IncrementalSearch = true;
		this.chk_Users.Location = new System.Drawing.Point(1, 302);
		this.chk_Users.Margin = new System.Windows.Forms.Padding(2);
		this.chk_Users.Name = "chk_Users";
		this.chk_Users.SchedulerControl = this.schedulerControl1;
		this.chk_Users.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
		this.chk_Users.Size = new System.Drawing.Size(211, 55);
		this.chk_Users.TabIndex = 15;
		this.dateNavigator1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.dateNavigator1.BoldAppointmentDates = false;
		this.dateNavigator1.CalendarAppearance.DayCellSpecial.FontStyleDelta = System.Drawing.FontStyle.Bold;
		this.dateNavigator1.CalendarAppearance.DayCellSpecial.Options.UseFont = true;
		this.dateNavigator1.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dateNavigator1.CellSize = new System.Drawing.Size(18, 19);
		this.dateNavigator1.Cursor = System.Windows.Forms.Cursors.Default;
		this.dateNavigator1.DateTime = new System.DateTime(2016, 10, 31, 0, 0, 0, 0);
		this.dateNavigator1.EditValue = new System.DateTime(2016, 10, 31, 0, 0, 0, 0);
		this.dateNavigator1.FirstDayOfWeek = System.DayOfWeek.Sunday;
		this.dateNavigator1.Location = new System.Drawing.Point(1, 63);
		this.dateNavigator1.Margin = new System.Windows.Forms.Padding(2);
		this.dateNavigator1.Name = "dateNavigator1";
		this.dateNavigator1.NavigationMode = DevExpress.XtraScheduler.DateNavigationMode.ScrollCalendar;
		this.dateNavigator1.Padding = new System.Windows.Forms.Padding(2);
		this.dateNavigator1.SchedulerControl = this.schedulerControl1;
		this.dateNavigator1.Size = new System.Drawing.Size(211, 212);
		this.dateNavigator1.TabIndex = 10;
		this.labelControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.labelControl1.Location = new System.Drawing.Point(3, 282);
		this.labelControl1.Margin = new System.Windows.Forms.Padding(2);
		this.labelControl1.Name = "labelControl1";
		this.labelControl1.Size = new System.Drawing.Size(50, 13);
		this.labelControl1.TabIndex = 21;
		this.labelControl1.Text = "Resources";
		this.xtra_Team.Controls.Add(this.grd_LineTeam);
		this.xtra_Team.EditControl = null;
		this.xtra_Team.EditValue = "Project Team";
		this.xtra_Team.FieldName = "xtra_Team";
		this.xtra_Team.IsSearchControl = false;
		this.xtra_Team.Margin = new System.Windows.Forms.Padding(2);
		this.xtra_Team.Name = "xtra_Team";
		this.xtra_Team.ReadOnly = false;
		this.xtra_Team.SettingInVisible = false;
		this.xtra_Team.SettingReadonly = false;
		this.xtra_Team.Size = new System.Drawing.Size(880, 208);
		this.xtra_Team.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtra_Team.Text = "Project Team";
		this.grd_LineTeam.AllowPasteExcel = false;
		this.grd_LineTeam.ClipboardData = null;
		this.grd_LineTeam.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_LineTeam.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(2);
		this.grd_LineTeam.Location = new System.Drawing.Point(0, 0);
		this.grd_LineTeam.MainView = this.gv_LineTeam;
		this.grd_LineTeam.Margin = new System.Windows.Forms.Padding(2);
		this.grd_LineTeam.Name = "grd_LineTeam";
		this.grd_LineTeam.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[4] { this.luk_branch, this.luk_dept, this.luk_role, this.btn_ResourceEmployee });
		this.grd_LineTeam.Size = new System.Drawing.Size(880, 208);
		this.grd_LineTeam.TabIndex = 0;
		this.grd_LineTeam.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_LineTeam });
		this.gv_LineTeam.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_LineTeam.ColumnKey = null;
		this.gv_LineTeam.ColumnPanelRowHeight = 0;
		this.gv_LineTeam.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[7] { this.gridColumnEx50, this.gridColumnEx51, this.gridColumnEx52, this.gridColumnEx53, this.gridColumnEx54, this.gridColumnEx55, this.gridColumnEx56 });
		this.gv_LineTeam.DetailHeight = 364;
		this.gv_LineTeam.DisableFormSetting = false;
		this.gv_LineTeam.DisplayException = false;
		this.gv_LineTeam.EditColumn = null;
		this.gv_LineTeam.FieldCurrency = null;
		this.gv_LineTeam.FieldRate = null;
		this.gv_LineTeam.FooterPanelHeight = 0;
		this.gv_LineTeam.FormId = "oProjectManagement";
		this.gv_LineTeam.GridControl = this.grd_LineTeam;
		this.gv_LineTeam.GroupRowHeight = 0;
		this.gv_LineTeam.HasFormSetting = true;
		this.gv_LineTeam.IgnoreFilter = false;
		this.gv_LineTeam.IndexEditRow = -1;
		this.gv_LineTeam.IndicatorWidth = 35;
		this.gv_LineTeam.IsApzTable = false;
		this.gv_LineTeam.IsInsertNewRow = false;
		this.gv_LineTeam.IsLoadingData = false;
		this.gv_LineTeam.IsMappingContextAction = false;
		this.gv_LineTeam.ItemId = "APZ_PRG3";
		this.gv_LineTeam.LevelIndent = 0;
		this.gv_LineTeam.LstColumnAllowBatch = null;
		this.gv_LineTeam.LstColumnNotAllowBatch = null;
		this.gv_LineTeam.LstColumnNotEditable = null;
		this.gv_LineTeam.LstColumNotSetting = null;
		this.gv_LineTeam.LstOldUDF = null;
		this.gv_LineTeam.Name = "gv_LineTeam";
		this.gv_LineTeam.OptionsView.BestFitMaxRowCount = 100;
		this.gv_LineTeam.OptionsView.ColumnAutoWidth = false;
		this.gv_LineTeam.OptionsView.ShowGroupPanel = false;
		this.gv_LineTeam.PreviewIndent = 0;
		this.gv_LineTeam.ReadOnly = false;
		this.gv_LineTeam.RowHeight = 0;
		this.gv_LineTeam.SettingReadonly = false;
		this.gv_LineTeam.UseBatchMod = false;
		this.gv_LineTeam.ViewCaptionHeight = 0;
		this.gv_LineTeam.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_LineTeam_RowCellStyle);
		this.gv_LineTeam.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_LineTeam_ShowingEditor);
		this.gv_LineTeam.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_LineTeam_CellValueChanging);
		this.gridColumnEx50.Caption = "AbsEntry";
		this.gridColumnEx50.CurrencyDateField = null;
		this.gridColumnEx50.CurrencyField = null;
		this.gridColumnEx50.CurrencyFixed = null;
		this.gridColumnEx50.CurrencyFixedRate = 0.0;
		this.gridColumnEx50.CurrencyRateField = null;
		this.gridColumnEx50.FieldName = "AbsEntry";
		this.gridColumnEx50.Name = "gridColumnEx50";
		this.gridColumnEx50.OptionsColumn.AllowEdit = false;
		this.gridColumnEx50.OptionsColumn.ReadOnly = true;
		this.gridColumnEx50.PrimaryKey = false;
		this.gridColumnEx50.UnitField = null;
		this.gridColumnEx51.Caption = "LineNum";
		this.gridColumnEx51.CurrencyDateField = null;
		this.gridColumnEx51.CurrencyField = null;
		this.gridColumnEx51.CurrencyFixed = null;
		this.gridColumnEx51.CurrencyFixedRate = 0.0;
		this.gridColumnEx51.CurrencyRateField = null;
		this.gridColumnEx51.FieldName = "LineNum";
		this.gridColumnEx51.Name = "gridColumnEx51";
		this.gridColumnEx51.OptionsColumn.ReadOnly = true;
		this.gridColumnEx51.PrimaryKey = false;
		this.gridColumnEx51.UnitField = null;
		this.gridColumnEx52.Caption = "Employee Code";
		this.gridColumnEx52.ColumnEdit = this.btn_ResourceEmployee;
		this.gridColumnEx52.CurrencyDateField = null;
		this.gridColumnEx52.CurrencyField = null;
		this.gridColumnEx52.CurrencyFixed = null;
		this.gridColumnEx52.CurrencyFixedRate = 0.0;
		this.gridColumnEx52.CurrencyRateField = null;
		this.gridColumnEx52.FieldName = "Resource";
		this.gridColumnEx52.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx52.Name = "gridColumnEx52";
		this.gridColumnEx52.OptionsColumn.ReadOnly = true;
		this.gridColumnEx52.PrimaryKey = false;
		this.gridColumnEx52.UnitField = null;
		this.gridColumnEx52.Width = 150;
		this.btn_ResourceEmployee.AutoHeight = false;
		this.btn_ResourceEmployee.AutoLoadDataSource = true;
		editorButtonImageOptions10.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions23.Image");
		this.btn_ResourceEmployee.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions10, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance19, appearanceHovered19, appearancePressed19, appearanceDisabled19, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_ResourceEmployee.ContainerControl = this;
		this.btn_ResourceEmployee.DisplayMember = "EmpId";
		this.btn_ResourceEmployee.DisplayMember2 = "";
		this.btn_ResourceEmployee.ListMandatoryField = null;
		this.btn_ResourceEmployee.LoadDataSourceWhenEnter = false;
		this.btn_ResourceEmployee.Name = "btn_ResourceEmployee";
		this.btn_ResourceEmployee.ReadOnly = true;
		this.btn_ResourceEmployee.SeparateCharacterForMulti = ';';
		this.btn_ResourceEmployee.SeparateString = ", ";
		this.btn_ResourceEmployee.UsingAutoCompleteBox = false;
		this.btn_ResourceEmployee.UsingDataSource = true;
		this.btn_ResourceEmployee.ValidateWhenNoDataSource = false;
		this.btn_ResourceEmployee.ValueMember = "EmpId";
		this.gridColumnEx53.Caption = "Employee Name";
		this.gridColumnEx53.CurrencyDateField = null;
		this.gridColumnEx53.CurrencyField = null;
		this.gridColumnEx53.CurrencyFixed = null;
		this.gridColumnEx53.CurrencyFixedRate = 0.0;
		this.gridColumnEx53.CurrencyRateField = null;
		this.gridColumnEx53.FieldName = "EmployeeName";
		this.gridColumnEx53.Name = "gridColumnEx53";
		this.gridColumnEx53.OptionsColumn.ReadOnly = true;
		this.gridColumnEx53.PrimaryKey = false;
		this.gridColumnEx53.UnitField = null;
		this.gridColumnEx53.Width = 150;
		this.gridColumnEx54.Caption = "Branch";
		this.gridColumnEx54.ColumnEdit = this.luk_branch;
		this.gridColumnEx54.CurrencyDateField = null;
		this.gridColumnEx54.CurrencyField = null;
		this.gridColumnEx54.CurrencyFixed = null;
		this.gridColumnEx54.CurrencyFixedRate = 0.0;
		this.gridColumnEx54.CurrencyRateField = null;
		this.gridColumnEx54.FieldName = "Branch";
		this.gridColumnEx54.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx54.Name = "gridColumnEx54";
		this.gridColumnEx54.OptionsColumn.ReadOnly = true;
		this.gridColumnEx54.PrimaryKey = false;
		this.gridColumnEx54.UnitField = null;
		this.gridColumnEx54.Width = 150;
		this.luk_branch.AutoHeight = false;
		this.luk_branch.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
		this.luk_branch.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_branch.CheckFields = null;
		this.luk_branch.CheckTables = null;
		this.luk_branch.Name = "luk_branch";
		this.luk_branch.NullText = "";
		this.luk_branch.ShowFooter = false;
		this.luk_branch.ShowHeader = false;
		this.gridColumnEx55.Caption = "Department";
		this.gridColumnEx55.ColumnEdit = this.luk_dept;
		this.gridColumnEx55.CurrencyDateField = null;
		this.gridColumnEx55.CurrencyField = null;
		this.gridColumnEx55.CurrencyFixed = null;
		this.gridColumnEx55.CurrencyFixedRate = 0.0;
		this.gridColumnEx55.CurrencyRateField = null;
		this.gridColumnEx55.FieldName = "Department";
		this.gridColumnEx55.Name = "gridColumnEx55";
		this.gridColumnEx55.OptionsColumn.ReadOnly = true;
		this.gridColumnEx55.PrimaryKey = false;
		this.gridColumnEx55.UnitField = null;
		this.gridColumnEx55.Width = 150;
		this.luk_dept.AutoHeight = false;
		this.luk_dept.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
		this.luk_dept.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_dept.CheckFields = null;
		this.luk_dept.CheckTables = null;
		this.luk_dept.Name = "luk_dept";
		this.luk_dept.NullText = "";
		this.luk_dept.ShowFooter = false;
		this.luk_dept.ShowHeader = false;
		this.gridColumnEx56.Caption = "Project Roles";
		this.gridColumnEx56.ColumnEdit = this.luk_role;
		this.gridColumnEx56.CurrencyDateField = null;
		this.gridColumnEx56.CurrencyField = null;
		this.gridColumnEx56.CurrencyFixed = null;
		this.gridColumnEx56.CurrencyFixedRate = 0.0;
		this.gridColumnEx56.CurrencyRateField = null;
		this.gridColumnEx56.FieldName = "Role";
		this.gridColumnEx56.Name = "gridColumnEx56";
		this.gridColumnEx56.PrimaryKey = false;
		this.gridColumnEx56.UnitField = null;
		this.gridColumnEx56.Width = 150;
		this.luk_role.AutoHeight = false;
		this.luk_role.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
		this.luk_role.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_role.CheckFields = null;
		this.luk_role.CheckTables = null;
		this.luk_role.Name = "luk_role";
		this.luk_role.NullText = "";
		this.luk_role.ShowFooter = false;
		this.luk_role.ShowHeader = false;
		this.xtra_Budget.Controls.Add(this.xtra_LineBudget);
		this.xtra_Budget.EditControl = null;
		this.xtra_Budget.EditValue = "Project Budget";
		this.xtra_Budget.FieldName = "xtra_Budget";
		this.xtra_Budget.IsSearchControl = false;
		this.xtra_Budget.Margin = new System.Windows.Forms.Padding(2);
		this.xtra_Budget.Name = "xtra_Budget";
		this.xtra_Budget.ReadOnly = false;
		this.xtra_Budget.SettingInVisible = false;
		this.xtra_Budget.SettingReadonly = false;
		this.xtra_Budget.Size = new System.Drawing.Size(880, 208);
		this.xtra_Budget.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtra_Budget.Text = "Project Budget";
		this.xtra_LineBudget.Dock = System.Windows.Forms.DockStyle.Fill;
		this.xtra_LineBudget.IsSearchControl = false;
		this.xtra_LineBudget.Location = new System.Drawing.Point(0, 0);
		this.xtra_LineBudget.Margin = new System.Windows.Forms.Padding(2);
		this.xtra_LineBudget.Name = "xtra_LineBudget";
		this.xtra_LineBudget.SelectedTabPage = this.xtra_LineItem;
		this.xtra_LineBudget.Size = new System.Drawing.Size(880, 208);
		this.xtra_LineBudget.TabIndex = 0;
		this.xtra_LineBudget.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[2] { this.xtra_LineItem, this.xtra_Payment });
		this.xtra_LineBudget.SelectedPageChanged += new DevExpress.XtraTab.TabPageChangedEventHandler(this.xtra_LineBudget_SelectedPageChanged);
		this.xtra_LineItem.Controls.Add(this.tv_budget);
		this.xtra_LineItem.EditControl = null;
		this.xtra_LineItem.EditValue = "Item Budget";
		this.xtra_LineItem.FieldName = "xtra_LineItem";
		this.xtra_LineItem.IsSearchControl = false;
		this.xtra_LineItem.Margin = new System.Windows.Forms.Padding(2);
		this.xtra_LineItem.Name = "xtra_LineItem";
		this.xtra_LineItem.ReadOnly = false;
		this.xtra_LineItem.SettingInVisible = false;
		this.xtra_LineItem.SettingReadonly = false;
		this.xtra_LineItem.Size = new System.Drawing.Size(878, 183);
		this.xtra_LineItem.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtra_LineItem.Text = "Item Budget";
		this.tv_budget.AllowPasteExcel = true;
		this.tv_budget.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.tv_budget.ColumnKey = null;
		this.tv_budget.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[39]
		{
			this.tb_AbsEntry, this.tb_LineNum, this.tb_ItemCode, this.tb_ItemName, this.tb_TaskCode, this.tb_TaskName, this.tb_plQty, this.tb_RefPrice, this.tb_Amount, this.tb_Branch,
			this.tb_ActualQty, this.tb_QtyDiff, this.tb_Notes, this.tb_FatherNum, this.tb_IsNew, this.tb_ActualAmount, this.tb_FromDate, this.tb_ToDate, this.tb_BomItem, this.tb_OrderItem,
			this.tb_PR, this.tb_PO, this.tb_GRPO, this.tb_Delivery, this.tb_Invoice, this.tb_UoM, this.tb_BaseLine, this.tb_AR, this.tb_BaseQty, this.tb_ItmsGrpCod,
			this.tb_DefaultWhs, this.tb_VisOrder, this.tb_Currency, this.tb_PQ, this.tb_ARDR, this.tb_APDR, this.tb_IGE, this.treeListColumnEx1, this.treeListColumnEx3
		});
		this.tv_budget.ContextMenuStrip = this.ctm_BudgetItem;
		this.tv_budget.Cursor = System.Windows.Forms.Cursors.Default;
		this.tv_budget.DisableFormSetting = false;
		this.tv_budget.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tv_budget.EditColumn = null;
		this.tv_budget.FieldCurrency = null;
		this.tv_budget.FieldRate = null;
		this.tv_budget.FormId = "oProjectManagement";
		this.tv_budget.HasFormSetting = true;
		this.tv_budget.IndexEditRow = 0;
		this.tv_budget.IndicatorWidth = 40;
		this.tv_budget.IsApzTable = false;
		this.tv_budget.IsLoadingData = false;
		this.tv_budget.ItemId = "APZ_PRG4";
		this.tv_budget.KeyFieldName = "LineNum";
		this.tv_budget.Location = new System.Drawing.Point(0, 0);
		this.tv_budget.LstColumnNotEditable = null;
		this.tv_budget.LstColumNotSetting = null;
		this.tv_budget.LstOldUDF = null;
		this.tv_budget.Margin = new System.Windows.Forms.Padding(2);
		this.tv_budget.Name = "tv_budget";
		this.tv_budget.OptionsBehavior.PopulateServiceColumns = true;
		this.tv_budget.OptionsMenu.ShowExpandCollapseItems = false;
		this.tv_budget.OptionsSelection.MultiSelect = true;
		this.tv_budget.OptionsView.AutoWidth = false;
		this.tv_budget.OptionsView.CheckBoxStyle = DevExpress.XtraTreeList.DefaultNodeCheckBoxStyle.Check;
		this.tv_budget.ParentFieldName = "FatherNum";
		this.tv_budget.ReadOnly = false;
		this.tv_budget.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[26]
		{
			this.luk_ItemBranch, this.btn_ItemCode, this.btn_TaskCode, this.btn_AbsEntry, this.txt_RefPrice, this.dat_IFromDate, this.dat_IToDate, this.btn_OrderItem, this.btn_BOM, this.luk_UoM,
			this.btn_PR, this.btn_PO, this.btn_GRPO, this.btn_Delivery, this.btn_APInvoice, this.btn_ARInvoice, this.luk_ItemGroup, this.luk_ItmsGrpCod, this.repositoryItemMemoExEdit3, this.cal_PlanQty,
			this.btn_WarehouseItem, this.luk_Currency, this.btn_PQ, this.btn_IGE, this.btn_WOR, this.rep_BudgetComments
		});
		this.tv_budget.SettingReadonly = false;
		this.tv_budget.Size = new System.Drawing.Size(878, 183);
		this.tv_budget.TabIndex = 1;
		this.tv_budget.CellValueChanged += new DevExpress.XtraTreeList.CellValueChangedEventHandler(this.tv_budget_CellValueChanged);
		this.tv_budget.AfterCheckNode += new DevExpress.XtraTreeList.NodeEventHandler(this.tv_budget_AfterCheckNode);
		this.tv_budget.FocusedNodeChanged += new DevExpress.XtraTreeList.FocusedNodeChangedEventHandler(this.tv_budget_FocusedNodeChanged);
		this.tv_budget.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.tv_budget_ShowingEditor);
		this.tv_budget.EditorKeyDown += new System.Windows.Forms.KeyEventHandler(this.tv_budget_EditorKeyDown);
		this.tv_budget.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tv_budget_KeyDown);
		this.tb_AbsEntry.CurrencyDateField = null;
		this.tb_AbsEntry.CurrencyField = "";
		this.tb_AbsEntry.CurrencyFixed = null;
		this.tb_AbsEntry.CurrencyFixedRate = 0.0;
		this.tb_AbsEntry.CurrencyRateField = null;
		this.tb_AbsEntry.FieldName = "AbsEntry";
		this.tb_AbsEntry.Name = "tb_AbsEntry";
		this.tb_AbsEntry.OptionsColumn.ReadOnly = true;
		this.tb_AbsEntry.ShowButtonMode = DevExpress.XtraTreeList.ShowButtonModeEnum.ShowAlways;
		this.tb_LineNum.Caption = "LineNum";
		this.tb_LineNum.CurrencyDateField = null;
		this.tb_LineNum.CurrencyField = null;
		this.tb_LineNum.CurrencyFixed = null;
		this.tb_LineNum.CurrencyFixedRate = 0.0;
		this.tb_LineNum.CurrencyRateField = null;
		this.tb_LineNum.FieldName = "LineNum";
		this.tb_LineNum.Name = "tb_LineNum";
		this.tb_LineNum.OptionsColumn.ReadOnly = true;
		this.tb_ItemCode.Caption = "ItemCode";
		this.tb_ItemCode.ColumnEdit = this.btn_ItemCode;
		this.tb_ItemCode.CurrencyDateField = null;
		this.tb_ItemCode.CurrencyField = null;
		this.tb_ItemCode.CurrencyFixed = null;
		this.tb_ItemCode.CurrencyFixedRate = 0.0;
		this.tb_ItemCode.CurrencyRateField = null;
		this.tb_ItemCode.FieldName = "ItemCode";
		this.tb_ItemCode.Name = "tb_ItemCode";
		this.tb_ItemCode.ShowButtonMode = DevExpress.XtraTreeList.ShowButtonModeEnum.ShowAlways;
		this.btn_ItemCode.AutoHeight = false;
		this.btn_ItemCode.AutoLoadDataSource = true;
		editorButtonImageOptions11.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions25.Image");
		this.btn_ItemCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions10, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance20, appearanceHovered20, appearancePressed20, appearanceDisabled20, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions11, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance21, appearanceHovered21, appearancePressed21, appearanceDisabled21, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_ItemCode.ContainerControl = this;
		this.btn_ItemCode.DisplayMember = "";
		this.btn_ItemCode.DisplayMember2 = "";
		this.btn_ItemCode.ListMandatoryField = null;
		this.btn_ItemCode.LoadDataSourceWhenEnter = false;
		this.btn_ItemCode.Name = "btn_ItemCode";
		this.btn_ItemCode.SeparateCharacterForMulti = ';';
		this.btn_ItemCode.SeparateString = ", ";
		this.btn_ItemCode.UsingAutoCompleteBox = false;
		this.btn_ItemCode.UsingDataSource = true;
		this.btn_ItemCode.ValidateWhenNoDataSource = false;
		this.btn_ItemCode.ValueMember = "";
		this.tb_ItemName.Caption = "Item Name";
		this.tb_ItemName.CurrencyDateField = null;
		this.tb_ItemName.CurrencyField = null;
		this.tb_ItemName.CurrencyFixed = null;
		this.tb_ItemName.CurrencyFixedRate = 0.0;
		this.tb_ItemName.CurrencyRateField = null;
		this.tb_ItemName.FieldName = "Dscription";
		this.tb_ItemName.Name = "tb_ItemName";
		this.tb_TaskCode.Caption = "Wbs Code";
		this.tb_TaskCode.ColumnEdit = this.btn_TaskCode;
		this.tb_TaskCode.CurrencyDateField = null;
		this.tb_TaskCode.CurrencyField = null;
		this.tb_TaskCode.CurrencyFixed = null;
		this.tb_TaskCode.CurrencyFixedRate = 0.0;
		this.tb_TaskCode.CurrencyRateField = null;
		this.tb_TaskCode.FieldName = "TaskCode";
		this.tb_TaskCode.MinWidth = 34;
		this.tb_TaskCode.Name = "tb_TaskCode";
		this.tb_TaskCode.ShowButtonMode = DevExpress.XtraTreeList.ShowButtonModeEnum.ShowForFocusedRow;
		this.btn_TaskCode.AutoHeight = false;
		this.btn_TaskCode.AutoLoadDataSource = true;
		editorButtonImageOptions12.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions27.Image");
		this.btn_TaskCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions11, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance22, appearanceHovered22, appearancePressed22, appearanceDisabled22, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions12, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance23, appearanceHovered23, appearancePressed23, appearanceDisabled23, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_TaskCode.ContainerControl = this;
		this.btn_TaskCode.DisplayMember = "TaskCode";
		this.btn_TaskCode.DisplayMember2 = "";
		this.btn_TaskCode.ListMandatoryField = null;
		this.btn_TaskCode.LoadDataSourceWhenEnter = false;
		this.btn_TaskCode.Name = "btn_TaskCode";
		this.btn_TaskCode.SeparateCharacterForMulti = ';';
		this.btn_TaskCode.SeparateString = ", ";
		this.btn_TaskCode.UsingAutoCompleteBox = false;
		this.btn_TaskCode.UsingDataSource = true;
		this.btn_TaskCode.ValidateWhenNoDataSource = false;
		this.btn_TaskCode.ValueMember = "TaskCode";
		this.tb_TaskName.Caption = "Wbs Name";
		this.tb_TaskName.CurrencyDateField = null;
		this.tb_TaskName.CurrencyField = null;
		this.tb_TaskName.CurrencyFixed = null;
		this.tb_TaskName.CurrencyFixedRate = 0.0;
		this.tb_TaskName.CurrencyRateField = null;
		this.tb_TaskName.FieldName = "TaskName";
		this.tb_TaskName.Name = "tb_TaskName";
		this.tb_plQty.Caption = "Plan Quantity";
		this.tb_plQty.ColumnEdit = this.cal_PlanQty;
		this.tb_plQty.CurrencyDateField = null;
		this.tb_plQty.CurrencyField = null;
		this.tb_plQty.CurrencyFixed = null;
		this.tb_plQty.CurrencyFixedRate = 0.0;
		this.tb_plQty.CurrencyRateField = null;
		this.tb_plQty.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.tb_plQty.FieldName = "PlanQty";
		this.tb_plQty.Name = "tb_plQty";
		this.cal_PlanQty.AutoHeight = false;
		this.cal_PlanQty.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cal_PlanQty.Name = "cal_PlanQty";
		this.tb_RefPrice.Caption = "Ref Price";
		this.tb_RefPrice.ColumnEdit = this.txt_RefPrice;
		this.tb_RefPrice.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.tb_RefPrice.CurrencyDateField = null;
		this.tb_RefPrice.CurrencyField = "Currency";
		this.tb_RefPrice.CurrencyFixed = null;
		this.tb_RefPrice.CurrencyFixedRate = 0.0;
		this.tb_RefPrice.CurrencyRateField = null;
		this.tb_RefPrice.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.tb_RefPrice.FieldName = "RefPrice";
		this.tb_RefPrice.Name = "tb_RefPrice";
		this.txt_RefPrice.AutoHeight = false;
		this.txt_RefPrice.Name = "txt_RefPrice";
		this.tb_Amount.Caption = "Amount";
		this.tb_Amount.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.tb_Amount.CurrencyDateField = null;
		this.tb_Amount.CurrencyField = "Currency";
		this.tb_Amount.CurrencyFixed = null;
		this.tb_Amount.CurrencyFixedRate = 0.0;
		this.tb_Amount.CurrencyRateField = null;
		this.tb_Amount.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.tb_Amount.FieldName = "Amount";
		this.tb_Amount.Name = "tb_Amount";
		this.tb_Branch.Caption = "Branch";
		this.tb_Branch.ColumnEdit = this.luk_ItemBranch;
		this.tb_Branch.CurrencyDateField = null;
		this.tb_Branch.CurrencyField = null;
		this.tb_Branch.CurrencyFixed = null;
		this.tb_Branch.CurrencyFixedRate = 0.0;
		this.tb_Branch.CurrencyRateField = null;
		this.tb_Branch.FieldName = "BplId";
		this.tb_Branch.Name = "tb_Branch";
		this.tb_Branch.ShowButtonMode = DevExpress.XtraTreeList.ShowButtonModeEnum.ShowAlways;
		this.luk_ItemBranch.AutoHeight = false;
		this.luk_ItemBranch.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
		this.luk_ItemBranch.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_ItemBranch.CheckFields = null;
		this.luk_ItemBranch.CheckTables = null;
		this.luk_ItemBranch.Name = "luk_ItemBranch";
		this.luk_ItemBranch.NullText = "";
		this.luk_ItemBranch.ShowFooter = false;
		this.luk_ItemBranch.ShowHeader = false;
		this.tb_ActualQty.Caption = "Actual Quantity";
		this.tb_ActualQty.CurrencyDateField = null;
		this.tb_ActualQty.CurrencyField = null;
		this.tb_ActualQty.CurrencyFixed = null;
		this.tb_ActualQty.CurrencyFixedRate = 0.0;
		this.tb_ActualQty.CurrencyRateField = null;
		this.tb_ActualQty.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.tb_ActualQty.FieldName = "ActualQty";
		this.tb_ActualQty.Name = "tb_ActualQty";
		this.tb_ActualQty.OptionsColumn.ReadOnly = true;
		this.tb_QtyDiff.Caption = "QtyDiff";
		this.tb_QtyDiff.CurrencyDateField = null;
		this.tb_QtyDiff.CurrencyField = null;
		this.tb_QtyDiff.CurrencyFixed = null;
		this.tb_QtyDiff.CurrencyFixedRate = 0.0;
		this.tb_QtyDiff.CurrencyRateField = null;
		this.tb_QtyDiff.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.tb_QtyDiff.FieldName = "QtyDiff";
		this.tb_QtyDiff.Name = "tb_QtyDiff";
		this.tb_Notes.Caption = "Notes";
		this.tb_Notes.ColumnEdit = this.repositoryItemMemoExEdit3;
		this.tb_Notes.CurrencyDateField = null;
		this.tb_Notes.CurrencyField = null;
		this.tb_Notes.CurrencyFixed = null;
		this.tb_Notes.CurrencyFixedRate = 0.0;
		this.tb_Notes.CurrencyRateField = null;
		this.tb_Notes.FieldName = "Notes";
		this.tb_Notes.Name = "tb_Notes";
		this.repositoryItemMemoExEdit3.AutoHeight = false;
		this.repositoryItemMemoExEdit3.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemMemoExEdit3.Name = "repositoryItemMemoExEdit3";
		this.repositoryItemMemoExEdit3.ShowIcon = false;
		this.tb_FatherNum.Caption = "FatherNum";
		this.tb_FatherNum.CurrencyDateField = null;
		this.tb_FatherNum.CurrencyField = null;
		this.tb_FatherNum.CurrencyFixed = null;
		this.tb_FatherNum.CurrencyFixedRate = 0.0;
		this.tb_FatherNum.CurrencyRateField = null;
		this.tb_FatherNum.FieldName = "FatherNum";
		this.tb_FatherNum.Name = "tb_FatherNum";
		this.tb_FatherNum.OptionsColumn.AllowEdit = false;
		this.tb_FatherNum.OptionsColumn.ReadOnly = true;
		this.tb_IsNew.Caption = "IsNew";
		this.tb_IsNew.CurrencyDateField = null;
		this.tb_IsNew.CurrencyField = null;
		this.tb_IsNew.CurrencyFixed = null;
		this.tb_IsNew.CurrencyFixedRate = 0.0;
		this.tb_IsNew.CurrencyRateField = null;
		this.tb_IsNew.FieldName = "IsNew";
		this.tb_IsNew.Name = "tb_IsNew";
		this.tb_IsNew.OptionsColumn.AllowEdit = false;
		this.tb_IsNew.OptionsColumn.ReadOnly = true;
		this.tb_ActualAmount.Caption = "ActualAmount";
		this.tb_ActualAmount.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.tb_ActualAmount.CurrencyDateField = null;
		this.tb_ActualAmount.CurrencyField = "Currency";
		this.tb_ActualAmount.CurrencyFixed = null;
		this.tb_ActualAmount.CurrencyFixedRate = 0.0;
		this.tb_ActualAmount.CurrencyRateField = null;
		this.tb_ActualAmount.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.tb_ActualAmount.FieldName = "ActualAmount";
		this.tb_ActualAmount.Name = "tb_ActualAmount";
		this.tb_ActualAmount.OptionsColumn.ReadOnly = true;
		this.tb_FromDate.Caption = "FromDate";
		this.tb_FromDate.ColumnEdit = this.dat_IFromDate;
		this.tb_FromDate.CurrencyDateField = null;
		this.tb_FromDate.CurrencyField = null;
		this.tb_FromDate.CurrencyFixed = null;
		this.tb_FromDate.CurrencyFixedRate = 0.0;
		this.tb_FromDate.CurrencyRateField = null;
		this.tb_FromDate.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.tb_FromDate.FieldName = "FromDate";
		this.tb_FromDate.Name = "tb_FromDate";
		this.dat_IFromDate.AutoHeight = false;
		this.dat_IFromDate.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dat_IFromDate.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dat_IFromDate.EditFormat.FormatString = "";
		this.dat_IFromDate.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.dat_IFromDate.IsSearchControl = false;
		this.dat_IFromDate.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.dat_IFromDate.Name = "dat_IFromDate";
		this.tb_ToDate.Caption = "ToDate";
		this.tb_ToDate.ColumnEdit = this.dat_IToDate;
		this.tb_ToDate.CurrencyDateField = null;
		this.tb_ToDate.CurrencyField = null;
		this.tb_ToDate.CurrencyFixed = null;
		this.tb_ToDate.CurrencyFixedRate = 0.0;
		this.tb_ToDate.CurrencyRateField = null;
		this.tb_ToDate.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.tb_ToDate.FieldName = "ToDate";
		this.tb_ToDate.Name = "tb_ToDate";
		this.dat_IToDate.AutoHeight = false;
		this.dat_IToDate.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dat_IToDate.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dat_IToDate.EditFormat.FormatString = "";
		this.dat_IToDate.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.dat_IToDate.IsSearchControl = false;
		this.dat_IToDate.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.dat_IToDate.Name = "dat_IToDate";
		this.tb_BomItem.Caption = "BomItem";
		this.tb_BomItem.ColumnEdit = this.btn_BOM;
		this.tb_BomItem.CurrencyDateField = null;
		this.tb_BomItem.CurrencyField = null;
		this.tb_BomItem.CurrencyFixed = null;
		this.tb_BomItem.CurrencyFixedRate = 0.0;
		this.tb_BomItem.CurrencyRateField = null;
		this.tb_BomItem.FieldName = "BomItem";
		this.tb_BomItem.Name = "tb_BomItem";
		this.tb_BomItem.ShowButtonMode = DevExpress.XtraTreeList.ShowButtonModeEnum.ShowAlways;
		this.btn_BOM.AutoHeight = false;
		this.btn_BOM.AutoLoadDataSource = true;
		editorButtonImageOptions13.Image = Apzon.Forms.Properties.Resources.right;
		this.btn_BOM.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions12, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance24, appearanceHovered24, appearancePressed24, appearanceDisabled24, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions13, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance25, appearanceHovered25, appearancePressed25, appearanceDisabled25, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_BOM.ContainerControl = this;
		this.btn_BOM.DisplayMember = "Code";
		this.btn_BOM.DisplayMember2 = "";
		this.btn_BOM.ListMandatoryField = null;
		this.btn_BOM.LoadDataSourceWhenEnter = false;
		this.btn_BOM.Name = "btn_BOM";
		this.btn_BOM.SeparateCharacterForMulti = ';';
		this.btn_BOM.SeparateString = ", ";
		this.btn_BOM.UsingAutoCompleteBox = false;
		this.btn_BOM.UsingDataSource = true;
		this.btn_BOM.ValidateWhenNoDataSource = false;
		this.btn_BOM.ValueMember = "Code";
		this.tb_OrderItem.Caption = "OrderItem";
		this.tb_OrderItem.ColumnEdit = this.btn_OrderItem;
		this.tb_OrderItem.CurrencyDateField = null;
		this.tb_OrderItem.CurrencyField = null;
		this.tb_OrderItem.CurrencyFixed = null;
		this.tb_OrderItem.CurrencyFixedRate = 0.0;
		this.tb_OrderItem.CurrencyRateField = null;
		this.tb_OrderItem.FieldName = "OrderItem";
		this.tb_OrderItem.Name = "tb_OrderItem";
		this.tb_OrderItem.ShowButtonMode = DevExpress.XtraTreeList.ShowButtonModeEnum.ShowAlways;
		this.btn_OrderItem.AutoHeight = false;
		this.btn_OrderItem.AutoLoadDataSource = true;
		this.btn_OrderItem.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions13, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance26, appearanceHovered26, appearancePressed26, appearanceDisabled26, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_OrderItem.ContainerControl = this;
		this.btn_OrderItem.DisplayMember = "LineNum";
		this.btn_OrderItem.DisplayMember2 = "";
		this.btn_OrderItem.ListMandatoryField = null;
		this.btn_OrderItem.LoadDataSourceWhenEnter = false;
		this.btn_OrderItem.Name = "btn_OrderItem";
		this.btn_OrderItem.SeparateCharacterForMulti = ';';
		this.btn_OrderItem.SeparateString = ", ";
		this.btn_OrderItem.UsingAutoCompleteBox = false;
		this.btn_OrderItem.UsingDataSource = true;
		this.btn_OrderItem.ValidateWhenNoDataSource = false;
		this.btn_OrderItem.ValueMember = "LineNum";
		this.tb_PR.Caption = "Purchase Request";
		this.tb_PR.ColumnEdit = this.btn_PR;
		this.tb_PR.CurrencyDateField = null;
		this.tb_PR.CurrencyField = null;
		this.tb_PR.CurrencyFixed = null;
		this.tb_PR.CurrencyFixedRate = 0.0;
		this.tb_PR.CurrencyRateField = null;
		this.tb_PR.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.tb_PR.FieldName = "PR";
		this.tb_PR.Name = "tb_PR";
		this.tb_PR.OptionsColumn.ReadOnly = true;
		this.tb_PR.ShowButtonMode = DevExpress.XtraTreeList.ShowButtonModeEnum.ShowAlways;
		this.tb_PR.Width = 126;
		this.btn_PR.AutoHeight = false;
		editorButtonImageOptions14.Image = Apzon.Forms.Properties.Resources.right;
		this.btn_PR.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions14, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance27, appearanceHovered27, appearancePressed27, appearanceDisabled27, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_PR.Name = "btn_PR";
		this.btn_PR.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btn_PR_ButtonClick);
		this.tb_PO.Caption = "Purchase Order";
		this.tb_PO.ColumnEdit = this.btn_PO;
		this.tb_PO.CurrencyDateField = null;
		this.tb_PO.CurrencyField = null;
		this.tb_PO.CurrencyFixed = null;
		this.tb_PO.CurrencyFixedRate = 0.0;
		this.tb_PO.CurrencyRateField = null;
		this.tb_PO.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.tb_PO.FieldName = "PO";
		this.tb_PO.Name = "tb_PO";
		this.tb_PO.OptionsColumn.ReadOnly = true;
		this.tb_PO.ShowButtonMode = DevExpress.XtraTreeList.ShowButtonModeEnum.ShowAlways;
		this.tb_PO.Width = 102;
		this.btn_PO.AutoHeight = false;
		editorButtonImageOptions15.Image = Apzon.Forms.Properties.Resources.right;
		this.btn_PO.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions15, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance28, appearanceHovered28, appearancePressed28, appearanceDisabled28, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_PO.Name = "btn_PO";
		this.btn_PO.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btn_PO_ButtonClick);
		this.tb_GRPO.Caption = "Goods Receipt  PO";
		this.tb_GRPO.ColumnEdit = this.btn_GRPO;
		this.tb_GRPO.CurrencyDateField = null;
		this.tb_GRPO.CurrencyField = null;
		this.tb_GRPO.CurrencyFixed = null;
		this.tb_GRPO.CurrencyFixedRate = 0.0;
		this.tb_GRPO.CurrencyRateField = null;
		this.tb_GRPO.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.tb_GRPO.FieldName = "GRPO";
		this.tb_GRPO.Name = "tb_GRPO";
		this.tb_GRPO.OptionsColumn.ReadOnly = true;
		this.tb_GRPO.ShowButtonMode = DevExpress.XtraTreeList.ShowButtonModeEnum.ShowAlways;
		this.tb_GRPO.Width = 111;
		this.btn_GRPO.AutoHeight = false;
		editorButtonImageOptions16.Image = Apzon.Forms.Properties.Resources.right;
		this.btn_GRPO.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions16, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance29, appearanceHovered29, appearancePressed29, appearanceDisabled29, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_GRPO.Name = "btn_GRPO";
		this.btn_GRPO.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btn_GRPO_ButtonClick);
		this.tb_Delivery.Caption = "Delivery";
		this.tb_Delivery.ColumnEdit = this.btn_Delivery;
		this.tb_Delivery.CurrencyDateField = null;
		this.tb_Delivery.CurrencyField = null;
		this.tb_Delivery.CurrencyFixed = null;
		this.tb_Delivery.CurrencyFixedRate = 0.0;
		this.tb_Delivery.CurrencyRateField = null;
		this.tb_Delivery.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.tb_Delivery.FieldName = "Delivery";
		this.tb_Delivery.Name = "tb_Delivery";
		this.tb_Delivery.OptionsColumn.ReadOnly = true;
		this.tb_Delivery.ShowButtonMode = DevExpress.XtraTreeList.ShowButtonModeEnum.ShowAlways;
		this.btn_Delivery.AutoHeight = false;
		editorButtonImageOptions17.Image = Apzon.Forms.Properties.Resources.right;
		this.btn_Delivery.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions17, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance30, appearanceHovered30, appearancePressed30, appearanceDisabled30, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_Delivery.Name = "btn_Delivery";
		this.btn_Delivery.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btn_Delivery_ButtonClick);
		this.tb_Invoice.Caption = "A/P Invoice";
		this.tb_Invoice.ColumnEdit = this.btn_APInvoice;
		this.tb_Invoice.CurrencyDateField = null;
		this.tb_Invoice.CurrencyField = null;
		this.tb_Invoice.CurrencyFixed = null;
		this.tb_Invoice.CurrencyFixedRate = 0.0;
		this.tb_Invoice.CurrencyRateField = null;
		this.tb_Invoice.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.tb_Invoice.FieldName = "Invoice";
		this.tb_Invoice.Name = "tb_Invoice";
		this.tb_Invoice.OptionsColumn.ReadOnly = true;
		this.tb_Invoice.ShowButtonMode = DevExpress.XtraTreeList.ShowButtonModeEnum.ShowAlways;
		this.btn_APInvoice.AutoHeight = false;
		editorButtonImageOptions18.Image = Apzon.Forms.Properties.Resources.right;
		this.btn_APInvoice.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions18, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance31, appearanceHovered31, appearancePressed31, appearanceDisabled31, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_APInvoice.Name = "btn_APInvoice";
		this.btn_APInvoice.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btn_APInvoice_ButtonClick);
		this.tb_UoM.Caption = "UoM";
		this.tb_UoM.CurrencyDateField = null;
		this.tb_UoM.CurrencyField = null;
		this.tb_UoM.CurrencyFixed = null;
		this.tb_UoM.CurrencyFixedRate = 0.0;
		this.tb_UoM.CurrencyRateField = null;
		this.tb_UoM.FieldName = "UoM";
		this.tb_UoM.Name = "tb_UoM";
		this.tb_UoM.ShowButtonMode = DevExpress.XtraTreeList.ShowButtonModeEnum.ShowAlways;
		this.tb_BaseLine.Caption = "BaseLine";
		this.tb_BaseLine.CurrencyDateField = null;
		this.tb_BaseLine.CurrencyField = null;
		this.tb_BaseLine.CurrencyFixed = null;
		this.tb_BaseLine.CurrencyFixedRate = 0.0;
		this.tb_BaseLine.CurrencyRateField = null;
		this.tb_BaseLine.FieldName = "BaseLine";
		this.tb_BaseLine.Name = "tb_BaseLine";
		this.tb_BaseLine.OptionsColumn.AllowEdit = false;
		this.tb_BaseLine.OptionsColumn.ReadOnly = true;
		this.tb_AR.Caption = "A/R Invoice Amount";
		this.tb_AR.ColumnEdit = this.btn_ARInvoice;
		this.tb_AR.CurrencyDateField = null;
		this.tb_AR.CurrencyField = null;
		this.tb_AR.CurrencyFixed = null;
		this.tb_AR.CurrencyFixedRate = 0.0;
		this.tb_AR.CurrencyRateField = null;
		this.tb_AR.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.tb_AR.FieldName = "AR";
		this.tb_AR.Name = "tb_AR";
		this.tb_AR.OptionsColumn.ReadOnly = true;
		this.tb_AR.ShowButtonMode = DevExpress.XtraTreeList.ShowButtonModeEnum.ShowAlways;
		this.btn_ARInvoice.AutoHeight = false;
		editorButtonImageOptions19.Image = Apzon.Forms.Properties.Resources.right;
		this.btn_ARInvoice.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions19, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance32, appearanceHovered32, appearancePressed32, appearanceDisabled32, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_ARInvoice.Name = "btn_ARInvoice";
		this.btn_ARInvoice.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btn_ARInvoice_ButtonClick);
		this.tb_BaseQty.Caption = "BaseQty";
		this.tb_BaseQty.CurrencyDateField = null;
		this.tb_BaseQty.CurrencyField = null;
		this.tb_BaseQty.CurrencyFixed = null;
		this.tb_BaseQty.CurrencyFixedRate = 0.0;
		this.tb_BaseQty.CurrencyRateField = null;
		this.tb_BaseQty.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.tb_BaseQty.FieldName = "BaseQty";
		this.tb_BaseQty.Name = "tb_BaseQty";
		this.tb_BaseQty.OptionsColumn.ReadOnly = true;
		this.tb_ItmsGrpCod.Caption = "Item Group";
		this.tb_ItmsGrpCod.ColumnEdit = this.luk_ItmsGrpCod;
		this.tb_ItmsGrpCod.CurrencyDateField = null;
		this.tb_ItmsGrpCod.CurrencyField = null;
		this.tb_ItmsGrpCod.CurrencyFixed = null;
		this.tb_ItmsGrpCod.CurrencyFixedRate = 0.0;
		this.tb_ItmsGrpCod.CurrencyRateField = null;
		this.tb_ItmsGrpCod.FieldName = "ItmsGrpCod";
		this.tb_ItmsGrpCod.Name = "tb_ItmsGrpCod";
		this.tb_ItmsGrpCod.OptionsColumn.ReadOnly = true;
		this.tb_ItmsGrpCod.ShowButtonMode = DevExpress.XtraTreeList.ShowButtonModeEnum.ShowAlways;
		this.luk_ItmsGrpCod.AutoHeight = false;
		this.luk_ItmsGrpCod.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
		this.luk_ItmsGrpCod.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_ItmsGrpCod.CheckFields = null;
		this.luk_ItmsGrpCod.CheckTables = null;
		this.luk_ItmsGrpCod.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ItmsGrpNam", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ItmsGrpCod", "Code")
		});
		this.luk_ItmsGrpCod.Name = "luk_ItmsGrpCod";
		this.luk_ItmsGrpCod.NullText = "";
		this.luk_ItmsGrpCod.ShowFooter = false;
		this.luk_ItmsGrpCod.ShowHeader = false;
		this.tb_DefaultWhs.Caption = "DefaultWhs";
		this.tb_DefaultWhs.ColumnEdit = this.btn_WarehouseItem;
		this.tb_DefaultWhs.CurrencyDateField = null;
		this.tb_DefaultWhs.CurrencyField = null;
		this.tb_DefaultWhs.CurrencyFixed = null;
		this.tb_DefaultWhs.CurrencyFixedRate = 0.0;
		this.tb_DefaultWhs.CurrencyRateField = null;
		this.tb_DefaultWhs.FieldName = "DefaultWhs";
		this.tb_DefaultWhs.Name = "tb_DefaultWhs";
		this.tb_DefaultWhs.ShowButtonMode = DevExpress.XtraTreeList.ShowButtonModeEnum.ShowAlways;
		this.btn_WarehouseItem.AutoHeight = false;
		this.btn_WarehouseItem.AutoLoadDataSource = true;
		editorButtonImageOptions20.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions38.Image");
		this.btn_WarehouseItem.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions14, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance33, appearanceHovered33, appearancePressed33, appearanceDisabled33, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions20, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance34, appearanceHovered34, appearancePressed34, appearanceDisabled34, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_WarehouseItem.ContainerControl = this;
		this.btn_WarehouseItem.DisplayMember = "WhsCode";
		this.btn_WarehouseItem.DisplayMember2 = "";
		this.btn_WarehouseItem.ListMandatoryField = null;
		this.btn_WarehouseItem.LoadDataSourceWhenEnter = false;
		this.btn_WarehouseItem.Name = "btn_WarehouseItem";
		this.btn_WarehouseItem.SeparateCharacterForMulti = ';';
		this.btn_WarehouseItem.SeparateString = ", ";
		this.btn_WarehouseItem.UsingAutoCompleteBox = false;
		this.btn_WarehouseItem.UsingDataSource = true;
		this.btn_WarehouseItem.ValidateWhenNoDataSource = false;
		this.btn_WarehouseItem.ValueMember = "WhsCode";
		this.tb_VisOrder.Caption = "VisOrder";
		this.tb_VisOrder.CurrencyDateField = null;
		this.tb_VisOrder.CurrencyField = null;
		this.tb_VisOrder.CurrencyFixed = null;
		this.tb_VisOrder.CurrencyFixedRate = 0.0;
		this.tb_VisOrder.CurrencyRateField = null;
		this.tb_VisOrder.FieldName = "VisOrder";
		this.tb_VisOrder.Name = "tb_VisOrder";
		this.tb_VisOrder.OptionsColumn.AllowEdit = false;
		this.tb_VisOrder.OptionsColumn.ShowInCustomizationForm = false;
		this.tb_VisOrder.SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
		this.tb_Currency.Caption = "Currency";
		this.tb_Currency.ColumnEdit = this.luk_Currency;
		this.tb_Currency.CurrencyDateField = null;
		this.tb_Currency.CurrencyField = null;
		this.tb_Currency.CurrencyFixed = null;
		this.tb_Currency.CurrencyFixedRate = 0.0;
		this.tb_Currency.CurrencyRateField = null;
		this.tb_Currency.FieldName = "Currency";
		this.tb_Currency.Name = "tb_Currency";
		this.luk_Currency.AutoHeight = false;
		this.luk_Currency.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Currency.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("CurrCode", "Code")
		});
		this.luk_Currency.KeyMember = "CurrCode";
		this.luk_Currency.Name = "luk_Currency";
		this.luk_Currency.NullText = "";
		this.luk_Currency.ValueMember = "CurrCode";
		this.tb_PQ.Caption = "Purchase Quotation";
		this.tb_PQ.ColumnEdit = this.btn_PQ;
		this.tb_PQ.CurrencyDateField = null;
		this.tb_PQ.CurrencyField = null;
		this.tb_PQ.CurrencyFixed = null;
		this.tb_PQ.CurrencyFixedRate = 0.0;
		this.tb_PQ.CurrencyRateField = null;
		this.tb_PQ.FieldName = "PQ";
		this.tb_PQ.Name = "tb_PQ";
		this.tb_PQ.OptionsColumn.ReadOnly = true;
		this.tb_PQ.ShowButtonMode = DevExpress.XtraTreeList.ShowButtonModeEnum.ShowAlways;
		this.tb_PQ.Width = 115;
		this.btn_PQ.AutoHeight = false;
		editorButtonImageOptions21.Image = Apzon.Forms.Properties.Resources.right;
		this.btn_PQ.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions21, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance35, appearanceHovered35, appearancePressed35, appearanceDisabled35, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_PQ.Name = "btn_PQ";
		this.btn_PQ.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btn_PQ_ButtonClick_1);
		this.tb_ARDR.Caption = "A/R Downpayment Request Amount";
		this.tb_ARDR.CurrencyDateField = null;
		this.tb_ARDR.CurrencyField = null;
		this.tb_ARDR.CurrencyFixed = null;
		this.tb_ARDR.CurrencyFixedRate = 0.0;
		this.tb_ARDR.CurrencyRateField = null;
		this.tb_ARDR.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.tb_ARDR.FieldName = "ARDR";
		this.tb_ARDR.Name = "tb_ARDR";
		this.tb_ARDR.OptionsColumn.ReadOnly = true;
		this.tb_APDR.Caption = "A/P Downpayment Request Amount";
		this.tb_APDR.CurrencyDateField = null;
		this.tb_APDR.CurrencyField = null;
		this.tb_APDR.CurrencyFixed = null;
		this.tb_APDR.CurrencyFixedRate = 0.0;
		this.tb_APDR.CurrencyRateField = null;
		this.tb_APDR.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.tb_APDR.FieldName = "APDR";
		this.tb_APDR.Name = "tb_APDR";
		this.tb_APDR.OptionsColumn.ReadOnly = true;
		this.tb_IGE.Caption = "Goods Issue";
		this.tb_IGE.ColumnEdit = this.btn_IGE;
		this.tb_IGE.CurrencyDateField = null;
		this.tb_IGE.CurrencyField = null;
		this.tb_IGE.CurrencyFixed = null;
		this.tb_IGE.CurrencyFixedRate = 0.0;
		this.tb_IGE.CurrencyRateField = null;
		this.tb_IGE.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.tb_IGE.FieldName = "IGE";
		this.tb_IGE.Name = "tb_IGE";
		this.tb_IGE.OptionsColumn.ReadOnly = true;
		this.tb_IGE.ShowButtonMode = DevExpress.XtraTreeList.ShowButtonModeEnum.ShowAlways;
		this.btn_IGE.AutoHeight = false;
		this.btn_IGE.AutoLoadDataSource = true;
		editorButtonImageOptions22.Image = Apzon.Forms.Properties.Resources.right;
		this.btn_IGE.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions22, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance36, appearanceHovered36, appearancePressed36, appearanceDisabled36, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_IGE.ContainerControl = this;
		this.btn_IGE.DisplayMember = "";
		this.btn_IGE.DisplayMember2 = "";
		this.btn_IGE.ListMandatoryField = null;
		this.btn_IGE.LoadDataSourceWhenEnter = false;
		this.btn_IGE.Name = "btn_IGE";
		this.btn_IGE.SeparateCharacterForMulti = ';';
		this.btn_IGE.SeparateString = ", ";
		this.btn_IGE.UsingAutoCompleteBox = false;
		this.btn_IGE.UsingDataSource = true;
		this.btn_IGE.ValidateWhenNoDataSource = false;
		this.btn_IGE.ValueMember = "";
		this.btn_IGE.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.Btn_IGE_ButtonClick);
		this.treeListColumnEx1.Caption = "Production Order";
		this.treeListColumnEx1.ColumnEdit = this.btn_WOR;
		this.treeListColumnEx1.CurrencyDateField = null;
		this.treeListColumnEx1.CurrencyField = null;
		this.treeListColumnEx1.CurrencyFixed = null;
		this.treeListColumnEx1.CurrencyFixedRate = 0.0;
		this.treeListColumnEx1.CurrencyRateField = null;
		this.treeListColumnEx1.FieldName = "WOR";
		this.treeListColumnEx1.Name = "treeListColumnEx1";
		this.treeListColumnEx1.ShowButtonMode = DevExpress.XtraTreeList.ShowButtonModeEnum.ShowAlways;
		this.btn_WOR.AutoHeight = false;
		this.btn_WOR.AutoLoadDataSource = true;
		editorButtonImageOptions23.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions42.Image");
		this.btn_WOR.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions15, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance37, appearanceHovered37, appearancePressed37, appearanceDisabled37, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions23, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance38, appearanceHovered38, appearancePressed38, appearanceDisabled38, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_WOR.ContainerControl = this;
		this.btn_WOR.DisplayMember = "";
		this.btn_WOR.DisplayMember2 = "";
		this.btn_WOR.ListMandatoryField = null;
		this.btn_WOR.LoadDataSourceWhenEnter = false;
		this.btn_WOR.Name = "btn_WOR";
		this.btn_WOR.SeparateCharacterForMulti = ';';
		this.btn_WOR.SeparateString = ", ";
		this.btn_WOR.UsingAutoCompleteBox = false;
		this.btn_WOR.UsingDataSource = true;
		this.btn_WOR.ValidateWhenNoDataSource = false;
		this.btn_WOR.ValueMember = "";
		this.btn_WOR.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btn_WOR_ButtonClick);
		this.treeListColumnEx3.Caption = "Comments";
		this.treeListColumnEx3.ColumnEdit = this.rep_BudgetComments;
		this.treeListColumnEx3.CurrencyDateField = null;
		this.treeListColumnEx3.CurrencyField = null;
		this.treeListColumnEx3.CurrencyFixed = null;
		this.treeListColumnEx3.CurrencyFixedRate = 0.0;
		this.treeListColumnEx3.CurrencyRateField = null;
		this.treeListColumnEx3.FieldName = "CommentsCount";
		this.treeListColumnEx3.Name = "treeListColumnEx3";
		this.treeListColumnEx3.ShowButtonMode = DevExpress.XtraTreeList.ShowButtonModeEnum.ShowAlways;
		this.rep_BudgetComments.AutoHeight = false;
		editorButtonImageOptions24.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("editorButtonImageOptions43.SvgImage");
		editorButtonImageOptions24.SvgImageSize = new System.Drawing.Size(16, 16);
		this.rep_BudgetComments.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, editorButtonImageOptions24, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance39, appearanceHovered39, appearancePressed39, appearanceDisabled39, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_BudgetComments.Name = "rep_BudgetComments";
		this.rep_BudgetComments.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_BudgetComments_ButtonClick);
		this.ctm_BudgetItem.ImageScalingSize = new System.Drawing.Size(32, 32);
		this.ctm_BudgetItem.Items.AddRange(new System.Windows.Forms.ToolStripItem[8] { this.addRowToolStripMenuItem1, this.toolStripMenuItemInsertRowBudget, this.toolStripSeparator10, this.toolStripMenuItemDuplicateRowBudget, this.toolStripSeparator9, this.removeRowToolStripMenuItem, this.addItemRowToolStripMenuItem, this.insertItemRowToolStripMenuItem });
		this.ctm_BudgetItem.Name = "ctm_BudgetItem";
		this.ctm_BudgetItem.Size = new System.Drawing.Size(197, 148);
		this.addRowToolStripMenuItem1.Name = "addRowToolStripMenuItem1";
		this.addRowToolStripMenuItem1.Size = new System.Drawing.Size(196, 22);
		this.addRowToolStripMenuItem1.Text = "Add Row";
		this.addRowToolStripMenuItem1.Click += new System.EventHandler(this.addRowToolStripMenuItem1_Click);
		this.toolStripMenuItemInsertRowBudget.Name = "toolStripMenuItemInsertRowBudget";
		this.toolStripMenuItemInsertRowBudget.Size = new System.Drawing.Size(196, 22);
		this.toolStripMenuItemInsertRowBudget.Text = "Insert Row";
		this.toolStripMenuItemInsertRowBudget.Click += new System.EventHandler(this.toolStripMenuItemInsertRowBudget_Click);
		this.toolStripSeparator10.Name = "toolStripSeparator10";
		this.toolStripSeparator10.Size = new System.Drawing.Size(193, 6);
		this.toolStripMenuItemDuplicateRowBudget.Name = "toolStripMenuItemDuplicateRowBudget";
		this.toolStripMenuItemDuplicateRowBudget.Size = new System.Drawing.Size(196, 22);
		this.toolStripMenuItemDuplicateRowBudget.Text = "Duplicate";
		this.toolStripMenuItemDuplicateRowBudget.Click += new System.EventHandler(this.toolStripMenuItemDuplicateRowBudget_Click);
		this.toolStripSeparator9.Name = "toolStripSeparator9";
		this.toolStripSeparator9.Size = new System.Drawing.Size(193, 6);
		this.removeRowToolStripMenuItem.Name = "removeRowToolStripMenuItem";
		this.removeRowToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
		this.removeRowToolStripMenuItem.Text = "Delete Row";
		this.removeRowToolStripMenuItem.Click += new System.EventHandler(this.removeRowToolStripMenuItem_Click);
		this.addItemRowToolStripMenuItem.Name = "addItemRowToolStripMenuItem";
		this.addItemRowToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
		this.addItemRowToolStripMenuItem.Text = "Add Item Row (Enter)";
		this.addItemRowToolStripMenuItem.Click += new System.EventHandler(this.addItemRowToolStripMenuItem_Click);
		this.insertItemRowToolStripMenuItem.Name = "insertItemRowToolStripMenuItem";
		this.insertItemRowToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
		this.insertItemRowToolStripMenuItem.Text = "Insert Item Row (Insert)";
		this.insertItemRowToolStripMenuItem.Click += new System.EventHandler(this.insertItemRowToolStripMenuItem_Click);
		this.btn_AbsEntry.AutoHeight = false;
		editorButtonImageOptions25.Image = Apzon.Forms.Properties.Resources.right;
		this.btn_AbsEntry.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Plus),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, editorButtonImageOptions25, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance40, appearanceHovered40, appearancePressed40, appearanceDisabled40, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_AbsEntry.Name = "btn_AbsEntry";
		this.btn_AbsEntry.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btn_AbsEntry_ButtonClick);
		this.luk_UoM.AutoHeight = false;
		this.luk_UoM.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
		this.luk_UoM.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_UoM.CheckFields = null;
		this.luk_UoM.CheckTables = null;
		this.luk_UoM.Name = "luk_UoM";
		this.luk_UoM.NullText = "";
		this.luk_UoM.ShowFooter = false;
		this.luk_UoM.ShowHeader = false;
		this.luk_ItemGroup.AutoHeight = false;
		this.luk_ItemGroup.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_ItemGroup.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ITMSGRPNAM", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ITMSGRPCOD", "Code")
		});
		this.luk_ItemGroup.DisplayMember = "ItmsGrpNam";
		this.luk_ItemGroup.Name = "luk_ItemGroup";
		this.luk_ItemGroup.NullText = "";
		this.luk_ItemGroup.ValueMember = "ItmsGrpCod";
		this.xtra_Payment.Controls.Add(this.tv_Payments);
		this.xtra_Payment.EditControl = null;
		this.xtra_Payment.EditValue = "Payments";
		this.xtra_Payment.FieldName = "xtra_Payment";
		this.xtra_Payment.IsSearchControl = false;
		this.xtra_Payment.Margin = new System.Windows.Forms.Padding(2);
		this.xtra_Payment.Name = "xtra_Payment";
		this.xtra_Payment.ReadOnly = false;
		this.xtra_Payment.SettingInVisible = false;
		this.xtra_Payment.SettingReadonly = false;
		this.xtra_Payment.Size = new System.Drawing.Size(878, 183);
		this.xtra_Payment.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtra_Payment.Text = "Payments";
		this.tv_Payments.AllowPasteExcel = false;
		this.tv_Payments.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.tv_Payments.ColumnKey = null;
		this.tv_Payments.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[13]
		{
			this.tp_AbsEntry, this.tp_TaskCode, this.tp_TaskName, this.tp_InvoicePlan, this.tp_IncomingPayment, this.tp_Payments, this.tp_Duedate, this.tp_Remarks, this.tp_LineNum, this.tp_FatherNum,
			this.tp_IsNew, this.tp_BaseLine, this.tp_VisOrder
		});
		this.tv_Payments.ContextMenuStrip = this.ctm_Payments;
		this.tv_Payments.Cursor = System.Windows.Forms.Cursors.Default;
		this.tv_Payments.DisableFormSetting = false;
		this.tv_Payments.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tv_Payments.EditColumn = null;
		this.tv_Payments.FieldCurrency = null;
		this.tv_Payments.FieldRate = null;
		this.tv_Payments.FormId = "oProjectManagement";
		this.tv_Payments.HasFormSetting = true;
		this.tv_Payments.IndexEditRow = 0;
		this.tv_Payments.IndicatorWidth = 40;
		this.tv_Payments.IsApzTable = false;
		this.tv_Payments.IsLoadingData = false;
		this.tv_Payments.ItemId = "APZ_PRG6";
		this.tv_Payments.KeyFieldName = "LineNum";
		this.tv_Payments.Location = new System.Drawing.Point(0, 0);
		this.tv_Payments.LstColumnNotEditable = null;
		this.tv_Payments.LstColumNotSetting = null;
		this.tv_Payments.LstOldUDF = null;
		this.tv_Payments.Margin = new System.Windows.Forms.Padding(2);
		this.tv_Payments.Name = "tv_Payments";
		this.tv_Payments.ParentFieldName = "FatherNum";
		this.tv_Payments.ReadOnly = false;
		this.tv_Payments.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.btn_WbsPayment });
		this.tv_Payments.SettingReadonly = false;
		this.tv_Payments.Size = new System.Drawing.Size(878, 183);
		this.tv_Payments.TabIndex = 0;
		this.tv_Payments.CellValueChanged += new DevExpress.XtraTreeList.CellValueChangedEventHandler(this.tv_Payments_CellValueChanged);
		this.tv_Payments.NodeCellStyle += new DevExpress.XtraTreeList.GetCustomNodeCellStyleEventHandler(this.tv_Payments_NodeCellStyle);
		this.tv_Payments.FocusedNodeChanged += new DevExpress.XtraTreeList.FocusedNodeChangedEventHandler(this.tv_Payments_FocusedNodeChanged);
		this.tv_Payments.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.tv_Payments_ShowingEditor);
		this.tv_Payments.EditorKeyDown += new System.Windows.Forms.KeyEventHandler(this.tv_Payments_EditorKeyDown);
		this.tp_AbsEntry.Caption = "AbsEntry";
		this.tp_AbsEntry.CurrencyDateField = null;
		this.tp_AbsEntry.CurrencyField = null;
		this.tp_AbsEntry.CurrencyFixed = null;
		this.tp_AbsEntry.CurrencyFixedRate = 0.0;
		this.tp_AbsEntry.CurrencyRateField = null;
		this.tp_AbsEntry.FieldName = "AbsEntry";
		this.tp_AbsEntry.Name = "tp_AbsEntry";
		this.tp_AbsEntry.OptionsColumn.AllowEdit = false;
		this.tp_AbsEntry.OptionsColumn.ReadOnly = true;
		this.tp_TaskCode.Caption = "Wbs Code";
		this.tp_TaskCode.ColumnEdit = this.btn_WbsPayment;
		this.tp_TaskCode.CurrencyDateField = null;
		this.tp_TaskCode.CurrencyField = null;
		this.tp_TaskCode.CurrencyFixed = null;
		this.tp_TaskCode.CurrencyFixedRate = 0.0;
		this.tp_TaskCode.CurrencyRateField = null;
		this.tp_TaskCode.FieldName = "TaskCode";
		this.tp_TaskCode.Name = "tp_TaskCode";
		this.tp_TaskCode.ShowButtonMode = DevExpress.XtraTreeList.ShowButtonModeEnum.ShowForFocusedRow;
		this.btn_WbsPayment.AutoHeight = false;
		this.btn_WbsPayment.AutoLoadDataSource = true;
		editorButtonImageOptions26.Image = Apzon.Forms.Properties.Resources.right;
		this.btn_WbsPayment.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions16, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance41, appearanceHovered41, appearancePressed41, appearanceDisabled41, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions26, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance42, appearanceHovered42, appearancePressed42, appearanceDisabled42, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_WbsPayment.ContainerControl = this;
		this.btn_WbsPayment.DisplayMember = "TaskCode";
		this.btn_WbsPayment.DisplayMember2 = "";
		this.btn_WbsPayment.ListMandatoryField = null;
		this.btn_WbsPayment.LoadDataSourceWhenEnter = false;
		this.btn_WbsPayment.Name = "btn_WbsPayment";
		this.btn_WbsPayment.SeparateCharacterForMulti = ';';
		this.btn_WbsPayment.SeparateString = ", ";
		this.btn_WbsPayment.UsingAutoCompleteBox = false;
		this.btn_WbsPayment.UsingDataSource = true;
		this.btn_WbsPayment.ValidateWhenNoDataSource = false;
		this.btn_WbsPayment.ValueMember = "TaskCode";
		this.tp_TaskName.Caption = "Wbs Name";
		this.tp_TaskName.CurrencyDateField = null;
		this.tp_TaskName.CurrencyField = null;
		this.tp_TaskName.CurrencyFixed = null;
		this.tp_TaskName.CurrencyFixedRate = 0.0;
		this.tp_TaskName.CurrencyRateField = null;
		this.tp_TaskName.FieldName = "TaskName";
		this.tp_TaskName.Name = "tp_TaskName";
		this.tp_InvoicePlan.Caption = "Invoice Plan";
		this.tp_InvoicePlan.CurrencyDateField = null;
		this.tp_InvoicePlan.CurrencyField = null;
		this.tp_InvoicePlan.CurrencyFixed = null;
		this.tp_InvoicePlan.CurrencyFixedRate = 0.0;
		this.tp_InvoicePlan.CurrencyRateField = null;
		this.tp_InvoicePlan.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.tp_InvoicePlan.FieldName = "InvoicePlan";
		this.tp_InvoicePlan.Name = "tp_InvoicePlan";
		this.tp_IncomingPayment.Caption = "Incoming Payment";
		this.tp_IncomingPayment.CurrencyDateField = null;
		this.tp_IncomingPayment.CurrencyField = null;
		this.tp_IncomingPayment.CurrencyFixed = null;
		this.tp_IncomingPayment.CurrencyFixedRate = 0.0;
		this.tp_IncomingPayment.CurrencyRateField = null;
		this.tp_IncomingPayment.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.tp_IncomingPayment.FieldName = "IncomingPayment";
		this.tp_IncomingPayment.Name = "tp_IncomingPayment";
		this.tp_Payments.Caption = "Payments";
		this.tp_Payments.CurrencyDateField = null;
		this.tp_Payments.CurrencyField = null;
		this.tp_Payments.CurrencyFixed = null;
		this.tp_Payments.CurrencyFixedRate = 0.0;
		this.tp_Payments.CurrencyRateField = null;
		this.tp_Payments.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.tp_Payments.FieldName = "Payments";
		this.tp_Payments.Name = "tp_Payments";
		this.tp_Duedate.Caption = "Due Date";
		this.tp_Duedate.CurrencyDateField = null;
		this.tp_Duedate.CurrencyField = null;
		this.tp_Duedate.CurrencyFixed = null;
		this.tp_Duedate.CurrencyFixedRate = 0.0;
		this.tp_Duedate.CurrencyRateField = null;
		this.tp_Duedate.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.tp_Duedate.FieldName = "Duedate";
		this.tp_Duedate.Name = "tp_Duedate";
		this.tp_Remarks.Caption = "Remarks";
		this.tp_Remarks.CurrencyDateField = null;
		this.tp_Remarks.CurrencyField = null;
		this.tp_Remarks.CurrencyFixed = null;
		this.tp_Remarks.CurrencyFixedRate = 0.0;
		this.tp_Remarks.CurrencyRateField = null;
		this.tp_Remarks.FieldName = "Remarks";
		this.tp_Remarks.Name = "tp_Remarks";
		this.tp_LineNum.Caption = "LineNum";
		this.tp_LineNum.CurrencyDateField = null;
		this.tp_LineNum.CurrencyField = null;
		this.tp_LineNum.CurrencyFixed = null;
		this.tp_LineNum.CurrencyFixedRate = 0.0;
		this.tp_LineNum.CurrencyRateField = null;
		this.tp_LineNum.FieldName = "LineNum";
		this.tp_LineNum.Name = "tp_LineNum";
		this.tp_LineNum.OptionsColumn.ReadOnly = true;
		this.tp_FatherNum.Caption = "FatherNum";
		this.tp_FatherNum.CurrencyDateField = null;
		this.tp_FatherNum.CurrencyField = null;
		this.tp_FatherNum.CurrencyFixed = null;
		this.tp_FatherNum.CurrencyFixedRate = 0.0;
		this.tp_FatherNum.CurrencyRateField = null;
		this.tp_FatherNum.FieldName = "FatherNum";
		this.tp_FatherNum.Name = "tp_FatherNum";
		this.tp_FatherNum.OptionsColumn.ReadOnly = true;
		this.tp_IsNew.Caption = "IsNew";
		this.tp_IsNew.CurrencyDateField = null;
		this.tp_IsNew.CurrencyField = null;
		this.tp_IsNew.CurrencyFixed = null;
		this.tp_IsNew.CurrencyFixedRate = 0.0;
		this.tp_IsNew.CurrencyRateField = null;
		this.tp_IsNew.FieldName = "IsNew";
		this.tp_IsNew.Name = "tp_IsNew";
		this.tp_IsNew.OptionsColumn.ReadOnly = true;
		this.tp_BaseLine.Caption = "BaseLine";
		this.tp_BaseLine.CurrencyDateField = null;
		this.tp_BaseLine.CurrencyField = null;
		this.tp_BaseLine.CurrencyFixed = null;
		this.tp_BaseLine.CurrencyFixedRate = 0.0;
		this.tp_BaseLine.CurrencyRateField = null;
		this.tp_BaseLine.FieldName = "BaseLine";
		this.tp_BaseLine.Name = "tp_BaseLine";
		this.tp_BaseLine.OptionsColumn.ReadOnly = true;
		this.tp_VisOrder.Caption = "VisOrder";
		this.tp_VisOrder.CurrencyDateField = null;
		this.tp_VisOrder.CurrencyField = null;
		this.tp_VisOrder.CurrencyFixed = null;
		this.tp_VisOrder.CurrencyFixedRate = 0.0;
		this.tp_VisOrder.CurrencyRateField = null;
		this.tp_VisOrder.FieldName = "VisOrder";
		this.tp_VisOrder.Name = "tp_VisOrder";
		this.tp_VisOrder.OptionsColumn.AllowEdit = false;
		this.tp_VisOrder.OptionsColumn.ReadOnly = true;
		this.ctm_Payments.ImageScalingSize = new System.Drawing.Size(32, 32);
		this.ctm_Payments.Items.AddRange(new System.Windows.Forms.ToolStripItem[6] { this.toolStripMenuItem2, this.toolStripMenuItem3, this.toolStripSeparator11, this.toolStripMenuItem4, this.toolStripSeparator12, this.toolStripMenuItem5 });
		this.ctm_Payments.Name = "ctm_BudgetItem";
		this.ctm_Payments.Size = new System.Drawing.Size(134, 104);
		this.toolStripMenuItem2.Name = "toolStripMenuItem2";
		this.toolStripMenuItem2.Size = new System.Drawing.Size(133, 22);
		this.toolStripMenuItem2.Text = "Add Row";
		this.toolStripMenuItem2.Click += new System.EventHandler(this.toolStripMenuItem2_Click);
		this.toolStripMenuItem3.Name = "toolStripMenuItem3";
		this.toolStripMenuItem3.Size = new System.Drawing.Size(133, 22);
		this.toolStripMenuItem3.Text = "Insert Row";
		this.toolStripMenuItem3.Click += new System.EventHandler(this.toolStripMenuItem3_Click);
		this.toolStripSeparator11.Name = "toolStripSeparator11";
		this.toolStripSeparator11.Size = new System.Drawing.Size(130, 6);
		this.toolStripMenuItem4.Name = "toolStripMenuItem4";
		this.toolStripMenuItem4.Size = new System.Drawing.Size(133, 22);
		this.toolStripMenuItem4.Text = "Duplicate";
		this.toolStripMenuItem4.Click += new System.EventHandler(this.toolStripMenuItem4_Click);
		this.toolStripSeparator12.Name = "toolStripSeparator12";
		this.toolStripSeparator12.Size = new System.Drawing.Size(130, 6);
		this.toolStripMenuItem5.Name = "toolStripMenuItem5";
		this.toolStripMenuItem5.Size = new System.Drawing.Size(133, 22);
		this.toolStripMenuItem5.Text = "Delete Row";
		this.toolStripMenuItem5.Click += new System.EventHandler(this.toolStripMenuItem5_Click);
		this.xtra_Document.Controls.Add(this.grd_LineDocument);
		this.xtra_Document.EditControl = null;
		this.xtra_Document.EditValue = "Documents";
		this.xtra_Document.FieldName = "xtra_Document";
		this.xtra_Document.IsSearchControl = false;
		this.xtra_Document.Margin = new System.Windows.Forms.Padding(2);
		this.xtra_Document.Name = "xtra_Document";
		this.xtra_Document.ReadOnly = false;
		this.xtra_Document.SettingInVisible = false;
		this.xtra_Document.SettingReadonly = false;
		this.xtra_Document.Size = new System.Drawing.Size(880, 208);
		this.xtra_Document.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtra_Document.Text = "Documents";
		this.grd_LineDocument.AllowPasteExcel = false;
		this.grd_LineDocument.ClipboardData = null;
		this.grd_LineDocument.ContextMenuStrip = this.ctm_Rows;
		this.grd_LineDocument.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_LineDocument.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(2);
		this.grd_LineDocument.Location = new System.Drawing.Point(0, 0);
		this.grd_LineDocument.MainView = this.gv_LineDocument;
		this.grd_LineDocument.Margin = new System.Windows.Forms.Padding(2);
		this.grd_LineDocument.Name = "grd_LineDocument";
		this.grd_LineDocument.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.btn_DocNumDo, this.luk_CurrencyLineDu });
		this.grd_LineDocument.Size = new System.Drawing.Size(880, 208);
		this.grd_LineDocument.TabIndex = 0;
		this.grd_LineDocument.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_LineDocument });
		this.gv_LineDocument.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_LineDocument.ColumnKey = null;
		this.gv_LineDocument.ColumnPanelRowHeight = 0;
		this.gv_LineDocument.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[8] { this.gridColumnEx47, this.gridColumnEx48, this.gridColumnEx49, this.gridColumnEx25, this.gridColumnEx26, this.gridColumnEx27, this.gridColumnEx29, this.gridColumnEx32 });
		this.gv_LineDocument.DetailHeight = 364;
		this.gv_LineDocument.DisableFormSetting = false;
		this.gv_LineDocument.DisplayException = false;
		this.gv_LineDocument.EditColumn = null;
		this.gv_LineDocument.FieldCurrency = null;
		this.gv_LineDocument.FieldRate = null;
		this.gv_LineDocument.FooterPanelHeight = 0;
		this.gv_LineDocument.FormId = "oProjectManagement";
		this.gv_LineDocument.GridControl = this.grd_LineDocument;
		this.gv_LineDocument.GroupRowHeight = 0;
		this.gv_LineDocument.HasFormSetting = true;
		this.gv_LineDocument.IgnoreFilter = false;
		this.gv_LineDocument.IndexEditRow = -1;
		this.gv_LineDocument.IndicatorWidth = 35;
		this.gv_LineDocument.IsApzTable = false;
		this.gv_LineDocument.IsInsertNewRow = false;
		this.gv_LineDocument.IsLoadingData = false;
		this.gv_LineDocument.IsMappingContextAction = false;
		this.gv_LineDocument.ItemId = "APZ_PRG5";
		this.gv_LineDocument.LevelIndent = 0;
		this.gv_LineDocument.LstColumnAllowBatch = null;
		this.gv_LineDocument.LstColumnNotAllowBatch = null;
		this.gv_LineDocument.LstColumnNotEditable = null;
		this.gv_LineDocument.LstColumNotSetting = null;
		this.gv_LineDocument.LstOldUDF = null;
		this.gv_LineDocument.Name = "gv_LineDocument";
		this.gv_LineDocument.OptionsView.BestFitMaxRowCount = 100;
		this.gv_LineDocument.OptionsView.ColumnAutoWidth = false;
		this.gv_LineDocument.OptionsView.ShowGroupPanel = false;
		this.gv_LineDocument.PreviewIndent = 0;
		this.gv_LineDocument.ReadOnly = false;
		this.gv_LineDocument.RowHeight = 0;
		this.gv_LineDocument.SettingReadonly = false;
		this.gv_LineDocument.UseBatchMod = false;
		this.gv_LineDocument.ViewCaptionHeight = 0;
		this.gv_LineDocument.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_LineDocument_RowCellStyle);
		this.gridColumnEx47.Caption = "AbsEntry";
		this.gridColumnEx47.CurrencyDateField = null;
		this.gridColumnEx47.CurrencyField = null;
		this.gridColumnEx47.CurrencyFixed = null;
		this.gridColumnEx47.CurrencyFixedRate = 0.0;
		this.gridColumnEx47.CurrencyRateField = null;
		this.gridColumnEx47.FieldName = "AbsEntry";
		this.gridColumnEx47.Name = "gridColumnEx47";
		this.gridColumnEx47.OptionsColumn.ReadOnly = true;
		this.gridColumnEx47.PrimaryKey = false;
		this.gridColumnEx47.UnitField = null;
		this.gridColumnEx48.Caption = "Doc Type";
		this.gridColumnEx48.CurrencyDateField = null;
		this.gridColumnEx48.CurrencyField = null;
		this.gridColumnEx48.CurrencyFixed = null;
		this.gridColumnEx48.CurrencyFixedRate = 0.0;
		this.gridColumnEx48.CurrencyRateField = null;
		this.gridColumnEx48.FieldName = "ObjTypeName";
		this.gridColumnEx48.Name = "gridColumnEx48";
		this.gridColumnEx48.OptionsColumn.ReadOnly = true;
		this.gridColumnEx48.PrimaryKey = false;
		this.gridColumnEx48.UnitField = null;
		this.gridColumnEx49.Caption = "Doc Entry";
		this.gridColumnEx49.ColumnEdit = this.btn_DocNumDo;
		this.gridColumnEx49.CurrencyDateField = null;
		this.gridColumnEx49.CurrencyField = null;
		this.gridColumnEx49.CurrencyFixed = null;
		this.gridColumnEx49.CurrencyFixedRate = 0.0;
		this.gridColumnEx49.CurrencyRateField = null;
		this.gridColumnEx49.FieldName = "DocEntry";
		this.gridColumnEx49.Name = "gridColumnEx49";
		this.gridColumnEx49.OptionsColumn.ReadOnly = true;
		this.gridColumnEx49.PrimaryKey = false;
		this.gridColumnEx49.UnitField = null;
		this.btn_DocNumDo.AutoHeight = false;
		this.btn_DocNumDo.AutoLoadDataSource = true;
		editorButtonImageOptions27.Image = Apzon.Forms.Properties.Resources.right;
		this.btn_DocNumDo.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions27, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance43, appearanceHovered43, appearancePressed43, appearanceDisabled43, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_DocNumDo.ContainerControl = this;
		this.btn_DocNumDo.DisplayMember = "";
		this.btn_DocNumDo.DisplayMember2 = "";
		this.btn_DocNumDo.ListMandatoryField = null;
		this.btn_DocNumDo.LoadDataSourceWhenEnter = false;
		this.btn_DocNumDo.Name = "btn_DocNumDo";
		this.btn_DocNumDo.ReadOnly = true;
		this.btn_DocNumDo.SeparateCharacterForMulti = ';';
		this.btn_DocNumDo.SeparateString = ", ";
		this.btn_DocNumDo.UsingAutoCompleteBox = false;
		this.btn_DocNumDo.UsingDataSource = true;
		this.btn_DocNumDo.ValidateWhenNoDataSource = false;
		this.btn_DocNumDo.ValueMember = "";
		this.gridColumnEx25.Caption = "Doc Num";
		this.gridColumnEx25.CurrencyDateField = null;
		this.gridColumnEx25.CurrencyField = null;
		this.gridColumnEx25.CurrencyFixed = null;
		this.gridColumnEx25.CurrencyFixedRate = 0.0;
		this.gridColumnEx25.CurrencyRateField = null;
		this.gridColumnEx25.FieldName = "DocNum";
		this.gridColumnEx25.Name = "gridColumnEx25";
		this.gridColumnEx25.OptionsColumn.ReadOnly = true;
		this.gridColumnEx25.PrimaryKey = false;
		this.gridColumnEx25.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridColumnEx25.UnitField = null;
		this.gridColumnEx26.Caption = "ObjType";
		this.gridColumnEx26.CurrencyDateField = null;
		this.gridColumnEx26.CurrencyField = null;
		this.gridColumnEx26.CurrencyFixed = null;
		this.gridColumnEx26.CurrencyFixedRate = 0.0;
		this.gridColumnEx26.CurrencyRateField = null;
		this.gridColumnEx26.FieldName = "ObjType";
		this.gridColumnEx26.Name = "gridColumnEx26";
		this.gridColumnEx26.OptionsColumn.ReadOnly = true;
		this.gridColumnEx26.PrimaryKey = false;
		this.gridColumnEx26.UnitField = null;
		this.gridColumnEx27.Caption = "Total";
		this.gridColumnEx27.ColumnEdit = this.repositoryItemCalcEditEx4;
		this.gridColumnEx27.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gridColumnEx27.CurrencyDateField = null;
		this.gridColumnEx27.CurrencyField = "Currency";
		this.gridColumnEx27.CurrencyFixed = null;
		this.gridColumnEx27.CurrencyFixedRate = 0.0;
		this.gridColumnEx27.CurrencyRateField = null;
		this.gridColumnEx27.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gridColumnEx27.FieldName = "Total";
		this.gridColumnEx27.Name = "gridColumnEx27";
		this.gridColumnEx27.OptionsColumn.ReadOnly = true;
		this.gridColumnEx27.PrimaryKey = false;
		this.gridColumnEx27.UnitField = null;
		this.gridColumnEx29.Caption = "Remark";
		this.gridColumnEx29.CurrencyDateField = null;
		this.gridColumnEx29.CurrencyField = null;
		this.gridColumnEx29.CurrencyFixed = null;
		this.gridColumnEx29.CurrencyFixedRate = 0.0;
		this.gridColumnEx29.CurrencyRateField = null;
		this.gridColumnEx29.FieldName = "Remark";
		this.gridColumnEx29.Name = "gridColumnEx29";
		this.gridColumnEx29.OptionsColumn.ReadOnly = true;
		this.gridColumnEx29.PrimaryKey = false;
		this.gridColumnEx29.UnitField = null;
		this.gridColumnEx32.Caption = "Currency";
		this.gridColumnEx32.ColumnEdit = this.luk_CurrencyLineDu;
		this.gridColumnEx32.CurrencyDateField = null;
		this.gridColumnEx32.CurrencyField = null;
		this.gridColumnEx32.CurrencyFixed = null;
		this.gridColumnEx32.CurrencyFixedRate = 0.0;
		this.gridColumnEx32.CurrencyRateField = null;
		this.gridColumnEx32.FieldName = "Currency";
		this.gridColumnEx32.Name = "gridColumnEx32";
		this.gridColumnEx32.OptionsColumn.ReadOnly = true;
		this.gridColumnEx32.PrimaryKey = false;
		this.gridColumnEx32.UnitField = null;
		this.luk_CurrencyLineDu.AutoHeight = false;
		this.luk_CurrencyLineDu.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_CurrencyLineDu.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("CurrCode", "Code")
		});
		this.luk_CurrencyLineDu.DisplayMember = "CurrCode";
		this.luk_CurrencyLineDu.Name = "luk_CurrencyLineDu";
		this.luk_CurrencyLineDu.NullText = "";
		this.luk_CurrencyLineDu.ValueMember = "CurrCode";
		this.xtra_Sub.Controls.Add(this.grd_LineSub);
		this.xtra_Sub.EditControl = null;
		this.xtra_Sub.EditValue = "Sub Projects";
		this.xtra_Sub.FieldName = "xtra_Sub";
		this.xtra_Sub.IsSearchControl = false;
		this.xtra_Sub.Margin = new System.Windows.Forms.Padding(2);
		this.xtra_Sub.Name = "xtra_Sub";
		this.xtra_Sub.ReadOnly = false;
		this.xtra_Sub.SettingInVisible = false;
		this.xtra_Sub.SettingReadonly = false;
		this.xtra_Sub.Size = new System.Drawing.Size(880, 208);
		this.xtra_Sub.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtra_Sub.Text = "Sub Projects";
		this.grd_LineSub.AllowPasteExcel = false;
		this.grd_LineSub.ClipboardData = null;
		this.grd_LineSub.ContextMenuStrip = this.ctm_Doc;
		this.grd_LineSub.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_LineSub.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(2);
		this.grd_LineSub.Location = new System.Drawing.Point(0, 0);
		this.grd_LineSub.MainView = this.gv_LineSub;
		this.grd_LineSub.Margin = new System.Windows.Forms.Padding(2);
		this.grd_LineSub.Name = "grd_LineSub";
		this.grd_LineSub.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[20]
		{
			this.btn_SubBpCode, this.btn_SubPM, this.btn_SubOwner, this.btn_SubSalesOrder, this.btn_SubMasterProject, this.luk_SubType, this.luk_SubStatus, this.repositoryItemLookUpEditCombo1, this.luk_SubPriority, this.btn_SubFinalProject,
			this.repositoryItemTextEdit1, this.repositoryItemTextEdit2, this.repositoryItemMemoExEdit4, this.btn_SubWhs, this.luk_SubCurrency, this.repositoryItemDateEditEx3, this.repositoryItemTimeSpanEdit3, this.repositoryItemDateEditEx4, this.repositoryItemTimeSpanEdit4, this.btn_Code
		});
		this.grd_LineSub.Size = new System.Drawing.Size(880, 208);
		this.grd_LineSub.TabIndex = 1;
		this.grd_LineSub.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_LineSub });
		this.gv_LineSub.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_LineSub.ColumnKey = null;
		this.gv_LineSub.ColumnPanelRowHeight = 0;
		this.gv_LineSub.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[33]
		{
			this.gridColumnEx39, this.gridColumnEx40, this.gridColumnEx41, this.gridColumnEx42, this.gridColumnEx43, this.gridColumnEx44, this.gridColumnEx45, this.gridColumnEx46, this.gridColumnEx57, this.gridColumnEx58,
			this.gridColumnEx59, this.gridColumnEx61, this.gridColumnEx64, this.gridColumnEx65, this.gridColumnEx66, this.gridColumnEx67, this.gridColumnEx68, this.gridColumnEx70, this.gridColumnEx73, this.gridColumnEx75,
			this.gridColumnEx76, this.gridColumnEx77, this.gridColumnEx83, this.gridColumnEx91, this.gridColumnEx92, this.gridColumnEx93, this.gridColumnEx94, this.gridColumnEx95, this.gridColumnEx96, this.gridColumnEx97,
			this.gridColumnEx98, this.gridColumnEx99, this.gridColumnEx100
		});
		this.gv_LineSub.DetailHeight = 364;
		this.gv_LineSub.DisableFormSetting = false;
		this.gv_LineSub.DisplayException = false;
		this.gv_LineSub.EditColumn = null;
		this.gv_LineSub.FieldCurrency = null;
		this.gv_LineSub.FieldRate = null;
		this.gv_LineSub.FooterPanelHeight = 0;
		gridFormatRule2.Column = this.gridColumnEx61;
		gridFormatRule2.ColumnApplyTo = this.gridColumnEx61;
		gridFormatRule2.Name = "Format0";
		formatConditionIconSet3.CategoryName = "Shapes";
		formatConditionIconSetIcon9.PredefinedName = "RedToBlack4_1.png";
		formatConditionIconSetIcon9.Value = new decimal(new int[4] { 4, 0, 0, 0 });
		formatConditionIconSetIcon9.ValueComparison = DevExpress.XtraEditors.FormatConditionComparisonType.GreaterOrEqual;
		formatConditionIconSetIcon10.PredefinedName = "TrafficLights4_2.png";
		formatConditionIconSetIcon10.Value = new decimal(new int[4] { 3, 0, 0, 0 });
		formatConditionIconSetIcon10.ValueComparison = DevExpress.XtraEditors.FormatConditionComparisonType.GreaterOrEqual;
		formatConditionIconSetIcon11.PredefinedName = "TrafficLights4_4.png";
		formatConditionIconSetIcon11.Value = new decimal(new int[4] { 2, 0, 0, 0 });
		formatConditionIconSetIcon11.ValueComparison = DevExpress.XtraEditors.FormatConditionComparisonType.GreaterOrEqual;
		formatConditionIconSetIcon12.PredefinedName = "TrafficLights4_1.png";
		formatConditionIconSetIcon12.Value = new decimal(new int[4] { 1, 0, 0, 0 });
		formatConditionIconSetIcon12.ValueComparison = DevExpress.XtraEditors.FormatConditionComparisonType.GreaterOrEqual;
		formatConditionIconSet3.Icons.Add(formatConditionIconSetIcon9);
		formatConditionIconSet3.Icons.Add(formatConditionIconSetIcon10);
		formatConditionIconSet3.Icons.Add(formatConditionIconSetIcon11);
		formatConditionIconSet3.Icons.Add(formatConditionIconSetIcon12);
		formatConditionIconSet3.Name = "RedToBlack";
		formatConditionRuleIconSet3.IconSet = formatConditionIconSet3;
		gridFormatRule2.Rule = formatConditionRuleIconSet3;
		this.gv_LineSub.FormatRules.Add(gridFormatRule2);
		this.gv_LineSub.FormId = "oProjectManagement";
		this.gv_LineSub.GridControl = this.grd_LineSub;
		this.gv_LineSub.GroupRowHeight = 0;
		this.gv_LineSub.HasFormSetting = true;
		this.gv_LineSub.IgnoreFilter = false;
		this.gv_LineSub.IndexEditRow = -1;
		this.gv_LineSub.IndicatorWidth = 35;
		this.gv_LineSub.IsApzTable = false;
		this.gv_LineSub.IsInsertNewRow = false;
		this.gv_LineSub.IsLoadingData = false;
		this.gv_LineSub.IsMappingContextAction = false;
		this.gv_LineSub.ItemId = "APZ_PRG";
		this.gv_LineSub.LevelIndent = 0;
		this.gv_LineSub.LstColumnAllowBatch = null;
		this.gv_LineSub.LstColumnNotAllowBatch = null;
		this.gv_LineSub.LstColumnNotEditable = null;
		this.gv_LineSub.LstColumNotSetting = null;
		this.gv_LineSub.LstOldUDF = null;
		this.gv_LineSub.Name = "gv_LineSub";
		this.gv_LineSub.OptionsView.BestFitMaxRowCount = 100;
		this.gv_LineSub.OptionsView.ColumnAutoWidth = false;
		this.gv_LineSub.OptionsView.ShowGroupPanel = false;
		this.gv_LineSub.PreviewIndent = 0;
		this.gv_LineSub.ReadOnly = false;
		this.gv_LineSub.RowHeight = 0;
		this.gv_LineSub.SettingReadonly = false;
		this.gv_LineSub.UseBatchMod = false;
		this.gv_LineSub.ViewCaptionHeight = 0;
		this.gv_LineSub.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gv_LineSub_FocusedRowChanged);
		this.gridColumnEx39.Caption = "AbsEntry";
		this.gridColumnEx39.ColumnEdit = this.btn_Code;
		this.gridColumnEx39.CurrencyDateField = null;
		this.gridColumnEx39.CurrencyField = null;
		this.gridColumnEx39.CurrencyFixed = null;
		this.gridColumnEx39.CurrencyFixedRate = 0.0;
		this.gridColumnEx39.CurrencyRateField = null;
		this.gridColumnEx39.FieldName = "AbsEntry";
		this.gridColumnEx39.Name = "gridColumnEx39";
		this.gridColumnEx39.OptionsColumn.ReadOnly = true;
		this.gridColumnEx39.PrimaryKey = false;
		this.gridColumnEx39.UnitField = null;
		this.gridColumnEx39.Width = 115;
		this.btn_Code.AutoHeight = false;
		this.btn_Code.AutoLoadDataSource = true;
		editorButtonImageOptions28.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions49.Image");
		this.btn_Code.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions28, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance44, appearanceHovered44, appearancePressed44, appearanceDisabled44, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_Code.ContainerControl = this;
		this.btn_Code.DisplayMember = "";
		this.btn_Code.DisplayMember2 = "";
		this.btn_Code.ListMandatoryField = null;
		this.btn_Code.LoadDataSourceWhenEnter = false;
		this.btn_Code.Name = "btn_Code";
		this.btn_Code.ReadOnly = true;
		this.btn_Code.SeparateCharacterForMulti = ';';
		this.btn_Code.SeparateString = ", ";
		this.btn_Code.UsingAutoCompleteBox = false;
		this.btn_Code.UsingDataSource = true;
		this.btn_Code.ValidateWhenNoDataSource = false;
		this.btn_Code.ValueMember = "";
		this.gridColumnEx40.Caption = "Sub Project Code";
		this.gridColumnEx40.ColumnEdit = this.repositoryItemTextEdit1;
		this.gridColumnEx40.CurrencyDateField = null;
		this.gridColumnEx40.CurrencyField = null;
		this.gridColumnEx40.CurrencyFixed = null;
		this.gridColumnEx40.CurrencyFixedRate = 0.0;
		this.gridColumnEx40.CurrencyRateField = null;
		this.gridColumnEx40.FieldName = "Code";
		this.gridColumnEx40.Name = "gridColumnEx40";
		this.gridColumnEx40.PrimaryKey = false;
		this.gridColumnEx40.UnitField = null;
		this.gridColumnEx40.Width = 148;
		this.repositoryItemTextEdit1.AutoHeight = false;
		this.repositoryItemTextEdit1.Name = "repositoryItemTextEdit1";
		this.gridColumnEx41.Caption = "Sub Project Name";
		this.gridColumnEx41.CurrencyDateField = null;
		this.gridColumnEx41.CurrencyField = null;
		this.gridColumnEx41.CurrencyFixed = null;
		this.gridColumnEx41.CurrencyFixedRate = 0.0;
		this.gridColumnEx41.CurrencyRateField = null;
		this.gridColumnEx41.FieldName = "Name";
		this.gridColumnEx41.Name = "gridColumnEx41";
		this.gridColumnEx41.PrimaryKey = false;
		this.gridColumnEx41.UnitField = null;
		this.gridColumnEx41.Width = 110;
		this.gridColumnEx42.Caption = "Type";
		this.gridColumnEx42.ColumnEdit = this.luk_SubType;
		this.gridColumnEx42.CurrencyDateField = null;
		this.gridColumnEx42.CurrencyField = null;
		this.gridColumnEx42.CurrencyFixed = null;
		this.gridColumnEx42.CurrencyFixedRate = 0.0;
		this.gridColumnEx42.CurrencyRateField = null;
		this.gridColumnEx42.FieldName = "ProjectType";
		this.gridColumnEx42.Name = "gridColumnEx42";
		this.gridColumnEx42.PrimaryKey = false;
		this.gridColumnEx42.UnitField = null;
		this.luk_SubType.AutoHeight = false;
		this.luk_SubType.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_SubType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_SubType.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.luk_SubType.IsSearchControl = false;
		this.luk_SubType.Name = "luk_SubType";
		this.luk_SubType.NullText = "";
		this.luk_SubType.ShowFooter = false;
		this.luk_SubType.ShowHeader = false;
		this.gridColumnEx43.Caption = "Status";
		this.gridColumnEx43.ColumnEdit = this.luk_SubStatus;
		this.gridColumnEx43.CurrencyDateField = null;
		this.gridColumnEx43.CurrencyField = null;
		this.gridColumnEx43.CurrencyFixed = null;
		this.gridColumnEx43.CurrencyFixedRate = 0.0;
		this.gridColumnEx43.CurrencyRateField = null;
		this.gridColumnEx43.FieldName = "PrgStatus";
		this.gridColumnEx43.Name = "gridColumnEx43";
		this.gridColumnEx43.PrimaryKey = false;
		this.gridColumnEx43.UnitField = null;
		this.luk_SubStatus.AutoHeight = false;
		this.luk_SubStatus.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_SubStatus.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_SubStatus.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.luk_SubStatus.IsSearchControl = false;
		this.luk_SubStatus.Name = "luk_SubStatus";
		this.luk_SubStatus.NullText = "";
		this.luk_SubStatus.ShowFooter = false;
		this.luk_SubStatus.ShowHeader = false;
		this.gridColumnEx44.Caption = "BP Code";
		this.gridColumnEx44.ColumnEdit = this.btn_SubBpCode;
		this.gridColumnEx44.CurrencyDateField = null;
		this.gridColumnEx44.CurrencyField = null;
		this.gridColumnEx44.CurrencyFixed = null;
		this.gridColumnEx44.CurrencyFixedRate = 0.0;
		this.gridColumnEx44.CurrencyRateField = null;
		this.gridColumnEx44.FieldName = "CardCode";
		this.gridColumnEx44.Name = "gridColumnEx44";
		this.gridColumnEx44.PrimaryKey = false;
		this.gridColumnEx44.UnitField = null;
		this.btn_SubBpCode.AutoHeight = false;
		this.btn_SubBpCode.AutoLoadDataSource = true;
		editorButtonImageOptions29.Image = Apzon.Forms.Properties.Resources.right;
		this.btn_SubBpCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions29, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance45, appearanceHovered45, appearancePressed45, appearanceDisabled45, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_SubBpCode.ContainerControl = this;
		this.btn_SubBpCode.DisplayMember = "CardCode";
		this.btn_SubBpCode.DisplayMember2 = "";
		this.btn_SubBpCode.ListMandatoryField = null;
		this.btn_SubBpCode.LoadDataSourceWhenEnter = false;
		this.btn_SubBpCode.Name = "btn_SubBpCode";
		this.btn_SubBpCode.SeparateCharacterForMulti = ';';
		this.btn_SubBpCode.SeparateString = ", ";
		this.btn_SubBpCode.UsingAutoCompleteBox = false;
		this.btn_SubBpCode.UsingDataSource = true;
		this.btn_SubBpCode.ValidateWhenNoDataSource = false;
		this.btn_SubBpCode.ValueMember = "CardCode";
		this.gridColumnEx45.Caption = "BP Name";
		this.gridColumnEx45.CurrencyDateField = null;
		this.gridColumnEx45.CurrencyField = null;
		this.gridColumnEx45.CurrencyFixed = null;
		this.gridColumnEx45.CurrencyFixedRate = 0.0;
		this.gridColumnEx45.CurrencyRateField = null;
		this.gridColumnEx45.FieldName = "CardName";
		this.gridColumnEx45.Name = "gridColumnEx45";
		this.gridColumnEx45.PrimaryKey = false;
		this.gridColumnEx45.UnitField = null;
		this.gridColumnEx46.Caption = "Project Manager";
		this.gridColumnEx46.ColumnEdit = this.btn_SubPM;
		this.gridColumnEx46.CurrencyDateField = null;
		this.gridColumnEx46.CurrencyField = null;
		this.gridColumnEx46.CurrencyFixed = null;
		this.gridColumnEx46.CurrencyFixedRate = 0.0;
		this.gridColumnEx46.CurrencyRateField = null;
		this.gridColumnEx46.FieldName = "PM";
		this.gridColumnEx46.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx46.Name = "gridColumnEx46";
		this.gridColumnEx46.PrimaryKey = false;
		this.gridColumnEx46.UnitField = null;
		this.gridColumnEx46.Width = 120;
		this.btn_SubPM.AutoHeight = false;
		this.btn_SubPM.AutoLoadDataSource = true;
		this.btn_SubPM.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, imageOptions17, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance46, appearanceHovered46, appearancePressed46, appearanceDisabled46, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_SubPM.ContainerControl = this;
		this.btn_SubPM.DisplayMember = "lastName";
		this.btn_SubPM.DisplayMember2 = "firstName";
		this.btn_SubPM.ListMandatoryField = null;
		this.btn_SubPM.LoadDataSourceWhenEnter = false;
		this.btn_SubPM.Name = "btn_SubPM";
		this.btn_SubPM.SeparateCharacterForMulti = ';';
		this.btn_SubPM.SeparateString = " ";
		this.btn_SubPM.UsingAutoCompleteBox = false;
		this.btn_SubPM.UsingDataSource = true;
		this.btn_SubPM.ValidateWhenNoDataSource = false;
		this.btn_SubPM.ValueMember = "EmpId";
		this.gridColumnEx57.Caption = "Start Date";
		this.gridColumnEx57.CurrencyDateField = null;
		this.gridColumnEx57.CurrencyField = null;
		this.gridColumnEx57.CurrencyFixed = null;
		this.gridColumnEx57.CurrencyFixedRate = 0.0;
		this.gridColumnEx57.CurrencyRateField = null;
		this.gridColumnEx57.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gridColumnEx57.FieldName = "StartDate";
		this.gridColumnEx57.Name = "gridColumnEx57";
		this.gridColumnEx57.PrimaryKey = false;
		this.gridColumnEx57.UnitField = null;
		this.gridColumnEx57.Width = 100;
		this.gridColumnEx58.Caption = "Due Date";
		this.gridColumnEx58.CurrencyDateField = null;
		this.gridColumnEx58.CurrencyField = null;
		this.gridColumnEx58.CurrencyFixed = null;
		this.gridColumnEx58.CurrencyFixedRate = 0.0;
		this.gridColumnEx58.CurrencyRateField = null;
		this.gridColumnEx58.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gridColumnEx58.FieldName = "DueDate";
		this.gridColumnEx58.Name = "gridColumnEx58";
		this.gridColumnEx58.PrimaryKey = false;
		this.gridColumnEx58.UnitField = null;
		this.gridColumnEx59.Caption = "Actual Closed Date";
		this.gridColumnEx59.CurrencyDateField = null;
		this.gridColumnEx59.CurrencyField = null;
		this.gridColumnEx59.CurrencyFixed = null;
		this.gridColumnEx59.CurrencyFixedRate = 0.0;
		this.gridColumnEx59.CurrencyRateField = null;
		this.gridColumnEx59.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gridColumnEx59.FieldName = "ActualDate";
		this.gridColumnEx59.Name = "gridColumnEx59";
		this.gridColumnEx59.PrimaryKey = false;
		this.gridColumnEx59.UnitField = null;
		this.gridColumnEx59.Width = 150;
		this.gridColumnEx64.Caption = "Master Project";
		this.gridColumnEx64.ColumnEdit = this.btn_SubMasterProject;
		this.gridColumnEx64.CurrencyDateField = null;
		this.gridColumnEx64.CurrencyField = null;
		this.gridColumnEx64.CurrencyFixed = null;
		this.gridColumnEx64.CurrencyFixedRate = 0.0;
		this.gridColumnEx64.CurrencyRateField = null;
		this.gridColumnEx64.FieldName = "MasterEntry";
		this.gridColumnEx64.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx64.Name = "gridColumnEx64";
		this.gridColumnEx64.PrimaryKey = false;
		this.gridColumnEx64.UnitField = null;
		this.gridColumnEx64.Width = 100;
		this.btn_SubMasterProject.AutoHeight = false;
		this.btn_SubMasterProject.AutoLoadDataSource = true;
		editorButtonImageOptions30.Image = Apzon.Forms.Properties.Resources.right;
		this.btn_SubMasterProject.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions30, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance47, appearanceHovered47, appearancePressed47, appearanceDisabled47, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_SubMasterProject.ContainerControl = this;
		this.btn_SubMasterProject.DisplayMember = "Name";
		this.btn_SubMasterProject.DisplayMember2 = "";
		this.btn_SubMasterProject.ListMandatoryField = null;
		this.btn_SubMasterProject.LoadDataSourceWhenEnter = false;
		this.btn_SubMasterProject.Name = "btn_SubMasterProject";
		this.btn_SubMasterProject.SeparateCharacterForMulti = ';';
		this.btn_SubMasterProject.SeparateString = ", ";
		this.btn_SubMasterProject.UsingAutoCompleteBox = false;
		this.btn_SubMasterProject.UsingDataSource = true;
		this.btn_SubMasterProject.ValidateWhenNoDataSource = false;
		this.btn_SubMasterProject.ValueMember = "AbsEntry";
		this.gridColumnEx65.Caption = "Risk Level";
		this.gridColumnEx65.CurrencyDateField = null;
		this.gridColumnEx65.CurrencyField = null;
		this.gridColumnEx65.CurrencyFixed = null;
		this.gridColumnEx65.CurrencyFixedRate = 0.0;
		this.gridColumnEx65.CurrencyRateField = null;
		this.gridColumnEx65.FieldName = "RiskLvl";
		this.gridColumnEx65.Name = "gridColumnEx65";
		this.gridColumnEx65.PrimaryKey = false;
		this.gridColumnEx65.UnitField = null;
		this.gridColumnEx65.Width = 100;
		this.gridColumnEx66.Caption = "Industry";
		this.gridColumnEx66.ColumnEdit = this.repositoryItemLookUpEditCombo1;
		this.gridColumnEx66.CurrencyDateField = null;
		this.gridColumnEx66.CurrencyField = null;
		this.gridColumnEx66.CurrencyFixed = null;
		this.gridColumnEx66.CurrencyFixedRate = 0.0;
		this.gridColumnEx66.CurrencyRateField = null;
		this.gridColumnEx66.FieldName = "IndCode";
		this.gridColumnEx66.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx66.Name = "gridColumnEx66";
		this.gridColumnEx66.PrimaryKey = false;
		this.gridColumnEx66.UnitField = null;
		this.repositoryItemLookUpEditCombo1.AutoHeight = false;
		this.repositoryItemLookUpEditCombo1.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.repositoryItemLookUpEditCombo1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, imageOptions18, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance48, appearanceHovered48, appearancePressed48, appearanceDisabled48, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.repositoryItemLookUpEditCombo1.IsSearchControl = false;
		this.repositoryItemLookUpEditCombo1.Name = "repositoryItemLookUpEditCombo1";
		this.repositoryItemLookUpEditCombo1.NullText = "";
		this.repositoryItemLookUpEditCombo1.ShowFooter = false;
		this.repositoryItemLookUpEditCombo1.ShowHeader = false;
		this.gridColumnEx67.Caption = "Project Owner";
		this.gridColumnEx67.ColumnEdit = this.btn_SubOwner;
		this.gridColumnEx67.CurrencyDateField = null;
		this.gridColumnEx67.CurrencyField = null;
		this.gridColumnEx67.CurrencyFixed = null;
		this.gridColumnEx67.CurrencyFixedRate = 0.0;
		this.gridColumnEx67.CurrencyRateField = null;
		this.gridColumnEx67.FieldName = "Owner";
		this.gridColumnEx67.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx67.Name = "gridColumnEx67";
		this.gridColumnEx67.PrimaryKey = false;
		this.gridColumnEx67.UnitField = null;
		this.gridColumnEx67.Width = 100;
		this.btn_SubOwner.AutoHeight = false;
		this.btn_SubOwner.AutoLoadDataSource = true;
		editorButtonImageOptions31.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions54.Image");
		this.btn_SubOwner.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions31, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance49, appearanceHovered49, appearancePressed49, appearanceDisabled49, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_SubOwner.ContainerControl = this;
		this.btn_SubOwner.DisplayMember = "lastName";
		this.btn_SubOwner.DisplayMember2 = "firstName";
		this.btn_SubOwner.ListMandatoryField = null;
		this.btn_SubOwner.LoadDataSourceWhenEnter = false;
		this.btn_SubOwner.Name = "btn_SubOwner";
		this.btn_SubOwner.SeparateCharacterForMulti = ';';
		this.btn_SubOwner.SeparateString = " ";
		this.btn_SubOwner.UsingAutoCompleteBox = false;
		this.btn_SubOwner.UsingDataSource = true;
		this.btn_SubOwner.ValidateWhenNoDataSource = false;
		this.btn_SubOwner.ValueMember = "EmpId";
		this.gridColumnEx68.Caption = "Sales Order";
		this.gridColumnEx68.ColumnEdit = this.btn_SubSalesOrder;
		this.gridColumnEx68.CurrencyDateField = null;
		this.gridColumnEx68.CurrencyField = null;
		this.gridColumnEx68.CurrencyFixed = null;
		this.gridColumnEx68.CurrencyFixedRate = 0.0;
		this.gridColumnEx68.CurrencyRateField = null;
		this.gridColumnEx68.FieldName = "Order";
		this.gridColumnEx68.Name = "gridColumnEx68";
		this.gridColumnEx68.PrimaryKey = false;
		this.gridColumnEx68.UnitField = null;
		this.gridColumnEx68.Width = 100;
		this.btn_SubSalesOrder.AutoHeight = false;
		this.btn_SubSalesOrder.AutoLoadDataSource = true;
		this.btn_SubSalesOrder.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, imageOptions19, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance50, appearanceHovered50, appearancePressed50, appearanceDisabled50, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_SubSalesOrder.ContainerControl = this;
		this.btn_SubSalesOrder.DisplayMember = "DocNum";
		this.btn_SubSalesOrder.DisplayMember2 = "";
		this.btn_SubSalesOrder.ListMandatoryField = null;
		this.btn_SubSalesOrder.LoadDataSourceWhenEnter = false;
		this.btn_SubSalesOrder.Name = "btn_SubSalesOrder";
		this.btn_SubSalesOrder.SeparateCharacterForMulti = ';';
		this.btn_SubSalesOrder.SeparateString = ", ";
		this.btn_SubSalesOrder.UsingAutoCompleteBox = false;
		this.btn_SubSalesOrder.UsingDataSource = true;
		this.btn_SubSalesOrder.ValidateWhenNoDataSource = false;
		this.btn_SubSalesOrder.ValueMember = "DocEntry";
		this.btn_SubSalesOrder.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btn_SubSalesOrder_ButtonClick);
		this.gridColumnEx70.Caption = "Remark";
		this.gridColumnEx70.ColumnEdit = this.repositoryItemMemoExEdit4;
		this.gridColumnEx70.CurrencyDateField = null;
		this.gridColumnEx70.CurrencyField = null;
		this.gridColumnEx70.CurrencyFixed = null;
		this.gridColumnEx70.CurrencyFixedRate = 0.0;
		this.gridColumnEx70.CurrencyRateField = null;
		this.gridColumnEx70.FieldName = "Remark";
		this.gridColumnEx70.Name = "gridColumnEx70";
		this.gridColumnEx70.PrimaryKey = false;
		this.gridColumnEx70.UnitField = null;
		this.repositoryItemMemoExEdit4.AutoHeight = false;
		this.repositoryItemMemoExEdit4.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemMemoExEdit4.Name = "repositoryItemMemoExEdit4";
		this.repositoryItemMemoExEdit4.ShowIcon = false;
		this.gridColumnEx73.Caption = "Budget Amount";
		this.gridColumnEx73.ColumnEdit = this.repositoryItemCalcEditEx1;
		this.gridColumnEx73.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gridColumnEx73.CurrencyDateField = null;
		this.gridColumnEx73.CurrencyField = "DocCur";
		this.gridColumnEx73.CurrencyFixed = null;
		this.gridColumnEx73.CurrencyFixedRate = 0.0;
		this.gridColumnEx73.CurrencyRateField = null;
		this.gridColumnEx73.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gridColumnEx73.FieldName = "BudgetAmount";
		this.gridColumnEx73.Name = "gridColumnEx73";
		this.gridColumnEx73.OptionsColumn.ReadOnly = true;
		this.gridColumnEx73.PrimaryKey = false;
		this.gridColumnEx73.UnitField = null;
		this.gridColumnEx73.Width = 100;
		this.gridColumnEx75.Caption = "Actual Amount";
		this.gridColumnEx75.ColumnEdit = this.repositoryItemCalcEditEx2;
		this.gridColumnEx75.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gridColumnEx75.CurrencyDateField = null;
		this.gridColumnEx75.CurrencyField = "DocCur";
		this.gridColumnEx75.CurrencyFixed = null;
		this.gridColumnEx75.CurrencyFixedRate = 0.0;
		this.gridColumnEx75.CurrencyRateField = null;
		this.gridColumnEx75.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gridColumnEx75.FieldName = "ActualAmount";
		this.gridColumnEx75.Name = "gridColumnEx75";
		this.gridColumnEx75.OptionsColumn.ReadOnly = true;
		this.gridColumnEx75.PrimaryKey = false;
		this.gridColumnEx75.UnitField = null;
		this.gridColumnEx75.Width = 100;
		this.gridColumnEx76.Caption = "Amount Diff ";
		this.gridColumnEx76.ColumnEdit = this.repositoryItemCalcEditEx3;
		this.gridColumnEx76.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gridColumnEx76.CurrencyDateField = null;
		this.gridColumnEx76.CurrencyField = "DocCur";
		this.gridColumnEx76.CurrencyFixed = null;
		this.gridColumnEx76.CurrencyFixedRate = 0.0;
		this.gridColumnEx76.CurrencyRateField = null;
		this.gridColumnEx76.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gridColumnEx76.FieldName = "AmountDiff";
		this.gridColumnEx76.Name = "gridColumnEx76";
		this.gridColumnEx76.OptionsColumn.ReadOnly = true;
		this.gridColumnEx76.PrimaryKey = false;
		this.gridColumnEx76.UnitField = null;
		this.gridColumnEx76.Width = 100;
		this.gridColumnEx77.Caption = "Financial Project";
		this.gridColumnEx77.ColumnEdit = this.btn_SubFinalProject;
		this.gridColumnEx77.CurrencyDateField = null;
		this.gridColumnEx77.CurrencyField = null;
		this.gridColumnEx77.CurrencyFixed = null;
		this.gridColumnEx77.CurrencyFixedRate = 0.0;
		this.gridColumnEx77.CurrencyRateField = null;
		this.gridColumnEx77.FieldName = "FinProject";
		this.gridColumnEx77.Name = "gridColumnEx77";
		this.gridColumnEx77.PrimaryKey = false;
		this.gridColumnEx77.UnitField = null;
		this.gridColumnEx77.Width = 100;
		this.btn_SubFinalProject.AutoHeight = false;
		this.btn_SubFinalProject.AutoLoadDataSource = true;
		editorButtonImageOptions32.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions57.Image");
		this.btn_SubFinalProject.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions20, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance51, appearanceHovered51, appearancePressed51, appearanceDisabled51, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions32, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance52, appearanceHovered52, appearancePressed52, appearanceDisabled52, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_SubFinalProject.ContainerControl = this;
		this.btn_SubFinalProject.DisplayMember = "";
		this.btn_SubFinalProject.DisplayMember2 = "";
		this.btn_SubFinalProject.ListMandatoryField = null;
		this.btn_SubFinalProject.LoadDataSourceWhenEnter = false;
		this.btn_SubFinalProject.Name = "btn_SubFinalProject";
		this.btn_SubFinalProject.SeparateCharacterForMulti = ';';
		this.btn_SubFinalProject.SeparateString = ", ";
		this.btn_SubFinalProject.UsingAutoCompleteBox = false;
		this.btn_SubFinalProject.UsingDataSource = true;
		this.btn_SubFinalProject.ValidateWhenNoDataSource = false;
		this.btn_SubFinalProject.ValueMember = "PrjCode";
		this.gridColumnEx83.Caption = "Budget Level";
		this.gridColumnEx83.ColumnEdit = this.repositoryItemTextEdit2;
		this.gridColumnEx83.CurrencyDateField = null;
		this.gridColumnEx83.CurrencyField = null;
		this.gridColumnEx83.CurrencyFixed = null;
		this.gridColumnEx83.CurrencyFixedRate = 0.0;
		this.gridColumnEx83.CurrencyRateField = null;
		this.gridColumnEx83.FieldName = "BudgetLevel";
		this.gridColumnEx83.Name = "gridColumnEx83";
		this.gridColumnEx83.PrimaryKey = false;
		this.gridColumnEx83.UnitField = null;
		this.repositoryItemTextEdit2.AutoHeight = false;
		this.repositoryItemTextEdit2.Name = "repositoryItemTextEdit2";
		this.gridColumnEx91.Caption = "Default Whs";
		this.gridColumnEx91.ColumnEdit = this.btn_SubWhs;
		this.gridColumnEx91.CurrencyDateField = null;
		this.gridColumnEx91.CurrencyField = null;
		this.gridColumnEx91.CurrencyFixed = null;
		this.gridColumnEx91.CurrencyFixedRate = 0.0;
		this.gridColumnEx91.CurrencyRateField = null;
		this.gridColumnEx91.FieldName = "DefaultWhs";
		this.gridColumnEx91.Name = "gridColumnEx91";
		this.gridColumnEx91.PrimaryKey = false;
		this.gridColumnEx91.UnitField = null;
		this.btn_SubWhs.AutoHeight = false;
		this.btn_SubWhs.AutoLoadDataSource = true;
		editorButtonImageOptions33.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions59.Image");
		this.btn_SubWhs.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions21, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance53, appearanceHovered53, appearancePressed53, appearanceDisabled53, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions33, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance54, appearanceHovered54, appearancePressed54, appearanceDisabled54, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_SubWhs.ContainerControl = this;
		this.btn_SubWhs.DisplayMember = "WhsCode";
		this.btn_SubWhs.DisplayMember2 = "";
		this.btn_SubWhs.ListMandatoryField = null;
		this.btn_SubWhs.LoadDataSourceWhenEnter = false;
		this.btn_SubWhs.Name = "btn_SubWhs";
		this.btn_SubWhs.SeparateCharacterForMulti = ';';
		this.btn_SubWhs.SeparateString = ", ";
		this.btn_SubWhs.UsingAutoCompleteBox = false;
		this.btn_SubWhs.UsingDataSource = true;
		this.btn_SubWhs.ValidateWhenNoDataSource = false;
		this.btn_SubWhs.ValueMember = "WhsCode";
		this.gridColumnEx92.Caption = "Doc Currency";
		this.gridColumnEx92.ColumnEdit = this.luk_SubCurrency;
		this.gridColumnEx92.CurrencyDateField = null;
		this.gridColumnEx92.CurrencyField = null;
		this.gridColumnEx92.CurrencyFixed = null;
		this.gridColumnEx92.CurrencyFixedRate = 0.0;
		this.gridColumnEx92.CurrencyRateField = null;
		this.gridColumnEx92.FieldName = "DocCur";
		this.gridColumnEx92.Name = "gridColumnEx92";
		this.gridColumnEx92.PrimaryKey = false;
		this.gridColumnEx92.UnitField = null;
		this.luk_SubCurrency.AutoHeight = false;
		this.luk_SubCurrency.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_SubCurrency.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("CurrCode", "Code")
		});
		this.luk_SubCurrency.DisplayMember = "CurrCode";
		this.luk_SubCurrency.Name = "luk_SubCurrency";
		this.luk_SubCurrency.NullText = "";
		this.luk_SubCurrency.ValueMember = "CurrCode";
		this.gridColumnEx93.Caption = "UserCodeSign";
		this.gridColumnEx93.CurrencyDateField = null;
		this.gridColumnEx93.CurrencyField = null;
		this.gridColumnEx93.CurrencyFixed = null;
		this.gridColumnEx93.CurrencyFixedRate = 0.0;
		this.gridColumnEx93.CurrencyRateField = null;
		this.gridColumnEx93.FieldName = "UserCodeSign";
		this.gridColumnEx93.Name = "gridColumnEx93";
		this.gridColumnEx93.OptionsColumn.ReadOnly = true;
		this.gridColumnEx93.PrimaryKey = false;
		this.gridColumnEx93.UnitField = null;
		this.gridColumnEx94.Caption = "UserCodeSign2";
		this.gridColumnEx94.CurrencyDateField = null;
		this.gridColumnEx94.CurrencyField = null;
		this.gridColumnEx94.CurrencyFixed = null;
		this.gridColumnEx94.CurrencyFixedRate = 0.0;
		this.gridColumnEx94.CurrencyRateField = null;
		this.gridColumnEx94.FieldName = "UserCodeSign2";
		this.gridColumnEx94.Name = "gridColumnEx94";
		this.gridColumnEx94.OptionsColumn.ReadOnly = true;
		this.gridColumnEx94.PrimaryKey = false;
		this.gridColumnEx94.UnitField = null;
		this.gridColumnEx95.Caption = "CreateDate";
		this.gridColumnEx95.ColumnEdit = this.repositoryItemDateEditEx3;
		this.gridColumnEx95.CurrencyDateField = null;
		this.gridColumnEx95.CurrencyField = null;
		this.gridColumnEx95.CurrencyFixed = null;
		this.gridColumnEx95.CurrencyFixedRate = 0.0;
		this.gridColumnEx95.CurrencyRateField = null;
		this.gridColumnEx95.FieldName = "CreateDate";
		this.gridColumnEx95.Name = "gridColumnEx95";
		this.gridColumnEx95.OptionsColumn.ReadOnly = true;
		this.gridColumnEx95.PrimaryKey = false;
		this.gridColumnEx95.UnitField = null;
		this.repositoryItemDateEditEx3.AutoHeight = false;
		this.repositoryItemDateEditEx3.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEditEx3.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEditEx3.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.repositoryItemDateEditEx3.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.repositoryItemDateEditEx3.EditFormat.FormatString = "dd/MM/yyyy";
		this.repositoryItemDateEditEx3.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.repositoryItemDateEditEx3.IsSearchControl = false;
		this.repositoryItemDateEditEx3.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.repositoryItemDateEditEx3.Name = "repositoryItemDateEditEx3";
		this.gridColumnEx96.Caption = "CreateTime";
		this.gridColumnEx96.ColumnEdit = this.repositoryItemTimeSpanEdit3;
		this.gridColumnEx96.CurrencyDateField = null;
		this.gridColumnEx96.CurrencyField = null;
		this.gridColumnEx96.CurrencyFixed = null;
		this.gridColumnEx96.CurrencyFixedRate = 0.0;
		this.gridColumnEx96.CurrencyRateField = null;
		this.gridColumnEx96.FieldName = "CreateTime";
		this.gridColumnEx96.Name = "gridColumnEx96";
		this.gridColumnEx96.OptionsColumn.ReadOnly = true;
		this.gridColumnEx96.PrimaryKey = false;
		this.gridColumnEx96.UnitField = null;
		this.repositoryItemTimeSpanEdit3.AutoHeight = false;
		this.repositoryItemTimeSpanEdit3.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemTimeSpanEdit3.Mask.EditMask = "dd.HH:mm:ss";
		this.repositoryItemTimeSpanEdit3.Name = "repositoryItemTimeSpanEdit3";
		this.gridColumnEx97.Caption = "UpdateDate";
		this.gridColumnEx97.ColumnEdit = this.repositoryItemDateEditEx4;
		this.gridColumnEx97.CurrencyDateField = null;
		this.gridColumnEx97.CurrencyField = null;
		this.gridColumnEx97.CurrencyFixed = null;
		this.gridColumnEx97.CurrencyFixedRate = 0.0;
		this.gridColumnEx97.CurrencyRateField = null;
		this.gridColumnEx97.FieldName = "UpdateDate";
		this.gridColumnEx97.Name = "gridColumnEx97";
		this.gridColumnEx97.OptionsColumn.ReadOnly = true;
		this.gridColumnEx97.PrimaryKey = false;
		this.gridColumnEx97.UnitField = null;
		this.repositoryItemDateEditEx4.AutoHeight = false;
		this.repositoryItemDateEditEx4.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEditEx4.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEditEx4.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.repositoryItemDateEditEx4.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.repositoryItemDateEditEx4.EditFormat.FormatString = "dd/MM/yyyy";
		this.repositoryItemDateEditEx4.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.repositoryItemDateEditEx4.IsSearchControl = false;
		this.repositoryItemDateEditEx4.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.repositoryItemDateEditEx4.Name = "repositoryItemDateEditEx4";
		this.gridColumnEx98.Caption = "UpdateTime";
		this.gridColumnEx98.ColumnEdit = this.repositoryItemTimeSpanEdit4;
		this.gridColumnEx98.CurrencyDateField = null;
		this.gridColumnEx98.CurrencyField = null;
		this.gridColumnEx98.CurrencyFixed = null;
		this.gridColumnEx98.CurrencyFixedRate = 0.0;
		this.gridColumnEx98.CurrencyRateField = null;
		this.gridColumnEx98.FieldName = "UpdateTime";
		this.gridColumnEx98.Name = "gridColumnEx98";
		this.gridColumnEx98.OptionsColumn.ReadOnly = true;
		this.gridColumnEx98.PrimaryKey = false;
		this.gridColumnEx98.UnitField = null;
		this.repositoryItemTimeSpanEdit4.AutoHeight = false;
		this.repositoryItemTimeSpanEdit4.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemTimeSpanEdit4.Mask.EditMask = "dd.HH:mm:ss";
		this.repositoryItemTimeSpanEdit4.Name = "repositoryItemTimeSpanEdit4";
		this.gridColumnEx99.Caption = "Completed";
		this.gridColumnEx99.CurrencyDateField = null;
		this.gridColumnEx99.CurrencyField = null;
		this.gridColumnEx99.CurrencyFixed = null;
		this.gridColumnEx99.CurrencyFixedRate = 0.0;
		this.gridColumnEx99.CurrencyRateField = null;
		this.gridColumnEx99.FieldName = "Completed";
		this.gridColumnEx99.Name = "gridColumnEx99";
		this.gridColumnEx99.PrimaryKey = false;
		this.gridColumnEx99.UnitField = null;
		this.gridColumnEx100.Caption = "Sub Project AbsEntry";
		this.gridColumnEx100.ColumnEdit = this.btn_Code;
		this.gridColumnEx100.CurrencyDateField = null;
		this.gridColumnEx100.CurrencyField = null;
		this.gridColumnEx100.CurrencyFixed = null;
		this.gridColumnEx100.CurrencyFixedRate = 0.0;
		this.gridColumnEx100.CurrencyRateField = null;
		this.gridColumnEx100.FieldName = "AbsEntry";
		this.gridColumnEx100.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx100.Name = "gridColumnEx100";
		this.gridColumnEx100.PrimaryKey = false;
		this.gridColumnEx100.UnitField = null;
		this.gridColumnEx100.Width = 130;
		this.xtra_Issue.Controls.Add(this.grd_lineissue);
		this.xtra_Issue.EditControl = null;
		this.xtra_Issue.EditValue = "Issues";
		this.xtra_Issue.FieldName = "xtra_Issue";
		this.xtra_Issue.IsSearchControl = false;
		this.xtra_Issue.Margin = new System.Windows.Forms.Padding(2);
		this.xtra_Issue.Name = "xtra_Issue";
		this.xtra_Issue.ReadOnly = false;
		this.xtra_Issue.SettingInVisible = false;
		this.xtra_Issue.SettingReadonly = false;
		this.xtra_Issue.Size = new System.Drawing.Size(880, 208);
		this.xtra_Issue.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtra_Issue.Text = "Issues";
		this.grd_lineissue.AllowPasteExcel = false;
		this.grd_lineissue.ClipboardData = null;
		this.grd_lineissue.ContextMenuStrip = this.ctm_Issues;
		this.grd_lineissue.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_lineissue.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(2);
		this.grd_lineissue.Location = new System.Drawing.Point(0, 0);
		this.grd_lineissue.MainView = this.gv_lineIssue;
		this.grd_lineissue.Margin = new System.Windows.Forms.Padding(2);
		this.grd_lineissue.Name = "grd_lineissue";
		this.grd_lineissue.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[12]
		{
			this.btn_Id, this.luk_priorityGrid, this.btn_report, this.btn_handle, this.btn_solution, this.luk_statusGrid, this.luk_TypeGrid, this.btn_IssueBp, this.btn_IssuePr, this.btn_IssueSLBy,
			this.btn_IssueTaskCode, this.btn_IssuwPrOwner
		});
		this.grd_lineissue.Size = new System.Drawing.Size(880, 208);
		this.grd_lineissue.TabIndex = 0;
		this.grd_lineissue.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_lineIssue });
		this.ctm_Issues.ImageScalingSize = new System.Drawing.Size(32, 32);
		this.ctm_Issues.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.toolStripMenuItemAddNew });
		this.ctm_Issues.Name = "ctm_Issues";
		this.ctm_Issues.Size = new System.Drawing.Size(128, 26);
		this.toolStripMenuItemAddNew.Name = "toolStripMenuItemAddNew";
		this.toolStripMenuItemAddNew.Size = new System.Drawing.Size(127, 22);
		this.toolStripMenuItemAddNew.Text = "New Issue";
		this.toolStripMenuItemAddNew.Click += new System.EventHandler(this.toolStripMenuItemAddNew_Click);
		this.gv_lineIssue.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_lineIssue.ColumnKey = null;
		this.gv_lineIssue.ColumnPanelRowHeight = 0;
		this.gv_lineIssue.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[18]
		{
			this.gridColumnEx13, this.gridColumnEx28, this.gridColumnEx69, this.gc_priorityy, this.gridColumnEx71, this.gridColumnEx72, this.gridColumnEx74, this.gridColumnEx78, this.gridColumnEx79, this.gridColumnEx80,
			this.gridColumnEx81, this.gridColumnEx82, this.gridColumnEx84, this.gridColumnEx85, this.gridColumnEx86, this.gridColumnEx87, this.gridColumnEx88, this.gridColumnEx89
		});
		this.gv_lineIssue.DetailHeight = 364;
		this.gv_lineIssue.DisableFormSetting = false;
		this.gv_lineIssue.DisplayException = false;
		this.gv_lineIssue.EditColumn = null;
		this.gv_lineIssue.FieldCurrency = null;
		this.gv_lineIssue.FieldRate = null;
		this.gv_lineIssue.FooterPanelHeight = 0;
		gridFormatRule3.Column = this.gc_priorityy;
		gridFormatRule3.ColumnApplyTo = this.gc_priorityy;
		gridFormatRule3.Name = "Format0";
		formatConditionIconSet4.CategoryName = "Shapes";
		formatConditionIconSetIcon13.PredefinedName = "TrafficLights4_3.png";
		formatConditionIconSetIcon13.Value = new decimal(new int[4] { 4, 0, 0, 0 });
		formatConditionIconSetIcon13.ValueComparison = DevExpress.XtraEditors.FormatConditionComparisonType.GreaterOrEqual;
		formatConditionIconSetIcon14.PredefinedName = "TrafficLights23_2.png";
		formatConditionIconSetIcon14.Value = new decimal(new int[4] { 3, 0, 0, 0 });
		formatConditionIconSetIcon14.ValueComparison = DevExpress.XtraEditors.FormatConditionComparisonType.GreaterOrEqual;
		formatConditionIconSetIcon15.PredefinedName = "TrafficLights4_4.png";
		formatConditionIconSetIcon15.Value = new decimal(new int[4] { 2, 0, 0, 0 });
		formatConditionIconSetIcon15.ValueComparison = DevExpress.XtraEditors.FormatConditionComparisonType.GreaterOrEqual;
		formatConditionIconSetIcon16.PredefinedName = "TrafficLights4_1.png";
		formatConditionIconSetIcon16.Value = new decimal(new int[4] { 1, 0, 0, 0 });
		formatConditionIconSetIcon16.ValueComparison = DevExpress.XtraEditors.FormatConditionComparisonType.GreaterOrEqual;
		formatConditionIconSet4.Icons.Add(formatConditionIconSetIcon13);
		formatConditionIconSet4.Icons.Add(formatConditionIconSetIcon14);
		formatConditionIconSet4.Icons.Add(formatConditionIconSetIcon15);
		formatConditionIconSet4.Icons.Add(formatConditionIconSetIcon16);
		formatConditionIconSet4.Name = "RedToBlack";
		formatConditionIconSet4.ValueType = DevExpress.XtraEditors.FormatConditionValueType.Number;
		formatConditionRuleIconSet4.IconSet = formatConditionIconSet4;
		gridFormatRule3.Rule = formatConditionRuleIconSet4;
		this.gv_lineIssue.FormatRules.Add(gridFormatRule3);
		this.gv_lineIssue.FormId = "oProjectManagement";
		this.gv_lineIssue.GridControl = this.grd_lineissue;
		this.gv_lineIssue.GroupRowHeight = 0;
		this.gv_lineIssue.HasFormSetting = true;
		this.gv_lineIssue.IgnoreFilter = false;
		this.gv_lineIssue.IndexEditRow = -1;
		this.gv_lineIssue.IndicatorWidth = 35;
		this.gv_lineIssue.IsApzTable = false;
		this.gv_lineIssue.IsInsertNewRow = false;
		this.gv_lineIssue.IsLoadingData = false;
		this.gv_lineIssue.IsMappingContextAction = false;
		this.gv_lineIssue.ItemId = "APZ_TGIE";
		this.gv_lineIssue.LevelIndent = 0;
		this.gv_lineIssue.LstColumnAllowBatch = null;
		this.gv_lineIssue.LstColumnNotAllowBatch = null;
		this.gv_lineIssue.LstColumnNotEditable = null;
		this.gv_lineIssue.LstColumNotSetting = null;
		this.gv_lineIssue.LstOldUDF = null;
		this.gv_lineIssue.Name = "gv_lineIssue";
		this.gv_lineIssue.OptionsView.BestFitMaxRowCount = 100;
		this.gv_lineIssue.OptionsView.ColumnAutoWidth = false;
		this.gv_lineIssue.OptionsView.ShowGroupPanel = false;
		this.gv_lineIssue.PreviewIndent = 0;
		this.gv_lineIssue.ReadOnly = false;
		this.gv_lineIssue.RowHeight = 0;
		this.gv_lineIssue.SettingReadonly = false;
		this.gv_lineIssue.UseBatchMod = false;
		this.gv_lineIssue.ViewCaptionHeight = 0;
		this.gv_lineIssue.CustomDrawCell += new DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventHandler(this.gv_lineIssue_CustomDrawCell);
		this.gv_lineIssue.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_lineIssue_RowCellStyle);
		this.gridColumnEx13.Caption = "No.";
		this.gridColumnEx13.ColumnEdit = this.btn_Id;
		this.gridColumnEx13.CurrencyDateField = null;
		this.gridColumnEx13.CurrencyField = null;
		this.gridColumnEx13.CurrencyFixed = null;
		this.gridColumnEx13.CurrencyFixedRate = 0.0;
		this.gridColumnEx13.CurrencyRateField = null;
		this.gridColumnEx13.FieldName = "Id";
		this.gridColumnEx13.Name = "gridColumnEx13";
		this.gridColumnEx13.OptionsColumn.ReadOnly = true;
		this.gridColumnEx13.PrimaryKey = false;
		this.gridColumnEx13.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridColumnEx13.UnitField = null;
		this.btn_Id.AutoHeight = false;
		this.btn_Id.AutoLoadDataSource = true;
		editorButtonImageOptions34.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions60.Image");
		this.btn_Id.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions34, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance55, appearanceHovered55, appearancePressed55, appearanceDisabled55, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_Id.ContainerControl = this;
		this.btn_Id.DisplayMember = "Id";
		this.btn_Id.DisplayMember2 = "";
		this.btn_Id.ListMandatoryField = null;
		this.btn_Id.LoadDataSourceWhenEnter = false;
		this.btn_Id.Name = "btn_Id";
		this.btn_Id.ReadOnly = true;
		this.btn_Id.SeparateCharacterForMulti = ';';
		this.btn_Id.SeparateString = ", ";
		this.btn_Id.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.TrackingIssue;
		this.btn_Id.UsingAutoCompleteBox = false;
		this.btn_Id.UsingDataSource = true;
		this.btn_Id.ValidateWhenNoDataSource = false;
		this.btn_Id.ValueMember = "Id";
		this.gridColumnEx28.Caption = "IssueDate";
		this.gridColumnEx28.CurrencyDateField = null;
		this.gridColumnEx28.CurrencyField = null;
		this.gridColumnEx28.CurrencyFixed = null;
		this.gridColumnEx28.CurrencyFixedRate = 0.0;
		this.gridColumnEx28.CurrencyRateField = null;
		this.gridColumnEx28.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gridColumnEx28.FieldName = "IssueDate";
		this.gridColumnEx28.Name = "gridColumnEx28";
		this.gridColumnEx28.OptionsColumn.ReadOnly = true;
		this.gridColumnEx28.PrimaryKey = false;
		this.gridColumnEx28.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowForFocusedCell;
		this.gridColumnEx28.UnitField = null;
		this.gridColumnEx28.Width = 150;
		this.gridColumnEx69.Caption = "IssueType";
		this.gridColumnEx69.ColumnEdit = this.luk_TypeGrid;
		this.gridColumnEx69.CurrencyDateField = null;
		this.gridColumnEx69.CurrencyField = null;
		this.gridColumnEx69.CurrencyFixed = null;
		this.gridColumnEx69.CurrencyFixedRate = 0.0;
		this.gridColumnEx69.CurrencyRateField = null;
		this.gridColumnEx69.FieldName = "IssueType";
		this.gridColumnEx69.Name = "gridColumnEx69";
		this.gridColumnEx69.OptionsColumn.ReadOnly = true;
		this.gridColumnEx69.PrimaryKey = false;
		this.gridColumnEx69.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowForFocusedCell;
		this.gridColumnEx69.UnitField = null;
		this.gridColumnEx69.Width = 150;
		this.luk_TypeGrid.AutoHeight = false;
		this.luk_TypeGrid.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
		this.luk_TypeGrid.CheckFields = null;
		this.luk_TypeGrid.CheckTables = null;
		this.luk_TypeGrid.Name = "luk_TypeGrid";
		this.luk_TypeGrid.NullText = "";
		this.luk_TypeGrid.ShowFooter = false;
		this.luk_TypeGrid.ShowHeader = false;
		this.gridColumnEx71.AppearanceCell.Options.UseTextOptions = true;
		this.gridColumnEx71.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.gridColumnEx71.Caption = "Status";
		this.gridColumnEx71.ColumnEdit = this.luk_statusGrid;
		this.gridColumnEx71.CurrencyDateField = null;
		this.gridColumnEx71.CurrencyField = null;
		this.gridColumnEx71.CurrencyFixed = null;
		this.gridColumnEx71.CurrencyFixedRate = 0.0;
		this.gridColumnEx71.CurrencyRateField = null;
		this.gridColumnEx71.FieldName = "Status";
		this.gridColumnEx71.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx71.Name = "gridColumnEx71";
		this.gridColumnEx71.OptionsColumn.ReadOnly = true;
		this.gridColumnEx71.PrimaryKey = false;
		this.gridColumnEx71.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridColumnEx71.UnitField = null;
		this.gridColumnEx71.Width = 150;
		this.luk_statusGrid.AutoHeight = false;
		this.luk_statusGrid.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_statusGrid.IsSearchControl = false;
		this.luk_statusGrid.Name = "luk_statusGrid";
		this.luk_statusGrid.NullText = "";
		this.luk_statusGrid.ShowFooter = false;
		this.luk_statusGrid.ShowHeader = false;
		this.gridColumnEx72.Caption = "Reporter";
		this.gridColumnEx72.ColumnEdit = this.btn_report;
		this.gridColumnEx72.CurrencyDateField = null;
		this.gridColumnEx72.CurrencyField = null;
		this.gridColumnEx72.CurrencyFixed = null;
		this.gridColumnEx72.CurrencyFixedRate = 0.0;
		this.gridColumnEx72.CurrencyRateField = null;
		this.gridColumnEx72.FieldName = "Reporter";
		this.gridColumnEx72.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx72.Name = "gridColumnEx72";
		this.gridColumnEx72.OptionsColumn.ReadOnly = true;
		this.gridColumnEx72.PrimaryKey = false;
		this.gridColumnEx72.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridColumnEx72.UnitField = null;
		this.gridColumnEx72.Width = 150;
		this.btn_report.AutoHeight = false;
		this.btn_report.AutoLoadDataSource = true;
		editorButtonImageOptions35.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions61.Image");
		this.btn_report.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions35, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance56, appearanceHovered56, appearancePressed56, appearanceDisabled56, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_report.ContainerControl = this;
		this.btn_report.DisplayMember = "lastName";
		this.btn_report.DisplayMember2 = "firstName";
		this.btn_report.ListMandatoryField = null;
		this.btn_report.LoadDataSourceWhenEnter = false;
		this.btn_report.Name = "btn_report";
		this.btn_report.ReadOnly = true;
		this.btn_report.SeparateCharacterForMulti = ';';
		this.btn_report.SeparateString = ", ";
		this.btn_report.UsingAutoCompleteBox = false;
		this.btn_report.UsingDataSource = true;
		this.btn_report.ValidateWhenNoDataSource = false;
		this.btn_report.ValueMember = "EmpId";
		this.gridColumnEx74.Caption = "HandleBy";
		this.gridColumnEx74.ColumnEdit = this.btn_handle;
		this.gridColumnEx74.CurrencyDateField = null;
		this.gridColumnEx74.CurrencyField = null;
		this.gridColumnEx74.CurrencyFixed = null;
		this.gridColumnEx74.CurrencyFixedRate = 0.0;
		this.gridColumnEx74.CurrencyRateField = null;
		this.gridColumnEx74.FieldName = "HandleBy";
		this.gridColumnEx74.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx74.Name = "gridColumnEx74";
		this.gridColumnEx74.OptionsColumn.ReadOnly = true;
		this.gridColumnEx74.PrimaryKey = false;
		this.gridColumnEx74.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridColumnEx74.UnitField = null;
		this.gridColumnEx74.Width = 150;
		this.btn_handle.AutoHeight = false;
		this.btn_handle.AutoLoadDataSource = true;
		editorButtonImageOptions36.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions62.Image");
		this.btn_handle.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions36, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance57, appearanceHovered57, appearancePressed57, appearanceDisabled57, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_handle.ContainerControl = this;
		this.btn_handle.DisplayMember = "lastName";
		this.btn_handle.DisplayMember2 = "firstName";
		this.btn_handle.ListMandatoryField = null;
		this.btn_handle.LoadDataSourceWhenEnter = false;
		this.btn_handle.Name = "btn_handle";
		this.btn_handle.ReadOnly = true;
		this.btn_handle.SeparateCharacterForMulti = ';';
		this.btn_handle.SeparateString = ", ";
		this.btn_handle.UsingAutoCompleteBox = false;
		this.btn_handle.UsingDataSource = true;
		this.btn_handle.ValidateWhenNoDataSource = false;
		this.btn_handle.ValueMember = "EmpId";
		this.gridColumnEx78.Caption = "SolutionId";
		this.gridColumnEx78.ColumnEdit = this.btn_solution;
		this.gridColumnEx78.CurrencyDateField = null;
		this.gridColumnEx78.CurrencyField = null;
		this.gridColumnEx78.CurrencyFixed = null;
		this.gridColumnEx78.CurrencyFixedRate = 0.0;
		this.gridColumnEx78.CurrencyRateField = null;
		this.gridColumnEx78.FieldName = "SolutionId";
		this.gridColumnEx78.Name = "gridColumnEx78";
		this.gridColumnEx78.OptionsColumn.ReadOnly = true;
		this.gridColumnEx78.PrimaryKey = false;
		this.gridColumnEx78.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridColumnEx78.UnitField = null;
		this.gridColumnEx78.Width = 150;
		this.btn_solution.AutoHeight = false;
		this.btn_solution.AutoLoadDataSource = true;
		editorButtonImageOptions37.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions63.Image");
		this.btn_solution.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions37, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance58, appearanceHovered58, appearancePressed58, appearanceDisabled58, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_solution.ContainerControl = this;
		this.btn_solution.DisplayMember = "Subject";
		this.btn_solution.DisplayMember2 = "";
		this.btn_solution.ListMandatoryField = null;
		this.btn_solution.LoadDataSourceWhenEnter = false;
		this.btn_solution.Name = "btn_solution";
		this.btn_solution.ReadOnly = true;
		this.btn_solution.SeparateCharacterForMulti = ';';
		this.btn_solution.SeparateString = ", ";
		this.btn_solution.UsingAutoCompleteBox = false;
		this.btn_solution.UsingDataSource = true;
		this.btn_solution.ValidateWhenNoDataSource = false;
		this.btn_solution.ValueMember = "SltCode";
		this.gridColumnEx79.Caption = "ResponseDate";
		this.gridColumnEx79.CurrencyDateField = null;
		this.gridColumnEx79.CurrencyField = null;
		this.gridColumnEx79.CurrencyFixed = null;
		this.gridColumnEx79.CurrencyFixedRate = 0.0;
		this.gridColumnEx79.CurrencyRateField = null;
		this.gridColumnEx79.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gridColumnEx79.FieldName = "ResponseDate";
		this.gridColumnEx79.Name = "gridColumnEx79";
		this.gridColumnEx79.OptionsColumn.ReadOnly = true;
		this.gridColumnEx79.PrimaryKey = false;
		this.gridColumnEx79.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowForFocusedCell;
		this.gridColumnEx79.UnitField = null;
		this.gridColumnEx79.Width = 150;
		this.gridColumnEx80.Caption = "BpCode";
		this.gridColumnEx80.ColumnEdit = this.btn_IssueBp;
		this.gridColumnEx80.CurrencyDateField = null;
		this.gridColumnEx80.CurrencyField = null;
		this.gridColumnEx80.CurrencyFixed = null;
		this.gridColumnEx80.CurrencyFixedRate = 0.0;
		this.gridColumnEx80.CurrencyRateField = null;
		this.gridColumnEx80.FieldName = "BpCode";
		this.gridColumnEx80.Name = "gridColumnEx80";
		this.gridColumnEx80.OptionsColumn.ReadOnly = true;
		this.gridColumnEx80.PrimaryKey = false;
		this.gridColumnEx80.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridColumnEx80.UnitField = null;
		this.gridColumnEx80.Width = 150;
		this.btn_IssueBp.AutoHeight = false;
		this.btn_IssueBp.AutoLoadDataSource = true;
		editorButtonImageOptions38.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions64.Image");
		this.btn_IssueBp.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions38, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance59, appearanceHovered59, appearancePressed59, appearanceDisabled59, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_IssueBp.ContainerControl = this;
		this.btn_IssueBp.DisplayMember = "CardCode";
		this.btn_IssueBp.DisplayMember2 = "";
		this.btn_IssueBp.ListMandatoryField = null;
		this.btn_IssueBp.LoadDataSourceWhenEnter = false;
		this.btn_IssueBp.Name = "btn_IssueBp";
		this.btn_IssueBp.ReadOnly = true;
		this.btn_IssueBp.SeparateCharacterForMulti = ';';
		this.btn_IssueBp.SeparateString = ", ";
		this.btn_IssueBp.UsingAutoCompleteBox = false;
		this.btn_IssueBp.UsingDataSource = true;
		this.btn_IssueBp.ValidateWhenNoDataSource = false;
		this.btn_IssueBp.ValueMember = "CardCode";
		this.gridColumnEx81.Caption = "WBS";
		this.gridColumnEx81.CurrencyDateField = null;
		this.gridColumnEx81.CurrencyField = null;
		this.gridColumnEx81.CurrencyFixed = null;
		this.gridColumnEx81.CurrencyFixedRate = 0.0;
		this.gridColumnEx81.CurrencyRateField = null;
		this.gridColumnEx81.FieldName = "WbsCode";
		this.gridColumnEx81.Name = "gridColumnEx81";
		this.gridColumnEx81.OptionsColumn.ReadOnly = true;
		this.gridColumnEx81.PrimaryKey = false;
		this.gridColumnEx81.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridColumnEx81.UnitField = null;
		this.gridColumnEx81.Width = 150;
		this.gridColumnEx82.Caption = "SolutionBy";
		this.gridColumnEx82.ColumnEdit = this.btn_IssueSLBy;
		this.gridColumnEx82.CurrencyDateField = null;
		this.gridColumnEx82.CurrencyField = null;
		this.gridColumnEx82.CurrencyFixed = null;
		this.gridColumnEx82.CurrencyFixedRate = 0.0;
		this.gridColumnEx82.CurrencyRateField = null;
		this.gridColumnEx82.FieldName = "SolutionBy";
		this.gridColumnEx82.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx82.Name = "gridColumnEx82";
		this.gridColumnEx82.OptionsColumn.ReadOnly = true;
		this.gridColumnEx82.PrimaryKey = false;
		this.gridColumnEx82.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridColumnEx82.UnitField = null;
		this.gridColumnEx82.Width = 150;
		this.btn_IssueSLBy.AutoHeight = false;
		this.btn_IssueSLBy.AutoLoadDataSource = true;
		this.btn_IssueSLBy.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, imageOptions22, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance60, appearanceHovered60, appearancePressed60, appearanceDisabled60, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_IssueSLBy.ContainerControl = this;
		this.btn_IssueSLBy.DisplayMember = "U_NAME";
		this.btn_IssueSLBy.DisplayMember2 = "";
		this.btn_IssueSLBy.ListMandatoryField = null;
		this.btn_IssueSLBy.LoadDataSourceWhenEnter = false;
		this.btn_IssueSLBy.Name = "btn_IssueSLBy";
		this.btn_IssueSLBy.ReadOnly = true;
		this.btn_IssueSLBy.SeparateCharacterForMulti = ';';
		this.btn_IssueSLBy.SeparateString = ", ";
		this.btn_IssueSLBy.UsingAutoCompleteBox = false;
		this.btn_IssueSLBy.UsingDataSource = true;
		this.btn_IssueSLBy.ValidateWhenNoDataSource = false;
		this.btn_IssueSLBy.ValueMember = "USERID";
		this.gridColumnEx84.Caption = "AttachedFiled";
		this.gridColumnEx84.CurrencyDateField = null;
		this.gridColumnEx84.CurrencyField = null;
		this.gridColumnEx84.CurrencyFixed = null;
		this.gridColumnEx84.CurrencyFixedRate = 0.0;
		this.gridColumnEx84.CurrencyRateField = null;
		this.gridColumnEx84.FieldName = "Attacheds";
		this.gridColumnEx84.Name = "gridColumnEx84";
		this.gridColumnEx84.OptionsColumn.ReadOnly = true;
		this.gridColumnEx84.PrimaryKey = false;
		this.gridColumnEx84.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowForFocusedCell;
		this.gridColumnEx84.UnitField = null;
		this.gridColumnEx84.Width = 150;
		this.gridColumnEx85.Caption = "Note";
		this.gridColumnEx85.CurrencyDateField = null;
		this.gridColumnEx85.CurrencyField = null;
		this.gridColumnEx85.CurrencyFixed = null;
		this.gridColumnEx85.CurrencyFixedRate = 0.0;
		this.gridColumnEx85.CurrencyRateField = null;
		this.gridColumnEx85.FieldName = "Note";
		this.gridColumnEx85.Name = "gridColumnEx85";
		this.gridColumnEx85.OptionsColumn.ReadOnly = true;
		this.gridColumnEx85.PrimaryKey = false;
		this.gridColumnEx85.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowForFocusedCell;
		this.gridColumnEx85.UnitField = null;
		this.gridColumnEx85.Width = 150;
		this.gridColumnEx86.Caption = "ProjectOwner";
		this.gridColumnEx86.ColumnEdit = this.btn_IssuwPrOwner;
		this.gridColumnEx86.CurrencyDateField = null;
		this.gridColumnEx86.CurrencyField = null;
		this.gridColumnEx86.CurrencyFixed = null;
		this.gridColumnEx86.CurrencyFixedRate = 0.0;
		this.gridColumnEx86.CurrencyRateField = null;
		this.gridColumnEx86.FieldName = "ProjectOwner";
		this.gridColumnEx86.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx86.Name = "gridColumnEx86";
		this.gridColumnEx86.OptionsColumn.ReadOnly = true;
		this.gridColumnEx86.PrimaryKey = false;
		this.gridColumnEx86.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridColumnEx86.UnitField = null;
		this.btn_IssuwPrOwner.AutoHeight = false;
		this.btn_IssuwPrOwner.AutoLoadDataSource = true;
		this.btn_IssuwPrOwner.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, imageOptions23, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance61, appearanceHovered61, appearancePressed61, appearanceDisabled61, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_IssuwPrOwner.ContainerControl = this;
		this.btn_IssuwPrOwner.DisplayMember = "lastName";
		this.btn_IssuwPrOwner.DisplayMember2 = "FirstName";
		this.btn_IssuwPrOwner.ListMandatoryField = null;
		this.btn_IssuwPrOwner.LoadDataSourceWhenEnter = false;
		this.btn_IssuwPrOwner.Name = "btn_IssuwPrOwner";
		this.btn_IssuwPrOwner.ReadOnly = true;
		this.btn_IssuwPrOwner.SeparateCharacterForMulti = ';';
		this.btn_IssuwPrOwner.SeparateString = ", ";
		this.btn_IssuwPrOwner.UsingAutoCompleteBox = false;
		this.btn_IssuwPrOwner.UsingDataSource = true;
		this.btn_IssuwPrOwner.ValidateWhenNoDataSource = false;
		this.btn_IssuwPrOwner.ValueMember = "EmpId";
		this.gridColumnEx87.Caption = "UserSign";
		this.gridColumnEx87.CurrencyDateField = null;
		this.gridColumnEx87.CurrencyField = null;
		this.gridColumnEx87.CurrencyFixed = null;
		this.gridColumnEx87.CurrencyFixedRate = 0.0;
		this.gridColumnEx87.CurrencyRateField = null;
		this.gridColumnEx87.FieldName = "UserSign";
		this.gridColumnEx87.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx87.Name = "gridColumnEx87";
		this.gridColumnEx87.OptionsColumn.ReadOnly = true;
		this.gridColumnEx87.PrimaryKey = false;
		this.gridColumnEx87.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowForFocusedCell;
		this.gridColumnEx87.UnitField = null;
		this.gridColumnEx88.Caption = "UserSign2";
		this.gridColumnEx88.CurrencyDateField = null;
		this.gridColumnEx88.CurrencyField = null;
		this.gridColumnEx88.CurrencyFixed = null;
		this.gridColumnEx88.CurrencyFixedRate = 0.0;
		this.gridColumnEx88.CurrencyRateField = null;
		this.gridColumnEx88.FieldName = "UserSign2";
		this.gridColumnEx88.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx88.Name = "gridColumnEx88";
		this.gridColumnEx88.OptionsColumn.ReadOnly = true;
		this.gridColumnEx88.PrimaryKey = false;
		this.gridColumnEx88.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowForFocusedCell;
		this.gridColumnEx88.UnitField = null;
		this.gridColumnEx89.Caption = "AttachedFiled";
		this.gridColumnEx89.CurrencyDateField = null;
		this.gridColumnEx89.CurrencyField = null;
		this.gridColumnEx89.CurrencyFixed = null;
		this.gridColumnEx89.CurrencyFixedRate = 0.0;
		this.gridColumnEx89.CurrencyRateField = null;
		this.gridColumnEx89.FieldName = "AttachedFiled";
		this.gridColumnEx89.Name = "gridColumnEx89";
		this.gridColumnEx89.OptionsColumn.ReadOnly = true;
		this.gridColumnEx89.PrimaryKey = false;
		this.gridColumnEx89.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowForFocusedCell;
		this.gridColumnEx89.UnitField = null;
		this.btn_IssuePr.AutoHeight = false;
		this.btn_IssuePr.AutoLoadDataSource = true;
		this.btn_IssuePr.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, imageOptions24, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance62, appearanceHovered62, appearancePressed62, appearanceDisabled62, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_IssuePr.ContainerControl = this;
		this.btn_IssuePr.DisplayMember = "Code";
		this.btn_IssuePr.DisplayMember2 = "";
		this.btn_IssuePr.ListMandatoryField = null;
		this.btn_IssuePr.LoadDataSourceWhenEnter = false;
		this.btn_IssuePr.Name = "btn_IssuePr";
		this.btn_IssuePr.SeparateCharacterForMulti = ';';
		this.btn_IssuePr.SeparateString = ", ";
		this.btn_IssuePr.UsingAutoCompleteBox = false;
		this.btn_IssuePr.UsingDataSource = true;
		this.btn_IssuePr.ValidateWhenNoDataSource = false;
		this.btn_IssuePr.ValueMember = "AbsEntry";
		this.btn_IssueTaskCode.AutoHeight = false;
		this.btn_IssueTaskCode.AutoLoadDataSource = true;
		this.btn_IssueTaskCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, imageOptions25, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance63, appearanceHovered63, appearancePressed63, appearanceDisabled63, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_IssueTaskCode.ContainerControl = this;
		this.btn_IssueTaskCode.DisplayMember = "TaskCode";
		this.btn_IssueTaskCode.DisplayMember2 = "";
		this.btn_IssueTaskCode.ListMandatoryField = null;
		this.btn_IssueTaskCode.LoadDataSourceWhenEnter = false;
		this.btn_IssueTaskCode.Name = "btn_IssueTaskCode";
		this.btn_IssueTaskCode.SeparateCharacterForMulti = ';';
		this.btn_IssueTaskCode.SeparateString = ", ";
		this.btn_IssueTaskCode.UsingAutoCompleteBox = false;
		this.btn_IssueTaskCode.UsingDataSource = true;
		this.btn_IssueTaskCode.ValidateWhenNoDataSource = false;
		this.btn_IssueTaskCode.ValueMember = "TaskCode";
		this.tab_TaskList.Controls.Add(this.tv_ListCompleted);
		this.tab_TaskList.EditControl = null;
		this.tab_TaskList.EditValue = "Task List";
		this.tab_TaskList.FieldName = "tab_TaskList";
		this.tab_TaskList.IsSearchControl = false;
		this.tab_TaskList.Margin = new System.Windows.Forms.Padding(2);
		this.tab_TaskList.Name = "tab_TaskList";
		this.tab_TaskList.ReadOnly = false;
		this.tab_TaskList.SettingInVisible = false;
		this.tab_TaskList.SettingReadonly = false;
		this.tab_TaskList.Size = new System.Drawing.Size(880, 208);
		this.tab_TaskList.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_TaskList.Text = "Task List";
		this.tv_ListCompleted.AllowPasteExcel = false;
		this.tv_ListCompleted.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.tv_ListCompleted.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.tv_ListCompleted.ColumnKey = null;
		this.tv_ListCompleted.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[13]
		{
			this.tl_RowId, this.tl_ClgCode, this.tl_Closed_Completed, this.tl_Details, this.tl_ProjectWBS_Completed, this.tl_AssignTo_Completed, this.tl_DueDate_Completed, this.tl_ProjectEntry_Completed, this.tl_OrderLine, this.tl_CntctDate,
			this.tl_ENDTime, this.tl_CloseDate_Completed, this.tl_Priority_Completed
		});
		this.tv_ListCompleted.ContextMenuStrip = this.ctm_TaskList;
		this.tv_ListCompleted.Cursor = System.Windows.Forms.Cursors.Default;
		this.tv_ListCompleted.DisableFormSetting = false;
		this.tv_ListCompleted.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tv_ListCompleted.EditColumn = null;
		this.tv_ListCompleted.FieldCurrency = null;
		this.tv_ListCompleted.FieldRate = null;
		treeListFormatRule2.Column = this.tl_Priority_Completed;
		treeListFormatRule2.ColumnApplyTo = this.tl_Priority_Completed;
		treeListFormatRule2.Name = "Format0";
		formatConditionIconSet5.CategoryName = "Shapes";
		formatConditionIconSetIcon17.PredefinedName = "RedToBlack4_1.png";
		formatConditionIconSetIcon17.Value = new decimal(new int[4] { 3, 0, 0, 0 });
		formatConditionIconSetIcon17.ValueComparison = DevExpress.XtraEditors.FormatConditionComparisonType.GreaterOrEqual;
		formatConditionIconSetIcon18.PredefinedName = "TrafficLights23_2.png";
		formatConditionIconSetIcon18.Value = new decimal(new int[4] { 2, 0, 0, 0 });
		formatConditionIconSetIcon18.ValueComparison = DevExpress.XtraEditors.FormatConditionComparisonType.GreaterOrEqual;
		formatConditionIconSetIcon19.PredefinedName = "RedToBlack4_3.png";
		formatConditionIconSetIcon19.Value = new decimal(new int[4] { 1, 0, 0, 0 });
		formatConditionIconSetIcon19.ValueComparison = DevExpress.XtraEditors.FormatConditionComparisonType.GreaterOrEqual;
		formatConditionIconSetIcon20.PredefinedName = "Signs3_1.png";
		formatConditionIconSetIcon20.ValueComparison = DevExpress.XtraEditors.FormatConditionComparisonType.GreaterOrEqual;
		formatConditionIconSet5.Icons.Add(formatConditionIconSetIcon17);
		formatConditionIconSet5.Icons.Add(formatConditionIconSetIcon18);
		formatConditionIconSet5.Icons.Add(formatConditionIconSetIcon19);
		formatConditionIconSet5.Icons.Add(formatConditionIconSetIcon20);
		formatConditionIconSet5.Name = "RedToBlack";
		formatConditionRuleIconSet5.IconSet = formatConditionIconSet5;
		treeListFormatRule2.Rule = formatConditionRuleIconSet5;
		this.tv_ListCompleted.FormatRules.Add(treeListFormatRule2);
		this.tv_ListCompleted.FormId = "oProjectManagement";
		this.tv_ListCompleted.HasFormSetting = true;
		this.tv_ListCompleted.IndexEditRow = 0;
		this.tv_ListCompleted.IndicatorWidth = 40;
		this.tv_ListCompleted.IsApzTable = false;
		this.tv_ListCompleted.IsLoadingData = false;
		this.tv_ListCompleted.ItemId = "APZ_OCLG";
		this.tv_ListCompleted.KeyFieldName = "ClgCode";
		this.tv_ListCompleted.Location = new System.Drawing.Point(0, 0);
		this.tv_ListCompleted.LstColumnNotEditable = null;
		this.tv_ListCompleted.LstColumNotSetting = null;
		this.tv_ListCompleted.LstOldUDF = null;
		this.tv_ListCompleted.Margin = new System.Windows.Forms.Padding(2);
		this.tv_ListCompleted.Name = "tv_ListCompleted";
		this.tv_ListCompleted.NewItemRowText = "New Task";
		this.tv_ListCompleted.OptionsNavigation.AutoMoveRowFocus = true;
		this.tv_ListCompleted.OptionsView.AutoWidth = false;
		this.tv_ListCompleted.ParentFieldName = "parentTask";
		this.tv_ListCompleted.ReadOnly = false;
		this.tv_ListCompleted.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[7] { this.rep_btn_Closed, this.rep_btn_Open, this.rep_btn_0, this.rep_btn_1, this.rep_btn_2, this.rep_btn_3, this.luk_prioritylistcompleted });
		this.tv_ListCompleted.SettingReadonly = false;
		this.tv_ListCompleted.Size = new System.Drawing.Size(880, 208);
		this.tv_ListCompleted.TabIndex = 1;
		this.tv_ListCompleted.CustomNodeCellEdit += new DevExpress.XtraTreeList.GetCustomNodeCellEditEventHandler(this.tv_ListCompleted_CustomNodeCellEdit);
		this.tv_ListCompleted.NodeCellStyle += new DevExpress.XtraTreeList.GetCustomNodeCellStyleEventHandler(this.tv_ListCompleted_NodeCellStyle);
		this.tv_ListCompleted.CustomUnboundColumnData += new DevExpress.XtraTreeList.CustomColumnDataEventHandler(this.tv_ListCompleted_CustomUnboundColumnData);
		this.tv_ListCompleted.CustomColumnDisplayText += new DevExpress.XtraTreeList.CustomColumnDisplayTextEventHandler(this.tv_ListCompleted_CustomColumnDisplayText);
		this.tv_ListCompleted.CustomDrawNodeIndicator += new DevExpress.XtraTreeList.CustomDrawNodeIndicatorEventHandler(this.tv_ListCompleted_CustomDrawNodeIndicator);
		this.tv_ListCompleted.DoubleClick += new System.EventHandler(this.tv_ListCompleted_DoubleClick);
		this.tl_RowId.Caption = "Id";
		this.tl_RowId.CurrencyDateField = null;
		this.tl_RowId.CurrencyField = null;
		this.tl_RowId.CurrencyFixed = null;
		this.tl_RowId.CurrencyFixedRate = 0.0;
		this.tl_RowId.CurrencyRateField = null;
		this.tl_RowId.FieldName = "RowId";
		this.tl_RowId.Name = "tl_RowId";
		this.tl_RowId.Width = 25;
		this.tl_ClgCode.Caption = "Code";
		this.tl_ClgCode.CurrencyDateField = null;
		this.tl_ClgCode.CurrencyField = null;
		this.tl_ClgCode.CurrencyFixed = null;
		this.tl_ClgCode.CurrencyFixedRate = 0.0;
		this.tl_ClgCode.CurrencyRateField = null;
		this.tl_ClgCode.FieldName = "ClgCode";
		this.tl_ClgCode.Name = "tl_ClgCode";
		this.tl_Closed_Completed.Caption = " ";
		this.tl_Closed_Completed.ColumnEdit = this.rep_btn_Closed;
		this.tl_Closed_Completed.CurrencyDateField = null;
		this.tl_Closed_Completed.CurrencyField = null;
		this.tl_Closed_Completed.CurrencyFixed = null;
		this.tl_Closed_Completed.CurrencyFixedRate = 0.0;
		this.tl_Closed_Completed.CurrencyRateField = null;
		this.tl_Closed_Completed.CustomizationCaption = " ";
		this.tl_Closed_Completed.FieldName = "Closed";
		this.tl_Closed_Completed.Name = "tl_Closed_Completed";
		this.tl_Closed_Completed.OptionsColumn.AllowSize = false;
		this.tl_Closed_Completed.OptionsColumn.FixedWidth = true;
		this.tl_Closed_Completed.ShowButtonMode = DevExpress.XtraTreeList.ShowButtonModeEnum.ShowAlways;
		this.tl_Closed_Completed.Width = 24;
		this.rep_btn_Closed.AutoHeight = false;
		editorButtonImageOptions39.Image = Apzon.Forms.Properties.Resources.completed;
		this.rep_btn_Closed.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, editorButtonImageOptions39, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance64, appearanceHovered64, appearancePressed64, appearanceDisabled64, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_btn_Closed.Name = "rep_btn_Closed";
		this.rep_btn_Closed.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
		this.tl_Details.Caption = "Details";
		this.tl_Details.CurrencyDateField = null;
		this.tl_Details.CurrencyField = null;
		this.tl_Details.CurrencyFixed = null;
		this.tl_Details.CurrencyFixedRate = 0.0;
		this.tl_Details.CurrencyRateField = null;
		this.tl_Details.FieldName = "Details";
		this.tl_Details.Name = "tl_Details";
		this.tl_Details.OptionsColumn.AllowEdit = false;
		this.tl_Details.Width = 515;
		this.tl_ProjectWBS_Completed.Caption = "WBS";
		this.tl_ProjectWBS_Completed.CurrencyDateField = null;
		this.tl_ProjectWBS_Completed.CurrencyField = null;
		this.tl_ProjectWBS_Completed.CurrencyFixed = null;
		this.tl_ProjectWBS_Completed.CurrencyFixedRate = 0.0;
		this.tl_ProjectWBS_Completed.CurrencyRateField = null;
		this.tl_ProjectWBS_Completed.FieldName = "ProjectWBS";
		this.tl_ProjectWBS_Completed.Name = "tl_ProjectWBS_Completed";
		this.tl_ProjectWBS_Completed.OptionsColumn.AllowEdit = false;
		this.tl_ProjectWBS_Completed.Width = 103;
		this.tl_AssignTo_Completed.Caption = "AssignTo";
		this.tl_AssignTo_Completed.CurrencyDateField = null;
		this.tl_AssignTo_Completed.CurrencyField = null;
		this.tl_AssignTo_Completed.CurrencyFixed = null;
		this.tl_AssignTo_Completed.CurrencyFixedRate = 0.0;
		this.tl_AssignTo_Completed.CurrencyRateField = null;
		this.tl_AssignTo_Completed.FieldName = "AssignTo";
		this.tl_AssignTo_Completed.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.tl_AssignTo_Completed.Name = "tl_AssignTo_Completed";
		this.tl_AssignTo_Completed.OptionsColumn.AllowEdit = false;
		this.tl_AssignTo_Completed.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.String;
		this.tl_AssignTo_Completed.Width = 225;
		this.tl_DueDate_Completed.Caption = "Due Date";
		this.tl_DueDate_Completed.CurrencyDateField = null;
		this.tl_DueDate_Completed.CurrencyField = null;
		this.tl_DueDate_Completed.CurrencyFixed = null;
		this.tl_DueDate_Completed.CurrencyFixedRate = 0.0;
		this.tl_DueDate_Completed.CurrencyRateField = null;
		this.tl_DueDate_Completed.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.tl_DueDate_Completed.FieldName = "endDate";
		this.tl_DueDate_Completed.Name = "tl_DueDate_Completed";
		this.tl_DueDate_Completed.Width = 150;
		this.tl_ProjectEntry_Completed.Caption = "ProjectEntry";
		this.tl_ProjectEntry_Completed.CurrencyDateField = null;
		this.tl_ProjectEntry_Completed.CurrencyField = null;
		this.tl_ProjectEntry_Completed.CurrencyFixed = null;
		this.tl_ProjectEntry_Completed.CurrencyFixedRate = 0.0;
		this.tl_ProjectEntry_Completed.CurrencyRateField = null;
		this.tl_ProjectEntry_Completed.FieldName = "ProjectEntry";
		this.tl_ProjectEntry_Completed.Name = "tl_ProjectEntry_Completed";
		this.tl_OrderLine.Caption = "OrderLine";
		this.tl_OrderLine.CurrencyDateField = null;
		this.tl_OrderLine.CurrencyField = null;
		this.tl_OrderLine.CurrencyFixed = null;
		this.tl_OrderLine.CurrencyFixedRate = 0.0;
		this.tl_OrderLine.CurrencyRateField = null;
		this.tl_OrderLine.FieldName = "OrderLine";
		this.tl_OrderLine.Name = "tl_OrderLine";
		this.tl_CntctDate.Caption = "CntctDate";
		this.tl_CntctDate.CurrencyDateField = null;
		this.tl_CntctDate.CurrencyField = null;
		this.tl_CntctDate.CurrencyFixed = null;
		this.tl_CntctDate.CurrencyFixedRate = 0.0;
		this.tl_CntctDate.CurrencyRateField = null;
		this.tl_CntctDate.FieldName = "CntctDate";
		this.tl_CntctDate.Name = "tl_CntctDate";
		this.tl_ENDTime.Caption = "ENDTime";
		this.tl_ENDTime.CurrencyDateField = null;
		this.tl_ENDTime.CurrencyField = null;
		this.tl_ENDTime.CurrencyFixed = null;
		this.tl_ENDTime.CurrencyFixedRate = 0.0;
		this.tl_ENDTime.CurrencyRateField = null;
		this.tl_ENDTime.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Time;
		this.tl_ENDTime.FieldName = "ENDTime";
		this.tl_ENDTime.Name = "tl_ENDTime";
		this.tl_ENDTime.OptionsColumn.AllowEdit = false;
		this.tl_ENDTime.Width = 148;
		this.tl_CloseDate_Completed.Caption = "CloseDate";
		this.tl_CloseDate_Completed.CurrencyDateField = null;
		this.tl_CloseDate_Completed.CurrencyField = null;
		this.tl_CloseDate_Completed.CurrencyFixed = null;
		this.tl_CloseDate_Completed.CurrencyFixedRate = 0.0;
		this.tl_CloseDate_Completed.CurrencyRateField = null;
		this.tl_CloseDate_Completed.FieldName = "CloseDate";
		this.tl_CloseDate_Completed.Name = "tl_CloseDate_Completed";
		this.tl_CloseDate_Completed.OptionsColumn.AllowEdit = false;
		this.ctm_TaskList.ImageScalingSize = new System.Drawing.Size(32, 32);
		this.ctm_TaskList.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.toolStripMenuItemNewTask });
		this.ctm_TaskList.Name = "ctm_TaskList";
		this.ctm_TaskList.Size = new System.Drawing.Size(124, 26);
		this.toolStripMenuItemNewTask.Name = "toolStripMenuItemNewTask";
		this.toolStripMenuItemNewTask.Size = new System.Drawing.Size(123, 22);
		this.toolStripMenuItemNewTask.Text = "New Task";
		this.toolStripMenuItemNewTask.Click += new System.EventHandler(this.toolStripMenuItemNewTask_Click);
		this.rep_btn_Open.AutoHeight = false;
		editorButtonImageOptions40.Image = Apzon.Forms.Properties.Resources.incompleted;
		this.rep_btn_Open.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, editorButtonImageOptions40, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance65, appearanceHovered65, appearancePressed65, appearanceDisabled65, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_btn_Open.Name = "rep_btn_Open";
		this.rep_btn_Open.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
		this.rep_btn_0.AutoHeight = false;
		editorButtonImageOptions41.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions71.Image");
		this.rep_btn_0.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Ellipsis, "", -1, true, true, false, editorButtonImageOptions41, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance66, appearanceHovered66, appearancePressed66, appearanceDisabled66, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_btn_0.Name = "rep_btn_0";
		this.rep_btn_0.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
		this.rep_btn_1.AutoHeight = false;
		editorButtonImageOptions42.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions72.Image");
		this.rep_btn_1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Ellipsis, "", -1, true, true, false, editorButtonImageOptions42, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance67, appearanceHovered67, appearancePressed67, appearanceDisabled67, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_btn_1.Name = "rep_btn_1";
		this.rep_btn_1.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
		this.rep_btn_2.AutoHeight = false;
		editorButtonImageOptions43.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions73.Image");
		this.rep_btn_2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, editorButtonImageOptions43, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance68, appearanceHovered68, appearancePressed68, appearanceDisabled68, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_btn_2.Name = "rep_btn_2";
		this.rep_btn_2.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
		this.rep_btn_3.AutoHeight = false;
		editorButtonImageOptions44.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions74.Image");
		this.rep_btn_3.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, editorButtonImageOptions44, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance69, appearanceHovered69, appearancePressed69, appearanceDisabled69, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_btn_3.Name = "rep_btn_3";
		this.rep_btn_3.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
		this.xtra_NoteList.Controls.Add(this.grd_NoteList);
		this.xtra_NoteList.EditControl = null;
		this.xtra_NoteList.EditValue = "Note List";
		this.xtra_NoteList.FieldName = "";
		this.xtra_NoteList.IsSearchControl = false;
		this.xtra_NoteList.Margin = new System.Windows.Forms.Padding(2);
		this.xtra_NoteList.Name = "xtra_NoteList";
		this.xtra_NoteList.ReadOnly = false;
		this.xtra_NoteList.SettingInVisible = false;
		this.xtra_NoteList.SettingReadonly = false;
		this.xtra_NoteList.Size = new System.Drawing.Size(880, 208);
		this.xtra_NoteList.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtra_NoteList.Text = "Note List";
		this.grd_NoteList.AllowPasteExcel = false;
		this.grd_NoteList.ClipboardData = null;
		this.grd_NoteList.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_NoteList.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(2);
		this.grd_NoteList.Location = new System.Drawing.Point(0, 0);
		this.grd_NoteList.MainView = this.gv_NoteList;
		this.grd_NoteList.Margin = new System.Windows.Forms.Padding(2);
		this.grd_NoteList.Name = "grd_NoteList";
		this.grd_NoteList.Size = new System.Drawing.Size(880, 208);
		this.grd_NoteList.TabIndex = 0;
		this.grd_NoteList.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_NoteList });
		this.gv_NoteList.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_NoteList.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_NoteList.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_NoteList.ColumnKey = null;
		this.gv_NoteList.ColumnPanelRowHeight = 0;
		this.gv_NoteList.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[5] { this.cl_ClgCode, this.cl_Details, this.gc_AssignTo, this.NoteDate, this.NoteTime });
		this.gv_NoteList.DetailHeight = 364;
		this.gv_NoteList.DisableFormSetting = false;
		this.gv_NoteList.DisplayException = false;
		this.gv_NoteList.EditColumn = null;
		this.gv_NoteList.FieldCurrency = null;
		this.gv_NoteList.FieldRate = null;
		this.gv_NoteList.FooterPanelHeight = 0;
		this.gv_NoteList.FormId = "oProjectManagement";
		this.gv_NoteList.GridControl = this.grd_NoteList;
		this.gv_NoteList.GroupRowHeight = 0;
		this.gv_NoteList.HasFormSetting = true;
		this.gv_NoteList.IgnoreFilter = false;
		this.gv_NoteList.IndexEditRow = -1;
		this.gv_NoteList.IndicatorWidth = 35;
		this.gv_NoteList.IsApzTable = false;
		this.gv_NoteList.IsInsertNewRow = false;
		this.gv_NoteList.IsLoadingData = false;
		this.gv_NoteList.IsMappingContextAction = false;
		this.gv_NoteList.ItemId = "APZ_OCLG";
		this.gv_NoteList.LevelIndent = 0;
		this.gv_NoteList.LstColumnAllowBatch = null;
		this.gv_NoteList.LstColumnNotAllowBatch = null;
		this.gv_NoteList.LstColumnNotEditable = null;
		this.gv_NoteList.LstColumNotSetting = null;
		this.gv_NoteList.LstOldUDF = null;
		this.gv_NoteList.Name = "gv_NoteList";
		this.gv_NoteList.NewItemRowText = "New Note";
		this.gv_NoteList.OptionsBehavior.Editable = false;
		this.gv_NoteList.OptionsBehavior.ReadOnly = true;
		this.gv_NoteList.OptionsPrint.AutoWidth = false;
		this.gv_NoteList.OptionsPrint.UsePrintStyles = false;
		this.gv_NoteList.OptionsView.BestFitMaxRowCount = 100;
		this.gv_NoteList.OptionsView.BestFitMode = DevExpress.XtraGrid.Views.Grid.GridBestFitMode.Full;
		this.gv_NoteList.OptionsView.ColumnAutoWidth = false;
		this.gv_NoteList.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_NoteList.OptionsView.ShowGroupedColumns = true;
		this.gv_NoteList.OptionsView.ShowGroupPanel = false;
		this.gv_NoteList.PreviewIndent = 0;
		this.gv_NoteList.ReadOnly = true;
		this.gv_NoteList.RowHeight = 0;
		this.gv_NoteList.SettingReadonly = false;
		this.gv_NoteList.UseBatchMod = false;
		this.gv_NoteList.ViewCaptionHeight = 0;
		this.gv_NoteList.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_NoteList_RowCellStyle);
		this.gv_NoteList.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gv_NoteList_CustomUnboundColumnData);
		this.gv_NoteList.DoubleClick += new System.EventHandler(this.gv_NoteList_DoubleClick);
		this.cl_ClgCode.Caption = "ClgCode";
		this.cl_ClgCode.CurrencyDateField = null;
		this.cl_ClgCode.CurrencyField = null;
		this.cl_ClgCode.CurrencyFixed = null;
		this.cl_ClgCode.CurrencyFixedRate = 0.0;
		this.cl_ClgCode.CurrencyRateField = null;
		this.cl_ClgCode.FieldName = "ClgCode";
		this.cl_ClgCode.Name = "cl_ClgCode";
		this.cl_ClgCode.PrimaryKey = false;
		this.cl_ClgCode.UnitField = null;
		this.cl_Details.Caption = "Details";
		this.cl_Details.CurrencyDateField = null;
		this.cl_Details.CurrencyField = null;
		this.cl_Details.CurrencyFixed = null;
		this.cl_Details.CurrencyFixedRate = 0.0;
		this.cl_Details.CurrencyRateField = null;
		this.cl_Details.FieldName = "Details";
		this.cl_Details.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.cl_Details.Name = "cl_Details";
		this.cl_Details.PrimaryKey = false;
		this.cl_Details.UnitField = null;
		this.cl_Details.Width = 677;
		this.gc_AssignTo.CurrencyDateField = null;
		this.gc_AssignTo.CurrencyField = null;
		this.gc_AssignTo.CurrencyFixed = null;
		this.gc_AssignTo.CurrencyFixedRate = 0.0;
		this.gc_AssignTo.CurrencyRateField = null;
		this.gc_AssignTo.FieldName = "AssignTo";
		this.gc_AssignTo.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gc_AssignTo.Name = "gc_AssignTo";
		this.gc_AssignTo.PrimaryKey = false;
		this.gc_AssignTo.UnboundType = DevExpress.Data.UnboundColumnType.String;
		this.gc_AssignTo.UnitField = null;
		this.gc_AssignTo.Width = 250;
		this.NoteDate.Caption = "Note Date";
		this.NoteDate.CurrencyDateField = null;
		this.NoteDate.CurrencyField = null;
		this.NoteDate.CurrencyFixed = null;
		this.NoteDate.CurrencyFixedRate = 0.0;
		this.NoteDate.CurrencyRateField = null;
		this.NoteDate.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.NoteDate.FieldName = "Recontact";
		this.NoteDate.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.NoteDate.Name = "NoteDate";
		this.NoteDate.PrimaryKey = false;
		this.NoteDate.UnitField = null;
		this.NoteTime.Caption = "Note Time";
		this.NoteTime.CurrencyDateField = null;
		this.NoteTime.CurrencyField = null;
		this.NoteTime.CurrencyFixed = null;
		this.NoteTime.CurrencyFixedRate = 0.0;
		this.NoteTime.CurrencyRateField = null;
		this.NoteTime.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Time;
		this.NoteTime.FieldName = "BeginTime";
		this.NoteTime.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.NoteTime.Name = "NoteTime";
		this.NoteTime.PrimaryKey = false;
		this.NoteTime.UnitField = null;
		this.xtra_EventList.Controls.Add(this.grd_EventList);
		this.xtra_EventList.EditControl = null;
		this.xtra_EventList.EditValue = "Event List";
		this.xtra_EventList.FieldName = "";
		this.xtra_EventList.IsSearchControl = false;
		this.xtra_EventList.Margin = new System.Windows.Forms.Padding(2);
		this.xtra_EventList.Name = "xtra_EventList";
		this.xtra_EventList.ReadOnly = false;
		this.xtra_EventList.SettingInVisible = false;
		this.xtra_EventList.SettingReadonly = false;
		this.xtra_EventList.Size = new System.Drawing.Size(880, 208);
		this.xtra_EventList.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtra_EventList.Text = "Event List";
		this.grd_EventList.AllowPasteExcel = false;
		this.grd_EventList.ClipboardData = null;
		this.grd_EventList.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_EventList.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(2);
		this.grd_EventList.Location = new System.Drawing.Point(0, 0);
		this.grd_EventList.MainView = this.gv_EventList;
		this.grd_EventList.Margin = new System.Windows.Forms.Padding(2);
		this.grd_EventList.Name = "grd_EventList";
		this.grd_EventList.Size = new System.Drawing.Size(880, 208);
		this.grd_EventList.TabIndex = 0;
		this.grd_EventList.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_EventList });
		this.gv_EventList.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_EventList.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_EventList.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_EventList.ColumnKey = null;
		this.gv_EventList.ColumnPanelRowHeight = 0;
		this.gv_EventList.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[11]
		{
			this.ClgCode, this.Details, this.CardCode, this.Recontact, this.endDate, this.AllDayEvent, this.AssignTo, this.CntctDate, this.BeginTime, this.ENDTime,
			this.gridColumnEx103
		});
		this.gv_EventList.DetailHeight = 364;
		this.gv_EventList.DisableFormSetting = false;
		this.gv_EventList.DisplayException = false;
		this.gv_EventList.EditColumn = null;
		this.gv_EventList.FieldCurrency = null;
		this.gv_EventList.FieldRate = null;
		this.gv_EventList.FooterPanelHeight = 0;
		this.gv_EventList.FormId = "oProjectManagement";
		this.gv_EventList.GridControl = this.grd_EventList;
		this.gv_EventList.GroupRowHeight = 0;
		this.gv_EventList.HasFormSetting = true;
		this.gv_EventList.IgnoreFilter = false;
		this.gv_EventList.IndexEditRow = -1;
		this.gv_EventList.IndicatorWidth = 35;
		this.gv_EventList.IsApzTable = false;
		this.gv_EventList.IsInsertNewRow = false;
		this.gv_EventList.IsLoadingData = false;
		this.gv_EventList.IsMappingContextAction = false;
		this.gv_EventList.ItemId = "APZ_OCLG";
		this.gv_EventList.LevelIndent = 0;
		this.gv_EventList.LstColumnAllowBatch = null;
		this.gv_EventList.LstColumnNotAllowBatch = null;
		this.gv_EventList.LstColumnNotEditable = null;
		this.gv_EventList.LstColumNotSetting = null;
		this.gv_EventList.LstOldUDF = null;
		this.gv_EventList.Name = "gv_EventList";
		this.gv_EventList.NewItemRowText = "New Event";
		this.gv_EventList.OptionsBehavior.Editable = false;
		this.gv_EventList.OptionsBehavior.ReadOnly = true;
		this.gv_EventList.OptionsPrint.AutoWidth = false;
		this.gv_EventList.OptionsPrint.UsePrintStyles = false;
		this.gv_EventList.OptionsView.BestFitMaxRowCount = 100;
		this.gv_EventList.OptionsView.BestFitMode = DevExpress.XtraGrid.Views.Grid.GridBestFitMode.Full;
		this.gv_EventList.OptionsView.ColumnAutoWidth = false;
		this.gv_EventList.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_EventList.OptionsView.ShowGroupedColumns = true;
		this.gv_EventList.OptionsView.ShowGroupPanel = false;
		this.gv_EventList.PreviewIndent = 0;
		this.gv_EventList.ReadOnly = true;
		this.gv_EventList.RowHeight = 0;
		this.gv_EventList.SettingReadonly = false;
		this.gv_EventList.UseBatchMod = false;
		this.gv_EventList.ViewCaptionHeight = 0;
		this.gv_EventList.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_EventList_RowCellStyle);
		this.gv_EventList.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gv_EventList_CustomUnboundColumnData);
		this.gv_EventList.DoubleClick += new System.EventHandler(this.gv_EventList_DoubleClick);
		this.ClgCode.Caption = "ClgCode";
		this.ClgCode.CurrencyDateField = null;
		this.ClgCode.CurrencyField = null;
		this.ClgCode.CurrencyFixed = null;
		this.ClgCode.CurrencyFixedRate = 0.0;
		this.ClgCode.CurrencyRateField = null;
		this.ClgCode.FieldName = "ClgCode";
		this.ClgCode.Name = "ClgCode";
		this.ClgCode.PrimaryKey = false;
		this.ClgCode.UnitField = null;
		this.Details.Caption = "Details";
		this.Details.CurrencyDateField = null;
		this.Details.CurrencyField = null;
		this.Details.CurrencyFixed = null;
		this.Details.CurrencyFixedRate = 0.0;
		this.Details.CurrencyRateField = null;
		this.Details.FieldName = "Details";
		this.Details.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.Details.Name = "Details";
		this.Details.PrimaryKey = false;
		this.Details.UnitField = null;
		this.Details.Width = 600;
		this.CardCode.Caption = "BP Code";
		this.CardCode.CurrencyDateField = null;
		this.CardCode.CurrencyField = null;
		this.CardCode.CurrencyFixed = null;
		this.CardCode.CurrencyFixedRate = 0.0;
		this.CardCode.CurrencyRateField = null;
		this.CardCode.FieldName = "CardCode";
		this.CardCode.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.CardCode.Name = "CardCode";
		this.CardCode.PrimaryKey = false;
		this.CardCode.UnitField = null;
		this.CardCode.Width = 100;
		this.Recontact.Caption = "From Date";
		this.Recontact.CurrencyDateField = null;
		this.Recontact.CurrencyField = null;
		this.Recontact.CurrencyFixed = null;
		this.Recontact.CurrencyFixedRate = 0.0;
		this.Recontact.CurrencyRateField = null;
		this.Recontact.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.Recontact.FieldName = "Recontact";
		this.Recontact.Name = "Recontact";
		this.Recontact.PrimaryKey = false;
		this.Recontact.UnitField = null;
		this.endDate.Caption = "To Date";
		this.endDate.CurrencyDateField = null;
		this.endDate.CurrencyField = null;
		this.endDate.CurrencyFixed = null;
		this.endDate.CurrencyFixedRate = 0.0;
		this.endDate.CurrencyRateField = null;
		this.endDate.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.endDate.FieldName = "endDate";
		this.endDate.Name = "endDate";
		this.endDate.PrimaryKey = false;
		this.endDate.UnitField = null;
		this.AllDayEvent.Caption = "All Day";
		this.AllDayEvent.CurrencyDateField = null;
		this.AllDayEvent.CurrencyField = null;
		this.AllDayEvent.CurrencyFixed = null;
		this.AllDayEvent.CurrencyFixedRate = 0.0;
		this.AllDayEvent.CurrencyRateField = null;
		this.AllDayEvent.FieldName = "AllDayEvent";
		this.AllDayEvent.Name = "AllDayEvent";
		this.AllDayEvent.PrimaryKey = false;
		this.AllDayEvent.UnitField = null;
		this.AssignTo.Caption = "Invite";
		this.AssignTo.CurrencyDateField = null;
		this.AssignTo.CurrencyField = null;
		this.AssignTo.CurrencyFixed = null;
		this.AssignTo.CurrencyFixedRate = 0.0;
		this.AssignTo.CurrencyRateField = null;
		this.AssignTo.FieldName = "AssignTo";
		this.AssignTo.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.AssignTo.Name = "AssignTo";
		this.AssignTo.PrimaryKey = false;
		this.AssignTo.UnboundType = DevExpress.Data.UnboundColumnType.String;
		this.AssignTo.UnitField = null;
		this.AssignTo.Width = 200;
		this.CntctDate.Caption = "Create Date";
		this.CntctDate.CurrencyDateField = null;
		this.CntctDate.CurrencyField = null;
		this.CntctDate.CurrencyFixed = null;
		this.CntctDate.CurrencyFixedRate = 0.0;
		this.CntctDate.CurrencyRateField = null;
		this.CntctDate.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.CntctDate.FieldName = "CntctDate";
		this.CntctDate.Name = "CntctDate";
		this.CntctDate.PrimaryKey = false;
		this.CntctDate.UnitField = null;
		this.BeginTime.Caption = "Begin Time";
		this.BeginTime.CurrencyDateField = null;
		this.BeginTime.CurrencyField = null;
		this.BeginTime.CurrencyFixed = null;
		this.BeginTime.CurrencyFixedRate = 0.0;
		this.BeginTime.CurrencyRateField = null;
		this.BeginTime.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.BeginTime.FieldName = "BeginTime";
		this.BeginTime.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.BeginTime.Name = "BeginTime";
		this.BeginTime.PrimaryKey = false;
		this.BeginTime.UnitField = null;
		this.ENDTime.Caption = "End Time";
		this.ENDTime.CurrencyDateField = null;
		this.ENDTime.CurrencyField = null;
		this.ENDTime.CurrencyFixed = null;
		this.ENDTime.CurrencyFixedRate = 0.0;
		this.ENDTime.CurrencyRateField = null;
		this.ENDTime.FieldName = "ENDTime";
		this.ENDTime.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.ENDTime.Name = "ENDTime";
		this.ENDTime.PrimaryKey = false;
		this.ENDTime.UnitField = null;
		this.gridColumnEx103.Caption = "Create Time";
		this.gridColumnEx103.CurrencyDateField = null;
		this.gridColumnEx103.CurrencyField = null;
		this.gridColumnEx103.CurrencyFixed = null;
		this.gridColumnEx103.CurrencyFixedRate = 0.0;
		this.gridColumnEx103.CurrencyRateField = null;
		this.gridColumnEx103.FieldName = "CntctTime";
		this.gridColumnEx103.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx103.Name = "gridColumnEx103";
		this.gridColumnEx103.PrimaryKey = false;
		this.gridColumnEx103.UnitField = null;
		this.ctm_Sub.ImageScalingSize = new System.Drawing.Size(32, 32);
		this.ctm_Sub.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.toolStripMenuItemAddSub });
		this.ctm_Sub.Name = "contextMenuStrip1";
		this.ctm_Sub.Size = new System.Drawing.Size(202, 26);
		this.ctm_Sub.Text = "Add";
		this.toolStripMenuItemAddSub.Name = "toolStripMenuItemAddSub";
		this.toolStripMenuItemAddSub.Size = new System.Drawing.Size(201, 22);
		this.toolStripMenuItemAddSub.Text = "Define New Sub. Project";
		this.toolStripMenuItemAddSub.Click += new System.EventHandler(this.toolStripMenuItemAddSub_Click);
		this.btn_Ok.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Ok.EditControl = null;
		this.btn_Ok.EditValue = "OK";
		this.btn_Ok.FieldName = null;
		this.btn_Ok.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Ok.ImageOptions.Image");
		this.btn_Ok.IsSearchControl = false;
		this.btn_Ok.Location = new System.Drawing.Point(12, 536);
		this.btn_Ok.Margin = new System.Windows.Forms.Padding(2);
		this.btn_Ok.Name = "btn_Ok";
		this.btn_Ok.ReadOnly = false;
		this.btn_Ok.SettingInVisible = false;
		this.btn_Ok.SettingReadonly = false;
		this.btn_Ok.Size = new System.Drawing.Size(75, 24);
		this.btn_Ok.TabIndex = 1;
		this.btn_Ok.Text = "OK";
		this.btn_Ok.Click += new System.EventHandler(this.btn_Ok_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(92, 536);
		this.btn_Cancel.Margin = new System.Windows.Forms.Padding(2);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 24);
		this.btn_Cancel.TabIndex = 2;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Project Code";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(2, 6);
		this.labelLine1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(98, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 3;
		this.labelLine1.Utext = "Project Code";
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "Status";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(208, 6);
		this.labelLine2.Margin = new System.Windows.Forms.Padding(2);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(92, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 4;
		this.labelLine2.Utext = "Status";
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "Bp Code";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(421, 31);
		this.labelLine3.Margin = new System.Windows.Forms.Padding(2);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(65, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 5;
		this.labelLine3.Utext = "Bp Code";
		this.txt_Code.EditControl = null;
		this.txt_Code.FieldName = null;
		this.txt_Code.IsSearchControl = true;
		this.txt_Code.Location = new System.Drawing.Point(99, 3);
		this.txt_Code.Margin = new System.Windows.Forms.Padding(2);
		this.txt_Code.Name = "txt_Code";
		this.txt_Code.SettingInVisible = false;
		this.txt_Code.SettingReadonly = false;
		this.txt_Code.Size = new System.Drawing.Size(100, 20);
		this.txt_Code.TabIndex = 6;
		this.luk_Sta.EditControl = null;
		this.luk_Sta.FieldName = null;
		this.luk_Sta.IsSearchControl = true;
		this.luk_Sta.Location = new System.Drawing.Point(306, 2);
		this.luk_Sta.Margin = new System.Windows.Forms.Padding(2);
		this.luk_Sta.Name = "luk_Sta";
		this.luk_Sta.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Sta.Properties.NullText = "";
		this.luk_Sta.SettingInVisible = false;
		this.luk_Sta.SettingReadonly = false;
		this.luk_Sta.Size = new System.Drawing.Size(100, 20);
		this.luk_Sta.TabIndex = 7;
		this.btn_Bp.EditControl = null;
		this.btn_Bp.FieldName = null;
		this.btn_Bp.IsSearchControl = true;
		this.btn_Bp.Location = new System.Drawing.Point(490, 29);
		this.btn_Bp.Margin = new System.Windows.Forms.Padding(2);
		this.btn_Bp.Name = "btn_Bp";
		this.btn_Bp.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions45.Image = Apzon.Forms.Properties.Resources.right;
		this.btn_Bp.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions26, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance70, appearanceHovered70, appearancePressed70, appearanceDisabled70, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions45, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance71, appearanceHovered71, appearancePressed71, appearanceDisabled71, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_Bp.Properties.ContainerControl = null;
		this.btn_Bp.Properties.DisplayMember = "CardCode";
		this.btn_Bp.Properties.DisplayMember2 = "";
		this.btn_Bp.Properties.ListMandatoryField = null;
		this.btn_Bp.Properties.LoadDataSourceWhenEnter = false;
		this.btn_Bp.Properties.SeparateCharacterForMulti = ';';
		this.btn_Bp.Properties.SeparateString = ", ";
		this.btn_Bp.Properties.UsingAutoCompleteBox = false;
		this.btn_Bp.Properties.UsingDataSource = true;
		this.btn_Bp.Properties.ValidateWhenNoDataSource = false;
		this.btn_Bp.Properties.ValueMember = "CardCode";
		this.btn_Bp.SettingInVisible = false;
		this.btn_Bp.SettingReadonly = false;
		this.btn_Bp.Size = new System.Drawing.Size(84, 20);
		this.btn_Bp.TabIndex = 8;
		this.btn_Search.EditControl = null;
		this.btn_Search.EditValue = "Search";
		this.btn_Search.FieldName = null;
		this.btn_Search.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Search.ImageOptions.Image");
		this.btn_Search.IsSearchControl = true;
		this.btn_Search.Location = new System.Drawing.Point(585, 24);
		this.btn_Search.Margin = new System.Windows.Forms.Padding(2);
		this.btn_Search.Name = "btn_Search";
		this.btn_Search.ReadOnly = false;
		this.btn_Search.SettingInVisible = false;
		this.btn_Search.SettingReadonly = false;
		this.btn_Search.Size = new System.Drawing.Size(75, 24);
		this.btn_Search.TabIndex = 9;
		this.btn_Search.Text = "Search";
		this.btn_Search.Click += new System.EventHandler(this.btn_Search_Click);
		this.btn_ShowGantt.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_ShowGantt.EditControl = null;
		this.btn_ShowGantt.EditValue = "Show Gantt Chart";
		this.btn_ShowGantt.FieldName = null;
		this.btn_ShowGantt.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_ShowGantt.ImageOptions.Image");
		this.btn_ShowGantt.IsSearchControl = true;
		this.btn_ShowGantt.Location = new System.Drawing.Point(1001, 562);
		this.btn_ShowGantt.Margin = new System.Windows.Forms.Padding(2);
		this.btn_ShowGantt.Name = "btn_ShowGantt";
		this.btn_ShowGantt.ReadOnly = false;
		this.btn_ShowGantt.SettingInVisible = false;
		this.btn_ShowGantt.SettingReadonly = false;
		this.btn_ShowGantt.Size = new System.Drawing.Size(123, 24);
		this.btn_ShowGantt.TabIndex = 10;
		this.btn_ShowGantt.Text = "Show Gantt Chart";
		this.btn_ShowGantt.Click += new System.EventHandler(this.btn_ShowGantt_Click);
		this.toolTipController1.KeepWhileHovered = true;
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "Start Date From";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(2, 31);
		this.labelLine4.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(98, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 11;
		this.labelLine4.Utext = "Start Date From";
		this.labelLine5.EditControl = null;
		this.labelLine5.EditValue = "Start Date To";
		this.labelLine5.FieldName = null;
		this.labelLine5.HideWhenEmpty = false;
		this.labelLine5.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine5.Location = new System.Drawing.Point(208, 30);
		this.labelLine5.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine5.Name = "labelLine5";
		this.labelLine5.ReadOnly = false;
		this.labelLine5.SettingInVisible = false;
		this.labelLine5.SettingReadonly = false;
		this.labelLine5.ShowUnderLine = true;
		this.labelLine5.Size = new System.Drawing.Size(98, 20);
		this.labelLine5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine5.TabIndex = 12;
		this.labelLine5.Utext = "Start Date To";
		this.date_StartTo.EditControl = null;
		this.date_StartTo.EditValue = null;
		this.date_StartTo.FieldName = null;
		this.date_StartTo.Location = new System.Drawing.Point(306, 28);
		this.date_StartTo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.date_StartTo.Name = "date_StartTo";
		this.date_StartTo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_StartTo.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_StartTo.Properties.IsSearchControl = true;
		this.date_StartTo.SettingInVisible = false;
		this.date_StartTo.SettingReadonly = false;
		this.date_StartTo.Size = new System.Drawing.Size(100, 20);
		this.date_StartTo.TabIndex = 13;
		this.date_StartFrom.EditControl = null;
		this.date_StartFrom.EditValue = null;
		this.date_StartFrom.FieldName = null;
		this.date_StartFrom.Location = new System.Drawing.Point(99, 28);
		this.date_StartFrom.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.date_StartFrom.Name = "date_StartFrom";
		this.date_StartFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_StartFrom.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_StartFrom.Properties.IsSearchControl = true;
		this.date_StartFrom.SettingInVisible = false;
		this.date_StartFrom.SettingReadonly = false;
		this.date_StartFrom.Size = new System.Drawing.Size(100, 20);
		this.date_StartFrom.TabIndex = 14;
		this.drp_CrDocumnet.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.drp_CrDocumnet.Location = new System.Drawing.Point(715, 536);
		this.drp_CrDocumnet.Margin = new System.Windows.Forms.Padding(2);
		this.drp_CrDocumnet.Name = "drp_CrDocumnet";
		this.drp_CrDocumnet.Size = new System.Drawing.Size(121, 24);
		this.drp_CrDocumnet.TabIndex = 15;
		this.drp_CrDocumnet.Text = "Create Document";
		this.drp_CrDocumnet.Visible = false;
		this.btn_Select.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Select.EditControl = null;
		this.btn_Select.EditValue = "Select All";
		this.btn_Select.FieldName = null;
		this.btn_Select.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Select.ImageOptions.Image");
		this.btn_Select.IsSearchControl = false;
		this.btn_Select.Location = new System.Drawing.Point(581, 537);
		this.btn_Select.Margin = new System.Windows.Forms.Padding(2);
		this.btn_Select.Name = "btn_Select";
		this.btn_Select.ReadOnly = false;
		this.btn_Select.SettingInVisible = false;
		this.btn_Select.SettingReadonly = false;
		this.btn_Select.Size = new System.Drawing.Size(106, 24);
		this.btn_Select.TabIndex = 16;
		this.btn_Select.Text = "Select All";
		this.btn_Select.Visible = false;
		this.btn_Select.Click += new System.EventHandler(this.btn_Select_Click);
		this.txt_No.EditControl = null;
		this.txt_No.FieldName = null;
		this.txt_No.IsSearchControl = true;
		this.txt_No.Location = new System.Drawing.Point(490, 5);
		this.txt_No.Margin = new System.Windows.Forms.Padding(2);
		this.txt_No.Name = "txt_No";
		this.txt_No.SettingInVisible = false;
		this.txt_No.SettingReadonly = false;
		this.txt_No.Size = new System.Drawing.Size(85, 20);
		this.txt_No.TabIndex = 17;
		this.labelLine6.EditControl = null;
		this.labelLine6.EditValue = "Doc. No";
		this.labelLine6.FieldName = null;
		this.labelLine6.HideWhenEmpty = false;
		this.labelLine6.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine6.Location = new System.Drawing.Point(421, 6);
		this.labelLine6.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine6.Name = "labelLine6";
		this.labelLine6.ReadOnly = false;
		this.labelLine6.SettingInVisible = false;
		this.labelLine6.SettingReadonly = false;
		this.labelLine6.ShowUnderLine = true;
		this.labelLine6.Size = new System.Drawing.Size(64, 20);
		this.labelLine6.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine6.TabIndex = 18;
		this.labelLine6.Utext = "Doc. No";
		this.btn_OwnerSearch.EditControl = null;
		this.btn_OwnerSearch.FieldName = null;
		this.btn_OwnerSearch.IsSearchControl = true;
		this.btn_OwnerSearch.Location = new System.Drawing.Point(717, 26);
		this.btn_OwnerSearch.Margin = new System.Windows.Forms.Padding(2);
		this.btn_OwnerSearch.Name = "btn_OwnerSearch";
		this.btn_OwnerSearch.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions46.Image = Apzon.Forms.Properties.Resources.right;
		this.btn_OwnerSearch.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions27, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance72, appearanceHovered72, appearancePressed72, appearanceDisabled72, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions46, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance73, appearanceHovered73, appearancePressed73, appearanceDisabled73, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_OwnerSearch.Properties.ContainerControl = null;
		this.btn_OwnerSearch.Properties.DisplayMember = "CardCode";
		this.btn_OwnerSearch.Properties.DisplayMember2 = "";
		this.btn_OwnerSearch.Properties.ListMandatoryField = null;
		this.btn_OwnerSearch.Properties.LoadDataSourceWhenEnter = false;
		this.btn_OwnerSearch.Properties.SeparateCharacterForMulti = ';';
		this.btn_OwnerSearch.Properties.SeparateString = ", ";
		this.btn_OwnerSearch.Properties.UsingAutoCompleteBox = false;
		this.btn_OwnerSearch.Properties.UsingDataSource = true;
		this.btn_OwnerSearch.Properties.ValidateWhenNoDataSource = false;
		this.btn_OwnerSearch.Properties.ValueMember = "CardCode";
		this.btn_OwnerSearch.SettingInVisible = false;
		this.btn_OwnerSearch.SettingReadonly = false;
		this.btn_OwnerSearch.Size = new System.Drawing.Size(71, 20);
		this.btn_OwnerSearch.TabIndex = 20;
		this.btn_OwnerSearch.Visible = false;
		this.labelLine7.EditControl = null;
		this.labelLine7.EditValue = "Owner";
		this.labelLine7.FieldName = null;
		this.labelLine7.HideWhenEmpty = false;
		this.labelLine7.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine7.Location = new System.Drawing.Point(665, 28);
		this.labelLine7.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine7.Name = "labelLine7";
		this.labelLine7.ReadOnly = false;
		this.labelLine7.SettingInVisible = false;
		this.labelLine7.SettingReadonly = false;
		this.labelLine7.ShowUnderLine = true;
		this.labelLine7.Size = new System.Drawing.Size(47, 20);
		this.labelLine7.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine7.TabIndex = 19;
		this.labelLine7.Utext = "Owner";
		this.labelLine7.Visible = false;
		this.btn_GanttView.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_GanttView.EditControl = null;
		this.btn_GanttView.EditValue = "Gantt view";
		this.btn_GanttView.FieldName = null;
		this.btn_GanttView.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_GanttView.ImageOptions.Image");
		this.btn_GanttView.IsSearchControl = true;
		this.btn_GanttView.Location = new System.Drawing.Point(475, 537);
		this.btn_GanttView.Margin = new System.Windows.Forms.Padding(2);
		this.btn_GanttView.Name = "btn_GanttView";
		this.btn_GanttView.ReadOnly = false;
		this.btn_GanttView.SettingInVisible = false;
		this.btn_GanttView.SettingReadonly = false;
		this.btn_GanttView.Size = new System.Drawing.Size(102, 24);
		this.btn_GanttView.TabIndex = 21;
		this.btn_GanttView.Text = "Gantt view";
		this.btn_GanttView.Visible = false;
		this.btn_GanttView.Click += new System.EventHandler(this.btn_GanttView_Click);
		this.btn_Approval.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Approval.EditControl = null;
		this.btn_Approval.EditValue = "Approval";
		this.btn_Approval.FieldName = null;
		this.btn_Approval.IsSearchControl = false;
		this.btn_Approval.Location = new System.Drawing.Point(193, 537);
		this.btn_Approval.Margin = new System.Windows.Forms.Padding(2);
		this.btn_Approval.Name = "btn_Approval";
		this.btn_Approval.ReadOnly = false;
		this.btn_Approval.SettingInVisible = false;
		this.btn_Approval.SettingReadonly = false;
		this.btn_Approval.Size = new System.Drawing.Size(75, 24);
		this.btn_Approval.TabIndex = 22;
		this.btn_Approval.Text = "Approval";
		this.btn_Approval.Visible = false;
		this.btn_Approval.Click += new System.EventHandler(this.btn_Approval_Click);
		this.btn_Reject.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Reject.EditControl = null;
		this.btn_Reject.EditValue = "Reject";
		this.btn_Reject.FieldName = null;
		this.btn_Reject.IsSearchControl = false;
		this.btn_Reject.Location = new System.Drawing.Point(274, 537);
		this.btn_Reject.Margin = new System.Windows.Forms.Padding(2);
		this.btn_Reject.Name = "btn_Reject";
		this.btn_Reject.ReadOnly = false;
		this.btn_Reject.SettingInVisible = false;
		this.btn_Reject.SettingReadonly = false;
		this.btn_Reject.Size = new System.Drawing.Size(75, 24);
		this.btn_Reject.TabIndex = 23;
		this.btn_Reject.Text = "Reject";
		this.btn_Reject.Visible = false;
		this.btn_Reject.Click += new System.EventHandler(this.btn_Reject_Click);
		this.btn_UndoApproval.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_UndoApproval.EditControl = null;
		this.btn_UndoApproval.EditValue = "Thu hồi phê duyệt";
		this.btn_UndoApproval.FieldName = null;
		this.btn_UndoApproval.IsSearchControl = false;
		this.btn_UndoApproval.Location = new System.Drawing.Point(353, 536);
		this.btn_UndoApproval.Margin = new System.Windows.Forms.Padding(2);
		this.btn_UndoApproval.Name = "btn_UndoApproval";
		this.btn_UndoApproval.ReadOnly = false;
		this.btn_UndoApproval.SettingInVisible = false;
		this.btn_UndoApproval.SettingReadonly = false;
		this.btn_UndoApproval.Size = new System.Drawing.Size(97, 24);
		this.btn_UndoApproval.TabIndex = 24;
		this.btn_UndoApproval.Text = "Thu hồi phê duyệt";
		this.btn_UndoApproval.Visible = false;
		this.btn_UndoApproval.Click += new System.EventHandler(this.btn_UndoApproval_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(96f, 96f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
		base.CancelButton = this.btn_Cancel;
		base.ClientSize = new System.Drawing.Size(898, 568);
		base.Controls.Add(this.btn_UndoApproval);
		base.Controls.Add(this.btn_Reject);
		base.Controls.Add(this.btn_Approval);
		base.Controls.Add(this.btn_GanttView);
		base.Controls.Add(this.btn_OwnerSearch);
		base.Controls.Add(this.labelLine7);
		base.Controls.Add(this.labelLine6);
		base.Controls.Add(this.txt_No);
		base.Controls.Add(this.btn_Select);
		base.Controls.Add(this.drp_CrDocumnet);
		base.Controls.Add(this.date_StartFrom);
		base.Controls.Add(this.date_StartTo);
		base.Controls.Add(this.labelLine5);
		base.Controls.Add(this.labelLine4);
		base.Controls.Add(this.btn_ShowGantt);
		base.Controls.Add(this.btn_Search);
		base.Controls.Add(this.btn_Bp);
		base.Controls.Add(this.luk_Sta);
		base.Controls.Add(this.txt_Code);
		base.Controls.Add(this.labelLine3);
		base.Controls.Add(this.labelLine2);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_Ok);
		base.Controls.Add(this.splitContainerControl1);
		base.FormId = "oProjectManagement";
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_ProjectManager.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Margin = new System.Windows.Forms.Padding(2);
		base.Name = "frm_ProjectManager";
		this.Text = "Project Manager";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Frm_ProjectManager_FormClosing);
		base.Shown += new System.EventHandler(this.frm_ProjectManager_Shown);
		((System.ComponentModel.ISupportInitialize)this.luk_Priority).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_taskPriority).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_SubPriority).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_priorityGrid).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_prioritylistcompleted).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx6).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx7).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx8).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		this.ctm_Doc.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_prjCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Type).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Status).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_BpCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_Manager).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_Project).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Industry).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_Owner).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_SaleOder).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_FinProject).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_budgetLevel).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_Whs).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_DocCurr).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx1.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTimeSpanEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx2.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTimeSpanEdit2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_WddStatus).EndInit();
		((System.ComponentModel.ISupportInitialize)this.look_LoadBom).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CommentsCount).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_BaseProject).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_BaseLine).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_BaseLineActive).EndInit();
		((System.ComponentModel.ISupportInitialize)this.xtra_Line).EndInit();
		this.xtra_Line.ResumeLayout(false);
		this.tab_Task.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl4).EndInit();
		this.splitContainerControl4.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.tv_Wbs).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_NewTask).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_TaskCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_fromdate.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_fromdate).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_todate.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_todate).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Milestone).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_taskStatus).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_resource).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_WBSTemplate).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_WBSMode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_WbsComments).EndInit();
		this.ctm_Rows.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl5).EndInit();
		this.splitContainerControl5.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.schedulerControl1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.schedulerStorage1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Action).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Users).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dateNavigator1.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dateNavigator1).EndInit();
		this.xtra_Team.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_LineTeam).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_LineTeam).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_ResourceEmployee).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_branch).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_dept).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_role).EndInit();
		this.xtra_Budget.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.xtra_LineBudget).EndInit();
		this.xtra_LineBudget.ResumeLayout(false);
		this.xtra_LineItem.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.tv_budget).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_ItemCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_TaskCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cal_PlanQty).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_RefPrice).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_ItemBranch).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dat_IFromDate.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dat_IFromDate).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dat_IToDate.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dat_IToDate).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_BOM).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_OrderItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_PR).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_PO).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_GRPO).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_Delivery).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_APInvoice).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_ARInvoice).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_ItmsGrpCod).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_WarehouseItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Currency).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_PQ).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_IGE).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_WOR).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_BudgetComments).EndInit();
		this.ctm_BudgetItem.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.btn_AbsEntry).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_UoM).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_ItemGroup).EndInit();
		this.xtra_Payment.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.tv_Payments).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_WbsPayment).EndInit();
		this.ctm_Payments.ResumeLayout(false);
		this.xtra_Document.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_LineDocument).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_LineDocument).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_DocNumDo).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_CurrencyLineDu).EndInit();
		this.xtra_Sub.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_LineSub).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_LineSub).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_Code).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_SubType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_SubStatus).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_SubBpCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_SubPM).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_SubMasterProject).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemLookUpEditCombo1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_SubOwner).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_SubSalesOrder).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_SubFinalProject).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEdit2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_SubWhs).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_SubCurrency).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx3.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTimeSpanEdit3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx4.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTimeSpanEdit4).EndInit();
		this.xtra_Issue.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_lineissue).EndInit();
		this.ctm_Issues.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_lineIssue).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_Id).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_TypeGrid).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_statusGrid).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_report).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_handle).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_solution).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_IssueBp).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_IssueSLBy).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_IssuwPrOwner).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_IssuePr).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_IssueTaskCode).EndInit();
		this.tab_TaskList.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.tv_ListCompleted).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_btn_Closed).EndInit();
		this.ctm_TaskList.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.rep_btn_Open).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_btn_0).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_btn_1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_btn_2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_btn_3).EndInit();
		this.xtra_NoteList.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_NoteList).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_NoteList).EndInit();
		this.xtra_EventList.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_EventList).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_EventList).EndInit();
		this.ctm_Sub.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txt_Code.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Sta.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_Bp.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_StartTo.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_StartTo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_StartFrom.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_StartFrom.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_No.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_OwnerSearch.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
