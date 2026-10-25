using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Dynamic;
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
using Apzon.Forms.Commons;
using Apzon.Forms.Helper;
using Apzon.Forms.Properties;
using Apzon.Forms.UserControls;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Container;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraTab;
using Facebook;

namespace Apzon.Forms.Facebook_CRM;

public class frm_FacebookCRM : XtraFormEx
{
	private enum FormLoginMode
	{
		Login = 1,
		Access = 2,
		Logout = 0
	}

	private const string AppId = "669236043274789";

	private const string ExtendedPermissions = "public_profile,manage_pages,read_page_mailboxes,publish_pages";

	private string _accessToken = "";

	private string _userID = "";

	private string _pageId = "";

	private string pageToken = "";

	private List<string> listCustomerFilter = new List<string>();

	private List<string> listCustomerBP = new List<string>();

	private List<string> listDocEntry = new List<string>();

	private List<string> bpCodes = new List<string>();

	private List<string> itemCodes = new List<string>();

	private List<string> orderStatus = new List<string>();

	private List<string> tags = new List<string>();

	private DateTime dateFrom = DateTime.MinValue;

	private DateTime dateTo = DateTime.MaxValue;

	private Stopwatch watch = new Stopwatch();

	private bool _isLoading;

	private bool loadCustomer;

	private bool loadMessage;

	private bool customerCommentUpdate;

	private bool customerMessageUpdate;

	private bool customerUpdate;

	private bool isFiltering;

	private bool isCancel;

	private bool hideComment;

	private bool changeCustomerRow;

	private bool saveBeforeClose;

	private bool refreshTagData;

	private string FormController = "";

	protected FacebookClient _fb = new FacebookClient();

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private Tuple<GridViewEx, string> newTuple;

	private DataTable dtAPZ_OCRMFilter = new DataTable();

	private DataTable dtAPZ_CRMCFilter = new DataTable();

	private DataTable dtAPZ_OCRM = new DataTable();

	private DataTable dtAPZ_CRMU = new DataTable();

	private DataTable dtAPZ_CRMP = new DataTable();

	private DataTable dtAPZ_CRMC = new DataTable();

	private DataTable dtAPZ_CRMT = new DataTable();

	private DataTable dtAPZ_CRMS = new DataTable();

	private DataTable dtRDR1 = new DataTable();

	private DataTable dtORDR = new DataTable();

	private DataTable _messageList = new DataTable();

	private DataTable _commentList = new DataTable();

	private Dictionary<int, int> dictionTagColor = new Dictionary<int, int>();

	private Dictionary<int, int> dictionTagOrderStatus = new Dictionary<int, int>();

	private Dictionary<int, string> dictionOrderStatusColumn = new Dictionary<int, string>();

	private string customerName = "";

	private string conversationID = "";

	private string conversationType = "";

	private string threadID = "";

	private string customerContent = "";

	private string _isActivedPage = "";

	private DateTime pageLastLog;

	private DateTime _since;

	private DateTime _until;

	private double maximumDaysForCommentChecking = 60.0;

	private int conversationCountForChecking = 3;

	private int messageCountForChecking = 5;

	private int conversationCountForAccess = 25;

	private int messageCountForAccess = 20;

	private int maximumCommentLoop = 2;

	private string focusedCustomerId = "";

	private FormLoginMode _formLoginMode;

	private IContainer components;

	private LookUpEdit luk_Page;

	private SimpleButtonEx btnLogin;

	private TextEditEx txtUserName;

	private SimpleButtonEx btnActive;

	private SimpleButtonEx btnAccess;

	private GridControlEx grd_Inbox;

	private GridViewEx gv_Inbox;

	private GridColumnEx gcOrderStatus;

	private GridColumnEx gc_Message_SaleOrder;

	private GridColumnEx gc_Message_Tag;

	private GridColumnEx gc_Message_CreatedTime;

	private XtraTabControlEx xtraTabControl1;

	private XtraTabPageEx tabpage_Message;

	private XtraTabPageEx tabpage_Comment;

	private GridColumnEx gcPageId;

	private GridColumnEx gcConversationId;

	private GridColumnEx gcConversationType;

	private GridColumnEx gcCustomerId;

	private GridColumnEx gc_Message_CustomerName;

	private RepositoryItemButtonEditCFL rep_cfl_SaleOrder;

	private GridColumnEx gcNote;

	private GridColumnEx gcDeleted;

	private GridColumnEx gcHided;

	private GridColumnEx gc_Message_Content;

	private RepositoryItemLookUpEditDefineNew rep_luk_OrderStatus;

	private GridControlEx grd_Comment;

	private GridViewEx gv_Comment;

	private GridColumnEx gridColumnEx1;

	private GridColumnEx gridColumnEx2;

	private GridColumnEx gridColumnEx3;

	private GridColumnEx gridColumnEx4;

	private GridColumnEx gc_Comment_Content;

	private GridColumnEx gc_Comment_CustomerName;

	private GridColumnEx gc_Comment_CreatedAt;

	private GridColumnEx gc_Comment_OrderStatus;

	private GridColumnEx gc_Comment_SaleOrder;

	private GridColumnEx gc_Comment_Tag;

	private GridColumnEx gridColumnEx11;

	private GridColumnEx gridColumnEx12;

	private GridColumnEx gc_Comment_Hided;

	private XtraTabPageEx tabpage_Customer;

	private GridControlEx grd_Customer;

	private GridViewEx gv_Customer;

	private GridColumnEx gridColumnEx14;

	private GridColumnEx gc_Customer_CustomerName;

	private GridColumnEx gc_Customer_BPCode;

	private GridColumnEx gc_Customer_BPName;

	private GridColumnEx gc_Customer_LastContact;

	private GridColumnEx gridColumnEx18;

	private GridColumnEx gridColumnEx19;

	private GridColumnEx gridColumnEx20;

	private GridColumnEx gridColumnEx21;

	private GridColumnEx gridColumnEx22;

	public SimpleButtonEx btnOK;

	private GridControlEx grd_CustomerInbox;

	private GridViewEx gv_CustomerInbox;

	private GridColumnEx gridColumnEx24;

	private GridColumnEx gridColumnEx25;

	private GridColumnEx gridColumnEx26;

	private GridColumnEx gridColumnEx27;

	private GridColumnEx gc_CustomerMessage_Content;

	private GridColumnEx gridColumnEx29;

	private GridColumnEx gc_CustomerMessage_CreatedAt;

	private GridColumnEx gc_CustomerMessage_OrderStatus;

	private GridColumnEx gc_CustomerMessage_SaleOrder;

	private GridColumnEx gc_CustomerMessage_Tag;

	private GridColumnEx gridColumnEx34;

	private GridColumnEx gridColumnEx35;

	private GridColumnEx gridColumnEx36;

	public SimpleButtonEx btCancel;

	private GridColumnEx gridColumnEx38;

	private XtraTabControlEx xtraTabControl2;

	private XtraTabPageEx tabpage_CustomerMessage;

	private XtraTabPageEx tabpage_CustomerComment;

	private RepositoryItemLookUpEditDefineNew rep_luk_Tag;

	private GridColumnEx gc_Message_ThreadID;

	private GridColumnEx gridColumnEx5;

	private GridColumnEx gridColumnEx10;

	private RepositoryItemButtonEditCFL rep_cfl_Message;

	private RepositoryItemDateEditEx rep_date_CreatedAt;

	private Timer timer1;

	private SimpleButtonEx btnSearch;

	private RepositoryItemButtonEditCFL rep_cfl_CustomerID;

	private RepositoryItemButtonEditCFL rep_cfl_BPCode;

	private RepositoryItemButtonEditCFL rep_cfl_BPName;

	private LabelLine labelLine1;

	private LabelLine labelLine2;

	private LabelLine labelLine3;

	private LabelLine labelLine4;

	private LabelLine labelLine5;

	private LabelLine labelLine6;

	private ButtonEditCFL cfl_ItemCode_Search;

	private ButtonEditCFL cfl_BPCode_Search;

	private SimpleButtonEx btnClear;

	private DateEditEx date_From;

	private DateEditEx date_To;

	private CheckedComboBoxEdit ccb_OrderStatus;

	private LabelLine labelLine7;

	private SimpleButtonEx btnCheck;

	private CheckedComboBoxEdit ccb_Tag;

	private LabelLine labelLine8;

	private XtraTabControlEx xtraTabControl3;

	private XtraTabPageEx tabpage_Login;

	private XtraTabPageEx tabpage_Search;

	private XtraTabPageEx tabpage_Info;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem replyToolStripMenuItem;

	private ToolStripMenuItem hideToolStripMenuItem;

	private ToolStripMenuItem hideToolStripMenuItem1;

	private CheckEditEx chkUnread;

	private CheckEditEx checkEdit2;

	private CheckEditEx checkEdit1;

	private XtraTabPageEx tabpage_Reply;

	private RepositoryItemCheckEdit rep_check_Hided;

	private GridControlEx grd_CustomerComment;

	private GridViewEx gv_CustomerComment;

	private GridColumnEx gridColumnEx9;

	private GridColumnEx gridColumnEx32;

	private GridColumnEx gridColumnEx37;

	private GridColumnEx gridColumnEx31;

	private GridColumnEx gridColumnEx39;

	private GridColumnEx gc_CustomerComment_Content;

	private RepositoryItemButtonEditCFL rep_cfl_Comment;

	private GridColumnEx gridColumnEx41;

	private GridColumnEx gc_CustomerComment_CreatedAt;

	private GridColumnEx gc_CustomerComment_OrderStatus;

	private GridColumnEx gc_CustomerComment_SaleOrder;

	private GridColumnEx gc_CustomerComment_Tag;

	private GridColumnEx gridColumnEx46;

	private GridColumnEx gridColumnEx47;

	private GridColumnEx gc_CustomerComment_Hided;

	private RepositoryItemCheckedComboBoxEditDefineNew rep_ccb_Tag;

	private Panel panel1;

	private SimpleButtonEx btnReply;

	private SimpleButtonEx btnReplyComment;

	private LookUpEditDefineNew luk_QuickReply;

	private MemoEditEx memo_ReplyContent;

	private TextEditEx txtCustomerContent;

	private TextEditEx txtCustomerName;

	private LabelLine labelLine10;

	private LabelLine labelLine9;

	private LabelControl labelControl2;

	private LabelControl labelControl1;

	private ListView listOrderStatus;

	private SidePanel sp_CustomerList;

	private SidePanel sp_CustomerInfo;

	private SidePanel sp_CustomerInfo_Top;

	private ButtonEditCFL cfl_BPName;

	private ButtonEditCFL cfl_BPCode;

	private LabelLine labelLine12;

	private LabelLine labelLine11;

	private LabelControl lblLastContact;

	private LabelControl lblName;

	private SimpleButtonEx btnSwitch;

	private SplitContainer splitContainer1;

	private FormLoginMode LoginMode
	{
		get
		{
			return _formLoginMode;
		}
		set
		{
			_formLoginMode = value;
			switch (value)
			{
			case FormLoginMode.Logout:
				_accessToken = "";
				_userID = "";
				_pageId = "";
				pageToken = "";
				txtUserName.Text = "Not yet logged in.";
				btnLogin.Text = "Login";
				btnAccess.Enabled = false;
				btnActive.Enabled = false;
				tabpage_Search.PageEnabled = false;
				tabpage_Reply.PageEnabled = false;
				tabpage_Info.PageEnabled = false;
				xtraTabControl1.Visible = false;
				panel1.Visible = false;
				timer1.Stop();
				break;
			case FormLoginMode.Login:
				btnLogin.Text = "Logout";
				btnAccess.Enabled = true;
				btnActive.Enabled = true;
				break;
			case FormLoginMode.Access:
				tabpage_Search.PageEnabled = true;
				tabpage_Reply.PageEnabled = true;
				tabpage_Info.PageEnabled = true;
				xtraTabControl1.Visible = true;
				panel1.Visible = true;
				splitContainer1.Panel1Collapsed = true;
				break;
			}
		}
	}

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
		{
			WinFuntions.DisableAllControl(Controls);
			btCancel.Enabled = true;
			return;
		}
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Update:
			btnOK.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btnOK.Text = Apzon.Commons.Properties.Resources.ButtonOK;
			break;
		}
	}

	public frm_FacebookCRM()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_FacebookCRM"))
		{
			InitializeComponent();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_FacebookCRM" && x.authoriza == "F"))
			{
				WinFuntions.DisableAllControl(Controls);
				btCancel.Enabled = true;
			}
			FormSetup();
			return;
		}
		throw new Exception(Apzon.Commons.Properties.Resources.Msg_FormAccessDenied);
	}

	private void FormSetup()
	{
		try
		{
			LoginMode = FormLoginMode.Logout;
			timer1.Interval = 10000;
			gc_Message_ThreadID.GroupIndex = 0;
			listOrderStatus.BorderStyle = BorderStyle.None;
			FormController = "FacebookCRM";
			FormId = "oFacebookCRM";
			gv_Inbox.FormId = FormId;
			gv_Comment.FormId = FormId;
			gv_Customer.FormId = FormId;
			gv_CustomerComment.FormId = FormId;
			gv_CustomerInbox.FormId = FormId;
			gv_Inbox.ItemId = "APZ_OCRM1";
			gv_Comment.ItemId = "APZ_OCRM2";
			gv_Customer.ItemId = "APZ_CRMC";
			gv_CustomerComment.ItemId = "APZ_OCRM4";
			gv_CustomerInbox.ItemId = "APZ_OCRM3";
			gv_Comment.Columns["CreatedTime"].SortOrder = ColumnSortOrder.Descending;
			gv_Inbox.Columns["CreatedTime"].SortOrder = ColumnSortOrder.Descending;
			gv_CustomerComment.Columns["CreatedTime"].SortOrder = ColumnSortOrder.Descending;
			gv_CustomerInbox.Columns["CreatedTime"].SortOrder = ColumnSortOrder.Descending;
			gv_Customer.Columns["BPCode"].SortOrder = ColumnSortOrder.Descending;
			gv_Customer.Columns["LastContact"].SortOrder = ColumnSortOrder.Descending;
			gv_Customer.OptionsBehavior.ImmediateUpdateRowPosition = false;
			gv_Comment.OptionsBehavior.ImmediateUpdateRowPosition = false;
			gv_Inbox.OptionsBehavior.ImmediateUpdateRowPosition = false;
			gv_CustomerComment.OptionsBehavior.ImmediateUpdateRowPosition = false;
			gv_CustomerInbox.OptionsBehavior.ImmediateUpdateRowPosition = false;
			gv_Comment.CellValueChanging += HideColumn_CellValueChanging;
			gv_CustomerComment.CellValueChanging += HideColumn_CellValueChanging;
			gv_Inbox.RowCellStyle += gv_Customer_RowCellStyle;
			gv_Comment.RowCellStyle += gv_Customer_RowCellStyle;
			newTuple = new Tuple<GridViewEx, string>(gv_Customer, gv_Customer.ItemId);
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
		btnLogin.Enabled = true;
	}

	private async Task RepositorySetup()
	{
		_ = 1;
		try
		{
			rep_cfl_SaleOrder.ValueMember = "DocNum";
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL = rep_cfl_SaleOrder;
			repositoryItemButtonEditCFL.ItemSelected = (Action<DataRow, GridView>)Delegate.Combine(repositoryItemButtonEditCFL.ItemSelected, new Action<DataRow, GridView>(AutoSetOrderStatusAfterSelectedSaleOrder));
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL2 = rep_cfl_SaleOrder;
			repositoryItemButtonEditCFL2.DataSourceLoaded = (Action<DataTable>)Delegate.Combine(repositoryItemButtonEditCFL2.DataSourceLoaded, (Action<DataTable>)((DataTable dt) =>
			{
				dtORDR = dt.Copy();
			}));
			rep_cfl_SaleOrder.FindCFLBy(Enums.TypeCFL.SalesOder, false, "", Enums.ItemObjectType.NoItemType, null);
			rep_cfl_SaleOrder.TypeDrillDown = ApzObjectTypes.oOrders;
			if (gc_Comment_SaleOrder.ColumnEdit == null)
			{
				gc_Comment_SaleOrder.ColumnEdit = rep_cfl_SaleOrder;
			}
			if (gc_CustomerMessage_SaleOrder.ColumnEdit == null)
			{
				gc_CustomerMessage_SaleOrder.ColumnEdit = rep_cfl_SaleOrder;
			}
			if (gc_CustomerComment_SaleOrder.ColumnEdit == null)
			{
				gc_CustomerComment_SaleOrder.ColumnEdit = rep_cfl_SaleOrder;
			}
			rep_cfl_BPCode.ValueMember = "CardCode";
			rep_cfl_BPCode.TypeDrillDown = ApzObjectTypes.oBusinessPartners;
			rep_cfl_BPCode.FieldNameInDataRow("CardCode", "CardName");
			rep_cfl_BPCode.FieldNameInGrid("BPCode", "BPName");
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL3 = rep_cfl_BPCode;
			repositoryItemButtonEditCFL3.DataSourceLoaded = (Action<DataTable>)Delegate.Combine(repositoryItemButtonEditCFL3.DataSourceLoaded, new Action<DataTable>(SetBusinessPartnerDataSource));
			rep_cfl_BPCode.FindCFLBy(Enums.TypeCFL.Customer, false, "", Enums.ItemObjectType.NoItemType, null);
			rep_cfl_BPName.ValueMember = "CardName";
			rep_cfl_BPName.FieldNameInDataRow("CardCode", "CardName");
			rep_cfl_BPName.FieldNameInGrid("BPCode", "BPName");
			rep_cfl_BPName.AcceptFalseInput = true;
			rep_cfl_BPName.TypeCFL = Enums.TypeCFL.Customer;
			cfl_BPCode.Properties.TypeDrillDown = ApzObjectTypes.oBusinessPartners;
			RepositoryItemButtonEditCFL properties = cfl_BPCode.Properties;
			properties.HandleChooseItem = (Action<DataRow>)Delegate.Combine(properties.HandleChooseItem, new Action<DataRow>(BPCode_HandleChooseItem));
			RepositoryItemButtonEditCFL properties2 = cfl_BPName.Properties;
			properties2.HandleChooseItem = (Action<DataRow>)Delegate.Combine(properties2.HandleChooseItem, new Action<DataRow>(BPCode_HandleChooseItem));
			cfl_BPCode_Search.Properties.ValueMember = "CardCode";
			cfl_BPCode_Search.Properties.AutoLoadDataSource = false;
			cfl_BPCode_Search.Properties.AcceptFalseInput = true;
			cfl_BPCode_Search.Properties.FindCFLBy(Enums.TypeCFL.Customer, true, "", Enums.ItemObjectType.NoItemType, null);
			RepositoryItemButtonEditCFL properties3 = cfl_BPCode_Search.Properties;
			properties3.HandleChooseItemMulti = (Action<List<DataRow>>)Delegate.Combine(properties3.HandleChooseItemMulti, (Action<List<DataRow>>)((List<DataRow> list) =>
			{
				cfl_BPCode_Search.EditValue = null;
				List<string> list2 = new List<string>();
				foreach (DataRow item in list)
				{
					list2.Add(Function.ToString(item["CardCode"]));
				}
				cfl_BPCode_Search.EditValue = string.Join(";", list2);
			}));
			cfl_ItemCode_Search.Properties.ValueMember = "ItemCode";
			cfl_ItemCode_Search.Properties.UsingAutoCompleteBox = true;
			RepositoryItemButtonEditCFL properties4 = cfl_ItemCode_Search.Properties;
			properties4.HandleChooseItemMulti = (Action<List<DataRow>>)Delegate.Combine(properties4.HandleChooseItemMulti, (Action<List<DataRow>>)((List<DataRow> list) =>
			{
				cfl_ItemCode_Search.EditValue = null;
				List<string> list2 = new List<string>();
				foreach (DataRow item2 in list)
				{
					list2.Add(Function.ToString(item2["ItemCode"]));
				}
				cfl_ItemCode_Search.EditValue = string.Join(";", list2);
			}));
			cfl_ItemCode_Search.Properties.AcceptFalseInput = true;
			cfl_ItemCode_Search.Properties.FindCFLBy(Enums.TypeCFL.ListItems, true, "", Enums.ItemObjectType.NoItemType, null);
			rep_luk_OrderStatus.DisplayMember = "Name";
			rep_luk_OrderStatus.ValueMember = "Id";
			rep_luk_OrderStatus.FormProperties.CheckTables = "APZ_OCRM";
			rep_luk_OrderStatus.FormProperties.CheckFields = "OrderStatus";
			rep_luk_OrderStatus.FormProperties.Text = "Order Status - Setup";
			rep_luk_OrderStatus.FormProperties.TableName = "APZ_CRMS";
			RepositoryItemLookUpEditDefineNew repositoryItemLookUpEditDefineNew = rep_luk_OrderStatus;
			repositoryItemLookUpEditDefineNew.DataSourceRefreshed = (Action<DataTable>)Delegate.Combine(repositoryItemLookUpEditDefineNew.DataSourceRefreshed, new Action<DataTable>(GetOrderStatusData));
			Task t2 = rep_luk_OrderStatus.RefreshDataSourceAsync();
			if (gc_Comment_OrderStatus.ColumnEdit == null)
			{
				gc_Comment_OrderStatus.ColumnEdit = rep_luk_OrderStatus;
			}
			if (gc_CustomerComment_OrderStatus.ColumnEdit == null)
			{
				gc_CustomerComment_OrderStatus.ColumnEdit = rep_luk_OrderStatus;
			}
			if (gc_CustomerMessage_OrderStatus.ColumnEdit == null)
			{
				gc_CustomerMessage_OrderStatus.ColumnEdit = rep_luk_OrderStatus;
			}
			ccb_OrderStatus.Properties.ValueMember = "Id";
			ccb_OrderStatus.Properties.DisplayMember = "Name";
			ccb_OrderStatus.Properties.SeparatorChar = ';';
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL4 = rep_cfl_CustomerID;
			repositoryItemButtonEditCFL4.DrillDownMethodWithGridView = (Action<string, GridView>)Delegate.Combine(repositoryItemButtonEditCFL4.DrillDownMethodWithGridView, new Action<string, GridView>(GetCustomerLink));
			if (gc_Comment_CustomerName.ColumnEdit == null)
			{
				gc_Comment_CustomerName.ColumnEdit = rep_cfl_CustomerID;
			}
			if (gc_Message_CustomerName.ColumnEdit == null)
			{
				gc_Message_CustomerName.ColumnEdit = rep_cfl_CustomerID;
			}
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL5 = rep_cfl_Comment;
			repositoryItemButtonEditCFL5.DrillDownMethodWithGridView = (Action<string, GridView>)Delegate.Combine(repositoryItemButtonEditCFL5.DrillDownMethodWithGridView, new Action<string, GridView>(GetCommentLink));
			if (gc_Comment_Content.ColumnEdit == null)
			{
				gc_Comment_Content.ColumnEdit = rep_cfl_Comment;
			}
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL6 = rep_cfl_Message;
			repositoryItemButtonEditCFL6.DrillDownMethodWithGridView = (Action<string, GridView>)Delegate.Combine(repositoryItemButtonEditCFL6.DrillDownMethodWithGridView, new Action<string, GridView>(GetMessageLink));
			if (gc_CustomerMessage_Content.ColumnEdit == null)
			{
				gc_CustomerMessage_Content.ColumnEdit = rep_cfl_Message;
			}
			rep_date_CreatedAt.Mask.MaskType = MaskType.DateTimeAdvancingCaret;
			if (gc_CustomerMessage_CreatedAt.ColumnEdit == null)
			{
				gc_CustomerMessage_CreatedAt.ColumnEdit = rep_date_CreatedAt;
			}
			if (gc_CustomerComment_CreatedAt.ColumnEdit == null)
			{
				gc_CustomerComment_CreatedAt.ColumnEdit = rep_date_CreatedAt;
			}
			if (gc_Comment_CreatedAt.ColumnEdit == null)
			{
				gc_Comment_CreatedAt.ColumnEdit = rep_date_CreatedAt;
			}
			if (gc_Customer_LastContact.ColumnEdit == null)
			{
				gc_Customer_LastContact.ColumnEdit = rep_date_CreatedAt;
			}
			ccb_Tag.Properties.ValueMember = "Id";
			ccb_Tag.Properties.DisplayMember = "Name";
			ccb_Tag.Properties.SeparatorChar = ';';
			rep_ccb_Tag.ValueMember = "Id";
			rep_ccb_Tag.DisplayMember = "Name";
			rep_ccb_Tag.SeparatorChar = ';';
			rep_ccb_Tag.FormProperties.DictionRepository.Add("Color", new RepositoryItemColorPickEdit
			{
				StoreColorAsInteger = true
			});
			rep_ccb_Tag.FormProperties.DictionRepository.Add("OrderStatus", rep_luk_OrderStatus);
			rep_ccb_Tag.FormProperties.DictionRepository.Add("Inactive", rep_check_Hided);
			rep_ccb_Tag.FormProperties.Text = "Tag - Setup";
			rep_ccb_Tag.FormProperties.CheckTables = "APZ_OCRM";
			rep_ccb_Tag.FormProperties.CheckFields = "Tag";
			rep_ccb_Tag.FormProperties.TableName = "APZ_CRMT";
			RepositoryItemCheckedComboBoxEditDefineNew repositoryItemCheckedComboBoxEditDefineNew = rep_ccb_Tag;
			repositoryItemCheckedComboBoxEditDefineNew.DataSourceRefreshed = (Action<DataTable>)Delegate.Combine(repositoryItemCheckedComboBoxEditDefineNew.DataSourceRefreshed, new Action<DataTable>(GetTagData));
			rep_ccb_Tag.EditValueChanged += rep_ccb_Tag_EditValueChanged;
			Task task = rep_ccb_Tag.RefreshDataAsync();
			if (gc_Comment_Tag.ColumnEdit == null)
			{
				gc_Comment_Tag.ColumnEdit = rep_ccb_Tag;
			}
			if (gc_CustomerComment_Tag.ColumnEdit == null)
			{
				gc_CustomerComment_Tag.ColumnEdit = rep_ccb_Tag;
			}
			if (gc_Message_Tag.ColumnEdit == null)
			{
				gc_Message_Tag.ColumnEdit = rep_ccb_Tag;
			}
			luk_QuickReply.Properties.FormProperties.Text = "Quick Reply - Setup";
			luk_QuickReply.Properties.FormProperties.TableName = "APZ_CRMR";
			luk_QuickReply.Properties.ResizePopup = false;
			luk_QuickReply.Properties.RefreshDataSource();
			if (gc_CustomerComment_Hided.ColumnEdit == null)
			{
				gc_CustomerComment_Hided.ColumnEdit = rep_check_Hided;
			}
			if (gc_Comment_Hided.ColumnEdit == null)
			{
				gc_Comment_Hided.ColumnEdit = rep_check_Hided;
			}
			await task;
			await t2;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void SaleOrderDrillDown(string s)
	{
		this.DrillDown(ApzObjectTypes.oOrders, Convert.ToString(await GetDocNumByDocEntry(Function.ParseInt(s))));
	}

	private void BPCode_HandleChooseItem(DataRow dr)
	{
		try
		{
			cfl_BPCode.EditValue = dr["CardCode"];
			cfl_BPName.EditValue = dr["CardName"];
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void SetBusinessPartnerDataSource(DataTable dt)
	{
		try
		{
			rep_cfl_BPName.DataSource = dt.Copy();
			cfl_BPCode_Search.Properties.DataSource = dt.Copy();
			cfl_BPCode.Properties.DataSource = dt.Copy();
			cfl_BPName.Properties.DataSource = dt.Copy();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rep_ccb_Tag_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!(sender is BaseEdit baseEdit))
			{
				return;
			}
			string[] stringArrayFromCheckedComboBox = GetStringArrayFromCheckedComboBox(baseEdit.EditValue);
			if (stringArrayFromCheckedComboBox.Length != 0)
			{
				int key = Function.ParseInt(stringArrayFromCheckedComboBox[0]);
				if (dictionTagOrderStatus.ContainsKey(key) && Function.ParseInt(dictionTagOrderStatus[key]) != 0 && baseEdit.Parent is GridControlEx gridControlEx)
				{
					((GridViewEx)gridControlEx.MainView).SetFocusedRowCellValue("OrderStatus", dictionTagOrderStatus[key]);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void AutoSetOrderStatusAfterSelectedSaleOrder(DataRow dr, GridView gridView)
	{
		gridView.SetFocusedRowCellValue("OrderStatus", 2);
	}

	private void GetCustomerLink(string s, GridView gridView)
	{
		try
		{
			string text = "";
			text = Function.ToString(gridView.GetFocusedRowCellValue("CustomerID"));
			if (!string.IsNullOrEmpty(text))
			{
				Process.Start("https://www.facebook.com/" + text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void GetCommentLink(string s, GridView gridView)
	{
		try
		{
			string text = "";
			text = Function.ToString(gridView.GetFocusedRowCellValue("ConversationID"));
			if (!string.IsNullOrEmpty(text))
			{
				Process.Start("https://www.facebook.com/" + text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void GetMessageLink(string s, GridView gridView)
	{
		try
		{
			string text = "";
			text = Function.ToString(gridView.GetFocusedRowCellValue("ThreadID"));
			text = text.Substring(2);
			string arg = Function.ToString(gridView.GetFocusedRowCellValue("PageID"));
			if (!string.IsNullOrEmpty(text))
			{
				Process.Start($"https://www.facebook.com/{arg}/messages/?folder=inbox&section=messages&threadid={text}");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void GetOrderStatusData(DataTable dt)
	{
		try
		{
			dtAPZ_CRMS = dt.Copy();
			if (dtAPZ_CRMS.HasColumn() && dtAPZ_CRMS.IsNotNull())
			{
				ccb_OrderStatus.Properties.DataSource = dtAPZ_CRMS;
				ccb_OrderStatus.Properties.DropDownRows = dtAPZ_CRMS.Rows.Count + 1;
				dictionOrderStatusColumn.Clear();
				dictionOrderStatusColumn = dtAPZ_CRMS.AsEnumerable().ToDictionary((DataRow s) => Function.ParseInt(s["Id"]), (DataRow s) => Function.ToString(s["Name"]));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void GetTagData(DataTable dt)
	{
		try
		{
			if (refreshTagData)
			{
				return;
			}
			dtAPZ_CRMT = dt.Copy();
			if (dtAPZ_CRMT.HasColumn() && dtAPZ_CRMT.IsNotNull())
			{
				string filterExpression = "Inactive<>'Y' OR Inactive IS NULL";
				DataRow[] array = dtAPZ_CRMT.Select(filterExpression);
				if (array.Length != 0)
				{
					dtAPZ_CRMT = array.CopyToDataTable();
				}
				else
				{
					dtAPZ_CRMT = dt.Clone();
				}
				dictionTagColor.Clear();
				dictionTagColor = dtAPZ_CRMT.AsEnumerable().ToDictionary((DataRow s) => Function.ParseInt(s["Id"]), (DataRow s) => Function.ParseInt(s["Color"]));
				dictionTagOrderStatus.Clear();
				dictionTagOrderStatus = dtAPZ_CRMT.AsEnumerable().ToDictionary((DataRow s) => Function.ParseInt(s["Id"]), (DataRow s) => Function.ParseInt(s["OrderStatus"]));
				ccb_Tag.Properties.DataSource = dtAPZ_CRMT;
				ccb_Tag.Properties.DropDownRows = dtAPZ_CRMT.Rows.Count + 1;
				refreshTagData = true;
				rep_ccb_Tag.DataSource = dtAPZ_CRMT.Copy();
				refreshTagData = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void GetTableStruct()
	{
		_ = 4;
		try
		{
			Task<DataTable> tableStructureAsync = WinFuntions.GetTableStructureAsync("APZ_OCRM");
			Task<DataTable> t2 = WinFuntions.GetTableStructureAsync("APZ_CRMU");
			Task<DataTable> t3 = WinFuntions.GetTableStructureAsync("APZ_CRMP");
			Task<DataTable> t4 = WinFuntions.GetTableStructureAsync("APZ_CRMC");
			Task<DataTable> t5 = LoadOrderAndItemCode();
			dtAPZ_OCRM = await tableStructureAsync;
			dtAPZ_CRMU = await t2;
			dtAPZ_CRMP = await t3;
			dtAPZ_CRMC = await t4;
			dtRDR1 = await t5;
			dtAPZ_OCRMFilter = dtAPZ_OCRM.Clone();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void FacebookCRM_Load(object sender, EventArgs e)
	{
		try
		{
			GetTableStruct();
			TryLogin();
			RepositorySetup();
			gv_Inbox.OptionsView.ColumnAutoWidth = true;
			gv_Comment.OptionsView.ColumnAutoWidth = true;
			gv_CustomerInbox.OptionsView.ColumnAutoWidth = true;
			gv_CustomerComment.OptionsView.ColumnAutoWidth = true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void TryLogin()
	{
		try
		{
			Uri uri = GenerateLoginUrl("669236043274789", "public_profile,manage_pages,read_page_mailboxes,publish_pages");
			WebBrowser webBrowser = new WebBrowser();
			webBrowser.Navigated += webBrowser_Navigated;
			webBrowser.Navigate(uri.AbsoluteUri);
		}
		catch (NullReferenceException)
		{
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
			WinCommons.ErrorLineNumber(ex2);
		}
	}

	private async void webBrowser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
	{
		try
		{
			_fb = new FacebookClient();
			if (e.Url.AbsolutePath.Equals("/connect/blank.html"))
			{
				return;
			}
			if (_fb.TryParseOAuthCallbackUrl(e.Url, out var facebookOAuthResult))
			{
				if (facebookOAuthResult.IsSuccess)
				{
					await GetUserToken(facebookOAuthResult);
				}
				else
				{
					LoginMode = FormLoginMode.Logout;
				}
			}
			else
			{
				LoginMode = FormLoginMode.Logout;
			}
		}
		catch (NullReferenceException)
		{
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
			WinCommons.ErrorLineNumber(ex2);
		}
	}

	private Uri GenerateLoginUrl(string appId, string extendedPermissions)
	{
		try
		{
			dynamic val = new ExpandoObject();
			val.client_id = appId;
			val.redirect_uri = "https://www.facebook.com/connect/login_success.html";
			val.response_type = "token";
			val.display = "popup";
			if (!string.IsNullOrWhiteSpace(extendedPermissions))
			{
				val.scope = extendedPermissions;
			}
			return _fb.GetLoginUrl(val);
		}
		catch (NullReferenceException)
		{
			return null;
		}
	}

	private async void btnLogin_Click(object sender, EventArgs e)
	{
		try
		{
			btnLogin.Enabled = false;
			switch (LoginMode)
			{
			case FormLoginMode.Login:
			case FormLoginMode.Access:
				Logout();
				break;
			case FormLoginMode.Logout:
				await LoginFacebook();
				break;
			}
		}
		catch (NullReferenceException)
		{
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
			WinCommons.ErrorLineNumber(ex2);
		}
		finally
		{
			btnLogin.Enabled = true;
		}
	}

	private void Logout()
	{
		if (!string.IsNullOrEmpty(_accessToken))
		{
			Uri logoutUrl = new FacebookClient().GetLogoutUrl(new
			{
				next = "https://www.facebook.com/connect/login_success.html",
				access_token = _accessToken
			});
			WebBrowser webBrowser = new WebBrowser();
			webBrowser.Navigated += (object o, WebBrowserNavigatedEventArgs args) =>
			{
				_ = args.Url.AbsoluteUri == "https://www.facebook.com/connect/login_success.html";
			};
			webBrowser.Navigate(logoutUrl.AbsoluteUri);
			LoginMode = FormLoginMode.Logout;
		}
	}

	private async Task LoginFacebook()
	{
		try
		{
			frm_FacebookLoginDialog frm_FacebookLoginDialog2 = new frm_FacebookLoginDialog("669236043274789", "public_profile,manage_pages,read_page_mailboxes,publish_pages");
			frm_FacebookLoginDialog2.ShowDialog(this);
			if (frm_FacebookLoginDialog2.FacebookOAuthResult != null && frm_FacebookLoginDialog2.FacebookOAuthResult.IsSuccess)
			{
				await GetUserToken(frm_FacebookLoginDialog2.FacebookOAuthResult);
			}
			else
			{
				LoginMode = FormLoginMode.Logout;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task GetUserToken(FacebookOAuthResult facebookOAuthResult)
	{
		_ = 2;
		try
		{
			string token = (_accessToken = facebookOAuthResult.AccessToken);
			FacebookClient fb = new FacebookClient(token);
			dynamic userInfo = await fb.GetTaskAsync("me?field=id,name");
			await SaveUserInfoToDB(token, userInfo);
			_userID = userInfo["id"];
			txtUserName.Text = userInfo["name"];
			await GetPageInfoFromGraphAPIAndSave(fb);
		}
		catch (NullReferenceException)
		{
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
			WinCommons.ErrorLineNumber(ex2);
		}
	}

	private async Task SaveUserInfoToDB(string token, dynamic userInfo)
	{
		try
		{
			DataTable tableStructure = WinFuntions.GetTableStructure("APZ_CRMU");
			DataRow dataRow = tableStructure.NewRow();
			dataRow["UserID"] = (object)userInfo["id"];
			dataRow["UserName"] = (object)userInfo["name"];
			dataRow["AccessToken"] = token;
			dataRow["LastLog"] = DateTime.Now;
			tableStructure.Rows.Add(dataRow);
			tableStructure.AcceptChanges();
			using HttpClientHelper<string> http = new HttpClientHelper<string>(Configuration.ApiServer, FormController);
			string text = await http.PostAsync("SaveUser", tableStructure);
			if (string.IsNullOrEmpty(text))
			{
				LoginMode = FormLoginMode.Login;
				return;
			}
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), text);
			WinCommons.Error(Enums.MessageCode.None, text);
		}
		catch (NullReferenceException)
		{
		}
	}

	private async Task GetPageInfoFromGraphAPIAndSave(FacebookClient fb)
	{
		_ = 2;
		try
		{
			dynamic val = await fb.GetTaskAsync("me/accounts");
			DataTable pageGraph = (dtAPZ_CRMP.HasColumn() ? dtAPZ_CRMP.Clone() : WinFuntions.GetTableStructure("APZ_CRMP"));
			if (!pageGraph.HasColumn())
			{
				return;
			}
			foreach (dynamic item in val.data.ToArray())
			{
				DataRow dataRow = pageGraph.NewRow();
				dataRow["UserID"] = _userID;
				dataRow["PageID"] = (object)item["id"];
				dataRow["PageName"] = (object)item["name"];
				dataRow["AccessToken"] = (object)item["access_token"];
				pageGraph.Rows.Add(dataRow);
				pageGraph.AcceptChanges();
			}
			if (pageGraph.IsNotNull())
			{
				using HttpClientHelper<string> http = new HttpClientHelper<string>(Configuration.ApiServer, FormController);
				string text = await http.PostAsync("SavePageList", pageGraph);
				if (!string.IsNullOrEmpty(text))
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), text);
				}
			}
			DataTable dt = await LoadPageInfoFromUser(_userID);
			if (!dt.HasColumn())
			{
				dt = pageGraph.Clone();
			}
			dtAPZ_CRMP = dt;
			if (!dtAPZ_CRMP.HasColumn())
			{
				return;
			}
			luk_Page.Properties.DisplayMember = "PageName";
			luk_Page.Properties.ValueMember = "PageID";
			luk_Page.Properties.ShowHeader = false;
			luk_Page.Properties.ShowFooter = false;
			luk_Page.Properties.DataSource = dtAPZ_CRMP.Copy();
			luk_Page.Properties.DropDownRows = dtAPZ_CRMP.Rows.Count;
			luk_Page.Properties.ForceInitialize();
			luk_Page.Properties.PopulateColumns();
			foreach (LookUpColumnInfo column in luk_Page.Properties.Columns)
			{
				column.Visible = false;
			}
			luk_Page.Properties.Columns["PageName"].Visible = true;
			if (dtAPZ_CRMP.IsNotNull())
			{
				luk_Page.EditValue = dtAPZ_CRMP.Rows[0]["PageID"];
			}
		}
		catch (NullReferenceException)
		{
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
			WinCommons.ErrorLineNumber(ex2);
		}
	}

	private async void btnActive_Click(object sender, EventArgs e)
	{
		_ = 2;
		try
		{
			if (string.IsNullOrEmpty(_pageId))
			{
				WinCommons.Error(Enums.MessageCode.PleaseSelectAPage, "");
			}
			else if (string.IsNullOrEmpty(Function.ToString(dtAPZ_CRMP.AsEnumerable().FirstOrDefault((DataRow s) => Function.ToString(s["PageId"]) == _pageId)["AccessToken"])))
			{
				WinCommons.Error(Enums.MessageCode.InvalidPageToken, "");
			}
			else if (!Function.ToString(dtAPZ_CRMP.AsEnumerable().FirstOrDefault((DataRow s) => Function.ToString(s["PageId"]) == _pageId)["Actived"]).Equals("Y") || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmReActivePage, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) != DialogResult.No)
			{
				_isLoading = true;
				btnActive.Enabled = false;
				WinCommons._OpenProcessing();
				timer1.Stop();
				if (string.IsNullOrEmpty(await ActiveMessagesAndSendToDB(await DataReturnFromGraphAPI())))
				{
					grd_Inbox.DataSource = dtAPZ_OCRM.Clone();
					grd_Comment.DataSource = dtAPZ_OCRM.Clone();
					WinCommons.Success(Enums.MessageCode.ActiveSuccess, "");
					pageLastLog = DateTime.Now;
					await SetPageLastLog(_pageId);
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.ActiveFailed, "");
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
			timer1.Start();
			WinCommons._CloseProcessing();
			btnActive.Enabled = true;
			_isLoading = false;
		}
	}

	private async Task<DataTable> DataReturnFromGraphAPI(string since = "", int until = 0, bool isChecking = false)
	{
		_ = 1;
		try
		{
			string accessToken = Function.ToString(dtAPZ_CRMP.AsEnumerable().FirstOrDefault((DataRow s) => Function.ToString(s["PageID"]) == _pageId)["AccessToken"]);
			DataTable dt = (dtAPZ_OCRM.HasColumn() ? dtAPZ_OCRM.Clone() : WinFuntions.GetTableStructure("APZ_OCRM"));
			FacebookClient fb = new FacebookClient(accessToken);
			Task<DataTable> inboxFromGraphAPI = GetInboxFromGraphAPI(dt, fb, _pageId, since, until, isChecking);
			Task<DataTable> t2 = GetCommentFromGraphAPI(dt, fb, _pageId, since, until, isChecking);
			_messageList = await inboxFromGraphAPI;
			_commentList = await t2;
			if (_messageList.HasColumn() && _messageList.IsNotNull())
			{
				dt.Merge(_messageList);
				dt.AcceptChanges();
			}
			if (_commentList.HasColumn() && _commentList.IsNotNull())
			{
				dt.Merge(_commentList);
				dt.AcceptChanges();
			}
			return dt;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	private async Task SetPageLastLog(string page)
	{
		try
		{
			using HttpClientHelper<string> http = new HttpClientHelper<string>(Configuration.ApiServer, FormController);
			Tuple<string, DateTime> pr = new Tuple<string, DateTime>(page, pageLastLog);
			string text = await http.PostAsync("ChangePageStatus", pr);
			if (!string.IsNullOrEmpty(text))
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), text);
				WinCommons.Error(Enums.MessageCode.None, text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<string> SaveMessagesToDB(DataTable dt)
	{
		try
		{
			using HttpClientHelper<string> http = new HttpClientHelper<string>(Configuration.ApiServer, FormController);
			string text = await http.PostAsync("SaveMessages", dt);
			if (Function.ParseInt(text) > 0)
			{
				return text;
			}
			if (!string.IsNullOrEmpty(text))
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), text);
			}
			return text;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return "Failed to save.";
	}

	private async Task<string> ActiveMessagesAndSendToDB(DataTable dt)
	{
		try
		{
			using HttpClientHelper<string> http = new HttpClientHelper<string>(Configuration.ApiServer, FormController);
			string text = await http.PostAsync("ActiveMessages", dt);
			if (string.IsNullOrEmpty(text))
			{
				return "";
			}
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), text);
			return text;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return "Failed to save.";
	}

	private async Task<string> SaveCustomersToDB(DataSet dt)
	{
		try
		{
			using HttpClientHelper<string> http = new HttpClientHelper<string>(Configuration.ApiServer, FormController);
			string text = await http.PostAsync("SaveCustomers", dt);
			if (string.IsNullOrEmpty(text))
			{
				return "";
			}
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), text);
			return text;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return "Failed to save.";
	}

	public static int ToUnix(DateTime dateTime)
	{
		return (int)(TimeZoneInfo.ConvertTimeToUtc(dateTime) - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
	}

	private async Task<DataTable> GetInboxFromGraphAPI(DataTable dtToClone, FacebookClient fb, string pageId, string since = "", int until = 0, bool isChecking = false)
	{
		_ = 2;
		try
		{
			watch.Start();
			if (dtToClone.HasColumn())
			{
				DataTable messageList = dtToClone.Clone();
				string text = "";
				string text2 = "";
				if (!string.IsNullOrEmpty(since))
				{
					if (isChecking)
					{
						text = $".limit({conversationCountForChecking})";
						text2 = $".limit({messageCountForChecking})";
					}
					else
					{
						text = $".limit({conversationCountForAccess})";
						text2 = $".limit({messageCountForAccess})";
					}
				}
				string path = pageId + "?fields=conversations" + text + "{updated_time,messages" + text2 + "{message,from,created_time},thread_key}";
				_ = until;
				_ = 0;
				DateTime sinceTime = DateTime.MinValue;
				if (!string.IsNullOrEmpty(since))
				{
					sinceTime = DateTime.Parse(since);
				}
				List<Task> tasks = new List<Task>();
				bool oldDataDetected = false;
				dynamic val = await fb.GetTaskAsync(path);
				foreach (dynamic item in val.conversations.data.ToArray())
				{
					DateTime dateTime = DateTime.Parse(Function.ToString(item.updated_time)).ToUniversalTime();
					if (sinceTime != DateTime.MinValue && dateTime < sinceTime)
					{
						oldDataDetected = true;
						break;
					}
					dynamic messagesInThread = GetMessagesInThread(fb, pageId, sinceTime, messageList, item);
					tasks.Add(messagesInThread);
				}
				if (val.conversations.paging != null && val.conversations.paging.next != null && !oldDataDetected)
				{
					string nextURL = val.conversations.paging.next;
					await GetNextConversation(nextURL, fb, messageList, pageId, sinceTime);
				}
				if (tasks.Count > 0)
				{
					await Task.WhenAll(tasks);
				}
				watch.Stop();
				return messageList;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			watch.Reset();
		}
		return new DataTable();
	}

	private async Task GetMessagesInThread(FacebookClient fb, string pageId, DateTime sinceTime, DataTable messageList, dynamic data1)
	{
		if (!((data1.messages != null) ? true : false))
		{
			return;
		}
		string text = data1.thread_key;
		foreach (dynamic item in data1.messages.data.ToArray())
		{
			DateTime dateTime = DateTime.Parse(Function.ToString(item.created_time)).ToUniversalTime();
			DataRow dataRow = messageList.NewRow();
			dataRow["PageID"] = pageId;
			dataRow["ConversationID"] = (object)item.id;
			dataRow["ConversationType"] = "M";
			dataRow["CustomerID"] = (object)item.from.id;
			dataRow["CustomerName"] = (object)item.from.name;
			dataRow["Content"] = (object)item.message;
			dataRow["CreatedTime"] = dateTime;
			dataRow["ThreadID"] = text;
			messageList.Rows.Add(dataRow);
			messageList.AcceptChanges();
		}
		if (data1.messages.paging != null && data1.messages.paging.next != null)
		{
			string nextURL = data1.messages.paging.next;
			await GetNextMessage(nextURL, fb, messageList, pageId, text, sinceTime);
		}
	}

	private async Task GetNextConversation(string nextURL, FacebookClient fb, DataTable messageList, string pageId, DateTime sinceTime)
	{
		_ = 1;
		try
		{
			bool oldDataDetected = false;
			while (!string.IsNullOrEmpty(nextURL))
			{
				List<Task> tasks = new List<Task>();
				dynamic info = await fb.GetTaskAsync(nextURL);
				foreach (dynamic item in info.data)
				{
					DateTime dateTime = DateTime.Parse(Function.ToString(item.updated_time)).ToUniversalTime();
					if (sinceTime != DateTime.MinValue && dateTime < sinceTime)
					{
						oldDataDetected = true;
						break;
					}
					dynamic messagesInThread = GetMessagesInThread(fb, pageId, sinceTime, messageList, item);
					tasks.Add(messagesInThread);
				}
				if (tasks.Count > 0)
				{
					await Task.WhenAll(tasks);
				}
				nextURL = ((!((info.paging != null && info.paging.next != null && !oldDataDetected) ? true : false)) ? "" : ((string)info.paging.next));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task GetNextMessage(string nextURL, FacebookClient fb, DataTable messageList, string pageId, string threadKey, DateTime sinceTime)
	{
		try
		{
			while (!string.IsNullOrEmpty(nextURL))
			{
				dynamic val = await fb.GetTaskAsync(nextURL);
				foreach (dynamic item in val.data)
				{
					DateTime dateTime = DateTime.Parse(Function.ToString(item.created_time)).ToUniversalTime();
					DataRow dataRow = messageList.NewRow();
					dataRow["PageID"] = pageId;
					dataRow["ConversationID"] = (object)item.id;
					dataRow["ConversationType"] = "M";
					dataRow["CustomerID"] = (object)item.from.id;
					dataRow["CustomerName"] = (object)item.from.name;
					dataRow["Content"] = (object)item.message;
					dataRow["CreatedTime"] = dateTime;
					dataRow["ThreadID"] = threadKey;
					messageList.Rows.Add(dataRow);
					messageList.AcceptChanges();
				}
				nextURL = ((!((val.paging != null && val.paging.next != null) ? true : false)) ? "" : ((string)val.paging.next));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<DataTable> GetCommentFromGraphAPI(DataTable dtToClone, FacebookClient fb, string pageId, string since = "", int until = 0, bool isChecking = false)
	{
		_ = 2;
		try
		{
			Stopwatch sw = new Stopwatch();
			sw.Start();
			if (dtToClone.HasColumn())
			{
				DataTable commentList = dtToClone.Clone();
				string text = "";
				if (!string.IsNullOrEmpty(since))
				{
					text = $".since({since})";
				}
				string path = pageId + "?fields=feed.limit(100){id,created_time,comments.filter(stream)" + text + "{id,from,message,created_time,is_hidden,parent}}";
				DateTime sinceTime = DateTime.MinValue;
				if (!string.IsNullOrEmpty(since))
				{
					sinceTime = DateTime.Parse(since);
				}
				List<Task> tasks = new List<Task>();
				bool outdatedFeed = false;
				dynamic val = await fb.GetTaskAsync(path);
				string nextURL = "";
				dynamic val2 = val.feed.data;
				foreach (dynamic item in val2.ToArray())
				{
					DateTime dateTime = DateTime.Parse(Function.ToString(item.created_time)).ToUniversalTime();
					if (isChecking && (DateTime.Now - dateTime).TotalDays >= maximumDaysForCommentChecking)
					{
						outdatedFeed = true;
						break;
					}
					if (item.comments != null)
					{
						dynamic commentsInFeed = GetCommentsInFeed(fb, pageId, commentList, item);
						tasks.Add(commentsInFeed);
					}
				}
				if (val.feed.paging != null && val.feed.paging.next != null && !outdatedFeed)
				{
					nextURL = val.feed.paging.next;
				}
				Task t1 = GetNextFeed(nextURL, fb, commentList, pageId, sinceTime, isChecking);
				if (tasks.Count > 0)
				{
					await Task.WhenAll(tasks);
				}
				await t1;
				sw.Stop();
				sw.Reset();
				return commentList;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	private async Task GetCommentsInFeed(FacebookClient fb, string pageId, DataTable commentList, dynamic data1)
	{
		try
		{
			string nextURL = "";
			if (data1.comments.paging != null && data1.comments.paging.next != null)
			{
				nextURL = data1.comments.paging.next;
			}
			Task nextComment = GetNextComment(nextURL, fb, commentList, pageId);
			foreach (dynamic item in data1.comments.data.ToArray())
			{
				AddCommentInfo(pageId, commentList, item, fb);
			}
			await nextComment;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task GetNextFeed(string nextURL, FacebookClient fb, DataTable commentList, string pageId, DateTime sinceTime, bool isChecking = false, int loopLevel = 1)
	{
		_ = 2;
		try
		{
			if (string.IsNullOrEmpty(nextURL) || loopLevel >= maximumCommentLoop)
			{
				return;
			}
			loopLevel++;
			bool outdatedFeed = false;
			List<Task> tasks = new List<Task>();
			dynamic val = await fb.GetTaskAsync(nextURL);
			foreach (dynamic item in val.data)
			{
				DateTime dateTime = DateTime.Parse(Function.ToString(item.created_time)).ToUniversalTime();
				if (isChecking && (DateTime.Now - dateTime).TotalDays >= maximumDaysForCommentChecking)
				{
					outdatedFeed = true;
					break;
				}
				if (item.comments != null)
				{
					dynamic commentsInFeed = GetCommentsInFeed(fb, pageId, commentList, item);
					tasks.Add(commentsInFeed);
				}
			}
			nextURL = ((!((val.paging != null && val.paging.next != null && !outdatedFeed) ? true : false)) ? "" : ((string)val.paging.next));
			Task t1 = GetNextFeed(nextURL, fb, commentList, pageId, sinceTime, isChecking, loopLevel);
			if (tasks.Count > 0)
			{
				await Task.WhenAll(tasks);
			}
			await t1;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task GetNextComment(string nextURL, FacebookClient fb, DataTable commentList, string pageId)
	{
		_ = 1;
		try
		{
			if (string.IsNullOrEmpty(nextURL))
			{
				return;
			}
			dynamic val = await fb.GetTaskAsync(nextURL);
			foreach (dynamic item in val.data)
			{
				AddCommentInfo(pageId, commentList, item, fb);
			}
			nextURL = ((!((val.paging != null && val.paging.next != null) ? true : false)) ? "" : ((string)val.paging.next));
			await GetNextComment(nextURL, fb, commentList, pageId);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void AddCommentInfo(string pageId, DataTable commentList, dynamic data2, FacebookClient fb)
	{
		try
		{
			if (data2.from != null)
			{
				DataRow dataRow = commentList.NewRow();
				dataRow["PageID"] = pageId;
				dataRow["ConversationID"] = (object)data2.id;
				dataRow["ConversationType"] = "C";
				dataRow["CustomerID"] = (object)data2.from.id;
				dataRow["CustomerName"] = (object)data2.from.name;
				dataRow["Content"] = (object)data2.message;
				DateTime dateTime = DateTime.Parse(Function.ToString(data2.created_time));
				dataRow["CreatedTime"] = dateTime;
				if ((bool)data2.is_hidden)
				{
					dataRow["Hided"] = "Y";
				}
				commentList.Rows.Add(dataRow);
				commentList.AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<DataTable> LoadMessagesFromDB()
	{
		try
		{
			using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
			return await http.GetAsync("LoadMessages", new
			{
				pageId = _pageId
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	private async Task<DataTable> LoadCustomersFromDB()
	{
		try
		{
			using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
			return await http.GetAsync("LoadCustomers", new
			{
				pageId = _pageId
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	private async Task<DataTable> LoadMessagesForCustomer(string customerId)
	{
		try
		{
			using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
			return await http.GetAsync("LoadMessagesForCustomer", new
			{
				pageId = _pageId,
				customerId = customerId
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	private async Task<DataTable> LoadPageInfo()
	{
		try
		{
			using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
			return await http.GetAsync("LoadPageInfo", new
			{
				pageId = _pageId
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	private async Task<DataTable> LoadPageInfoFromUser(string userId)
	{
		try
		{
			using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
			return await http.GetAsync("LoadPageInfoFromUser", new { userId });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	private async Task<DataTable> LoadOrderAndItemCode()
	{
		try
		{
			using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
			return await http.GetAsync("LoadOrderAndItemCode");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	private async Task<int> GetDocNumByDocEntry(int docEntry)
	{
		try
		{
			using HttpClientHelper<string> http = new HttpClientHelper<string>(Configuration.ApiServer, FormController);
			return await http.GetIntAsync("GetDocNumByDocEntry", new { docEntry });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return 0;
	}

	private void btnSwitch_Click(object sender, EventArgs e)
	{
		splitContainer1.Panel1Collapsed = !splitContainer1.Panel1Collapsed;
	}

	private bool StayToSave()
	{
		if (FormMode == Enums.FormViewModeType.Update)
		{
			switch (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1))
			{
			case DialogResult.Yes:
				return false;
			case DialogResult.No:
				return true;
			}
		}
		return false;
	}

	private void CheckSaveState()
	{
		if (!_isLoading && !hideComment)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	private void btCancel_Click(object sender, EventArgs e)
	{
		isCancel = true;
		Close();
	}

	private void frm_FacebookCRM_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Update && !isCancel)
			{
				switch (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1))
				{
				case DialogResult.Cancel:
					e.Cancel = true;
					break;
				case DialogResult.Yes:
					e.Cancel = true;
					saveBeforeClose = true;
					btnOK_Click(null, null);
					break;
				}
			}
			else if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmClose, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.No)
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

	private void gv_Customer_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			GridView gridView = sender as GridView;
			if (e.RowHandle == gridView.FocusedRowHandle)
			{
				e.Appearance.BackColor = Color.FromArgb(226, 234, 253);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Inbox_CustomDrawGroupRow(object sender, RowObjectCustomDrawEventArgs e)
	{
		try
		{
			GridViewEx gridViewEx = sender as GridViewEx;
			GridGroupRowInfo gridGroupRowInfo = e.Info as GridGroupRowInfo;
			if (gridGroupRowInfo.Column.Caption == "ThreadID")
			{
				string threadID = Function.ToString(gridViewEx.GetGroupRowValue(gridGroupRowInfo.RowHandle, gridGroupRowInfo.Column));
				DataRow dataRow = gridViewEx.GridControl.DataSourceTable().AsEnumerable().FirstOrDefault((DataRow s) => Function.ToString(s["ThreadID"]).Equals(threadID) && !Function.ToString(s["CustomerID"]).Equals(_pageId));
				if (dataRow != null)
				{
					gridGroupRowInfo.GroupText = Function.ToString(dataRow["CustomerName"]);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_CustomerComment_RowStyle(object sender, RowStyleEventArgs e)
	{
		try
		{
			if (Function.ToString((sender as GridView).GetRowCellValue(e.RowHandle, "Hided")) == "Y")
			{
				e.Appearance.BackColor = Color.FromArgb(229, 229, 229);
				e.Appearance.ForeColor = Color.FromArgb(115, 115, 115);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Inbox_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			GridView gridView = sender as GridView;
			if (!e.Column.FieldName.Equals("Tag") || gridView.GetRowCellValue(e.RowHandle, "Tag") == null)
			{
				return;
			}
			string[] stringArrayFromCheckedComboBox = GetStringArrayFromCheckedComboBox(gridView.GetRowCellValue(e.RowHandle, e.Column));
			if (stringArrayFromCheckedComboBox.Length != 0)
			{
				int num = Function.ParseInt(stringArrayFromCheckedComboBox[0]);
				if (num != 0 && dictionTagColor.Count > 0 && dictionTagColor.ContainsKey(num))
				{
					int num2 = dictionTagColor[num];
					ColorEdit colorEdit = new ColorEdit();
					colorEdit.EditValue = num2;
					e.Appearance.ForeColor = Color.Black;
					e.Appearance.BackColor = colorEdit.Color;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Customer_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
	{
		try
		{
			if (e.Column is GridColumnEx gridColumnEx && gridColumnEx.ColumnType == typeof(DateTime))
			{
				e.DisplayText = WinFuntions.FormatCreatedDate(Function.ToString(e.Value), shorten: true);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void luk_Page_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			_pageId = Function.ToString(luk_Page.EditValue);
			DataRow dataRow = dtAPZ_CRMP.AsEnumerable().FirstOrDefault((DataRow s) => Function.ToString(s["PageID"]) == _pageId);
			_isActivedPage = Function.ToString(dataRow["Actived"]);
			pageLastLog = Function.ParseDateTimes(dataRow["LastLog"]);
			pageToken = Function.ToString(dataRow["AccessToken"]);
			_isActivedPage.Equals("Y");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Inbox_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			CheckSaveState();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Customer_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			if (!customerUpdate)
			{
				changeCustomerRow = true;
				focusedCustomerId = Function.ToString(gv_Customer.GetRowCellValue(e.RowHandle, "CustomerID"));
				RefreshSubGrids();
				xtraTabControl2_SelectedPageChanged(null, null);
				changeCustomerRow = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_CustomerComment_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (!_isLoading && !loadMessage && !loadCustomer)
			{
				customerCommentUpdate = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_CustomerInbox_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (!_isLoading && !loadMessage && !loadCustomer)
			{
				customerMessageUpdate = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Customer_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (!_isLoading && !loadMessage && !loadCustomer)
			{
				customerUpdate = true;
				CheckSaveState();
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
		try
		{
			if (!_isLoading && !loadMessage && !loadCustomer && !changeCustomerRow)
			{
				if (cfl_BPCode.EditValue == null)
				{
					changeCustomerRow = true;
					cfl_BPName.EditValue = null;
					changeCustomerRow = false;
				}
				customerUpdate = true;
				CheckSaveState();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void cfl_BPName_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading && !loadMessage && !loadCustomer && !changeCustomerRow)
			{
				if (cfl_BPName.EditValue == null)
				{
					changeCustomerRow = true;
					cfl_BPCode.EditValue = null;
					changeCustomerRow = false;
				}
				customerUpdate = true;
				CheckSaveState();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void xtraTabControl1_SelectedPageChanging(object sender, TabPageChangingEventArgs e)
	{
		try
		{
			if (StayToSave())
			{
				e.Cancel = true;
			}
			else
			{
				FormMode = Enums.FormViewModeType.View;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void xtraTabControl1_SelectedPageChanged(object sender, TabPageChangedEventArgs e)
	{
		try
		{
			GridView view = new GridView();
			switch (e.Page.Name)
			{
			case "tabpage_Message":
				view = gv_Inbox;
				break;
			case "tabpage_Comment":
				view = gv_Comment;
				break;
			case "tabpage_Customer":
			{
				string text = xtraTabControl2.SelectedTabPage.Text;
				if (!(text == "Messages"))
				{
					if (text == "Comments")
					{
						view = gv_CustomerComment;
					}
				}
				else
				{
					view = gv_CustomerInbox;
				}
				break;
			}
			}
			FillReplyByFocusedRow(view);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void xtraTabControl2_SelectedPageChanged(object sender, TabPageChangedEventArgs e)
	{
		try
		{
			GridView view = new GridView();
			string name = xtraTabControl2.SelectedTabPage.Name;
			if (!(name == "tabpage_CustomerMessage"))
			{
				if (name == "tabpage_CustomerComment")
				{
					view = gv_CustomerComment;
				}
			}
			else
			{
				view = gv_CustomerInbox;
			}
			FillReplyByFocusedRow(view);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Customer_BeforeLeaveRow(object sender, RowAllowEventArgs e)
	{
		try
		{
			if (xtraTabControl1.SelectedTabPage.Text != "Customers")
			{
				return;
			}
			if (StayToSave())
			{
				e.Allow = false;
				return;
			}
			if (customerUpdate)
			{
				grd_Customer.DataSourceTable.RejectChanges();
			}
			FormMode = Enums.FormViewModeType.View;
			customerUpdate = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btnAccess_Click(object sender, EventArgs e)
	{
		_ = 2;
		try
		{
			_isLoading = true;
			btnAccess.Enabled = false;
			WinCommons._OpenProcessing();
			timer1.Stop();
			if (string.IsNullOrEmpty(_pageId))
			{
				WinCommons.Error(Enums.MessageCode.PleaseSelectAPage, "");
				return;
			}
			DataTable dataTable = await LoadPageInfo();
			if (dataTable.HasColumn() && dataTable.IsNotNull())
			{
				pageLastLog = Function.ParseDateTimes(dataTable.Rows[0]["LastLog"]);
			}
			await RefreshGrid();
			await UpdateDataSinceLastLog();
			LoginMode = FormLoginMode.Access;
			xtraTabControl3.SelectedTabPage = tabpage_Search;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			timer1.Start();
			WinCommons._CloseProcessing();
			btnAccess.Enabled = true;
			_isLoading = false;
		}
	}

	private async Task UpdateDataSinceLastLog(bool isChecking = false)
	{
		_ = 1;
		try
		{
			Stopwatch watch = new Stopwatch();
			watch.Start();
			_since = pageLastLog;
			_until = DateTime.Now;
			string since = _since.ToUniversalTime().ToString("s");
			int until = ToUnix(_until);
			DataTable dataTable = await DataReturnFromGraphAPI(since, until, isChecking);
			if (dataTable.HasColumn() && dataTable.IsNotNull())
			{
				Task task = SaveDataSinceLastLogToDB(dataTable);
				dtAPZ_OCRM.PrimaryKey = new DataColumn[1] { dtAPZ_OCRM.Columns["ConversationID"] };
				dataTable.PrimaryKey = new DataColumn[1] { dataTable.Columns["ConversationID"] };
				dtAPZ_OCRM.Merge(dataTable);
				dtAPZ_OCRM.AcceptChanges();
				pageLastLog = _until;
				if (_messageList.HasColumn() && _messageList.IsNotNull())
				{
					DataTable dataSourceTable = grd_Inbox.DataSourceTable;
					dataSourceTable.PrimaryKey = new DataColumn[1] { dataSourceTable.Columns["ConversationID"] };
					_messageList.PrimaryKey = new DataColumn[1] { _messageList.Columns["ConversationID"] };
					dataSourceTable.Merge(_messageList);
					dataSourceTable.AcceptChanges();
				}
				if (_commentList.HasColumn() && _commentList.IsNotNull())
				{
					DataTable dataSourceTable2 = grd_Comment.DataSourceTable;
					dataSourceTable2.PrimaryKey = new DataColumn[1] { dataSourceTable2.Columns["ConversationID"] };
					_commentList.PrimaryKey = new DataColumn[1] { _commentList.Columns["ConversationID"] };
					dataSourceTable2.Merge(_commentList);
					dataSourceTable2.AcceptChanges();
				}
				UpdateCustomerGridSinceLastLog(dataTable);
				await task;
			}
			_messageList = new DataTable();
			_commentList = new DataTable();
			watch.Stop();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task SaveDataSinceLastLogToDB(DataTable dt)
	{
		_isLoading = true;
		string text = await SaveMessagesToDB(dt);
		if (string.IsNullOrEmpty(text))
		{
			WinCommons.Info(Enums.MessageCode.PostAdded, 30, Function.ToString(dt.Rows.Count));
			await SetPageLastLog(_pageId);
		}
		else
		{
			WinCommons.Error(Enums.MessageCode.None, text);
		}
		_isLoading = false;
	}

	private void UpdateCustomerGridSinceLastLog(DataTable dt)
	{
		try
		{
			var enumerable = from s in dt.AsEnumerable()
				group s by new
				{
					Id = Function.ToString(s["CustomerID"]),
					Name = s["CustomerName"]
				} into t
				select new
				{
					PageID = _pageId,
					CustomerID = t.Key.Id,
					CustomerName = t.Key.Name,
					LastContact = Function.ParseDateTimes(t.Max((DataRow s) => s["CreatedTime"]))
				};
			enumerable.ToDictionary(s => s.CustomerID, s => s.LastContact);
			if (!dtAPZ_CRMC.HasColumn())
			{
				return;
			}
			DataTable dataSourceTable = grd_Customer.DataSourceTable;
			if (!dataSourceTable.HasColumn())
			{
				return;
			}
			foreach (var customer in enumerable)
			{
				if (dataSourceTable.AsEnumerable().Any((DataRow s) => s["CustomerID"].Equals(customer.CustomerID)))
				{
					foreach (DataRow item in dataSourceTable.AsEnumerable())
					{
						if (item["CustomerID"].Equals(customer.CustomerID))
						{
							item["LastContact"] = customer.LastContact;
						}
					}
					dataSourceTable.AcceptChanges();
				}
				else
				{
					DataRow dataRow = dataSourceTable.NewRow();
					dataRow["PageID"] = customer.PageID;
					dataRow["CustomerID"] = customer.CustomerID;
					dataRow["CustomerName"] = customer.CustomerName;
					dataRow["LastContact"] = customer.LastContact;
					dataSourceTable.Rows.Add(dataRow);
					dataSourceTable.AcceptChanges();
				}
			}
			dtAPZ_CRMC = dataSourceTable.Copy();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btnCheck_Click(object sender, EventArgs e)
	{
		await CheckNewPost();
	}

	private async Task CheckNewPost()
	{
		try
		{
			WinCommons._OpenProcessing();
			await UpdateDataSinceLastLog();
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

	private async void timer1_Tick(object sender, EventArgs e)
	{
		try
		{
			if (timer1.Enabled)
			{
				btnLogin.Enabled = false;
				timer1.Stop();
				await UpdateDataSinceLastLog(isChecking: true);
				timer1.Start();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			btnLogin.Enabled = true;
		}
	}

	private async Task RefreshGrid()
	{
		_ = 1;
		try
		{
			_isLoading = true;
			Task<DataTable> task = LoadMessagesFromDB();
			Task<DataTable> t2 = LoadCustomersFromDB();
			DataTable dt = await task;
			if (dt.HasColumn())
			{
				dtAPZ_OCRM = dt;
			}
			dtAPZ_OCRM.AcceptChanges();
			DataTable dt2 = await t2;
			if (dt2.HasColumn())
			{
				dtAPZ_CRMC = dt2;
			}
			dtAPZ_CRMC.AcceptChanges();
			SetCustomersDataSource();
			SetMainSource(dtAPZ_OCRM);
			RefreshSubGrids();
			_isLoading = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void SetMainSource(DataTable dt)
	{
		loadMessage = true;
		int focusedRowHandle = gv_Inbox.FocusedRowHandle;
		grd_Inbox.DataSource = dt;
		gv_Inbox.FocusedRowHandle = focusedRowHandle;
		int focusedRowHandle2 = gv_Comment.FocusedRowHandle;
		grd_Comment.DataSource = dt;
		gv_Comment.FocusedRowHandle = focusedRowHandle2;
		grd_CustomerInbox.DataSource = dtAPZ_OCRM;
		grd_CustomerComment.DataSource = dtAPZ_OCRM;
		loadMessage = false;
	}

	private void SetCustomersDataSource()
	{
		try
		{
			loadCustomer = true;
			int focusedRowHandle = gv_Customer.FocusedRowHandle;
			gv_Customer.BeginDataUpdate();
			DataTable dataTable = dtAPZ_CRMC;
			if (dataTable.HasColumn() && dataTable.IsNotNull())
			{
				dataTable.BeginInit();
				gv_Customer.BeginUpdate();
				grd_Customer.DataSource = dataTable.Copy();
				grd_Customer.RefreshDataSource();
				gv_Customer.EndUpdate();
				dataTable.EndInit();
			}
			else
			{
				grd_Customer.DataSource = dtAPZ_CRMC.Clone();
			}
			grd_Customer.DataSourceTable.AcceptChanges();
			gv_Customer.EndDataUpdate();
			gv_Customer.FocusedRowHandle = focusedRowHandle;
			loadCustomer = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ConfigColumn(GridColumn column, int width, bool readOnly = true, bool visible = false, int visibleIndex = -1, string caption = "")
	{
		try
		{
			column.Width = width;
			column.OptionsColumn.ReadOnly = readOnly;
			column.Visible = visible;
			column.VisibleIndex = visibleIndex;
			if (caption != "")
			{
				column.Caption = caption;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void RefreshSubGrids()
	{
		try
		{
			Function.ToString(gv_Customer.GetFocusedRowCellValue("CustomerID"));
			cfl_BPCode.EditValue = gv_Customer.GetFocusedRowCellValue("BPCode");
			cfl_BPName.EditValue = gv_Customer.GetFocusedRowCellValue("BPName");
			lblName.Text = Function.ToString(gv_Customer.GetFocusedRowCellValue("CustomerName"));
			lblLastContact.Text = "Last contact since " + WinFuntions.FormatCreatedDate(Function.ToString(gv_Customer.GetFocusedRowCellValue("LastContact")), shorten: true);
			listOrderStatus.Items.Clear();
			foreach (DataColumn column in (grd_Customer.DataSource as DataTable).Columns)
			{
				int key = Function.ParseInt(column.ColumnName);
				if (dictionOrderStatusColumn.ContainsKey(key))
				{
					int num = Function.ParseInt(gv_Customer.GetFocusedRowCellValue(column.ColumnName));
					string text = dictionOrderStatusColumn[key] + ": " + num;
					if (num <= 0)
					{
						text = "";
					}
					listOrderStatus.Items.Add(text);
				}
			}
			grd_CustomerInbox.RefreshDataSource();
			grd_CustomerComment.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void SetSubCommentGridData(string customerId)
	{
		gv_CustomerComment.BeginDataUpdate();
		DataTable dataTable = ((!isFiltering) ? dtAPZ_OCRM : dtAPZ_OCRMFilter);
		if (dataTable.HasColumn() && dataTable.IsNotNull())
		{
			DataRow[] array = dataTable.Select($"ConversationType = 'C' AND CustomerID ='{customerId}'");
			if (array.Length != 0)
			{
				grd_CustomerComment.DataSource = array.CopyToDataTable();
			}
			else
			{
				grd_CustomerComment.DataSource = dtAPZ_OCRM.Clone();
			}
		}
		else
		{
			grd_CustomerComment.DataSource = dtAPZ_OCRM.Clone();
		}
		gv_CustomerComment.EndDataUpdate();
	}

	private void SetSubMessageGridData(string customerId)
	{
		gv_CustomerInbox.BeginDataUpdate();
		DataTable dataTable = ((!isFiltering) ? dtAPZ_OCRM : dtAPZ_OCRMFilter);
		if (dataTable.HasColumn() && dataTable.IsNotNull())
		{
			DataRow[] array = dataTable.Select($"ConversationType = 'M' AND CustomerID ='{customerId}'");
			if (array.Length != 0)
			{
				grd_CustomerInbox.DataSource = array.CopyToDataTable();
			}
			else
			{
				grd_CustomerInbox.DataSource = dtAPZ_OCRM.Clone();
			}
		}
		else
		{
			grd_CustomerInbox.DataSource = dtAPZ_OCRM.Clone();
		}
		gv_CustomerInbox.EndDataUpdate();
	}

	private async void btnOK_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Update)
			{
				btnOK.Enabled = false;
				WinCommons._OpenProcessing();
				timer1.Stop();
				await UpdateData();
			}
			else
			{
				Close();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			timer1.Start();
			WinCommons._CloseProcessing();
			btnOK.Enabled = true;
		}
	}

	private async Task UpdateData()
	{
		try
		{
			DataSet dataSet = new DataSet();
			switch (xtraTabControl1.SelectedTabPage.Text)
			{
			case "Customers":
			{
				DataTable dataTable2 = dtAPZ_CRMC.Clone();
				if (customerUpdate)
				{
					DataRow dataRow = dataTable2.NewRow();
					dataRow.ItemArray = gv_Customer.GetDataRow(gv_Customer.FocusedRowHandle).ItemArray;
					dataRow["BPCode"] = Function.ToString(cfl_BPCode.EditValue);
					dataRow["BPName"] = Function.ToString(cfl_BPName.EditValue);
					dataTable2.Rows.Add(dataRow);
					dataTable2.AcceptChanges();
				}
				DataTable dataTable3 = dtAPZ_OCRM.Clone();
				foreach (DataRow item in dtAPZ_OCRM.AsEnumerable())
				{
					if (item.RowState == DataRowState.Modified)
					{
						dataTable3.ImportRow(item);
					}
				}
				dataTable3.AcceptChanges();
				dataTable2.TableName = "Customers";
				dataTable3.TableName = "Messages";
				dataSet.Tables.Add(dataTable2);
				dataSet.Tables.Add(dataTable3);
				break;
			}
			case "Messages":
			{
				DataTable dataTable4 = dtAPZ_OCRM.Clone();
				DataRow[] array2 = (from s in grd_Inbox.DataSourceTable.AsEnumerable()
					where s.RowState == DataRowState.Modified
					select s).ToArray();
				if (array2.Length != 0)
				{
					dataTable4 = array2.CopyToDataTable();
				}
				dataTable4.AcceptChanges();
				if (dataTable4.IsNotNull())
				{
					dataTable4.TableName = "Messages";
					dataSet.Tables.Add(dataTable4);
				}
				break;
			}
			case "Comments":
			{
				DataTable dataTable = dtAPZ_OCRM.Clone();
				DataRow[] array = (from s in grd_Comment.DataSourceTable.AsEnumerable()
					where s.RowState == DataRowState.Modified
					select s).ToArray();
				if (array.Length != 0)
				{
					dataTable = array.CopyToDataTable();
				}
				if (dataTable.IsNotNull())
				{
					dataTable.TableName = "Messages";
					dataSet.Tables.Add(dataTable);
				}
				break;
			}
			}
			await SaveCurrentGridToDB(dataSet);
			if (saveBeforeClose)
			{
				Close();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			saveBeforeClose = false;
		}
	}

	private async Task SaveCurrentGridToDB(DataSet ds)
	{
		string text;
		if (ds.Tables.Count > 1)
		{
			text = await SaveCustomersToDB(ds);
		}
		else
		{
			if (ds.Tables.Count <= 0)
			{
				customerMessageUpdate = false;
				customerCommentUpdate = false;
				customerUpdate = false;
				FormMode = Enums.FormViewModeType.View;
				return;
			}
			text = await SaveMessagesToDB(ds.Tables[0]);
		}
		if (string.IsNullOrEmpty(text))
		{
			WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
			customerMessageUpdate = false;
			customerCommentUpdate = false;
			customerUpdate = false;
			FormMode = Enums.FormViewModeType.View;
			RefreshGrid();
		}
		else
		{
			WinCommons.Error(Enums.MessageCode.None, text);
		}
	}

	private void btnSearch_Click(object sender, EventArgs e)
	{
		try
		{
			if (!dtAPZ_OCRM.HasColumn() || !dtAPZ_CRMC.HasColumn())
			{
				return;
			}
			if (FormMode == Enums.FormViewModeType.Update)
			{
				WinCommons.Warning(Enums.MessageCode.PleaseSaveDataFirst, "");
				return;
			}
			btnSearch.Enabled = false;
			isFiltering = true;
			bpCodes = Function.ToString(cfl_BPCode_Search.EditValue).Split(new char[1] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
			itemCodes = Function.ToString(cfl_ItemCode_Search.EditValue).Split(new char[1] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
			orderStatus = Function.ToString(ccb_OrderStatus.EditValue).Split(new char[1] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
			tags = Function.ToString(ccb_Tag.EditValue).Split(new char[1] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
			for (int i = 0; i < orderStatus.Count; i++)
			{
				orderStatus[i] = orderStatus[i].Trim();
			}
			for (int j = 0; j < tags.Count; j++)
			{
				tags[j] = tags[j].Trim();
			}
			dateTo = DateTime.MaxValue;
			if (date_To.EditValue != null)
			{
				dateTo = Function.ParseDateTimes(date_To.EditValue);
			}
			dateFrom = DateTime.MinValue;
			if (date_From.EditValue != null)
			{
				dateFrom = Function.ParseDateTimes(date_From.EditValue);
			}
			listCustomerBP.Clear();
			if (bpCodes.Count > 0)
			{
				foreach (DataRow item in grd_Customer.DataSourceTable.AsEnumerable())
				{
					string text = Function.ToString(item["CustomerID"]);
					if (text != "")
					{
						listCustomerBP.Add(text);
					}
				}
			}
			listDocEntry.Clear();
			if (itemCodes.Count > 0)
			{
				DataRow[] source = (from s in dtRDR1.AsEnumerable()
					where itemCodes.Contains(Function.ToString(s["ItemCode"]))
					select s).ToArray();
				listDocEntry = source.Select((DataRow s) => Function.ToString(s["DocEntry"])).Distinct().ToList();
			}
			grd_Inbox.RefreshDataSource();
			grd_Comment.RefreshDataSource();
			listCustomerFilter.Clear();
			for (int num = 0; num < gv_Inbox.RowCount; num++)
			{
				string text2 = Function.ToString(gv_Inbox.GetRowCellValue(num, "CustomerID"));
				if (text2 != "")
				{
					listCustomerFilter.Add(text2);
				}
			}
			listCustomerFilter = listCustomerFilter.Distinct().ToList();
			grd_Customer.RefreshDataSource();
			grd_CustomerInbox.RefreshDataSource();
			grd_CustomerComment.RefreshDataSource();
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			btnSearch.Enabled = true;
		}
	}

	private void SearchFilter(RowFilterEventArgs e, GridView view)
	{
		bool handled = false;
		if (date_From.EditValue != null || date_To.EditValue != null)
		{
			DateTime dateTime = Function.ParseDateTimes(view.GetListSourceRowCellValue(e.ListSourceRow, "CreatedTime"));
			if (!(dateFrom <= dateTime) || !(dateTime <= dateTo))
			{
				e.Visible = false;
				handled = true;
			}
		}
		if (orderStatus.Count > 0)
		{
			string item = Function.ToString(view.GetListSourceRowCellValue(e.ListSourceRow, "OrderStatus"));
			if (!orderStatus.Contains(item))
			{
				e.Visible = false;
				handled = true;
			}
		}
		if (tags.Count > 0)
		{
			string item2 = Function.ToString(view.GetListSourceRowCellValue(e.ListSourceRow, "Tag"));
			if (!tags.Contains(item2))
			{
				e.Visible = false;
				handled = true;
			}
		}
		if (bpCodes.Count > 0)
		{
			string item3 = Function.ToString(view.GetListSourceRowCellValue(e.ListSourceRow, "CustomerID"));
			if (!listCustomerBP.Contains(item3))
			{
				e.Visible = false;
				handled = true;
			}
		}
		if (itemCodes.Count > 0)
		{
			string text = Function.ToString(view.GetListSourceRowCellValue(e.ListSourceRow, "DocEntry"));
			if (text == "" || !listDocEntry.Contains(text))
			{
				e.Visible = false;
				handled = true;
			}
		}
		e.Handled = handled;
	}

	private void gv_Inbox_CustomRowFilter(object sender, RowFilterEventArgs e)
	{
		try
		{
			GridView gridView = sender as GridView;
			string text = Function.ToString(gridView.GetListSourceRowCellValue(e.ListSourceRow, "ConversationType"));
			if (gridView == gv_Inbox)
			{
				if (text != "M")
				{
					e.Visible = false;
					e.Handled = true;
					return;
				}
			}
			else if (gridView == gv_Comment && text != "C")
			{
				e.Visible = false;
				e.Handled = true;
				return;
			}
			if (isFiltering)
			{
				SearchFilter(e, gridView);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Customer_CustomRowFilter(object sender, RowFilterEventArgs e)
	{
		if (!isFiltering)
		{
			return;
		}
		try
		{
			string item = Function.ToString((sender as GridView).GetListSourceRowCellValue(e.ListSourceRow, "CustomerID"));
			if (!listCustomerFilter.Contains(item))
			{
				e.Visible = false;
				e.Handled = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_CustomerInbox_CustomRowFilter(object sender, RowFilterEventArgs e)
	{
		try
		{
			GridView gridView = sender as GridView;
			string value = Function.ToString(gridView.GetListSourceRowCellValue(e.ListSourceRow, "CustomerID"));
			string text = Function.ToString(gridView.GetListSourceRowCellValue(e.ListSourceRow, "ConversationType"));
			if (!focusedCustomerId.Equals(value) || text != "M")
			{
				e.Visible = false;
				e.Handled = true;
			}
			else if (isFiltering)
			{
				SearchFilter(e, gridView);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_CustomerComment_CustomRowFilter(object sender, RowFilterEventArgs e)
	{
		try
		{
			GridView gridView = sender as GridView;
			string value = Function.ToString(gridView.GetListSourceRowCellValue(e.ListSourceRow, "CustomerID"));
			string text = Function.ToString(gridView.GetListSourceRowCellValue(e.ListSourceRow, "ConversationType"));
			if (!focusedCustomerId.Equals(value) || text != "C")
			{
				e.Visible = false;
				e.Handled = true;
			}
			else if (isFiltering)
			{
				SearchFilter(e, gridView);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private string[] GetStringArrayFromCheckedComboBox(object editValue, char separateChar = ';')
	{
		try
		{
			string[] array = Function.ToString(editValue).Split(new char[1] { separateChar }, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = array[i].Trim();
			}
			return array;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new string[1] { "" };
	}

	private void btnClear_Click(object sender, EventArgs e)
	{
		try
		{
			isFiltering = false;
			grd_Inbox.RefreshDataSource();
			grd_Comment.RefreshDataSource();
			(gv_Customer.DataSource as DataView).RowFilter = "";
			grd_Customer.RefreshDataSource();
			grd_CustomerInbox.RefreshDataSource();
			grd_CustomerComment.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btnReplyComment_Click(object sender, EventArgs e)
	{
		try
		{
			btnReplyComment.Enabled = false;
			if (conversationType == "C")
			{
				string nodeID = conversationID;
				await ReplyCommentWithComment(nodeID);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			btnReplyComment.Enabled = true;
		}
	}

	private async void btnReply_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			btnReply.Enabled = false;
			string text = conversationType;
			if (!(text == "M"))
			{
				if (text == "C")
				{
					string nodeID = conversationID;
					await ReplyCommentWithMessage(nodeID);
				}
			}
			else
			{
				string nodeID = threadID;
				await ReplyMessage(nodeID);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			btnReply.Enabled = true;
		}
	}

	private async Task ReplyMessage(string nodeID)
	{
		try
		{
			if (string.IsNullOrEmpty(pageToken))
			{
				return;
			}
			FacebookClient facebookClient = new FacebookClient(pageToken);
			if (!string.IsNullOrEmpty(nodeID) && !string.IsNullOrEmpty(memo_ReplyContent.Text))
			{
				string path = nodeID + "/messages?message=";
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary["message"] = memo_ReplyContent.Text;
				if (((dynamic)(await facebookClient.PostTaskAsync(path, dictionary))).id != null)
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.FailedToReply, "");
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task ReplyCommentWithMessage(string nodeID)
	{
		try
		{
			if (string.IsNullOrEmpty(pageToken))
			{
				return;
			}
			FacebookClient facebookClient = new FacebookClient(pageToken);
			if (!string.IsNullOrEmpty(nodeID) && !string.IsNullOrEmpty(memo_ReplyContent.Text))
			{
				string path = nodeID + "/private_replies?message=";
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary["message"] = memo_ReplyContent.Text;
				if (((dynamic)(await facebookClient.PostTaskAsync(path, dictionary))).id != null)
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.FailedToReply, "");
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task ReplyCommentWithComment(string nodeID)
	{
		try
		{
			if (string.IsNullOrEmpty(pageToken))
			{
				return;
			}
			FacebookClient facebookClient = new FacebookClient(pageToken);
			if (!string.IsNullOrEmpty(nodeID) && !string.IsNullOrEmpty(memo_ReplyContent.Text))
			{
				string path = nodeID + "/comments?message=";
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary["message"] = memo_ReplyContent.Text;
				if (((dynamic)(await facebookClient.PostTaskAsync(path, dictionary))).id != null)
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.FailedToReply, "");
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_CustomerInbox_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			if (ActiveControl == null)
			{
				return;
			}
			EditorContainer activeGridControl = ActiveControl.GetActiveGridControl();
			if (activeGridControl != null && (activeGridControl as GridControlEx).MainView is GridView gridView)
			{
				GridView gridView2 = sender as GridView;
				if (gridView2 == gridView)
				{
					FillReplyByFocusedRow(gridView2);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void FillReplyByFocusedRow(GridView view)
	{
		if (view == null || view.DataRowCount == 0)
		{
			customerName = "";
			conversationID = "";
			conversationType = "";
			threadID = "";
			customerContent = "";
			txtCustomerName.Text = "";
			txtCustomerContent.Text = "";
			btnReplyComment.Enabled = false;
			btnReply.Enabled = false;
			return;
		}
		customerName = Function.ToString(view.GetFocusedRowCellValue("CustomerName"));
		conversationID = Function.ToString(view.GetFocusedRowCellValue("ConversationID"));
		conversationType = Function.ToString(view.GetFocusedRowCellValue("ConversationType"));
		threadID = Function.ToString(view.GetFocusedRowCellValue("ThreadID"));
		customerContent = Function.ToString(view.GetFocusedRowCellValue("Content"));
		txtCustomerName.Text = customerName;
		txtCustomerContent.Text = customerContent;
		string text = conversationType;
		if (!(text == "M"))
		{
			if (text == "C")
			{
				btnReplyComment.Enabled = true;
			}
		}
		else
		{
			btnReplyComment.Enabled = false;
		}
		btnReply.Enabled = true;
	}

	private async void hideToolStripMenuItem1_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (_isLoading || hideComment)
			{
				return;
			}
			hideComment = true;
			ContextMenuStrip contextMenuStrip = (sender as ToolStripMenuItem).Owner as ContextMenuStrip;
			if (!((contextMenuStrip.SourceControl as GridControl).MainView is GridView view))
			{
				return;
			}
			string text = Function.ToString(view.GetFocusedRowCellValue("Hided"));
			if (!conversationType.Equals("C"))
			{
				return;
			}
			if (!(text == "Y") && ((text != null && text.Length == 0) || text == "N") && await CheckCommentCanHide(conversationID, commentHiddenStatusInGrid: true))
			{
				await ChangeCommentHideStatus(conversationID, wantToHide: true, view);
				view.SetFocusedRowCellValue("Hided", "Y");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			hideComment = false;
		}
		CheckSaveState();
	}

	private void rep_check_Hided_CheckedChanged(object sender, EventArgs e)
	{
	}

	private async Task<bool> CheckCommentCanHide(string nodeID, bool commentHiddenStatusInGrid)
	{
		try
		{
			if (string.IsNullOrEmpty(pageToken))
			{
				return false;
			}
			FacebookClient facebookClient = new FacebookClient(pageToken);
			if (!string.IsNullOrEmpty(nodeID))
			{
				string path = nodeID + "/?fields=can_hide,is_hidden";
				dynamic val = await facebookClient.GetTaskAsync(path);
				if (val.can_hide != null)
				{
					dynamic val2 = Function.ParseBool(val.can_hide);
					if ((!val2))
					{
						if (commentHiddenStatusInGrid)
						{
							WinCommons.Error(Enums.MessageCode.CannotHideThisComment, Msg_Time._TimeTiny, "");
						}
						else
						{
							WinCommons.Error(Enums.MessageCode.CannotUnhideThisCommnet, Msg_Time._TimeTiny, "");
						}
						return false;
					}
					if (val.is_hidden != null)
					{
						if ((bool)Function.ParseBool(val.is_hidden) != commentHiddenStatusInGrid)
						{
							return true;
						}
						if (commentHiddenStatusInGrid)
						{
							WinCommons.Warning(Enums.MessageCode.ThisCommentAlreadyHide, Msg_Time._TimeTiny, "");
						}
						else
						{
							WinCommons.Warning(Enums.MessageCode.ThisCommentAlreadyShow, Msg_Time._TimeTiny, "");
						}
						return false;
					}
					return false;
				}
				return false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return false;
	}

	private async Task ChangeCommentHideStatus(string nodeID, bool wantToHide, GridView view)
	{
		_ = 1;
		try
		{
			if (string.IsNullOrEmpty(pageToken))
			{
				return;
			}
			FacebookClient facebookClient = new FacebookClient(pageToken);
			if (string.IsNullOrEmpty(nodeID))
			{
				return;
			}
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["is_hidden"] = wantToHide;
			if (((dynamic)(await facebookClient.PostTaskAsync(nodeID, dictionary))).success != null)
			{
				if (wantToHide)
				{
					WinCommons.Success(Enums.MessageCode.Hidden, Msg_Time._TimeTiny, "");
				}
				else
				{
					WinCommons.Success(Enums.MessageCode.Shown, Msg_Time._TimeTiny, "");
				}
				DataTable dataTable = dtAPZ_OCRM.Clone();
				DataRow dataRow = dataTable.NewRow();
				dataRow["PageID"] = Function.ToString(view.GetFocusedRowCellValue("PageID"));
				dataRow["CustomerID"] = Function.ToString(view.GetFocusedRowCellValue("CustomerID"));
				dataRow["CustomerName"] = Function.ToString(view.GetFocusedRowCellValue("CustomerName"));
				dataRow["ConversationId"] = Function.ToString(view.GetFocusedRowCellValue("ConversationID"));
				int num = Function.ParseInt(view.GetFocusedRowCellValue("OrderStatus"));
				if (num > 0)
				{
					dataRow["OrderStatus"] = num;
				}
				int num2 = Function.ParseInt(view.GetFocusedRowCellValue("DocEntry"));
				if (num2 > 0)
				{
					dataRow["DocEntry"] = num2;
				}
				int num3 = Function.ParseInt(view.GetFocusedRowCellValue("Tag"));
				if (num3 > 0)
				{
					dataRow["Tag"] = num3;
				}
				dataRow["Hided"] = (wantToHide ? "Y" : "N");
				dataRow["Note"] = Function.ToString(view.GetFocusedRowCellValue("Note"));
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
				await SaveMessagesToDB(dataTable);
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.Failed, Msg_Time._TimeTiny, "");
				if (wantToHide)
				{
					view.SetFocusedRowCellValue("Hided", "N");
				}
				else
				{
					view.SetFocusedRowCellValue("Hided", "Y");
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void HideColumn_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		_ = 3;
		try
		{
			if (_isLoading || hideComment || e.Column.FieldName != "Hided")
			{
				return;
			}
			hideComment = true;
			GridView view = sender as GridView;
			string text = Function.ToString(e.Value);
			if (view == null || !conversationType.Equals("C"))
			{
				return;
			}
			if (FormMode == Enums.FormViewModeType.Update)
			{
				view.SetRowCellValue(e.RowHandle, e.Column, (text == "Y") ? "Y" : "N");
				WinCommons.Warning(Enums.MessageCode.PleaseSaveDataFirst, "");
				return;
			}
			if (!(text == "Y"))
			{
				if (text == "N")
				{
					if (await CheckCommentCanHide(conversationID, commentHiddenStatusInGrid: false))
					{
						await ChangeCommentHideStatus(conversationID, wantToHide: false, view);
					}
					else
					{
						view.SetRowCellValue(e.RowHandle, e.Column, "Y");
					}
				}
			}
			else if (await CheckCommentCanHide(conversationID, commentHiddenStatusInGrid: true))
			{
				await ChangeCommentHideStatus(conversationID, wantToHide: true, view);
			}
			else
			{
				view.SetRowCellValue(e.RowHandle, e.Column, "N");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			hideComment = false;
		}
		CheckSaveState();
	}

	private void grd_Comment_Click(object sender, EventArgs e)
	{
	}

	private void sp_CustomerInfo_Top_Click(object sender, EventArgs e)
	{
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
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Facebook_CRM.frm_FacebookCRM));
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions4 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions5 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions6 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions7 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions8 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance11 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered11 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed11 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled11 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions9 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance12 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered12 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed12 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled12 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions10 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance13 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered13 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed13 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled13 = new DevExpress.Utils.SerializableAppearanceObject();
		this.rep_cfl_CustomerID = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.rep_cfl_BPCode = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.rep_cfl_BPName = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.luk_Page = new DevExpress.XtraEditors.LookUpEdit();
		this.btnLogin = new Apzon.Controls.Controls.SimpleButtonEx();
		this.txtUserName = new Apzon.Controls.Editor.TextEditEx();
		this.btnActive = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnAccess = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_Inbox = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.replyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.hideToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.hideToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Inbox = new Apzon.Controls.Grid.GridViewEx();
		this.gcPageId = new Apzon.Controls.Grid.GridColumnEx();
		this.gcConversationId = new Apzon.Controls.Grid.GridColumnEx();
		this.gcConversationType = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Message_ThreadID = new Apzon.Controls.Grid.GridColumnEx();
		this.gcCustomerId = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Message_Content = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_cfl_Message = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gc_Message_CustomerName = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Message_CreatedTime = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_date_CreatedAt = new Apzon.Controls.Editor.RepositoryItemDateEditEx();
		this.gcOrderStatus = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_luk_OrderStatus = new Apzon.Forms.UserControls.RepositoryItemLookUpEditDefineNew();
		this.gc_Message_SaleOrder = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_cfl_SaleOrder = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gc_Message_Tag = new Apzon.Controls.Grid.GridColumnEx();
		this.gcNote = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDeleted = new Apzon.Controls.Grid.GridColumnEx();
		this.gcHided = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_check_Hided = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.rep_luk_Tag = new Apzon.Forms.UserControls.RepositoryItemLookUpEditDefineNew();
		this.xtraTabControl1 = new Apzon.Controls.Controls.XtraTabControlEx();
		this.tabpage_Customer = new Apzon.Controls.Controls.XtraTabPageEx();
		this.sp_CustomerInfo = new DevExpress.XtraEditors.SidePanel();
		this.xtraTabControl2 = new Apzon.Controls.Controls.XtraTabControlEx();
		this.tabpage_CustomerMessage = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_CustomerInbox = new Apzon.Controls.Grid.GridControlEx();
		this.gv_CustomerInbox = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx24 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx25 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx26 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx5 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx27 = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_CustomerMessage_Content = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx29 = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_CustomerMessage_CreatedAt = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_CustomerMessage_OrderStatus = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_CustomerMessage_SaleOrder = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_CustomerMessage_Tag = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_ccb_Tag = new Apzon.Forms.UserControls.RepositoryItemCheckedComboBoxEditDefineNew();
		this.gridColumnEx34 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx35 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx36 = new Apzon.Controls.Grid.GridColumnEx();
		this.tabpage_CustomerComment = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_CustomerComment = new Apzon.Controls.Grid.GridControlEx();
		this.gv_CustomerComment = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx9 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx32 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx37 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx31 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx39 = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_CustomerComment_Content = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_cfl_Comment = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx41 = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_CustomerComment_CreatedAt = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_CustomerComment_OrderStatus = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_CustomerComment_SaleOrder = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_CustomerComment_Tag = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx46 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx47 = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_CustomerComment_Hided = new Apzon.Controls.Grid.GridColumnEx();
		this.sp_CustomerInfo_Top = new DevExpress.XtraEditors.SidePanel();
		this.lblName = new DevExpress.XtraEditors.LabelControl();
		this.cfl_BPName = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.cfl_BPCode = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.labelLine12 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine11 = new Apzon.Controls.Controls.LabelLine();
		this.listOrderStatus = new System.Windows.Forms.ListView();
		this.lblLastContact = new DevExpress.XtraEditors.LabelControl();
		this.sp_CustomerList = new DevExpress.XtraEditors.SidePanel();
		this.grd_Customer = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Customer = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx38 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx14 = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Customer_CustomerName = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Customer_BPCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Customer_BPName = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Customer_LastContact = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx18 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx19 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx20 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx21 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx22 = new Apzon.Controls.Grid.GridColumnEx();
		this.tabpage_Message = new Apzon.Controls.Controls.XtraTabPageEx();
		this.tabpage_Comment = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_Comment = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Comment = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx10 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Comment_Content = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Comment_CustomerName = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Comment_CreatedAt = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Comment_OrderStatus = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Comment_SaleOrder = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Comment_Tag = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx11 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx12 = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Comment_Hided = new Apzon.Controls.Grid.GridColumnEx();
		this.btnSwitch = new Apzon.Controls.Controls.SimpleButtonEx();
		this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
		this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
		this.btnOK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.timer1 = new System.Windows.Forms.Timer(this.components);
		this.btnSearch = new Apzon.Controls.Controls.SimpleButtonEx();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine5 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine6 = new Apzon.Controls.Controls.LabelLine();
		this.cfl_ItemCode_Search = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.cfl_BPCode_Search = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.btnClear = new Apzon.Controls.Controls.SimpleButtonEx();
		this.date_From = new Apzon.Controls.Editor.DateEditEx();
		this.date_To = new Apzon.Controls.Editor.DateEditEx();
		this.labelLine7 = new Apzon.Controls.Controls.LabelLine();
		this.ccb_OrderStatus = new DevExpress.XtraEditors.CheckedComboBoxEdit();
		this.btnCheck = new Apzon.Controls.Controls.SimpleButtonEx();
		this.labelLine8 = new Apzon.Controls.Controls.LabelLine();
		this.ccb_Tag = new DevExpress.XtraEditors.CheckedComboBoxEdit();
		this.tabpage_Login = new Apzon.Controls.Controls.XtraTabPageEx();
		this.xtraTabControl3 = new Apzon.Controls.Controls.XtraTabControlEx();
		this.tabpage_Search = new Apzon.Controls.Controls.XtraTabPageEx();
		this.checkEdit2 = new Apzon.Controls.Editor.CheckEditEx();
		this.checkEdit1 = new Apzon.Controls.Editor.CheckEditEx();
		this.tabpage_Reply = new Apzon.Controls.Controls.XtraTabPageEx();
		this.txtCustomerContent = new Apzon.Controls.Editor.TextEditEx();
		this.txtCustomerName = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine10 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine9 = new Apzon.Controls.Controls.LabelLine();
		this.tabpage_Info = new Apzon.Controls.Controls.XtraTabPageEx();
		this.chkUnread = new Apzon.Controls.Editor.CheckEditEx();
		this.panel1 = new System.Windows.Forms.Panel();
		this.btnReply = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnReplyComment = new Apzon.Controls.Controls.SimpleButtonEx();
		this.luk_QuickReply = new Apzon.Forms.UserControls.LookUpEditDefineNew();
		this.memo_ReplyContent = new Apzon.Controls.Editor.MemoEditEx();
		this.splitContainer1 = new System.Windows.Forms.SplitContainer();
		((System.ComponentModel.ISupportInitialize)this.rep_cfl_CustomerID).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_cfl_BPCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_cfl_BPName).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Page.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtUserName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Inbox).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Inbox).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_cfl_Message).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_date_CreatedAt).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_date_CreatedAt.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_luk_OrderStatus).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_cfl_SaleOrder).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_check_Hided).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_luk_Tag).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).BeginInit();
		this.xtraTabControl1.SuspendLayout();
		this.tabpage_Customer.SuspendLayout();
		this.sp_CustomerInfo.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl2).BeginInit();
		this.xtraTabControl2.SuspendLayout();
		this.tabpage_CustomerMessage.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_CustomerInbox).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_CustomerInbox).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ccb_Tag).BeginInit();
		this.tabpage_CustomerComment.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_CustomerComment).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_CustomerComment).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_cfl_Comment).BeginInit();
		this.sp_CustomerInfo_Top.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.cfl_BPName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cfl_BPCode.Properties).BeginInit();
		this.sp_CustomerList.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Customer).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Customer).BeginInit();
		this.tabpage_Message.SuspendLayout();
		this.tabpage_Comment.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Comment).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Comment).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cfl_ItemCode_Search.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cfl_BPCode_Search.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_From.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_From.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_To.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_To.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ccb_OrderStatus.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ccb_Tag.Properties).BeginInit();
		this.tabpage_Login.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl3).BeginInit();
		this.xtraTabControl3.SuspendLayout();
		this.tabpage_Search.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.checkEdit2.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.checkEdit1.Properties).BeginInit();
		this.tabpage_Reply.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txtCustomerContent.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtCustomerName.Properties).BeginInit();
		this.tabpage_Info.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.chkUnread.Properties).BeginInit();
		this.panel1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.luk_QuickReply.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.memo_ReplyContent.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainer1).BeginInit();
		this.splitContainer1.Panel1.SuspendLayout();
		this.splitContainer1.Panel2.SuspendLayout();
		this.splitContainer1.SuspendLayout();
		base.SuspendLayout();
		this.rep_cfl_CustomerID.AutoHeight = false;
		this.rep_cfl_CustomerID.AutoLoadDataSource = true;
		editorButtonImageOptions.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions2.Image");
		this.rep_cfl_CustomerID.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, false, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_cfl_CustomerID.ContainerControl = this;
		this.rep_cfl_CustomerID.DisplayMember = "";
		this.rep_cfl_CustomerID.DisplayMember2 = "";
		this.rep_cfl_CustomerID.LoadDataSourceWhenEnter = false;
		this.rep_cfl_CustomerID.Name = "rep_cfl_CustomerID";
		this.rep_cfl_CustomerID.ReadOnly = true;
		this.rep_cfl_CustomerID.SeparateCharacterForMulti = ';';
		this.rep_cfl_CustomerID.SeparateString = ", ";
		this.rep_cfl_CustomerID.UsingAutoCompleteBox = false;
		this.rep_cfl_CustomerID.UsingDataSource = true;
		this.rep_cfl_CustomerID.ValidateWhenNoDataSource = false;
		this.rep_cfl_CustomerID.ValueMember = "";
		this.rep_cfl_BPCode.AutoHeight = false;
		this.rep_cfl_BPCode.AutoLoadDataSource = true;
		editorButtonImageOptions2.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions9.Image");
		this.rep_cfl_BPCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_cfl_BPCode.ContainerControl = this;
		this.rep_cfl_BPCode.DisplayMember = "";
		this.rep_cfl_BPCode.DisplayMember2 = "";
		this.rep_cfl_BPCode.LoadDataSourceWhenEnter = false;
		this.rep_cfl_BPCode.Name = "rep_cfl_BPCode";
		this.rep_cfl_BPCode.SeparateCharacterForMulti = ';';
		this.rep_cfl_BPCode.SeparateString = ", ";
		this.rep_cfl_BPCode.UsingAutoCompleteBox = false;
		this.rep_cfl_BPCode.UsingDataSource = true;
		this.rep_cfl_BPCode.ValidateWhenNoDataSource = false;
		this.rep_cfl_BPCode.ValueMember = "";
		this.rep_cfl_BPName.AutoHeight = false;
		this.rep_cfl_BPName.AutoLoadDataSource = true;
		editorButtonImageOptions3.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions10.Image");
		this.rep_cfl_BPName.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, false, true, editorButtonImageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance4, appearanceHovered4, appearancePressed4, appearanceDisabled4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_cfl_BPName.ContainerControl = this;
		this.rep_cfl_BPName.DisplayMember = "";
		this.rep_cfl_BPName.DisplayMember2 = "";
		this.rep_cfl_BPName.LoadDataSourceWhenEnter = false;
		this.rep_cfl_BPName.Name = "rep_cfl_BPName";
		this.rep_cfl_BPName.SeparateCharacterForMulti = ';';
		this.rep_cfl_BPName.SeparateString = ", ";
		this.rep_cfl_BPName.UsingAutoCompleteBox = false;
		this.rep_cfl_BPName.UsingDataSource = true;
		this.rep_cfl_BPName.ValidateWhenNoDataSource = false;
		this.rep_cfl_BPName.ValueMember = "";
		this.luk_Page.Location = new System.Drawing.Point(42, 33);
		this.luk_Page.Name = "luk_Page";
		this.luk_Page.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Page.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Id", "Name1"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name2")
		});
		this.luk_Page.Properties.NullText = "";
		this.luk_Page.Size = new System.Drawing.Size(150, 20);
		this.luk_Page.TabIndex = 13;
		this.luk_Page.EditValueChanged += new System.EventHandler(this.luk_Page_EditValueChanged);
		this.btnLogin.EditControl = null;
		this.btnLogin.EditValue = "Login";
		this.btnLogin.FieldName = null;
		this.btnLogin.IsSearchControl = false;
		this.btnLogin.Location = new System.Drawing.Point(198, 4);
		this.btnLogin.Name = "btnLogin";
		this.btnLogin.ReadOnly = false;
		this.btnLogin.SettingInVisible = false;
		this.btnLogin.SettingReadonly = false;
		this.btnLogin.Size = new System.Drawing.Size(61, 21);
		this.btnLogin.TabIndex = 14;
		this.btnLogin.Text = "Login";
		this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
		this.txtUserName.EditControl = null;
		this.txtUserName.FieldName = null;
		this.txtUserName.IsSearchControl = false;
		this.txtUserName.Location = new System.Drawing.Point(42, 6);
		this.txtUserName.Name = "txtUserName";
		this.txtUserName.Properties.ReadOnly = true;
		this.txtUserName.SettingInVisible = false;
		this.txtUserName.SettingReadonly = false;
		this.txtUserName.Size = new System.Drawing.Size(150, 20);
		this.txtUserName.TabIndex = 15;
		this.btnActive.EditControl = null;
		this.btnActive.EditValue = "Active";
		this.btnActive.Enabled = false;
		this.btnActive.FieldName = null;
		this.btnActive.IsSearchControl = false;
		this.btnActive.Location = new System.Drawing.Point(265, 31);
		this.btnActive.Name = "btnActive";
		this.btnActive.ReadOnly = false;
		this.btnActive.SettingInVisible = false;
		this.btnActive.SettingReadonly = false;
		this.btnActive.Size = new System.Drawing.Size(61, 21);
		this.btnActive.TabIndex = 17;
		this.btnActive.Text = "Active";
		this.btnActive.Click += new System.EventHandler(this.btnActive_Click);
		this.btnAccess.EditControl = null;
		this.btnAccess.EditValue = "Access";
		this.btnAccess.Enabled = false;
		this.btnAccess.FieldName = null;
		this.btnAccess.IsSearchControl = false;
		this.btnAccess.Location = new System.Drawing.Point(198, 31);
		this.btnAccess.Name = "btnAccess";
		this.btnAccess.ReadOnly = false;
		this.btnAccess.SettingInVisible = false;
		this.btnAccess.SettingReadonly = false;
		this.btnAccess.Size = new System.Drawing.Size(61, 21);
		this.btnAccess.TabIndex = 18;
		this.btnAccess.Text = "Access";
		this.btnAccess.Click += new System.EventHandler(this.btnAccess_Click);
		this.grd_Inbox.AllowPasteExcel = false;
		this.grd_Inbox.ClipboardData = null;
		this.grd_Inbox.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_Inbox.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Inbox.Location = new System.Drawing.Point(0, 0);
		this.grd_Inbox.MainView = this.gv_Inbox;
		this.grd_Inbox.Name = "grd_Inbox";
		this.grd_Inbox.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[6] { this.rep_cfl_SaleOrder, this.rep_luk_OrderStatus, this.rep_luk_Tag, this.rep_cfl_Message, this.rep_date_CreatedAt, this.rep_check_Hided });
		this.grd_Inbox.Size = new System.Drawing.Size(929, 425);
		this.grd_Inbox.TabIndex = 19;
		this.grd_Inbox.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Inbox });
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.replyToolStripMenuItem, this.hideToolStripMenuItem, this.hideToolStripMenuItem1 });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(138, 70);
		this.replyToolStripMenuItem.Name = "replyToolStripMenuItem";
		this.replyToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
		this.replyToolStripMenuItem.Text = "Reply";
		this.hideToolStripMenuItem.Name = "hideToolStripMenuItem";
		this.hideToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
		this.hideToolStripMenuItem.Text = "Quick Reply";
		this.hideToolStripMenuItem1.Name = "hideToolStripMenuItem1";
		this.hideToolStripMenuItem1.Size = new System.Drawing.Size(137, 22);
		this.hideToolStripMenuItem1.Text = "Hide";
		this.hideToolStripMenuItem1.Click += new System.EventHandler(this.hideToolStripMenuItem1_Click);
		this.gv_Inbox.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Inbox.ColumnKey = null;
		this.gv_Inbox.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[14]
		{
			this.gcPageId, this.gcConversationId, this.gcConversationType, this.gc_Message_ThreadID, this.gcCustomerId, this.gc_Message_Content, this.gc_Message_CustomerName, this.gc_Message_CreatedTime, this.gcOrderStatus, this.gc_Message_SaleOrder,
			this.gc_Message_Tag, this.gcNote, this.gcDeleted, this.gcHided
		});
		this.gv_Inbox.DisableFormSetting = false;
		this.gv_Inbox.DisplayException = false;
		this.gv_Inbox.EditColumn = null;
		this.gv_Inbox.FieldCurrency = null;
		this.gv_Inbox.FieldRate = null;
		this.gv_Inbox.FormId = "oFacebookCrm";
		this.gv_Inbox.GridControl = this.grd_Inbox;
		this.gv_Inbox.IgnoreFilter = false;
		this.gv_Inbox.IndexEditRow = -1;
		this.gv_Inbox.IndicatorWidth = 35;
		this.gv_Inbox.IsApzTable = false;
		this.gv_Inbox.IsInsertNewRow = false;
		this.gv_Inbox.IsLoadingData = false;
		this.gv_Inbox.IsMappingContextAction = false;
		this.gv_Inbox.ItemId = "OINBOX";
		this.gv_Inbox.LstColumnAllowBatch = null;
		this.gv_Inbox.LstColumnNotAllowBatch = null;
		this.gv_Inbox.LstColumnNotEditable = null;
		this.gv_Inbox.LstColumNotSetting = null;
		this.gv_Inbox.Name = "gv_Inbox";
		this.gv_Inbox.OptionsBehavior.AutoExpandAllGroups = true;
		this.gv_Inbox.OptionsView.ShowGroupPanel = false;
		this.gv_Inbox.UseBatchMod = false;
		this.gv_Inbox.CustomDrawGroupRow += new DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventHandler(this.gv_Inbox_CustomDrawGroupRow);
		this.gv_Inbox.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Inbox_RowCellStyle);
		this.gv_Inbox.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_CustomerInbox_FocusedRowObjectChanged);
		this.gv_Inbox.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Inbox_CellValueChanging);
		this.gv_Inbox.CustomRowFilter += new DevExpress.XtraGrid.Views.Base.RowFilterEventHandler(this.gv_Inbox_CustomRowFilter);
		this.gv_Inbox.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.gv_Customer_CustomColumnDisplayText);
		this.gcPageId.Caption = "Page Id";
		this.gcPageId.CurrencyDateField = null;
		this.gcPageId.CurrencyField = null;
		this.gcPageId.CurrencyFixed = null;
		this.gcPageId.CurrencyFixedRate = 0.0;
		this.gcPageId.CurrencyRateField = null;
		this.gcPageId.FieldName = "PageID";
		this.gcPageId.Name = "gcPageId";
		this.gcPageId.OptionsColumn.ReadOnly = true;
		this.gcPageId.PrimaryKey = false;
		this.gcPageId.UnitField = null;
		this.gcConversationId.Caption = "Message Id";
		this.gcConversationId.CurrencyDateField = null;
		this.gcConversationId.CurrencyField = null;
		this.gcConversationId.CurrencyFixed = null;
		this.gcConversationId.CurrencyFixedRate = 0.0;
		this.gcConversationId.CurrencyRateField = null;
		this.gcConversationId.FieldName = "ConversationID";
		this.gcConversationId.Name = "gcConversationId";
		this.gcConversationId.OptionsColumn.ReadOnly = true;
		this.gcConversationId.PrimaryKey = false;
		this.gcConversationId.UnitField = null;
		this.gcConversationType.Caption = "Conversation Type";
		this.gcConversationType.CurrencyDateField = null;
		this.gcConversationType.CurrencyField = null;
		this.gcConversationType.CurrencyFixed = null;
		this.gcConversationType.CurrencyFixedRate = 0.0;
		this.gcConversationType.CurrencyRateField = null;
		this.gcConversationType.FieldName = "ConversationType";
		this.gcConversationType.Name = "gcConversationType";
		this.gcConversationType.OptionsColumn.ReadOnly = true;
		this.gcConversationType.PrimaryKey = false;
		this.gcConversationType.UnitField = null;
		this.gcConversationType.Width = 55;
		this.gc_Message_ThreadID.Caption = "ThreadID";
		this.gc_Message_ThreadID.CurrencyDateField = null;
		this.gc_Message_ThreadID.CurrencyField = null;
		this.gc_Message_ThreadID.CurrencyFixed = null;
		this.gc_Message_ThreadID.CurrencyFixedRate = 0.0;
		this.gc_Message_ThreadID.CurrencyRateField = null;
		this.gc_Message_ThreadID.FieldName = "ThreadID";
		this.gc_Message_ThreadID.Name = "gc_Message_ThreadID";
		this.gc_Message_ThreadID.OptionsColumn.ReadOnly = true;
		this.gc_Message_ThreadID.PrimaryKey = false;
		this.gc_Message_ThreadID.UnitField = null;
		this.gcCustomerId.Caption = "Customer Id";
		this.gcCustomerId.CurrencyDateField = null;
		this.gcCustomerId.CurrencyField = null;
		this.gcCustomerId.CurrencyFixed = null;
		this.gcCustomerId.CurrencyFixedRate = 0.0;
		this.gcCustomerId.CurrencyRateField = null;
		this.gcCustomerId.FieldName = "CustomerID";
		this.gcCustomerId.Name = "gcCustomerId";
		this.gcCustomerId.OptionsColumn.ReadOnly = true;
		this.gcCustomerId.PrimaryKey = false;
		this.gcCustomerId.UnitField = null;
		this.gcCustomerId.Width = 81;
		this.gc_Message_Content.Caption = "Content";
		this.gc_Message_Content.ColumnEdit = this.rep_cfl_Message;
		this.gc_Message_Content.CurrencyDateField = null;
		this.gc_Message_Content.CurrencyField = null;
		this.gc_Message_Content.CurrencyFixed = null;
		this.gc_Message_Content.CurrencyFixedRate = 0.0;
		this.gc_Message_Content.CurrencyRateField = null;
		this.gc_Message_Content.FieldName = "Content";
		this.gc_Message_Content.Name = "gc_Message_Content";
		this.gc_Message_Content.OptionsColumn.ReadOnly = true;
		this.gc_Message_Content.PrimaryKey = false;
		this.gc_Message_Content.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowForFocusedRow;
		this.gc_Message_Content.UnitField = null;
		this.gc_Message_Content.Visible = true;
		this.gc_Message_Content.VisibleIndex = 1;
		this.gc_Message_Content.Width = 289;
		this.rep_cfl_Message.AutoHeight = false;
		this.rep_cfl_Message.AutoLoadDataSource = true;
		editorButtonImageOptions4.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions12.Image");
		this.rep_cfl_Message.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, false, false, imageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance5, appearanceHovered5, appearancePressed5, appearanceDisabled5, "", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance6, appearanceHovered6, appearancePressed6, appearanceDisabled6, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_cfl_Message.ContainerControl = this;
		this.rep_cfl_Message.DisplayMember = "";
		this.rep_cfl_Message.DisplayMember2 = "";
		this.rep_cfl_Message.LoadDataSourceWhenEnter = false;
		this.rep_cfl_Message.Name = "rep_cfl_Message";
		this.rep_cfl_Message.ReadOnly = true;
		this.rep_cfl_Message.SeparateCharacterForMulti = ';';
		this.rep_cfl_Message.SeparateString = ", ";
		this.rep_cfl_Message.UsingAutoCompleteBox = false;
		this.rep_cfl_Message.UsingDataSource = true;
		this.rep_cfl_Message.ValidateWhenNoDataSource = false;
		this.rep_cfl_Message.ValueMember = "";
		this.gc_Message_CustomerName.Caption = "From";
		this.gc_Message_CustomerName.CurrencyDateField = null;
		this.gc_Message_CustomerName.CurrencyField = null;
		this.gc_Message_CustomerName.CurrencyFixed = null;
		this.gc_Message_CustomerName.CurrencyFixedRate = 0.0;
		this.gc_Message_CustomerName.CurrencyRateField = null;
		this.gc_Message_CustomerName.FieldName = "CustomerName";
		this.gc_Message_CustomerName.Name = "gc_Message_CustomerName";
		this.gc_Message_CustomerName.OptionsColumn.ReadOnly = true;
		this.gc_Message_CustomerName.PrimaryKey = false;
		this.gc_Message_CustomerName.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowForFocusedRow;
		this.gc_Message_CustomerName.UnitField = null;
		this.gc_Message_CustomerName.Visible = true;
		this.gc_Message_CustomerName.VisibleIndex = 0;
		this.gc_Message_CustomerName.Width = 139;
		this.gc_Message_CreatedTime.Caption = "Created At";
		this.gc_Message_CreatedTime.ColumnEdit = this.rep_date_CreatedAt;
		this.gc_Message_CreatedTime.CurrencyDateField = null;
		this.gc_Message_CreatedTime.CurrencyField = null;
		this.gc_Message_CreatedTime.CurrencyFixed = null;
		this.gc_Message_CreatedTime.CurrencyFixedRate = 0.0;
		this.gc_Message_CreatedTime.CurrencyRateField = null;
		this.gc_Message_CreatedTime.FieldName = "CreatedTime";
		this.gc_Message_CreatedTime.Name = "gc_Message_CreatedTime";
		this.gc_Message_CreatedTime.OptionsColumn.ReadOnly = true;
		this.gc_Message_CreatedTime.PrimaryKey = false;
		this.gc_Message_CreatedTime.UnitField = null;
		this.gc_Message_CreatedTime.Visible = true;
		this.gc_Message_CreatedTime.VisibleIndex = 2;
		this.gc_Message_CreatedTime.Width = 134;
		this.rep_date_CreatedAt.AutoHeight = false;
		this.rep_date_CreatedAt.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_date_CreatedAt.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_date_CreatedAt.DisplayFormat.FormatString = "g";
		this.rep_date_CreatedAt.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.rep_date_CreatedAt.EditFormat.FormatString = "g";
		this.rep_date_CreatedAt.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.rep_date_CreatedAt.IsSearchControl = false;
		this.rep_date_CreatedAt.Mask.EditMask = null;
		this.rep_date_CreatedAt.Name = "rep_date_CreatedAt";
		this.gcOrderStatus.Caption = "Status";
		this.gcOrderStatus.ColumnEdit = this.rep_luk_OrderStatus;
		this.gcOrderStatus.CurrencyDateField = null;
		this.gcOrderStatus.CurrencyField = null;
		this.gcOrderStatus.CurrencyFixed = null;
		this.gcOrderStatus.CurrencyFixedRate = 0.0;
		this.gcOrderStatus.CurrencyRateField = null;
		this.gcOrderStatus.FieldName = "OrderStatus";
		this.gcOrderStatus.Name = "gcOrderStatus";
		this.gcOrderStatus.PrimaryKey = false;
		this.gcOrderStatus.UnitField = null;
		this.gcOrderStatus.Visible = true;
		this.gcOrderStatus.VisibleIndex = 3;
		this.gcOrderStatus.Width = 78;
		this.rep_luk_OrderStatus.AutoHeight = false;
		this.rep_luk_OrderStatus.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
		this.rep_luk_OrderStatus.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_luk_OrderStatus.CheckFields = null;
		this.rep_luk_OrderStatus.CheckTables = null;
		this.rep_luk_OrderStatus.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Id", "Name3", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name4")
		});
		this.rep_luk_OrderStatus.Name = "rep_luk_OrderStatus";
		this.rep_luk_OrderStatus.NullText = "";
		this.rep_luk_OrderStatus.ShowFooter = false;
		this.rep_luk_OrderStatus.ShowHeader = false;
		this.gc_Message_SaleOrder.Caption = "Sale Order";
		this.gc_Message_SaleOrder.ColumnEdit = this.rep_cfl_SaleOrder;
		this.gc_Message_SaleOrder.CurrencyDateField = null;
		this.gc_Message_SaleOrder.CurrencyField = null;
		this.gc_Message_SaleOrder.CurrencyFixed = null;
		this.gc_Message_SaleOrder.CurrencyFixedRate = 0.0;
		this.gc_Message_SaleOrder.CurrencyRateField = null;
		this.gc_Message_SaleOrder.FieldName = "DocEntry";
		this.gc_Message_SaleOrder.Name = "gc_Message_SaleOrder";
		this.gc_Message_SaleOrder.PrimaryKey = false;
		this.gc_Message_SaleOrder.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowForFocusedRow;
		this.gc_Message_SaleOrder.UnitField = null;
		this.gc_Message_SaleOrder.Visible = true;
		this.gc_Message_SaleOrder.VisibleIndex = 4;
		this.gc_Message_SaleOrder.Width = 77;
		this.rep_cfl_SaleOrder.AutoHeight = false;
		this.rep_cfl_SaleOrder.AutoLoadDataSource = true;
		editorButtonImageOptions5.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_cfl_SaleOrder.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions5, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance7, appearanceHovered7, appearancePressed7, appearanceDisabled7, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_cfl_SaleOrder.ContainerControl = this;
		this.rep_cfl_SaleOrder.DisplayMember = "";
		this.rep_cfl_SaleOrder.DisplayMember2 = "";
		this.rep_cfl_SaleOrder.LoadDataSourceWhenEnter = false;
		this.rep_cfl_SaleOrder.Name = "rep_cfl_SaleOrder";
		this.rep_cfl_SaleOrder.SeparateCharacterForMulti = ';';
		this.rep_cfl_SaleOrder.SeparateString = ", ";
		this.rep_cfl_SaleOrder.UsingAutoCompleteBox = false;
		this.rep_cfl_SaleOrder.UsingDataSource = true;
		this.rep_cfl_SaleOrder.ValidateWhenNoDataSource = false;
		this.rep_cfl_SaleOrder.ValueMember = "";
		this.gc_Message_Tag.Caption = "Tag";
		this.gc_Message_Tag.CurrencyDateField = null;
		this.gc_Message_Tag.CurrencyField = null;
		this.gc_Message_Tag.CurrencyFixed = null;
		this.gc_Message_Tag.CurrencyFixedRate = 0.0;
		this.gc_Message_Tag.CurrencyRateField = null;
		this.gc_Message_Tag.FieldName = "Tag";
		this.gc_Message_Tag.Name = "gc_Message_Tag";
		this.gc_Message_Tag.PrimaryKey = false;
		this.gc_Message_Tag.UnitField = null;
		this.gc_Message_Tag.Visible = true;
		this.gc_Message_Tag.VisibleIndex = 5;
		this.gc_Message_Tag.Width = 79;
		this.gcNote.Caption = "Note";
		this.gcNote.CurrencyDateField = null;
		this.gcNote.CurrencyField = null;
		this.gcNote.CurrencyFixed = null;
		this.gcNote.CurrencyFixedRate = 0.0;
		this.gcNote.CurrencyRateField = null;
		this.gcNote.FieldName = "Note";
		this.gcNote.Name = "gcNote";
		this.gcNote.PrimaryKey = false;
		this.gcNote.UnitField = null;
		this.gcNote.Width = 117;
		this.gcDeleted.Caption = "Deleted";
		this.gcDeleted.CurrencyDateField = null;
		this.gcDeleted.CurrencyField = null;
		this.gcDeleted.CurrencyFixed = null;
		this.gcDeleted.CurrencyFixedRate = 0.0;
		this.gcDeleted.CurrencyRateField = null;
		this.gcDeleted.FieldName = "Deleted";
		this.gcDeleted.Name = "gcDeleted";
		this.gcDeleted.PrimaryKey = false;
		this.gcDeleted.UnitField = null;
		this.gcDeleted.Width = 70;
		this.gcHided.Caption = "Hided";
		this.gcHided.ColumnEdit = this.rep_check_Hided;
		this.gcHided.CurrencyDateField = null;
		this.gcHided.CurrencyField = null;
		this.gcHided.CurrencyFixed = null;
		this.gcHided.CurrencyFixedRate = 0.0;
		this.gcHided.CurrencyRateField = null;
		this.gcHided.FieldName = "Hided";
		this.gcHided.Name = "gcHided";
		this.gcHided.PrimaryKey = false;
		this.gcHided.UnitField = null;
		this.gcHided.Width = 69;
		this.rep_check_Hided.AutoHeight = false;
		this.rep_check_Hided.Name = "rep_check_Hided";
		this.rep_check_Hided.ValueChecked = "Y";
		this.rep_check_Hided.ValueUnchecked = "N";
		this.rep_check_Hided.CheckedChanged += new System.EventHandler(this.rep_check_Hided_CheckedChanged);
		this.rep_luk_Tag.AutoHeight = false;
		this.rep_luk_Tag.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
		this.rep_luk_Tag.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_luk_Tag.CheckFields = null;
		this.rep_luk_Tag.CheckTables = null;
		this.rep_luk_Tag.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name2")
		});
		this.rep_luk_Tag.Name = "rep_luk_Tag";
		this.rep_luk_Tag.NullText = "";
		this.rep_luk_Tag.ShowFooter = false;
		this.rep_luk_Tag.ShowHeader = false;
		this.xtraTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.xtraTabControl1.HeaderLocation = DevExpress.XtraTab.TabHeaderLocation.Left;
		this.xtraTabControl1.IsSearchControl = false;
		this.xtraTabControl1.Location = new System.Drawing.Point(0, 0);
		this.xtraTabControl1.Name = "xtraTabControl1";
		this.xtraTabControl1.SelectedTabPage = this.tabpage_Customer;
		this.xtraTabControl1.ShowTabHeader = DevExpress.Utils.DefaultBoolean.True;
		this.xtraTabControl1.Size = new System.Drawing.Size(955, 427);
		this.xtraTabControl1.TabIndex = 20;
		this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[3] { this.tabpage_Customer, this.tabpage_Message, this.tabpage_Comment });
		this.xtraTabControl1.SelectedPageChanged += new DevExpress.XtraTab.TabPageChangedEventHandler(this.xtraTabControl1_SelectedPageChanged);
		this.xtraTabControl1.SelectedPageChanging += new DevExpress.XtraTab.TabPageChangingEventHandler(this.xtraTabControl1_SelectedPageChanging);
		this.tabpage_Customer.Controls.Add(this.sp_CustomerInfo);
		this.tabpage_Customer.Controls.Add(this.sp_CustomerList);
		this.tabpage_Customer.EditControl = null;
		this.tabpage_Customer.EditValue = "Customers";
		this.tabpage_Customer.FieldName = null;
		this.tabpage_Customer.IsSearchControl = false;
		this.tabpage_Customer.Name = "tabpage_Customer";
		this.tabpage_Customer.ReadOnly = false;
		this.tabpage_Customer.SettingInVisible = false;
		this.tabpage_Customer.SettingReadonly = false;
		this.tabpage_Customer.Size = new System.Drawing.Size(929, 425);
		this.tabpage_Customer.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tabpage_Customer.Text = "Customers";
		this.sp_CustomerInfo.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.sp_CustomerInfo.Controls.Add(this.xtraTabControl2);
		this.sp_CustomerInfo.Controls.Add(this.sp_CustomerInfo_Top);
		this.sp_CustomerInfo.Location = new System.Drawing.Point(216, 0);
		this.sp_CustomerInfo.Name = "sp_CustomerInfo";
		this.sp_CustomerInfo.Size = new System.Drawing.Size(713, 430);
		this.sp_CustomerInfo.TabIndex = 30;
		this.sp_CustomerInfo.Text = "sidePanel2";
		this.xtraTabControl2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.xtraTabControl2.IsSearchControl = false;
		this.xtraTabControl2.Location = new System.Drawing.Point(0, 104);
		this.xtraTabControl2.Name = "xtraTabControl2";
		this.xtraTabControl2.SelectedTabPage = this.tabpage_CustomerMessage;
		this.xtraTabControl2.Size = new System.Drawing.Size(713, 326);
		this.xtraTabControl2.TabIndex = 22;
		this.xtraTabControl2.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[2] { this.tabpage_CustomerMessage, this.tabpage_CustomerComment });
		this.xtraTabControl2.SelectedPageChanged += new DevExpress.XtraTab.TabPageChangedEventHandler(this.xtraTabControl2_SelectedPageChanged);
		this.tabpage_CustomerMessage.Controls.Add(this.grd_CustomerInbox);
		this.tabpage_CustomerMessage.EditControl = null;
		this.tabpage_CustomerMessage.EditValue = "Messages";
		this.tabpage_CustomerMessage.FieldName = null;
		this.tabpage_CustomerMessage.IsSearchControl = false;
		this.tabpage_CustomerMessage.Name = "tabpage_CustomerMessage";
		this.tabpage_CustomerMessage.ReadOnly = false;
		this.tabpage_CustomerMessage.SettingInVisible = false;
		this.tabpage_CustomerMessage.SettingReadonly = false;
		this.tabpage_CustomerMessage.Size = new System.Drawing.Size(711, 301);
		this.tabpage_CustomerMessage.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tabpage_CustomerMessage.Text = "Messages";
		this.grd_CustomerInbox.AllowPasteExcel = true;
		this.grd_CustomerInbox.ClipboardData = null;
		this.grd_CustomerInbox.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_CustomerInbox.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_CustomerInbox.Location = new System.Drawing.Point(0, 0);
		this.grd_CustomerInbox.MainView = this.gv_CustomerInbox;
		this.grd_CustomerInbox.Name = "grd_CustomerInbox";
		this.grd_CustomerInbox.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.rep_ccb_Tag });
		this.grd_CustomerInbox.Size = new System.Drawing.Size(711, 301);
		this.grd_CustomerInbox.TabIndex = 20;
		this.grd_CustomerInbox.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_CustomerInbox });
		this.gv_CustomerInbox.Appearance.GroupButton.ForeColor = System.Drawing.Color.White;
		this.gv_CustomerInbox.Appearance.GroupButton.Options.UseForeColor = true;
		this.gv_CustomerInbox.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_CustomerInbox.ColumnKey = null;
		this.gv_CustomerInbox.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[14]
		{
			this.gridColumnEx24, this.gridColumnEx25, this.gridColumnEx26, this.gridColumnEx5, this.gridColumnEx27, this.gc_CustomerMessage_Content, this.gridColumnEx29, this.gc_CustomerMessage_CreatedAt, this.gc_CustomerMessage_OrderStatus, this.gc_CustomerMessage_SaleOrder,
			this.gc_CustomerMessage_Tag, this.gridColumnEx34, this.gridColumnEx35, this.gridColumnEx36
		});
		this.gv_CustomerInbox.DisableFormSetting = false;
		this.gv_CustomerInbox.DisplayException = false;
		this.gv_CustomerInbox.EditColumn = null;
		this.gv_CustomerInbox.FieldCurrency = null;
		this.gv_CustomerInbox.FieldRate = null;
		this.gv_CustomerInbox.FormId = "oFacebookCrm";
		this.gv_CustomerInbox.GridControl = this.grd_CustomerInbox;
		this.gv_CustomerInbox.IgnoreFilter = false;
		this.gv_CustomerInbox.IndexEditRow = -1;
		this.gv_CustomerInbox.IndicatorWidth = 35;
		this.gv_CustomerInbox.IsApzTable = false;
		this.gv_CustomerInbox.IsInsertNewRow = false;
		this.gv_CustomerInbox.IsLoadingData = false;
		this.gv_CustomerInbox.IsMappingContextAction = false;
		this.gv_CustomerInbox.ItemId = "OCUSTOMERINBOX";
		this.gv_CustomerInbox.LstColumnAllowBatch = null;
		this.gv_CustomerInbox.LstColumnNotAllowBatch = null;
		this.gv_CustomerInbox.LstColumnNotEditable = null;
		this.gv_CustomerInbox.LstColumNotSetting = null;
		this.gv_CustomerInbox.Name = "gv_CustomerInbox";
		this.gv_CustomerInbox.OptionsBehavior.AutoExpandAllGroups = true;
		this.gv_CustomerInbox.OptionsView.ShowGroupPanel = false;
		this.gv_CustomerInbox.UseBatchMod = false;
		this.gv_CustomerInbox.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Inbox_RowCellStyle);
		this.gv_CustomerInbox.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_CustomerInbox_FocusedRowObjectChanged);
		this.gv_CustomerInbox.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_CustomerInbox_CellValueChanged);
		this.gv_CustomerInbox.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Inbox_CellValueChanging);
		this.gv_CustomerInbox.CustomRowFilter += new DevExpress.XtraGrid.Views.Base.RowFilterEventHandler(this.gv_CustomerInbox_CustomRowFilter);
		this.gv_CustomerInbox.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.gv_Customer_CustomColumnDisplayText);
		this.gridColumnEx24.Caption = "Page Id";
		this.gridColumnEx24.CurrencyDateField = null;
		this.gridColumnEx24.CurrencyField = null;
		this.gridColumnEx24.CurrencyFixed = null;
		this.gridColumnEx24.CurrencyFixedRate = 0.0;
		this.gridColumnEx24.CurrencyRateField = null;
		this.gridColumnEx24.FieldName = "PageID";
		this.gridColumnEx24.Name = "gridColumnEx24";
		this.gridColumnEx24.OptionsColumn.ReadOnly = true;
		this.gridColumnEx24.PrimaryKey = true;
		this.gridColumnEx24.UnitField = null;
		this.gridColumnEx25.Caption = "Message Id";
		this.gridColumnEx25.CurrencyDateField = null;
		this.gridColumnEx25.CurrencyField = null;
		this.gridColumnEx25.CurrencyFixed = null;
		this.gridColumnEx25.CurrencyFixedRate = 0.0;
		this.gridColumnEx25.CurrencyRateField = null;
		this.gridColumnEx25.FieldName = "ConversationID";
		this.gridColumnEx25.Name = "gridColumnEx25";
		this.gridColumnEx25.OptionsColumn.ReadOnly = true;
		this.gridColumnEx25.PrimaryKey = false;
		this.gridColumnEx25.UnitField = null;
		this.gridColumnEx26.Caption = "Conversation Type";
		this.gridColumnEx26.CurrencyDateField = null;
		this.gridColumnEx26.CurrencyField = null;
		this.gridColumnEx26.CurrencyFixed = null;
		this.gridColumnEx26.CurrencyFixedRate = 0.0;
		this.gridColumnEx26.CurrencyRateField = null;
		this.gridColumnEx26.FieldName = "ConversationType";
		this.gridColumnEx26.Name = "gridColumnEx26";
		this.gridColumnEx26.OptionsColumn.ReadOnly = true;
		this.gridColumnEx26.PrimaryKey = false;
		this.gridColumnEx26.UnitField = null;
		this.gridColumnEx26.Width = 55;
		this.gridColumnEx5.Caption = "ThreadId";
		this.gridColumnEx5.CurrencyDateField = null;
		this.gridColumnEx5.CurrencyField = null;
		this.gridColumnEx5.CurrencyFixed = null;
		this.gridColumnEx5.CurrencyFixedRate = 0.0;
		this.gridColumnEx5.CurrencyRateField = null;
		this.gridColumnEx5.FieldName = "ThreadID";
		this.gridColumnEx5.Name = "gridColumnEx5";
		this.gridColumnEx5.OptionsColumn.ReadOnly = true;
		this.gridColumnEx5.PrimaryKey = false;
		this.gridColumnEx5.UnitField = null;
		this.gridColumnEx27.Caption = "Customer Id";
		this.gridColumnEx27.CurrencyDateField = null;
		this.gridColumnEx27.CurrencyField = null;
		this.gridColumnEx27.CurrencyFixed = null;
		this.gridColumnEx27.CurrencyFixedRate = 0.0;
		this.gridColumnEx27.CurrencyRateField = null;
		this.gridColumnEx27.FieldName = "CustomerID";
		this.gridColumnEx27.Name = "gridColumnEx27";
		this.gridColumnEx27.OptionsColumn.ReadOnly = true;
		this.gridColumnEx27.PrimaryKey = false;
		this.gridColumnEx27.UnitField = null;
		this.gridColumnEx27.Width = 81;
		this.gc_CustomerMessage_Content.Caption = "Content";
		this.gc_CustomerMessage_Content.CurrencyDateField = null;
		this.gc_CustomerMessage_Content.CurrencyField = null;
		this.gc_CustomerMessage_Content.CurrencyFixed = null;
		this.gc_CustomerMessage_Content.CurrencyFixedRate = 0.0;
		this.gc_CustomerMessage_Content.CurrencyRateField = null;
		this.gc_CustomerMessage_Content.FieldName = "Content";
		this.gc_CustomerMessage_Content.Name = "gc_CustomerMessage_Content";
		this.gc_CustomerMessage_Content.OptionsColumn.ReadOnly = true;
		this.gc_CustomerMessage_Content.PrimaryKey = false;
		this.gc_CustomerMessage_Content.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowForFocusedRow;
		this.gc_CustomerMessage_Content.UnitField = null;
		this.gc_CustomerMessage_Content.Visible = true;
		this.gc_CustomerMessage_Content.VisibleIndex = 0;
		this.gc_CustomerMessage_Content.Width = 353;
		this.gridColumnEx29.Caption = "From";
		this.gridColumnEx29.CurrencyDateField = null;
		this.gridColumnEx29.CurrencyField = null;
		this.gridColumnEx29.CurrencyFixed = null;
		this.gridColumnEx29.CurrencyFixedRate = 0.0;
		this.gridColumnEx29.CurrencyRateField = null;
		this.gridColumnEx29.FieldName = "CustomerName";
		this.gridColumnEx29.Name = "gridColumnEx29";
		this.gridColumnEx29.OptionsColumn.ReadOnly = true;
		this.gridColumnEx29.PrimaryKey = false;
		this.gridColumnEx29.UnitField = null;
		this.gridColumnEx29.Width = 105;
		this.gc_CustomerMessage_CreatedAt.Caption = "Created At";
		this.gc_CustomerMessage_CreatedAt.CurrencyDateField = null;
		this.gc_CustomerMessage_CreatedAt.CurrencyField = null;
		this.gc_CustomerMessage_CreatedAt.CurrencyFixed = null;
		this.gc_CustomerMessage_CreatedAt.CurrencyFixedRate = 0.0;
		this.gc_CustomerMessage_CreatedAt.CurrencyRateField = null;
		this.gc_CustomerMessage_CreatedAt.FieldName = "CreatedTime";
		this.gc_CustomerMessage_CreatedAt.Name = "gc_CustomerMessage_CreatedAt";
		this.gc_CustomerMessage_CreatedAt.OptionsColumn.ReadOnly = true;
		this.gc_CustomerMessage_CreatedAt.PrimaryKey = false;
		this.gc_CustomerMessage_CreatedAt.UnitField = null;
		this.gc_CustomerMessage_CreatedAt.Visible = true;
		this.gc_CustomerMessage_CreatedAt.VisibleIndex = 1;
		this.gc_CustomerMessage_CreatedAt.Width = 130;
		this.gc_CustomerMessage_OrderStatus.Caption = "Status";
		this.gc_CustomerMessage_OrderStatus.CurrencyDateField = null;
		this.gc_CustomerMessage_OrderStatus.CurrencyField = null;
		this.gc_CustomerMessage_OrderStatus.CurrencyFixed = null;
		this.gc_CustomerMessage_OrderStatus.CurrencyFixedRate = 0.0;
		this.gc_CustomerMessage_OrderStatus.CurrencyRateField = null;
		this.gc_CustomerMessage_OrderStatus.FieldName = "OrderStatus";
		this.gc_CustomerMessage_OrderStatus.Name = "gc_CustomerMessage_OrderStatus";
		this.gc_CustomerMessage_OrderStatus.PrimaryKey = false;
		this.gc_CustomerMessage_OrderStatus.UnitField = null;
		this.gc_CustomerMessage_OrderStatus.Visible = true;
		this.gc_CustomerMessage_OrderStatus.VisibleIndex = 2;
		this.gc_CustomerMessage_OrderStatus.Width = 125;
		this.gc_CustomerMessage_SaleOrder.Caption = "Sale Order";
		this.gc_CustomerMessage_SaleOrder.CurrencyDateField = null;
		this.gc_CustomerMessage_SaleOrder.CurrencyField = null;
		this.gc_CustomerMessage_SaleOrder.CurrencyFixed = null;
		this.gc_CustomerMessage_SaleOrder.CurrencyFixedRate = 0.0;
		this.gc_CustomerMessage_SaleOrder.CurrencyRateField = null;
		this.gc_CustomerMessage_SaleOrder.FieldName = "DocEntry";
		this.gc_CustomerMessage_SaleOrder.Name = "gc_CustomerMessage_SaleOrder";
		this.gc_CustomerMessage_SaleOrder.PrimaryKey = false;
		this.gc_CustomerMessage_SaleOrder.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowForFocusedRow;
		this.gc_CustomerMessage_SaleOrder.UnitField = null;
		this.gc_CustomerMessage_SaleOrder.Visible = true;
		this.gc_CustomerMessage_SaleOrder.VisibleIndex = 3;
		this.gc_CustomerMessage_SaleOrder.Width = 92;
		this.gc_CustomerMessage_Tag.Caption = "Tag";
		this.gc_CustomerMessage_Tag.ColumnEdit = this.rep_ccb_Tag;
		this.gc_CustomerMessage_Tag.CurrencyDateField = null;
		this.gc_CustomerMessage_Tag.CurrencyField = null;
		this.gc_CustomerMessage_Tag.CurrencyFixed = null;
		this.gc_CustomerMessage_Tag.CurrencyFixedRate = 0.0;
		this.gc_CustomerMessage_Tag.CurrencyRateField = null;
		this.gc_CustomerMessage_Tag.FieldName = "Tag";
		this.gc_CustomerMessage_Tag.Name = "gc_CustomerMessage_Tag";
		this.gc_CustomerMessage_Tag.PrimaryKey = false;
		this.gc_CustomerMessage_Tag.UnitField = null;
		this.gc_CustomerMessage_Tag.Visible = true;
		this.gc_CustomerMessage_Tag.VisibleIndex = 4;
		this.gc_CustomerMessage_Tag.Width = 110;
		this.rep_ccb_Tag.AutoHeight = false;
		this.rep_ccb_Tag.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_ccb_Tag.CheckFields = null;
		this.rep_ccb_Tag.CheckTables = null;
		this.rep_ccb_Tag.DisableDefineNew = false;
		this.rep_ccb_Tag.IncrementalSearch = true;
		this.rep_ccb_Tag.Name = "rep_ccb_Tag";
		this.gridColumnEx34.Caption = "Note";
		this.gridColumnEx34.CurrencyDateField = null;
		this.gridColumnEx34.CurrencyField = null;
		this.gridColumnEx34.CurrencyFixed = null;
		this.gridColumnEx34.CurrencyFixedRate = 0.0;
		this.gridColumnEx34.CurrencyRateField = null;
		this.gridColumnEx34.FieldName = "Note";
		this.gridColumnEx34.Name = "gridColumnEx34";
		this.gridColumnEx34.PrimaryKey = false;
		this.gridColumnEx34.UnitField = null;
		this.gridColumnEx34.Width = 117;
		this.gridColumnEx35.Caption = "Deleted";
		this.gridColumnEx35.CurrencyDateField = null;
		this.gridColumnEx35.CurrencyField = null;
		this.gridColumnEx35.CurrencyFixed = null;
		this.gridColumnEx35.CurrencyFixedRate = 0.0;
		this.gridColumnEx35.CurrencyRateField = null;
		this.gridColumnEx35.FieldName = "Deleted";
		this.gridColumnEx35.Name = "gridColumnEx35";
		this.gridColumnEx35.OptionsColumn.ReadOnly = true;
		this.gridColumnEx35.PrimaryKey = false;
		this.gridColumnEx35.UnitField = null;
		this.gridColumnEx35.Width = 70;
		this.gridColumnEx36.Caption = "Hided";
		this.gridColumnEx36.CurrencyDateField = null;
		this.gridColumnEx36.CurrencyField = null;
		this.gridColumnEx36.CurrencyFixed = null;
		this.gridColumnEx36.CurrencyFixedRate = 0.0;
		this.gridColumnEx36.CurrencyRateField = null;
		this.gridColumnEx36.FieldName = "Hided";
		this.gridColumnEx36.Name = "gridColumnEx36";
		this.gridColumnEx36.OptionsColumn.ReadOnly = true;
		this.gridColumnEx36.PrimaryKey = false;
		this.gridColumnEx36.UnitField = null;
		this.gridColumnEx36.Width = 69;
		this.tabpage_CustomerComment.Controls.Add(this.grd_CustomerComment);
		this.tabpage_CustomerComment.EditControl = null;
		this.tabpage_CustomerComment.EditValue = "Comments";
		this.tabpage_CustomerComment.FieldName = null;
		this.tabpage_CustomerComment.IsSearchControl = false;
		this.tabpage_CustomerComment.Name = "tabpage_CustomerComment";
		this.tabpage_CustomerComment.ReadOnly = false;
		this.tabpage_CustomerComment.SettingInVisible = false;
		this.tabpage_CustomerComment.SettingReadonly = false;
		this.tabpage_CustomerComment.Size = new System.Drawing.Size(711, 301);
		this.tabpage_CustomerComment.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tabpage_CustomerComment.Text = "Comments";
		this.grd_CustomerComment.AllowPasteExcel = false;
		this.grd_CustomerComment.ClipboardData = null;
		this.grd_CustomerComment.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_CustomerComment.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_CustomerComment.Location = new System.Drawing.Point(0, 0);
		this.grd_CustomerComment.MainView = this.gv_CustomerComment;
		this.grd_CustomerComment.Name = "grd_CustomerComment";
		this.grd_CustomerComment.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.rep_cfl_Comment });
		this.grd_CustomerComment.Size = new System.Drawing.Size(711, 301);
		this.grd_CustomerComment.TabIndex = 21;
		this.grd_CustomerComment.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_CustomerComment });
		this.gv_CustomerComment.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_CustomerComment.ColumnKey = null;
		this.gv_CustomerComment.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[14]
		{
			this.gridColumnEx9, this.gridColumnEx32, this.gridColumnEx37, this.gridColumnEx31, this.gridColumnEx39, this.gc_CustomerComment_Content, this.gridColumnEx41, this.gc_CustomerComment_CreatedAt, this.gc_CustomerComment_OrderStatus, this.gc_CustomerComment_SaleOrder,
			this.gc_CustomerComment_Tag, this.gridColumnEx46, this.gridColumnEx47, this.gc_CustomerComment_Hided
		});
		this.gv_CustomerComment.DisableFormSetting = false;
		this.gv_CustomerComment.DisplayException = false;
		this.gv_CustomerComment.EditColumn = null;
		this.gv_CustomerComment.FieldCurrency = null;
		this.gv_CustomerComment.FieldRate = null;
		this.gv_CustomerComment.FormId = "oFacebookCrm";
		this.gv_CustomerComment.GridControl = this.grd_CustomerComment;
		this.gv_CustomerComment.IgnoreFilter = false;
		this.gv_CustomerComment.IndexEditRow = -1;
		this.gv_CustomerComment.IndicatorWidth = 35;
		this.gv_CustomerComment.IsApzTable = false;
		this.gv_CustomerComment.IsInsertNewRow = false;
		this.gv_CustomerComment.IsLoadingData = false;
		this.gv_CustomerComment.IsMappingContextAction = false;
		this.gv_CustomerComment.ItemId = "OCUSTOMERCOMMENT";
		this.gv_CustomerComment.LstColumnAllowBatch = null;
		this.gv_CustomerComment.LstColumnNotAllowBatch = null;
		this.gv_CustomerComment.LstColumnNotEditable = null;
		this.gv_CustomerComment.LstColumNotSetting = null;
		this.gv_CustomerComment.Name = "gv_CustomerComment";
		this.gv_CustomerComment.OptionsBehavior.AutoExpandAllGroups = true;
		this.gv_CustomerComment.OptionsView.ShowGroupPanel = false;
		this.gv_CustomerComment.UseBatchMod = false;
		this.gv_CustomerComment.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Inbox_RowCellStyle);
		this.gv_CustomerComment.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.gv_CustomerComment_RowStyle);
		this.gv_CustomerComment.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_CustomerInbox_FocusedRowObjectChanged);
		this.gv_CustomerComment.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_CustomerComment_CellValueChanged);
		this.gv_CustomerComment.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Inbox_CellValueChanging);
		this.gv_CustomerComment.CustomRowFilter += new DevExpress.XtraGrid.Views.Base.RowFilterEventHandler(this.gv_CustomerComment_CustomRowFilter);
		this.gv_CustomerComment.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.gv_Customer_CustomColumnDisplayText);
		this.gridColumnEx9.Caption = "Page Id";
		this.gridColumnEx9.CurrencyDateField = null;
		this.gridColumnEx9.CurrencyField = null;
		this.gridColumnEx9.CurrencyFixed = null;
		this.gridColumnEx9.CurrencyFixedRate = 0.0;
		this.gridColumnEx9.CurrencyRateField = null;
		this.gridColumnEx9.FieldName = "PageId";
		this.gridColumnEx9.Name = "gridColumnEx9";
		this.gridColumnEx9.OptionsColumn.ReadOnly = true;
		this.gridColumnEx9.PrimaryKey = false;
		this.gridColumnEx9.UnitField = null;
		this.gridColumnEx32.Caption = "Message Id";
		this.gridColumnEx32.CurrencyDateField = null;
		this.gridColumnEx32.CurrencyField = null;
		this.gridColumnEx32.CurrencyFixed = null;
		this.gridColumnEx32.CurrencyFixedRate = 0.0;
		this.gridColumnEx32.CurrencyRateField = null;
		this.gridColumnEx32.FieldName = "ConversationID";
		this.gridColumnEx32.Name = "gridColumnEx32";
		this.gridColumnEx32.OptionsColumn.ReadOnly = true;
		this.gridColumnEx32.PrimaryKey = false;
		this.gridColumnEx32.UnitField = null;
		this.gridColumnEx37.Caption = "Conversation Type";
		this.gridColumnEx37.CurrencyDateField = null;
		this.gridColumnEx37.CurrencyField = null;
		this.gridColumnEx37.CurrencyFixed = null;
		this.gridColumnEx37.CurrencyFixedRate = 0.0;
		this.gridColumnEx37.CurrencyRateField = null;
		this.gridColumnEx37.FieldName = "ConversationType";
		this.gridColumnEx37.Name = "gridColumnEx37";
		this.gridColumnEx37.OptionsColumn.ReadOnly = true;
		this.gridColumnEx37.PrimaryKey = false;
		this.gridColumnEx37.UnitField = null;
		this.gridColumnEx37.Width = 55;
		this.gridColumnEx31.Caption = "ThreadId";
		this.gridColumnEx31.CurrencyDateField = null;
		this.gridColumnEx31.CurrencyField = null;
		this.gridColumnEx31.CurrencyFixed = null;
		this.gridColumnEx31.CurrencyFixedRate = 0.0;
		this.gridColumnEx31.CurrencyRateField = null;
		this.gridColumnEx31.FieldName = "ThreadID";
		this.gridColumnEx31.Name = "gridColumnEx31";
		this.gridColumnEx31.PrimaryKey = false;
		this.gridColumnEx31.UnitField = null;
		this.gridColumnEx39.Caption = "Customer Id";
		this.gridColumnEx39.CurrencyDateField = null;
		this.gridColumnEx39.CurrencyField = null;
		this.gridColumnEx39.CurrencyFixed = null;
		this.gridColumnEx39.CurrencyFixedRate = 0.0;
		this.gridColumnEx39.CurrencyRateField = null;
		this.gridColumnEx39.FieldName = "CustomerId";
		this.gridColumnEx39.Name = "gridColumnEx39";
		this.gridColumnEx39.OptionsColumn.ReadOnly = true;
		this.gridColumnEx39.PrimaryKey = false;
		this.gridColumnEx39.UnitField = null;
		this.gridColumnEx39.Width = 81;
		this.gc_CustomerComment_Content.Caption = "Content";
		this.gc_CustomerComment_Content.ColumnEdit = this.rep_cfl_Comment;
		this.gc_CustomerComment_Content.CurrencyDateField = null;
		this.gc_CustomerComment_Content.CurrencyField = null;
		this.gc_CustomerComment_Content.CurrencyFixed = null;
		this.gc_CustomerComment_Content.CurrencyFixedRate = 0.0;
		this.gc_CustomerComment_Content.CurrencyRateField = null;
		this.gc_CustomerComment_Content.FieldName = "Content";
		this.gc_CustomerComment_Content.Name = "gc_CustomerComment_Content";
		this.gc_CustomerComment_Content.OptionsColumn.ReadOnly = true;
		this.gc_CustomerComment_Content.PrimaryKey = false;
		this.gc_CustomerComment_Content.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowForFocusedRow;
		this.gc_CustomerComment_Content.UnitField = null;
		this.gc_CustomerComment_Content.Visible = true;
		this.gc_CustomerComment_Content.VisibleIndex = 0;
		this.gc_CustomerComment_Content.Width = 227;
		this.rep_cfl_Comment.AutoHeight = false;
		this.rep_cfl_Comment.AutoLoadDataSource = true;
		editorButtonImageOptions6.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions6.Image");
		this.rep_cfl_Comment.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, false, false, imageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance8, appearanceHovered8, appearancePressed8, appearanceDisabled8, "", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions6, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance9, appearanceHovered9, appearancePressed9, appearanceDisabled9, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_cfl_Comment.ContainerControl = this;
		this.rep_cfl_Comment.DisplayMember = "";
		this.rep_cfl_Comment.DisplayMember2 = "";
		this.rep_cfl_Comment.LoadDataSourceWhenEnter = false;
		this.rep_cfl_Comment.Name = "rep_cfl_Comment";
		this.rep_cfl_Comment.ReadOnly = true;
		this.rep_cfl_Comment.SeparateCharacterForMulti = ';';
		this.rep_cfl_Comment.SeparateString = ", ";
		this.rep_cfl_Comment.UsingAutoCompleteBox = false;
		this.rep_cfl_Comment.UsingDataSource = true;
		this.rep_cfl_Comment.ValidateWhenNoDataSource = false;
		this.rep_cfl_Comment.ValueMember = "";
		this.gridColumnEx41.Caption = "From";
		this.gridColumnEx41.CurrencyDateField = null;
		this.gridColumnEx41.CurrencyField = null;
		this.gridColumnEx41.CurrencyFixed = null;
		this.gridColumnEx41.CurrencyFixedRate = 0.0;
		this.gridColumnEx41.CurrencyRateField = null;
		this.gridColumnEx41.FieldName = "CustomerName";
		this.gridColumnEx41.Name = "gridColumnEx41";
		this.gridColumnEx41.OptionsColumn.ReadOnly = true;
		this.gridColumnEx41.PrimaryKey = false;
		this.gridColumnEx41.UnitField = null;
		this.gridColumnEx41.Width = 105;
		this.gc_CustomerComment_CreatedAt.Caption = "Created At";
		this.gc_CustomerComment_CreatedAt.CurrencyDateField = null;
		this.gc_CustomerComment_CreatedAt.CurrencyField = null;
		this.gc_CustomerComment_CreatedAt.CurrencyFixed = null;
		this.gc_CustomerComment_CreatedAt.CurrencyFixedRate = 0.0;
		this.gc_CustomerComment_CreatedAt.CurrencyRateField = null;
		this.gc_CustomerComment_CreatedAt.FieldName = "CreatedTime";
		this.gc_CustomerComment_CreatedAt.Name = "gc_CustomerComment_CreatedAt";
		this.gc_CustomerComment_CreatedAt.OptionsColumn.ReadOnly = true;
		this.gc_CustomerComment_CreatedAt.PrimaryKey = false;
		this.gc_CustomerComment_CreatedAt.UnitField = null;
		this.gc_CustomerComment_CreatedAt.Visible = true;
		this.gc_CustomerComment_CreatedAt.VisibleIndex = 1;
		this.gc_CustomerComment_CreatedAt.Width = 100;
		this.gc_CustomerComment_OrderStatus.Caption = "Status";
		this.gc_CustomerComment_OrderStatus.CurrencyDateField = null;
		this.gc_CustomerComment_OrderStatus.CurrencyField = null;
		this.gc_CustomerComment_OrderStatus.CurrencyFixed = null;
		this.gc_CustomerComment_OrderStatus.CurrencyFixedRate = 0.0;
		this.gc_CustomerComment_OrderStatus.CurrencyRateField = null;
		this.gc_CustomerComment_OrderStatus.FieldName = "OrderStatus";
		this.gc_CustomerComment_OrderStatus.Name = "gc_CustomerComment_OrderStatus";
		this.gc_CustomerComment_OrderStatus.PrimaryKey = false;
		this.gc_CustomerComment_OrderStatus.UnitField = null;
		this.gc_CustomerComment_OrderStatus.Visible = true;
		this.gc_CustomerComment_OrderStatus.VisibleIndex = 2;
		this.gc_CustomerComment_OrderStatus.Width = 97;
		this.gc_CustomerComment_SaleOrder.Caption = "Sale Order";
		this.gc_CustomerComment_SaleOrder.CurrencyDateField = null;
		this.gc_CustomerComment_SaleOrder.CurrencyField = null;
		this.gc_CustomerComment_SaleOrder.CurrencyFixed = null;
		this.gc_CustomerComment_SaleOrder.CurrencyFixedRate = 0.0;
		this.gc_CustomerComment_SaleOrder.CurrencyRateField = null;
		this.gc_CustomerComment_SaleOrder.FieldName = "DocEntry";
		this.gc_CustomerComment_SaleOrder.Name = "gc_CustomerComment_SaleOrder";
		this.gc_CustomerComment_SaleOrder.PrimaryKey = false;
		this.gc_CustomerComment_SaleOrder.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowForFocusedRow;
		this.gc_CustomerComment_SaleOrder.UnitField = null;
		this.gc_CustomerComment_SaleOrder.Visible = true;
		this.gc_CustomerComment_SaleOrder.VisibleIndex = 3;
		this.gc_CustomerComment_SaleOrder.Width = 69;
		this.gc_CustomerComment_Tag.Caption = "Tag";
		this.gc_CustomerComment_Tag.CurrencyDateField = null;
		this.gc_CustomerComment_Tag.CurrencyField = null;
		this.gc_CustomerComment_Tag.CurrencyFixed = null;
		this.gc_CustomerComment_Tag.CurrencyFixedRate = 0.0;
		this.gc_CustomerComment_Tag.CurrencyRateField = null;
		this.gc_CustomerComment_Tag.FieldName = "Tag";
		this.gc_CustomerComment_Tag.Name = "gc_CustomerComment_Tag";
		this.gc_CustomerComment_Tag.PrimaryKey = false;
		this.gc_CustomerComment_Tag.UnitField = null;
		this.gc_CustomerComment_Tag.Visible = true;
		this.gc_CustomerComment_Tag.VisibleIndex = 4;
		this.gc_CustomerComment_Tag.Width = 105;
		this.gridColumnEx46.Caption = "Note";
		this.gridColumnEx46.CurrencyDateField = null;
		this.gridColumnEx46.CurrencyField = null;
		this.gridColumnEx46.CurrencyFixed = null;
		this.gridColumnEx46.CurrencyFixedRate = 0.0;
		this.gridColumnEx46.CurrencyRateField = null;
		this.gridColumnEx46.FieldName = "Note";
		this.gridColumnEx46.Name = "gridColumnEx46";
		this.gridColumnEx46.PrimaryKey = false;
		this.gridColumnEx46.UnitField = null;
		this.gridColumnEx46.Width = 117;
		this.gridColumnEx47.Caption = "Deleted";
		this.gridColumnEx47.CurrencyDateField = null;
		this.gridColumnEx47.CurrencyField = null;
		this.gridColumnEx47.CurrencyFixed = null;
		this.gridColumnEx47.CurrencyFixedRate = 0.0;
		this.gridColumnEx47.CurrencyRateField = null;
		this.gridColumnEx47.FieldName = "Deleted";
		this.gridColumnEx47.Name = "gridColumnEx47";
		this.gridColumnEx47.PrimaryKey = false;
		this.gridColumnEx47.UnitField = null;
		this.gridColumnEx47.Width = 70;
		this.gc_CustomerComment_Hided.Caption = "Hidden";
		this.gc_CustomerComment_Hided.CurrencyDateField = null;
		this.gc_CustomerComment_Hided.CurrencyField = null;
		this.gc_CustomerComment_Hided.CurrencyFixed = null;
		this.gc_CustomerComment_Hided.CurrencyFixedRate = 0.0;
		this.gc_CustomerComment_Hided.CurrencyRateField = null;
		this.gc_CustomerComment_Hided.FieldName = "Hided";
		this.gc_CustomerComment_Hided.Name = "gc_CustomerComment_Hided";
		this.gc_CustomerComment_Hided.PrimaryKey = false;
		this.gc_CustomerComment_Hided.UnitField = null;
		this.gc_CustomerComment_Hided.Width = 41;
		this.sp_CustomerInfo_Top.Controls.Add(this.lblName);
		this.sp_CustomerInfo_Top.Controls.Add(this.cfl_BPName);
		this.sp_CustomerInfo_Top.Controls.Add(this.cfl_BPCode);
		this.sp_CustomerInfo_Top.Controls.Add(this.labelLine12);
		this.sp_CustomerInfo_Top.Controls.Add(this.labelLine11);
		this.sp_CustomerInfo_Top.Controls.Add(this.listOrderStatus);
		this.sp_CustomerInfo_Top.Controls.Add(this.lblLastContact);
		this.sp_CustomerInfo_Top.Dock = System.Windows.Forms.DockStyle.Top;
		this.sp_CustomerInfo_Top.Location = new System.Drawing.Point(0, 0);
		this.sp_CustomerInfo_Top.Name = "sp_CustomerInfo_Top";
		this.sp_CustomerInfo_Top.Size = new System.Drawing.Size(713, 104);
		this.sp_CustomerInfo_Top.TabIndex = 1;
		this.sp_CustomerInfo_Top.Text = "sidePanel1";
		this.sp_CustomerInfo_Top.Click += new System.EventHandler(this.sp_CustomerInfo_Top_Click);
		this.lblName.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.lblName.Appearance.Options.UseFont = true;
		this.lblName.Location = new System.Drawing.Point(8, 8);
		this.lblName.MaximumSize = new System.Drawing.Size(165, 13);
		this.lblName.Name = "lblName";
		this.lblName.Size = new System.Drawing.Size(32, 13);
		this.lblName.TabIndex = 30;
		this.lblName.Text = "Name";
		this.cfl_BPName.EditControl = null;
		this.cfl_BPName.FieldName = null;
		this.cfl_BPName.IsSearchControl = false;
		this.cfl_BPName.Location = new System.Drawing.Point(55, 52);
		this.cfl_BPName.Name = "cfl_BPName";
		this.cfl_BPName.Properties.AcceptFalseInput = true;
		this.cfl_BPName.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions7.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions7.Image");
		this.cfl_BPName.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, false, true, editorButtonImageOptions7, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance10, appearanceHovered10, appearancePressed10, appearanceDisabled10, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.cfl_BPName.Properties.ContainerControl = null;
		this.cfl_BPName.Properties.DisplayMember = "CardName";
		this.cfl_BPName.Properties.DisplayMember2 = "";
		this.cfl_BPName.Properties.LoadDataSourceWhenEnter = false;
		this.cfl_BPName.Properties.SeparateCharacterForMulti = ';';
		this.cfl_BPName.Properties.SeparateString = ", ";
		this.cfl_BPName.Properties.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.Customer;
		this.cfl_BPName.Properties.UsingAutoCompleteBox = false;
		this.cfl_BPName.Properties.UsingDataSource = true;
		this.cfl_BPName.Properties.ValidateWhenNoDataSource = false;
		this.cfl_BPName.Properties.ValueMember = "CardName";
		this.cfl_BPName.SettingInVisible = false;
		this.cfl_BPName.SettingReadonly = false;
		this.cfl_BPName.Size = new System.Drawing.Size(132, 20);
		this.cfl_BPName.TabIndex = 24;
		this.cfl_BPName.EditValueChanged += new System.EventHandler(this.cfl_BPName_EditValueChanged);
		this.cfl_BPCode.EditControl = null;
		this.cfl_BPCode.FieldName = null;
		this.cfl_BPCode.IsSearchControl = false;
		this.cfl_BPCode.Location = new System.Drawing.Point(55, 26);
		this.cfl_BPCode.Name = "cfl_BPCode";
		this.cfl_BPCode.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions8.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions8.Image");
		this.cfl_BPCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions8, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance11, appearanceHovered11, appearancePressed11, appearanceDisabled11, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.cfl_BPCode.Properties.ContainerControl = null;
		this.cfl_BPCode.Properties.DisplayMember = "CardCode";
		this.cfl_BPCode.Properties.DisplayMember2 = "";
		this.cfl_BPCode.Properties.LoadDataSourceWhenEnter = false;
		this.cfl_BPCode.Properties.SeparateCharacterForMulti = ';';
		this.cfl_BPCode.Properties.SeparateString = ", ";
		this.cfl_BPCode.Properties.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.Customer;
		this.cfl_BPCode.Properties.UsingAutoCompleteBox = false;
		this.cfl_BPCode.Properties.UsingDataSource = true;
		this.cfl_BPCode.Properties.ValidateWhenNoDataSource = false;
		this.cfl_BPCode.Properties.ValueMember = "CardCode";
		this.cfl_BPCode.SettingInVisible = false;
		this.cfl_BPCode.SettingReadonly = false;
		this.cfl_BPCode.Size = new System.Drawing.Size(132, 20);
		this.cfl_BPCode.TabIndex = 23;
		this.cfl_BPCode.EditValueChanged += new System.EventHandler(this.cfl_BPCode_EditValueChanged);
		this.labelLine12.EditControl = null;
		this.labelLine12.EditValue = "BP Name";
		this.labelLine12.FieldName = null;
		this.labelLine12.HideWhenEmpty = false;
		this.labelLine12.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine12.Location = new System.Drawing.Point(8, 55);
		this.labelLine12.Name = "labelLine12";
		this.labelLine12.ReadOnly = false;
		this.labelLine12.SettingInVisible = false;
		this.labelLine12.SettingReadonly = false;
		this.labelLine12.ShowUnderLine = true;
		this.labelLine12.Size = new System.Drawing.Size(61, 20);
		this.labelLine12.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine12.TabIndex = 29;
		this.labelLine12.Utext = "BP Name";
		this.labelLine11.EditControl = null;
		this.labelLine11.EditValue = "BP Code";
		this.labelLine11.FieldName = null;
		this.labelLine11.HideWhenEmpty = false;
		this.labelLine11.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine11.Location = new System.Drawing.Point(8, 29);
		this.labelLine11.Name = "labelLine11";
		this.labelLine11.ReadOnly = false;
		this.labelLine11.SettingInVisible = false;
		this.labelLine11.SettingReadonly = false;
		this.labelLine11.ShowUnderLine = true;
		this.labelLine11.Size = new System.Drawing.Size(61, 20);
		this.labelLine11.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine11.TabIndex = 29;
		this.labelLine11.Utext = "BP Code";
		this.listOrderStatus.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.listOrderStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		this.listOrderStatus.HideSelection = false;
		this.listOrderStatus.Location = new System.Drawing.Point(5, 81);
		this.listOrderStatus.Name = "listOrderStatus";
		this.listOrderStatus.Size = new System.Drawing.Size(704, 23);
		this.listOrderStatus.TabIndex = 28;
		this.listOrderStatus.UseCompatibleStateImageBehavior = false;
		this.listOrderStatus.View = System.Windows.Forms.View.SmallIcon;
		this.lblLastContact.Appearance.ForeColor = System.Drawing.Color.Silver;
		this.lblLastContact.Appearance.Options.UseForeColor = true;
		this.lblLastContact.Location = new System.Drawing.Point(160, 8);
		this.lblLastContact.MaximumSize = new System.Drawing.Size(170, 13);
		this.lblLastContact.Name = "lblLastContact";
		this.lblLastContact.Size = new System.Drawing.Size(65, 13);
		this.lblLastContact.TabIndex = 25;
		this.lblLastContact.Text = "Last Contact:";
		this.lblLastContact.ToolTip = "Last Contact";
		this.sp_CustomerList.Controls.Add(this.grd_Customer);
		this.sp_CustomerList.Dock = System.Windows.Forms.DockStyle.Left;
		this.sp_CustomerList.Location = new System.Drawing.Point(0, 0);
		this.sp_CustomerList.Name = "sp_CustomerList";
		this.sp_CustomerList.Size = new System.Drawing.Size(216, 425);
		this.sp_CustomerList.TabIndex = 29;
		this.sp_CustomerList.Text = "sidePanel1";
		this.grd_Customer.AllowPasteExcel = false;
		this.grd_Customer.ClipboardData = null;
		this.grd_Customer.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Customer.Location = new System.Drawing.Point(0, 0);
		this.grd_Customer.MainView = this.gv_Customer;
		this.grd_Customer.Name = "grd_Customer";
		this.grd_Customer.Size = new System.Drawing.Size(215, 425);
		this.grd_Customer.TabIndex = 0;
		this.grd_Customer.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Customer });
		this.gv_Customer.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Customer.ColumnKey = null;
		this.gv_Customer.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[11]
		{
			this.gridColumnEx38, this.gridColumnEx14, this.gc_Customer_CustomerName, this.gc_Customer_BPCode, this.gc_Customer_BPName, this.gc_Customer_LastContact, this.gridColumnEx18, this.gridColumnEx19, this.gridColumnEx20, this.gridColumnEx21,
			this.gridColumnEx22
		});
		this.gv_Customer.DisableFormSetting = false;
		this.gv_Customer.DisplayException = false;
		this.gv_Customer.EditColumn = null;
		this.gv_Customer.FieldCurrency = null;
		this.gv_Customer.FieldRate = null;
		this.gv_Customer.FormId = "oFacebookCrm";
		this.gv_Customer.GridControl = this.grd_Customer;
		this.gv_Customer.IgnoreFilter = false;
		this.gv_Customer.IndexEditRow = -1;
		this.gv_Customer.IndicatorWidth = 35;
		this.gv_Customer.IsApzTable = false;
		this.gv_Customer.IsInsertNewRow = false;
		this.gv_Customer.IsLoadingData = false;
		this.gv_Customer.IsMappingContextAction = false;
		this.gv_Customer.ItemId = "OCUSTOMER";
		this.gv_Customer.LstColumnAllowBatch = null;
		this.gv_Customer.LstColumnNotAllowBatch = null;
		this.gv_Customer.LstColumnNotEditable = null;
		this.gv_Customer.LstColumNotSetting = null;
		this.gv_Customer.Name = "gv_Customer";
		this.gv_Customer.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Customer.OptionsView.ColumnAutoWidth = false;
		this.gv_Customer.OptionsView.ShowColumnHeaders = false;
		this.gv_Customer.OptionsView.ShowGroupPanel = false;
		this.gv_Customer.UseBatchMod = false;
		this.gv_Customer.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Customer_RowCellStyle);
		this.gv_Customer.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_Customer_FocusedRowObjectChanged);
		this.gv_Customer.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Customer_CellValueChanged);
		this.gv_Customer.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Inbox_CellValueChanging);
		this.gv_Customer.BeforeLeaveRow += new DevExpress.XtraGrid.Views.Base.RowAllowEventHandler(this.gv_Customer_BeforeLeaveRow);
		this.gv_Customer.CustomRowFilter += new DevExpress.XtraGrid.Views.Base.RowFilterEventHandler(this.gv_Customer_CustomRowFilter);
		this.gv_Customer.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.gv_Customer_CustomColumnDisplayText);
		this.gridColumnEx38.Caption = "PageID";
		this.gridColumnEx38.CurrencyDateField = null;
		this.gridColumnEx38.CurrencyField = null;
		this.gridColumnEx38.CurrencyFixed = null;
		this.gridColumnEx38.CurrencyFixedRate = 0.0;
		this.gridColumnEx38.CurrencyRateField = null;
		this.gridColumnEx38.FieldName = "PageID";
		this.gridColumnEx38.Name = "gridColumnEx38";
		this.gridColumnEx38.OptionsColumn.ReadOnly = true;
		this.gridColumnEx38.PrimaryKey = false;
		this.gridColumnEx38.UnitField = null;
		this.gridColumnEx14.Caption = "ID";
		this.gridColumnEx14.CurrencyDateField = null;
		this.gridColumnEx14.CurrencyField = null;
		this.gridColumnEx14.CurrencyFixed = null;
		this.gridColumnEx14.CurrencyFixedRate = 0.0;
		this.gridColumnEx14.CurrencyRateField = null;
		this.gridColumnEx14.FieldName = "CustomerID";
		this.gridColumnEx14.Name = "gridColumnEx14";
		this.gridColumnEx14.OptionsColumn.ReadOnly = true;
		this.gridColumnEx14.PrimaryKey = false;
		this.gridColumnEx14.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridColumnEx14.UnitField = null;
		this.gridColumnEx14.Width = 143;
		this.gc_Customer_CustomerName.Caption = "Name";
		this.gc_Customer_CustomerName.ColumnEdit = this.rep_cfl_CustomerID;
		this.gc_Customer_CustomerName.CurrencyDateField = null;
		this.gc_Customer_CustomerName.CurrencyField = null;
		this.gc_Customer_CustomerName.CurrencyFixed = null;
		this.gc_Customer_CustomerName.CurrencyFixedRate = 0.0;
		this.gc_Customer_CustomerName.CurrencyRateField = null;
		this.gc_Customer_CustomerName.FieldName = "CustomerName";
		this.gc_Customer_CustomerName.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gc_Customer_CustomerName.Name = "gc_Customer_CustomerName";
		this.gc_Customer_CustomerName.OptionsColumn.AllowEdit = false;
		this.gc_Customer_CustomerName.OptionsColumn.ReadOnly = true;
		this.gc_Customer_CustomerName.PrimaryKey = false;
		this.gc_Customer_CustomerName.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowForFocusedRow;
		this.gc_Customer_CustomerName.UnitField = null;
		this.gc_Customer_CustomerName.Visible = true;
		this.gc_Customer_CustomerName.VisibleIndex = 0;
		this.gc_Customer_CustomerName.Width = 178;
		this.gc_Customer_BPCode.Caption = "BP Code";
		this.gc_Customer_BPCode.ColumnEdit = this.rep_cfl_BPCode;
		this.gc_Customer_BPCode.CurrencyDateField = null;
		this.gc_Customer_BPCode.CurrencyField = null;
		this.gc_Customer_BPCode.CurrencyFixed = null;
		this.gc_Customer_BPCode.CurrencyFixedRate = 0.0;
		this.gc_Customer_BPCode.CurrencyRateField = null;
		this.gc_Customer_BPCode.FieldName = "BPCode";
		this.gc_Customer_BPCode.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gc_Customer_BPCode.Name = "gc_Customer_BPCode";
		this.gc_Customer_BPCode.PrimaryKey = false;
		this.gc_Customer_BPCode.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowForFocusedRow;
		this.gc_Customer_BPCode.UnitField = null;
		this.gc_Customer_BPCode.Width = 94;
		this.gc_Customer_BPName.Caption = "BP Name";
		this.gc_Customer_BPName.ColumnEdit = this.rep_cfl_BPName;
		this.gc_Customer_BPName.CurrencyDateField = null;
		this.gc_Customer_BPName.CurrencyField = null;
		this.gc_Customer_BPName.CurrencyFixed = null;
		this.gc_Customer_BPName.CurrencyFixedRate = 0.0;
		this.gc_Customer_BPName.CurrencyRateField = null;
		this.gc_Customer_BPName.FieldName = "BPName";
		this.gc_Customer_BPName.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gc_Customer_BPName.Name = "gc_Customer_BPName";
		this.gc_Customer_BPName.PrimaryKey = false;
		this.gc_Customer_BPName.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowForFocusedRow;
		this.gc_Customer_BPName.UnitField = null;
		this.gc_Customer_BPName.Width = 143;
		this.gc_Customer_LastContact.Caption = "Last Contact";
		this.gc_Customer_LastContact.CurrencyDateField = null;
		this.gc_Customer_LastContact.CurrencyField = null;
		this.gc_Customer_LastContact.CurrencyFixed = null;
		this.gc_Customer_LastContact.CurrencyFixedRate = 0.0;
		this.gc_Customer_LastContact.CurrencyRateField = null;
		this.gc_Customer_LastContact.FieldName = "LastContact";
		this.gc_Customer_LastContact.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gc_Customer_LastContact.Name = "gc_Customer_LastContact";
		this.gc_Customer_LastContact.OptionsColumn.ReadOnly = true;
		this.gc_Customer_LastContact.PrimaryKey = false;
		this.gc_Customer_LastContact.UnitField = null;
		this.gc_Customer_LastContact.Width = 163;
		this.gridColumnEx18.Caption = "Phone";
		this.gridColumnEx18.CurrencyDateField = null;
		this.gridColumnEx18.CurrencyField = null;
		this.gridColumnEx18.CurrencyFixed = null;
		this.gridColumnEx18.CurrencyFixedRate = 0.0;
		this.gridColumnEx18.CurrencyRateField = null;
		this.gridColumnEx18.FieldName = "Phone";
		this.gridColumnEx18.Name = "gridColumnEx18";
		this.gridColumnEx18.PrimaryKey = false;
		this.gridColumnEx18.UnitField = null;
		this.gridColumnEx18.Width = 71;
		this.gridColumnEx19.Caption = "Email";
		this.gridColumnEx19.CurrencyDateField = null;
		this.gridColumnEx19.CurrencyField = null;
		this.gridColumnEx19.CurrencyFixed = null;
		this.gridColumnEx19.CurrencyFixedRate = 0.0;
		this.gridColumnEx19.CurrencyRateField = null;
		this.gridColumnEx19.FieldName = "Email";
		this.gridColumnEx19.Name = "gridColumnEx19";
		this.gridColumnEx19.PrimaryKey = false;
		this.gridColumnEx19.UnitField = null;
		this.gridColumnEx19.Width = 71;
		this.gridColumnEx20.Caption = "Location";
		this.gridColumnEx20.CurrencyDateField = null;
		this.gridColumnEx20.CurrencyField = null;
		this.gridColumnEx20.CurrencyFixed = null;
		this.gridColumnEx20.CurrencyFixedRate = 0.0;
		this.gridColumnEx20.CurrencyRateField = null;
		this.gridColumnEx20.FieldName = "Location";
		this.gridColumnEx20.Name = "gridColumnEx20";
		this.gridColumnEx20.PrimaryKey = false;
		this.gridColumnEx20.UnitField = null;
		this.gridColumnEx20.Width = 82;
		this.gridColumnEx21.Caption = "Tag";
		this.gridColumnEx21.CurrencyDateField = null;
		this.gridColumnEx21.CurrencyField = null;
		this.gridColumnEx21.CurrencyFixed = null;
		this.gridColumnEx21.CurrencyFixedRate = 0.0;
		this.gridColumnEx21.CurrencyRateField = null;
		this.gridColumnEx21.FieldName = "Tag";
		this.gridColumnEx21.Name = "gridColumnEx21";
		this.gridColumnEx21.PrimaryKey = false;
		this.gridColumnEx21.UnitField = null;
		this.gridColumnEx22.Caption = "Note";
		this.gridColumnEx22.CurrencyDateField = null;
		this.gridColumnEx22.CurrencyField = null;
		this.gridColumnEx22.CurrencyFixed = null;
		this.gridColumnEx22.CurrencyFixedRate = 0.0;
		this.gridColumnEx22.CurrencyRateField = null;
		this.gridColumnEx22.FieldName = "Note";
		this.gridColumnEx22.Name = "gridColumnEx22";
		this.gridColumnEx22.PrimaryKey = false;
		this.gridColumnEx22.UnitField = null;
		this.gridColumnEx22.Width = 145;
		this.tabpage_Message.Controls.Add(this.grd_Inbox);
		this.tabpage_Message.EditControl = null;
		this.tabpage_Message.EditValue = "Messages";
		this.tabpage_Message.FieldName = null;
		this.tabpage_Message.IsSearchControl = false;
		this.tabpage_Message.Name = "tabpage_Message";
		this.tabpage_Message.ReadOnly = false;
		this.tabpage_Message.SettingInVisible = false;
		this.tabpage_Message.SettingReadonly = false;
		this.tabpage_Message.Size = new System.Drawing.Size(929, 425);
		this.tabpage_Message.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tabpage_Message.Text = "Messages";
		this.tabpage_Comment.Controls.Add(this.grd_Comment);
		this.tabpage_Comment.EditControl = null;
		this.tabpage_Comment.EditValue = "Comments";
		this.tabpage_Comment.FieldName = null;
		this.tabpage_Comment.IsSearchControl = false;
		this.tabpage_Comment.Name = "tabpage_Comment";
		this.tabpage_Comment.ReadOnly = false;
		this.tabpage_Comment.SettingInVisible = false;
		this.tabpage_Comment.SettingReadonly = false;
		this.tabpage_Comment.Size = new System.Drawing.Size(929, 425);
		this.tabpage_Comment.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tabpage_Comment.Text = "Comments";
		this.grd_Comment.AllowPasteExcel = false;
		this.grd_Comment.ClipboardData = null;
		this.grd_Comment.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_Comment.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Comment.Location = new System.Drawing.Point(0, 0);
		this.grd_Comment.MainView = this.gv_Comment;
		this.grd_Comment.Name = "grd_Comment";
		this.grd_Comment.Size = new System.Drawing.Size(929, 425);
		this.grd_Comment.TabIndex = 20;
		this.grd_Comment.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Comment });
		this.grd_Comment.Click += new System.EventHandler(this.grd_Comment_Click);
		this.gv_Comment.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Comment.ColumnKey = null;
		this.gv_Comment.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[14]
		{
			this.gridColumnEx1, this.gridColumnEx2, this.gridColumnEx3, this.gridColumnEx10, this.gridColumnEx4, this.gc_Comment_Content, this.gc_Comment_CustomerName, this.gc_Comment_CreatedAt, this.gc_Comment_OrderStatus, this.gc_Comment_SaleOrder,
			this.gc_Comment_Tag, this.gridColumnEx11, this.gridColumnEx12, this.gc_Comment_Hided
		});
		this.gv_Comment.DisableFormSetting = false;
		this.gv_Comment.DisplayException = false;
		this.gv_Comment.EditColumn = null;
		this.gv_Comment.FieldCurrency = null;
		this.gv_Comment.FieldRate = null;
		this.gv_Comment.FormId = "oFacebookCrm";
		this.gv_Comment.GridControl = this.grd_Comment;
		this.gv_Comment.IgnoreFilter = false;
		this.gv_Comment.IndexEditRow = -1;
		this.gv_Comment.IndicatorWidth = 35;
		this.gv_Comment.IsApzTable = false;
		this.gv_Comment.IsInsertNewRow = false;
		this.gv_Comment.IsLoadingData = false;
		this.gv_Comment.IsMappingContextAction = false;
		this.gv_Comment.ItemId = "OCOMMENT";
		this.gv_Comment.LstColumnAllowBatch = null;
		this.gv_Comment.LstColumnNotAllowBatch = null;
		this.gv_Comment.LstColumnNotEditable = null;
		this.gv_Comment.LstColumNotSetting = null;
		this.gv_Comment.Name = "gv_Comment";
		this.gv_Comment.OptionsBehavior.AutoExpandAllGroups = true;
		this.gv_Comment.OptionsView.ShowGroupPanel = false;
		this.gv_Comment.UseBatchMod = false;
		this.gv_Comment.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Inbox_RowCellStyle);
		this.gv_Comment.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.gv_CustomerComment_RowStyle);
		this.gv_Comment.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_CustomerInbox_FocusedRowObjectChanged);
		this.gv_Comment.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Inbox_CellValueChanging);
		this.gv_Comment.CustomRowFilter += new DevExpress.XtraGrid.Views.Base.RowFilterEventHandler(this.gv_Inbox_CustomRowFilter);
		this.gv_Comment.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.gv_Customer_CustomColumnDisplayText);
		this.gridColumnEx1.Caption = "Page Id";
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "PageID";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.OptionsColumn.ReadOnly = true;
		this.gridColumnEx1.PrimaryKey = false;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx2.Caption = "Message Id";
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.FieldName = "ConversationID";
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.OptionsColumn.ReadOnly = true;
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.UnitField = null;
		this.gridColumnEx3.Caption = "Conversation Type";
		this.gridColumnEx3.CurrencyDateField = null;
		this.gridColumnEx3.CurrencyField = null;
		this.gridColumnEx3.CurrencyFixed = null;
		this.gridColumnEx3.CurrencyFixedRate = 0.0;
		this.gridColumnEx3.CurrencyRateField = null;
		this.gridColumnEx3.FieldName = "ConversationType";
		this.gridColumnEx3.Name = "gridColumnEx3";
		this.gridColumnEx3.OptionsColumn.ReadOnly = true;
		this.gridColumnEx3.PrimaryKey = false;
		this.gridColumnEx3.UnitField = null;
		this.gridColumnEx3.Width = 55;
		this.gridColumnEx10.Caption = "ThreadID";
		this.gridColumnEx10.CurrencyDateField = null;
		this.gridColumnEx10.CurrencyField = null;
		this.gridColumnEx10.CurrencyFixed = null;
		this.gridColumnEx10.CurrencyFixedRate = 0.0;
		this.gridColumnEx10.CurrencyRateField = null;
		this.gridColumnEx10.FieldName = "ThreadID";
		this.gridColumnEx10.Name = "gridColumnEx10";
		this.gridColumnEx10.OptionsColumn.ReadOnly = true;
		this.gridColumnEx10.PrimaryKey = false;
		this.gridColumnEx10.UnitField = null;
		this.gridColumnEx4.Caption = "Customer Id";
		this.gridColumnEx4.CurrencyDateField = null;
		this.gridColumnEx4.CurrencyField = null;
		this.gridColumnEx4.CurrencyFixed = null;
		this.gridColumnEx4.CurrencyFixedRate = 0.0;
		this.gridColumnEx4.CurrencyRateField = null;
		this.gridColumnEx4.FieldName = "CustomerID";
		this.gridColumnEx4.Name = "gridColumnEx4";
		this.gridColumnEx4.OptionsColumn.ReadOnly = true;
		this.gridColumnEx4.PrimaryKey = false;
		this.gridColumnEx4.UnitField = null;
		this.gridColumnEx4.Width = 81;
		this.gc_Comment_Content.Caption = "Content";
		this.gc_Comment_Content.CurrencyDateField = null;
		this.gc_Comment_Content.CurrencyField = null;
		this.gc_Comment_Content.CurrencyFixed = null;
		this.gc_Comment_Content.CurrencyFixedRate = 0.0;
		this.gc_Comment_Content.CurrencyRateField = null;
		this.gc_Comment_Content.FieldName = "Content";
		this.gc_Comment_Content.Name = "gc_Comment_Content";
		this.gc_Comment_Content.OptionsColumn.ReadOnly = true;
		this.gc_Comment_Content.PrimaryKey = false;
		this.gc_Comment_Content.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowForFocusedRow;
		this.gc_Comment_Content.UnitField = null;
		this.gc_Comment_Content.Visible = true;
		this.gc_Comment_Content.VisibleIndex = 1;
		this.gc_Comment_Content.Width = 314;
		this.gc_Comment_CustomerName.Caption = "From";
		this.gc_Comment_CustomerName.CurrencyDateField = null;
		this.gc_Comment_CustomerName.CurrencyField = null;
		this.gc_Comment_CustomerName.CurrencyFixed = null;
		this.gc_Comment_CustomerName.CurrencyFixedRate = 0.0;
		this.gc_Comment_CustomerName.CurrencyRateField = null;
		this.gc_Comment_CustomerName.FieldName = "CustomerName";
		this.gc_Comment_CustomerName.Name = "gc_Comment_CustomerName";
		this.gc_Comment_CustomerName.OptionsColumn.ReadOnly = true;
		this.gc_Comment_CustomerName.PrimaryKey = false;
		this.gc_Comment_CustomerName.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowForFocusedRow;
		this.gc_Comment_CustomerName.UnitField = null;
		this.gc_Comment_CustomerName.Visible = true;
		this.gc_Comment_CustomerName.VisibleIndex = 0;
		this.gc_Comment_CustomerName.Width = 153;
		this.gc_Comment_CreatedAt.Caption = "Created At";
		this.gc_Comment_CreatedAt.CurrencyDateField = null;
		this.gc_Comment_CreatedAt.CurrencyField = null;
		this.gc_Comment_CreatedAt.CurrencyFixed = null;
		this.gc_Comment_CreatedAt.CurrencyFixedRate = 0.0;
		this.gc_Comment_CreatedAt.CurrencyRateField = null;
		this.gc_Comment_CreatedAt.FieldName = "CreatedTime";
		this.gc_Comment_CreatedAt.Name = "gc_Comment_CreatedAt";
		this.gc_Comment_CreatedAt.OptionsColumn.ReadOnly = true;
		this.gc_Comment_CreatedAt.PrimaryKey = false;
		this.gc_Comment_CreatedAt.UnitField = null;
		this.gc_Comment_CreatedAt.Visible = true;
		this.gc_Comment_CreatedAt.VisibleIndex = 2;
		this.gc_Comment_CreatedAt.Width = 134;
		this.gc_Comment_OrderStatus.Caption = "Status";
		this.gc_Comment_OrderStatus.CurrencyDateField = null;
		this.gc_Comment_OrderStatus.CurrencyField = null;
		this.gc_Comment_OrderStatus.CurrencyFixed = null;
		this.gc_Comment_OrderStatus.CurrencyFixedRate = 0.0;
		this.gc_Comment_OrderStatus.CurrencyRateField = null;
		this.gc_Comment_OrderStatus.FieldName = "OrderStatus";
		this.gc_Comment_OrderStatus.Name = "gc_Comment_OrderStatus";
		this.gc_Comment_OrderStatus.PrimaryKey = false;
		this.gc_Comment_OrderStatus.UnitField = null;
		this.gc_Comment_OrderStatus.Visible = true;
		this.gc_Comment_OrderStatus.VisibleIndex = 3;
		this.gc_Comment_OrderStatus.Width = 85;
		this.gc_Comment_SaleOrder.Caption = "Sale Order";
		this.gc_Comment_SaleOrder.CurrencyDateField = null;
		this.gc_Comment_SaleOrder.CurrencyField = null;
		this.gc_Comment_SaleOrder.CurrencyFixed = null;
		this.gc_Comment_SaleOrder.CurrencyFixedRate = 0.0;
		this.gc_Comment_SaleOrder.CurrencyRateField = null;
		this.gc_Comment_SaleOrder.FieldName = "DocEntry";
		this.gc_Comment_SaleOrder.Name = "gc_Comment_SaleOrder";
		this.gc_Comment_SaleOrder.PrimaryKey = false;
		this.gc_Comment_SaleOrder.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowForFocusedRow;
		this.gc_Comment_SaleOrder.UnitField = null;
		this.gc_Comment_SaleOrder.Visible = true;
		this.gc_Comment_SaleOrder.VisibleIndex = 4;
		this.gc_Comment_SaleOrder.Width = 89;
		this.gc_Comment_Tag.Caption = "Tag";
		this.gc_Comment_Tag.CurrencyDateField = null;
		this.gc_Comment_Tag.CurrencyField = null;
		this.gc_Comment_Tag.CurrencyFixed = null;
		this.gc_Comment_Tag.CurrencyFixedRate = 0.0;
		this.gc_Comment_Tag.CurrencyRateField = null;
		this.gc_Comment_Tag.FieldName = "Tag";
		this.gc_Comment_Tag.Name = "gc_Comment_Tag";
		this.gc_Comment_Tag.PrimaryKey = false;
		this.gc_Comment_Tag.UnitField = null;
		this.gc_Comment_Tag.Visible = true;
		this.gc_Comment_Tag.VisibleIndex = 5;
		this.gc_Comment_Tag.Width = 114;
		this.gridColumnEx11.Caption = "Note";
		this.gridColumnEx11.CurrencyDateField = null;
		this.gridColumnEx11.CurrencyField = null;
		this.gridColumnEx11.CurrencyFixed = null;
		this.gridColumnEx11.CurrencyFixedRate = 0.0;
		this.gridColumnEx11.CurrencyRateField = null;
		this.gridColumnEx11.FieldName = "Note";
		this.gridColumnEx11.Name = "gridColumnEx11";
		this.gridColumnEx11.PrimaryKey = false;
		this.gridColumnEx11.UnitField = null;
		this.gridColumnEx11.Width = 117;
		this.gridColumnEx12.Caption = "Deleted";
		this.gridColumnEx12.CurrencyDateField = null;
		this.gridColumnEx12.CurrencyField = null;
		this.gridColumnEx12.CurrencyFixed = null;
		this.gridColumnEx12.CurrencyFixedRate = 0.0;
		this.gridColumnEx12.CurrencyRateField = null;
		this.gridColumnEx12.FieldName = "Deleted";
		this.gridColumnEx12.Name = "gridColumnEx12";
		this.gridColumnEx12.OptionsColumn.ReadOnly = true;
		this.gridColumnEx12.PrimaryKey = false;
		this.gridColumnEx12.UnitField = null;
		this.gridColumnEx12.Width = 70;
		this.gc_Comment_Hided.Caption = "Hidden";
		this.gc_Comment_Hided.CurrencyDateField = null;
		this.gc_Comment_Hided.CurrencyField = null;
		this.gc_Comment_Hided.CurrencyFixed = null;
		this.gc_Comment_Hided.CurrencyFixedRate = 0.0;
		this.gc_Comment_Hided.CurrencyRateField = null;
		this.gc_Comment_Hided.FieldName = "Hided";
		this.gc_Comment_Hided.Name = "gc_Comment_Hided";
		this.gc_Comment_Hided.PrimaryKey = false;
		this.gc_Comment_Hided.UnitField = null;
		this.gc_Comment_Hided.Width = 44;
		this.btnSwitch.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnSwitch.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btnSwitch.EditControl = null;
		this.btnSwitch.EditValue = "";
		this.btnSwitch.FieldName = null;
		this.btnSwitch.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnSwitch.ImageOptions.Image");
		this.btnSwitch.IsSearchControl = false;
		this.btnSwitch.Location = new System.Drawing.Point(928, 572);
		this.btnSwitch.Name = "btnSwitch";
		this.btnSwitch.ReadOnly = false;
		this.btnSwitch.SettingInVisible = false;
		this.btnSwitch.SettingReadonly = false;
		this.btnSwitch.Size = new System.Drawing.Size(23, 23);
		this.btnSwitch.TabIndex = 31;
		this.btnSwitch.Click += new System.EventHandler(this.btnSwitch_Click);
		this.labelControl2.Location = new System.Drawing.Point(330, 6);
		this.labelControl2.Name = "labelControl2";
		this.labelControl2.Size = new System.Drawing.Size(49, 13);
		this.labelControl2.TabIndex = 27;
		this.labelControl2.Text = "BP Name: ";
		this.labelControl1.Location = new System.Drawing.Point(156, 6);
		this.labelControl1.Name = "labelControl1";
		this.labelControl1.Size = new System.Drawing.Size(47, 13);
		this.labelControl1.TabIndex = 26;
		this.labelControl1.Text = "BP Code: ";
		this.btnOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnOK.EditControl = null;
		this.btnOK.EditValue = "OK";
		this.btnOK.FieldName = null;
		this.btnOK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnOK.ImageOptions.Image");
		this.btnOK.IsSearchControl = false;
		this.btnOK.Location = new System.Drawing.Point(25, 572);
		this.btnOK.Name = "btnOK";
		this.btnOK.ReadOnly = false;
		this.btnOK.SettingInVisible = false;
		this.btnOK.SettingReadonly = false;
		this.btnOK.Size = new System.Drawing.Size(75, 23);
		this.btnOK.TabIndex = 21;
		this.btnOK.Text = "OK";
		this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
		this.btCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btCancel.EditControl = null;
		this.btCancel.EditValue = "Cancel";
		this.btCancel.FieldName = null;
		this.btCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btCancel.ImageOptions.Image");
		this.btCancel.IsSearchControl = false;
		this.btCancel.Location = new System.Drawing.Point(106, 572);
		this.btCancel.Name = "btCancel";
		this.btCancel.ReadOnly = false;
		this.btCancel.SettingInVisible = false;
		this.btCancel.SettingReadonly = false;
		this.btCancel.Size = new System.Drawing.Size(75, 23);
		this.btCancel.TabIndex = 22;
		this.btCancel.Text = "Cancel";
		this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
		this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
		this.btnSearch.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btnSearch.EditControl = null;
		this.btnSearch.EditValue = "Search";
		this.btnSearch.FieldName = null;
		this.btnSearch.IsSearchControl = false;
		this.btnSearch.Location = new System.Drawing.Point(886, 9);
		this.btnSearch.Name = "btnSearch";
		this.btnSearch.ReadOnly = false;
		this.btnSearch.SettingInVisible = false;
		this.btnSearch.SettingReadonly = false;
		this.btnSearch.Size = new System.Drawing.Size(61, 21);
		this.btnSearch.TabIndex = 5;
		this.btnSearch.Text = "Search";
		this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "From Date";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(3, 13);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = false;
		this.labelLine1.Size = new System.Drawing.Size(75, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 27;
		this.labelLine1.Utext = "From Date";
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "User";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(8, 9);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = false;
		this.labelLine2.Size = new System.Drawing.Size(35, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 28;
		this.labelLine2.Utext = "User";
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "Page";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(8, 35);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = false;
		this.labelLine3.Size = new System.Drawing.Size(35, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 29;
		this.labelLine3.Utext = "Page";
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "BP Codes";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(435, 13);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = false;
		this.labelLine4.Size = new System.Drawing.Size(80, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 30;
		this.labelLine4.Utext = "BP Codes";
		this.labelLine5.EditControl = null;
		this.labelLine5.EditValue = "To Date";
		this.labelLine5.FieldName = null;
		this.labelLine5.HideWhenEmpty = false;
		this.labelLine5.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine5.Location = new System.Drawing.Point(224, 13);
		this.labelLine5.Name = "labelLine5";
		this.labelLine5.ReadOnly = false;
		this.labelLine5.SettingInVisible = false;
		this.labelLine5.SettingReadonly = false;
		this.labelLine5.ShowUnderLine = false;
		this.labelLine5.Size = new System.Drawing.Size(71, 20);
		this.labelLine5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine5.TabIndex = 27;
		this.labelLine5.Utext = "To Date";
		this.labelLine6.EditControl = null;
		this.labelLine6.EditValue = "Item Codes";
		this.labelLine6.FieldName = null;
		this.labelLine6.HideWhenEmpty = false;
		this.labelLine6.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine6.Location = new System.Drawing.Point(435, 37);
		this.labelLine6.Name = "labelLine6";
		this.labelLine6.ReadOnly = false;
		this.labelLine6.SettingInVisible = false;
		this.labelLine6.SettingReadonly = false;
		this.labelLine6.ShowUnderLine = false;
		this.labelLine6.Size = new System.Drawing.Size(80, 20);
		this.labelLine6.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine6.TabIndex = 32;
		this.labelLine6.Utext = "Item Codes";
		this.cfl_ItemCode_Search.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.cfl_ItemCode_Search.EditControl = null;
		this.cfl_ItemCode_Search.FieldName = null;
		this.cfl_ItemCode_Search.IsSearchControl = false;
		this.cfl_ItemCode_Search.Location = new System.Drawing.Point(497, 34);
		this.cfl_ItemCode_Search.Name = "cfl_ItemCode_Search";
		this.cfl_ItemCode_Search.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions9.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions4.Image");
		this.cfl_ItemCode_Search.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, false, true, editorButtonImageOptions9, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance12, appearanceHovered12, appearancePressed12, appearanceDisabled12, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.cfl_ItemCode_Search.Properties.ContainerControl = null;
		this.cfl_ItemCode_Search.Properties.DisplayMember = "";
		this.cfl_ItemCode_Search.Properties.DisplayMember2 = "";
		this.cfl_ItemCode_Search.Properties.LoadDataSourceWhenEnter = false;
		this.cfl_ItemCode_Search.Properties.SeparateCharacterForMulti = ';';
		this.cfl_ItemCode_Search.Properties.SeparateString = ", ";
		this.cfl_ItemCode_Search.Properties.UsingAutoCompleteBox = false;
		this.cfl_ItemCode_Search.Properties.UsingDataSource = true;
		this.cfl_ItemCode_Search.Properties.ValidateWhenNoDataSource = false;
		this.cfl_ItemCode_Search.Properties.ValueMember = "";
		this.cfl_ItemCode_Search.SettingInVisible = false;
		this.cfl_ItemCode_Search.SettingReadonly = false;
		this.cfl_ItemCode_Search.Size = new System.Drawing.Size(301, 20);
		this.cfl_ItemCode_Search.TabIndex = 2;
		this.cfl_BPCode_Search.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.cfl_BPCode_Search.EditControl = null;
		this.cfl_BPCode_Search.FieldName = null;
		this.cfl_BPCode_Search.IsSearchControl = false;
		this.cfl_BPCode_Search.Location = new System.Drawing.Point(497, 10);
		this.cfl_BPCode_Search.Name = "cfl_BPCode_Search";
		this.cfl_BPCode_Search.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions10.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions3.Image");
		this.cfl_BPCode_Search.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, false, true, editorButtonImageOptions10, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance13, appearanceHovered13, appearancePressed13, appearanceDisabled13, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.cfl_BPCode_Search.Properties.ContainerControl = null;
		this.cfl_BPCode_Search.Properties.DisplayMember = "";
		this.cfl_BPCode_Search.Properties.DisplayMember2 = "";
		this.cfl_BPCode_Search.Properties.LoadDataSourceWhenEnter = false;
		this.cfl_BPCode_Search.Properties.SeparateCharacterForMulti = ';';
		this.cfl_BPCode_Search.Properties.SeparateString = ", ";
		this.cfl_BPCode_Search.Properties.UsingAutoCompleteBox = false;
		this.cfl_BPCode_Search.Properties.UsingDataSource = true;
		this.cfl_BPCode_Search.Properties.ValidateWhenNoDataSource = false;
		this.cfl_BPCode_Search.Properties.ValueMember = "";
		this.cfl_BPCode_Search.SettingInVisible = false;
		this.cfl_BPCode_Search.SettingReadonly = false;
		this.cfl_BPCode_Search.Size = new System.Drawing.Size(302, 20);
		this.cfl_BPCode_Search.TabIndex = 1;
		this.btnClear.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btnClear.EditControl = null;
		this.btnClear.EditValue = "Clear";
		this.btnClear.FieldName = null;
		this.btnClear.IsSearchControl = false;
		this.btnClear.Location = new System.Drawing.Point(886, 34);
		this.btnClear.Name = "btnClear";
		this.btnClear.ReadOnly = false;
		this.btnClear.SettingInVisible = false;
		this.btnClear.SettingReadonly = false;
		this.btnClear.Size = new System.Drawing.Size(61, 20);
		this.btnClear.TabIndex = 33;
		this.btnClear.Text = "Clear";
		this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
		this.date_From.EditControl = null;
		this.date_From.EditValue = null;
		this.date_From.FieldName = null;
		this.date_From.Location = new System.Drawing.Point(59, 10);
		this.date_From.Name = "date_From";
		this.date_From.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_From.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_From.Properties.DisplayFormat.FormatString = "";
		this.date_From.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_From.Properties.EditFormat.FormatString = "";
		this.date_From.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_From.Properties.IsSearchControl = false;
		this.date_From.Properties.Mask.EditMask = "dd.MM.yyyy hh:mm tt";
		this.date_From.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.DateTimeAdvancingCaret;
		this.date_From.SettingInVisible = false;
		this.date_From.SettingReadonly = false;
		this.date_From.Size = new System.Drawing.Size(159, 20);
		this.date_From.TabIndex = 3;
		this.date_To.EditControl = null;
		this.date_To.EditValue = null;
		this.date_To.FieldName = null;
		this.date_To.Location = new System.Drawing.Point(270, 10);
		this.date_To.Name = "date_To";
		this.date_To.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_To.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_To.Properties.DisplayFormat.FormatString = "";
		this.date_To.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_To.Properties.EditFormat.FormatString = "";
		this.date_To.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_To.Properties.IsSearchControl = false;
		this.date_To.Properties.Mask.EditMask = "dd.MM.yyyy hh:mm tt";
		this.date_To.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.DateTimeAdvancingCaret;
		this.date_To.SettingInVisible = false;
		this.date_To.SettingReadonly = false;
		this.date_To.Size = new System.Drawing.Size(159, 20);
		this.date_To.TabIndex = 4;
		this.labelLine7.EditControl = null;
		this.labelLine7.EditValue = "Status";
		this.labelLine7.FieldName = null;
		this.labelLine7.HideWhenEmpty = false;
		this.labelLine7.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine7.Location = new System.Drawing.Point(3, 37);
		this.labelLine7.Name = "labelLine7";
		this.labelLine7.ReadOnly = false;
		this.labelLine7.SettingInVisible = false;
		this.labelLine7.SettingReadonly = false;
		this.labelLine7.ShowUnderLine = false;
		this.labelLine7.Size = new System.Drawing.Size(85, 20);
		this.labelLine7.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine7.TabIndex = 35;
		this.labelLine7.Utext = "Status";
		this.ccb_OrderStatus.Location = new System.Drawing.Point(59, 34);
		this.ccb_OrderStatus.Name = "ccb_OrderStatus";
		this.ccb_OrderStatus.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.ccb_OrderStatus.Size = new System.Drawing.Size(159, 20);
		this.ccb_OrderStatus.TabIndex = 34;
		this.btnCheck.EditControl = null;
		this.btnCheck.EditValue = "Check";
		this.btnCheck.FieldName = null;
		this.btnCheck.IsSearchControl = false;
		this.btnCheck.Location = new System.Drawing.Point(265, 4);
		this.btnCheck.Name = "btnCheck";
		this.btnCheck.ReadOnly = false;
		this.btnCheck.SettingInVisible = false;
		this.btnCheck.SettingReadonly = false;
		this.btnCheck.Size = new System.Drawing.Size(61, 21);
		this.btnCheck.TabIndex = 36;
		this.btnCheck.Text = "Check";
		this.btnCheck.Click += new System.EventHandler(this.btnCheck_Click);
		this.labelLine8.EditControl = null;
		this.labelLine8.EditValue = "Tags";
		this.labelLine8.FieldName = null;
		this.labelLine8.HideWhenEmpty = false;
		this.labelLine8.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine8.Location = new System.Drawing.Point(224, 37);
		this.labelLine8.Name = "labelLine8";
		this.labelLine8.ReadOnly = false;
		this.labelLine8.SettingInVisible = false;
		this.labelLine8.SettingReadonly = false;
		this.labelLine8.ShowUnderLine = false;
		this.labelLine8.Size = new System.Drawing.Size(85, 20);
		this.labelLine8.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine8.TabIndex = 35;
		this.labelLine8.Utext = "Tags";
		this.ccb_Tag.Location = new System.Drawing.Point(270, 34);
		this.ccb_Tag.Name = "ccb_Tag";
		this.ccb_Tag.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.ccb_Tag.Size = new System.Drawing.Size(159, 20);
		this.ccb_Tag.TabIndex = 34;
		this.tabpage_Login.Controls.Add(this.luk_Page);
		this.tabpage_Login.Controls.Add(this.btnCheck);
		this.tabpage_Login.Controls.Add(this.txtUserName);
		this.tabpage_Login.Controls.Add(this.btnLogin);
		this.tabpage_Login.Controls.Add(this.btnActive);
		this.tabpage_Login.Controls.Add(this.btnAccess);
		this.tabpage_Login.Controls.Add(this.labelLine2);
		this.tabpage_Login.Controls.Add(this.labelLine3);
		this.tabpage_Login.EditControl = null;
		this.tabpage_Login.EditValue = "Login";
		this.tabpage_Login.FieldName = null;
		this.tabpage_Login.IsSearchControl = false;
		this.tabpage_Login.Name = "tabpage_Login";
		this.tabpage_Login.ReadOnly = false;
		this.tabpage_Login.SettingInVisible = false;
		this.tabpage_Login.SettingReadonly = false;
		this.tabpage_Login.Size = new System.Drawing.Size(953, 70);
		this.tabpage_Login.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tabpage_Login.Text = "Login";
		this.xtraTabControl3.Dock = System.Windows.Forms.DockStyle.Fill;
		this.xtraTabControl3.HeaderOrientation = DevExpress.XtraTab.TabOrientation.Horizontal;
		this.xtraTabControl3.IsSearchControl = false;
		this.xtraTabControl3.Location = new System.Drawing.Point(0, 0);
		this.xtraTabControl3.Name = "xtraTabControl3";
		this.xtraTabControl3.SelectedTabPage = this.tabpage_Login;
		this.xtraTabControl3.ShowTabHeader = DevExpress.Utils.DefaultBoolean.True;
		this.xtraTabControl3.Size = new System.Drawing.Size(955, 95);
		this.xtraTabControl3.TabIndex = 37;
		this.xtraTabControl3.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[4] { this.tabpage_Login, this.tabpage_Search, this.tabpage_Reply, this.tabpage_Info });
		this.tabpage_Search.Controls.Add(this.checkEdit2);
		this.tabpage_Search.Controls.Add(this.checkEdit1);
		this.tabpage_Search.Controls.Add(this.cfl_BPCode_Search);
		this.tabpage_Search.Controls.Add(this.date_From);
		this.tabpage_Search.Controls.Add(this.date_To);
		this.tabpage_Search.Controls.Add(this.cfl_ItemCode_Search);
		this.tabpage_Search.Controls.Add(this.ccb_Tag);
		this.tabpage_Search.Controls.Add(this.labelLine1);
		this.tabpage_Search.Controls.Add(this.labelLine4);
		this.tabpage_Search.Controls.Add(this.ccb_OrderStatus);
		this.tabpage_Search.Controls.Add(this.btnSearch);
		this.tabpage_Search.Controls.Add(this.labelLine8);
		this.tabpage_Search.Controls.Add(this.labelLine5);
		this.tabpage_Search.Controls.Add(this.labelLine7);
		this.tabpage_Search.Controls.Add(this.labelLine6);
		this.tabpage_Search.Controls.Add(this.btnClear);
		this.tabpage_Search.EditControl = null;
		this.tabpage_Search.EditValue = "Search";
		this.tabpage_Search.FieldName = null;
		this.tabpage_Search.IsSearchControl = false;
		this.tabpage_Search.Name = "tabpage_Search";
		this.tabpage_Search.ReadOnly = false;
		this.tabpage_Search.SettingInVisible = false;
		this.tabpage_Search.SettingReadonly = false;
		this.tabpage_Search.Size = new System.Drawing.Size(953, 70);
		this.tabpage_Search.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tabpage_Search.Text = "Search";
		this.checkEdit2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.checkEdit2.EditControl = null;
		this.checkEdit2.FieldName = null;
		this.checkEdit2.IsSearchControl = false;
		this.checkEdit2.Location = new System.Drawing.Point(804, 36);
		this.checkEdit2.Name = "checkEdit2";
		this.checkEdit2.Properties.Caption = "Self Post";
		this.checkEdit2.SettingInVisible = false;
		this.checkEdit2.SettingReadonly = false;
		this.checkEdit2.Size = new System.Drawing.Size(75, 20);
		this.checkEdit2.TabIndex = 36;
		this.checkEdit1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.checkEdit1.EditControl = null;
		this.checkEdit1.FieldName = null;
		this.checkEdit1.IsSearchControl = false;
		this.checkEdit1.Location = new System.Drawing.Point(805, 11);
		this.checkEdit1.Name = "checkEdit1";
		this.checkEdit1.Properties.Caption = "Unread";
		this.checkEdit1.SettingInVisible = false;
		this.checkEdit1.SettingReadonly = false;
		this.checkEdit1.Size = new System.Drawing.Size(75, 20);
		this.checkEdit1.TabIndex = 36;
		this.tabpage_Reply.Controls.Add(this.txtCustomerContent);
		this.tabpage_Reply.Controls.Add(this.txtCustomerName);
		this.tabpage_Reply.Controls.Add(this.labelLine10);
		this.tabpage_Reply.Controls.Add(this.labelLine9);
		this.tabpage_Reply.EditControl = null;
		this.tabpage_Reply.EditValue = "Reply";
		this.tabpage_Reply.FieldName = null;
		this.tabpage_Reply.IsSearchControl = false;
		this.tabpage_Reply.Name = "tabpage_Reply";
		this.tabpage_Reply.PageVisible = false;
		this.tabpage_Reply.ReadOnly = false;
		this.tabpage_Reply.SettingInVisible = false;
		this.tabpage_Reply.SettingReadonly = false;
		this.tabpage_Reply.Size = new System.Drawing.Size(953, 70);
		this.tabpage_Reply.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tabpage_Reply.Text = "Reply";
		this.txtCustomerContent.EditControl = null;
		this.txtCustomerContent.FieldName = null;
		this.txtCustomerContent.IsSearchControl = false;
		this.txtCustomerContent.Location = new System.Drawing.Point(25, 32);
		this.txtCustomerContent.Name = "txtCustomerContent";
		this.txtCustomerContent.Properties.ReadOnly = true;
		this.txtCustomerContent.SettingInVisible = false;
		this.txtCustomerContent.SettingReadonly = false;
		this.txtCustomerContent.Size = new System.Drawing.Size(341, 20);
		this.txtCustomerContent.TabIndex = 44;
		this.txtCustomerContent.Visible = false;
		this.txtCustomerName.EditControl = null;
		this.txtCustomerName.FieldName = null;
		this.txtCustomerName.IsSearchControl = false;
		this.txtCustomerName.Location = new System.Drawing.Point(25, 5);
		this.txtCustomerName.Name = "txtCustomerName";
		this.txtCustomerName.Properties.ReadOnly = true;
		this.txtCustomerName.SettingInVisible = false;
		this.txtCustomerName.SettingReadonly = false;
		this.txtCustomerName.Size = new System.Drawing.Size(171, 20);
		this.txtCustomerName.TabIndex = 43;
		this.txtCustomerName.Visible = false;
		this.labelLine10.EditControl = null;
		this.labelLine10.EditValue = "To";
		this.labelLine10.FieldName = null;
		this.labelLine10.HideWhenEmpty = false;
		this.labelLine10.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine10.Location = new System.Drawing.Point(6, 8);
		this.labelLine10.Name = "labelLine10";
		this.labelLine10.ReadOnly = false;
		this.labelLine10.SettingInVisible = false;
		this.labelLine10.SettingReadonly = false;
		this.labelLine10.ShowUnderLine = false;
		this.labelLine10.Size = new System.Drawing.Size(23, 20);
		this.labelLine10.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine10.TabIndex = 42;
		this.labelLine10.Utext = "To";
		this.labelLine10.Visible = false;
		this.labelLine9.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom;
		this.labelLine9.EditControl = null;
		this.labelLine9.EditValue = "Quick Reply";
		this.labelLine9.FieldName = null;
		this.labelLine9.HideWhenEmpty = false;
		this.labelLine9.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine9.Location = new System.Drawing.Point(205, 8);
		this.labelLine9.Name = "labelLine9";
		this.labelLine9.ReadOnly = false;
		this.labelLine9.SettingInVisible = false;
		this.labelLine9.SettingReadonly = false;
		this.labelLine9.ShowUnderLine = false;
		this.labelLine9.Size = new System.Drawing.Size(63, 29);
		this.labelLine9.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine9.TabIndex = 49;
		this.labelLine9.Utext = "Quick Reply";
		this.tabpage_Info.Controls.Add(this.labelControl2);
		this.tabpage_Info.Controls.Add(this.labelControl1);
		this.tabpage_Info.EditControl = null;
		this.tabpage_Info.EditValue = "Info";
		this.tabpage_Info.FieldName = null;
		this.tabpage_Info.IsSearchControl = false;
		this.tabpage_Info.Name = "tabpage_Info";
		this.tabpage_Info.PageVisible = false;
		this.tabpage_Info.ReadOnly = false;
		this.tabpage_Info.SettingInVisible = false;
		this.tabpage_Info.SettingReadonly = false;
		this.tabpage_Info.Size = new System.Drawing.Size(953, 70);
		this.tabpage_Info.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tabpage_Info.Text = "Info";
		this.chkUnread.EditControl = null;
		this.chkUnread.FieldName = null;
		this.chkUnread.IsSearchControl = false;
		this.chkUnread.Location = new System.Drawing.Point(219, 1);
		this.chkUnread.Name = "chkUnread";
		this.chkUnread.Properties.Caption = "Unread";
		this.chkUnread.SettingInVisible = false;
		this.chkUnread.SettingReadonly = false;
		this.chkUnread.Size = new System.Drawing.Size(62, 19);
		this.chkUnread.TabIndex = 3;
		this.panel1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.panel1.Controls.Add(this.btnReply);
		this.panel1.Controls.Add(this.btnReplyComment);
		this.panel1.Controls.Add(this.luk_QuickReply);
		this.panel1.Controls.Add(this.memo_ReplyContent);
		this.panel1.Location = new System.Drawing.Point(4, 532);
		this.panel1.Name = "panel1";
		this.panel1.Size = new System.Drawing.Size(951, 36);
		this.panel1.TabIndex = 38;
		this.btnReply.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btnReply.EditControl = null;
		this.btnReply.EditValue = "Reply as Message";
		this.btnReply.FieldName = null;
		this.btnReply.IsSearchControl = false;
		this.btnReply.Location = new System.Drawing.Point(747, 2);
		this.btnReply.Name = "btnReply";
		this.btnReply.ReadOnly = false;
		this.btnReply.SettingInVisible = false;
		this.btnReply.SettingReadonly = false;
		this.btnReply.Size = new System.Drawing.Size(97, 24);
		this.btnReply.TabIndex = 46;
		this.btnReply.Text = "Reply as Message";
		this.btnReplyComment.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btnReplyComment.EditControl = null;
		this.btnReplyComment.EditValue = "Reply as Comment";
		this.btnReplyComment.FieldName = null;
		this.btnReplyComment.IsSearchControl = false;
		this.btnReplyComment.Location = new System.Drawing.Point(850, 3);
		this.btnReplyComment.Name = "btnReplyComment";
		this.btnReplyComment.ReadOnly = false;
		this.btnReplyComment.SettingInVisible = false;
		this.btnReplyComment.SettingReadonly = false;
		this.btnReplyComment.Size = new System.Drawing.Size(97, 22);
		this.btnReplyComment.TabIndex = 47;
		this.btnReplyComment.Text = "Reply as Comment";
		this.luk_QuickReply.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.luk_QuickReply.EditControl = null;
		this.luk_QuickReply.FieldName = null;
		this.luk_QuickReply.IsSearchControl = false;
		this.luk_QuickReply.Location = new System.Drawing.Point(720, 3);
		this.luk_QuickReply.Name = "luk_QuickReply";
		this.luk_QuickReply.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_QuickReply.Properties.CheckFields = null;
		this.luk_QuickReply.Properties.CheckTables = null;
		this.luk_QuickReply.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Id", "Name1", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Content", "Quick Replies")
		});
		this.luk_QuickReply.Properties.DisplayMember = "Content";
		this.luk_QuickReply.Properties.NullText = "";
		this.luk_QuickReply.Properties.ValueMember = "Id";
		this.luk_QuickReply.SettingInVisible = false;
		this.luk_QuickReply.SettingReadonly = false;
		this.luk_QuickReply.Size = new System.Drawing.Size(21, 20);
		this.luk_QuickReply.TabIndex = 48;
		this.luk_QuickReply.ToolTip = "Quick Reply";
		this.memo_ReplyContent.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.memo_ReplyContent.EditControl = null;
		this.memo_ReplyContent.FieldName = null;
		this.memo_ReplyContent.IsSearchControl = false;
		this.memo_ReplyContent.Location = new System.Drawing.Point(21, 3);
		this.memo_ReplyContent.Name = "memo_ReplyContent";
		this.memo_ReplyContent.SettingInVisible = false;
		this.memo_ReplyContent.SettingReadonly = false;
		this.memo_ReplyContent.Size = new System.Drawing.Size(720, 32);
		this.memo_ReplyContent.TabIndex = 45;
		this.splitContainer1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
		this.splitContainer1.Location = new System.Drawing.Point(2, 4);
		this.splitContainer1.Name = "splitContainer1";
		this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
		this.splitContainer1.Panel1.Controls.Add(this.xtraTabControl3);
		this.splitContainer1.Panel2.Controls.Add(this.xtraTabControl1);
		this.splitContainer1.Size = new System.Drawing.Size(955, 526);
		this.splitContainer1.SplitterDistance = 95;
		this.splitContainer1.TabIndex = 39;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btCancel;
		base.ClientSize = new System.Drawing.Size(955, 601);
		base.Controls.Add(this.btnSwitch);
		base.Controls.Add(this.splitContainer1);
		base.Controls.Add(this.btCancel);
		base.Controls.Add(this.btnOK);
		base.Controls.Add(this.panel1);
		base.FormId = "oFacebookCrm";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_FacebookCRM";
		base.ShowInTaskbar = false;
		this.Text = "Facebook Customer Relationship Management";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_FacebookCRM_FormClosing);
		base.Load += new System.EventHandler(this.FacebookCRM_Load);
		((System.ComponentModel.ISupportInitialize)this.rep_cfl_CustomerID).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_cfl_BPCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_cfl_BPName).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Page.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtUserName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Inbox).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Inbox).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_cfl_Message).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_date_CreatedAt.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_date_CreatedAt).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_luk_OrderStatus).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_cfl_SaleOrder).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_check_Hided).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_luk_Tag).EndInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).EndInit();
		this.xtraTabControl1.ResumeLayout(false);
		this.tabpage_Customer.ResumeLayout(false);
		this.sp_CustomerInfo.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl2).EndInit();
		this.xtraTabControl2.ResumeLayout(false);
		this.tabpage_CustomerMessage.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_CustomerInbox).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_CustomerInbox).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ccb_Tag).EndInit();
		this.tabpage_CustomerComment.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_CustomerComment).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_CustomerComment).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_cfl_Comment).EndInit();
		this.sp_CustomerInfo_Top.ResumeLayout(false);
		this.sp_CustomerInfo_Top.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.cfl_BPName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cfl_BPCode.Properties).EndInit();
		this.sp_CustomerList.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Customer).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Customer).EndInit();
		this.tabpage_Message.ResumeLayout(false);
		this.tabpage_Comment.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Comment).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Comment).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cfl_ItemCode_Search.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cfl_BPCode_Search.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_From.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_From.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_To.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_To.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ccb_OrderStatus.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ccb_Tag.Properties).EndInit();
		this.tabpage_Login.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl3).EndInit();
		this.xtraTabControl3.ResumeLayout(false);
		this.tabpage_Search.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.checkEdit2.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.checkEdit1.Properties).EndInit();
		this.tabpage_Reply.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txtCustomerContent.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtCustomerName.Properties).EndInit();
		this.tabpage_Info.ResumeLayout(false);
		this.tabpage_Info.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.chkUnread.Properties).EndInit();
		this.panel1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.luk_QuickReply.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.memo_ReplyContent.Properties).EndInit();
		this.splitContainer1.Panel1.ResumeLayout(false);
		this.splitContainer1.Panel2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.splitContainer1).EndInit();
		this.splitContainer1.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
