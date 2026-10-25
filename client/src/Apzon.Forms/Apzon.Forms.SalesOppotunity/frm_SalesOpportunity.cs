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
using Apzon.Commons.Constants;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Forms.Mrps;
using Apzon.Forms.PM;
using Apzon.Forms.Properties;
using Apzon.Forms.Purchase;
using Apzon.Forms.Sales;
using Apzon.Forms.SalesOppotunity.SetUp.Opportunity;
using Apzon.Forms.UserControls;
using Apzon.Global;
using Apzon.HttpClientHelper;
using Commons.Helper;
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
using DevExpress.XtraTab;

namespace Apzon.Forms.SalesOppotunity;

public class frm_SalesOpportunity : XtraFormEx
{
	public List<Formular> Formulars = new List<Formular>();

	public List<Formular> OppFormulars = new List<Formular>();

	public Dictionary<string, GridViewEx> TblFormular = new Dictionary<string, GridViewEx>();

	private bool _isLoadingChildData;

	private bool _focusedObjectChanged;

	private bool _focusedRowChanged;

	private bool _isLoading;

	private bool _isDayChange;

	private bool _isSettingColCardCode;

	private bool _isSettingInterestPrimary;

	private IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private List<string> _docTypes = new List<string> { "-1", "23", "17", "15", "13" };

	private DataTable dtDoc = new DataTable();

	private DataTable _dtSta = new DataTable();

	private DataTable _dtBus = new DataTable();

	private DataTable _dtCom = new DataTable();

	private DataTable _dtRea = new DataTable();

	private DataTable _dtIte = new DataTable();

	private DataTable _dtLinkedAct = new DataTable();

	private DataTable _dtListComBoStage = new DataTable();

	private int _newOpprId;

	private int _opprId;

	private int _staErrRow = -1;

	private string _staErrCol = "";

	private string _staErrorString = "";

	private string _loadDataBy = "";

	private FtpHelper _ftpHelper = new FtpHelper();

	private List<string> _readOnlyColsInViewMode = new List<string> { "OpprType", "CardCode", "CardName", "TotalInvc", "OpprId", "Status", "CloseDate", "CloPrcnt" };

	private List<string> _readOnlyColsInAddMode = new List<string> { "OpprId", "Status", "CloseDate", "CloPrcnt", "TotalInvc" };

	public string FromDocEntry = "";

	private IContainer components;

	private LabelLine lb_OpportunityStatus;

	private LabelLine lb_ToDate;

	private LabelLine lb_NoOfTrans;

	private LabelLine lb_BusinessPartner;

	private LabelLine lb_MainSalesEmp;

	private LabelLine lb_Stage;

	private LabelLine lb_OpporStatus;

	private ButtonEditEx btEditBusPartner;

	private DateEditEx dateFrom;

	private DateEditEx dateTo;

	private SimpleButtonEx btSearch;

	private GridControlEx grdDoc;

	private SimpleButtonEx btAdd;

	private SimpleButtonEx btCancel;

	private SimpleButtonEx btRelatedAct;

	private SimpleButtonEx btRalatedDoc;

	private XtraTabControlEx xtraTabControl1;

	private XtraTabPageEx tab_Stage;

	private XtraTabPageEx tab_Competitor;

	private GridControlEx grdStage;

	private GridControlEx grdCompetitor;

	private XtraTabPageEx tab_Partner;

	private GridControlEx grdPartner;

	private XtraTabPageEx tab_Summary;

	private RadioGroupEx rdOppoStatus;

	private GridControlEx grdReason;

	private ComboBoxEditEx cbSummaryDocType;

	private LabelLine lb_DocumentNo;

	private LabelLine lb_DocumentType;

	private GridColumnEx gcReasonDesc;

	private LabelLine lb_FromDate;

	private GridColumnEx gcOpporType;

	private GridColumnEx gcOpporNo;

	private GridColumnEx gcOpporName;

	private GridColumnEx gcOpporBusCode;

	private GridColumnEx gcOpporBPName;

	private GridColumnEx gcOpporSalesEmp;

	private GridColumnEx gcOpporStatus;

	private GridColumnEx gcOpporStartDate;

	private GridColumnEx gcOpporClosingDate;

	private GridColumnEx gcOpporClosing;

	private GridColumnEx gcStageStartDate;

	private GridColumnEx gcStageClosingDate;

	private GridColumnEx gcStageSalesEmp;

	private GridColumnEx gcStage;

	private GridColumnEx gcStagePrct;

	private GridColumnEx gcStagePotentialAmount;

	private GridColumnEx gcStageWeightAmount;

	private GridColumnEx gcStageShowBPDocs;

	private GridColumnEx gcStageDocType;

	private GridColumnEx gcStageDocNo;

	private GridColumnEx gcStageOwner;

	private GridColumnEx gcStageActivity;

	private GridColumnEx gcCompetitorName;

	private GridColumnEx gcCompetitorThreatLevel;

	private GridColumnEx gcCompetitorRemark;

	private GridColumnEx gcCompetitorWon;

	private GridColumnEx gcPartnerName;

	private GridColumnEx gcPartnerRelationShip;

	private GridColumnEx gcPartnerRelatedBP;

	private GridColumnEx gcPartnerRemark;

	private GridColumnEx gcCompeOpportId;

	private GridColumnEx gcCompeLine;

	private GridColumnEx gcStageOpprId;

	private GridColumnEx gcStageLine;

	private GridColumnEx gcPartnerOpportId;

	private GridColumnEx gcPartnerLine;

	private GridColumnEx gcReasonOpportId;

	private GridColumnEx gcReasonLine;

	private RepositoryItemLookUpEditCombo rILSalesEmp;

	private RepositoryItemProgressBar rIPClosing;

	private RepositoryItemLookUpEditCombo rILStage;

	private RepositoryItemCheckEdit rICKWon;

	private RepositoryItemLookUpEditCombo rILStaSalesEmpl;

	private RepositoryItemLookUpEditCombo rILCompetitor;

	private RepositoryItemComboBox rICThreatLvl;

	private RepositoryItemButtonEdit rIBActivities;

	private RepositoryItemButtonEdit rIBBussPartner;

	private RepositoryItemLookUpEditCombo rILPartner;

	private RepositoryItemLookUpEditCombo rILPartnerRela;

	private RepositoryItemLookUpEditCombo rILReason;

	private XtraTabPageEx tab_ItemDetail;

	private GridControlEx grdItemDetail;

	private GridColumnEx gcItemCode;

	private RepositoryItemButtonEdit rIBItemCode;

	private GridColumnEx gcItemDesc;

	private GridColumnEx gcItemQty;

	private GridColumnEx gcIPercent;

	private GridColumnEx gcItemPrice;

	private GridColumnEx gcItemPotentAmount;

	private GridColumnEx gcItemDeliveryDate;

	private GridColumnEx gridColumn13;

	private GridColumnEx gridColumn14;

	private GridColumnEx gcItemFrgnName;

	private ContextMenuStrip ctm_Stage;

	private ToolStripMenuItem toolStripMenuItemAddRow;

	private ToolStripMenuItem deleteRowToolStripMenuItem;

	private RepositoryItemButtonEdit rIBRelaPartner;

	private XtraTabPageEx tab_Potential;

	private LabelLine lb_GrossProfit;

	private LabelLine lb_GrossProfitTotal;

	private LabelLine lb_PotentialAmount;

	private LabelLine lb_PredictedClosingDate;

	private LabelLine lb_PredictedClosingIn;

	private ComboBoxEditEx cbClosingType;

	private TextEditEx txtPredictCloseIn;

	private CalcEditEx calcPotentialAmount;

	private DateEditEx datePredCloDate;

	private CalcEditEx calcWeightAmount;

	private LabelLine lb_WeightedAmount;

	private ContextMenuStrip ctm_Items;

	private ToolStripMenuItem deleteRowsToolStripMenuItem;

	private ContextMenuStrip ctm_Doc;

	private ToolStripMenuItem addNewToolStripMenuItem;

	private ToolStripMenuItem deleteDocumentToolStripMenuItem;

	private RepositoryItemCheckEdit rICShowBPDoc;

	private RepositoryItemDateEdit repositoryItemDateEdit1;

	private RepositoryItemDateEdit repositoryItemDateEdit2;

	private RepositoryItemButtonEdit rIBStageDocNo;

	private ButtonEditEx txtDocNo;

	private GridViewEx gv_Doc;

	private GridViewEx gv_Stage;

	private GridViewEx gv_Competitor;

	private GridViewEx gv_Partner;

	private GridViewEx gv_Reason;

	private GridViewEx gv_ItemDetail;

	private ToolStripMenuItem createSalesQuotationToolStripMenuItem;

	private ToolStripMenuItem viewForeCastToolStripMenuItem;

	private RepositoryItemDateEdit repositoryItemDateEdit3;

	private ToolStripMenuItem viewChangeLogToolStripMenuItem;

	private GridColumnEx gcItemAttFile;

	private RepositoryItemButtonEdit rep_bte_ItemAttFile;

	private GridColumnEx gcItemAttFullPath;

	private RepositoryItemDateEdit repositoryItemDateEdit4;

	private RepositoryItemLookUpEditCombo rep_stage_DocType;

	private GridColumnEx gc_Stage_DocType_Name;

	private ToolStripMenuItem addRowToolStripMenuItem;

	private RepositoryItemLookUpEditCombo rep_Stage_Owner;

	private LookUpEditCombo cbStage;

	private LookUpEditCombo cbOpportStatus;

	private LookUpEditCombo cbMainSalesEmp;

	private SplitContainerControl splitContainerControl1;

	private ContextMenuStrip ctm_Description;

	private ToolStripMenuItem removeRowToolStripMenuItem;

	private RepositoryItemLookUpEditCombo rep_luk_OPR3_ThreatLevl;

	private RepositoryItemLookUpEditCombo rep_luk_OOPR_Status;

	private RepositoryItemLookUpEditCombo rep_Luk_OOPR_OpprType;

	private XtraTabPageEx tab_General;

	private TextEditEx txt_ChnCrdName;

	private LabelLine labelLine1;

	private LabelLine labelLine2;

	private LabelLine labelLine3;

	private LabelLine labelLine7;

	private LabelLine labelLine8;

	private LabelLine labelLine9;

	private Label label1;

	private MemoEditEx memo_Memo;

	private ButtonEditCFL bteCFL_PrjCode;

	private ButtonEditCFL bteCFL_ChnCrdCode;

	private LookUpEditCombo lukDN_ChnCrdCon;

	private LookUpEditDefineNew lukDN_Industry;

	private LookUpEditDefineNew lukDN_Source;

	private XtraTabPageEx tab_Attachment;

	private ucAttachment ucAttachment1;

	private LookUpEditCombo luk_LevelOfInterest;

	private LabelLine labelLine4;

	private LabelLine labelLine5;

	private GridControlEx grd_InterestRange;

	private GridViewEx gv_InterestRange;

	private GridColumnEx gridColumnEx1;

	private GridColumnEx gridColumnEx2;

	private GridColumnEx gridColumnEx3;

	private RepositoryItemLookUpEditCombo rep_luk_IntrestRange_IntId;

	private GridColumnEx gridColumnEx4;

	private RepositoryItemCheckEdit rep_chk_IntrestRange_Prmry;

	private ContextMenuStrip ctm_InterestRange;

	private ToolStripMenuItem removeRowToolStripMenuItem1;

	private ToolStripMenuItem addRowToolStripMenuItem1;

	private CalcEditEx calcGrossProfitTotal;

	private CalcEditEx calcGrossProfit;

	private TextEditEx txtNoOfTrans;

	private GridColumnEx gridColumnEx5;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx1;

	private GridColumnEx gridColumnEx6;

	private RepositoryItemMemoExEdit repositoryItemMemoExEdit1;

	private GridColumnEx gcOwner;

	private RepositoryItemButtonEditCFL rep_Owner;

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
		case Enums.FormViewModeType.Add:
			btAdd.Text = Apzon.Commons.Properties.Resources.ButtonAdd;
			break;
		case Enums.FormViewModeType.Update:
			btAdd.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btAdd.Text = Apzon.Commons.Properties.Resources.ButtonOK;
			break;
		case Enums.FormViewModeType.Find:
			btAdd.Text = Apzon.Commons.Properties.Resources.ButtonFind;
			break;
		}
	}

	public async Task REFRESH()
	{
		_ = 1;
		try
		{
			if (!_isLoading)
			{
				FormMode = Enums.FormViewModeType.Add;
				rdOppoStatus.SelectedIndex = 0;
				rdOppoStatus.Properties.Items[1].Enabled = true;
				rdOppoStatus.Properties.Items[2].Enabled = true;
				AddNewRowDoc();
				AddNewRowStage(clear: true);
				AddNewRowPartner(clear: true);
				AddNewRowCompetition(clear: true);
				AddNewRowInterestRange(clear: true);
				AddNewRowReason(clear: true);
				if (GlobalData.LoginSap)
				{
					await AddNewRowItem(clear: true);
				}
				rdOppoStatus.SelectedIndex = 0;
				cbSummaryDocType.SelectedIndex = -1;
				datePredCloDate.EditValue = "";
				cbClosingType.SelectedIndex = 2;
				calcGrossProfit.EditValue = 100;
				calcGrossProfitTotal.EditValue = 0;
				calcWeightAmount.EditValue = 0;
				calcPotentialAmount.EditValue = 0;
				txtDocNo.EditValue = 0;
				txtPredictCloseIn.Text = "0";
				ShowHideStageCombo("R");
				await LoadLinkedAct(-1);
				ucAttachment1.grd_File.DataSource = WinFuntions.GetTableStructure("ATC1");
				ucAttachment1.grd_File.RefreshDataSource();
				bteCFL_ChnCrdCode.EditValue = "";
				bteCFL_PrjCode.EditValue = "";
				txt_ChnCrdName.Text = "";
				lukDN_Source.EditValue = 0;
				lukDN_ChnCrdCon.EditValue = 0;
				lukDN_Industry.EditValue = 0;
				memo_Memo.Text = "";
				LoadComboInterest();
				SetColumnsReadOnly();
				gv_Doc.Focus();
				gv_Doc.FocusedColumn = gv_Doc.Columns["CardCode"];
				gv_Doc.FocusedRowHandle = 0;
				gv_Doc.ShowEditor();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task MOVE_FIRST()
	{
		await GetRecord("F");
	}

	public async Task MOVE_NEXT()
	{
		await GetRecord("N");
	}

	public async Task MOVE_LAST()
	{
		await GetRecord("L");
	}

	public async Task MOVE_PREVIOUS()
	{
		await GetRecord("P");
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			if (_loadDataBy == "N")
			{
				await GetRecord("", refresh: true);
			}
			else if (_loadDataBy == "S")
			{
				bool flag = true;
				if (FormMode == Enums.FormViewModeType.Update && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
				{
					flag = false;
				}
				if (flag)
				{
					btSearch_Click(null, null);
				}
			}
			SetColumnsReadOnly();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public frm_SalesOpportunity()
	{
		InitializeComponent();
		InitEventControls();
		LoadReasonGridNumber();
		OnLoadAsync += frm_SalesOpportunity_Load;
		gv_Doc.CustomRowCellEdit += Gv_Doc_CustomRowCellEdit;
		gv_Competitor.CustomRowCellEdit += Gv_Doc_CustomRowCellEdit;
		gv_ItemDetail.CustomRowCellEdit += Gv_Doc_CustomRowCellEdit;
		gv_Stage.CustomRowCellEdit += Gv_Doc_CustomRowCellEdit;
	}

	public frm_SalesOpportunity(string docEntry)
	{
		InitializeComponent();
		FromDocEntry = docEntry;
		InitEventControls();
		LoadReasonGridNumber();
		OnLoadAsync += frm_SalesOpportunity_Load;
		gv_Doc.CustomRowCellEdit += Gv_Doc_CustomRowCellEdit;
		gv_Competitor.CustomRowCellEdit += Gv_Doc_CustomRowCellEdit;
		gv_ItemDetail.CustomRowCellEdit += Gv_Doc_CustomRowCellEdit;
		gv_Stage.CustomRowCellEdit += Gv_Doc_CustomRowCellEdit;
	}

	private void Gv_Doc_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
	{
		try
		{
			if (e.Column.ColumnType == typeof(DateTime) && typeof(RepositoryItemDateEditEx) != e.RepositoryItem.GetType())
			{
				e.RepositoryItem = new RepositoryItemDateEditEx();
			}
			if (e.Column is GridColumnEx && ((GridColumnEx)e.Column).DisplayFormatType == Enums.DisplayFormatType.Time && e.Column.ColumnType == typeof(int) && typeof(RepositoryItemTimeEditEx) != e.RepositoryItem.GetType())
			{
				e.RepositoryItem = new RepositoryItemTimeEditEx();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private async Task frm_SalesOpportunity_Load(object sender, EventArgs e)
	{
		_ = 4;
		try
		{
			dateFrom.DateTime = DateTime.Today.AddDays(-Configuration.SearchDate);
			dateTo.DateTime = DateTime.Today;
			OppFormulars = await WinFuntions.GetFormulas(97);
			LoadTableFormular();
			Task task = WinFuntions.LoadUdfColumns(gv_Doc, "OOPR", TblFormular);
			Task task2 = WinFuntions.LoadUdfColumns(gv_Stage, "OPR1", TblFormular);
			Task task3 = WinFuntions.LoadUdfColumns(gv_Partner, "OPR2", TblFormular);
			await Task.WhenAll(task, task2, task3);
			gv_Doc.InitFormular(OppFormulars, TblFormular);
			gv_Stage.InitFormular(OppFormulars, TblFormular);
			Task task4 = WinFuntions.LoadUdfColumns(gv_ItemDetail, "@OPR62", TblFormular);
			Task<List<Formular>> taskFormulars = WinFuntions.GetFormulas(-1);
			await task4;
			Formulars = await taskFormulars;
			gv_ItemDetail.InitFormular(Formulars, TblFormular);
			rep_Owner.RefreshDataSource();
			DataTable searchDataTable = GetSearchDataTable();
			DataRow dataRow = searchDataTable.NewRow();
			if (Function.ParseInt(FromDocEntry) != 0)
			{
				dateFrom.EditValue = DBNull.Value;
				dateTo.EditValue = DBNull.Value;
				txtNoOfTrans.EditValue = Function.ParseInt(FromDocEntry);
			}
			DateTime? dateTime = Function.ParseDateTime(dateFrom.EditValue);
			DateTime? dateTime2 = Function.ParseDateTime(dateTo.EditValue);
			dataRow["FromDate"] = ((!dateTime.HasValue || dateTime.Value.Date < new DateTime(1950, 1, 1)) ? "" : dateTime.Value.ToString("yyyyMMdd"));
			dataRow["ToDate"] = ((!dateTime2.HasValue || dateTime2.Value.Date < new DateTime(1950, 1, 1)) ? "" : dateTime2.Value.ToString("yyyyMMdd"));
			dataRow["NoTrans"] = Function.ParseInt(txtNoOfTrans.EditValue);
			dataRow["BusPartner"] = Function.ToString(btEditBusPartner.EditValue);
			dataRow["Status"] = Function.ToString(cbOpportStatus.EditValue);
			dataRow["Stage"] = Function.ParseInt(cbStage.EditValue);
			dataRow["SaleEmp"] = Function.ParseInt(cbMainSalesEmp.EditValue);
			dataRow["UserName"] = GlobalData.UserCodeLogin;
			searchDataTable.Rows.Add(dataRow);
			await Search(searchDataTable);
			rep_Luk_OOPR_OpprType.SetDataSource(new string[2, 2]
			{
				{ "R", "Sales" },
				{ "P", "Purchasing" }
			}, isStringCode: true);
			rep_luk_OPR3_ThreatLevl.SetDataSource(new string[3, 2]
			{
				{ "1", "Low" },
				{ "2", "Medium" },
				{ "3", "High" }
			}, isStringCode: false, isShort: true);
			rep_luk_OOPR_Status.SetDataSource(new string[3, 2]
			{
				{ "O", "Open" },
				{ "L", "Lost" },
				{ "W", "Won" }
			}, isStringCode: true);
			if (!GlobalData.LoginSap)
			{
				tab_ItemDetail.PageVisible = false;
			}
			SetDevControls();
			GridColumn gridColumn = gv_Doc.Columns["U_RelatedTyp"];
			if (gridColumn != null)
			{
				RepositoryItemLookUpEditUDF repositoryItemLookUpEditUDF = new RepositoryItemLookUpEditUDF();
				repositoryItemLookUpEditUDF.Name = "luk_typeUDF";
				repositoryItemLookUpEditUDF.ShowFooter = false;
				repositoryItemLookUpEditUDF.ShowHeader = false;
				gridColumn.ColumnEdit = repositoryItemLookUpEditUDF;
			}
			GridColumn gridColumn2 = gv_Doc.Columns["U_RelatedEnt"];
			if (gridColumn2 != null)
			{
				RegisterChooseFormListUDF registerChooseFormListUDF = new RegisterChooseFormListUDF();
				registerChooseFormListUDF.fileName = "U_RelatedEnt";
				registerChooseFormListUDF.GridView = gv_Doc;
				gridColumn2.ColumnEdit = registerChooseFormListUDF;
			}
			ucAttachment1.ButtonBrowse_Click += Controls_EditValueChanged;
			ucAttachment1.ButtonDelete_Click += Controls_EditValueChanged;
			ucAttachment1.ButtonDisplay_Click += Controls_EditValueChanged;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void Controls_EditValueChanged(object sender, EventArgs e)
	{
		ChangeButtonOkText(sender);
	}

	private void gvReason_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
	{
		try
		{
			if (e.IsGetData)
			{
				e.Value = e.ListSourceRowIndex + 1;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		try
		{
			if (msg.Msg == 256 || msg.Msg == 260)
			{
				if (keyData == (Keys.A | Keys.Control) && !_isLoading)
				{
					addNewToolStripMenuItem_Click(null, null);
				}
				if (keyData == (Keys.F | Keys.Control))
				{
					btSearch_Click(null, null);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return base.ProcessCmdKey(ref msg, keyData);
	}

	private void gvStage_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			int dataRowCount = gv_Stage.DataRowCount;
			if ((gv_Stage.FocusedRowHandle != dataRowCount - 1 && dataRowCount > 1 && gv_Stage.FocusedColumn.FieldName != "Linked") || string.IsNullOrEmpty(Function.ToString(gv_Doc.GetFocusedRowCellValue("CardCode"))))
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

	private void gvItemDetail_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (!(gv_ItemDetail.FocusedColumn.FieldName != "PotentAmount"))
			{
				decimal num = ((gv_ItemDetail.GetFocusedRowCellValue("Quantity") == DBNull.Value || gv_ItemDetail.GetFocusedRowCellValue("Quantity") == null) ? 0m : Function.ParseDecimal(gv_ItemDetail.GetFocusedRowCellValue("Quantity")));
				decimal num2 = ((gv_ItemDetail.GetFocusedRowCellValue("Quantity") == DBNull.Value || gv_ItemDetail.GetFocusedRowCellValue("Quantity") == null) ? 0m : Function.ParseDecimal(gv_ItemDetail.GetFocusedRowCellValue("Quantity")));
				if (num == 0m || num2 == 0m)
				{
					e.Cancel = true;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvStage_RowStyle(object sender, RowStyleEventArgs e)
	{
		try
		{
			int rowCount = gv_Stage.RowCount;
			if (e.RowHandle < rowCount - 1)
			{
				e.Appearance.BackColor = Color.LightGray;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void contextMenuStripItems_Opening(object sender, CancelEventArgs e)
	{
		try
		{
			int num = Function.ParseInt(gv_Doc.GetRowCellValue(gv_Doc.FocusedRowHandle, "OpprId"));
			createSalesQuotationToolStripMenuItem.Enabled = num != _newOpprId;
			viewForeCastToolStripMenuItem.Enabled = num != _newOpprId;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Doc_InvalidValueException(object sender, InvalidValueExceptionEventArgs e)
	{
		e.ExceptionMode = ExceptionMode.NoAction;
	}

	private void gv_Doc_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
	{
		try
		{
			if (!(gv_Doc.FocusedColumn.FieldName == "CardCode") || _isSettingColCardCode)
			{
				return;
			}
			string text = Function.ToString(e.Value).Trim();
			if (text != "")
			{
				DataTable dataTable = _httpClient.OpportunityClient.SearchBPData(text);
				if (dataTable.IsNotNull())
				{
					e.Value = Function.ToString(dataTable.Rows[0]["CardCode"]);
					LoadBpOnChange(dataTable.Rows[0], gv_Doc.FocusedRowHandle, sender);
				}
				else
				{
					e.Valid = false;
					e.ErrorText = "";
					ChooseFromListBp(sender, text);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_InterestRange_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (_isSettingInterestPrimary)
			{
				return;
			}
			ChangeButtonOkText(sender);
			if (e.Column.FieldName == "Prmry")
			{
				_isSettingInterestPrimary = true;
				for (int i = 0; i < gv_InterestRange.DataRowCount; i++)
				{
					if (i != e.RowHandle)
					{
						gv_InterestRange.SetRowCellValue(i, "Prmry", "N");
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		_isSettingInterestPrimary = false;
	}

	private void rep_luk_IntrestRange_IntId_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			gv_InterestRange.PostEditor();
			AddNewRowInterestRange();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void removeRowToolStripMenuItem1_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataTable = grd_InterestRange.DataSource as DataTable;
			gv_InterestRange.DeleteSelectedRows();
			dataTable?.AcceptChanges();
			grd_InterestRange.DataSource = dataTable;
			grd_InterestRange.RefreshDataSource();
			ChangeButtonOkText(sender);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void addRowToolStripMenuItem1_Click(object sender, EventArgs e)
	{
		try
		{
			if (!string.IsNullOrEmpty(Function.ToString(gv_Doc.GetFocusedRowCellValue("CardCode"))))
			{
				AddNewRowInterestRange();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Doc_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (string.IsNullOrEmpty(Function.ToString(gv_Doc.GetFocusedRowCellValue("CardCode"))) && gv_Doc.FocusedColumn.FieldName != "OpprType" && gv_Doc.FocusedColumn.FieldName != "CardCode")
			{
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void bteCFL_ChnCrdCode_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (Function.ToString(e.OldValue) != Function.ToString(e.NewValue))
			{
				ChangeButtonOkText(sender);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void GvChild_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (string.IsNullOrEmpty(Function.ToString(gv_Doc.GetFocusedRowCellValue("CardCode"))))
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

	private void gv_InterestRange_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (string.IsNullOrEmpty(Function.ToString(gv_Doc.GetFocusedRowCellValue("CardCode"))) || (gv_InterestRange.FocusedColumn.FieldName == "Prmry" && gv_InterestRange.GetFocusedRowCellValue("IntId") == DBNull.Value))
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

	private void Controls_Properties_Enter(object sender, EventArgs e)
	{
		try
		{
			if (string.IsNullOrEmpty(Function.ToString(gv_Doc.GetFocusedRowCellValue("CardCode"))))
			{
				if (sender is ucAttachment)
				{
					(sender as ucAttachment).EnableButtons(isEnabled: false);
				}
				else
				{
					if (!(sender is BaseEdit { BackColor: var backColor } baseEdit))
					{
						return;
					}
					baseEdit.ReadOnly = true;
					baseEdit.BackColor = backColor;
					if (!(baseEdit is ButtonEdit buttonEdit))
					{
						return;
					}
					for (int i = 0; i < buttonEdit.Properties.Buttons.Count; i++)
					{
						if (buttonEdit.Properties.Buttons[i].Kind == ButtonPredefines.Search)
						{
							buttonEdit.Properties.Buttons[i].Visible = false;
						}
					}
				}
			}
			else if (sender is ucAttachment)
			{
				(sender as ucAttachment).EnableButtons(isEnabled: true);
			}
			else
			{
				if (!(sender is BaseEdit baseEdit2))
				{
					return;
				}
				baseEdit2.ReadOnly = false;
				if (!(baseEdit2 is ButtonEdit buttonEdit2))
				{
					return;
				}
				for (int j = 0; j < buttonEdit2.Properties.Buttons.Count; j++)
				{
					if (buttonEdit2.Properties.Buttons[j].Kind == ButtonPredefines.Search)
					{
						buttonEdit2.Properties.Buttons[j].Visible = true;
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void datePredCloDate_Leave(object sender, CancelEventArgs e)
	{
		try
		{
			DateTime? dateTime = Function.ParseDateTime(gv_Doc.GetFocusedRowCellValue("OpenDate"));
			DateTime? dateTime2 = Function.ParseDateTime(datePredCloDate.EditValue);
			if (!dateTime.HasValue)
			{
				return;
			}
			_ = dateTime.Value;
			if (dateTime2.HasValue)
			{
				_ = dateTime2.Value;
				if (DateTime.Compare(dateTime2.Value, dateTime.Value) < 0)
				{
					WinCommons.Error(Enums.MessageCode.OnStagesTabStartDateMustLaterOrEqualStartDateInGeneral, "");
					e.Cancel = true;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btAdd_TextChanged(object sender, EventArgs e)
	{
		try
		{
			SetColumnsReadOnly();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Doc_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (e.RowHandle == gv_Doc.FocusedRowHandle)
			{
				e.Appearance.BackColor = StyleColor.FocusedRow;
			}
			else if (e.Column.OptionsColumn.ReadOnly)
			{
				e.Appearance.BackColor = StyleColor.GridDisable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Doc_ShownEditor(object sender, EventArgs e)
	{
		try
		{
			GridView gridView = sender as GridView;
			if (gridView.FocusedColumn.OptionsColumn.ReadOnly)
			{
				gridView.ActiveEditor.BackColor = StyleColor.FocusedRow;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void luk_LevelOfInterest_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (Function.ParseShort(e.NewValue) == -10)
			{
				frm_InterestLevel frm_InterestLevel2 = new frm_InterestLevel();
				frm_InterestLevel2.MdiParent = MdiParent;
				frm_InterestLevel2.FormClosing += (object s1, FormClosingEventArgs e2) =>
				{
					LoadComboInterest();
				};
				frm_InterestLevel2.Show();
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rep_luk_IntrestRange_IntId_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (Function.ParseShort(e.NewValue) == -10)
			{
				frm_Interest frm_Interest2 = new frm_Interest();
				frm_Interest2.MdiParent = MdiParent;
				frm_Interest2.FormClosing += (object s1, FormClosingEventArgs e2) =>
				{
					LoadComboInterest();
				};
				frm_Interest2.Show();
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rILStage_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (Function.ParseShort(e.NewValue) == -10)
			{
				frm_SalesOpportunityStage frm_SalesOpportunityStage2 = new frm_SalesOpportunityStage();
				frm_SalesOpportunityStage2.MdiParent = MdiParent;
				frm_SalesOpportunityStage2.FormClosing += async (object snder, FormClosingEventArgs e2) =>
				{
					await LoadComboStage();
				};
				frm_SalesOpportunityStage2.Show();
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvDoc_BeforeLeaveRow(object sender, RowAllowEventArgs e)
	{
		try
		{
			if (_isLoadingChildData || (_isLoading && FormMode != Enums.FormViewModeType.Add))
			{
				e.Allow = false;
				return;
			}
			object focusedRowCellValue = gv_Doc.GetFocusedRowCellValue("OpprId");
			if (gv_Doc.IsRowVisible(gv_Doc.FocusedRowHandle) != RowVisibleState.Visible)
			{
				return;
			}
			if (focusedRowCellValue == null && FormMode != Enums.FormViewModeType.Add)
			{
				e.Allow = false;
			}
			else if (FormMode == Enums.FormViewModeType.Update && _opprId == Function.ParseInt(focusedRowCellValue))
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
				{
					FormMode = Enums.FormViewModeType.View;
					btRefresh_Click();
				}
				else
				{
					e.Allow = false;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void gvDoc_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		_focusedObjectChanged = true;
		try
		{
			if (_focusedRowChanged)
			{
				_focusedObjectChanged = false;
				return;
			}
			object focusedRowCellValue = gv_Doc.GetFocusedRowCellValue("OpprId");
			_opprId = Function.ParseInt(focusedRowCellValue);
			_isLoadingChildData = true;
			await LoadOneDoc(_opprId);
			_isLoadingChildData = false;
			rdOppoStatus.Properties.Items[1].Enabled = true;
			rdOppoStatus.Properties.Items[2].Enabled = true;
			ShowHideStageCombo(Function.ToString(gv_Doc.GetFocusedRowCellValue("OpprType")));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		_focusedObjectChanged = false;
	}

	private async void gvDoc_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
	{
		_focusedRowChanged = true;
		try
		{
			if (_focusedObjectChanged)
			{
				_focusedRowChanged = false;
				return;
			}
			object focusedRowCellValue = gv_Doc.GetFocusedRowCellValue("OpprId");
			_opprId = Function.ParseInt(focusedRowCellValue);
			_isLoadingChildData = true;
			await LoadOneDoc(_opprId);
			_isLoadingChildData = false;
			rdOppoStatus.Properties.Items[1].Enabled = true;
			rdOppoStatus.Properties.Items[2].Enabled = true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		_focusedRowChanged = false;
	}

	private void rdOppoStatus_SelectedIndexChanged(object sender, EventArgs e)
	{
		try
		{
			ChangeButtonOkText(sender);
			RadioGroup radioGroup = sender as RadioGroup;
			string value = "Open";
			if (radioGroup != null)
			{
				switch (radioGroup.SelectedIndex)
				{
				case 0:
					value = "O";
					cbSummaryDocType.Enabled = true;
					txtDocNo.Enabled = true;
					SetGridColumnReadOnly(new GridView[4] { gv_ItemDetail, gv_Stage, gv_Partner, gv_Competitor }, readOnly: false);
					gv_Reason.OptionsBehavior.Editable = false;
					txtPredictCloseIn.Properties.ReadOnly = false;
					cbClosingType.Properties.ReadOnly = false;
					calcPotentialAmount.Properties.ReadOnly = false;
					calcWeightAmount.Properties.ReadOnly = false;
					calcGrossProfit.Properties.ReadOnly = false;
					calcGrossProfitTotal.Properties.ReadOnly = false;
					datePredCloDate.Properties.ReadOnly = false;
					cbSummaryDocType.ReadOnly = false;
					txtDocNo.ReadOnly = false;
					bteCFL_ChnCrdCode.ReadOnly = false;
					lukDN_ChnCrdCon.ReadOnly = false;
					bteCFL_PrjCode.ReadOnly = false;
					lukDN_Source.ReadOnly = false;
					lukDN_Industry.ReadOnly = false;
					memo_Memo.ReadOnly = false;
					gv_Doc.Columns["OpenDate"].OptionsColumn.ReadOnly = false;
					gv_Doc.Columns["OpenDate"].OptionsColumn.AllowEdit = true;
					gv_Doc.Columns["CloseDate"].OptionsColumn.ReadOnly = true;
					gv_Doc.Columns["CloseDate"].OptionsColumn.AllowEdit = false;
					if (!_isLoading)
					{
						DataRow dataRow = dtDoc.Rows.Find(_opprId);
						if (dataRow != null && FormMode == Enums.FormViewModeType.View)
						{
							dataRow["CloseDate"] = DBNull.Value;
							dtDoc.AcceptChanges();
						}
					}
					break;
				case 1:
					value = "W";
					cbSummaryDocType.Enabled = true;
					txtDocNo.Enabled = true;
					SetGridColumnReadOnly(new GridView[4] { gv_ItemDetail, gv_Stage, gv_Partner, gv_Competitor }, readOnly: true);
					gv_Reason.OptionsBehavior.Editable = true;
					txtPredictCloseIn.Properties.ReadOnly = true;
					cbClosingType.Properties.ReadOnly = true;
					calcPotentialAmount.Properties.ReadOnly = true;
					calcWeightAmount.Properties.ReadOnly = true;
					calcGrossProfit.Properties.ReadOnly = true;
					calcGrossProfitTotal.Properties.ReadOnly = true;
					datePredCloDate.Properties.ReadOnly = true;
					cbSummaryDocType.ReadOnly = false;
					txtDocNo.ReadOnly = false;
					bteCFL_ChnCrdCode.ReadOnly = true;
					lukDN_ChnCrdCon.ReadOnly = true;
					bteCFL_PrjCode.ReadOnly = true;
					lukDN_Source.ReadOnly = true;
					lukDN_Industry.ReadOnly = true;
					memo_Memo.ReadOnly = true;
					gv_Doc.Columns["OpenDate"].OptionsColumn.ReadOnly = true;
					gv_Doc.Columns["OpenDate"].OptionsColumn.AllowEdit = false;
					gv_Doc.Columns["CloseDate"].OptionsColumn.ReadOnly = false;
					gv_Doc.Columns["CloseDate"].OptionsColumn.AllowEdit = true;
					break;
				case 2:
					value = "L";
					grdReason.Enabled = true;
					cbSummaryDocType.Enabled = false;
					txtDocNo.Enabled = false;
					SetGridColumnReadOnly(new GridView[4] { gv_ItemDetail, gv_Stage, gv_Partner, gv_Competitor }, readOnly: true);
					gv_Reason.OptionsBehavior.Editable = true;
					txtPredictCloseIn.Properties.ReadOnly = true;
					cbClosingType.Properties.ReadOnly = true;
					calcPotentialAmount.Properties.ReadOnly = true;
					calcWeightAmount.Properties.ReadOnly = true;
					calcGrossProfit.Properties.ReadOnly = true;
					calcGrossProfitTotal.Properties.ReadOnly = true;
					datePredCloDate.Properties.ReadOnly = true;
					cbSummaryDocType.ReadOnly = true;
					txtDocNo.ReadOnly = true;
					bteCFL_ChnCrdCode.ReadOnly = true;
					lukDN_ChnCrdCon.ReadOnly = true;
					bteCFL_PrjCode.ReadOnly = true;
					lukDN_Source.ReadOnly = true;
					lukDN_Industry.ReadOnly = true;
					memo_Memo.ReadOnly = true;
					gv_Doc.Columns["OpenDate"].OptionsColumn.ReadOnly = true;
					gv_Doc.Columns["OpenDate"].OptionsColumn.AllowEdit = false;
					gv_Doc.Columns["CloseDate"].OptionsColumn.ReadOnly = false;
					gv_Doc.Columns["CloseDate"].OptionsColumn.AllowEdit = true;
					break;
				}
			}
			if (!_isLoading)
			{
				DataRow dataRow2 = dtDoc.Rows.Find(_opprId);
				if (dataRow2 != null)
				{
					dataRow2["Status"] = value;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void cbClosingType_SelectedIndexChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isDayChange)
			{
				_isDayChange = true;
				ChangeButtonOkText(sender);
				SetPredictDate();
				_isDayChange = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void cbSummaryDocType_SelectedIndexChanged(object sender, EventArgs e)
	{
		try
		{
			ChangeButtonOkText(sender);
			if (FormMode == Enums.FormViewModeType.Update)
			{
				txtDocNo.EditValue = 0;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rep_chk_IntrestRange_Prmry_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			gv_InterestRange.PostEditor();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txtPredictCloseIn_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isDayChange)
			{
				_isDayChange = true;
				ChangeButtonOkText(sender);
				SetPredictDate();
				_isDayChange = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void datePredCloDate_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (_isDayChange)
			{
				return;
			}
			ChangeButtonOkText(sender);
			_isDayChange = true;
			DateTime? dateTime = Function.ParseDateTime(gv_Doc.GetFocusedRowCellValue("OpenDate"));
			if (dateTime.HasValue)
			{
				int num = (int)Math.Floor((datePredCloDate.DateTime.Date - dateTime.Value.Date).TotalDays);
				cbClosingType.SelectedIndex = 2;
				if (num > 0)
				{
					txtPredictCloseIn.Text = num.ToString();
				}
				else
				{
					txtPredictCloseIn.Text = "";
				}
			}
			_isDayChange = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvDoc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			ChangeButtonOkText(sender);
			if (e.Column.FieldName.Equals("MaxSumLoc"))
			{
				SetStagePotentialAmount(Function.ParseDecimal(e.Value));
				SetStageWeightAmount(Function.ParseDecimal(e.Value));
			}
			if (e.Column.FieldName.Equals("Memo"))
			{
				memo_Memo.EditValue = e.Value;
			}
			if (e.Column.FieldName.Equals("CloseDate"))
			{
				DateTime dateTime = ((gv_Doc.GetFocusedRowCellValue("OpenDate") == null) ? DateTime.Today : Convert.ToDateTime(gv_Doc.GetFocusedRowCellValue("OpenDate")));
				if (e.Value != null && Convert.ToDateTime(e.Value) < dateTime)
				{
					WinCommons.Error(Enums.MessageCode.OpenDateCanNotAfterClosingDate, "");
					gv_Doc.FocusedColumn = gv_Doc.Columns["CloseDate"];
					gv_Doc.ShowEditor();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadBpOnChange(DataRow buss, int rowHandle, object sender)
	{
		try
		{
			_isSettingColCardCode = true;
			gv_Doc.SetRowCellValue(rowHandle, "CardCode", Function.ToString(buss["CardCode"]));
			_isSettingColCardCode = false;
			gv_Doc.SetRowCellValue(rowHandle, "CardName", Function.ToString(buss["CardName"]));
			gv_Doc.SetRowCellValue(rowHandle, "SlpCode", Function.ParseInt(buss["SlpCode"]));
			gv_Doc.SetRowCellValue(rowHandle, "TotalInvc", Function.ParseDouble(buss["TotalInvc"]));
			DataTable ownerByBpUser = HttpClientHelperFactory.GetHttpClient().Documents.GetOwnerByBpUser(Function.ToString(buss["CardCode"]), GlobalData.UserID);
			if (ownerByBpUser != null && ownerByBpUser.Rows.Count > 0)
			{
				if (Function.ParseInt(ownerByBpUser.Rows[0]["OwnerCode"]) > 0)
				{
					gv_Doc.SetRowCellValue(rowHandle, "Owner", Function.ParseInt(ownerByBpUser.Rows[0]["OwnerCode"]));
					if (Function.ParseInt(ownerByBpUser.Rows[0]["SlpCode"]) > 0)
					{
						gv_Doc.SetRowCellValue(rowHandle, "SlpCode", Function.ParseInt(ownerByBpUser.Rows[0]["SlpCode"]));
					}
				}
				else
				{
					gv_Doc.SetRowCellValue(rowHandle, "Owner", null);
				}
			}
			if (gv_Stage.RowCount > 0)
			{
				gv_Stage.SetRowCellValue(gv_Stage.RowCount - 1, "SlpCode", Function.ParseInt(buss["SlpCode"]));
			}
			ChangeButtonOkText(sender);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void bteCFL_ChnCrdCode_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			string text = Function.ToString(bteCFL_ChnCrdCode.EditValue);
			if (string.IsNullOrEmpty(text))
			{
				txt_ChnCrdName.Text = "";
				lukDN_ChnCrdCon.Properties.DataSource = new DataTable();
				LoadComboBpChanelContact();
				return;
			}
			DataTable bPData = _httpClient.OpportunityClient.GetBPData(text);
			if (bPData.IsNotNull())
			{
				txt_ChnCrdName.Text = Function.ToString(bPData.Rows[0]["CardName"]);
			}
			LoadComboBpChanelContact(setDefault: true);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void gvItemDetail_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			_opprId = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("OpprId"));
			ChangeButtonOkText(sender);
			if (e.Column.FieldName.Equals("ItemCode"))
			{
				string itemCode = Function.ToString(e.Value);
				DataTable dataTable = HttpClient.Commons.ItemMaster(itemCode, GlobalData.DefaultBranch, Enums.ItemObjectType.SellItem, Function.ToString(gv_Doc.GetFocusedRowCellValue("CardCode")), null, 1.0, null);
				if (dataTable != null && dataTable.Rows.Count == 1)
				{
					gv_ItemDetail.SetRowCellValue(e.RowHandle, "OpportId", _opprId);
					gv_ItemDetail.SetRowCellValue(e.RowHandle, "Line", GetLineNum(_dtIte, "ItemCode"));
					gv_ItemDetail.SetRowCellValue(e.RowHandle, "ItemName", dataTable.Rows[0]["ItemDescription"]);
					gv_ItemDetail.SetRowCellValue(e.RowHandle, "Quantity", 1);
					gv_ItemDetail.SetRowCellValue(e.RowHandle, "Price", 0);
					gv_ItemDetail.SetRowCellValue(e.RowHandle, "FrgnName", dataTable.Rows[0]["FrgnName"]);
					gv_ItemDetail.SetRowCellValue(e.RowHandle, "DeliveryDate", Function.ParseDateTimes(datePredCloDate.EditValue));
					gv_ItemDetail.SetRowCellValue(e.RowHandle, "IPercent", 0);
					if (Function.ToString(dataTable.Rows[0]["BOMTreeType"]) == "S" || Function.ToString(dataTable.Rows[0]["BOMTreeType"]) == "T")
					{
						DataTable childItemsInBom = WinFuntions.GetChildItemsInBom(Function.ToString(e.Value), null);
						int num = e.RowHandle;
						if (childItemsInBom.IsNotNull())
						{
							for (int i = 0; i < childItemsInBom.Rows.Count; i++)
							{
								num++;
								DataRow row = _dtIte.NewRow();
								_dtIte.Rows.InsertAt(row, num);
								_dtIte.AcceptChanges();
								gv_ItemDetail.SetRowCellValue(num, "ItemCode", childItemsInBom.Rows[i]["ItemCode"]);
							}
						}
					}
				}
				await AddNewRowItem(clear: false);
			}
			else if (e.Column.FieldName.ToUpper().Equals("QUANTITY") || e.Column.FieldName.ToUpper().Equals("IPERCENT") || e.Column.FieldName.ToUpper().Equals("PRICE"))
			{
				decimal num2 = ((gv_ItemDetail.GetFocusedRowCellValue("Quantity") == null) ? 0m : Function.ParseDecimal(gv_ItemDetail.GetFocusedRowCellValue("Quantity")));
				decimal num3 = ((gv_ItemDetail.GetFocusedRowCellValue("IPercent") == null) ? 0m : Function.ParseDecimal(gv_ItemDetail.GetFocusedRowCellValue("IPercent")));
				decimal num4 = ((gv_ItemDetail.GetFocusedRowCellValue("Price") == null) ? 0m : Function.ParseDecimal(gv_ItemDetail.GetFocusedRowCellValue("Price"))) * num2 * (1m + num3 / 100m);
				int ln = ((gv_ItemDetail.GetFocusedRowCellValue("Line") == null) ? (-1) : Function.ParseInt(gv_ItemDetail.GetFocusedRowCellValue("Line")));
				if (grdItemDetail.DataSource is DataTable dataTable2)
				{
					DataRow dataRow = dataTable2.AsEnumerable().FirstOrDefault((DataRow t) => ln == Function.ParseInt(t["Line"]));
					if (dataRow != null)
					{
						dataRow["PotentAmount"] = num4;
						dataTable2.AcceptChanges();
					}
				}
			}
			if (e.Column.FieldName.ToUpper().Equals("POTENTAMOUNT"))
			{
				decimal num5 = ((gv_ItemDetail.GetFocusedRowCellValue("Quantity") == null) ? 0m : Function.ParseDecimal(gv_ItemDetail.GetFocusedRowCellValue("Quantity")));
				decimal num6 = ((gv_ItemDetail.GetFocusedRowCellValue("PotentAmount") == null) ? 0m : Function.ParseDecimal(gv_ItemDetail.GetFocusedRowCellValue("PotentAmount")));
				decimal num7 = ((gv_ItemDetail.GetFocusedRowCellValue("Price") == null) ? 0m : Function.ParseDecimal(gv_ItemDetail.GetFocusedRowCellValue("Price")));
				if (num7 != 0m && num5 != 0m)
				{
					decimal num8 = (num6 / (num7 * num5) - 1m) * 100m;
					int ln2 = ((gv_ItemDetail.GetFocusedRowCellValue("Line") == null) ? (-1) : Function.ParseInt(gv_ItemDetail.GetFocusedRowCellValue("Line")));
					if (grdItemDetail.DataSource is DataTable dataTable3)
					{
						DataRow dataRow2 = dataTable3.AsEnumerable().FirstOrDefault((DataRow t) => ln2 == Function.ParseInt(t["Line"]));
						if (dataRow2 != null)
						{
							dataRow2["IPercent"] = num8;
							dataTable3.AcceptChanges();
						}
					}
				}
			}
			object summaryValue = gv_ItemDetail.Columns["PotentAmount"].SummaryItem.SummaryValue;
			if (summaryValue != null && GlobalData.ICfg_OppPotential == "Y")
			{
				calcPotentialAmount.EditValue = Function.ParseDecimal(summaryValue);
				CalcEditEx calcEditEx = calcWeightAmount;
				object editValue = (calcWeightAmount.EditValue = GetStageWeightAmount(Function.ParseDecimal(summaryValue)));
				calcEditEx.EditValue = editValue;
				if (grdStage.DataSource is DataTable dataTable4)
				{
					DataRow dataRow3 = dataTable4.Rows[dataTable4.Rows.Count - 1];
					decimal num9 = Function.ParseDecimal(summaryValue);
					decimal num10 = ((gv_Stage.GetFocusedRowCellValue("ClosePrcnt") == null) ? 0m : Function.ParseDecimal(gv_Stage.GetFocusedRowCellValue("ClosePrcnt")));
					decimal num11 = num9 * num10 / 100m;
					dataRow3["MaxSumLoc"] = num9;
					dataRow3["WtSumLoc"] = num11;
					dataTable4.AcceptChanges();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rILStage_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (sender is LookUpEdit lookUpEdit && lookUpEdit.Properties.GetDataSourceRowByKeyValue(lookUpEdit.EditValue) is DataRowView dataRowView)
			{
				int num = Function.ParseInt(dataRowView["Step_Id"]);
				DataTable dataTable = rILStage.DataSource as DataTable;
				if (dataTable != null && dataTable.PrimaryKey.Length == 0)
				{
					dataTable.PrimaryKey = new DataColumn[1] { dataTable.Columns["Step_Id"] };
				}
				int num2 = Function.ParseInt(gv_Stage.GetFocusedRowCellValue("Line"));
				if (dataTable != null)
				{
					_dtSta.PrimaryKey = new DataColumn[1] { _dtSta.Columns["Line"] };
					DataRow dataRow = dataTable.Rows.Find(num);
					_dtSta.Rows.Find(num2)["ClosePrcnt"] = dataRow["CloPrcnt"];
					if (grdStage.DataSource is DataTable dataTable2)
					{
						dataTable2.PrimaryKey = new DataColumn[1] { dataTable2.Columns["Line"] };
						int num3 = ((gv_Stage.GetFocusedRowCellValue("Line") != null) ? Function.ParseInt(gv_Stage.GetFocusedRowCellValue("Line")) : 0);
						DataRow dataRow2 = dataTable2.Rows.Find(num3);
						decimal num4 = Function.ParseDecimal(dataRow["CloPrcnt"]);
						decimal num5 = ((gv_Stage.GetFocusedRowCellValue("MaxSumLoc") == null) ? 0m : Function.ParseDecimal(gv_Stage.GetFocusedRowCellValue("MaxSumLoc"))) * num4 / 100m;
						dataRow2["WtSumLoc"] = num5;
						dataTable2.AcceptChanges();
						calcWeightAmount.EditValue = Function.ParseDecimal(num5);
					}
					int dataSourceRowIndex = gv_Doc.GetDataSourceRowIndex(gv_Doc.FocusedRowHandle);
					dtDoc.Rows[dataSourceRowIndex]["CloPrcnt"] = dataRow["CloPrcnt"];
				}
			}
			dtDoc.AcceptChanges();
			_dtSta.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvStage_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (_isLoading)
			{
				return;
			}
			_opprId = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("OpprId"));
			ChangeButtonOkText(sender);
			if (e.Column.FieldName == "ClosePrcnt")
			{
				if (dtDoc.Columns.Contains("OpprId") && !dtDoc.PrimaryKey.Contains(dtDoc.Columns["OpprId"]))
				{
					dtDoc.PrimaryKey = new DataColumn[1] { dtDoc.Columns["OpprId"] };
				}
				DataRow dataRow = dtDoc.Rows.Find(_opprId);
				if (dataRow != null)
				{
					dataRow["CloPrcnt"] = e.Value;
				}
			}
			if ((e.Column.FieldName == "CloseDate" || e.Column.FieldName == "OpenDate") && grdStage.DataSource is DataTable dataTable)
			{
				dataTable.PrimaryKey = new DataColumn[1] { dataTable.Columns["Line"] };
				int num = ((gv_Stage.GetFocusedRowCellValue("Line") != null && gv_Stage.GetFocusedRowCellValue("Line") != DBNull.Value) ? Function.ParseInt(gv_Stage.GetFocusedRowCellValue("Line")) : 0);
				DataRow dataRow2 = dataTable.Rows.Find(num);
				if (dataRow2 != null)
				{
					int num2 = dataTable.Rows.IndexOf(dataRow2);
					DateTime dateTime = ((gv_Stage.GetFocusedRowCellValue("OpenDate") == null || gv_Stage.GetFocusedRowCellValue("OpenDate") == DBNull.Value) ? DateTime.Today : Convert.ToDateTime(gv_Stage.GetFocusedRowCellValue("OpenDate")));
					DateTime? dateTime2 = Function.ParseDateTime(gv_Stage.GetFocusedRowCellValue("CloseDate"));
					if (num2 > 0)
					{
						DateTime? dateTime3 = Function.ParseDateTime(dataTable.Rows[num2 - 1]["CloseDate"]);
						if (dateTime3.HasValue)
						{
							DateTime value = dateTime;
							DateTime? dateTime4 = dateTime3;
							if (value < dateTime4)
							{
								WinCommons.Error(Enums.MessageCode.NextStageOpenDateCanNotBeforePreviousStage, "");
								_staErrRow = e.RowHandle;
								_staErrCol = e.Column.FieldName;
								_staErrorString = "Next stage Open date can not before previous stage closing date";
								return;
							}
						}
					}
					if (dateTime2.HasValue)
					{
						DateTime value = dateTime;
						DateTime? dateTime4 = dateTime2;
						if (value > dateTime4)
						{
							WinCommons.Error(Enums.MessageCode.OpenDateCanNotAfterClosingDate, "");
							_staErrorString = "Open date can not after closing date";
							_staErrRow = e.RowHandle;
							_staErrCol = e.Column.FieldName;
							return;
						}
					}
					_staErrRow = -1;
					_staErrCol = "";
					_staErrorString = "";
				}
			}
			if (e.Column.FieldName == "ClosePrcnt" && grdStage.DataSource is DataTable dataTable2)
			{
				dataTable2.PrimaryKey = new DataColumn[1] { dataTable2.Columns["Line"] };
				int num3 = ((gv_Stage.GetFocusedRowCellValue("Line") != null) ? Function.ParseInt(gv_Stage.GetFocusedRowCellValue("Line")) : 0);
				DataRow dataRow3 = dataTable2.Rows.Find(num3);
				decimal num4 = Function.ParseDecimal(e.Value);
				decimal num5 = ((gv_Stage.GetFocusedRowCellValue("MaxSumLoc") == null) ? 0m : Function.ParseDecimal(gv_Stage.GetFocusedRowCellValue("MaxSumLoc"))) * num4 / 100m;
				dataRow3["WtSumLoc"] = num5;
				dataTable2.AcceptChanges();
				calcWeightAmount.EditValue = Function.ParseDecimal(num5);
			}
			if (e.Column.FieldName == "WtSumLoc" && grdStage.DataSource is DataTable dataTable3)
			{
				dataTable3.PrimaryKey = new DataColumn[1] { dataTable3.Columns["Line"] };
				int num6 = ((gv_Stage.GetFocusedRowCellValue("Line") != null) ? Function.ParseInt(gv_Stage.GetFocusedRowCellValue("Line")) : 0);
				DataRow dataRow4 = dataTable3.Rows.Find(num6);
				decimal num7 = Function.ParseDecimal(e.Value);
				decimal num8 = ((gv_Stage.GetFocusedRowCellValue("ClosePrcnt") == null) ? 0m : Function.ParseDecimal(gv_Stage.GetFocusedRowCellValue("ClosePrcnt")));
				decimal num9 = num7 * 100m / ((num8 == 0m) ? 1m : num8);
				dataRow4["MaxSumLoc"] = num9;
				dataTable3.AcceptChanges();
				calcPotentialAmount.EditValue = num9;
				calcWeightAmount.EditValue = num7;
			}
			if (e.Column.FieldName == "MaxSumLoc")
			{
				SetStageWeightAmount(Function.ParseDecimal(e.Value));
				calcWeightAmount.EditValue = GetStageWeightAmount(Function.ParseDecimal(e.Value));
				calcPotentialAmount.EditValue = Function.ParseDecimal(e.Value);
				decimal num10 = ((calcGrossProfit.EditValue == null) ? 0m : Function.ParseDecimal(calcGrossProfit.EditValue));
				calcGrossProfitTotal.EditValue = Function.ParseDecimal(e.Value) * num10 / 100m;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvReason_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			ChangeButtonOkText(sender);
			if (e.Column.FieldName.ToUpper().Equals("REASONDID"))
			{
				AddNewRowReason(clear: false);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvDoc_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View && _opprId != -1)
			{
				ChangeButtonOkText(sender);
				_opprId = ((gv_Doc.GetFocusedRowCellValue("OpprId") == null) ? (-1) : Function.ParseInt(gv_Doc.GetFocusedRowCellValue("OpprId")));
			}
			if (!_isLoading && e.Column.FieldName == "OpprType")
			{
				gv_Doc.SetFocusedRowCellValue("CardCode", "");
				gv_Doc.SetFocusedRowCellValue("CardName", "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void rILSalesEmp_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (Function.ParseInt(gv_Doc.GetFocusedRowCellValue("Owner")) <= 0 && !_isLoading && sender is LookUpEdit lookUpEdit)
			{
				lookUpEdit.GetSelectedDataRow();
				DataTable dataTable = await ExtenseionMethods.GetEmployeeCodeBySlpCode(Function.ParseInt(((DataRowView)lookUpEdit.GetSelectedDataRow()).Row["SlpCode"]));
				if (dataTable.Rows.Count != 0)
				{
					gv_Doc.SetFocusedRowCellValue("Owner", Function.ParseInt(dataTable.Rows[0]["OwnerCode"]));
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rILPartner_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			ChangeButtonOkText(sender);
			DataRow dataRow = _dtBus.Rows[gv_Partner.GetDataSourceRowIndex(gv_Partner.FocusedRowHandle)];
			if (dataRow != null)
			{
				int num = Function.ParseInt(((LookUpEdit)sender).EditValue);
				object displayValueByKeyValue = rILPartner.GetDisplayValueByKeyValue(num);
				DataRowView dataRowView = (DataRowView)rILPartner.GetDataSourceRowByKeyValue(num);
				if (displayValueByKeyValue == null || displayValueByKeyValue == DBNull.Value || string.IsNullOrWhiteSpace(displayValueByKeyValue.ToString()))
				{
					dataRow["ParterId"] = num;
					dataRow["OrlCode"] = -1;
					dataRow["RelatCard"] = "";
					dataRow["Memo"] = "";
				}
				else
				{
					dataRow["ParterId"] = num;
					dataRow["OrlCode"] = dataRowView.Row["DefaultORL"];
					dataRow["RelatCard"] = dataRowView.Row["RelatCard"];
					dataRow["Memo"] = dataRowView.Row["Memo"];
				}
				_dtBus.AcceptChanges();
			}
			AddNewRowPartner(clear: false);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rILCompetitor_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			ChangeButtonOkText(sender);
			DataRow dataRow = _dtCom.Rows[gv_Competitor.GetDataSourceRowIndex(gv_Competitor.FocusedRowHandle)];
			if (dataRow != null)
			{
				int num = Function.ParseInt(((LookUpEdit)sender).EditValue);
				object displayValueByKeyValue = rILCompetitor.GetDisplayValueByKeyValue(num);
				if (displayValueByKeyValue == null || displayValueByKeyValue == DBNull.Value || string.IsNullOrWhiteSpace(displayValueByKeyValue.ToString()))
				{
					dataRow["CompetId"] = num;
					dataRow["ThreatLevl"] = Function.ParseShort(1);
					dataRow["Memo"] = "";
				}
				else
				{
					dataRow["CompetId"] = num;
					DataRowView dataRowView = (DataRowView)rILCompetitor.GetDataSourceRowByKeyValue(num);
					switch ((dataRowView.Row["ThreatLevl"] == DBNull.Value || dataRowView.Row["ThreatLevl"] == null) ? (-1) : Function.ParseInt(dataRowView.Row["ThreatLevl"]))
					{
					case -1:
						dataRow["ThreatLevl"] = Function.ParseShort(1);
						break;
					case 1:
						dataRow["ThreatLevl"] = Function.ParseShort(1);
						break;
					case 2:
						dataRow["ThreatLevl"] = Function.ParseShort(2);
						break;
					case 3:
						dataRow["ThreatLevl"] = Function.ParseShort(3);
						break;
					}
					dataRow["Memo"] = dataRowView.Row["Memo"];
				}
				_dtCom.AcceptChanges();
			}
			AddNewRowCompetition(clear: false);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void calcGrossProfit_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				if (FormMode != Enums.FormViewModeType.View)
				{
					decimal num = ((calcPotentialAmount.EditValue == null) ? 0m : Function.ParseDecimal(calcPotentialAmount.EditValue));
					decimal num2 = ((calcGrossProfit.EditValue == null) ? 0m : Function.ParseDecimal(calcGrossProfit.EditValue));
					calcGrossProfitTotal.EditValue = Math.Round(num * num2 / 100m, GlobalData.GeneralSetting.SumDec.Value);
				}
				else if (!calcGrossProfit.Properties.IsRounding)
				{
					ChangeButtonOkText(sender);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void calcPotentialAmount_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				if (FormMode != Enums.FormViewModeType.View)
				{
					decimal num = ((calcPotentialAmount.EditValue == null) ? 0m : Function.ParseDecimal(calcPotentialAmount.EditValue));
					DataTable dataTable = grdStage.DataSource as DataTable;
					DataRow dataRow = dataTable.Rows[dataTable.Rows.Count - 1];
					decimal num2 = ((gv_Stage.GetRowCellValue(dataTable.Rows.Count - 1, "ClosePrcnt") == null) ? 0m : Function.ParseDecimal(gv_Stage.GetRowCellValue(dataTable.Rows.Count - 1, "ClosePrcnt")));
					decimal num3 = num * num2 / 100m;
					dataRow["MaxSumLoc"] = num;
					dataRow["WtSumLoc"] = num3;
					_isLoading = true;
					dataTable.AcceptChanges();
					decimal num4 = ((calcGrossProfit.EditValue == null) ? 0m : Function.ParseDecimal(calcGrossProfit.EditValue));
					_isLoading = true;
					calcWeightAmount.EditValue = Math.Round(num3, GlobalData.GeneralSetting.SumDec.Value);
					_isLoading = true;
					calcGrossProfitTotal.EditValue = Math.Round(num4 * num / 100m, GlobalData.GeneralSetting.SumDec.Value);
					_isLoading = false;
				}
				else if (!calcPotentialAmount.Properties.IsRounding)
				{
					ChangeButtonOkText(sender);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void calcWeightAmount_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (_isLoading)
			{
				return;
			}
			if (FormMode != Enums.FormViewModeType.View)
			{
				decimal num = ((gv_Stage.GetFocusedRowCellValue("ClosePrcnt") == null) ? 0m : Function.ParseDecimal(gv_Stage.GetFocusedRowCellValue("ClosePrcnt")));
				decimal num2;
				if (num > 0m)
				{
					num2 = ((calcWeightAmount.EditValue == null) ? 0m : Function.ParseDecimal(calcWeightAmount.EditValue)) * 100m / num;
					SetStagePotentialAmount(num2);
					decimal num3 = ((calcGrossProfit.EditValue == null) ? 0m : Function.ParseDecimal(calcGrossProfit.EditValue));
					calcPotentialAmount.EditValue = Math.Round(num2);
					calcGrossProfitTotal.EditValue = Math.Round(num3 * num2 / 100m, GlobalData.GeneralSetting.SumDec.Value);
				}
				else
				{
					num2 = 0m;
				}
				SetStageWeightAmount(num2);
			}
			else if (!calcWeightAmount.Properties.IsRounding)
			{
				ChangeButtonOkText(sender);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rep_Luk_OOPR_OpprType_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (sender is BaseEdit baseEdit)
			{
				ShowHideStageCombo(Function.ToString(baseEdit.EditValue));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rIBStageDocNo_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			int num = Function.ParseInt(gv_Stage.GetFocusedRowCellValue("ObjType"));
			if (e.Button.Index == 0)
			{
				string text = ((gv_Doc.GetFocusedRowCellValue("CardCode") == DBNull.Value || gv_Doc.GetFocusedRowCellValue("CardCode") == null) ? "" : gv_Doc.GetFocusedRowCellValue("CardCode").ToString());
				if (text != "" && ((gv_Stage.GetFocusedRowCellValue("DocChkbox") == DBNull.Value || gv_Stage.GetFocusedRowCellValue("DocChkbox") == null) ? "" : gv_Stage.GetFocusedRowCellValue("DocChkbox").ToString()) == "N")
				{
					text = "";
				}
				if (num <= 0)
				{
					return;
				}
				frm_List_Document frm_List_Document2 = new frm_List_Document((ApzObjectTypes)num, multiSelect: false, text, isNoStatus: true);
				frm_List_Document2.ShowDialog(this);
				if (frm_List_Document2.ChooseItem != null)
				{
					gv_Stage.SetFocusedRowCellValue("DocNumber", frm_List_Document2.ChooseItem.DocNum);
					if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdateAmountAndProfit, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
					{
						double documentAmountAndProfit = _httpClient.OpportunityClient.GetDocumentAmountAndProfit(num, frm_List_Document2.ChooseItem.DocNum);
						gv_Stage.SetFocusedRowCellValue("MaxSumLoc", documentAmountAndProfit);
					}
					cbSummaryDocType.SelectedItem = num;
					txtDocNo.EditValue = frm_List_Document2.ChooseItem.DocNum;
				}
			}
			else
			{
				string text2 = ((gv_Stage.GetFocusedRowCellValue("DocNumber") == DBNull.Value || gv_Stage.GetFocusedRowCellValue("DocNumber") == null) ? "" : gv_Stage.GetFocusedRowCellValue("DocNumber").ToString());
				if (!string.IsNullOrWhiteSpace(text2))
				{
					OpenDocForm(Function.ParseInt(num), Function.ParseInt(text2));
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txtDocNo_Properties_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index != 0)
			{
				return;
			}
			string text = ((cbSummaryDocType.SelectedItem == null) ? "" : cbSummaryDocType.SelectedItem.ToString());
			if (text != "")
			{
				string txtText = ((gv_Doc.GetFocusedRowCellValue("CardCode") == DBNull.Value || gv_Doc.GetFocusedRowCellValue("CardCode") == null) ? "" : gv_Doc.GetFocusedRowCellValue("CardCode").ToString());
				frm_List_Document frm_List_Document2 = new frm_List_Document(WinFuntions.GetDocTypeByName(text), multiSelect: false, txtText, isNoStatus: true);
				frm_List_Document2.ShowDialog(this);
				if (frm_List_Document2.ChooseItem != null)
				{
					gv_Stage.SetFocusedRowCellValue("DocNumber", frm_List_Document2.ChooseItem.DocNum);
					txtDocNo.EditValue = frm_List_Document2.ChooseItem.DocNum;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void deleteRowsToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			int[] selectedRows = gv_ItemDetail.GetSelectedRows();
			List<int> list = new List<int>();
			int[] array = selectedRows;
			foreach (int rowHandle in array)
			{
				list.Add(Function.ParseInt(gv_ItemDetail.GetRowCellValue(rowHandle, "Line")));
			}
			foreach (int item in list)
			{
				DataRow row = (from t in _dtIte.AsEnumerable()
					where Function.ParseInt(t["Line"]) == item
					select t).FirstOrDefault();
				_dtIte.Rows.Remove(row);
				_dtIte.AcceptChanges();
			}
			if (selectedRows.Length != 0 && FormMode == Enums.FormViewModeType.View)
			{
				ChangeButtonOkText(sender);
				_opprId = ((gv_Doc.GetFocusedRowCellValue("OpprId") == null) ? (-1) : Function.ParseInt(gv_Doc.GetFocusedRowCellValue("OpprId")));
			}
			if (GlobalData.LoginSap)
			{
				await AddNewRowItem(clear: false);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void addNewToolStripMenuItem_Click(object sender, EventArgs e)
	{
		await REFRESH();
	}

	private async void deleteDocumentToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (_isLoading || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmRemoveSalesOpportunity, Enums.MessageCode.SystemMessage, MessageBoxButtons.OKCancel) != DialogResult.OK)
			{
				return;
			}
			int[] selectedRows = gv_Doc.GetSelectedRows();
			if (selectedRows.Length != 0)
			{
				string ids = Function.ToString(gv_Doc.GetRowCellValue(selectedRows[0], "OpprId"));
				string text = await _httpClient.OpportunityClient.DeleteDoc(ids);
				if (string.IsNullOrEmpty(text))
				{
					WinCommons.Success(Enums.MessageCode.RemoveSuccess, "");
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, text);
				}
			}
			btSearch_Click(null, null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void createSalesQuotationToolStripMenuItem_Click(object sender, EventArgs e)
	{
		WinCommons._OpenProcessing();
		try
		{
			frm_SalesQuotationForBatch frm_SalesQuotationForBatch2 = new frm_SalesQuotationForBatch();
			int num = Function.ParseInt(gv_Doc.GetRowCellValue(gv_Doc.FocusedRowHandle, "OpprId"));
			if (num == _newOpprId)
			{
				return;
			}
			frm_SalesQuotationForBatch2.OpporId = num;
			frm_SalesQuotationForBatch2.MdiParent = MdiParent;
			frm_SalesQuotationForBatch2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	private async void rIBItemCode_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			ChangeButtonOkText(sender);
			string text = ((gv_ItemDetail.GetFocusedRowCellValue("ItemCode") == null) ? "" : Function.ToString(gv_ItemDetail.GetFocusedRowCellValue("ItemCode")));
			if (e.Button.Index == 0)
			{
				frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.ListItems, true, text, "O", Enums.ItemObjectType.SellItem, null, "");
				frm_CFL2.ShowDialog(this);
				List<DataRow> chooseItems = frm_CFL2.ChooseItems;
				if (frm_CFL2.ChooseItems != null && frm_CFL2.ChooseItems.Count > 0)
				{
					DataRow dataRow = chooseItems[0];
					gv_ItemDetail.SetFocusedRowCellValue("ItemCode", dataRow["ItemCode"]);
					for (int i = 1; i < frm_CFL2.ChooseItems.Count; i++)
					{
						gv_ItemDetail.SetRowCellValue(gv_ItemDetail.RowCount - 1, "ItemCode", chooseItems[i]["ItemCode"]);
						_dtIte.AcceptChanges();
					}
					if (GlobalData.LoginSap)
					{
						await AddNewRowItem(clear: false);
					}
				}
			}
			else if (e.Button.Index == 1 && !string.IsNullOrEmpty(text))
			{
				ExtenseionMethods.LoadBoMFromGridItem(text, MdiParent);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void addRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (GlobalData.LoginSap)
			{
				await AddNewRowItem(clear: false);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void toolStripMenuItemAddRow_Click(object sender, EventArgs e)
	{
		try
		{
			if (!string.IsNullOrEmpty(Function.ToString(gv_Doc.GetFocusedRowCellValue("CardCode"))))
			{
				AddNewRowStage(clear: false);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			int dataRowCount = gv_Stage.DataRowCount;
			if (dataRowCount > 1)
			{
				_dtSta.Rows.RemoveAt(gv_Stage.GetFocusedDataSourceRowIndex());
				_dtSta.AcceptChanges();
				decimal num = Function.ParseDecimal(_dtSta.Rows[dataRowCount - 2]["MaxSumLoc"]);
				decimal num2 = Function.ParseDecimal(_dtSta.Rows[dataRowCount - 2]["WtSumLoc"]);
				decimal num3 = Function.ParseDecimal(calcGrossProfit.EditValue) * num / 100m;
				_isLoading = true;
				calcPotentialAmount.EditValue = num;
				calcWeightAmount.EditValue = num2;
				calcGrossProfitTotal.EditValue = num3;
				_isLoading = false;
				grdStage.RefreshDataSource();
				_dtLinkedAct.Rows.Clear();
				_dtLinkedAct.AcceptChanges();
				ChangeButtonOkText(sender);
			}
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rIBRelaPartner_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			string txtText = "";
			if (gv_Partner.GetFocusedRowCellValue("RelatCard") != null)
			{
				txtText = gv_Partner.GetFocusedRowCellValue("RelatCard").ToString();
			}
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.Bp, multiSelect: false, txtText);
			frm_ChooseFromList2.ShowDialog(this);
			if (frm_ChooseFromList2.ChooseItem != null)
			{
				BusinessPartnersImpl businessPartnersImpl = (BusinessPartnersImpl)frm_ChooseFromList2.ChooseItem;
				gv_Partner.SetFocusedRowCellValue("RelatCard", businessPartnersImpl.CardCode);
				ChangeButtonOkText(sender);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rILPartner_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 1)
			{
				frm_SalesOpportunityPartner frm_SalesOpportunityPartner2 = new frm_SalesOpportunityPartner();
				frm_SalesOpportunityPartner2.MdiParent = MdiParent;
				frm_SalesOpportunityPartner2.FormClosing += async (object snder, FormClosingEventArgs e2) =>
				{
					await LoadComboPartner();
				};
				frm_SalesOpportunityPartner2.Show();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rILPartnerRela_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 1)
			{
				frm_SalesOpportunityRelationship frm_SalesOpportunityRelationship2 = new frm_SalesOpportunityRelationship();
				frm_SalesOpportunityRelationship2.MdiParent = MdiParent;
				frm_SalesOpportunityRelationship2.FormClosing += async (object snder, FormClosingEventArgs e2) =>
				{
					await LoadComboPartnerRelationship();
				};
				frm_SalesOpportunityRelationship2.Show();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rILReason_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 1)
			{
				frm_SalesOpportunityReason frm_SalesOpportunityReason2 = new frm_SalesOpportunityReason();
				frm_SalesOpportunityReason2.MdiParent = MdiParent;
				frm_SalesOpportunityReason2.FormClosing += async (object snder, FormClosingEventArgs e2) =>
				{
					await LoadComboReason();
				};
				frm_SalesOpportunityReason2.Show();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rILCompetitor_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 1)
			{
				frm_SalesOpportunityCompetitor frm_SalesOpportunityCompetitor2 = new frm_SalesOpportunityCompetitor();
				frm_SalesOpportunityCompetitor2.MdiParent = MdiParent;
				frm_SalesOpportunityCompetitor2.FormClosing += async (object snder, FormClosingEventArgs e2) =>
				{
					await LoadComboCompetitor();
				};
				frm_SalesOpportunityCompetitor2.Show();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void removeRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (grdReason.DataSource is DataTable dataTable && dataTable.Rows.Count > 0 && FormMode == Enums.FormViewModeType.View)
			{
				string text = Function.ToString(gv_Doc.GetFocusedRowCellValue("Status"));
				if (text == "Lost" || text == "Won")
				{
					gv_Reason.DeleteSelectedRows();
					dataTable.AcceptChanges();
					ChangeButtonOkText(sender);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void viewForeCastToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			int opportId = Function.ParseInt(gv_Doc.GetRowCellValue(gv_Doc.FocusedRowHandle, "OpprId"));
			frm_ForeCast frm_ForeCast2 = new frm_ForeCast();
			frm_ForeCast2.OpportId = opportId;
			frm_ForeCast2.MdiParent = MdiParent;
			frm_ForeCast2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void viewChangeLogToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			int num = Function.ParseInt(gv_Doc.GetRowCellValue(gv_Doc.FocusedRowHandle, "OpprId"));
			if (num == _newOpprId)
			{
				WinCommons.Error(Enums.MessageCode.ChangeLogIsNotAvailable, "");
				return;
			}
			frm_ChangeLog frm_ChangeLog2 = new frm_ChangeLog();
			frm_ChangeLog2.TypeChangeLog = Enums.TypeChangeLog.OpportunityItemDetail;
			frm_ChangeLog2.OppId = num;
			frm_ChangeLog2.MdiParent = MdiParent;
			frm_ChangeLog2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rep_bte_itemAttFile_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				OpenFileDialog openFileDialog = new OpenFileDialog();
				openFileDialog.Multiselect = false;
				if (openFileDialog.ShowDialog() == DialogResult.OK)
				{
					string fileName = openFileDialog.FileName;
					if (!string.IsNullOrEmpty(fileName))
					{
						gv_ItemDetail.SetFocusedRowCellValue("AttFile", Path.GetFileName(fileName));
						gv_ItemDetail.SetFocusedRowCellValue("AttFullPath", fileName);
					}
				}
				return;
			}
			string text = ((gv_ItemDetail.GetFocusedRowCellValue("AttFile") == DBNull.Value || gv_ItemDetail.GetFocusedRowCellValue("AttFile") == null) ? "" : gv_ItemDetail.GetFocusedRowCellValue("AttFile").ToString());
			string upLoadPath = CopyFileShareHelper.GetUpLoadPath(Path.GetExtension(text));
			if (!string.IsNullOrWhiteSpace(text))
			{
				string text2 = _ftpHelper.CopyFileSharing(CopyFileSharingCfg.Domain, upLoadPath + text, CopyFileSharingCfg.UserName, CopyFileSharingCfg.Password, Path.GetTempPath() + "APZ-IRS-SalesOpportunity" + text, copyForOpen: true);
				if (!File.Exists(text2))
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Sales-Oppr: File path=" + text2);
					text = ((gv_ItemDetail.GetFocusedRowCellValue("AttFullPath") == DBNull.Value || gv_ItemDetail.GetFocusedRowCellValue("AttFullPath") == null) ? "" : gv_ItemDetail.GetFocusedRowCellValue("AttFullPath").ToString());
					if (File.Exists(text))
					{
						Process.Start(text);
					}
				}
				else
				{
					Process.Start(text2);
				}
			}
			else
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Sales-Oppr: View file -> File path is empty. FileName:" + text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btEditBusPartner_Properties_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			string txtText = "";
			if (btEditBusPartner.EditValue != null)
			{
				txtText = btEditBusPartner.EditValue.ToString();
			}
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.Bp, multiSelect: false, txtText);
			frm_ChooseFromList2.ShowDialog(this);
			if (frm_ChooseFromList2.ChooseItem != null)
			{
				BusinessPartnersImpl businessPartnersImpl = (BusinessPartnersImpl)frm_ChooseFromList2.ChooseItem;
				btEditBusPartner.EditValue = businessPartnersImpl.CardCode;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rIBActivities_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			string text = Function.ToString(gv_Doc.GetFocusedRowCellValue("CardCode"));
			if (text == "")
			{
				WinCommons.Error(Enums.MessageCode.NoBusinessPartnerChoose, "");
				return;
			}
			if (_dtLinkedAct != null && _dtLinkedAct.Rows.Count > 0 && string.IsNullOrWhiteSpace(_dtLinkedAct.Rows[0]["ClgCode"].ToString()))
			{
				_dtLinkedAct.Rows.RemoveAt(0);
				_dtLinkedAct.AcceptChanges();
			}
			int focusedRowHandle = gv_Stage.FocusedRowHandle;
			if (_dtLinkedAct != null)
			{
				if (_dtLinkedAct.IsNotNull())
				{
					DataTable dataTable = _dtLinkedAct.Select("OprLine=" + focusedRowHandle).CopyToDataTable();
					dataTable.Copy();
					_ = dataTable.Rows.Count;
				}
				else
				{
					_dtLinkedAct.Clone();
				}
				frm_RelatedActivites frm_RelatedActivites2 = new frm_RelatedActivites(ApzObjectTypes.oSalesOpportunities, Function.ToString(_opprId), Function.ToString(focusedRowHandle));
				frm_RelatedActivites2.CardCode = text;
				frm_RelatedActivites2.MdiParent = MdiParent;
				frm_RelatedActivites2.Show();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rIBBussPartner_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				ChooseFromListBp(sender, Function.ToString(gv_Doc.GetFocusedRowCellValue("CardCode")));
				return;
			}
			string text = Function.ToString(gv_Doc.GetFocusedRowCellValue("CardCode")).Trim();
			if (text != "" && _httpClient.BusinessMaster.CheckDoc(text))
			{
				this.DrillDown(ApzObjectTypes.oBusinessPartners, text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ChooseFromListBp(object sender, string cardCode)
	{
		try
		{
			string text = ((gv_Doc.GetFocusedRowCellValue("OpprType") == null) ? "" : gv_Doc.GetFocusedRowCellValue("OpprType").ToString());
			Enums.TypeCFL type = Enums.TypeCFL.CustomerLead;
			if (text == "P")
			{
				type = Enums.TypeCFL.Vendor;
			}
			frm_CFL frm_CFL2 = new frm_CFL(type, false, cardCode, "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem != null)
			{
				LoadBpOnChange(frm_CFL2.ChooseItem, gv_Doc.FocusedRowHandle, sender);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btAdd_Click(object sender, EventArgs e)
	{
		_ = 5;
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
				return;
			}
			btAdd.Enabled = false;
			if (string.IsNullOrEmpty(Function.ToString(gv_Doc.GetFocusedRowCellValue("CardCode"))))
			{
				WinCommons.Error(Enums.MessageCode.BusinessPartnerCanotBeNull, "");
				btAdd.Enabled = true;
				return;
			}
			string text = await CheckCardCodes();
			if (text != "")
			{
				WinCommons.Error(Enums.MessageCode.BusinessPartnerDoesNotExistsOopr, "", text);
				btAdd.Enabled = true;
				return;
			}
			if (await CheckItems() != "")
			{
				WinCommons.Error(Enums.MessageCode.ItemCodeDoesNotExistsOopr, "");
				btAdd.Enabled = true;
				return;
			}
			if (_staErrRow != -1)
			{
				ErrorOnStageDate();
				btAdd.Enabled = true;
				return;
			}
			if (!CheckDocCloseDate())
			{
				btAdd.Enabled = true;
				return;
			}
			if (!CheckQuantity())
			{
				btAdd.Enabled = true;
				return;
			}
			List<DataRow> lstEmptyDr = new List<DataRow>();
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				DataTable dt = dtDoc.Clone();
				dt.TableName = "OOPR";
				dt.Rows.Clear();
				if (_opprId == -1)
				{
					if (FormMode == Enums.FormViewModeType.View)
					{
						_opprId = _newOpprId;
					}
					else
					{
						_opprId = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("OpprId"));
					}
				}
				DataRow dataRow = dtDoc.AsEnumerable().Single((DataRow t) => Function.ParseInt(t["OpprId"]) == _opprId);
				if (dataRow != null)
				{
					DataRow dataRow2 = dt.NewRow();
					dataRow2.ItemArray = dataRow.ItemArray;
					dt.Rows.Add(dataRow2);
				}
				if (_opprId == _newOpprId)
				{
					dt.Rows[0]["OpprId"] = 0;
				}
				else
				{
					dt.Rows[0]["OpprId"] = _opprId;
				}
				if (!GlobalData.LoginSap)
				{
					dt.Rows[0]["Owner"] = Function.ParseInt(dt.Rows[0]["OwnerCode"]);
					if (dt.Columns.Contains("OwnerName"))
					{
						dt.Columns.Remove(dt.Columns["OwnerName"]);
					}
					if (dt.Columns.Contains("OwnerCode"))
					{
						dt.Columns.Remove(dt.Columns["OwnerCode"]);
					}
					if (dt.Columns.Contains("TotalInvc"))
					{
						dt.Columns.Remove(dt.Columns["TotalInvc"]);
					}
					dt.AcceptChanges();
				}
				if (!CheckPredictDate(datePredCloDate.DateTime))
				{
					DateTime? dateTime = ((_dtSta.Rows[_dtSta.Rows.Count - 1]["CloseDate"] == null || _dtSta.Rows[_dtSta.Rows.Count - 1]["CloseDate"] == DBNull.Value) ? ((DateTime?)null) : Function.ParseDateTime(_dtSta.Rows[_dtSta.Rows.Count - 1]["CloseDate"]));
					if (dateTime.HasValue)
					{
						dt.Rows[0]["PredDate"] = dateTime;
					}
					else
					{
						dt.Rows[0]["PredDate"] = Function.ParseDateTime(_dtSta.Rows[dt.Rows.Count - 1]["OpenDate"]);
					}
				}
				else
				{
					dt.Rows[0]["PredDate"] = datePredCloDate.DateTime;
				}
				dt.Rows[0]["WtSumLoc"] = Function.ParseDecimal(calcWeightAmount.EditValue);
				dt.Rows[0]["PrcnProf"] = Function.ParseDecimal(calcGrossProfit.EditValue);
				dt.Rows[0]["SumProfL"] = Function.ParseDecimal(calcGrossProfitTotal.EditValue);
				string value = "M";
				switch (cbClosingType.SelectedIndex)
				{
				case 0:
					value = "M";
					break;
				case 1:
					value = "W";
					break;
				case 2:
					value = "D";
					break;
				}
				dt.Rows[0]["DifType"] = value;
				dt.Rows[0]["MaxSumLoc"] = Function.ParseDecimal(calcPotentialAmount.EditValue);
				if (cbSummaryDocType.SelectedItem != null)
				{
					dt.Rows[0]["DocType"] = ((int)WinFuntions.GetDocTypeByName(cbSummaryDocType.SelectedItem.ToString())).ToString();
				}
				if (dt.Rows[0]["DocType"] != null && !string.IsNullOrWhiteSpace(dt.Rows[0]["DocType"].ToString()) && !string.IsNullOrWhiteSpace(txtDocNo.Text))
				{
					dt.Rows[0]["DocNum"] = txtDocNo.Text;
				}
				dt.Rows[0]["ChnCrdCode"] = Function.ToString(bteCFL_ChnCrdCode.EditValue);
				dt.Rows[0]["ChnCrdName"] = txt_ChnCrdName.Text;
				dt.Rows[0]["ChnCrdCon"] = Function.ParseInt(lukDN_ChnCrdCon.EditValue);
				dt.Rows[0]["PrjCode"] = Function.ToString(bteCFL_PrjCode.EditValue);
				dt.Rows[0]["Source"] = Function.ParseInt(lukDN_Source.EditValue);
				dt.Rows[0]["Industry"] = Function.ParseInt(lukDN_Industry.EditValue);
				dt.Rows[0]["Memo"] = Function.ToString(memo_Memo.EditValue);
				if (FormMode == Enums.FormViewModeType.Add)
				{
					dt.Rows[0]["AtcEntry"] = 0;
				}
				dt.Rows[0]["IntRate"] = Function.ParseInt(luk_LevelOfInterest.EditValue);
				lstEmptyDr.Clear();
				foreach (DataRow row in _dtSta.Rows)
				{
					if (row["Step_Id"] != null)
					{
						if (Function.ParseInt(row["Step_Id"]) == -1)
						{
							lstEmptyDr.Add(row);
						}
					}
					else
					{
						lstEmptyDr.Add(row);
					}
				}
				foreach (DataRow item2 in lstEmptyDr)
				{
					_dtSta.Rows.Remove(item2);
				}
				if (_dtSta.Rows[_dtSta.Rows.Count - 1]["CloseDate"] == null || _dtSta.Rows[_dtSta.Rows.Count - 1]["CloseDate"] == DBNull.Value)
				{
					_dtSta.Rows[_dtSta.Rows.Count - 1]["CloseDate"] = _dtSta.Rows[_dtSta.Rows.Count - 1]["OpenDate"];
				}
				lstEmptyDr.Clear();
				foreach (DataRow row2 in _dtBus.Rows)
				{
					if (row2["ParterId"] != null)
					{
						if (Function.ParseInt(row2["ParterId"]) == -1)
						{
							lstEmptyDr.Add(row2);
						}
					}
					else
					{
						lstEmptyDr.Add(row2);
					}
				}
				foreach (DataRow item3 in lstEmptyDr)
				{
					_dtBus.Rows.Remove(item3);
				}
				lstEmptyDr.Clear();
				foreach (DataRow row3 in _dtCom.Rows)
				{
					if (row3["ThreatLevl"] != null)
					{
						switch (row3["ThreatLevl"].ToString())
						{
						case "Low":
							row3["ThreatLevl"] = "1";
							break;
						case "Medium":
							row3["ThreatLevl"] = "2";
							break;
						case "High":
							row3["ThreatLevl"] = "3";
							break;
						}
					}
					if (row3["CompetId"] != null)
					{
						if (Function.ParseInt(row3["CompetId"]) == -1)
						{
							lstEmptyDr.Add(row3);
						}
					}
					else
					{
						lstEmptyDr.Add(row3);
					}
				}
				foreach (DataRow item4 in lstEmptyDr)
				{
					_dtCom.Rows.Remove(item4);
				}
				lstEmptyDr.Clear();
				foreach (DataRow row4 in _dtRea.Rows)
				{
					if (row4["ReasondId"] != null)
					{
						if (Function.ParseInt(row4["ReasondId"]) == -1)
						{
							lstEmptyDr.Add(row4);
						}
					}
					else
					{
						lstEmptyDr.Add(row4);
					}
				}
				foreach (DataRow item5 in lstEmptyDr)
				{
					_dtRea.Rows.Remove(item5);
				}
				lstEmptyDr.Clear();
				foreach (DataRow row5 in _dtIte.Rows)
				{
					if (row5["ItemCode"] != null)
					{
						if (string.IsNullOrEmpty(row5["ItemCode"].ToString()))
						{
							lstEmptyDr.Add(row5);
						}
					}
					else
					{
						lstEmptyDr.Add(row5);
					}
				}
				foreach (DataRow item6 in lstEmptyDr)
				{
					_dtIte.Rows.Remove(item6);
				}
				DataTable dtLAct = new DataTable
				{
					Columns = 
					{
						{
							"ClgCode",
							typeof(string)
						},
						{
							"OprId",
							typeof(string)
						},
						{
							"OprLine",
							typeof(int)
						}
					}
				};
				foreach (DataRow row6 in _dtLinkedAct.Rows)
				{
					if (row6["ClgCode"] != null && !string.IsNullOrWhiteSpace(row6["ClgCode"].ToString()))
					{
						DataRow dataRow9 = dtLAct.NewRow();
						dataRow9["ClgCode"] = row6["ClgCode"];
						dataRow9["OprId"] = Function.ToString(_opprId);
						dataRow9["OprLine"] = Function.ParseInt(row6["OprLine"]);
						dtLAct.Rows.Add(dataRow9);
						dtLAct.AcceptChanges();
					}
				}
				foreach (DataRow row7 in _dtIte.Rows)
				{
					string sPath = ((row7["AttFullPath"] == DBNull.Value || row7["AttFullPath"] == null) ? "" : row7["AttFullPath"].ToString());
					string sName = ((row7["AttFile"] == DBNull.Value || row7["AttFile"] == null) ? "" : row7["AttFile"].ToString());
					bool flag = !string.IsNullOrWhiteSpace(sPath) && !string.IsNullOrWhiteSpace(sName) && File.Exists(sPath);
					if (flag)
					{
						flag = !(await ExistFileInDoc(_opprId, sName));
					}
					if (!flag)
					{
						continue;
					}
					string upLoadPath = CopyFileShareHelper.GetUpLoadPath(Path.GetExtension(sName));
					string text2 = _ftpHelper.CopyFileSharing(CopyFileSharingCfg.Domain, sPath, CopyFileSharingCfg.UserName, CopyFileSharingCfg.Password, upLoadPath + sName);
					if (text2 != upLoadPath + sName)
					{
						WinCommons._CloseProcessing();
						WinCommons.Error(Enums.MessageCode.UploadAttachmentFailed, "", text2);
						if (GlobalData.LoginSap)
						{
							await AddNewRowItem(clear: false);
						}
						AddNewRowCompetition(clear: false);
						AddNewRowReason(clear: false);
						AddNewRowPartner(clear: false);
						return;
					}
				}
				DataTable dataTable = ucAttachment1.UploadFileNoSAP();
				if (ucAttachment1.DataSource.IsNotNull() && (dataTable == null || dataTable.Rows.Count == 0))
				{
					return;
				}
				DataTable dataTable2 = grd_InterestRange.DataSource as DataTable;
				if (dataTable2 == null)
				{
					dataTable2 = OpportunityTableHelper.Interest.Clone();
				}
				lstEmptyDr.Clear();
				foreach (DataRow row8 in dataTable2.Rows)
				{
					if (Function.ParseInt(row8["IntId"]) <= 0)
					{
						lstEmptyDr.Add(row8);
					}
				}
				foreach (DataRow item7 in lstEmptyDr)
				{
					dataTable2.Rows.Remove(item7);
				}
				dataTable2.AcceptChanges();
				DataSet ds = new DataSet();
				List<DataTable> list = new List<DataTable> { dt, _dtSta, _dtCom, _dtBus, _dtRea, _dtIte, dtLAct, dataTable, dataTable2 };
				if (!GlobalData.LoginSap)
				{
					list.Remove(_dtIte);
				}
				DataTable[] array = list.ToArray();
				for (int num = 0; num < array.Length; num++)
				{
					array[num].TableName = "dt" + num;
					array[num].AcceptChanges();
					DataSet dataSet = array[num].DataSet;
					if (dataSet != null)
					{
						dataSet.Tables.Remove(array[num].TableName);
						dataSet.AcceptChanges();
					}
				}
				ds.Tables.AddRange(array);
				ds.AcceptChanges();
				WinCommons._OpenProcessing();
				string item = ((FormMode != Enums.FormViewModeType.Add) ? "U" : "A");
				string text3 = await _httpClient.OpportunityClient.SaveDoc(new Tuple<string, DataSet>(item, ds));
				if (!string.IsNullOrEmpty(text3))
				{
					WinCommons.Error(Enums.MessageCode.None, text3);
					ds.Tables.Clear();
					if (GlobalData.LoginSap)
					{
						await AddNewRowItem(clear: false);
					}
					AddNewRowCompetition(clear: false);
					AddNewRowReason(clear: false);
					AddNewRowPartner(clear: false);
				}
				else
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
					ds.Tables.Clear();
					btSearch_Click(null, null);
				}
				WinCommons._CloseProcessing();
			}
			btAdd.Enabled = true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			btAdd.Enabled = true;
		}
	}

	private void btCancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void btRelatedAct_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataTable = _dtLinkedAct.Copy();
			string cardCode = Function.ToString(gv_Doc.GetFocusedRowCellValue("CardCode"));
			dataTable.Copy();
			frm_RelatedActivites frm_RelatedActivites2 = new frm_RelatedActivites(ApzObjectTypes.oSalesOpportunities, Function.ToString(_opprId));
			frm_RelatedActivites2.CardCode = cardCode;
			frm_RelatedActivites2.MdiParent = MdiParent;
			frm_RelatedActivites2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btRalatedDoc_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataTable = grdStage.DataSource as DataTable;
			if (!dataTable.IsNotNull())
			{
				return;
			}
			DataTable dataTable2 = new DataTable();
			dataTable2.Columns.Add("DocNum", typeof(int));
			dataTable2.Columns.Add("ObjType", typeof(int));
			dataTable2.AcceptChanges();
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				int num = Function.ParseInt(dataTable.Rows[i]["ObjType"]);
				if (num > 0)
				{
					DataRow dataRow = dataTable2.NewRow();
					dataRow["DocNum"] = Function.ParseInt(dataTable.Rows[i]["DocNumber"]);
					dataRow["ObjType"] = num;
					dataTable2.Rows.Add(dataRow);
				}
			}
			dataTable2.AcceptChanges();
			if (dataTable2.IsNotNull())
			{
				frm_OpportunityLinkedDocument frm_OpportunityLinkedDocument2 = new frm_OpportunityLinkedDocument();
				frm_OpportunityLinkedDocument2.MdiParent = MdiParent;
				frm_OpportunityLinkedDocument2.DTDocNumAndType = dataTable2;
				frm_OpportunityLinkedDocument2.CardCode = Function.ToString(gv_Doc.GetFocusedRowCellValue("CardCode"));
				frm_OpportunityLinkedDocument2.CardName = Function.ToString(gv_Doc.GetFocusedRowCellValue("CardName"));
				frm_OpportunityLinkedDocument2.OpprId = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("OpprId"));
				frm_OpportunityLinkedDocument2.Show();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btSearch_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			_isLoading = true;
			DataTable searchDataTable = GetSearchDataTable();
			DataRow dataRow = searchDataTable.NewRow();
			if (Function.ParseInt(FromDocEntry) != 0)
			{
				dateFrom.EditValue = DBNull.Value;
				dateTo.EditValue = DBNull.Value;
				txtNoOfTrans.EditValue = Function.ParseInt(FromDocEntry);
			}
			DateTime? dateTime = Function.ParseDateTime(dateFrom.EditValue);
			DateTime? dateTime2 = Function.ParseDateTime(dateTo.EditValue);
			dataRow["FromDate"] = ((!dateTime.HasValue || dateTime.Value.Date < new DateTime(1950, 1, 1)) ? "" : dateTime.Value.ToString("yyyyMMdd"));
			dataRow["ToDate"] = ((!dateTime2.HasValue || dateTime2.Value.Date < new DateTime(1950, 1, 1)) ? "" : dateTime2.Value.ToString("yyyyMMdd"));
			dataRow["NoTrans"] = Function.ParseInt(txtNoOfTrans.EditValue);
			dataRow["BusPartner"] = Function.ToString(btEditBusPartner.EditValue);
			dataRow["Status"] = Function.ToString(cbOpportStatus.EditValue);
			dataRow["Stage"] = Function.ParseInt(cbStage.EditValue);
			dataRow["SaleEmp"] = Function.ParseInt(cbMainSalesEmp.EditValue);
			dataRow["UserName"] = GlobalData.UserCodeLogin;
			searchDataTable.Rows.Add(dataRow);
			_loadDataBy = "S";
			await Search(searchDataTable);
			int num = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("OpprId"));
			if (num > 0)
			{
				await LoadOneDoc(num);
			}
			ShowHideStageCombo(Function.ToString(gv_Doc.GetFocusedRowCellValue("OpprType")));
			_isLoading = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.View;
	}

	private void btRefresh_Click()
	{
		_opprId = -1;
		btSearch_Click(null, null);
	}

	private void OpenDocForm(int docType, int docNum = 0)
	{
		try
		{
			XtraFormEx xtraFormEx = new XtraFormEx();
			switch (docType)
			{
			case 23:
				xtraFormEx = new frm_SalesQuotationForBatch(docNum);
				break;
			case 17:
				xtraFormEx = new frm_SalesOrder(docNum);
				break;
			case 15:
				xtraFormEx = new frm_Delivery(docNum);
				break;
			case 13:
				xtraFormEx = new frm_ARInvoices(docNum);
				break;
			case 540000006:
				xtraFormEx = new frm_PurchaseQuotation(docNum);
				break;
			case 22:
				xtraFormEx = new frm_PurchaseOder(docNum);
				break;
			case 20:
				xtraFormEx = new frm_GoodReceiptPO(docNum);
				break;
			case 18:
				xtraFormEx = new frm_APInvoices(docNum);
				break;
			}
			xtraFormEx.MdiParent = MdiParent;
			xtraFormEx.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void SetColumnsReadOnly()
	{
		try
		{
			switch (btAdd.Text.ToLower())
			{
			case "ok":
			case "update":
			{
				for (int j = 0; j < gv_Doc.Columns.Count; j++)
				{
					gv_Doc.Columns[j].OptionsColumn.ReadOnly = _readOnlyColsInViewMode.Contains(gv_Doc.Columns[j].FieldName);
				}
				rIBBussPartner.Buttons[0].Visible = false;
				break;
			}
			case "find":
			case "add":
			{
				for (int i = 0; i < gv_Doc.Columns.Count; i++)
				{
					gv_Doc.Columns[i].OptionsColumn.ReadOnly = _readOnlyColsInAddMode.Contains(gv_Doc.Columns[i].FieldName);
				}
				rIBBussPartner.Buttons[0].Visible = true;
				break;
			}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task Search(DataTable dt)
	{
		_ = 2;
		try
		{
			WinCommons._OpenProcessing();
			Task task = RefreshData();
			Task<DataTable> taskTmp = _httpClient.OpportunityClient.SearchDoc(dt);
			await task;
			DataTable dataTable = await taskTmp;
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				dtDoc = dataTable;
				if (dtDoc.Columns.Contains("OpprId"))
				{
					dtDoc.PrimaryKey = new DataColumn[1] { dtDoc.Columns["OpprId"] };
				}
				for (int i = 0; i < dtDoc.Rows.Count; i++)
				{
					string text = Function.ToString(dtDoc.Rows[i]["Memo"]);
					if (!string.IsNullOrEmpty(text))
					{
						text = text.Replace("\r", Environment.NewLine);
						dtDoc.Rows[i]["Memo"] = text;
					}
				}
			}
			else
			{
				dtDoc.Rows.Clear();
			}
			dtDoc.AcceptChanges();
			grdDoc.DataSource = dtDoc;
			grdDoc.RefreshDataSource();
			_opprId = -1;
			rdOppoStatus.SelectedIndex = 0;
			rdOppoStatus.Properties.Items[1].Enabled = false;
			rdOppoStatus.Properties.Items[2].Enabled = false;
			FormMode = Enums.FormViewModeType.View;
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				object focusedRowCellValue = gv_Doc.GetFocusedRowCellValue("OpprId");
				if (focusedRowCellValue != null && FormMode == Enums.FormViewModeType.View && _opprId != Function.ParseInt(focusedRowCellValue))
				{
					_opprId = Function.ParseInt(focusedRowCellValue);
					await LoadOneDoc(_opprId);
					rdOppoStatus.Properties.Items[1].Enabled = true;
					rdOppoStatus.Properties.Items[2].Enabled = true;
				}
				else
				{
					for (int j = 0; j < gv_Doc.DataRowCount; j++)
					{
						object rowCellValue = gv_Doc.GetRowCellValue(j, "OpprId");
						if (rowCellValue != null && Function.ParseInt(rowCellValue) == _opprId)
						{
							gv_Doc.FocusedRowHandle = j;
						}
					}
				}
			}
			else
			{
				Logging.Write("WATCH", "Opportunity-->Search(): No record found");
			}
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ShowHideStageCombo(string opportType)
	{
		try
		{
			DataTable dataTable = _dtListComBoStage.Copy();
			List<DataRow> list = new List<DataRow>();
			string columnName = ((!(opportType == "R")) ? "PurStage" : "SalesStage");
			foreach (DataRow row in dataTable.Rows)
			{
				if (Function.ToString(row[columnName]) != "Y" || (Function.ToString(row["Canceled"]) == "Y" && Function.ParseInt(row["Step_Id"]) != -10))
				{
					list.Add(row);
				}
			}
			foreach (DataRow item in list)
			{
				dataTable.Rows.Remove(item);
			}
			dataTable.AcceptChanges();
			object obj = (rILStage.DataSource = dataTable);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private bool CheckQuantity()
	{
		bool flag = true;
		try
		{
			for (int i = 0; i < gv_ItemDetail.DataRowCount; i++)
			{
				if (!string.IsNullOrWhiteSpace(Function.ToString(gv_ItemDetail.GetRowCellValue(i, "ItemCode"))) && Function.ParseDouble(gv_ItemDetail.GetRowCellValue(i, "Quantity")) <= 0.0)
				{
					flag = false;
					break;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			flag = false;
		}
		if (!flag)
		{
			WinCommons.Error(Enums.MessageCode.EnterQtyGreater0, "");
		}
		return flag;
	}

	private void SettingLanguage()
	{
		try
		{
			Text = Apzon.Commons.Properties.Resources.Titel_SalesOpportunity;
			btSearch.Text = Apzon.Commons.Properties.Resources.ButtonSearch;
			btCancel.Text = Apzon.Commons.Properties.Resources.ButtonCancel;
			btAdd.Text = "OK";
			lb_FromDate.Utext = Apzon.Commons.Properties.Resources.Doc_lb_FromDate;
			lb_ToDate.Utext = Apzon.Commons.Properties.Resources.Doc_lb_ToDate;
			lb_NoOfTrans.Utext = Apzon.Commons.Properties.Resources.Doc_lb_NoOfTrans;
			lb_BusinessPartner.Utext = Apzon.Commons.Properties.Resources.Doc_lb_BusinessPartner;
			lb_Stage.Utext = Apzon.Commons.Properties.Resources.lb_Stage;
			lb_OpporStatus.Utext = Apzon.Commons.Properties.Resources.lb_OpporStatus;
			lb_MainSalesEmp.Utext = Apzon.Commons.Properties.Resources.lb_MainSalesEmp;
			tab_Potential.Text = Apzon.Commons.Properties.Resources.tab_Potential;
			lb_PredictedClosingIn.Utext = Apzon.Commons.Properties.Resources.lb_PredictedClosingIn;
			lb_PredictedClosingDate.Utext = Apzon.Commons.Properties.Resources.lb_PredictedClosingDate;
			lb_PotentialAmount.Utext = Apzon.Commons.Properties.Resources.lb_PotentialAmount;
			lb_WeightedAmount.Utext = Apzon.Commons.Properties.Resources.lb_WeighAmount;
			lb_GrossProfit.Utext = Apzon.Commons.Properties.Resources.lb_GrossProfit;
			lb_GrossProfitTotal.Utext = Apzon.Commons.Properties.Resources.lb_GrossProfitTotal;
			tab_Stage.Text = Apzon.Commons.Properties.Resources.lb_Stage;
			gcStageStartDate.Caption = Apzon.Commons.Properties.Resources.lb_StartDate;
			gcStageClosingDate.Caption = Apzon.Commons.Properties.Resources.lb_ClosingDate;
			gcStageSalesEmp.Caption = Apzon.Commons.Properties.Resources.lb_SalesEmployee;
			gcStage.Caption = Apzon.Commons.Properties.Resources.lb_Stage;
			gcStagePrct.Caption = Apzon.Commons.Properties.Resources.lb_ClosePrcnt;
			gcStagePotentialAmount.Caption = Apzon.Commons.Properties.Resources.lb_PotentialAmount;
			gcStageWeightAmount.Caption = Apzon.Commons.Properties.Resources.lb_WeighAmount;
			gcStageShowBPDocs.Caption = Apzon.Commons.Properties.Resources.lb_ShowBPDocs;
			gcStageDocType.Caption = Apzon.Commons.Properties.Resources.lb_DocumentType;
			gcStageDocNo.Caption = Apzon.Commons.Properties.Resources.lb_DocumentNo;
			gcStageOwner.Caption = Apzon.Commons.Properties.Resources.lb_Owner;
			gcStageActivity.Caption = Apzon.Commons.Properties.Resources.lb_Activity;
			tab_ItemDetail.Text = Apzon.Commons.Properties.Resources.tab_ItemDetail;
			gcItemCode.Caption = Apzon.Commons.Properties.Resources.lb_ItemNo;
			gcItemDesc.Caption = Apzon.Commons.Properties.Resources.lb_Description;
			gcItemQty.Caption = Apzon.Commons.Properties.Resources.lb_Quantity;
			gcItemPrice.Caption = Apzon.Commons.Properties.Resources.lb_Price;
			gcItemPotentAmount.Caption = Apzon.Commons.Properties.Resources.lb_PotentialAmount;
			gcItemDeliveryDate.Caption = Apzon.Commons.Properties.Resources.lb_DeliveryDate;
			gcItemAttFile.Caption = Apzon.Commons.Properties.Resources.lb_AttFile;
			gcItemAttFullPath.Caption = Apzon.Commons.Properties.Resources.lb_AttFullPath;
			tab_Competitor.Text = Apzon.Commons.Properties.Resources.tab_Competitor;
			gcCompetitorName.Caption = Apzon.Commons.Properties.Resources.lb_Name;
			gcCompetitorThreatLevel.Caption = Apzon.Commons.Properties.Resources.lb_ThreatLevel;
			gcCompetitorRemark.Caption = Apzon.Commons.Properties.Resources.Doc_gvDoc_Remark;
			gcCompetitorWon.Caption = Apzon.Commons.Properties.Resources.lb_Won;
			tab_Partner.Text = Apzon.Commons.Properties.Resources.tab_Partner;
			gcPartnerName.Caption = Apzon.Commons.Properties.Resources.lb_Name;
			gcPartnerRelationShip.Caption = Apzon.Commons.Properties.Resources.lb_Relationship;
			gcPartnerRelatedBP.Caption = Apzon.Commons.Properties.Resources.lb_RelatedBP;
			gcPartnerRemark.Caption = Apzon.Commons.Properties.Resources.Doc_gvDoc_Remark;
			tab_Summary.Text = Apzon.Commons.Properties.Resources.tab_Summary;
			lb_OpportunityStatus.Utext = Apzon.Commons.Properties.Resources.lb_OpportunityStatus;
			lb_DocumentType.Utext = Apzon.Commons.Properties.Resources.lb_DocumentType;
			lb_DocumentNo.Utext = Apzon.Commons.Properties.Resources.lb_DocumentNo;
			gcReasonDesc.Caption = Apzon.Commons.Properties.Resources.lb_Description;
			gcOpporType.Caption = Apzon.Commons.Properties.Resources.lb_Type;
			gcOpporNo.Caption = Apzon.Commons.Properties.Resources.lb_OpporNo;
			gcOpporName.Caption = Apzon.Commons.Properties.Resources.lb_OpportunityName;
			gcOpporBusCode.Caption = Apzon.Commons.Properties.Resources.lb_BPCode;
			gcOpporBPName.Caption = Apzon.Commons.Properties.Resources.lb_BPName;
			gcOpporSalesEmp.Caption = Apzon.Commons.Properties.Resources.lb_SalesEmployee;
			gcOpporStatus.Caption = Apzon.Commons.Properties.Resources.lb_Status;
			gcOpporStartDate.Caption = Apzon.Commons.Properties.Resources.lb_StartDate;
			gcOpporClosingDate.Caption = Apzon.Commons.Properties.Resources.lb_ClosingDate;
			gcOpporClosing.Caption = Apzon.Commons.Properties.Resources.lb_ClosePrcnt;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void InitEventControls()
	{
		try
		{
			gv_ItemDetail.InitPasteFromExcelSaleOpportunity();
			string dateTimeFormat = GlobalData.DateTimeFormat;
			datePredCloDate.Properties.Mask.MaskType = MaskType.None;
			datePredCloDate.Properties.DisplayFormat.FormatType = FormatType.DateTime;
			datePredCloDate.Properties.DisplayFormat.FormatString = dateTimeFormat;
			datePredCloDate.Properties.EditFormat.FormatType = FormatType.DateTime;
			datePredCloDate.Properties.EditFormat.FormatString = dateTimeFormat;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadTableFormular()
	{
		try
		{
			TblFormular["@OPR62"] = gv_ItemDetail;
			TblFormular["OOPR"] = gv_Doc;
			TblFormular["OPR1"] = gv_Stage;
			TblFormular["OPR2"] = gv_Partner;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void SetDevControls()
	{
		try
		{
			bteCFL_ChnCrdCode.Properties.RefreshDataSource();
			RepositoryItemButtonEditCFL properties = bteCFL_ChnCrdCode.Properties;
			properties.HandleChooseItem = (Action<DataRow>)Delegate.Combine(properties.HandleChooseItem, new Action<DataRow>(HandleChooseChanelCardCode));
			bteCFL_PrjCode.Properties.RefreshDataSource();
			lukDN_Source.Properties.FormProperties.TableName = "OOSR";
			lukDN_Source.Properties.RefreshDataSource();
			lukDN_Industry.Properties.FormProperties.TableName = "OOND";
			lukDN_Industry.Properties.RefreshDataSource();
			if (!GlobalData.LoginSap)
			{
				return;
			}
			lukDN_Industry.Properties.BeforeShowForm = (Form frm) =>
			{
				if (frm is frm_DefineNew frm_DefineNew2)
				{
					frm_DefineNew2.Text = "Industry - Setup";
					frm_DefineNew2.UseFormSetting = true;
					frm_DefineNew2.CustomSave = (Func<DataTable, HttpResult>)Delegate.Combine(frm_DefineNew2.CustomSave, new Func<DataTable, HttpResult>(CustomSaveIndustry));
				}
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private HttpResult CustomSaveIndustry(DataTable dt)
	{
		try
		{
			string message = _httpClient.Industry.SaveDoc(dt);
			return new HttpResult(Enums.MessageCode.Success, message);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new HttpResult(Enums.MessageCode.Error, "Save Error!!");
	}

	private void HandleChooseChanelCardCode(DataRow obj)
	{
		try
		{
			if (obj != null)
			{
				bteCFL_ChnCrdCode.EditValue = Function.ToString(obj["CardCode"]);
				txt_ChnCrdName.Text = Function.ToString(obj["CardName"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task GetNewOpprId()
	{
		try
		{
			_newOpprId = await _httpClient.OpportunityClient.GetNewOpprId() + 1;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadOneDoc(int opprId)
	{
		_ = 2;
		try
		{
			_isLoading = true;
			_opprId = opprId;
			WinCommons._OpenProcessing();
			DataTable dt = await _httpClient.OpportunityClient.GetDoc(opprId, GlobalData.UserCodeLogin);
			if (dt != null && dt.Rows.Count > 0)
			{
				string text = Function.ToString(dt.Rows[0]["Status"]);
				string docType = Function.ToString(dt.Rows[0]["DocType"]);
				switch (text)
				{
				case "O":
					rdOppoStatus.SelectedIndex = 0;
					break;
				case "W":
					rdOppoStatus.SelectedIndex = 1;
					break;
				case "L":
					rdOppoStatus.SelectedIndex = 2;
					break;
				}
				cbSummaryDocType.SelectedIndex = _docTypes.FindIndex((string x) => x == docType);
				switch ((dt.Rows[0]["DifType"] == null) ? "" : dt.Rows[0]["DifType"].ToString())
				{
				case "M":
					cbClosingType.SelectedIndex = 0;
					break;
				case "W":
					cbClosingType.SelectedIndex = 1;
					break;
				case "D":
					cbClosingType.SelectedIndex = 2;
					break;
				}
				if (dt.Rows[0]["PredDate"] != null)
				{
					DateTime? dateTime = Function.ParseDateTime(dt.Rows[0]["PredDate"]);
					if (dateTime.HasValue)
					{
						datePredCloDate.EditValue = dateTime.Value.Date;
						DateTime dateTime2 = Convert.ToDateTime(dateTime);
						DateTime dateTime3 = Convert.ToDateTime(dt.Rows[0]["OpenDate"].ToString());
						switch (cbClosingType.SelectedIndex)
						{
						case 0:
						{
							YearsMonths yearsMonths = YearMonthDiff(dateTime3, dateTime2);
							txtPredictCloseIn.Text = ((yearsMonths == null) ? "" : (yearsMonths.TotalMonths - 1).ToString());
							break;
						}
						case 1:
							txtPredictCloseIn.Text = ((dateTime2 - dateTime3).TotalDays / 7.0).ToString();
							break;
						case 2:
							txtPredictCloseIn.Text = (dateTime2 - dateTime3).TotalDays.ToString();
							break;
						}
					}
				}
				else
				{
					datePredCloDate.EditValue = null;
					txtPredictCloseIn.Text = "";
				}
				_isLoading = true;
				calcGrossProfit.EditValue = Function.ParseDouble(dt.Rows[0]["PrcnProf"]);
				calcGrossProfitTotal.EditValue = Function.ParseDouble(dt.Rows[0]["SumProfL"]);
				calcWeightAmount.EditValue = Function.ParseDouble(dt.Rows[0]["WtSumLoc"]);
				calcPotentialAmount.EditValue = Function.ParseDouble(dt.Rows[0]["MaxSumLoc"]);
				txtDocNo.EditValue = 0;
				txtDocNo.EditValue = Function.ParseInt(dt.Rows[0]["DocNum"]);
				string text2 = ((dt.Rows[0]["DocType"] == null) ? "" : dt.Rows[0]["DocType"].ToString());
				if (!string.IsNullOrWhiteSpace(text2))
				{
					switch (text2)
					{
					case "23":
						cbSummaryDocType.SelectedIndex = 1;
						break;
					case "17":
						cbSummaryDocType.SelectedIndex = 2;
						break;
					case "15":
						cbSummaryDocType.SelectedIndex = 3;
						break;
					case "13":
						cbSummaryDocType.SelectedIndex = 4;
						break;
					case "540000006":
						cbSummaryDocType.SelectedIndex = 5;
						break;
					case "22":
						cbSummaryDocType.SelectedIndex = 6;
						break;
					case "20":
						cbSummaryDocType.SelectedIndex = 7;
						break;
					case "18":
						cbSummaryDocType.SelectedIndex = 8;
						break;
					default:
						cbSummaryDocType.SelectedIndex = 0;
						break;
					}
				}
				luk_LevelOfInterest.EditValue = Function.ParseInt(dt.Rows[0]["IntRate"]);
				LoadComboInterest();
				Task task = LoadStage(opprId);
				Task task2 = LoadCompetitor(opprId);
				Task task3 = LoadPartner(opprId);
				Task task4 = LoadReason(opprId);
				Task task5 = LoadItemDetail(opprId);
				Task task6 = LoadLinkedAct(opprId);
				Task task7 = LoadInterestRange(opprId);
				Task task8 = ucAttachment1.LoadAttachments(Function.ParseInt(dt.Rows[0]["AtcEntry"]));
				await Task.WhenAll(task, task2, task3, task4, task5, task6, task7, task8);
				AddNewRowPartner(clear: false);
				AddNewRowCompetition(clear: false);
				AddNewRowReason(clear: false);
				AddNewRowInterestRange();
				if (GlobalData.LoginSap)
				{
					await AddNewRowItem(clear: false);
				}
				bteCFL_ChnCrdCode.EditValue = Function.ToString(dt.Rows[0]["ChnCrdCode"]);
				txt_ChnCrdName.Text = Function.ToString(dt.Rows[0]["ChnCrdName"]);
				_isLoading = true;
				lukDN_ChnCrdCon.EditValue = Function.ParseInt(dt.Rows[0]["ChnCrdCon"]);
				bteCFL_PrjCode.EditValue = Function.ToString(dt.Rows[0]["PrjCode"]);
				lukDN_Source.EditValue = Function.ParseInt(dt.Rows[0]["Source"]);
				lukDN_Industry.EditValue = Function.ParseInt(dt.Rows[0]["Industry"]);
				memo_Memo.EditValue = Function.ToString(dt.Rows[0]["Memo"]);
				LoadComboBpChanelContact();
				_isLoading = false;
			}
			WinCommons._CloseProcessing();
			if (opprId == _opprId)
			{
				_isLoading = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		SetColumnsReadOnly();
	}

	private async Task LoadInterestRange(int opprId)
	{
		try
		{
			DataTable dataSource = await _httpClient.OpportunityClient.GetInterestRange(opprId);
			grd_InterestRange.DataSource = dataSource;
			grd_InterestRange.RefreshDataSource();
			AddNewRowInterestRange();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private YearsMonths YearMonthDiff(DateTime startDate, DateTime endDate)
	{
		try
		{
			int num = endDate.Year * 12 + endDate.Month - (startDate.Year * 12 + startDate.Month) + 1;
			int years = (int)Math.Floor((decimal)(num / 12));
			int months = num % 12;
			return new YearsMonths
			{
				TotalMonths = num,
				Years = years,
				Months = months
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return null;
		}
	}

	private async Task LoadStage(int opprId)
	{
		try
		{
			if (_dtSta.Columns.Count > 0 && opprId == -1)
			{
				_dtSta = WinFuntions.GetTableStructure("OPR1");
				_dtSta.PrimaryKey = new DataColumn[1] { _dtSta.Columns["Line"] };
				grdStage.DataSource = _dtSta;
				grdStage.RefreshDataSource();
				return;
			}
			DataTable dataTable = await _httpClient.OpportunityClient.GetStage(opprId);
			if (dataTable.Columns.Count > 0)
			{
				_dtSta = dataTable;
				_dtSta.PrimaryKey = new DataColumn[1] { _dtSta.Columns["Line"] };
				if (opprId == -1)
				{
					_dtSta.Rows.Clear();
				}
				grdStage.DataSource = _dtSta;
				grdStage.RefreshDataSource();
			}
			else
			{
				AddNewRowStage(clear: true);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadCompetitor(int opprId)
	{
		try
		{
			if (_dtCom.Columns.Count > 0 && opprId == -1)
			{
				grdCompetitor.DataSource = null;
				grdCompetitor.RefreshDataSource();
				return;
			}
			DataTable dataTable = await _httpClient.OpportunityClient.GetCompetitor(opprId);
			if (dataTable.Columns.Count > 0)
			{
				_dtCom = dataTable;
				if (opprId == -1)
				{
					_dtCom.Rows.Clear();
				}
				grdCompetitor.DataSource = _dtCom;
				grdCompetitor.RefreshDataSource();
			}
			else
			{
				AddNewRowCompetition(clear: true);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadPartner(int opprId)
	{
		try
		{
			if (_dtBus.Columns.Count > 0 && opprId == -1)
			{
				grdPartner.DataSource = null;
				grdPartner.RefreshDataSource();
				return;
			}
			DataTable dataTable = await _httpClient.OpportunityClient.GetPartner(opprId);
			if (dataTable.Columns.Count > 0)
			{
				_dtBus = dataTable;
				if (opprId == -1)
				{
					_dtBus.Rows.Clear();
				}
				grdPartner.DataSource = _dtBus;
				grdPartner.RefreshDataSource();
			}
			else
			{
				AddNewRowPartner(clear: true);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadReason(int opprId)
	{
		try
		{
			if (_dtRea.Columns.Count > 0 && opprId == -1)
			{
				grdReason.DataSource = null;
				grdReason.RefreshDataSource();
				return;
			}
			DataTable dataTable = await _httpClient.OpportunityClient.GetReason(opprId);
			if (dataTable.Columns.Count > 0)
			{
				_dtRea = dataTable;
				if (opprId == -1)
				{
					_dtRea.Rows.Clear();
				}
				grdReason.DataSource = _dtRea;
				grdReason.RefreshDataSource();
			}
			else
			{
				AddNewRowReason(clear: true);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadItemDetail(int opprId)
	{
		_ = 2;
		try
		{
			if (_dtIte.Columns.Count > 0 && opprId == -1)
			{
				grdItemDetail.DataSource = null;
				grdItemDetail.RefreshDataSource();
				return;
			}
			DataTable dataTable = await _httpClient.OpportunityClient.GetItemDetail(opprId);
			if (dataTable != null && dataTable.Columns.Count > 0)
			{
				_dtIte = dataTable;
			}
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				if (opprId == -1)
				{
					_dtIte.Rows.Clear();
				}
				await WinFuntions.ChangeTypeUdf(_dtIte, "@OPR62");
				grdItemDetail.DataSource = _dtIte;
				grdItemDetail.RefreshDataSource();
			}
			else if (GlobalData.LoginSap)
			{
				await AddNewRowItem(clear: false);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task RefreshData()
	{
		_ = 11;
		try
		{
			LoadCbDocType();
			_isLoading = true;
			FormMode = Enums.FormViewModeType.View;
			WinCommons._OpenProcessing();
			Task newOpprId = GetNewOpprId();
			Task T2 = LoadStage(-1);
			Task T3 = LoadCompetitor(-1);
			Task T4 = LoadPartner(-1);
			Task T5 = LoadReason(-1);
			Task T6 = LoadItemDetail(-1);
			Task T7 = LoadComboStage();
			Task T8 = LoadComboSalesEmp();
			Task T9 = LoadComboCompetitor();
			Task T10 = LoadComboPartner();
			Task T11 = LoadComboPartnerRelationship();
			Task T12 = LoadComboReason();
			WinCommons._CloseProcessing();
			if (_opprId > 0)
			{
				for (int i = 0; i < gv_Doc.RowCount; i++)
				{
					if (Function.ParseInt(gv_Doc.GetRowCellValue(i, "OpprId")) == _opprId)
					{
						gv_Doc.FocusedRowHandle = i;
					}
				}
			}
			if (gv_Doc.DataRowCount > 1)
			{
				_opprId = -1;
			}
			rdOppoStatus.SelectedIndex = 0;
			rdOppoStatus.Properties.Items[1].Enabled = false;
			rdOppoStatus.Properties.Items[2].Enabled = false;
			datePredCloDate.EditValue = "";
			cbClosingType.SelectedIndex = 2;
			calcGrossProfit.EditValue = 0;
			calcGrossProfitTotal.EditValue = 0;
			calcWeightAmount.EditValue = 0;
			calcPotentialAmount.EditValue = 0m;
			txtDocNo.EditValue = 0;
			txtPredictCloseIn.Text = "0";
			await newOpprId;
			await T2;
			await T3;
			await T4;
			await T5;
			await T6;
			await T7;
			await T8;
			await T9;
			await T10;
			await T11;
			await T12;
			_isLoading = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadCbDocType()
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("ObjType", typeof(string));
			dataTable.Columns.Add("Name", typeof(string));
			DataRow dataRow = dataTable.NewRow();
			dataRow["ObjType"] = 23.ToString();
			dataRow["Name"] = "Sale Quotations";
			dataTable.Rows.Add(dataRow);
			DataRow dataRow2 = dataTable.NewRow();
			dataRow2["ObjType"] = 17.ToString();
			dataRow2["Name"] = "Sales Orders";
			dataTable.Rows.Add(dataRow2);
			DataRow dataRow3 = dataTable.NewRow();
			dataRow3["ObjType"] = 15.ToString();
			dataRow3["Name"] = "Deliveries";
			dataTable.Rows.Add(dataRow3);
			DataRow dataRow4 = dataTable.NewRow();
			dataRow4["ObjType"] = 13.ToString();
			dataRow4["Name"] = "A/R Invoices";
			dataTable.Rows.Add(dataRow4);
			DataRow dataRow5 = dataTable.NewRow();
			dataRow5["ObjType"] = 540000006.ToString();
			dataRow5["Name"] = "Purchase Quotation";
			dataTable.Rows.Add(dataRow5);
			DataRow dataRow6 = dataTable.NewRow();
			dataRow6["ObjType"] = 22.ToString();
			dataRow6["Name"] = "Purchase Orders";
			dataTable.Rows.Add(dataRow6);
			DataRow dataRow7 = dataTable.NewRow();
			dataRow7["ObjType"] = 20.ToString();
			dataRow7["Name"] = "Goods Receipt PO";
			dataTable.Rows.Add(dataRow7);
			DataRow dataRow8 = dataTable.NewRow();
			dataRow8["ObjType"] = 18.ToString();
			dataRow8["Name"] = "A/P Invoices";
			dataTable.Rows.Add(dataRow8);
			dataTable.AcceptChanges();
			rep_stage_DocType.DataSource = dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadComboStage()
	{
		_ = 1;
		try
		{
			cbOpportStatus.Properties.DisplayMember = "Name";
			cbOpportStatus.Properties.ValueMember = "Code";
			cbOpportStatus.Properties.Columns.Clear();
			cbOpportStatus.Properties.Columns.Add(new LookUpColumnInfo("Name", ""));
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add(new DataColumn("Code", typeof(string)));
			dataTable.Columns.Add(new DataColumn("Name", typeof(string)));
			dataTable.AcceptChanges();
			DataRow dataRow = dataTable.NewRow();
			dataRow["Code"] = "";
			dataRow["Name"] = "";
			dataTable.Rows.Add(dataRow);
			DataRow dataRow2 = dataTable.NewRow();
			dataRow2["Code"] = "O";
			dataRow2["Name"] = "Open";
			dataTable.Rows.Add(dataRow2);
			DataRow dataRow3 = dataTable.NewRow();
			dataRow3["Code"] = "L";
			dataRow3["Name"] = "Lost";
			dataTable.Rows.Add(dataRow3);
			DataRow dataRow4 = dataTable.NewRow();
			dataRow4["Code"] = "W";
			dataRow4["Name"] = "Won";
			dataTable.Rows.Add(dataRow4);
			dataTable.AcceptChanges();
			cbOpportStatus.Properties.DataSource = dataTable;
			cbStage.Properties.DisplayMember = "Descript";
			cbStage.Properties.ValueMember = "Step_Id";
			cbStage.Properties.Columns.Clear();
			cbStage.Properties.Columns.Add(new LookUpColumnInfo("Descript", ""));
			DataTable dataTable2 = await _httpClient.OpportunityClient.GetListStage();
			DataTable dataTable3 = new DataTable();
			if (dataTable2 == null || dataTable2.Columns.Count == 0)
			{
				dataTable3.Columns.Add(new DataColumn("Num", typeof(int)));
				dataTable3.Columns.Add(new DataColumn("Descript", typeof(string)));
				dataTable3.Columns.Add(new DataColumn("Step_Id", typeof(int)));
				dataTable3.Columns.Add(new DataColumn("CloPrcnt", typeof(decimal)));
				dataTable3.Columns.Add(new DataColumn("Canceled", typeof(string)));
				dataTable3.Columns.Add(new DataColumn("UserSign", typeof(int)));
				dataTable3.Columns.Add(new DataColumn("SalesStage", typeof(string)));
				dataTable3.Columns.Add(new DataColumn("PurStage", typeof(string)));
				dataTable3.AcceptChanges();
			}
			else
			{
				dataTable3 = dataTable2.Clone();
				dataTable3.Columns["Step_Id"].DataType = typeof(int);
				for (int i = 0; i < dataTable2.Rows.Count; i++)
				{
					dataTable3.ImportRow(dataTable2.Rows[i]);
				}
				dataTable3.AcceptChanges();
			}
			DataTable dataTable4 = dataTable3.Copy();
			if (!dataTable4.AsEnumerable().Any((DataRow r) => Function.ParseInt(r["Step_Id"]) == 0))
			{
				DataRow dataRow5 = dataTable4.NewRow();
				dataRow5["Descript"] = "";
				dataRow5["Step_Id"] = 0;
				dataTable4.Rows.Add(dataRow5);
				dataTable4.AcceptChanges();
			}
			cbStage.Properties.DataSource = dataTable4;
			_dtListComBoStage = dataTable3.Copy();
			rILStage.ValueMember = "Step_Id";
			if (!_dtListComBoStage.AsEnumerable().Any((DataRow r) => Function.ParseInt(r["Step_Id"]) == -10))
			{
				DataRow dataRow6 = _dtListComBoStage.NewRow();
				dataRow6["Descript"] = "Define New";
				dataRow6["Step_Id"] = -10;
				_dtListComBoStage.Rows.Add(dataRow6);
				_dtListComBoStage.AcceptChanges();
			}
			rILStage.DataSource = _dtListComBoStage;
			DataTable dataSource = await _httpClient.OpportunityClient.GetOwnerForStage();
			rep_Stage_Owner.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadComboCompetitor()
	{
		try
		{
			DataTable dataTable = await _httpClient.OpportunityClient.GetListCompetitor();
			if (dataTable.Columns.Count == 0)
			{
				dataTable.Columns.Add(new DataColumn("CompetId", typeof(int)));
				dataTable.Columns.Add(new DataColumn("Name", typeof(string)));
				dataTable.Columns.Add(new DataColumn("ThreatLevl", typeof(string)));
				dataTable.Columns.Add(new DataColumn("Memo", typeof(string)));
				dataTable.Columns.Add(new DataColumn("DataSource", typeof(string)));
				dataTable.Columns.Add(new DataColumn("UserSign", typeof(int)));
			}
			DataRow dataRow = dataTable.NewRow();
			dataRow["CompetId"] = -1;
			dataTable.Rows.InsertAt(dataRow, 0);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				rILCompetitor.DataSource = dataTable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadComboSalesEmp()
	{
		try
		{
			cbMainSalesEmp.Properties.Columns.Clear();
			cbMainSalesEmp.Properties.Columns.Add(new LookUpColumnInfo("SlpName", ""));
			cbMainSalesEmp.Properties.ValueMember = "SlpCode";
			cbMainSalesEmp.Properties.DisplayMember = "SlpName";
			cbMainSalesEmp.Properties.NullText = "";
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "SaleEmployee");
			DataTable dataTable = await _helper.GetDataTableAsync("ShowSaleEmployee");
			DataRow dataRow = dataTable.NewRow();
			dataRow["SlpCode"] = 0;
			dataRow["SlpName"] = "";
			dataTable.Rows.InsertAt(dataRow, 0);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				rILSalesEmp.DataSource = dataTable;
				rILStaSalesEmpl.DataSource = dataTable;
				cbMainSalesEmp.Properties.DataSource = dataTable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadComboPartner()
	{
		try
		{
			DataTable dataTable = await _httpClient.OpportunityClient.GetListPartner();
			if (dataTable.Columns.Count == 0)
			{
				dataTable.Columns.Add(new DataColumn("PrtId", typeof(int)));
				dataTable.Columns.Add(new DataColumn("Name", typeof(string)));
				dataTable.Columns.Add(new DataColumn("RelatnType", typeof(string)));
				dataTable.Columns.Add(new DataColumn("RelatCard", typeof(string)));
				dataTable.Columns.Add(new DataColumn("Memo", typeof(string)));
				dataTable.Columns.Add(new DataColumn("DataSource", typeof(string)));
				dataTable.Columns.Add(new DataColumn("UserSign", typeof(int)));
				dataTable.Columns.Add(new DataColumn("DefaultORL", typeof(int)));
			}
			DataRow dataRow = dataTable.NewRow();
			dataRow["PrtId"] = -1;
			dataRow["Name"] = "";
			dataTable.Rows.InsertAt(dataRow, 0);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				rILPartner.DataSource = dataTable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadComboPartnerRelationship()
	{
		try
		{
			DataTable dataTable = await _httpClient.OpportunityClient.GetListPartnerRelation();
			if (dataTable.Columns.Count == 0)
			{
				dataTable.Columns.Add(new DataColumn("OrlCode", typeof(int)));
				dataTable.Columns.Add(new DataColumn("OrlDesc", typeof(string)));
			}
			DataRow dataRow = dataTable.NewRow();
			dataRow["OrlCode"] = -1;
			dataRow["OrlDesc"] = "";
			dataTable.Rows.InsertAt(dataRow, 0);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				rILPartnerRela.DataSource = dataTable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadComboReason()
	{
		try
		{
			DataTable dataTable = await _httpClient.OpportunityClient.GetListReason();
			if (dataTable.Columns.Count == 0)
			{
				dataTable.Columns.Add(new DataColumn("Num", typeof(int)));
				dataTable.Columns.Add(new DataColumn("Descript", typeof(string)));
				dataTable.Columns.Add(new DataColumn("SortOrder", typeof(int)));
				dataTable.Columns.Add(new DataColumn("UserSign", typeof(int)));
			}
			DataRow dataRow = dataTable.NewRow();
			dataRow["Num"] = -1;
			dataRow["Descript"] = "";
			dataTable.Rows.InsertAt(dataRow, 0);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				rILReason.DataSource = dataTable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadLinkedAct(int oprId)
	{
		try
		{
			_dtLinkedAct = await _httpClient.OpportunityClient.GetLinkedActivities(oprId);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadReasonGridNumber()
	{
		try
		{
			GridColumn gridColumn = gv_Reason.Columns.Add();
			gridColumn.FieldName = "counter";
			gridColumn.Caption = "#";
			gridColumn.Visible = true;
			gridColumn.VisibleIndex = 0;
			gridColumn.OptionsColumn.AllowEdit = false;
			gridColumn.UnboundType = UnboundColumnType.Integer;
			gv_Reason.CustomUnboundColumnData += gvReason_CustomUnboundColumnData;
			gv_Reason.Columns["ReasondId"].VisibleIndex = 1;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ErrorOnStageDate()
	{
		try
		{
			gv_Stage.FocusedRowHandle = _staErrRow;
			gv_Stage.FocusedColumn = gv_Stage.Columns[_staErrCol];
			gv_Stage.ShowEditor();
			WinCommons.Error(Enums.MessageCode.None, _staErrorString);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private bool CheckDocCloseDate()
	{
		try
		{
			if (((gv_Doc.GetFocusedRowCellValue("Status") == null) ? "" : gv_Doc.GetFocusedRowCellValue("Status").ToString()) != "O")
			{
				if (gv_Doc.GetFocusedRowCellValue("CloseDate") == DBNull.Value)
				{
					WinCommons.Error(Enums.MessageCode.CloseDateCannotNullWhenDocumentClose, "");
					gv_Doc.FocusedColumn = gv_Doc.Columns["CloseDate"];
					gv_Doc.ShowEditor();
					return false;
				}
				DateTime dateTime = ((gv_Doc.GetFocusedRowCellValue("OpenDate") == DBNull.Value) ? DateTime.Today : Convert.ToDateTime(gv_Doc.GetFocusedRowCellValue("OpenDate")));
				if (((gv_Doc.GetFocusedRowCellValue("CloseDate") == DBNull.Value) ? DateTime.Today : Convert.ToDateTime(gv_Doc.GetFocusedRowCellValue("CloseDate"))) < dateTime)
				{
					WinCommons.Error(Enums.MessageCode.CloseDateCannotBeBeforeStartDate, "");
					gv_Doc.FocusedColumn = gv_Doc.Columns["CloseDate"];
					gv_Doc.ShowEditor();
					return false;
				}
			}
			return true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return false;
		}
	}

	private async Task<string> CheckItems()
	{
		try
		{
			for (int i = 0; i < gv_ItemDetail.DataRowCount; i++)
			{
				string itCode = Function.ToString(gv_ItemDetail.GetRowCellValue(i, "ItemCode"));
				if (itCode != null && !string.IsNullOrWhiteSpace(itCode) && !(await ExtenseionMethods.CheckExistItem(itCode)))
				{
					return itCode;
				}
			}
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return "";
		}
	}

	private async Task<string> CheckCardCodes()
	{
		try
		{
			for (int i = 0; i < gv_Doc.DataRowCount; i++)
			{
				if (gv_Doc.GetRowCellValue(i, "CardCode") != null)
				{
					string cardCode = Function.ToString(gv_Doc.GetRowCellValue(i, "CardCode"));
					DataTable dataTable = await ExtenseionMethods.CheckExistCardCode(cardCode);
					if (dataTable == null || dataTable.Rows.Count <= 0)
					{
						return cardCode;
					}
				}
			}
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return "";
		}
	}

	private void gv_Doc_KeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.Tab)
			{
				string cardCode = Function.ToString(gv_Doc.GetFocusedRowCellValue("CardCode")).Trim();
				if (!_httpClient.OpportunityClient.SearchBPData(cardCode).IsNotNull())
				{
					ChooseFromListBp(sender, cardCode);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task<bool> ExistFileInDoc(int oprId, string fileName)
	{
		bool ret = false;
		try
		{
			DataTable dataTable = await new CommonsClient().ExistFileInDocOpportunity(oprId, fileName);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				ret = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return ret;
	}

	private void SetGridColumnReadOnly(GridView[] gvs, bool readOnly)
	{
		try
		{
			for (int i = 0; i < gvs.Length; i++)
			{
				for (int j = 0; j < gvs[i].Columns.Count; j++)
				{
					gvs[i].Columns[j].OptionsColumn.ReadOnly = readOnly;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void SetStageWeightAmount(decimal potentAmount)
	{
		try
		{
			if (grdStage.DataSource is DataTable dataTable)
			{
				dataTable.PrimaryKey = new DataColumn[1] { dataTable.Columns["Line"] };
				int num = ((gv_Stage.GetFocusedRowCellValue("Line") != null) ? Function.ParseInt(gv_Stage.GetFocusedRowCellValue("Line")) : 0);
				DataRow dataRow = dataTable.Rows.Find(num);
				decimal num2 = ((gv_Stage.GetFocusedRowCellValue("ClosePrcnt") == null) ? 0m : Function.ParseDecimal(gv_Stage.GetFocusedRowCellValue("ClosePrcnt")));
				decimal num3 = potentAmount * num2 / 100m;
				dataRow["WtSumLoc"] = num3;
				dataTable.AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private decimal GetStageWeightAmount(decimal potentAmount)
	{
		try
		{
			decimal num = ((gv_Stage.GetFocusedRowCellValue("ClosePrcnt") == null) ? 0m : Function.ParseDecimal(gv_Stage.GetFocusedRowCellValue("ClosePrcnt")));
			return potentAmount * num / 100m;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return 0m;
		}
	}

	private void SetStagePotentialAmount(decimal potentAmount)
	{
		try
		{
			if (grdStage.DataSource is DataTable dataTable)
			{
				dataTable.PrimaryKey = new DataColumn[1] { dataTable.Columns["Line"] };
				int num = ((gv_Stage.GetFocusedRowCellValue("Line") != null) ? Function.ParseInt(gv_Stage.GetFocusedRowCellValue("Line")) : 0);
				dataTable.Rows.Find(num)["MaxSumLoc"] = potentAmount;
				dataTable.AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void cbSummaryDocType_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			_dtSta = grdStage.DatasourceTable();
			if (_dtSta != null && _dtSta.Rows.Count > 0)
			{
				switch (cbSummaryDocType.Text)
				{
				case "Sale Quotations":
					_dtSta.Rows[_dtSta.Rows.Count - 1]["ObjType"] = "23";
					break;
				case "Sales Orders":
					_dtSta.Rows[_dtSta.Rows.Count - 1]["ObjType"] = "17";
					break;
				case "Deliveries":
					_dtSta.Rows[_dtSta.Rows.Count - 1]["ObjType"] = "15";
					break;
				case "A/R Invoices":
					_dtSta.Rows[_dtSta.Rows.Count - 1]["ObjType"] = "13";
					break;
				case "Purchase Quotation":
					_dtSta.Rows[_dtSta.Rows.Count - 1]["ObjType"] = "540000006";
					break;
				case "Purchase Orders":
					_dtSta.Rows[_dtSta.Rows.Count - 1]["ObjType"] = "22";
					break;
				case "Goods Receipt PO":
					_dtSta.Rows[_dtSta.Rows.Count - 1]["ObjType"] = "20";
					break;
				case "A/P Invoices":
					_dtSta.Rows[_dtSta.Rows.Count - 1]["ObjType"] = "18";
					break;
				}
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void txtDocNo_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			_dtSta = grdStage.DatasourceTable();
			if (_dtSta != null && _dtSta.Rows.Count > 0)
			{
				_dtSta.PrimaryKey = new DataColumn[1] { _dtSta.Columns["Line"] };
				_dtSta.Rows[_dtSta.Rows.Count - 1]["DocNumber"] = e.NewValue;
				_dtSta.Rows[_dtSta.Rows.Count - 1]["DocId"] = e.NewValue;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ctm_Doc_Opening(object sender, CancelEventArgs e)
	{
		try
		{
			deleteDocumentToolStripMenuItem.Visible = Function.ToString(gv_Doc.GetFocusedRowCellValue("Status")).Equals("O");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ChangeButtonOkText(object sender)
	{
		try
		{
			DataTable dataTable = grdDoc.DataSource as DataTable;
			if (FormMode == Enums.FormViewModeType.View && !_isLoading && _opprId != -1 && dataTable != null && dataTable.Rows.Count > 0)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void SetPredictDate()
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				return;
			}
			string text = ((txtPredictCloseIn.EditValue == null) ? "" : txtPredictCloseIn.EditValue.ToString());
			if (string.IsNullOrWhiteSpace(text) || !int.TryParse(text, out var result))
			{
				return;
			}
			DateTime? dateTime = Function.ParseDateTime(gv_Doc.GetFocusedRowCellValue("OpenDate"));
			if (dateTime.HasValue)
			{
				DateTime? dateTime2 = dateTime;
				switch (cbClosingType.SelectedIndex)
				{
				case 0:
					dateTime2 = dateTime.Value.AddMonths(result);
					break;
				case 1:
					dateTime2 = dateTime.Value.AddDays(result * 7);
					break;
				case 2:
					dateTime2 = dateTime.Value.AddDays(result);
					break;
				}
				datePredCloDate.DateTime = dateTime2.Value;
				if (!CheckPredictDate(dateTime2.Value))
				{
					WinCommons.Error(Enums.MessageCode.PredicClosingDateMustLaterOrEqualStageClosingDate, "");
					txtPredictCloseIn.Focus();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private bool CheckPredictDate(DateTime predictDate)
	{
		try
		{
			DateTime? dateTime = Function.ParseDateTime(gv_Doc.GetFocusedRowCellValue("OpenDate"));
			if (dateTime.HasValue && dateTime.Value.Date > predictDate.Date)
			{
				return false;
			}
			return true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return false;
		}
	}

	private void LoadComboBpChanelContact(bool setDefault = false)
	{
		try
		{
			string cardCode = Function.ToString(bteCFL_ChnCrdCode.EditValue);
			DataTable bPChanelContact = _httpClient.OpportunityClient.GetBPChanelContact(cardCode);
			lukDN_ChnCrdCon.Properties.DataSource = bPChanelContact;
			if ((bPChanelContact.IsNotNull() & setDefault) && !_isLoading)
			{
				DataRow dataRow = bPChanelContact.AsEnumerable().FirstOrDefault((DataRow r) => Function.ToString(r["DfltCode"]) == "Y");
				if (dataRow != null)
				{
					lukDN_ChnCrdCon.EditValue = Function.ParseInt(dataRow["CntctCode"]);
				}
			}
			if (!bPChanelContact.IsNotNull())
			{
				lukDN_ChnCrdCon.EditValue = null;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadComboInterest()
	{
		try
		{
			DataTable dataTable = _httpClient.OpportunityClient.GetComboInterest();
			if (dataTable == null || dataTable.Rows.Count == 0)
			{
				dataTable = new DataTable();
				dataTable.Columns.AddRange(new DataColumn[4]
				{
					new DataColumn("Num", typeof(int)),
					new DataColumn("Descript", typeof(string)),
					new DataColumn("SortOrder", typeof(short)),
					new DataColumn("UserSign", typeof(short))
				});
				dataTable.AcceptChanges();
			}
			if (!dataTable.AsEnumerable().Any((DataRow r) => Function.ParseInt(r["Num"]) == -10))
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["Num"] = -10;
				dataRow["Descript"] = "Define New";
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
			}
			if (!dataTable.AsEnumerable().Any((DataRow r) => r["Num"] == DBNull.Value))
			{
				DataRow dataRow2 = dataTable.NewRow();
				dataRow2["Num"] = DBNull.Value;
				dataRow2["Descript"] = "";
				dataTable.Rows.InsertAt(dataRow2, 0);
				dataTable.AcceptChanges();
			}
			rep_luk_IntrestRange_IntId.DataSource = dataTable;
			DataTable dataTable2 = _httpClient.OpportunityClient.GetComboInterestLevel();
			if (dataTable2 == null || dataTable2.Rows.Count == 0)
			{
				dataTable2 = new DataTable();
				dataTable2.Columns.AddRange(new DataColumn[4]
				{
					new DataColumn("Num", typeof(int)),
					new DataColumn("Descript", typeof(string)),
					new DataColumn("SortOrder", typeof(short)),
					new DataColumn("UserSign", typeof(short))
				});
				dataTable2.AcceptChanges();
			}
			if (!dataTable2.AsEnumerable().Any((DataRow r) => Function.ParseInt(r["Num"]) == -10))
			{
				DataRow dataRow3 = dataTable2.NewRow();
				dataRow3["Num"] = -10;
				dataRow3["Descript"] = "Define New";
				dataTable2.Rows.Add(dataRow3);
				dataTable2.AcceptChanges();
			}
			luk_LevelOfInterest.Properties.DataSource = dataTable2;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task GetRecord(string position, bool refresh = false)
	{
		if (_isLoading)
		{
			return;
		}
		try
		{
			_isLoading = true;
			bool flag = true;
			if (FormMode == Enums.FormViewModeType.Update && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				flag = false;
			}
			if (flag)
			{
				int opprId = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("OpprId"));
				string userCodeLogin = GlobalData.UserCodeLogin;
				int defaultBranch = GlobalData.DefaultBranch;
				DataTable dataTable = await _httpClient.OpportunityClient.GetRecord(position, opprId, userCodeLogin, defaultBranch);
				if (dataTable != null && dataTable.Rows.Count > 0)
				{
					DataTable searchDataTable = GetSearchDataTable();
					DataRow dataRow = searchDataTable.NewRow();
					dataRow["UserName"] = GlobalData.UserCodeLogin;
					if (refresh)
					{
						dataRow["NoTrans"] = opprId;
					}
					else
					{
						dataRow["NoTrans"] = Function.ParseInt(dataTable.Rows[0][0]);
						_loadDataBy = "N";
					}
					searchDataTable.Rows.Add(dataRow);
					searchDataTable.AcceptChanges();
					await Search(searchDataTable);
					ShowHideStageCombo(Function.ToString(gv_Doc.GetFocusedRowCellValue("OpprType")));
					SetColumnsReadOnly();
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.NoMatchingRecordFound, "");
				}
			}
			_isLoading = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			_isLoading = false;
		}
	}

	private DataTable GetSearchDataTable()
	{
		DataTable dataTable = new DataTable();
		try
		{
			dataTable.Columns.Add("FromDate");
			dataTable.Columns.Add("ToDate");
			dataTable.Columns.Add("NoTrans");
			dataTable.Columns.Add("BusPartner");
			dataTable.Columns.Add("Status");
			dataTable.Columns.Add("Stage");
			dataTable.Columns.Add("SaleEmp");
			dataTable.Columns.Add("UserName");
			dataTable.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return dataTable;
	}

	private void AddNewRowDoc()
	{
		try
		{
			if (dtDoc.Columns.Count < 10)
			{
				dtDoc = WinFuntions.GetTableStructure("OOPR");
				dtDoc.Columns.Add("TotalInvc", typeof(double));
			}
			_isLoading = true;
			dtDoc.SetDefaultAndMaxLen(gv_Doc.ItemId);
			DataRow dataRow = dtDoc.NewRow();
			dtDoc.Rows.Clear();
			dataRow["OpprId"] = _newOpprId;
			dataRow["OpprType"] = "R";
			dataRow["Status"] = "O";
			dataRow["OpenDate"] = DateTime.Today;
			dataRow["CloseDate"] = DBNull.Value;
			dataRow["SlpCode"] = -1;
			dataRow["MaxSumLoc"] = 1;
			dataRow["CloPrcnt"] = 1;
			dataRow["DocType"] = -1;
			dtDoc.Rows.Add(dataRow);
			dtDoc.AcceptChanges();
			grdDoc.DataSource = dtDoc;
			grdDoc.RefreshDataSource();
			_opprId = _newOpprId;
			_isLoading = false;
			if (!dtDoc.PrimaryKey.Any())
			{
				dtDoc.PrimaryKey = new DataColumn[1] { dtDoc.Columns["OpprId"] };
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void AddNewRowStage(bool clear)
	{
		try
		{
			if (_dtSta.Rows.Count > 0 && !clear)
			{
				CheckStageDateTime(checkCloseDateNull: true);
			}
			_isLoading = true;
			if (_staErrRow != -1)
			{
				ErrorOnStageDate();
				return;
			}
			ChangeButtonOkText(null);
			if (clear)
			{
				_dtSta.Rows.Clear();
			}
			DateTime dateTime = default;
			DateTime dateTime2 = DateTime.Today;
			if (_dtSta.Rows.Count > 0 && _dtSta.Rows[_dtSta.Rows.Count - 1] != null)
			{
				try
				{
					if (Function.ParseDateTime(_dtSta.Rows[_dtSta.Rows.Count - 1]["CloseDate"]).HasValue)
					{
						dateTime = Convert.ToDateTime(_dtSta.Rows[_dtSta.Rows.Count - 1]["CloseDate"]);
					}
				}
				catch
				{
					dateTime = new DateTime(1990, 1, 1);
				}
				if (dateTime == new DateTime(1990, 1, 1))
				{
					_staErrRow = _dtSta.Rows.Count - 1;
					_staErrCol = "CloseDate";
					_staErrorString = "Stage closing date can not null";
					ErrorOnStageDate();
					return;
				}
				if (Function.ParseDateTime(_dtSta.Rows[_dtSta.Rows.Count - 1]["CloseDate"]).HasValue)
				{
					dateTime2 = Convert.ToDateTime(_dtSta.Rows[_dtSta.Rows.Count - 1]["CloseDate"]);
				}
			}
			else
			{
				if (Function.ParseDateTime(gv_Doc.GetFocusedRowCellValue("OpenDate")).HasValue)
				{
					dateTime2 = Convert.ToDateTime(gv_Doc.GetFocusedRowCellValue("OpenDate").ToString());
				}
				dateTime2 = dateTime2.AddDays(-1.0);
			}
			DataRow dataRow = _dtSta.NewRow();
			dataRow["OpprId"] = _opprId;
			dataRow["SlpCode"] = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("SlpCode"));
			DataTable dataTable = rILStage.DataSource as DataTable;
			int dataSourceRowIndex = gv_Doc.GetDataSourceRowIndex(gv_Doc.FocusedRowHandle);
			if (dataTable != null && dataSourceRowIndex > 0)
			{
				dataTable.PrimaryKey = new DataColumn[1] { dataTable.Columns["Step_Id"] };
				int num = Function.ParseInt(dataTable.Rows[0]["Step_Id"]);
				dataRow["Step_Id"] = num;
				DataRow dataRow2 = dataTable.Rows.Find(num);
				dataRow["ClosePrcnt"] = dataRow2["CloPrcnt"];
				dtDoc.Rows[dataSourceRowIndex]["CloPrcnt"] = dataRow2["CloPrcnt"];
				dtDoc.AcceptChanges();
			}
			dataRow["OpenDate"] = dateTime2.AddDays(1.0);
			dataRow["CloseDate"] = DBNull.Value;
			if (_dtSta.Rows.Count > 0)
			{
				dataRow["MaxSumLoc"] = _dtSta.Rows[_dtSta.Rows.Count - 1]["MaxSumLoc"];
			}
			else
			{
				dataRow["MaxSumLoc"] = 0;
			}
			dataRow["WtSumLoc"] = Function.ParseDecimal(dataRow["MaxSumLoc"]) * Function.ParseDecimal(dataRow["ClosePrcnt"]) / 100m;
			calcWeightAmount.EditValue = Function.ParseInt(dataRow["WtSumLoc"]).ToString();
			calcPotentialAmount.EditValue = Function.ParseInt(dataRow["MaxSumLoc"]).ToString();
			calcGrossProfitTotal.EditValue = Function.ParseInt(dataRow["MaxSumLoc"]) * Function.ParseInt(calcGrossProfit.EditValue) / 100;
			dataRow["DocChkbox"] = "N";
			dataRow["Owner"] = 0;
			dataRow["Linked"] = "";
			dataRow["Line"] = GetLineNum(_dtSta, null) + 1;
			_dtSta.Rows.Add(dataRow);
			_dtSta.AcceptChanges();
			_dtSta.PrimaryKey = null;
			_dtSta.Columns["Line"].Unique = false;
			grdStage.DataSource = _dtSta;
			grdStage.RefreshDataSource();
			_isLoading = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void CheckStageDateTime(bool checkCloseDateNull = false)
	{
		try
		{
			DataRow dataRow = _dtSta.Rows[_dtSta.Rows.Count - 1];
			object obj = dataRow["OpenDate"];
			object obj2 = dataRow["CloseDate"];
			if (obj == null || string.IsNullOrWhiteSpace(obj.ToString()))
			{
				_staErrCol = "OpenDate";
				_staErrRow = _dtSta.Rows.Count - 1;
				_staErrorString = Apzon.Commons.Properties.Resources.Msg_StartDateIsNull;
				return;
			}
			if (obj2 == null || obj2 == DBNull.Value || string.IsNullOrWhiteSpace(obj2.ToString()))
			{
				if (checkCloseDateNull)
				{
					_staErrCol = "CloseDate";
					_staErrRow = _dtSta.Rows.Count - 1;
					_staErrorString = Apzon.Commons.Properties.Resources.Msg_CloseDateIsNull;
				}
				else
				{
					_staErrRow = -1;
				}
				return;
			}
			DateTime? dateTime = Function.ParseDateTime(obj);
			DateTime? dateTime2 = Function.ParseDateTime(obj2);
			if (!dateTime.HasValue)
			{
				_staErrCol = "OpenDate";
				_staErrRow = _dtSta.Rows.Count - 1;
				_staErrorString = Apzon.Commons.Properties.Resources.Msg_StartDateIsNull;
			}
			else if (!dateTime2.HasValue)
			{
				if (checkCloseDateNull)
				{
					_staErrCol = "CloseDate";
					_staErrRow = _dtSta.Rows.Count - 1;
					_staErrorString = Apzon.Commons.Properties.Resources.Msg_CloseDateIsNull;
				}
				else
				{
					_staErrRow = -1;
				}
			}
			else if (dateTime.Value > dateTime2.Value)
			{
				_staErrCol = "CloseDate";
				_staErrRow = _dtSta.Rows.Count - 1;
				_staErrorString = "Close date can not before start date";
			}
			else
			{
				_staErrRow = -1;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void AddNewRowPartner(bool clear)
	{
		try
		{
			if (clear)
			{
				_dtBus.Rows.Clear();
			}
			bool flag = false;
			foreach (DataRow row in _dtBus.Rows)
			{
				if (row["ParterId"] != null)
				{
					if (Function.ParseInt(row["ParterId"]) == -1)
					{
						flag = true;
					}
				}
				else
				{
					flag = true;
				}
			}
			if (!flag)
			{
				DataRow dataRow2 = _dtBus.NewRow();
				_dtBus.Rows.Add(dataRow2);
				dataRow2["OpportId"] = _opprId;
				dataRow2["Line"] = GetLineNum(_dtBus, null);
				dataRow2["ParterId"] = -1;
			}
			grdPartner.DataSource = _dtBus;
			grdPartner.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void AddNewRowCompetition(bool clear)
	{
		try
		{
			if (clear)
			{
				_dtCom.Rows.Clear();
			}
			bool flag = false;
			foreach (DataRow row in _dtCom.Rows)
			{
				if (row["CompetId"] != null)
				{
					if (Function.ParseInt(row["CompetId"]) == -1)
					{
						flag = true;
					}
				}
				else
				{
					flag = true;
				}
			}
			if (!flag)
			{
				DataRow dataRow2 = _dtCom.NewRow();
				_dtCom.Rows.Add(dataRow2);
				dataRow2["OpportId"] = _opprId;
				dataRow2["Memo"] = "";
				dataRow2["ThreatLevl"] = Function.ParseShort(1);
				dataRow2["CompetId"] = -1;
				dataRow2["Won"] = "N";
				dataRow2["Line"] = GetLineNum(_dtCom, null);
			}
			grdCompetitor.DataSource = _dtCom;
			grdCompetitor.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void AddNewRowInterestRange(bool clear = false)
	{
		try
		{
			DataTable dataTable = grd_InterestRange.DataSource as DataTable;
			if (dataTable == null || dataTable.Columns.Count == 0)
			{
				dataTable = WinFuntions.GetTableStructure("OPR4");
			}
			if (clear)
			{
				dataTable.Rows.Clear();
				dataTable.AcceptChanges();
			}
			if (!dataTable.AsEnumerable().Any((DataRow r) => r["IntId"] == DBNull.Value))
			{
				DataRow dataRow = dataTable.NewRow();
				dataTable.Rows.Add(dataRow);
				dataRow["OprId"] = _opprId;
				dataRow["Prmry"] = "N";
				dataRow["Line"] = GetLineNum(dataTable, null);
				dataRow["IntId"] = DBNull.Value;
				dataTable.AcceptChanges();
				grd_InterestRange.DataSource = dataTable;
				grd_InterestRange.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void AddNewRowReason(bool clear)
	{
		try
		{
			if (clear)
			{
				_dtRea.Rows.Clear();
			}
			bool flag = false;
			foreach (DataRow row in _dtRea.Rows)
			{
				if (row["ReasondId"] != null)
				{
					if (Function.ParseInt(row["ReasondId"]) == -1)
					{
						flag = true;
					}
				}
				else
				{
					flag = true;
				}
			}
			if (!flag)
			{
				if (_dtRea.Columns.Count < 3)
				{
					_dtRea.Columns.Add(new DataColumn("OpportId", typeof(int)));
					_dtRea.Columns.Add(new DataColumn("Line", typeof(int)));
					_dtRea.Columns.Add(new DataColumn("ReasondId", typeof(int)));
				}
				DataRow dataRow2 = _dtRea.NewRow();
				_dtRea.Rows.Add(dataRow2);
				dataRow2["OpportId"] = _opprId;
				dataRow2["Line"] = GetLineNum(_dtRea, null);
				dataRow2["ReasondId"] = -1;
			}
			grdReason.DataSource = _dtRea;
			grdReason.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task AddNewRowItem(bool clear)
	{
		try
		{
			if (clear)
			{
				_dtIte.Rows.Clear();
			}
			if (!_dtIte.IsNotNull() || _dtIte.AsEnumerable().All((DataRow t) => !string.IsNullOrEmpty(Function.ToString(t["ItemCode"]))))
			{
				DataRow dataRow = _dtIte.NewRow();
				if (_dtIte.Columns.Count < 10)
				{
					return;
				}
				dataRow["OpportId"] = _opprId;
				object focusedRowCellValue = gv_Doc.GetFocusedRowCellValue("U_ItemDetail");
				dataRow["DocEntry"] = ((focusedRowCellValue == null || focusedRowCellValue == DBNull.Value || string.IsNullOrWhiteSpace(focusedRowCellValue.ToString())) ? (-1) : Function.ParseInt(focusedRowCellValue));
				dataRow["Line"] = GetLineNum(_dtIte, "ItemCode") + 1;
				dataRow["ItemCode"] = "";
				dataRow["ItemName"] = "";
				dataRow["Quantity"] = 0;
				dataRow["Price"] = 0;
				dataRow["FrgnName"] = "";
				dataRow["DeliveryDate"] = Function.ParseDateTimes(datePredCloDate.EditValue);
				dataRow["IPercent"] = 0;
				_dtIte.Rows.Add(dataRow);
			}
			await WinFuntions.ChangeTypeUdf(_dtIte, "@OPR62");
			grdItemDetail.DataSource = _dtIte;
			grdItemDetail.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private int GetLineNum(DataTable dt, string fieldNotNull)
	{
		try
		{
			if (dt.IsNotNull())
			{
				if (string.IsNullOrEmpty(fieldNotNull))
				{
					return dt.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["Line"]));
				}
				return (from t in dt.AsEnumerable()
					where !string.IsNullOrEmpty(Function.ToString(t[fieldNotNull]))
					select t).Max((DataRow t) => Function.ParseInt(t["Line"]));
			}
			return 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return 0;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.SalesOppotunity.frm_SalesOpportunity));
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions7 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions8 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions9 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions10 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance11 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered11 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed11 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled11 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions11 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance12 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered12 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed12 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled12 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance13 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered13 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed13 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled13 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions12 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance14 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered14 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed14 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled14 = new DevExpress.Utils.SerializableAppearanceObject();
		this.repositoryItemCalcEditEx1 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.lb_OpportunityStatus = new Apzon.Controls.Controls.LabelLine();
		this.lb_ToDate = new Apzon.Controls.Controls.LabelLine();
		this.lb_NoOfTrans = new Apzon.Controls.Controls.LabelLine();
		this.lb_BusinessPartner = new Apzon.Controls.Controls.LabelLine();
		this.lb_MainSalesEmp = new Apzon.Controls.Controls.LabelLine();
		this.lb_Stage = new Apzon.Controls.Controls.LabelLine();
		this.lb_OpporStatus = new Apzon.Controls.Controls.LabelLine();
		this.btEditBusPartner = new Apzon.Controls.Editor.ButtonEditEx();
		this.dateFrom = new Apzon.Controls.Editor.DateEditEx();
		this.dateTo = new Apzon.Controls.Editor.DateEditEx();
		this.btSearch = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grdDoc = new Apzon.Controls.Grid.GridControlEx();
		this.ctm_Doc = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addNewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteDocumentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.gcOpporType = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Luk_OOPR_OpprType = new Apzon.Forms.UserControls.RepositoryItemLookUpEditCombo();
		this.gcOpporNo = new Apzon.Controls.Grid.GridColumnEx();
		this.gcOpporName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcOpporBusCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rIBBussPartner = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcOpporBPName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcOpporSalesEmp = new Apzon.Controls.Grid.GridColumnEx();
		this.rILSalesEmp = new Apzon.Forms.UserControls.RepositoryItemLookUpEditCombo();
		this.gcOpporStatus = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_luk_OOPR_Status = new Apzon.Forms.UserControls.RepositoryItemLookUpEditCombo();
		this.gcOpporStartDate = new Apzon.Controls.Grid.GridColumnEx();
		this.gcOpporClosingDate = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemDateEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.gcOpporClosing = new Apzon.Controls.Grid.GridColumnEx();
		this.rIPClosing = new DevExpress.XtraEditors.Repository.RepositoryItemProgressBar();
		this.gridColumnEx5 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx6 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemMemoExEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit();
		this.btAdd = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btRelatedAct = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btRalatedDoc = new Apzon.Controls.Controls.SimpleButtonEx();
		this.xtraTabControl1 = new Apzon.Controls.Controls.XtraTabControlEx();
		this.tab_Potential = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_InterestRange = new Apzon.Controls.Grid.GridControlEx();
		this.ctm_InterestRange = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.removeRowToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.addRowToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_InterestRange = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx3 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_luk_IntrestRange_IntId = new Apzon.Forms.UserControls.RepositoryItemLookUpEditCombo();
		this.gridColumnEx4 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_chk_IntrestRange_Prmry = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.labelLine5 = new Apzon.Controls.Controls.LabelLine();
		this.luk_LevelOfInterest = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.calcWeightAmount = new Apzon.Controls.Editor.CalcEditEx();
		this.lb_WeightedAmount = new Apzon.Controls.Controls.LabelLine();
		this.calcGrossProfitTotal = new Apzon.Controls.Editor.CalcEditEx();
		this.calcGrossProfit = new Apzon.Controls.Editor.CalcEditEx();
		this.calcPotentialAmount = new Apzon.Controls.Editor.CalcEditEx();
		this.datePredCloDate = new Apzon.Controls.Editor.DateEditEx();
		this.cbClosingType = new Apzon.Controls.Controls.ComboBoxEditEx();
		this.txtPredictCloseIn = new Apzon.Controls.Editor.TextEditEx();
		this.lb_GrossProfit = new Apzon.Controls.Controls.LabelLine();
		this.lb_GrossProfitTotal = new Apzon.Controls.Controls.LabelLine();
		this.lb_PotentialAmount = new Apzon.Controls.Controls.LabelLine();
		this.lb_PredictedClosingDate = new Apzon.Controls.Controls.LabelLine();
		this.lb_PredictedClosingIn = new Apzon.Controls.Controls.LabelLine();
		this.tab_General = new Apzon.Controls.Controls.XtraTabPageEx();
		this.bteCFL_PrjCode = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.bteCFL_ChnCrdCode = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.lukDN_ChnCrdCon = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.lukDN_Industry = new Apzon.Forms.UserControls.LookUpEditDefineNew();
		this.lukDN_Source = new Apzon.Forms.UserControls.LookUpEditDefineNew();
		this.label1 = new System.Windows.Forms.Label();
		this.memo_Memo = new Apzon.Controls.Editor.MemoEditEx();
		this.txt_ChnCrdName = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine7 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine8 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine9 = new Apzon.Controls.Controls.LabelLine();
		this.tab_Stage = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grdStage = new Apzon.Controls.Grid.GridControlEx();
		this.ctm_Stage = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.toolStripMenuItemAddRow = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Stage = new Apzon.Controls.Grid.GridViewEx();
		this.gcStageStartDate = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemDateEdit4 = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.gcStageClosingDate = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemDateEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.gcStageSalesEmp = new Apzon.Controls.Grid.GridColumnEx();
		this.rILStaSalesEmpl = new Apzon.Forms.UserControls.RepositoryItemLookUpEditCombo();
		this.gcStage = new Apzon.Controls.Grid.GridColumnEx();
		this.rILStage = new Apzon.Forms.UserControls.RepositoryItemLookUpEditCombo();
		this.gcStagePrct = new Apzon.Controls.Grid.GridColumnEx();
		this.gcStagePotentialAmount = new Apzon.Controls.Grid.GridColumnEx();
		this.gcStageWeightAmount = new Apzon.Controls.Grid.GridColumnEx();
		this.gcStageShowBPDocs = new Apzon.Controls.Grid.GridColumnEx();
		this.rICShowBPDoc = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gcStageDocType = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_stage_DocType = new Apzon.Forms.UserControls.RepositoryItemLookUpEditCombo();
		this.gcStageDocNo = new Apzon.Controls.Grid.GridColumnEx();
		this.rIBStageDocNo = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcStageOwner = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Stage_Owner = new Apzon.Forms.UserControls.RepositoryItemLookUpEditCombo();
		this.gcStageActivity = new Apzon.Controls.Grid.GridColumnEx();
		this.rIBActivities = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcStageOpprId = new Apzon.Controls.Grid.GridColumnEx();
		this.gcStageLine = new Apzon.Controls.Grid.GridColumnEx();
		this.tab_ItemDetail = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grdItemDetail = new Apzon.Controls.Grid.GridControlEx();
		this.ctm_Items = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.deleteRowsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.createSalesQuotationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.viewChangeLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.viewForeCastToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.addRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_ItemDetail = new Apzon.Controls.Grid.GridViewEx();
		this.gcItemCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rIBItemCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcItemDesc = new Apzon.Controls.Grid.GridColumnEx();
		this.gcItemQty = new Apzon.Controls.Grid.GridColumnEx();
		this.gcIPercent = new Apzon.Controls.Grid.GridColumnEx();
		this.gcItemPrice = new Apzon.Controls.Grid.GridColumnEx();
		this.gcItemPotentAmount = new Apzon.Controls.Grid.GridColumnEx();
		this.gcItemDeliveryDate = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemDateEdit3 = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.gridColumn13 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn14 = new Apzon.Controls.Grid.GridColumnEx();
		this.gcItemFrgnName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcItemAttFile = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_bte_ItemAttFile = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcItemAttFullPath = new Apzon.Controls.Grid.GridColumnEx();
		this.tab_Competitor = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grdCompetitor = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Competitor = new Apzon.Controls.Grid.GridViewEx();
		this.gcCompetitorName = new Apzon.Controls.Grid.GridColumnEx();
		this.rILCompetitor = new Apzon.Forms.UserControls.RepositoryItemLookUpEditCombo();
		this.gcCompetitorThreatLevel = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_luk_OPR3_ThreatLevl = new Apzon.Forms.UserControls.RepositoryItemLookUpEditCombo();
		this.gcCompetitorRemark = new Apzon.Controls.Grid.GridColumnEx();
		this.gcCompetitorWon = new Apzon.Controls.Grid.GridColumnEx();
		this.rICKWon = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gcCompeOpportId = new Apzon.Controls.Grid.GridColumnEx();
		this.gcCompeLine = new Apzon.Controls.Grid.GridColumnEx();
		this.rICThreatLvl = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
		this.tab_Partner = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grdPartner = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Partner = new Apzon.Controls.Grid.GridViewEx();
		this.gcPartnerName = new Apzon.Controls.Grid.GridColumnEx();
		this.rILPartner = new Apzon.Forms.UserControls.RepositoryItemLookUpEditCombo();
		this.gcPartnerRelationShip = new Apzon.Controls.Grid.GridColumnEx();
		this.rILPartnerRela = new Apzon.Forms.UserControls.RepositoryItemLookUpEditCombo();
		this.gcPartnerRelatedBP = new Apzon.Controls.Grid.GridColumnEx();
		this.rIBRelaPartner = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcPartnerRemark = new Apzon.Controls.Grid.GridColumnEx();
		this.gcPartnerOpportId = new Apzon.Controls.Grid.GridColumnEx();
		this.gcPartnerLine = new Apzon.Controls.Grid.GridColumnEx();
		this.tab_Summary = new Apzon.Controls.Controls.XtraTabPageEx();
		this.cbSummaryDocType = new Apzon.Controls.Controls.ComboBoxEditEx();
		this.lb_DocumentNo = new Apzon.Controls.Controls.LabelLine();
		this.lb_DocumentType = new Apzon.Controls.Controls.LabelLine();
		this.rdOppoStatus = new Apzon.Controls.Editor.RadioGroupEx();
		this.grdReason = new Apzon.Controls.Grid.GridControlEx();
		this.ctm_Description = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.removeRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Reason = new Apzon.Controls.Grid.GridViewEx();
		this.gcReasonDesc = new Apzon.Controls.Grid.GridColumnEx();
		this.rILReason = new Apzon.Forms.UserControls.RepositoryItemLookUpEditCombo();
		this.gcReasonOpportId = new Apzon.Controls.Grid.GridColumnEx();
		this.gcReasonLine = new Apzon.Controls.Grid.GridColumnEx();
		this.txtDocNo = new Apzon.Controls.Editor.ButtonEditEx();
		this.tab_Attachment = new Apzon.Controls.Controls.XtraTabPageEx();
		this.ucAttachment1 = new Apzon.Forms.UserControls.ucAttachment();
		this.gc_Stage_DocType_Name = new Apzon.Controls.Grid.GridColumnEx();
		this.lb_FromDate = new Apzon.Controls.Controls.LabelLine();
		this.cbStage = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.cbOpportStatus = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.cbMainSalesEmp = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.txtNoOfTrans = new Apzon.Controls.Editor.TextEditEx();
		this.gcOwner = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Owner = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btEditBusPartner.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dateFrom.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dateFrom.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dateTo.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dateTo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).BeginInit();
		this.ctm_Doc.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Luk_OOPR_OpprType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rIBBussPartner).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rILSalesEmp).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_luk_OOPR_Status).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit1.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rIPClosing).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).BeginInit();
		this.xtraTabControl1.SuspendLayout();
		this.tab_Potential.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_InterestRange).BeginInit();
		this.ctm_InterestRange.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_InterestRange).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_luk_IntrestRange_IntId).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_chk_IntrestRange_Prmry).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_LevelOfInterest.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calcWeightAmount.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calcGrossProfitTotal.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calcGrossProfit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calcPotentialAmount.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.datePredCloDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.datePredCloDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbClosingType.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtPredictCloseIn.Properties).BeginInit();
		this.tab_General.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.bteCFL_PrjCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bteCFL_ChnCrdCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lukDN_ChnCrdCon.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lukDN_Industry.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lukDN_Source.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.memo_Memo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ChnCrdName.Properties).BeginInit();
		this.tab_Stage.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdStage).BeginInit();
		this.ctm_Stage.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Stage).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit4.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit2.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rILStaSalesEmpl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rILStage).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rICShowBPDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_stage_DocType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rIBStageDocNo).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Stage_Owner).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rIBActivities).BeginInit();
		this.tab_ItemDetail.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdItemDetail).BeginInit();
		this.ctm_Items.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_ItemDetail).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rIBItemCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit3.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_bte_ItemAttFile).BeginInit();
		this.tab_Competitor.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdCompetitor).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Competitor).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rILCompetitor).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_luk_OPR3_ThreatLevl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rICKWon).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rICThreatLvl).BeginInit();
		this.tab_Partner.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdPartner).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Partner).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rILPartner).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rILPartnerRela).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rIBRelaPartner).BeginInit();
		this.tab_Summary.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.cbSummaryDocType.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rdOppoStatus.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grdReason).BeginInit();
		this.ctm_Description.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Reason).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rILReason).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtDocNo.Properties).BeginInit();
		this.tab_Attachment.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.cbStage.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbOpportStatus.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbMainSalesEmp.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txtNoOfTrans.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Owner).BeginInit();
		base.SuspendLayout();
		this.repositoryItemCalcEditEx1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx1.IsSearchControl = false;
		this.repositoryItemCalcEditEx1.Name = "repositoryItemCalcEditEx1";
		this.lb_OpportunityStatus.EditControl = null;
		this.lb_OpportunityStatus.EditValue = "Opportunity Status";
		this.lb_OpportunityStatus.FieldName = null;
		this.lb_OpportunityStatus.HideWhenEmpty = false;
		this.lb_OpportunityStatus.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_OpportunityStatus.Location = new System.Drawing.Point(20, 15);
		this.lb_OpportunityStatus.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
		this.lb_OpportunityStatus.Name = "lb_OpportunityStatus";
		this.lb_OpportunityStatus.ReadOnly = false;
		this.lb_OpportunityStatus.SettingInVisible = false;
		this.lb_OpportunityStatus.SettingReadonly = false;
		this.lb_OpportunityStatus.ShowUnderLine = false;
		this.lb_OpportunityStatus.Size = new System.Drawing.Size(139, 25);
		this.lb_OpportunityStatus.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_OpportunityStatus.TabIndex = 0;
		this.lb_OpportunityStatus.Utext = "Opportunity Status";
		this.lb_ToDate.EditControl = null;
		this.lb_ToDate.EditValue = "To Date";
		this.lb_ToDate.FieldName = null;
		this.lb_ToDate.HideWhenEmpty = false;
		this.lb_ToDate.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_ToDate.Location = new System.Drawing.Point(9, 34);
		this.lb_ToDate.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
		this.lb_ToDate.Name = "lb_ToDate";
		this.lb_ToDate.ReadOnly = false;
		this.lb_ToDate.SettingInVisible = false;
		this.lb_ToDate.SettingReadonly = false;
		this.lb_ToDate.ShowUnderLine = true;
		this.lb_ToDate.Size = new System.Drawing.Size(93, 25);
		this.lb_ToDate.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_ToDate.TabIndex = 1;
		this.lb_ToDate.Utext = "To Date";
		this.lb_NoOfTrans.EditControl = null;
		this.lb_NoOfTrans.EditValue = "No. of Trans.";
		this.lb_NoOfTrans.FieldName = null;
		this.lb_NoOfTrans.HideWhenEmpty = false;
		this.lb_NoOfTrans.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_NoOfTrans.Location = new System.Drawing.Point(225, 7);
		this.lb_NoOfTrans.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
		this.lb_NoOfTrans.Name = "lb_NoOfTrans";
		this.lb_NoOfTrans.ReadOnly = false;
		this.lb_NoOfTrans.SettingInVisible = false;
		this.lb_NoOfTrans.SettingReadonly = false;
		this.lb_NoOfTrans.ShowUnderLine = true;
		this.lb_NoOfTrans.Size = new System.Drawing.Size(121, 25);
		this.lb_NoOfTrans.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_NoOfTrans.TabIndex = 2;
		this.lb_NoOfTrans.Utext = "No. of Trans.";
		this.lb_BusinessPartner.EditControl = null;
		this.lb_BusinessPartner.EditValue = "Business Partner";
		this.lb_BusinessPartner.FieldName = null;
		this.lb_BusinessPartner.HideWhenEmpty = false;
		this.lb_BusinessPartner.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_BusinessPartner.Location = new System.Drawing.Point(225, 34);
		this.lb_BusinessPartner.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
		this.lb_BusinessPartner.Name = "lb_BusinessPartner";
		this.lb_BusinessPartner.ReadOnly = false;
		this.lb_BusinessPartner.SettingInVisible = false;
		this.lb_BusinessPartner.SettingReadonly = false;
		this.lb_BusinessPartner.ShowUnderLine = true;
		this.lb_BusinessPartner.Size = new System.Drawing.Size(121, 25);
		this.lb_BusinessPartner.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_BusinessPartner.TabIndex = 3;
		this.lb_BusinessPartner.Utext = "Business Partner";
		this.lb_MainSalesEmp.EditControl = null;
		this.lb_MainSalesEmp.EditValue = "Main Sales Emp.";
		this.lb_MainSalesEmp.FieldName = null;
		this.lb_MainSalesEmp.HideWhenEmpty = false;
		this.lb_MainSalesEmp.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_MainSalesEmp.Location = new System.Drawing.Point(783, 9);
		this.lb_MainSalesEmp.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
		this.lb_MainSalesEmp.Name = "lb_MainSalesEmp";
		this.lb_MainSalesEmp.ReadOnly = false;
		this.lb_MainSalesEmp.SettingInVisible = false;
		this.lb_MainSalesEmp.SettingReadonly = false;
		this.lb_MainSalesEmp.ShowUnderLine = true;
		this.lb_MainSalesEmp.Size = new System.Drawing.Size(103, 25);
		this.lb_MainSalesEmp.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_MainSalesEmp.TabIndex = 6;
		this.lb_MainSalesEmp.Utext = "Main Sales Emp.";
		this.lb_Stage.EditControl = null;
		this.lb_Stage.EditValue = "Stage";
		this.lb_Stage.FieldName = null;
		this.lb_Stage.HideWhenEmpty = false;
		this.lb_Stage.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Stage.Location = new System.Drawing.Point(470, 34);
		this.lb_Stage.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
		this.lb_Stage.Name = "lb_Stage";
		this.lb_Stage.ReadOnly = false;
		this.lb_Stage.SettingInVisible = false;
		this.lb_Stage.SettingReadonly = false;
		this.lb_Stage.ShowUnderLine = true;
		this.lb_Stage.Size = new System.Drawing.Size(93, 25);
		this.lb_Stage.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Stage.TabIndex = 5;
		this.lb_Stage.Utext = "Stage";
		this.lb_OpporStatus.EditControl = null;
		this.lb_OpporStatus.EditValue = "Oppor. Status";
		this.lb_OpporStatus.FieldName = null;
		this.lb_OpporStatus.HideWhenEmpty = false;
		this.lb_OpporStatus.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_OpporStatus.Location = new System.Drawing.Point(470, 9);
		this.lb_OpporStatus.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
		this.lb_OpporStatus.Name = "lb_OpporStatus";
		this.lb_OpporStatus.ReadOnly = false;
		this.lb_OpporStatus.SettingInVisible = false;
		this.lb_OpporStatus.SettingReadonly = false;
		this.lb_OpporStatus.ShowUnderLine = true;
		this.lb_OpporStatus.Size = new System.Drawing.Size(93, 25);
		this.lb_OpporStatus.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_OpporStatus.TabIndex = 4;
		this.lb_OpporStatus.Utext = "Oppor. Status";
		this.btEditBusPartner.EditControl = null;
		this.btEditBusPartner.FieldName = null;
		this.btEditBusPartner.IsSearchControl = true;
		this.btEditBusPartner.Location = new System.Drawing.Point(344, 31);
		this.btEditBusPartner.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.btEditBusPartner.Name = "btEditBusPartner";
		this.btEditBusPartner.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.btEditBusPartner.Properties.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btEditBusPartner_Properties_ButtonPressed);
		this.btEditBusPartner.SettingInVisible = false;
		this.btEditBusPartner.SettingReadonly = false;
		this.btEditBusPartner.Size = new System.Drawing.Size(106, 22);
		this.btEditBusPartner.TabIndex = 11;
		this.dateFrom.EditControl = null;
		this.dateFrom.EditValue = null;
		this.dateFrom.FieldName = null;
		this.dateFrom.Location = new System.Drawing.Point(101, 5);
		this.dateFrom.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.dateFrom.Name = "dateFrom";
		this.dateFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dateFrom.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dateFrom.Properties.IsSearchControl = true;
		this.dateFrom.SettingInVisible = false;
		this.dateFrom.SettingReadonly = false;
		this.dateFrom.Size = new System.Drawing.Size(117, 22);
		this.dateFrom.TabIndex = 8;
		this.dateTo.EditControl = null;
		this.dateTo.EditValue = null;
		this.dateTo.FieldName = null;
		this.dateTo.Location = new System.Drawing.Point(101, 31);
		this.dateTo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.dateTo.Name = "dateTo";
		this.dateTo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dateTo.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dateTo.Properties.IsSearchControl = true;
		this.dateTo.SettingInVisible = false;
		this.dateTo.SettingReadonly = false;
		this.dateTo.Size = new System.Drawing.Size(117, 22);
		this.dateTo.TabIndex = 9;
		this.btSearch.EditControl = null;
		this.btSearch.EditValue = "Search";
		this.btSearch.FieldName = null;
		this.btSearch.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btSearch.ImageOptions.Image");
		this.btSearch.IsSearchControl = true;
		this.btSearch.Location = new System.Drawing.Point(783, 32);
		this.btSearch.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.btSearch.Name = "btSearch";
		this.btSearch.ReadOnly = false;
		this.btSearch.SettingInVisible = false;
		this.btSearch.SettingReadonly = false;
		this.btSearch.Size = new System.Drawing.Size(87, 28);
		this.btSearch.TabIndex = 15;
		this.btSearch.Text = "Search";
		this.btSearch.Click += new System.EventHandler(this.btSearch_Click);
		this.grdDoc.AllowPasteExcel = true;
		this.grdDoc.ClipboardData = null;
		this.grdDoc.ContextMenuStrip = this.ctm_Doc;
		this.grdDoc.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grdDoc.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.grdDoc.Location = new System.Drawing.Point(0, 0);
		this.grdDoc.MainView = this.gv_Doc;
		this.grdDoc.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.grdDoc.Name = "grdDoc";
		this.grdDoc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[8] { this.rILSalesEmp, this.rIPClosing, this.rIBBussPartner, this.repositoryItemDateEdit1, this.rep_luk_OOPR_Status, this.rep_Luk_OOPR_OpprType, this.repositoryItemMemoExEdit1, this.rep_Owner });
		this.grdDoc.Size = new System.Drawing.Size(992, 208);
		this.grdDoc.TabIndex = 16;
		this.grdDoc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.ctm_Doc.ImageScalingSize = new System.Drawing.Size(20, 20);
		this.ctm_Doc.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addNewToolStripMenuItem, this.deleteDocumentToolStripMenuItem });
		this.ctm_Doc.Name = "contextMenuStripDoc";
		this.ctm_Doc.Size = new System.Drawing.Size(141, 52);
		this.ctm_Doc.Opening += new System.ComponentModel.CancelEventHandler(this.ctm_Doc_Opening);
		this.addNewToolStripMenuItem.Name = "addNewToolStripMenuItem";
		this.addNewToolStripMenuItem.Size = new System.Drawing.Size(140, 24);
		this.addNewToolStripMenuItem.Text = "Add New";
		this.addNewToolStripMenuItem.Click += new System.EventHandler(this.addNewToolStripMenuItem_Click);
		this.deleteDocumentToolStripMenuItem.Name = "deleteDocumentToolStripMenuItem";
		this.deleteDocumentToolStripMenuItem.Size = new System.Drawing.Size(140, 24);
		this.deleteDocumentToolStripMenuItem.Text = "Remove";
		this.deleteDocumentToolStripMenuItem.Click += new System.EventHandler(this.deleteDocumentToolStripMenuItem_Click);
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = null;
		this.gv_Doc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[13]
		{
			this.gcOpporType, this.gcOpporNo, this.gcOpporName, this.gcOpporBusCode, this.gcOpporBPName, this.gcOpporSalesEmp, this.gcOwner, this.gcOpporStatus, this.gcOpporStartDate, this.gcOpporClosingDate,
			this.gcOpporClosing, this.gridColumnEx5, this.gridColumnEx6
		});
		this.gv_Doc.DetailHeight = 431;
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = "oOppItemDetail";
		this.gv_Doc.GridControl = this.grdDoc;
		this.gv_Doc.IgnoreFilter = false;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 41;
		this.gv_Doc.IsApzTable = false;
		this.gv_Doc.IsInsertNewRow = false;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.IsMappingContextAction = false;
		this.gv_Doc.ItemId = "OOPR";
		this.gv_Doc.LanguageSource = Apzon.Commons.Enums.Enums.LanguageSource.Documents;
		this.gv_Doc.LstColumnAllowBatch = null;
		this.gv_Doc.LstColumnNotAllowBatch = null;
		this.gv_Doc.LstColumnNotEditable = null;
		this.gv_Doc.LstColumNotSetting = null;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsFind.AllowFindPanel = false;
		this.gv_Doc.OptionsSelection.CheckBoxSelectorColumnWidth = 35;
		this.gv_Doc.OptionsView.ColumnAutoWidth = false;
		this.gv_Doc.OptionsView.ShowDetailButtons = false;
		this.gv_Doc.OptionsView.ShowGroupPanel = false;
		this.gv_Doc.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Doc.UseBatchMod = false;
		this.gv_Doc.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Doc_RowCellStyle);
		this.gv_Doc.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_Doc_ShowingEditor);
		this.gv_Doc.ShownEditor += new System.EventHandler(this.gv_Doc_ShownEditor);
		this.gv_Doc.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gvDoc_FocusedRowChanged);
		this.gv_Doc.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gvDoc_FocusedRowObjectChanged);
		this.gv_Doc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvDoc_CellValueChanged);
		this.gv_Doc.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvDoc_CellValueChanging);
		this.gv_Doc.BeforeLeaveRow += new DevExpress.XtraGrid.Views.Base.RowAllowEventHandler(this.gvDoc_BeforeLeaveRow);
		this.gv_Doc.KeyDown += new System.Windows.Forms.KeyEventHandler(this.gv_Doc_KeyDown);
		this.gv_Doc.ValidatingEditor += new DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventHandler(this.gv_Doc_ValidatingEditor);
		this.gv_Doc.InvalidValueException += new DevExpress.XtraEditors.Controls.InvalidValueExceptionEventHandler(this.gv_Doc_InvalidValueException);
		this.gcOpporType.Caption = "Type";
		this.gcOpporType.ColumnEdit = this.rep_Luk_OOPR_OpprType;
		this.gcOpporType.CurrencyDateField = null;
		this.gcOpporType.CurrencyField = null;
		this.gcOpporType.CurrencyFixed = null;
		this.gcOpporType.CurrencyFixedRate = 0.0;
		this.gcOpporType.CurrencyRateField = null;
		this.gcOpporType.FieldName = "OpprType";
		this.gcOpporType.MinWidth = 23;
		this.gcOpporType.Name = "gcOpporType";
		this.gcOpporType.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
		this.gcOpporType.PrimaryKey = true;
		this.gcOpporType.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcOpporType.UnitField = null;
		this.gcOpporType.Visible = true;
		this.gcOpporType.VisibleIndex = 0;
		this.gcOpporType.Width = 90;
		this.rep_Luk_OOPR_OpprType.AutoHeight = false;
		this.rep_Luk_OOPR_OpprType.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.rep_Luk_OOPR_OpprType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Luk_OOPR_OpprType.IsSearchControl = false;
		this.rep_Luk_OOPR_OpprType.Name = "rep_Luk_OOPR_OpprType";
		this.rep_Luk_OOPR_OpprType.NullText = "";
		this.rep_Luk_OOPR_OpprType.ShowFooter = false;
		this.rep_Luk_OOPR_OpprType.ShowHeader = false;
		this.rep_Luk_OOPR_OpprType.EditValueChanged += new System.EventHandler(this.rep_Luk_OOPR_OpprType_EditValueChanged);
		this.gcOpporNo.Caption = "Oppor. No";
		this.gcOpporNo.CurrencyDateField = null;
		this.gcOpporNo.CurrencyField = null;
		this.gcOpporNo.CurrencyFixed = null;
		this.gcOpporNo.CurrencyFixedRate = 0.0;
		this.gcOpporNo.CurrencyRateField = null;
		this.gcOpporNo.FieldName = "OpprId";
		this.gcOpporNo.MinWidth = 23;
		this.gcOpporNo.Name = "gcOpporNo";
		this.gcOpporNo.OptionsColumn.AllowEdit = false;
		this.gcOpporNo.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
		this.gcOpporNo.OptionsColumn.ReadOnly = true;
		this.gcOpporNo.PrimaryKey = false;
		this.gcOpporNo.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcOpporNo.UnitField = null;
		this.gcOpporNo.Visible = true;
		this.gcOpporNo.VisibleIndex = 1;
		this.gcOpporName.Caption = "Opportunity Name";
		this.gcOpporName.CurrencyDateField = null;
		this.gcOpporName.CurrencyField = null;
		this.gcOpporName.CurrencyFixed = null;
		this.gcOpporName.CurrencyFixedRate = 0.0;
		this.gcOpporName.CurrencyRateField = null;
		this.gcOpporName.FieldName = "Name";
		this.gcOpporName.MinWidth = 23;
		this.gcOpporName.Name = "gcOpporName";
		this.gcOpporName.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
		this.gcOpporName.PrimaryKey = false;
		this.gcOpporName.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcOpporName.UnitField = null;
		this.gcOpporName.Visible = true;
		this.gcOpporName.VisibleIndex = 4;
		this.gcOpporName.Width = 152;
		this.gcOpporBusCode.Caption = "BP Code";
		this.gcOpporBusCode.ColumnEdit = this.rIBBussPartner;
		this.gcOpporBusCode.CurrencyDateField = null;
		this.gcOpporBusCode.CurrencyField = null;
		this.gcOpporBusCode.CurrencyFixed = null;
		this.gcOpporBusCode.CurrencyFixedRate = 0.0;
		this.gcOpporBusCode.CurrencyRateField = null;
		this.gcOpporBusCode.FieldName = "CardCode";
		this.gcOpporBusCode.MinWidth = 23;
		this.gcOpporBusCode.Name = "gcOpporBusCode";
		this.gcOpporBusCode.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
		this.gcOpporBusCode.PrimaryKey = false;
		this.gcOpporBusCode.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcOpporBusCode.UnitField = null;
		this.gcOpporBusCode.Visible = true;
		this.gcOpporBusCode.VisibleIndex = 2;
		this.gcOpporBusCode.Width = 118;
		this.rIBBussPartner.AutoHeight = false;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.rIBBussPartner.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rIBBussPartner.Name = "rIBBussPartner";
		this.rIBBussPartner.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rIBBussPartner_ButtonPressed);
		this.gcOpporBPName.Caption = "BP Name";
		this.gcOpporBPName.CurrencyDateField = null;
		this.gcOpporBPName.CurrencyField = null;
		this.gcOpporBPName.CurrencyFixed = null;
		this.gcOpporBPName.CurrencyFixedRate = 0.0;
		this.gcOpporBPName.CurrencyRateField = null;
		this.gcOpporBPName.FieldName = "CardName";
		this.gcOpporBPName.MinWidth = 23;
		this.gcOpporBPName.Name = "gcOpporBPName";
		this.gcOpporBPName.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
		this.gcOpporBPName.PrimaryKey = false;
		this.gcOpporBPName.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcOpporBPName.UnitField = null;
		this.gcOpporBPName.Visible = true;
		this.gcOpporBPName.VisibleIndex = 3;
		this.gcOpporBPName.Width = 180;
		this.gcOpporSalesEmp.Caption = "Sales Employee";
		this.gcOpporSalesEmp.ColumnEdit = this.rILSalesEmp;
		this.gcOpporSalesEmp.CurrencyDateField = null;
		this.gcOpporSalesEmp.CurrencyField = null;
		this.gcOpporSalesEmp.CurrencyFixed = null;
		this.gcOpporSalesEmp.CurrencyFixedRate = 0.0;
		this.gcOpporSalesEmp.CurrencyRateField = null;
		this.gcOpporSalesEmp.FieldName = "SlpCode";
		this.gcOpporSalesEmp.MinWidth = 23;
		this.gcOpporSalesEmp.Name = "gcOpporSalesEmp";
		this.gcOpporSalesEmp.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
		this.gcOpporSalesEmp.PrimaryKey = false;
		this.gcOpporSalesEmp.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcOpporSalesEmp.UnitField = null;
		this.gcOpporSalesEmp.Visible = true;
		this.gcOpporSalesEmp.VisibleIndex = 7;
		this.gcOpporSalesEmp.Width = 152;
		this.rILSalesEmp.AutoHeight = false;
		this.rILSalesEmp.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.rILSalesEmp.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rILSalesEmp.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("SlpName", "Name")
		});
		this.rILSalesEmp.DisplayMember = "SlpName";
		this.rILSalesEmp.IsSearchControl = false;
		this.rILSalesEmp.Name = "rILSalesEmp";
		this.rILSalesEmp.NullText = "";
		this.rILSalesEmp.ShowFooter = false;
		this.rILSalesEmp.ShowHeader = false;
		this.rILSalesEmp.ValueMember = "SlpCode";
		this.rILSalesEmp.EditValueChanged += new System.EventHandler(this.rILSalesEmp_EditValueChanged);
		this.gcOpporStatus.Caption = "Status";
		this.gcOpporStatus.ColumnEdit = this.rep_luk_OOPR_Status;
		this.gcOpporStatus.CurrencyDateField = null;
		this.gcOpporStatus.CurrencyField = null;
		this.gcOpporStatus.CurrencyFixed = null;
		this.gcOpporStatus.CurrencyFixedRate = 0.0;
		this.gcOpporStatus.CurrencyRateField = null;
		this.gcOpporStatus.FieldName = "Status";
		this.gcOpporStatus.MinWidth = 23;
		this.gcOpporStatus.Name = "gcOpporStatus";
		this.gcOpporStatus.OptionsColumn.AllowEdit = false;
		this.gcOpporStatus.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
		this.gcOpporStatus.OptionsColumn.ReadOnly = true;
		this.gcOpporStatus.PrimaryKey = false;
		this.gcOpporStatus.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcOpporStatus.UnitField = null;
		this.gcOpporStatus.Visible = true;
		this.gcOpporStatus.VisibleIndex = 8;
		this.gcOpporStatus.Width = 62;
		this.rep_luk_OOPR_Status.AutoHeight = false;
		this.rep_luk_OOPR_Status.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.rep_luk_OOPR_Status.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_luk_OOPR_Status.IsSearchControl = false;
		this.rep_luk_OOPR_Status.Name = "rep_luk_OOPR_Status";
		this.rep_luk_OOPR_Status.NullText = "";
		this.rep_luk_OOPR_Status.ShowFooter = false;
		this.rep_luk_OOPR_Status.ShowHeader = false;
		this.gcOpporStartDate.Caption = "Start Date";
		this.gcOpporStartDate.CurrencyDateField = null;
		this.gcOpporStartDate.CurrencyField = null;
		this.gcOpporStartDate.CurrencyFixed = null;
		this.gcOpporStartDate.CurrencyFixedRate = 0.0;
		this.gcOpporStartDate.CurrencyRateField = null;
		this.gcOpporStartDate.FieldName = "OpenDate";
		this.gcOpporStartDate.MinWidth = 23;
		this.gcOpporStartDate.Name = "gcOpporStartDate";
		this.gcOpporStartDate.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
		this.gcOpporStartDate.PrimaryKey = false;
		this.gcOpporStartDate.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcOpporStartDate.UnitField = null;
		this.gcOpporStartDate.Visible = true;
		this.gcOpporStartDate.VisibleIndex = 9;
		this.gcOpporStartDate.Width = 108;
		this.gcOpporClosingDate.Caption = "Closing Date";
		this.gcOpporClosingDate.ColumnEdit = this.repositoryItemDateEdit1;
		this.gcOpporClosingDate.CurrencyDateField = null;
		this.gcOpporClosingDate.CurrencyField = null;
		this.gcOpporClosingDate.CurrencyFixed = null;
		this.gcOpporClosingDate.CurrencyFixedRate = 0.0;
		this.gcOpporClosingDate.CurrencyRateField = null;
		this.gcOpporClosingDate.FieldName = "CloseDate";
		this.gcOpporClosingDate.MinWidth = 23;
		this.gcOpporClosingDate.Name = "gcOpporClosingDate";
		this.gcOpporClosingDate.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
		this.gcOpporClosingDate.PrimaryKey = false;
		this.gcOpporClosingDate.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcOpporClosingDate.UnitField = null;
		this.gcOpporClosingDate.Visible = true;
		this.gcOpporClosingDate.VisibleIndex = 10;
		this.gcOpporClosingDate.Width = 107;
		this.repositoryItemDateEdit1.AutoHeight = false;
		this.repositoryItemDateEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEdit1.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEdit1.Name = "repositoryItemDateEdit1";
		this.gcOpporClosing.Caption = "Closing";
		this.gcOpporClosing.ColumnEdit = this.rIPClosing;
		this.gcOpporClosing.CurrencyDateField = null;
		this.gcOpporClosing.CurrencyField = null;
		this.gcOpporClosing.CurrencyFixed = null;
		this.gcOpporClosing.CurrencyFixedRate = 0.0;
		this.gcOpporClosing.CurrencyRateField = null;
		this.gcOpporClosing.FieldName = "CloPrcnt";
		this.gcOpporClosing.MinWidth = 23;
		this.gcOpporClosing.Name = "gcOpporClosing";
		this.gcOpporClosing.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
		this.gcOpporClosing.PrimaryKey = false;
		this.gcOpporClosing.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcOpporClosing.UnitField = null;
		this.gcOpporClosing.Visible = true;
		this.gcOpporClosing.VisibleIndex = 11;
		this.gcOpporClosing.Width = 104;
		this.rIPClosing.Name = "rIPClosing";
		this.gridColumnEx5.Caption = "Total Invoiced";
		this.gridColumnEx5.ColumnEdit = this.repositoryItemCalcEditEx1;
		this.gridColumnEx5.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.SystemCurrency;
		this.gridColumnEx5.CurrencyDateField = null;
		this.gridColumnEx5.CurrencyField = null;
		this.gridColumnEx5.CurrencyFixed = null;
		this.gridColumnEx5.CurrencyFixedRate = 0.0;
		this.gridColumnEx5.CurrencyRateField = null;
		this.gridColumnEx5.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gridColumnEx5.FieldName = "TotalInvc";
		this.gridColumnEx5.MinWidth = 23;
		this.gridColumnEx5.Name = "gridColumnEx5";
		this.gridColumnEx5.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
		this.gridColumnEx5.PrimaryKey = false;
		this.gridColumnEx5.UnitField = null;
		this.gridColumnEx5.Visible = true;
		this.gridColumnEx5.VisibleIndex = 5;
		this.gridColumnEx5.Width = 91;
		this.gridColumnEx6.Caption = "Memo";
		this.gridColumnEx6.ColumnEdit = this.repositoryItemMemoExEdit1;
		this.gridColumnEx6.CurrencyDateField = null;
		this.gridColumnEx6.CurrencyField = null;
		this.gridColumnEx6.CurrencyFixed = null;
		this.gridColumnEx6.CurrencyFixedRate = 0.0;
		this.gridColumnEx6.CurrencyRateField = null;
		this.gridColumnEx6.FieldName = "Memo";
		this.gridColumnEx6.MinWidth = 23;
		this.gridColumnEx6.Name = "gridColumnEx6";
		this.gridColumnEx6.PrimaryKey = false;
		this.gridColumnEx6.UnitField = null;
		this.gridColumnEx6.Visible = true;
		this.gridColumnEx6.VisibleIndex = 12;
		this.gridColumnEx6.Width = 87;
		this.repositoryItemMemoExEdit1.AutoHeight = false;
		this.repositoryItemMemoExEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemMemoExEdit1.Name = "repositoryItemMemoExEdit1";
		this.repositoryItemMemoExEdit1.ShowIcon = false;
		this.btAdd.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btAdd.ButtonKind = Apzon.Commons.Enums.Enums.SimpleButtonType.Save;
		this.btAdd.EditControl = null;
		this.btAdd.EditValue = "Save";
		this.btAdd.FieldName = null;
		this.btAdd.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btAdd.ImageOptions.Image");
		this.btAdd.IsSearchControl = false;
		this.btAdd.Location = new System.Drawing.Point(9, 549);
		this.btAdd.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.btAdd.Name = "btAdd";
		this.btAdd.ReadOnly = false;
		this.btAdd.SettingInVisible = false;
		this.btAdd.SettingReadonly = false;
		this.btAdd.Size = new System.Drawing.Size(87, 28);
		this.btAdd.TabIndex = 18;
		this.btAdd.Text = "Save";
		this.btAdd.TextChanged += new System.EventHandler(this.btAdd_TextChanged);
		this.btAdd.Click += new System.EventHandler(this.btAdd_Click);
		this.btCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btCancel.EditControl = null;
		this.btCancel.EditValue = "Cancel";
		this.btCancel.FieldName = null;
		this.btCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btCancel.ImageOptions.Image");
		this.btCancel.IsSearchControl = false;
		this.btCancel.Location = new System.Drawing.Point(106, 549);
		this.btCancel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.btCancel.Name = "btCancel";
		this.btCancel.ReadOnly = false;
		this.btCancel.SettingInVisible = false;
		this.btCancel.SettingReadonly = false;
		this.btCancel.Size = new System.Drawing.Size(87, 28);
		this.btCancel.TabIndex = 19;
		this.btCancel.Text = "Cancel";
		this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
		this.btRelatedAct.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btRelatedAct.EditControl = null;
		this.btRelatedAct.EditValue = "Related Activities";
		this.btRelatedAct.FieldName = null;
		this.btRelatedAct.IsSearchControl = false;
		this.btRelatedAct.Location = new System.Drawing.Point(562, 549);
		this.btRelatedAct.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.btRelatedAct.Name = "btRelatedAct";
		this.btRelatedAct.ReadOnly = false;
		this.btRelatedAct.SettingInVisible = false;
		this.btRelatedAct.SettingReadonly = false;
		this.btRelatedAct.Size = new System.Drawing.Size(146, 28);
		this.btRelatedAct.TabIndex = 20;
		this.btRelatedAct.Text = "Related Activities";
		this.btRelatedAct.Click += new System.EventHandler(this.btRelatedAct_Click);
		this.btRalatedDoc.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btRalatedDoc.EditControl = null;
		this.btRalatedDoc.EditValue = "Related Documents";
		this.btRalatedDoc.FieldName = null;
		this.btRalatedDoc.IsSearchControl = false;
		this.btRalatedDoc.Location = new System.Drawing.Point(715, 549);
		this.btRalatedDoc.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.btRalatedDoc.Name = "btRalatedDoc";
		this.btRalatedDoc.ReadOnly = false;
		this.btRalatedDoc.SettingInVisible = false;
		this.btRalatedDoc.SettingReadonly = false;
		this.btRalatedDoc.Size = new System.Drawing.Size(145, 28);
		this.btRalatedDoc.TabIndex = 21;
		this.btRalatedDoc.Text = "Related Documents";
		this.btRalatedDoc.Click += new System.EventHandler(this.btRalatedDoc_Click);
		this.xtraTabControl1.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.xtraTabControl1.Appearance.Options.UseBackColor = true;
		this.xtraTabControl1.AppearancePage.PageClient.BackColor = System.Drawing.SystemColors.Control;
		this.xtraTabControl1.AppearancePage.PageClient.Options.UseBackColor = true;
		this.xtraTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.xtraTabControl1.IsSearchControl = false;
		this.xtraTabControl1.Location = new System.Drawing.Point(0, 0);
		this.xtraTabControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.xtraTabControl1.Name = "xtraTabControl1";
		this.xtraTabControl1.PaintStyleName = "Skin";
		this.xtraTabControl1.SelectedTabPage = this.tab_Potential;
		this.xtraTabControl1.Size = new System.Drawing.Size(992, 255);
		this.xtraTabControl1.TabIndex = 22;
		this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[8] { this.tab_Potential, this.tab_General, this.tab_Stage, this.tab_ItemDetail, this.tab_Competitor, this.tab_Partner, this.tab_Summary, this.tab_Attachment });
		this.tab_Potential.Appearance.PageClient.BackColor = System.Drawing.SystemColors.Control;
		this.tab_Potential.Appearance.PageClient.Options.UseBackColor = true;
		this.tab_Potential.Controls.Add(this.grd_InterestRange);
		this.tab_Potential.Controls.Add(this.labelLine5);
		this.tab_Potential.Controls.Add(this.luk_LevelOfInterest);
		this.tab_Potential.Controls.Add(this.labelLine4);
		this.tab_Potential.Controls.Add(this.calcWeightAmount);
		this.tab_Potential.Controls.Add(this.lb_WeightedAmount);
		this.tab_Potential.Controls.Add(this.calcGrossProfitTotal);
		this.tab_Potential.Controls.Add(this.calcGrossProfit);
		this.tab_Potential.Controls.Add(this.calcPotentialAmount);
		this.tab_Potential.Controls.Add(this.datePredCloDate);
		this.tab_Potential.Controls.Add(this.cbClosingType);
		this.tab_Potential.Controls.Add(this.txtPredictCloseIn);
		this.tab_Potential.Controls.Add(this.lb_GrossProfit);
		this.tab_Potential.Controls.Add(this.lb_GrossProfitTotal);
		this.tab_Potential.Controls.Add(this.lb_PotentialAmount);
		this.tab_Potential.Controls.Add(this.lb_PredictedClosingDate);
		this.tab_Potential.Controls.Add(this.lb_PredictedClosingIn);
		this.tab_Potential.EditControl = null;
		this.tab_Potential.EditValue = "Potential";
		this.tab_Potential.FieldName = "tab_Potential";
		this.tab_Potential.IsSearchControl = false;
		this.tab_Potential.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.tab_Potential.Name = "tab_Potential";
		this.tab_Potential.ReadOnly = false;
		this.tab_Potential.SettingInVisible = false;
		this.tab_Potential.SettingReadonly = false;
		this.tab_Potential.Size = new System.Drawing.Size(990, 225);
		this.tab_Potential.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Potential.Text = "Potential";
		this.grd_InterestRange.AllowPasteExcel = true;
		this.grd_InterestRange.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_InterestRange.ClipboardData = null;
		this.grd_InterestRange.ContextMenuStrip = this.ctm_InterestRange;
		this.grd_InterestRange.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.grd_InterestRange.Location = new System.Drawing.Point(552, 31);
		this.grd_InterestRange.MainView = this.gv_InterestRange;
		this.grd_InterestRange.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.grd_InterestRange.Name = "grd_InterestRange";
		this.grd_InterestRange.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.rep_luk_IntrestRange_IntId, this.rep_chk_IntrestRange_Prmry });
		this.grd_InterestRange.Size = new System.Drawing.Size(410, 181);
		this.grd_InterestRange.TabIndex = 38;
		this.grd_InterestRange.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_InterestRange });
		this.ctm_InterestRange.ImageScalingSize = new System.Drawing.Size(20, 20);
		this.ctm_InterestRange.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.removeRowToolStripMenuItem1, this.addRowToolStripMenuItem1 });
		this.ctm_InterestRange.Name = "ctm_InterestRange";
		this.ctm_InterestRange.Size = new System.Drawing.Size(166, 52);
		this.removeRowToolStripMenuItem1.Name = "removeRowToolStripMenuItem1";
		this.removeRowToolStripMenuItem1.Size = new System.Drawing.Size(165, 24);
		this.removeRowToolStripMenuItem1.Text = "Remove Row";
		this.removeRowToolStripMenuItem1.Click += new System.EventHandler(this.removeRowToolStripMenuItem1_Click);
		this.addRowToolStripMenuItem1.Name = "addRowToolStripMenuItem1";
		this.addRowToolStripMenuItem1.Size = new System.Drawing.Size(165, 24);
		this.addRowToolStripMenuItem1.Text = "Add Row";
		this.addRowToolStripMenuItem1.Click += new System.EventHandler(this.addRowToolStripMenuItem1_Click);
		this.gv_InterestRange.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_InterestRange.ColumnKey = null;
		this.gv_InterestRange.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[4] { this.gridColumnEx1, this.gridColumnEx2, this.gridColumnEx3, this.gridColumnEx4 });
		this.gv_InterestRange.DetailHeight = 431;
		this.gv_InterestRange.DisableFormSetting = false;
		this.gv_InterestRange.DisplayException = false;
		this.gv_InterestRange.EditColumn = null;
		this.gv_InterestRange.FieldCurrency = null;
		this.gv_InterestRange.FieldRate = null;
		this.gv_InterestRange.FormId = "oSalesOpportunity";
		this.gv_InterestRange.GridControl = this.grd_InterestRange;
		this.gv_InterestRange.IgnoreFilter = false;
		this.gv_InterestRange.IndexEditRow = -1;
		this.gv_InterestRange.IndicatorWidth = 41;
		this.gv_InterestRange.IsApzTable = false;
		this.gv_InterestRange.IsInsertNewRow = false;
		this.gv_InterestRange.IsLoadingData = false;
		this.gv_InterestRange.IsMappingContextAction = false;
		this.gv_InterestRange.ItemId = "OINTERRANGE";
		this.gv_InterestRange.LstColumnAllowBatch = null;
		this.gv_InterestRange.LstColumnNotAllowBatch = null;
		this.gv_InterestRange.LstColumnNotEditable = null;
		this.gv_InterestRange.LstColumNotSetting = null;
		this.gv_InterestRange.Name = "gv_InterestRange";
		this.gv_InterestRange.OptionsView.BestFitMaxRowCount = 100;
		this.gv_InterestRange.OptionsView.ColumnAutoWidth = false;
		this.gv_InterestRange.OptionsView.ShowGroupPanel = false;
		this.gv_InterestRange.UseBatchMod = false;
		this.gv_InterestRange.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_InterestRange_ShowingEditor);
		this.gv_InterestRange.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_InterestRange_CellValueChanged);
		this.gridColumnEx1.Caption = "Sequence No.";
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "OprId";
		this.gridColumnEx1.MinWidth = 23;
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.PrimaryKey = true;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx1.Width = 87;
		this.gridColumnEx2.Caption = "Row No.";
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.FieldName = "Line";
		this.gridColumnEx2.MinWidth = 23;
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.UnitField = null;
		this.gridColumnEx2.Width = 87;
		this.gridColumnEx3.Caption = "Interest ID";
		this.gridColumnEx3.ColumnEdit = this.rep_luk_IntrestRange_IntId;
		this.gridColumnEx3.CurrencyDateField = null;
		this.gridColumnEx3.CurrencyField = null;
		this.gridColumnEx3.CurrencyFixed = null;
		this.gridColumnEx3.CurrencyFixedRate = 0.0;
		this.gridColumnEx3.CurrencyRateField = null;
		this.gridColumnEx3.FieldName = "IntId";
		this.gridColumnEx3.MinWidth = 23;
		this.gridColumnEx3.Name = "gridColumnEx3";
		this.gridColumnEx3.PrimaryKey = false;
		this.gridColumnEx3.UnitField = null;
		this.gridColumnEx3.Visible = true;
		this.gridColumnEx3.VisibleIndex = 0;
		this.gridColumnEx3.Width = 244;
		this.rep_luk_IntrestRange_IntId.AutoHeight = false;
		this.rep_luk_IntrestRange_IntId.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.rep_luk_IntrestRange_IntId.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_luk_IntrestRange_IntId.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Descript", "Descript")
		});
		this.rep_luk_IntrestRange_IntId.DisplayMember = "Descript";
		this.rep_luk_IntrestRange_IntId.IsSearchControl = false;
		this.rep_luk_IntrestRange_IntId.Name = "rep_luk_IntrestRange_IntId";
		this.rep_luk_IntrestRange_IntId.NullText = "";
		this.rep_luk_IntrestRange_IntId.ShowFooter = false;
		this.rep_luk_IntrestRange_IntId.ShowHeader = false;
		this.rep_luk_IntrestRange_IntId.ValueMember = "Num";
		this.rep_luk_IntrestRange_IntId.EditValueChanged += new System.EventHandler(this.rep_luk_IntrestRange_IntId_EditValueChanged);
		this.rep_luk_IntrestRange_IntId.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.rep_luk_IntrestRange_IntId_EditValueChanging);
		this.gridColumnEx4.Caption = "Primary Interest";
		this.gridColumnEx4.ColumnEdit = this.rep_chk_IntrestRange_Prmry;
		this.gridColumnEx4.CurrencyDateField = null;
		this.gridColumnEx4.CurrencyField = null;
		this.gridColumnEx4.CurrencyFixed = null;
		this.gridColumnEx4.CurrencyFixedRate = 0.0;
		this.gridColumnEx4.CurrencyRateField = null;
		this.gridColumnEx4.FieldName = "Prmry";
		this.gridColumnEx4.MinWidth = 23;
		this.gridColumnEx4.Name = "gridColumnEx4";
		this.gridColumnEx4.PrimaryKey = false;
		this.gridColumnEx4.UnitField = null;
		this.gridColumnEx4.Visible = true;
		this.gridColumnEx4.VisibleIndex = 1;
		this.gridColumnEx4.Width = 113;
		this.rep_chk_IntrestRange_Prmry.AutoHeight = false;
		this.rep_chk_IntrestRange_Prmry.DisplayValueChecked = "Y";
		this.rep_chk_IntrestRange_Prmry.DisplayValueGrayed = "N";
		this.rep_chk_IntrestRange_Prmry.DisplayValueUnchecked = "N";
		this.rep_chk_IntrestRange_Prmry.Name = "rep_chk_IntrestRange_Prmry";
		this.rep_chk_IntrestRange_Prmry.ValueChecked = "Y";
		this.rep_chk_IntrestRange_Prmry.ValueGrayed = "N";
		this.rep_chk_IntrestRange_Prmry.ValueUnchecked = "N";
		this.rep_chk_IntrestRange_Prmry.EditValueChanged += new System.EventHandler(this.rep_chk_IntrestRange_Prmry_EditValueChanged);
		this.labelLine5.EditControl = null;
		this.labelLine5.EditValue = "Interest Range";
		this.labelLine5.FieldName = null;
		this.labelLine5.HideWhenEmpty = false;
		this.labelLine5.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine5.Location = new System.Drawing.Point(552, 5);
		this.labelLine5.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
		this.labelLine5.Name = "labelLine5";
		this.labelLine5.ReadOnly = false;
		this.labelLine5.SettingInVisible = false;
		this.labelLine5.SettingReadonly = false;
		this.labelLine5.ShowUnderLine = false;
		this.labelLine5.Size = new System.Drawing.Size(108, 25);
		this.labelLine5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine5.TabIndex = 37;
		this.labelLine5.Utext = "Interest Range";
		this.luk_LevelOfInterest.EditControl = null;
		this.luk_LevelOfInterest.FieldName = null;
		this.luk_LevelOfInterest.IsSearchControl = false;
		this.luk_LevelOfInterest.Location = new System.Drawing.Point(215, 178);
		this.luk_LevelOfInterest.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.luk_LevelOfInterest.Name = "luk_LevelOfInterest";
		this.luk_LevelOfInterest.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_LevelOfInterest.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Descript", "Descript")
		});
		this.luk_LevelOfInterest.Properties.DisplayMember = "Descript";
		this.luk_LevelOfInterest.Properties.IsSearchControl = false;
		this.luk_LevelOfInterest.Properties.NullText = "";
		this.luk_LevelOfInterest.Properties.ValueMember = "Num";
		this.luk_LevelOfInterest.Properties.Enter += new System.EventHandler(this.Controls_Properties_Enter);
		this.luk_LevelOfInterest.SettingInVisible = false;
		this.luk_LevelOfInterest.SettingReadonly = false;
		this.luk_LevelOfInterest.Size = new System.Drawing.Size(180, 22);
		this.luk_LevelOfInterest.TabIndex = 36;
		this.luk_LevelOfInterest.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.luk_LevelOfInterest.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.luk_LevelOfInterest_EditValueChanging);
		this.luk_LevelOfInterest.Enter += new System.EventHandler(this.Controls_Properties_Enter);
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "Level of Interest";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(20, 182);
		this.labelLine4.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(196, 25);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 35;
		this.labelLine4.Utext = "Level of Interest";
		this.calcWeightAmount.EditControl = null;
		this.calcWeightAmount.FieldName = null;
		this.calcWeightAmount.IsSearchControl = false;
		this.calcWeightAmount.Location = new System.Drawing.Point(215, 92);
		this.calcWeightAmount.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.calcWeightAmount.Name = "calcWeightAmount";
		this.calcWeightAmount.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.calcWeightAmount.Properties.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.calcWeightAmount.Properties.IsSearchControl = false;
		this.calcWeightAmount.Properties.Enter += new System.EventHandler(this.Controls_Properties_Enter);
		this.calcWeightAmount.SettingInVisible = false;
		this.calcWeightAmount.SettingReadonly = false;
		this.calcWeightAmount.Size = new System.Drawing.Size(180, 22);
		this.calcWeightAmount.TabIndex = 34;
		this.calcWeightAmount.EditValueChanged += new System.EventHandler(this.calcWeightAmount_EditValueChanged);
		this.lb_WeightedAmount.EditControl = null;
		this.lb_WeightedAmount.EditValue = "Weighted Amount";
		this.lb_WeightedAmount.FieldName = null;
		this.lb_WeightedAmount.HideWhenEmpty = false;
		this.lb_WeightedAmount.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_WeightedAmount.Location = new System.Drawing.Point(20, 96);
		this.lb_WeightedAmount.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
		this.lb_WeightedAmount.Name = "lb_WeightedAmount";
		this.lb_WeightedAmount.ReadOnly = false;
		this.lb_WeightedAmount.SettingInVisible = false;
		this.lb_WeightedAmount.SettingReadonly = false;
		this.lb_WeightedAmount.ShowUnderLine = false;
		this.lb_WeightedAmount.Size = new System.Drawing.Size(196, 25);
		this.lb_WeightedAmount.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_WeightedAmount.TabIndex = 33;
		this.lb_WeightedAmount.Utext = "Weighted Amount";
		this.calcGrossProfitTotal.EditControl = null;
		this.calcGrossProfitTotal.FieldName = null;
		this.calcGrossProfitTotal.IsSearchControl = false;
		this.calcGrossProfitTotal.Location = new System.Drawing.Point(215, 146);
		this.calcGrossProfitTotal.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.calcGrossProfitTotal.Name = "calcGrossProfitTotal";
		this.calcGrossProfitTotal.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.calcGrossProfitTotal.Properties.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.calcGrossProfitTotal.Properties.IsSearchControl = false;
		this.calcGrossProfitTotal.Properties.Enter += new System.EventHandler(this.Controls_Properties_Enter);
		this.calcGrossProfitTotal.SettingInVisible = false;
		this.calcGrossProfitTotal.SettingReadonly = false;
		this.calcGrossProfitTotal.Size = new System.Drawing.Size(180, 22);
		this.calcGrossProfitTotal.TabIndex = 32;
		this.calcGrossProfit.EditControl = null;
		this.calcGrossProfit.FieldName = null;
		this.calcGrossProfit.IsSearchControl = false;
		this.calcGrossProfit.Location = new System.Drawing.Point(215, 119);
		this.calcGrossProfit.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.calcGrossProfit.Name = "calcGrossProfit";
		this.calcGrossProfit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.calcGrossProfit.Properties.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Percent;
		this.calcGrossProfit.Properties.IsSearchControl = false;
		this.calcGrossProfit.Properties.Enter += new System.EventHandler(this.Controls_Properties_Enter);
		this.calcGrossProfit.SettingInVisible = false;
		this.calcGrossProfit.SettingReadonly = false;
		this.calcGrossProfit.Size = new System.Drawing.Size(180, 22);
		this.calcGrossProfit.TabIndex = 31;
		this.calcGrossProfit.EditValueChanged += new System.EventHandler(this.calcGrossProfit_EditValueChanged);
		this.calcPotentialAmount.EditControl = null;
		this.calcPotentialAmount.FieldName = null;
		this.calcPotentialAmount.IsSearchControl = false;
		this.calcPotentialAmount.Location = new System.Drawing.Point(215, 65);
		this.calcPotentialAmount.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.calcPotentialAmount.Name = "calcPotentialAmount";
		this.calcPotentialAmount.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.calcPotentialAmount.Properties.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.calcPotentialAmount.Properties.IsSearchControl = false;
		this.calcPotentialAmount.Properties.Enter += new System.EventHandler(this.Controls_Properties_Enter);
		this.calcPotentialAmount.SettingInVisible = false;
		this.calcPotentialAmount.SettingReadonly = false;
		this.calcPotentialAmount.Size = new System.Drawing.Size(180, 22);
		this.calcPotentialAmount.TabIndex = 30;
		this.calcPotentialAmount.EditValueChanged += new System.EventHandler(this.calcPotentialAmount_EditValueChanged);
		this.datePredCloDate.EditControl = null;
		this.datePredCloDate.EditValue = null;
		this.datePredCloDate.FieldName = null;
		this.datePredCloDate.Location = new System.Drawing.Point(215, 38);
		this.datePredCloDate.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.datePredCloDate.Name = "datePredCloDate";
		this.datePredCloDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.datePredCloDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.datePredCloDate.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.datePredCloDate.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.datePredCloDate.Properties.IsSearchControl = false;
		this.datePredCloDate.Properties.Enter += new System.EventHandler(this.Controls_Properties_Enter);
		this.datePredCloDate.SettingInVisible = false;
		this.datePredCloDate.SettingReadonly = false;
		this.datePredCloDate.Size = new System.Drawing.Size(180, 22);
		this.datePredCloDate.TabIndex = 29;
		this.datePredCloDate.EditValueChanged += new System.EventHandler(this.datePredCloDate_EditValueChanged);
		this.datePredCloDate.Validating += new System.ComponentModel.CancelEventHandler(this.datePredCloDate_Leave);
		this.cbClosingType.EditControl = null;
		this.cbClosingType.FieldName = null;
		this.cbClosingType.IsSearchControl = false;
		this.cbClosingType.Location = new System.Drawing.Point(273, 11);
		this.cbClosingType.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.cbClosingType.Name = "cbClosingType";
		this.cbClosingType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbClosingType.Properties.Items.AddRange(new object[3] { "Months", "Weeks", "Days" });
		this.cbClosingType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.cbClosingType.Properties.Enter += new System.EventHandler(this.Controls_Properties_Enter);
		this.cbClosingType.SettingInVisible = false;
		this.cbClosingType.SettingReadonly = false;
		this.cbClosingType.Size = new System.Drawing.Size(121, 22);
		this.cbClosingType.TabIndex = 28;
		this.cbClosingType.SelectedIndexChanged += new System.EventHandler(this.cbClosingType_SelectedIndexChanged);
		this.txtPredictCloseIn.EditControl = null;
		this.txtPredictCloseIn.FieldName = null;
		this.txtPredictCloseIn.IsSearchControl = false;
		this.txtPredictCloseIn.Location = new System.Drawing.Point(215, 11);
		this.txtPredictCloseIn.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.txtPredictCloseIn.Name = "txtPredictCloseIn";
		this.txtPredictCloseIn.Properties.Enter += new System.EventHandler(this.Controls_Properties_Enter);
		this.txtPredictCloseIn.SettingInVisible = false;
		this.txtPredictCloseIn.SettingReadonly = false;
		this.txtPredictCloseIn.Size = new System.Drawing.Size(55, 22);
		this.txtPredictCloseIn.TabIndex = 27;
		this.txtPredictCloseIn.EditValueChanged += new System.EventHandler(this.txtPredictCloseIn_EditValueChanged);
		this.lb_GrossProfit.EditControl = null;
		this.lb_GrossProfit.EditValue = "Gross Profit %";
		this.lb_GrossProfit.FieldName = null;
		this.lb_GrossProfit.HideWhenEmpty = false;
		this.lb_GrossProfit.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_GrossProfit.Location = new System.Drawing.Point(20, 123);
		this.lb_GrossProfit.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
		this.lb_GrossProfit.Name = "lb_GrossProfit";
		this.lb_GrossProfit.ReadOnly = false;
		this.lb_GrossProfit.SettingInVisible = false;
		this.lb_GrossProfit.SettingReadonly = false;
		this.lb_GrossProfit.ShowUnderLine = true;
		this.lb_GrossProfit.Size = new System.Drawing.Size(196, 25);
		this.lb_GrossProfit.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_GrossProfit.TabIndex = 26;
		this.lb_GrossProfit.Utext = "Gross Profit %";
		this.lb_GrossProfitTotal.EditControl = null;
		this.lb_GrossProfitTotal.EditValue = "Gross Profit Total";
		this.lb_GrossProfitTotal.FieldName = null;
		this.lb_GrossProfitTotal.HideWhenEmpty = false;
		this.lb_GrossProfitTotal.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_GrossProfitTotal.Location = new System.Drawing.Point(20, 150);
		this.lb_GrossProfitTotal.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
		this.lb_GrossProfitTotal.Name = "lb_GrossProfitTotal";
		this.lb_GrossProfitTotal.ReadOnly = false;
		this.lb_GrossProfitTotal.SettingInVisible = false;
		this.lb_GrossProfitTotal.SettingReadonly = false;
		this.lb_GrossProfitTotal.ShowUnderLine = false;
		this.lb_GrossProfitTotal.Size = new System.Drawing.Size(196, 25);
		this.lb_GrossProfitTotal.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_GrossProfitTotal.TabIndex = 25;
		this.lb_GrossProfitTotal.Utext = "Gross Profit Total";
		this.lb_PotentialAmount.EditControl = null;
		this.lb_PotentialAmount.EditValue = "Potential Amount";
		this.lb_PotentialAmount.FieldName = null;
		this.lb_PotentialAmount.HideWhenEmpty = false;
		this.lb_PotentialAmount.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_PotentialAmount.Location = new System.Drawing.Point(20, 69);
		this.lb_PotentialAmount.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
		this.lb_PotentialAmount.Name = "lb_PotentialAmount";
		this.lb_PotentialAmount.ReadOnly = false;
		this.lb_PotentialAmount.SettingInVisible = false;
		this.lb_PotentialAmount.SettingReadonly = false;
		this.lb_PotentialAmount.ShowUnderLine = false;
		this.lb_PotentialAmount.Size = new System.Drawing.Size(196, 25);
		this.lb_PotentialAmount.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_PotentialAmount.TabIndex = 24;
		this.lb_PotentialAmount.Utext = "Potential Amount";
		this.lb_PredictedClosingDate.EditControl = null;
		this.lb_PredictedClosingDate.EditValue = "Predicted Closing Date";
		this.lb_PredictedClosingDate.FieldName = null;
		this.lb_PredictedClosingDate.HideWhenEmpty = false;
		this.lb_PredictedClosingDate.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_PredictedClosingDate.Location = new System.Drawing.Point(20, 42);
		this.lb_PredictedClosingDate.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
		this.lb_PredictedClosingDate.Name = "lb_PredictedClosingDate";
		this.lb_PredictedClosingDate.ReadOnly = false;
		this.lb_PredictedClosingDate.SettingInVisible = false;
		this.lb_PredictedClosingDate.SettingReadonly = false;
		this.lb_PredictedClosingDate.ShowUnderLine = true;
		this.lb_PredictedClosingDate.Size = new System.Drawing.Size(196, 25);
		this.lb_PredictedClosingDate.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_PredictedClosingDate.TabIndex = 23;
		this.lb_PredictedClosingDate.Utext = "Predicted Closing Date";
		this.lb_PredictedClosingIn.EditControl = null;
		this.lb_PredictedClosingIn.EditValue = "Predicted Closing In";
		this.lb_PredictedClosingIn.FieldName = null;
		this.lb_PredictedClosingIn.HideWhenEmpty = false;
		this.lb_PredictedClosingIn.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_PredictedClosingIn.Location = new System.Drawing.Point(20, 15);
		this.lb_PredictedClosingIn.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
		this.lb_PredictedClosingIn.Name = "lb_PredictedClosingIn";
		this.lb_PredictedClosingIn.ReadOnly = false;
		this.lb_PredictedClosingIn.SettingInVisible = false;
		this.lb_PredictedClosingIn.SettingReadonly = false;
		this.lb_PredictedClosingIn.ShowUnderLine = true;
		this.lb_PredictedClosingIn.Size = new System.Drawing.Size(196, 25);
		this.lb_PredictedClosingIn.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_PredictedClosingIn.TabIndex = 22;
		this.lb_PredictedClosingIn.Utext = "Predicted Closing In";
		this.tab_General.Controls.Add(this.bteCFL_PrjCode);
		this.tab_General.Controls.Add(this.bteCFL_ChnCrdCode);
		this.tab_General.Controls.Add(this.lukDN_ChnCrdCon);
		this.tab_General.Controls.Add(this.lukDN_Industry);
		this.tab_General.Controls.Add(this.lukDN_Source);
		this.tab_General.Controls.Add(this.label1);
		this.tab_General.Controls.Add(this.memo_Memo);
		this.tab_General.Controls.Add(this.txt_ChnCrdName);
		this.tab_General.Controls.Add(this.labelLine1);
		this.tab_General.Controls.Add(this.labelLine2);
		this.tab_General.Controls.Add(this.labelLine3);
		this.tab_General.Controls.Add(this.labelLine7);
		this.tab_General.Controls.Add(this.labelLine8);
		this.tab_General.Controls.Add(this.labelLine9);
		this.tab_General.EditControl = null;
		this.tab_General.EditValue = "General";
		this.tab_General.FieldName = "tab_General";
		this.tab_General.IsSearchControl = false;
		this.tab_General.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.tab_General.Name = "tab_General";
		this.tab_General.ReadOnly = false;
		this.tab_General.SettingInVisible = false;
		this.tab_General.SettingReadonly = false;
		this.tab_General.Size = new System.Drawing.Size(990, 225);
		this.tab_General.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_General.Text = "General";
		this.bteCFL_PrjCode.EditControl = null;
		this.bteCFL_PrjCode.FieldName = null;
		this.bteCFL_PrjCode.IsSearchControl = false;
		this.bteCFL_PrjCode.Location = new System.Drawing.Point(772, 15);
		this.bteCFL_PrjCode.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.bteCFL_PrjCode.Name = "bteCFL_PrjCode";
		this.bteCFL_PrjCode.Properties.AutoLoadDataSource = true;
		this.bteCFL_PrjCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, false, true, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.bteCFL_PrjCode.Properties.ContainerControl = null;
		this.bteCFL_PrjCode.Properties.DisplayMember = "PrjCode";
		this.bteCFL_PrjCode.Properties.DisplayMember2 = "";
		this.bteCFL_PrjCode.Properties.LoadDataSourceWhenEnter = false;
		this.bteCFL_PrjCode.Properties.SeparateCharacterForMulti = ';';
		this.bteCFL_PrjCode.Properties.SeparateString = ", ";
		this.bteCFL_PrjCode.Properties.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.Project;
		this.bteCFL_PrjCode.Properties.UsingAutoCompleteBox = false;
		this.bteCFL_PrjCode.Properties.UsingDataSource = true;
		this.bteCFL_PrjCode.Properties.ValidateWhenNoDataSource = false;
		this.bteCFL_PrjCode.Properties.ValueMember = "PrjCode";
		this.bteCFL_PrjCode.SettingInVisible = false;
		this.bteCFL_PrjCode.SettingReadonly = false;
		this.bteCFL_PrjCode.Size = new System.Drawing.Size(182, 22);
		this.bteCFL_PrjCode.TabIndex = 47;
		this.bteCFL_PrjCode.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.bteCFL_PrjCode.Enter += new System.EventHandler(this.Controls_Properties_Enter);
		this.bteCFL_ChnCrdCode.EditControl = null;
		this.bteCFL_ChnCrdCode.FieldName = null;
		this.bteCFL_ChnCrdCode.IsSearchControl = false;
		this.bteCFL_ChnCrdCode.Location = new System.Drawing.Point(199, 15);
		this.bteCFL_ChnCrdCode.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.bteCFL_ChnCrdCode.Name = "bteCFL_ChnCrdCode";
		this.bteCFL_ChnCrdCode.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions2.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions5.Image");
		this.bteCFL_ChnCrdCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.bteCFL_ChnCrdCode.Properties.ContainerControl = null;
		this.bteCFL_ChnCrdCode.Properties.DisplayMember = "CardCode";
		this.bteCFL_ChnCrdCode.Properties.DisplayMember2 = "";
		this.bteCFL_ChnCrdCode.Properties.LoadDataSourceWhenEnter = false;
		this.bteCFL_ChnCrdCode.Properties.SeparateCharacterForMulti = ';';
		this.bteCFL_ChnCrdCode.Properties.SeparateString = ", ";
		this.bteCFL_ChnCrdCode.Properties.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.Bp;
		this.bteCFL_ChnCrdCode.Properties.TypeDrillDown = Apzon.Commons.ApzObjectTypes.oBusinessPartners;
		this.bteCFL_ChnCrdCode.Properties.UsingAutoCompleteBox = false;
		this.bteCFL_ChnCrdCode.Properties.UsingDataSource = true;
		this.bteCFL_ChnCrdCode.Properties.ValidateWhenNoDataSource = false;
		this.bteCFL_ChnCrdCode.Properties.ValueMember = "CardCode";
		this.bteCFL_ChnCrdCode.Properties.Enter += new System.EventHandler(this.Controls_Properties_Enter);
		this.bteCFL_ChnCrdCode.SettingInVisible = false;
		this.bteCFL_ChnCrdCode.SettingReadonly = false;
		this.bteCFL_ChnCrdCode.Size = new System.Drawing.Size(182, 22);
		this.bteCFL_ChnCrdCode.TabIndex = 46;
		this.bteCFL_ChnCrdCode.EditValueChanged += new System.EventHandler(this.bteCFL_ChnCrdCode_EditValueChanged);
		this.bteCFL_ChnCrdCode.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.bteCFL_ChnCrdCode_EditValueChanging);
		this.lukDN_ChnCrdCon.EditControl = null;
		this.lukDN_ChnCrdCon.FieldName = null;
		this.lukDN_ChnCrdCon.IsSearchControl = false;
		this.lukDN_ChnCrdCon.Location = new System.Drawing.Point(199, 69);
		this.lukDN_ChnCrdCon.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lukDN_ChnCrdCon.Name = "lukDN_ChnCrdCon";
		this.lukDN_ChnCrdCon.Properties.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
		this.lukDN_ChnCrdCon.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.lukDN_ChnCrdCon.Properties.DisplayMember = "Name";
		this.lukDN_ChnCrdCon.Properties.IsSearchControl = false;
		this.lukDN_ChnCrdCon.Properties.NullText = "";
		this.lukDN_ChnCrdCon.Properties.ShowFooter = false;
		this.lukDN_ChnCrdCon.Properties.ShowHeader = false;
		this.lukDN_ChnCrdCon.Properties.ValueMember = "CntctCode";
		this.lukDN_ChnCrdCon.SettingInVisible = false;
		this.lukDN_ChnCrdCon.SettingReadonly = false;
		this.lukDN_ChnCrdCon.Size = new System.Drawing.Size(182, 22);
		this.lukDN_ChnCrdCon.TabIndex = 45;
		this.lukDN_ChnCrdCon.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.lukDN_ChnCrdCon.Enter += new System.EventHandler(this.Controls_Properties_Enter);
		this.lukDN_Industry.EditControl = null;
		this.lukDN_Industry.FieldName = null;
		this.lukDN_Industry.IsSearchControl = false;
		this.lukDN_Industry.Location = new System.Drawing.Point(772, 69);
		this.lukDN_Industry.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lukDN_Industry.Name = "lukDN_Industry";
		this.lukDN_Industry.Properties.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
		this.lukDN_Industry.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.lukDN_Industry.Properties.CheckFields = null;
		this.lukDN_Industry.Properties.CheckTables = null;
		this.lukDN_Industry.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("IndName", "IndName")
		});
		this.lukDN_Industry.Properties.DisplayMember = "IndName";
		this.lukDN_Industry.Properties.NullText = "";
		this.lukDN_Industry.Properties.ShowFooter = false;
		this.lukDN_Industry.Properties.ShowHeader = false;
		this.lukDN_Industry.Properties.ValueMember = "IndCode";
		this.lukDN_Industry.SettingInVisible = false;
		this.lukDN_Industry.SettingReadonly = false;
		this.lukDN_Industry.Size = new System.Drawing.Size(182, 22);
		this.lukDN_Industry.TabIndex = 44;
		this.lukDN_Industry.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.lukDN_Industry.Enter += new System.EventHandler(this.Controls_Properties_Enter);
		this.lukDN_Source.EditControl = null;
		this.lukDN_Source.FieldName = null;
		this.lukDN_Source.IsSearchControl = false;
		this.lukDN_Source.Location = new System.Drawing.Point(772, 42);
		this.lukDN_Source.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lukDN_Source.Name = "lukDN_Source";
		this.lukDN_Source.Properties.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
		this.lukDN_Source.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.lukDN_Source.Properties.CheckFields = null;
		this.lukDN_Source.Properties.CheckTables = null;
		this.lukDN_Source.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Descript", "Descript")
		});
		this.lukDN_Source.Properties.DisplayMember = "Descript";
		this.lukDN_Source.Properties.NullText = "";
		this.lukDN_Source.Properties.ShowFooter = false;
		this.lukDN_Source.Properties.ShowHeader = false;
		this.lukDN_Source.Properties.ValueMember = "Num";
		this.lukDN_Source.SettingInVisible = false;
		this.lukDN_Source.SettingReadonly = false;
		this.lukDN_Source.Size = new System.Drawing.Size(182, 22);
		this.lukDN_Source.TabIndex = 43;
		this.lukDN_Source.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.lukDN_Source.Enter += new System.EventHandler(this.Controls_Properties_Enter);
		this.label1.AutoSize = true;
		this.label1.Location = new System.Drawing.Point(16, 107);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(61, 17);
		this.label1.TabIndex = 42;
		this.label1.Text = "Remarks";
		this.memo_Memo.EditControl = null;
		this.memo_Memo.FieldName = null;
		this.memo_Memo.IsSearchControl = false;
		this.memo_Memo.Location = new System.Drawing.Point(199, 106);
		this.memo_Memo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.memo_Memo.Name = "memo_Memo";
		this.memo_Memo.SettingInVisible = false;
		this.memo_Memo.SettingReadonly = false;
		this.memo_Memo.Size = new System.Drawing.Size(755, 110);
		this.memo_Memo.TabIndex = 41;
		this.memo_Memo.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.memo_Memo.Enter += new System.EventHandler(this.Controls_Properties_Enter);
		this.txt_ChnCrdName.EditControl = null;
		this.txt_ChnCrdName.FieldName = null;
		this.txt_ChnCrdName.IsSearchControl = false;
		this.txt_ChnCrdName.Location = new System.Drawing.Point(199, 42);
		this.txt_ChnCrdName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.txt_ChnCrdName.Name = "txt_ChnCrdName";
		this.txt_ChnCrdName.Properties.ReadOnly = true;
		this.txt_ChnCrdName.SettingInVisible = false;
		this.txt_ChnCrdName.SettingReadonly = false;
		this.txt_ChnCrdName.Size = new System.Drawing.Size(182, 22);
		this.txt_ChnCrdName.TabIndex = 38;
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Information Source";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(591, 46);
		this.labelLine1.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(182, 25);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 35;
		this.labelLine1.Utext = "Information Source";
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "BP Project";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(591, 20);
		this.labelLine2.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(182, 25);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 34;
		this.labelLine2.Utext = "BP Project";
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "Industry";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(591, 73);
		this.labelLine3.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(182, 25);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 33;
		this.labelLine3.Utext = "Industry";
		this.labelLine7.EditControl = null;
		this.labelLine7.EditValue = "BP Channel Name";
		this.labelLine7.FieldName = null;
		this.labelLine7.HideWhenEmpty = false;
		this.labelLine7.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine7.Location = new System.Drawing.Point(20, 46);
		this.labelLine7.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
		this.labelLine7.Name = "labelLine7";
		this.labelLine7.ReadOnly = false;
		this.labelLine7.SettingInVisible = false;
		this.labelLine7.SettingReadonly = false;
		this.labelLine7.ShowUnderLine = true;
		this.labelLine7.Size = new System.Drawing.Size(182, 25);
		this.labelLine7.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine7.TabIndex = 32;
		this.labelLine7.Utext = "BP Channel Name";
		this.labelLine8.EditControl = null;
		this.labelLine8.EditValue = "BP Channel Code";
		this.labelLine8.FieldName = null;
		this.labelLine8.HideWhenEmpty = false;
		this.labelLine8.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine8.Location = new System.Drawing.Point(20, 20);
		this.labelLine8.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
		this.labelLine8.Name = "labelLine8";
		this.labelLine8.ReadOnly = false;
		this.labelLine8.SettingInVisible = false;
		this.labelLine8.SettingReadonly = false;
		this.labelLine8.ShowUnderLine = true;
		this.labelLine8.Size = new System.Drawing.Size(182, 25);
		this.labelLine8.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine8.TabIndex = 31;
		this.labelLine8.Utext = "BP Channel Code";
		this.labelLine9.EditControl = null;
		this.labelLine9.EditValue = "BP Channel Contact";
		this.labelLine9.FieldName = null;
		this.labelLine9.HideWhenEmpty = false;
		this.labelLine9.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine9.Location = new System.Drawing.Point(19, 73);
		this.labelLine9.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
		this.labelLine9.Name = "labelLine9";
		this.labelLine9.ReadOnly = false;
		this.labelLine9.SettingInVisible = false;
		this.labelLine9.SettingReadonly = false;
		this.labelLine9.ShowUnderLine = true;
		this.labelLine9.Size = new System.Drawing.Size(182, 25);
		this.labelLine9.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine9.TabIndex = 30;
		this.labelLine9.Utext = "BP Channel Contact";
		this.tab_Stage.Controls.Add(this.grdStage);
		this.tab_Stage.EditControl = null;
		this.tab_Stage.EditValue = "Stage";
		this.tab_Stage.FieldName = "tab_Stage";
		this.tab_Stage.IsSearchControl = false;
		this.tab_Stage.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.tab_Stage.Name = "tab_Stage";
		this.tab_Stage.ReadOnly = false;
		this.tab_Stage.SettingInVisible = false;
		this.tab_Stage.SettingReadonly = false;
		this.tab_Stage.Size = new System.Drawing.Size(990, 225);
		this.tab_Stage.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Stage.Text = "Stage";
		this.grdStage.AllowPasteExcel = false;
		this.grdStage.ClipboardData = null;
		this.grdStage.ContextMenuStrip = this.ctm_Stage;
		this.grdStage.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grdStage.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.grdStage.Location = new System.Drawing.Point(0, 0);
		this.grdStage.MainView = this.gv_Stage;
		this.grdStage.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.grdStage.Name = "grdStage";
		this.grdStage.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[9] { this.rILStage, this.rILStaSalesEmpl, this.rIBActivities, this.rICShowBPDoc, this.repositoryItemDateEdit2, this.rIBStageDocNo, this.repositoryItemDateEdit4, this.rep_stage_DocType, this.rep_Stage_Owner });
		this.grdStage.Size = new System.Drawing.Size(990, 225);
		this.grdStage.TabIndex = 17;
		this.grdStage.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Stage });
		this.ctm_Stage.ImageScalingSize = new System.Drawing.Size(20, 20);
		this.ctm_Stage.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.toolStripMenuItemAddRow, this.deleteRowToolStripMenuItem });
		this.ctm_Stage.Name = "contextMenuStripStage";
		this.ctm_Stage.Size = new System.Drawing.Size(156, 52);
		this.toolStripMenuItemAddRow.Name = "toolStripMenuItemAddRow";
		this.toolStripMenuItemAddRow.Size = new System.Drawing.Size(155, 24);
		this.toolStripMenuItemAddRow.Text = "Add Row";
		this.toolStripMenuItemAddRow.Click += new System.EventHandler(this.toolStripMenuItemAddRow_Click);
		this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
		this.deleteRowToolStripMenuItem.Size = new System.Drawing.Size(155, 24);
		this.deleteRowToolStripMenuItem.Text = "Delete Row";
		this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.deleteRowToolStripMenuItem_Click);
		this.gv_Stage.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Stage.ColumnKey = null;
		this.gv_Stage.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[14]
		{
			this.gcStageStartDate, this.gcStageClosingDate, this.gcStageSalesEmp, this.gcStage, this.gcStagePrct, this.gcStagePotentialAmount, this.gcStageWeightAmount, this.gcStageShowBPDocs, this.gcStageDocType, this.gcStageDocNo,
			this.gcStageOwner, this.gcStageActivity, this.gcStageOpprId, this.gcStageLine
		});
		this.gv_Stage.DetailHeight = 431;
		this.gv_Stage.DisableFormSetting = false;
		this.gv_Stage.DisplayException = false;
		this.gv_Stage.EditColumn = null;
		this.gv_Stage.FieldCurrency = null;
		this.gv_Stage.FieldRate = null;
		this.gv_Stage.FormId = "oOppItemDetail";
		this.gv_Stage.GridControl = this.grdStage;
		this.gv_Stage.IgnoreFilter = false;
		this.gv_Stage.IndexEditRow = -1;
		this.gv_Stage.IndicatorWidth = 41;
		this.gv_Stage.IsApzTable = false;
		this.gv_Stage.IsInsertNewRow = false;
		this.gv_Stage.IsLoadingData = false;
		this.gv_Stage.IsMappingContextAction = false;
		this.gv_Stage.ItemId = "OPR1";
		this.gv_Stage.LanguageSource = Apzon.Commons.Enums.Enums.LanguageSource.Documents;
		this.gv_Stage.LstColumnAllowBatch = null;
		this.gv_Stage.LstColumnNotAllowBatch = null;
		this.gv_Stage.LstColumnNotEditable = null;
		this.gv_Stage.LstColumNotSetting = null;
		this.gv_Stage.Name = "gv_Stage";
		this.gv_Stage.OptionsCustomization.AllowSort = false;
		this.gv_Stage.OptionsFind.AllowFindPanel = false;
		this.gv_Stage.OptionsSelection.CheckBoxSelectorColumnWidth = 35;
		this.gv_Stage.OptionsSelection.MultiSelect = true;
		this.gv_Stage.OptionsView.ColumnAutoWidth = false;
		this.gv_Stage.OptionsView.ShowGroupPanel = false;
		this.gv_Stage.UseBatchMod = false;
		this.gv_Stage.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.gvStage_RowStyle);
		this.gv_Stage.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gvStage_ShowingEditor);
		this.gv_Stage.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvStage_CellValueChanged);
		this.gcStageStartDate.Caption = "Start Date";
		this.gcStageStartDate.ColumnEdit = this.repositoryItemDateEdit4;
		this.gcStageStartDate.CurrencyDateField = null;
		this.gcStageStartDate.CurrencyField = null;
		this.gcStageStartDate.CurrencyFixed = null;
		this.gcStageStartDate.CurrencyFixedRate = 0.0;
		this.gcStageStartDate.CurrencyRateField = null;
		this.gcStageStartDate.FieldName = "OpenDate";
		this.gcStageStartDate.MinWidth = 23;
		this.gcStageStartDate.Name = "gcStageStartDate";
		this.gcStageStartDate.PrimaryKey = false;
		this.gcStageStartDate.UnitField = null;
		this.gcStageStartDate.Visible = true;
		this.gcStageStartDate.VisibleIndex = 0;
		this.gcStageStartDate.Width = 99;
		this.repositoryItemDateEdit4.AutoHeight = false;
		this.repositoryItemDateEdit4.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEdit4.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEdit4.Name = "repositoryItemDateEdit4";
		this.gcStageClosingDate.Caption = "Closing Date";
		this.gcStageClosingDate.ColumnEdit = this.repositoryItemDateEdit2;
		this.gcStageClosingDate.CurrencyDateField = null;
		this.gcStageClosingDate.CurrencyField = null;
		this.gcStageClosingDate.CurrencyFixed = null;
		this.gcStageClosingDate.CurrencyFixedRate = 0.0;
		this.gcStageClosingDate.CurrencyRateField = null;
		this.gcStageClosingDate.FieldName = "CloseDate";
		this.gcStageClosingDate.MinWidth = 23;
		this.gcStageClosingDate.Name = "gcStageClosingDate";
		this.gcStageClosingDate.PrimaryKey = false;
		this.gcStageClosingDate.UnitField = null;
		this.gcStageClosingDate.Visible = true;
		this.gcStageClosingDate.VisibleIndex = 1;
		this.gcStageClosingDate.Width = 103;
		this.repositoryItemDateEdit2.AutoHeight = false;
		this.repositoryItemDateEdit2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEdit2.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEdit2.Name = "repositoryItemDateEdit2";
		this.gcStageSalesEmp.Caption = "Sales Emp";
		this.gcStageSalesEmp.ColumnEdit = this.rILStaSalesEmpl;
		this.gcStageSalesEmp.CurrencyDateField = null;
		this.gcStageSalesEmp.CurrencyField = null;
		this.gcStageSalesEmp.CurrencyFixed = null;
		this.gcStageSalesEmp.CurrencyFixedRate = 0.0;
		this.gcStageSalesEmp.CurrencyRateField = null;
		this.gcStageSalesEmp.FieldName = "SlpCode";
		this.gcStageSalesEmp.MinWidth = 23;
		this.gcStageSalesEmp.Name = "gcStageSalesEmp";
		this.gcStageSalesEmp.PrimaryKey = false;
		this.gcStageSalesEmp.UnitField = null;
		this.gcStageSalesEmp.Visible = true;
		this.gcStageSalesEmp.VisibleIndex = 2;
		this.gcStageSalesEmp.Width = 141;
		this.rILStaSalesEmpl.AutoHeight = false;
		this.rILStaSalesEmpl.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.rILStaSalesEmpl.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rILStaSalesEmpl.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("SlpName", "Name")
		});
		this.rILStaSalesEmpl.DisplayMember = "SlpName";
		this.rILStaSalesEmpl.IsSearchControl = false;
		this.rILStaSalesEmpl.Name = "rILStaSalesEmpl";
		this.rILStaSalesEmpl.NullText = "";
		this.rILStaSalesEmpl.ShowFooter = false;
		this.rILStaSalesEmpl.ShowHeader = false;
		this.rILStaSalesEmpl.ValueMember = "SlpCode";
		this.gcStage.Caption = "Stage";
		this.gcStage.ColumnEdit = this.rILStage;
		this.gcStage.CurrencyDateField = null;
		this.gcStage.CurrencyField = null;
		this.gcStage.CurrencyFixed = null;
		this.gcStage.CurrencyFixedRate = 0.0;
		this.gcStage.CurrencyRateField = null;
		this.gcStage.FieldName = "Step_Id";
		this.gcStage.MinWidth = 23;
		this.gcStage.Name = "gcStage";
		this.gcStage.PrimaryKey = false;
		this.gcStage.UnitField = null;
		this.gcStage.Visible = true;
		this.gcStage.VisibleIndex = 3;
		this.gcStage.Width = 140;
		this.rILStage.AutoHeight = false;
		this.rILStage.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.rILStage.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rILStage.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Descript", "Name")
		});
		this.rILStage.DisplayMember = "Descript";
		this.rILStage.IsSearchControl = false;
		this.rILStage.Name = "rILStage";
		this.rILStage.NullText = "";
		this.rILStage.ShowFooter = false;
		this.rILStage.ShowHeader = false;
		this.rILStage.ValueMember = "Step_Id";
		this.rILStage.EditValueChanged += new System.EventHandler(this.rILStage_EditValueChanged);
		this.rILStage.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.rILStage_EditValueChanging);
		this.gcStagePrct.Caption = "%";
		this.gcStagePrct.CurrencyDateField = null;
		this.gcStagePrct.CurrencyField = null;
		this.gcStagePrct.CurrencyFixed = null;
		this.gcStagePrct.CurrencyFixedRate = 0.0;
		this.gcStagePrct.CurrencyRateField = null;
		this.gcStagePrct.FieldName = "ClosePrcnt";
		this.gcStagePrct.MinWidth = 23;
		this.gcStagePrct.Name = "gcStagePrct";
		this.gcStagePrct.PrimaryKey = false;
		this.gcStagePrct.UnitField = null;
		this.gcStagePrct.Visible = true;
		this.gcStagePrct.VisibleIndex = 4;
		this.gcStagePrct.Width = 42;
		this.gcStagePotentialAmount.Caption = "Potential Amount";
		this.gcStagePotentialAmount.CurrencyDateField = null;
		this.gcStagePotentialAmount.CurrencyField = null;
		this.gcStagePotentialAmount.CurrencyFixed = null;
		this.gcStagePotentialAmount.CurrencyFixedRate = 0.0;
		this.gcStagePotentialAmount.CurrencyRateField = null;
		this.gcStagePotentialAmount.DisplayFormat.FormatString = "n2";
		this.gcStagePotentialAmount.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gcStagePotentialAmount.FieldName = "MaxSumLoc";
		this.gcStagePotentialAmount.MinWidth = 23;
		this.gcStagePotentialAmount.Name = "gcStagePotentialAmount";
		this.gcStagePotentialAmount.PrimaryKey = false;
		this.gcStagePotentialAmount.UnitField = null;
		this.gcStagePotentialAmount.Visible = true;
		this.gcStagePotentialAmount.VisibleIndex = 5;
		this.gcStagePotentialAmount.Width = 110;
		this.gcStageWeightAmount.Caption = "Weight Amount";
		this.gcStageWeightAmount.CurrencyDateField = null;
		this.gcStageWeightAmount.CurrencyField = null;
		this.gcStageWeightAmount.CurrencyFixed = null;
		this.gcStageWeightAmount.CurrencyFixedRate = 0.0;
		this.gcStageWeightAmount.CurrencyRateField = null;
		this.gcStageWeightAmount.DisplayFormat.FormatString = "n2";
		this.gcStageWeightAmount.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gcStageWeightAmount.FieldName = "WtSumLoc";
		this.gcStageWeightAmount.MinWidth = 23;
		this.gcStageWeightAmount.Name = "gcStageWeightAmount";
		this.gcStageWeightAmount.PrimaryKey = false;
		this.gcStageWeightAmount.UnitField = null;
		this.gcStageWeightAmount.Visible = true;
		this.gcStageWeightAmount.VisibleIndex = 6;
		this.gcStageWeightAmount.Width = 115;
		this.gcStageShowBPDocs.Caption = "Show BP Docs";
		this.gcStageShowBPDocs.ColumnEdit = this.rICShowBPDoc;
		this.gcStageShowBPDocs.CurrencyDateField = null;
		this.gcStageShowBPDocs.CurrencyField = null;
		this.gcStageShowBPDocs.CurrencyFixed = null;
		this.gcStageShowBPDocs.CurrencyFixedRate = 0.0;
		this.gcStageShowBPDocs.CurrencyRateField = null;
		this.gcStageShowBPDocs.FieldName = "DocChkbox";
		this.gcStageShowBPDocs.MinWidth = 23;
		this.gcStageShowBPDocs.Name = "gcStageShowBPDocs";
		this.gcStageShowBPDocs.PrimaryKey = false;
		this.gcStageShowBPDocs.UnitField = null;
		this.gcStageShowBPDocs.Visible = true;
		this.gcStageShowBPDocs.VisibleIndex = 7;
		this.gcStageShowBPDocs.Width = 87;
		this.rICShowBPDoc.AutoHeight = false;
		this.rICShowBPDoc.DisplayValueChecked = "Y";
		this.rICShowBPDoc.DisplayValueUnchecked = "N";
		this.rICShowBPDoc.Name = "rICShowBPDoc";
		this.rICShowBPDoc.ValueChecked = "Y";
		this.rICShowBPDoc.ValueUnchecked = "N";
		this.gcStageDocType.Caption = "Documents Type";
		this.gcStageDocType.ColumnEdit = this.rep_stage_DocType;
		this.gcStageDocType.CurrencyDateField = null;
		this.gcStageDocType.CurrencyField = null;
		this.gcStageDocType.CurrencyFixed = null;
		this.gcStageDocType.CurrencyFixedRate = 0.0;
		this.gcStageDocType.CurrencyRateField = null;
		this.gcStageDocType.FieldName = "ObjType";
		this.gcStageDocType.MinWidth = 23;
		this.gcStageDocType.Name = "gcStageDocType";
		this.gcStageDocType.PrimaryKey = false;
		this.gcStageDocType.UnitField = null;
		this.gcStageDocType.Visible = true;
		this.gcStageDocType.VisibleIndex = 8;
		this.gcStageDocType.Width = 108;
		this.rep_stage_DocType.AutoHeight = false;
		this.rep_stage_DocType.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.rep_stage_DocType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_stage_DocType.DisplayMember = "Name";
		this.rep_stage_DocType.IsSearchControl = false;
		this.rep_stage_DocType.Name = "rep_stage_DocType";
		this.rep_stage_DocType.NullText = "";
		this.rep_stage_DocType.PopupFormMinSize = new System.Drawing.Size(140, 209);
		this.rep_stage_DocType.PopupSizeable = false;
		this.rep_stage_DocType.PopupWidth = 140;
		this.rep_stage_DocType.ShowFooter = false;
		this.rep_stage_DocType.ShowHeader = false;
		this.rep_stage_DocType.ValueMember = "ObjType";
		this.gcStageDocNo.Caption = "Doc. No.";
		this.gcStageDocNo.ColumnEdit = this.rIBStageDocNo;
		this.gcStageDocNo.CurrencyDateField = null;
		this.gcStageDocNo.CurrencyField = null;
		this.gcStageDocNo.CurrencyFixed = null;
		this.gcStageDocNo.CurrencyFixedRate = 0.0;
		this.gcStageDocNo.CurrencyRateField = null;
		this.gcStageDocNo.FieldName = "DocNumber";
		this.gcStageDocNo.MinWidth = 23;
		this.gcStageDocNo.Name = "gcStageDocNo";
		this.gcStageDocNo.PrimaryKey = false;
		this.gcStageDocNo.UnitField = null;
		this.gcStageDocNo.Visible = true;
		this.gcStageDocNo.VisibleIndex = 9;
		this.rIBStageDocNo.AutoHeight = false;
		editorButtonImageOptions3.Image = Apzon.Forms.Properties.Resources.right;
		this.rIBStageDocNo.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance4, appearanceHovered4, appearancePressed4, appearanceDisabled4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rIBStageDocNo.Name = "rIBStageDocNo";
		this.rIBStageDocNo.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rIBStageDocNo_ButtonPressed);
		this.gcStageOwner.Caption = "Owner";
		this.gcStageOwner.ColumnEdit = this.rep_Stage_Owner;
		this.gcStageOwner.CurrencyDateField = null;
		this.gcStageOwner.CurrencyField = null;
		this.gcStageOwner.CurrencyFixed = null;
		this.gcStageOwner.CurrencyFixedRate = 0.0;
		this.gcStageOwner.CurrencyRateField = null;
		this.gcStageOwner.FieldName = "Owner";
		this.gcStageOwner.MinWidth = 23;
		this.gcStageOwner.Name = "gcStageOwner";
		this.gcStageOwner.PrimaryKey = false;
		this.gcStageOwner.UnitField = null;
		this.gcStageOwner.Visible = true;
		this.gcStageOwner.VisibleIndex = 10;
		this.gcStageOwner.Width = 132;
		this.rep_Stage_Owner.AutoHeight = false;
		this.rep_Stage_Owner.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.rep_Stage_Owner.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Stage_Owner.DisplayMember = "Name";
		this.rep_Stage_Owner.IsSearchControl = false;
		this.rep_Stage_Owner.Name = "rep_Stage_Owner";
		this.rep_Stage_Owner.NullText = "";
		this.rep_Stage_Owner.ShowFooter = false;
		this.rep_Stage_Owner.ShowHeader = false;
		this.rep_Stage_Owner.ValueMember = "Code";
		this.gcStageActivity.Caption = "Activity";
		this.gcStageActivity.ColumnEdit = this.rIBActivities;
		this.gcStageActivity.CurrencyDateField = null;
		this.gcStageActivity.CurrencyField = null;
		this.gcStageActivity.CurrencyFixed = null;
		this.gcStageActivity.CurrencyFixedRate = 0.0;
		this.gcStageActivity.CurrencyRateField = null;
		this.gcStageActivity.FieldName = "Linked";
		this.gcStageActivity.MinWidth = 23;
		this.gcStageActivity.Name = "gcStageActivity";
		this.gcStageActivity.OptionsColumn.ReadOnly = true;
		this.gcStageActivity.PrimaryKey = false;
		this.gcStageActivity.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcStageActivity.UnitField = null;
		this.gcStageActivity.Visible = true;
		this.gcStageActivity.VisibleIndex = 11;
		this.gcStageActivity.Width = 66;
		this.rIBActivities.Appearance.Image = (System.Drawing.Image)resources.GetObject("rIBActivities.Appearance.Image");
		this.rIBActivities.Appearance.Options.UseImage = true;
		this.rIBActivities.AutoHeight = false;
		editorButtonImageOptions4.Image = Apzon.Forms.Properties.Resources.right;
		this.rIBActivities.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance5, appearanceHovered5, appearancePressed5, appearanceDisabled5, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rIBActivities.Name = "rIBActivities";
		this.rIBActivities.ReadOnly = true;
		this.rIBActivities.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
		this.rIBActivities.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rIBActivities_ButtonPressed);
		this.gcStageOpprId.Caption = "OpprId";
		this.gcStageOpprId.CurrencyDateField = null;
		this.gcStageOpprId.CurrencyField = null;
		this.gcStageOpprId.CurrencyFixed = null;
		this.gcStageOpprId.CurrencyFixedRate = 0.0;
		this.gcStageOpprId.CurrencyRateField = null;
		this.gcStageOpprId.FieldName = "OpprId";
		this.gcStageOpprId.MinWidth = 23;
		this.gcStageOpprId.Name = "gcStageOpprId";
		this.gcStageOpprId.PrimaryKey = false;
		this.gcStageOpprId.UnitField = null;
		this.gcStageOpprId.Width = 87;
		this.gcStageLine.Caption = "Line";
		this.gcStageLine.CurrencyDateField = null;
		this.gcStageLine.CurrencyField = null;
		this.gcStageLine.CurrencyFixed = null;
		this.gcStageLine.CurrencyFixedRate = 0.0;
		this.gcStageLine.CurrencyRateField = null;
		this.gcStageLine.FieldName = "Line";
		this.gcStageLine.MinWidth = 23;
		this.gcStageLine.Name = "gcStageLine";
		this.gcStageLine.PrimaryKey = false;
		this.gcStageLine.UnitField = null;
		this.gcStageLine.Width = 87;
		this.tab_ItemDetail.Controls.Add(this.grdItemDetail);
		this.tab_ItemDetail.EditControl = null;
		this.tab_ItemDetail.EditValue = "Item detail";
		this.tab_ItemDetail.FieldName = "tab_ItemDetail";
		this.tab_ItemDetail.IsSearchControl = false;
		this.tab_ItemDetail.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.tab_ItemDetail.Name = "tab_ItemDetail";
		this.tab_ItemDetail.ReadOnly = false;
		this.tab_ItemDetail.SettingInVisible = false;
		this.tab_ItemDetail.SettingReadonly = false;
		this.tab_ItemDetail.Size = new System.Drawing.Size(990, 225);
		this.tab_ItemDetail.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_ItemDetail.Text = "Item detail";
		this.grdItemDetail.AllowPasteExcel = false;
		this.grdItemDetail.ClipboardData = null;
		this.grdItemDetail.ContextMenuStrip = this.ctm_Items;
		this.grdItemDetail.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grdItemDetail.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.grdItemDetail.Location = new System.Drawing.Point(0, 0);
		this.grdItemDetail.MainView = this.gv_ItemDetail;
		this.grdItemDetail.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.grdItemDetail.Name = "grdItemDetail";
		this.grdItemDetail.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.rIBItemCode, this.repositoryItemDateEdit3, this.rep_bte_ItemAttFile });
		this.grdItemDetail.Size = new System.Drawing.Size(990, 225);
		this.grdItemDetail.TabIndex = 18;
		this.grdItemDetail.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_ItemDetail });
		this.ctm_Items.ImageScalingSize = new System.Drawing.Size(20, 20);
		this.ctm_Items.Items.AddRange(new System.Windows.Forms.ToolStripItem[5] { this.deleteRowsToolStripMenuItem, this.createSalesQuotationToolStripMenuItem, this.viewChangeLogToolStripMenuItem, this.viewForeCastToolStripMenuItem, this.addRowToolStripMenuItem });
		this.ctm_Items.Name = "contextMenuStripItems";
		this.ctm_Items.Size = new System.Drawing.Size(231, 124);
		this.ctm_Items.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripItems_Opening);
		this.deleteRowsToolStripMenuItem.Name = "deleteRowsToolStripMenuItem";
		this.deleteRowsToolStripMenuItem.Size = new System.Drawing.Size(230, 24);
		this.deleteRowsToolStripMenuItem.Text = "Delete Rows";
		this.deleteRowsToolStripMenuItem.Click += new System.EventHandler(this.deleteRowsToolStripMenuItem_Click);
		this.createSalesQuotationToolStripMenuItem.Name = "createSalesQuotationToolStripMenuItem";
		this.createSalesQuotationToolStripMenuItem.Size = new System.Drawing.Size(230, 24);
		this.createSalesQuotationToolStripMenuItem.Text = "Create Sales Quotation";
		this.createSalesQuotationToolStripMenuItem.Visible = false;
		this.createSalesQuotationToolStripMenuItem.Click += new System.EventHandler(this.createSalesQuotationToolStripMenuItem_Click);
		this.viewChangeLogToolStripMenuItem.Name = "viewChangeLogToolStripMenuItem";
		this.viewChangeLogToolStripMenuItem.Size = new System.Drawing.Size(230, 24);
		this.viewChangeLogToolStripMenuItem.Text = "View Change Log";
		this.viewChangeLogToolStripMenuItem.Click += new System.EventHandler(this.viewChangeLogToolStripMenuItem_Click);
		this.viewForeCastToolStripMenuItem.Name = "viewForeCastToolStripMenuItem";
		this.viewForeCastToolStripMenuItem.Size = new System.Drawing.Size(230, 24);
		this.viewForeCastToolStripMenuItem.Text = "View ForeCast";
		this.viewForeCastToolStripMenuItem.Click += new System.EventHandler(this.viewForeCastToolStripMenuItem_Click);
		this.addRowToolStripMenuItem.Name = "addRowToolStripMenuItem";
		this.addRowToolStripMenuItem.Size = new System.Drawing.Size(230, 24);
		this.addRowToolStripMenuItem.Text = "Add Row";
		this.addRowToolStripMenuItem.Click += new System.EventHandler(this.addRowToolStripMenuItem_Click);
		this.gv_ItemDetail.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_ItemDetail.ColumnKey = null;
		this.gv_ItemDetail.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[12]
		{
			this.gcItemCode, this.gcItemDesc, this.gcItemQty, this.gcIPercent, this.gcItemPrice, this.gcItemPotentAmount, this.gcItemDeliveryDate, this.gridColumn13, this.gridColumn14, this.gcItemFrgnName,
			this.gcItemAttFile, this.gcItemAttFullPath
		});
		this.gv_ItemDetail.DetailHeight = 431;
		this.gv_ItemDetail.DisableFormSetting = false;
		this.gv_ItemDetail.DisplayException = false;
		this.gv_ItemDetail.EditColumn = null;
		this.gv_ItemDetail.FieldCurrency = null;
		this.gv_ItemDetail.FieldRate = null;
		this.gv_ItemDetail.FormId = "oOppItemDetail";
		this.gv_ItemDetail.GridControl = this.grdItemDetail;
		this.gv_ItemDetail.IgnoreFilter = false;
		this.gv_ItemDetail.IndexEditRow = -1;
		this.gv_ItemDetail.IndicatorWidth = 41;
		this.gv_ItemDetail.IsApzTable = false;
		this.gv_ItemDetail.IsInsertNewRow = false;
		this.gv_ItemDetail.IsLoadingData = false;
		this.gv_ItemDetail.IsMappingContextAction = false;
		this.gv_ItemDetail.ItemId = "@OPR62";
		this.gv_ItemDetail.LanguageSource = Apzon.Commons.Enums.Enums.LanguageSource.Documents;
		this.gv_ItemDetail.LstColumnAllowBatch = null;
		this.gv_ItemDetail.LstColumnNotAllowBatch = null;
		this.gv_ItemDetail.LstColumnNotEditable = null;
		this.gv_ItemDetail.LstColumNotSetting = null;
		this.gv_ItemDetail.Name = "gv_ItemDetail";
		this.gv_ItemDetail.OptionsFind.AllowFindPanel = false;
		this.gv_ItemDetail.OptionsSelection.CheckBoxSelectorColumnWidth = 35;
		this.gv_ItemDetail.OptionsSelection.MultiSelect = true;
		this.gv_ItemDetail.OptionsView.ColumnAutoWidth = false;
		this.gv_ItemDetail.OptionsView.ShowFooter = true;
		this.gv_ItemDetail.OptionsView.ShowGroupPanel = false;
		this.gv_ItemDetail.UseBatchMod = false;
		this.gv_ItemDetail.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gvItemDetail_ShowingEditor);
		this.gv_ItemDetail.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvItemDetail_CellValueChanged);
		this.gcItemCode.Caption = "Item No";
		this.gcItemCode.ColumnEdit = this.rIBItemCode;
		this.gcItemCode.CurrencyDateField = null;
		this.gcItemCode.CurrencyField = null;
		this.gcItemCode.CurrencyFixed = null;
		this.gcItemCode.CurrencyFixedRate = 0.0;
		this.gcItemCode.CurrencyRateField = null;
		this.gcItemCode.FieldName = "ItemCode";
		this.gcItemCode.MinWidth = 23;
		this.gcItemCode.Name = "gcItemCode";
		this.gcItemCode.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcItemCode.PrimaryKey = false;
		this.gcItemCode.UnitField = null;
		this.gcItemCode.Visible = true;
		this.gcItemCode.VisibleIndex = 0;
		this.gcItemCode.Width = 99;
		this.rIBItemCode.AutoHeight = false;
		editorButtonImageOptions5.Image = Apzon.Forms.Properties.Resources.right;
		this.rIBItemCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions5, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance6, appearanceHovered6, appearancePressed6, appearanceDisabled6, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rIBItemCode.Name = "rIBItemCode";
		this.rIBItemCode.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rIBItemCode_ButtonPressed);
		this.gcItemDesc.Caption = "Description";
		this.gcItemDesc.CurrencyDateField = null;
		this.gcItemDesc.CurrencyField = null;
		this.gcItemDesc.CurrencyFixed = null;
		this.gcItemDesc.CurrencyFixedRate = 0.0;
		this.gcItemDesc.CurrencyRateField = null;
		this.gcItemDesc.FieldName = "ItemName";
		this.gcItemDesc.MinWidth = 23;
		this.gcItemDesc.Name = "gcItemDesc";
		this.gcItemDesc.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcItemDesc.PrimaryKey = false;
		this.gcItemDesc.UnitField = null;
		this.gcItemDesc.Visible = true;
		this.gcItemDesc.VisibleIndex = 1;
		this.gcItemDesc.Width = 103;
		this.gcItemQty.Caption = "Quantity";
		this.gcItemQty.CurrencyDateField = null;
		this.gcItemQty.CurrencyField = null;
		this.gcItemQty.CurrencyFixed = null;
		this.gcItemQty.CurrencyFixedRate = 0.0;
		this.gcItemQty.CurrencyRateField = null;
		this.gcItemQty.FieldName = "Quantity";
		this.gcItemQty.MinWidth = 23;
		this.gcItemQty.Name = "gcItemQty";
		this.gcItemQty.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcItemQty.PrimaryKey = false;
		this.gcItemQty.UnitField = null;
		this.gcItemQty.Visible = true;
		this.gcItemQty.VisibleIndex = 2;
		this.gcItemQty.Width = 141;
		this.gcIPercent.Caption = "%";
		this.gcIPercent.CurrencyDateField = null;
		this.gcIPercent.CurrencyField = null;
		this.gcIPercent.CurrencyFixed = null;
		this.gcIPercent.CurrencyFixedRate = 0.0;
		this.gcIPercent.CurrencyRateField = null;
		this.gcIPercent.FieldName = "IPercent";
		this.gcIPercent.MinWidth = 23;
		this.gcIPercent.Name = "gcIPercent";
		this.gcIPercent.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcIPercent.PrimaryKey = false;
		this.gcIPercent.UnitField = null;
		this.gcIPercent.Visible = true;
		this.gcIPercent.VisibleIndex = 3;
		this.gcIPercent.Width = 42;
		this.gcItemPrice.Caption = "Price";
		this.gcItemPrice.CurrencyDateField = null;
		this.gcItemPrice.CurrencyField = null;
		this.gcItemPrice.CurrencyFixed = null;
		this.gcItemPrice.CurrencyFixedRate = 0.0;
		this.gcItemPrice.CurrencyRateField = null;
		this.gcItemPrice.DisplayFormat.FormatString = "n2";
		this.gcItemPrice.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gcItemPrice.FieldName = "Price";
		this.gcItemPrice.MinWidth = 23;
		this.gcItemPrice.Name = "gcItemPrice";
		this.gcItemPrice.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcItemPrice.PrimaryKey = false;
		this.gcItemPrice.UnitField = null;
		this.gcItemPrice.Visible = true;
		this.gcItemPrice.VisibleIndex = 4;
		this.gcItemPrice.Width = 110;
		this.gcItemPotentAmount.Caption = "Potential Amount";
		this.gcItemPotentAmount.CurrencyDateField = null;
		this.gcItemPotentAmount.CurrencyField = null;
		this.gcItemPotentAmount.CurrencyFixed = null;
		this.gcItemPotentAmount.CurrencyFixedRate = 0.0;
		this.gcItemPotentAmount.CurrencyRateField = null;
		this.gcItemPotentAmount.DisplayFormat.FormatString = "n2";
		this.gcItemPotentAmount.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gcItemPotentAmount.FieldName = "PotentAmount";
		this.gcItemPotentAmount.MinWidth = 23;
		this.gcItemPotentAmount.Name = "gcItemPotentAmount";
		this.gcItemPotentAmount.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcItemPotentAmount.PrimaryKey = false;
		this.gcItemPotentAmount.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[1]
		{
			new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "PotentAmount", "{0:n2}")
		});
		this.gcItemPotentAmount.UnitField = null;
		this.gcItemPotentAmount.Visible = true;
		this.gcItemPotentAmount.VisibleIndex = 5;
		this.gcItemPotentAmount.Width = 115;
		this.gcItemDeliveryDate.Caption = "Delivery Date";
		this.gcItemDeliveryDate.ColumnEdit = this.repositoryItemDateEdit3;
		this.gcItemDeliveryDate.CurrencyDateField = null;
		this.gcItemDeliveryDate.CurrencyField = null;
		this.gcItemDeliveryDate.CurrencyFixed = null;
		this.gcItemDeliveryDate.CurrencyFixedRate = 0.0;
		this.gcItemDeliveryDate.CurrencyRateField = null;
		this.gcItemDeliveryDate.FieldName = "DeliveryDate";
		this.gcItemDeliveryDate.MinWidth = 23;
		this.gcItemDeliveryDate.Name = "gcItemDeliveryDate";
		this.gcItemDeliveryDate.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcItemDeliveryDate.PrimaryKey = false;
		this.gcItemDeliveryDate.UnitField = null;
		this.gcItemDeliveryDate.Visible = true;
		this.gcItemDeliveryDate.VisibleIndex = 6;
		this.gcItemDeliveryDate.Width = 87;
		this.repositoryItemDateEdit3.AutoHeight = false;
		this.repositoryItemDateEdit3.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEdit3.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEdit3.Name = "repositoryItemDateEdit3";
		this.gridColumn13.Caption = "OpprId";
		this.gridColumn13.CurrencyDateField = null;
		this.gridColumn13.CurrencyField = null;
		this.gridColumn13.CurrencyFixed = null;
		this.gridColumn13.CurrencyFixedRate = 0.0;
		this.gridColumn13.CurrencyRateField = null;
		this.gridColumn13.FieldName = "OpportId";
		this.gridColumn13.MinWidth = 23;
		this.gridColumn13.Name = "gridColumn13";
		this.gridColumn13.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gridColumn13.PrimaryKey = false;
		this.gridColumn13.UnitField = null;
		this.gridColumn13.Width = 87;
		this.gridColumn14.Caption = "Line";
		this.gridColumn14.CurrencyDateField = null;
		this.gridColumn14.CurrencyField = null;
		this.gridColumn14.CurrencyFixed = null;
		this.gridColumn14.CurrencyFixedRate = 0.0;
		this.gridColumn14.CurrencyRateField = null;
		this.gridColumn14.FieldName = "Line";
		this.gridColumn14.MinWidth = 23;
		this.gridColumn14.Name = "gridColumn14";
		this.gridColumn14.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gridColumn14.PrimaryKey = false;
		this.gridColumn14.UnitField = null;
		this.gridColumn14.Width = 87;
		this.gcItemFrgnName.Caption = "Foreign Name";
		this.gcItemFrgnName.CurrencyDateField = null;
		this.gcItemFrgnName.CurrencyField = null;
		this.gcItemFrgnName.CurrencyFixed = null;
		this.gcItemFrgnName.CurrencyFixedRate = 0.0;
		this.gcItemFrgnName.CurrencyRateField = null;
		this.gcItemFrgnName.FieldName = "FrgnName";
		this.gcItemFrgnName.MinWidth = 23;
		this.gcItemFrgnName.Name = "gcItemFrgnName";
		this.gcItemFrgnName.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcItemFrgnName.PrimaryKey = false;
		this.gcItemFrgnName.UnitField = null;
		this.gcItemFrgnName.Width = 87;
		this.gcItemAttFile.Caption = "Attch. File";
		this.gcItemAttFile.ColumnEdit = this.rep_bte_ItemAttFile;
		this.gcItemAttFile.CurrencyDateField = null;
		this.gcItemAttFile.CurrencyField = null;
		this.gcItemAttFile.CurrencyFixed = null;
		this.gcItemAttFile.CurrencyFixedRate = 0.0;
		this.gcItemAttFile.CurrencyRateField = null;
		this.gcItemAttFile.FieldName = "AttFile";
		this.gcItemAttFile.MinWidth = 23;
		this.gcItemAttFile.Name = "gcItemAttFile";
		this.gcItemAttFile.PrimaryKey = false;
		this.gcItemAttFile.UnitField = null;
		this.gcItemAttFile.Visible = true;
		this.gcItemAttFile.VisibleIndex = 7;
		this.gcItemAttFile.Width = 87;
		this.rep_bte_ItemAttFile.AutoHeight = false;
		editorButtonImageOptions6.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_bte_ItemAttFile.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions6, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance7, appearanceHovered7, appearancePressed7, appearanceDisabled7, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_bte_ItemAttFile.Name = "rep_bte_ItemAttFile";
		this.rep_bte_ItemAttFile.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_bte_itemAttFile_ButtonPressed);
		this.gcItemAttFullPath.Caption = "Att Full Path";
		this.gcItemAttFullPath.CurrencyDateField = null;
		this.gcItemAttFullPath.CurrencyField = null;
		this.gcItemAttFullPath.CurrencyFixed = null;
		this.gcItemAttFullPath.CurrencyFixedRate = 0.0;
		this.gcItemAttFullPath.CurrencyRateField = null;
		this.gcItemAttFullPath.FieldName = "AttFullPath";
		this.gcItemAttFullPath.MinWidth = 23;
		this.gcItemAttFullPath.Name = "gcItemAttFullPath";
		this.gcItemAttFullPath.PrimaryKey = false;
		this.gcItemAttFullPath.UnitField = null;
		this.gcItemAttFullPath.Width = 87;
		this.tab_Competitor.Controls.Add(this.grdCompetitor);
		this.tab_Competitor.EditControl = null;
		this.tab_Competitor.EditValue = "Competitor";
		this.tab_Competitor.FieldName = "tab_Competitor";
		this.tab_Competitor.IsSearchControl = false;
		this.tab_Competitor.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.tab_Competitor.Name = "tab_Competitor";
		this.tab_Competitor.ReadOnly = false;
		this.tab_Competitor.SettingInVisible = false;
		this.tab_Competitor.SettingReadonly = false;
		this.tab_Competitor.Size = new System.Drawing.Size(990, 225);
		this.tab_Competitor.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Competitor.Text = "Competitor";
		this.grdCompetitor.AllowPasteExcel = false;
		this.grdCompetitor.ClipboardData = null;
		this.grdCompetitor.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grdCompetitor.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.grdCompetitor.Location = new System.Drawing.Point(0, 0);
		this.grdCompetitor.MainView = this.gv_Competitor;
		this.grdCompetitor.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.grdCompetitor.Name = "grdCompetitor";
		this.grdCompetitor.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[4] { this.rICKWon, this.rICThreatLvl, this.rILCompetitor, this.rep_luk_OPR3_ThreatLevl });
		this.grdCompetitor.Size = new System.Drawing.Size(990, 225);
		this.grdCompetitor.TabIndex = 18;
		this.grdCompetitor.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Competitor });
		this.gv_Competitor.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Competitor.ColumnKey = null;
		this.gv_Competitor.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[6] { this.gcCompetitorName, this.gcCompetitorThreatLevel, this.gcCompetitorRemark, this.gcCompetitorWon, this.gcCompeOpportId, this.gcCompeLine });
		this.gv_Competitor.DetailHeight = 431;
		this.gv_Competitor.DisableFormSetting = false;
		this.gv_Competitor.DisplayException = false;
		this.gv_Competitor.EditColumn = null;
		this.gv_Competitor.FieldCurrency = null;
		this.gv_Competitor.FieldRate = null;
		this.gv_Competitor.FormId = "oOppItemDetail";
		this.gv_Competitor.GridControl = this.grdCompetitor;
		this.gv_Competitor.IgnoreFilter = false;
		this.gv_Competitor.IndexEditRow = -1;
		this.gv_Competitor.IndicatorWidth = 41;
		this.gv_Competitor.IsApzTable = false;
		this.gv_Competitor.IsInsertNewRow = false;
		this.gv_Competitor.IsLoadingData = false;
		this.gv_Competitor.IsMappingContextAction = false;
		this.gv_Competitor.ItemId = "OCOMPETIOR";
		this.gv_Competitor.LanguageSource = Apzon.Commons.Enums.Enums.LanguageSource.Documents;
		this.gv_Competitor.LstColumnAllowBatch = null;
		this.gv_Competitor.LstColumnNotAllowBatch = null;
		this.gv_Competitor.LstColumnNotEditable = null;
		this.gv_Competitor.LstColumNotSetting = null;
		this.gv_Competitor.Name = "gv_Competitor";
		this.gv_Competitor.OptionsSelection.CheckBoxSelectorColumnWidth = 35;
		this.gv_Competitor.OptionsSelection.MultiSelect = true;
		this.gv_Competitor.OptionsView.ColumnAutoWidth = false;
		this.gv_Competitor.OptionsView.ShowGroupPanel = false;
		this.gv_Competitor.UseBatchMod = false;
		this.gv_Competitor.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.GvChild_ShowingEditor);
		this.gv_Competitor.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.Controls_EditValueChanged);
		this.gcCompetitorName.Caption = "Name";
		this.gcCompetitorName.ColumnEdit = this.rILCompetitor;
		this.gcCompetitorName.CurrencyDateField = null;
		this.gcCompetitorName.CurrencyField = null;
		this.gcCompetitorName.CurrencyFixed = null;
		this.gcCompetitorName.CurrencyFixedRate = 0.0;
		this.gcCompetitorName.CurrencyRateField = null;
		this.gcCompetitorName.FieldName = "CompetId";
		this.gcCompetitorName.MinWidth = 23;
		this.gcCompetitorName.Name = "gcCompetitorName";
		this.gcCompetitorName.PrimaryKey = false;
		this.gcCompetitorName.UnitField = null;
		this.gcCompetitorName.Visible = true;
		this.gcCompetitorName.VisibleIndex = 0;
		this.gcCompetitorName.Width = 233;
		this.rILCompetitor.AutoHeight = false;
		this.rILCompetitor.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		editorButtonImageOptions7.Image = Apzon.Forms.Properties.Resources.right;
		this.rILCompetitor.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions7, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance8, appearanceHovered8, appearancePressed8, appearanceDisabled8, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rILCompetitor.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.rILCompetitor.DisplayMember = "Name";
		this.rILCompetitor.IsSearchControl = false;
		this.rILCompetitor.Name = "rILCompetitor";
		this.rILCompetitor.NullText = "";
		this.rILCompetitor.ShowFooter = false;
		this.rILCompetitor.ShowHeader = false;
		this.rILCompetitor.ValueMember = "CompetId";
		this.rILCompetitor.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rILCompetitor_ButtonPressed);
		this.rILCompetitor.EditValueChanged += new System.EventHandler(this.rILCompetitor_EditValueChanged);
		this.gcCompetitorThreatLevel.Caption = "Threat Level";
		this.gcCompetitorThreatLevel.ColumnEdit = this.rep_luk_OPR3_ThreatLevl;
		this.gcCompetitorThreatLevel.CurrencyDateField = null;
		this.gcCompetitorThreatLevel.CurrencyField = null;
		this.gcCompetitorThreatLevel.CurrencyFixed = null;
		this.gcCompetitorThreatLevel.CurrencyFixedRate = 0.0;
		this.gcCompetitorThreatLevel.CurrencyRateField = null;
		this.gcCompetitorThreatLevel.FieldName = "ThreatLevl";
		this.gcCompetitorThreatLevel.MinWidth = 23;
		this.gcCompetitorThreatLevel.Name = "gcCompetitorThreatLevel";
		this.gcCompetitorThreatLevel.PrimaryKey = false;
		this.gcCompetitorThreatLevel.UnitField = null;
		this.gcCompetitorThreatLevel.Visible = true;
		this.gcCompetitorThreatLevel.VisibleIndex = 1;
		this.gcCompetitorThreatLevel.Width = 233;
		this.rep_luk_OPR3_ThreatLevl.AutoHeight = false;
		this.rep_luk_OPR3_ThreatLevl.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.rep_luk_OPR3_ThreatLevl.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_luk_OPR3_ThreatLevl.IsSearchControl = false;
		this.rep_luk_OPR3_ThreatLevl.Name = "rep_luk_OPR3_ThreatLevl";
		this.rep_luk_OPR3_ThreatLevl.NullText = "";
		this.rep_luk_OPR3_ThreatLevl.ShowFooter = false;
		this.rep_luk_OPR3_ThreatLevl.ShowHeader = false;
		this.gcCompetitorRemark.Caption = "Remark";
		this.gcCompetitorRemark.CurrencyDateField = null;
		this.gcCompetitorRemark.CurrencyField = null;
		this.gcCompetitorRemark.CurrencyFixed = null;
		this.gcCompetitorRemark.CurrencyFixedRate = 0.0;
		this.gcCompetitorRemark.CurrencyRateField = null;
		this.gcCompetitorRemark.FieldName = "Memo";
		this.gcCompetitorRemark.MinWidth = 23;
		this.gcCompetitorRemark.Name = "gcCompetitorRemark";
		this.gcCompetitorRemark.PrimaryKey = false;
		this.gcCompetitorRemark.UnitField = null;
		this.gcCompetitorRemark.Visible = true;
		this.gcCompetitorRemark.VisibleIndex = 2;
		this.gcCompetitorRemark.Width = 233;
		this.gcCompetitorWon.Caption = "Won";
		this.gcCompetitorWon.ColumnEdit = this.rICKWon;
		this.gcCompetitorWon.CurrencyDateField = null;
		this.gcCompetitorWon.CurrencyField = null;
		this.gcCompetitorWon.CurrencyFixed = null;
		this.gcCompetitorWon.CurrencyFixedRate = 0.0;
		this.gcCompetitorWon.CurrencyRateField = null;
		this.gcCompetitorWon.FieldName = "Won";
		this.gcCompetitorWon.MinWidth = 23;
		this.gcCompetitorWon.Name = "gcCompetitorWon";
		this.gcCompetitorWon.PrimaryKey = false;
		this.gcCompetitorWon.UnitField = null;
		this.gcCompetitorWon.Visible = true;
		this.gcCompetitorWon.VisibleIndex = 3;
		this.gcCompetitorWon.Width = 234;
		this.rICKWon.AutoHeight = false;
		this.rICKWon.DisplayValueChecked = "Y";
		this.rICKWon.DisplayValueUnchecked = "N";
		this.rICKWon.Name = "rICKWon";
		this.rICKWon.ValueChecked = "Y";
		this.rICKWon.ValueUnchecked = "N";
		this.gcCompeOpportId.Caption = "OpportId";
		this.gcCompeOpportId.CurrencyDateField = null;
		this.gcCompeOpportId.CurrencyField = null;
		this.gcCompeOpportId.CurrencyFixed = null;
		this.gcCompeOpportId.CurrencyFixedRate = 0.0;
		this.gcCompeOpportId.CurrencyRateField = null;
		this.gcCompeOpportId.FieldName = "OpportId";
		this.gcCompeOpportId.MinWidth = 23;
		this.gcCompeOpportId.Name = "gcCompeOpportId";
		this.gcCompeOpportId.PrimaryKey = false;
		this.gcCompeOpportId.UnitField = null;
		this.gcCompeOpportId.Width = 87;
		this.gcCompeLine.Caption = "Line";
		this.gcCompeLine.CurrencyDateField = null;
		this.gcCompeLine.CurrencyField = null;
		this.gcCompeLine.CurrencyFixed = null;
		this.gcCompeLine.CurrencyFixedRate = 0.0;
		this.gcCompeLine.CurrencyRateField = null;
		this.gcCompeLine.FieldName = "Line";
		this.gcCompeLine.MinWidth = 23;
		this.gcCompeLine.Name = "gcCompeLine";
		this.gcCompeLine.PrimaryKey = false;
		this.gcCompeLine.UnitField = null;
		this.gcCompeLine.Width = 87;
		this.rICThreatLvl.AutoHeight = false;
		this.rICThreatLvl.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rICThreatLvl.Items.AddRange(new object[3] { "Low", "Medium", "High" });
		this.rICThreatLvl.Name = "rICThreatLvl";
		this.rICThreatLvl.SelectedIndexChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.tab_Partner.Controls.Add(this.grdPartner);
		this.tab_Partner.EditControl = null;
		this.tab_Partner.EditValue = "Partner";
		this.tab_Partner.FieldName = "tab_Partner";
		this.tab_Partner.IsSearchControl = false;
		this.tab_Partner.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.tab_Partner.Name = "tab_Partner";
		this.tab_Partner.ReadOnly = false;
		this.tab_Partner.SettingInVisible = false;
		this.tab_Partner.SettingReadonly = false;
		this.tab_Partner.Size = new System.Drawing.Size(990, 225);
		this.tab_Partner.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Partner.Text = "Partner";
		this.grdPartner.AllowPasteExcel = false;
		this.grdPartner.ClipboardData = null;
		this.grdPartner.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grdPartner.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.grdPartner.Location = new System.Drawing.Point(0, 0);
		this.grdPartner.MainView = this.gv_Partner;
		this.grdPartner.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.grdPartner.Name = "grdPartner";
		this.grdPartner.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.rILPartner, this.rILPartnerRela, this.rIBRelaPartner });
		this.grdPartner.Size = new System.Drawing.Size(990, 225);
		this.grdPartner.TabIndex = 18;
		this.grdPartner.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Partner });
		this.gv_Partner.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Partner.ColumnKey = null;
		this.gv_Partner.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[6] { this.gcPartnerName, this.gcPartnerRelationShip, this.gcPartnerRelatedBP, this.gcPartnerRemark, this.gcPartnerOpportId, this.gcPartnerLine });
		this.gv_Partner.DetailHeight = 431;
		this.gv_Partner.DisableFormSetting = false;
		this.gv_Partner.DisplayException = false;
		this.gv_Partner.EditColumn = null;
		this.gv_Partner.FieldCurrency = null;
		this.gv_Partner.FieldRate = null;
		this.gv_Partner.FormId = "oOppItemDetail";
		this.gv_Partner.GridControl = this.grdPartner;
		this.gv_Partner.IgnoreFilter = false;
		this.gv_Partner.IndexEditRow = -1;
		this.gv_Partner.IndicatorWidth = 41;
		this.gv_Partner.IsApzTable = false;
		this.gv_Partner.IsInsertNewRow = false;
		this.gv_Partner.IsLoadingData = false;
		this.gv_Partner.IsMappingContextAction = false;
		this.gv_Partner.ItemId = "OPARTNER";
		this.gv_Partner.LanguageSource = Apzon.Commons.Enums.Enums.LanguageSource.Documents;
		this.gv_Partner.LstColumnAllowBatch = null;
		this.gv_Partner.LstColumnNotAllowBatch = null;
		this.gv_Partner.LstColumnNotEditable = null;
		this.gv_Partner.LstColumNotSetting = null;
		this.gv_Partner.Name = "gv_Partner";
		this.gv_Partner.OptionsSelection.CheckBoxSelectorColumnWidth = 35;
		this.gv_Partner.OptionsSelection.MultiSelect = true;
		this.gv_Partner.OptionsView.ColumnAutoWidth = false;
		this.gv_Partner.OptionsView.ShowGroupPanel = false;
		this.gv_Partner.UseBatchMod = false;
		this.gv_Partner.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.GvChild_ShowingEditor);
		this.gv_Partner.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.Controls_EditValueChanged);
		this.gcPartnerName.Caption = "Name";
		this.gcPartnerName.ColumnEdit = this.rILPartner;
		this.gcPartnerName.CurrencyDateField = null;
		this.gcPartnerName.CurrencyField = null;
		this.gcPartnerName.CurrencyFixed = null;
		this.gcPartnerName.CurrencyFixedRate = 0.0;
		this.gcPartnerName.CurrencyRateField = null;
		this.gcPartnerName.FieldName = "ParterId";
		this.gcPartnerName.MinWidth = 23;
		this.gcPartnerName.Name = "gcPartnerName";
		this.gcPartnerName.PrimaryKey = false;
		this.gcPartnerName.UnitField = null;
		this.gcPartnerName.Visible = true;
		this.gcPartnerName.VisibleIndex = 0;
		this.gcPartnerName.Width = 233;
		this.rILPartner.AutoHeight = false;
		this.rILPartner.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		editorButtonImageOptions8.Image = Apzon.Forms.Properties.Resources.right;
		this.rILPartner.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions8, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance9, appearanceHovered9, appearancePressed9, appearanceDisabled9, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rILPartner.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.rILPartner.DisplayMember = "Name";
		this.rILPartner.IsSearchControl = false;
		this.rILPartner.Name = "rILPartner";
		this.rILPartner.NullText = "";
		this.rILPartner.ShowFooter = false;
		this.rILPartner.ShowHeader = false;
		this.rILPartner.ValueMember = "PrtId";
		this.rILPartner.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rILPartner_ButtonPressed);
		this.rILPartner.EditValueChanged += new System.EventHandler(this.rILPartner_EditValueChanged);
		this.gcPartnerRelationShip.Caption = "Relation ship";
		this.gcPartnerRelationShip.ColumnEdit = this.rILPartnerRela;
		this.gcPartnerRelationShip.CurrencyDateField = null;
		this.gcPartnerRelationShip.CurrencyField = null;
		this.gcPartnerRelationShip.CurrencyFixed = null;
		this.gcPartnerRelationShip.CurrencyFixedRate = 0.0;
		this.gcPartnerRelationShip.CurrencyRateField = null;
		this.gcPartnerRelationShip.FieldName = "OrlCode";
		this.gcPartnerRelationShip.MinWidth = 23;
		this.gcPartnerRelationShip.Name = "gcPartnerRelationShip";
		this.gcPartnerRelationShip.PrimaryKey = false;
		this.gcPartnerRelationShip.UnitField = null;
		this.gcPartnerRelationShip.Visible = true;
		this.gcPartnerRelationShip.VisibleIndex = 1;
		this.gcPartnerRelationShip.Width = 233;
		this.rILPartnerRela.AutoHeight = false;
		this.rILPartnerRela.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		editorButtonImageOptions9.Image = Apzon.Forms.Properties.Resources.right;
		this.rILPartnerRela.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions9, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance10, appearanceHovered10, appearancePressed10, appearanceDisabled10, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rILPartnerRela.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("OrlDesc", "Desc")
		});
		this.rILPartnerRela.DisplayMember = "OrlDesc";
		this.rILPartnerRela.IsSearchControl = false;
		this.rILPartnerRela.Name = "rILPartnerRela";
		this.rILPartnerRela.NullText = "";
		this.rILPartnerRela.ShowFooter = false;
		this.rILPartnerRela.ShowHeader = false;
		this.rILPartnerRela.ValueMember = "OrlCode";
		this.rILPartnerRela.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rILPartnerRela_ButtonClick);
		this.rILPartnerRela.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.gcPartnerRelatedBP.Caption = "Related BP";
		this.gcPartnerRelatedBP.ColumnEdit = this.rIBRelaPartner;
		this.gcPartnerRelatedBP.CurrencyDateField = null;
		this.gcPartnerRelatedBP.CurrencyField = null;
		this.gcPartnerRelatedBP.CurrencyFixed = null;
		this.gcPartnerRelatedBP.CurrencyFixedRate = 0.0;
		this.gcPartnerRelatedBP.CurrencyRateField = null;
		this.gcPartnerRelatedBP.FieldName = "RelatCard";
		this.gcPartnerRelatedBP.MinWidth = 23;
		this.gcPartnerRelatedBP.Name = "gcPartnerRelatedBP";
		this.gcPartnerRelatedBP.PrimaryKey = false;
		this.gcPartnerRelatedBP.UnitField = null;
		this.gcPartnerRelatedBP.Visible = true;
		this.gcPartnerRelatedBP.VisibleIndex = 2;
		this.gcPartnerRelatedBP.Width = 233;
		this.rIBRelaPartner.AutoHeight = false;
		this.rIBRelaPartner.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rIBRelaPartner.Name = "rIBRelaPartner";
		this.rIBRelaPartner.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rIBRelaPartner_ButtonPressed);
		this.rIBRelaPartner.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.gcPartnerRemark.Caption = "Remark";
		this.gcPartnerRemark.CurrencyDateField = null;
		this.gcPartnerRemark.CurrencyField = null;
		this.gcPartnerRemark.CurrencyFixed = null;
		this.gcPartnerRemark.CurrencyFixedRate = 0.0;
		this.gcPartnerRemark.CurrencyRateField = null;
		this.gcPartnerRemark.FieldName = "Memo";
		this.gcPartnerRemark.MinWidth = 23;
		this.gcPartnerRemark.Name = "gcPartnerRemark";
		this.gcPartnerRemark.PrimaryKey = false;
		this.gcPartnerRemark.UnitField = null;
		this.gcPartnerRemark.Visible = true;
		this.gcPartnerRemark.VisibleIndex = 3;
		this.gcPartnerRemark.Width = 233;
		this.gcPartnerOpportId.Caption = "OpportId";
		this.gcPartnerOpportId.CurrencyDateField = null;
		this.gcPartnerOpportId.CurrencyField = null;
		this.gcPartnerOpportId.CurrencyFixed = null;
		this.gcPartnerOpportId.CurrencyFixedRate = 0.0;
		this.gcPartnerOpportId.CurrencyRateField = null;
		this.gcPartnerOpportId.FieldName = "OpportId";
		this.gcPartnerOpportId.MinWidth = 23;
		this.gcPartnerOpportId.Name = "gcPartnerOpportId";
		this.gcPartnerOpportId.PrimaryKey = false;
		this.gcPartnerOpportId.UnitField = null;
		this.gcPartnerOpportId.Width = 87;
		this.gcPartnerLine.Caption = "Line";
		this.gcPartnerLine.CurrencyDateField = null;
		this.gcPartnerLine.CurrencyField = null;
		this.gcPartnerLine.CurrencyFixed = null;
		this.gcPartnerLine.CurrencyFixedRate = 0.0;
		this.gcPartnerLine.CurrencyRateField = null;
		this.gcPartnerLine.FieldName = "Line";
		this.gcPartnerLine.MinWidth = 23;
		this.gcPartnerLine.Name = "gcPartnerLine";
		this.gcPartnerLine.PrimaryKey = false;
		this.gcPartnerLine.UnitField = null;
		this.gcPartnerLine.Width = 87;
		this.tab_Summary.Controls.Add(this.cbSummaryDocType);
		this.tab_Summary.Controls.Add(this.lb_DocumentNo);
		this.tab_Summary.Controls.Add(this.lb_DocumentType);
		this.tab_Summary.Controls.Add(this.rdOppoStatus);
		this.tab_Summary.Controls.Add(this.grdReason);
		this.tab_Summary.Controls.Add(this.lb_OpportunityStatus);
		this.tab_Summary.Controls.Add(this.txtDocNo);
		this.tab_Summary.EditControl = null;
		this.tab_Summary.EditValue = "Summary";
		this.tab_Summary.FieldName = "tab_Summary";
		this.tab_Summary.IsSearchControl = false;
		this.tab_Summary.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.tab_Summary.Name = "tab_Summary";
		this.tab_Summary.ReadOnly = false;
		this.tab_Summary.SettingInVisible = false;
		this.tab_Summary.SettingReadonly = false;
		this.tab_Summary.Size = new System.Drawing.Size(990, 225);
		this.tab_Summary.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Summary.Text = "Summary";
		this.cbSummaryDocType.EditControl = null;
		this.cbSummaryDocType.FieldName = null;
		this.cbSummaryDocType.IsSearchControl = false;
		this.cbSummaryDocType.Location = new System.Drawing.Point(289, 71);
		this.cbSummaryDocType.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.cbSummaryDocType.Name = "cbSummaryDocType";
		this.cbSummaryDocType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbSummaryDocType.Properties.Items.AddRange(new object[9] { "", "Sale Quotations", "Sales Orders", "Deliveries", "A/R Invoices", "Purchase Quotation", "Purchase Orders", "Goods Receipt PO", "A/P Invoices" });
		this.cbSummaryDocType.SettingInVisible = false;
		this.cbSummaryDocType.SettingReadonly = false;
		this.cbSummaryDocType.Size = new System.Drawing.Size(117, 22);
		this.cbSummaryDocType.TabIndex = 23;
		this.cbSummaryDocType.SelectedIndexChanged += new System.EventHandler(this.cbSummaryDocType_SelectedIndexChanged);
		this.cbSummaryDocType.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.cbSummaryDocType_EditValueChanging);
		this.lb_DocumentNo.EditControl = null;
		this.lb_DocumentNo.EditValue = "Document No.";
		this.lb_DocumentNo.FieldName = null;
		this.lb_DocumentNo.HideWhenEmpty = false;
		this.lb_DocumentNo.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_DocumentNo.Location = new System.Drawing.Point(20, 103);
		this.lb_DocumentNo.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
		this.lb_DocumentNo.Name = "lb_DocumentNo";
		this.lb_DocumentNo.ReadOnly = false;
		this.lb_DocumentNo.SettingInVisible = false;
		this.lb_DocumentNo.SettingReadonly = false;
		this.lb_DocumentNo.ShowUnderLine = false;
		this.lb_DocumentNo.Size = new System.Drawing.Size(139, 25);
		this.lb_DocumentNo.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_DocumentNo.TabIndex = 22;
		this.lb_DocumentNo.Utext = "Document No.";
		this.lb_DocumentType.EditControl = null;
		this.lb_DocumentType.EditValue = "Document Type";
		this.lb_DocumentType.FieldName = null;
		this.lb_DocumentType.HideWhenEmpty = false;
		this.lb_DocumentType.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_DocumentType.Location = new System.Drawing.Point(20, 71);
		this.lb_DocumentType.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
		this.lb_DocumentType.Name = "lb_DocumentType";
		this.lb_DocumentType.ReadOnly = false;
		this.lb_DocumentType.SettingInVisible = false;
		this.lb_DocumentType.SettingReadonly = false;
		this.lb_DocumentType.ShowUnderLine = false;
		this.lb_DocumentType.Size = new System.Drawing.Size(139, 25);
		this.lb_DocumentType.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_DocumentType.TabIndex = 21;
		this.lb_DocumentType.Utext = "Document Type";
		this.rdOppoStatus.EditControl = null;
		this.rdOppoStatus.FieldName = null;
		this.rdOppoStatus.IsSearchControl = false;
		this.rdOppoStatus.Location = new System.Drawing.Point(289, 7);
		this.rdOppoStatus.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.rdOppoStatus.Name = "rdOppoStatus";
		this.rdOppoStatus.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.rdOppoStatus.Properties.Appearance.Options.UseBackColor = true;
		this.rdOppoStatus.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.rdOppoStatus.Properties.Columns = 3;
		this.rdOppoStatus.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[3]
		{
			new DevExpress.XtraEditors.Controls.RadioGroupItem("O", "Open"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem("W", "Won"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem("L", "Lost")
		});
		this.rdOppoStatus.SettingInVisible = false;
		this.rdOppoStatus.SettingReadonly = false;
		this.rdOppoStatus.Size = new System.Drawing.Size(264, 32);
		this.rdOppoStatus.TabIndex = 20;
		this.rdOppoStatus.SelectedIndexChanged += new System.EventHandler(this.rdOppoStatus_SelectedIndexChanged);
		this.rdOppoStatus.Enter += new System.EventHandler(this.Controls_Properties_Enter);
		this.grdReason.AllowPasteExcel = false;
		this.grdReason.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grdReason.ClipboardData = null;
		this.grdReason.ContextMenuStrip = this.ctm_Description;
		this.grdReason.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.grdReason.Location = new System.Drawing.Point(600, 10);
		this.grdReason.MainView = this.gv_Reason;
		this.grdReason.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.grdReason.Name = "grdReason";
		this.grdReason.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.rILReason });
		this.grdReason.Size = new System.Drawing.Size(381, 174);
		this.grdReason.TabIndex = 19;
		this.grdReason.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Reason });
		this.ctm_Description.ImageScalingSize = new System.Drawing.Size(20, 20);
		this.ctm_Description.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.removeRowToolStripMenuItem });
		this.ctm_Description.Name = "contextMenuStripDescription";
		this.ctm_Description.Size = new System.Drawing.Size(166, 28);
		this.removeRowToolStripMenuItem.Name = "removeRowToolStripMenuItem";
		this.removeRowToolStripMenuItem.Size = new System.Drawing.Size(165, 24);
		this.removeRowToolStripMenuItem.Text = "Remove Row";
		this.removeRowToolStripMenuItem.Click += new System.EventHandler(this.removeRowToolStripMenuItem_Click);
		this.gv_Reason.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Reason.ColumnKey = null;
		this.gv_Reason.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.gcReasonDesc, this.gcReasonOpportId, this.gcReasonLine });
		this.gv_Reason.DetailHeight = 431;
		this.gv_Reason.DisableFormSetting = false;
		this.gv_Reason.DisplayException = false;
		this.gv_Reason.EditColumn = null;
		this.gv_Reason.FieldCurrency = null;
		this.gv_Reason.FieldRate = null;
		this.gv_Reason.FormId = "oOppItemDetail";
		this.gv_Reason.GridControl = this.grdReason;
		this.gv_Reason.IgnoreFilter = false;
		this.gv_Reason.IndexEditRow = -1;
		this.gv_Reason.IndicatorWidth = 41;
		this.gv_Reason.IsApzTable = false;
		this.gv_Reason.IsInsertNewRow = false;
		this.gv_Reason.IsLoadingData = false;
		this.gv_Reason.IsMappingContextAction = false;
		this.gv_Reason.ItemId = "OREASON";
		this.gv_Reason.LanguageSource = Apzon.Commons.Enums.Enums.LanguageSource.Documents;
		this.gv_Reason.LstColumnAllowBatch = null;
		this.gv_Reason.LstColumnNotAllowBatch = null;
		this.gv_Reason.LstColumnNotEditable = null;
		this.gv_Reason.LstColumNotSetting = null;
		this.gv_Reason.Name = "gv_Reason";
		this.gv_Reason.OptionsSelection.CheckBoxSelectorColumnWidth = 35;
		this.gv_Reason.OptionsSelection.MultiSelect = true;
		this.gv_Reason.OptionsView.ColumnAutoWidth = false;
		this.gv_Reason.OptionsView.ShowGroupPanel = false;
		this.gv_Reason.OptionsView.ShowIndicator = false;
		this.gv_Reason.UseBatchMod = false;
		this.gv_Reason.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.GvChild_ShowingEditor);
		this.gv_Reason.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvReason_CellValueChanged);
		this.gcReasonDesc.Caption = "Description";
		this.gcReasonDesc.ColumnEdit = this.rILReason;
		this.gcReasonDesc.CurrencyDateField = null;
		this.gcReasonDesc.CurrencyField = null;
		this.gcReasonDesc.CurrencyFixed = null;
		this.gcReasonDesc.CurrencyFixedRate = 0.0;
		this.gcReasonDesc.CurrencyRateField = null;
		this.gcReasonDesc.FieldName = "ReasondId";
		this.gcReasonDesc.MinWidth = 23;
		this.gcReasonDesc.Name = "gcReasonDesc";
		this.gcReasonDesc.PrimaryKey = false;
		this.gcReasonDesc.UnitField = null;
		this.gcReasonDesc.Visible = true;
		this.gcReasonDesc.VisibleIndex = 0;
		this.gcReasonDesc.Width = 344;
		this.rILReason.AutoHeight = false;
		this.rILReason.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		editorButtonImageOptions10.Image = Apzon.Forms.Properties.Resources.right;
		this.rILReason.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions10, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance11, appearanceHovered11, appearancePressed11, appearanceDisabled11, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rILReason.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Descript", "Name")
		});
		this.rILReason.DisplayMember = "Descript";
		this.rILReason.IsSearchControl = false;
		this.rILReason.Name = "rILReason";
		this.rILReason.NullText = "";
		this.rILReason.ShowFooter = false;
		this.rILReason.ShowHeader = false;
		this.rILReason.ValueMember = "Num";
		this.rILReason.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rILReason_ButtonPressed);
		this.rILReason.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.gcReasonOpportId.Caption = "OpportId";
		this.gcReasonOpportId.CurrencyDateField = null;
		this.gcReasonOpportId.CurrencyField = null;
		this.gcReasonOpportId.CurrencyFixed = null;
		this.gcReasonOpportId.CurrencyFixedRate = 0.0;
		this.gcReasonOpportId.CurrencyRateField = null;
		this.gcReasonOpportId.FieldName = "OpportId";
		this.gcReasonOpportId.MinWidth = 23;
		this.gcReasonOpportId.Name = "gcReasonOpportId";
		this.gcReasonOpportId.PrimaryKey = false;
		this.gcReasonOpportId.UnitField = null;
		this.gcReasonOpportId.Width = 87;
		this.gcReasonLine.Caption = "Line";
		this.gcReasonLine.CurrencyDateField = null;
		this.gcReasonLine.CurrencyField = null;
		this.gcReasonLine.CurrencyFixed = null;
		this.gcReasonLine.CurrencyFixedRate = 0.0;
		this.gcReasonLine.CurrencyRateField = null;
		this.gcReasonLine.FieldName = "Line";
		this.gcReasonLine.MinWidth = 23;
		this.gcReasonLine.Name = "gcReasonLine";
		this.gcReasonLine.PrimaryKey = false;
		this.gcReasonLine.UnitField = null;
		this.gcReasonLine.Width = 87;
		this.txtDocNo.EditControl = null;
		this.txtDocNo.FieldName = null;
		this.txtDocNo.IsSearchControl = false;
		this.txtDocNo.Location = new System.Drawing.Point(289, 100);
		this.txtDocNo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.txtDocNo.Name = "txtDocNo";
		editorButtonImageOptions11.Image = Apzon.Forms.Properties.Resources.right;
		this.txtDocNo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions11, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance12, appearanceHovered12, appearancePressed12, appearanceDisabled12, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.txtDocNo.Properties.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtDocNo_Properties_ButtonPressed);
		this.txtDocNo.SettingInVisible = false;
		this.txtDocNo.SettingReadonly = false;
		this.txtDocNo.Size = new System.Drawing.Size(117, 22);
		this.txtDocNo.TabIndex = 24;
		this.txtDocNo.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.txtDocNo_EditValueChanging);
		this.tab_Attachment.Controls.Add(this.ucAttachment1);
		this.tab_Attachment.EditControl = null;
		this.tab_Attachment.EditValue = "Attachments";
		this.tab_Attachment.FieldName = "tab_Attachment";
		this.tab_Attachment.IsSearchControl = false;
		this.tab_Attachment.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.tab_Attachment.Name = "tab_Attachment";
		this.tab_Attachment.ReadOnly = false;
		this.tab_Attachment.SettingInVisible = false;
		this.tab_Attachment.SettingReadonly = false;
		this.tab_Attachment.Size = new System.Drawing.Size(990, 225);
		this.tab_Attachment.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Attachment.Text = "Attachments";
		this.ucAttachment1.DataSource = null;
		this.ucAttachment1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.ucAttachment1.FormId = "oOppItemDetail";
		this.ucAttachment1.ItemId = "ATC1";
		this.ucAttachment1.Location = new System.Drawing.Point(0, 0);
		this.ucAttachment1.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
		this.ucAttachment1.Name = "ucAttachment1";
		this.ucAttachment1.Size = new System.Drawing.Size(990, 225);
		this.ucAttachment1.TabIndex = 0;
		this.ucAttachment1.Enter += new System.EventHandler(this.Controls_Properties_Enter);
		this.gc_Stage_DocType_Name.Caption = "Name";
		this.gc_Stage_DocType_Name.CurrencyDateField = null;
		this.gc_Stage_DocType_Name.CurrencyField = null;
		this.gc_Stage_DocType_Name.CurrencyFixed = null;
		this.gc_Stage_DocType_Name.CurrencyFixedRate = 0.0;
		this.gc_Stage_DocType_Name.CurrencyRateField = null;
		this.gc_Stage_DocType_Name.FieldName = "Name";
		this.gc_Stage_DocType_Name.Name = "gc_Stage_DocType_Name";
		this.gc_Stage_DocType_Name.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gc_Stage_DocType_Name.PrimaryKey = false;
		this.gc_Stage_DocType_Name.UnitField = null;
		this.gc_Stage_DocType_Name.Visible = true;
		this.gc_Stage_DocType_Name.VisibleIndex = 0;
		this.gc_Stage_DocType_Name.Width = 120;
		this.lb_FromDate.EditControl = null;
		this.lb_FromDate.EditValue = "From Date";
		this.lb_FromDate.FieldName = null;
		this.lb_FromDate.HideWhenEmpty = false;
		this.lb_FromDate.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_FromDate.Location = new System.Drawing.Point(9, 9);
		this.lb_FromDate.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
		this.lb_FromDate.Name = "lb_FromDate";
		this.lb_FromDate.ReadOnly = false;
		this.lb_FromDate.SettingInVisible = false;
		this.lb_FromDate.SettingReadonly = false;
		this.lb_FromDate.ShowUnderLine = true;
		this.lb_FromDate.Size = new System.Drawing.Size(93, 25);
		this.lb_FromDate.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_FromDate.TabIndex = 23;
		this.lb_FromDate.Utext = "From Date";
		this.cbStage.EditControl = null;
		this.cbStage.FieldName = null;
		this.cbStage.IsSearchControl = true;
		this.cbStage.Location = new System.Drawing.Point(562, 31);
		this.cbStage.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.cbStage.Name = "cbStage";
		this.cbStage.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbStage.Properties.IsSearchControl = false;
		this.cbStage.Properties.NullText = "";
		this.cbStage.SettingInVisible = false;
		this.cbStage.SettingReadonly = false;
		this.cbStage.Size = new System.Drawing.Size(167, 22);
		this.cbStage.TabIndex = 25;
		this.cbOpportStatus.EditControl = null;
		this.cbOpportStatus.FieldName = null;
		this.cbOpportStatus.IsSearchControl = true;
		this.cbOpportStatus.Location = new System.Drawing.Point(562, 5);
		this.cbOpportStatus.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.cbOpportStatus.Name = "cbOpportStatus";
		this.cbOpportStatus.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbOpportStatus.Properties.IsSearchControl = false;
		this.cbOpportStatus.Properties.NullText = "";
		this.cbOpportStatus.SettingInVisible = false;
		this.cbOpportStatus.SettingReadonly = false;
		this.cbOpportStatus.Size = new System.Drawing.Size(167, 22);
		this.cbOpportStatus.TabIndex = 26;
		this.cbMainSalesEmp.EditControl = null;
		this.cbMainSalesEmp.FieldName = null;
		this.cbMainSalesEmp.IsSearchControl = true;
		this.cbMainSalesEmp.Location = new System.Drawing.Point(884, 5);
		this.cbMainSalesEmp.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.cbMainSalesEmp.Name = "cbMainSalesEmp";
		this.cbMainSalesEmp.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbMainSalesEmp.Properties.IsSearchControl = false;
		this.cbMainSalesEmp.Properties.NullText = "";
		this.cbMainSalesEmp.SettingInVisible = false;
		this.cbMainSalesEmp.SettingReadonly = false;
		this.cbMainSalesEmp.Size = new System.Drawing.Size(117, 22);
		this.cbMainSalesEmp.TabIndex = 27;
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(9, 66);
		this.splitContainerControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.grdDoc);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.xtraTabControl1);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(992, 475);
		this.splitContainerControl1.SplitterPosition = 208;
		this.splitContainerControl1.TabIndex = 29;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.txtNoOfTrans.EditControl = null;
		this.txtNoOfTrans.FieldName = null;
		this.txtNoOfTrans.IsSearchControl = true;
		this.txtNoOfTrans.Location = new System.Drawing.Point(344, 4);
		this.txtNoOfTrans.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.txtNoOfTrans.Name = "txtNoOfTrans";
		this.txtNoOfTrans.Properties.Mask.EditMask = "n0";
		this.txtNoOfTrans.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
		this.txtNoOfTrans.Properties.Mask.UseMaskAsDisplayFormat = true;
		this.txtNoOfTrans.SettingInVisible = false;
		this.txtNoOfTrans.SettingReadonly = false;
		this.txtNoOfTrans.Size = new System.Drawing.Size(106, 22);
		this.txtNoOfTrans.TabIndex = 30;
		this.gcOwner.Caption = "Owner";
		this.gcOwner.ColumnEdit = this.rep_Owner;
		this.gcOwner.CurrencyDateField = null;
		this.gcOwner.CurrencyField = null;
		this.gcOwner.CurrencyFixed = null;
		this.gcOwner.CurrencyFixedRate = 0.0;
		this.gcOwner.CurrencyRateField = null;
		this.gcOwner.FieldName = "Owner";
		this.gcOwner.MinWidth = 25;
		this.gcOwner.Name = "gcOwner";
		this.gcOwner.PrimaryKey = false;
		this.gcOwner.UnitField = null;
		this.gcOwner.Visible = true;
		this.gcOwner.VisibleIndex = 6;
		this.gcOwner.Width = 94;
		this.rep_Owner.AutoHeight = false;
		this.rep_Owner.AutoLoadDataSource = true;
		editorButtonImageOptions12.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions3.Image");
		this.rep_Owner.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance13, appearanceHovered13, appearancePressed13, appearanceDisabled13, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions12, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance14, appearanceHovered14, appearancePressed14, appearanceDisabled14, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_Owner.ContainerControl = this;
		this.rep_Owner.DisplayMember = "firstName";
		this.rep_Owner.DisplayMember2 = "lastName";
		this.rep_Owner.LoadDataSourceWhenEnter = false;
		this.rep_Owner.Name = "rep_Owner";
		this.rep_Owner.SeparateCharacterForMulti = ';';
		this.rep_Owner.SeparateString = ", ";
		this.rep_Owner.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.Employees;
		this.rep_Owner.TypeDrillDown = Apzon.Commons.ApzObjectTypes.oEmployeesInfo;
		this.rep_Owner.UsingAutoCompleteBox = false;
		this.rep_Owner.UsingDataSource = true;
		this.rep_Owner.ValidateWhenNoDataSource = false;
		this.rep_Owner.ValueMember = "EmpId";
		base.AutoScaleDimensions = new System.Drawing.SizeF(7f, 16f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1029, 586);
		base.Controls.Add(this.txtNoOfTrans);
		base.Controls.Add(this.cbMainSalesEmp);
		base.Controls.Add(this.cbOpportStatus);
		base.Controls.Add(this.cbStage);
		base.Controls.Add(this.btEditBusPartner);
		base.Controls.Add(this.dateFrom);
		base.Controls.Add(this.dateTo);
		base.Controls.Add(this.splitContainerControl1);
		base.Controls.Add(this.lb_FromDate);
		base.Controls.Add(this.btRalatedDoc);
		base.Controls.Add(this.btRelatedAct);
		base.Controls.Add(this.btCancel);
		base.Controls.Add(this.btAdd);
		base.Controls.Add(this.btSearch);
		base.Controls.Add(this.lb_MainSalesEmp);
		base.Controls.Add(this.lb_Stage);
		base.Controls.Add(this.lb_OpporStatus);
		base.Controls.Add(this.lb_BusinessPartner);
		base.Controls.Add(this.lb_NoOfTrans);
		base.Controls.Add(this.lb_ToDate);
		base.FormId = "oOppItemDetail";
		base.IconOptions.ShowIcon = false;
		base.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		base.Name = "frm_SalesOpportunity";
		this.Text = "Sales Opportunity";
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btEditBusPartner.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dateFrom.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dateFrom.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dateTo.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dateTo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).EndInit();
		this.ctm_Doc.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Luk_OOPR_OpprType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rIBBussPartner).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rILSalesEmp).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_luk_OOPR_Status).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit1.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rIPClosing).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).EndInit();
		this.xtraTabControl1.ResumeLayout(false);
		this.tab_Potential.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_InterestRange).EndInit();
		this.ctm_InterestRange.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_InterestRange).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_luk_IntrestRange_IntId).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_chk_IntrestRange_Prmry).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_LevelOfInterest.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calcWeightAmount.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calcGrossProfitTotal.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calcGrossProfit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calcPotentialAmount.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.datePredCloDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.datePredCloDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbClosingType.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtPredictCloseIn.Properties).EndInit();
		this.tab_General.ResumeLayout(false);
		this.tab_General.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.bteCFL_PrjCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bteCFL_ChnCrdCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lukDN_ChnCrdCon.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lukDN_Industry.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lukDN_Source.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.memo_Memo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ChnCrdName.Properties).EndInit();
		this.tab_Stage.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdStage).EndInit();
		this.ctm_Stage.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Stage).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit4.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit2.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rILStaSalesEmpl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rILStage).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rICShowBPDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_stage_DocType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rIBStageDocNo).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Stage_Owner).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rIBActivities).EndInit();
		this.tab_ItemDetail.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdItemDetail).EndInit();
		this.ctm_Items.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_ItemDetail).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rIBItemCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit3.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_bte_ItemAttFile).EndInit();
		this.tab_Competitor.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdCompetitor).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Competitor).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rILCompetitor).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_luk_OPR3_ThreatLevl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rICKWon).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rICThreatLvl).EndInit();
		this.tab_Partner.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdPartner).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Partner).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rILPartner).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rILPartnerRela).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rIBRelaPartner).EndInit();
		this.tab_Summary.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.cbSummaryDocType.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rdOppoStatus.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grdReason).EndInit();
		this.ctm_Description.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Reason).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rILReason).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtDocNo.Properties).EndInit();
		this.tab_Attachment.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.cbStage.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbOpportStatus.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbMainSalesEmp.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txtNoOfTrans.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Owner).EndInit();
		base.ResumeLayout(false);
	}
}
