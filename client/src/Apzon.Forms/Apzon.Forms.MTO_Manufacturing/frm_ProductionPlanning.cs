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
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.MasterData;
using Apzon.Entities.Models.Production;
using Apzon.Entities.Models.Production_Addon;
using Apzon.Entities.Models.UserFields;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Forms.Properties;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTab;
using SAPbobsCOM;

namespace Apzon.Forms.MTO_Manufacturing;

public class frm_ProductionPlanning : XtraFormEx
{
	public delegate void DelegatePC();

	private List<ProductionPlanningImpl> pl;

	private string FormController = "ProductionPlanning";

	private bool save;

	private bool changeReceipt;

	private bool taoLenh;

	private int rowHand = -1;

	private bool statusSaved;

	private bool statusChangedValueOnRow;

	private bool statusKeyCode = true;

	private static int batchQuantityCurrent;

	private static bool statusRowMO = false;

	private static bool beginForm = true;

	private static bool CheckValidateToPlanPro = true;

	private bool _edit;

	public DelegatePC PcClose;

	private IContainer components;

	private Panel panel1;

	private SimpleButtonEx btCancel;

	private SimpleButtonEx btSave;

	private PanelControl panelControl1;

	private ButtonEditEx txtCardCode;

	private TextEditEx txtCardName;

	private LabelLine lb_CustomerName;

	private LabelLine lb_CustomerCode;

	private SimpleButtonEx btSearch;

	private LabelLine lb_ToDate;

	private LabelLine lb_FromDate;

	private DateEditEx dtToDate;

	private DateEditEx dtFromDate;

	private SimpleButtonEx btnMOPlanning;

	private SimpleButtonEx btCheckCapacity;

	private XtraTabControlEx xtraTabControl1;

	private XtraTabPageEx tabMO;

	private SplitContainerControl splitContainerControl2;

	private GridControlEx grdMO;

	private GridViewEx gvMO;

	private GridColumnEx clMONo;

	private GridColumnEx clMoName;

	private GridColumnEx clPostDate;

	private GridColumnEx clDueDate;

	private GridColumnEx clRouting;

	private GridColumnEx clBatchToIssue;

	private GridColumnEx clUoM;

	private GridColumnEx clQuantity;

	private GridColumnEx clWidth;

	private GridColumnEx clLength;

	private GridColumnEx clProject;

	private GridColumnEx clFixedBatch;

	private GridColumnEx clStatus;

	private GridColumnEx clRemark;

	private XtraTabControlEx xtratabDoc;

	private XtraTabPageEx tab_Routing;

	private GridControlEx grdRouting;

	private GridViewEx gvRouting;

	private GridColumnEx clOpCode;

	private GridColumnEx clSupOpCode;

	private RepositoryItemButtonEdit repSupCode;

	private GridColumnEx clWorkCenter;

	private GridColumnEx clCtrKey;

	private GridColumnEx clBaseQty;

	private GridColumnEx clDescription;

	private GridColumnEx colRemark;

	private GridColumnEx columUOm;

	private GridColumnEx clSetUpTime;

	private GridColumnEx clLaborTime;

	private GridColumnEx clMachineTime;

	private GridColumnEx clPRT;

	private GridColumnEx clBOM;

	private GridColumnEx clPrOp;

	private GridColumnEx clNextOp;

	private XtraTabPageEx tab_ReceiptFromProduction;

	private GridControlEx grdReceipt;

	private GridViewEx gvReceipt;

	private GridColumnEx clItemCode;

	private GridColumnEx clItemName;

	private GridColumnEx clOrderCode;

	private GridColumnEx clOderLine;

	private GridColumnEx clDelDate;

	private RepositoryItemDateEdit repDelDate;

	private GridColumnEx clBatch;

	private GridColumnEx colWidth;

	private GridColumnEx colLength;

	private GridColumnEx colUOM;

	private GridColumnEx colQuantity;

	private GridColumnEx colWHse;

	private GridColumnEx colProject;

	private GridColumnEx colOperation;

	private XtraTabPageEx tab_Attachment;

	private GridControlEx gridControl3;

	private GridViewEx gridView3;

	private GridColumnEx gcPath;

	private GridColumnEx gcFileName;

	private GridColumnEx gcExtention;

	private GridColumnEx gcDownload;

	private XtraTabPageEx tabPlaned;

	private XtraTabPageEx tabRelease;

	private XtraTabPageEx tabClose;

	private GridViewEx gridView1;

	private GridViewEx gridView2;

	private GridViewEx gridView4;

	private GridViewEx gridView5;

	private GridColumnEx colMONo;

	private GridColumnEx colLineNum;

	private GridColumnEx clDelTime;

	private GridColumnEx clBatchIssue;

	private GridColumnEx clBatchComplete;

	private GridColumnEx clThickPlan;

	private GridColumnEx WidthPlan;

	private GridColumnEx LengthPlan;

	private GridColumnEx ThickFinal;

	private GridColumnEx WidthFinal;

	private GridColumnEx LengthFinal;

	private GridColumnEx OcrCode;

	private GridColumnEx OcrCode2;

	private GridColumnEx OcrCode3;

	private GridColumnEx OcrCode4;

	private GridColumnEx OcrCode5;

	private GridColumnEx UoM;

	private GridColumnEx OpCode;

	private GridColumnEx Project;

	private GridColumnEx Comments;

	private GridColumnEx MONo;

	private GridColumnEx LineNum;

	private GridColumnEx colUoMTime;

	private GridColumnEx UoMSetup;

	private GridColumnEx UoMLabor;

	private GridColumnEx UoMMachine;

	private GridColumnEx PrtCode;

	private GridColumnEx ItemCode;

	private RepositoryItemLookUpEdit rep_Status;

	private GridColumnEx colItemCode;

	private GridColumnEx clDistRule1;

	private GridColumnEx clDistRule2;

	private GridColumnEx clDistRule3;

	private GridColumnEx clDistRule4;

	private GridColumnEx clDistRule5;

	private SimpleButtonEx btn;

	private SimpleButtonEx btnRelease;

	private GridControlEx grd_Release;

	private GridViewEx gv_Release;

	private GridColumnEx gridColumn10;

	private GridColumnEx gridColumn11;

	private GridColumnEx gridColumn12;

	private GridColumnEx gridColumn13;

	private GridColumnEx gridColumn14;

	private GridColumnEx gridColumn15;

	private GridColumnEx gridColumn16;

	private GridColumnEx gridColumn17;

	private GridColumnEx gridColumn18;

	private GridColumnEx gridColumn19;

	private GridColumnEx gridColumn20;

	private RepositoryItemCheckEdit repFixBatch;

	private GridColumnEx FixBatch;

	private GridColumnEx DocEntrySO;

	private GridColumnEx LineNumSO;

	private RepositoryItemButtonEdit repOpCode;

	private RepositoryItemLookUpEdit repUOM;

	private RepositoryItemButtonEdit repOcr1;

	private RepositoryItemButtonEdit repOcr2;

	private RepositoryItemButtonEdit repOcr3;

	private RepositoryItemButtonEdit repOcr4;

	private RepositoryItemButtonEdit repOcr5;

	private RepositoryItemButtonEdit repItemCode;

	private RepositoryItemButtonEdit repWorkCenter;

	private RepositoryItemButtonEdit re_ControlKey;

	private RepositoryItemCalcEdit repTime;

	private RepositoryItemCheckEdit repPRT;

	private RepositoryItemButtonEdit repPreOp;

	private RepositoryItemButtonEdit repNextOp;

	private RepositoryItemButtonEdit reitemcode;

	private ContextMenuStrip ctxDocsMO;

	private ToolStripMenuItem deleteRowToolStripMenuItem;

	private GridColumnEx clBatchQuantity;

	private GridColumnEx columBatchIssue;

	private RepositoryItemComboBox rep;

	private RepositoryItemComboBox repBatComplete;

	private RepositoryItemButtonEdit repGetBatch;

	private ContextMenuStrip contextRouting;

	private ToolStripMenuItem deleteRowToolStripMenuItem1;

	private ToolStripMenuItem insertRowToolStripMenuItem;

	private RepositoryItemLookUpEdit repUOMReceipt;

	private RepositoryItemLookUpEdit repUOMRouting;

	private RepositoryItemButtonEdit repRouting;

	private SimpleButtonEx btn_Plan;

	private GridControlEx grd_Planed;

	private GridViewEx gv_Planed;

	private GridColumnEx columMoNo;

	private GridColumnEx columMoName;

	private GridColumnEx colItemName;

	private GridColumnEx clumItemName;

	private GridColumnEx clumQuantity;

	private GridColumnEx coluOrDer;

	private GridColumnEx columnWhse;

	private GridColumnEx columDueDate;

	private GridColumnEx columnRemark;

	private GridColumnEx columnBatchIssue;

	private GridColumnEx clProduc;

	private GridColumnEx Whse;

	private RepositoryItemButtonEdit repWhse;

	private GridControlEx gridControl1;

	private GridViewEx gridView6;

	private GridColumnEx gridColumn1;

	private GridColumnEx gridColumn2;

	private GridColumnEx gridColumn3;

	private GridColumnEx gridColumn4;

	private GridColumnEx gridColumn5;

	private GridColumnEx gridColumn6;

	private GridColumnEx gridColumn7;

	private GridColumnEx gridColumn8;

	private GridColumnEx gridColumn9;

	private GridColumnEx gridColumn21;

	private GridColumnEx gridColumn22;

	private GridColumnEx colBatchComplete;

	private RepositoryItemButtonEdit repoWhse;

	private GridColumnEx columBatchComplete;

	private GridColumnEx gridColumn23;

	private GridColumnEx gridColumn24;

	private GridColumnEx gridColumn25;

	private GridColumnEx gridColumn26;

	private GridColumnEx gridColumn27;

	private GridColumnEx gridColumn28;

	private GridColumnEx gridColumn29;

	private GridColumnEx gridColumn30;

	private RepositoryItemMemoEdit repositoryItemMemoEdit1;

	private RepositoryItemMemoEdit repositoryItemMemoEdit2;

	private RepositoryItemMemoEdit repositoryItemMemoEdit4;

	private RepositoryItemMemoExEdit repositoryItemMemoExEdit1;

	private RepositoryItemMemoExEdit repositoryItemMemoExEdit2;

	private RepositoryItemMemoExEdit repositoryItemMemoExEdit4;

	private RepositoryItemMemoExEdit repositoryItemMemoExEdit3;

	private RepositoryItemMemoExEdit repositoryItemMemoExEdit6;

	private RepositoryItemMemoExEdit repositoryItemMemoExEdit5;

	private RepositoryItemMemoExEdit repositoryItemMemoExEdit7;

	private RepositoryItemComboBox repSaleorderNo;

	private RepositoryItemMemoEdit repositoryItemMemoEdit3;

	private RepositoryItemComboBox repSOLine;

	private RepositoryItemButtonEdit rep_WHSE;

	public frm_ProductionPlanning(DataTable dt)
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_ProductionPlanning"))
		{
			InitializeComponent();
			SettingLanguage();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_ProductionPlanning" && x.authoriza == "F"))
			{
				WinFuntions.DisableAllControl(Controls);
				btCancel.Enabled = true;
			}
			return;
		}
		throw new Exception(Apzon.Commons.Properties.Resources.Msg_FormAccessDenied);
	}

	public frm_ProductionPlanning()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_ProductionPlanning"))
		{
			InitializeComponent();
			SettingLanguage();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_ProductionPlanning" && x.authoriza == "F"))
			{
				WinFuntions.DisableAllControl(Controls);
				btCancel.Enabled = true;
			}
			return;
		}
		throw new Exception(Apzon.Commons.Properties.Resources.Msg_FormAccessDenied);
	}

	private void btSave_Click(object sender, EventArgs e)
	{
		try
		{
			statusChangedValueOnRow = false;
			DialogResult dialogResult = XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
			if (DialogResult.Yes == dialogResult)
			{
				WinCommons._OpenProcessing();
				SaveFunction();
				WinCommons._CloseProcessing();
			}
			if (pl == null || pl.Count == 0)
			{
				beginForm = false;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void frm_ProductionPlanning_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if (_edit)
			{
				if (XtraMessageBox.Show(this, "Are you sure?", "Do you still want ... ?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
				{
					e.Cancel = true;
				}
				_edit = false;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Planed_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			_edit = true;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Release_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			_edit = true;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rep_WHSE_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.Warehouse, multiSelect: false, string.Empty);
				frm_ChooseFromList2.ShowDialog(this);
				if (frm_ChooseFromList2.ChooseItem != null)
				{
					WarehouseImpl warehouseImpl = (WarehouseImpl)frm_ChooseFromList2.ChooseItem;
					if (!string.IsNullOrEmpty(warehouseImpl.WhsCode))
					{
						gvReceipt.SetRowCellValue(gvReceipt.FocusedRowHandle, gvRouting.Columns["Whse"], warehouseImpl.WhsCode);
					}
				}
			}
			else
			{
				string text = Function.ToString(gvReceipt.GetRowCellValue(gvReceipt.FocusedRowHandle, "Whse"));
				if (!string.IsNullOrEmpty(text))
				{
					this.DrillDown(ApzObjectTypes.oWarehouses, ApzObjectTypes.oWarehouses, text);
				}
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gvRouting_RowCellClick(object sender, RowCellClickEventArgs e)
	{
	}

	private void gvRouting_MouseEnter(object sender, EventArgs e)
	{
	}

	private void frm_ProductionPlanning_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Control || e.KeyCode == Keys.S)
		{
			DialogResult dialogResult = XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo);
			if (DialogResult.Yes == dialogResult)
			{
				SaveFunction();
			}
		}
		if (e.KeyCode == Keys.Control || e.KeyCode == Keys.P)
		{
			DialogResult dialogResult2 = XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCreateProductionPlan, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo);
			if (DialogResult.Yes == dialogResult2)
			{
				taoLenh = true;
				SaveFunction();
			}
		}
	}

	private bool FunctionCheckToCreatProPlan()
	{
		try
		{
			for (int i = 0; i < gvRouting.RowCount; i++)
			{
				if (string.IsNullOrEmpty(Function.ToString(gvRouting.GetRowCellValue(i, "BatchIssue"))))
				{
					CheckValidateToPlanPro = false;
					gvRouting.FocusedRowHandle = i;
					GridColumn focusedColumn = gvRouting.Columns.ColumnByName("BatchIssue");
					gvRouting.FocusedColumn = focusedColumn;
					WinCommons.Error(Enums.MessageCode.BatchInTabRoutingIsEmpty, "");
					break;
				}
				if (!string.IsNullOrEmpty(Function.ToString(gvRouting.GetRowCellValue(i, "OpCode"))) && string.IsNullOrEmpty(Function.ToString(gvRouting.GetRowCellValue(i, "BatchQuantity"))))
				{
					CheckValidateToPlanPro = false;
					gvRouting.FocusedRowHandle = i;
					GridColumn focusedColumn2 = gvRouting.Columns.ColumnByName("BatchQuantity");
					gvRouting.FocusedColumn = focusedColumn2;
					WinCommons.Error(Enums.MessageCode.QuantityInTabRoutingIsEmpty, "");
					break;
				}
				if (!string.IsNullOrEmpty(Function.ToString(gvRouting.GetRowCellValue(i, "Whse"))))
				{
					CheckValidateToPlanPro = false;
					gvRouting.FocusedRowHandle = i;
					GridColumn focusedColumn3 = gvRouting.Columns.ColumnByName("Whse");
					gvRouting.FocusedColumn = focusedColumn3;
					WinCommons.Error(Enums.MessageCode.WarehousrInTabRoutingIsEmpty, "");
					break;
				}
			}
			for (int j = 0; j < gvReceipt.RowCount; j++)
			{
				if (string.IsNullOrEmpty(Function.ToString(gvReceipt.GetRowCellValue(j, "BatchIssue"))))
				{
					CheckValidateToPlanPro = false;
					gvReceipt.FocusedRowHandle = j;
					GridColumn focusedColumn4 = gvReceipt.Columns.ColumnByName("BatchIssue");
					gvReceipt.FocusedColumn = focusedColumn4;
					WinCommons.Error(Enums.MessageCode.BatchIssueInTabReceiptIsEmpty, "");
					break;
				}
				if (string.IsNullOrEmpty(Function.ToString(gvReceipt.GetRowCellValue(j, "BatchComplete"))))
				{
					CheckValidateToPlanPro = false;
					gvReceipt.FocusedRowHandle = j;
					GridColumn focusedColumn5 = gvReceipt.Columns.ColumnByName("BatchComplete");
					gvReceipt.FocusedColumn = focusedColumn5;
					WinCommons.Error(Enums.MessageCode.BatchCompleteInTabReceiptIsEmpty, "");
					break;
				}
				if (string.IsNullOrEmpty(Function.ToString(gvReceipt.GetRowCellValue(j, "SaleOrder"))))
				{
					CheckValidateToPlanPro = false;
					gvReceipt.FocusedRowHandle = j;
					GridColumn focusedColumn6 = gvReceipt.Columns.ColumnByName("SaleOrder");
					gvReceipt.FocusedColumn = focusedColumn6;
					WinCommons.Error(Enums.MessageCode.OrderNoInTabReceiptIsEmpty, "");
					break;
				}
				if (string.IsNullOrEmpty(Function.ToString(gvReceipt.GetRowCellValue(j, "LineSale"))))
				{
					CheckValidateToPlanPro = false;
					gvReceipt.FocusedRowHandle = j;
					GridColumn focusedColumn7 = gvReceipt.Columns.ColumnByName("LineSale");
					gvReceipt.FocusedColumn = focusedColumn7;
					WinCommons.Error(Enums.MessageCode.OrderLineInTabReceiptIsEmpty, "");
					break;
				}
			}
			return CheckValidateToPlanPro;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return false;
		}
	}

	private void gvReceipt_MouseUp(object sender, MouseEventArgs e)
	{
	}

	private void frm_ProductionPlanning_Load(object sender, EventArgs e)
	{
		try
		{
			LoadMOPlanningByCustomer();
			LoadDocs();
			LoadInIt();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gvRouting_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			_edit = true;
			if (!statusChangedValueOnRow)
			{
				statusChangedValueOnRow = true;
			}
			int num = Function.ParseInt(gvRouting.GetRowCellValue(gvRouting.FocusedRowHandle, "BatchQuantity"));
			if (e.Column.FieldName == "BatchQuantity" && num != batchQuantityCurrent)
			{
				if (num != 0)
				{
					DialogResult dialogResult = XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmRefreshDataReceiptTab, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo);
					if (DialogResult.Yes == dialogResult)
					{
						if (e.Column.FieldName == "BatchQuantity")
						{
							ReceiptFromBatchQuantity();
						}
						batchQuantityCurrent = Function.ParseInt(gvRouting.GetRowCellValue(gvRouting.FocusedRowHandle, "BatchQuantity"));
					}
					else
					{
						gvRouting.SetRowCellValue(gvRouting.FocusedRowHandle, "BatchQuantity", batchQuantityCurrent);
					}
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.InQuantityColumnEnterGreaterThan0, "");
				}
			}
			for (int i = 0; i < gvRouting.RowCount; i++)
			{
				if (i != 0 && i != gvRouting.RowCount)
				{
					if (e.Column.FieldName == "OpCode" || e.Column.FieldName == "SubOpCode")
					{
						if (!string.IsNullOrEmpty(Function.ToString(gvRouting.GetRowCellValue(i, "OpCode"))))
						{
							gvRouting.SetRowCellValue(i - 1, "NextOp", gvRouting.GetRowCellValue(i, "OpCode"));
							gvRouting.SetRowCellValue(i + 1, "PrevOp", gvRouting.GetRowCellValue(i, "OpCode"));
						}
						if (!string.IsNullOrEmpty(Function.ToString(gvRouting.GetRowCellValue(i, "SubOpCode"))))
						{
							gvRouting.SetRowCellValue(i - 1, "NextOp", gvRouting.GetRowCellValue(i, "SubOpCode"));
							gvRouting.SetRowCellValue(i + 1, "PrevOp", gvRouting.GetRowCellValue(i, "SubOpCode"));
						}
					}
				}
				else if (i == 0)
				{
					if (e.Column.FieldName == "OpCode" || e.Column.FieldName == "SubOpCode")
					{
						if (!string.IsNullOrEmpty(Function.ToString(gvRouting.GetRowCellValue(i, "OpCode"))))
						{
							gvRouting.SetRowCellValue(i + 1, "PrevOp", gvRouting.GetRowCellValue(i, "OpCode"));
						}
						if (!string.IsNullOrEmpty(Function.ToString(gvRouting.GetRowCellValue(i, "SubOpCode"))))
						{
							gvRouting.SetRowCellValue(i + 1, "PrevOp", gvRouting.GetRowCellValue(i, "SubOpCode"));
						}
					}
				}
				else if (e.Column.FieldName == "OpCode" || e.Column.FieldName == "SubOpCode")
				{
					if (!string.IsNullOrEmpty(Function.ToString(gvRouting.GetRowCellValue(i, "OpCode"))))
					{
						gvRouting.SetRowCellValue(i - 1, "NextOp", gvRouting.GetRowCellValue(i, "OpCode"));
					}
					if (!string.IsNullOrEmpty(Function.ToString(gvRouting.GetRowCellValue(i, "SubOpCode"))))
					{
						gvRouting.SetRowCellValue(i - 1, "NextOp", gvRouting.GetRowCellValue(i, "SubOpCode"));
					}
				}
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gvRouting_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
	{
	}

	private async void LoadDocs()
	{
		_ = 1;
		try
		{
			if (pl == null || pl.Count == 0)
			{
				ProductionPlanningImpl obj = new ProductionPlanningImpl();
				DataTable dataTable = await AwaitSearch(obj);
				if (dataTable != null && dataTable.Rows.Count > 0)
				{
					grdMO.DataSource = dataTable;
					grdMO.RefreshDataSource();
				}
				return;
			}
			new List<ProductionPlanningImpl>();
			for (int i = 0; i < pl.Count - 1; i++)
			{
				for (int j = i + 1; j < pl.Count; j++)
				{
					if ((pl[i].BatchToIssue.Contains(pl[j].BatchToIssue) && pl[i].ItemCode == pl[j].ItemCode) || (pl[j].BatchToIssue.Contains(pl[i].BatchToIssue) && pl[i].ItemCode == pl[j].ItemCode))
					{
						ProductionPlanningImpl PPJoin = new ProductionPlanningImpl();
						ProductionPlanningImpl productionPlanningImpl = PPJoin;
						productionPlanningImpl.MONo = await GetMaxMOCode() + i;
						PPJoin.RouCode = pl[i].RouCode;
						DateTime postDate = ((pl[i].PostDate > pl[j].PostDate) ? pl[i].PostDate : pl[j].PostDate);
						PPJoin.PostDate = postDate;
						DateTime dueDate = ((pl[i].DueDate < pl[j].DueDate) ? pl[i].DueDate : pl[j].DueDate);
						PPJoin.DueDate = dueDate;
						if (pl[i].BatchToIssue.Contains(pl[j].BatchToIssue))
						{
							PPJoin.BatchToIssue = pl[i].BatchToIssue;
						}
						else if (pl[j].BatchToIssue.Contains(pl[i].BatchToIssue))
						{
							PPJoin.BatchToIssue = pl[j].BatchToIssue;
						}
						else
						{
							PPJoin.BatchToIssue = pl[i].BatchToIssue;
						}
						PPJoin.ItemCode = pl[i].ItemCode;
						PPJoin.ItemName = pl[i].ItemName;
						PPJoin.UoM = pl[i].UoM;
						PPJoin.Quantity = pl[i].Quantity + pl[j].Quantity;
						PPJoin.Width = ((pl[i].Quantity > pl[j].Quantity) ? pl[i].Quantity : pl[j].Quantity);
						PPJoin.Length = ((pl[i].Quantity < pl[j].Quantity) ? pl[i].Quantity : pl[j].Quantity);
						PPJoin.OcrCode = pl[i].OcrCode;
						PPJoin.OcrCode2 = pl[i].OcrCode2;
						PPJoin.OcrCode3 = pl[i].OcrCode3;
						PPJoin.OcrCode4 = pl[i].OcrCode4;
						PPJoin.OcrCode5 = pl[i].OcrCode5;
						PPJoin.Project = pl[i].Project;
						PPJoin.BatchSOFixed = pl[i].BatchSOFixed;
						PPJoin.Status = "";
						PPJoin.Remark = pl[i].Remark;
						PPJoin.SONo = pl[i].SONo;
						PPJoin.ShipDate = pl[i].ShipDate;
						PPJoin.Whse = pl[i].Whse;
						PPJoin.Operation = "";
						PPJoin.OrderLine = pl[i].OrderLine;
						PPJoin.LineSale = pl[i].LineSale;
						PPJoin.DocEntrySO = pl[i].DocEntrySO + "," + pl[j].DocEntrySO;
						PPJoin.LineNumSO = pl[i].LineNumSO + "," + pl[j].LineNumSO;
						pl[i] = PPJoin;
						PPJoin.MOName = "Master Order of Sale Order No " + PPJoin.DocEntrySO;
						pl.Remove(pl[j]);
						j--;
					}
				}
			}
			grdMO.DataSource = pl;
			grdMO.RefreshDataSource();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gvMO_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			if (gvMO.FocusedRowHandle != rowHand && statusChangedValueOnRow)
			{
				DialogResult dialogResult = XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
				if (DialogResult.Yes == dialogResult)
				{
					rowHand = gvMO.FocusedRowHandle;
					GetDoc();
					statusChangedValueOnRow = false;
				}
				else
				{
					gvMO.FocusedRowHandle = rowHand;
				}
			}
			if (gvMO.FocusedRowHandle != rowHand && !statusChangedValueOnRow)
			{
				rowHand = gvMO.FocusedRowHandle;
				GetDoc();
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private bool CheckValidateProPlan(DataTable dt)
	{
		bool result = true;
		try
		{
			if (dt.Rows.Count > 0)
			{
				for (int i = 0; i < dt.Rows.Count; i++)
				{
					if (string.IsNullOrEmpty(Function.ToString(dt.Rows[i]["Quantity"])) || Function.ParseInt(dt.Rows[i]["Quantity"]) == 0)
					{
						result = false;
					}
				}
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		return result;
	}

	private void repWhse_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				string text = Function.ToString(gvRouting.GetFocusedRowCellValue("Whse"));
				string vatType = Function.ToString(gvRouting.GetFocusedRowCellValue("ItemCode"));
				frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.WareHouseByItemCode, multiSelect: false, string.Empty, vatType);
				frm_ChooseFromList2.ShowDialog(this);
				if (!((frm_ChooseFromList2.ChooseItem != null) ? true : false))
				{
					return;
				}
				DataRowView dataRowView = (DataRowView)frm_ChooseFromList2.ChooseItem;
				if (!string.IsNullOrEmpty(Function.ToString(dataRowView["WhsCode"])))
				{
					gvRouting.SetRowCellValue(gvRouting.FocusedRowHandle, gvRouting.Columns["Whse"], dataRowView["WhsCode"]);
					if (Function.ToString(dataRowView["WhsCode"]).ToUpper() != text.ToUpper())
					{
						gvRouting.SetFocusedRowCellValue(gvRouting.Columns["Whse"], dataRowView["WhsCode"]);
					}
				}
			}
			else
			{
				string text2 = Function.ToString(gvRouting.GetRowCellValue(gvRouting.FocusedRowHandle, "Whse"));
				if (!string.IsNullOrEmpty(text2))
				{
					this.DrillDown(ApzObjectTypes.oWarehouses, ApzObjectTypes.oWarehouses, text2);
				}
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gvMO_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			_edit = true;
			if (!statusChangedValueOnRow)
			{
				statusChangedValueOnRow = true;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gvReceipt_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			_edit = true;
			if (!statusChangedValueOnRow)
			{
				statusChangedValueOnRow = true;
			}
			if (e.Column.FieldName == "Width" || e.Column.FieldName == "Length" || e.Column.FieldName == "Quantity")
			{
				float num = Function.ParseFloat(Function.ParseFloat(gvReceipt.GetRowCellValue(gvReceipt.FocusedRowHandle, "Width"))) * Function.ParseFloat(gvReceipt.GetRowCellValue(gvReceipt.FocusedRowHandle, "Length")) * Function.ParseFloat(gvReceipt.GetRowCellValue(gvReceipt.FocusedRowHandle, "Quantity")) / 100f;
				gvReceipt.SetRowCellValue(gvReceipt.FocusedRowHandle, "Area", num);
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gvRouting_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
	{
	}

	private void gvRouting_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			batchQuantityCurrent = Function.ParseInt(gvRouting.GetRowCellValue(gvRouting.FocusedRowHandle, "BatchQuantity"));
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void gvRouting_FocusedColumnChanged(object sender, FocusedColumnChangedEventArgs e)
	{
	}

	private async void gvRouting_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == "BatchQuantity")
			{
				string keyCode = Function.ToString(gvRouting.GetRowCellValue(gvRouting.FocusedRowHandle, "KeyCode"));
				if (!(await AwaitCheckControlKey(keyCode)))
				{
					XtraMessageBox.Show("Error! This ControlKey don't allow !");
					gvRouting.SetRowCellValue(gvRouting.FocusedRowHandle, "BatchQuantity", 0);
				}
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void panel1_Paint(object sender, PaintEventArgs e)
	{
	}

	private async Task<int> GetMaxMOCode()
	{
		try
		{
			using HttpClientHelper<ProductionPlanningImpl> _helper = new HttpClientHelper<ProductionPlanningImpl>(Configuration.ApiServer, "ProductionPlanning");
			return await _helper.GetIntAsync("GetMaxMOCode") + 1;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return 0;
		}
	}

	private void btSearch_Click(object sender, EventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			FunctionSearch();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void SelectTabDoc()
	{
		_ = 5;
		try
		{
			int i;
			if (xtraTabControl1.SelectedTabPage.Equals(tabPlaned))
			{
				if (Configuration.MoPlanning == "1")
				{
					DataTable dataSource = await GetPlan(1);
					grd_Planed.DataSource = dataSource;
					grd_Planed.RefreshDataSource();
				}
				else
				{
					DataTable dt = await GetRelease();
					if (!dt.Columns.Contains("BatchComplete"))
					{
						dt.Columns.Add("BatchComplete", typeof(string));
					}
					for (i = 0; i < dt.Rows.Count; i++)
					{
						string batchComplete = Function.ToString(dt.Rows[i]["BatchIssue"]);
						string value = await SetBatchComplete(batchComplete);
						dt.Rows[i]["BatchComplete"] = value;
					}
					grd_Planed.DataSource = dt;
					grd_Planed.RefreshDataSource();
				}
			}
			if (xtraTabControl1.SelectedTabPage.Equals(tabRelease) && !(Configuration.MoPlanning == "1"))
			{
				i = Function.ParseInt(gv_Planed.GetRowCellValue(gv_Planed.FocusedRowHandle, "MONo"));
				DataTable dt = await GetClosed();
				if (!dt.Columns.Contains("BatchComplete"))
				{
					dt.Columns.Add("BatchComplete", typeof(string));
				}
				for (int j = 0; j < dt.Rows.Count; j++)
				{
					string batchComplete2 = Function.ToString(dt.Rows[j]["BatchIssue"]);
					string value2 = await SetBatchComplete(batchComplete2);
					dt.Rows[j]["BatchComplete"] = value2;
				}
				grd_Release.DataSource = dt;
				grd_Release.RefreshDataSource();
				for (int k = 0; k < dt.Rows.Count; k++)
				{
					if (Function.ParseInt(dt.Rows[k]["MONo"]) == i)
					{
						gv_Release.FocusedRowHandle = k;
					}
				}
			}
			if (!xtraTabControl1.SelectedTabPage.Equals(tabClose) || Configuration.MoPlanning == "1")
			{
				return;
			}
			i = Function.ParseInt(gv_Planed.GetRowCellValue(gv_Planed.FocusedRowHandle, "MONo"));
			DataTable dataTable = await GetClosed();
			grd_Release.DataSource = dataTable;
			grd_Release.RefreshDataSource();
			for (int l = 0; l < dataTable.Rows.Count; l++)
			{
				if (Function.ParseInt(dataTable.Rows[l]["MONo"]) == i)
				{
					gv_Release.FocusedRowHandle = l;
				}
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void FunctionSearch()
	{
		try
		{
			pl = null;
			grdRouting.DataSource = null;
			grdReceipt.DataSource = null;
			ProductionPlanningImpl productionPlanningImpl = new ProductionPlanningImpl();
			productionPlanningImpl.ItemCode = txtCardCode.Text;
			productionPlanningImpl.ItemName = txtCardName.Text;
			productionPlanningImpl.FromDate = Function.ParseDateTime(dtFromDate.EditValue);
			productionPlanningImpl.ToDate = Function.ParseDateTime(dtToDate.EditValue);
			DataTable dataSource = await AwaitSearch(productionPlanningImpl);
			grdMO.DataSource = dataSource;
			grdMO.RefreshDataSource();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gvMO_RowClick(object sender, RowClickEventArgs e)
	{
	}

	private void simpleButton2_Click(object sender, EventArgs e)
	{
		try
		{
			frm_MasterOrderReplan frm_MasterOrderReplan2 = new frm_MasterOrderReplan((List<ProductionPlanningImpl>)grdMO.DataSource);
			frm_MasterOrderReplan2.MdiParent = MdiParent;
			frm_MasterOrderReplan2.Show();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void repSupCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			DataTable dataTable = grdRouting.DataSource as DataTable;
			ButtonEdit buttonEdit = sender as ButtonEdit;
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.OperationByStatus, multiSelect: false, "");
			frm_ChooseFromList2.ShowDialog(this);
			if (frm_ChooseFromList2.ChooseItem != null)
			{
				OperationImpl operationImpl = (OperationImpl)frm_ChooseFromList2.ChooseItem;
				DataRow dataRow = dataTable.Rows[gvRouting.GetDataSourceRowIndex(gvRouting.FocusedRowHandle)];
				dataRow["SubOpCode"] = operationImpl.Code;
				dataRow["OpName"] = operationImpl.Name;
				dataTable.AcceptChanges();
				buttonEdit.EditValue = operationImpl.Code;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Warning, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private void txtCardCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.Items, multiSelect: true, string.Empty);
			frm_ChooseFromList2.ShowDialog(this);
			ButtonEdit buttonEdit = sender as ButtonEdit;
			if (frm_ChooseFromList2.ChooseItems != null && frm_ChooseFromList2.ChooseItems.Count > 0)
			{
				ItemsImpl itemsImpl = (ItemsImpl)frm_ChooseFromList2.ChooseItems[0];
				txtCardCode.Text = itemsImpl.ItemCode;
				txtCardName.Text = itemsImpl.ItemName;
				buttonEdit.EditValue = itemsImpl.ItemCode;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<string> SetBatchComplete(string batchIssue)
	{
		try
		{
			string batCpt = "";
			DataTable dataTable = await APZ_ProductionPlanning_GetBatchCompleteByBatIssue();
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				if (Function.ToString(dataTable.Rows[i]["BatchIssue"]).Trim().ToUpper() == batchIssue.Trim().ToUpper())
				{
					batCpt = ((!string.IsNullOrEmpty(batCpt)) ? (batCpt + " ; " + dataTable.Rows[i]["BatchComplete"]) : (batCpt + dataTable.Rows[i]["BatchComplete"]));
				}
			}
			return batCpt;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return "";
		}
	}

	private async void xtraTabControl1_SelectedPageChanged(object sender, TabPageChangedEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			SelectTabDoc();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void repOpCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			DataTable dataTable = grdRouting.DataSource as DataTable;
			ButtonEdit buttonEdit = sender as ButtonEdit;
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.OperationByStatus, multiSelect: false, "");
			frm_ChooseFromList2.ShowDialog(this);
			if (frm_ChooseFromList2.ChooseItem != null)
			{
				OperationImpl operationImpl = (OperationImpl)frm_ChooseFromList2.ChooseItem;
				DataRow dataRow = dataTable.Rows[gvRouting.GetDataSourceRowIndex(gvRouting.FocusedRowHandle)];
				dataRow["OpCode"] = operationImpl.Code;
				dataRow["OpName"] = operationImpl.Name;
				dataTable.AcceptChanges();
				buttonEdit.EditValue = operationImpl.Code;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gvMO_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void gv_Planed_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void gvRouting_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void gvReceipt_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void repOcr1_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.OrcCode, multiSelect: false, "");
			frm_ChooseFromList2.ShowDialog(this);
			if (frm_ChooseFromList2.ChooseItem != null)
			{
				DataRow row = ((DataRowView)frm_ChooseFromList2.ChooseItem).Row;
				gvMO.SetRowCellValue(gvMO.FocusedRowHandle, "OcrCode", row[0]);
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void repOcr2_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.OrcCode2, multiSelect: false, "");
			frm_ChooseFromList2.ShowDialog(this);
			if (frm_ChooseFromList2.ChooseItem != null)
			{
				DataRow row = ((DataRowView)frm_ChooseFromList2.ChooseItem).Row;
				gvMO.SetRowCellValue(gvMO.FocusedRowHandle, "OcrCode2", row[0]);
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void repOcr3_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.OrcCode3, multiSelect: false, "");
			frm_ChooseFromList2.ShowDialog(this);
			if (frm_ChooseFromList2.ChooseItem != null)
			{
				DataRow row = ((DataRowView)frm_ChooseFromList2.ChooseItem).Row;
				gvMO.SetRowCellValue(gvMO.FocusedRowHandle, "OcrCode3", row[0]);
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void repOcr4_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.OrcCode4, multiSelect: false, "");
			frm_ChooseFromList2.ShowDialog(this);
			if (frm_ChooseFromList2.ChooseItem != null)
			{
				DataRow row = ((DataRowView)frm_ChooseFromList2.ChooseItem).Row;
				gvMO.SetRowCellValue(gvMO.FocusedRowHandle, "OcrCode4", row[0]);
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void repOcr5_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.OrcCode5, multiSelect: false, "");
			frm_ChooseFromList2.ShowDialog(this);
			if (frm_ChooseFromList2.ChooseItem != null)
			{
				DataRow row = ((DataRowView)frm_ChooseFromList2.ChooseItem).Row;
				gvMO.SetRowCellValue(gvMO.FocusedRowHandle, "OcrCode5", row[0]);
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void repItemCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.ItemsInventory, multiSelect: false, "");
			frm_ChooseFromList2.ShowDialog(this);
			if (frm_ChooseFromList2.ChooseItem != null)
			{
				ItemsImpl itemsImpl = (ItemsImpl)frm_ChooseFromList2.ChooseItem;
				gvMO.SetRowCellValue(gvMO.FocusedRowHandle, "ItemCode", itemsImpl.ItemCode);
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void repWorkCenter_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			DataTable dataTable = grdRouting.DataSource as DataTable;
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.Resource, multiSelect: true, string.Empty);
			frm_ChooseFromList2.ShowDialog(this);
			ButtonEdit buttonEdit = sender as ButtonEdit;
			if (frm_ChooseFromList2.ChooseItems != null && frm_ChooseFromList2.ChooseItems.Count > 0)
			{
				GetResCodeImpl getResCodeImpl = (GetResCodeImpl)frm_ChooseFromList2.ChooseItems[0];
				dataTable.Rows[gvRouting.GetDataSourceRowIndex(gvRouting.FocusedRowHandle)]["ResCode"] = getResCodeImpl.ResCode;
				dataTable.AcceptChanges();
				buttonEdit.EditValue = getResCodeImpl.ResCode;
				string value = await GetCtrlKeyByResCode(getResCodeImpl.ResCode);
				if (!string.IsNullOrEmpty(value))
				{
					gvRouting.SetRowCellValue(gvRouting.FocusedRowHandle, "KeyCode", value);
				}
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private bool IsAllowingEdit(GridViewEx view, int row)
	{
		try
		{
			return Convert.ToBoolean(view.GetRowCellValue(row, "BatchQuantity"));
		}
		catch
		{
			return false;
		}
	}

	private void re_ControlKey_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			DataTable dataTable = grdRouting.DataSource as DataTable;
			ButtonEdit buttonEdit = sender as ButtonEdit;
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.ControlKey, multiSelect: false, "");
			frm_ChooseFromList2.ShowDialog(this);
			if (frm_ChooseFromList2.ChooseItem != null)
			{
				ControlKeyImpl controlKeyImpl = (ControlKeyImpl)frm_ChooseFromList2.ChooseItem;
				dataTable.Rows[gvRouting.GetDataSourceRowIndex(gvRouting.FocusedRowHandle)]["KeyCode"] = controlKeyImpl.KeyCode;
				dataTable.AcceptChanges();
				buttonEdit.EditValue = controlKeyImpl.KeyCode;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Warning, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private void repPreOp_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.OperationByStatus, multiSelect: false, "");
			frm_ChooseFromList2.ShowDialog(this);
			if (frm_ChooseFromList2.ChooseItem != null)
			{
				OperationImpl operationImpl = (OperationImpl)frm_ChooseFromList2.ChooseItem;
				gvRouting.SetRowCellValue(gvRouting.FocusedRowHandle, "PrevOp", operationImpl.Code);
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void repNextOp_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.OperationByStatus, multiSelect: false, "");
			frm_ChooseFromList2.ShowDialog(this);
			if (frm_ChooseFromList2.ChooseItem != null)
			{
				OperationImpl operationImpl = (OperationImpl)frm_ChooseFromList2.ChooseItem;
				gvRouting.SetRowCellValue(gvRouting.FocusedRowHandle, "NextOp", operationImpl.Code);
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void reUOM_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
	}

	private void reitemcode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.ItemsInventory, multiSelect: false, "");
			frm_ChooseFromList2.ShowDialog(this);
			if (frm_ChooseFromList2.ChooseItem != null)
			{
				ItemsImpl itemsImpl = (ItemsImpl)frm_ChooseFromList2.ChooseItem;
				gvRouting.SetRowCellValue(gvRouting.FocusedRowHandle, "ItemCode", itemsImpl.ItemCode);
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void simpleButton2_Click_1(object sender, EventArgs e)
	{
		_ = 2;
		try
		{
			DataTable dataTable = grd_Planed.DataSource as DataTable;
			int[] selectedRows = gv_Planed.GetSelectedRows();
			if (selectedRows.Count() > 0)
			{
				List<DataRow> list = new List<DataRow>();
				for (int i = 0; i < selectedRows.Count(); i++)
				{
					DataRow item = dataTable.Rows[selectedRows[i]];
					list.Add(item);
				}
				DataTable dataTable2 = list.CopyToDataTable();
				List<ProductionOder_Impl_OWOR> list2 = new List<ProductionOder_Impl_OWOR>();
				for (int j = 0; j < dataTable2.Rows.Count; j++)
				{
					DataRow dataRow = dataTable2.Rows[j];
					if (Function.ToString(dataRow["OriginNum"]).Contains(','))
					{
						string[] array = Function.ToString(dataRow["OriginNum"]).Trim().Split(',');
						for (int k = 0; k < array.Length; k++)
						{
							ProductionOder_Impl_OWOR productionOder_Impl_OWOR = new ProductionOder_Impl_OWOR();
							productionOder_Impl_OWOR.ItemCode = Function.ToString(dataRow["ItemCode"]);
							productionOder_Impl_OWOR.DueDate = Function.ParseDateTimes(dataRow["DueDate"]);
							productionOder_Impl_OWOR.PostDate = productionOder_Impl_OWOR.DueDate;
							productionOder_Impl_OWOR.Warehouse = Function.ToString(dataRow["Warehouse"]);
							productionOder_Impl_OWOR.Status = "R";
							productionOder_Impl_OWOR.DocEntry = Function.ParseInt(array[k]);
							list2.Add(productionOder_Impl_OWOR);
						}
					}
					else
					{
						ProductionOder_Impl_OWOR productionOder_Impl_OWOR2 = new ProductionOder_Impl_OWOR();
						productionOder_Impl_OWOR2.ItemCode = Function.ToString(dataRow["ItemCode"]);
						productionOder_Impl_OWOR2.DueDate = Function.ParseDateTimes(dataRow["DueDate"]);
						productionOder_Impl_OWOR2.PostDate = productionOder_Impl_OWOR2.DueDate;
						productionOder_Impl_OWOR2.Warehouse = Function.ToString(dataRow["Warehouse"]);
						productionOder_Impl_OWOR2.Status = "R";
						productionOder_Impl_OWOR2.DocEntry = Function.ParseInt(dataRow["OriginNum"]);
						list2.Add(productionOder_Impl_OWOR2);
					}
				}
				HttpResult httpResult = await ProPlanning(list2);
				if (httpResult.MessageCode == Enums.MessageCode.Success)
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
					DataTable dtt = await GetRelease();
					if (!dtt.Columns.Contains("BatchComplete"))
					{
						dtt.Columns.Add("BatchComplete", typeof(string));
					}
					for (int l = 0; l < dtt.Rows.Count; l++)
					{
						string batchComplete = Function.ToString(dtt.Rows[l]["BatchIssue"]);
						string value = await SetBatchComplete(batchComplete);
						dtt.Rows[l]["BatchComplete"] = value;
					}
					grd_Planed.DataSource = dtt;
					grd_Planed.RefreshDataSource();
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				}
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.PleaseChooseRowBeforePressThisButton, "");
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void simpleButton3_Click(object sender, EventArgs e)
	{
		_ = 2;
		try
		{
			DataTable dataTable = grd_Release.DataSource as DataTable;
			int[] selectedRows = gv_Release.GetSelectedRows();
			if (selectedRows.Count() > 0)
			{
				List<DataRow> list = new List<DataRow>();
				for (int i = 0; i < selectedRows.Count(); i++)
				{
					DataRow item = dataTable.Rows[selectedRows[i]];
					list.Add(item);
				}
				DataTable dataTable2 = list.CopyToDataTable();
				List<ProductionOder_Impl_OWOR> list2 = new List<ProductionOder_Impl_OWOR>();
				for (int j = 0; j < dataTable2.Rows.Count; j++)
				{
					DataRow dataRow = dataTable2.Rows[j];
					if (Function.ToString(dataRow["OriginNum"]).Contains(','))
					{
						string[] array = Function.ToString(dataRow["OriginNum"]).Trim().Split(',');
						for (int k = 0; k < array.Length; k++)
						{
							ProductionOder_Impl_OWOR productionOder_Impl_OWOR = new ProductionOder_Impl_OWOR();
							productionOder_Impl_OWOR.ItemCode = Function.ToString(dataRow["ItemCode"]);
							productionOder_Impl_OWOR.DueDate = Function.ParseDateTimes(dataRow["DueDate"]);
							productionOder_Impl_OWOR.PostDate = productionOder_Impl_OWOR.DueDate;
							productionOder_Impl_OWOR.PlannedQty = Function.ParseDouble(dataRow["Quantity"]);
							productionOder_Impl_OWOR.Status = "C";
							productionOder_Impl_OWOR.DocEntry = Function.ParseInt(array[k]);
							productionOder_Impl_OWOR.Series = 31;
							list2.Add(productionOder_Impl_OWOR);
						}
					}
					else
					{
						ProductionOder_Impl_OWOR productionOder_Impl_OWOR2 = new ProductionOder_Impl_OWOR();
						productionOder_Impl_OWOR2.ItemCode = Function.ToString(dataRow["ItemCode"]);
						productionOder_Impl_OWOR2.DueDate = Function.ParseDateTimes(dataRow["DueDate"]);
						productionOder_Impl_OWOR2.PostDate = productionOder_Impl_OWOR2.DueDate;
						productionOder_Impl_OWOR2.PlannedQty = Function.ParseDouble(dataRow["Quantity"]);
						productionOder_Impl_OWOR2.Status = "C";
						productionOder_Impl_OWOR2.DocEntry = Function.ParseInt(dataRow["DocEntry"]);
						productionOder_Impl_OWOR2.Series = 31;
						list2.Add(productionOder_Impl_OWOR2);
					}
				}
				HttpResult httpResult = await ProPlanning(list2);
				if (httpResult.MessageCode == Enums.MessageCode.Success)
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
					DataTable dtt = await GetClosed();
					if (!dtt.Columns.Contains("BatchComplete"))
					{
						dtt.Columns.Add("BatchComplete", typeof(string));
					}
					for (int l = 0; l < dtt.Rows.Count; l++)
					{
						string batchComplete = Function.ToString(dtt.Rows[l]["BatchIssue"]);
						string value = await SetBatchComplete(batchComplete);
						dtt.Rows[l]["BatchComplete"] = value;
					}
					grd_Release.DataSource = dtt;
					grd_Release.RefreshDataSource();
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				}
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.PleaseChooseRowBeforePressThisButton, "");
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void tabRelease_Paint(object sender, PaintEventArgs e)
	{
	}

	private void gv_Release_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator && e.RowHandle >= 0)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (pl == null || pl.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.RemoveError, "");
			}
			else if (grdMO.DataSource is List<ProductionPlanningImpl> list && list.Count() > 0)
			{
				int monoRemove = Function.ParseInt(gvMO.GetRowCellValue(gvMO.FocusedRowHandle, "MONo"));
				ProductionPlanningImpl item = list.Where((ProductionPlanningImpl p) => p.MONo == monoRemove).ToList().FirstOrDefault();
				list.Remove(item);
				grdMO.DataSource = list;
				grdMO.RefreshDataSource();
				WinCommons.Success(Enums.MessageCode.RemoveSuccess, "");
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.RemoveRowErrorTableIsNull, "");
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btCancel_Click(object sender, EventArgs e)
	{
		try
		{
			if (save)
			{
				Close();
				if (beginForm)
				{
					PcClose();
				}
				save = false;
			}
			else
			{
				Close();
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gvMO_RowStyle(object sender, RowStyleEventArgs e)
	{
	}

	private async void gvRouting_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			for (int i = 0; i < gvRouting.RowCount; i++)
			{
				string text = Function.ToString(gvRouting.GetRowCellValue(gvRouting.FocusedRowHandle, "KeyCode"));
				if (!string.IsNullOrEmpty(text))
				{
					if (!(await AwaitCheckControlKey(text)))
					{
						if (gvRouting.FocusedColumn.FieldName == "BatchQuantity")
						{
							e.Cancel = true;
						}
						gvRouting.SetRowCellValue(gvRouting.FocusedRowHandle, "BatchQuantity", "");
					}
				}
				else if (gvRouting.FocusedColumn.FieldName == "BatchQuantity")
				{
					e.Cancel = true;
				}
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void panelControl1_Paint(object sender, PaintEventArgs e)
	{
	}

	private void xtraTabControl2_SelectedPageChanged(object sender, TabPageChangedEventArgs e)
	{
		try
		{
			SelectTabDocs();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gvReceipt_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
	{
		if (grdReceipt.DataSource == null || gvReceipt.RowCount <= 0)
		{
			return;
		}
		for (int i = 0; i < gvReceipt.RowCount; i++)
		{
			if (e.RowHandle != i || !(e.Column.FieldName == "BatchIssue"))
			{
				continue;
			}
			bool flag = false;
			for (int j = 0; j < gvReceipt.FocusedRowHandle; j++)
			{
				string text = Function.ToString(gvReceipt.GetRowCellValue(j, "BatchComplete"));
				for (int k = 0; k < gvReceipt.FocusedRowHandle; k++)
				{
					string text2 = Function.ToString(gvReceipt.GetRowCellValue(k, "BatchIssue"));
					if (text == text2)
					{
						flag = true;
					}
				}
				if (!flag)
				{
					if (!rep.Items.Contains(text))
					{
						rep.Items.Add(text);
					}
					rep.ReadOnly = false;
				}
			}
			e.RepositoryItem = rep;
		}
	}

	private void xtratabDoc_Click(object sender, EventArgs e)
	{
	}

	private void repGetBatch_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (gvReceipt.RowCount <= 0)
			{
				return;
			}
			string value = Function.ToString(gvReceipt.GetRowCellValue(gvReceipt.FocusedRowHandle, "BatchComplete"));
			for (int i = 0; i < gvRouting.RowCount; i++)
			{
				if (string.IsNullOrEmpty(Function.ToString(gvRouting.GetRowCellValue(i, "BatchIssue"))))
				{
					gvRouting.SetRowCellValue(i, "BatchIssue", value);
					return;
				}
			}
			DataTable dataTable = grdRouting.DataSource as DataTable;
			DataRow dataRow = dataTable.NewRow();
			int num = gvRouting.RowCount - 1;
			dataRow["MONo"] = dataTable.Rows[num]["MONo"];
			dataRow["UoM"] = "C";
			dataRow["BatchIssue"] = value;
			dataRow["ItemCode"] = gvMO.GetFocusedRowCellValue("ItemCode");
			dataTable.Rows.InsertAt(dataRow, num + 1);
			dataTable.AcceptChanges();
			grdRouting.DataSource = dataTable;
			grdRouting.RefreshDataSource();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void deleteRowToolStripMenuItem1_Click(object sender, EventArgs e)
	{
		try
		{
			if (!(grdRouting.DataSource is DataTable dataTable) || dataTable.Rows.Count <= 0)
			{
				return;
			}
			int rowHandle = gvRouting.GetSelectedRows()[0];
			DataRow dataRow = gvRouting.GetDataRow(rowHandle);
			int num = 0;
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				if (dataTable.Rows[i]["BatchIssue"] == dataRow["BatchIssue"])
				{
					num++;
				}
			}
			if (dataTable.Rows.Count == 1)
			{
				WinCommons.Error(Enums.MessageCode.DeleteErrorCannotDeleteAllRows, "");
				return;
			}
			dataTable.Rows.Remove(dataRow);
			dataTable.AcceptChanges();
			grdRouting.DataSource = dataTable;
			grdRouting.RefreshDataSource();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void insertRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			InsertRowRouting();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void InsertRowRouting()
	{
		try
		{
			DataTable dataTable = grdRouting.DataSource as DataTable;
			DataRow dataRow = dataTable.NewRow();
			int num = gvRouting.GetSelectedRows()[0];
			dataRow["MONo"] = dataTable.Rows[num]["MONo"];
			dataRow["UoM"] = "C";
			dataRow["BatchIssue"] = "";
			dataRow["ItemCode"] = gvMO.GetFocusedRowCellValue("ItemCode");
			dataTable.Rows.InsertAt(dataRow, num + 1);
			dataTable.AcceptChanges();
			grdRouting.DataSource = dataTable;
			grdRouting.RefreshDataSource();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void repRouting_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.Routing, multiSelect: true, string.Empty);
			frm_ChooseFromList2.ShowDialog(this);
			if (frm_ChooseFromList2.ChooseItems != null && frm_ChooseFromList2.ChooseItems.Count > 0)
			{
				GetListRoutingImpl getListRoutingImpl = (GetListRoutingImpl)frm_ChooseFromList2.ChooseItems[0];
				gvMO.SetRowCellValue(gvMO.FocusedRowHandle, "RouCode", getListRoutingImpl.RouCode);
				GetDoc();
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Plan_Click(object sender, EventArgs e)
	{
		try
		{
			DialogResult dialogResult = XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCreateProductionPlan, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo);
			if (DialogResult.Yes == dialogResult)
			{
				WinCommons._OpenProcessing();
				taoLenh = true;
				SaveFunction();
				WinCommons._CloseProcessing();
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void SettingLanguage()
	{
		Text = Apzon.Commons.Properties.Resources.Title_frmProductionPlanning;
		lb_CustomerCode.Utext = Apzon.Commons.Properties.Resources.lb_ItemNo;
		lb_CustomerName.Utext = Apzon.Commons.Properties.Resources.lb_ItemName;
		lb_FromDate.Utext = Apzon.Commons.Properties.Resources.Doc_lb_FromDate;
		lb_ToDate.Utext = Apzon.Commons.Properties.Resources.Doc_lb_ToDate;
		btSearch.Text = Apzon.Commons.Properties.Resources.ButtonSearch;
		btSave.Text = Apzon.Commons.Properties.Resources.ButtonSave;
		btCancel.Text = Apzon.Commons.Properties.Resources.ButtonCancel;
		btCheckCapacity.Text = Apzon.Commons.Properties.Resources.ButtonCheckCapacity;
		btnMOPlanning.Text = Apzon.Commons.Properties.Resources.ButtonMOPlanning;
		tabMO.Text = Apzon.Commons.Properties.Resources.tab_GeneralMO;
		clMONo.Caption = Apzon.Commons.Properties.Resources.lb_MasterOrderNo;
		clMoName.Caption = Apzon.Commons.Properties.Resources.lb_MasterOrderName;
		clPostDate.Caption = Apzon.Commons.Properties.Resources.Doc_gvDoc_gcPostingDate;
	}

	private void SelectTabDocs()
	{
		try
		{
			if (xtratabDoc.SelectedTabPage.Equals(tab_ReceiptFromProduction) && gvMO.RowCount > 0)
			{
				repSaleorderNo.Items.Clear();
				repSOLine.Items.Clear();
				string[] array = Function.ToString(gvMO.GetRowCellValue(gvMO.FocusedRowHandle, "DocEntrySO")).Trim().Split(',')
					.Distinct()
					.ToArray();
				for (int i = 0; i < array.Count(); i++)
				{
					repSaleorderNo.Items.Add(array[i]);
				}
				string[] array2 = Function.ToString(gvMO.GetRowCellValue(gvMO.FocusedRowHandle, "LineNumSO")).Trim().Split(',')
					.Distinct()
					.ToArray();
				for (int j = 0; j < array2.Count(); j++)
				{
					repSOLine.Items.Add(array2[j]);
				}
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<DataTable> APZ_ProductionPlanning_GetBatchCompleteByBatIssue()
	{
		try
		{
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
			return await _helper.GetDataTableAsync("APZ_ProductionPlanning_GetBatchCompleteByBatIssue");
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	private async void SaveFunction()
	{
		_ = 1;
		try
		{
			statusSaved = true;
			if (gvMO.RowCount > 0)
			{
				if (!string.IsNullOrEmpty(Function.ToString(gvMO.GetRowCellValue(gvMO.FocusedRowHandle, "MOName"))))
				{
					List<ProductionPlanningImpl> list = grdMO.DataSource as List<ProductionPlanningImpl>;
					DataTable dtPP = new DataTable();
					dtPP.Columns.Add("MONo", typeof(int));
					dtPP.Columns.Add("MOName", typeof(string));
					dtPP.Columns.Add("PostDate", typeof(DateTime));
					dtPP.Columns.Add("DueDate", typeof(DateTime));
					dtPP.Columns.Add("BatchToIssue", typeof(string));
					dtPP.Columns.Add("UoM", typeof(string));
					dtPP.Columns.Add("Quantity", typeof(double));
					dtPP.Columns.Add("Width", typeof(double));
					dtPP.Columns.Add("Length", typeof(double));
					dtPP.Columns.Add("Project", typeof(string));
					dtPP.Columns.Add("OcrCode", typeof(string));
					dtPP.Columns.Add("OcrCode2", typeof(string));
					dtPP.Columns.Add("OcrCode3", typeof(string));
					dtPP.Columns.Add("OcrCode4", typeof(string));
					dtPP.Columns.Add("OcrCode5", typeof(string));
					dtPP.Columns.Add("RouCode", typeof(string));
					dtPP.Columns.Add("Remark", typeof(string));
					dtPP.Columns.Add("Status", typeof(string));
					dtPP.Columns.Add("FixBatch", typeof(string));
					dtPP.Columns.Add("ItemCode", typeof(string));
					dtPP.Columns.Add("DocEntrySO", typeof(string));
					dtPP.Columns.Add("LineNumSo", typeof(string));
					dtPP.AcceptChanges();
					if (list == null || list.Count == 0)
					{
						DataTable dataTable = grdMO.DataSource as DataTable;
						for (int i = 0; i < dataTable.Rows.Count; i++)
						{
							DataRow dataRow = dtPP.NewRow();
							dataRow["MONo"] = dataTable.Rows[i]["MONo"];
							dataRow["MOName"] = dataTable.Rows[i]["MOName"];
							dataRow["PostDate"] = dataTable.Rows[i]["PostDate"];
							dataRow["DueDate"] = dataTable.Rows[i]["DueDate"];
							dataRow["BatchToIssue"] = dataTable.Rows[i]["BatchToIssue"];
							dataRow["UoM"] = dataTable.Rows[i]["UoM"];
							dataRow["Quantity"] = dataTable.Rows[i]["Quantity"];
							dataRow["Project"] = dataTable.Rows[i]["Project"];
							dataRow["OcrCode"] = ((dataTable.Rows[i]["OcrCode"] == null) ? "" : dataTable.Rows[i]["OcrCode"]);
							dataRow["OcrCode2"] = ((dataTable.Rows[i]["OcrCode2"] == null) ? "" : dataTable.Rows[i]["OcrCode2"]);
							dataRow["OcrCode3"] = ((dataTable.Rows[i]["OcrCode3"] == null) ? "" : dataTable.Rows[i]["OcrCode3"]);
							dataRow["OcrCode4"] = ((dataTable.Rows[i]["OcrCode4"] == null) ? "" : dataTable.Rows[i]["OcrCode4"]);
							dataRow["OcrCode5"] = ((dataTable.Rows[i]["OcrCode5"] == null) ? "" : dataTable.Rows[i]["OcrCode5"]);
							dataRow["RouCode"] = dataTable.Rows[i]["RouCode"];
							dataRow["Remark"] = dataTable.Rows[i]["Remark"];
							dataRow["Status"] = dataTable.Rows[i]["Status"];
							dataRow["FixBatch"] = dataTable.Rows[i]["BatchSOFixed"];
							dataRow["ItemCode"] = dataTable.Rows[i]["ItemCode"];
							dataRow["DocEntrySO"] = dataTable.Rows[i]["DocEntrySO"];
							dataRow["LineNumSo"] = dataTable.Rows[i]["LineNumSo"];
							dtPP.Rows.Add(dataRow);
							dtPP.AcceptChanges();
						}
					}
					else
					{
						for (int j = 0; j < list.Count; j++)
						{
							ProductionPlanningImpl productionPlanningImpl = list[j];
							DataRow dataRow2 = dtPP.NewRow();
							dataRow2["MONo"] = productionPlanningImpl.MONo;
							dataRow2["MOName"] = productionPlanningImpl.MOName;
							dataRow2["PostDate"] = productionPlanningImpl.PostDate;
							dataRow2["DueDate"] = productionPlanningImpl.DueDate;
							dataRow2["BatchToIssue"] = productionPlanningImpl.BatchToIssue;
							dataRow2["UoM"] = productionPlanningImpl.UoM;
							dataRow2["Quantity"] = productionPlanningImpl.Quantity;
							dataRow2["Width"] = productionPlanningImpl.Width;
							dataRow2["Length"] = productionPlanningImpl.Length;
							dataRow2["Project"] = productionPlanningImpl.Project;
							dataRow2["OcrCode"] = ((productionPlanningImpl.OcrCode == null) ? "" : productionPlanningImpl.OcrCode);
							dataRow2["OcrCode2"] = ((productionPlanningImpl.OcrCode2 == null) ? "" : productionPlanningImpl.OcrCode2);
							dataRow2["OcrCode3"] = ((productionPlanningImpl.OcrCode3 == null) ? "" : productionPlanningImpl.OcrCode3);
							dataRow2["OcrCode4"] = ((productionPlanningImpl.OcrCode4 == null) ? "" : productionPlanningImpl.OcrCode4);
							dataRow2["OcrCode5"] = ((productionPlanningImpl.OcrCode5 == null) ? "" : productionPlanningImpl.OcrCode5);
							dataRow2["RouCode"] = productionPlanningImpl.RouCode;
							dataRow2["Remark"] = productionPlanningImpl.Remark;
							dataRow2["Status"] = productionPlanningImpl.Status;
							dataRow2["FixBatch"] = productionPlanningImpl.BatchSOFixed;
							dataRow2["ItemCode"] = productionPlanningImpl.ItemCode;
							dataRow2["DocEntrySO"] = productionPlanningImpl.DocEntrySO;
							dataRow2["LineNumSo"] = productionPlanningImpl.LineNumSO;
							dtPP.Rows.Add(dataRow2);
							dtPP.AcceptChanges();
						}
					}
					_ = grdReceipt.DataSource;
					DataTable dtReceipt = new DataTable();
					dtReceipt.Columns.Add("MONo", typeof(int));
					dtReceipt.Columns.Add("LineNum", typeof(int));
					dtReceipt.Columns.Add("ItemCode", typeof(string));
					dtReceipt.Columns.Add("ItemName", typeof(string));
					dtReceipt.Columns.Add("SaleOrder", typeof(int));
					dtReceipt.Columns.Add("LineSale", typeof(int));
					dtReceipt.Columns.Add("DelDate", typeof(DateTime));
					dtReceipt.Columns.Add("DelTime", typeof(TimeSpan));
					dtReceipt.Columns.Add("BatchIssue", typeof(string));
					dtReceipt.Columns.Add("BatchComplete", typeof(string));
					dtReceipt.Columns.Add("ThickPlan", typeof(double));
					dtReceipt.Columns.Add("WidthPlan", typeof(double));
					dtReceipt.Columns.Add("LengthPlan", typeof(double));
					dtReceipt.Columns.Add("ThickFinal", typeof(double));
					dtReceipt.Columns.Add("WidthFinal", typeof(double));
					dtReceipt.Columns.Add("LengthFinal", typeof(double));
					dtReceipt.Columns.Add("OcrCode", typeof(string));
					dtReceipt.Columns.Add("OcrCode2", typeof(string));
					dtReceipt.Columns.Add("OcrCode3", typeof(string));
					dtReceipt.Columns.Add("OcrCode4", typeof(string));
					dtReceipt.Columns.Add("OcrCode5", typeof(string));
					dtReceipt.Columns.Add("UoM", typeof(string));
					dtReceipt.Columns.Add("OpCode", typeof(string));
					dtReceipt.Columns.Add("Comments", typeof(string));
					dtReceipt.Columns.Add("Project", typeof(string));
					dtReceipt.Columns.Add("Quantity", typeof(double));
					dtReceipt.Columns.Add("Whse", typeof(string));
					dtReceipt.AcceptChanges();
					if (grdReceipt.DataSource is List<ProductionPlanningImpl> { Count: >0 } list2)
					{
						for (int k = 0; k < list2.Count; k++)
						{
							ProductionPlanningImpl productionPlanningImpl2 = list2[k];
							DataRow dataRow3 = dtReceipt.NewRow();
							dataRow3["MONo"] = gvMO.GetRowCellValue(gvMO.FocusedRowHandle, "MONo");
							dataRow3["LineNum"] = k + 1;
							dataRow3["Quantity"] = productionPlanningImpl2.Quantity;
							dataRow3["ItemCode"] = ((productionPlanningImpl2.ItemCode == null) ? "" : productionPlanningImpl2.ItemCode);
							dataRow3["ItemName"] = ((productionPlanningImpl2.ItemName == null) ? "" : productionPlanningImpl2.ItemName);
							dataRow3["SaleOrder"] = productionPlanningImpl2.SaleOrder;
							dataRow3["LineSale"] = productionPlanningImpl2.LineSale;
							dataRow3["DelDate"] = productionPlanningImpl2.ShipDate;
							dataRow3["DelTime"] = TimeSpan.Zero;
							dataRow3["BatchIssue"] = ((productionPlanningImpl2.BatchToIssue == null) ? "" : productionPlanningImpl2.BatchToIssue);
							dataRow3["BatchComplete"] = ((productionPlanningImpl2.BatchComplete == null) ? "" : productionPlanningImpl2.BatchComplete);
							dataRow3["ThickPlan"] = productionPlanningImpl2.ThickPlan;
							dataRow3["WidthPlan"] = productionPlanningImpl2.WidthPlan;
							dataRow3["LengthPlan"] = productionPlanningImpl2.LengthPlan;
							dataRow3["ThickFinal"] = productionPlanningImpl2.ThickFinal;
							dataRow3["WidthFinal"] = productionPlanningImpl2.Width;
							dataRow3["LengthFinal"] = productionPlanningImpl2.Length;
							dataRow3["OcrCode"] = productionPlanningImpl2.OcrCode;
							dataRow3["OcrCode2"] = productionPlanningImpl2.OcrCode2;
							dataRow3["OcrCode3"] = productionPlanningImpl2.OcrCode3;
							dataRow3["OcrCode4"] = productionPlanningImpl2.OcrCode4;
							dataRow3["OcrCode5"] = productionPlanningImpl2.OcrCode5;
							dataRow3["UoM"] = productionPlanningImpl2.UoM;
							dataRow3["OpCode"] = productionPlanningImpl2.OpCode;
							dataRow3["Project"] = productionPlanningImpl2.Project;
							dataRow3["Comments"] = productionPlanningImpl2.Remark;
							dataRow3["Whse"] = productionPlanningImpl2.Whse;
							dtReceipt.Rows.Add(dataRow3);
							dtReceipt.AcceptChanges();
						}
					}
					else
					{
						for (int l = 0; l < gvReceipt.RowCount; l++)
						{
							DataRow dataRow4 = dtReceipt.NewRow();
							dataRow4["MONo"] = gvMO.GetRowCellValue(gvMO.FocusedRowHandle, "MONo");
							dataRow4["LineNum"] = gvReceipt.GetRowCellValue(l, "LineNum");
							dataRow4["Quantity"] = gvReceipt.GetRowCellValue(l, "Quantity");
							dataRow4["ItemCode"] = gvReceipt.GetRowCellValue(l, "ItemCode");
							dataRow4["ItemName"] = gvReceipt.GetRowCellValue(l, "ItemName");
							dataRow4["SaleOrder"] = gvReceipt.GetRowCellValue(l, "SaleOrder");
							dataRow4["LineSale"] = gvReceipt.GetRowCellValue(l, "LineSale");
							dataRow4["DelDate"] = ((gvReceipt.GetRowCellValue(l, "ShipDate") == null) ? ((object)DateTime.Today) : gvReceipt.GetRowCellValue(l, "ShipDate"));
							dataRow4["DelTime"] = ((gvReceipt.GetRowCellValue(l, "DelTime") == null) ? ((object)TimeSpan.Zero) : gvReceipt.GetRowCellValue(l, "DelTime"));
							dataRow4["BatchIssue"] = gvReceipt.GetRowCellValue(l, "BatchIssue");
							dataRow4["BatchComplete"] = gvReceipt.GetRowCellValue(l, "BatchComplete");
							dataRow4["ThickPlan"] = ((gvReceipt.GetRowCellValue(l, "ThickPlan") == null) ? ((object)0) : gvReceipt.GetRowCellValue(l, "ThickPlan"));
							dataRow4["WidthPlan"] = ((gvReceipt.GetRowCellValue(l, "WidthPlan") == null) ? ((object)0) : gvReceipt.GetRowCellValue(l, "WidthPlan"));
							dataRow4["LengthPlan"] = ((gvReceipt.GetRowCellValue(l, "LengthPlan") == null) ? ((object)0) : gvReceipt.GetRowCellValue(l, "LengthPlan"));
							dataRow4["ThickFinal"] = ((gvReceipt.GetRowCellValue(l, "ThickFinal") == null) ? ((object)0) : gvReceipt.GetRowCellValue(l, "ThickFinal"));
							dataRow4["WidthFinal"] = ((gvReceipt.GetRowCellValue(l, "Width") == null) ? ((object)0) : gvReceipt.GetRowCellValue(l, "Width"));
							dataRow4["LengthFinal"] = ((gvReceipt.GetRowCellValue(l, "Length") == null) ? ((object)0) : gvReceipt.GetRowCellValue(l, "Length"));
							dataRow4["OcrCode"] = ((gvReceipt.GetRowCellValue(l, "OcrCode") == null) ? "" : gvReceipt.GetRowCellValue(l, "OcrCode"));
							dataRow4["OcrCode2"] = ((gvReceipt.GetRowCellValue(l, "OcrCode2") == null) ? "" : gvReceipt.GetRowCellValue(l, "OcrCode2"));
							dataRow4["OcrCode3"] = ((gvReceipt.GetRowCellValue(l, "OcrCode3") == null) ? "" : gvReceipt.GetRowCellValue(l, "OcrCode3"));
							dataRow4["OcrCode4"] = ((gvReceipt.GetRowCellValue(l, "OcrCode4") == null) ? "" : gvReceipt.GetRowCellValue(l, "OcrCode4"));
							dataRow4["OcrCode5"] = ((gvReceipt.GetRowCellValue(l, "OcrCode5") == null) ? "" : gvReceipt.GetRowCellValue(l, "OcrCode5"));
							dataRow4["UoM"] = gvReceipt.GetRowCellValue(l, "UoM");
							dataRow4["OpCode"] = gvReceipt.GetRowCellValue(l, "OpCode");
							dataRow4["Project"] = ((gvReceipt.GetRowCellValue(l, "Project") == null) ? "" : gvReceipt.GetRowCellValue(l, "Project"));
							dataRow4["Comments"] = ((gvReceipt.GetRowCellValue(l, "Comments") == null) ? "" : gvReceipt.GetRowCellValue(l, "Comments"));
							dataRow4["Whse"] = ((gvReceipt.GetRowCellValue(l, "Whse") == null) ? "" : gvReceipt.GetRowCellValue(l, "Whse"));
							dtReceipt.Rows.Add(dataRow4);
							dtReceipt.AcceptChanges();
						}
					}
					DataTable dtRoutingSource = grdRouting.DataSource as DataTable;
					DataTable dtRouting = new DataTable();
					DataTable dataTable2 = await AwaitGetTableSQL();
					if (dataTable2 != null && dataTable2.Rows.Count > 0)
					{
						for (int m = 0; m < dataTable2.Count(); m++)
						{
							Type typeFromSql = WinFuntions.GetTypeFromSql(Function.ToString(dataTable2.Rows[m]["DATA_TYPE"]));
							dtRouting.Columns.Add(Function.ToString(dataTable2.Rows[m]["COLUMN_NAME"]), typeFromSql);
						}
					}
					dtRouting.AcceptChanges();
					for (int n = 0; n < dtRoutingSource.Count(); n++)
					{
						DataRow dataRow5 = dtRouting.NewRow();
						dataRow5["PrevOpCode"] = dtRoutingSource.Rows[n]["PrevOp"];
						dataRow5["NextOpCode"] = dtRoutingSource.Rows[n]["NextOp"];
						dataRow5["ControlKey"] = dtRoutingSource.Rows[n]["KeyCode"];
						dataRow5["Description"] = dtRoutingSource.Rows[n]["OpName"];
						dataRow5["Remark"] = dtRoutingSource.Rows[n]["Comments"];
						for (int num = 0; num < dataTable2.Count(); num++)
						{
							dataRow5["BaseQty"] = dtRoutingSource.Rows[n]["Quantity"];
							string text = Function.ToString(dataTable2.Rows[num]["COLUMN_NAME"]);
							if (dtRoutingSource.Columns.Contains(text))
							{
								dataRow5[text] = ((dtRoutingSource.Rows[n][text] == null) ? "" : dtRoutingSource.Rows[n][text]);
							}
							else if (text == "MONo")
							{
								dataRow5[text] = gvMO.GetRowCellValue(gvMO.FocusedRowHandle, "MONo");
							}
						}
						dtRouting.Rows.Add(dataRow5);
						dtRouting.AcceptChanges();
					}
					DataSet dataSet = new DataSet();
					if (dtRouting != null && dtRouting.Rows.Count > 0)
					{
						for (int num2 = 0; num2 < dtRouting.Rows.Count; num2++)
						{
							dtRouting.Rows[num2]["LineNum"] = num2;
							dtRouting.AcceptChanges();
						}
					}
					dataSet.Tables.AddRange(new DataTable[3] { dtPP, dtRouting, dtReceipt });
					if (await AwaitSave(dataSet) == "")
					{
						if (taoLenh)
						{
							if (CheckValidateToPlanPro)
							{
								TaoLenh();
								taoLenh = false;
							}
							else
							{
								WinCommons.Error(Enums.MessageCode.CreateProductionPlanError, "");
							}
						}
						else
						{
							FunctionSearch();
							WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
						}
						LoadDocs();
						save = true;
					}
					else
					{
						WinCommons.Error(Enums.MessageCode.SaveError, "");
					}
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.MasterOrderNameCannotBeEmpty, "");
				}
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.TableIsEmptyCheckAgainInSaleOrder, "");
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<DataTable> AwaitGetTableSQL()
	{
		try
		{
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
			return await _helper.GetDataTableAsync("GetTableSQL");
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	private async Task<string> AwaitSave(DataSet PP)
	{
		try
		{
			using HttpClientHelper<ProductionPlanningImpl> _helper = new HttpClientHelper<ProductionPlanningImpl>(Configuration.ApiServer, FormController);
			return await _helper.PostParamsAsJsonAsync("Save", PP);
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return "Error";
		}
	}

	private void LoadMOPlanningByCustomer()
	{
		try
		{
			if (Configuration.MoPlanning != null && Configuration.MoPlanning.Equals("1"))
			{
				btnMOPlanning.Show();
			}
			else
			{
				btnMOPlanning.Hide();
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void LoadInIt()
	{
		try
		{
			List<Common.Table_ID> list = new List<Common.Table_ID>();
			list.Add(new Common.Table_ID
			{
				Name = "Plan",
				ID = "P"
			});
			list.Add(new Common.Table_ID
			{
				Name = "Release",
				ID = "R"
			});
			list.Add(new Common.Table_ID
			{
				Name = "Cancel",
				ID = "C"
			});
			rep_Status.DataSource = list;
			rep_Status.ValueMember = "ID";
			rep_Status.DisplayMember = "Name";
			List<Common.Table_ID> list2 = new List<Common.Table_ID>();
			list2.Add(new Common.Table_ID
			{
				Name = "Cuộn",
				ID = "C"
			});
			list2.Add(new Common.Table_ID
			{
				Name = "Tờ",
				ID = "T"
			});
			repUOMRouting.DataSource = list2;
			repUOMReceipt.DataSource = list2;
			repUOM.DataSource = list2;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void GetDoc()
	{
		try
		{
			GetDocRouting();
			GetDocReceipt();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void GetDocRouting()
	{
		_ = 2;
		try
		{
			grdRouting.DataSource = null;
			string rouCode = Function.ToString(gvMO.GetRowCellValue(gvMO.FocusedRowHandle, "RouCode"));
			new DataTable();
			DataTable dataTable;
			if (pl == null || pl.Count == 0)
			{
				int moNo = Function.ParseInt(gvMO.GetRowCellValue(gvMO.FocusedRowHandle, "MONo"));
				dataTable = await AwaitGetDocRouting_PPL1(moNo);
				if (dataTable == null || dataTable.Rows.Count == 0)
				{
					dataTable = await AwaitGetDocRouting(rouCode);
					int count = dataTable.Rows.Count;
					char c = ',';
					string text = Function.ToString(gvMO.GetRowCellValue(gvMO.FocusedRowHandle, "BatchToIssue"));
					if (!string.IsNullOrEmpty(text))
					{
						string[] array = text.Trim().Split(c);
						if (dataTable != null && dataTable.Rows.Count > 0)
						{
							if (!dataTable.Columns.Contains("ItemCode"))
							{
								dataTable.Columns.Add("ItemCode", typeof(string));
								for (int i = 0; i < dataTable.Rows.Count; i++)
								{
									dataTable.Rows[i]["ItemCode"] = gvMO.GetRowCellValue(gvMO.FocusedRowHandle, "ItemCode");
								}
							}
							if (!dataTable.Columns.Contains("BatchQuantity"))
							{
								dataTable.Columns.Add("BatchQuantity", typeof(int));
								for (int j = 0; j < dataTable.Rows.Count; j++)
								{
									dataTable.Rows[j]["BatchQuantity"] = 0;
								}
							}
							if (!dataTable.Columns.Contains("MONo"))
							{
								dataTable.Columns.Add("MONo", typeof(int));
								for (int k = 0; k < dataTable.Rows.Count; k++)
								{
									dataTable.Rows[k]["MONo"] = gvMO.GetRowCellValue(gvMO.FocusedRowHandle, "MONo");
								}
							}
							if (!dataTable.Columns.Contains("BatchIssue"))
							{
								dataTable.Columns.Add("BatchIssue", typeof(string));
								dataTable.Rows[0]["BatchIssue"] = array[0];
								if (array.Length > 1)
								{
									for (int l = 0; l < array.Length - 1; l++)
									{
										for (int m = 0; m < count; m++)
										{
											DataRow dataRow = dataTable.NewRow();
											for (int n = 0; n < dataTable.Columns.Count; n++)
											{
												if (dataTable.Columns[n].ColumnName != "BatchIssue")
												{
													dataRow[n] = dataTable.Rows[m][n];
												}
												else if (m % count == 0)
												{
													dataRow[n] = array[l + 1];
												}
												else
												{
													dataRow[n] = "";
												}
											}
											dataTable.Rows.Add(dataRow);
											dataTable.AcceptChanges();
										}
									}
								}
							}
							if (!dataTable.Columns.Contains("Whse"))
							{
								dataTable.Columns.Add("Whse", typeof(string));
								for (int num = 0; num < dataTable.Rows.Count; num++)
								{
									dataTable.Rows[num]["Whse"] = "";
								}
							}
							dataTable.AcceptChanges();
							if (pl != null && pl.Count > 0 && array.Length > 1)
							{
								for (int num2 = 0; num2 < array.Length - 1; num2++)
								{
									for (int num3 = 0; num3 < count; num3++)
									{
										DataRow dataRow2 = dataTable.NewRow();
										for (int num4 = 0; num4 < dataTable.Columns.Count; num4++)
										{
											if (dataTable.Columns[num4].ColumnName != "BatchIssue")
											{
												dataRow2[num4] = dataTable.Rows[num3][num4];
											}
											else
											{
												dataRow2[num4] = array[num2 + 1];
											}
										}
										dataTable.Rows.Add(dataRow2);
										dataTable.AcceptChanges();
									}
								}
							}
						}
					}
					if (dataTable.Rows.Count > 0)
					{
						for (int num5 = 0; num5 < dataTable.Rows.Count; num5++)
						{
							dataTable.Rows[num5]["LineNum"] = num5 + 1;
						}
					}
				}
			}
			else
			{
				dataTable = await AwaitGetDocRouting(rouCode);
				int count2 = dataTable.Rows.Count;
				char c2 = ',';
				string text2 = Function.ToString(gvMO.GetRowCellValue(gvMO.FocusedRowHandle, "BatchToIssue"));
				if (!string.IsNullOrEmpty(text2))
				{
					string[] array2 = text2.Trim().Split(c2);
					if (dataTable != null && dataTable.Rows.Count > 0)
					{
						if (!dataTable.Columns.Contains("ItemCode"))
						{
							dataTable.Columns.Add("ItemCode", typeof(string));
							for (int num6 = 0; num6 < dataTable.Rows.Count; num6++)
							{
								dataTable.Rows[num6]["ItemCode"] = gvMO.GetRowCellValue(gvMO.FocusedRowHandle, "ItemCode");
							}
						}
						if (!dataTable.Columns.Contains("BatchQuantity"))
						{
							dataTable.Columns.Add("BatchQuantity", typeof(int));
							for (int num7 = 0; num7 < dataTable.Rows.Count; num7++)
							{
								dataTable.Rows[num7]["BatchQuantity"] = 0;
							}
						}
						if (!dataTable.Columns.Contains("MONo"))
						{
							dataTable.Columns.Add("MONo", typeof(int));
							for (int num8 = 0; num8 < dataTable.Rows.Count; num8++)
							{
								dataTable.Rows[num8]["MONo"] = gvMO.GetRowCellValue(gvMO.FocusedRowHandle, "MONo");
							}
						}
						if (!dataTable.Columns.Contains("BatchIssue"))
						{
							dataTable.Columns.Add("BatchIssue", typeof(string));
							dataTable.Rows[0]["BatchIssue"] = array2[0];
						}
						if (!dataTable.Columns.Contains("Whse"))
						{
							dataTable.Columns.Add("Whse", typeof(string));
							for (int num9 = 0; num9 < dataTable.Rows.Count; num9++)
							{
								dataTable.Rows[num9]["Whse"] = "";
							}
						}
						dataTable.AcceptChanges();
						if (pl != null && pl.Count > 0 && array2.Length > 1)
						{
							for (int num10 = 0; num10 < array2.Length - 1; num10++)
							{
								for (int num11 = 0; num11 < count2; num11++)
								{
									DataRow dataRow3 = dataTable.NewRow();
									for (int num12 = 0; num12 < dataTable.Columns.Count; num12++)
									{
										if (dataTable.Columns[num12].ColumnName != "BatchIssue")
										{
											dataRow3[num12] = dataTable.Rows[num11][num12];
										}
										else if (num11 % count2 == 0)
										{
											dataRow3[num12] = array2[num10 + 1];
										}
									}
									dataTable.Rows.Add(dataRow3);
									dataTable.AcceptChanges();
								}
							}
						}
					}
				}
				if (dataTable.Rows.Count > 0)
				{
					for (int num13 = 0; num13 < dataTable.Rows.Count; num13++)
					{
						dataTable.Rows[num13]["LineNum"] = num13 + 1;
					}
				}
			}
			grdRouting.DataSource = dataTable;
			grdRouting.RefreshDataSource();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ReceiptFromBatchQuantity()
	{
		try
		{
			List<ProductionPlanningImpl> list = grdReceipt.DataSource as List<ProductionPlanningImpl>;
			_ = grdRouting.DataSource;
			int num = 1;
			string text = Function.ToString(gvRouting.GetRowCellValue(gvRouting.FocusedRowHandle, "BatchIssue"));
			if (list != null && list.Count > 0)
			{
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i].BatchToIssue == text)
					{
						list.RemoveAt(i);
						i--;
					}
				}
			}
			int num2 = Function.ParseInt(gvRouting.GetRowCellValue(gvRouting.FocusedRowHandle, "BatchQuantity"));
			for (int j = 0; j < num2; j++)
			{
				ProductionPlanningImpl productionPlanningImpl = new ProductionPlanningImpl();
				productionPlanningImpl.MONo = Function.ParseInt(gvMO.GetRowCellValue(gvMO.FocusedRowHandle, "MONo"));
				productionPlanningImpl.LineNum = num;
				productionPlanningImpl.SaleOrder = 0;
				productionPlanningImpl.LineSale = 0;
				productionPlanningImpl.UoM = "C";
				productionPlanningImpl.DelDate = Function.ParseDateTimes(gvMO.GetRowCellValue(gvMO.FocusedRowHandle, "DueDate"));
				productionPlanningImpl.DelTime = TimeSpan.Zero;
				productionPlanningImpl.ThickPlan = 0.0;
				productionPlanningImpl.WidthPlan = 0.0;
				productionPlanningImpl.LengthPlan = 0.0;
				productionPlanningImpl.ThickFinal = 0.0;
				productionPlanningImpl.WidthFinal = 0.0;
				productionPlanningImpl.LengthFinal = 0.0;
				productionPlanningImpl.OcrCode = Function.ToString(gvMO.GetRowCellValue(gvMO.FocusedRowHandle, "OcrCode"));
				productionPlanningImpl.OcrCode2 = Function.ToString(gvMO.GetRowCellValue(gvMO.FocusedRowHandle, "OcrCode2"));
				productionPlanningImpl.OcrCode3 = Function.ToString(gvMO.GetRowCellValue(gvMO.FocusedRowHandle, "OcrCode3"));
				productionPlanningImpl.OcrCode4 = Function.ToString(gvMO.GetRowCellValue(gvMO.FocusedRowHandle, "OcrCode4"));
				productionPlanningImpl.OcrCode5 = Function.ToString(gvMO.GetRowCellValue(gvMO.FocusedRowHandle, "OcrCode5"));
				productionPlanningImpl.MOName = Function.ToString(gvMO.GetRowCellValue(gvMO.FocusedRowHandle, "MOName"));
				productionPlanningImpl.RouCode = Function.ToString(gvMO.GetRowCellValue(gvMO.FocusedRowHandle, "MOName"));
				productionPlanningImpl.PostDate = DateTime.Today;
				productionPlanningImpl.DueDate = Function.ParseDateTimes(gvMO.GetRowCellValue(gvMO.FocusedRowHandle, "DueDate"));
				productionPlanningImpl.BatchComplete = text + "/0" + (j + 1);
				productionPlanningImpl.BatchToIssue = text;
				productionPlanningImpl.ItemCode = Function.ToString(gvMO.GetRowCellValue(gvMO.FocusedRowHandle, "ItemCode"));
				productionPlanningImpl.ItemName = Function.ToString(gvMO.GetRowCellValue(gvMO.FocusedRowHandle, "ItemName"));
				productionPlanningImpl.Quantity = 1.0;
				productionPlanningImpl.Width = 0.0;
				productionPlanningImpl.Length = 0.0;
				productionPlanningImpl.Project = Function.ToString(gvMO.GetRowCellValue(gvMO.FocusedRowHandle, "Project"));
				productionPlanningImpl.Remark = "";
				productionPlanningImpl.ShipDate = Function.ParseDateTimes(gvMO.GetRowCellValue(gvMO.FocusedRowHandle, "PostDate"));
				productionPlanningImpl.Whse = "";
				productionPlanningImpl.OpCode = ((!string.IsNullOrEmpty(Function.ToString(gvRouting.GetRowCellValue(gvRouting.FocusedRowHandle, "OpCode")))) ? Function.ToString(Function.ToString(gvRouting.GetRowCellValue(gvRouting.FocusedRowHandle, "OpCode"))) : Function.ToString(Function.ToString(gvRouting.GetRowCellValue(gvRouting.FocusedRowHandle, "SubOpCode"))));
				productionPlanningImpl.OrderLine = 0;
				productionPlanningImpl.LineNumSO = "";
				if (list == null)
				{
					list = new List<ProductionPlanningImpl>();
				}
				list.Add(productionPlanningImpl);
				num++;
				changeReceipt = false;
			}
			grdReceipt.DataSource = list;
			grdReceipt.RefreshDataSource();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void GetReceiptFromDataBase()
	{
		try
		{
			int moNo = Function.ParseInt(gvMO.GetRowCellValue(gvMO.FocusedRowHandle, "MONo"));
			DataTable dataSource = await AwaitGetReceipt(moNo);
			grdReceipt.DataSource = dataSource;
			grdReceipt.RefreshDataSource();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<DataTable> GetListToPlanPro(int MONo)
	{
		try
		{
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
			return await _helper.PostAsync("APZ_ProductionAddon_ProductionPlanning_GetFromAPZ_PPL1ByMoNoToCreatPlan", MONo);
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	private async void TaoLenh()
	{
		_ = 1;
		try
		{
			int mONo = Function.ParseInt(gvMO.GetRowCellValue(gvMO.FocusedRowHandle, "MONo"));
			DataTable dataTable = await GetListToPlanPro(mONo);
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				string.IsNullOrEmpty(Function.ToString(dataTable.Rows[i]["Whse"]));
			}
			List<ProductionOder_Impl_OWOR> list = new List<ProductionOder_Impl_OWOR>();
			for (int j = 0; j < dataTable.Rows.Count; j++)
			{
				DataRow dataRow = dataTable.Rows[j];
				ProductionOder_Impl_OWOR productionOder_Impl_OWOR = new ProductionOder_Impl_OWOR();
				productionOder_Impl_OWOR.MONo = Function.ParseInt(dataRow["MONo"]);
				productionOder_Impl_OWOR.BatchIssue = Function.ToString(dataRow["BatchIssue"]);
				productionOder_Impl_OWOR.OriginNum = Function.ParseInt(Function.ToString(dataRow["OriginNum"]).Split(',')[0]);
				productionOder_Impl_OWOR.ItemCode = Function.ToString(dataRow["ItemCode"]);
				productionOder_Impl_OWOR.DueDate = Function.ParseDateTimes(dataRow["DueDate"]);
				productionOder_Impl_OWOR.PostDate = productionOder_Impl_OWOR.DueDate;
				productionOder_Impl_OWOR.PlannedQty = Function.ParseDouble(dataRow["Quantity"]);
				productionOder_Impl_OWOR.Warehouse = Function.ToString(dataRow["Whse"]);
				productionOder_Impl_OWOR.Status = "P";
				productionOder_Impl_OWOR.OriginAbs = Function.ParseInt(dataRow["OriginNum"]);
				productionOder_Impl_OWOR.Line_WOR1 = new List<ProductionOder_LineImpl_WOR1>();
				ProductionOder_LineImpl_WOR1 productionOder_LineImpl_WOR = new ProductionOder_LineImpl_WOR1();
				productionOder_LineImpl_WOR.DocEntry = Function.ParseInt(gvMO.GetRowCellValue(gvMO.FocusedRowHandle, "DocEntrySO"));
				productionOder_LineImpl_WOR.ItemType = "290";
				productionOder_LineImpl_WOR.UomCode = Function.ToString(dataRow["UoM"]);
				productionOder_Impl_OWOR.Line_WOR1.Add(productionOder_LineImpl_WOR);
				productionOder_Impl_OWOR.UserFields = new List<UserFieldsImpl>();
				UserFieldsImpl userFieldsImpl = new UserFieldsImpl();
				userFieldsImpl.Name = "U_MONo";
				userFieldsImpl.Value = Function.ToString(dataRow["MONo"]);
				userFieldsImpl.Type = BoFieldTypes.db_Alpha;
				productionOder_Impl_OWOR.UserFields.Add(userFieldsImpl);
				UserFieldsImpl userFieldsImpl2 = new UserFieldsImpl();
				userFieldsImpl2.Name = "U_IssBatchC";
				userFieldsImpl2.Value = Function.ToString(dataRow["BatchIssue"]);
				userFieldsImpl2.Type = BoFieldTypes.db_Alpha;
				productionOder_Impl_OWOR.UserFields.Add(userFieldsImpl2);
				UserFieldsImpl userFieldsImpl3 = new UserFieldsImpl();
				userFieldsImpl3.Name = "U_SOLine";
				userFieldsImpl3.Value = Function.ParseInt(Function.ToString(dataRow["LineNumSo"]).Split(',')[0]);
				userFieldsImpl3.Type = BoFieldTypes.db_Numeric;
				productionOder_Impl_OWOR.UserFields.Add(userFieldsImpl3);
				UserFieldsImpl userFieldsImpl4 = new UserFieldsImpl();
				userFieldsImpl3.Name = "U_ReBatch";
				userFieldsImpl4.Value = Function.ParseInt(Function.ToString(dataRow["BatchIssue"]).Split(',')[0]);
				userFieldsImpl4.Type = BoFieldTypes.db_Numeric;
				productionOder_Impl_OWOR.UserFields.Add(userFieldsImpl4);
				list.Add(productionOder_Impl_OWOR);
			}
			HttpResult httpResult = await ProPlanning(list);
			if (httpResult.MessageCode == Enums.MessageCode.Success)
			{
				WinCommons.Success(Enums.MessageCode.CreateSuccess, "");
				FunctionSearch();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void GetDocReceipt()
	{
		try
		{
			grdReceipt.DataSource = null;
			Function.ToString(gvMO.GetRowCellValue(gvMO.FocusedRowHandle, "RouCode"));
			int moNo = Function.ParseInt(gvMO.GetRowCellValue(gvMO.FocusedRowHandle, "MONo"));
			new ProductionPlanningImpl();
			if (pl == null || pl.Count == 0)
			{
				GetReceiptFromDataBase();
				return;
			}
			ProductionPlanningImpl productionPlanningImpl = pl.FindLast((ProductionPlanningImpl p) => p.MONo == moNo);
			if (productionPlanningImpl == null)
			{
				return;
			}
			int num = 1;
			List<ProductionPlanningImpl> list = new List<ProductionPlanningImpl>();
			if (!(grdRouting.DataSource is DataTable dataTable) || dataTable.Rows.Count <= 0)
			{
				return;
			}
			for (int num2 = 0; num2 < dataTable.Count(); num2++)
			{
				string text = Function.ToString(dataTable.Rows[num2]["BatchIssue"]);
				int num3 = Function.ParseInt(dataTable.Rows[num2]["BatchQuantity"]);
				if (dataTable.Rows[num2]["BatchQuantity"] == null || Function.ParseInt(dataTable.Rows[num2]["BatchQuantity"]) <= 0)
				{
					continue;
				}
				for (int num4 = 0; num4 < num3; num4++)
				{
					ProductionPlanningImpl productionPlanningImpl2 = new ProductionPlanningImpl();
					productionPlanningImpl2.MONo = productionPlanningImpl.MONo;
					productionPlanningImpl2.LineNum = num;
					productionPlanningImpl2.SaleOrder = 0;
					productionPlanningImpl2.LineSale = 0;
					productionPlanningImpl2.DelDate = productionPlanningImpl.DelDate;
					productionPlanningImpl2.DelTime = productionPlanningImpl.DelTime;
					productionPlanningImpl2.ThickPlan = 0.0;
					productionPlanningImpl2.WidthPlan = 0.0;
					productionPlanningImpl2.LengthPlan = 0.0;
					productionPlanningImpl2.ThickFinal = 0.0;
					productionPlanningImpl2.WidthFinal = 0.0;
					productionPlanningImpl2.LengthFinal = 0.0;
					productionPlanningImpl2.OcrCode = productionPlanningImpl.OcrCode;
					productionPlanningImpl2.OcrCode2 = productionPlanningImpl.OcrCode2;
					productionPlanningImpl2.OcrCode3 = productionPlanningImpl.OcrCode3;
					productionPlanningImpl2.OcrCode4 = productionPlanningImpl.OcrCode4;
					productionPlanningImpl2.OcrCode5 = productionPlanningImpl.OcrCode5;
					productionPlanningImpl2.MOName = productionPlanningImpl.MOName;
					productionPlanningImpl2.RouCode = productionPlanningImpl.RouCode;
					productionPlanningImpl2.PostDate = productionPlanningImpl.PostDate;
					productionPlanningImpl2.DueDate = productionPlanningImpl.DueDate;
					if ((num2 == 0 && num4 < num3) || (Function.ToString(dataTable.Rows[num2]["BatchIssue"]) != Function.ToString(dataTable.Rows[num2 - 1]["BatchIssue"]) && num4 < num3))
					{
						productionPlanningImpl2.BatchComplete = text + "/0" + (num4 + 1);
						productionPlanningImpl2.BatchToIssue = text;
					}
					else
					{
						productionPlanningImpl2.BatchComplete = "";
						productionPlanningImpl2.BatchToIssue = "";
					}
					productionPlanningImpl2.ItemCode = productionPlanningImpl.ItemCode;
					productionPlanningImpl2.ItemName = productionPlanningImpl.ItemName;
					productionPlanningImpl2.UoM = productionPlanningImpl.UoM;
					productionPlanningImpl2.Quantity = 0.0;
					productionPlanningImpl2.Width = 0.0;
					productionPlanningImpl2.Length = 0.0;
					productionPlanningImpl2.Project = productionPlanningImpl.Project;
					productionPlanningImpl2.BatchSOFixed = productionPlanningImpl2.BatchSOFixed;
					productionPlanningImpl2.Status = productionPlanningImpl.Status;
					productionPlanningImpl2.Remark = productionPlanningImpl.Remark;
					productionPlanningImpl2.SONo = productionPlanningImpl.SONo;
					productionPlanningImpl2.ShipDate = productionPlanningImpl.ShipDate;
					productionPlanningImpl2.Whse = productionPlanningImpl.Whse;
					productionPlanningImpl2.Operation = productionPlanningImpl.Operation;
					productionPlanningImpl2.OpCode = productionPlanningImpl.OpCode;
					productionPlanningImpl2.OrderLine = 0;
					productionPlanningImpl2.LineNumSO = "";
					list.Add(productionPlanningImpl2);
					num++;
				}
			}
			grdReceipt.DataSource = list;
			grdReceipt.RefreshDataSource();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<string> GetCtrlKeyByResCode(string resCode)
	{
		try
		{
			using HttpClientHelper<string> _helper = new HttpClientHelper<string>(Configuration.ApiServer, FormController);
			return await _helper.PostAsync("GetCtrlKeyByResCode", resCode);
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return "Error";
		}
	}

	private async Task<DataTable> AwaitGetDocRouting(string rouCode)
	{
		try
		{
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Routing");
			new DataTable();
			DataTable dataTable = await _helper.GetDataTableAsync("GetDoc", new
			{
				roucode = rouCode
			});
			if (dataTable == null)
			{
				return new DataTable();
			}
			return dataTable;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	private async Task<DataTable> AwaitGetDocRouting_PPL1(int MoNo)
	{
		try
		{
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
			new DataTable();
			DataTable dataTable = await _helper.PostAsync("GetDocRecpt", MoNo);
			if (dataTable == null)
			{
				return new DataTable();
			}
			return dataTable;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	private async Task<DataTable> AwaitSearch(ProductionPlanningImpl obj)
	{
		try
		{
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
			return await _helper.PostAsync("Search", obj);
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	private async Task<DataTable> AwaitGetReceipt(int moNo)
	{
		try
		{
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
			return await _helper.PostAsync("GetReceipt", moNo);
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	private async Task<DataTable> GetPlan(int type)
	{
		try
		{
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
			return await _helper.PostAsync("GetPlan", type);
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	private async Task<DataTable> GetClosed()
	{
		try
		{
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
			return await _helper.GetDataTableAsync("GetClosed");
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	private async Task<DataTable> GetRelease()
	{
		try
		{
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
			return await _helper.GetDataTableAsync("GetRelease");
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	private async Task<HttpResult> ProPlanning(List<ProductionOder_Impl_OWOR> lst)
	{
		try
		{
			using HttpClientHelper<HttpResult> _helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "ProductionOder");
			return await _helper.PostAsync("ProPlanning", lst);
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task<DataTable> APZ_ProductionPlanning_GetDataReceiptByMOCode(DataTable lst)
	{
		try
		{
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
			return await _helper.PostAsync("APZ_ProductionPlanning_GetDataReceiptByMOCode", lst);
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return null;
		}
	}

	private async Task<bool> AwaitCheckControlKey(string keyCode)
	{
		try
		{
			using HttpClientHelper<string> _helper = new HttpClientHelper<string>(Configuration.ApiServer, FormController);
			if (string.IsNullOrEmpty(await _helper.PostAsync("CheckControlKey", keyCode)))
			{
				return true;
			}
			return false;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return false;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.MTO_Manufacturing.frm_ProductionPlanning));
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions4 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions5 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled8 = new DevExpress.Utils.SerializableAppearanceObject();
		this.panel1 = new System.Windows.Forms.Panel();
		this.btn_Plan = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnMOPlanning = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btCheckCapacity = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btSave = new Apzon.Controls.Controls.SimpleButtonEx();
		this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
		this.dtToDate = new Apzon.Controls.Editor.DateEditEx();
		this.dtFromDate = new Apzon.Controls.Editor.DateEditEx();
		this.btSearch = new Apzon.Controls.Controls.SimpleButtonEx();
		this.lb_ToDate = new Apzon.Controls.Controls.LabelLine();
		this.lb_FromDate = new Apzon.Controls.Controls.LabelLine();
		this.txtCardCode = new Apzon.Controls.Editor.ButtonEditEx();
		this.txtCardName = new Apzon.Controls.Editor.TextEditEx();
		this.lb_CustomerName = new Apzon.Controls.Controls.LabelLine();
		this.lb_CustomerCode = new Apzon.Controls.Controls.LabelLine();
		this.xtraTabControl1 = new Apzon.Controls.Controls.XtraTabControlEx();
		this.tabMO = new Apzon.Controls.Controls.XtraTabPageEx();
		this.splitContainerControl2 = new DevExpress.XtraEditors.SplitContainerControl();
		this.grdMO = new Apzon.Controls.Grid.GridControlEx();
		this.ctxDocsMO = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gvMO = new Apzon.Controls.Grid.GridViewEx();
		this.clMONo = new Apzon.Controls.Grid.GridColumnEx();
		this.clMoName = new Apzon.Controls.Grid.GridColumnEx();
		this.clPostDate = new Apzon.Controls.Grid.GridColumnEx();
		this.clDueDate = new Apzon.Controls.Grid.GridColumnEx();
		this.colItemCode = new Apzon.Controls.Grid.GridColumnEx();
		this.repItemCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.clRouting = new Apzon.Controls.Grid.GridColumnEx();
		this.repRouting = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.clBatchToIssue = new Apzon.Controls.Grid.GridColumnEx();
		this.clUoM = new Apzon.Controls.Grid.GridColumnEx();
		this.repUOM = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.clQuantity = new Apzon.Controls.Grid.GridColumnEx();
		this.clWidth = new Apzon.Controls.Grid.GridColumnEx();
		this.clLength = new Apzon.Controls.Grid.GridColumnEx();
		this.clDistRule1 = new Apzon.Controls.Grid.GridColumnEx();
		this.repOcr1 = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.clDistRule2 = new Apzon.Controls.Grid.GridColumnEx();
		this.repOcr2 = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.clDistRule3 = new Apzon.Controls.Grid.GridColumnEx();
		this.repOcr3 = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.clDistRule4 = new Apzon.Controls.Grid.GridColumnEx();
		this.repOcr4 = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.clDistRule5 = new Apzon.Controls.Grid.GridColumnEx();
		this.repOcr5 = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.clProject = new Apzon.Controls.Grid.GridColumnEx();
		this.clFixedBatch = new Apzon.Controls.Grid.GridColumnEx();
		this.repFixBatch = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.clStatus = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Status = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.clRemark = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemMemoExEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit();
		this.FixBatch = new Apzon.Controls.Grid.GridColumnEx();
		this.DocEntrySO = new Apzon.Controls.Grid.GridColumnEx();
		this.LineNumSO = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemMemoEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
		this.gridView5 = new Apzon.Controls.Grid.GridViewEx();
		this.xtratabDoc = new Apzon.Controls.Controls.XtraTabControlEx();
		this.tab_Routing = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grdRouting = new Apzon.Controls.Grid.GridControlEx();
		this.contextRouting = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.deleteRowToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.insertRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gvRouting = new Apzon.Controls.Grid.GridViewEx();
		this.MONo = new Apzon.Controls.Grid.GridColumnEx();
		this.colUoMTime = new Apzon.Controls.Grid.GridColumnEx();
		this.columBatchIssue = new Apzon.Controls.Grid.GridColumnEx();
		this.clOpCode = new Apzon.Controls.Grid.GridColumnEx();
		this.repOpCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.clSupOpCode = new Apzon.Controls.Grid.GridColumnEx();
		this.repSupCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.clWorkCenter = new Apzon.Controls.Grid.GridColumnEx();
		this.repWorkCenter = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.clCtrKey = new Apzon.Controls.Grid.GridColumnEx();
		this.re_ControlKey = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.clBatchQuantity = new Apzon.Controls.Grid.GridColumnEx();
		this.clBaseQty = new Apzon.Controls.Grid.GridColumnEx();
		this.clDescription = new Apzon.Controls.Grid.GridColumnEx();
		this.colRemark = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemMemoExEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit();
		this.columUOm = new Apzon.Controls.Grid.GridColumnEx();
		this.repUOMRouting = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.clSetUpTime = new Apzon.Controls.Grid.GridColumnEx();
		this.repTime = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.clLaborTime = new Apzon.Controls.Grid.GridColumnEx();
		this.clMachineTime = new Apzon.Controls.Grid.GridColumnEx();
		this.clPRT = new Apzon.Controls.Grid.GridColumnEx();
		this.repPRT = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.clBOM = new Apzon.Controls.Grid.GridColumnEx();
		this.clPrOp = new Apzon.Controls.Grid.GridColumnEx();
		this.repPreOp = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.clNextOp = new Apzon.Controls.Grid.GridColumnEx();
		this.repNextOp = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.LineNum = new Apzon.Controls.Grid.GridColumnEx();
		this.UoMSetup = new Apzon.Controls.Grid.GridColumnEx();
		this.UoMLabor = new Apzon.Controls.Grid.GridColumnEx();
		this.UoMMachine = new Apzon.Controls.Grid.GridColumnEx();
		this.PrtCode = new Apzon.Controls.Grid.GridColumnEx();
		this.ItemCode = new Apzon.Controls.Grid.GridColumnEx();
		this.reitemcode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.Whse = new Apzon.Controls.Grid.GridColumnEx();
		this.repWhse = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gridView4 = new Apzon.Controls.Grid.GridViewEx();
		this.tab_ReceiptFromProduction = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grdReceipt = new Apzon.Controls.Grid.GridControlEx();
		this.gvReceipt = new Apzon.Controls.Grid.GridViewEx();
		this.colMONo = new Apzon.Controls.Grid.GridColumnEx();
		this.colLineNum = new Apzon.Controls.Grid.GridColumnEx();
		this.clItemCode = new Apzon.Controls.Grid.GridColumnEx();
		this.clItemName = new Apzon.Controls.Grid.GridColumnEx();
		this.clOrderCode = new Apzon.Controls.Grid.GridColumnEx();
		this.repSaleorderNo = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
		this.clOderLine = new Apzon.Controls.Grid.GridColumnEx();
		this.repSOLine = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
		this.clDelDate = new Apzon.Controls.Grid.GridColumnEx();
		this.repDelDate = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.clBatch = new Apzon.Controls.Grid.GridColumnEx();
		this.colWidth = new Apzon.Controls.Grid.GridColumnEx();
		this.colLength = new Apzon.Controls.Grid.GridColumnEx();
		this.colUOM = new Apzon.Controls.Grid.GridColumnEx();
		this.repUOMReceipt = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.colQuantity = new Apzon.Controls.Grid.GridColumnEx();
		this.colWHse = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_WHSE = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.colProject = new Apzon.Controls.Grid.GridColumnEx();
		this.colOperation = new Apzon.Controls.Grid.GridColumnEx();
		this.clDelTime = new Apzon.Controls.Grid.GridColumnEx();
		this.clBatchIssue = new Apzon.Controls.Grid.GridColumnEx();
		this.rep = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
		this.clBatchComplete = new Apzon.Controls.Grid.GridColumnEx();
		this.repGetBatch = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.clThickPlan = new Apzon.Controls.Grid.GridColumnEx();
		this.WidthPlan = new Apzon.Controls.Grid.GridColumnEx();
		this.LengthPlan = new Apzon.Controls.Grid.GridColumnEx();
		this.ThickFinal = new Apzon.Controls.Grid.GridColumnEx();
		this.WidthFinal = new Apzon.Controls.Grid.GridColumnEx();
		this.LengthFinal = new Apzon.Controls.Grid.GridColumnEx();
		this.OcrCode = new Apzon.Controls.Grid.GridColumnEx();
		this.OcrCode2 = new Apzon.Controls.Grid.GridColumnEx();
		this.OcrCode3 = new Apzon.Controls.Grid.GridColumnEx();
		this.OcrCode4 = new Apzon.Controls.Grid.GridColumnEx();
		this.OcrCode5 = new Apzon.Controls.Grid.GridColumnEx();
		this.UoM = new Apzon.Controls.Grid.GridColumnEx();
		this.OpCode = new Apzon.Controls.Grid.GridColumnEx();
		this.Project = new Apzon.Controls.Grid.GridColumnEx();
		this.Comments = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemMemoExEdit4 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit();
		this.gridColumn23 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn24 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn25 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn26 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn27 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn28 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn29 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn30 = new Apzon.Controls.Grid.GridColumnEx();
		this.repBatComplete = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
		this.repoWhse = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.repositoryItemMemoEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
		this.repositoryItemMemoExEdit3 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit();
		this.repositoryItemMemoEdit3 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
		this.gridView2 = new Apzon.Controls.Grid.GridViewEx();
		this.tab_Attachment = new Apzon.Controls.Controls.XtraTabPageEx();
		this.gridControl3 = new Apzon.Controls.Grid.GridControlEx();
		this.gridView3 = new Apzon.Controls.Grid.GridViewEx();
		this.gcPath = new Apzon.Controls.Grid.GridColumnEx();
		this.gcFileName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcExtention = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDownload = new Apzon.Controls.Grid.GridColumnEx();
		this.gridView1 = new Apzon.Controls.Grid.GridViewEx();
		this.tabPlaned = new Apzon.Controls.Controls.XtraTabPageEx();
		this.btn = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_Planed = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Planed = new Apzon.Controls.Grid.GridViewEx();
		this.columMoNo = new Apzon.Controls.Grid.GridColumnEx();
		this.columMoName = new Apzon.Controls.Grid.GridColumnEx();
		this.colItemName = new Apzon.Controls.Grid.GridColumnEx();
		this.clumItemName = new Apzon.Controls.Grid.GridColumnEx();
		this.clumQuantity = new Apzon.Controls.Grid.GridColumnEx();
		this.coluOrDer = new Apzon.Controls.Grid.GridColumnEx();
		this.columnWhse = new Apzon.Controls.Grid.GridColumnEx();
		this.columDueDate = new Apzon.Controls.Grid.GridColumnEx();
		this.columnRemark = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemMemoExEdit6 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit();
		this.clProduc = new Apzon.Controls.Grid.GridColumnEx();
		this.columnBatchIssue = new Apzon.Controls.Grid.GridColumnEx();
		this.colBatchComplete = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemMemoExEdit5 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit();
		this.tabRelease = new Apzon.Controls.Controls.XtraTabPageEx();
		this.btnRelease = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_Release = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Release = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn10 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn11 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn12 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn13 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn14 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn15 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn16 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn17 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn18 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemMemoExEdit7 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit();
		this.gridColumn19 = new Apzon.Controls.Grid.GridColumnEx();
		this.columBatchComplete = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn20 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemMemoEdit4 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
		this.tabClose = new Apzon.Controls.Controls.XtraTabPageEx();
		this.gridControl1 = new Apzon.Controls.Grid.GridControlEx();
		this.gridView6 = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn5 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn6 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn7 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn8 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn9 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn21 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn22 = new Apzon.Controls.Grid.GridColumnEx();
		this.panel1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.panelControl1).BeginInit();
		this.panelControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.dtToDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dtToDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dtFromDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dtFromDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtCardCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtCardName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).BeginInit();
		this.xtraTabControl1.SuspendLayout();
		this.tabMO.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl2).BeginInit();
		this.splitContainerControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdMO).BeginInit();
		this.ctxDocsMO.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gvMO).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repItemCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repRouting).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repUOM).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repOcr1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repOcr2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repOcr3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repOcr4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repOcr5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repFixBatch).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Status).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridView5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.xtratabDoc).BeginInit();
		this.xtratabDoc.SuspendLayout();
		this.tab_Routing.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdRouting).BeginInit();
		this.contextRouting.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gvRouting).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repOpCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repSupCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repWorkCenter).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.re_ControlKey).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repUOMRouting).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repTime).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repPRT).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repPreOp).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repNextOp).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.reitemcode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repWhse).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridView4).BeginInit();
		this.tab_ReceiptFromProduction.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdReceipt).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gvReceipt).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repSaleorderNo).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repSOLine).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repDelDate).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repDelDate.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repUOMReceipt).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_WHSE).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repGetBatch).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repBatComplete).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repoWhse).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoEdit2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoEdit3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridView2).BeginInit();
		this.tab_Attachment.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gridControl3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridView3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridView1).BeginInit();
		this.tabPlaned.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Planed).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Planed).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit6).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit5).BeginInit();
		this.tabRelease.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Release).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Release).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit7).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoEdit4).BeginInit();
		this.tabClose.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gridControl1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridView6).BeginInit();
		base.SuspendLayout();
		this.panel1.Controls.Add(this.btn_Plan);
		this.panel1.Controls.Add(this.btnMOPlanning);
		this.panel1.Controls.Add(this.btCheckCapacity);
		this.panel1.Controls.Add(this.btCancel);
		this.panel1.Controls.Add(this.btSave);
		this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.panel1.Location = new System.Drawing.Point(0, 222);
		this.panel1.Name = "panel1";
		this.panel1.Size = new System.Drawing.Size(740, 33);
		this.panel1.TabIndex = 63;
		this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
		this.btn_Plan.Appearance.Font = new System.Drawing.Font("Tahoma", 7.5f);
		this.btn_Plan.Appearance.Options.UseFont = true;
		this.btn_Plan.EditControl = null;
		this.btn_Plan.EditValue = "Plan";
		this.btn_Plan.FieldName = null;
		this.btn_Plan.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Plan.ImageOptions.Image");
		this.btn_Plan.IsSearchControl = false;
		this.btn_Plan.Location = new System.Drawing.Point(115, 7);
		this.btn_Plan.Name = "btn_Plan";
		this.btn_Plan.ReadOnly = false;
		this.btn_Plan.SettingInVisible = false;
		this.btn_Plan.SettingReadonly = false;
		this.btn_Plan.Size = new System.Drawing.Size(99, 19);
		this.btn_Plan.TabIndex = 17;
		this.btn_Plan.Text = "Plan";
		this.btn_Plan.Click += new System.EventHandler(this.btn_Plan_Click);
		this.btnMOPlanning.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btnMOPlanning.Appearance.Font = new System.Drawing.Font("Tahoma", 7.5f);
		this.btnMOPlanning.Appearance.Options.UseFont = true;
		this.btnMOPlanning.EditControl = null;
		this.btnMOPlanning.EditValue = "M/O Planning";
		this.btnMOPlanning.FieldName = null;
		this.btnMOPlanning.IsSearchControl = false;
		this.btnMOPlanning.Location = new System.Drawing.Point(627, 7);
		this.btnMOPlanning.Name = "btnMOPlanning";
		this.btnMOPlanning.ReadOnly = false;
		this.btnMOPlanning.SettingInVisible = false;
		this.btnMOPlanning.SettingReadonly = false;
		this.btnMOPlanning.Size = new System.Drawing.Size(99, 19);
		this.btnMOPlanning.TabIndex = 16;
		this.btnMOPlanning.Text = "M/O Planning";
		this.btnMOPlanning.Click += new System.EventHandler(this.simpleButton2_Click);
		this.btCheckCapacity.Appearance.Font = new System.Drawing.Font("Tahoma", 7.5f);
		this.btCheckCapacity.Appearance.Options.UseFont = true;
		this.btCheckCapacity.EditControl = null;
		this.btCheckCapacity.EditValue = "Check Capacity";
		this.btCheckCapacity.FieldName = null;
		this.btCheckCapacity.IsSearchControl = false;
		this.btCheckCapacity.Location = new System.Drawing.Point(329, 7);
		this.btCheckCapacity.Name = "btCheckCapacity";
		this.btCheckCapacity.ReadOnly = false;
		this.btCheckCapacity.SettingInVisible = false;
		this.btCheckCapacity.SettingReadonly = false;
		this.btCheckCapacity.Size = new System.Drawing.Size(99, 19);
		this.btCheckCapacity.TabIndex = 15;
		this.btCheckCapacity.Text = "Check Capacity";
		this.btCancel.Appearance.Font = new System.Drawing.Font("Tahoma", 7.5f);
		this.btCancel.Appearance.Options.UseFont = true;
		this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btCancel.EditControl = null;
		this.btCancel.EditValue = "Cancel";
		this.btCancel.FieldName = null;
		this.btCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btCancel.ImageOptions.Image");
		this.btCancel.IsSearchControl = false;
		this.btCancel.Location = new System.Drawing.Point(225, 7);
		this.btCancel.Name = "btCancel";
		this.btCancel.ReadOnly = false;
		this.btCancel.SettingInVisible = false;
		this.btCancel.SettingReadonly = false;
		this.btCancel.Size = new System.Drawing.Size(99, 19);
		this.btCancel.TabIndex = 14;
		this.btCancel.Text = "Cancel";
		this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
		this.btSave.Appearance.Font = new System.Drawing.Font("Tahoma", 7.5f);
		this.btSave.Appearance.Options.UseFont = true;
		this.btSave.ButtonKind = Apzon.Commons.Enums.Enums.SimpleButtonType.Save;
		this.btSave.EditControl = null;
		this.btSave.EditValue = "Save";
		this.btSave.FieldName = null;
		this.btSave.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btSave.ImageOptions.Image");
		this.btSave.IsSearchControl = false;
		this.btSave.Location = new System.Drawing.Point(10, 7);
		this.btSave.Name = "btSave";
		this.btSave.ReadOnly = false;
		this.btSave.SettingInVisible = false;
		this.btSave.SettingReadonly = false;
		this.btSave.Size = new System.Drawing.Size(99, 19);
		this.btSave.TabIndex = 13;
		this.btSave.Text = "Save";
		this.btSave.Click += new System.EventHandler(this.btSave_Click);
		this.panelControl1.Controls.Add(this.dtToDate);
		this.panelControl1.Controls.Add(this.dtFromDate);
		this.panelControl1.Controls.Add(this.btSearch);
		this.panelControl1.Controls.Add(this.lb_ToDate);
		this.panelControl1.Controls.Add(this.lb_FromDate);
		this.panelControl1.Controls.Add(this.txtCardCode);
		this.panelControl1.Controls.Add(this.txtCardName);
		this.panelControl1.Controls.Add(this.lb_CustomerName);
		this.panelControl1.Controls.Add(this.lb_CustomerCode);
		this.panelControl1.Dock = System.Windows.Forms.DockStyle.Top;
		this.panelControl1.Location = new System.Drawing.Point(0, 0);
		this.panelControl1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.panelControl1.Name = "panelControl1";
		this.panelControl1.Size = new System.Drawing.Size(742, 53);
		this.panelControl1.TabIndex = 64;
		this.panelControl1.Paint += new System.Windows.Forms.PaintEventHandler(this.panelControl1_Paint);
		this.dtToDate.EditControl = null;
		this.dtToDate.EditValue = null;
		this.dtToDate.FieldName = null;
		this.dtToDate.Location = new System.Drawing.Point(447, 28);
		this.dtToDate.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.dtToDate.Name = "dtToDate";
		this.dtToDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dtToDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dtToDate.Properties.IsSearchControl = false;
		this.dtToDate.SettingInVisible = false;
		this.dtToDate.SettingReadonly = false;
		this.dtToDate.Size = new System.Drawing.Size(147, 20);
		this.dtToDate.TabIndex = 69;
		this.dtFromDate.EditControl = null;
		this.dtFromDate.EditValue = null;
		this.dtFromDate.FieldName = null;
		this.dtFromDate.Location = new System.Drawing.Point(447, 3);
		this.dtFromDate.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.dtFromDate.Name = "dtFromDate";
		this.dtFromDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dtFromDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dtFromDate.Properties.IsSearchControl = false;
		this.dtFromDate.SettingInVisible = false;
		this.dtFromDate.SettingReadonly = false;
		this.dtFromDate.Size = new System.Drawing.Size(147, 20);
		this.dtFromDate.TabIndex = 68;
		this.btSearch.Appearance.Font = new System.Drawing.Font("Tahoma", 7.5f);
		this.btSearch.Appearance.Options.UseFont = true;
		this.btSearch.EditControl = null;
		this.btSearch.EditValue = "Search";
		this.btSearch.FieldName = null;
		this.btSearch.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btSearch.ImageOptions.Image");
		this.btSearch.IsSearchControl = false;
		this.btSearch.Location = new System.Drawing.Point(615, 27);
		this.btSearch.Name = "btSearch";
		this.btSearch.ReadOnly = false;
		this.btSearch.SettingInVisible = false;
		this.btSearch.SettingReadonly = false;
		this.btSearch.Size = new System.Drawing.Size(99, 19);
		this.btSearch.TabIndex = 66;
		this.btSearch.Text = "Search";
		this.btSearch.Click += new System.EventHandler(this.btSearch_Click);
		this.lb_ToDate.EditControl = null;
		this.lb_ToDate.EditValue = "To Date";
		this.lb_ToDate.FieldName = null;
		this.lb_ToDate.HideWhenEmpty = false;
		this.lb_ToDate.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_ToDate.Location = new System.Drawing.Point(336, 32);
		this.lb_ToDate.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_ToDate.Name = "lb_ToDate";
		this.lb_ToDate.ReadOnly = false;
		this.lb_ToDate.SettingInVisible = false;
		this.lb_ToDate.SettingReadonly = false;
		this.lb_ToDate.ShowUnderLine = true;
		this.lb_ToDate.Size = new System.Drawing.Size(114, 20);
		this.lb_ToDate.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_ToDate.TabIndex = 64;
		this.lb_ToDate.Utext = "To Date";
		this.lb_FromDate.EditControl = null;
		this.lb_FromDate.EditValue = "From Date";
		this.lb_FromDate.FieldName = null;
		this.lb_FromDate.HideWhenEmpty = false;
		this.lb_FromDate.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_FromDate.Location = new System.Drawing.Point(336, 7);
		this.lb_FromDate.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_FromDate.Name = "lb_FromDate";
		this.lb_FromDate.ReadOnly = false;
		this.lb_FromDate.SettingInVisible = false;
		this.lb_FromDate.SettingReadonly = false;
		this.lb_FromDate.ShowUnderLine = true;
		this.lb_FromDate.Size = new System.Drawing.Size(114, 20);
		this.lb_FromDate.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_FromDate.TabIndex = 63;
		this.lb_FromDate.Utext = "From Date";
		this.txtCardCode.EditControl = null;
		this.txtCardCode.FieldName = null;
		this.txtCardCode.IsSearchControl = false;
		this.txtCardCode.Location = new System.Drawing.Point(127, 4);
		this.txtCardCode.Name = "txtCardCode";
		this.txtCardCode.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 6.5f);
		this.txtCardCode.Properties.Appearance.Options.UseFont = true;
		this.txtCardCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.txtCardCode.SettingInVisible = false;
		this.txtCardCode.SettingReadonly = false;
		this.txtCardCode.Size = new System.Drawing.Size(147, 18);
		this.txtCardCode.TabIndex = 52;
		this.txtCardCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtCardCode_ButtonClick);
		this.txtCardName.EditControl = null;
		this.txtCardName.FieldName = null;
		this.txtCardName.IsSearchControl = false;
		this.txtCardName.Location = new System.Drawing.Point(127, 27);
		this.txtCardName.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.txtCardName.Name = "txtCardName";
		this.txtCardName.SettingInVisible = false;
		this.txtCardName.SettingReadonly = false;
		this.txtCardName.Size = new System.Drawing.Size(147, 20);
		this.txtCardName.TabIndex = 51;
		this.lb_CustomerName.EditControl = null;
		this.lb_CustomerName.EditValue = "Item Name";
		this.lb_CustomerName.FieldName = null;
		this.lb_CustomerName.HideWhenEmpty = false;
		this.lb_CustomerName.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_CustomerName.Location = new System.Drawing.Point(12, 31);
		this.lb_CustomerName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_CustomerName.Name = "lb_CustomerName";
		this.lb_CustomerName.ReadOnly = false;
		this.lb_CustomerName.SettingInVisible = false;
		this.lb_CustomerName.SettingReadonly = false;
		this.lb_CustomerName.ShowUnderLine = true;
		this.lb_CustomerName.Size = new System.Drawing.Size(117, 20);
		this.lb_CustomerName.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_CustomerName.TabIndex = 50;
		this.lb_CustomerName.Utext = "Item Name";
		this.lb_CustomerCode.EditControl = null;
		this.lb_CustomerCode.EditValue = "Item Code";
		this.lb_CustomerCode.FieldName = null;
		this.lb_CustomerCode.HideWhenEmpty = false;
		this.lb_CustomerCode.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_CustomerCode.Location = new System.Drawing.Point(12, 6);
		this.lb_CustomerCode.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_CustomerCode.Name = "lb_CustomerCode";
		this.lb_CustomerCode.ReadOnly = false;
		this.lb_CustomerCode.SettingInVisible = false;
		this.lb_CustomerCode.SettingReadonly = false;
		this.lb_CustomerCode.ShowUnderLine = true;
		this.lb_CustomerCode.Size = new System.Drawing.Size(117, 20);
		this.lb_CustomerCode.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_CustomerCode.TabIndex = 49;
		this.lb_CustomerCode.Utext = "Item Code";
		this.xtraTabControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.xtraTabControl1.IsSearchControl = false;
		this.xtraTabControl1.Location = new System.Drawing.Point(1, 57);
		this.xtraTabControl1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.xtraTabControl1.Name = "xtraTabControl1";
		this.xtraTabControl1.SelectedTabPage = this.tabMO;
		this.xtraTabControl1.Size = new System.Drawing.Size(742, 410);
		this.xtraTabControl1.TabIndex = 0;
		this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[4] { this.tabMO, this.tabPlaned, this.tabRelease, this.tabClose });
		this.xtraTabControl1.SelectedPageChanged += new DevExpress.XtraTab.TabPageChangedEventHandler(this.xtraTabControl1_SelectedPageChanged);
		this.tabMO.Controls.Add(this.splitContainerControl2);
		this.tabMO.EditControl = null;
		this.tabMO.EditValue = "General M/O";
		this.tabMO.FieldName = null;
		this.tabMO.IsSearchControl = false;
		this.tabMO.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.tabMO.Name = "tabMO";
		this.tabMO.ReadOnly = false;
		this.tabMO.SettingInVisible = false;
		this.tabMO.SettingReadonly = false;
		this.tabMO.Size = new System.Drawing.Size(740, 385);
		this.tabMO.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tabMO.Text = "General M/O";
		this.splitContainerControl2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainerControl2.Horizontal = false;
		this.splitContainerControl2.Location = new System.Drawing.Point(0, 0);
		this.splitContainerControl2.Name = "splitContainerControl2";
		this.splitContainerControl2.Panel1.Controls.Add(this.grdMO);
		this.splitContainerControl2.Panel1.Text = "Panel1";
		this.splitContainerControl2.Panel2.Controls.Add(this.xtratabDoc);
		this.splitContainerControl2.Panel2.Controls.Add(this.panel1);
		this.splitContainerControl2.Panel2.Text = "Panel2";
		this.splitContainerControl2.Size = new System.Drawing.Size(740, 385);
		this.splitContainerControl2.SplitterPosition = 120;
		this.splitContainerControl2.TabIndex = 1;
		this.splitContainerControl2.Text = "splitContainerControl2";
		this.grdMO.AllowPasteExcel = true;
		this.grdMO.ClipboardData = null;
		this.grdMO.ContextMenuStrip = this.ctxDocsMO;
		this.grdMO.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grdMO.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grdMO.Location = new System.Drawing.Point(0, 0);
		this.grdMO.MainView = this.gvMO;
		this.grdMO.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grdMO.Name = "grdMO";
		this.grdMO.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[12]
		{
			this.rep_Status, this.repFixBatch, this.repUOM, this.repOcr1, this.repOcr2, this.repOcr3, this.repOcr4, this.repOcr5, this.repItemCode, this.repRouting,
			this.repositoryItemMemoEdit1, this.repositoryItemMemoExEdit1
		});
		this.grdMO.Size = new System.Drawing.Size(740, 120);
		this.grdMO.TabIndex = 0;
		this.grdMO.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[2] { this.gvMO, this.gridView5 });
		this.ctxDocsMO.ImageScalingSize = new System.Drawing.Size(20, 20);
		this.ctxDocsMO.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.deleteRowToolStripMenuItem });
		this.ctxDocsMO.Name = "ctxDocsMO";
		this.ctxDocsMO.Size = new System.Drawing.Size(144, 26);
		this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
		this.deleteRowToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
		this.deleteRowToolStripMenuItem.Text = "Remove Row";
		this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.deleteRowToolStripMenuItem_Click);
		this.gvMO.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvMO.ColumnKey = null;
		this.gvMO.ColumnPanelRowHeight = 40;
		this.gvMO.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[23]
		{
			this.clMONo, this.clMoName, this.clPostDate, this.clDueDate, this.colItemCode, this.clRouting, this.clBatchToIssue, this.clUoM, this.clQuantity, this.clWidth,
			this.clLength, this.clDistRule1, this.clDistRule2, this.clDistRule3, this.clDistRule4, this.clDistRule5, this.clProject, this.clFixedBatch, this.clStatus, this.clRemark,
			this.FixBatch, this.DocEntrySO, this.LineNumSO
		});
		this.gvMO.DisableFormSetting = false;
		this.gvMO.DisplayException = false;
		this.gvMO.EditColumn = null;
		this.gvMO.FieldCurrency = null;
		this.gvMO.FieldRate = null;
		this.gvMO.FormId = null;
		this.gvMO.GridControl = this.grdMO;
		this.gvMO.IgnoreFilter = false;
		this.gvMO.IndexEditRow = -1;
		this.gvMO.IndicatorWidth = 40;
		this.gvMO.IsApzTable = false;
		this.gvMO.IsInsertNewRow = false;
		this.gvMO.IsLoadingData = false;
		this.gvMO.IsMappingContextAction = false;
		this.gvMO.ItemId = null;
		this.gvMO.LstColumnAllowBatch = null;
		this.gvMO.LstColumnNotAllowBatch = null;
		this.gvMO.LstColumnNotEditable = null;
		this.gvMO.LstColumNotSetting = null;
		this.gvMO.Name = "gvMO";
		this.gvMO.OptionsView.ColumnAutoWidth = false;
		this.gvMO.OptionsView.ShowGroupPanel = false;
		this.gvMO.UseBatchMod = false;
		this.gvMO.RowClick += new DevExpress.XtraGrid.Views.Grid.RowClickEventHandler(this.gvMO_RowClick);
		this.gvMO.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gvMO_CustomDrawRowIndicator);
		this.gvMO.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.gvMO_RowStyle);
		this.gvMO.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gvMO_FocusedRowObjectChanged);
		this.gvMO.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvMO_CellValueChanged);
		this.clMONo.AppearanceHeader.Options.UseTextOptions = true;
		this.clMONo.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.clMONo.AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
		this.clMONo.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
		this.clMONo.Caption = "Master Order No.";
		this.clMONo.CurrencyDateField = null;
		this.clMONo.CurrencyField = null;
		this.clMONo.CurrencyFixed = null;
		this.clMONo.CurrencyFixedRate = 0.0;
		this.clMONo.CurrencyRateField = null;
		this.clMONo.FieldName = "MONo";
		this.clMONo.Name = "clMONo";
		this.clMONo.OptionsColumn.AllowEdit = false;
		this.clMONo.PrimaryKey = false;
		this.clMONo.UnitField = null;
		this.clMONo.Visible = true;
		this.clMONo.VisibleIndex = 0;
		this.clMoName.AppearanceHeader.Options.UseTextOptions = true;
		this.clMoName.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.clMoName.AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
		this.clMoName.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
		this.clMoName.Caption = "Master Order Name";
		this.clMoName.CurrencyDateField = null;
		this.clMoName.CurrencyField = null;
		this.clMoName.CurrencyFixed = null;
		this.clMoName.CurrencyFixedRate = 0.0;
		this.clMoName.CurrencyRateField = null;
		this.clMoName.FieldName = "MOName";
		this.clMoName.Name = "clMoName";
		this.clMoName.PrimaryKey = false;
		this.clMoName.UnitField = null;
		this.clMoName.Visible = true;
		this.clMoName.VisibleIndex = 1;
		this.clMoName.Width = 120;
		this.clPostDate.Caption = "Posting Date";
		this.clPostDate.CurrencyDateField = null;
		this.clPostDate.CurrencyField = null;
		this.clPostDate.CurrencyFixed = null;
		this.clPostDate.CurrencyFixedRate = 0.0;
		this.clPostDate.CurrencyRateField = null;
		this.clPostDate.FieldName = "PostDate";
		this.clPostDate.Name = "clPostDate";
		this.clPostDate.OptionsColumn.AllowEdit = false;
		this.clPostDate.PrimaryKey = false;
		this.clPostDate.UnitField = null;
		this.clPostDate.Visible = true;
		this.clPostDate.VisibleIndex = 11;
		this.clPostDate.Width = 95;
		this.clDueDate.Caption = "Due Date";
		this.clDueDate.CurrencyDateField = null;
		this.clDueDate.CurrencyField = null;
		this.clDueDate.CurrencyFixed = null;
		this.clDueDate.CurrencyFixedRate = 0.0;
		this.clDueDate.CurrencyRateField = null;
		this.clDueDate.FieldName = "DueDate";
		this.clDueDate.Name = "clDueDate";
		this.clDueDate.OptionsColumn.AllowEdit = false;
		this.clDueDate.PrimaryKey = false;
		this.clDueDate.UnitField = null;
		this.clDueDate.Visible = true;
		this.clDueDate.VisibleIndex = 12;
		this.clDueDate.Width = 86;
		this.colItemCode.Caption = "Item Code";
		this.colItemCode.ColumnEdit = this.repItemCode;
		this.colItemCode.CurrencyDateField = null;
		this.colItemCode.CurrencyField = null;
		this.colItemCode.CurrencyFixed = null;
		this.colItemCode.CurrencyFixedRate = 0.0;
		this.colItemCode.CurrencyRateField = null;
		this.colItemCode.FieldName = "ItemCode";
		this.colItemCode.Name = "colItemCode";
		this.colItemCode.OptionsColumn.AllowEdit = false;
		this.colItemCode.PrimaryKey = false;
		this.colItemCode.UnitField = null;
		this.colItemCode.Visible = true;
		this.colItemCode.VisibleIndex = 4;
		this.repItemCode.AutoHeight = false;
		this.repItemCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.repItemCode.Name = "repItemCode";
		this.repItemCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repItemCode_ButtonClick);
		this.clRouting.Caption = "Routing";
		this.clRouting.ColumnEdit = this.repRouting;
		this.clRouting.CurrencyDateField = null;
		this.clRouting.CurrencyField = null;
		this.clRouting.CurrencyFixed = null;
		this.clRouting.CurrencyFixedRate = 0.0;
		this.clRouting.CurrencyRateField = null;
		this.clRouting.FieldName = "RouCode";
		this.clRouting.Name = "clRouting";
		this.clRouting.PrimaryKey = false;
		this.clRouting.UnitField = null;
		this.clRouting.Visible = true;
		this.clRouting.VisibleIndex = 6;
		this.repRouting.AutoHeight = false;
		this.repRouting.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.repRouting.Name = "repRouting";
		this.repRouting.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repRouting_ButtonClick);
		this.clBatchToIssue.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.clBatchToIssue.AppearanceCell.Options.UseFont = true;
		this.clBatchToIssue.AppearanceCell.Options.UseTextOptions = true;
		this.clBatchToIssue.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.clBatchToIssue.AppearanceCell.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
		this.clBatchToIssue.AppearanceCell.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
		this.clBatchToIssue.AppearanceHeader.Options.UseTextOptions = true;
		this.clBatchToIssue.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.clBatchToIssue.AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
		this.clBatchToIssue.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
		this.clBatchToIssue.Caption = "Batch To Issue";
		this.clBatchToIssue.CurrencyDateField = null;
		this.clBatchToIssue.CurrencyField = null;
		this.clBatchToIssue.CurrencyFixed = null;
		this.clBatchToIssue.CurrencyFixedRate = 0.0;
		this.clBatchToIssue.CurrencyRateField = null;
		this.clBatchToIssue.FieldName = "BatchToIssue";
		this.clBatchToIssue.Name = "clBatchToIssue";
		this.clBatchToIssue.OptionsColumn.AllowEdit = false;
		this.clBatchToIssue.PrimaryKey = false;
		this.clBatchToIssue.UnitField = null;
		this.clBatchToIssue.Visible = true;
		this.clBatchToIssue.VisibleIndex = 5;
		this.clBatchToIssue.Width = 139;
		this.clUoM.Caption = "UoM";
		this.clUoM.ColumnEdit = this.repUOM;
		this.clUoM.CurrencyDateField = null;
		this.clUoM.CurrencyField = null;
		this.clUoM.CurrencyFixed = null;
		this.clUoM.CurrencyFixedRate = 0.0;
		this.clUoM.CurrencyRateField = null;
		this.clUoM.FieldName = "UoM";
		this.clUoM.Name = "clUoM";
		this.clUoM.OptionsColumn.AllowEdit = false;
		this.clUoM.PrimaryKey = false;
		this.clUoM.UnitField = null;
		this.clUoM.Visible = true;
		this.clUoM.VisibleIndex = 8;
		this.repUOM.AutoHeight = false;
		this.repUOM.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repUOM.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ID", "ID", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.repUOM.DisplayMember = "Name";
		this.repUOM.Name = "repUOM";
		this.repUOM.ValueMember = "ID";
		this.clQuantity.Caption = "Quantity";
		this.clQuantity.CurrencyDateField = null;
		this.clQuantity.CurrencyField = null;
		this.clQuantity.CurrencyFixed = null;
		this.clQuantity.CurrencyFixedRate = 0.0;
		this.clQuantity.CurrencyRateField = null;
		this.clQuantity.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.clQuantity.FieldName = "Quantity";
		this.clQuantity.Name = "clQuantity";
		this.clQuantity.PrimaryKey = false;
		this.clQuantity.UnitField = null;
		this.clQuantity.Visible = true;
		this.clQuantity.VisibleIndex = 7;
		this.clWidth.Caption = "Width";
		this.clWidth.CurrencyDateField = null;
		this.clWidth.CurrencyField = null;
		this.clWidth.CurrencyFixed = null;
		this.clWidth.CurrencyFixedRate = 0.0;
		this.clWidth.CurrencyRateField = null;
		this.clWidth.DisplayFormat.FormatString = "n2";
		this.clWidth.FieldName = "Width";
		this.clWidth.Name = "clWidth";
		this.clWidth.PrimaryKey = false;
		this.clWidth.UnitField = null;
		this.clWidth.Visible = true;
		this.clWidth.VisibleIndex = 9;
		this.clLength.Caption = "Length";
		this.clLength.CurrencyDateField = null;
		this.clLength.CurrencyField = null;
		this.clLength.CurrencyFixed = null;
		this.clLength.CurrencyFixedRate = 0.0;
		this.clLength.CurrencyRateField = null;
		this.clLength.DisplayFormat.FormatString = "n2";
		this.clLength.FieldName = "Length";
		this.clLength.Name = "clLength";
		this.clLength.PrimaryKey = false;
		this.clLength.UnitField = null;
		this.clLength.Visible = true;
		this.clLength.VisibleIndex = 10;
		this.clDistRule1.Caption = "Dist.Rule 1";
		this.clDistRule1.ColumnEdit = this.repOcr1;
		this.clDistRule1.CurrencyDateField = null;
		this.clDistRule1.CurrencyField = null;
		this.clDistRule1.CurrencyFixed = null;
		this.clDistRule1.CurrencyFixedRate = 0.0;
		this.clDistRule1.CurrencyRateField = null;
		this.clDistRule1.FieldName = "OcrCode";
		this.clDistRule1.Name = "clDistRule1";
		this.clDistRule1.OptionsColumn.AllowEdit = false;
		this.clDistRule1.PrimaryKey = false;
		this.clDistRule1.UnitField = null;
		this.clDistRule1.Visible = true;
		this.clDistRule1.VisibleIndex = 13;
		this.repOcr1.AutoHeight = false;
		this.repOcr1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.repOcr1.Name = "repOcr1";
		this.repOcr1.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repOcr1_ButtonClick);
		this.clDistRule2.Caption = "Dist.Rule 2";
		this.clDistRule2.ColumnEdit = this.repOcr2;
		this.clDistRule2.CurrencyDateField = null;
		this.clDistRule2.CurrencyField = null;
		this.clDistRule2.CurrencyFixed = null;
		this.clDistRule2.CurrencyFixedRate = 0.0;
		this.clDistRule2.CurrencyRateField = null;
		this.clDistRule2.FieldName = "OcrCode2";
		this.clDistRule2.Name = "clDistRule2";
		this.clDistRule2.OptionsColumn.AllowEdit = false;
		this.clDistRule2.PrimaryKey = false;
		this.clDistRule2.UnitField = null;
		this.clDistRule2.Visible = true;
		this.clDistRule2.VisibleIndex = 14;
		this.repOcr2.AutoHeight = false;
		this.repOcr2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.repOcr2.Name = "repOcr2";
		this.repOcr2.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repOcr2_ButtonClick);
		this.clDistRule3.Caption = "Dist.Rule 3";
		this.clDistRule3.ColumnEdit = this.repOcr3;
		this.clDistRule3.CurrencyDateField = null;
		this.clDistRule3.CurrencyField = null;
		this.clDistRule3.CurrencyFixed = null;
		this.clDistRule3.CurrencyFixedRate = 0.0;
		this.clDistRule3.CurrencyRateField = null;
		this.clDistRule3.FieldName = "OcrCode3";
		this.clDistRule3.Name = "clDistRule3";
		this.clDistRule3.OptionsColumn.AllowEdit = false;
		this.clDistRule3.PrimaryKey = false;
		this.clDistRule3.UnitField = null;
		this.clDistRule3.Visible = true;
		this.clDistRule3.VisibleIndex = 15;
		this.repOcr3.AutoHeight = false;
		this.repOcr3.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.repOcr3.Name = "repOcr3";
		this.repOcr3.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repOcr3_ButtonClick);
		this.clDistRule4.Caption = "Dist.Rule 4";
		this.clDistRule4.ColumnEdit = this.repOcr4;
		this.clDistRule4.CurrencyDateField = null;
		this.clDistRule4.CurrencyField = null;
		this.clDistRule4.CurrencyFixed = null;
		this.clDistRule4.CurrencyFixedRate = 0.0;
		this.clDistRule4.CurrencyRateField = null;
		this.clDistRule4.FieldName = "OcrCode4";
		this.clDistRule4.Name = "clDistRule4";
		this.clDistRule4.OptionsColumn.AllowEdit = false;
		this.clDistRule4.PrimaryKey = false;
		this.clDistRule4.UnitField = null;
		this.clDistRule4.Visible = true;
		this.clDistRule4.VisibleIndex = 16;
		this.repOcr4.AutoHeight = false;
		this.repOcr4.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.repOcr4.Name = "repOcr4";
		this.repOcr4.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repOcr4_ButtonClick);
		this.clDistRule5.Caption = "Dist.Rule 5";
		this.clDistRule5.ColumnEdit = this.repOcr5;
		this.clDistRule5.CurrencyDateField = null;
		this.clDistRule5.CurrencyField = null;
		this.clDistRule5.CurrencyFixed = null;
		this.clDistRule5.CurrencyFixedRate = 0.0;
		this.clDistRule5.CurrencyRateField = null;
		this.clDistRule5.FieldName = "OcrCode5";
		this.clDistRule5.Name = "clDistRule5";
		this.clDistRule5.OptionsColumn.AllowEdit = false;
		this.clDistRule5.PrimaryKey = false;
		this.clDistRule5.UnitField = null;
		this.clDistRule5.Visible = true;
		this.clDistRule5.VisibleIndex = 17;
		this.repOcr5.AutoHeight = false;
		this.repOcr5.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.repOcr5.Name = "repOcr5";
		this.repOcr5.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repOcr5_ButtonClick);
		this.clProject.Caption = "Project";
		this.clProject.CurrencyDateField = null;
		this.clProject.CurrencyField = null;
		this.clProject.CurrencyFixed = null;
		this.clProject.CurrencyFixedRate = 0.0;
		this.clProject.CurrencyRateField = null;
		this.clProject.FieldName = "Project";
		this.clProject.Name = "clProject";
		this.clProject.OptionsColumn.AllowEdit = false;
		this.clProject.OptionsColumn.ReadOnly = true;
		this.clProject.PrimaryKey = false;
		this.clProject.UnitField = null;
		this.clProject.Visible = true;
		this.clProject.VisibleIndex = 18;
		this.clFixedBatch.Caption = "Fixed Batch";
		this.clFixedBatch.ColumnEdit = this.repFixBatch;
		this.clFixedBatch.CurrencyDateField = null;
		this.clFixedBatch.CurrencyField = null;
		this.clFixedBatch.CurrencyFixed = null;
		this.clFixedBatch.CurrencyFixedRate = 0.0;
		this.clFixedBatch.CurrencyRateField = null;
		this.clFixedBatch.FieldName = "BatchSOFixed";
		this.clFixedBatch.Name = "clFixedBatch";
		this.clFixedBatch.OptionsColumn.AllowEdit = false;
		this.clFixedBatch.OptionsColumn.ReadOnly = true;
		this.clFixedBatch.PrimaryKey = false;
		this.clFixedBatch.UnitField = null;
		this.clFixedBatch.Visible = true;
		this.clFixedBatch.VisibleIndex = 19;
		this.repFixBatch.AllowFocused = false;
		this.repFixBatch.AutoHeight = false;
		this.repFixBatch.Name = "repFixBatch";
		this.repFixBatch.ValueChecked = "Y";
		this.repFixBatch.ValueUnchecked = "N";
		this.clStatus.Caption = "Status";
		this.clStatus.ColumnEdit = this.rep_Status;
		this.clStatus.CurrencyDateField = null;
		this.clStatus.CurrencyField = null;
		this.clStatus.CurrencyFixed = null;
		this.clStatus.CurrencyFixedRate = 0.0;
		this.clStatus.CurrencyRateField = null;
		this.clStatus.FieldName = "Status";
		this.clStatus.Name = "clStatus";
		this.clStatus.OptionsColumn.AllowEdit = false;
		this.clStatus.OptionsColumn.ReadOnly = true;
		this.clStatus.PrimaryKey = false;
		this.clStatus.UnitField = null;
		this.rep_Status.AutoHeight = false;
		this.rep_Status.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Status.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ID", "ID", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rep_Status.Name = "rep_Status";
		this.clRemark.Caption = "Remark";
		this.clRemark.ColumnEdit = this.repositoryItemMemoExEdit1;
		this.clRemark.CurrencyDateField = null;
		this.clRemark.CurrencyField = null;
		this.clRemark.CurrencyFixed = null;
		this.clRemark.CurrencyFixedRate = 0.0;
		this.clRemark.CurrencyRateField = null;
		this.clRemark.FieldName = "Remark";
		this.clRemark.Name = "clRemark";
		this.clRemark.PrimaryKey = false;
		this.clRemark.UnitField = null;
		this.clRemark.Visible = true;
		this.clRemark.VisibleIndex = 20;
		this.repositoryItemMemoExEdit1.AutoHeight = false;
		this.repositoryItemMemoExEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemMemoExEdit1.Name = "repositoryItemMemoExEdit1";
		this.repositoryItemMemoExEdit1.ShowIcon = false;
		this.FixBatch.Caption = "FixBatch";
		this.FixBatch.CurrencyDateField = null;
		this.FixBatch.CurrencyField = null;
		this.FixBatch.CurrencyFixed = null;
		this.FixBatch.CurrencyFixedRate = 0.0;
		this.FixBatch.CurrencyRateField = null;
		this.FixBatch.FieldName = "FixBatch";
		this.FixBatch.Name = "FixBatch";
		this.FixBatch.OptionsColumn.AllowEdit = false;
		this.FixBatch.PrimaryKey = false;
		this.FixBatch.UnitField = null;
		this.DocEntrySO.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.DocEntrySO.AppearanceCell.Options.UseFont = true;
		this.DocEntrySO.Caption = "SO No.";
		this.DocEntrySO.CurrencyDateField = null;
		this.DocEntrySO.CurrencyField = null;
		this.DocEntrySO.CurrencyFixed = null;
		this.DocEntrySO.CurrencyFixedRate = 0.0;
		this.DocEntrySO.CurrencyRateField = null;
		this.DocEntrySO.FieldName = "DocEntrySO";
		this.DocEntrySO.Name = "DocEntrySO";
		this.DocEntrySO.OptionsColumn.ReadOnly = true;
		this.DocEntrySO.PrimaryKey = false;
		this.DocEntrySO.UnitField = null;
		this.DocEntrySO.Visible = true;
		this.DocEntrySO.VisibleIndex = 2;
		this.LineNumSO.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.LineNumSO.AppearanceCell.Options.UseFont = true;
		this.LineNumSO.Caption = "SO Line";
		this.LineNumSO.CurrencyDateField = null;
		this.LineNumSO.CurrencyField = null;
		this.LineNumSO.CurrencyFixed = null;
		this.LineNumSO.CurrencyFixedRate = 0.0;
		this.LineNumSO.CurrencyRateField = null;
		this.LineNumSO.FieldName = "LineNumSO";
		this.LineNumSO.Name = "LineNumSO";
		this.LineNumSO.OptionsColumn.ReadOnly = true;
		this.LineNumSO.PrimaryKey = false;
		this.LineNumSO.UnitField = null;
		this.LineNumSO.Visible = true;
		this.LineNumSO.VisibleIndex = 3;
		this.repositoryItemMemoEdit1.Name = "repositoryItemMemoEdit1";
		this.gridView5.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gridView5.ColumnKey = null;
		this.gridView5.DisableFormSetting = false;
		this.gridView5.DisplayException = false;
		this.gridView5.EditColumn = null;
		this.gridView5.FieldCurrency = null;
		this.gridView5.FieldRate = null;
		this.gridView5.FormId = null;
		this.gridView5.GridControl = this.grdMO;
		this.gridView5.IgnoreFilter = false;
		this.gridView5.IndexEditRow = -1;
		this.gridView5.IndicatorWidth = 35;
		this.gridView5.IsApzTable = false;
		this.gridView5.IsInsertNewRow = false;
		this.gridView5.IsLoadingData = false;
		this.gridView5.IsMappingContextAction = false;
		this.gridView5.ItemId = null;
		this.gridView5.LstColumnAllowBatch = null;
		this.gridView5.LstColumnNotAllowBatch = null;
		this.gridView5.LstColumnNotEditable = null;
		this.gridView5.LstColumNotSetting = null;
		this.gridView5.Name = "gridView5";
		this.gridView5.UseBatchMod = false;
		this.xtratabDoc.Dock = System.Windows.Forms.DockStyle.Fill;
		this.xtratabDoc.IsSearchControl = false;
		this.xtratabDoc.Location = new System.Drawing.Point(0, 0);
		this.xtratabDoc.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.xtratabDoc.Name = "xtratabDoc";
		this.xtratabDoc.SelectedTabPage = this.tab_Routing;
		this.xtratabDoc.Size = new System.Drawing.Size(740, 222);
		this.xtratabDoc.TabIndex = 1;
		this.xtratabDoc.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[3] { this.tab_Routing, this.tab_ReceiptFromProduction, this.tab_Attachment });
		this.xtratabDoc.SelectedPageChanged += new DevExpress.XtraTab.TabPageChangedEventHandler(this.xtraTabControl2_SelectedPageChanged);
		this.xtratabDoc.Click += new System.EventHandler(this.xtratabDoc_Click);
		this.tab_Routing.Controls.Add(this.grdRouting);
		this.tab_Routing.EditControl = null;
		this.tab_Routing.EditValue = "Routing";
		this.tab_Routing.FieldName = null;
		this.tab_Routing.IsSearchControl = false;
		this.tab_Routing.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.tab_Routing.Name = "tab_Routing";
		this.tab_Routing.ReadOnly = false;
		this.tab_Routing.SettingInVisible = false;
		this.tab_Routing.SettingReadonly = false;
		this.tab_Routing.Size = new System.Drawing.Size(738, 197);
		this.tab_Routing.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Routing.Text = "Routing";
		this.grdRouting.AllowPasteExcel = true;
		this.grdRouting.ClipboardData = null;
		this.grdRouting.ContextMenuStrip = this.contextRouting;
		this.grdRouting.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grdRouting.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grdRouting.Location = new System.Drawing.Point(0, 0);
		this.grdRouting.MainView = this.gvRouting;
		this.grdRouting.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grdRouting.Name = "grdRouting";
		this.grdRouting.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[12]
		{
			this.repSupCode, this.repOpCode, this.repWorkCenter, this.re_ControlKey, this.repPRT, this.repPreOp, this.repNextOp, this.repTime, this.reitemcode, this.repUOMRouting,
			this.repWhse, this.repositoryItemMemoExEdit2
		});
		this.grdRouting.Size = new System.Drawing.Size(738, 197);
		this.grdRouting.TabIndex = 1;
		this.grdRouting.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[2] { this.gvRouting, this.gridView4 });
		this.contextRouting.ImageScalingSize = new System.Drawing.Size(20, 20);
		this.contextRouting.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.deleteRowToolStripMenuItem1, this.insertRowToolStripMenuItem });
		this.contextRouting.Name = "contextRouting";
		this.contextRouting.Size = new System.Drawing.Size(134, 48);
		this.deleteRowToolStripMenuItem1.Name = "deleteRowToolStripMenuItem1";
		this.deleteRowToolStripMenuItem1.Size = new System.Drawing.Size(133, 22);
		this.deleteRowToolStripMenuItem1.Text = "Delete Row";
		this.deleteRowToolStripMenuItem1.Click += new System.EventHandler(this.deleteRowToolStripMenuItem1_Click);
		this.insertRowToolStripMenuItem.Name = "insertRowToolStripMenuItem";
		this.insertRowToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
		this.insertRowToolStripMenuItem.Text = "Insert Row";
		this.insertRowToolStripMenuItem.Click += new System.EventHandler(this.insertRowToolStripMenuItem_Click);
		this.gvRouting.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvRouting.ColumnKey = null;
		this.gvRouting.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[26]
		{
			this.MONo, this.colUoMTime, this.columBatchIssue, this.clOpCode, this.clSupOpCode, this.clWorkCenter, this.clCtrKey, this.clBatchQuantity, this.clBaseQty, this.clDescription,
			this.colRemark, this.columUOm, this.clSetUpTime, this.clLaborTime, this.clMachineTime, this.clPRT, this.clBOM, this.clPrOp, this.clNextOp, this.LineNum,
			this.UoMSetup, this.UoMLabor, this.UoMMachine, this.PrtCode, this.ItemCode, this.Whse
		});
		this.gvRouting.DisableFormSetting = false;
		this.gvRouting.DisplayException = false;
		this.gvRouting.EditColumn = null;
		this.gvRouting.FieldCurrency = null;
		this.gvRouting.FieldRate = null;
		this.gvRouting.FormId = null;
		this.gvRouting.GridControl = this.grdRouting;
		this.gvRouting.IgnoreFilter = false;
		this.gvRouting.IndexEditRow = -1;
		this.gvRouting.IndicatorWidth = 40;
		this.gvRouting.IsApzTable = false;
		this.gvRouting.IsInsertNewRow = false;
		this.gvRouting.IsLoadingData = false;
		this.gvRouting.IsMappingContextAction = false;
		this.gvRouting.ItemId = null;
		this.gvRouting.LstColumnAllowBatch = null;
		this.gvRouting.LstColumnNotAllowBatch = null;
		this.gvRouting.LstColumnNotEditable = null;
		this.gvRouting.LstColumNotSetting = null;
		this.gvRouting.Name = "gvRouting";
		this.gvRouting.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.True;
		this.gvRouting.OptionsBehavior.AllowDeleteRows = DevExpress.Utils.DefaultBoolean.True;
		this.gvRouting.OptionsBehavior.AutoExpandAllGroups = true;
		this.gvRouting.OptionsBehavior.KeepGroupExpandedOnSorting = false;
		this.gvRouting.OptionsCustomization.AllowGroup = false;
		this.gvRouting.OptionsCustomization.AllowSort = false;
		this.gvRouting.OptionsView.ColumnAutoWidth = false;
		this.gvRouting.OptionsView.ShowGroupPanel = false;
		this.gvRouting.UseBatchMod = false;
		this.gvRouting.RowCellClick += new DevExpress.XtraGrid.Views.Grid.RowCellClickEventHandler(this.gvRouting_RowCellClick);
		this.gvRouting.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gvRouting_CustomDrawRowIndicator);
		this.gvRouting.CustomDrawCell += new DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventHandler(this.gvRouting_CustomDrawCell);
		this.gvRouting.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gvRouting_CustomRowCellEdit);
		this.gvRouting.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gvRouting_ShowingEditor);
		this.gvRouting.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gvRouting_FocusedRowObjectChanged);
		this.gvRouting.FocusedColumnChanged += new DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventHandler(this.gvRouting_FocusedColumnChanged);
		this.gvRouting.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvRouting_CellValueChanged);
		this.gvRouting.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvRouting_CellValueChanging);
		this.gvRouting.MouseEnter += new System.EventHandler(this.gvRouting_MouseEnter);
		this.MONo.Caption = "MONo";
		this.MONo.CurrencyDateField = null;
		this.MONo.CurrencyField = null;
		this.MONo.CurrencyFixed = null;
		this.MONo.CurrencyFixedRate = 0.0;
		this.MONo.CurrencyRateField = null;
		this.MONo.FieldName = "MONo";
		this.MONo.Name = "MONo";
		this.MONo.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.MONo.PrimaryKey = false;
		this.MONo.UnitField = null;
		this.colUoMTime.Caption = "UoMTime";
		this.colUoMTime.CurrencyDateField = null;
		this.colUoMTime.CurrencyField = null;
		this.colUoMTime.CurrencyFixed = null;
		this.colUoMTime.CurrencyFixedRate = 0.0;
		this.colUoMTime.CurrencyRateField = null;
		this.colUoMTime.FieldName = "UoMTime";
		this.colUoMTime.Name = "colUoMTime";
		this.colUoMTime.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.colUoMTime.PrimaryKey = false;
		this.colUoMTime.UnitField = null;
		this.columBatchIssue.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.columBatchIssue.AppearanceCell.Options.UseFont = true;
		this.columBatchIssue.Caption = "Batch Issue";
		this.columBatchIssue.CurrencyDateField = null;
		this.columBatchIssue.CurrencyField = null;
		this.columBatchIssue.CurrencyFixed = null;
		this.columBatchIssue.CurrencyFixedRate = 0.0;
		this.columBatchIssue.CurrencyRateField = null;
		this.columBatchIssue.FieldName = "BatchIssue";
		this.columBatchIssue.Name = "columBatchIssue";
		this.columBatchIssue.OptionsColumn.AllowGroup = DevExpress.Utils.DefaultBoolean.True;
		this.columBatchIssue.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.columBatchIssue.OptionsFilter.AllowAutoFilter = false;
		this.columBatchIssue.OptionsFilter.AllowFilter = false;
		this.columBatchIssue.OptionsFilter.FilterBySortField = DevExpress.Utils.DefaultBoolean.False;
		this.columBatchIssue.PrimaryKey = false;
		this.columBatchIssue.UnitField = null;
		this.columBatchIssue.Visible = true;
		this.columBatchIssue.VisibleIndex = 0;
		this.columBatchIssue.Width = 115;
		this.clOpCode.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.clOpCode.AppearanceCell.Options.UseFont = true;
		this.clOpCode.Caption = "Op. Code";
		this.clOpCode.ColumnEdit = this.repOpCode;
		this.clOpCode.CurrencyDateField = null;
		this.clOpCode.CurrencyField = null;
		this.clOpCode.CurrencyFixed = null;
		this.clOpCode.CurrencyFixedRate = 0.0;
		this.clOpCode.CurrencyRateField = null;
		this.clOpCode.FieldName = "OpCode";
		this.clOpCode.Name = "clOpCode";
		this.clOpCode.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.clOpCode.OptionsColumn.ReadOnly = true;
		this.clOpCode.PrimaryKey = false;
		this.clOpCode.UnitField = null;
		this.clOpCode.Visible = true;
		this.clOpCode.VisibleIndex = 1;
		this.clOpCode.Width = 59;
		this.repOpCode.AutoHeight = false;
		this.repOpCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, false, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.repOpCode.Name = "repOpCode";
		this.repOpCode.ReadOnly = true;
		this.repOpCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repOpCode_ButtonClick);
		this.clSupOpCode.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.clSupOpCode.AppearanceCell.Options.UseFont = true;
		this.clSupOpCode.Caption = "Sub Op. Code";
		this.clSupOpCode.ColumnEdit = this.repSupCode;
		this.clSupOpCode.CurrencyDateField = null;
		this.clSupOpCode.CurrencyField = null;
		this.clSupOpCode.CurrencyFixed = null;
		this.clSupOpCode.CurrencyFixedRate = 0.0;
		this.clSupOpCode.CurrencyRateField = null;
		this.clSupOpCode.FieldName = "SubOpCode";
		this.clSupOpCode.Name = "clSupOpCode";
		this.clSupOpCode.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.clSupOpCode.OptionsColumn.ReadOnly = true;
		this.clSupOpCode.PrimaryKey = false;
		this.clSupOpCode.UnitField = null;
		this.clSupOpCode.Visible = true;
		this.clSupOpCode.VisibleIndex = 17;
		this.clSupOpCode.Width = 101;
		this.repSupCode.AutoHeight = false;
		this.repSupCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, false, false, imageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.repSupCode.Name = "repSupCode";
		this.repSupCode.ReadOnly = true;
		this.repSupCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repSupCode_ButtonClick);
		this.clWorkCenter.Caption = "Work Center";
		this.clWorkCenter.ColumnEdit = this.repWorkCenter;
		this.clWorkCenter.CurrencyDateField = null;
		this.clWorkCenter.CurrencyField = null;
		this.clWorkCenter.CurrencyFixed = null;
		this.clWorkCenter.CurrencyFixedRate = 0.0;
		this.clWorkCenter.CurrencyRateField = null;
		this.clWorkCenter.FieldName = "ResCode";
		this.clWorkCenter.Name = "clWorkCenter";
		this.clWorkCenter.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.clWorkCenter.OptionsColumn.ReadOnly = true;
		this.clWorkCenter.PrimaryKey = false;
		this.clWorkCenter.UnitField = null;
		this.clWorkCenter.Visible = true;
		this.clWorkCenter.VisibleIndex = 5;
		this.clWorkCenter.Width = 61;
		this.repWorkCenter.AutoHeight = false;
		this.repWorkCenter.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, false, false, imageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.repWorkCenter.Name = "repWorkCenter";
		this.repWorkCenter.ReadOnly = true;
		this.repWorkCenter.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repWorkCenter_ButtonClick);
		this.clCtrKey.Caption = "Ctrl Key";
		this.clCtrKey.ColumnEdit = this.re_ControlKey;
		this.clCtrKey.CurrencyDateField = null;
		this.clCtrKey.CurrencyField = null;
		this.clCtrKey.CurrencyFixed = null;
		this.clCtrKey.CurrencyFixedRate = 0.0;
		this.clCtrKey.CurrencyRateField = null;
		this.clCtrKey.FieldName = "KeyCode";
		this.clCtrKey.Name = "clCtrKey";
		this.clCtrKey.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.clCtrKey.PrimaryKey = false;
		this.clCtrKey.UnitField = null;
		this.clCtrKey.Visible = true;
		this.clCtrKey.VisibleIndex = 6;
		this.re_ControlKey.AutoHeight = false;
		this.re_ControlKey.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.re_ControlKey.Name = "re_ControlKey";
		this.re_ControlKey.ReadOnly = true;
		this.re_ControlKey.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.re_ControlKey_ButtonClick);
		this.clBatchQuantity.Caption = "Batch Issue Part";
		this.clBatchQuantity.CurrencyDateField = null;
		this.clBatchQuantity.CurrencyField = null;
		this.clBatchQuantity.CurrencyFixed = null;
		this.clBatchQuantity.CurrencyFixedRate = 0.0;
		this.clBatchQuantity.CurrencyRateField = null;
		this.clBatchQuantity.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.clBatchQuantity.FieldName = "BatchQuantity";
		this.clBatchQuantity.Name = "clBatchQuantity";
		this.clBatchQuantity.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.clBatchQuantity.PrimaryKey = false;
		this.clBatchQuantity.UnitField = null;
		this.clBatchQuantity.Visible = true;
		this.clBatchQuantity.VisibleIndex = 7;
		this.clBatchQuantity.Width = 48;
		this.clBaseQty.Caption = "Base Qty";
		this.clBaseQty.CurrencyDateField = null;
		this.clBaseQty.CurrencyField = null;
		this.clBaseQty.CurrencyFixed = null;
		this.clBaseQty.CurrencyFixedRate = 0.0;
		this.clBaseQty.CurrencyRateField = null;
		this.clBaseQty.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.clBaseQty.FieldName = "Quantity";
		this.clBaseQty.Name = "clBaseQty";
		this.clBaseQty.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.clBaseQty.PrimaryKey = false;
		this.clBaseQty.UnitField = null;
		this.clBaseQty.Width = 92;
		this.clDescription.Caption = "Description";
		this.clDescription.CurrencyDateField = null;
		this.clDescription.CurrencyField = null;
		this.clDescription.CurrencyFixed = null;
		this.clDescription.CurrencyFixedRate = 0.0;
		this.clDescription.CurrencyRateField = null;
		this.clDescription.FieldName = "OpName";
		this.clDescription.Name = "clDescription";
		this.clDescription.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.clDescription.PrimaryKey = false;
		this.clDescription.UnitField = null;
		this.clDescription.Visible = true;
		this.clDescription.VisibleIndex = 2;
		this.clDescription.Width = 83;
		this.colRemark.Caption = "Remark";
		this.colRemark.ColumnEdit = this.repositoryItemMemoExEdit2;
		this.colRemark.CurrencyDateField = null;
		this.colRemark.CurrencyField = null;
		this.colRemark.CurrencyFixed = null;
		this.colRemark.CurrencyFixedRate = 0.0;
		this.colRemark.CurrencyRateField = null;
		this.colRemark.FieldName = "Comments";
		this.colRemark.Name = "colRemark";
		this.colRemark.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.colRemark.PrimaryKey = false;
		this.colRemark.UnitField = null;
		this.colRemark.Visible = true;
		this.colRemark.VisibleIndex = 16;
		this.colRemark.Width = 136;
		this.repositoryItemMemoExEdit2.AutoHeight = false;
		this.repositoryItemMemoExEdit2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemMemoExEdit2.Name = "repositoryItemMemoExEdit2";
		this.repositoryItemMemoExEdit2.ShowIcon = false;
		this.columUOm.Caption = "UoM";
		this.columUOm.ColumnEdit = this.repUOMRouting;
		this.columUOm.CurrencyDateField = null;
		this.columUOm.CurrencyField = null;
		this.columUOm.CurrencyFixed = null;
		this.columUOm.CurrencyFixedRate = 0.0;
		this.columUOm.CurrencyRateField = null;
		this.columUOm.FieldName = "UoM";
		this.columUOm.Name = "columUOm";
		this.columUOm.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.columUOm.PrimaryKey = false;
		this.columUOm.UnitField = null;
		this.columUOm.Visible = true;
		this.columUOm.VisibleIndex = 8;
		this.columUOm.Width = 49;
		this.repUOMRouting.AutoHeight = false;
		this.repUOMRouting.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repUOMRouting.DisplayMember = "Name";
		this.repUOMRouting.Name = "repUOMRouting";
		this.repUOMRouting.ValueMember = "ID";
		this.clSetUpTime.Caption = "Setup Time";
		this.clSetUpTime.ColumnEdit = this.repTime;
		this.clSetUpTime.CurrencyDateField = null;
		this.clSetUpTime.CurrencyField = null;
		this.clSetUpTime.CurrencyFixed = null;
		this.clSetUpTime.CurrencyFixedRate = 0.0;
		this.clSetUpTime.CurrencyRateField = null;
		this.clSetUpTime.FieldName = "SetupTime";
		this.clSetUpTime.Name = "clSetUpTime";
		this.clSetUpTime.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.clSetUpTime.PrimaryKey = false;
		this.clSetUpTime.UnitField = null;
		this.clSetUpTime.Visible = true;
		this.clSetUpTime.VisibleIndex = 9;
		this.clSetUpTime.Width = 70;
		this.repTime.AutoHeight = false;
		this.repTime.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repTime.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.repTime.Name = "repTime";
		this.clLaborTime.Caption = "Labor Time";
		this.clLaborTime.ColumnEdit = this.repTime;
		this.clLaborTime.CurrencyDateField = null;
		this.clLaborTime.CurrencyField = null;
		this.clLaborTime.CurrencyFixed = null;
		this.clLaborTime.CurrencyFixedRate = 0.0;
		this.clLaborTime.CurrencyRateField = null;
		this.clLaborTime.FieldName = "LaborTime";
		this.clLaborTime.Name = "clLaborTime";
		this.clLaborTime.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.clLaborTime.PrimaryKey = false;
		this.clLaborTime.UnitField = null;
		this.clLaborTime.Visible = true;
		this.clLaborTime.VisibleIndex = 10;
		this.clMachineTime.Caption = "Machine Time";
		this.clMachineTime.ColumnEdit = this.repTime;
		this.clMachineTime.CurrencyDateField = null;
		this.clMachineTime.CurrencyField = null;
		this.clMachineTime.CurrencyFixed = null;
		this.clMachineTime.CurrencyFixedRate = 0.0;
		this.clMachineTime.CurrencyRateField = null;
		this.clMachineTime.FieldName = "MachineTime";
		this.clMachineTime.Name = "clMachineTime";
		this.clMachineTime.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.clMachineTime.PrimaryKey = false;
		this.clMachineTime.UnitField = null;
		this.clMachineTime.Visible = true;
		this.clMachineTime.VisibleIndex = 11;
		this.clMachineTime.Width = 54;
		this.clPRT.Caption = "PRT";
		this.clPRT.ColumnEdit = this.repPRT;
		this.clPRT.CurrencyDateField = null;
		this.clPRT.CurrencyField = null;
		this.clPRT.CurrencyFixed = null;
		this.clPRT.CurrencyFixedRate = 0.0;
		this.clPRT.CurrencyRateField = null;
		this.clPRT.FieldName = "PRT";
		this.clPRT.Name = "clPRT";
		this.clPRT.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.clPRT.PrimaryKey = false;
		this.clPRT.UnitField = null;
		this.clPRT.Visible = true;
		this.clPRT.VisibleIndex = 12;
		this.clPRT.Width = 34;
		this.repPRT.AutoHeight = false;
		this.repPRT.Name = "repPRT";
		this.repPRT.ValueChecked = "Y";
		this.repPRT.ValueUnchecked = "N";
		this.clBOM.Caption = "BOM";
		this.clBOM.CurrencyDateField = null;
		this.clBOM.CurrencyField = null;
		this.clBOM.CurrencyFixed = null;
		this.clBOM.CurrencyFixedRate = 0.0;
		this.clBOM.CurrencyRateField = null;
		this.clBOM.FieldName = "BOMCode";
		this.clBOM.Name = "clBOM";
		this.clBOM.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.clBOM.OptionsColumn.ReadOnly = true;
		this.clBOM.PrimaryKey = false;
		this.clBOM.UnitField = null;
		this.clBOM.Visible = true;
		this.clBOM.VisibleIndex = 13;
		this.clBOM.Width = 53;
		this.clPrOp.Caption = "Pre. Op";
		this.clPrOp.ColumnEdit = this.repPreOp;
		this.clPrOp.CurrencyDateField = null;
		this.clPrOp.CurrencyField = null;
		this.clPrOp.CurrencyFixed = null;
		this.clPrOp.CurrencyFixedRate = 0.0;
		this.clPrOp.CurrencyRateField = null;
		this.clPrOp.FieldName = "PrevOp";
		this.clPrOp.Name = "clPrOp";
		this.clPrOp.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.clPrOp.OptionsColumn.ReadOnly = true;
		this.clPrOp.PrimaryKey = false;
		this.clPrOp.UnitField = null;
		this.clPrOp.Visible = true;
		this.clPrOp.VisibleIndex = 14;
		this.clPrOp.Width = 54;
		this.repPreOp.AutoHeight = false;
		this.repPreOp.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, false, false, imageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance4, appearanceHovered4, appearancePressed4, appearanceDisabled4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.repPreOp.Name = "repPreOp";
		this.repPreOp.ReadOnly = true;
		this.repPreOp.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repPreOp_ButtonClick);
		this.clNextOp.Caption = "Next Op.";
		this.clNextOp.ColumnEdit = this.repNextOp;
		this.clNextOp.CurrencyDateField = null;
		this.clNextOp.CurrencyField = null;
		this.clNextOp.CurrencyFixed = null;
		this.clNextOp.CurrencyFixedRate = 0.0;
		this.clNextOp.CurrencyRateField = null;
		this.clNextOp.FieldName = "NextOp";
		this.clNextOp.Name = "clNextOp";
		this.clNextOp.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.clNextOp.OptionsColumn.ReadOnly = true;
		this.clNextOp.PrimaryKey = false;
		this.clNextOp.UnitField = null;
		this.clNextOp.Visible = true;
		this.clNextOp.VisibleIndex = 15;
		this.clNextOp.Width = 43;
		this.repNextOp.AutoHeight = false;
		this.repNextOp.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, false, false, imageOptions5, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance5, appearanceHovered5, appearancePressed5, appearanceDisabled5, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.repNextOp.Name = "repNextOp";
		this.repNextOp.ReadOnly = true;
		this.repNextOp.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repNextOp_ButtonClick);
		this.LineNum.Caption = "LineNum";
		this.LineNum.CurrencyDateField = null;
		this.LineNum.CurrencyField = null;
		this.LineNum.CurrencyFixed = null;
		this.LineNum.CurrencyFixedRate = 0.0;
		this.LineNum.CurrencyRateField = null;
		this.LineNum.FieldName = "LineNum";
		this.LineNum.Name = "LineNum";
		this.LineNum.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.LineNum.PrimaryKey = false;
		this.LineNum.UnitField = null;
		this.UoMSetup.Caption = "UoMSetup";
		this.UoMSetup.CurrencyDateField = null;
		this.UoMSetup.CurrencyField = null;
		this.UoMSetup.CurrencyFixed = null;
		this.UoMSetup.CurrencyFixedRate = 0.0;
		this.UoMSetup.CurrencyRateField = null;
		this.UoMSetup.FieldName = "UoMSetup";
		this.UoMSetup.Name = "UoMSetup";
		this.UoMSetup.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.UoMSetup.PrimaryKey = false;
		this.UoMSetup.UnitField = null;
		this.UoMLabor.Caption = "UoMLabor";
		this.UoMLabor.CurrencyDateField = null;
		this.UoMLabor.CurrencyField = null;
		this.UoMLabor.CurrencyFixed = null;
		this.UoMLabor.CurrencyFixedRate = 0.0;
		this.UoMLabor.CurrencyRateField = null;
		this.UoMLabor.FieldName = "UoMLabor";
		this.UoMLabor.Name = "UoMLabor";
		this.UoMLabor.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.UoMLabor.PrimaryKey = false;
		this.UoMLabor.UnitField = null;
		this.UoMMachine.Caption = "UoMMachine";
		this.UoMMachine.CurrencyDateField = null;
		this.UoMMachine.CurrencyField = null;
		this.UoMMachine.CurrencyFixed = null;
		this.UoMMachine.CurrencyFixedRate = 0.0;
		this.UoMMachine.CurrencyRateField = null;
		this.UoMMachine.FieldName = "UoMMachine";
		this.UoMMachine.Name = "UoMMachine";
		this.UoMMachine.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.UoMMachine.PrimaryKey = false;
		this.UoMMachine.UnitField = null;
		this.PrtCode.Caption = "PrtCode";
		this.PrtCode.CurrencyDateField = null;
		this.PrtCode.CurrencyField = null;
		this.PrtCode.CurrencyFixed = null;
		this.PrtCode.CurrencyFixedRate = 0.0;
		this.PrtCode.CurrencyRateField = null;
		this.PrtCode.FieldName = "PrtCode";
		this.PrtCode.Name = "PrtCode";
		this.PrtCode.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.PrtCode.PrimaryKey = false;
		this.PrtCode.UnitField = null;
		this.ItemCode.Caption = "ItemCode";
		this.ItemCode.ColumnEdit = this.reitemcode;
		this.ItemCode.CurrencyDateField = null;
		this.ItemCode.CurrencyField = null;
		this.ItemCode.CurrencyFixed = null;
		this.ItemCode.CurrencyFixedRate = 0.0;
		this.ItemCode.CurrencyRateField = null;
		this.ItemCode.FieldName = "ItemCode";
		this.ItemCode.Name = "ItemCode";
		this.ItemCode.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.ItemCode.PrimaryKey = false;
		this.ItemCode.UnitField = null;
		this.ItemCode.Visible = true;
		this.ItemCode.VisibleIndex = 3;
		this.reitemcode.AutoHeight = false;
		this.reitemcode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.reitemcode.Name = "reitemcode";
		this.reitemcode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.reitemcode_ButtonClick);
		this.Whse.Caption = "Whse";
		this.Whse.ColumnEdit = this.repWhse;
		this.Whse.CurrencyDateField = null;
		this.Whse.CurrencyField = null;
		this.Whse.CurrencyFixed = null;
		this.Whse.CurrencyFixedRate = 0.0;
		this.Whse.CurrencyRateField = null;
		this.Whse.FieldName = "Whse";
		this.Whse.Name = "Whse";
		this.Whse.PrimaryKey = false;
		this.Whse.UnitField = null;
		this.Whse.Visible = true;
		this.Whse.VisibleIndex = 4;
		this.repWhse.AutoHeight = false;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.repWhse.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance6, appearanceHovered6, appearancePressed6, appearanceDisabled6, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.repWhse.Name = "repWhse";
		this.repWhse.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repWhse_ButtonClick);
		this.gridView4.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gridView4.ColumnKey = null;
		this.gridView4.DisableFormSetting = false;
		this.gridView4.DisplayException = false;
		this.gridView4.EditColumn = null;
		this.gridView4.FieldCurrency = null;
		this.gridView4.FieldRate = null;
		this.gridView4.FormId = null;
		this.gridView4.GridControl = this.grdRouting;
		this.gridView4.IgnoreFilter = false;
		this.gridView4.IndexEditRow = -1;
		this.gridView4.IndicatorWidth = 35;
		this.gridView4.IsApzTable = false;
		this.gridView4.IsInsertNewRow = false;
		this.gridView4.IsLoadingData = false;
		this.gridView4.IsMappingContextAction = false;
		this.gridView4.ItemId = null;
		this.gridView4.LstColumnAllowBatch = null;
		this.gridView4.LstColumnNotAllowBatch = null;
		this.gridView4.LstColumnNotEditable = null;
		this.gridView4.LstColumNotSetting = null;
		this.gridView4.Name = "gridView4";
		this.gridView4.UseBatchMod = false;
		this.tab_ReceiptFromProduction.Controls.Add(this.grdReceipt);
		this.tab_ReceiptFromProduction.EditControl = null;
		this.tab_ReceiptFromProduction.EditValue = "Receipt From Production";
		this.tab_ReceiptFromProduction.FieldName = null;
		this.tab_ReceiptFromProduction.IsSearchControl = false;
		this.tab_ReceiptFromProduction.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.tab_ReceiptFromProduction.Name = "tab_ReceiptFromProduction";
		this.tab_ReceiptFromProduction.ReadOnly = false;
		this.tab_ReceiptFromProduction.SettingInVisible = false;
		this.tab_ReceiptFromProduction.SettingReadonly = false;
		this.tab_ReceiptFromProduction.Size = new System.Drawing.Size(734, 194);
		this.tab_ReceiptFromProduction.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_ReceiptFromProduction.Text = "Receipt From Production";
		this.grdReceipt.AllowPasteExcel = false;
		this.grdReceipt.ClipboardData = null;
		this.grdReceipt.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grdReceipt.Location = new System.Drawing.Point(0, 0);
		this.grdReceipt.MainView = this.gvReceipt;
		this.grdReceipt.Name = "grdReceipt";
		this.grdReceipt.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[13]
		{
			this.repDelDate, this.rep, this.repBatComplete, this.repGetBatch, this.repUOMReceipt, this.repoWhse, this.repositoryItemMemoEdit2, this.repositoryItemMemoExEdit3, this.repositoryItemMemoExEdit4, this.repositoryItemMemoEdit3,
			this.repSaleorderNo, this.repSOLine, this.rep_WHSE
		});
		this.grdReceipt.Size = new System.Drawing.Size(734, 194);
		this.grdReceipt.TabIndex = 0;
		this.grdReceipt.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[2] { this.gvReceipt, this.gridView2 });
		this.gvReceipt.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvReceipt.ColumnKey = null;
		this.gvReceipt.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[41]
		{
			this.colMONo, this.colLineNum, this.clItemCode, this.clItemName, this.clOrderCode, this.clOderLine, this.clDelDate, this.clBatch, this.colWidth, this.colLength,
			this.colUOM, this.colQuantity, this.colWHse, this.colProject, this.colOperation, this.clDelTime, this.clBatchIssue, this.clBatchComplete, this.clThickPlan, this.WidthPlan,
			this.LengthPlan, this.ThickFinal, this.WidthFinal, this.LengthFinal, this.OcrCode, this.OcrCode2, this.OcrCode3, this.OcrCode4, this.OcrCode5, this.UoM,
			this.OpCode, this.Project, this.Comments, this.gridColumn23, this.gridColumn24, this.gridColumn25, this.gridColumn26, this.gridColumn27, this.gridColumn28, this.gridColumn29,
			this.gridColumn30
		});
		this.gvReceipt.DisableFormSetting = false;
		this.gvReceipt.DisplayException = false;
		this.gvReceipt.EditColumn = null;
		this.gvReceipt.FieldCurrency = null;
		this.gvReceipt.FieldRate = null;
		this.gvReceipt.FormId = null;
		this.gvReceipt.GridControl = this.grdReceipt;
		this.gvReceipt.IgnoreFilter = false;
		this.gvReceipt.IndexEditRow = -1;
		this.gvReceipt.IndicatorWidth = 40;
		this.gvReceipt.IsApzTable = false;
		this.gvReceipt.IsInsertNewRow = false;
		this.gvReceipt.IsLoadingData = false;
		this.gvReceipt.IsMappingContextAction = false;
		this.gvReceipt.ItemId = null;
		this.gvReceipt.LstColumnAllowBatch = null;
		this.gvReceipt.LstColumnNotAllowBatch = null;
		this.gvReceipt.LstColumnNotEditable = null;
		this.gvReceipt.LstColumNotSetting = null;
		this.gvReceipt.Name = "gvReceipt";
		this.gvReceipt.OptionsCustomization.AllowSort = false;
		this.gvReceipt.OptionsView.ColumnAutoWidth = false;
		this.gvReceipt.OptionsView.ShowGroupPanel = false;
		this.gvReceipt.UseBatchMod = false;
		this.gvReceipt.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gvReceipt_CustomDrawRowIndicator);
		this.gvReceipt.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gvReceipt_CustomRowCellEdit);
		this.gvReceipt.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvReceipt_CellValueChanged);
		this.gvReceipt.MouseUp += new System.Windows.Forms.MouseEventHandler(this.gvReceipt_MouseUp);
		this.colMONo.Caption = "MONo";
		this.colMONo.CurrencyDateField = null;
		this.colMONo.CurrencyField = null;
		this.colMONo.CurrencyFixed = null;
		this.colMONo.CurrencyFixedRate = 0.0;
		this.colMONo.CurrencyRateField = null;
		this.colMONo.FieldName = "MONo";
		this.colMONo.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.colMONo.Name = "colMONo";
		this.colMONo.PrimaryKey = false;
		this.colMONo.UnitField = null;
		this.colLineNum.Caption = "LineNum";
		this.colLineNum.CurrencyDateField = null;
		this.colLineNum.CurrencyField = null;
		this.colLineNum.CurrencyFixed = null;
		this.colLineNum.CurrencyFixedRate = 0.0;
		this.colLineNum.CurrencyRateField = null;
		this.colLineNum.FieldName = "LineNum";
		this.colLineNum.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.colLineNum.Name = "colLineNum";
		this.colLineNum.PrimaryKey = false;
		this.colLineNum.UnitField = null;
		this.clItemCode.Caption = "Item No.";
		this.clItemCode.CurrencyDateField = null;
		this.clItemCode.CurrencyField = null;
		this.clItemCode.CurrencyFixed = null;
		this.clItemCode.CurrencyFixedRate = 0.0;
		this.clItemCode.CurrencyRateField = null;
		this.clItemCode.FieldName = "ItemCode";
		this.clItemCode.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.clItemCode.Name = "clItemCode";
		this.clItemCode.PrimaryKey = false;
		this.clItemCode.UnitField = null;
		this.clItemCode.Visible = true;
		this.clItemCode.VisibleIndex = 2;
		this.clItemName.Caption = "Item Name";
		this.clItemName.CurrencyDateField = null;
		this.clItemName.CurrencyField = null;
		this.clItemName.CurrencyFixed = null;
		this.clItemName.CurrencyFixedRate = 0.0;
		this.clItemName.CurrencyRateField = null;
		this.clItemName.FieldName = "ItemName";
		this.clItemName.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.clItemName.Name = "clItemName";
		this.clItemName.PrimaryKey = false;
		this.clItemName.UnitField = null;
		this.clItemName.Width = 147;
		this.clOrderCode.Caption = "Order No.";
		this.clOrderCode.ColumnEdit = this.repSaleorderNo;
		this.clOrderCode.CurrencyDateField = null;
		this.clOrderCode.CurrencyField = null;
		this.clOrderCode.CurrencyFixed = null;
		this.clOrderCode.CurrencyFixedRate = 0.0;
		this.clOrderCode.CurrencyRateField = null;
		this.clOrderCode.FieldName = "SaleOrder";
		this.clOrderCode.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.clOrderCode.Name = "clOrderCode";
		this.clOrderCode.PrimaryKey = false;
		this.clOrderCode.UnitField = null;
		this.clOrderCode.Visible = true;
		this.clOrderCode.VisibleIndex = 0;
		this.clOrderCode.Width = 56;
		this.repSaleorderNo.AutoHeight = false;
		this.repSaleorderNo.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repSaleorderNo.Name = "repSaleorderNo";
		this.clOderLine.Caption = "Order Line";
		this.clOderLine.ColumnEdit = this.repSOLine;
		this.clOderLine.CurrencyDateField = null;
		this.clOderLine.CurrencyField = null;
		this.clOderLine.CurrencyFixed = null;
		this.clOderLine.CurrencyFixedRate = 0.0;
		this.clOderLine.CurrencyRateField = null;
		this.clOderLine.FieldName = "LineSale";
		this.clOderLine.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.clOderLine.Name = "clOderLine";
		this.clOderLine.PrimaryKey = false;
		this.clOderLine.UnitField = null;
		this.clOderLine.Visible = true;
		this.clOderLine.VisibleIndex = 1;
		this.clOderLine.Width = 47;
		this.repSOLine.AutoHeight = false;
		this.repSOLine.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repSOLine.Name = "repSOLine";
		this.clDelDate.Caption = "Del. Date";
		this.clDelDate.ColumnEdit = this.repDelDate;
		this.clDelDate.CurrencyDateField = null;
		this.clDelDate.CurrencyField = null;
		this.clDelDate.CurrencyFixed = null;
		this.clDelDate.CurrencyFixedRate = 0.0;
		this.clDelDate.CurrencyRateField = null;
		this.clDelDate.FieldName = "ShipDate";
		this.clDelDate.Name = "clDelDate";
		this.clDelDate.PrimaryKey = false;
		this.clDelDate.UnitField = null;
		this.clDelDate.Visible = true;
		this.clDelDate.VisibleIndex = 8;
		this.clDelDate.Width = 46;
		this.repDelDate.AutoHeight = false;
		this.repDelDate.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repDelDate.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repDelDate.Name = "repDelDate";
		this.clBatch.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.clBatch.AppearanceCell.Options.UseFont = true;
		this.clBatch.Caption = "Batch";
		this.clBatch.CurrencyDateField = null;
		this.clBatch.CurrencyField = null;
		this.clBatch.CurrencyFixed = null;
		this.clBatch.CurrencyFixedRate = 0.0;
		this.clBatch.CurrencyRateField = null;
		this.clBatch.FieldName = "BatchToIssue";
		this.clBatch.Name = "clBatch";
		this.clBatch.PrimaryKey = false;
		this.clBatch.UnitField = null;
		this.clBatch.Visible = true;
		this.clBatch.VisibleIndex = 3;
		this.clBatch.Width = 110;
		this.colWidth.Caption = "Width";
		this.colWidth.CurrencyDateField = null;
		this.colWidth.CurrencyField = null;
		this.colWidth.CurrencyFixed = null;
		this.colWidth.CurrencyFixedRate = 0.0;
		this.colWidth.CurrencyRateField = null;
		this.colWidth.FieldName = "Width";
		this.colWidth.Name = "colWidth";
		this.colWidth.PrimaryKey = false;
		this.colWidth.UnitField = null;
		this.colWidth.Visible = true;
		this.colWidth.VisibleIndex = 5;
		this.colWidth.Width = 48;
		this.colLength.Caption = "Length ";
		this.colLength.CurrencyDateField = null;
		this.colLength.CurrencyField = null;
		this.colLength.CurrencyFixed = null;
		this.colLength.CurrencyFixedRate = 0.0;
		this.colLength.CurrencyRateField = null;
		this.colLength.FieldName = "Length";
		this.colLength.Name = "colLength";
		this.colLength.PrimaryKey = false;
		this.colLength.UnitField = null;
		this.colLength.Visible = true;
		this.colLength.VisibleIndex = 6;
		this.colLength.Width = 42;
		this.colUOM.Caption = "UoM";
		this.colUOM.ColumnEdit = this.repUOMReceipt;
		this.colUOM.CurrencyDateField = null;
		this.colUOM.CurrencyField = null;
		this.colUOM.CurrencyFixed = null;
		this.colUOM.CurrencyFixedRate = 0.0;
		this.colUOM.CurrencyRateField = null;
		this.colUOM.FieldName = "UoM";
		this.colUOM.Name = "colUOM";
		this.colUOM.PrimaryKey = false;
		this.colUOM.UnitField = null;
		this.colUOM.Visible = true;
		this.colUOM.VisibleIndex = 7;
		this.colUOM.Width = 44;
		this.repUOMReceipt.AutoHeight = false;
		this.repUOMReceipt.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repUOMReceipt.DisplayMember = "Name";
		this.repUOMReceipt.Name = "repUOMReceipt";
		this.repUOMReceipt.ValueMember = "ID";
		this.colQuantity.Caption = "Quantity";
		this.colQuantity.CurrencyDateField = null;
		this.colQuantity.CurrencyField = null;
		this.colQuantity.CurrencyFixed = null;
		this.colQuantity.CurrencyFixedRate = 0.0;
		this.colQuantity.CurrencyRateField = null;
		this.colQuantity.FieldName = "Quantity";
		this.colQuantity.Name = "colQuantity";
		this.colQuantity.PrimaryKey = false;
		this.colQuantity.UnitField = null;
		this.colQuantity.Visible = true;
		this.colQuantity.VisibleIndex = 4;
		this.colQuantity.Width = 55;
		this.colWHse.Caption = "Whse";
		this.colWHse.ColumnEdit = this.rep_WHSE;
		this.colWHse.CurrencyDateField = null;
		this.colWHse.CurrencyField = null;
		this.colWHse.CurrencyFixed = null;
		this.colWHse.CurrencyFixedRate = 0.0;
		this.colWHse.CurrencyRateField = null;
		this.colWHse.FieldName = "Whse";
		this.colWHse.Name = "colWHse";
		this.colWHse.PrimaryKey = false;
		this.colWHse.UnitField = null;
		this.colWHse.Visible = true;
		this.colWHse.VisibleIndex = 11;
		this.colWHse.Width = 92;
		this.rep_WHSE.AutoHeight = false;
		editorButtonImageOptions2.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_WHSE.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance7, appearanceHovered7, appearancePressed7, appearanceDisabled7, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_WHSE.Name = "rep_WHSE";
		this.rep_WHSE.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_WHSE_ButtonClick);
		this.colProject.Caption = "Project";
		this.colProject.CurrencyDateField = null;
		this.colProject.CurrencyField = null;
		this.colProject.CurrencyFixed = null;
		this.colProject.CurrencyFixedRate = 0.0;
		this.colProject.CurrencyRateField = null;
		this.colProject.FieldName = "Project";
		this.colProject.Name = "colProject";
		this.colProject.PrimaryKey = false;
		this.colProject.UnitField = null;
		this.colProject.Width = 61;
		this.colOperation.Caption = "Operation";
		this.colOperation.CurrencyDateField = null;
		this.colOperation.CurrencyField = null;
		this.colOperation.CurrencyFixed = null;
		this.colOperation.CurrencyFixedRate = 0.0;
		this.colOperation.CurrencyRateField = null;
		this.colOperation.FieldName = "OpCode";
		this.colOperation.Name = "colOperation";
		this.colOperation.PrimaryKey = false;
		this.colOperation.UnitField = null;
		this.colOperation.Visible = true;
		this.colOperation.VisibleIndex = 9;
		this.colOperation.Width = 54;
		this.clDelTime.Caption = "DelTime";
		this.clDelTime.CurrencyDateField = null;
		this.clDelTime.CurrencyField = null;
		this.clDelTime.CurrencyFixed = null;
		this.clDelTime.CurrencyFixedRate = 0.0;
		this.clDelTime.CurrencyRateField = null;
		this.clDelTime.FieldName = "DelTime";
		this.clDelTime.Name = "clDelTime";
		this.clDelTime.PrimaryKey = false;
		this.clDelTime.UnitField = null;
		this.clBatchIssue.Caption = "BatchIssue";
		this.clBatchIssue.ColumnEdit = this.rep;
		this.clBatchIssue.CurrencyDateField = null;
		this.clBatchIssue.CurrencyField = null;
		this.clBatchIssue.CurrencyFixed = null;
		this.clBatchIssue.CurrencyFixedRate = 0.0;
		this.clBatchIssue.CurrencyRateField = null;
		this.clBatchIssue.FieldName = "BatchIssue";
		this.clBatchIssue.Name = "clBatchIssue";
		this.clBatchIssue.PrimaryKey = false;
		this.clBatchIssue.UnitField = null;
		this.rep.AutoHeight = false;
		this.rep.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep.Name = "rep";
		this.clBatchComplete.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.clBatchComplete.AppearanceCell.Options.UseFont = true;
		this.clBatchComplete.Caption = "BatchComplete";
		this.clBatchComplete.ColumnEdit = this.repGetBatch;
		this.clBatchComplete.CurrencyDateField = null;
		this.clBatchComplete.CurrencyField = null;
		this.clBatchComplete.CurrencyFixed = null;
		this.clBatchComplete.CurrencyFixedRate = 0.0;
		this.clBatchComplete.CurrencyRateField = null;
		this.clBatchComplete.FieldName = "BatchComplete";
		this.clBatchComplete.Name = "clBatchComplete";
		this.clBatchComplete.PrimaryKey = false;
		this.clBatchComplete.UnitField = null;
		this.clBatchComplete.Visible = true;
		this.clBatchComplete.VisibleIndex = 10;
		this.clBatchComplete.Width = 128;
		this.repGetBatch.AutoHeight = false;
		editorButtonImageOptions3.Image = Apzon.Forms.Properties.Resources.LeftShowFormArrow;
		this.repGetBatch.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Right, "", -1, true, true, false, editorButtonImageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance8, appearanceHovered8, appearancePressed8, appearanceDisabled8, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.repGetBatch.Name = "repGetBatch";
		this.repGetBatch.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repGetBatch_ButtonClick);
		this.clThickPlan.Caption = "ThickPlan";
		this.clThickPlan.CurrencyDateField = null;
		this.clThickPlan.CurrencyField = null;
		this.clThickPlan.CurrencyFixed = null;
		this.clThickPlan.CurrencyFixedRate = 0.0;
		this.clThickPlan.CurrencyRateField = null;
		this.clThickPlan.FieldName = "ThickPlan";
		this.clThickPlan.Name = "clThickPlan";
		this.clThickPlan.PrimaryKey = false;
		this.clThickPlan.UnitField = null;
		this.WidthPlan.Caption = "WidthPlan";
		this.WidthPlan.CurrencyDateField = null;
		this.WidthPlan.CurrencyField = null;
		this.WidthPlan.CurrencyFixed = null;
		this.WidthPlan.CurrencyFixedRate = 0.0;
		this.WidthPlan.CurrencyRateField = null;
		this.WidthPlan.FieldName = "WidthPlan";
		this.WidthPlan.Name = "WidthPlan";
		this.WidthPlan.PrimaryKey = false;
		this.WidthPlan.UnitField = null;
		this.LengthPlan.Caption = "LengthPlan";
		this.LengthPlan.CurrencyDateField = null;
		this.LengthPlan.CurrencyField = null;
		this.LengthPlan.CurrencyFixed = null;
		this.LengthPlan.CurrencyFixedRate = 0.0;
		this.LengthPlan.CurrencyRateField = null;
		this.LengthPlan.FieldName = "LengthPlan";
		this.LengthPlan.Name = "LengthPlan";
		this.LengthPlan.PrimaryKey = false;
		this.LengthPlan.UnitField = null;
		this.ThickFinal.Caption = "ThickFinal";
		this.ThickFinal.CurrencyDateField = null;
		this.ThickFinal.CurrencyField = null;
		this.ThickFinal.CurrencyFixed = null;
		this.ThickFinal.CurrencyFixedRate = 0.0;
		this.ThickFinal.CurrencyRateField = null;
		this.ThickFinal.FieldName = "ThickFinal";
		this.ThickFinal.Name = "ThickFinal";
		this.ThickFinal.PrimaryKey = false;
		this.ThickFinal.UnitField = null;
		this.WidthFinal.Caption = "WidthFinal";
		this.WidthFinal.CurrencyDateField = null;
		this.WidthFinal.CurrencyField = null;
		this.WidthFinal.CurrencyFixed = null;
		this.WidthFinal.CurrencyFixedRate = 0.0;
		this.WidthFinal.CurrencyRateField = null;
		this.WidthFinal.FieldName = "WidthFinal";
		this.WidthFinal.Name = "WidthFinal";
		this.WidthFinal.PrimaryKey = false;
		this.WidthFinal.UnitField = null;
		this.LengthFinal.Caption = "LengthFinal";
		this.LengthFinal.CurrencyDateField = null;
		this.LengthFinal.CurrencyField = null;
		this.LengthFinal.CurrencyFixed = null;
		this.LengthFinal.CurrencyFixedRate = 0.0;
		this.LengthFinal.CurrencyRateField = null;
		this.LengthFinal.FieldName = "LengthFinal";
		this.LengthFinal.Name = "LengthFinal";
		this.LengthFinal.PrimaryKey = false;
		this.LengthFinal.UnitField = null;
		this.OcrCode.Caption = "OcrCode";
		this.OcrCode.CurrencyDateField = null;
		this.OcrCode.CurrencyField = null;
		this.OcrCode.CurrencyFixed = null;
		this.OcrCode.CurrencyFixedRate = 0.0;
		this.OcrCode.CurrencyRateField = null;
		this.OcrCode.FieldName = "OcrCode";
		this.OcrCode.Name = "OcrCode";
		this.OcrCode.PrimaryKey = false;
		this.OcrCode.UnitField = null;
		this.OcrCode2.Caption = "OcrCode2";
		this.OcrCode2.CurrencyDateField = null;
		this.OcrCode2.CurrencyField = null;
		this.OcrCode2.CurrencyFixed = null;
		this.OcrCode2.CurrencyFixedRate = 0.0;
		this.OcrCode2.CurrencyRateField = null;
		this.OcrCode2.FieldName = "OcrCode2";
		this.OcrCode2.Name = "OcrCode2";
		this.OcrCode2.PrimaryKey = false;
		this.OcrCode2.UnitField = null;
		this.OcrCode3.Caption = "OcrCode3";
		this.OcrCode3.CurrencyDateField = null;
		this.OcrCode3.CurrencyField = null;
		this.OcrCode3.CurrencyFixed = null;
		this.OcrCode3.CurrencyFixedRate = 0.0;
		this.OcrCode3.CurrencyRateField = null;
		this.OcrCode3.FieldName = "OcrCode3";
		this.OcrCode3.Name = "OcrCode3";
		this.OcrCode3.PrimaryKey = false;
		this.OcrCode3.UnitField = null;
		this.OcrCode4.Caption = "OcrCode4";
		this.OcrCode4.CurrencyDateField = null;
		this.OcrCode4.CurrencyField = null;
		this.OcrCode4.CurrencyFixed = null;
		this.OcrCode4.CurrencyFixedRate = 0.0;
		this.OcrCode4.CurrencyRateField = null;
		this.OcrCode4.FieldName = "OcrCode4";
		this.OcrCode4.Name = "OcrCode4";
		this.OcrCode4.PrimaryKey = false;
		this.OcrCode4.UnitField = null;
		this.OcrCode5.Caption = "OcrCode5";
		this.OcrCode5.CurrencyDateField = null;
		this.OcrCode5.CurrencyField = null;
		this.OcrCode5.CurrencyFixed = null;
		this.OcrCode5.CurrencyFixedRate = 0.0;
		this.OcrCode5.CurrencyRateField = null;
		this.OcrCode5.FieldName = "OcrCode5";
		this.OcrCode5.Name = "OcrCode5";
		this.OcrCode5.PrimaryKey = false;
		this.OcrCode5.UnitField = null;
		this.UoM.Caption = "UoM";
		this.UoM.CurrencyDateField = null;
		this.UoM.CurrencyField = null;
		this.UoM.CurrencyFixed = null;
		this.UoM.CurrencyFixedRate = 0.0;
		this.UoM.CurrencyRateField = null;
		this.UoM.FieldName = "UoM";
		this.UoM.Name = "UoM";
		this.UoM.PrimaryKey = false;
		this.UoM.UnitField = null;
		this.OpCode.Caption = "OpCode";
		this.OpCode.CurrencyDateField = null;
		this.OpCode.CurrencyField = null;
		this.OpCode.CurrencyFixed = null;
		this.OpCode.CurrencyFixedRate = 0.0;
		this.OpCode.CurrencyRateField = null;
		this.OpCode.FieldName = "OpCode";
		this.OpCode.Name = "OpCode";
		this.OpCode.PrimaryKey = false;
		this.OpCode.UnitField = null;
		this.Project.Caption = "Project";
		this.Project.CurrencyDateField = null;
		this.Project.CurrencyField = null;
		this.Project.CurrencyFixed = null;
		this.Project.CurrencyFixedRate = 0.0;
		this.Project.CurrencyRateField = null;
		this.Project.FieldName = "Project";
		this.Project.Name = "Project";
		this.Project.PrimaryKey = false;
		this.Project.UnitField = null;
		this.Comments.Caption = "Comments";
		this.Comments.ColumnEdit = this.repositoryItemMemoExEdit4;
		this.Comments.CurrencyDateField = null;
		this.Comments.CurrencyField = null;
		this.Comments.CurrencyFixed = null;
		this.Comments.CurrencyFixedRate = 0.0;
		this.Comments.CurrencyRateField = null;
		this.Comments.FieldName = "Comments";
		this.Comments.Name = "Comments";
		this.Comments.PrimaryKey = false;
		this.Comments.UnitField = null;
		this.Comments.Visible = true;
		this.Comments.VisibleIndex = 12;
		this.repositoryItemMemoExEdit4.AutoHeight = false;
		this.repositoryItemMemoExEdit4.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemMemoExEdit4.Name = "repositoryItemMemoExEdit4";
		this.repositoryItemMemoExEdit4.ShowIcon = false;
		this.gridColumn23.Caption = "Area";
		this.gridColumn23.CurrencyDateField = null;
		this.gridColumn23.CurrencyField = null;
		this.gridColumn23.CurrencyFixed = null;
		this.gridColumn23.CurrencyFixedRate = 0.0;
		this.gridColumn23.CurrencyRateField = null;
		this.gridColumn23.FieldName = "Area";
		this.gridColumn23.Name = "gridColumn23";
		this.gridColumn23.PrimaryKey = false;
		this.gridColumn23.UnitField = null;
		this.gridColumn24.Caption = "Bỏ đầu đuôi ";
		this.gridColumn24.CurrencyDateField = null;
		this.gridColumn24.CurrencyField = null;
		this.gridColumn24.CurrencyFixed = null;
		this.gridColumn24.CurrencyFixedRate = 0.0;
		this.gridColumn24.CurrencyRateField = null;
		this.gridColumn24.Name = "gridColumn24";
		this.gridColumn24.PrimaryKey = false;
		this.gridColumn24.UnitField = null;
		this.gridColumn25.Caption = "Hư hỏng";
		this.gridColumn25.CurrencyDateField = null;
		this.gridColumn25.CurrencyField = null;
		this.gridColumn25.CurrencyFixed = null;
		this.gridColumn25.CurrencyFixedRate = 0.0;
		this.gridColumn25.CurrencyRateField = null;
		this.gridColumn25.Name = "gridColumn25";
		this.gridColumn25.PrimaryKey = false;
		this.gridColumn25.UnitField = null;
		this.gridColumn26.Caption = "Thiếu";
		this.gridColumn26.CurrencyDateField = null;
		this.gridColumn26.CurrencyField = null;
		this.gridColumn26.CurrencyFixed = null;
		this.gridColumn26.CurrencyFixedRate = 0.0;
		this.gridColumn26.CurrencyRateField = null;
		this.gridColumn26.Name = "gridColumn26";
		this.gridColumn26.PrimaryKey = false;
		this.gridColumn26.UnitField = null;
		this.gridColumn27.Caption = "Tổng hư hỏng";
		this.gridColumn27.CurrencyDateField = null;
		this.gridColumn27.CurrencyField = null;
		this.gridColumn27.CurrencyFixed = null;
		this.gridColumn27.CurrencyFixedRate = 0.0;
		this.gridColumn27.CurrencyRateField = null;
		this.gridColumn27.Name = "gridColumn27";
		this.gridColumn27.PrimaryKey = false;
		this.gridColumn27.UnitField = null;
		this.gridColumn28.Caption = "Thu hổi thực tế";
		this.gridColumn28.CurrencyDateField = null;
		this.gridColumn28.CurrencyField = null;
		this.gridColumn28.CurrencyFixed = null;
		this.gridColumn28.CurrencyFixedRate = 0.0;
		this.gridColumn28.CurrencyRateField = null;
		this.gridColumn28.Name = "gridColumn28";
		this.gridColumn28.PrimaryKey = false;
		this.gridColumn28.UnitField = null;
		this.gridColumn29.Caption = "Giờ bắt đầu chạy";
		this.gridColumn29.CurrencyDateField = null;
		this.gridColumn29.CurrencyField = null;
		this.gridColumn29.CurrencyFixed = null;
		this.gridColumn29.CurrencyFixedRate = 0.0;
		this.gridColumn29.CurrencyRateField = null;
		this.gridColumn29.Name = "gridColumn29";
		this.gridColumn29.PrimaryKey = false;
		this.gridColumn29.UnitField = null;
		this.gridColumn30.Caption = "Giờ hoàn thành";
		this.gridColumn30.CurrencyDateField = null;
		this.gridColumn30.CurrencyField = null;
		this.gridColumn30.CurrencyFixed = null;
		this.gridColumn30.CurrencyFixedRate = 0.0;
		this.gridColumn30.CurrencyRateField = null;
		this.gridColumn30.Name = "gridColumn30";
		this.gridColumn30.PrimaryKey = false;
		this.gridColumn30.UnitField = null;
		this.repBatComplete.AutoHeight = false;
		this.repBatComplete.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repBatComplete.Name = "repBatComplete";
		this.repoWhse.AutoHeight = false;
		this.repoWhse.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.repoWhse.Name = "repoWhse";
		this.repositoryItemMemoEdit2.Name = "repositoryItemMemoEdit2";
		this.repositoryItemMemoExEdit3.AutoHeight = false;
		this.repositoryItemMemoExEdit3.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemMemoExEdit3.Name = "repositoryItemMemoExEdit3";
		this.repositoryItemMemoEdit3.Name = "repositoryItemMemoEdit3";
		this.gridView2.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gridView2.ColumnKey = null;
		this.gridView2.DisableFormSetting = false;
		this.gridView2.DisplayException = false;
		this.gridView2.EditColumn = null;
		this.gridView2.FieldCurrency = null;
		this.gridView2.FieldRate = null;
		this.gridView2.FormId = null;
		this.gridView2.GridControl = this.grdReceipt;
		this.gridView2.IgnoreFilter = false;
		this.gridView2.IndexEditRow = -1;
		this.gridView2.IndicatorWidth = 35;
		this.gridView2.IsApzTable = false;
		this.gridView2.IsInsertNewRow = false;
		this.gridView2.IsLoadingData = false;
		this.gridView2.IsMappingContextAction = false;
		this.gridView2.ItemId = null;
		this.gridView2.LstColumnAllowBatch = null;
		this.gridView2.LstColumnNotAllowBatch = null;
		this.gridView2.LstColumnNotEditable = null;
		this.gridView2.LstColumNotSetting = null;
		this.gridView2.Name = "gridView2";
		this.gridView2.UseBatchMod = false;
		this.tab_Attachment.Controls.Add(this.gridControl3);
		this.tab_Attachment.EditControl = null;
		this.tab_Attachment.EditValue = "Attachment";
		this.tab_Attachment.FieldName = null;
		this.tab_Attachment.IsSearchControl = false;
		this.tab_Attachment.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.tab_Attachment.Name = "tab_Attachment";
		this.tab_Attachment.ReadOnly = false;
		this.tab_Attachment.SettingInVisible = false;
		this.tab_Attachment.SettingReadonly = false;
		this.tab_Attachment.Size = new System.Drawing.Size(734, 194);
		this.tab_Attachment.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Attachment.Text = "Attachment";
		this.gridControl3.AllowPasteExcel = false;
		this.gridControl3.ClipboardData = null;
		this.gridControl3.Dock = System.Windows.Forms.DockStyle.Fill;
		this.gridControl3.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.gridControl3.Location = new System.Drawing.Point(0, 0);
		this.gridControl3.MainView = this.gridView3;
		this.gridControl3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.gridControl3.Name = "gridControl3";
		this.gridControl3.Size = new System.Drawing.Size(734, 194);
		this.gridControl3.TabIndex = 0;
		this.gridControl3.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[2] { this.gridView3, this.gridView1 });
		this.gridView3.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gridView3.ColumnKey = null;
		this.gridView3.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[4] { this.gcPath, this.gcFileName, this.gcExtention, this.gcDownload });
		this.gridView3.DisableFormSetting = false;
		this.gridView3.DisplayException = false;
		this.gridView3.EditColumn = null;
		this.gridView3.FieldCurrency = null;
		this.gridView3.FieldRate = null;
		this.gridView3.FormId = null;
		this.gridView3.GridControl = this.gridControl3;
		this.gridView3.IgnoreFilter = false;
		this.gridView3.IndexEditRow = -1;
		this.gridView3.IndicatorWidth = 35;
		this.gridView3.IsApzTable = false;
		this.gridView3.IsInsertNewRow = false;
		this.gridView3.IsLoadingData = false;
		this.gridView3.IsMappingContextAction = false;
		this.gridView3.ItemId = null;
		this.gridView3.LstColumnAllowBatch = null;
		this.gridView3.LstColumnNotAllowBatch = null;
		this.gridView3.LstColumnNotEditable = null;
		this.gridView3.LstColumNotSetting = null;
		this.gridView3.Name = "gridView3";
		this.gridView3.OptionsView.ColumnAutoWidth = false;
		this.gridView3.OptionsView.ShowGroupPanel = false;
		this.gridView3.UseBatchMod = false;
		this.gcPath.Caption = "Path";
		this.gcPath.CurrencyDateField = null;
		this.gcPath.CurrencyField = null;
		this.gcPath.CurrencyFixed = null;
		this.gcPath.CurrencyFixedRate = 0.0;
		this.gcPath.CurrencyRateField = null;
		this.gcPath.Name = "gcPath";
		this.gcPath.PrimaryKey = false;
		this.gcPath.UnitField = null;
		this.gcPath.Visible = true;
		this.gcPath.VisibleIndex = 0;
		this.gcPath.Width = 247;
		this.gcFileName.Caption = "File Name";
		this.gcFileName.CurrencyDateField = null;
		this.gcFileName.CurrencyField = null;
		this.gcFileName.CurrencyFixed = null;
		this.gcFileName.CurrencyFixedRate = 0.0;
		this.gcFileName.CurrencyRateField = null;
		this.gcFileName.Name = "gcFileName";
		this.gcFileName.PrimaryKey = false;
		this.gcFileName.UnitField = null;
		this.gcFileName.Visible = true;
		this.gcFileName.VisibleIndex = 1;
		this.gcFileName.Width = 250;
		this.gcExtention.Caption = "Extention";
		this.gcExtention.CurrencyDateField = null;
		this.gcExtention.CurrencyField = null;
		this.gcExtention.CurrencyFixed = null;
		this.gcExtention.CurrencyFixedRate = 0.0;
		this.gcExtention.CurrencyRateField = null;
		this.gcExtention.Name = "gcExtention";
		this.gcExtention.PrimaryKey = false;
		this.gcExtention.UnitField = null;
		this.gcExtention.Visible = true;
		this.gcExtention.VisibleIndex = 2;
		this.gcDownload.Caption = "Download";
		this.gcDownload.CurrencyDateField = null;
		this.gcDownload.CurrencyField = null;
		this.gcDownload.CurrencyFixed = null;
		this.gcDownload.CurrencyFixedRate = 0.0;
		this.gcDownload.CurrencyRateField = null;
		this.gcDownload.Name = "gcDownload";
		this.gcDownload.PrimaryKey = false;
		this.gcDownload.UnitField = null;
		this.gcDownload.Visible = true;
		this.gcDownload.VisibleIndex = 3;
		this.gridView1.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gridView1.ColumnKey = null;
		this.gridView1.DisableFormSetting = false;
		this.gridView1.DisplayException = false;
		this.gridView1.EditColumn = null;
		this.gridView1.FieldCurrency = null;
		this.gridView1.FieldRate = null;
		this.gridView1.FormId = null;
		this.gridView1.GridControl = this.gridControl3;
		this.gridView1.IgnoreFilter = false;
		this.gridView1.IndexEditRow = -1;
		this.gridView1.IndicatorWidth = 35;
		this.gridView1.IsApzTable = false;
		this.gridView1.IsInsertNewRow = false;
		this.gridView1.IsLoadingData = false;
		this.gridView1.IsMappingContextAction = false;
		this.gridView1.ItemId = null;
		this.gridView1.LstColumnAllowBatch = null;
		this.gridView1.LstColumnNotAllowBatch = null;
		this.gridView1.LstColumnNotEditable = null;
		this.gridView1.LstColumNotSetting = null;
		this.gridView1.Name = "gridView1";
		this.gridView1.UseBatchMod = false;
		this.tabPlaned.Controls.Add(this.btn);
		this.tabPlaned.Controls.Add(this.grd_Planed);
		this.tabPlaned.EditControl = null;
		this.tabPlaned.EditValue = "Planed";
		this.tabPlaned.FieldName = null;
		this.tabPlaned.IsSearchControl = false;
		this.tabPlaned.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.tabPlaned.Name = "tabPlaned";
		this.tabPlaned.ReadOnly = false;
		this.tabPlaned.SettingInVisible = false;
		this.tabPlaned.SettingReadonly = false;
		this.tabPlaned.Size = new System.Drawing.Size(740, 385);
		this.tabPlaned.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tabPlaned.Text = "Planed";
		this.btn.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn.EditControl = null;
		this.btn.EditValue = "Release";
		this.btn.FieldName = null;
		this.btn.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn.ImageOptions.Image");
		this.btn.IsSearchControl = false;
		this.btn.Location = new System.Drawing.Point(11, 351);
		this.btn.Name = "btn";
		this.btn.ReadOnly = false;
		this.btn.SettingInVisible = false;
		this.btn.SettingReadonly = false;
		this.btn.Size = new System.Drawing.Size(104, 20);
		this.btn.TabIndex = 1;
		this.btn.Text = "Release";
		this.btn.Click += new System.EventHandler(this.simpleButton2_Click_1);
		this.grd_Planed.AllowPasteExcel = false;
		this.grd_Planed.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Planed.ClipboardData = null;
		this.grd_Planed.Location = new System.Drawing.Point(-1, 0);
		this.grd_Planed.MainView = this.gv_Planed;
		this.grd_Planed.Name = "grd_Planed";
		this.grd_Planed.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.repositoryItemMemoExEdit5, this.repositoryItemMemoExEdit6 });
		this.grd_Planed.Size = new System.Drawing.Size(737, 337);
		this.grd_Planed.TabIndex = 0;
		this.grd_Planed.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Planed });
		this.gv_Planed.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Planed.ColumnKey = null;
		this.gv_Planed.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[12]
		{
			this.columMoNo, this.columMoName, this.colItemName, this.clumItemName, this.clumQuantity, this.coluOrDer, this.columnWhse, this.columDueDate, this.columnRemark, this.clProduc,
			this.columnBatchIssue, this.colBatchComplete
		});
		this.gv_Planed.DisableFormSetting = false;
		this.gv_Planed.DisplayException = false;
		this.gv_Planed.EditColumn = null;
		this.gv_Planed.FieldCurrency = null;
		this.gv_Planed.FieldRate = null;
		this.gv_Planed.FormId = null;
		this.gv_Planed.GridControl = this.grd_Planed;
		this.gv_Planed.IgnoreFilter = false;
		this.gv_Planed.IndexEditRow = -1;
		this.gv_Planed.IndicatorWidth = 40;
		this.gv_Planed.IsApzTable = false;
		this.gv_Planed.IsInsertNewRow = false;
		this.gv_Planed.IsLoadingData = false;
		this.gv_Planed.IsMappingContextAction = false;
		this.gv_Planed.ItemId = null;
		this.gv_Planed.LstColumnAllowBatch = null;
		this.gv_Planed.LstColumnNotAllowBatch = null;
		this.gv_Planed.LstColumnNotEditable = null;
		this.gv_Planed.LstColumNotSetting = null;
		this.gv_Planed.Name = "gv_Planed";
		this.gv_Planed.OptionsSelection.MultiSelect = true;
		this.gv_Planed.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
		this.gv_Planed.OptionsView.ColumnAutoWidth = false;
		this.gv_Planed.OptionsView.ShowGroupPanel = false;
		this.gv_Planed.UseBatchMod = false;
		this.gv_Planed.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_Planed_CustomDrawRowIndicator);
		this.gv_Planed.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Planed_CellValueChanged);
		this.columMoNo.Caption = "MO No.";
		this.columMoNo.CurrencyDateField = null;
		this.columMoNo.CurrencyField = null;
		this.columMoNo.CurrencyFixed = null;
		this.columMoNo.CurrencyFixedRate = 0.0;
		this.columMoNo.CurrencyRateField = null;
		this.columMoNo.FieldName = "MONo";
		this.columMoNo.Name = "columMoNo";
		this.columMoNo.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True;
		this.columMoNo.PrimaryKey = false;
		this.columMoNo.UnitField = null;
		this.columMoNo.Visible = true;
		this.columMoNo.VisibleIndex = 1;
		this.columMoNo.Width = 81;
		this.columMoName.Caption = "MO. Name";
		this.columMoName.CurrencyDateField = null;
		this.columMoName.CurrencyField = null;
		this.columMoName.CurrencyFixed = null;
		this.columMoName.CurrencyFixedRate = 0.0;
		this.columMoName.CurrencyRateField = null;
		this.columMoName.FieldName = "MOName";
		this.columMoName.Name = "columMoName";
		this.columMoName.PrimaryKey = false;
		this.columMoName.UnitField = null;
		this.columMoName.Visible = true;
		this.columMoName.VisibleIndex = 2;
		this.columMoName.Width = 81;
		this.colItemName.Caption = "ItemCode";
		this.colItemName.CurrencyDateField = null;
		this.colItemName.CurrencyField = null;
		this.colItemName.CurrencyFixed = null;
		this.colItemName.CurrencyFixedRate = 0.0;
		this.colItemName.CurrencyRateField = null;
		this.colItemName.FieldName = "ItemCode";
		this.colItemName.Name = "colItemName";
		this.colItemName.PrimaryKey = false;
		this.colItemName.UnitField = null;
		this.colItemName.Visible = true;
		this.colItemName.VisibleIndex = 3;
		this.colItemName.Width = 116;
		this.clumItemName.Caption = "Item Name";
		this.clumItemName.CurrencyDateField = null;
		this.clumItemName.CurrencyField = null;
		this.clumItemName.CurrencyFixed = null;
		this.clumItemName.CurrencyFixedRate = 0.0;
		this.clumItemName.CurrencyRateField = null;
		this.clumItemName.FieldName = "ItemName";
		this.clumItemName.Name = "clumItemName";
		this.clumItemName.PrimaryKey = false;
		this.clumItemName.UnitField = null;
		this.clumItemName.Visible = true;
		this.clumItemName.VisibleIndex = 4;
		this.clumItemName.Width = 91;
		this.clumQuantity.Caption = "Quantity";
		this.clumQuantity.CurrencyDateField = null;
		this.clumQuantity.CurrencyField = null;
		this.clumQuantity.CurrencyFixed = null;
		this.clumQuantity.CurrencyFixedRate = 0.0;
		this.clumQuantity.CurrencyRateField = null;
		this.clumQuantity.FieldName = "Quantity";
		this.clumQuantity.Name = "clumQuantity";
		this.clumQuantity.PrimaryKey = false;
		this.clumQuantity.UnitField = null;
		this.clumQuantity.Visible = true;
		this.clumQuantity.VisibleIndex = 7;
		this.clumQuantity.Width = 52;
		this.coluOrDer.Caption = "Order";
		this.coluOrDer.CurrencyDateField = null;
		this.coluOrDer.CurrencyField = null;
		this.coluOrDer.CurrencyFixed = null;
		this.coluOrDer.CurrencyFixedRate = 0.0;
		this.coluOrDer.CurrencyRateField = null;
		this.coluOrDer.FieldName = "DocEntry";
		this.coluOrDer.Name = "coluOrDer";
		this.coluOrDer.PrimaryKey = false;
		this.coluOrDer.UnitField = null;
		this.coluOrDer.Visible = true;
		this.coluOrDer.VisibleIndex = 8;
		this.coluOrDer.Width = 50;
		this.columnWhse.Caption = "Whse";
		this.columnWhse.CurrencyDateField = null;
		this.columnWhse.CurrencyField = null;
		this.columnWhse.CurrencyFixed = null;
		this.columnWhse.CurrencyFixedRate = 0.0;
		this.columnWhse.CurrencyRateField = null;
		this.columnWhse.FieldName = "Warehouse";
		this.columnWhse.Name = "columnWhse";
		this.columnWhse.PrimaryKey = false;
		this.columnWhse.UnitField = null;
		this.columnWhse.Visible = true;
		this.columnWhse.VisibleIndex = 10;
		this.columnWhse.Width = 50;
		this.columDueDate.Caption = "DueDate";
		this.columDueDate.CurrencyDateField = null;
		this.columDueDate.CurrencyField = null;
		this.columDueDate.CurrencyFixed = null;
		this.columDueDate.CurrencyFixedRate = 0.0;
		this.columDueDate.CurrencyRateField = null;
		this.columDueDate.FieldName = "DueDate";
		this.columDueDate.Name = "columDueDate";
		this.columDueDate.PrimaryKey = false;
		this.columDueDate.UnitField = null;
		this.columDueDate.Visible = true;
		this.columDueDate.VisibleIndex = 11;
		this.columDueDate.Width = 71;
		this.columnRemark.Caption = "Remark";
		this.columnRemark.ColumnEdit = this.repositoryItemMemoExEdit6;
		this.columnRemark.CurrencyDateField = null;
		this.columnRemark.CurrencyField = null;
		this.columnRemark.CurrencyFixed = null;
		this.columnRemark.CurrencyFixedRate = 0.0;
		this.columnRemark.CurrencyRateField = null;
		this.columnRemark.FieldName = "Remark";
		this.columnRemark.Name = "columnRemark";
		this.columnRemark.PrimaryKey = false;
		this.columnRemark.UnitField = null;
		this.columnRemark.Visible = true;
		this.columnRemark.VisibleIndex = 12;
		this.columnRemark.Width = 74;
		this.repositoryItemMemoExEdit6.AutoHeight = false;
		this.repositoryItemMemoExEdit6.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemMemoExEdit6.Name = "repositoryItemMemoExEdit6";
		this.repositoryItemMemoExEdit6.ShowIcon = false;
		this.clProduc.Caption = "Produc No";
		this.clProduc.CurrencyDateField = null;
		this.clProduc.CurrencyField = null;
		this.clProduc.CurrencyFixed = null;
		this.clProduc.CurrencyFixedRate = 0.0;
		this.clProduc.CurrencyRateField = null;
		this.clProduc.FieldName = "OriginNum";
		this.clProduc.Name = "clProduc";
		this.clProduc.PrimaryKey = false;
		this.clProduc.UnitField = null;
		this.clProduc.Visible = true;
		this.clProduc.VisibleIndex = 9;
		this.clProduc.Width = 50;
		this.columnBatchIssue.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.columnBatchIssue.AppearanceCell.ForeColor = System.Drawing.Color.Blue;
		this.columnBatchIssue.AppearanceCell.Options.UseFont = true;
		this.columnBatchIssue.AppearanceCell.Options.UseForeColor = true;
		this.columnBatchIssue.Caption = "Batch Issued";
		this.columnBatchIssue.CurrencyDateField = null;
		this.columnBatchIssue.CurrencyField = null;
		this.columnBatchIssue.CurrencyFixed = null;
		this.columnBatchIssue.CurrencyFixedRate = 0.0;
		this.columnBatchIssue.CurrencyRateField = null;
		this.columnBatchIssue.FieldName = "BatchIssue";
		this.columnBatchIssue.Name = "columnBatchIssue";
		this.columnBatchIssue.PrimaryKey = false;
		this.columnBatchIssue.UnitField = null;
		this.columnBatchIssue.Visible = true;
		this.columnBatchIssue.VisibleIndex = 5;
		this.columnBatchIssue.Width = 196;
		this.colBatchComplete.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.colBatchComplete.AppearanceCell.ForeColor = System.Drawing.Color.FromArgb(192, 64, 0);
		this.colBatchComplete.AppearanceCell.Options.UseFont = true;
		this.colBatchComplete.AppearanceCell.Options.UseForeColor = true;
		this.colBatchComplete.AppearanceCell.Options.UseTextOptions = true;
		this.colBatchComplete.AppearanceCell.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
		this.colBatchComplete.Caption = "Batch Complete";
		this.colBatchComplete.CurrencyDateField = null;
		this.colBatchComplete.CurrencyField = null;
		this.colBatchComplete.CurrencyFixed = null;
		this.colBatchComplete.CurrencyFixedRate = 0.0;
		this.colBatchComplete.CurrencyRateField = null;
		this.colBatchComplete.FieldName = "BatchComplete";
		this.colBatchComplete.Name = "colBatchComplete";
		this.colBatchComplete.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
		this.colBatchComplete.PrimaryKey = false;
		this.colBatchComplete.UnitField = null;
		this.colBatchComplete.Visible = true;
		this.colBatchComplete.VisibleIndex = 6;
		this.colBatchComplete.Width = 339;
		this.repositoryItemMemoExEdit5.AutoHeight = false;
		this.repositoryItemMemoExEdit5.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemMemoExEdit5.Name = "repositoryItemMemoExEdit5";
		this.tabRelease.Controls.Add(this.btnRelease);
		this.tabRelease.Controls.Add(this.grd_Release);
		this.tabRelease.EditControl = null;
		this.tabRelease.EditValue = "Released";
		this.tabRelease.FieldName = null;
		this.tabRelease.IsSearchControl = false;
		this.tabRelease.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.tabRelease.Name = "tabRelease";
		this.tabRelease.ReadOnly = false;
		this.tabRelease.SettingInVisible = false;
		this.tabRelease.SettingReadonly = false;
		this.tabRelease.Size = new System.Drawing.Size(740, 385);
		this.tabRelease.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tabRelease.Text = "Released";
		this.tabRelease.Paint += new System.Windows.Forms.PaintEventHandler(this.tabRelease_Paint);
		this.btnRelease.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnRelease.EditControl = null;
		this.btnRelease.EditValue = "Close";
		this.btnRelease.FieldName = null;
		this.btnRelease.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnRelease.ImageOptions.Image");
		this.btnRelease.IsSearchControl = false;
		this.btnRelease.Location = new System.Drawing.Point(9, 349);
		this.btnRelease.Name = "btnRelease";
		this.btnRelease.ReadOnly = false;
		this.btnRelease.SettingInVisible = false;
		this.btnRelease.SettingReadonly = false;
		this.btnRelease.Size = new System.Drawing.Size(109, 20);
		this.btnRelease.TabIndex = 3;
		this.btnRelease.Text = "Close";
		this.btnRelease.Click += new System.EventHandler(this.simpleButton3_Click);
		this.grd_Release.AllowPasteExcel = false;
		this.grd_Release.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Release.ClipboardData = null;
		this.grd_Release.Location = new System.Drawing.Point(0, 0);
		this.grd_Release.MainView = this.gv_Release;
		this.grd_Release.Name = "grd_Release";
		this.grd_Release.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.repositoryItemMemoEdit4, this.repositoryItemMemoExEdit7 });
		this.grd_Release.Size = new System.Drawing.Size(736, 332);
		this.grd_Release.TabIndex = 2;
		this.grd_Release.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Release });
		this.gv_Release.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Release.ColumnKey = null;
		this.gv_Release.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[12]
		{
			this.gridColumn10, this.gridColumn11, this.gridColumn12, this.gridColumn13, this.gridColumn14, this.gridColumn15, this.gridColumn16, this.gridColumn17, this.gridColumn18, this.gridColumn19,
			this.columBatchComplete, this.gridColumn20
		});
		this.gv_Release.DisableFormSetting = false;
		this.gv_Release.DisplayException = false;
		this.gv_Release.EditColumn = null;
		this.gv_Release.FieldCurrency = null;
		this.gv_Release.FieldRate = null;
		this.gv_Release.FormId = null;
		this.gv_Release.GridControl = this.grd_Release;
		this.gv_Release.IgnoreFilter = false;
		this.gv_Release.IndexEditRow = -1;
		this.gv_Release.IndicatorWidth = 40;
		this.gv_Release.IsApzTable = false;
		this.gv_Release.IsInsertNewRow = false;
		this.gv_Release.IsLoadingData = false;
		this.gv_Release.IsMappingContextAction = false;
		this.gv_Release.ItemId = null;
		this.gv_Release.LstColumnAllowBatch = null;
		this.gv_Release.LstColumnNotAllowBatch = null;
		this.gv_Release.LstColumnNotEditable = null;
		this.gv_Release.LstColumNotSetting = null;
		this.gv_Release.Name = "gv_Release";
		this.gv_Release.OptionsSelection.MultiSelect = true;
		this.gv_Release.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
		this.gv_Release.OptionsView.ColumnAutoWidth = false;
		this.gv_Release.OptionsView.ShowGroupPanel = false;
		this.gv_Release.UseBatchMod = false;
		this.gv_Release.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_Release_CustomDrawRowIndicator);
		this.gv_Release.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Release_CellValueChanged);
		this.gridColumn10.Caption = "MO No.";
		this.gridColumn10.CurrencyDateField = null;
		this.gridColumn10.CurrencyField = null;
		this.gridColumn10.CurrencyFixed = null;
		this.gridColumn10.CurrencyFixedRate = 0.0;
		this.gridColumn10.CurrencyRateField = null;
		this.gridColumn10.FieldName = "MONo";
		this.gridColumn10.Name = "gridColumn10";
		this.gridColumn10.PrimaryKey = false;
		this.gridColumn10.UnitField = null;
		this.gridColumn10.Visible = true;
		this.gridColumn10.VisibleIndex = 1;
		this.gridColumn10.Width = 80;
		this.gridColumn11.Caption = "MO. Name";
		this.gridColumn11.CurrencyDateField = null;
		this.gridColumn11.CurrencyField = null;
		this.gridColumn11.CurrencyFixed = null;
		this.gridColumn11.CurrencyFixedRate = 0.0;
		this.gridColumn11.CurrencyRateField = null;
		this.gridColumn11.FieldName = "MOName";
		this.gridColumn11.Name = "gridColumn11";
		this.gridColumn11.PrimaryKey = false;
		this.gridColumn11.UnitField = null;
		this.gridColumn11.Visible = true;
		this.gridColumn11.VisibleIndex = 2;
		this.gridColumn11.Width = 80;
		this.gridColumn12.Caption = "ItemCode";
		this.gridColumn12.CurrencyDateField = null;
		this.gridColumn12.CurrencyField = null;
		this.gridColumn12.CurrencyFixed = null;
		this.gridColumn12.CurrencyFixedRate = 0.0;
		this.gridColumn12.CurrencyRateField = null;
		this.gridColumn12.FieldName = "ItemCode";
		this.gridColumn12.Name = "gridColumn12";
		this.gridColumn12.PrimaryKey = false;
		this.gridColumn12.UnitField = null;
		this.gridColumn12.Visible = true;
		this.gridColumn12.VisibleIndex = 3;
		this.gridColumn12.Width = 80;
		this.gridColumn13.Caption = "Item Name";
		this.gridColumn13.CurrencyDateField = null;
		this.gridColumn13.CurrencyField = null;
		this.gridColumn13.CurrencyFixed = null;
		this.gridColumn13.CurrencyFixedRate = 0.0;
		this.gridColumn13.CurrencyRateField = null;
		this.gridColumn13.FieldName = "ItemName";
		this.gridColumn13.Name = "gridColumn13";
		this.gridColumn13.PrimaryKey = false;
		this.gridColumn13.UnitField = null;
		this.gridColumn13.Visible = true;
		this.gridColumn13.VisibleIndex = 4;
		this.gridColumn13.Width = 80;
		this.gridColumn14.Caption = "Quantity";
		this.gridColumn14.CurrencyDateField = null;
		this.gridColumn14.CurrencyField = null;
		this.gridColumn14.CurrencyFixed = null;
		this.gridColumn14.CurrencyFixedRate = 0.0;
		this.gridColumn14.CurrencyRateField = null;
		this.gridColumn14.FieldName = "Quantity";
		this.gridColumn14.Name = "gridColumn14";
		this.gridColumn14.PrimaryKey = false;
		this.gridColumn14.UnitField = null;
		this.gridColumn14.Visible = true;
		this.gridColumn14.VisibleIndex = 7;
		this.gridColumn14.Width = 55;
		this.gridColumn15.Caption = "Order";
		this.gridColumn15.CurrencyDateField = null;
		this.gridColumn15.CurrencyField = null;
		this.gridColumn15.CurrencyFixed = null;
		this.gridColumn15.CurrencyFixedRate = 0.0;
		this.gridColumn15.CurrencyRateField = null;
		this.gridColumn15.FieldName = "OriginNum";
		this.gridColumn15.Name = "gridColumn15";
		this.gridColumn15.PrimaryKey = false;
		this.gridColumn15.UnitField = null;
		this.gridColumn15.Visible = true;
		this.gridColumn15.VisibleIndex = 8;
		this.gridColumn15.Width = 42;
		this.gridColumn16.Caption = "Whse";
		this.gridColumn16.CurrencyDateField = null;
		this.gridColumn16.CurrencyField = null;
		this.gridColumn16.CurrencyFixed = null;
		this.gridColumn16.CurrencyFixedRate = 0.0;
		this.gridColumn16.CurrencyRateField = null;
		this.gridColumn16.FieldName = "Warehouse";
		this.gridColumn16.Name = "gridColumn16";
		this.gridColumn16.PrimaryKey = false;
		this.gridColumn16.UnitField = null;
		this.gridColumn16.Visible = true;
		this.gridColumn16.VisibleIndex = 9;
		this.gridColumn16.Width = 42;
		this.gridColumn17.Caption = "DueDate";
		this.gridColumn17.CurrencyDateField = null;
		this.gridColumn17.CurrencyField = null;
		this.gridColumn17.CurrencyFixed = null;
		this.gridColumn17.CurrencyFixedRate = 0.0;
		this.gridColumn17.CurrencyRateField = null;
		this.gridColumn17.FieldName = "DueDate";
		this.gridColumn17.Name = "gridColumn17";
		this.gridColumn17.PrimaryKey = false;
		this.gridColumn17.UnitField = null;
		this.gridColumn17.Visible = true;
		this.gridColumn17.VisibleIndex = 10;
		this.gridColumn17.Width = 42;
		this.gridColumn18.Caption = "Remark";
		this.gridColumn18.ColumnEdit = this.repositoryItemMemoExEdit7;
		this.gridColumn18.CurrencyDateField = null;
		this.gridColumn18.CurrencyField = null;
		this.gridColumn18.CurrencyFixed = null;
		this.gridColumn18.CurrencyFixedRate = 0.0;
		this.gridColumn18.CurrencyRateField = null;
		this.gridColumn18.FieldName = "Remark";
		this.gridColumn18.Name = "gridColumn18";
		this.gridColumn18.PrimaryKey = false;
		this.gridColumn18.UnitField = null;
		this.gridColumn18.Visible = true;
		this.gridColumn18.VisibleIndex = 12;
		this.gridColumn18.Width = 47;
		this.repositoryItemMemoExEdit7.AutoHeight = false;
		this.repositoryItemMemoExEdit7.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemMemoExEdit7.Name = "repositoryItemMemoExEdit7";
		this.repositoryItemMemoExEdit7.ShowIcon = false;
		this.gridColumn19.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.gridColumn19.AppearanceCell.ForeColor = System.Drawing.Color.Blue;
		this.gridColumn19.AppearanceCell.Options.UseFont = true;
		this.gridColumn19.AppearanceCell.Options.UseForeColor = true;
		this.gridColumn19.Caption = "Batch Issued";
		this.gridColumn19.CurrencyDateField = null;
		this.gridColumn19.CurrencyField = null;
		this.gridColumn19.CurrencyFixed = null;
		this.gridColumn19.CurrencyFixedRate = 0.0;
		this.gridColumn19.CurrencyRateField = null;
		this.gridColumn19.FieldName = "BatchIssue";
		this.gridColumn19.Name = "gridColumn19";
		this.gridColumn19.PrimaryKey = false;
		this.gridColumn19.UnitField = null;
		this.gridColumn19.Visible = true;
		this.gridColumn19.VisibleIndex = 5;
		this.gridColumn19.Width = 241;
		this.columBatchComplete.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.columBatchComplete.AppearanceCell.ForeColor = System.Drawing.Color.Maroon;
		this.columBatchComplete.AppearanceCell.Options.UseFont = true;
		this.columBatchComplete.AppearanceCell.Options.UseForeColor = true;
		this.columBatchComplete.AppearanceCell.Options.UseTextOptions = true;
		this.columBatchComplete.AppearanceCell.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
		this.columBatchComplete.AppearanceHeader.Options.UseTextOptions = true;
		this.columBatchComplete.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
		this.columBatchComplete.Caption = "Batch Complete";
		this.columBatchComplete.CurrencyDateField = null;
		this.columBatchComplete.CurrencyField = null;
		this.columBatchComplete.CurrencyFixed = null;
		this.columBatchComplete.CurrencyFixedRate = 0.0;
		this.columBatchComplete.CurrencyRateField = null;
		this.columBatchComplete.FieldName = "BatchComplete";
		this.columBatchComplete.Name = "columBatchComplete";
		this.columBatchComplete.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
		this.columBatchComplete.PrimaryKey = false;
		this.columBatchComplete.UnitField = null;
		this.columBatchComplete.Visible = true;
		this.columBatchComplete.VisibleIndex = 6;
		this.columBatchComplete.Width = 394;
		this.gridColumn20.Caption = "Production No";
		this.gridColumn20.CurrencyDateField = null;
		this.gridColumn20.CurrencyField = null;
		this.gridColumn20.CurrencyFixed = null;
		this.gridColumn20.CurrencyFixedRate = 0.0;
		this.gridColumn20.CurrencyRateField = null;
		this.gridColumn20.FieldName = "DocEntry";
		this.gridColumn20.Name = "gridColumn20";
		this.gridColumn20.PrimaryKey = false;
		this.gridColumn20.UnitField = null;
		this.gridColumn20.Visible = true;
		this.gridColumn20.VisibleIndex = 11;
		this.gridColumn20.Width = 69;
		this.repositoryItemMemoEdit4.Name = "repositoryItemMemoEdit4";
		this.tabClose.Controls.Add(this.gridControl1);
		this.tabClose.EditControl = null;
		this.tabClose.EditValue = "Closed";
		this.tabClose.FieldName = null;
		this.tabClose.IsSearchControl = false;
		this.tabClose.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.tabClose.Name = "tabClose";
		this.tabClose.ReadOnly = false;
		this.tabClose.SettingInVisible = false;
		this.tabClose.SettingReadonly = false;
		this.tabClose.Size = new System.Drawing.Size(740, 385);
		this.tabClose.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tabClose.Text = "Closed";
		this.gridControl1.AllowPasteExcel = false;
		this.gridControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.gridControl1.ClipboardData = null;
		this.gridControl1.Location = new System.Drawing.Point(-3, 0);
		this.gridControl1.MainView = this.gridView6;
		this.gridControl1.Name = "gridControl1";
		this.gridControl1.Size = new System.Drawing.Size(739, 335);
		this.gridControl1.TabIndex = 3;
		this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gridView6 });
		this.gridView6.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gridView6.ColumnKey = null;
		this.gridView6.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[11]
		{
			this.gridColumn1, this.gridColumn2, this.gridColumn3, this.gridColumn4, this.gridColumn5, this.gridColumn6, this.gridColumn7, this.gridColumn8, this.gridColumn9, this.gridColumn21,
			this.gridColumn22
		});
		this.gridView6.DisableFormSetting = false;
		this.gridView6.DisplayException = false;
		this.gridView6.EditColumn = null;
		this.gridView6.FieldCurrency = null;
		this.gridView6.FieldRate = null;
		this.gridView6.FormId = null;
		this.gridView6.GridControl = this.gridControl1;
		this.gridView6.IgnoreFilter = false;
		this.gridView6.IndexEditRow = -1;
		this.gridView6.IndicatorWidth = 40;
		this.gridView6.IsApzTable = false;
		this.gridView6.IsInsertNewRow = false;
		this.gridView6.IsLoadingData = false;
		this.gridView6.IsMappingContextAction = false;
		this.gridView6.ItemId = null;
		this.gridView6.LstColumnAllowBatch = null;
		this.gridView6.LstColumnNotAllowBatch = null;
		this.gridView6.LstColumnNotEditable = null;
		this.gridView6.LstColumNotSetting = null;
		this.gridView6.Name = "gridView6";
		this.gridView6.OptionsSelection.MultiSelect = true;
		this.gridView6.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
		this.gridView6.OptionsView.ShowGroupPanel = false;
		this.gridView6.UseBatchMod = false;
		this.gridColumn1.Caption = "MO No.";
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "MONo";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.PrimaryKey = false;
		this.gridColumn1.UnitField = null;
		this.gridColumn1.Visible = true;
		this.gridColumn1.VisibleIndex = 1;
		this.gridColumn2.Caption = "MO. Name";
		this.gridColumn2.CurrencyDateField = null;
		this.gridColumn2.CurrencyField = null;
		this.gridColumn2.CurrencyFixed = null;
		this.gridColumn2.CurrencyFixedRate = 0.0;
		this.gridColumn2.CurrencyRateField = null;
		this.gridColumn2.FieldName = "MOName";
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.PrimaryKey = false;
		this.gridColumn2.UnitField = null;
		this.gridColumn2.Visible = true;
		this.gridColumn2.VisibleIndex = 2;
		this.gridColumn3.Caption = "ItemCode";
		this.gridColumn3.CurrencyDateField = null;
		this.gridColumn3.CurrencyField = null;
		this.gridColumn3.CurrencyFixed = null;
		this.gridColumn3.CurrencyFixedRate = 0.0;
		this.gridColumn3.CurrencyRateField = null;
		this.gridColumn3.FieldName = "ItemCode";
		this.gridColumn3.Name = "gridColumn3";
		this.gridColumn3.PrimaryKey = false;
		this.gridColumn3.UnitField = null;
		this.gridColumn3.Visible = true;
		this.gridColumn3.VisibleIndex = 3;
		this.gridColumn4.Caption = "Item Name";
		this.gridColumn4.CurrencyDateField = null;
		this.gridColumn4.CurrencyField = null;
		this.gridColumn4.CurrencyFixed = null;
		this.gridColumn4.CurrencyFixedRate = 0.0;
		this.gridColumn4.CurrencyRateField = null;
		this.gridColumn4.FieldName = "ItemName";
		this.gridColumn4.Name = "gridColumn4";
		this.gridColumn4.PrimaryKey = false;
		this.gridColumn4.UnitField = null;
		this.gridColumn4.Visible = true;
		this.gridColumn4.VisibleIndex = 4;
		this.gridColumn5.Caption = "Quantity";
		this.gridColumn5.CurrencyDateField = null;
		this.gridColumn5.CurrencyField = null;
		this.gridColumn5.CurrencyFixed = null;
		this.gridColumn5.CurrencyFixedRate = 0.0;
		this.gridColumn5.CurrencyRateField = null;
		this.gridColumn5.FieldName = "Quantity";
		this.gridColumn5.Name = "gridColumn5";
		this.gridColumn5.PrimaryKey = false;
		this.gridColumn5.UnitField = null;
		this.gridColumn5.Visible = true;
		this.gridColumn5.VisibleIndex = 6;
		this.gridColumn6.Caption = "Order";
		this.gridColumn6.CurrencyDateField = null;
		this.gridColumn6.CurrencyField = null;
		this.gridColumn6.CurrencyFixed = null;
		this.gridColumn6.CurrencyFixedRate = 0.0;
		this.gridColumn6.CurrencyRateField = null;
		this.gridColumn6.FieldName = "OriginNum";
		this.gridColumn6.Name = "gridColumn6";
		this.gridColumn6.PrimaryKey = false;
		this.gridColumn6.UnitField = null;
		this.gridColumn6.Visible = true;
		this.gridColumn6.VisibleIndex = 7;
		this.gridColumn7.Caption = "Whse";
		this.gridColumn7.CurrencyDateField = null;
		this.gridColumn7.CurrencyField = null;
		this.gridColumn7.CurrencyFixed = null;
		this.gridColumn7.CurrencyFixedRate = 0.0;
		this.gridColumn7.CurrencyRateField = null;
		this.gridColumn7.FieldName = "Warehouse";
		this.gridColumn7.Name = "gridColumn7";
		this.gridColumn7.PrimaryKey = false;
		this.gridColumn7.UnitField = null;
		this.gridColumn7.Visible = true;
		this.gridColumn7.VisibleIndex = 8;
		this.gridColumn8.Caption = "DueDate";
		this.gridColumn8.CurrencyDateField = null;
		this.gridColumn8.CurrencyField = null;
		this.gridColumn8.CurrencyFixed = null;
		this.gridColumn8.CurrencyFixedRate = 0.0;
		this.gridColumn8.CurrencyRateField = null;
		this.gridColumn8.FieldName = "DueDate";
		this.gridColumn8.Name = "gridColumn8";
		this.gridColumn8.PrimaryKey = false;
		this.gridColumn8.UnitField = null;
		this.gridColumn8.Visible = true;
		this.gridColumn8.VisibleIndex = 9;
		this.gridColumn9.Caption = "Remark";
		this.gridColumn9.CurrencyDateField = null;
		this.gridColumn9.CurrencyField = null;
		this.gridColumn9.CurrencyFixed = null;
		this.gridColumn9.CurrencyFixedRate = 0.0;
		this.gridColumn9.CurrencyRateField = null;
		this.gridColumn9.FieldName = "Remark";
		this.gridColumn9.Name = "gridColumn9";
		this.gridColumn9.PrimaryKey = false;
		this.gridColumn9.UnitField = null;
		this.gridColumn9.Visible = true;
		this.gridColumn9.VisibleIndex = 11;
		this.gridColumn21.Caption = "Batch Issued";
		this.gridColumn21.CurrencyDateField = null;
		this.gridColumn21.CurrencyField = null;
		this.gridColumn21.CurrencyFixed = null;
		this.gridColumn21.CurrencyFixedRate = 0.0;
		this.gridColumn21.CurrencyRateField = null;
		this.gridColumn21.FieldName = "BatchIssue";
		this.gridColumn21.Name = "gridColumn21";
		this.gridColumn21.PrimaryKey = false;
		this.gridColumn21.UnitField = null;
		this.gridColumn21.Visible = true;
		this.gridColumn21.VisibleIndex = 5;
		this.gridColumn22.Caption = "Production No";
		this.gridColumn22.CurrencyDateField = null;
		this.gridColumn22.CurrencyField = null;
		this.gridColumn22.CurrencyFixed = null;
		this.gridColumn22.CurrencyFixedRate = 0.0;
		this.gridColumn22.CurrencyRateField = null;
		this.gridColumn22.FieldName = "DocEntry";
		this.gridColumn22.Name = "gridColumn22";
		this.gridColumn22.PrimaryKey = false;
		this.gridColumn22.UnitField = null;
		this.gridColumn22.Visible = true;
		this.gridColumn22.VisibleIndex = 10;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(742, 464);
		base.Controls.Add(this.xtraTabControl1);
		base.Controls.Add(this.panelControl1);
		base.IconOptions.ShowIcon = false;
		base.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		base.Name = "frm_ProductionPlanning";
		base.ShowInTaskbar = false;
		this.Text = "Production Planning";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_ProductionPlanning_FormClosing);
		base.Load += new System.EventHandler(this.frm_ProductionPlanning_Load);
		base.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm_ProductionPlanning_KeyDown);
		this.panel1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.panelControl1).EndInit();
		this.panelControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.dtToDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dtToDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dtFromDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dtFromDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtCardCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtCardName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).EndInit();
		this.xtraTabControl1.ResumeLayout(false);
		this.tabMO.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl2).EndInit();
		this.splitContainerControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdMO).EndInit();
		this.ctxDocsMO.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gvMO).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repItemCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repRouting).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repUOM).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repOcr1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repOcr2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repOcr3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repOcr4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repOcr5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repFixBatch).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Status).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridView5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.xtratabDoc).EndInit();
		this.xtratabDoc.ResumeLayout(false);
		this.tab_Routing.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdRouting).EndInit();
		this.contextRouting.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gvRouting).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repOpCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repSupCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repWorkCenter).EndInit();
		((System.ComponentModel.ISupportInitialize)this.re_ControlKey).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repUOMRouting).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repTime).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repPRT).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repPreOp).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repNextOp).EndInit();
		((System.ComponentModel.ISupportInitialize)this.reitemcode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repWhse).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridView4).EndInit();
		this.tab_ReceiptFromProduction.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdReceipt).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gvReceipt).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repSaleorderNo).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repSOLine).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repDelDate.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repDelDate).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repUOMReceipt).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_WHSE).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repGetBatch).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repBatComplete).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repoWhse).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoEdit2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoEdit3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridView2).EndInit();
		this.tab_Attachment.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gridControl3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridView3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridView1).EndInit();
		this.tabPlaned.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Planed).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Planed).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit6).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit5).EndInit();
		this.tabRelease.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Release).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Release).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit7).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoEdit4).EndInit();
		this.tabClose.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gridControl1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridView6).EndInit();
		base.ResumeLayout(false);
	}
}
