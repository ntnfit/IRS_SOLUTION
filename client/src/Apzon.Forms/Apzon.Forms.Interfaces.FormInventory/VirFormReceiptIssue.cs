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
using Apzon.Commons.Constants;
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
using Apzon.Entities.Models.MasterData;
using Apzon.Entities.Models.Production;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Forms.Properties;
using Apzon.Forms.SalesOppotunity;
using Apzon.Forms.UserControls;
using Apzon.Forms.UserControls.BatchAndSerial;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTab;

namespace Apzon.Forms.Interfaces.FormInventory;

public class VirFormReceiptIssue : XtraFormEx
{
	protected delegate void ShowForms(XtraFormEx frm);

	private DataTable _originalDocTable = new DataTable();

	private readonly string[] _arrListColumnsReadOnly = new string[12]
	{
		"CodeBars", "DiscPrcnt", "LineTotal", "ItemCode", "Dscription", "ItemType", "UomCode", "IsByPrdct", "LineNum", "NumPerMsr",
		"UseBaseUn", "Price"
	};

	private string _sourceActionField = "";

	private DataTable _dtDim = new DataTable();

	private Enums.SaveAction _saveAction = Enums.SaveAction.SaveAndSearch;

	public ApzObjectTypes FromType;

	public int FromEntry;

	public XtraFormEx FormCopy;

	public int BaseType = -1;

	protected int DocEntryProductionOrder;

	public int DocEntryIssueComponents;

	protected DataTable TableSearch = new DataTable();

	protected int _hotTrackRow = int.MinValue;

	public Enums.ChangeLog ChangeLogType;

	protected Dictionary<string, GridViewEx> TblFormular = new Dictionary<string, GridViewEx>();

	protected new bool IsLoadForm;

	public Enums.FormViewModeType OpenFormViewMode = Enums.FormViewModeType.View;

	private bool _isSearching;

	private readonly Image _btnSaveImg;

	private static readonly string[] _lstFieldLineNotEditable = new string[26]
	{
		"LineNum", "BaseType", "BaseLine", "BaseEntry", "TargetType", "TrgetEntry", "OpenQty", "OpenInvQty", "LineStatus", "UomEntry",
		"InvntSttus", "QuantityOnWareHouse", "ManBtchSerial", "ManBatchSerial", "UgpCode", "QuantityOnStock", "IsCommited", "OnOrder", "IsCommitedInWareHouse", "IsByPrdct",
		"OnOrderInWareHouse", "BOMTreeType", "TreeType", "AvgPrice", "CmpltQty", "IsReturnComponent"
	};

	private bool changeItemFromBaseRef;

	private bool _loadingDetail;

	private static readonly string[] _lstDocFieldNotEditable = new string[19]
	{
		"DocEntry", "DocStatus", "DocStatusName", "UserSign", "UserSign2", "CreateDate", "CreateTS", "UpdateDate", "UpdateTS", "TransId",
		"InvntSttus", "WddStatus", "DocTotal", "DocTotalFC", "IsDraft", "draftKey", "AtcEntry", "OpenForLaC", "DataSource"
	};

	private static readonly string[] _lstEditableAfterAdd = new string[3] { "Comments", "JrnlMemo", "Ref2" };

	private bool _saving;

	private IHttpClient _httpClient;

	private DataTable _dtSeries;

	private IContainer components;

	private TextEditEx txtNo;

	private LabelLine lblPri;

	private LabelLine lblNum;

	private DateEditEx txttDate;

	private LabelLine lbltdate;

	private DateEditEx txtfDate;

	private LabelLine lblfdate;

	private SplitContainerControl splitContainerControl1;

	private GridControlEx grd_Doc;

	private XtraTabControlEx xtraTabControl1;

	private XtraTabPageEx xtraTabPage1;

	private ContextMenuStrip contextMenuStrip_Document;

	private ToolStripMenuItem docAddNewToolStripMenuItem;

	private ContextMenuStrip contextMenuStrip_Line;

	private ToolStripMenuItem lineDeleteToolStripMenuItem;

	private RepositoryItemDateEdit rDocDateEdit;

	private RepositoryItemCalcEdit rDocCal;

	private ToolStripMenuItem docCmtDuplicate;

	private BarButtonItem barButtonItem1;

	private BarButtonItem barButtonItem2;

	private BarManager barManager1;

	private BarDockControl barDockControlTop;

	private BarDockControl barDockControlBottom;

	private BarDockControl barDockControlLeft;

	private BarDockControl barDockControlRight;

	private ToolStripMenuItem LineCmtDuplicate;

	private XtraTabPageEx xtraTabPage2;

	private RepositoryItemButtonEdit rDocRequester;

	private RepositoryItemGridLookUpEdit rDocGLU_Department;

	private GridColumnEx gridColumn7;

	private GridColumnEx gridColumn10;

	private GridColumnEx gridColumn11;

	private RepositoryItemLookUpEdit rDocBranch;

	public RepositoryItemLookUpEdit rDocCurrency;

	private SimpleButtonEx btn_Cancel;

	private SimpleButtonEx btnSearch;

	protected GridViewEx gv_Doc;

	private GridViewEx gridView2;

	private ToolStripMenuItem ctm_NewTask;

	private RepositoryItemLookUpEdit rDocGroupNum;

	private RepositoryItemLookUpEdit rDocSeries;

	private RepositoryItemLookUpEdit rDocContactPerson;

	private ToolStripSeparator toolStripSeparator1;

	private SplitContainerControl splitContainerControl2;

	private GridControlEx grd_Lines;

	protected GridViewEx gv_Lines;

	private RepositoryItemButtonEdit rLineBaseRef;

	private RepositoryItemButtonEdit rLineItemCode;

	private RepositoryItemDateEdit rLineDateEdit;

	private RepositoryItemButtonEdit rLineTaxCode;

	private RepositoryItemButtonEdit rLineWhs;

	private RepositoryItemLookUpEdit rLineCurrency;

	private RepositoryItemButtonEdit rLineAcctCode;

	private RepositoryItemLookUpEdit rLineUomGroup;

	private RepositoryItemLookUpEdit rLineInventoryUom;

	private RepositoryItemCalcEdit rLineCal;

	private RepositoryItemButtonEdit rLineUomCode;

	private ToolStripMenuItem docRefreshToolStripMenuItem;

	protected GridColumnEx gcLineBaseRef;

	protected GridColumnEx gc_Line_ItemCode;

	protected GridColumnEx gcLineItemDescription;

	protected GridColumnEx gcLineQuantity;

	protected GridColumnEx gc_LinePrice;

	protected GridColumnEx gc_LinePQTReqdate;

	protected GridColumnEx gc_LineDiscountPercent;

	protected GridColumnEx gc_LineVatGroup;

	protected GridColumnEx gc_LineTotal;

	protected GridColumnEx gcLineWarehouseCode;

	protected GridColumnEx gcLineFreeTxt;

	protected GridColumnEx gcLineOpenQty;

	protected GridColumnEx gcLineCurrency;

	protected GridColumnEx gcLineRate;

	protected GridColumnEx gcLineDocEntry;

	protected GridColumnEx gcLineNum;

	protected GridColumnEx gcLineSumLineTotalRate;

	protected GridColumnEx gcLineBaseType;

	protected GridColumnEx gcLineBaseEntry;

	protected GridColumnEx gcLineBaseLine;

	protected GridColumnEx gcLineAcctCode;

	protected GridColumnEx gcLineStatus;

	protected GridColumnEx ShipDate;

	protected GridColumnEx gcLineUomGroup;

	protected GridColumnEx gcLineUomName;

	protected GridColumnEx gcLineInventoryUom;

	protected GridColumnEx gcLineItemPerUnit;

	protected GridColumnEx gcLineUomCode;

	protected GridColumnEx gcLineIvnvQty;

	protected GridColumnEx gcLineOpenInvQty;

	protected GridColumnEx gcLineQuantityOnStock;

	protected GridColumnEx gc_Line_QuantityOnWareHouse;

	protected GridColumnEx gcDocNum;

	protected GridColumnEx gcDocCardCode;

	protected GridColumnEx gcDocDocumentStatus;

	protected GridColumnEx gcDocTaxDate;

	protected GridColumnEx gcDocDocDueDate;

	protected GridColumnEx gcDocDate;

	protected GridColumnEx gcDocComments;

	protected GridColumnEx gcDocTotalBeforeDis;

	protected GridColumnEx gcDocDiscPrcnt;

	protected GridColumnEx gcDocVatSum;

	protected GridColumnEx gcDocDocTotal;

	protected GridColumnEx gcDocCurrency;

	protected GridColumnEx gcDocDocRate;

	protected GridColumnEx gcDocNumAtCard;

	protected GridColumnEx gcDocSeries;

	protected GridColumnEx gcDocCANCELED;

	protected GridColumnEx gcDocContactPersonCode;

	protected GridColumnEx gcDocRounding;

	protected GridColumnEx gcDocRequiredDate;

	protected GridColumnEx gcDocRequester;

	protected GridColumnEx gcDocBranch;

	protected GridColumnEx gcDocDepartment;

	protected GridColumnEx gcDocGroupNum;

	protected GridColumnEx gcDocDocRef2;

	protected GridColumnEx gcDocEntryDoc;

	protected GridColumnEx gcDoc_JournalMemo;

	private XtraTabPageEx tab_Attachments;

	private GridColumnEx gc_LineProject;

	private RepositoryItemButtonEditCFL rep_LineProject;

	protected ucBatchAndSerial ucBatchAndSerial1;

	private ToolStripMenuItem batchSerialNumberToolStripMenuItem;

	private ToolStripMenuItem ctm_NewEvent;

	private ToolStripMenuItem ctm_NewNote;

	protected ToolStripMenuItem docSaveAsDraftToolStripMenuItem;

	private RepositoryItemTextEditExPrice rPriceEdit;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx1;

	private GridColumnEx gcTranType;

	private RepositoryItemLookUpEdit rTranType;

	private LookUpEditEx txtPriceList;

	private GridColumnEx gcLineAvgPrice;

	private GridColumnEx gcLineSpecPrice;

	private RepositoryItemLookUpEditSimple rLineSpecPrice;

	private GridColumnEx gcLinePrice;

	private RepositoryItemTextEditExPrice repositoryItemTextEditExPrice2;

	private RepositoryItemTextEditExPrice repositoryItemTextEditExPrice1;

	private DropDownButtonEx btn_Save;

	public DropDownButtonEx drlCopyTo;

	public DropDownButtonEx drlCopyFrom;

	protected ucAttachment ucAttachment1;

	private RepositoryItemLookUpEdit rDocStatus;

	private GridColumnEx gcBaseRef;

	protected SimpleButtonEx btn_RetCom;

	public virtual bool Cancellation { get; set; }

	protected Enums.TypeCFL TypeProductionOrder { get; set; }

	protected int HotTrackRow
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
			catch (Exception ex)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
				WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			}
		}
	}

	protected ApzObjectTypes ObjectType { get; set; }

	protected string FormController { get; set; }

	public DataTable OpenFormDocument { get; set; }

	public DataTable OpenFormLines { get; set; }

	public DataTable OpenFormBatchSerial { get; set; }

	public ContextActionMappingImpl[] MappingDocument { get; set; }

	public ContextActionMappingImpl[] MappingLines { get; set; }

	public ContextActionMappingImpl[] MappingBatchSerial { get; set; }

	public List<ApzObjectTypes> PopCopyFrom { get; set; }

	public List<ApzObjectTypes> PopCopyTo { get; set; }

	public string FormLayoutCode { get; set; }

	public VirFormReceiptIssue()
	{
		InitializeComponent();
		gv_Lines.LstColumNotSetting = new List<string> { "IsReturnComponent" };
		_btnSaveImg = btn_Save.ImageOptions.Image;
		OnLoadAsync += VirFormReceiptIssue_Load;
		HttpClient = HttpClientHelperFactory.GetHttpClient();
	}

	public VirFormReceiptIssue(int baseType = -1)
	{
		InitializeComponent();
		gv_Lines.LstColumNotSetting = new List<string> { "IsReturnComponent" };
		OnLoadAsync += VirFormReceiptIssue_Load;
		HttpClient = HttpClientHelperFactory.GetHttpClient();
		BaseType = baseType;
	}

	public VirFormReceiptIssue(int productionNum = 0, int baseType = -1)
	{
		BaseType = baseType;
		DocEntryProductionOrder = productionNum;
		InitializeComponent();
		gv_Lines.LstColumNotSetting = new List<string> { "IsReturnComponent" };
		OnLoadAsync += VirFormReceiptIssue_Load;
		HttpClient = HttpClientHelperFactory.GetHttpClient();
	}

	public VirFormReceiptIssue(ApzObjectTypes fromType, int docEntry)
	{
		FromType = fromType;
		FromEntry = docEntry;
		InitializeComponent();
		gv_Lines.LstColumNotSetting = new List<string> { "IsReturnComponent" };
		OnLoadAsync += VirFormReceiptIssue_Load;
		HttpClient = HttpClientHelperFactory.GetHttpClient();
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		try
		{
			Close();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task VirFormReceiptIssue_Load(object sender, EventArgs e)
	{
		_ = 7;
		try
		{
			_httpClient = HttpClientHelperFactory.GetHttpClient(ObjectType);
			if (BaseType == 202)
			{
				rLineItemCode.Buttons.RemoveAt(0);
				rLineItemCode.TextEditStyle = TextEditStyles.DisableTextEditor;
			}
			else
			{
				rLineBaseRef.Buttons.Clear();
			}
			EnableDisableControls(isEnable: false);
			LoadTableFormular();
			ucBatchAndSerial ucBatchAndSerial2 = ucBatchAndSerial1;
			ucBatchAndSerial2.DBatchSerialChanged = (DataBatchSerialChanged)Delegate.Combine(ucBatchAndSerial2.DBatchSerialChanged, new DataBatchSerialChanged(BatchSerialDataChanged));
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
			txtfDate.EditValue = DateTime.Today.AddDays(-num);
			txttDate.EditValue = DateTime.Today.AddDays(num);
			if ((FromType != ApzObjectTypes.oNone && FromType != ObjectType) || DocEntryProductionOrder != 0 || DocEntryIssueComponents != 0 || OpenFormViewMode == Enums.FormViewModeType.Add)
			{
				FormMode = Enums.FormViewModeType.Add;
			}
			else
			{
				FormMode = Enums.FormViewModeType.View;
			}
			gv_Doc.ShowLoadingPanel();
			await Task.WhenAll(InitCustomize(), LoadComb());
			LoadCopyMenu();
			splitContainerControl2.PanelVisibility = SplitPanelVisibility.Panel1;
			if (BaseType != -1)
			{
				WinFuntions.SetReadOnlyListColumnGrid(gv_Lines, _arrListColumnsReadOnly.ToList());
			}
			ucAttachment ucAttachment2 = ucAttachment1;
			ucAttachment2.BrowsedFile = (Action<List<DataRow>>)Delegate.Combine(ucAttachment2.BrowsedFile, new Action<List<DataRow>>(Attachment_BrowseFile));
			ucAttachment ucAttachment3 = ucAttachment1;
			ucAttachment3.DeletedFile = (Action)Delegate.Combine(ucAttachment3.DeletedFile, new Action(Attachment_DeleteFile));
			ucBatchAndSerial1.DataSourceBatchAndSerial = DocumentsHelper.BatchSerialNumbersStruct.Copy();
			ucBatchAndSerial1.DataSourceBinAllowLine = DocumentsHelper.BatchSerialNumbersStruct.Copy();
			if (FromType != ApzObjectTypes.oNone && FromType != ObjectType)
			{
				DataTable dataSource;
				using (HttpClientHelper<DataTable> docTableHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController))
				{
					HttpClientHelper<DataTable> httpClientHelper = docTableHelper;
					int fromType = (int)FromType;
					dataSource = await httpClientHelper.GetAsync("CopyFrom", new
					{
						type = fromType.ToString(),
						entry = FromEntry,
						bplId = GlobalData.DefaultBranch,
						username = GlobalData.UserName
					});
				}
				IsLoadForm = true;
				grd_Doc.DataSource = dataSource;
				grd_Doc.RefreshDataSource();
				LoadSeriesByPostingDate();
				IsLoadForm = false;
				if (FromType == ApzObjectTypes.oDrafts)
				{
					ucAttachment1.LoadAttachments(Function.ParseInt(gv_Doc.GetFocusedRowCellValue("AtcEntry")));
				}
				DataTable dataTable2;
				using (HttpClientHelper<DataTable> docTableHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController))
				{
					HttpClientHelper<DataTable> httpClientHelper2 = docTableHelper;
					string fromentry = Function.ToString(FromEntry);
					int fromType = (int)FromType;
					dataTable2 = await httpClientHelper2.GetAsync("CopyBatchSerial", new
					{
						fromentry = fromentry,
						fromtype = fromType.ToString()
					});
				}
				if (dataTable2.IsNotNull())
				{
					DataTable dataTable3 = DocumentsHelper.BatchSerialNumbersStruct.Copy();
					string[] columnNames = (from DataColumn t in dataTable3.Columns
						select t.ColumnName).ToArray();
					dataTable2 = dataTable2.DefaultView.ToTable("Source", distinct: false, columnNames);
					DataRow dataRow = dataTable3.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["DistNumber"]).Equals(""));
					if (dataRow != null)
					{
						dataTable3.Rows.Remove(dataRow);
						dataTable3.AcceptChanges();
					}
					foreach (DataRow row in dataTable2.Rows)
					{
						DataRow dataRow3 = dataTable3.NewRow();
						dataRow3.ItemArray = row.ItemArray;
						dataTable3.Rows.Add(dataRow3);
					}
					ucBatchAndSerial1.DataSourceBatchAndSerial = dataTable3;
				}
				DataTable dataSource2;
				using (HttpClientHelper<DataTable> docTableHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController))
				{
					HttpClientHelper<DataTable> httpClientHelper3 = docTableHelper;
					int fromType = (int)FromType;
					dataSource2 = await httpClientHelper3.GetAsync("CopyFromLine", new
					{
						type = fromType.ToString(),
						entry = FromEntry
					});
				}
				grd_Lines.DataSource = dataSource2;
				grd_Lines.RefreshDataSource();
				AddNewBlankRowLine();
				if (FromType != ApzObjectTypes.oDrafts && gv_Lines.RowCount > 0)
				{
					gv_Lines.SetRowCellValue(0, "Quantity", gv_Lines.GetRowCellValue(0, "Quantity"));
				}
				if (FormCopy == null)
				{
					Text = $"{Text} - Draft";
				}
				if (FromType == ApzObjectTypes.oDrafts)
				{
					_onlyChangeRemark = true;
				}
				FromType = ApzObjectTypes.oNone;
				FromEntry = 0;
				return;
			}
			rDocCurrency.DataSource = GlobalData.Currencies;
			if (DocEntryProductionOrder != 0 || DocEntryIssueComponents != 0)
			{
				await AddNewRowDoc();
				await BindDataLines();
				return;
			}
			switch (OpenFormViewMode)
			{
			case Enums.FormViewModeType.Add:
				IsLoadForm = true;
				await AddNewRowDoc();
				AddNewBlankRowLine();
				IsLoadForm = false;
				if (OpenFormDocument.IsNotNull() && OpenFormLines.IsNotNull())
				{
					gv_Doc.MappingSource(OpenFormDocument, MappingDocument, null, null, null, multirow: false);
					gv_Doc.HideLoadingPanel();
					gv_Lines.ShowLoadingPanel();
					gv_Lines.MappingSource(OpenFormLines, MappingLines, OpenFormBatchSerial, MappingBatchSerial, ucBatchAndSerial1);
					gv_Lines.HideLoadingPanel();
					ucBatchAndSerial1.SetVisibleAfterContextAction();
				}
				break;
			case Enums.FormViewModeType.View:
				await RefreshData();
				FromEntry = 0;
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
			EnableDisableControls(isEnable: true);
			gv_Doc.HideLoadingPanel();
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

	public override void CustomFormSetting()
	{
		EnableDisableGridLine(FormMode == Enums.FormViewModeType.Add);
	}

	private void CreateColumnsDistRule(GridViewEx gv)
	{
		try
		{
			if (GlobalData.Dimensions == null || GlobalData.Dimensions.Rows.Count <= 0)
			{
				return;
			}
			_dtDim = GlobalData.Dimensions;
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

	private void BatchSerialDataChanged()
	{
		if (FormMode == Enums.FormViewModeType.View || FormMode == Enums.FormViewModeType.Find)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
		_onlyChangeRemark = false;
	}

	protected virtual void LoadCopyMenu()
	{
		try
		{
			if (PopCopyFrom != null && PopCopyFrom.Count > 0)
			{
				DXPopupMenu dXPopupMenu = new DXPopupMenu();
				foreach (ApzObjectTypes item in PopCopyFrom)
				{
					DXMenuItem dXMenuItem = new DXMenuItem();
					switch (item)
					{
					case ApzObjectTypes.oInventoryGenEntry:
						dXMenuItem.Caption = MenuCaption.GoodsReceipt;
						break;
					case ApzObjectTypes.oInventoryGenExit:
						dXMenuItem.Caption = MenuCaption.GoodsIssue;
						break;
					}
					dXMenuItem.Click += async (object s, EventArgs e) =>
					{
						WinCommons._OpenProcessing();
						try
						{
							using frm_CFL frm = new frm_CFL(Enums.TypeCFL.GoodsIssueOpen, true, "", "", Enums.ItemObjectType.NoItemType, null, "");
							frm.ShowDialog(this);
							if (frm.ChooseItems != null && frm.ChooseItems.Count != 0)
							{
								gv_Lines.ShowLoadingPanel();
								string text = string.Join(",", frm.ChooseItems.Select((DataRow dataRow5) => Function.ToString(dataRow5["DocEntry"])));
								IHttpClient httpClient = HttpClientHelperFactory.GetHttpClient(ApzObjectTypes.oInventoryGenExit);
								DataTable dataTable = httpClient.Documents.CopyBatchSerial(text, ApzObjectTypes.oInventoryGenExit);
								if (dataTable.IsNotNull())
								{
									DataTable dataTable2 = DocumentsHelper.BatchSerialNumbersStruct.Copy();
									string[] columnNames = (from DataColumn t in dataTable2.Columns
										select t.ColumnName).ToArray();
									dataTable = dataTable.DefaultView.ToTable("Source", distinct: false, columnNames);
									DataRow dataRow = dataTable2.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["DistNumber"]).Equals(""));
									if (dataRow != null)
									{
										dataTable2.Rows.Remove(dataRow);
										dataTable2.AcceptChanges();
									}
									foreach (DataRow row in dataTable.Rows)
									{
										DataRow dataRow3 = dataTable2.NewRow();
										dataRow3.ItemArray = row.ItemArray;
										dataTable2.Rows.Add(dataRow3);
									}
									dataTable2.Rows.Add(dataTable2.NewRow());
									ucBatchAndSerial1.DataSourceBatchAndSerial = dataTable2;
								}
								else
								{
									ucBatchAndSerial1.DataSourceBatchAndSerial = DocumentsHelper.BatchSerialNumbersStruct.Copy();
								}
								DataTable dtBin = httpClient.BinLocMasterDataClient.GetBatchBinAllowcateLineByDocEntry(text, ApzObjectTypes.oInventoryGenExit);
								DocumentLineTable documentLineTable = await httpClient.Documents.Copy(ApzObjectTypes.oInventoryGenExit, ApzObjectTypes.oInventoryGenEntry, text);
								if (dtBin.IsNotNull())
								{
									foreach (DataRow r in dtBin.Rows)
									{
										DataRow dataRow4 = documentLineTable.AsEnumerable().FirstOrDefault((DataRow n) => Function.ParseInt(n["BaseEntry"]) == Function.ParseInt(r["BaseDocEntry"]) && Function.ParseInt(n["BaseLine"]) == Function.ParseInt(r["BaseLineNumber"]));
										if (dataRow4 != null)
										{
											r["BaseLineNumber"] = dataRow4["LineNum"];
										}
									}
								}
								ucBatchAndSerial1.DataSourceBinAllowLine = (dtBin.IsNotNull() ? dtBin : DocumentsHelper.BatchSerialNumbersStruct.Copy());
								grd_Lines.DataSource = documentLineTable;
								grd_Lines.RefreshDataSource();
								AddNewBlankRowLine();
								if (frm.ChooseItems.Count == 1)
								{
									DataTable dataTable3 = frm.ChooseItems.CopyToDataTable();
									dataTable3.Rows[0]["DocNum"] = gv_Doc.GetFocusedRowCellValue("DocNum");
									dataTable3.Rows[0]["Series"] = gv_Doc.GetFocusedRowCellValue("Series");
									dataTable3.Rows[0]["DocEntry"] = DBNull.Value;
									dataTable3.Rows[0]["DocStatus"] = "O";
									dataTable3.Rows[0]["ObjType"] = (int)ObjectType;
									dataTable3.Rows[0]["Instance"] = 0;
									dataTable3.Rows[0]["Segment"] = 0;
									dataTable3.Rows[0]["PIndicator"] = "Default";
									dataTable3.Rows[0]["DocSubType"] = "--";
									dataTable3.Rows[0]["DocDate"] = DateTime.Today;
									dataTable3.Rows[0]["DocDueDate"] = DateTime.Today;
									dataTable3.Rows[0]["TaxDate"] = DateTime.Today;
									dataTable3.Rows[0]["DocCur"] = GlobalData.MainCurrency;
									dataTable3.Rows[0]["ReqDate"] = DateTime.Today;
									dataTable3.Rows[0]["GroupNum"] = -1;
									dataTable3.Rows[0]["JrnlMemo"] = Text;
									dataTable3.Rows[0]["DocRate"] = 1;
									dataTable3.Rows[0]["DocCur"] = GlobalData.MainCurrency;
									dataTable3.Rows[0]["Ref2"] = string.Empty;
									grd_Doc.DataSource = dataTable3;
									grd_Doc.RefreshDataSource();
								}
							}
						}
						catch (Exception ex2)
						{
							Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.ToString());
							WinCommons.ErrorLineNumber(ex2);
						}
						finally
						{
							gv_Lines.HideLoadingPanel();
							WinCommons._CloseProcessing();
						}
					};
					dXPopupMenu.Items.Add(dXMenuItem);
				}
				drlCopyFrom.DropDownControl = dXPopupMenu;
			}
			if (PopCopyTo == null || PopCopyTo.Count <= 0)
			{
				return;
			}
			DXPopupMenu dXPopupMenu2 = new DXPopupMenu();
			foreach (ApzObjectTypes item2 in PopCopyTo)
			{
				DXMenuItem dXMenuItem2 = new DXMenuItem();
				switch (item2)
				{
				case ApzObjectTypes.oInventoryGenEntry:
					dXMenuItem2.Caption = MenuCaption.GoodsReceipt;
					break;
				case ApzObjectTypes.oInventoryGenExit:
					dXMenuItem2.Caption = MenuCaption.GoodsIssue;
					break;
				}
				dXMenuItem2.Click += (object s, EventArgs e) =>
				{
					try
					{
						WinFuntions.CopyToDocument(ApzObjectTypes.oInventoryGenExit, ApzObjectTypes.oInventoryGenEntry, this, gv_Doc);
					}
					catch (Exception ex2)
					{
						Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
						WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex2.Message);
					}
				};
				dXPopupMenu2.Items.Add(dXMenuItem2);
			}
			drlCopyTo.DropDownControl = dXPopupMenu2;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void rep_bte_LineWhs_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				string whs = "";
				if (gv_Lines.ActiveEditor != null)
				{
					whs = Function.ToString(gv_Lines.ActiveEditor.EditValue);
				}
				await SetWhs(whs);
			}
			else if (Function.ToString(gv_Lines.GetFocusedRowCellValue(gcLineWarehouseCode)).Trim() != "")
			{
				this.DrillDown(ApzObjectTypes.oWarehouses, Function.ToString(gv_Lines.GetFocusedRowCellValue(gcLineWarehouseCode)).Trim());
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ribAcctCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			ChooseFromListItemAcctCode();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Lines_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Add)
			{
				e.Cancel = true;
				return;
			}
			string fieldName = gv_Lines.FocusedColumn.FieldName;
			if (_lstFieldLineNotEditable.Contains(fieldName))
			{
				e.Cancel = true;
			}
			else if (fieldName == "LineTotal" && BaseType == 202)
			{
				e.Cancel = true;
			}
			else if (fieldName == "PriceBefDi" && BaseType == 202 && ((ObjectType == ApzObjectTypes.oInventoryGenEntry && Function.ToString(gv_Lines.GetFocusedRowCellValue("IsByPrdct")) != "Y") || (ObjectType == ApzObjectTypes.oInventoryGenExit && Function.ParseInt(gv_Lines.GetFocusedRowCellValue("BaseLine")) >= 0)))
			{
				e.Cancel = true;
			}
			else if (fieldName == "BaseRef" && BaseType != 202)
			{
				e.Cancel = true;
			}
			else if (fieldName == gcLineInventoryUom.FieldName)
			{
				double num = 0.0;
				num = ((ObjectType != ApzObjectTypes.oInventoryGenEntry) ? Function.ParseDouble(Function.ParseDouble(gv_Lines.GetFocusedRowCellValue("NumInSale"))) : Function.ParseDouble(Function.ParseDouble(gv_Lines.GetFocusedRowCellValue("NumInBuy"))));
				if (Function.ParseInt(gv_Lines.GetFocusedRowCellValue(gcLineUomGroup)) == -1 && Math.Abs(num - 1.0) < 1E-07)
				{
					e.Cancel = true;
				}
			}
			else if (fieldName == gcLineUomCode.FieldName)
			{
				if (Function.ToString(gv_Lines.GetFocusedRowCellValue(gcLineInventoryUom)).Equals("Y") || Function.ParseInt(gv_Lines.GetFocusedRowCellValue(gcLineUomGroup)) == -1)
				{
					e.Cancel = true;
				}
			}
			else if (fieldName == gcLineUomName.FieldName)
			{
				if (!Function.ToString(gv_Lines.GetFocusedRowCellValue(gcLineInventoryUom)).Equals("N") || Function.ParseInt(gv_Lines.GetFocusedRowCellValue(gcLineUomGroup)) != -1)
				{
					e.Cancel = true;
				}
			}
			else if (gv_Lines.FocusedColumn.FieldName == gcLineItemPerUnit.FieldName && (Function.ParseInt(gv_Lines.GetFocusedRowCellValue(gcLineUomGroup)) > 0 || Function.ParseInt(gv_Lines.GetFocusedRowCellValue("TargetType")) > 0 || Function.ToString(gv_Lines.GetFocusedRowCellValue(gcLineInventoryUom)).Equals("Y")))
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

	private void rLineUomCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.ListUomCode, false, "", "", Enums.ItemObjectType.NoItemType, null, "")
			{
				UomGroup = Function.ParseInt(gv_Lines.GetFocusedRowCellValue(gcLineUomGroup))
			};
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem != null)
			{
				gv_Lines.SetFocusedRowCellValue(gcLineUomCode, frm_CFL2.ChooseItem["UomCode"]);
				grd_Lines.DataSourceTable.AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void rep_BaseRef_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				await ShowChooseFromListProductionOrder();
			}
			else if (Function.ToString(gv_Lines.GetFocusedRowCellValue(gcLineBaseRef)).Trim() != "")
			{
				this.DrillDown(ApzObjectTypes.oProductionOrders, ApzObjectTypes.oProductionOrders, Function.ToString(gv_Lines.GetFocusedRowCellValue(gcLineBaseEntry)));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void rep_BaseRef_KeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.Tab)
			{
				await ShowChooseFromListProductionOrder();
			}
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
			if (e.RowHandle == HotTrackRow)
			{
				e.Appearance.BackColor = ApzonCommon.colorFocusRow;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Lines_RowStyle(object sender, RowStyleEventArgs e)
	{
		try
		{
			if (Function.ToString(gv_Lines.GetRowCellValue(e.RowHandle, gcLineStatus)).Equals("C"))
			{
				e.HighPriority = true;
				e.Appearance.BackColor = StyleColor.GridDisable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void cmtDuplicate_Line_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View || FormMode == Enums.FormViewModeType.Update)
			{
				await DuplicateDoc();
			}
			else
			{
				DuplicateRowLine();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void cmtDuplicate_Return_Click(object sender, EventArgs e)
	{
		await DuplicateDoc();
	}

	private void batchSerialNumbersToolStripMenuItem_Click(object sender, EventArgs e)
	{
		splitContainerControl2.PanelVisibility = ((splitContainerControl2.PanelVisibility != SplitPanelVisibility.Panel1) ? SplitPanelVisibility.Panel1 : SplitPanelVisibility.Both);
	}

	private async void btn_Save_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCloseForm, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
				{
					Close();
				}
			}
			else if (FormAuth != Enums.FormAuthorities.Full)
			{
				WinCommons.Error(Enums.MessageCode.YouDoNotPermisionModifiedForm, "");
			}
			else
			{
				await Save(isdraft: false);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void refreshToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			EnableDisableControls(isEnable: false);
			FormMode = Enums.FormViewModeType.View;
			await RefreshData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			EnableDisableControls(isEnable: true);
			WinCommons._CloseProcessing();
		}
	}

	private void rep_TaxCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.VatOut, multiSelect: false, string.Empty);
			frm_ChooseFromList2.ShowDialog(this);
			if (frm_ChooseFromList2.ChooseItem != null)
			{
				VatGroupsImpl vatGroupsImpl = (VatGroupsImpl)frm_ChooseFromList2.ChooseItem;
				gv_Lines.SetFocusedRowCellValue(gc_LineVatGroup, vatGroupsImpl.Code);
				gv_Lines.SetFocusedRowCellValue(gcLineRate, vatGroupsImpl.Rate);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rep_ItemCode_Line_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			string text = Function.ToString(gv_Lines.GetFocusedRowCellValue(gc_Line_ItemCode));
			if (!e.Button.IsLeft)
			{
				if (gv_Lines.ActiveEditor != null)
				{
					text = Function.ToString(gv_Lines.ActiveEditor.EditValue);
				}
				gv_Lines.RefreshEditor(updateEditorValue: true);
				ChooseFormListItemCode(text);
			}
			else if (!string.IsNullOrWhiteSpace(text))
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

	private void rpRequester_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			ChooseFromListRequester(string.Empty);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void deleteToolStripMenuItem_DocLine_Click(object sender, EventArgs e)
	{
		try
		{
			ucBatchAndSerial1.DeleteBatchAndSerialByLineNum();
			ucBatchAndSerial1.BeginUpdateLine();
			gv_Lines.DeleteDataSourceRowSelected();
			ucBatchAndSerial1.EndUpdateLine();
			AddNewBlankRowLine();
			ucBatchAndSerial1.SetFocusLine();
			_onlyChangeRemark = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btSearch_Click(object sender, EventArgs e)
	{
		try
		{
			if ((FormMode != Enums.FormViewModeType.Add && FormMode != Enums.FormViewModeType.Update) || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
			{
				EnableDisableControls(isEnable: false);
				FormMode = Enums.FormViewModeType.View;
				grd_Doc.DataSourceTable.Rows.Clear();
				grd_Lines.DataSourceTable.Rows.Clear();
				await SearchData(Enums.NavigateType.Current);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			EnableDisableControls(isEnable: true);
		}
	}

	private async void addNewToolStripMenuItem_Document_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormAuth == Enums.FormAuthorities.Full)
			{
				WinCommons._OpenProcessing();
				EnableDisableControls(isEnable: false);
				FormMode = Enums.FormViewModeType.Add;
				await AddNewRowDoc();
				AddNewBlankRowLine();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			WinCommons._CloseProcessing();
			EnableDisableControls(isEnable: true);
		}
	}

	private async void gv_Lines_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		_ = 1;
		try
		{
			_onlyChangeRemark = false;
			DataTable lst = grd_Lines.DataSourceTable;
			int indexRowHanle = gv_Lines.GetDataSourceRowIndex(e.RowHandle);
			if (e.Column.FieldName.Equals("BaseRef") && !gv_Lines.IsMappingContextAction)
			{
				int docNum = Function.ParseInt(gv_Lines.GetRowCellValue(e.RowHandle, "BaseRef"));
				string type;
				if (TypeProductionOrder == Enums.TypeCFL.ReceiptProductionOrder)
				{
					type = "R";
				}
				else
				{
					type = ((TypeProductionOrder == Enums.TypeCFL.IssueProductionOrder) ? "I" : "D");
				}
				DataTable dataTable = await HttpClient.ProductionOrders.CheckByDocNum(type, docNum);
				if (dataTable.Rows.Count == 1)
				{
					int baseEntry = Function.ParseInt(dataTable.Rows[0]["DocEntry"]);
					SetBaseEntryToLine(baseEntry, gv_Lines.FocusedRowHandle);
				}
				else
				{
					gv_Lines.FocusedColumn = gcLineBaseRef;
					await ShowChooseFromListProductionOrder();
				}
			}
			if (e.Column == gc_Line_ItemCode)
			{
				string itemCode = Function.ToString(gv_Lines.GetRowCellValue(e.RowHandle, "ItemCode"));
				DataTable dataItemCode = GetDataItemCode(itemCode, Function.ParseInt(gv_Doc.GetFocusedRowCellValue(gcDocGroupNum)));
				lst.Rows[indexRowHanle]["DocEntry"] = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("DocEntry"));
				lst.Rows[indexRowHanle]["LineNum"] = GetNewLineNum();
				lst.Rows[indexRowHanle]["PQTReqdate"] = DateTime.Today;
				lst.Rows[indexRowHanle]["ShipDate"] = DateTime.Today;
				lst.Rows[indexRowHanle]["BaseType"] = BaseType;
				if (!changeItemFromBaseRef)
				{
					lst.Rows[indexRowHanle]["BaseEntry"] = 0;
				}
				lst.Rows[indexRowHanle]["Quantity"] = 1;
				lst.Rows[indexRowHanle]["Currency"] = GlobalData.MainCurrency;
				lst.Rows[indexRowHanle]["LineStatus"] = "O";
				lst.Rows[indexRowHanle]["AcctCode"] = "";
				lst.Rows[indexRowHanle]["DiscPrcnt"] = 0;
				lst.Rows[indexRowHanle]["UomCode"] = string.Empty;
				if (dataItemCode.Rows.Count > 0)
				{
					lst.Rows[indexRowHanle]["ItemCode"] = dataItemCode.Rows[0]["ItemCode"];
					string text = Function.ToString(dataItemCode.Rows[0]["BaseCurrency"]);
					lst.Rows[indexRowHanle]["Currency"] = (string.IsNullOrEmpty(text) ? GlobalData.MainCurrency : text);
					double num = Function.ParseDouble(dataItemCode.Rows[0]["Rate"]);
					lst.Rows[indexRowHanle]["Rate"] = ((num < 1E-06) ? 1.0 : num);
					lst.Rows[indexRowHanle]["WhsCode"] = dataItemCode.Rows[0]["DfltWH"];
					lst.Rows[indexRowHanle]["AvgPrice"] = dataItemCode.Rows[0]["AvgPrice"];
					lst.Rows[indexRowHanle]["QuantityOnStock"] = dataItemCode.Rows[0]["OnHand"];
					lst.Rows[indexRowHanle]["QuantityOnWareHouse"] = dataItemCode.Rows[0]["InWhs"];
					lst.Rows[indexRowHanle]["ManBtchSerial"] = dataItemCode.Rows[0]["ManBtchSerial"];
					lst.Rows[indexRowHanle]["Dscription"] = dataItemCode.Rows[0]["ItemName"];
					lst.Rows[indexRowHanle]["SalUnitMsr"] = dataItemCode.Rows[0]["SalUnitMsr"];
					lst.Rows[indexRowHanle]["NumInSale"] = dataItemCode.Rows[0]["NumInSale"];
					lst.Rows[indexRowHanle]["BuyUnitMsr"] = Function.ToString(dataItemCode.Rows[0]["BuyUnitMsr"]);
					lst.Rows[indexRowHanle]["NumInBuy"] = Function.ParseDouble(dataItemCode.Rows[0]["NumInBuy"]);
					lst.Rows[indexRowHanle]["NumPerMsr"] = 1;
					lst.Rows[indexRowHanle]["UgpCode"] = dataItemCode.Rows[0]["UgpEntry"];
					lst.Rows[indexRowHanle]["UseBaseUn"] = "Y";
					gv_Lines.SetRowCellValue(e.RowHandle, "unitMsr", dataItemCode.Rows[0]["InvntryUom"]);
					lst.Rows[indexRowHanle]["InvUnitMsr"] = dataItemCode.Rows[0]["InvntryUom"];
					lst.Rows[indexRowHanle]["NumInInven"] = dataItemCode.Rows[0]["NumInInven"];
					lst.Rows[indexRowHanle][gcLineIvnvQty.FieldName] = Function.ParseDouble(gv_Lines.GetRowCellValue(e.RowHandle, gcLineQuantity)) * Function.ParseDouble(gv_Lines.GetRowCellValue(e.RowHandle, gcLineItemPerUnit));
					lst.Rows[indexRowHanle][gcLineOpenInvQty.FieldName] = Function.ParseDouble(gv_Lines.GetRowCellValue(e.RowHandle, gcLineQuantity)) * Function.ParseDouble(gv_Lines.GetRowCellValue(e.RowHandle, gcLineItemPerUnit));
					if (Function.ParseInt(dataItemCode.Rows[0]["UgpEntry"]) == -1)
					{
						lst.Rows[indexRowHanle]["UomCode"] = gv_Lines.GetRowCellDisplayText(e.RowHandle, "UgpCode");
					}
					else
					{
						lst.Rows[indexRowHanle]["UomCode"] = WinFuntions.GetUoMCodeByName(Function.ParseInt(dataItemCode.Rows[0]["UgpEntry"]), Function.ToString(dataItemCode.Rows[0]["InvntryUom"]));
					}
					if (!changeItemFromBaseRef)
					{
						gv_Lines.SetRowCellValue(e.RowHandle, gc_LinePrice, Function.ParseDouble(dataItemCode.Rows[0]["BasePrice"]));
					}
				}
				else
				{
					lst.Rows[indexRowHanle]["Rate"] = 1;
					lst.Rows[indexRowHanle]["Currency"] = GlobalData.MainCurrency;
					lst.Rows[indexRowHanle]["ManBtchSerial"] = "N";
					lst.Rows[indexRowHanle]["Dscription"] = "";
					gv_Lines.SetRowCellValue(e.RowHandle, gc_LinePrice, 0);
					lst.Rows[indexRowHanle]["Price"] = 0;
				}
				grd_Lines.DataSourceTable.AcceptChanges();
				ucBatchAndSerial1.SetFocusLine(isChangeItem: true);
				gv_Lines.FocusedColumn = gcLineWarehouseCode;
				gv_Lines.ShowEditor();
				AddNewBlankRowLine();
			}
			else if (e.Column.FieldName == gcLineUomCode.FieldName)
			{
				DataTable dataUomByCode = WinFuntions.GetDataUomByCode(Function.ParseInt(gv_Lines.GetRowCellValue(e.RowHandle, gcLineUomGroup)), Function.ToString(gv_Lines.GetRowCellValue(e.RowHandle, gcLineUomCode)));
				if (dataUomByCode != null && dataUomByCode.Rows.Count > 0)
				{
					double num2 = Function.ParseDouble(lst.Rows[indexRowHanle][gcLineItemPerUnit.FieldName]);
					lst.Rows[indexRowHanle]["UomEntry"] = dataUomByCode.Rows[0]["UomEntry"];
					lst.Rows[indexRowHanle][gcLineUomName.FieldName] = dataUomByCode.Rows[0]["UomName"];
					lst.Rows[indexRowHanle][gcLineItemPerUnit.FieldName] = Function.ParseDouble(gv_Lines.GetRowCellValue(e.RowHandle, "NumInInven")) * Function.ParseDouble(dataUomByCode.Rows[0]["BaseQty"]) / ((Function.ParseDouble(dataUomByCode.Rows[0]["AltQty"]) <= 0.0) ? 1.0 : Function.ParseDouble(dataUomByCode.Rows[0]["AltQty"]));
					gv_Lines.SetRowCellValue(e.RowHandle, gc_LinePrice, Function.ParseDouble(lst.Rows[indexRowHanle][gc_LinePrice.FieldName]) * Function.ParseDouble(lst.Rows[indexRowHanle][gcLineItemPerUnit.FieldName]) / num2);
					if (_sourceActionField != gcLineInventoryUom.FieldName)
					{
						if (Function.ToString(dataUomByCode.Rows[0]["UomName"]).Equals(Function.ToString(gv_Lines.GetRowCellValue(e.RowHandle, "InvUnitMsr"))))
						{
							lst.Rows[indexRowHanle][gcLineInventoryUom.FieldName] = "Y";
							gv_Lines.FocusedColumn = gv_Lines.Columns[gcLineInventoryUom.FieldName];
						}
						else
						{
							lst.Rows[indexRowHanle][gcLineInventoryUom.FieldName] = "N";
						}
					}
					_sourceActionField = "uomcode";
					gv_Lines.SetRowCellValue(e.RowHandle, gcLineIvnvQty.FieldName, Function.ParseDouble(gv_Lines.GetRowCellValue(e.RowHandle, gcLineQuantity)) * Function.ParseDouble(gv_Lines.GetRowCellValue(e.RowHandle, gcLineItemPerUnit)));
					gv_Lines.SetRowCellValue(e.RowHandle, gcLineOpenInvQty.FieldName, Function.ParseDouble(gv_Lines.GetRowCellValue(e.RowHandle, gcLineQuantity)) * Function.ParseDouble(gv_Lines.GetRowCellValue(e.RowHandle, gcLineItemPerUnit)));
					grd_Lines.DataSourceTable.AcceptChanges();
				}
			}
			if (e.Column.FieldName.Equals("NumPerMsr"))
			{
				double num3 = Function.ParseDouble(e.Value);
				gv_Lines.SetRowCellValue(e.RowHandle, gc_LinePrice, num3 * Function.ParseDouble(lst.Rows[indexRowHanle][gc_LinePrice.FieldName]) * Function.ParseDouble(gv_Lines.GetRowCellValue(e.RowHandle, gcLineOpenQty)) / Function.ParseDouble(gv_Lines.GetRowCellValue(e.RowHandle, gcLineOpenInvQty)));
				gv_Lines.SetRowCellValue(e.RowHandle, gcLineIvnvQty.FieldName, Function.ParseDouble(gv_Lines.GetRowCellValue(e.RowHandle, gcLineQuantity)) * num3);
				gv_Lines.SetRowCellValue(e.RowHandle, gcLineOpenInvQty.FieldName, Function.ParseDouble(gv_Lines.GetRowCellValue(e.RowHandle, gcLineOpenQty)) * num3);
			}
			if (e.Column.FieldName.Equals(gcLineInventoryUom.FieldName))
			{
				_sourceActionField = e.Column.FieldName;
				if (Function.ToString(e.Value).Equals("Y"))
				{
					DataTable inventoryUoMByItemCode = WinFuntions.GetInventoryUoMByItemCode(Function.ToString(gv_Lines.GetRowCellValue(e.RowHandle, gc_Line_ItemCode)), Enums.ItemUomType.InventoryUom);
					if (inventoryUoMByItemCode != null && inventoryUoMByItemCode.Rows.Count > 0)
					{
						gv_Lines.SetRowCellValue(e.RowHandle, gcLineUomCode, inventoryUoMByItemCode.Rows[0]["UomCode"]);
						gv_Lines.SetRowCellValue(e.RowHandle, gcLineItemPerUnit, 1);
						gv_Lines.SetRowCellValue(e.RowHandle, gcLineUomName, inventoryUoMByItemCode.Rows[0]["InvntryUom"]);
					}
				}
				else
				{
					gv_Lines.SetRowCellValue(e.RowHandle, gcLineUomCode, (Function.ParseInt(gv_Lines.GetRowCellValue(e.RowHandle, gcLineUomGroup)) != -1) ? gv_Lines.GetRowCellValue(e.RowHandle, (ObjectType == ApzObjectTypes.oInventoryGenExit) ? "SalUnitMsr" : "BuyUnitMsr") : Function.ToString(gv_Lines.GetRowCellDisplayText(e.RowHandle, gcLineUomGroup)));
					gv_Lines.SetRowCellValue(e.RowHandle, gcLineItemPerUnit, gv_Lines.GetRowCellValue(e.RowHandle, (ObjectType == ApzObjectTypes.oInventoryGenExit) ? "NumInSale" : "NumInBuy"));
					gv_Lines.SetRowCellValue(e.RowHandle, gcLineUomName, gv_Lines.GetRowCellValue(e.RowHandle, (ObjectType == ApzObjectTypes.oInventoryGenExit) ? "SalUnitMsr" : "BuyUnitMsr"));
				}
				gv_Lines.SetRowCellValue(e.RowHandle, gcLineIvnvQty, Function.ParseDouble(gv_Lines.GetRowCellValue(e.RowHandle, gcLineQuantity)) * Function.ParseDouble(gv_Lines.GetRowCellValue(e.RowHandle, gcLineItemPerUnit)));
				gv_Lines.SetRowCellValue(e.RowHandle, gcLineOpenInvQty, Function.ParseDouble(gv_Lines.GetRowCellValue(e.RowHandle, gcLineQuantity)) * Function.ParseDouble(gv_Lines.GetRowCellValue(e.RowHandle, gcLineItemPerUnit)));
			}
			else if (e.Column.FieldName.Equals(gcLineCurrency.FieldName))
			{
				lst.Rows[indexRowHanle][gcLineRate.FieldName] = HttpClient.ExRateAndIndex.GetExchangeRate(Function.ToString(e.Value), DateTime.Now.Date);
				lst.AcceptChanges();
			}
			else if (e.Column == gc_LineTotal || e.Column == gcLineRate)
			{
				double num4 = Function.ParseDouble(gv_Lines.GetRowCellValue(e.RowHandle, gc_LineTotal)) * Function.ParseDouble(gv_Lines.GetRowCellValue(e.RowHandle, gcLineRate)) / 100.0;
				gv_Lines.SetRowCellValue(e.RowHandle, "SumLineTotalRate", num4);
				double num5 = 0.0;
				num5 += Function.ParseDouble(lst.Compute("sum(LineVat)", ""));
				gv_Doc.SetRowCellValue(e.RowHandle, "VatSum", num5);
				if (e.Column.FieldName.Equals(gc_LineTotal.FieldName))
				{
					double num6 = Function.ParseDouble(e.Value) / Function.ParseDouble(gv_Lines.GetRowCellValue(e.RowHandle, gcLineQuantity));
					double num7 = Function.ParseDouble(gv_Lines.GetRowCellValue(e.RowHandle, "DiscPrcnt")) / 100.0;
					double num8 = ((Math.Abs(num7 - 1.0) > 1E-07) ? (num6 / (1.0 - num7)) : 0.0);
					lst.Rows[indexRowHanle]["PriceBefDi"] = num8;
					lst.Rows[indexRowHanle]["Price"] = num6;
				}
				lst.AcceptChanges();
			}
			else if (e.Column.FieldName == "Quantity")
			{
				if (!_sourceActionField.ToLower().Equals("invqty"))
				{
					_sourceActionField = "quantity";
					gv_Lines.SetRowCellValue(e.RowHandle, gcLineIvnvQty.FieldName, Function.ParseDouble(gv_Lines.GetRowCellValue(e.RowHandle, gcLineQuantity)) * Function.ParseDouble(gv_Lines.GetRowCellValue(e.RowHandle, gcLineItemPerUnit)));
				}
				else
				{
					_sourceActionField = "";
				}
				gv_Lines.SetRowCellValue(e.RowHandle, gcLineOpenQty.FieldName, Function.ParseDouble(gv_Lines.GetRowCellValue(e.RowHandle, gcLineQuantity)));
				gv_Lines.SetRowCellValue(e.RowHandle, gcLineOpenInvQty.FieldName, Function.ParseDouble(gv_Lines.GetRowCellValue(e.RowHandle, gcLineOpenQty)) * Function.ParseDouble(gv_Lines.GetRowCellValue(e.RowHandle, gcLineItemPerUnit)));
				lst.AcceptChanges();
			}
			else if (e.Column.FieldName.Equals(gcLineIvnvQty.FieldName))
			{
				double num9 = Function.ParseDouble(e.Value) / Function.ParseDouble(gv_Lines.GetRowCellValue(e.RowHandle, gcLineItemPerUnit));
				if (!_sourceActionField.ToLower().Equals("quantity") && !_sourceActionField.ToLower().Equals("uomcode"))
				{
					_sourceActionField = "invqty";
					gv_Lines.SetRowCellValue(e.RowHandle, gcLineQuantity, num9);
				}
				else
				{
					_sourceActionField = "";
				}
			}
			else if (e.Column.FieldName == "WhsCode")
			{
				string text2 = Function.ToString(gv_Lines.GetRowCellValue(e.RowHandle, "ItemCode"));
				string text3 = Function.ToString(e.Value);
				if (string.IsNullOrEmpty(text3) || string.IsNullOrEmpty(text2))
				{
					lst.Rows[indexRowHanle][gc_Line_QuantityOnWareHouse.FieldName] = 0;
					lst.AcceptChanges();
				}
				else
				{
					DataTable itemInStockAllWarehouse = GetItemInStockAllWarehouse(text2, text3);
					if (itemInStockAllWarehouse.IsNotNull())
					{
						lst.Rows[indexRowHanle][gc_Line_QuantityOnWareHouse.FieldName] = itemInStockAllWarehouse.Rows[0]["InWhs"];
						lst.Rows[indexRowHanle][gcLineAvgPrice.FieldName] = itemInStockAllWarehouse.Rows[0]["AvgPrice"];
						lst.AcceptChanges();
					}
				}
			}
			if (e.Column == gcLineQuantity || e.Column == gc_LinePrice || e.Column == gc_LineDiscountPercent || e.Column.FieldName.Equals(gc_Line_ItemCode.FieldName) || e.Column.FieldName.Equals(gcLineCurrency.FieldName))
			{
				double num10 = Function.ParseDouble(gv_Lines.GetRowCellValue(e.RowHandle, gcLineQuantity));
				double num11 = Function.ParseDouble(gv_Lines.GetRowCellValue(e.RowHandle, gc_LinePrice));
				double num12 = Function.ParseDouble(gv_Lines.GetRowCellValue(e.RowHandle, "DiscPrcnt")) / 100.0;
				double num13 = num11 * (1.0 - num12);
				double num14 = num10 * num13;
				lst.Rows[indexRowHanle]["Price"] = num13;
				lst.Rows[indexRowHanle]["LineTotal"] = num14;
				double num15 = 0.0;
				double num16 = 0.0;
				for (int i = 0; i < lst.Rows.Count; i++)
				{
					num15 += Function.ParseDouble(lst.Rows[i]["Quantity"]) * Function.ParseDouble(lst.Rows[i]["Price"]) * (1.0 - Function.ParseDouble(lst.Rows[i]["DiscPrcnt"]) / 100.0) * Function.ParseDouble(lst.Rows[i]["Rate"]);
					num16 += Function.ParseDouble(lst.Rows[i]["Quantity"]) * Function.ParseDouble(lst.Rows[i]["Price"]) * Function.ParseDouble(lst.Rows[i]["Rate"]);
				}
				gv_Doc.SetFocusedRowCellValue("TotalBeforeDis", num15 / Function.ParseDouble(gv_Doc.GetFocusedRowCellValue("DocRate")));
				gv_Doc.SetFocusedRowCellValue("DocTotal", num16 / Function.ParseDouble(gv_Doc.GetFocusedRowCellValue("DocRate")));
				lst.AcceptChanges();
			}
			grd_Doc.DataSourceTable.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void gv_Doc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (_onlyChangeRemark && !new string[3] { "Comments", "JrnlMemo", "REFDOC" }.Contains(e.Column.FieldName))
			{
				_onlyChangeRemark = false;
			}
			if (e.Column == gcDocTotalBeforeDis || e.Column == gcDocDiscPrcnt || e.Column == gcDocVatSum)
			{
				double num = Function.ParseDouble(gv_Doc.GetRowCellValue(e.RowHandle, gcDocTotalBeforeDis));
				double num2 = Function.ParseDouble(gv_Doc.GetRowCellValue(e.RowHandle, gcDocVatSum));
				double num3 = Function.ParseDouble(gv_Doc.GetRowCellValue(e.RowHandle, gcDocDiscPrcnt));
				double num4 = num + num2 - num * num3;
				gv_Doc.SetRowCellValue(e.RowHandle, gcDocDocTotal, num4);
				grd_Doc.Refresh();
			}
			if (e.Column.FieldName.ToUpper().Equals("SERIES"))
			{
				int series = Function.ParseInt(gv_Doc.GetRowCellValue(e.RowHandle, "Series"));
				int num5 = await GetNewDocNum(series);
				if (num5 > 0)
				{
					gv_Doc.SetRowCellValue(e.RowHandle, "DocNum", num5);
				}
			}
			else if (e.Column == gcDocRequiredDate)
			{
				DateTime? dateTime = Function.ParseDateTime(gv_Doc.GetRowCellValue(e.RowHandle, gcDocRequiredDate));
				gv_Lines.SetRowCellValue(gv_Lines.FocusedRowHandle, gc_LinePQTReqdate, dateTime);
			}
			else if (e.Column == gcDocCardCode)
			{
				string cardCode = Function.ToString(gv_Doc.GetFocusedRowCellValue(gcDocCardCode));
				CheckBusinessPartner(cardCode);
				if (FormAuth == Enums.FormAuthorities.Full)
				{
					btn_Save.Enabled = true;
				}
			}
			else if (e.Column.FieldName.Equals(gcDocCurrency.FieldName))
			{
				grd_Doc.DataSourceTable.Rows[gv_Doc.GetFocusedDataSourceRowIndex()]["DocRate"] = HttpClient.ExRateAndIndex.GetExchangeRate(Function.ToString(e.Value), DateTime.Now.Date);
			}
			else if (e.Column.FieldName.Equals(gcDocGroupNum.FieldName))
			{
				DataTable dtItem = grd_Lines.DataSourceTable.DefaultView.ToTable(true, "ItemCode");
				DataTable priceForListItem = HttpClient.PriceListSetup.GetPriceForListItem(dtItem, Function.ParseInt(e.Value));
				dtItem = grd_Lines.DataSourceTable;
				double num6 = 0.0;
				double num7 = 0.0;
				if (priceForListItem.IsNotNull())
				{
					int i;
					for (i = 0; i < dtItem.Rows.Count; i++)
					{
						if (!string.IsNullOrEmpty(Function.ToString(dtItem.Rows[i][gc_Line_ItemCode.FieldName])))
						{
							dtItem.Rows[i]["DiscPrcnt"] = 0;
							DataRow dataRow = priceForListItem.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(dtItem.Rows[i][gc_Line_ItemCode.FieldName]).Equals(Function.ToString(t["ItemCode"])));
							if (dataRow != null)
							{
								DataRow dataRow2 = dtItem.Rows[i];
								object value = (dtItem.Rows[i][gc_LinePrice.FieldName] = dataRow["Price"]);
								dataRow2["Price"] = value;
								dtItem.Rows[i]["Currency"] = dataRow["Currency"];
								dtItem.Rows[i]["Rate"] = ((Function.ParseDouble(dataRow["Rate"]) > 1E-07) ? Function.ParseDouble(dataRow["Rate"]) : 1.0);
							}
							else
							{
								DataRow dataRow3 = dtItem.Rows[i];
								object value = (dtItem.Rows[i][gc_LinePrice.FieldName] = 0);
								dataRow3["Price"] = value;
								dtItem.Rows[i]["Currency"] = GlobalData.MainCurrency;
								dtItem.Rows[i]["Rate"] = 1;
							}
							double num8 = Function.ParseDouble(dtItem.Rows[i]["Price"]) * Function.ParseDouble(dtItem.Rows[i][gcLineQuantity.FieldName]);
							dtItem.Rows[i][gc_LineTotal.FieldName] = num8;
							num6 += num8 * Function.ParseDouble(dtItem.Rows[i]["Rate"]);
							num7 += num8 * Function.ParseDouble(dtItem.Rows[i]["Rate"]) * (1.0 - Function.ParseDouble(dtItem.Rows[i]["DiscPrcnt"]) / 100.0);
						}
					}
					gv_Doc.SetRowCellValue(e.RowHandle, gcDocDocTotal, num6 / Function.ParseDouble(gv_Doc.GetFocusedRowCellValue("DocRate")));
					gv_Doc.SetRowCellValue(e.RowHandle, gcDocTotalBeforeDis, num7 / Function.ParseDouble(gv_Doc.GetFocusedRowCellValue("DocRate")));
				}
			}
			else if (e.Column.FieldName.Equals(gcDocDate.FieldName))
			{
				if (Function.ParseDateTime(e.Value) == DateTime.MinValue)
				{
					WinCommons.Error(Enums.MessageCode.InvalidDate, "");
					return;
				}
				gv_Doc.SetFocusedRowCellValue(gcDocTaxDate.FieldName, e.Value);
				await LoadSeriesByPostingDate();
			}
			if (FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
			grd_Doc.DataSourceTable.AcceptChanges();
			grd_Lines.DataSourceTable.AcceptChanges();
			grd_Doc.RefreshDataSource();
			grd_Lines.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Doc_RowStyle(object sender, RowStyleEventArgs e)
	{
		try
		{
			if (Function.ToString(gv_Doc.GetRowCellValue(e.RowHandle, "DocStatus")).Equals("C"))
			{
				e.HighPriority = true;
				e.Appearance.BackColor = StyleColor.GridDisable;
			}
			else if (e.RowHandle == gv_Doc.FocusedRowHandle)
			{
				e.Appearance.BackColor = StyleColor.GridEditting;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void gv_Doc_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			_loadingDetail = true;
			if (!_isSearching && !IsLoadForm && FormMode != Enums.FormViewModeType.Add)
			{
				drlCopyTo.Enabled = ObjectType == ApzObjectTypes.oInventoryGenExit && (FormMode == Enums.FormViewModeType.View || FormMode == Enums.FormViewModeType.Update) && Function.ToString(gv_Doc.GetFocusedRowCellValue("DocStatus")) == "O";
				EnableDisableControls(isEnable: false);
				FormMode = Enums.FormViewModeType.View;
				_onlyChangeRemark = true;
				await BindDataLines();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			EnableDisableControls(isEnable: true);
			_loadingDetail = false;
		}
	}

	private void rGroupNum_EditValueChanged(object sender, EventArgs e)
	{
	}

	private void repGLU_Department_Popup(object sender, EventArgs e)
	{
		try
		{
			string text = Function.ToString(gv_Doc.GetRowCellValue(gv_Doc.FocusedRowHandle, gcDocRequester));
			if (sender is GridLookUpEdit gridLookUpEdit)
			{
				gridLookUpEdit.Properties.View.ActiveFilterString = "[Requester] like '" + text + "'";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void contextMenuStrip_Line_Opening(object sender, CancelEventArgs e)
	{
		try
		{
			string text = Function.ToString(gv_Lines.GetFocusedRowCellValue("ManBtchSerial"));
			batchSerialNumberToolStripMenuItem.Enabled = text.Equals("S") || text.Equals("B");
			batchSerialNumberToolStripMenuItem.Text = ((splitContainerControl2.PanelVisibility == SplitPanelVisibility.Both) ? "Hide Batch/Serial Numbers" : "Batch/Serial Numbers");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void newActivityToolStripMenuItem_Click(object sender, EventArgs e)
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
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void grid_EditorKeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (!(sender is GridControlEx gridControlEx))
			{
				return;
			}
			GridViewEx gridViewEx = gridControlEx.FocusedView as GridViewEx;
			if (e.KeyData == Keys.Tab)
			{
				grd_Lines.DataSourceTable.AcceptChanges();
				if (gridViewEx != null && gridViewEx.Name.Equals(gv_Lines.Name) && gridViewEx.FocusedColumn.FieldName.ToUpper() == "ITEMCODE")
				{
					string text = Function.ToString(gv_Lines.EditingValue);
					if (string.IsNullOrEmpty(text))
					{
						gv_Lines.RefreshEditor(updateEditorValue: true);
						ChooseFormListItemCode(text);
					}
				}
			}
			else if (e.KeyCode == Keys.V && e.Modifiers == Keys.Control)
			{
				List<string> list = new List<string>();
				if (gridControlEx.Name.Equals(grd_Lines.Name))
				{
					list.Add(gc_Line_ItemCode.FieldName);
				}
				ExtenseionMethods.PasteFromExcelToDataTable(gridControlEx, gridViewEx, list);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void saveAsDraftToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			await Save(isdraft: true);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Lines_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			if (!IsLoadForm)
			{
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

	private void gv_Lines_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (e.RowHandle == gv_Lines.FocusedRowHandle)
			{
				e.Appearance.BackColor = Color.Beige;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Doc_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (_lstDocFieldNotEditable.Contains(gv_Doc.FocusedColumn.FieldName))
			{
				e.Cancel = true;
			}
			else if (FormMode != Enums.FormViewModeType.Add && !_lstEditableAfterAdd.Contains(gv_Doc.FocusedColumn.FieldName) && !gv_Doc.FocusedColumn.FieldName.StartsWith("U_"))
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

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		try
		{
			if (FormAuth != Enums.FormAuthorities.Full)
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
			if (btn_RetCom.Visible)
			{
				btn_RetCom.Enabled = value == Enums.FormViewModeType.Add;
			}
			SetFormViewMode(value);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task MOVE_FIRST()
	{
		try
		{
			EnableDisableControls(isEnable: false);
			FormMode = Enums.FormViewModeType.View;
			await SearchData(Enums.NavigateType.First);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			EnableDisableControls(isEnable: true);
		}
	}

	public async Task MOVE_NEXT()
	{
		try
		{
			EnableDisableControls(isEnable: false);
			FormMode = Enums.FormViewModeType.View;
			await SearchData(Enums.NavigateType.Next);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			EnableDisableControls(isEnable: true);
		}
	}

	public async Task MOVE_LAST()
	{
		try
		{
			EnableDisableControls(isEnable: false);
			FormMode = Enums.FormViewModeType.View;
			await SearchData(Enums.NavigateType.Last);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			EnableDisableControls(isEnable: true);
		}
	}

	public async Task MOVE_PREVIOUS()
	{
		try
		{
			EnableDisableControls(isEnable: false);
			FormMode = Enums.FormViewModeType.View;
			await SearchData(Enums.NavigateType.Previous);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			EnableDisableControls(isEnable: true);
		}
	}

	public async Task MODE_FIND()
	{
		WinCommons._OpenProcessing();
		EnableDisableControls(isEnable: false);
		FormMode = Enums.FormViewModeType.View;
		try
		{
			await RefreshData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			EnableDisableControls(isEnable: true);
			WinCommons._CloseProcessing();
		}
	}

	public async Task REFRESH()
	{
		try
		{
			WinCommons._OpenProcessing();
			EnableDisableControls(isEnable: false);
			FormMode = Enums.FormViewModeType.Add;
			await AddNewRowDoc();
			AddNewBlankRowLine();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			EnableDisableControls(isEnable: true);
			WinCommons._CloseProcessing();
		}
	}

	private async Task RefreshData()
	{
		await LoadComb();
		grd_Lines.RefreshDataSource();
		txtNo.EditValue = null;
		await SearchData(Enums.NavigateType.Current);
	}

	public async void PRINTLAYOUT()
	{
		try
		{
			string cardCode = Function.ToString(gv_Doc.GetFocusedRowCellValue("CardCode"));
			int num = Function.ParseInt((!Function.ParseBool(gv_Doc.GetFocusedRowCellValue("IsDraft"))) ? gv_Doc.GetFocusedRowCellValue("DocEntry") : gv_Doc.GetFocusedRowCellValue("DraftKey"));
			await WinFuntions.PrintLayout(cardCode, num, FormLayoutCode, ((int)ObjectType).ToString());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async void PREVIEWLAYOUTS()
	{
		try
		{
			string cardCode = Function.ToString(gv_Doc.GetFocusedRowCellValue("CardCode"));
			bool flag = Function.ParseBool(gv_Doc.GetFocusedRowCellValue("IsDraft"));
			ApzObjectTypes apzObjectTypes = ((!flag) ? ObjectType : ApzObjectTypes.oDrafts);
			int num = Function.ParseInt((!flag) ? gv_Doc.GetFocusedRowCellValue("DocEntry") : gv_Doc.GetFocusedRowCellValue("DraftKey"));
			object docEntry = num;
			string formLayoutCode = FormLayoutCode;
			VirFormReceiptIssue form = this;
			int num2 = (int)apzObjectTypes;
			await WinFuntions.PreviewLayout(cardCode, docEntry, formLayoutCode, form, num2.ToString());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
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

	private async Task<DataTable> GetByBaseEntry(int baseEntry)
	{
		try
		{
			return await HttpClient.ProductionOrders.GetByBaseEntry(baseEntry, ObjectType);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new DataTable();
	}

	private DataTable GetItemInStockAllWarehouse(string itemCode, string warehouse)
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons");
			return httpClientHelper.Get("GetItemOnwh", new
			{
				item = itemCode,
				wh = warehouse
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return null;
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
		WinCommons._OpenProcessing();
		EnableDisableControls(isEnable: false);
		try
		{
			await RefreshData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			FormMode = Enums.FormViewModeType.View;
			EnableDisableControls(isEnable: true);
			WinCommons._CloseProcessing();
		}
	}

	private async Task LoadComb()
	{
		try
		{
			await Task.WhenAll(BindDataToContactPerson(string.Empty), GetSeries(), GetDepartment(), GetBranch(), GetGroupNum(), LoadUomGroup(), GetLineCurrency());
			rLineSpecPrice.DataSource = WinFuntions.GetDataPriceSource();
			LoadInventoryUoM();
			List<Common.Table_Code> dataSource = new List<Common.Table_Code>
			{
				new Common.Table_Code
				{
					Code = "C",
					Name = "Complete"
				},
				new Common.Table_Code
				{
					Code = "R",
					Name = "Reject"
				},
				new Common.Table_Code
				{
					Name = ""
				}
			};
			rTranType.DataSource = dataSource;
			List<Common.Table_Code> dataSource2 = new List<Common.Table_Code>
			{
				new Common.Table_Code
				{
					Code = "O",
					Name = "Open"
				},
				new Common.Table_Code
				{
					Code = "C",
					Name = "Closed"
				},
				new Common.Table_Code
				{
					Name = ""
				}
			};
			rDocStatus.DataSource = dataSource2;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task GetGroupNum()
	{
		try
		{
			DataTable dataSource = await HttpClient.Commons.GetPriceList();
			rDocGroupNum.DataSource = dataSource;
			txtPriceList.Properties.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task GetBranch()
	{
		try
		{
			RepositoryItemLookUpEdit repositoryItemLookUpEdit = rDocBranch;
			repositoryItemLookUpEdit.DataSource = await HttpClient.Commons.GetBranchList(GlobalData.UserName);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task GetDepartment()
	{
		try
		{
			List<CFL_Department> dataSource;
			using (HttpClientHelper<CFL_Department> helper = new HttpClientHelper<CFL_Department>(Configuration.ApiServer, FormController))
			{
				dataSource = await helper.GetListAsync("GetDepartment");
			}
			rDocGLU_Department.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task BindDataToContactPerson(string cardCode)
	{
		try
		{
			using HttpClientHelper<CFL_ContactPerson> helper = new HttpClientHelper<CFL_ContactPerson>(Configuration.ApiServer, "Commons");
			List<CFL_ContactPerson> dataSource = await helper.GetListAsync("GetContactPerson", new { cardCode });
			rDocContactPerson.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void CheckBusinessPartner(string cardCode)
	{
		try
		{
			DataTable businessPartner = HttpClient.Commons.GetBusinessPartner(cardCode);
			if (businessPartner.IsNotNull() && businessPartner.Rows.Count == 1)
			{
				SetItemToDoc(businessPartner.Rows[0]);
			}
			else
			{
				ChooseFromListDoc(cardCode);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ChooseFromListDoc(string cardCode)
	{
		try
		{
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.Customer, multiSelect: false, cardCode);
			frm_ChooseFromList2.ShowDialog(this);
			if (frm_ChooseFromList2.ChooseItem != null)
			{
				BusinessPartnersImpl businessPartnersImpl = (BusinessPartnersImpl)frm_ChooseFromList2.ChooseItem;
				gv_Doc.SetFocusedRowCellValue(gcDocCardCode, businessPartnersImpl.CardCode);
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
			int docEntry = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("DocEntry"));
			if (grd_Doc.DataSource is DocumentTable documentTable)
			{
				DocumentRow documentRow = documentTable.GetEnumerable().FirstOrDefault((DocumentRow p) => p.DocNum == docEntry);
				if (documentRow != null)
				{
					documentRow.CardCode = Function.ToString(buss["CardCode"]);
					documentRow.CardName = Function.ToString(buss["CardName"]);
					documentRow.ContactPersonCode = Function.ParseInt(buss["CntctCode"]);
					documentRow.DocCur = Function.ToString(buss["BaseCurrency"]);
					documentRow.DocRate = Function.ParseDouble(buss["Rate"]);
					documentRow.Currency = Function.ToString(buss["Currency"]);
				}
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

	private void ChooseFormListItemCode(string itemCode)
	{
		try
		{
			frm_CFL frm_CFL2 = ((BaseType == -1) ? new frm_CFL(Enums.TypeCFL.ListItems, true, itemCode, "O", Enums.ItemObjectType.InventoryItem, null, "") : new frm_CFL(TypeProductionOrder, true, "", "", Enums.ItemObjectType.NoItemType, null, ""));
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
						DataRow dataRow2 = dataSourceTable.NewRow();
						dataSourceTable.Rows.InsertAt(dataRow2, focusedDataSourceRowIndex);
						dataSourceTable.AcceptChanges();
						grd_Lines.DataSource = dataSourceTable;
						int num2 = gv_Lines.FindRowHandleByDataRow(dataRow2);
						if (num2 >= 0)
						{
							gv_Lines.FocusedRowHandle = num2;
							gv_Lines.SetFocusedRowCellValue(gc_Line_ItemCode, dataRow["ItemCode"]);
							if (BaseType != -1)
							{
								dataRow2["BaseRef"] = dataRow["DocNum"];
								dataSourceTable.AcceptChanges();
							}
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
				gv_Doc.SetFocusedRowCellValue("TotalBeforeDis", num3);
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
		WinCommons._CloseProcessing();
	}

	public virtual async Task<bool> ApprovalDocuments(DocumentsImpl doc, bool isDraft)
	{
		try
		{
			if (GlobalData.GeneralSetting.useDocWrf == "Y" && !isDraft && (!_onlyChangeRemark || (doc.IsDraft && (doc.WddStatus == "" || doc.WddStatus == "-"))))
			{
				using HttpClientHelper<DataTable> dtClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
				if (!(await doc.GetApprovalTemplate(dtClientHelper, ((int)FormMode).ToString(), this)))
				{
					WinCommons.Error(Enums.MessageCode.CannotGetApprovalTemplate, "");
					return false;
				}
			}
			return true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return false;
		}
	}

	private async Task Save(bool isdraft)
	{
		if (!grd_Doc.DataSourceTable.IsNotNull())
		{
			return;
		}
		if (!Function.ParseDateTime(gv_Doc.GetFocusedRowCellValue("DocDate")).HasValue && !isdraft)
		{
			gv_Doc.Focus();
			gv_Doc.FocusedColumn = gcDocDate;
			gv_Doc.ShowEditor();
			WinCommons.Error(Enums.MessageCode.DocDateCannotBeNull, "");
			return;
		}
		try
		{
			gv_Doc.MakeRowVisible(gv_Doc.FocusedRowHandle);
			if (XtraMessageBoxEx.Show(isdraft ? Enums.MessageCode.ConfirmSaveDraft : Enums.MessageCode.ConfirmSaveDocument, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
			{
				return;
			}
			_saving = true;
			string text = CheckDataLine(out var dt);
			if (!string.IsNullOrEmpty(text))
			{
				WinCommons.Error(Enums.MessageCode.None, text);
				return;
			}
			EnableDisableControls(isEnable: false);
			WinCommons._OpenProcessing();
			if (GlobalData.LoginSap)
			{
				DocumentsImpl doc = gv_Doc.GetFocusedDataRow().ToDocument();
				if (BaseType == 202)
				{
					doc.TaxDate = new DateTime(1900, 1, 1);
				}
				doc.ReqDate = new DateTime(1900, 1, 1);
				doc.BPLId = doc.Branch;
				if (FormMode == Enums.FormViewModeType.Add)
				{
					doc.DocEntry = 0;
				}
				List<Document_LinesImpl> list = dt.ToDocumentLine(priceSpecified: true);
				DataTable dataTable = ucBatchAndSerial1.CheckDataBatch(dt, isdraft);
				if (dataTable == null)
				{
					EnableDisableControls(isEnable: true);
					WinCommons._CloseProcessing();
					return;
				}
				list.LoadDataBatch(dataTable, ObjectType);
				DataTable dataTable2 = ucBatchAndSerial1.CheckDataBinAllowcationLine(dt, isdraft);
				if (dataTable2 == null)
				{
					EnableDisableControls(isEnable: true);
					WinCommons._CloseProcessing();
					return;
				}
				list.LoadBinAllowcation(dataTable2, ObjectType);
				doc.Lines = list;
				if (!(await ApprovalDocuments(doc, isdraft)))
				{
					EnableDisableControls(isEnable: true);
					return;
				}
				List<AttachmentFile_Impl> list2 = ucAttachment1.UploadFile();
				if (ucAttachment1.DataSource.IsNotNull() && list2.Count == 0)
				{
					EnableDisableControls(isEnable: true);
					return;
				}
				doc.AttachmentFiles = list2;
				string text2 = Function.ToString(gv_Doc.GetFocusedRowCellValue("WddStatus"));
				HttpResult httpResult;
				if (!isdraft && (!doc.IsDraft || !(text2 == "W") || doc.UserSign != GlobalData.UserID))
				{
					if (doc.IsDraft && _onlyChangeRemark && (doc.Document_ApprovalRequests == null || doc.Document_ApprovalRequests.Count == 0))
					{
						using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "WorkingProcess");
						httpResult = await helper.PostAsync("SaveDraftToDocument", doc);
					}
					else
					{
						using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, FormController);
						httpResult = await helper.PostAsync("SaveDoc", doc);
					}
				}
				else
				{
					if (doc.IsDraft && (text2 == "-" || (_onlyChangeRemark && text2 == "W" && doc.UserSign == GlobalData.UserID)))
					{
						doc.DocEntry = doc.draftKey;
					}
					if (FormMode == Enums.FormViewModeType.Add && text2 == "-")
					{
						doc.DocNum = -1;
					}
					using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, FormController);
					httpResult = await helper.PostAsync("SaveAsDraft", doc);
				}
				_saving = false;
				if (httpResult.MessageCode == Enums.MessageCode.Success)
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
					switch (_saveAction)
					{
					case Enums.SaveAction.SaveAndSearch:
						FormMode = Enums.FormViewModeType.View;
						await SearchData(Enums.NavigateType.Current);
						break;
					case Enums.SaveAction.SaveAndClose:
						FormMode = Enums.FormViewModeType.View;
						Close();
						break;
					case Enums.SaveAction.SaveAndNew:
						FormMode = Enums.FormViewModeType.Add;
						await AddNewRowDoc();
						AddNewBlankRowLine();
						break;
					case Enums.SaveAction.SaveAndView:
						FromEntry = Function.ParseInt(httpResult.Content);
						FormMode = Enums.FormViewModeType.View;
						await SearchData(Enums.NavigateType.Current);
						break;
					}
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				}
				return;
			}
			DataTable dataTable3 = ucBatchAndSerial1.CheckDataBatch(dt, isdraft);
			if (dataTable3 == null)
			{
				EnableDisableControls(isEnable: true);
				WinCommons._CloseProcessing();
				return;
			}
			DataSet dataSet = new DataSet();
			DataTable dataTable4 = grd_Doc.DataSourceTable.Clone();
			DataRow dataRow = dataTable4.NewRow();
			dataRow.ItemArray = gv_Doc.GetFocusedDataRow().ItemArray;
			dataRow["TaxDate"] = new DateTime(1900, 1, 1);
			dataRow["BPLId"] = dataRow["Branch"];
			dataTable4.Rows.Add(dataRow);
			dataTable4.AcceptChanges();
			dataTable4.TableName = "DOC";
			dt.TableName = "LINES";
			dataTable3.TableName = "BatchAndSeralNumer";
			if (!ucAttachment1.GetAttachmentNoSAP(out var dt2, (FromEntry == 0) ? "" : Function.ToString(FromEntry), Function.ParseInt(dataTable4.Rows[0]["AtcEntry"])))
			{
				EnableDisableControls(isEnable: true);
				return;
			}
			dt2.TableName = "ATTFILE";
			dataSet.Tables.Add(dataTable4);
			dataSet.Tables.Add(dt);
			dataSet.Tables.Add(dataTable3);
			dataSet.Tables.Add(dt2);
			HttpResult httpResult2;
			using (HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, FormController))
			{
				httpResult2 = await helper.PostAsync(isdraft ? "SaveAsDraftDataset" : "SaveDataset", dataSet);
			}
			if (string.IsNullOrEmpty(httpResult2.Message))
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
				FormMode = Enums.FormViewModeType.View;
				await RefreshData();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, httpResult2.Message);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			EnableDisableControls(isEnable: true);
			WinCommons._CloseProcessing();
			_saving = false;
		}
	}

	private void ChooseFromListRequester(string requester)
	{
		try
		{
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.Requester, multiSelect: false, requester);
			frm_ChooseFromList2.ShowDialog(this);
			if (frm_ChooseFromList2.ChooseItem != null)
			{
				RequesterImpl itemToRequester = (RequesterImpl)frm_ChooseFromList2.ChooseItem;
				SetItemToRequester(itemToRequester);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void SetItemToRequester(RequesterImpl buss)
	{
		try
		{
			int docEntry = await GetNewDocEntry();
			DocumentRow documentRow = ((DocumentTable)grd_Doc.DataSource).GetEnumerable().FirstOrDefault((DocumentRow p) => p.DocNum == docEntry);
			if (documentRow != null)
			{
				documentRow.Requester = buss.Requester;
				documentRow.ReqName = buss.ReqName;
			}
			grd_Doc.RefreshDataSource();
			grd_Doc.Refresh();
			gv_Doc.FocusedColumn = gcDocRequester;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ClearGrids()
	{
		grd_Doc.DataSourceTable.Rows.Clear();
		ucBatchAndSerial1.ClearLineDataSource();
		grd_Lines.DataSourceTable.Rows.Clear();
		ucAttachment1.grd_File.DataSourceTable.Rows.Clear();
	}

	private async Task SearchData(Enums.NavigateType type)
	{
		int num = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("DocNum"));
		ClearGrids();
		gv_Doc.ShowLoadingPanel();
		try
		{
			_isSearching = true;
			DataTable dataTable;
			using (HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController))
			{
				dataTable = await http.GetAsync("GetDataDoc", new
				{
					baseType = BaseType,
					docNum = ((type == Enums.NavigateType.Current) ? Function.ParseInt(txtNo.EditValue) : num),
					docEntry = FromEntry,
					priList = Function.ParseInt(txtPriceList.EditValue),
					fromDate = (string.IsNullOrEmpty(txtfDate.Text) ? "" : txtfDate.DateTime.ToString("ddMMyyyy")),
					toDate = (string.IsNullOrEmpty(txttDate.Text) ? "" : txttDate.DateTime.ToString("ddMMyyyy")),
					type = type,
					userCode = GlobalData.UserName
				});
			}
			FromEntry = 0;
			grd_Doc.DataSource = dataTable;
			_originalDocTable = dataTable.Copy();
			grd_Doc.RefreshDataSource();
			gv_Doc.HideLoadingPanel();
			if (dataTable.IsNotNull())
			{
				gv_Doc.OptionsSelection.EnableAppearanceHotTrackedRow = DefaultBoolean.False;
				await BindDataLines();
				gv_Doc.OptionsSelection.EnableAppearanceHotTrackedRow = DefaultBoolean.True;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		finally
		{
			_isSearching = false;
			gv_Doc.HideLoadingPanel();
		}
	}

	private async Task<DataSet> GetLines(int docEntry)
	{
		try
		{
			using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, FormController);
			return await helper.GetAsync("SelectOne", new
			{
				idDocEntry = docEntry
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return null;
		}
	}

	private async Task BindDataLines()
	{
		_ = 2;
		try
		{
			ucBatchAndSerial1.ClearDataSource();
			grd_Lines.DataSourceTable.Rows.Clear();
			gv_Lines.ShowLoadingPanel();
			DataSet dataSet = await GetLines(Function.ParseInt(gv_Doc.GetFocusedRowCellValue("DocEntry")));
			if (dataSet == null || dataSet.Tables.Count != 3)
			{
				grd_Lines.DataSourceTable.Rows.Clear();
				WinCommons.Error(Enums.MessageCode.DataLinesIsEmpty, "");
				return;
			}
			Task t = ucAttachment1.LoadAttachments(Function.ParseInt(gv_Doc.GetFocusedRowCellValue("AtcEntry")));
			ucBatchAndSerial1.DataSourceBatchAndSerial = dataSet.Tables[1];
			ucBatchAndSerial1.DataSourceBinAllowLine = dataSet.Tables[2];
			if (DocEntryIssueComponents > 0)
			{
				DataSet dataSet2 = await HttpClient.ProductionOrders.GetIssueComponents(DocEntryIssueComponents);
				if (dataSet2.IsNotNull())
				{
					if (dataSet2.Tables.Count > 1)
					{
						ucBatchAndSerial1.DataSourceBatchAndSerial = dataSet2.Tables[1].Copy();
					}
					grd_Lines.DataSource = dataSet2.Tables[0].Copy();
					grd_Lines.RefreshDataSource();
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.CannotGetIssueComponent, "");
				}
				DocEntryIssueComponents = 0;
			}
			else
			{
				grd_Lines.DataSource = dataSet.Tables[0];
				grd_Lines.RefreshDataSource();
			}
			if (gv_Lines.FocusedRowHandle < 0)
			{
				ucBatchAndSerial1.FinishEditorBatchAndSerial();
				ucBatchAndSerial1.SetFocusLine();
			}
			if (FormMode == Enums.FormViewModeType.Add)
			{
				AddNewBlankRowLine();
			}
			if (DocEntryProductionOrder > 0)
			{
				SetBaseEntryToLine(DocEntryProductionOrder, 0);
				DocEntryProductionOrder = 0;
			}
			await t;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			gv_Lines.HideLoadingPanel();
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
			grd_Lines.DataSourceTable.Rows.Clear();
			grd_Lines.DataSourceTable.AcceptChanges();
			ucAttachment1.grd_File.DataSourceTable.Rows.Clear();
			DataTable tableDoc = grd_Doc.DataSourceTable;
			if (tableDoc == null || tableDoc.Columns.Count == 0)
			{
				tableDoc = DocumentsHelper.Document.Clone();
			}
			tableDoc.Rows.Clear();
			tableDoc.AcceptChanges();
			tableDoc.Clear();
			DataRow doc = tableDoc.NewRow();
			await GetNewDocEntry();
			doc["DocEntry"] = DBNull.Value;
			doc["DocStatus"] = "O";
			doc["ObjType"] = (int)ObjectType;
			doc["Instance"] = 0;
			doc["Segment"] = 0;
			doc["PIndicator"] = "Default";
			doc["DocSubType"] = "--";
			doc["Branch"] = GlobalData.DefaultBranch;
			doc["DocDate"] = DateTime.Today;
			doc["DocDueDate"] = DateTime.Today;
			doc["TaxDate"] = DateTime.Today;
			doc["DocCur"] = GlobalData.MainCurrency;
			doc["ReqDate"] = DateTime.Today;
			doc["GroupNum"] = -1;
			doc["JrnlMemo"] = Text;
			doc["DocRate"] = 1;
			doc["Currency"] = GlobalData.MainCurrency;
			if (BaseType == 202)
			{
				doc["JrnlMemo"] = ((ObjectType == ApzObjectTypes.oInventoryGenEntry) ? "Receipt from production" : "Issue for production");
			}
			doc["Ref2"] = string.Empty;
			doc["Series"] = 0;
			doc["CANCELED"] = "N";
			tableDoc.Rows.Add(doc);
			tableDoc.AcceptChanges();
			grd_Doc.DataSource = tableDoc;
			grd_Doc.RefreshDataSource();
			LoadSeriesByPostingDate();
			_onlyChangeRemark = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ChooseFromListItemAcctCode()
	{
		try
		{
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.AcctCode, multiSelect: false, Function.ToString(gv_Lines.GetFocusedRowCellValue(gcLineAcctCode)));
			frm_ChooseFromList2.ShowDialog(this);
			if (frm_ChooseFromList2.ChooseItem != null)
			{
				string acctCode = ((AcctCodeImpl)frm_ChooseFromList2.ChooseItem).AcctCode;
				if (!string.IsNullOrEmpty(acctCode))
				{
					gv_Lines.SetFocusedRowCellValue("AcctCode", acctCode);
				}
			}
			gv_Lines.FocusedColumn = gc_LineTotal;
			gv_Lines.ShowEditor();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task<int> GetNewDocNum(int series)
	{
		try
		{
			using HttpClientHelper<string> http = new HttpClientHelper<string>(Configuration.ApiServer, FormController);
			return await http.GetIntAsync("GetNewDocNum", new { series });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return 0;
	}

	private void AddNewBlankRowLine()
	{
		try
		{
			if (gv_Doc.FocusedRowHandle < 0)
			{
				return;
			}
			DataTable dataTable = grd_Lines.DataSourceTable;
			if (dataTable == null || dataTable.Columns.Count == 0)
			{
				DataSet result = Task.Run(() => GetLines(0)).Result;
				if (result == null || result.Tables.Count == 0 || result.Tables[0].Columns.Count == 0)
				{
					WinCommons.Error(Enums.MessageCode.DataLinesIsEmpty, "");
					return;
				}
				dataTable = result.Tables[0].Clone();
			}
			if (!dataTable.IsNotNull() || dataTable.Select("ItemCode = ''").Length == 0)
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["DocEntry"] = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("DocEntry"));
				object value = (dataRow["Dscription"] = string.Empty);
				dataRow["ItemCode"] = value;
				dataRow["LineNum"] = GetNewLineNum();
				dataRow["Quantity"] = 1;
				dataRow["Price"] = 0;
				dataRow["PQTReqdate"] = DateTime.Today;
				dataRow["ShipDate"] = DateTime.Today;
				dataRow["Currency"] = GlobalData.MainCurrency;
				dataRow["WhsCode"] = string.Empty;
				dataRow["BaseType"] = BaseType;
				dataRow["BaseEntry"] = 0;
				dataRow["LineStatus"] = "O";
				dataRow["AcctCode"] = "";
				dataRow["ManBtchSerial"] = "N";
				dataTable.Rows.Add(dataRow);
				grd_Lines.DataSource = dataTable;
				grd_Lines.DataSourceTable.AcceptChanges();
				grd_Lines.RefreshDataSource();
				_onlyChangeRemark = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task GetSeries()
	{
		try
		{
			DataTable dataTable = await _httpClient.Documents.LoadSeries(GlobalData.UserName);
			rDocSeries.DataSource = dataTable;
			_dtSeries = dataTable.Copy();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task<DataTable> GetItemOnwh(string item, string wh)
	{
		try
		{
			using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons");
			return await http.GetAsync("GetItemOnwh", new { item, wh });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new DataTable();
	}

	private DataTable GetPriceItem(int priceList, string itemcode)
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons");
			return httpClientHelper.Get("GetPriceItem", new { priceList, itemcode });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new DataTable();
	}

	private async Task InitCustomize()
	{
		try
		{
			TableSearch = new DataTable();
			TableSearch.Columns.Add("FromDate", typeof(DateTime));
			TableSearch.Columns.Add("ToDate", typeof(DateTime));
			TableSearch.Columns.Add("Number", typeof(int));
			TableSearch.Columns.Add("PriceList", typeof(string));
			TableSearch.Columns.Add("BaseType", typeof(string));
			Task task = WinFuntions.LoadUdfColumns(gv_Doc, gv_Doc.ItemId, TblFormular);
			Task task2 = WinFuntions.LoadUdfColumns(gv_Lines, gv_Lines.ItemId, TblFormular);
			CreateColumnsDistRule(gv_Lines);
			await Task.WhenAll(task, task2);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void LoadTableFormular()
	{
		TblFormular[gv_Doc.ItemId] = gv_Doc;
		TblFormular[gv_Lines.ItemId] = gv_Lines;
	}

	private async Task<int> GetNewDocEntry()
	{
		try
		{
			using HttpClientHelper<DocumentLineRow> helper = new HttpClientHelper<DocumentLineRow>(Configuration.ApiServer, FormController);
			return await helper.GetIntAsync("GetMaxDocEntry") + 1;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return 0;
		}
	}

	private string CheckDataLine(out DataTable dt)
	{
		dt = grd_Lines.DataSourceTable.Copy();
		for (int i = 0; i < dt.Rows.Count; i++)
		{
			if (string.IsNullOrEmpty(Function.ToString(dt.Rows[i]["ItemCode"])))
			{
				dt.Rows.RemoveAt(i--);
				continue;
			}
			if (string.IsNullOrEmpty(Function.ToString(dt.Rows[i]["WhsCode"])))
			{
				gv_Lines.Focus();
				gv_Lines.FocusedRowHandle = i;
				gv_Lines.FocusedColumn = gcLineWarehouseCode;
				gv_Lines.ShowEditor();
				return "You have not enter Whs!";
			}
			if (Function.ParseDouble(dt.Rows[i]["Quantity"]) <= 0.0)
			{
				gv_Lines.Focus();
				gv_Lines.FocusedRowHandle = i;
				gv_Lines.FocusedColumn = gcLineQuantity;
				gv_Lines.ShowEditor();
				return "In \"Qty\" (Quantity) column, enter value greater than 0";
			}
		}
		if (dt.Rows.Count == 0)
		{
			return "On \"Contents\" tab, enter item or items";
		}
		return "";
	}

	private async Task SetWhs(string whsCode)
	{
		try
		{
			gv_Lines.RefreshEditor(updateEditorValue: true);
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.Warehouse, multiSelect: false, whsCode);
			frm_ChooseFromList2.ShowDialog(this);
			if (frm_ChooseFromList2.ChooseItem != null)
			{
				DataRow dataRow = (DataRow)frm_ChooseFromList2.ChooseItem;
				if (Function.ToString(dataRow["WhsCode"]) != "" && Function.ToString(dataRow["WhsCode"]).ToUpper() != whsCode.ToUpper())
				{
					gv_Lines.SetFocusedRowCellValue("WhsCode", dataRow["WhsCode"]);
					DataTable dataTable = await GetItemOnwh(Function.ToString(gv_Lines.GetFocusedRowCellValue("ItemCode")), Function.ToString(dataRow["WhsCode"]));
					if (dataTable != null && dataTable.Rows.Count > 0)
					{
						gv_Lines.SetFocusedRowCellValue("QuantityOnWareHouse", Function.ParseDouble(dataTable.Rows[0]["InWhs"]));
					}
				}
			}
			gv_Lines.FocusedColumn = gcLineQuantity;
			gv_Lines.ShowEditor();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
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
			rLineInventoryUom.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task GetLineCurrency()
	{
		RepositoryItemLookUpEdit repositoryItemLookUpEdit = rLineCurrency;
		repositoryItemLookUpEdit.DataSource = await HttpClient.Commons.GetCurrency();
	}

	private async Task LoadUomGroup()
	{
		try
		{
			RepositoryItemLookUpEdit repositoryItemLookUpEdit = rLineUomGroup;
			repositoryItemLookUpEdit.DataSource = await HttpClient.Commons.GetDataUomGroup();
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
			return HttpClient.Commons.GetDataItemCode(itemCode, priceList, GlobalData.UserName, GlobalData.DefaultBranch);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new DataTable();
	}

	private async Task ShowChooseFromListProductionOrder()
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(TypeProductionOrder, true, Function.ToString(gv_Lines.GetFocusedRowCellValue("BaseRef")), "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItems == null || frm_CFL2.ChooseItems.Count <= 0)
			{
				return;
			}
			int focusedRowHandle = gv_Lines.FocusedRowHandle;
			int focusedDataSourceRowIndex = gv_Lines.GetFocusedDataSourceRowIndex();
			DataTable dataSourceTable = grd_Lines.DataSourceTable;
			for (int i = 0; i < frm_CFL2.ChooseItems.Count; i++)
			{
				DataRow dataRow = frm_CFL2.ChooseItems[i];
				DataRow row = dataSourceTable.NewRow();
				dataSourceTable.Rows.InsertAt(row, focusedDataSourceRowIndex++);
				dataSourceTable.AcceptChanges();
				gv_Lines.FocusedRowHandle = focusedRowHandle++;
				if (dataRow != null)
				{
					DataRow focusedDataRow = gv_Lines.GetFocusedDataRow();
					focusedDataRow["BaseRef"] = dataRow["DocNum"];
					focusedDataRow["BaseEntry"] = dataRow["DocEntry"];
					focusedDataRow["BaseLine"] = ((Function.ParseInt(dataRow["LineNum"]) >= 0) ? dataRow["LineNum"] : ((object)(-1)));
					changeItemFromBaseRef = true;
					gv_Lines.SetFocusedRowCellValue(gc_Line_ItemCode, dataRow["ItemCode"]);
					changeItemFromBaseRef = false;
					focusedDataRow["Quantity"] = dataRow["Qty"];
					focusedDataRow["WhsCode"] = dataRow["Warehouse"];
					focusedDataRow["Dscription"] = dataRow["ItemName"];
					focusedDataRow["NumPerMsr"] = 1;
					focusedDataRow["UgpCode"] = dataRow["UgpEntry"];
					focusedDataRow["UseBaseUn"] = "Y";
					focusedDataRow["unitMsr"] = dataRow["InvntryUom"];
					focusedDataRow["InvUnitMsr"] = dataRow["InvntryUom"];
					focusedDataRow["Project"] = dataRow["Project"];
					focusedDataRow["OcrCode"] = dataRow["OcrCode"];
					focusedDataRow["OcrCode2"] = dataRow["OcrCode2"];
					focusedDataRow["OcrCode3"] = dataRow["OcrCode3"];
					focusedDataRow["OcrCode4"] = dataRow["OcrCode4"];
					focusedDataRow["OcrCode5"] = dataRow["OcrCode5"];
					if (dataRow.IsExistsColumn("IsByPrdct"))
					{
						focusedDataRow["IsByPrdct"] = dataRow["IsByPrdct"];
					}
					if (Function.ParseInt(dataRow["UgpEntry"]) == -1)
					{
						focusedDataRow["UomCode"] = gv_Lines.GetFocusedRowCellDisplayText("UgpCode");
					}
					else
					{
						focusedDataRow["UomCode"] = WinFuntions.GetUoMCodeByName(Function.ParseInt(dataRow["UgpEntry"]), Function.ToString(dataRow["InvntryUom"]));
					}
					focusedDataRow[gcLineIvnvQty.FieldName] = Function.ParseDouble(gv_Lines.GetFocusedRowCellValue(gcLineQuantity)) * Function.ParseDouble(gv_Lines.GetFocusedRowCellValue(gcLineItemPerUnit));
					focusedDataRow[gcLineOpenInvQty.FieldName] = Function.ParseDouble(gv_Lines.GetFocusedRowCellValue(gcLineQuantity)) * Function.ParseDouble(gv_Lines.GetFocusedRowCellValue(gcLineItemPerUnit));
					gv_Lines.SetFocusedRowCellValue(gc_LinePrice, dataRow["AvgPrice"]);
				}
			}
			dataSourceTable.AcceptChanges();
			ucBatchAndSerial1.CalculateOpenQty();
			double num = 0.0;
			double num2 = 0.0;
			for (int j = 0; j < dataSourceTable.Rows.Count; j++)
			{
				num += Function.ParseDouble(dataSourceTable.Rows[j]["LineTotal"]);
				num2 += Function.ParseDouble(dataSourceTable.Rows[j]["LineVat"]);
			}
			gv_Doc.SetFocusedRowCellValue("TotalBeforeDis", num);
			gv_Doc.SetFocusedRowCellValue("VatSum", num2);
			gv_Doc.SetFocusedRowCellValue("DocTotal", num2 + num);
			grd_Lines.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void SetBaseEntryToLine(int baseEntry, int i)
	{
		DataTable result = Task.Run(() => GetByBaseEntry(baseEntry)).Result;
		if (result.IsNotNull())
		{
			for (int num = 0; num < result.Rows.Count; num++)
			{
				DataRow dataRow = gv_Lines.GetDataRow(i);
				if (dataRow != null)
				{
					dataRow["BaseRef"] = Function.ParseInt(result.Rows[num]["DocNum"]);
					dataRow["BaseEntry"] = baseEntry;
					if (result.ExistsColumn("BaseLine"))
					{
						dataRow["BaseLine"] = result.Rows[num]["BaseLine"];
					}
					dataRow["ItemCode"] = Function.ToString(result.Rows[num]["ItemCode"]);
					decimal num2 = Function.ParseDecimal(result.Rows[num]["PlannedQty"]) - Function.ParseDecimal(result.Rows[num]["CmpltQty"]);
					if (num2 <= 0m)
					{
						num2 = 1m;
					}
					dataRow["Quantity"] = num2;
					dataRow["WhsCode"] = Function.ToString(result.Rows[num]["wareHouse"]);
					dataRow["Dscription"] = Function.ToString(result.Rows[num]["ItemName"]);
					dataRow["NumPerMsr"] = 1;
					dataRow["UgpCode"] = result.Rows[num]["UgpEntry"];
					dataRow["UseBaseUn"] = "Y";
					dataRow["unitMsr"] = result.Rows[num]["InvntryUom"];
					dataRow["InvUnitMsr"] = result.Rows[num]["InvntryUom"];
					dataRow["Project"] = result.Rows[num]["Project"];
					dataRow["OcrCode"] = result.Rows[num]["OcrCode"];
					dataRow["OcrCode2"] = result.Rows[num]["OcrCode2"];
					dataRow["OcrCode3"] = result.Rows[num]["OcrCode3"];
					dataRow["OcrCode4"] = result.Rows[num]["OcrCode4"];
					dataRow["OcrCode5"] = result.Rows[num]["OcrCode5"];
					if (result.IsExistsColumn("IsByPrdct"))
					{
						dataRow["IsByPrdct"] = result.Rows[num]["IsByPrdct"];
					}
					if (Function.ParseInt(result.Rows[num]["UgpEntry"]) == -1)
					{
						dataRow["UomCode"] = gv_Lines.GetFocusedRowCellDisplayText("UgpCode");
					}
					else
					{
						dataRow["UomCode"] = WinFuntions.GetUoMCodeByName(Function.ParseInt(result.Rows[num]["UgpEntry"]), Function.ToString(result.Rows[num]["InvntryUom"]));
					}
					dataRow["ManBtchSerial"] = Function.ToString(result.Rows[num]["ManBtchSerial"]);
					dataRow[gcLineIvnvQty.FieldName] = Function.ParseDouble(gv_Lines.GetRowCellValue(i, gcLineQuantity)) * Function.ParseDouble(gv_Lines.GetRowCellValue(i, gcLineItemPerUnit));
					dataRow[gcLineOpenInvQty.FieldName] = Function.ParseDouble(gv_Lines.GetRowCellValue(i, gcLineQuantity)) * Function.ParseDouble(gv_Lines.GetRowCellValue(i, gcLineItemPerUnit));
					gv_Lines.SetRowCellValue(i, gc_LinePrice, result.Rows[num]["AvgPrice"]);
				}
				AddNewBlankRowLine();
				i++;
			}
			grd_Lines.DataSourceTable.AcceptChanges();
			grd_Lines.RefreshDataSource();
			ucBatchAndSerial1.SetFocusLine(isChangeItem: true);
		}
		else
		{
			WinCommons.Error(Enums.MessageCode.NoMatchingRecordFound, "");
		}
	}

	private void EnableDisableControls(bool isEnable)
	{
		try
		{
			drlCopyFrom.Enabled = isEnable && ObjectType == ApzObjectTypes.oInventoryGenEntry && FormMode == Enums.FormViewModeType.Add;
			drlCopyTo.Enabled = isEnable && ObjectType == ApzObjectTypes.oInventoryGenExit && FormMode != Enums.FormViewModeType.Add && Function.ToString(gv_Doc.GetFocusedRowCellValue("DocStatus")) == "O";
			txtNo.ReadOnly = !isEnable;
			txtPriceList.ReadOnly = !isEnable;
			txtfDate.ReadOnly = !isEnable;
			txttDate.ReadOnly = !isEnable;
			txtPriceList.Properties.Buttons[0].Visible = isEnable;
			txtfDate.Properties.Buttons[0].Visible = isEnable;
			txttDate.Properties.Buttons[0].Visible = isEnable;
			btnSearch.Enabled = isEnable;
			btn_Save.Enabled = isEnable;
			btn_Cancel.Enabled = isEnable;
			gv_Doc.ReadOnly = !isEnable;
			gv_Lines.ReadOnly = !isEnable;
			contextMenuStrip_Document.Enabled = isEnable;
			contextMenuStrip_Line.Enabled = isEnable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void EnableDisableGridLine(bool isEnable)
	{
		gv_Lines.SetViewModeButtonEdit(isEnable, isHeader: false);
		if (BaseType != -1 && !isEnable)
		{
			rLineItemCode.Buttons[0].Visible = false;
		}
		List<string> list = new List<string>();
		if (FormMode == Enums.FormViewModeType.Add)
		{
			list.AddRange(new string[4] { "TranType", "Quantity", "WhsCode", "Project" });
			foreach (DataRow row in _dtDim.Rows)
			{
				string text = Function.ToString(row["DimCode"]);
				list.Add("OcrCode" + ((text == "1") ? "" : text));
			}
		}
		for (int i = 0; i < gv_Lines.Columns.Count; i++)
		{
			if (list.Contains(gv_Lines.Columns[i].FieldName))
			{
				gv_Lines.Columns[i].OptionsColumn.ReadOnly = false;
				RepositoryItem columnEdit = gv_Lines.Columns[i].ColumnEdit;
				if (columnEdit is RepositoryItemButtonEdit)
				{
					RepositoryItemButtonEdit repositoryItemButtonEdit = columnEdit as RepositoryItemButtonEdit;
					if (repositoryItemButtonEdit.Buttons.Count > 0)
					{
						repositoryItemButtonEdit.Buttons[0].Visible = true;
					}
				}
			}
			else
			{
				gv_Lines.Columns[i].OptionsColumn.ReadOnly = !isEnable;
			}
		}
	}

	private void SetViewGridDoc(bool isView)
	{
		gv_Doc.SetViewModeButtonEdit(!isView);
		for (int i = 0; i < gv_Doc.Columns.Count; i++)
		{
			if (!gv_Doc.Columns[i].FieldName.StartsWith("U_"))
			{
				gv_Doc.Columns[i].OptionsColumn.ReadOnly = isView;
				if (gv_Doc.Columns[i].FieldName == gcDocTaxDate.FieldName || gv_Doc.Columns[i].FieldName == gcDocDocRef2.FieldName || gv_Doc.Columns[i].FieldName == gcDoc_JournalMemo.FieldName || gv_Doc.Columns[i].FieldName == gcDocComments.FieldName)
				{
					gv_Doc.Columns[i].OptionsColumn.ReadOnly = false;
				}
			}
		}
	}

	private async Task DuplicateDoc()
	{
		try
		{
			if (FormAuth != Enums.FormAuthorities.Full)
			{
				return;
			}
			DataRow doc = gv_Doc.GetFocusedDataRow();
			if (doc == null)
			{
				WinCommons.writeStatus(Enums.MessageCode.NeedChoseDocumentDuplicate, Enums.MessageType.Error, Msg_Time._TimeMedium, "");
				return;
			}
			WinCommons._OpenProcessing();
			EnableDisableControls(isEnable: false);
			FormMode = Enums.FormViewModeType.Add;
			DataTable tbdoc = grd_Doc.DataSourceTable.Clone();
			DataTable tbline = grd_Lines.DataSourceTable.Copy();
			object value = (doc["draftKey"] = DBNull.Value);
			doc["DocEntry"] = value;
			if (!string.IsNullOrEmpty(Function.ToString(doc["Series"])))
			{
				DataRow dataRow = doc;
				dataRow["DocNum"] = await GetNewDocNum(Function.ParseInt(doc["Series"]));
			}
			object obj = (doc["DocDueDate"] = DateTime.Now.Date);
			value = (doc["TaxDate"] = obj);
			doc["DocDate"] = value;
			doc["WddStatus"] = "";
			tbdoc.ImportRow(doc);
			tbdoc.AcceptChanges();
			_ = tbline.Rows.Count;
			IsLoadForm = true;
			grd_Doc.DataSource = tbdoc;
			grd_Doc.RefreshDataSource();
			IsLoadForm = false;
			ucBatchAndSerial1.ClearDataSource();
			tbline.AcceptChanges();
			grd_Lines.DataSource = tbline;
			AddNewBlankRowLine();
			_onlyChangeRemark = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			EnableDisableControls(isEnable: true);
			WinCommons._CloseProcessing();
		}
	}

	private void DuplicateRowLine()
	{
		try
		{
			if (string.IsNullOrEmpty(Function.ToString(gv_Lines.GetFocusedRowCellValue(gc_Line_ItemCode))))
			{
				WinCommons.Warning(Enums.MessageCode.DataRowIsEmpty, "");
				return;
			}
			int focusedDataSourceRowIndex = gv_Lines.GetFocusedDataSourceRowIndex();
			DataTable dataSourceTable = grd_Lines.DataSourceTable;
			DataRow dataRow = dataSourceTable.NewRow();
			dataRow.ItemArray = gv_Lines.GetFocusedDataRow().ItemArray;
			dataRow["LineNum"] = GetNewLineNum();
			dataSourceTable.Rows.InsertAt(dataRow, focusedDataSourceRowIndex + 1);
			dataSourceTable.AcceptChanges();
			grd_Lines.DataSource = dataSourceTable;
			grd_Lines.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
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

	protected virtual void SetFormViewMode(Enums.FormViewModeType value)
	{
		ucBatchAndSerial1.SetFormViewMode(value);
		switch (value)
		{
		case Enums.FormViewModeType.Update:
		{
			LineCmtDuplicate.Text = "Duplicate";
			SetViewGridDoc(isView: true);
			EnableDisableGridLine(isEnable: false);
			LineCmtDuplicate.Enabled = false;
			toolStripSeparator1.Visible = false;
			docSaveAsDraftToolStripMenuItem.Visible = false;
			lineDeleteToolStripMenuItem.Visible = false;
			string text2 = _saveAction switch
			{
				Enums.SaveAction.SaveAndNew => "New", 
				Enums.SaveAction.SaveAndView => "View", 
				Enums.SaveAction.SaveAndClose => "Close", 
				_ => "Search", 
			};
			btn_Save.Text = "Update && " + text2;
			if (btn_Save.ImageOptions.Image == null)
			{
				btn_Save.ImageOptions.Image = _btnSaveImg;
			}
			btn_Save.DropDownArrowStyle = DropDownArrowStyle.SplitButton;
			break;
		}
		case Enums.FormViewModeType.Add:
		{
			SetViewGridDoc(isView: false);
			EnableDisableGridLine(isEnable: true);
			toolStripSeparator1.Visible = true;
			docSaveAsDraftToolStripMenuItem.Visible = BaseType != 202;
			lineDeleteToolStripMenuItem.Visible = true;
			LineCmtDuplicate.Text = "Duplicate Row";
			docCmtDuplicate.Enabled = false;
			string text = _saveAction switch
			{
				Enums.SaveAction.SaveAndNew => "New", 
				Enums.SaveAction.SaveAndView => "View", 
				Enums.SaveAction.SaveAndClose => "Close", 
				_ => "Search", 
			};
			btn_Save.Text = "Add && " + text;
			if (btn_Save.ImageOptions.Image == null)
			{
				btn_Save.ImageOptions.Image = _btnSaveImg;
			}
			btn_Save.DropDownArrowStyle = DropDownArrowStyle.SplitButton;
			break;
		}
		case Enums.FormViewModeType.View:
			LineCmtDuplicate.Text = "Duplicate";
			SetViewGridDoc(isView: true);
			EnableDisableGridLine(isEnable: false);
			docCmtDuplicate.Enabled = true;
			toolStripSeparator1.Visible = false;
			docSaveAsDraftToolStripMenuItem.Visible = false;
			lineDeleteToolStripMenuItem.Visible = false;
			btn_Save.Text = "OK";
			btn_Save.ImageOptions.Image = null;
			btn_Save.DropDownArrowStyle = DropDownArrowStyle.Hide;
			break;
		case Enums.FormViewModeType.Find:
			drlCopyFrom.Enabled = false;
			drlCopyTo.Enabled = ObjectType == ApzObjectTypes.oInventoryGenExit;
			break;
		}
		LoadDropDownSave(value);
	}

	private void InitFormatControl()
	{
		try
		{
			txtfDate.SetFormatControl(Enums.DisplayFormatType.DateTime);
			txttDate.SetFormatControl(Enums.DisplayFormatType.DateTime);
			txtNo.SetFormatControl(Enums.DisplayFormatType.Integer);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
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

	private void ucAttachment1_ButtonBrowse_Click(object sender, EventArgs e)
	{
		if (FormMode == Enums.FormViewModeType.View)
		{
			FormMode = Enums.FormViewModeType.Update;
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
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	protected override void NoAuthSetControlSearch()
	{
		ToolStripMenuItem toolStripMenuItem = docSaveAsDraftToolStripMenuItem;
		bool visible = (batchSerialNumberToolStripMenuItem.Visible = true);
		toolStripMenuItem.Visible = visible;
		drlCopyTo.SettingReadonly = false;
		drlCopyTo.Enabled = true;
	}

	private void btn_RetCom_Click(object sender, EventArgs e)
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.ReturnComponents, true, "", "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItems == null || frm_CFL2.ChooseItems.Count <= 0)
			{
				return;
			}
			DataTable dataSourceTable = grd_Lines.DataSourceTable;
			int focusedDataSourceRowIndex = gv_Lines.GetFocusedDataSourceRowIndex();
			int focusedRowHandle = gv_Lines.FocusedRowHandle;
			if (!dataSourceTable.Columns.Contains("IsReturnComponent"))
			{
				dataSourceTable.Columns.Add("IsReturnComponent");
				dataSourceTable.AcceptChanges();
			}
			for (int i = 0; i < frm_CFL2.ChooseItems.Count; i++)
			{
				DataRow dataRow = frm_CFL2.ChooseItems[i];
				DataRow row = dataSourceTable.NewRow();
				dataSourceTable.Rows.InsertAt(row, focusedDataSourceRowIndex++);
				dataSourceTable.AcceptChanges();
				gv_Lines.FocusedRowHandle = focusedRowHandle++;
				if (dataRow != null)
				{
					DataRow focusedDataRow = gv_Lines.GetFocusedDataRow();
					focusedDataRow["IsReturnComponent"] = "Y";
					focusedDataRow["BaseRef"] = dataRow["DocNum"];
					gv_Lines.SetFocusedRowCellValue(gc_Line_ItemCode, dataRow["ItemCode"]);
					focusedDataRow["BaseEntry"] = dataRow["DocEntry"];
					focusedDataRow["BaseLine"] = ((Function.ParseInt(dataRow["LineNum"]) >= 0) ? dataRow["LineNum"] : ((object)(-1)));
					gv_Lines.SetFocusedRowCellValue("Quantity", dataRow["Qty"]);
					focusedDataRow["WhsCode"] = dataRow["Warehouse"];
					focusedDataRow["Dscription"] = dataRow["ItemName"];
					focusedDataRow["NumPerMsr"] = 1;
					focusedDataRow["UgpCode"] = dataRow["UgpEntry"];
					focusedDataRow["UseBaseUn"] = "Y";
					focusedDataRow["unitMsr"] = dataRow["InvntryUom"];
					focusedDataRow["InvUnitMsr"] = dataRow["InvntryUom"];
					focusedDataRow["Project"] = dataRow["Project"];
					focusedDataRow["OcrCode"] = dataRow["OcrCode"];
					focusedDataRow["OcrCode2"] = dataRow["OcrCode2"];
					focusedDataRow["OcrCode3"] = dataRow["OcrCode3"];
					focusedDataRow["OcrCode4"] = dataRow["OcrCode4"];
					focusedDataRow["OcrCode5"] = dataRow["OcrCode5"];
					if (Function.ParseInt(dataRow["UgpEntry"]) == -1)
					{
						focusedDataRow["UomCode"] = gv_Lines.GetFocusedRowCellDisplayText("UgpCode");
					}
					else
					{
						focusedDataRow["UomCode"] = WinFuntions.GetUoMCodeByName(Function.ParseInt(dataRow["UgpEntry"]), Function.ToString(dataRow["InvntryUom"]));
					}
					focusedDataRow[gcLineIvnvQty.FieldName] = Function.ParseDouble(gv_Lines.GetFocusedRowCellValue(gcLineQuantity)) * Function.ParseDouble(gv_Lines.GetFocusedRowCellValue(gcLineItemPerUnit));
					focusedDataRow[gcLineOpenInvQty.FieldName] = Function.ParseDouble(gv_Lines.GetFocusedRowCellValue(gcLineQuantity)) * Function.ParseDouble(gv_Lines.GetFocusedRowCellValue(gcLineItemPerUnit));
				}
			}
			dataSourceTable.AcceptChanges();
			ucBatchAndSerial1.CalculateOpenQty();
			double num = 0.0;
			double num2 = 0.0;
			for (int j = 0; j < dataSourceTable.Rows.Count; j++)
			{
				num += Function.ParseDouble(dataSourceTable.Rows[j]["LineTotal"]);
				num2 += Function.ParseDouble(dataSourceTable.Rows[j]["LineVat"]);
			}
			gv_Doc.SetFocusedRowCellValue("TotalBeforeDis", num);
			gv_Doc.SetFocusedRowCellValue("VatSum", num2);
			gv_Doc.SetFocusedRowCellValue("DocTotal", num2 + num);
			grd_Lines.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex.ToString());
		}
	}

	private void VirFormReceiptIssue_FormClosing(object sender, FormClosingEventArgs e)
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Interfaces.FormInventory.VirFormReceiptIssue));
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
		DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions4 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled6 = new DevExpress.Utils.SerializableAppearanceObject();
		this.repositoryItemTextEditExPrice1 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.repositoryItemTextEditExPrice2 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.repositoryItemCalcEditEx1 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.txtNo = new Apzon.Controls.Editor.TextEditEx();
		this.btnSearch = new Apzon.Controls.Controls.SimpleButtonEx();
		this.lblPri = new Apzon.Controls.Controls.LabelLine();
		this.lblNum = new Apzon.Controls.Controls.LabelLine();
		this.txttDate = new Apzon.Controls.Editor.DateEditEx();
		this.lbltdate = new Apzon.Controls.Controls.LabelLine();
		this.txtfDate = new Apzon.Controls.Editor.DateEditEx();
		this.lblfdate = new Apzon.Controls.Controls.LabelLine();
		this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
		this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
		this.barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
		this.barButtonItem2 = new DevExpress.XtraBars.BarButtonItem();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip_Document = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.docAddNewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.docRefreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.docCmtDuplicate = new System.Windows.Forms.ToolStripMenuItem();
		this.ctm_NewTask = new System.Windows.Forms.ToolStripMenuItem();
		this.ctm_NewEvent = new System.Windows.Forms.ToolStripMenuItem();
		this.ctm_NewNote = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
		this.docSaveAsDraftToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.gcDocNum = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDocCardCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDocDocumentStatus = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDocTaxDate = new Apzon.Controls.Grid.GridColumnEx();
		this.rDocDateEdit = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.gcDocDocDueDate = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDocDate = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDocComments = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDocTotalBeforeDis = new Apzon.Controls.Grid.GridColumnEx();
		this.rDocCal = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gcDocDiscPrcnt = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDocVatSum = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDocDocTotal = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDocCurrency = new Apzon.Controls.Grid.GridColumnEx();
		this.rDocCurrency = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcDocDocRate = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDocNumAtCard = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDocSeries = new Apzon.Controls.Grid.GridColumnEx();
		this.rDocSeries = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcDocCANCELED = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDocContactPersonCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rDocContactPerson = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcDocRounding = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDocRequiredDate = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDocRequester = new Apzon.Controls.Grid.GridColumnEx();
		this.rDocRequester = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcDocBranch = new Apzon.Controls.Grid.GridColumnEx();
		this.rDocBranch = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcDocDepartment = new Apzon.Controls.Grid.GridColumnEx();
		this.rDocGLU_Department = new DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit();
		this.gridView2 = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn7 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn10 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn11 = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDocGroupNum = new Apzon.Controls.Grid.GridColumnEx();
		this.rDocGroupNum = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcDocDocRef2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDocEntryDoc = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDoc_JournalMemo = new Apzon.Controls.Grid.GridColumnEx();
		this.rDocStatus = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.xtraTabControl1 = new Apzon.Controls.Controls.XtraTabControlEx();
		this.xtraTabPage1 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.splitContainerControl2 = new DevExpress.XtraEditors.SplitContainerControl();
		this.grd_Lines = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip_Line = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.lineDeleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.LineCmtDuplicate = new System.Windows.Forms.ToolStripMenuItem();
		this.batchSerialNumberToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Lines = new Apzon.Controls.Grid.GridViewEx();
		this.gcLineBaseRef = new Apzon.Controls.Grid.GridColumnEx();
		this.rLineBaseRef = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gc_Line_ItemCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rLineItemCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcLineItemDescription = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineQuantity = new Apzon.Controls.Grid.GridColumnEx();
		this.rLineCal = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gcLineAvgPrice = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_LinePrice = new Apzon.Controls.Grid.GridColumnEx();
		this.rPriceEdit = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.gcLinePrice = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_LinePQTReqdate = new Apzon.Controls.Grid.GridColumnEx();
		this.rLineDateEdit = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.gc_LineDiscountPercent = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_LineVatGroup = new Apzon.Controls.Grid.GridColumnEx();
		this.rLineTaxCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gc_LineTotal = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineWarehouseCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rLineWhs = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcLineFreeTxt = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineOpenQty = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineCurrency = new Apzon.Controls.Grid.GridColumnEx();
		this.rLineCurrency = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcLineRate = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineDocEntry = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineNum = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineSumLineTotalRate = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineBaseType = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineBaseEntry = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineBaseLine = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineAcctCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rLineAcctCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcLineStatus = new Apzon.Controls.Grid.GridColumnEx();
		this.ShipDate = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineUomGroup = new Apzon.Controls.Grid.GridColumnEx();
		this.rLineUomGroup = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcLineUomName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineInventoryUom = new Apzon.Controls.Grid.GridColumnEx();
		this.rLineInventoryUom = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcLineItemPerUnit = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineUomCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rLineUomCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcLineIvnvQty = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineOpenInvQty = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineQuantityOnStock = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Line_QuantityOnWareHouse = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_LineProject = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_LineProject = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gcTranType = new Apzon.Controls.Grid.GridColumnEx();
		this.rTranType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcLineSpecPrice = new Apzon.Controls.Grid.GridColumnEx();
		this.rLineSpecPrice = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.ucBatchAndSerial1 = new Apzon.Forms.UserControls.BatchAndSerial.ucBatchAndSerial();
		this.tab_Attachments = new Apzon.Controls.Controls.XtraTabPageEx();
		this.ucAttachment1 = new Apzon.Forms.UserControls.ucAttachment();
		this.xtraTabPage2 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.txtPriceList = new Apzon.Controls.Editor.LookUpEditEx();
		this.btn_Save = new Apzon.Controls.Controls.DropDownButtonEx();
		this.drlCopyTo = new Apzon.Controls.Controls.DropDownButtonEx();
		this.drlCopyFrom = new Apzon.Controls.Controls.DropDownButtonEx();
		this.gcBaseRef = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_RetCom = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtNo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txttDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txttDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtfDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtfDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.barManager1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		this.contextMenuStrip_Document.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rDocDateEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rDocDateEdit.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rDocCal).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rDocCurrency).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rDocSeries).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rDocContactPerson).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rDocRequester).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rDocBranch).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rDocGLU_Department).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridView2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rDocGroupNum).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rDocStatus).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).BeginInit();
		this.xtraTabControl1.SuspendLayout();
		this.xtraTabPage1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl2).BeginInit();
		this.splitContainerControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Lines).BeginInit();
		this.contextMenuStrip_Line.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Lines).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rLineBaseRef).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rLineItemCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rLineCal).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rPriceEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rLineDateEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rLineDateEdit.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rLineTaxCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rLineWhs).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rLineCurrency).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rLineAcctCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rLineUomGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rLineInventoryUom).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rLineUomCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_LineProject).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rTranType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rLineSpecPrice).BeginInit();
		this.tab_Attachments.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txtPriceList.Properties).BeginInit();
		base.SuspendLayout();
		this.repositoryItemTextEditExPrice1.Name = "repositoryItemTextEditExPrice1";
		this.repositoryItemTextEditExPrice2.Name = "repositoryItemTextEditExPrice2";
		this.repositoryItemCalcEditEx1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx1.IsSearchControl = false;
		this.repositoryItemCalcEditEx1.Name = "repositoryItemCalcEditEx1";
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Cancel.Appearance.Options.UseBackColor = true;
		this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = true;
		this.btn_Cancel.Location = new System.Drawing.Point(132, 586);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(80, 20);
		this.btn_Cancel.TabIndex = 8;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.txtNo.EditControl = null;
		this.txtNo.FieldName = null;
		this.txtNo.IsSearchControl = true;
		this.txtNo.Location = new System.Drawing.Point(288, 2);
		this.txtNo.Name = "txtNo";
		this.txtNo.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 6.5f);
		this.txtNo.Properties.Appearance.Options.UseFont = true;
		this.txtNo.SettingInVisible = false;
		this.txtNo.SettingReadonly = false;
		this.txtNo.Size = new System.Drawing.Size(100, 18);
		this.txtNo.TabIndex = 2;
		this.btnSearch.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btnSearch.Appearance.Options.UseBackColor = true;
		this.btnSearch.EditControl = null;
		this.btnSearch.EditValue = "Search";
		this.btnSearch.FieldName = null;
		this.btnSearch.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnSearch.ImageOptions.Image");
		this.btnSearch.IsSearchControl = true;
		this.btnSearch.Location = new System.Drawing.Point(394, 23);
		this.btnSearch.Name = "btnSearch";
		this.btnSearch.ReadOnly = false;
		this.btnSearch.SettingInVisible = false;
		this.btnSearch.SettingReadonly = false;
		this.btnSearch.Size = new System.Drawing.Size(90, 20);
		this.btnSearch.TabIndex = 5;
		this.btnSearch.Text = "Search";
		this.btnSearch.Click += new System.EventHandler(this.btSearch_Click);
		this.lblPri.EditControl = null;
		this.lblPri.EditValue = "Price List";
		this.lblPri.FieldName = null;
		this.lblPri.HideWhenEmpty = false;
		this.lblPri.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lblPri.Location = new System.Drawing.Point(191, 26);
		this.lblPri.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lblPri.Name = "lblPri";
		this.lblPri.ReadOnly = false;
		this.lblPri.SettingInVisible = false;
		this.lblPri.SettingReadonly = false;
		this.lblPri.ShowUnderLine = true;
		this.lblPri.Size = new System.Drawing.Size(98, 20);
		this.lblPri.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lblPri.TabIndex = 0;
		this.lblPri.Utext = "Price List";
		this.lblNum.EditControl = null;
		this.lblNum.EditValue = "Doc. Number";
		this.lblNum.FieldName = null;
		this.lblNum.HideWhenEmpty = false;
		this.lblNum.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lblNum.Location = new System.Drawing.Point(191, 2);
		this.lblNum.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lblNum.Name = "lblNum";
		this.lblNum.ReadOnly = false;
		this.lblNum.SettingInVisible = false;
		this.lblNum.SettingReadonly = false;
		this.lblNum.ShowUnderLine = true;
		this.lblNum.Size = new System.Drawing.Size(98, 20);
		this.lblNum.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lblNum.TabIndex = 0;
		this.lblNum.Utext = "Doc. Number";
		this.txttDate.EditControl = null;
		this.txttDate.EditValue = null;
		this.txttDate.FieldName = null;
		this.txttDate.Location = new System.Drawing.Point(83, 25);
		this.txttDate.Name = "txttDate";
		this.txttDate.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 6.5f);
		this.txttDate.Properties.Appearance.Options.UseFont = true;
		this.txttDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txttDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txttDate.Properties.IsSearchControl = true;
		this.txttDate.Properties.Mask.EditMask = "";
		this.txttDate.SettingInVisible = false;
		this.txttDate.SettingReadonly = false;
		this.txttDate.Size = new System.Drawing.Size(100, 18);
		this.txttDate.TabIndex = 3;
		this.lbltdate.EditControl = null;
		this.lbltdate.EditValue = "To Date";
		this.lbltdate.FieldName = null;
		this.lbltdate.HideWhenEmpty = false;
		this.lbltdate.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbltdate.Location = new System.Drawing.Point(3, 26);
		this.lbltdate.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lbltdate.Name = "lbltdate";
		this.lbltdate.ReadOnly = false;
		this.lbltdate.SettingInVisible = false;
		this.lbltdate.SettingReadonly = false;
		this.lbltdate.ShowUnderLine = true;
		this.lbltdate.Size = new System.Drawing.Size(80, 20);
		this.lbltdate.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbltdate.TabIndex = 0;
		this.lbltdate.Utext = "To Date";
		this.txtfDate.EditControl = null;
		this.txtfDate.EditValue = null;
		this.txtfDate.FieldName = null;
		this.txtfDate.Location = new System.Drawing.Point(83, 1);
		this.txtfDate.Name = "txtfDate";
		this.txtfDate.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 6.5f);
		this.txtfDate.Properties.Appearance.Options.UseFont = true;
		this.txtfDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtfDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtfDate.Properties.IsSearchControl = true;
		this.txtfDate.Properties.Mask.EditMask = "";
		this.txtfDate.SettingInVisible = false;
		this.txtfDate.SettingReadonly = false;
		this.txtfDate.Size = new System.Drawing.Size(100, 18);
		this.txtfDate.TabIndex = 1;
		this.lblfdate.EditControl = null;
		this.lblfdate.EditValue = "From Date";
		this.lblfdate.FieldName = null;
		this.lblfdate.HideWhenEmpty = false;
		this.lblfdate.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lblfdate.Location = new System.Drawing.Point(3, 2);
		this.lblfdate.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lblfdate.Name = "lblfdate";
		this.lblfdate.ReadOnly = false;
		this.lblfdate.SettingInVisible = false;
		this.lblfdate.SettingReadonly = false;
		this.lblfdate.ShowUnderLine = true;
		this.lblfdate.Size = new System.Drawing.Size(80, 20);
		this.lblfdate.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lblfdate.TabIndex = 0;
		this.lblfdate.Utext = "From Date";
		this.barManager1.DockControls.Add(this.barDockControlTop);
		this.barManager1.DockControls.Add(this.barDockControlBottom);
		this.barManager1.DockControls.Add(this.barDockControlLeft);
		this.barManager1.DockControls.Add(this.barDockControlRight);
		this.barManager1.Form = this;
		this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[2] { this.barButtonItem1, this.barButtonItem2 });
		this.barManager1.MaxItemId = 5;
		this.barDockControlTop.CausesValidation = false;
		this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
		this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
		this.barDockControlTop.Manager = this.barManager1;
		this.barDockControlTop.Size = new System.Drawing.Size(1015, 0);
		this.barDockControlBottom.CausesValidation = false;
		this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.barDockControlBottom.Location = new System.Drawing.Point(0, 537);
		this.barDockControlBottom.Manager = this.barManager1;
		this.barDockControlBottom.Size = new System.Drawing.Size(1015, 0);
		this.barDockControlLeft.CausesValidation = false;
		this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
		this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
		this.barDockControlLeft.Manager = this.barManager1;
		this.barDockControlLeft.Size = new System.Drawing.Size(0, 537);
		this.barDockControlRight.CausesValidation = false;
		this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
		this.barDockControlRight.Location = new System.Drawing.Point(1015, 0);
		this.barDockControlRight.Manager = this.barManager1;
		this.barDockControlRight.Size = new System.Drawing.Size(0, 537);
		this.barButtonItem1.Id = 3;
		this.barButtonItem1.Name = "barButtonItem1";
		this.barButtonItem2.Id = 4;
		this.barButtonItem2.Name = "barButtonItem2";
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(4, 49);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.grd_Doc);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.xtraTabControl1);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(1011, 456);
		this.splitContainerControl1.SplitterPosition = 176;
		this.splitContainerControl1.TabIndex = 33;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.ContextMenuStrip = this.contextMenuStrip_Document;
		this.grd_Doc.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Doc.Location = new System.Drawing.Point(0, 0);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[10] { this.rDocDateEdit, this.rDocCal, this.rDocRequester, this.rDocGLU_Department, this.rDocBranch, this.rDocCurrency, this.rDocGroupNum, this.rDocSeries, this.rDocContactPerson, this.rDocStatus });
		this.grd_Doc.Size = new System.Drawing.Size(1011, 176);
		this.grd_Doc.TabIndex = 0;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.contextMenuStrip_Document.ImageScalingSize = new System.Drawing.Size(20, 20);
		this.contextMenuStrip_Document.Items.AddRange(new System.Windows.Forms.ToolStripItem[8] { this.docAddNewToolStripMenuItem, this.docRefreshToolStripMenuItem, this.docCmtDuplicate, this.ctm_NewTask, this.ctm_NewEvent, this.ctm_NewNote, this.toolStripSeparator1, this.docSaveAsDraftToolStripMenuItem });
		this.contextMenuStrip_Document.Name = "contextMenuStrip_Document";
		this.contextMenuStrip_Document.Size = new System.Drawing.Size(144, 164);
		this.docAddNewToolStripMenuItem.Name = "docAddNewToolStripMenuItem";
		this.docAddNewToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
		this.docAddNewToolStripMenuItem.Text = "Add New";
		this.docAddNewToolStripMenuItem.Click += new System.EventHandler(this.addNewToolStripMenuItem_Document_Click);
		this.docRefreshToolStripMenuItem.Name = "docRefreshToolStripMenuItem";
		this.docRefreshToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
		this.docRefreshToolStripMenuItem.Text = "Refresh";
		this.docRefreshToolStripMenuItem.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
		this.docCmtDuplicate.Name = "docCmtDuplicate";
		this.docCmtDuplicate.Size = new System.Drawing.Size(143, 22);
		this.docCmtDuplicate.Text = "Duplicate";
		this.docCmtDuplicate.Click += new System.EventHandler(this.cmtDuplicate_Return_Click);
		this.ctm_NewTask.Name = "ctm_NewTask";
		this.ctm_NewTask.Size = new System.Drawing.Size(143, 22);
		this.ctm_NewTask.Text = "New Task";
		this.ctm_NewTask.Click += new System.EventHandler(this.newActivityToolStripMenuItem_Click);
		this.ctm_NewEvent.Name = "ctm_NewEvent";
		this.ctm_NewEvent.Size = new System.Drawing.Size(143, 22);
		this.ctm_NewEvent.Text = "New Event";
		this.ctm_NewNote.Name = "ctm_NewNote";
		this.ctm_NewNote.Size = new System.Drawing.Size(143, 22);
		this.ctm_NewNote.Text = "New Note";
		this.toolStripSeparator1.Name = "toolStripSeparator1";
		this.toolStripSeparator1.Size = new System.Drawing.Size(140, 6);
		this.docSaveAsDraftToolStripMenuItem.Name = "docSaveAsDraftToolStripMenuItem";
		this.docSaveAsDraftToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
		this.docSaveAsDraftToolStripMenuItem.Text = "Save As Draft";
		this.docSaveAsDraftToolStripMenuItem.Click += new System.EventHandler(this.saveAsDraftToolStripMenuItem_Click);
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = null;
		this.gv_Doc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[26]
		{
			this.gcDocNum, this.gcDocCardCode, this.gcDocDocumentStatus, this.gcDocTaxDate, this.gcDocDocDueDate, this.gcDocDate, this.gcDocComments, this.gcDocTotalBeforeDis, this.gcDocDiscPrcnt, this.gcDocVatSum,
			this.gcDocDocTotal, this.gcDocCurrency, this.gcDocDocRate, this.gcDocNumAtCard, this.gcDocSeries, this.gcDocCANCELED, this.gcDocContactPersonCode, this.gcDocRounding, this.gcDocRequiredDate, this.gcDocRequester,
			this.gcDocBranch, this.gcDocDepartment, this.gcDocGroupNum, this.gcDocDocRef2, this.gcDocEntryDoc, this.gcDoc_JournalMemo
		});
		this.gv_Doc.DetailTabHeaderLocation = DevExpress.XtraTab.TabHeaderLocation.Bottom;
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = "DocCur";
		this.gv_Doc.FieldRate = "DocRate";
		this.gv_Doc.FormId = "oReceiptIssue";
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.HasFormSetting = false;
		this.gv_Doc.IgnoreFilter = false;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 40;
		this.gv_Doc.IsApzTable = false;
		this.gv_Doc.IsInsertNewRow = false;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.IsMappingContextAction = false;
		this.gv_Doc.ItemId = "OIGN";
		this.gv_Doc.LstColumnAllowBatch = null;
		this.gv_Doc.LstColumnNotAllowBatch = null;
		this.gv_Doc.LstColumnNotEditable = null;
		this.gv_Doc.LstColumNotSetting = null;
		this.gv_Doc.LstOldUDF = null;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsDetail.EnableMasterViewMode = false;
		this.gv_Doc.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.gv_Doc.OptionsSelection.EnableAppearanceFocusedRow = false;
		this.gv_Doc.OptionsSelection.EnableAppearanceHideSelection = false;
		this.gv_Doc.OptionsSelection.MultiSelect = true;
		this.gv_Doc.OptionsView.ColumnAutoWidth = false;
		this.gv_Doc.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Doc.OptionsView.ShowGroupPanel = false;
		this.gv_Doc.UseBatchMod = false;
		this.gv_Doc.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Doc_RowCellStyle);
		this.gv_Doc.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.gv_Doc_RowStyle);
		this.gv_Doc.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_Doc_ShowingEditor);
		this.gv_Doc.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_Doc_FocusedRowObjectChanged);
		this.gv_Doc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Doc_CellValueChanged);
		this.gv_Doc.BeforeLeaveRow += new DevExpress.XtraGrid.Views.Base.RowAllowEventHandler(this.gv_Doc_BeforeLeaveRow);
		this.gcDocNum.Caption = "Doc Num";
		this.gcDocNum.CurrencyDateField = null;
		this.gcDocNum.CurrencyField = null;
		this.gcDocNum.CurrencyFixed = null;
		this.gcDocNum.CurrencyFixedRate = 0.0;
		this.gcDocNum.CurrencyRateField = null;
		this.gcDocNum.FieldName = "DocNum";
		this.gcDocNum.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gcDocNum.Name = "gcDocNum";
		this.gcDocNum.OptionsColumn.ReadOnly = true;
		this.gcDocNum.PrimaryKey = false;
		this.gcDocNum.UnitField = null;
		this.gcDocNum.Visible = true;
		this.gcDocNum.VisibleIndex = 0;
		this.gcDocNum.Width = 100;
		this.gcDocCardCode.Caption = "Card Code";
		this.gcDocCardCode.CurrencyDateField = null;
		this.gcDocCardCode.CurrencyField = null;
		this.gcDocCardCode.CurrencyFixed = null;
		this.gcDocCardCode.CurrencyFixedRate = 0.0;
		this.gcDocCardCode.CurrencyRateField = null;
		this.gcDocCardCode.FieldName = "CardCode";
		this.gcDocCardCode.Name = "gcDocCardCode";
		this.gcDocCardCode.PrimaryKey = false;
		this.gcDocCardCode.UnitField = null;
		this.gcDocCardCode.Width = 100;
		this.gcDocDocumentStatus.Caption = "Status";
		this.gcDocDocumentStatus.ColumnEdit = this.rDocStatus;
		this.gcDocDocumentStatus.CurrencyDateField = null;
		this.gcDocDocumentStatus.CurrencyField = null;
		this.gcDocDocumentStatus.CurrencyFixed = null;
		this.gcDocDocumentStatus.CurrencyFixedRate = 0.0;
		this.gcDocDocumentStatus.CurrencyRateField = null;
		this.gcDocDocumentStatus.FieldName = "DocStatus";
		this.gcDocDocumentStatus.Name = "gcDocDocumentStatus";
		this.gcDocDocumentStatus.OptionsColumn.ReadOnly = true;
		this.gcDocDocumentStatus.PrimaryKey = false;
		this.gcDocDocumentStatus.UnitField = null;
		this.gcDocDocumentStatus.Width = 100;
		this.gcDocTaxDate.Caption = "Document Date";
		this.gcDocTaxDate.CurrencyDateField = null;
		this.gcDocTaxDate.CurrencyField = null;
		this.gcDocTaxDate.CurrencyFixed = null;
		this.gcDocTaxDate.CurrencyFixedRate = 0.0;
		this.gcDocTaxDate.CurrencyRateField = null;
		this.gcDocTaxDate.DisplayFormat.FormatString = "d";
		this.gcDocTaxDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.gcDocTaxDate.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gcDocTaxDate.FieldName = "TaxDate";
		this.gcDocTaxDate.Name = "gcDocTaxDate";
		this.gcDocTaxDate.PrimaryKey = false;
		this.gcDocTaxDate.UnitField = null;
		this.gcDocTaxDate.Visible = true;
		this.gcDocTaxDate.VisibleIndex = 2;
		this.gcDocTaxDate.Width = 100;
		this.rDocDateEdit.AutoHeight = false;
		this.rDocDateEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rDocDateEdit.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rDocDateEdit.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.rDocDateEdit.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.rDocDateEdit.Mask.EditMask = "dd/MM/yyyy";
		this.rDocDateEdit.Name = "rDocDateEdit";
		this.gcDocDocDueDate.Caption = "Valid Until";
		this.gcDocDocDueDate.CurrencyDateField = null;
		this.gcDocDocDueDate.CurrencyField = null;
		this.gcDocDocDueDate.CurrencyFixed = null;
		this.gcDocDocDueDate.CurrencyFixedRate = 0.0;
		this.gcDocDocDueDate.CurrencyRateField = null;
		this.gcDocDocDueDate.DisplayFormat.FormatString = "d";
		this.gcDocDocDueDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.gcDocDocDueDate.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gcDocDocDueDate.FieldName = "DocDueDate";
		this.gcDocDocDueDate.Name = "gcDocDocDueDate";
		this.gcDocDocDueDate.PrimaryKey = false;
		this.gcDocDocDueDate.UnitField = null;
		this.gcDocDocDueDate.Width = 100;
		this.gcDocDate.Caption = "PostingDate";
		this.gcDocDate.CurrencyDateField = null;
		this.gcDocDate.CurrencyField = null;
		this.gcDocDate.CurrencyFixed = null;
		this.gcDocDate.CurrencyFixedRate = 0.0;
		this.gcDocDate.CurrencyRateField = null;
		this.gcDocDate.DisplayFormat.FormatString = "d";
		this.gcDocDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.gcDocDate.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gcDocDate.FieldName = "DocDate";
		this.gcDocDate.Name = "gcDocDate";
		this.gcDocDate.PrimaryKey = false;
		this.gcDocDate.UnitField = null;
		this.gcDocDate.Visible = true;
		this.gcDocDate.VisibleIndex = 3;
		this.gcDocDate.Width = 100;
		this.gcDocComments.Caption = "Remark";
		this.gcDocComments.CurrencyDateField = null;
		this.gcDocComments.CurrencyField = null;
		this.gcDocComments.CurrencyFixed = null;
		this.gcDocComments.CurrencyFixedRate = 0.0;
		this.gcDocComments.CurrencyRateField = null;
		this.gcDocComments.FieldName = "Comments";
		this.gcDocComments.Name = "gcDocComments";
		this.gcDocComments.PrimaryKey = false;
		this.gcDocComments.UnitField = null;
		this.gcDocComments.Visible = true;
		this.gcDocComments.VisibleIndex = 4;
		this.gcDocComments.Width = 100;
		this.gcDocTotalBeforeDis.Caption = "Total Before Dis";
		this.gcDocTotalBeforeDis.ColumnEdit = this.rDocCal;
		this.gcDocTotalBeforeDis.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gcDocTotalBeforeDis.CurrencyDateField = null;
		this.gcDocTotalBeforeDis.CurrencyField = "DocCur";
		this.gcDocTotalBeforeDis.CurrencyFixed = null;
		this.gcDocTotalBeforeDis.CurrencyFixedRate = 0.0;
		this.gcDocTotalBeforeDis.CurrencyRateField = null;
		this.gcDocTotalBeforeDis.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gcDocTotalBeforeDis.FieldName = "TotalBeforeDis";
		this.gcDocTotalBeforeDis.Name = "gcDocTotalBeforeDis";
		this.gcDocTotalBeforeDis.OptionsColumn.ReadOnly = true;
		this.gcDocTotalBeforeDis.PrimaryKey = false;
		this.gcDocTotalBeforeDis.UnitField = null;
		this.gcDocTotalBeforeDis.Width = 100;
		this.rDocCal.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Down)
		});
		this.rDocCal.Name = "rDocCal";
		this.gcDocDiscPrcnt.Caption = "Discount";
		this.gcDocDiscPrcnt.ColumnEdit = this.rDocCal;
		this.gcDocDiscPrcnt.CurrencyDateField = null;
		this.gcDocDiscPrcnt.CurrencyField = null;
		this.gcDocDiscPrcnt.CurrencyFixed = null;
		this.gcDocDiscPrcnt.CurrencyFixedRate = 0.0;
		this.gcDocDiscPrcnt.CurrencyRateField = null;
		this.gcDocDiscPrcnt.DisplayFormat.FormatString = "P0";
		this.gcDocDiscPrcnt.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gcDocDiscPrcnt.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Percent;
		this.gcDocDiscPrcnt.FieldName = "DiscPrcnt";
		this.gcDocDiscPrcnt.Name = "gcDocDiscPrcnt";
		this.gcDocDiscPrcnt.PrimaryKey = false;
		this.gcDocDiscPrcnt.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gcDocDiscPrcnt.UnitField = null;
		this.gcDocDiscPrcnt.Width = 100;
		this.gcDocVatSum.Caption = "Total Vat";
		this.gcDocVatSum.ColumnEdit = this.rDocCal;
		this.gcDocVatSum.CurrencyDateField = null;
		this.gcDocVatSum.CurrencyField = null;
		this.gcDocVatSum.CurrencyFixed = null;
		this.gcDocVatSum.CurrencyFixedRate = 0.0;
		this.gcDocVatSum.CurrencyRateField = null;
		this.gcDocVatSum.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gcDocVatSum.FieldName = "VatSum";
		this.gcDocVatSum.Name = "gcDocVatSum";
		this.gcDocVatSum.OptionsColumn.ReadOnly = true;
		this.gcDocVatSum.PrimaryKey = false;
		this.gcDocVatSum.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gcDocVatSum.UnitField = null;
		this.gcDocVatSum.Width = 100;
		this.gcDocDocTotal.Caption = "Total ";
		this.gcDocDocTotal.ColumnEdit = this.rDocCal;
		this.gcDocDocTotal.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gcDocDocTotal.CurrencyDateField = null;
		this.gcDocDocTotal.CurrencyField = "DocCur";
		this.gcDocDocTotal.CurrencyFixed = null;
		this.gcDocDocTotal.CurrencyFixedRate = 0.0;
		this.gcDocDocTotal.CurrencyRateField = null;
		this.gcDocDocTotal.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gcDocDocTotal.FieldName = "DocTotal";
		this.gcDocDocTotal.Name = "gcDocDocTotal";
		this.gcDocDocTotal.OptionsColumn.ReadOnly = true;
		this.gcDocDocTotal.PrimaryKey = false;
		this.gcDocDocTotal.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gcDocDocTotal.UnitField = null;
		this.gcDocDocTotal.Visible = true;
		this.gcDocDocTotal.VisibleIndex = 5;
		this.gcDocDocTotal.Width = 100;
		this.gcDocCurrency.Caption = "Currency";
		this.gcDocCurrency.ColumnEdit = this.rDocCurrency;
		this.gcDocCurrency.CurrencyDateField = null;
		this.gcDocCurrency.CurrencyField = null;
		this.gcDocCurrency.CurrencyFixed = null;
		this.gcDocCurrency.CurrencyFixedRate = 0.0;
		this.gcDocCurrency.CurrencyRateField = null;
		this.gcDocCurrency.FieldName = "DocCur";
		this.gcDocCurrency.Name = "gcDocCurrency";
		this.gcDocCurrency.OptionsColumn.AllowGroup = DevExpress.Utils.DefaultBoolean.False;
		this.gcDocCurrency.OptionsColumn.ReadOnly = true;
		this.gcDocCurrency.PrimaryKey = false;
		this.gcDocCurrency.UnitField = null;
		this.gcDocCurrency.Width = 100;
		this.rDocCurrency.AutoHeight = false;
		this.rDocCurrency.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rDocCurrency.DisplayMember = "Code";
		this.rDocCurrency.Name = "rDocCurrency";
		this.rDocCurrency.ValueMember = "Code";
		this.gcDocDocRate.Caption = "Rate";
		this.gcDocDocRate.ColumnEdit = this.rDocCal;
		this.gcDocDocRate.CurrencyDateField = null;
		this.gcDocDocRate.CurrencyField = null;
		this.gcDocDocRate.CurrencyFixed = null;
		this.gcDocDocRate.CurrencyFixedRate = 0.0;
		this.gcDocDocRate.CurrencyRateField = null;
		this.gcDocDocRate.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Rate;
		this.gcDocDocRate.FieldName = "DocRate";
		this.gcDocDocRate.Name = "gcDocDocRate";
		this.gcDocDocRate.PrimaryKey = false;
		this.gcDocDocRate.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gcDocDocRate.UnitField = null;
		this.gcDocDocRate.Width = 100;
		this.gcDocNumAtCard.Caption = "Vendor Ref. No";
		this.gcDocNumAtCard.CurrencyDateField = null;
		this.gcDocNumAtCard.CurrencyField = null;
		this.gcDocNumAtCard.CurrencyFixed = null;
		this.gcDocNumAtCard.CurrencyFixedRate = 0.0;
		this.gcDocNumAtCard.CurrencyRateField = null;
		this.gcDocNumAtCard.FieldName = "NumAtCard";
		this.gcDocNumAtCard.Name = "gcDocNumAtCard";
		this.gcDocNumAtCard.PrimaryKey = false;
		this.gcDocNumAtCard.UnitField = null;
		this.gcDocNumAtCard.Width = 100;
		this.gcDocSeries.Caption = "Series";
		this.gcDocSeries.ColumnEdit = this.rDocSeries;
		this.gcDocSeries.CurrencyDateField = null;
		this.gcDocSeries.CurrencyField = null;
		this.gcDocSeries.CurrencyFixed = null;
		this.gcDocSeries.CurrencyFixedRate = 0.0;
		this.gcDocSeries.CurrencyRateField = null;
		this.gcDocSeries.FieldName = "Series";
		this.gcDocSeries.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gcDocSeries.Name = "gcDocSeries";
		this.gcDocSeries.PrimaryKey = false;
		this.gcDocSeries.UnitField = null;
		this.gcDocSeries.Visible = true;
		this.gcDocSeries.VisibleIndex = 1;
		this.gcDocSeries.Width = 100;
		this.rDocSeries.AutoHeight = false;
		this.rDocSeries.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rDocSeries.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[4]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("SeriesName", "Name3"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Series", "Name4", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("BPLId", "Name9", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("IsDefault", "Name10", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rDocSeries.DisplayMember = "SeriesName";
		this.rDocSeries.Name = "rDocSeries";
		this.rDocSeries.NullText = "";
		this.rDocSeries.ShowFooter = false;
		this.rDocSeries.ShowHeader = false;
		this.rDocSeries.ValueMember = "Series";
		this.gcDocCANCELED.Caption = "CANCELED";
		this.gcDocCANCELED.CurrencyDateField = null;
		this.gcDocCANCELED.CurrencyField = null;
		this.gcDocCANCELED.CurrencyFixed = null;
		this.gcDocCANCELED.CurrencyFixedRate = 0.0;
		this.gcDocCANCELED.CurrencyRateField = null;
		this.gcDocCANCELED.FieldName = "Cancelled";
		this.gcDocCANCELED.Name = "gcDocCANCELED";
		this.gcDocCANCELED.PrimaryKey = false;
		this.gcDocCANCELED.UnitField = null;
		this.gcDocCANCELED.Width = 100;
		this.gcDocContactPersonCode.Caption = "Contact Person";
		this.gcDocContactPersonCode.ColumnEdit = this.rDocContactPerson;
		this.gcDocContactPersonCode.CurrencyDateField = null;
		this.gcDocContactPersonCode.CurrencyField = null;
		this.gcDocContactPersonCode.CurrencyFixed = null;
		this.gcDocContactPersonCode.CurrencyFixedRate = 0.0;
		this.gcDocContactPersonCode.CurrencyRateField = null;
		this.gcDocContactPersonCode.FieldName = "CntctCode";
		this.gcDocContactPersonCode.Name = "gcDocContactPersonCode";
		this.gcDocContactPersonCode.PrimaryKey = false;
		this.gcDocContactPersonCode.UnitField = null;
		this.gcDocContactPersonCode.Width = 100;
		this.rDocContactPerson.AutoHeight = false;
		this.rDocContactPerson.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rDocContactPerson.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name5"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Value", "Name6", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rDocContactPerson.DisplayMember = "Name";
		this.rDocContactPerson.Name = "rDocContactPerson";
		this.rDocContactPerson.ShowFooter = false;
		this.rDocContactPerson.ShowHeader = false;
		this.rDocContactPerson.ValueMember = "Value";
		this.gcDocRounding.Caption = "Rounding";
		this.gcDocRounding.CurrencyDateField = null;
		this.gcDocRounding.CurrencyField = null;
		this.gcDocRounding.CurrencyFixed = null;
		this.gcDocRounding.CurrencyFixedRate = 0.0;
		this.gcDocRounding.CurrencyRateField = null;
		this.gcDocRounding.FieldName = "Rounding";
		this.gcDocRounding.Name = "gcDocRounding";
		this.gcDocRounding.PrimaryKey = false;
		this.gcDocRounding.UnitField = null;
		this.gcDocRounding.Width = 100;
		this.gcDocRequiredDate.Caption = "RequiredDate";
		this.gcDocRequiredDate.CurrencyDateField = null;
		this.gcDocRequiredDate.CurrencyField = null;
		this.gcDocRequiredDate.CurrencyFixed = null;
		this.gcDocRequiredDate.CurrencyFixedRate = 0.0;
		this.gcDocRequiredDate.CurrencyRateField = null;
		this.gcDocRequiredDate.DisplayFormat.FormatString = "d";
		this.gcDocRequiredDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.gcDocRequiredDate.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gcDocRequiredDate.FieldName = "ReqDate";
		this.gcDocRequiredDate.Name = "gcDocRequiredDate";
		this.gcDocRequiredDate.PrimaryKey = false;
		this.gcDocRequiredDate.UnitField = null;
		this.gcDocRequiredDate.Width = 100;
		this.gcDocRequester.Caption = "Requester";
		this.gcDocRequester.ColumnEdit = this.rDocRequester;
		this.gcDocRequester.CurrencyDateField = null;
		this.gcDocRequester.CurrencyField = null;
		this.gcDocRequester.CurrencyFixed = null;
		this.gcDocRequester.CurrencyFixedRate = 0.0;
		this.gcDocRequester.CurrencyRateField = null;
		this.gcDocRequester.FieldName = "Requester";
		this.gcDocRequester.Name = "gcDocRequester";
		this.gcDocRequester.PrimaryKey = false;
		this.gcDocRequester.UnitField = null;
		this.gcDocRequester.Width = 100;
		this.rDocRequester.AutoHeight = false;
		this.rDocRequester.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.rDocRequester.Name = "rDocRequester";
		this.rDocRequester.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rpRequester_ButtonClick);
		this.gcDocBranch.Caption = "Branch";
		this.gcDocBranch.ColumnEdit = this.rDocBranch;
		this.gcDocBranch.CurrencyDateField = null;
		this.gcDocBranch.CurrencyField = null;
		this.gcDocBranch.CurrencyFixed = null;
		this.gcDocBranch.CurrencyFixedRate = 0.0;
		this.gcDocBranch.CurrencyRateField = null;
		this.gcDocBranch.FieldName = "Branch";
		this.gcDocBranch.Name = "gcDocBranch";
		this.gcDocBranch.PrimaryKey = false;
		this.gcDocBranch.UnitField = null;
		this.gcDocBranch.Width = 100;
		this.rDocBranch.AutoHeight = false;
		this.rDocBranch.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rDocBranch.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("BPLId", "BPLId"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("BPLName", "BPLName")
		});
		this.rDocBranch.DisplayMember = "BPLName";
		this.rDocBranch.Name = "rDocBranch";
		this.rDocBranch.NullText = "";
		this.rDocBranch.ValueMember = "BPLId";
		this.gcDocDepartment.Caption = "Department";
		this.gcDocDepartment.ColumnEdit = this.rDocGLU_Department;
		this.gcDocDepartment.CurrencyDateField = null;
		this.gcDocDepartment.CurrencyField = null;
		this.gcDocDepartment.CurrencyFixed = null;
		this.gcDocDepartment.CurrencyFixedRate = 0.0;
		this.gcDocDepartment.CurrencyRateField = null;
		this.gcDocDepartment.FieldName = "Department";
		this.gcDocDepartment.Name = "gcDocDepartment";
		this.gcDocDepartment.PrimaryKey = false;
		this.gcDocDepartment.UnitField = null;
		this.gcDocDepartment.Width = 100;
		this.rDocGLU_Department.AutoHeight = false;
		this.rDocGLU_Department.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rDocGLU_Department.DisplayMember = "Name";
		this.rDocGLU_Department.Name = "rDocGLU_Department";
		this.rDocGLU_Department.NullText = "";
		this.rDocGLU_Department.PopupView = this.gridView2;
		this.rDocGLU_Department.ValueMember = "Code";
		this.rDocGLU_Department.Popup += new System.EventHandler(this.repGLU_Department_Popup);
		this.gridView2.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gridView2.ColumnKey = null;
		this.gridView2.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.gridColumn7, this.gridColumn10, this.gridColumn11 });
		this.gridView2.DisableFormSetting = false;
		this.gridView2.DisplayException = false;
		this.gridView2.EditColumn = null;
		this.gridView2.FieldCurrency = null;
		this.gridView2.FieldRate = null;
		this.gridView2.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
		this.gridView2.FormId = "oReceiptIssue";
		this.gridView2.HasFormSetting = false;
		this.gridView2.IgnoreFilter = false;
		this.gridView2.IndexEditRow = -1;
		this.gridView2.IndicatorWidth = 35;
		this.gridView2.IsApzTable = false;
		this.gridView2.IsInsertNewRow = false;
		this.gridView2.IsLoadingData = false;
		this.gridView2.IsMappingContextAction = false;
		this.gridView2.ItemId = "OGRID2";
		this.gridView2.LstColumnAllowBatch = null;
		this.gridView2.LstColumnNotAllowBatch = null;
		this.gridView2.LstColumnNotEditable = null;
		this.gridView2.LstColumNotSetting = null;
		this.gridView2.LstOldUDF = null;
		this.gridView2.Name = "gridView2";
		this.gridView2.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.gridView2.OptionsView.ShowGroupPanel = false;
		this.gridView2.UseBatchMod = false;
		this.gridColumn7.Caption = "Department Code";
		this.gridColumn7.CurrencyDateField = null;
		this.gridColumn7.CurrencyField = null;
		this.gridColumn7.CurrencyFixed = null;
		this.gridColumn7.CurrencyFixedRate = 0.0;
		this.gridColumn7.CurrencyRateField = null;
		this.gridColumn7.FieldName = "Code";
		this.gridColumn7.Name = "gridColumn7";
		this.gridColumn7.PrimaryKey = false;
		this.gridColumn7.UnitField = null;
		this.gridColumn7.Visible = true;
		this.gridColumn7.VisibleIndex = 0;
		this.gridColumn10.Caption = "Department Name";
		this.gridColumn10.CurrencyDateField = null;
		this.gridColumn10.CurrencyField = null;
		this.gridColumn10.CurrencyFixed = null;
		this.gridColumn10.CurrencyFixedRate = 0.0;
		this.gridColumn10.CurrencyRateField = null;
		this.gridColumn10.FieldName = "Name";
		this.gridColumn10.Name = "gridColumn10";
		this.gridColumn10.PrimaryKey = false;
		this.gridColumn10.UnitField = null;
		this.gridColumn10.Visible = true;
		this.gridColumn10.VisibleIndex = 1;
		this.gridColumn11.Caption = "Requester";
		this.gridColumn11.CurrencyDateField = null;
		this.gridColumn11.CurrencyField = null;
		this.gridColumn11.CurrencyFixed = null;
		this.gridColumn11.CurrencyFixedRate = 0.0;
		this.gridColumn11.CurrencyRateField = null;
		this.gridColumn11.FieldName = "Requester";
		this.gridColumn11.Name = "gridColumn11";
		this.gridColumn11.PrimaryKey = false;
		this.gridColumn11.UnitField = null;
		this.gcDocGroupNum.Caption = "Price list";
		this.gcDocGroupNum.ColumnEdit = this.rDocGroupNum;
		this.gcDocGroupNum.CurrencyDateField = null;
		this.gcDocGroupNum.CurrencyField = null;
		this.gcDocGroupNum.CurrencyFixed = null;
		this.gcDocGroupNum.CurrencyFixedRate = 0.0;
		this.gcDocGroupNum.CurrencyRateField = null;
		this.gcDocGroupNum.FieldName = "GroupNum";
		this.gcDocGroupNum.Name = "gcDocGroupNum";
		this.gcDocGroupNum.PrimaryKey = false;
		this.gcDocGroupNum.UnitField = null;
		this.gcDocGroupNum.Visible = true;
		this.gcDocGroupNum.VisibleIndex = 6;
		this.gcDocGroupNum.Width = 100;
		this.rDocGroupNum.AutoHeight = false;
		this.rDocGroupNum.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rDocGroupNum.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rDocGroupNum.DisplayMember = "Name";
		this.rDocGroupNum.Name = "rDocGroupNum";
		this.rDocGroupNum.NullText = "";
		this.rDocGroupNum.ShowFooter = false;
		this.rDocGroupNum.ShowHeader = false;
		this.rDocGroupNum.ValueMember = "Code";
		this.rDocGroupNum.EditValueChanged += new System.EventHandler(this.rGroupNum_EditValueChanged);
		this.gcDocDocRef2.Caption = "Ref.2";
		this.gcDocDocRef2.CurrencyDateField = null;
		this.gcDocDocRef2.CurrencyField = null;
		this.gcDocDocRef2.CurrencyFixed = null;
		this.gcDocDocRef2.CurrencyFixedRate = 0.0;
		this.gcDocDocRef2.CurrencyRateField = null;
		this.gcDocDocRef2.FieldName = "Ref2";
		this.gcDocDocRef2.Name = "gcDocDocRef2";
		this.gcDocDocRef2.PrimaryKey = false;
		this.gcDocDocRef2.UnitField = null;
		this.gcDocDocRef2.Visible = true;
		this.gcDocDocRef2.VisibleIndex = 7;
		this.gcDocDocRef2.Width = 100;
		this.gcDocEntryDoc.Caption = "DocEntry";
		this.gcDocEntryDoc.CurrencyDateField = null;
		this.gcDocEntryDoc.CurrencyField = null;
		this.gcDocEntryDoc.CurrencyFixed = null;
		this.gcDocEntryDoc.CurrencyFixedRate = 0.0;
		this.gcDocEntryDoc.CurrencyRateField = null;
		this.gcDocEntryDoc.FieldName = "DocEntry";
		this.gcDocEntryDoc.Name = "gcDocEntryDoc";
		this.gcDocEntryDoc.PrimaryKey = false;
		this.gcDocEntryDoc.UnitField = null;
		this.gcDocEntryDoc.Width = 100;
		this.gcDoc_JournalMemo.Caption = "Journal Remarks";
		this.gcDoc_JournalMemo.CurrencyDateField = null;
		this.gcDoc_JournalMemo.CurrencyField = null;
		this.gcDoc_JournalMemo.CurrencyFixed = null;
		this.gcDoc_JournalMemo.CurrencyFixedRate = 0.0;
		this.gcDoc_JournalMemo.CurrencyRateField = null;
		this.gcDoc_JournalMemo.FieldName = "JrnlMemo";
		this.gcDoc_JournalMemo.Name = "gcDoc_JournalMemo";
		this.gcDoc_JournalMemo.PrimaryKey = false;
		this.gcDoc_JournalMemo.UnitField = null;
		this.gcDoc_JournalMemo.Visible = true;
		this.gcDoc_JournalMemo.VisibleIndex = 8;
		this.gcDoc_JournalMemo.Width = 100;
		this.rDocStatus.AutoHeight = false;
		this.rDocStatus.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo, "", -1, true, false, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rDocStatus.DisplayMember = "Name";
		this.rDocStatus.Name = "rDocStatus";
		this.rDocStatus.ValueMember = "Code";
		this.xtraTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.xtraTabControl1.IsSearchControl = false;
		this.xtraTabControl1.Location = new System.Drawing.Point(0, 0);
		this.xtraTabControl1.Name = "xtraTabControl1";
		this.xtraTabControl1.SelectedTabPage = this.xtraTabPage1;
		this.xtraTabControl1.Size = new System.Drawing.Size(1011, 270);
		this.xtraTabControl1.TabIndex = 0;
		this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[2] { this.xtraTabPage1, this.tab_Attachments });
		this.xtraTabPage1.Controls.Add(this.splitContainerControl2);
		this.xtraTabPage1.EditControl = null;
		this.xtraTabPage1.EditValue = "Contents";
		this.xtraTabPage1.FieldName = "xtraTabPage1";
		this.xtraTabPage1.IsSearchControl = false;
		this.xtraTabPage1.Name = "xtraTabPage1";
		this.xtraTabPage1.ReadOnly = false;
		this.xtraTabPage1.SettingInVisible = false;
		this.xtraTabPage1.SettingReadonly = false;
		this.xtraTabPage1.Size = new System.Drawing.Size(1009, 245);
		this.xtraTabPage1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPage1.Text = "Contents";
		this.splitContainerControl2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainerControl2.Horizontal = false;
		this.splitContainerControl2.Location = new System.Drawing.Point(0, 0);
		this.splitContainerControl2.Name = "splitContainerControl2";
		this.splitContainerControl2.Panel1.Controls.Add(this.grd_Lines);
		this.splitContainerControl2.Panel1.Text = "Panel1";
		this.splitContainerControl2.Panel2.Appearance.BackColor = System.Drawing.Color.WhiteSmoke;
		this.splitContainerControl2.Panel2.Appearance.Options.UseBackColor = true;
		this.splitContainerControl2.Panel2.Controls.Add(this.ucBatchAndSerial1);
		this.splitContainerControl2.Panel2.Text = "Panel2";
		this.splitContainerControl2.Size = new System.Drawing.Size(1009, 245);
		this.splitContainerControl2.SplitterPosition = 155;
		this.splitContainerControl2.TabIndex = 0;
		this.splitContainerControl2.Text = "splitLineBatchSerial";
		this.grd_Lines.AllowPasteExcel = true;
		this.grd_Lines.ClipboardData = null;
		this.grd_Lines.ContextMenuStrip = this.contextMenuStrip_Line;
		this.grd_Lines.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Lines.Location = new System.Drawing.Point(0, 0);
		this.grd_Lines.MainView = this.gv_Lines;
		this.grd_Lines.Name = "grd_Lines";
		this.grd_Lines.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[15]
		{
			this.rLineItemCode, this.rLineTaxCode, this.rLineDateEdit, this.rLineAcctCode, this.rLineWhs, this.rLineCurrency, this.rLineUomCode, this.rLineCal, this.rLineInventoryUom, this.rLineUomGroup,
			this.rLineBaseRef, this.rep_LineProject, this.rPriceEdit, this.rTranType, this.rLineSpecPrice
		});
		this.grd_Lines.Size = new System.Drawing.Size(1009, 155);
		this.grd_Lines.TabIndex = 0;
		this.grd_Lines.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Lines });
		this.grd_Lines.EditorKeyDown += new System.Windows.Forms.KeyEventHandler(this.grid_EditorKeyDown);
		this.contextMenuStrip_Line.ImageScalingSize = new System.Drawing.Size(20, 20);
		this.contextMenuStrip_Line.Items.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.lineDeleteToolStripMenuItem, this.LineCmtDuplicate, this.batchSerialNumberToolStripMenuItem });
		this.contextMenuStrip_Line.Name = "contextMenuStrip1_DocLine";
		this.contextMenuStrip_Line.Size = new System.Drawing.Size(185, 70);
		this.contextMenuStrip_Line.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Line_Opening);
		this.lineDeleteToolStripMenuItem.Name = "lineDeleteToolStripMenuItem";
		this.lineDeleteToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
		this.lineDeleteToolStripMenuItem.Text = "Delete Row";
		this.lineDeleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_DocLine_Click);
		this.LineCmtDuplicate.Name = "LineCmtDuplicate";
		this.LineCmtDuplicate.Size = new System.Drawing.Size(184, 22);
		this.LineCmtDuplicate.Text = "Duplicate Row";
		this.LineCmtDuplicate.Click += new System.EventHandler(this.cmtDuplicate_Line_Click);
		this.batchSerialNumberToolStripMenuItem.Name = "batchSerialNumberToolStripMenuItem";
		this.batchSerialNumberToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
		this.batchSerialNumberToolStripMenuItem.Text = "Batch/Serial Number";
		this.batchSerialNumberToolStripMenuItem.Click += new System.EventHandler(this.batchSerialNumbersToolStripMenuItem_Click);
		this.gv_Lines.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Lines.ColumnKey = "ItemCode";
		this.gv_Lines.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[38]
		{
			this.gcLineBaseRef, this.gc_Line_ItemCode, this.gcLineItemDescription, this.gcLineQuantity, this.gcLineAvgPrice, this.gc_LinePrice, this.gcLinePrice, this.gc_LinePQTReqdate, this.gc_LineDiscountPercent, this.gc_LineVatGroup,
			this.gc_LineTotal, this.gcLineWarehouseCode, this.gcLineFreeTxt, this.gcLineOpenQty, this.gcLineCurrency, this.gcLineRate, this.gcLineDocEntry, this.gcLineNum, this.gcLineSumLineTotalRate, this.gcLineBaseType,
			this.gcLineBaseEntry, this.gcBaseRef, this.gcLineBaseLine, this.gcLineAcctCode, this.gcLineStatus, this.ShipDate, this.gcLineUomGroup, this.gcLineUomName, this.gcLineInventoryUom, this.gcLineItemPerUnit,
			this.gcLineUomCode, this.gcLineIvnvQty, this.gcLineOpenInvQty, this.gcLineQuantityOnStock, this.gc_Line_QuantityOnWareHouse, this.gc_LineProject, this.gcTranType, this.gcLineSpecPrice
		});
		this.gv_Lines.DisableFormSetting = false;
		this.gv_Lines.DisplayException = false;
		this.gv_Lines.EditColumn = null;
		this.gv_Lines.FieldCurrency = "Currency";
		this.gv_Lines.FieldRate = "Rate";
		this.gv_Lines.FormId = "oReceiptIssue";
		this.gv_Lines.GridControl = this.grd_Lines;
		this.gv_Lines.HasFormSetting = false;
		this.gv_Lines.IgnoreFilter = false;
		this.gv_Lines.IndexEditRow = -1;
		this.gv_Lines.IndicatorWidth = 35;
		this.gv_Lines.IsApzTable = false;
		this.gv_Lines.IsInsertNewRow = false;
		this.gv_Lines.IsLoadingData = false;
		this.gv_Lines.IsMappingContextAction = false;
		this.gv_Lines.ItemId = "IGE1";
		this.gv_Lines.LstColumnAllowBatch = null;
		this.gv_Lines.LstColumnNotAllowBatch = null;
		this.gv_Lines.LstColumnNotEditable = null;
		this.gv_Lines.LstColumNotSetting = null;
		this.gv_Lines.Name = "gv_Lines";
		this.gv_Lines.OptionsDetail.EnableMasterViewMode = false;
		this.gv_Lines.OptionsFilter.DefaultFilterEditorView = DevExpress.XtraEditors.FilterEditorViewMode.VisualAndText;
		this.gv_Lines.OptionsSelection.MultiSelect = true;
		this.gv_Lines.OptionsView.ColumnAutoWidth = false;
		this.gv_Lines.OptionsView.ShowFooter = true;
		this.gv_Lines.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Lines.OptionsView.ShowGroupPanel = false;
		this.gv_Lines.UseBatchMod = false;
		this.gv_Lines.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Lines_RowCellStyle);
		this.gv_Lines.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.gv_Lines_RowStyle);
		this.gv_Lines.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_Lines_ShowingEditor);
		this.gv_Lines.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_Lines_FocusedRowObjectChanged);
		this.gv_Lines.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Lines_CellValueChanged);
		this.gcLineBaseRef.Caption = "Order No.";
		this.gcLineBaseRef.ColumnEdit = this.rLineBaseRef;
		this.gcLineBaseRef.CurrencyDateField = null;
		this.gcLineBaseRef.CurrencyField = null;
		this.gcLineBaseRef.CurrencyFixed = null;
		this.gcLineBaseRef.CurrencyFixedRate = 0.0;
		this.gcLineBaseRef.CurrencyRateField = null;
		this.gcLineBaseRef.FieldName = "BaseRef";
		this.gcLineBaseRef.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gcLineBaseRef.Name = "gcLineBaseRef";
		this.gcLineBaseRef.PrimaryKey = true;
		this.gcLineBaseRef.UnitField = null;
		this.gcLineBaseRef.Visible = true;
		this.gcLineBaseRef.VisibleIndex = 0;
		this.gcLineBaseRef.Width = 100;
		this.rLineBaseRef.AutoHeight = false;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.rLineBaseRef.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rLineBaseRef.Name = "rLineBaseRef";
		this.rLineBaseRef.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_BaseRef_ButtonClick);
		this.rLineBaseRef.KeyDown += new System.Windows.Forms.KeyEventHandler(this.rep_BaseRef_KeyDown);
		this.gc_Line_ItemCode.Caption = "Item Code";
		this.gc_Line_ItemCode.CflType = Apzon.Commons.Enums.Enums.TypeCFL.Items;
		this.gc_Line_ItemCode.ColumnEdit = this.rLineItemCode;
		this.gc_Line_ItemCode.CurrencyDateField = null;
		this.gc_Line_ItemCode.CurrencyField = null;
		this.gc_Line_ItemCode.CurrencyFixed = null;
		this.gc_Line_ItemCode.CurrencyFixedRate = 0.0;
		this.gc_Line_ItemCode.CurrencyRateField = null;
		this.gc_Line_ItemCode.FieldName = "ItemCode";
		this.gc_Line_ItemCode.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gc_Line_ItemCode.Name = "gc_Line_ItemCode";
		this.gc_Line_ItemCode.PrimaryKey = true;
		this.gc_Line_ItemCode.UnitField = null;
		this.gc_Line_ItemCode.Visible = true;
		this.gc_Line_ItemCode.VisibleIndex = 1;
		this.gc_Line_ItemCode.Width = 124;
		this.rLineItemCode.AutoHeight = false;
		editorButtonImageOptions2.Image = Apzon.Forms.Properties.Resources.right;
		this.rLineItemCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rLineItemCode.Name = "rLineItemCode";
		this.rLineItemCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_ItemCode_Line_ButtonClick);
		this.gcLineItemDescription.Caption = "Item Description";
		this.gcLineItemDescription.CurrencyDateField = null;
		this.gcLineItemDescription.CurrencyField = null;
		this.gcLineItemDescription.CurrencyFixed = null;
		this.gcLineItemDescription.CurrencyFixedRate = 0.0;
		this.gcLineItemDescription.CurrencyRateField = null;
		this.gcLineItemDescription.FieldName = "Dscription";
		this.gcLineItemDescription.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gcLineItemDescription.Name = "gcLineItemDescription";
		this.gcLineItemDescription.PrimaryKey = false;
		this.gcLineItemDescription.UnitField = null;
		this.gcLineItemDescription.Visible = true;
		this.gcLineItemDescription.VisibleIndex = 2;
		this.gcLineItemDescription.Width = 151;
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
		this.gcLineQuantity.VisibleIndex = 3;
		this.gcLineQuantity.Width = 100;
		this.rLineCal.AutoHeight = false;
		this.rLineCal.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rLineCal.Name = "rLineCal";
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
		this.gcLineAvgPrice.VisibleIndex = 4;
		this.gc_LinePrice.Caption = "Unit Price";
		this.gc_LinePrice.ColumnEdit = this.rPriceEdit;
		this.gc_LinePrice.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gc_LinePrice.CurrencyDateField = "DocDate";
		this.gc_LinePrice.CurrencyField = "Currency";
		this.gc_LinePrice.CurrencyFixed = null;
		this.gc_LinePrice.CurrencyFixedRate = 0.0;
		this.gc_LinePrice.CurrencyRateField = "Rate";
		this.gc_LinePrice.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gc_LinePrice.FieldName = "PriceBefDi";
		this.gc_LinePrice.Name = "gc_LinePrice";
		this.gc_LinePrice.PrimaryKey = false;
		this.gc_LinePrice.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gc_LinePrice.UnitField = null;
		this.gc_LinePrice.Visible = true;
		this.gc_LinePrice.VisibleIndex = 5;
		this.gc_LinePrice.Width = 100;
		this.rPriceEdit.AutoHeight = false;
		this.rPriceEdit.Name = "rPriceEdit";
		this.gcLinePrice.AllowChSet = false;
		this.gcLinePrice.Caption = "Price After Discount";
		this.gcLinePrice.ColumnEdit = this.repositoryItemTextEditExPrice1;
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
		this.gcLinePrice.UnitField = null;
		this.gcLinePrice.Visible = true;
		this.gcLinePrice.VisibleIndex = 6;
		this.gc_LinePQTReqdate.Caption = "RequiredDate";
		this.gc_LinePQTReqdate.ColumnEdit = this.rLineDateEdit;
		this.gc_LinePQTReqdate.CurrencyDateField = null;
		this.gc_LinePQTReqdate.CurrencyField = null;
		this.gc_LinePQTReqdate.CurrencyFixed = null;
		this.gc_LinePQTReqdate.CurrencyFixedRate = 0.0;
		this.gc_LinePQTReqdate.CurrencyRateField = null;
		this.gc_LinePQTReqdate.DisplayFormat.FormatString = "d";
		this.gc_LinePQTReqdate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.gc_LinePQTReqdate.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gc_LinePQTReqdate.FieldName = "PQTReqdate";
		this.gc_LinePQTReqdate.Name = "gc_LinePQTReqdate";
		this.gc_LinePQTReqdate.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gc_LinePQTReqdate.OptionsColumn.ReadOnly = true;
		this.gc_LinePQTReqdate.PrimaryKey = false;
		this.gc_LinePQTReqdate.UnitField = null;
		this.gc_LinePQTReqdate.Width = 100;
		this.rLineDateEdit.AutoHeight = false;
		this.rLineDateEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rLineDateEdit.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rLineDateEdit.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.rLineDateEdit.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.rLineDateEdit.EditFormat.FormatString = "dd/MM/yyyy";
		this.rLineDateEdit.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.rLineDateEdit.Mask.EditMask = "dd/MM/yyyy";
		this.rLineDateEdit.Name = "rLineDateEdit";
		this.gc_LineDiscountPercent.Caption = "Discount";
		this.gc_LineDiscountPercent.ColumnEdit = this.rLineCal;
		this.gc_LineDiscountPercent.CurrencyDateField = null;
		this.gc_LineDiscountPercent.CurrencyField = null;
		this.gc_LineDiscountPercent.CurrencyFixed = null;
		this.gc_LineDiscountPercent.CurrencyFixedRate = 0.0;
		this.gc_LineDiscountPercent.CurrencyRateField = null;
		this.gc_LineDiscountPercent.DisplayFormat.FormatString = "p0";
		this.gc_LineDiscountPercent.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gc_LineDiscountPercent.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Percent;
		this.gc_LineDiscountPercent.FieldName = "DiscPrcnt";
		this.gc_LineDiscountPercent.Name = "gc_LineDiscountPercent";
		this.gc_LineDiscountPercent.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gc_LineDiscountPercent.PrimaryKey = false;
		this.gc_LineDiscountPercent.UnitField = null;
		this.gc_LineDiscountPercent.Width = 100;
		this.gc_LineVatGroup.Caption = "Tax Code";
		this.gc_LineVatGroup.ColumnEdit = this.rLineTaxCode;
		this.gc_LineVatGroup.CurrencyDateField = null;
		this.gc_LineVatGroup.CurrencyField = null;
		this.gc_LineVatGroup.CurrencyFixed = null;
		this.gc_LineVatGroup.CurrencyFixedRate = 0.0;
		this.gc_LineVatGroup.CurrencyRateField = null;
		this.gc_LineVatGroup.FieldName = "VatGroup";
		this.gc_LineVatGroup.Name = "gc_LineVatGroup";
		this.gc_LineVatGroup.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gc_LineVatGroup.OptionsColumn.ReadOnly = true;
		this.gc_LineVatGroup.PrimaryKey = false;
		this.gc_LineVatGroup.UnitField = null;
		this.gc_LineVatGroup.Width = 100;
		this.rLineTaxCode.AutoHeight = false;
		this.rLineTaxCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, false, false, imageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance4, appearanceHovered4, appearancePressed4, appearanceDisabled4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rLineTaxCode.Name = "rLineTaxCode";
		this.rLineTaxCode.ReadOnly = true;
		this.rLineTaxCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_TaxCode_ButtonClick);
		this.gc_LineTotal.Caption = "Total";
		this.gc_LineTotal.ColumnEdit = this.rLineCal;
		this.gc_LineTotal.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.MainCurrency;
		this.gc_LineTotal.CurrencyDateField = null;
		this.gc_LineTotal.CurrencyField = "Currency";
		this.gc_LineTotal.CurrencyFixed = null;
		this.gc_LineTotal.CurrencyFixedRate = 0.0;
		this.gc_LineTotal.CurrencyRateField = null;
		this.gc_LineTotal.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gc_LineTotal.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gc_LineTotal.FieldName = "LineTotal";
		this.gc_LineTotal.Name = "gc_LineTotal";
		this.gc_LineTotal.PrimaryKey = false;
		this.gc_LineTotal.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gc_LineTotal.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[1]
		{
			new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "LineTotal", "{0:N}")
		});
		this.gc_LineTotal.UnitField = null;
		this.gc_LineTotal.Visible = true;
		this.gc_LineTotal.VisibleIndex = 7;
		this.gc_LineTotal.Width = 100;
		this.gcLineWarehouseCode.Caption = "Whse Code";
		this.gcLineWarehouseCode.CflType = Apzon.Commons.Enums.Enums.TypeCFL.Warehouse;
		this.gcLineWarehouseCode.ColumnEdit = this.rLineWhs;
		this.gcLineWarehouseCode.CurrencyDateField = null;
		this.gcLineWarehouseCode.CurrencyField = null;
		this.gcLineWarehouseCode.CurrencyFixed = null;
		this.gcLineWarehouseCode.CurrencyFixedRate = 0.0;
		this.gcLineWarehouseCode.CurrencyRateField = null;
		this.gcLineWarehouseCode.FieldName = "WhsCode";
		this.gcLineWarehouseCode.Name = "gcLineWarehouseCode";
		this.gcLineWarehouseCode.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcLineWarehouseCode.PrimaryKey = false;
		this.gcLineWarehouseCode.UnitField = null;
		this.gcLineWarehouseCode.Visible = true;
		this.gcLineWarehouseCode.VisibleIndex = 8;
		this.gcLineWarehouseCode.Width = 100;
		this.rLineWhs.AutoHeight = false;
		editorButtonImageOptions3.Image = Apzon.Forms.Properties.Resources.right;
		this.rLineWhs.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance5, appearanceHovered5, appearancePressed5, appearanceDisabled5, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rLineWhs.Name = "rLineWhs";
		this.rLineWhs.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_bte_LineWhs_ButtonPressed);
		this.gcLineFreeTxt.Caption = "Remark";
		this.gcLineFreeTxt.CurrencyDateField = null;
		this.gcLineFreeTxt.CurrencyField = null;
		this.gcLineFreeTxt.CurrencyFixed = null;
		this.gcLineFreeTxt.CurrencyFixedRate = 0.0;
		this.gcLineFreeTxt.CurrencyRateField = null;
		this.gcLineFreeTxt.FieldName = "FreeTxt";
		this.gcLineFreeTxt.Name = "gcLineFreeTxt";
		this.gcLineFreeTxt.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcLineFreeTxt.PrimaryKey = false;
		this.gcLineFreeTxt.UnitField = null;
		this.gcLineFreeTxt.Width = 100;
		this.gcLineOpenQty.Caption = "gcLineOpenQty";
		this.gcLineOpenQty.ColumnEdit = this.rLineCal;
		this.gcLineOpenQty.CurrencyDateField = null;
		this.gcLineOpenQty.CurrencyField = null;
		this.gcLineOpenQty.CurrencyFixed = null;
		this.gcLineOpenQty.CurrencyFixedRate = 0.0;
		this.gcLineOpenQty.CurrencyRateField = null;
		this.gcLineOpenQty.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gcLineOpenQty.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gcLineOpenQty.FieldName = "OpenQty";
		this.gcLineOpenQty.Name = "gcLineOpenQty";
		this.gcLineOpenQty.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcLineOpenQty.PrimaryKey = false;
		this.gcLineOpenQty.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gcLineOpenQty.UnitField = null;
		this.gcLineOpenQty.Width = 100;
		this.gcLineCurrency.Caption = "Currency";
		this.gcLineCurrency.ColumnEdit = this.rLineCurrency;
		this.gcLineCurrency.CurrencyDateField = null;
		this.gcLineCurrency.CurrencyField = null;
		this.gcLineCurrency.CurrencyFixed = null;
		this.gcLineCurrency.CurrencyFixedRate = 0.0;
		this.gcLineCurrency.CurrencyRateField = null;
		this.gcLineCurrency.FieldName = "Currency";
		this.gcLineCurrency.Name = "gcLineCurrency";
		this.gcLineCurrency.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcLineCurrency.PrimaryKey = false;
		this.gcLineCurrency.UnitField = null;
		this.gcLineCurrency.Width = 100;
		this.rLineCurrency.AutoHeight = false;
		this.rLineCurrency.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rLineCurrency.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name1"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Value", "Name2", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rLineCurrency.DisplayMember = "Name";
		this.rLineCurrency.Name = "rLineCurrency";
		this.rLineCurrency.NullText = "";
		this.rLineCurrency.ShowFooter = false;
		this.rLineCurrency.ShowHeader = false;
		this.rLineCurrency.ValueMember = "Value";
		this.gcLineRate.Caption = "Rate";
		this.gcLineRate.ColumnEdit = this.rLineCal;
		this.gcLineRate.CurrencyDateField = null;
		this.gcLineRate.CurrencyField = null;
		this.gcLineRate.CurrencyFixed = null;
		this.gcLineRate.CurrencyFixedRate = 0.0;
		this.gcLineRate.CurrencyRateField = null;
		this.gcLineRate.DisplayFormat.FormatString = "n3";
		this.gcLineRate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gcLineRate.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Rate;
		this.gcLineRate.FieldName = "Rate";
		this.gcLineRate.Name = "gcLineRate";
		this.gcLineRate.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcLineRate.PrimaryKey = false;
		this.gcLineRate.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gcLineRate.UnitField = null;
		this.gcLineRate.Width = 100;
		this.gcLineDocEntry.Caption = "DocEntry";
		this.gcLineDocEntry.CurrencyDateField = null;
		this.gcLineDocEntry.CurrencyField = null;
		this.gcLineDocEntry.CurrencyFixed = null;
		this.gcLineDocEntry.CurrencyFixedRate = 0.0;
		this.gcLineDocEntry.CurrencyRateField = null;
		this.gcLineDocEntry.FieldName = "DocEntry";
		this.gcLineDocEntry.Name = "gcLineDocEntry";
		this.gcLineDocEntry.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcLineDocEntry.PrimaryKey = false;
		this.gcLineDocEntry.UnitField = null;
		this.gcLineDocEntry.Width = 100;
		this.gcLineNum.Caption = "LineNum";
		this.gcLineNum.CurrencyDateField = null;
		this.gcLineNum.CurrencyField = null;
		this.gcLineNum.CurrencyFixed = null;
		this.gcLineNum.CurrencyFixedRate = 0.0;
		this.gcLineNum.CurrencyRateField = null;
		this.gcLineNum.FieldName = "LineNum";
		this.gcLineNum.Name = "gcLineNum";
		this.gcLineNum.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcLineNum.PrimaryKey = false;
		this.gcLineNum.UnitField = null;
		this.gcLineNum.Width = 100;
		this.gcLineSumLineTotalRate.Caption = "SumLineTotalRate";
		this.gcLineSumLineTotalRate.ColumnEdit = this.rLineCal;
		this.gcLineSumLineTotalRate.CurrencyDateField = null;
		this.gcLineSumLineTotalRate.CurrencyField = null;
		this.gcLineSumLineTotalRate.CurrencyFixed = null;
		this.gcLineSumLineTotalRate.CurrencyFixedRate = 0.0;
		this.gcLineSumLineTotalRate.CurrencyRateField = null;
		this.gcLineSumLineTotalRate.DisplayFormat.FormatString = "n3";
		this.gcLineSumLineTotalRate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gcLineSumLineTotalRate.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gcLineSumLineTotalRate.FieldName = "LineVat";
		this.gcLineSumLineTotalRate.Name = "gcLineSumLineTotalRate";
		this.gcLineSumLineTotalRate.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcLineSumLineTotalRate.PrimaryKey = false;
		this.gcLineSumLineTotalRate.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gcLineSumLineTotalRate.UnitField = null;
		this.gcLineSumLineTotalRate.Width = 100;
		this.gcLineBaseType.Caption = "Base Type";
		this.gcLineBaseType.CurrencyDateField = null;
		this.gcLineBaseType.CurrencyField = null;
		this.gcLineBaseType.CurrencyFixed = null;
		this.gcLineBaseType.CurrencyFixedRate = 0.0;
		this.gcLineBaseType.CurrencyRateField = null;
		this.gcLineBaseType.FieldName = "BaseType";
		this.gcLineBaseType.Name = "gcLineBaseType";
		this.gcLineBaseType.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcLineBaseType.PrimaryKey = false;
		this.gcLineBaseType.UnitField = null;
		this.gcLineBaseType.Width = 100;
		this.gcLineBaseEntry.Caption = "BaseEntry";
		this.gcLineBaseEntry.CurrencyDateField = null;
		this.gcLineBaseEntry.CurrencyField = null;
		this.gcLineBaseEntry.CurrencyFixed = null;
		this.gcLineBaseEntry.CurrencyFixedRate = 0.0;
		this.gcLineBaseEntry.CurrencyRateField = null;
		this.gcLineBaseEntry.FieldName = "BaseEntry";
		this.gcLineBaseEntry.Name = "gcLineBaseEntry";
		this.gcLineBaseEntry.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcLineBaseEntry.PrimaryKey = false;
		this.gcLineBaseEntry.UnitField = null;
		this.gcLineBaseEntry.Width = 100;
		this.gcLineBaseLine.Caption = "BaseLine";
		this.gcLineBaseLine.CurrencyDateField = null;
		this.gcLineBaseLine.CurrencyField = null;
		this.gcLineBaseLine.CurrencyFixed = null;
		this.gcLineBaseLine.CurrencyFixedRate = 0.0;
		this.gcLineBaseLine.CurrencyRateField = null;
		this.gcLineBaseLine.FieldName = "BaseLine";
		this.gcLineBaseLine.Name = "gcLineBaseLine";
		this.gcLineBaseLine.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcLineBaseLine.PrimaryKey = false;
		this.gcLineBaseLine.UnitField = null;
		this.gcLineBaseLine.Width = 100;
		this.gcLineAcctCode.Caption = "Account Number";
		this.gcLineAcctCode.CflType = Apzon.Commons.Enums.Enums.TypeCFL.AcctCode;
		this.gcLineAcctCode.ColumnEdit = this.rLineAcctCode;
		this.gcLineAcctCode.CurrencyDateField = null;
		this.gcLineAcctCode.CurrencyField = null;
		this.gcLineAcctCode.CurrencyFixed = null;
		this.gcLineAcctCode.CurrencyFixedRate = 0.0;
		this.gcLineAcctCode.CurrencyRateField = null;
		this.gcLineAcctCode.FieldName = "AcctCode";
		this.gcLineAcctCode.Name = "gcLineAcctCode";
		this.gcLineAcctCode.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcLineAcctCode.PrimaryKey = false;
		this.gcLineAcctCode.UnitField = null;
		this.gcLineAcctCode.Width = 100;
		this.rLineAcctCode.AutoHeight = false;
		this.rLineAcctCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rLineAcctCode.Name = "rLineAcctCode";
		this.rLineAcctCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.ribAcctCode_ButtonClick);
		this.gcLineStatus.Caption = "LineStatus";
		this.gcLineStatus.CurrencyDateField = null;
		this.gcLineStatus.CurrencyField = null;
		this.gcLineStatus.CurrencyFixed = null;
		this.gcLineStatus.CurrencyFixedRate = 0.0;
		this.gcLineStatus.CurrencyRateField = null;
		this.gcLineStatus.FieldName = "LineStatus";
		this.gcLineStatus.Name = "gcLineStatus";
		this.gcLineStatus.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcLineStatus.PrimaryKey = false;
		this.gcLineStatus.UnitField = null;
		this.gcLineStatus.Width = 100;
		this.ShipDate.Caption = "ShipDate";
		this.ShipDate.ColumnEdit = this.rLineDateEdit;
		this.ShipDate.CurrencyDateField = null;
		this.ShipDate.CurrencyField = null;
		this.ShipDate.CurrencyFixed = null;
		this.ShipDate.CurrencyFixedRate = 0.0;
		this.ShipDate.CurrencyRateField = null;
		this.ShipDate.DisplayFormat.FormatString = "d";
		this.ShipDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.ShipDate.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.ShipDate.FieldName = "ShipDate";
		this.ShipDate.Name = "ShipDate";
		this.ShipDate.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.ShipDate.PrimaryKey = false;
		this.ShipDate.UnitField = null;
		this.ShipDate.Width = 100;
		this.gcLineUomGroup.Caption = "Uom Group";
		this.gcLineUomGroup.ColumnEdit = this.rLineUomGroup;
		this.gcLineUomGroup.CurrencyDateField = null;
		this.gcLineUomGroup.CurrencyField = null;
		this.gcLineUomGroup.CurrencyFixed = null;
		this.gcLineUomGroup.CurrencyFixedRate = 0.0;
		this.gcLineUomGroup.CurrencyRateField = null;
		this.gcLineUomGroup.FieldName = "UgpCode";
		this.gcLineUomGroup.Name = "gcLineUomGroup";
		this.gcLineUomGroup.OptionsColumn.AllowEdit = false;
		this.gcLineUomGroup.PrimaryKey = false;
		this.gcLineUomGroup.UnitField = null;
		this.gcLineUomGroup.Visible = true;
		this.gcLineUomGroup.VisibleIndex = 9;
		this.rLineUomGroup.AutoHeight = false;
		this.rLineUomGroup.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rLineUomGroup.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name3"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Name4", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rLineUomGroup.DisplayMember = "Name";
		this.rLineUomGroup.Name = "rLineUomGroup";
		this.rLineUomGroup.NullText = "";
		this.rLineUomGroup.ShowFooter = false;
		this.rLineUomGroup.ShowHeader = false;
		this.rLineUomGroup.ValueMember = "Code";
		this.gcLineUomName.Caption = "UoM Name";
		this.gcLineUomName.CurrencyDateField = null;
		this.gcLineUomName.CurrencyField = null;
		this.gcLineUomName.CurrencyFixed = null;
		this.gcLineUomName.CurrencyFixedRate = 0.0;
		this.gcLineUomName.CurrencyRateField = null;
		this.gcLineUomName.FieldName = "unitMsr";
		this.gcLineUomName.Name = "gcLineUomName";
		this.gcLineUomName.PrimaryKey = false;
		this.gcLineUomName.UnitField = null;
		this.gcLineUomName.Visible = true;
		this.gcLineUomName.VisibleIndex = 10;
		this.gcLineInventoryUom.Caption = "Inventory Uom";
		this.gcLineInventoryUom.ColumnEdit = this.rLineInventoryUom;
		this.gcLineInventoryUom.CurrencyDateField = null;
		this.gcLineInventoryUom.CurrencyField = null;
		this.gcLineInventoryUom.CurrencyFixed = null;
		this.gcLineInventoryUom.CurrencyFixedRate = 0.0;
		this.gcLineInventoryUom.CurrencyRateField = null;
		this.gcLineInventoryUom.FieldName = "UseBaseUn";
		this.gcLineInventoryUom.Name = "gcLineInventoryUom";
		this.gcLineInventoryUom.PrimaryKey = false;
		this.gcLineInventoryUom.UnitField = null;
		this.gcLineInventoryUom.Visible = true;
		this.gcLineInventoryUom.VisibleIndex = 11;
		this.rLineInventoryUom.AutoHeight = false;
		this.rLineInventoryUom.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rLineInventoryUom.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name1"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Name2", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rLineInventoryUom.DisplayMember = "Name";
		this.rLineInventoryUom.Name = "rLineInventoryUom";
		this.rLineInventoryUom.NullText = "";
		this.rLineInventoryUom.ShowFooter = false;
		this.rLineInventoryUom.ShowHeader = false;
		this.rLineInventoryUom.ValueMember = "Code";
		this.gcLineItemPerUnit.Caption = "Item Per Unit";
		this.gcLineItemPerUnit.ColumnEdit = this.rLineCal;
		this.gcLineItemPerUnit.CurrencyDateField = null;
		this.gcLineItemPerUnit.CurrencyField = null;
		this.gcLineItemPerUnit.CurrencyFixed = null;
		this.gcLineItemPerUnit.CurrencyFixedRate = 0.0;
		this.gcLineItemPerUnit.CurrencyRateField = null;
		this.gcLineItemPerUnit.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gcLineItemPerUnit.FieldName = "NumPerMsr";
		this.gcLineItemPerUnit.Name = "gcLineItemPerUnit";
		this.gcLineItemPerUnit.PrimaryKey = false;
		this.gcLineItemPerUnit.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gcLineItemPerUnit.UnitField = null;
		this.gcLineItemPerUnit.Visible = true;
		this.gcLineItemPerUnit.VisibleIndex = 12;
		this.gcLineUomCode.Caption = "UoM Code";
		this.gcLineUomCode.ColumnEdit = this.rLineUomCode;
		this.gcLineUomCode.CurrencyDateField = null;
		this.gcLineUomCode.CurrencyField = null;
		this.gcLineUomCode.CurrencyFixed = null;
		this.gcLineUomCode.CurrencyFixedRate = 0.0;
		this.gcLineUomCode.CurrencyRateField = null;
		this.gcLineUomCode.FieldName = "UomCode";
		this.gcLineUomCode.Name = "gcLineUomCode";
		this.gcLineUomCode.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcLineUomCode.PrimaryKey = false;
		this.gcLineUomCode.UnitField = null;
		this.gcLineUomCode.Visible = true;
		this.gcLineUomCode.VisibleIndex = 13;
		this.gcLineUomCode.Width = 100;
		this.rLineUomCode.AutoHeight = false;
		serializableAppearanceObject.BackColor = System.Drawing.Color.OldLace;
		serializableAppearanceObject.Options.UseBackColor = true;
		serializableAppearanceObject2.BackColor = System.Drawing.Color.OldLace;
		serializableAppearanceObject2.Options.UseBackColor = true;
		serializableAppearanceObject3.BackColor = System.Drawing.Color.OldLace;
		serializableAppearanceObject3.Options.UseBackColor = true;
		serializableAppearanceObject4.BackColor = System.Drawing.Color.OldLace;
		serializableAppearanceObject4.Options.UseBackColor = true;
		this.rLineUomCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Ellipsis, "", -1, true, true, false, imageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject, serializableAppearanceObject2, serializableAppearanceObject3, serializableAppearanceObject4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rLineUomCode.Name = "rLineUomCode";
		this.rLineUomCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rLineUomCode_ButtonClick);
		this.gcLineIvnvQty.Caption = "Qty(Inventory UoM)";
		this.gcLineIvnvQty.ColumnEdit = this.repositoryItemCalcEditEx1;
		this.gcLineIvnvQty.CurrencyDateField = null;
		this.gcLineIvnvQty.CurrencyField = null;
		this.gcLineIvnvQty.CurrencyFixed = null;
		this.gcLineIvnvQty.CurrencyFixedRate = 0.0;
		this.gcLineIvnvQty.CurrencyRateField = null;
		this.gcLineIvnvQty.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gcLineIvnvQty.FieldName = "InvQty";
		this.gcLineIvnvQty.Name = "gcLineIvnvQty";
		this.gcLineIvnvQty.PrimaryKey = false;
		this.gcLineIvnvQty.UnitField = null;
		this.gcLineIvnvQty.Visible = true;
		this.gcLineIvnvQty.VisibleIndex = 14;
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
		this.gcLineOpenInvQty.PrimaryKey = false;
		this.gcLineOpenInvQty.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gcLineOpenInvQty.UnitField = null;
		this.gcLineOpenInvQty.Visible = true;
		this.gcLineOpenInvQty.VisibleIndex = 15;
		this.gcLineQuantityOnStock.Caption = "In Stock";
		this.gcLineQuantityOnStock.ColumnEdit = this.rLineCal;
		this.gcLineQuantityOnStock.CurrencyDateField = null;
		this.gcLineQuantityOnStock.CurrencyField = null;
		this.gcLineQuantityOnStock.CurrencyFixed = null;
		this.gcLineQuantityOnStock.CurrencyFixedRate = 0.0;
		this.gcLineQuantityOnStock.CurrencyRateField = null;
		this.gcLineQuantityOnStock.DisplayFormat.FormatString = "n3";
		this.gcLineQuantityOnStock.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gcLineQuantityOnStock.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gcLineQuantityOnStock.FieldName = "QuantityOnStock";
		this.gcLineQuantityOnStock.Name = "gcLineQuantityOnStock";
		this.gcLineQuantityOnStock.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcLineQuantityOnStock.OptionsColumn.ReadOnly = true;
		this.gcLineQuantityOnStock.PrimaryKey = false;
		this.gcLineQuantityOnStock.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gcLineQuantityOnStock.UnitField = null;
		this.gcLineQuantityOnStock.Visible = true;
		this.gcLineQuantityOnStock.VisibleIndex = 16;
		this.gcLineQuantityOnStock.Width = 100;
		this.gc_Line_QuantityOnWareHouse.Caption = "Qty in Warehouse";
		this.gc_Line_QuantityOnWareHouse.ColumnEdit = this.rLineCal;
		this.gc_Line_QuantityOnWareHouse.CurrencyDateField = null;
		this.gc_Line_QuantityOnWareHouse.CurrencyField = null;
		this.gc_Line_QuantityOnWareHouse.CurrencyFixed = null;
		this.gc_Line_QuantityOnWareHouse.CurrencyFixedRate = 0.0;
		this.gc_Line_QuantityOnWareHouse.CurrencyRateField = null;
		this.gc_Line_QuantityOnWareHouse.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gc_Line_QuantityOnWareHouse.FieldName = "QuantityOnWareHouse";
		this.gc_Line_QuantityOnWareHouse.Name = "gc_Line_QuantityOnWareHouse";
		this.gc_Line_QuantityOnWareHouse.OptionsColumn.ReadOnly = true;
		this.gc_Line_QuantityOnWareHouse.PrimaryKey = false;
		this.gc_Line_QuantityOnWareHouse.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gc_Line_QuantityOnWareHouse.UnitField = null;
		this.gc_Line_QuantityOnWareHouse.Visible = true;
		this.gc_Line_QuantityOnWareHouse.VisibleIndex = 17;
		this.gc_Line_QuantityOnWareHouse.Width = 100;
		this.gc_LineProject.Caption = "Project";
		this.gc_LineProject.CflType = Apzon.Commons.Enums.Enums.TypeCFL.Project;
		this.gc_LineProject.ColumnEdit = this.rep_LineProject;
		this.gc_LineProject.CurrencyDateField = null;
		this.gc_LineProject.CurrencyField = null;
		this.gc_LineProject.CurrencyFixed = null;
		this.gc_LineProject.CurrencyFixedRate = 0.0;
		this.gc_LineProject.CurrencyRateField = null;
		this.gc_LineProject.FieldName = "Project";
		this.gc_LineProject.Name = "gc_LineProject";
		this.gc_LineProject.PrimaryKey = false;
		this.gc_LineProject.UnitField = null;
		this.gc_LineProject.Visible = true;
		this.gc_LineProject.VisibleIndex = 18;
		this.rep_LineProject.AutoHeight = false;
		this.rep_LineProject.AutoLoadDataSource = true;
		this.rep_LineProject.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, imageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance6, appearanceHovered6, appearancePressed6, appearanceDisabled6, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
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
		this.gcTranType.Caption = "Trans. Type";
		this.gcTranType.ColumnEdit = this.rTranType;
		this.gcTranType.CurrencyDateField = null;
		this.gcTranType.CurrencyField = null;
		this.gcTranType.CurrencyFixed = null;
		this.gcTranType.CurrencyFixedRate = 0.0;
		this.gcTranType.CurrencyRateField = null;
		this.gcTranType.FieldName = "TranType";
		this.gcTranType.Name = "gcTranType";
		this.gcTranType.PrimaryKey = false;
		this.gcTranType.UnitField = null;
		this.gcTranType.Visible = true;
		this.gcTranType.VisibleIndex = 19;
		this.rTranType.AutoHeight = false;
		this.rTranType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rTranType.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.rTranType.DisplayMember = "Name";
		this.rTranType.Name = "rTranType";
		this.rTranType.NullText = "";
		this.rTranType.ShowHeader = false;
		this.rTranType.ValueMember = "Code";
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
		this.ucBatchAndSerial1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		this.ucBatchAndSerial1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.ucBatchAndSerial1.FieldQuantity = "InvQty";
		this.ucBatchAndSerial1.FormId = "BatchAndSerial";
		this.ucBatchAndSerial1.GvHeader = this.gv_Doc;
		this.ucBatchAndSerial1.GvLineItem = this.gv_Lines;
		this.ucBatchAndSerial1.IsAllowcateBinLocationToWhs = false;
		this.ucBatchAndSerial1.ItemId = "OBTN";
		this.ucBatchAndSerial1.ItemIdBin = "oBinAllowcation";
		this.ucBatchAndSerial1.Location = new System.Drawing.Point(0, 0);
		this.ucBatchAndSerial1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.ucBatchAndSerial1.Name = "ucBatchAndSerial1";
		this.ucBatchAndSerial1.Size = new System.Drawing.Size(1009, 80);
		this.ucBatchAndSerial1.TabIndex = 0;
		this.tab_Attachments.Controls.Add(this.ucAttachment1);
		this.tab_Attachments.EditControl = null;
		this.tab_Attachments.EditValue = "Attachments";
		this.tab_Attachments.FieldName = "tab_Attachments";
		this.tab_Attachments.IsSearchControl = false;
		this.tab_Attachments.Name = "tab_Attachments";
		this.tab_Attachments.ReadOnly = false;
		this.tab_Attachments.SettingInVisible = false;
		this.tab_Attachments.SettingReadonly = false;
		this.tab_Attachments.Size = new System.Drawing.Size(1009, 245);
		this.tab_Attachments.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Attachments.Text = "Attachments";
		this.ucAttachment1.DataSource = null;
		this.ucAttachment1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.ucAttachment1.FormId = "";
		this.ucAttachment1.ItemId = "ATC1";
		this.ucAttachment1.Location = new System.Drawing.Point(0, 0);
		this.ucAttachment1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.ucAttachment1.Name = "ucAttachment1";
		this.ucAttachment1.Size = new System.Drawing.Size(1009, 245);
		this.ucAttachment1.TabIndex = 0;
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
		this.txtPriceList.EditControl = null;
		this.txtPriceList.FieldName = null;
		this.txtPriceList.IsSearchControl = true;
		this.txtPriceList.Location = new System.Drawing.Point(288, 25);
		this.txtPriceList.MenuManager = this.barManager1;
		this.txtPriceList.Name = "txtPriceList";
		this.txtPriceList.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtPriceList.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.txtPriceList.Properties.DisplayMember = "Name";
		this.txtPriceList.Properties.NullText = "";
		this.txtPriceList.Properties.ValueMember = "Code";
		this.txtPriceList.SettingInVisible = false;
		this.txtPriceList.SettingReadonly = false;
		this.txtPriceList.Size = new System.Drawing.Size(100, 20);
		this.txtPriceList.TabIndex = 38;
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
		this.btn_Save.MenuManager = this.barManager1;
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.ReadOnly = false;
		this.btn_Save.SettingInVisible = false;
		this.btn_Save.SettingReadonly = false;
		this.btn_Save.Size = new System.Drawing.Size(120, 20);
		this.btn_Save.TabIndex = 43;
		this.btn_Save.Text = "Add && Search";
		this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
		this.drlCopyTo.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.drlCopyTo.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.drlCopyTo.Appearance.Options.UseBackColor = true;
		this.drlCopyTo.EditControl = null;
		this.drlCopyTo.EditValue = "Copy To";
		this.drlCopyTo.Enabled = false;
		this.drlCopyTo.FieldName = null;
		this.drlCopyTo.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("drlCopyTo.ImageOptions.Image");
		this.drlCopyTo.IsSearchControl = false;
		this.drlCopyTo.ItemId = null;
		this.drlCopyTo.Location = new System.Drawing.Point(908, 510);
		this.drlCopyTo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.drlCopyTo.Name = "drlCopyTo";
		this.drlCopyTo.ReadOnly = false;
		this.drlCopyTo.SettingInVisible = false;
		this.drlCopyTo.SettingReadonly = false;
		this.drlCopyTo.Size = new System.Drawing.Size(99, 20);
		this.drlCopyTo.TabIndex = 49;
		this.drlCopyTo.Text = "Copy To";
		this.drlCopyFrom.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.drlCopyFrom.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.drlCopyFrom.Appearance.Options.UseBackColor = true;
		this.drlCopyFrom.EditControl = null;
		this.drlCopyFrom.EditValue = "Copy From";
		this.drlCopyFrom.Enabled = false;
		this.drlCopyFrom.FieldName = null;
		this.drlCopyFrom.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("drlCopyFrom.ImageOptions.Image");
		this.drlCopyFrom.IsSearchControl = false;
		this.drlCopyFrom.ItemId = null;
		this.drlCopyFrom.Location = new System.Drawing.Point(803, 510);
		this.drlCopyFrom.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.drlCopyFrom.Name = "drlCopyFrom";
		this.drlCopyFrom.ReadOnly = false;
		this.drlCopyFrom.SettingInVisible = false;
		this.drlCopyFrom.SettingReadonly = false;
		this.drlCopyFrom.Size = new System.Drawing.Size(99, 20);
		this.drlCopyFrom.TabIndex = 48;
		this.drlCopyFrom.Text = "Copy From";
		this.gcBaseRef.Caption = "gridColumnEx1";
		this.gcBaseRef.CurrencyDateField = null;
		this.gcBaseRef.CurrencyField = null;
		this.gcBaseRef.CurrencyFixed = null;
		this.gcBaseRef.CurrencyFixedRate = 0.0;
		this.gcBaseRef.CurrencyRateField = null;
		this.gcBaseRef.FieldName = "BaseRef";
		this.gcBaseRef.Name = "gcBaseRef";
		this.gcBaseRef.PrimaryKey = false;
		this.gcBaseRef.UnitField = null;
		this.btn_RetCom.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_RetCom.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_RetCom.Appearance.Options.UseBackColor = true;
		this.btn_RetCom.EditControl = null;
		this.btn_RetCom.EditValue = "Return Components";
		this.btn_RetCom.Enabled = false;
		this.btn_RetCom.FieldName = null;
		this.btn_RetCom.IsSearchControl = false;
		this.btn_RetCom.Location = new System.Drawing.Point(686, 510);
		this.btn_RetCom.Name = "btn_RetCom";
		this.btn_RetCom.ReadOnly = false;
		this.btn_RetCom.SettingInVisible = false;
		this.btn_RetCom.SettingReadonly = false;
		this.btn_RetCom.Size = new System.Drawing.Size(111, 23);
		this.btn_RetCom.TabIndex = 54;
		this.btn_RetCom.Text = "Return Components";
		this.btn_RetCom.Visible = false;
		this.btn_RetCom.Click += new System.EventHandler(this.btn_RetCom_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btn_Cancel;
		base.ClientSize = new System.Drawing.Size(1015, 537);
		base.Controls.Add(this.btn_RetCom);
		base.Controls.Add(this.drlCopyTo);
		base.Controls.Add(this.drlCopyFrom);
		base.Controls.Add(this.btn_Save);
		base.Controls.Add(this.txtPriceList);
		base.Controls.Add(this.txtNo);
		base.Controls.Add(this.lblfdate);
		base.Controls.Add(this.txtfDate);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btnSearch);
		base.Controls.Add(this.splitContainerControl1);
		base.Controls.Add(this.lbltdate);
		base.Controls.Add(this.txttDate);
		base.Controls.Add(this.lblNum);
		base.Controls.Add(this.lblPri);
		base.Controls.Add(this.barDockControlLeft);
		base.Controls.Add(this.barDockControlRight);
		base.Controls.Add(this.barDockControlBottom);
		base.Controls.Add(this.barDockControlTop);
		base.FormId = "oReceiptIssue";
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("VirFormReceiptIssue.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "VirFormReceiptIssue";
		this.Text = "Inventory Transaction - Receipt/Issue";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.VirFormReceiptIssue_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtNo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txttDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txttDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtfDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtfDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.barManager1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		this.contextMenuStrip_Document.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rDocDateEdit.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rDocDateEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rDocCal).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rDocCurrency).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rDocSeries).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rDocContactPerson).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rDocRequester).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rDocBranch).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rDocGLU_Department).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridView2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rDocGroupNum).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rDocStatus).EndInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).EndInit();
		this.xtraTabControl1.ResumeLayout(false);
		this.xtraTabPage1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl2).EndInit();
		this.splitContainerControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Lines).EndInit();
		this.contextMenuStrip_Line.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Lines).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rLineBaseRef).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rLineItemCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rLineCal).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rPriceEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rLineDateEdit.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rLineDateEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rLineTaxCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rLineWhs).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rLineCurrency).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rLineAcctCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rLineUomGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rLineInventoryUom).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rLineUomCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_LineProject).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rTranType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rLineSpecPrice).EndInit();
		this.tab_Attachments.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txtPriceList.Properties).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
