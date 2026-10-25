using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Constants;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;
using Apzon.Entities.Models.Production;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Forms.Properties;
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
using DevExpress.XtraTab;
using SAPbobsCOM;

namespace Apzon.Forms.Production;

public class frm_ProductionOrderImportTool : XtraFormEx
{
	private IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private readonly int _startDataRow = 1;

	private RepositoryItemLookUpEdit _rep_luk_Status = new RepositoryItemLookUpEdit();

	private RepositoryItemLookUpEdit _rep_luk_IssueMethodCP = new RepositoryItemLookUpEdit();

	private RepositoryItemLookUpEdit _rep_luk_Type = new RepositoryItemLookUpEdit();

	private RepositoryItemButtonEditCFL _rep_luk_ItemCode = new RepositoryItemButtonEditCFL();

	private RepositoryItemButtonEditCFL _rep_luk_WhsCode = new RepositoryItemButtonEditCFL();

	private RepositoryItemButtonEditCFL _rep_luk_WhsCodeEdit = new RepositoryItemButtonEditCFL();

	private RepositoryItemButtonEdit _rep_bte_DocNum = new RepositoryItemButtonEdit();

	private RepositoryItemButtonEditCFL bte_ToBin = new RepositoryItemButtonEditCFL();

	private DataTable _DTBinDataTransfer = new DataTable();

	private DataTable _dtStructFG = new DataTable();

	private bool _isLoading;

	private bool _isImportFromFile;

	private bool _isTransferRowObjectChanged;

	private bool _isSetBinBatchData;

	private string[] _allowEditColWhenUpdateFG = new string[3] { "DueDate", "U_PlnCode", "U_LotNo" };

	private string[] _allowEditTR = new string[2] { "DfltWH", "RequestQty" };

	private string[] _allowEditRC = new string[3] { "CmpQuantiy", "RjQuantity", "PostingDate" };

	private string[] _allowEditCL = new string[1] { "CloseDate" };

	private string[] _allowShowingEditorTR = new string[5] { "DocNum", "DfltWH", "RequestQty", "DocItemCode", "ItemCode" };

	private string[] _allowShowingEditorFG = new string[5] { "ItemCode", "DocNum", "DueDate", "U_PlnCode", "U_LotNo" };

	private string[] _allowShowingEditorCP = new string[3] { "ItemCode", "DocNum", "DocItemCode" };

	private string[] _allowShowingEditorCL = new string[3] { "ItemCode", "DocNum", "CloseDate" };

	private string[] _allowShowingEditorRC = new string[5] { "ItemCode", "DocNum", "CmpQuantiy", "RjQuantity", "PostingDate" };

	private List<string> _binWarehouse = new List<string>();

	private List<Tuple<string, string, string>> _imporFgErrors = new List<Tuple<string, string, string>>();

	private int _packQty;

	private int _sleepTime;

	private int _totals;

	private int _proccessedFg;

	private int _totalImportFgTime;

	private DataSet _dsImportFgWithSerial = new DataSet();

	private System.Windows.Forms.Timer _imporFgtTimer = new System.Windows.Forms.Timer();

	private string _txtElapsedTime = "Elapsed Time - ";

	private string _fgPlnCode = "";

	private IContainer components;

	private XtraTabControlEx xtraTabControl1;

	private XtraTabPageEx xPage_ImportFinishGoods;

	private XtraTabPageEx xPage_Components;

	private XtraTabPageEx xPage_ReportComplete;

	private XtraTabPageEx xPage_CloseProduction;

	private TextEditEx txt_Code;

	private LabelLine lb_Name;

	private GridControlEx grdEx_FinishGoods;

	private GridViewEx gvEx_FinishGoods;

	private SimpleButtonEx btn_SaveFG;

	private SimpleButtonEx btn_SaveComp;

	private GridControlEx grdEx_Components;

	private GridViewEx gvEx_Components;

	private SimpleButtonEx btn_SaveRC;

	private GridControlEx grdEx_RC;

	private GridViewEx gvEx_RC;

	private SimpleButtonEx btn_SaveClose;

	private GridControlEx grdEx_Close;

	private GridViewEx gvEx_Close;

	private ContextMenuStrip ctm_FG;

	private ToolStripMenuItem tsi_addRowFG;

	private ToolStripMenuItem tsi_removeRowFG;

	private XtraTabPageEx xPage_Transfer;

	private SimpleButtonEx btn_SaveTransfer;

	private GridControlEx grdEx_Transfer;

	private GridViewEx gvEx_Transfer;

	private SimpleButtonEx btn_ImportFG;

	private SimpleButtonEx btn_UpdateTime;

	private SimpleButtonEx btn_Barcode;

	private SplitContainerControl splitContainerControl1;

	private GridControlEx grdEx_TransferBin;

	private GridViewEx gvEx_TransferBin;

	private GridColumnEx gridColumnEx2;

	private GridColumnEx gridColumnEx3;

	private GridColumnEx gridColumnEx4;

	private GridColumnEx gridColumnEx5;

	private GridColumnEx gridColumnEx6;

	private GridColumnEx gridColumnEx7;

	private GridColumnEx gridColumnEx8;

	private XtraTabControlEx xtraTabControl2;

	private XtraTabPageEx xtraTabPage1;

	private XtraTabPageEx xtraTabPage2;

	private GridControlEx grdEx_TransferedBin;

	private GridViewEx gvEx_TransferedBin;

	private GridColumnEx gridColumnEx10;

	private GridColumnEx gridColumnEx11;

	private GridColumnEx gridColumnEx13;

	private GridColumnEx gridColumnEx14;

	private GridColumnEx gridColumnEx15;

	private GridColumnEx gridColumnEx16;

	private GridColumnEx gridColumnEx9;

	private GridColumnEx gridColumnEx1;

	private RepositoryItemCalcEdit repositoryItemCalcEdit1;

	private LabelLine labelLine1;

	private DateEditEx date_InvTransferPostDate;

	private DateEditEx date_RcPostDate;

	private LabelLine labelLine2;

	private SimpleButtonEx btn_ImportToDb;

	private CalcEdit calc_FGInteruptTime;

	private LabelLine labelLine4;

	private LabelLine labelLine3;

	private CalcEdit calc_NoOfPrdAtime;

	private Label lbl_ImportStatus;

	private System.Windows.Forms.ProgressBar prg_ProcessFg;

	private BackgroundWorker bgw_FG;

	private XtraTabPage xPage_FinishGoods;

	private GridControlEx grd_FindFg;

	private GridViewEx gv_FindFg;

	private Label lbl_ImportFgElapseTime;

	private BackgroundWorker bgw_SaveTmpFg;

	private Label lbl_UpdPrdStatus;

	public async Task MODE_FIND()
	{
		try
		{
			SimpleButton simpleButton = new SimpleButton();
			GridControlEx gridControlEx = new GridControlEx();
			switch (xtraTabControl1.SelectedTabPage.Name)
			{
			case "xPage_ImportFinishGoods":
				simpleButton = btn_SaveFG;
				gridControlEx = grdEx_FinishGoods;
				break;
			case "xPage_FinishGoods":
				return;
			case "xPage_Components":
				simpleButton = btn_SaveComp;
				gridControlEx = grdEx_Components;
				break;
			case "xPage_Transfer":
				simpleButton = btn_SaveTransfer;
				gridControlEx = grdEx_Transfer;
				break;
			case "xPage_ReportComplete":
				simpleButton = btn_SaveRC;
				gridControlEx = grdEx_RC;
				break;
			case "xPage_CloseProduction":
				simpleButton = btn_SaveClose;
				gridControlEx = grdEx_Close;
				break;
			}
			bool flag = false;
			if (string.Compare(simpleButton.Text, "find", ignoreCase: true) == 0)
			{
				return;
			}
			if (string.Compare(simpleButton.Text, "update", ignoreCase: true) == 0 || string.Compare(simpleButton.Text, "add", ignoreCase: true) == 0)
			{
				flag = _isLoading || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes;
			}
			else if (string.Compare(simpleButton.Text, "add", ignoreCase: true) == 0)
			{
				flag = true;
			}
			if (xtraTabControl1.SelectedTabPage.Name == "xPage_ReportComplete" || xtraTabControl1.SelectedTabPage.Name == "xPage_Transfer" || xtraTabControl1.SelectedTabPage.Name == "xPage_CloseProduction" || xtraTabControl1.SelectedTabPage.Name == "xPage_ImportFinishGoods")
			{
				flag = true;
			}
			if (flag)
			{
				btn_ImportToDb.Enabled = false;
				btn_ImportFG.Enabled = true;
				_isImportFromFile = false;
				prg_ProcessFg.Value = 0;
				prg_ProcessFg.Visible = false;
				lbl_ImportFgElapseTime.Visible = false;
				lbl_ImportFgElapseTime.Text = _txtElapsedTime;
				lbl_ImportStatus.Visible = false;
				lbl_ImportStatus.Text = "";
				if (xtraTabControl1.SelectedTabPage.Name != "xPage_ImportFinishGoods")
				{
					simpleButton.Text = "Find";
				}
				gvEx_FinishGoods.OptionsSelection.MultiSelect = true;
				gridControlEx.DataSource = null;
				gridControlEx.RefreshDataSource();
				if (xtraTabControl1.SelectedTabPage.Name == "xPage_Transfer")
				{
					_DTBinDataTransfer.Rows.Clear();
					_DTBinDataTransfer.AcceptChanges();
					grdEx_TransferBin.DataSource = _DTBinDataTransfer.Copy();
					grdEx_TransferBin.RefreshDataSource();
				}
				simpleButton.Enabled = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task REFRESH()
	{
	}

	public frm_ProductionOrderImportTool()
	{
		InitializeComponent();
		OnLoadAsync += frm_ProductionOrderImportTool_Load;
	}

	private async Task frm_ProductionOrderImportTool_Load(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			calc_FGInteruptTime.EditValue = 2;
			calc_NoOfPrdAtime.EditValue = 10;
			btn_ImportToDb.Enabled = false;
			prg_ProcessFg.Minimum = 0;
			prg_ProcessFg.Step = 1;
			_imporFgtTimer.Interval = 1000;
			_imporFgtTimer.Tick += _imporFgtTimer_Tick;
			DataTable dataTable = await _httpClient.ProductionOrderImportTool.GetBinWarehouse();
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				foreach (DataRow row in dataTable.Rows)
				{
					_binWarehouse.Add(Function.ToString(row[0]).ToLower());
				}
			}
			_dtStructFG = _httpClient.ProductionOrderImportTool.GetTableStructure("FG");
			date_RcPostDate.EditValue = DateTime.Today;
			DataTable editTableEnableColumns = _httpClient.ProductionOrderImportTool.GetEditTableEnableColumns();
			if (editTableEnableColumns != null && editTableEnableColumns.Rows.Count > 0)
			{
				_allowEditColWhenUpdateFG = Function.ToString(editTableEnableColumns.Rows[0]["allowEditColWhenUpdateFG"]).Split(',');
				_allowEditTR = Function.ToString(editTableEnableColumns.Rows[0]["allowEditTR"]).Split(',');
				_allowEditRC = Function.ToString(editTableEnableColumns.Rows[0]["allowEditRC"]).Split(',');
				_allowEditCL = Function.ToString(editTableEnableColumns.Rows[0]["allowEditCL"]).Split(',');
				_allowShowingEditorTR = Function.ToString(editTableEnableColumns.Rows[0]["allowShowingEditorTR"]).Split(',');
				_allowShowingEditorFG = Function.ToString(editTableEnableColumns.Rows[0]["allowShowingEditorFG"]).Split(',');
				_allowShowingEditorCP = Function.ToString(editTableEnableColumns.Rows[0]["allowShowingEditorCP"]).Split(',');
				_allowShowingEditorCL = Function.ToString(editTableEnableColumns.Rows[0]["allowShowingEditorCL"]).Split(',');
				_allowShowingEditorRC = Function.ToString(editTableEnableColumns.Rows[0]["allowShowingEditorRC"]).Split(',');
			}
			LoadComboBox();
			FormId = "oProductionOrderImportTool";
			gvEx_FinishGoods.ItemId = "POIT_FG";
			gv_FindFg.ItemId = "POIT_FG1";
			gvEx_Components.ItemId = "POIT_COMP";
			gvEx_Transfer.ItemId = "POIT_TRNS";
			gvEx_RC.ItemId = "POIT_RC";
			gvEx_Close.ItemId = "POIT_CL";
			_DTBinDataTransfer = await _httpClient.ProductionOrderImportTool.GetAllBinData("-1", "");
			_DTBinDataTransfer.Rows.Clear();
			_DTBinDataTransfer.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void _imporFgtTimer_Tick(object sender, EventArgs e)
	{
		try
		{
			_totalImportFgTime++;
			double num = 0.0;
			double num2 = 0.0;
			double num3 = 0.0;
			num = Math.Floor((double)(_totalImportFgTime / 3600));
			num2 = Math.Floor(((double)_totalImportFgTime - num * 3600.0) / 60.0);
			num3 = (double)_totalImportFgTime - (num * 3600.0 + num2 * 60.0);
			lbl_ImportFgElapseTime.Text = _txtElapsedTime + num + ":" + num2 + ":" + num3;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void grdEx_FinishGoods_ProcessGridKey(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.V && e.Modifiers == Keys.Control && string.Compare(btn_SaveFG.Text, "add", ignoreCase: true) == 0)
			{
				WinCommons._OpenProcessing();
				PasteFromExcelToDataTable(grdEx_FinishGoods, gvEx_FinishGoods, new List<string> { "ItemCode" });
				WinCommons._CloseProcessing();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void _rep_bte_DocEntryComp_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			this.DrillDown(ApzObjectTypes.oProductionOrders, ApzObjectTypes.oProductionOrders, Function.ParseInt((((sender as ButtonEdit).Parent as GridControl).FocusedView as GridView).GetFocusedRowCellValue("DocNum")).ToString());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void txt_Code_KeyDown(object sender, KeyEventArgs e)
	{
		_ = 1;
		try
		{
			if (e.KeyCode != Keys.Return)
			{
				return;
			}
			switch (xtraTabControl1.SelectedTabPage.Name)
			{
			case "xPage_ImportFinishGoods":
			{
				await Search(grdEx_FinishGoods, gvEx_FinishGoods, "FG");
				for (int i = 0; i < gvEx_FinishGoods.Columns.Count; i++)
				{
					GridColumn gridColumn = gvEx_FinishGoods.Columns[i];
					if (!_allowEditColWhenUpdateFG.Contains(Function.ToString(gridColumn.FieldName)))
					{
						gridColumn.OptionsColumn.ReadOnly = true;
					}
				}
				btn_ImportToDb.Enabled = false;
				btn_ImportFG.Enabled = true;
				_isImportFromFile = false;
				gvEx_FinishGoods.OptionsSelection.MultiSelect = true;
				break;
			}
			case "xPage_FinishGoods":
			{
				await Search(grd_FindFg, gv_FindFg, "FG1");
				for (int j = 0; j < gv_FindFg.Columns.Count; j++)
				{
					gv_FindFg.Columns[j].OptionsColumn.ReadOnly = true;
				}
				break;
			}
			case "xPage_Components":
				if (string.Compare(btn_SaveComp.Text, "find", ignoreCase: true) == 0)
				{
					btn_SaveComp_Click(null, null);
				}
				break;
			case "xPage_Transfer":
				if (string.Compare(btn_SaveTransfer.Text, "find", ignoreCase: true) == 0)
				{
					btn_SaveTransfer_Click(null, null);
				}
				break;
			case "xPage_ReportComplete":
				if (string.Compare(btn_SaveRC.Text, "find", ignoreCase: true) == 0)
				{
					btn_SaveRC_Click(null, null);
				}
				break;
			case "xPage_CloseProduction":
				if (string.Compare(btn_SaveClose.Text, "find", ignoreCase: true) == 0)
				{
					btn_SaveClose_Click(null, null);
				}
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		try
		{
			if (e.Action != CollectionChangeAction.Remove)
			{
				PatchSelection(sender as GridView);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gridView_RowStyle(object sender, RowStyleEventArgs e)
	{
		try
		{
			string text = Function.ToString((sender as GridView).GetRowCellValue(e.RowHandle, "Status"));
			if (text == "C" || text == "L")
			{
				e.Appearance.BackColor = StyleColor.GridDisable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void _rep_luk_WhsCode_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 1)
			{
				ButtonEdit buttonEdit = sender as ButtonEdit;
				this.DrillDown(ApzObjectTypes.oWarehouses, Function.ToString(buttonEdit.EditValue));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void _rep_luk_ItemCode_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 1)
			{
				ButtonEdit buttonEdit = sender as ButtonEdit;
				this.DrillDown(ApzObjectTypes.oItems, Function.ToString(buttonEdit.EditValue));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void xtraTabControl1_SelectedPageChanged(object sender, TabPageChangedEventArgs e)
	{
		try
		{
			if (string.IsNullOrWhiteSpace(txt_Code.Text))
			{
				return;
			}
			switch (e.Page.Name)
			{
			case "xPage_CloseProduction":
				if (string.Compare(btn_SaveClose.Text, "find", ignoreCase: true) == 0)
				{
					btn_SaveClose_Click(null, null);
				}
				break;
			case "xPage_Components":
				if (string.Compare(btn_SaveComp.Text, "find", ignoreCase: true) == 0)
				{
					btn_SaveComp_Click(null, null);
				}
				break;
			case "xPage_FinishGoods":
				if (string.Compare(btn_SaveFG.Text, "find", ignoreCase: true) == 0)
				{
					btn_SaveFG_Click(null, null);
				}
				break;
			case "xPage_ReportComplete":
				if (string.Compare(btn_SaveRC.Text, "find", ignoreCase: true) == 0)
				{
					btn_SaveRC_Click(null, null);
				}
				break;
			case "xPage_Transfer":
				if (string.Compare(btn_SaveTransfer.Text, "find", ignoreCase: true) == 0)
				{
					btn_SaveTransfer_Click(null, null);
				}
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvEx_TransferBin_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
	{
		try
		{
			if (!(gvEx_TransferBin.FocusedColumn.FieldName == "Quantity"))
			{
				return;
			}
			string text = Function.ToString(gvEx_Transfer.GetFocusedRowCellValue("ManBatchSer"));
			decimal num = Function.ParseDecimal(gvEx_Transfer.GetFocusedRowCellValue("RequestQty"));
			decimal num2 = Function.ParseDecimal(e.Value);
			decimal num3 = Function.ParseDecimal(gvEx_TransferBin.GetFocusedRowCellValue("Quantity"));
			if (text == "B")
			{
				decimal num4 = Function.ParseDecimal(gvEx_TransferBin.GetFocusedRowCellValue("AvailQty"));
				if (num2 > num4)
				{
					e.ErrorText = "Quantity cannot exceed available quantity";
					e.Valid = false;
				}
			}
			else if (text == "S" && num2 > 1m)
			{
				e.ErrorText = "Serial quantity must equal 1";
				e.Valid = false;
			}
			if (grdEx_TransferBin.DataSource is DataTable dataTable && dataTable.Rows.Count > 0 && dataTable.AsEnumerable().Sum((DataRow r) => Function.ParseDecimal(r["Quantity"])) - num3 + num2 > num)
			{
				e.ErrorText = "Total quantity cannot greater than line quantity";
				e.Valid = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvEx_TransferBin_InvalidValueException(object sender, InvalidValueExceptionEventArgs e)
	{
		try
		{
			e.ExceptionMode = ExceptionMode.DisplayError;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void date_RcPostDate_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			DateTime? dateTime = Function.ParseDateTime(date_RcPostDate.EditValue);
			for (int i = 0; i < gvEx_RC.DataRowCount; i++)
			{
				gvEx_RC.SetRowCellValue(i, "PostingDate", dateTime);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvEx_Transfer_BeforeLeaveRow(object sender, RowAllowEventArgs e)
	{
		try
		{
			if (sender is GridView)
			{
				SetBinBatchDataTransfer(e.RowHandle);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void grdEx_Transfer_EditorKeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode != Keys.V || e.Modifiers != Keys.Control || !(gvEx_Transfer.FocusedColumn.FieldName == "DfltWH"))
			{
				return;
			}
			int focusedRowHandle = gvEx_Transfer.FocusedRowHandle;
			DataObject dataObject = (DataObject)Clipboard.GetDataObject();
			DataTable dt = new DataTable();
			if (dataObject != null && dataObject.GetData(DataFormats.UnicodeText) != null)
			{
				string clipboardData = dataObject.GetData(DataFormats.UnicodeText).ToString();
				dt = (dataObject.GetDataPresent(DataFormats.Text) ? Function.PasteFromExcel(clipboardData) : new DataTable());
			}
			if (focusedRowHandle < 0 || dt == null || dt.Rows.Count <= 0)
			{
				return;
			}
			GridViewInfo gridViewInfo = WinFuntions.GetGridViewInfo(gvEx_Transfer);
			int rowIndex = 0;
			int countIndex = ((dt.Rows.Count < gridViewInfo.RowsInfo.Count) ? (dt.Rows.Count + focusedRowHandle) : (gridViewInfo.RowsInfo.Count + focusedRowHandle));
			for (int i = focusedRowHandle; i < countIndex; i++)
			{
				string whsCode = Function.ToString(dt.Rows[rowIndex][0]);
				DataTable dataTable = await ExtenseionMethods.GetWareHouseByCode(whsCode);
				if (dataTable == null || dataTable.Rows.Count == 0)
				{
					whsCode = "";
				}
				gvEx_Transfer.SetRowCellValue(i, "DfltWH", whsCode);
				rowIndex++;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void grdEx_RC_EditorKeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode != Keys.V || e.Modifiers != Keys.Control || !(gvEx_RC.FocusedColumn.FieldName == "Warehouse"))
			{
				return;
			}
			int focusedRowHandle = gvEx_RC.FocusedRowHandle;
			DataObject dataObject = (DataObject)Clipboard.GetDataObject();
			DataTable dt = new DataTable();
			if (dataObject != null && dataObject.GetData(DataFormats.UnicodeText) != null)
			{
				string clipboardData = dataObject.GetData(DataFormats.UnicodeText).ToString();
				dt = (dataObject.GetDataPresent(DataFormats.Text) ? Function.PasteFromExcel(clipboardData) : new DataTable());
			}
			if (focusedRowHandle < 0 || dt == null || dt.Rows.Count <= 0)
			{
				return;
			}
			GridViewInfo gridViewInfo = WinFuntions.GetGridViewInfo(gvEx_RC);
			int rowIndex = 0;
			int countIndex = ((dt.Rows.Count < gridViewInfo.RowsInfo.Count) ? (dt.Rows.Count + focusedRowHandle) : (gridViewInfo.RowsInfo.Count + focusedRowHandle));
			for (int i = focusedRowHandle; i < countIndex; i++)
			{
				string whsCode = Function.ToString(dt.Rows[rowIndex][0]);
				DataTable dataTable = await ExtenseionMethods.GetWareHouseByCode(whsCode);
				if (dataTable == null || dataTable.Rows.Count == 0)
				{
					whsCode = "";
				}
				gvEx_RC.SetRowCellValue(i, "Warehouse", whsCode);
				rowIndex++;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void gvEx_Transfer_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		if (_isTransferRowObjectChanged)
		{
			_isTransferRowObjectChanged = false;
			return;
		}
		_isTransferRowObjectChanged = true;
		await ShowBinBatchDataTransfer();
	}

	private async void gvEx_Transfer_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
	{
		if (_isTransferRowObjectChanged)
		{
			_isTransferRowObjectChanged = false;
			return;
		}
		_isTransferRowObjectChanged = true;
		await ShowBinBatchDataTransfer();
	}

	private void gvEx_FinishGoods_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (e.Column.OptionsColumn.ReadOnly || !e.Column.OptionsColumn.AllowEdit)
			{
				e.Appearance.BackColor = StyleColor.GridDisable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvEx_Components_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (e.Column.OptionsColumn.ReadOnly || !e.Column.OptionsColumn.AllowEdit)
			{
				e.Appearance.BackColor = StyleColor.GridDisable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvEx_Transfer_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (e.Column.OptionsColumn.ReadOnly || !e.Column.OptionsColumn.AllowEdit)
			{
				e.Appearance.BackColor = StyleColor.GridDisable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvEx_RC_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (e.Column.OptionsColumn.ReadOnly || !e.Column.OptionsColumn.AllowEdit)
			{
				e.Appearance.BackColor = StyleColor.GridDisable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvEx_Close_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (e.Column.OptionsColumn.ReadOnly || !e.Column.OptionsColumn.AllowEdit)
			{
				e.Appearance.BackColor = StyleColor.GridDisable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvEx_FinishGoods_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			GridView gridView = sender as GridView;
			if (!_allowShowingEditorFG.Contains(gridView.FocusedColumn.FieldName))
			{
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvEx_Components_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			GridView gridView = sender as GridView;
			if (!_allowShowingEditorCP.Contains(gridView.FocusedColumn.FieldName))
			{
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvEx_Transfer_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			GridView gridView = sender as GridView;
			if (!_allowShowingEditorTR.Contains(gridView.FocusedColumn.FieldName))
			{
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvEx_RC_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			GridView gridView = sender as GridView;
			if (!_allowShowingEditorRC.Contains(gridView.FocusedColumn.FieldName))
			{
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvEx_Close_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			GridView gridView = sender as GridView;
			if (!_allowShowingEditorCL.Contains(gridView.FocusedColumn.FieldName))
			{
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvEx_FinishGoods_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			_ = _isLoading;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void gvEx_Transfer_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				btn_SaveTransfer.Text = "Confirm";
				if (e.Column.FieldName == "RequestQty")
				{
					gvEx_Transfer.SelectRow(e.RowHandle);
				}
				else if (e.Column.FieldName == "DfltWH")
				{
					await ReSetBinBatchDataTransfer(e.RowHandle);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvEx_RC_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				if (e.Column.FieldName == "CmpQuantiy" || e.Column.FieldName == "RjQuantity")
				{
					gvEx_RC.SelectRow(e.RowHandle);
				}
				btn_SaveRC.Text = "Confirm";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvEx_Close_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				btn_SaveClose.Text = "Confirm";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void tsi_addRowFG_Click(object sender, EventArgs e)
	{
		try
		{
			AddNewFGRow();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void tsi_removeRowFG_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataTable = grdEx_FinishGoods.DataSource as DataTable;
			gvEx_FinishGoods.DeleteSelectedRows();
			dataTable?.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btn_SaveFG_Click(object sender, EventArgs e)
	{
		try
		{
			EnableDisableButton(isEnable: false);
			_imporFgErrors = new List<Tuple<string, string, string>>();
			if (!(grdEx_FinishGoods.DataSource is DataTable dataTable) || dataTable.Rows.Count <= 0)
			{
				WinCommons.Error(Enums.MessageCode.NoDataToSave, "");
				EnableDisableButton(isEnable: true);
				btn_ImportToDb.Enabled = false;
				return;
			}
			DataTable dataTable2 = dataTable.Clone();
			int[] selectedRows = gvEx_FinishGoods.GetSelectedRows();
			if (selectedRows.Length != 0)
			{
				for (int i = 0; i < selectedRows.Length; i++)
				{
					dataTable2.ImportRow(dataTable.Rows[gvEx_FinishGoods.GetDataSourceRowIndex(selectedRows[i])]);
				}
			}
			dataTable2.AcceptChanges();
			for (int num = dataTable2.Rows.Count - 1; num >= 0; num--)
			{
				DataRow dataRow = dataTable2.Rows[num];
				if (string.IsNullOrWhiteSpace(Function.ToString(dataRow["ItemCode"])))
				{
					dataTable2.Rows.Remove(dataRow);
				}
			}
			dataTable2.AcceptChanges();
			if (!ValidateData(dataTable2))
			{
				EnableDisableButton(isEnable: true);
				btn_ImportToDb.Enabled = false;
				return;
			}
			_packQty = Function.ParseInt(calc_NoOfPrdAtime.EditValue);
			_sleepTime = Function.ParseInt(calc_FGInteruptTime.EditValue);
			_totals = dataTable2.Rows.Count;
			lbl_ImportStatus.Visible = true;
			lbl_ImportFgElapseTime.Visible = true;
			prg_ProcessFg.Maximum = _totals;
			prg_ProcessFg.Value = 0;
			prg_ProcessFg.Visible = true;
			lbl_ImportStatus.Text = "0/" + _totals + " rows.";
			_totalImportFgTime = 0;
			_proccessedFg = 0;
			_imporFgtTimer.Start();
			bgw_FG.RunWorkerAsync();
		}
		catch (Exception ex)
		{
			EnableDisableButton(isEnable: true);
			btn_ImportToDb.Enabled = false;
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.None, ex.Message);
		}
		btn_SaveFG.Enabled = false;
		btn_ImportToDb.Enabled = false;
	}

	private async void btn_SaveComp_Click(object sender, EventArgs e)
	{
		try
		{
			if (string.Compare(btn_SaveComp.Text, "find", ignoreCase: true) == 0)
			{
				await Search(grdEx_Components, gvEx_Components, "CP");
				GridColumn gridColumn = gvEx_Components.Columns["IssueType"];
				if (gridColumn != null)
				{
					gridColumn.ColumnEdit = _rep_luk_IssueMethodCP;
				}
				for (int i = 0; i < gvEx_Components.Columns.Count; i++)
				{
					gvEx_Components.Columns[i].OptionsColumn.ReadOnly = true;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btn_SaveTransfer_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (string.Compare(btn_SaveTransfer.Text, "find", ignoreCase: true) == 0)
			{
				splitContainerControl1.PanelVisibility = SplitPanelVisibility.Panel1;
				await Search(grdEx_Transfer, gvEx_Transfer, "TR");
				for (int i = 0; i < gvEx_Transfer.Columns.Count; i++)
				{
					GridColumn gridColumn = gvEx_Transfer.Columns[i];
					if (!_allowEditTR.Contains(Function.ToString(gridColumn.FieldName)))
					{
						gridColumn.OptionsColumn.ReadOnly = true;
					}
					else
					{
						gridColumn.OptionsColumn.ReadOnly = false;
					}
				}
			}
			else
			{
				EnableDisableButton(isEnable: false);
				await SaveTransfer();
				EnableDisableButton(isEnable: true);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btn_SaveRC_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (string.Compare(btn_SaveRC.Text, "find", ignoreCase: true) == 0)
			{
				await Search(grdEx_RC, gvEx_RC, "RC");
				for (int i = 0; i < gvEx_RC.Columns.Count; i++)
				{
					GridColumn gridColumn = gvEx_RC.Columns[i];
					if (!_allowEditRC.Contains(Function.ToString(gridColumn.FieldName)))
					{
						gridColumn.OptionsColumn.ReadOnly = true;
					}
					else
					{
						gridColumn.OptionsColumn.ReadOnly = false;
					}
				}
				GridColumn gridColumn2 = gvEx_RC.Columns["Status"];
				if (gridColumn2 != null)
				{
					gridColumn2.ColumnEdit = _rep_luk_Status;
				}
				GridColumn gridColumn3 = gvEx_RC.Columns["Type"];
				if (gridColumn3 != null)
				{
					gridColumn3.ColumnEdit = _rep_luk_Type;
				}
			}
			else
			{
				EnableDisableButton(isEnable: false);
				await SaveRC();
				EnableDisableButton(isEnable: true);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btn_SaveClose_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (string.Compare(btn_SaveClose.Text, "find", ignoreCase: true) == 0)
			{
				await Search(grdEx_Close, gvEx_Close, "CL");
				GridColumn gridColumn = gvEx_Close.Columns["Type"];
				if (gridColumn != null)
				{
					gridColumn.ColumnEdit = _rep_luk_Type;
				}
				for (int i = 0; i < gvEx_Close.Columns.Count; i++)
				{
					GridColumn gridColumn2 = gvEx_Close.Columns[i];
					if (!_allowEditCL.Contains(Function.ToString(gridColumn2.FieldName)))
					{
						gridColumn2.OptionsColumn.ReadOnly = true;
					}
					else
					{
						gridColumn2.OptionsColumn.ReadOnly = false;
					}
				}
			}
			else
			{
				EnableDisableButton(isEnable: false);
				await SaveClose();
				EnableDisableButton(isEnable: true);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_ImportFG_Click(object sender, EventArgs e)
	{
		prg_ProcessFg.Value = 0;
		prg_ProcessFg.Visible = false;
		lbl_ImportStatus.Visible = false;
		lbl_ImportStatus.Text = "";
		lbl_ImportFgElapseTime.Visible = false;
		btn_SaveFG.Enabled = false;
		btn_ImportFG.Enabled = false;
		btn_ImportToDb.Enabled = false;
		try
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "Excels file (*.xlsx)|*.xlsx";
			openFileDialog.Multiselect = false;
			openFileDialog.ShowDialog(this);
			if (File.Exists(openFileDialog.FileName))
			{
				REFRESH();
				Tuple<string, DataTable> dataFromExcelFileFG = GetDataFromExcelFileFG(openFileDialog.FileName);
				if (dataFromExcelFileFG.Item1 == "")
				{
					btn_ImportToDb.Enabled = true;
					_isImportFromFile = true;
					_dsImportFgWithSerial.Tables.Clear();
					grdEx_FinishGoods.DataSource = dataFromExcelFileFG.Item2;
					grdEx_FinishGoods.RefreshDataSource();
					gvEx_FinishGoods.OptionsSelection.MultiSelect = false;
					btn_ImportToDb.Enabled = true;
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, dataFromExcelFileFG.Item1);
					grdEx_FinishGoods.DataSource = null;
					grdEx_FinishGoods.RefreshDataSource();
					btn_ImportToDb.Enabled = false;
				}
			}
			else
			{
				grdEx_FinishGoods.DataSource = null;
				grdEx_FinishGoods.RefreshDataSource();
				btn_ImportToDb.Enabled = false;
			}
		}
		catch (Exception ex)
		{
			grdEx_FinishGoods.DataSource = null;
			grdEx_FinishGoods.RefreshDataSource();
			btn_ImportToDb.Enabled = false;
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		btn_ImportFG.Enabled = true;
		btn_SaveFG.Enabled = false;
	}

	private void btn_ImportPoWserial_Click(object sender, EventArgs e)
	{
		try
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "Excels file (*.xlsx)|*.xlsx";
			openFileDialog.Multiselect = false;
			openFileDialog.ShowDialog(this);
			if (File.Exists(openFileDialog.FileName))
			{
				REFRESH();
				if (string.IsNullOrEmpty(GetDataFromExcelFileFGSerial(openFileDialog.FileName)))
				{
					btn_ImportToDb.Enabled = true;
					_isImportFromFile = true;
					grdEx_FinishGoods.DataSource = _dsImportFgWithSerial.Tables[0];
					grdEx_FinishGoods.RefreshDataSource();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_UpdateTime_Click(object sender, EventArgs e)
	{
		try
		{
			frm_ProdOrderImpToolSetTime frm = new frm_ProdOrderImpToolSetTime();
			frm.FormClosing += (object s1, FormClosingEventArgs e2) =>
			{
				if (frm.isSaved)
				{
					MODE_FIND();
					btn_SaveComp_Click(null, null);
				}
			};
			frm.ShowDialog(this);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_Barcode_Click(object sender, EventArgs e)
	{
		try
		{
			frm_ProdOrderImpToolRCBarcode frm = new frm_ProdOrderImpToolRCBarcode();
			frm.FormClosing += (object s1, FormClosingEventArgs e2) =>
			{
				if (frm.isSaved)
				{
					MODE_FIND();
					btn_SaveRC_Click(null, null);
				}
			};
			frm.ShowDialog(this);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btn_ImportToDb_Click(object sender, EventArgs e)
	{
		lbl_ImportFgElapseTime.Visible = true;
		lbl_ImportFgElapseTime.Text = _txtElapsedTime + "0:0:0";
		_totalImportFgTime = 0;
		_imporFgtTimer.Start();
		btn_ImportToDb.Enabled = false;
		btn_ImportFG.Enabled = false;
		try
		{
			if (!_isImportFromFile)
			{
				return;
			}
			if (grdEx_FinishGoods.DataSource is DataTable dataTable && dataTable.Rows.Count > 0)
			{
				DataTable dataTable2 = dataTable.Copy();
				for (int num = dataTable2.Rows.Count - 1; num >= 0; num--)
				{
					DataRow dataRow = dataTable2.Rows[num];
					if (string.IsNullOrWhiteSpace(Function.ToString(dataRow["ItemCode"])))
					{
						dataTable2.Rows.Remove(dataRow);
					}
				}
				dataTable2.AcceptChanges();
				bgw_SaveTmpFg.RunWorkerAsync();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.NoDataToSave, "");
				btn_ImportFG.Enabled = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void bgw_FG_DoWork(object sender, DoWorkEventArgs e)
	{
		try
		{
			SaveFG();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void bgw_FG_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
	{
		try
		{
			_imporFgtTimer.Stop();
			if (_imporFgErrors.Count > 0)
			{
				ShowErrorResultForm(_imporFgErrors);
			}
			WinCommons.Success(Enums.MessageCode.ImportFinished, "");
			_isLoading = true;
			if (grdEx_FinishGoods.DataSource is DataTable dataTable)
			{
				dataTable.Rows.Clear();
				dataTable.AcceptChanges();
			}
			btn_SaveFG.Enabled = false;
			btn_ImportToDb.Enabled = false;
			_isLoading = false;
			EnableDisableButton(isEnable: true);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		string text = _httpClient.ProductionOrderImportTool.UpdateDataForBarcodeApp(_fgPlnCode);
		if (string.IsNullOrWhiteSpace(text))
		{
			WinCommons.Success(Enums.MessageCode.StoreUpdateProductionOrderExecuteSuccess, "");
		}
		else
		{
			WinCommons.Error(Enums.MessageCode.StoreUpdateProductionFailed, "", text);
		}
		lbl_UpdPrdStatus.Visible = false;
	}

	private void bgw_FG_ProgressChanged(object sender, ProgressChangedEventArgs e)
	{
		try
		{
			if (e.ProgressPercentage == 1)
			{
				int proccessedFg = ((_proccessedFg + _packQty > _totals) ? _totals : (_proccessedFg + _packQty));
				_proccessedFg = proccessedFg;
				prg_ProcessFg.Value = _proccessedFg;
				lbl_ImportStatus.Text = _proccessedFg + "/" + _totals + " rows.";
				lbl_ImportStatus.Update();
			}
			else if (e.ProgressPercentage == 2)
			{
				int? num = e.UserState as int?;
				if (num > 0)
				{
					lbl_UpdPrdStatus.Visible = true;
					Label label = lbl_UpdPrdStatus;
					int? num2 = num;
					label.Text = "Updating status " + num2 + " entries";
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void bgw_SaveTmpFg_DoWork(object sender, DoWorkEventArgs e)
	{
		try
		{
			if (!(grdEx_FinishGoods.DataSource is DataTable dataTable) || dataTable.Rows.Count <= 0)
			{
				return;
			}
			DataTable dtSave = dataTable.Copy();
			for (int num = dtSave.Rows.Count - 1; num >= 0; num--)
			{
				DataRow dataRow = dtSave.Rows[num];
				if (string.IsNullOrWhiteSpace(Function.ToString(dataRow["ItemCode"])))
				{
					dtSave.Rows.Remove(dataRow);
				}
			}
			dtSave.AcceptChanges();
			e.Result = Task.Run(async () => await _httpClient.ProductionOrderImportTool.SaveTempFG(dtSave)).Result;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void bgw_SaveTmpFg_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
	{
		try
		{
			if (e.Result is HttpResult httpResult && !string.IsNullOrEmpty(httpResult.Message))
			{
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				btn_ImportToDb.Enabled = true;
				btn_ImportFG.Enabled = true;
			}
			else
			{
				WinCommons.Success(Enums.MessageCode.Import2TempTableFinishSuccess, "");
				_isLoading = true;
				await Search(grdEx_FinishGoods, gvEx_FinishGoods, "FG");
				for (int i = 0; i < gvEx_FinishGoods.Columns.Count; i++)
				{
					GridColumn gridColumn = gvEx_FinishGoods.Columns[i];
					if (!_allowEditColWhenUpdateFG.Contains(Function.ToString(gridColumn.FieldName)))
					{
						gridColumn.OptionsColumn.ReadOnly = true;
					}
				}
				btn_ImportToDb.Enabled = false;
				_isImportFromFile = false;
				gvEx_FinishGoods.OptionsSelection.MultiSelect = true;
				btn_SaveFG.Enabled = true;
				btn_ImportFG.Enabled = true;
				_isLoading = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		_imporFgtTimer.Stop();
		lbl_ImportFgElapseTime.Visible = false;
	}

	private async Task Search(GridControlEx grd, GridViewEx gv, string tabName)
	{
		try
		{
			_isLoading = true;
			DataTable searchDataTable = GetSearchDataTable();
			DataRow dataRow = searchDataTable.Rows[0];
			dataRow["Code"] = txt_Code.Text.Trim();
			dataRow["TabName"] = tabName;
			dataRow["Branch"] = GlobalData.DefaultBranch;
			searchDataTable.AcceptChanges();
			gv.Columns.Clear();
			DataTable dataTable = _httpClient.ProductionOrderImportTool.Search(searchDataTable);
			switch (tabName)
			{
			case "CP":
			{
				DataTable dataTable2 = dataTable.Copy();
				dataTable2.Columns.Remove("U_Time");
				dataTable2.AcceptChanges();
				dataTable2.Columns.Add(new DataColumn("U_Time", typeof(string)));
				dataTable2.AcceptChanges();
				for (int i = 0; i < dataTable2.Rows.Count; i++)
				{
					dataTable2.Rows[i]["U_Time"] = Function.ParseSAPHourToString(dataTable.Rows[i]["U_Time"]);
				}
				dataTable2.AcceptChanges();
				dataTable = dataTable2;
				break;
			}
			case "TR":
				if (dataTable.IsNotNull())
				{
					btn_SaveTransfer.Text = "Confirm";
				}
				break;
			case "RC":
				if (dataTable.IsNotNull())
				{
					btn_SaveRC.Text = "Confirm";
				}
				break;
			case "CL":
				if (dataTable.IsNotNull())
				{
					btn_SaveClose.Text = "Confirm";
				}
				break;
			case "FG":
				if (dataTable.IsNotNull())
				{
					btn_SaveFG.Enabled = true;
				}
				else
				{
					btn_SaveFG.Enabled = false;
				}
				break;
			}
			grd.DataSource = dataTable;
			grd.RefreshDataSource();
			gv.PopulateColumns();
			if (gv.Columns["DocNum"] != null)
			{
				gv.Columns["DocNum"].OptionsColumn.ReadOnly = true;
				gv.Columns["DocNum"].ColumnEdit = _rep_bte_DocNum;
			}
			if (gv.Columns["Status"] != null)
			{
				gv.Columns["DocNum"].OptionsColumn.ReadOnly = true;
				gv.Columns["Status"].ColumnEdit = _rep_luk_Status;
			}
			if (gv.Columns["ItemCode"] != null)
			{
				gv.Columns["ItemCode"].ColumnEdit = _rep_luk_ItemCode;
			}
			if (gv.Columns["DocItemCode"] != null)
			{
				gv.Columns["DocItemCode"].ColumnEdit = _rep_luk_ItemCode;
			}
			if (gv.Columns["WhsCode"] != null)
			{
				gv.Columns["WhsCode"].ColumnEdit = _rep_luk_WhsCode;
			}
			if (gv.Columns["FromWhsCod"] != null)
			{
				gv.Columns["FromWhsCod"].ColumnEdit = _rep_luk_WhsCode;
			}
			if (gv.Columns["DfltWH"] != null)
			{
				gv.Columns["DfltWH"].ColumnEdit = _rep_luk_WhsCodeEdit;
			}
			if (tabName == "CP")
			{
				GridColumn gridColumn = gv.Columns["U_Time"];
				if (gridColumn != null)
				{
					gridColumn.UnboundType = UnboundColumnType.String;
					RepositoryItemTextEdit repositoryItemTextEdit = new RepositoryItemTextEdit();
					repositoryItemTextEdit.DisplayFormat.FormatType = FormatType.Custom;
					repositoryItemTextEdit.DisplayFormat.FormatString = "##:##";
					repositoryItemTextEdit.EditFormat.FormatType = FormatType.Custom;
					repositoryItemTextEdit.EditFormat.FormatString = "##:##";
					repositoryItemTextEdit.Mask.EditMask = "(0?\\d|1\\d|2[0-3])[0-5]\\d";
					repositoryItemTextEdit.Mask.MaskType = MaskType.RegEx;
					gridColumn.ColumnEdit = repositoryItemTextEdit;
				}
			}
			await gv.ReloadFormSetting();
			_isLoading = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadComboBox()
	{
		try
		{
			_rep_luk_Status = new RepositoryItemLookUpEdit();
			_rep_luk_Status.Name = "rep_luk_StatusFG";
			_rep_luk_Status.SetDataSource(new string[4, 2]
			{
				{ "P", "Planned" },
				{ "R", "Released" },
				{ "L", "Closed" },
				{ "C", "Canceled" }
			}, isStringCode: true);
			if (!grdEx_FinishGoods.RepositoryItems.Contains(_rep_luk_Status))
			{
				grdEx_FinishGoods.RepositoryItems.Add(_rep_luk_Status);
			}
			_rep_luk_ItemCode = new RepositoryItemButtonEditCFL();
			_rep_luk_ItemCode.Buttons[0].Visible = false;
			_rep_luk_ItemCode.ButtonPressed += _rep_luk_ItemCode_ButtonPressed;
			_rep_luk_WhsCode = new RepositoryItemButtonEditCFL();
			_rep_luk_WhsCode.Buttons[0].Visible = false;
			_rep_luk_WhsCode.ButtonPressed += _rep_luk_WhsCode_ButtonPressed;
			_rep_luk_WhsCodeEdit = new RepositoryItemButtonEditCFL();
			_rep_luk_WhsCodeEdit.ValueMember = "WhsCode";
			_rep_luk_WhsCodeEdit.DisplayMember = "WhsCode";
			_rep_luk_WhsCodeEdit.FindCFLBy(Enums.TypeCFL.GetListWareHouse, false, "", Enums.ItemObjectType.NoItemType, null);
			_rep_luk_WhsCodeEdit.ButtonPressed += _rep_luk_WhsCode_ButtonPressed;
			_rep_luk_Type = new RepositoryItemLookUpEdit();
			_rep_luk_Type.Name = "rep_luk_StatusFG";
			_rep_luk_Type.SetDataSource(new string[2, 2]
			{
				{ "S", "Standard" },
				{ "P", "Special" }
			}, isStringCode: true);
			if (!grdEx_FinishGoods.RepositoryItems.Contains(_rep_luk_Type))
			{
				grdEx_FinishGoods.RepositoryItems.Add(_rep_luk_Type);
			}
			_rep_luk_IssueMethodCP = new RepositoryItemLookUpEdit();
			_rep_luk_IssueMethodCP.Name = "_rep_luk_IssueMethodCP";
			_rep_luk_IssueMethodCP.SetDataSource(new string[2, 2]
			{
				{ "M", "Manual" },
				{ "B", "Backflush" }
			}, isStringCode: true);
			if (!grdEx_Components.RepositoryItems.Contains(_rep_luk_IssueMethodCP))
			{
				grdEx_Components.RepositoryItems.Add(_rep_luk_IssueMethodCP);
			}
			_rep_bte_DocNum = new RepositoryItemButtonEdit();
			_rep_bte_DocNum.Name = "_rep_bte_DocEntryComp";
			SerializableAppearanceObject appearance = new SerializableAppearanceObject();
			_rep_bte_DocNum.Buttons.Clear();
			_rep_bte_DocNum.Buttons.Add(new EditorButton(ButtonPredefines.Glyph, "", -1, enabled: true, visible: true, isLeft: true, ImageLocation.MiddleCenter, Resources.right, new KeyShortcut(Keys.None), appearance, "", null, null, enableImageTransparency: true));
			_rep_bte_DocNum.ButtonPressed += _rep_bte_DocEntryComp_ButtonPressed;
			bte_ToBin = new RepositoryItemButtonEditCFL();
			bte_ToBin.ValueMember = "AbsEntry";
			bte_ToBin.DisplayMember = "BinCode";
			bte_ToBin.ContainerControl = this;
			bte_ToBin.BeforeShowCFL = (frm_CFL frm) =>
			{
				frm.DataString1 = Function.ToString(gvEx_Transfer.GetFocusedRowCellValue("wareHouse"));
			};
			bte_ToBin.TypeCFL = Enums.TypeCFL.ListOfBinLocationByWareHouse;
			bte_ToBin.RefreshDataSource();
			gvEx_TransferBin.Columns["ToBin"].ColumnEdit = bte_ToBin;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void SetButtonTextToUpdate(SimpleButton btn)
	{
		try
		{
			if (!_isLoading && (string.Compare(btn_SaveFG.Text, "add", ignoreCase: true) != 0 || !(btn.Name == "btn_SaveFG")))
			{
				btn.Text = "Update";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private DataTable GetSearchDataTable()
	{
		try
		{
			DataTable dataTable = new DataTable
			{
				Columns = 
				{
					new DataColumn("Code", typeof(string)),
					new DataColumn("Name", typeof(string)),
					new DataColumn("Remarks", typeof(string)),
					new DataColumn("FromDate", typeof(string)),
					new DataColumn("ToDate", typeof(string)),
					new DataColumn("TabName", typeof(string)),
					new DataColumn("Branch", typeof(int))
				}
			};
			DataRow row = dataTable.NewRow();
			dataTable.Rows.Add(row);
			dataTable.AcceptChanges();
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return new DataTable();
		}
	}

	private void PasteFromExcelToDataTable(GridControl grd, GridViewEx gv, List<string> key)
	{
		try
		{
			DataObject dataObject = (DataObject)Clipboard.GetDataObject();
			if (dataObject == null)
			{
				return;
			}
			string clipboardData = dataObject.GetData(DataFormats.UnicodeText).ToString();
			if (!dataObject.GetDataPresent(DataFormats.Text))
			{
				return;
			}
			DataTable dataTable = Function.PasteFromExcel(clipboardData);
			if (dataTable == null || (dataTable.Rows.Count <= 0 && dataTable.Columns.Count <= 0))
			{
				return;
			}
			int num = gv.FocusedRowHandle;
			GridColumn focusedColumn = gv.FocusedColumn;
			if (key.Contains(focusedColumn.FieldName))
			{
				int focusedRowHandle = gv.FocusedRowHandle;
				for (int num2 = dataTable.Rows.Count - 1; num2 >= 0; num2--)
				{
					DataRow row = gv.GridControl.DataSourceTable().NewRow();
					gv.GridControl.DataSourceTable().Rows.InsertAt(row, focusedRowHandle);
					int colunmIndex = focusedColumn.VisibleIndex;
					string code = Function.ToString(dataTable.Rows[num2][0]);
					DataTable bOMHeader = _httpClient.ProductionOrderImportTool.GetBOMHeader(code);
					if (bOMHeader.IsNotNull())
					{
						DataRow dataRow = bOMHeader.Rows[0];
						gv.SetRowCellValue(num, "ItemName", Function.ToString(dataRow["ItemName"]));
						gv.SetRowCellValue(num, "Warehouse", Function.ToString(dataRow["ToWH"]));
						gv.SetRowCellValue(num, "PlannedQty", Function.ParseDouble(dataRow["Qauntity"]));
						gv.SetRowCellValue(num, "Status", "R");
						gv.SetRowCellValue(num, "DocEntry", 0);
					}
					for (int i = 0; i < dataTable.Columns.Count; i++)
					{
						gv.FocusedRowHandle = focusedRowHandle;
						GridColumn gridColumn = gv.Columns.FirstOrDefault((GridColumn t) => t.VisibleIndex == colunmIndex);
						if (gridColumn == null)
						{
							break;
						}
						gv.SetRowCellValue(num, gridColumn, dataTable.Rows[num2][i]);
						colunmIndex++;
					}
					gv.GridControl.RefreshDataSource();
				}
			}
			else
			{
				int focusedRowHandle2 = gv.FocusedRowHandle;
				for (int num3 = focusedRowHandle2; num3 < gv.RowCount; num3++)
				{
					int colIndex = focusedColumn.VisibleIndex;
					bool flag = true;
					for (int num4 = 0; num4 < key.Count; num4++)
					{
						if (string.IsNullOrEmpty(Function.ToString(gv.GetRowCellValue(num3, key[num4]))))
						{
							flag = false;
							break;
						}
					}
					if (!flag || num3 - focusedRowHandle2 >= dataTable.Rows.Count)
					{
						break;
					}
					for (int num5 = 0; num5 < dataTable.Columns.Count; num5++)
					{
						GridColumn gridColumn2 = gv.Columns.FirstOrDefault((GridColumn t) => t.VisibleIndex == colIndex);
						if (gridColumn2 == null)
						{
							break;
						}
						gv.SetRowCellValue(num, gridColumn2, dataTable.Rows[num3 - focusedRowHandle2][num5]);
						colIndex++;
					}
					num++;
				}
				gv.GridControl.RefreshDataSource();
			}
			Clipboard.Clear();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
	}

	private Tuple<string, DataTable> GetDataFromExcelFileFG(string path, bool withSerial = false)
	{
		try
		{
			DataTable dataTableFromExcel = ExcelHelper.GetDataTableFromExcel(path, hasHeader: false, _startDataRow);
			if (dataTableFromExcel != null && dataTableFromExcel.Rows.Count > 0)
			{
				dataTableFromExcel.TableName = "Data";
				DataTable dataTable = _dtStructFG.Copy();
				dataTable.Columns.Add("StPostDate", typeof(string));
				dataTable.Columns.Add("StDueDate", typeof(string));
				for (int i = 0; i < dataTableFromExcel.Rows.Count; i++)
				{
					string text = Function.ToString(dataTableFromExcel.Rows[i][2]);
					if (!string.IsNullOrWhiteSpace(text))
					{
						DataRow dataRow = dataTable.NewRow();
						dataRow["DocEntry"] = 0;
						dataRow["Status"] = "R";
						dataRow["Type"] = "S";
						DataTable bOMHeader = _httpClient.ProductionOrderImportTool.GetBOMHeader(text);
						if (bOMHeader.IsNotNull())
						{
							dataRow["Warehouse"] = Function.ToString(bOMHeader.Rows[0]["ToWH"]);
						}
						dataRow["U_PlnCode"] = Function.ToString(dataTableFromExcel.Rows[i][0]);
						dataRow["U_LotNo"] = Function.ToString(dataTableFromExcel.Rows[i][1]);
						dataRow["ItemCode"] = text;
						dataRow["ItemName"] = Function.ToString(dataTableFromExcel.Rows[i][3]);
						dataRow["PlannedQty"] = Function.ParseDouble(dataTableFromExcel.Rows[i][4]);
						dataRow["StPostDate"] = Function.ToString(dataTableFromExcel.Rows[i][5]).Replace("-", "");
						dataRow["StDueDate"] = Function.ToString(dataTableFromExcel.Rows[i][6]).Replace("-", "");
						if (withSerial)
						{
							dataRow["PickRmrk"] = Function.ToString(dataTableFromExcel.Rows[i][7]);
						}
						dataTable.Rows.Add(dataRow);
					}
				}
				dataTable.AcceptChanges();
				return new Tuple<string, DataTable>("", dataTable);
			}
			return new Tuple<string, DataTable>("File contains no data", new DataTable());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new Tuple<string, DataTable>("Error reading excel file: " + ex.Message, new DataTable());
		}
	}

	private string GetDataFromExcelFileFGSerial(string path, bool withSerial = false)
	{
		_dsImportFgWithSerial.Tables.Clear();
		try
		{
			DataTable dataTableFromExcel = ExcelHelper.GetDataTableFromExcel(path, hasHeader: false, _startDataRow);
			if (dataTableFromExcel != null && dataTableFromExcel.Rows.Count > 0)
			{
				dataTableFromExcel.TableName = "Data";
				DataTable dataTable = _dtStructFG.Copy();
				for (int i = 0; i < dataTableFromExcel.Rows.Count; i++)
				{
					string value = Function.ToString(dataTableFromExcel.Rows[i][2]);
					DataRow dataRow = dataTable.NewRow();
					dataRow["U_PlnCode"] = Function.ToString(dataTableFromExcel.Rows[i][0]);
					dataRow["U_LotNo"] = Function.ToString(dataTableFromExcel.Rows[i][1]);
					dataRow["ItemCode"] = value;
					dataRow["ItemName"] = Function.ToString(dataTableFromExcel.Rows[i][3]);
					dataRow["PlannedQty"] = Function.ParseDouble(dataTableFromExcel.Rows[i][4]);
					DateTime? dateTime = Function.ParseDateTime(dataTableFromExcel.Rows[i][5], "yyyy-MM-dd");
					DateTime dateTime2;
					if (dateTime.HasValue)
					{
						_ = dateTime.Value;
						dateTime2 = dateTime.Value;
					}
					else
					{
						dateTime2 = new DateTime(1900, 1, 1);
					}
					dataRow["PostDate"] = dateTime2;
					DateTime? dateTime3 = Function.ParseDateTime(dataTableFromExcel.Rows[i][6], "yyyy-MM-dd");
					DateTime dateTime4;
					if (dateTime3.HasValue)
					{
						_ = dateTime3.Value;
						dateTime4 = dateTime3.Value;
					}
					else
					{
						dateTime4 = new DateTime(1900, 1, 1);
					}
					dataRow["DueDate"] = dateTime4;
					dataRow["PickRmrk"] = Function.ToString(dataTableFromExcel.Rows[i][7]);
					dataTable.Rows.Add(dataRow);
				}
				dataTable.AcceptChanges();
				DataTable dtSave = new DataTable();
				dtSave.Columns.Add("U_PlnCode", typeof(string));
				dtSave.Columns.Add("U_LotNo", typeof(string));
				dtSave.Columns.Add("ItemCode", typeof(string));
				dtSave.Columns.Add("ItemName", typeof(string));
				dtSave.Columns.Add("PostDate", typeof(DateTime));
				dtSave.Columns.Add("DueDate", typeof(DateTime));
				dtSave.Columns.Add("ID", typeof(int));
				dtSave.AcceptChanges();
				DataTable dataTable2 = new DataTable();
				dataTable2.Columns.Add("ID", typeof(int));
				dataTable2.Columns.Add("Serial", typeof(string));
				dataTable2.AcceptChanges();
				var source = from c in dataTable.AsEnumerable()
					group c by new
					{
						U_PlnCode = c["U_PlnCode"],
						U_LotNo = c["U_LotNo"],
						ItemCode = c["ItemCode"],
						ItemName = c["ItemName"],
						PostDate = c["PostDate"],
						DueDate = c["DueDate"]
					} into gcs
					select new
					{
						gcs.Key.U_PlnCode,
						gcs.Key.U_LotNo,
						gcs.Key.ItemCode,
						gcs.Key.ItemName,
						gcs.Key.PostDate,
						gcs.Key.DueDate
					};
				if (source.Any())
				{
					var lst = source.ToList();
					int i2;
					for (i2 = 0; i2 < lst.Count; i2++)
					{
						DataRow dataRow2 = dtSave.NewRow();
						dataRow2["U_PlnCode"] = lst[i2].U_PlnCode;
						dataRow2["U_LotNo"] = lst[i2].U_LotNo;
						dataRow2["ItemCode"] = lst[i2].ItemCode;
						dataRow2["ItemName"] = lst[i2].ItemName;
						dataRow2["PostDate"] = lst[i2].PostDate;
						dataRow2["DueDate"] = lst[i2].DueDate;
						dataRow2["ID"] = i2;
						dtSave.Rows.Add(dataRow2);
						dtSave.AcceptChanges();
						List<DataRow> list = (from r in dataTable.AsEnumerable()
							where Function.ToString(r["U_PlnCode"]).ToLower() == Function.ToString(lst[i2].U_PlnCode).ToLower() && Function.ToString(r["U_LotNo"]).ToLower() == Function.ToString(lst[i2].U_LotNo).ToLower() && Function.ToString(r["ItemCode"]).ToLower() == Function.ToString(lst[i2].ItemCode).ToLower() && Function.ToString(r["ItemName"]).ToLower() == Function.ToString(lst[i2].ItemName).ToLower() && Function.ToString(r["PostDate"]) == Function.ToString(lst[i2].PostDate) && Function.ToString(r["DueDate"]) == Function.ToString(lst[i2].DueDate)
							select r).ToList();
						if (list.Count <= 0)
						{
							continue;
						}
						foreach (DataRow item in list)
						{
							DataRow dataRow3 = dataTable2.NewRow();
							dataRow3["ID"] = i2;
							dataRow3["Serial"] = Function.ToString(item["PickRmrk"]);
							dataTable2.Rows.Add(dataRow3);
							dataTable2.AcceptChanges();
						}
					}
				}
				DataTable dataTable3 = dataTable.Clone();
				int i3;
				for (i3 = 0; i3 < dtSave.Rows.Count; i3++)
				{
					DataRow dataRow4 = dataTable3.NewRow();
					dataRow4["DocEntry"] = Function.ParseInt(dtSave.Rows[i3]["ID"]);
					dataRow4["Status"] = "P";
					dataRow4["Type"] = "S";
					dataRow4["U_PlnCode"] = dtSave.Rows[i3]["U_PlnCode"];
					dataRow4["U_LotNo"] = dtSave.Rows[i3]["U_LotNo"];
					dataRow4["ItemCode"] = dtSave.Rows[i3]["ItemCode"];
					dataRow4["ItemName"] = dtSave.Rows[i3]["ItemName"];
					dataRow4["PostDate"] = dtSave.Rows[i3]["PostDate"];
					dataRow4["DueDate"] = dtSave.Rows[i3]["DueDate"];
					dataRow4["PlannedQty"] = dataTable2.AsEnumerable().Count((DataRow x) => Function.ParseInt(x["ID"]) == Function.ParseInt(dtSave.Rows[i3]["ID"]));
					DataTable bOMHeader = _httpClient.ProductionOrderImportTool.GetBOMHeader(Function.ToString(dataRow4["ItemCode"]));
					if (bOMHeader.IsNotNull())
					{
						dataRow4["Warehouse"] = Function.ToString(bOMHeader.Rows[0]["ToWH"]);
					}
					dataTable3.Rows.Add(dataRow4);
				}
				dataTable3.AcceptChanges();
				_dsImportFgWithSerial.Tables.AddRange(new DataTable[2] { dataTable3, dataTable2 });
				return "";
			}
			return "File contains no data";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return ex.Message;
		}
	}

	private void EnableDisableButton(bool isEnable)
	{
		try
		{
			btn_SaveFG.Enabled = isEnable;
			btn_SaveComp.Enabled = isEnable;
			btn_SaveTransfer.Enabled = isEnable;
			btn_SaveRC.Enabled = isEnable;
			btn_SaveClose.Enabled = isEnable;
			btn_UpdateTime.Enabled = isEnable;
			btn_Barcode.Enabled = isEnable;
			txt_Code.Enabled = isEnable;
			btn_ImportToDb.Enabled = isEnable;
			btn_ImportFG.Enabled = isEnable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void PatchSelection(GridView view)
	{
		try
		{
			int[] selectedRows = view.GetSelectedRows();
			for (int i = 0; i < selectedRows.Length; i++)
			{
				string text = Function.ToString(view.GetRowCellValue(selectedRows[i], "Status"));
				if (text == "C" || text == "L")
				{
					view.UnselectRow(selectedRows[i]);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void SetBinBatchDataTransfer(int rowHandle)
	{
		if (_isSetBinBatchData)
		{
			return;
		}
		_isSetBinBatchData = true;
		try
		{
			int num = Function.ParseInt(gvEx_Transfer.GetRowCellValue(rowHandle, "LineNum"));
			string text = Function.ToString(gvEx_Transfer.GetRowCellValue(rowHandle, "DfltWH"));
			string text2 = Function.ToString(gvEx_Transfer.GetRowCellValue(rowHandle, "wareHouse"));
			string text3 = Function.ToString(gvEx_Transfer.GetRowCellValue(rowHandle, "ManBatchSer"));
			DataRow[] array = _DTBinDataTransfer.Select("LineNum=" + num);
			bool flag = _binWarehouse.Contains(text.ToLower());
			bool flag2 = _binWarehouse.Contains(text2.ToLower());
			if (((flag2 | flag) || text3.ToLower() == "b" || text3.ToLower() == "s") && grdEx_TransferBin.DataSource is DataTable dataTable && dataTable.Rows.Count != 0)
			{
				if (array != null && array.Length != 0)
				{
					for (int i = 0; i < array.Length; i++)
					{
						_DTBinDataTransfer.Rows.Remove(array[i]);
					}
					_DTBinDataTransfer.AcceptChanges();
				}
				for (int j = 0; j < dataTable.Rows.Count; j++)
				{
					if (text3 == "B" || text3 == "S")
					{
						if (!string.IsNullOrWhiteSpace(Function.ToString(dataTable.Rows[j]["DistNumber"])))
						{
							_DTBinDataTransfer.ImportRow(dataTable.Rows[j]);
						}
					}
					else if (flag)
					{
						if (Function.ParseInt(dataTable.Rows[j]["FromBin"]) > 0)
						{
							_DTBinDataTransfer.ImportRow(dataTable.Rows[j]);
						}
					}
					else if (flag2 && Function.ParseInt(dataTable.Rows[j]["ToBin"]) > 0)
					{
						_DTBinDataTransfer.ImportRow(dataTable.Rows[j]);
					}
				}
				_DTBinDataTransfer.AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		_isSetBinBatchData = false;
	}

	private async Task ReSetBinBatchDataTransfer(int rowHandle)
	{
		if (_isSetBinBatchData)
		{
			return;
		}
		_isSetBinBatchData = true;
		try
		{
			string text = Function.ToString(gvEx_Transfer.GetRowCellValue(rowHandle, "DfltWH"));
			string text2 = Function.ToString(gvEx_Transfer.GetRowCellValue(rowHandle, "wareHouse"));
			bool flag = _binWarehouse.Contains(text.ToLower());
			bool flag2 = _binWarehouse.Contains(text2.ToLower());
			string text3 = Function.ToString(gvEx_Transfer.GetRowCellValue(rowHandle, "ManBatchSer"));
			if ((flag2 | flag) || text3.ToLower() == "b" || text3.ToLower() == "s")
			{
				int num = Function.ParseInt(gvEx_Transfer.GetRowCellValue(rowHandle, "LineNum"));
				DataRow[] array = _DTBinDataTransfer.Select("LineNum=" + num);
				if (array != null && array.Length != 0)
				{
					for (int i = 0; i < array.Length; i++)
					{
						_DTBinDataTransfer.Rows.Remove(array[i]);
					}
					_DTBinDataTransfer.AcceptChanges();
				}
				if (rowHandle == gvEx_Transfer.FocusedRowHandle)
				{
					await ShowBinBatchDataTransfer();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		_isSetBinBatchData = false;
	}

	private async Task ShowBinBatchDataTransfer()
	{
		try
		{
			int docEntry = Function.ParseInt(gvEx_Transfer.GetFocusedRowCellValue("DocEntry"));
			int lineNum = Function.ParseInt(gvEx_Transfer.GetFocusedRowCellValue("LineNum"));
			string itemCode = Function.ToString(gvEx_Transfer.GetFocusedRowCellValue("ItemCode"));
			string text = Function.ToString(gvEx_Transfer.GetFocusedRowCellValue("DfltWH"));
			string text2 = Function.ToString(gvEx_Transfer.GetFocusedRowCellValue("wareHouse"));
			string text3 = Function.ToString(gvEx_Transfer.GetFocusedRowCellValue("ManBatchSer"));
			bool flag = _binWarehouse.Contains(text.ToLower());
			bool flag2 = _binWarehouse.Contains(text2.ToLower());
			if (((text3 == "B" || text3 == "S") | flag | flag2) && string.Compare(btn_SaveTransfer.Text, "confirm", ignoreCase: true) == 0)
			{
				splitContainerControl1.PanelVisibility = SplitPanelVisibility.Both;
				DataTable dataTable = _DTBinDataTransfer.Clone();
				DataRow[] array = _DTBinDataTransfer.Select("LineNum=" + lineNum);
				if (array == null || array.Length == 0)
				{
					await AddNewBinBatchTransferRow(docEntry, lineNum, itemCode, text, text2);
					return;
				}
				for (int i = 0; i < array.Length; i++)
				{
					dataTable.ImportRow(array[i]);
					dataTable.AcceptChanges();
				}
				grdEx_TransferBin.DataSource = dataTable;
				grdEx_TransferBin.RefreshDataSource();
			}
			else
			{
				splitContainerControl1.PanelVisibility = SplitPanelVisibility.Panel1;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task AddNewBinBatchTransferRow(int docEntry, int lineNum, string itemCode, string fromWhsCode, string toWhsCode)
	{
		try
		{
			DataTable dataTable = await _httpClient.ProductionOrderImportTool.GetAllBatchSerial(itemCode, fromWhsCode, toWhsCode);
			bool flag = _binWarehouse.Contains(toWhsCode.ToLower());
			bool flag2 = _binWarehouse.Contains(fromWhsCode.ToLower());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				if (_DTBinDataTransfer == null || _DTBinDataTransfer.Columns.Count == 0)
				{
					_DTBinDataTransfer = dataTable.Clone();
				}
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					dataTable.Rows[i]["DocEntry"] = docEntry;
					dataTable.Rows[i]["LineNum"] = lineNum;
					dataTable.Rows[i]["ToBinActivat"] = (flag ? "Y" : "N");
					dataTable.Rows[i]["FromBinActivat"] = (flag2 ? "Y" : "N");
					dataTable.AcceptChanges();
					_DTBinDataTransfer.ImportRow(dataTable.Rows[i]);
				}
			}
			_DTBinDataTransfer.AcceptChanges();
			grdEx_TransferBin.DataSource = dataTable;
			grdEx_TransferBin.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void SaveFG()
	{
		_fgPlnCode = "";
		try
		{
			List<Tuple<int, string>> list = new List<Tuple<int, string>>();
			DataTable dataTable = grdEx_FinishGoods.DataSource as DataTable;
			DataTable dataTable2 = dataTable.Clone();
			int[] selectedRows = gvEx_FinishGoods.GetSelectedRows();
			if (selectedRows.Length != 0)
			{
				for (int i = 0; i < selectedRows.Length; i++)
				{
					dataTable2.ImportRow(dataTable.Rows[gvEx_FinishGoods.GetDataSourceRowIndex(selectedRows[i])]);
				}
			}
			dataTable2.AcceptChanges();
			if (dataTable2.Columns.Contains("U_ProductLine"))
			{
				dataTable2.Columns.Remove("U_ProductLine");
			}
			dataTable2.AcceptChanges();
			if (dataTable2.Columns.Contains("U_PlnCode"))
			{
				_fgPlnCode = Function.ToString(dataTable2.Rows[0]["U_PlnCode"]);
			}
			List<List<DataRow>> list2 = new List<List<DataRow>>();
			int num = 0;
			while (num < dataTable2.Rows.Count)
			{
				List<DataRow> list3 = new List<DataRow>();
				for (int j = 0; j < _packQty; j++)
				{
					if (num < dataTable2.Rows.Count)
					{
						list3.Add(dataTable2.Rows[num]);
						num++;
					}
				}
				list2.Add(list3);
			}
			for (int k = 0; k < list2.Count; k++)
			{
				DataTable dtTmp = list2[k].CopyToDataTable();
				HttpResult result = Task.Run(async () => await _httpClient.ProductionOrderImportTool.SaveFG(dtTmp)).Result;
				if (!string.IsNullOrWhiteSpace(result.Message))
				{
					string text = "";
					string text2 = "";
					if (dataTable2.Columns.Contains("U_LotNo"))
					{
						for (int num2 = 0; num2 < list2[k].Count; num2++)
						{
							text = text + Function.ToString(list2[k][num2]["U_LotNo"]) + ",";
						}
					}
					if (text.EndsWith(","))
					{
						text = text.Remove(text.Length - 1);
					}
					if (dataTable2.Columns.Contains("ItemCode"))
					{
						for (int num3 = 0; num3 < list2[k].Count; num3++)
						{
							text2 = text2 + Function.ToString(list2[k][num3]["ItemCode"]) + ",";
						}
					}
					if (text2.EndsWith(","))
					{
						text2 = text2.Remove(text2.Length - 1);
					}
					_imporFgErrors.Add(new Tuple<string, string, string>(text2, text, result.Message));
				}
				if (result.Content is string text3)
				{
					string[] array = text3.Split(';');
					for (int num4 = 0; num4 < array.Length; num4++)
					{
						string[] array2 = array[num4].Split(',');
						if (array2.Length == 2)
						{
							int item = Function.ParseInt(array2[0]);
							list.Add(new Tuple<int, string>(item, array2[1]));
						}
					}
				}
				bgw_FG.ReportProgress(1);
				if (k != list2.Count - 1)
				{
					Thread.Sleep(_sleepTime * 1000);
				}
			}
			if (list.Count <= 0)
			{
				return;
			}
			bgw_FG.ReportProgress(2, list.Count);
			List<List<ProductionOder_Impl_OWOR>> lstPack1 = new List<List<ProductionOder_Impl_OWOR>>();
			int num5 = 0;
			while (num5 < list.Count)
			{
				List<ProductionOder_Impl_OWOR> list4 = new List<ProductionOder_Impl_OWOR>();
				for (int num6 = 0; num6 < _packQty; num6++)
				{
					if (num5 >= list.Count)
					{
						continue;
					}
					ProductionOder_Impl_OWOR productionOder_Impl_OWOR = new ProductionOder_Impl_OWOR();
					productionOder_Impl_OWOR.DocEntry = list[num5].Item1;
					if (list[num5].Item2 != "nulll")
					{
						string text4 = "";
						string value = "";
						string value2 = "";
						try
						{
							text4 = list[num5].Item2.Substring(0, 4);
							value = list[num5].Item2.Substring(4, 2);
							value2 = list[num5].Item2.Substring(6, 2);
						}
						catch (Exception)
						{
							text4 = "";
						}
						if (text4 != "")
						{
							productionOder_Impl_OWOR.CloseDate = new DateTime(Convert.ToInt32(text4), Convert.ToInt32(value), Convert.ToInt32(value2));
						}
					}
					productionOder_Impl_OWOR.Status = "R";
					list4.Add(productionOder_Impl_OWOR);
					num5++;
				}
				lstPack1.Add(list4);
			}
			int i2;
			for (i2 = 0; i2 < lstPack1.Count; i2++)
			{
				for (int num7 = 1; num7 < 10; num7++)
				{
					if (string.IsNullOrEmpty(Task.Run(async () => await _httpClient.ProductionOrders.UpdateStatus(lstPack1[i2])).Result.Message))
					{
						break;
					}
				}
				if (i2 != lstPack1.Count - 1)
				{
					Thread.Sleep(_sleepTime * 1000);
				}
			}
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", ex2);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex2.Message);
		}
	}

	private async Task SaveFGSerial()
	{
		try
		{
			string text = await _httpClient.ProductionOrderImportTool.SaveFGSerial(_dsImportFgWithSerial);
			if (!string.IsNullOrEmpty(text))
			{
				WinCommons.Error(Enums.MessageCode.None, text);
				return;
			}
			WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
			_isLoading = true;
			MODE_FIND();
			btn_SaveFG_Click(null, null);
			_isLoading = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private bool ValidateData(DataTable dt, string tab = "")
	{
		try
		{
			if (dt == null || dt.Rows.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.NoDataToSave, "");
				return false;
			}
			return true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return false;
		}
	}

	private async Task SaveTransfer()
	{
		try
		{
			string currFilterString = gvEx_Transfer.ActiveFilterString;
			SetBinBatchDataTransfer(gvEx_Transfer.FocusedRowHandle);
			DataTable dataTable = _DTBinDataTransfer.Copy();
			for (int num = dataTable.Rows.Count - 1; num >= 0; num--)
			{
				DataRow dataRow = dataTable.Rows[num];
				if (Function.ParseInt(dataRow["DocEntry"]) <= 0 || Function.ParseDouble(dataRow["Quantity"]) <= 0.0 || (Function.ParseInt(dataRow["FromBin"]) <= 0 && Function.ToString(dataRow["FromBinActivat"]) == "Y") || (Function.ParseInt(dataRow["ToBin"]) <= 0 && Function.ToString(dataRow["ToBinActivat"]) == "Y"))
				{
					dataTable.Rows.Remove(dataRow);
				}
			}
			DataTable dataTable2 = grdEx_Transfer.DataSource as DataTable;
			int[] selectedRows = gvEx_Transfer.GetSelectedRows();
			DataTable dataTable3 = new DataTable();
			if (dataTable2 != null)
			{
				dataTable3 = dataTable2.Clone();
				for (int i = 0; i < selectedRows.Length; i++)
				{
					DataRow row = dataTable2.Rows[gvEx_Transfer.GetDataSourceRowIndex(selectedRows[i])];
					dataTable3.ImportRow(row);
				}
			}
			dataTable3.AcceptChanges();
			if (!ValidateData(dataTable3, "TR") || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
			{
				return;
			}
			DocumentsImpl documentsImpl = new DocumentsImpl();
			documentsImpl.DocEntry = 0;
			DateTime? dateTime = Function.ParseDateTime(date_InvTransferPostDate.EditValue);
			DateTime docDate;
			if (dateTime.HasValue)
			{
				_ = dateTime.Value;
				docDate = dateTime.Value;
			}
			else
			{
				docDate = DateTime.Now;
			}
			documentsImpl.DocDate = docDate;
			DataTable tableStructure = WinFuntions.GetTableStructure("WTR1");
			List<DocumentLinesBinAllocationsImpl> list = new List<DocumentLinesBinAllocationsImpl>();
			List<Tuple<List<BatchNumbersImpl>, int>> list2 = new List<Tuple<List<BatchNumbersImpl>, int>>();
			List<Tuple<List<SerialNumbersImpl>, int>> list3 = new List<Tuple<List<SerialNumbersImpl>, int>>();
			for (int j = 0; j < dataTable3.Rows.Count; j++)
			{
				DataRow dataRow2 = dataTable3.Rows[j];
				DataRow dataRow3 = tableStructure.NewRow();
				dataRow3["DocEntry"] = 0;
				dataRow3["ItemCode"] = Function.ToString(dataRow2["ItemCode"]);
				dataRow3["LineNum"] = j;
				dataRow3["LineStatus"] = "O";
				dataRow3["Dscription"] = Function.ToString(dataRow2["ItemName"]);
				dataRow3["Quantity"] = Function.ParseDouble(dataRow2["RequestQty"]);
				dataRow3["FromWhsCod"] = Function.ToString(dataRow2["DfltWH"]);
				dataRow3["WhsCode"] = Function.ToString(dataRow2["wareHouse"]);
				dataRow3["AcctCode"] = "";
				dataRow3["U_PlnCode"] = Function.ToString(dataRow2["U_PlnCode"]);
				dataRow3["U_LotNo"] = Function.ToString(dataRow2["U_LotNo"]);
				dataRow3["U_ProductionLine"] = Function.ToString(dataRow2["U_ProductionLine"]);
				dataRow3["U_SKUCode"] = Function.ToString(dataRow2["DocItemCode"]);
				string text = Function.ToString(dataRow2["ManBatchSer"]);
				if (text != "N" && dataTable != null && dataTable.Rows.Count > 0)
				{
					List<Tuple<string, double>> list4 = new List<Tuple<string, double>>();
					List<Tuple<string, double>> list5 = new List<Tuple<string, double>>();
					int num2 = Function.ParseInt(dataRow2["LineNum"]);
					int num3 = Function.ParseInt(dataRow2["DocEntry"]);
					DataRow[] binRows = dataTable.Select("LineNum=" + num2 + " And DocEntry=" + num3);
					if (binRows != null && binRows.Length != 0)
					{
						int k;
						for (k = 0; k < binRows.Length; k++)
						{
							double num4 = Function.ParseDouble(binRows[k]["Quantity"]);
							if (text == "B")
							{
								Tuple<string, double> tuple = list4.FirstOrDefault((Tuple<string, double> b) => b.Item1 == Function.ToString(binRows[k]["DistNumber"]));
								if (tuple == null)
								{
									list4.Add(new Tuple<string, double>(Function.ToString(binRows[k]["DistNumber"]), num4));
								}
								else
								{
									double item = tuple.Item2 + num4;
									Tuple<string, double> item2 = new Tuple<string, double>(tuple.Item1, item);
									list4.Remove(tuple);
									list4.Add(item2);
								}
							}
							if (text == "S")
							{
								Tuple<string, double> tuple2 = list5.FirstOrDefault((Tuple<string, double> b) => b.Item1 == Function.ToString(binRows[k]["DistNumber"]));
								if (tuple2 == null)
								{
									list5.Add(new Tuple<string, double>(Function.ToString(binRows[k]["DistNumber"]), num2));
									continue;
								}
								double item3 = tuple2.Item2 + num4;
								Tuple<string, double> item4 = new Tuple<string, double>(tuple2.Item1, item3);
								list5.Remove(tuple2);
								list5.Add(item4);
							}
						}
					}
					List<BatchNumbersImpl> list6 = new List<BatchNumbersImpl>();
					for (int num5 = 0; num5 < list4.Count; num5++)
					{
						BatchNumbersImpl batchNumbersImpl = new BatchNumbersImpl();
						batchNumbersImpl.AddmisionDate = DateTime.Today;
						batchNumbersImpl.BatchNumber = list4[num5].Item1;
						batchNumbersImpl.BaseLineNumber = j;
						batchNumbersImpl.Quantity = list4[num5].Item2;
						list6.Add(batchNumbersImpl);
					}
					if (list6.Count > 0)
					{
						list2.Add(new Tuple<List<BatchNumbersImpl>, int>(list6, j));
					}
					List<SerialNumbersImpl> list7 = new List<SerialNumbersImpl>();
					for (int num6 = 0; num6 < list5.Count; num6++)
					{
						SerialNumbersImpl serialNumbersImpl = new SerialNumbersImpl();
						serialNumbersImpl.InternalSerialNumber = list5[num6].Item1;
						serialNumbersImpl.BaseLineNumber = j;
						serialNumbersImpl.Quantity = 1.0;
						list7.Add(serialNumbersImpl);
					}
					if (list7.Count > 0)
					{
						list3.Add(new Tuple<List<SerialNumbersImpl>, int>(list7, j));
					}
					if (binRows != null && binRows.Length != 0)
					{
						for (int num7 = 0; num7 < binRows.Length; num7++)
						{
							string distNumber = Function.ToString(binRows[num7]["DistNumber"]);
							int num8 = -1;
							if (text == "B")
							{
								num8 = list6.FindIndex((BatchNumbersImpl b) => b.BatchNumber == distNumber);
							}
							if (text == "S")
							{
								num8 = list7.FindIndex((SerialNumbersImpl b) => b.InternalSerialNumber == distNumber);
							}
							int num9 = Function.ParseInt(binRows[num7]["Quantity"]);
							int binAbsEntry = Function.ParseInt(binRows[num7]["FromBin"]);
							DocumentLinesBinAllocationsImpl documentLinesBinAllocationsImpl = new DocumentLinesBinAllocationsImpl();
							documentLinesBinAllocationsImpl.BaseLineNumber = j;
							documentLinesBinAllocationsImpl.BinAbsEntry = binAbsEntry;
							documentLinesBinAllocationsImpl.BinActionType = BinActionTypeEnum.batFromWarehouse;
							documentLinesBinAllocationsImpl.Quantity = num9;
							if (num8 >= 0)
							{
								documentLinesBinAllocationsImpl.SerialAndBatchNumbersBaseLine = num8;
							}
							if (documentLinesBinAllocationsImpl.BinAbsEntry > 0)
							{
								list.Add(documentLinesBinAllocationsImpl);
							}
							int binAbsEntry2 = Function.ParseInt(binRows[num7]["ToBin"]);
							DocumentLinesBinAllocationsImpl documentLinesBinAllocationsImpl2 = new DocumentLinesBinAllocationsImpl();
							documentLinesBinAllocationsImpl2.BaseLineNumber = j;
							documentLinesBinAllocationsImpl2.BinAbsEntry = binAbsEntry2;
							documentLinesBinAllocationsImpl2.BinActionType = BinActionTypeEnum.batToWarehouse;
							documentLinesBinAllocationsImpl2.Quantity = num9;
							if (num8 >= 0)
							{
								documentLinesBinAllocationsImpl2.SerialAndBatchNumbersBaseLine = num8;
							}
							if (documentLinesBinAllocationsImpl2.BinAbsEntry > 0)
							{
								list.Add(documentLinesBinAllocationsImpl2);
							}
						}
					}
				}
				tableStructure.Rows.Add(dataRow3);
			}
			documentsImpl.BinAllocations = list;
			List<Document_LinesImpl> list8 = tableStructure.ToDocumentLine();
			int i2;
			for (i2 = 0; i2 < list8.Count; i2++)
			{
				Tuple<List<BatchNumbersImpl>, int> tuple3 = list2.FirstOrDefault((Tuple<List<BatchNumbersImpl>, int> l) => l.Item2 == i2);
				if (tuple3 != null)
				{
					list8[i2].BatchNumbers = tuple3.Item1;
				}
				Tuple<List<SerialNumbersImpl>, int> tuple4 = list3.FirstOrDefault((Tuple<List<SerialNumbersImpl>, int> l) => l.Item2 == i2);
				if (tuple4 != null)
				{
					list8[i2].SerialNumbers = tuple4.Item1;
				}
			}
			documentsImpl.Lines = list8;
			string text2 = await _httpClient.ProductionOrderImportTool.SaveTransfer(documentsImpl);
			if (!string.IsNullOrEmpty(text2))
			{
				WinCommons.Error(Enums.MessageCode.None, text2);
				return;
			}
			WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
			_isLoading = true;
			MODE_FIND();
			btn_SaveTransfer_Click(null, null);
			gvEx_Transfer.ActiveFilterString = currFilterString;
			_isLoading = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task SaveRC()
	{
		try
		{
			DataTable dataTable = grdEx_RC.DataSource as DataTable;
			int[] selectedRows = gvEx_RC.GetSelectedRows();
			DataTable dataTable2 = new DataTable();
			if (dataTable != null)
			{
				dataTable2 = dataTable.Clone();
				for (int i = 0; i < selectedRows.Length; i++)
				{
					DataRow row = dataTable.Rows[gvEx_RC.GetDataSourceRowIndex(selectedRows[i])];
					dataTable2.ImportRow(row);
				}
			}
			dataTable2.AcceptChanges();
			if (!ValidateData(dataTable2))
			{
				return;
			}
			DateTime? dateTime = Function.ParseDateTime(date_RcPostDate.EditValue);
			for (int j = 0; j < dataTable2.Rows.Count; j++)
			{
				dataTable2.Rows[j]["PostingDate"] = dateTime;
			}
			dataTable2.AcceptChanges();
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) != DialogResult.No)
			{
				new Tuple<DataTable>(dataTable2);
				string text = await _httpClient.ProductionOrderImportTool.SaveRCFromPC(dataTable2);
				if (!string.IsNullOrEmpty(text))
				{
					WinCommons.Error(Enums.MessageCode.None, text);
					return;
				}
				WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
				_isLoading = true;
				MODE_FIND();
				btn_SaveRC_Click(null, null);
				_isLoading = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task SaveClose()
	{
		try
		{
			DataTable dataTable = grdEx_Close.DataSource as DataTable;
			int[] selectedRows = gvEx_Close.GetSelectedRows();
			DataTable dataTable2 = new DataTable();
			if (dataTable != null)
			{
				dataTable2 = dataTable.Clone();
				for (int i = 0; i < selectedRows.Length; i++)
				{
					DataRow row = dataTable.Rows[gvEx_Close.GetDataSourceRowIndex(selectedRows[i])];
					dataTable2.ImportRow(row);
				}
			}
			dataTable2.AcceptChanges();
			if (!ValidateData(dataTable2) || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
			{
				return;
			}
			for (int j = 0; j < dataTable2.Rows.Count; j++)
			{
				DateTime? dateTime = Function.ParseDateTime(dataTable2.Rows[j]["CloseDate"]);
				DataRow dataRow = dataTable2.Rows[j];
				DateTime dateTime2;
				if (dateTime.HasValue)
				{
					_ = dateTime.Value;
					dateTime2 = dateTime.Value;
				}
				else
				{
					dateTime2 = DateTime.Today;
				}
				dataRow["CloseDate"] = dateTime2;
			}
			dataTable2.AcceptChanges();
			new Tuple<DataTable>(dataTable2);
			string text = await _httpClient.ProductionOrderImportTool.SaveClose(dataTable2);
			if (!string.IsNullOrEmpty(text))
			{
				WinCommons.Error(Enums.MessageCode.None, text);
				return;
			}
			WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
			_isLoading = true;
			MODE_FIND();
			btn_SaveClose_Click(null, null);
			_isLoading = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ShowErrorResultForm(List<Tuple<string, string, string>> errors)
	{
		try
		{
			XtraForm frm = new XtraForm();
			try
			{
				frm.Text = "Import Result";
				frm.FormBorderStyle = FormBorderStyle.FixedSingle;
				frm.MaximizeBox = false;
				frm.Width = 410;
				frm.Height = 300;
				TextBox textBox = new TextBox();
				textBox.Multiline = true;
				textBox.Location = new Point(5, 5);
				textBox.Size = new Size(390, 235);
				textBox.ScrollBars = ScrollBars.Both;
				textBox.WordWrap = false;
				string text = "Item Code\tLot No.\tError.";
				for (int i = 0; i < errors.Count; i++)
				{
					text = text + errors[i].Item1 + "\t" + errors[i].Item2 + "\t" + errors[i].Item3 + Environment.NewLine;
				}
				textBox.Text = text;
				textBox.ReadOnly = true;
				frm.Controls.Add(textBox);
				SimpleButton simpleButton = new SimpleButton();
				simpleButton.Location = new Point(170, 243);
				simpleButton.Size = new Size(60, 20);
				simpleButton.Text = "Close";
				simpleButton.Click += (object s, EventArgs e1) =>
				{
					frm.Close();
				};
				frm.Controls.Add(simpleButton);
				frm.ShowIcon = false;
				frm.ShowDialog();
			}
			finally
			{
				if (frm != null)
				{
					((IDisposable)frm).Dispose();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void AddNewFGRow()
	{
		try
		{
			DataTable dataTable = grdEx_FinishGoods.DataSource as DataTable;
			if (dataTable == null || dataTable.Columns.Count == 0)
			{
				dataTable = _dtStructFG.Copy();
			}
			if (!dataTable.AsEnumerable().Any((DataRow r) => string.IsNullOrWhiteSpace(Function.ToString(r["ItemCode"]))))
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["DocEntry"] = 0;
				dataRow["Status"] = "R";
				dataRow["Type"] = "S";
				dataRow["PostDate"] = DateTime.Today;
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task AddDefaultBatchRows(string itemCode, string fromWhsCode, string toWhsCode, int docEntry, int lineNum)
	{
		try
		{
			DataTable dataTable = await _httpClient.ProductionOrderImportTool.GetAllBatchSerial(itemCode, fromWhsCode, toWhsCode);
			bool flag = _binWarehouse.Contains(toWhsCode.ToLower());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				if (_DTBinDataTransfer == null || _DTBinDataTransfer.Rows.Count < 3)
				{
					_DTBinDataTransfer = dataTable.Clone();
				}
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					dataTable.Rows[i]["DocEntry"] = docEntry;
					dataTable.Rows[i]["LineNum"] = lineNum;
					if (flag)
					{
						dataTable.Rows[i]["ToBinActivat"] = "Y";
					}
					dataTable.AcceptChanges();
					_DTBinDataTransfer.ImportRow(dataTable.Rows[i]);
				}
			}
			_DTBinDataTransfer.AcceptChanges();
			grdEx_TransferBin.DataSource = dataTable;
			grdEx_TransferBin.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Production.frm_ProductionOrderImportTool));
		this.xtraTabControl1 = new Apzon.Controls.Controls.XtraTabControlEx();
		this.xPage_ImportFinishGoods = new Apzon.Controls.Controls.XtraTabPageEx();
		this.lbl_UpdPrdStatus = new System.Windows.Forms.Label();
		this.lbl_ImportFgElapseTime = new System.Windows.Forms.Label();
		this.lbl_ImportStatus = new System.Windows.Forms.Label();
		this.prg_ProcessFg = new System.Windows.Forms.ProgressBar();
		this.calc_NoOfPrdAtime = new DevExpress.XtraEditors.CalcEdit();
		this.calc_FGInteruptTime = new DevExpress.XtraEditors.CalcEdit();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.btn_ImportToDb = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_ImportFG = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_SaveFG = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grdEx_FinishGoods = new Apzon.Controls.Grid.GridControlEx();
		this.ctm_FG = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.tsi_addRowFG = new System.Windows.Forms.ToolStripMenuItem();
		this.tsi_removeRowFG = new System.Windows.Forms.ToolStripMenuItem();
		this.gvEx_FinishGoods = new Apzon.Controls.Grid.GridViewEx();
		this.xPage_FinishGoods = new DevExpress.XtraTab.XtraTabPage();
		this.grd_FindFg = new Apzon.Controls.Grid.GridControlEx();
		this.gv_FindFg = new Apzon.Controls.Grid.GridViewEx();
		this.xPage_Components = new Apzon.Controls.Controls.XtraTabPageEx();
		this.btn_UpdateTime = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_SaveComp = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grdEx_Components = new Apzon.Controls.Grid.GridControlEx();
		this.gvEx_Components = new Apzon.Controls.Grid.GridViewEx();
		this.xPage_Transfer = new Apzon.Controls.Controls.XtraTabPageEx();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.date_InvTransferPostDate = new Apzon.Controls.Editor.DateEditEx();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.grdEx_Transfer = new Apzon.Controls.Grid.GridControlEx();
		this.gvEx_Transfer = new Apzon.Controls.Grid.GridViewEx();
		this.xtraTabControl2 = new Apzon.Controls.Controls.XtraTabControlEx();
		this.xtraTabPage1 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grdEx_TransferBin = new Apzon.Controls.Grid.GridControlEx();
		this.gvEx_TransferBin = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx5 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gridColumnEx6 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx7 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx8 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx9 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx3 = new Apzon.Controls.Grid.GridColumnEx();
		this.xtraTabPage2 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grdEx_TransferedBin = new Apzon.Controls.Grid.GridControlEx();
		this.gvEx_TransferedBin = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx10 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx11 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx13 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx14 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx15 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx16 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_SaveTransfer = new Apzon.Controls.Controls.SimpleButtonEx();
		this.xPage_ReportComplete = new Apzon.Controls.Controls.XtraTabPageEx();
		this.date_RcPostDate = new Apzon.Controls.Editor.DateEditEx();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.btn_Barcode = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_SaveRC = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grdEx_RC = new Apzon.Controls.Grid.GridControlEx();
		this.gvEx_RC = new Apzon.Controls.Grid.GridViewEx();
		this.xPage_CloseProduction = new Apzon.Controls.Controls.XtraTabPageEx();
		this.btn_SaveClose = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grdEx_Close = new Apzon.Controls.Grid.GridControlEx();
		this.gvEx_Close = new Apzon.Controls.Grid.GridViewEx();
		this.txt_Code = new Apzon.Controls.Editor.TextEditEx();
		this.lb_Name = new Apzon.Controls.Controls.LabelLine();
		this.bgw_FG = new System.ComponentModel.BackgroundWorker();
		this.bgw_SaveTmpFg = new System.ComponentModel.BackgroundWorker();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).BeginInit();
		this.xtraTabControl1.SuspendLayout();
		this.xPage_ImportFinishGoods.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.calc_NoOfPrdAtime.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calc_FGInteruptTime.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grdEx_FinishGoods).BeginInit();
		this.ctm_FG.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gvEx_FinishGoods).BeginInit();
		this.xPage_FinishGoods.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_FindFg).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_FindFg).BeginInit();
		this.xPage_Components.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdEx_Components).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gvEx_Components).BeginInit();
		this.xPage_Transfer.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.date_InvTransferPostDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_InvTransferPostDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grdEx_Transfer).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gvEx_Transfer).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl2).BeginInit();
		this.xtraTabControl2.SuspendLayout();
		this.xtraTabPage1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdEx_TransferBin).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gvEx_TransferBin).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit1).BeginInit();
		this.xtraTabPage2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdEx_TransferedBin).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gvEx_TransferedBin).BeginInit();
		this.xPage_ReportComplete.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.date_RcPostDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_RcPostDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grdEx_RC).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gvEx_RC).BeginInit();
		this.xPage_CloseProduction.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdEx_Close).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gvEx_Close).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Code.Properties).BeginInit();
		base.SuspendLayout();
		this.xtraTabControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.xtraTabControl1.IsSearchControl = false;
		this.xtraTabControl1.Location = new System.Drawing.Point(5, 32);
		this.xtraTabControl1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.xtraTabControl1.Name = "xtraTabControl1";
		this.xtraTabControl1.PaintStyleName = "Standard";
		this.xtraTabControl1.SelectedTabPage = this.xPage_ImportFinishGoods;
		this.xtraTabControl1.Size = new System.Drawing.Size(968, 522);
		this.xtraTabControl1.TabIndex = 0;
		this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[6] { this.xPage_ImportFinishGoods, this.xPage_FinishGoods, this.xPage_Components, this.xPage_Transfer, this.xPage_ReportComplete, this.xPage_CloseProduction });
		this.xtraTabControl1.SelectedPageChanged += new DevExpress.XtraTab.TabPageChangedEventHandler(this.xtraTabControl1_SelectedPageChanged);
		this.xPage_ImportFinishGoods.Controls.Add(this.lbl_UpdPrdStatus);
		this.xPage_ImportFinishGoods.Controls.Add(this.lbl_ImportFgElapseTime);
		this.xPage_ImportFinishGoods.Controls.Add(this.lbl_ImportStatus);
		this.xPage_ImportFinishGoods.Controls.Add(this.prg_ProcessFg);
		this.xPage_ImportFinishGoods.Controls.Add(this.calc_NoOfPrdAtime);
		this.xPage_ImportFinishGoods.Controls.Add(this.calc_FGInteruptTime);
		this.xPage_ImportFinishGoods.Controls.Add(this.labelLine4);
		this.xPage_ImportFinishGoods.Controls.Add(this.labelLine3);
		this.xPage_ImportFinishGoods.Controls.Add(this.btn_ImportToDb);
		this.xPage_ImportFinishGoods.Controls.Add(this.btn_ImportFG);
		this.xPage_ImportFinishGoods.Controls.Add(this.btn_SaveFG);
		this.xPage_ImportFinishGoods.Controls.Add(this.grdEx_FinishGoods);
		this.xPage_ImportFinishGoods.EditControl = null;
		this.xPage_ImportFinishGoods.EditValue = "Import";
		this.xPage_ImportFinishGoods.FieldName = "xPage_FinishGoods";
		this.xPage_ImportFinishGoods.IsSearchControl = false;
		this.xPage_ImportFinishGoods.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.xPage_ImportFinishGoods.Name = "xPage_ImportFinishGoods";
		this.xPage_ImportFinishGoods.ReadOnly = false;
		this.xPage_ImportFinishGoods.SettingInVisible = false;
		this.xPage_ImportFinishGoods.SettingReadonly = false;
		this.xPage_ImportFinishGoods.Size = new System.Drawing.Size(964, 498);
		this.xPage_ImportFinishGoods.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xPage_ImportFinishGoods.Text = "Import";
		this.lbl_UpdPrdStatus.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.lbl_UpdPrdStatus.AutoSize = true;
		this.lbl_UpdPrdStatus.Location = new System.Drawing.Point(505, 444);
		this.lbl_UpdPrdStatus.Name = "lbl_UpdPrdStatus";
		this.lbl_UpdPrdStatus.Size = new System.Drawing.Size(141, 13);
		this.lbl_UpdPrdStatus.TabIndex = 22;
		this.lbl_UpdPrdStatus.Text = "Updating status to 'Release'";
		this.lbl_UpdPrdStatus.Visible = false;
		this.lbl_ImportFgElapseTime.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.lbl_ImportFgElapseTime.AutoSize = true;
		this.lbl_ImportFgElapseTime.Location = new System.Drawing.Point(358, 444);
		this.lbl_ImportFgElapseTime.Name = "lbl_ImportFgElapseTime";
		this.lbl_ImportFgElapseTime.Size = new System.Drawing.Size(69, 13);
		this.lbl_ImportFgElapseTime.TabIndex = 21;
		this.lbl_ImportFgElapseTime.Text = "Elapsed Time";
		this.lbl_ImportStatus.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.lbl_ImportStatus.AutoSize = true;
		this.lbl_ImportStatus.Location = new System.Drawing.Point(582, 471);
		this.lbl_ImportStatus.Name = "lbl_ImportStatus";
		this.lbl_ImportStatus.Size = new System.Drawing.Size(0, 13);
		this.lbl_ImportStatus.TabIndex = 20;
		this.prg_ProcessFg.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.prg_ProcessFg.Location = new System.Drawing.Point(361, 462);
		this.prg_ProcessFg.Name = "prg_ProcessFg";
		this.prg_ProcessFg.Size = new System.Drawing.Size(217, 23);
		this.prg_ProcessFg.TabIndex = 19;
		this.calc_NoOfPrdAtime.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.calc_NoOfPrdAtime.Location = new System.Drawing.Point(892, 466);
		this.calc_NoOfPrdAtime.Name = "calc_NoOfPrdAtime";
		this.calc_NoOfPrdAtime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.calc_NoOfPrdAtime.Size = new System.Drawing.Size(60, 20);
		this.calc_NoOfPrdAtime.TabIndex = 18;
		this.calc_FGInteruptTime.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.calc_FGInteruptTime.Location = new System.Drawing.Point(892, 443);
		this.calc_FGInteruptTime.Name = "calc_FGInteruptTime";
		this.calc_FGInteruptTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.calc_FGInteruptTime.Size = new System.Drawing.Size(60, 20);
		this.calc_FGInteruptTime.TabIndex = 17;
		this.labelLine4.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "No. of Prd Order at the same time";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(722, 469);
		this.labelLine4.Margin = new System.Windows.Forms.Padding(4);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(169, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 16;
		this.labelLine4.Utext = "No. of Prd Order at the same time";
		this.labelLine3.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "Interrupt time (s)";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(722, 446);
		this.labelLine3.Margin = new System.Windows.Forms.Padding(4);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(169, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 15;
		this.labelLine3.Utext = "Interrupt time (s)";
		this.btn_ImportToDb.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_ImportToDb.ButtonKind = Apzon.Commons.Enums.Enums.SimpleButtonType.Save;
		this.btn_ImportToDb.EditControl = null;
		this.btn_ImportToDb.EditValue = "Save";
		this.btn_ImportToDb.FieldName = null;
		this.btn_ImportToDb.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_ImportToDb.ImageOptions.Image");
		this.btn_ImportToDb.IsSearchControl = false;
		this.btn_ImportToDb.Location = new System.Drawing.Point(105, 462);
		this.btn_ImportToDb.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.btn_ImportToDb.Name = "btn_ImportToDb";
		this.btn_ImportToDb.ReadOnly = false;
		this.btn_ImportToDb.SettingInVisible = false;
		this.btn_ImportToDb.SettingReadonly = false;
		this.btn_ImportToDb.Size = new System.Drawing.Size(72, 24);
		this.btn_ImportToDb.TabIndex = 14;
		this.btn_ImportToDb.Text = "Save";
		this.btn_ImportToDb.Click += new System.EventHandler(this.btn_ImportToDb_Click);
		this.btn_ImportFG.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_ImportFG.EditControl = null;
		this.btn_ImportFG.EditValue = "Open Excell";
		this.btn_ImportFG.FieldName = null;
		this.btn_ImportFG.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_ImportFG.ImageOptions.Image");
		this.btn_ImportFG.IsSearchControl = false;
		this.btn_ImportFG.Location = new System.Drawing.Point(12, 462);
		this.btn_ImportFG.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.btn_ImportFG.Name = "btn_ImportFG";
		this.btn_ImportFG.ReadOnly = false;
		this.btn_ImportFG.SettingInVisible = false;
		this.btn_ImportFG.SettingReadonly = false;
		this.btn_ImportFG.Size = new System.Drawing.Size(86, 24);
		this.btn_ImportFG.TabIndex = 12;
		this.btn_ImportFG.Text = "Open Excell";
		this.btn_ImportFG.Click += new System.EventHandler(this.btn_ImportFG_Click);
		this.btn_SaveFG.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_SaveFG.ButtonKind = Apzon.Commons.Enums.Enums.SimpleButtonType.Save;
		this.btn_SaveFG.EditControl = null;
		this.btn_SaveFG.EditValue = "Generate Production Order";
		this.btn_SaveFG.FieldName = null;
		this.btn_SaveFG.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_SaveFG.ImageOptions.Image");
		this.btn_SaveFG.IsSearchControl = false;
		this.btn_SaveFG.Location = new System.Drawing.Point(185, 462);
		this.btn_SaveFG.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.btn_SaveFG.Name = "btn_SaveFG";
		this.btn_SaveFG.ReadOnly = false;
		this.btn_SaveFG.SettingInVisible = false;
		this.btn_SaveFG.SettingReadonly = false;
		this.btn_SaveFG.Size = new System.Drawing.Size(160, 24);
		this.btn_SaveFG.TabIndex = 11;
		this.btn_SaveFG.Text = "Generate Production Order";
		this.btn_SaveFG.Click += new System.EventHandler(this.btn_SaveFG_Click);
		this.grdEx_FinishGoods.AllowPasteExcel = true;
		this.grdEx_FinishGoods.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grdEx_FinishGoods.ClipboardData = null;
		this.grdEx_FinishGoods.ContextMenuStrip = this.ctm_FG;
		this.grdEx_FinishGoods.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grdEx_FinishGoods.Location = new System.Drawing.Point(10, 2);
		this.grdEx_FinishGoods.MainView = this.gvEx_FinishGoods;
		this.grdEx_FinishGoods.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grdEx_FinishGoods.Name = "grdEx_FinishGoods";
		this.grdEx_FinishGoods.Size = new System.Drawing.Size(942, 436);
		this.grdEx_FinishGoods.TabIndex = 10;
		this.grdEx_FinishGoods.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvEx_FinishGoods });
		this.grdEx_FinishGoods.ProcessGridKey += new System.Windows.Forms.KeyEventHandler(this.grdEx_FinishGoods_ProcessGridKey);
		this.ctm_FG.ImageScalingSize = new System.Drawing.Size(24, 24);
		this.ctm_FG.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.tsi_addRowFG, this.tsi_removeRowFG });
		this.ctm_FG.Name = "ctm_FG";
		this.ctm_FG.Size = new System.Drawing.Size(144, 48);
		this.tsi_addRowFG.Name = "tsi_addRowFG";
		this.tsi_addRowFG.Size = new System.Drawing.Size(143, 22);
		this.tsi_addRowFG.Text = "Add Row";
		this.tsi_addRowFG.Click += new System.EventHandler(this.tsi_addRowFG_Click);
		this.tsi_removeRowFG.Name = "tsi_removeRowFG";
		this.tsi_removeRowFG.Size = new System.Drawing.Size(143, 22);
		this.tsi_removeRowFG.Text = "Remove Row";
		this.tsi_removeRowFG.Click += new System.EventHandler(this.tsi_removeRowFG_Click);
		this.gvEx_FinishGoods.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvEx_FinishGoods.ColumnKey = null;
		this.gvEx_FinishGoods.DisableFormSetting = false;
		this.gvEx_FinishGoods.DisplayException = false;
		this.gvEx_FinishGoods.EditColumn = null;
		this.gvEx_FinishGoods.FieldCurrency = null;
		this.gvEx_FinishGoods.FieldRate = null;
		this.gvEx_FinishGoods.FormId = "oProductionOrderImportTool";
		this.gvEx_FinishGoods.GridControl = this.grdEx_FinishGoods;
		this.gvEx_FinishGoods.IgnoreFilter = false;
		this.gvEx_FinishGoods.IndexEditRow = -1;
		this.gvEx_FinishGoods.IndicatorWidth = 70;
		this.gvEx_FinishGoods.IsApzTable = false;
		this.gvEx_FinishGoods.IsInsertNewRow = false;
		this.gvEx_FinishGoods.IsLoadingData = false;
		this.gvEx_FinishGoods.IsMappingContextAction = false;
		this.gvEx_FinishGoods.ItemId = "OITT";
		this.gvEx_FinishGoods.LstColumnAllowBatch = null;
		this.gvEx_FinishGoods.LstColumnNotAllowBatch = null;
		this.gvEx_FinishGoods.LstColumnNotEditable = null;
		this.gvEx_FinishGoods.LstColumNotSetting = null;
		this.gvEx_FinishGoods.Name = "gvEx_FinishGoods";
		this.gvEx_FinishGoods.OptionsFind.AllowFindPanel = false;
		this.gvEx_FinishGoods.OptionsSelection.MultiSelect = true;
		this.gvEx_FinishGoods.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
		this.gvEx_FinishGoods.OptionsView.ColumnAutoWidth = false;
		this.gvEx_FinishGoods.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gvEx_FinishGoods.OptionsView.ShowGroupPanel = false;
		this.gvEx_FinishGoods.UseBatchMod = false;
		this.gvEx_FinishGoods.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gvEx_FinishGoods_RowCellStyle);
		this.gvEx_FinishGoods.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gvEx_FinishGoods_ShowingEditor);
		this.gvEx_FinishGoods.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvEx_FinishGoods_CellValueChanged);
		this.xPage_FinishGoods.Controls.Add(this.grd_FindFg);
		this.xPage_FinishGoods.Name = "xPage_FinishGoods";
		this.xPage_FinishGoods.Size = new System.Drawing.Size(964, 498);
		this.xPage_FinishGoods.Text = "Finish Goods";
		this.grd_FindFg.AllowPasteExcel = false;
		this.grd_FindFg.ClipboardData = null;
		this.grd_FindFg.ContextMenuStrip = this.ctm_FG;
		this.grd_FindFg.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_FindFg.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grd_FindFg.Location = new System.Drawing.Point(0, 0);
		this.grd_FindFg.MainView = this.gv_FindFg;
		this.grd_FindFg.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grd_FindFg.Name = "grd_FindFg";
		this.grd_FindFg.Size = new System.Drawing.Size(964, 498);
		this.grd_FindFg.TabIndex = 11;
		this.grd_FindFg.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_FindFg });
		this.gv_FindFg.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_FindFg.ColumnKey = null;
		this.gv_FindFg.DisableFormSetting = false;
		this.gv_FindFg.DisplayException = false;
		this.gv_FindFg.EditColumn = null;
		this.gv_FindFg.FieldCurrency = null;
		this.gv_FindFg.FieldRate = null;
		this.gv_FindFg.FormId = "oProductionOrderImportTool";
		this.gv_FindFg.GridControl = this.grd_FindFg;
		this.gv_FindFg.IgnoreFilter = false;
		this.gv_FindFg.IndexEditRow = -1;
		this.gv_FindFg.IndicatorWidth = 70;
		this.gv_FindFg.IsApzTable = false;
		this.gv_FindFg.IsInsertNewRow = false;
		this.gv_FindFg.IsLoadingData = false;
		this.gv_FindFg.IsMappingContextAction = false;
		this.gv_FindFg.ItemId = "FOITT";
		this.gv_FindFg.LstColumnAllowBatch = null;
		this.gv_FindFg.LstColumnNotAllowBatch = null;
		this.gv_FindFg.LstColumnNotEditable = null;
		this.gv_FindFg.LstColumNotSetting = null;
		this.gv_FindFg.Name = "gv_FindFg";
		this.gv_FindFg.OptionsFind.AllowFindPanel = false;
		this.gv_FindFg.OptionsSelection.MultiSelect = true;
		this.gv_FindFg.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
		this.gv_FindFg.OptionsView.ColumnAutoWidth = false;
		this.gv_FindFg.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_FindFg.OptionsView.ShowGroupPanel = false;
		this.gv_FindFg.UseBatchMod = false;
		this.xPage_Components.Controls.Add(this.btn_UpdateTime);
		this.xPage_Components.Controls.Add(this.btn_SaveComp);
		this.xPage_Components.Controls.Add(this.grdEx_Components);
		this.xPage_Components.EditControl = null;
		this.xPage_Components.EditValue = "Components";
		this.xPage_Components.FieldName = "xPage_Components";
		this.xPage_Components.IsSearchControl = false;
		this.xPage_Components.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.xPage_Components.Name = "xPage_Components";
		this.xPage_Components.ReadOnly = false;
		this.xPage_Components.SettingInVisible = false;
		this.xPage_Components.SettingReadonly = false;
		this.xPage_Components.Size = new System.Drawing.Size(964, 498);
		this.xPage_Components.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xPage_Components.Text = "Components";
		this.btn_UpdateTime.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_UpdateTime.EditControl = null;
		this.btn_UpdateTime.EditValue = "Update Time";
		this.btn_UpdateTime.FieldName = null;
		this.btn_UpdateTime.IsSearchControl = false;
		this.btn_UpdateTime.Location = new System.Drawing.Point(99, 462);
		this.btn_UpdateTime.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.btn_UpdateTime.Name = "btn_UpdateTime";
		this.btn_UpdateTime.ReadOnly = false;
		this.btn_UpdateTime.SettingInVisible = false;
		this.btn_UpdateTime.SettingReadonly = false;
		this.btn_UpdateTime.Size = new System.Drawing.Size(92, 24);
		this.btn_UpdateTime.TabIndex = 53;
		this.btn_UpdateTime.Text = "Update Time";
		this.btn_UpdateTime.Click += new System.EventHandler(this.btn_UpdateTime_Click);
		this.btn_SaveComp.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_SaveComp.EditControl = null;
		this.btn_SaveComp.EditValue = "Find";
		this.btn_SaveComp.FieldName = null;
		this.btn_SaveComp.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_SaveComp.ImageOptions.Image");
		this.btn_SaveComp.IsSearchControl = false;
		this.btn_SaveComp.Location = new System.Drawing.Point(10, 462);
		this.btn_SaveComp.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.btn_SaveComp.Name = "btn_SaveComp";
		this.btn_SaveComp.ReadOnly = false;
		this.btn_SaveComp.SettingInVisible = false;
		this.btn_SaveComp.SettingReadonly = false;
		this.btn_SaveComp.Size = new System.Drawing.Size(75, 24);
		this.btn_SaveComp.TabIndex = 24;
		this.btn_SaveComp.Text = "Find";
		this.btn_SaveComp.Click += new System.EventHandler(this.btn_SaveComp_Click);
		this.grdEx_Components.AllowPasteExcel = false;
		this.grdEx_Components.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grdEx_Components.ClipboardData = null;
		this.grdEx_Components.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grdEx_Components.Location = new System.Drawing.Point(10, 2);
		this.grdEx_Components.MainView = this.gvEx_Components;
		this.grdEx_Components.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grdEx_Components.Name = "grdEx_Components";
		this.grdEx_Components.Size = new System.Drawing.Size(904, 448);
		this.grdEx_Components.TabIndex = 23;
		this.grdEx_Components.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvEx_Components });
		this.gvEx_Components.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvEx_Components.ColumnKey = null;
		this.gvEx_Components.DisableFormSetting = false;
		this.gvEx_Components.DisplayException = false;
		this.gvEx_Components.EditColumn = null;
		this.gvEx_Components.FieldCurrency = null;
		this.gvEx_Components.FieldRate = null;
		this.gvEx_Components.FormId = "oProductionOrderImportTool";
		this.gvEx_Components.GridControl = this.grdEx_Components;
		this.gvEx_Components.IgnoreFilter = false;
		this.gvEx_Components.IndexEditRow = -1;
		this.gvEx_Components.IndicatorWidth = 70;
		this.gvEx_Components.IsApzTable = false;
		this.gvEx_Components.IsInsertNewRow = false;
		this.gvEx_Components.IsLoadingData = false;
		this.gvEx_Components.IsMappingContextAction = false;
		this.gvEx_Components.ItemId = "ITT1";
		this.gvEx_Components.LstColumnAllowBatch = null;
		this.gvEx_Components.LstColumnNotAllowBatch = null;
		this.gvEx_Components.LstColumnNotEditable = null;
		this.gvEx_Components.LstColumNotSetting = null;
		this.gvEx_Components.Name = "gvEx_Components";
		this.gvEx_Components.OptionsFind.AllowFindPanel = false;
		this.gvEx_Components.OptionsView.ColumnAutoWidth = false;
		this.gvEx_Components.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gvEx_Components.OptionsView.ShowGroupPanel = false;
		this.gvEx_Components.UseBatchMod = false;
		this.gvEx_Components.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gvEx_Components_RowCellStyle);
		this.gvEx_Components.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gvEx_Components_ShowingEditor);
		this.xPage_Transfer.Controls.Add(this.splitContainerControl1);
		this.xPage_Transfer.Controls.Add(this.btn_SaveTransfer);
		this.xPage_Transfer.EditControl = null;
		this.xPage_Transfer.EditValue = "Inventory Transfer";
		this.xPage_Transfer.FieldName = "xPage_Transfer";
		this.xPage_Transfer.IsSearchControl = false;
		this.xPage_Transfer.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.xPage_Transfer.Name = "xPage_Transfer";
		this.xPage_Transfer.ReadOnly = false;
		this.xPage_Transfer.SettingInVisible = false;
		this.xPage_Transfer.SettingReadonly = false;
		this.xPage_Transfer.Size = new System.Drawing.Size(964, 498);
		this.xPage_Transfer.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xPage_Transfer.Text = "Inventory Transfer";
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(3, 2);
		this.splitContainerControl1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
		this.splitContainerControl1.Panel1.Controls.Add(this.date_InvTransferPostDate);
		this.splitContainerControl1.Panel1.Controls.Add(this.labelLine1);
		this.splitContainerControl1.Panel1.Controls.Add(this.grdEx_Transfer);
		this.splitContainerControl1.Panel1.ShowCaption = true;
		this.splitContainerControl1.Panel1.Text = "Items";
		this.splitContainerControl1.Panel2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
		this.splitContainerControl1.Panel2.Controls.Add(this.xtraTabControl2);
		this.splitContainerControl1.Panel2.ShowCaption = true;
		this.splitContainerControl1.Panel2.Text = "Bin";
		this.splitContainerControl1.Size = new System.Drawing.Size(923, 458);
		this.splitContainerControl1.SplitterPosition = 231;
		this.splitContainerControl1.TabIndex = 51;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.date_InvTransferPostDate.EditControl = null;
		this.date_InvTransferPostDate.EditValue = null;
		this.date_InvTransferPostDate.FieldName = null;
		this.date_InvTransferPostDate.Location = new System.Drawing.Point(86, 1);
		this.date_InvTransferPostDate.Name = "date_InvTransferPostDate";
		this.date_InvTransferPostDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_InvTransferPostDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_InvTransferPostDate.Properties.IsSearchControl = false;
		this.date_InvTransferPostDate.SettingInVisible = false;
		this.date_InvTransferPostDate.SettingReadonly = false;
		this.date_InvTransferPostDate.Size = new System.Drawing.Size(100, 20);
		this.date_InvTransferPostDate.TabIndex = 52;
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Posting Date";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(5, 5);
		this.labelLine1.Margin = new System.Windows.Forms.Padding(4);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(99, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 51;
		this.labelLine1.Utext = "Posting Date";
		this.grdEx_Transfer.AllowPasteExcel = false;
		this.grdEx_Transfer.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grdEx_Transfer.ClipboardData = null;
		this.grdEx_Transfer.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grdEx_Transfer.Location = new System.Drawing.Point(0, 26);
		this.grdEx_Transfer.MainView = this.gvEx_Transfer;
		this.grdEx_Transfer.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grdEx_Transfer.Name = "grdEx_Transfer";
		this.grdEx_Transfer.Size = new System.Drawing.Size(919, 183);
		this.grdEx_Transfer.TabIndex = 49;
		this.grdEx_Transfer.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvEx_Transfer });
		this.grdEx_Transfer.EditorKeyDown += new System.Windows.Forms.KeyEventHandler(this.grdEx_Transfer_EditorKeyDown);
		this.gvEx_Transfer.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvEx_Transfer.ColumnKey = null;
		this.gvEx_Transfer.DisableFormSetting = false;
		this.gvEx_Transfer.DisplayException = false;
		this.gvEx_Transfer.EditColumn = null;
		this.gvEx_Transfer.FieldCurrency = null;
		this.gvEx_Transfer.FieldRate = null;
		this.gvEx_Transfer.FormId = "oProductionOrderImportTool";
		this.gvEx_Transfer.GridControl = this.grdEx_Transfer;
		this.gvEx_Transfer.IgnoreFilter = false;
		this.gvEx_Transfer.IndexEditRow = -1;
		this.gvEx_Transfer.IndicatorWidth = 70;
		this.gvEx_Transfer.IsApzTable = false;
		this.gvEx_Transfer.IsInsertNewRow = false;
		this.gvEx_Transfer.IsLoadingData = false;
		this.gvEx_Transfer.IsMappingContextAction = false;
		this.gvEx_Transfer.ItemId = "OWTR";
		this.gvEx_Transfer.LstColumnAllowBatch = null;
		this.gvEx_Transfer.LstColumnNotAllowBatch = null;
		this.gvEx_Transfer.LstColumnNotEditable = null;
		this.gvEx_Transfer.LstColumNotSetting = null;
		this.gvEx_Transfer.Name = "gvEx_Transfer";
		this.gvEx_Transfer.OptionsFind.AllowFindPanel = false;
		this.gvEx_Transfer.OptionsSelection.MultiSelect = true;
		this.gvEx_Transfer.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
		this.gvEx_Transfer.OptionsView.ColumnAutoWidth = false;
		this.gvEx_Transfer.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gvEx_Transfer.OptionsView.ShowGroupPanel = false;
		this.gvEx_Transfer.UseBatchMod = false;
		this.gvEx_Transfer.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gvEx_Transfer_RowCellStyle);
		this.gvEx_Transfer.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gvEx_Transfer_ShowingEditor);
		this.gvEx_Transfer.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gvEx_Transfer_FocusedRowChanged);
		this.gvEx_Transfer.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gvEx_Transfer_FocusedRowObjectChanged);
		this.gvEx_Transfer.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvEx_Transfer_CellValueChanged);
		this.gvEx_Transfer.BeforeLeaveRow += new DevExpress.XtraGrid.Views.Base.RowAllowEventHandler(this.gvEx_Transfer_BeforeLeaveRow);
		this.xtraTabControl2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.xtraTabControl2.IsSearchControl = false;
		this.xtraTabControl2.Location = new System.Drawing.Point(0, 0);
		this.xtraTabControl2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.xtraTabControl2.Name = "xtraTabControl2";
		this.xtraTabControl2.SelectedTabPage = this.xtraTabPage1;
		this.xtraTabControl2.Size = new System.Drawing.Size(919, 192);
		this.xtraTabControl2.TabIndex = 51;
		this.xtraTabControl2.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[2] { this.xtraTabPage1, this.xtraTabPage2 });
		this.xtraTabPage1.Controls.Add(this.grdEx_TransferBin);
		this.xtraTabPage1.EditControl = null;
		this.xtraTabPage1.EditValue = "Available Bins";
		this.xtraTabPage1.FieldName = "xtraTabPage1";
		this.xtraTabPage1.IsSearchControl = false;
		this.xtraTabPage1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.xtraTabPage1.Name = "xtraTabPage1";
		this.xtraTabPage1.ReadOnly = false;
		this.xtraTabPage1.SettingInVisible = false;
		this.xtraTabPage1.SettingReadonly = false;
		this.xtraTabPage1.Size = new System.Drawing.Size(917, 167);
		this.xtraTabPage1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPage1.Text = "Available Bins";
		this.grdEx_TransferBin.AllowPasteExcel = false;
		this.grdEx_TransferBin.ClipboardData = null;
		this.grdEx_TransferBin.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grdEx_TransferBin.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grdEx_TransferBin.Location = new System.Drawing.Point(0, 0);
		this.grdEx_TransferBin.MainView = this.gvEx_TransferBin;
		this.grdEx_TransferBin.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grdEx_TransferBin.Name = "grdEx_TransferBin";
		this.grdEx_TransferBin.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.repositoryItemCalcEdit1 });
		this.grdEx_TransferBin.Size = new System.Drawing.Size(917, 167);
		this.grdEx_TransferBin.TabIndex = 50;
		this.grdEx_TransferBin.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvEx_TransferBin });
		this.gvEx_TransferBin.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvEx_TransferBin.ColumnKey = null;
		this.gvEx_TransferBin.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[9] { this.gridColumnEx2, this.gridColumnEx1, this.gridColumnEx4, this.gridColumnEx5, this.gridColumnEx6, this.gridColumnEx7, this.gridColumnEx8, this.gridColumnEx9, this.gridColumnEx3 });
		this.gvEx_TransferBin.DisableFormSetting = false;
		this.gvEx_TransferBin.DisplayException = false;
		this.gvEx_TransferBin.EditColumn = null;
		this.gvEx_TransferBin.FieldCurrency = null;
		this.gvEx_TransferBin.FieldRate = null;
		this.gvEx_TransferBin.FormId = "oProductionOrderImportTool";
		this.gvEx_TransferBin.GridControl = this.grdEx_TransferBin;
		this.gvEx_TransferBin.IgnoreFilter = false;
		this.gvEx_TransferBin.IndexEditRow = -1;
		this.gvEx_TransferBin.IndicatorWidth = 70;
		this.gvEx_TransferBin.IsApzTable = false;
		this.gvEx_TransferBin.IsInsertNewRow = false;
		this.gvEx_TransferBin.IsLoadingData = false;
		this.gvEx_TransferBin.IsMappingContextAction = false;
		this.gvEx_TransferBin.ItemId = "OBIN";
		this.gvEx_TransferBin.LstColumnAllowBatch = null;
		this.gvEx_TransferBin.LstColumnNotAllowBatch = null;
		this.gvEx_TransferBin.LstColumnNotEditable = null;
		this.gvEx_TransferBin.LstColumNotSetting = null;
		this.gvEx_TransferBin.Name = "gvEx_TransferBin";
		this.gvEx_TransferBin.OptionsFind.AllowFindPanel = false;
		this.gvEx_TransferBin.OptionsView.ColumnAutoWidth = false;
		this.gvEx_TransferBin.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gvEx_TransferBin.OptionsView.ShowGroupPanel = false;
		this.gvEx_TransferBin.UseBatchMod = false;
		this.gvEx_TransferBin.ValidatingEditor += new DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventHandler(this.gvEx_TransferBin_ValidatingEditor);
		this.gvEx_TransferBin.InvalidValueException += new DevExpress.XtraEditors.Controls.InvalidValueExceptionEventHandler(this.gvEx_TransferBin_InvalidValueException);
		this.gridColumnEx2.Caption = "Batch/Serial";
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.FieldName = "DistNumber";
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.OptionsColumn.ReadOnly = true;
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.UnitField = null;
		this.gridColumnEx2.Visible = true;
		this.gridColumnEx2.VisibleIndex = 0;
		this.gridColumnEx2.Width = 141;
		this.gridColumnEx1.Caption = "From Bin";
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "FromBinCode";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.OptionsColumn.ReadOnly = true;
		this.gridColumnEx1.PrimaryKey = false;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx1.Visible = true;
		this.gridColumnEx1.VisibleIndex = 1;
		this.gridColumnEx4.Caption = "Available Qty.";
		this.gridColumnEx4.CurrencyDateField = null;
		this.gridColumnEx4.CurrencyField = null;
		this.gridColumnEx4.CurrencyFixed = null;
		this.gridColumnEx4.CurrencyFixedRate = 0.0;
		this.gridColumnEx4.CurrencyRateField = null;
		this.gridColumnEx4.FieldName = "AvailQty";
		this.gridColumnEx4.Name = "gridColumnEx4";
		this.gridColumnEx4.OptionsColumn.ReadOnly = true;
		this.gridColumnEx4.PrimaryKey = false;
		this.gridColumnEx4.UnitField = null;
		this.gridColumnEx4.Visible = true;
		this.gridColumnEx4.VisibleIndex = 2;
		this.gridColumnEx4.Width = 100;
		this.gridColumnEx5.Caption = "Quantity";
		this.gridColumnEx5.ColumnEdit = this.repositoryItemCalcEdit1;
		this.gridColumnEx5.CurrencyDateField = null;
		this.gridColumnEx5.CurrencyField = null;
		this.gridColumnEx5.CurrencyFixed = null;
		this.gridColumnEx5.CurrencyFixedRate = 0.0;
		this.gridColumnEx5.CurrencyRateField = null;
		this.gridColumnEx5.FieldName = "Quantity";
		this.gridColumnEx5.Name = "gridColumnEx5";
		this.gridColumnEx5.PrimaryKey = false;
		this.gridColumnEx5.UnitField = null;
		this.gridColumnEx5.Visible = true;
		this.gridColumnEx5.VisibleIndex = 3;
		this.repositoryItemCalcEdit1.AutoHeight = false;
		this.repositoryItemCalcEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEdit1.Name = "repositoryItemCalcEdit1";
		this.gridColumnEx6.Caption = "To Bin";
		this.gridColumnEx6.CurrencyDateField = null;
		this.gridColumnEx6.CurrencyField = null;
		this.gridColumnEx6.CurrencyFixed = null;
		this.gridColumnEx6.CurrencyFixedRate = 0.0;
		this.gridColumnEx6.CurrencyRateField = null;
		this.gridColumnEx6.FieldName = "ToBin";
		this.gridColumnEx6.Name = "gridColumnEx6";
		this.gridColumnEx6.PrimaryKey = false;
		this.gridColumnEx6.UnitField = null;
		this.gridColumnEx6.Visible = true;
		this.gridColumnEx6.VisibleIndex = 4;
		this.gridColumnEx7.Caption = "Doc Entry";
		this.gridColumnEx7.CurrencyDateField = null;
		this.gridColumnEx7.CurrencyField = null;
		this.gridColumnEx7.CurrencyFixed = null;
		this.gridColumnEx7.CurrencyFixedRate = 0.0;
		this.gridColumnEx7.CurrencyRateField = null;
		this.gridColumnEx7.FieldName = "DocEntry";
		this.gridColumnEx7.Name = "gridColumnEx7";
		this.gridColumnEx7.PrimaryKey = false;
		this.gridColumnEx7.UnitField = null;
		this.gridColumnEx8.Caption = "Line Num";
		this.gridColumnEx8.CurrencyDateField = null;
		this.gridColumnEx8.CurrencyField = null;
		this.gridColumnEx8.CurrencyFixed = null;
		this.gridColumnEx8.CurrencyFixedRate = 0.0;
		this.gridColumnEx8.CurrencyRateField = null;
		this.gridColumnEx8.FieldName = "LineNum";
		this.gridColumnEx8.Name = "gridColumnEx8";
		this.gridColumnEx8.PrimaryKey = false;
		this.gridColumnEx8.UnitField = null;
		this.gridColumnEx9.Caption = "BtSAbsEntry";
		this.gridColumnEx9.CurrencyDateField = null;
		this.gridColumnEx9.CurrencyField = null;
		this.gridColumnEx9.CurrencyFixed = null;
		this.gridColumnEx9.CurrencyFixedRate = 0.0;
		this.gridColumnEx9.CurrencyRateField = null;
		this.gridColumnEx9.FieldName = "BtSAbsEntry";
		this.gridColumnEx9.Name = "gridColumnEx9";
		this.gridColumnEx9.PrimaryKey = false;
		this.gridColumnEx9.UnitField = null;
		this.gridColumnEx3.Caption = "From Bin";
		this.gridColumnEx3.CurrencyDateField = null;
		this.gridColumnEx3.CurrencyField = null;
		this.gridColumnEx3.CurrencyFixed = null;
		this.gridColumnEx3.CurrencyFixedRate = 0.0;
		this.gridColumnEx3.CurrencyRateField = null;
		this.gridColumnEx3.FieldName = "FromBin";
		this.gridColumnEx3.Name = "gridColumnEx3";
		this.gridColumnEx3.PrimaryKey = false;
		this.gridColumnEx3.UnitField = null;
		this.gridColumnEx3.Width = 120;
		this.xtraTabPage2.Controls.Add(this.grdEx_TransferedBin);
		this.xtraTabPage2.EditControl = null;
		this.xtraTabPage2.EditValue = "History";
		this.xtraTabPage2.FieldName = "xtraTabPage2";
		this.xtraTabPage2.IsSearchControl = false;
		this.xtraTabPage2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.xtraTabPage2.Name = "xtraTabPage2";
		this.xtraTabPage2.ReadOnly = false;
		this.xtraTabPage2.SettingInVisible = false;
		this.xtraTabPage2.SettingReadonly = false;
		this.xtraTabPage2.Size = new System.Drawing.Size(917, 167);
		this.xtraTabPage2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPage2.Text = "History";
		this.grdEx_TransferedBin.AllowPasteExcel = false;
		this.grdEx_TransferedBin.ClipboardData = null;
		this.grdEx_TransferedBin.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grdEx_TransferedBin.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grdEx_TransferedBin.Location = new System.Drawing.Point(0, 0);
		this.grdEx_TransferedBin.MainView = this.gvEx_TransferedBin;
		this.grdEx_TransferedBin.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grdEx_TransferedBin.Name = "grdEx_TransferedBin";
		this.grdEx_TransferedBin.Size = new System.Drawing.Size(917, 167);
		this.grdEx_TransferedBin.TabIndex = 51;
		this.grdEx_TransferedBin.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvEx_TransferedBin });
		this.gvEx_TransferedBin.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvEx_TransferedBin.ColumnKey = null;
		this.gvEx_TransferedBin.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[6] { this.gridColumnEx10, this.gridColumnEx11, this.gridColumnEx13, this.gridColumnEx14, this.gridColumnEx15, this.gridColumnEx16 });
		this.gvEx_TransferedBin.DisableFormSetting = false;
		this.gvEx_TransferedBin.DisplayException = false;
		this.gvEx_TransferedBin.EditColumn = null;
		this.gvEx_TransferedBin.FieldCurrency = null;
		this.gvEx_TransferedBin.FieldRate = null;
		this.gvEx_TransferedBin.FormId = "oProductionOrderImportTool";
		this.gvEx_TransferedBin.GridControl = this.grdEx_TransferedBin;
		this.gvEx_TransferedBin.IgnoreFilter = false;
		this.gvEx_TransferedBin.IndexEditRow = -1;
		this.gvEx_TransferedBin.IndicatorWidth = 70;
		this.gvEx_TransferedBin.IsApzTable = false;
		this.gvEx_TransferedBin.IsInsertNewRow = false;
		this.gvEx_TransferedBin.IsLoadingData = false;
		this.gvEx_TransferedBin.IsMappingContextAction = false;
		this.gvEx_TransferedBin.ItemId = "OHISTORY";
		this.gvEx_TransferedBin.LstColumnAllowBatch = null;
		this.gvEx_TransferedBin.LstColumnNotAllowBatch = null;
		this.gvEx_TransferedBin.LstColumnNotEditable = null;
		this.gvEx_TransferedBin.LstColumNotSetting = null;
		this.gvEx_TransferedBin.Name = "gvEx_TransferedBin";
		this.gvEx_TransferedBin.OptionsFind.AllowFindPanel = false;
		this.gvEx_TransferedBin.OptionsView.ColumnAutoWidth = false;
		this.gvEx_TransferedBin.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gvEx_TransferedBin.OptionsView.ShowGroupPanel = false;
		this.gvEx_TransferedBin.UseBatchMod = false;
		this.gridColumnEx10.Caption = "Batch/Serial";
		this.gridColumnEx10.CurrencyDateField = null;
		this.gridColumnEx10.CurrencyField = null;
		this.gridColumnEx10.CurrencyFixed = null;
		this.gridColumnEx10.CurrencyFixedRate = 0.0;
		this.gridColumnEx10.CurrencyRateField = null;
		this.gridColumnEx10.FieldName = "DistNumber";
		this.gridColumnEx10.Name = "gridColumnEx10";
		this.gridColumnEx10.OptionsColumn.ReadOnly = true;
		this.gridColumnEx10.PrimaryKey = false;
		this.gridColumnEx10.UnitField = null;
		this.gridColumnEx10.Visible = true;
		this.gridColumnEx10.VisibleIndex = 0;
		this.gridColumnEx10.Width = 141;
		this.gridColumnEx11.Caption = "From Bin";
		this.gridColumnEx11.CurrencyDateField = null;
		this.gridColumnEx11.CurrencyField = null;
		this.gridColumnEx11.CurrencyFixed = null;
		this.gridColumnEx11.CurrencyFixedRate = 0.0;
		this.gridColumnEx11.CurrencyRateField = null;
		this.gridColumnEx11.FieldName = "FromBinCode";
		this.gridColumnEx11.Name = "gridColumnEx11";
		this.gridColumnEx11.OptionsColumn.ReadOnly = true;
		this.gridColumnEx11.PrimaryKey = false;
		this.gridColumnEx11.UnitField = null;
		this.gridColumnEx11.Visible = true;
		this.gridColumnEx11.VisibleIndex = 1;
		this.gridColumnEx11.Width = 120;
		this.gridColumnEx13.Caption = "Quantity";
		this.gridColumnEx13.CurrencyDateField = null;
		this.gridColumnEx13.CurrencyField = null;
		this.gridColumnEx13.CurrencyFixed = null;
		this.gridColumnEx13.CurrencyFixedRate = 0.0;
		this.gridColumnEx13.CurrencyRateField = null;
		this.gridColumnEx13.FieldName = "Quantity";
		this.gridColumnEx13.Name = "gridColumnEx13";
		this.gridColumnEx13.OptionsColumn.ReadOnly = true;
		this.gridColumnEx13.PrimaryKey = false;
		this.gridColumnEx13.UnitField = null;
		this.gridColumnEx13.Visible = true;
		this.gridColumnEx13.VisibleIndex = 2;
		this.gridColumnEx14.Caption = "To Bin";
		this.gridColumnEx14.CurrencyDateField = null;
		this.gridColumnEx14.CurrencyField = null;
		this.gridColumnEx14.CurrencyFixed = null;
		this.gridColumnEx14.CurrencyFixedRate = 0.0;
		this.gridColumnEx14.CurrencyRateField = null;
		this.gridColumnEx14.FieldName = "ToBinCode";
		this.gridColumnEx14.Name = "gridColumnEx14";
		this.gridColumnEx14.OptionsColumn.ReadOnly = true;
		this.gridColumnEx14.PrimaryKey = false;
		this.gridColumnEx14.UnitField = null;
		this.gridColumnEx14.Visible = true;
		this.gridColumnEx14.VisibleIndex = 3;
		this.gridColumnEx15.Caption = "Doc Entry";
		this.gridColumnEx15.CurrencyDateField = null;
		this.gridColumnEx15.CurrencyField = null;
		this.gridColumnEx15.CurrencyFixed = null;
		this.gridColumnEx15.CurrencyFixedRate = 0.0;
		this.gridColumnEx15.CurrencyRateField = null;
		this.gridColumnEx15.FieldName = "DocEntry";
		this.gridColumnEx15.Name = "gridColumnEx15";
		this.gridColumnEx15.OptionsColumn.ReadOnly = true;
		this.gridColumnEx15.PrimaryKey = false;
		this.gridColumnEx15.UnitField = null;
		this.gridColumnEx16.Caption = "Line Num";
		this.gridColumnEx16.CurrencyDateField = null;
		this.gridColumnEx16.CurrencyField = null;
		this.gridColumnEx16.CurrencyFixed = null;
		this.gridColumnEx16.CurrencyFixedRate = 0.0;
		this.gridColumnEx16.CurrencyRateField = null;
		this.gridColumnEx16.FieldName = "LineNum";
		this.gridColumnEx16.Name = "gridColumnEx16";
		this.gridColumnEx16.OptionsColumn.ReadOnly = true;
		this.gridColumnEx16.PrimaryKey = false;
		this.gridColumnEx16.UnitField = null;
		this.btn_SaveTransfer.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_SaveTransfer.EditControl = null;
		this.btn_SaveTransfer.EditValue = "Find";
		this.btn_SaveTransfer.FieldName = null;
		this.btn_SaveTransfer.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_SaveTransfer.ImageOptions.Image");
		this.btn_SaveTransfer.IsSearchControl = false;
		this.btn_SaveTransfer.Location = new System.Drawing.Point(10, 466);
		this.btn_SaveTransfer.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.btn_SaveTransfer.Name = "btn_SaveTransfer";
		this.btn_SaveTransfer.ReadOnly = false;
		this.btn_SaveTransfer.SettingInVisible = false;
		this.btn_SaveTransfer.SettingReadonly = false;
		this.btn_SaveTransfer.Size = new System.Drawing.Size(75, 24);
		this.btn_SaveTransfer.TabIndex = 50;
		this.btn_SaveTransfer.Text = "Find";
		this.btn_SaveTransfer.Click += new System.EventHandler(this.btn_SaveTransfer_Click);
		this.xPage_ReportComplete.Controls.Add(this.date_RcPostDate);
		this.xPage_ReportComplete.Controls.Add(this.labelLine2);
		this.xPage_ReportComplete.Controls.Add(this.btn_Barcode);
		this.xPage_ReportComplete.Controls.Add(this.btn_SaveRC);
		this.xPage_ReportComplete.Controls.Add(this.grdEx_RC);
		this.xPage_ReportComplete.EditControl = null;
		this.xPage_ReportComplete.EditValue = "Report Complete";
		this.xPage_ReportComplete.FieldName = "xPage_ReportComplete";
		this.xPage_ReportComplete.IsSearchControl = false;
		this.xPage_ReportComplete.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.xPage_ReportComplete.Name = "xPage_ReportComplete";
		this.xPage_ReportComplete.ReadOnly = false;
		this.xPage_ReportComplete.SettingInVisible = false;
		this.xPage_ReportComplete.SettingReadonly = false;
		this.xPage_ReportComplete.Size = new System.Drawing.Size(964, 498);
		this.xPage_ReportComplete.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xPage_ReportComplete.Text = "Report Complete";
		this.date_RcPostDate.EditControl = null;
		this.date_RcPostDate.EditValue = null;
		this.date_RcPostDate.FieldName = null;
		this.date_RcPostDate.Location = new System.Drawing.Point(91, 5);
		this.date_RcPostDate.Name = "date_RcPostDate";
		this.date_RcPostDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_RcPostDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_RcPostDate.Properties.IsSearchControl = false;
		this.date_RcPostDate.SettingInVisible = false;
		this.date_RcPostDate.SettingReadonly = false;
		this.date_RcPostDate.Size = new System.Drawing.Size(100, 20);
		this.date_RcPostDate.TabIndex = 56;
		this.date_RcPostDate.EditValueChanged += new System.EventHandler(this.date_RcPostDate_EditValueChanged);
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "Posting Date";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(10, 8);
		this.labelLine2.Margin = new System.Windows.Forms.Padding(4);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(99, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 55;
		this.labelLine2.Utext = "Posting Date";
		this.btn_Barcode.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Barcode.EditControl = null;
		this.btn_Barcode.EditValue = "Barcode";
		this.btn_Barcode.FieldName = null;
		this.btn_Barcode.IsSearchControl = false;
		this.btn_Barcode.Location = new System.Drawing.Point(99, 466);
		this.btn_Barcode.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.btn_Barcode.Name = "btn_Barcode";
		this.btn_Barcode.ReadOnly = false;
		this.btn_Barcode.SettingInVisible = false;
		this.btn_Barcode.SettingReadonly = false;
		this.btn_Barcode.Size = new System.Drawing.Size(75, 24);
		this.btn_Barcode.TabIndex = 54;
		this.btn_Barcode.Text = "Barcode";
		this.btn_Barcode.Click += new System.EventHandler(this.btn_Barcode_Click);
		this.btn_SaveRC.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_SaveRC.EditControl = null;
		this.btn_SaveRC.EditValue = "Find";
		this.btn_SaveRC.FieldName = null;
		this.btn_SaveRC.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_SaveRC.ImageOptions.Image");
		this.btn_SaveRC.IsSearchControl = false;
		this.btn_SaveRC.Location = new System.Drawing.Point(10, 466);
		this.btn_SaveRC.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.btn_SaveRC.Name = "btn_SaveRC";
		this.btn_SaveRC.ReadOnly = false;
		this.btn_SaveRC.SettingInVisible = false;
		this.btn_SaveRC.SettingReadonly = false;
		this.btn_SaveRC.Size = new System.Drawing.Size(75, 24);
		this.btn_SaveRC.TabIndex = 50;
		this.btn_SaveRC.Text = "Find";
		this.btn_SaveRC.Click += new System.EventHandler(this.btn_SaveRC_Click);
		this.grdEx_RC.AllowPasteExcel = false;
		this.grdEx_RC.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grdEx_RC.ClipboardData = null;
		this.grdEx_RC.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grdEx_RC.Location = new System.Drawing.Point(10, 30);
		this.grdEx_RC.MainView = this.gvEx_RC;
		this.grdEx_RC.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grdEx_RC.Name = "grdEx_RC";
		this.grdEx_RC.Size = new System.Drawing.Size(906, 424);
		this.grdEx_RC.TabIndex = 49;
		this.grdEx_RC.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvEx_RC });
		this.grdEx_RC.EditorKeyDown += new System.Windows.Forms.KeyEventHandler(this.grdEx_RC_EditorKeyDown);
		this.gvEx_RC.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvEx_RC.ColumnKey = null;
		this.gvEx_RC.DisableFormSetting = false;
		this.gvEx_RC.DisplayException = false;
		this.gvEx_RC.EditColumn = null;
		this.gvEx_RC.FieldCurrency = null;
		this.gvEx_RC.FieldRate = null;
		this.gvEx_RC.FormId = "oProductionOrderImportTool";
		this.gvEx_RC.GridControl = this.grdEx_RC;
		this.gvEx_RC.IgnoreFilter = false;
		this.gvEx_RC.IndexEditRow = -1;
		this.gvEx_RC.IndicatorWidth = 70;
		this.gvEx_RC.IsApzTable = false;
		this.gvEx_RC.IsInsertNewRow = false;
		this.gvEx_RC.IsLoadingData = false;
		this.gvEx_RC.IsMappingContextAction = false;
		this.gvEx_RC.ItemId = "ORERORT";
		this.gvEx_RC.LstColumnAllowBatch = null;
		this.gvEx_RC.LstColumnNotAllowBatch = null;
		this.gvEx_RC.LstColumnNotEditable = null;
		this.gvEx_RC.LstColumNotSetting = null;
		this.gvEx_RC.Name = "gvEx_RC";
		this.gvEx_RC.OptionsFind.AllowFindPanel = false;
		this.gvEx_RC.OptionsSelection.MultiSelect = true;
		this.gvEx_RC.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
		this.gvEx_RC.OptionsView.ColumnAutoWidth = false;
		this.gvEx_RC.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gvEx_RC.OptionsView.ShowGroupPanel = false;
		this.gvEx_RC.UseBatchMod = false;
		this.gvEx_RC.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gvEx_RC_RowCellStyle);
		this.gvEx_RC.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.gridView_RowStyle);
		this.gvEx_RC.SelectionChanged += new DevExpress.Data.SelectionChangedEventHandler(this.gridView_SelectionChanged);
		this.gvEx_RC.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gvEx_RC_ShowingEditor);
		this.gvEx_RC.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvEx_RC_CellValueChanged);
		this.xPage_CloseProduction.Controls.Add(this.btn_SaveClose);
		this.xPage_CloseProduction.Controls.Add(this.grdEx_Close);
		this.xPage_CloseProduction.EditControl = null;
		this.xPage_CloseProduction.EditValue = "Close Production";
		this.xPage_CloseProduction.FieldName = "xPage_CloseProduction";
		this.xPage_CloseProduction.IsSearchControl = false;
		this.xPage_CloseProduction.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.xPage_CloseProduction.Name = "xPage_CloseProduction";
		this.xPage_CloseProduction.ReadOnly = false;
		this.xPage_CloseProduction.SettingInVisible = false;
		this.xPage_CloseProduction.SettingReadonly = false;
		this.xPage_CloseProduction.Size = new System.Drawing.Size(964, 498);
		this.xPage_CloseProduction.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xPage_CloseProduction.Text = "Close Production";
		this.btn_SaveClose.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_SaveClose.EditControl = null;
		this.btn_SaveClose.EditValue = "Find";
		this.btn_SaveClose.FieldName = null;
		this.btn_SaveClose.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_SaveClose.ImageOptions.Image");
		this.btn_SaveClose.IsSearchControl = false;
		this.btn_SaveClose.Location = new System.Drawing.Point(10, 462);
		this.btn_SaveClose.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.btn_SaveClose.Name = "btn_SaveClose";
		this.btn_SaveClose.ReadOnly = false;
		this.btn_SaveClose.SettingInVisible = false;
		this.btn_SaveClose.SettingReadonly = false;
		this.btn_SaveClose.Size = new System.Drawing.Size(75, 24);
		this.btn_SaveClose.TabIndex = 50;
		this.btn_SaveClose.Text = "Find";
		this.btn_SaveClose.Click += new System.EventHandler(this.btn_SaveClose_Click);
		this.grdEx_Close.AllowPasteExcel = false;
		this.grdEx_Close.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grdEx_Close.ClipboardData = null;
		this.grdEx_Close.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grdEx_Close.Location = new System.Drawing.Point(10, 2);
		this.grdEx_Close.MainView = this.gvEx_Close;
		this.grdEx_Close.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grdEx_Close.Name = "grdEx_Close";
		this.grdEx_Close.Size = new System.Drawing.Size(904, 448);
		this.grdEx_Close.TabIndex = 49;
		this.grdEx_Close.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvEx_Close });
		this.gvEx_Close.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvEx_Close.ColumnKey = null;
		this.gvEx_Close.DisableFormSetting = false;
		this.gvEx_Close.DisplayException = false;
		this.gvEx_Close.EditColumn = null;
		this.gvEx_Close.FieldCurrency = null;
		this.gvEx_Close.FieldRate = null;
		this.gvEx_Close.FormId = "oProductionOrderImportTool";
		this.gvEx_Close.GridControl = this.grdEx_Close;
		this.gvEx_Close.IgnoreFilter = false;
		this.gvEx_Close.IndexEditRow = -1;
		this.gvEx_Close.IndicatorWidth = 70;
		this.gvEx_Close.IsApzTable = false;
		this.gvEx_Close.IsInsertNewRow = false;
		this.gvEx_Close.IsLoadingData = false;
		this.gvEx_Close.IsMappingContextAction = false;
		this.gvEx_Close.ItemId = "OCLOSE";
		this.gvEx_Close.LstColumnAllowBatch = null;
		this.gvEx_Close.LstColumnNotAllowBatch = null;
		this.gvEx_Close.LstColumnNotEditable = null;
		this.gvEx_Close.LstColumNotSetting = null;
		this.gvEx_Close.Name = "gvEx_Close";
		this.gvEx_Close.OptionsFind.AllowFindPanel = false;
		this.gvEx_Close.OptionsSelection.MultiSelect = true;
		this.gvEx_Close.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
		this.gvEx_Close.OptionsView.ColumnAutoWidth = false;
		this.gvEx_Close.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gvEx_Close.OptionsView.ShowGroupPanel = false;
		this.gvEx_Close.UseBatchMod = false;
		this.gvEx_Close.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gvEx_Close_RowCellStyle);
		this.gvEx_Close.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.gridView_RowStyle);
		this.gvEx_Close.SelectionChanged += new DevExpress.Data.SelectionChangedEventHandler(this.gridView_SelectionChanged);
		this.gvEx_Close.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gvEx_Close_ShowingEditor);
		this.gvEx_Close.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvEx_Close_CellValueChanged);
		this.txt_Code.EditControl = null;
		this.txt_Code.FieldName = null;
		this.txt_Code.IsSearchControl = false;
		this.txt_Code.Location = new System.Drawing.Point(97, 10);
		this.txt_Code.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.txt_Code.Name = "txt_Code";
		this.txt_Code.SettingInVisible = false;
		this.txt_Code.SettingReadonly = false;
		this.txt_Code.Size = new System.Drawing.Size(170, 20);
		this.txt_Code.TabIndex = 0;
		this.txt_Code.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_Code_KeyDown);
		this.lb_Name.EditControl = null;
		this.lb_Name.EditValue = "Week No.";
		this.lb_Name.FieldName = null;
		this.lb_Name.HideWhenEmpty = false;
		this.lb_Name.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Name.Location = new System.Drawing.Point(5, 13);
		this.lb_Name.Margin = new System.Windows.Forms.Padding(4);
		this.lb_Name.Name = "lb_Name";
		this.lb_Name.ReadOnly = false;
		this.lb_Name.SettingInVisible = false;
		this.lb_Name.SettingReadonly = false;
		this.lb_Name.ShowUnderLine = true;
		this.lb_Name.Size = new System.Drawing.Size(99, 20);
		this.lb_Name.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Name.TabIndex = 1;
		this.lb_Name.Utext = "Week No.";
		this.bgw_FG.WorkerReportsProgress = true;
		this.bgw_FG.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgw_FG_DoWork);
		this.bgw_FG.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bgw_FG_ProgressChanged);
		this.bgw_FG.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgw_FG_RunWorkerCompleted);
		this.bgw_SaveTmpFg.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgw_SaveTmpFg_DoWork);
		this.bgw_SaveTmpFg.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgw_SaveTmpFg_RunWorkerCompleted);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(968, 559);
		base.Controls.Add(this.txt_Code);
		base.Controls.Add(this.lb_Name);
		base.Controls.Add(this.xtraTabControl1);
		base.FormId = "oProductionOrderImportTool";
		base.IconOptions.ShowIcon = false;
		base.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		base.Name = "frm_ProductionOrderImportTool";
		this.Text = "Production Order Import Tool";
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).EndInit();
		this.xtraTabControl1.ResumeLayout(false);
		this.xPage_ImportFinishGoods.ResumeLayout(false);
		this.xPage_ImportFinishGoods.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.calc_NoOfPrdAtime.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calc_FGInteruptTime.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grdEx_FinishGoods).EndInit();
		this.ctm_FG.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gvEx_FinishGoods).EndInit();
		this.xPage_FinishGoods.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_FindFg).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_FindFg).EndInit();
		this.xPage_Components.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdEx_Components).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gvEx_Components).EndInit();
		this.xPage_Transfer.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.date_InvTransferPostDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_InvTransferPostDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grdEx_Transfer).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gvEx_Transfer).EndInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl2).EndInit();
		this.xtraTabControl2.ResumeLayout(false);
		this.xtraTabPage1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdEx_TransferBin).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gvEx_TransferBin).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit1).EndInit();
		this.xtraTabPage2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdEx_TransferedBin).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gvEx_TransferedBin).EndInit();
		this.xPage_ReportComplete.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.date_RcPostDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_RcPostDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grdEx_RC).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gvEx_RC).EndInit();
		this.xPage_CloseProduction.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdEx_Close).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gvEx_Close).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Code.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
