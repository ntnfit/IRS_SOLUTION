using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Commons;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Controls.Pivot;
using Apzon.Controls.Tree;
using Apzon.Entities.Models;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Dashboard;
using Apzon.Forms.Helper;
using Apzon.Forms.UserControls;
using Apzon.Forms.UserControls.ContextAction;
using Apzon.Forms.Widget;
using Apzon.Global;
using Apzon.HttpClientHelper;
using CrystalDecisions.CrystalReports.Engine;
using DevExpress.DashboardCommon;
using DevExpress.Data.Filtering;
using DevExpress.Snap;
using DevExpress.Snap.Extensions;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraBars.Docking2010.Views;
using DevExpress.XtraBars.Docking2010.Views.NoDocuments;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraRichEdit;
using DevExpress.XtraTab;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Nodes;

namespace Apzon.Forms.Commons;

public class frm_ContextAction : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private DataTable _dtParams = new DataTable();

	private DataTable _dtMapping = new DataTable();

	private DataTable _dtAttachment = new DataTable();

	private DataTable _dtAttachParams = new DataTable();

	private Dictionary<int, DataTable> _dicFilter = new Dictionary<int, DataTable>();

	private Dictionary<int, DevExpress.DashboardCommon.Dashboard> _dicDashboard = new Dictionary<int, DevExpress.DashboardCommon.Dashboard>();

	private Dictionary<string, UcContextActionMapping> _dicTab = new Dictionary<string, UcContextActionMapping>();

	private DataTable _dtHeaderDocument = new DataTable();

	private DataTable _dtItemDocument = new DataTable();

	private DataTable _dtBatchDocument = new DataTable();

	private DataTable _dtItemProduction = new DataTable();

	private DataTable _dtHeaderProduction = new DataTable();

	private DataTable _dtPaymentInvoice = new DataTable();

	private DataTable _dtHeaderForecast = new DataTable();

	private DataTable _dtItemForecast = new DataTable();

	private DataTable _dtApprovalDecision = new DataTable();

	private DataTable _dtHttpRequestConfig = new DataTable();

	private DataTable _dtPhysicalTable = new DataTable();

	private readonly RepositoryItemLookUpEdit _repForecastView = new RepositoryItemLookUpEdit
	{
		NullText = ""
	};

	private DataTable _dtDashboardStruct;

	private bool _isLoading;

	private string[] _listNotEditableField = new string[4] { "FormId", "ItemId", "AbsEntry", "FatherId" };

	private IContainer components;

	private SplitContainerControl splitContainerControl1;

	private TreeEx tv_Doc;

	private TreeListColumnEx treeListColumn1;

	private TreeListColumnEx treeListColumn2;

	private TreeListColumnEx treeListColumn3;

	private TreeListColumnEx treeListColumn6;

	private TreeListColumnEx treeListColumn7;

	private XtraTabControlEx xtraTab_Main;

	private XtraTabPageEx tab_SourceData;

	private LookUpEditEx look_DataFrom;

	private LabelLine lbl_DataFrom;

	private XtraTabPageEx tab_Report;

	private ButtonEditCFL bte_DataFrom;

	private GridControlEx grd_Parameters;

	private GridViewEx gv_Parameters;

	private SimpleButtonEx btn_OK;

	private SimpleButtonEx btn_Cancel;

	private ButtonEditCFL bte_RptString;

	private LookUpEdit look_ReportFrom;

	private LabelLine labelLine2;

	private XtraTabPageEx tab_Documents;

	private XtraTabControlEx tabControl_MappingDocument;

	private XtraTabPageEx xtraTabPage5;

	private GridControlEx grd_Header;

	private GridViewEx gv_Header;

	private GridColumnEx gridColumnEx1;

	private GridColumnEx gridColumnEx2;

	private GridColumnEx gridColumnEx3;

	private GridColumnEx gridColumnEx4;

	private XtraTabPageEx xtraTabPage6;

	private XtraTabPageEx tab_Udo;

	private GridControlEx grd_Items;

	private GridViewEx gv_Items;

	private GridColumnEx gridColumnEx5;

	private GridColumnEx gridColumnEx6;

	private GridColumnEx gridColumnEx7;

	private GridColumnEx gridColumnEx8;

	private ButtonEditEx bte_FilterData;

	private LabelLine lbl_FilterData;

	private RepositoryItemLookUpEdit rep_ActionType;

	private RepositoryItemLookUpEdit rep_SourceType;

	private RepositoryItemButtonEdit rep_Caption;

	private LookUpEditEx look_ObjectType;

	private LabelLine labelLine1;

	private RepositoryItemLookUpEdit rep_TargetField;

	private RepositoryItemLookUpEdit rep_TargetFieldLine;

	private LookUpEditEx look_UdoCode;

	private LabelLine labelLine3;

	private XtraTabControlEx tabControl_Udo;

	private GridColumnEx gridColumnEx9;

	private GridColumnEx gridColumnEx10;

	private GridColumnEx gridColumnEx11;

	private RepositoryItemLookUpEdit rep_DataType;

	private GridColumnEx gridColumnEx12;

	private GridColumnEx gridColumnEx13;

	private RepositoryItemCheckEdit repositoryItemCheckEdit1;

	private GridColumnEx gridColumnEx16;

	private GridControlEx grd_Filter;

	private GridViewEx gv_Filter;

	private MemoExEditEx memo_DataString;

	private MemoExEditEx memo_RptString;

	private GridControlEx grd_Report;

	private GridViewEx gv_Report;

	private GridColumnEx gridColumnEx15;

	private GridColumnEx gridColumnEx17;

	private GridColumnEx gridColumnEx18;

	private RepositoryItemLookUpEdit rep_ReportDataType;

	private GridColumnEx gridColumnEx19;

	private GridColumnEx gridColumnEx20;

	private GridColumnEx gridColumnEx21;

	private GridColumnEx gridColumnEx22;

	private RepositoryItemCheckEdit repositoryItemCheckEdit2;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem removeToolStripMenuItem;

	private TreeListColumnEx treeListColumn4;

	private RepositoryItemLookUpEdit rep_DataRange;

	private RepositoryItemLookUpEdit rep_SourceField;

	private RepositoryItemLookUpEdit rep_SourceFieldHeader;

	private RepositoryItemLookUpEdit rep_SourceFieldItem;

	private GridColumnEx gridColumnEx14;

	private RepositoryItemLookUpEdit rep_SourceFieldData;

	private GridColumnEx gridColumnEx23;

	private GridColumnEx gridColumnEx24;

	private CheckEditEx chk_ShowDocForm;

	private CheckEditEx chk_ShowUdoForm;

	private XtraTabPageEx xtraTabPage1;

	private GridControlEx grd_Batch;

	private GridViewEx gv_Batch;

	private GridColumnEx gridColumnEx25;

	private GridColumnEx gridColumnEx26;

	private RepositoryItemLookUpEdit rep_SourceFieldBatch;

	private GridColumnEx gridColumnEx27;

	private GridColumnEx gridColumnEx28;

	private RepositoryItemLookUpEdit rep_TargetFieldBatch;

	private GridColumnEx gridColumnEx29;

	private DropDownButton drop_ExportSetting;

	private SimpleButtonEx btn_Import;

	private GridColumnEx gridColumnEx30;

	private RepositoryItemLookUpEdit rep_SumTypeHeader;

	private GridColumnEx gridColumnEx31;

	private RepositoryItemLookUpEdit rep_SumTypeLine;

	private GridColumnEx gridColumnEx32;

	private RepositoryItemLookUpEdit rep_SumTypeBatch;

	private XtraTabPageEx tab_Dashboard;

	private SimpleButtonEx btn_EditDesign;

	private UcDashboardViewer ucDashboardViewer1;

	private SimpleButtonEx btn_ChooseDashboard;

	private XtraTabPageEx tab_ProductionOrder;

	private CheckEditEx chk_ShowProductionForm;

	private XtraTabControlEx tabControl_MappingProduction;

	private XtraTabPageEx xtraTabPage2;

	private GridControlEx grd_ProductionHeader;

	private GridViewEx gv_ProductionHeader;

	private GridColumnEx gridColumnEx33;

	private GridColumnEx gridColumnEx34;

	private RepositoryItemLookUpEdit rep_ProductionHeaderSourceField;

	private GridColumnEx gridColumnEx35;

	private GridColumnEx gridColumnEx36;

	private RepositoryItemLookUpEdit rep_ProductionTargetHeader;

	private GridColumnEx gridColumnEx37;

	private GridColumnEx gridColumnEx38;

	private RepositoryItemLookUpEdit rep_ProductionHeaderSumType;

	private XtraTabPageEx xtraTabPage3;

	private GridControlEx grd_ProductionItem;

	private GridViewEx gv_ProductionItem;

	private GridColumnEx gridColumnEx39;

	private GridColumnEx gridColumnEx40;

	private RepositoryItemLookUpEdit rep_ProductionLineSourceField;

	private GridColumnEx gridColumnEx41;

	private GridColumnEx gridColumnEx42;

	private RepositoryItemLookUpEdit rep_ProductionOrderTargetLine;

	private GridColumnEx gridColumnEx43;

	private GridColumnEx gridColumnEx44;

	private RepositoryItemLookUpEdit rep_ProductionLineSumType;

	private XtraTabPageEx xtraTabPage4;

	private GridControlEx grd_ProductionBatch;

	private GridViewEx gv_ProductionBatch;

	private GridColumnEx gridColumnEx45;

	private GridColumnEx gridColumnEx46;

	private RepositoryItemLookUpEdit rep_ProductionBatchSourceField;

	private GridColumnEx gridColumnEx47;

	private GridColumnEx gridColumnEx48;

	private RepositoryItemLookUpEdit rep_ProdoctionTargetBatch;

	private GridColumnEx gridColumnEx49;

	private GridColumnEx gridColumnEx50;

	private RepositoryItemLookUpEdit rep_ProductionBatchSumType;

	private XtraTabPageEx tab_Email;

	private LabelLine labelLine4;

	private LabelLine labelLine6;

	private LabelLine labelLine5;

	private LabelLine labelLine7;

	private LabelLine labelLine8;

	private ButtonEditEx txt_Subject;

	private ButtonEditEx txt_AddressBcc;

	private ButtonEditEx txt_AddressCc;

	private ButtonEditEx txt_AddressTo;

	private ButtonEditEx txt_AddressFrom;

	private XtraTabControlEx xtraTabControl4;

	private XtraTabPageEx xtraTabPage8;

	private XtraTabPageEx xtraTabPage9;

	private SplitContainerControl splitContainerControl2;

	private GridControlEx grd_AttachFiles;

	private GridViewEx gv_AttachFiles;

	private GridControlEx grd_RptParam;

	private GridViewEx gv_RptParam;

	private GridColumnEx gridColumnEx51;

	private GridColumnEx gridColumnEx52;

	private GridColumnEx gridColumnEx53;

	private GridColumnEx gridColumnEx54;

	private GridColumnEx gridColumnEx55;

	private GridColumnEx gridColumnEx56;

	private GridColumnEx gridColumnEx57;

	private RepositoryItemLookUpEdit rep_AttParamDataType;

	private RepositoryItemLookUpEdit rep_AttParamSourceField;

	private RepositoryItemCheckEdit repositoryItemCheckEdit3;

	private GridColumnEx gridColumnEx58;

	private GridColumnEx gridColumnEx59;

	private GridColumnEx gridColumnEx60;

	private RepositoryItemButtonEdit rep_Path;

	private RepositoryItemLookUpEdit rep_AttType;

	private SnapControl snapControl1;

	private SnapDockManager snapDockManager1;

	private SnapDocumentManager snapDocumentManager1;

	private NoDocumentsView noDocumentsView1;

	private OpenFileDialog openFileDialog1;

	private TreeListColumnEx treeListColumn5;

	private SimpleButton btn_EditEmailContent;

	private TreeListColumnEx treeListColumn8;

	private RepositoryItemButtonEdit rep_MsgSuccess;

	private TreeListColumnEx treeListColumn9;

	private RepositoryItemLookUpEdit rep_MsgType;

	private LookUpEditEx look_EmailGroupBy;

	private LabelLine labelLine9;

	private LabelLine labelLine10;

	private ButtonEditEx txt_EmailGroupByFields;

	private XtraTabPage tab_Forecast;

	private XtraTabControlEx xtraTabControlEx1;

	private XtraTabPageEx xtraTabPageEx1;

	private GridControlEx grd_ForecastHeader;

	private GridViewEx gv_ForecastHeader;

	private GridColumnEx gridColumnEx73;

	private GridColumnEx gridColumnEx74;

	private RepositoryItemLookUpEdit rep_ForecastSourceField;

	private GridColumnEx gridColumnEx75;

	private GridColumnEx gridColumnEx76;

	private RepositoryItemLookUpEdit rep_ForecastTargetField;

	private GridColumnEx gridColumnEx77;

	private XtraTabPageEx xtraTabPageEx2;

	private GridControlEx grd_ForecastItems;

	private GridViewEx gv_ForecastItems;

	private GridColumnEx gridColumnEx79;

	private GridColumnEx gridColumnEx80;

	private RepositoryItemLookUpEdit rep_ForecastItemSourceField;

	private GridColumnEx gridColumnEx81;

	private GridColumnEx gridColumnEx82;

	private RepositoryItemLookUpEdit rep_ForecastItemTargetField;

	private GridColumnEx gridColumnEx83;

	private CheckEditEx chk_ForecastShowWindow;

	private RepositoryItemTextEdit repositoryItemTextEdit1;

	private TreeListColumnEx treeListColumnEx1;

	private RepositoryItemLookUpEdit rep_Method;

	private LookUpEditEx look_DocEntry;

	private LabelLine lb_DocumentDocEntry;

	private LookUpEditEx look_ProductionEntry;

	private LabelLine lb_ProductionEntry;

	private TreeListColumnEx treeListColumnEx2;

	private RepositoryItemLookUpEdit rep_EditorType;

	private TreeListColumnEx treeListColumnEx3;

	private XtraTabPageEx tab_DoQuery;

	private MemoExEditEx memo_QueryString;

	private ButtonEditCFL txt_QueryString;

	private LookUpEditEx look_QueryFrom;

	private LabelLine labelLine11;

	private LookUpEditEx look_ParameterType;

	private LabelLine lbl_ParameterType;

	private ContextMenuStrip contextMenuStrip2;

	private ToolStripMenuItem removeRowToolStripMenuItem;

	private XtraTabPageEx tab_PrintLayout;

	private LabelLine labelLine13;

	private ButtonEditEx bte_PrintLayoutPath;

	private LookUpEditEx look_PrintLayoutFileType;

	private LabelLine labelLine14;

	private GridControlEx grd_PrintLayout;

	private GridViewEx gv_PrintLayout;

	private GridColumnEx gridColumnEx90;

	private GridColumnEx gridColumnEx91;

	private GridColumnEx gridColumnEx92;

	private GridColumnEx gridColumnEx93;

	private GridColumnEx gridColumnEx94;

	private GridColumnEx gridColumnEx95;

	private GridColumnEx gridColumnEx96;

	private OpenFileDialog openFileDialog2;

	private XtraTabPageEx tab_ApprovalDecision;

	private GridControlEx grd_Approval;

	private GridViewEx gv_Approval;

	private GridColumnEx gridColumnEx97;

	private GridColumnEx gridColumnEx98;

	private RepositoryItemLookUpEdit rep_ApprovalSourceField;

	private GridColumnEx gridColumnEx99;

	private GridColumnEx gridColumnEx100;

	private RepositoryItemLookUpEdit rep_ApprovalTargetField;

	private GridColumnEx gridColumnEx101;

	private RepositoryItemLookUpEdit repositoryItemLookUpEdit3;

	private XtraTabPageEx tab_DIYScript;

	private SimpleButton btn_EditScript;

	private RichTextBox rtxt_Script;

	private LookUpEditEx look_RptUseSourceFrom;

	private LabelLine labelLine15;

	private ToolTipController toolTipController1;

	private LabelLine lbl_TemporaryName;

	private RepositoryItemLookUpEdit rep_PrintLayoutDataType;

	private RepositoryItemLookUpEdit rep_PrintLayoutSourceField;

	private TextEditEx txt_TemporaryTableName;

	private SimpleButton btn_PublishService;

	private XtraTabPageEx tab_HttpRequest;

	private UcContextActionHttpRequest ucHttpRequest;

	private TreeListColumnEx treeListColumnEx4;

	private TreeListColumnEx treeListColumnEx5;

	private RepositoryItemCheckEdit chk_Invisible;

	private TreeListColumnEx treeListColumnEx6;

	private RepositoryItemImageComboBox rep_Icon;

	private ImageCollection imageCollection1;

	private XtraTabPageEx tab_PhysicalTable;

	private LabelLine labelLine12;

	private TextEditEx txt_PhysicalTableName;

	private GridControlEx grd_PhysicalTable;

	private GridViewEx gv_PhysicalTable;

	private GridColumnEx gridColumnEx102;

	private GridColumnEx gridColumnEx103;

	private RepositoryItemLookUpEdit rep_PhysicalTableSourceField;

	private GridColumnEx gridColumnEx104;

	private GridColumnEx gridColumnEx105;

	private RepositoryItemLookUpEdit rep_PhysicalTableTargetField;

	private GridColumnEx gridColumnEx106;

	private GridColumnEx gridColumnEx107;

	private RepositoryItemLookUpEdit repositoryItemLookUpEdit4;

	private LookUpEditEx look_PrintAction;

	private GridControlEx grd_DoQuery;

	private GridViewEx gv_DoQuery;

	private GridColumnEx gridColumnEx78;

	private GridColumnEx gridColumnEx84;

	private GridColumnEx gridColumnEx85;

	private RepositoryItemLookUpEdit rep_QueryDataType;

	private GridColumnEx gridColumnEx86;

	private RepositoryItemLookUpEdit rep_QuerySourceField;

	private GridColumnEx gridColumnEx87;

	private GridColumnEx gridColumnEx88;

	private GridColumnEx gridColumnEx89;

	private RepositoryItemCheckEdit repositoryItemCheckEdit4;

	private CheckEdit chk_JoinValue;

	private TreeListColumnEx treeListColumnIgnoreError;

	private RepositoryItemCheckEdit rIgnoreError;

	private TextEditEx txt_TempTableNameDoQuery;

	private LabelLine lbl_TempTableNameDoQuery;

	private CheckEditEx chk_UseTempTable;

	private TreeListColumnEx treeListColumnEx7;

	private string _description { get; set; }

	public DataTable UiDataSource { get; set; }

	private DataTable UiSourceField { get; set; }

	private DataTable DatabaseSourceField { get; set; }

	public GridViewEx GridSource { get; set; }

	public PivotGridControlEx PivotSource { get; set; }

	public VGridControlEx VGridSource { get; set; }

	public List<ViewEditModel> ViewEditModels { get; set; }

	public TreeEx TreeSource { get; set; }

	public string ActionFormId { get; set; }

	public string ActionItemId { get; set; }

	public XtraFormEx BaseForm { get; set; }

	public Dictionary<int, bool> DicSourceColumn { get; set; }

	public bool OnlyOneAction { get; set; }

	public int FirstEntry
	{
		get
		{
			try
			{
				DataTable dataSourceTable = tv_Doc.DataSourceTable;
				if (dataSourceTable.IsNotNull())
				{
					return Function.ParseInt(dataSourceTable.AsEnumerable().FirstOrDefault((DataRow t) => !string.IsNullOrEmpty(Function.ToString(t["Caption"])))?["AbsEntry"]);
				}
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", ex);
				WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			}
			return 0;
		}
	}

	public bool CloseFormAfterSave { get; set; }

	public frm_ContextAction()
	{
		InitializeComponent();
	}

	public frm_ContextAction(string actionFormId, string actionItemId)
	{
		Init(actionFormId, actionItemId);
	}

	public frm_ContextAction(string actionFormId, string actionItemId, DataTable datasource)
	{
		Init(actionFormId, actionItemId);
		UiDataSource = datasource;
	}

	public frm_ContextAction(string actionFormId, string actionItemId, DataTable datasource, string description = "")
	{
		Init(actionFormId, actionItemId);
		UiDataSource = datasource;
		_description = description;
	}

	public frm_ContextAction(string actionFormId, string actionItemId, DataTable datasource, GridViewEx gv)
	{
		Init(actionFormId, actionItemId);
		UiDataSource = datasource;
		GridSource = gv;
		BaseForm = gv?.GridControl?.FindForm() as XtraFormEx;
	}

	public frm_ContextAction(string actionFormId, string actionItemId, DataTable datasource, TreeEx tv)
	{
		Init(actionFormId, actionItemId);
		UiDataSource = datasource;
		TreeSource = tv;
		BaseForm = tv?.FindForm() as XtraFormEx;
	}

	public frm_ContextAction(string actionFormId, string actionItemId, DataTable datasource, PivotGridControlEx gv)
	{
		Init(actionFormId, actionItemId);
		UiDataSource = datasource;
		PivotSource = gv;
		BaseForm = gv?.FindForm() as XtraFormEx;
	}

	public frm_ContextAction(string actionFormId, string actionItemId, DataTable datasource, VGridControlEx vc)
	{
		Init(actionFormId, actionItemId);
		UiDataSource = datasource;
		VGridSource = vc;
		BaseForm = vc?.FindForm() as XtraFormEx;
	}

	public frm_ContextAction(string actionFormId, string actionItemId, List<ViewEditModel> viewEditModel)
	{
		Init(actionFormId, actionItemId);
		ViewEditModels = viewEditModel;
	}

	public frm_ContextAction(string actionFormId, string actionItemId, List<ViewEditModel> viewEditModel, XtraFormEx form)
	{
		Init(actionFormId, actionItemId);
		ViewEditModels = viewEditModel;
		BaseForm = form;
	}

	public frm_ContextAction(string actionFormId, string actionItemId, DataTable datasource, XtraFormEx form)
	{
		Init(actionFormId, actionItemId);
		UiDataSource = datasource;
		BaseForm = form;
	}

	public frm_ContextAction(string actionFormId, string actionItemId, DataSet datasource, XtraFormEx form)
	{
		Init(actionFormId, actionItemId);
		BaseForm = form;
	}

	private void Init(string actionFormId, string actionItemId)
	{
		InitializeComponent();
		if (string.IsNullOrEmpty(actionFormId))
		{
			throw new ArgumentNullException("actionFormId");
		}
		ActionFormId = actionFormId;
		if (string.IsNullOrEmpty(actionItemId))
		{
			throw new ArgumentNullException("actionItemId");
		}
		ActionItemId = actionItemId;
		tab_Forecast.PageVisible = false;
		tab_PrintLayout.PageVisible = false;
		tab_Documents.PageVisible = false;
		tab_Report.PageVisible = false;
		tab_Udo.PageVisible = false;
		tab_Dashboard.PageVisible = false;
		tab_ProductionOrder.PageVisible = false;
		tab_Email.PageVisible = false;
		tab_Forecast.Visible = false;
		lb_DocumentDocEntry.Visible = false;
		look_DocEntry.Visible = false;
		lb_ProductionEntry.Visible = false;
		look_ProductionEntry.Visible = false;
		tabControl_MappingDocument.Visible = false;
		tabControl_MappingProduction.Visible = false;
		tab_DoQuery.PageVisible = false;
		tab_ApprovalDecision.PageVisible = false;
		tab_DIYScript.PageVisible = false;
		tab_HttpRequest.PageVisible = false;
		tab_PhysicalTable.PageVisible = false;
		UcContextActionHttpRequest ucContextActionHttpRequest = ucHttpRequest;
		ucContextActionHttpRequest.VerbsChanged = (Action<string>)Delegate.Combine(ucContextActionHttpRequest.VerbsChanged, new Action<string>(HttpRequest_VerbsChanged));
		UcContextActionHttpRequest ucContextActionHttpRequest2 = ucHttpRequest;
		ucContextActionHttpRequest2.UriChanged = (Action<string>)Delegate.Combine(ucContextActionHttpRequest2.UriChanged, new Action<string>(HttpRequest_UriChanged));
		UcContextActionHttpRequest ucContextActionHttpRequest3 = ucHttpRequest;
		ucContextActionHttpRequest3.AuthTypeChanged = (Action<string>)Delegate.Combine(ucContextActionHttpRequest3.AuthTypeChanged, new Action<string>(HttpRequest_AuthTypeChanged));
		UcContextActionHttpRequest ucContextActionHttpRequest4 = ucHttpRequest;
		ucContextActionHttpRequest4.DecryptPwdChanged = (Action<string>)Delegate.Combine(ucContextActionHttpRequest4.DecryptPwdChanged, new Action<string>(HttpRequest_DecryptPwdChanged));
		UcContextActionHttpRequest ucContextActionHttpRequest5 = ucHttpRequest;
		ucContextActionHttpRequest5.BodyTypeChanged = (Action<string>)Delegate.Combine(ucContextActionHttpRequest5.BodyTypeChanged, new Action<string>(HttpRequest_BodyTypeChanged));
		UcContextActionHttpRequest ucContextActionHttpRequest6 = ucHttpRequest;
		ucContextActionHttpRequest6.ResponseTypeChanged = (Action<string>)Delegate.Combine(ucContextActionHttpRequest6.ResponseTypeChanged, new Action<string>(HttpRequest_ResponseTypeChanged));
		UcContextActionHttpRequest ucContextActionHttpRequest7 = ucHttpRequest;
		ucContextActionHttpRequest7.MappingChanged = (Action)Delegate.Combine(ucContextActionHttpRequest7.MappingChanged, new Action(HttpRequest_MappingChanged));
	}

	private void HttpRequest_MappingChanged()
	{
		try
		{
			Control_EditValueChanged(null, null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void HttpRequest_ResponseTypeChanged(string obj)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Busy && tv_Doc.FocusedNode != null)
			{
				tv_Doc.FocusedNode.SetValue("HttpResponseType", obj);
				tv_Doc.DataSourceTable().AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void HttpRequest_BodyTypeChanged(string obj)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Busy && tv_Doc.FocusedNode != null)
			{
				tv_Doc.FocusedNode.SetValue("HttpBodyType", obj);
				tv_Doc.DataSourceTable().AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void HttpRequest_DecryptPwdChanged(string obj)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Busy && tv_Doc.FocusedNode != null)
			{
				tv_Doc.FocusedNode.SetValue("HttpDecryptPwd", obj);
				tv_Doc.DataSourceTable().AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void HttpRequest_AuthTypeChanged(string obj)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Busy && tv_Doc.FocusedNode != null)
			{
				tv_Doc.FocusedNode.SetValue("HttpAuthType", obj);
				tv_Doc.DataSourceTable().AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void HttpRequest_UriChanged(string obj)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Busy && tv_Doc.FocusedNode != null)
			{
				tv_Doc.FocusedNode.SetValue("HttpUri", obj);
				tv_Doc.DataSourceTable().AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void HttpRequest_VerbsChanged(string obj)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Busy && tv_Doc.FocusedNode != null)
			{
				tv_Doc.FocusedNode.SetValue("HttpVerb", obj);
				tv_Doc.DataSourceTable().AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		try
		{
			_formMode = ((value == 0) ? Enums.FormViewModeType.View : value);
			switch (_formMode)
			{
			case Enums.FormViewModeType.Add:
			{
				SimpleButtonEx simpleButtonEx4 = btn_OK;
				bool enabled = (btn_Cancel.Enabled = true);
				simpleButtonEx4.Enabled = enabled;
				btn_OK.Text = Resources.ButtonAdd;
				break;
			}
			case Enums.FormViewModeType.Update:
			{
				SimpleButtonEx simpleButtonEx3 = btn_OK;
				bool enabled = (btn_Cancel.Enabled = true);
				simpleButtonEx3.Enabled = enabled;
				btn_OK.Text = Resources.ButtonUpdate;
				break;
			}
			case Enums.FormViewModeType.View:
			{
				SimpleButtonEx simpleButtonEx2 = btn_OK;
				bool enabled = (btn_Cancel.Enabled = true);
				simpleButtonEx2.Enabled = enabled;
				btn_OK.Text = Resources.ButtonOK;
				break;
			}
			case Enums.FormViewModeType.Busy:
			{
				SimpleButtonEx simpleButtonEx = btn_OK;
				bool enabled = (btn_Cancel.Enabled = false);
				simpleButtonEx.Enabled = enabled;
				break;
			}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void GetExportRow(DataRow row, ref DataTable dtDoc, ref DataTable dtPr, ref DataTable dtMap)
	{
		DataRow dataRow = dtDoc.NewRow();
		dataRow.ItemArray = row.ItemArray;
		dtDoc.Rows.Add(dataRow);
		int entry = Function.ParseInt(row["AbsEntry"]);
		DataRow[] array = (from t in _dtParams.AsEnumerable()
			where Function.ParseInt(t["AbsEntry"]) == entry
			select t).ToArray();
		if (array.Length != 0)
		{
			DataRow[] array2 = array;
			foreach (DataRow dataRow2 in array2)
			{
				DataRow dataRow3 = dtPr.NewRow();
				dataRow3.ItemArray = dataRow2.ItemArray;
				dtPr.Rows.Add(dataRow3);
			}
		}
		dtPr.AcceptChanges();
		DataRow[] array3 = (from t in _dtMapping.AsEnumerable()
			where Function.ParseInt(t["AbsEntry"]) == entry
			select t).ToArray();
		if (array3.Length != 0)
		{
			DataRow[] array2 = array3;
			foreach (DataRow dataRow4 in array2)
			{
				DataRow dataRow5 = dtMap.NewRow();
				dataRow5.ItemArray = dataRow4.ItemArray;
				dtMap.Rows.Add(dataRow5);
			}
		}
		dtMap.AcceptChanges();
	}

	private async Task frm_ContextMenu_Load(object sender, EventArgs e)
	{
		_ = 7;
		try
		{
			tv_Doc.ShowLoadingPanel();
			FormMode = Enums.FormViewModeType.Busy;
			DXPopupMenu dXPopupMenu = new DXPopupMenu();
			dXPopupMenu.Items.Add(new DXMenuItem("Export Focused Row", (object s, EventArgs ev) =>
			{
				try
				{
					DataTable dataSourceTable = tv_Doc.DataSourceTable;
					DataRow focusedDataRow = tv_Doc.GetFocusedDataRow();
					if (focusedDataRow != null)
					{
						DataTable dtDoc = new DataRow[1] { focusedDataRow }.CopyToDataTable();
						dtDoc.TableName = "APZ_OCTA";
						int entry = Function.ParseInt(focusedDataRow["AbsEntry"]);
						InsertDataParams("S", grd_Parameters.DataSourceTable.Copy());
						InsertDataParams("R", grd_Report.DataSourceTable.Copy());
						DataTable dtPr = new DataTable();
						InsertDataMapping("D", "H", grd_Header.DataSourceTable.Copy());
						InsertDataMapping("D", "I", grd_Items.DataSourceTable.Copy());
						InsertDataMapping("D", "B", grd_Batch.DataSourceTable.Copy());
						InsertDataMapping("W", "H", grd_ProductionHeader.DataSourceTable.Copy());
						InsertDataMapping("W", "I", grd_ProductionItem.DataSourceTable.Copy());
						InsertDataMapping("W", "B", grd_ProductionBatch.DataSourceTable.Copy());
						DataTable dtMap = new DataTable();
						DataRow[] array = (from t in _dtParams.AsEnumerable()
							where Function.ParseInt(t["AbsEntry"]) == entry
							select t).ToArray();
						if (array.Length != 0)
						{
							dtPr = array.CopyToDataTable();
							dtPr.TableName = "APZ_CTA1";
						}
						DataRow[] array2 = (from t in _dtMapping.AsEnumerable()
							where Function.ParseInt(t["AbsEntry"]) == entry
							select t).ToArray();
						if (array2.Length != 0)
						{
							dtMap = array2.CopyToDataTable();
							dtMap.TableName = "APZ_CTA2";
						}
						foreach (DataRow item in from t in dataSourceTable.AsEnumerable()
							where Function.ParseInt(t["FatherId"]) == entry
							select t)
						{
							GetExportRow(item, ref dtDoc, ref dtPr, ref dtMap);
						}
						DataSet dataSet = new DataSet();
						dataSet.Tables.AddRange(new DataTable[3] { dtDoc, dtPr, dtMap });
						ExportDataSet(dataSet);
					}
				}
				catch (Exception ex2)
				{
					Logging.Write("ERROR", ex2);
					WinCommons.ErrorLineNumber(ex2);
				}
			}));
			drop_ExportSetting.DropDownControl = dXPopupMenu;
			rep_ActionType.SetDataSource(new Dictionary<string, string>
			{
				{
					0.ToString(),
					"None"
				},
				{
					7000030.ToString(),
					"Dashboard"
				},
				{
					810015.ToString(),
					"E-Mail"
				},
				{
					17.ToString(),
					"Maketing Document"
				},
				{
					202.ToString(),
					"Production Order"
				},
				{
					1200001.ToString(),
					"Query Report"
				},
				{
					206000.ToString(),
					"User-Defined Object"
				},
				{
					8000005.ToString(),
					"Forecast"
				},
				{
					66.ToString(),
					"Bill of Material"
				},
				{
					2.ToString(),
					"Business Partner Cards"
				},
				{
					4.ToString(),
					"Items"
				},
				{
					7000037.ToString(),
					"Do Query"
				},
				{
					7000038.ToString(),
					"Workflow/Process Group"
				},
				{
					7000039.ToString(),
					"Print Layout"
				},
				{
					27.ToString(),
					"Journal Voucher"
				},
				{
					30.ToString(),
					"Journal Entry"
				},
				{
					121.ToString(),
					"Approval Decision"
				},
				{
					1470000128.ToString(),
					"Task"
				},
				{
					1470000129.ToString(),
					"Event"
				},
				{
					19000102.ToString(),
					"Note"
				},
				{
					1470000130.ToString(),
					"Tracking Issue"
				},
				{
					1470000132.ToString(),
					"Project Management"
				},
				{
					6.ToString(),
					"Opportunity"
				},
				{
					9.ToString(),
					"Http Request"
				},
				{
					7000040.ToString(),
					"Script"
				},
				{
					1.ToString(),
					"Chart Of Account"
				},
				{
					62.ToString(),
					"Distribute Rules"
				},
				{
					5.ToString(),
					"VAT Groups"
				},
				{
					53.ToString(),
					"Sales Employees/Buyers"
				},
				{
					27110.ToString(),
					"PhysicalTable"
				},
				{
					1250000025.ToString(),
					"Blanket"
				},
				{
					24.ToString(),
					"Payment"
				},
				{
					321.ToString(),
					"Internal Reconciliations"
				},
				{
					69.ToString(),
					"Purchase Landed Cost"
				},
				{
					64.ToString(),
					"Warehouses"
				}
			});
			rep_Method.SetDataSource(new Dictionary<string, string>
			{
				{ "A", "Add New" },
				{ "U", "Update" },
				{ "R", "Release" },
				{ "L", "Close" },
				{ "C", "Cancel" },
				{ "S", "Add Or Update" },
				{ "D", "Save Draft to Document" },
				{ "G", "Get" },
				{ "P", "Post" },
				{ "F", "Save As Draft" }
			});
			rep_SourceType.SetDataSource(new Dictionary<string, string>
			{
				{ "N", "None" },
				{ "U", "UI" },
				{ "D", "Database" },
				{ "I", "Import from excel" }
			});
			rep_AttType.SetDataSource(new Dictionary<string, string>
			{
				{ "L", "General" },
				{ "R", "Report" }
			});
			look_PrintLayoutFileType.SetDataSource(new Dictionary<string, string>
			{
				{ "L", "General" },
				{ "R", "Report" }
			});
			look_PrintAction.SetDataSource(new Dictionary<string, string>
			{
				{ "V", "Preview" },
				{ "P", "Print" }
			});
			if (ActionFormId == ApzObjectTypes.oDashboardDetail.ToString())
			{
				rep_DataRange.SetDataSource(new Dictionary<string, string>
				{
					{ "S", "Selected Item" },
					{ "A", "All" }
				});
			}
			else
			{
				rep_DataRange.SetDataSource(new Dictionary<string, string>
				{
					{ "F", "Focused Row" },
					{ "S", "Selected Row(s)" },
					{ "A", "All" }
				});
			}
			rep_MsgType.SetDataSource(new Dictionary<string, string>
			{
				{ "-", "None" },
				{ "S", "Status bar" },
				{ "M", "Message Box" }
			});
			look_EmailGroupBy.SetDataSource(new Dictionary<string, string>
			{
				{ "N", "One Email" },
				{ "R", "Each row is an Email" },
				{ "F", "Group by Fields" }
			});
			_repForecastView.SetDataSource(new Dictionary<string, string>
			{
				{ "D", "Daily" },
				{ "W", "Weekly" },
				{ "M", "Monthly" }
			});
			look_ParameterType.SetDataSource(new Dictionary<string, string>
			{
				{ "V", "Variables(Default)" },
				{ "T", "Tempory Table" }
			});
			look_RptUseSourceFrom.SetDataSource(new Dictionary<string, string>
			{
				{ "R", "Report Source" },
				{ "I", "Input Source" }
			});
			for (int num = 0; num < imageCollection1.Images.Count; num++)
			{
				string text = imageCollection1.Images.Keys[num].Replace("_16x16.png", "");
				rep_Icon.Items.Add(text, text, num);
			}
			rep_Icon.Items.Add(null, null, -1);
			DataTable dataTable = await _httpClient.Udo.GetListUdo();
			if (dataTable.IsNotNull())
			{
				DataTable dataTable2 = new DataTable();
				dataTable2.Columns.Add("Code", typeof(string));
				dataTable2.Columns.Add("Name", typeof(string));
				DataRow dataRow = dataTable2.NewRow();
				dataRow["Name"] = DBNull.Value;
				dataRow["Code"] = DBNull.Value;
				dataTable2.Rows.Add(dataRow);
				for (int num2 = 0; num2 < dataTable.Rows.Count; num2++)
				{
					dataRow = dataTable2.NewRow();
					dataRow["Name"] = Function.ToString(dataTable.Rows[num2]["Name"]);
					dataRow["Code"] = Function.ToString(dataTable.Rows[num2]["Code"]);
					dataTable2.Rows.Add(dataRow);
				}
				look_UdoCode.Properties.DataSource = (from t in dataTable2.AsEnumerable()
					orderby Function.ToString(t["Name"])
					select t).CopyToDataTable();
				look_UdoCode.Properties.DisplayMember = "Name";
				look_UdoCode.Properties.ValueMember = "Code";
			}
			look_DataFrom.SetDataSource(new Dictionary<string, string>
			{
				{ "Q", "Defined Query" },
				{ "T", "Query Text" }
			});
			look_QueryFrom.SetDataSource(new Dictionary<string, string>
			{
				{ "Q", "Defined Query" },
				{ "T", "Query Text" }
			});
			look_ReportFrom.SetDataSource(new Dictionary<string, string>
			{
				{ "Q", "Defined Query" },
				{ "T", "Query Text" },
				{ "U", "User-Defined Report" }
			});
			bte_DataFrom.Properties.DisplayMember = "QName";
			bte_DataFrom.Properties.ValueMember = "IntrnalKey";
			bte_DataFrom.Properties.TypeCFL = Enums.TypeCFL.Queries;
			bte_DataFrom.Properties.RefreshDataSource();
			RepositoryItemButtonEditCFL properties = bte_DataFrom.Properties;
			properties.ItemSelected = (Action<DataRow, GridView>)Delegate.Combine(properties.ItemSelected, new Action<DataRow, GridView>(DataFrom_ItemSelected));
			memo_DataString.Size = bte_DataFrom.Size;
			memo_DataString.Location = bte_DataFrom.Location;
			RepositoryItemButtonEditCFL properties2 = bte_RptString.Properties;
			properties2.ItemSelected = (Action<DataRow, GridView>)Delegate.Combine(properties2.ItemSelected, new Action<DataRow, GridView>(RptString_ItemSelected));
			memo_RptString.Size = bte_RptString.Size;
			memo_RptString.Location = bte_RptString.Location;
			txt_QueryString.Properties.DisplayMember = "QName";
			txt_QueryString.Properties.ValueMember = "IntrnalKey";
			txt_QueryString.Properties.TypeCFL = Enums.TypeCFL.Queries;
			txt_QueryString.Properties.RefreshDataSource();
			RepositoryItemButtonEditCFL properties3 = txt_QueryString.Properties;
			properties3.ItemSelected = (Action<DataRow, GridView>)Delegate.Combine(properties3.ItemSelected, new Action<DataRow, GridView>(QueryString_ItemSelected));
			memo_QueryString.Size = txt_QueryString.Size;
			memo_QueryString.Location = txt_QueryString.Location;
			DataTable dataTable3 = new DataTable();
			dataTable3.Columns.Add("Code", typeof(string));
			dataTable3.Columns.Add("Name", typeof(string));
			foreach (Enums.ContextActionDocumentType value4 in Enum.GetValues(typeof(Enums.ContextActionDocumentType)))
			{
				DataRow dataRow2 = dataTable3.NewRow();
				dataRow2["Code"] = (int)value4;
				string text2 = Function.ToString(value4);
				dataRow2["Name"] = Function.SpaceBeforeCapital(text2.Substring(1, text2.Length - 1));
				dataTable3.Rows.Add(dataRow2);
			}
			look_ObjectType.SetDataSource(dataTable3, "Name", "Code", emptyRow: true);
			DataTable dataTable4 = dataTable3.Clone();
			foreach (Enums.FormTemplateEditorType value5 in Enum.GetValues(typeof(Enums.FormTemplateEditorType)))
			{
				DataRow dataRow3 = dataTable4.NewRow();
				dataRow3["Code"] = (int)value5;
				dataRow3["Name"] = Function.SpaceBeforeCapital(Function.ToString(value5));
				dataTable4.Rows.Add(dataRow3);
			}
			rep_EditorType.SetDataSource(dataTable4, "Name", "Code", emptyRow: true);
			DataTable dataTable5 = new DataTable();
			dataTable5.Columns.Add("Code", typeof(string));
			dataTable5.Columns.Add("Name", typeof(string));
			foreach (Enums.DisplayFormatType value6 in Enum.GetValues(typeof(Enums.DisplayFormatType)))
			{
				DataRow dataRow4 = dataTable5.NewRow();
				dataRow4["Code"] = (int)value6;
				string value = Function.ToString(value6);
				dataRow4["Name"] = Function.SpaceBeforeCapital(value);
				dataTable5.Rows.Add(dataRow4);
			}
			rep_DataType.SetDataSource(dataTable5, "Name", "Code", emptyRow: true);
			rep_QueryDataType.SetDataSource(dataTable5, "Name", "Code", emptyRow: true);
			rep_ReportDataType.SetDataSource(dataTable5, "Name", "Code", emptyRow: true);
			rep_PrintLayoutDataType.SetDataSource(dataTable5, "Name", "Code", emptyRow: true);
			Dictionary<string, string> dic = new Dictionary<string, string>
			{
				{ "", "" },
				{ "S", "SUM" },
				{ "A", "AVG" },
				{ "C", "COUNT" }
			};
			rep_SumTypeHeader.SetDataSource(dic);
			rep_SumTypeLine.SetDataSource(dic);
			rep_SumTypeBatch.SetDataSource(dic);
			rep_ProductionHeaderSumType.SetDataSource(dic);
			rep_ProductionLineSumType.SetDataSource(dic);
			rep_ProductionBatchSumType.SetDataSource(dic);
			UiSourceField = new DataTable();
			UiSourceField.Columns.Add("Code", typeof(string));
			UiSourceField.Columns.Add("Name", typeof(string));
			if (UiDataSource != null)
			{
				foreach (DataColumn item2 in from DataColumn t in UiDataSource.Columns
					where !string.IsNullOrEmpty(t.ColumnName)
					select t)
				{
					DataRow dataRow5 = UiSourceField.NewRow();
					dataRow5["Code"] = item2.ColumnName;
					if (GridSource != null)
					{
						DataRow dataRow6 = dataRow5;
						string value2;
						if (GridSource.Columns[item2.ColumnName] == null)
						{
							value2 = item2.Caption;
						}
						else
						{
							value2 = (string.IsNullOrEmpty(GridSource.Columns[item2.ColumnName].Caption) ? item2.ColumnName : GridSource.Columns[item2.ColumnName].Caption);
						}
						dataRow6["Name"] = value2;
					}
					else if (TreeSource != null)
					{
						DataRow dataRow7 = dataRow5;
						string value3;
						if (TreeSource.Columns[item2.ColumnName] == null)
						{
							value3 = item2.Caption;
						}
						else
						{
							value3 = (string.IsNullOrEmpty(TreeSource.Columns[item2.ColumnName].Caption) ? item2.ColumnName : TreeSource.Columns[item2.ColumnName].Caption);
						}
						dataRow7["Name"] = value3;
					}
					else
					{
						dataRow5["Name"] = item2.Caption;
					}
					UiSourceField.Rows.Add(dataRow5);
				}
			}
			rep_SourceFieldData.SetDataSource(UiSourceField, "Name", "Code", emptyRow: true);
			rep_AttParamSourceField.SetDataSource(UiSourceField, "Name", "Code", emptyRow: true);
			look_DocEntry.SetDataSource(UiSourceField, "Name", "Code", emptyRow: true);
			look_ProductionEntry.SetDataSource(UiSourceField, "Name", "Code", emptyRow: true);
			LoadDocumentField();
			_dtParams = await _httpClient.ContextAction.GetListParams(ActionFormId, ActionItemId);
			_dtParams.TableName = "APZ_CTA1";
			_dtMapping = await _httpClient.ContextAction.GetListMapping(ActionFormId, ActionItemId);
			_dtMapping.TableName = "APZ_CTA2";
			_dtAttachParams = await _httpClient.ContextAction.GetListAttachParams(ActionFormId, ActionItemId);
			_dtAttachParams.TableName = "APZ_CTA4";
			_dtAttachment = await _httpClient.ContextAction.GetListAttachment(ActionFormId, ActionItemId);
			_dtAttachment.TableName = "APZ_CTA3";
			_dtAttachment.Columns["LineId"].AutoIncrement = true;
			_dtAttachment.Columns["LineId"].AutoIncrementSeed = (_dtAttachment.IsNotNull() ? (_dtAttachment.AsEnumerable().Max((DataRow t) => Function.ParseLong(t["LineId"])) + 1) : 1);
			_dtAttachment.Columns["LineId"].AutoIncrementStep = 1L;
			_dtAttachment.Rows.Add(_dtAttachment.NewRow());
			_dtAttachment.AcceptChanges();
			_dtHttpRequestConfig = await _httpClient.ContextAction.GetListHttpRequestConfig(ActionFormId, ActionItemId);
			_dtHttpRequestConfig.TableName = "APZ_CTA5";
			grd_AttachFiles.DataSource = _dtAttachment.Copy();
			DataTable dt = await _httpClient.ContextAction.GetListContextAction(ActionFormId, ActionItemId);
			dt.TableName = "APZ_OCTA";
			dt.Columns["AbsEntry"].AutoIncrement = true;
			dt.Columns["AbsEntry"].AutoIncrementSeed = (dt.IsNotNull() ? (dt.AsEnumerable().Max((DataRow t) => Function.ParseLong(t["AbsEntry"])) + 1) : 1);
			dt.Columns["AbsEntry"].AutoIncrementStep = 1L;
			tv_Doc.DataSource = dt;
			tv_Doc.RefreshDataSource();
			tv_Doc.ExpandAll();
			_dtDashboardStruct = await _httpClient.DashboardDetail.GetById(-1);
			if (dt.Rows.Count == 0)
			{
				FormMode = Enums.FormViewModeType.Add;
			}
			else
			{
				FormMode = Enums.FormViewModeType.View;
			}
			InitChangeDataEvent();
			NewDoc();
			if (FormMode == Enums.FormViewModeType.Add && tv_Doc.Nodes.Count > 0)
			{
				tv_Doc.DataSourceTable.Rows[0]["Caption"] = _description;
				tv_Doc.DataSourceTable.AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			tv_Doc.HideLoadingPanel();
			WinCommons._CloseProcessing();
		}
	}

	private async Task LoadPhysicalTableField(string physicalTableName)
	{
		try
		{
			_dtPhysicalTable = new DataTable();
			_dtPhysicalTable.Columns.Add("Code", typeof(string));
			_dtPhysicalTable.Columns.Add("Name", typeof(string));
			DataTable dataTable = await _httpClient.ContextAction.GetColumnsFromPhysicalTableName(physicalTableName);
			if (dataTable.IsNotNull())
			{
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					DataRow dataRow = _dtPhysicalTable.NewRow();
					dataRow["Code"] = Function.ToString(dataTable.Rows[i]["COLUMN_NAME"]);
					dataRow["Name"] = Function.ToString(dataTable.Rows[i]["COLUMN_NAME"]);
					_dtPhysicalTable.Rows.Add(dataRow);
				}
				_dtPhysicalTable.AcceptChanges();
				rep_PhysicalTableTargetField.SetDataSource(_dtPhysicalTable, "Name", "Code");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void RptString_ItemSelected(DataRow row, GridView gv)
	{
		_ = 1;
		try
		{
			if (row != null)
			{
				switch ((Enums.ContextActionReportFrom)(Function.ParseChar(look_ReportFrom.EditValue) ?? 'Q'))
				{
				case Enums.ContextActionReportFrom.DefinedQuery:
				case Enums.ContextActionReportFrom.QueryText:
				{
					string qtxt = Function.ToString(row["Qstring"]);
					await LoadRptParameters(qtxt);
					break;
				}
				case Enums.ContextActionReportFrom.UserDefinedReport:
				{
					DataSetResult dataSetResult = await _httpClient.Udr.GetParamByStore(Function.ToString(row["StoreName"]), Function.ToString(row["Code"]), GlobalData.UserName, GlobalData.DefaultBranch);
					if (dataSetResult == null || !dataSetResult.Data.IsNotNull() || dataSetResult.Data.Tables.Count < 2)
					{
						break;
					}
					DataTable dataTable = dataSetResult.Data.Tables[1];
					DataTable dataTable2 = grd_Report.DataSourceTable();
					dataTable2.Rows.Clear();
					dataTable2.AcceptChanges();
					foreach (DataRow row2 in dataTable.Rows)
					{
						DataRow dataRow2 = dataTable2.NewRow();
						dataRow2["AbsEntry"] = Function.ParseInt(tv_Doc.FocusedNode.GetValue("AbsEntry"));
						dataRow2["FormId"] = ActionFormId;
						dataRow2["ItemId"] = ActionItemId;
						dataRow2["ParamType"] = "R";
						dataRow2["ParamName"] = Function.ToString(row2["ParamName"]);
						dataRow2["Label"] = Function.ToString(row2["Label"]);
						if (Function.ToString(row2["DataType"]).ToLower() == "datetime")
						{
							dataRow2["DataType"] = 7;
						}
						else
						{
							dataRow2["DataType"] = 0;
						}
						dataRow2["Active"] = "Y";
						dataRow2["SubType"] = "";
						dataTable2.Rows.Add(dataRow2);
					}
					dataTable2.AcceptChanges();
					grd_Report.DataSource = dataTable2;
					grd_Report.RefreshDataSource();
					break;
				}
				}
			}
			Control_EditValueChanged(null, null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void DataFrom_ItemSelected(DataRow row, GridView gv)
	{
		try
		{
			string qtxt = Function.ToString(row["QString"]);
			await LoadParameters(qtxt);
			Control_EditValueChanged(null, null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void QueryString_ItemSelected(DataRow row, GridView gv)
	{
		try
		{
			string qtxt = Function.ToString(row["QString"]);
			await LoadQueryParameters(qtxt);
			Control_EditValueChanged(null, null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadQueryParameters(string qtxt)
	{
		if (string.IsNullOrEmpty(qtxt))
		{
			return;
		}
		DataTable dataTable = await _httpClient.Cfl.GetQueryParams(qtxt);
		List<QueryParamater> list = new List<QueryParamater>();
		DataTable dataTable2 = grd_DoQuery.DataSourceTable();
		if (!dataTable2.IsNotNull())
		{
			dataTable2 = _dtParams.Clone();
		}
		dataTable2.Rows.Clear();
		dataTable2.AcceptChanges();
		foreach (DataRow row in dataTable.Rows)
		{
			QueryParamater item = new QueryParamater
			{
				DataType = Function.ToString(row["DataType"]),
				ParaName = Function.ToString(row["Para"]),
				Value = WinFuntions.GetDefaultSqlValue(Function.ToString(row["DataType"]))
			};
			list.Add(item);
			DataRow dataRow2 = dataTable2.NewRow();
			dataRow2["AbsEntry"] = Function.ParseInt(tv_Doc.FocusedNode.GetValue("AbsEntry"));
			dataRow2["FormId"] = ActionFormId;
			dataRow2["ItemId"] = ActionItemId;
			dataRow2["ParamType"] = "Q";
			dataRow2["ParamName"] = Function.ToString(row["Para"]);
			dataRow2["Label"] = Function.ToString(row["ColumnName"]);
			if (Function.ToString(row["DataType"]).ToLower() == "datetime")
			{
				dataRow2["DataType"] = 7;
			}
			else
			{
				dataRow2["DataType"] = 0;
			}
			dataRow2["Active"] = "Y";
			dataRow2["SubType"] = "";
			dataTable2.Rows.Add(dataRow2);
		}
		dataTable2.AcceptChanges();
		grd_DoQuery.DataSource = dataTable2;
		grd_DoQuery.RefreshDataSource();
	}

	private async Task LoadRptParameters(string qtxt)
	{
		if (string.IsNullOrEmpty(qtxt))
		{
			return;
		}
		DataTable dataTable = await _httpClient.Cfl.GetQueryParams(qtxt);
		DataTable dataTable2 = grd_Report.DataSourceTable();
		if (!dataTable2.IsNotNull())
		{
			dataTable2 = _dtParams.Clone();
		}
		dataTable2.Rows.Clear();
		dataTable2.AcceptChanges();
		foreach (DataRow row in dataTable.Rows)
		{
			DataRow dataRow2 = dataTable2.NewRow();
			dataRow2["AbsEntry"] = Function.ParseInt(tv_Doc.FocusedNode.GetValue("AbsEntry"));
			dataRow2["FormId"] = ActionFormId;
			dataRow2["ItemId"] = ActionItemId;
			dataRow2["ParamType"] = "R";
			dataRow2["ParamName"] = Function.ToString(row["Para"]);
			dataRow2["Label"] = Function.ToString(row["ColumnName"]);
			if (Function.ToString(row["DataType"]).ToLower() == "datetime")
			{
				dataRow2["DataType"] = 7;
			}
			else
			{
				dataRow2["DataType"] = 0;
			}
			dataRow2["Active"] = "Y";
			dataRow2["SubType"] = "";
			dataTable2.Rows.Add(dataRow2);
		}
		dataTable2.AcceptChanges();
		grd_Report.DataSource = dataTable2;
		grd_Report.RefreshDataSource();
	}

	private async Task LoadSourceColumn(int absEntry, string qtxt)
	{
		if (string.IsNullOrEmpty(qtxt))
		{
			return;
		}
		string dataParamType = Function.ToString(tv_Doc.FocusedNode.GetValue("DataParamType"));
		DataTable dtTemporary = new DataTable();
		List<QueryParamater> lstPr = new List<QueryParamater>();
		DataTable dtPr = grd_Parameters.DataSourceTable;
		if (dtPr.IsNotNull())
		{
			EnumerableRowCollection<DataRow> enumerableRowCollection = from t in dtPr.AsEnumerable()
				where Function.ToString(t["ParamType"]) == "S"
				select t;
			if (dataParamType == "T")
			{
				string text = Function.ToString(tv_Doc.FocusedNode.GetValue("TempTblName"));
				if (string.IsNullOrEmpty(text))
				{
					text = "tbltemp";
				}
				foreach (DataRow item in enumerableRowCollection)
				{
					DataColumn dataColumn = new DataColumn();
					string text2 = (dataColumn.ColumnName = Function.ToString(item["SourceField"]));
					if (!string.IsNullOrEmpty(text2))
					{
						string text4 = Function.ToString(item["DefaultValue"]);
						if (!string.IsNullOrEmpty(text4))
						{
							dataColumn.DefaultValue = text4;
						}
						if (UiDataSource != null && UiDataSource.ExistsColumn(text2))
						{
							dataColumn.DataType = UiDataSource.Columns[text2].DataType;
						}
						else
						{
							dataColumn.DataType = typeof(string);
						}
						if (!dtTemporary.ExistsColumn(text2))
						{
							dtTemporary.Columns.Add(dataColumn);
						}
					}
				}
				if (dtTemporary.Columns.Count == 0 && UiDataSource != null)
				{
					dtTemporary = UiDataSource.Clone();
				}
				dtTemporary.TableName = text;
			}
		}
		if (dataParamType != "T")
		{
			foreach (DataRow row in (await _httpClient.Cfl.GetQueryParams(qtxt)).Rows)
			{
				QueryParamater par = new QueryParamater
				{
					DataType = Function.ToString(row["DataType"]),
					ParaName = Function.ToString(row["Para"])
				};
				if (dtPr.IsNotNull())
				{
					DataRow dataRow2 = dtPr.AsEnumerable().FirstOrDefault((DataRow r) => Function.ToString(r["ParamName"]) == par.ParaName);
					if (dataRow2 != null)
					{
						par.Value = WinFuntions.GetDefaultSqlValue(Function.ToString(dataRow2["DataType"]));
					}
				}
				else
				{
					par.Value = WinFuntions.GetDefaultSqlValue(Function.ToString(row["DataType"]));
				}
				lstPr.Add(par);
			}
		}
		qtxt = WinFuntions.QueryReplaceUserAndBranch(qtxt);
		DataSet dataSet = ((!(dataParamType == "T")) ? _httpClient.Commons.ExecuteSql(qtxt, lstPr) : _httpClient.Commons.ExecuteTemporyTable(qtxt, dtTemporary, dtTemporary.TableName));
		if (dataSet == null || dataSet.Tables.Count <= 1)
		{
			return;
		}
		DataTable dataTable = dataSet.Tables["Message"];
		if (dataTable.IsNotNull())
		{
			string text5 = Function.ToString(dataTable.Rows[0]["MsgType"]);
			string msg = Function.ToString(dataTable.Rows[0]["Message"]);
			if (text5 == "5xx")
			{
				WinCommons.Error(Enums.MessageCode.None, msg);
			}
		}
		InsertDataFilter(Function.ParseInt(tv_Doc.FocusedNode.GetValue("AbsEntry")), dataSet.Tables[1]);
	}

	private async Task LoadParameters(string qtxt)
	{
		if (string.IsNullOrEmpty(qtxt))
		{
			return;
		}
		List<QueryParamater> lstPr = new List<QueryParamater>();
		string dataParamType = Function.ToString(tv_Doc.FocusedNode.GetValue("DataParamType"));
		if (dataParamType.Equals("V"))
		{
			DataTable dataTable = await _httpClient.Cfl.GetQueryParams(qtxt);
			DataTable dataTable2 = grd_Parameters.DataSourceTable();
			if (!dataTable2.IsNotNull())
			{
				dataTable2 = _dtParams.Clone();
			}
			dataTable2.Rows.Clear();
			dataTable2.AcceptChanges();
			foreach (DataRow row in dataTable.Rows)
			{
				QueryParamater par = new QueryParamater
				{
					DataType = Function.ToString(row["DataType"]),
					ParaName = Function.ToString(row["Para"])
				};
				DataRow dataRow2 = dataTable2.AsEnumerable().FirstOrDefault((DataRow r) => Function.ToString(r["ParamName"]) == par.ParaName);
				if (dataRow2 != null)
				{
					par.Value = WinFuntions.GetDefaultSqlValue(Function.ToString(dataRow2["DataType"]));
				}
				else
				{
					par.Value = WinFuntions.GetDefaultSqlValue(Function.ToString(row["DataType"]));
				}
				lstPr.Add(par);
				DataRow dataRow3 = dataTable2.NewRow();
				dataRow3["AbsEntry"] = Function.ParseInt(tv_Doc.FocusedNode.GetValue("AbsEntry"));
				dataRow3["FormId"] = ActionFormId;
				dataRow3["ItemId"] = ActionItemId;
				dataRow3["ParamType"] = "S";
				dataRow3["ParamName"] = Function.ToString(row["Para"]);
				dataRow3["Label"] = Function.ToString(row["ColumnName"]);
				if (Function.ToString(row["DataType"]).ToLower() == "datetime")
				{
					dataRow3["DataType"] = 7;
				}
				else
				{
					dataRow3["DataType"] = 0;
				}
				dataRow3["Active"] = "Y";
				dataRow3["SubType"] = "";
				dataTable2.Rows.Add(dataRow3);
			}
			dataTable2.AcceptChanges();
			grd_Parameters.DataSource = dataTable2;
			grd_Parameters.RefreshDataSource();
		}
		DataTable dataTable3 = new DataTable();
		DataTable dataSourceTable = grd_Parameters.DataSourceTable;
		if (dataSourceTable.IsNotNull())
		{
			EnumerableRowCollection<DataRow> enumerableRowCollection = from t in dataSourceTable.AsEnumerable()
				where Function.ToString(t["ParamType"]) == "S"
				select t;
			if (dataParamType == "T")
			{
				foreach (DataRow item in enumerableRowCollection)
				{
					DataColumn dataColumn = new DataColumn();
					string text = (dataColumn.ColumnName = Function.ToString(item["SourceField"]));
					if (!string.IsNullOrEmpty(text))
					{
						string text3 = Function.ToString(item["DefaultValue"]);
						if (!string.IsNullOrEmpty(text3))
						{
							dataColumn.DefaultValue = text3;
						}
						if (UiDataSource != null && UiDataSource.ExistsColumn(text))
						{
							dataColumn.DataType = UiDataSource.Columns[text].DataType;
						}
						else
						{
							dataColumn.DataType = typeof(string);
						}
						if (!dataTable3.ExistsColumn(text))
						{
							dataTable3.Columns.Add(dataColumn);
						}
					}
				}
			}
		}
		if (dataTable3.Columns.Count == 0 && UiDataSource != null)
		{
			dataTable3 = UiDataSource.Clone();
		}
		qtxt = WinFuntions.QueryReplaceUserAndBranch(qtxt);
		DataSet dataSet;
		if (dataParamType == "T")
		{
			string text4 = Function.ToString(tv_Doc.FocusedNode.GetValue("TempTblName"));
			if (string.IsNullOrEmpty(text4))
			{
				text4 = "tbltemp";
			}
			dataTable3.TableName = text4;
			dataSet = _httpClient.Commons.ExecuteTemporyTable(qtxt, dataTable3, dataTable3.TableName);
		}
		else
		{
			dataSet = _httpClient.Commons.ExecuteSql(qtxt, lstPr);
		}
		if (dataSet == null || dataSet.Tables.Count <= 1)
		{
			return;
		}
		DataTable dataTable4 = dataSet.Tables["Message"];
		if (dataTable4.IsNotNull())
		{
			string text5 = Function.ToString(dataTable4.Rows[0]["MsgType"]);
			string msg = Function.ToString(dataTable4.Rows[0]["Message"]);
			if (text5 == "5xx")
			{
				WinCommons.Error(Enums.MessageCode.None, msg);
			}
		}
		InsertDataFilter(Function.ParseInt(tv_Doc.FocusedNode.GetValue("AbsEntry")), dataSet.Tables[1]);
	}

	private void InsertDataFilter(int absEntry, DataTable dt)
	{
		if (_dicFilter == null)
		{
			_dicFilter = new Dictionary<int, DataTable>();
		}
		if (DicSourceColumn == null)
		{
			DicSourceColumn = new Dictionary<int, bool>();
		}
		_dicFilter[absEntry] = dt;
		DicSourceColumn[absEntry] = true;
		LoadSourceField(dt);
	}

	private void InsertDataParams(string type, DataTable dt)
	{
		if (_dtParams == null || _dtParams.Columns.Count == 0)
		{
			WinCommons.Error(Enums.MessageCode.CouldNotGetDataParamater, "");
		}
		else
		{
			if (dt == null || dt.Rows.Count == 0)
			{
				return;
			}
			DataRow[] array = _dtParams.Select("AbsEntry=" + Function.ParseInt(dt.Rows[0]["AbsEntry"]) + " AND ParamType='" + type + "'");
			for (int i = 0; i < array.Length; i++)
			{
				_dtParams.Rows.Remove(array[i]);
			}
			for (int j = 0; j < dt.Rows.Count; j++)
			{
				if (!string.IsNullOrEmpty(Function.ToString(dt.Rows[j]["ParamName"])) || !string.IsNullOrEmpty(Function.ToString(dt.Rows[j]["SourceField"])) || !string.IsNullOrEmpty(Function.ToString(dt.Rows[j]["DefaultValue"])))
				{
					DataRow dataRow = _dtParams.NewRow();
					dataRow.ItemArray = dt.Rows[j].ItemArray;
					_dtParams.Rows.Add(dataRow);
				}
			}
			_dtParams.AcceptChanges();
		}
	}

	private void InsertDataMapping(string type, string item, DataTable dt)
	{
		if (_dtMapping == null || _dtMapping.Columns.Count == 0)
		{
			WinCommons.Error(Enums.MessageCode.CouldNotGetDataMapping, "");
		}
		else
		{
			if (dt == null || dt.Rows.Count == 0)
			{
				return;
			}
			DataRow[] array = _dtMapping.Select("AbsEntry=" + Function.ParseInt(dt.Rows[0]["AbsEntry"]) + " AND MappingType='" + type + "' AND TargetItem='" + item + "'");
			for (int i = 0; i < array.Length; i++)
			{
				_dtMapping.Rows.Remove(array[i]);
			}
			for (int j = 0; j < dt.Rows.Count; j++)
			{
				if (!string.IsNullOrEmpty(Function.ToString(dt.Rows[j]["SourceField"])) || !string.IsNullOrEmpty(Function.ToString(dt.Rows[j]["DfltValue"])))
				{
					DataRow dataRow = _dtMapping.NewRow();
					dataRow.ItemArray = dt.Rows[j].ItemArray;
					_dtMapping.Rows.Add(dataRow);
				}
			}
			_dtMapping.AcceptChanges();
		}
	}

	private async Task SetFocusLine(int actionType)
	{
		_ = 3;
		try
		{
			grd_Parameters.DataSourceTable.Rows.Clear();
			grd_Parameters.DataSourceTable.AcceptChanges();
			if (_dtParams == null || _dtParams.Columns.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.CouldNotGetDataParamater, "");
				return;
			}
			DataTable dataSource = _dtParams.Clone();
			if (string.IsNullOrEmpty(Function.ToString(tv_Doc.FocusedNode.GetValue("Caption"))))
			{
				grd_Parameters.DataSource = dataSource;
				grd_Parameters.RefreshDataSource();
				return;
			}
			int absEntry = Function.ParseInt(tv_Doc.FocusedNode.GetValue("AbsEntry"));
			DataRow[] array = _dtParams.Select(string.Format("AbsEntry={0} AND ParamType='{1}'", absEntry, "S"));
			if (array.Length != 0)
			{
				grd_Parameters.DataSource = array.CopyToDataTable();
			}
			else
			{
				grd_Parameters.DataSource = _dtParams.Clone();
			}
			grd_Parameters.RefreshDataSource();
			grd_DoQuery.DataSourceTable.Rows.Clear();
			grd_DoQuery.DataSourceTable.AcceptChanges();
			if (_dtParams == null || _dtParams.Columns.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.CouldNotGetDataParamater, "");
				return;
			}
			DataTable dataSource2 = _dtParams.Clone();
			if (string.IsNullOrEmpty(Function.ToString(tv_Doc.FocusedNode.GetValue("Caption"))))
			{
				grd_DoQuery.DataSource = dataSource2;
				grd_DoQuery.RefreshDataSource();
				return;
			}
			DataRow[] array2 = _dtParams.Select(string.Format("AbsEntry={0} AND ParamType='{1}'", absEntry, "Q"));
			if (array2.Length != 0)
			{
				grd_DoQuery.DataSource = array2.CopyToDataTable();
			}
			else
			{
				grd_DoQuery.DataSource = _dtParams.Clone();
			}
			grd_DoQuery.RefreshDataSource();
			DataRow[] array3 = _dtParams.Select(string.Format("AbsEntry={0} AND ParamType='{1}'", absEntry, "R"));
			if (array3.Length != 0)
			{
				grd_Report.DataSource = array3.CopyToDataTable();
			}
			else
			{
				grd_Report.DataSource = _dtParams.Clone();
			}
			grd_Report.RefreshDataSource();
			DataRow[] array4 = _dtAttachment.Select($"AbsEntry={absEntry}");
			if (array4.Length != 0)
			{
				grd_AttachFiles.DataSource = array4.CopyToDataTable();
				AddBlankAtt();
			}
			else
			{
				DataTable dataTable = _dtAttachment.Clone();
				dataTable.Rows.Add(dataTable.NewRow());
				dataTable.AcceptChanges();
				grd_AttachFiles.DataSource = dataTable;
			}
			grd_Report.RefreshDataSource();
			await ChangeSourceField(absEntry);
			switch (actionType)
			{
			case 17:
				MappingTarget(absEntry, "D", "H", _dtHeaderDocument, grd_Header);
				MappingTarget(absEntry, "D", "I", _dtItemDocument, grd_Items);
				MappingTarget(absEntry, "D", "B", _dtBatchDocument, grd_Batch);
				break;
			case 206000:
				await LoadTabUdo(Function.ToString(look_UdoCode.EditValue), Function.ToString(look_UdoCode.Text));
				break;
			case 7000030:
			{
				int dashboardId = Function.ParseInt(tv_Doc.FocusedNode.GetValue("DashboardId"));
				if (dashboardId > 0)
				{
					if (_dicDashboard.ContainsKey(absEntry))
					{
						ucDashboardViewer1.SetDashboard(_dicDashboard[absEntry], dashboardId);
					}
					else if (_dicFilter.ContainsKey(absEntry))
					{
						DevExpress.DashboardCommon.Dashboard dashboard = await this.LoadDashboardFromSource(dashboardId, _dicFilter[absEntry]);
						ucDashboardViewer1.SetDashboard(dashboard, dashboardId);
						_dicDashboard[absEntry] = dashboard;
					}
					btn_EditDesign.Text = "Edit Design";
				}
				else
				{
					btn_EditDesign.Text = "Create New Dashboard";
					ucDashboardViewer1.Clear();
				}
				break;
			}
			case 202:
				MappingTarget(absEntry, "W", "H", _dtHeaderProduction, grd_ProductionHeader);
				MappingTarget(absEntry, "W", "I", _dtItemProduction, grd_ProductionItem);
				MappingTarget(absEntry, "W", "B", _dtBatchDocument, grd_ProductionBatch);
				break;
			case 8000005:
				MappingTarget(absEntry, "F", "H", _dtHeaderForecast, grd_ForecastHeader);
				MappingTarget(absEntry, "F", "I", _dtItemForecast, grd_ForecastItems);
				break;
			case 121:
				MappingTarget(absEntry, "V", "H", _dtApprovalDecision, grd_Approval);
				break;
			case 7000039:
			{
				DataRow[] array5 = _dtAttachParams.Select($"AbsEntry={absEntry} ");
				_dtAttachParams.Clone();
				DataTable dataTable2 = ((array5.Length == 0) ? _dtAttachParams.Clone() : array5.CopyToDataTable());
				grd_PrintLayout.DataSource = dataTable2.Copy();
				grd_PrintLayout.RefreshDataSource();
				break;
			}
			case 9:
				ucHttpRequest.ActionFormId = ActionFormId;
				ucHttpRequest.ActionItemId = ActionItemId;
				ucHttpRequest.ActionEntry = absEntry;
				ucHttpRequest.MappingTarget(absEntry, _dtMapping);
				ucHttpRequest.MappingTargetBody(absEntry, _dtHttpRequestConfig);
				ucHttpRequest.SetFocusedValue(tv_Doc.GetFocusedDataRow());
				break;
			case 27110:
			{
				string text = Function.ToString(tv_Doc.FocusedNode.GetValue("PhysicalTableName"));
				if (!string.IsNullOrEmpty(text))
				{
					await LoadPhysicalTableField(text);
					MappingTarget(absEntry, "P", "H", _dtPhysicalTable, grd_PhysicalTable);
				}
				else
				{
					grd_PhysicalTable.DataSourceTable.Rows.Clear();
					grd_PhysicalTable.RefreshDataSource();
				}
				break;
			}
			}
			if (_dicTab.ContainsKey(actionType.ToString()))
			{
				_dicTab[actionType.ToString()]?.MappingTarget(absEntry, _dtMapping);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void AddBlankAtt()
	{
		try
		{
			DataTable dataSourceTable = grd_AttachFiles.DataSourceTable;
			if (dataSourceTable.AsEnumerable().FirstOrDefault((DataRow t) => string.IsNullOrEmpty(Function.ToString(t["FilePath"]))) == null)
			{
				DataRow row = dataSourceTable.NewRow();
				dataSourceTable.Rows.Add(row);
				dataSourceTable.AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void MappingTarget(int absEntry, string maptype, string targetitem, DataTable source, GridControlEx grd)
	{
		DataRow[] array = _dtMapping.Select($"AbsEntry={absEntry} AND MappingType='{maptype}' AND TargetItem='{targetitem}'");
		DataTable ret = _dtMapping.Clone();
		DataTable dataTable = _dtMapping.Clone();
		if (array.Length != 0)
		{
			dataTable = array.CopyToDataTable();
		}
		DataRow[] array2 = (from t0 in source.AsEnumerable()
			join t1 in dataTable.AsEnumerable() on Function.ToString(t0["Code"]) equals Function.ToString(t1["TargetField"]) into grp
			from item in grp.DefaultIfEmpty()
			select ret.LoadDataRow(new object[9]
			{
				absEntry,
				ActionFormId,
				ActionItemId,
				maptype,
				targetitem,
				Function.ToString(t0["Code"]),
				(item != null) ? Function.ToString(item["SourceField"]) : "",
				(item != null) ? Function.ToString(item["DfltValue"]) : null,
				(item != null) ? Function.ToString(item["SumType"]) : ""
			}, fAcceptChanges: false)).ToArray();
		if (array2.Length != 0)
		{
			grd.DataSource = array2.CopyToDataTable();
		}
		else
		{
			grd.DataSource = dataTable;
		}
		grd.RefreshDataSource();
	}

	private async Task ChangeSourceField(int absEntry)
	{
		string text = Function.ToString(tv_Doc.FocusedNode.GetValue("SourceType"));
		if (text == "U")
		{
			rep_SourceFieldHeader.SetDataSource(UiSourceField, "Name", "Code", emptyRow: true);
			rep_SourceFieldItem.SetDataSource(UiSourceField, "Name", "Code", emptyRow: true);
			rep_SourceField.SetDataSource(UiSourceField, "Name", "Code", emptyRow: true);
			rep_SourceFieldBatch.SetDataSource(UiSourceField, "Name", "Code", emptyRow: true);
			rep_QuerySourceField.SetDataSource(UiSourceField, "Name", "Code", emptyRow: true);
			rep_ProductionHeaderSourceField.SetDataSource(UiSourceField, "Name", "Code", emptyRow: true);
			rep_ProductionLineSourceField.SetDataSource(UiSourceField, "Name", "Code", emptyRow: true);
			rep_ProductionBatchSourceField.SetDataSource(UiSourceField, "Name", "Code", emptyRow: true);
			rep_AttParamSourceField.SetDataSource(UiSourceField, "Name", "Code", emptyRow: true);
			rep_PrintLayoutSourceField.SetDataSource(UiSourceField, "Name", "Code", emptyRow: true);
			rep_ForecastSourceField.SetDataSource(UiSourceField, "Name", "Code", emptyRow: true);
			rep_ForecastItemSourceField.SetDataSource(UiSourceField, "Name", "Code", emptyRow: true);
			rep_ApprovalSourceField.SetDataSource(UiSourceField, "Name", "Code", emptyRow: true);
			rep_PhysicalTableSourceField.SetDataSource(UiSourceField, "Name", "Code", emptyRow: true);
			look_DocEntry.SetDataSource(UiSourceField, "Name", "Code", emptyRow: true);
			look_ProductionEntry.SetDataSource(UiSourceField, "Name", "Code", emptyRow: true);
			ucHttpRequest.SourceFieldsChanged = UiSourceField;
			_dicFilter[absEntry] = UiDataSource;
			{
				foreach (KeyValuePair<string, UcContextActionMapping> item in _dicTab)
				{
					item.Value?.ChangeSourceField(UiSourceField);
				}
				return;
			}
		}
		if (!(text == "D"))
		{
			return;
		}
		if (DicSourceColumn == null || (DicSourceColumn != null && (!DicSourceColumn.ContainsKey(absEntry) || (DicSourceColumn.ContainsKey(absEntry) && !DicSourceColumn[absEntry]))))
		{
			Enums.ContextActionDataFrom contextActionDataFrom = (Enums.ContextActionDataFrom)(Function.ParseChar(tv_Doc.FocusedNode.GetValue("DataFrom")) ?? 'Q');
			string text2 = Function.ToString(tv_Doc.FocusedNode.GetValue("DataString"));
			string qtxt = "";
			switch (contextActionDataFrom)
			{
			case Enums.ContextActionDataFrom.DefinedQuery:
			{
				int queryId = Function.ParseInt(text2);
				qtxt = await _httpClient.Queries.GetQueryText(queryId);
				break;
			}
			case Enums.ContextActionDataFrom.QueryText:
				qtxt = text2;
				break;
			}
			await LoadSourceColumn(absEntry, qtxt);
		}
		else if (_dicFilter != null && _dicFilter.ContainsKey(absEntry))
		{
			LoadSourceField(_dicFilter[absEntry]);
		}
	}

	private void LoadSourceField(DataTable dt)
	{
		DataTable dataTable = new DataTable();
		dataTable.Columns.Add("Code", typeof(string));
		dataTable.Columns.Add("Name", typeof(string));
		if (dt == null || dt.Columns.Count <= 0)
		{
			return;
		}
		foreach (DataColumn item in from DataColumn t in dt.Columns
			where !string.IsNullOrEmpty(t.ColumnName)
			select t)
		{
			DataRow dataRow = dataTable.NewRow();
			dataRow["Code"] = item.ColumnName;
			dataRow["Name"] = ((!string.IsNullOrEmpty(item.Caption)) ? item.Caption : item.ColumnName);
			dataTable.Rows.Add(dataRow);
		}
		DatabaseSourceField = dataTable.Copy();
		SetRepositorySourceField(dataTable);
	}

	private void SetRepositorySourceField(DataTable dtsource)
	{
		rep_SourceFieldHeader.SetDataSource(dtsource, "Name", "Code", emptyRow: true);
		rep_SourceFieldItem.SetDataSource(dtsource, "Name", "Code", emptyRow: true);
		rep_SourceField.SetDataSource(dtsource, "Name", "Code", emptyRow: true);
		rep_SourceFieldBatch.SetDataSource(dtsource, "Name", "Code", emptyRow: true);
		rep_QuerySourceField.SetDataSource(dtsource, "Name", "Code", emptyRow: true);
		rep_ProductionHeaderSourceField.SetDataSource(dtsource, "Name", "Code", emptyRow: true);
		rep_ProductionLineSourceField.SetDataSource(dtsource, "Name", "Code", emptyRow: true);
		rep_ProductionBatchSourceField.SetDataSource(dtsource, "Name", "Code", emptyRow: true);
		rep_AttParamSourceField.SetDataSource(dtsource, "Name", "Code", emptyRow: true);
		rep_PrintLayoutSourceField.SetDataSource(dtsource, "Name", "Code", emptyRow: true);
		rep_ForecastSourceField.SetDataSource(dtsource, "Name", "Code", emptyRow: true);
		rep_ForecastItemSourceField.SetDataSource(dtsource, "Name", "Code", emptyRow: true);
		rep_ApprovalSourceField.SetDataSource(dtsource, "Name", "Code", emptyRow: true);
		rep_PhysicalTableSourceField.SetDataSource(dtsource, "Name", "Code", emptyRow: true);
		look_DocEntry.SetDataSource(dtsource, "Name", "Code", emptyRow: true);
		look_ProductionEntry.SetDataSource(dtsource, "Name", "Code", emptyRow: true);
		ucHttpRequest.SourceFieldsChanged = dtsource;
		foreach (KeyValuePair<string, UcContextActionMapping> item in _dicTab)
		{
			item.Value?.ChangeSourceField(dtsource);
		}
		if (Function.ParseInt(tv_Doc.FocusedNode.GetValue("ActionTypeEx")) != 206000)
		{
			return;
		}
		foreach (XtraTabPage tabPage in tabControl_Udo.TabPages)
		{
			foreach (object control in tabPage.Controls)
			{
				if (control is GridControlEx)
				{
					if ((control as GridControlEx).MainView is GridViewEx gridViewEx && gridViewEx.ColumnsEx["SourceField"] is GridColumnEx gridColumnEx)
					{
						(gridColumnEx.ColumnEdit as RepositoryItemLookUpEdit).SetDataSource(dtsource, "Name", "Code");
					}
					break;
				}
			}
		}
	}

	private void LoadDocumentField()
	{
		try
		{
			if (Type.GetType("Apzon.Commons.Properties.Documents, Apzon.Commons") != null)
			{
				ComponentResourceManager componentResourceManager = new ComponentResourceManager(Type.GetType("Apzon.Commons.Properties.Documents, Apzon.Commons"));
				_dtHeaderDocument = new DataTable();
				_dtHeaderDocument.Columns.Add("Code", typeof(string));
				_dtHeaderDocument.Columns.Add("Name", typeof(string));
				DataRow dataRow = _dtHeaderDocument.NewRow();
				dataRow["Code"] = "CardCode";
				dataRow["Name"] = componentResourceManager.GetString("CardCode", CultureInfo.CreateSpecificCulture(GlobalData.CURRENTLANGUAGE)) ?? "BP Code";
				_dtHeaderDocument.Rows.Add(dataRow);
				dataRow = _dtHeaderDocument.NewRow();
				dataRow["Code"] = "CardName";
				dataRow["Name"] = componentResourceManager.GetString("CardName", CultureInfo.CreateSpecificCulture(GlobalData.CURRENTLANGUAGE)) ?? "BP Name";
				_dtHeaderDocument.Rows.Add(dataRow);
				foreach (DataColumn item in from DataColumn t in DocumentsHelper.Document.Columns
					where !string.IsNullOrEmpty(t.ColumnName) && !new string[20]
					{
						"CardCode", "CardName", "DocEntry", "DocNum", "CANCELED", "Handwrtten", "Printed", "DocStatus", "InvntSttus", "ObjType",
						"TaxTotal", "Total", "TotalBeforeDis", "TotalExpns", "VatSum", "VatSumFC", "WddStatus", "DataSource", "Instance", "LogInstanc"
					}.Contains(t.ColumnName)
					select t)
				{
					DataRow dataRow2 = _dtHeaderDocument.NewRow();
					dataRow2["Code"] = item.ColumnName;
					dataRow2["Name"] = componentResourceManager.GetString(item.ColumnName, CultureInfo.CreateSpecificCulture(GlobalData.CURRENTLANGUAGE)) ?? item.Caption;
					_dtHeaderDocument.Rows.Add(dataRow2);
				}
				rep_TargetField.SetDataSource(_dtHeaderDocument, "Name", "Code");
			}
			if (Type.GetType("Apzon.Commons.Properties.DocumentsLines, Apzon.Commons") != null)
			{
				ComponentResourceManager componentResourceManager2 = new ComponentResourceManager(Type.GetType("Apzon.Commons.Properties.DocumentsLines, Apzon.Commons"));
				_dtItemDocument = new DataTable();
				_dtItemDocument.Columns.Add("Code", typeof(string));
				_dtItemDocument.Columns.Add("Name", typeof(string));
				DataRow dataRow3 = _dtItemDocument.NewRow();
				dataRow3["Code"] = "ItemCode";
				dataRow3["Name"] = componentResourceManager2.GetString("ItemCode", CultureInfo.CreateSpecificCulture(GlobalData.CURRENTLANGUAGE)) ?? "Item Code";
				_dtItemDocument.Rows.Add(dataRow3);
				dataRow3 = _dtItemDocument.NewRow();
				dataRow3["Code"] = "Dscription";
				dataRow3["Name"] = componentResourceManager2.GetString("Dscription", CultureInfo.CreateSpecificCulture(GlobalData.CURRENTLANGUAGE)) ?? "Item Name";
				_dtItemDocument.Rows.Add(dataRow3);
				foreach (DataColumn item2 in from DataColumn t in DocumentsHelper.Lines.Columns
					where !string.IsNullOrEmpty(t.ColumnName) && !new string[7] { "ItemCode", "Dscription", "DocEntry", "OpenQty", "OpenInvQty", "Instance", "LogInstanc" }.Contains(t.ColumnName)
					select t)
				{
					DataRow dataRow4 = _dtItemDocument.NewRow();
					dataRow4["Code"] = item2.ColumnName;
					dataRow4["Name"] = componentResourceManager2.GetString(item2.ColumnName, CultureInfo.CreateSpecificCulture(GlobalData.CURRENTLANGUAGE)) ?? item2.Caption;
					_dtItemDocument.Rows.Add(dataRow4);
				}
				rep_TargetFieldLine.SetDataSource(_dtItemDocument, "Name", "Code");
			}
			if (Type.GetType("Apzon.Commons.Properties.DocumentsBatches, Apzon.Commons") != null)
			{
				ComponentResourceManager componentResourceManager3 = new ComponentResourceManager(Type.GetType("Apzon.Commons.Properties.DocumentsBatches, Apzon.Commons"));
				_dtBatchDocument = new DataTable();
				_dtBatchDocument.Columns.Add("Code", typeof(string));
				_dtBatchDocument.Columns.Add("Name", typeof(string));
				foreach (DataColumn item3 in from DataColumn t in DocumentsHelper.BatchSerialNumbersStruct.Columns
					where !string.IsNullOrEmpty(t.ColumnName) && !new string[19]
					{
						"ItemCode", "SysNumber", "Quantity", "AbsEntry", "Balance", "CostTotal", "CreateDate", "DataSource", "Instance", "itemName",
						"LogInstanc", "QuantOut", "Transfered", "UserSign", "UserSign2", "IsExits", "BaseLineNumber", "IsBase", "ManBatchSerial"
					}.Contains(t.ColumnName)
					select t)
				{
					DataRow dataRow5 = _dtBatchDocument.NewRow();
					dataRow5["Code"] = item3.ColumnName;
					dataRow5["Name"] = componentResourceManager3.GetString(item3.ColumnName, CultureInfo.CreateSpecificCulture(GlobalData.CURRENTLANGUAGE)) ?? item3.Caption ?? item3.ColumnName;
					_dtBatchDocument.Rows.Add(dataRow5);
				}
				DataRow dataRow6 = _dtBatchDocument.NewRow();
				dataRow6["Code"] = "ToBinAbs";
				dataRow6["Name"] = "To BinAbs";
				_dtBatchDocument.Rows.Add(dataRow6);
				rep_TargetFieldBatch.SetDataSource(_dtBatchDocument, "Name", "Code");
				rep_ProdoctionTargetBatch.SetDataSource(_dtBatchDocument, "Name", "Code");
			}
			if (Type.GetType("Apzon.Commons.Properties.ProductionOrder, Apzon.Commons") != null)
			{
				ComponentResourceManager componentResourceManager4 = new ComponentResourceManager(Type.GetType("Apzon.Commons.Properties.ProductionOrder, Apzon.Commons"));
				_dtHeaderProduction = new DataTable();
				_dtHeaderProduction.Columns.Add("Code", typeof(string));
				_dtHeaderProduction.Columns.Add("Name", typeof(string));
				DataRow dataRow7 = _dtHeaderProduction.NewRow();
				dataRow7["Code"] = "ItemCode";
				dataRow7["Name"] = componentResourceManager4.GetString("ItemCode", CultureInfo.CreateSpecificCulture(GlobalData.CURRENTLANGUAGE)) ?? "Item Code";
				_dtHeaderProduction.Rows.Add(dataRow7);
				foreach (DataColumn item4 in from DataColumn t in ProductionOrderTableHelper.ProductionHeader.Columns
					where !string.IsNullOrEmpty(t.ColumnName) && !new string[10] { "ItemCode", "DocEntry", "DocNum", "CmpltQty", "RjctQty", "UserSign", "Printed", "UserSign2", "LogInstanc", "UpdateDate" }.Contains(t.ColumnName)
					select t)
				{
					DataRow dataRow8 = _dtHeaderProduction.NewRow();
					dataRow8["Code"] = item4.ColumnName;
					dataRow8["Name"] = componentResourceManager4.GetString(item4.ColumnName, CultureInfo.CreateSpecificCulture(GlobalData.CURRENTLANGUAGE)) ?? item4.Caption;
					_dtHeaderProduction.Rows.Add(dataRow8);
				}
				rep_ProductionTargetHeader.SetDataSource(_dtHeaderProduction, "Name", "Code");
			}
			if (Type.GetType("Apzon.Commons.Properties.ProductionOrderLines, Apzon.Commons") != null)
			{
				ComponentResourceManager componentResourceManager5 = new ComponentResourceManager(Type.GetType("Apzon.Commons.Properties.ProductionOrderLines, Apzon.Commons"));
				_dtItemProduction = new DataTable();
				_dtItemProduction.Columns.Add("Code", typeof(string));
				_dtItemProduction.Columns.Add("Name", typeof(string));
				DataRow dataRow9 = _dtItemProduction.NewRow();
				dataRow9["Code"] = "ItemCode";
				dataRow9["Name"] = componentResourceManager5.GetString("ItemCode", CultureInfo.CreateSpecificCulture(GlobalData.CURRENTLANGUAGE)) ?? "Item Code";
				_dtItemProduction.Rows.Add(dataRow9);
				if (ProductionOrderTableHelper.ProductionLines != null && ProductionOrderTableHelper.ProductionLines.Columns.Count > 0)
				{
					foreach (DataColumn item5 in from DataColumn t in ProductionOrderTableHelper.ProductionLines.Columns
						where !string.IsNullOrEmpty(t.ColumnName) && !new string[6] { "ItemCode", "DocEntry", "LineNum", "IssuedQty", "VisOrder", "LogInstanc" }.Contains(t.ColumnName)
						select t)
					{
						DataRow dataRow10 = _dtItemProduction.NewRow();
						dataRow10["Code"] = item5.ColumnName;
						dataRow10["Name"] = componentResourceManager5.GetString(item5.ColumnName, CultureInfo.CreateSpecificCulture(GlobalData.CURRENTLANGUAGE)) ?? item5.Caption;
						_dtItemProduction.Rows.Add(dataRow10);
					}
				}
				rep_ProductionOrderTargetLine.SetDataSource(_dtItemProduction, "Name", "Code");
			}
			if (Type.GetType("Apzon.Commons.Properties.AdvanceForecast, Apzon.Commons") != null)
			{
				ComponentResourceManager componentResourceManager6 = new ComponentResourceManager(Type.GetType("Apzon.Commons.Properties.AdvanceForecast, Apzon.Commons"));
				_dtHeaderForecast = new DataTable();
				_dtHeaderForecast.Columns.Add("Code", typeof(string));
				_dtHeaderForecast.Columns.Add("Name", typeof(string));
				foreach (DataColumn item6 in from DataColumn t in ForecastTableHelper.Header.Columns
					where !string.IsNullOrEmpty(t.ColumnName) && !new string[3] { "Code", "UserSign", "CreateDate" }.Contains(t.ColumnName)
					select t)
				{
					DataRow dataRow11 = _dtHeaderForecast.NewRow();
					dataRow11["Code"] = item6.ColumnName;
					dataRow11["Name"] = componentResourceManager6.GetString(item6.ColumnName, CultureInfo.CreateSpecificCulture(GlobalData.CURRENTLANGUAGE)) ?? item6.Caption;
					_dtHeaderForecast.Rows.Add(dataRow11);
				}
				rep_ForecastTargetField.SetDataSource(_dtHeaderForecast, "Name", "Code");
			}
			if (Type.GetType("Apzon.Commons.Properties.AdvanceForecastItems, Apzon.Commons") != null)
			{
				ComponentResourceManager componentResourceManager7 = new ComponentResourceManager(Type.GetType("Apzon.Commons.Properties.AdvanceForecastItems, Apzon.Commons"));
				_dtItemForecast = new DataTable();
				_dtItemForecast.Columns.Add("Code", typeof(string));
				_dtItemForecast.Columns.Add("Name", typeof(string));
				DataRow dataRow12 = _dtItemForecast.NewRow();
				dataRow12["Code"] = "ItemCode";
				dataRow12["Name"] = componentResourceManager7.GetString("ItemCode", CultureInfo.CreateSpecificCulture(GlobalData.CURRENTLANGUAGE)) ?? "Item Code";
				_dtItemForecast.Rows.Add(dataRow12);
				foreach (DataColumn item7 in from DataColumn t in ForecastTableHelper.Items.Columns
					where !string.IsNullOrEmpty(t.ColumnName) && !new string[3] { "ItemCode", "Code", "LineID" }.Contains(t.ColumnName)
					select t)
				{
					DataRow dataRow13 = _dtItemForecast.NewRow();
					dataRow13["Code"] = item7.ColumnName;
					dataRow13["Name"] = componentResourceManager7.GetString(item7.ColumnName, CultureInfo.CreateSpecificCulture(GlobalData.CURRENTLANGUAGE)) ?? item7.Caption;
					_dtItemForecast.Rows.Add(dataRow13);
				}
				rep_ForecastItemTargetField.SetDataSource(_dtItemForecast, "Name", "Code");
			}
			_dtApprovalDecision = new DataTable();
			_dtApprovalDecision.Columns.Add("Code", typeof(string));
			_dtApprovalDecision.Columns.Add("Name", typeof(string));
			DataRow dataRow14 = _dtApprovalDecision.NewRow();
			dataRow14["Code"] = "WddCode";
			dataRow14["Name"] = "Approval Code";
			_dtApprovalDecision.Rows.Add(dataRow14);
			dataRow14 = _dtApprovalDecision.NewRow();
			dataRow14["Code"] = "status";
			dataRow14["Name"] = "status";
			_dtApprovalDecision.Rows.Add(dataRow14);
			dataRow14 = _dtApprovalDecision.NewRow();
			dataRow14["Code"] = "Remarks";
			dataRow14["Name"] = "Remarks";
			_dtApprovalDecision.Rows.Add(dataRow14);
			rep_ApprovalTargetField.SetDataSource(_dtApprovalDecision, "Name", "Code");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void NewDoc(int fatherId = -1)
	{
		try
		{
			if (tv_Doc.DataSource is DataTable dataTable && dataTable.AsEnumerable().FirstOrDefault((DataRow t) => string.IsNullOrEmpty(Function.ToString(t["Caption"])) && Function.ParseInt(t["FatherId"]) == fatherId) == null)
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["FatherId"] = fatherId;
				dataRow["FormId"] = ActionFormId;
				dataRow["ItemId"] = ActionItemId;
				dataRow["ActionTypeEx"] = 0.ToString();
				dataRow["SourceType"] = "N";
				dataTable.Rows.Add(dataRow);
				tv_Doc.DataSource = dataTable;
				tv_Doc.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void tv_Doc_CellValueChanged(object sender, DevExpress.XtraTreeList.CellValueChangedEventArgs e)
	{
		_ = 1;
		try
		{
			if (e.Column == null)
			{
				return;
			}
			if (e.Column.FieldName == "Caption")
			{
				NewDoc();
			}
			else if (e.Column.FieldName == "ActionTypeEx")
			{
				int actiontype = Function.ParseInt(tv_Doc.GetFocusedRowCellValue(e.Column));
				VisibleActionTypePage(actiontype);
				await SetFocusLine(actiontype);
				if (!new int[4] { 17, 202, 2, 4 }.Contains(actiontype))
				{
					tv_Doc.FocusedNode.SetValue("Method", "A");
				}
			}
			else if (e.Column.FieldName == "SourceType")
			{
				string text = Function.ToString(tv_Doc.GetFocusedRowCellValue(e.Column));
				VisibleSourceTypePage(text);
				if (text == "D")
				{
					tv_Doc.FocusedNode.SetValue("DataRange", "A");
				}
				else if (text == "U" && ActionFormId == ApzObjectTypes.oDashboardDetail.ToString())
				{
					tv_Doc.FocusedNode.SetValue("DataRange", "S");
				}
				await ChangeSourceField(Function.ParseInt(tv_Doc.FocusedNode.GetValue("AbsEntry")));
			}
			else if (e.Column.FieldName == "DashboardId")
			{
				int num = Function.ParseInt(tv_Doc.GetFocusedRowCellValue(e.Column));
				btn_EditDesign.Text = ((num > 0) ? "Edit Design" : "Create New Design");
			}
			else if (e.Column.FieldName == "Method")
			{
				string method = Function.ToString(tv_Doc.GetFocusedRowCellValue(e.Column));
				VisibleMethodField(Function.ParseInt(tv_Doc.FocusedNode.GetValue("ActionTypeEx")), method);
			}
			else if (e.Column.FieldName == "UniqueAlias")
			{
				string.IsNullOrEmpty(Function.ToString(e.Value));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void VisibleMethodField(int actionType, string method)
	{
		if (actionType == 30)
		{
			if (_dicTab.ContainsKey(actionType.ToString()))
			{
				_dicTab[actionType.ToString()].DocEntryVisible = method != "A";
				_dicTab[actionType.ToString()].TabMappingVisible = method == "A";
			}
			return;
		}
		LabelLine labelLine = lb_DocumentDocEntry;
		LabelLine labelLine2 = lb_ProductionEntry;
		LookUpEditEx lookUpEditEx = look_DocEntry;
		bool flag = (look_ProductionEntry.Visible = method != "A" && method != "");
		bool flag3 = (lookUpEditEx.Visible = flag);
		bool visible = (labelLine2.Visible = flag3);
		labelLine.Visible = visible;
		XtraTabControlEx xtraTabControlEx = tabControl_MappingDocument;
		XtraTabControlEx xtraTabControlEx2 = tabControl_MappingProduction;
		CheckEditEx checkEditEx = chk_ShowDocForm;
		flag = (chk_ShowProductionForm.Visible = new string[5] { "A", "U", "S", "", "F" }.Contains(method));
		flag3 = (checkEditEx.Visible = flag);
		visible = (xtraTabControlEx2.Visible = flag3);
		xtraTabControlEx.Visible = visible;
	}

	private async void tv_Doc_FocusedNodeChanged(object sender, FocusedNodeChangedEventArgs e)
	{
		if (FormMode == Enums.FormViewModeType.Busy)
		{
			return;
		}
		_isLoading = true;
		Enums.FormViewModeType currMode = FormMode;
		try
		{
			int num = Function.ParseInt(e.Node.GetValue("ActionTypeEx"));
			FormMode = Enums.FormViewModeType.Busy;
			VisibleActionTypePage(num);
			VisibleMethodField(num, Function.ToString(e.Node.GetValue("Method")));
			VisibleSourceTypePage(Function.ToString(e.Node.GetValue("SourceType")));
			InsertDataParams("S", grd_Parameters.DataSourceTable.Copy());
			InsertDataParams("Q", grd_DoQuery.DataSourceTable.Copy());
			InsertDataParams("R", grd_Report.DataSourceTable.Copy());
			InsertDataMapping("D", "H", grd_Header.DataSourceTable.Copy());
			InsertDataMapping("D", "I", grd_Items.DataSourceTable.Copy());
			InsertDataMapping("D", "B", grd_Batch.DataSourceTable.Copy());
			InsertDataMapping("W", "H", grd_ProductionHeader.DataSourceTable.Copy());
			InsertDataMapping("W", "I", grd_ProductionItem.DataSourceTable.Copy());
			InsertDataMapping("W", "B", grd_ProductionBatch.DataSourceTable.Copy());
			InsertDataMapping("V", "H", grd_Approval.DataSourceTable.Copy());
			InsertDataMapping("9", "Q", ucHttpRequest.MappingQuery);
			InsertDataMapping("9", "H", ucHttpRequest.MappingHeder);
			InsertDataMapping("9", "A", ucHttpRequest.MappingAuth);
			InsertDataHttpRequestConfig(ucHttpRequest.MappingBody);
			InsertDataAttach(grd_AttachFiles.DataSourceTable.Copy());
			InsertDataRptParams(grd_RptParam.DataSourceTable.Copy());
			InsertDataRptParams(grd_PrintLayout.DataSourceTable.Copy());
			foreach (KeyValuePair<string, UcContextActionMapping> item2 in _dicTab)
			{
				item2.Value.InsertDataMapping(_dtMapping);
			}
			if (e.OldNode != null && Function.ParseInt(e.OldNode.GetValue("ActionTypeEx")) == 206000)
			{
				foreach (XtraTabPage tabPage in tabControl_Udo.TabPages)
				{
					foreach (Control control in tabPage.Controls)
					{
						if (control is GridControlEx { DataSourceTable: var dataSourceTable })
						{
							if (dataSourceTable.IsNotNull())
							{
								string item = Function.ToString(dataSourceTable.Rows[0]["TargetItem"]);
								InsertDataMapping("O", item, dataSourceTable.Copy());
							}
							break;
						}
					}
				}
			}
			string text = Function.ToString(tv_Doc.FocusedNode.GetValue("DataFrom"));
			if (Function.ToString(e.Node.GetValue("SourceType")) == "D")
			{
				look_DataFrom.EditValue = text;
				bte_DataFrom.EditValue = ((text == "Q") ? Function.ToString(tv_Doc.FocusedNode.GetValue("DataString")) : "");
				memo_DataString.EditValue = ((text == "T") ? Function.ToString(tv_Doc.FocusedNode.GetValue("DataString")) : "");
				look_ParameterType.EditValue = Function.ToString(tv_Doc.FocusedNode.GetValue("DataParamType"));
				txt_TemporaryTableName.EditValue = Function.ToString(tv_Doc.FocusedNode.GetValue("TmpTblName"));
			}
			VisibleDataString(text);
			string text2 = Function.ToString(tv_Doc.FocusedNode.GetValue("QueryFrom"));
			string text3 = Function.ToString(tv_Doc.FocusedNode.GetValue("QueryString"));
			look_QueryFrom.EditValue = text2;
			txt_QueryString.EditValue = ((text2 == "Q") ? text3 : "");
			memo_QueryString.EditValue = ((text2 == "T") ? text3 : "");
			VisibleQueryString(text2);
			bte_FilterData.EditValue = Function.ToString(tv_Doc.FocusedNode.GetValue("FilterString"));
			Enums.ContextActionReportFrom contextActionReportFrom = (Enums.ContextActionReportFrom)(Function.ParseChar(tv_Doc.FocusedNode.GetValue("RptFrom")) ?? 'Q');
			look_ReportFrom.EditValue = Function.ToString((char)contextActionReportFrom);
			bte_RptString.EditValue = ((contextActionReportFrom == Enums.ContextActionReportFrom.DefinedQuery || contextActionReportFrom == Enums.ContextActionReportFrom.UserDefinedReport) ? Function.ToString(tv_Doc.FocusedNode.GetValue("RptString")) : "");
			memo_RptString.EditValue = ((contextActionReportFrom == Enums.ContextActionReportFrom.QueryText) ? Function.ToString(tv_Doc.FocusedNode.GetValue("RptString")) : "");
			VisibleReportString(contextActionReportFrom);
			look_ObjectType.EditValue = Function.ToString(tv_Doc.FocusedNode.GetValue("ObjType"));
			look_UdoCode.EditValue = Function.ToString(tv_Doc.FocusedNode.GetValue("UdoCode"));
			bte_PrintLayoutPath.EditValue = Function.ToString(tv_Doc.FocusedNode.GetValue("FilePath"));
			look_PrintLayoutFileType.EditValue = Function.ToString(tv_Doc.FocusedNode.GetValue("FileType"));
			look_RptUseSourceFrom.EditValue = Function.ToString(tv_Doc.FocusedNode.GetValue("RptUseSource"));
			txt_TemporaryTableName.EditValue = Function.ToString(tv_Doc.FocusedNode.GetValue("TempTblName"));
			txt_PhysicalTableName.EditValue = Function.ToString(tv_Doc.FocusedNode.GetValue("PhysicalTableName"));
			look_PrintAction.EditValue = Function.ToString(tv_Doc.FocusedNode.GetValue("PrintAction"));
			chk_JoinValue.EditValue = Function.ToString(tv_Doc.FocusedNode.GetValue("JoinValue"));
			chk_UseTempTable.Checked = !string.IsNullOrWhiteSpace(Function.ToString(tv_Doc.FocusedNode.GetValue("TempTblNameDoQuery")));
			txt_TempTableNameDoQuery.EditValue = Function.ToString(tv_Doc.FocusedNode.GetValue("TempTblNameDoQuery"));
			string value = Function.ToString(tv_Doc.FocusedNode.GetValue("DIYScript"));
			rtxt_Script.Text = value;
			btn_EditScript.Text = (string.IsNullOrEmpty(value) ? "Create New Script" : "Edit Script");
			switch (num)
			{
			case 17:
				chk_ShowDocForm.EditValue = Function.ToString(tv_Doc.FocusedNode.GetValue("ShowDocForm"));
				look_DocEntry.EditValue = Function.ToString(tv_Doc.FocusedNode.GetValue("ObjectId"));
				break;
			case 206000:
				chk_ShowUdoForm.EditValue = Function.ToString(tv_Doc.FocusedNode.GetValue("ShowDocForm"));
				break;
			case 810015:
				txt_AddressFrom.EditValue = Function.ToString(tv_Doc.FocusedNode.GetValue("AddressFrom"));
				txt_AddressTo.EditValue = Function.ToString(tv_Doc.FocusedNode.GetValue("AddressTo"));
				txt_Subject.EditValue = Function.ToString(tv_Doc.FocusedNode.GetValue("Subject"));
				txt_AddressCc.EditValue = Function.ToString(tv_Doc.FocusedNode.GetValue("AddressCc"));
				txt_AddressBcc.EditValue = Function.ToString(tv_Doc.FocusedNode.GetValue("AddressBcc"));
				snapControl1.HtmlText = Function.ToString(tv_Doc.FocusedNode.GetValue("BodyText"));
				txt_EmailGroupByFields.EditValue = Function.ToString(tv_Doc.FocusedNode.GetValue("EmailGroupByFields"));
				look_EmailGroupBy.EditValue = Function.ToString(tv_Doc.FocusedNode.GetValue("EmailGroupBy"));
				break;
			case 202:
				chk_ShowProductionForm.EditValue = Function.ToString(tv_Doc.FocusedNode.GetValue("ShowDocForm"));
				look_ProductionEntry.EditValue = Function.ToString(tv_Doc.FocusedNode.GetValue("ObjectId"));
				break;
			}
			foreach (KeyValuePair<string, UcContextActionMapping> item3 in _dicTab)
			{
				item3.Value.ShowDocForm = Function.ToString(tv_Doc.FocusedNode.GetValue("ShowDocForm")) != "N";
				item3.Value.DocEntry = Function.ToString(tv_Doc.FocusedNode.GetValue("ObjectId"));
				item3.Value.ObjectType = Function.ToString(tv_Doc.FocusedNode.GetValue("ObjType"));
			}
			await SetFocusLine(num);
			SetVisibleSourceParameter();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			_isLoading = false;
		}
		FormMode = currMode;
	}

	private void InsertDataAttach(DataTable dt)
	{
		if (_dtAttachment == null || _dtAttachment.Columns.Count == 0)
		{
			WinCommons.Error(Enums.MessageCode.CouldNotGetDataReportParamater, "");
		}
		else
		{
			if (dt == null || dt.Rows.Count == 0)
			{
				return;
			}
			DataRow[] array = _dtAttachment.Select("AbsEntry=" + Function.ParseInt(dt.Rows[0]["AbsEntry"]));
			for (int i = 0; i < array.Length; i++)
			{
				_dtAttachment.Rows.Remove(array[i]);
			}
			for (int j = 0; j < dt.Rows.Count; j++)
			{
				if (!string.IsNullOrEmpty(Function.ToString(dt.Rows[j]["FilePath"])))
				{
					DataRow dataRow = _dtAttachment.NewRow();
					dataRow.ItemArray = dt.Rows[j].ItemArray;
					_dtAttachment.Rows.Add(dataRow);
				}
			}
			_dtAttachment.AcceptChanges();
		}
	}

	private void VisibleActionTypePage(int actiontype)
	{
		tab_Documents.PageVisible = actiontype == 17;
		tab_Report.PageVisible = actiontype == 1200001;
		tab_Udo.PageVisible = actiontype == 206000;
		tab_Dashboard.PageVisible = actiontype == 7000030;
		tab_ProductionOrder.PageVisible = actiontype == 202;
		tab_Email.PageVisible = actiontype == 810015;
		tab_Forecast.PageVisible = actiontype == 8000005;
		tab_DoQuery.PageVisible = actiontype == 7000037;
		tab_PrintLayout.PageVisible = actiontype == 7000039;
		tab_ApprovalDecision.PageVisible = actiontype == 121;
		tab_DIYScript.PageVisible = actiontype == 7000040;
		tab_HttpRequest.PageVisible = actiontype == 9;
		tab_PhysicalTable.PageVisible = actiontype == 27110;
		switch (actiontype)
		{
		case 2:
			if (!_dicTab.ContainsKey(actiontype.ToString()))
			{
				XtraTabPageEx xtraTabPageEx14 = new XtraTabPageEx();
				xtraTabPageEx14.Size = new Size(926, 449);
				xtraTabPageEx14.SuspendLayout();
				UcContextActionMapping ucContextActionMapping14 = new UcContextActionMapping();
				ucContextActionMapping14.CellValueChanged = (Action)Delegate.Combine(ucContextActionMapping14.CellValueChanged, new Action(UcContextActionMappingValueChanged));
				ucContextActionMapping14.ActionFormId = ActionFormId;
				ucContextActionMapping14.ActionItemId = ActionItemId;
				ucContextActionMapping14.ObjectIdCaption = "BP Code";
				ucContextActionMapping14.ShowDocFormVisible = false;
				ucContextActionMapping14.Dock = DockStyle.Fill;
				ucContextActionMapping14.ObjectIdValueChanged = (Action<string>)Delegate.Combine(ucContextActionMapping14.ObjectIdValueChanged, new Action<string>(ObjectIdValueChanged));
				ucContextActionMapping14.ShowFormValueChanged = (Action<string>)Delegate.Combine(ucContextActionMapping14.ShowFormValueChanged, new Action<string>(ShowFormValueChanged));
				xtraTabPageEx14.Controls.Add(ucContextActionMapping14);
				xtraTabPageEx14.Name = Enums.ContextActionActionTypeEx.BusinessBusinessPartner.ToString();
				xtraTabPageEx14.FieldName = Enums.ContextActionActionTypeEx.BusinessBusinessPartner.ToString();
				xtraTabPageEx14.Text = "Business Partner Cards";
				xtraTab_Main.TabPages.Add(xtraTabPageEx14);
				xtraTabPageEx14.Visible = true;
				List<ContextActionMappingTab> tabs14 = new List<ContextActionMappingTab>
				{
					new ContextActionMappingTab
					{
						Name = "General",
						Caption = "General",
						MappingType = "2",
						TargetItem = "H",
						HasSumaryType = false,
						ResourceName = "BusinessPartners",
						FixedRow = new List<ContextActionMappingFixedRow>
						{
							new ContextActionMappingFixedRow
							{
								Code = "CardCode",
								ResourceKey = "CardCode",
								DlftCaption = "BP Code"
							},
							new ContextActionMappingFixedRow
							{
								Code = "CardName",
								ResourceKey = "CardName",
								DlftCaption = "BP Name"
							}
						},
						IgnoreFields = new List<string> { "CardCode", "CardName", "UserSign", "UserSign2", "LogInstanc", "DataSource", "ObjType" },
						Structure = BussinessPartnerTableHelper.BusinessPartner
					},
					new ContextActionMappingTab
					{
						Name = "Address",
						Caption = "Address",
						MappingType = "2",
						TargetItem = "A",
						HasSumaryType = false,
						ResourceName = "BPAddresses",
						IgnoreFields = new List<string> { "CardCode", "ObjType", "LineNum", "UserSign", "LogInstanc" },
						Structure = BussinessPartnerTableHelper.BPAddresses
					},
					new ContextActionMappingTab
					{
						Name = "Contact",
						Caption = "Contact",
						MappingType = "2",
						TargetItem = "C",
						HasSumaryType = false,
						ResourceName = "ContactEmployees",
						IgnoreFields = new List<string> { "CardCode", "ObjType", "DataSource", "LogInstanc", "UserSign", "CntctCode" },
						Structure = BussinessPartnerTableHelper.ContactEmployees
					}
				};
				ucContextActionMapping14.Tabs = tabs14;
				_dicTab[actiontype.ToString()] = ucContextActionMapping14;
				xtraTabPageEx14.ResumeLayout(performLayout: false);
			}
			break;
		case 4:
			if (!_dicTab.ContainsKey(actiontype.ToString()))
			{
				XtraTabPageEx xtraTabPageEx4 = new XtraTabPageEx();
				xtraTabPageEx4.Size = new Size(926, 449);
				xtraTabPageEx4.SuspendLayout();
				UcContextActionMapping ucContextActionMapping4 = new UcContextActionMapping();
				ucContextActionMapping4.CellValueChanged = (Action)Delegate.Combine(ucContextActionMapping4.CellValueChanged, new Action(UcContextActionMappingValueChanged));
				ucContextActionMapping4.ActionFormId = ActionFormId;
				ucContextActionMapping4.ActionItemId = ActionItemId;
				ucContextActionMapping4.ObjectIdCaption = "Item No.";
				ucContextActionMapping4.ShowDocFormVisible = false;
				ucContextActionMapping4.Dock = DockStyle.Fill;
				ucContextActionMapping4.ObjectIdValueChanged = (Action<string>)Delegate.Combine(ucContextActionMapping4.ObjectIdValueChanged, new Action<string>(ObjectIdValueChanged));
				ucContextActionMapping4.ShowFormValueChanged = (Action<string>)Delegate.Combine(ucContextActionMapping4.ShowFormValueChanged, new Action<string>(ShowFormValueChanged));
				xtraTabPageEx4.Controls.Add(ucContextActionMapping4);
				xtraTabPageEx4.Name = Enums.ContextActionActionTypeEx.Items.ToString();
				xtraTabPageEx4.FieldName = Enums.ContextActionActionTypeEx.Items.ToString();
				xtraTabPageEx4.Text = "Item Master Data";
				xtraTab_Main.TabPages.Add(xtraTabPageEx4);
				xtraTabPageEx4.Visible = true;
				List<ContextActionMappingTab> tabs4 = new List<ContextActionMappingTab>
				{
					new ContextActionMappingTab
					{
						Name = "General",
						Caption = "General",
						MappingType = "4",
						TargetItem = "H",
						HasSumaryType = false,
						ResourceName = "Items",
						FixedRow = new List<ContextActionMappingFixedRow>
						{
							new ContextActionMappingFixedRow
							{
								Code = "ItemCode",
								ResourceKey = "ItemCode",
								DlftCaption = "Item No."
							},
							new ContextActionMappingFixedRow
							{
								Code = "ItemName",
								ResourceKey = "ItemName",
								DlftCaption = "Item Description"
							}
						},
						IgnoreFields = new List<string> { "ItemCode", "ItemName", "UserSign", "UserSign2", "LogInstanc", "DataSource", "ObjType" },
						Structure = ItemsTableHelper.Items
					},
					new ContextActionMappingTab
					{
						Name = "Prices",
						Caption = "Prices",
						MappingType = "4",
						TargetItem = "P",
						HasSumaryType = false,
						ResourceName = "Item_Prices",
						IgnoreFields = new List<string> { "ItemCode", "ObjType", "LineNum", "UserSign", "LogInstanc" },
						Structure = ItemsTableHelper.ItemPrices
					},
					new ContextActionMappingTab
					{
						Name = "Warehouses",
						Caption = "Warehouses",
						MappingType = "4",
						TargetItem = "W",
						HasSumaryType = false,
						ResourceName = "ItemWarehouses",
						IgnoreFields = new List<string> { "ItemCode", "ObjType", "DataSource", "LogInstanc", "UserSign", "CntctCode" },
						Structure = ItemsTableHelper.ItemWarehouses
					}
				};
				ucContextActionMapping4.Tabs = tabs4;
				_dicTab[actiontype.ToString()] = ucContextActionMapping4;
				xtraTabPageEx4.ResumeLayout(performLayout: false);
			}
			break;
		case 66:
			if (!_dicTab.ContainsKey(actiontype.ToString()))
			{
				XtraTabPageEx xtraTabPageEx7 = new XtraTabPageEx();
				xtraTabPageEx7.Size = new Size(926, 449);
				xtraTabPageEx7.SuspendLayout();
				UcContextActionMapping ucContextActionMapping7 = new UcContextActionMapping();
				ucContextActionMapping7.CellValueChanged = (Action)Delegate.Combine(ucContextActionMapping7.CellValueChanged, new Action(UcContextActionMappingValueChanged));
				ucContextActionMapping7.ActionFormId = ActionFormId;
				ucContextActionMapping7.ActionItemId = ActionItemId;
				ucContextActionMapping7.ObjectIdCaption = "Product Code";
				ucContextActionMapping7.ShowDocFormVisible = false;
				ucContextActionMapping7.Dock = DockStyle.Fill;
				ucContextActionMapping7.ObjectIdValueChanged = (Action<string>)Delegate.Combine(ucContextActionMapping7.ObjectIdValueChanged, new Action<string>(ObjectIdValueChanged));
				ucContextActionMapping7.ShowFormValueChanged = (Action<string>)Delegate.Combine(ucContextActionMapping7.ShowFormValueChanged, new Action<string>(ShowFormValueChanged));
				xtraTabPageEx7.Controls.Add(ucContextActionMapping7);
				xtraTabPageEx7.Name = Enums.ContextActionActionTypeEx.BillofMaterials.ToString();
				xtraTabPageEx7.FieldName = Enums.ContextActionActionTypeEx.BillofMaterials.ToString();
				xtraTabPageEx7.Text = "Bill Of Materials";
				xtraTab_Main.TabPages.Add(xtraTabPageEx7);
				xtraTabPageEx7.Visible = true;
				List<ContextActionMappingTab> tabs7 = new List<ContextActionMappingTab>
				{
					new ContextActionMappingTab
					{
						Name = "General",
						Caption = "General",
						MappingType = "T",
						TargetItem = "H",
						HasSumaryType = false,
						ResourceName = "ProductTree",
						FixedRow = new List<ContextActionMappingFixedRow>
						{
							new ContextActionMappingFixedRow
							{
								Code = "Code",
								ResourceKey = "Code",
								DlftCaption = "Parent Item"
							},
							new ContextActionMappingFixedRow
							{
								Code = "TreeType",
								ResourceKey = "TreeType",
								DlftCaption = "BOM Type"
							}
						},
						IgnoreFields = new List<string>
						{
							"Code", "TreeType", "UserSign", "UserSign2", "LogInstac", "DataSource", "Object", "Transfered", "CreateDate", "UpdateDate",
							"UpdateTime"
						},
						Structure = ProductTreeTableHelper.ProductTreeHeader
					},
					new ContextActionMappingTab
					{
						Name = "Components",
						Caption = "Components",
						MappingType = "T",
						TargetItem = "I",
						HasSumaryType = false,
						ResourceName = "ProductTreeComponents",
						IgnoreFields = new List<string> { "Father", "Object", "ChildNum", "UserSign", "LogInstanc" },
						Structure = ProductTreeTableHelper.ProductTreeLines
					}
				};
				ucContextActionMapping7.Tabs = tabs7;
				_dicTab[actiontype.ToString()] = ucContextActionMapping7;
				xtraTabPageEx7.ResumeLayout(performLayout: false);
			}
			break;
		case 30:
			if (!_dicTab.ContainsKey(actiontype.ToString()))
			{
				XtraTabPageEx xtraTabPageEx18 = new XtraTabPageEx();
				xtraTabPageEx18.Size = new Size(926, 449);
				xtraTabPageEx18.SuspendLayout();
				UcContextActionMapping ucContextActionMapping18 = new UcContextActionMapping();
				ucContextActionMapping18.CellValueChanged = (Action)Delegate.Combine(ucContextActionMapping18.CellValueChanged, new Action(UcContextActionMappingValueChanged));
				ucContextActionMapping18.ActionFormId = ActionFormId;
				ucContextActionMapping18.ActionItemId = ActionItemId;
				ucContextActionMapping18.ObjectIdCaption = "TransId";
				ucContextActionMapping18.ShowDocFormVisible = false;
				ucContextActionMapping18.Dock = DockStyle.Fill;
				ucContextActionMapping18.ObjectIdValueChanged = (Action<string>)Delegate.Combine(ucContextActionMapping18.ObjectIdValueChanged, new Action<string>(ObjectIdValueChanged));
				ucContextActionMapping18.ShowFormValueChanged = (Action<string>)Delegate.Combine(ucContextActionMapping18.ShowFormValueChanged, new Action<string>(ShowFormValueChanged));
				xtraTabPageEx18.Controls.Add(ucContextActionMapping18);
				xtraTabPageEx18.Name = Enums.ContextActionActionTypeEx.JournalEntry.ToString();
				xtraTabPageEx18.FieldName = Enums.ContextActionActionTypeEx.JournalEntry.ToString();
				xtraTabPageEx18.Text = "Journal Entry";
				xtraTab_Main.TabPages.Add(xtraTabPageEx18);
				xtraTabPageEx18.Visible = true;
				List<ContextActionMappingTab> tabs18 = new List<ContextActionMappingTab>
				{
					new ContextActionMappingTab
					{
						Name = "General",
						Caption = "General",
						MappingType = "J",
						TargetItem = "H",
						HasSumaryType = false,
						ResourceName = "JournalEntry",
						IgnoreFields = new List<string> { "TransId", "UpdateDate", "CreateDate", "UserSign", "UserSign2", "LogInstanc", "CreateTime", "Printed" },
						Structure = JournalEntryTableHelper.JournalEntry
					},
					new ContextActionMappingTab
					{
						Name = "Components",
						Caption = "Rows",
						MappingType = "J",
						TargetItem = "I",
						HasSumaryType = false,
						ResourceName = "JournalEntryRows",
						IgnoreFields = new List<string> { "TransId", "Line_ID", "LogInstanc", "UserSign" },
						Structure = JournalEntryTableHelper.JournalEntryRows
					}
				};
				ucContextActionMapping18.Tabs = tabs18;
				_dicTab[actiontype.ToString()] = ucContextActionMapping18;
				xtraTabPageEx18.ResumeLayout(performLayout: false);
			}
			break;
		case 27:
			if (!_dicTab.ContainsKey(actiontype.ToString()))
			{
				XtraTabPageEx xtraTabPageEx9 = new XtraTabPageEx();
				xtraTabPageEx9.Size = new Size(926, 449);
				xtraTabPageEx9.SuspendLayout();
				UcContextActionMapping ucContextActionMapping9 = new UcContextActionMapping();
				ucContextActionMapping9.CellValueChanged = (Action)Delegate.Combine(ucContextActionMapping9.CellValueChanged, new Action(UcContextActionMappingValueChanged));
				ucContextActionMapping9.ActionFormId = ActionFormId;
				ucContextActionMapping9.ActionItemId = ActionItemId;
				ucContextActionMapping9.ObjectIdCaption = "JV No.";
				ucContextActionMapping9.ShowDocFormVisible = false;
				ucContextActionMapping9.Dock = DockStyle.Fill;
				ucContextActionMapping9.ObjectIdValueChanged = (Action<string>)Delegate.Combine(ucContextActionMapping9.ObjectIdValueChanged, new Action<string>(ObjectIdValueChanged));
				ucContextActionMapping9.ShowFormValueChanged = (Action<string>)Delegate.Combine(ucContextActionMapping9.ShowFormValueChanged, new Action<string>(ShowFormValueChanged));
				xtraTabPageEx9.Controls.Add(ucContextActionMapping9);
				xtraTabPageEx9.Name = Enums.ContextActionActionTypeEx.JournalVouchers.ToString();
				xtraTabPageEx9.FieldName = Enums.ContextActionActionTypeEx.JournalVouchers.ToString();
				xtraTabPageEx9.Text = "Journal Vourcher";
				xtraTab_Main.TabPages.Add(xtraTabPageEx9);
				xtraTabPageEx9.Visible = true;
				List<ContextActionMappingTab> tabs9 = new List<ContextActionMappingTab>
				{
					new ContextActionMappingTab
					{
						Name = "General",
						Caption = "General",
						MappingType = "J",
						TargetItem = "H",
						HasSumaryType = false,
						ResourceName = "JournalVoucher",
						IgnoreFields = new List<string> { "BatchNum", "UpdateDate", "CreateDate", "UserSign", "UserSign2", "LogInstanc", "CreateTime", "Printed" },
						Structure = JournalEntryTableHelper.JournalVoucher
					},
					new ContextActionMappingTab
					{
						Name = "Components",
						Caption = "Rows",
						MappingType = "J",
						TargetItem = "I",
						HasSumaryType = false,
						ResourceName = "JournalVoucher_Rows",
						IgnoreFields = new List<string> { "LogInstanc", "UserSign" },
						Structure = JournalEntryTableHelper.JournalVoucherRows
					}
				};
				ucContextActionMapping9.Tabs = tabs9;
				_dicTab[actiontype.ToString()] = ucContextActionMapping9;
				xtraTabPageEx9.ResumeLayout(performLayout: false);
			}
			break;
		case 1470000128:
			if (!_dicTab.ContainsKey(actiontype.ToString()))
			{
				XtraTabPageEx xtraTabPageEx11 = new XtraTabPageEx();
				xtraTabPageEx11.Size = new Size(926, 449);
				xtraTabPageEx11.SuspendLayout();
				UcContextActionMapping ucContextActionMapping11 = new UcContextActionMapping();
				ucContextActionMapping11.CellValueChanged = (Action)Delegate.Combine(ucContextActionMapping11.CellValueChanged, new Action(UcContextActionMappingValueChanged));
				ucContextActionMapping11.ActionFormId = ActionFormId;
				ucContextActionMapping11.ActionItemId = ActionItemId;
				ucContextActionMapping11.ObjectIdCaption = "Task Id";
				ucContextActionMapping11.ShowDocFormVisible = true;
				ucContextActionMapping11.Dock = DockStyle.Fill;
				ucContextActionMapping11.ObjectIdValueChanged = (Action<string>)Delegate.Combine(ucContextActionMapping11.ObjectIdValueChanged, new Action<string>(ObjectIdValueChanged));
				ucContextActionMapping11.ShowFormValueChanged = (Action<string>)Delegate.Combine(ucContextActionMapping11.ShowFormValueChanged, new Action<string>(ShowFormValueChanged));
				xtraTabPageEx11.Controls.Add(ucContextActionMapping11);
				xtraTabPageEx11.Name = Enums.ContextActionActionTypeEx.Task.ToString();
				xtraTabPageEx11.FieldName = Enums.ContextActionActionTypeEx.Task.ToString();
				xtraTabPageEx11.Text = "Task Management";
				xtraTab_Main.TabPages.Add(xtraTabPageEx11);
				xtraTabPageEx11.Visible = true;
				List<ContextActionMappingTab> tabs11 = new List<ContextActionMappingTab>
				{
					new ContextActionMappingTab
					{
						Name = "General",
						Caption = "General",
						MappingType = "A",
						TargetItem = "H",
						HasSumaryType = false,
						ResourceName = "TaskManagement",
						IgnoreFields = new List<string> { "ClgCode", "DataSource", "Transfered", "UserSign" },
						Structure = ActivitiesTableHelper.Acivity
					}
				};
				ucContextActionMapping11.Tabs = tabs11;
				_dicTab[actiontype.ToString()] = ucContextActionMapping11;
				xtraTabPageEx11.ResumeLayout(performLayout: false);
			}
			break;
		case 19000102:
			if (!_dicTab.ContainsKey(actiontype.ToString()))
			{
				XtraTabPageEx xtraTabPageEx20 = new XtraTabPageEx();
				xtraTabPageEx20.Size = new Size(926, 449);
				xtraTabPageEx20.SuspendLayout();
				UcContextActionMapping ucContextActionMapping20 = new UcContextActionMapping();
				ucContextActionMapping20.CellValueChanged = (Action)Delegate.Combine(ucContextActionMapping20.CellValueChanged, new Action(UcContextActionMappingValueChanged));
				ucContextActionMapping20.ActionFormId = ActionFormId;
				ucContextActionMapping20.ActionItemId = ActionItemId;
				ucContextActionMapping20.ObjectIdCaption = "Note Id";
				ucContextActionMapping20.ShowDocFormVisible = true;
				ucContextActionMapping20.Dock = DockStyle.Fill;
				ucContextActionMapping20.ObjectIdValueChanged = (Action<string>)Delegate.Combine(ucContextActionMapping20.ObjectIdValueChanged, new Action<string>(ObjectIdValueChanged));
				ucContextActionMapping20.ShowFormValueChanged = (Action<string>)Delegate.Combine(ucContextActionMapping20.ShowFormValueChanged, new Action<string>(ShowFormValueChanged));
				xtraTabPageEx20.Controls.Add(ucContextActionMapping20);
				xtraTabPageEx20.Name = Enums.ContextActionActionTypeEx.Note.ToString();
				xtraTabPageEx20.FieldName = Enums.ContextActionActionTypeEx.Note.ToString();
				xtraTabPageEx20.Text = "Note Management";
				xtraTab_Main.TabPages.Add(xtraTabPageEx20);
				xtraTabPageEx20.Visible = true;
				List<ContextActionMappingTab> tabs20 = new List<ContextActionMappingTab>
				{
					new ContextActionMappingTab
					{
						Name = "General",
						Caption = "General",
						MappingType = "A",
						TargetItem = "H",
						HasSumaryType = false,
						ResourceName = "NoteManagement",
						IgnoreFields = new List<string> { "ClgCode", "DataSource", "Transfered", "UserSign" },
						Structure = ActivitiesTableHelper.Acivity
					}
				};
				ucContextActionMapping20.Tabs = tabs20;
				_dicTab[actiontype.ToString()] = ucContextActionMapping20;
				xtraTabPageEx20.ResumeLayout(performLayout: false);
			}
			break;
		case 1470000129:
			if (!_dicTab.ContainsKey(actiontype.ToString()))
			{
				XtraTabPageEx xtraTabPageEx16 = new XtraTabPageEx();
				xtraTabPageEx16.Size = new Size(926, 449);
				xtraTabPageEx16.SuspendLayout();
				UcContextActionMapping ucContextActionMapping16 = new UcContextActionMapping();
				ucContextActionMapping16.CellValueChanged = (Action)Delegate.Combine(ucContextActionMapping16.CellValueChanged, new Action(UcContextActionMappingValueChanged));
				ucContextActionMapping16.ActionFormId = ActionFormId;
				ucContextActionMapping16.ActionItemId = ActionItemId;
				ucContextActionMapping16.ObjectIdCaption = "Event Id";
				ucContextActionMapping16.ShowDocFormVisible = true;
				ucContextActionMapping16.Dock = DockStyle.Fill;
				ucContextActionMapping16.ObjectIdValueChanged = (Action<string>)Delegate.Combine(ucContextActionMapping16.ObjectIdValueChanged, new Action<string>(ObjectIdValueChanged));
				ucContextActionMapping16.ShowFormValueChanged = (Action<string>)Delegate.Combine(ucContextActionMapping16.ShowFormValueChanged, new Action<string>(ShowFormValueChanged));
				xtraTabPageEx16.Controls.Add(ucContextActionMapping16);
				xtraTabPageEx16.Name = Enums.ContextActionActionTypeEx.Event.ToString();
				xtraTabPageEx16.FieldName = Enums.ContextActionActionTypeEx.Event.ToString();
				xtraTabPageEx16.Text = "Event Management";
				xtraTab_Main.TabPages.Add(xtraTabPageEx16);
				xtraTabPageEx16.Visible = true;
				List<ContextActionMappingTab> tabs16 = new List<ContextActionMappingTab>
				{
					new ContextActionMappingTab
					{
						Name = "General",
						Caption = "General",
						MappingType = "A",
						TargetItem = "H",
						HasSumaryType = false,
						ResourceName = "EventManagement",
						IgnoreFields = new List<string> { "ClgCode", "DataSource", "Transfered", "UserSign" },
						Structure = ActivitiesTableHelper.Acivity
					}
				};
				ucContextActionMapping16.Tabs = tabs16;
				_dicTab[actiontype.ToString()] = ucContextActionMapping16;
				xtraTabPageEx16.ResumeLayout(performLayout: false);
			}
			break;
		case 1470000132:
			if (!_dicTab.ContainsKey(actiontype.ToString()))
			{
				XtraTabPageEx xtraTabPageEx10 = new XtraTabPageEx();
				xtraTabPageEx10.Size = new Size(926, 449);
				xtraTabPageEx10.SuspendLayout();
				UcContextActionMapping ucContextActionMapping10 = new UcContextActionMapping();
				ucContextActionMapping10.CellValueChanged = (Action)Delegate.Combine(ucContextActionMapping10.CellValueChanged, new Action(UcContextActionMappingValueChanged));
				ucContextActionMapping10.ActionFormId = ActionFormId;
				ucContextActionMapping10.ActionItemId = ActionItemId;
				ucContextActionMapping10.ObjectIdCaption = "Internal Entry";
				ucContextActionMapping10.ShowDocFormVisible = true;
				ucContextActionMapping10.Dock = DockStyle.Fill;
				ucContextActionMapping10.ObjectIdValueChanged = (Action<string>)Delegate.Combine(ucContextActionMapping10.ObjectIdValueChanged, new Action<string>(ObjectIdValueChanged));
				ucContextActionMapping10.ShowFormValueChanged = (Action<string>)Delegate.Combine(ucContextActionMapping10.ShowFormValueChanged, new Action<string>(ShowFormValueChanged));
				xtraTabPageEx10.Controls.Add(ucContextActionMapping10);
				xtraTabPageEx10.Name = Enums.ContextActionActionTypeEx.ProjectManagement.ToString();
				xtraTabPageEx10.FieldName = Enums.ContextActionActionTypeEx.ProjectManagement.ToString();
				xtraTabPageEx10.Text = "Project Management";
				xtraTab_Main.TabPages.Add(xtraTabPageEx10);
				xtraTabPageEx10.Visible = true;
				List<ContextActionMappingTab> tabs10 = new List<ContextActionMappingTab>
				{
					new ContextActionMappingTab
					{
						Name = "General",
						Caption = "General",
						MappingType = "5",
						TargetItem = "H",
						HasSumaryType = false,
						ResourceName = "ProjectManagement",
						IgnoreFields = new List<string> { "AbsEntry", "BudgetAmount", "ActualAmount", "AmountDiff" },
						Structure = ProjectManagementTableHelper.Project
					},
					new ContextActionMappingTab
					{
						Name = "Wbs",
						Caption = "Work breakdown structure",
						MappingType = "5",
						TargetItem = "W",
						HasSumaryType = false,
						ResourceName = "ProjectManagementWbs",
						IgnoreFields = new List<string>(),
						Structure = ProjectManagementTableHelper.ProjectWorkBreakdownStructure
					},
					new ContextActionMappingTab
					{
						Name = "BudgetItem",
						Caption = "Project Budget",
						MappingType = "5",
						TargetItem = "B",
						HasSumaryType = false,
						ResourceName = "ProjectManagementBudgetItem",
						IgnoreFields = new List<string>(),
						Structure = ProjectManagementTableHelper.ProjectBudget
					}
				};
				ucContextActionMapping10.Tabs = tabs10;
				_dicTab[actiontype.ToString()] = ucContextActionMapping10;
				xtraTabPageEx10.ResumeLayout(performLayout: false);
			}
			break;
		case 6:
			if (!_dicTab.ContainsKey(actiontype.ToString()))
			{
				XtraTabPageEx xtraTabPageEx12 = new XtraTabPageEx();
				xtraTabPageEx12.Size = new Size(926, 449);
				xtraTabPageEx12.SuspendLayout();
				UcContextActionMapping ucContextActionMapping12 = new UcContextActionMapping();
				ucContextActionMapping12.CellValueChanged = (Action)Delegate.Combine(ucContextActionMapping12.CellValueChanged, new Action(UcContextActionMappingValueChanged));
				ucContextActionMapping12.ActionFormId = ActionFormId;
				ucContextActionMapping12.ActionItemId = ActionItemId;
				ucContextActionMapping12.ObjectIdCaption = "Opportunity Id";
				ucContextActionMapping12.ShowDocFormVisible = true;
				ucContextActionMapping12.Dock = DockStyle.Fill;
				ucContextActionMapping12.ObjectIdValueChanged = (Action<string>)Delegate.Combine(ucContextActionMapping12.ObjectIdValueChanged, new Action<string>(ObjectIdValueChanged));
				ucContextActionMapping12.ShowFormValueChanged = (Action<string>)Delegate.Combine(ucContextActionMapping12.ShowFormValueChanged, new Action<string>(ShowFormValueChanged));
				xtraTabPageEx12.Controls.Add(ucContextActionMapping12);
				xtraTabPageEx12.Name = Enums.ContextActionActionTypeEx.Opportunity.ToString();
				xtraTabPageEx12.FieldName = Enums.ContextActionActionTypeEx.Opportunity.ToString();
				xtraTabPageEx12.Text = "Opportunity";
				xtraTab_Main.TabPages.Add(xtraTabPageEx12);
				xtraTabPageEx12.Visible = true;
				List<ContextActionMappingTab> tabs12 = new List<ContextActionMappingTab>
				{
					new ContextActionMappingTab
					{
						Name = "General",
						Caption = "General",
						MappingType = "6",
						TargetItem = "H",
						HasSumaryType = false,
						ResourceName = "Opportuntity",
						IgnoreFields = new List<string> { "OpprId", "UserSign", "Transfered", "Instance", "LogInstanc", "UserSign2", "UpdateDate", "UpdateTS" },
						Structure = OpportunityTableHelper.Opportunity
					},
					new ContextActionMappingTab
					{
						Name = "Stages",
						Caption = "Stages",
						MappingType = "6",
						TargetItem = "S",
						HasSumaryType = false,
						ResourceName = "OpportuntityStages",
						IgnoreFields = new List<string> { "OpprId", "Line", "ObjType", "UserSign" },
						Structure = OpportunityTableHelper.OpportunityStages
					},
					new ContextActionMappingTab
					{
						Name = "ItemDetail",
						Caption = "Item Detail",
						MappingType = "6",
						TargetItem = "I",
						HasSumaryType = false,
						ResourceName = "OpportunityItems",
						IgnoreFields = new List<string>(),
						Structure = OpportunityTableHelper.OpportunityItems
					}
				};
				ucContextActionMapping12.Tabs = tabs12;
				_dicTab[actiontype.ToString()] = ucContextActionMapping12;
				xtraTabPageEx12.ResumeLayout(performLayout: false);
			}
			break;
		case 1470000130:
			if (!_dicTab.ContainsKey(actiontype.ToString()))
			{
				XtraTabPageEx xtraTabPageEx8 = new XtraTabPageEx();
				xtraTabPageEx8.Size = new Size(926, 449);
				xtraTabPageEx8.SuspendLayout();
				UcContextActionMapping ucContextActionMapping8 = new UcContextActionMapping();
				ucContextActionMapping8.CellValueChanged = (Action)Delegate.Combine(ucContextActionMapping8.CellValueChanged, new Action(UcContextActionMappingValueChanged));
				ucContextActionMapping8.ActionFormId = ActionFormId;
				ucContextActionMapping8.ActionItemId = ActionItemId;
				ucContextActionMapping8.ObjectIdCaption = "No.";
				ucContextActionMapping8.ShowDocFormVisible = true;
				ucContextActionMapping8.Dock = DockStyle.Fill;
				ucContextActionMapping8.ObjectIdValueChanged = (Action<string>)Delegate.Combine(ucContextActionMapping8.ObjectIdValueChanged, new Action<string>(ObjectIdValueChanged));
				ucContextActionMapping8.ShowFormValueChanged = (Action<string>)Delegate.Combine(ucContextActionMapping8.ShowFormValueChanged, new Action<string>(ShowFormValueChanged));
				xtraTabPageEx8.Controls.Add(ucContextActionMapping8);
				xtraTabPageEx8.Name = Enums.ContextActionActionTypeEx.TrackingIssue.ToString();
				xtraTabPageEx8.FieldName = Enums.ContextActionActionTypeEx.TrackingIssue.ToString();
				xtraTabPageEx8.Text = "Tracking Issue";
				xtraTab_Main.TabPages.Add(xtraTabPageEx8);
				xtraTabPageEx8.Visible = true;
				List<ContextActionMappingTab> tabs8 = new List<ContextActionMappingTab>
				{
					new ContextActionMappingTab
					{
						Name = "General",
						Caption = "General",
						MappingType = "Y",
						TargetItem = "H",
						HasSumaryType = false,
						ResourceName = "TrackingIssues",
						IgnoreFields = new List<string> { "Id", "AttachedFiled", "UserSign", "UserSign2" },
						Structure = TrackingIssueTableHelper.TrackingIssues
					}
				};
				ucContextActionMapping8.Tabs = tabs8;
				_dicTab[actiontype.ToString()] = ucContextActionMapping8;
				xtraTabPageEx8.ResumeLayout(performLayout: false);
			}
			break;
		case 1:
			if (!_dicTab.ContainsKey(actiontype.ToString()))
			{
				XtraTabPageEx xtraTabPageEx6 = new XtraTabPageEx();
				xtraTabPageEx6.Size = new Size(926, 449);
				xtraTabPageEx6.SuspendLayout();
				UcContextActionMapping ucContextActionMapping6 = new UcContextActionMapping();
				ucContextActionMapping6.CellValueChanged = (Action)Delegate.Combine(ucContextActionMapping6.CellValueChanged, new Action(UcContextActionMappingValueChanged));
				ucContextActionMapping6.ActionFormId = ActionFormId;
				ucContextActionMapping6.ActionItemId = ActionItemId;
				ucContextActionMapping6.ObjectIdCaption = "Account Code";
				ucContextActionMapping6.ShowDocFormVisible = true;
				ucContextActionMapping6.Dock = DockStyle.Fill;
				ucContextActionMapping6.ObjectIdValueChanged = (Action<string>)Delegate.Combine(ucContextActionMapping6.ObjectIdValueChanged, new Action<string>(ObjectIdValueChanged));
				ucContextActionMapping6.ShowFormValueChanged = (Action<string>)Delegate.Combine(ucContextActionMapping6.ShowFormValueChanged, new Action<string>(ShowFormValueChanged));
				xtraTabPageEx6.Controls.Add(ucContextActionMapping6);
				xtraTabPageEx6.Name = Enums.ContextActionActionTypeEx.ChartOfAccount.ToString();
				xtraTabPageEx6.FieldName = Enums.ContextActionActionTypeEx.ChartOfAccount.ToString();
				xtraTabPageEx6.Text = "Chart of Account";
				xtraTab_Main.TabPages.Add(xtraTabPageEx6);
				xtraTabPageEx6.Visible = true;
				List<ContextActionMappingTab> tabs6 = new List<ContextActionMappingTab>
				{
					new ContextActionMappingTab
					{
						Name = "General",
						Caption = "General",
						MappingType = "-",
						TargetItem = "H",
						HasSumaryType = false,
						ResourceName = "ChartOfAccount",
						IgnoreFields = new List<string>(),
						Structure = ChartOfAccountTableHelper.Account
					}
				};
				ucContextActionMapping6.Tabs = tabs6;
				_dicTab[actiontype.ToString()] = ucContextActionMapping6;
				xtraTabPageEx6.ResumeLayout(performLayout: false);
			}
			break;
		case 62:
			if (!_dicTab.ContainsKey(actiontype.ToString()))
			{
				XtraTabPageEx xtraTabPageEx2 = new XtraTabPageEx();
				xtraTabPageEx2.Size = new Size(926, 449);
				xtraTabPageEx2.SuspendLayout();
				UcContextActionMapping ucContextActionMapping2 = new UcContextActionMapping();
				ucContextActionMapping2.CellValueChanged = (Action)Delegate.Combine(ucContextActionMapping2.CellValueChanged, new Action(UcContextActionMappingValueChanged));
				ucContextActionMapping2.ActionFormId = ActionFormId;
				ucContextActionMapping2.ActionItemId = ActionItemId;
				ucContextActionMapping2.ObjectIdCaption = "Code";
				ucContextActionMapping2.ShowDocFormVisible = true;
				ucContextActionMapping2.Dock = DockStyle.Fill;
				ucContextActionMapping2.ObjectIdValueChanged = (Action<string>)Delegate.Combine(ucContextActionMapping2.ObjectIdValueChanged, new Action<string>(ObjectIdValueChanged));
				ucContextActionMapping2.ShowFormValueChanged = (Action<string>)Delegate.Combine(ucContextActionMapping2.ShowFormValueChanged, new Action<string>(ShowFormValueChanged));
				xtraTabPageEx2.Controls.Add(ucContextActionMapping2);
				xtraTabPageEx2.Name = Enums.ContextActionActionTypeEx.DistributionRule.ToString();
				xtraTabPageEx2.FieldName = Enums.ContextActionActionTypeEx.DistributionRule.ToString();
				xtraTabPageEx2.Text = "Distribute Rule";
				xtraTab_Main.TabPages.Add(xtraTabPageEx2);
				xtraTabPageEx2.Visible = true;
				List<ContextActionMappingTab> tabs2 = new List<ContextActionMappingTab>
				{
					new ContextActionMappingTab
					{
						Name = "General",
						Caption = "Distribution Rule",
						MappingType = "-",
						TargetItem = "H",
						HasSumaryType = false,
						ResourceName = "DistributionRule",
						IgnoreFields = new List<string>(),
						Structure = DistributeRuleTableHelper.DistributeRule
					},
					new ContextActionMappingTab
					{
						Name = "DistributionRuleRows",
						Caption = "Distribution Rule Rows",
						MappingType = "-",
						TargetItem = "I",
						HasSumaryType = false,
						ResourceName = "DistributionRuleRows",
						IgnoreFields = new List<string> { "OcrCode" },
						Structure = DistributeRuleTableHelper.DistributionRuleRows
					}
				};
				ucContextActionMapping2.Tabs = tabs2;
				_dicTab[actiontype.ToString()] = ucContextActionMapping2;
				xtraTabPageEx2.ResumeLayout(performLayout: false);
			}
			break;
		case 5:
			if (!_dicTab.ContainsKey(actiontype.ToString()))
			{
				XtraTabPageEx xtraTabPageEx19 = new XtraTabPageEx();
				xtraTabPageEx19.Size = new Size(926, 449);
				xtraTabPageEx19.SuspendLayout();
				UcContextActionMapping ucContextActionMapping19 = new UcContextActionMapping();
				ucContextActionMapping19.CellValueChanged = (Action)Delegate.Combine(ucContextActionMapping19.CellValueChanged, new Action(UcContextActionMappingValueChanged));
				ucContextActionMapping19.ActionFormId = ActionFormId;
				ucContextActionMapping19.ActionItemId = ActionItemId;
				ucContextActionMapping19.ObjectIdCaption = "Code";
				ucContextActionMapping19.ShowDocFormVisible = true;
				ucContextActionMapping19.Dock = DockStyle.Fill;
				ucContextActionMapping19.ObjectIdValueChanged = (Action<string>)Delegate.Combine(ucContextActionMapping19.ObjectIdValueChanged, new Action<string>(ObjectIdValueChanged));
				ucContextActionMapping19.ShowFormValueChanged = (Action<string>)Delegate.Combine(ucContextActionMapping19.ShowFormValueChanged, new Action<string>(ShowFormValueChanged));
				xtraTabPageEx19.Controls.Add(ucContextActionMapping19);
				xtraTabPageEx19.Name = Enums.ContextActionActionTypeEx.VatGroup.ToString();
				xtraTabPageEx19.FieldName = Enums.ContextActionActionTypeEx.VatGroup.ToString();
				xtraTabPageEx19.Text = "VAT Groups";
				xtraTab_Main.TabPages.Add(xtraTabPageEx19);
				xtraTabPageEx19.Visible = true;
				List<ContextActionMappingTab> tabs19 = new List<ContextActionMappingTab>
				{
					new ContextActionMappingTab
					{
						Name = "General",
						Caption = "General",
						MappingType = "-",
						TargetItem = "H",
						HasSumaryType = false,
						ResourceName = "VatGroup",
						IgnoreFields = new List<string>(),
						Structure = VatGroupTableHelper.VatGroup
					},
					new ContextActionMappingTab
					{
						Name = "TaxDefinition",
						Caption = "Tax Definition",
						MappingType = "-",
						TargetItem = "I",
						HasSumaryType = false,
						ResourceName = "VatGroupLines",
						IgnoreFields = new List<string> { "Code", "Name" },
						Structure = VatGroupTableHelper.VatGroupLines
					}
				};
				ucContextActionMapping19.Tabs = tabs19;
				_dicTab[actiontype.ToString()] = ucContextActionMapping19;
				xtraTabPageEx19.ResumeLayout(performLayout: false);
			}
			break;
		case 53:
			if (!_dicTab.ContainsKey(actiontype.ToString()))
			{
				XtraTabPageEx xtraTabPageEx17 = new XtraTabPageEx();
				xtraTabPageEx17.Size = new Size(926, 449);
				xtraTabPageEx17.SuspendLayout();
				UcContextActionMapping ucContextActionMapping17 = new UcContextActionMapping();
				ucContextActionMapping17.CellValueChanged = (Action)Delegate.Combine(ucContextActionMapping17.CellValueChanged, new Action(UcContextActionMappingValueChanged));
				ucContextActionMapping17.ActionFormId = ActionFormId;
				ucContextActionMapping17.ActionItemId = ActionItemId;
				ucContextActionMapping17.ObjectIdCaption = "Sales Employees/Buyers";
				ucContextActionMapping17.ShowDocFormVisible = true;
				ucContextActionMapping17.Dock = DockStyle.Fill;
				ucContextActionMapping17.ObjectIdValueChanged = (Action<string>)Delegate.Combine(ucContextActionMapping17.ObjectIdValueChanged, new Action<string>(ObjectIdValueChanged));
				ucContextActionMapping17.ShowFormValueChanged = (Action<string>)Delegate.Combine(ucContextActionMapping17.ShowFormValueChanged, new Action<string>(ShowFormValueChanged));
				xtraTabPageEx17.Controls.Add(ucContextActionMapping17);
				xtraTabPageEx17.Name = Enums.ContextActionActionTypeEx.SalesPersons.ToString();
				xtraTabPageEx17.FieldName = Enums.ContextActionActionTypeEx.SalesPersons.ToString();
				xtraTabPageEx17.Text = "Sales Employees/Buyers";
				xtraTab_Main.TabPages.Add(xtraTabPageEx17);
				xtraTabPageEx17.Visible = true;
				List<ContextActionMappingTab> tabs17 = new List<ContextActionMappingTab>
				{
					new ContextActionMappingTab
					{
						Name = "General",
						Caption = "General",
						MappingType = "A",
						TargetItem = "H",
						HasSumaryType = false,
						ResourceName = "SalesEmployee",
						IgnoreFields = new List<string> { "SlpCode", "DataSource", "UserSign" },
						Structure = _httpClient.SalesEmployeeBuyerClient.GetStructure()
					}
				};
				ucContextActionMapping17.Tabs = tabs17;
				_dicTab[actiontype.ToString()] = ucContextActionMapping17;
				xtraTabPageEx17.ResumeLayout(performLayout: false);
			}
			break;
		case 1250000025:
			if (!_dicTab.ContainsKey(actiontype.ToString()))
			{
				XtraTabPageEx xtraTabPageEx15 = new XtraTabPageEx();
				xtraTabPageEx15.Size = new Size(926, 449);
				xtraTabPageEx15.SuspendLayout();
				UcContextActionMapping ucContextActionMapping15 = new UcContextActionMapping();
				ucContextActionMapping15.CellValueChanged = (Action)Delegate.Combine(ucContextActionMapping15.CellValueChanged, new Action(UcContextActionMappingValueChanged));
				ucContextActionMapping15.ActionFormId = ActionFormId;
				ucContextActionMapping15.ActionItemId = ActionItemId;
				ucContextActionMapping15.ObjectIdCaption = "Agree No.";
				ucContextActionMapping15.ShowDocFormVisible = true;
				ucContextActionMapping15.Dock = DockStyle.Fill;
				ucContextActionMapping15.ObjectIdValueChanged = (Action<string>)Delegate.Combine(ucContextActionMapping15.ObjectIdValueChanged, new Action<string>(ObjectIdValueChanged));
				ucContextActionMapping15.ShowFormValueChanged = (Action<string>)Delegate.Combine(ucContextActionMapping15.ShowFormValueChanged, new Action<string>(ShowFormValueChanged));
				xtraTabPageEx15.Controls.Add(ucContextActionMapping15);
				xtraTabPageEx15.Name = Enums.ContextActionActionTypeEx.Blanket.ToString();
				xtraTabPageEx15.FieldName = Enums.ContextActionActionTypeEx.Blanket.ToString();
				xtraTabPageEx15.Text = "Blanket";
				xtraTab_Main.TabPages.Add(xtraTabPageEx15);
				xtraTabPageEx15.Visible = true;
				List<ContextActionMappingTab> tabs15 = new List<ContextActionMappingTab>
				{
					new ContextActionMappingTab
					{
						Name = "General",
						Caption = "General",
						MappingType = "B",
						TargetItem = "H",
						HasSumaryType = true,
						ResourceName = "Blanket",
						IgnoreFields = new List<string>(),
						Structure = BlanketTableHelper.Blanket
					},
					new ContextActionMappingTab
					{
						Name = "Rows",
						Caption = "Rows",
						MappingType = "B",
						TargetItem = "R",
						HasSumaryType = true,
						ResourceName = "Blanket_Rows",
						IgnoreFields = new List<string>(),
						Structure = BlanketTableHelper.BlanketRows
					},
					new ContextActionMappingTab
					{
						Name = "Details",
						Caption = "Details",
						MappingType = "B",
						TargetItem = "D",
						HasSumaryType = true,
						ResourceName = "Blanket_Details",
						IgnoreFields = new List<string>(),
						Structure = BlanketTableHelper.BlanketDetails
					}
				};
				ucContextActionMapping15.Tabs = tabs15;
				_dicTab[actiontype.ToString()] = ucContextActionMapping15;
				xtraTabPageEx15.ResumeLayout(performLayout: false);
			}
			break;
		case 24:
			if (!_dicTab.ContainsKey(actiontype.ToString()))
			{
				XtraTabPageEx xtraTabPageEx13 = new XtraTabPageEx();
				xtraTabPageEx13.Size = new Size(926, 449);
				xtraTabPageEx13.SuspendLayout();
				UcContextActionMapping ucContextActionMapping13 = new UcContextActionMapping();
				ucContextActionMapping13.CellValueChanged = (Action)Delegate.Combine(ucContextActionMapping13.CellValueChanged, new Action(UcContextActionMappingValueChanged));
				ucContextActionMapping13.ActionFormId = ActionFormId;
				ucContextActionMapping13.ActionItemId = ActionItemId;
				ucContextActionMapping13.ObjectIdCaption = "Doc. No.";
				ucContextActionMapping13.ShowDocFormVisible = false;
				ucContextActionMapping13.ObjectTypeVisible = true;
				ucContextActionMapping13.DocEntryVisible = true;
				ucContextActionMapping13.LoadObjectType(new Dictionary<string, string>
				{
					{ "24", "Incoming Payment" },
					{ "46", "Outgoing Payment" },
					{ "2400", "Incoming Payment Draft" },
					{ "4600", "Outgoing Payment Draft" }
				});
				ucContextActionMapping13.Dock = DockStyle.Fill;
				ucContextActionMapping13.ObjectIdValueChanged = (Action<string>)Delegate.Combine(ucContextActionMapping13.ObjectIdValueChanged, new Action<string>(ObjectIdValueChanged));
				ucContextActionMapping13.ShowFormValueChanged = (Action<string>)Delegate.Combine(ucContextActionMapping13.ShowFormValueChanged, new Action<string>(ShowFormValueChanged));
				ucContextActionMapping13.ObjectTypeValueChanged = (Action<string>)Delegate.Combine(ucContextActionMapping13.ObjectTypeValueChanged, new Action<string>(ObjectTypeValueChanged));
				xtraTabPageEx13.Controls.Add(ucContextActionMapping13);
				xtraTabPageEx13.Name = Enums.ContextActionActionTypeEx.Payments.ToString();
				xtraTabPageEx13.FieldName = Enums.ContextActionActionTypeEx.Payments.ToString();
				xtraTabPageEx13.Text = "Payment";
				xtraTab_Main.TabPages.Add(xtraTabPageEx13);
				xtraTabPageEx13.Visible = true;
				List<ContextActionMappingTab> tabs13 = new List<ContextActionMappingTab>
				{
					new ContextActionMappingTab
					{
						Name = "General",
						Caption = "General",
						MappingType = "P",
						TargetItem = "H",
						HasSumaryType = false,
						ResourceName = "IncomingPayments",
						IgnoreFields = new List<string> { "DocEntry", "DocNum", "LogInstanc", "UserSign" },
						Structure = IncommingPaymentTableHelper.Payment
					},
					new ContextActionMappingTab
					{
						Name = "Invoices",
						Caption = "Invoices",
						MappingType = "P",
						TargetItem = "I",
						HasSumaryType = false,
						ResourceName = "IncomingPayments_Invoices",
						IgnoreFields = new List<string> { "ObjType", "LogInstanc", "UserSign" },
						Structure = IncommingPaymentTableHelper.PaymentInvoices
					},
					new ContextActionMappingTab
					{
						Name = "Accounts",
						Caption = "Accounts",
						MappingType = "P",
						TargetItem = "A",
						HasSumaryType = false,
						ResourceName = "IncomingPayments_Accounts",
						IgnoreFields = new List<string> { "DocEntry", "LineId", "LogInstanc", "UserSign" },
						Structure = IncommingPaymentTableHelper.PaymentAccounts
					},
					new ContextActionMappingTab
					{
						Name = "Checks",
						Caption = "Checks",
						MappingType = "P",
						TargetItem = "C",
						HasSumaryType = false,
						ResourceName = "IncomingPayments_Checks",
						IgnoreFields = new List<string> { "DocEntry", "LogInstanc", "UserSign" },
						Structure = IncommingPaymentTableHelper.PaymentChecks
					}
				};
				ucContextActionMapping13.Tabs = tabs13;
				_dicTab[actiontype.ToString()] = ucContextActionMapping13;
				xtraTabPageEx13.ResumeLayout(performLayout: false);
			}
			break;
		case 321:
		{
			if (_dicTab.ContainsKey(actiontype.ToString()))
			{
				break;
			}
			XtraTabPageEx xtraTabPageEx5 = new XtraTabPageEx();
			xtraTabPageEx5.Size = new Size(926, 449);
			xtraTabPageEx5.SuspendLayout();
			UcContextActionMapping ucContextActionMapping5 = new UcContextActionMapping();
			ucContextActionMapping5.CellValueChanged = (Action)Delegate.Combine(ucContextActionMapping5.CellValueChanged, new Action(UcContextActionMappingValueChanged));
			ucContextActionMapping5.ActionFormId = ActionFormId;
			ucContextActionMapping5.ActionItemId = ActionItemId;
			ucContextActionMapping5.ObjectIdCaption = "Doc. No.";
			ucContextActionMapping5.ShowDocFormVisible = true;
			ucContextActionMapping5.Dock = DockStyle.Fill;
			ucContextActionMapping5.ObjectIdValueChanged = (Action<string>)Delegate.Combine(ucContextActionMapping5.ObjectIdValueChanged, new Action<string>(ObjectIdValueChanged));
			ucContextActionMapping5.ShowFormValueChanged = (Action<string>)Delegate.Combine(ucContextActionMapping5.ShowFormValueChanged, new Action<string>(ShowFormValueChanged));
			xtraTabPageEx5.Controls.Add(ucContextActionMapping5);
			xtraTabPageEx5.Name = Enums.ContextActionActionTypeEx.InternalReconciliations.ToString();
			xtraTabPageEx5.FieldName = Enums.ContextActionActionTypeEx.InternalReconciliations.ToString();
			xtraTabPageEx5.Text = "Internal Reconciliations";
			xtraTab_Main.TabPages.Add(xtraTabPageEx5);
			xtraTabPageEx5.Visible = true;
			if (InternalReconciliationTableHelper.Header == null || InternalReconciliationTableHelper.Line == null)
			{
				DataSet structure = _httpClient.InternalReconciliationsClient.GetStructure();
				if (structure == null || structure.Tables.Count != 2)
				{
					WinCommons.Error(Enums.MessageCode.CannotLoadStructure, "");
					return;
				}
				InternalReconciliationTableHelper.Header = structure.Tables[0];
				InternalReconciliationTableHelper.Line = structure.Tables[1];
			}
			List<ContextActionMappingTab> tabs5 = new List<ContextActionMappingTab>
			{
				new ContextActionMappingTab
				{
					Name = "General",
					Caption = "Internal Reconciliations",
					MappingType = "R",
					TargetItem = "H",
					HasSumaryType = false,
					ResourceName = "InternalReconciliations",
					IgnoreFields = new List<string>(),
					Structure = InternalReconciliationTableHelper.Header
				},
				new ContextActionMappingTab
				{
					Name = "Lines",
					Caption = "Internal Reconciliation Lines",
					MappingType = "R",
					TargetItem = "I",
					HasSumaryType = false,
					ResourceName = "InternalReconciliationLines",
					IgnoreFields = new List<string>(),
					Structure = InternalReconciliationTableHelper.Line
				}
			};
			ucContextActionMapping5.Tabs = tabs5;
			_dicTab[actiontype.ToString()] = ucContextActionMapping5;
			xtraTabPageEx5.ResumeLayout(performLayout: false);
			break;
		}
		case 69:
			if (!_dicTab.ContainsKey(actiontype.ToString()))
			{
				XtraTabPageEx xtraTabPageEx3 = new XtraTabPageEx();
				xtraTabPageEx3.Size = new Size(926, 449);
				xtraTabPageEx3.SuspendLayout();
				UcContextActionMapping ucContextActionMapping3 = new UcContextActionMapping();
				ucContextActionMapping3.CellValueChanged = (Action)Delegate.Combine(ucContextActionMapping3.CellValueChanged, new Action(UcContextActionMappingValueChanged));
				ucContextActionMapping3.ActionFormId = ActionFormId;
				ucContextActionMapping3.ActionItemId = ActionItemId;
				ucContextActionMapping3.ObjectIdCaption = "Doc. No.";
				ucContextActionMapping3.ShowDocFormVisible = true;
				ucContextActionMapping3.Dock = DockStyle.Fill;
				ucContextActionMapping3.ObjectIdValueChanged = (Action<string>)Delegate.Combine(ucContextActionMapping3.ObjectIdValueChanged, new Action<string>(ObjectIdValueChanged));
				ucContextActionMapping3.ShowFormValueChanged = (Action<string>)Delegate.Combine(ucContextActionMapping3.ShowFormValueChanged, new Action<string>(ShowFormValueChanged));
				xtraTabPageEx3.Controls.Add(ucContextActionMapping3);
				xtraTabPageEx3.Name = Enums.ContextActionActionTypeEx.PurchaseLandedCost.ToString();
				xtraTabPageEx3.FieldName = Enums.ContextActionActionTypeEx.PurchaseLandedCost.ToString();
				xtraTabPageEx3.Text = "Purchase Landed Cost";
				xtraTab_Main.TabPages.Add(xtraTabPageEx3);
				xtraTabPageEx3.Visible = true;
				List<ContextActionMappingTab> tabs3 = new List<ContextActionMappingTab>
				{
					new ContextActionMappingTab
					{
						Name = "General",
						Caption = "Purchase Landed Cost",
						MappingType = "L",
						TargetItem = "H",
						HasSumaryType = false,
						ResourceName = "LandedCosts",
						IgnoreFields = new List<string>(),
						Structure = CtxTableHelper.PurchaseLandedCosts
					},
					new ContextActionMappingTab
					{
						Name = "LandedCosts_Rows",
						Caption = "Rows",
						MappingType = "R",
						TargetItem = "I",
						HasSumaryType = false,
						ResourceName = "LandedCosts_Rows",
						IgnoreFields = new List<string>(),
						Structure = CtxTableHelper.PurchaseLandedCosts_Rows
					},
					new ContextActionMappingTab
					{
						Name = "LandedCosts_Costs",
						Caption = "Costs",
						MappingType = "R",
						TargetItem = "C",
						HasSumaryType = false,
						ResourceName = "LandedCosts_Costs",
						IgnoreFields = new List<string>(),
						Structure = CtxTableHelper.PurchaseLandedCosts_Costs
					},
					new ContextActionMappingTab
					{
						Name = "LanedCosts_Customs",
						Caption = "Customs",
						MappingType = "R",
						TargetItem = "T",
						HasSumaryType = false,
						ResourceName = "LanedCosts_Customs",
						IgnoreFields = new List<string>(),
						Structure = CtxTableHelper.PurchaseLandedCosts_Customs
					}
				};
				ucContextActionMapping3.Tabs = tabs3;
				_dicTab[actiontype.ToString()] = ucContextActionMapping3;
				xtraTabPageEx3.ResumeLayout(performLayout: false);
			}
			break;
		case 64:
			if (!_dicTab.ContainsKey(actiontype.ToString()))
			{
				XtraTabPageEx xtraTabPageEx = new XtraTabPageEx();
				xtraTabPageEx.Size = new Size(926, 449);
				xtraTabPageEx.SuspendLayout();
				UcContextActionMapping ucContextActionMapping = new UcContextActionMapping();
				ucContextActionMapping.CellValueChanged = (Action)Delegate.Combine(ucContextActionMapping.CellValueChanged, new Action(UcContextActionMappingValueChanged));
				ucContextActionMapping.ActionFormId = ActionFormId;
				ucContextActionMapping.ActionItemId = ActionItemId;
				ucContextActionMapping.ShowDocFormVisible = false;
				ucContextActionMapping.ObjectTypeVisible = false;
				ucContextActionMapping.DocEntryVisible = false;
				ucContextActionMapping.Dock = DockStyle.Fill;
				ucContextActionMapping.ObjectIdValueChanged = (Action<string>)Delegate.Combine(ucContextActionMapping.ObjectIdValueChanged, new Action<string>(ObjectIdValueChanged));
				ucContextActionMapping.ShowFormValueChanged = (Action<string>)Delegate.Combine(ucContextActionMapping.ShowFormValueChanged, new Action<string>(ShowFormValueChanged));
				xtraTabPageEx.Controls.Add(ucContextActionMapping);
				xtraTabPageEx.Name = Enums.ContextActionActionTypeEx.Warehouses.ToString();
				xtraTabPageEx.FieldName = Enums.ContextActionActionTypeEx.Warehouses.ToString();
				xtraTabPageEx.Text = "Warehouse";
				xtraTab_Main.TabPages.Add(xtraTabPageEx);
				xtraTabPageEx.Visible = true;
				List<ContextActionMappingTab> tabs = new List<ContextActionMappingTab>
				{
					new ContextActionMappingTab
					{
						Name = "Warehouse",
						Caption = "Warehouse",
						MappingType = "G",
						TargetItem = "G",
						HasSumaryType = false,
						ResourceName = "Warehouses",
						IgnoreFields = new List<string>
						{
							"IntrnalKey", "Grp_Code", "BalInvntAc", "SaleCostAc", "TransferAc", "Locked", "DataSource", "UserSign", "RevenuesAc", "VarianceAc",
							"DecreasAc", "IncreasAc", "ReturnAc", "ExpensesAc", "EURevenuAc", "EUExpensAc", "FrRevenuAc", "FrExpensAc", "VatGroup", "ExmptIncom",
							"UseTax", "PriceDifAc", "ExchangeAc", "BalanceAcc", "PurchaseAc", "PAReturnAc", "PurchOfsAc", "ShpdGdsAct", "VatRevAct", "DecresGlAc",
							"IncresGlAc", "StokRvlAct", "StkOffsAct", "WipAcct", "WipVarAcct", "CostRvlAct", "CstOffsAct", "ExpClrAct", "ExpOfstAct", "objType",
							"logInstanc", "createDate", "userSign2", "updateDate", "ARCMAct", "ARCMFrnAct", "ARCMEUAct", "ARCMExpAct", "APCMAct", "APCMFrnAct",
							"APCMEUAct", "RevRetAct", "OwnerCode", "NegStckAct", "StkInTnAct", "AddrType", "PurBalAct", "Excisable", "WhICenAct", "WhOCenAct",
							"WhShipTo", "WipOffset", "StockOffst", "StorKeeper", "Shipper", "BinSeptor", "DftBinAbs", "DftBinEnfd", "AutoIssMtd", "RecItemsBy",
							"RecBinEnab", "RecvEmpBin", "RecvMaxQty", "AutoRecvMd", "RecvMaxWT", "RecvUpTo", "FreeChrgSA", "FreeChrgPU", "Address3", "External"
						},
						Structure = WarehousesTableHelper.Warehouse
					},
					new ContextActionMappingTab
					{
						Name = "Accounting",
						Caption = "Accounting",
						MappingType = "A",
						TargetItem = "A",
						HasSumaryType = false,
						ResourceName = "Warehouses",
						IgnoreFields = new List<string>
						{
							"WhsCode", "WhsName", "IntrnalKey", "Grp_Code", "Locked", "DataSource", "UserSign", "EURevenuAc", "EUExpensAc", "FrRevenuAc",
							"FrExpensAc", "VatGroup", "Street", "Block", "ZipCode", "City", "County", "Country", "State", "Location",
							"DropShip", "ExmptIncom", "UseTax", "PurchaseAc", "PAReturnAc", "PurchOfsAc", "FedTaxID", "Building", "VatRevAct", "Nettable",
							"StokRvlAct", "StkOffsAct", "CostRvlAct", "CstOffsAct", "ExpOfstAct", "objType", "logInstanc", "createDate", "userSign2", "updateDate",
							"ARCMFrnAct", "ARCMEUAct", "ARCMExpAct", "APCMFrnAct", "APCMEUAct", "RevRetAct", "BPLid", "OwnerCode", "AddrType", "StreetNo",
							"PurBalAct", "Excisable", "WhICenAct", "WhOCenAct", "WhShipTo", "StorKeeper", "Shipper", "BinActivat", "BinSeptor", "DftBinAbs",
							"DftBinEnfd", "AutoIssMtd", "ManageSnB", "RecItemsBy", "RecBinEnab", "GlblLocNum", "RecvEmpBin", "Inactive", "RecvMaxQty", "AutoRecvMd",
							"RecvMaxWT", "RecvUpTo", "FreeChrgSA", "FreeChrgPU", "TaxOffice", "Address2", "Address3", "External"
						},
						Structure = WarehousesTableHelper.Warehouse
					},
					new ContextActionMappingTab
					{
						Name = "BinLocation",
						Caption = "Bin Location",
						MappingType = "B",
						TargetItem = "B",
						HasSumaryType = false,
						ResourceName = "Warehouses",
						IgnoreFields = new List<string>
						{
							"DftBinAbs", "WhsCode", "WhsName", "IntrnalKey", "Grp_Code", "BalInvntAc", "SaleCostAc", "TransferAc", "Locked", "DataSource",
							"UserSign", "RevenuesAc", "VarianceAc", "DecreasAc", "IncreasAc", "ReturnAc", "ExpensesAc", "EURevenuAc", "EUExpensAc", "FrRevenuAc",
							"FrExpensAc", "VatGroup", "Street", "Block", "ZipCode", "City", "County", "Country", "State", "Location",
							"DropShip", "ExmptIncom", "UseTax", "PriceDifAc", "ExchangeAc", "BalanceAcc", "PurchaseAc", "PAReturnAc", "PurchOfsAc", "FedTaxID",
							"Building", "ShpdGdsAct", "VatRevAct", "DecresGlAc", "IncresGlAc", "Nettable", "StokRvlAct", "StkOffsAct", "WipAcct", "WipVarAcct",
							"CostRvlAct", "CstOffsAct", "ExpClrAct", "ExpOfstAct", "objType", "logInstanc", "createDate", "userSign2", "updateDate", "ARCMAct",
							"ARCMFrnAct", "ARCMEUAct", "ARCMExpAct", "APCMAct", "APCMFrnAct", "APCMEUAct", "RevRetAct", "BPLid", "OwnerCode", "NegStckAct",
							"StkInTnAct", "AddrType", "StreetNo", "PurBalAct", "Excisable", "WhICenAct", "WhOCenAct", "WhShipTo", "WipOffset", "StockOffst",
							"StorKeeper", "Shipper", "BinActivat", "GlblLocNum", "Inactive", "FreeChrgSA", "FreeChrgPU", "TaxOffice", "Address2", "Address3",
							"External"
						},
						Structure = WarehousesTableHelper.Warehouse
					}
				};
				ucContextActionMapping.Tabs = tabs;
				_dicTab[actiontype.ToString()] = ucContextActionMapping;
				xtraTabPageEx.ResumeLayout(performLayout: false);
			}
			break;
		}
		XtraTabPage xtraTabPage = xtraTab_Main.TabPages.FirstOrDefault((XtraTabPage t) => t.Name == Enums.ContextActionActionTypeEx.BusinessBusinessPartner.ToString());
		if (xtraTabPage != null)
		{
			xtraTabPage.PageVisible = actiontype == 2;
		}
		XtraTabPage xtraTabPage2 = xtraTab_Main.TabPages.FirstOrDefault((XtraTabPage t) => t.Name == Enums.ContextActionActionTypeEx.Items.ToString());
		if (xtraTabPage2 != null)
		{
			xtraTabPage2.PageVisible = actiontype == 4;
		}
		XtraTabPage xtraTabPage3 = xtraTab_Main.TabPages.FirstOrDefault((XtraTabPage t) => t.Name == Enums.ContextActionActionTypeEx.BillofMaterials.ToString());
		if (xtraTabPage3 != null)
		{
			xtraTabPage3.PageVisible = actiontype == 66;
		}
		XtraTabPage xtraTabPage4 = xtraTab_Main.TabPages.FirstOrDefault((XtraTabPage t) => t.Name == Enums.ContextActionActionTypeEx.JournalEntry.ToString());
		if (xtraTabPage4 != null)
		{
			xtraTabPage4.PageVisible = actiontype == 30;
		}
		XtraTabPage xtraTabPage5 = xtraTab_Main.TabPages.FirstOrDefault((XtraTabPage t) => t.Name == Enums.ContextActionActionTypeEx.JournalVouchers.ToString());
		if (xtraTabPage5 != null)
		{
			xtraTabPage5.PageVisible = actiontype == 27;
		}
		XtraTabPage xtraTabPage6 = xtraTab_Main.TabPages.FirstOrDefault((XtraTabPage t) => t.Name == Enums.ContextActionActionTypeEx.Task.ToString());
		if (xtraTabPage6 != null)
		{
			xtraTabPage6.PageVisible = actiontype == 1470000128;
		}
		XtraTabPage xtraTabPage7 = xtraTab_Main.TabPages.FirstOrDefault((XtraTabPage t) => t.Name == Enums.ContextActionActionTypeEx.Note.ToString());
		if (xtraTabPage7 != null)
		{
			xtraTabPage7.PageVisible = actiontype == 19000102;
		}
		XtraTabPage xtraTabPage8 = xtraTab_Main.TabPages.FirstOrDefault((XtraTabPage t) => t.Name == Enums.ContextActionActionTypeEx.Event.ToString());
		if (xtraTabPage8 != null)
		{
			xtraTabPage8.PageVisible = actiontype == 1470000129;
		}
		XtraTabPage xtraTabPage9 = xtraTab_Main.TabPages.FirstOrDefault((XtraTabPage t) => t.Name == Enums.ContextActionActionTypeEx.ProjectManagement.ToString());
		if (xtraTabPage9 != null)
		{
			xtraTabPage9.PageVisible = actiontype == 1470000132;
		}
		XtraTabPage xtraTabPage10 = xtraTab_Main.TabPages.FirstOrDefault((XtraTabPage t) => t.Name == Enums.ContextActionActionTypeEx.Opportunity.ToString());
		if (xtraTabPage10 != null)
		{
			xtraTabPage10.PageVisible = actiontype == 6;
		}
		XtraTabPage xtraTabPage11 = xtraTab_Main.TabPages.FirstOrDefault((XtraTabPage t) => t.Name == Enums.ContextActionActionTypeEx.TrackingIssue.ToString());
		if (xtraTabPage11 != null)
		{
			xtraTabPage11.PageVisible = actiontype == 1470000130;
		}
		XtraTabPage xtraTabPage12 = xtraTab_Main.TabPages.FirstOrDefault((XtraTabPage t) => t.Name == Enums.ContextActionActionTypeEx.ChartOfAccount.ToString());
		if (xtraTabPage12 != null)
		{
			xtraTabPage12.PageVisible = actiontype == 1;
		}
		XtraTabPage xtraTabPage13 = xtraTab_Main.TabPages.FirstOrDefault((XtraTabPage t) => t.Name == Enums.ContextActionActionTypeEx.DistributionRule.ToString());
		if (xtraTabPage13 != null)
		{
			xtraTabPage13.PageVisible = actiontype == 62;
		}
		XtraTabPage xtraTabPage14 = xtraTab_Main.TabPages.FirstOrDefault((XtraTabPage t) => t.Name == Enums.ContextActionActionTypeEx.VatGroup.ToString());
		if (xtraTabPage14 != null)
		{
			xtraTabPage14.PageVisible = actiontype == 5;
		}
		XtraTabPage xtraTabPage15 = xtraTab_Main.TabPages.FirstOrDefault((XtraTabPage t) => t.Name == Enums.ContextActionActionTypeEx.SalesPersons.ToString());
		if (xtraTabPage15 != null)
		{
			xtraTabPage15.PageVisible = actiontype == 53;
		}
		XtraTabPage xtraTabPage16 = xtraTab_Main.TabPages.FirstOrDefault((XtraTabPage t) => t.Name == Enums.ContextActionActionTypeEx.Blanket.ToString());
		if (xtraTabPage16 != null)
		{
			xtraTabPage16.PageVisible = actiontype == 1250000025;
		}
		XtraTabPage xtraTabPage17 = xtraTab_Main.TabPages.FirstOrDefault((XtraTabPage t) => t.Name == Enums.ContextActionActionTypeEx.Payments.ToString());
		if (xtraTabPage17 != null)
		{
			xtraTabPage17.PageVisible = actiontype == 24;
		}
		XtraTabPage xtraTabPage18 = xtraTab_Main.TabPages.FirstOrDefault((XtraTabPage t) => t.Name == Enums.ContextActionActionTypeEx.InternalReconciliations.ToString());
		if (xtraTabPage18 != null)
		{
			xtraTabPage18.PageVisible = actiontype == 321;
		}
		XtraTabPage xtraTabPage19 = xtraTab_Main.TabPages.FirstOrDefault((XtraTabPage t) => t.Name == Enums.ContextActionActionTypeEx.PurchaseLandedCost.ToString());
		if (xtraTabPage19 != null)
		{
			xtraTabPage19.PageVisible = actiontype == 69;
		}
		XtraTabPage xtraTabPage20 = xtraTab_Main.TabPages.FirstOrDefault((XtraTabPage t) => t.Name == Enums.ContextActionActionTypeEx.Warehouses.ToString());
		if (xtraTabPage20 != null)
		{
			xtraTabPage20.PageVisible = actiontype == 64;
		}
	}

	private void ObjectIdValueChanged(string s)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Busy && tv_Doc.FocusedNode != null && new int[15]
			{
				17, 202, 2, 4, 1470000128, 1470000129, 19000102, 1470000130, 1470000132, 6,
				66, 30, 53, 1250000025, 24
			}.Contains(Function.ParseInt(tv_Doc.FocusedNode.GetValue("ActionTypeEx"))))
			{
				tv_Doc.FocusedNode.SetValue("ObjectId", s);
				tv_Doc.DataSourceTable().AcceptChanges();
				Control_EditValueChanged(null, null);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void UcContextActionMappingValueChanged()
	{
		try
		{
			Control_EditValueChanged(null, null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ShowFormValueChanged(string s)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Busy && tv_Doc.FocusedNode != null && new int[10] { 17, 202, 2, 4, 1470000128, 1470000129, 19000102, 1470000130, 1470000132, 6 }.Contains(Function.ParseInt(tv_Doc.FocusedNode.GetValue("ActionTypeEx"))))
			{
				tv_Doc.FocusedNode.SetValue("ShowDocForm", s);
				tv_Doc.DataSourceTable().AcceptChanges();
				Control_EditValueChanged(null, null);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ObjectTypeValueChanged(string s)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Busy)
			{
				tv_Doc.FocusedNode.SetValue("ObjType", s);
				tv_Doc.DataSourceTable().AcceptChanges();
				Control_EditValueChanged(null, null);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void VisibleSourceTypePage(string sourceType)
	{
		if (sourceType == "D")
		{
			lbl_DataFrom.Visible = true;
			look_DataFrom.Visible = true;
			bte_DataFrom.Visible = true;
			grd_Parameters.Visible = true;
			look_ParameterType.Visible = true;
			lbl_TemporaryName.Visible = true;
			txt_TemporaryTableName.Visible = true;
			lbl_FilterData.Location = new Point(txt_TemporaryTableName.Location.X + 240, lbl_DataFrom.Location.Y);
			bte_FilterData.Location = new Point(lbl_FilterData.Location.X + 80, txt_TemporaryTableName.Location.Y);
		}
		else
		{
			lbl_FilterData.Location = lbl_DataFrom.Location;
			bte_FilterData.Location = look_DataFrom.Location;
			lbl_DataFrom.Visible = false;
			look_DataFrom.Visible = false;
			bte_DataFrom.Visible = false;
			grd_Parameters.Visible = false;
			look_ParameterType.Visible = false;
			lbl_ParameterType.Visible = false;
			lbl_TemporaryName.Visible = false;
			txt_TemporaryTableName.Visible = false;
		}
	}

	private void rep_Caption_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			int fatherId = Function.ParseInt(tv_Doc.FocusedNode.GetValue("AbsEntry"));
			NewDoc(fatherId);
			tv_Doc.FocusedNode.Expand();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void look_DataFrom_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Busy)
			{
				if (tv_Doc.FocusedNode != null)
				{
					tv_Doc.FocusedNode.SetValue("DataFrom", Function.ToString(look_DataFrom.EditValue));
					tv_Doc.DataSourceTable().AcceptChanges();
				}
				string dataFrom = Function.ToString(look_DataFrom.EditValue);
				VisibleDataString(dataFrom);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void VisibleDataString(string dataFrom)
	{
		if (Function.ToString(tv_Doc.FocusedNode.GetValue("SourceType")) == "D")
		{
			if (!(dataFrom == "Q"))
			{
				if (dataFrom == "T")
				{
					memo_DataString.Visible = true;
					bte_DataFrom.Visible = false;
				}
			}
			else
			{
				memo_DataString.Visible = false;
				bte_DataFrom.Visible = true;
			}
		}
		else
		{
			memo_DataString.Visible = false;
			bte_DataFrom.Visible = false;
			look_DataFrom.Visible = false;
		}
	}

	private void bte_DataFrom_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Busy)
			{
				if (tv_Doc.FocusedNode != null && Function.ToString(look_DataFrom.EditValue) == "Q")
				{
					tv_Doc.FocusedNode.SetValue("DataString", Function.ToString(bte_DataFrom.EditValue));
					tv_Doc.DataSourceTable().AcceptChanges();
				}
				if (string.IsNullOrEmpty(Function.ToString(bte_DataFrom.EditValue)) && !Function.ToString(tv_Doc.FocusedNode.GetValue("DataParamType")).Equals("T"))
				{
					grd_Parameters.DataSourceTable.Rows.Clear();
					grd_Parameters.DataSourceTable.AcceptChanges();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void bte_FilterData_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Busy && tv_Doc.FocusedNode != null)
			{
				tv_Doc.FocusedNode.SetValue("FilterString", Function.ToString(bte_FilterData.EditValue));
				tv_Doc.DataSourceTable().AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void look_ReportForm_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Busy)
			{
				if (tv_Doc.FocusedNode != null)
				{
					tv_Doc.FocusedNode.SetValue("RptFrom", Function.ToString(look_ReportFrom.EditValue));
					tv_Doc.DataSourceTable().AcceptChanges();
				}
				char? c = Function.ParseChar(look_ReportFrom.EditValue);
				if (c.HasValue)
				{
					Enums.ContextActionReportFrom value = (Enums.ContextActionReportFrom)c.Value;
					VisibleReportString(value);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void VisibleReportString(Enums.ContextActionReportFrom rptFrom)
	{
		switch (rptFrom)
		{
		case Enums.ContextActionReportFrom.DefinedQuery:
			memo_RptString.Visible = false;
			bte_RptString.Visible = true;
			bte_RptString.Properties.DisplayMember = "QName";
			bte_RptString.Properties.ValueMember = "IntrnalKey";
			bte_RptString.Properties.TypeCFL = Enums.TypeCFL.Queries;
			bte_RptString.Properties.RefreshDataSource();
			break;
		case Enums.ContextActionReportFrom.QueryText:
			memo_RptString.Visible = true;
			bte_RptString.Visible = false;
			break;
		case Enums.ContextActionReportFrom.UserDefinedReport:
			memo_RptString.Visible = false;
			bte_RptString.Visible = true;
			bte_RptString.Properties.DisplayMember = "Name";
			bte_RptString.Properties.ValueMember = "Code";
			bte_RptString.Properties.TypeCFL = Enums.TypeCFL.UserDefinedReport;
			bte_RptString.Properties.RefreshDataSource();
			break;
		case (Enums.ContextActionReportFrom)82:
		case (Enums.ContextActionReportFrom)83:
			break;
		}
	}

	private void buttonEdit2_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Busy)
			{
				if (tv_Doc.FocusedNode != null && new string[2] { "Q", "U" }.Contains(Function.ToString(look_ReportFrom.EditValue)))
				{
					tv_Doc.FocusedNode.SetValue("RptString", Function.ToString(bte_RptString.EditValue));
					tv_Doc.DataSourceTable().AcceptChanges();
				}
				if (string.IsNullOrEmpty(Function.ToString(bte_RptString.EditValue)))
				{
					grd_Report.DataSourceTable.Rows.Clear();
					grd_Report.DataSourceTable.AcceptChanges();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void look_ObjectType_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Busy && tv_Doc.FocusedNode != null)
			{
				tv_Doc.FocusedNode.SetValue("ObjType", Function.ToString(look_ObjectType.EditValue));
				tv_Doc.DataSourceTable().AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void look_UdoCode_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Busy && tv_Doc.FocusedNode != null)
			{
				string text = Function.ToString(look_UdoCode.EditValue);
				string udoName = Function.ToString(look_UdoCode.Text);
				tv_Doc.FocusedNode.SetValue("UdoCode", text);
				tv_Doc.DataSourceTable().AcceptChanges();
				await LoadTabUdo(text, udoName);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadTabUdo(string udocode, string udoName)
	{
		if (string.IsNullOrEmpty(udocode) || Function.ParseInt(tv_Doc.FocusedNode.GetValue("ActionTypeEx")) != 206000)
		{
			return;
		}
		tabControl_Udo.TabPages.Clear();
		XtraTabPage page = new XtraTabPage
		{
			Name = udocode,
			Text = udoName + " - Header"
		};
		tabControl_Udo.TabPages.Add(page);
		await AddTabPage(page, udocode);
		DataTable dtChild = await _httpClient.Udo.GetUdo1ByCode(udocode);
		for (int i = 0; i < dtChild.Rows.Count; i++)
		{
			page = new XtraTabPage();
			string tableName = "@" + Function.ToString(dtChild.Rows[i]["TableName"]);
			page.Name = tableName;
			page.Text = Function.ToString(dtChild.Rows[i]["Descr"]);
			if (tabControl_Udo.TabPages.FirstOrDefault((XtraTabPage t) => t.Name == tableName) == null)
			{
				tabControl_Udo.TabPages.Add(page);
				await AddTabPage(page, udocode, Function.ToString(dtChild.Rows[i]["TableName"]));
			}
		}
	}

	private async Task AddTabPage(XtraTabPage page, string udocode, string sonname = "")
	{
		int absEntry = Function.ParseInt(tv_Doc.FocusedNode.GetValue("AbsEntry"));
		string targetItem = ((sonname == "") ? udocode : sonname);
		GridControlEx grdudo = new GridControlEx
		{
			Dock = DockStyle.Fill
		};
		GridViewEx gvUdo = new GridViewEx(grdudo);
		gvUdo.CellValueChanged += Control_EditValueChanged;
		gvUdo.BeginInit();
		gvUdo.BeginUpdate();
		DataTable dtTargetField = await _httpClient.ContextAction.GetTargetField(udocode, sonname);
		RepositoryItemLookUpEdit repositoryItemLookUpEdit = new RepositoryItemLookUpEdit
		{
			AutoHeight = false,
			NullText = ""
		};
		repositoryItemLookUpEdit.SetDataSource(dtTargetField, "Name", "Code", emptyRow: true);
		RepositoryItemLookUpEdit repositoryItemLookUpEdit2 = new RepositoryItemLookUpEdit
		{
			AutoHeight = false,
			NullText = ""
		};
		string text = Function.ToString(tv_Doc.FocusedNode.GetValue("SourceType"));
		if (!(text == "U"))
		{
			if (text == "D")
			{
				repositoryItemLookUpEdit2.SetDataSource(DatabaseSourceField, "Name", "Code", emptyRow: true);
			}
		}
		else
		{
			repositoryItemLookUpEdit2.SetDataSource(UiSourceField, "Name", "Code", emptyRow: true);
		}
		DataTable dataTable = new DataTable();
		dataTable.Columns.Add("Code", typeof(string));
		dataTable.Columns.Add("Name", typeof(string));
		DataRow dataRow = dataTable.NewRow();
		dataRow[0] = "";
		dataRow[1] = "";
		dataTable.Rows.Add(dataRow);
		dataRow = dataTable.NewRow();
		dataRow[0] = "S";
		dataRow[1] = "SUM";
		dataTable.Rows.Add(dataRow);
		dataRow = dataTable.NewRow();
		dataRow[0] = "A";
		dataRow[1] = "AVG";
		dataTable.Rows.Add(dataRow);
		dataRow = dataTable.NewRow();
		dataRow[0] = "C";
		dataRow[1] = "COUNT";
		dataTable.Rows.Add(dataRow);
		dataTable.AcceptChanges();
		RepositoryItemLookUpEdit repositoryItemLookUpEdit3 = new RepositoryItemLookUpEdit
		{
			AutoHeight = false,
			NullText = ""
		};
		repositoryItemLookUpEdit3.SetDataSource(dataTable, "Name", "Code", emptyRow: true);
		grdudo.RepositoryItems.AddRange(new RepositoryItem[3] { repositoryItemLookUpEdit2, repositoryItemLookUpEdit, repositoryItemLookUpEdit3 });
		GridColumnEx gridColumnEx = new GridColumnEx();
		gridColumnEx.Caption = "Target Field Name";
		gridColumnEx.FieldName = "TargetField";
		gridColumnEx.Visible = true;
		gridColumnEx.VisibleIndex = 0;
		gridColumnEx.Width = 121;
		gridColumnEx.OptionsColumn.AllowEdit = false;
		gridColumnEx.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		gridColumnEx.ColumnEdit = repositoryItemLookUpEdit;
		GridColumnEx column = gridColumnEx;
		gvUdo.Columns.Add(column);
		column = new GridColumnEx
		{
			Caption = "Source Field Name",
			FieldName = "SourceField",
			Visible = true,
			VisibleIndex = 1,
			Width = 121,
			ColumnEdit = repositoryItemLookUpEdit2
		};
		gvUdo.Columns.Add(column);
		column = new GridColumnEx
		{
			Caption = "Default Value",
			FieldName = "DfltValue",
			Visible = true,
			VisibleIndex = 2,
			Width = 175
		};
		gvUdo.Columns.Add(column);
		column = new GridColumnEx
		{
			Caption = "Summary Type",
			FieldName = "SumType",
			Visible = true,
			VisibleIndex = 3,
			ColumnEdit = repositoryItemLookUpEdit3,
			Width = 175
		};
		gvUdo.Columns.Add(column);
		gvUdo.EndUpdate();
		gvUdo.EndInit();
		grdudo.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { gvUdo });
		grdudo.MainView = gvUdo;
		page.Controls.Add(grdudo);
		DataRow[] array = _dtMapping.Select(string.Format("AbsEntry={0} AND MappingType='{1}' AND TargetItem='{2}'", absEntry, "O", targetItem));
		DataTable ret = _dtMapping.Clone();
		DataTable dataTable2 = _dtMapping.Clone();
		if (array.Length != 0)
		{
			dataTable2 = array.CopyToDataTable();
		}
		if (!dataTable2.IsNotNull())
		{
			dataTable2 = await _httpClient.ContextAction.GetMappingUdo(udocode, ActionFormId, ActionItemId, absEntry, sonname);
		}
		DataRow[] array2 = (from t0 in dtTargetField.AsEnumerable()
			join t1 in dataTable2.AsEnumerable() on Function.ToString(t0["Code"]) equals Function.ToString(t1["TargetField"]) into grp
			from item in grp.DefaultIfEmpty()
			select ret.LoadDataRow(new object[9]
			{
				absEntry,
				ActionFormId,
				ActionItemId,
				"O",
				targetItem,
				Function.ToString(t0["Code"]),
				(item != null) ? Function.ToString(item["SourceField"]) : "",
				(item != null) ? Function.ToString(item["DfltValue"]) : null,
				(item != null) ? Function.ToString(item["SumType"]) : ""
			}, fAcceptChanges: false)).ToArray();
		grdudo.DataSource = ((array2.Length != 0) ? array2.CopyToDataTable() : dataTable2);
		grdudo.RefreshDataSource();
	}

	private void InitChangeDataEvent()
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Busy)
			{
				return;
			}
			foreach (Control control in Controls)
			{
				SetChangedEvent(control);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void SetChangedEvent(Control control)
	{
		try
		{
			if (control is BaseEdit)
			{
				((BaseEdit)control).EditValueChanged += Control_EditValueChanged;
				return;
			}
			if (control is GridControlEx)
			{
				DevExpress.XtraGrid.Views.Base.BaseView defaultView = ((GridControlEx)control).DefaultView;
				if (defaultView is GridViewEx)
				{
					((GridViewEx)defaultView).CellValueChanged += Control_EditValueChanged;
				}
				return;
			}
			if (control is TreeList)
			{
				((TreeList)control).CellValueChanged += Control_EditValueChanged;
				return;
			}
			foreach (Control control2 in control.Controls)
			{
				SetChangedEvent(control2);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void Control_EditValueChanged(object sender, EventArgs e)
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
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void GetDataNodes(DataTable dt, TreeListNodes listNodes)
	{
		TreeListNode[] array = listNodes.OrderBy((TreeListNode n) => listNodes.TreeList.GetNodeIndex(n)).ToArray();
		for (int num = 0; num < array.Length; num++)
		{
			dt.ImportRow(tv_Doc.GetDataRow(listNodes[num].Id));
			if (array[num].HasChildren)
			{
				GetDataNodes(dt, array[num].Nodes);
			}
		}
	}

	private DataSet GetDataset()
	{
		DataTable dataTable = tv_Doc.DataSourceTable().Clone();
		GetDataNodes(dataTable, tv_Doc.Nodes);
		dataTable.AcceptChanges();
		InsertDataParams("S", grd_Parameters.DataSourceTable.Copy());
		InsertDataParams("R", grd_Report.DataSourceTable.Copy());
		InsertDataParams("Q", grd_DoQuery.DataSourceTable.Copy());
		DataTable dataTable2 = _dtParams.Copy();
		InsertDataMapping("D", "H", grd_Header.DataSourceTable.Copy());
		InsertDataMapping("D", "I", grd_Items.DataSourceTable.Copy());
		InsertDataMapping("D", "B", grd_Batch.DataSourceTable.Copy());
		InsertDataMapping("W", "H", grd_ProductionHeader.DataSourceTable.Copy());
		InsertDataMapping("W", "I", grd_ProductionItem.DataSourceTable.Copy());
		InsertDataMapping("W", "B", grd_ProductionBatch.DataSourceTable.Copy());
		InsertDataMapping("F", "H", grd_ForecastHeader.DataSourceTable.Copy());
		InsertDataMapping("F", "I", grd_ForecastItems.DataSourceTable.Copy());
		InsertDataMapping("V", "H", grd_Approval.DataSourceTable.Copy());
		InsertDataMapping("9", "Q", ucHttpRequest.MappingQuery);
		InsertDataMapping("9", "H", ucHttpRequest.MappingHeder);
		InsertDataMapping("9", "A", ucHttpRequest.MappingAuth);
		InsertDataMapping("P", "H", grd_PhysicalTable.DataSourceTable.Copy());
		InsertDataHttpRequestConfig(ucHttpRequest.MappingBody);
		foreach (KeyValuePair<string, UcContextActionMapping> item2 in _dicTab)
		{
			item2.Value.InsertDataMapping(_dtMapping);
		}
		switch (Function.ParseInt(tv_Doc.FocusedNode.GetValue("ActionTypeEx")))
		{
		case 206000:
			foreach (XtraTabPage tabPage in tabControl_Udo.TabPages)
			{
				foreach (Control control in tabPage.Controls)
				{
					if (control is GridControlEx { DataSourceTable: var dataSourceTable })
					{
						if (dataSourceTable.IsNotNull())
						{
							string item = Function.ToString(dataSourceTable.Rows[0]["TargetItem"]);
							InsertDataMapping("O", item, dataSourceTable.Copy());
						}
						break;
					}
				}
			}
			break;
		case 810015:
			InsertDataAttach(grd_AttachFiles.DataSourceTable.Copy());
			InsertDataRptParams(grd_RptParam.DataSourceTable.Copy());
			break;
		case 7000039:
			InsertDataRptParams(grd_PrintLayout.DataSourceTable.Copy());
			break;
		}
		DataTable dataTable3 = _dtMapping.Copy();
		DataTable dataTable4 = _dtAttachment.Copy();
		DataTable dataTable5 = _dtAttachParams.Copy();
		DataTable dataTable6 = _dtHttpRequestConfig.Copy();
		DataSet dataSet = new DataSet();
		dataSet.Tables.AddRange(new DataTable[6] { dataTable, dataTable2, dataTable3, dataTable4, dataTable5, dataTable6 });
		return dataSet;
	}

	private async void btn_OK_Click(object sender, EventArgs e)
	{
		Enums.FormViewModeType currMode = FormMode;
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			if (btn_OK.Text == Resources.ButtonOK || FormMode == Enums.FormViewModeType.View)
			{
				Close();
				return;
			}
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				FormMode = currMode;
				return;
			}
			WinCommons._OpenProcessing();
			DataSet dataset = GetDataset();
			HttpResult httpResult = await _httpClient.ContextAction.Save(ActionFormId, ActionItemId, dataset);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				FormMode = currMode;
				return;
			}
			WinCommons.Success(Enums.MessageCode.Success, "");
			BaseForm?.ListContextAction.Clear();
			BaseForm?.ListSyntaxContextAction.Clear();
			FormMode = Enums.FormViewModeType.View;
			WinFuntions.UpdateReportFile("Action");
			if (CloseFormAfterSave)
			{
				DialogResult = DialogResult.Yes;
			}
			return;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			WinCommons._CloseProcessing();
		}
		FormMode = currMode;
	}

	private void bte_DataFrom_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
	}

	private void bte_FilterData_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			string text = Function.ToString(tv_Doc.FocusedNode.GetValue("SourceType"));
			int key = Function.ParseInt(tv_Doc.FocusedNode.GetValue("AbsEntry"));
			if (text == "I")
			{
				DataTable dataTable = new DataTable();
				OpenFileDialog openFileDialog = new OpenFileDialog();
				openFileDialog.Filter = $"Excel Files|*.xls;*.xlsx;*.xlsm";
				openFileDialog.FilterIndex = 1;
				openFileDialog.Multiselect = false;
				if (openFileDialog.ShowDialog() == DialogResult.OK)
				{
					string fileName = openFileDialog.FileName;
					DataSet dataSet = WinFuntions.ReadExcelToDataTable(fileName, Path.GetExtension(fileName).ToLower());
					if (dataSet.IsNotNull())
					{
						dataTable = dataSet.Tables[0];
					}
				}
				if (dataTable.IsNotNull())
				{
					DataSet dataSet2 = new DataSet();
					dataSet2.Tables.Add(dataTable.Copy());
					frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.UserDefinedData, false, "", "", Enums.ItemObjectType.NoItemType, null, "");
					frm_CFL2.DataSource = dataSet2;
					frm_CFL2.ShowDialog(this);
					InsertDataFilter(Function.ParseInt(tv_Doc.FocusedNode.GetValue("AbsEntry")), dataTable.Copy());
				}
			}
			else
			{
				if (text == "U")
				{
					grd_Filter.DataSource = UiDataSource;
				}
				else if (_dicFilter != null && _dicFilter.ContainsKey(key))
				{
					grd_Filter.DataSource = _dicFilter[key];
				}
				grd_Filter.RefreshDataSource();
				gv_Filter.PopulateColumns();
				gv_Filter.ActiveFilterString = Function.ToString(bte_FilterData.EditValue);
				if (gv_Filter.Columns.Count > 0)
				{
					gv_Filter.ShowFilterEditor(gv_Filter.Columns[0]);
				}
				string dataSetWhere = CriteriaToWhereClauseHelper.GetDataSetWhere(gv_Filter.ActiveFilterCriteria);
				bte_FilterData.EditValue = dataSetWhere;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void memo_DataString_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Busy)
			{
				string text = Function.ToString(memo_DataString.EditValue);
				if (tv_Doc.FocusedNode != null && Function.ToString(look_DataFrom.EditValue) == "T")
				{
					tv_Doc.FocusedNode.SetValue("DataString", text);
					tv_Doc.DataSourceTable().AcceptChanges();
				}
				if (string.IsNullOrEmpty(text))
				{
					grd_Parameters.DataSourceTable.Rows.Clear();
					grd_Parameters.DataSourceTable.AcceptChanges();
				}
				await LoadParameters(text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void memo_RptString_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Busy)
			{
				string text = Function.ToString(memo_RptString.EditValue);
				if (tv_Doc.FocusedNode != null && Function.ToString(look_ReportFrom.EditValue) == "T")
				{
					tv_Doc.FocusedNode.SetValue("RptString", text);
					tv_Doc.DataSourceTable().AcceptChanges();
				}
				if (string.IsNullOrEmpty(text))
				{
					grd_Parameters.DataSourceTable.Rows.Clear();
					grd_Parameters.DataSourceTable.AcceptChanges();
				}
				await LoadRptParameters(text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void removeToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (tv_Doc.FocusedNode != null)
			{
				int num = Function.ParseInt(tv_Doc.FocusedNode.GetValue("AbsEntry"));
				tv_Doc.DeleteSelectedNodes();
				tv_Doc.DataSourceTable.AcceptChanges();
				DataRow[] array = _dtParams.Select("AbsEntry=" + num);
				for (int i = 0; i < array.Length; i++)
				{
					_dtParams.Rows.Remove(array[i]);
				}
				_dtParams.AcceptChanges();
				DataRow[] array2 = _dtMapping.Select("AbsEntry=" + num);
				for (int j = 0; j < array2.Length; j++)
				{
					_dtMapping.Rows.Remove(array2[j]);
				}
				_dtMapping.AcceptChanges();
				grd_Parameters.DataSourceTable.Rows.Clear();
				grd_Parameters.RefreshDataSource();
				grd_DoQuery.DataSourceTable.Rows.Clear();
				grd_DoQuery.RefreshDataSource();
				grd_Report.DataSourceTable.Rows.Clear();
				grd_Report.RefreshDataSource();
				grd_Header.DataSourceTable.Rows.Clear();
				grd_Header.RefreshDataSource();
				grd_Items.DataSourceTable.Rows.Clear();
				grd_Items.RefreshDataSource();
				grd_Batch.DataSourceTable.Rows.Clear();
				grd_Batch.RefreshDataSource();
				grd_Approval.DataSourceTable.Rows.Clear();
				grd_Approval.RefreshDataSource();
				grd_PhysicalTable.DataSourceTable.Rows.Clear();
				grd_PhysicalTable.RefreshDataSource();
				Control_EditValueChanged(null, null);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void tv_Doc_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (_listNotEditableField.Contains(tv_Doc.FocusedColumn.FieldName))
			{
				e.Cancel = true;
			}
			else if (tv_Doc.FocusedColumn.FieldName == "DataRange")
			{
				if (ActionFormId == ApzObjectTypes.oDashboardDetail.ToString())
				{
					e.Cancel = true;
				}
			}
			else if (tv_Doc.FocusedColumn.FieldName == "Method")
			{
				int value = Function.ParseInt(tv_Doc.FocusedNode.GetValue("ActionTypeEx"));
				if (!new int[6] { 17, 202, 2, 4, 30, 24 }.Contains(value))
				{
					e.Cancel = true;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void tv_Doc_NodeCellStyle(object sender, GetCustomNodeCellStyleEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == "DataRange" && ActionFormId == ApzObjectTypes.oDashboardDetail.ToString())
			{
				e.Appearance.BackColor = GridColor.GridReadOnly;
			}
			if (e.Column.FieldName == "Method" && tv_Doc.FocusedNode != null)
			{
				int value = Function.ParseInt(tv_Doc.FocusedNode.GetValue("ActionTypeEx"));
				if (!new int[5] { 17, 202, 2, 4, 30 }.Contains(value))
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

	private void chk_ShowDocForm_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Busy && tv_Doc.FocusedNode != null && new int[2] { 17, 202 }.Contains(Function.ParseInt(tv_Doc.FocusedNode.GetValue("ActionTypeEx"))))
			{
				tv_Doc.FocusedNode.SetValue("ShowDocForm", Function.ToString(chk_ShowDocForm.EditValue));
				tv_Doc.DataSourceTable().AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void chk_ShowUdoForm_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Busy && tv_Doc.FocusedNode != null && Function.ParseInt(tv_Doc.FocusedNode.GetValue("ActionTypeEx")) == 206000)
			{
				tv_Doc.FocusedNode.SetValue("ShowDocForm", Function.ToString(chk_ShowUdoForm.EditValue));
				tv_Doc.DataSourceTable().AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void drop_ExportSetting_Click(object sender, EventArgs e)
	{
		try
		{
			ExportDataSet(GetDataset());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private static void ExportDataSet(DataSet ds)
	{
		if (ds.IsNotNull())
		{
			ds.DataSetName = "ContextAction";
			string text = FileIO.PathSaveFile("XML", "xml");
			if (!string.IsNullOrEmpty(text))
			{
				using (StreamWriter writer = new StreamWriter(text, append: false))
				{
					ds.WriteXml(writer);
					WinCommons.Success(Enums.MessageCode.Success, "");
				}
			}
		}
		else
		{
			WinCommons.Error(Enums.MessageCode.DataIsEmpty, "");
		}
	}

	private void btn_Import_Click(object sender, EventArgs e)
	{
		try
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Multiselect = false;
			openFileDialog.Filter = "XML Files(*.xml;*.txt)|*.xml;*.txt";
			if (openFileDialog.ShowDialog() != DialogResult.OK)
			{
				return;
			}
			DataSet dataSet = new DataSet();
			dataSet.ReadXml(openFileDialog.FileName);
			if (dataSet.IsNotNull())
			{
				if (dataSet.Tables.Contains("APZ_OCTA"))
				{
					DataTable dataTable = dataSet.Tables["APZ_OCTA"];
					DataTable dtpr = new DataTable();
					DataTable dtMap = new DataTable();
					if (dataSet.Tables.Contains("APZ_CTA1"))
					{
						dtpr = dataSet.Tables["APZ_CTA1"];
					}
					if (dataSet.Tables.Contains("APZ_CTA2"))
					{
						dtMap = dataSet.Tables["APZ_CTA2"];
					}
					if (!dataTable.IsNotNull())
					{
						return;
					}
					DataTable dataSourceTable = tv_Doc.DataSourceTable;
					DataRow[] array = (from t in dataSourceTable.AsEnumerable()
						where string.IsNullOrEmpty(Function.ToString(t["Caption"]))
						select t).ToArray();
					foreach (DataRow row in array)
					{
						dataSourceTable.Rows.Remove(row);
					}
					dataSourceTable.AcceptChanges();
					int minFatherId = dataTable.AsEnumerable().Min((DataRow t) => Function.ParseInt(t["FatherId"]));
					DataRow[] array2 = (from t in dataTable.AsEnumerable()
						where Function.ParseInt(t["FatherId"]) == minFatherId && !string.IsNullOrEmpty(Function.ToString(t["Caption"]))
						select t).ToArray();
					if (array2.Length != 0)
					{
						for (int num2 = 0; num2 < array2.Length; num2++)
						{
							ImportData(dataSourceTable, dataTable, dtpr, dtMap, array2[num2], -1);
						}
						dataSourceTable.AcceptChanges();
						tv_Doc.RefreshDataSource();
						NewDoc();
						Control_EditValueChanged(null, null);
					}
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.CouldNotReadDataFromThisFileTagAPZ_OCTANotAvailable, "");
				}
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.CouldNotReadDataFormThisFile, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ImportData(DataTable source, DataTable dtDoc, DataTable dtpr, DataTable dtMap, DataRow data, int newFatherId)
	{
		DataRow dataRow = source.NewRow();
		int num = Function.ParseInt(dataRow["AbsEntry"]);
		int oldEntry = Function.ParseInt(data["AbsEntry"]);
		dataRow["AbsEntry"] = num;
		dataRow["FormId"] = ActionFormId;
		dataRow["ItemId"] = ActionItemId;
		dataRow["FatherId"] = newFatherId;
		dataRow["Caption"] = (data.Table.ExistsColumn("Caption") ? Function.ToString(data["Caption"]) : "");
		dataRow["ActionTypeEx"] = (data.Table.ExistsColumn("ActionTypeEx") ? Function.ToString(data["ActionTypeEx"]) : "");
		dataRow["SourceType"] = (data.Table.ExistsColumn("SourceType") ? Function.ToString(data["SourceType"]) : "");
		dataRow["DataRange"] = (data.Table.ExistsColumn("DataRange") ? Function.ToString(data["DataRange"]) : "");
		dataRow["DataFrom"] = (data.Table.ExistsColumn("DataFrom") ? Function.ToString(data["DataFrom"]) : "");
		dataRow["DataString"] = (data.Table.ExistsColumn("DataString") ? Function.ToString(data["DataString"]) : "");
		dataRow["FilterString"] = (data.Table.ExistsColumn("FilterString") ? Function.ToString(data["FilterString"]) : "");
		dataRow["ObjType"] = (data.Table.ExistsColumn("ObjType") ? Function.ToString(data["ObjType"]) : "");
		dataRow["RptFrom"] = (data.Table.ExistsColumn("RptFrom") ? Function.ToString(data["RptFrom"]) : "");
		dataRow["RptString"] = (data.Table.ExistsColumn("RptString") ? Function.ToString(data["RptString"]) : "");
		dataRow["UdoCode"] = (data.Table.ExistsColumn("UdoCode") ? Function.ToString(data["UdoCode"]) : "");
		dataRow["ShowDocForm"] = (data.Table.ExistsColumn("ShowDocForm") ? Function.ToString(data["ShowDocForm"]) : "");
		dataRow["DataParamType"] = (data.Table.ExistsColumn("DataParamType") ? Function.ToString(data["DataParamType"]) : "");
		source.Rows.Add(dataRow);
		if (dtpr.IsNotNull())
		{
			DataRow[] array = (from t in dtpr.AsEnumerable()
				where Function.ParseInt(t["AbsEntry"]) == oldEntry
				select t).ToArray();
			if (array.Length != 0)
			{
				for (int num2 = 0; num2 < array.Length; num2++)
				{
					DataRow dataRow2 = _dtParams.NewRow();
					dataRow2["AbsEntry"] = num;
					dataRow2["FormId"] = ActionFormId;
					dataRow2["ItemId"] = ActionItemId;
					dataRow2["ParamType"] = (dtpr.ExistsColumn("ParamType") ? Function.ToString(array[num2]["ParamType"]) : "");
					dataRow2["ParamName"] = (dtpr.ExistsColumn("ParamName") ? Function.ToString(array[num2]["ParamName"]) : "");
					dataRow2["Label"] = (dtpr.ExistsColumn("Label") ? Function.ToString(array[num2]["Label"]) : "");
					dataRow2["DataType"] = (dtpr.ExistsColumn("DataType") ? Function.ToString(array[num2]["DataType"]) : "");
					dataRow2["SubType"] = (dtpr.ExistsColumn("SubType") ? Function.ToString(array[num2]["SubType"]) : "");
					dataRow2["DefaultValue"] = (dtpr.ExistsColumn("DefaultValue") ? Function.ToString(array[num2]["DefaultValue"]) : "");
					dataRow2["Active"] = (dtpr.ExistsColumn("Active") ? Function.ToString(array[num2]["Active"]) : "");
					dataRow2["SourceField"] = (dtpr.ExistsColumn("SourceField") ? Function.ToString(array[num2]["SourceField"]) : "");
					_dtParams.Rows.Add(dataRow2);
				}
				_dtParams.AcceptChanges();
			}
		}
		if (dtMap.IsNotNull())
		{
			DataRow[] array2 = (from t in dtMap.AsEnumerable()
				where Function.ParseInt(t["AbsEntry"]) == oldEntry
				select t).ToArray();
			if (array2.Length != 0)
			{
				for (int num3 = 0; num3 < array2.Length; num3++)
				{
					DataRow dataRow3 = _dtMapping.NewRow();
					dataRow3["AbsEntry"] = num;
					dataRow3["FormId"] = ActionFormId;
					dataRow3["ItemId"] = ActionItemId;
					dataRow3["MappingType"] = (dtMap.ExistsColumn("MappingType") ? Function.ToString(array2[num3]["MappingType"]) : "");
					dataRow3["TargetItem"] = (dtMap.ExistsColumn("TargetItem") ? Function.ToString(array2[num3]["TargetItem"]) : "");
					dataRow3["TargetField"] = (dtMap.ExistsColumn("TargetField") ? Function.ToString(array2[num3]["TargetField"]) : "");
					dataRow3["SourceField"] = (dtMap.ExistsColumn("SourceField") ? Function.ToString(array2[num3]["SourceField"]) : "");
					dataRow3["DfltValue"] = (dtMap.ExistsColumn("DfltValue") ? Function.ToString(array2[num3]["DfltValue"]) : "");
					dataRow3["SumType"] = (dtMap.ExistsColumn("SumType") ? Function.ToString(array2[num3]["SumType"]) : "");
					_dtMapping.Rows.Add(dataRow3);
				}
				_dtMapping.AcceptChanges();
			}
		}
		foreach (DataRow item in from t in dtDoc.AsEnumerable()
			where Function.ParseInt(t["FatherId"]) == oldEntry
			select t)
		{
			ImportData(source, dtDoc, dtpr, dtMap, item, num);
		}
	}

	private async void btn_EditDesign_Click(object sender, EventArgs e)
	{
		try
		{
			string caption = Function.ToString(tv_Doc.FocusedNode.GetValue("Caption"));
			DataTable dashboardSource = GetDashboarDataSource();
			int dashboardId = Function.ParseInt(tv_Doc.FocusedNode.GetValue("DashboardId"));
			DevExpress.DashboardCommon.Dashboard dashboard = await this.LoadDashboardFromSource(dashboardId, dashboardSource);
			frm_DashboardDesigner frm;
			if (dashboard != null)
			{
				dashboard.Title.Text = caption;
				frm = new frm_DashboardDesigner(dashboard);
			}
			else
			{
				DataSet dataSet = new DataSet();
				dataSet.Tables.Add(dashboardSource.Copy());
				frm = new frm_DashboardDesigner(dataSet, caption);
			}
			frm_DashboardDesigner frm_DashboardDesigner2 = frm;
			frm_DashboardDesigner2.SaveEditedDashboard = (Action<DevExpress.DashboardCommon.Dashboard>)Delegate.Combine(frm_DashboardDesigner2.SaveEditedDashboard, (Action<DevExpress.DashboardCommon.Dashboard>)(async (DevExpress.DashboardCommon.Dashboard obj) =>
			{
				await SaveEditedDashboard(obj, frm);
			}));
			frm.ShowDialog();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private DataTable GetDashboarDataSource()
	{
		DataTable result = new DataTable(Function.ToString(tv_Doc.FocusedNode.GetValue("Caption")));
		string text = Function.ToString(tv_Doc.FocusedNode.GetValue("SourceType"));
		int key = Function.ParseInt(tv_Doc.FocusedNode.GetValue("AbsEntry"));
		if (text == "D")
		{
			if (_dicFilter.ContainsKey(key))
			{
				result = _dicFilter[key];
			}
		}
		else
		{
			result = UiDataSource;
		}
		return result;
	}

	private async Task SaveEditedDashboard(DevExpress.DashboardCommon.Dashboard dashboard, frm_DashboardDesigner frm)
	{
		try
		{
			Enums.FormViewModeType currentMode = FormMode;
			FormMode = Enums.FormViewModeType.Busy;
			try
			{
				DataTable dataTable = _dtDashboardStruct.Clone().Clone();
				DataRow dataRow = dataTable.NewRow();
				dataRow["Name"] = Function.ToString(tv_Doc.FocusedNode.GetValue("Caption"));
				XDocument xDocument = dashboard.SaveToXDocument();
				dataRow["Layout"] = xDocument.ToString();
				dataRow["Id"] = Function.ParseInt(tv_Doc.FocusedNode.GetValue("DashboardId"));
				dataRow["FormId"] = ActionFormId;
				dataRow["ItemId"] = ActionItemId;
				dataTable.Rows.Add(dataRow);
				HttpResult httpResult = await _httpClient.DashboardDetail.Save(dataTable);
				int id = Function.ParseInt(httpResult.Content);
				if (string.IsNullOrEmpty(httpResult.Message))
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
					SetDashboardView(dashboard, id, currentMode);
					frm.Close();
					return;
				}
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", ex);
				WinCommons.ErrorLineNumber(ex);
			}
			FormMode = currentMode;
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", ex2);
			WinCommons.ErrorLineNumber(ex2);
		}
	}

	private void SetDashboardView(DevExpress.DashboardCommon.Dashboard dashboard, int id, Enums.FormViewModeType currentMode)
	{
		if (Function.ParseInt(tv_Doc.FocusedNode.GetValue("DashboardId")) != id)
		{
			tv_Doc.FocusedNode.SetValue("DashboardId", id);
			tv_Doc.DataSourceTable.AcceptChanges();
			if (FormMode != Enums.FormViewModeType.Add)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		else
		{
			FormMode = currentMode;
		}
		ucDashboardViewer1.SetDashboard(dashboard, id);
		int key = Function.ParseInt(tv_Doc.FocusedNode.GetValue("AbsEntry"));
		_dicDashboard[key] = dashboard;
		btn_EditDesign.Text = "Edit Design";
	}

	private void btn_ChooseDashboard_Click(object sender, EventArgs e)
	{
		try
		{
			frm_WidgetAddItem frm_WidgetAddItem2 = new frm_WidgetAddItem();
			frm_WidgetAddItem2.SelectItem = (Action<DataRow[]>)Delegate.Combine(frm_WidgetAddItem2.SelectItem, (Action<DataRow[]>)(async (DataRow[] obj) =>
			{
				await ChooseDashboard(obj);
			}));
			frm_WidgetAddItem2.ShowDialog(this);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task ChooseDashboard(IReadOnlyList<DataRow> dataRows)
	{
		try
		{
			if (dataRows != null && dataRows.Count > 0)
			{
				int dashboardId = Function.ParseInt(dataRows[0]["Id"]);
				if (dashboardId > 0)
				{
					SetDashboardView(await this.LoadDashboardFromSource(dashboardId, GetDashboarDataSource()), dashboardId, FormMode);
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, "Could not load dashboard " + dashboardId);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void txt_AddressFrom_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.UserDefinedData, false, "", "", Enums.ItemObjectType.NoItemType, null, "");
			DataSet dataSet = new DataSet();
			dataSet.Tables.Add(UiSourceField.Copy());
			frm_CFL2.DataSource = dataSet;
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem != null)
			{
				txt_AddressFrom.EditValue = string.Format("$[{0}]", Function.ToString(frm_CFL2.ChooseItem["Code"]));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void txt_AddressTo_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		ChooseAddressFromField(txt_AddressTo);
	}

	private void ChooseAddressFromField(ButtonEditEx btn, string stringjoin = ";")
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.UserDefinedData, true, "", "", Enums.ItemObjectType.NoItemType, null, "");
			DataSet dataSet = new DataSet();
			if (rep_AttParamSourceField.DataSource is DataTable dataTable)
			{
				dataSet.Tables.Add(dataTable.Copy());
			}
			frm_CFL2.DataSource = dataSet;
			frm_CFL2.ShowDialog(this);
			List<DataRow> chooseItems = frm_CFL2.ChooseItems;
			if (chooseItems != null && chooseItems.Count > 0)
			{
				btn.EditValue = (string.IsNullOrEmpty(Function.ToString(btn.EditValue)) ? "" : (Function.ToString(btn.EditValue) + stringjoin)) + string.Join(stringjoin, frm_CFL2.ChooseItems.Select((DataRow t) => string.Format("$[{0}]", Function.ToString(t["Code"]))));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void AddressCc_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		ChooseAddressFromField(txt_AddressCc);
	}

	private void txt_AddressBcc_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		ChooseAddressFromField(txt_AddressBcc);
	}

	private void txt_AddressFrom_EditValueChanged(object sender, EventArgs e)
	{
		SetAddressToSource("AddressFrom", txt_AddressFrom);
	}

	private void SetAddressToSource(string fieldName, ButtonEditEx btn)
	{
		Enums.FormViewModeType formMode = FormMode;
		try
		{
			if (FormMode == Enums.FormViewModeType.Busy)
			{
				return;
			}
			FormMode = Enums.FormViewModeType.Busy;
			if (tv_Doc.FocusedNode != null && new int[1] { 810015 }.Contains(Function.ParseInt(tv_Doc.FocusedNode.GetValue("ActionTypeEx"))))
			{
				tv_Doc.FocusedNode.SetValue(fieldName, Function.ToString(btn.EditValue));
				tv_Doc.DataSourceTable().AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		FormMode = formMode;
	}

	private void txt_AddressTo_EditValueChanged(object sender, EventArgs e)
	{
		SetAddressToSource("AddressTo", txt_AddressTo);
	}

	private void txt_AddressCc_EditValueChanged(object sender, EventArgs e)
	{
		SetAddressToSource("AddressCc", txt_AddressCc);
	}

	private void txt_AddressBcc_EditValueChanged(object sender, EventArgs e)
	{
		SetAddressToSource("AddressBcc", txt_AddressBcc);
	}

	private void txt_Subject_EditValueChanged(object sender, EventArgs e)
	{
		SetAddressToSource("Subject", txt_Subject);
	}

	private void txt_Subject_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		ChooseAddressFromField(txt_Subject, " ");
	}

	private async void rep_Path_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (openFileDialog1.ShowDialog() != DialogResult.OK)
			{
				return;
			}
			string[] fileNames = openFileDialog1.FileNames;
			if (fileNames == null || fileNames.Length == 0)
			{
				return;
			}
			int sourceIndex = gv_AttachFiles.GetDataSourceRowIndex(gv_AttachFiles.FocusedRowHandle);
			DataTable dt = grd_AttachFiles.DataSourceTable;
			if (dt.IsNotNull())
			{
				dt.Rows.RemoveAt(sourceIndex);
				dt.AcceptChanges();
			}
			dt.Columns["LineId"].AutoIncrement = true;
			dt.Columns["LineId"].AutoIncrementSeed = (dt.IsNotNull() ? (dt.AsEnumerable().Max((DataRow t) => Function.ParseLong(t["LineId"])) + 1) : 1);
			dt.Columns["LineId"].AutoIncrementStep = 1L;
			for (int i = 0; i < openFileDialog1.FileNames.Length; i++)
			{
				string sourcePath = openFileDialog1.FileNames[i];
				if (!string.IsNullOrEmpty(sourcePath))
				{
					string text = await _httpClient.FileNetworkManagerClient.UpLoadFile("\\Action\\", true, sourcePath);
					if (!string.IsNullOrEmpty(text))
					{
						WinCommons.Error(Enums.MessageCode.None, text);
						continue;
					}
				}
				DataRow dataRow = dt.NewRow();
				dataRow["FilePath"] = sourcePath;
				dataRow["FileExt"] = Path.GetExtension(sourcePath);
				dataRow["FileAlias"] = Path.GetFileNameWithoutExtension(sourcePath);
				dataRow["AbsEntry"] = Function.ParseInt(tv_Doc.FocusedNode.GetValue("AbsEntry"));
				dataRow["FormId"] = ActionFormId;
				dataRow["ItemId"] = ActionItemId;
				dt.Rows.InsertAt(dataRow, sourceIndex);
			}
			if (dt.AsEnumerable().FirstOrDefault((DataRow t) => string.IsNullOrEmpty(Function.ToString(t["FilePath"]))) == null)
			{
				dt.Rows.Add(dt.NewRow());
			}
			dt.AcceptChanges();
			grd_AttachFiles.DataSource = dt;
			grd_AttachFiles.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_AttachFiles_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
	{
		try
		{
			if (!(Function.ToString(gv_AttachFiles.GetFocusedRowCellValue("FileType")) == "R"))
			{
				return;
			}
			ReportDocument reportDocument = new ReportDocument();
			string text = Function.ToString(gv_AttachFiles.GetFocusedRowCellValue("FilePath"));
			if (!FileIO.FileExist(text))
			{
				WinCommons.Error(Enums.MessageCode.ReportFileNotFoundWithPath, "", Path.GetFileName(text));
			}
			reportDocument.Load(text);
			reportDocument.Refresh();
			DataTable dataTable = _dtAttachParams.Clone();
			foreach (object parameterField in reportDocument.DataDefinition.ParameterFields)
			{
				if (parameterField is ParameterFieldDefinition)
				{
					string parameterFieldName = ((ParameterFieldDefinition)parameterField).ParameterFieldName;
					DataRow dataRow = dataTable.NewRow();
					dataRow["AbsEntry"] = Function.ParseInt(tv_Doc.FocusedNode.GetValue("AbsEntry"));
					dataRow["FormId"] = ActionFormId;
					dataRow["ItemId"] = ActionItemId;
					dataRow["LineId"] = Function.ParseInt(gv_AttachFiles.GetFocusedRowCellValue("LineId"));
					dataRow["ParamName"] = parameterFieldName;
					dataRow["ParamType"] = "-";
					dataTable.Rows.Add(dataRow);
				}
			}
			dataTable.AcceptChanges();
			grd_RptParam.DataSource = dataTable;
			grd_RptParam.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_AttachFiles_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			InsertDataRptParams(grd_RptParam.DataSourceTable.Copy());
			if (tv_Doc.FocusedNode != null)
			{
				int num = Function.ParseInt(tv_Doc.FocusedNode.GetValue("AbsEntry"));
				int num2 = Function.ParseInt(gv_AttachFiles.GetFocusedRowCellValue("LineId"));
				DataRow[] array = _dtAttachParams.Select($"AbsEntry={num} AND LineId='{num2}'");
				if (array.Length != 0)
				{
					grd_RptParam.DataSource = array.CopyToDataTable();
				}
				else
				{
					grd_RptParam.DataSource = _dtAttachParams.Clone();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void InsertDataRptParams(DataTable dt)
	{
		if (_dtAttachParams == null || _dtAttachParams.Columns.Count == 0)
		{
			WinCommons.Error(Enums.MessageCode.CouldNotGetDataReportParamater, "");
		}
		else
		{
			if (dt == null || dt.Rows.Count == 0)
			{
				return;
			}
			DataRow[] array = _dtAttachParams.Select("AbsEntry=" + Function.ParseInt(dt.Rows[0]["AbsEntry"]) + " AND LineId='" + Function.ParseInt(dt.Rows[0]["LineId"]) + "'");
			for (int i = 0; i < array.Length; i++)
			{
				_dtAttachParams.Rows.Remove(array[i]);
			}
			for (int j = 0; j < dt.Rows.Count; j++)
			{
				if (!string.IsNullOrEmpty(Function.ToString(dt.Rows[j]["ParamName"])))
				{
					DataRow dataRow = _dtAttachParams.NewRow();
					dataRow.ItemArray = dt.Rows[j].ItemArray;
					_dtAttachParams.Rows.Add(dataRow);
				}
			}
			_dtAttachParams.AcceptChanges();
		}
	}

	private void InsertDataHttpRequestConfig(DataTable dt)
	{
		if (_dtHttpRequestConfig == null || _dtHttpRequestConfig.Columns.Count == 0)
		{
			WinCommons.Error(Enums.MessageCode.CouldNotGetDataHttpRequestConfig, "");
		}
		else if (dt != null && dt.Rows.Count != 0)
		{
			DataRow[] array = _dtHttpRequestConfig.Select("AbsEntry=" + Function.ParseInt(dt.Rows[0]["AbsEntry"]));
			for (int i = 0; i < array.Length; i++)
			{
				_dtHttpRequestConfig.Rows.Remove(array[i]);
			}
			for (int j = 0; j < dt.Rows.Count; j++)
			{
				DataRow dataRow = _dtHttpRequestConfig.NewRow();
				dataRow.ItemArray = dt.Rows[j].ItemArray;
				_dtHttpRequestConfig.Rows.Add(dataRow);
			}
			_dtHttpRequestConfig.AcceptChanges();
		}
	}

	private void btn_EditEmailContent_Click(object sender, EventArgs e)
	{
		try
		{
			frm_ReportWriter frm_ReportWriter2 = new frm_ReportWriter();
			DataSet dataSet = new DataSet("Dataset");
			dataSet.Tables.Add(GetDashboarDataSource().Copy());
			frm_ReportWriter2.Source = dataSet;
			if (frm_ReportWriter2.ShowDialog() == DialogResult.Yes)
			{
				snapControl1.HtmlText = frm_ReportWriter2.HtmlText;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void snapControl1_ContentChanged(object sender, EventArgs e)
	{
		try
		{
			Enums.FormViewModeType formMode = FormMode;
			try
			{
				if (FormMode == Enums.FormViewModeType.Busy)
				{
					return;
				}
				FormMode = Enums.FormViewModeType.Busy;
				if (tv_Doc.FocusedNode != null && new int[1] { 810015 }.Contains(Function.ParseInt(tv_Doc.FocusedNode.GetValue("ActionTypeEx"))))
				{
					tv_Doc.FocusedNode.SetValue("BodyText", Function.ToString(snapControl1.HtmlText));
					tv_Doc.DataSourceTable().AcceptChanges();
				}
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", ex);
				WinCommons.ErrorLineNumber(ex);
			}
			FormMode = formMode;
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", ex2);
			WinCommons.ErrorLineNumber(ex2);
		}
	}

	private void rep_MsgSuccess_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			try
			{
				frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.UserDefinedData, true, "", "", Enums.ItemObjectType.NoItemType, null, "");
				DataSet dataSet = new DataSet();
				if (rep_AttParamSourceField.DataSource is DataTable dataTable)
				{
					dataSet.Tables.Add(dataTable.Copy());
				}
				frm_CFL2.DataSource = dataSet;
				frm_CFL2.ShowDialog(this);
				object value = tv_Doc.FocusedNode.GetValue("MsgSuccess");
				List<DataRow> chooseItems = frm_CFL2.ChooseItems;
				if (chooseItems != null && chooseItems.Count > 0)
				{
					tv_Doc.FocusedNode.SetValue("MsgSuccess", (string.IsNullOrEmpty(Function.ToString(value)) ? "" : (Function.ToString(value) + " ")) + string.Join(" ", frm_CFL2.ChooseItems.Select((DataRow t) => string.Format("$[{0}]", Function.ToString(t["Code"])))));
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

	private void chk_ShowProductionForm_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Busy && tv_Doc.FocusedNode != null && new int[2] { 17, 202 }.Contains(Function.ParseInt(tv_Doc.FocusedNode.GetValue("ActionTypeEx"))))
			{
				tv_Doc.FocusedNode.SetValue("ShowDocForm", Function.ToString(chk_ShowProductionForm.EditValue));
				tv_Doc.DataSourceTable().AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void look_EmailGroupBy_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			Enums.FormViewModeType formMode = FormMode;
			try
			{
				if (FormMode == Enums.FormViewModeType.Busy)
				{
					return;
				}
				FormMode = Enums.FormViewModeType.Busy;
				if (tv_Doc.FocusedNode != null && new int[1] { 810015 }.Contains(Function.ParseInt(tv_Doc.FocusedNode.GetValue("ActionTypeEx"))))
				{
					tv_Doc.FocusedNode.SetValue("EmailGroupBy", Function.ToString(look_EmailGroupBy.EditValue));
					tv_Doc.DataSourceTable().AcceptChanges();
				}
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", ex);
				WinCommons.ErrorLineNumber(ex);
			}
			FormMode = formMode;
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", ex2);
			WinCommons.ErrorLineNumber(ex2);
		}
	}

	private void txt_EmailGroupByFields_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			Enums.FormViewModeType formMode = FormMode;
			try
			{
				if (FormMode == Enums.FormViewModeType.Busy)
				{
					return;
				}
				FormMode = Enums.FormViewModeType.Busy;
				if (tv_Doc.FocusedNode != null && new int[1] { 810015 }.Contains(Function.ParseInt(tv_Doc.FocusedNode.GetValue("ActionTypeEx"))))
				{
					tv_Doc.FocusedNode.SetValue("EmailGroupByFields", Function.ToString(txt_EmailGroupByFields.EditValue));
					tv_Doc.DataSourceTable().AcceptChanges();
				}
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", ex);
				WinCommons.ErrorLineNumber(ex);
			}
			FormMode = formMode;
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", ex2);
			WinCommons.ErrorLineNumber(ex2);
		}
	}

	private void txt_EmailGroupByFields_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.UserDefinedData, true, "", "", Enums.ItemObjectType.NoItemType, null, "");
			DataSet dataSet = new DataSet();
			if (rep_AttParamSourceField.DataSource is DataTable dataTable)
			{
				dataSet.Tables.Add(dataTable.Copy());
			}
			frm_CFL2.DataSource = dataSet;
			frm_CFL2.ShowDialog(this);
			List<DataRow> chooseItems = frm_CFL2.ChooseItems;
			if (chooseItems != null && chooseItems.Count > 0)
			{
				txt_EmailGroupByFields.EditValue = (string.IsNullOrEmpty(Function.ToString(txt_EmailGroupByFields.EditValue)) ? "" : (Function.ToString(txt_EmailGroupByFields.EditValue) + ";")) + string.Join(";", frm_CFL2.ChooseItems.Select((DataRow t) => string.Format("{0}", Function.ToString(t["Code"]))));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void chk_ForecastShowWindow_CheckedChanged(object sender, EventArgs e)
	{
		Enums.FormViewModeType formMode = FormMode;
		try
		{
			if (FormMode == Enums.FormViewModeType.Busy)
			{
				return;
			}
			FormMode = Enums.FormViewModeType.Busy;
			if (tv_Doc.FocusedNode != null && new int[1] { 8000005 }.Contains(Function.ParseInt(tv_Doc.FocusedNode.GetValue("ActionTypeEx"))))
			{
				tv_Doc.FocusedNode.SetValue("ShowDocForm", Function.ToString(chk_ForecastShowWindow.EditValue));
				tv_Doc.DataSourceTable().AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		FormMode = formMode;
	}

	private void gv_ForecastHeader_CustomRowCellEditForEditing(object sender, CustomRowCellEditEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == "DfltValue" && Function.ToString(gv_ForecastHeader.GetRowCellValue(e.RowHandle, "TargetField")) == "FormView")
			{
				e.RepositoryItem = _repForecastView;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
	}

	private void gv_ForecastHeader_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
	{
		try
		{
			if (!(e.Column.FieldName == "DfltValue") || e.IsForGroupRow || !(Function.ToString(grd_ForecastHeader.DataSourceTable.Rows[e.ListSourceRowIndex]["TargetField"]) == "FormView"))
			{
				return;
			}
			string text = Function.ToString(e.Value);
			if (!(text == "W"))
			{
				if (text == "M")
				{
					e.DisplayText = "Monthly";
				}
				else
				{
					e.DisplayText = "Daily";
				}
			}
			else
			{
				e.DisplayText = "Weekly";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
	}

	private void look_DocEntry_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Busy && tv_Doc.FocusedNode != null && new int[2] { 17, 202 }.Contains(Function.ParseInt(tv_Doc.FocusedNode.GetValue("ActionTypeEx"))))
			{
				tv_Doc.FocusedNode.SetValue("ObjectId", Function.ToString(look_DocEntry.EditValue));
				tv_Doc.DataSourceTable().AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void look_ProductionEntry_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Busy && tv_Doc.FocusedNode != null && new int[2] { 17, 202 }.Contains(Function.ParseInt(tv_Doc.FocusedNode.GetValue("ActionTypeEx"))))
			{
				tv_Doc.FocusedNode.SetValue("ObjectId", Function.ToString(look_ProductionEntry.EditValue));
				tv_Doc.DataSourceTable().AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void look_ParameterType_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Busy)
			{
				if (tv_Doc.FocusedNode != null)
				{
					tv_Doc.FocusedNode.SetValue("DataParamType", Function.ToString(look_ParameterType.EditValue));
					tv_Doc.DataSourceTable().AcceptChanges();
				}
				SetVisibleSourceParameter();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void SetVisibleSourceParameter()
	{
		try
		{
			if (Function.ToString(look_ParameterType.EditValue) == "T")
			{
				removeRowToolStripMenuItem.Visible = true;
				gv_Parameters.Columns["ParamName"].Visible = false;
				gv_Parameters.Columns["Label"].Visible = false;
				gv_Parameters.Columns["DataType"].Visible = false;
				gv_Parameters.Columns["SourceField"].Visible = true;
				gv_Parameters.Columns["SubType"].Visible = false;
				gv_Parameters.Columns["DefaultValue"].Visible = true;
				gv_Parameters.Columns["Active"].Visible = false;
				DataTable dataSourceTable = grd_Parameters.DataSourceTable;
				if (dataSourceTable.AsEnumerable().FirstOrDefault((DataRow t) => string.IsNullOrEmpty(Function.ToString(t["SourceField"])) && string.IsNullOrEmpty(Function.ToString(t["DefaultValue"]))) == null)
				{
					DataRow dataRow = dataSourceTable.NewRow();
					dataRow["AbsEntry"] = Function.ParseInt(tv_Doc.FocusedNode.GetValue("AbsEntry"));
					dataRow["FormId"] = ActionFormId;
					dataRow["ItemId"] = ActionItemId;
					dataRow["ParamType"] = "S";
					dataRow["ParamName"] = ((!dataSourceTable.IsNotNull()) ? 1 : (dataSourceTable.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["ParamName"])) + 1));
					dataSourceTable.Rows.Add(dataRow);
					dataSourceTable.AcceptChanges();
				}
			}
			else
			{
				removeRowToolStripMenuItem.Visible = false;
				gv_Parameters.Columns["ParamName"].Visible = true;
				gv_Parameters.Columns["Label"].Visible = true;
				gv_Parameters.Columns["DataType"].Visible = true;
				gv_Parameters.Columns["SourceField"].Visible = true;
				gv_Parameters.Columns["SubType"].Visible = true;
				gv_Parameters.Columns["DefaultValue"].Visible = true;
				gv_Parameters.Columns["Active"].Visible = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Parameters_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
	{
		try
		{
			if (!(Function.ToString(look_ParameterType.EditValue) == "T") || !new string[2] { "SourceField", "DefaultValue" }.Contains(e.Column.FieldName))
			{
				return;
			}
			DataTable dataSourceTable = grd_Parameters.DataSourceTable;
			if (dataSourceTable.AsEnumerable().FirstOrDefault((DataRow t) => string.IsNullOrEmpty(Function.ToString(t["SourceField"])) && string.IsNullOrEmpty(Function.ToString(t["DefaultValue"]))) == null)
			{
				DataRow dataRow = dataSourceTable.NewRow();
				dataRow["AbsEntry"] = Function.ParseInt(tv_Doc.FocusedNode.GetValue("AbsEntry"));
				dataRow["FormId"] = ActionFormId;
				dataRow["ItemId"] = ActionItemId;
				dataRow["ParamType"] = "S";
				dataRow["ParamName"] = dataSourceTable.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["ParamName"])) + 1;
				dataSourceTable.Rows.Add(dataRow);
				dataSourceTable.AcceptChanges();
			}
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
			if (Function.ToString(look_ParameterType.EditValue) == "T")
			{
				gv_Parameters.DeleteSelectedRows();
				grd_Parameters.DataSourceTable.AcceptChanges();
			}
			else
			{
				WinCommons.Warning(Enums.MessageCode.CannotRemoveThisParamater, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void look_QueryFrom_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			try
			{
				if (FormMode != Enums.FormViewModeType.Busy)
				{
					if (tv_Doc.FocusedNode != null)
					{
						tv_Doc.FocusedNode.SetValue("QueryFrom", Function.ToString(look_QueryFrom.EditValue));
						tv_Doc.DataSourceTable().AcceptChanges();
					}
					string queryFrom = Function.ToString(look_QueryFrom.EditValue);
					VisibleQueryString(queryFrom);
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

	private void VisibleQueryString(string queryFrom)
	{
		if (!(queryFrom == "Q"))
		{
			if (queryFrom == "T")
			{
				memo_QueryString.Visible = true;
				txt_QueryString.Visible = false;
			}
		}
		else
		{
			memo_QueryString.Visible = false;
			txt_QueryString.Visible = true;
		}
	}

	private void txt_QueryString_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Busy)
			{
				if (tv_Doc.FocusedNode != null && Function.ToString(look_DataFrom.EditValue) == "Q")
				{
					tv_Doc.FocusedNode.SetValue("QueryString", Function.ToString(txt_QueryString.EditValue));
					tv_Doc.DataSourceTable().AcceptChanges();
				}
				if (string.IsNullOrEmpty(Function.ToString(txt_QueryString.EditValue)))
				{
					grd_DoQuery.DataSourceTable.Rows.Clear();
					grd_DoQuery.DataSourceTable.AcceptChanges();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void memo_QueryString_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Busy)
			{
				string text = Function.ToString(memo_QueryString.EditValue);
				if (tv_Doc.FocusedNode != null && Function.ToString(look_QueryFrom.EditValue) == "T")
				{
					tv_Doc.FocusedNode.SetValue("QueryString", text);
					tv_Doc.DataSourceTable().AcceptChanges();
				}
				if (string.IsNullOrEmpty(text))
				{
					grd_DoQuery.DataSourceTable.Rows.Clear();
					grd_DoQuery.DataSourceTable.AcceptChanges();
				}
				await LoadQueryParameters(text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void bte_PrintLayoutPath_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (openFileDialog2.ShowDialog() != DialogResult.OK || string.IsNullOrEmpty(openFileDialog2.FileName))
			{
				return;
			}
			string sourcePath = openFileDialog2.FileName;
			if (!string.IsNullOrEmpty(sourcePath))
			{
				string text = await _httpClient.FileNetworkManagerClient.UpLoadFile("\\Action\\", true, sourcePath);
				if (!string.IsNullOrEmpty(text))
				{
					WinCommons.Error(Enums.MessageCode.None, text);
					return;
				}
			}
			bte_PrintLayoutPath.EditValue = sourcePath;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void bte_PrintLayoutPath_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			string text = Function.ToString(bte_PrintLayoutPath.EditValue);
			if (FormMode == Enums.FormViewModeType.Busy)
			{
				return;
			}
			if (tv_Doc.FocusedNode != null)
			{
				tv_Doc.FocusedNode.SetValue("FilePath", Path.GetFileName(text));
				tv_Doc.DataSourceTable().AcceptChanges();
			}
			ReportDocument reportDocument = new ReportDocument();
			string text2 = Function.ToString(text);
			if (!FileIO.FileExist(text2))
			{
				WinCommons.Error(Enums.MessageCode.ReportFileNotFoundWithPath, "", Path.GetFileName(text2));
			}
			reportDocument.Load(text2);
			reportDocument.Refresh();
			DataTable dataTable = _dtAttachParams.Clone();
			foreach (object parameterField in reportDocument.DataDefinition.ParameterFields)
			{
				if (parameterField is ParameterFieldDefinition)
				{
					string parameterFieldName = ((ParameterFieldDefinition)parameterField).ParameterFieldName;
					DataRow dataRow = dataTable.NewRow();
					if (tv_Doc.FocusedNode != null)
					{
						dataRow["AbsEntry"] = Function.ParseInt(tv_Doc.FocusedNode.GetValue("AbsEntry"));
					}
					dataRow["FormId"] = ActionFormId;
					dataRow["ItemId"] = ActionItemId;
					dataRow["LineId"] = 0;
					dataRow["ParamName"] = parameterFieldName;
					dataRow["ParamType"] = "P";
					dataTable.Rows.Add(dataRow);
				}
			}
			dataTable.AcceptChanges();
			grd_PrintLayout.DataSource = dataTable;
			grd_PrintLayout.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void look_PrintLayoutFileType_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			string val = Function.ToString(look_PrintLayoutFileType.EditValue);
			if (FormMode != Enums.FormViewModeType.Busy && tv_Doc.FocusedNode != null)
			{
				tv_Doc.FocusedNode.SetValue("FileType", val);
				tv_Doc.DataSourceTable().AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_EditScript_Click(object sender, EventArgs e)
	{
		try
		{
			int actionId = Function.ParseInt(tv_Doc.FocusedNode.GetValue("AbsEntry"));
			string actionName = Function.ToString(tv_Doc.FocusedNode.GetValue("Caption"));
			frm_UserDefinedProcedureCreate frm_UserDefinedProcedureCreate2 = new frm_UserDefinedProcedureCreate(BaseForm, ActionFormId, actionId, actionName, isFromContextAction: true, Function.ToString(tv_Doc.FocusedNode.GetValue("DIYScript")));
			frm_UserDefinedProcedureCreate2.SaveContextActionScript = (Action<string>)Delegate.Combine(frm_UserDefinedProcedureCreate2.SaveContextActionScript, new Action<string>(SaveContextActionScript));
			frm_UserDefinedProcedureCreate2.ShowDialog();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void SaveContextActionScript(string obj)
	{
		try
		{
			rtxt_Script.Text = obj;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rtxt_Script_TextChanged(object sender, EventArgs e)
	{
		try
		{
			string val = Function.ToString(rtxt_Script.Text);
			if (FormMode != Enums.FormViewModeType.Busy && tv_Doc.FocusedNode != null)
			{
				tv_Doc.FocusedNode.SetValue("DIYScript", val);
				tv_Doc.DataSourceTable().AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void look_RptUseSourceFrom_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			string val = Function.ToString(look_RptUseSourceFrom.EditValue);
			if (FormMode != Enums.FormViewModeType.Busy && tv_Doc.FocusedNode != null)
			{
				tv_Doc.FocusedNode.SetValue("RptUseSource", val);
				tv_Doc.DataSourceTable().AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void txt_TemporaryTableName_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			string val = Function.ToString(txt_TemporaryTableName.EditValue);
			if (FormMode != Enums.FormViewModeType.Busy && tv_Doc.FocusedNode != null)
			{
				tv_Doc.FocusedNode.SetValue("TempTblName", val);
				tv_Doc.DataSourceTable().AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_PublishService_Click(object sender, EventArgs e)
	{
		try
		{
			frm_ContextActionPublishService frm_ContextActionPublishService2 = new frm_ContextActionPublishService();
			frm_ContextActionPublishService2.ActionAbsEntry = Function.ParseInt(tv_Doc.FocusedNode.GetValue("AbsEntry"));
			frm_ContextActionPublishService2.ActionFormId = ActionFormId;
			frm_ContextActionPublishService2.ActionItemId = ActionItemId;
			frm_ContextActionPublishService2.Source = rep_SourceFieldHeader.DataSource as DataTable;
			frm_ContextActionPublishService2.ModifyFormContextAction = true;
			frm_ContextActionPublishService2.ShowDialog();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void txt_PhysicalTableName_Leave(object sender, EventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Busy)
			{
				string text = Function.ToString(txt_PhysicalTableName.EditValue);
				if (tv_Doc.FocusedNode != null)
				{
					Enums.FormViewModeType formMode = FormMode;
					FormMode = Enums.FormViewModeType.Busy;
					tv_Doc.FocusedNode.SetValue("PhysicalTableName", text);
					tv_Doc.DataSourceTable().AcceptChanges();
					FormMode = formMode;
				}
				if (!string.IsNullOrEmpty(text))
				{
					await LoadPhysicalTableField(text);
					grd_PhysicalTable.DataSourceTable.Rows.Clear();
					grd_PhysicalTable.RefreshDataSource();
					MappingTarget(Function.ParseInt(tv_Doc.FocusedNode.GetValue("AbsEntry")), "P", "H", _dtPhysicalTable, grd_PhysicalTable);
				}
				else
				{
					grd_PhysicalTable.DataSourceTable.Rows.Clear();
					grd_PhysicalTable.RefreshDataSource();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void look_PrintAction_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Busy && tv_Doc.FocusedNode != null)
			{
				tv_Doc.FocusedNode.SetValue("PrintAction", Function.ToString(look_PrintAction.EditValue));
				tv_Doc.DataSourceTable().AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void chk_JoinValue_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Busy && tv_Doc.FocusedNode != null)
			{
				tv_Doc.FocusedNode.SetValue("JoinValue", Function.ToString(chk_JoinValue.EditValue));
				tv_Doc.DataSourceTable().AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void chk_UseTempTable_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			if (chk_UseTempTable.Checked)
			{
				txt_TempTableNameDoQuery.Visible = true;
				lbl_TempTableNameDoQuery.Visible = true;
			}
			else if (!chk_UseTempTable.Checked)
			{
				txt_TempTableNameDoQuery.EditValue = string.Empty;
				txt_TempTableNameDoQuery.Visible = false;
				lbl_TempTableNameDoQuery.Visible = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void txt_TempTableNameDoQuery_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			string val = Function.ToString(txt_TempTableNameDoQuery.EditValue);
			if (FormMode != Enums.FormViewModeType.Busy && tv_Doc.FocusedNode != null)
			{
				tv_Doc.FocusedNode.SetValue("TempTblNameDoQuery", val);
				tv_Doc.DataSourceTable().AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void tv_Doc_DragDrop(object sender, DragEventArgs e)
	{
		if (FormMode == Enums.FormViewModeType.View)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	private void tv_Doc_BeforeFocusNode(object sender, BeforeFocusNodeEventArgs e)
	{
		if (_isLoading)
		{
			e.CanFocus = false;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Commons.frm_ContextAction));
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled2 = new DevExpress.Utils.SerializableAppearanceObject();
		this.rep_AttParamDataType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.rep_AttParamSourceField = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.repositoryItemCheckEdit3 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.grd_Filter = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Filter = new Apzon.Controls.Grid.GridViewEx();
		this.tv_Doc = new Apzon.Controls.Tree.TreeEx();
		this.treeListColumn1 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.rep_Caption = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.treeListColumn3 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.rep_SourceType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.treeListColumn4 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.rep_DataRange = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.treeListColumn2 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.rep_ActionType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.treeListColumnEx1 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.rep_Method = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.treeListColumn6 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.treeListColumn7 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.treeListColumn5 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.treeListColumnEx3 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.treeListColumn8 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.rep_MsgSuccess = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.treeListColumn9 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.rep_MsgType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.treeListColumnEx2 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.rep_EditorType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.treeListColumnEx4 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.treeListColumnEx5 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.chk_Invisible = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.treeListColumnEx6 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.rep_Icon = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
		this.imageCollection1 = new DevExpress.Utils.ImageCollection(this.components);
		this.treeListColumnIgnoreError = new Apzon.Controls.Tree.TreeListColumnEx();
		this.rIgnoreError = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.treeListColumnEx7 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.xtraTab_Main = new Apzon.Controls.Controls.XtraTabControlEx();
		this.tab_SourceData = new Apzon.Controls.Controls.XtraTabPageEx();
		this.txt_TemporaryTableName = new Apzon.Controls.Editor.TextEditEx();
		this.lbl_TemporaryName = new Apzon.Controls.Controls.LabelLine();
		this.look_ParameterType = new Apzon.Controls.Editor.LookUpEditEx();
		this.lbl_ParameterType = new Apzon.Controls.Controls.LabelLine();
		this.memo_DataString = new Apzon.Controls.Editor.MemoExEditEx();
		this.bte_FilterData = new Apzon.Controls.Editor.ButtonEditEx();
		this.lbl_FilterData = new Apzon.Controls.Controls.LabelLine();
		this.grd_Parameters = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.removeRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Parameters = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx9 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx10 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx11 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_DataType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx14 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_SourceFieldData = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx12 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx13 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx16 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.bte_DataFrom = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.look_DataFrom = new Apzon.Controls.Editor.LookUpEditEx();
		this.lbl_DataFrom = new Apzon.Controls.Controls.LabelLine();
		this.tab_Report = new Apzon.Controls.Controls.XtraTabPageEx();
		this.chk_JoinValue = new DevExpress.XtraEditors.CheckEdit();
		this.grd_Report = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Report = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx15 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx17 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx18 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_ReportDataType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx19 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_SourceField = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx20 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx21 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx22 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCheckEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.memo_RptString = new Apzon.Controls.Editor.MemoExEditEx();
		this.bte_RptString = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.look_ReportFrom = new DevExpress.XtraEditors.LookUpEdit();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.tab_Documents = new Apzon.Controls.Controls.XtraTabPageEx();
		this.look_DocEntry = new Apzon.Controls.Editor.LookUpEditEx();
		this.lb_DocumentDocEntry = new Apzon.Controls.Controls.LabelLine();
		this.chk_ShowDocForm = new Apzon.Controls.Editor.CheckEditEx();
		this.look_ObjectType = new Apzon.Controls.Editor.LookUpEditEx();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.tabControl_MappingDocument = new Apzon.Controls.Controls.XtraTabControlEx();
		this.xtraTabPage5 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_Header = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Header = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_SourceFieldHeader = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx4 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_TargetField = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx23 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx30 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_SumTypeHeader = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.xtraTabPage6 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_Items = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Items = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx5 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx6 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_SourceFieldItem = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx7 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx8 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_TargetFieldLine = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx24 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx31 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_SumTypeLine = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.xtraTabPage1 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_Batch = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Batch = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx25 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx26 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_SourceFieldBatch = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx27 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx28 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_TargetFieldBatch = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx29 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx32 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_SumTypeBatch = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.tab_Udo = new Apzon.Controls.Controls.XtraTabPageEx();
		this.chk_ShowUdoForm = new Apzon.Controls.Editor.CheckEditEx();
		this.look_UdoCode = new Apzon.Controls.Editor.LookUpEditEx();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.tabControl_Udo = new Apzon.Controls.Controls.XtraTabControlEx();
		this.tab_Dashboard = new Apzon.Controls.Controls.XtraTabPageEx();
		this.btn_ChooseDashboard = new Apzon.Controls.Controls.SimpleButtonEx();
		this.ucDashboardViewer1 = new Apzon.Forms.Widget.UcDashboardViewer();
		this.btn_EditDesign = new Apzon.Controls.Controls.SimpleButtonEx();
		this.tab_ProductionOrder = new Apzon.Controls.Controls.XtraTabPageEx();
		this.look_ProductionEntry = new Apzon.Controls.Editor.LookUpEditEx();
		this.lb_ProductionEntry = new Apzon.Controls.Controls.LabelLine();
		this.tabControl_MappingProduction = new Apzon.Controls.Controls.XtraTabControlEx();
		this.xtraTabPage2 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_ProductionHeader = new Apzon.Controls.Grid.GridControlEx();
		this.gv_ProductionHeader = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx33 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx34 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_ProductionHeaderSourceField = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx35 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx36 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_ProductionTargetHeader = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx37 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx38 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_ProductionHeaderSumType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.xtraTabPage3 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_ProductionItem = new Apzon.Controls.Grid.GridControlEx();
		this.gv_ProductionItem = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx39 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx40 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_ProductionLineSourceField = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx41 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx42 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_ProductionOrderTargetLine = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx43 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx44 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_ProductionLineSumType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.xtraTabPage4 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_ProductionBatch = new Apzon.Controls.Grid.GridControlEx();
		this.gv_ProductionBatch = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx45 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx46 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_ProductionBatchSourceField = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx47 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx48 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_ProdoctionTargetBatch = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx49 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx50 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_ProductionBatchSumType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.chk_ShowProductionForm = new Apzon.Controls.Editor.CheckEditEx();
		this.tab_Email = new Apzon.Controls.Controls.XtraTabPageEx();
		this.txt_EmailGroupByFields = new Apzon.Controls.Editor.ButtonEditEx();
		this.labelLine10 = new Apzon.Controls.Controls.LabelLine();
		this.look_EmailGroupBy = new Apzon.Controls.Editor.LookUpEditEx();
		this.labelLine9 = new Apzon.Controls.Controls.LabelLine();
		this.btn_EditEmailContent = new DevExpress.XtraEditors.SimpleButton();
		this.xtraTabControl4 = new Apzon.Controls.Controls.XtraTabControlEx();
		this.xtraTabPage8 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.snapControl1 = new DevExpress.Snap.SnapControl();
		this.xtraTabPage9 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.splitContainerControl2 = new DevExpress.XtraEditors.SplitContainerControl();
		this.grd_AttachFiles = new Apzon.Controls.Grid.GridControlEx();
		this.gv_AttachFiles = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx58 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Path = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gridColumnEx59 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx60 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_AttType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.grd_RptParam = new Apzon.Controls.Grid.GridControlEx();
		this.gv_RptParam = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx51 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx52 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx53 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx54 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx55 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx56 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx57 = new Apzon.Controls.Grid.GridColumnEx();
		this.txt_Subject = new Apzon.Controls.Editor.ButtonEditEx();
		this.txt_AddressBcc = new Apzon.Controls.Editor.ButtonEditEx();
		this.txt_AddressCc = new Apzon.Controls.Editor.ButtonEditEx();
		this.txt_AddressTo = new Apzon.Controls.Editor.ButtonEditEx();
		this.txt_AddressFrom = new Apzon.Controls.Editor.ButtonEditEx();
		this.labelLine8 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine7 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine6 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine5 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.tab_Forecast = new DevExpress.XtraTab.XtraTabPage();
		this.xtraTabControlEx1 = new Apzon.Controls.Controls.XtraTabControlEx();
		this.xtraTabPageEx1 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_ForecastHeader = new Apzon.Controls.Grid.GridControlEx();
		this.gv_ForecastHeader = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx73 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx74 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_ForecastSourceField = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx75 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx76 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_ForecastTargetField = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx77 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemTextEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.xtraTabPageEx2 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_ForecastItems = new Apzon.Controls.Grid.GridControlEx();
		this.gv_ForecastItems = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx79 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx80 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_ForecastItemSourceField = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx81 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx82 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_ForecastItemTargetField = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx83 = new Apzon.Controls.Grid.GridColumnEx();
		this.chk_ForecastShowWindow = new Apzon.Controls.Editor.CheckEditEx();
		this.tab_DoQuery = new Apzon.Controls.Controls.XtraTabPageEx();
		this.txt_TempTableNameDoQuery = new Apzon.Controls.Editor.TextEditEx();
		this.lbl_TempTableNameDoQuery = new Apzon.Controls.Controls.LabelLine();
		this.chk_UseTempTable = new Apzon.Controls.Editor.CheckEditEx();
		this.grd_DoQuery = new Apzon.Controls.Grid.GridControlEx();
		this.gv_DoQuery = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx78 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx84 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx85 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_QueryDataType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx86 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_QuerySourceField = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx87 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx88 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx89 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCheckEdit4 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.memo_QueryString = new Apzon.Controls.Editor.MemoExEditEx();
		this.txt_QueryString = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.look_QueryFrom = new Apzon.Controls.Editor.LookUpEditEx();
		this.labelLine11 = new Apzon.Controls.Controls.LabelLine();
		this.tab_PrintLayout = new Apzon.Controls.Controls.XtraTabPageEx();
		this.look_PrintAction = new Apzon.Controls.Editor.LookUpEditEx();
		this.look_RptUseSourceFrom = new Apzon.Controls.Editor.LookUpEditEx();
		this.toolTipController1 = new DevExpress.Utils.ToolTipController(this.components);
		this.labelLine15 = new Apzon.Controls.Controls.LabelLine();
		this.grd_PrintLayout = new Apzon.Controls.Grid.GridControlEx();
		this.gv_PrintLayout = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx90 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx91 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx92 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_PrintLayoutDataType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx93 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_PrintLayoutSourceField = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx94 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx95 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx96 = new Apzon.Controls.Grid.GridColumnEx();
		this.look_PrintLayoutFileType = new Apzon.Controls.Editor.LookUpEditEx();
		this.labelLine14 = new Apzon.Controls.Controls.LabelLine();
		this.bte_PrintLayoutPath = new Apzon.Controls.Editor.ButtonEditEx();
		this.labelLine13 = new Apzon.Controls.Controls.LabelLine();
		this.tab_ApprovalDecision = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_Approval = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Approval = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx97 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx98 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_ApprovalSourceField = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx99 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx100 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_ApprovalTargetField = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx101 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemLookUpEdit3 = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.tab_DIYScript = new Apzon.Controls.Controls.XtraTabPageEx();
		this.rtxt_Script = new System.Windows.Forms.RichTextBox();
		this.btn_EditScript = new DevExpress.XtraEditors.SimpleButton();
		this.tab_HttpRequest = new Apzon.Controls.Controls.XtraTabPageEx();
		this.ucHttpRequest = new Apzon.Forms.UserControls.ContextAction.UcContextActionHttpRequest();
		this.tab_PhysicalTable = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_PhysicalTable = new Apzon.Controls.Grid.GridControlEx();
		this.gv_PhysicalTable = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx102 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx103 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_PhysicalTableSourceField = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx104 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx105 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_PhysicalTableTargetField = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx106 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx107 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemLookUpEdit4 = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.txt_PhysicalTableName = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine12 = new Apzon.Controls.Controls.LabelLine();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.drop_ExportSetting = new DevExpress.XtraEditors.DropDownButton();
		this.btn_Import = new Apzon.Controls.Controls.SimpleButtonEx();
		this.snapDockManager1 = new DevExpress.Snap.Extensions.SnapDockManager(this.components);
		this.snapDocumentManager1 = new DevExpress.Snap.Extensions.SnapDocumentManager(this.components);
		this.noDocumentsView1 = new DevExpress.XtraBars.Docking2010.Views.NoDocuments.NoDocumentsView(this.components);
		this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
		this.openFileDialog2 = new System.Windows.Forms.OpenFileDialog();
		this.btn_PublishService = new DevExpress.XtraEditors.SimpleButton();
		((System.ComponentModel.ISupportInitialize)this.rep_AttParamDataType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_AttParamSourceField).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Filter).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Filter).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Caption).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_SourceType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_DataRange).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ActionType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Method).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_MsgSuccess).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_MsgType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_EditorType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Invisible).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Icon).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.imageCollection1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rIgnoreError).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.xtraTab_Main).BeginInit();
		this.xtraTab_Main.SuspendLayout();
		this.tab_SourceData.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txt_TemporaryTableName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.look_ParameterType.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.memo_DataString.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bte_FilterData.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Parameters).BeginInit();
		this.contextMenuStrip2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Parameters).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_DataType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_SourceFieldData).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bte_DataFrom.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.look_DataFrom.Properties).BeginInit();
		this.tab_Report.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.chk_JoinValue.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Report).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Report).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ReportDataType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_SourceField).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.memo_RptString.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bte_RptString.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.look_ReportFrom.Properties).BeginInit();
		this.tab_Documents.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.look_DocEntry.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_ShowDocForm.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.look_ObjectType.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tabControl_MappingDocument).BeginInit();
		this.tabControl_MappingDocument.SuspendLayout();
		this.xtraTabPage5.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Header).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Header).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_SourceFieldHeader).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_TargetField).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_SumTypeHeader).BeginInit();
		this.xtraTabPage6.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Items).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Items).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_SourceFieldItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_TargetFieldLine).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_SumTypeLine).BeginInit();
		this.xtraTabPage1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Batch).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Batch).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_SourceFieldBatch).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_TargetFieldBatch).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_SumTypeBatch).BeginInit();
		this.tab_Udo.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.chk_ShowUdoForm.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.look_UdoCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tabControl_Udo).BeginInit();
		this.tab_Dashboard.SuspendLayout();
		this.tab_ProductionOrder.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.look_ProductionEntry.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tabControl_MappingProduction).BeginInit();
		this.tabControl_MappingProduction.SuspendLayout();
		this.xtraTabPage2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_ProductionHeader).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_ProductionHeader).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ProductionHeaderSourceField).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ProductionTargetHeader).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ProductionHeaderSumType).BeginInit();
		this.xtraTabPage3.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_ProductionItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_ProductionItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ProductionLineSourceField).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ProductionOrderTargetLine).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ProductionLineSumType).BeginInit();
		this.xtraTabPage4.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_ProductionBatch).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_ProductionBatch).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ProductionBatchSourceField).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ProdoctionTargetBatch).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ProductionBatchSumType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_ShowProductionForm.Properties).BeginInit();
		this.tab_Email.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txt_EmailGroupByFields.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.look_EmailGroupBy.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl4).BeginInit();
		this.xtraTabControl4.SuspendLayout();
		this.xtraTabPage8.SuspendLayout();
		this.xtraTabPage9.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl2).BeginInit();
		this.splitContainerControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_AttachFiles).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_AttachFiles).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Path).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_AttType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_RptParam).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_RptParam).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Subject.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_AddressBcc.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_AddressCc.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_AddressTo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_AddressFrom.Properties).BeginInit();
		this.tab_Forecast.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControlEx1).BeginInit();
		this.xtraTabControlEx1.SuspendLayout();
		this.xtraTabPageEx1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_ForecastHeader).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_ForecastHeader).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ForecastSourceField).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ForecastTargetField).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEdit1).BeginInit();
		this.xtraTabPageEx2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_ForecastItems).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_ForecastItems).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ForecastItemSourceField).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ForecastItemTargetField).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_ForecastShowWindow.Properties).BeginInit();
		this.tab_DoQuery.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txt_TempTableNameDoQuery.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_UseTempTable.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_DoQuery).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_DoQuery).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_QueryDataType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_QuerySourceField).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.memo_QueryString.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_QueryString.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.look_QueryFrom.Properties).BeginInit();
		this.tab_PrintLayout.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.look_PrintAction.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.look_RptUseSourceFrom.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_PrintLayout).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_PrintLayout).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_PrintLayoutDataType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_PrintLayoutSourceField).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.look_PrintLayoutFileType.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bte_PrintLayoutPath.Properties).BeginInit();
		this.tab_ApprovalDecision.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Approval).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Approval).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ApprovalSourceField).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ApprovalTargetField).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemLookUpEdit3).BeginInit();
		this.tab_DIYScript.SuspendLayout();
		this.tab_HttpRequest.SuspendLayout();
		this.tab_PhysicalTable.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_PhysicalTable).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_PhysicalTable).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_PhysicalTableSourceField).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_PhysicalTableTargetField).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemLookUpEdit4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_PhysicalTableName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.snapDockManager1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.snapDocumentManager1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.noDocumentsView1).BeginInit();
		base.SuspendLayout();
		this.rep_AttParamDataType.AutoHeight = false;
		this.rep_AttParamDataType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_AttParamDataType.Name = "rep_AttParamDataType";
		this.rep_AttParamDataType.NullText = "";
		this.rep_AttParamSourceField.AutoHeight = false;
		this.rep_AttParamSourceField.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_AttParamSourceField.Name = "rep_AttParamSourceField";
		this.rep_AttParamSourceField.NullText = "";
		this.repositoryItemCheckEdit3.AutoHeight = false;
		this.repositoryItemCheckEdit3.Name = "repositoryItemCheckEdit3";
		this.repositoryItemCheckEdit3.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.repositoryItemCheckEdit3.ValueChecked = "Y";
		this.repositoryItemCheckEdit3.ValueGrayed = "N";
		this.repositoryItemCheckEdit3.ValueUnchecked = "N";
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(1, 2);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.grd_Filter);
		this.splitContainerControl1.Panel1.Controls.Add(this.tv_Doc);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.xtraTab_Main);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(940, 656);
		this.splitContainerControl1.SplitterPosition = 221;
		this.splitContainerControl1.TabIndex = 0;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.grd_Filter.AllowPasteExcel = false;
		this.grd_Filter.ClipboardData = null;
		this.grd_Filter.Location = new System.Drawing.Point(223, 53);
		this.grd_Filter.MainView = this.gv_Filter;
		this.grd_Filter.Name = "grd_Filter";
		this.grd_Filter.Size = new System.Drawing.Size(413, 144);
		this.grd_Filter.TabIndex = 3;
		this.grd_Filter.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Filter });
		this.grd_Filter.Visible = false;
		this.gv_Filter.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Filter.ColumnKey = null;
		this.gv_Filter.DisableFormSetting = false;
		this.gv_Filter.DisplayException = false;
		this.gv_Filter.FieldCurrency = null;
		this.gv_Filter.FieldRate = null;
		this.gv_Filter.FormId = "oContextAction";
		this.gv_Filter.GridControl = this.grd_Filter;
		this.gv_Filter.IndicatorWidth = 35;
		this.gv_Filter.IsApzTable = false;
		this.gv_Filter.IsInsertNewRow = false;
		this.gv_Filter.IsLoadingData = false;
		this.gv_Filter.ItemId = "OFILLTER";
		this.gv_Filter.LstColumnAllowBatch = null;
		this.gv_Filter.LstColumnNotAllowBatch = null;
		this.gv_Filter.LstColumnNotEditable = null;
		this.gv_Filter.LstColumNotSetting = null;
		this.gv_Filter.Name = "gv_Filter";
		this.gv_Filter.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Filter.OptionsView.ColumnAutoWidth = false;
		this.gv_Filter.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Filter.OptionsView.ShowGroupPanel = false;
		this.gv_Filter.UseBatchMod = false;
		this.tv_Doc.AllowPasteExcel = false;
		this.tv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.tv_Doc.ColumnKey = null;
		this.tv_Doc.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[17]
		{
			this.treeListColumn1, this.treeListColumn3, this.treeListColumn4, this.treeListColumn2, this.treeListColumnEx1, this.treeListColumn6, this.treeListColumn7, this.treeListColumn5, this.treeListColumnEx3, this.treeListColumn8,
			this.treeListColumn9, this.treeListColumnEx2, this.treeListColumnEx4, this.treeListColumnEx5, this.treeListColumnEx6, this.treeListColumnIgnoreError, this.treeListColumnEx7
		});
		this.tv_Doc.ContextMenuStrip = this.contextMenuStrip1;
		this.tv_Doc.Cursor = System.Windows.Forms.Cursors.Default;
		this.tv_Doc.DisableFormSetting = false;
		this.tv_Doc.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tv_Doc.EditColumn = null;
		this.tv_Doc.FieldCurrency = null;
		this.tv_Doc.FieldRate = null;
		this.tv_Doc.FormId = "oContextAction";
		this.tv_Doc.HasFormSetting = false;
		this.tv_Doc.IndexEditRow = 0;
		this.tv_Doc.IndicatorWidth = 40;
		this.tv_Doc.IsApzTable = false;
		this.tv_Doc.IsLoadingData = false;
		this.tv_Doc.ItemId = "ODOC";
		this.tv_Doc.KeyFieldName = "AbsEntry";
		this.tv_Doc.Location = new System.Drawing.Point(0, 0);
		this.tv_Doc.LstColumnNotEditable = null;
		this.tv_Doc.LstColumNotSetting = null;
		this.tv_Doc.LstOldUDF = null;
		this.tv_Doc.Name = "tv_Doc";
		this.tv_Doc.OptionsDragAndDrop.DragNodesMode = DevExpress.XtraTreeList.DragNodesMode.Single;
		this.tv_Doc.OptionsView.AutoWidth = false;
		this.tv_Doc.ParentFieldName = "FatherId";
		this.tv_Doc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[11]
		{
			this.rep_ActionType, this.rep_SourceType, this.rep_Caption, this.rep_DataRange, this.rep_MsgSuccess, this.rep_MsgType, this.rep_Method, this.rep_EditorType, this.chk_Invisible, this.rep_Icon,
			this.rIgnoreError
		});
		this.tv_Doc.SettingReadonly = false;
		this.tv_Doc.ShowButtonMode = DevExpress.XtraTreeList.ShowButtonModeEnum.ShowAlways;
		this.tv_Doc.Size = new System.Drawing.Size(940, 221);
		this.tv_Doc.TabIndex = 0;
		this.tv_Doc.CellValueChanged += new DevExpress.XtraTreeList.CellValueChangedEventHandler(this.tv_Doc_CellValueChanged);
		this.tv_Doc.NodeCellStyle += new DevExpress.XtraTreeList.GetCustomNodeCellStyleEventHandler(this.tv_Doc_NodeCellStyle);
		this.tv_Doc.BeforeFocusNode += new DevExpress.XtraTreeList.BeforeFocusNodeEventHandler(this.tv_Doc_BeforeFocusNode);
		this.tv_Doc.FocusedNodeChanged += new DevExpress.XtraTreeList.FocusedNodeChangedEventHandler(this.tv_Doc_FocusedNodeChanged);
		this.tv_Doc.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.tv_Doc_ShowingEditor);
		this.tv_Doc.DragDrop += new System.Windows.Forms.DragEventHandler(this.tv_Doc_DragDrop);
		this.treeListColumn1.Caption = "Caption";
		this.treeListColumn1.ColumnEdit = this.rep_Caption;
		this.treeListColumn1.CurrencyDateField = null;
		this.treeListColumn1.CurrencyField = null;
		this.treeListColumn1.CurrencyFixed = null;
		this.treeListColumn1.CurrencyFixedRate = 0.0;
		this.treeListColumn1.CurrencyRateField = null;
		this.treeListColumn1.FieldName = "Caption";
		this.treeListColumn1.Name = "treeListColumn1";
		this.treeListColumn1.Visible = true;
		this.treeListColumn1.VisibleIndex = 0;
		this.treeListColumn1.Width = 200;
		this.rep_Caption.AutoHeight = false;
		this.rep_Caption.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Plus)
		});
		this.rep_Caption.Name = "rep_Caption";
		this.rep_Caption.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_Caption_ButtonClick);
		this.treeListColumn3.Caption = "Source Data Type";
		this.treeListColumn3.ColumnEdit = this.rep_SourceType;
		this.treeListColumn3.CurrencyDateField = null;
		this.treeListColumn3.CurrencyField = null;
		this.treeListColumn3.CurrencyFixed = null;
		this.treeListColumn3.CurrencyFixedRate = 0.0;
		this.treeListColumn3.CurrencyRateField = null;
		this.treeListColumn3.FieldName = "SourceType";
		this.treeListColumn3.Name = "treeListColumn3";
		this.treeListColumn3.Visible = true;
		this.treeListColumn3.VisibleIndex = 1;
		this.treeListColumn3.Width = 140;
		this.rep_SourceType.AutoHeight = false;
		this.rep_SourceType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_SourceType.Name = "rep_SourceType";
		this.rep_SourceType.NullText = "";
		this.treeListColumn4.Caption = "Data Range";
		this.treeListColumn4.ColumnEdit = this.rep_DataRange;
		this.treeListColumn4.CurrencyDateField = null;
		this.treeListColumn4.CurrencyField = null;
		this.treeListColumn4.CurrencyFixed = null;
		this.treeListColumn4.CurrencyFixedRate = 0.0;
		this.treeListColumn4.CurrencyRateField = null;
		this.treeListColumn4.FieldName = "DataRange";
		this.treeListColumn4.Name = "treeListColumn4";
		this.treeListColumn4.Visible = true;
		this.treeListColumn4.VisibleIndex = 2;
		this.treeListColumn4.Width = 182;
		this.rep_DataRange.AutoHeight = false;
		this.rep_DataRange.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_DataRange.Name = "rep_DataRange";
		this.rep_DataRange.NullText = "";
		this.treeListColumn2.Caption = "Action Type";
		this.treeListColumn2.ColumnEdit = this.rep_ActionType;
		this.treeListColumn2.CurrencyDateField = null;
		this.treeListColumn2.CurrencyField = null;
		this.treeListColumn2.CurrencyFixed = null;
		this.treeListColumn2.CurrencyFixedRate = 0.0;
		this.treeListColumn2.CurrencyRateField = null;
		this.treeListColumn2.FieldName = "ActionTypeEx";
		this.treeListColumn2.Name = "treeListColumn2";
		this.treeListColumn2.Visible = true;
		this.treeListColumn2.VisibleIndex = 3;
		this.treeListColumn2.Width = 166;
		this.rep_ActionType.AutoHeight = false;
		this.rep_ActionType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_ActionType.Name = "rep_ActionType";
		this.rep_ActionType.NullText = "";
		this.treeListColumnEx1.Caption = "Method";
		this.treeListColumnEx1.ColumnEdit = this.rep_Method;
		this.treeListColumnEx1.CurrencyDateField = null;
		this.treeListColumnEx1.CurrencyField = null;
		this.treeListColumnEx1.CurrencyFixed = null;
		this.treeListColumnEx1.CurrencyFixedRate = 0.0;
		this.treeListColumnEx1.CurrencyRateField = null;
		this.treeListColumnEx1.FieldName = "Method";
		this.treeListColumnEx1.Name = "treeListColumnEx1";
		this.treeListColumnEx1.Visible = true;
		this.treeListColumnEx1.VisibleIndex = 4;
		this.rep_Method.AutoHeight = false;
		this.rep_Method.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Method.Name = "rep_Method";
		this.rep_Method.NullText = "";
		this.treeListColumn6.Caption = "FormId";
		this.treeListColumn6.CurrencyDateField = null;
		this.treeListColumn6.CurrencyField = null;
		this.treeListColumn6.CurrencyFixed = null;
		this.treeListColumn6.CurrencyFixedRate = 0.0;
		this.treeListColumn6.CurrencyRateField = null;
		this.treeListColumn6.FieldName = "FormId";
		this.treeListColumn6.Name = "treeListColumn6";
		this.treeListColumn6.Width = 100;
		this.treeListColumn7.Caption = "ItemId";
		this.treeListColumn7.CurrencyDateField = null;
		this.treeListColumn7.CurrencyField = null;
		this.treeListColumn7.CurrencyFixed = null;
		this.treeListColumn7.CurrencyFixedRate = 0.0;
		this.treeListColumn7.CurrencyRateField = null;
		this.treeListColumn7.FieldName = "ItemId";
		this.treeListColumn7.Name = "treeListColumn7";
		this.treeListColumn7.Width = 100;
		this.treeListColumn5.Caption = "Address From";
		this.treeListColumn5.CurrencyDateField = null;
		this.treeListColumn5.CurrencyField = null;
		this.treeListColumn5.CurrencyFixed = null;
		this.treeListColumn5.CurrencyFixedRate = 0.0;
		this.treeListColumn5.CurrencyRateField = null;
		this.treeListColumn5.FieldName = "AddressFrom";
		this.treeListColumn5.Name = "treeListColumn5";
		this.treeListColumnEx3.Caption = "Message Confirm (Before Action)";
		this.treeListColumnEx3.CurrencyDateField = null;
		this.treeListColumnEx3.CurrencyField = null;
		this.treeListColumnEx3.CurrencyFixed = null;
		this.treeListColumnEx3.CurrencyFixedRate = 0.0;
		this.treeListColumnEx3.CurrencyRateField = null;
		this.treeListColumnEx3.FieldName = "MsgBefore";
		this.treeListColumnEx3.Name = "treeListColumnEx3";
		this.treeListColumnEx3.Visible = true;
		this.treeListColumnEx3.VisibleIndex = 5;
		this.treeListColumnEx3.Width = 177;
		this.treeListColumn8.Caption = "Message After Success";
		this.treeListColumn8.ColumnEdit = this.rep_MsgSuccess;
		this.treeListColumn8.CurrencyDateField = null;
		this.treeListColumn8.CurrencyField = null;
		this.treeListColumn8.CurrencyFixed = null;
		this.treeListColumn8.CurrencyFixedRate = 0.0;
		this.treeListColumn8.CurrencyRateField = null;
		this.treeListColumn8.FieldName = "MsgSuccess";
		this.treeListColumn8.Name = "treeListColumn8";
		this.treeListColumn8.Visible = true;
		this.treeListColumn8.VisibleIndex = 6;
		this.treeListColumn8.Width = 136;
		this.rep_MsgSuccess.AutoHeight = false;
		this.rep_MsgSuccess.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rep_MsgSuccess.Name = "rep_MsgSuccess";
		this.rep_MsgSuccess.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_MsgSuccess_ButtonClick);
		this.treeListColumn9.Caption = "Message Success Showing Type";
		this.treeListColumn9.ColumnEdit = this.rep_MsgType;
		this.treeListColumn9.CurrencyDateField = null;
		this.treeListColumn9.CurrencyField = null;
		this.treeListColumn9.CurrencyFixed = null;
		this.treeListColumn9.CurrencyFixedRate = 0.0;
		this.treeListColumn9.CurrencyRateField = null;
		this.treeListColumn9.FieldName = "MsgType";
		this.treeListColumn9.Name = "treeListColumn9";
		this.treeListColumn9.Visible = true;
		this.treeListColumn9.VisibleIndex = 7;
		this.treeListColumn9.Width = 166;
		this.rep_MsgType.AutoHeight = false;
		this.rep_MsgType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_MsgType.Name = "rep_MsgType";
		this.rep_MsgType.NullText = "";
		this.treeListColumnEx2.Caption = "Editor Type";
		this.treeListColumnEx2.ColumnEdit = this.rep_EditorType;
		this.treeListColumnEx2.CurrencyDateField = null;
		this.treeListColumnEx2.CurrencyField = null;
		this.treeListColumnEx2.CurrencyFixed = null;
		this.treeListColumnEx2.CurrencyFixedRate = 0.0;
		this.treeListColumnEx2.CurrencyRateField = null;
		this.treeListColumnEx2.FieldName = "EditorType";
		this.treeListColumnEx2.Name = "treeListColumnEx2";
		this.treeListColumnEx2.Visible = true;
		this.treeListColumnEx2.VisibleIndex = 8;
		this.rep_EditorType.AutoHeight = false;
		this.rep_EditorType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_EditorType.Name = "rep_EditorType";
		this.rep_EditorType.NullText = "";
		this.treeListColumnEx4.Caption = "Unique Alias";
		this.treeListColumnEx4.CurrencyDateField = null;
		this.treeListColumnEx4.CurrencyField = null;
		this.treeListColumnEx4.CurrencyFixed = null;
		this.treeListColumnEx4.CurrencyFixedRate = 0.0;
		this.treeListColumnEx4.CurrencyRateField = null;
		this.treeListColumnEx4.FieldName = "UniqueAlias";
		this.treeListColumnEx4.Name = "treeListColumnEx4";
		this.treeListColumnEx4.Visible = true;
		this.treeListColumnEx4.VisibleIndex = 9;
		this.treeListColumnEx5.Caption = "Invisible";
		this.treeListColumnEx5.ColumnEdit = this.chk_Invisible;
		this.treeListColumnEx5.CurrencyDateField = null;
		this.treeListColumnEx5.CurrencyField = null;
		this.treeListColumnEx5.CurrencyFixed = null;
		this.treeListColumnEx5.CurrencyFixedRate = 0.0;
		this.treeListColumnEx5.CurrencyRateField = null;
		this.treeListColumnEx5.FieldName = "Invisible";
		this.treeListColumnEx5.Name = "treeListColumnEx5";
		this.treeListColumnEx5.Visible = true;
		this.treeListColumnEx5.VisibleIndex = 10;
		this.chk_Invisible.AutoHeight = false;
		this.chk_Invisible.Name = "chk_Invisible";
		this.chk_Invisible.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.chk_Invisible.ValueChecked = "Y";
		this.chk_Invisible.ValueGrayed = "N";
		this.chk_Invisible.ValueUnchecked = "N";
		this.treeListColumnEx6.Caption = "Button Icon";
		this.treeListColumnEx6.ColumnEdit = this.rep_Icon;
		this.treeListColumnEx6.CurrencyDateField = null;
		this.treeListColumnEx6.CurrencyField = null;
		this.treeListColumnEx6.CurrencyFixed = null;
		this.treeListColumnEx6.CurrencyFixedRate = 0.0;
		this.treeListColumnEx6.CurrencyRateField = null;
		this.treeListColumnEx6.FieldName = "Icon";
		this.treeListColumnEx6.Name = "treeListColumnEx6";
		this.treeListColumnEx6.Visible = true;
		this.treeListColumnEx6.VisibleIndex = 11;
		this.rep_Icon.AutoHeight = false;
		this.rep_Icon.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Icon.Name = "rep_Icon";
		this.rep_Icon.SmallImages = this.imageCollection1;
		this.imageCollection1.ImageStream = (DevExpress.Utils.ImageCollectionStreamer)resources.GetObject("imageCollection1.ImageStream");
		this.imageCollection1.Images.SetKeyName(0, "Add_16x16.png");
		this.imageCollection1.Images.SetKeyName(1, "AddFile_16x16.png");
		this.imageCollection1.Images.SetKeyName(2, "Apply_16x16.png");
		this.imageCollection1.Images.SetKeyName(3, "Cancel_16x16.png");
		this.imageCollection1.Images.SetKeyName(4, "Clear_16x16.png");
		this.imageCollection1.Images.SetKeyName(5, "Close_16x16.png");
		this.imageCollection1.Images.SetKeyName(6, "Convert_16x16.png");
		this.imageCollection1.Images.SetKeyName(7, "DeleteList_16x16.png");
		this.imageCollection1.Images.SetKeyName(8, "Download_16x16.png");
		this.imageCollection1.Images.SetKeyName(9, "Insert_16x16.png");
		this.imageCollection1.Images.SetKeyName(10, "LoadFrom_16x16.png");
		this.imageCollection1.Images.SetKeyName(11, "New_16x16.png");
		this.imageCollection1.Images.SetKeyName(12, "Open_16x16.png");
		this.imageCollection1.Images.SetKeyName(13, "Refresh_16x16.png");
		this.imageCollection1.Images.SetKeyName(14, "RemoveItem_16x16.png");
		this.imageCollection1.Images.SetKeyName(15, "Reset_16x16.png");
		this.imageCollection1.Images.SetKeyName(16, "Find_16x16.png");
		this.imageCollection1.Images.SetKeyName(17, "Attachment_16x16.png");
		this.imageCollection1.Images.SetKeyName(18, "Mail_16x16.png");
		this.imageCollection1.Images.SetKeyName(19, "NewMail_16x16.png");
		this.imageCollection1.Images.SetKeyName(20, "Send_16x16.png");
		this.imageCollection1.Images.SetKeyName(21, "Preview_16x16.png");
		this.imageCollection1.Images.SetKeyName(22, "Print_16x16.png");
		this.imageCollection1.Images.SetKeyName(23, "Save_16x16.png");
		this.imageCollection1.Images.SetKeyName(24, "SaveAll_16x16.png");
		this.imageCollection1.Images.SetKeyName(25, "SaveAndClose_16x16.png");
		this.imageCollection1.Images.SetKeyName(26, "Properties_16x16.png");
		this.imageCollection1.Images.SetKeyName(27, "Area_16x16.png");
		this.imageCollection1.Images.SetKeyName(28, "Bar_16x16.png");
		this.imageCollection1.Images.SetKeyName(29, "Bubble_16x16.png");
		this.imageCollection1.Images.SetKeyName(30, "Chart_16x16.png");
		this.imageCollection1.Images.SetKeyName(31, "FullStackedArea_16x16.png");
		this.imageCollection1.Images.SetKeyName(32, "FullStackedBar_16x16.png");
		this.imageCollection1.Images.SetKeyName(33, "FullStackedLine_16x16.png");
		this.imageCollection1.Images.SetKeyName(34, "FullStackedSplineArea_16x16.png");
		this.imageCollection1.Images.SetKeyName(35, "HighLowClose_16x16.png");
		this.imageCollection1.Images.SetKeyName(36, "KPI_16x16.png");
		this.imageCollection1.Images.SetKeyName(37, "Line_16x16.png");
		this.imageCollection1.Images.SetKeyName(38, "OpenHighLowCloseCandleStick_16x16.png");
		this.imageCollection1.Images.SetKeyName(39, "Pie_16x16.png");
		this.imageCollection1.Images.SetKeyName(40, "PieLabelsDataLabels_16x16.png");
		this.imageCollection1.Images.SetKeyName(41, "PieStyleDonut_16x16.png");
		this.imageCollection1.Images.SetKeyName(42, "Point_16x16.png");
		this.imageCollection1.Images.SetKeyName(43, "RangeArea_16x16.png");
		this.imageCollection1.Images.SetKeyName(44, "RangeBar_16x16.png");
		this.imageCollection1.Images.SetKeyName(45, "SideBySideRangeBar_16x16.png");
		this.imageCollection1.Images.SetKeyName(46, "Spline_16x16.png");
		this.imageCollection1.Images.SetKeyName(47, "SplineArea_16x16.png");
		this.imageCollection1.Images.SetKeyName(48, "StackedArea_16x16.png");
		this.imageCollection1.Images.SetKeyName(49, "StackedBar_16x16.png");
		this.imageCollection1.Images.SetKeyName(50, "StackedLine_16x16.png");
		this.imageCollection1.Images.SetKeyName(51, "StackedSplineArea_16x16.png");
		this.imageCollection1.Images.SetKeyName(52, "StepArea_16x16.png");
		this.imageCollection1.Images.SetKeyName(53, "Stepline_16x16.png");
		this.imageCollection1.Images.SetKeyName(54, "Barcode_16x16.png");
		this.imageCollection1.Images.SetKeyName(55, "Grid_16x16.png");
		this.imageCollection1.Images.SetKeyName(56, "Pivot_16x16.png");
		this.imageCollection1.Images.SetKeyName(57, "Backward_16x16.png");
		this.imageCollection1.Images.SetKeyName(58, "Forward_16x16.png");
		this.imageCollection1.Images.SetKeyName(59, "Next_16x16.png");
		this.imageCollection1.Images.SetKeyName(60, "Previous_16x16.png");
		this.imageCollection1.Images.SetKeyName(61, "Customer_16x16.png");
		this.imageCollection1.Images.SetKeyName(62, "Employee_16x16.png");
		this.imageCollection1.Images.SetKeyName(63, "Team_16x16.png");
		this.imageCollection1.Images.SetKeyName(64, "UserGroup_16x16.png");
		this.imageCollection1.Images.SetKeyName(65, "Report_16x16.png");
		this.treeListColumnIgnoreError.Caption = "Ignore Error";
		this.treeListColumnIgnoreError.ColumnEdit = this.rIgnoreError;
		this.treeListColumnIgnoreError.CurrencyDateField = null;
		this.treeListColumnIgnoreError.CurrencyField = null;
		this.treeListColumnIgnoreError.CurrencyFixed = null;
		this.treeListColumnIgnoreError.CurrencyFixedRate = 0.0;
		this.treeListColumnIgnoreError.CurrencyRateField = null;
		this.treeListColumnIgnoreError.FieldName = "IgnError";
		this.treeListColumnIgnoreError.Name = "treeListColumnIgnoreError";
		this.treeListColumnIgnoreError.Visible = true;
		this.treeListColumnIgnoreError.VisibleIndex = 12;
		this.rIgnoreError.AutoHeight = false;
		this.rIgnoreError.Name = "rIgnoreError";
		this.rIgnoreError.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.rIgnoreError.ValueChecked = "Y";
		this.rIgnoreError.ValueUnchecked = "N";
		this.treeListColumnEx7.Caption = "Log Request";
		this.treeListColumnEx7.ColumnEdit = this.rIgnoreError;
		this.treeListColumnEx7.CurrencyDateField = null;
		this.treeListColumnEx7.CurrencyField = null;
		this.treeListColumnEx7.CurrencyFixed = null;
		this.treeListColumnEx7.CurrencyFixedRate = 0.0;
		this.treeListColumnEx7.CurrencyRateField = null;
		this.treeListColumnEx7.FieldName = "LogRequest";
		this.treeListColumnEx7.Name = "treeListColumnEx7";
		this.treeListColumnEx7.Visible = true;
		this.treeListColumnEx7.VisibleIndex = 13;
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.removeToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(118, 26);
		this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
		this.removeToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
		this.removeToolStripMenuItem.Text = "Remove";
		this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
		this.xtraTab_Main.Dock = System.Windows.Forms.DockStyle.Fill;
		this.xtraTab_Main.IsSearchControl = false;
		this.xtraTab_Main.Location = new System.Drawing.Point(0, 0);
		this.xtraTab_Main.Name = "xtraTab_Main";
		this.xtraTab_Main.SelectedTabPage = this.tab_SourceData;
		this.xtraTab_Main.Size = new System.Drawing.Size(940, 425);
		this.xtraTab_Main.TabIndex = 0;
		this.xtraTab_Main.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[14]
		{
			this.tab_SourceData, this.tab_Report, this.tab_Documents, this.tab_Udo, this.tab_Dashboard, this.tab_ProductionOrder, this.tab_Email, this.tab_Forecast, this.tab_DoQuery, this.tab_PrintLayout,
			this.tab_ApprovalDecision, this.tab_DIYScript, this.tab_HttpRequest, this.tab_PhysicalTable
		});
		this.tab_SourceData.Controls.Add(this.txt_TemporaryTableName);
		this.tab_SourceData.Controls.Add(this.lbl_TemporaryName);
		this.tab_SourceData.Controls.Add(this.look_ParameterType);
		this.tab_SourceData.Controls.Add(this.lbl_ParameterType);
		this.tab_SourceData.Controls.Add(this.memo_DataString);
		this.tab_SourceData.Controls.Add(this.bte_FilterData);
		this.tab_SourceData.Controls.Add(this.lbl_FilterData);
		this.tab_SourceData.Controls.Add(this.grd_Parameters);
		this.tab_SourceData.Controls.Add(this.bte_DataFrom);
		this.tab_SourceData.Controls.Add(this.look_DataFrom);
		this.tab_SourceData.Controls.Add(this.lbl_DataFrom);
		this.tab_SourceData.EditControl = null;
		this.tab_SourceData.EditValue = "Source Data Properties";
		this.tab_SourceData.FieldName = "tab_SourceData";
		this.tab_SourceData.IsSearchControl = false;
		this.tab_SourceData.Name = "tab_SourceData";
		this.tab_SourceData.ReadOnly = false;
		this.tab_SourceData.SettingInVisible = false;
		this.tab_SourceData.SettingReadonly = false;
		this.tab_SourceData.Size = new System.Drawing.Size(938, 400);
		this.tab_SourceData.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_SourceData.Text = "Source Data Properties";
		this.txt_TemporaryTableName.EditControl = null;
		this.txt_TemporaryTableName.EditValue = "tbltemp";
		this.txt_TemporaryTableName.FieldName = null;
		this.txt_TemporaryTableName.IsSearchControl = false;
		this.txt_TemporaryTableName.Location = new System.Drawing.Point(791, 4);
		this.txt_TemporaryTableName.Name = "txt_TemporaryTableName";
		this.txt_TemporaryTableName.SettingInVisible = false;
		this.txt_TemporaryTableName.SettingReadonly = false;
		this.txt_TemporaryTableName.Size = new System.Drawing.Size(133, 20);
		this.txt_TemporaryTableName.TabIndex = 15;
		this.txt_TemporaryTableName.EditValueChanged += new System.EventHandler(this.txt_TemporaryTableName_EditValueChanged);
		this.lbl_TemporaryName.EditControl = null;
		this.lbl_TemporaryName.EditValue = "Temporary Table Name";
		this.lbl_TemporaryName.FieldName = null;
		this.lbl_TemporaryName.HideWhenEmpty = false;
		this.lbl_TemporaryName.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_TemporaryName.Location = new System.Drawing.Point(634, 7);
		this.lbl_TemporaryName.Name = "lbl_TemporaryName";
		this.lbl_TemporaryName.ReadOnly = false;
		this.lbl_TemporaryName.SettingInVisible = false;
		this.lbl_TemporaryName.SettingReadonly = false;
		this.lbl_TemporaryName.ShowUnderLine = true;
		this.lbl_TemporaryName.Size = new System.Drawing.Size(167, 20);
		this.lbl_TemporaryName.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_TemporaryName.TabIndex = 14;
		this.lbl_TemporaryName.Utext = "Temporary Table Name";
		this.look_ParameterType.EditControl = null;
		this.look_ParameterType.FieldName = null;
		this.look_ParameterType.IsSearchControl = false;
		this.look_ParameterType.Location = new System.Drawing.Point(528, 4);
		this.look_ParameterType.Name = "look_ParameterType";
		this.look_ParameterType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.look_ParameterType.Properties.NullText = "";
		this.look_ParameterType.SettingInVisible = false;
		this.look_ParameterType.SettingReadonly = false;
		this.look_ParameterType.Size = new System.Drawing.Size(100, 20);
		this.look_ParameterType.TabIndex = 13;
		this.look_ParameterType.EditValueChanged += new System.EventHandler(this.look_ParameterType_EditValueChanged);
		this.lbl_ParameterType.EditControl = null;
		this.lbl_ParameterType.EditValue = "Parameter Type";
		this.lbl_ParameterType.FieldName = null;
		this.lbl_ParameterType.HideWhenEmpty = false;
		this.lbl_ParameterType.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_ParameterType.Location = new System.Drawing.Point(429, 7);
		this.lbl_ParameterType.Name = "lbl_ParameterType";
		this.lbl_ParameterType.ReadOnly = false;
		this.lbl_ParameterType.SettingInVisible = false;
		this.lbl_ParameterType.SettingReadonly = false;
		this.lbl_ParameterType.ShowUnderLine = true;
		this.lbl_ParameterType.Size = new System.Drawing.Size(105, 20);
		this.lbl_ParameterType.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_ParameterType.TabIndex = 12;
		this.lbl_ParameterType.Utext = "Parameter Type";
		this.memo_DataString.EditControl = null;
		this.memo_DataString.FieldName = null;
		this.memo_DataString.IsSearchControl = false;
		this.memo_DataString.Location = new System.Drawing.Point(930, 4);
		this.memo_DataString.Name = "memo_DataString";
		this.memo_DataString.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.memo_DataString.Properties.ShowIcon = false;
		this.memo_DataString.SettingInVisible = false;
		this.memo_DataString.SettingReadonly = false;
		this.memo_DataString.Size = new System.Drawing.Size(77, 20);
		this.memo_DataString.TabIndex = 6;
		this.memo_DataString.Visible = false;
		this.memo_DataString.EditValueChanged += new System.EventHandler(this.memo_DataString_EditValueChanged);
		this.bte_FilterData.EditControl = null;
		this.bte_FilterData.FieldName = null;
		this.bte_FilterData.IsSearchControl = false;
		this.bte_FilterData.Location = new System.Drawing.Point(1090, 4);
		this.bte_FilterData.Name = "bte_FilterData";
		this.bte_FilterData.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.bte_FilterData.SettingInVisible = false;
		this.bte_FilterData.SettingReadonly = false;
		this.bte_FilterData.Size = new System.Drawing.Size(232, 20);
		this.bte_FilterData.TabIndex = 5;
		this.bte_FilterData.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.bte_FilterData_ButtonClick);
		this.bte_FilterData.EditValueChanged += new System.EventHandler(this.bte_FilterData_EditValueChanged);
		this.lbl_FilterData.EditControl = null;
		this.lbl_FilterData.EditValue = "Filter Data";
		this.lbl_FilterData.FieldName = null;
		this.lbl_FilterData.HideWhenEmpty = false;
		this.lbl_FilterData.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_FilterData.Location = new System.Drawing.Point(1013, 7);
		this.lbl_FilterData.Name = "lbl_FilterData";
		this.lbl_FilterData.ReadOnly = false;
		this.lbl_FilterData.SettingInVisible = false;
		this.lbl_FilterData.SettingReadonly = false;
		this.lbl_FilterData.ShowUnderLine = true;
		this.lbl_FilterData.Size = new System.Drawing.Size(80, 20);
		this.lbl_FilterData.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_FilterData.TabIndex = 4;
		this.lbl_FilterData.Utext = "Filter Data";
		this.grd_Parameters.AllowPasteExcel = true;
		this.grd_Parameters.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Parameters.ClipboardData = null;
		this.grd_Parameters.ContextMenuStrip = this.contextMenuStrip2;
		this.grd_Parameters.Location = new System.Drawing.Point(-1, 27);
		this.grd_Parameters.MainView = this.gv_Parameters;
		this.grd_Parameters.Name = "grd_Parameters";
		this.grd_Parameters.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.repositoryItemCheckEdit1, this.rep_DataType, this.rep_SourceFieldData });
		this.grd_Parameters.Size = new System.Drawing.Size(929, 372);
		this.grd_Parameters.TabIndex = 3;
		this.grd_Parameters.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Parameters });
		this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.removeRowToolStripMenuItem });
		this.contextMenuStrip2.Name = "contextMenuStrip2";
		this.contextMenuStrip2.Size = new System.Drawing.Size(144, 26);
		this.removeRowToolStripMenuItem.Name = "removeRowToolStripMenuItem";
		this.removeRowToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
		this.removeRowToolStripMenuItem.Text = "Remove Row";
		this.removeRowToolStripMenuItem.Click += new System.EventHandler(this.removeRowToolStripMenuItem_Click);
		this.gv_Parameters.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Parameters.ColumnKey = null;
		this.gv_Parameters.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[7] { this.gridColumnEx9, this.gridColumnEx10, this.gridColumnEx11, this.gridColumnEx14, this.gridColumnEx12, this.gridColumnEx13, this.gridColumnEx16 });
		this.gv_Parameters.DisableFormSetting = true;
		this.gv_Parameters.DisplayException = false;
		this.gv_Parameters.FieldCurrency = null;
		this.gv_Parameters.FieldRate = null;
		this.gv_Parameters.FormId = "oContextAction";
		this.gv_Parameters.GridControl = this.grd_Parameters;
		this.gv_Parameters.IndicatorWidth = 35;
		this.gv_Parameters.IsApzTable = false;
		this.gv_Parameters.IsInsertNewRow = false;
		this.gv_Parameters.IsLoadingData = false;
		this.gv_Parameters.ItemId = "OPARAMATER";
		this.gv_Parameters.LstColumnAllowBatch = null;
		this.gv_Parameters.LstColumnNotAllowBatch = null;
		this.gv_Parameters.LstColumnNotEditable = null;
		this.gv_Parameters.LstColumNotSetting = null;
		this.gv_Parameters.Name = "gv_Parameters";
		this.gv_Parameters.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Parameters.OptionsView.ColumnAutoWidth = false;
		this.gv_Parameters.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Parameters.OptionsView.ShowGroupPanel = false;
		this.gv_Parameters.UseBatchMod = false;
		this.gv_Parameters.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Parameters_CellValueChanged);
		this.gridColumnEx9.Caption = "Name";
		this.gridColumnEx9.CurrencyDateField = null;
		this.gridColumnEx9.CurrencyField = null;
		this.gridColumnEx9.CurrencyFixed = null;
		this.gridColumnEx9.CurrencyFixedRate = 0.0;
		this.gridColumnEx9.CurrencyRateField = null;
		this.gridColumnEx9.FieldName = "ParamName";
		this.gridColumnEx9.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx9.Name = "gridColumnEx9";
		this.gridColumnEx9.OptionsColumn.ReadOnly = true;
		this.gridColumnEx9.PrimaryKey = true;
		this.gridColumnEx9.UnitField = null;
		this.gridColumnEx9.Visible = true;
		this.gridColumnEx9.VisibleIndex = 0;
		this.gridColumnEx9.Width = 100;
		this.gridColumnEx10.Caption = "Label";
		this.gridColumnEx10.CurrencyDateField = null;
		this.gridColumnEx10.CurrencyField = null;
		this.gridColumnEx10.CurrencyFixed = null;
		this.gridColumnEx10.CurrencyFixedRate = 0.0;
		this.gridColumnEx10.CurrencyRateField = null;
		this.gridColumnEx10.FieldName = "Label";
		this.gridColumnEx10.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx10.Name = "gridColumnEx10";
		this.gridColumnEx10.PrimaryKey = false;
		this.gridColumnEx10.UnitField = null;
		this.gridColumnEx10.Visible = true;
		this.gridColumnEx10.VisibleIndex = 1;
		this.gridColumnEx10.Width = 100;
		this.gridColumnEx11.Caption = "Data Type";
		this.gridColumnEx11.ColumnEdit = this.rep_DataType;
		this.gridColumnEx11.CurrencyDateField = null;
		this.gridColumnEx11.CurrencyField = null;
		this.gridColumnEx11.CurrencyFixed = null;
		this.gridColumnEx11.CurrencyFixedRate = 0.0;
		this.gridColumnEx11.CurrencyRateField = null;
		this.gridColumnEx11.FieldName = "DataType";
		this.gridColumnEx11.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx11.Name = "gridColumnEx11";
		this.gridColumnEx11.PrimaryKey = false;
		this.gridColumnEx11.UnitField = null;
		this.gridColumnEx11.Visible = true;
		this.gridColumnEx11.VisibleIndex = 2;
		this.gridColumnEx11.Width = 100;
		this.rep_DataType.AutoHeight = false;
		this.rep_DataType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_DataType.Name = "rep_DataType";
		this.rep_DataType.NullText = "";
		this.gridColumnEx14.Caption = "Source Field";
		this.gridColumnEx14.ColumnEdit = this.rep_SourceFieldData;
		this.gridColumnEx14.CurrencyDateField = null;
		this.gridColumnEx14.CurrencyField = null;
		this.gridColumnEx14.CurrencyFixed = null;
		this.gridColumnEx14.CurrencyFixedRate = 0.0;
		this.gridColumnEx14.CurrencyRateField = null;
		this.gridColumnEx14.FieldName = "SourceField";
		this.gridColumnEx14.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx14.Name = "gridColumnEx14";
		this.gridColumnEx14.PrimaryKey = false;
		this.gridColumnEx14.UnitField = null;
		this.gridColumnEx14.Visible = true;
		this.gridColumnEx14.VisibleIndex = 3;
		this.gridColumnEx14.Width = 103;
		this.rep_SourceFieldData.AutoHeight = false;
		this.rep_SourceFieldData.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_SourceFieldData.Name = "rep_SourceFieldData";
		this.rep_SourceFieldData.NullText = "";
		this.gridColumnEx12.Caption = "Sub. Type";
		this.gridColumnEx12.CurrencyDateField = null;
		this.gridColumnEx12.CurrencyField = null;
		this.gridColumnEx12.CurrencyFixed = null;
		this.gridColumnEx12.CurrencyFixedRate = 0.0;
		this.gridColumnEx12.CurrencyRateField = null;
		this.gridColumnEx12.FieldName = "SubType";
		this.gridColumnEx12.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx12.Name = "gridColumnEx12";
		this.gridColumnEx12.PrimaryKey = false;
		this.gridColumnEx12.UnitField = null;
		this.gridColumnEx12.Visible = true;
		this.gridColumnEx12.VisibleIndex = 4;
		this.gridColumnEx12.Width = 100;
		this.gridColumnEx13.Caption = "Default Value";
		this.gridColumnEx13.CurrencyDateField = null;
		this.gridColumnEx13.CurrencyField = null;
		this.gridColumnEx13.CurrencyFixed = null;
		this.gridColumnEx13.CurrencyFixedRate = 0.0;
		this.gridColumnEx13.CurrencyRateField = null;
		this.gridColumnEx13.FieldName = "DefaultValue";
		this.gridColumnEx13.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx13.Name = "gridColumnEx13";
		this.gridColumnEx13.PrimaryKey = false;
		this.gridColumnEx13.UnitField = null;
		this.gridColumnEx13.Visible = true;
		this.gridColumnEx13.VisibleIndex = 5;
		this.gridColumnEx13.Width = 100;
		this.gridColumnEx16.Caption = "Active";
		this.gridColumnEx16.ColumnEdit = this.repositoryItemCheckEdit1;
		this.gridColumnEx16.CurrencyDateField = null;
		this.gridColumnEx16.CurrencyField = null;
		this.gridColumnEx16.CurrencyFixed = null;
		this.gridColumnEx16.CurrencyFixedRate = 0.0;
		this.gridColumnEx16.CurrencyRateField = null;
		this.gridColumnEx16.FieldName = "Active";
		this.gridColumnEx16.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx16.Name = "gridColumnEx16";
		this.gridColumnEx16.PrimaryKey = false;
		this.gridColumnEx16.UnitField = null;
		this.gridColumnEx16.Visible = true;
		this.gridColumnEx16.VisibleIndex = 6;
		this.repositoryItemCheckEdit1.AutoHeight = false;
		this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
		this.repositoryItemCheckEdit1.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.repositoryItemCheckEdit1.ValueChecked = "Y";
		this.repositoryItemCheckEdit1.ValueGrayed = "N";
		this.repositoryItemCheckEdit1.ValueUnchecked = "N";
		this.bte_DataFrom.EditControl = null;
		this.bte_DataFrom.FieldName = null;
		this.bte_DataFrom.IsSearchControl = false;
		this.bte_DataFrom.Location = new System.Drawing.Point(193, 4);
		this.bte_DataFrom.Name = "bte_DataFrom";
		this.bte_DataFrom.Properties.AutoLoadDataSource = true;
		this.bte_DataFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.bte_DataFrom.Properties.ContainerControl = null;
		this.bte_DataFrom.Properties.DisplayMember = "";
		this.bte_DataFrom.Properties.DisplayMember2 = "";
		this.bte_DataFrom.Properties.ListMandatoryField = null;
		this.bte_DataFrom.Properties.LoadDataSourceWhenEnter = false;
		this.bte_DataFrom.Properties.SeparateCharacterForMulti = ';';
		this.bte_DataFrom.Properties.SeparateString = ", ";
		this.bte_DataFrom.Properties.UsingAutoCompleteBox = false;
		this.bte_DataFrom.Properties.UsingDataSource = true;
		this.bte_DataFrom.Properties.ValidateWhenNoDataSource = false;
		this.bte_DataFrom.Properties.ValueMember = "";
		this.bte_DataFrom.SettingInVisible = false;
		this.bte_DataFrom.SettingReadonly = false;
		this.bte_DataFrom.Size = new System.Drawing.Size(230, 20);
		this.bte_DataFrom.TabIndex = 2;
		this.bte_DataFrom.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.bte_DataFrom_ButtonClick);
		this.bte_DataFrom.EditValueChanged += new System.EventHandler(this.bte_DataFrom_EditValueChanged);
		this.look_DataFrom.EditControl = null;
		this.look_DataFrom.FieldName = null;
		this.look_DataFrom.IsSearchControl = false;
		this.look_DataFrom.Location = new System.Drawing.Point(87, 4);
		this.look_DataFrom.Name = "look_DataFrom";
		this.look_DataFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.look_DataFrom.Properties.NullText = "";
		this.look_DataFrom.SettingInVisible = false;
		this.look_DataFrom.SettingReadonly = false;
		this.look_DataFrom.Size = new System.Drawing.Size(100, 20);
		this.look_DataFrom.TabIndex = 1;
		this.look_DataFrom.EditValueChanged += new System.EventHandler(this.look_DataFrom_EditValueChanged);
		this.lbl_DataFrom.EditControl = null;
		this.lbl_DataFrom.EditValue = "Data From";
		this.lbl_DataFrom.FieldName = null;
		this.lbl_DataFrom.HideWhenEmpty = false;
		this.lbl_DataFrom.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_DataFrom.Location = new System.Drawing.Point(10, 7);
		this.lbl_DataFrom.Name = "lbl_DataFrom";
		this.lbl_DataFrom.ReadOnly = false;
		this.lbl_DataFrom.SettingInVisible = false;
		this.lbl_DataFrom.SettingReadonly = false;
		this.lbl_DataFrom.ShowUnderLine = true;
		this.lbl_DataFrom.Size = new System.Drawing.Size(80, 20);
		this.lbl_DataFrom.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_DataFrom.TabIndex = 0;
		this.lbl_DataFrom.Utext = "Data From";
		this.tab_Report.Controls.Add(this.chk_JoinValue);
		this.tab_Report.Controls.Add(this.grd_Report);
		this.tab_Report.Controls.Add(this.memo_RptString);
		this.tab_Report.Controls.Add(this.bte_RptString);
		this.tab_Report.Controls.Add(this.look_ReportFrom);
		this.tab_Report.Controls.Add(this.labelLine2);
		this.tab_Report.EditControl = null;
		this.tab_Report.EditValue = "Report";
		this.tab_Report.FieldName = "tab_Report";
		this.tab_Report.IsSearchControl = false;
		this.tab_Report.Name = "tab_Report";
		this.tab_Report.ReadOnly = false;
		this.tab_Report.SettingInVisible = false;
		this.tab_Report.SettingReadonly = false;
		this.tab_Report.Size = new System.Drawing.Size(938, 400);
		this.tab_Report.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Report.Text = "Report";
		this.chk_JoinValue.Location = new System.Drawing.Point(525, 3);
		this.chk_JoinValue.Name = "chk_JoinValue";
		this.chk_JoinValue.Properties.Caption = "Join value";
		this.chk_JoinValue.Properties.ValueChecked = "Y";
		this.chk_JoinValue.Properties.ValueGrayed = "N";
		this.chk_JoinValue.Properties.ValueUnchecked = "N";
		this.chk_JoinValue.Size = new System.Drawing.Size(75, 20);
		this.chk_JoinValue.TabIndex = 10;
		this.chk_JoinValue.CheckedChanged += new System.EventHandler(this.chk_JoinValue_CheckedChanged);
		this.grd_Report.AllowPasteExcel = false;
		this.grd_Report.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Report.ClipboardData = null;
		this.grd_Report.Location = new System.Drawing.Point(-1, 26);
		this.grd_Report.MainView = this.gv_Report;
		this.grd_Report.Name = "grd_Report";
		this.grd_Report.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.repositoryItemCheckEdit2, this.rep_ReportDataType, this.rep_SourceField });
		this.grd_Report.Size = new System.Drawing.Size(893, 373);
		this.grd_Report.TabIndex = 9;
		this.grd_Report.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Report });
		this.gv_Report.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Report.ColumnKey = null;
		this.gv_Report.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[7] { this.gridColumnEx15, this.gridColumnEx17, this.gridColumnEx18, this.gridColumnEx19, this.gridColumnEx20, this.gridColumnEx21, this.gridColumnEx22 });
		this.gv_Report.DisableFormSetting = true;
		this.gv_Report.DisplayException = false;
		this.gv_Report.FieldCurrency = null;
		this.gv_Report.FieldRate = null;
		this.gv_Report.FormId = "oContextAction";
		this.gv_Report.GridControl = this.grd_Report;
		this.gv_Report.IndicatorWidth = 35;
		this.gv_Report.IsApzTable = false;
		this.gv_Report.IsInsertNewRow = false;
		this.gv_Report.IsLoadingData = false;
		this.gv_Report.ItemId = "OREPORT";
		this.gv_Report.LstColumnAllowBatch = null;
		this.gv_Report.LstColumnNotAllowBatch = null;
		this.gv_Report.LstColumnNotEditable = null;
		this.gv_Report.LstColumNotSetting = null;
		this.gv_Report.Name = "gv_Report";
		this.gv_Report.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Report.OptionsView.ColumnAutoWidth = false;
		this.gv_Report.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Report.OptionsView.ShowGroupPanel = false;
		this.gv_Report.UseBatchMod = false;
		this.gridColumnEx15.Caption = "Name";
		this.gridColumnEx15.CurrencyDateField = null;
		this.gridColumnEx15.CurrencyField = null;
		this.gridColumnEx15.CurrencyFixed = null;
		this.gridColumnEx15.CurrencyFixedRate = 0.0;
		this.gridColumnEx15.CurrencyRateField = null;
		this.gridColumnEx15.FieldName = "ParamName";
		this.gridColumnEx15.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx15.Name = "gridColumnEx15";
		this.gridColumnEx15.OptionsColumn.ReadOnly = true;
		this.gridColumnEx15.PrimaryKey = false;
		this.gridColumnEx15.UnitField = null;
		this.gridColumnEx15.Visible = true;
		this.gridColumnEx15.VisibleIndex = 0;
		this.gridColumnEx15.Width = 100;
		this.gridColumnEx17.Caption = "Label";
		this.gridColumnEx17.CurrencyDateField = null;
		this.gridColumnEx17.CurrencyField = null;
		this.gridColumnEx17.CurrencyFixed = null;
		this.gridColumnEx17.CurrencyFixedRate = 0.0;
		this.gridColumnEx17.CurrencyRateField = null;
		this.gridColumnEx17.FieldName = "Label";
		this.gridColumnEx17.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx17.Name = "gridColumnEx17";
		this.gridColumnEx17.PrimaryKey = false;
		this.gridColumnEx17.UnitField = null;
		this.gridColumnEx17.Visible = true;
		this.gridColumnEx17.VisibleIndex = 1;
		this.gridColumnEx17.Width = 100;
		this.gridColumnEx18.Caption = "Data Type";
		this.gridColumnEx18.ColumnEdit = this.rep_ReportDataType;
		this.gridColumnEx18.CurrencyDateField = null;
		this.gridColumnEx18.CurrencyField = null;
		this.gridColumnEx18.CurrencyFixed = null;
		this.gridColumnEx18.CurrencyFixedRate = 0.0;
		this.gridColumnEx18.CurrencyRateField = null;
		this.gridColumnEx18.FieldName = "DataType";
		this.gridColumnEx18.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx18.Name = "gridColumnEx18";
		this.gridColumnEx18.PrimaryKey = false;
		this.gridColumnEx18.UnitField = null;
		this.gridColumnEx18.Visible = true;
		this.gridColumnEx18.VisibleIndex = 3;
		this.gridColumnEx18.Width = 100;
		this.rep_ReportDataType.AutoHeight = false;
		this.rep_ReportDataType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_ReportDataType.Name = "rep_ReportDataType";
		this.rep_ReportDataType.NullText = "";
		this.gridColumnEx19.Caption = "Source Field";
		this.gridColumnEx19.ColumnEdit = this.rep_SourceField;
		this.gridColumnEx19.CurrencyDateField = null;
		this.gridColumnEx19.CurrencyField = null;
		this.gridColumnEx19.CurrencyFixed = null;
		this.gridColumnEx19.CurrencyFixedRate = 0.0;
		this.gridColumnEx19.CurrencyRateField = null;
		this.gridColumnEx19.FieldName = "SourceField";
		this.gridColumnEx19.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx19.Name = "gridColumnEx19";
		this.gridColumnEx19.PrimaryKey = false;
		this.gridColumnEx19.UnitField = null;
		this.gridColumnEx19.Visible = true;
		this.gridColumnEx19.VisibleIndex = 2;
		this.gridColumnEx19.Width = 100;
		this.rep_SourceField.AutoHeight = false;
		this.rep_SourceField.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_SourceField.Name = "rep_SourceField";
		this.rep_SourceField.NullText = "";
		this.gridColumnEx20.Caption = "Sub. Type";
		this.gridColumnEx20.CurrencyDateField = null;
		this.gridColumnEx20.CurrencyField = null;
		this.gridColumnEx20.CurrencyFixed = null;
		this.gridColumnEx20.CurrencyFixedRate = 0.0;
		this.gridColumnEx20.CurrencyRateField = null;
		this.gridColumnEx20.FieldName = "SubType";
		this.gridColumnEx20.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx20.Name = "gridColumnEx20";
		this.gridColumnEx20.PrimaryKey = false;
		this.gridColumnEx20.UnitField = null;
		this.gridColumnEx20.Visible = true;
		this.gridColumnEx20.VisibleIndex = 4;
		this.gridColumnEx20.Width = 100;
		this.gridColumnEx21.Caption = "Default Value";
		this.gridColumnEx21.CurrencyDateField = null;
		this.gridColumnEx21.CurrencyField = null;
		this.gridColumnEx21.CurrencyFixed = null;
		this.gridColumnEx21.CurrencyFixedRate = 0.0;
		this.gridColumnEx21.CurrencyRateField = null;
		this.gridColumnEx21.FieldName = "DefaultValue";
		this.gridColumnEx21.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx21.Name = "gridColumnEx21";
		this.gridColumnEx21.PrimaryKey = false;
		this.gridColumnEx21.UnitField = null;
		this.gridColumnEx21.Visible = true;
		this.gridColumnEx21.VisibleIndex = 5;
		this.gridColumnEx21.Width = 100;
		this.gridColumnEx22.Caption = "Active";
		this.gridColumnEx22.ColumnEdit = this.repositoryItemCheckEdit2;
		this.gridColumnEx22.CurrencyDateField = null;
		this.gridColumnEx22.CurrencyField = null;
		this.gridColumnEx22.CurrencyFixed = null;
		this.gridColumnEx22.CurrencyFixedRate = 0.0;
		this.gridColumnEx22.CurrencyRateField = null;
		this.gridColumnEx22.FieldName = "Active";
		this.gridColumnEx22.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx22.Name = "gridColumnEx22";
		this.gridColumnEx22.PrimaryKey = false;
		this.gridColumnEx22.UnitField = null;
		this.gridColumnEx22.Visible = true;
		this.gridColumnEx22.VisibleIndex = 6;
		this.repositoryItemCheckEdit2.AutoHeight = false;
		this.repositoryItemCheckEdit2.Name = "repositoryItemCheckEdit2";
		this.repositoryItemCheckEdit2.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.repositoryItemCheckEdit2.ValueChecked = "Y";
		this.repositoryItemCheckEdit2.ValueGrayed = "N";
		this.repositoryItemCheckEdit2.ValueUnchecked = "N";
		this.memo_RptString.EditControl = null;
		this.memo_RptString.FieldName = null;
		this.memo_RptString.IsSearchControl = false;
		this.memo_RptString.Location = new System.Drawing.Point(431, 4);
		this.memo_RptString.Name = "memo_RptString";
		this.memo_RptString.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.memo_RptString.Properties.ShowIcon = false;
		this.memo_RptString.SettingInVisible = false;
		this.memo_RptString.SettingReadonly = false;
		this.memo_RptString.Size = new System.Drawing.Size(77, 20);
		this.memo_RptString.TabIndex = 8;
		this.memo_RptString.Visible = false;
		this.memo_RptString.EditValueChanged += new System.EventHandler(this.memo_RptString_EditValueChanged);
		this.bte_RptString.EditControl = null;
		this.bte_RptString.FieldName = null;
		this.bte_RptString.IsSearchControl = false;
		this.bte_RptString.Location = new System.Drawing.Point(193, 4);
		this.bte_RptString.Name = "bte_RptString";
		this.bte_RptString.Properties.AutoLoadDataSource = true;
		this.bte_RptString.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.bte_RptString.Properties.ContainerControl = null;
		this.bte_RptString.Properties.DisplayMember = "";
		this.bte_RptString.Properties.DisplayMember2 = "";
		this.bte_RptString.Properties.ListMandatoryField = null;
		this.bte_RptString.Properties.LoadDataSourceWhenEnter = false;
		this.bte_RptString.Properties.SeparateCharacterForMulti = ';';
		this.bte_RptString.Properties.SeparateString = ", ";
		this.bte_RptString.Properties.UsingAutoCompleteBox = false;
		this.bte_RptString.Properties.UsingDataSource = true;
		this.bte_RptString.Properties.ValidateWhenNoDataSource = false;
		this.bte_RptString.Properties.ValueMember = "";
		this.bte_RptString.SettingInVisible = false;
		this.bte_RptString.SettingReadonly = false;
		this.bte_RptString.Size = new System.Drawing.Size(232, 20);
		this.bte_RptString.TabIndex = 6;
		this.bte_RptString.EditValueChanged += new System.EventHandler(this.buttonEdit2_EditValueChanged);
		this.look_ReportFrom.Location = new System.Drawing.Point(87, 4);
		this.look_ReportFrom.Name = "look_ReportFrom";
		this.look_ReportFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.look_ReportFrom.Properties.NullText = "";
		this.look_ReportFrom.Size = new System.Drawing.Size(100, 20);
		this.look_ReportFrom.TabIndex = 5;
		this.look_ReportFrom.EditValueChanged += new System.EventHandler(this.look_ReportForm_EditValueChanged);
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "Report From";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(10, 7);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(80, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 4;
		this.labelLine2.Utext = "Report From";
		this.tab_Documents.Controls.Add(this.look_DocEntry);
		this.tab_Documents.Controls.Add(this.lb_DocumentDocEntry);
		this.tab_Documents.Controls.Add(this.chk_ShowDocForm);
		this.tab_Documents.Controls.Add(this.look_ObjectType);
		this.tab_Documents.Controls.Add(this.labelLine1);
		this.tab_Documents.Controls.Add(this.tabControl_MappingDocument);
		this.tab_Documents.EditControl = null;
		this.tab_Documents.EditValue = "Documents";
		this.tab_Documents.FieldName = "tab_Documents";
		this.tab_Documents.IsSearchControl = false;
		this.tab_Documents.Name = "tab_Documents";
		this.tab_Documents.ReadOnly = false;
		this.tab_Documents.SettingInVisible = false;
		this.tab_Documents.SettingReadonly = false;
		this.tab_Documents.Size = new System.Drawing.Size(938, 400);
		this.tab_Documents.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Documents.Text = "Documents";
		this.look_DocEntry.EditControl = null;
		this.look_DocEntry.FieldName = null;
		this.look_DocEntry.IsSearchControl = false;
		this.look_DocEntry.Location = new System.Drawing.Point(437, 2);
		this.look_DocEntry.Name = "look_DocEntry";
		this.look_DocEntry.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.look_DocEntry.Properties.NullText = "";
		this.look_DocEntry.SettingInVisible = false;
		this.look_DocEntry.SettingReadonly = false;
		this.look_DocEntry.Size = new System.Drawing.Size(141, 20);
		this.look_DocEntry.TabIndex = 5;
		this.look_DocEntry.EditValueChanged += new System.EventHandler(this.look_DocEntry_EditValueChanged);
		this.lb_DocumentDocEntry.EditControl = null;
		this.lb_DocumentDocEntry.EditValue = "Doc. Entry";
		this.lb_DocumentDocEntry.FieldName = null;
		this.lb_DocumentDocEntry.HideWhenEmpty = false;
		this.lb_DocumentDocEntry.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_DocumentDocEntry.Location = new System.Drawing.Point(306, 5);
		this.lb_DocumentDocEntry.Name = "lb_DocumentDocEntry";
		this.lb_DocumentDocEntry.ReadOnly = false;
		this.lb_DocumentDocEntry.SettingInVisible = false;
		this.lb_DocumentDocEntry.SettingReadonly = false;
		this.lb_DocumentDocEntry.ShowUnderLine = true;
		this.lb_DocumentDocEntry.Size = new System.Drawing.Size(132, 20);
		this.lb_DocumentDocEntry.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_DocumentDocEntry.TabIndex = 4;
		this.lb_DocumentDocEntry.Utext = "Doc. Entry";
		this.chk_ShowDocForm.EditControl = null;
		this.chk_ShowDocForm.EditValue = "Y";
		this.chk_ShowDocForm.FieldName = null;
		this.chk_ShowDocForm.IsSearchControl = false;
		this.chk_ShowDocForm.Location = new System.Drawing.Point(11, 31);
		this.chk_ShowDocForm.Name = "chk_ShowDocForm";
		this.chk_ShowDocForm.Properties.Caption = "Show Document Window";
		this.chk_ShowDocForm.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.chk_ShowDocForm.Properties.ValueChecked = "Y";
		this.chk_ShowDocForm.Properties.ValueGrayed = "N";
		this.chk_ShowDocForm.Properties.ValueUnchecked = "N";
		this.chk_ShowDocForm.SettingInVisible = false;
		this.chk_ShowDocForm.SettingReadonly = false;
		this.chk_ShowDocForm.Size = new System.Drawing.Size(174, 20);
		this.chk_ShowDocForm.TabIndex = 3;
		this.chk_ShowDocForm.CheckedChanged += new System.EventHandler(this.chk_ShowDocForm_CheckedChanged);
		this.look_ObjectType.EditControl = null;
		this.look_ObjectType.FieldName = null;
		this.look_ObjectType.IsSearchControl = false;
		this.look_ObjectType.Location = new System.Drawing.Point(142, 2);
		this.look_ObjectType.Name = "look_ObjectType";
		this.look_ObjectType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.look_ObjectType.Properties.NullText = "";
		this.look_ObjectType.SettingInVisible = false;
		this.look_ObjectType.SettingReadonly = false;
		this.look_ObjectType.Size = new System.Drawing.Size(141, 20);
		this.look_ObjectType.TabIndex = 2;
		this.look_ObjectType.EditValueChanged += new System.EventHandler(this.look_ObjectType_EditValueChanged);
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Business Object Type";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(11, 5);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(132, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 1;
		this.labelLine1.Utext = "Business Object Type";
		this.tabControl_MappingDocument.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.tabControl_MappingDocument.IsSearchControl = false;
		this.tabControl_MappingDocument.Location = new System.Drawing.Point(0, 56);
		this.tabControl_MappingDocument.Name = "tabControl_MappingDocument";
		this.tabControl_MappingDocument.SelectedTabPage = this.xtraTabPage5;
		this.tabControl_MappingDocument.Size = new System.Drawing.Size(936, 343);
		this.tabControl_MappingDocument.TabIndex = 0;
		this.tabControl_MappingDocument.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[3] { this.xtraTabPage5, this.xtraTabPage6, this.xtraTabPage1 });
		this.xtraTabPage5.Controls.Add(this.grd_Header);
		this.xtraTabPage5.EditControl = null;
		this.xtraTabPage5.EditValue = "Header";
		this.xtraTabPage5.FieldName = "xtraTabPage5";
		this.xtraTabPage5.IsSearchControl = false;
		this.xtraTabPage5.Name = "xtraTabPage5";
		this.xtraTabPage5.ReadOnly = false;
		this.xtraTabPage5.SettingInVisible = false;
		this.xtraTabPage5.SettingReadonly = false;
		this.xtraTabPage5.Size = new System.Drawing.Size(934, 318);
		this.xtraTabPage5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPage5.Text = "Header";
		this.grd_Header.AllowPasteExcel = false;
		this.grd_Header.ClipboardData = null;
		this.grd_Header.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Header.Location = new System.Drawing.Point(0, 0);
		this.grd_Header.MainView = this.gv_Header;
		this.grd_Header.Name = "grd_Header";
		this.grd_Header.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.rep_TargetField, this.rep_SourceFieldHeader, this.rep_SumTypeHeader });
		this.grd_Header.Size = new System.Drawing.Size(934, 318);
		this.grd_Header.TabIndex = 0;
		this.grd_Header.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Header });
		this.gv_Header.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Header.ColumnKey = null;
		this.gv_Header.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[6] { this.gridColumnEx1, this.gridColumnEx2, this.gridColumnEx3, this.gridColumnEx4, this.gridColumnEx23, this.gridColumnEx30 });
		this.gv_Header.DisableFormSetting = true;
		this.gv_Header.DisplayException = false;
		this.gv_Header.FieldCurrency = null;
		this.gv_Header.FieldRate = null;
		this.gv_Header.FormId = "oContextAction";
		this.gv_Header.GridControl = this.grd_Header;
		this.gv_Header.IndicatorWidth = 35;
		this.gv_Header.IsApzTable = false;
		this.gv_Header.IsInsertNewRow = false;
		this.gv_Header.IsLoadingData = false;
		this.gv_Header.ItemId = "OHEADER";
		this.gv_Header.LstColumnAllowBatch = null;
		this.gv_Header.LstColumnNotAllowBatch = null;
		this.gv_Header.LstColumnNotEditable = null;
		this.gv_Header.LstColumNotSetting = null;
		this.gv_Header.Name = "gv_Header";
		this.gv_Header.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Header.OptionsView.ColumnAutoWidth = false;
		this.gv_Header.OptionsView.ShowGroupPanel = false;
		this.gv_Header.UseBatchMod = false;
		this.gridColumnEx1.Caption = "AbsEntry";
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "AbsEntry";
		this.gridColumnEx1.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.PrimaryKey = false;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx2.Caption = "Source Field Name";
		this.gridColumnEx2.ColumnEdit = this.rep_SourceFieldHeader;
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.FieldName = "SourceField";
		this.gridColumnEx2.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.UnitField = null;
		this.gridColumnEx2.Visible = true;
		this.gridColumnEx2.VisibleIndex = 1;
		this.gridColumnEx2.Width = 121;
		this.rep_SourceFieldHeader.AutoHeight = false;
		this.rep_SourceFieldHeader.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_SourceFieldHeader.Name = "rep_SourceFieldHeader";
		this.rep_SourceFieldHeader.NullText = "";
		this.gridColumnEx3.Caption = "TargetItem";
		this.gridColumnEx3.CurrencyDateField = null;
		this.gridColumnEx3.CurrencyField = null;
		this.gridColumnEx3.CurrencyFixed = null;
		this.gridColumnEx3.CurrencyFixedRate = 0.0;
		this.gridColumnEx3.CurrencyRateField = null;
		this.gridColumnEx3.FieldName = "TargetItem";
		this.gridColumnEx3.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx3.Name = "gridColumnEx3";
		this.gridColumnEx3.PrimaryKey = false;
		this.gridColumnEx3.UnitField = null;
		this.gridColumnEx4.Caption = "Target Field Name";
		this.gridColumnEx4.ColumnEdit = this.rep_TargetField;
		this.gridColumnEx4.CurrencyDateField = null;
		this.gridColumnEx4.CurrencyField = null;
		this.gridColumnEx4.CurrencyFixed = null;
		this.gridColumnEx4.CurrencyFixedRate = 0.0;
		this.gridColumnEx4.CurrencyRateField = null;
		this.gridColumnEx4.FieldName = "TargetField";
		this.gridColumnEx4.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx4.Name = "gridColumnEx4";
		this.gridColumnEx4.OptionsColumn.AllowEdit = false;
		this.gridColumnEx4.PrimaryKey = false;
		this.gridColumnEx4.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gridColumnEx4.UnitField = null;
		this.gridColumnEx4.Visible = true;
		this.gridColumnEx4.VisibleIndex = 0;
		this.gridColumnEx4.Width = 114;
		this.rep_TargetField.AutoHeight = false;
		this.rep_TargetField.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_TargetField.Name = "rep_TargetField";
		this.rep_TargetField.NullText = "";
		this.gridColumnEx23.Caption = "Default Value";
		this.gridColumnEx23.CurrencyDateField = null;
		this.gridColumnEx23.CurrencyField = null;
		this.gridColumnEx23.CurrencyFixed = null;
		this.gridColumnEx23.CurrencyFixedRate = 0.0;
		this.gridColumnEx23.CurrencyRateField = null;
		this.gridColumnEx23.FieldName = "DfltValue";
		this.gridColumnEx23.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx23.Name = "gridColumnEx23";
		this.gridColumnEx23.PrimaryKey = false;
		this.gridColumnEx23.UnitField = null;
		this.gridColumnEx23.Visible = true;
		this.gridColumnEx23.VisibleIndex = 2;
		this.gridColumnEx23.Width = 175;
		this.gridColumnEx30.Caption = "Summary Type";
		this.gridColumnEx30.ColumnEdit = this.rep_SumTypeHeader;
		this.gridColumnEx30.CurrencyDateField = null;
		this.gridColumnEx30.CurrencyField = null;
		this.gridColumnEx30.CurrencyFixed = null;
		this.gridColumnEx30.CurrencyFixedRate = 0.0;
		this.gridColumnEx30.CurrencyRateField = null;
		this.gridColumnEx30.FieldName = "SumType";
		this.gridColumnEx30.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx30.Name = "gridColumnEx30";
		this.gridColumnEx30.PrimaryKey = false;
		this.gridColumnEx30.UnitField = null;
		this.gridColumnEx30.Visible = true;
		this.gridColumnEx30.VisibleIndex = 3;
		this.gridColumnEx30.Width = 100;
		this.rep_SumTypeHeader.AutoHeight = false;
		this.rep_SumTypeHeader.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_SumTypeHeader.Name = "rep_SumTypeHeader";
		this.rep_SumTypeHeader.NullText = "";
		this.xtraTabPage6.Controls.Add(this.grd_Items);
		this.xtraTabPage6.EditControl = null;
		this.xtraTabPage6.EditValue = "Items";
		this.xtraTabPage6.FieldName = "xtraTabPage6";
		this.xtraTabPage6.IsSearchControl = false;
		this.xtraTabPage6.Name = "xtraTabPage6";
		this.xtraTabPage6.ReadOnly = false;
		this.xtraTabPage6.SettingInVisible = false;
		this.xtraTabPage6.SettingReadonly = false;
		this.xtraTabPage6.Size = new System.Drawing.Size(934, 318);
		this.xtraTabPage6.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPage6.Text = "Items";
		this.grd_Items.AllowPasteExcel = false;
		this.grd_Items.ClipboardData = null;
		this.grd_Items.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Items.Location = new System.Drawing.Point(0, 0);
		this.grd_Items.MainView = this.gv_Items;
		this.grd_Items.Name = "grd_Items";
		this.grd_Items.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.rep_TargetFieldLine, this.rep_SourceFieldItem, this.rep_SumTypeLine });
		this.grd_Items.Size = new System.Drawing.Size(934, 318);
		this.grd_Items.TabIndex = 1;
		this.grd_Items.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Items });
		this.gv_Items.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Items.ColumnKey = null;
		this.gv_Items.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[6] { this.gridColumnEx5, this.gridColumnEx6, this.gridColumnEx7, this.gridColumnEx8, this.gridColumnEx24, this.gridColumnEx31 });
		this.gv_Items.DisableFormSetting = true;
		this.gv_Items.DisplayException = false;
		this.gv_Items.FieldCurrency = null;
		this.gv_Items.FieldRate = null;
		this.gv_Items.FormId = "oContextAction";
		this.gv_Items.GridControl = this.grd_Items;
		this.gv_Items.IndicatorWidth = 35;
		this.gv_Items.IsApzTable = false;
		this.gv_Items.IsInsertNewRow = false;
		this.gv_Items.IsLoadingData = false;
		this.gv_Items.ItemId = "OITEMS";
		this.gv_Items.LstColumnAllowBatch = null;
		this.gv_Items.LstColumnNotAllowBatch = null;
		this.gv_Items.LstColumnNotEditable = null;
		this.gv_Items.LstColumNotSetting = null;
		this.gv_Items.Name = "gv_Items";
		this.gv_Items.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Items.OptionsView.ColumnAutoWidth = false;
		this.gv_Items.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Items.OptionsView.ShowGroupPanel = false;
		this.gv_Items.UseBatchMod = false;
		this.gridColumnEx5.Caption = "AbsEntry";
		this.gridColumnEx5.CurrencyDateField = null;
		this.gridColumnEx5.CurrencyField = null;
		this.gridColumnEx5.CurrencyFixed = null;
		this.gridColumnEx5.CurrencyFixedRate = 0.0;
		this.gridColumnEx5.CurrencyRateField = null;
		this.gridColumnEx5.FieldName = "AbsEntry";
		this.gridColumnEx5.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx5.Name = "gridColumnEx5";
		this.gridColumnEx5.PrimaryKey = false;
		this.gridColumnEx5.UnitField = null;
		this.gridColumnEx6.Caption = "Source Field Name";
		this.gridColumnEx6.ColumnEdit = this.rep_SourceFieldItem;
		this.gridColumnEx6.CurrencyDateField = null;
		this.gridColumnEx6.CurrencyField = null;
		this.gridColumnEx6.CurrencyFixed = null;
		this.gridColumnEx6.CurrencyFixedRate = 0.0;
		this.gridColumnEx6.CurrencyRateField = null;
		this.gridColumnEx6.FieldName = "SourceField";
		this.gridColumnEx6.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx6.Name = "gridColumnEx6";
		this.gridColumnEx6.PrimaryKey = false;
		this.gridColumnEx6.UnitField = null;
		this.gridColumnEx6.Visible = true;
		this.gridColumnEx6.VisibleIndex = 1;
		this.gridColumnEx6.Width = 121;
		this.rep_SourceFieldItem.AutoHeight = false;
		this.rep_SourceFieldItem.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_SourceFieldItem.Name = "rep_SourceFieldItem";
		this.rep_SourceFieldItem.NullText = "";
		this.gridColumnEx7.Caption = "TargetItem";
		this.gridColumnEx7.CurrencyDateField = null;
		this.gridColumnEx7.CurrencyField = null;
		this.gridColumnEx7.CurrencyFixed = null;
		this.gridColumnEx7.CurrencyFixedRate = 0.0;
		this.gridColumnEx7.CurrencyRateField = null;
		this.gridColumnEx7.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx7.Name = "gridColumnEx7";
		this.gridColumnEx7.PrimaryKey = false;
		this.gridColumnEx7.UnitField = null;
		this.gridColumnEx8.Caption = "Target Field Name";
		this.gridColumnEx8.ColumnEdit = this.rep_TargetFieldLine;
		this.gridColumnEx8.CurrencyDateField = null;
		this.gridColumnEx8.CurrencyField = null;
		this.gridColumnEx8.CurrencyFixed = null;
		this.gridColumnEx8.CurrencyFixedRate = 0.0;
		this.gridColumnEx8.CurrencyRateField = null;
		this.gridColumnEx8.FieldName = "TargetField";
		this.gridColumnEx8.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx8.Name = "gridColumnEx8";
		this.gridColumnEx8.OptionsColumn.AllowEdit = false;
		this.gridColumnEx8.PrimaryKey = false;
		this.gridColumnEx8.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gridColumnEx8.UnitField = null;
		this.gridColumnEx8.Visible = true;
		this.gridColumnEx8.VisibleIndex = 0;
		this.gridColumnEx8.Width = 114;
		this.rep_TargetFieldLine.AutoHeight = false;
		this.rep_TargetFieldLine.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_TargetFieldLine.Name = "rep_TargetFieldLine";
		this.rep_TargetFieldLine.NullText = "";
		this.gridColumnEx24.Caption = "Default Value";
		this.gridColumnEx24.CurrencyDateField = null;
		this.gridColumnEx24.CurrencyField = null;
		this.gridColumnEx24.CurrencyFixed = null;
		this.gridColumnEx24.CurrencyFixedRate = 0.0;
		this.gridColumnEx24.CurrencyRateField = null;
		this.gridColumnEx24.FieldName = "DfltValue";
		this.gridColumnEx24.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx24.Name = "gridColumnEx24";
		this.gridColumnEx24.PrimaryKey = false;
		this.gridColumnEx24.UnitField = null;
		this.gridColumnEx24.Visible = true;
		this.gridColumnEx24.VisibleIndex = 2;
		this.gridColumnEx24.Width = 175;
		this.gridColumnEx31.Caption = "Summary Type";
		this.gridColumnEx31.ColumnEdit = this.rep_SumTypeLine;
		this.gridColumnEx31.CurrencyDateField = null;
		this.gridColumnEx31.CurrencyField = null;
		this.gridColumnEx31.CurrencyFixed = null;
		this.gridColumnEx31.CurrencyFixedRate = 0.0;
		this.gridColumnEx31.CurrencyRateField = null;
		this.gridColumnEx31.FieldName = "SumType";
		this.gridColumnEx31.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx31.Name = "gridColumnEx31";
		this.gridColumnEx31.PrimaryKey = false;
		this.gridColumnEx31.UnitField = null;
		this.gridColumnEx31.Visible = true;
		this.gridColumnEx31.VisibleIndex = 3;
		this.gridColumnEx31.Width = 100;
		this.rep_SumTypeLine.AutoHeight = false;
		this.rep_SumTypeLine.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_SumTypeLine.Name = "rep_SumTypeLine";
		this.rep_SumTypeLine.NullText = "";
		this.xtraTabPage1.Controls.Add(this.grd_Batch);
		this.xtraTabPage1.EditControl = null;
		this.xtraTabPage1.EditValue = "Batch/ Serial/ Binlocation";
		this.xtraTabPage1.FieldName = "xtraTabPage1";
		this.xtraTabPage1.IsSearchControl = false;
		this.xtraTabPage1.Name = "xtraTabPage1";
		this.xtraTabPage1.ReadOnly = false;
		this.xtraTabPage1.SettingInVisible = false;
		this.xtraTabPage1.SettingReadonly = false;
		this.xtraTabPage1.Size = new System.Drawing.Size(934, 318);
		this.xtraTabPage1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPage1.Text = "Batch/ Serial/ Binlocation";
		this.grd_Batch.AllowPasteExcel = false;
		this.grd_Batch.ClipboardData = null;
		this.grd_Batch.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Batch.Location = new System.Drawing.Point(0, 0);
		this.grd_Batch.MainView = this.gv_Batch;
		this.grd_Batch.Name = "grd_Batch";
		this.grd_Batch.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.rep_TargetFieldBatch, this.rep_SourceFieldBatch, this.rep_SumTypeBatch });
		this.grd_Batch.Size = new System.Drawing.Size(934, 318);
		this.grd_Batch.TabIndex = 2;
		this.grd_Batch.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Batch });
		this.gv_Batch.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Batch.ColumnKey = null;
		this.gv_Batch.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[6] { this.gridColumnEx25, this.gridColumnEx26, this.gridColumnEx27, this.gridColumnEx28, this.gridColumnEx29, this.gridColumnEx32 });
		this.gv_Batch.DisableFormSetting = true;
		this.gv_Batch.DisplayException = false;
		this.gv_Batch.FieldCurrency = null;
		this.gv_Batch.FieldRate = null;
		this.gv_Batch.FormId = "oContextAction";
		this.gv_Batch.GridControl = this.grd_Batch;
		this.gv_Batch.IndicatorWidth = 35;
		this.gv_Batch.IsApzTable = false;
		this.gv_Batch.IsInsertNewRow = false;
		this.gv_Batch.IsLoadingData = false;
		this.gv_Batch.ItemId = "OBATCH";
		this.gv_Batch.LstColumnAllowBatch = null;
		this.gv_Batch.LstColumnNotAllowBatch = null;
		this.gv_Batch.LstColumnNotEditable = null;
		this.gv_Batch.LstColumNotSetting = null;
		this.gv_Batch.Name = "gv_Batch";
		this.gv_Batch.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Batch.OptionsView.ColumnAutoWidth = false;
		this.gv_Batch.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Batch.OptionsView.ShowGroupPanel = false;
		this.gv_Batch.UseBatchMod = false;
		this.gridColumnEx25.Caption = "AbsEntry";
		this.gridColumnEx25.CurrencyDateField = null;
		this.gridColumnEx25.CurrencyField = null;
		this.gridColumnEx25.CurrencyFixed = null;
		this.gridColumnEx25.CurrencyFixedRate = 0.0;
		this.gridColumnEx25.CurrencyRateField = null;
		this.gridColumnEx25.FieldName = "AbsEntry";
		this.gridColumnEx25.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx25.Name = "gridColumnEx25";
		this.gridColumnEx25.PrimaryKey = false;
		this.gridColumnEx25.UnitField = null;
		this.gridColumnEx26.Caption = "Source Field Name";
		this.gridColumnEx26.ColumnEdit = this.rep_SourceFieldBatch;
		this.gridColumnEx26.CurrencyDateField = null;
		this.gridColumnEx26.CurrencyField = null;
		this.gridColumnEx26.CurrencyFixed = null;
		this.gridColumnEx26.CurrencyFixedRate = 0.0;
		this.gridColumnEx26.CurrencyRateField = null;
		this.gridColumnEx26.FieldName = "SourceField";
		this.gridColumnEx26.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx26.Name = "gridColumnEx26";
		this.gridColumnEx26.PrimaryKey = false;
		this.gridColumnEx26.UnitField = null;
		this.gridColumnEx26.Visible = true;
		this.gridColumnEx26.VisibleIndex = 1;
		this.gridColumnEx26.Width = 121;
		this.rep_SourceFieldBatch.AutoHeight = false;
		this.rep_SourceFieldBatch.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_SourceFieldBatch.Name = "rep_SourceFieldBatch";
		this.rep_SourceFieldBatch.NullText = "";
		this.gridColumnEx27.Caption = "TargetItem";
		this.gridColumnEx27.CurrencyDateField = null;
		this.gridColumnEx27.CurrencyField = null;
		this.gridColumnEx27.CurrencyFixed = null;
		this.gridColumnEx27.CurrencyFixedRate = 0.0;
		this.gridColumnEx27.CurrencyRateField = null;
		this.gridColumnEx27.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx27.Name = "gridColumnEx27";
		this.gridColumnEx27.PrimaryKey = false;
		this.gridColumnEx27.UnitField = null;
		this.gridColumnEx28.Caption = "Target Field Name";
		this.gridColumnEx28.ColumnEdit = this.rep_TargetFieldBatch;
		this.gridColumnEx28.CurrencyDateField = null;
		this.gridColumnEx28.CurrencyField = null;
		this.gridColumnEx28.CurrencyFixed = null;
		this.gridColumnEx28.CurrencyFixedRate = 0.0;
		this.gridColumnEx28.CurrencyRateField = null;
		this.gridColumnEx28.FieldName = "TargetField";
		this.gridColumnEx28.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx28.Name = "gridColumnEx28";
		this.gridColumnEx28.OptionsColumn.AllowEdit = false;
		this.gridColumnEx28.PrimaryKey = false;
		this.gridColumnEx28.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gridColumnEx28.UnitField = null;
		this.gridColumnEx28.Visible = true;
		this.gridColumnEx28.VisibleIndex = 0;
		this.gridColumnEx28.Width = 114;
		this.rep_TargetFieldBatch.AutoHeight = false;
		this.rep_TargetFieldBatch.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_TargetFieldBatch.Name = "rep_TargetFieldBatch";
		this.rep_TargetFieldBatch.NullText = "";
		this.gridColumnEx29.Caption = "Default Value";
		this.gridColumnEx29.CurrencyDateField = null;
		this.gridColumnEx29.CurrencyField = null;
		this.gridColumnEx29.CurrencyFixed = null;
		this.gridColumnEx29.CurrencyFixedRate = 0.0;
		this.gridColumnEx29.CurrencyRateField = null;
		this.gridColumnEx29.FieldName = "DfltValue";
		this.gridColumnEx29.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx29.Name = "gridColumnEx29";
		this.gridColumnEx29.PrimaryKey = false;
		this.gridColumnEx29.UnitField = null;
		this.gridColumnEx29.Visible = true;
		this.gridColumnEx29.VisibleIndex = 2;
		this.gridColumnEx29.Width = 175;
		this.gridColumnEx32.Caption = "Summary Type";
		this.gridColumnEx32.ColumnEdit = this.rep_SumTypeBatch;
		this.gridColumnEx32.CurrencyDateField = null;
		this.gridColumnEx32.CurrencyField = null;
		this.gridColumnEx32.CurrencyFixed = null;
		this.gridColumnEx32.CurrencyFixedRate = 0.0;
		this.gridColumnEx32.CurrencyRateField = null;
		this.gridColumnEx32.FieldName = "SumType";
		this.gridColumnEx32.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx32.Name = "gridColumnEx32";
		this.gridColumnEx32.PrimaryKey = false;
		this.gridColumnEx32.UnitField = null;
		this.gridColumnEx32.Visible = true;
		this.gridColumnEx32.VisibleIndex = 3;
		this.gridColumnEx32.Width = 100;
		this.rep_SumTypeBatch.AutoHeight = false;
		this.rep_SumTypeBatch.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_SumTypeBatch.Name = "rep_SumTypeBatch";
		this.rep_SumTypeBatch.NullText = "";
		this.tab_Udo.Controls.Add(this.chk_ShowUdoForm);
		this.tab_Udo.Controls.Add(this.look_UdoCode);
		this.tab_Udo.Controls.Add(this.labelLine3);
		this.tab_Udo.Controls.Add(this.tabControl_Udo);
		this.tab_Udo.EditControl = null;
		this.tab_Udo.EditValue = "User-Defined Object";
		this.tab_Udo.FieldName = "tab_Udo";
		this.tab_Udo.IsSearchControl = false;
		this.tab_Udo.Name = "tab_Udo";
		this.tab_Udo.ReadOnly = false;
		this.tab_Udo.SettingInVisible = false;
		this.tab_Udo.SettingReadonly = false;
		this.tab_Udo.Size = new System.Drawing.Size(938, 400);
		this.tab_Udo.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Udo.Text = "User-Defined Object";
		this.chk_ShowUdoForm.EditControl = null;
		this.chk_ShowUdoForm.EditValue = "Y";
		this.chk_ShowUdoForm.FieldName = null;
		this.chk_ShowUdoForm.IsSearchControl = false;
		this.chk_ShowUdoForm.Location = new System.Drawing.Point(10, 28);
		this.chk_ShowUdoForm.Name = "chk_ShowUdoForm";
		this.chk_ShowUdoForm.Properties.Caption = "Show Udo Window";
		this.chk_ShowUdoForm.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.chk_ShowUdoForm.Properties.ValueChecked = "Y";
		this.chk_ShowUdoForm.Properties.ValueGrayed = "N";
		this.chk_ShowUdoForm.Properties.ValueUnchecked = "N";
		this.chk_ShowUdoForm.SettingInVisible = false;
		this.chk_ShowUdoForm.SettingReadonly = false;
		this.chk_ShowUdoForm.Size = new System.Drawing.Size(174, 20);
		this.chk_ShowUdoForm.TabIndex = 6;
		this.chk_ShowUdoForm.CheckedChanged += new System.EventHandler(this.chk_ShowUdoForm_CheckedChanged);
		this.look_UdoCode.EditControl = null;
		this.look_UdoCode.FieldName = null;
		this.look_UdoCode.IsSearchControl = false;
		this.look_UdoCode.Location = new System.Drawing.Point(141, 2);
		this.look_UdoCode.Name = "look_UdoCode";
		this.look_UdoCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.look_UdoCode.Properties.NullText = "";
		this.look_UdoCode.SettingInVisible = false;
		this.look_UdoCode.SettingReadonly = false;
		this.look_UdoCode.Size = new System.Drawing.Size(141, 20);
		this.look_UdoCode.TabIndex = 4;
		this.look_UdoCode.EditValueChanged += new System.EventHandler(this.look_UdoCode_EditValueChanged);
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "UDO Name";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(10, 5);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(132, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 3;
		this.labelLine3.Utext = "UDO Name";
		this.tabControl_Udo.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.tabControl_Udo.IsSearchControl = false;
		this.tabControl_Udo.Location = new System.Drawing.Point(2, 53);
		this.tabControl_Udo.Name = "tabControl_Udo";
		this.tabControl_Udo.Size = new System.Drawing.Size(917, 337);
		this.tabControl_Udo.TabIndex = 5;
		this.tab_Dashboard.Controls.Add(this.btn_ChooseDashboard);
		this.tab_Dashboard.Controls.Add(this.ucDashboardViewer1);
		this.tab_Dashboard.Controls.Add(this.btn_EditDesign);
		this.tab_Dashboard.EditControl = null;
		this.tab_Dashboard.EditValue = "Dashboard";
		this.tab_Dashboard.FieldName = "tab_Dashboard";
		this.tab_Dashboard.IsSearchControl = false;
		this.tab_Dashboard.Name = "tab_Dashboard";
		this.tab_Dashboard.ReadOnly = false;
		this.tab_Dashboard.SettingInVisible = false;
		this.tab_Dashboard.SettingReadonly = false;
		this.tab_Dashboard.Size = new System.Drawing.Size(938, 400);
		this.tab_Dashboard.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Dashboard.Text = "Dashboard";
		this.btn_ChooseDashboard.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.btn_ChooseDashboard.EditControl = null;
		this.btn_ChooseDashboard.EditValue = "Choose Existing Item";
		this.btn_ChooseDashboard.FieldName = null;
		this.btn_ChooseDashboard.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_ChooseDashboard.ImageOptions.Image");
		this.btn_ChooseDashboard.IsSearchControl = false;
		this.btn_ChooseDashboard.Location = new System.Drawing.Point(153, 8);
		this.btn_ChooseDashboard.Name = "btn_ChooseDashboard";
		this.btn_ChooseDashboard.ReadOnly = false;
		this.btn_ChooseDashboard.SettingInVisible = false;
		this.btn_ChooseDashboard.SettingReadonly = false;
		this.btn_ChooseDashboard.Size = new System.Drawing.Size(129, 23);
		this.btn_ChooseDashboard.TabIndex = 3;
		this.btn_ChooseDashboard.Text = "Choose Existing Item";
		this.btn_ChooseDashboard.Click += new System.EventHandler(this.btn_ChooseDashboard_Click);
		this.ucDashboardViewer1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.ucDashboardViewer1.DashboardName = null;
		this.ucDashboardViewer1.Location = new System.Drawing.Point(0, 37);
		this.ucDashboardViewer1.Name = "ucDashboardViewer1";
		this.ucDashboardViewer1.Size = new System.Drawing.Size(925, 360);
		this.ucDashboardViewer1.TabIndex = 2;
		this.btn_EditDesign.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.btn_EditDesign.EditControl = null;
		this.btn_EditDesign.EditValue = "Create New Dashboard";
		this.btn_EditDesign.FieldName = null;
		this.btn_EditDesign.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_EditDesign.ImageOptions.Image");
		this.btn_EditDesign.IsSearchControl = false;
		this.btn_EditDesign.Location = new System.Drawing.Point(4, 8);
		this.btn_EditDesign.Name = "btn_EditDesign";
		this.btn_EditDesign.ReadOnly = false;
		this.btn_EditDesign.SettingInVisible = false;
		this.btn_EditDesign.SettingReadonly = false;
		this.btn_EditDesign.Size = new System.Drawing.Size(140, 23);
		this.btn_EditDesign.TabIndex = 0;
		this.btn_EditDesign.Text = "Create New Dashboard";
		this.btn_EditDesign.Click += new System.EventHandler(this.btn_EditDesign_Click);
		this.tab_ProductionOrder.Controls.Add(this.look_ProductionEntry);
		this.tab_ProductionOrder.Controls.Add(this.lb_ProductionEntry);
		this.tab_ProductionOrder.Controls.Add(this.tabControl_MappingProduction);
		this.tab_ProductionOrder.Controls.Add(this.chk_ShowProductionForm);
		this.tab_ProductionOrder.EditControl = null;
		this.tab_ProductionOrder.EditValue = "Production Order";
		this.tab_ProductionOrder.FieldName = "tab_ProductionOrder";
		this.tab_ProductionOrder.IsSearchControl = false;
		this.tab_ProductionOrder.Name = "tab_ProductionOrder";
		this.tab_ProductionOrder.ReadOnly = false;
		this.tab_ProductionOrder.SettingInVisible = false;
		this.tab_ProductionOrder.SettingReadonly = false;
		this.tab_ProductionOrder.Size = new System.Drawing.Size(938, 400);
		this.tab_ProductionOrder.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_ProductionOrder.Text = "Production Order";
		this.look_ProductionEntry.EditControl = null;
		this.look_ProductionEntry.FieldName = null;
		this.look_ProductionEntry.IsSearchControl = false;
		this.look_ProductionEntry.Location = new System.Drawing.Point(251, 3);
		this.look_ProductionEntry.Name = "look_ProductionEntry";
		this.look_ProductionEntry.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.look_ProductionEntry.Properties.NullText = "";
		this.look_ProductionEntry.SettingInVisible = false;
		this.look_ProductionEntry.SettingReadonly = false;
		this.look_ProductionEntry.Size = new System.Drawing.Size(141, 20);
		this.look_ProductionEntry.TabIndex = 7;
		this.look_ProductionEntry.EditValueChanged += new System.EventHandler(this.look_ProductionEntry_EditValueChanged);
		this.lb_ProductionEntry.EditControl = null;
		this.lb_ProductionEntry.EditValue = "Doc. Entry";
		this.lb_ProductionEntry.FieldName = null;
		this.lb_ProductionEntry.HideWhenEmpty = false;
		this.lb_ProductionEntry.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_ProductionEntry.Location = new System.Drawing.Point(120, 6);
		this.lb_ProductionEntry.Name = "lb_ProductionEntry";
		this.lb_ProductionEntry.ReadOnly = false;
		this.lb_ProductionEntry.SettingInVisible = false;
		this.lb_ProductionEntry.SettingReadonly = false;
		this.lb_ProductionEntry.ShowUnderLine = true;
		this.lb_ProductionEntry.Size = new System.Drawing.Size(132, 20);
		this.lb_ProductionEntry.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_ProductionEntry.TabIndex = 6;
		this.lb_ProductionEntry.Utext = "Doc. Entry";
		this.tabControl_MappingProduction.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.tabControl_MappingProduction.IsSearchControl = false;
		this.tabControl_MappingProduction.Location = new System.Drawing.Point(0, 28);
		this.tabControl_MappingProduction.Name = "tabControl_MappingProduction";
		this.tabControl_MappingProduction.SelectedTabPage = this.xtraTabPage2;
		this.tabControl_MappingProduction.Size = new System.Drawing.Size(925, 369);
		this.tabControl_MappingProduction.TabIndex = 5;
		this.tabControl_MappingProduction.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[3] { this.xtraTabPage2, this.xtraTabPage3, this.xtraTabPage4 });
		this.xtraTabPage2.Controls.Add(this.grd_ProductionHeader);
		this.xtraTabPage2.EditControl = null;
		this.xtraTabPage2.EditValue = "Header";
		this.xtraTabPage2.FieldName = "xtraTabPage2";
		this.xtraTabPage2.IsSearchControl = false;
		this.xtraTabPage2.Name = "xtraTabPage2";
		this.xtraTabPage2.ReadOnly = false;
		this.xtraTabPage2.SettingInVisible = false;
		this.xtraTabPage2.SettingReadonly = false;
		this.xtraTabPage2.Size = new System.Drawing.Size(923, 344);
		this.xtraTabPage2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPage2.Text = "Header";
		this.grd_ProductionHeader.AllowPasteExcel = false;
		this.grd_ProductionHeader.ClipboardData = null;
		this.grd_ProductionHeader.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_ProductionHeader.Location = new System.Drawing.Point(0, 0);
		this.grd_ProductionHeader.MainView = this.gv_ProductionHeader;
		this.grd_ProductionHeader.Name = "grd_ProductionHeader";
		this.grd_ProductionHeader.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.rep_ProductionTargetHeader, this.rep_ProductionHeaderSourceField, this.rep_ProductionHeaderSumType });
		this.grd_ProductionHeader.Size = new System.Drawing.Size(923, 344);
		this.grd_ProductionHeader.TabIndex = 0;
		this.grd_ProductionHeader.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_ProductionHeader });
		this.gv_ProductionHeader.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_ProductionHeader.ColumnKey = null;
		this.gv_ProductionHeader.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[6] { this.gridColumnEx33, this.gridColumnEx34, this.gridColumnEx35, this.gridColumnEx36, this.gridColumnEx37, this.gridColumnEx38 });
		this.gv_ProductionHeader.DisableFormSetting = true;
		this.gv_ProductionHeader.DisplayException = false;
		this.gv_ProductionHeader.FieldCurrency = null;
		this.gv_ProductionHeader.FieldRate = null;
		this.gv_ProductionHeader.FormId = "oContextAction";
		this.gv_ProductionHeader.GridControl = this.grd_ProductionHeader;
		this.gv_ProductionHeader.IndicatorWidth = 35;
		this.gv_ProductionHeader.IsApzTable = false;
		this.gv_ProductionHeader.IsInsertNewRow = false;
		this.gv_ProductionHeader.IsLoadingData = false;
		this.gv_ProductionHeader.ItemId = "OPRODUCTIONHEADER";
		this.gv_ProductionHeader.LstColumnAllowBatch = null;
		this.gv_ProductionHeader.LstColumnNotAllowBatch = null;
		this.gv_ProductionHeader.LstColumnNotEditable = null;
		this.gv_ProductionHeader.LstColumNotSetting = null;
		this.gv_ProductionHeader.Name = "gv_ProductionHeader";
		this.gv_ProductionHeader.OptionsView.BestFitMaxRowCount = 100;
		this.gv_ProductionHeader.OptionsView.ColumnAutoWidth = false;
		this.gv_ProductionHeader.OptionsView.ShowGroupPanel = false;
		this.gv_ProductionHeader.UseBatchMod = false;
		this.gridColumnEx33.Caption = "AbsEntry";
		this.gridColumnEx33.CurrencyDateField = null;
		this.gridColumnEx33.CurrencyField = null;
		this.gridColumnEx33.CurrencyFixed = null;
		this.gridColumnEx33.CurrencyFixedRate = 0.0;
		this.gridColumnEx33.CurrencyRateField = null;
		this.gridColumnEx33.FieldName = "AbsEntry";
		this.gridColumnEx33.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx33.Name = "gridColumnEx33";
		this.gridColumnEx33.PrimaryKey = false;
		this.gridColumnEx33.UnitField = null;
		this.gridColumnEx34.Caption = "Source Field Name";
		this.gridColumnEx34.ColumnEdit = this.rep_ProductionHeaderSourceField;
		this.gridColumnEx34.CurrencyDateField = null;
		this.gridColumnEx34.CurrencyField = null;
		this.gridColumnEx34.CurrencyFixed = null;
		this.gridColumnEx34.CurrencyFixedRate = 0.0;
		this.gridColumnEx34.CurrencyRateField = null;
		this.gridColumnEx34.FieldName = "SourceField";
		this.gridColumnEx34.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx34.Name = "gridColumnEx34";
		this.gridColumnEx34.PrimaryKey = false;
		this.gridColumnEx34.UnitField = null;
		this.gridColumnEx34.Visible = true;
		this.gridColumnEx34.VisibleIndex = 1;
		this.gridColumnEx34.Width = 121;
		this.rep_ProductionHeaderSourceField.AutoHeight = false;
		this.rep_ProductionHeaderSourceField.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_ProductionHeaderSourceField.Name = "rep_ProductionHeaderSourceField";
		this.rep_ProductionHeaderSourceField.NullText = "";
		this.gridColumnEx35.Caption = "TargetItem";
		this.gridColumnEx35.CurrencyDateField = null;
		this.gridColumnEx35.CurrencyField = null;
		this.gridColumnEx35.CurrencyFixed = null;
		this.gridColumnEx35.CurrencyFixedRate = 0.0;
		this.gridColumnEx35.CurrencyRateField = null;
		this.gridColumnEx35.FieldName = "TargetItem";
		this.gridColumnEx35.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx35.Name = "gridColumnEx35";
		this.gridColumnEx35.PrimaryKey = false;
		this.gridColumnEx35.UnitField = null;
		this.gridColumnEx36.Caption = "Target Field Name";
		this.gridColumnEx36.ColumnEdit = this.rep_ProductionTargetHeader;
		this.gridColumnEx36.CurrencyDateField = null;
		this.gridColumnEx36.CurrencyField = null;
		this.gridColumnEx36.CurrencyFixed = null;
		this.gridColumnEx36.CurrencyFixedRate = 0.0;
		this.gridColumnEx36.CurrencyRateField = null;
		this.gridColumnEx36.FieldName = "TargetField";
		this.gridColumnEx36.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx36.Name = "gridColumnEx36";
		this.gridColumnEx36.OptionsColumn.AllowEdit = false;
		this.gridColumnEx36.PrimaryKey = false;
		this.gridColumnEx36.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gridColumnEx36.UnitField = null;
		this.gridColumnEx36.Visible = true;
		this.gridColumnEx36.VisibleIndex = 0;
		this.gridColumnEx36.Width = 114;
		this.rep_ProductionTargetHeader.AutoHeight = false;
		this.rep_ProductionTargetHeader.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_ProductionTargetHeader.Name = "rep_ProductionTargetHeader";
		this.rep_ProductionTargetHeader.NullText = "";
		this.gridColumnEx37.Caption = "Default Value";
		this.gridColumnEx37.CurrencyDateField = null;
		this.gridColumnEx37.CurrencyField = null;
		this.gridColumnEx37.CurrencyFixed = null;
		this.gridColumnEx37.CurrencyFixedRate = 0.0;
		this.gridColumnEx37.CurrencyRateField = null;
		this.gridColumnEx37.FieldName = "DfltValue";
		this.gridColumnEx37.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx37.Name = "gridColumnEx37";
		this.gridColumnEx37.PrimaryKey = false;
		this.gridColumnEx37.UnitField = null;
		this.gridColumnEx37.Visible = true;
		this.gridColumnEx37.VisibleIndex = 2;
		this.gridColumnEx37.Width = 175;
		this.gridColumnEx38.Caption = "Summary Type";
		this.gridColumnEx38.ColumnEdit = this.rep_ProductionHeaderSumType;
		this.gridColumnEx38.CurrencyDateField = null;
		this.gridColumnEx38.CurrencyField = null;
		this.gridColumnEx38.CurrencyFixed = null;
		this.gridColumnEx38.CurrencyFixedRate = 0.0;
		this.gridColumnEx38.CurrencyRateField = null;
		this.gridColumnEx38.FieldName = "SumType";
		this.gridColumnEx38.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx38.Name = "gridColumnEx38";
		this.gridColumnEx38.PrimaryKey = false;
		this.gridColumnEx38.UnitField = null;
		this.gridColumnEx38.Visible = true;
		this.gridColumnEx38.VisibleIndex = 3;
		this.gridColumnEx38.Width = 100;
		this.rep_ProductionHeaderSumType.AutoHeight = false;
		this.rep_ProductionHeaderSumType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_ProductionHeaderSumType.Name = "rep_ProductionHeaderSumType";
		this.rep_ProductionHeaderSumType.NullText = "";
		this.xtraTabPage3.Controls.Add(this.grd_ProductionItem);
		this.xtraTabPage3.EditControl = null;
		this.xtraTabPage3.EditValue = "Items";
		this.xtraTabPage3.FieldName = "xtraTabPage3";
		this.xtraTabPage3.IsSearchControl = false;
		this.xtraTabPage3.Name = "xtraTabPage3";
		this.xtraTabPage3.ReadOnly = false;
		this.xtraTabPage3.SettingInVisible = false;
		this.xtraTabPage3.SettingReadonly = false;
		this.xtraTabPage3.Size = new System.Drawing.Size(923, 344);
		this.xtraTabPage3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPage3.Text = "Items";
		this.grd_ProductionItem.AllowPasteExcel = false;
		this.grd_ProductionItem.ClipboardData = null;
		this.grd_ProductionItem.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_ProductionItem.Location = new System.Drawing.Point(0, 0);
		this.grd_ProductionItem.MainView = this.gv_ProductionItem;
		this.grd_ProductionItem.Name = "grd_ProductionItem";
		this.grd_ProductionItem.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.rep_ProductionOrderTargetLine, this.rep_ProductionLineSourceField, this.rep_ProductionLineSumType });
		this.grd_ProductionItem.Size = new System.Drawing.Size(923, 344);
		this.grd_ProductionItem.TabIndex = 1;
		this.grd_ProductionItem.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_ProductionItem });
		this.gv_ProductionItem.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_ProductionItem.ColumnKey = null;
		this.gv_ProductionItem.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[6] { this.gridColumnEx39, this.gridColumnEx40, this.gridColumnEx41, this.gridColumnEx42, this.gridColumnEx43, this.gridColumnEx44 });
		this.gv_ProductionItem.DisableFormSetting = true;
		this.gv_ProductionItem.DisplayException = false;
		this.gv_ProductionItem.FieldCurrency = null;
		this.gv_ProductionItem.FieldRate = null;
		this.gv_ProductionItem.FormId = "oContextAction";
		this.gv_ProductionItem.GridControl = this.grd_ProductionItem;
		this.gv_ProductionItem.IndicatorWidth = 35;
		this.gv_ProductionItem.IsApzTable = false;
		this.gv_ProductionItem.IsInsertNewRow = false;
		this.gv_ProductionItem.IsLoadingData = false;
		this.gv_ProductionItem.ItemId = "OPRODUCTIONITEM";
		this.gv_ProductionItem.LstColumnAllowBatch = null;
		this.gv_ProductionItem.LstColumnNotAllowBatch = null;
		this.gv_ProductionItem.LstColumnNotEditable = null;
		this.gv_ProductionItem.LstColumNotSetting = null;
		this.gv_ProductionItem.Name = "gv_ProductionItem";
		this.gv_ProductionItem.OptionsView.BestFitMaxRowCount = 100;
		this.gv_ProductionItem.OptionsView.ColumnAutoWidth = false;
		this.gv_ProductionItem.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_ProductionItem.OptionsView.ShowGroupPanel = false;
		this.gv_ProductionItem.UseBatchMod = false;
		this.gridColumnEx39.Caption = "AbsEntry";
		this.gridColumnEx39.CurrencyDateField = null;
		this.gridColumnEx39.CurrencyField = null;
		this.gridColumnEx39.CurrencyFixed = null;
		this.gridColumnEx39.CurrencyFixedRate = 0.0;
		this.gridColumnEx39.CurrencyRateField = null;
		this.gridColumnEx39.FieldName = "AbsEntry";
		this.gridColumnEx39.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx39.Name = "gridColumnEx39";
		this.gridColumnEx39.PrimaryKey = false;
		this.gridColumnEx39.UnitField = null;
		this.gridColumnEx40.Caption = "Source Field Name";
		this.gridColumnEx40.ColumnEdit = this.rep_ProductionLineSourceField;
		this.gridColumnEx40.CurrencyDateField = null;
		this.gridColumnEx40.CurrencyField = null;
		this.gridColumnEx40.CurrencyFixed = null;
		this.gridColumnEx40.CurrencyFixedRate = 0.0;
		this.gridColumnEx40.CurrencyRateField = null;
		this.gridColumnEx40.FieldName = "SourceField";
		this.gridColumnEx40.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx40.Name = "gridColumnEx40";
		this.gridColumnEx40.PrimaryKey = false;
		this.gridColumnEx40.UnitField = null;
		this.gridColumnEx40.Visible = true;
		this.gridColumnEx40.VisibleIndex = 1;
		this.gridColumnEx40.Width = 121;
		this.rep_ProductionLineSourceField.AutoHeight = false;
		this.rep_ProductionLineSourceField.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_ProductionLineSourceField.Name = "rep_ProductionLineSourceField";
		this.rep_ProductionLineSourceField.NullText = "";
		this.gridColumnEx41.Caption = "TargetItem";
		this.gridColumnEx41.CurrencyDateField = null;
		this.gridColumnEx41.CurrencyField = null;
		this.gridColumnEx41.CurrencyFixed = null;
		this.gridColumnEx41.CurrencyFixedRate = 0.0;
		this.gridColumnEx41.CurrencyRateField = null;
		this.gridColumnEx41.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx41.Name = "gridColumnEx41";
		this.gridColumnEx41.PrimaryKey = false;
		this.gridColumnEx41.UnitField = null;
		this.gridColumnEx42.Caption = "Target Field Name";
		this.gridColumnEx42.ColumnEdit = this.rep_ProductionOrderTargetLine;
		this.gridColumnEx42.CurrencyDateField = null;
		this.gridColumnEx42.CurrencyField = null;
		this.gridColumnEx42.CurrencyFixed = null;
		this.gridColumnEx42.CurrencyFixedRate = 0.0;
		this.gridColumnEx42.CurrencyRateField = null;
		this.gridColumnEx42.FieldName = "TargetField";
		this.gridColumnEx42.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx42.Name = "gridColumnEx42";
		this.gridColumnEx42.OptionsColumn.AllowEdit = false;
		this.gridColumnEx42.PrimaryKey = false;
		this.gridColumnEx42.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gridColumnEx42.UnitField = null;
		this.gridColumnEx42.Visible = true;
		this.gridColumnEx42.VisibleIndex = 0;
		this.gridColumnEx42.Width = 114;
		this.rep_ProductionOrderTargetLine.AutoHeight = false;
		this.rep_ProductionOrderTargetLine.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_ProductionOrderTargetLine.Name = "rep_ProductionOrderTargetLine";
		this.rep_ProductionOrderTargetLine.NullText = "";
		this.gridColumnEx43.Caption = "Default Value";
		this.gridColumnEx43.CurrencyDateField = null;
		this.gridColumnEx43.CurrencyField = null;
		this.gridColumnEx43.CurrencyFixed = null;
		this.gridColumnEx43.CurrencyFixedRate = 0.0;
		this.gridColumnEx43.CurrencyRateField = null;
		this.gridColumnEx43.FieldName = "DfltValue";
		this.gridColumnEx43.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx43.Name = "gridColumnEx43";
		this.gridColumnEx43.PrimaryKey = false;
		this.gridColumnEx43.UnitField = null;
		this.gridColumnEx43.Visible = true;
		this.gridColumnEx43.VisibleIndex = 2;
		this.gridColumnEx43.Width = 175;
		this.gridColumnEx44.Caption = "Summary Type";
		this.gridColumnEx44.ColumnEdit = this.rep_ProductionLineSumType;
		this.gridColumnEx44.CurrencyDateField = null;
		this.gridColumnEx44.CurrencyField = null;
		this.gridColumnEx44.CurrencyFixed = null;
		this.gridColumnEx44.CurrencyFixedRate = 0.0;
		this.gridColumnEx44.CurrencyRateField = null;
		this.gridColumnEx44.FieldName = "SumType";
		this.gridColumnEx44.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx44.Name = "gridColumnEx44";
		this.gridColumnEx44.PrimaryKey = false;
		this.gridColumnEx44.UnitField = null;
		this.gridColumnEx44.Visible = true;
		this.gridColumnEx44.VisibleIndex = 3;
		this.gridColumnEx44.Width = 100;
		this.rep_ProductionLineSumType.AutoHeight = false;
		this.rep_ProductionLineSumType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_ProductionLineSumType.Name = "rep_ProductionLineSumType";
		this.rep_ProductionLineSumType.NullText = "";
		this.xtraTabPage4.Controls.Add(this.grd_ProductionBatch);
		this.xtraTabPage4.EditControl = null;
		this.xtraTabPage4.EditValue = "Batch/ Serial/ Binlocation";
		this.xtraTabPage4.FieldName = "xtraTabPage4";
		this.xtraTabPage4.IsSearchControl = false;
		this.xtraTabPage4.Name = "xtraTabPage4";
		this.xtraTabPage4.PageVisible = false;
		this.xtraTabPage4.ReadOnly = false;
		this.xtraTabPage4.SettingInVisible = false;
		this.xtraTabPage4.SettingReadonly = false;
		this.xtraTabPage4.Size = new System.Drawing.Size(923, 344);
		this.xtraTabPage4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPage4.Text = "Batch/ Serial/ Binlocation";
		this.grd_ProductionBatch.AllowPasteExcel = false;
		this.grd_ProductionBatch.ClipboardData = null;
		this.grd_ProductionBatch.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_ProductionBatch.Location = new System.Drawing.Point(0, 0);
		this.grd_ProductionBatch.MainView = this.gv_ProductionBatch;
		this.grd_ProductionBatch.Name = "grd_ProductionBatch";
		this.grd_ProductionBatch.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.rep_ProdoctionTargetBatch, this.rep_ProductionBatchSourceField, this.rep_ProductionBatchSumType });
		this.grd_ProductionBatch.Size = new System.Drawing.Size(923, 344);
		this.grd_ProductionBatch.TabIndex = 2;
		this.grd_ProductionBatch.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_ProductionBatch });
		this.gv_ProductionBatch.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_ProductionBatch.ColumnKey = null;
		this.gv_ProductionBatch.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[6] { this.gridColumnEx45, this.gridColumnEx46, this.gridColumnEx47, this.gridColumnEx48, this.gridColumnEx49, this.gridColumnEx50 });
		this.gv_ProductionBatch.DisableFormSetting = true;
		this.gv_ProductionBatch.DisplayException = false;
		this.gv_ProductionBatch.FieldCurrency = null;
		this.gv_ProductionBatch.FieldRate = null;
		this.gv_ProductionBatch.FormId = "oContextAction";
		this.gv_ProductionBatch.GridControl = this.grd_ProductionBatch;
		this.gv_ProductionBatch.IndicatorWidth = 35;
		this.gv_ProductionBatch.IsApzTable = false;
		this.gv_ProductionBatch.IsInsertNewRow = false;
		this.gv_ProductionBatch.IsLoadingData = false;
		this.gv_ProductionBatch.ItemId = "OPRODUCTIONBATCH";
		this.gv_ProductionBatch.LstColumnAllowBatch = null;
		this.gv_ProductionBatch.LstColumnNotAllowBatch = null;
		this.gv_ProductionBatch.LstColumnNotEditable = null;
		this.gv_ProductionBatch.LstColumNotSetting = null;
		this.gv_ProductionBatch.Name = "gv_ProductionBatch";
		this.gv_ProductionBatch.OptionsView.BestFitMaxRowCount = 100;
		this.gv_ProductionBatch.OptionsView.ColumnAutoWidth = false;
		this.gv_ProductionBatch.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_ProductionBatch.OptionsView.ShowGroupPanel = false;
		this.gv_ProductionBatch.UseBatchMod = false;
		this.gridColumnEx45.Caption = "AbsEntry";
		this.gridColumnEx45.CurrencyDateField = null;
		this.gridColumnEx45.CurrencyField = null;
		this.gridColumnEx45.CurrencyFixed = null;
		this.gridColumnEx45.CurrencyFixedRate = 0.0;
		this.gridColumnEx45.CurrencyRateField = null;
		this.gridColumnEx45.FieldName = "AbsEntry";
		this.gridColumnEx45.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx45.Name = "gridColumnEx45";
		this.gridColumnEx45.PrimaryKey = false;
		this.gridColumnEx45.UnitField = null;
		this.gridColumnEx46.Caption = "Source Field Name";
		this.gridColumnEx46.ColumnEdit = this.rep_ProductionBatchSourceField;
		this.gridColumnEx46.CurrencyDateField = null;
		this.gridColumnEx46.CurrencyField = null;
		this.gridColumnEx46.CurrencyFixed = null;
		this.gridColumnEx46.CurrencyFixedRate = 0.0;
		this.gridColumnEx46.CurrencyRateField = null;
		this.gridColumnEx46.FieldName = "SourceField";
		this.gridColumnEx46.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx46.Name = "gridColumnEx46";
		this.gridColumnEx46.PrimaryKey = false;
		this.gridColumnEx46.UnitField = null;
		this.gridColumnEx46.Visible = true;
		this.gridColumnEx46.VisibleIndex = 1;
		this.gridColumnEx46.Width = 121;
		this.rep_ProductionBatchSourceField.AutoHeight = false;
		this.rep_ProductionBatchSourceField.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_ProductionBatchSourceField.Name = "rep_ProductionBatchSourceField";
		this.rep_ProductionBatchSourceField.NullText = "";
		this.gridColumnEx47.Caption = "TargetItem";
		this.gridColumnEx47.CurrencyDateField = null;
		this.gridColumnEx47.CurrencyField = null;
		this.gridColumnEx47.CurrencyFixed = null;
		this.gridColumnEx47.CurrencyFixedRate = 0.0;
		this.gridColumnEx47.CurrencyRateField = null;
		this.gridColumnEx47.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx47.Name = "gridColumnEx47";
		this.gridColumnEx47.PrimaryKey = false;
		this.gridColumnEx47.UnitField = null;
		this.gridColumnEx48.Caption = "Target Field Name";
		this.gridColumnEx48.ColumnEdit = this.rep_ProdoctionTargetBatch;
		this.gridColumnEx48.CurrencyDateField = null;
		this.gridColumnEx48.CurrencyField = null;
		this.gridColumnEx48.CurrencyFixed = null;
		this.gridColumnEx48.CurrencyFixedRate = 0.0;
		this.gridColumnEx48.CurrencyRateField = null;
		this.gridColumnEx48.FieldName = "TargetField";
		this.gridColumnEx48.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx48.Name = "gridColumnEx48";
		this.gridColumnEx48.OptionsColumn.AllowEdit = false;
		this.gridColumnEx48.PrimaryKey = false;
		this.gridColumnEx48.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gridColumnEx48.UnitField = null;
		this.gridColumnEx48.Visible = true;
		this.gridColumnEx48.VisibleIndex = 0;
		this.gridColumnEx48.Width = 114;
		this.rep_ProdoctionTargetBatch.AutoHeight = false;
		this.rep_ProdoctionTargetBatch.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_ProdoctionTargetBatch.Name = "rep_ProdoctionTargetBatch";
		this.rep_ProdoctionTargetBatch.NullText = "";
		this.gridColumnEx49.Caption = "Default Value";
		this.gridColumnEx49.CurrencyDateField = null;
		this.gridColumnEx49.CurrencyField = null;
		this.gridColumnEx49.CurrencyFixed = null;
		this.gridColumnEx49.CurrencyFixedRate = 0.0;
		this.gridColumnEx49.CurrencyRateField = null;
		this.gridColumnEx49.FieldName = "DfltValue";
		this.gridColumnEx49.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx49.Name = "gridColumnEx49";
		this.gridColumnEx49.PrimaryKey = false;
		this.gridColumnEx49.UnitField = null;
		this.gridColumnEx49.Visible = true;
		this.gridColumnEx49.VisibleIndex = 2;
		this.gridColumnEx49.Width = 175;
		this.gridColumnEx50.Caption = "Summary Type";
		this.gridColumnEx50.ColumnEdit = this.rep_ProductionBatchSumType;
		this.gridColumnEx50.CurrencyDateField = null;
		this.gridColumnEx50.CurrencyField = null;
		this.gridColumnEx50.CurrencyFixed = null;
		this.gridColumnEx50.CurrencyFixedRate = 0.0;
		this.gridColumnEx50.CurrencyRateField = null;
		this.gridColumnEx50.FieldName = "SumType";
		this.gridColumnEx50.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx50.Name = "gridColumnEx50";
		this.gridColumnEx50.PrimaryKey = false;
		this.gridColumnEx50.UnitField = null;
		this.gridColumnEx50.Visible = true;
		this.gridColumnEx50.VisibleIndex = 3;
		this.gridColumnEx50.Width = 100;
		this.rep_ProductionBatchSumType.AutoHeight = false;
		this.rep_ProductionBatchSumType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_ProductionBatchSumType.Name = "rep_ProductionBatchSumType";
		this.rep_ProductionBatchSumType.NullText = "";
		this.chk_ShowProductionForm.EditControl = null;
		this.chk_ShowProductionForm.EditValue = "Y";
		this.chk_ShowProductionForm.FieldName = null;
		this.chk_ShowProductionForm.IsSearchControl = false;
		this.chk_ShowProductionForm.Location = new System.Drawing.Point(10, 3);
		this.chk_ShowProductionForm.Name = "chk_ShowProductionForm";
		this.chk_ShowProductionForm.Properties.Caption = "Show Window";
		this.chk_ShowProductionForm.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.chk_ShowProductionForm.Properties.ValueChecked = "Y";
		this.chk_ShowProductionForm.Properties.ValueGrayed = "N";
		this.chk_ShowProductionForm.Properties.ValueUnchecked = "N";
		this.chk_ShowProductionForm.SettingInVisible = false;
		this.chk_ShowProductionForm.SettingReadonly = false;
		this.chk_ShowProductionForm.Size = new System.Drawing.Size(174, 20);
		this.chk_ShowProductionForm.TabIndex = 4;
		this.chk_ShowProductionForm.CheckedChanged += new System.EventHandler(this.chk_ShowProductionForm_CheckedChanged);
		this.tab_Email.Controls.Add(this.txt_EmailGroupByFields);
		this.tab_Email.Controls.Add(this.labelLine10);
		this.tab_Email.Controls.Add(this.look_EmailGroupBy);
		this.tab_Email.Controls.Add(this.labelLine9);
		this.tab_Email.Controls.Add(this.btn_EditEmailContent);
		this.tab_Email.Controls.Add(this.xtraTabControl4);
		this.tab_Email.Controls.Add(this.txt_Subject);
		this.tab_Email.Controls.Add(this.txt_AddressBcc);
		this.tab_Email.Controls.Add(this.txt_AddressCc);
		this.tab_Email.Controls.Add(this.txt_AddressTo);
		this.tab_Email.Controls.Add(this.txt_AddressFrom);
		this.tab_Email.Controls.Add(this.labelLine8);
		this.tab_Email.Controls.Add(this.labelLine7);
		this.tab_Email.Controls.Add(this.labelLine6);
		this.tab_Email.Controls.Add(this.labelLine5);
		this.tab_Email.Controls.Add(this.labelLine4);
		this.tab_Email.EditControl = null;
		this.tab_Email.EditValue = "E-Mail";
		this.tab_Email.FieldName = "tab_Email";
		this.tab_Email.IsSearchControl = false;
		this.tab_Email.Name = "tab_Email";
		this.tab_Email.ReadOnly = false;
		this.tab_Email.SettingInVisible = false;
		this.tab_Email.SettingReadonly = false;
		this.tab_Email.Size = new System.Drawing.Size(938, 400);
		this.tab_Email.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Email.Text = "E-Mail";
		this.txt_EmailGroupByFields.EditControl = null;
		this.txt_EmailGroupByFields.FieldName = null;
		this.txt_EmailGroupByFields.IsSearchControl = false;
		this.txt_EmailGroupByFields.Location = new System.Drawing.Point(492, 34);
		this.txt_EmailGroupByFields.Name = "txt_EmailGroupByFields";
		this.txt_EmailGroupByFields.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.txt_EmailGroupByFields.SettingInVisible = false;
		this.txt_EmailGroupByFields.SettingReadonly = false;
		this.txt_EmailGroupByFields.Size = new System.Drawing.Size(300, 20);
		this.txt_EmailGroupByFields.TabIndex = 19;
		this.txt_EmailGroupByFields.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txt_EmailGroupByFields_ButtonClick);
		this.txt_EmailGroupByFields.EditValueChanged += new System.EventHandler(this.txt_EmailGroupByFields_EditValueChanged);
		this.labelLine10.EditControl = null;
		this.labelLine10.EditValue = "Group By Fields";
		this.labelLine10.FieldName = null;
		this.labelLine10.HideWhenEmpty = false;
		this.labelLine10.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine10.Location = new System.Drawing.Point(397, 37);
		this.labelLine10.Name = "labelLine10";
		this.labelLine10.ReadOnly = false;
		this.labelLine10.SettingInVisible = false;
		this.labelLine10.SettingReadonly = false;
		this.labelLine10.ShowUnderLine = true;
		this.labelLine10.Size = new System.Drawing.Size(95, 20);
		this.labelLine10.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine10.TabIndex = 18;
		this.labelLine10.Utext = "Group By Fields";
		this.look_EmailGroupBy.EditControl = null;
		this.look_EmailGroupBy.FieldName = null;
		this.look_EmailGroupBy.IsSearchControl = false;
		this.look_EmailGroupBy.Location = new System.Drawing.Point(492, 10);
		this.look_EmailGroupBy.Name = "look_EmailGroupBy";
		this.look_EmailGroupBy.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.look_EmailGroupBy.Properties.NullText = "";
		this.look_EmailGroupBy.SettingInVisible = false;
		this.look_EmailGroupBy.SettingReadonly = false;
		this.look_EmailGroupBy.Size = new System.Drawing.Size(300, 20);
		this.look_EmailGroupBy.TabIndex = 17;
		this.look_EmailGroupBy.EditValueChanged += new System.EventHandler(this.look_EmailGroupBy_EditValueChanged);
		this.labelLine9.EditControl = null;
		this.labelLine9.EditValue = "Group By";
		this.labelLine9.FieldName = null;
		this.labelLine9.HideWhenEmpty = false;
		this.labelLine9.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine9.Location = new System.Drawing.Point(397, 13);
		this.labelLine9.Name = "labelLine9";
		this.labelLine9.ReadOnly = false;
		this.labelLine9.SettingInVisible = false;
		this.labelLine9.SettingReadonly = false;
		this.labelLine9.ShowUnderLine = true;
		this.labelLine9.Size = new System.Drawing.Size(95, 20);
		this.labelLine9.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine9.TabIndex = 16;
		this.labelLine9.Utext = "Group By";
		this.btn_EditEmailContent.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
		this.btn_EditEmailContent.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_EditEmailContent.ImageOptions.Image");
		this.btn_EditEmailContent.Location = new System.Drawing.Point(709, 121);
		this.btn_EditEmailContent.Name = "btn_EditEmailContent";
		this.btn_EditEmailContent.Size = new System.Drawing.Size(106, 23);
		this.btn_EditEmailContent.TabIndex = 15;
		this.btn_EditEmailContent.Text = "Edit Content";
		this.btn_EditEmailContent.Click += new System.EventHandler(this.btn_EditEmailContent_Click);
		this.xtraTabControl4.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.xtraTabControl4.IsSearchControl = false;
		this.xtraTabControl4.Location = new System.Drawing.Point(11, 150);
		this.xtraTabControl4.Name = "xtraTabControl4";
		this.xtraTabControl4.SelectedTabPage = this.xtraTabPage8;
		this.xtraTabControl4.Size = new System.Drawing.Size(809, 235);
		this.xtraTabControl4.TabIndex = 14;
		this.xtraTabControl4.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[2] { this.xtraTabPage8, this.xtraTabPage9 });
		this.xtraTabPage8.Controls.Add(this.snapControl1);
		this.xtraTabPage8.EditControl = null;
		this.xtraTabPage8.EditValue = "Content";
		this.xtraTabPage8.FieldName = "xtraTabPage8";
		this.xtraTabPage8.IsSearchControl = false;
		this.xtraTabPage8.Name = "xtraTabPage8";
		this.xtraTabPage8.ReadOnly = false;
		this.xtraTabPage8.SettingInVisible = false;
		this.xtraTabPage8.SettingReadonly = false;
		this.xtraTabPage8.Size = new System.Drawing.Size(807, 207);
		this.xtraTabPage8.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPage8.Text = "Content";
		this.snapControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.snapControl1.Location = new System.Drawing.Point(0, 0);
		this.snapControl1.Name = "snapControl1";
		this.snapControl1.Options.DocumentCapabilities.TrackChanges = DevExpress.XtraRichEdit.DocumentCapability.Disabled;
		this.snapControl1.Options.SnapMailMergeVisualOptions.DataSourceName = null;
		this.snapControl1.Size = new System.Drawing.Size(807, 207);
		this.snapControl1.TabIndex = 0;
		this.snapControl1.Text = "snapControl1";
		this.snapControl1.ContentChanged += new System.EventHandler(this.snapControl1_ContentChanged);
		this.xtraTabPage9.Controls.Add(this.splitContainerControl2);
		this.xtraTabPage9.EditControl = null;
		this.xtraTabPage9.EditValue = "Attachments";
		this.xtraTabPage9.FieldName = "xtraTabPage9";
		this.xtraTabPage9.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("xtraTabPage9.ImageOptions.Image");
		this.xtraTabPage9.IsSearchControl = false;
		this.xtraTabPage9.Name = "xtraTabPage9";
		this.xtraTabPage9.ReadOnly = false;
		this.xtraTabPage9.SettingInVisible = false;
		this.xtraTabPage9.SettingReadonly = false;
		this.xtraTabPage9.Size = new System.Drawing.Size(807, 207);
		this.xtraTabPage9.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPage9.Text = "Attachments";
		this.splitContainerControl2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainerControl2.Horizontal = false;
		this.splitContainerControl2.Location = new System.Drawing.Point(0, 0);
		this.splitContainerControl2.Name = "splitContainerControl2";
		this.splitContainerControl2.Panel1.Controls.Add(this.grd_AttachFiles);
		this.splitContainerControl2.Panel1.Text = "Panel1";
		this.splitContainerControl2.Panel2.Controls.Add(this.grd_RptParam);
		this.splitContainerControl2.Panel2.Text = "Panel2";
		this.splitContainerControl2.Size = new System.Drawing.Size(807, 207);
		this.splitContainerControl2.SplitterPosition = 130;
		this.splitContainerControl2.TabIndex = 0;
		this.splitContainerControl2.Text = "splitContainerControl2";
		this.grd_AttachFiles.AllowPasteExcel = false;
		this.grd_AttachFiles.ClipboardData = null;
		this.grd_AttachFiles.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_AttachFiles.Location = new System.Drawing.Point(0, 0);
		this.grd_AttachFiles.MainView = this.gv_AttachFiles;
		this.grd_AttachFiles.Name = "grd_AttachFiles";
		this.grd_AttachFiles.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.rep_Path, this.rep_AttType });
		this.grd_AttachFiles.Size = new System.Drawing.Size(807, 130);
		this.grd_AttachFiles.TabIndex = 0;
		this.grd_AttachFiles.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_AttachFiles });
		this.gv_AttachFiles.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_AttachFiles.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_AttachFiles.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_AttachFiles.ColumnKey = null;
		this.gv_AttachFiles.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.gridColumnEx58, this.gridColumnEx59, this.gridColumnEx60 });
		this.gv_AttachFiles.DisableFormSetting = false;
		this.gv_AttachFiles.DisplayException = false;
		this.gv_AttachFiles.FieldCurrency = null;
		this.gv_AttachFiles.FieldRate = null;
		this.gv_AttachFiles.FormId = "oContextAction";
		this.gv_AttachFiles.GridControl = this.grd_AttachFiles;
		this.gv_AttachFiles.IndicatorWidth = 35;
		this.gv_AttachFiles.IsApzTable = false;
		this.gv_AttachFiles.IsInsertNewRow = false;
		this.gv_AttachFiles.IsLoadingData = false;
		this.gv_AttachFiles.ItemId = "OATTACHFILE";
		this.gv_AttachFiles.LstColumnAllowBatch = null;
		this.gv_AttachFiles.LstColumnNotAllowBatch = null;
		this.gv_AttachFiles.LstColumnNotEditable = null;
		this.gv_AttachFiles.LstColumNotSetting = null;
		this.gv_AttachFiles.Name = "gv_AttachFiles";
		this.gv_AttachFiles.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_AttachFiles.OptionsView.BestFitMaxRowCount = 100;
		this.gv_AttachFiles.OptionsView.ColumnAutoWidth = false;
		this.gv_AttachFiles.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_AttachFiles.OptionsView.ShowGroupPanel = false;
		this.gv_AttachFiles.UseBatchMod = false;
		this.gv_AttachFiles.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_AttachFiles_FocusedRowObjectChanged);
		this.gv_AttachFiles.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_AttachFiles_CellValueChanged);
		this.gridColumnEx58.Caption = "Path";
		this.gridColumnEx58.ColumnEdit = this.rep_Path;
		this.gridColumnEx58.CurrencyDateField = null;
		this.gridColumnEx58.CurrencyField = null;
		this.gridColumnEx58.CurrencyFixed = null;
		this.gridColumnEx58.CurrencyFixedRate = 0.0;
		this.gridColumnEx58.CurrencyRateField = null;
		this.gridColumnEx58.FieldName = "FilePath";
		this.gridColumnEx58.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx58.Name = "gridColumnEx58";
		this.gridColumnEx58.PrimaryKey = false;
		this.gridColumnEx58.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridColumnEx58.UnitField = null;
		this.gridColumnEx58.Visible = true;
		this.gridColumnEx58.VisibleIndex = 0;
		this.gridColumnEx58.Width = 224;
		this.rep_Path.AutoHeight = false;
		this.rep_Path.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph)
		});
		this.rep_Path.Name = "rep_Path";
		this.rep_Path.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_Path_ButtonClick);
		this.gridColumnEx59.Caption = "Alias";
		this.gridColumnEx59.CurrencyDateField = null;
		this.gridColumnEx59.CurrencyField = null;
		this.gridColumnEx59.CurrencyFixed = null;
		this.gridColumnEx59.CurrencyFixedRate = 0.0;
		this.gridColumnEx59.CurrencyRateField = null;
		this.gridColumnEx59.FieldName = "FileAlias";
		this.gridColumnEx59.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx59.Name = "gridColumnEx59";
		this.gridColumnEx59.PrimaryKey = false;
		this.gridColumnEx59.UnitField = null;
		this.gridColumnEx59.Visible = true;
		this.gridColumnEx59.VisibleIndex = 1;
		this.gridColumnEx59.Width = 244;
		this.gridColumnEx60.Caption = "Type";
		this.gridColumnEx60.ColumnEdit = this.rep_AttType;
		this.gridColumnEx60.CurrencyDateField = null;
		this.gridColumnEx60.CurrencyField = null;
		this.gridColumnEx60.CurrencyFixed = null;
		this.gridColumnEx60.CurrencyFixedRate = 0.0;
		this.gridColumnEx60.CurrencyRateField = null;
		this.gridColumnEx60.FieldName = "FileType";
		this.gridColumnEx60.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx60.Name = "gridColumnEx60";
		this.gridColumnEx60.PrimaryKey = false;
		this.gridColumnEx60.UnitField = null;
		this.gridColumnEx60.Visible = true;
		this.gridColumnEx60.VisibleIndex = 2;
		this.gridColumnEx60.Width = 87;
		this.rep_AttType.AutoHeight = false;
		this.rep_AttType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_AttType.Name = "rep_AttType";
		this.rep_AttType.NullText = "";
		this.grd_RptParam.AllowPasteExcel = false;
		this.grd_RptParam.ClipboardData = null;
		this.grd_RptParam.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_RptParam.Location = new System.Drawing.Point(0, 0);
		this.grd_RptParam.MainView = this.gv_RptParam;
		this.grd_RptParam.Name = "grd_RptParam";
		this.grd_RptParam.Size = new System.Drawing.Size(807, 67);
		this.grd_RptParam.TabIndex = 4;
		this.grd_RptParam.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_RptParam });
		this.gv_RptParam.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_RptParam.ColumnKey = null;
		this.gv_RptParam.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[7] { this.gridColumnEx51, this.gridColumnEx52, this.gridColumnEx53, this.gridColumnEx54, this.gridColumnEx55, this.gridColumnEx56, this.gridColumnEx57 });
		this.gv_RptParam.DisableFormSetting = false;
		this.gv_RptParam.DisplayException = false;
		this.gv_RptParam.FieldCurrency = null;
		this.gv_RptParam.FieldRate = null;
		this.gv_RptParam.FormId = "oContextAction";
		this.gv_RptParam.GridControl = this.grd_RptParam;
		this.gv_RptParam.IndicatorWidth = 35;
		this.gv_RptParam.IsApzTable = false;
		this.gv_RptParam.IsInsertNewRow = false;
		this.gv_RptParam.IsLoadingData = false;
		this.gv_RptParam.ItemId = "ORPTPARAM";
		this.gv_RptParam.LstColumnAllowBatch = null;
		this.gv_RptParam.LstColumnNotAllowBatch = null;
		this.gv_RptParam.LstColumnNotEditable = null;
		this.gv_RptParam.LstColumNotSetting = null;
		this.gv_RptParam.Name = "gv_RptParam";
		this.gv_RptParam.OptionsView.BestFitMaxRowCount = 100;
		this.gv_RptParam.OptionsView.ColumnAutoWidth = false;
		this.gv_RptParam.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_RptParam.OptionsView.ShowGroupPanel = false;
		this.gv_RptParam.OptionsView.ShowViewCaption = true;
		this.gv_RptParam.UseBatchMod = false;
		this.gv_RptParam.ViewCaption = "Parameters";
		this.gridColumnEx51.Caption = "Name";
		this.gridColumnEx51.CurrencyDateField = null;
		this.gridColumnEx51.CurrencyField = null;
		this.gridColumnEx51.CurrencyFixed = null;
		this.gridColumnEx51.CurrencyFixedRate = 0.0;
		this.gridColumnEx51.CurrencyRateField = null;
		this.gridColumnEx51.FieldName = "ParamName";
		this.gridColumnEx51.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx51.Name = "gridColumnEx51";
		this.gridColumnEx51.OptionsColumn.ReadOnly = true;
		this.gridColumnEx51.PrimaryKey = false;
		this.gridColumnEx51.UnitField = null;
		this.gridColumnEx51.Visible = true;
		this.gridColumnEx51.VisibleIndex = 0;
		this.gridColumnEx51.Width = 100;
		this.gridColumnEx52.Caption = "Label";
		this.gridColumnEx52.CurrencyDateField = null;
		this.gridColumnEx52.CurrencyField = null;
		this.gridColumnEx52.CurrencyFixed = null;
		this.gridColumnEx52.CurrencyFixedRate = 0.0;
		this.gridColumnEx52.CurrencyRateField = null;
		this.gridColumnEx52.FieldName = "Label";
		this.gridColumnEx52.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx52.Name = "gridColumnEx52";
		this.gridColumnEx52.PrimaryKey = false;
		this.gridColumnEx52.UnitField = null;
		this.gridColumnEx52.Visible = true;
		this.gridColumnEx52.VisibleIndex = 1;
		this.gridColumnEx52.Width = 118;
		this.gridColumnEx53.Caption = "Data Type";
		this.gridColumnEx53.ColumnEdit = this.rep_AttParamDataType;
		this.gridColumnEx53.CurrencyDateField = null;
		this.gridColumnEx53.CurrencyField = null;
		this.gridColumnEx53.CurrencyFixed = null;
		this.gridColumnEx53.CurrencyFixedRate = 0.0;
		this.gridColumnEx53.CurrencyRateField = null;
		this.gridColumnEx53.FieldName = "DataType";
		this.gridColumnEx53.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx53.Name = "gridColumnEx53";
		this.gridColumnEx53.PrimaryKey = false;
		this.gridColumnEx53.UnitField = null;
		this.gridColumnEx53.Visible = true;
		this.gridColumnEx53.VisibleIndex = 2;
		this.gridColumnEx53.Width = 100;
		this.gridColumnEx54.Caption = "Source Field";
		this.gridColumnEx54.ColumnEdit = this.rep_AttParamSourceField;
		this.gridColumnEx54.CurrencyDateField = null;
		this.gridColumnEx54.CurrencyField = null;
		this.gridColumnEx54.CurrencyFixed = null;
		this.gridColumnEx54.CurrencyFixedRate = 0.0;
		this.gridColumnEx54.CurrencyRateField = null;
		this.gridColumnEx54.FieldName = "SourceField";
		this.gridColumnEx54.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx54.Name = "gridColumnEx54";
		this.gridColumnEx54.PrimaryKey = false;
		this.gridColumnEx54.UnitField = null;
		this.gridColumnEx54.Visible = true;
		this.gridColumnEx54.VisibleIndex = 3;
		this.gridColumnEx54.Width = 103;
		this.gridColumnEx55.Caption = "Sub. Type";
		this.gridColumnEx55.CurrencyDateField = null;
		this.gridColumnEx55.CurrencyField = null;
		this.gridColumnEx55.CurrencyFixed = null;
		this.gridColumnEx55.CurrencyFixedRate = 0.0;
		this.gridColumnEx55.CurrencyRateField = null;
		this.gridColumnEx55.FieldName = "SubType";
		this.gridColumnEx55.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx55.Name = "gridColumnEx55";
		this.gridColumnEx55.PrimaryKey = false;
		this.gridColumnEx55.UnitField = null;
		this.gridColumnEx55.Width = 100;
		this.gridColumnEx56.Caption = "Default Value";
		this.gridColumnEx56.CurrencyDateField = null;
		this.gridColumnEx56.CurrencyField = null;
		this.gridColumnEx56.CurrencyFixed = null;
		this.gridColumnEx56.CurrencyFixedRate = 0.0;
		this.gridColumnEx56.CurrencyRateField = null;
		this.gridColumnEx56.FieldName = "DfltValue";
		this.gridColumnEx56.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx56.Name = "gridColumnEx56";
		this.gridColumnEx56.PrimaryKey = false;
		this.gridColumnEx56.UnitField = null;
		this.gridColumnEx56.Visible = true;
		this.gridColumnEx56.VisibleIndex = 4;
		this.gridColumnEx56.Width = 100;
		this.gridColumnEx57.Caption = "Active";
		this.gridColumnEx57.ColumnEdit = this.repositoryItemCheckEdit3;
		this.gridColumnEx57.CurrencyDateField = null;
		this.gridColumnEx57.CurrencyField = null;
		this.gridColumnEx57.CurrencyFixed = null;
		this.gridColumnEx57.CurrencyFixedRate = 0.0;
		this.gridColumnEx57.CurrencyRateField = null;
		this.gridColumnEx57.FieldName = "Active";
		this.gridColumnEx57.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx57.Name = "gridColumnEx57";
		this.gridColumnEx57.PrimaryKey = false;
		this.gridColumnEx57.UnitField = null;
		this.txt_Subject.EditControl = null;
		this.txt_Subject.FieldName = null;
		this.txt_Subject.IsSearchControl = false;
		this.txt_Subject.Location = new System.Drawing.Point(91, 106);
		this.txt_Subject.Name = "txt_Subject";
		this.txt_Subject.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.txt_Subject.SettingInVisible = false;
		this.txt_Subject.SettingReadonly = false;
		this.txt_Subject.Size = new System.Drawing.Size(300, 20);
		this.txt_Subject.TabIndex = 13;
		this.txt_Subject.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txt_Subject_ButtonClick);
		this.txt_Subject.EditValueChanged += new System.EventHandler(this.txt_Subject_EditValueChanged);
		this.txt_AddressBcc.EditControl = null;
		this.txt_AddressBcc.FieldName = null;
		this.txt_AddressBcc.IsSearchControl = false;
		this.txt_AddressBcc.Location = new System.Drawing.Point(91, 82);
		this.txt_AddressBcc.Name = "txt_AddressBcc";
		this.txt_AddressBcc.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.txt_AddressBcc.SettingInVisible = false;
		this.txt_AddressBcc.SettingReadonly = false;
		this.txt_AddressBcc.Size = new System.Drawing.Size(300, 20);
		this.txt_AddressBcc.TabIndex = 12;
		this.txt_AddressBcc.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txt_AddressBcc_ButtonClick);
		this.txt_AddressBcc.EditValueChanged += new System.EventHandler(this.txt_AddressBcc_EditValueChanged);
		this.txt_AddressCc.EditControl = null;
		this.txt_AddressCc.FieldName = null;
		this.txt_AddressCc.IsSearchControl = false;
		this.txt_AddressCc.Location = new System.Drawing.Point(91, 58);
		this.txt_AddressCc.Name = "txt_AddressCc";
		this.txt_AddressCc.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.txt_AddressCc.SettingInVisible = false;
		this.txt_AddressCc.SettingReadonly = false;
		this.txt_AddressCc.Size = new System.Drawing.Size(300, 20);
		this.txt_AddressCc.TabIndex = 11;
		this.txt_AddressCc.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.AddressCc_ButtonClick);
		this.txt_AddressCc.EditValueChanged += new System.EventHandler(this.txt_AddressCc_EditValueChanged);
		this.txt_AddressTo.EditControl = null;
		this.txt_AddressTo.FieldName = null;
		this.txt_AddressTo.IsSearchControl = false;
		this.txt_AddressTo.Location = new System.Drawing.Point(91, 34);
		this.txt_AddressTo.Name = "txt_AddressTo";
		this.txt_AddressTo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.txt_AddressTo.SettingInVisible = false;
		this.txt_AddressTo.SettingReadonly = false;
		this.txt_AddressTo.Size = new System.Drawing.Size(300, 20);
		this.txt_AddressTo.TabIndex = 10;
		this.txt_AddressTo.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txt_AddressTo_ButtonClick);
		this.txt_AddressTo.EditValueChanged += new System.EventHandler(this.txt_AddressTo_EditValueChanged);
		this.txt_AddressFrom.EditControl = null;
		this.txt_AddressFrom.FieldName = null;
		this.txt_AddressFrom.IsSearchControl = false;
		this.txt_AddressFrom.Location = new System.Drawing.Point(91, 10);
		this.txt_AddressFrom.Name = "txt_AddressFrom";
		this.txt_AddressFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.txt_AddressFrom.SettingInVisible = false;
		this.txt_AddressFrom.SettingReadonly = false;
		this.txt_AddressFrom.Size = new System.Drawing.Size(300, 20);
		this.txt_AddressFrom.TabIndex = 9;
		this.txt_AddressFrom.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txt_AddressFrom_ButtonClick);
		this.txt_AddressFrom.EditValueChanged += new System.EventHandler(this.txt_AddressFrom_EditValueChanged);
		this.labelLine8.EditControl = null;
		this.labelLine8.EditValue = "Subject";
		this.labelLine8.FieldName = null;
		this.labelLine8.HideWhenEmpty = false;
		this.labelLine8.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine8.Location = new System.Drawing.Point(11, 109);
		this.labelLine8.Name = "labelLine8";
		this.labelLine8.ReadOnly = false;
		this.labelLine8.SettingInVisible = false;
		this.labelLine8.SettingReadonly = false;
		this.labelLine8.ShowUnderLine = true;
		this.labelLine8.Size = new System.Drawing.Size(80, 20);
		this.labelLine8.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine8.TabIndex = 8;
		this.labelLine8.Utext = "Subject";
		this.labelLine7.EditControl = null;
		this.labelLine7.EditValue = "BCC";
		this.labelLine7.FieldName = null;
		this.labelLine7.HideWhenEmpty = false;
		this.labelLine7.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine7.Location = new System.Drawing.Point(11, 85);
		this.labelLine7.Name = "labelLine7";
		this.labelLine7.ReadOnly = false;
		this.labelLine7.SettingInVisible = false;
		this.labelLine7.SettingReadonly = false;
		this.labelLine7.ShowUnderLine = true;
		this.labelLine7.Size = new System.Drawing.Size(80, 20);
		this.labelLine7.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine7.TabIndex = 6;
		this.labelLine7.Utext = "BCC";
		this.labelLine6.EditControl = null;
		this.labelLine6.EditValue = "CC";
		this.labelLine6.FieldName = null;
		this.labelLine6.HideWhenEmpty = false;
		this.labelLine6.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine6.Location = new System.Drawing.Point(11, 61);
		this.labelLine6.Name = "labelLine6";
		this.labelLine6.ReadOnly = false;
		this.labelLine6.SettingInVisible = false;
		this.labelLine6.SettingReadonly = false;
		this.labelLine6.ShowUnderLine = true;
		this.labelLine6.Size = new System.Drawing.Size(80, 20);
		this.labelLine6.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine6.TabIndex = 4;
		this.labelLine6.Utext = "CC";
		this.labelLine5.EditControl = null;
		this.labelLine5.EditValue = "To";
		this.labelLine5.FieldName = null;
		this.labelLine5.HideWhenEmpty = false;
		this.labelLine5.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine5.Location = new System.Drawing.Point(11, 37);
		this.labelLine5.Name = "labelLine5";
		this.labelLine5.ReadOnly = false;
		this.labelLine5.SettingInVisible = false;
		this.labelLine5.SettingReadonly = false;
		this.labelLine5.ShowUnderLine = true;
		this.labelLine5.Size = new System.Drawing.Size(80, 20);
		this.labelLine5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine5.TabIndex = 2;
		this.labelLine5.Utext = "To";
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "From";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(11, 13);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(80, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 0;
		this.labelLine4.Utext = "From";
		this.tab_Forecast.Controls.Add(this.xtraTabControlEx1);
		this.tab_Forecast.Controls.Add(this.chk_ForecastShowWindow);
		this.tab_Forecast.Name = "tab_Forecast";
		this.tab_Forecast.Size = new System.Drawing.Size(938, 400);
		this.tab_Forecast.Text = "Forecast";
		this.xtraTabControlEx1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.xtraTabControlEx1.IsSearchControl = false;
		this.xtraTabControlEx1.Location = new System.Drawing.Point(0, 25);
		this.xtraTabControlEx1.Name = "xtraTabControlEx1";
		this.xtraTabControlEx1.SelectedTabPage = this.xtraTabPageEx1;
		this.xtraTabControlEx1.Size = new System.Drawing.Size(931, 288);
		this.xtraTabControlEx1.TabIndex = 7;
		this.xtraTabControlEx1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[2] { this.xtraTabPageEx1, this.xtraTabPageEx2 });
		this.xtraTabPageEx1.Controls.Add(this.grd_ForecastHeader);
		this.xtraTabPageEx1.EditControl = null;
		this.xtraTabPageEx1.EditValue = "Header";
		this.xtraTabPageEx1.FieldName = "xtraTabPage2";
		this.xtraTabPageEx1.IsSearchControl = false;
		this.xtraTabPageEx1.Name = "xtraTabPageEx1";
		this.xtraTabPageEx1.ReadOnly = false;
		this.xtraTabPageEx1.SettingInVisible = false;
		this.xtraTabPageEx1.SettingReadonly = false;
		this.xtraTabPageEx1.Size = new System.Drawing.Size(929, 263);
		this.xtraTabPageEx1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPageEx1.Text = "Header";
		this.grd_ForecastHeader.AllowPasteExcel = false;
		this.grd_ForecastHeader.ClipboardData = null;
		this.grd_ForecastHeader.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_ForecastHeader.Location = new System.Drawing.Point(0, 0);
		this.grd_ForecastHeader.MainView = this.gv_ForecastHeader;
		this.grd_ForecastHeader.Name = "grd_ForecastHeader";
		this.grd_ForecastHeader.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.rep_ForecastTargetField, this.rep_ForecastSourceField, this.repositoryItemTextEdit1 });
		this.grd_ForecastHeader.Size = new System.Drawing.Size(929, 263);
		this.grd_ForecastHeader.TabIndex = 0;
		this.grd_ForecastHeader.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_ForecastHeader });
		this.gv_ForecastHeader.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_ForecastHeader.ColumnKey = null;
		this.gv_ForecastHeader.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[5] { this.gridColumnEx73, this.gridColumnEx74, this.gridColumnEx75, this.gridColumnEx76, this.gridColumnEx77 });
		this.gv_ForecastHeader.DisableFormSetting = true;
		this.gv_ForecastHeader.DisplayException = false;
		this.gv_ForecastHeader.FieldCurrency = null;
		this.gv_ForecastHeader.FieldRate = null;
		this.gv_ForecastHeader.FormId = "oContextAction";
		this.gv_ForecastHeader.GridControl = this.grd_ForecastHeader;
		this.gv_ForecastHeader.IndicatorWidth = 35;
		this.gv_ForecastHeader.IsApzTable = false;
		this.gv_ForecastHeader.IsInsertNewRow = false;
		this.gv_ForecastHeader.IsLoadingData = false;
		this.gv_ForecastHeader.ItemId = "OFORECASTHEADER";
		this.gv_ForecastHeader.LstColumnAllowBatch = null;
		this.gv_ForecastHeader.LstColumnNotAllowBatch = null;
		this.gv_ForecastHeader.LstColumnNotEditable = null;
		this.gv_ForecastHeader.LstColumNotSetting = null;
		this.gv_ForecastHeader.Name = "gv_ForecastHeader";
		this.gv_ForecastHeader.OptionsView.BestFitMaxRowCount = 100;
		this.gv_ForecastHeader.OptionsView.ColumnAutoWidth = false;
		this.gv_ForecastHeader.OptionsView.ShowGroupPanel = false;
		this.gv_ForecastHeader.UseBatchMod = false;
		this.gv_ForecastHeader.CustomRowCellEditForEditing += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gv_ForecastHeader_CustomRowCellEditForEditing);
		this.gv_ForecastHeader.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.gv_ForecastHeader_CustomColumnDisplayText);
		this.gridColumnEx73.Caption = "AbsEntry";
		this.gridColumnEx73.CurrencyDateField = null;
		this.gridColumnEx73.CurrencyField = null;
		this.gridColumnEx73.CurrencyFixed = null;
		this.gridColumnEx73.CurrencyFixedRate = 0.0;
		this.gridColumnEx73.CurrencyRateField = null;
		this.gridColumnEx73.FieldName = "AbsEntry";
		this.gridColumnEx73.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx73.Name = "gridColumnEx73";
		this.gridColumnEx73.PrimaryKey = false;
		this.gridColumnEx73.UnitField = null;
		this.gridColumnEx74.Caption = "Source Field Name";
		this.gridColumnEx74.ColumnEdit = this.rep_ForecastSourceField;
		this.gridColumnEx74.CurrencyDateField = null;
		this.gridColumnEx74.CurrencyField = null;
		this.gridColumnEx74.CurrencyFixed = null;
		this.gridColumnEx74.CurrencyFixedRate = 0.0;
		this.gridColumnEx74.CurrencyRateField = null;
		this.gridColumnEx74.FieldName = "SourceField";
		this.gridColumnEx74.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx74.Name = "gridColumnEx74";
		this.gridColumnEx74.PrimaryKey = false;
		this.gridColumnEx74.UnitField = null;
		this.gridColumnEx74.Visible = true;
		this.gridColumnEx74.VisibleIndex = 1;
		this.gridColumnEx74.Width = 121;
		this.rep_ForecastSourceField.AutoHeight = false;
		this.rep_ForecastSourceField.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_ForecastSourceField.Name = "rep_ForecastSourceField";
		this.rep_ForecastSourceField.NullText = "";
		this.gridColumnEx75.Caption = "TargetItem";
		this.gridColumnEx75.CurrencyDateField = null;
		this.gridColumnEx75.CurrencyField = null;
		this.gridColumnEx75.CurrencyFixed = null;
		this.gridColumnEx75.CurrencyFixedRate = 0.0;
		this.gridColumnEx75.CurrencyRateField = null;
		this.gridColumnEx75.FieldName = "TargetItem";
		this.gridColumnEx75.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx75.Name = "gridColumnEx75";
		this.gridColumnEx75.PrimaryKey = false;
		this.gridColumnEx75.UnitField = null;
		this.gridColumnEx76.Caption = "Target Field Name";
		this.gridColumnEx76.ColumnEdit = this.rep_ForecastTargetField;
		this.gridColumnEx76.CurrencyDateField = null;
		this.gridColumnEx76.CurrencyField = null;
		this.gridColumnEx76.CurrencyFixed = null;
		this.gridColumnEx76.CurrencyFixedRate = 0.0;
		this.gridColumnEx76.CurrencyRateField = null;
		this.gridColumnEx76.FieldName = "TargetField";
		this.gridColumnEx76.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx76.Name = "gridColumnEx76";
		this.gridColumnEx76.OptionsColumn.AllowEdit = false;
		this.gridColumnEx76.PrimaryKey = false;
		this.gridColumnEx76.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gridColumnEx76.UnitField = null;
		this.gridColumnEx76.Visible = true;
		this.gridColumnEx76.VisibleIndex = 0;
		this.gridColumnEx76.Width = 114;
		this.rep_ForecastTargetField.AutoHeight = false;
		this.rep_ForecastTargetField.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_ForecastTargetField.Name = "rep_ForecastTargetField";
		this.rep_ForecastTargetField.NullText = "";
		this.gridColumnEx77.Caption = "Default Value";
		this.gridColumnEx77.ColumnEdit = this.repositoryItemTextEdit1;
		this.gridColumnEx77.CurrencyDateField = null;
		this.gridColumnEx77.CurrencyField = null;
		this.gridColumnEx77.CurrencyFixed = null;
		this.gridColumnEx77.CurrencyFixedRate = 0.0;
		this.gridColumnEx77.CurrencyRateField = null;
		this.gridColumnEx77.FieldName = "DfltValue";
		this.gridColumnEx77.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx77.Name = "gridColumnEx77";
		this.gridColumnEx77.PrimaryKey = false;
		this.gridColumnEx77.UnitField = null;
		this.gridColumnEx77.Visible = true;
		this.gridColumnEx77.VisibleIndex = 2;
		this.gridColumnEx77.Width = 175;
		this.repositoryItemTextEdit1.AutoHeight = false;
		this.repositoryItemTextEdit1.Name = "repositoryItemTextEdit1";
		this.xtraTabPageEx2.Controls.Add(this.grd_ForecastItems);
		this.xtraTabPageEx2.EditControl = null;
		this.xtraTabPageEx2.EditValue = "Items";
		this.xtraTabPageEx2.FieldName = "xtraTabPage3";
		this.xtraTabPageEx2.IsSearchControl = false;
		this.xtraTabPageEx2.Name = "xtraTabPageEx2";
		this.xtraTabPageEx2.ReadOnly = false;
		this.xtraTabPageEx2.SettingInVisible = false;
		this.xtraTabPageEx2.SettingReadonly = false;
		this.xtraTabPageEx2.Size = new System.Drawing.Size(929, 263);
		this.xtraTabPageEx2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPageEx2.Text = "Items";
		this.grd_ForecastItems.AllowPasteExcel = false;
		this.grd_ForecastItems.ClipboardData = null;
		this.grd_ForecastItems.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_ForecastItems.Location = new System.Drawing.Point(0, 0);
		this.grd_ForecastItems.MainView = this.gv_ForecastItems;
		this.grd_ForecastItems.Name = "grd_ForecastItems";
		this.grd_ForecastItems.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.rep_ForecastItemTargetField, this.rep_ForecastItemSourceField });
		this.grd_ForecastItems.Size = new System.Drawing.Size(929, 263);
		this.grd_ForecastItems.TabIndex = 1;
		this.grd_ForecastItems.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_ForecastItems });
		this.gv_ForecastItems.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_ForecastItems.ColumnKey = null;
		this.gv_ForecastItems.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[5] { this.gridColumnEx79, this.gridColumnEx80, this.gridColumnEx81, this.gridColumnEx82, this.gridColumnEx83 });
		this.gv_ForecastItems.DisableFormSetting = true;
		this.gv_ForecastItems.DisplayException = false;
		this.gv_ForecastItems.FieldCurrency = null;
		this.gv_ForecastItems.FieldRate = null;
		this.gv_ForecastItems.FormId = "oContextAction";
		this.gv_ForecastItems.GridControl = this.grd_ForecastItems;
		this.gv_ForecastItems.IndicatorWidth = 35;
		this.gv_ForecastItems.IsApzTable = false;
		this.gv_ForecastItems.IsInsertNewRow = false;
		this.gv_ForecastItems.IsLoadingData = false;
		this.gv_ForecastItems.ItemId = "OFORECASTITEM";
		this.gv_ForecastItems.LstColumnAllowBatch = null;
		this.gv_ForecastItems.LstColumnNotAllowBatch = null;
		this.gv_ForecastItems.LstColumnNotEditable = null;
		this.gv_ForecastItems.LstColumNotSetting = null;
		this.gv_ForecastItems.Name = "gv_ForecastItems";
		this.gv_ForecastItems.OptionsView.BestFitMaxRowCount = 100;
		this.gv_ForecastItems.OptionsView.ColumnAutoWidth = false;
		this.gv_ForecastItems.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_ForecastItems.OptionsView.ShowGroupPanel = false;
		this.gv_ForecastItems.UseBatchMod = false;
		this.gridColumnEx79.Caption = "AbsEntry";
		this.gridColumnEx79.CurrencyDateField = null;
		this.gridColumnEx79.CurrencyField = null;
		this.gridColumnEx79.CurrencyFixed = null;
		this.gridColumnEx79.CurrencyFixedRate = 0.0;
		this.gridColumnEx79.CurrencyRateField = null;
		this.gridColumnEx79.FieldName = "AbsEntry";
		this.gridColumnEx79.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx79.Name = "gridColumnEx79";
		this.gridColumnEx79.PrimaryKey = false;
		this.gridColumnEx79.UnitField = null;
		this.gridColumnEx80.Caption = "Source Field Name";
		this.gridColumnEx80.ColumnEdit = this.rep_ForecastItemSourceField;
		this.gridColumnEx80.CurrencyDateField = null;
		this.gridColumnEx80.CurrencyField = null;
		this.gridColumnEx80.CurrencyFixed = null;
		this.gridColumnEx80.CurrencyFixedRate = 0.0;
		this.gridColumnEx80.CurrencyRateField = null;
		this.gridColumnEx80.FieldName = "SourceField";
		this.gridColumnEx80.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx80.Name = "gridColumnEx80";
		this.gridColumnEx80.PrimaryKey = false;
		this.gridColumnEx80.UnitField = null;
		this.gridColumnEx80.Visible = true;
		this.gridColumnEx80.VisibleIndex = 1;
		this.gridColumnEx80.Width = 121;
		this.rep_ForecastItemSourceField.AutoHeight = false;
		this.rep_ForecastItemSourceField.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_ForecastItemSourceField.Name = "rep_ForecastItemSourceField";
		this.rep_ForecastItemSourceField.NullText = "";
		this.gridColumnEx81.Caption = "TargetItem";
		this.gridColumnEx81.CurrencyDateField = null;
		this.gridColumnEx81.CurrencyField = null;
		this.gridColumnEx81.CurrencyFixed = null;
		this.gridColumnEx81.CurrencyFixedRate = 0.0;
		this.gridColumnEx81.CurrencyRateField = null;
		this.gridColumnEx81.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx81.Name = "gridColumnEx81";
		this.gridColumnEx81.PrimaryKey = false;
		this.gridColumnEx81.UnitField = null;
		this.gridColumnEx82.Caption = "Target Field Name";
		this.gridColumnEx82.ColumnEdit = this.rep_ForecastItemTargetField;
		this.gridColumnEx82.CurrencyDateField = null;
		this.gridColumnEx82.CurrencyField = null;
		this.gridColumnEx82.CurrencyFixed = null;
		this.gridColumnEx82.CurrencyFixedRate = 0.0;
		this.gridColumnEx82.CurrencyRateField = null;
		this.gridColumnEx82.FieldName = "TargetField";
		this.gridColumnEx82.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx82.Name = "gridColumnEx82";
		this.gridColumnEx82.OptionsColumn.AllowEdit = false;
		this.gridColumnEx82.PrimaryKey = false;
		this.gridColumnEx82.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gridColumnEx82.UnitField = null;
		this.gridColumnEx82.Visible = true;
		this.gridColumnEx82.VisibleIndex = 0;
		this.gridColumnEx82.Width = 114;
		this.rep_ForecastItemTargetField.AutoHeight = false;
		this.rep_ForecastItemTargetField.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_ForecastItemTargetField.Name = "rep_ForecastItemTargetField";
		this.rep_ForecastItemTargetField.NullText = "";
		this.gridColumnEx83.Caption = "Default Value";
		this.gridColumnEx83.CurrencyDateField = null;
		this.gridColumnEx83.CurrencyField = null;
		this.gridColumnEx83.CurrencyFixed = null;
		this.gridColumnEx83.CurrencyFixedRate = 0.0;
		this.gridColumnEx83.CurrencyRateField = null;
		this.gridColumnEx83.FieldName = "DfltValue";
		this.gridColumnEx83.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx83.Name = "gridColumnEx83";
		this.gridColumnEx83.PrimaryKey = false;
		this.gridColumnEx83.UnitField = null;
		this.gridColumnEx83.Visible = true;
		this.gridColumnEx83.VisibleIndex = 2;
		this.gridColumnEx83.Width = 175;
		this.chk_ForecastShowWindow.EditControl = null;
		this.chk_ForecastShowWindow.EditValue = "Y";
		this.chk_ForecastShowWindow.FieldName = null;
		this.chk_ForecastShowWindow.IsSearchControl = false;
		this.chk_ForecastShowWindow.Location = new System.Drawing.Point(10, 0);
		this.chk_ForecastShowWindow.Name = "chk_ForecastShowWindow";
		this.chk_ForecastShowWindow.Properties.Caption = "Show Window";
		this.chk_ForecastShowWindow.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.chk_ForecastShowWindow.Properties.ValueChecked = "Y";
		this.chk_ForecastShowWindow.Properties.ValueGrayed = "N";
		this.chk_ForecastShowWindow.Properties.ValueUnchecked = "N";
		this.chk_ForecastShowWindow.SettingInVisible = false;
		this.chk_ForecastShowWindow.SettingReadonly = false;
		this.chk_ForecastShowWindow.Size = new System.Drawing.Size(174, 20);
		this.chk_ForecastShowWindow.TabIndex = 6;
		this.chk_ForecastShowWindow.CheckedChanged += new System.EventHandler(this.chk_ForecastShowWindow_CheckedChanged);
		this.tab_DoQuery.Controls.Add(this.txt_TempTableNameDoQuery);
		this.tab_DoQuery.Controls.Add(this.lbl_TempTableNameDoQuery);
		this.tab_DoQuery.Controls.Add(this.chk_UseTempTable);
		this.tab_DoQuery.Controls.Add(this.grd_DoQuery);
		this.tab_DoQuery.Controls.Add(this.memo_QueryString);
		this.tab_DoQuery.Controls.Add(this.txt_QueryString);
		this.tab_DoQuery.Controls.Add(this.look_QueryFrom);
		this.tab_DoQuery.Controls.Add(this.labelLine11);
		this.tab_DoQuery.EditControl = null;
		this.tab_DoQuery.EditValue = "Do Query";
		this.tab_DoQuery.FieldName = null;
		this.tab_DoQuery.IsSearchControl = false;
		this.tab_DoQuery.Name = "tab_DoQuery";
		this.tab_DoQuery.ReadOnly = false;
		this.tab_DoQuery.SettingInVisible = false;
		this.tab_DoQuery.SettingReadonly = false;
		this.tab_DoQuery.Size = new System.Drawing.Size(938, 400);
		this.tab_DoQuery.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_DoQuery.Text = "Do Query";
		this.txt_TempTableNameDoQuery.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.txt_TempTableNameDoQuery.EditControl = null;
		this.txt_TempTableNameDoQuery.FieldName = null;
		this.txt_TempTableNameDoQuery.IsSearchControl = false;
		this.txt_TempTableNameDoQuery.Location = new System.Drawing.Point(761, 2);
		this.txt_TempTableNameDoQuery.Name = "txt_TempTableNameDoQuery";
		this.txt_TempTableNameDoQuery.SettingInVisible = false;
		this.txt_TempTableNameDoQuery.SettingReadonly = false;
		this.txt_TempTableNameDoQuery.Size = new System.Drawing.Size(164, 20);
		this.txt_TempTableNameDoQuery.TabIndex = 17;
		this.txt_TempTableNameDoQuery.Visible = false;
		this.txt_TempTableNameDoQuery.EditValueChanged += new System.EventHandler(this.txt_TempTableNameDoQuery_EditValueChanged);
		this.lbl_TempTableNameDoQuery.EditControl = null;
		this.lbl_TempTableNameDoQuery.EditValue = "Table Name";
		this.lbl_TempTableNameDoQuery.FieldName = null;
		this.lbl_TempTableNameDoQuery.HideWhenEmpty = false;
		this.lbl_TempTableNameDoQuery.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_TempTableNameDoQuery.Location = new System.Drawing.Point(680, 5);
		this.lbl_TempTableNameDoQuery.Name = "lbl_TempTableNameDoQuery";
		this.lbl_TempTableNameDoQuery.ReadOnly = false;
		this.lbl_TempTableNameDoQuery.SettingInVisible = false;
		this.lbl_TempTableNameDoQuery.SettingReadonly = false;
		this.lbl_TempTableNameDoQuery.ShowUnderLine = true;
		this.lbl_TempTableNameDoQuery.Size = new System.Drawing.Size(80, 19);
		this.lbl_TempTableNameDoQuery.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_TempTableNameDoQuery.TabIndex = 16;
		this.lbl_TempTableNameDoQuery.Utext = "Table Name";
		this.lbl_TempTableNameDoQuery.Visible = false;
		this.chk_UseTempTable.EditControl = null;
		this.chk_UseTempTable.FieldName = "IsNightShift";
		this.chk_UseTempTable.IsSearchControl = false;
		this.chk_UseTempTable.Location = new System.Drawing.Point(539, 2);
		this.chk_UseTempTable.Name = "chk_UseTempTable";
		this.chk_UseTempTable.Properties.Caption = "Use Temporary Table";
		this.chk_UseTempTable.Properties.ValueChecked = "Y";
		this.chk_UseTempTable.Properties.ValueUnchecked = "N";
		this.chk_UseTempTable.SettingInVisible = false;
		this.chk_UseTempTable.SettingReadonly = false;
		this.chk_UseTempTable.Size = new System.Drawing.Size(135, 20);
		this.chk_UseTempTable.TabIndex = 15;
		this.chk_UseTempTable.CheckedChanged += new System.EventHandler(this.chk_UseTempTable_CheckedChanged);
		this.grd_DoQuery.AllowPasteExcel = false;
		this.grd_DoQuery.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_DoQuery.ClipboardData = null;
		this.grd_DoQuery.Location = new System.Drawing.Point(0, 24);
		this.grd_DoQuery.MainView = this.gv_DoQuery;
		this.grd_DoQuery.Name = "grd_DoQuery";
		this.grd_DoQuery.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.repositoryItemCheckEdit4, this.rep_QueryDataType, this.rep_QuerySourceField });
		this.grd_DoQuery.Size = new System.Drawing.Size(928, 373);
		this.grd_DoQuery.TabIndex = 14;
		this.grd_DoQuery.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_DoQuery });
		this.gv_DoQuery.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_DoQuery.ColumnKey = null;
		this.gv_DoQuery.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[7] { this.gridColumnEx78, this.gridColumnEx84, this.gridColumnEx85, this.gridColumnEx86, this.gridColumnEx87, this.gridColumnEx88, this.gridColumnEx89 });
		this.gv_DoQuery.DisableFormSetting = true;
		this.gv_DoQuery.DisplayException = false;
		this.gv_DoQuery.FieldCurrency = null;
		this.gv_DoQuery.FieldRate = null;
		this.gv_DoQuery.FormId = "oContextAction";
		this.gv_DoQuery.GridControl = this.grd_DoQuery;
		this.gv_DoQuery.IndicatorWidth = 35;
		this.gv_DoQuery.IsApzTable = false;
		this.gv_DoQuery.IsInsertNewRow = false;
		this.gv_DoQuery.IsLoadingData = false;
		this.gv_DoQuery.ItemId = "ODOQUERY";
		this.gv_DoQuery.LstColumnAllowBatch = null;
		this.gv_DoQuery.LstColumnNotAllowBatch = null;
		this.gv_DoQuery.LstColumnNotEditable = null;
		this.gv_DoQuery.LstColumNotSetting = null;
		this.gv_DoQuery.Name = "gv_DoQuery";
		this.gv_DoQuery.OptionsView.BestFitMaxRowCount = 100;
		this.gv_DoQuery.OptionsView.ColumnAutoWidth = false;
		this.gv_DoQuery.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_DoQuery.OptionsView.ShowGroupPanel = false;
		this.gv_DoQuery.UseBatchMod = false;
		this.gridColumnEx78.Caption = "Name";
		this.gridColumnEx78.CurrencyDateField = null;
		this.gridColumnEx78.CurrencyField = null;
		this.gridColumnEx78.CurrencyFixed = null;
		this.gridColumnEx78.CurrencyFixedRate = 0.0;
		this.gridColumnEx78.CurrencyRateField = null;
		this.gridColumnEx78.FieldName = "ParamName";
		this.gridColumnEx78.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx78.Name = "gridColumnEx78";
		this.gridColumnEx78.OptionsColumn.ReadOnly = true;
		this.gridColumnEx78.PrimaryKey = false;
		this.gridColumnEx78.UnitField = null;
		this.gridColumnEx78.Visible = true;
		this.gridColumnEx78.VisibleIndex = 0;
		this.gridColumnEx78.Width = 100;
		this.gridColumnEx84.Caption = "Label";
		this.gridColumnEx84.CurrencyDateField = null;
		this.gridColumnEx84.CurrencyField = null;
		this.gridColumnEx84.CurrencyFixed = null;
		this.gridColumnEx84.CurrencyFixedRate = 0.0;
		this.gridColumnEx84.CurrencyRateField = null;
		this.gridColumnEx84.FieldName = "Label";
		this.gridColumnEx84.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx84.Name = "gridColumnEx84";
		this.gridColumnEx84.PrimaryKey = false;
		this.gridColumnEx84.UnitField = null;
		this.gridColumnEx84.Visible = true;
		this.gridColumnEx84.VisibleIndex = 1;
		this.gridColumnEx84.Width = 100;
		this.gridColumnEx85.Caption = "Data Type";
		this.gridColumnEx85.ColumnEdit = this.rep_QueryDataType;
		this.gridColumnEx85.CurrencyDateField = null;
		this.gridColumnEx85.CurrencyField = null;
		this.gridColumnEx85.CurrencyFixed = null;
		this.gridColumnEx85.CurrencyFixedRate = 0.0;
		this.gridColumnEx85.CurrencyRateField = null;
		this.gridColumnEx85.FieldName = "DataType";
		this.gridColumnEx85.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx85.Name = "gridColumnEx85";
		this.gridColumnEx85.PrimaryKey = false;
		this.gridColumnEx85.UnitField = null;
		this.gridColumnEx85.Visible = true;
		this.gridColumnEx85.VisibleIndex = 3;
		this.gridColumnEx85.Width = 100;
		this.rep_QueryDataType.AutoHeight = false;
		this.rep_QueryDataType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_QueryDataType.Name = "rep_QueryDataType";
		this.rep_QueryDataType.NullText = "";
		this.gridColumnEx86.Caption = "Source Field";
		this.gridColumnEx86.ColumnEdit = this.rep_QuerySourceField;
		this.gridColumnEx86.CurrencyDateField = null;
		this.gridColumnEx86.CurrencyField = null;
		this.gridColumnEx86.CurrencyFixed = null;
		this.gridColumnEx86.CurrencyFixedRate = 0.0;
		this.gridColumnEx86.CurrencyRateField = null;
		this.gridColumnEx86.FieldName = "SourceField";
		this.gridColumnEx86.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx86.Name = "gridColumnEx86";
		this.gridColumnEx86.PrimaryKey = false;
		this.gridColumnEx86.UnitField = null;
		this.gridColumnEx86.Visible = true;
		this.gridColumnEx86.VisibleIndex = 2;
		this.gridColumnEx86.Width = 100;
		this.rep_QuerySourceField.AutoHeight = false;
		this.rep_QuerySourceField.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_QuerySourceField.Name = "rep_QuerySourceField";
		this.rep_QuerySourceField.NullText = "";
		this.gridColumnEx87.Caption = "Sub. Type";
		this.gridColumnEx87.CurrencyDateField = null;
		this.gridColumnEx87.CurrencyField = null;
		this.gridColumnEx87.CurrencyFixed = null;
		this.gridColumnEx87.CurrencyFixedRate = 0.0;
		this.gridColumnEx87.CurrencyRateField = null;
		this.gridColumnEx87.FieldName = "SubType";
		this.gridColumnEx87.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx87.Name = "gridColumnEx87";
		this.gridColumnEx87.PrimaryKey = false;
		this.gridColumnEx87.UnitField = null;
		this.gridColumnEx87.Visible = true;
		this.gridColumnEx87.VisibleIndex = 4;
		this.gridColumnEx87.Width = 100;
		this.gridColumnEx88.Caption = "Default Value";
		this.gridColumnEx88.CurrencyDateField = null;
		this.gridColumnEx88.CurrencyField = null;
		this.gridColumnEx88.CurrencyFixed = null;
		this.gridColumnEx88.CurrencyFixedRate = 0.0;
		this.gridColumnEx88.CurrencyRateField = null;
		this.gridColumnEx88.FieldName = "DefaultValue";
		this.gridColumnEx88.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx88.Name = "gridColumnEx88";
		this.gridColumnEx88.PrimaryKey = false;
		this.gridColumnEx88.UnitField = null;
		this.gridColumnEx88.Visible = true;
		this.gridColumnEx88.VisibleIndex = 5;
		this.gridColumnEx88.Width = 100;
		this.gridColumnEx89.Caption = "Active";
		this.gridColumnEx89.ColumnEdit = this.repositoryItemCheckEdit4;
		this.gridColumnEx89.CurrencyDateField = null;
		this.gridColumnEx89.CurrencyField = null;
		this.gridColumnEx89.CurrencyFixed = null;
		this.gridColumnEx89.CurrencyFixedRate = 0.0;
		this.gridColumnEx89.CurrencyRateField = null;
		this.gridColumnEx89.FieldName = "Active";
		this.gridColumnEx89.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx89.Name = "gridColumnEx89";
		this.gridColumnEx89.PrimaryKey = false;
		this.gridColumnEx89.UnitField = null;
		this.gridColumnEx89.Visible = true;
		this.gridColumnEx89.VisibleIndex = 6;
		this.repositoryItemCheckEdit4.AutoHeight = false;
		this.repositoryItemCheckEdit4.Name = "repositoryItemCheckEdit4";
		this.repositoryItemCheckEdit4.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.repositoryItemCheckEdit4.ValueChecked = "Y";
		this.repositoryItemCheckEdit4.ValueGrayed = "N";
		this.repositoryItemCheckEdit4.ValueUnchecked = "N";
		this.memo_QueryString.EditControl = null;
		this.memo_QueryString.FieldName = null;
		this.memo_QueryString.IsSearchControl = false;
		this.memo_QueryString.Location = new System.Drawing.Point(432, 2);
		this.memo_QueryString.Name = "memo_QueryString";
		this.memo_QueryString.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.memo_QueryString.Properties.ShowIcon = false;
		this.memo_QueryString.SettingInVisible = false;
		this.memo_QueryString.SettingReadonly = false;
		this.memo_QueryString.Size = new System.Drawing.Size(77, 20);
		this.memo_QueryString.TabIndex = 13;
		this.memo_QueryString.Visible = false;
		this.memo_QueryString.EditValueChanged += new System.EventHandler(this.memo_QueryString_EditValueChanged);
		this.txt_QueryString.EditControl = null;
		this.txt_QueryString.FieldName = null;
		this.txt_QueryString.IsSearchControl = false;
		this.txt_QueryString.Location = new System.Drawing.Point(194, 2);
		this.txt_QueryString.Name = "txt_QueryString";
		this.txt_QueryString.Properties.AutoLoadDataSource = true;
		this.txt_QueryString.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.txt_QueryString.Properties.ContainerControl = null;
		this.txt_QueryString.Properties.DisplayMember = "";
		this.txt_QueryString.Properties.DisplayMember2 = "";
		this.txt_QueryString.Properties.ListMandatoryField = null;
		this.txt_QueryString.Properties.LoadDataSourceWhenEnter = false;
		this.txt_QueryString.Properties.SeparateCharacterForMulti = ';';
		this.txt_QueryString.Properties.SeparateString = ", ";
		this.txt_QueryString.Properties.UsingAutoCompleteBox = false;
		this.txt_QueryString.Properties.UsingDataSource = true;
		this.txt_QueryString.Properties.ValidateWhenNoDataSource = false;
		this.txt_QueryString.Properties.ValueMember = "";
		this.txt_QueryString.SettingInVisible = false;
		this.txt_QueryString.SettingReadonly = false;
		this.txt_QueryString.Size = new System.Drawing.Size(232, 20);
		this.txt_QueryString.TabIndex = 12;
		this.txt_QueryString.EditValueChanged += new System.EventHandler(this.txt_QueryString_EditValueChanged);
		this.look_QueryFrom.EditControl = null;
		this.look_QueryFrom.FieldName = null;
		this.look_QueryFrom.IsSearchControl = false;
		this.look_QueryFrom.Location = new System.Drawing.Point(88, 2);
		this.look_QueryFrom.Name = "look_QueryFrom";
		this.look_QueryFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.look_QueryFrom.Properties.NullText = "";
		this.look_QueryFrom.SettingInVisible = false;
		this.look_QueryFrom.SettingReadonly = false;
		this.look_QueryFrom.Size = new System.Drawing.Size(100, 20);
		this.look_QueryFrom.TabIndex = 11;
		this.look_QueryFrom.EditValueChanged += new System.EventHandler(this.look_QueryFrom_EditValueChanged);
		this.labelLine11.EditControl = null;
		this.labelLine11.EditValue = "Query Form";
		this.labelLine11.FieldName = null;
		this.labelLine11.HideWhenEmpty = false;
		this.labelLine11.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine11.Location = new System.Drawing.Point(11, 5);
		this.labelLine11.Name = "labelLine11";
		this.labelLine11.ReadOnly = false;
		this.labelLine11.SettingInVisible = false;
		this.labelLine11.SettingReadonly = false;
		this.labelLine11.ShowUnderLine = true;
		this.labelLine11.Size = new System.Drawing.Size(80, 20);
		this.labelLine11.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine11.TabIndex = 10;
		this.labelLine11.Utext = "Query Form";
		this.tab_PrintLayout.Controls.Add(this.look_PrintAction);
		this.tab_PrintLayout.Controls.Add(this.look_RptUseSourceFrom);
		this.tab_PrintLayout.Controls.Add(this.labelLine15);
		this.tab_PrintLayout.Controls.Add(this.grd_PrintLayout);
		this.tab_PrintLayout.Controls.Add(this.look_PrintLayoutFileType);
		this.tab_PrintLayout.Controls.Add(this.labelLine14);
		this.tab_PrintLayout.Controls.Add(this.bte_PrintLayoutPath);
		this.tab_PrintLayout.Controls.Add(this.labelLine13);
		this.tab_PrintLayout.EditControl = null;
		this.tab_PrintLayout.EditValue = "Print Layout";
		this.tab_PrintLayout.FieldName = null;
		this.tab_PrintLayout.IsSearchControl = false;
		this.tab_PrintLayout.Name = "tab_PrintLayout";
		this.tab_PrintLayout.ReadOnly = false;
		this.tab_PrintLayout.SettingInVisible = false;
		this.tab_PrintLayout.SettingReadonly = false;
		this.tab_PrintLayout.Size = new System.Drawing.Size(938, 400);
		this.tab_PrintLayout.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_PrintLayout.Text = "Print Layout";
		this.look_PrintAction.EditControl = null;
		this.look_PrintAction.FieldName = null;
		this.look_PrintAction.IsSearchControl = false;
		this.look_PrintAction.Location = new System.Drawing.Point(797, 16);
		this.look_PrintAction.Name = "look_PrintAction";
		this.look_PrintAction.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.look_PrintAction.Properties.NullText = "";
		this.look_PrintAction.SettingInVisible = false;
		this.look_PrintAction.SettingReadonly = false;
		this.look_PrintAction.Size = new System.Drawing.Size(134, 20);
		this.look_PrintAction.TabIndex = 8;
		this.look_PrintAction.EditValueChanged += new System.EventHandler(this.look_PrintAction_EditValueChanged);
		this.look_RptUseSourceFrom.EditControl = null;
		this.look_RptUseSourceFrom.FieldName = null;
		this.look_RptUseSourceFrom.IsSearchControl = false;
		this.look_RptUseSourceFrom.Location = new System.Drawing.Point(661, 16);
		this.look_RptUseSourceFrom.Name = "look_RptUseSourceFrom";
		this.look_RptUseSourceFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.look_RptUseSourceFrom.Properties.NullText = "";
		this.look_RptUseSourceFrom.SettingInVisible = false;
		this.look_RptUseSourceFrom.SettingReadonly = false;
		this.look_RptUseSourceFrom.Size = new System.Drawing.Size(130, 20);
		this.look_RptUseSourceFrom.TabIndex = 7;
		this.look_RptUseSourceFrom.ToolTip = resources.GetString("look_RptUseSourceFrom.ToolTip");
		this.look_RptUseSourceFrom.ToolTipController = this.toolTipController1;
		this.look_RptUseSourceFrom.ToolTipIconType = DevExpress.Utils.ToolTipIconType.Information;
		this.look_RptUseSourceFrom.ToolTipTitle = "Select the data source to display on the report";
		this.look_RptUseSourceFrom.EditValueChanged += new System.EventHandler(this.look_RptUseSourceFrom_EditValueChanged);
		this.toolTipController1.AllowHtmlText = true;
		this.toolTipController1.AutoPopDelay = 10000;
		this.toolTipController1.ToolTipType = DevExpress.Utils.ToolTipType.SuperTip;
		this.labelLine15.EditControl = null;
		this.labelLine15.EditValue = "Use Source From";
		this.labelLine15.FieldName = null;
		this.labelLine15.HideWhenEmpty = false;
		this.labelLine15.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine15.Location = new System.Drawing.Point(555, 19);
		this.labelLine15.Name = "labelLine15";
		this.labelLine15.ReadOnly = false;
		this.labelLine15.SettingInVisible = false;
		this.labelLine15.SettingReadonly = false;
		this.labelLine15.ShowUnderLine = true;
		this.labelLine15.Size = new System.Drawing.Size(107, 20);
		this.labelLine15.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine15.TabIndex = 6;
		this.labelLine15.Utext = "Use Source From";
		this.grd_PrintLayout.AllowPasteExcel = false;
		this.grd_PrintLayout.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_PrintLayout.ClipboardData = null;
		this.grd_PrintLayout.Location = new System.Drawing.Point(3, 45);
		this.grd_PrintLayout.MainView = this.gv_PrintLayout;
		this.grd_PrintLayout.Name = "grd_PrintLayout";
		this.grd_PrintLayout.Size = new System.Drawing.Size(929, 352);
		this.grd_PrintLayout.TabIndex = 5;
		this.grd_PrintLayout.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_PrintLayout });
		this.gv_PrintLayout.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_PrintLayout.ColumnKey = null;
		this.gv_PrintLayout.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[7] { this.gridColumnEx90, this.gridColumnEx91, this.gridColumnEx92, this.gridColumnEx93, this.gridColumnEx94, this.gridColumnEx95, this.gridColumnEx96 });
		this.gv_PrintLayout.DisableFormSetting = true;
		this.gv_PrintLayout.DisplayException = false;
		this.gv_PrintLayout.FieldCurrency = null;
		this.gv_PrintLayout.FieldRate = null;
		this.gv_PrintLayout.FormId = "oContextAction";
		this.gv_PrintLayout.GridControl = this.grd_PrintLayout;
		this.gv_PrintLayout.IndicatorWidth = 35;
		this.gv_PrintLayout.IsApzTable = false;
		this.gv_PrintLayout.IsInsertNewRow = false;
		this.gv_PrintLayout.IsLoadingData = false;
		this.gv_PrintLayout.ItemId = "OPRINTLAYOUT";
		this.gv_PrintLayout.LstColumnAllowBatch = null;
		this.gv_PrintLayout.LstColumnNotAllowBatch = null;
		this.gv_PrintLayout.LstColumnNotEditable = null;
		this.gv_PrintLayout.LstColumNotSetting = null;
		this.gv_PrintLayout.Name = "gv_PrintLayout";
		this.gv_PrintLayout.OptionsView.BestFitMaxRowCount = 100;
		this.gv_PrintLayout.OptionsView.ColumnAutoWidth = false;
		this.gv_PrintLayout.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_PrintLayout.OptionsView.ShowGroupPanel = false;
		this.gv_PrintLayout.OptionsView.ShowViewCaption = true;
		this.gv_PrintLayout.UseBatchMod = false;
		this.gv_PrintLayout.ViewCaption = "Parameters";
		this.gridColumnEx90.Caption = "Name";
		this.gridColumnEx90.CurrencyDateField = null;
		this.gridColumnEx90.CurrencyField = null;
		this.gridColumnEx90.CurrencyFixed = null;
		this.gridColumnEx90.CurrencyFixedRate = 0.0;
		this.gridColumnEx90.CurrencyRateField = null;
		this.gridColumnEx90.FieldName = "ParamName";
		this.gridColumnEx90.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx90.Name = "gridColumnEx90";
		this.gridColumnEx90.OptionsColumn.ReadOnly = true;
		this.gridColumnEx90.PrimaryKey = false;
		this.gridColumnEx90.UnitField = null;
		this.gridColumnEx90.Visible = true;
		this.gridColumnEx90.VisibleIndex = 0;
		this.gridColumnEx90.Width = 100;
		this.gridColumnEx91.Caption = "Label";
		this.gridColumnEx91.CurrencyDateField = null;
		this.gridColumnEx91.CurrencyField = null;
		this.gridColumnEx91.CurrencyFixed = null;
		this.gridColumnEx91.CurrencyFixedRate = 0.0;
		this.gridColumnEx91.CurrencyRateField = null;
		this.gridColumnEx91.FieldName = "Label";
		this.gridColumnEx91.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx91.Name = "gridColumnEx91";
		this.gridColumnEx91.PrimaryKey = false;
		this.gridColumnEx91.UnitField = null;
		this.gridColumnEx91.Visible = true;
		this.gridColumnEx91.VisibleIndex = 1;
		this.gridColumnEx91.Width = 118;
		this.gridColumnEx92.Caption = "Data Type";
		this.gridColumnEx92.ColumnEdit = this.rep_PrintLayoutDataType;
		this.gridColumnEx92.CurrencyDateField = null;
		this.gridColumnEx92.CurrencyField = null;
		this.gridColumnEx92.CurrencyFixed = null;
		this.gridColumnEx92.CurrencyFixedRate = 0.0;
		this.gridColumnEx92.CurrencyRateField = null;
		this.gridColumnEx92.FieldName = "DataType";
		this.gridColumnEx92.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx92.Name = "gridColumnEx92";
		this.gridColumnEx92.PrimaryKey = false;
		this.gridColumnEx92.UnitField = null;
		this.gridColumnEx92.Visible = true;
		this.gridColumnEx92.VisibleIndex = 2;
		this.gridColumnEx92.Width = 100;
		this.rep_PrintLayoutDataType.AutoHeight = false;
		this.rep_PrintLayoutDataType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_PrintLayoutDataType.Name = "rep_PrintLayoutDataType";
		this.rep_PrintLayoutDataType.NullText = "";
		this.gridColumnEx93.Caption = "Source Field";
		this.gridColumnEx93.ColumnEdit = this.rep_PrintLayoutSourceField;
		this.gridColumnEx93.CurrencyDateField = null;
		this.gridColumnEx93.CurrencyField = null;
		this.gridColumnEx93.CurrencyFixed = null;
		this.gridColumnEx93.CurrencyFixedRate = 0.0;
		this.gridColumnEx93.CurrencyRateField = null;
		this.gridColumnEx93.FieldName = "SourceField";
		this.gridColumnEx93.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx93.Name = "gridColumnEx93";
		this.gridColumnEx93.PrimaryKey = false;
		this.gridColumnEx93.UnitField = null;
		this.gridColumnEx93.Visible = true;
		this.gridColumnEx93.VisibleIndex = 3;
		this.gridColumnEx93.Width = 103;
		this.rep_PrintLayoutSourceField.AutoHeight = false;
		this.rep_PrintLayoutSourceField.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_PrintLayoutSourceField.Name = "rep_PrintLayoutSourceField";
		this.rep_PrintLayoutSourceField.NullText = "";
		this.gridColumnEx94.Caption = "Sub. Type";
		this.gridColumnEx94.CurrencyDateField = null;
		this.gridColumnEx94.CurrencyField = null;
		this.gridColumnEx94.CurrencyFixed = null;
		this.gridColumnEx94.CurrencyFixedRate = 0.0;
		this.gridColumnEx94.CurrencyRateField = null;
		this.gridColumnEx94.FieldName = "SubType";
		this.gridColumnEx94.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx94.Name = "gridColumnEx94";
		this.gridColumnEx94.PrimaryKey = false;
		this.gridColumnEx94.UnitField = null;
		this.gridColumnEx94.Width = 100;
		this.gridColumnEx95.Caption = "Default Value";
		this.gridColumnEx95.CurrencyDateField = null;
		this.gridColumnEx95.CurrencyField = null;
		this.gridColumnEx95.CurrencyFixed = null;
		this.gridColumnEx95.CurrencyFixedRate = 0.0;
		this.gridColumnEx95.CurrencyRateField = null;
		this.gridColumnEx95.FieldName = "DfltValue";
		this.gridColumnEx95.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx95.Name = "gridColumnEx95";
		this.gridColumnEx95.PrimaryKey = false;
		this.gridColumnEx95.UnitField = null;
		this.gridColumnEx95.Visible = true;
		this.gridColumnEx95.VisibleIndex = 4;
		this.gridColumnEx95.Width = 100;
		this.gridColumnEx96.Caption = "Active";
		this.gridColumnEx96.ColumnEdit = this.repositoryItemCheckEdit3;
		this.gridColumnEx96.CurrencyDateField = null;
		this.gridColumnEx96.CurrencyField = null;
		this.gridColumnEx96.CurrencyFixed = null;
		this.gridColumnEx96.CurrencyFixedRate = 0.0;
		this.gridColumnEx96.CurrencyRateField = null;
		this.gridColumnEx96.FieldName = "Active";
		this.gridColumnEx96.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx96.Name = "gridColumnEx96";
		this.gridColumnEx96.PrimaryKey = false;
		this.gridColumnEx96.UnitField = null;
		this.look_PrintLayoutFileType.EditControl = null;
		this.look_PrintLayoutFileType.FieldName = null;
		this.look_PrintLayoutFileType.IsSearchControl = false;
		this.look_PrintLayoutFileType.Location = new System.Drawing.Point(455, 16);
		this.look_PrintLayoutFileType.Name = "look_PrintLayoutFileType";
		this.look_PrintLayoutFileType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.look_PrintLayoutFileType.Properties.NullText = "";
		this.look_PrintLayoutFileType.SettingInVisible = false;
		this.look_PrintLayoutFileType.SettingReadonly = false;
		this.look_PrintLayoutFileType.Size = new System.Drawing.Size(83, 20);
		this.look_PrintLayoutFileType.TabIndex = 3;
		this.look_PrintLayoutFileType.EditValueChanged += new System.EventHandler(this.look_PrintLayoutFileType_EditValueChanged);
		this.labelLine14.EditControl = null;
		this.labelLine14.EditValue = "File Type";
		this.labelLine14.FieldName = null;
		this.labelLine14.HideWhenEmpty = false;
		this.labelLine14.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine14.Location = new System.Drawing.Point(380, 19);
		this.labelLine14.Name = "labelLine14";
		this.labelLine14.ReadOnly = false;
		this.labelLine14.SettingInVisible = false;
		this.labelLine14.SettingReadonly = false;
		this.labelLine14.ShowUnderLine = true;
		this.labelLine14.Size = new System.Drawing.Size(80, 20);
		this.labelLine14.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine14.TabIndex = 2;
		this.labelLine14.Utext = "File Type";
		this.bte_PrintLayoutPath.EditControl = null;
		this.bte_PrintLayoutPath.FieldName = null;
		this.bte_PrintLayoutPath.IsSearchControl = false;
		this.bte_PrintLayoutPath.Location = new System.Drawing.Point(89, 12);
		this.bte_PrintLayoutPath.Name = "bte_PrintLayoutPath";
		editorButtonImageOptions.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions2.Image");
		this.bte_PrintLayoutPath.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.bte_PrintLayoutPath.SettingInVisible = false;
		this.bte_PrintLayoutPath.SettingReadonly = false;
		this.bte_PrintLayoutPath.Size = new System.Drawing.Size(281, 24);
		this.bte_PrintLayoutPath.TabIndex = 1;
		this.bte_PrintLayoutPath.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.bte_PrintLayoutPath_ButtonClick);
		this.bte_PrintLayoutPath.EditValueChanged += new System.EventHandler(this.bte_PrintLayoutPath_EditValueChanged);
		this.labelLine13.EditControl = null;
		this.labelLine13.EditValue = "Path";
		this.labelLine13.FieldName = null;
		this.labelLine13.HideWhenEmpty = false;
		this.labelLine13.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine13.Location = new System.Drawing.Point(10, 19);
		this.labelLine13.Name = "labelLine13";
		this.labelLine13.ReadOnly = false;
		this.labelLine13.SettingInVisible = false;
		this.labelLine13.SettingReadonly = false;
		this.labelLine13.ShowUnderLine = true;
		this.labelLine13.Size = new System.Drawing.Size(80, 20);
		this.labelLine13.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine13.TabIndex = 0;
		this.labelLine13.Utext = "Path";
		this.tab_ApprovalDecision.Controls.Add(this.grd_Approval);
		this.tab_ApprovalDecision.EditControl = null;
		this.tab_ApprovalDecision.EditValue = "Approval Decision";
		this.tab_ApprovalDecision.FieldName = null;
		this.tab_ApprovalDecision.IsSearchControl = false;
		this.tab_ApprovalDecision.Name = "tab_ApprovalDecision";
		this.tab_ApprovalDecision.ReadOnly = false;
		this.tab_ApprovalDecision.SettingInVisible = false;
		this.tab_ApprovalDecision.SettingReadonly = false;
		this.tab_ApprovalDecision.Size = new System.Drawing.Size(938, 400);
		this.tab_ApprovalDecision.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_ApprovalDecision.Text = "Approval Decision";
		this.grd_Approval.AllowPasteExcel = false;
		this.grd_Approval.ClipboardData = null;
		this.grd_Approval.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Approval.Location = new System.Drawing.Point(0, 0);
		this.grd_Approval.MainView = this.gv_Approval;
		this.grd_Approval.Name = "grd_Approval";
		this.grd_Approval.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.rep_ApprovalTargetField, this.rep_ApprovalSourceField, this.repositoryItemLookUpEdit3 });
		this.grd_Approval.Size = new System.Drawing.Size(938, 400);
		this.grd_Approval.TabIndex = 1;
		this.grd_Approval.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Approval });
		this.gv_Approval.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Approval.ColumnKey = null;
		this.gv_Approval.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[5] { this.gridColumnEx97, this.gridColumnEx98, this.gridColumnEx99, this.gridColumnEx100, this.gridColumnEx101 });
		this.gv_Approval.DisableFormSetting = true;
		this.gv_Approval.DisplayException = false;
		this.gv_Approval.FieldCurrency = null;
		this.gv_Approval.FieldRate = null;
		this.gv_Approval.FormId = "oContextAction";
		this.gv_Approval.GridControl = this.grd_Approval;
		this.gv_Approval.IndicatorWidth = 35;
		this.gv_Approval.IsApzTable = false;
		this.gv_Approval.IsInsertNewRow = false;
		this.gv_Approval.IsLoadingData = false;
		this.gv_Approval.ItemId = "OAPPROVAL";
		this.gv_Approval.LstColumnAllowBatch = null;
		this.gv_Approval.LstColumnNotAllowBatch = null;
		this.gv_Approval.LstColumnNotEditable = null;
		this.gv_Approval.LstColumNotSetting = null;
		this.gv_Approval.Name = "gv_Approval";
		this.gv_Approval.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Approval.OptionsView.ColumnAutoWidth = false;
		this.gv_Approval.OptionsView.ShowGroupPanel = false;
		this.gv_Approval.UseBatchMod = false;
		this.gridColumnEx97.Caption = "AbsEntry";
		this.gridColumnEx97.CurrencyDateField = null;
		this.gridColumnEx97.CurrencyField = null;
		this.gridColumnEx97.CurrencyFixed = null;
		this.gridColumnEx97.CurrencyFixedRate = 0.0;
		this.gridColumnEx97.CurrencyRateField = null;
		this.gridColumnEx97.FieldName = "AbsEntry";
		this.gridColumnEx97.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx97.Name = "gridColumnEx97";
		this.gridColumnEx97.PrimaryKey = false;
		this.gridColumnEx97.UnitField = null;
		this.gridColumnEx98.Caption = "Source Field Name";
		this.gridColumnEx98.ColumnEdit = this.rep_ApprovalSourceField;
		this.gridColumnEx98.CurrencyDateField = null;
		this.gridColumnEx98.CurrencyField = null;
		this.gridColumnEx98.CurrencyFixed = null;
		this.gridColumnEx98.CurrencyFixedRate = 0.0;
		this.gridColumnEx98.CurrencyRateField = null;
		this.gridColumnEx98.FieldName = "SourceField";
		this.gridColumnEx98.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx98.Name = "gridColumnEx98";
		this.gridColumnEx98.PrimaryKey = false;
		this.gridColumnEx98.UnitField = null;
		this.gridColumnEx98.Visible = true;
		this.gridColumnEx98.VisibleIndex = 1;
		this.gridColumnEx98.Width = 121;
		this.rep_ApprovalSourceField.AutoHeight = false;
		this.rep_ApprovalSourceField.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_ApprovalSourceField.Name = "rep_ApprovalSourceField";
		this.rep_ApprovalSourceField.NullText = "";
		this.gridColumnEx99.Caption = "TargetItem";
		this.gridColumnEx99.CurrencyDateField = null;
		this.gridColumnEx99.CurrencyField = null;
		this.gridColumnEx99.CurrencyFixed = null;
		this.gridColumnEx99.CurrencyFixedRate = 0.0;
		this.gridColumnEx99.CurrencyRateField = null;
		this.gridColumnEx99.FieldName = "TargetItem";
		this.gridColumnEx99.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx99.Name = "gridColumnEx99";
		this.gridColumnEx99.PrimaryKey = false;
		this.gridColumnEx99.UnitField = null;
		this.gridColumnEx100.Caption = "Target Field Name";
		this.gridColumnEx100.ColumnEdit = this.rep_ApprovalTargetField;
		this.gridColumnEx100.CurrencyDateField = null;
		this.gridColumnEx100.CurrencyField = null;
		this.gridColumnEx100.CurrencyFixed = null;
		this.gridColumnEx100.CurrencyFixedRate = 0.0;
		this.gridColumnEx100.CurrencyRateField = null;
		this.gridColumnEx100.FieldName = "TargetField";
		this.gridColumnEx100.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx100.Name = "gridColumnEx100";
		this.gridColumnEx100.OptionsColumn.AllowEdit = false;
		this.gridColumnEx100.PrimaryKey = false;
		this.gridColumnEx100.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gridColumnEx100.UnitField = null;
		this.gridColumnEx100.Visible = true;
		this.gridColumnEx100.VisibleIndex = 0;
		this.gridColumnEx100.Width = 114;
		this.rep_ApprovalTargetField.AutoHeight = false;
		this.rep_ApprovalTargetField.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_ApprovalTargetField.Name = "rep_ApprovalTargetField";
		this.rep_ApprovalTargetField.NullText = "";
		this.gridColumnEx101.Caption = "Default Value";
		this.gridColumnEx101.CurrencyDateField = null;
		this.gridColumnEx101.CurrencyField = null;
		this.gridColumnEx101.CurrencyFixed = null;
		this.gridColumnEx101.CurrencyFixedRate = 0.0;
		this.gridColumnEx101.CurrencyRateField = null;
		this.gridColumnEx101.FieldName = "DfltValue";
		this.gridColumnEx101.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx101.Name = "gridColumnEx101";
		this.gridColumnEx101.PrimaryKey = false;
		this.gridColumnEx101.UnitField = null;
		this.gridColumnEx101.Visible = true;
		this.gridColumnEx101.VisibleIndex = 2;
		this.gridColumnEx101.Width = 175;
		this.repositoryItemLookUpEdit3.AutoHeight = false;
		this.repositoryItemLookUpEdit3.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemLookUpEdit3.Name = "repositoryItemLookUpEdit3";
		this.repositoryItemLookUpEdit3.NullText = "";
		this.tab_DIYScript.Controls.Add(this.rtxt_Script);
		this.tab_DIYScript.Controls.Add(this.btn_EditScript);
		this.tab_DIYScript.EditControl = null;
		this.tab_DIYScript.EditValue = "DIY Script";
		this.tab_DIYScript.FieldName = null;
		this.tab_DIYScript.IsSearchControl = false;
		this.tab_DIYScript.Name = "tab_DIYScript";
		this.tab_DIYScript.ReadOnly = false;
		this.tab_DIYScript.SettingInVisible = false;
		this.tab_DIYScript.SettingReadonly = false;
		this.tab_DIYScript.Size = new System.Drawing.Size(938, 400);
		this.tab_DIYScript.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_DIYScript.Text = "DIY Script";
		this.rtxt_Script.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.rtxt_Script.BackColor = System.Drawing.Color.White;
		this.rtxt_Script.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		this.rtxt_Script.Location = new System.Drawing.Point(11, 51);
		this.rtxt_Script.Name = "rtxt_Script";
		this.rtxt_Script.ReadOnly = true;
		this.rtxt_Script.Size = new System.Drawing.Size(898, 326);
		this.rtxt_Script.TabIndex = 1;
		this.rtxt_Script.Text = "";
		this.rtxt_Script.TextChanged += new System.EventHandler(this.rtxt_Script_TextChanged);
		this.btn_EditScript.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
		this.btn_EditScript.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_EditScript.ImageOptions.Image");
		this.btn_EditScript.Location = new System.Drawing.Point(11, 22);
		this.btn_EditScript.Name = "btn_EditScript";
		this.btn_EditScript.Size = new System.Drawing.Size(133, 23);
		this.btn_EditScript.TabIndex = 0;
		this.btn_EditScript.Text = "Create New Script";
		this.btn_EditScript.Click += new System.EventHandler(this.btn_EditScript_Click);
		this.tab_HttpRequest.Controls.Add(this.ucHttpRequest);
		this.tab_HttpRequest.EditControl = null;
		this.tab_HttpRequest.EditValue = "HttpRequest";
		this.tab_HttpRequest.FieldName = null;
		this.tab_HttpRequest.IsSearchControl = false;
		this.tab_HttpRequest.Name = "tab_HttpRequest";
		this.tab_HttpRequest.ReadOnly = false;
		this.tab_HttpRequest.SettingInVisible = false;
		this.tab_HttpRequest.SettingReadonly = false;
		this.tab_HttpRequest.Size = new System.Drawing.Size(938, 400);
		this.tab_HttpRequest.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_HttpRequest.Text = "HttpRequest";
		this.ucHttpRequest.ActionEntry = 0;
		this.ucHttpRequest.ActionFormId = null;
		this.ucHttpRequest.ActionItemId = null;
		this.ucHttpRequest.AuthItemId = "OAUTH";
		this.ucHttpRequest.BodyItemId = "OBODY";
		this.ucHttpRequest.Dock = System.Windows.Forms.DockStyle.Fill;
		this.ucHttpRequest.FormId = null;
		this.ucHttpRequest.HeaderItemId = "OHEADER";
		this.ucHttpRequest.Location = new System.Drawing.Point(0, 0);
		this.ucHttpRequest.MappingStructure = null;
		this.ucHttpRequest.Name = "ucHttpRequest";
		this.ucHttpRequest.QueryItemId = "OQUERY";
		this.ucHttpRequest.Size = new System.Drawing.Size(938, 400);
		this.ucHttpRequest.TabIndex = 0;
		this.tab_PhysicalTable.Controls.Add(this.grd_PhysicalTable);
		this.tab_PhysicalTable.Controls.Add(this.txt_PhysicalTableName);
		this.tab_PhysicalTable.Controls.Add(this.labelLine12);
		this.tab_PhysicalTable.EditControl = null;
		this.tab_PhysicalTable.EditValue = "Insert data into physical table";
		this.tab_PhysicalTable.FieldName = "";
		this.tab_PhysicalTable.IsSearchControl = false;
		this.tab_PhysicalTable.Name = "tab_PhysicalTable";
		this.tab_PhysicalTable.ReadOnly = false;
		this.tab_PhysicalTable.SettingInVisible = false;
		this.tab_PhysicalTable.SettingReadonly = false;
		this.tab_PhysicalTable.Size = new System.Drawing.Size(938, 400);
		this.tab_PhysicalTable.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_PhysicalTable.Text = "Insert data into physical table";
		this.grd_PhysicalTable.AllowPasteExcel = false;
		this.grd_PhysicalTable.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_PhysicalTable.ClipboardData = null;
		this.grd_PhysicalTable.Location = new System.Drawing.Point(1, 44);
		this.grd_PhysicalTable.MainView = this.gv_PhysicalTable;
		this.grd_PhysicalTable.Name = "grd_PhysicalTable";
		this.grd_PhysicalTable.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.rep_PhysicalTableTargetField, this.rep_PhysicalTableSourceField, this.repositoryItemLookUpEdit4 });
		this.grd_PhysicalTable.Size = new System.Drawing.Size(934, 357);
		this.grd_PhysicalTable.TabIndex = 2;
		this.grd_PhysicalTable.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_PhysicalTable });
		this.gv_PhysicalTable.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_PhysicalTable.ColumnKey = null;
		this.gv_PhysicalTable.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[6] { this.gridColumnEx102, this.gridColumnEx103, this.gridColumnEx104, this.gridColumnEx105, this.gridColumnEx106, this.gridColumnEx107 });
		this.gv_PhysicalTable.DisableFormSetting = true;
		this.gv_PhysicalTable.DisplayException = false;
		this.gv_PhysicalTable.FieldCurrency = null;
		this.gv_PhysicalTable.FieldRate = null;
		this.gv_PhysicalTable.FormId = "oContextAction";
		this.gv_PhysicalTable.GridControl = this.grd_PhysicalTable;
		this.gv_PhysicalTable.IndicatorWidth = 35;
		this.gv_PhysicalTable.IsApzTable = false;
		this.gv_PhysicalTable.IsInsertNewRow = false;
		this.gv_PhysicalTable.IsLoadingData = false;
		this.gv_PhysicalTable.ItemId = "OHEADER";
		this.gv_PhysicalTable.LstColumnAllowBatch = null;
		this.gv_PhysicalTable.LstColumnNotAllowBatch = null;
		this.gv_PhysicalTable.LstColumnNotEditable = null;
		this.gv_PhysicalTable.LstColumNotSetting = null;
		this.gv_PhysicalTable.Name = "gv_PhysicalTable";
		this.gv_PhysicalTable.OptionsView.BestFitMaxRowCount = 100;
		this.gv_PhysicalTable.OptionsView.ColumnAutoWidth = false;
		this.gv_PhysicalTable.OptionsView.ShowGroupPanel = false;
		this.gv_PhysicalTable.UseBatchMod = false;
		this.gridColumnEx102.Caption = "AbsEntry";
		this.gridColumnEx102.CurrencyDateField = null;
		this.gridColumnEx102.CurrencyField = null;
		this.gridColumnEx102.CurrencyFixed = null;
		this.gridColumnEx102.CurrencyFixedRate = 0.0;
		this.gridColumnEx102.CurrencyRateField = null;
		this.gridColumnEx102.FieldName = "AbsEntry";
		this.gridColumnEx102.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx102.Name = "gridColumnEx102";
		this.gridColumnEx102.PrimaryKey = false;
		this.gridColumnEx102.UnitField = null;
		this.gridColumnEx103.Caption = "Source Field Name";
		this.gridColumnEx103.ColumnEdit = this.rep_PhysicalTableSourceField;
		this.gridColumnEx103.CurrencyDateField = null;
		this.gridColumnEx103.CurrencyField = null;
		this.gridColumnEx103.CurrencyFixed = null;
		this.gridColumnEx103.CurrencyFixedRate = 0.0;
		this.gridColumnEx103.CurrencyRateField = null;
		this.gridColumnEx103.FieldName = "SourceField";
		this.gridColumnEx103.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx103.Name = "gridColumnEx103";
		this.gridColumnEx103.PrimaryKey = false;
		this.gridColumnEx103.UnitField = null;
		this.gridColumnEx103.Visible = true;
		this.gridColumnEx103.VisibleIndex = 1;
		this.gridColumnEx103.Width = 121;
		this.rep_PhysicalTableSourceField.AutoHeight = false;
		this.rep_PhysicalTableSourceField.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_PhysicalTableSourceField.Name = "rep_PhysicalTableSourceField";
		this.rep_PhysicalTableSourceField.NullText = "";
		this.gridColumnEx104.Caption = "TargetItem";
		this.gridColumnEx104.CurrencyDateField = null;
		this.gridColumnEx104.CurrencyField = null;
		this.gridColumnEx104.CurrencyFixed = null;
		this.gridColumnEx104.CurrencyFixedRate = 0.0;
		this.gridColumnEx104.CurrencyRateField = null;
		this.gridColumnEx104.FieldName = "TargetItem";
		this.gridColumnEx104.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx104.Name = "gridColumnEx104";
		this.gridColumnEx104.PrimaryKey = false;
		this.gridColumnEx104.UnitField = null;
		this.gridColumnEx105.Caption = "Target Field Name";
		this.gridColumnEx105.ColumnEdit = this.rep_PhysicalTableTargetField;
		this.gridColumnEx105.CurrencyDateField = null;
		this.gridColumnEx105.CurrencyField = null;
		this.gridColumnEx105.CurrencyFixed = null;
		this.gridColumnEx105.CurrencyFixedRate = 0.0;
		this.gridColumnEx105.CurrencyRateField = null;
		this.gridColumnEx105.FieldName = "TargetField";
		this.gridColumnEx105.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx105.Name = "gridColumnEx105";
		this.gridColumnEx105.OptionsColumn.AllowEdit = false;
		this.gridColumnEx105.PrimaryKey = false;
		this.gridColumnEx105.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gridColumnEx105.UnitField = null;
		this.gridColumnEx105.Visible = true;
		this.gridColumnEx105.VisibleIndex = 0;
		this.gridColumnEx105.Width = 114;
		this.rep_PhysicalTableTargetField.AutoHeight = false;
		this.rep_PhysicalTableTargetField.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_PhysicalTableTargetField.Name = "rep_PhysicalTableTargetField";
		this.rep_PhysicalTableTargetField.NullText = "";
		this.gridColumnEx106.Caption = "Default Value";
		this.gridColumnEx106.CurrencyDateField = null;
		this.gridColumnEx106.CurrencyField = null;
		this.gridColumnEx106.CurrencyFixed = null;
		this.gridColumnEx106.CurrencyFixedRate = 0.0;
		this.gridColumnEx106.CurrencyRateField = null;
		this.gridColumnEx106.FieldName = "DfltValue";
		this.gridColumnEx106.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx106.Name = "gridColumnEx106";
		this.gridColumnEx106.PrimaryKey = false;
		this.gridColumnEx106.UnitField = null;
		this.gridColumnEx106.Visible = true;
		this.gridColumnEx106.VisibleIndex = 2;
		this.gridColumnEx106.Width = 175;
		this.gridColumnEx107.Caption = "Summary Type";
		this.gridColumnEx107.ColumnEdit = this.repositoryItemLookUpEdit4;
		this.gridColumnEx107.CurrencyDateField = null;
		this.gridColumnEx107.CurrencyField = null;
		this.gridColumnEx107.CurrencyFixed = null;
		this.gridColumnEx107.CurrencyFixedRate = 0.0;
		this.gridColumnEx107.CurrencyRateField = null;
		this.gridColumnEx107.FieldName = "SumType";
		this.gridColumnEx107.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx107.Name = "gridColumnEx107";
		this.gridColumnEx107.PrimaryKey = false;
		this.gridColumnEx107.UnitField = null;
		this.gridColumnEx107.Visible = true;
		this.gridColumnEx107.VisibleIndex = 3;
		this.gridColumnEx107.Width = 100;
		this.repositoryItemLookUpEdit4.AutoHeight = false;
		this.repositoryItemLookUpEdit4.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemLookUpEdit4.Name = "repositoryItemLookUpEdit4";
		this.repositoryItemLookUpEdit4.NullText = "";
		this.txt_PhysicalTableName.EditControl = null;
		this.txt_PhysicalTableName.FieldName = null;
		this.txt_PhysicalTableName.IsSearchControl = false;
		this.txt_PhysicalTableName.Location = new System.Drawing.Point(127, 15);
		this.txt_PhysicalTableName.Name = "txt_PhysicalTableName";
		this.txt_PhysicalTableName.SettingInVisible = false;
		this.txt_PhysicalTableName.SettingReadonly = false;
		this.txt_PhysicalTableName.Size = new System.Drawing.Size(152, 20);
		this.txt_PhysicalTableName.TabIndex = 1;
		this.txt_PhysicalTableName.Leave += new System.EventHandler(this.txt_PhysicalTableName_Leave);
		this.labelLine12.EditControl = null;
		this.labelLine12.EditValue = "Table name";
		this.labelLine12.FieldName = null;
		this.labelLine12.HideWhenEmpty = false;
		this.labelLine12.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine12.Location = new System.Drawing.Point(11, 18);
		this.labelLine12.Name = "labelLine12";
		this.labelLine12.ReadOnly = false;
		this.labelLine12.SettingInVisible = false;
		this.labelLine12.SettingReadonly = false;
		this.labelLine12.ShowUnderLine = true;
		this.labelLine12.Size = new System.Drawing.Size(120, 20);
		this.labelLine12.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine12.TabIndex = 0;
		this.labelLine12.Utext = "Table name";
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_OK.Appearance.Options.UseBackColor = true;
		this.btn_OK.EditControl = null;
		this.btn_OK.EditValue = "OK";
		this.btn_OK.FieldName = null;
		this.btn_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_OK.ImageOptions.Image");
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(2, 664);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.ReadOnly = false;
		this.btn_OK.SettingInVisible = false;
		this.btn_OK.SettingReadonly = false;
		this.btn_OK.Size = new System.Drawing.Size(75, 23);
		this.btn_OK.TabIndex = 1;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Cancel.Appearance.Options.UseBackColor = true;
		this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(83, 664);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 2;
		this.btn_Cancel.Text = "Cancel";
		this.drop_ExportSetting.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.drop_ExportSetting.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("drop_ExportSetting.ImageOptions.Image");
		this.drop_ExportSetting.Location = new System.Drawing.Point(660, 664);
		this.drop_ExportSetting.Name = "drop_ExportSetting";
		this.drop_ExportSetting.Size = new System.Drawing.Size(135, 23);
		this.drop_ExportSetting.TabIndex = 3;
		this.drop_ExportSetting.Text = "Export Setting";
		this.drop_ExportSetting.Click += new System.EventHandler(this.drop_ExportSetting_Click);
		this.btn_Import.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Import.EditControl = null;
		this.btn_Import.EditValue = "Import Setting";
		this.btn_Import.FieldName = null;
		this.btn_Import.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Import.ImageOptions.Image");
		this.btn_Import.IsSearchControl = false;
		this.btn_Import.Location = new System.Drawing.Point(801, 664);
		this.btn_Import.Name = "btn_Import";
		this.btn_Import.ReadOnly = false;
		this.btn_Import.SettingInVisible = false;
		this.btn_Import.SettingReadonly = false;
		this.btn_Import.Size = new System.Drawing.Size(135, 23);
		this.btn_Import.TabIndex = 4;
		this.btn_Import.Text = "Import Setting";
		this.btn_Import.Click += new System.EventHandler(this.btn_Import_Click);
		this.snapDockManager1.Form = this;
		this.snapDockManager1.SnapControl = this.snapControl1;
		this.snapDockManager1.TopZIndexControls.AddRange(new string[10] { "DevExpress.XtraBars.BarDockControl", "DevExpress.XtraBars.StandaloneBarDockControl", "System.Windows.Forms.StatusBar", "System.Windows.Forms.MenuStrip", "System.Windows.Forms.StatusStrip", "DevExpress.XtraBars.Ribbon.RibbonStatusBar", "DevExpress.XtraBars.Ribbon.RibbonControl", "DevExpress.XtraBars.Navigation.OfficeNavigationBar", "DevExpress.XtraBars.Navigation.TileNavPane", "DevExpress.XtraBars.TabFormControl" });
		this.snapDocumentManager1.ClientControl = this.snapControl1;
		this.snapDocumentManager1.View = this.noDocumentsView1;
		this.snapDocumentManager1.ViewCollection.AddRange(new DevExpress.XtraBars.Docking2010.Views.BaseView[1] { this.noDocumentsView1 });
		this.openFileDialog1.DefaultExt = "rpt";
		this.openFileDialog1.FileName = "openFileDialog1";
		this.openFileDialog1.Filter = "Rpt Files (*.rpt)|*.rpt|All files (*.*)|*.*";
		this.openFileDialog1.Multiselect = true;
		this.openFileDialog1.Title = "Choose files";
		this.openFileDialog2.DefaultExt = "rpt";
		this.openFileDialog2.FileName = "openFileDialog1";
		this.openFileDialog2.Filter = "Rpt Files (*.rpt)|*.rpt|All files (*.*)|*.*";
		this.openFileDialog2.Title = "Choose files";
		this.btn_PublishService.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_PublishService.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_PublishService.ImageOptions.Image");
		this.btn_PublishService.Location = new System.Drawing.Point(530, 664);
		this.btn_PublishService.Name = "btn_PublishService";
		this.btn_PublishService.Size = new System.Drawing.Size(124, 23);
		this.btn_PublishService.TabIndex = 5;
		this.btn_PublishService.Text = "Publish Service";
		this.btn_PublishService.Click += new System.EventHandler(this.btn_PublishService_Click);
		base.AcceptButton = this.btn_OK;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btn_Cancel;
		base.ClientSize = new System.Drawing.Size(942, 694);
		base.Controls.Add(this.btn_PublishService);
		base.Controls.Add(this.btn_Import);
		base.Controls.Add(this.drop_ExportSetting);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_OK);
		base.Controls.Add(this.splitContainerControl1);
		base.DisableContextAction = true;
		base.DisableLoadScript = true;
		base.FormId = "oContextAction";
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_ContextAction.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_ContextAction";
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Context Action";
		base.OnLoadAsync += new Apzon.Controls.Form.LoadAsyncEventHandler(this.frm_ContextMenu_Load);
		((System.ComponentModel.ISupportInitialize)this.rep_AttParamDataType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_AttParamSourceField).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Filter).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Filter).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Caption).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_SourceType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_DataRange).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ActionType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Method).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_MsgSuccess).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_MsgType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_EditorType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Invisible).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Icon).EndInit();
		((System.ComponentModel.ISupportInitialize)this.imageCollection1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rIgnoreError).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.xtraTab_Main).EndInit();
		this.xtraTab_Main.ResumeLayout(false);
		this.tab_SourceData.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txt_TemporaryTableName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.look_ParameterType.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.memo_DataString.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bte_FilterData.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Parameters).EndInit();
		this.contextMenuStrip2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Parameters).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_DataType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_SourceFieldData).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bte_DataFrom.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.look_DataFrom.Properties).EndInit();
		this.tab_Report.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.chk_JoinValue.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Report).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Report).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ReportDataType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_SourceField).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.memo_RptString.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bte_RptString.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.look_ReportFrom.Properties).EndInit();
		this.tab_Documents.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.look_DocEntry.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_ShowDocForm.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.look_ObjectType.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tabControl_MappingDocument).EndInit();
		this.tabControl_MappingDocument.ResumeLayout(false);
		this.xtraTabPage5.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Header).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Header).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_SourceFieldHeader).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_TargetField).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_SumTypeHeader).EndInit();
		this.xtraTabPage6.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Items).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Items).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_SourceFieldItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_TargetFieldLine).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_SumTypeLine).EndInit();
		this.xtraTabPage1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Batch).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Batch).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_SourceFieldBatch).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_TargetFieldBatch).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_SumTypeBatch).EndInit();
		this.tab_Udo.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.chk_ShowUdoForm.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.look_UdoCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tabControl_Udo).EndInit();
		this.tab_Dashboard.ResumeLayout(false);
		this.tab_ProductionOrder.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.look_ProductionEntry.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tabControl_MappingProduction).EndInit();
		this.tabControl_MappingProduction.ResumeLayout(false);
		this.xtraTabPage2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_ProductionHeader).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_ProductionHeader).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ProductionHeaderSourceField).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ProductionTargetHeader).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ProductionHeaderSumType).EndInit();
		this.xtraTabPage3.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_ProductionItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_ProductionItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ProductionLineSourceField).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ProductionOrderTargetLine).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ProductionLineSumType).EndInit();
		this.xtraTabPage4.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_ProductionBatch).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_ProductionBatch).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ProductionBatchSourceField).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ProdoctionTargetBatch).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ProductionBatchSumType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_ShowProductionForm.Properties).EndInit();
		this.tab_Email.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txt_EmailGroupByFields.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.look_EmailGroupBy.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl4).EndInit();
		this.xtraTabControl4.ResumeLayout(false);
		this.xtraTabPage8.ResumeLayout(false);
		this.xtraTabPage9.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl2).EndInit();
		this.splitContainerControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_AttachFiles).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_AttachFiles).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Path).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_AttType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_RptParam).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_RptParam).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Subject.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_AddressBcc.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_AddressCc.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_AddressTo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_AddressFrom.Properties).EndInit();
		this.tab_Forecast.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.xtraTabControlEx1).EndInit();
		this.xtraTabControlEx1.ResumeLayout(false);
		this.xtraTabPageEx1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_ForecastHeader).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_ForecastHeader).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ForecastSourceField).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ForecastTargetField).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEdit1).EndInit();
		this.xtraTabPageEx2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_ForecastItems).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_ForecastItems).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ForecastItemSourceField).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ForecastItemTargetField).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_ForecastShowWindow.Properties).EndInit();
		this.tab_DoQuery.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txt_TempTableNameDoQuery.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_UseTempTable.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_DoQuery).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_DoQuery).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_QueryDataType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_QuerySourceField).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.memo_QueryString.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_QueryString.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.look_QueryFrom.Properties).EndInit();
		this.tab_PrintLayout.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.look_PrintAction.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.look_RptUseSourceFrom.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_PrintLayout).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_PrintLayout).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_PrintLayoutDataType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_PrintLayoutSourceField).EndInit();
		((System.ComponentModel.ISupportInitialize)this.look_PrintLayoutFileType.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bte_PrintLayoutPath.Properties).EndInit();
		this.tab_ApprovalDecision.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Approval).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Approval).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ApprovalSourceField).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ApprovalTargetField).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemLookUpEdit3).EndInit();
		this.tab_DIYScript.ResumeLayout(false);
		this.tab_HttpRequest.ResumeLayout(false);
		this.tab_PhysicalTable.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_PhysicalTable).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_PhysicalTable).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_PhysicalTableSourceField).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_PhysicalTableTargetField).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemLookUpEdit4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_PhysicalTableName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.snapDockManager1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.snapDocumentManager1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.noDocumentsView1).EndInit();
		base.ResumeLayout(false);
	}
}
