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
using Apzon.Controls.Editor.MyLookUpEdit;
using Apzon.Controls.Editor.TextEditExPrice;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;
using Apzon.Entities.Models.Production;
using Apzon.Forms.BusinessMaster;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Forms.Inventory.InventoryTransaction;
using Apzon.Forms.Properties;
using Apzon.Forms.SalesOppotunity;
using Apzon.Forms.UserControls;
using Apzon.Forms.UserControls.BatchAndSerial;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTab;

namespace Apzon.Forms.Interfaces.FormInventory;

public class VirFormInventoryTranfer : XtraFormEx
{
	public delegate void ShowForms(XtraFormEx frm);

	public bool IsCreateFromInvInWarehouse;

	public DocumentTable DocsFromIiWhs = new DocumentTable();

	public DocumentLineTable LinesFromIiWhs = new DocumentLineTable();

	protected Enums.SaveAction _saveAction = Enums.SaveAction.SaveAndSearch;

	public List<Tuple<GridViewEx, string>> FormItem = new List<Tuple<GridViewEx, string>>();

	protected string FormDocumentTable;

	protected string FormLineTable;

	protected string FormController;

	public DocumentRow ObjFocus;

	public DocumentLineTable LinesCopy = new DocumentLineTable();

	public int ObjTypeFrom;

	public ApzObjectTypes ObjectType = ApzObjectTypes.oStockTransfer;

	public ApzObjectTypes FromType;

	public ShowForms FrmShowChildrenForm;

	protected string JournalRemarksDefault = "";

	public string FromDocEntry = "";

	public DataTable _currentRefDoc = new DataTable();

	public int CurrentFileEntry;

	public int NewFileEntry;

	private bool _isSearching;

	private string _sourceActionField = "";

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient(ApzObjectTypes.oStockTransfer);

	protected new bool IsLoadForm;

	public Enums.ChangeLog ChangeLogType;

	protected Enums.FormViewModeType OpenFormViewMode = Enums.FormViewModeType.View;

	private DataTable _originalDocTable;

	protected readonly Image _btnSaveImg;

	private bool _loadingDetail;

	private static readonly string[] _lstFieldLineNotEditable = new string[25]
	{
		"LineNum", "BaseType", "BaseLine", "BaseEntry", "BaseRef", "TargetType", "TrgetEntry", "OpenQty", "OpenInvQty", "LineStatus",
		"UomEntry", "InvntSttus", "QuantityOnWareHouse", "ManBtchSerial", "ManBatchSerial", "UgpCode", "QuantityOnStock", "IsCommited", "OnOrder", "IsCommitedInWareHouse",
		"OnOrderInWareHouse", "BOMTreeType", "TreeType", "AvgPrice", "CmpltQty"
	};

	private static readonly string[] _lstDocFieldNotEditable = new string[19]
	{
		"DocEntry", "DocStatus", "DocStatusName", "UserSign", "UserSign2", "CreateDate", "CreateTS", "UpdateDate", "UpdateTS", "TransId",
		"InvntSttus", "WddStatus", "DocTotal", "DocTotalFC", "IsDraft", "draftKey", "AtcEntry", "OpenForLaC", "DataSource"
	};

	private static readonly string[] _lstEditableAfterAdd = new string[3] { "TaxDate", "JrnlMemo", "Comments" };

	private DataTable _dtSeries;

	private bool _saving;

	public SplitContainerControl splitContainerControl1;

	public XtraTabControlEx xtraTabControl1;

	public XtraTabPageEx xtraTabPage1;

	public GridControlEx grd_Lines;

	public GridViewEx gv_Lines;

	public GridColumnEx gcLineItemCode;

	public GridColumnEx gcLineDscription;

	public GridColumnEx gcLineQuantity;

	public GridColumnEx gcLinePrice;

	public GridColumnEx gcLineTotal;

	public GridColumnEx gcLineWhsCode;

	public GridColumnEx gcLineNum;

	public ContextMenuStrip contextMenuStrip_Document;

	public ToolStripMenuItem addNewToolStripMenuItem_Document;

	public ContextMenuStrip contextMenuStrip1_DocLine;

	public ToolStripMenuItem cmtDuplicate_Return;

	public ToolStripMenuItem ctmCancel_Doc;

	public ToolStripMenuItem cmtClose_Doc;

	public GridColumnEx gcLineStatus;

	public XtraTabPageEx xtraTabPage2;

	public XtraTabPageEx xtraTabPage3;

	public GridControlEx grd_Logistic;

	public GridViewEx gv_Logistic;

	public GridColumnEx gcShipType;

	public RepositoryItemLookUpEdit rlpShipType;

	public GridColumnEx gcShipTo;

	public RepositoryItemLookUpEdit rlpShipTo;

	public GridColumnEx gccBillTo;

	public RepositoryItemLookUpEdit rlpBillTo;

	public GridColumnEx gcConfirmed;

	public GridColumnEx gcAddress;

	public GridColumnEx gcAddress2;

	public GridControlEx grd_Doc;

	public GridViewEx gv_Doc;

	public GridColumnEx gcDocNum;

	public GridColumnEx gcDocCardCode;

	public GridColumnEx gcDocCardName;

	public GridColumnEx gcDocCntctCode;

	public GridColumnEx gcDocStatus;

	public GridColumnEx gcDocTaxDate;

	public GridColumnEx gcDocDueDate;

	public GridColumnEx gcDocDate;

	public GridColumnEx gcDocComments;

	public GridColumnEx gcDocSeries;

	public GridColumnEx gcDocPriceList;

	public GridColumnEx gcLineFromWhsCode;

	private IContainer components;

	private ToolStripMenuItem ctm_NewTask;

	private ToolStripMenuItem newActivityToolStripMenuItem1;

	private XtraTabPageEx xtraTabPage4;

	public GridColumnEx gcDocToWhs;

	public GridColumnEx gcDocFromWhs;

	public GridColumnEx gcDocFromBranch;

	public GridColumnEx gcDocShipTo;

	public GridColumnEx gcDocSalesEmployee;

	public GridColumnEx gcDocJournalRemarks;

	public GridColumnEx gcDocPickPackRemarks;

	public GridColumnEx gcLineOpenQty;

	public GridColumnEx gcLineBarCode;

	public GridColumnEx gcLineVendorCatNo;

	public GridColumnEx gcLineSerialNo;

	public GridColumnEx gcLineFactor1;

	public GridColumnEx gcLineFactor2;

	public GridColumnEx gcLineFactor3;

	public GridColumnEx gcLineFactor4;

	public GridColumnEx gcLineInStock;

	public GridColumnEx gcLineCommitted;

	public GridColumnEx gcLineOrdered;

	public GridColumnEx gcLineProject;

	public GridColumnEx gcLineBlockNo;

	public GridColumnEx gcLineImportLog;

	public GridColumnEx gcLineBaseRow;

	public GridColumnEx gcLineBaseRef;

	public GridColumnEx gcLineInventoryUoM;

	public GridColumnEx gcLineUoMName;

	public GridColumnEx gcLineItemsperUnit;

	public GridColumnEx gcLineDistrRule;

	public GridColumnEx gcLineInvQty;

	public GridColumnEx gcLineUoMCode;

	public GridColumnEx gcLineOpenInvQty;

	public GridColumnEx gcLineQtyinWhse;

	public GridColumnEx gcLineCommittedQtyWhse;

	public GridColumnEx gcLineOrderedQtyWhse;

	public GridColumnEx gcDocTotal;

	public GridColumnEx gcLineUoMGroup;

	protected ToolStripMenuItem toolStripMenuItem_AddNewDocLine;

	protected ToolStripMenuItem deleteToolStripMenuItem_DocLine;

	protected ToolStripMenuItem cmtDuplicate_Line;

	protected ToolStripMenuItem insertRowToolStripMenuItem;

	private TextEditEx txtDocNum;

	private LabelLine labelLine1;

	public LabelLine lb_FromDate;

	public SimpleButtonEx btnSearch;

	public LabelLine lb_ToDate;

	public LabelLine lb_DocStatus;

	public LabelLine lb_Number;

	public LabelLine lb_PriceList;

	private LookUpEditEx txtPriceList;

	private ButtonEditEx txtCardCode;

	private DateEditEx txtFromDate;

	private DateEditEx txtToDate;

	public SimpleButtonEx btn_Cancel;

	private GridSplitContainer gridSplitContainer2;

	protected LookUpEditEx txtDocStatus;

	public DropDownButtonEx btnCopyFrom;

	protected RepositoryItemLookUpEdit rDocSeries;

	private GridColumnEx gcLineOpenCreQty;

	protected ToolStripMenuItem baseDocumentToolStripMenuItem;

	protected ToolStripMenuItem targetDocumentToolStripMenuItem;

	protected ToolStripMenuItem baseDocumentToolStripMenuItem1;

	protected ToolStripMenuItem targetDocumentToolStripMenuItem1;

	protected RepositoryItemButtonEdit rItemCodeLine;

	protected RepositoryItemCalcEdit rLineCal;

	protected RepositoryItemButtonEdit rDocCustomerCode;

	protected RepositoryItemDateEdit rDate;

	protected RepositoryItemButtonEdit rDocFromWhsDoc;

	protected RepositoryItemLookUpEdit rDocStatus;

	protected RepositoryItemLookUpEdit rDocBranch;

	protected RepositoryItemLookUpEdit rDocSalesEmployee;

	protected RepositoryItemButtonEdit rLineBarCode;

	protected RepositoryItemLookUpEdit rLineUseBaseUn;

	protected RepositoryItemButtonEdit rLineDistrRule;

	protected RepositoryItemLookUpEdit rDocPriceList;

	protected RepositoryItemButtonEdit rDocContactPersion;

	protected RepositoryItemButtonEdit rLineWareHouse;

	protected RepositoryItemLookUpEdit rLineStatus;

	protected RepositoryItemButtonEdit rLineUoMCode;

	protected RepositoryItemLookUpEdit rLineUoMGroup;

	private GridSplitContainer gridSplitContainer1;

	protected ToolStripMenuItem closeLineToolStripMenuItem;

	private SplitContainerControl splitContainerLine;

	private RepositoryItemCalcEdit rDocCal;

	private GridColumnEx gcLineTaxCode;

	protected ucBatchAndSerial ucBatchAndSerial1;

	protected ToolStripMenuItem batchSerialNumberToolStripMenuItem;

	private GridColumnEx gcLineCurrency;

	private RepositoryItemLookUpEdit rLineCurrency;

	private GridColumnEx gcDocCurrency;

	private RepositoryItemLookUpEdit rDocCur;

	private ToolStripMenuItem ctm_NewEvent;

	private ToolStripMenuItem toolStripMenuItem1;

	private RepositoryItemTextEditExPrice rPriceEdit;

	protected ToolStripMenuItem saveAsDraftToolStripMenuItem;

	protected RepositoryItemButtonEditCFL rep_LineProject;

	protected RepositoryItemLookUpEdit rLineTaxCode;

	private GridColumnEx gcBranch;

	private RepositoryItemLookUpEdit rBranch;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx1;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx2;

	private GridColumnEx gcLineBaseType;

	private GridColumnEx gcLineBaseEntry;

	protected ToolStripMenuItem saveAsDraftToolStripMenuItem1;

	private GridColumnEx gcLineAvgPrice;

	private GridColumnEx gcLinePriceBefDi;

	private RepositoryItemTextEditExPrice repositoryItemTextEditExPrice2;

	private ToolStripMenuItem viewDiagramToolStripMenuItem;

	private RepositoryItemTextEditExPrice repositoryItemTextEditExPrice1;

	private GridColumnEx gcLineSpecPrice;

	private RepositoryItemLookUpEditSimple rLineSpecPrice;

	private RepositoryItemCheckEdit rLogicticsConfirm;

	protected ucAttachment ucAttachment1;

	protected DropDownButtonEx btn_Save;

	protected ToolStripMenuItem ctm_Reverse;

	public double ProductionQuantity { get; set; }

	public DataTable OpenFormDocument { get; set; }

	public DataTable OpenFormLines { get; set; }

	public DataTable OpenFormBatchSerial { get; set; }

	public ContextActionMappingImpl[] MappingDocument { get; set; }

	public ContextActionMappingImpl[] MappingLines { get; set; }

	public ContextActionMappingImpl[] MappingBatchSerial { get; set; }

	public string FormLayoutCode { get; set; }

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		if ((FormAuth == Enums.FormAuthorities.NoAuthorities || FormAuth == Enums.FormAuthorities.Read) && value != Enums.FormViewModeType.Find)
		{
			_formMode = Enums.FormViewModeType.View;
			btn_Save.Text = Apzon.Commons.Properties.Resources.ButtonOK;
			btn_Save.ImageOptions.Image = null;
			btn_Save.DropDownArrowStyle = DropDownArrowStyle.Hide;
			return;
		}
		if (_dtSeries != null && _formMode == Enums.FormViewModeType.Add && value != Enums.FormViewModeType.Add)
		{
			rDocSeries.DataSource = _dtSeries;
		}
		_formMode = value;
		ucBatchAndSerial1.SetFormViewMode(value);
		LoadFormView();
		LoadDropDownSave(value);
		if (value == Enums.FormViewModeType.Find)
		{
			txtCardCode.Properties.Appearance.BackColor = Color.BlanchedAlmond;
			txtDocNum.Properties.Appearance.BackColor = Color.BlanchedAlmond;
			txtDocStatus.Properties.Appearance.BackColor = Color.BlanchedAlmond;
			txtFromDate.Properties.Appearance.BackColor = Color.BlanchedAlmond;
			txtPriceList.Properties.Appearance.BackColor = Color.BlanchedAlmond;
			txtToDate.Properties.Appearance.BackColor = Color.BlanchedAlmond;
		}
		else
		{
			txtCardCode.Properties.Appearance.BackColor = Color.White;
			txtDocNum.Properties.Appearance.BackColor = Color.White;
			txtDocStatus.Properties.Appearance.BackColor = Color.White;
			txtFromDate.Properties.Appearance.BackColor = Color.White;
			txtPriceList.Properties.Appearance.BackColor = Color.White;
			txtToDate.Properties.Appearance.BackColor = Color.White;
		}
	}

	public VirFormInventoryTranfer(string docEntry, bool flag)
	{
		InitializeComponent();
		LoadButtonCopy();
		_btnSaveImg = btn_Save.ImageOptions.Image;
		OnLoadAsync += VirFormInventoryTranfer_Load;
		FromDocEntry = docEntry;
	}

	public VirFormInventoryTranfer()
	{
		InitializeComponent();
		LoadButtonCopy();
		_btnSaveImg = btn_Save.ImageOptions.Image;
		OnLoadAsync += VirFormInventoryTranfer_Load;
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private async Task VirFormInventoryTranfer_Load(object sender, EventArgs e)
	{
		_ = 3;
		try
		{
			EnableDisableControl(isEnable: false);
			if ((FromType != ApzObjectTypes.oNone && FromType != ObjectType) || IsCreateFromInvInWarehouse || OpenFormViewMode == Enums.FormViewModeType.Add)
			{
				FormMode = Enums.FormViewModeType.Add;
			}
			else
			{
				FormMode = Enums.FormViewModeType.View;
			}
			ucBatchAndSerial ucBatchAndSerial2 = ucBatchAndSerial1;
			ucBatchAndSerial2.DBatchSerialChanged = (DataBatchSerialChanged)Delegate.Combine(ucBatchAndSerial2.DBatchSerialChanged, new DataBatchSerialChanged(BatchSerialDataChanged));
			gcDocFromBranch.Visible = GlobalData.IsMultiBranch;
			int num = 0;
			DataTable dataTable = GlobalData.AdditionalGeneralSettings.Tables[0];
			if (dataTable.IsNotNull() && dataTable.Columns.Contains("SearchDate"))
			{
				num = Function.ParseInt(dataTable.Rows[0]["SearchDate"]);
			}
			if (num < 0)
			{
				num = ((Configuration.SearchDate > 0) ? Configuration.SearchDate : 15);
			}
			txtFromDate.EditValue = DateTime.Today.AddDays(-num);
			txtToDate.EditValue = DateTime.Today.AddDays(num);
			txtDocStatus.EditValue = "O";
			ucAttachment ucAttachment2 = ucAttachment1;
			ucAttachment2.BrowsedFile = (Action<List<DataRow>>)Delegate.Combine(ucAttachment2.BrowsedFile, new Action<List<DataRow>>(Attachment_BrowseFile));
			ucAttachment ucAttachment3 = ucAttachment1;
			ucAttachment3.DeletedFile = (Action)Delegate.Combine(ucAttachment3.DeletedFile, new Action(Attachment_DeleteFile));
			InitFormatControl();
			Dictionary<string, GridViewEx> dictionary = new Dictionary<string, GridViewEx>();
			dictionary[FormDocumentTable] = gv_Doc;
			dictionary[FormLineTable] = gv_Lines;
			LoadComb();
			await Task.WhenAll(WinFuntions.LoadUdfColumns(gv_Doc, FormDocumentTable, dictionary), WinFuntions.LoadUdfColumns(gv_Lines, FormLineTable, dictionary));
			CreateColumnsDistRule(gv_Lines);
			ucBatchAndSerial1.DataSourceBatchAndSerial = DocumentsHelper.BatchSerialNumbersStruct.Copy();
			ucBatchAndSerial1.DataSourceBinAllowLine = DocumentsHelper.BatchSerialNumbersStruct.Copy();
			if (FromType != ApzObjectTypes.oNone && FromType != ObjectType)
			{
				await LoadDataCopyFrom(FromDocEntry, showCustomDataLine: false);
				FromDocEntry = "";
				if (FromType == ApzObjectTypes.oDrafts)
				{
					Text = $"{Text} - Draft";
					ucAttachment1.LoadAttachments(Function.ParseInt(gv_Doc.GetFocusedRowCellValue("AtcEntry")));
					_onlyChangeRemark = true;
				}
				ucBatchAndSerial1.FinishEditorBatchAndSerial();
				return;
			}
			if (IsCreateFromInvInWarehouse)
			{
				IsLoadForm = true;
				grd_Doc.DataSource = DocsFromIiWhs;
				grd_Doc.DataSourceTable.AcceptChanges();
				grd_Lines.DataSource = LinesFromIiWhs;
				grd_Lines.DataSourceTable.AcceptChanges();
				IsLoadForm = false;
				return;
			}
			switch (OpenFormViewMode)
			{
			case Enums.FormViewModeType.Add:
				grd_Doc.DataSource = DocumentsHelper.Document.Clone();
				grd_Doc.RefreshDataSource();
				grd_Lines.DataSource = DocumentsHelper.Lines.Clone();
				grd_Lines.RefreshDataSource();
				IsLoadForm = true;
				await AddNewRowDoc();
				IsLoadForm = false;
				if (OpenFormDocument.IsNotNull() && OpenFormLines.IsNotNull())
				{
					gv_Doc.MappingSource(OpenFormDocument, MappingDocument, null, null, null, multirow: false);
					gv_Lines.MappingSource(OpenFormLines, MappingLines, OpenFormBatchSerial, MappingBatchSerial, ucBatchAndSerial1);
					ucBatchAndSerial1.SetVisibleAfterContextAction();
				}
				break;
			case Enums.FormViewModeType.View:
				await LoadDataDoc(Enums.NavigateType.Current);
				FromDocEntry = "";
				break;
			case Enums.FormViewModeType.Find:
			case Enums.FormViewModeType.Update:
			case Enums.FormViewModeType.Busy:
			case Enums.FormViewModeType.Run:
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			EnableDisableControl(isEnable: true);
			WinCommons._CloseProcessing();
		}
	}

	private void Attachment_BrowseFile(List<DataRow> lst)
	{
		if (lst.Any() && FormMode == Enums.FormViewModeType.View)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	private void Attachment_DeleteFile()
	{
		if (FormMode == Enums.FormViewModeType.View)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	private void BatchSerialDataChanged()
	{
		ChangeViewMode();
		_onlyChangeRemark = false;
	}

	private static void CreateColumnsDistRule(GridViewEx gv)
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
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private static void AddDistRule(GridViewEx gv, DataRow r, string fld = "", string caption = "")
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
			string fieldName = string.Format("{0}{1}", "OcrCode" + fld, code);
			RepositoryItemButtonEdit repositoryItemButtonEdit = new RepositoryItemButtonEdit();
			repositoryItemButtonEdit.Name = "rep_line_" + fieldName;
			repositoryItemButtonEdit.Buttons[0].Kind = ButtonPredefines.Search;
			repositoryItemButtonEdit.ButtonClick += (object s, ButtonPressedEventArgs e) =>
			{
				try
				{
					if (!gv.Columns[fieldName].OptionsColumn.ReadOnly)
					{
						frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(code switch
						{
							"2" => Enums.TypeCFL.OrcCode2, 
							"3" => Enums.TypeCFL.OrcCode3, 
							"4" => Enums.TypeCFL.OrcCode4, 
							"5" => Enums.TypeCFL.OrcCode5, 
							_ => Enums.TypeCFL.OrcCode, 
						}, multiSelect: false, "");
						frm_ChooseFromList2.ShowDialog(gv.GridControl.FindForm());
						if (frm_ChooseFromList2.ChooseItem != null)
						{
							DataRow row = ((DataRowView)frm_ChooseFromList2.ChooseItem).Row;
							gv.SetRowCellValue(gv.FocusedRowHandle, fieldName, row[0]);
						}
					}
				}
				catch (Exception ex2)
				{
					WinCommons.ErrorLineNumber(ex2);
				}
			};
			GridColumn gridColumn = gv.Columns[fieldName];
			if (gridColumn == null)
			{
				gridColumn = new GridColumnEx();
				gridColumn.FieldName = fieldName;
				gridColumn.Caption = string.Format("{0} {1}", caption, Function.ToString(r["DimDesc"]));
				gridColumn.Visible = false;
				gridColumn.Width = 100;
				gv.Columns.Add(gridColumn);
			}
			gridColumn.ColumnEdit = repositoryItemButtonEdit;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void rep_bte_FromWhsDoc_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			string text = gv_Doc.GetFocusedRowCellValue(gv_Doc.FocusedColumn.FieldName).ToString();
			if (e.Button.Index == 0)
			{
				frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.Warehouse, multiSelect: false, string.Empty);
				frm_ChooseFromList2.ShowDialog(this);
				if (frm_ChooseFromList2.ChooseItem != null)
				{
					DataRow dataRow = (DataRow)frm_ChooseFromList2.ChooseItem;
					if (Function.ToString(dataRow["WhsCode"]) != "" && !Function.ToString(dataRow["WhsCode"]).Equals(text))
					{
						gv_Doc.SetFocusedRowCellValue(gv_Doc.FocusedColumn.FieldName, dataRow["WhsCode"]);
						grd_Doc.DataSourceTable.AcceptChanges();
					}
				}
			}
			else
			{
				this.DrillDown(ApzObjectTypes.oWarehouses, ApzObjectTypes.oWarehouses, text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public void btn_Cancel_Click(object sender, EventArgs e)
	{
		try
		{
			if ((FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCloseForm, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
			{
				return;
			}
			Close();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		Close();
	}

	public void gv_Doc_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	public void gv_Lines_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	public void gv_Logistic_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	public async void cmtClose_Doc_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (Function.ToString(gv_Doc.GetFocusedRowCellValue(gcDocStatus)).Equals("C"))
			{
				WinCommons.Warning(Enums.MessageCode.ThisDocumentHadBeenClose, "");
			}
			else if (GlobalData.ListAccessForm.Where((AuthsImpl f) => f.menucode == "barSub_CloseDocument" && f.authoriza == "F").FirstOrDefault() == null)
			{
				XtraMessageBoxEx.Show(Enums.MessageCode.NoPermissionPerformAction, Enums.MessageCode.SystemMessage, MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
			else if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCloseForm, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				EnableDisableControl(isEnable: false);
				WinCommons._OpenProcessing();
				string text;
				using (HttpClientHelper<string> http = new HttpClientHelper<string>(Configuration.ApiServer, FormController))
				{
					text = await http.PostAsync("CloseDoc", Function.ParseInt(gv_Doc.GetFocusedRowCellValue("DocEntry")));
				}
				if (string.IsNullOrEmpty(text))
				{
					WinCommons.Success(Enums.MessageCode.CloseSuccess, "");
					FormMode = Enums.FormViewModeType.View;
					await LoadDataDoc(Enums.NavigateType.Current);
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
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			EnableDisableControl(isEnable: true);
			WinCommons._CloseProcessing();
		}
	}

	public async void ctmCancel_Doc_Click(object sender, EventArgs e)
	{
		try
		{
			EnableDisableControl(isEnable: false);
			await CancelDoc();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			EnableDisableControl(isEnable: true);
		}
	}

	protected virtual void cmtDuplicate_Line_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataSourceTable = grd_Lines.DataSourceTable;
			DataRow dataRow = dataSourceTable.NewRow();
			dataRow.ItemArray = gv_Lines.GetFocusedDataRow().ItemArray;
			dataRow["LineNum"] = GetNewLineNum();
			dataSourceTable.Rows.InsertAt(dataRow, gv_Lines.GetFocusedDataSourceRowIndex());
			grd_Lines.DataSourceTable.AcceptChanges();
			_onlyChangeRemark = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		ChangeViewMode();
	}

	private void ChangeViewMode()
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View || FormMode == Enums.FormViewModeType.Find)
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

	public async void cmtDuplicate_Return_Click(object sender, EventArgs e)
	{
		try
		{
			if (gv_Doc.GetFocusedDataRow() != null)
			{
				EnableDisableControl(isEnable: false);
				FormMode = Enums.FormViewModeType.Add;
				DataTable dataTable = grd_Doc.DataSourceTable.Clone();
				DataRow dataRow = dataTable.NewRow();
				dataRow.ItemArray = gv_Doc.GetFocusedDataRow().ItemArray;
				dataRow["DocEntry"] = 0;
				dataRow["DocStatus"] = "O";
				dataRow["UserSign"] = GlobalData.UserSignLogin;
				dataTable.Rows.Add(dataRow);
				DataTable dataTable2 = grd_Lines.DataSourceTable.Copy();
				for (int i = 0; i < dataTable2.Rows.Count; i++)
				{
					dataTable2.Rows[i]["DocEntry"] = 0;
					dataTable2.Rows[i]["BaseRef"] = DBNull.Value;
					dataTable2.Rows[i]["BaseType"] = DBNull.Value;
					dataTable2.Rows[i]["BaseEntry"] = DBNull.Value;
					dataTable2.Rows[i]["BaseLine"] = DBNull.Value;
					dataTable2.Rows[i]["LineStatus"] = "O";
					dataTable2.Rows[i]["Currency"] = GlobalData.MainCurrency;
				}
				IsLoadForm = true;
				grd_Doc.DataSource = dataTable;
				grd_Doc.RefreshDataSource();
				IsLoadForm = false;
				ucBatchAndSerial1.ClearDataSource();
				grd_Lines.DataSource = dataTable2;
				await GetNewDocNum();
				AddNewRowLine();
				_onlyChangeRemark = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			EnableDisableControl(isEnable: true);
		}
	}

	public async void btn_Save_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			switch (FormMode)
			{
			case Enums.FormViewModeType.Add:
				if (FormAuth != Enums.FormAuthorities.Full)
				{
					WinCommons.Error(Enums.MessageCode.YouDoNotPermisionModifiedForm, "");
				}
				else
				{
					await Save(isAsDraft: false);
				}
				break;
			case Enums.FormViewModeType.Find:
				Close();
				break;
			case Enums.FormViewModeType.Update:
				if (FormAuth != Enums.FormAuthorities.Full)
				{
					WinCommons.Error(Enums.MessageCode.YouDoNotPermisionModifiedForm, "");
				}
				else
				{
					await Save(isAsDraft: false);
				}
				break;
			case Enums.FormViewModeType.View:
				Close();
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected virtual void toolStripMenuItem_AddNewDocLine_Click(object sender, EventArgs e)
	{
		try
		{
			AddNewRowLine();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		ChangeViewMode();
	}

	public async void addNewToolStripMenuItem_Document_Click(object sender, EventArgs e)
	{
		try
		{
			EnableDisableControl(isEnable: false);
			FormMode = Enums.FormViewModeType.Add;
			await AddNewRowDoc();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			EnableDisableControl(isEnable: true);
		}
	}

	public void rep_CustomerCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				string txtSeach = "";
				if (gv_Doc.ActiveEditor != null)
				{
					txtSeach = Function.ToString(gv_Doc.ActiveEditor.EditValue);
				}
				frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.CustomerVendor, false, txtSeach, "", Enums.ItemObjectType.NoItemType, null, "");
				frm_CFL2.ShowDialog(this);
				if (frm_CFL2.ChooseItem != null)
				{
					gv_Doc.SetFocusedRowCellValue("CardCode", Function.ToString(frm_CFL2.ChooseItem["CardCode"]));
					grd_Doc.DataSourceTable.AcceptChanges();
				}
			}
			else
			{
				string text = Function.ToString(gv_Doc.GetRowCellValue(gv_Doc.FocusedRowHandle, "CardCode"));
				if (!string.IsNullOrEmpty(text))
				{
					frm_BusinessMasterData frm_BusinessMasterData2 = new frm_BusinessMasterData();
					frm_BusinessMasterData2._copyF_CodeCustomer = text;
					frm_BusinessMasterData2.MdiParent = MdiParent;
					frm_BusinessMasterData2.Show();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public void rep_ItemCode_Line_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			string text = Function.ToString(gv_Lines.GetRowCellValue(gv_Lines.FocusedRowHandle, gcLineItemCode));
			if (e.Button.Index == 0)
			{
				if (gv_Lines.ActiveEditor != null)
				{
					text = Function.ToString(gv_Lines.ActiveEditor.EditValue);
				}
				ChooseFromListLineItem(text);
				AddNewRowLine();
			}
			else if (e.Button.Index == 1 && !string.IsNullOrWhiteSpace(text))
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

	public async void gv_Doc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (_onlyChangeRemark && !new string[3] { "Comments", "JrnlMemo", "REFDOC" }.Contains(e.Column.FieldName))
			{
				_onlyChangeRemark = false;
			}
			if (e.Column.FieldName == gcDocCardCode.FieldName)
			{
				string text = Function.ToString(gv_Doc.GetFocusedRowCellValue(gcDocCardCode));
				DataTable businessPartner = _httpClient.Commons.GetBusinessPartner(text);
				if (businessPartner.IsNotNull() && businessPartner.Rows.Count == 1)
				{
					SetItemToDoc(businessPartner.Rows[0]);
				}
				else
				{
					frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.Customer, multiSelect: false, text);
					frm_ChooseFromList2.ShowDialog(this);
					if (frm_ChooseFromList2.ChooseItem != null)
					{
						BusinessPartnersImpl businessPartnersImpl = (BusinessPartnersImpl)frm_ChooseFromList2.ChooseItem;
						gv_Doc.SetRowCellValue(e.RowHandle, gcDocCardCode, businessPartnersImpl.CardCode);
					}
				}
			}
			else if (e.Column.FieldName.Equals(gcDocFromWhs.FieldName) && !gv_Doc.IsMappingContextAction)
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdateFromWhsToLine, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes && gv_Lines.RowCount > 0)
				{
					for (int i = 0; i < gv_Lines.RowCount; i++)
					{
						gv_Lines.SetRowCellValue(i, gcLineFromWhsCode, e.Value);
					}
					grd_Lines.DataSourceTable.AcceptChanges();
				}
			}
			else if (e.Column.FieldName.Equals(gcDocToWhs.FieldName) && !gv_Doc.IsMappingContextAction)
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdateToWhsToLine, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes && gv_Lines.RowCount > 0)
				{
					for (int j = 0; j < gv_Lines.RowCount; j++)
					{
						gv_Lines.SetRowCellValue(j, gcLineWhsCode, e.Value);
					}
					grd_Lines.DataSourceTable.AcceptChanges();
				}
			}
			else if (e.Column.FieldName.Equals(gcDocSeries.FieldName))
			{
				await GetNewDocNum();
			}
			else if (e.Column.FieldName.Equals(gcDocCurrency.FieldName))
			{
				grd_Doc.DataSourceTable.Rows[gv_Doc.GetFocusedDataSourceRowIndex()]["DocRate"] = _httpClient.ExRateAndIndex.GetExchangeRate(Function.ToString(e.Value), DateTime.Now.Date);
			}
			else if (e.Column.FieldName.Equals(gcDocPriceList.FieldName))
			{
				DataTable dtItem = grd_Lines.DataSourceTable.DefaultView.ToTable(true, "ItemCode");
				DataTable priceForListItem = _httpClient.PriceListSetup.GetPriceForListItem(dtItem, Function.ParseInt(e.Value));
				dtItem = grd_Lines.DataSourceTable;
				double num = 0.0;
				double num2 = 0.0;
				int k;
				for (k = 0; k < dtItem.Rows.Count; k++)
				{
					if (!string.IsNullOrEmpty(Function.ToString(dtItem.Rows[k][gcLineItemCode.FieldName])))
					{
						dtItem.Rows[k]["DiscPrcnt"] = 0;
						DataRow dataRow = priceForListItem.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(dtItem.Rows[k][gcLineItemCode.FieldName]).Equals(Function.ToString(t["ItemCode"])));
						if (dataRow != null)
						{
							DataRow dataRow2 = dtItem.Rows[k];
							object value = (dtItem.Rows[k]["PriceBefDi"] = dataRow["Price"]);
							dataRow2["Price"] = value;
							dtItem.Rows[k]["Currency"] = dataRow["Currency"];
							dtItem.Rows[k]["Rate"] = ((Function.ParseDouble(dataRow["Rate"]) > 1E-07) ? Function.ParseDouble(dataRow["Rate"]) : 1.0);
						}
						else
						{
							DataRow dataRow3 = dtItem.Rows[k];
							object value = (dtItem.Rows[k]["PriceBefDi"] = 0);
							dataRow3["Price"] = value;
							dtItem.Rows[k]["Currency"] = GlobalData.MainCurrency;
							dtItem.Rows[k]["Rate"] = 1;
						}
						double num3 = Function.ParseDouble(dtItem.Rows[k]["Price"]) * Function.ParseDouble(dtItem.Rows[k][gcLineQuantity.FieldName]);
						dtItem.Rows[k][gcLineTotal.FieldName] = num3;
						num += num3 * Function.ParseDouble(dtItem.Rows[k]["Rate"]);
						num2 += num3 * Function.ParseDouble(dtItem.Rows[k]["Rate"]) * (1.0 - Function.ParseDouble(dtItem.Rows[k]["DiscPrcnt"]) / 100.0);
					}
				}
				gv_Doc.SetRowCellValue(e.RowHandle, gcDocTotal, num / Function.ParseDouble(gv_Doc.GetFocusedRowCellValue("DocRate")));
			}
			else if (e.Column == gcDocDate)
			{
				await LoadSeriesByPostingDate();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		ChangeViewMode();
	}

	public void gv_Lines_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			_onlyChangeRemark = false;
			DataTable dataSourceTable = grd_Lines.DataSourceTable;
			int dataSourceRowIndex = gv_Lines.GetDataSourceRowIndex(e.RowHandle);
			if (dataSourceRowIndex < 0)
			{
				return;
			}
			if (e.Column.FieldName == gcLineItemCode.FieldName)
			{
				string itemCode = Function.ToString(e.Value);
				DataTable dataItemCode = GetDataItemCode(itemCode, Function.ParseInt(gv_Doc.GetFocusedRowCellValue(gcDocPriceList)));
				dataSourceTable.Rows[dataSourceRowIndex]["DocEntry"] = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("DocEntry"));
				dataSourceTable.Rows[dataSourceRowIndex]["LineNum"] = GetNewLineNum();
				dataSourceTable.Rows[dataSourceRowIndex]["PQTReqdate"] = DateTime.Today;
				dataSourceTable.Rows[dataSourceRowIndex]["ShipDate"] = DateTime.Today;
				dataSourceTable.Rows[dataSourceRowIndex]["BaseType"] = -1;
				dataSourceTable.Rows[dataSourceRowIndex]["BaseEntry"] = DBNull.Value;
				dataSourceTable.Rows[dataSourceRowIndex]["BaseRef"] = DBNull.Value;
				dataSourceTable.Rows[dataSourceRowIndex]["BaseLine"] = DBNull.Value;
				dataSourceTable.Rows[dataSourceRowIndex]["Quantity"] = 1;
				dataSourceTable.Rows[dataSourceRowIndex]["Currency"] = GlobalData.MainCurrency;
				dataSourceTable.Rows[dataSourceRowIndex]["LineStatus"] = "O";
				dataSourceTable.Rows[dataSourceRowIndex]["AcctCode"] = "";
				dataSourceTable.Rows[dataSourceRowIndex]["UoMCode"] = string.Empty;
				if (dataItemCode == null || dataItemCode.Rows.Count == 0)
				{
					dataSourceTable.Rows[dataSourceRowIndex]["Rate"] = 1;
					dataSourceTable.Rows[dataSourceRowIndex]["Currency"] = GlobalData.MainCurrency;
					dataSourceTable.Rows[dataSourceRowIndex]["ManBtchSerial"] = "N";
					dataSourceTable.Rows[dataSourceRowIndex]["Dscription"] = "";
					dataSourceTable.Rows[dataSourceRowIndex]["PriceBefDi"] = 0;
					gv_Lines.SetRowCellValue(e.RowHandle, gcLinePrice, 0);
					gv_Lines.FocusedColumn = gcLineItemCode;
				}
				else
				{
					dataSourceTable.Rows[dataSourceRowIndex]["ManBtchSerial"] = dataItemCode.Rows[0]["ManBtchSerial"];
					dataSourceTable.Rows[dataSourceRowIndex][gcLineDscription.FieldName] = dataItemCode.Rows[0]["ItemName"];
					string text = Function.ToString(dataItemCode.Rows[0]["BaseCurrency"]);
					dataSourceTable.Rows[dataSourceRowIndex]["Currency"] = (string.IsNullOrEmpty(text) ? GlobalData.MainCurrency : text);
					double num = Function.ParseDouble(dataItemCode.Rows[0]["Rate"]);
					dataSourceTable.Rows[dataSourceRowIndex]["Rate"] = ((num < 1E-06) ? 1.0 : num);
					gv_Lines.SetRowCellValue(e.RowHandle, gcLineFromWhsCode, gv_Doc.GetFocusedRowCellValue(gcDocFromWhs));
					dataSourceTable.Rows[dataSourceRowIndex][gcLineWhsCode.FieldName] = gv_Doc.GetFocusedRowCellValue(gcDocToWhs);
					dataSourceTable.Rows[dataSourceRowIndex]["ItemCode"] = dataItemCode.Rows[0]["ItemCode"];
					dataSourceTable.Rows[dataSourceRowIndex]["SalUnitMsr"] = dataItemCode.Rows[0]["SalUnitMsr"];
					dataSourceTable.Rows[dataSourceRowIndex]["NumInSale"] = dataItemCode.Rows[0]["NumInSale"];
					dataSourceTable.Rows[dataSourceRowIndex][gcLineItemsperUnit.FieldName] = 1;
					dataSourceTable.Rows[dataSourceRowIndex][gcLineUoMGroup.FieldName] = dataItemCode.Rows[0]["UgpEntry"];
					dataSourceTable.Rows[dataSourceRowIndex][gcLineInventoryUoM.FieldName] = "Y";
					gv_Lines.SetRowCellValue(e.RowHandle, gcLineUoMName, dataItemCode.Rows[0]["InvntryUom"]);
					dataSourceTable.Rows[dataSourceRowIndex]["InvUnitMsr"] = dataItemCode.Rows[0]["InvntryUom"];
					dataSourceTable.Rows[dataSourceRowIndex]["NumInInven"] = dataItemCode.Rows[0]["NumInInven"];
					dataSourceTable.Rows[dataSourceRowIndex][gcLineInvQty.FieldName] = Function.ParseDouble(gv_Lines.GetRowCellValue(e.RowHandle, gcLineQuantity)) * Function.ParseDouble(gv_Lines.GetRowCellValue(e.RowHandle, gcLineItemsperUnit));
					dataSourceTable.Rows[dataSourceRowIndex][gcLineOpenInvQty.FieldName] = Function.ParseDouble(gv_Lines.GetRowCellValue(e.RowHandle, gcLineQuantity)) * Function.ParseDouble(gv_Lines.GetRowCellValue(e.RowHandle, gcLineItemsperUnit));
					if (Function.ParseInt(dataItemCode.Rows[0]["UgpEntry"]) == -1)
					{
						dataSourceTable.Rows[dataSourceRowIndex][gcLineUoMCode.FieldName] = gv_Lines.GetFocusedRowCellDisplayText(gcLineUoMGroup);
					}
					else
					{
						dataSourceTable.Rows[dataSourceRowIndex][gcLineUoMCode.FieldName] = WinFuntions.GetUoMCodeByName(Function.ParseInt(dataItemCode.Rows[0]["UgpEntry"]), Function.ToString(dataItemCode.Rows[0]["InvntryUom"]));
					}
					gv_Lines.SetRowCellValue(e.RowHandle, gcLinePriceBefDi, Function.ParseDouble(dataItemCode.Rows[0]["BasePrice"]));
					grd_Lines.DataSourceTable.AcceptChanges();
				}
				AddNewRowLine();
				grd_Lines.DataSourceTable.AcceptChanges();
				ucBatchAndSerial1.SetFocusLine(isChangeItem: true);
				gv_Lines.FocusedColumn = gcLineFromWhsCode;
				gv_Lines.ShowEditor();
			}
			else if (e.Column.FieldName == gcLineUoMCode.FieldName)
			{
				DataTable dataUomByCode = WinFuntions.GetDataUomByCode(Function.ParseInt(gv_Lines.GetRowCellValue(e.RowHandle, gcLineUoMGroup)), Function.ToString(gv_Lines.GetRowCellValue(e.RowHandle, gcLineUoMCode)));
				if (dataUomByCode != null && dataUomByCode.Rows.Count > 0)
				{
					double num2 = Function.ParseDouble(dataSourceTable.Rows[dataSourceRowIndex][gcLineItemsperUnit.FieldName]);
					dataSourceTable.Rows[dataSourceRowIndex]["UomEntry"] = dataUomByCode.Rows[0]["UomEntry"];
					dataSourceTable.Rows[dataSourceRowIndex][gcLineUoMName.FieldName] = dataUomByCode.Rows[0]["UomName"];
					dataSourceTable.Rows[dataSourceRowIndex][gcLineItemsperUnit.FieldName] = Function.ParseDouble(gv_Lines.GetRowCellValue(e.RowHandle, "NumInInven")) * Function.ParseDouble(dataUomByCode.Rows[0]["BaseQty"]) / ((Function.ParseDouble(dataUomByCode.Rows[0]["AltQty"]) <= 0.0) ? 1.0 : Function.ParseDouble(dataUomByCode.Rows[0]["AltQty"]));
					gv_Lines.SetRowCellValue(e.RowHandle, gcLinePriceBefDi, Function.ParseDouble(dataSourceTable.Rows[dataSourceRowIndex][gcLinePriceBefDi.FieldName]) * Function.ParseDouble(dataSourceTable.Rows[dataSourceRowIndex][gcLineItemsperUnit.FieldName]) / num2);
					if (_sourceActionField != gcLineInventoryUoM.FieldName)
					{
						if (Function.ToString(dataUomByCode.Rows[0]["UomName"]).Equals(Function.ToString(gv_Lines.GetRowCellValue(e.RowHandle, "InvUnitMsr"))))
						{
							dataSourceTable.Rows[dataSourceRowIndex][gcLineInventoryUoM.FieldName] = "Y";
							gv_Lines.FocusedColumn = gv_Lines.Columns[gcLineInventoryUoM.FieldName];
						}
						else
						{
							dataSourceTable.Rows[dataSourceRowIndex][gcLineInventoryUoM.FieldName] = "N";
						}
					}
					_sourceActionField = "uomcode";
					gv_Lines.SetRowCellValue(e.RowHandle, gcLineInvQty.FieldName, Function.ParseDouble(gv_Lines.GetRowCellValue(e.RowHandle, gcLineQuantity)) * Function.ParseDouble(gv_Lines.GetRowCellValue(e.RowHandle, gcLineItemsperUnit)));
					gv_Lines.SetRowCellValue(e.RowHandle, gcLineOpenInvQty.FieldName, Function.ParseDouble(gv_Lines.GetRowCellValue(e.RowHandle, gcLineOpenQty)) * Function.ParseDouble(gv_Lines.GetRowCellValue(e.RowHandle, gcLineItemsperUnit)));
					grd_Lines.DataSourceTable.AcceptChanges();
				}
			}
			if (e.Column.FieldName.Equals("NumPerMsr"))
			{
				double num3 = Function.ParseDouble(e.Value);
				gv_Lines.SetRowCellValue(e.RowHandle, gcLinePriceBefDi, num3 * Function.ParseDouble(dataSourceTable.Rows[dataSourceRowIndex][gcLinePriceBefDi.FieldName]) * Function.ParseDouble(gv_Lines.GetRowCellValue(e.RowHandle, gcLineOpenQty)) / Function.ParseDouble(gv_Lines.GetRowCellValue(e.RowHandle, gcLineOpenInvQty)));
				gv_Lines.SetRowCellValue(e.RowHandle, gcLineInvQty.FieldName, Function.ParseDouble(gv_Lines.GetRowCellValue(e.RowHandle, gcLineQuantity)) * num3);
				gv_Lines.SetRowCellValue(e.RowHandle, gcLineOpenInvQty.FieldName, Function.ParseDouble(gv_Lines.GetRowCellValue(e.RowHandle, gcLineOpenQty)) * num3);
			}
			if (e.Column.FieldName.Equals(gcLineInventoryUoM.FieldName))
			{
				_sourceActionField = e.Column.FieldName;
				if (Function.ToString(e.Value).Equals("Y"))
				{
					DataTable inventoryUoMByItemCode = WinFuntions.GetInventoryUoMByItemCode(Function.ToString(gv_Lines.GetRowCellValue(e.RowHandle, gcLineItemCode)), Enums.ItemUomType.InventoryUom);
					if (inventoryUoMByItemCode != null && inventoryUoMByItemCode.Rows.Count > 0)
					{
						gv_Lines.SetRowCellValue(e.RowHandle, gcLineUoMCode, inventoryUoMByItemCode.Rows[0]["UomCode"]);
						gv_Lines.SetRowCellValue(e.RowHandle, gcLineItemsperUnit, 1);
						gv_Lines.SetRowCellValue(e.RowHandle, gcLineUoMName, inventoryUoMByItemCode.Rows[0]["InvntryUom"]);
					}
				}
				else
				{
					gv_Lines.SetRowCellValue(e.RowHandle, gcLineUoMCode, (Function.ParseInt(gv_Lines.GetRowCellValue(e.RowHandle, gcLineUoMGroup)) != -1) ? gv_Lines.GetRowCellValue(e.RowHandle, "SalUnitMsr") : Function.ToString(gv_Lines.GetRowCellDisplayText(e.RowHandle, gcLineUoMGroup)));
					gv_Lines.SetRowCellValue(e.RowHandle, gcLineItemsperUnit, gv_Lines.GetRowCellValue(e.RowHandle, "NumInSale"));
					gv_Lines.SetRowCellValue(e.RowHandle, gcLineUoMName, gv_Lines.GetRowCellValue(e.RowHandle, "SalUnitMsr"));
				}
				gv_Lines.SetRowCellValue(e.RowHandle, gcLineInvQty, Function.ParseDouble(gv_Lines.GetRowCellValue(e.RowHandle, gcLineQuantity)) * Function.ParseDouble(gv_Lines.GetRowCellValue(e.RowHandle, gcLineItemsperUnit)));
				gv_Lines.SetRowCellValue(e.RowHandle, gcLineOpenInvQty, Function.ParseDouble(gv_Lines.GetRowCellValue(e.RowHandle, gcLineQuantity)) * Function.ParseDouble(gv_Lines.GetRowCellValue(e.RowHandle, gcLineItemsperUnit)));
			}
			else if (e.Column.FieldName == gcLineFactor1.FieldName || e.Column.FieldName == gcLineFactor2.FieldName || e.Column.FieldName == gcLineFactor3.FieldName || e.Column.FieldName == gcLineFactor4.FieldName)
			{
				if (Function.ParseDouble(e.Value) <= 0.0)
				{
					WinCommons.Error(Enums.MessageCode.FactorMustBeGreaterThan0, "");
					gv_Lines.FocusedColumn = e.Column;
				}
				else
				{
					gv_Lines.SetRowCellValue(e.RowHandle, gcLineQuantity, Function.ParseDouble(gv_Lines.GetRowCellValue(e.RowHandle, gcLineFactor1)) * Function.ParseDouble(gv_Lines.GetRowCellValue(e.RowHandle, gcLineFactor2)) * Function.ParseDouble(gv_Lines.GetRowCellValue(e.RowHandle, gcLineFactor3)) * Function.ParseDouble(gv_Lines.GetRowCellValue(e.RowHandle, gcLineFactor4)));
				}
				grd_Lines.DataSourceTable.AcceptChanges();
			}
			else if (e.Column.FieldName.Equals("Currency"))
			{
				dataSourceTable.Rows[dataSourceRowIndex]["Rate"] = _httpClient.ExRateAndIndex.GetExchangeRate(Function.ToString(e.Value), DateTime.Now.Date);
			}
			else if (e.Column.FieldName.Equals(gcLineTotal.FieldName))
			{
				double num4 = Function.ParseDouble(e.Value) / Function.ParseDouble(gv_Lines.GetRowCellValue(e.RowHandle, gcLineQuantity));
				double num5 = Function.ParseDouble(gv_Lines.GetRowCellValue(e.RowHandle, "DiscPrcnt")) / 100.0;
				double num6 = ((Math.Abs(num5 - 1.0) > 1E-07) ? (num4 / (1.0 - num5)) : 0.0);
				dataSourceTable.Rows[dataSourceRowIndex]["PriceBefDi"] = num6;
				dataSourceTable.Rows[dataSourceRowIndex]["Price"] = num4;
			}
			else if (e.Column.FieldName == "Quantity")
			{
				if (!_sourceActionField.ToLower().Equals("invqty"))
				{
					_sourceActionField = "quantity";
					gv_Lines.SetRowCellValue(e.RowHandle, gcLineInvQty.FieldName, Function.ParseDouble(gv_Lines.GetRowCellValue(e.RowHandle, gcLineQuantity)) * Function.ParseDouble(gv_Lines.GetRowCellValue(e.RowHandle, gcLineItemsperUnit)));
				}
				else
				{
					_sourceActionField = "";
				}
				gv_Lines.SetRowCellValue(e.RowHandle, gcLineOpenQty.FieldName, Function.ParseDouble(gv_Lines.GetRowCellValue(e.RowHandle, gcLineQuantity)));
				gv_Lines.SetRowCellValue(e.RowHandle, gcLineOpenInvQty.FieldName, Function.ParseDouble(gv_Lines.GetRowCellValue(e.RowHandle, gcLineOpenQty)) * Function.ParseDouble(gv_Lines.GetRowCellValue(e.RowHandle, gcLineItemsperUnit)));
			}
			else if (e.Column.FieldName.Equals(gcLineInvQty.FieldName))
			{
				double num7 = Function.ParseDouble(e.Value) / Function.ParseDouble(gv_Lines.GetRowCellValue(e.RowHandle, gcLineItemsperUnit));
				if (!_sourceActionField.ToLower().Equals("quantity") && !_sourceActionField.ToLower().Equals("uomcode"))
				{
					_sourceActionField = "invqty";
					gv_Lines.SetRowCellValue(e.RowHandle, gcLineQuantity, num7);
				}
				else
				{
					_sourceActionField = "";
				}
			}
			if (e.Column.FieldName == gcLineQuantity.FieldName || e.Column.FieldName == gcLinePriceBefDi.FieldName || e.Column.FieldName.Equals(gcLineCurrency.FieldName) || e.Column.FieldName.Equals("DiscPrcnt") || e.Column.FieldName.Equals(gcLineItemCode.FieldName))
			{
				double num8 = Function.ParseDouble(gv_Lines.GetRowCellValue(e.RowHandle, gcLineQuantity));
				double num9 = Function.ParseDouble(gv_Lines.GetRowCellValue(e.RowHandle, gcLinePriceBefDi));
				double num10 = Function.ParseDouble(gv_Lines.GetRowCellValue(e.RowHandle, "DiscPrcnt")) / 100.0;
				double num11 = num9 * (1.0 - num10);
				dataSourceTable.Rows[dataSourceRowIndex]["Price"] = num11;
				double num12 = num8 * num11;
				dataSourceTable.Rows[dataSourceRowIndex]["LineTotal"] = num12;
				double num13 = 0.0;
				double num14 = 0.0;
				for (int i = 0; i < dataSourceTable.Rows.Count; i++)
				{
					num13 += Function.ParseDouble(dataSourceTable.Rows[i]["Quantity"]) * Function.ParseDouble(dataSourceTable.Rows[i]["Price"]) * (1.0 - Function.ParseDouble(dataSourceTable.Rows[i]["DiscPrcnt"]) / 100.0) * Function.ParseDouble(dataSourceTable.Rows[i]["Rate"]);
					num14 += Function.ParseDouble(dataSourceTable.Rows[i]["Quantity"]) * Function.ParseDouble(dataSourceTable.Rows[i]["Price"]) * Function.ParseDouble(dataSourceTable.Rows[i]["Rate"]);
				}
				gv_Doc.SetFocusedRowCellValue("TotalBeforeDis", num13 / Function.ParseDouble(gv_Doc.GetFocusedRowCellValue("DocRate")));
				gv_Doc.SetFocusedRowCellValue("DocTotal", num14 / Function.ParseDouble(gv_Doc.GetFocusedRowCellValue("DocRate")));
			}
			if ((e.Column.FieldName == gcLineItemCode.FieldName || e.Column.FieldName == gcLineFromWhsCode.FieldName) && !string.IsNullOrEmpty(Function.ToString(gv_Lines.GetRowCellValue(e.RowHandle, gcLineItemCode))) && !string.IsNullOrEmpty(Function.ToString(gv_Lines.GetRowCellValue(e.RowHandle, gcLineFromWhsCode))))
			{
				DataTable dataItemOnhand = GetDataItemOnhand(Function.ToString(gv_Lines.GetRowCellValue(e.RowHandle, gcLineItemCode)), Function.ToString(gv_Lines.GetRowCellValue(e.RowHandle, gcLineFromWhsCode)));
				if (dataItemOnhand != null && dataItemOnhand.Rows.Count > 0)
				{
					dataSourceTable.Rows[dataSourceRowIndex][gcLineCommitted.FieldName] = dataItemOnhand.Rows[0]["IsCommittedItem"];
					dataSourceTable.Rows[dataSourceRowIndex][gcLineCommittedQtyWhse.FieldName] = dataItemOnhand.Rows[0]["IsCommittedWhs"];
					dataSourceTable.Rows[dataSourceRowIndex][gcLineInStock.FieldName] = dataItemOnhand.Rows[0]["OnHandItem"];
					dataSourceTable.Rows[dataSourceRowIndex][gcLineOrdered.FieldName] = dataItemOnhand.Rows[0]["OnOrderedItem"];
					dataSourceTable.Rows[dataSourceRowIndex][gcLineOrderedQtyWhse.FieldName] = dataItemOnhand.Rows[0]["OnOrderedWhs"];
					dataSourceTable.Rows[dataSourceRowIndex][gcLineQtyinWhse.FieldName] = dataItemOnhand.Rows[0]["OnHandWhs"];
					dataSourceTable.Rows[dataSourceRowIndex][gcLineAvgPrice.FieldName] = dataItemOnhand.Rows[0]["AvgPrice"];
					grd_Lines.DataSourceTable.AcceptChanges();
				}
			}
			dataSourceTable.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		ChangeViewMode();
	}

	private void gv_Lines_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == gcLineTotal.FieldName)
			{
				gv_Doc.SetFocusedRowCellValue(gcDocTotal, Function.ParseDouble(gv_Doc.GetFocusedRowCellValue(gcDocTotal)) - Function.ParseDouble(gv_Lines.GetFocusedRowCellValue(gcLineTotal)) + Function.ParseDouble(e.Value));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected virtual void deleteToolStripMenuItem_DocLine_Click(object sender, EventArgs e)
	{
		try
		{
			DeleteLine();
			_onlyChangeRemark = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		ChangeViewMode();
	}

	private async void gv_Doc_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			_loadingDetail = true;
			if (!_isSearching && !IsLoadForm && FormMode != Enums.FormViewModeType.Add)
			{
				if (Function.ToString(gv_Doc.GetFocusedRowCellValue("WddStatus")) == "Y")
				{
					Text += "(Approved)";
				}
				else
				{
					Text = Text.Replace("(Approved)", "");
				}
				EnableDisableControl(isEnable: false);
				FormMode = Enums.FormViewModeType.View;
				_onlyChangeRemark = true;
				await LoadDataLine();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			EnableDisableControl(isEnable: true);
			_loadingDetail = false;
		}
	}

	public void txtCardCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.Bp, false, Function.ToString(txtCardCode.Text), "", Enums.ItemObjectType.NoItemType, null, "");
		frm_CFL2.ShowDialog(this);
		if (frm_CFL2.ChooseItem != null)
		{
			DataRow chooseItem = frm_CFL2.ChooseItem;
			if (chooseItem != null && chooseItem.IsExistsColumn("CardCode"))
			{
				txtCardCode.EditValue = Function.ToString(chooseItem["CardCode"]);
			}
		}
	}

	private void rDocContactPersion_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.ContactPerson, multiSelect: false, "");
			frm_ChooseFromList2.ShowDialog(this);
			if (frm_ChooseFromList2.ChooseItem != null)
			{
				BusinessPartnersImpl businessPartnersImpl = (BusinessPartnersImpl)frm_ChooseFromList2.ChooseItem;
				gv_Doc.SetFocusedRowCellValue(gcDocCntctCode, businessPartnersImpl.Name);
				grd_Doc.DataSourceTable.Rows[gv_Doc.GetDataSourceRowIndex(gv_Doc.FocusedRowHandle)]["CntctCode"] = businessPartnersImpl.CntctCode;
				grd_Doc.DataSourceTable.AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rLineWhareHouse_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			string text = Function.ToString(gv_Lines.GetFocusedRowCellValue(gv_Lines.FocusedColumn.FieldName));
			if (e.Button.Index == 0)
			{
				if (gv_Lines.ActiveEditor != null)
				{
					text = Function.ToString(gv_Lines.ActiveEditor.EditValue);
				}
				frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.GetListWareHouse, false, text, "", Enums.ItemObjectType.NoItemType, null, "");
				frm_CFL2.ShowDialog(this);
				if (frm_CFL2.ChooseItem != null)
				{
					gv_Lines.SetFocusedRowCellValue(gv_Lines.FocusedColumn, frm_CFL2.ChooseItem["WhsCode"]);
					grd_Lines.DataSourceTable.AcceptChanges();
				}
			}
			else if (!string.IsNullOrWhiteSpace(text))
			{
				this.DrillDown(ApzObjectTypes.oWarehouses, ApzObjectTypes.oWarehouses, text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rLineDistrRule_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			string text = Function.ToString(gv_Lines.GetFocusedRowCellValue(gcLineDistrRule));
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.DistributeRule, multiSelect: false, "");
			frm_ChooseFromList2._strOcrDistRule = Function.ToString(text);
			frm_ChooseFromList2.ShowDialog(this);
			DataTable dtSource = frm_ChooseFromList2._dtSource;
			string text2 = "";
			if (dtSource != null && dtSource.Rows.Count > 0)
			{
				for (int i = 0; i < dtSource.Rows.Count; i++)
				{
					text2 = text2 + Function.ToString(dtSource.Rows[i]["OcrCode"]) + ";";
				}
				if (text2.Length > 1)
				{
					text2 = text2.Remove(text2.Length - 1);
				}
			}
			string value = text2.Replace(";", "");
			if (!string.IsNullOrWhiteSpace(value))
			{
				DataTable dataSourceTable = grd_Lines.DataSourceTable;
				int focusedDataSourceRowIndex = gv_Lines.GetFocusedDataSourceRowIndex();
				dataSourceTable.Rows[focusedDataSourceRowIndex][gcLineDistrRule.FieldName] = value;
				string[] array = Function.ToString(dataSourceTable.Rows[focusedDataSourceRowIndex][gcLineDistrRule.FieldName]).Split(';');
				for (int j = 0; j < array.Length; j++)
				{
					dataSourceTable.Rows[focusedDataSourceRowIndex]["OcrCode" + ((j == 0) ? "" : Function.ToString(j + 1))] = array[j];
				}
				for (int k = array.Length; k < 5; k++)
				{
					dataSourceTable.Rows[focusedDataSourceRowIndex]["OcrCode" + ((k == 0) ? "" : Function.ToString(k + 1))] = "";
				}
			}
			grd_Lines.DataSourceTable.AcceptChanges();
			grd_Lines.DataSourceTable.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public virtual void gv_Doc_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (e.Column.OptionsColumn.ReadOnly || !e.Column.OptionsColumn.AllowEdit)
			{
				e.Appearance.BackColor = Color.Gainsboro;
			}
			if (e.RowHandle == gv_Doc.FocusedRowHandle)
			{
				e.Appearance.BackColor = Color.FromArgb(245, 231, 188);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public virtual void gv_Lines_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (e.RowHandle == gv_Lines.FocusedRowHandle)
			{
				e.Appearance.BackColor = Color.FromArgb(245, 231, 188);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Logistic_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (e.Column.OptionsColumn.ReadOnly || !e.Column.OptionsColumn.AllowEdit)
			{
				e.Appearance.BackColor = Color.Gainsboro;
			}
			if (e.RowHandle == gv_Logistic.FocusedRowHandle)
			{
				e.Appearance.BackColor = Color.FromArgb(245, 231, 188);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public virtual void gv_Lines_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Add)
			{
				string fieldName = gv_Lines.FocusedColumn.FieldName;
				if (_lstFieldLineNotEditable.Contains(fieldName))
				{
					e.Cancel = true;
				}
				return;
			}
			if (ObjectType == ApzObjectTypes.oStockTransfer || (ObjectType == ApzObjectTypes.oInventoryTransferRequest && Function.ToString(gv_Doc.GetFocusedRowCellValue("DocStatus")) == "C"))
			{
				e.Cancel = true;
				return;
			}
			if (FormMode != Enums.FormViewModeType.Add && (Function.ToString(gv_Doc.GetFocusedRowCellValue(gcDocStatus)).Equals("C") || Function.ToString(gv_Lines.GetFocusedRowCellValue(gcLineStatus)).Equals("C")))
			{
				e.Cancel = true;
				return;
			}
			if (gv_Lines.FocusedColumn.FieldName != gcLineItemCode.FieldName && gv_Lines.FocusedColumn.FieldName != gcLineDscription.FieldName && string.IsNullOrEmpty(Function.ToString(gv_Lines.GetFocusedRowCellValue(gcLineItemCode))))
			{
				WinCommons.Warning(Enums.MessageCode.ItemIsMissing, "");
				gv_Lines.FocusedColumn = gv_Lines.Columns[gcLineItemCode.FieldName];
				gv_Lines.ShowEditor();
				e.Cancel = true;
			}
			if (gv_Lines.FocusedColumn.FieldName == gcLineInventoryUoM.FieldName)
			{
				if (Function.ParseInt(gv_Lines.GetFocusedRowCellValue("UgpCode")) == -1 && Math.Abs(Function.ParseDouble(gv_Lines.GetFocusedRowCellValue("NumInSale")) - 1.0) < 1E-08)
				{
					e.Cancel = true;
				}
			}
			else if (gv_Lines.FocusedColumn.FieldName == gcLineUoMCode.FieldName)
			{
				if (Function.ToString(gv_Lines.GetFocusedRowCellValue(gcLineInventoryUoM)).Equals("Y") || Function.ParseInt(gv_Lines.GetFocusedRowCellValue(gcLineUoMGroup)) == -1)
				{
					e.Cancel = true;
				}
			}
			else if (gv_Lines.FocusedColumn.FieldName == gcLineUoMName.FieldName)
			{
				if (!Function.ToString(gv_Lines.GetFocusedRowCellValue(gcLineInventoryUoM)).Equals("N") || Function.ParseInt(gv_Lines.GetFocusedRowCellValue(gcLineUoMGroup)) != -1)
				{
					e.Cancel = true;
				}
			}
			else if (gv_Lines.FocusedColumn.FieldName == gcLineItemsperUnit.FieldName && (Function.ParseInt(gv_Lines.GetFocusedRowCellValue(gcLineUoMGroup)) > 0 || Function.ParseInt(gv_Lines.GetFocusedRowCellValue("TargetType")) > 0 || Function.ToString(gv_Lines.GetFocusedRowCellValue(gcLineInventoryUoM)).Equals("Y")))
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

	public virtual void gv_Doc_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (_lstDocFieldNotEditable.Contains(gv_Doc.FocusedColumn.FieldName))
			{
				e.Cancel = true;
			}
			else if (((ObjectType == ApzObjectTypes.oStockTransfer && FormMode != Enums.FormViewModeType.Add) || (ObjectType == ApzObjectTypes.oInventoryTransferRequest && Function.ToString(gv_Doc.GetFocusedRowCellValue("DocStatus")) == "C")) && !gv_Doc.FocusedColumn.FieldName.StartsWith("U_") && !_lstEditableAfterAdd.Contains(gv_Doc.FocusedColumn.FieldName))
			{
				e.Cancel = true;
			}
			else if (gv_Doc.FocusedColumn.FieldName.Equals(gcDocCurrency.FieldName) && !Function.ToString(gv_Doc.GetFocusedRowCellValue("Currency")).Equals("##"))
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

	private void rLineUoMCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.ListUomCode, false, "", "", Enums.ItemObjectType.NoItemType, null, "")
			{
				UomGroup = Function.ParseInt(gv_Lines.GetFocusedRowCellValue(gcLineUoMGroup))
			};
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem != null)
			{
				gv_Lines.SetFocusedRowCellValue(gcLineUoMCode, frm_CFL2.ChooseItem["UomCode"]);
				grd_Lines.DataSourceTable.AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void grd_Lines_EditorKeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyData == Keys.Tab)
			{
				grd_Lines.DataSourceTable.AcceptChanges();
				if (gv_Lines.FocusedColumn.FieldName.Equals(gcLineItemCode.FieldName))
				{
					string text = Function.ToString(gv_Lines.EditingValue);
					if (string.IsNullOrEmpty(text))
					{
						gv_Lines.RefreshEditor(updateEditorValue: true);
						ChooseFromListLineItem(text);
					}
				}
			}
			else if (e.KeyCode == Keys.V && e.Modifiers == Keys.Control)
			{
				WinCommons._OpenProcessing();
				ExtenseionMethods.PasteFromExcelToDataTable(grd_Lines, gv_Lines, new List<string> { gcLineItemCode.FieldName });
				AddNewRowLine();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	private void grd_Doc_EditorKeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyData == Keys.Tab && gv_Doc.FocusedColumn.FieldName.Equals(gcDocCardCode.FieldName))
			{
				frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.CustomerVendor, false, string.Empty, "", Enums.ItemObjectType.NoItemType, null, "");
				frm_CFL2.ShowDialog(this);
				if (frm_CFL2.ChooseItem != null)
				{
					gv_Doc.SetFocusedRowCellValue("CardCode", Function.ToString(frm_CFL2.ChooseItem["CardCode"]));
					grd_Doc.DataSourceTable.AcceptChanges();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	protected virtual void insertRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			AddNewRowLine(isInsert: true);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		ChangeViewMode();
	}

	private async void btnSearch_Click(object sender, EventArgs e)
	{
		try
		{
			if ((FormMode != Enums.FormViewModeType.Add && FormMode != Enums.FormViewModeType.Update) || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
			{
				EnableDisableControl(isEnable: false);
				FormMode = Enums.FormViewModeType.View;
				DocumentTable clone = DocumentsHelper.Document.GetClone();
				grd_Doc.DataSource = clone;
				grd_Doc.RefreshDataSource();
				gv_Doc.FocusedRowHandle = 0;
				ucBatchAndSerial1.ClearLineDataSource();
				grd_Lines.DataSource = new DocumentLineTable();
				grd_Lines.RefreshDataSource();
				await LoadDataDoc(Enums.NavigateType.Current);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			EnableDisableControl(isEnable: true);
		}
	}

	private void baseDocumentToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			int num = Function.ParseInt(gv_Lines.GetFocusedRowCellValue("BaseEntry"));
			if (num > 0)
			{
				frm_InventoryTranferRequest frm_InventoryTranferRequest2 = new frm_InventoryTranferRequest(Function.ToString(num), ApzObjectTypes.oInventoryTransferRequest);
				frm_InventoryTranferRequest2.MdiParent = MdiParent;
				frm_InventoryTranferRequest2.Show();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void targetDocumentToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			int num = Function.ParseInt(gv_Lines.GetFocusedRowCellValue("TrgetEntry"));
			if (num > 0)
			{
				frm_InventoryTranfer frm_InventoryTranfer2 = new frm_InventoryTranfer(Function.ToString(num), ApzObjectTypes.oStockTransfer);
				frm_InventoryTranfer2.MdiParent = MdiParent;
				frm_InventoryTranfer2.Show();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void closeLineToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (Function.ToString(gv_Lines.GetFocusedRowCellValue(gcLineStatus)).Equals("C"))
			{
				WinCommons.Warning(Enums.MessageCode.LineHadBeenClose, "");
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.ThisFunctionIsNotSupported, "");
			}
			return;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.Update;
	}

	private void gv_Lines_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			if (!IsLoadForm)
			{
				ucBatchAndSerial1.IsAllowcateBinLocationToWhs = false;
				ucBatchAndSerial1.FinishEditorBatchAndSerial();
				ucBatchAndSerial1.SetFocusLine();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void batchSerialNumberToolStripMenuItem_Click(object sender, EventArgs e)
	{
		string text = Function.ToString(gv_Lines.GetFocusedRowCellValue("ManBtchSerial"));
		splitContainerLine.PanelVisibility = ((splitContainerLine.PanelVisibility == SplitPanelVisibility.Both) ? SplitPanelVisibility.Panel1 : SplitPanelVisibility.Both);
		ucBatchAndSerial1.IsAllowcateBinLocationToWhs = text.Equals("N") && ucBatchAndSerial1.CheckWhsManBinLocation(Function.ToString(gv_Lines.GetFocusedRowCellValue("WhsCode"))) && !ucBatchAndSerial1.IsAllowcateBinLocationToWhs;
	}

	private void contextMenuStrip1_DocLine_Opening(object sender, CancelEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Add)
			{
				closeLineToolStripMenuItem.Visible = false;
				toolStripMenuItem_AddNewDocLine.Visible = true;
				deleteToolStripMenuItem_DocLine.Visible = true;
				cmtDuplicate_Line.Visible = true;
				insertRowToolStripMenuItem.Visible = true;
			}
			else
			{
				closeLineToolStripMenuItem.Visible = ObjectType == ApzObjectTypes.oInventoryTransferRequest && Function.ToString(gv_Lines.GetFocusedRowCellValue("LineStatus")) == "O";
				ToolStripMenuItem toolStripMenuItem = toolStripMenuItem_AddNewDocLine;
				ToolStripMenuItem toolStripMenuItem2 = cmtDuplicate_Line;
				bool flag = (insertRowToolStripMenuItem.Visible = ObjectType == ApzObjectTypes.oInventoryTransferRequest && Function.ToString(gv_Doc.GetFocusedRowCellValue("DocStatus")) != "C");
				bool visible = (toolStripMenuItem2.Visible = flag);
				toolStripMenuItem.Visible = visible;
				deleteToolStripMenuItem_DocLine.Visible = ObjectType == ApzObjectTypes.oInventoryTransferRequest && (Function.ToString(gv_Lines.GetFocusedRowCellValue("LineStatus")) == "O" || Function.ParseInt(gv_Lines.GetFocusedRowCellValue("TargetType")) <= 0);
			}
			string text = Function.ToString(gv_Lines.GetFocusedRowCellValue("ManBtchSerial"));
			batchSerialNumberToolStripMenuItem.Enabled = text.Equals("S") || text.Equals("B") || (ucBatchAndSerial1.CheckWhsManBinLocation(Function.ToString(gv_Lines.GetFocusedRowCellValue("WhsCode"))) && ObjectType == ApzObjectTypes.oStockTransfer);
			if (text.Equals("S") || text.Equals("B"))
			{
				batchSerialNumberToolStripMenuItem.Text = ((splitContainerLine.PanelVisibility == SplitPanelVisibility.Both) ? "Hide Batch/Serial Numbers" : "Batch/Serial Numbers");
			}
			else if (ucBatchAndSerial1.CheckWhsManBinLocation(Function.ToString(gv_Lines.GetFocusedRowCellValue("WhsCode"))))
			{
				batchSerialNumberToolStripMenuItem.Text = ((splitContainerLine.PanelVisibility == SplitPanelVisibility.Both && ucBatchAndSerial1.IsAllowcateBinLocationToWhs) ? "Hide Allocation receipt bin" : "Allocation receipt bin");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void newActivityToolStripMenuItem1_Click(object sender, EventArgs e)
	{
		try
		{
			if (gv_Doc.GetFocusedDataRow() != null)
			{
				bool flag = Function.ParseBool(gv_Doc.GetFocusedRowCellValue("IsDraft"));
				int num = ((!flag) ? Function.ParseInt(gv_Doc.GetFocusedRowCellValue("DocEntry")) : Function.ParseInt(gv_Doc.GetFocusedRowCellValue("DraftKey")));
				int num2 = (int)(flag ? ApzObjectTypes.oDrafts : ObjectType);
				int num3 = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("DocNum"));
				string value = Function.ToString(gv_Doc.GetFocusedRowCellValue("CardCode"));
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
					string text = DateTime.Now.Minute.ToString();
					dataRow["BeginTime"] = DateTime.Now.Hour + ((text.Length == 2) ? text : ("0" + text));
				}
				dataRow["Closed"] = "N";
				dataRow["DocType"] = num2;
				if (num != 0)
				{
					dataRow["DocEntry"] = num;
				}
				if (num3 != 0)
				{
					dataRow["DocNum"] = num3;
				}
				dataRow["CardCode"] = value;
				tableStructure.Rows.Add(dataRow);
				tableStructure.AcceptChanges();
				frm_TaskManagement2.DataRowFromCal = dataRow;
				frm_TaskManagement2.Show();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void saveAsDraftToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			await Save(isAsDraft: true);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public void SetMapping(DataTable doc, DataTable lines, ContextActionMappingImpl[] mappingDoc, ContextActionMappingImpl[] mappingLines, DataTable batch, ContextActionMappingImpl[] mappingbatch, Enums.FormViewModeType viewmode = Enums.FormViewModeType.Add)
	{
		OpenFormDocument = doc;
		OpenFormLines = lines;
		MappingDocument = mappingDoc;
		MappingLines = mappingLines;
		OpenFormBatchSerial = batch;
		MappingBatchSerial = mappingbatch;
		OpenFormViewMode = viewmode;
	}

	public async Task PRINTLAYOUT()
	{
		try
		{
			int num = Function.ParseInt((FromType != ApzObjectTypes.oDrafts) ? gv_Doc.GetFocusedRowCellValue("DocEntry") : gv_Doc.GetFocusedRowCellValue("draftKey"));
			ApzObjectTypes apzObjectTypes = ((FromType != ApzObjectTypes.oDrafts) ? ObjectType : ApzObjectTypes.oDrafts);
			object docEntry = num;
			string formLayoutCode = FormLayoutCode;
			int num2 = (int)apzObjectTypes;
			await WinFuntions.PrintLayout("", docEntry, formLayoutCode, num2.ToString());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task PREVIEWLAYOUTS()
	{
		try
		{
			int num = Function.ParseInt((FromType != ApzObjectTypes.oDrafts) ? gv_Doc.GetFocusedRowCellValue("DocEntry") : gv_Doc.GetFocusedRowCellValue("draftKey"));
			ApzObjectTypes apzObjectTypes = ((FromType != ApzObjectTypes.oDrafts) ? ObjectType : ApzObjectTypes.oDrafts);
			object docEntry = num;
			string formLayoutCode = FormLayoutCode;
			VirFormInventoryTranfer form = this;
			int num2 = (int)apzObjectTypes;
			await WinFuntions.PreviewLayout("", docEntry, formLayoutCode, form, num2.ToString());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async void PRINT()
	{
		try
		{
			int docEntry = Function.ParseInt((FromType != ApzObjectTypes.oDrafts) ? gv_Doc.GetFocusedRowCellValue("DocEntry") : gv_Doc.GetFocusedRowCellValue("draftKey"));
			ApzObjectTypes apzObjectTypes = ((FromType != ApzObjectTypes.oDrafts) ? ObjectType : ApzObjectTypes.oDrafts);
			string formLayoutCode = FormLayoutCode;
			int num = (int)apzObjectTypes;
			await WinFuntions.Print("", docEntry, formLayoutCode, num.ToString());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task MOVE_PREVIOUS()
	{
		try
		{
			WinCommons._OpenProcessing();
			EnableDisableControl(isEnable: false);
			FormMode = Enums.FormViewModeType.View;
			await LoadDataDoc(Enums.NavigateType.Previous);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			EnableDisableControl(isEnable: true);
			WinCommons._CloseProcessing();
		}
	}

	public async Task MOVE_NEXT()
	{
		try
		{
			WinCommons._OpenProcessing();
			EnableDisableControl(isEnable: false);
			FormMode = Enums.FormViewModeType.View;
			await LoadDataDoc(Enums.NavigateType.Next);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			EnableDisableControl(isEnable: true);
			WinCommons._CloseProcessing();
		}
	}

	public async Task MOVE_FIRST()
	{
		try
		{
			WinCommons._OpenProcessing();
			EnableDisableControl(isEnable: false);
			FormMode = Enums.FormViewModeType.View;
			await LoadDataDoc(Enums.NavigateType.First);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			EnableDisableControl(isEnable: true);
			WinCommons._CloseProcessing();
		}
	}

	public async Task MOVE_LAST()
	{
		try
		{
			WinCommons._OpenProcessing();
			EnableDisableControl(isEnable: false);
			FormMode = Enums.FormViewModeType.View;
			await LoadDataDoc(Enums.NavigateType.Last);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			EnableDisableControl(isEnable: true);
			WinCommons._CloseProcessing();
		}
	}

	public async Task MODE_FIND()
	{
		try
		{
			WinCommons._OpenProcessing();
			EnableDisableControl(isEnable: false);
			FormMode = Enums.FormViewModeType.View;
			await LoadDataDoc(Enums.NavigateType.Current);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			EnableDisableControl(isEnable: true);
			WinCommons._CloseProcessing();
		}
	}

	public async Task REFRESH()
	{
		try
		{
			WinCommons._OpenProcessing();
			EnableDisableControl(isEnable: false);
			FormMode = Enums.FormViewModeType.Add;
			await AddNewRowDoc();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			EnableDisableControl(isEnable: true);
		}
	}

	public override void CHANGELOG()
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View || FormMode == Enums.FormViewModeType.Update)
			{
				string primaryKey = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("DocEntry")).ToString();
				Enums.ChangeLog changeLogType = ChangeLogType;
				GetChangeLogParams getChangeLogParams = new GetChangeLogParams();
				getChangeLogParams.PrimaryKey = primaryKey;
				getChangeLogParams.Object = changeLogType;
				frm_ChangeLogList frm_ChangeLogList2 = new frm_ChangeLogList();
				frm_ChangeLogList2.GetChangeLogParams = getChangeLogParams;
				frm_ChangeLogList2.MdiParent = MdiParent;
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

	public async Task REFRESH_DATA()
	{
		await MODE_FIND();
	}

	protected DataTable GetDataBatch(string batNo, string itemCode, string whsCode)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons");
		return httpClientHelper.Get("GetDataBatch", new { batNo, itemCode, whsCode });
	}

	protected virtual void LoadButtonCopy()
	{
	}

	private void ClearGrid()
	{
		grd_Doc.DataSourceTable.Rows.Clear();
		grd_Lines.DataSourceTable.Rows.Clear();
		ucAttachment1.grd_File.DataSourceTable.Rows.Clear();
		ucBatchAndSerial1.ClearDataSource();
	}

	protected virtual async Task LoadDataDoc(Enums.NavigateType type)
	{
		int num = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("DocNum"));
		ClearGrid();
		WinCommons._OpenProcessing();
		_isSearching = true;
		try
		{
			DataTable dataTable;
			using (HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController))
			{
				dataTable = await http.GetAsync("GetDataDoc", new
				{
					cardCode = Function.ToString(txtCardCode.EditValue),
					docNum = ((type == Enums.NavigateType.Current) ? Function.ParseInt(txtDocNum.EditValue) : num),
					docEntry = Function.ParseInt(FromDocEntry),
					docStatus = Function.ToString(txtDocStatus.EditValue),
					prilist = Function.ParseInt(txtPriceList.EditValue),
					fromDate = (string.IsNullOrEmpty(txtFromDate.Text) ? "" : txtFromDate.DateTime.ToString("ddMMyyyy")),
					toDate = (string.IsNullOrEmpty(txtToDate.Text) ? "" : txtToDate.DateTime.ToString("ddMMyyyy")),
					type = type
				});
			}
			if (dataTable != null && dataTable.Columns.Count > 0)
			{
				dataTable.Columns[gcDocFromBranch.FieldName].DefaultValue = GlobalData.BranchLogin;
				dataTable.Columns[gcDocDate.FieldName].DefaultValue = DateTime.Now.Date;
				dataTable.Columns[gcDocTaxDate.FieldName].DefaultValue = DateTime.Now.Date;
				dataTable.Columns[gcDocDueDate.FieldName].DefaultValue = DateTime.Now.Date;
				dataTable.Columns[gcDocStatus.FieldName].DefaultValue = "O";
				dataTable.Columns[gcDocJournalRemarks.FieldName].DefaultValue = JournalRemarksDefault;
				dataTable.Columns["DocCur"].DefaultValue = GlobalData.MainCurrency;
				dataTable.Columns["GrossBase"].DefaultValue = -5;
				dataTable.Columns["UpdInvnt"].DefaultValue = "C";
				dataTable.Columns["UserSign"].DefaultValue = GlobalData.UserSignLogin;
				dataTable.Columns["UserSign2"].DefaultValue = GlobalData.UserSignLogin;
				dataTable.Columns["PIndicator"].DefaultValue = "Default";
				if (rDocPriceList.DataSource is DataTable dataTable2 && dataTable2.Rows.Count > 0)
				{
					dataTable.Columns[gcDocPriceList.FieldName].DefaultValue = dataTable2.Rows[0]["Code"];
				}
			}
			FromDocEntry = "";
			_ = gv_Doc.FocusedRowHandle;
			grd_Doc.DataSource = dataTable;
			_originalDocTable = dataTable.Copy();
			grd_Doc.DataSourceTable.AcceptChanges();
			if (dataTable.Rows.Count > 0)
			{
				await LoadDataLine();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			_isSearching = false;
		}
		WinCommons._CloseProcessing();
	}

	protected virtual async Task LoadDataLine()
	{
		WinCommons._OpenProcessing();
		try
		{
			ucBatchAndSerial1.ClearDataSource();
			grd_Lines.DataSourceTable.Rows.Clear();
			DataSet dataSet;
			using (HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, FormController))
			{
				dataSet = await http.GetAsync("GetDataLine", new
				{
					docEntry = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("DocEntry"))
				});
			}
			if (dataSet == null || dataSet.Tables.Count != 4)
			{
				grd_Lines.DataSourceTable.Rows.Clear();
				WinCommons.Error(Enums.MessageCode.DataLinesIsEmpty, "");
				return;
			}
			Task task = ucAttachment1.LoadAttachments(Function.ParseInt(gv_Doc.GetFocusedRowCellValue("AtcEntry")));
			dataSet.Tables[0].Columns[gcLineFactor1.FieldName].DefaultValue = 1;
			dataSet.Tables[0].Columns[gcLineFactor2.FieldName].DefaultValue = 1;
			dataSet.Tables[0].Columns[gcLineFactor3.FieldName].DefaultValue = 1;
			dataSet.Tables[0].Columns[gcLineFactor4.FieldName].DefaultValue = 1;
			dataSet.Tables[0].Columns[gcLineQuantity.FieldName].DefaultValue = 0;
			dataSet.Tables[0].Columns["Currency"].DefaultValue = (string.IsNullOrEmpty(GlobalData.MainCurrency) ? "VND" : GlobalData.MainCurrency);
			dataSet.Tables[0].Columns[gcLineStatus.FieldName].DefaultValue = "O";
			dataSet.Tables[0].Columns[gcLineInvQty.FieldName].DefaultValue = 1;
			dataSet.Tables[0].Columns[gcLineOpenQty.FieldName].DefaultValue = 1;
			dataSet.Tables[0].Columns[gcLineOpenCreQty.FieldName].DefaultValue = 1;
			dataSet.Tables[0].Columns[gcLineOpenInvQty.FieldName].DefaultValue = 1;
			dataSet.Tables[0].Columns[gcLinePrice.FieldName].DefaultValue = 0;
			ucBatchAndSerial1.DataSourceBatchAndSerial = dataSet.Tables[1];
			ucBatchAndSerial1.DataSourceBinAllowLine = dataSet.Tables[2];
			grd_Lines.DataSource = dataSet.Tables[0];
			grd_Lines.DataSourceTable.AcceptChanges();
			if (gv_Lines.FocusedRowHandle < 0)
			{
				ucBatchAndSerial1.FinishEditorBatchAndSerial();
				ucBatchAndSerial1.SetFocusLine();
			}
			if (FormMode == Enums.FormViewModeType.Add || (ObjectType == ApzObjectTypes.oInventoryTransferRequest && Function.ToString(gv_Doc.GetFocusedRowCellValue("DocStatus")) != "C"))
			{
				AddNewRowLine();
			}
			if (gv_Doc.RowCount == 0)
			{
				grd_Lines.DataSourceTable.Rows.Clear();
				grd_Lines.DataSourceTable.AcceptChanges();
			}
			_currentRefDoc = dataSet.Tables[3];
			await task;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	private async Task LoadComb()
	{
		_ = 7;
		try
		{
			LoadDataStatusControl();
			LoadInventoryUoM();
			LoadDocStatusControl();
			rLineSpecPrice.DataSource = WinFuntions.GetDataPriceSource();
			await LoadDataSeriesControl();
			await LoadDataPriceList();
			await LoadSalesEmployeeControl();
			await LoadTaxCodeControl();
			await LoadDataBranch();
			await LoadUomGroup();
			await GetBranch();
			RepositoryItemLookUpEdit repositoryItemLookUpEdit = rLineCurrency;
			repositoryItemLookUpEdit.DataSource = await _httpClient.Commons.GetCurrency();
			rDocCur.DataSource = rLineCurrency.DataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected virtual void LoadDocStatusControl()
	{
	}

	private void LoadInventoryUoM()
	{
		try
		{
			List<Common.Table_Code> dataSource = new List<Common.Table_Code>
			{
				new Common.Table_Code
				{
					Name = "Yes",
					Code = "Y"
				},
				new Common.Table_Code
				{
					Name = "No",
					Code = "N"
				}
			};
			rLineUseBaseUn.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadUomGroup()
	{
		try
		{
			RepositoryItemLookUpEdit repositoryItemLookUpEdit = rLineUoMGroup;
			repositoryItemLookUpEdit.DataSource = await _httpClient.Commons.GetDataUomGroup();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadDataBranch()
	{
		try
		{
			RepositoryItemLookUpEdit repositoryItemLookUpEdit = rDocBranch;
			repositoryItemLookUpEdit.DataSource = await _httpClient.Commons.GetBranchList(GlobalData.UserName);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadTaxCodeControl()
	{
		try
		{
			DataTable dataSource;
			using (HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons"))
			{
				dataSource = await http.GetAsync("GetDataTaxCode");
			}
			rLineTaxCode.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadSalesEmployeeControl()
	{
		try
		{
			DataTable dataSource;
			using (HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons"))
			{
				dataSource = await http.GetAsync("GetSaleEmployee");
			}
			rDocSalesEmployee.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadDataSeriesControl()
	{
		try
		{
			using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, (ObjectType == ApzObjectTypes.oStockTransfer) ? "InventoryTranfer" : "TranferRequest");
			DataTable dataTable = await http.GetAsync("LoadSeries", new
			{
				usercode = GlobalData.UserName
			});
			rDocSeries.DataSource = dataTable;
			_dtSeries = dataTable.Copy();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadDataStatusControl()
	{
		try
		{
			List<Common.Table_Code> dataSource = new List<Common.Table_Code>
			{
				new Common.Table_Code
				{
					Name = "Open",
					Code = "O"
				},
				new Common.Table_Code
				{
					Name = "Close",
					Code = "C"
				}
			};
			rDocStatus.DataSource = dataSource;
			rLineStatus.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected virtual int CheckDataDocSave()
	{
		if (gv_Doc.FocusedRowHandle < 0)
		{
			WinCommons.Error(Enums.MessageCode.CanNotGetDataHeader, "");
			return 1;
		}
		if (string.IsNullOrEmpty(Function.ToString(gv_Doc.GetFocusedRowCellValue(gcDocFromWhs))))
		{
			WinCommons.Error(Enums.MessageCode.FromWhsInHeaderCannotBeEmpty, "");
			gv_Doc.FocusedColumn = gv_Doc.Columns[gcDocFromWhs.FieldName];
			gv_Doc.ShowEditor();
			return 1;
		}
		if (string.IsNullOrEmpty(Function.ToString(gv_Doc.GetFocusedRowCellValue(gcDocToWhs))))
		{
			WinCommons.Error(Enums.MessageCode.ToWhsInHeaderCannotBeNull, "");
			gv_Doc.FocusedColumn = gv_Doc.Columns[gcDocToWhs.FieldName];
			gv_Doc.ShowEditor();
			return 1;
		}
		if (string.IsNullOrEmpty(Function.ToString(gv_Doc.GetFocusedRowCellValue(gcDocDate))))
		{
			WinCommons.Error(Enums.MessageCode.DateDeviatesFromPermissibleRange, "");
			gv_Doc.FocusedColumn = gv_Doc.Columns[gcDocDate.FieldName];
			gv_Doc.ShowEditor();
			return 1;
		}
		if (string.IsNullOrEmpty(Function.ToString(gv_Doc.GetFocusedRowCellValue(gcDocDate))))
		{
			WinCommons.Error(Enums.MessageCode.PostingDateCannotBeNull, "");
			gv_Doc.FocusedColumn = gv_Doc.Columns[gcDocDate.FieldName];
			gv_Doc.ShowEditor();
			return 1;
		}
		return 0;
	}

	private async Task GetBranch()
	{
		try
		{
			RepositoryItemLookUpEdit repositoryItemLookUpEdit = rBranch;
			repositoryItemLookUpEdit.DataSource = await _httpClient.Commons.GetBranchList(GlobalData.UserName);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task SaveWithDiServer(bool isAsDraft)
	{
		DocumentsImpl doc = gv_Doc.GetFocusedDataRow().ToDocument();
		doc.PickRmrk = Function.ToString(gv_Doc.GetFocusedRowCellValue("PickRmrk"));
		doc.ObjType = (int)ObjectType;
		doc.TypeTo = ObjectType;
		if (ObjTypeFrom != 0)
		{
			doc.ObjTypeFrom = ObjTypeFrom;
		}
		DataTable dataTable = grd_Lines.DataSourceTable.Copy();
		for (int i = 0; i < dataTable.Rows.Count; i++)
		{
			if (string.IsNullOrEmpty(Function.ToString(dataTable.Rows[i]["ItemCode"])))
			{
				dataTable.Rows.RemoveAt(i);
				i--;
			}
			else if (!isAsDraft)
			{
				if (string.IsNullOrEmpty(Function.ToString(dataTable.Rows[i][gcLineFromWhsCode.FieldName])) || string.IsNullOrEmpty(Function.ToString(dataTable.Rows[i][gcLineWhsCode.FieldName])))
				{
					WinCommons.Error(Enums.MessageCode.RowWithoutReceiptWhs, "");
					WinCommons._CloseProcessing();
					return;
				}
				if (Function.ToString(dataTable.Rows[i][gcLineFromWhsCode.FieldName]).Equals(Function.ToString(dataTable.Rows[i][gcLineWhsCode.FieldName])) && !ucBatchAndSerial1.CheckWhsManBinLocation(Function.ToString(dataTable.Rows[i][gcLineFromWhsCode.FieldName])))
				{
					WinCommons.Error(Enums.MessageCode.NoReleaseWhsHasBeenDefined, "");
					WinCommons._CloseProcessing();
					return;
				}
			}
		}
		if (!isAsDraft && dataTable.Rows.Count == 0)
		{
			WinCommons.Error(Enums.MessageCode.DataLinesIsEmpty, "");
			WinCommons._CloseProcessing();
			return;
		}
		List<Document_LinesImpl> list = dataTable.ToDocumentLine(priceSpecified: true);
		DataTable dataTable2 = ucBatchAndSerial1.CheckDataBatch(dataTable, isAsDraft);
		if (dataTable2 == null)
		{
			WinCommons._CloseProcessing();
			return;
		}
		list.LoadDataBatch(dataTable2, FormObjectType);
		DataTable dataTable3 = ucBatchAndSerial1.CheckDataBinAllowcationLine(dataTable, isAsDraft);
		if (dataTable3 == null)
		{
			WinCommons._CloseProcessing();
			return;
		}
		list.LoadBinAllowcation(dataTable3, ObjectType);
		doc.Lines = list;
		if (GlobalData.GeneralSetting.useDocWrf == "Y" && !isAsDraft && (!_onlyChangeRemark || (doc.IsDraft && (doc.WddStatus == "" || doc.WddStatus == "-"))))
		{
			using HttpClientHelper<DataTable> dtClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
			if (!(await doc.GetApprovalTemplate(dtClientHelper, ((int)FormMode).ToString(), this)))
			{
				WinCommons.Error(Enums.MessageCode.CannotGetApprovalTemplate, "");
				WinCommons._CloseProcessing();
				return;
			}
		}
		List<AttachmentFile_Impl> list2 = ucAttachment1.UploadFile();
		if (ucAttachment1.DataSource.IsNotNull() && list2.Count == 0)
		{
			return;
		}
		doc.AttachmentFiles = list2;
		string text = Function.ToString(gv_Doc.GetFocusedRowCellValue("WddStatus"));
		if (_currentRefDoc.IsNotNull())
		{
			foreach (DataRow item in _currentRefDoc.AsEnumerable())
			{
				doc.DocumentRef = new List<Document_ReferencedImpl>();
				Document_ReferencedImpl document_ReferencedImpl = new Document_ReferencedImpl();
				int num = Function.ParseInt(item["RefDocNum"]);
				if (num != 0)
				{
					document_ReferencedImpl.RefDocEntr = Function.ParseInt(item["RefDocEntr"]);
					document_ReferencedImpl.RefDocNum = num;
					document_ReferencedImpl.RefObjType = Function.ParseInt(item["RefObjType"]);
					document_ReferencedImpl.Remark = Function.ToString(item["Remark"]);
					doc.DocumentRef.Add(document_ReferencedImpl);
				}
			}
		}
		HttpResult httpResult;
		if (!isAsDraft && (!doc.IsDraft || !(text == "W") || doc.UserSign != GlobalData.UserID))
		{
			if (doc.IsDraft && _onlyChangeRemark && (doc.Document_ApprovalRequests == null || doc.Document_ApprovalRequests.Count == 0))
			{
				using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "InventoryTranfer");
				httpResult = await helper.PostAsync("SaveDraftToDocument", doc.draftKey);
			}
			else
			{
				using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, FormController);
				httpResult = await helper.PostAsync("SaveDoc", doc);
			}
		}
		else
		{
			if (doc.IsDraft && (text == "-" || (text == "W" && doc.UserSign == GlobalData.UserID)))
			{
				doc.DocEntry = doc.draftKey;
			}
			if (FormMode == Enums.FormViewModeType.Add && text == "-")
			{
				doc.DocNum = -1;
			}
			using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, FormController);
			httpResult = await helper.PostAsync("SaveAsDraft", doc);
		}
		if (string.IsNullOrEmpty(httpResult.Message))
		{
			WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
			switch (_saveAction)
			{
			case Enums.SaveAction.SaveAndSearch:
				FormMode = Enums.FormViewModeType.View;
				await LoadDataDoc(Enums.NavigateType.Current);
				break;
			case Enums.SaveAction.SaveAndClose:
				FormMode = Enums.FormViewModeType.View;
				Close();
				break;
			case Enums.SaveAction.SaveAndNew:
				FormMode = Enums.FormViewModeType.Add;
				await AddNewRowDoc();
				break;
			case Enums.SaveAction.SaveAndView:
				FromDocEntry = Function.ToString(httpResult.Content);
				FormMode = Enums.FormViewModeType.View;
				btnSearch_Click(null, null);
				break;
			}
			WinCommons._CloseProcessing();
		}
		else
		{
			WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
		}
	}

	private async Task SaveWithApiService(bool isAsDraft)
	{
		DataSet dataSet = new DataSet();
		DataTable dataTable = grd_Doc.DataSourceTable.Clone();
		DataRow dataRow = dataTable.NewRow();
		dataRow.ItemArray = gv_Doc.GetFocusedDataRow().ItemArray;
		dataRow["ObjType"] = (int)ObjectType;
		dataTable.Rows.Add(dataRow);
		dataTable.TableName = "Table doc";
		dataSet.Tables.Add(dataTable);
		DataTable dataTable2 = grd_Lines.DataSourceTable.Copy();
		for (int i = 0; i < dataTable2.Rows.Count; i++)
		{
			if (string.IsNullOrEmpty(Function.ToString(dataTable2.Rows[i]["ItemCode"])))
			{
				dataTable2.Rows.RemoveAt(i);
				i--;
			}
			else if (!isAsDraft)
			{
				if (Function.ToString(dataTable2.Rows[i][gcLineFromWhsCode.FieldName]).Equals(Function.ToString(dataTable2.Rows[i][gcLineWhsCode.FieldName])) && ucBatchAndSerial1.CheckWhsManBinLocation(Function.ToString(dataTable2.Rows[i][gcLineFromWhsCode.FieldName])))
				{
					WinCommons.Error(Enums.MessageCode.ReceiptWhsCannotBeIndenticalToReleaseWhs, "");
					WinCommons._CloseProcessing();
					return;
				}
				if (string.IsNullOrEmpty(Function.ToString(dataTable2.Rows[i][gcLineFromWhsCode.FieldName])) || string.IsNullOrEmpty(Function.ToString(dataTable2.Rows[i][gcLineWhsCode.FieldName])))
				{
					WinCommons.Error(Enums.MessageCode.LineWhsConnotBeNull, "");
					WinCommons._CloseProcessing();
					return;
				}
			}
		}
		if (!isAsDraft && dataTable2.Rows.Count == 0)
		{
			WinCommons.Error(Enums.MessageCode.DataLinesIsEmpty, "");
			WinCommons._CloseProcessing();
			return;
		}
		dataTable2.TableName = "Table Line";
		dataSet.Tables.Add(dataTable2);
		DataTable dataTable3 = ucBatchAndSerial1.CheckDataBatch(dataTable2, isAsDraft);
		if (dataTable3 == null)
		{
			WinCommons._CloseProcessing();
			return;
		}
		dataTable3.TableName = "Table batch";
		dataSet.Tables.Add(dataTable3);
		if (ucAttachment1.GetAttachmentNoSAP(out var _))
		{
			HttpResult httpResult;
			using (HttpClientHelper<HttpResult> http = new HttpClientHelper<HttpResult>(Configuration.ApiServer, FormController))
			{
				httpResult = (isAsDraft ? (await http.PostAsync("SaveAsDraftDataSet", dataSet)) : (await http.PostAsync("SaveDataSet", dataSet)));
			}
			if (string.IsNullOrEmpty(httpResult.Message))
			{
				WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
				await LoadDataDoc(Enums.NavigateType.Current);
				FormMode = Enums.FormViewModeType.View;
				WinCommons._CloseProcessing();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
			}
		}
	}

	private async Task Save(bool isAsDraft)
	{
		_ = 1;
		try
		{
			if (!isAsDraft && CheckDataDocSave() == 1)
			{
				return;
			}
			if (FormMode == Enums.FormViewModeType.Update)
			{
				if (!(grd_Lines.DataSource is DataTable source))
				{
					return;
				}
				int docEntry = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("DocEntry"));
				if (source.AsEnumerable().Any((DataRow r) => Function.ParseInt(r["DocEntry"]) != 0 && Function.ParseInt(r["DocEntry"]) != docEntry))
				{
					WinCommons.Error(Enums.MessageCode.DocumentHeaderAndDetailNotMatch, "");
					return;
				}
			}
			gv_Doc.MakeRowVisible(gv_Doc.FocusedRowHandle);
			if (isAsDraft || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSaveDocument, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) != DialogResult.No)
			{
				_saving = true;
				EnableDisableControl(isEnable: false);
				WinCommons._OpenProcessing();
				if (GlobalData.LoginSap)
				{
					await SaveWithDiServer(isAsDraft);
				}
				else
				{
					await SaveWithApiService(isAsDraft);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			EnableDisableControl(isEnable: true);
			WinCommons._CloseProcessing();
			_saving = false;
		}
	}

	private async Task LoadSeriesByPostingDate()
	{
		string postingDate = Function.ParseDateTimes(gv_Doc.GetFocusedRowCellValue("DocDate")).ToString("yyyy-MM-dd");
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons");
		DataTable dataTable = await http.GetAsync("LoadSeriesByPostingDate", new
		{
			usercode = GlobalData.UserName,
			apzObjectTypes = ObjectType,
			docSubType = "--",
			postingDate = postingDate
		});
		rDocSeries.DataSource = dataTable;
		if (!dataTable.AsEnumerable().Any((DataRow r) => Function.ParseInt(r["Series"]) == Function.ParseInt(gv_Doc.GetFocusedRowCellValue("Series"))))
		{
			DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow r) => Function.ToString(r["IsDefault"]) == "Y");
			if (dataRow != null)
			{
				gv_Doc.SetFocusedRowCellValue("Series", dataRow["Series"]);
			}
			else if (dataTable.Rows.Count > 0)
			{
				gv_Doc.SetFocusedRowCellValue("Series", dataTable.Rows[0][0]);
			}
		}
	}

	private async Task AddNewRowDoc()
	{
		try
		{
			ucBatchAndSerial1.ClearDataSource();
			DataTable dataTable = grd_Doc.DataSourceTable.Clone();
			grd_Lines.DataSourceTable.Clear();
			grd_Lines.RefreshDataSource();
			ucAttachment1.grd_File.DataSourceTable.Clear();
			grd_Logistic.DataSourceTable.Clear();
			ucAttachment1.grd_File.DataSourceTable.AcceptChanges();
			grd_Logistic.DataSourceTable.AcceptChanges();
			if (dataTable.Columns.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.CannotGetStructureToAddNew, "");
				return;
			}
			DataRow dataRow = dataTable.NewRow();
			dataRow["UserSign"] = GlobalData.UserSignLogin;
			dataRow["Filler"] = GlobalData.DefaultWhs;
			dataRow["ToWhsCode"] = GlobalData.DefaultWhs;
			dataRow[gcDocDate.FieldName] = DateTime.Now.Date;
			dataRow[gcDocDueDate.FieldName] = DateTime.Now.Date;
			dataRow[gcDocTaxDate.FieldName] = DateTime.Now.Date;
			dataRow["Branch"] = GlobalData.DefaultBranch;
			dataRow["DocTime"] = DateTime.Now.ToString("hhmm");
			dataRow[gcDocCurrency.FieldName] = GlobalData.MainCurrency;
			dataRow["DocRate"] = 1;
			dataRow["GroupNum"] = -1;
			dataRow["Currency"] = GlobalData.MainCurrency;
			dataRow["Series"] = 0;
			dataTable.Rows.Add(dataRow);
			grd_Doc.DataSource = dataTable;
			grd_Doc.RefreshDataSource();
			gv_Doc.FocusedRowHandle = 0;
			LoadSeriesByPostingDate();
			AddNewRowLine();
			if (_currentRefDoc.IsNotNull())
			{
				_currentRefDoc.Rows.Clear();
			}
			_onlyChangeRemark = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected void AddNewRowLine(bool isInsert = false)
	{
		try
		{
			DataTable dataTable = grd_Lines.DataSourceTable;
			if (isInsert)
			{
				if (gv_Lines.FocusedRowHandle > 0 && string.IsNullOrEmpty(Function.ToString(gv_Lines.GetRowCellValue(gv_Lines.FocusedRowHandle - 1, gcLineItemCode))))
				{
					return;
				}
			}
			else if (CheckExistsBankLine(dataTable))
			{
				return;
			}
			int num = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("DocEntry"));
			if (dataTable == null || dataTable.Columns.Count == 0)
			{
				dataTable = DocumentsHelper.Lines.Clone();
			}
			DataRow dataRow = dataTable.NewRow();
			dataRow["LineNum"] = GetNewLineNum();
			dataRow["DocEntry"] = num;
			dataRow["Currency"] = GlobalData.MainCurrency;
			dataRow["ManBtchSerial"] = "N";
			if (!isInsert)
			{
				dataTable.Rows.Add(dataRow);
			}
			else
			{
				dataTable.Rows.InsertAt(dataRow, gv_Lines.GetFocusedDataSourceRowIndex());
			}
			grd_Lines.DataSource = dataTable;
			grd_Lines.DataSourceTable.AcceptChanges();
			_onlyChangeRemark = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private bool CheckExistsBankLine(DataTable dtLine)
	{
		try
		{
			if (dtLine == null || dtLine.Columns.Count == 0)
			{
				return false;
			}
			for (int num = dtLine.Rows.Count - 1; num >= 0; num--)
			{
				if (string.IsNullOrEmpty(Function.ToString(dtLine.Rows[num][gcLineItemCode.FieldName])))
				{
					return true;
				}
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return true;
	}

	private int GetNewLineNum()
	{
		if (grd_Lines.DataSourceTable.IsNotNull())
		{
			DataTable dataSourceTable = grd_Lines.DataSourceTable;
			if (!dataSourceTable.IsNotNull())
			{
				return 0;
			}
			DataRow[] array = dataSourceTable.Select("isnull(ItemCode,'') <> ''");
			if (array.Length == 0)
			{
				return 0;
			}
			DataTable dataTable = array.CopyToDataTable();
			if (!dataTable.IsNotNull())
			{
				return 0;
			}
			return Function.ParseInt(dataTable.Compute("MAX(LineNum)", "")) + 1;
		}
		return 0;
	}

	private void DeleteLine()
	{
		ucBatchAndSerial1.DeleteBatchAndSerialByLineNum();
		ucBatchAndSerial1.BeginUpdateLine();
		gv_Lines.DeleteDataSourceRowSelected();
		ucBatchAndSerial1.EndUpdateLine();
		AddNewRowLine();
		ucBatchAndSerial1.SetFocusLine();
	}

	private void ChooseFromListLineItem(string itemCode)
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.ListItems, true, itemCode, "", Enums.ItemObjectType.InventoryItem, null, "");
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItems != null && frm_CFL2.ChooseItems.Count > 0)
			{
				GridColumn focusedColumn = gv_Lines.FocusedColumn;
				gv_Lines.BeginInsertRow();
				int focusedDataSourceRowIndex = gv_Lines.GetFocusedDataSourceRowIndex();
				DataTable dataSourceTable = grd_Lines.DataSourceTable;
				ucBatchAndSerial1.ClearLineDataSource();
				ucBatchAndSerial1.BeginUpdateLine();
				dataSourceTable.Rows.RemoveAt(focusedDataSourceRowIndex);
				ucBatchAndSerial1.EndUpdateLine();
				for (int num = frm_CFL2.ChooseItems.Count - 1; num >= 0; num--)
				{
					DataRow dataRow = frm_CFL2.ChooseItems[num];
					if (dataRow != null)
					{
						gv_Lines.FocusedColumn = focusedColumn;
						DataRow row = dataSourceTable.NewRow();
						dataSourceTable.Rows.InsertAt(row, focusedDataSourceRowIndex);
						dataSourceTable.AcceptChanges();
						grd_Lines.DataSource = dataSourceTable;
						grd_Lines.DataSourceTable.AcceptChanges();
						int num2 = gv_Lines.FindRowHandleByDataRow(row);
						if (num2 >= 0)
						{
							gv_Lines.FocusedRowHandle = num2;
							gv_Lines.SetFocusedRowCellValue(gcLineItemCode, dataRow["ItemCode"]);
						}
					}
				}
				double num3 = 0.0;
				double num4 = 0.0;
				for (int i = 0; i < dataSourceTable.Rows.Count; i++)
				{
					num3 += Function.ParseDouble(dataSourceTable.Rows[i]["LineTotal"]);
					num4 += Function.ParseDouble(dataSourceTable.Rows[i]["LineVat"]);
				}
				gv_Doc.SetFocusedRowCellValue("VatSum", num4);
				gv_Doc.SetFocusedRowCellValue("DocTotal", num4 + num3);
				gv_Lines.EndInsertRow();
				grd_Lines.DataSourceTable.AcceptChanges();
				grd_Lines.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		gv_Lines.EndInsertRow();
	}

	protected async Task GetNewDocNum()
	{
		try
		{
			int num;
			using (HttpClientHelper<string> helper = new HttpClientHelper<string>(Configuration.ApiServer, "Commons"))
			{
				num = Function.ParseInt(await helper.GetStringAsync("GetNewDocNum", new
				{
					objType = (int)ObjectType,
					series = Function.ParseInt(gv_Doc.GetFocusedRowCellValue(gcDocSeries)),
					bplId = Function.ParseInt(GlobalData.BranchLogin),
					usersign = Function.ParseInt(GlobalData.UserSignLogin)
				}));
			}
			gv_Doc.SetFocusedRowCellValue(gcDocNum, num);
			grd_Doc.DataSourceTable.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected virtual void EnableDisableControl(bool isEnable)
	{
		try
		{
			txtCardCode.ReadOnly = !isEnable;
			txtCardCode.Properties.Buttons[0].Visible = isEnable;
			txtDocNum.ReadOnly = !isEnable;
			txtDocStatus.ReadOnly = !isEnable;
			txtFromDate.ReadOnly = !isEnable;
			txtPriceList.ReadOnly = !isEnable;
			txtToDate.ReadOnly = !isEnable;
			btn_Save.Enabled = isEnable;
			btnCopyFrom.Enabled = isEnable && ((ObjectType == ApzObjectTypes.oStockTransfer && FormMode == Enums.FormViewModeType.Add) || (ObjectType == ApzObjectTypes.oInventoryTransferRequest && FormMode != Enums.FormViewModeType.Add && Function.ToString(gv_Doc.GetFocusedRowCellValue("DocStatus")) == "O"));
			btnSearch.Enabled = isEnable;
			btn_Cancel.Enabled = isEnable;
			gv_Doc.ReadOnly = !isEnable;
			gv_Lines.ReadOnly = !isEnable;
			contextMenuStrip_Document.Enabled = isEnable;
			contextMenuStrip1_DocLine.Enabled = isEnable;
			saveAsDraftToolStripMenuItem.Enabled = isEnable && FormMode == Enums.FormViewModeType.Add;
			cmtClose_Doc.Enabled = isEnable && ObjectType == ApzObjectTypes.oInventoryTransferRequest && Function.ToString(gv_Doc.GetFocusedRowCellValue("DocStatus")) == "O";
			ctmCancel_Doc.Enabled = isEnable && ObjectType == ApzObjectTypes.oStockTransfer && Function.ToString(gv_Doc.GetFocusedRowCellValue("DocStatus")) == "O";
			ToolStripMenuItem toolStripMenuItem = cmtDuplicate_Line;
			bool enabled = (insertRowToolStripMenuItem.Enabled = isEnable && (FormMode == Enums.FormViewModeType.Add || (ObjectType == ApzObjectTypes.oInventoryTransferRequest && Function.ToString(gv_Doc.GetFocusedRowCellValue("DocStatus")) == "O")));
			toolStripMenuItem.Enabled = enabled;
			contextMenuStrip_Document.Enabled = isEnable;
			contextMenuStrip1_DocLine.Enabled = isEnable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected void EnableDisableButtonGrid(bool isEnable)
	{
		try
		{
			gv_Doc.SetViewModeButtonEdit(isEnable);
			gv_Lines.SetViewModeButtonEdit(isEnable, isHeader: false);
			for (int i = 0; i < gv_Doc.Columns.Count; i++)
			{
				if (!gv_Doc.Columns[i].FieldName.StartsWith("U_") && !new string[3] { "TaxDate", "JrnlMemo", "Comments" }.Contains(gv_Doc.Columns[i].FieldName))
				{
					gv_Doc.Columns[i].OptionsColumn.ReadOnly = !isEnable;
				}
			}
			for (int j = 0; j < gv_Lines.Columns.Count; j++)
			{
				if (!gv_Lines.Columns[j].FieldName.StartsWith("U_"))
				{
					gv_Lines.Columns[j].OptionsColumn.ReadOnly = !isEnable;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadDataPriceList()
	{
		try
		{
			DataTable dataSource = await _httpClient.Commons.GetPriceList();
			txtPriceList.Properties.DataSource = dataSource;
			rDocPriceList.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private DataTable GetDataItemCode(string itemCode, int priceList)
	{
		try
		{
			return _httpClient.Commons.GetDataItemCode(itemCode, priceList, GlobalData.UserName, GlobalData.DefaultBranch);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new DataTable();
	}

	private async Task CancelDoc()
	{
		_ = 1;
		try
		{
			if (Function.ToString(gv_Doc.GetFocusedRowCellValue(gcDocStatus)).Equals("C"))
			{
				WinCommons.Warning(Enums.MessageCode.ThisDocumentHadBeenCanceled, "");
				return;
			}
			if (ObjectType == ApzObjectTypes.oStockTransfer && GlobalData.ListAccessForm.Where((AuthsImpl f) => f.menucode == "barSub_CancelDocGenCancelation" && f.authoriza == "F").FirstOrDefault() == null)
			{
				XtraMessageBoxEx.Show(Enums.MessageCode.NoPermissionPerformAction, Enums.MessageCode.SystemMessage, MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCancelDocument, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				WinCommons._OpenProcessing();
				string text;
				using (HttpClientHelper<string> http = new HttpClientHelper<string>(Configuration.ApiServer, FormController))
				{
					text = await http.PostAsync("CancelDoc", Function.ParseInt(gv_Doc.GetFocusedRowCellValue("DocEntry")));
				}
				if (string.IsNullOrEmpty(text))
				{
					WinCommons.Success(Enums.MessageCode.CancelSuccess, "");
					FormMode = Enums.FormViewModeType.View;
					await LoadDataDoc(Enums.NavigateType.Current);
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
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	private DataTable GetDataItemOnhand(string itemCode, string whsCode)
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons");
			return httpClientHelper.Get("GetDataItemOnhand", new { itemCode, whsCode });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return new DataTable();
		}
	}

	private void InitFormatControl()
	{
		try
		{
			txtDocNum.SetFormatControl(Enums.DisplayFormatType.Integer);
			txtFromDate.SetFormatControl(Enums.DisplayFormatType.DateTime);
			txtToDate.SetFormatControl(Enums.DisplayFormatType.DateTime);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected virtual async Task LoadDataCopyFrom(string lstDocEntry, bool showCustomDataLine = true)
	{
		_ = 2;
		try
		{
			DataSet dataSet;
			if (FromType == ApzObjectTypes.oProductionOrders)
			{
				dataSet = await HttpClient.ProductionOrders.GetTransferComponents(Function.ParseInt(lstDocEntry), ProductionQuantity);
			}
			else
			{
				DataSet dataSet2 = ((ObjectType != ApzObjectTypes.oStockTransfer) ? (await _httpClient.InventoryTransferRequest.GetDataCopyFrom(lstDocEntry, FromType)) : (await _httpClient.InventoryTransferClient.GetDataCopyFrom(lstDocEntry, FromType)));
				dataSet = dataSet2;
			}
			DataSet dataSet3 = dataSet;
			if (dataSet3 == null || dataSet3.Tables.Count < 4 || dataSet3.Tables[1] == null || dataSet3.Tables[1].Columns.Count == 0 || !dataSet3.Tables[2].IsNotNull())
			{
				WinCommons.Error(Enums.MessageCode.CannotLoadDataCopy, "");
				return;
			}
			IsLoadForm = true;
			SetDefaulData(dataSet3.Tables[1]);
			DataTable dataTable;
			if (FromType == ApzObjectTypes.oDrafts)
			{
				dataTable = dataSet3.Tables[1];
			}
			else
			{
				dataTable = dataSet3.Tables[1].Copy();
				if (dataSet3.Tables[1].Rows.Count == 0)
				{
					DataRow dataRow = dataTable.NewRow();
					dataRow["UserSign"] = GlobalData.UserSignLogin;
					dataRow["DocTime"] = DateTime.Now.ToString("hhmm");
					if (dataSet3.Tables[2].IsNotNull())
					{
						dataRow["Filler"] = dataSet3.Tables[2].Rows[0]["FromWhsCod"];
						dataRow["ToWhsCode"] = dataSet3.Tables[2].Rows[0]["WhsCode"];
						if (dataSet3.Tables[2].Columns.Contains("ProjectHeader"))
						{
							dataRow["Project"] = dataSet3.Tables[2].Rows[0]["ProjectHeader"];
						}
					}
					dataTable.Rows.Add(dataRow);
					dataTable.AcceptChanges();
				}
				else if (dataTable.Rows.Count > 1)
				{
					int num = 1;
					while (num < dataTable.Rows.Count)
					{
						dataTable.Rows.RemoveAt(1);
					}
				}
			}
			if (FromType != ApzObjectTypes.oDrafts)
			{
				dataTable.Rows[0]["DocNum"] = DBNull.Value;
			}
			grd_Doc.DataSource = dataTable;
			grd_Doc.DataSourceTable.AcceptChanges();
			if (FromType != ApzObjectTypes.oDrafts)
			{
				LoadSeriesByPostingDate();
				if (dataSet3.Tables[0].IsNotNull() && dataSet3.Tables[0].Rows.Count == 1)
				{
					gv_Doc.SetFocusedRowCellValue("CardCode", Function.ToString(dataSet3.Tables[0].Rows[0][0]));
				}
			}
			IsLoadForm = false;
			if (((FromType != ApzObjectTypes.oDrafts) & showCustomDataLine) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCustomDataLine, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.CustomLineInventoryTrnasfer, multiSelect: true, "");
				frm_ChooseFromList2.DataInventoryTransferCopy = dataSet3.Tables[2];
				frm_ChooseFromList2.ShowDialog(this);
				DataTable dataInventoryTransferCopy = frm_ChooseFromList2.DataInventoryTransferCopy;
				List<int> lstLine = (from r in dataInventoryTransferCopy.AsEnumerable()
					select Function.ParseInt(r["LineNum"])).Distinct().ToList();
				ucBatchAndSerial1.SetFormViewMode(Enums.FormViewModeType.Add);
				if (dataSet3.Tables[3].IsNotNull())
				{
					DataRow[] array = (from t in dataSet3.Tables[3].AsEnumerable()
						where !lstLine.Contains(Function.ParseInt(t["BaseLineNumber"]))
						select t).ToArray();
					for (int num2 = 0; num2 < array.Length; num2++)
					{
						dataSet3.Tables[3].Rows.Remove(array[num2]);
					}
					dataSet3.Tables[3].AcceptChanges();
					ucBatchAndSerial1.DataSourceBatchAndSerial = dataSet3.Tables[3];
				}
				else
				{
					ucBatchAndSerial1.DataSourceBatchAndSerial = DocumentsHelper.BatchSerialNumbersStruct.Copy();
				}
				if (dataSet3.Tables[4].IsNotNull())
				{
					DataRow[] array2 = (from t in dataSet3.Tables[4].AsEnumerable()
						where !lstLine.Contains(Function.ParseInt(t["BaseLineNumber"]))
						select t).ToArray();
					for (int num3 = 0; num3 < array2.Length; num3++)
					{
						dataSet3.Tables[4].Rows.Remove(array2[num3]);
					}
					dataSet3.Tables[4].AcceptChanges();
					ucBatchAndSerial1.DataSourceBinAllowLine = dataSet3.Tables[4];
				}
				else
				{
					ucBatchAndSerial1.DataSourceBinAllowLine = DocumentsHelper.BatchSerialNumbersStruct.Copy();
				}
				grd_Lines.DataSource = dataInventoryTransferCopy;
				for (int num4 = 0; num4 < gv_Lines.RowCount; num4++)
				{
					gv_Lines.SetRowCellValue(num4, "Quantity", Function.ParseDouble(gv_Lines.GetRowCellValue(num4, "Quantity")));
				}
			}
			else
			{
				ucBatchAndSerial1.SetFormViewMode(Enums.FormViewModeType.Add);
				if (dataSet3.Tables[3].IsNotNull())
				{
					ucBatchAndSerial1.DataSourceBatchAndSerial = dataSet3.Tables[3];
				}
				if (dataSet3.Tables[4].IsNotNull())
				{
					ucBatchAndSerial1.DataSourceBinAllowLine = dataSet3.Tables[4];
				}
				grd_Lines.DataSource = dataSet3.Tables[2];
			}
			if (dataSet3.Tables[0] != null && dataSet3.Tables[0].Rows.Count == 1)
			{
				gv_Doc.SetFocusedRowCellValue(gcDocCardCode, dataSet3.Tables[0].Rows[0][0]);
			}
			if (gv_Lines.FocusedRowHandle < 0)
			{
				ucBatchAndSerial1.FinishEditorBatchAndSerial();
				ucBatchAndSerial1.SetFocusLine();
			}
			AddNewRowLine();
			grd_Doc.RefreshDataSource();
			grd_Lines.RefreshDataSource();
			if (!string.IsNullOrEmpty(FromDocEntry) && FromType == ApzObjectTypes.oProductionOrders)
			{
				try
				{
					_currentRefDoc = _httpClient.ProductionOrders.GetReferencedDocument(Function.ToString(FromDocEntry));
				}
				catch (Exception ex)
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
				}
			}
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex2.Message);
		}
		FormMode = Enums.FormViewModeType.Add;
	}

	private void SetDefaulData(DataTable dt)
	{
		try
		{
			dt.Columns[gcDocFromBranch.FieldName].DefaultValue = GlobalData.BranchLogin;
			dt.Columns[gcDocDate.FieldName].DefaultValue = DateTime.Now.Date;
			dt.Columns[gcDocTaxDate.FieldName].DefaultValue = DateTime.Now.Date;
			dt.Columns[gcDocDueDate.FieldName].DefaultValue = DateTime.Now.Date;
			dt.Columns[gcDocStatus.FieldName].DefaultValue = "O";
			dt.Columns[gcDocJournalRemarks.FieldName].DefaultValue = JournalRemarksDefault;
			dt.Columns["DocCur"].DefaultValue = GlobalData.MainCurrency;
			dt.Columns["GrossBase"].DefaultValue = -5;
			dt.Columns["UpdInvnt"].DefaultValue = "C";
			dt.Columns["UserSign"].DefaultValue = GlobalData.UserSignLogin;
			dt.Columns["UserSign2"].DefaultValue = GlobalData.UserSignLogin;
			dt.Columns["PIndicator"].DefaultValue = "Default";
			if (rDocPriceList.DataSource is DataTable dataTable && dataTable.Rows.Count > 0)
			{
				dt.Columns[gcDocPriceList.FieldName].DefaultValue = dataTable.Rows[0]["Code"];
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void SetItemToDoc(DataRow buss)
	{
		try
		{
			Function.ParseInt(gv_Doc.GetFocusedRowCellValue("DocEntry"));
			DataTable dataSourceTable = grd_Doc.DataSourceTable;
			int focusedDataSourceRowIndex = gv_Doc.GetFocusedDataSourceRowIndex();
			if (dataSourceTable.IsNotNull())
			{
				dataSourceTable.Rows[focusedDataSourceRowIndex]["CardCode"] = Function.ToString(buss["CardCode"]);
				dataSourceTable.Rows[focusedDataSourceRowIndex]["CardName"] = Function.ToString(buss["CardName"]);
				dataSourceTable.Rows[focusedDataSourceRowIndex]["CntctCode"] = Function.ParseInt(buss["CntctCode"]);
				dataSourceTable.Rows[focusedDataSourceRowIndex]["DocCur"] = Function.ToString(buss["BaseCurrency"]);
				dataSourceTable.Rows[focusedDataSourceRowIndex]["DocRate"] = Function.ParseDouble(buss["Rate"]);
				dataSourceTable.Rows[focusedDataSourceRowIndex]["Currency"] = Function.ToString(buss["Currency"]);
				dataSourceTable.Rows[focusedDataSourceRowIndex]["CntctName"] = Function.ToString(buss["CntctPrsn"]);
			}
			grd_Doc.RefreshDataSource();
			grd_Doc.Refresh();
			gv_Doc.FocusedColumn = gcDocCardCode;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected virtual void LoadFormView()
	{
	}

	protected virtual void LoadDropDownSave(Enums.FormViewModeType formView)
	{
		switch (formView)
		{
		case Enums.FormViewModeType.View:
			btn_Save.DropDownControl = null;
			break;
		case Enums.FormViewModeType.Update:
		case Enums.FormViewModeType.Add:
		{
			string text = ((formView == Enums.FormViewModeType.Update) ? "Update && " : "Add && ");
			DXPopupMenu dXPopupMenu = new DXPopupMenu();
			DXMenuItem barNew = new DXMenuItem();
			barNew.Caption = text + "New";
			barNew.Click += (object s, EventArgs x) =>
			{
				try
				{
					btn_Save.Text = barNew.Caption;
					_saveAction = Enums.SaveAction.SaveAndNew;
					btn_Save.PerformClick();
				}
				catch (Exception ex)
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
					WinCommons.ErrorLineNumber(ex);
				}
			};
			dXPopupMenu.Items.Add(barNew);
			DXMenuItem barView = new DXMenuItem();
			barView.Caption = text + "View";
			barView.Click += (object s, EventArgs x) =>
			{
				try
				{
					btn_Save.Text = barView.Caption;
					_saveAction = Enums.SaveAction.SaveAndView;
					btn_Save.PerformClick();
				}
				catch (Exception ex)
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
					WinCommons.ErrorLineNumber(ex);
				}
			};
			dXPopupMenu.Items.Add(barView);
			DXMenuItem barSearch = new DXMenuItem();
			barSearch.Caption = text + "Search";
			barSearch.Click += (object s, EventArgs x) =>
			{
				try
				{
					btn_Save.Text = barSearch.Caption;
					_saveAction = Enums.SaveAction.SaveAndSearch;
					btn_Save.PerformClick();
				}
				catch (Exception ex)
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
					WinCommons.ErrorLineNumber(ex);
				}
			};
			dXPopupMenu.Items.Add(barSearch);
			DXMenuItem barClose = new DXMenuItem();
			barClose.Caption = text + "Close";
			barClose.Click += (object s, EventArgs x) =>
			{
				try
				{
					btn_Save.Text = barClose.Caption;
					_saveAction = Enums.SaveAction.SaveAndClose;
					btn_Save.PerformClick();
				}
				catch (Exception ex)
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
					WinCommons.ErrorLineNumber(ex);
				}
			};
			dXPopupMenu.Items.Add(barClose);
			btn_Save.DropDownControl = dXPopupMenu;
			break;
		}
		}
	}

	private void viewDiagramToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (gv_Doc.GetFocusedDataRow() != null)
			{
				frm_Diagram frm_Diagram2 = new frm_Diagram();
				frm_Diagram2.ObjectType = ObjectType;
				frm_Diagram2.ObjectId = Function.ToString(gv_Doc.GetFocusedRowCellValue("DocEntry"));
				frm_Diagram2.MdiParent = MdiParent;
				frm_Diagram2.Show();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void ctm_Reverse_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (gv_Doc.GetFocusedDataRow() == null)
			{
				return;
			}
			EnableDisableControl(isEnable: false);
			FormMode = Enums.FormViewModeType.Add;
			if (gv_Doc.FocusedRowHandle == int.MinValue)
			{
				return;
			}
			DataTable dtDoc;
			using (HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController))
			{
				dtDoc = await http.GetAsync("GetDataDoc", new
				{
					cardCode = "",
					docNum = 0,
					docEntry = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("DocEntry")),
					docStatus = "",
					prilist = 0,
					fromDate = "",
					toDate = "",
					type = Enums.NavigateType.Current
				});
			}
			if (dtDoc != null && dtDoc.Rows.Count == 1)
			{
				ucBatchAndSerial1.ClearDataSource();
				DataSet dataSet;
				using (HttpClientHelper<DataSet> http2 = new HttpClientHelper<DataSet>(Configuration.ApiServer, FormController))
				{
					dataSet = await http2.GetAsync("GetDataLine", new
					{
						docEntry = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("DocEntry"))
					});
				}
				if (dataSet == null || dataSet.Tables.Count != 4)
				{
					grd_Lines.DataSourceTable.Rows.Clear();
					WinCommons.Error(Enums.MessageCode.DataLinesIsEmpty, "");
					return;
				}
				DataTable dataTable = dataSet.Tables[0];
				object value2;
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					dataTable.Rows[i]["BaseType"] = -1;
					DataRow dataRow = dataTable.Rows[i];
					DataRow dataRow2 = dataTable.Rows[i];
					object obj = (dataTable.Rows[i]["BaseRef"] = DBNull.Value);
					value2 = (dataRow2["BaseLine"] = obj);
					dataRow["BaseEntry"] = value2;
					if (Function.ParseDouble(dataTable.Rows[i]["Quantity"]) > 0.0)
					{
						object value3 = dataTable.Rows[i]["FromWhsCod"];
						dataTable.Rows[i]["FromWhsCod"] = dataTable.Rows[i]["WhsCode"];
						dataTable.Rows[i]["WhsCode"] = value3;
					}
					else
					{
						dataTable.Rows[i]["Quantity"] = 0.0 - Function.ParseDouble(dataTable.Rows[i]["Quantity"]);
					}
				}
				DataTable dataTable2 = dataSet.Tables[1];
				for (int j = 0; j < dataTable2.Rows.Count; j++)
				{
					if (Function.ParseInt(dataTable2.Rows[j]["TranType"]) == 1)
					{
						dataTable2.Rows[j]["TranType"] = 2;
					}
					else
					{
						dataTable2.Rows[j]["TranType"] = 1;
					}
				}
				DataTable dataTable3 = dataSet.Tables[2];
				for (int k = 0; k < dataTable3.Rows.Count; k++)
				{
					if (Function.ParseInt(dataTable3.Rows[k]["TranType"]) == 1)
					{
						dataTable3.Rows[k]["TranType"] = 2;
					}
					else
					{
						dataTable3.Rows[k]["TranType"] = 1;
					}
				}
				dtDoc.Rows[0]["DocEntry"] = DBNull.Value;
				DataRow dataRow3 = dtDoc.Rows[0];
				value2 = (dtDoc.Rows[0]["TaxDate"] = DateTime.Today);
				dataRow3["DocDate"] = value2;
				dtDoc.Rows[0]["Series"] = 0;
				dtDoc.AcceptChanges();
				IsLoadForm = true;
				grd_Doc.DataSource = dtDoc;
				grd_Doc.RefreshDataSource();
				LoadSeriesByPostingDate();
				ucBatchAndSerial1.DataSourceBatchAndSerial = dataTable2;
				ucBatchAndSerial1.DataSourceBinAllowLine = dataTable3;
				grd_Lines.DataSource = dataTable;
				grd_Lines.RefreshDataSource();
				AddNewRowLine();
				IsLoadForm = false;
				ucBatchAndSerial1.FinishEditorBatchAndSerial();
				ucBatchAndSerial1.SetFocusLine();
				FormMode = Enums.FormViewModeType.Add;
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.CannotGetData, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			EnableDisableControl(isEnable: true);
		}
	}

	private void VirFormInventoryTranfer_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Update || FormMode == Enums.FormViewModeType.Add)
			{
				if (!Focused)
				{
					XtraFormEx.LockWindowUpdate(Handle);
					Focus();
					XtraFormEx.LockWindowUpdate(IntPtr.Zero);
				}
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
				{
					e.Cancel = true;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_BeforeLeaveRow(object sender, RowAllowEventArgs e)
	{
		try
		{
			if (_loadingDetail || _saving)
			{
				e.Allow = false;
			}
			else if (FormMode == Enums.FormViewModeType.Update)
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
				{
					e.Allow = false;
				}
				else
				{
					grd_Doc.DataSource = _originalDocTable.Copy();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
	}

	public void InitializeComponent()
	{
		this.components = new System.ComponentModel.Container();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject11 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject12 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject13 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject14 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject15 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject16 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject17 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject18 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject19 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject20 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions4 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject21 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject22 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject23 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject24 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject25 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject26 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject27 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject28 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions5 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject29 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject30 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject31 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject32 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions6 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject33 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject34 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject35 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject36 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions4 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject37 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject38 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject39 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject40 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions5 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Interfaces.FormInventory.VirFormInventoryTranfer));
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions7 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject41 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject42 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject43 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject44 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions8 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject45 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject46 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject47 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject48 = new DevExpress.Utils.SerializableAppearanceObject();
		this.repositoryItemTextEditExPrice1 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.repositoryItemTextEditExPrice2 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.repositoryItemCalcEditEx1 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx2 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.rLineUoMGroup = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.gridSplitContainer2 = new DevExpress.XtraGrid.GridSplitContainer();
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip_Document = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addNewToolStripMenuItem_Document = new System.Windows.Forms.ToolStripMenuItem();
		this.cmtDuplicate_Return = new System.Windows.Forms.ToolStripMenuItem();
		this.cmtClose_Doc = new System.Windows.Forms.ToolStripMenuItem();
		this.ctmCancel_Doc = new System.Windows.Forms.ToolStripMenuItem();
		this.ctm_Reverse = new System.Windows.Forms.ToolStripMenuItem();
		this.baseDocumentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.targetDocumentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.ctm_NewTask = new System.Windows.Forms.ToolStripMenuItem();
		this.ctm_NewEvent = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.saveAsDraftToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.viewDiagramToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.gcDocSeries = new Apzon.Controls.Grid.GridColumnEx();
		this.rDocSeries = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcDocNum = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDocCardCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rDocCustomerCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcDocCardName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDocCntctCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rDocContactPersion = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcDocStatus = new Apzon.Controls.Grid.GridColumnEx();
		this.rDocStatus = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcDocDate = new Apzon.Controls.Grid.GridColumnEx();
		this.rDate = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.gcDocDueDate = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDocTaxDate = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDocTotal = new Apzon.Controls.Grid.GridColumnEx();
		this.rDocCal = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gcDocCurrency = new Apzon.Controls.Grid.GridColumnEx();
		this.rDocCur = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcDocFromBranch = new Apzon.Controls.Grid.GridColumnEx();
		this.rDocBranch = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcDocFromWhs = new Apzon.Controls.Grid.GridColumnEx();
		this.rDocFromWhsDoc = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcDocToWhs = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDocPriceList = new Apzon.Controls.Grid.GridColumnEx();
		this.rDocPriceList = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcDocShipTo = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDocSalesEmployee = new Apzon.Controls.Grid.GridColumnEx();
		this.rDocSalesEmployee = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcDocJournalRemarks = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDocPickPackRemarks = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDocComments = new Apzon.Controls.Grid.GridColumnEx();
		this.gcBranch = new Apzon.Controls.Grid.GridColumnEx();
		this.rBranch = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.splitContainerLine = new DevExpress.XtraEditors.SplitContainerControl();
		this.xtraTabControl1 = new Apzon.Controls.Controls.XtraTabControlEx();
		this.xtraTabPage1 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.gridSplitContainer1 = new DevExpress.XtraGrid.GridSplitContainer();
		this.grd_Lines = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1_DocLine = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.toolStripMenuItem_AddNewDocLine = new System.Windows.Forms.ToolStripMenuItem();
		this.insertRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.closeLineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteToolStripMenuItem_DocLine = new System.Windows.Forms.ToolStripMenuItem();
		this.cmtDuplicate_Line = new System.Windows.Forms.ToolStripMenuItem();
		this.newActivityToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.saveAsDraftToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.baseDocumentToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.targetDocumentToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.batchSerialNumberToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Lines = new Apzon.Controls.Grid.GridViewEx();
		this.gcLineItemCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rItemCodeLine = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcLineDscription = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineBarCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rLineBarCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcLineVendorCatNo = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineSerialNo = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineFromWhsCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rLineWareHouse = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcLineWhsCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineFactor1 = new Apzon.Controls.Grid.GridColumnEx();
		this.rLineCal = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gcLineFactor2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineFactor3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineFactor4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineQuantity = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineAvgPrice = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineInStock = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineCommitted = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineOrdered = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineProject = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_LineProject = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gcLineBlockNo = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineImportLog = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineBaseRow = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineBaseRef = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineInventoryUoM = new Apzon.Controls.Grid.GridColumnEx();
		this.rLineUseBaseUn = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcLineUoMName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineItemsperUnit = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineDistrRule = new Apzon.Controls.Grid.GridColumnEx();
		this.rLineDistrRule = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcLineInvQty = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineUoMCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rLineUoMCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcLineOpenInvQty = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineQtyinWhse = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineCommittedQtyWhse = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineOrderedQtyWhse = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineOpenCreQty = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineOpenQty = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLinePrice = new Apzon.Controls.Grid.GridColumnEx();
		this.rPriceEdit = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.gcLineCurrency = new Apzon.Controls.Grid.GridColumnEx();
		this.rLineCurrency = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcLineUoMGroup = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineTotal = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineNum = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineStatus = new Apzon.Controls.Grid.GridColumnEx();
		this.rLineStatus = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcLineTaxCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rLineTaxCode = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcLineBaseType = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineBaseEntry = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLinePriceBefDi = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineSpecPrice = new Apzon.Controls.Grid.GridColumnEx();
		this.rLineSpecPrice = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.xtraTabPage3 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_Logistic = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Logistic = new Apzon.Controls.Grid.GridViewEx();
		this.gcShipType = new Apzon.Controls.Grid.GridColumnEx();
		this.rlpShipType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcShipTo = new Apzon.Controls.Grid.GridColumnEx();
		this.rlpShipTo = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gccBillTo = new Apzon.Controls.Grid.GridColumnEx();
		this.rlpBillTo = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcConfirmed = new Apzon.Controls.Grid.GridColumnEx();
		this.rLogicticsConfirm = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gcAddress = new Apzon.Controls.Grid.GridColumnEx();
		this.gcAddress2 = new Apzon.Controls.Grid.GridColumnEx();
		this.xtraTabPage4 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.ucAttachment1 = new Apzon.Forms.UserControls.ucAttachment();
		this.ucBatchAndSerial1 = new Apzon.Forms.UserControls.BatchAndSerial.ucBatchAndSerial();
		this.xtraTabPage2 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.txtDocNum = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.lb_FromDate = new Apzon.Controls.Controls.LabelLine();
		this.btnSearch = new Apzon.Controls.Controls.SimpleButtonEx();
		this.lb_ToDate = new Apzon.Controls.Controls.LabelLine();
		this.lb_DocStatus = new Apzon.Controls.Controls.LabelLine();
		this.lb_Number = new Apzon.Controls.Controls.LabelLine();
		this.lb_PriceList = new Apzon.Controls.Controls.LabelLine();
		this.txtPriceList = new Apzon.Controls.Editor.LookUpEditEx();
		this.txtCardCode = new Apzon.Controls.Editor.ButtonEditEx();
		this.txtDocStatus = new Apzon.Controls.Editor.LookUpEditEx();
		this.txtFromDate = new Apzon.Controls.Editor.DateEditEx();
		this.txtToDate = new Apzon.Controls.Editor.DateEditEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnCopyFrom = new Apzon.Controls.Controls.DropDownButtonEx();
		this.btn_Save = new Apzon.Controls.Controls.DropDownButtonEx();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rLineUoMGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gridSplitContainer2).BeginInit();
		this.gridSplitContainer2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		this.contextMenuStrip_Document.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rDocSeries).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rDocCustomerCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rDocContactPersion).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rDocStatus).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rDate).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rDate.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rDocCal).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rDocCur).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rDocBranch).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rDocFromWhsDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rDocPriceList).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rDocSalesEmployee).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rBranch).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerLine).BeginInit();
		this.splitContainerLine.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).BeginInit();
		this.xtraTabControl1.SuspendLayout();
		this.xtraTabPage1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gridSplitContainer1).BeginInit();
		this.gridSplitContainer1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Lines).BeginInit();
		this.contextMenuStrip1_DocLine.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Lines).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rItemCodeLine).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rLineBarCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rLineWareHouse).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rLineCal).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_LineProject).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rLineUseBaseUn).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rLineDistrRule).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rLineUoMCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rPriceEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rLineCurrency).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rLineStatus).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rLineTaxCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rLineSpecPrice).BeginInit();
		this.xtraTabPage3.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Logistic).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Logistic).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rlpShipType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rlpShipTo).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rlpBillTo).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rLogicticsConfirm).BeginInit();
		this.xtraTabPage4.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txtDocNum.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtPriceList.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtCardCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtDocStatus.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtFromDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtFromDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtToDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtToDate.Properties).BeginInit();
		base.SuspendLayout();
		this.repositoryItemTextEditExPrice1.Name = "repositoryItemTextEditExPrice1";
		this.repositoryItemTextEditExPrice2.Name = "repositoryItemTextEditExPrice2";
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
		this.rLineUoMGroup.AutoHeight = false;
		this.rLineUoMGroup.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rLineUoMGroup.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name1"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Name2", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rLineUoMGroup.DisplayMember = "Name";
		this.rLineUoMGroup.Name = "rLineUoMGroup";
		this.rLineUoMGroup.NullText = "";
		this.rLineUoMGroup.ValueMember = "Code";
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(0, 57);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.gridSplitContainer2);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.splitContainerLine);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(1090, 448);
		this.splitContainerControl1.SplitterPosition = 146;
		this.splitContainerControl1.TabIndex = 33;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.gridSplitContainer2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.gridSplitContainer2.Grid = this.grd_Doc;
		this.gridSplitContainer2.Location = new System.Drawing.Point(0, 0);
		this.gridSplitContainer2.Name = "gridSplitContainer2";
		this.gridSplitContainer2.Panel1.Controls.Add(this.grd_Doc);
		this.gridSplitContainer2.Size = new System.Drawing.Size(1090, 146);
		this.gridSplitContainer2.TabIndex = 0;
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.ContextMenuStrip = this.contextMenuStrip_Document;
		this.grd_Doc.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Doc.Location = new System.Drawing.Point(0, 0);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[12]
		{
			this.rDocCustomerCode, this.rDate, this.rDocFromWhsDoc, this.rDocStatus, this.rDocSalesEmployee, this.rDocBranch, this.rDocSeries, this.rDocPriceList, this.rDocContactPersion, this.rDocCal,
			this.rDocCur, this.rBranch
		});
		this.grd_Doc.Size = new System.Drawing.Size(1090, 146);
		this.grd_Doc.TabIndex = 2;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.grd_Doc.EditorKeyDown += new System.Windows.Forms.KeyEventHandler(this.grd_Doc_EditorKeyDown);
		this.contextMenuStrip_Document.ImageScalingSize = new System.Drawing.Size(20, 20);
		this.contextMenuStrip_Document.Items.AddRange(new System.Windows.Forms.ToolStripItem[12]
		{
			this.addNewToolStripMenuItem_Document, this.cmtDuplicate_Return, this.cmtClose_Doc, this.ctmCancel_Doc, this.ctm_Reverse, this.baseDocumentToolStripMenuItem, this.targetDocumentToolStripMenuItem, this.ctm_NewTask, this.ctm_NewEvent, this.toolStripMenuItem1,
			this.saveAsDraftToolStripMenuItem, this.viewDiagramToolStripMenuItem
		});
		this.contextMenuStrip_Document.Name = "contextMenuStrip_Document";
		this.contextMenuStrip_Document.Size = new System.Drawing.Size(167, 268);
		this.addNewToolStripMenuItem_Document.Name = "addNewToolStripMenuItem_Document";
		this.addNewToolStripMenuItem_Document.Size = new System.Drawing.Size(166, 22);
		this.addNewToolStripMenuItem_Document.Text = "Add New";
		this.addNewToolStripMenuItem_Document.Click += new System.EventHandler(this.addNewToolStripMenuItem_Document_Click);
		this.cmtDuplicate_Return.Name = "cmtDuplicate_Return";
		this.cmtDuplicate_Return.Size = new System.Drawing.Size(166, 22);
		this.cmtDuplicate_Return.Text = "Duplicate";
		this.cmtDuplicate_Return.Click += new System.EventHandler(this.cmtDuplicate_Return_Click);
		this.cmtClose_Doc.Name = "cmtClose_Doc";
		this.cmtClose_Doc.Size = new System.Drawing.Size(166, 22);
		this.cmtClose_Doc.Text = "Close";
		this.cmtClose_Doc.Click += new System.EventHandler(this.cmtClose_Doc_Click);
		this.ctmCancel_Doc.Name = "ctmCancel_Doc";
		this.ctmCancel_Doc.Size = new System.Drawing.Size(166, 22);
		this.ctmCancel_Doc.Text = "Cancel";
		this.ctmCancel_Doc.Click += new System.EventHandler(this.ctmCancel_Doc_Click);
		this.ctm_Reverse.Name = "ctm_Reverse";
		this.ctm_Reverse.Size = new System.Drawing.Size(166, 22);
		this.ctm_Reverse.Text = "Reverse";
		this.ctm_Reverse.Click += new System.EventHandler(this.ctm_Reverse_Click);
		this.baseDocumentToolStripMenuItem.Name = "baseDocumentToolStripMenuItem";
		this.baseDocumentToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
		this.baseDocumentToolStripMenuItem.Text = "Base Document";
		this.baseDocumentToolStripMenuItem.Click += new System.EventHandler(this.baseDocumentToolStripMenuItem_Click);
		this.targetDocumentToolStripMenuItem.Name = "targetDocumentToolStripMenuItem";
		this.targetDocumentToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
		this.targetDocumentToolStripMenuItem.Text = "Target Document";
		this.targetDocumentToolStripMenuItem.Click += new System.EventHandler(this.targetDocumentToolStripMenuItem_Click);
		this.ctm_NewTask.Name = "ctm_NewTask";
		this.ctm_NewTask.Size = new System.Drawing.Size(166, 22);
		this.ctm_NewTask.Text = "New Task";
		this.ctm_NewTask.Click += new System.EventHandler(this.newActivityToolStripMenuItem1_Click);
		this.ctm_NewEvent.Name = "ctm_NewEvent";
		this.ctm_NewEvent.Size = new System.Drawing.Size(166, 22);
		this.ctm_NewEvent.Text = "New Event";
		this.toolStripMenuItem1.Name = "toolStripMenuItem1";
		this.toolStripMenuItem1.Size = new System.Drawing.Size(166, 22);
		this.toolStripMenuItem1.Text = "New Note";
		this.saveAsDraftToolStripMenuItem.Name = "saveAsDraftToolStripMenuItem";
		this.saveAsDraftToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
		this.saveAsDraftToolStripMenuItem.Text = "Save As Draft";
		this.saveAsDraftToolStripMenuItem.Click += new System.EventHandler(this.saveAsDraftToolStripMenuItem_Click);
		this.viewDiagramToolStripMenuItem.Name = "viewDiagramToolStripMenuItem";
		this.viewDiagramToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
		this.viewDiagramToolStripMenuItem.Text = "View Diagram";
		this.viewDiagramToolStripMenuItem.Click += new System.EventHandler(this.viewDiagramToolStripMenuItem_Click);
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = null;
		this.gv_Doc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[21]
		{
			this.gcDocSeries, this.gcDocNum, this.gcDocCardCode, this.gcDocCardName, this.gcDocCntctCode, this.gcDocStatus, this.gcDocDate, this.gcDocDueDate, this.gcDocTaxDate, this.gcDocTotal,
			this.gcDocCurrency, this.gcDocFromBranch, this.gcDocFromWhs, this.gcDocToWhs, this.gcDocPriceList, this.gcDocShipTo, this.gcDocSalesEmployee, this.gcDocJournalRemarks, this.gcDocPickPackRemarks, this.gcDocComments,
			this.gcBranch
		});
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = "DocCur";
		this.gv_Doc.FieldRate = "DocRate";
		this.gv_Doc.FormId = "oVirTransfer";
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.HasFormSetting = false;
		this.gv_Doc.IgnoreFilter = false;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 60;
		this.gv_Doc.IsApzTable = false;
		this.gv_Doc.IsInsertNewRow = false;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.IsMappingContextAction = false;
		this.gv_Doc.ItemId = "OWTR";
		this.gv_Doc.LstColumnAllowBatch = null;
		this.gv_Doc.LstColumnNotAllowBatch = null;
		this.gv_Doc.LstColumnNotEditable = null;
		this.gv_Doc.LstColumNotSetting = null;
		this.gv_Doc.LstOldUDF = null;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsView.ColumnAutoWidth = false;
		this.gv_Doc.OptionsView.ShowGroupPanel = false;
		this.gv_Doc.UseBatchMod = false;
		this.gv_Doc.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_Doc_CustomDrawRowIndicator);
		this.gv_Doc.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Doc_RowCellStyle);
		this.gv_Doc.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_Doc_ShowingEditor);
		this.gv_Doc.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_Doc_FocusedRowObjectChanged);
		this.gv_Doc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Doc_CellValueChanged);
		this.gv_Doc.BeforeLeaveRow += new DevExpress.XtraGrid.Views.Base.RowAllowEventHandler(this.gv_Doc_BeforeLeaveRow);
		this.gcDocSeries.Caption = "Code Series";
		this.gcDocSeries.ColumnEdit = this.rDocSeries;
		this.gcDocSeries.CurrencyDateField = null;
		this.gcDocSeries.CurrencyField = null;
		this.gcDocSeries.CurrencyFixed = null;
		this.gcDocSeries.CurrencyFixedRate = 0.0;
		this.gcDocSeries.CurrencyRateField = null;
		this.gcDocSeries.FieldName = "Series";
		this.gcDocSeries.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcDocSeries.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gcDocSeries.Name = "gcDocSeries";
		this.gcDocSeries.PrimaryKey = true;
		this.gcDocSeries.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcDocSeries.UnitField = null;
		this.gcDocSeries.Visible = true;
		this.gcDocSeries.VisibleIndex = 0;
		this.gcDocSeries.Width = 67;
		this.rDocSeries.AutoHeight = false;
		this.rDocSeries.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rDocSeries.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[4]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("SeriesName", "Name13"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Series", "Name14", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("BPLId", "Name13", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("IsDefault", "Name14", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rDocSeries.DisplayMember = "SeriesName";
		this.rDocSeries.Name = "rDocSeries";
		this.rDocSeries.NullText = "";
		this.rDocSeries.ShowFooter = false;
		this.rDocSeries.ShowHeader = false;
		this.rDocSeries.ValueMember = "Series";
		this.gcDocNum.Caption = "Doc Num";
		this.gcDocNum.CurrencyDateField = null;
		this.gcDocNum.CurrencyField = null;
		this.gcDocNum.CurrencyFixed = null;
		this.gcDocNum.CurrencyFixedRate = 0.0;
		this.gcDocNum.CurrencyRateField = null;
		this.gcDocNum.FieldName = "DocNum";
		this.gcDocNum.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gcDocNum.Name = "gcDocNum";
		this.gcDocNum.PrimaryKey = false;
		this.gcDocNum.UnitField = null;
		this.gcDocNum.Visible = true;
		this.gcDocNum.VisibleIndex = 1;
		this.gcDocNum.Width = 85;
		this.gcDocCardCode.Caption = "Card Code";
		this.gcDocCardCode.CflType = Apzon.Commons.Enums.Enums.TypeCFL.CustomerVendor;
		this.gcDocCardCode.ColumnEdit = this.rDocCustomerCode;
		this.gcDocCardCode.CurrencyDateField = null;
		this.gcDocCardCode.CurrencyField = null;
		this.gcDocCardCode.CurrencyFixed = null;
		this.gcDocCardCode.CurrencyFixedRate = 0.0;
		this.gcDocCardCode.CurrencyRateField = null;
		this.gcDocCardCode.FieldName = "CardCode";
		this.gcDocCardCode.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcDocCardCode.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gcDocCardCode.Name = "gcDocCardCode";
		this.gcDocCardCode.PrimaryKey = false;
		this.gcDocCardCode.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcDocCardCode.UnitField = null;
		this.gcDocCardCode.Visible = true;
		this.gcDocCardCode.VisibleIndex = 2;
		this.gcDocCardCode.Width = 91;
		this.rDocCustomerCode.AutoHeight = false;
		serializableAppearanceObject.BackColor = System.Drawing.Color.Beige;
		serializableAppearanceObject.Options.UseBackColor = true;
		serializableAppearanceObject2.BackColor = System.Drawing.Color.Beige;
		serializableAppearanceObject2.Options.UseBackColor = true;
		serializableAppearanceObject3.BackColor = System.Drawing.Color.Beige;
		serializableAppearanceObject3.Options.UseBackColor = true;
		serializableAppearanceObject4.BackColor = System.Drawing.Color.Beige;
		serializableAppearanceObject4.Options.UseBackColor = true;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		serializableAppearanceObject5.BackColor = System.Drawing.Color.Transparent;
		serializableAppearanceObject5.BorderColor = System.Drawing.Color.Transparent;
		serializableAppearanceObject5.Options.UseBackColor = true;
		serializableAppearanceObject5.Options.UseBorderColor = true;
		serializableAppearanceObject6.BackColor = System.Drawing.Color.Transparent;
		serializableAppearanceObject6.BorderColor = System.Drawing.Color.Transparent;
		serializableAppearanceObject6.Options.UseBackColor = true;
		serializableAppearanceObject6.Options.UseBorderColor = true;
		serializableAppearanceObject7.BackColor = System.Drawing.Color.Transparent;
		serializableAppearanceObject7.BorderColor = System.Drawing.Color.Transparent;
		serializableAppearanceObject7.Options.UseBackColor = true;
		serializableAppearanceObject7.Options.UseBorderColor = true;
		serializableAppearanceObject8.BackColor = System.Drawing.Color.Transparent;
		serializableAppearanceObject8.BorderColor = System.Drawing.Color.Transparent;
		serializableAppearanceObject8.Options.UseBackColor = true;
		serializableAppearanceObject8.Options.UseBorderColor = true;
		this.rDocCustomerCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Ellipsis, "", -1, true, true, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject, serializableAppearanceObject2, serializableAppearanceObject3, serializableAppearanceObject4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject5, serializableAppearanceObject6, serializableAppearanceObject7, serializableAppearanceObject8, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rDocCustomerCode.Name = "rDocCustomerCode";
		this.rDocCustomerCode.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_CustomerCode_ButtonClick);
		this.gcDocCardName.Caption = "Name";
		this.gcDocCardName.ColumnEdit = this.rDocCustomerCode;
		this.gcDocCardName.CurrencyDateField = null;
		this.gcDocCardName.CurrencyField = null;
		this.gcDocCardName.CurrencyFixed = null;
		this.gcDocCardName.CurrencyFixedRate = 0.0;
		this.gcDocCardName.CurrencyRateField = null;
		this.gcDocCardName.FieldName = "CardName";
		this.gcDocCardName.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcDocCardName.Name = "gcDocCardName";
		this.gcDocCardName.PrimaryKey = false;
		this.gcDocCardName.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcDocCardName.UnitField = null;
		this.gcDocCardName.Visible = true;
		this.gcDocCardName.VisibleIndex = 3;
		this.gcDocCardName.Width = 90;
		this.gcDocCntctCode.Caption = "Contact Person";
		this.gcDocCntctCode.ColumnEdit = this.rDocContactPersion;
		this.gcDocCntctCode.CurrencyDateField = null;
		this.gcDocCntctCode.CurrencyField = null;
		this.gcDocCntctCode.CurrencyFixed = null;
		this.gcDocCntctCode.CurrencyFixedRate = 0.0;
		this.gcDocCntctCode.CurrencyRateField = null;
		this.gcDocCntctCode.FieldName = "CntctName";
		this.gcDocCntctCode.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcDocCntctCode.Name = "gcDocCntctCode";
		this.gcDocCntctCode.PrimaryKey = false;
		this.gcDocCntctCode.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcDocCntctCode.UnitField = null;
		this.gcDocCntctCode.Visible = true;
		this.gcDocCntctCode.VisibleIndex = 4;
		this.gcDocCntctCode.Width = 103;
		this.rDocContactPersion.AutoHeight = false;
		serializableAppearanceObject9.BackColor = System.Drawing.Color.Beige;
		serializableAppearanceObject9.Options.UseBackColor = true;
		serializableAppearanceObject10.BackColor = System.Drawing.Color.Beige;
		serializableAppearanceObject10.Options.UseBackColor = true;
		serializableAppearanceObject11.BackColor = System.Drawing.Color.Beige;
		serializableAppearanceObject11.Options.UseBackColor = true;
		serializableAppearanceObject12.BackColor = System.Drawing.Color.Beige;
		serializableAppearanceObject12.Options.UseBackColor = true;
		this.rDocContactPersion.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Ellipsis, "", -1, true, true, false, imageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject9, serializableAppearanceObject10, serializableAppearanceObject11, serializableAppearanceObject12, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rDocContactPersion.Name = "rDocContactPersion";
		this.rDocContactPersion.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rDocContactPersion_ButtonClick);
		this.gcDocStatus.Caption = "Status";
		this.gcDocStatus.ColumnEdit = this.rDocStatus;
		this.gcDocStatus.CurrencyDateField = null;
		this.gcDocStatus.CurrencyField = null;
		this.gcDocStatus.CurrencyFixed = null;
		this.gcDocStatus.CurrencyFixedRate = 0.0;
		this.gcDocStatus.CurrencyRateField = null;
		this.gcDocStatus.FieldName = "DocStatus";
		this.gcDocStatus.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcDocStatus.Name = "gcDocStatus";
		this.gcDocStatus.OptionsColumn.ReadOnly = true;
		this.gcDocStatus.PrimaryKey = false;
		this.gcDocStatus.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gcDocStatus.UnitField = null;
		this.gcDocStatus.Visible = true;
		this.gcDocStatus.VisibleIndex = 5;
		this.gcDocStatus.Width = 83;
		this.rDocStatus.AutoHeight = false;
		this.rDocStatus.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rDocStatus.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name1"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Name2", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rDocStatus.DisplayMember = "Name";
		this.rDocStatus.Name = "rDocStatus";
		this.rDocStatus.NullText = "";
		this.rDocStatus.ShowFooter = false;
		this.rDocStatus.ShowHeader = false;
		this.rDocStatus.ValueMember = "Code";
		this.gcDocDate.Caption = "Posting Date";
		this.gcDocDate.ColumnEdit = this.rDate;
		this.gcDocDate.CurrencyDateField = null;
		this.gcDocDate.CurrencyField = null;
		this.gcDocDate.CurrencyFixed = null;
		this.gcDocDate.CurrencyFixedRate = 0.0;
		this.gcDocDate.CurrencyRateField = null;
		this.gcDocDate.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.gcDocDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.gcDocDate.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gcDocDate.FieldName = "DocDate";
		this.gcDocDate.Name = "gcDocDate";
		this.gcDocDate.PrimaryKey = false;
		this.gcDocDate.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcDocDate.UnitField = null;
		this.gcDocDate.Visible = true;
		this.gcDocDate.VisibleIndex = 6;
		this.gcDocDate.Width = 123;
		this.rDate.AutoHeight = false;
		this.rDate.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rDate.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rDate.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.rDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.rDate.Mask.EditMask = "dd/MM/yyyy";
		this.rDate.Name = "rDate";
		this.gcDocDueDate.Caption = "Due Date";
		this.gcDocDueDate.ColumnEdit = this.rDate;
		this.gcDocDueDate.CurrencyDateField = null;
		this.gcDocDueDate.CurrencyField = null;
		this.gcDocDueDate.CurrencyFixed = null;
		this.gcDocDueDate.CurrencyFixedRate = 0.0;
		this.gcDocDueDate.CurrencyRateField = null;
		this.gcDocDueDate.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.gcDocDueDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.gcDocDueDate.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gcDocDueDate.FieldName = "DocDueDate";
		this.gcDocDueDate.Name = "gcDocDueDate";
		this.gcDocDueDate.PrimaryKey = false;
		this.gcDocDueDate.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcDocDueDate.UnitField = null;
		this.gcDocDueDate.Visible = true;
		this.gcDocDueDate.VisibleIndex = 7;
		this.gcDocDueDate.Width = 93;
		this.gcDocTaxDate.Caption = "Document Date";
		this.gcDocTaxDate.ColumnEdit = this.rDate;
		this.gcDocTaxDate.CurrencyDateField = null;
		this.gcDocTaxDate.CurrencyField = null;
		this.gcDocTaxDate.CurrencyFixed = null;
		this.gcDocTaxDate.CurrencyFixedRate = 0.0;
		this.gcDocTaxDate.CurrencyRateField = null;
		this.gcDocTaxDate.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.gcDocTaxDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.gcDocTaxDate.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gcDocTaxDate.FieldName = "TaxDate";
		this.gcDocTaxDate.Name = "gcDocTaxDate";
		this.gcDocTaxDate.PrimaryKey = false;
		this.gcDocTaxDate.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcDocTaxDate.UnitField = null;
		this.gcDocTaxDate.Visible = true;
		this.gcDocTaxDate.VisibleIndex = 8;
		this.gcDocTaxDate.Width = 114;
		this.gcDocTotal.Caption = "Doc Total";
		this.gcDocTotal.ColumnEdit = this.rDocCal;
		this.gcDocTotal.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gcDocTotal.CurrencyDateField = null;
		this.gcDocTotal.CurrencyField = "DocCur";
		this.gcDocTotal.CurrencyFixed = null;
		this.gcDocTotal.CurrencyFixedRate = 0.0;
		this.gcDocTotal.CurrencyRateField = null;
		this.gcDocTotal.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gcDocTotal.FieldName = "DocTotal";
		this.gcDocTotal.Name = "gcDocTotal";
		this.gcDocTotal.OptionsColumn.ReadOnly = true;
		this.gcDocTotal.PrimaryKey = false;
		this.gcDocTotal.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gcDocTotal.UnitField = null;
		this.gcDocTotal.Visible = true;
		this.gcDocTotal.VisibleIndex = 9;
		this.rDocCal.AutoHeight = false;
		this.rDocCal.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rDocCal.Name = "rDocCal";
		this.gcDocCurrency.Caption = "Currency";
		this.gcDocCurrency.ColumnEdit = this.rDocCur;
		this.gcDocCurrency.CurrencyDateField = null;
		this.gcDocCurrency.CurrencyField = "";
		this.gcDocCurrency.CurrencyFixed = null;
		this.gcDocCurrency.CurrencyFixedRate = 0.0;
		this.gcDocCurrency.CurrencyRateField = null;
		this.gcDocCurrency.FieldName = "DocCur";
		this.gcDocCurrency.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcDocCurrency.Name = "gcDocCurrency";
		this.gcDocCurrency.PrimaryKey = false;
		this.gcDocCurrency.UnitField = null;
		this.gcDocCurrency.Visible = true;
		this.gcDocCurrency.VisibleIndex = 10;
		this.rDocCur.AutoHeight = false;
		this.rDocCur.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rDocCur.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name5"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Value", "Name6")
		});
		this.rDocCur.DisplayMember = "Value";
		this.rDocCur.Name = "rDocCur";
		this.rDocCur.NullText = "";
		this.rDocCur.ShowHeader = false;
		this.rDocCur.ValueMember = "Value";
		this.gcDocFromBranch.Caption = "From Branch";
		this.gcDocFromBranch.ColumnEdit = this.rDocBranch;
		this.gcDocFromBranch.CurrencyDateField = null;
		this.gcDocFromBranch.CurrencyField = null;
		this.gcDocFromBranch.CurrencyFixed = null;
		this.gcDocFromBranch.CurrencyFixedRate = 0.0;
		this.gcDocFromBranch.CurrencyRateField = null;
		this.gcDocFromBranch.FieldName = "BPLId";
		this.gcDocFromBranch.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcDocFromBranch.Name = "gcDocFromBranch";
		this.gcDocFromBranch.OptionsColumn.AllowEdit = false;
		this.gcDocFromBranch.PrimaryKey = false;
		this.gcDocFromBranch.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcDocFromBranch.UnitField = null;
		this.gcDocFromBranch.Visible = true;
		this.gcDocFromBranch.VisibleIndex = 11;
		this.rDocBranch.AutoHeight = false;
		this.rDocBranch.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rDocBranch.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("BPLName", "Name5"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("BPLId", "Name6", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rDocBranch.DisplayMember = "BPLName";
		this.rDocBranch.Name = "rDocBranch";
		this.rDocBranch.NullText = "";
		this.rDocBranch.ShowFooter = false;
		this.rDocBranch.ShowHeader = false;
		this.rDocBranch.ValueMember = "BPLId";
		this.gcDocFromWhs.Caption = "From Warehouse";
		this.gcDocFromWhs.CflType = Apzon.Commons.Enums.Enums.TypeCFL.Warehouse;
		this.gcDocFromWhs.ColumnEdit = this.rDocFromWhsDoc;
		this.gcDocFromWhs.CurrencyDateField = null;
		this.gcDocFromWhs.CurrencyField = null;
		this.gcDocFromWhs.CurrencyFixed = null;
		this.gcDocFromWhs.CurrencyFixedRate = 0.0;
		this.gcDocFromWhs.CurrencyRateField = null;
		this.gcDocFromWhs.FieldName = "Filler";
		this.gcDocFromWhs.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcDocFromWhs.Name = "gcDocFromWhs";
		this.gcDocFromWhs.PrimaryKey = false;
		this.gcDocFromWhs.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcDocFromWhs.UnitField = null;
		this.gcDocFromWhs.Visible = true;
		this.gcDocFromWhs.VisibleIndex = 12;
		this.gcDocFromWhs.Width = 93;
		this.rDocFromWhsDoc.AutoHeight = false;
		serializableAppearanceObject13.BackColor = System.Drawing.Color.Beige;
		serializableAppearanceObject13.Options.UseBackColor = true;
		serializableAppearanceObject14.BackColor = System.Drawing.Color.Beige;
		serializableAppearanceObject14.Options.UseBackColor = true;
		serializableAppearanceObject15.BackColor = System.Drawing.Color.Beige;
		serializableAppearanceObject15.Options.UseBackColor = true;
		serializableAppearanceObject16.BackColor = System.Drawing.Color.Beige;
		serializableAppearanceObject16.Options.UseBackColor = true;
		editorButtonImageOptions2.Image = Apzon.Forms.Properties.Resources.right;
		serializableAppearanceObject17.BackColor = System.Drawing.Color.Transparent;
		serializableAppearanceObject17.BorderColor = System.Drawing.Color.Transparent;
		serializableAppearanceObject17.Options.UseBackColor = true;
		serializableAppearanceObject17.Options.UseBorderColor = true;
		serializableAppearanceObject18.BackColor = System.Drawing.Color.Transparent;
		serializableAppearanceObject18.BorderColor = System.Drawing.Color.Transparent;
		serializableAppearanceObject18.Options.UseBackColor = true;
		serializableAppearanceObject18.Options.UseBorderColor = true;
		serializableAppearanceObject19.BackColor = System.Drawing.Color.Transparent;
		serializableAppearanceObject19.BorderColor = System.Drawing.Color.Transparent;
		serializableAppearanceObject19.Options.UseBackColor = true;
		serializableAppearanceObject19.Options.UseBorderColor = true;
		serializableAppearanceObject20.BackColor = System.Drawing.Color.Transparent;
		serializableAppearanceObject20.BorderColor = System.Drawing.Color.Transparent;
		serializableAppearanceObject20.Options.UseBackColor = true;
		serializableAppearanceObject20.Options.UseBorderColor = true;
		this.rDocFromWhsDoc.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Ellipsis, "", -1, true, true, false, imageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject13, serializableAppearanceObject14, serializableAppearanceObject15, serializableAppearanceObject16, "", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject17, serializableAppearanceObject18, serializableAppearanceObject19, serializableAppearanceObject20, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rDocFromWhsDoc.Name = "rDocFromWhsDoc";
		this.rDocFromWhsDoc.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_bte_FromWhsDoc_ButtonPressed);
		this.gcDocToWhs.Caption = "To Warehouse";
		this.gcDocToWhs.CflType = Apzon.Commons.Enums.Enums.TypeCFL.Warehouse;
		this.gcDocToWhs.ColumnEdit = this.rDocFromWhsDoc;
		this.gcDocToWhs.CurrencyDateField = null;
		this.gcDocToWhs.CurrencyField = null;
		this.gcDocToWhs.CurrencyFixed = null;
		this.gcDocToWhs.CurrencyFixedRate = 0.0;
		this.gcDocToWhs.CurrencyRateField = null;
		this.gcDocToWhs.FieldName = "ToWhsCode";
		this.gcDocToWhs.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcDocToWhs.Name = "gcDocToWhs";
		this.gcDocToWhs.PrimaryKey = false;
		this.gcDocToWhs.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcDocToWhs.UnitField = null;
		this.gcDocToWhs.Visible = true;
		this.gcDocToWhs.VisibleIndex = 13;
		this.gcDocToWhs.Width = 103;
		this.gcDocPriceList.Caption = "ListName";
		this.gcDocPriceList.ColumnEdit = this.rDocPriceList;
		this.gcDocPriceList.CurrencyDateField = null;
		this.gcDocPriceList.CurrencyField = null;
		this.gcDocPriceList.CurrencyFixed = null;
		this.gcDocPriceList.CurrencyFixedRate = 0.0;
		this.gcDocPriceList.CurrencyRateField = null;
		this.gcDocPriceList.FieldName = "GroupNum";
		this.gcDocPriceList.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcDocPriceList.Name = "gcDocPriceList";
		this.gcDocPriceList.PrimaryKey = false;
		this.gcDocPriceList.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcDocPriceList.UnitField = null;
		this.gcDocPriceList.Visible = true;
		this.gcDocPriceList.VisibleIndex = 14;
		this.gcDocPriceList.Width = 120;
		this.rDocPriceList.AutoHeight = false;
		this.rDocPriceList.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rDocPriceList.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name1"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Name2", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rDocPriceList.DisplayMember = "Name";
		this.rDocPriceList.Name = "rDocPriceList";
		this.rDocPriceList.NullText = "";
		this.rDocPriceList.ShowFooter = false;
		this.rDocPriceList.ShowHeader = false;
		this.rDocPriceList.ValueMember = "Code";
		this.gcDocShipTo.Caption = "Ship To";
		this.gcDocShipTo.CurrencyDateField = null;
		this.gcDocShipTo.CurrencyField = null;
		this.gcDocShipTo.CurrencyFixed = null;
		this.gcDocShipTo.CurrencyFixedRate = 0.0;
		this.gcDocShipTo.CurrencyRateField = null;
		this.gcDocShipTo.FieldName = "Address";
		this.gcDocShipTo.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcDocShipTo.Name = "gcDocShipTo";
		this.gcDocShipTo.PrimaryKey = false;
		this.gcDocShipTo.UnitField = null;
		this.gcDocShipTo.Visible = true;
		this.gcDocShipTo.VisibleIndex = 15;
		this.gcDocSalesEmployee.Caption = "Sales Employee";
		this.gcDocSalesEmployee.ColumnEdit = this.rDocSalesEmployee;
		this.gcDocSalesEmployee.CurrencyDateField = null;
		this.gcDocSalesEmployee.CurrencyField = null;
		this.gcDocSalesEmployee.CurrencyFixed = null;
		this.gcDocSalesEmployee.CurrencyFixedRate = 0.0;
		this.gcDocSalesEmployee.CurrencyRateField = null;
		this.gcDocSalesEmployee.FieldName = "SlpCode";
		this.gcDocSalesEmployee.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcDocSalesEmployee.Name = "gcDocSalesEmployee";
		this.gcDocSalesEmployee.PrimaryKey = false;
		this.gcDocSalesEmployee.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcDocSalesEmployee.UnitField = null;
		this.gcDocSalesEmployee.Visible = true;
		this.gcDocSalesEmployee.VisibleIndex = 16;
		this.rDocSalesEmployee.AutoHeight = false;
		this.rDocSalesEmployee.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rDocSalesEmployee.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("SlpName", "Name1"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("SlpCode", "Name2", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rDocSalesEmployee.DisplayMember = "SlpName";
		this.rDocSalesEmployee.Name = "rDocSalesEmployee";
		this.rDocSalesEmployee.NullText = "-No Sales Employee-";
		this.rDocSalesEmployee.ShowFooter = false;
		this.rDocSalesEmployee.ShowHeader = false;
		this.rDocSalesEmployee.ValueMember = "SlpCode";
		this.gcDocJournalRemarks.Caption = "Journal Remarks";
		this.gcDocJournalRemarks.CurrencyDateField = null;
		this.gcDocJournalRemarks.CurrencyField = null;
		this.gcDocJournalRemarks.CurrencyFixed = null;
		this.gcDocJournalRemarks.CurrencyFixedRate = 0.0;
		this.gcDocJournalRemarks.CurrencyRateField = null;
		this.gcDocJournalRemarks.FieldName = "JrnlMemo";
		this.gcDocJournalRemarks.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcDocJournalRemarks.Name = "gcDocJournalRemarks";
		this.gcDocJournalRemarks.PrimaryKey = false;
		this.gcDocJournalRemarks.UnitField = null;
		this.gcDocJournalRemarks.Visible = true;
		this.gcDocJournalRemarks.VisibleIndex = 17;
		this.gcDocPickPackRemarks.Caption = "Pick and Pack Remarks";
		this.gcDocPickPackRemarks.CurrencyDateField = null;
		this.gcDocPickPackRemarks.CurrencyField = null;
		this.gcDocPickPackRemarks.CurrencyFixed = null;
		this.gcDocPickPackRemarks.CurrencyFixedRate = 0.0;
		this.gcDocPickPackRemarks.CurrencyRateField = null;
		this.gcDocPickPackRemarks.FieldName = "PickRmrk";
		this.gcDocPickPackRemarks.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcDocPickPackRemarks.Name = "gcDocPickPackRemarks";
		this.gcDocPickPackRemarks.PrimaryKey = false;
		this.gcDocPickPackRemarks.UnitField = null;
		this.gcDocPickPackRemarks.Visible = true;
		this.gcDocPickPackRemarks.VisibleIndex = 18;
		this.gcDocComments.Caption = "Remark";
		this.gcDocComments.CurrencyDateField = null;
		this.gcDocComments.CurrencyField = null;
		this.gcDocComments.CurrencyFixed = null;
		this.gcDocComments.CurrencyFixedRate = 0.0;
		this.gcDocComments.CurrencyRateField = null;
		this.gcDocComments.FieldName = "Comments";
		this.gcDocComments.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcDocComments.Name = "gcDocComments";
		this.gcDocComments.PrimaryKey = false;
		this.gcDocComments.UnitField = null;
		this.gcDocComments.Visible = true;
		this.gcDocComments.VisibleIndex = 19;
		this.gcDocComments.Width = 194;
		this.gcBranch.Caption = "Branch";
		this.gcBranch.ColumnEdit = this.rBranch;
		this.gcBranch.CurrencyDateField = null;
		this.gcBranch.CurrencyField = null;
		this.gcBranch.CurrencyFixed = null;
		this.gcBranch.CurrencyFixedRate = 0.0;
		this.gcBranch.CurrencyRateField = null;
		this.gcBranch.FieldName = "Branch";
		this.gcBranch.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcBranch.Name = "gcBranch";
		this.gcBranch.PrimaryKey = false;
		this.gcBranch.UnitField = null;
		this.gcBranch.Visible = true;
		this.gcBranch.VisibleIndex = 20;
		this.rBranch.AutoHeight = false;
		this.rBranch.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rBranch.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("BPLId", "Name9"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("BPLName", "Name10")
		});
		this.rBranch.DisplayMember = "BPLName";
		this.rBranch.Name = "rBranch";
		this.rBranch.NullText = "";
		this.rBranch.ValueMember = "BPLId";
		this.splitContainerLine.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainerLine.Horizontal = false;
		this.splitContainerLine.Location = new System.Drawing.Point(0, 0);
		this.splitContainerLine.Name = "splitContainerLine";
		this.splitContainerLine.Panel1.Controls.Add(this.xtraTabControl1);
		this.splitContainerLine.Panel1.Text = "Panel1";
		this.splitContainerLine.Panel2.Controls.Add(this.ucBatchAndSerial1);
		this.splitContainerLine.Panel2.Text = "Panel2";
		this.splitContainerLine.Size = new System.Drawing.Size(1090, 297);
		this.splitContainerLine.SplitterPosition = 194;
		this.splitContainerLine.TabIndex = 0;
		this.splitContainerLine.Text = "splitLineBatchSerial";
		this.xtraTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.xtraTabControl1.IsSearchControl = false;
		this.xtraTabControl1.Location = new System.Drawing.Point(0, 0);
		this.xtraTabControl1.Name = "xtraTabControl1";
		this.xtraTabControl1.SelectedTabPage = this.xtraTabPage1;
		this.xtraTabControl1.Size = new System.Drawing.Size(1090, 194);
		this.xtraTabControl1.TabIndex = 0;
		this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[3] { this.xtraTabPage1, this.xtraTabPage3, this.xtraTabPage4 });
		this.xtraTabPage1.Controls.Add(this.gridSplitContainer1);
		this.xtraTabPage1.EditControl = null;
		this.xtraTabPage1.EditValue = "Document Line";
		this.xtraTabPage1.FieldName = null;
		this.xtraTabPage1.IsSearchControl = false;
		this.xtraTabPage1.Name = "xtraTabPage1";
		this.xtraTabPage1.ReadOnly = false;
		this.xtraTabPage1.SettingInVisible = false;
		this.xtraTabPage1.SettingReadonly = false;
		this.xtraTabPage1.Size = new System.Drawing.Size(1088, 169);
		this.xtraTabPage1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPage1.Text = "Document Line";
		this.gridSplitContainer1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.gridSplitContainer1.Grid = this.grd_Lines;
		this.gridSplitContainer1.Location = new System.Drawing.Point(0, 0);
		this.gridSplitContainer1.Name = "gridSplitContainer1";
		this.gridSplitContainer1.Panel1.Controls.Add(this.grd_Lines);
		this.gridSplitContainer1.Size = new System.Drawing.Size(1084, 168);
		this.gridSplitContainer1.TabIndex = 0;
		this.grd_Lines.AllowPasteExcel = true;
		this.grd_Lines.ClipboardData = null;
		this.grd_Lines.ContextMenuStrip = this.contextMenuStrip1_DocLine;
		this.grd_Lines.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Lines.Location = new System.Drawing.Point(0, 0);
		this.grd_Lines.MainView = this.gv_Lines;
		this.grd_Lines.Name = "grd_Lines";
		this.grd_Lines.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[14]
		{
			this.rItemCodeLine, this.rLineCal, this.rLineBarCode, this.rLineUseBaseUn, this.rLineDistrRule, this.rLineWareHouse, this.rLineStatus, this.rLineUoMGroup, this.rLineUoMCode, this.rep_LineProject,
			this.rLineTaxCode, this.rLineCurrency, this.rPriceEdit, this.rLineSpecPrice
		});
		this.grd_Lines.Size = new System.Drawing.Size(1084, 168);
		this.grd_Lines.TabIndex = 1;
		this.grd_Lines.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Lines });
		this.grd_Lines.EditorKeyDown += new System.Windows.Forms.KeyEventHandler(this.grd_Lines_EditorKeyDown);
		this.contextMenuStrip1_DocLine.ImageScalingSize = new System.Drawing.Size(20, 20);
		this.contextMenuStrip1_DocLine.Items.AddRange(new System.Windows.Forms.ToolStripItem[10] { this.toolStripMenuItem_AddNewDocLine, this.insertRowToolStripMenuItem, this.closeLineToolStripMenuItem, this.deleteToolStripMenuItem_DocLine, this.cmtDuplicate_Line, this.newActivityToolStripMenuItem1, this.saveAsDraftToolStripMenuItem1, this.baseDocumentToolStripMenuItem1, this.targetDocumentToolStripMenuItem1, this.batchSerialNumberToolStripMenuItem });
		this.contextMenuStrip1_DocLine.Name = "contextMenuStrip1_DocLine";
		this.contextMenuStrip1_DocLine.Size = new System.Drawing.Size(185, 224);
		this.contextMenuStrip1_DocLine.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_DocLine_Opening);
		this.toolStripMenuItem_AddNewDocLine.Name = "toolStripMenuItem_AddNewDocLine";
		this.toolStripMenuItem_AddNewDocLine.Size = new System.Drawing.Size(184, 22);
		this.toolStripMenuItem_AddNewDocLine.Text = "Add New Row";
		this.toolStripMenuItem_AddNewDocLine.Click += new System.EventHandler(this.toolStripMenuItem_AddNewDocLine_Click);
		this.insertRowToolStripMenuItem.Name = "insertRowToolStripMenuItem";
		this.insertRowToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
		this.insertRowToolStripMenuItem.Text = "Insert Row";
		this.insertRowToolStripMenuItem.Click += new System.EventHandler(this.insertRowToolStripMenuItem_Click);
		this.closeLineToolStripMenuItem.Name = "closeLineToolStripMenuItem";
		this.closeLineToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
		this.closeLineToolStripMenuItem.Text = "Close Line";
		this.closeLineToolStripMenuItem.Click += new System.EventHandler(this.closeLineToolStripMenuItem_Click);
		this.deleteToolStripMenuItem_DocLine.Name = "deleteToolStripMenuItem_DocLine";
		this.deleteToolStripMenuItem_DocLine.Size = new System.Drawing.Size(184, 22);
		this.deleteToolStripMenuItem_DocLine.Text = "Delete Row";
		this.deleteToolStripMenuItem_DocLine.Click += new System.EventHandler(this.deleteToolStripMenuItem_DocLine_Click);
		this.cmtDuplicate_Line.Name = "cmtDuplicate_Line";
		this.cmtDuplicate_Line.Size = new System.Drawing.Size(184, 22);
		this.cmtDuplicate_Line.Text = "Duplicate";
		this.cmtDuplicate_Line.Click += new System.EventHandler(this.cmtDuplicate_Line_Click);
		this.newActivityToolStripMenuItem1.Name = "newActivityToolStripMenuItem1";
		this.newActivityToolStripMenuItem1.Size = new System.Drawing.Size(184, 22);
		this.newActivityToolStripMenuItem1.Text = "New Activity";
		this.newActivityToolStripMenuItem1.Click += new System.EventHandler(this.newActivityToolStripMenuItem1_Click);
		this.saveAsDraftToolStripMenuItem1.Name = "saveAsDraftToolStripMenuItem1";
		this.saveAsDraftToolStripMenuItem1.Size = new System.Drawing.Size(184, 22);
		this.saveAsDraftToolStripMenuItem1.Text = "Save As Draft";
		this.saveAsDraftToolStripMenuItem1.Click += new System.EventHandler(this.saveAsDraftToolStripMenuItem_Click);
		this.baseDocumentToolStripMenuItem1.Name = "baseDocumentToolStripMenuItem1";
		this.baseDocumentToolStripMenuItem1.Size = new System.Drawing.Size(184, 22);
		this.baseDocumentToolStripMenuItem1.Text = "Base Document";
		this.baseDocumentToolStripMenuItem1.Click += new System.EventHandler(this.baseDocumentToolStripMenuItem_Click);
		this.targetDocumentToolStripMenuItem1.Name = "targetDocumentToolStripMenuItem1";
		this.targetDocumentToolStripMenuItem1.Size = new System.Drawing.Size(184, 22);
		this.targetDocumentToolStripMenuItem1.Text = "Target Document";
		this.targetDocumentToolStripMenuItem1.Click += new System.EventHandler(this.targetDocumentToolStripMenuItem_Click);
		this.batchSerialNumberToolStripMenuItem.Name = "batchSerialNumberToolStripMenuItem";
		this.batchSerialNumberToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
		this.batchSerialNumberToolStripMenuItem.Text = "Batch/Serial Number";
		this.batchSerialNumberToolStripMenuItem.Click += new System.EventHandler(this.batchSerialNumberToolStripMenuItem_Click);
		this.gv_Lines.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Lines.ColumnKey = "ItemCode";
		this.gv_Lines.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[44]
		{
			this.gcLineItemCode, this.gcLineDscription, this.gcLineBarCode, this.gcLineVendorCatNo, this.gcLineSerialNo, this.gcLineFromWhsCode, this.gcLineWhsCode, this.gcLineFactor1, this.gcLineFactor2, this.gcLineFactor3,
			this.gcLineFactor4, this.gcLineQuantity, this.gcLineAvgPrice, this.gcLineInStock, this.gcLineCommitted, this.gcLineOrdered, this.gcLineProject, this.gcLineBlockNo, this.gcLineImportLog, this.gcLineBaseRow,
			this.gcLineBaseRef, this.gcLineInventoryUoM, this.gcLineUoMName, this.gcLineItemsperUnit, this.gcLineDistrRule, this.gcLineInvQty, this.gcLineUoMCode, this.gcLineOpenInvQty, this.gcLineQtyinWhse, this.gcLineCommittedQtyWhse,
			this.gcLineOrderedQtyWhse, this.gcLineOpenCreQty, this.gcLineOpenQty, this.gcLinePrice, this.gcLineCurrency, this.gcLineUoMGroup, this.gcLineTotal, this.gcLineNum, this.gcLineStatus, this.gcLineTaxCode,
			this.gcLineBaseType, this.gcLineBaseEntry, this.gcLinePriceBefDi, this.gcLineSpecPrice
		});
		this.gv_Lines.DisableFormSetting = false;
		this.gv_Lines.DisplayException = false;
		this.gv_Lines.EditColumn = null;
		this.gv_Lines.FieldCurrency = "Currency";
		this.gv_Lines.FieldRate = "Rate";
		this.gv_Lines.FormId = "oVirTransfer";
		this.gv_Lines.GridControl = this.grd_Lines;
		this.gv_Lines.HasFormSetting = false;
		this.gv_Lines.IgnoreFilter = false;
		this.gv_Lines.IndexEditRow = -1;
		this.gv_Lines.IndicatorWidth = 35;
		this.gv_Lines.IsApzTable = false;
		this.gv_Lines.IsInsertNewRow = false;
		this.gv_Lines.IsLoadingData = false;
		this.gv_Lines.IsMappingContextAction = false;
		this.gv_Lines.ItemId = "WTR1";
		this.gv_Lines.LstColumnAllowBatch = null;
		this.gv_Lines.LstColumnNotAllowBatch = null;
		this.gv_Lines.LstColumnNotEditable = null;
		this.gv_Lines.LstColumNotSetting = null;
		this.gv_Lines.LstOldUDF = null;
		this.gv_Lines.Name = "gv_Lines";
		this.gv_Lines.OptionsDetail.EnableMasterViewMode = false;
		this.gv_Lines.OptionsSelection.MultiSelect = true;
		this.gv_Lines.OptionsView.ColumnAutoWidth = false;
		this.gv_Lines.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Lines.OptionsView.ShowGroupPanel = false;
		this.gv_Lines.UseBatchMod = false;
		this.gv_Lines.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_Lines_CustomDrawRowIndicator);
		this.gv_Lines.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Lines_RowCellStyle);
		this.gv_Lines.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_Lines_ShowingEditor);
		this.gv_Lines.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_Lines_FocusedRowObjectChanged);
		this.gv_Lines.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Lines_CellValueChanged);
		this.gv_Lines.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Lines_CellValueChanging);
		this.gcLineItemCode.Caption = "Item Code";
		this.gcLineItemCode.CflType = Apzon.Commons.Enums.Enums.TypeCFL.Items;
		this.gcLineItemCode.ColumnEdit = this.rItemCodeLine;
		this.gcLineItemCode.CurrencyDateField = null;
		this.gcLineItemCode.CurrencyField = null;
		this.gcLineItemCode.CurrencyFixed = null;
		this.gcLineItemCode.CurrencyFixedRate = 0.0;
		this.gcLineItemCode.CurrencyRateField = null;
		this.gcLineItemCode.FieldName = "ItemCode";
		this.gcLineItemCode.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcLineItemCode.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gcLineItemCode.Name = "gcLineItemCode";
		this.gcLineItemCode.PrimaryKey = true;
		this.gcLineItemCode.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcLineItemCode.UnitField = null;
		this.gcLineItemCode.Visible = true;
		this.gcLineItemCode.VisibleIndex = 0;
		this.gcLineItemCode.Width = 116;
		this.rItemCodeLine.AutoHeight = false;
		serializableAppearanceObject21.BackColor = System.Drawing.Color.Beige;
		serializableAppearanceObject21.Options.UseBackColor = true;
		serializableAppearanceObject22.BackColor = System.Drawing.Color.Beige;
		serializableAppearanceObject22.Options.UseBackColor = true;
		serializableAppearanceObject23.BackColor = System.Drawing.Color.Beige;
		serializableAppearanceObject23.Options.UseBackColor = true;
		serializableAppearanceObject24.BackColor = System.Drawing.Color.Beige;
		serializableAppearanceObject24.Options.UseBackColor = true;
		editorButtonImageOptions3.Image = Apzon.Forms.Properties.Resources.right;
		serializableAppearanceObject25.BackColor = System.Drawing.Color.Transparent;
		serializableAppearanceObject25.BorderColor = System.Drawing.Color.Transparent;
		serializableAppearanceObject25.Options.UseBackColor = true;
		serializableAppearanceObject25.Options.UseBorderColor = true;
		serializableAppearanceObject26.BackColor = System.Drawing.Color.Transparent;
		serializableAppearanceObject26.BorderColor = System.Drawing.Color.Transparent;
		serializableAppearanceObject26.Options.UseBackColor = true;
		serializableAppearanceObject26.Options.UseBorderColor = true;
		serializableAppearanceObject27.BackColor = System.Drawing.Color.Transparent;
		serializableAppearanceObject27.BorderColor = System.Drawing.Color.Transparent;
		serializableAppearanceObject27.Options.UseBackColor = true;
		serializableAppearanceObject27.Options.UseBorderColor = true;
		serializableAppearanceObject28.BackColor = System.Drawing.Color.Transparent;
		serializableAppearanceObject28.BorderColor = System.Drawing.Color.Transparent;
		serializableAppearanceObject28.Options.UseBackColor = true;
		serializableAppearanceObject28.Options.UseBorderColor = true;
		this.rItemCodeLine.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject21, serializableAppearanceObject22, serializableAppearanceObject23, serializableAppearanceObject24, "", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject25, serializableAppearanceObject26, serializableAppearanceObject27, serializableAppearanceObject28, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rItemCodeLine.Name = "rItemCodeLine";
		this.rItemCodeLine.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_ItemCode_Line_ButtonClick);
		this.gcLineDscription.Caption = "Item Description";
		this.gcLineDscription.ColumnEdit = this.rItemCodeLine;
		this.gcLineDscription.CurrencyDateField = null;
		this.gcLineDscription.CurrencyField = null;
		this.gcLineDscription.CurrencyFixed = null;
		this.gcLineDscription.CurrencyFixedRate = 0.0;
		this.gcLineDscription.CurrencyRateField = null;
		this.gcLineDscription.FieldName = "Dscription";
		this.gcLineDscription.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcLineDscription.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gcLineDscription.Name = "gcLineDscription";
		this.gcLineDscription.PrimaryKey = false;
		this.gcLineDscription.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcLineDscription.UnitField = null;
		this.gcLineDscription.Visible = true;
		this.gcLineDscription.VisibleIndex = 1;
		this.gcLineDscription.Width = 103;
		this.gcLineBarCode.Caption = "Bar Code";
		this.gcLineBarCode.ColumnEdit = this.rLineBarCode;
		this.gcLineBarCode.CurrencyDateField = null;
		this.gcLineBarCode.CurrencyField = null;
		this.gcLineBarCode.CurrencyFixed = null;
		this.gcLineBarCode.CurrencyFixedRate = 0.0;
		this.gcLineBarCode.CurrencyRateField = null;
		this.gcLineBarCode.FieldName = "CodeBars";
		this.gcLineBarCode.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcLineBarCode.Name = "gcLineBarCode";
		this.gcLineBarCode.PrimaryKey = false;
		this.gcLineBarCode.UnitField = null;
		this.gcLineBarCode.Visible = true;
		this.gcLineBarCode.VisibleIndex = 2;
		this.rLineBarCode.AutoHeight = false;
		serializableAppearanceObject29.BackColor = System.Drawing.Color.Beige;
		serializableAppearanceObject29.Options.UseBackColor = true;
		serializableAppearanceObject30.BackColor = System.Drawing.Color.Beige;
		serializableAppearanceObject30.Options.UseBackColor = true;
		serializableAppearanceObject31.BackColor = System.Drawing.Color.Beige;
		serializableAppearanceObject31.Options.UseBackColor = true;
		serializableAppearanceObject32.BackColor = System.Drawing.Color.Beige;
		serializableAppearanceObject32.Options.UseBackColor = true;
		this.rLineBarCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions5, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject29, serializableAppearanceObject30, serializableAppearanceObject31, serializableAppearanceObject32, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rLineBarCode.Name = "rLineBarCode";
		this.gcLineVendorCatNo.Caption = "Vendor Cat. No.";
		this.gcLineVendorCatNo.CurrencyDateField = null;
		this.gcLineVendorCatNo.CurrencyField = null;
		this.gcLineVendorCatNo.CurrencyFixed = null;
		this.gcLineVendorCatNo.CurrencyFixedRate = 0.0;
		this.gcLineVendorCatNo.CurrencyRateField = null;
		this.gcLineVendorCatNo.FieldName = "VendorNum";
		this.gcLineVendorCatNo.Name = "gcLineVendorCatNo";
		this.gcLineVendorCatNo.PrimaryKey = false;
		this.gcLineVendorCatNo.UnitField = null;
		this.gcLineVendorCatNo.Visible = true;
		this.gcLineVendorCatNo.VisibleIndex = 3;
		this.gcLineSerialNo.Caption = "Serial No.";
		this.gcLineSerialNo.CurrencyDateField = null;
		this.gcLineSerialNo.CurrencyField = null;
		this.gcLineSerialNo.CurrencyFixed = null;
		this.gcLineSerialNo.CurrencyFixedRate = 0.0;
		this.gcLineSerialNo.CurrencyRateField = null;
		this.gcLineSerialNo.FieldName = "SerialNum";
		this.gcLineSerialNo.Name = "gcLineSerialNo";
		this.gcLineSerialNo.PrimaryKey = false;
		this.gcLineSerialNo.UnitField = null;
		this.gcLineSerialNo.Visible = true;
		this.gcLineSerialNo.VisibleIndex = 4;
		this.gcLineFromWhsCode.Caption = "From Warehouse";
		this.gcLineFromWhsCode.CflType = Apzon.Commons.Enums.Enums.TypeCFL.Warehouse;
		this.gcLineFromWhsCode.ColumnEdit = this.rLineWareHouse;
		this.gcLineFromWhsCode.CurrencyDateField = null;
		this.gcLineFromWhsCode.CurrencyField = null;
		this.gcLineFromWhsCode.CurrencyFixed = null;
		this.gcLineFromWhsCode.CurrencyFixedRate = 0.0;
		this.gcLineFromWhsCode.CurrencyRateField = null;
		this.gcLineFromWhsCode.FieldName = "FromWhsCod";
		this.gcLineFromWhsCode.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcLineFromWhsCode.Name = "gcLineFromWhsCode";
		this.gcLineFromWhsCode.PrimaryKey = false;
		this.gcLineFromWhsCode.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcLineFromWhsCode.UnitField = null;
		this.gcLineFromWhsCode.Visible = true;
		this.gcLineFromWhsCode.VisibleIndex = 5;
		this.gcLineFromWhsCode.Width = 102;
		this.rLineWareHouse.AutoHeight = false;
		serializableAppearanceObject33.BackColor = System.Drawing.Color.Beige;
		serializableAppearanceObject33.Options.UseBackColor = true;
		serializableAppearanceObject34.BackColor = System.Drawing.Color.Beige;
		serializableAppearanceObject34.Options.UseBackColor = true;
		serializableAppearanceObject35.BackColor = System.Drawing.Color.Beige;
		serializableAppearanceObject35.Options.UseBackColor = true;
		serializableAppearanceObject36.BackColor = System.Drawing.Color.Beige;
		serializableAppearanceObject36.Options.UseBackColor = true;
		editorButtonImageOptions4.Image = Apzon.Forms.Properties.Resources.right;
		serializableAppearanceObject37.BackColor = System.Drawing.Color.Transparent;
		serializableAppearanceObject37.BorderColor = System.Drawing.Color.Transparent;
		serializableAppearanceObject37.Options.UseBackColor = true;
		serializableAppearanceObject37.Options.UseBorderColor = true;
		serializableAppearanceObject38.BackColor = System.Drawing.Color.Transparent;
		serializableAppearanceObject38.BorderColor = System.Drawing.Color.Transparent;
		serializableAppearanceObject38.Options.UseBackColor = true;
		serializableAppearanceObject38.Options.UseBorderColor = true;
		serializableAppearanceObject39.BackColor = System.Drawing.Color.Transparent;
		serializableAppearanceObject39.BorderColor = System.Drawing.Color.Transparent;
		serializableAppearanceObject39.Options.UseBackColor = true;
		serializableAppearanceObject39.Options.UseBorderColor = true;
		serializableAppearanceObject40.BackColor = System.Drawing.Color.Transparent;
		serializableAppearanceObject40.BorderColor = System.Drawing.Color.Transparent;
		serializableAppearanceObject40.Options.UseBackColor = true;
		serializableAppearanceObject40.Options.UseBorderColor = true;
		this.rLineWareHouse.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions6, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject33, serializableAppearanceObject34, serializableAppearanceObject35, serializableAppearanceObject36, "", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject37, serializableAppearanceObject38, serializableAppearanceObject39, serializableAppearanceObject40, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rLineWareHouse.Name = "rLineWareHouse";
		this.rLineWareHouse.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rLineWhareHouse_ButtonClick);
		this.gcLineWhsCode.Caption = "To Warehouse";
		this.gcLineWhsCode.CflType = Apzon.Commons.Enums.Enums.TypeCFL.Warehouse;
		this.gcLineWhsCode.ColumnEdit = this.rLineWareHouse;
		this.gcLineWhsCode.CurrencyDateField = null;
		this.gcLineWhsCode.CurrencyField = null;
		this.gcLineWhsCode.CurrencyFixed = null;
		this.gcLineWhsCode.CurrencyFixedRate = 0.0;
		this.gcLineWhsCode.CurrencyRateField = null;
		this.gcLineWhsCode.FieldName = "WhsCode";
		this.gcLineWhsCode.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcLineWhsCode.Name = "gcLineWhsCode";
		this.gcLineWhsCode.PrimaryKey = false;
		this.gcLineWhsCode.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcLineWhsCode.UnitField = null;
		this.gcLineWhsCode.Visible = true;
		this.gcLineWhsCode.VisibleIndex = 6;
		this.gcLineWhsCode.Width = 93;
		this.gcLineFactor1.Caption = "Factor 1";
		this.gcLineFactor1.ColumnEdit = this.rLineCal;
		this.gcLineFactor1.CurrencyDateField = null;
		this.gcLineFactor1.CurrencyField = null;
		this.gcLineFactor1.CurrencyFixed = null;
		this.gcLineFactor1.CurrencyFixedRate = 0.0;
		this.gcLineFactor1.CurrencyRateField = null;
		this.gcLineFactor1.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gcLineFactor1.FieldName = "Factor1";
		this.gcLineFactor1.Name = "gcLineFactor1";
		this.gcLineFactor1.PrimaryKey = false;
		this.gcLineFactor1.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gcLineFactor1.UnitField = null;
		this.gcLineFactor1.Visible = true;
		this.gcLineFactor1.VisibleIndex = 7;
		this.rLineCal.AllowMouseWheel = false;
		this.rLineCal.AutoHeight = false;
		this.rLineCal.Name = "rLineCal";
		this.gcLineFactor2.Caption = "Factor 2";
		this.gcLineFactor2.ColumnEdit = this.rLineCal;
		this.gcLineFactor2.CurrencyDateField = null;
		this.gcLineFactor2.CurrencyField = null;
		this.gcLineFactor2.CurrencyFixed = null;
		this.gcLineFactor2.CurrencyFixedRate = 0.0;
		this.gcLineFactor2.CurrencyRateField = null;
		this.gcLineFactor2.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gcLineFactor2.FieldName = "Factor2";
		this.gcLineFactor2.Name = "gcLineFactor2";
		this.gcLineFactor2.PrimaryKey = false;
		this.gcLineFactor2.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gcLineFactor2.UnitField = null;
		this.gcLineFactor2.Visible = true;
		this.gcLineFactor2.VisibleIndex = 8;
		this.gcLineFactor3.Caption = "Factor 3";
		this.gcLineFactor3.ColumnEdit = this.rLineCal;
		this.gcLineFactor3.CurrencyDateField = null;
		this.gcLineFactor3.CurrencyField = null;
		this.gcLineFactor3.CurrencyFixed = null;
		this.gcLineFactor3.CurrencyFixedRate = 0.0;
		this.gcLineFactor3.CurrencyRateField = null;
		this.gcLineFactor3.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gcLineFactor3.FieldName = "Factor3";
		this.gcLineFactor3.Name = "gcLineFactor3";
		this.gcLineFactor3.PrimaryKey = false;
		this.gcLineFactor3.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gcLineFactor3.UnitField = null;
		this.gcLineFactor3.Visible = true;
		this.gcLineFactor3.VisibleIndex = 9;
		this.gcLineFactor4.Caption = "Factor 4";
		this.gcLineFactor4.ColumnEdit = this.rLineCal;
		this.gcLineFactor4.CurrencyDateField = null;
		this.gcLineFactor4.CurrencyField = null;
		this.gcLineFactor4.CurrencyFixed = null;
		this.gcLineFactor4.CurrencyFixedRate = 0.0;
		this.gcLineFactor4.CurrencyRateField = null;
		this.gcLineFactor4.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gcLineFactor4.FieldName = "Factor4";
		this.gcLineFactor4.Name = "gcLineFactor4";
		this.gcLineFactor4.PrimaryKey = false;
		this.gcLineFactor4.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gcLineFactor4.UnitField = null;
		this.gcLineFactor4.Visible = true;
		this.gcLineFactor4.VisibleIndex = 10;
		this.gcLineQuantity.Caption = "Quantity";
		this.gcLineQuantity.ColumnEdit = this.rLineCal;
		this.gcLineQuantity.CurrencyDateField = null;
		this.gcLineQuantity.CurrencyField = null;
		this.gcLineQuantity.CurrencyFixed = null;
		this.gcLineQuantity.CurrencyFixedRate = 0.0;
		this.gcLineQuantity.CurrencyRateField = null;
		this.gcLineQuantity.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gcLineQuantity.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gcLineQuantity.FieldName = "Quantity";
		this.gcLineQuantity.Name = "gcLineQuantity";
		this.gcLineQuantity.PrimaryKey = false;
		this.gcLineQuantity.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gcLineQuantity.UnitField = null;
		this.gcLineQuantity.Visible = true;
		this.gcLineQuantity.VisibleIndex = 11;
		this.gcLineQuantity.Width = 105;
		this.gcLineAvgPrice.Caption = "Item Cost";
		this.gcLineAvgPrice.ColumnEdit = this.repositoryItemTextEditExPrice2;
		this.gcLineAvgPrice.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.MainCurrency;
		this.gcLineAvgPrice.CurrencyDateField = null;
		this.gcLineAvgPrice.CurrencyField = null;
		this.gcLineAvgPrice.CurrencyFixed = null;
		this.gcLineAvgPrice.CurrencyFixedRate = 0.0;
		this.gcLineAvgPrice.CurrencyRateField = null;
		this.gcLineAvgPrice.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gcLineAvgPrice.FieldName = "AvgPrice";
		this.gcLineAvgPrice.Name = "gcLineAvgPrice";
		this.gcLineAvgPrice.OptionsColumn.ReadOnly = true;
		this.gcLineAvgPrice.PrimaryKey = false;
		this.gcLineAvgPrice.UnitField = null;
		this.gcLineAvgPrice.Visible = true;
		this.gcLineAvgPrice.VisibleIndex = 12;
		this.gcLineInStock.Caption = "In Stock";
		this.gcLineInStock.ColumnEdit = this.repositoryItemCalcEditEx1;
		this.gcLineInStock.CurrencyDateField = null;
		this.gcLineInStock.CurrencyField = null;
		this.gcLineInStock.CurrencyFixed = null;
		this.gcLineInStock.CurrencyFixedRate = 0.0;
		this.gcLineInStock.CurrencyRateField = null;
		this.gcLineInStock.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gcLineInStock.FieldName = "OnHandItem";
		this.gcLineInStock.Name = "gcLineInStock";
		this.gcLineInStock.OptionsColumn.AllowEdit = false;
		this.gcLineInStock.OptionsColumn.ReadOnly = true;
		this.gcLineInStock.PrimaryKey = false;
		this.gcLineInStock.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gcLineInStock.UnitField = null;
		this.gcLineInStock.Visible = true;
		this.gcLineInStock.VisibleIndex = 13;
		this.gcLineCommitted.Caption = "Committed";
		this.gcLineCommitted.ColumnEdit = this.rLineCal;
		this.gcLineCommitted.CurrencyDateField = null;
		this.gcLineCommitted.CurrencyField = null;
		this.gcLineCommitted.CurrencyFixed = null;
		this.gcLineCommitted.CurrencyFixedRate = 0.0;
		this.gcLineCommitted.CurrencyRateField = null;
		this.gcLineCommitted.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gcLineCommitted.FieldName = "IsCommittedItem";
		this.gcLineCommitted.Name = "gcLineCommitted";
		this.gcLineCommitted.OptionsColumn.AllowEdit = false;
		this.gcLineCommitted.OptionsColumn.ReadOnly = true;
		this.gcLineCommitted.PrimaryKey = false;
		this.gcLineCommitted.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gcLineCommitted.UnitField = null;
		this.gcLineCommitted.Visible = true;
		this.gcLineCommitted.VisibleIndex = 14;
		this.gcLineOrdered.Caption = "Ordered";
		this.gcLineOrdered.ColumnEdit = this.repositoryItemCalcEditEx2;
		this.gcLineOrdered.CurrencyDateField = null;
		this.gcLineOrdered.CurrencyField = null;
		this.gcLineOrdered.CurrencyFixed = null;
		this.gcLineOrdered.CurrencyFixedRate = 0.0;
		this.gcLineOrdered.CurrencyRateField = null;
		this.gcLineOrdered.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gcLineOrdered.FieldName = "OnOrderedItem";
		this.gcLineOrdered.Name = "gcLineOrdered";
		this.gcLineOrdered.OptionsColumn.AllowEdit = false;
		this.gcLineOrdered.OptionsColumn.ReadOnly = true;
		this.gcLineOrdered.PrimaryKey = false;
		this.gcLineOrdered.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gcLineOrdered.UnitField = null;
		this.gcLineOrdered.Visible = true;
		this.gcLineOrdered.VisibleIndex = 15;
		this.gcLineProject.Caption = "Project";
		this.gcLineProject.CflType = Apzon.Commons.Enums.Enums.TypeCFL.Project;
		this.gcLineProject.ColumnEdit = this.rep_LineProject;
		this.gcLineProject.CurrencyDateField = null;
		this.gcLineProject.CurrencyField = null;
		this.gcLineProject.CurrencyFixed = null;
		this.gcLineProject.CurrencyFixedRate = 0.0;
		this.gcLineProject.CurrencyRateField = null;
		this.gcLineProject.FieldName = "Project";
		this.gcLineProject.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcLineProject.Name = "gcLineProject";
		this.gcLineProject.PrimaryKey = false;
		this.gcLineProject.UnitField = null;
		this.gcLineProject.Visible = true;
		this.gcLineProject.VisibleIndex = 16;
		this.rep_LineProject.AutoHeight = false;
		this.rep_LineProject.AutoLoadDataSource = true;
		editorButtonImageOptions5.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions11.Image");
		this.rep_LineProject.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions5, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_LineProject.ContainerControl = this;
		this.rep_LineProject.DisplayMember = "PrjCode";
		this.rep_LineProject.DisplayMember2 = "";
		this.rep_LineProject.LoadDataSourceWhenEnter = false;
		this.rep_LineProject.Name = "rep_LineProject";
		this.rep_LineProject.SeparateCharacterForMulti = ';';
		this.rep_LineProject.SeparateString = ", ";
		this.rep_LineProject.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.Project;
		this.rep_LineProject.TypeDrillDown = Apzon.Commons.ApzObjectTypes.oProjectCodes;
		this.rep_LineProject.UsingAutoCompleteBox = false;
		this.rep_LineProject.UsingDataSource = true;
		this.rep_LineProject.ValidateWhenNoDataSource = false;
		this.rep_LineProject.ValueMember = "PrjCode";
		this.gcLineBlockNo.Caption = "Block No.";
		this.gcLineBlockNo.CurrencyDateField = null;
		this.gcLineBlockNo.CurrencyField = null;
		this.gcLineBlockNo.CurrencyFixed = null;
		this.gcLineBlockNo.CurrencyFixedRate = 0.0;
		this.gcLineBlockNo.CurrencyRateField = null;
		this.gcLineBlockNo.FieldName = "BlockNum";
		this.gcLineBlockNo.Name = "gcLineBlockNo";
		this.gcLineBlockNo.PrimaryKey = false;
		this.gcLineBlockNo.UnitField = null;
		this.gcLineBlockNo.Visible = true;
		this.gcLineBlockNo.VisibleIndex = 17;
		this.gcLineImportLog.Caption = "Import Log";
		this.gcLineImportLog.CurrencyDateField = null;
		this.gcLineImportLog.CurrencyField = null;
		this.gcLineImportLog.CurrencyFixed = null;
		this.gcLineImportLog.CurrencyFixedRate = 0.0;
		this.gcLineImportLog.CurrencyRateField = null;
		this.gcLineImportLog.FieldName = "ImportLog";
		this.gcLineImportLog.Name = "gcLineImportLog";
		this.gcLineImportLog.OptionsColumn.ReadOnly = true;
		this.gcLineImportLog.PrimaryKey = false;
		this.gcLineImportLog.UnitField = null;
		this.gcLineImportLog.Visible = true;
		this.gcLineImportLog.VisibleIndex = 18;
		this.gcLineBaseRow.Caption = "Base Row";
		this.gcLineBaseRow.CurrencyDateField = null;
		this.gcLineBaseRow.CurrencyField = null;
		this.gcLineBaseRow.CurrencyFixed = null;
		this.gcLineBaseRow.CurrencyFixedRate = 0.0;
		this.gcLineBaseRow.CurrencyRateField = null;
		this.gcLineBaseRow.FieldName = "BaseLine";
		this.gcLineBaseRow.Name = "gcLineBaseRow";
		this.gcLineBaseRow.OptionsColumn.AllowEdit = false;
		this.gcLineBaseRow.OptionsColumn.ReadOnly = true;
		this.gcLineBaseRow.PrimaryKey = false;
		this.gcLineBaseRow.UnitField = null;
		this.gcLineBaseRow.Visible = true;
		this.gcLineBaseRow.VisibleIndex = 19;
		this.gcLineBaseRef.Caption = "Base Ref.";
		this.gcLineBaseRef.CurrencyDateField = null;
		this.gcLineBaseRef.CurrencyField = null;
		this.gcLineBaseRef.CurrencyFixed = null;
		this.gcLineBaseRef.CurrencyFixedRate = 0.0;
		this.gcLineBaseRef.CurrencyRateField = null;
		this.gcLineBaseRef.FieldName = "BaseRef";
		this.gcLineBaseRef.Name = "gcLineBaseRef";
		this.gcLineBaseRef.OptionsColumn.AllowEdit = false;
		this.gcLineBaseRef.OptionsColumn.ReadOnly = true;
		this.gcLineBaseRef.PrimaryKey = false;
		this.gcLineBaseRef.UnitField = null;
		this.gcLineBaseRef.Visible = true;
		this.gcLineBaseRef.VisibleIndex = 20;
		this.gcLineInventoryUoM.Caption = "Inventory UoM";
		this.gcLineInventoryUoM.ColumnEdit = this.rLineUseBaseUn;
		this.gcLineInventoryUoM.CurrencyDateField = null;
		this.gcLineInventoryUoM.CurrencyField = null;
		this.gcLineInventoryUoM.CurrencyFixed = null;
		this.gcLineInventoryUoM.CurrencyFixedRate = 0.0;
		this.gcLineInventoryUoM.CurrencyRateField = null;
		this.gcLineInventoryUoM.FieldName = "UseBaseUn";
		this.gcLineInventoryUoM.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcLineInventoryUoM.Name = "gcLineInventoryUoM";
		this.gcLineInventoryUoM.PrimaryKey = false;
		this.gcLineInventoryUoM.UnitField = null;
		this.gcLineInventoryUoM.Visible = true;
		this.gcLineInventoryUoM.VisibleIndex = 21;
		this.rLineUseBaseUn.AutoHeight = false;
		this.rLineUseBaseUn.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rLineUseBaseUn.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name7"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Name8", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rLineUseBaseUn.DisplayMember = "Name";
		this.rLineUseBaseUn.Name = "rLineUseBaseUn";
		this.rLineUseBaseUn.NullText = "";
		this.rLineUseBaseUn.ShowFooter = false;
		this.rLineUseBaseUn.ShowHeader = false;
		this.rLineUseBaseUn.ValueMember = "Code";
		this.gcLineUoMName.Caption = "UoM Name";
		this.gcLineUoMName.CurrencyDateField = null;
		this.gcLineUoMName.CurrencyField = null;
		this.gcLineUoMName.CurrencyFixed = null;
		this.gcLineUoMName.CurrencyFixedRate = 0.0;
		this.gcLineUoMName.CurrencyRateField = null;
		this.gcLineUoMName.FieldName = "unitMsr";
		this.gcLineUoMName.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcLineUoMName.Name = "gcLineUoMName";
		this.gcLineUoMName.PrimaryKey = false;
		this.gcLineUoMName.UnitField = null;
		this.gcLineUoMName.Visible = true;
		this.gcLineUoMName.VisibleIndex = 22;
		this.gcLineItemsperUnit.Caption = "Items per Unit";
		this.gcLineItemsperUnit.ColumnEdit = this.rLineCal;
		this.gcLineItemsperUnit.CurrencyDateField = null;
		this.gcLineItemsperUnit.CurrencyField = null;
		this.gcLineItemsperUnit.CurrencyFixed = null;
		this.gcLineItemsperUnit.CurrencyFixedRate = 0.0;
		this.gcLineItemsperUnit.CurrencyRateField = null;
		this.gcLineItemsperUnit.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gcLineItemsperUnit.FieldName = "NumPerMsr";
		this.gcLineItemsperUnit.Name = "gcLineItemsperUnit";
		this.gcLineItemsperUnit.OptionsColumn.AllowEdit = false;
		this.gcLineItemsperUnit.OptionsColumn.ReadOnly = true;
		this.gcLineItemsperUnit.PrimaryKey = false;
		this.gcLineItemsperUnit.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gcLineItemsperUnit.UnitField = null;
		this.gcLineItemsperUnit.Visible = true;
		this.gcLineItemsperUnit.VisibleIndex = 23;
		this.gcLineDistrRule.Caption = "Distr. Rule";
		this.gcLineDistrRule.ColumnEdit = this.rLineDistrRule;
		this.gcLineDistrRule.CurrencyDateField = null;
		this.gcLineDistrRule.CurrencyField = null;
		this.gcLineDistrRule.CurrencyFixed = null;
		this.gcLineDistrRule.CurrencyFixedRate = 0.0;
		this.gcLineDistrRule.CurrencyRateField = null;
		this.gcLineDistrRule.FieldName = "Rules";
		this.gcLineDistrRule.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcLineDistrRule.Name = "gcLineDistrRule";
		this.gcLineDistrRule.PrimaryKey = false;
		this.gcLineDistrRule.UnitField = null;
		this.gcLineDistrRule.Visible = true;
		this.gcLineDistrRule.VisibleIndex = 24;
		this.rLineDistrRule.AutoHeight = false;
		serializableAppearanceObject41.BackColor = System.Drawing.Color.Beige;
		serializableAppearanceObject41.Options.UseBackColor = true;
		serializableAppearanceObject42.BackColor = System.Drawing.Color.Beige;
		serializableAppearanceObject42.Options.UseBackColor = true;
		serializableAppearanceObject43.BackColor = System.Drawing.Color.Beige;
		serializableAppearanceObject43.Options.UseBackColor = true;
		serializableAppearanceObject44.BackColor = System.Drawing.Color.Beige;
		serializableAppearanceObject44.Options.UseBackColor = true;
		this.rLineDistrRule.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions7, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject41, serializableAppearanceObject42, serializableAppearanceObject43, serializableAppearanceObject44, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rLineDistrRule.Name = "rLineDistrRule";
		this.rLineDistrRule.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rLineDistrRule_ButtonClick);
		this.gcLineInvQty.Caption = "Qty(Inventory UoM)";
		this.gcLineInvQty.ColumnEdit = this.rLineCal;
		this.gcLineInvQty.CurrencyDateField = null;
		this.gcLineInvQty.CurrencyField = null;
		this.gcLineInvQty.CurrencyFixed = null;
		this.gcLineInvQty.CurrencyFixedRate = 0.0;
		this.gcLineInvQty.CurrencyRateField = null;
		this.gcLineInvQty.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gcLineInvQty.FieldName = "InvQty";
		this.gcLineInvQty.Name = "gcLineInvQty";
		this.gcLineInvQty.PrimaryKey = false;
		this.gcLineInvQty.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gcLineInvQty.UnitField = null;
		this.gcLineInvQty.Visible = true;
		this.gcLineInvQty.VisibleIndex = 25;
		this.gcLineUoMCode.Caption = "UoM Code";
		this.gcLineUoMCode.ColumnEdit = this.rLineUoMCode;
		this.gcLineUoMCode.CurrencyDateField = null;
		this.gcLineUoMCode.CurrencyField = null;
		this.gcLineUoMCode.CurrencyFixed = null;
		this.gcLineUoMCode.CurrencyFixedRate = 0.0;
		this.gcLineUoMCode.CurrencyRateField = null;
		this.gcLineUoMCode.FieldName = "UomCode";
		this.gcLineUoMCode.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcLineUoMCode.Name = "gcLineUoMCode";
		this.gcLineUoMCode.PrimaryKey = false;
		this.gcLineUoMCode.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gcLineUoMCode.UnitField = null;
		this.gcLineUoMCode.Visible = true;
		this.gcLineUoMCode.VisibleIndex = 26;
		this.rLineUoMCode.AutoHeight = false;
		serializableAppearanceObject45.BackColor = System.Drawing.Color.Beige;
		serializableAppearanceObject45.Options.UseBackColor = true;
		serializableAppearanceObject46.BackColor = System.Drawing.Color.Beige;
		serializableAppearanceObject46.Options.UseBackColor = true;
		serializableAppearanceObject47.BackColor = System.Drawing.Color.Beige;
		serializableAppearanceObject47.Options.UseBackColor = true;
		serializableAppearanceObject48.BackColor = System.Drawing.Color.Beige;
		serializableAppearanceObject48.Options.UseBackColor = true;
		this.rLineUoMCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions8, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject45, serializableAppearanceObject46, serializableAppearanceObject47, serializableAppearanceObject48, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rLineUoMCode.Name = "rLineUoMCode";
		this.rLineUoMCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rLineUoMCode_ButtonClick);
		this.gcLineOpenInvQty.Caption = "Open Inv. Qty";
		this.gcLineOpenInvQty.ColumnEdit = this.rLineCal;
		this.gcLineOpenInvQty.CurrencyDateField = null;
		this.gcLineOpenInvQty.CurrencyField = null;
		this.gcLineOpenInvQty.CurrencyFixed = null;
		this.gcLineOpenInvQty.CurrencyFixedRate = 0.0;
		this.gcLineOpenInvQty.CurrencyRateField = null;
		this.gcLineOpenInvQty.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gcLineOpenInvQty.FieldName = "OpenInvQty";
		this.gcLineOpenInvQty.Name = "gcLineOpenInvQty";
		this.gcLineOpenInvQty.OptionsColumn.AllowEdit = false;
		this.gcLineOpenInvQty.OptionsColumn.ReadOnly = true;
		this.gcLineOpenInvQty.PrimaryKey = false;
		this.gcLineOpenInvQty.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gcLineOpenInvQty.UnitField = null;
		this.gcLineOpenInvQty.Visible = true;
		this.gcLineOpenInvQty.VisibleIndex = 27;
		this.gcLineQtyinWhse.Caption = "Qty in Whse";
		this.gcLineQtyinWhse.ColumnEdit = this.rLineCal;
		this.gcLineQtyinWhse.CurrencyDateField = null;
		this.gcLineQtyinWhse.CurrencyField = null;
		this.gcLineQtyinWhse.CurrencyFixed = null;
		this.gcLineQtyinWhse.CurrencyFixedRate = 0.0;
		this.gcLineQtyinWhse.CurrencyRateField = null;
		this.gcLineQtyinWhse.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gcLineQtyinWhse.FieldName = "OnHandWhs";
		this.gcLineQtyinWhse.Name = "gcLineQtyinWhse";
		this.gcLineQtyinWhse.OptionsColumn.AllowEdit = false;
		this.gcLineQtyinWhse.OptionsColumn.ReadOnly = true;
		this.gcLineQtyinWhse.PrimaryKey = false;
		this.gcLineQtyinWhse.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gcLineQtyinWhse.UnitField = null;
		this.gcLineQtyinWhse.Visible = true;
		this.gcLineQtyinWhse.VisibleIndex = 28;
		this.gcLineCommittedQtyWhse.Caption = "Committed Qty in Whse";
		this.gcLineCommittedQtyWhse.ColumnEdit = this.rLineCal;
		this.gcLineCommittedQtyWhse.CurrencyDateField = null;
		this.gcLineCommittedQtyWhse.CurrencyField = null;
		this.gcLineCommittedQtyWhse.CurrencyFixed = null;
		this.gcLineCommittedQtyWhse.CurrencyFixedRate = 0.0;
		this.gcLineCommittedQtyWhse.CurrencyRateField = null;
		this.gcLineCommittedQtyWhse.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gcLineCommittedQtyWhse.FieldName = "IsCommittedWhs";
		this.gcLineCommittedQtyWhse.Name = "gcLineCommittedQtyWhse";
		this.gcLineCommittedQtyWhse.OptionsColumn.AllowEdit = false;
		this.gcLineCommittedQtyWhse.OptionsColumn.ReadOnly = true;
		this.gcLineCommittedQtyWhse.PrimaryKey = false;
		this.gcLineCommittedQtyWhse.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gcLineCommittedQtyWhse.UnitField = null;
		this.gcLineCommittedQtyWhse.Visible = true;
		this.gcLineCommittedQtyWhse.VisibleIndex = 29;
		this.gcLineOrderedQtyWhse.Caption = "Ordered Qty in Whse";
		this.gcLineOrderedQtyWhse.ColumnEdit = this.rLineCal;
		this.gcLineOrderedQtyWhse.CurrencyDateField = null;
		this.gcLineOrderedQtyWhse.CurrencyField = null;
		this.gcLineOrderedQtyWhse.CurrencyFixed = null;
		this.gcLineOrderedQtyWhse.CurrencyFixedRate = 0.0;
		this.gcLineOrderedQtyWhse.CurrencyRateField = null;
		this.gcLineOrderedQtyWhse.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gcLineOrderedQtyWhse.FieldName = "OnOrderedWhs";
		this.gcLineOrderedQtyWhse.Name = "gcLineOrderedQtyWhse";
		this.gcLineOrderedQtyWhse.OptionsColumn.AllowEdit = false;
		this.gcLineOrderedQtyWhse.OptionsColumn.ReadOnly = true;
		this.gcLineOrderedQtyWhse.PrimaryKey = false;
		this.gcLineOrderedQtyWhse.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gcLineOrderedQtyWhse.UnitField = null;
		this.gcLineOrderedQtyWhse.Visible = true;
		this.gcLineOrderedQtyWhse.VisibleIndex = 30;
		this.gcLineOpenCreQty.Caption = "Open Cre Qty";
		this.gcLineOpenCreQty.ColumnEdit = this.rLineCal;
		this.gcLineOpenCreQty.CurrencyDateField = null;
		this.gcLineOpenCreQty.CurrencyField = null;
		this.gcLineOpenCreQty.CurrencyFixed = null;
		this.gcLineOpenCreQty.CurrencyFixedRate = 0.0;
		this.gcLineOpenCreQty.CurrencyRateField = null;
		this.gcLineOpenCreQty.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gcLineOpenCreQty.FieldName = "OpenCreQty";
		this.gcLineOpenCreQty.Name = "gcLineOpenCreQty";
		this.gcLineOpenCreQty.OptionsColumn.AllowEdit = false;
		this.gcLineOpenCreQty.PrimaryKey = false;
		this.gcLineOpenCreQty.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gcLineOpenCreQty.UnitField = null;
		this.gcLineOpenQty.Caption = "Open Qty";
		this.gcLineOpenQty.ColumnEdit = this.rLineCal;
		this.gcLineOpenQty.CurrencyDateField = null;
		this.gcLineOpenQty.CurrencyField = null;
		this.gcLineOpenQty.CurrencyFixed = null;
		this.gcLineOpenQty.CurrencyFixedRate = 0.0;
		this.gcLineOpenQty.CurrencyRateField = null;
		this.gcLineOpenQty.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gcLineOpenQty.FieldName = "OpenQty";
		this.gcLineOpenQty.Name = "gcLineOpenQty";
		this.gcLineOpenQty.OptionsColumn.AllowEdit = false;
		this.gcLineOpenQty.OptionsColumn.ReadOnly = true;
		this.gcLineOpenQty.PrimaryKey = false;
		this.gcLineOpenQty.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gcLineOpenQty.UnitField = null;
		this.gcLinePrice.AllowChSet = false;
		this.gcLinePrice.Caption = "Info Price After Discount";
		this.gcLinePrice.ColumnEdit = this.rPriceEdit;
		this.gcLinePrice.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gcLinePrice.CurrencyDateField = "DocDate";
		this.gcLinePrice.CurrencyField = "Currency";
		this.gcLinePrice.CurrencyFixed = null;
		this.gcLinePrice.CurrencyFixedRate = 0.0;
		this.gcLinePrice.CurrencyRateField = "Rate";
		this.gcLinePrice.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gcLinePrice.FieldName = "Price";
		this.gcLinePrice.Name = "gcLinePrice";
		this.gcLinePrice.OptionsColumn.AllowEdit = false;
		this.gcLinePrice.PrimaryKey = false;
		this.gcLinePrice.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gcLinePrice.UnitField = null;
		this.gcLinePrice.Width = 108;
		this.rPriceEdit.AutoHeight = false;
		this.rPriceEdit.Name = "rPriceEdit";
		this.gcLineCurrency.Caption = "Currency";
		this.gcLineCurrency.ColumnEdit = this.rLineCurrency;
		this.gcLineCurrency.CurrencyDateField = null;
		this.gcLineCurrency.CurrencyField = null;
		this.gcLineCurrency.CurrencyFixed = null;
		this.gcLineCurrency.CurrencyFixedRate = 0.0;
		this.gcLineCurrency.CurrencyRateField = null;
		this.gcLineCurrency.FieldName = "Currency";
		this.gcLineCurrency.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcLineCurrency.Name = "gcLineCurrency";
		this.gcLineCurrency.PrimaryKey = false;
		this.gcLineCurrency.UnitField = null;
		this.rLineCurrency.AutoHeight = false;
		this.rLineCurrency.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rLineCurrency.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name3"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Value", "Name4")
		});
		this.rLineCurrency.DisplayMember = "Value";
		this.rLineCurrency.Name = "rLineCurrency";
		this.rLineCurrency.NullText = "";
		this.rLineCurrency.ShowHeader = false;
		this.rLineCurrency.ValueMember = "Value";
		this.gcLineUoMGroup.Caption = "UoM Group";
		this.gcLineUoMGroup.ColumnEdit = this.rLineUoMGroup;
		this.gcLineUoMGroup.CurrencyDateField = null;
		this.gcLineUoMGroup.CurrencyField = null;
		this.gcLineUoMGroup.CurrencyFixed = null;
		this.gcLineUoMGroup.CurrencyFixedRate = 0.0;
		this.gcLineUoMGroup.CurrencyRateField = null;
		this.gcLineUoMGroup.FieldName = "UgpCode";
		this.gcLineUoMGroup.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcLineUoMGroup.Name = "gcLineUoMGroup";
		this.gcLineUoMGroup.OptionsColumn.AllowEdit = false;
		this.gcLineUoMGroup.OptionsColumn.ReadOnly = true;
		this.gcLineUoMGroup.PrimaryKey = false;
		this.gcLineUoMGroup.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gcLineUoMGroup.UnitField = null;
		this.gcLineUoMGroup.Visible = true;
		this.gcLineUoMGroup.VisibleIndex = 31;
		this.gcLineTotal.Caption = "Total";
		this.gcLineTotal.ColumnEdit = this.rLineCal;
		this.gcLineTotal.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gcLineTotal.CurrencyDateField = null;
		this.gcLineTotal.CurrencyField = "Currency";
		this.gcLineTotal.CurrencyFixed = null;
		this.gcLineTotal.CurrencyFixedRate = 0.0;
		this.gcLineTotal.CurrencyRateField = null;
		this.gcLineTotal.DisplayFormat.FormatString = "n3";
		this.gcLineTotal.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gcLineTotal.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gcLineTotal.FieldName = "LineTotal";
		this.gcLineTotal.Name = "gcLineTotal";
		this.gcLineTotal.PrimaryKey = false;
		this.gcLineTotal.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gcLineTotal.UnitField = null;
		this.gcLineTotal.Width = 128;
		this.gcLineNum.Caption = "LineNum";
		this.gcLineNum.CurrencyDateField = null;
		this.gcLineNum.CurrencyField = null;
		this.gcLineNum.CurrencyFixed = null;
		this.gcLineNum.CurrencyFixedRate = 0.0;
		this.gcLineNum.CurrencyRateField = null;
		this.gcLineNum.FieldName = "LineNum";
		this.gcLineNum.Name = "gcLineNum";
		this.gcLineNum.OptionsColumn.AllowEdit = false;
		this.gcLineNum.OptionsColumn.ReadOnly = true;
		this.gcLineNum.PrimaryKey = false;
		this.gcLineNum.UnitField = null;
		this.gcLineNum.Visible = true;
		this.gcLineNum.VisibleIndex = 32;
		this.gcLineNum.Width = 79;
		this.gcLineStatus.Caption = "LineStatus";
		this.gcLineStatus.ColumnEdit = this.rLineStatus;
		this.gcLineStatus.CurrencyDateField = null;
		this.gcLineStatus.CurrencyField = null;
		this.gcLineStatus.CurrencyFixed = null;
		this.gcLineStatus.CurrencyFixedRate = 0.0;
		this.gcLineStatus.CurrencyRateField = null;
		this.gcLineStatus.FieldName = "LineStatus";
		this.gcLineStatus.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcLineStatus.Name = "gcLineStatus";
		this.gcLineStatus.OptionsColumn.AllowEdit = false;
		this.gcLineStatus.OptionsColumn.ReadOnly = true;
		this.gcLineStatus.PrimaryKey = false;
		this.gcLineStatus.UnitField = null;
		this.gcLineStatus.Visible = true;
		this.gcLineStatus.VisibleIndex = 33;
		this.rLineStatus.AutoHeight = false;
		this.rLineStatus.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rLineStatus.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name1"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Name2", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rLineStatus.DisplayMember = "Name";
		this.rLineStatus.Name = "rLineStatus";
		this.rLineStatus.NullText = "";
		this.rLineStatus.ShowFooter = false;
		this.rLineStatus.ShowHeader = false;
		this.rLineStatus.ValueMember = "Code";
		this.gcLineTaxCode.Caption = "Tax Code";
		this.gcLineTaxCode.ColumnEdit = this.rLineTaxCode;
		this.gcLineTaxCode.CurrencyDateField = null;
		this.gcLineTaxCode.CurrencyField = null;
		this.gcLineTaxCode.CurrencyFixed = null;
		this.gcLineTaxCode.CurrencyFixedRate = 0.0;
		this.gcLineTaxCode.CurrencyRateField = null;
		this.gcLineTaxCode.FieldName = "VatGroup";
		this.gcLineTaxCode.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcLineTaxCode.Name = "gcLineTaxCode";
		this.gcLineTaxCode.PrimaryKey = false;
		this.gcLineTaxCode.UnitField = null;
		this.rLineTaxCode.AutoHeight = false;
		this.rLineTaxCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rLineTaxCode.DisplayMember = "Name";
		this.rLineTaxCode.Name = "rLineTaxCode";
		this.rLineTaxCode.NullText = "";
		this.rLineTaxCode.ValueMember = "Code";
		this.gcLineBaseType.Caption = "BaseType";
		this.gcLineBaseType.CurrencyDateField = null;
		this.gcLineBaseType.CurrencyField = null;
		this.gcLineBaseType.CurrencyFixed = null;
		this.gcLineBaseType.CurrencyFixedRate = 0.0;
		this.gcLineBaseType.CurrencyRateField = null;
		this.gcLineBaseType.FieldName = "BaseType";
		this.gcLineBaseType.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcLineBaseType.Name = "gcLineBaseType";
		this.gcLineBaseType.OptionsColumn.ReadOnly = true;
		this.gcLineBaseType.PrimaryKey = false;
		this.gcLineBaseType.UnitField = null;
		this.gcLineBaseType.Visible = true;
		this.gcLineBaseType.VisibleIndex = 34;
		this.gcLineBaseEntry.Caption = "Base Entry";
		this.gcLineBaseEntry.CurrencyDateField = null;
		this.gcLineBaseEntry.CurrencyField = null;
		this.gcLineBaseEntry.CurrencyFixed = null;
		this.gcLineBaseEntry.CurrencyFixedRate = 0.0;
		this.gcLineBaseEntry.CurrencyRateField = null;
		this.gcLineBaseEntry.FieldName = "BaseEntry";
		this.gcLineBaseEntry.Name = "gcLineBaseEntry";
		this.gcLineBaseEntry.OptionsColumn.ReadOnly = true;
		this.gcLineBaseEntry.PrimaryKey = false;
		this.gcLineBaseEntry.UnitField = null;
		this.gcLineBaseEntry.Visible = true;
		this.gcLineBaseEntry.VisibleIndex = 35;
		this.gcLinePriceBefDi.Caption = "Info Price";
		this.gcLinePriceBefDi.ColumnEdit = this.repositoryItemTextEditExPrice1;
		this.gcLinePriceBefDi.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gcLinePriceBefDi.CurrencyDateField = "DocDate";
		this.gcLinePriceBefDi.CurrencyField = "Currency";
		this.gcLinePriceBefDi.CurrencyFixed = "";
		this.gcLinePriceBefDi.CurrencyFixedRate = 0.0;
		this.gcLinePriceBefDi.CurrencyRateField = "Rate";
		this.gcLinePriceBefDi.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gcLinePriceBefDi.FieldName = "PriceBefDi";
		this.gcLinePriceBefDi.Name = "gcLinePriceBefDi";
		this.gcLinePriceBefDi.PrimaryKey = false;
		this.gcLinePriceBefDi.UnitField = null;
		this.gcLineSpecPrice.Caption = "Price Source";
		this.gcLineSpecPrice.ColumnEdit = this.rLineSpecPrice;
		this.gcLineSpecPrice.CurrencyDateField = null;
		this.gcLineSpecPrice.CurrencyField = null;
		this.gcLineSpecPrice.CurrencyFixed = null;
		this.gcLineSpecPrice.CurrencyFixedRate = 0.0;
		this.gcLineSpecPrice.CurrencyRateField = null;
		this.gcLineSpecPrice.FieldName = "SpecPrice";
		this.gcLineSpecPrice.Name = "gcLineSpecPrice";
		this.gcLineSpecPrice.PrimaryKey = false;
		this.gcLineSpecPrice.UnitField = null;
		this.rLineSpecPrice.AutoHeight = false;
		this.rLineSpecPrice.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.rLineSpecPrice.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rLineSpecPrice.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rLineSpecPrice.DisplayMember = "Name";
		this.rLineSpecPrice.IsSearchControl = false;
		this.rLineSpecPrice.Name = "rLineSpecPrice";
		this.rLineSpecPrice.NullText = "";
		this.rLineSpecPrice.ShowFooter = false;
		this.rLineSpecPrice.ShowHeader = false;
		this.rLineSpecPrice.ValueMember = "Code";
		this.xtraTabPage3.Controls.Add(this.grd_Logistic);
		this.xtraTabPage3.EditControl = null;
		this.xtraTabPage3.EditValue = "Logistic";
		this.xtraTabPage3.FieldName = null;
		this.xtraTabPage3.IsSearchControl = false;
		this.xtraTabPage3.Name = "xtraTabPage3";
		this.xtraTabPage3.ReadOnly = false;
		this.xtraTabPage3.SettingInVisible = false;
		this.xtraTabPage3.SettingReadonly = false;
		this.xtraTabPage3.Size = new System.Drawing.Size(1088, 169);
		this.xtraTabPage3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPage3.Text = "Logistic";
		this.grd_Logistic.AllowPasteExcel = false;
		this.grd_Logistic.ClipboardData = null;
		this.grd_Logistic.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Logistic.Location = new System.Drawing.Point(0, 0);
		this.grd_Logistic.MainView = this.gv_Logistic;
		this.grd_Logistic.Name = "grd_Logistic";
		this.grd_Logistic.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[4] { this.rlpShipType, this.rlpShipTo, this.rlpBillTo, this.rLogicticsConfirm });
		this.grd_Logistic.Size = new System.Drawing.Size(1088, 169);
		this.grd_Logistic.TabIndex = 1;
		this.grd_Logistic.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Logistic });
		this.gv_Logistic.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Logistic.ColumnKey = null;
		this.gv_Logistic.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[6] { this.gcShipType, this.gcShipTo, this.gccBillTo, this.gcConfirmed, this.gcAddress, this.gcAddress2 });
		this.gv_Logistic.DisableFormSetting = false;
		this.gv_Logistic.DisplayException = false;
		this.gv_Logistic.EditColumn = null;
		this.gv_Logistic.FieldCurrency = null;
		this.gv_Logistic.FieldRate = null;
		this.gv_Logistic.FormId = "oVirTransfer";
		this.gv_Logistic.GridControl = this.grd_Logistic;
		this.gv_Logistic.HasFormSetting = false;
		this.gv_Logistic.IgnoreFilter = false;
		this.gv_Logistic.IndexEditRow = -1;
		this.gv_Logistic.IndicatorWidth = 35;
		this.gv_Logistic.IsApzTable = false;
		this.gv_Logistic.IsInsertNewRow = false;
		this.gv_Logistic.IsLoadingData = false;
		this.gv_Logistic.IsMappingContextAction = false;
		this.gv_Logistic.ItemId = "OLGT";
		this.gv_Logistic.LstColumnAllowBatch = null;
		this.gv_Logistic.LstColumnNotAllowBatch = null;
		this.gv_Logistic.LstColumnNotEditable = null;
		this.gv_Logistic.LstColumNotSetting = null;
		this.gv_Logistic.LstOldUDF = null;
		this.gv_Logistic.Name = "gv_Logistic";
		this.gv_Logistic.OptionsDetail.EnableMasterViewMode = false;
		this.gv_Logistic.OptionsView.ShowGroupPanel = false;
		this.gv_Logistic.UseBatchMod = false;
		this.gv_Logistic.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_Logistic_CustomDrawRowIndicator);
		this.gv_Logistic.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Logistic_RowCellStyle);
		this.gcShipType.Caption = "ShipType";
		this.gcShipType.ColumnEdit = this.rlpShipType;
		this.gcShipType.CurrencyDateField = null;
		this.gcShipType.CurrencyField = null;
		this.gcShipType.CurrencyFixed = null;
		this.gcShipType.CurrencyFixedRate = 0.0;
		this.gcShipType.CurrencyRateField = null;
		this.gcShipType.FieldName = "TransportationCode";
		this.gcShipType.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcShipType.Name = "gcShipType";
		this.gcShipType.PrimaryKey = false;
		this.gcShipType.UnitField = null;
		this.gcShipType.Visible = true;
		this.gcShipType.VisibleIndex = 0;
		this.rlpShipType.AutoHeight = false;
		this.rlpShipType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rlpShipType.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rlpShipType.DisplayMember = "Name";
		this.rlpShipType.Name = "rlpShipType";
		this.rlpShipType.NullText = "Select Ship type";
		this.rlpShipType.ValueMember = "Code";
		this.gcShipTo.Caption = "Ship to";
		this.gcShipTo.ColumnEdit = this.rlpShipTo;
		this.gcShipTo.CurrencyDateField = null;
		this.gcShipTo.CurrencyField = null;
		this.gcShipTo.CurrencyFixed = null;
		this.gcShipTo.CurrencyFixedRate = 0.0;
		this.gcShipTo.CurrencyRateField = null;
		this.gcShipTo.FieldName = "ShipToCode";
		this.gcShipTo.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcShipTo.Name = "gcShipTo";
		this.gcShipTo.PrimaryKey = false;
		this.gcShipTo.UnitField = null;
		this.gcShipTo.Visible = true;
		this.gcShipTo.VisibleIndex = 1;
		this.rlpShipTo.AutoHeight = false;
		this.rlpShipTo.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rlpShipTo.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Address", "Address"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Street", "Street", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rlpShipTo.DisplayMember = "Address";
		this.rlpShipTo.Name = "rlpShipTo";
		this.rlpShipTo.NullText = "Select Shipto";
		this.rlpShipTo.ValueMember = "Address";
		this.gccBillTo.Caption = "Bill To ";
		this.gccBillTo.ColumnEdit = this.rlpBillTo;
		this.gccBillTo.CurrencyDateField = null;
		this.gccBillTo.CurrencyField = null;
		this.gccBillTo.CurrencyFixed = null;
		this.gccBillTo.CurrencyFixedRate = 0.0;
		this.gccBillTo.CurrencyRateField = null;
		this.gccBillTo.FieldName = "PayToCode";
		this.gccBillTo.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gccBillTo.Name = "gccBillTo";
		this.gccBillTo.PrimaryKey = false;
		this.gccBillTo.UnitField = null;
		this.gccBillTo.Visible = true;
		this.gccBillTo.VisibleIndex = 2;
		this.rlpBillTo.AutoHeight = false;
		this.rlpBillTo.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rlpBillTo.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Address", "Address"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Street", "Street", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rlpBillTo.DisplayMember = "Address";
		this.rlpBillTo.Name = "rlpBillTo";
		this.rlpBillTo.NullText = "Select BillTo";
		this.rlpBillTo.ValueMember = "Address";
		this.gcConfirmed.Caption = "Approval";
		this.gcConfirmed.ColumnEdit = this.rLogicticsConfirm;
		this.gcConfirmed.CurrencyDateField = null;
		this.gcConfirmed.CurrencyField = null;
		this.gcConfirmed.CurrencyFixed = null;
		this.gcConfirmed.CurrencyFixedRate = 0.0;
		this.gcConfirmed.CurrencyRateField = null;
		this.gcConfirmed.FieldName = "Confirmed";
		this.gcConfirmed.Name = "gcConfirmed";
		this.gcConfirmed.PrimaryKey = false;
		this.gcConfirmed.UnitField = null;
		this.rLogicticsConfirm.AutoHeight = false;
		this.rLogicticsConfirm.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
		this.rLogicticsConfirm.Name = "rLogicticsConfirm";
		this.rLogicticsConfirm.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.rLogicticsConfirm.ValueChecked = "Y";
		this.rLogicticsConfirm.ValueUnchecked = "N";
		this.gcAddress.Caption = "Address";
		this.gcAddress.CurrencyDateField = null;
		this.gcAddress.CurrencyField = null;
		this.gcAddress.CurrencyFixed = null;
		this.gcAddress.CurrencyFixedRate = 0.0;
		this.gcAddress.CurrencyRateField = null;
		this.gcAddress.FieldName = "Address";
		this.gcAddress.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcAddress.Name = "gcAddress";
		this.gcAddress.PrimaryKey = false;
		this.gcAddress.UnitField = null;
		this.gcAddress.Visible = true;
		this.gcAddress.VisibleIndex = 3;
		this.gcAddress2.Caption = "Address2";
		this.gcAddress2.CurrencyDateField = null;
		this.gcAddress2.CurrencyField = null;
		this.gcAddress2.CurrencyFixed = null;
		this.gcAddress2.CurrencyFixedRate = 0.0;
		this.gcAddress2.CurrencyRateField = null;
		this.gcAddress2.FieldName = "Address2";
		this.gcAddress2.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcAddress2.Name = "gcAddress2";
		this.gcAddress2.PrimaryKey = false;
		this.gcAddress2.UnitField = null;
		this.gcAddress2.Visible = true;
		this.gcAddress2.VisibleIndex = 4;
		this.xtraTabPage4.Controls.Add(this.ucAttachment1);
		this.xtraTabPage4.EditControl = null;
		this.xtraTabPage4.EditValue = "Attachments";
		this.xtraTabPage4.FieldName = null;
		this.xtraTabPage4.IsSearchControl = false;
		this.xtraTabPage4.Name = "xtraTabPage4";
		this.xtraTabPage4.ReadOnly = false;
		this.xtraTabPage4.SettingInVisible = false;
		this.xtraTabPage4.SettingReadonly = false;
		this.xtraTabPage4.Size = new System.Drawing.Size(1088, 169);
		this.xtraTabPage4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPage4.Text = "Attachments";
		this.ucAttachment1.DataSource = null;
		this.ucAttachment1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.ucAttachment1.FormId = "oVirTransfer";
		this.ucAttachment1.ItemId = "ATC1";
		this.ucAttachment1.Location = new System.Drawing.Point(0, 0);
		this.ucAttachment1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.ucAttachment1.Name = "ucAttachment1";
		this.ucAttachment1.Size = new System.Drawing.Size(1088, 169);
		this.ucAttachment1.TabIndex = 0;
		this.ucBatchAndSerial1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		this.ucBatchAndSerial1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.ucBatchAndSerial1.FieldQuantity = "InvQty";
		this.ucBatchAndSerial1.FieldToTranferWhsCode = "WhsCode";
		this.ucBatchAndSerial1.FieldWhsCode = "FromWhsCod";
		this.ucBatchAndSerial1.FormId = "oVirTransfer";
		this.ucBatchAndSerial1.GvHeader = null;
		this.ucBatchAndSerial1.GvLineItem = this.gv_Lines;
		this.ucBatchAndSerial1.IsAllowcateBinLocationToWhs = false;
		this.ucBatchAndSerial1.ItemId = "OBTN";
		this.ucBatchAndSerial1.ItemIdBin = "oBinAllowcation";
		this.ucBatchAndSerial1.Location = new System.Drawing.Point(0, 0);
		this.ucBatchAndSerial1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.ucBatchAndSerial1.Name = "ucBatchAndSerial1";
		this.ucBatchAndSerial1.Size = new System.Drawing.Size(1090, 93);
		this.ucBatchAndSerial1.TabIndex = 0;
		this.xtraTabPage2.EditControl = null;
		this.xtraTabPage2.EditValue = "";
		this.xtraTabPage2.FieldName = null;
		this.xtraTabPage2.IsSearchControl = false;
		this.xtraTabPage2.Name = "xtraTabPage2";
		this.xtraTabPage2.ReadOnly = false;
		this.xtraTabPage2.SettingInVisible = false;
		this.xtraTabPage2.SettingReadonly = false;
		this.xtraTabPage2.Size = new System.Drawing.Size(1077, 143);
		this.xtraTabPage2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.txtDocNum.EditControl = null;
		this.txtDocNum.FieldName = null;
		this.txtDocNum.IsSearchControl = true;
		this.txtDocNum.Location = new System.Drawing.Point(299, 4);
		this.txtDocNum.Name = "txtDocNum";
		this.txtDocNum.SettingInVisible = false;
		this.txtDocNum.SettingReadonly = false;
		this.txtDocNum.Size = new System.Drawing.Size(120, 20);
		this.txtDocNum.TabIndex = 44;
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Card Code";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(427, 31);
		this.labelLine1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(80, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 34;
		this.labelLine1.Utext = "Card Code";
		this.lb_FromDate.EditControl = null;
		this.lb_FromDate.EditValue = "From Date";
		this.lb_FromDate.FieldName = null;
		this.lb_FromDate.HideWhenEmpty = false;
		this.lb_FromDate.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_FromDate.Location = new System.Drawing.Point(9, 7);
		this.lb_FromDate.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_FromDate.Name = "lb_FromDate";
		this.lb_FromDate.ReadOnly = false;
		this.lb_FromDate.SettingInVisible = false;
		this.lb_FromDate.SettingReadonly = false;
		this.lb_FromDate.ShowUnderLine = true;
		this.lb_FromDate.Size = new System.Drawing.Size(80, 20);
		this.lb_FromDate.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_FromDate.TabIndex = 35;
		this.lb_FromDate.Utext = "From Date";
		this.btnSearch.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btnSearch.Appearance.Options.UseBackColor = true;
		this.btnSearch.EditControl = null;
		this.btnSearch.EditValue = "Search";
		this.btnSearch.FieldName = null;
		this.btnSearch.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnSearch.ImageOptions.Image");
		this.btnSearch.IsSearchControl = true;
		this.btnSearch.Location = new System.Drawing.Point(634, 29);
		this.btnSearch.Name = "btnSearch";
		this.btnSearch.ReadOnly = false;
		this.btnSearch.SettingInVisible = false;
		this.btnSearch.SettingReadonly = false;
		this.btnSearch.Size = new System.Drawing.Size(80, 20);
		this.btnSearch.TabIndex = 41;
		this.btnSearch.Text = "Search";
		this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
		this.lb_ToDate.EditControl = null;
		this.lb_ToDate.EditValue = "To Date";
		this.lb_ToDate.FieldName = null;
		this.lb_ToDate.HideWhenEmpty = false;
		this.lb_ToDate.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_ToDate.Location = new System.Drawing.Point(9, 31);
		this.lb_ToDate.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_ToDate.Name = "lb_ToDate";
		this.lb_ToDate.ReadOnly = false;
		this.lb_ToDate.SettingInVisible = false;
		this.lb_ToDate.SettingReadonly = false;
		this.lb_ToDate.ShowUnderLine = true;
		this.lb_ToDate.Size = new System.Drawing.Size(80, 20);
		this.lb_ToDate.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_ToDate.TabIndex = 36;
		this.lb_ToDate.Utext = "To Date";
		this.lb_DocStatus.EditControl = null;
		this.lb_DocStatus.EditValue = "Doc. Status";
		this.lb_DocStatus.FieldName = null;
		this.lb_DocStatus.HideWhenEmpty = false;
		this.lb_DocStatus.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_DocStatus.Location = new System.Drawing.Point(427, 7);
		this.lb_DocStatus.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_DocStatus.Name = "lb_DocStatus";
		this.lb_DocStatus.ReadOnly = false;
		this.lb_DocStatus.SettingInVisible = false;
		this.lb_DocStatus.SettingReadonly = false;
		this.lb_DocStatus.ShowUnderLine = true;
		this.lb_DocStatus.Size = new System.Drawing.Size(80, 20);
		this.lb_DocStatus.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_DocStatus.TabIndex = 37;
		this.lb_DocStatus.Utext = "Doc. Status";
		this.lb_Number.EditControl = null;
		this.lb_Number.EditValue = "Doc. Number";
		this.lb_Number.FieldName = null;
		this.lb_Number.HideWhenEmpty = false;
		this.lb_Number.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Number.Location = new System.Drawing.Point(219, 7);
		this.lb_Number.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_Number.Name = "lb_Number";
		this.lb_Number.ReadOnly = false;
		this.lb_Number.SettingInVisible = false;
		this.lb_Number.SettingReadonly = false;
		this.lb_Number.ShowUnderLine = true;
		this.lb_Number.Size = new System.Drawing.Size(80, 20);
		this.lb_Number.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Number.TabIndex = 38;
		this.lb_Number.Utext = "Doc. Number";
		this.lb_PriceList.EditControl = null;
		this.lb_PriceList.EditValue = "Price List";
		this.lb_PriceList.FieldName = null;
		this.lb_PriceList.HideWhenEmpty = false;
		this.lb_PriceList.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_PriceList.Location = new System.Drawing.Point(219, 31);
		this.lb_PriceList.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_PriceList.Name = "lb_PriceList";
		this.lb_PriceList.ReadOnly = false;
		this.lb_PriceList.SettingInVisible = false;
		this.lb_PriceList.SettingReadonly = false;
		this.lb_PriceList.ShowUnderLine = true;
		this.lb_PriceList.Size = new System.Drawing.Size(80, 20);
		this.lb_PriceList.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_PriceList.TabIndex = 39;
		this.lb_PriceList.Utext = "Price List";
		this.txtPriceList.EditControl = null;
		this.txtPriceList.FieldName = null;
		this.txtPriceList.IsSearchControl = true;
		this.txtPriceList.Location = new System.Drawing.Point(299, 29);
		this.txtPriceList.Name = "txtPriceList";
		this.txtPriceList.Properties.AppearanceFocused.BackColor = System.Drawing.Color.BlanchedAlmond;
		this.txtPriceList.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.txtPriceList.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtPriceList.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name3"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Name4", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.txtPriceList.Properties.DisplayMember = "Name";
		this.txtPriceList.Properties.NullText = "";
		this.txtPriceList.Properties.PopupSizeable = false;
		this.txtPriceList.Properties.ShowFooter = false;
		this.txtPriceList.Properties.ShowHeader = false;
		this.txtPriceList.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
		this.txtPriceList.Properties.ValueMember = "Code";
		this.txtPriceList.SettingInVisible = false;
		this.txtPriceList.SettingReadonly = false;
		this.txtPriceList.Size = new System.Drawing.Size(120, 20);
		this.txtPriceList.TabIndex = 40;
		this.txtCardCode.EditControl = null;
		this.txtCardCode.FieldName = null;
		this.txtCardCode.IsSearchControl = true;
		this.txtCardCode.Location = new System.Drawing.Point(507, 29);
		this.txtCardCode.Name = "txtCardCode";
		this.txtCardCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.txtCardCode.SettingInVisible = false;
		this.txtCardCode.SettingReadonly = false;
		this.txtCardCode.Size = new System.Drawing.Size(120, 20);
		this.txtCardCode.TabIndex = 42;
		this.txtCardCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtCardCode_ButtonClick);
		this.txtDocStatus.EditControl = null;
		this.txtDocStatus.FieldName = null;
		this.txtDocStatus.IsSearchControl = true;
		this.txtDocStatus.Location = new System.Drawing.Point(507, 4);
		this.txtDocStatus.Name = "txtDocStatus";
		this.txtDocStatus.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtDocStatus.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name1"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Name2", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.txtDocStatus.Properties.DisplayMember = "Name";
		this.txtDocStatus.Properties.NullText = "";
		this.txtDocStatus.Properties.ShowFooter = false;
		this.txtDocStatus.Properties.ShowHeader = false;
		this.txtDocStatus.Properties.ValueMember = "Code";
		this.txtDocStatus.SettingInVisible = false;
		this.txtDocStatus.SettingReadonly = false;
		this.txtDocStatus.Size = new System.Drawing.Size(120, 20);
		this.txtDocStatus.TabIndex = 43;
		this.txtFromDate.EditControl = null;
		this.txtFromDate.EditValue = null;
		this.txtFromDate.FieldName = null;
		this.txtFromDate.Location = new System.Drawing.Point(89, 5);
		this.txtFromDate.Name = "txtFromDate";
		this.txtFromDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtFromDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtFromDate.Properties.IsSearchControl = true;
		this.txtFromDate.Properties.Mask.EditMask = "";
		this.txtFromDate.SettingInVisible = false;
		this.txtFromDate.SettingReadonly = false;
		this.txtFromDate.Size = new System.Drawing.Size(120, 20);
		this.txtFromDate.TabIndex = 45;
		this.txtToDate.EditControl = null;
		this.txtToDate.EditValue = null;
		this.txtToDate.FieldName = null;
		this.txtToDate.Location = new System.Drawing.Point(89, 29);
		this.txtToDate.Name = "txtToDate";
		this.txtToDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtToDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtToDate.Properties.IsSearchControl = true;
		this.txtToDate.Properties.Mask.EditMask = "";
		this.txtToDate.SettingInVisible = false;
		this.txtToDate.SettingReadonly = false;
		this.txtToDate.Size = new System.Drawing.Size(120, 20);
		this.txtToDate.TabIndex = 46;
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Cancel.Appearance.Options.UseBackColor = true;
		this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = true;
		this.btn_Cancel.Location = new System.Drawing.Point(132, 510);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(99, 20);
		this.btn_Cancel.TabIndex = 48;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.btnCopyFrom.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnCopyFrom.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btnCopyFrom.Appearance.Options.UseBackColor = true;
		this.btnCopyFrom.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCopyFrom.ImageOptions.Image");
		this.btnCopyFrom.Location = new System.Drawing.Point(983, 510);
		this.btnCopyFrom.Name = "btnCopyFrom";
		this.btnCopyFrom.Size = new System.Drawing.Size(99, 20);
		this.btnCopyFrom.TabIndex = 49;
		this.btnCopyFrom.Text = "Copy From";
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Save.Appearance.Options.UseBackColor = true;
		this.btn_Save.EditControl = null;
		this.btn_Save.EditValue = "Add && Search";
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.ItemId = null;
		this.btn_Save.Location = new System.Drawing.Point(6, 510);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.ReadOnly = false;
		this.btn_Save.SettingInVisible = false;
		this.btn_Save.SettingReadonly = false;
		this.btn_Save.Size = new System.Drawing.Size(120, 20);
		this.btn_Save.TabIndex = 50;
		this.btn_Save.Text = "Add && Search";
		this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btn_Cancel;
		base.ClientSize = new System.Drawing.Size(1090, 537);
		base.Controls.Add(this.btn_Save);
		base.Controls.Add(this.btnCopyFrom);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.txtDocNum);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.lb_FromDate);
		base.Controls.Add(this.btnSearch);
		base.Controls.Add(this.lb_ToDate);
		base.Controls.Add(this.lb_DocStatus);
		base.Controls.Add(this.lb_Number);
		base.Controls.Add(this.lb_PriceList);
		base.Controls.Add(this.txtPriceList);
		base.Controls.Add(this.txtCardCode);
		base.Controls.Add(this.txtDocStatus);
		base.Controls.Add(this.txtFromDate);
		base.Controls.Add(this.txtToDate);
		base.Controls.Add(this.splitContainerControl1);
		base.FormId = "oVirTransfer";
		base.IconOptions.ShowIcon = false;
		base.Name = "VirFormInventoryTranfer";
		this.Text = "Inventory Transfer";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.VirFormInventoryTranfer_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rLineUoMGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gridSplitContainer2).EndInit();
		this.gridSplitContainer2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		this.contextMenuStrip_Document.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rDocSeries).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rDocCustomerCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rDocContactPersion).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rDocStatus).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rDate.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rDate).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rDocCal).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rDocCur).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rDocBranch).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rDocFromWhsDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rDocPriceList).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rDocSalesEmployee).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rBranch).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerLine).EndInit();
		this.splitContainerLine.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).EndInit();
		this.xtraTabControl1.ResumeLayout(false);
		this.xtraTabPage1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gridSplitContainer1).EndInit();
		this.gridSplitContainer1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Lines).EndInit();
		this.contextMenuStrip1_DocLine.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Lines).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rItemCodeLine).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rLineBarCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rLineWareHouse).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rLineCal).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_LineProject).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rLineUseBaseUn).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rLineDistrRule).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rLineUoMCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rPriceEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rLineCurrency).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rLineStatus).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rLineTaxCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rLineSpecPrice).EndInit();
		this.xtraTabPage3.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Logistic).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Logistic).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rlpShipType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rlpShipTo).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rlpBillTo).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rLogicticsConfirm).EndInit();
		this.xtraTabPage4.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txtDocNum.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtPriceList.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtCardCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtDocStatus.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtFromDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtFromDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtToDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtToDate.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
