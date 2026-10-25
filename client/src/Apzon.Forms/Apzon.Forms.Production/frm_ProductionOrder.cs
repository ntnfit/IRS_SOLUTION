using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
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
using Apzon.Controls.Editor.TextEditExPrice;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.MasterData;
using Apzon.Entities.Models.Production;
using Apzon.Entities.Models.UserFields;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Forms.Interfaces.FormDocument;
using Apzon.Forms.Inventory.InventoryTransaction;
using Apzon.Forms.PM;
using Apzon.Forms.Properties;
using Apzon.Forms.UserControls;
using Apzon.Forms.UserControls.BatchAndSerial;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTab;
using SAPbobsCOM;

namespace Apzon.Forms.Production;

public class frm_ProductionOrder : XtraFormEx
{
	public int DocEntry;

	public string CurrentDocStatus = "P";

	public string ItemCode;

	public string WareH;

	private int LineNumFocus = -1;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private int _activeDimCount;

	private DataTable _dtSearch = new DataTable();

	private DataTable _dtSearchLine = new DataTable();

	private DataTable _dtIsIssued = new DataTable();

	private int _currentDocEntry;

	private int _newDocNum;

	private int _currentDocNum;

	private int _lineNum;

	private int _curRowtype;

	private string _oldTxtCodeValue = "";

	private bool _isLoading;

	private bool _isChangeQty;

	private bool _isPasteFromExcel;

	private bool _isChangedWhenAdd;

	private string[,] _arrStatus = new string[4, 2]
	{
		{ "P", "Planned" },
		{ "R", "Released" },
		{ "L", "Closed" },
		{ "C", "Canceled" }
	};

	private Enums.FormViewModeType _openFormViewMode = Enums.FormViewModeType.View;

	private bool _isMapping;

	private bool _showMessageEndate = true;

	private GridViewExPainter painter;

	private string _first = "firstR";

	private string _last = "lastR";

	private string _current = "currentR";

	private string _lastFocusLeave;

	private string _wibActCode;

	public static string FormLayoutCode = "WOR1";

	private IContainer components;

	private LabelLine lb_Type;

	private LabelLine lb_Warehouse;

	private LabelLine lb_Project;

	private LabelLine lb_DistrRule;

	private LabelLine lb_Customer;

	private LabelLine lb_SalesOrder;

	private LabelLine lb_Origin;

	private LabelLine lb_User;

	private LabelLine lb_DueDate;

	private LabelLine lb_OrderDate;

	private LabelLine lb_No;

	private LabelLine lb_PlannedQuantity;

	private LabelLine lb_ProductDescription;

	private LabelLine lb_ProductNo;

	private LabelLine lb_Status;

	private ButtonEditEx txtWarehouse;

	private TextEditEx txtProductDesc;

	private TextEditEx txtUoMName;

	private ButtonEditEx txtSaleOrder;

	private ButtonEditEx txtCustomer;

	private TextEditEx txtNo;

	private DateEditEx dateOrder;

	private DateEditEx dateDue;

	private XtraTabControlEx xtraTabControl1;

	private XtraTabPageEx tab_Components;

	private XtraTabPageEx tab_Summary;

	private MemoEditEx memoPickRemarks;

	private MemoEditEx memoRemarks;

	private LabelLine lb_PickAndPackRemarks;

	private LabelLine lb_Remarks;

	private LabelLine lb_UoMName;

	private GridControlEx grdDoc;

	private LabelLine lb_TotalVariance;

	private LabelLine lb_ActualByProductCost;

	private LabelLine lblapc;

	private LabelLine lb_AtlAdditionalCost;

	private LabelLine lb_ActualResourceComponentCost;

	private LabelLine lb_ActualItemComponentCost;

	private LabelLine labelLine38;

	private TextEditEx txtJournalRemark;

	private LabelLine lb_OverDue;

	private TextEditEx txtOverDue;

	private LabelLine lb_ActualClosingDate;

	private LabelLine lb_DueDate1;

	private LabelLine lb_TotalTime;

	private TextEditEx txtTotalTime;

	private LabelLine lb_TotalAdditionalTime;

	private TextEditEx txtTotalAdditionalTime;

	private LabelLine lb_TotalRunTime;

	private TextEditEx txtTotalRunTime;

	private LabelLine lbRejectedQuantity;

	private LabelLine lb_CompletedQuantity;

	private LabelLine labelLine30;

	private GridColumnEx gcDocEntry;

	private GridColumnEx gcLineNum;

	private GridColumnEx gcItemCode;

	private GridColumnEx gcBaseQty;

	private GridColumnEx gcPlannedQty;

	private GridColumnEx gcIssuedQty;

	private GridColumnEx gcwareHouse;

	private GridColumnEx gcVisOrder;

	private GridColumnEx gcWipActCode;

	private GridColumnEx gcCompTotal;

	private GridColumnEx gcOcrCode;

	private GridColumnEx gcOcrCode2;

	private GridColumnEx gcOcrCode3;

	private GridColumnEx gcOcrCode4;

	private GridColumnEx gcOcrCode5;

	private GridColumnEx gcLocCode;

	private GridColumnEx gcLogInstanc;

	private GridColumnEx gcLoadFrBOM;

	private GridColumnEx gcProject;

	private GridColumnEx gcUomEntry;

	private GridColumnEx gcUomCode;

	private GridColumnEx gcItemType;

	private GridColumnEx gcAdditQty;

	private GridColumnEx gcLineText;

	private GridColumnEx gcPickStatus;

	private GridColumnEx gcPickQty;

	private GridColumnEx gcPickIdNo;

	private GridColumnEx gcReleaseQty;

	private GridColumnEx gcResAlloc;

	private RepositoryItemButtonEdit rBT_WibActCode;

	private GridColumnEx gcItemName;

	private RepositoryItemButtonEdit rBT_ItemCode;

	private GridColumnEx gcUomName;

	private GridColumnEx gcOnHand;

	private GridColumnEx gcIssueType;

	private GridColumnEx gcAvailable;

	private RepositoryItemButtonEdit rBT_WareH;

	private ContextMenuStrip contextMenuStrip_Line;

	private ToolStripMenuItem deleteToolStripMenuItem;

	private ToolStripMenuItem AddRowToolStripMenuItem;

	private RepositoryItemCalcEdit repositoryItemCalcEdit1;

	private ButtonEditEx txtProject;

	private RepositoryItemButtonEdit rep_bte_lineProject;

	private XtraTabPageEx tab_UDF;

	private GridViewEx gvDoc;

	private SimpleButtonEx btn_OK;

	private SimpleButtonEx btn_Cancel;

	private LookUpEditCombo luk_Doc_Type;

	private LookUpEditCombo luk_Doc_Status;

	private ButtonEditEx cbDistrRule;

	private LookUpEditCombo luk_Doc_User;

	private RepositoryItemLookUpEditCombo rep_Line_Type;

	private RepositoryItemLookUpEditCombo rep_line_IssueMtd;

	private GridColumnEx gcIsCommited;

	private GridColumnEx gcOnOrder;

	private ContextMenuStrip contextMenuStrip_Doc;

	private ToolStripMenuItem cancelToolStripMenuItem;

	private ToolStripMenuItem duplicateToolStripMenuItem;

	private ToolStripMenuItem reportCompletionToolStripMenuItem;

	private LookUpEditCombo look_Series;

	private DateEditEx txtDueDate;

	private DateEditEx txtAtlClosingDate;

	private CalcEditEx calcAtlResourceCost;

	private CalcEditEx calcAtlByProductCost;

	private CalcEditEx calcAtlAdditionalCost;

	private LookUpEditCombo look_OriginType;

	private LabelControl labelControl2;

	private LabelControl labelControl1;

	private LabelControl labelControl4;

	private LabelControl labelControl3;

	private LabelLine lbBranch;

	private LookUpEditCombo txt_Branch;

	private ToolStripSeparator toolStripSeparator1;

	private ToolStripMenuItem viewDiagramToolStripMenuItem;

	private ToolStripSeparator toolStripSeparator2;

	private ToolStripMenuItem issueComponentsToolStripMenuItem;

	private DateEditEx dateStart;

	private LabelLine labelLine1;

	private GridColumnEx gridColumnEx1;

	private GridColumnEx gridColumnEx2;

	private GridColumnEx gridColumnEx3;

	private GridColumnEx gridColumnEx4;

	private GridColumnEx gridColumnEx5;

	private SplitContainerControl spl_LinesBatchSerial;

	private ucBatchAndSerial ucBatchAndSerial1;

	private ToolStripMenuItem batchSerialToolStripMenuItem;

	private CalcEditEx calcPlannedQuantity;

	private CalcEditEx calcRejectedQuantityInfo;

	private CalcEditEx calcCompletedQuantityInfo;

	private CalcEditEx calcPlannedQuantityInfo;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx1;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx2;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx3;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx4;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx5;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx6;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx7;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx8;

	private TextEditExPrice calcAtlItemCost;

	private TextEditExPrice calcAtlProductCost;

	private TextEditExPrice calcTotalVariance;

	private ToolStripMenuItem transferRequestToolStripMenuItem;

	private ToolStripMenuItem componentTransferToolStripMenuItem;

	private ButtonEditCFL txtProductNo;

	private ButtonEditCFL txtProductNo_Item;

	private ButtonEditCFL txtProductNo_Bom;

	private GridControlEx grd_UDF;

	private GridViewEx gv_UDF;

	private RepositoryItemComboBox repositoryItemComboBox3;

	private RepositoryItemButtonEdit repositoryItemButtonEdit2;

	private RepositoryItemCalcEdit repositoryItemCalcEdit2;

	private RepositoryItemButtonEdit repositoryItemButtonEdit3;

	private RepositoryItemComboBox repositoryItemComboBox2;

	private RepositoryItemButtonEdit repositoryItemButtonEdit4;

	private RepositoryItemComboBox repositoryItemComboBox1;

	private RepositoryItemButtonEdit repositoryItemButtonEdit5;

	private XtraTabPageEx tab_Attachments;

	private ToolStripMenuItem relateToolStripMenuItem;

	protected ucAttachment ucAttachment1;

	private GridColumnEx gridColumnEx6;

	private GridColumnEx gridColumnEx7;

	private DataTable OpenFormDocument { get; set; }

	private DataTable OpenFormLines { get; set; }

	private DataTable OpenFormBatchSerial { get; set; }

	private ContextActionMappingImpl[] MappingDocument { get; set; }

	private ContextActionMappingImpl[] MappingLines { get; set; }

	private ContextActionMappingImpl[] MappingBatchSerial { get; set; }

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		_formMode = value;
		if (_formMode == Enums.FormViewModeType.Add)
		{
			txtNo.Text = _newDocNum.ToString();
			_dtIsIssued = new DataTable();
		}
		ucBatchAndSerial1.SetFormViewMode(value);
		switch (_formMode)
		{
		case Enums.FormViewModeType.Find:
			btn_OK.Text = Apzon.Commons.Properties.Resources.ButtonFind;
			btn_OK.Enabled = true;
			btn_Cancel.Enabled = true;
			break;
		case Enums.FormViewModeType.Update:
			btn_OK.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
			btn_OK.Enabled = true;
			btn_Cancel.Enabled = true;
			break;
		case Enums.FormViewModeType.Add:
			btn_OK.Text = Apzon.Commons.Properties.Resources.ButtonAdd;
			btn_OK.Enabled = true;
			btn_Cancel.Enabled = true;
			_isChangedWhenAdd = true;
			break;
		case Enums.FormViewModeType.View:
			btn_OK.Text = Apzon.Commons.Properties.Resources.ButtonOK;
			btn_OK.Enabled = true;
			btn_Cancel.Enabled = true;
			break;
		case Enums.FormViewModeType.Busy:
			btn_OK.Enabled = false;
			btn_Cancel.Enabled = false;
			break;
		default:
			btn_OK.Text = Apzon.Commons.Properties.Resources.ButtonOK;
			btn_OK.Enabled = true;
			btn_Cancel.Enabled = true;
			break;
		}
	}

	public override void CHANGELOG()
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View || FormMode == Enums.FormViewModeType.Update)
			{
				string primaryKey = _currentDocEntry.ToString();
				Enums.ChangeLog changeLog = Enums.ChangeLog.clProductionOrders;
				Apzon.Entities.Models.Common.GetChangeLogParams getChangeLogParams = new Apzon.Entities.Models.Common.GetChangeLogParams();
				getChangeLogParams.PrimaryKey = primaryKey;
				getChangeLogParams.Object = changeLog;
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

	public async Task MODE_FIND()
	{
		try
		{
			if ((string.Compare(btn_OK.Text, "update", ignoreCase: true) != 0 && (string.Compare(btn_OK.Text, "add", ignoreCase: true) != 0 || !_isChangedWhenAdd)) || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				btn_OK.Text = Apzon.Commons.Properties.Resources.ButtonFind;
				FormMode = Enums.FormViewModeType.Find;
				ResetControl();
				SetContextMenu();
				txtNo.Focus();
				grdDoc.DataSource = null;
				grdDoc.RefreshDataSource();
				DataTable dataSourceTable = grd_UDF.DataSourceTable;
				if (dataSourceTable.IsNotNull())
				{
					dataSourceTable.Rows.Clear();
					DataRow row = dataSourceTable.NewRow();
					dataSourceTable.Rows.Add(row);
					dataSourceTable.AcceptChanges();
				}
				grd_UDF.DataSource = dataSourceTable;
				grd_UDF.RefreshDataSource();
				DisableControls("P");
				luk_Doc_Status.SetDataSource(_arrStatus, isStringCode: true);
				this.SetFindMode();
				_first = "firstR";
				_last = "lastR";
				_current = "currentR";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ResetControl()
	{
		txtNo.Enabled = true;
		calcPlannedQuantity.Text = string.Empty;
		txtNo.Text = string.Empty;
		txtProductDesc.Properties.ReadOnly = true;
		luk_Doc_Type.EditValue = string.Empty;
		luk_Doc_Status.EditValue = string.Empty;
		txtProductNo.Text = string.Empty;
		txtProductDesc.Text = string.Empty;
		calcPlannedQuantityInfo.Text = Function.ToString(0);
		txtWarehouse.Text = string.Empty;
		txtWarehouse.Text = string.Empty;
		txtUoMName.Text = string.Empty;
		dateStart.EditValue = string.Empty;
		dateDue.EditValue = string.Empty;
		dateOrder.EditValue = string.Empty;
		luk_Doc_User.EditValue = string.Empty;
		look_OriginType.EditValue = null;
		txtSaleOrder.Text = string.Empty;
		txtCustomer.Text = string.Empty;
		cbDistrRule.Text = string.Empty;
		txtProject.Text = string.Empty;
		memoRemarks.Text = string.Empty;
		memoPickRemarks.Text = string.Empty;
		calcAtlItemCost.Text = string.Empty;
		calcAtlResourceCost.Text = string.Empty;
		calcAtlAdditionalCost.Text = string.Empty;
		calcAtlProductCost.Text = string.Empty;
		calcAtlByProductCost.Text = string.Empty;
		calcTotalVariance.Text = string.Empty;
		txtJournalRemark.Text = string.Empty;
		calcPlannedQuantityInfo.Text = string.Empty;
		calcCompletedQuantityInfo.Text = string.Empty;
		calcRejectedQuantityInfo.Text = string.Empty;
		txtDueDate.EditValue = null;
		txtAtlClosingDate.EditValue = null;
		txtOverDue.Text = string.Empty;
		txtTotalRunTime.Text = string.Empty;
		txtTotalAdditionalTime.Text = string.Empty;
		txtTotalTime.Text = string.Empty;
	}

	public async Task REFRESH()
	{
		try
		{
			if (string.Compare(btn_OK.Text, "add", ignoreCase: true) == 0 || (string.Compare(btn_OK.Text, "update", ignoreCase: true) == 0 && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes))
			{
				return;
			}
			_isLoading = true;
			DataTable tableStructure = WinFuntions.GetTableStructure("OWOR");
			if (tableStructure != null)
			{
				tableStructure.TableName = "OWOR";
				tableStructure.Rows.Clear();
				tableStructure.AcceptChanges();
				DataTable dataTableNewRow = WinFuntions.GetDataTableNewRow(tableStructure.Copy());
				if (dataTableNewRow != null && dataTableNewRow.Columns.Count > 0)
				{
					dataTableNewRow.AcceptChanges();
					DataSource = dataTableNewRow.Copy();
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.CannotGetDatasourceTableStructWithTableName, "", "OWOR");
				}
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.CannotGetDatasourceTableStructWithTableName, "", "OWOR");
			}
			FormMode = Enums.FormViewModeType.Add;
			SetControlsBackColor();
			ResetControl();
			await RefreshData();
			LoadSeriesByPostingDate();
			luk_Doc_User.EditValue = GlobalData.UserID;
			SetContextMenu();
			DisableControls("P");
			txtProductNo.ReadOnly = false;
			txtProductNo.Properties.Buttons[0].Visible = true;
			txtProductNo.BackColor = StyleColor.Editable;
			txtProductDesc.BackColor = StyleColor.GridDisable;
			txtNo.BackColor = StyleColor.GridDisable;
			txtUoMName.BackColor = StyleColor.GridDisable;
			_isChangedWhenAdd = false;
			_first = "firstR";
			_last = "lastR";
			_current = "currentR";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		_isLoading = false;
	}

	public async Task MOVE_FIRST()
	{
		WinCommons._OpenProcessing();
		try
		{
			if (await FindNextLastFrist(Enums.NavigateType.First) == 0)
			{
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
		try
		{
			if (await FindNextLastFrist(Enums.NavigateType.Next) == 0)
			{
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
		try
		{
			if (await FindNextLastFrist(Enums.NavigateType.Last) == 0)
			{
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
		try
		{
			if (await FindNextLastFrist(Enums.NavigateType.Previous) == 0)
			{
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
			if ((string.Compare(btn_OK.Text, "update", ignoreCase: true) == 0 || (string.Compare(btn_OK.Text, "add", ignoreCase: true) == 0 && _isChangedWhenAdd)) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				return 0;
			}
			DataTable dataTable = await _httpClient.ProductionOrders.FindDataNextLast(_currentDocEntry, (int)type);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				txtNo.Text = Function.ToString(dataTable.Rows[0]["DocNum"]);
				look_Series.EditValue = Function.ParseInt(dataTable.Rows[0]["Series"]);
				await ShowData(Function.ParseInt(dataTable.Rows[0]["DocEntry"]));
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		return 0;
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			if (Function.ParseInt(txtNo.EditValue) > 0)
			{
				if ((string.Compare(btn_OK.Text, "update", ignoreCase: true) == 0 || (string.Compare(btn_OK.Text, "add", ignoreCase: true) == 0 && _isChangedWhenAdd)) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
				{
					return;
				}
				_first = "firstR";
				_last = "lastR";
				_current = "currentR";
				WinCommons._OpenProcessing();
				await ShowData(_currentDocEntry);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private void SetFormId()
	{
		FormId = ApzObjectTypes.oProductionOrders.ToString();
		GridViewEx gridViewEx = gvDoc;
		string formId = (ucAttachment1.FormId = FormId);
		gridViewEx.FormId = formId;
		gvDoc.ItemId = "WOR1";
		gv_UDF.FormId = FormId;
		gv_UDF.ItemId = "OWOR";
	}

	public frm_ProductionOrder()
	{
		InitializeComponent();
		SetFormId();
		SettingLanguage();
		OnLoadAsync += frm_ProductionOrder_Load;
	}

	public frm_ProductionOrder(int docEntry)
	{
		DocEntry = docEntry;
		InitializeComponent();
		SetFormId();
		SettingLanguage();
		OnLoadAsync += frm_ProductionOrder_Load;
	}

	public frm_ProductionOrder(int docEntry, int lineFocus)
	{
		DocEntry = docEntry;
		LineNumFocus = lineFocus;
		InitializeComponent();
		SetFormId();
		SettingLanguage();
		OnLoadAsync += frm_ProductionOrder_Load;
	}

	private async Task GetReCord(string position, string docEntry = "0")
	{
		_ = 1;
		try
		{
			if ((string.Compare(btn_OK.Text, "update", ignoreCase: true) == 0 || (string.Compare(btn_OK.Text, "add", ignoreCase: true) == 0 && _isChangedWhenAdd)) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				return;
			}
			DataTable dataTable = await _httpClient.ProductionOrders.GetRecord(position, docEntry);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				txtNo.Text = Function.ToString(dataTable.Rows[0]["DocNum"]);
				look_Series.EditValue = Function.ParseInt(dataTable.Rows[0]["Series"]);
				if (position.ToUpper() == "L")
				{
					_last = Function.ToString(dataTable.Rows[0]["DocEntry"]);
				}
				else if (position.ToUpper() == "F")
				{
					_first = Function.ToString(dataTable.Rows[0]["DocEntry"]);
				}
				_current = Function.ToString(dataTable.Rows[0]["DocEntry"]);
				await ShowData(Function.ParseInt(dataTable.Rows[0]["DocEntry"]));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task frm_ProductionOrder_Load(object sender, EventArgs e)
	{
		gvDoc.ShowLoadingPanel();
		try
		{
			ucBatchAndSerial ucBatchAndSerial2 = ucBatchAndSerial1;
			ucBatchAndSerial2.DBatchSerialChanged = (DataBatchSerialChanged)Delegate.Combine(ucBatchAndSerial2.DBatchSerialChanged, new DataBatchSerialChanged(BatchSerialDataChanged));
			InitChangeDataEvent();
			if (painter == null)
			{
				painter = new GridViewExPainter(gvDoc);
			}
			new ProductionOder_LineImpl_WOR1().DocEntry = 0;
			Task<int> activeDimensionCount = GetActiveDimensionCount();
			Dictionary<string, GridViewEx> tblFormular = new Dictionary<string, GridViewEx> { ["OWOR"] = gv_UDF };
			tblFormular["WOR1"] = gvDoc;
			Task T4 = WinFuntions.LoadUdfColumns(gvDoc, "WOR1", tblFormular);
			Task T5 = WinFuntions.LoadUdfColumns(gv_UDF, "OWOR", tblFormular);
			_dtSearch = ProductionOrderTableHelper.ProductionHeader.Copy();
			if (!_dtSearch.Columns.Contains("AtcEntry"))
			{
				tab_Attachments.PageVisible = false;
			}
			_dtSearchLine = ProductionOrderTableHelper.ProductionLines.Copy();
			_activeDimCount = await activeDimensionCount;
			await T4;
			await T5;
			List<Formular> formulars = await WinFuntions.GetFormulas(202);
			gv_UDF.InitFormular(formulars, tblFormular);
			gvDoc.InitFormular(formulars, tblFormular);
			if (DocumentsHelper.BatchSerialNumbersStruct != null)
			{
				ucBatchAndSerial1.DataSourceBatchAndSerial = DocumentsHelper.BatchSerialNumbersStruct.Copy();
			}
			if (DocumentsHelper.BatchSerialNumbersStruct != null)
			{
				ucBatchAndSerial1.DataSourceBinAllowLine = DocumentsHelper.BatchSerialNumbersStruct.Copy();
			}
			btn_OK.Text = Apzon.Commons.Properties.Resources.ButtonOK;
			ucAttachment ucAttachment2 = ucAttachment1;
			ucAttachment2.BrowsedFile = (Action<List<DataRow>>)Delegate.Combine(ucAttachment2.BrowsedFile, new Action<List<DataRow>>(Attachment_BrowseFile));
			ucAttachment1.gv_File.CellValueChanged += Control_EditValueChanged;
			if (DocEntry != 0)
			{
				await LoadCb();
				await ShowData(DocEntry);
			}
			else
			{
				await REFRESH();
				if (_openFormViewMode == Enums.FormViewModeType.Add)
				{
					FormMode = Enums.FormViewModeType.Add;
					if (OpenFormDocument.IsNotNull())
					{
						_isMapping = true;
						ContextActionMappingImpl mapping = MappingDocument.FirstOrDefault((ContextActionMappingImpl t) => t.TargetField == "ItemCode");
						txtProductNo.EditValue = ContextActionMappingSource.GetValueSource(OpenFormDocument.Rows[0], mapping);
						txtProductNo.Focus();
						await ChooseFormList_Item(isLoad: true);
						calcPlannedQuantity.Focus();
						mapping = MappingDocument.FirstOrDefault((ContextActionMappingImpl t) => t.TargetField == "PlannedQty");
						calcPlannedQuantity.EditValue = ContextActionMappingSource.GetValueSource(OpenFormDocument.Rows[0], mapping, typeof(double));
						txtWarehouse.Focus();
						foreach (ContextActionMappingImpl item in MappingDocument.Where((ContextActionMappingImpl t) => !new string[2] { "ItemCode", "PlannedQty" }.Contains(t.TargetField) && !t.TargetField.StartsWith("U_")))
						{
							switch (item.TargetField)
							{
							case "Type":
								luk_Doc_Type.EditValue = ContextActionMappingSource.GetValueSource(OpenFormDocument.Rows[0], item, typeof(string));
								break;
							case "Warehouse":
								txtWarehouse.EditValue = ContextActionMappingSource.GetValueSource(OpenFormDocument.Rows[0], item, typeof(string));
								break;
							case "CardCode":
								txtCustomer.EditValue = ContextActionMappingSource.GetValueSource(OpenFormDocument.Rows[0], item, typeof(string));
								break;
							case "PostDate":
								dateOrder.EditValue = ContextActionMappingSource.GetValueSource(OpenFormDocument.Rows[0], item, typeof(DateTime));
								break;
							case "StartDate":
								dateStart.EditValue = ContextActionMappingSource.GetValueSource(OpenFormDocument.Rows[0], item, typeof(DateTime));
								break;
							case "DueDate":
								dateDue.EditValue = ContextActionMappingSource.GetValueSource(OpenFormDocument.Rows[0], item, typeof(DateTime));
								break;
							case "Uom":
								txtCustomer.EditValue = ContextActionMappingSource.GetValueSource(OpenFormDocument.Rows[0], item, typeof(string));
								break;
							case "Series":
								look_Series.EditValue = ContextActionMappingSource.GetValueSource(OpenFormDocument.Rows[0], item, typeof(string));
								break;
							case "OriginType":
								look_OriginType.EditValue = ContextActionMappingSource.GetValueSource(OpenFormDocument.Rows[0], item, typeof(string));
								break;
							case "BplId":
								txt_Branch.EditValue = ContextActionMappingSource.GetValueSource(OpenFormDocument.Rows[0], item, typeof(string));
								break;
							case "OriginNum":
								txtSaleOrder.EditValue = ContextActionMappingSource.GetValueSource(OpenFormDocument.Rows[0], item, typeof(string));
								txtSaleOrder.CausesValidation = true;
								txtSaleOrder.DoValidate();
								ValidateChildren();
								break;
							case "Project":
								txtProject.EditValue = ContextActionMappingSource.GetValueSource(OpenFormDocument.Rows[0], item, typeof(string));
								break;
							case "Comments":
								memoRemarks.EditValue = ContextActionMappingSource.GetValueSource(OpenFormDocument.Rows[0], item, typeof(string));
								break;
							case "PickRmrk":
								memoPickRemarks.EditValue = ContextActionMappingSource.GetValueSource(OpenFormDocument.Rows[0], item, typeof(string));
								break;
							case "JrnlMemo":
								txtJournalRemark.EditValue = ContextActionMappingSource.GetValueSource(OpenFormDocument.Rows[0], item, typeof(string));
								break;
							}
						}
						string text = "";
						for (int num = 0; num < 5; num++)
						{
							string targetFiled = "OcrCode" + ((num > 0) ? Function.ToString(num + 1) : "");
							ContextActionMappingImpl contextActionMappingImpl = MappingDocument.FirstOrDefault((ContextActionMappingImpl t) => t.TargetField.Equals(targetFiled));
							if (contextActionMappingImpl != null)
							{
								string text2 = Function.ToString(ContextActionMappingSource.GetValueSource(OpenFormDocument.Rows[0], contextActionMappingImpl, typeof(string)));
								if (!string.IsNullOrEmpty(text2))
								{
									text = text + text2 + ";";
									continue;
								}
							}
							text += ";";
						}
						while (text.Length > 0 && text[text.Length - 1].Equals(';'))
						{
							text = text.Remove(text.Length - 1);
						}
						cbDistrRule.EditValue = text;
						ContextActionMappingImpl[] array = MappingDocument.Where((ContextActionMappingImpl t) => !new string[2] { "ItemCode", "PlannedQty" }.Contains(t.TargetField) && t.TargetField.StartsWith("U_")).ToArray();
						DataTable dataTable = ProductionOrderTableHelper.ProductionHeader.DefaultView.ToTable(distinct: true, (from DataColumn t in ProductionOrderTableHelper.ProductionHeader.Columns
							where t.ColumnName.StartsWith("U_")
							select t.ColumnName).ToArray());
						DataRow dataRow = dataTable.NewRow();
						ContextActionMappingImpl[] array2 = array;
						foreach (ContextActionMappingImpl contextActionMappingImpl2 in array2)
						{
							dataRow[contextActionMappingImpl2.TargetField] = ContextActionMappingSource.GetValueSource(OpenFormDocument.Rows[0], contextActionMappingImpl2, typeof(string));
						}
						dataTable.Rows.Add(dataRow);
						dataTable.AcceptChanges();
						grd_UDF.DataSource = dataTable;
						grd_UDF.RefreshDataSource();
						if (MappingLines.Length != 0)
						{
							grdDoc.DataSourceTable.Rows.Clear();
							grdDoc.DataSourceTable.AcceptChanges();
							await AddNewRow();
							gvDoc.MappingSource(OpenFormLines, MappingLines);
						}
						_isMapping = false;
					}
				}
			}
			await WinFuntions.CreateColumnsDistRule(gvDoc);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
			_isMapping = false;
		}
		gvDoc.HideLoadingPanel();
	}

	private void gvDoc_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			ucBatchAndSerial1.FinishEditorBatchAndSerial();
			ucBatchAndSerial1.SetFocusLine();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void batchSerialToolStripMenuItem_Click(object sender, EventArgs e)
	{
		spl_LinesBatchSerial.PanelVisibility = ((spl_LinesBatchSerial.PanelVisibility != SplitPanelVisibility.Panel1) ? SplitPanelVisibility.Panel1 : SplitPanelVisibility.Both);
	}

	private void Control_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!(sender is CalcEditEx { Name: "txtOpenQty" }))
			{
				CheckChangedData();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gvDoc_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			gvDoc.DeleteSelectedRows();
			grdDoc.DataSourceTable.AcceptChanges();
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

	private async void AddNewToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			await AddNewRow();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btCancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private async void btOK_Click(object sender, EventArgs e)
	{
		btn_OK.Enabled = false;
		try
		{
			if (btn_OK.Text == Apzon.Commons.Properties.Resources.ButtonAdd || btn_OK.Text == Apzon.Commons.Properties.Resources.ButtonUpdate)
			{
				if (!ValidateDataSave())
				{
					btn_OK.Enabled = true;
					return;
				}
				if (!GlobalData.LoginSap)
				{
					await SaveDataSet();
				}
				else
				{
					await Save();
				}
			}
			else if (btn_OK.Text == Apzon.Commons.Properties.Resources.ButtonFind)
			{
				await Search();
			}
			else if (btn_OK.Text == Apzon.Commons.Properties.Resources.ButtonOK)
			{
				Close();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		btn_OK.Enabled = true;
	}

	private async void txtProductNo_Properties_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		_ = 1;
		try
		{
			if (e.Button.Index == 0)
			{
				await ChooseFormList_Item();
			}
			if (e.Button.Index == 1)
			{
				ItemCode = Function.ToString(txtProductNo.Text);
				if (!string.IsNullOrEmpty(ItemCode))
				{
					this.DrillDown(ApzObjectTypes.oProductionOrders, ((await frm_BillOfMaterials.GetBoMByItemCode(ItemCode)).Rows.Count > 0) ? ApzObjectTypes.oProductTrees : ApzObjectTypes.oItems, ItemCode);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void txtWarehouse_Properties_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		if (e.Button.Index == 0)
		{
			ChooseFromListWarehouse();
		}
		if (e.Button.Index == 1)
		{
			WareH = Function.ToString(txtWarehouse.Text);
			if (!string.IsNullOrEmpty(WareH))
			{
				this.DrillDown(ApzObjectTypes.oProductionOrders, ApzObjectTypes.oWarehouses, WareH);
			}
		}
	}

	private void txtCustomer_Properties_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				ChooseFormList_Customer();
			}
			if (e.Button.Index == 1)
			{
				string text = txtCustomer.Text;
				if (!string.IsNullOrEmpty(text))
				{
					this.DrillDown(ApzObjectTypes.oProductionOrders, ApzObjectTypes.oBusinessPartners, text);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ChooseFormList_Customer()
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.Customer, false, txtCustomer.Text, "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem != null)
			{
				txtCustomer.Text = Function.ToString(frm_CFL2.ChooseItem["CardCode"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void txtSaleOrder_Properties_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			string obj = txtSaleOrder.Text;
			if (e.Button.Index == 0)
			{
				ChooseFromListSales();
			}
			if (e.Button.Index == 1 && Function.ParseInt(obj) > 0)
			{
				this.DrillDown(ApzObjectTypes.oOrders, ApzObjectTypes.oOrders, GetSalesOrderEntry(Function.ParseInt(obj)).ToString());
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, Keys keyData)
	{
		if (msg.Msg == 256 || msg.Msg == 260)
		{
			switch (keyData)
			{
			case Keys.Tab:
				if (gvDoc.IsFocusedView || (FormMode != Enums.FormViewModeType.Add && FormMode != Enums.FormViewModeType.Update && FormMode != Enums.FormViewModeType.View))
				{
					break;
				}
				switch (_lastFocusLeave)
				{
				case "txtProductNo":
					if (string.IsNullOrEmpty(txtProductNo.Text.Trim()))
					{
						ChooseFormList_Item();
					}
					break;
				case "txtWarehouse":
					if (string.IsNullOrEmpty(Function.ToString(txtWarehouse.EditValue)))
					{
						ChooseFromListWarehouse();
					}
					break;
				case "txtCustomer":
					if (string.IsNullOrEmpty(Function.ToString(txtCustomer.EditValue)))
					{
						ChooseFormList_Customer();
					}
					break;
				case "txtProject":
					if (string.IsNullOrEmpty(Function.ToString(txtProject.EditValue)))
					{
						ChooseFromListProject();
					}
					break;
				case "txtSaleOrder":
					if (string.IsNullOrEmpty(Function.ToString(txtSaleOrder.EditValue)))
					{
						ChooseFromListSales();
					}
					break;
				}
				_lastFocusLeave = "";
				break;
			case Keys.Return:
				if (btn_OK.Text == Apzon.Commons.Properties.Resources.ButtonUpdate || btn_OK.Text == Apzon.Commons.Properties.Resources.ButtonAdd)
				{
					Save();
				}
				break;
			}
		}
		return base.ProcessCmdKey(ref msg, keyData);
	}

	private void contextMenuStrip_Line_Opening(object sender, CancelEventArgs e)
	{
		try
		{
			deleteToolStripMenuItem.Visible = CheckEditable("ItemCode", gvDoc.FocusedRowHandle);
			AddRowToolStripMenuItem.Visible = CheckEditable("ItemCode", gvDoc.FocusedRowHandle);
			string text = Function.ToString(gvDoc.GetFocusedRowCellValue("ManBtchSerial"));
			batchSerialToolStripMenuItem.Enabled = text.Equals("S") || text.Equals("B");
			batchSerialToolStripMenuItem.Text = ((spl_LinesBatchSerial.PanelVisibility == SplitPanelVisibility.Both) ? "Hide Batch/Serial Numbers" : "Batch/Serial Numbers");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private bool CheckEditable(string fieldName, int rowhandle)
	{
		try
		{
			if (FormAuth == Enums.FormAuthorities.Read)
			{
				return false;
			}
			GridColumn gridColumn = gvDoc.Columns[fieldName];
			if (gridColumn == null)
			{
				return false;
			}
			if (!new string[2] { "P", "R" }.Contains(CurrentDocStatus))
			{
				return false;
			}
			if (gridColumn.OptionsColumn.ReadOnly)
			{
				return false;
			}
			double num = Function.ParseDouble(gvDoc.GetRowCellValue(rowhandle, "IssuedQty"));
			switch (fieldName)
			{
			case "ItemCode":
			case "ItemName":
			case "Type":
				if (num > 0.0)
				{
					return false;
				}
				break;
			}
			string text = Function.ToString(gvDoc.GetRowCellValue(rowhandle, "IssueType"));
			double num2 = Function.ParseDouble(calcCompletedQuantityInfo.EditValue);
			if ((fieldName == "PlannedQty" || fieldName == "BaseQty") && num2 > 0.0 && text == "B")
			{
				return false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		return true;
	}

	private void gvDoc_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (!CheckEditable(e.Column.FieldName, e.RowHandle))
			{
				e.Appearance.BackColor = StyleColor.GridDisable;
			}
			if (e.Column.FieldName == "StartDate" || e.Column.FieldName == "EndDate")
			{
				DateTime? dateTime = Function.ParseDateTime(e.CellValue);
				if (dateTime.HasValue && dateTime.Value.Date < DateTime.Today)
				{
					e.Appearance.ForeColor = Color.Red;
				}
				else
				{
					e.Appearance.ForeColor = Color.Black;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void txtProject_Properties_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			ChooseFromListProject();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rep_bte_lineProject_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			ChooseFromListProject(isGrid: true);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gvDoc_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
	{
		contextMenuStrip_Line.Show(grdDoc, e.Point);
	}

	public void SetContextMenu()
	{
		try
		{
			DataTable dt = grdDoc.DataSource as DataTable;
			if (btn_OK.Text == Apzon.Commons.Properties.Resources.ButtonAdd || btn_OK.Text == Apzon.Commons.Properties.Resources.ButtonFind)
			{
				if (dt.IsNotNull())
				{
					AddRowToolStripMenuItem.Visible = false;
					deleteToolStripMenuItem.Visible = false;
				}
				else if (dt.IsNotNull())
				{
					deleteToolStripMenuItem.Visible = true;
				}
			}
			else if (btn_OK.Text == Apzon.Commons.Properties.Resources.ButtonUpdate && dt.IsNotNull())
			{
				AddRowToolStripMenuItem.Visible = true;
				deleteToolStripMenuItem.Visible = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public void SetControlValue()
	{
		WinFuntions.SetControlsValueAndType_Empty(Controls);
	}

	private void comboBoxEdit1_Properties_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			string value = Function.ToString(cbDistrRule.EditValue);
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.DistributeRule, multiSelect: false, "");
			frm_ChooseFromList2._strOcrDistRule = Function.ToString(cbDistrRule.EditValue);
			frm_ChooseFromList2.ShowDialog(this);
			DataTable dtSource = frm_ChooseFromList2._dtSource;
			string text = "";
			if (dtSource != null && dtSource.Rows.Count > 0)
			{
				for (int i = 0; i < dtSource.Rows.Count; i++)
				{
					text = text + Function.ToString(dtSource.Rows[i]["OcrCode"]) + ";";
				}
				if (text.Length > 1)
				{
					text = text.Remove(text.Length - 1);
				}
			}
			string value2 = text.Replace(";", "");
			if (string.IsNullOrWhiteSpace(value) || !string.IsNullOrWhiteSpace(value2))
			{
				while (text.Length > 0 && text[text.Length - 1].Equals(';'))
				{
					text = text.Remove(text.Length - 1);
				}
				cbDistrRule.EditValue = text;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rBT_WibActCode_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		if (e.Button.Index == 0)
		{
			ChooseFromListWibActCode();
			if (FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		if (e.Button.Index == 1)
		{
			string.IsNullOrEmpty(Function.ToString(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, gcWipActCode)));
		}
	}

	private void rBT_ItemCode_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		string text = Function.ToString(gvDoc.GetFocusedRowCellValue("ItemType"));
		if (e.Button.Index == 0 && text != "-18")
		{
			if (!CheckEditable("ItemCode", gvDoc.FocusedRowHandle))
			{
				return;
			}
			ChooseListColumItem();
			ucBatchAndSerial1.SetFocusLine(isChangeItem: true);
		}
		if (e.Button.Index != 1)
		{
			return;
		}
		string text2 = Function.ToString(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, gcItemCode));
		if (string.IsNullOrEmpty(text2))
		{
			return;
		}
		if (text == "4")
		{
			this.DrillDown(ApzObjectTypes.oProductionOrders, ApzObjectTypes.oItems, text2);
			return;
		}
		if (text == "290")
		{
			this.DrillDown(ApzObjectTypes.oProductionOrders, ApzObjectTypes.WorkCenter, text2);
			return;
		}
		frm_RowTextDetails frm_RowTextDetails2 = new frm_RowTextDetails();
		frm_RowTextDetails2.RowText = Function.ToString(gvDoc.GetFocusedRowCellValue("ItemCode"));
		if (frm_RowTextDetails2.ShowDialog(this) == DialogResult.OK)
		{
			gvDoc.SetRowCellValue(gvDoc.FocusedRowHandle, "ItemCode", frm_RowTextDetails2.RowText);
		}
	}

	private bool ChooseListColumItem()
	{
		try
		{
			string txtSeach = Function.ToString(gvDoc.GetFocusedRowCellValue(gcItemCode));
			int num = Function.ParseInt(gvDoc.GetFocusedRowCellValue("ItemType"));
			Enums.TypeCFL type = Enums.TypeCFL.ListItems;
			if (num == 290)
			{
				type = Enums.TypeCFL.ListMasterResource;
			}
			frm_CFL frm_CFL2 = new frm_CFL(type, true, txtSeach, "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ListMandatoryField = "ManBatchSerial";
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItems != null)
			{
				if (frm_CFL2.ChooseItems.Count <= 0)
				{
					return false;
				}
				List<DataRow> chooseItems = frm_CFL2.ChooseItems;
				DataTable dataSourceTable = grdDoc.DataSourceTable;
				if (num == 290)
				{
					DataRow dataRow = chooseItems[0];
					gvDoc.SetFocusedRowCellValue("ItemCode", dataRow["ResCode"]);
					for (int i = 1; i < chooseItems.Count; i++)
					{
						DataRow dataRow2 = chooseItems[i];
						DataRow dataRow3 = dataSourceTable.NewRow();
						dataRow3["ItemType"] = num;
						dataRow3["BaseQty"] = 1;
						object obj = (dataRow3["PlannedQty"] = 1m);
						object value = (dataRow3["OldFatherQty"] = obj);
						dataRow3["OldLineQty"] = value;
						dataRow3["IssueType"] = "B";
						dataRow3["UomCode"] = "Manual";
						_lineNum = dataSourceTable.AsEnumerable().Max((DataRow x) => Function.ParseInt(x["LineNum"])) + 1;
						dataRow3["LineNum"] = _lineNum;
						dataSourceTable.Rows.Add(dataRow3);
						dataSourceTable.AcceptChanges();
						grdDoc.DataSource = dataSourceTable;
						int rowHandle = gvDoc.FindRowHandleByDataRow(dataRow3);
						gvDoc.SetRowCellValue(rowHandle, "ItemCode", dataRow2["ResCode"]);
					}
				}
				else
				{
					DataRow dataRow4 = dataSourceTable.Rows[gvDoc.GetFocusedDataSourceRowIndex()];
					DataRow dataRow5 = chooseItems[0];
					dataRow4["UomCode"] = "Manual";
					dataRow4["BaseQty"] = 1;
					object obj = (dataRow4["PlannedQty"] = 1m);
					object value = (dataRow4["OldFatherQty"] = obj);
					dataRow4["OldLineQty"] = value;
					dataRow4["UomCode"] = "Manual";
					dataRow4["ManBtchSerial"] = dataRow5["ManBatchSerial"];
					gvDoc.SetFocusedRowCellValue("ItemCode", dataRow5["ItemCode"]);
					for (int num2 = 1; num2 < chooseItems.Count; num2++)
					{
						DataRow dataRow6 = chooseItems[num2];
						DataRow dataRow7 = dataSourceTable.NewRow();
						dataRow7["IssueType"] = Function.ToString(dataRow6["IssueMthd"]);
						dataRow7["ItemType"] = num;
						dataRow7["BaseQty"] = 1;
						obj = (dataRow7["PlannedQty"] = 1m);
						value = (dataRow7["OldFatherQty"] = obj);
						dataRow7["OldLineQty"] = value;
						dataRow7["UomCode"] = "Manual";
						_lineNum = dataSourceTable.AsEnumerable().Max((DataRow x) => Function.ParseInt(x["LineNum"])) + 1;
						dataRow7["LineNum"] = _lineNum;
						dataRow7["ManBtchSerial"] = dataRow6["ManBatchSerial"];
						dataSourceTable.Rows.Add(dataRow7);
						dataSourceTable.AcceptChanges();
						grdDoc.DataSource = dataSourceTable;
						int rowHandle2 = gvDoc.FindRowHandleByDataRow(dataRow7);
						gvDoc.SetRowCellValue(rowHandle2, "ItemCode", dataRow6["ItemCode"]);
					}
				}
				dataSourceTable.AcceptChanges();
				if (btn_OK.Text == Apzon.Commons.Properties.Resources.ButtonAdd)
				{
					for (int num3 = 0; num3 < dataSourceTable.Rows.Count; num3++)
					{
						dataSourceTable.Rows[num3]["LineNum"] = num3;
					}
				}
				List<DataRow> list = new List<DataRow>();
				for (int num4 = 0; num4 < dataSourceTable.Rows.Count; num4++)
				{
					if (string.IsNullOrEmpty(Function.ToString(dataSourceTable.Rows[num4]["ItemCode"])))
					{
						list.Add(dataSourceTable.Rows[num4]);
					}
				}
				for (int num5 = 0; num5 < list.Count; num5++)
				{
					dataSourceTable.Rows.Remove(list[num5]);
				}
				dataSourceTable.AcceptChanges();
				grdDoc.DataSource = dataSourceTable;
				_oldTxtCodeValue = txtProductNo.Text;
				grdDoc.RefreshDataSource();
				gvDoc.HideLoadingPanel();
				AddNewRow();
				if (FormMode == Enums.FormViewModeType.View)
				{
					FormMode = Enums.FormViewModeType.Update;
				}
				return true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		return false;
	}

	private void rBT_WareH_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				ChooseFromListWareH();
			}
			if (e.Button.Index == 1 && !string.IsNullOrEmpty(WareH))
			{
				this.DrillDown(ApzObjectTypes.oProductionOrders, ApzObjectTypes.oWarehouses, WareH);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void gvDoc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			CheckChangedData();
			if (e.Column.FieldName.ToLower() == "itemcode")
			{
				if (!_isPasteFromExcel)
				{
					_curRowtype = Function.ParseInt(gvDoc.GetFocusedRowCellValue("ItemType"));
				}
				LoadItemDetails(e.RowHandle);
				await AddNewRow();
				ucBatchAndSerial1.SetFocusLine(isChangeItem: true);
			}
			decimal num = Function.ParseDecimal(calcPlannedQuantity.EditValue);
			if (e.Column.FieldName.ToLower() == "baseqty" && !_isChangeQty)
			{
				_isChangeQty = true;
				if (num == 0m)
				{
					gvDoc.SetRowCellValue(e.RowHandle, "PlannedQty", 0);
				}
				else
				{
					gvDoc.SetRowCellValue(e.RowHandle, "PlannedQty", Function.ParseDecimal(e.Value) * num);
				}
				CalculateBaseRatio(Function.ParseDecimal(e.Value), 1m, out var numerator, out var denominator);
				gvDoc.SetRowCellValue(e.RowHandle, "OldLineQty", numerator);
				gvDoc.SetRowCellValue(e.RowHandle, "OldFatherQty", denominator);
				_isChangeQty = false;
			}
			if (e.Column.FieldName.ToLower() == "plannedqty" && !_isChangeQty)
			{
				_isChangeQty = true;
				if (num == 0m)
				{
					gvDoc.SetRowCellValue(e.RowHandle, "BaseQty", 0);
				}
				else
				{
					gvDoc.SetRowCellValue(e.RowHandle, "BaseQty", decimal.Round(Function.ParseDecimal(e.Value) / num, 6));
				}
				CalculateBaseRatio(Function.ParseDecimal(e.Value), Function.ParseDecimal(calcPlannedQuantity.EditValue), out var numerator2, out var denominator2);
				gvDoc.SetRowCellValue(e.RowHandle, "OldLineQty", numerator2);
				gvDoc.SetRowCellValue(e.RowHandle, "OldFatherQty", denominator2);
				_isChangeQty = false;
			}
			if (!(e.Column.FieldName.ToLower() == "warehouse"))
			{
				return;
			}
			string text = Function.ToString(gvDoc.GetRowCellValue(e.RowHandle, "Code"));
			string whsCode = Function.ToString(gvDoc.GetRowCellValue(e.RowHandle, "wareHouse"));
			if (!string.IsNullOrWhiteSpace(text) && !string.IsNullOrWhiteSpace(text))
			{
				DataTable oITWQuantity = GetOITWQuantity(text, whsCode);
				if (oITWQuantity.IsNotNull())
				{
					gvDoc.SetRowCellValue(e.RowHandle, "IsCommitedInWhse", Function.ParseDecimal(oITWQuantity.Rows[0]["IsCommited"]));
					gvDoc.SetRowCellValue(e.RowHandle, "OnOrderInWhse", Function.ParseDecimal(oITWQuantity.Rows[0]["OnOrder"]));
					gvDoc.SetRowCellValue(e.RowHandle, "OnHandInWhse", Function.ParseDecimal(oITWQuantity.Rows[0]["OnHand"]));
					gvDoc.SetRowCellValue(e.RowHandle, "Available", Function.ParseDecimal(oITWQuantity.Rows[0]["Available"]));
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void calcPlannedQuantity_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!calcPlannedQuantity.ContainsFocus || _isLoading)
			{
				return;
			}
			decimal num = Function.ParseDecimal(calcPlannedQuantity.EditValue);
			for (int i = 0; i < gvDoc.DataRowCount; i++)
			{
				Function.ParseDecimal(gvDoc.GetRowCellValue(i, "BaseQty"));
				decimal num2 = Function.ParseDecimal(gvDoc.GetRowCellValue(i, "OldLineQty"));
				decimal num3 = Function.ParseDecimal(gvDoc.GetRowCellValue(i, "OldFatherQty"));
				_isChangeQty = true;
				try
				{
					gvDoc.SetRowCellValue(i, "PlannedQty", num * num2 / num3);
				}
				catch (Exception)
				{
				}
				_isChangeQty = false;
			}
			grdDoc.DataSourceTable.AcceptChanges();
			calcPlannedQuantity.Focus();
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", ex2);
			WinCommons.ErrorLineNumber(ex2);
		}
	}

	private async void cancelToolStripMenuItem_Click(object sender, EventArgs e)
	{
		btn_OK.Enabled = false;
		try
		{
			string text = await _httpClient.ProductionOrders.CancelDoc(_currentDocEntry);
			if (string.IsNullOrWhiteSpace(text))
			{
				WinCommons.Success(Enums.MessageCode.CancelSuccessWithDocNo, "", Function.ToString(_currentDocNum));
				await ShowData(_currentDocEntry);
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		btn_OK.Enabled = true;
	}

	private async void duplicateToolStripMenuItem1_Click(object sender, EventArgs e)
	{
		btn_OK.Enabled = false;
		try
		{
			string type = Function.ToString(luk_Doc_Type.EditValue);
			string product = txtProductNo.Text;
			string desc = txtProductDesc.Text;
			double qty = Function.ParseDouble(calcPlannedQuantity.EditValue);
			string uom = txtUoMName.Text;
			string whs = txtWarehouse.Text;
			string user = Function.ToString(luk_Doc_User.EditValue);
			string order = txtSaleOrder.Text;
			string customer = txtCustomer.Text;
			string distRule = Function.ToString(cbDistrRule.EditValue);
			string project = txtProject.Text;
			int series = Function.ParseInt(look_Series.EditValue);
			DataTable dt = grdDoc.DataSourceTable.Copy();
			FormMode = Enums.FormViewModeType.Add;
			_currentDocNum = await _httpClient.ProductionOrders.GetDocNum(series);
			txtNo.Text = _currentDocNum.ToString();
			if (dt.IsNotNull())
			{
				foreach (DataRow row in dt.Rows)
				{
					row["DocEntry"] = 0;
					row["IssuedQty"] = 0;
					_currentDocEntry = 0;
					CurrentDocStatus = "P";
				}
				dt.AcceptChanges();
				grdDoc.DataSource = dt;
				_oldTxtCodeValue = txtProductNo.Text;
				grdDoc.RefreshDataSource();
				gvDoc.OptionsBehavior.Editable = true;
				string[] array = new string[8] { "IssuedQty", "UomCode", "ItemName", "InvntryUom", "OnHand", "OnOrder", "OnOrder", "IsCommited" };
				for (int i = 0; i < array.Length; i++)
				{
					if (gvDoc.Columns[array[i]] != null)
					{
						gvDoc.Columns[array[i]].OptionsColumn.ReadOnly = true;
						gvDoc.Columns[array[i]].OptionsColumn.AllowEdit = false;
					}
				}
			}
			luk_Doc_Status.SetDataSource(new string[1, 2] { { "P", "Planned" } }, isStringCode: true);
			luk_Doc_Type.EditValue = type;
			luk_Doc_Type.Enabled = true;
			luk_Doc_Status.EditValue = "P";
			luk_Doc_Status.Enabled = true;
			txtProductNo.Text = product;
			txtProductNo.Enabled = true;
			txtProductDesc.Text = desc;
			calcPlannedQuantity.EditValue = qty;
			calcPlannedQuantity.Enabled = true;
			txtUoMName.Text = uom;
			txtWarehouse.Text = whs;
			txtWarehouse.Enabled = true;
			luk_Doc_User.EditValue = user;
			luk_Doc_User.Enabled = true;
			txtSaleOrder.Text = order;
			txtSaleOrder.Enabled = true;
			txtCustomer.Text = customer;
			txtCustomer.Enabled = true;
			cbDistrRule.EditValue = distRule;
			cbDistrRule.Enabled = true;
			txtProject.Text = project;
			txtProject.Enabled = true;
			dateOrder.Enabled = true;
			dateOrder.EditValue = DateTime.Today;
			dateStart.Enabled = true;
			dateStart.EditValue = DateTime.Today;
			dateDue.Enabled = true;
			dateDue.EditValue = DateTime.Today;
			luk_Doc_Type.Properties.ReadOnly = false;
			luk_Doc_Status.Properties.ReadOnly = false;
			txtProductNo.Properties.ReadOnly = false;
			txtProductDesc.Properties.ReadOnly = false;
			calcPlannedQuantity.Properties.ReadOnly = false;
			txtWarehouse.Properties.ReadOnly = false;
			dateOrder.Properties.ReadOnly = false;
			dateStart.Properties.ReadOnly = false;
			dateDue.Properties.ReadOnly = false;
			luk_Doc_User.Properties.ReadOnly = false;
			txtSaleOrder.Properties.ReadOnly = false;
			txtCustomer.Properties.ReadOnly = false;
			memoRemarks.Properties.ReadOnly = false;
			memoPickRemarks.Properties.ReadOnly = false;
			gv_UDF.OptionsBehavior.Editable = true;
			DateTime? dateTime = Function.ParseDateTime(dateStart.EditValue);
			DateTime? dateTime2 = Function.ParseDateTime(dateDue.EditValue);
			for (int j = 0; j < gvDoc.DataRowCount; j++)
			{
				gvDoc.SetRowCellValue(j, "StartDate", dateTime);
				gvDoc.SetRowCellValue(j, "EndDate", dateTime2);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		btn_OK.Enabled = true;
	}

	private void gvDoc_ShowingEditor(object sender, CancelEventArgs e)
	{
	}

	private void contextMenuStrip_Doc_Opening(object sender, CancelEventArgs e)
	{
		try
		{
			if (CurrentDocStatus == "C" || CurrentDocStatus == "L")
			{
				cancelToolStripMenuItem.Enabled = false;
				if (FormMode == Enums.FormViewModeType.Add)
				{
					duplicateToolStripMenuItem.Enabled = false;
				}
				else
				{
					duplicateToolStripMenuItem.Enabled = true;
				}
			}
			else
			{
				cancelToolStripMenuItem.Enabled = true;
				duplicateToolStripMenuItem.Enabled = true;
			}
			if (Function.ToString(luk_Doc_Type.EditValue) == "D")
			{
				issueComponentsToolStripMenuItem.Text = "Receipt Components";
			}
			else
			{
				issueComponentsToolStripMenuItem.Text = "Issue Components";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void look_Series_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			int series = Function.ParseInt(look_Series.EditValue);
			if (!(btn_OK.Text != Apzon.Commons.Properties.Resources.ButtonAdd))
			{
				_currentDocNum = await _httpClient.ProductionOrders.GetDocNum(series);
				txtNo.EditValue = _currentDocNum;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void grdDoc_EditorKeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.V && e.Modifiers == Keys.Control)
			{
				_curRowtype = Function.ParseInt(gvDoc.GetFocusedRowCellValue("ItemType"));
				PasteFromExcelToDataTable(grdDoc, gvDoc, new List<string> { "ItemCode" }, _curRowtype);
				e.SuppressKeyPress = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void reportCompletionToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (_currentDocEntry <= 0)
			{
				return;
			}
			if (CurrentDocStatus != "R")
			{
				WinCommons.Error(Enums.MessageCode.CannotReportCompletionChangeProductionToRelease, "");
				return;
			}
			switch (Function.ToString(luk_Doc_Type.EditValue))
			{
			case "S":
			case "P":
			{
				frm_ReceiptFromProduction frm_ReceiptFromProduction2 = new frm_ReceiptFromProduction(_currentDocEntry, 202);
				frm_ReceiptFromProduction2.MdiParent = MdiParent;
				frm_ReceiptFromProduction2.Show();
				break;
			}
			case "D":
			{
				frm_IssueForProduction frm_IssueForProduction2 = new frm_IssueForProduction(_currentDocEntry, 202);
				frm_IssueForProduction2.MdiParent = MdiParent;
				frm_IssueForProduction2.Show();
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

	private void txtProductNo_InvalidValue(object sender, InvalidValueExceptionEventArgs e)
	{
		e.ExceptionMode = ExceptionMode.NoAction;
	}

	private void Control_Leave(object sender, EventArgs e)
	{
		_lastFocusLeave = "";
	}

	private void Control_Enter(object sender, EventArgs e)
	{
		Control control = sender as Control;
		_lastFocusLeave = control.Name;
	}

	private void txtSaleOrder_Validating(object sender, CancelEventArgs e)
	{
		try
		{
			int docNum = Function.ParseInt(txtSaleOrder.EditValue);
			if (docNum > 0)
			{
				Task<DataTable> task = Task.Run(async () => await ExtenseionMethods.GetDocumentHeader(docNum, ApzObjectTypes.oOrders));
				task.Wait();
				DataTable result = task.Result;
				if (result == null || result.Rows.Count == 0 || Function.ToString(result.Rows[0]["DocStatus"]).ToLower() != "o")
				{
					e.Cancel = !ChooseFromListSales();
					txtCustomer.ReadOnly = !e.Cancel;
					txtCustomer.BackColor = ((!e.Cancel) ? Color.LightGray : Color.White);
					EditorButton editorButton = txtCustomer.Properties.Buttons.FirstOrDefault((EditorButton b) => b.Kind == ButtonPredefines.Search);
					if (editorButton != null)
					{
						editorButton.Visible = e.Cancel;
					}
					return;
				}
				txtCustomer.EditValue = Function.ToString(result.Rows[0]["CardCode"]);
				SetDataSourceControl("OriginAbs", Function.ParseInt(result.Rows[0]["DocEntry"]));
				txtCustomer.BackColor = Color.LightGray;
				txtCustomer.ReadOnly = true;
				EditorButton editorButton2 = txtCustomer.Properties.Buttons.FirstOrDefault((EditorButton b) => b.Kind == ButtonPredefines.Search);
				if (editorButton2 != null)
				{
					editorButton2.Visible = false;
				}
			}
			else if (txtCustomer.ReadOnly)
			{
				txtCustomer.EditValue = "";
				SetDataSourceControl("OriginAbs", 0);
				txtCustomer.BackColor = Color.White;
				txtCustomer.ReadOnly = false;
				EditorButton editorButton3 = txtCustomer.Properties.Buttons.FirstOrDefault((EditorButton b) => b.Kind == ButtonPredefines.Search);
				if (editorButton3 != null)
				{
					editorButton3.Visible = true;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void txtProductNo_Validating(object sender, CancelEventArgs e)
	{
		_ = 1;
		try
		{
			if (!_isMapping && FormMode != Enums.FormViewModeType.Find)
			{
				txtProductNo.Text = txtProductNo.Text.Trim();
				if (!string.IsNullOrEmpty(txtProductNo.Text) && !_httpClient.BillOfMasterials.IsABOM(txtProductNo.Text))
				{
					CancelEventArgs e2 = e;
					e2.Cancel = await ChooseFormList_Item();
				}
				else if (!string.IsNullOrEmpty(txtProductNo.Text.Trim()))
				{
					CancelEventArgs e2 = e;
					e2.Cancel = await ChooseFormList_Item(isLoad: true);
				}
				else
				{
					_oldTxtCodeValue = "";
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void cbDistrRule_Validating(object sender, CancelEventArgs e)
	{
		try
		{
			ButtonEditEx buttonEditEx = cbDistrRule;
			if (!buttonEditEx.IsModified)
			{
				return;
			}
			string text = Function.ToString(buttonEditEx.EditValue);
			if (text == "")
			{
				return;
			}
			string text2 = (string)(buttonEditEx.EditValue = WinFuntions.ValidateDistrRule(text));
			if (text2 != text)
			{
				BeginInvoke((Action)(() =>
				{
					WinFuntions.ShowCFL(cbDistrRule);
				}));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void txtWarehouse_Validating(object sender, CancelEventArgs e)
	{
		try
		{
			string whsCode = Function.ToString(txtWarehouse.EditValue);
			if (!string.IsNullOrEmpty(whsCode))
			{
				DataTable result = Task.Run(async () => await ExtenseionMethods.GetWareHouseByCode(whsCode)).Result;
				if (!result.IsNotNull())
				{
					e.Cancel = !ChooseFromListWarehouse();
				}
				else
				{
					txt_Branch.EditValue = result.Rows[0]["BPLid"];
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvDoc_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
	{
		try
		{
			if (gvDoc.FocusedColumn == gvDoc.Columns["PlannedQty"] || gvDoc.FocusedColumn == gvDoc.Columns["BaseQty"])
			{
				double num = ((gvDoc.FocusedColumn != gvDoc.Columns["PlannedQty"]) ? (Function.ParseDouble(calcPlannedQuantity.EditValue) * Function.ParseDouble(e.Value)) : Function.ParseDouble(e.Value));
				double num2 = Function.ParseDouble(gvDoc.GetFocusedRowCellValue("IssuedQty"));
				if (num2 > 0.0 && num < num2)
				{
					e.ErrorText = "Planned quantity is lower than issued QTY";
					e.Valid = false;
					WinCommons.Error(Enums.MessageCode.PlannedQtyLesserIssueQty, "");
					return;
				}
				e.ErrorText = "";
				e.Valid = true;
			}
			else if (gvDoc.FocusedColumn == gvDoc.Columns["ItemCode"])
			{
				string itemCode = Function.ToString(e.Value);
				if (!string.IsNullOrEmpty(itemCode))
				{
					Task<bool> task = Task.Run(async () => await ExtenseionMethods.CheckExistItem(itemCode));
					task.Wait();
					if (!task.Result)
					{
						e.Valid = ChooseListColumItem();
						return;
					}
				}
			}
			else if (gvDoc.FocusedColumn == gvDoc.Columns["wareHouse"])
			{
				string whsCode = Function.ToString(e.Value);
				if (!string.IsNullOrEmpty(whsCode) && !Task.Run(async () => await ExtenseionMethods.GetWareHouseByCode(whsCode)).Result.IsNotNull())
				{
					e.Valid = ChooseFromListWareH();
					return;
				}
			}
			else if (gvDoc.FocusedColumn == gvDoc.Columns["Project"])
			{
				string code = Function.ToString(e.Value);
				if (!string.IsNullOrEmpty(code))
				{
					Task<bool> task2 = Task.Run(async () => await ExtenseionMethods.CheckExistProject(code));
					task2.Wait();
					if (!task2.Result)
					{
						e.Valid = ChooseFromListProject(isGrid: true);
						return;
					}
				}
			}
			e.ErrorText = "";
			e.Valid = true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void txtProject_Validating(object sender, CancelEventArgs e)
	{
		try
		{
			string code = Function.ToString(txtProject.EditValue);
			if (!string.IsNullOrEmpty(code))
			{
				Task<bool> task = Task.Run(async () => await ExtenseionMethods.CheckExistProject(code));
				task.Wait();
				if (!task.Result)
				{
					e.Cancel = !ChooseFromListProject();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txtProject_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			string value = Function.ToString(txtProject.EditValue);
			if (WinFuntions.CheckExistValidValue("CheckExistValidProject", value) && grdDoc.DataSource is DataTable dataTable && dataTable.Rows.Count > 0)
			{
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					dataTable.Rows[i]["Project"] = value;
				}
				dataTable.AcceptChanges();
				grdDoc.DataSource = dataTable;
				grdDoc.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void transferRequestToolStripMenuItem_Click(object sender, EventArgs e)
	{
		TransferComponent(ApzObjectTypes.oInventoryTransferRequest);
	}

	private void componentTransferToolStripMenuItem_Click(object sender, EventArgs e)
	{
		TransferComponent(ApzObjectTypes.oStockTransfer);
	}

	private async Task LoadSeriesByPostingDate()
	{
		string postingDate = Function.ParseDateTimes(dateOrder.EditValue).ToString("yyyy-MM-dd");
		DataTable dataTable = await _httpClient.Commons.LoadSeriesByPostingDate(GlobalData.UserName, ApzObjectTypes.oProductionOrders, "--", postingDate);
		look_Series.Properties.DataSource = dataTable;
		if (!dataTable.AsEnumerable().Any((DataRow r) => Function.ParseInt(r["Series"]) == Function.ParseInt(look_Series.EditValue)))
		{
			DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow r) => Function.ToString(r["IsDefault"]) == "Y");
			if (dataRow != null)
			{
				look_Series.EditValue = dataRow["Series"];
			}
			else if (dataTable.Rows.Count > 0)
			{
				look_Series.EditValue = dataTable.Rows[0][0];
			}
		}
	}

	private void CheckChangedData()
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View && Function.ToString(luk_Doc_Status.EditValue) != "C" && !_isLoading && !_isChangeQty)
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

	private void SettingLanguage()
	{
		Text = Apzon.Commons.Properties.Resources.Title_frmProductionOder;
		lb_Type.Utext = Apzon.Commons.Properties.Resources.lb_Type;
		lb_Status.Utext = Apzon.Commons.Properties.Resources.lb_Status;
		lb_ProductNo.Utext = Apzon.Commons.Properties.Resources.lb_ProductNo;
		lb_ProductDescription.Utext = Apzon.Commons.Properties.Resources.lb_ProductDescription;
		lb_PlannedQuantity.Utext = Apzon.Commons.Properties.Resources.lb_PlannedQuantity;
		lb_UoMName.Utext = Apzon.Commons.Properties.Resources.lb_UoMName;
		lb_Warehouse.Utext = Apzon.Commons.Properties.Resources.lb_Warehouse;
		lb_No.Utext = Apzon.Commons.Properties.Resources.lb_No;
		lb_OrderDate.Utext = Apzon.Commons.Properties.Resources.lb_OrderDate;
		lb_DueDate.Utext = Apzon.Commons.Properties.Resources.lb_DueDate;
		lb_User.Utext = Apzon.Commons.Properties.Resources.lb_User;
		lb_Origin.Utext = Apzon.Commons.Properties.Resources.lb_Origin;
		lb_SalesOrder.Utext = Apzon.Commons.Properties.Resources.lb_SalesOrder;
		lb_Customer.Utext = Apzon.Commons.Properties.Resources.lb_Customer;
		lb_DistrRule.Utext = Apzon.Commons.Properties.Resources.lb_DistrRule;
		lb_Project.Utext = Apzon.Commons.Properties.Resources.lb_Project;
		lb_Remarks.Utext = Apzon.Commons.Properties.Resources.lb_Remark;
		lb_PickAndPackRemarks.Utext = Apzon.Commons.Properties.Resources.lb_PickAndPackRemarks;
		btn_OK.Text = Apzon.Commons.Properties.Resources.ButtonAdd;
		btn_Cancel.Text = Apzon.Commons.Properties.Resources.ButtonCancel;
		deleteToolStripMenuItem.Text = Apzon.Commons.Properties.Resources.ButtonDelete;
		AddRowToolStripMenuItem.Text = Apzon.Commons.Properties.Resources.ctx_AddRow;
		tab_Components.Text = Apzon.Commons.Properties.Resources.tab_Components;
		gcItemType.Caption = Apzon.Commons.Properties.Resources.lb_ItemType;
		gcItemCode.Caption = Apzon.Commons.Properties.Resources.Line_gvLine_gcItemCode;
		gcItemName.Caption = Apzon.Commons.Properties.Resources.lb_Description;
		gcBaseQty.Caption = Apzon.Commons.Properties.Resources.lb_BaseQty;
		gcPlannedQty.Caption = Apzon.Commons.Properties.Resources.lb_PlannedQty;
		gcIssuedQty.Caption = Apzon.Commons.Properties.Resources.lb_IssuedQty;
		gcwareHouse.Caption = Apzon.Commons.Properties.Resources.lb_Warehouse;
		gcWipActCode.Caption = Apzon.Commons.Properties.Resources.lb_WipActCode;
		gcUomCode.Caption = Apzon.Commons.Properties.Resources.lb_UomCode;
		gcUomName.Caption = Apzon.Commons.Properties.Resources.lb_UoMName;
		gcOnHand.Caption = Apzon.Commons.Properties.Resources.OITW_gcOnHand;
		gcAvailable.Caption = Apzon.Commons.Properties.Resources.OITW_gcAvaiable;
		gcIssueType.Caption = Apzon.Commons.Properties.Resources.lb_IssueType;
		lb_ActualItemComponentCost.Utext = Apzon.Commons.Properties.Resources.lb_ActualItemComponentCost;
		lb_ActualResourceComponentCost.Utext = Apzon.Commons.Properties.Resources.lb_ActualResourceComponentCost;
		lb_AtlAdditionalCost.Utext = Apzon.Commons.Properties.Resources.lb_AtlAdditionalCost;
		lblapc.Utext = Apzon.Commons.Properties.Resources.lb_ActualProductCost;
		lb_ActualByProductCost.Utext = Apzon.Commons.Properties.Resources.lb_ActualByProductCost;
		lb_TotalVariance.Utext = Apzon.Commons.Properties.Resources.lb_TotalVariance;
		lb_PlannedQuantity.Utext = Apzon.Commons.Properties.Resources.lb_PlannedQuantity;
		lb_CompletedQuantity.Utext = Apzon.Commons.Properties.Resources.lb_CompletedQuantity;
		lbRejectedQuantity.Utext = Apzon.Commons.Properties.Resources.lb_RejectedQuantity;
		lb_DueDate1.Utext = Apzon.Commons.Properties.Resources.lb_DueDate;
		lb_ActualClosingDate.Utext = Apzon.Commons.Properties.Resources.lb_ActualClosingDate;
		lb_OverDue.Utext = Apzon.Commons.Properties.Resources.lb_OverDue;
	}

	private async Task LoadCb()
	{
		_ = 1;
		try
		{
			await LoadCbUser();
			LoadCbSeries();
			await SetDataBranch();
			luk_Doc_Type.SetDataSource(new string[3, 2]
			{
				{ "S", "Standard" },
				{ "P", "Special" },
				{ "D", "Disassembly" }
			}, isStringCode: true);
			luk_Doc_Status.SetDataSource(_arrStatus, isStringCode: true);
			rep_Line_Type.SetDataSource(new string[3, 2]
			{
				{ "4", "Item" },
				{ "290", "Resource" },
				{ "-18", "Text" }
			}, isStringCode: false);
			rep_line_IssueMtd.SetDataSource(new string[2, 2]
			{
				{ "M", "Manual" },
				{ "B", "Backflush" }
			}, isStringCode: true);
			look_OriginType.SetDataSource(new string[4, 2]
			{
				{ "M", "Manual" },
				{ "R", "MRP" },
				{ "S", "Sales Order" },
				{ "U", "Upgrade" }
			}, isStringCode: true);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task SetDataBranch()
	{
		try
		{
			if (!GlobalData.IsMultiBranch)
			{
				txt_Branch.Visible = false;
				lbBranch.Visible = false;
				return;
			}
			txt_Branch.Visible = true;
			lbBranch.Visible = true;
			DataTable dataSource = await GetDataBranch();
			txt_Branch.Properties.DataSource = dataSource;
			txt_Branch.EditValue = GlobalData.DefaultBranch;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<DataTable> GetDataBranch()
	{
		try
		{
			string userCodeLogin = GlobalData.UserCodeLogin;
			return await _httpClient.ProductionOrders.Commons_GetDataBrach(userCodeLogin);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	private void LoadCbSeries()
	{
		try
		{
			DataTable dataTable = _httpClient.ProductionOrders.Commons_GetSeries(202);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				look_Series.Properties.DataSource = dataTable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadCbUser()
	{
		try
		{
			DataTable dt = new DataTable();
			dt.Columns.Add("Code", typeof(int));
			dt.Columns.Add("Name", typeof(string));
			dt.AcceptChanges();
			DataTable dataTable = await _httpClient.ProductionOrders.ShowUsers();
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					DataRow dataRow = dt.NewRow();
					dataRow["Code"] = Function.ParseInt(dataTable.Rows[i]["USERID"]);
					dataRow["Name"] = Function.ToString(dataTable.Rows[i]["U_NAME"]);
					dt.Rows.Add(dataRow);
				}
				dt.AcceptChanges();
				luk_Doc_User.Properties.DataSource = dt;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task RefreshData()
	{
		_ = 1;
		try
		{
			ucBatchAndSerial1.ClearDataSource();
			spl_LinesBatchSerial.PanelVisibility = SplitPanelVisibility.Panel1;
			await LoadCb();
			_newDocNum = await _httpClient.ProductionOrders.GetDocNum(Function.ParseInt(look_Series.EditValue));
			_currentDocEntry = 0;
			CurrentDocStatus = "P";
			FormMode = Enums.FormViewModeType.Add;
			txtNo.Properties.ReadOnly = true;
			DateEditEx dateEditEx = dateDue;
			DateEditEx dateEditEx2 = dateOrder;
			DateTime dateTime = (dateStart.DateTime = DateTime.Today);
			DateTime dateTime2 = (dateEditEx2.DateTime = dateTime);
			dateEditEx.DateTime = dateTime2;
			txtProductNo.Text = string.Empty;
			txtProductDesc.Properties.ReadOnly = true;
			txtProductDesc.Text = string.Empty;
			calcPlannedQuantity.EditValue = 1f;
			calcPlannedQuantityInfo.EditValue = 1f;
			txtWarehouse.Text = string.Empty;
			txtSaleOrder.Text = string.Empty;
			txtCustomer.Text = string.Empty;
			DataTable dataSource = _dtSearchLine.Clone();
			grdDoc.DataSource = dataSource;
			_oldTxtCodeValue = txtProductNo.Text;
			grdDoc.RefreshDataSource();
			ucAttachment1.grd_File.DataSource = WinFuntions.GetTableStructure("ATC1");
			ucAttachment1.grd_File.RefreshDataSource();
			gvDoc.HideLoadingPanel();
			SetContextMenu();
			look_OriginType.EditValue = "M";
			ResetUdfGrid();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ResetUdfGrid()
	{
		try
		{
			DataTable dataTable = new DataTable();
			for (int i = 0; i < _dtSearch.Columns.Count; i++)
			{
				if (_dtSearch.Columns[i].ColumnName.StartsWith("U_"))
				{
					dataTable.Columns.Add(new DataColumn(_dtSearch.Columns[i].ColumnName, _dtSearch.Columns[i].DataType));
					if (_dtSearch.Columns[i].DefaultValue != null)
					{
						dataTable.Columns[_dtSearch.Columns[i].ColumnName].DefaultValue = _dtSearch.Columns[i].DefaultValue;
					}
				}
			}
			dataTable.Rows.Add(dataTable.NewRow());
			dataTable.AcceptChanges();
			grd_UDF.DataSource = dataTable;
			grd_UDF.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task AddNewRow()
	{
		try
		{
			if (FormAuth == Enums.FormAuthorities.Read)
			{
				return;
			}
			DataTable dataTable = grdDoc.DataSource as DataTable;
			if (dataTable == null || dataTable.Columns.Count == 0)
			{
				dataTable = _dtSearchLine.Clone();
			}
			if (dataTable.AsEnumerable().FirstOrDefault((DataRow x) => string.IsNullOrWhiteSpace(Function.ToString(x["ItemCode"]))) != null)
			{
				return;
			}
			DataRow dataRow = dataTable.NewRow();
			if (dataTable.IsNotNull())
			{
				_lineNum = dataTable.AsEnumerable().Max((DataRow r) => Function.ParseInt(r["LineNum"])) + 1;
				dataRow["LineNum"] = _lineNum;
			}
			else
			{
				dataRow["LineNum"] = _lineNum + 1;
			}
			dataRow["DocEntry"] = _currentDocEntry;
			dataRow["ItemCode"] = "";
			dataRow["ItemType"] = 4;
			dataRow["BaseQty"] = 1;
			dataRow["PlannedQty"] = 1;
			dataRow["IssueType"] = "M";
			dataRow["UomCode"] = "Manual";
			dataRow["StartDate"] = dateStart.EditValue ?? DBNull.Value;
			dataRow["EndDate"] = dateDue.EditValue ?? DBNull.Value;
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			grdDoc.DataSource = dataTable;
			grdDoc.RefreshDataSource();
			gvDoc.HideLoadingPanel();
			if (gvDoc.Columns.AsEnumerable().FirstOrDefault((GridColumn col) => col.ColumnEditName.Contains("rep_line_OcrCode")) == null)
			{
				await WinFuntions.CreateColumnsDistRule(gvDoc);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<int> GetActiveDimensionCount()
	{
		int ret = 0;
		try
		{
			DataTable dataTable = await _httpClient.ProductionOrders.GetActiveDimensionCount();
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				ret = Function.ParseInt(dataTable.Rows[0][0]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		return ret;
	}

	private async Task<bool> ChooseFormList_Item(bool isLoad = false)
	{
		bool cancelValidating = false;
		try
		{
			ucBatchAndSerial1.ClearDataSource();
			spl_LinesBatchSerial.PanelVisibility = SplitPanelVisibility.Panel1;
			if (!isLoad)
			{
				frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.BillOfMaterForProduction, false, txtProductNo.Text, "", Enums.ItemObjectType.NoItemType, null, "");
				frm_CFL2.ShowDialog(this);
				if (frm_CFL2.ChooseItem != null)
				{
					DataRow chooseItem = frm_CFL2.ChooseItem;
					ItemCode = Function.ToString(chooseItem["Code"]);
					if (AskChangeProductNo(ItemCode))
					{
						txtProductDesc.Text = Function.ToString(chooseItem["ItemName"]);
						txtWarehouse.Text = Function.ToString(chooseItem["ToWH"]);
						calcPlannedQuantity.EditValue = 1f;
						txtProductNo.Text = ItemCode;
						DataTable dataTable = await GetItemNameUoM(ItemCode);
						if (dataTable.IsNotNull())
						{
							txtUoMName.Text = Function.ToString(dataTable.Rows[0]["InvntryUom"]);
						}
						txtJournalRemark.EditValue = "Production Order - " + ItemCode;
						await SearchLine_ITT1();
						_oldTxtCodeValue = txtProductNo.Text.Trim();
						if (string.Compare(btn_OK.Text, "add", ignoreCase: true) == 0)
						{
							_isChangedWhenAdd = true;
						}
						ResetUdfGrid();
					}
					else
					{
						txtProductNo.Text = _oldTxtCodeValue;
						cancelValidating = true;
					}
				}
				else
				{
					cancelValidating = true;
				}
			}
			else
			{
				string itCode = txtProductNo.Text.Trim();
				if (!string.IsNullOrWhiteSpace(itCode) && string.Compare(_oldTxtCodeValue, itCode, ignoreCase: true) != 0 && await ExtenseionMethods.CheckExistsItemCode(itCode))
				{
					if (AskChangeProductNo(itCode))
					{
						DataTable dataTable2 = await GetItemNameUoM(itCode);
						if (dataTable2.IsNotNull())
						{
							txtProductDesc.Text = Function.ToString(dataTable2.Rows[0]["ItemName"]);
							txtUoMName.Text = Function.ToString(dataTable2.Rows[0]["InvntryUom"]);
						}
						DataTable dataTable3 = await frm_BillOfMaterials.GetBoMByItemCode(itCode);
						if (dataTable3.IsNotNull())
						{
							txtWarehouse.Text = Function.ToString(dataTable3.Rows[0]["ToWH"]);
							txtProject.Text = Function.ToString(dataTable3.Rows[0]["Project"]);
							cbDistrRule.Text = GetOcrCodeString(dataTable3.Rows[0]);
						}
						await SearchLine_ITT1();
						_oldTxtCodeValue = itCode.Trim();
						if (string.Compare(btn_OK.Text, "add", ignoreCase: true) == 0)
						{
							_isChangedWhenAdd = true;
						}
						ResetUdfGrid();
					}
					else
					{
						txtProductNo.Text = _oldTxtCodeValue;
						cancelValidating = true;
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		return cancelValidating;
	}

	private async void Item_HandleChooseItem(DataRow row)
	{
		_ = 1;
		try
		{
			if (!(Function.ToString(luk_Doc_Type.EditValue) == "P") || row == null)
			{
				return;
			}
			ItemCode = Function.ToString(row["ItemCode"]);
			if (AskChangeProductNo(ItemCode))
			{
				txtProductDesc.Text = Function.ToString(row["ItemDescription"]);
				txtWarehouse.Text = Function.ToString(row["WarehouseCode"]);
				calcPlannedQuantity.EditValue = 1f;
				txtProductNo.Text = ItemCode;
				DataTable dataTable = await GetItemNameUoM(ItemCode);
				if (dataTable.IsNotNull())
				{
					txtUoMName.Text = Function.ToString(dataTable.Rows[0]["InvntryUom"]);
				}
				txtJournalRemark.EditValue = "Production Order - " + ItemCode;
				grdDoc.DataSourceTable.Rows.Clear();
				await AddNewRow();
				_oldTxtCodeValue = txtProductNo.Text.Trim();
				if (string.Compare(btn_OK.Text, "add", ignoreCase: true) == 0)
				{
					_isChangedWhenAdd = true;
				}
				ResetUdfGrid();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void Bom_HandleChooseItem(DataRow row)
	{
		_ = 1;
		try
		{
			if (!(Function.ToString(luk_Doc_Type.EditValue) != "P") || row == null)
			{
				return;
			}
			ItemCode = Function.ToString(row["Code"]);
			if (AskChangeProductNo(ItemCode))
			{
				txtProductDesc.Text = Function.ToString(row["ItemName"]);
				txtWarehouse.Text = Function.ToString(row["ToWH"]);
				if (FormMode != Enums.FormViewModeType.View && FormMode != Enums.FormViewModeType.Find)
				{
					calcPlannedQuantity.EditValue = 1f;
				}
				txtProductNo.Text = ItemCode;
				cbDistrRule.Text = Function.ToString(row["OcrCode"]);
				DataTable dataTable = await GetItemNameUoM(ItemCode);
				if (dataTable.IsNotNull())
				{
					txtUoMName.Text = Function.ToString(dataTable.Rows[0]["InvntryUom"]);
				}
				txtJournalRemark.EditValue = "Production Order - " + ItemCode;
				await SearchLine_ITT1();
				_oldTxtCodeValue = txtProductNo.Text.Trim();
				if (string.Compare(btn_OK.Text, "add", ignoreCase: true) == 0)
				{
					_isChangedWhenAdd = true;
				}
				ResetUdfGrid();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task SearchLine_ITT1()
	{
		_ = 1;
		try
		{
			grdDoc.DataSourceTable().Rows.Clear();
			BillOfMasterials_LineImpl_ITT1 billOfMasterials_LineImpl_ITT = new BillOfMasterials_LineImpl_ITT1();
			billOfMasterials_LineImpl_ITT.Father = Function.ToString(txtProductNo.Text);
			DataTable dataTable = await _httpClient.ProductionOrders.GetItem_ITT1(billOfMasterials_LineImpl_ITT);
			DataTable dataTable2 = _dtSearchLine.Clone();
			if (dataTable2.Columns.Count > 0 && dataTable != null && dataTable.Rows.Count > 0)
			{
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					DataRow des = dataTable2.NewRow();
					des["StartDate"] = dateStart.EditValue;
					des["EndDate"] = dateDue.EditValue;
					SetDTRValues(dataTable.Rows[i], ref des);
					dataTable2.Rows.Add(des);
				}
				dataTable2.AcceptChanges();
				try
				{
					grdDoc.DataSource = dataTable2;
					grdDoc.RefreshDataSource();
					gvDoc.HideLoadingPanel();
					SetContextMenu();
					await AddNewRow();
					return;
				}
				catch (Exception ex)
				{
					Logging.Write("ERROR", ex);
					WinCommons.ErrorLineNumber(ex);
					return;
				}
			}
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", ex2);
			WinCommons.ErrorLineNumber(ex2);
		}
	}

	private void SetDTRValues(DataRow source, ref DataRow des)
	{
		try
		{
			for (int i = 0; i < des.Table.Columns.Count; i++)
			{
				if (source.Table.Columns.Contains(des.Table.Columns[i].ColumnName))
				{
					des[des.Table.Columns[i].ColumnName] = source[des.Table.Columns[i].ColumnName];
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void SetControlsBackColor()
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Find)
			{
				this.SetFindMode();
			}
			else
			{
				this.ResetData();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private DataTable GetOITWQuantity(string code, string whsCode)
	{
		DataTable result = new DataTable();
		try
		{
			result = _httpClient.ProductionOrders.GetOITWQuantity(code, whsCode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		return result;
	}

	private async Task<DataTable> GetItemNameUoM(string itemCode)
	{
		DataTable dt = new DataTable();
		try
		{
			dt = await _httpClient.ProductionOrders.GetItemNameUoM(itemCode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		return dt;
	}

	private int GetSalesOrderEntry(int docNum)
	{
		int result = 0;
		try
		{
			DataTable salesOrderEntry = _httpClient.ProductionOrders.GetSalesOrderEntry(docNum);
			if (salesOrderEntry != null && salesOrderEntry.Rows.Count > 0)
			{
				result = Function.ParseInt(salesOrderEntry.Rows[0][0]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		return result;
	}

	private void LoadItemDetails(int rowHandle)
	{
		try
		{
			string text = Function.ToString(gvDoc.GetRowCellValue(rowHandle, "ItemCode"));
			if (string.IsNullOrWhiteSpace(text))
			{
				return;
			}
			string text2 = ((_curRowtype == 290) ? "R" : "I");
			if (text2 == "R")
			{
				DataTable itemDetails = _httpClient.ProductionOrders.GetItemDetails(text2, text);
				if (itemDetails != null && itemDetails.Rows.Count > 0 && grdDoc.DataSource is DataTable dataTable)
				{
					DataRow dataRow = dataTable.Rows[gvDoc.GetDataSourceRowIndex(rowHandle)];
					DataRow dataRow2 = itemDetails.Rows[0];
					dataRow["ItemName"] = Function.ToString(dataRow2["ResName"]);
					if (itemDetails.Rows.Count > 0)
					{
						DataRow dataRow3 = itemDetails.Rows[0];
						dataRow["InvntryUom"] = Function.ToString(dataRow3["InvntryUom"]);
						dataRow["OnHand"] = Function.ParseDecimal(dataRow3["OnHand"]);
						dataRow["OnOrder"] = Function.ParseDecimal(dataRow3["OnOrder"]);
						dataRow["wareHouse"] = txtWarehouse.Text;
					}
					dataRow["ItemType"] = _curRowtype;
					dataRow["BaseQty"] = 1;
					dataRow["PlannedQty"] = Function.ParseDouble(calcPlannedQuantity.EditValue);
					dataRow["UomCode"] = "Manual";
					dataRow["StartDate"] = dateStart.EditValue;
					dataRow["EndDate"] = dateDue.EditValue;
					dataRow["IssueType"] = Function.ToString(dataRow2["IssueMthd"]);
				}
				return;
			}
			DataTable itemDetails2 = _httpClient.ProductionOrders.GetItemDetails(text2, text);
			if (itemDetails2 != null && itemDetails2.Rows.Count == 1 && grdDoc.DataSource is DataTable dataTable2)
			{
				DataRow dataRow4 = dataTable2.Rows[gvDoc.GetDataSourceRowIndex(rowHandle)];
				DataRow dataRow5 = itemDetails2.Rows[0];
				dataRow4["ItemName"] = Function.ToString(dataRow5["ItemName"]);
				dataRow4["InvntryUom"] = Function.ToString(dataRow5["InvntryUom"]);
				dataRow4["OnHand"] = Function.ParseDecimal(dataRow5["OnHand"]);
				dataRow4["OnOrder"] = Function.ParseDecimal(dataRow5["OnOrder"]);
				dataRow4["IsCommited"] = Function.ParseDouble(dataRow5["IsCommited"]);
				dataRow4["OnHandInWhse"] = Function.ParseDouble(dataRow5["OnWhs"]);
				dataRow4["IsCommitedInWhse"] = Function.ParseDouble(dataRow5["IsCommitedInWhs"]);
				dataRow4["OnOrderInWhse"] = Function.ParseDouble(dataRow5["OnOrderInWhs"]);
				dataRow4["wareHouse"] = Function.ToString(dataRow5["DfltWH"]);
				dataRow4["IssueType"] = Function.ToString(dataRow5["IssueMthd"]);
				dataRow4["ItemType"] = _curRowtype;
				dataRow4["BaseQty"] = 1;
				dataRow4["PlannedQty"] = Function.ParseDouble(calcPlannedQuantity.EditValue);
				dataRow4["UomCode"] = "Manual";
				dataRow4["ManBtchSerial"] = dataRow5["ManBatchSerial"];
				dataRow4["StartDate"] = dateStart.EditValue;
				dataRow4["EndDate"] = dateDue.EditValue;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void PasteFromExcelToDataTable(GridControl grd, GridView gv, List<string> key, int type)
	{
		_isPasteFromExcel = true;
		try
		{
			if (type != 290 && type != 4)
			{
				return;
			}
			DataObject dataObject = (DataObject)Clipboard.GetDataObject();
			if (dataObject != null)
			{
				string clipboardData = dataObject.GetData(DataFormats.UnicodeText).ToString();
				if (dataObject.GetDataPresent(DataFormats.Text))
				{
					DataTable dataTable = Function.PasteFromExcel(clipboardData);
					if (dataTable != null && (dataTable.Rows.Count > 0 || dataTable.Columns.Count > 0))
					{
						int num = gv.FocusedRowHandle;
						GridColumn focusedColumn = gv.FocusedColumn;
						if (key.Contains(focusedColumn.FieldName))
						{
							for (int i = 0; i < dataTable.Rows.Count; i++)
							{
								if (!frm_BillOfMaterials.IsValidCode(type, Function.ToString(dataTable.Rows[i][0])))
								{
									WinCommons.Warning(Enums.MessageCode.ItemCodeDoesNotExistsWithCode, "", Function.ToString(dataTable.Rows[i][0]));
									break;
								}
								if (num >= gv.RowCount)
								{
									AddNewRow();
								}
								gv.FocusedRowHandle = num;
								int colunmIndex = focusedColumn.VisibleIndex;
								for (int j = 0; j < dataTable.Columns.Count; j++)
								{
									GridColumn gridColumn = gv.Columns.FirstOrDefault((GridColumn t) => t.VisibleIndex == colunmIndex);
									if (gridColumn == null)
									{
										break;
									}
									gv.SetRowCellValue(num, gridColumn, dataTable.Rows[i][j]);
									colunmIndex++;
								}
								gv.SetRowCellValue(num, "ItemType", type);
								gv.FocusedRowHandle = num++;
							}
						}
						else
						{
							int focusedRowHandle = gv.FocusedRowHandle;
							for (int num2 = focusedRowHandle; num2 < gv.RowCount; num2++)
							{
								int colIndex = focusedColumn.VisibleIndex;
								bool flag = true;
								for (int num3 = 0; num3 < key.Count; num3++)
								{
									if (string.IsNullOrEmpty(Function.ToString(gv.GetRowCellValue(num2, key[num3]))))
									{
										flag = false;
										break;
									}
								}
								if (!flag || num2 - focusedRowHandle >= dataTable.Rows.Count)
								{
									break;
								}
								for (int num4 = 0; num4 < dataTable.Columns.Count; num4++)
								{
									GridColumn gridColumn2 = gv.Columns.FirstOrDefault((GridColumn t) => t.VisibleIndex == colIndex);
									if (gridColumn2 == null)
									{
										break;
									}
									gv.SetRowCellValue(num, gridColumn2, dataTable.Rows[num2 - focusedRowHandle][num4]);
									colIndex++;
								}
								num++;
							}
							grd.RefreshDataSource();
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
		_isPasteFromExcel = false;
	}

	private async Task Save()
	{
		_ = 3;
		try
		{
			if (FormAuth != Enums.FormAuthorities.Full)
			{
				WinCommons.Error(Enums.MessageCode.YouDoNotPermisionModifiedForm, "");
				return;
			}
			btn_OK.Enabled = false;
			if (dateOrder.EditValue == null || string.IsNullOrWhiteSpace(dateOrder.EditValue.ToString()))
			{
				WinCommons.Error(Enums.MessageCode.OrderDateIsMissingProductionOrder, "");
				btn_OK.Enabled = true;
				dateOrder.Focus();
				return;
			}
			if (dateStart.EditValue == null || string.IsNullOrWhiteSpace(dateStart.EditValue.ToString()))
			{
				WinCommons.Error(Enums.MessageCode.StartDateIsMissingProductionOrder, "");
				btn_OK.Enabled = true;
				dateStart.Focus();
				return;
			}
			if (dateDue.EditValue == null || string.IsNullOrWhiteSpace(dateDue.EditValue.ToString()))
			{
				WinCommons.Error(Enums.MessageCode.DueDateIsMissingProductionOrder, "");
				btn_OK.Enabled = true;
				dateDue.Focus();
				return;
			}
			ProductionOder_Impl_OWOR doc = GetValueFromDatasource();
			doc.DocEntry = _currentDocEntry;
			if (FormMode == Enums.FormViewModeType.Add)
			{
				doc.DocEntry = 0;
				doc.DocNum = 0;
			}
			string text = ((cbDistrRule.EditValue == null) ? "" : cbDistrRule.EditValue.ToString());
			if (!string.IsNullOrWhiteSpace(text))
			{
				string[] array = text.Split(';');
				for (int i = 0; i < array.Length; i++)
				{
					if (!string.IsNullOrWhiteSpace(array[i]))
					{
						switch (i)
						{
						case 0:
							doc.OcrCode = array[i];
							break;
						case 1:
							doc.OcrCode2 = array[i];
							break;
						case 2:
							doc.OcrCode3 = array[i];
							break;
						case 3:
							doc.OcrCode4 = array[i];
							break;
						case 4:
							doc.OcrCode5 = array[i];
							break;
						}
					}
				}
			}
			DataTable dataSourceTable = grd_UDF.DataSourceTable;
			if (dataSourceTable != null && dataSourceTable.Rows.Count > 0)
			{
				doc.UserFields = new List<UserFieldsImpl>();
				foreach (DataColumn item2 in from DataColumn t in dataSourceTable.Columns
					where t.ColumnName.StartsWith("U_")
					select t)
				{
					UserFieldsImpl userFieldsImpl = new UserFieldsImpl();
					Type dataType = item2.DataType;
					if (dataType == typeof(DateTime))
					{
						userFieldsImpl.Type = BoFieldTypes.db_Date;
					}
					else if (dataType == typeof(float))
					{
						userFieldsImpl.Type = BoFieldTypes.db_Float;
					}
					else if (dataType == typeof(double) || dataType == typeof(decimal))
					{
						userFieldsImpl.Type = BoFieldTypes.db_Numeric;
					}
					else
					{
						userFieldsImpl.Type = BoFieldTypes.db_Alpha;
					}
					userFieldsImpl.Name = item2.ColumnName;
					userFieldsImpl.Value = dataSourceTable.Rows[0][item2];
					doc.UserFields.Add(userFieldsImpl);
				}
			}
			List<ProductionOder_LineImpl_WOR1> lines = new List<ProductionOder_LineImpl_WOR1>();
			DataTable dtItems = (DataTable)grdDoc.DataSource;
			int i2 = 0;
			foreach (DataRow dr in dtItems.Rows)
			{
				if (string.IsNullOrEmpty(dr["ItemCode"].ToString().Trim()))
				{
					continue;
				}
				ProductionOder_LineImpl_WOR1 item = new ProductionOder_LineImpl_WOR1
				{
					DocEntry = doc.DocEntry,
					LineNum = Function.ParseInt(dr["LineNum"]),
					ItemCode = Function.ToString(dr["ItemCode"]),
					ItemName = Function.ToString(dr["ItemName"]),
					BaseQty = Function.ParseDouble(dr["BaseQty"]),
					PlannedQty = Function.ParseDouble(dr["PlannedQty"]),
					IssuedQty = Function.ParseDouble(dr["IssuedQty"]),
					IssueType = Function.ToString(dr["IssueType"]),
					wareHouse = Function.ToString(dr["Warehouse"]),
					WipActCode = Function.ToString(dr["WipActCode"]),
					UomCode = Function.ToString(dr["UomCode"]),
					ItemType = Function.ToString(dr["ItemType"]),
					Project = Function.ToString(dr["Project"]),
					StartDate = Function.ParseDateTime(dr["StartDate"]),
					EndDate = Function.ParseDateTime(dr["EndDate"]),
					OcrCode = Function.ToString(dr["OcrCode"]),
					OcrCode2 = Function.ToString(dr["OcrCode2"]),
					OcrCode3 = Function.ToString(dr["OcrCode3"]),
					OcrCode4 = Function.ToString(dr["OcrCode4"]),
					OcrCode5 = Function.ToString(dr["OcrCode5"]),
					UserFields = new List<UserFieldsImpl>()
				};
				List<UserFieldsImpl> source = await _httpClient.Udf.GetListField("WOR1");
				foreach (DataColumn col in from DataColumn t in dtItems.Columns
					where t.ColumnName.StartsWith("U_")
					select t)
				{
					UserFieldsImpl userFieldsImpl2 = source.FirstOrDefault((UserFieldsImpl u) => u.Name == col.ColumnName.Remove(0, 2));
					userFieldsImpl2.Name = col.ColumnName;
					userFieldsImpl2.Value = dr[col];
					item.UserFields.Add(userFieldsImpl2);
				}
				lines.Add(item);
				i2++;
			}
			DataTable dataTable = ucBatchAndSerial1.CheckDataBatch(dtItems, isDraft: false);
			if (dataTable == null)
			{
				WinCommons._CloseProcessing();
				btn_OK.Enabled = true;
				dateDue.Focus();
				return;
			}
			lines.LoadDataBatch(dataTable, ApzObjectTypes.oProductionOrders);
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				WinCommons._OpenProcessing();
				lines.RemoveAll((ProductionOder_LineImpl_WOR1 p) => string.IsNullOrEmpty(p.ItemCode));
				doc.Line_WOR1 = lines;
				List<AttachmentFile_Impl> list = ucAttachment1.UploadFile();
				if (ucAttachment1.DataSource.IsNotNull() && list.Count == 0)
				{
					return;
				}
				doc.AttachmentFiles = list;
				HttpResult httpResult = await _httpClient.ProductionOrders.SaveDoc(doc);
				if (httpResult.MessageCode == Enums.MessageCode.Success)
				{
					WinCommons.Success(Enums.MessageCode.OperationCompletedSuccess, "");
					if (string.Compare(btn_OK.Text, "add", ignoreCase: true) == 0)
					{
						btn_OK.Text = Apzon.Commons.Properties.Resources.ButtonOK;
						await REFRESH();
						_first = "firstR";
						_last = "lastR";
						_current = "currentR";
					}
					else
					{
						btn_OK.Text = Apzon.Commons.Properties.Resources.ButtonOK;
						await REFRESH_DATA();
						_first = "firstR";
						_last = "lastR";
						_current = "currentR";
					}
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, Apzon.Commons.Properties.Resources.Msg_SaveError + ": " + httpResult.Message);
				}
				btn_OK.Enabled = true;
			}
			else
			{
				btn_OK.Enabled = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private async Task SaveDataSet()
	{
		Enums.FormViewModeType currentMode = FormMode;
		try
		{
			if (dateOrder.EditValue == null || string.IsNullOrWhiteSpace(dateOrder.EditValue.ToString()))
			{
				WinCommons.Error(Enums.MessageCode.OrderDateIsMissingProductionOrder, "");
				btn_OK.Enabled = true;
				dateOrder.Focus();
				return;
			}
			if (dateStart.EditValue == null || string.IsNullOrWhiteSpace(dateStart.EditValue.ToString()))
			{
				WinCommons.Error(Enums.MessageCode.StartDateIsMissingProductionOrder, "");
				btn_OK.Enabled = true;
				dateStart.Focus();
				return;
			}
			if (dateDue.EditValue == null || string.IsNullOrWhiteSpace(dateDue.EditValue.ToString()))
			{
				WinCommons.Error(Enums.MessageCode.DueDateIsMissingProductionOrder, "");
				btn_OK.Enabled = true;
				dateDue.Focus();
				return;
			}
			DataTable dataTable = _dtSearch.Clone();
			DataRow dataRow = dataTable.NewRow();
			dataRow["DocEntry"] = _currentDocEntry;
			dataRow["DocNum"] = Function.ParseInt(txtNo.Text);
			dataRow["PIndicator"] = "Default";
			dataRow["PostDate"] = Function.ParseDateTime(dateOrder.EditValue);
			dataRow["StartDate"] = Function.ParseDateTime(dateStart.EditValue);
			dataRow["DueDate"] = Function.ParseDateTime(dateDue.EditValue);
			dataRow["UserSign"] = Function.ParseInt(luk_Doc_User.EditValue);
			dataRow["OriginType"] = Function.ToString(look_OriginType.EditValue);
			dataRow["OriginNum"] = Function.ParseInt(txtSaleOrder.Text);
			if (GlobalData.IsMultiBranch)
			{
				dataRow["BplId"] = Function.ParseInt(txt_Branch.EditValue);
			}
			else
			{
				dataRow["BplId"] = -1;
			}
			dataRow["CardCode"] = txtCustomer.Text;
			dataRow["Project"] = txtProject.Text;
			dataRow["Type"] = Function.ToString(luk_Doc_Type.EditValue);
			dataRow["Status"] = Function.ToString(luk_Doc_Status.EditValue);
			dataRow["ItemCode"] = Function.ToString(txtProductNo.Text);
			dataRow["PlannedQty"] = Function.ParseDouble(calcPlannedQuantity.Text);
			dataRow["Uom"] = txtUoMName.Text;
			dataRow["Warehouse"] = txtWarehouse.Text;
			dataRow["Comments"] = memoRemarks.Text;
			dataRow["PickRmrk"] = memoPickRemarks.Text;
			dataRow["JrnlMemo"] = txtJournalRemark.Text;
			dataRow["CmpltQty"] = Function.ParseDouble(calcCompletedQuantityInfo.Text);
			dataRow["RjctQty"] = Function.ParseDouble(calcRejectedQuantityInfo.Text);
			dataRow["Series"] = Function.ParseInt(look_Series.EditValue);
			string text = ((cbDistrRule.EditValue == null) ? "" : cbDistrRule.EditValue.ToString());
			if (!string.IsNullOrWhiteSpace(text))
			{
				string[] array = text.Split(';');
				for (int i = 0; i < array.Length; i++)
				{
					if (!string.IsNullOrWhiteSpace(array[i]))
					{
						switch (i)
						{
						case 0:
							dataRow["OcrCode"] = array[i];
							break;
						case 1:
							dataRow["OcrCode2"] = array[i];
							break;
						case 2:
							dataRow["OcrCode3"] = array[i];
							break;
						case 3:
							dataRow["OcrCode4"] = array[i];
							break;
						case 4:
							dataRow["OcrCode5"] = array[i];
							break;
						}
					}
				}
			}
			DataTable dataTable2 = grd_UDF.DataSource as DataTable;
			if (dataTable2.IsNotNull())
			{
				foreach (DataColumn column in dataTable2.Columns)
				{
					dataRow[column.ColumnName] = dataTable2.Rows[0][column];
				}
			}
			DataTable dataTable3 = grdDoc.DataSourceTable.Copy();
			if (!dataTable3.IsNotNull())
			{
				WinCommons.Error(Enums.MessageCode.ItemIsMissing, "");
				FormMode = currentMode;
				return;
			}
			dataTable3.AcceptChanges();
			DataRow[] array2 = (from t in dataTable3.AsEnumerable()
				where !string.IsNullOrEmpty(Function.ToString(t["ItemCode"]))
				select t).ToArray();
			if (array2.Length != 0)
			{
				dataTable3 = array2.CopyToDataTable();
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
				{
					return;
				}
				FormMode = Enums.FormViewModeType.Busy;
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
				DataSet ds = new DataSet();
				ds.Tables.Clear();
				ds.Tables.Add(dataTable.Copy());
				ds.Tables.Add(dataTable3.Copy());
				DataTable dataTable4 = ucBatchAndSerial1.CheckDataBatch(dataTable3, isDraft: false);
				if (dataTable4 == null)
				{
					btn_OK.Enabled = true;
					dateOrder.Focus();
					WinCommons._CloseProcessing();
					return;
				}
				dataTable4.TableName = "BatchAndSerialNumer";
				ds.Tables.Add(dataTable4.Copy());
				WinCommons._OpenProcessing();
				HttpResult httpResult = await _httpClient.ProductionOrders.SaveDataSet(ds);
				ds.Dispose();
				if (string.IsNullOrWhiteSpace(httpResult.Message))
				{
					WinCommons.Success(Enums.MessageCode.OperationCompletedSuccess, "");
					if (string.Compare(btn_OK.Text, "add", ignoreCase: true) == 0)
					{
						_first = "firstR";
						_last = "lastR";
						_current = "currentR";
						btn_OK.Text = "OK";
						await REFRESH();
					}
					else
					{
						FormMode = Enums.FormViewModeType.View;
						await ShowData(Function.ParseInt(httpResult.Content));
						_first = "firstR";
						_last = "lastR";
						_current = "currentR";
						FormMode = Enums.FormViewModeType.View;
					}
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
					FormMode = currentMode;
				}
				WinCommons._CloseProcessing();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.ItemIsMissing, "");
				FormMode = currentMode;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void issueComponentsToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (_currentDocEntry <= 0)
			{
				return;
			}
			if (CurrentDocStatus != "R")
			{
				WinCommons.Error(Enums.MessageCode.CouldNotIssueComponentChangeProductionToRelease, "");
				return;
			}
			switch (Function.ToString(luk_Doc_Type.EditValue))
			{
			case "D":
			{
				frm_ReceiptFromProduction frm_ReceiptFromProduction2 = new frm_ReceiptFromProduction(0, 202);
				frm_ReceiptFromProduction2.MdiParent = MdiParent;
				frm_ReceiptFromProduction2.DocEntryIssueComponents = _currentDocEntry;
				frm_ReceiptFromProduction2.Show();
				break;
			}
			case "S":
			case "P":
			{
				frm_IssueForProduction frm_IssueForProduction2 = new frm_IssueForProduction(0, 202);
				frm_IssueForProduction2.MdiParent = MdiParent;
				frm_IssueForProduction2.DocEntryIssueComponents = _currentDocEntry;
				frm_IssueForProduction2.Show();
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

	public async void PRINTLAYOUT()
	{
		try
		{
			string cardCode = txtCustomer.Text;
			int currentDocEntry = _currentDocEntry;
			await WinFuntions.PrintLayout(cardCode, currentDocEntry, FormLayoutCode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async void PREVIEWLAYOUTS()
	{
		try
		{
			string cardCode = txtCustomer.Text;
			int currentDocEntry = _currentDocEntry;
			await WinFuntions.PreviewLayout(cardCode, currentDocEntry, FormLayoutCode, this);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task Search()
	{
		_ = 2;
		try
		{
			DataTable dataTable = _dtSearch.Clone();
			foreach (DataColumn column in dataTable.Columns)
			{
				column.DefaultValue = null;
				if (column.DataType != typeof(string) && column.DataType != typeof(DateTime))
				{
					column.DataType = typeof(string);
				}
			}
			if (dataTable.Rows.Count == 0)
			{
				dataTable.Rows.Add(dataTable.NewRow());
			}
			dataTable.Rows[0]["DocNum"] = txtNo.Text;
			dataTable.Rows[0]["Status"] = Function.ToString(luk_Doc_Status.EditValue);
			if (Function.ParseInt(txtNo.Text) <= 0)
			{
				if (luk_Doc_Type.EditValue != null)
				{
					dataTable.Rows[0]["Type"] = Function.ToString(luk_Doc_Type.EditValue);
				}
				if (luk_Doc_Status.EditValue != null)
				{
					dataTable.Rows[0]["Status"] = Function.ToString(luk_Doc_Status.EditValue);
				}
				if (Function.ParseInt(luk_Doc_User.EditValue) > 0)
				{
					dataTable.Rows[0]["UserSign"] = Function.ToString(luk_Doc_User.EditValue);
				}
				dataTable.Rows[0]["PostDate"] = ((dateOrder.EditValue == null || string.IsNullOrWhiteSpace(Function.ToString(dateOrder.EditValue))) ? new DateTime(1900, 1, 1) : Function.ParseDateTimes(dateOrder.EditValue));
				dataTable.Rows[0]["StartDate"] = ((dateStart.EditValue == null || string.IsNullOrWhiteSpace(Function.ToString(dateStart.EditValue))) ? new DateTime(1900, 1, 1) : Function.ParseDateTimes(dateStart.EditValue));
				dataTable.Rows[0]["DueDate"] = ((dateDue.EditValue == null || string.IsNullOrWhiteSpace(Function.ToString(dateDue.EditValue))) ? new DateTime(1900, 1, 1) : Function.ParseDateTimes(dateDue.EditValue));
				dataTable.Rows[0]["ItemCode"] = txtProductNo.Text;
				if (!string.IsNullOrEmpty(Function.ToString(txtProject.EditValue)))
				{
					dataTable.Rows[0]["Project"] = Function.ToString(txtProject.EditValue);
				}
			}
			DataTable dataSourceTable = grd_UDF.DataSourceTable;
			if (dataSourceTable.IsNotNull())
			{
				for (int i = 0; i < dataSourceTable.Columns.Count; i++)
				{
					string columnName = dataSourceTable.Columns[i].ColumnName;
					if (dataTable.Columns.Contains(columnName))
					{
						dataTable.Rows[0][columnName] = dataSourceTable.Rows[0][columnName];
					}
				}
			}
			DataSet dataSet = new DataSet();
			dataSet.Tables.Clear();
			dataSet.Tables.Add(dataTable.Copy());
			DataTable dataTable2 = new DataTable();
			dataSet.Tables.Add(dataTable2.Copy());
			DataTable dataTable3 = await _httpClient.ProductionOrders.Search(dataSet);
			if (dataTable3 != null && dataTable3.Rows.Count > 1)
			{
				frm_SearchDataList frm_SearchDataList2 = new frm_SearchDataList
				{
					SourceDt = dataTable3,
					XFormId = "OWOR_SRL",
					ItemId = "GV_CFL",
					Text = "Production Order List",
					ObjType = ApzObjectTypes.oProductionOrders
				};
				frm_SearchDataList2.ShowDialog(this);
				if (frm_SearchDataList2.ChooseItem != null)
				{
					await ShowData(Function.ParseInt(frm_SearchDataList2.ChooseItem["DocEntry"]));
				}
			}
			else if (dataTable3 != null && dataTable3.Rows.Count == 1)
			{
				await ShowData(Function.ParseInt(dataTable3.Rows[0]["DocEntry"]));
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.NoMatchingRecordFound, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task ShowData(int docEntry)
	{
		_isLoading = true;
		try
		{
			ucBatchAndSerial1.ClearDataSource();
			spl_LinesBatchSerial.PanelVisibility = SplitPanelVisibility.Panel1;
			FormMode = Enums.FormViewModeType.View;
			DataTable dataTable = await _httpClient.ProductionOrders.GetADoc(docEntry);
			if (dataTable == null || dataTable.Rows.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.NoMatchingRecordFound, "");
				return;
			}
			DataRow row = dataTable.Rows[0];
			_currentDocEntry = Function.ParseInt(row["DocEntry"]);
			_currentDocNum = Function.ParseInt(row["DocNum"]);
			_current = Function.ToString(row["DocEntry"]);
			DataSource = dataTable;
			txtProductDesc.Text = Function.ToString(row["ProdName"]);
			txtSaleOrder.Text = ((Function.ToString(row["OriginNum"]) == "0") ? "" : Function.ToString(row["OriginNum"]));
			txtDueDate.EditValue = Function.ParseDateTime(row["DueDate"]);
			if (Function.ParseDateTime(row["CloseDate"]).HasValue)
			{
				txtAtlClosingDate.EditValue = Function.ParseDateTime(row["CloseDate"]);
				DateTime dateTime = Function.ParseDateTimes(row["CloseDate"]);
				DateTime dateTime2 = Function.ParseDateTimes(row["DueDate"]);
				if (dateTime > dateTime2)
				{
					txtOverDue.Text = (Function.ParseDateTimes(row["CloseDate"]) - Function.ParseDateTimes(row["DueDate"])).TotalDays.ToString();
				}
			}
			else
			{
				txtAtlClosingDate.Text = "";
				txtOverDue.Text = "";
			}
			LoadDataUdf(row);
			string text = "";
			if (_activeDimCount > 0)
			{
				text = (string.IsNullOrWhiteSpace(Function.ToString(row["OcrCode"])) ? (text + ";") : (text + Function.ToString(row["OcrCode"]) + ";"));
			}
			for (int i = 2; i < _activeDimCount + 1; i++)
			{
				text = (string.IsNullOrWhiteSpace(Function.ToString(row["OcrCode" + i])) ? (text + ";") : (text + Function.ToString(row["OcrCode" + i]) + ";"));
			}
			if (text[text.Length - 1] == ';')
			{
				text = text.Remove(text.Length - 1);
			}
			while (text.Length > 0 && text[text.Length - 1].Equals(';'))
			{
				text = text.Remove(text.Length - 1);
			}
			cbDistrRule.EditValue = text;
			await SearchLine();
			if (tab_Attachments.PageVisible)
			{
				await ucAttachment1.LoadAttachments(Function.ParseInt(row["AtcEntry"]));
			}
			txtNo.Properties.ReadOnly = true;
			string stat = (CurrentDocStatus = Function.ToString(row["Status"]));
			_dtIsIssued = await IsIssued(docEntry);
			DisableControls(stat.ToUpper(), _dtIsIssued);
			btn_OK.Text = Apzon.Commons.Properties.Resources.ButtonOK;
			if (Function.ToString(row["Status"]).ToLower() == "p")
			{
				txtProductNo.ReadOnly = false;
				txtProductNo.BackColor = StyleColor.Editable;
				txtProductNo.Properties.Buttons[0].Visible = true;
				luk_Doc_Type.ReadOnly = false;
				luk_Doc_Type.BackColor = StyleColor.Editable;
			}
			else
			{
				txtProductNo.ReadOnly = true;
				txtProductNo.BackColor = StyleColor.GridDisable;
				txtProductNo.Properties.Buttons[0].Visible = false;
				luk_Doc_Type.ReadOnly = true;
				luk_Doc_Type.BackColor = StyleColor.GridDisable;
			}
			_oldTxtCodeValue = txtProductNo.Text;
			txtProductDesc.BackColor = StyleColor.GridDisable;
			txtNo.BackColor = StyleColor.GridDisable;
			txtUoMName.BackColor = StyleColor.GridDisable;
			dateOrder.BackColor = StyleColor.GridDisable;
			look_Series.BackColor = StyleColor.GridDisable;
			SetDateControlBackColor();
			dateOrder.ReadOnly = true;
			look_Series.ReadOnly = true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		_isLoading = false;
	}

	private async Task<DataTable> IsIssued(int docEntry)
	{
		DataTable dt = new DataTable();
		try
		{
			dt = await _httpClient.ProductionOrders.GetIsIssued(docEntry);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		return dt;
	}

	private void DisableControls(string p, DataTable isIssued = null)
	{
		try
		{
			if (p == "P" || p == "R")
			{
				if (p == "P" || FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Find)
				{
					luk_Doc_Type.Properties.ReadOnly = false;
				}
				else
				{
					luk_Doc_Type.Properties.ReadOnly = true;
				}
				luk_Doc_Status.Properties.ReadOnly = false;
				txtProductNo.Properties.ReadOnly = false;
				calcPlannedQuantity.Properties.ReadOnly = false;
				txtWarehouse.Properties.ReadOnly = false;
				look_Series.Properties.ReadOnly = false;
				txtNo.Properties.ReadOnly = false;
				if (FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Find)
				{
					dateOrder.Properties.ReadOnly = false;
				}
				else
				{
					dateOrder.Properties.ReadOnly = false;
				}
				dateStart.Properties.ReadOnly = false;
				dateDue.Properties.ReadOnly = false;
				luk_Doc_User.Properties.ReadOnly = false;
				txtSaleOrder.Properties.ReadOnly = false;
				txtCustomer.Properties.ReadOnly = false;
				cbDistrRule.Properties.ReadOnly = false;
				txtProject.Properties.ReadOnly = false;
				memoRemarks.Properties.ReadOnly = false;
				memoPickRemarks.Properties.ReadOnly = false;
				gvDoc.OptionsBehavior.Editable = true;
				gv_UDF.OptionsBehavior.Editable = true;
				string[] array = new string[10] { "IssuedQty", "UomCode", "ItemName", "InvntryUom", "OnHand", "OnOrder", "IsCommited", "OnHandInWhse", "IsCommitedInWhse", "OnOrderInWhse" };
				for (int i = 0; i < array.Length; i++)
				{
					if (gvDoc.Columns[array[i]] != null)
					{
						gvDoc.Columns[array[i]].OptionsColumn.ReadOnly = true;
						gvDoc.Columns[array[i]].OptionsColumn.AllowEdit = false;
					}
				}
			}
			else
			{
				luk_Doc_Type.Properties.ReadOnly = true;
				luk_Doc_Status.Properties.ReadOnly = true;
				txtProductNo.Properties.ReadOnly = true;
				calcPlannedQuantity.Properties.ReadOnly = true;
				txtWarehouse.Properties.ReadOnly = true;
				look_Series.Properties.ReadOnly = true;
				txtNo.Properties.ReadOnly = true;
				dateOrder.Properties.ReadOnly = true;
				dateStart.Properties.ReadOnly = true;
				dateDue.Properties.ReadOnly = true;
				luk_Doc_User.Properties.ReadOnly = true;
				txtSaleOrder.Properties.ReadOnly = true;
				txtCustomer.Properties.ReadOnly = true;
				cbDistrRule.Properties.ReadOnly = true;
				txtProject.Properties.ReadOnly = true;
				memoRemarks.Properties.ReadOnly = true;
				memoPickRemarks.Properties.ReadOnly = true;
				gvDoc.OptionsBehavior.Editable = false;
				gv_UDF.OptionsBehavior.Editable = false;
			}
			txtProductDesc.Properties.ReadOnly = true;
			txtUoMName.Properties.ReadOnly = true;
			look_OriginType.Properties.ReadOnly = true;
			string[,] array2 = new string[_arrStatus.GetLength(0), _arrStatus.GetLength(1)];
			Array.Copy(_arrStatus, array2, _arrStatus.Length);
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			if (FormMode == Enums.FormViewModeType.Add)
			{
				new List<DataRow>();
				for (int j = 0; j < array2.GetLength(0); j++)
				{
					if (array2[j, 0].ToString().ToUpper() == "P")
					{
						dictionary.Add(array2[j, 0], array2[j, 1]);
					}
				}
			}
			else if (FormMode == Enums.FormViewModeType.Update || FormMode == Enums.FormViewModeType.View)
			{
				switch (p)
				{
				case "P":
				{
					for (int m = 0; m < array2.GetLength(0); m++)
					{
						if (array2[m, 0].ToString().ToUpper() == "P" || array2[m, 0].ToString().ToUpper() == "R")
						{
							dictionary.Add(array2[m, 0], array2[m, 1]);
						}
					}
					break;
				}
				case "R":
				{
					if (isIssued.IsNotNull())
					{
						for (int k = 0; k < array2.GetLength(0); k++)
						{
							if (array2[k, 0].ToString().ToUpper() != "P" && array2[k, 0].ToString().ToUpper() != "C")
							{
								dictionary.Add(array2[k, 0], array2[k, 1]);
							}
						}
						break;
					}
					for (int l = 0; l < array2.GetLength(0); l++)
					{
						if (array2[l, 0].ToString().ToUpper() != "C")
						{
							dictionary.Add(array2[l, 0], array2[l, 1]);
						}
					}
					break;
				}
				case "L":
					dictionary.Add("L", "Closed");
					break;
				case "C":
					dictionary.Add("C", "Canceled");
					break;
				}
			}
			string[,] array3 = new string[dictionary.Count, 2];
			for (int n = 0; n < dictionary.Count; n++)
			{
				array3[n, 0] = dictionary.Keys.ElementAt(n);
				array3[n, 1] = dictionary.Values.ElementAt(n);
			}
			luk_Doc_Status.SetDataSource(array3, isStringCode: true);
			if (FormMode == Enums.FormViewModeType.Add)
			{
				luk_Doc_Status.EditValue = "P";
				luk_Doc_Type.EditValue = "S";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void LoadDataUdf(DataRow dr)
	{
		try
		{
			DataTable dataTable = new DataTable();
			for (int i = 0; i < dr.Table.Columns.Count; i++)
			{
				if (dr.Table.Columns[i].ColumnName.StartsWith("U_"))
				{
					dataTable.Columns.Add(new DataColumn(dr.Table.Columns[i].ColumnName, dr.Table.Columns[i].DataType));
				}
			}
			dataTable.AcceptChanges();
			DataRow dataRow = dataTable.NewRow();
			for (int j = 0; j < dr.Table.Columns.Count; j++)
			{
				if (dr.Table.Columns[j].ColumnName.StartsWith("U_"))
				{
					dataRow[dr.Table.Columns[j].ColumnName] = dr[j];
				}
			}
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			grd_UDF.DataSource = dataTable;
			grd_UDF.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task SearchLine()
	{
		_ = 2;
		try
		{
			ProductionOder_LineImpl_WOR1 productionOder_LineImpl_WOR = new ProductionOder_LineImpl_WOR1
			{
				DocEntry = _currentDocEntry
			};
			Task<DataSet> task = _httpClient.ProductionOrders.SearchLine(productionOder_LineImpl_WOR);
			Task<DataTable> t2 = _httpClient.OutsourcingClient.GetBatchSerials(Function.ToString(productionOder_LineImpl_WOR.DocEntry), "202");
			DataSet dt = await task;
			DataTable dataTable = await t2;
			if (dataTable != null)
			{
				ucBatchAndSerial1.DataSourceBatchAndSerial = dataTable;
			}
			if (dt != null && dt.Tables.Count > 0)
			{
				foreach (DataRow row in dt.Tables[0].Rows)
				{
					if (Function.ParseInt(row["ItemType"]) == -18)
					{
						row["ItemCode"] = row["LineText"];
					}
				}
				dt.Tables[0].AcceptChanges();
				grdDoc.DataSource = dt.Tables[0];
				grdDoc.RefreshDataSource();
				gvDoc.HideLoadingPanel();
				await AddNewRow();
				if (LineNumFocus > 0)
				{
					DataRow dataRow2 = grdDoc.DataSourceTable.AsEnumerable().FirstOrDefault((DataRow dataRow3) => Function.ParseInt(dataRow3["LineNum"]) == LineNumFocus);
					if (dataRow2 != null)
					{
						gvDoc.FocusedRowHandle = gvDoc.FindRowHandleByDataRow(dataRow2);
					}
					LineNumFocus = -1;
				}
				SetContextMenu();
			}
			else
			{
				DataTable dataSourceTable = grdDoc.DataSourceTable;
				if (dataSourceTable.IsNotNull())
				{
					dataSourceTable.Rows.Clear();
					dataSourceTable.AcceptChanges();
					grdDoc.DataSource = dataSourceTable;
					grdDoc.RefreshDataSource();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void Attachment_BrowseFile(List<DataRow> lst)
	{
		if (lst.Any() && FormMode == Enums.FormViewModeType.View)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	private void viewDiagramToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (_currentDocEntry > 0)
			{
				frm_Diagram frm_Diagram2 = new frm_Diagram();
				frm_Diagram2.ObjectType = ApzObjectTypes.oProductionOrders;
				frm_Diagram2.ObjectId = _currentDocEntry.ToString();
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

	private bool AskChangeProductNo(string productNo)
	{
		try
		{
			if (!string.IsNullOrEmpty(_oldTxtCodeValue.Trim()) && string.Compare(_oldTxtCodeValue, productNo, ignoreCase: true) != 0)
			{
				return XtraMessageBoxEx.Show(Enums.MessageCode.ProductionOrderConfirmUpdateComponent, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return true;
	}

	private void frm_ProductionOrder_OnControlValueChanged(object sender, ControlChangedEventArgs e)
	{
		try
		{
			if (_isLoading || FormMode == Enums.FormViewModeType.Find)
			{
				return;
			}
			if (e.FormControl.Name.Equals(dateStart.Name))
			{
				if (_isMapping || XtraMessageBoxEx.Show(this, Enums.MessageCode.ProductionOrderConfirmUpdateStartAndEndDate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
				{
					for (int i = 0; i < gvDoc.RowCount; i++)
					{
						gvDoc.SetRowCellValue(i, "StartDate", e.Value);
						gvDoc.SetRowCellValue(i, "EndDate", e.Value);
					}
					_showMessageEndate = false;
					if (!_isMapping && (MappingDocument == null || !MappingDocument.Any((ContextActionMappingImpl t) => t.TargetField == "DueDate")))
					{
						dateDue.EditValue = e.Value;
					}
					_showMessageEndate = true;
				}
			}
			else if (e.FormControl.Name.Equals(dateDue.Name) && (_isMapping || !_showMessageEndate || XtraMessageBoxEx.Show(this, Enums.MessageCode.ProductionOrderConfirmUpdateEndDate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes))
			{
				for (int num = 0; num < gvDoc.RowCount; num++)
				{
					gvDoc.SetRowCellValue(num, "EndDate", e.Value);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private bool ValidateDataSave()
	{
		try
		{
			if (string.IsNullOrEmpty(txtProductNo.Text.Trim()))
			{
				WinCommons.Error(Enums.MessageCode.ProductNumberIsMissingProductionOrder, "");
				return false;
			}
			for (int i = 0; i < gvDoc.DataRowCount; i++)
			{
				if (string.IsNullOrEmpty(Function.ToString(gvDoc.GetRowCellValue(i, "ItemCode"))) && !string.IsNullOrEmpty(Function.ToString(gvDoc.GetRowCellValue(i, "ItemName"))))
				{
					WinCommons.Error(Enums.MessageCode.InvalidItemNumber, "");
					gvDoc.FocusedRowHandle = i;
					gvDoc.FocusedColumn = gvDoc.Columns["ItemCode"];
					gvDoc.ShowEditor();
					return false;
				}
			}
			if (!(grdDoc.DataSource is DataTable dataTable) || dataTable.Rows.Count == 0 || !dataTable.AsEnumerable().Any((DataRow r) => !string.IsNullOrEmpty(Function.ToString(r["ItemCode"]))))
			{
				WinCommons.Error(Enums.MessageCode.MustHaveComponentInProductionRow, "");
				return false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return true;
	}

	private void luk_Doc_Status_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			string text = Function.ToString(luk_Doc_Status.EditValue);
			if (string.Equals(text, "L") && text != CurrentDocStatus)
			{
				txtAtlClosingDate.Enabled = true;
				txtAtlClosingDate.BackColor = Color.White;
				txtAtlClosingDate.ReadOnly = false;
			}
			else
			{
				txtAtlClosingDate.ReadOnly = true;
				txtAtlClosingDate.BackColor = StyleColor.GridDisable;
			}
			if (string.Equals(text, "L") && btn_OK.Text == Apzon.Commons.Properties.Resources.ButtonOK)
			{
				luk_Doc_Type.ReadOnly = true;
				luk_Doc_Type.BackColor = StyleColor.GridDisable;
				luk_Doc_Status.ReadOnly = true;
				luk_Doc_Status.BackColor = StyleColor.GridDisable;
				txtProductNo.ReadOnly = true;
				txtProductNo.BackColor = StyleColor.GridDisable;
				txtProductDesc.ReadOnly = true;
				txtProductDesc.BackColor = StyleColor.GridDisable;
				calcPlannedQuantity.ReadOnly = true;
				calcPlannedQuantity.BackColor = StyleColor.GridDisable;
				txtUoMName.ReadOnly = true;
				txtUoMName.BackColor = StyleColor.GridDisable;
				txtWarehouse.ReadOnly = true;
				txtWarehouse.BackColor = StyleColor.GridDisable;
				txtWarehouse.Properties.Buttons[0].Visible = false;
				txt_Branch.ReadOnly = true;
				txt_Branch.BackColor = StyleColor.GridDisable;
				look_Series.ReadOnly = true;
				look_Series.BackColor = StyleColor.GridDisable;
				txtNo.ReadOnly = true;
				txtNo.BackColor = StyleColor.GridDisable;
				dateOrder.ReadOnly = true;
				dateOrder.BackColor = StyleColor.GridDisable;
				dateStart.ReadOnly = true;
				dateStart.BackColor = StyleColor.GridDisable;
				dateDue.ReadOnly = true;
				dateDue.BackColor = StyleColor.GridDisable;
				luk_Doc_User.ReadOnly = true;
				luk_Doc_User.BackColor = StyleColor.GridDisable;
				txtSaleOrder.ReadOnly = true;
				txtSaleOrder.BackColor = StyleColor.GridDisable;
				txtCustomer.ReadOnly = true;
				txtCustomer.BackColor = StyleColor.GridDisable;
				cbDistrRule.ReadOnly = true;
				cbDistrRule.BackColor = StyleColor.GridDisable;
				txtProject.ReadOnly = true;
				txtProject.BackColor = StyleColor.GridDisable;
				memoRemarks.ReadOnly = true;
				memoRemarks.BackColor = StyleColor.GridDisable;
				memoPickRemarks.ReadOnly = true;
				memoPickRemarks.BackColor = StyleColor.GridDisable;
				txtJournalRemark.ReadOnly = true;
				txtJournalRemark.BackColor = StyleColor.GridDisable;
				FormMode = Enums.FormViewModeType.View;
			}
			else
			{
				txtAtlClosingDate.ReadOnly = true;
				luk_Doc_Type.ReadOnly = false;
				luk_Doc_Status.ReadOnly = false;
				txtProductNo.ReadOnly = false;
				txtProductDesc.ReadOnly = false;
				calcPlannedQuantity.ReadOnly = false;
				txtUoMName.ReadOnly = false;
				txtWarehouse.ReadOnly = false;
				txt_Branch.ReadOnly = false;
				look_Series.ReadOnly = false;
				txtNo.ReadOnly = false;
				dateOrder.ReadOnly = false;
				dateStart.ReadOnly = false;
				dateDue.ReadOnly = false;
				luk_Doc_User.ReadOnly = false;
				txtSaleOrder.ReadOnly = false;
				txtCustomer.ReadOnly = false;
				cbDistrRule.ReadOnly = false;
				txtProject.ReadOnly = false;
				memoRemarks.ReadOnly = false;
				memoPickRemarks.ReadOnly = false;
				txtJournalRemark.ReadOnly = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void SetReadOnly(bool value)
	{
		luk_Doc_Type.ReadOnly = value;
		luk_Doc_Status.ReadOnly = value;
		txtProductNo.ReadOnly = value;
		txtProductDesc.ReadOnly = value;
		calcPlannedQuantity.ReadOnly = value;
		txtUoMName.ReadOnly = value;
		txtWarehouse.ReadOnly = value;
		txt_Branch.ReadOnly = value;
		look_Series.ReadOnly = value;
		txtNo.ReadOnly = value;
		dateOrder.ReadOnly = value;
		dateStart.ReadOnly = value;
		dateDue.ReadOnly = value;
		luk_Doc_User.ReadOnly = value;
		txtSaleOrder.ReadOnly = value;
		txtCustomer.ReadOnly = value;
		cbDistrRule.ReadOnly = value;
		txtProject.ReadOnly = value;
		memoRemarks.ReadOnly = value;
		memoPickRemarks.ReadOnly = value;
		txtJournalRemark.ReadOnly = value;
		gvDoc.OptionsBehavior.Editable = !value;
		gv_UDF.OptionsBehavior.Editable = !value;
	}

	private void luk_Doc_Type_EditValueChanging(object sender, ChangingEventArgs e)
	{
	}

	private void luk_Doc_Type_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (Function.ToString(luk_Doc_Type.EditValue) == "P")
			{
				txtProductNo.Properties.ValueMember = "ItemCode";
				txtProductNo.Properties.TypeCFL = Enums.TypeCFL.ListItems;
				txtProductNo.Properties.ListMandatoryField = "ItemDescription,WarehouseCode";
				txtProductNo.Properties.TypeDrillDown = ApzObjectTypes.oItems;
				RepositoryItemButtonEditCFL properties = txtProductNo.Properties;
				properties.HandleChooseItem = (Action<DataRow>)Delegate.Combine(properties.HandleChooseItem, new Action<DataRow>(Item_HandleChooseItem));
				txtProductNo.Properties.RefreshDataSource();
			}
			else
			{
				txtProductNo.Properties.ValueMember = "Code";
				txtProductNo.Properties.TypeCFL = Enums.TypeCFL.BillOfMaterForProduction;
				RepositoryItemButtonEditCFL properties2 = txtProductNo.Properties;
				properties2.HandleChooseItem = (Action<DataRow>)Delegate.Combine(properties2.HandleChooseItem, new Action<DataRow>(Bom_HandleChooseItem));
				txtProductNo.Properties.RefreshDataSource();
				txtProductNo.Properties.TypeDrillDown = ApzObjectTypes.oProductTrees;
			}
		}
		catch (Exception)
		{
			throw;
		}
	}

	private void grdDoc_Paint(object sender, PaintEventArgs e)
	{
		try
		{
			DrawMergedCells(e);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void DrawMergedCells(PaintEventArgs e)
	{
		DataTable dataSourceTable = grdDoc.DataSourceTable;
		for (int i = 0; i < dataSourceTable.Rows.Count; i++)
		{
			if (Function.ParseInt(dataSourceTable.Rows[i]["ItemType"]) != -18)
			{
				continue;
			}
			GridColumn col = gvDoc.Columns.FirstOrDefault((GridColumn x) => x.VisibleIndex == gvDoc.Columns.Max((GridColumn t) => t.VisibleIndex));
			GridViewExMergeCell cell = new GridViewExMergeCell(i, gcItemCode, col);
			painter.DrawMergedCell(cell, e);
		}
	}

	private void rep_Line_Type_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			gvDoc.CloseEditor();
			if (Function.ParseInt(gvDoc.GetFocusedRowCellValue("ItemType")) == -18)
			{
				foreach (GridColumn column in gvDoc.Columns)
				{
					if (column != gcItemType)
					{
						gvDoc.SetFocusedRowCellValue(column, null);
					}
				}
				frm_RowTextDetails frm_RowTextDetails2 = new frm_RowTextDetails();
				frm_RowTextDetails2.RowText = Function.ToString(gvDoc.GetFocusedRowCellValue("ItemCode"));
				if (frm_RowTextDetails2.ShowDialog(this) == DialogResult.OK)
				{
					gvDoc.SetRowCellValue(gvDoc.FocusedRowHandle, "ItemCode", frm_RowTextDetails2.RowText);
				}
			}
			else
			{
				gvDoc.SetFocusedRowCellValue(gcItemCode, null);
				gvDoc.SetFocusedRowCellValue(gcIssueType, null);
			}
			grdDoc.RefreshDataSource();
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
	}

	private void ucAttachment1_ButtonDelete_Click(object sender, EventArgs e)
	{
		if (FormMode == Enums.FormViewModeType.View)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	private void relateToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (Function.ParseInt(gvDoc.GetFocusedRowCellValue("DocEntry")) <= 0)
			{
				WinCommons.Warning(Enums.MessageCode.DataIsNotAvailable, "");
				return;
			}
			frm_RelatedActivites frm_RelatedActivites2 = new frm_RelatedActivites(ApzObjectTypes.oProductionOrders, Function.ToString(gvDoc.GetFocusedRowCellValue("DocEntry")));
			frm_RelatedActivites2.DocNum = Function.ToString(txtNo.Text);
			frm_RelatedActivites2.MdiParent = MdiParent;
			frm_RelatedActivites2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void dateOrder_EditValueChanged(object sender, EventArgs e)
	{
		if (!_isLoading && FormMode != Enums.FormViewModeType.Find)
		{
			dateStart.EditValue = dateOrder.EditValue;
		}
		await LoadSeriesByPostingDate();
	}

	private void InitChangeDataEvent()
	{
		try
		{
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
			if (control is GridControl)
			{
				BaseView defaultView = ((GridControl)control).DefaultView;
				if (defaultView is GridView)
				{
					((GridView)defaultView).CellValueChanged += Control_EditValueChanged;
				}
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

	private string GetDataTableSelectString(DataTable dt)
	{
		StringBuilder stringBuilder = new StringBuilder("SELECT ");
		try
		{
			for (int i = 0; i < dt.Columns.Count; i++)
			{
				stringBuilder.Append(dt.Columns[i].ColumnName + ",");
			}
			stringBuilder.Length--;
			stringBuilder.Append(" FROM ");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			stringBuilder = new StringBuilder(ex.Message);
		}
		return stringBuilder.ToString();
	}

	public void SetMapping(DataTable doc, DataTable lines, ContextActionMappingImpl[] mappingDoc, ContextActionMappingImpl[] mappingLines, DataTable batch, ContextActionMappingImpl[] mappingbatch, Enums.FormViewModeType viewmode = Enums.FormViewModeType.Add)
	{
		OpenFormDocument = doc;
		OpenFormLines = lines;
		MappingDocument = mappingDoc;
		MappingLines = mappingLines;
		OpenFormBatchSerial = batch;
		MappingBatchSerial = mappingbatch;
		_openFormViewMode = viewmode;
	}

	private string GetOcrCodeString(DataRow row)
	{
		try
		{
			string text = "";
			if (_activeDimCount > 0)
			{
				text = (string.IsNullOrWhiteSpace(Function.ToString(row["OcrCode"])) ? (text + ";") : (text + Function.ToString(row["OcrCode"]) + ";"));
			}
			for (int i = 2; i < _activeDimCount + 1; i++)
			{
				text = (string.IsNullOrWhiteSpace(Function.ToString(row["OcrCode" + i])) ? (text + ";") : (text + Function.ToString(row["OcrCode" + i]) + ";"));
			}
			if (text[text.Length - 1] == ';')
			{
				text = text.Remove(text.Length - 1);
			}
			while (text.Length > 0 && text[text.Length - 1].Equals(';'))
			{
				text = text.Remove(text.Length - 1);
			}
			return text;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return "";
		}
	}

	private void SetDateControlBackColor()
	{
		try
		{
			DateTime? dateTime = Function.ParseDateTime(dateDue.EditValue);
			if (dateTime.HasValue && dateTime.Value.Date < DateTime.Today)
			{
				dateDue.ForeColor = Color.Red;
			}
			else
			{
				dateDue.ForeColor = Color.Black;
			}
			DateTime? dateTime2 = Function.ParseDateTime(dateStart.EditValue);
			if (dateTime2.HasValue && dateTime2.Value.Date < DateTime.Today)
			{
				dateStart.ForeColor = Color.Red;
			}
			else
			{
				dateStart.ForeColor = Color.Black;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private ProductionOder_Impl_OWOR GetValueFromDatasource()
	{
		ProductionOder_Impl_OWOR result = new ProductionOder_Impl_OWOR();
		try
		{
			if (DataSource is DataTable dataTable && dataTable.Rows.Count > 0)
			{
				dataTable.Rows[0]["OriginAbs"] = GetSalesOrderEntry(Function.ParseInt(txtSaleOrder.Text));
				result = Function.DataRowToEntity<ProductionOder_Impl_OWOR>(dataTable.Rows[0]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return result;
	}

	private void ChooseFromListWibActCode()
	{
		frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.ChartOfAccount, multiSelect: false, string.Empty);
		frm_ChooseFromList2.ShowDialog(this);
		ChartOfAccountsImpl chartOfAccountsImpl = (ChartOfAccountsImpl)frm_ChooseFromList2.ChooseItem;
		ProductionOder_LineImpl_WOR1 productionOder_LineImpl_WOR = new ProductionOder_LineImpl_WOR1();
		if (frm_ChooseFromList2.ChooseItem != null)
		{
			string wibActCode = (productionOder_LineImpl_WOR.WipActCode = chartOfAccountsImpl.AcctCode);
			_wibActCode = wibActCode;
			gvDoc.SetRowCellValue(gvDoc.FocusedRowHandle, gcWipActCode, _wibActCode);
		}
		else
		{
			_wibActCode = Function.ToString(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, gcWipActCode));
			gvDoc.SetRowCellValue(gvDoc.FocusedRowHandle, gcWipActCode, _wibActCode);
		}
		frm_ChooseFromList2.Close();
	}

	private bool ChooseFromListSales()
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.SalesOder, false, string.Empty, "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem != null)
			{
				string text = Function.ToString(frm_CFL2.ChooseItem["DocNum"]);
				txtSaleOrder.EditValue = ((text == "0") ? "" : text);
				SetDataSourceControl("OriginAbs", Function.ParseInt(frm_CFL2.ChooseItem["DocEntry"]));
				txtCustomer.EditValue = Function.ToString(frm_CFL2.ChooseItem["CardCode"]);
				return true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		return false;
	}

	private bool ChooseFromListWareH()
	{
		try
		{
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.Warehouse, multiSelect: false, string.Empty);
			frm_ChooseFromList2.ShowDialog(this);
			if (frm_ChooseFromList2.ChooseItem is DataRow dataRow)
			{
				gvDoc.SetRowCellValue(gvDoc.FocusedRowHandle, gcwareHouse, dataRow["WhsCode"]);
				return true;
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
			return false;
		}
	}

	private bool ChooseFromListWarehouse()
	{
		try
		{
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.Warehouse, multiSelect: false, string.Empty);
			frm_ChooseFromList2.ShowDialog(this);
			if (frm_ChooseFromList2.ChooseItem is DataRow dataRow)
			{
				txtWarehouse.Text = Function.ToString(dataRow["WhsCode"]);
				txt_Branch.EditValue = dataRow["BPLid"];
				return true;
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
			return false;
		}
	}

	private bool ChooseFromListProject(bool isGrid = false)
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.Project, false, txtProject.Text, "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem != null && frm_CFL2.ChooseItem.IsExistsColumn("PrjCode"))
			{
				if (!isGrid)
				{
					txtProject.Text = Function.ToString(frm_CFL2.ChooseItem["PrjCode"]);
				}
				else
				{
					gvDoc.SetFocusedRowCellValue("Project", Function.ToString(frm_CFL2.ChooseItem["PrjCode"]));
				}
				return true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return false;
	}

	private void TransferComponent(ApzObjectTypes objType)
	{
		try
		{
			DataTable dataTable = DataSource as DataTable;
			frm_ComponentTransfer frm_ComponentTransfer2 = new frm_ComponentTransfer(Function.ParseDouble(dataTable.Rows[0]["PlannedQty"]) - Function.ParseDouble(dataTable.Rows[0]["CmpltQty"]));
			frm_ComponentTransfer2.ShowDialog(this);
			if (frm_ComponentTransfer2.DialogResult == DialogResult.OK && dataTable.IsNotNull())
			{
				if (objType == ApzObjectTypes.oStockTransfer)
				{
					frm_InventoryTranfer frm_InventoryTranfer2 = new frm_InventoryTranfer(Function.ToString(_currentDocEntry), ApzObjectTypes.oProductionOrders);
					frm_InventoryTranfer2.ProductionQuantity = frm_ComponentTransfer2.Quantity;
					frm_InventoryTranfer2.MdiParent = MdiParent;
					frm_InventoryTranfer2.Show();
				}
				else
				{
					frm_InventoryTranferRequest frm_InventoryTranferRequest2 = new frm_InventoryTranferRequest(Function.ToString(_currentDocEntry), ApzObjectTypes.oProductionOrders);
					frm_InventoryTranferRequest2.ProductionQuantity = frm_ComponentTransfer2.Quantity;
					frm_InventoryTranferRequest2.MdiParent = MdiParent;
					frm_InventoryTranferRequest2.Show();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void CalculateBaseRatio(decimal lineQty, decimal fatherQty, out long numerator, out long denominator)
	{
		double num = GlobalData.GeneralSetting.QtyDec.Value;
		decimal num2 = (decimal)Math.Pow(10.0, num);
		long num3 = (long)Math.Round(Math.Abs(lineQty) * num2, MidpointRounding.AwayFromZero);
		long num4 = (long)Math.Round(fatherQty * num2, MidpointRounding.AwayFromZero);
		long num5 = num3;
		long num6 = num4;
		while (num4 > 0)
		{
			long num7 = num3 % num4;
			num3 = num4;
			num4 = num7;
		}
		numerator = num5 / num3;
		denominator = num6 / num3;
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions4 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions5 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions6 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance11 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered11 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed11 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled11 = new DevExpress.Utils.SerializableAppearanceObject();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Production.frm_ProductionOrder));
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions7 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance12 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered12 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed12 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled12 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions8 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance13 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered13 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed13 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled13 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions9 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance14 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered14 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed14 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled14 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions6 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance15 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered15 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed15 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled15 = new DevExpress.Utils.SerializableAppearanceObject();
		this.repositoryItemCalcEditEx1 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx2 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx3 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx4 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx5 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx6 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx7 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx8 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.lb_Type = new Apzon.Controls.Controls.LabelLine();
		this.luk_Doc_Type = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.lb_Warehouse = new Apzon.Controls.Controls.LabelLine();
		this.txtWarehouse = new Apzon.Controls.Editor.ButtonEditEx();
		this.lb_Project = new Apzon.Controls.Controls.LabelLine();
		this.txtProject = new Apzon.Controls.Editor.ButtonEditEx();
		this.lb_DistrRule = new Apzon.Controls.Controls.LabelLine();
		this.cbDistrRule = new Apzon.Controls.Editor.ButtonEditEx();
		this.lb_Customer = new Apzon.Controls.Controls.LabelLine();
		this.txtCustomer = new Apzon.Controls.Editor.ButtonEditEx();
		this.lb_SalesOrder = new Apzon.Controls.Controls.LabelLine();
		this.txtSaleOrder = new Apzon.Controls.Editor.ButtonEditEx();
		this.lb_Origin = new Apzon.Controls.Controls.LabelLine();
		this.look_OriginType = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.lb_User = new Apzon.Controls.Controls.LabelLine();
		this.luk_Doc_User = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.lb_DueDate = new Apzon.Controls.Controls.LabelLine();
		this.dateDue = new Apzon.Controls.Editor.DateEditEx();
		this.lb_OrderDate = new Apzon.Controls.Controls.LabelLine();
		this.dateOrder = new Apzon.Controls.Editor.DateEditEx();
		this.lb_No = new Apzon.Controls.Controls.LabelLine();
		this.txtNo = new Apzon.Controls.Editor.TextEditEx();
		this.lb_PlannedQuantity = new Apzon.Controls.Controls.LabelLine();
		this.calcPlannedQuantity = new Apzon.Controls.Editor.CalcEditEx();
		this.lb_ProductDescription = new Apzon.Controls.Controls.LabelLine();
		this.txtProductDesc = new Apzon.Controls.Editor.TextEditEx();
		this.lb_ProductNo = new Apzon.Controls.Controls.LabelLine();
		this.txtProductNo = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.lb_Status = new Apzon.Controls.Controls.LabelLine();
		this.luk_Doc_Status = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.txtUoMName = new Apzon.Controls.Editor.TextEditEx();
		this.xtraTabControl1 = new Apzon.Controls.Controls.XtraTabControlEx();
		this.tab_Components = new Apzon.Controls.Controls.XtraTabPageEx();
		this.spl_LinesBatchSerial = new DevExpress.XtraEditors.SplitContainerControl();
		this.grdDoc = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip_Line = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.AddRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.batchSerialToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gvDoc = new Apzon.Controls.Grid.GridViewEx();
		this.gcDocEntry = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineNum = new Apzon.Controls.Grid.GridColumnEx();
		this.gcItemType = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Line_Type = new Apzon.Forms.UserControls.RepositoryItemLookUpEditCombo();
		this.gcItemCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rBT_ItemCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcItemName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcBaseQty = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gcPlannedQty = new Apzon.Controls.Grid.GridColumnEx();
		this.gcIssuedQty = new Apzon.Controls.Grid.GridColumnEx();
		this.gcAvailable = new Apzon.Controls.Grid.GridColumnEx();
		this.gcwareHouse = new Apzon.Controls.Grid.GridColumnEx();
		this.rBT_WareH = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcVisOrder = new Apzon.Controls.Grid.GridColumnEx();
		this.gcWipActCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rBT_WibActCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcCompTotal = new Apzon.Controls.Grid.GridColumnEx();
		this.gcOcrCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gcOcrCode2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gcOcrCode3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gcOcrCode4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gcOcrCode5 = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLocCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLogInstanc = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLoadFrBOM = new Apzon.Controls.Grid.GridColumnEx();
		this.gcProject = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_bte_lineProject = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcUomEntry = new Apzon.Controls.Grid.GridColumnEx();
		this.gcUomCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gcAdditQty = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineText = new Apzon.Controls.Grid.GridColumnEx();
		this.gcPickStatus = new Apzon.Controls.Grid.GridColumnEx();
		this.gcPickQty = new Apzon.Controls.Grid.GridColumnEx();
		this.gcPickIdNo = new Apzon.Controls.Grid.GridColumnEx();
		this.gcReleaseQty = new Apzon.Controls.Grid.GridColumnEx();
		this.gcResAlloc = new Apzon.Controls.Grid.GridColumnEx();
		this.gcUomName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcIssueType = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_line_IssueMtd = new Apzon.Forms.UserControls.RepositoryItemLookUpEditCombo();
		this.gcOnHand = new Apzon.Controls.Grid.GridColumnEx();
		this.gcIsCommited = new Apzon.Controls.Grid.GridColumnEx();
		this.gcOnOrder = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx5 = new Apzon.Controls.Grid.GridColumnEx();
		this.ucBatchAndSerial1 = new Apzon.Forms.UserControls.BatchAndSerial.ucBatchAndSerial();
		this.tab_Summary = new Apzon.Controls.Controls.XtraTabPageEx();
		this.calcTotalVariance = new Apzon.Controls.Editor.TextEditExPrice.TextEditExPrice();
		this.calcAtlProductCost = new Apzon.Controls.Editor.TextEditExPrice.TextEditExPrice();
		this.calcAtlItemCost = new Apzon.Controls.Editor.TextEditExPrice.TextEditExPrice();
		this.txtJournalRemark = new Apzon.Controls.Editor.TextEditEx();
		this.txtOverDue = new Apzon.Controls.Editor.TextEditEx();
		this.txtDueDate = new Apzon.Controls.Editor.DateEditEx();
		this.txtAtlClosingDate = new Apzon.Controls.Editor.DateEditEx();
		this.calcRejectedQuantityInfo = new Apzon.Controls.Editor.CalcEditEx();
		this.calcCompletedQuantityInfo = new Apzon.Controls.Editor.CalcEditEx();
		this.calcPlannedQuantityInfo = new Apzon.Controls.Editor.CalcEditEx();
		this.calcAtlByProductCost = new Apzon.Controls.Editor.CalcEditEx();
		this.calcAtlAdditionalCost = new Apzon.Controls.Editor.CalcEditEx();
		this.calcAtlResourceCost = new Apzon.Controls.Editor.CalcEditEx();
		this.txtTotalTime = new Apzon.Controls.Editor.TextEditEx();
		this.txtTotalAdditionalTime = new Apzon.Controls.Editor.TextEditEx();
		this.txtTotalRunTime = new Apzon.Controls.Editor.TextEditEx();
		this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
		this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
		this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
		this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
		this.labelLine38 = new Apzon.Controls.Controls.LabelLine();
		this.lb_OverDue = new Apzon.Controls.Controls.LabelLine();
		this.lb_ActualClosingDate = new Apzon.Controls.Controls.LabelLine();
		this.lb_DueDate1 = new Apzon.Controls.Controls.LabelLine();
		this.lb_TotalTime = new Apzon.Controls.Controls.LabelLine();
		this.lb_TotalAdditionalTime = new Apzon.Controls.Controls.LabelLine();
		this.lb_TotalRunTime = new Apzon.Controls.Controls.LabelLine();
		this.lbRejectedQuantity = new Apzon.Controls.Controls.LabelLine();
		this.lb_CompletedQuantity = new Apzon.Controls.Controls.LabelLine();
		this.labelLine30 = new Apzon.Controls.Controls.LabelLine();
		this.lb_TotalVariance = new Apzon.Controls.Controls.LabelLine();
		this.lb_ActualByProductCost = new Apzon.Controls.Controls.LabelLine();
		this.lblapc = new Apzon.Controls.Controls.LabelLine();
		this.lb_AtlAdditionalCost = new Apzon.Controls.Controls.LabelLine();
		this.lb_ActualResourceComponentCost = new Apzon.Controls.Controls.LabelLine();
		this.lb_ActualItemComponentCost = new Apzon.Controls.Controls.LabelLine();
		this.tab_Attachments = new Apzon.Controls.Controls.XtraTabPageEx();
		this.ucAttachment1 = new Apzon.Forms.UserControls.ucAttachment();
		this.tab_UDF = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_UDF = new Apzon.Controls.Grid.GridControlEx();
		this.gv_UDF = new Apzon.Controls.Grid.GridViewEx();
		this.repositoryItemComboBox3 = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
		this.repositoryItemButtonEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.repositoryItemCalcEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.repositoryItemButtonEdit3 = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.repositoryItemComboBox2 = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
		this.repositoryItemButtonEdit4 = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.repositoryItemComboBox1 = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
		this.repositoryItemButtonEdit5 = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.memoPickRemarks = new Apzon.Controls.Editor.MemoEditEx();
		this.memoRemarks = new Apzon.Controls.Editor.MemoEditEx();
		this.lb_PickAndPackRemarks = new Apzon.Controls.Controls.LabelLine();
		this.lb_Remarks = new Apzon.Controls.Controls.LabelLine();
		this.lb_UoMName = new Apzon.Controls.Controls.LabelLine();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.contextMenuStrip_Doc = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.cancelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.duplicateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
		this.reportCompletionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.issueComponentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.transferRequestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.componentTransferToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
		this.viewDiagramToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.relateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.look_Series = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.lbBranch = new Apzon.Controls.Controls.LabelLine();
		this.txt_Branch = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.dateStart = new Apzon.Controls.Editor.DateEditEx();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.txtProductNo_Item = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.txtProductNo_Bom = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.gridColumnEx6 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx7 = new Apzon.Controls.Grid.GridColumnEx();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx6).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx7).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx8).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Doc_Type.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtWarehouse.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtProject.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbDistrRule.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtCustomer.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtSaleOrder.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.look_OriginType.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Doc_User.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dateDue.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dateDue.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dateOrder.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dateOrder.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtNo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calcPlannedQuantity.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtProductDesc.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtProductNo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Doc_Status.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtUoMName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).BeginInit();
		this.xtraTabControl1.SuspendLayout();
		this.tab_Components.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.spl_LinesBatchSerial).BeginInit();
		this.spl_LinesBatchSerial.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).BeginInit();
		this.contextMenuStrip_Line.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Line_Type).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rBT_ItemCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rBT_WareH).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rBT_WibActCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_bte_lineProject).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_line_IssueMtd).BeginInit();
		this.tab_Summary.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.calcTotalVariance.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calcAtlProductCost.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calcAtlItemCost.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtJournalRemark.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtOverDue.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtDueDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtDueDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtAtlClosingDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtAtlClosingDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calcRejectedQuantityInfo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calcCompletedQuantityInfo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calcPlannedQuantityInfo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calcAtlByProductCost.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calcAtlAdditionalCost.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calcAtlResourceCost.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtTotalTime.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtTotalAdditionalTime.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtTotalRunTime.Properties).BeginInit();
		this.tab_Attachments.SuspendLayout();
		this.tab_UDF.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_UDF).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_UDF).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemComboBox3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemButtonEdit2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemButtonEdit3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemComboBox2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemButtonEdit4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemComboBox1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemButtonEdit5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.memoPickRemarks.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.memoRemarks.Properties).BeginInit();
		this.contextMenuStrip_Doc.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.look_Series.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Branch.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dateStart.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dateStart.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtProductNo_Item.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtProductNo_Bom.Properties).BeginInit();
		base.SuspendLayout();
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
		this.lb_Type.EditControl = this.luk_Doc_Type;
		this.lb_Type.EditValue = "Type";
		this.lb_Type.FieldName = null;
		this.lb_Type.HideWhenEmpty = false;
		this.lb_Type.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Type.Location = new System.Drawing.Point(8, 5);
		this.lb_Type.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_Type.Name = "lb_Type";
		this.lb_Type.ReadOnly = false;
		this.lb_Type.SettingInVisible = false;
		this.lb_Type.SettingReadonly = false;
		this.lb_Type.ShowUnderLine = true;
		this.lb_Type.Size = new System.Drawing.Size(130, 20);
		this.lb_Type.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Type.TabIndex = 0;
		this.lb_Type.Utext = "Type";
		this.luk_Doc_Type.EditControl = null;
		this.luk_Doc_Type.FieldName = "Type";
		this.luk_Doc_Type.IsSearchControl = false;
		this.luk_Doc_Type.Location = new System.Drawing.Point(137, 2);
		this.luk_Doc_Type.Name = "luk_Doc_Type";
		this.luk_Doc_Type.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Doc_Type.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.luk_Doc_Type.Properties.DisplayMember = "Name";
		this.luk_Doc_Type.Properties.IsSearchControl = false;
		this.luk_Doc_Type.Properties.NullText = "";
		this.luk_Doc_Type.Properties.ValueMember = "Code";
		this.luk_Doc_Type.SettingInVisible = false;
		this.luk_Doc_Type.SettingReadonly = false;
		this.luk_Doc_Type.Size = new System.Drawing.Size(135, 20);
		this.luk_Doc_Type.TabIndex = 0;
		this.luk_Doc_Type.EditValueChanged += new System.EventHandler(this.luk_Doc_Type_EditValueChanged);
		this.luk_Doc_Type.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.luk_Doc_Type_EditValueChanging);
		this.lb_Warehouse.EditControl = this.txtWarehouse;
		this.lb_Warehouse.EditValue = "Warehouse";
		this.lb_Warehouse.FieldName = null;
		this.lb_Warehouse.HideWhenEmpty = false;
		this.lb_Warehouse.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Warehouse.Location = new System.Drawing.Point(8, 109);
		this.lb_Warehouse.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_Warehouse.Name = "lb_Warehouse";
		this.lb_Warehouse.ReadOnly = false;
		this.lb_Warehouse.SettingInVisible = false;
		this.lb_Warehouse.SettingReadonly = false;
		this.lb_Warehouse.ShowUnderLine = true;
		this.lb_Warehouse.Size = new System.Drawing.Size(130, 20);
		this.lb_Warehouse.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Warehouse.TabIndex = 1;
		this.lb_Warehouse.Utext = "Warehouse";
		this.txtWarehouse.EditControl = null;
		this.txtWarehouse.FieldName = "Warehouse";
		this.txtWarehouse.IsSearchControl = false;
		this.txtWarehouse.Location = new System.Drawing.Point(137, 107);
		this.txtWarehouse.Name = "txtWarehouse";
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.txtWarehouse.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.txtWarehouse.Properties.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtWarehouse_Properties_ButtonPressed);
		this.txtWarehouse.Properties.Enter += new System.EventHandler(this.Control_Enter);
		this.txtWarehouse.SettingInVisible = false;
		this.txtWarehouse.SettingReadonly = false;
		this.txtWarehouse.Size = new System.Drawing.Size(135, 20);
		this.txtWarehouse.TabIndex = 7;
		this.txtWarehouse.Leave += new System.EventHandler(this.Control_Leave);
		this.txtWarehouse.Validating += new System.ComponentModel.CancelEventHandler(this.txtWarehouse_Validating);
		this.lb_Project.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lb_Project.EditControl = this.txtProject;
		this.lb_Project.EditValue = "Project";
		this.lb_Project.FieldName = null;
		this.lb_Project.HideWhenEmpty = false;
		this.lb_Project.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Project.Location = new System.Drawing.Point(440, 194);
		this.lb_Project.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_Project.Name = "lb_Project";
		this.lb_Project.ReadOnly = false;
		this.lb_Project.SettingInVisible = false;
		this.lb_Project.SettingReadonly = false;
		this.lb_Project.ShowUnderLine = true;
		this.lb_Project.Size = new System.Drawing.Size(180, 20);
		this.lb_Project.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Project.TabIndex = 2;
		this.lb_Project.Utext = "Project";
		this.txtProject.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txtProject.EditControl = null;
		this.txtProject.FieldName = "Project";
		this.txtProject.IsSearchControl = false;
		this.txtProject.Location = new System.Drawing.Point(619, 191);
		this.txtProject.Name = "txtProject";
		this.txtProject.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.txtProject.Properties.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtProject_Properties_ButtonPressed);
		this.txtProject.SettingInVisible = false;
		this.txtProject.SettingReadonly = false;
		this.txtProject.Size = new System.Drawing.Size(128, 20);
		this.txtProject.TabIndex = 19;
		this.txtProject.EditValueChanged += new System.EventHandler(this.txtProject_EditValueChanged);
		this.txtProject.Enter += new System.EventHandler(this.Control_Enter);
		this.txtProject.Leave += new System.EventHandler(this.Control_Leave);
		this.txtProject.Validating += new System.ComponentModel.CancelEventHandler(this.txtProject_Validating);
		this.lb_DistrRule.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lb_DistrRule.EditControl = this.cbDistrRule;
		this.lb_DistrRule.EditValue = "Distr. Rule";
		this.lb_DistrRule.FieldName = null;
		this.lb_DistrRule.HideWhenEmpty = false;
		this.lb_DistrRule.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_DistrRule.Location = new System.Drawing.Point(440, 173);
		this.lb_DistrRule.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_DistrRule.Name = "lb_DistrRule";
		this.lb_DistrRule.ReadOnly = false;
		this.lb_DistrRule.SettingInVisible = false;
		this.lb_DistrRule.SettingReadonly = false;
		this.lb_DistrRule.ShowUnderLine = true;
		this.lb_DistrRule.Size = new System.Drawing.Size(180, 20);
		this.lb_DistrRule.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_DistrRule.TabIndex = 3;
		this.lb_DistrRule.Utext = "Distr. Rule";
		this.cbDistrRule.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.cbDistrRule.EditControl = null;
		this.cbDistrRule.FieldName = "DistrRule";
		this.cbDistrRule.IsSearchControl = false;
		this.cbDistrRule.Location = new System.Drawing.Point(619, 170);
		this.cbDistrRule.Name = "cbDistrRule";
		this.cbDistrRule.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.cbDistrRule.Properties.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.comboBoxEdit1_Properties_ButtonPressed);
		this.cbDistrRule.SettingInVisible = false;
		this.cbDistrRule.SettingReadonly = false;
		this.cbDistrRule.Size = new System.Drawing.Size(128, 20);
		this.cbDistrRule.TabIndex = 18;
		this.cbDistrRule.Validating += new System.ComponentModel.CancelEventHandler(this.cbDistrRule_Validating);
		this.lb_Customer.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lb_Customer.EditControl = this.txtCustomer;
		this.lb_Customer.EditValue = "Customer";
		this.lb_Customer.FieldName = null;
		this.lb_Customer.HideWhenEmpty = false;
		this.lb_Customer.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Customer.Location = new System.Drawing.Point(440, 151);
		this.lb_Customer.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_Customer.Name = "lb_Customer";
		this.lb_Customer.ReadOnly = false;
		this.lb_Customer.SettingInVisible = false;
		this.lb_Customer.SettingReadonly = false;
		this.lb_Customer.ShowUnderLine = true;
		this.lb_Customer.Size = new System.Drawing.Size(180, 20);
		this.lb_Customer.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Customer.TabIndex = 4;
		this.lb_Customer.Utext = "Customer";
		this.txtCustomer.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txtCustomer.EditControl = null;
		this.txtCustomer.FieldName = "CardCode";
		this.txtCustomer.IsSearchControl = false;
		this.txtCustomer.Location = new System.Drawing.Point(619, 149);
		this.txtCustomer.Name = "txtCustomer";
		editorButtonImageOptions2.Image = Apzon.Forms.Properties.Resources.right;
		this.txtCustomer.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.txtCustomer.Properties.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtCustomer_Properties_ButtonPressed);
		this.txtCustomer.Properties.Enter += new System.EventHandler(this.Control_Enter);
		this.txtCustomer.SettingInVisible = false;
		this.txtCustomer.SettingReadonly = false;
		this.txtCustomer.Size = new System.Drawing.Size(128, 20);
		this.txtCustomer.TabIndex = 17;
		this.txtCustomer.Leave += new System.EventHandler(this.Control_Leave);
		this.lb_SalesOrder.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lb_SalesOrder.EditControl = this.txtSaleOrder;
		this.lb_SalesOrder.EditValue = "Sales Order";
		this.lb_SalesOrder.FieldName = null;
		this.lb_SalesOrder.HideWhenEmpty = false;
		this.lb_SalesOrder.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_SalesOrder.Location = new System.Drawing.Point(440, 131);
		this.lb_SalesOrder.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_SalesOrder.Name = "lb_SalesOrder";
		this.lb_SalesOrder.ReadOnly = false;
		this.lb_SalesOrder.SettingInVisible = false;
		this.lb_SalesOrder.SettingReadonly = false;
		this.lb_SalesOrder.ShowUnderLine = true;
		this.lb_SalesOrder.Size = new System.Drawing.Size(180, 20);
		this.lb_SalesOrder.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_SalesOrder.TabIndex = 5;
		this.lb_SalesOrder.Utext = "Sales Order";
		this.txtSaleOrder.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txtSaleOrder.EditControl = null;
		this.txtSaleOrder.FieldName = "OriginNum";
		this.txtSaleOrder.IsSearchControl = false;
		this.txtSaleOrder.Location = new System.Drawing.Point(619, 128);
		this.txtSaleOrder.Name = "txtSaleOrder";
		this.txtSaleOrder.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.txtSaleOrder.Properties.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtSaleOrder_Properties_ButtonPressed);
		this.txtSaleOrder.SettingInVisible = false;
		this.txtSaleOrder.SettingReadonly = false;
		this.txtSaleOrder.Size = new System.Drawing.Size(128, 20);
		this.txtSaleOrder.TabIndex = 16;
		this.txtSaleOrder.Enter += new System.EventHandler(this.Control_Enter);
		this.txtSaleOrder.Leave += new System.EventHandler(this.Control_Leave);
		this.txtSaleOrder.Validating += new System.ComponentModel.CancelEventHandler(this.txtSaleOrder_Validating);
		this.lb_Origin.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lb_Origin.EditControl = this.look_OriginType;
		this.lb_Origin.EditValue = "Origin";
		this.lb_Origin.FieldName = null;
		this.lb_Origin.HideWhenEmpty = false;
		this.lb_Origin.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Origin.Location = new System.Drawing.Point(440, 110);
		this.lb_Origin.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_Origin.Name = "lb_Origin";
		this.lb_Origin.ReadOnly = true;
		this.lb_Origin.SettingInVisible = false;
		this.lb_Origin.SettingReadonly = false;
		this.lb_Origin.ShowUnderLine = true;
		this.lb_Origin.Size = new System.Drawing.Size(180, 20);
		this.lb_Origin.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Origin.TabIndex = 6;
		this.lb_Origin.Utext = "Origin";
		this.look_OriginType.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.look_OriginType.EditControl = null;
		this.look_OriginType.FieldName = "OriginType";
		this.look_OriginType.IsSearchControl = false;
		this.look_OriginType.Location = new System.Drawing.Point(619, 107);
		this.look_OriginType.Name = "look_OriginType";
		this.look_OriginType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.look_OriginType.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.look_OriginType.Properties.DisplayMember = "Name";
		this.look_OriginType.Properties.IsSearchControl = false;
		this.look_OriginType.Properties.NullText = "";
		this.look_OriginType.Properties.ReadOnly = true;
		this.look_OriginType.Properties.ValueMember = "Code";
		this.look_OriginType.SettingInVisible = false;
		this.look_OriginType.SettingReadonly = false;
		this.look_OriginType.Size = new System.Drawing.Size(128, 20);
		this.look_OriginType.TabIndex = 15;
		this.lb_User.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lb_User.EditControl = this.luk_Doc_User;
		this.lb_User.EditValue = "User";
		this.lb_User.FieldName = null;
		this.lb_User.HideWhenEmpty = false;
		this.lb_User.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_User.Location = new System.Drawing.Point(440, 89);
		this.lb_User.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_User.Name = "lb_User";
		this.lb_User.ReadOnly = false;
		this.lb_User.SettingInVisible = false;
		this.lb_User.SettingReadonly = false;
		this.lb_User.ShowUnderLine = true;
		this.lb_User.Size = new System.Drawing.Size(180, 20);
		this.lb_User.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_User.TabIndex = 7;
		this.lb_User.Utext = "User";
		this.luk_Doc_User.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.luk_Doc_User.EditControl = null;
		this.luk_Doc_User.FieldName = "UserSign";
		this.luk_Doc_User.IsSearchControl = false;
		this.luk_Doc_User.Location = new System.Drawing.Point(619, 86);
		this.luk_Doc_User.Name = "luk_Doc_User";
		this.luk_Doc_User.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Doc_User.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.luk_Doc_User.Properties.DisplayMember = "Name";
		this.luk_Doc_User.Properties.IsSearchControl = false;
		this.luk_Doc_User.Properties.NullText = "";
		this.luk_Doc_User.Properties.ValueMember = "Code";
		this.luk_Doc_User.SettingInVisible = false;
		this.luk_Doc_User.SettingReadonly = false;
		this.luk_Doc_User.Size = new System.Drawing.Size(128, 20);
		this.luk_Doc_User.TabIndex = 14;
		this.lb_DueDate.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lb_DueDate.EditControl = this.dateDue;
		this.lb_DueDate.EditValue = "Due Date";
		this.lb_DueDate.FieldName = null;
		this.lb_DueDate.HideWhenEmpty = false;
		this.lb_DueDate.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_DueDate.Location = new System.Drawing.Point(440, 69);
		this.lb_DueDate.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_DueDate.Name = "lb_DueDate";
		this.lb_DueDate.ReadOnly = false;
		this.lb_DueDate.SettingInVisible = false;
		this.lb_DueDate.SettingReadonly = false;
		this.lb_DueDate.ShowUnderLine = true;
		this.lb_DueDate.Size = new System.Drawing.Size(180, 20);
		this.lb_DueDate.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_DueDate.TabIndex = 8;
		this.lb_DueDate.Utext = "Due Date";
		this.dateDue.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.dateDue.EditControl = null;
		this.dateDue.EditValue = null;
		this.dateDue.FieldName = "DueDate";
		this.dateDue.Location = new System.Drawing.Point(619, 66);
		this.dateDue.Name = "dateDue";
		this.dateDue.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dateDue.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dateDue.Properties.IsSearchControl = false;
		this.dateDue.SettingInVisible = false;
		this.dateDue.SettingReadonly = false;
		this.dateDue.Size = new System.Drawing.Size(128, 20);
		this.dateDue.TabIndex = 13;
		this.lb_OrderDate.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lb_OrderDate.EditControl = this.dateOrder;
		this.lb_OrderDate.EditValue = "Order Date";
		this.lb_OrderDate.FieldName = null;
		this.lb_OrderDate.HideWhenEmpty = false;
		this.lb_OrderDate.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_OrderDate.Location = new System.Drawing.Point(440, 27);
		this.lb_OrderDate.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_OrderDate.Name = "lb_OrderDate";
		this.lb_OrderDate.ReadOnly = false;
		this.lb_OrderDate.SettingInVisible = false;
		this.lb_OrderDate.SettingReadonly = false;
		this.lb_OrderDate.ShowUnderLine = true;
		this.lb_OrderDate.Size = new System.Drawing.Size(180, 20);
		this.lb_OrderDate.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_OrderDate.TabIndex = 9;
		this.lb_OrderDate.Utext = "Order Date";
		this.dateOrder.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.dateOrder.EditControl = null;
		this.dateOrder.EditValue = null;
		this.dateOrder.FieldName = "PostDate";
		this.dateOrder.Location = new System.Drawing.Point(619, 24);
		this.dateOrder.Name = "dateOrder";
		this.dateOrder.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dateOrder.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dateOrder.Properties.IsSearchControl = false;
		this.dateOrder.SettingInVisible = false;
		this.dateOrder.SettingReadonly = false;
		this.dateOrder.Size = new System.Drawing.Size(128, 20);
		this.dateOrder.TabIndex = 11;
		this.dateOrder.EditValueChanged += new System.EventHandler(this.dateOrder_EditValueChanged);
		this.lb_No.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lb_No.EditControl = this.txtNo;
		this.lb_No.EditValue = "No.";
		this.lb_No.FieldName = null;
		this.lb_No.HideWhenEmpty = false;
		this.lb_No.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_No.Location = new System.Drawing.Point(440, 5);
		this.lb_No.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_No.Name = "lb_No";
		this.lb_No.ReadOnly = false;
		this.lb_No.SettingInVisible = false;
		this.lb_No.SettingReadonly = false;
		this.lb_No.ShowUnderLine = true;
		this.lb_No.Size = new System.Drawing.Size(75, 20);
		this.lb_No.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_No.TabIndex = 10;
		this.lb_No.Utext = "No.";
		this.txtNo.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txtNo.EditControl = null;
		this.txtNo.FieldName = "DocNum";
		this.txtNo.IsSearchControl = false;
		this.txtNo.Location = new System.Drawing.Point(619, 3);
		this.txtNo.Name = "txtNo";
		this.txtNo.SettingInVisible = false;
		this.txtNo.SettingReadonly = false;
		this.txtNo.Size = new System.Drawing.Size(128, 20);
		this.txtNo.TabIndex = 10;
		this.lb_PlannedQuantity.EditControl = this.calcPlannedQuantity;
		this.lb_PlannedQuantity.EditValue = "Planned Quantity";
		this.lb_PlannedQuantity.FieldName = null;
		this.lb_PlannedQuantity.HideWhenEmpty = false;
		this.lb_PlannedQuantity.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_PlannedQuantity.Location = new System.Drawing.Point(8, 89);
		this.lb_PlannedQuantity.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_PlannedQuantity.Name = "lb_PlannedQuantity";
		this.lb_PlannedQuantity.ReadOnly = false;
		this.lb_PlannedQuantity.SettingInVisible = false;
		this.lb_PlannedQuantity.SettingReadonly = false;
		this.lb_PlannedQuantity.ShowUnderLine = true;
		this.lb_PlannedQuantity.Size = new System.Drawing.Size(130, 20);
		this.lb_PlannedQuantity.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_PlannedQuantity.TabIndex = 11;
		this.lb_PlannedQuantity.Utext = "Planned Quantity";
		this.calcPlannedQuantity.EditControl = null;
		this.calcPlannedQuantity.FieldName = "PlannedQty";
		this.calcPlannedQuantity.IsSearchControl = false;
		this.calcPlannedQuantity.Location = new System.Drawing.Point(137, 86);
		this.calcPlannedQuantity.Name = "calcPlannedQuantity";
		this.calcPlannedQuantity.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.calcPlannedQuantity.Properties.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.calcPlannedQuantity.Properties.IsSearchControl = false;
		this.calcPlannedQuantity.SettingInVisible = false;
		this.calcPlannedQuantity.SettingReadonly = false;
		this.calcPlannedQuantity.Size = new System.Drawing.Size(135, 20);
		this.calcPlannedQuantity.TabIndex = 57;
		this.calcPlannedQuantity.EditValueChanged += new System.EventHandler(this.calcPlannedQuantity_EditValueChanged);
		this.lb_ProductDescription.EditControl = this.txtProductDesc;
		this.lb_ProductDescription.EditValue = "Product Description";
		this.lb_ProductDescription.FieldName = null;
		this.lb_ProductDescription.HideWhenEmpty = false;
		this.lb_ProductDescription.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_ProductDescription.Location = new System.Drawing.Point(8, 68);
		this.lb_ProductDescription.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_ProductDescription.Name = "lb_ProductDescription";
		this.lb_ProductDescription.ReadOnly = false;
		this.lb_ProductDescription.SettingInVisible = false;
		this.lb_ProductDescription.SettingReadonly = false;
		this.lb_ProductDescription.ShowUnderLine = true;
		this.lb_ProductDescription.Size = new System.Drawing.Size(130, 20);
		this.lb_ProductDescription.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_ProductDescription.TabIndex = 12;
		this.lb_ProductDescription.Utext = "Product Description";
		this.txtProductDesc.EditControl = null;
		this.txtProductDesc.FieldName = "ItemName";
		this.txtProductDesc.IsSearchControl = false;
		this.txtProductDesc.Location = new System.Drawing.Point(137, 65);
		this.txtProductDesc.Name = "txtProductDesc";
		this.txtProductDesc.SettingInVisible = false;
		this.txtProductDesc.SettingReadonly = false;
		this.txtProductDesc.Size = new System.Drawing.Size(272, 20);
		this.txtProductDesc.TabIndex = 4;
		this.lb_ProductNo.EditControl = this.txtProductNo;
		this.lb_ProductNo.EditValue = "Product No.";
		this.lb_ProductNo.FieldName = null;
		this.lb_ProductNo.HideWhenEmpty = false;
		this.lb_ProductNo.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_ProductNo.Location = new System.Drawing.Point(8, 47);
		this.lb_ProductNo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_ProductNo.Name = "lb_ProductNo";
		this.lb_ProductNo.ReadOnly = false;
		this.lb_ProductNo.SettingInVisible = false;
		this.lb_ProductNo.SettingReadonly = false;
		this.lb_ProductNo.ShowUnderLine = true;
		this.lb_ProductNo.Size = new System.Drawing.Size(130, 20);
		this.lb_ProductNo.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_ProductNo.TabIndex = 13;
		this.lb_ProductNo.Utext = "Product No.";
		this.txtProductNo.EditControl = null;
		this.txtProductNo.FieldName = "ItemCode";
		this.txtProductNo.IsSearchControl = false;
		this.txtProductNo.Location = new System.Drawing.Point(137, 44);
		this.txtProductNo.Name = "txtProductNo";
		this.txtProductNo.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions3.Image = Apzon.Forms.Properties.Resources.right;
		this.txtProductNo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance4, appearanceHovered4, appearancePressed4, appearanceDisabled4, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance5, appearanceHovered5, appearancePressed5, appearanceDisabled5, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.txtProductNo.Properties.ContainerControl = this;
		this.txtProductNo.Properties.DisplayMember = "";
		this.txtProductNo.Properties.DisplayMember2 = "";
		this.txtProductNo.Properties.ListMandatoryField = null;
		this.txtProductNo.Properties.LoadDataSourceWhenEnter = false;
		this.txtProductNo.Properties.SeparateCharacterForMulti = ';';
		this.txtProductNo.Properties.SeparateString = ", ";
		this.txtProductNo.Properties.UsingAutoCompleteBox = false;
		this.txtProductNo.Properties.UsingDataSource = true;
		this.txtProductNo.Properties.ValidateWhenNoDataSource = false;
		this.txtProductNo.Properties.ValueMember = "";
		this.txtProductNo.SettingInVisible = false;
		this.txtProductNo.SettingReadonly = false;
		this.txtProductNo.Size = new System.Drawing.Size(135, 20);
		this.txtProductNo.TabIndex = 58;
		this.lb_Status.EditControl = this.luk_Doc_Status;
		this.lb_Status.EditValue = "Status";
		this.lb_Status.FieldName = null;
		this.lb_Status.HideWhenEmpty = false;
		this.lb_Status.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Status.Location = new System.Drawing.Point(8, 26);
		this.lb_Status.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_Status.Name = "lb_Status";
		this.lb_Status.ReadOnly = false;
		this.lb_Status.SettingInVisible = false;
		this.lb_Status.SettingReadonly = false;
		this.lb_Status.ShowUnderLine = true;
		this.lb_Status.Size = new System.Drawing.Size(130, 20);
		this.lb_Status.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Status.TabIndex = 14;
		this.lb_Status.Utext = "Status";
		this.luk_Doc_Status.EditControl = null;
		this.luk_Doc_Status.FieldName = "Status";
		this.luk_Doc_Status.IsSearchControl = false;
		this.luk_Doc_Status.Location = new System.Drawing.Point(137, 23);
		this.luk_Doc_Status.Name = "luk_Doc_Status";
		this.luk_Doc_Status.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Doc_Status.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.luk_Doc_Status.Properties.DisplayMember = "Name";
		this.luk_Doc_Status.Properties.IsSearchControl = false;
		this.luk_Doc_Status.Properties.NullText = "";
		this.luk_Doc_Status.Properties.ValueMember = "Code";
		this.luk_Doc_Status.SettingInVisible = false;
		this.luk_Doc_Status.SettingReadonly = false;
		this.luk_Doc_Status.Size = new System.Drawing.Size(135, 20);
		this.luk_Doc_Status.TabIndex = 2;
		this.luk_Doc_Status.EditValueChanged += new System.EventHandler(this.luk_Doc_Status_EditValueChanged);
		this.txtUoMName.EditControl = null;
		this.txtUoMName.FieldName = "Uom";
		this.txtUoMName.IsSearchControl = false;
		this.txtUoMName.Location = new System.Drawing.Point(332, 86);
		this.txtUoMName.Name = "txtUoMName";
		this.txtUoMName.SettingInVisible = false;
		this.txtUoMName.SettingReadonly = false;
		this.txtUoMName.Size = new System.Drawing.Size(77, 20);
		this.txtUoMName.TabIndex = 6;
		this.xtraTabControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.xtraTabControl1.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.xtraTabControl1.Appearance.Options.UseBackColor = true;
		this.xtraTabControl1.IsSearchControl = false;
		this.xtraTabControl1.Location = new System.Drawing.Point(8, 219);
		this.xtraTabControl1.Name = "xtraTabControl1";
		this.xtraTabControl1.SelectedTabPage = this.tab_Components;
		this.xtraTabControl1.Size = new System.Drawing.Size(739, 334);
		this.xtraTabControl1.TabIndex = 21;
		this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[4] { this.tab_Components, this.tab_Summary, this.tab_Attachments, this.tab_UDF });
		this.tab_Components.Controls.Add(this.spl_LinesBatchSerial);
		this.tab_Components.EditControl = null;
		this.tab_Components.EditValue = "Components";
		this.tab_Components.FieldName = "tab_Components";
		this.tab_Components.IsSearchControl = false;
		this.tab_Components.Name = "tab_Components";
		this.tab_Components.ReadOnly = false;
		this.tab_Components.SettingInVisible = false;
		this.tab_Components.SettingReadonly = false;
		this.tab_Components.Size = new System.Drawing.Size(737, 309);
		this.tab_Components.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Components.Text = "Components";
		this.spl_LinesBatchSerial.Dock = System.Windows.Forms.DockStyle.Fill;
		this.spl_LinesBatchSerial.Horizontal = false;
		this.spl_LinesBatchSerial.Location = new System.Drawing.Point(0, 0);
		this.spl_LinesBatchSerial.Name = "spl_LinesBatchSerial";
		this.spl_LinesBatchSerial.Panel1.Controls.Add(this.grdDoc);
		this.spl_LinesBatchSerial.Panel1.Text = "Panel1";
		this.spl_LinesBatchSerial.Panel2.Controls.Add(this.ucBatchAndSerial1);
		this.spl_LinesBatchSerial.Panel2.Text = "Panel2";
		this.spl_LinesBatchSerial.Size = new System.Drawing.Size(737, 309);
		this.spl_LinesBatchSerial.SplitterPosition = 170;
		this.spl_LinesBatchSerial.TabIndex = 20;
		this.spl_LinesBatchSerial.Text = "splitContainerControl1";
		this.grdDoc.AllowPasteExcel = false;
		this.grdDoc.ClipboardData = null;
		this.grdDoc.ContextMenuStrip = this.contextMenuStrip_Line;
		this.grdDoc.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grdDoc.Location = new System.Drawing.Point(0, 0);
		this.grdDoc.MainView = this.gvDoc;
		this.grdDoc.Name = "grdDoc";
		this.grdDoc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[7] { this.rBT_WibActCode, this.rBT_ItemCode, this.rBT_WareH, this.repositoryItemCalcEdit1, this.rep_bte_lineProject, this.rep_Line_Type, this.rep_line_IssueMtd });
		this.grdDoc.Size = new System.Drawing.Size(737, 170);
		this.grdDoc.TabIndex = 19;
		this.grdDoc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvDoc });
		this.grdDoc.Paint += new System.Windows.Forms.PaintEventHandler(this.grdDoc_Paint);
		this.grdDoc.EditorKeyDown += new System.Windows.Forms.KeyEventHandler(this.grdDoc_EditorKeyDown);
		this.contextMenuStrip_Line.Items.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.deleteToolStripMenuItem, this.AddRowToolStripMenuItem, this.batchSerialToolStripMenuItem });
		this.contextMenuStrip_Line.Name = "contextMenuStrip1";
		this.contextMenuStrip_Line.Size = new System.Drawing.Size(185, 70);
		this.contextMenuStrip_Line.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Line_Opening);
		this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
		this.deleteToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
		this.deleteToolStripMenuItem.Text = "Delete";
		this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
		this.AddRowToolStripMenuItem.Name = "AddRowToolStripMenuItem";
		this.AddRowToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
		this.AddRowToolStripMenuItem.Text = "Add Row";
		this.AddRowToolStripMenuItem.Click += new System.EventHandler(this.AddNewToolStripMenuItem_Click);
		this.batchSerialToolStripMenuItem.Name = "batchSerialToolStripMenuItem";
		this.batchSerialToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
		this.batchSerialToolStripMenuItem.Text = "Batch/Serial Number";
		this.batchSerialToolStripMenuItem.Click += new System.EventHandler(this.batchSerialToolStripMenuItem_Click);
		this.gvDoc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvDoc.ColumnKey = null;
		this.gvDoc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[43]
		{
			this.gcDocEntry, this.gcLineNum, this.gcItemType, this.gcItemCode, this.gcItemName, this.gcBaseQty, this.gcPlannedQty, this.gcIssuedQty, this.gcAvailable, this.gcwareHouse,
			this.gcVisOrder, this.gcWipActCode, this.gcCompTotal, this.gcOcrCode, this.gcOcrCode2, this.gcOcrCode3, this.gcOcrCode4, this.gcOcrCode5, this.gcLocCode, this.gcLogInstanc,
			this.gcLoadFrBOM, this.gcProject, this.gcUomEntry, this.gcUomCode, this.gcAdditQty, this.gcLineText, this.gcPickStatus, this.gcPickQty, this.gcPickIdNo, this.gcReleaseQty,
			this.gcResAlloc, this.gcUomName, this.gcIssueType, this.gcOnHand, this.gcIsCommited, this.gcOnOrder, this.gridColumnEx1, this.gridColumnEx2, this.gridColumnEx3, this.gridColumnEx4,
			this.gridColumnEx5, this.gridColumnEx6, this.gridColumnEx7
		});
		this.gvDoc.DisableFormSetting = false;
		this.gvDoc.DisplayException = true;
		this.gvDoc.FieldCurrency = null;
		this.gvDoc.FieldRate = null;
		this.gvDoc.FormId = "oProductionOrder";
		this.gvDoc.GridControl = this.grdDoc;
		this.gvDoc.IndicatorWidth = 35;
		this.gvDoc.IsApzTable = false;
		this.gvDoc.IsInsertNewRow = false;
		this.gvDoc.IsLoadingData = false;
		this.gvDoc.ItemId = "WOR1";
		this.gvDoc.LstColumnAllowBatch = null;
		this.gvDoc.LstColumnNotAllowBatch = null;
		this.gvDoc.LstColumnNotEditable = null;
		this.gvDoc.LstColumNotSetting = new System.Collections.Generic.List<string> { "OldLineQty", "OldFatherQty" };
		this.gvDoc.Name = "gvDoc";
		this.gvDoc.OptionsCustomization.AllowSort = false;
		this.gvDoc.OptionsView.ColumnAutoWidth = false;
		this.gvDoc.OptionsView.ShowDetailButtons = false;
		this.gvDoc.OptionsView.ShowGroupPanel = false;
		this.gvDoc.UseBatchMod = false;
		this.gvDoc.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gvDoc_CustomDrawRowIndicator);
		this.gvDoc.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gvDoc_RowCellStyle);
		this.gvDoc.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(this.gvDoc_PopupMenuShowing);
		this.gvDoc.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gvDoc_ShowingEditor);
		this.gvDoc.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gvDoc_FocusedRowObjectChanged);
		this.gvDoc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvDoc_CellValueChanged);
		this.gvDoc.ValidatingEditor += new DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventHandler(this.gvDoc_ValidatingEditor);
		this.gcDocEntry.Caption = "Doc Entry";
		this.gcDocEntry.CurrencyDateField = null;
		this.gcDocEntry.CurrencyField = null;
		this.gcDocEntry.CurrencyFixed = null;
		this.gcDocEntry.CurrencyFixedRate = 0.0;
		this.gcDocEntry.CurrencyRateField = null;
		this.gcDocEntry.FieldName = "DocEntry";
		this.gcDocEntry.Name = "gcDocEntry";
		this.gcDocEntry.OptionsColumn.AllowEdit = false;
		this.gcDocEntry.OptionsColumn.ReadOnly = true;
		this.gcDocEntry.PrimaryKey = false;
		this.gcDocEntry.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcDocEntry.UnitField = null;
		this.gcLineNum.Caption = "Line Num";
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
		this.gcLineNum.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcLineNum.UnitField = null;
		this.gcItemType.Caption = "ItemType";
		this.gcItemType.ColumnEdit = this.rep_Line_Type;
		this.gcItemType.CurrencyDateField = null;
		this.gcItemType.CurrencyField = null;
		this.gcItemType.CurrencyFixed = null;
		this.gcItemType.CurrencyFixedRate = 0.0;
		this.gcItemType.CurrencyRateField = null;
		this.gcItemType.FieldName = "ItemType";
		this.gcItemType.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcItemType.Name = "gcItemType";
		this.gcItemType.PrimaryKey = false;
		this.gcItemType.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcItemType.UnitField = null;
		this.gcItemType.Visible = true;
		this.gcItemType.VisibleIndex = 0;
		this.rep_Line_Type.AutoHeight = false;
		this.rep_Line_Type.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.rep_Line_Type.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Line_Type.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.rep_Line_Type.DisplayMember = "Name";
		this.rep_Line_Type.IsSearchControl = false;
		this.rep_Line_Type.Name = "rep_Line_Type";
		this.rep_Line_Type.NullText = "";
		this.rep_Line_Type.ShowFooter = false;
		this.rep_Line_Type.ShowHeader = false;
		this.rep_Line_Type.ValueMember = "Code";
		this.rep_Line_Type.EditValueChanged += new System.EventHandler(this.rep_Line_Type_EditValueChanged);
		this.gcItemCode.Caption = "Item Code";
		this.gcItemCode.ColumnEdit = this.rBT_ItemCode;
		this.gcItemCode.CurrencyDateField = null;
		this.gcItemCode.CurrencyField = null;
		this.gcItemCode.CurrencyFixed = null;
		this.gcItemCode.CurrencyFixedRate = 0.0;
		this.gcItemCode.CurrencyRateField = null;
		this.gcItemCode.FieldName = "ItemCode";
		this.gcItemCode.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcItemCode.Name = "gcItemCode";
		this.gcItemCode.PrimaryKey = false;
		this.gcItemCode.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcItemCode.UnitField = null;
		this.gcItemCode.Visible = true;
		this.gcItemCode.VisibleIndex = 1;
		this.gcItemCode.Width = 150;
		this.rBT_ItemCode.AutoHeight = false;
		editorButtonImageOptions4.Image = Apzon.Forms.Properties.Resources.right;
		this.rBT_ItemCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance6, appearanceHovered6, appearancePressed6, appearanceDisabled6, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rBT_ItemCode.Name = "rBT_ItemCode";
		this.rBT_ItemCode.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rBT_ItemCode_ButtonPressed);
		this.gcItemName.Caption = "Description";
		this.gcItemName.CurrencyDateField = null;
		this.gcItemName.CurrencyField = null;
		this.gcItemName.CurrencyFixed = null;
		this.gcItemName.CurrencyFixedRate = 0.0;
		this.gcItemName.CurrencyRateField = null;
		this.gcItemName.FieldName = "ItemName";
		this.gcItemName.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcItemName.Name = "gcItemName";
		this.gcItemName.OptionsColumn.AllowEdit = false;
		this.gcItemName.OptionsColumn.ReadOnly = true;
		this.gcItemName.PrimaryKey = false;
		this.gcItemName.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcItemName.UnitField = null;
		this.gcItemName.Visible = true;
		this.gcItemName.VisibleIndex = 2;
		this.gcItemName.Width = 150;
		this.gcBaseQty.Caption = "Base Qty";
		this.gcBaseQty.ColumnEdit = this.repositoryItemCalcEdit1;
		this.gcBaseQty.CurrencyDateField = null;
		this.gcBaseQty.CurrencyField = null;
		this.gcBaseQty.CurrencyFixed = null;
		this.gcBaseQty.CurrencyFixedRate = 0.0;
		this.gcBaseQty.CurrencyRateField = null;
		this.gcBaseQty.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gcBaseQty.FieldName = "BaseQty";
		this.gcBaseQty.Name = "gcBaseQty";
		this.gcBaseQty.PrimaryKey = false;
		this.gcBaseQty.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcBaseQty.UnitField = null;
		this.gcBaseQty.Visible = true;
		this.gcBaseQty.VisibleIndex = 3;
		this.repositoryItemCalcEdit1.AutoHeight = false;
		this.repositoryItemCalcEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEdit1.Name = "repositoryItemCalcEdit1";
		this.gcPlannedQty.Caption = "Planned Qty";
		this.gcPlannedQty.ColumnEdit = this.repositoryItemCalcEditEx1;
		this.gcPlannedQty.CurrencyDateField = null;
		this.gcPlannedQty.CurrencyField = null;
		this.gcPlannedQty.CurrencyFixed = null;
		this.gcPlannedQty.CurrencyFixedRate = 0.0;
		this.gcPlannedQty.CurrencyRateField = null;
		this.gcPlannedQty.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gcPlannedQty.FieldName = "PlannedQty";
		this.gcPlannedQty.Name = "gcPlannedQty";
		this.gcPlannedQty.PrimaryKey = false;
		this.gcPlannedQty.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcPlannedQty.UnitField = null;
		this.gcPlannedQty.Visible = true;
		this.gcPlannedQty.VisibleIndex = 4;
		this.gcIssuedQty.Caption = "Issued Qty";
		this.gcIssuedQty.ColumnEdit = this.repositoryItemCalcEditEx2;
		this.gcIssuedQty.CurrencyDateField = null;
		this.gcIssuedQty.CurrencyField = null;
		this.gcIssuedQty.CurrencyFixed = null;
		this.gcIssuedQty.CurrencyFixedRate = 0.0;
		this.gcIssuedQty.CurrencyRateField = null;
		this.gcIssuedQty.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gcIssuedQty.FieldName = "IssuedQty";
		this.gcIssuedQty.Name = "gcIssuedQty";
		this.gcIssuedQty.OptionsColumn.ReadOnly = true;
		this.gcIssuedQty.PrimaryKey = false;
		this.gcIssuedQty.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcIssuedQty.UnitField = null;
		this.gcIssuedQty.Visible = true;
		this.gcIssuedQty.VisibleIndex = 5;
		this.gcAvailable.Caption = "Available";
		this.gcAvailable.ColumnEdit = this.repositoryItemCalcEditEx3;
		this.gcAvailable.CurrencyDateField = null;
		this.gcAvailable.CurrencyField = null;
		this.gcAvailable.CurrencyFixed = null;
		this.gcAvailable.CurrencyFixedRate = 0.0;
		this.gcAvailable.CurrencyRateField = null;
		this.gcAvailable.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gcAvailable.FieldName = "Available";
		this.gcAvailable.Name = "gcAvailable";
		this.gcAvailable.OptionsColumn.AllowEdit = false;
		this.gcAvailable.OptionsColumn.ReadOnly = true;
		this.gcAvailable.PrimaryKey = false;
		this.gcAvailable.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcAvailable.UnitField = null;
		this.gcAvailable.Visible = true;
		this.gcAvailable.VisibleIndex = 6;
		this.gcwareHouse.Caption = "Ware House";
		this.gcwareHouse.ColumnEdit = this.rBT_WareH;
		this.gcwareHouse.CurrencyDateField = null;
		this.gcwareHouse.CurrencyField = null;
		this.gcwareHouse.CurrencyFixed = null;
		this.gcwareHouse.CurrencyFixedRate = 0.0;
		this.gcwareHouse.CurrencyRateField = null;
		this.gcwareHouse.FieldName = "wareHouse";
		this.gcwareHouse.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcwareHouse.Name = "gcwareHouse";
		this.gcwareHouse.PrimaryKey = false;
		this.gcwareHouse.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcwareHouse.UnitField = null;
		this.gcwareHouse.Visible = true;
		this.gcwareHouse.VisibleIndex = 9;
		this.rBT_WareH.AutoHeight = false;
		editorButtonImageOptions5.Image = Apzon.Forms.Properties.Resources.right;
		this.rBT_WareH.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions5, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance7, appearanceHovered7, appearancePressed7, appearanceDisabled7, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rBT_WareH.Name = "rBT_WareH";
		this.rBT_WareH.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rBT_WareH_ButtonPressed);
		this.gcVisOrder.Caption = "Vis Order";
		this.gcVisOrder.CurrencyDateField = null;
		this.gcVisOrder.CurrencyField = null;
		this.gcVisOrder.CurrencyFixed = null;
		this.gcVisOrder.CurrencyFixedRate = 0.0;
		this.gcVisOrder.CurrencyRateField = null;
		this.gcVisOrder.FieldName = "VisOrder";
		this.gcVisOrder.Name = "gcVisOrder";
		this.gcVisOrder.PrimaryKey = false;
		this.gcVisOrder.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcVisOrder.UnitField = null;
		this.gcWipActCode.Caption = "Wip Act Code";
		this.gcWipActCode.ColumnEdit = this.rBT_WibActCode;
		this.gcWipActCode.CurrencyDateField = null;
		this.gcWipActCode.CurrencyField = null;
		this.gcWipActCode.CurrencyFixed = null;
		this.gcWipActCode.CurrencyFixedRate = 0.0;
		this.gcWipActCode.CurrencyRateField = null;
		this.gcWipActCode.FieldName = "WipActCode";
		this.gcWipActCode.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcWipActCode.Name = "gcWipActCode";
		this.gcWipActCode.PrimaryKey = false;
		this.gcWipActCode.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcWipActCode.UnitField = null;
		this.rBT_WibActCode.AutoHeight = false;
		this.rBT_WibActCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, imageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance8, appearanceHovered8, appearancePressed8, appearanceDisabled8, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rBT_WibActCode.Name = "rBT_WibActCode";
		this.rBT_WibActCode.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rBT_WibActCode_ButtonPressed);
		this.gcCompTotal.Caption = "Comp Total";
		this.gcCompTotal.CurrencyDateField = null;
		this.gcCompTotal.CurrencyField = null;
		this.gcCompTotal.CurrencyFixed = null;
		this.gcCompTotal.CurrencyFixedRate = 0.0;
		this.gcCompTotal.CurrencyRateField = null;
		this.gcCompTotal.FieldName = "CompTotal";
		this.gcCompTotal.Name = "gcCompTotal";
		this.gcCompTotal.PrimaryKey = false;
		this.gcCompTotal.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcCompTotal.UnitField = null;
		this.gcOcrCode.Caption = "OcrCode";
		this.gcOcrCode.CurrencyDateField = null;
		this.gcOcrCode.CurrencyField = null;
		this.gcOcrCode.CurrencyFixed = null;
		this.gcOcrCode.CurrencyFixedRate = 0.0;
		this.gcOcrCode.CurrencyRateField = null;
		this.gcOcrCode.FieldName = "OcrCode";
		this.gcOcrCode.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcOcrCode.Name = "gcOcrCode";
		this.gcOcrCode.PrimaryKey = false;
		this.gcOcrCode.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcOcrCode.UnitField = null;
		this.gcOcrCode2.Caption = "OcrCode2";
		this.gcOcrCode2.CurrencyDateField = null;
		this.gcOcrCode2.CurrencyField = null;
		this.gcOcrCode2.CurrencyFixed = null;
		this.gcOcrCode2.CurrencyFixedRate = 0.0;
		this.gcOcrCode2.CurrencyRateField = null;
		this.gcOcrCode2.FieldName = "OcrCode2";
		this.gcOcrCode2.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcOcrCode2.Name = "gcOcrCode2";
		this.gcOcrCode2.PrimaryKey = false;
		this.gcOcrCode2.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcOcrCode2.UnitField = null;
		this.gcOcrCode3.Caption = "OcrCode3";
		this.gcOcrCode3.CurrencyDateField = null;
		this.gcOcrCode3.CurrencyField = null;
		this.gcOcrCode3.CurrencyFixed = null;
		this.gcOcrCode3.CurrencyFixedRate = 0.0;
		this.gcOcrCode3.CurrencyRateField = null;
		this.gcOcrCode3.FieldName = "OcrCode3";
		this.gcOcrCode3.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcOcrCode3.Name = "gcOcrCode3";
		this.gcOcrCode3.PrimaryKey = false;
		this.gcOcrCode3.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcOcrCode3.UnitField = null;
		this.gcOcrCode4.Caption = "OcrCode4";
		this.gcOcrCode4.CurrencyDateField = null;
		this.gcOcrCode4.CurrencyField = null;
		this.gcOcrCode4.CurrencyFixed = null;
		this.gcOcrCode4.CurrencyFixedRate = 0.0;
		this.gcOcrCode4.CurrencyRateField = null;
		this.gcOcrCode4.FieldName = "OcrCode4";
		this.gcOcrCode4.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcOcrCode4.Name = "gcOcrCode4";
		this.gcOcrCode4.PrimaryKey = false;
		this.gcOcrCode4.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcOcrCode4.UnitField = null;
		this.gcOcrCode5.Caption = "OcrCode5";
		this.gcOcrCode5.CurrencyDateField = null;
		this.gcOcrCode5.CurrencyField = null;
		this.gcOcrCode5.CurrencyFixed = null;
		this.gcOcrCode5.CurrencyFixedRate = 0.0;
		this.gcOcrCode5.CurrencyRateField = null;
		this.gcOcrCode5.FieldName = "OcrCode5";
		this.gcOcrCode5.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcOcrCode5.Name = "gcOcrCode5";
		this.gcOcrCode5.PrimaryKey = false;
		this.gcOcrCode5.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcOcrCode5.UnitField = null;
		this.gcLocCode.Caption = "LocCode";
		this.gcLocCode.CurrencyDateField = null;
		this.gcLocCode.CurrencyField = null;
		this.gcLocCode.CurrencyFixed = null;
		this.gcLocCode.CurrencyFixedRate = 0.0;
		this.gcLocCode.CurrencyRateField = null;
		this.gcLocCode.FieldName = "LocCode";
		this.gcLocCode.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcLocCode.Name = "gcLocCode";
		this.gcLocCode.PrimaryKey = false;
		this.gcLocCode.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcLocCode.UnitField = null;
		this.gcLogInstanc.Caption = "LogInstanc";
		this.gcLogInstanc.CurrencyDateField = null;
		this.gcLogInstanc.CurrencyField = null;
		this.gcLogInstanc.CurrencyFixed = null;
		this.gcLogInstanc.CurrencyFixedRate = 0.0;
		this.gcLogInstanc.CurrencyRateField = null;
		this.gcLogInstanc.FieldName = "LogInstanc";
		this.gcLogInstanc.Name = "gcLogInstanc";
		this.gcLogInstanc.PrimaryKey = false;
		this.gcLogInstanc.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcLogInstanc.UnitField = null;
		this.gcLoadFrBOM.Caption = "LoadFrBOM";
		this.gcLoadFrBOM.CurrencyDateField = null;
		this.gcLoadFrBOM.CurrencyField = null;
		this.gcLoadFrBOM.CurrencyFixed = null;
		this.gcLoadFrBOM.CurrencyFixedRate = 0.0;
		this.gcLoadFrBOM.CurrencyRateField = null;
		this.gcLoadFrBOM.FieldName = "LoadFrBOM";
		this.gcLoadFrBOM.Name = "gcLoadFrBOM";
		this.gcLoadFrBOM.PrimaryKey = false;
		this.gcLoadFrBOM.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcLoadFrBOM.UnitField = null;
		this.gcProject.Caption = "Project";
		this.gcProject.ColumnEdit = this.rep_bte_lineProject;
		this.gcProject.CurrencyDateField = null;
		this.gcProject.CurrencyField = null;
		this.gcProject.CurrencyFixed = null;
		this.gcProject.CurrencyFixedRate = 0.0;
		this.gcProject.CurrencyRateField = null;
		this.gcProject.FieldName = "Project";
		this.gcProject.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcProject.Name = "gcProject";
		this.gcProject.PrimaryKey = false;
		this.gcProject.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcProject.UnitField = null;
		this.gcProject.Visible = true;
		this.gcProject.VisibleIndex = 14;
		this.rep_bte_lineProject.AutoHeight = false;
		this.rep_bte_lineProject.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rep_bte_lineProject.Name = "rep_bte_lineProject";
		this.rep_bte_lineProject.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_bte_lineProject_ButtonPressed);
		this.gcUomEntry.Caption = "UomEntry";
		this.gcUomEntry.CurrencyDateField = null;
		this.gcUomEntry.CurrencyField = null;
		this.gcUomEntry.CurrencyFixed = null;
		this.gcUomEntry.CurrencyFixedRate = 0.0;
		this.gcUomEntry.CurrencyRateField = null;
		this.gcUomEntry.FieldName = "UomEntry";
		this.gcUomEntry.Name = "gcUomEntry";
		this.gcUomEntry.PrimaryKey = false;
		this.gcUomEntry.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcUomEntry.UnitField = null;
		this.gcUomCode.Caption = "UomCode";
		this.gcUomCode.CurrencyDateField = null;
		this.gcUomCode.CurrencyField = null;
		this.gcUomCode.CurrencyFixed = null;
		this.gcUomCode.CurrencyFixedRate = 0.0;
		this.gcUomCode.CurrencyRateField = null;
		this.gcUomCode.FieldName = "UomCode";
		this.gcUomCode.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcUomCode.Name = "gcUomCode";
		this.gcUomCode.OptionsColumn.ReadOnly = true;
		this.gcUomCode.PrimaryKey = false;
		this.gcUomCode.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcUomCode.UnitField = null;
		this.gcUomCode.Visible = true;
		this.gcUomCode.VisibleIndex = 7;
		this.gcAdditQty.Caption = "AdditQty";
		this.gcAdditQty.ColumnEdit = this.repositoryItemCalcEditEx4;
		this.gcAdditQty.CurrencyDateField = null;
		this.gcAdditQty.CurrencyField = null;
		this.gcAdditQty.CurrencyFixed = null;
		this.gcAdditQty.CurrencyFixedRate = 0.0;
		this.gcAdditQty.CurrencyRateField = null;
		this.gcAdditQty.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gcAdditQty.FieldName = "AdditQty";
		this.gcAdditQty.Name = "gcAdditQty";
		this.gcAdditQty.PrimaryKey = false;
		this.gcAdditQty.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcAdditQty.UnitField = null;
		this.gcLineText.Caption = "LineText";
		this.gcLineText.CurrencyDateField = null;
		this.gcLineText.CurrencyField = null;
		this.gcLineText.CurrencyFixed = null;
		this.gcLineText.CurrencyFixedRate = 0.0;
		this.gcLineText.CurrencyRateField = null;
		this.gcLineText.FieldName = "LineText";
		this.gcLineText.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcLineText.Name = "gcLineText";
		this.gcLineText.PrimaryKey = false;
		this.gcLineText.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcLineText.UnitField = null;
		this.gcPickStatus.Caption = "PickStatus";
		this.gcPickStatus.CurrencyDateField = null;
		this.gcPickStatus.CurrencyField = null;
		this.gcPickStatus.CurrencyFixed = null;
		this.gcPickStatus.CurrencyFixedRate = 0.0;
		this.gcPickStatus.CurrencyRateField = null;
		this.gcPickStatus.FieldName = "PickStatus";
		this.gcPickStatus.Name = "gcPickStatus";
		this.gcPickStatus.PrimaryKey = false;
		this.gcPickStatus.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcPickStatus.UnitField = null;
		this.gcPickQty.Caption = "PickQty";
		this.gcPickQty.CurrencyDateField = null;
		this.gcPickQty.CurrencyField = null;
		this.gcPickQty.CurrencyFixed = null;
		this.gcPickQty.CurrencyFixedRate = 0.0;
		this.gcPickQty.CurrencyRateField = null;
		this.gcPickQty.FieldName = "PickQty";
		this.gcPickQty.Name = "gcPickQty";
		this.gcPickQty.PrimaryKey = false;
		this.gcPickQty.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcPickQty.UnitField = null;
		this.gcPickIdNo.Caption = "PickIdNo";
		this.gcPickIdNo.CurrencyDateField = null;
		this.gcPickIdNo.CurrencyField = null;
		this.gcPickIdNo.CurrencyFixed = null;
		this.gcPickIdNo.CurrencyFixedRate = 0.0;
		this.gcPickIdNo.CurrencyRateField = null;
		this.gcPickIdNo.FieldName = "PickIdNo";
		this.gcPickIdNo.Name = "gcPickIdNo";
		this.gcPickIdNo.PrimaryKey = false;
		this.gcPickIdNo.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcPickIdNo.UnitField = null;
		this.gcReleaseQty.Caption = "ReleaseQty";
		this.gcReleaseQty.ColumnEdit = this.repositoryItemCalcEditEx5;
		this.gcReleaseQty.CurrencyDateField = null;
		this.gcReleaseQty.CurrencyField = null;
		this.gcReleaseQty.CurrencyFixed = null;
		this.gcReleaseQty.CurrencyFixedRate = 0.0;
		this.gcReleaseQty.CurrencyRateField = null;
		this.gcReleaseQty.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gcReleaseQty.FieldName = "ReleaseQty";
		this.gcReleaseQty.Name = "gcReleaseQty";
		this.gcReleaseQty.PrimaryKey = false;
		this.gcReleaseQty.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcReleaseQty.UnitField = null;
		this.gcResAlloc.Caption = "ResAlloc";
		this.gcResAlloc.CurrencyDateField = null;
		this.gcResAlloc.CurrencyField = null;
		this.gcResAlloc.CurrencyFixed = null;
		this.gcResAlloc.CurrencyFixedRate = 0.0;
		this.gcResAlloc.CurrencyRateField = null;
		this.gcResAlloc.FieldName = "ResAlloc";
		this.gcResAlloc.Name = "gcResAlloc";
		this.gcResAlloc.PrimaryKey = false;
		this.gcResAlloc.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcResAlloc.UnitField = null;
		this.gcUomName.Caption = "UoM Name";
		this.gcUomName.CurrencyDateField = null;
		this.gcUomName.CurrencyField = null;
		this.gcUomName.CurrencyFixed = null;
		this.gcUomName.CurrencyFixedRate = 0.0;
		this.gcUomName.CurrencyRateField = null;
		this.gcUomName.FieldName = "InvntryUom";
		this.gcUomName.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcUomName.Name = "gcUomName";
		this.gcUomName.OptionsColumn.ReadOnly = true;
		this.gcUomName.PrimaryKey = false;
		this.gcUomName.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcUomName.UnitField = null;
		this.gcUomName.Visible = true;
		this.gcUomName.VisibleIndex = 8;
		this.gcIssueType.Caption = "Issue Method";
		this.gcIssueType.ColumnEdit = this.rep_line_IssueMtd;
		this.gcIssueType.CurrencyDateField = null;
		this.gcIssueType.CurrencyField = null;
		this.gcIssueType.CurrencyFixed = null;
		this.gcIssueType.CurrencyFixedRate = 0.0;
		this.gcIssueType.CurrencyRateField = null;
		this.gcIssueType.FieldName = "IssueType";
		this.gcIssueType.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcIssueType.Name = "gcIssueType";
		this.gcIssueType.PrimaryKey = false;
		this.gcIssueType.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcIssueType.UnitField = null;
		this.gcIssueType.Visible = true;
		this.gcIssueType.VisibleIndex = 10;
		this.gcIssueType.Width = 80;
		this.rep_line_IssueMtd.AutoHeight = false;
		this.rep_line_IssueMtd.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.rep_line_IssueMtd.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_line_IssueMtd.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.rep_line_IssueMtd.DisplayMember = "Name";
		this.rep_line_IssueMtd.IsSearchControl = false;
		this.rep_line_IssueMtd.Name = "rep_line_IssueMtd";
		this.rep_line_IssueMtd.NullText = "";
		this.rep_line_IssueMtd.ShowFooter = false;
		this.rep_line_IssueMtd.ShowHeader = false;
		this.rep_line_IssueMtd.ValueMember = "Code";
		this.gcOnHand.Caption = "In Stock";
		this.gcOnHand.ColumnEdit = this.repositoryItemCalcEditEx6;
		this.gcOnHand.CurrencyDateField = null;
		this.gcOnHand.CurrencyField = null;
		this.gcOnHand.CurrencyFixed = null;
		this.gcOnHand.CurrencyFixedRate = 0.0;
		this.gcOnHand.CurrencyRateField = null;
		this.gcOnHand.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gcOnHand.FieldName = "OnHand";
		this.gcOnHand.Name = "gcOnHand";
		this.gcOnHand.OptionsColumn.AllowEdit = false;
		this.gcOnHand.OptionsColumn.ReadOnly = true;
		this.gcOnHand.PrimaryKey = false;
		this.gcOnHand.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcOnHand.UnitField = null;
		this.gcOnHand.Visible = true;
		this.gcOnHand.VisibleIndex = 11;
		this.gcIsCommited.Caption = "Is Commited";
		this.gcIsCommited.ColumnEdit = this.repositoryItemCalcEditEx7;
		this.gcIsCommited.CurrencyDateField = null;
		this.gcIsCommited.CurrencyField = null;
		this.gcIsCommited.CurrencyFixed = null;
		this.gcIsCommited.CurrencyFixedRate = 0.0;
		this.gcIsCommited.CurrencyRateField = null;
		this.gcIsCommited.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gcIsCommited.FieldName = "IsCommited";
		this.gcIsCommited.Name = "gcIsCommited";
		this.gcIsCommited.OptionsColumn.ReadOnly = true;
		this.gcIsCommited.PrimaryKey = false;
		this.gcIsCommited.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcIsCommited.UnitField = null;
		this.gcIsCommited.Visible = true;
		this.gcIsCommited.VisibleIndex = 12;
		this.gcOnOrder.Caption = "On Order";
		this.gcOnOrder.ColumnEdit = this.repositoryItemCalcEditEx8;
		this.gcOnOrder.CurrencyDateField = null;
		this.gcOnOrder.CurrencyField = null;
		this.gcOnOrder.CurrencyFixed = null;
		this.gcOnOrder.CurrencyFixedRate = 0.0;
		this.gcOnOrder.CurrencyRateField = null;
		this.gcOnOrder.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gcOnOrder.FieldName = "OnOrder";
		this.gcOnOrder.Name = "gcOnOrder";
		this.gcOnOrder.OptionsColumn.ReadOnly = true;
		this.gcOnOrder.PrimaryKey = false;
		this.gcOnOrder.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcOnOrder.UnitField = null;
		this.gcOnOrder.Visible = true;
		this.gcOnOrder.VisibleIndex = 13;
		this.gridColumnEx1.Caption = "Start Date";
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gridColumnEx1.FieldName = "StartDate";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.PrimaryKey = false;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx1.Visible = true;
		this.gridColumnEx1.VisibleIndex = 15;
		this.gridColumnEx2.Caption = "End Date";
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gridColumnEx2.FieldName = "EndDate";
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.UnitField = null;
		this.gridColumnEx2.Visible = true;
		this.gridColumnEx2.VisibleIndex = 16;
		this.gridColumnEx3.Caption = "Qty In Whse";
		this.gridColumnEx3.CurrencyDateField = null;
		this.gridColumnEx3.CurrencyField = null;
		this.gridColumnEx3.CurrencyFixed = null;
		this.gridColumnEx3.CurrencyFixedRate = 0.0;
		this.gridColumnEx3.CurrencyRateField = null;
		this.gridColumnEx3.FieldName = "OnHandInWhse";
		this.gridColumnEx3.Name = "gridColumnEx3";
		this.gridColumnEx3.PrimaryKey = false;
		this.gridColumnEx3.UnitField = null;
		this.gridColumnEx3.Visible = true;
		this.gridColumnEx3.VisibleIndex = 17;
		this.gridColumnEx4.Caption = "Commited In Whse";
		this.gridColumnEx4.CurrencyDateField = null;
		this.gridColumnEx4.CurrencyField = null;
		this.gridColumnEx4.CurrencyFixed = null;
		this.gridColumnEx4.CurrencyFixedRate = 0.0;
		this.gridColumnEx4.CurrencyRateField = null;
		this.gridColumnEx4.FieldName = "IsCommitedInWhse";
		this.gridColumnEx4.Name = "gridColumnEx4";
		this.gridColumnEx4.PrimaryKey = false;
		this.gridColumnEx4.UnitField = null;
		this.gridColumnEx4.Visible = true;
		this.gridColumnEx4.VisibleIndex = 18;
		this.gridColumnEx5.Caption = "Ordered In Whse";
		this.gridColumnEx5.CurrencyDateField = null;
		this.gridColumnEx5.CurrencyField = null;
		this.gridColumnEx5.CurrencyFixed = null;
		this.gridColumnEx5.CurrencyFixedRate = 0.0;
		this.gridColumnEx5.CurrencyRateField = null;
		this.gridColumnEx5.FieldName = "OnOrderInWhse";
		this.gridColumnEx5.Name = "gridColumnEx5";
		this.gridColumnEx5.PrimaryKey = false;
		this.gridColumnEx5.UnitField = null;
		this.gridColumnEx5.Visible = true;
		this.gridColumnEx5.VisibleIndex = 19;
		this.ucBatchAndSerial1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		this.ucBatchAndSerial1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.ucBatchAndSerial1.FieldBaseObject = "";
		this.ucBatchAndSerial1.FieldItemName = "ItemName";
		this.ucBatchAndSerial1.FieldQuantity = "PlannedQty";
		this.ucBatchAndSerial1.FieldWhsCode = "wareHouse";
		this.ucBatchAndSerial1.FormId = "oProductionOrder";
		this.ucBatchAndSerial1.FormObjectType = Apzon.Commons.ApzObjectTypes.oProductionOrders;
		this.ucBatchAndSerial1.GvHeader = null;
		this.ucBatchAndSerial1.GvLineItem = this.gvDoc;
		this.ucBatchAndSerial1.IsAllowcateBinLocationToWhs = false;
		this.ucBatchAndSerial1.ItemId = "OBTN";
		this.ucBatchAndSerial1.ItemIdBin = "oBinAllowcation";
		this.ucBatchAndSerial1.Location = new System.Drawing.Point(0, 0);
		this.ucBatchAndSerial1.Name = "ucBatchAndSerial1";
		this.ucBatchAndSerial1.Size = new System.Drawing.Size(737, 129);
		this.ucBatchAndSerial1.TabIndex = 0;
		this.ucBatchAndSerial1.TransType = Apzon.Commons.Enums.Enums.BatchSerialTransType.OutAlocate;
		this.tab_Summary.Appearance.PageClient.BackColor = System.Drawing.SystemColors.Control;
		this.tab_Summary.Appearance.PageClient.Options.UseBackColor = true;
		this.tab_Summary.Controls.Add(this.calcTotalVariance);
		this.tab_Summary.Controls.Add(this.calcAtlProductCost);
		this.tab_Summary.Controls.Add(this.calcAtlItemCost);
		this.tab_Summary.Controls.Add(this.txtJournalRemark);
		this.tab_Summary.Controls.Add(this.txtOverDue);
		this.tab_Summary.Controls.Add(this.txtDueDate);
		this.tab_Summary.Controls.Add(this.txtAtlClosingDate);
		this.tab_Summary.Controls.Add(this.calcRejectedQuantityInfo);
		this.tab_Summary.Controls.Add(this.calcCompletedQuantityInfo);
		this.tab_Summary.Controls.Add(this.calcPlannedQuantityInfo);
		this.tab_Summary.Controls.Add(this.calcAtlByProductCost);
		this.tab_Summary.Controls.Add(this.calcAtlAdditionalCost);
		this.tab_Summary.Controls.Add(this.calcAtlResourceCost);
		this.tab_Summary.Controls.Add(this.txtTotalTime);
		this.tab_Summary.Controls.Add(this.txtTotalAdditionalTime);
		this.tab_Summary.Controls.Add(this.txtTotalRunTime);
		this.tab_Summary.Controls.Add(this.labelControl4);
		this.tab_Summary.Controls.Add(this.labelControl3);
		this.tab_Summary.Controls.Add(this.labelControl2);
		this.tab_Summary.Controls.Add(this.labelControl1);
		this.tab_Summary.Controls.Add(this.labelLine38);
		this.tab_Summary.Controls.Add(this.lb_OverDue);
		this.tab_Summary.Controls.Add(this.lb_ActualClosingDate);
		this.tab_Summary.Controls.Add(this.lb_DueDate1);
		this.tab_Summary.Controls.Add(this.lb_TotalTime);
		this.tab_Summary.Controls.Add(this.lb_TotalAdditionalTime);
		this.tab_Summary.Controls.Add(this.lb_TotalRunTime);
		this.tab_Summary.Controls.Add(this.lbRejectedQuantity);
		this.tab_Summary.Controls.Add(this.lb_CompletedQuantity);
		this.tab_Summary.Controls.Add(this.labelLine30);
		this.tab_Summary.Controls.Add(this.lb_TotalVariance);
		this.tab_Summary.Controls.Add(this.lb_ActualByProductCost);
		this.tab_Summary.Controls.Add(this.lblapc);
		this.tab_Summary.Controls.Add(this.lb_AtlAdditionalCost);
		this.tab_Summary.Controls.Add(this.lb_ActualResourceComponentCost);
		this.tab_Summary.Controls.Add(this.lb_ActualItemComponentCost);
		this.tab_Summary.EditControl = null;
		this.tab_Summary.EditValue = "Summary";
		this.tab_Summary.FieldName = "tab_Summary";
		this.tab_Summary.IsSearchControl = false;
		this.tab_Summary.Name = "tab_Summary";
		this.tab_Summary.ReadOnly = false;
		this.tab_Summary.SettingInVisible = false;
		this.tab_Summary.SettingReadonly = false;
		this.tab_Summary.Size = new System.Drawing.Size(737, 309);
		this.tab_Summary.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Summary.Text = "Summary";
		this.calcTotalVariance.CheckExChangeRate = false;
		this.calcTotalVariance.Currency = null;
		this.calcTotalVariance.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.calcTotalVariance.EditControl = null;
		this.calcTotalVariance.Enabled = false;
		this.calcTotalVariance.FieldName = "TotalVariable";
		this.calcTotalVariance.IsSearchControl = false;
		this.calcTotalVariance.Location = new System.Drawing.Point(176, 134);
		this.calcTotalVariance.Name = "calcTotalVariance";
		this.calcTotalVariance.Properties.Appearance.BackColor = System.Drawing.Color.Gainsboro;
		this.calcTotalVariance.Properties.Appearance.Options.UseBackColor = true;
		this.calcTotalVariance.Properties.Appearance.Options.UseTextOptions = true;
		this.calcTotalVariance.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.calcTotalVariance.SettingInVisible = false;
		this.calcTotalVariance.SettingReadonly = false;
		this.calcTotalVariance.Size = new System.Drawing.Size(110, 20);
		this.calcTotalVariance.TabIndex = 90;
		this.calcAtlProductCost.CheckExChangeRate = false;
		this.calcAtlProductCost.Currency = null;
		this.calcAtlProductCost.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.calcAtlProductCost.EditControl = null;
		this.calcAtlProductCost.Enabled = false;
		this.calcAtlProductCost.FieldName = "ActualProCost";
		this.calcAtlProductCost.IsSearchControl = false;
		this.calcAtlProductCost.Location = new System.Drawing.Point(176, 92);
		this.calcAtlProductCost.Name = "calcAtlProductCost";
		this.calcAtlProductCost.Properties.Appearance.BackColor = System.Drawing.Color.Gainsboro;
		this.calcAtlProductCost.Properties.Appearance.Options.UseBackColor = true;
		this.calcAtlProductCost.Properties.Appearance.Options.UseTextOptions = true;
		this.calcAtlProductCost.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.calcAtlProductCost.SettingInVisible = false;
		this.calcAtlProductCost.SettingReadonly = false;
		this.calcAtlProductCost.Size = new System.Drawing.Size(110, 20);
		this.calcAtlProductCost.TabIndex = 89;
		this.calcAtlItemCost.CheckExChangeRate = false;
		this.calcAtlItemCost.Currency = null;
		this.calcAtlItemCost.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.calcAtlItemCost.EditControl = null;
		this.calcAtlItemCost.Enabled = false;
		this.calcAtlItemCost.FieldName = "ActualCompCost";
		this.calcAtlItemCost.IsSearchControl = false;
		this.calcAtlItemCost.Location = new System.Drawing.Point(176, 29);
		this.calcAtlItemCost.Name = "calcAtlItemCost";
		this.calcAtlItemCost.Properties.Appearance.BackColor = System.Drawing.Color.Gainsboro;
		this.calcAtlItemCost.Properties.Appearance.Options.UseBackColor = true;
		this.calcAtlItemCost.Properties.Appearance.Options.UseTextOptions = true;
		this.calcAtlItemCost.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.calcAtlItemCost.SettingInVisible = false;
		this.calcAtlItemCost.SettingReadonly = false;
		this.calcAtlItemCost.Size = new System.Drawing.Size(110, 20);
		this.calcAtlItemCost.TabIndex = 88;
		this.txtJournalRemark.EditControl = null;
		this.txtJournalRemark.FieldName = "JrnlMemo";
		this.txtJournalRemark.IsSearchControl = false;
		this.txtJournalRemark.Location = new System.Drawing.Point(127, 168);
		this.txtJournalRemark.Name = "txtJournalRemark";
		this.txtJournalRemark.SettingInVisible = false;
		this.txtJournalRemark.SettingReadonly = false;
		this.txtJournalRemark.Size = new System.Drawing.Size(159, 20);
		this.txtJournalRemark.TabIndex = 70;
		this.txtOverDue.EditControl = null;
		this.txtOverDue.Enabled = false;
		this.txtOverDue.FieldName = null;
		this.txtOverDue.IsSearchControl = false;
		this.txtOverDue.Location = new System.Drawing.Point(433, 164);
		this.txtOverDue.Name = "txtOverDue";
		this.txtOverDue.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(219, 219, 219);
		this.txtOverDue.Properties.Appearance.Options.UseBackColor = true;
		this.txtOverDue.Properties.ReadOnly = true;
		this.txtOverDue.SettingInVisible = false;
		this.txtOverDue.SettingReadonly = false;
		this.txtOverDue.Size = new System.Drawing.Size(111, 20);
		this.txtOverDue.TabIndex = 68;
		this.txtDueDate.EditControl = null;
		this.txtDueDate.EditValue = null;
		this.txtDueDate.Enabled = false;
		this.txtDueDate.FieldName = "DueDate";
		this.txtDueDate.Location = new System.Drawing.Point(433, 121);
		this.txtDueDate.Name = "txtDueDate";
		this.txtDueDate.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(219, 219, 219);
		this.txtDueDate.Properties.Appearance.Options.UseBackColor = true;
		this.txtDueDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtDueDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtDueDate.Properties.IsSearchControl = false;
		this.txtDueDate.Properties.ReadOnly = true;
		this.txtDueDate.SettingInVisible = false;
		this.txtDueDate.SettingReadonly = false;
		this.txtDueDate.Size = new System.Drawing.Size(111, 20);
		this.txtDueDate.TabIndex = 64;
		this.txtAtlClosingDate.EditControl = null;
		this.txtAtlClosingDate.EditValue = null;
		this.txtAtlClosingDate.Enabled = false;
		this.txtAtlClosingDate.FieldName = "CloseDate";
		this.txtAtlClosingDate.Location = new System.Drawing.Point(433, 142);
		this.txtAtlClosingDate.Name = "txtAtlClosingDate";
		this.txtAtlClosingDate.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(219, 219, 219);
		this.txtAtlClosingDate.Properties.Appearance.Options.UseBackColor = true;
		this.txtAtlClosingDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtAtlClosingDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtAtlClosingDate.Properties.IsSearchControl = false;
		this.txtAtlClosingDate.Properties.ReadOnly = true;
		this.txtAtlClosingDate.SettingInVisible = false;
		this.txtAtlClosingDate.SettingReadonly = false;
		this.txtAtlClosingDate.Size = new System.Drawing.Size(111, 20);
		this.txtAtlClosingDate.TabIndex = 66;
		this.calcRejectedQuantityInfo.EditControl = null;
		this.calcRejectedQuantityInfo.Enabled = false;
		this.calcRejectedQuantityInfo.FieldName = "RjctQty";
		this.calcRejectedQuantityInfo.IsSearchControl = false;
		this.calcRejectedQuantityInfo.Location = new System.Drawing.Point(433, 72);
		this.calcRejectedQuantityInfo.Name = "calcRejectedQuantityInfo";
		this.calcRejectedQuantityInfo.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(219, 219, 219);
		this.calcRejectedQuantityInfo.Properties.Appearance.Options.UseBackColor = true;
		this.calcRejectedQuantityInfo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.calcRejectedQuantityInfo.Properties.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.calcRejectedQuantityInfo.Properties.IsSearchControl = false;
		this.calcRejectedQuantityInfo.Properties.ReadOnly = true;
		this.calcRejectedQuantityInfo.SettingInVisible = false;
		this.calcRejectedQuantityInfo.SettingReadonly = false;
		this.calcRejectedQuantityInfo.Size = new System.Drawing.Size(111, 20);
		this.calcRejectedQuantityInfo.TabIndex = 87;
		this.calcCompletedQuantityInfo.EditControl = null;
		this.calcCompletedQuantityInfo.Enabled = false;
		this.calcCompletedQuantityInfo.FieldName = "CmpltQty";
		this.calcCompletedQuantityInfo.IsSearchControl = false;
		this.calcCompletedQuantityInfo.Location = new System.Drawing.Point(433, 51);
		this.calcCompletedQuantityInfo.Name = "calcCompletedQuantityInfo";
		this.calcCompletedQuantityInfo.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(219, 219, 219);
		this.calcCompletedQuantityInfo.Properties.Appearance.Options.UseBackColor = true;
		this.calcCompletedQuantityInfo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.calcCompletedQuantityInfo.Properties.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.calcCompletedQuantityInfo.Properties.IsSearchControl = false;
		this.calcCompletedQuantityInfo.Properties.ReadOnly = true;
		this.calcCompletedQuantityInfo.SettingInVisible = false;
		this.calcCompletedQuantityInfo.SettingReadonly = false;
		this.calcCompletedQuantityInfo.Size = new System.Drawing.Size(111, 20);
		this.calcCompletedQuantityInfo.TabIndex = 86;
		this.calcPlannedQuantityInfo.EditControl = null;
		this.calcPlannedQuantityInfo.Enabled = false;
		this.calcPlannedQuantityInfo.FieldName = "PlannedQty";
		this.calcPlannedQuantityInfo.IsSearchControl = false;
		this.calcPlannedQuantityInfo.Location = new System.Drawing.Point(433, 30);
		this.calcPlannedQuantityInfo.Name = "calcPlannedQuantityInfo";
		this.calcPlannedQuantityInfo.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(219, 219, 219);
		this.calcPlannedQuantityInfo.Properties.Appearance.Options.UseBackColor = true;
		this.calcPlannedQuantityInfo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.calcPlannedQuantityInfo.Properties.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.calcPlannedQuantityInfo.Properties.IsSearchControl = false;
		this.calcPlannedQuantityInfo.Properties.ReadOnly = true;
		this.calcPlannedQuantityInfo.SettingInVisible = false;
		this.calcPlannedQuantityInfo.SettingReadonly = false;
		this.calcPlannedQuantityInfo.Size = new System.Drawing.Size(111, 20);
		this.calcPlannedQuantityInfo.TabIndex = 85;
		this.calcAtlByProductCost.EditControl = null;
		this.calcAtlByProductCost.Enabled = false;
		this.calcAtlByProductCost.FieldName = null;
		this.calcAtlByProductCost.IsSearchControl = false;
		this.calcAtlByProductCost.Location = new System.Drawing.Point(176, 113);
		this.calcAtlByProductCost.Name = "calcAtlByProductCost";
		this.calcAtlByProductCost.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(219, 219, 219);
		this.calcAtlByProductCost.Properties.Appearance.Options.UseBackColor = true;
		this.calcAtlByProductCost.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.calcAtlByProductCost.Properties.IsSearchControl = false;
		this.calcAtlByProductCost.Properties.ReadOnly = true;
		this.calcAtlByProductCost.SettingInVisible = false;
		this.calcAtlByProductCost.SettingReadonly = false;
		this.calcAtlByProductCost.Size = new System.Drawing.Size(110, 20);
		this.calcAtlByProductCost.TabIndex = 79;
		this.calcAtlAdditionalCost.EditControl = null;
		this.calcAtlAdditionalCost.Enabled = false;
		this.calcAtlAdditionalCost.FieldName = "ActualAddCost";
		this.calcAtlAdditionalCost.IsSearchControl = false;
		this.calcAtlAdditionalCost.Location = new System.Drawing.Point(176, 71);
		this.calcAtlAdditionalCost.Name = "calcAtlAdditionalCost";
		this.calcAtlAdditionalCost.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(219, 219, 219);
		this.calcAtlAdditionalCost.Properties.Appearance.Options.UseBackColor = true;
		this.calcAtlAdditionalCost.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.calcAtlAdditionalCost.Properties.IsSearchControl = false;
		this.calcAtlAdditionalCost.Properties.ReadOnly = true;
		this.calcAtlAdditionalCost.SettingInVisible = false;
		this.calcAtlAdditionalCost.SettingReadonly = false;
		this.calcAtlAdditionalCost.Size = new System.Drawing.Size(110, 20);
		this.calcAtlAdditionalCost.TabIndex = 77;
		this.calcAtlResourceCost.EditControl = null;
		this.calcAtlResourceCost.Enabled = false;
		this.calcAtlResourceCost.FieldName = null;
		this.calcAtlResourceCost.IsSearchControl = false;
		this.calcAtlResourceCost.Location = new System.Drawing.Point(176, 50);
		this.calcAtlResourceCost.Name = "calcAtlResourceCost";
		this.calcAtlResourceCost.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(219, 219, 219);
		this.calcAtlResourceCost.Properties.Appearance.Options.UseBackColor = true;
		this.calcAtlResourceCost.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.calcAtlResourceCost.Properties.IsSearchControl = false;
		this.calcAtlResourceCost.Properties.ReadOnly = true;
		this.calcAtlResourceCost.SettingInVisible = false;
		this.calcAtlResourceCost.SettingReadonly = false;
		this.calcAtlResourceCost.Size = new System.Drawing.Size(110, 20);
		this.calcAtlResourceCost.TabIndex = 76;
		this.txtTotalTime.EditControl = null;
		this.txtTotalTime.Enabled = false;
		this.txtTotalTime.FieldName = null;
		this.txtTotalTime.IsSearchControl = false;
		this.txtTotalTime.Location = new System.Drawing.Point(691, 72);
		this.txtTotalTime.Name = "txtTotalTime";
		this.txtTotalTime.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(219, 219, 219);
		this.txtTotalTime.Properties.Appearance.Options.UseBackColor = true;
		this.txtTotalTime.Properties.ReadOnly = true;
		this.txtTotalTime.SettingInVisible = false;
		this.txtTotalTime.SettingReadonly = false;
		this.txtTotalTime.Size = new System.Drawing.Size(111, 20);
		this.txtTotalTime.TabIndex = 61;
		this.txtTotalAdditionalTime.EditControl = null;
		this.txtTotalAdditionalTime.Enabled = false;
		this.txtTotalAdditionalTime.FieldName = null;
		this.txtTotalAdditionalTime.IsSearchControl = false;
		this.txtTotalAdditionalTime.Location = new System.Drawing.Point(691, 51);
		this.txtTotalAdditionalTime.Name = "txtTotalAdditionalTime";
		this.txtTotalAdditionalTime.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(219, 219, 219);
		this.txtTotalAdditionalTime.Properties.Appearance.Options.UseBackColor = true;
		this.txtTotalAdditionalTime.Properties.ReadOnly = true;
		this.txtTotalAdditionalTime.SettingInVisible = false;
		this.txtTotalAdditionalTime.SettingReadonly = false;
		this.txtTotalAdditionalTime.Size = new System.Drawing.Size(111, 20);
		this.txtTotalAdditionalTime.TabIndex = 59;
		this.txtTotalRunTime.EditControl = null;
		this.txtTotalRunTime.Enabled = false;
		this.txtTotalRunTime.FieldName = null;
		this.txtTotalRunTime.IsSearchControl = false;
		this.txtTotalRunTime.Location = new System.Drawing.Point(691, 30);
		this.txtTotalRunTime.Name = "txtTotalRunTime";
		this.txtTotalRunTime.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(219, 219, 219);
		this.txtTotalRunTime.Properties.Appearance.Options.UseBackColor = true;
		this.txtTotalRunTime.Properties.ReadOnly = true;
		this.txtTotalRunTime.SettingInVisible = false;
		this.txtTotalRunTime.SettingReadonly = false;
		this.txtTotalRunTime.Size = new System.Drawing.Size(111, 20);
		this.txtTotalRunTime.TabIndex = 57;
		this.labelControl4.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline);
		this.labelControl4.Appearance.Options.UseFont = true;
		this.labelControl4.Location = new System.Drawing.Point(306, 103);
		this.labelControl4.Name = "labelControl4";
		this.labelControl4.Size = new System.Drawing.Size(33, 13);
		this.labelControl4.TabIndex = 84;
		this.labelControl4.Text = "Dates";
		this.labelControl3.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline);
		this.labelControl3.Appearance.Options.UseFont = true;
		this.labelControl3.Location = new System.Drawing.Point(557, 8);
		this.labelControl3.Name = "labelControl3";
		this.labelControl3.Size = new System.Drawing.Size(82, 13);
		this.labelControl3.TabIndex = 83;
		this.labelControl3.Text = "Planned Times";
		this.labelControl2.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline);
		this.labelControl2.Appearance.Options.UseFont = true;
		this.labelControl2.Location = new System.Drawing.Point(304, 8);
		this.labelControl2.Name = "labelControl2";
		this.labelControl2.Size = new System.Drawing.Size(58, 13);
		this.labelControl2.TabIndex = 82;
		this.labelControl2.Text = "Quantities";
		this.labelControl1.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline);
		this.labelControl1.Appearance.Options.UseFont = true;
		this.labelControl1.Location = new System.Drawing.Point(9, 8);
		this.labelControl1.Name = "labelControl1";
		this.labelControl1.Size = new System.Drawing.Size(31, 13);
		this.labelControl1.TabIndex = 81;
		this.labelControl1.Text = "Costs";
		this.labelLine38.EditControl = this.txtJournalRemark;
		this.labelLine38.EditValue = "Journal Remark";
		this.labelLine38.FieldName = null;
		this.labelLine38.HideWhenEmpty = false;
		this.labelLine38.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine38.Location = new System.Drawing.Point(9, 171);
		this.labelLine38.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine38.Name = "labelLine38";
		this.labelLine38.ReadOnly = false;
		this.labelLine38.SettingInVisible = false;
		this.labelLine38.SettingReadonly = false;
		this.labelLine38.ShowUnderLine = true;
		this.labelLine38.Size = new System.Drawing.Size(119, 20);
		this.labelLine38.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine38.TabIndex = 71;
		this.labelLine38.Utext = "Journal Remark";
		this.lb_OverDue.EditControl = this.txtOverDue;
		this.lb_OverDue.EditValue = "Over Due";
		this.lb_OverDue.FieldName = null;
		this.lb_OverDue.HideWhenEmpty = false;
		this.lb_OverDue.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_OverDue.Location = new System.Drawing.Point(304, 167);
		this.lb_OverDue.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_OverDue.Name = "lb_OverDue";
		this.lb_OverDue.ReadOnly = true;
		this.lb_OverDue.SettingInVisible = false;
		this.lb_OverDue.SettingReadonly = false;
		this.lb_OverDue.ShowUnderLine = true;
		this.lb_OverDue.Size = new System.Drawing.Size(130, 20);
		this.lb_OverDue.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_OverDue.TabIndex = 69;
		this.lb_OverDue.Utext = "Over Due";
		this.lb_ActualClosingDate.EditControl = this.txtAtlClosingDate;
		this.lb_ActualClosingDate.EditValue = "Actual Closing Date";
		this.lb_ActualClosingDate.FieldName = null;
		this.lb_ActualClosingDate.HideWhenEmpty = false;
		this.lb_ActualClosingDate.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_ActualClosingDate.Location = new System.Drawing.Point(304, 145);
		this.lb_ActualClosingDate.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_ActualClosingDate.Name = "lb_ActualClosingDate";
		this.lb_ActualClosingDate.ReadOnly = true;
		this.lb_ActualClosingDate.SettingInVisible = false;
		this.lb_ActualClosingDate.SettingReadonly = false;
		this.lb_ActualClosingDate.ShowUnderLine = true;
		this.lb_ActualClosingDate.Size = new System.Drawing.Size(130, 20);
		this.lb_ActualClosingDate.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_ActualClosingDate.TabIndex = 67;
		this.lb_ActualClosingDate.Utext = "Actual Closing Date";
		this.lb_DueDate1.EditControl = this.txtDueDate;
		this.lb_DueDate1.EditValue = "Due Date";
		this.lb_DueDate1.FieldName = null;
		this.lb_DueDate1.HideWhenEmpty = false;
		this.lb_DueDate1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_DueDate1.Location = new System.Drawing.Point(304, 124);
		this.lb_DueDate1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_DueDate1.Name = "lb_DueDate1";
		this.lb_DueDate1.ReadOnly = true;
		this.lb_DueDate1.SettingInVisible = false;
		this.lb_DueDate1.SettingReadonly = false;
		this.lb_DueDate1.ShowUnderLine = true;
		this.lb_DueDate1.Size = new System.Drawing.Size(130, 20);
		this.lb_DueDate1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_DueDate1.TabIndex = 65;
		this.lb_DueDate1.Utext = "Due Date";
		this.lb_TotalTime.EditControl = this.txtTotalTime;
		this.lb_TotalTime.EditValue = "Total Time";
		this.lb_TotalTime.FieldName = null;
		this.lb_TotalTime.HideWhenEmpty = false;
		this.lb_TotalTime.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_TotalTime.Location = new System.Drawing.Point(557, 75);
		this.lb_TotalTime.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_TotalTime.Name = "lb_TotalTime";
		this.lb_TotalTime.ReadOnly = true;
		this.lb_TotalTime.SettingInVisible = false;
		this.lb_TotalTime.SettingReadonly = false;
		this.lb_TotalTime.ShowUnderLine = true;
		this.lb_TotalTime.Size = new System.Drawing.Size(135, 20);
		this.lb_TotalTime.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_TotalTime.TabIndex = 62;
		this.lb_TotalTime.Utext = "Total Time";
		this.lb_TotalAdditionalTime.EditControl = this.txtTotalAdditionalTime;
		this.lb_TotalAdditionalTime.EditValue = "Total Additional Time";
		this.lb_TotalAdditionalTime.FieldName = null;
		this.lb_TotalAdditionalTime.HideWhenEmpty = false;
		this.lb_TotalAdditionalTime.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_TotalAdditionalTime.Location = new System.Drawing.Point(557, 54);
		this.lb_TotalAdditionalTime.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_TotalAdditionalTime.Name = "lb_TotalAdditionalTime";
		this.lb_TotalAdditionalTime.ReadOnly = true;
		this.lb_TotalAdditionalTime.SettingInVisible = false;
		this.lb_TotalAdditionalTime.SettingReadonly = false;
		this.lb_TotalAdditionalTime.ShowUnderLine = true;
		this.lb_TotalAdditionalTime.Size = new System.Drawing.Size(135, 20);
		this.lb_TotalAdditionalTime.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_TotalAdditionalTime.TabIndex = 60;
		this.lb_TotalAdditionalTime.Utext = "Total Additional Time";
		this.lb_TotalRunTime.EditControl = this.txtTotalRunTime;
		this.lb_TotalRunTime.EditValue = "Total Run Time";
		this.lb_TotalRunTime.FieldName = null;
		this.lb_TotalRunTime.HideWhenEmpty = false;
		this.lb_TotalRunTime.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_TotalRunTime.Location = new System.Drawing.Point(557, 33);
		this.lb_TotalRunTime.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_TotalRunTime.Name = "lb_TotalRunTime";
		this.lb_TotalRunTime.ReadOnly = true;
		this.lb_TotalRunTime.SettingInVisible = false;
		this.lb_TotalRunTime.SettingReadonly = false;
		this.lb_TotalRunTime.ShowUnderLine = true;
		this.lb_TotalRunTime.Size = new System.Drawing.Size(135, 20);
		this.lb_TotalRunTime.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_TotalRunTime.TabIndex = 58;
		this.lb_TotalRunTime.Utext = "Total Run Time";
		this.lbRejectedQuantity.EditControl = this.calcRejectedQuantityInfo;
		this.lbRejectedQuantity.EditValue = "Rejected Quantity";
		this.lbRejectedQuantity.FieldName = null;
		this.lbRejectedQuantity.HideWhenEmpty = false;
		this.lbRejectedQuantity.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbRejectedQuantity.Location = new System.Drawing.Point(304, 75);
		this.lbRejectedQuantity.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lbRejectedQuantity.Name = "lbRejectedQuantity";
		this.lbRejectedQuantity.ReadOnly = true;
		this.lbRejectedQuantity.SettingInVisible = false;
		this.lbRejectedQuantity.SettingReadonly = false;
		this.lbRejectedQuantity.ShowUnderLine = true;
		this.lbRejectedQuantity.Size = new System.Drawing.Size(130, 20);
		this.lbRejectedQuantity.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbRejectedQuantity.TabIndex = 56;
		this.lbRejectedQuantity.Utext = "Rejected Quantity";
		this.lb_CompletedQuantity.EditControl = this.calcCompletedQuantityInfo;
		this.lb_CompletedQuantity.EditValue = "Completed Quantity";
		this.lb_CompletedQuantity.FieldName = null;
		this.lb_CompletedQuantity.HideWhenEmpty = false;
		this.lb_CompletedQuantity.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_CompletedQuantity.Location = new System.Drawing.Point(304, 54);
		this.lb_CompletedQuantity.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_CompletedQuantity.Name = "lb_CompletedQuantity";
		this.lb_CompletedQuantity.ReadOnly = true;
		this.lb_CompletedQuantity.SettingInVisible = false;
		this.lb_CompletedQuantity.SettingReadonly = false;
		this.lb_CompletedQuantity.ShowUnderLine = true;
		this.lb_CompletedQuantity.Size = new System.Drawing.Size(130, 20);
		this.lb_CompletedQuantity.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_CompletedQuantity.TabIndex = 54;
		this.lb_CompletedQuantity.Utext = "Completed Quantity";
		this.labelLine30.EditControl = this.calcPlannedQuantityInfo;
		this.labelLine30.EditValue = "Planned Quantity";
		this.labelLine30.FieldName = null;
		this.labelLine30.HideWhenEmpty = false;
		this.labelLine30.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine30.Location = new System.Drawing.Point(304, 33);
		this.labelLine30.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine30.Name = "labelLine30";
		this.labelLine30.ReadOnly = true;
		this.labelLine30.SettingInVisible = false;
		this.labelLine30.SettingReadonly = false;
		this.labelLine30.ShowUnderLine = true;
		this.labelLine30.Size = new System.Drawing.Size(130, 20);
		this.labelLine30.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine30.TabIndex = 52;
		this.labelLine30.Utext = "Planned Quantity";
		this.lb_TotalVariance.EditControl = this.calcTotalVariance;
		this.lb_TotalVariance.EditValue = "Total Variance";
		this.lb_TotalVariance.FieldName = null;
		this.lb_TotalVariance.HideWhenEmpty = false;
		this.lb_TotalVariance.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_TotalVariance.Location = new System.Drawing.Point(9, 137);
		this.lb_TotalVariance.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_TotalVariance.Name = "lb_TotalVariance";
		this.lb_TotalVariance.ReadOnly = false;
		this.lb_TotalVariance.SettingInVisible = false;
		this.lb_TotalVariance.SettingReadonly = false;
		this.lb_TotalVariance.ShowUnderLine = true;
		this.lb_TotalVariance.Size = new System.Drawing.Size(168, 20);
		this.lb_TotalVariance.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_TotalVariance.TabIndex = 50;
		this.lb_TotalVariance.Utext = "Total Variance";
		this.lb_ActualByProductCost.EditControl = this.calcAtlByProductCost;
		this.lb_ActualByProductCost.EditValue = "Actual By-Product Cost";
		this.lb_ActualByProductCost.FieldName = null;
		this.lb_ActualByProductCost.HideWhenEmpty = false;
		this.lb_ActualByProductCost.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_ActualByProductCost.Location = new System.Drawing.Point(9, 116);
		this.lb_ActualByProductCost.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_ActualByProductCost.Name = "lb_ActualByProductCost";
		this.lb_ActualByProductCost.ReadOnly = true;
		this.lb_ActualByProductCost.SettingInVisible = false;
		this.lb_ActualByProductCost.SettingReadonly = false;
		this.lb_ActualByProductCost.ShowUnderLine = true;
		this.lb_ActualByProductCost.Size = new System.Drawing.Size(168, 20);
		this.lb_ActualByProductCost.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_ActualByProductCost.TabIndex = 48;
		this.lb_ActualByProductCost.Utext = "Actual By-Product Cost";
		this.lblapc.EditControl = this.calcAtlProductCost;
		this.lblapc.EditValue = "Actual Product Cost";
		this.lblapc.FieldName = null;
		this.lblapc.HideWhenEmpty = false;
		this.lblapc.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lblapc.Location = new System.Drawing.Point(9, 95);
		this.lblapc.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lblapc.Name = "lblapc";
		this.lblapc.ReadOnly = false;
		this.lblapc.SettingInVisible = false;
		this.lblapc.SettingReadonly = false;
		this.lblapc.ShowUnderLine = true;
		this.lblapc.Size = new System.Drawing.Size(168, 20);
		this.lblapc.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lblapc.TabIndex = 46;
		this.lblapc.Utext = "Actual Product Cost";
		this.lb_AtlAdditionalCost.EditControl = this.calcAtlAdditionalCost;
		this.lb_AtlAdditionalCost.EditValue = "Actual Additional Cost";
		this.lb_AtlAdditionalCost.FieldName = null;
		this.lb_AtlAdditionalCost.HideWhenEmpty = false;
		this.lb_AtlAdditionalCost.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_AtlAdditionalCost.Location = new System.Drawing.Point(9, 74);
		this.lb_AtlAdditionalCost.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_AtlAdditionalCost.Name = "lb_AtlAdditionalCost";
		this.lb_AtlAdditionalCost.ReadOnly = true;
		this.lb_AtlAdditionalCost.SettingInVisible = false;
		this.lb_AtlAdditionalCost.SettingReadonly = false;
		this.lb_AtlAdditionalCost.ShowUnderLine = true;
		this.lb_AtlAdditionalCost.Size = new System.Drawing.Size(168, 20);
		this.lb_AtlAdditionalCost.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_AtlAdditionalCost.TabIndex = 44;
		this.lb_AtlAdditionalCost.Utext = "Actual Additional Cost";
		this.lb_ActualResourceComponentCost.EditControl = this.calcAtlResourceCost;
		this.lb_ActualResourceComponentCost.EditValue = "Actual Resource Component Cost";
		this.lb_ActualResourceComponentCost.FieldName = null;
		this.lb_ActualResourceComponentCost.HideWhenEmpty = false;
		this.lb_ActualResourceComponentCost.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_ActualResourceComponentCost.Location = new System.Drawing.Point(9, 53);
		this.lb_ActualResourceComponentCost.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_ActualResourceComponentCost.Name = "lb_ActualResourceComponentCost";
		this.lb_ActualResourceComponentCost.ReadOnly = true;
		this.lb_ActualResourceComponentCost.SettingInVisible = false;
		this.lb_ActualResourceComponentCost.SettingReadonly = false;
		this.lb_ActualResourceComponentCost.ShowUnderLine = true;
		this.lb_ActualResourceComponentCost.Size = new System.Drawing.Size(168, 20);
		this.lb_ActualResourceComponentCost.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_ActualResourceComponentCost.TabIndex = 42;
		this.lb_ActualResourceComponentCost.Utext = "Actual Resource Component Cost";
		this.lb_ActualItemComponentCost.EditControl = this.calcAtlItemCost;
		this.lb_ActualItemComponentCost.EditValue = "Actual Item Component Cost";
		this.lb_ActualItemComponentCost.FieldName = null;
		this.lb_ActualItemComponentCost.HideWhenEmpty = false;
		this.lb_ActualItemComponentCost.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_ActualItemComponentCost.Location = new System.Drawing.Point(9, 31);
		this.lb_ActualItemComponentCost.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_ActualItemComponentCost.Name = "lb_ActualItemComponentCost";
		this.lb_ActualItemComponentCost.ReadOnly = false;
		this.lb_ActualItemComponentCost.SettingInVisible = false;
		this.lb_ActualItemComponentCost.SettingReadonly = false;
		this.lb_ActualItemComponentCost.ShowUnderLine = true;
		this.lb_ActualItemComponentCost.Size = new System.Drawing.Size(168, 20);
		this.lb_ActualItemComponentCost.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_ActualItemComponentCost.TabIndex = 40;
		this.lb_ActualItemComponentCost.Utext = "Actual Item Component Cost";
		this.tab_Attachments.Controls.Add(this.ucAttachment1);
		this.tab_Attachments.EditControl = null;
		this.tab_Attachments.EditValue = "Attachments";
		this.tab_Attachments.FieldName = null;
		this.tab_Attachments.IsSearchControl = false;
		this.tab_Attachments.Name = "tab_Attachments";
		this.tab_Attachments.ReadOnly = false;
		this.tab_Attachments.SettingInVisible = false;
		this.tab_Attachments.SettingReadonly = false;
		this.tab_Attachments.Size = new System.Drawing.Size(737, 309);
		this.tab_Attachments.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Attachments.Text = "Attachments";
		this.ucAttachment1.DataSource = null;
		this.ucAttachment1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.ucAttachment1.FormId = "oProductionOrder";
		this.ucAttachment1.ItemId = "ATC1";
		this.ucAttachment1.Location = new System.Drawing.Point(0, 0);
		this.ucAttachment1.Name = "ucAttachment1";
		this.ucAttachment1.Size = new System.Drawing.Size(737, 309);
		this.ucAttachment1.TabIndex = 0;
		this.ucAttachment1.ButtonDelete_Click += new System.EventHandler(this.ucAttachment1_ButtonDelete_Click);
		this.tab_UDF.Controls.Add(this.grd_UDF);
		this.tab_UDF.EditControl = null;
		this.tab_UDF.EditValue = "User Defined Fields";
		this.tab_UDF.FieldName = "tab_UDF";
		this.tab_UDF.IsSearchControl = false;
		this.tab_UDF.Name = "tab_UDF";
		this.tab_UDF.ReadOnly = false;
		this.tab_UDF.SettingInVisible = false;
		this.tab_UDF.SettingReadonly = false;
		this.tab_UDF.Size = new System.Drawing.Size(737, 309);
		this.tab_UDF.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_UDF.Text = "User Defined Fields";
		this.grd_UDF.AllowPasteExcel = false;
		this.grd_UDF.ClipboardData = null;
		this.grd_UDF.ContextMenuStrip = this.contextMenuStrip_Line;
		this.grd_UDF.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_UDF.Location = new System.Drawing.Point(0, 0);
		this.grd_UDF.MainView = this.gv_UDF;
		this.grd_UDF.Name = "grd_UDF";
		this.grd_UDF.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[8] { this.repositoryItemComboBox3, this.repositoryItemButtonEdit2, this.repositoryItemCalcEdit2, this.repositoryItemButtonEdit3, this.repositoryItemComboBox2, this.repositoryItemButtonEdit4, this.repositoryItemComboBox1, this.repositoryItemButtonEdit5 });
		this.grd_UDF.Size = new System.Drawing.Size(737, 309);
		this.grd_UDF.TabIndex = 18;
		this.grd_UDF.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_UDF });
		this.gv_UDF.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_UDF.ColumnKey = null;
		this.gv_UDF.DisableFormSetting = false;
		this.gv_UDF.DisplayException = false;
		this.gv_UDF.FieldCurrency = null;
		this.gv_UDF.FieldRate = null;
		this.gv_UDF.FormId = "oProductionOrder";
		this.gv_UDF.GridControl = this.grd_UDF;
		this.gv_UDF.IndicatorWidth = 35;
		this.gv_UDF.IsApzTable = false;
		this.gv_UDF.IsInsertNewRow = false;
		this.gv_UDF.IsLoadingData = false;
		this.gv_UDF.ItemId = "OUDF";
		this.gv_UDF.LstColumnAllowBatch = null;
		this.gv_UDF.LstColumnNotAllowBatch = null;
		this.gv_UDF.LstColumnNotEditable = null;
		this.gv_UDF.LstColumNotSetting = null;
		this.gv_UDF.Name = "gv_UDF";
		this.gv_UDF.OptionsView.ColumnAutoWidth = false;
		this.gv_UDF.OptionsView.ShowDetailButtons = false;
		this.gv_UDF.OptionsView.ShowFooter = true;
		this.gv_UDF.OptionsView.ShowGroupPanel = false;
		this.gv_UDF.UseBatchMod = false;
		this.repositoryItemComboBox3.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
		this.repositoryItemComboBox3.AutoHeight = false;
		this.repositoryItemComboBox3.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemComboBox3.Name = "repositoryItemComboBox3";
		this.repositoryItemButtonEdit2.AutoHeight = false;
		this.repositoryItemButtonEdit2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.SpinRight, "", -1, true, true, true, imageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance9, appearanceHovered9, appearancePressed9, appearanceDisabled9, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.repositoryItemButtonEdit2.Name = "repositoryItemButtonEdit2";
		this.repositoryItemCalcEdit2.AutoHeight = false;
		this.repositoryItemCalcEdit2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEdit2.Name = "repositoryItemCalcEdit2";
		this.repositoryItemButtonEdit3.AutoHeight = false;
		this.repositoryItemButtonEdit3.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.SpinRight, "", -1, true, true, true, imageOptions5, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance10, appearanceHovered10, appearancePressed10, appearanceDisabled10, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.repositoryItemButtonEdit3.Name = "repositoryItemButtonEdit3";
		this.repositoryItemComboBox2.AutoHeight = false;
		this.repositoryItemComboBox2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemComboBox2.Name = "repositoryItemComboBox2";
		this.repositoryItemButtonEdit4.AutoHeight = false;
		this.repositoryItemButtonEdit4.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.SpinRight, "", -1, true, true, true, imageOptions6, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance11, appearanceHovered11, appearancePressed11, appearanceDisabled11, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.repositoryItemButtonEdit4.Name = "repositoryItemButtonEdit4";
		this.repositoryItemComboBox1.AutoHeight = false;
		this.repositoryItemComboBox1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemComboBox1.Name = "repositoryItemComboBox1";
		this.repositoryItemButtonEdit5.AutoHeight = false;
		this.repositoryItemButtonEdit5.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.repositoryItemButtonEdit5.Name = "repositoryItemButtonEdit5";
		this.memoPickRemarks.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.memoPickRemarks.EditControl = null;
		this.memoPickRemarks.FieldName = "PickRmrk";
		this.memoPickRemarks.IsSearchControl = false;
		this.memoPickRemarks.Location = new System.Drawing.Point(538, 560);
		this.memoPickRemarks.Name = "memoPickRemarks";
		this.memoPickRemarks.SettingInVisible = false;
		this.memoPickRemarks.SettingReadonly = false;
		this.memoPickRemarks.Size = new System.Drawing.Size(282, 78);
		this.memoPickRemarks.TabIndex = 23;
		this.memoRemarks.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.memoRemarks.EditControl = null;
		this.memoRemarks.FieldName = "Comments";
		this.memoRemarks.IsSearchControl = false;
		this.memoRemarks.Location = new System.Drawing.Point(89, 560);
		this.memoRemarks.Name = "memoRemarks";
		this.memoRemarks.SettingInVisible = false;
		this.memoRemarks.SettingReadonly = false;
		this.memoRemarks.Size = new System.Drawing.Size(282, 78);
		this.memoRemarks.TabIndex = 22;
		this.lb_PickAndPackRemarks.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.lb_PickAndPackRemarks.EditControl = this.memoPickRemarks;
		this.lb_PickAndPackRemarks.EditValue = "Pick and Pack Remarks";
		this.lb_PickAndPackRemarks.FieldName = null;
		this.lb_PickAndPackRemarks.HideWhenEmpty = false;
		this.lb_PickAndPackRemarks.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_PickAndPackRemarks.Location = new System.Drawing.Point(409, 557);
		this.lb_PickAndPackRemarks.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_PickAndPackRemarks.Name = "lb_PickAndPackRemarks";
		this.lb_PickAndPackRemarks.ReadOnly = false;
		this.lb_PickAndPackRemarks.SettingInVisible = false;
		this.lb_PickAndPackRemarks.SettingReadonly = false;
		this.lb_PickAndPackRemarks.ShowUnderLine = true;
		this.lb_PickAndPackRemarks.Size = new System.Drawing.Size(129, 20);
		this.lb_PickAndPackRemarks.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_PickAndPackRemarks.TabIndex = 37;
		this.lb_PickAndPackRemarks.Utext = "Pick and Pack Remarks";
		this.lb_Remarks.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.lb_Remarks.EditControl = this.memoRemarks;
		this.lb_Remarks.EditValue = "Remarks";
		this.lb_Remarks.FieldName = null;
		this.lb_Remarks.HideWhenEmpty = false;
		this.lb_Remarks.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Remarks.Location = new System.Drawing.Point(8, 559);
		this.lb_Remarks.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_Remarks.Name = "lb_Remarks";
		this.lb_Remarks.ReadOnly = false;
		this.lb_Remarks.SettingInVisible = false;
		this.lb_Remarks.SettingReadonly = false;
		this.lb_Remarks.ShowUnderLine = true;
		this.lb_Remarks.Size = new System.Drawing.Size(81, 20);
		this.lb_Remarks.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Remarks.TabIndex = 36;
		this.lb_Remarks.Utext = "Remarks";
		this.lb_UoMName.EditControl = this.txtUoMName;
		this.lb_UoMName.EditValue = "UoM Name";
		this.lb_UoMName.FieldName = null;
		this.lb_UoMName.HideWhenEmpty = false;
		this.lb_UoMName.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_UoMName.Location = new System.Drawing.Point(277, 89);
		this.lb_UoMName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_UoMName.Name = "lb_UoMName";
		this.lb_UoMName.ReadOnly = false;
		this.lb_UoMName.SettingInVisible = false;
		this.lb_UoMName.SettingReadonly = false;
		this.lb_UoMName.ShowUnderLine = true;
		this.lb_UoMName.Size = new System.Drawing.Size(55, 20);
		this.lb_UoMName.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_UoMName.TabIndex = 38;
		this.lb_UoMName.Utext = "UoM Name";
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_OK.Appearance.Options.UseBackColor = true;
		this.btn_OK.ButtonKind = Apzon.Commons.Enums.Enums.SimpleButtonType.Add;
		this.btn_OK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_OK.EditControl = null;
		this.btn_OK.EditValue = "Add";
		this.btn_OK.FieldName = null;
		this.btn_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_OK.ImageOptions.Image");
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(7, 644);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.ReadOnly = false;
		this.btn_OK.SettingInVisible = false;
		this.btn_OK.SettingReadonly = false;
		this.btn_OK.Size = new System.Drawing.Size(99, 20);
		this.btn_OK.TabIndex = 24;
		this.btn_OK.Text = "Add";
		this.btn_OK.Click += new System.EventHandler(this.btOK_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Cancel.Appearance.Options.UseBackColor = true;
		this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(112, 644);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(99, 20);
		this.btn_Cancel.TabIndex = 25;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btCancel_Click);
		this.contextMenuStrip_Doc.Items.AddRange(new System.Windows.Forms.ToolStripItem[10] { this.cancelToolStripMenuItem, this.duplicateToolStripMenuItem, this.toolStripSeparator1, this.reportCompletionToolStripMenuItem, this.issueComponentsToolStripMenuItem, this.transferRequestToolStripMenuItem, this.componentTransferToolStripMenuItem, this.toolStripSeparator2, this.viewDiagramToolStripMenuItem, this.relateToolStripMenuItem });
		this.contextMenuStrip_Doc.Name = "contextMenuStrip_Doc";
		this.contextMenuStrip_Doc.Size = new System.Drawing.Size(183, 192);
		this.contextMenuStrip_Doc.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Doc_Opening);
		this.cancelToolStripMenuItem.Name = "cancelToolStripMenuItem";
		this.cancelToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
		this.cancelToolStripMenuItem.Text = "Cancel";
		this.cancelToolStripMenuItem.Click += new System.EventHandler(this.cancelToolStripMenuItem_Click);
		this.duplicateToolStripMenuItem.Name = "duplicateToolStripMenuItem";
		this.duplicateToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
		this.duplicateToolStripMenuItem.Text = "Duplicate";
		this.duplicateToolStripMenuItem.Click += new System.EventHandler(this.duplicateToolStripMenuItem1_Click);
		this.toolStripSeparator1.Name = "toolStripSeparator1";
		this.toolStripSeparator1.Size = new System.Drawing.Size(179, 6);
		this.reportCompletionToolStripMenuItem.Name = "reportCompletionToolStripMenuItem";
		this.reportCompletionToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
		this.reportCompletionToolStripMenuItem.Text = "Report Completion";
		this.reportCompletionToolStripMenuItem.Click += new System.EventHandler(this.reportCompletionToolStripMenuItem_Click);
		this.issueComponentsToolStripMenuItem.Name = "issueComponentsToolStripMenuItem";
		this.issueComponentsToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
		this.issueComponentsToolStripMenuItem.Text = "Issue Components";
		this.issueComponentsToolStripMenuItem.Click += new System.EventHandler(this.issueComponentsToolStripMenuItem_Click);
		this.transferRequestToolStripMenuItem.Name = "transferRequestToolStripMenuItem";
		this.transferRequestToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
		this.transferRequestToolStripMenuItem.Text = "Transfer Request";
		this.transferRequestToolStripMenuItem.Click += new System.EventHandler(this.transferRequestToolStripMenuItem_Click);
		this.componentTransferToolStripMenuItem.Name = "componentTransferToolStripMenuItem";
		this.componentTransferToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
		this.componentTransferToolStripMenuItem.Text = "Component Transfer";
		this.componentTransferToolStripMenuItem.Click += new System.EventHandler(this.componentTransferToolStripMenuItem_Click);
		this.toolStripSeparator2.Name = "toolStripSeparator2";
		this.toolStripSeparator2.Size = new System.Drawing.Size(179, 6);
		this.viewDiagramToolStripMenuItem.Name = "viewDiagramToolStripMenuItem";
		this.viewDiagramToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
		this.viewDiagramToolStripMenuItem.Text = "View Diagram";
		this.viewDiagramToolStripMenuItem.Click += new System.EventHandler(this.viewDiagramToolStripMenuItem_Click);
		this.relateToolStripMenuItem.Name = "relateToolStripMenuItem";
		this.relateToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
		this.relateToolStripMenuItem.Text = "Related Activities";
		this.relateToolStripMenuItem.Click += new System.EventHandler(this.relateToolStripMenuItem_Click);
		this.look_Series.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.look_Series.EditControl = null;
		this.look_Series.FieldName = "Series";
		this.look_Series.IsSearchControl = false;
		this.look_Series.Location = new System.Drawing.Point(512, 2);
		this.look_Series.Name = "look_Series";
		this.look_Series.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.look_Series.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("SeriesName", "Name")
		});
		this.look_Series.Properties.DisplayMember = "SeriesName";
		this.look_Series.Properties.IsSearchControl = false;
		this.look_Series.Properties.NullText = "";
		this.look_Series.Properties.ValueMember = "Series";
		this.look_Series.SettingInVisible = false;
		this.look_Series.SettingReadonly = false;
		this.look_Series.Size = new System.Drawing.Size(100, 20);
		this.look_Series.TabIndex = 9;
		this.look_Series.EditValueChanged += new System.EventHandler(this.look_Series_EditValueChanged);
		this.lbBranch.EditControl = this.txt_Branch;
		this.lbBranch.EditValue = "Branch";
		this.lbBranch.FieldName = null;
		this.lbBranch.HideWhenEmpty = false;
		this.lbBranch.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbBranch.Location = new System.Drawing.Point(8, 131);
		this.lbBranch.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lbBranch.Name = "lbBranch";
		this.lbBranch.ReadOnly = false;
		this.lbBranch.SettingInVisible = false;
		this.lbBranch.SettingReadonly = false;
		this.lbBranch.ShowUnderLine = true;
		this.lbBranch.Size = new System.Drawing.Size(129, 20);
		this.lbBranch.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbBranch.TabIndex = 55;
		this.lbBranch.Utext = "Branch";
		this.txt_Branch.EditControl = null;
		this.txt_Branch.Enabled = false;
		this.txt_Branch.FieldName = "Branch";
		this.txt_Branch.IsSearchControl = false;
		this.txt_Branch.Location = new System.Drawing.Point(137, 128);
		this.txt_Branch.Name = "txt_Branch";
		this.txt_Branch.Properties.AppearanceFocused.BackColor = System.Drawing.Color.PapayaWhip;
		this.txt_Branch.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.txt_Branch.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_Branch.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name1"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Value", "Name2", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.txt_Branch.Properties.DisplayMember = "Name";
		this.txt_Branch.Properties.IsSearchControl = false;
		this.txt_Branch.Properties.NullText = "";
		this.txt_Branch.Properties.ShowFooter = false;
		this.txt_Branch.Properties.ShowHeader = false;
		this.txt_Branch.Properties.ValueMember = "Value";
		this.txt_Branch.SettingInVisible = false;
		this.txt_Branch.SettingReadonly = false;
		this.txt_Branch.Size = new System.Drawing.Size(135, 20);
		this.txt_Branch.TabIndex = 8;
		this.dateStart.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.dateStart.EditControl = null;
		this.dateStart.EditValue = null;
		this.dateStart.FieldName = "StartDate";
		this.dateStart.Location = new System.Drawing.Point(619, 45);
		this.dateStart.Name = "dateStart";
		this.dateStart.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dateStart.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dateStart.Properties.IsSearchControl = false;
		this.dateStart.SettingInVisible = false;
		this.dateStart.SettingReadonly = false;
		this.dateStart.Size = new System.Drawing.Size(128, 20);
		this.dateStart.TabIndex = 12;
		this.labelLine1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.labelLine1.EditControl = this.dateStart;
		this.labelLine1.EditValue = "Start Date";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(440, 48);
		this.labelLine1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(180, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 56;
		this.labelLine1.Utext = "Start Date";
		this.txtProductNo_Item.EditControl = null;
		this.txtProductNo_Item.FieldName = null;
		this.txtProductNo_Item.IsSearchControl = false;
		this.txtProductNo_Item.Location = new System.Drawing.Point(365, 151);
		this.txtProductNo_Item.Name = "txtProductNo_Item";
		this.txtProductNo_Item.Properties.AutoLoadDataSource = true;
		this.txtProductNo_Item.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions7, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance12, appearanceHovered12, appearancePressed12, appearanceDisabled12, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, imageOptions8, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance13, appearanceHovered13, appearancePressed13, appearanceDisabled13, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.txtProductNo_Item.Properties.ContainerControl = null;
		this.txtProductNo_Item.Properties.DisplayMember = "";
		this.txtProductNo_Item.Properties.DisplayMember2 = "";
		this.txtProductNo_Item.Properties.ListMandatoryField = null;
		this.txtProductNo_Item.Properties.LoadDataSourceWhenEnter = false;
		this.txtProductNo_Item.Properties.SeparateCharacterForMulti = ';';
		this.txtProductNo_Item.Properties.SeparateString = ", ";
		this.txtProductNo_Item.Properties.UsingAutoCompleteBox = false;
		this.txtProductNo_Item.Properties.UsingDataSource = true;
		this.txtProductNo_Item.Properties.ValidateWhenNoDataSource = false;
		this.txtProductNo_Item.Properties.ValueMember = "";
		this.txtProductNo_Item.SettingInVisible = false;
		this.txtProductNo_Item.SettingReadonly = false;
		this.txtProductNo_Item.Size = new System.Drawing.Size(100, 22);
		this.txtProductNo_Item.TabIndex = 59;
		this.txtProductNo_Item.Visible = false;
		this.txtProductNo_Bom.EditControl = null;
		this.txtProductNo_Bom.FieldName = null;
		this.txtProductNo_Bom.IsSearchControl = false;
		this.txtProductNo_Bom.Location = new System.Drawing.Point(365, 177);
		this.txtProductNo_Bom.Name = "txtProductNo_Bom";
		this.txtProductNo_Bom.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions6.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions7.Image");
		this.txtProductNo_Bom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions9, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance14, appearanceHovered14, appearancePressed14, appearanceDisabled14, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions6, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance15, appearanceHovered15, appearancePressed15, appearanceDisabled15, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.txtProductNo_Bom.Properties.ContainerControl = null;
		this.txtProductNo_Bom.Properties.DisplayMember = "";
		this.txtProductNo_Bom.Properties.DisplayMember2 = "";
		this.txtProductNo_Bom.Properties.ListMandatoryField = null;
		this.txtProductNo_Bom.Properties.LoadDataSourceWhenEnter = false;
		this.txtProductNo_Bom.Properties.SeparateCharacterForMulti = ';';
		this.txtProductNo_Bom.Properties.SeparateString = ", ";
		this.txtProductNo_Bom.Properties.UsingAutoCompleteBox = false;
		this.txtProductNo_Bom.Properties.UsingDataSource = true;
		this.txtProductNo_Bom.Properties.ValidateWhenNoDataSource = false;
		this.txtProductNo_Bom.Properties.ValueMember = "";
		this.txtProductNo_Bom.SettingInVisible = false;
		this.txtProductNo_Bom.SettingReadonly = false;
		this.txtProductNo_Bom.Size = new System.Drawing.Size(100, 20);
		this.txtProductNo_Bom.TabIndex = 60;
		this.txtProductNo_Bom.Visible = false;
		this.gridColumnEx6.Caption = "OldLineQty";
		this.gridColumnEx6.CurrencyDateField = null;
		this.gridColumnEx6.CurrencyField = null;
		this.gridColumnEx6.CurrencyFixed = null;
		this.gridColumnEx6.CurrencyFixedRate = 0.0;
		this.gridColumnEx6.CurrencyRateField = null;
		this.gridColumnEx6.FieldName = "OldLineQty";
		this.gridColumnEx6.Name = "gridColumnEx6";
		this.gridColumnEx6.PrimaryKey = false;
		this.gridColumnEx6.UnitField = null;
		this.gridColumnEx7.Caption = "OldFatherQty";
		this.gridColumnEx7.CurrencyDateField = null;
		this.gridColumnEx7.CurrencyField = null;
		this.gridColumnEx7.CurrencyFixed = null;
		this.gridColumnEx7.CurrencyFixedRate = 0.0;
		this.gridColumnEx7.CurrencyRateField = null;
		this.gridColumnEx7.FieldName = "OldFatherQty";
		this.gridColumnEx7.Name = "gridColumnEx7";
		this.gridColumnEx7.PrimaryKey = false;
		this.gridColumnEx7.UnitField = null;
		base.AcceptButton = this.btn_OK;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btn_Cancel;
		base.ClientSize = new System.Drawing.Size(757, 665);
		this.ContextMenuStrip = this.contextMenuStrip_Doc;
		base.Controls.Add(this.txtProductNo_Bom);
		base.Controls.Add(this.txtProductNo_Item);
		base.Controls.Add(this.txtProductNo);
		base.Controls.Add(this.calcPlannedQuantity);
		base.Controls.Add(this.txtProject);
		base.Controls.Add(this.lb_Project);
		base.Controls.Add(this.lb_UoMName);
		base.Controls.Add(this.dateStart);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.lbBranch);
		base.Controls.Add(this.txt_Branch);
		base.Controls.Add(this.look_OriginType);
		base.Controls.Add(this.look_Series);
		base.Controls.Add(this.luk_Doc_User);
		base.Controls.Add(this.cbDistrRule);
		base.Controls.Add(this.luk_Doc_Status);
		base.Controls.Add(this.luk_Doc_Type);
		base.Controls.Add(this.btn_OK);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.lb_PickAndPackRemarks);
		base.Controls.Add(this.lb_Remarks);
		base.Controls.Add(this.memoRemarks);
		base.Controls.Add(this.memoPickRemarks);
		base.Controls.Add(this.xtraTabControl1);
		base.Controls.Add(this.dateDue);
		base.Controls.Add(this.dateOrder);
		base.Controls.Add(this.txtNo);
		base.Controls.Add(this.txtCustomer);
		base.Controls.Add(this.txtSaleOrder);
		base.Controls.Add(this.txtProductDesc);
		base.Controls.Add(this.txtWarehouse);
		base.Controls.Add(this.lb_Status);
		base.Controls.Add(this.lb_ProductNo);
		base.Controls.Add(this.lb_ProductDescription);
		base.Controls.Add(this.lb_PlannedQuantity);
		base.Controls.Add(this.lb_No);
		base.Controls.Add(this.lb_OrderDate);
		base.Controls.Add(this.lb_DueDate);
		base.Controls.Add(this.lb_User);
		base.Controls.Add(this.lb_Origin);
		base.Controls.Add(this.lb_SalesOrder);
		base.Controls.Add(this.lb_Customer);
		base.Controls.Add(this.lb_DistrRule);
		base.Controls.Add(this.lb_Warehouse);
		base.Controls.Add(this.lb_Type);
		base.Controls.Add(this.txtUoMName);
		base.FormId = "oProductionOrder";
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_ProductionOrder.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_ProductionOrder";
		this.Text = "Production Order";
		base.OnControlValueChanged += new Apzon.Controls.Form.ControlValueChanged(this.frm_ProductionOrder_OnControlValueChanged);
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx6).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx7).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx8).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Doc_Type.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtWarehouse.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtProject.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbDistrRule.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtCustomer.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtSaleOrder.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.look_OriginType.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Doc_User.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dateDue.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dateDue.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dateOrder.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dateOrder.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtNo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calcPlannedQuantity.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtProductDesc.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtProductNo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Doc_Status.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtUoMName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).EndInit();
		this.xtraTabControl1.ResumeLayout(false);
		this.tab_Components.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.spl_LinesBatchSerial).EndInit();
		this.spl_LinesBatchSerial.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdDoc).EndInit();
		this.contextMenuStrip_Line.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gvDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Line_Type).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rBT_ItemCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rBT_WareH).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rBT_WibActCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_bte_lineProject).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_line_IssueMtd).EndInit();
		this.tab_Summary.ResumeLayout(false);
		this.tab_Summary.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.calcTotalVariance.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calcAtlProductCost.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calcAtlItemCost.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtJournalRemark.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtOverDue.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtDueDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtDueDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtAtlClosingDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtAtlClosingDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calcRejectedQuantityInfo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calcCompletedQuantityInfo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calcPlannedQuantityInfo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calcAtlByProductCost.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calcAtlAdditionalCost.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calcAtlResourceCost.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtTotalTime.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtTotalAdditionalTime.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtTotalRunTime.Properties).EndInit();
		this.tab_Attachments.ResumeLayout(false);
		this.tab_UDF.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_UDF).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_UDF).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemComboBox3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemButtonEdit2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemButtonEdit3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemComboBox2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemButtonEdit4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemComboBox1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemButtonEdit5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.memoPickRemarks.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.memoRemarks.Properties).EndInit();
		this.contextMenuStrip_Doc.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.look_Series.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Branch.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dateStart.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dateStart.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtProductNo_Item.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtProductNo_Bom.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
