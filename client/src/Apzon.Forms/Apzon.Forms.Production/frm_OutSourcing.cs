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
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Entities.Models.Documents;
using Apzon.Entities.Models.UserFields;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Forms.UserControls;
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
using SAPbobsCOM;

namespace Apzon.Forms.Production;

public class frm_OutSourcing : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private RepositoryItemButtonEditCFL rep_bte_itemCode_issue = new RepositoryItemButtonEditCFL();

	private RepositoryItemButtonEditCFL rep_bte_itemCode_receipt = new RepositoryItemButtonEditCFL();

	private RepositoryItemButtonEditCFL rep_bte_whsCode = new RepositoryItemButtonEditCFL();

	private RepositoryItemButtonEditCFL rep_bte_acctCode = new RepositoryItemButtonEditCFL();

	private RepositoryItemButtonEditCFL rep_bte_docEntry_issue = new RepositoryItemButtonEditCFL();

	private RepositoryItemButtonEditCFL rep_bte_docEntry_receipt = new RepositoryItemButtonEditCFL();

	private bool _isIssueRowObjectChanging;

	private bool _isIssueFocusedRowChanging;

	private bool _isReceiptRowObjectChanged;

	private bool _isSaving;

	private bool _isSetBinBatchData;

	private string[] _arrROnlyColIssueFind = new string[6] { "ItemCode", "ItemName", "WhsCode", "Quantity", "DocEntry", "ManBatchSer" };

	private string[] _arrROnlyColIssueAdd = new string[3] { "DocEntry", "LineNum", "ManBatchSer" };

	private string[] _arrROnlyColIssueAddBOM = new string[3] { "DocEntry", "LineNum", "ManBatchSer" };

	private string[] _arrROnlyColReceiptFind = new string[7] { "ItemCode", "ItemName", "WhsCode", "Quantity", "DocEntry", "AmountFactor", "ManBatchSer" };

	private string[] _arrROnlyColReceiptAdd = new string[3] { "DocEntry", "LineNum", "ManBatchSer" };

	private string[] _arrROnlyColIssueBatch = new string[7] { "DistNumber", "FromBinCode", "AvailQty", "LineNum", "BtSAbsEntry", "FromBinAbs", "FromBinActivat" };

	private string[] _arrROnlyColReceiptBatch = new string[3] { "LineNum", "BtSAbsEntry", "BinActivat" };

	private DataTable _DTIssueBinData = new DataTable();

	private DataTable _DTReceiptBinData = new DataTable();

	private List<Tuple<GridView, int>> _lstLastFocusedCol = new List<Tuple<GridView, int>>();

	private List<string> _binWarehouse = new List<string>();

	private int _docNum;

	private IContainer components;

	private TextEditEx txt_DocNum;

	private LabelLine labelLine3;

	private TextEditEx txt_Comments;

	private LabelLine labelLine1;

	private LabelLine labelLine2;

	private LabelLine labelLine4;

	private DateEditEx dateEx_DocDate;

	private SplitContainerControl splitContainerControl1;

	private GridControlEx grd_Issue;

	private GridViewEx gv_Issue;

	private GridControlEx grd_Receipt;

	private GridViewEx gv_Receipt;

	private SimpleButtonEx btn_Save;

	private SplitContainerControl splitContainerControl2;

	protected GridControlEx grd_BatchesIssue;

	protected GridViewEx gv_BatchesIssue;

	private SplitContainerControl splitContainerControl3;

	protected GridControlEx grd_BatchesReceipt;

	protected GridViewEx gv_BatchesReceipt;

	private RadioGroupEx rdg_InputMethod;

	private LabelLine labelLine5;

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

	private GridColumnEx gridColumnEx12;

	private RepositoryItemButtonEdit rep_bte_ToBinCode_Receipt;

	private GridColumnEx gridColumnEx13;

	private ButtonEditCFL bteCFL_Account;

	private ContextMenuStrip ctm_Issue;

	private ContextMenuStrip ctm_Receipt;

	private ContextMenuStrip ctm_IssueBatch;

	private ToolStripMenuItem removeRowToolStripMenuItem;

	private ToolStripMenuItem removeRowToolStripMenuItem1;

	private ToolStripMenuItem removeRowToolStripMenuItem2;

	private GridColumnEx gridColumnEx14;

	private GridColumnEx gridColumnEx15;

	private GridColumnEx gridColumnEx16;

	private GridColumnEx gridColumnEx17;

	private GridColumnEx gridColumnEx18;

	private GridColumnEx gridColumnEx19;

	private GridColumnEx gridColumnEx22;

	private GridColumnEx gridColumnEx23;

	private GridColumnEx gridColumnEx24;

	private GridColumnEx gridColumnEx25;

	private GridColumnEx gridColumnEx26;

	private GridColumnEx gridColumnEx27;

	private GridColumnEx gridColumnEx28;

	private GridColumnEx gridColumnEx20;

	private GridColumnEx gridColumnEx21;

	private GridColumnEx gridColumnEx29;

	private TextEditEx txt_JVNumber;

	private LabelLine labelLine6;

	private GridColumnEx gridColumnEx35;

	private GridColumnEx gridColumnEx41;

	private RepositoryItemButtonEditCFL rep_LineProject;

	private RepositoryItemButtonEditCFL rep_LineProject1;

	private LabelLine lbBranch;

	private LookUpEditCombo txt_Branch;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Find:
			btn_Save.Text = "Find";
			break;
		case Enums.FormViewModeType.Update:
			btn_Save.Text = "Save";
			break;
		case Enums.FormViewModeType.View:
			btn_Save.Text = "OK";
			break;
		case Enums.FormViewModeType.Add:
		case Enums.FormViewModeType.Busy:
		case Enums.FormViewModeType.Run:
			break;
		}
	}

	public frm_OutSourcing()
	{
		InitializeComponent();
		CreateRepositoryItem();
	}

	public frm_OutSourcing(int docNum)
	{
		InitializeComponent();
		CreateRepositoryItem();
		_docNum = docNum;
	}

	public async Task MODE_FIND()
	{
		try
		{
			bool flag = true;
			if (FormMode == Enums.FormViewModeType.Update)
			{
				flag = XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
			}
			if (flag)
			{
				FormMode = Enums.FormViewModeType.Find;
				grd_Issue.DataSource = null;
				grd_Issue.RefreshDataSource();
				grd_Receipt.DataSource = null;
				grd_Receipt.RefreshDataSource();
				rdg_InputMethod.Enabled = false;
				ShowHideRepositoryButton();
				_DTIssueBinData.Rows.Clear();
				_DTReceiptBinData.Rows.Clear();
				grd_BatchesIssue.DataSource = _DTIssueBinData.Clone();
				grd_BatchesIssue.RefreshDataSource();
				grd_BatchesReceipt.DataSource = _DTReceiptBinData.Clone();
				grd_BatchesReceipt.RefreshDataSource();
				grd_Issue.AllowPasteExcel = false;
				grd_Receipt.AllowPasteExcel = false;
				txt_DocNum.Focus();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task REFRESH()
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Update)
			{
				grd_Issue.AllowPasteExcel = true;
				grd_Receipt.AllowPasteExcel = true;
				rdg_InputMethod.Enabled = true;
				FormMode = Enums.FormViewModeType.Update;
				grd_Issue.DataSource = null;
				grd_Issue.RefreshDataSource();
				if (rdg_InputMethod.SelectedIndex != 1 && gv_Receipt.Columns["AmountFactor"] != null)
				{
					gv_Receipt.Columns["AmountFactor"].Visible = false;
				}
				rep_bte_itemCode_receipt.FindCFLBy(Enums.TypeCFL.ListItems, true, "", Enums.ItemObjectType.NoItemType, null);
				AddNewRow("I");
				_DTIssueBinData.Rows.Clear();
				_DTReceiptBinData.Rows.Clear();
				grd_BatchesIssue.DataSource = _DTIssueBinData.Clone();
				grd_BatchesIssue.RefreshDataSource();
				grd_BatchesReceipt.DataSource = _DTReceiptBinData.Clone();
				grd_BatchesReceipt.RefreshDataSource();
				grd_Receipt.DataSource = null;
				grd_Receipt.RefreshDataSource();
				AddNewRow("R");
				ShowHideRepositoryButton();
				splitContainerControl2.PanelVisibility = SplitPanelVisibility.Panel1;
				splitContainerControl3.PanelVisibility = SplitPanelVisibility.Panel1;
				grd_Issue.AllowPasteExcel = true;
				grd_Receipt.AllowPasteExcel = true;
				txt_DocNum.Text = "";
				txt_Comments.Text = "";
				dateEx_DocDate.EditValue = null;
				bteCFL_Account.EditValue = "";
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
		try
		{
			await GetRecord("F", 0);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task MOVE_NEXT()
	{
		_ = 2;
		try
		{
			int code = Function.ParseInt(txt_DocNum.Text);
			bool flag = code > 0;
			if (flag)
			{
				flag = await _httpClient.OutsourcingClient.ExistsCode(code);
			}
			if (flag)
			{
				await GetRecord("N", code);
			}
			else
			{
				await GetRecord("L", 0);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task MOVE_LAST()
	{
		try
		{
			await GetRecord("L", 0);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task MOVE_PREVIOUS()
	{
		_ = 2;
		try
		{
			int code = Function.ParseInt(txt_DocNum.Text);
			bool flag = code > 0;
			if (flag)
			{
				flag = await _httpClient.OutsourcingClient.ExistsCode(code);
			}
			if (flag)
			{
				await GetRecord("P", code);
			}
			else
			{
				await GetRecord("F", 0);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task GetDataByDocNum(int docNum)
	{
		_ = 1;
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add(new DataColumn("DocNum", typeof(int)));
			dataTable.Columns.Add(new DataColumn("Comments", typeof(string)));
			dataTable.Columns.Add(new DataColumn("DocDate", typeof(string)));
			DataRow dataRow = dataTable.NewRow();
			dataRow["DocNum"] = docNum;
			dataRow["Comments"] = "";
			dataRow["DocDate"] = DBNull.Value;
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			DataTable dataTable2 = await _httpClient.OutsourcingClient.Search(dataTable);
			if (dataTable2 != null && dataTable2.Rows.Count > 0)
			{
				await ShowData(dataTable2.Rows[0]);
			}
			else
			{
				WinCommons.Warning(Enums.MessageCode.InvalidDocObjectCode, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex.ToString());
			WinCommons.Error(Enums.MessageCode.Error, "Could not get data.");
		}
	}

	private async void frm_OutSourcing_Load(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			List<Task> list = new List<Task> { Task.Run(async () =>
			{
				DataTable dataTable = await _httpClient.ProductionOrderImportTool.GetBinWarehouse();
				if (dataTable != null && dataTable.Rows.Count > 0)
				{
					foreach (DataRow row in dataTable.Rows)
					{
						_binWarehouse.Add(Function.ToString(row[0]).ToLower());
					}
				}
			}) };
			frm_OutSourcing frm_OutSourcing2 = this;
			GridViewEx gridViewEx = gv_Issue;
			GridViewEx gridViewEx2 = gv_Receipt;
			GridViewEx gridViewEx3 = gv_BatchesIssue;
			string text = (gv_BatchesReceipt.FormId = "oOutsourcing");
			string text3 = (gridViewEx3.FormId = text);
			string text5 = (gridViewEx2.FormId = text3);
			string formId = (gridViewEx.FormId = text5);
			frm_OutSourcing2.FormId = formId;
			list.Add(Task.Run(() =>
			{
				_DTIssueBinData = _httpClient.OutsourcingClient.GetAllBinData("-1", "");
				_DTIssueBinData.Rows.Clear();
				_DTIssueBinData.AcceptChanges();
			}));
			list.Add(Task.Run(() =>
			{
				_DTReceiptBinData = _httpClient.OutsourcingClient.GetAllBinData("-2", "");
				_DTReceiptBinData.Rows.Clear();
				_DTReceiptBinData.AcceptChanges();
			}));
			rep_bte_itemCode_issue.FindCFLBy(Enums.TypeCFL.ListItems, true, "", Enums.ItemObjectType.NoItemType, null);
			rep_bte_itemCode_receipt.FindCFLBy(Enums.TypeCFL.ListItems, true, "", Enums.ItemObjectType.NoItemType, null);
			rep_bte_acctCode.FindCFLBy(Enums.TypeCFL.ChartOfAccount, false, "", Enums.ItemObjectType.NoItemType, null);
			rep_bte_whsCode.FindCFLBy(Enums.TypeCFL.GetListWareHouse, false, "", Enums.ItemObjectType.NoItemType, null);
			grd_Issue.AllowPasteExcel = false;
			grd_Receipt.AllowPasteExcel = false;
			_lstLastFocusedCol.AddRange(new List<Tuple<GridView, int>>
			{
				new Tuple<GridView, int>(gv_Issue, gv_Issue.FocusedRowHandle),
				new Tuple<GridView, int>(gv_Receipt, gv_Receipt.FocusedRowHandle)
			});
			list.Add(CreateColumnsDistRule(gv_Issue));
			list.Add(CreateColumnsDistRule(gv_Receipt));
			list.Add(SetDataBranch());
			Dictionary<string, GridViewEx> tblFormular = new Dictionary<string, GridViewEx>
			{
				["IGE1"] = gv_Issue,
				["IGN1"] = gv_Receipt
			};
			list.Add(WinFuntions.LoadUdfColumns(gv_Issue, "IGE1", tblFormular));
			list.Add(WinFuntions.LoadUdfColumns(gv_Receipt, "IGN1", tblFormular));
			await Task.WhenAll(list);
			if (_docNum > 0)
			{
				await GetDataByDocNum(_docNum);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rdg_InputMethod_SelectedIndexChanged(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Update && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				grd_Issue.DataSource = null;
				grd_Issue.RefreshDataSource();
				if (rdg_InputMethod.SelectedIndex != 1 && gv_Receipt.Columns["AmountFactor"] != null)
				{
					gv_Receipt.Columns["AmountFactor"].Visible = false;
				}
				rep_bte_itemCode_receipt.FindCFLBy(Enums.TypeCFL.ListItems, true, "", Enums.ItemObjectType.NoItemType, null);
				AddNewRow("I");
				grd_Receipt.DataSource = null;
				grd_Receipt.RefreshDataSource();
				AddNewRow("R");
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
		if (FormMode == Enums.FormViewModeType.Update)
		{
			RemoveRow(gv_Issue);
		}
	}

	private void removeRowToolStripMenuItem1_Click(object sender, EventArgs e)
	{
		RemoveRow(gv_Receipt);
	}

	private void removeRowToolStripMenuItem2_Click(object sender, EventArgs e)
	{
		RemoveRow(gv_BatchesReceipt);
	}

	private void txt_DocNum_KeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.Return && FormMode == Enums.FormViewModeType.Find)
			{
				btn_Save_Click(null, null);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void splitContainerControl_Resize(object sender, EventArgs e)
	{
		try
		{
			if (sender is SplitContainerControl splitContainerControl)
			{
				splitContainerControl.SplitterPosition = (splitContainerControl.Horizontal ? (splitContainerControl1.Width / 2) : (splitContainerControl1.Height / 2));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_BeforeLeaveRow(object sender, RowAllowEventArgs e)
	{
		try
		{
			GridView gv = sender as GridView;
			if (gv != null)
			{
				Tuple<GridView, int> tuple = _lstLastFocusedCol.FirstOrDefault((Tuple<GridView, int> t) => t.Item1.Name == gv.Name);
				if (tuple != null)
				{
					_lstLastFocusedCol.Remove(tuple);
					_lstLastFocusedCol.Add(new Tuple<GridView, int>(gv, gv.FocusedRowHandle));
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_BatchesIssue_InvalidValueException(object sender, InvalidValueExceptionEventArgs e)
	{
		try
		{
			e.ExceptionMode = ExceptionMode.DisplayError;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_BatchesIssue_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
	{
		ValidatingSnBGridQuantity(gv_BatchesIssue, gv_Issue, e);
	}

	private void gv_BatchesReceipt_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
	{
		try
		{
			ValidatingSnBGridQuantity(gv_BatchesReceipt, gv_Receipt, e);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_RowStyle(object sender, RowStyleEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Find)
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

	private void gv_Issue_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		SetRowCellStyle(e);
	}

	private void gv_Receipt_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		SetRowCellStyle(e);
	}

	private void gv_BatchesIssue_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		SetRowCellStyle(e);
	}

	private void gv_Issue_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Update)
			{
				return;
			}
			GridViewEx gridViewEx = sender as GridViewEx;
			if (e.Column.FieldName == "ItemCode")
			{
				ResetLineNum();
				DataTable itemDetails = _httpClient.OutsourcingClient.GetItemDetails(Function.ToString(e.Value));
				if (itemDetails.IsNotNull())
				{
					gridViewEx.SetRowCellValue(e.RowHandle, "ItemName", Function.ToString(itemDetails.Rows[0]["ItemName"]));
					gridViewEx.SetRowCellValue(e.RowHandle, "WhsCode", Function.ToString(itemDetails.Rows[0]["WhsCode"]));
					string text = Function.ToString(itemDetails.Rows[0]["ManBtchNum"]);
					string text2 = Function.ToString(itemDetails.Rows[0]["ManSerNum"]);
					if (text == "Y")
					{
						gridViewEx.SetRowCellValue(e.RowHandle, "ManBatchSer", "B");
					}
					else if (text2 == "Y")
					{
						gridViewEx.SetRowCellValue(e.RowHandle, "ManBatchSer", "S");
					}
					else
					{
						gridViewEx.SetRowCellValue(e.RowHandle, "ManBatchSer", "N");
					}
					AddBinBatchDataTableIssue(e.RowHandle);
					AddNewRow("I");
				}
			}
			if (e.Column.FieldName == "WhsCode")
			{
				AddBinBatchDataTableIssue(e.RowHandle, changeWarehouse: true);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Receipt_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Update)
			{
				return;
			}
			GridViewEx gridViewEx = sender as GridViewEx;
			int num = Function.ParseInt(gridViewEx.GetRowCellValue(e.RowHandle, "LineNum"));
			if (e.Column.FieldName == "ItemCode")
			{
				DataTable itemDetails = _httpClient.OutsourcingClient.GetItemDetails(Function.ToString(e.Value));
				if (itemDetails.IsNotNull())
				{
					gridViewEx.SetRowCellValue(e.RowHandle, "ItemName", Function.ToString(itemDetails.Rows[0]["ItemName"]));
					gridViewEx.SetRowCellValue(e.RowHandle, "AmountFactor", Function.ParseDouble(itemDetails.Rows[0]["AmountFactor"]));
					gridViewEx.SetRowCellValue(e.RowHandle, "WhsCode", Function.ToString(itemDetails.Rows[0]["WhsCode"]));
					if (rdg_InputMethod.SelectedIndex == 0)
					{
						ReLoadGridIssue(Function.ToString(e.Value), Function.ParseDouble(gridViewEx.GetFocusedRowCellValue("Quantity")), "ItemCode");
					}
					string text = Function.ToString(itemDetails.Rows[0]["ManBtchNum"]);
					string text2 = Function.ToString(itemDetails.Rows[0]["ManSerNum"]);
					if (text == "Y")
					{
						gridViewEx.SetRowCellValue(e.RowHandle, "ManBatchSer", "B");
					}
					else if (text2 == "Y")
					{
						gridViewEx.SetRowCellValue(e.RowHandle, "ManBatchSer", "S");
					}
					else
					{
						gridViewEx.SetRowCellValue(e.RowHandle, "ManBatchSer", "N");
					}
					grd_BatchesReceipt.DataSource = _DTReceiptBinData.Clone();
					grd_BatchesReceipt.RefreshDataSource();
					if (text == "Y" || text2 == "Y")
					{
						ShowBatchData(gv_Receipt, _DTReceiptBinData, grd_BatchesReceipt, splitContainerControl3);
						AddNewRowBatchReceipt();
					}
					else
					{
						grd_BatchesReceipt.DataSource = _DTReceiptBinData.Clone();
					}
					AddNewRow("R");
					DataRow[] array = _DTReceiptBinData.Select("LineNum=" + num);
					if (array.Length != 0)
					{
						for (int i = 0; i < array.Length; i++)
						{
							_DTReceiptBinData.Rows.Remove(array[i]);
						}
						_DTReceiptBinData.AcceptChanges();
					}
				}
			}
			if (e.Column.FieldName == "Quantity" && rdg_InputMethod.SelectedIndex == 0)
			{
				ReLoadGridIssue(Function.ToString(gridViewEx.GetFocusedRowCellValue("ItemCode")), Function.ParseDouble(e.Value), "Quantity");
			}
			if (!(e.Column.FieldName == "WhsCode"))
			{
				return;
			}
			grd_BatchesReceipt.DataSource = _DTReceiptBinData.Clone();
			grd_BatchesReceipt.RefreshDataSource();
			DataRow[] array2 = _DTIssueBinData.Select("LineNum=" + num);
			if (array2 != null && array2.Length != 0)
			{
				for (int j = 0; j < array2.Length; j++)
				{
					_DTIssueBinData.Rows.Remove(array2[j]);
				}
				_DTIssueBinData.AcceptChanges();
			}
			string text3 = Function.ToString(gridViewEx.GetRowCellValue(e.RowHandle, "ManBatchSer"));
			if (text3 == "B" || text3 == "S")
			{
				AddNewRowBatchReceipt();
			}
			ShowBatchData(gv_Receipt, _DTReceiptBinData, grd_BatchesReceipt, splitContainerControl3);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_BatchesReceipt_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			string text = Function.ToString(gv_Receipt.GetFocusedRowCellValue("ManBatchSer"));
			if (FormMode == Enums.FormViewModeType.Update && (text == "B" || text == "S" || _binWarehouse.Contains(Function.ToString(gv_Receipt.GetFocusedRowCellValue("WhsCode")).ToLower())))
			{
				AddNewRowBatchReceipt();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void gv_Issue_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		_isIssueRowObjectChanging = true;
		if (_isSaving)
		{
			_isIssueRowObjectChanging = false;
			return;
		}
		try
		{
			if (_isIssueFocusedRowChanging)
			{
				_isIssueRowObjectChanging = false;
				return;
			}
			Tuple<GridView, int> tuple = _lstLastFocusedCol.FirstOrDefault((Tuple<GridView, int> t) => t.Item1.Name == gv_Issue.Name);
			if (tuple != null)
			{
				await SetBinBatchData(gv_Issue, grd_BatchesIssue, _DTIssueBinData, tuple.Item2, splitContainerControl2);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		_isIssueRowObjectChanging = false;
	}

	private async void gv_Issue_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
	{
		_isIssueFocusedRowChanging = true;
		if (_isSaving)
		{
			_isIssueFocusedRowChanging = false;
			return;
		}
		try
		{
			if (_isIssueRowObjectChanging)
			{
				_isIssueFocusedRowChanging = false;
				return;
			}
			Tuple<GridView, int> tuple = _lstLastFocusedCol.FirstOrDefault((Tuple<GridView, int> t) => t.Item1.Name == gv_Issue.Name);
			if (tuple != null)
			{
				await SetBinBatchData(gv_Issue, grd_BatchesIssue, _DTIssueBinData, tuple.Item2, splitContainerControl2);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		_isIssueFocusedRowChanging = false;
	}

	private async void gv_Receipt_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		if (_isReceiptRowObjectChanged)
		{
			_isReceiptRowObjectChanged = false;
			return;
		}
		_isReceiptRowObjectChanged = true;
		if (_isSaving)
		{
			return;
		}
		try
		{
			Tuple<GridView, int> tuple = _lstLastFocusedCol.FirstOrDefault((Tuple<GridView, int> t) => t.Item1.Name == gv_Receipt.Name);
			if (tuple != null)
			{
				await SetBinBatchData(gv_Receipt, grd_BatchesReceipt, _DTReceiptBinData, tuple.Item2, splitContainerControl3);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void gv_Receipt_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
	{
		if (_isReceiptRowObjectChanged)
		{
			_isReceiptRowObjectChanged = false;
			return;
		}
		_isReceiptRowObjectChanged = true;
		if (_isSaving)
		{
			return;
		}
		try
		{
			Tuple<GridView, int> tuple = _lstLastFocusedCol.FirstOrDefault((Tuple<GridView, int> t) => t.Item1.Name == gv_Receipt.Name);
			if (tuple != null)
			{
				await SetBinBatchData(gv_Receipt, grd_BatchesReceipt, _DTReceiptBinData, tuple.Item2, splitContainerControl3);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rep_bte_whsCode_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Kind == ButtonPredefines.Glyph)
			{
				ButtonEdit buttonEdit = sender as ButtonEdit;
				this.DrillDown(ApzObjectTypes.oWarehouses, Function.ToString(buttonEdit.EditValue));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rep_bte_acctCode_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Kind == ButtonPredefines.Glyph)
			{
				ButtonEdit buttonEdit = sender as ButtonEdit;
				this.DrillDown(ApzObjectTypes.oChartOfAccounts, Function.ToString(buttonEdit.EditValue));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rep_bte_issue_itemCode_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Kind == ButtonPredefines.Glyph)
			{
				ButtonEdit buttonEdit = sender as ButtonEdit;
				this.DrillDown(ApzObjectTypes.oItems, Function.ToString(buttonEdit.EditValue));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rep_bte_itemCode_receipt_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Kind == ButtonPredefines.Glyph)
			{
				ButtonEdit buttonEdit = sender as ButtonEdit;
				this.DrillDown(ApzObjectTypes.oItems, Function.ToString(buttonEdit.EditValue));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rep_bte_docEntry_receipt_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Kind == ButtonPredefines.Glyph)
			{
				ButtonEdit buttonEdit = sender as ButtonEdit;
				this.DrillDown(ApzObjectTypes.oInventoryGenEntry, Function.ToString(buttonEdit.EditValue));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rep_bte_docEntry_issue_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Kind == ButtonPredefines.Glyph)
			{
				ButtonEdit buttonEdit = sender as ButtonEdit;
				this.DrillDown(ApzObjectTypes.oInventoryGenExit, Function.ToString(buttonEdit.EditValue));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rep_bte_FromBinCode_Receipt_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			string dataString = Function.ToString(gv_Receipt.GetFocusedRowCellValue("WhsCode"));
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.ListOfBinLocationByWareHouse, false, "", "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.DataString1 = dataString;
			frm_CFL2.ShowDialog(this);
			DataRow chooseItem = frm_CFL2.ChooseItem;
			if (chooseItem != null)
			{
				gv_BatchesReceipt.SetFocusedRowCellValue("ToBinAbs", Function.ParseInt(chooseItem["AbsEntry"]));
				gv_BatchesReceipt.SetFocusedRowCellValue("ToBinCode", Function.ToString(chooseItem["BinCode"]));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btn_Save_Click(object sender, EventArgs e)
	{
		btn_Save.Enabled = false;
		try
		{
			if (FormMode == Enums.FormViewModeType.Find)
			{
				await Search();
			}
			else if (FormMode == Enums.FormViewModeType.Update)
			{
				await Save();
			}
			else
			{
				Close();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		btn_Save.Enabled = true;
	}

	private void CreateRepositoryItem()
	{
		try
		{
			bteCFL_Account.Properties.ValueMember = "AcctCode";
			bteCFL_Account.Properties.UsingAutoCompleteBox = true;
			bteCFL_Account.Properties.FindCFLBy(Enums.TypeCFL.ChartOfAccount, false, "", Enums.ItemObjectType.NoItemType, null);
			rep_bte_itemCode_issue = new RepositoryItemButtonEditCFL();
			rep_bte_itemCode_issue.UsingAutoCompleteBox = true;
			rep_bte_itemCode_issue.ValueMember = "ItemCode";
			rep_bte_itemCode_issue.DisplayMember = "ItemCode";
			rep_bte_itemCode_issue.AutoLoadDataSource = false;
			rep_bte_itemCode_issue.ButtonPressed += rep_bte_issue_itemCode_ButtonPressed;
			rep_bte_itemCode_issue.HandleChooseItemMulti = ChooseItemIssueMulti;
			rep_bte_itemCode_receipt = new RepositoryItemButtonEditCFL();
			rep_bte_itemCode_receipt.UsingAutoCompleteBox = true;
			rep_bte_itemCode_receipt.ValueMember = "ItemCode";
			rep_bte_itemCode_receipt.DisplayMember = "ItemCode";
			rep_bte_itemCode_receipt.AutoLoadDataSource = false;
			rep_bte_itemCode_receipt.ButtonPressed += rep_bte_itemCode_receipt_ButtonPressed;
			rep_bte_itemCode_receipt.HandleChooseItemMulti = ChooseItemReceiptMulti;
			rep_bte_whsCode = new RepositoryItemButtonEditCFL();
			rep_bte_whsCode.ValueMember = "WhsCode";
			rep_bte_whsCode.DisplayMember = "WhsCode";
			rep_bte_whsCode.AutoLoadDataSource = false;
			rep_bte_whsCode.ButtonPressed += rep_bte_whsCode_ButtonPressed;
			rep_bte_acctCode = new RepositoryItemButtonEditCFL();
			rep_bte_acctCode.ValueMember = "AcctCode";
			rep_bte_acctCode.DisplayMember = "AcctCode";
			rep_bte_acctCode.AutoLoadDataSource = false;
			rep_bte_acctCode.ButtonPressed += rep_bte_acctCode_ButtonPressed;
			rep_bte_docEntry_issue = new RepositoryItemButtonEditCFL();
			rep_bte_docEntry_issue.Buttons[0].Visible = false;
			rep_bte_docEntry_issue.ButtonPressed += rep_bte_docEntry_issue_ButtonPressed;
			rep_bte_docEntry_receipt = new RepositoryItemButtonEditCFL();
			rep_bte_docEntry_receipt.Buttons[0].Visible = false;
			rep_bte_docEntry_receipt.ButtonPressed += rep_bte_docEntry_receipt_ButtonPressed;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ChooseItemReceiptMulti(List<DataRow> rows)
	{
		try
		{
			if (!(grd_Receipt.DataSource is DataTable dataTable) || dataTable.Rows.Count <= 0 || rows.Count <= 0)
			{
				return;
			}
			for (int i = 0; i < rows.Count; i++)
			{
				string code = Function.ToString(rows[i]["ItemCode"]);
				if (dataTable.AsEnumerable().Any((DataRow r) => Function.ToString(r["ItemCode"]) == code))
				{
					WinCommons.Error(Enums.MessageCode.ItemAlreadyChooseWithItemCode, "", code);
					return;
				}
			}
			gv_Receipt.SetFocusedRowCellValue("ItemCode", Function.ToString(rows[0]["ItemCode"]));
			for (int num = 1; num < rows.Count; num++)
			{
				DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow r) => string.IsNullOrWhiteSpace(Function.ToString(r["ItemCode"])));
				if (dataRow != null)
				{
					int dataSourceIndex = dataTable.Rows.IndexOf(dataRow);
					gv_Receipt.SetRowCellValue(gv_Receipt.GetRowHandle(dataSourceIndex), "ItemCode", Function.ToString(rows[num]["ItemCode"]));
					continue;
				}
				AddNewRow("R");
				DataRow row = dataTable.AsEnumerable().FirstOrDefault((DataRow r) => string.IsNullOrWhiteSpace(Function.ToString(r["ItemCode"])));
				int dataSourceIndex2 = dataTable.Rows.IndexOf(row);
				gv_Receipt.SetRowCellValue(gv_Receipt.GetRowHandle(dataSourceIndex2), "ItemCode", Function.ToString(rows[num]["ItemCode"]));
			}
			AddNewRow("I");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ChooseItemIssueMulti(List<DataRow> rows)
	{
		try
		{
			if (!(grd_Issue.DataSource is DataTable dataTable) || dataTable.Rows.Count <= 0 || rows.Count <= 0)
			{
				return;
			}
			gv_Issue.SetFocusedRowCellValue("ItemCode", Function.ToString(rows[0]["ItemCode"]));
			for (int i = 1; i < rows.Count; i++)
			{
				DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow r) => string.IsNullOrWhiteSpace(Function.ToString(r["ItemCode"])));
				if (dataRow != null)
				{
					int dataSourceIndex = dataTable.Rows.IndexOf(dataRow);
					gv_Issue.SetRowCellValue(gv_Issue.GetRowHandle(dataSourceIndex), "ItemCode", Function.ToString(rows[i]["ItemCode"]));
					continue;
				}
				AddNewRow("I");
				DataRow row = dataTable.AsEnumerable().FirstOrDefault((DataRow r) => string.IsNullOrWhiteSpace(Function.ToString(r["ItemCode"])));
				int dataSourceIndex2 = dataTable.Rows.IndexOf(row);
				gv_Issue.SetRowCellValue(gv_Issue.GetRowHandle(dataSourceIndex2), "ItemCode", Function.ToString(rows[i]["ItemCode"]));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task Search()
	{
		_ = 2;
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add(new DataColumn("DocNum", typeof(int)));
			dataTable.Columns.Add(new DataColumn("Comments", typeof(string)));
			dataTable.Columns.Add(new DataColumn("DocDate", typeof(string)));
			DataRow dataRow = dataTable.NewRow();
			dataRow["DocNum"] = Function.ParseInt(txt_DocNum.Text);
			dataRow["Comments"] = txt_Comments.Text;
			DateTime? dateTime = Function.ParseDateTime(dateEx_DocDate.EditValue);
			string value;
			if (dateTime.HasValue)
			{
				_ = dateTime.Value;
				value = dateTime.Value.ToString("yyyyMMdd");
			}
			else
			{
				value = "";
			}
			dataRow["DocDate"] = value;
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			grd_Issue.DataSource = null;
			grd_Issue.RefreshDataSource();
			grd_Receipt.DataSource = null;
			grd_Receipt.RefreshDataSource();
			DataTable dataTable2 = await _httpClient.OutsourcingClient.Search(dataTable);
			if (dataTable2 != null && dataTable2.Rows.Count > 0)
			{
				if (dataTable2.Rows.Count == 1)
				{
					await ShowData(dataTable2.Rows[0]);
				}
				else
				{
					frm_OutsourcingSearchCFL frm_OutsourcingSearchCFL2 = new frm_OutsourcingSearchCFL();
					frm_OutsourcingSearchCFL2.DTSource = dataTable2;
					frm_OutsourcingSearchCFL2.ShowDialog(this);
					if (frm_OutsourcingSearchCFL2.ChoosenRow != null)
					{
						await ShowData(frm_OutsourcingSearchCFL2.ChoosenRow);
					}
				}
			}
			SetRepositoryItemToGrid();
			SetReadonlyColumns();
			SetVisibleColumns();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task ShowData(DataRow row)
	{
		_ = 3;
		try
		{
			int docNum = Function.ParseInt(row["Code"]);
			string comments = Function.ToString(row["U_Comments"]);
			DateTime? docDate = Function.ParseDateTime(row["U_DocDate"]);
			string acctCode = Function.ToString(row["U_ClearAct"]);
			DataTable dtIssue = await _httpClient.OutsourcingClient.GetADoc(docNum, "I");
			DataTable dtReceipt = await _httpClient.OutsourcingClient.GetADoc(docNum, "R");
			txt_DocNum.Text = docNum.ToString();
			txt_Comments.Text = comments;
			dateEx_DocDate.EditValue = docDate;
			bteCFL_Account.EditValue = acctCode;
			grd_Issue.DataSource = dtIssue;
			grd_Issue.RefreshDataSource();
			if (dtIssue.IsNotNull())
			{
				DataTable dataTable = await _httpClient.OutsourcingClient.GetBinData(Function.ToString(docNum), 1, Function.ToString(gv_Issue.GetFocusedRowCellValue("ItemCode")));
				if (dataTable.IsNotNull())
				{
					grd_BatchesIssue.DataSource = dataTable;
					grd_BatchesIssue.RefreshDataSource();
					splitContainerControl2.PanelVisibility = SplitPanelVisibility.Both;
				}
			}
			grd_Receipt.DataSource = dtReceipt;
			grd_Receipt.RefreshDataSource();
			if (dtReceipt.IsNotNull())
			{
				DataTable dataTable2 = await _httpClient.OutsourcingClient.GetBinData(Function.ToString(docNum), 2, Function.ToString(gv_Receipt.GetFocusedRowCellValue("ItemCode")));
				if (dataTable2.IsNotNull())
				{
					grd_BatchesReceipt.DataSource = dataTable2;
					grd_BatchesReceipt.RefreshDataSource();
					splitContainerControl2.PanelVisibility = SplitPanelVisibility.Both;
				}
			}
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task Save()
	{
		_isSaving = true;
		try
		{
			if (!ValidateData())
			{
				_isSaving = false;
				return;
			}
			DataTable dtItemWhs = new DataTable();
			dtItemWhs.Columns.Add(new DataColumn("ItemCode", typeof(string)));
			dtItemWhs.Columns.Add(new DataColumn("WhsCode", typeof(string)));
			dtItemWhs.Columns.Add(new DataColumn("Quantity", typeof(double)));
			dtItemWhs.AcceptChanges();
			await SetBinBatchData(gv_Issue, grd_BatchesIssue, _DTIssueBinData, gv_Issue.FocusedRowHandle, splitContainerControl2);
			await SetBinBatchData(gv_Receipt, grd_BatchesReceipt, _DTReceiptBinData, gv_Receipt.FocusedRowHandle, splitContainerControl3);
			DataTable dataTable = _DTIssueBinData.Copy();
			for (int num = dataTable.Rows.Count - 1; num >= 0; num--)
			{
				DataRow dataRow = dataTable.Rows[num];
				if (Function.ParseDouble(dataRow["Quantity"]) <= 0.0)
				{
					dataTable.Rows.Remove(dataRow);
				}
			}
			dataTable.AcceptChanges();
			DataTable dtBatchReceipt = _DTReceiptBinData.Copy();
			for (int num2 = dtBatchReceipt.Rows.Count - 1; num2 >= 0; num2--)
			{
				DataRow dataRow2 = dtBatchReceipt.Rows[num2];
				if (Function.ParseDouble(dataRow2["Quantity"]) <= 0.0 || (string.IsNullOrWhiteSpace(Function.ToString(dataRow2["DistNumber"])) && Function.ToString(dataRow2["BinActivat"]) != "Y"))
				{
					dtBatchReceipt.Rows.Remove(dataRow2);
				}
			}
			dtBatchReceipt.AcceptChanges();
			DataTable dataTable2 = grd_Issue.DataSource as DataTable;
			dataTable2.AcceptChanges();
			DataTable dtIssue = dataTable2.Copy();
			for (int num3 = dtIssue.Rows.Count - 1; num3 >= 0; num3--)
			{
				DataRow dataRow3 = dtIssue.Rows[num3];
				if (string.IsNullOrWhiteSpace(Function.ToString(dataRow3["ItemCode"])))
				{
					dtIssue.Rows.Remove(dataRow3);
				}
			}
			dtIssue.AcceptChanges();
			DocumentsImpl issueDoc = new DocumentsImpl
			{
				DocType = "I",
				ObjType = 60
			};
			DateTime? date = Function.ParseDateTime(dateEx_DocDate.EditValue);
			DateTime docDate;
			if (date.HasValue)
			{
				_ = date.Value;
				docDate = date.Value;
			}
			else
			{
				docDate = DateTime.Today;
			}
			issueDoc.DocDate = docDate;
			issueDoc.Comments = Function.ToString(txt_Comments.Text);
			issueDoc.JrnlMemo = "Issued by Apz Outsourcing tool";
			if (GlobalData.IsMultiBranch)
			{
				issueDoc.BPLId = Function.ParseInt(txt_Branch.EditValue);
			}
			DataTable dtDoc = new DataTable();
			dtDoc.Columns.Add(new DataColumn("U_JVNumber", typeof(string)));
			dtDoc.Rows.Add(dtDoc.NewRow());
			dtDoc.Rows[0][0] = Function.ToString(txt_JVNumber.Text);
			dtDoc.AcceptChanges();
			issueDoc.UserFields = WinFuntions.GetUDFByTableName(dtDoc.Rows[0], "OIGE");
			List<Document_LinesImpl> list = new List<Document_LinesImpl>();
			int num4 = 0;
			for (int i = 0; i < dtIssue.Rows.Count; i++)
			{
				DataRow dataRow4 = dtIssue.Rows[i];
				Document_LinesImpl document_LinesImpl = new Document_LinesImpl();
				document_LinesImpl.ItemCode = Function.ToString(dataRow4["ItemCode"]);
				document_LinesImpl.Quantity = Function.ParseDouble(dataRow4["Quantity"]);
				document_LinesImpl.LineNum = num4;
				document_LinesImpl.BaseType = -1;
				document_LinesImpl.WhsCode = Function.ToString(dataRow4["WhsCode"]);
				document_LinesImpl.AcctCode = Function.ToString(bteCFL_Account.EditValue);
				document_LinesImpl.OcrCode = Function.ToString(dataRow4["OcrCode"]);
				document_LinesImpl.OcrCode2 = Function.ToString(dataRow4["OcrCode2"]);
				document_LinesImpl.OcrCode3 = Function.ToString(dataRow4["OcrCode3"]);
				document_LinesImpl.OcrCode4 = Function.ToString(dataRow4["OcrCode4"]);
				document_LinesImpl.OcrCode5 = Function.ToString(dataRow4["OcrCode5"]);
				document_LinesImpl.Project = Function.ToString(dataRow4["Project"]);
				document_LinesImpl.UserFields = WinFuntions.GetUDFByTableName(dataRow4, "IGE1");
				DataRow dataRow5 = dtItemWhs.NewRow();
				dataRow5["ItemCode"] = document_LinesImpl.ItemCode;
				dataRow5["WhsCode"] = document_LinesImpl.WhsCode;
				dataRow5["Quantity"] = document_LinesImpl.Quantity;
				dtItemWhs.Rows.Add(dataRow5);
				dtItemWhs.AcceptChanges();
				string text = Function.ToString(dataRow4["ManBatchSer"]);
				if (text == "B" || text == "S" || _binWarehouse.Contains(document_LinesImpl.WhsCode.ToLower()))
				{
					List<Tuple<string, double>> list2 = new List<Tuple<string, double>>();
					List<Tuple<string, double>> list3 = new List<Tuple<string, double>>();
					int baseLineNumber = Function.ParseInt(dataRow4["LineNum"]);
					DataRow[] binRows = dataTable.Select("LineNum=" + baseLineNumber);
					if (binRows != null && binRows.Length != 0)
					{
						int j;
						for (j = 0; j < binRows.Length; j++)
						{
							double num5 = Function.ParseDouble(binRows[j]["Quantity"]);
							if (text == "B")
							{
								Tuple<string, double> tuple = list2.FirstOrDefault((Tuple<string, double> b) => b.Item1 == Function.ToString(binRows[j]["DistNumber"]));
								if (tuple == null)
								{
									list2.Add(new Tuple<string, double>(Function.ToString(binRows[j]["DistNumber"]), num5));
								}
								else
								{
									double item = tuple.Item2 + num5;
									Tuple<string, double> item2 = new Tuple<string, double>(tuple.Item1, item);
									list2.Remove(tuple);
									list2.Add(item2);
								}
							}
							if (text == "S")
							{
								Tuple<string, double> tuple2 = list3.FirstOrDefault((Tuple<string, double> b) => b.Item1 == Function.ToString(binRows[j]["DistNumber"]));
								if (tuple2 == null)
								{
									list3.Add(new Tuple<string, double>(Function.ToString(binRows[j]["DistNumber"]), num5));
									continue;
								}
								double item3 = tuple2.Item2 + num5;
								Tuple<string, double> item4 = new Tuple<string, double>(tuple2.Item1, item3);
								list3.Remove(tuple2);
								list3.Add(item4);
							}
						}
					}
					List<BatchNumbersImpl> list4 = new List<BatchNumbersImpl>();
					for (int num6 = 0; num6 < list2.Count; num6++)
					{
						BatchNumbersImpl batchNumbersImpl = new BatchNumbersImpl();
						batchNumbersImpl.BatchNumber = list2[num6].Item1;
						batchNumbersImpl.Quantity = list2[num6].Item2;
						batchNumbersImpl.BaseLineNumber = i;
						list4.Add(batchNumbersImpl);
					}
					if (list4.Count > 0)
					{
						document_LinesImpl.BatchNumbers = list4;
					}
					List<SerialNumbersImpl> list5 = new List<SerialNumbersImpl>();
					for (int num7 = 0; num7 < list3.Count; num7++)
					{
						SerialNumbersImpl serialNumbersImpl = new SerialNumbersImpl();
						serialNumbersImpl.InternalSerialNumber = list3[num7].Item1;
						serialNumbersImpl.Quantity = 1.0;
						serialNumbersImpl.BaseLineNumber = baseLineNumber;
						list5.Add(serialNumbersImpl);
					}
					if (list5.Count > 0)
					{
						document_LinesImpl.SerialNumbers = list5;
					}
					List<DocumentLinesBinAllocationsImpl> list6 = new List<DocumentLinesBinAllocationsImpl>();
					if (binRows != null && binRows.Length != 0)
					{
						for (int num8 = 0; num8 < binRows.Length; num8++)
						{
							string distNumber = Function.ToString(binRows[num8]["DistNumber"]);
							int num9 = -1;
							if (text == "B")
							{
								num9 = list4.FindIndex((BatchNumbersImpl b) => b.BatchNumber == distNumber);
							}
							if (text == "S")
							{
								num9 = list5.FindIndex((SerialNumbersImpl b) => b.InternalSerialNumber == distNumber);
							}
							double quantity = Function.ParseDouble(binRows[num8]["Quantity"]);
							int binAbsEntry = Function.ParseInt(binRows[num8]["FromBinAbs"]);
							DocumentLinesBinAllocationsImpl documentLinesBinAllocationsImpl = new DocumentLinesBinAllocationsImpl();
							documentLinesBinAllocationsImpl.BaseLineNumber = i;
							documentLinesBinAllocationsImpl.BinAbsEntry = binAbsEntry;
							documentLinesBinAllocationsImpl.Quantity = quantity;
							if (num9 >= 0)
							{
								documentLinesBinAllocationsImpl.SerialAndBatchNumbersBaseLine = num9;
							}
							if (documentLinesBinAllocationsImpl.BinAbsEntry > 0)
							{
								list6.Add(documentLinesBinAllocationsImpl);
							}
						}
					}
					document_LinesImpl.BinAllocations = list6;
				}
				list.Add(document_LinesImpl);
				num4++;
			}
			issueDoc.Lines = list;
			double num10 = await _httpClient.OutsourcingClient.GetTotalAvgPrice(dtItemWhs);
			DataTable obj = grd_Receipt.DataSource as DataTable;
			obj.AcceptChanges();
			DataTable dataTable3 = obj.Copy();
			for (int num11 = dataTable3.Rows.Count - 1; num11 >= 0; num11--)
			{
				DataRow dataRow6 = dataTable3.Rows[num11];
				if (string.IsNullOrWhiteSpace(Function.ToString(dataRow6["ItemCode"])))
				{
					dataTable3.Rows.Remove(dataRow6);
				}
			}
			dataTable3.AcceptChanges();
			double num12 = dataTable3.AsEnumerable().Sum((DataRow r) => Function.ParseDouble(r["Quantity"]) * Function.ParseDouble(r["AmountFactor"]));
			DocumentsImpl documentsImpl = new DocumentsImpl();
			documentsImpl.DocType = "I";
			documentsImpl.ObjType = 59;
			DateTime docDate2;
			if (date.HasValue)
			{
				_ = date.Value;
				docDate2 = date.Value;
			}
			else
			{
				docDate2 = DateTime.Today;
			}
			documentsImpl.DocDate = docDate2;
			documentsImpl.Comments = Function.ToString(txt_Comments.Text);
			documentsImpl.JrnlMemo = "Apz Outsourcing";
			if (GlobalData.IsMultiBranch)
			{
				documentsImpl.BPLId = Function.ParseInt(txt_Branch.EditValue);
			}
			documentsImpl.UserFields = WinFuntions.GetUDFByTableName(dtDoc.Rows[0], "OIGN");
			List<Document_LinesImpl> list7 = new List<Document_LinesImpl>();
			num4 = 0;
			double num13 = 0.0;
			CurrencyCodesImpl currencyCodesImpl = GlobalData.CurrencyCodes.FirstOrDefault((CurrencyCodesImpl t) => t.CurrCode.ToUpper().Equals(GlobalData.MainCurrency));
			int digits = ((currencyCodesImpl.Decimals == -1) ? GlobalData.GeneralSetting.SumDec.GetValueOrDefault() : currencyCodesImpl.Decimals);
			for (int num14 = 0; num14 < dataTable3.Rows.Count; num14++)
			{
				DataRow dataRow7 = dataTable3.Rows[num14];
				Document_LinesImpl document_LinesImpl2 = new Document_LinesImpl();
				document_LinesImpl2.ItemCode = Function.ToString(dataRow7["ItemCode"]);
				document_LinesImpl2.Quantity = Function.ParseDouble(dataRow7["Quantity"]);
				document_LinesImpl2.LineNum = num4;
				document_LinesImpl2.BaseType = -1;
				document_LinesImpl2.WhsCode = Function.ToString(dataRow7["WhsCode"]);
				document_LinesImpl2.AcctCode = Function.ToString(bteCFL_Account.EditValue);
				document_LinesImpl2.SpecifiedPriceBefDi = true;
				if (num14 != dataTable3.Rows.Count - 1)
				{
					if (rdg_InputMethod.SelectedIndex == 0)
					{
						document_LinesImpl2.LineTotal = Math.Round(Function.ParseDouble((decimal)GetBOMPrice(document_LinesImpl2.ItemCode, dtIssue)), digits, MidpointRounding.AwayFromZero);
					}
					else
					{
						document_LinesImpl2.LineTotal = Math.Round(num10 * Function.ParseDouble(dataRow7["Quantity"]) * Function.ParseDouble(dataRow7["AmountFactor"]) / num12, digits, MidpointRounding.AwayFromZero);
					}
					num13 += document_LinesImpl2.LineTotal;
				}
				else
				{
					document_LinesImpl2.LineTotal = num10 - num13;
				}
				string text2 = Function.ToString(dataRow7["ManBatchSer"]);
				if (text2 == "B" || text2 == "S" || _binWarehouse.Contains(document_LinesImpl2.WhsCode.ToLower()))
				{
					List<Tuple<string, double>> list8 = new List<Tuple<string, double>>();
					List<Tuple<string, double>> list9 = new List<Tuple<string, double>>();
					DataRow[] binRows2 = dtBatchReceipt.Select("LineNum=" + Function.ParseInt(dataRow7["LineNum"]));
					if (binRows2 != null && binRows2.Length != 0)
					{
						int j2;
						for (j2 = 0; j2 < binRows2.Length; j2++)
						{
							double num15 = Function.ParseDouble(binRows2[j2]["Quantity"]);
							if (text2 == "B")
							{
								Tuple<string, double> tuple3 = list8.FirstOrDefault((Tuple<string, double> b) => b.Item1 == Function.ToString(binRows2[j2]["DistNumber"]));
								if (tuple3 == null)
								{
									list8.Add(new Tuple<string, double>(Function.ToString(binRows2[j2]["DistNumber"]), num15));
								}
								else
								{
									double item5 = tuple3.Item2 + num15;
									Tuple<string, double> item6 = new Tuple<string, double>(tuple3.Item1, item5);
									list8.Remove(tuple3);
									list8.Add(item6);
								}
							}
							if (text2 == "S")
							{
								Tuple<string, double> tuple4 = list9.FirstOrDefault((Tuple<string, double> b) => b.Item1 == Function.ToString(binRows2[j2]["DistNumber"]));
								if (tuple4 == null)
								{
									list9.Add(new Tuple<string, double>(Function.ToString(binRows2[j2]["DistNumber"]), num15));
									continue;
								}
								double item7 = tuple4.Item2 + num15;
								Tuple<string, double> item8 = new Tuple<string, double>(tuple4.Item1, item7);
								list9.Remove(tuple4);
								list9.Add(item8);
							}
						}
					}
					List<BatchNumbersImpl> list10 = new List<BatchNumbersImpl>();
					for (int num16 = 0; num16 < list8.Count; num16++)
					{
						BatchNumbersImpl batchNumbersImpl2 = new BatchNumbersImpl();
						batchNumbersImpl2.AddmisionDate = DateTime.Today;
						batchNumbersImpl2.BatchNumber = list8[num16].Item1;
						batchNumbersImpl2.Quantity = list8[num16].Item2;
						batchNumbersImpl2.BaseLineNumber = num14;
						batchNumbersImpl2.ManufacturerSerialNumber = Function.ToString(dtBatchReceipt.Rows[num16]["MnfSerial"]);
						batchNumbersImpl2.InternalSerialNumber = Function.ToString(dtBatchReceipt.Rows[num16]["LotNumber"]);
						List<DataColumn> list11 = (from DataColumn t in dtBatchReceipt.Columns
							where Function.ToString(t.ColumnName).StartsWith("U_")
							select t).ToList();
						if (list11.Count > 0)
						{
							batchNumbersImpl2.UserFields = new List<UserFieldsImpl>();
							foreach (DataColumn item9 in list11)
							{
								UserFieldsImpl userFieldsImpl = new UserFieldsImpl
								{
									Name = item9.ColumnName
								};
								Type dataType = item9.DataType;
								if (dataType == typeof(DateTime))
								{
									userFieldsImpl.Type = BoFieldTypes.db_Date;
									userFieldsImpl.Value = Function.ParseDateTime(dtBatchReceipt.Rows[num16][item9.ColumnName]);
								}
								else if (dataType == typeof(float))
								{
									userFieldsImpl.Type = BoFieldTypes.db_Float;
									userFieldsImpl.Value = Function.ParseDouble(dtBatchReceipt.Rows[num16][item9.ColumnName]);
								}
								else if (dataType == typeof(double))
								{
									userFieldsImpl.Type = BoFieldTypes.db_Numeric;
									userFieldsImpl.Value = Function.ParseDouble(dtBatchReceipt.Rows[num16][item9.ColumnName]);
								}
								else
								{
									userFieldsImpl.Type = BoFieldTypes.db_Alpha;
									userFieldsImpl.Value = Function.ToString(dtBatchReceipt.Rows[num16][item9.ColumnName]);
								}
								batchNumbersImpl2.UserFields.Add(userFieldsImpl);
							}
						}
						list10.Add(batchNumbersImpl2);
					}
					if (list10.Count > 0)
					{
						document_LinesImpl2.BatchNumbers = list10;
					}
					List<SerialNumbersImpl> list12 = new List<SerialNumbersImpl>();
					for (int num17 = 0; num17 < list9.Count; num17++)
					{
						SerialNumbersImpl serialNumbersImpl2 = new SerialNumbersImpl();
						serialNumbersImpl2.InternalSerialNumber = list9[num17].Item1;
						serialNumbersImpl2.Quantity = 1.0;
						serialNumbersImpl2.BaseLineNumber = num14;
						list12.Add(serialNumbersImpl2);
					}
					if (list12.Count > 0)
					{
						document_LinesImpl2.SerialNumbers = list12;
					}
					List<DocumentLinesBinAllocationsImpl> list13 = new List<DocumentLinesBinAllocationsImpl>();
					if (binRows2 != null && binRows2.Length != 0)
					{
						for (int num18 = 0; num18 < binRows2.Length; num18++)
						{
							string distNumber2 = Function.ToString(binRows2[num18]["DistNumber"]);
							int num19 = -1;
							if (text2 == "B")
							{
								num19 = list10.FindIndex((BatchNumbersImpl b) => b.BatchNumber == distNumber2);
							}
							if (text2 == "S")
							{
								num19 = list12.FindIndex((SerialNumbersImpl b) => b.InternalSerialNumber == distNumber2);
							}
							double quantity2 = Function.ParseDouble(binRows2[num18]["Quantity"]);
							int binAbsEntry2 = Function.ParseInt(binRows2[num18]["ToBinAbs"]);
							DocumentLinesBinAllocationsImpl documentLinesBinAllocationsImpl2 = new DocumentLinesBinAllocationsImpl();
							documentLinesBinAllocationsImpl2.BaseLineNumber = num14;
							documentLinesBinAllocationsImpl2.BinAbsEntry = binAbsEntry2;
							documentLinesBinAllocationsImpl2.Quantity = quantity2;
							if (num19 >= 0)
							{
								documentLinesBinAllocationsImpl2.SerialAndBatchNumbersBaseLine = num19;
							}
							if (documentLinesBinAllocationsImpl2.BinAbsEntry > 0)
							{
								list13.Add(documentLinesBinAllocationsImpl2);
							}
						}
					}
					document_LinesImpl2.BinAllocations = list13;
				}
				document_LinesImpl2.UserFields = WinFuntions.GetUDFByTableName(dataRow7, "IGN1");
				UserFieldsImpl userFieldsImpl2 = new UserFieldsImpl();
				userFieldsImpl2.Name = "OutAmtFactor";
				userFieldsImpl2.Type = BoFieldTypes.db_Numeric;
				userFieldsImpl2.SubType = BoFldSubTypes.st_None;
				userFieldsImpl2.Value = Function.ParseDouble(dataRow7["AmountFactor"]);
				document_LinesImpl2.UserFields.Add(userFieldsImpl2);
				document_LinesImpl2.OcrCode = Function.ToString(dataRow7["OcrCode"]);
				document_LinesImpl2.OcrCode2 = Function.ToString(dataRow7["OcrCode2"]);
				document_LinesImpl2.OcrCode3 = Function.ToString(dataRow7["OcrCode3"]);
				document_LinesImpl2.OcrCode4 = Function.ToString(dataRow7["OcrCode4"]);
				document_LinesImpl2.OcrCode5 = Function.ToString(dataRow7["OcrCode5"]);
				document_LinesImpl2.Project = Function.ToString(dataRow7["Project"]);
				list7.Add(document_LinesImpl2);
				num4++;
			}
			documentsImpl.Lines = list7;
			List<DocumentsImpl> lst = new List<DocumentsImpl> { issueDoc, documentsImpl };
			string text3 = await _httpClient.OutsourcingClient.SaveDoc(lst);
			if (string.IsNullOrEmpty(text3))
			{
				WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
				grd_Issue.DataSource = null;
				grd_Issue.RefreshDataSource();
				grd_Receipt.DataSource = null;
				grd_Receipt.RefreshDataSource();
				grd_BatchesIssue.DataSource = _DTIssueBinData.Clone();
				grd_BatchesIssue.RefreshDataSource();
				grd_BatchesReceipt.DataSource = _DTReceiptBinData.Clone();
				grd_BatchesReceipt.RefreshDataSource();
				splitContainerControl2.PanelVisibility = SplitPanelVisibility.Panel1;
				splitContainerControl3.PanelVisibility = SplitPanelVisibility.Panel1;
				txt_DocNum.Text = string.Empty;
				txt_Comments.Text = string.Empty;
				txt_JVNumber.Text = string.Empty;
				dateEx_DocDate.EditValue = DBNull.Value;
				bteCFL_Account.EditValue = DBNull.Value;
				FormMode = Enums.FormViewModeType.View;
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, text3);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		_isSaving = false;
	}

	private double GetBOMPrice(string itemCode, DataTable dtIssue)
	{
		try
		{
			double num = 0.0;
			EnumerableRowCollection<DataRow> source = from r in dtIssue.AsEnumerable()
				where Function.ToString(r["Father"]) == itemCode
				select r;
			if (source.Any())
			{
				List<DataRow> list = source.ToList();
				DataTable dataTable = new DataTable();
				dataTable.Columns.Add("ItemCode", typeof(string));
				dataTable.Columns.Add("WhsCode", typeof(string));
				dataTable.AcceptChanges();
				for (int num2 = 0; num2 < list.Count; num2++)
				{
					DataRow dataRow = dataTable.NewRow();
					dataRow["ItemCode"] = Function.ToString(list[num2]["ItemCode"]);
					dataRow["WhsCode"] = Function.ToString(list[num2]["WhsCode"]);
					dataTable.Rows.Add(dataRow);
				}
				dataTable.AcceptChanges();
				DataTable itemsAvgPrice = _httpClient.OutsourcingClient.GetItemsAvgPrice(dataTable);
				if (itemsAvgPrice.IsNotNull())
				{
					for (int num3 = 0; num3 < list.Count; num3++)
					{
						num += Function.ParseDouble(list[num3]["Quantity"]) * Function.ParseDouble(itemsAvgPrice.Rows[num3][0]);
					}
				}
			}
			return num;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return 0.0;
		}
	}

	private bool ValidateData()
	{
		try
		{
			if (!(grd_Issue.DataSource as DataTable).IsNotNull())
			{
				WinCommons.Error(Enums.MessageCode.HaveNoIssueData, "");
				return false;
			}
			if (!(grd_Receipt.DataSource as DataTable).IsNotNull())
			{
				WinCommons.Error(Enums.MessageCode.HaveNoReceiptData, "");
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

	private void AddNewRow(string grid)
	{
		try
		{
			if (grid == "I")
			{
				DataTable dataTable = grd_Issue.DataSource as DataTable;
				if (dataTable == null)
				{
					dataTable = _httpClient.OutsourcingClient.AddNewRow("I");
					dataTable.Rows.Clear();
					dataTable.AcceptChanges();
					grd_Issue.DataSource = dataTable;
					grd_Issue.RefreshDataSource();
				}
				if (!dataTable.AsEnumerable().Any((DataRow r) => string.IsNullOrWhiteSpace(Function.ToString(r["ItemCode"]))))
				{
					DataRow dataRow = dataTable.NewRow();
					dataRow["LineNum"] = GetLineNum(dataTable) + 1;
					dataRow["Quantity"] = 1;
					dataRow["ManBatchSer"] = "N";
					dataTable.Rows.Add(dataRow);
					dataTable.AcceptChanges();
				}
				grd_Issue.DataSource = dataTable;
				grd_Issue.RefreshDataSource();
			}
			else if (grid == "R")
			{
				DataTable dataTable2 = grd_Receipt.DataSource as DataTable;
				if (dataTable2 == null)
				{
					dataTable2 = _httpClient.OutsourcingClient.AddNewRow("R");
					dataTable2.Rows.Clear();
					dataTable2.AcceptChanges();
					grd_Receipt.DataSource = dataTable2;
					grd_Receipt.RefreshDataSource();
				}
				if (!dataTable2.AsEnumerable().Any((DataRow r) => string.IsNullOrWhiteSpace(Function.ToString(r["ItemCode"]))))
				{
					DataRow dataRow2 = dataTable2.NewRow();
					dataRow2["Quantity"] = 1;
					dataRow2["LineNum"] = GetLineNum(dataTable2) + 1;
					dataRow2["ManBatchSer"] = "N";
					dataTable2.Rows.Add(dataRow2);
					dataTable2.AcceptChanges();
				}
				grd_Receipt.DataSource = dataTable2;
				grd_Receipt.RefreshDataSource();
			}
			SetRepositoryItemToGrid();
			SetReadonlyColumns();
			SetVisibleColumns();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		SetReceiptLineNum();
	}

	private void SetReceiptLineNum()
	{
		try
		{
			if (!(grd_Receipt.DataSource is DataTable dataTable) || dataTable.Rows.Count <= 0)
			{
				return;
			}
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				if (dataTable.Rows[i]["LineNum"] == DBNull.Value)
				{
					dataTable.Rows[i]["LineNum"] = GetLineNum(dataTable) + 1;
					dataTable.AcceptChanges();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private int GetLineNum(DataTable dt)
	{
		try
		{
			if (dt == null || dt.Rows.Count == 0)
			{
				return -1;
			}
			int num = -1;
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				int num2 = Function.ParseInt(dt.Rows[i]["LineNum"]);
				if (num2 > num)
				{
					num = num2;
				}
			}
			return num;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return -1;
		}
	}

	private void SetRepositoryItemToGrid()
	{
		try
		{
			if (gv_Issue.Columns.Count <= 5)
			{
				gv_Issue.PopulateColumns();
			}
			if (gv_Issue.Columns["ItemCode"] != null && gv_Issue.Columns["ItemCode"].ColumnEdit == null)
			{
				gv_Issue.Columns["ItemCode"].ColumnEdit = rep_bte_itemCode_issue;
			}
			if (gv_Issue.Columns["WhsCode"] != null && gv_Issue.Columns["WhsCode"].ColumnEdit == null)
			{
				gv_Issue.Columns["WhsCode"].ColumnEdit = rep_bte_whsCode;
			}
			if (gv_Issue.Columns["AcctCode"] != null && gv_Issue.Columns["AcctCode"].ColumnEdit == null)
			{
				gv_Issue.Columns["AcctCode"].ColumnEdit = rep_bte_acctCode;
			}
			if (gv_Issue.Columns["DocEntry"] != null && gv_Issue.Columns["DocEntry"].ColumnEdit == null)
			{
				gv_Issue.Columns["DocEntry"].ColumnEdit = rep_bte_docEntry_issue;
			}
			if (gv_Receipt.Columns.Count <= 5)
			{
				gv_Receipt.PopulateColumns();
			}
			if (gv_Receipt.Columns["ItemCode"] != null && gv_Receipt.Columns["ItemCode"].ColumnEdit == null)
			{
				gv_Receipt.Columns["ItemCode"].ColumnEdit = rep_bte_itemCode_receipt;
			}
			if (gv_Receipt.Columns["WhsCode"] != null && gv_Receipt.Columns["WhsCode"].ColumnEdit == null)
			{
				gv_Receipt.Columns["WhsCode"].ColumnEdit = rep_bte_whsCode;
			}
			if (gv_Receipt.Columns["AcctCode"] != null && gv_Receipt.Columns["AcctCode"].ColumnEdit == null)
			{
				gv_Receipt.Columns["AcctCode"].ColumnEdit = rep_bte_acctCode;
			}
			if (gv_Receipt.Columns["DocEntry"] != null && gv_Receipt.Columns["DocEntry"].ColumnEdit == null)
			{
				gv_Receipt.Columns["DocEntry"].ColumnEdit = rep_bte_docEntry_receipt;
			}
			ShowHideRepositoryButton();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ReLoadGridIssue(string itemCode, double quantity, string column)
	{
		try
		{
			DataTable dataTable = grd_Issue.DataSource as DataTable;
			if (!(grd_Receipt.DataSource as DataTable).IsNotNull())
			{
				return;
			}
			DataTable dataTable2 = new DataTable();
			dataTable2.Columns.Add("ItemCode", typeof(string));
			dataTable2.Columns.Add("Quantity", typeof(double));
			dataTable2.AcceptChanges();
			DataRow dataRow = dataTable2.NewRow();
			dataRow["ItemCode"] = itemCode;
			dataRow["Quantity"] = quantity;
			dataTable2.Rows.Add(dataRow);
			dataTable2.AcceptChanges();
			if (!dataTable2.IsNotNull())
			{
				return;
			}
			DataTable bOMComponents = _httpClient.OutsourcingClient.GetBOMComponents(dataTable2);
			if (!bOMComponents.IsNotNull() || dataTable == null || dataTable.Rows.Count <= 0)
			{
				return;
			}
			if (column == "ItemCode")
			{
				if (!dataTable.AsEnumerable().Any((DataRow r) => Function.ToString(r["Father"]).ToUpper() == itemCode.ToUpper()))
				{
					for (int num = 0; num < bOMComponents.Rows.Count; num++)
					{
						dataTable.ImportRow(bOMComponents.Rows[num]);
					}
					dataTable.AcceptChanges();
					grd_Issue.DataSource = dataTable;
					grd_Issue.RefreshDataSource();
					AddNewRow("I");
				}
			}
			else if (column == "Quantity")
			{
				List<DataRow> lst = (from r in dataTable.AsEnumerable()
					where Function.ToString(r["Father"]).ToUpper() == itemCode.ToUpper()
					select r).ToList();
				if (lst.Count > 0)
				{
					int i;
					for (i = 0; i < lst.Count; i++)
					{
						DataRow dataRow2 = bOMComponents.AsEnumerable().FirstOrDefault((DataRow r) => Function.ParseInt(r["ChildNum"]) == Function.ParseInt(lst[i]["ChildNum"]));
						lst[i]["Quantity"] = ((dataRow2 != null) ? Function.ParseDouble(dataRow2["Quantity"]) : 0.0);
					}
				}
			}
			for (int num2 = dataTable.Rows.Count - 1; num2 >= 0; num2--)
			{
				if (string.IsNullOrWhiteSpace(Function.ToString(dataTable.Rows[num2]["ItemCode"])))
				{
					dataTable.Rows.Remove(dataTable.Rows[num2]);
				}
			}
			AddNewRow("I");
			ResetLineNum();
			dataTable.AcceptChanges();
			for (int num3 = 0; num3 < gv_Issue.DataRowCount; num3++)
			{
				AddBinBatchDataTableIssue(num3);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ResetLineNum()
	{
		try
		{
			if (!(grd_Issue.DataSource is DataTable dataTable) || dataTable.Rows.Count <= 0)
			{
				return;
			}
			int num = dataTable.AsEnumerable().Max((DataRow r) => Function.ParseInt(r["LineNum"]));
			foreach (DataRow row in dataTable.Rows)
			{
				if (Function.ParseInt(row["LineNum"]) <= 0)
				{
					num++;
					row["LineNum"] = num;
				}
			}
			dataTable.AcceptChanges();
			grd_Issue.DataSource = dataTable;
			grd_Issue.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ShowHideRepositoryButton()
	{
		try
		{
			EditorButton editorButton = rep_bte_itemCode_issue.Buttons.FirstOrDefault((EditorButton b) => b.Kind == ButtonPredefines.Search);
			EditorButton editorButton2 = rep_bte_itemCode_receipt.Buttons.FirstOrDefault((EditorButton b) => b.Kind == ButtonPredefines.Search);
			EditorButton editorButton3 = rep_bte_whsCode.Buttons.FirstOrDefault((EditorButton b) => b.Kind == ButtonPredefines.Search);
			if (FormMode == Enums.FormViewModeType.Update)
			{
				editorButton.Visible = true;
				if (editorButton2 != null)
				{
					editorButton2.Visible = true;
				}
				if (editorButton3 != null)
				{
					editorButton3.Visible = true;
				}
				rep_bte_docEntry_receipt.Buttons[1].Visible = false;
				return;
			}
			if (editorButton != null)
			{
				editorButton.Visible = false;
			}
			if (editorButton2 != null)
			{
				editorButton2.Visible = false;
			}
			if (editorButton3 != null)
			{
				editorButton3.Visible = false;
			}
			rep_bte_docEntry_receipt.Buttons[1].Visible = true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void RemoveRow(GridView gv)
	{
		try
		{
			DataTable dataTable = gv.GridControl.DataSource as DataTable;
			int[] selRows = gv.GetSelectedRows();
			if (gv.Name == "gv_Receipt" && selRows.Length != 0 && rdg_InputMethod.SelectedIndex == 0 && grd_Issue.DataSource is DataTable dataTable2 && dataTable2.Rows.Count > 0)
			{
				int i;
				for (i = 0; i < selRows.Length; i++)
				{
					List<DataRow> list = (from r in dataTable2.AsEnumerable()
						where Function.ToString(r["Father"]).ToUpper() == Function.ToString(gv.GetRowCellValue(selRows[i], "ItemCode")).ToUpper()
						select r).ToList();
					if (list.Count > 0)
					{
						for (int num = 0; num < list.Count; num++)
						{
							dataTable2.Rows.Remove(list[num]);
						}
						dataTable2.AcceptChanges();
					}
				}
			}
			gv.DeleteSelectedRows();
			dataTable?.AcceptChanges();
			gv.GridControl.DataSource = dataTable;
			gv.GridControl.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void AddBinBatchDataTableIssue(int rowHandle, bool changeWarehouse = false)
	{
		try
		{
			DataTable allBinData = _httpClient.OutsourcingClient.GetAllBinData(Function.ToString(gv_Issue.GetRowCellValue(rowHandle, "ItemCode")), Function.ToString(gv_Issue.GetRowCellValue(rowHandle, "WhsCode")));
			if (allBinData == null || allBinData.Rows.Count <= 0)
			{
				return;
			}
			int num = Function.ParseInt(gv_Issue.GetRowCellValue(rowHandle, "LineNum"));
			DataRow[] array = _DTIssueBinData.Select("LineNum=" + num);
			if ((array.Length == 0) | changeWarehouse)
			{
				if (changeWarehouse)
				{
					for (int i = 0; i < array.Length; i++)
					{
						_DTIssueBinData.Rows.Remove(array[i]);
					}
					_DTIssueBinData.AcceptChanges();
				}
				for (int j = 0; j < allBinData.Rows.Count; j++)
				{
					allBinData.Rows[j]["LineNum"] = num;
					allBinData.AcceptChanges();
					_DTIssueBinData.ImportRow(allBinData.Rows[j]);
				}
				_DTIssueBinData.AcceptChanges();
			}
			if (rowHandle == gv_Issue.FocusedRowHandle)
			{
				ShowBatchData(gv_Issue, _DTIssueBinData, grd_BatchesIssue, splitContainerControl2);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void SetRowCellStyle(RowCellStyleEventArgs e)
	{
		try
		{
			if (e.Column.OptionsColumn.ReadOnly)
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

	private void ValidatingSnBGridQuantity(GridView gv_Batches, GridView gv_Line, BaseContainerValidateEditorEventArgs e)
	{
		try
		{
			if (!(gv_Batches.FocusedColumn.FieldName == "Quantity"))
			{
				return;
			}
			string text = Function.ToString(gv_Line.GetFocusedRowCellValue("ManBatchSer"));
			decimal num = Function.ParseDecimal(gv_Line.GetFocusedRowCellValue("Quantity"));
			decimal num2 = Function.ParseDecimal(e.Value);
			decimal num3 = Function.ParseDecimal(gv_Batches.GetFocusedRowCellValue("Quantity"));
			if (text == "B" && gv_Line.Name == "gv_Issue")
			{
				decimal num4 = Function.ParseDecimal(gv_Batches.GetFocusedRowCellValue("AvailQty"));
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
			if (gv_Batches.GridControl.DataSource is DataTable dataTable && dataTable.Rows.Count > 0 && dataTable.AsEnumerable().Sum((DataRow r) => Function.ParseDecimal(r["Quantity"])) - num3 + num2 > num)
			{
				e.ErrorText = "Total quantity cannot greater than line quantity";
				e.Valid = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task GetRecord(string position, int clgCode)
	{
		_ = 4;
		try
		{
			bool flag = true;
			if (FormMode == Enums.FormViewModeType.Update && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				flag = false;
			}
			if (!flag)
			{
				return;
			}
			int num = await _httpClient.OutsourcingClient.GetRecord(position, clgCode);
			if (num > 0)
			{
				DataTable dataTable = new DataTable();
				dataTable.Columns.Add(new DataColumn("DocNum", typeof(int)));
				dataTable.Columns.Add(new DataColumn("Comments", typeof(string)));
				dataTable.Columns.Add(new DataColumn("DocDate", typeof(string)));
				DataRow dataRow = dataTable.NewRow();
				dataRow["DocNum"] = num;
				dataRow["Comments"] = "";
				dataRow["DocDate"] = DBNull.Value;
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
				DataTable dataTable2 = await _httpClient.OutsourcingClient.Search(dataTable);
				if (dataTable2 != null && dataTable2.Rows.Count > 0)
				{
					await ShowData(dataTable2.Rows[0]);
				}
			}
			else if (position == "N")
			{
				await GetRecord("F", 0);
			}
			else if (position == "P")
			{
				await GetRecord("L", 0);
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.NoMatchingRecordFound, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task CreateColumnsDistRule(GridViewEx gv)
	{
		try
		{
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Sales");
			DataTable dataTable = await hp.GetAsync("GetDimensions");
			if (dataTable == null || dataTable.Rows.Count <= 0)
			{
				return;
			}
			foreach (DataRow row in dataTable.Rows)
			{
				AddDistRule(gv, row);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void AddDistRule(GridViewEx gv, DataRow r, string fld = "", string caption = "")
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
							gv.SetRowCellValue(gv.FocusedRowHandle, fieldName, Function.ToString(row[0]));
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
				gridColumn.Visible = true;
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

	private void SetVisibleColumns()
	{
		try
		{
			if (gv_Issue.Columns["LineNum"] != null)
			{
				gv_Issue.Columns["LineNum"].Visible = false;
			}
			if (gv_Issue.Columns["ManBatchSer"] != null)
			{
				gv_Issue.Columns["ManBatchSer"].Visible = false;
			}
			if (gv_Receipt.Columns["ManBatchSer"] != null)
			{
				gv_Receipt.Columns["ManBatchSer"].Visible = false;
			}
			if (FormMode == Enums.FormViewModeType.Find)
			{
				if (gv_Issue.Columns["DocEntry"] != null)
				{
					gv_Issue.Columns["DocEntry"].Visible = true;
				}
				if (gv_Receipt.Columns["DocEntry"] != null)
				{
					gv_Receipt.Columns["DocEntry"].Visible = true;
				}
			}
			else if (FormMode == Enums.FormViewModeType.Update)
			{
				if (gv_Issue.Columns["DocEntry"] != null)
				{
					gv_Issue.Columns["DocEntry"].Visible = false;
				}
				if (gv_Receipt.Columns["DocEntry"] != null)
				{
					gv_Receipt.Columns["DocEntry"].Visible = false;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void SetReadonlyColumns()
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Find)
			{
				SetReadonlyGridColumns(_arrROnlyColIssueFind, gv_Issue);
				SetReadonlyGridColumns(_arrROnlyColReceiptFind, gv_Receipt);
				SetReadonlyGridColumns(_arrROnlyColIssueBatch, gv_Receipt);
			}
			else if (FormMode == Enums.FormViewModeType.Update)
			{
				if (rdg_InputMethod.SelectedIndex == 0)
				{
					SetReadonlyGridColumns(_arrROnlyColIssueAddBOM, gv_Issue);
				}
				else
				{
					SetReadonlyGridColumns(_arrROnlyColIssueAdd, gv_Issue);
				}
				SetReadonlyGridColumns(_arrROnlyColReceiptAdd, gv_Receipt);
			}
			SetReadonlyGridColumns(_arrROnlyColIssueBatch, gv_BatchesIssue);
			SetReadonlyGridColumns(_arrROnlyColReceiptBatch, gv_BatchesReceipt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void SetReadonlyGridColumns(string[] arrColumns, GridViewEx gv)
	{
		try
		{
			for (int i = 0; i < gv.Columns.Count; i++)
			{
				if (arrColumns.Contains(gv.Columns[i].FieldName))
				{
					gv.Columns[i].OptionsColumn.ReadOnly = true;
				}
				else
				{
					gv.Columns[i].OptionsColumn.ReadOnly = false;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ShowBatchData(GridView gvLine, DataTable dtBinData, GridControl grdBatch, SplitContainerControl splitCont)
	{
		try
		{
			if ((Function.ToString(gvLine.GetFocusedRowCellValue("ManBatchSer")) == "B" || Function.ToString(gvLine.GetFocusedRowCellValue("ManBatchSer")) == "S" || _binWarehouse.Contains(Function.ToString(gvLine.GetFocusedRowCellValue("WhsCode")).ToLower())) && FormMode == Enums.FormViewModeType.Update)
			{
				splitCont.PanelVisibility = SplitPanelVisibility.Both;
			}
			else
			{
				splitCont.PanelVisibility = SplitPanelVisibility.Panel1;
			}
			grdBatch.DataSource = dtBinData.Clone();
			grdBatch.RefreshDataSource();
			int num = Function.ParseInt(gvLine.GetFocusedRowCellValue("LineNum"));
			string itemCode = Function.ToString(gvLine.GetFocusedRowCellValue("ItemCode"));
			string text = Function.ToString(gvLine.GetFocusedRowCellValue("WhsCode"));
			string text2 = Function.ToString(gvLine.GetFocusedRowCellValue("ManBatchSer"));
			bool flag = _binWarehouse.Contains(text.ToLower());
			if (text2 != "B" && text2 != "S" && !flag)
			{
				return;
			}
			DataTable allBinData = _httpClient.OutsourcingClient.GetAllBinData(itemCode, text);
			if (gvLine.Name == "gv_Issue" && (allBinData == null || allBinData.Rows.Count == 0))
			{
				return;
			}
			DataRow[] array = dtBinData.Select("LineNum=" + num);
			DataTable dataTable = dtBinData.Clone();
			if (array != null && array.Length != 0)
			{
				for (int i = 0; i < array.Length; i++)
				{
					dataTable.ImportRow(array[i]);
					dataTable.AcceptChanges();
				}
			}
			else
			{
				if (gvLine.Name == "gv_Issue")
				{
					for (int j = 0; j < allBinData.Rows.Count; j++)
					{
						allBinData.Rows[j]["LineNum"] = num;
						allBinData.AcceptChanges();
						dtBinData.ImportRow(allBinData.Rows[j]);
						dataTable.ImportRow(allBinData.Rows[j]);
					}
				}
				dtBinData.AcceptChanges();
				dataTable.AcceptChanges();
			}
			grdBatch.DataSource = dataTable;
			grdBatch.RefreshDataSource();
			if (gvLine.Name == "gv_Receipt" && ((text2 == "B" || text2 == "S") | flag))
			{
				AddNewRowBatchReceipt();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task SetBinBatchData(GridView gvLine, GridControl grdBatch, DataTable dtBinData, int rowHandle, SplitContainerControl splitCont)
	{
		if (_isSetBinBatchData)
		{
			return;
		}
		_isSetBinBatchData = true;
		try
		{
			int num = Function.ParseInt(gvLine.GetRowCellValue(rowHandle, "LineNum"));
			string text = Function.ToString(gvLine.GetRowCellValue(rowHandle, "WhsCode"));
			string text2 = Function.ToString(gvLine.GetRowCellValue(rowHandle, "ManBatchSer"));
			DataRow[] array = dtBinData.Select("LineNum=" + num);
			bool flag = _binWarehouse.Contains(text.ToLower());
			if (flag || text2.ToLower() == "b" || text2.ToLower() == "s")
			{
				DataTable dataTable = grdBatch.DataSource as DataTable;
				dataTable.AcceptChanges();
				if (dataTable == null || dataTable.Rows.Count == 0)
				{
					if (gvLine.Name == gv_Issue.Name)
					{
						AddBinBatchDataTableIssue(rowHandle);
					}
				}
				else
				{
					if (array != null && array.Length != 0)
					{
						for (int i = 0; i < array.Length; i++)
						{
							dtBinData.Rows.Remove(array[i]);
						}
						dtBinData.AcceptChanges();
					}
					for (int j = 0; j < dataTable.Rows.Count; j++)
					{
						if ((!string.IsNullOrWhiteSpace(Function.ToString(dataTable.Rows[j]["DistNumber"])) && (text2 == "B" || text2 == "S")) | flag)
						{
							dtBinData.ImportRow(dataTable.Rows[j]);
						}
					}
					dtBinData.AcceptChanges();
				}
			}
			ShowBatchData(gvLine, dtBinData, grdBatch, splitCont);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		_isSetBinBatchData = false;
	}

	private void AddNewRowBatchReceipt()
	{
		try
		{
			string text = Function.ToString(gv_Receipt.GetFocusedRowCellValue("WhsCode"));
			bool flag = _binWarehouse.Contains(text.ToLower());
			if ((Function.ToString(gv_Receipt.GetFocusedRowCellValue("ManBatchSer")) == "B" || Function.ToString(gv_Receipt.GetFocusedRowCellValue("ManBatchSer")) == "S") | flag)
			{
				splitContainerControl3.PanelVisibility = SplitPanelVisibility.Both;
			}
			DataTable dataTable = grd_BatchesReceipt.DataSource as DataTable;
			if (dataTable == null || dataTable.Columns.Count < 4)
			{
				dataTable = _DTReceiptBinData.Clone();
			}
			if (!dataTable.IsNotNull() || !dataTable.AsEnumerable().Any((DataRow r) => string.IsNullOrWhiteSpace(Function.ToString(r["DistNumber"]))))
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["DistNumber"] = "";
				dataRow["LineNum"] = Function.ParseInt(gv_Receipt.GetFocusedRowCellValue("LineNum"));
				dataRow["ToBinCode"] = "";
				dataRow["Quantity"] = 0.0;
				dataRow["BtSAbsEntry"] = 0;
				dataRow["ToBinAbs"] = 0;
				dataRow["BinActivat"] = (flag ? "Y" : "N");
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
				grd_BatchesReceipt.DataSource = dataTable;
				grd_BatchesReceipt.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Production.frm_OutSourcing));
		DevExpress.Utils.SerializableAppearanceObject appearance2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled5 = new DevExpress.Utils.SerializableAppearanceObject();
		this.txt_DocNum = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.txt_Comments = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.dateEx_DocDate = new Apzon.Controls.Editor.DateEditEx();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.splitContainerControl2 = new DevExpress.XtraEditors.SplitContainerControl();
		this.grd_Issue = new Apzon.Controls.Grid.GridControlEx();
		this.ctm_Issue = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.removeRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Issue = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx22 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx23 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx24 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx25 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx26 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx29 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx27 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx28 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx35 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_LineProject = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.grd_BatchesIssue = new Apzon.Controls.Grid.GridControlEx();
		this.gv_BatchesIssue = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx5 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx6 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx12 = new Apzon.Controls.Grid.GridColumnEx();
		this.splitContainerControl3 = new DevExpress.XtraEditors.SplitContainerControl();
		this.grd_Receipt = new Apzon.Controls.Grid.GridControlEx();
		this.ctm_Receipt = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.removeRowToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Receipt = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx14 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx15 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx16 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx17 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx18 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx19 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx20 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx21 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx41 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_LineProject1 = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.grd_BatchesReceipt = new Apzon.Controls.Grid.GridControlEx();
		this.ctm_IssueBatch = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.removeRowToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_BatchesReceipt = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx7 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx8 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_bte_ToBinCode_Receipt = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gridColumnEx9 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx10 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx11 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx13 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.rdg_InputMethod = new Apzon.Controls.Editor.RadioGroupEx();
		this.labelLine5 = new Apzon.Controls.Controls.LabelLine();
		this.bteCFL_Account = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.txt_JVNumber = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine6 = new Apzon.Controls.Controls.LabelLine();
		this.lbBranch = new Apzon.Controls.Controls.LabelLine();
		this.txt_Branch = new Apzon.Forms.UserControls.LookUpEditCombo();
		((System.ComponentModel.ISupportInitialize)this.txt_DocNum.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Comments.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dateEx_DocDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dateEx_DocDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl2).BeginInit();
		this.splitContainerControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Issue).BeginInit();
		this.ctm_Issue.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Issue).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_LineProject).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_BatchesIssue).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_BatchesIssue).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl3).BeginInit();
		this.splitContainerControl3.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Receipt).BeginInit();
		this.ctm_Receipt.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Receipt).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_LineProject1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_BatchesReceipt).BeginInit();
		this.ctm_IssueBatch.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_BatchesReceipt).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_bte_ToBinCode_Receipt).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rdg_InputMethod.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bteCFL_Account.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_JVNumber.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Branch.Properties).BeginInit();
		base.SuspendLayout();
		this.txt_DocNum.EditControl = null;
		this.txt_DocNum.FieldName = "txtDocNum";
		this.txt_DocNum.IsSearchControl = false;
		this.txt_DocNum.Location = new System.Drawing.Point(143, 3);
		this.txt_DocNum.Name = "txt_DocNum";
		this.txt_DocNum.SettingInVisible = false;
		this.txt_DocNum.SettingReadonly = false;
		this.txt_DocNum.Size = new System.Drawing.Size(104, 20);
		this.txt_DocNum.TabIndex = 13;
		this.txt_DocNum.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_DocNum_KeyDown);
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "Doc Num";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(9, 6);
		this.labelLine3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(135, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 12;
		this.labelLine3.Utext = "Doc Num";
		this.txt_Comments.EditControl = null;
		this.txt_Comments.FieldName = "txtComments";
		this.txt_Comments.IsSearchControl = false;
		this.txt_Comments.Location = new System.Drawing.Point(143, 52);
		this.txt_Comments.Name = "txt_Comments";
		this.txt_Comments.SettingInVisible = false;
		this.txt_Comments.SettingReadonly = false;
		this.txt_Comments.Size = new System.Drawing.Size(353, 20);
		this.txt_Comments.TabIndex = 15;
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Comments";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(9, 55);
		this.labelLine1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(135, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 14;
		this.labelLine1.Utext = "Comments";
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "Doc Date";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(536, 5);
		this.labelLine2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(135, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 16;
		this.labelLine2.Utext = "Doc Date";
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "Clearing Account";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(536, 28);
		this.labelLine4.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(135, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 18;
		this.labelLine4.Utext = "Clearing Account";
		this.dateEx_DocDate.EditControl = null;
		this.dateEx_DocDate.EditValue = null;
		this.dateEx_DocDate.Location = new System.Drawing.Point(666, 3);
		this.dateEx_DocDate.FieldName = "txtDocDate";
		this.dateEx_DocDate.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.dateEx_DocDate.Name = "dateEx_DocDate";
		this.dateEx_DocDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dateEx_DocDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dateEx_DocDate.Properties.IsSearchControl = false;
		this.dateEx_DocDate.SettingInVisible = false;
		this.dateEx_DocDate.SettingReadonly = false;
		this.dateEx_DocDate.Size = new System.Drawing.Size(135, 20);
		this.dateEx_DocDate.TabIndex = 20;
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Location = new System.Drawing.Point(9, 82);
		this.splitContainerControl1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
		this.splitContainerControl1.Panel1.Controls.Add(this.splitContainerControl2);
		this.splitContainerControl1.Panel1.ShowCaption = true;
		this.splitContainerControl1.Panel1.Text = "Issue";
		this.splitContainerControl1.Panel2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
		this.splitContainerControl1.Panel2.Controls.Add(this.splitContainerControl3);
		this.splitContainerControl1.Panel2.ShowCaption = true;
		this.splitContainerControl1.Panel2.Text = "Receipt";
		this.splitContainerControl1.Size = new System.Drawing.Size(1167, 493);
		this.splitContainerControl1.SplitterPosition = 594;
		this.splitContainerControl1.TabIndex = 22;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.splitContainerControl1.Resize += new System.EventHandler(this.splitContainerControl_Resize);
		this.splitContainerControl2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainerControl2.Horizontal = false;
		this.splitContainerControl2.Location = new System.Drawing.Point(0, 0);
		this.splitContainerControl2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.splitContainerControl2.Name = "splitContainerControl2";
		this.splitContainerControl2.Panel1.Controls.Add(this.grd_Issue);
		this.splitContainerControl2.Panel1.Text = "Panel1";
		this.splitContainerControl2.Panel2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
		this.splitContainerControl2.Panel2.Controls.Add(this.grd_BatchesIssue);
		this.splitContainerControl2.Panel2.ShowCaption = true;
		this.splitContainerControl2.Panel2.Text = "Issue Batch/Serial/Bin";
		this.splitContainerControl2.Size = new System.Drawing.Size(590, 468);
		this.splitContainerControl2.SplitterPosition = 223;
		this.splitContainerControl2.TabIndex = 1;
		this.splitContainerControl2.Text = "splitContainerControl2";
		this.splitContainerControl2.Resize += new System.EventHandler(this.splitContainerControl_Resize);
		this.grd_Issue.AllowPasteExcel = true;
		this.grd_Issue.ClipboardData = null;
		this.grd_Issue.ContextMenuStrip = this.ctm_Issue;
		this.grd_Issue.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Issue.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grd_Issue.Location = new System.Drawing.Point(0, 0);
		this.grd_Issue.MainView = this.gv_Issue;
		this.grd_Issue.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grd_Issue.Name = "grd_Issue";
		this.grd_Issue.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.rep_LineProject });
		this.grd_Issue.Size = new System.Drawing.Size(590, 223);
		this.grd_Issue.TabIndex = 0;
		this.grd_Issue.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Issue });
		this.ctm_Issue.ImageScalingSize = new System.Drawing.Size(20, 20);
		this.ctm_Issue.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.removeRowToolStripMenuItem });
		this.ctm_Issue.Name = "contextMenuStrip1";
		this.ctm_Issue.Size = new System.Drawing.Size(144, 26);
		this.removeRowToolStripMenuItem.Name = "removeRowToolStripMenuItem";
		this.removeRowToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
		this.removeRowToolStripMenuItem.Text = "Remove Row";
		this.removeRowToolStripMenuItem.Click += new System.EventHandler(this.removeRowToolStripMenuItem_Click);
		this.gv_Issue.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Issue.ColumnKey = null;
		this.gv_Issue.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[9] { this.gridColumnEx22, this.gridColumnEx23, this.gridColumnEx24, this.gridColumnEx25, this.gridColumnEx26, this.gridColumnEx29, this.gridColumnEx27, this.gridColumnEx28, this.gridColumnEx35 });
		this.gv_Issue.DisableFormSetting = false;
		this.gv_Issue.DisplayException = false;
		this.gv_Issue.EditColumn = null;
		this.gv_Issue.FieldCurrency = null;
		this.gv_Issue.FieldRate = null;
		this.gv_Issue.FormId = "oOutsourcing";
		this.gv_Issue.GridControl = this.grd_Issue;
		this.gv_Issue.IgnoreFilter = false;
		this.gv_Issue.IndexEditRow = -1;
		this.gv_Issue.IndicatorWidth = 50;
		this.gv_Issue.IsApzTable = false;
		this.gv_Issue.IsInsertNewRow = false;
		this.gv_Issue.IsLoadingData = false;
		this.gv_Issue.IsMappingContextAction = false;
		this.gv_Issue.ItemId = "ISSUE";
		this.gv_Issue.LstColumnAllowBatch = null;
		this.gv_Issue.LstColumnNotAllowBatch = null;
		this.gv_Issue.LstColumnNotEditable = null;
		this.gv_Issue.LstColumNotSetting = null;
		this.gv_Issue.Name = "gv_Issue";
		this.gv_Issue.OptionsFind.AllowFindPanel = false;
		this.gv_Issue.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Issue.OptionsView.ColumnAutoWidth = false;
		this.gv_Issue.OptionsView.ShowGroupPanel = false;
		this.gv_Issue.UseBatchMod = false;
		this.gv_Issue.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Issue_RowCellStyle);
		this.gv_Issue.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.gv_RowStyle);
		this.gv_Issue.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gv_Issue_FocusedRowChanged);
		this.gv_Issue.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_Issue_FocusedRowObjectChanged);
		this.gv_Issue.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Issue_CellValueChanged);
		this.gv_Issue.BeforeLeaveRow += new DevExpress.XtraGrid.Views.Base.RowAllowEventHandler(this.gv_BeforeLeaveRow);
		this.gridColumnEx22.Caption = "Line No.";
		this.gridColumnEx22.CurrencyDateField = null;
		this.gridColumnEx22.CurrencyField = null;
		this.gridColumnEx22.CurrencyFixed = null;
		this.gridColumnEx22.CurrencyFixedRate = 0.0;
		this.gridColumnEx22.CurrencyRateField = null;
		this.gridColumnEx22.FieldName = "LineNum";
		this.gridColumnEx22.Name = "gridColumnEx22";
		this.gridColumnEx22.PrimaryKey = true;
		this.gridColumnEx22.UnitField = null;
		this.gridColumnEx23.Caption = "Item Code";
		this.gridColumnEx23.CurrencyDateField = null;
		this.gridColumnEx23.CurrencyField = null;
		this.gridColumnEx23.CurrencyFixed = null;
		this.gridColumnEx23.CurrencyFixedRate = 0.0;
		this.gridColumnEx23.CurrencyRateField = null;
		this.gridColumnEx23.FieldName = "ItemCode";
		this.gridColumnEx23.Name = "gridColumnEx23";
		this.gridColumnEx23.PrimaryKey = true;
		this.gridColumnEx23.UnitField = null;
		this.gridColumnEx23.Visible = true;
		this.gridColumnEx23.VisibleIndex = 0;
		this.gridColumnEx23.Width = 85;
		this.gridColumnEx24.Caption = "Item Name";
		this.gridColumnEx24.CurrencyDateField = null;
		this.gridColumnEx24.CurrencyField = null;
		this.gridColumnEx24.CurrencyFixed = null;
		this.gridColumnEx24.CurrencyFixedRate = 0.0;
		this.gridColumnEx24.CurrencyRateField = null;
		this.gridColumnEx24.FieldName = "ItemName";
		this.gridColumnEx24.Name = "gridColumnEx24";
		this.gridColumnEx24.PrimaryKey = false;
		this.gridColumnEx24.UnitField = null;
		this.gridColumnEx24.Visible = true;
		this.gridColumnEx24.VisibleIndex = 1;
		this.gridColumnEx24.Width = 189;
		this.gridColumnEx25.Caption = "Warehouse";
		this.gridColumnEx25.CurrencyDateField = null;
		this.gridColumnEx25.CurrencyField = null;
		this.gridColumnEx25.CurrencyFixed = null;
		this.gridColumnEx25.CurrencyFixedRate = 0.0;
		this.gridColumnEx25.CurrencyRateField = null;
		this.gridColumnEx25.FieldName = "WhsCode";
		this.gridColumnEx25.Name = "gridColumnEx25";
		this.gridColumnEx25.PrimaryKey = false;
		this.gridColumnEx25.UnitField = null;
		this.gridColumnEx25.Visible = true;
		this.gridColumnEx25.VisibleIndex = 2;
		this.gridColumnEx25.Width = 63;
		this.gridColumnEx26.Caption = "Quantity";
		this.gridColumnEx26.CurrencyDateField = null;
		this.gridColumnEx26.CurrencyField = null;
		this.gridColumnEx26.CurrencyFixed = null;
		this.gridColumnEx26.CurrencyFixedRate = 0.0;
		this.gridColumnEx26.CurrencyRateField = null;
		this.gridColumnEx26.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gridColumnEx26.FieldName = "Quantity";
		this.gridColumnEx26.Name = "gridColumnEx26";
		this.gridColumnEx26.PrimaryKey = false;
		this.gridColumnEx26.UnitField = null;
		this.gridColumnEx26.Visible = true;
		this.gridColumnEx26.VisibleIndex = 3;
		this.gridColumnEx26.Width = 64;
		this.gridColumnEx29.Caption = "Father";
		this.gridColumnEx29.CurrencyDateField = null;
		this.gridColumnEx29.CurrencyField = null;
		this.gridColumnEx29.CurrencyFixed = null;
		this.gridColumnEx29.CurrencyFixedRate = 0.0;
		this.gridColumnEx29.CurrencyRateField = null;
		this.gridColumnEx29.FieldName = "Father";
		this.gridColumnEx29.Name = "gridColumnEx29";
		this.gridColumnEx29.OptionsColumn.AllowEdit = false;
		this.gridColumnEx29.OptionsColumn.ReadOnly = true;
		this.gridColumnEx29.PrimaryKey = false;
		this.gridColumnEx29.UnitField = null;
		this.gridColumnEx29.Visible = true;
		this.gridColumnEx29.VisibleIndex = 4;
		this.gridColumnEx27.Caption = "Doc. Entry";
		this.gridColumnEx27.CurrencyDateField = null;
		this.gridColumnEx27.CurrencyField = null;
		this.gridColumnEx27.CurrencyFixed = null;
		this.gridColumnEx27.CurrencyFixedRate = 0.0;
		this.gridColumnEx27.CurrencyRateField = null;
		this.gridColumnEx27.FieldName = "DocEntry";
		this.gridColumnEx27.Name = "gridColumnEx27";
		this.gridColumnEx27.PrimaryKey = false;
		this.gridColumnEx27.UnitField = null;
		this.gridColumnEx28.Caption = "Managed by";
		this.gridColumnEx28.CurrencyDateField = null;
		this.gridColumnEx28.CurrencyField = null;
		this.gridColumnEx28.CurrencyFixed = null;
		this.gridColumnEx28.CurrencyFixedRate = 0.0;
		this.gridColumnEx28.CurrencyRateField = null;
		this.gridColumnEx28.FieldName = "ManBatchSer";
		this.gridColumnEx28.Name = "gridColumnEx28";
		this.gridColumnEx28.PrimaryKey = false;
		this.gridColumnEx28.UnitField = null;
		this.gridColumnEx35.Caption = "Project";
		this.gridColumnEx35.ColumnEdit = this.rep_LineProject;
		this.gridColumnEx35.CurrencyDateField = null;
		this.gridColumnEx35.CurrencyField = null;
		this.gridColumnEx35.CurrencyFixed = null;
		this.gridColumnEx35.CurrencyFixedRate = 0.0;
		this.gridColumnEx35.CurrencyRateField = null;
		this.gridColumnEx35.FieldName = "Project";
		this.gridColumnEx35.Name = "gridColumnEx35";
		this.gridColumnEx35.PrimaryKey = false;
		this.gridColumnEx35.UnitField = null;
		this.gridColumnEx35.Visible = true;
		this.gridColumnEx35.VisibleIndex = 5;
		this.rep_LineProject.AutoHeight = false;
		this.rep_LineProject.AutoLoadDataSource = true;
		editorButtonImageOptions.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions2.Image");
		this.rep_LineProject.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_LineProject.ContainerControl = this;
		this.rep_LineProject.DisplayMember = "PrjName";
		this.rep_LineProject.DisplayMember2 = "";
		this.rep_LineProject.LoadDataSourceWhenEnter = false;
		this.rep_LineProject.Name = "rep_LineProject";
		this.rep_LineProject.SeparateCharacterForMulti = ';';
		this.rep_LineProject.SeparateString = ", ";
		this.rep_LineProject.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.Project;
		this.rep_LineProject.UsingAutoCompleteBox = false;
		this.rep_LineProject.UsingDataSource = true;
		this.rep_LineProject.ValidateWhenNoDataSource = false;
		this.rep_LineProject.ValueMember = "PrjCode";
		this.grd_BatchesIssue.AllowPasteExcel = true;
		this.grd_BatchesIssue.ClipboardData = null;
		this.grd_BatchesIssue.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_BatchesIssue.Location = new System.Drawing.Point(0, 0);
		this.grd_BatchesIssue.MainView = this.gv_BatchesIssue;
		this.grd_BatchesIssue.Name = "grd_BatchesIssue";
		this.grd_BatchesIssue.Size = new System.Drawing.Size(586, 210);
		this.grd_BatchesIssue.TabIndex = 1;
		this.grd_BatchesIssue.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_BatchesIssue });
		this.gv_BatchesIssue.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_BatchesIssue.ColumnKey = "DistNumber";
		this.gv_BatchesIssue.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[7] { this.gridColumnEx1, this.gridColumnEx2, this.gridColumnEx3, this.gridColumnEx4, this.gridColumnEx5, this.gridColumnEx6, this.gridColumnEx12 });
		this.gv_BatchesIssue.DisableFormSetting = false;
		this.gv_BatchesIssue.DisplayException = false;
		this.gv_BatchesIssue.EditColumn = null;
		this.gv_BatchesIssue.FieldCurrency = null;
		this.gv_BatchesIssue.FieldRate = null;
		this.gv_BatchesIssue.FormId = "oOutsourcing";
		this.gv_BatchesIssue.GridControl = this.grd_BatchesIssue;
		this.gv_BatchesIssue.IgnoreFilter = false;
		this.gv_BatchesIssue.IndexEditRow = -1;
		this.gv_BatchesIssue.IndicatorWidth = 50;
		this.gv_BatchesIssue.IsApzTable = false;
		this.gv_BatchesIssue.IsInsertNewRow = false;
		this.gv_BatchesIssue.IsLoadingData = false;
		this.gv_BatchesIssue.IsMappingContextAction = false;
		this.gv_BatchesIssue.ItemId = "OBATCHISSUSE";
		this.gv_BatchesIssue.LstColumnAllowBatch = null;
		this.gv_BatchesIssue.LstColumnNotAllowBatch = null;
		this.gv_BatchesIssue.LstColumnNotEditable = null;
		this.gv_BatchesIssue.LstColumNotSetting = null;
		this.gv_BatchesIssue.Name = "gv_BatchesIssue";
		this.gv_BatchesIssue.OptionsSelection.MultiSelect = true;
		this.gv_BatchesIssue.OptionsView.ColumnAutoWidth = false;
		this.gv_BatchesIssue.OptionsView.ShowGroupPanel = false;
		this.gv_BatchesIssue.UseBatchMod = false;
		this.gv_BatchesIssue.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_BatchesIssue_RowCellStyle);
		this.gv_BatchesIssue.ValidatingEditor += new DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventHandler(this.gv_BatchesIssue_ValidatingEditor);
		this.gv_BatchesIssue.InvalidValueException += new DevExpress.XtraEditors.Controls.InvalidValueExceptionEventHandler(this.gv_BatchesIssue_InvalidValueException);
		this.gridColumnEx1.Caption = "Serial/Batch";
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "DistNumber";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.PrimaryKey = true;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx1.Visible = true;
		this.gridColumnEx1.VisibleIndex = 0;
		this.gridColumnEx1.Width = 134;
		this.gridColumnEx2.Caption = "Bin";
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.FieldName = "FromBinCode";
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.UnitField = null;
		this.gridColumnEx2.Visible = true;
		this.gridColumnEx2.VisibleIndex = 1;
		this.gridColumnEx2.Width = 120;
		this.gridColumnEx3.Caption = "Available Qty.";
		this.gridColumnEx3.CurrencyDateField = null;
		this.gridColumnEx3.CurrencyField = null;
		this.gridColumnEx3.CurrencyFixed = null;
		this.gridColumnEx3.CurrencyFixedRate = 0.0;
		this.gridColumnEx3.CurrencyRateField = null;
		this.gridColumnEx3.FieldName = "AvailQty";
		this.gridColumnEx3.Name = "gridColumnEx3";
		this.gridColumnEx3.PrimaryKey = false;
		this.gridColumnEx3.UnitField = null;
		this.gridColumnEx3.Visible = true;
		this.gridColumnEx3.VisibleIndex = 2;
		this.gridColumnEx3.Width = 80;
		this.gridColumnEx4.Caption = "Quantity";
		this.gridColumnEx4.CurrencyDateField = null;
		this.gridColumnEx4.CurrencyField = null;
		this.gridColumnEx4.CurrencyFixed = null;
		this.gridColumnEx4.CurrencyFixedRate = 0.0;
		this.gridColumnEx4.CurrencyRateField = null;
		this.gridColumnEx4.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gridColumnEx4.FieldName = "Quantity";
		this.gridColumnEx4.Name = "gridColumnEx4";
		this.gridColumnEx4.PrimaryKey = false;
		this.gridColumnEx4.UnitField = null;
		this.gridColumnEx4.Visible = true;
		this.gridColumnEx4.VisibleIndex = 3;
		this.gridColumnEx5.Caption = "Line No.";
		this.gridColumnEx5.CurrencyDateField = null;
		this.gridColumnEx5.CurrencyField = null;
		this.gridColumnEx5.CurrencyFixed = null;
		this.gridColumnEx5.CurrencyFixedRate = 0.0;
		this.gridColumnEx5.CurrencyRateField = null;
		this.gridColumnEx5.FieldName = "LineNum";
		this.gridColumnEx5.Name = "gridColumnEx5";
		this.gridColumnEx5.PrimaryKey = false;
		this.gridColumnEx5.UnitField = null;
		this.gridColumnEx6.Caption = "Batch/Serial Entry";
		this.gridColumnEx6.CurrencyDateField = null;
		this.gridColumnEx6.CurrencyField = null;
		this.gridColumnEx6.CurrencyFixed = null;
		this.gridColumnEx6.CurrencyFixedRate = 0.0;
		this.gridColumnEx6.CurrencyRateField = null;
		this.gridColumnEx6.FieldName = "BtSAbsEntry";
		this.gridColumnEx6.Name = "gridColumnEx6";
		this.gridColumnEx6.PrimaryKey = false;
		this.gridColumnEx6.UnitField = null;
		this.gridColumnEx12.Caption = "From Bin";
		this.gridColumnEx12.CurrencyDateField = null;
		this.gridColumnEx12.CurrencyField = null;
		this.gridColumnEx12.CurrencyFixed = null;
		this.gridColumnEx12.CurrencyFixedRate = 0.0;
		this.gridColumnEx12.CurrencyRateField = null;
		this.gridColumnEx12.FieldName = "FromBinAbs";
		this.gridColumnEx12.Name = "gridColumnEx12";
		this.gridColumnEx12.PrimaryKey = false;
		this.gridColumnEx12.UnitField = null;
		this.splitContainerControl3.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainerControl3.Horizontal = false;
		this.splitContainerControl3.Location = new System.Drawing.Point(0, 0);
		this.splitContainerControl3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.splitContainerControl3.Name = "splitContainerControl3";
		this.splitContainerControl3.Panel1.Controls.Add(this.grd_Receipt);
		this.splitContainerControl3.Panel1.Text = "Panel1";
		this.splitContainerControl3.Panel2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
		this.splitContainerControl3.Panel2.Controls.Add(this.grd_BatchesReceipt);
		this.splitContainerControl3.Panel2.ShowCaption = true;
		this.splitContainerControl3.Panel2.Text = "Receipt Batch/Serial/Bin";
		this.splitContainerControl3.Size = new System.Drawing.Size(559, 468);
		this.splitContainerControl3.SplitterPosition = 223;
		this.splitContainerControl3.TabIndex = 2;
		this.splitContainerControl3.Text = "splitContainerControl3";
		this.splitContainerControl3.Resize += new System.EventHandler(this.splitContainerControl_Resize);
		this.grd_Receipt.AllowPasteExcel = true;
		this.grd_Receipt.ClipboardData = null;
		this.grd_Receipt.ContextMenuStrip = this.ctm_Receipt;
		this.grd_Receipt.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Receipt.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grd_Receipt.Location = new System.Drawing.Point(0, 0);
		this.grd_Receipt.MainView = this.gv_Receipt;
		this.grd_Receipt.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grd_Receipt.Name = "grd_Receipt";
		this.grd_Receipt.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.rep_LineProject1 });
		this.grd_Receipt.Size = new System.Drawing.Size(559, 223);
		this.grd_Receipt.TabIndex = 1;
		this.grd_Receipt.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Receipt });
		this.ctm_Receipt.ImageScalingSize = new System.Drawing.Size(20, 20);
		this.ctm_Receipt.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.removeRowToolStripMenuItem1 });
		this.ctm_Receipt.Name = "ctm_Receipt";
		this.ctm_Receipt.Size = new System.Drawing.Size(144, 26);
		this.removeRowToolStripMenuItem1.Name = "removeRowToolStripMenuItem1";
		this.removeRowToolStripMenuItem1.Size = new System.Drawing.Size(143, 22);
		this.removeRowToolStripMenuItem1.Text = "Remove Row";
		this.removeRowToolStripMenuItem1.Click += new System.EventHandler(this.removeRowToolStripMenuItem1_Click);
		this.gv_Receipt.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Receipt.ColumnKey = "ItemCode";
		this.gv_Receipt.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[9] { this.gridColumnEx14, this.gridColumnEx15, this.gridColumnEx16, this.gridColumnEx17, this.gridColumnEx18, this.gridColumnEx19, this.gridColumnEx20, this.gridColumnEx21, this.gridColumnEx41 });
		this.gv_Receipt.DisableFormSetting = false;
		this.gv_Receipt.DisplayException = false;
		this.gv_Receipt.EditColumn = null;
		this.gv_Receipt.FieldCurrency = null;
		this.gv_Receipt.FieldRate = null;
		this.gv_Receipt.FormId = "oOutsourcing";
		this.gv_Receipt.GridControl = this.grd_Receipt;
		this.gv_Receipt.IgnoreFilter = false;
		this.gv_Receipt.IndexEditRow = -1;
		this.gv_Receipt.IndicatorWidth = 50;
		this.gv_Receipt.IsApzTable = false;
		this.gv_Receipt.IsInsertNewRow = false;
		this.gv_Receipt.IsLoadingData = false;
		this.gv_Receipt.IsMappingContextAction = false;
		this.gv_Receipt.ItemId = "RECEIPT";
		this.gv_Receipt.LstColumnAllowBatch = null;
		this.gv_Receipt.LstColumnNotAllowBatch = null;
		this.gv_Receipt.LstColumnNotEditable = null;
		this.gv_Receipt.LstColumNotSetting = null;
		this.gv_Receipt.Name = "gv_Receipt";
		this.gv_Receipt.OptionsFind.AllowFindPanel = false;
		this.gv_Receipt.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Receipt.OptionsView.ColumnAutoWidth = false;
		this.gv_Receipt.OptionsView.ShowGroupPanel = false;
		this.gv_Receipt.UseBatchMod = false;
		this.gv_Receipt.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Receipt_RowCellStyle);
		this.gv_Receipt.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.gv_RowStyle);
		this.gv_Receipt.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gv_Receipt_FocusedRowChanged);
		this.gv_Receipt.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_Receipt_FocusedRowObjectChanged);
		this.gv_Receipt.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Receipt_CellValueChanged);
		this.gv_Receipt.BeforeLeaveRow += new DevExpress.XtraGrid.Views.Base.RowAllowEventHandler(this.gv_BeforeLeaveRow);
		this.gridColumnEx14.Caption = "Line No.";
		this.gridColumnEx14.CurrencyDateField = null;
		this.gridColumnEx14.CurrencyField = null;
		this.gridColumnEx14.CurrencyFixed = null;
		this.gridColumnEx14.CurrencyFixedRate = 0.0;
		this.gridColumnEx14.CurrencyRateField = null;
		this.gridColumnEx14.FieldName = "LineNum";
		this.gridColumnEx14.Name = "gridColumnEx14";
		this.gridColumnEx14.PrimaryKey = true;
		this.gridColumnEx14.UnitField = null;
		this.gridColumnEx15.Caption = "Item Code";
		this.gridColumnEx15.CurrencyDateField = null;
		this.gridColumnEx15.CurrencyField = null;
		this.gridColumnEx15.CurrencyFixed = null;
		this.gridColumnEx15.CurrencyFixedRate = 0.0;
		this.gridColumnEx15.CurrencyRateField = null;
		this.gridColumnEx15.FieldName = "ItemCode";
		this.gridColumnEx15.Name = "gridColumnEx15";
		this.gridColumnEx15.PrimaryKey = true;
		this.gridColumnEx15.UnitField = null;
		this.gridColumnEx15.Visible = true;
		this.gridColumnEx15.VisibleIndex = 0;
		this.gridColumnEx15.Width = 87;
		this.gridColumnEx16.Caption = "Item Name";
		this.gridColumnEx16.CurrencyDateField = null;
		this.gridColumnEx16.CurrencyField = null;
		this.gridColumnEx16.CurrencyFixed = null;
		this.gridColumnEx16.CurrencyFixedRate = 0.0;
		this.gridColumnEx16.CurrencyRateField = null;
		this.gridColumnEx16.FieldName = "ItemName";
		this.gridColumnEx16.Name = "gridColumnEx16";
		this.gridColumnEx16.PrimaryKey = false;
		this.gridColumnEx16.UnitField = null;
		this.gridColumnEx16.Visible = true;
		this.gridColumnEx16.VisibleIndex = 1;
		this.gridColumnEx16.Width = 118;
		this.gridColumnEx17.Caption = "Warehouse";
		this.gridColumnEx17.CurrencyDateField = null;
		this.gridColumnEx17.CurrencyField = null;
		this.gridColumnEx17.CurrencyFixed = null;
		this.gridColumnEx17.CurrencyFixedRate = 0.0;
		this.gridColumnEx17.CurrencyRateField = null;
		this.gridColumnEx17.FieldName = "WhsCode";
		this.gridColumnEx17.Name = "gridColumnEx17";
		this.gridColumnEx17.PrimaryKey = false;
		this.gridColumnEx17.UnitField = null;
		this.gridColumnEx17.Visible = true;
		this.gridColumnEx17.VisibleIndex = 2;
		this.gridColumnEx17.Width = 64;
		this.gridColumnEx18.Caption = "Quantity";
		this.gridColumnEx18.CurrencyDateField = null;
		this.gridColumnEx18.CurrencyField = null;
		this.gridColumnEx18.CurrencyFixed = null;
		this.gridColumnEx18.CurrencyFixedRate = 0.0;
		this.gridColumnEx18.CurrencyRateField = null;
		this.gridColumnEx18.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gridColumnEx18.FieldName = "Quantity";
		this.gridColumnEx18.Name = "gridColumnEx18";
		this.gridColumnEx18.PrimaryKey = false;
		this.gridColumnEx18.UnitField = null;
		this.gridColumnEx18.Visible = true;
		this.gridColumnEx18.VisibleIndex = 4;
		this.gridColumnEx19.Caption = "Factor";
		this.gridColumnEx19.CurrencyDateField = null;
		this.gridColumnEx19.CurrencyField = null;
		this.gridColumnEx19.CurrencyFixed = null;
		this.gridColumnEx19.CurrencyFixedRate = 0.0;
		this.gridColumnEx19.CurrencyRateField = null;
		this.gridColumnEx19.FieldName = "AmountFactor";
		this.gridColumnEx19.Name = "gridColumnEx19";
		this.gridColumnEx19.PrimaryKey = false;
		this.gridColumnEx19.UnitField = null;
		this.gridColumnEx19.Visible = true;
		this.gridColumnEx19.VisibleIndex = 3;
		this.gridColumnEx20.Caption = "Doc. Entry";
		this.gridColumnEx20.CurrencyDateField = null;
		this.gridColumnEx20.CurrencyField = null;
		this.gridColumnEx20.CurrencyFixed = null;
		this.gridColumnEx20.CurrencyFixedRate = 0.0;
		this.gridColumnEx20.CurrencyRateField = null;
		this.gridColumnEx20.FieldName = "DocEntry";
		this.gridColumnEx20.Name = "gridColumnEx20";
		this.gridColumnEx20.PrimaryKey = false;
		this.gridColumnEx20.UnitField = null;
		this.gridColumnEx21.Caption = "Managed by";
		this.gridColumnEx21.CurrencyDateField = null;
		this.gridColumnEx21.CurrencyField = null;
		this.gridColumnEx21.CurrencyFixed = null;
		this.gridColumnEx21.CurrencyFixedRate = 0.0;
		this.gridColumnEx21.CurrencyRateField = null;
		this.gridColumnEx21.FieldName = "ManBatchSer";
		this.gridColumnEx21.Name = "gridColumnEx21";
		this.gridColumnEx21.PrimaryKey = false;
		this.gridColumnEx21.UnitField = null;
		this.gridColumnEx41.Caption = "Project";
		this.gridColumnEx41.ColumnEdit = this.rep_LineProject1;
		this.gridColumnEx41.CurrencyDateField = null;
		this.gridColumnEx41.CurrencyField = null;
		this.gridColumnEx41.CurrencyFixed = null;
		this.gridColumnEx41.CurrencyFixedRate = 0.0;
		this.gridColumnEx41.CurrencyRateField = null;
		this.gridColumnEx41.FieldName = "Project";
		this.gridColumnEx41.Name = "gridColumnEx41";
		this.gridColumnEx41.PrimaryKey = false;
		this.gridColumnEx41.UnitField = null;
		this.gridColumnEx41.Visible = true;
		this.gridColumnEx41.VisibleIndex = 5;
		this.rep_LineProject1.AutoHeight = false;
		this.rep_LineProject1.AutoLoadDataSource = true;
		editorButtonImageOptions2.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions5.Image");
		this.rep_LineProject1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance4, appearanceHovered4, appearancePressed4, appearanceDisabled4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_LineProject1.ContainerControl = this;
		this.rep_LineProject1.DisplayMember = "PrjName";
		this.rep_LineProject1.DisplayMember2 = "";
		this.rep_LineProject1.LoadDataSourceWhenEnter = false;
		this.rep_LineProject1.Name = "rep_LineProject1";
		this.rep_LineProject1.SeparateCharacterForMulti = ';';
		this.rep_LineProject1.SeparateString = ", ";
		this.rep_LineProject1.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.Project;
		this.rep_LineProject1.UsingAutoCompleteBox = false;
		this.rep_LineProject1.UsingDataSource = true;
		this.rep_LineProject1.ValidateWhenNoDataSource = false;
		this.rep_LineProject1.ValueMember = "PrjCode";
		this.grd_BatchesReceipt.AllowPasteExcel = true;
		this.grd_BatchesReceipt.ClipboardData = null;
		this.grd_BatchesReceipt.ContextMenuStrip = this.ctm_IssueBatch;
		this.grd_BatchesReceipt.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_BatchesReceipt.Location = new System.Drawing.Point(0, 0);
		this.grd_BatchesReceipt.MainView = this.gv_BatchesReceipt;
		this.grd_BatchesReceipt.Name = "grd_BatchesReceipt";
		this.grd_BatchesReceipt.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.rep_bte_ToBinCode_Receipt });
		this.grd_BatchesReceipt.Size = new System.Drawing.Size(555, 210);
		this.grd_BatchesReceipt.TabIndex = 2;
		this.grd_BatchesReceipt.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_BatchesReceipt });
		this.ctm_IssueBatch.ImageScalingSize = new System.Drawing.Size(20, 20);
		this.ctm_IssueBatch.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.removeRowToolStripMenuItem2 });
		this.ctm_IssueBatch.Name = "ctm_IssueBatch";
		this.ctm_IssueBatch.Size = new System.Drawing.Size(144, 26);
		this.removeRowToolStripMenuItem2.Name = "removeRowToolStripMenuItem2";
		this.removeRowToolStripMenuItem2.Size = new System.Drawing.Size(143, 22);
		this.removeRowToolStripMenuItem2.Text = "Remove Row";
		this.removeRowToolStripMenuItem2.Click += new System.EventHandler(this.removeRowToolStripMenuItem2_Click);
		this.gv_BatchesReceipt.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_BatchesReceipt.ColumnKey = "DistNumber";
		this.gv_BatchesReceipt.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[6] { this.gridColumnEx7, this.gridColumnEx8, this.gridColumnEx9, this.gridColumnEx10, this.gridColumnEx11, this.gridColumnEx13 });
		this.gv_BatchesReceipt.DisableFormSetting = false;
		this.gv_BatchesReceipt.DisplayException = false;
		this.gv_BatchesReceipt.EditColumn = null;
		this.gv_BatchesReceipt.FieldCurrency = null;
		this.gv_BatchesReceipt.FieldRate = null;
		this.gv_BatchesReceipt.FormId = "oOutsourcing";
		this.gv_BatchesReceipt.GridControl = this.grd_BatchesReceipt;
		this.gv_BatchesReceipt.IgnoreFilter = false;
		this.gv_BatchesReceipt.IndexEditRow = -1;
		this.gv_BatchesReceipt.IndicatorWidth = 50;
		this.gv_BatchesReceipt.IsApzTable = false;
		this.gv_BatchesReceipt.IsInsertNewRow = false;
		this.gv_BatchesReceipt.IsLoadingData = false;
		this.gv_BatchesReceipt.IsMappingContextAction = false;
		this.gv_BatchesReceipt.ItemId = "OBATCHRECEIPT";
		this.gv_BatchesReceipt.LstColumnAllowBatch = null;
		this.gv_BatchesReceipt.LstColumnNotAllowBatch = null;
		this.gv_BatchesReceipt.LstColumnNotEditable = null;
		this.gv_BatchesReceipt.LstColumNotSetting = null;
		this.gv_BatchesReceipt.Name = "gv_BatchesReceipt";
		this.gv_BatchesReceipt.OptionsSelection.MultiSelect = true;
		this.gv_BatchesReceipt.OptionsView.ColumnAutoWidth = false;
		this.gv_BatchesReceipt.OptionsView.ShowGroupPanel = false;
		this.gv_BatchesReceipt.UseBatchMod = false;
		this.gv_BatchesReceipt.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_BatchesReceipt_CellValueChanged);
		this.gv_BatchesReceipt.ValidatingEditor += new DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventHandler(this.gv_BatchesReceipt_ValidatingEditor);
		this.gv_BatchesReceipt.InvalidValueException += new DevExpress.XtraEditors.Controls.InvalidValueExceptionEventHandler(this.gv_BatchesIssue_InvalidValueException);
		this.gridColumnEx7.Caption = "Serial/Batch";
		this.gridColumnEx7.CurrencyDateField = null;
		this.gridColumnEx7.CurrencyField = null;
		this.gridColumnEx7.CurrencyFixed = null;
		this.gridColumnEx7.CurrencyFixedRate = 0.0;
		this.gridColumnEx7.CurrencyRateField = null;
		this.gridColumnEx7.FieldName = "DistNumber";
		this.gridColumnEx7.Name = "gridColumnEx7";
		this.gridColumnEx7.PrimaryKey = true;
		this.gridColumnEx7.UnitField = null;
		this.gridColumnEx7.Visible = true;
		this.gridColumnEx7.VisibleIndex = 0;
		this.gridColumnEx7.Width = 126;
		this.gridColumnEx8.Caption = "Bin";
		this.gridColumnEx8.ColumnEdit = this.rep_bte_ToBinCode_Receipt;
		this.gridColumnEx8.CurrencyDateField = null;
		this.gridColumnEx8.CurrencyField = null;
		this.gridColumnEx8.CurrencyFixed = null;
		this.gridColumnEx8.CurrencyFixedRate = 0.0;
		this.gridColumnEx8.CurrencyRateField = null;
		this.gridColumnEx8.FieldName = "ToBinCode";
		this.gridColumnEx8.Name = "gridColumnEx8";
		this.gridColumnEx8.PrimaryKey = false;
		this.gridColumnEx8.UnitField = null;
		this.gridColumnEx8.Visible = true;
		this.gridColumnEx8.VisibleIndex = 1;
		this.gridColumnEx8.Width = 123;
		this.rep_bte_ToBinCode_Receipt.AutoHeight = false;
		this.rep_bte_ToBinCode_Receipt.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rep_bte_ToBinCode_Receipt.Name = "rep_bte_ToBinCode_Receipt";
		this.rep_bte_ToBinCode_Receipt.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_bte_FromBinCode_Receipt_ButtonPressed);
		this.gridColumnEx9.Caption = "Quantity";
		this.gridColumnEx9.CurrencyDateField = null;
		this.gridColumnEx9.CurrencyField = null;
		this.gridColumnEx9.CurrencyFixed = null;
		this.gridColumnEx9.CurrencyFixedRate = 0.0;
		this.gridColumnEx9.CurrencyRateField = null;
		this.gridColumnEx9.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gridColumnEx9.FieldName = "Quantity";
		this.gridColumnEx9.Name = "gridColumnEx9";
		this.gridColumnEx9.PrimaryKey = false;
		this.gridColumnEx9.UnitField = null;
		this.gridColumnEx9.Visible = true;
		this.gridColumnEx9.VisibleIndex = 2;
		this.gridColumnEx9.Width = 97;
		this.gridColumnEx10.Caption = "LineNum";
		this.gridColumnEx10.CurrencyDateField = null;
		this.gridColumnEx10.CurrencyField = null;
		this.gridColumnEx10.CurrencyFixed = null;
		this.gridColumnEx10.CurrencyFixedRate = 0.0;
		this.gridColumnEx10.CurrencyRateField = null;
		this.gridColumnEx10.FieldName = "LineNum";
		this.gridColumnEx10.Name = "gridColumnEx10";
		this.gridColumnEx10.PrimaryKey = false;
		this.gridColumnEx10.UnitField = null;
		this.gridColumnEx11.Caption = "Serial/Batch Entry";
		this.gridColumnEx11.CurrencyDateField = null;
		this.gridColumnEx11.CurrencyField = null;
		this.gridColumnEx11.CurrencyFixed = null;
		this.gridColumnEx11.CurrencyFixedRate = 0.0;
		this.gridColumnEx11.CurrencyRateField = null;
		this.gridColumnEx11.FieldName = "BtSAbsEntry";
		this.gridColumnEx11.Name = "gridColumnEx11";
		this.gridColumnEx11.PrimaryKey = false;
		this.gridColumnEx11.UnitField = null;
		this.gridColumnEx13.Caption = "To Bin Abs";
		this.gridColumnEx13.CurrencyDateField = null;
		this.gridColumnEx13.CurrencyField = null;
		this.gridColumnEx13.CurrencyFixed = null;
		this.gridColumnEx13.CurrencyFixedRate = 0.0;
		this.gridColumnEx13.CurrencyRateField = null;
		this.gridColumnEx13.FieldName = "ToBinAbs";
		this.gridColumnEx13.Name = "gridColumnEx13";
		this.gridColumnEx13.PrimaryKey = false;
		this.gridColumnEx13.UnitField = null;
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.EditControl = null;
		this.btn_Save.EditValue = "OK";
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(12, 579);
		this.btn_Save.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.ReadOnly = false;
		this.btn_Save.SettingInVisible = false;
		this.btn_Save.SettingReadonly = false;
		this.btn_Save.Size = new System.Drawing.Size(98, 22);
		this.btn_Save.TabIndex = 23;
		this.btn_Save.Text = "OK";
		this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
		this.rdg_InputMethod.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.rdg_InputMethod.EditControl = null;
		this.rdg_InputMethod.EditValue = "B";
		this.rdg_InputMethod.FieldName = "rdg_InputMethod";
		this.rdg_InputMethod.IsSearchControl = false;
		this.rdg_InputMethod.Location = new System.Drawing.Point(220, 578);
		this.rdg_InputMethod.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.rdg_InputMethod.Name = "rdg_InputMethod";
		this.rdg_InputMethod.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.rdg_InputMethod.Properties.Appearance.Options.UseBackColor = true;
		this.rdg_InputMethod.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.rdg_InputMethod.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[2]
		{
			new DevExpress.XtraEditors.Controls.RadioGroupItem("B", "BOM"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem("M", "Manual")
		});
		this.rdg_InputMethod.SettingInVisible = false;
		this.rdg_InputMethod.SettingReadonly = false;
		this.rdg_InputMethod.Size = new System.Drawing.Size(111, 22);
		this.rdg_InputMethod.TabIndex = 24;
		this.rdg_InputMethod.SelectedIndexChanged += new System.EventHandler(this.rdg_InputMethod_SelectedIndexChanged);
		this.labelLine5.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.labelLine5.EditControl = null;
		this.labelLine5.EditValue = "Base on";
		this.labelLine5.FieldName = null;
		this.labelLine5.HideWhenEmpty = false;
		this.labelLine5.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine5.Location = new System.Drawing.Point(144, 581);
		this.labelLine5.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine5.Name = "labelLine5";
		this.labelLine5.ReadOnly = false;
		this.labelLine5.SettingInVisible = false;
		this.labelLine5.SettingReadonly = false;
		this.labelLine5.ShowUnderLine = true;
		this.labelLine5.Size = new System.Drawing.Size(85, 20);
		this.labelLine5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine5.TabIndex = 25;
		this.labelLine5.Utext = "Base on";
		this.bteCFL_Account.EditControl = null;
		this.bteCFL_Account.FieldName = "txtAccount";
		this.bteCFL_Account.IsSearchControl = false;
		this.bteCFL_Account.Location = new System.Drawing.Point(666, 27);
		this.bteCFL_Account.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.bteCFL_Account.Name = "bteCFL_Account";
		this.bteCFL_Account.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions3.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions3.Image");
		this.bteCFL_Account.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance5, appearanceHovered5, appearancePressed5, appearanceDisabled5, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.bteCFL_Account.Properties.ContainerControl = null;
		this.bteCFL_Account.Properties.DisplayMember = "";
		this.bteCFL_Account.Properties.DisplayMember2 = "";
		this.bteCFL_Account.Properties.LoadDataSourceWhenEnter = false;
		this.bteCFL_Account.Properties.SeparateCharacterForMulti = ';';
		this.bteCFL_Account.Properties.SeparateString = ", ";
		this.bteCFL_Account.Properties.UsingAutoCompleteBox = false;
		this.bteCFL_Account.Properties.UsingDataSource = true;
		this.bteCFL_Account.Properties.ValidateWhenNoDataSource = false;
		this.bteCFL_Account.Properties.ValueMember = "";
		this.bteCFL_Account.SettingInVisible = false;
		this.bteCFL_Account.SettingReadonly = false;
		this.bteCFL_Account.Size = new System.Drawing.Size(135, 20);
		this.bteCFL_Account.TabIndex = 26;
		this.txt_JVNumber.EditControl = null;
		this.txt_JVNumber.FieldName = "txtJVNumber";
		this.txt_JVNumber.IsSearchControl = false;
		this.txt_JVNumber.Location = new System.Drawing.Point(143, 27);
		this.txt_JVNumber.Name = "txt_JVNumber";
		this.txt_JVNumber.SettingInVisible = false;
		this.txt_JVNumber.SettingReadonly = false;
		this.txt_JVNumber.Size = new System.Drawing.Size(104, 20);
		this.txt_JVNumber.TabIndex = 28;
		this.labelLine6.EditControl = null;
		this.labelLine6.EditValue = "JV Number";
		this.labelLine6.FieldName = null;
		this.labelLine6.HideWhenEmpty = false;
		this.labelLine6.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine6.Location = new System.Drawing.Point(9, 30);
		this.labelLine6.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine6.Name = "labelLine6";
		this.labelLine6.ReadOnly = false;
		this.labelLine6.SettingInVisible = false;
		this.labelLine6.SettingReadonly = false;
		this.labelLine6.ShowUnderLine = true;
		this.labelLine6.Size = new System.Drawing.Size(135, 20);
		this.labelLine6.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine6.TabIndex = 27;
		this.labelLine6.Utext = "JV Number";
		this.lbBranch.EditControl = null;
		this.lbBranch.EditValue = "Branch";
		this.lbBranch.FieldName = null;
		this.lbBranch.HideWhenEmpty = false;
		this.lbBranch.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbBranch.Location = new System.Drawing.Point(537, 55);
		this.lbBranch.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lbBranch.Name = "lbBranch";
		this.lbBranch.ReadOnly = false;
		this.lbBranch.SettingInVisible = false;
		this.lbBranch.SettingReadonly = false;
		this.lbBranch.ShowUnderLine = true;
		this.lbBranch.Size = new System.Drawing.Size(129, 20);
		this.lbBranch.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbBranch.TabIndex = 57;
		this.lbBranch.Utext = "Branch";
		this.txt_Branch.EditControl = null;
		this.txt_Branch.FieldName = "Branch";
		this.txt_Branch.IsSearchControl = false;
		this.txt_Branch.Location = new System.Drawing.Point(666, 52);
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
		this.txt_Branch.TabIndex = 56;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1188, 607);
		base.Controls.Add(this.lbBranch);
		base.Controls.Add(this.txt_Branch);
		base.Controls.Add(this.txt_JVNumber);
		base.Controls.Add(this.labelLine6);
		base.Controls.Add(this.bteCFL_Account);
		base.Controls.Add(this.rdg_InputMethod);
		base.Controls.Add(this.labelLine5);
		base.Controls.Add(this.btn_Save);
		base.Controls.Add(this.splitContainerControl1);
		base.Controls.Add(this.dateEx_DocDate);
		base.Controls.Add(this.labelLine4);
		base.Controls.Add(this.labelLine2);
		base.Controls.Add(this.txt_Comments);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.txt_DocNum);
		base.Controls.Add(this.labelLine3);
		base.FormId = "oOutsourcing";
		base.IconOptions.ShowIcon = false;
		base.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		base.Name = "frm_OutSourcing";
		this.Text = "Outsourcing";
		base.Load += new System.EventHandler(this.frm_OutSourcing_Load);
		((System.ComponentModel.ISupportInitialize)this.txt_DocNum.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Comments.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dateEx_DocDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dateEx_DocDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl2).EndInit();
		this.splitContainerControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Issue).EndInit();
		this.ctm_Issue.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Issue).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_LineProject).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_BatchesIssue).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_BatchesIssue).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl3).EndInit();
		this.splitContainerControl3.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Receipt).EndInit();
		this.ctm_Receipt.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Receipt).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_LineProject1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_BatchesReceipt).EndInit();
		this.ctm_IssueBatch.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_BatchesReceipt).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_bte_ToBinCode_Receipt).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rdg_InputMethod.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bteCFL_Account.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_JVNumber.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Branch.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
