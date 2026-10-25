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
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
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

namespace Apzon.Forms.UserControls.BatchAndSerial;

public class ucBatchAndSerial : XtraUserControl
{
	private bool _isLoadLine;

	private readonly Dictionary<string, Common.Table_Code> _isBin = new Dictionary<string, Common.Table_Code>();

	private DataTable _oldBatchSerialLine = new DataTable();

	private Enums.FormViewModeType _formView = Enums.FormViewModeType.View;

	public DataBatchSerialChanged DBatchSerialChanged;

	private DataTable _batchAndSerialData = new DataTable();

	public readonly Dictionary<ApzObjectTypes, List<ApzObjectTypes>> CopyObjectNotBatchAndSerial;

	private const string KeyDefaultBin = "APZBINDEF";

	private DataTable _binAllowData = new DataTable();

	private IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	public bool IsBinAllowToWhs;

	private const string TitleAllowcateBin = "Bin Location Allocation";

	private const string TitleAllowcateBinToWhs = "Bin Location Allocation To Warehouse";

	private const string TitleAllowcateBatchAndSerial = "Batch/Serial Number";

	private int _tranBinAllowcateType = 1;

	private int _absEntry = -1;

	public bool RaiseLineItemChanged = true;

	private bool _chooseBatchSerial;

	private IContainer components;

	protected GridColumnEx gc_Batch_Qty;

	protected GridColumnEx gc_Batch_VaribleQty;

	protected GridColumnEx gc_Bath_MnfSerial;

	protected GridColumnEx gc_Batch_LotNumber;

	protected GridColumnEx gc_Batch_Status;

	protected GridColumnEx gc_Batch_ExpDate;

	protected GridColumnEx gc_Batch_MnfDate;

	protected GridColumnEx gc_Batch_InDate;

	protected GridColumnEx gc_Batch_Location;

	protected GridColumnEx gc_Batch_Notes;

	private ContextMenuStrip contextMenuStrip_Batch;

	private ToolStripMenuItem batcheDeleteRowToolStripMenuItem;

	private ToolStripMenuItem batchDuplicateRowToolStripMenuItem;

	private ToolStripMenuItem automaticCreationToolStripMenuItem;

	private RepositoryItemButtonEdit rBatchNo;

	private RepositoryItemCalcEdit rBatch_Cal;

	private RepositoryItemLookUpEdit rBatch_Status;

	private RepositoryItemDateEdit rBatch_Date;

	public TextEditEx txtBatchNo;

	public LabelLine lbBatchNo;

	private SimpleButtonEx btnRotate;

	public GridControlEx grdBatches;

	private ToolStripMenuItem autoSelectToolStripMenuItem;

	public GridViewEx gvBatches;

	private SimpleButtonEx btnHide;

	private GridColumnEx gc_BinLocation;

	private RepositoryItemButtonEdit rBinCode;

	private ToolStripMenuItem allowcateSerialBatchToWhsToolStripMenuItem;

	private RepositoryItemButtonEditCFL rBinAllowBinCode;

	private GridViewEx gvBinAllowcate;

	private GridColumnEx gcBinAllowBinCode;

	private GridColumnEx gcBinAllowQty;

	private GridColumnEx gcBinAllowAvailQty;

	private GridColumnEx gcBinAllowCreateDate;

	private GridColumnEx gcBinAllowDetail;

	private ContextMenuStrip contextMenuStrip_Bin;

	private ToolStripMenuItem binAllowDeleteRowToolStripMenuItem;

	private LabelLine lbTitle;

	private ToolStripMenuItem autoSelectToolBin;

	private CalcEditEx txtOpenQty;

	private LabelLine lbOpenQty;

	protected GridColumnEx gc_Batch_Batch;

	[DefaultValue("DocDate")]
	public string FieldPostingDate { get; set; }

	[DefaultValue(Enums.BatchSerialTransType.In)]
	public Enums.BatchSerialTransType TransType { get; set; }

	[DefaultValue(false)]
	public bool InBatchFromWhs { get; set; }

	[DefaultValue("ItemCode")]
	public string FieldItemCode { get; set; }

	[DefaultValue("Dscription")]
	public string FieldItemName { get; set; }

	[DefaultValue("WhsCode")]
	public string FieldWhsCode { get; set; }

	[DefaultValue("")]
	public string FieldToTranferWhsCode { get; set; }

	[DefaultValue("LineNum")]
	public string FieldLineNum { get; set; }

	[DefaultValue("Quantity")]
	public string FieldQuantity { get; set; }

	[DefaultValue("ManBtchSerial")]
	public string FieldType { get; set; }

	[DefaultValue("BaseType")]
	public string FieldBaseObject { get; set; }

	[DefaultValue("ItemType")]
	public string FieldItemType { get; set; }

	public string FieldLineType { get; set; }

	[DefaultValue(false)]
	public bool CheckNoInvtryMv { get; set; }

	public GridViewEx GvLineItem { get; set; }

	public GridViewEx GvHeader { get; set; }

	[DefaultValue("CardCode")]
	public string FieldBpCode { get; set; }

	[DefaultValue(false)]
	public bool GetDataFormDelivery { get; set; }

	[DefaultValue(false)]
	public bool FromReserve { get; set; }

	public string FormId
	{
		get
		{
			return gvBatches.FormId;
		}
		set
		{
			GridViewEx gridViewEx = gvBatches;
			string formId = (gvBinAllowcate.FormId = value);
			gridViewEx.FormId = formId;
		}
	}

	public string ItemId
	{
		get
		{
			return gvBatches.ItemId;
		}
		set
		{
			gvBatches.ItemId = value;
		}
	}

	[DefaultValue("OBIN")]
	public string ItemIdBin
	{
		get
		{
			return gvBinAllowcate.ItemId;
		}
		set
		{
			gvBinAllowcate.ItemId = value;
		}
	}

	[DefaultValue(ApzObjectTypes.oNone)]
	public ApzObjectTypes FormObjectType { get; set; }

	public DataTable DataSourceBatchAndSerial
	{
		private get
		{
			return _batchAndSerialData;
		}
		set
		{
			_batchAndSerialData = value.Copy();
			grdBatches.DataSource = value.Clone();
			try
			{
				if ((TransType == Enums.BatchSerialTransType.Allocate || TransType == Enums.BatchSerialTransType.InAllocate || TransType == Enums.BatchSerialTransType.OutAlocate) && GlobalData.LoginSap)
				{
					_oldBatchSerialLine = value.Copy();
				}
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
				WinCommons.ErrorLineNumber(ex);
			}
		}
	}

	public DataTable DataSourceBinAllowLine
	{
		private get
		{
			return _binAllowData;
		}
		set
		{
			_binAllowData = value;
			grdBatches.DataSource = _binAllowData.Clone();
		}
	}

	public bool IsAllowcateBinLocationToWhs
	{
		get
		{
			return _tranBinAllowcateType == 2;
		}
		set
		{
			if (FormObjectType == ApzObjectTypes.oStockTransfer)
			{
				FinishEditorBatchAndSerial();
				_tranBinAllowcateType = ((!value) ? 1 : 2);
				SetFocusLine();
			}
		}
	}

	public ucBatchAndSerial()
	{
		FieldPostingDate = "DocDate";
		FieldItemCode = "ItemCode";
		FieldItemName = "Dscription";
		FieldWhsCode = "WhsCode";
		FieldLineNum = "LineNum";
		FieldQuantity = "Quantity";
		FieldType = "ManBtchSerial";
		FieldToTranferWhsCode = "";
		FieldBaseObject = "BaseType";
		FieldItemType = "ItemType";
		CheckNoInvtryMv = false;
		InBatchFromWhs = false;
		TransType = Enums.BatchSerialTransType.In;
		FieldBpCode = "CardCode";
		GetDataFormDelivery = false;
		InitializeComponent();
		CopyObjectNotBatchAndSerial = new Dictionary<ApzObjectTypes, List<ApzObjectTypes>>
		{
			{
				ApzObjectTypes.oPurchaseInvoices,
				new List<ApzObjectTypes> { ApzObjectTypes.oPurchaseDeliveryNotes }
			},
			{
				ApzObjectTypes.oInvoices,
				new List<ApzObjectTypes> { ApzObjectTypes.oDeliveryNotes }
			},
			{
				ApzObjectTypes.oCreditNotes,
				new List<ApzObjectTypes>
				{
					ApzObjectTypes.oReturns,
					ApzObjectTypes.oReserveInvoices,
					ApzObjectTypes.oDownPayments
				}
			},
			{
				ApzObjectTypes.oPurchaseCreditNotes,
				new List<ApzObjectTypes>
				{
					ApzObjectTypes.oPurchaseReturns,
					ApzObjectTypes.oAPReserveInvoice,
					ApzObjectTypes.oPurchaseDownPayments
				}
			}
		};
	}

	private void txtBatchNo_EditValueChanged(object sender, EventArgs e)
	{
	}

	private void txtBatchNo_KeyDown(object sender, KeyEventArgs e)
	{
		if (GvLineItem == null)
		{
			return;
		}
		try
		{
			if (e.KeyCode == Keys.Return && (TransType == Enums.BatchSerialTransType.Out || TransType == Enums.BatchSerialTransType.OutAlocate))
			{
				LoadBatchAndSerialValueChangeOutput(-1, Function.ToString(txtBatchNo.EditValue));
			}
			else if (e.KeyCode == Keys.Return && TransType == Enums.BatchSerialTransType.In)
			{
				int openquantity = Function.ParseInt(txtOpenQty.Text);
				LoadBatchAndSerialInput(Function.ToString(txtBatchNo.EditValue), openquantity);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadBatchAndSerialInput(string serial, int Openquantity)
	{
		if (!string.IsNullOrEmpty(serial) && Openquantity > 0)
		{
			int num = gvBatches.RowCount - 1;
			if (Function.ToString(gvBatches.GetRowCellValue(gvBatches.RowCount - 1, "DistNumber")) != "")
			{
				DataTable dataTable = grdBatches.DatasourceTable();
				DataRow row = dataTable.NewRow();
				dataTable.Rows.Add(row);
				num = gvBatches.GetRowHandle(dataTable.Rows.IndexOf(row));
			}
			txtBatchNo.EditValue = null;
			gvBatches.FocusedRowHandle = num;
			gvBatches.SetRowCellValue(num, "DistNumber", serial);
		}
	}

	private void grdBatches_EditorKeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (GvLineItem == null || e.KeyData == Keys.Tab || e.KeyCode != Keys.V || e.Modifiers != Keys.Control)
			{
				return;
			}
			if (grdBatches.MainView == gvBatches)
			{
				List<string> list = new List<string> { gc_Batch_Batch.FieldName };
				if (Function.ToString(GvLineItem.GetFocusedRowCellValue(FieldType)).Equals("S"))
				{
					list.Add(gc_Bath_MnfSerial.FieldName);
					list.Add(gc_Batch_LotNumber.FieldName);
				}
				ExtenseionMethods.PasteFromExcelToDataTable(grdBatches, gvBatches, list);
			}
			else
			{
				List<string> key = new List<string> { gcBinAllowBinCode.FieldName };
				ExtenseionMethods.PasteFromExcelToDataTable(grdBatches, gvBinAllowcate, key);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ucBatchAndSerial_Load(object sender, EventArgs e)
	{
		try
		{
			if (DesignMode)
			{
				return;
			}
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL = rBinAllowBinCode;
			repositoryItemButtonEditCFL.BeforeShowCFL = (Action<frm_CFL>)Delegate.Combine(repositoryItemButtonEditCFL.BeforeShowCFL, (Action<frm_CFL>)((frm_CFL cfl) =>
			{
				cfl.WhsCode = ((_tranBinAllowcateType == 1) ? Function.ToString(GvLineItem.GetFocusedRowCellValue(FieldWhsCode)) : Function.ToString(GvLineItem.GetFocusedRowCellValue(FieldToTranferWhsCode)));
				cfl.ItemCode = Function.ToString(GvLineItem.GetFocusedRowCellValue(FieldItemCode));
				cfl.DataInt1 = (int)((_tranBinAllowcateType == 1) ? TransType : Enums.BatchSerialTransType.In);
			}));
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL2 = rBinAllowBinCode;
			repositoryItemButtonEditCFL2.HandleChooseItem = (Action<DataRow>)Delegate.Combine(repositoryItemButtonEditCFL2.HandleChooseItem, (Action<DataRow>)((DataRow chooseItem) =>
			{
				if (chooseItem != null)
				{
					gvBinAllowcate.SetFocusedRowCellValue(gcBinAllowBinCode, chooseItem["BinCode"]);
					gvBinAllowcate.SetFocusedRowCellValue("AbsEntry", chooseItem["AbsEntry"]);
					grdBatches.DataSourceTable.AcceptChanges();
					gvBinAllowcate.RefreshData();
				}
			}));
			GvLineItem.BeforeLeaveRow += Line_BeforeLeaveRow;
			GvLineItem.CellValueChanged += Line_CellValueChanged;
			if (GvHeader != null)
			{
				GvHeader.CellValueChanged += Header_CellValueChanged;
			}
			LoadBatchStatus();
			automaticCreationToolStripMenuItem.Visible = TransType == Enums.BatchSerialTransType.In || TransType == Enums.BatchSerialTransType.InAllocate;
			autoSelectToolStripMenuItem.Visible = TransType == Enums.BatchSerialTransType.Out || TransType == Enums.BatchSerialTransType.OutAlocate;
			btnHide.Visible = TransType == Enums.BatchSerialTransType.OutAlocate || TransType == Enums.BatchSerialTransType.InAllocate;
			gc_BinLocation.Visible = TransType == Enums.BatchSerialTransType.In || TransType == Enums.BatchSerialTransType.Out;
			Dictionary<string, GridViewEx> tblFormular = new Dictionary<string, GridViewEx>();
			if (GvHeader != null)
			{
				tblFormular[GvHeader.ItemId] = GvHeader;
			}
			if (GvLineItem != null)
			{
				tblFormular[GvLineItem.ItemId] = GvLineItem;
			}
			tblFormular["OBTN"] = gvBatches;
			Task.Run(() => WinFuntions.LoadUdfColumns(gvBatches, "OBTN", tblFormular)).Wait();
			dynamic val = Parent;
			if (val != null)
			{
				val = val.Parent;
			}
			if ((TransType == Enums.BatchSerialTransType.OutAlocate || TransType == Enums.BatchSerialTransType.InAllocate || TransType == Enums.BatchSerialTransType.None || !GvLineItem.GridControl.DataSourceTable().IsNotNull()) && val is SplitContainerControl)
			{
				val.PanelVisibility = SplitPanelVisibility.Panel1;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btnRotate_Click(object sender, EventArgs e)
	{
		try
		{
			dynamic val = Parent;
			if (val != null)
			{
				val = val.Parent;
			}
			if (val is SplitContainerControl)
			{
				dynamic val2 = val.Size;
				val.Horizontal = !val.Horizontal;
				if (val.Horizontal)
				{
					val.SplitterPosition = val2.Width / 2;
				}
				else
				{
					val.SplitterPosition = val2.Height / 2 + 20;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btnHide_Click(object sender, EventArgs e)
	{
		try
		{
			if (grdBatches.MainView == gvBinAllowcate)
			{
				if (_tranBinAllowcateType == 2)
				{
					IsAllowcateBinLocationToWhs = false;
				}
				return;
			}
			dynamic val = Parent;
			if (val != null)
			{
				val = val.Parent;
			}
			if (val is SplitContainerControl)
			{
				val.PanelVisibility = SplitPanelVisibility.Panel1;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvBatches_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (e.RowHandle == gvBatches.FocusedRowHandle)
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

	private void gvBatches_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (Function.ToString(GvLineItem.GetFocusedRowCellValue("LineStatus")) == "C")
			{
				e.Cancel = true;
				return;
			}
			if ((TransType == Enums.BatchSerialTransType.Out || TransType == Enums.BatchSerialTransType.OutAlocate) && Function.ParseDouble(GvLineItem.GetFocusedRowCellValue(FieldQuantity)) > 0.0 && gvBatches.FocusedColumn != gc_Batch_Batch && gvBatches.FocusedColumn != gc_Batch_Qty && gvBatches.FocusedColumn != gc_BinLocation)
			{
				e.Cancel = true;
				return;
			}
			if (string.IsNullOrEmpty(Function.ToString(GvLineItem.GetFocusedRowCellValue(FieldItemCode))))
			{
				WinCommons.Warning(Enums.MessageCode.ItemCodeCannotBeNull, "");
				e.Cancel = true;
				return;
			}
			if (string.IsNullOrEmpty(Function.ToString(GvLineItem.GetFocusedRowCellValue(FieldWhsCode))))
			{
				WinCommons.Warning(Enums.MessageCode.WarehouseCannotBeNull, "");
				e.Cancel = true;
				return;
			}
			ApzObjectTypes apzObjectTypes = ((!string.IsNullOrEmpty(FieldBaseObject)) ? ((ApzObjectTypes)Function.ParseInt(GvLineItem.GetFocusedRowCellValue(FieldBaseObject))) : ApzObjectTypes.oNone);
			if (FromReserve)
			{
				switch (apzObjectTypes)
				{
				case ApzObjectTypes.oInvoices:
					apzObjectTypes = ApzObjectTypes.oReserveInvoices;
					break;
				case ApzObjectTypes.oPurchaseInvoices:
					apzObjectTypes = ApzObjectTypes.oAPReserveInvoice;
					break;
				}
			}
			if (CopyObjectNotBatchAndSerial.ContainsKey(FormObjectType) && CopyObjectNotBatchAndSerial[FormObjectType].Contains(apzObjectTypes))
			{
				WinCommons.Error(Enums.MessageCode.BatchSerialNumberInfomationDefinedUpdate, "");
				e.Cancel = true;
			}
			else if (gvBatches.FocusedColumn.FieldName.Equals(gc_BinLocation.FieldName))
			{
				if (TransType == Enums.BatchSerialTransType.None || TransType == Enums.BatchSerialTransType.InAllocate || TransType == Enums.BatchSerialTransType.Allocate || TransType == Enums.BatchSerialTransType.OutAlocate)
				{
					e.Cancel = true;
					return;
				}
				string text = Function.ToString(GvLineItem.GetFocusedRowCellValue(FieldWhsCode));
				if (!CheckWhsManBinLocation(text))
				{
					WinCommons.Warning(Enums.MessageCode.WarehouseNotSetupFollowBin, "", text);
					e.Cancel = true;
				}
				else if (Function.ToString(GvLineItem.GetFocusedRowCellValue("LineStatus")) == "C" || (_formView != Enums.FormViewModeType.Add && FormObjectType != ApzObjectTypes.oOrders && FormObjectType != ApzObjectTypes.oPurchaseOrders && FormObjectType != ApzObjectTypes.oReturnRequest && FormObjectType != ApzObjectTypes.oGoodsReturnRequest && FormObjectType != ApzObjectTypes.oInventoryTransferRequest && FormObjectType != ApzObjectTypes.oProductionOrders))
				{
					e.Cancel = true;
				}
			}
			else if (gvBatches.FocusedColumn.FieldName.Equals(gc_Batch_Status.FieldName) && Function.ToString(gvBatches.GetFocusedRowCellValue("IsExits")).Equals("Y"))
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

	private void gvBatches_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (DBatchSerialChanged != null)
			{
				DBatchSerialChanged();
			}
			DataTable dataSourceTable = grdBatches.DataSourceTable;
			if (e.Column.FieldName == gc_Batch_Qty.FieldName)
			{
				dataSourceTable.AcceptChanges();
				CalculateOpenQty();
				double num = Math.Abs(Function.ParseDouble(GvLineItem.GetFocusedRowCellValue(FieldQuantity))) - Function.ParseDouble(dataSourceTable.Compute("SUM(LogQty)", ""));
				if (Function.ParseDouble(e.Value) <= 0.0)
				{
					WinCommons.Warning(Enums.MessageCode.InQtyValueGreater0, "");
					gvBatches.FocusedColumn = gc_Batch_Qty;
					gvBatches.ShowEditor();
					return;
				}
				if (num < 0.0)
				{
					WinCommons.Warning(Enums.MessageCode.AllocationBatchSerialQtyGreaterThanItemQty, "");
					gvBatches.FocusedColumn = gc_Batch_Qty;
					gvBatches.ShowEditor();
					return;
				}
			}
			else if (e.Column.FieldName == gc_Batch_Batch.FieldName || (Function.ToString(GvLineItem.GetFocusedRowCellValue(FieldType)).Equals("S") && (e.Column.FieldName == gc_Bath_MnfSerial.FieldName || e.Column.FieldName == gc_Batch_LotNumber.FieldName)))
			{
				AllowcateDeleteBatch(gvBatches.GetDataRow(e.RowHandle));
				if (!string.IsNullOrEmpty(Function.ToString(e.Value)))
				{
					if (((TransType == Enums.BatchSerialTransType.Out || TransType == Enums.BatchSerialTransType.OutAlocate) && Function.ParseDouble(GvLineItem.GetFocusedRowCellValue(FieldQuantity)) > 0.0) || ((TransType == Enums.BatchSerialTransType.In || TransType == Enums.BatchSerialTransType.InAllocate) && Function.ParseDouble(GvLineItem.GetFocusedRowCellValue(FieldQuantity)) < 0.0))
					{
						LoadBatchAndSerialValueChangeOutput(e.RowHandle, Function.ToString(gvBatches.GetRowCellValue(e.RowHandle, gc_Batch_Batch)));
					}
					else if (((TransType == Enums.BatchSerialTransType.In || TransType == Enums.BatchSerialTransType.InAllocate) && Function.ParseDouble(GvLineItem.GetFocusedRowCellValue(FieldQuantity)) > 0.0) || ((TransType == Enums.BatchSerialTransType.Out || TransType == Enums.BatchSerialTransType.OutAlocate) && Function.ParseDouble(GvLineItem.GetFocusedRowCellValue(FieldQuantity)) < 0.0))
					{
						LoadBatchAndSerialValueChangeInput(e.RowHandle, Function.ToString(gvBatches.GetRowCellValue(e.RowHandle, gc_Batch_Batch)), Function.ToString(gvBatches.GetRowCellValue(e.RowHandle, gc_Bath_MnfSerial)), Function.ToString(gvBatches.GetRowCellValue(e.RowHandle, gc_Batch_LotNumber)));
					}
					AutoAllocateToWhs(gvBatches.GetDataRow(e.RowHandle));
				}
			}
			else if (e.Column.FieldName.Equals(gc_BinLocation.FieldName))
			{
				int dataSourceRowIndex = gvBatches.GetDataSourceRowIndex(e.RowHandle);
				string binCode = Function.ToString(gvBatches.GetFocusedRowCellValue("BinCode"));
				if (string.IsNullOrEmpty(binCode))
				{
					if (dataSourceTable.Rows.Count > dataSourceRowIndex)
					{
						dataSourceTable.Rows[dataSourceRowIndex]["BinCode"] = "";
						dataSourceTable.Rows[dataSourceRowIndex]["BinAbs"] = 0;
					}
				}
				else
				{
					string whsCode = Function.ToString(GvLineItem.GetFocusedRowCellValue(FieldWhsCode));
					int batchSerialAbs = Function.ParseInt(gvBatches.GetFocusedRowCellValue("AbsEntry"));
					string itemCode = Function.ToString(GvLineItem.GetFocusedRowCellValue(FieldItemCode));
					DataTable result = Task.Run(() => _httpClient.InventoryCountingBin.LoadAllBin(itemCode, whsCode, batchSerialAbs, (int)TransType, binCode)).Result;
					if (!result.IsNotNull())
					{
						dataSourceTable.Rows[dataSourceRowIndex]["BinCode"] = "";
						dataSourceTable.Rows[dataSourceRowIndex]["BinAbs"] = 0;
						WinCommons.Warning((TransType == Enums.BatchSerialTransType.In || TransType == Enums.BatchSerialTransType.InAllocate) ? Enums.MessageCode.BinDidNotExists : Enums.MessageCode.BatchSerialDidNotExistsInBin, "");
					}
					else if (dataSourceTable.Rows.Count > dataSourceRowIndex)
					{
						dataSourceTable.Rows[dataSourceRowIndex]["BinCode"] = result.Rows[0]["BinCode"];
						dataSourceTable.Rows[dataSourceRowIndex]["BinAbs"] = result.Rows[0]["AbsEntry"];
					}
				}
			}
			if (e.Column == gc_Batch_Batch || e.Column.FieldName == gc_Batch_Qty.FieldName)
			{
				if (e.RowHandle != gvBatches.FocusedRowHandle)
				{
					return;
				}
				if (Math.Abs(Function.ParseDouble(GvLineItem.GetFocusedRowCellValue(FieldQuantity))) - Function.ParseDouble(dataSourceTable.Compute("SUM(LogQty)", "")) > 0.0)
				{
					AddNewRowBatch();
				}
				else
				{
					CalculateOpenQty();
				}
				string text = Function.ToString(GvLineItem.GetFocusedRowCellValue(FieldType));
				string whsCode2 = Function.ToString(GvLineItem.GetFocusedRowCellValue(FieldWhsCode));
				string itemCode2 = Function.ToString(GvLineItem.GetFocusedRowCellValue(FieldItemCode));
				int absEntry = Function.ParseInt(gvBatches.GetRowCellValue(e.RowHandle, "AbsEntry"));
				if (CheckWhsManBinLocation(whsCode2) && (TransType == Enums.BatchSerialTransType.Out || TransType == Enums.BatchSerialTransType.In))
				{
					IBinLocMasterDataClient binLocMasterDataClient = _httpClient.BinLocMasterDataClient;
					string tranType;
					if (TransType != Enums.BatchSerialTransType.In)
					{
						tranType = ((TransType == Enums.BatchSerialTransType.Out) ? "O" : "");
					}
					else
					{
						tranType = "I";
					}
					DataTable binDefaultBatch = binLocMasterDataClient.GetBinDefaultBatch(tranType, whsCode2, itemCode2, absEntry, text.Equals("B") ? Function.ParseDouble(gvBatches.GetRowCellValue(e.RowHandle, "LogQty")) : 1.0);
					if (binDefaultBatch.IsNotNull())
					{
						if (text.Equals("B"))
						{
							object[] itemArray = gvBatches.GetDataRow(e.RowHandle).ItemArray;
							int dataSourceRowIndex2 = gvBatches.GetDataSourceRowIndex(e.RowHandle);
							foreach (DataRow row in binDefaultBatch.Rows)
							{
								DataRow dataRow2 = dataSourceTable.NewRow();
								dataRow2.ItemArray = itemArray;
								dataRow2["LogQty"] = row["Qty"];
								dataRow2["SelectedQty"] = row["OnHand"];
								dataRow2["BinCode"] = row["BinCode"];
								dataRow2["BinAbs"] = row["AbsEntry"];
								dataSourceTable.Rows.InsertAt(dataRow2, dataSourceRowIndex2++);
							}
							dataSourceTable.Rows.RemoveAt(dataSourceRowIndex2);
						}
						else
						{
							int dataSourceRowIndex3 = gvBatches.GetDataSourceRowIndex(e.RowHandle);
							dataSourceTable.Rows[dataSourceRowIndex3]["BinCode"] = binDefaultBatch.Rows[0]["BinCode"];
							dataSourceTable.Rows[dataSourceRowIndex3]["BinAbs"] = binDefaultBatch.Rows[0]["AbsEntry"];
						}
					}
					dataSourceTable.AcceptChanges();
					grdBatches.DataSource = dataSourceTable;
					grdBatches.RefreshDataSource();
				}
				if (FormObjectType == ApzObjectTypes.oStockTransfer)
				{
					string whsCode3 = Function.ToString(GvLineItem.GetFocusedRowCellValue(FieldToTranferWhsCode));
					if (CheckWhsManBinLocation(whsCode3))
					{
						DataRow dataRow3 = gvBatches.GetDataRow(e.RowHandle);
						if (grdBatches.DataSourceTable.Select("isnull(AbsEntry, 0) <> 0").Length == 0)
						{
							return;
						}
						DataRow[] array = _batchAndSerialData.Select(string.Format("BaseLineNumber = {0} and AbsEntry = {1} and TranType = 2", Function.ParseInt(GvLineItem.GetFocusedRowCellValue(FieldLineNum)), Function.ParseInt(dataRow3["AbsEntry"])));
						for (int num2 = 0; num2 < array.Length; num2++)
						{
							_batchAndSerialData.Rows.Remove(array[num2]);
						}
						DataTable binDefaultBatch2 = _httpClient.BinLocMasterDataClient.GetBinDefaultBatch("I", whsCode3, itemCode2, absEntry, Function.ParseDouble(gvBatches.GetRowCellValue(e.RowHandle, "LogQty")));
						if (binDefaultBatch2.IsNotNull())
						{
							DataRow dataRow4 = _batchAndSerialData.NewRow();
							dataRow4["ItemCode"] = dataRow3["ItemCode"];
							dataRow4["DistNumber"] = dataRow3["DistNumber"];
							dataRow4["MnfSerial"] = dataRow3["MnfSerial"];
							dataRow4["LotNumber"] = dataRow3["LotNumber"];
							dataRow4["LogQty"] = binDefaultBatch2.Rows[0]["Qty"];
							dataRow4["AbsEntry"] = dataRow3["AbsEntry"];
							dataRow4["SysNumber"] = dataRow3["SysNumber"];
							dataRow4["BaseLineNumber"] = dataRow3["BaseLineNumber"];
							dataRow4["ManBatchSerial"] = dataRow3["ManBatchSerial"];
							dataRow4["ObjType"] = dataRow3["ObjType"];
							dataRow4["IsExits"] = "Y";
							dataRow4["TranType"] = 2;
							dataRow4["BinAbs"] = binDefaultBatch2.Rows[0]["AbsEntry"];
							dataRow4["BinCode"] = binDefaultBatch2.Rows[0]["BinCode"];
							_batchAndSerialData.Rows.Add(dataRow4);
							_batchAndSerialData.AcceptChanges();
						}
					}
				}
			}
			dataSourceTable.AcceptChanges();
			grdBatches.DataSource = dataSourceTable;
			grdBatches.RefreshDataSource();
			AddNewRowBatch();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void batcheDeleteRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDeleteSelectedRow, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) != DialogResult.Yes)
			{
				return;
			}
			List<DataRow> listDataSourceSelectedRow = gvBatches.GetListDataSourceSelectedRow();
			DataTable dataSourceTable = grdBatches.DataSourceTable;
			for (int i = 0; i < listDataSourceSelectedRow.Count; i++)
			{
				if (FormObjectType == ApzObjectTypes.oStockTransfer)
				{
					AllowcateDeleteBatch(listDataSourceSelectedRow[i]);
				}
				dataSourceTable.Rows.Remove(listDataSourceSelectedRow[i]);
			}
			dataSourceTable.AcceptChanges();
			grdBatches.DataSource = dataSourceTable;
			grdBatches.RefreshDataSource();
			AddNewRowBatch();
			if (DBatchSerialChanged != null)
			{
				DBatchSerialChanged();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void batchDuplicateRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		DuplicateRowBatch();
		if (DBatchSerialChanged != null)
		{
			DBatchSerialChanged();
		}
	}

	private void rBatchNo_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				string text = Function.ToString(GvLineItem.GetFocusedRowCellValue(FieldItemCode));
				string text2 = Function.ToString(GvLineItem.GetFocusedRowCellValue(FieldWhsCode));
				string bpCode = ((GetDataFormDelivery && GvHeader != null && !string.IsNullOrEmpty(FieldBpCode)) ? Function.ToString(GvHeader.GetFocusedRowCellValue(FieldBpCode)) : "");
				if (string.IsNullOrEmpty(text))
				{
					WinCommons.Warning(Enums.MessageCode.YouMustChooseItemCodeBeforeChooseBatch, "");
					GvLineItem.Focus();
					GvLineItem.FocusedColumn = GvLineItem.Columns[FieldItemCode];
					GvLineItem.ShowEditor();
					return;
				}
				if (string.IsNullOrEmpty(text2))
				{
					WinCommons.Warning(Enums.MessageCode.YouMustChooseWhsBeforeChooseBatch, "");
					GvLineItem.Focus();
					GvLineItem.FocusedColumn = GvLineItem.Columns[FieldWhsCode];
					GvLineItem.ShowEditor();
					return;
				}
				DataTable dtCons = new DataTable();
				dtCons.Columns.Add("ItemCode", typeof(string));
				dtCons.Columns.Add("AbsEntry", typeof(int));
				dtCons.Columns.Add("SysNumber", typeof(int));
				dtCons.Columns.Add("DistNumber", typeof(string));
				dtCons.Columns.Add("LogQty", typeof(double));
				DataTable dataTable = grdBatches.DataSourceTable.Copy();
				dataTable.Rows.RemoveAt(gvBatches.GetFocusedDataSourceRowIndex());
				dataTable.AcceptChanges();
				DataTable dataBatchAndSerial = ((dataTable.Columns.Count > 0 && (dataTable.IsNotNull() || _batchAndSerialData.IsNotNull())) ? (from row in dataTable.AsEnumerable().Union(_batchAndSerialData.AsEnumerable()).AsEnumerable()
					group row by new
					{
						absEntry = Function.ParseInt(row["AbsEntry"]),
						sysNumber = Function.ParseInt(row["SysNumber"]),
						ItemCode = Function.ToString(row["ItemCode"]),
						disNumber = Function.ToString(row["DistNumber"])
					} into g
					select dtCons.LoadDataRow(new object[5]
					{
						g.Key.ItemCode,
						g.Key.absEntry,
						g.Key.sysNumber,
						g.Key.disNumber,
						Function.ParseDouble(g.Sum((DataRow r) => Function.ParseDouble(r["LogQty"])))
					}, fAcceptChanges: false)).CopyToDataTable() : dtCons);
				frm_CFL frm_CFL2 = ((!(Function.ToString(GvLineItem.GetFocusedRowCellValue("IsReturnComponent")) != "Y")) ? new frm_CFL(Enums.TypeCFL.BatchSerialForReturnComponents, true, Function.ToString(txtBatchNo.Text), "", Enums.ItemObjectType.NoItemType, null, "")
				{
					DataInt1 = Function.ParseInt(GvLineItem.GetFocusedRowCellValue("BaseEntry")),
					DataInt2 = Function.ParseInt(GvLineItem.GetFocusedRowCellValue("BaseLine")),
					WhsCode = text2,
					DataBatchAndSerial = dataBatchAndSerial
				} : new frm_CFL(Enums.TypeCFL.BatchSerialNumber, true, Function.ToString(txtBatchNo.Text), "", Enums.ItemObjectType.NoItemType, null, "")
				{
					ItemCode = text,
					WhsCode = text2,
					BpCode = bpCode,
					ObjType = Function.ToString((int)FormObjectType),
					DataBatchAndSerial = dataBatchAndSerial
				});
				frm_CFL2.ShowDialog(FindForm());
				if (frm_CFL2.ChooseItems == null || frm_CFL2.ChooseItems.Count == 0)
				{
					return;
				}
				gvBatches.BeginInsertRow();
				int focusedDataSourceRowIndex = gvBatches.GetFocusedDataSourceRowIndex();
				DataTable dataSourceTable = grdBatches.DataSourceTable;
				AllowcateDeleteBatch(dataSourceTable.Rows[focusedDataSourceRowIndex]);
				dataSourceTable.Rows.RemoveAt(focusedDataSourceRowIndex);
				for (int num = frm_CFL2.ChooseItems.Count - 1; num >= 0; num--)
				{
					if (frm_CFL2.ChooseItems[num] != null)
					{
						DataRow dataRow = dataSourceTable.NewRow();
						dataRow["IsExits"] = "N";
						dataRow[gc_Bath_MnfSerial.FieldName] = frm_CFL2.ChooseItems[num][gc_Bath_MnfSerial.FieldName];
						dataRow[gc_Batch_LotNumber.FieldName] = frm_CFL2.ChooseItems[num][gc_Batch_LotNumber.FieldName];
						dataSourceTable.Rows.InsertAt(dataRow, focusedDataSourceRowIndex);
						dataSourceTable.AcceptChanges();
						grdBatches.DataSource = dataSourceTable;
						int focusedRowHandle = gvBatches.FindRowHandleByDataRow(dataRow);
						gvBatches.FocusedRowHandle = focusedRowHandle;
						_absEntry = Function.ParseInt(frm_CFL2.ChooseItems[num]["AbsEntry"]);
						gvBatches.SetFocusedRowCellValue(gc_Batch_Batch, frm_CFL2.ChooseItems[num]["DistNumber"]);
					}
				}
				gvBatches.EndInsertRow();
				grdBatches.RefreshDataSource();
			}
			else if (e.Button.Index == 1)
			{
				string text3 = Function.ToString(GvLineItem.GetFocusedRowCellValue(FieldType));
				string text4 = Function.ToString(gvBatches.GetFocusedRowCellValue("AbsEntry"));
				if (string.IsNullOrEmpty(text4))
				{
					return;
				}
				if (text3.Equals("S"))
				{
					(FindForm() as XtraFormEx).DrillDown(ApzObjectTypes.oSerialNumberDetails, ApzObjectTypes.oSerialNumberDetails, text4);
				}
				else
				{
					(FindForm() as XtraFormEx).DrillDown(ApzObjectTypes.oBatchDetails, ApzObjectTypes.oBatchDetails, text4);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		gvBatches.EndInsertRow();
	}

	private void automaticCreationToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			GvLineItem.CloseEditor();
			GvLineItem.UpdateCurrentRow();
			GvLineItem.GridControl.DataSourceTable().AcceptChanges();
			string whsCode = Function.ToString(GvLineItem.GetFocusedRowCellValue(FieldWhsCode));
			string itemCode = Function.ToString(GvLineItem.GetFocusedRowCellValue(FieldItemCode));
			double createQty = Math.Abs(Function.ParseDouble(GvLineItem.GetFocusedRowCellValue(FieldQuantity))) - Function.ParseDouble(grdBatches.DataSourceTable.Compute($"Sum({gc_Batch_Qty.FieldName})", ""));
			frm_AutoCreationBatchSerial frm_AutoCreationBatchSerial2 = new frm_AutoCreationBatchSerial(Function.ToString(GvLineItem.GetFocusedRowCellValue(FieldItemCode)), Function.ToString(GvLineItem.GetFocusedRowCellValue(FieldItemName)), whsCode, "", createQty, Function.ToString(GvLineItem.GetFocusedRowCellValue(FieldType)));
			frm_AutoCreationBatchSerial2.ShowDialog(FindForm());
			if (frm_AutoCreationBatchSerial2.DialogResult != DialogResult.OK)
			{
				return;
			}
			DataTable dataTable = grdBatches.DataSourceTable();
			DeleteEmptyRow(dataTable);
			int num = -1;
			string value = "";
			if (TransType == Enums.BatchSerialTransType.In && CheckWhsManBinLocation(whsCode))
			{
				DataTable binDefault = _httpClient.BinLocMasterDataClient.GetBinDefault("I", whsCode, itemCode, 0.0);
				if (binDefault.IsNotNull())
				{
					num = Function.ParseInt(binDefault.Rows[0]["AbsEntry"]);
					value = Function.ToString(binDefault.Rows[0]["BinCode"]);
				}
			}
			for (int i = 0; i < frm_AutoCreationBatchSerial2.ListDistNumber.Count; i++)
			{
				string value2 = "0";
				int num2 = 0;
				string text = Function.ToString(GvLineItem.GetFocusedRowCellValue(FieldType));
				DataTable dataBatchSerial = _httpClient.BatchDetails.GetDataBatchSerial(frm_AutoCreationBatchSerial2.ListDistNumber[i], frm_AutoCreationBatchSerial2.ListMnfSerial[i], frm_AutoCreationBatchSerial2.ListLotNumber[i], Function.ToString(GvLineItem.GetFocusedRowCellValue(FieldItemCode)), text);
				if (dataBatchSerial.IsNotNull())
				{
					if (XtraMessageBoxEx.Show(text.Equals("S") ? Enums.MessageCode.InSerialNumberFieldEnterAlreadyExists : Enums.MessageCode.ConfirmContinuteBatchExists, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, frm_AutoCreationBatchSerial2.ListDistNumber[i]) == DialogResult.No)
					{
						continue;
					}
					value2 = Function.ToString(dataBatchSerial.Rows[0]["Status"]);
					num2 = Function.ParseInt(dataBatchSerial.Rows[0]["AbsEntry"]);
				}
				DataRow dataRow = dataTable.NewRow();
				dataRow["AbsEntry"] = num2;
				dataRow["DistNumber"] = frm_AutoCreationBatchSerial2.ListDistNumber[i];
				dataRow["MnfSerial"] = frm_AutoCreationBatchSerial2.ListMnfSerial[i];
				dataRow["LotNumber"] = frm_AutoCreationBatchSerial2.ListLotNumber[i];
				if (frm_AutoCreationBatchSerial2.ExpDate.HasValue)
				{
					dataRow["ExpDate"] = frm_AutoCreationBatchSerial2.ExpDate;
				}
				if (frm_AutoCreationBatchSerial2.MnfDate.HasValue)
				{
					dataRow["MnfDate"] = frm_AutoCreationBatchSerial2.MnfDate;
				}
				if (frm_AutoCreationBatchSerial2.InDate.HasValue)
				{
					dataRow["InDate"] = frm_AutoCreationBatchSerial2.InDate;
				}
				dataRow["Location"] = frm_AutoCreationBatchSerial2.BinLocation;
				dataRow["Notes"] = frm_AutoCreationBatchSerial2.Notes;
				dataRow["BaseLineNumber"] = Function.ParseInt(GvLineItem.GetFocusedRowCellValue(FieldLineNum));
				dataRow["ItemCode"] = Function.ToString(GvLineItem.GetFocusedRowCellValue(FieldItemCode));
				dataRow["ItemName"] = Function.ToString(GvLineItem.GetFocusedRowCellValue(FieldItemName));
				dataRow["UserSign"] = Function.ParseInt(GlobalData.UserSignLogin);
				dataRow["IsExits"] = "N";
				dataRow["TranType"] = 1;
				dataRow["LogQty"] = frm_AutoCreationBatchSerial2.LotQty;
				if (num > 0)
				{
					dataRow["BinAbs"] = num;
					dataRow["BinCode"] = value;
				}
				dataRow[gc_Batch_Status.FieldName] = value2;
				dataRow[gc_Batch_InDate.FieldName] = DateTime.Now.Date;
				dataRow["ObjType"] = (text.Equals("S") ? 10000045 : 10000044);
				dataTable.Rows.Add(dataRow);
			}
			dataTable.AcceptChanges();
			grdBatches.DataSource = dataTable;
			AddNewRowBatch();
			if (DBatchSerialChanged != null)
			{
				DBatchSerialChanged();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void contextMenuStrip_Batch_Opening(object sender, CancelEventArgs e)
	{
		try
		{
			bool enabled;
			if (Function.ToString(GvLineItem.GetFocusedRowCellValue("LineStatus")) == "C" || (_formView != Enums.FormViewModeType.Add && FormObjectType != ApzObjectTypes.oOrders && FormObjectType != ApzObjectTypes.oPurchaseOrders && FormObjectType != ApzObjectTypes.oReturnRequest && FormObjectType != ApzObjectTypes.oGoodsReturnRequest && FormObjectType != ApzObjectTypes.oInventoryTransferRequest && FormObjectType != ApzObjectTypes.oProductionOrders))
			{
				ToolStripMenuItem toolStripMenuItem = autoSelectToolStripMenuItem;
				ToolStripMenuItem toolStripMenuItem2 = autoSelectToolStripMenuItem;
				ToolStripMenuItem toolStripMenuItem3 = binAllowDeleteRowToolStripMenuItem;
				ToolStripMenuItem toolStripMenuItem4 = batcheDeleteRowToolStripMenuItem;
				bool flag = (batchDuplicateRowToolStripMenuItem.Enabled = false);
				bool flag3 = (toolStripMenuItem4.Enabled = flag);
				bool flag5 = (toolStripMenuItem3.Enabled = flag3);
				enabled = (toolStripMenuItem2.Enabled = flag5);
				toolStripMenuItem.Enabled = enabled;
			}
			else
			{
				ToolStripMenuItem toolStripMenuItem5 = autoSelectToolStripMenuItem;
				enabled = (autoSelectToolStripMenuItem.Enabled = true);
				toolStripMenuItem5.Enabled = enabled;
				ToolStripMenuItem toolStripMenuItem6 = binAllowDeleteRowToolStripMenuItem;
				ToolStripMenuItem toolStripMenuItem7 = batchDuplicateRowToolStripMenuItem;
				bool flag5 = (batcheDeleteRowToolStripMenuItem.Enabled = gvBatches.GetFocusedDataRow() != null);
				enabled = (toolStripMenuItem7.Enabled = flag5);
				toolStripMenuItem6.Enabled = enabled;
			}
			if (FormObjectType == ApzObjectTypes.oStockTransfer)
			{
				string whsCode = Function.ToString(GvLineItem.GetFocusedRowCellValue(FieldToTranferWhsCode));
				allowcateSerialBatchToWhsToolStripMenuItem.Visible = CheckWhsManBinLocation(whsCode);
			}
			if (_formView == Enums.FormViewModeType.View)
			{
				allowcateSerialBatchToWhsToolStripMenuItem.Text = "Batch/Serial transactions report to Whse";
			}
			else
			{
				allowcateSerialBatchToWhsToolStripMenuItem.Text = "Allocate Serial/Batch to Whse";
			}
			GvLineItem.GridControl.DatasourceTable().AcceptChanges();
			DataTable dataSourceTable = grdBatches.DataSourceTable;
			if (dataSourceTable == null || dataSourceTable.Rows.Count == 0)
			{
				return;
			}
			double num = Math.Abs(Function.ParseDouble(GvLineItem.GetFocusedRowCellValue(FieldQuantity))) - Function.ParseDouble(dataSourceTable.Compute($"Sum({gc_Batch_Qty.FieldName})", ""));
			ApzObjectTypes apzObjectTypes = ((!string.IsNullOrEmpty(FieldBaseObject)) ? ((ApzObjectTypes)Function.ParseInt(GvLineItem.GetFocusedRowCellValue(FieldBaseObject))) : ApzObjectTypes.oNone);
			if (FromReserve)
			{
				switch (apzObjectTypes)
				{
				case ApzObjectTypes.oInvoices:
					apzObjectTypes = ApzObjectTypes.oReserveInvoices;
					break;
				case ApzObjectTypes.oPurchaseInvoices:
					apzObjectTypes = ApzObjectTypes.oAPReserveInvoice;
					break;
				}
			}
			ToolStripMenuItem toolStripMenuItem8 = automaticCreationToolStripMenuItem;
			enabled = (autoSelectToolStripMenuItem.Enabled = autoSelectToolStripMenuItem.Enabled && num > 0.0 && (!CopyObjectNotBatchAndSerial.ContainsKey(FormObjectType) || !CopyObjectNotBatchAndSerial[FormObjectType].Contains(apzObjectTypes)));
			toolStripMenuItem8.Enabled = enabled;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void autoSelectToolStripMenuItem_Click(object sender, EventArgs e)
	{
		DoAutoSelect();
	}

	private void rBinCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index != 0)
			{
				return;
			}
			string whsCode = Function.ToString(GvLineItem.GetFocusedRowCellValue(FieldWhsCode));
			int batchSerialAbs = Function.ParseInt(gvBatches.GetFocusedRowCellValue("AbsEntry"));
			string itemCode = Function.ToString(GvLineItem.GetFocusedRowCellValue(FieldItemCode));
			string txtText = Function.ToString(gvBatches.GetFocusedRowCellValue("BinCode"));
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.Bin, Function.ToString(GvLineItem.GetFocusedRowCellValue(FieldType)).Equals("B"), txtText)
			{
				WhsCode = whsCode,
				ItemCode = itemCode,
				BatchSerialAbs = batchSerialAbs,
				BatchSerialTrantype = (int)TransType
			};
			frm_ChooseFromList2.ShowDialog(FindForm());
			if (Function.ToString(GvLineItem.GetFocusedRowCellValue(FieldType)).Equals("S"))
			{
				if (frm_ChooseFromList2.ChooseItem != null)
				{
					DataRow focusedDataRow = gvBatches.GetFocusedDataRow();
					focusedDataRow["BinAbs"] = (object)frm_ChooseFromList2.ChooseItem["AbsEntry"];
					focusedDataRow["BinCode"] = (object)frm_ChooseFromList2.ChooseItem["BinCode"];
					grdBatches.DataSourceTable.AcceptChanges();
					gvBatches.FocusedColumn = gc_Batch_Qty;
				}
			}
			else
			{
				if (frm_ChooseFromList2.ChooseItems == null || frm_ChooseFromList2.ChooseItems.Count <= 0)
				{
					return;
				}
				DataTable dataTable = grdBatches.DataSourceTable();
				bool flag = ((DataRowView)frm_ChooseFromList2.ChooseItems[0]).DataView.Table.Columns.Contains("OnHandQty") && (TransType == Enums.BatchSerialTransType.Out || TransType == Enums.BatchSerialTransType.OutAlocate);
				DataRow focusedDataRow2 = gvBatches.GetFocusedDataRow();
				int focusedDataSourceRowIndex = gvBatches.GetFocusedDataSourceRowIndex();
				for (int i = 0; i < frm_ChooseFromList2.ChooseItems.Count; i++)
				{
					DataRowView dataRowView = (DataRowView)frm_ChooseFromList2.ChooseItems[i];
					int num = Function.ParseInt(dataRowView["AbsEntry"]);
					string value = Function.ToString(dataRowView["BinCode"]);
					if (i == 0)
					{
						focusedDataRow2["BinAbs"] = num;
						focusedDataRow2["BinCode"] = value;
						if (Function.ParseDouble(focusedDataRow2[gc_Batch_Qty.FieldName]) == 0.0)
						{
							focusedDataRow2[gc_Batch_Qty.FieldName] = (flag ? Function.ParseDouble(dataRowView["OnHandQty"]) : Function.ParseDouble(focusedDataRow2[gc_Batch_Qty.FieldName]));
						}
					}
					else
					{
						DataRow dataRow = dataTable.NewRow();
						dataRow.ItemArray = focusedDataRow2.ItemArray;
						dataRow["BinAbs"] = num;
						dataRow["BinCode"] = value;
						dataRow[gc_Batch_Qty.FieldName] = (flag ? Function.ParseDouble(dataRowView["OnHandQty"]) : 0.0);
						dataTable.Rows.InsertAt(dataRow, focusedDataSourceRowIndex + 1);
					}
				}
				dataTable.AcceptChanges();
				grdBatches.DataSource = dataTable;
				gvBatches.FocusedColumn = gc_Batch_Qty;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void allowcateSerialBatchToWhsToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormObjectType != ApzObjectTypes.oStockTransfer)
			{
				return;
			}
			DataRow[] array = grdBatches.DataSourceTable.Select("isnull(AbsEntry, 0) <> 0");
			if (array.Length == 0)
			{
				WinCommons.Error(Enums.MessageCode.YouMustBeChooseBatchSerialBeforeAllocateBin, "");
				return;
			}
			DataRow[] array2 = _batchAndSerialData.Select($"BaseLineNumber = {Function.ParseInt(GvLineItem.GetFocusedRowCellValue(FieldLineNum))} and TranType = 2");
			frm_AllowcateSerialBatch frm_AllowcateSerialBatch2 = new frm_AllowcateSerialBatch(array.CopyToDataTable(), (array2.Length == 0) ? _batchAndSerialData.Clone() : array2.CopyToDataTable(), _formView == Enums.FormViewModeType.Add || _formView == Enums.FormViewModeType.Update, Function.ToString(GvLineItem.GetFocusedRowCellValue(FieldToTranferWhsCode)), Function.ToString(GvLineItem.GetFocusedRowCellValue(FieldType)), FormId);
			frm_AllowcateSerialBatch2.ShowDialog(FindForm());
			if (frm_AllowcateSerialBatch2.DialogResult == DialogResult.OK)
			{
				for (int i = 0; i < array2.Length; i++)
				{
					_batchAndSerialData.Rows.Remove(array2[i]);
				}
				for (int j = 0; j < frm_AllowcateSerialBatch2.DataBin.Rows.Count; j++)
				{
					_batchAndSerialData.ImportRow(frm_AllowcateSerialBatch2.DataBin.Rows[j]);
				}
				_batchAndSerialData.AcceptChanges();
				if (DBatchSerialChanged != null)
				{
					DBatchSerialChanged();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvBatches_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
	{
		try
		{
			if (FormObjectType == ApzObjectTypes.oStockTransfer && gvBatches.FocusedColumn.FieldName.Equals(gc_Batch_Batch.FieldName) && !string.IsNullOrEmpty(Function.ToString(gvBatches.GetFocusedRowCellValue(gc_Batch_Batch))))
			{
				AllowcateDeleteBatch(gvBatches.GetFocusedDataRow());
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void Line_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		if (!RaiseLineItemChanged)
		{
			return;
		}
		bool flag = Function.ToString(GvLineItem.GetRowCellValue(e.RowHandle, FieldType)).Equals("S") || Function.ToString(GvLineItem.GetRowCellValue(e.RowHandle, FieldType)).Equals("B");
		if (e.Column.FieldName.Equals(FieldItemCode) && e.RowHandle == GvLineItem.FocusedRowHandle)
		{
			IsAllowcateBinLocationToWhs = false;
		}
		else if (e.Column.FieldName.Equals(FieldToTranferWhsCode))
		{
			if (e.RowHandle == GvLineItem.FocusedRowHandle)
			{
				IsAllowcateBinLocationToWhs = false;
			}
			if (flag)
			{
				if (e.RowHandle == GvLineItem.FocusedRowHandle)
				{
					grdBatches.MainView = gvBatches;
					grdBatches.ContextMenuStrip = contextMenuStrip_Batch;
				}
				ClearAllocateBinToWhs(Function.ParseInt(GvLineItem.GetRowCellValue(e.RowHandle, FieldLineNum)));
				DataTable dataTable = grdBatches.DataSourceTable();
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					AutoAllocateToWhs(dataTable.Rows[i]);
				}
			}
			else
			{
				if (e.RowHandle == GvLineItem.FocusedRowHandle)
				{
					grdBatches.MainView = gvBinAllowcate;
					grdBatches.ContextMenuStrip = contextMenuStrip_Bin;
				}
				ClearAllocateBinForLine(Function.ParseInt(GvLineItem.GetRowCellValue(e.RowHandle, FieldLineNum)));
				AutoAllowcateBinToWhs(e.RowHandle);
				if (e.RowHandle == GvLineItem.FocusedRowHandle)
				{
					LoadBinAllowLine(Function.ParseInt(GvLineItem.GetRowCellValue(e.RowHandle, FieldLineNum)));
				}
			}
		}
		else if (e.Column.FieldName.Equals(FieldWhsCode))
		{
			if (e.RowHandle == GvLineItem.FocusedRowHandle)
			{
				IsAllowcateBinLocationToWhs = false;
			}
			if (!flag)
			{
				if (e.RowHandle == GvLineItem.FocusedRowHandle)
				{
					grdBatches.ContextMenuStrip = contextMenuStrip_Bin;
					grdBatches.MainView = gvBinAllowcate;
				}
				ClearAllocateBinForLine(Function.ParseInt(GvLineItem.GetRowCellValue(e.RowHandle, FieldLineNum)));
				string whsCode = Function.ToString(GvLineItem.GetRowCellValue(e.RowHandle, FieldWhsCode));
				if (e.RowHandle != GvLineItem.FocusedRowHandle)
				{
					return;
				}
				dynamic val = Parent;
				if (!((val != null) ? true : false))
				{
					return;
				}
				val = val.Parent;
				if (!(val is SplitContainerControl))
				{
					return;
				}
				if ((TransType != Enums.BatchSerialTransType.In && TransType != Enums.BatchSerialTransType.Out) || Function.ParseInt(GvLineItem.GetFocusedRowCellValue(FieldItemType)) != 4 || !CheckWhsManBinLocation(whsCode))
				{
					val.PanelVisibility = SplitPanelVisibility.Panel1;
					return;
				}
				lbTitle.Utext = ((_tranBinAllowcateType == 1) ? "Bin Location Allocation" : "Bin Location Allocation To Warehouse");
				val.PanelVisibility = SplitPanelVisibility.Both;
				AutoAllowcateBinToWhs(e.RowHandle);
				if (e.RowHandle == GvLineItem.FocusedRowHandle)
				{
					LoadBinAllowLine(Function.ParseInt(GvLineItem.GetRowCellValue(e.RowHandle, FieldLineNum)));
				}
				return;
			}
			if (TransType != Enums.BatchSerialTransType.In && TransType != Enums.BatchSerialTransType.InAllocate)
			{
				ClearLineDataSource();
				return;
			}
			string whsCode2 = Function.ToString(GvLineItem.GetRowCellValue(e.RowHandle, FieldWhsCode));
			DataTable dt = grdBatches.DataSourceTable.Copy();
			foreach (DataRow row in dt.Rows)
			{
				row["BinCode"] = "";
				row["BinAbs"] = 0;
			}
			Dictionary<string, double> dictionary = (from r in dt.AsEnumerable()
				group r by r["DistNumber"]).Select((IGrouping<object, DataRow> g) =>
			{
				DataRow dataRow3 = dt.NewRow();
				dataRow3["DistNumber"] = g.Key;
				dataRow3["LogQty"] = g.Sum((DataRow r) => Function.ParseDouble(r["LogQty"]));
				return dataRow3;
			}).ToDictionary((DataRow r) => Function.ToString(r["DistNumber"]), (DataRow r) => Function.ParseDouble(r["LogQty"]));
			DataTable dataTable2 = dt.Clone();
			foreach (DataRow row2 in dt.Rows)
			{
				string key = Function.ToString(row2["DistNumber"]);
				if (dictionary.ContainsKey(key))
				{
					row2["LogQty"] = dictionary[key];
					dataTable2.ImportRow(row2);
					dictionary.Remove(key);
				}
			}
			dataTable2.AcceptChanges();
			grdBatches.DataSource = dataTable2;
			grdBatches.RefreshDataSource();
			if (!CheckWhsManBinLocation(whsCode2))
			{
				return;
			}
			for (int num = 0; num < gvBatches.RowCount; num++)
			{
				if (Function.ToString(gvBatches.GetRowCellValue(num, "DistNumber")) != "")
				{
					gvBatches.SetRowCellValue(num, "DistNumber", gvBatches.GetRowCellValue(num, "DistNumber"));
				}
			}
		}
		else if (e.Column.FieldName.Equals(FieldQuantity))
		{
			if (!flag)
			{
				if (TransType == Enums.BatchSerialTransType.OutAlocate && Function.ParseDouble(GvLineItem.GetFocusedRowCellValue(FieldQuantity)) < 0.0)
				{
					if (e.RowHandle == GvLineItem.FocusedRowHandle)
					{
						ClearAllocateBinForLine(Function.ParseInt(GvLineItem.GetRowCellValue(e.RowHandle, FieldLineNum)));
					}
					dynamic val2 = Parent;
					if (val2 != null)
					{
						val2 = val2.Parent;
						if (val2 is SplitContainerControl)
						{
							val2.PanelVisibility = SplitPanelVisibility.Panel1;
						}
					}
				}
				else
				{
					AutoAllowcateBinToWhs(e.RowHandle);
					if (e.RowHandle == GvLineItem.FocusedRowHandle)
					{
						LoadBinAllowLine(Function.ParseInt(GvLineItem.GetRowCellValue(e.RowHandle, FieldLineNum)));
					}
				}
			}
			else if (e.RowHandle == GvLineItem.FocusedRowHandle)
			{
				AddNewRowBatch();
			}
		}
		else if (e.Column.FieldName.Equals("NoInvtryMv"))
		{
			if (Function.ToString(e.Value).Equals("Y"))
			{
				grdBatches.DataSourceTable.Rows.Clear();
				grdBatches.DataSourceTable.AcceptChanges();
			}
			SetFocusLine();
		}
	}

	private void Line_BeforeLeaveRow(object sender, RowAllowEventArgs e)
	{
		try
		{
			DataTable dataSourceTable = grdBatches.DataSourceTable;
			if (_isLoadLine || !dataSourceTable.IsNotNull())
			{
				return;
			}
			bool flag = grdBatches.MainView == gvBatches;
			if ((flag && dataSourceTable.AsEnumerable().FirstOrDefault((DataRow t) => (!string.IsNullOrEmpty(Function.ToString(t["DistNumber"])) || !string.IsNullOrEmpty(Function.ToString(t["MnfSerial"])) || !string.IsNullOrEmpty(Function.ToString(t["LotNumber"]))) && Function.ParseDouble(t["LogQty"]) == 0.0) != null) || (!flag && dataSourceTable.AsEnumerable().FirstOrDefault((DataRow t) => !string.IsNullOrEmpty(Function.ToString(t["BinCode"])) && Function.ParseDouble(t["LogQty"]) == 0.0) != null))
			{
				WinCommons.Error(Enums.MessageCode.AllocateBatchSerialMustBeGreater0, "");
				e.Allow = false;
				return;
			}
			double num;
			if (flag)
			{
				num = (Function.ToString(GvLineItem.GetFocusedRowCellValue(FieldType)).Equals("B") ? Function.ParseDouble(dataSourceTable.Compute("SUM(LogQty)", "isnull(DistNumber, '') <> ''")) : Function.ParseDouble(dataSourceTable.Compute("SUM(LogQty)", "isnull(DistNumber, '') <> '' or isnull(MnfSerial, '') <> '' or isnull(LotNumber, '') <> ''")));
			}
			else
			{
				num = Function.ParseDouble(dataSourceTable.Compute("SUM(LogQty)", "isnull(BinCode, '') <> ''"));
			}
			if (Math.Abs(Function.ParseDouble(GvLineItem.GetFocusedRowCellValue(FieldQuantity))) < num)
			{
				WinCommons.Error(Enums.MessageCode.AllocateQtyGreaterLineQty, "");
				e.Allow = false;
			}
			else if (flag)
			{
				var array = (from row in dataSourceTable.AsEnumerable()
					group row by new
					{
						DistNumber = Function.ToString(row["DistNumber"]),
						MnfSerial = Function.ToString(row["MnfSerial"]),
						LotNumber = Function.ToString(row["LotNumber"]),
						BinCode = Function.ToString(row["BinCode"])
					} into g
					where g.Count() > 1
					select new
					{
						g.Key.DistNumber,
						g.Key.BinCode
					}).ToArray();
				if (array.Any())
				{
					WinCommons.Error(Enums.MessageCode.DuplicateRowWithDisNumberAndBinCode, "", array[0].DistNumber, array[0].BinCode);
					e.Allow = false;
				}
			}
			else
			{
				var array2 = (from row in dataSourceTable.AsEnumerable()
					group row by new
					{
						BinCode = Function.ToString(row["BinCode"])
					} into g
					where g.Count() > 1
					select new { g.Key.BinCode }).ToArray();
				if (array2.Any())
				{
					WinCommons.Error(Enums.MessageCode.DuplicateRowWithBinCode, "", array2[0].BinCode);
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

	private void Header_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (e.Column.FieldName != FieldPostingDate || TransType == Enums.BatchSerialTransType.Out || TransType == Enums.BatchSerialTransType.OutAlocate)
			{
				return;
			}
			for (int i = 0; i < gvBatches.RowCount; i++)
			{
				gvBatches.SetRowCellValue(i, "InDate", Function.ParseDateTimes(e.Value));
			}
			foreach (DataRow row in _batchAndSerialData.Rows)
			{
				row["InDate"] = Function.ParseDateTimes(e.Value);
			}
			_batchAndSerialData.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvBinAllowcate_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (DBatchSerialChanged != null)
			{
				DBatchSerialChanged();
			}
			DataTable dataSourceTable = grdBatches.DataSourceTable;
			if (e.Column.FieldName == gcBinAllowQty.FieldName)
			{
				dataSourceTable.AcceptChanges();
				CalculateOpenQty();
				double num = Math.Abs(Function.ParseDouble(GvLineItem.GetFocusedRowCellValue(FieldQuantity))) - Function.ParseDouble(dataSourceTable.Compute("SUM(LogQty)", ""));
				if (Function.ParseDouble(e.Value) <= 0.0)
				{
					WinCommons.Warning(Enums.MessageCode.InQtyValueGreater0, "");
					gvBinAllowcate.FocusedColumn = gcBinAllowAvailQty;
					gvBinAllowcate.ShowEditor();
					return;
				}
				if (num < 0.0)
				{
					WinCommons.Warning(Enums.MessageCode.BinAllocateQtyGreaterThanItemQuantity, "");
					gvBinAllowcate.FocusedColumn = gcBinAllowQty;
					gvBinAllowcate.ShowEditor();
					return;
				}
			}
			else if (e.Column.FieldName == gcBinAllowBinCode.FieldName && !string.IsNullOrEmpty(Function.ToString(e.Value)))
			{
				LoadDataBinAllowcation(e.RowHandle);
			}
			dataSourceTable.AcceptChanges();
			grdBatches.DataSource = dataSourceTable;
			grdBatches.RefreshDataSource();
			AddNewRowBatch();
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
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDeleteSelectedRow, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				List<DataRow> listDataSourceSelectedRow = gvBinAllowcate.GetListDataSourceSelectedRow();
				DataTable dataSourceTable = grdBatches.DataSourceTable;
				for (int i = 0; i < listDataSourceSelectedRow.Count; i++)
				{
					dataSourceTable.Rows.Remove(listDataSourceSelectedRow[i]);
				}
				dataSourceTable.AcceptChanges();
				grdBatches.DataSource = dataSourceTable;
				grdBatches.RefreshDataSource();
				AddNewRowBatch();
				if (DBatchSerialChanged != null)
				{
					DBatchSerialChanged();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvBinAllowcate_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (gvBinAllowcate.FocusedColumn != gcBinAllowBinCode && gvBinAllowcate.FocusedColumn != gcBinAllowQty)
			{
				e.Cancel = true;
				return;
			}
			ApzObjectTypes apzObjectTypes = ((!string.IsNullOrEmpty(FieldBaseObject)) ? ((ApzObjectTypes)Function.ParseInt(GvLineItem.GetFocusedRowCellValue(FieldBaseObject))) : ApzObjectTypes.oNone);
			if (FromReserve)
			{
				switch (apzObjectTypes)
				{
				case ApzObjectTypes.oInvoices:
					apzObjectTypes = ApzObjectTypes.oReserveInvoices;
					break;
				case ApzObjectTypes.oPurchaseInvoices:
					apzObjectTypes = ApzObjectTypes.oAPReserveInvoice;
					break;
				}
			}
			if (CopyObjectNotBatchAndSerial.ContainsKey(FormObjectType) && CopyObjectNotBatchAndSerial[FormObjectType].Contains(apzObjectTypes))
			{
				WinCommons.Error(Enums.MessageCode.BinAllocationCanBeDefinedUpdate, "");
				e.Cancel = true;
			}
			else if (string.IsNullOrEmpty(Function.ToString(gvBinAllowcate.GetFocusedRowCellValue(gcBinAllowBinCode))) && !gvBinAllowcate.FocusedColumn.FieldName.Equals(gcBinAllowBinCode.FieldName))
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

	private void autoSelectToolBin_Click(object sender, EventArgs e)
	{
		DoAutoSelect();
	}

	private void contextMenuStrip_Bin_Opening(object sender, CancelEventArgs e)
	{
		try
		{
			autoSelectToolBin.Visible = !IsAllowcateBinLocationToWhs;
			autoSelectToolBin.Enabled = _formView == Enums.FormViewModeType.Add || (Function.ToString(GvLineItem.GetFocusedRowCellValue("LineStatus")) == "O" && (FormObjectType == ApzObjectTypes.oOrders || FormObjectType == ApzObjectTypes.oPurchaseOrders || FormObjectType == ApzObjectTypes.oReturnRequest || FormObjectType == ApzObjectTypes.oGoodsReturnRequest || FormObjectType == ApzObjectTypes.oInventoryTransferRequest));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void SearchBatchByInputString()
	{
		DataTable dataSourceTable = grdBatches.DataSourceTable;
		string text = txtBatchNo.Text;
		if (dataSourceTable == null || dataSourceTable.Rows.Count == 0 || string.IsNullOrEmpty(text))
		{
			return;
		}
		for (int i = 0; i < dataSourceTable.Rows.Count; i++)
		{
			string text2 = Function.ToString(dataSourceTable.Rows[i][gc_Batch_Batch.FieldName]);
			if (!string.IsNullOrEmpty(text2) && text2.Length >= text.Length && text2.Substring(0, text.Length).ToUpper().Contains(text.ToUpper()))
			{
				gvBatches.FocusedRowHandle = i;
				break;
			}
		}
	}

	private void LoadBatchAndSerialValueChangeOutput(int rowHandle, string disNumber)
	{
		txtBatchNo.EditValue = null;
		DataTable dataBatchAndSerial = _httpClient.Commons.GetDataBatchAndSerial(_absEntry, disNumber, Function.ToString(GvLineItem.GetFocusedRowCellValue(FieldItemCode)), Function.ToString(GvLineItem.GetFocusedRowCellValue(FieldWhsCode)));
		_absEntry = -1;
		string text = Function.ToString(GvLineItem.GetFocusedRowCellValue(FieldType));
		DataTable dataSourceTable = grdBatches.DataSourceTable;
		if (!dataBatchAndSerial.IsNotNull())
		{
			WinCommons.Error(text.Equals("S") ? Enums.MessageCode.InSerialFieldNotExists : Enums.MessageCode.InBatchNumberFieldNotExists, "");
			if (rowHandle >= 0)
			{
				dataSourceTable.Rows.RemoveAt(gvBatches.GetDataSourceRowIndex(rowHandle));
				if (dataSourceTable.Rows.Count == 0 || dataSourceTable.AsEnumerable().All((DataRow r) => Function.ToString(r["DistNumber"]) != ""))
				{
					AddNewRowBatch();
				}
			}
			dataSourceTable.AcceptChanges();
			grdBatches.RefreshDataSource();
			if (rowHandle == -1)
			{
				txtBatchNo.Focus();
			}
			return;
		}
		int num = 0;
		if (rowHandle == -1)
		{
			DataRow row = dataSourceTable.NewRow();
			dataSourceTable.Rows.InsertAt(row, 0);
		}
		else
		{
			num = gvBatches.GetDataSourceRowIndex(rowHandle);
		}
		dataSourceTable.Rows[num][gc_Batch_Qty.FieldName] = 0;
		if (dataBatchAndSerial.Rows.Count == 1)
		{
			SetDataInputToDataSource(num, dataBatchAndSerial.Rows[0], dataSourceTable, text);
		}
		else
		{
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.SerialDetail, multiSelect: true, "")
			{
				DataSerialDetail = dataBatchAndSerial
			};
			frm_ChooseFromList2.ShowDialog(FindForm());
			if (frm_ChooseFromList2.ChooseItems != null && frm_ChooseFromList2.ChooseItems.Count > 0)
			{
				SetDataInputToDataSource(num, frm_ChooseFromList2.ChooseItems[0], dataSourceTable, text);
				for (int num2 = 1; num2 < frm_ChooseFromList2.ChooseItems.Count; num2++)
				{
					DataRow row2 = dataSourceTable.NewRow();
					dataSourceTable.Rows.InsertAt(row2, num);
					SetDataInputToDataSource(num + 1, frm_ChooseFromList2.ChooseItems[num2], dataSourceTable, text);
				}
			}
			else
			{
				dataSourceTable.Rows.RemoveAt(num);
			}
		}
		dataSourceTable.AcceptChanges();
		grdBatches.RefreshDataSource();
		if (rowHandle == -1)
		{
			txtBatchNo.Focus();
		}
	}

	private void SetDataInputToDataSource(int index, DataRow sourceRow, DataTable dataSource, string objType)
	{
		DataRow dataRow = dataSource.Rows[index];
		string text = Function.ToString(GvLineItem.GetFocusedRowCellValue(FieldItemCode));
		dataRow["AbsEntry"] = sourceRow["AbsEntry"];
		dataRow["SysNumber"] = sourceRow["SysNumber"];
		dataRow["ObjType"] = (objType.Equals("S") ? Function.ToString(10000045) : Function.ToString(10000044));
		double num = 1.0;
		if (objType.Equals("S"))
		{
			dataRow[gc_Batch_Qty.FieldName] = 1;
			dataRow["SelectedQty"] = 1;
			num = 1.0;
		}
		else
		{
			double num2 = Math.Abs(Function.ParseDouble(GvLineItem.GetFocusedRowCellValue(FieldQuantity))) - Function.ParseDouble(dataSource.Compute($"sum({gc_Batch_Qty.FieldName})", ""));
			double num3 = Function.ParseDouble(sourceRow["InStock"]) - Function.ParseDouble(sourceRow["CommitQty"]);
			num = ((num2 >= num3) ? num3 : num2);
			dataRow[gc_Batch_Qty.FieldName] = num;
			dataRow["SelectedQty"] = num3;
		}
		dataRow[gc_Batch_Batch.FieldName] = sourceRow[gc_Batch_Batch.FieldName];
		dataRow[gc_Bath_MnfSerial.FieldName] = sourceRow[gc_Bath_MnfSerial.FieldName];
		dataRow[gc_Batch_LotNumber.FieldName] = sourceRow[gc_Batch_LotNumber.FieldName];
		dataRow["IsExits"] = "Y";
		dataRow["BaseLineNumber"] = Function.ParseInt(GvLineItem.GetFocusedRowCellValue(FieldLineNum));
		dataRow["ItemCode"] = text;
		dataRow["ItemName"] = Function.ToString(GvLineItem.GetFocusedRowCellValue(FieldItemName));
		dataRow["UserSign"] = Function.ParseInt(GlobalData.UserSignLogin);
		dataRow["TranType"] = 1;
		dataRow[gc_Batch_Status.FieldName] = sourceRow[gc_Batch_Status.FieldName];
		dataRow[gc_Batch_InDate.FieldName] = sourceRow[gc_Batch_InDate.FieldName];
		dataRow["ExpDate"] = sourceRow["ExpDate"];
		dataRow["MnfDate"] = sourceRow["MnfDate"];
		dataRow["CreateDate"] = sourceRow["CreateDate"];
		dataRow["Location"] = sourceRow["Location"];
		dataRow["Notes"] = sourceRow["Notes"];
		List<DataColumn> list = (from DataColumn t in sourceRow.Table.Columns
			where Function.ToString(t.ColumnName).StartsWith("U_")
			select t).ToList();
		for (int num4 = 0; num4 < list.Count; num4++)
		{
			if (dataRow.Table.Columns.Contains(list[num4].ColumnName))
			{
				dataRow[list[num4].ColumnName] = sourceRow[list[num4].ColumnName];
			}
		}
		string whsCode = Function.ToString(GvLineItem.GetFocusedRowCellValue(FieldWhsCode));
		if (CheckWhsManBinLocation(whsCode) && TransType == Enums.BatchSerialTransType.Out)
		{
			LoadDefaultBin(dataSource, index, text, whsCode, Function.ParseInt(sourceRow["AbsEntry"]), num);
		}
	}

	private void LoadDefaultBin(DataTable dataSource, int index, string itemCode, string whsCode, int absEntry, double qty)
	{
		DataTable binDefault = _httpClient.BinLocMasterDataClient.GetBinDefault("O", whsCode, itemCode, qty);
		if (binDefault.IsNotNull())
		{
			DataRow dataRow = dataSource.Rows[index];
			dataRow[gc_Batch_Qty.FieldName] = binDefault.Rows[0]["Qty"];
			dataRow["BinAbs"] = binDefault.Rows[0]["AbsEntry"];
			dataRow["BinCode"] = binDefault.Rows[0]["BinCode"];
			dataRow["SelectedQty"] = binDefault.Rows[0]["OnHand"];
			for (int i = 1; i < binDefault.Rows.Count; i++)
			{
				DataRow dataRow2 = dataSource.NewRow();
				dataRow2.ItemArray = dataRow.ItemArray;
				dataRow2[gc_Batch_Qty.FieldName] = binDefault.Rows[i]["Qty"];
				dataRow2["BinAbs"] = binDefault.Rows[i]["AbsEntry"];
				dataRow2["BinCode"] = binDefault.Rows[i]["BinCode"];
				dataRow2["SelectedQty"] = binDefault.Rows[i]["OnHand"];
				dataSource.Rows.InsertAt(dataRow2, index);
			}
		}
	}

	private void LoadBatchAndSerialValueChangeInput(int rowHandle, string disNumber, string mnfSerial, string lotNumber)
	{
		string itemCode = Function.ToString(GvLineItem.GetFocusedRowCellValue(FieldItemCode));
		string text = Function.ToString(GvLineItem.GetFocusedRowCellValue(FieldType));
		DataTable dataSourceTable = grdBatches.DataSourceTable;
		int dataSourceRowIndex = gvBatches.GetDataSourceRowIndex(rowHandle);
		DataTable dataBatchSerial = _httpClient.BatchDetails.GetDataBatchSerial(disNumber, mnfSerial, lotNumber, itemCode, text);
		if (dataBatchSerial.IsNotNull())
		{
			if (!InBatchFromWhs && Function.ToString(GvLineItem.GetFocusedRowCellValue("IsReturnComponent")) != "Y" && XtraMessageBoxEx.Show(text.Equals("S") ? Enums.MessageCode.InSerialNumberFieldEnterAlreadyExists : Enums.MessageCode.ConfirmContinuteBatchExists, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, disNumber) == DialogResult.No)
			{
				dataSourceTable.Rows.RemoveAt(dataSourceRowIndex);
				dataSourceTable.AcceptChanges();
				grdBatches.DataSource = dataSourceTable;
				grdBatches.RefreshDataSource();
				return;
			}
			dataSourceTable.Rows[dataSourceRowIndex]["AbsEntry"] = dataBatchSerial.Rows[0]["AbsEntry"];
			dataSourceTable.Rows[dataSourceRowIndex][gc_Batch_Status.FieldName] = dataBatchSerial.Rows[0]["Status"];
			dataSourceTable.Rows[dataSourceRowIndex]["IsExits"] = "Y";
		}
		else
		{
			dataSourceTable.Rows[dataSourceRowIndex][gc_Batch_Status.FieldName] = "0";
			dataSourceTable.Rows[dataSourceRowIndex]["IsExits"] = "N";
		}
		dataSourceTable.Rows[dataSourceRowIndex][gc_Batch_Qty.FieldName] = 0;
		double num = 1.0;
		if (text.Equals("S"))
		{
			num = 1.0;
			dataSourceTable.Rows[dataSourceRowIndex]["ObjType"] = Function.ToString(10000045);
		}
		else
		{
			num = Math.Abs(Function.ParseDouble(GvLineItem.GetFocusedRowCellValue(FieldQuantity))) - Function.ParseDouble(dataSourceTable.Compute($"sum({gc_Batch_Qty.FieldName})", ""));
			dataSourceTable.Rows[dataSourceRowIndex]["ObjType"] = Function.ToString(10000044);
		}
		dataSourceTable.Rows[dataSourceRowIndex][gc_Batch_Qty.FieldName] = num;
		dataSourceTable.Rows[dataSourceRowIndex][gc_Batch_InDate.FieldName] = ((GvHeader != null) ? Function.ParseDateTimes(GvHeader.GetFocusedRowCellValue(FieldPostingDate)) : DateTime.Now.Date);
		dataSourceTable.Rows[dataSourceRowIndex]["TranType"] = 1;
		dataSourceTable.Rows[dataSourceRowIndex]["BaseLineNumber"] = Function.ParseInt(GvLineItem.GetFocusedRowCellValue(FieldLineNum));
		dataSourceTable.Rows[dataSourceRowIndex]["ItemCode"] = Function.ToString(GvLineItem.GetFocusedRowCellValue(FieldItemCode));
		dataSourceTable.Rows[dataSourceRowIndex]["ItemName"] = Function.ToString(GvLineItem.GetFocusedRowCellValue(FieldItemName));
		dataSourceTable.Rows[dataSourceRowIndex]["UserSign"] = Function.ParseInt(GlobalData.UserSignLogin);
		Function.ToString(GvLineItem.GetFocusedRowCellValue(FieldWhsCode));
		int num2 = -1;
		string value = "";
		dataSourceTable.Rows[dataSourceRowIndex]["BinAbs"] = num2;
		dataSourceTable.Rows[dataSourceRowIndex][gc_BinLocation.FieldName] = value;
		dataSourceTable.AcceptChanges();
		grdBatches.RefreshDataSource();
	}

	private void AddNewRowBatch()
	{
		try
		{
			if (Function.ToString(GvLineItem.GetFocusedRowCellValue("LineStatus")) == "C" || (_formView != Enums.FormViewModeType.Add && FormObjectType != ApzObjectTypes.oOrders && FormObjectType != ApzObjectTypes.oPurchaseOrders && FormObjectType != ApzObjectTypes.oReturnRequest && FormObjectType != ApzObjectTypes.oGoodsReturnRequest && FormObjectType != ApzObjectTypes.oInventoryTransferRequest && FormObjectType != ApzObjectTypes.oProductionOrders))
			{
				CalculateOpenQty();
				return;
			}
			bool flag = Function.ToString(GvLineItem.GetFocusedRowCellValue(FieldType)).Equals("B") || Function.ToString(GvLineItem.GetFocusedRowCellValue(FieldType)).Equals("S");
			if (string.IsNullOrEmpty(Function.ToString(GvLineItem.GetFocusedRowCellValue(FieldItemCode))))
			{
				return;
			}
			CalculateOpenQty();
			if (Function.ParseDouble(txtOpenQty.EditValue) < 1E-07)
			{
				return;
			}
			if (flag)
			{
				grdBatches.ContextMenuStrip = contextMenuStrip_Batch;
				grdBatches.MainView = gvBatches;
			}
			else
			{
				grdBatches.ContextMenuStrip = contextMenuStrip_Bin;
				grdBatches.MainView = gvBinAllowcate;
			}
			DataTable dataTable = grdBatches.DataSourceTable;
			if (dataTable == null || dataTable.Columns.Count == 0)
			{
				dataTable = _batchAndSerialData.Clone();
			}
			if (dataTable.Columns.Count == 0)
			{
				return;
			}
			if (grdBatches.MainView == gvBatches)
			{
				DataRow[] array = (Function.ToString(GvLineItem.GetFocusedRowCellValue(FieldType)).Equals("B") ? dataTable.Select("isnull(DistNumber,'') = ''") : dataTable.Select("isnull(DistNumber, '') = '' and isnull(MnfSerial, '') = '' and isnull(LotNumber, '') = ''"));
				if (array.Length != 0)
				{
					for (int i = 0; i < array.Length - 1; i++)
					{
						dataTable.Rows.Remove(array[i]);
					}
					dataTable.AcceptChanges();
					grdBatches.DataSource = dataTable;
					return;
				}
				DataRow dataRow = dataTable.NewRow();
				dataRow["DistNumber"] = "";
				dataRow["MnfSerial"] = "";
				dataRow["LotNumber"] = "";
				dataRow["BaseLineNumber"] = Function.ParseInt(GvLineItem.GetFocusedRowCellValue(FieldLineNum));
				dataRow["ItemCode"] = Function.ToString(GvLineItem.GetFocusedRowCellValue(FieldItemCode));
				dataRow["ItemName"] = Function.ToString(GvLineItem.GetFocusedRowCellValue(FieldItemName));
				dataRow["UserSign"] = Function.ParseInt(GlobalData.UserSignLogin);
				dataRow["IsExits"] = "N";
				dataRow["LogQty"] = 0;
				dataRow["TranType"] = 1;
				dataRow["IsBase"] = "N";
				dataRow[gc_Batch_Status.FieldName] = "0";
				dataRow[gc_Batch_InDate.FieldName] = ((GvHeader != null) ? Function.ParseDateTimes(GvHeader.GetFocusedRowCellValue(FieldPostingDate)) : DateTime.Now.Date);
				dataRow["ObjType"] = Function.ToString(Function.ToString(GvLineItem.GetFocusedRowCellValue(FieldType)).Equals("S") ? 10000045 : 10000044);
				dataTable.Rows.Add(dataRow);
			}
			else
			{
				DataRow[] array2 = dataTable.Select("isnull(BinCode,'') = ''");
				if (array2.Length != 0)
				{
					for (int j = 0; j < array2.Length - 1; j++)
					{
						dataTable.Rows.Remove(array2[j]);
					}
					dataTable.AcceptChanges();
					grdBatches.DataSource = dataTable;
					return;
				}
				DataRow dataRow2 = dataTable.NewRow();
				dataRow2["BinCode"] = "";
				dataRow2["BaseLineNumber"] = Function.ParseInt(GvLineItem.GetFocusedRowCellValue(FieldLineNum));
				dataRow2["LogQty"] = 0;
				dataRow2["TranType"] = _tranBinAllowcateType;
				dataTable.Rows.Add(dataRow2);
			}
			dataTable.AcceptChanges();
			grdBatches.DataSource = dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void DuplicateRowBatch()
	{
		try
		{
			if (string.IsNullOrEmpty(Function.ToString(gvBatches.GetFocusedRowCellValue(gc_Batch_Batch))))
			{
				WinCommons.Warning(Enums.MessageCode.DataRowIsEmpty, "");
				return;
			}
			DataTable dataSourceTable = grdBatches.DataSourceTable;
			if (Function.ParseDouble(dataSourceTable.Compute($"Sum({gc_Batch_Qty.FieldName})", "")) + Function.ParseDouble(gvBatches.GetFocusedRowCellValue(gc_Batch_Qty)) > Math.Abs(Function.ParseDouble(GvLineItem.GetFocusedRowCellValue(FieldQuantity))))
			{
				WinCommons.Error(Enums.MessageCode.DecreaseBatchQty, "");
				return;
			}
			DataRow dataRow = dataSourceTable.NewRow();
			dataRow.ItemArray = gvBatches.GetFocusedDataRow().ItemArray;
			dataSourceTable.Rows.Add(dataRow);
			dataSourceTable.AcceptChanges();
			grdBatches.DataSource = dataSourceTable;
			CalculateOpenQty();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public void ClearDataSource()
	{
		try
		{
			_batchAndSerialData.Rows.Clear();
			_batchAndSerialData.AcceptChanges();
			_binAllowData.Rows.Clear();
			_binAllowData.AcceptChanges();
			grdBatches.DataSourceTable.Rows.Clear();
			grdBatches.DataSourceTable.AcceptChanges();
			_oldBatchSerialLine.Rows.Clear();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	public void ClearLineDataSource()
	{
		try
		{
			grdBatches.DataSourceTable.Rows.Clear();
			grdBatches.DataSourceTable.AcceptChanges();
			ClearAllocateBinToWhs(Function.ParseInt(GvLineItem.GetFocusedRowCellValue(FieldLineNum)));
			AddNewRowBatch();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void LoadBatchStatus()
	{
		try
		{
			List<Common.Table_Code> dataSource = new List<Common.Table_Code>
			{
				new Common.Table_Code
				{
					Name = "Released",
					Code = "0"
				},
				new Common.Table_Code
				{
					Name = "Not Accessible",
					Code = "1"
				},
				new Common.Table_Code
				{
					Name = "Locked",
					Code = "2"
				}
			};
			rBatch_Status.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public void SetFocusLine(bool isChangeItem = false)
	{
		if (_isLoadLine)
		{
			return;
		}
		grdBatches.DataSourceTable.Rows.Clear();
		grdBatches.DataSourceTable.AcceptChanges();
		dynamic val = Parent;
		if (val != null)
		{
			val = val.Parent;
		}
		string text = Function.ToString(GvLineItem.GetFocusedRowCellValue(FieldType));
		string whsCode = Function.ToString(GvLineItem.GetFocusedRowCellValue(FieldWhsCode));
		string whsCode2 = Function.ToString(GvLineItem.GetFocusedRowCellValue(FieldToTranferWhsCode));
		ApzObjectTypes apzObjectTypes = ((!string.IsNullOrEmpty(FieldBaseObject)) ? ((ApzObjectTypes)Function.ParseInt(GvLineItem.GetFocusedRowCellValue(FieldBaseObject))) : ApzObjectTypes.oNone);
		if (FromReserve)
		{
			switch (apzObjectTypes)
			{
			case ApzObjectTypes.oInvoices:
				apzObjectTypes = ApzObjectTypes.oReserveInvoices;
				break;
			case ApzObjectTypes.oPurchaseInvoices:
				apzObjectTypes = ApzObjectTypes.oAPReserveInvoice;
				break;
			}
		}
		if (TransType == Enums.BatchSerialTransType.None || Function.ParseInt(GvLineItem.GetFocusedRowCellValue(FieldItemType)) != 4 || (CheckNoInvtryMv && Function.ToString(GvLineItem.GetFocusedRowCellValue("NoInvtryMv")).Equals("Y")) || (CopyObjectNotBatchAndSerial.ContainsKey(FormObjectType) && CopyObjectNotBatchAndSerial[FormObjectType].Contains(apzObjectTypes)) || (!text.Equals("B") && !text.Equals("S") && ((TransType != Enums.BatchSerialTransType.In && TransType != Enums.BatchSerialTransType.Out) || (_tranBinAllowcateType == 1 && !CheckWhsManBinLocation(whsCode)) || (_tranBinAllowcateType == 2 && !CheckWhsManBinLocation(whsCode2)))))
		{
			if (val is SplitContainerControl)
			{
				val.PanelVisibility = SplitPanelVisibility.Panel1;
			}
		}
		else if (!string.IsNullOrEmpty(Function.ToString(GvLineItem.GetFocusedRowCellValue(FieldItemCode))))
		{
			if (text.Equals("B") || text.Equals("S"))
			{
				if (Function.ToString(GvLineItem.GetFocusedRowCellValue("IsReturnComponent")) == "Y")
				{
					rBatchNo.Buttons[0].Visible = true;
				}
				else
				{
					rBatchNo.Buttons[0].Visible = _chooseBatchSerial;
				}
				lbTitle.Utext = "Batch/Serial Number";
				grdBatches.ContextMenuStrip = contextMenuStrip_Batch;
				grdBatches.MainView = gvBatches;
				string itemId = gvBatches.ItemId2;
				if (text.Equals("B"))
				{
					gvBatches.ItemId2 = "OBTN";
					gc_Batch_Qty.Visible = true;
					gc_Batch_Status.Visible = true;
					gc_Batch_VaribleQty.Visible = true;
				}
				else
				{
					gvBatches.ItemId2 = "OSRN";
					gc_Batch_Qty.Visible = false;
					gc_Batch_Status.Visible = false;
					gc_Batch_VaribleQty.Visible = false;
				}
				if (gvBatches.ItemId2 != itemId)
				{
					gvBatches.ReloadFormSetting();
				}
				if (!GvLineItem.IsMappingContextAction && (TransType == Enums.BatchSerialTransType.In || TransType == Enums.BatchSerialTransType.Out) && val is SplitContainerControl)
				{
					dynamic val2 = val.Size;
					if (!(val.Horizontal ? true : false))
					{
						val.SplitterPosition = val2.Height / 2 + 20;
					}
					else
					{
						val.SplitterPosition = val2.Width / 2;
					}
					val.PanelVisibility = SplitPanelVisibility.Both;
				}
				LoadBatchNumber(Function.ParseInt(GvLineItem.GetFocusedRowCellValue(FieldLineNum)));
				return;
			}
			grdBatches.MainView = gvBinAllowcate;
			if (TransType != Enums.BatchSerialTransType.In && TransType != Enums.BatchSerialTransType.Out)
			{
				if (val is SplitContainerControl)
				{
					val.PanelVisibility = SplitPanelVisibility.Panel1;
				}
				return;
			}
			lbTitle.Utext = ((_tranBinAllowcateType == 1) ? "Bin Location Allocation" : "Bin Location Allocation To Warehouse");
			grdBatches.ContextMenuStrip = contextMenuStrip_Bin;
			if (!GvLineItem.IsMappingContextAction && val is SplitContainerControl)
			{
				dynamic val3 = val.Size;
				if (!(val.Horizontal ? true : false))
				{
					val.SplitterPosition = val3.Height / 2 + 20;
				}
				else
				{
					val.SplitterPosition = val3.Width / 2;
				}
				val.PanelVisibility = SplitPanelVisibility.Both;
			}
			if (isChangeItem)
			{
				AutoAllowcateBinToWhs(GvLineItem.FocusedRowHandle);
			}
			LoadBinAllowLine(Function.ParseInt(GvLineItem.GetFocusedRowCellValue(FieldLineNum)));
		}
		else if (val is SplitContainerControl)
		{
			val.PanelVisibility = SplitPanelVisibility.Panel1;
		}
	}

	public void SetVisibleAfterContextAction()
	{
		if (string.IsNullOrWhiteSpace(Function.ToString(GvLineItem.GetFocusedRowCellValue(FieldItemCode))))
		{
			return;
		}
		string text = Function.ToString(GvLineItem.GetFocusedRowCellValue(FieldType));
		string whsCode = Function.ToString(GvLineItem.GetFocusedRowCellValue(FieldWhsCode));
		string whsCode2 = Function.ToString(GvLineItem.GetFocusedRowCellValue(FieldToTranferWhsCode));
		ApzObjectTypes item = ((!string.IsNullOrEmpty(FieldBaseObject)) ? ((ApzObjectTypes)Function.ParseInt(GvLineItem.GetFocusedRowCellValue(FieldBaseObject))) : ApzObjectTypes.oNone);
		if (TransType == Enums.BatchSerialTransType.None || Function.ParseInt(GvLineItem.GetFocusedRowCellValue(FieldItemType)) != 4 || (CheckNoInvtryMv && Function.ToString(GvLineItem.GetFocusedRowCellValue("NoInvtryMv")).Equals("Y")) || (CopyObjectNotBatchAndSerial.ContainsKey(FormObjectType) && CopyObjectNotBatchAndSerial[FormObjectType].Contains(item)) || (!text.Equals("B") && !text.Equals("S") && ((TransType != Enums.BatchSerialTransType.In && TransType != Enums.BatchSerialTransType.Out) || (_tranBinAllowcateType == 1 && !CheckWhsManBinLocation(whsCode)) || (_tranBinAllowcateType == 2 && !CheckWhsManBinLocation(whsCode2)))))
		{
			return;
		}
		dynamic val = Parent;
		if (val != null)
		{
			val = val.Parent;
		}
		if ((text == "B" || text == "S") && (TransType == Enums.BatchSerialTransType.In || TransType == Enums.BatchSerialTransType.Out))
		{
			if (val is SplitContainerControl)
			{
				dynamic val2 = val.Size;
				if (!(val.Horizontal ? true : false))
				{
					val.SplitterPosition = val2.Height / 2 + 20;
				}
				else
				{
					val.SplitterPosition = val2.Width / 2;
				}
				val.PanelVisibility = SplitPanelVisibility.Both;
			}
		}
		else if ((TransType == Enums.BatchSerialTransType.In || TransType == Enums.BatchSerialTransType.Out) && val is SplitContainerControl)
		{
			dynamic val3 = val.Size;
			if (!(val.Horizontal ? true : false))
			{
				val.SplitterPosition = val3.Height / 2 + 20;
			}
			else
			{
				val.SplitterPosition = val3.Width / 2;
			}
			val.PanelVisibility = SplitPanelVisibility.Both;
		}
	}

	public void LoadBatchNumber(int docLine)
	{
		if (_batchAndSerialData == null || _batchAndSerialData.Columns.Count == 0)
		{
			WinCommons.Error(Enums.MessageCode.CanNotGetDataBatchNumber, "");
			return;
		}
		DataTable dataTable = _batchAndSerialData.Clone();
		if (string.IsNullOrEmpty(Function.ToString(GvLineItem.GetFocusedRowCellValue(FieldItemCode))))
		{
			grdBatches.DataSource = dataTable;
			grdBatches.RefreshDataSource();
			return;
		}
		DataRow[] array = _batchAndSerialData.Select($"BaseLineNumber = {docLine} and TranType = 1");
		for (int i = 0; i < array.Length; i++)
		{
			DataRow dataRow = dataTable.NewRow();
			dataRow.ItemArray = array[i].ItemArray;
			dataTable.Rows.Add(dataRow);
			_batchAndSerialData.Rows.Remove(array[i]);
		}
		_batchAndSerialData.AcceptChanges();
		dataTable.AcceptChanges();
		grdBatches.DataSource = dataTable;
		grdBatches.RefreshDataSource();
		AddNewRowBatch();
	}

	public void LoadBinAllowLine(int docLine)
	{
		if (_binAllowData == null || _binAllowData.Columns.Count == 0)
		{
			WinCommons.Error(Enums.MessageCode.CanNotGetDataBinAllocation, "");
			return;
		}
		DataTable dataTable = _binAllowData.Clone();
		if (string.IsNullOrEmpty(Function.ToString(GvLineItem.GetFocusedRowCellValue(FieldItemCode))))
		{
			grdBatches.DataSource = dataTable;
			grdBatches.RefreshDataSource();
			return;
		}
		DataRow[] array = _binAllowData.Select($"BaseLineNumber = {docLine} and TranType = {_tranBinAllowcateType}");
		for (int i = 0; i < array.Length; i++)
		{
			DataRow dataRow = dataTable.NewRow();
			dataRow.ItemArray = array[i].ItemArray;
			dataTable.Rows.Add(dataRow);
			_binAllowData.Rows.Remove(array[i]);
		}
		_binAllowData.AcceptChanges();
		dataTable.AcceptChanges();
		grdBatches.DataSource = dataTable;
		grdBatches.RefreshDataSource();
		AddNewRowBatch();
	}

	public Enums.MessageCode FinishEditorBatchAndSerial(bool isDraft = false)
	{
		if (_isLoadLine)
		{
			return Enums.MessageCode.Success;
		}
		Enums.MessageCode messageCode = UpdateBatchEditor(isDraft);
		DataTable dataSourceTable = grdBatches.DataSourceTable;
		if (dataSourceTable == null || (grdBatches.MainView == gvBatches && (dataSourceTable.Rows.Count == 0 || _batchAndSerialData == null || _batchAndSerialData.Columns.Count == 0)) || (grdBatches.MainView == gvBinAllowcate && (dataSourceTable.Rows.Count == 0 || _binAllowData == null || _binAllowData.Columns.Count == 0)))
		{
			return Enums.MessageCode.Success;
		}
		DataTable dataTable = ((grdBatches.MainView == gvBatches) ? _batchAndSerialData : _binAllowData);
		DataRow[] array = dataTable.Select(string.Format("BaseLineNumber = {0} AND TranType = {1}", Function.ParseInt(dataSourceTable.Rows[0]["BaseLineNumber"]), _tranBinAllowcateType));
		for (int i = 0; i < array.Length; i++)
		{
			dataTable.Rows.Remove(array[i]);
		}
		for (int j = 0; j < dataSourceTable.Rows.Count; j++)
		{
			if (grdBatches.MainView == gvBatches)
			{
				if (Function.ToString(dataSourceTable.Rows[j]["ObjType"]).Equals(Function.ToString(10000044)))
				{
					if (string.IsNullOrEmpty(Function.ToString(dataSourceTable.Rows[j][gc_Batch_Batch.FieldName])))
					{
						continue;
					}
				}
				else if (string.IsNullOrEmpty(Function.ToString(dataSourceTable.Rows[j][gc_Batch_Batch.FieldName])) && string.IsNullOrEmpty(Function.ToString(dataSourceTable.Rows[j][gc_Bath_MnfSerial.FieldName])) && string.IsNullOrEmpty(Function.ToString(dataSourceTable.Rows[j][gc_Batch_LotNumber.FieldName])))
				{
					continue;
				}
			}
			else if (string.IsNullOrEmpty(Function.ToString(dataSourceTable.Rows[j][gcBinAllowBinCode.FieldName])))
			{
				continue;
			}
			DataRow dataRow = dataTable.NewRow();
			dataRow.ItemArray = dataSourceTable.Rows[j].ItemArray;
			dataTable.Rows.Add(dataRow);
		}
		dataTable.AcceptChanges();
		if (messageCode != Enums.MessageCode.Success)
		{
			WinCommons.Warning(messageCode, "");
		}
		return messageCode;
	}

	public DataTable CheckDataBatch(DataTable dtLine, bool isDraft)
	{
		if (TransType == Enums.BatchSerialTransType.None)
		{
			return new DataTable();
		}
		if (FinishEditorBatchAndSerial(isDraft) != Enums.MessageCode.Success)
		{
			return null;
		}
		if (_batchAndSerialData == null || _batchAndSerialData.Columns.Count == 0 || dtLine == null || dtLine.Rows.Count == 0)
		{
			return new DataTable();
		}
		DataRow[] array = _batchAndSerialData.Select($"Isnull({gc_Batch_Batch.FieldName},'') = '' and Isnull({gc_Batch_LotNumber.FieldName},'') = '' and Isnull({gc_Bath_MnfSerial.FieldName},'') = ''");
		for (int i = 0; i < array.Length; i++)
		{
			_batchAndSerialData.Rows.Remove(array[i]);
		}
		_batchAndSerialData.AcceptChanges();
		DataTable dataTable = _batchAndSerialData.Copy();
		int j;
		for (j = 0; j < dtLine.Rows.Count; j++)
		{
			string text = ((!string.IsNullOrEmpty(FieldLineType)) ? Function.ToString(dtLine.Rows[j][FieldLineType]) : "R");
			string text2 = Function.ToString(dtLine.Rows[j][FieldType]);
			ApzObjectTypes apzObjectTypes = ((!string.IsNullOrEmpty(FieldBaseObject)) ? ((ApzObjectTypes)Function.ParseInt(dtLine.Rows[j][FieldBaseObject])) : ApzObjectTypes.oNone);
			if (FromReserve)
			{
				switch (apzObjectTypes)
				{
				case ApzObjectTypes.oInvoices:
					apzObjectTypes = ApzObjectTypes.oReserveInvoices;
					break;
				case ApzObjectTypes.oPurchaseInvoices:
					apzObjectTypes = ApzObjectTypes.oAPReserveInvoice;
					break;
				}
			}
			if (text != "R" || (!text2.Equals("B") && !text2.Equals("S")) || string.IsNullOrEmpty(Function.ToString(dtLine.Rows[j][FieldItemCode])) || (CopyObjectNotBatchAndSerial.ContainsKey(FormObjectType) && CopyObjectNotBatchAndSerial[FormObjectType].Contains(apzObjectTypes)))
			{
				continue;
			}
			if (Math.Abs(Function.ParseDouble(dtLine.Rows[j][FieldQuantity])) < 1E-07)
			{
				if (_batchAndSerialData.AsEnumerable().Any((DataRow t) => Function.ParseInt(t["BaseLineNumber"]) == Function.ParseInt(dtLine.Rows[j][FieldLineNum])))
				{
					WinCommons.Warning(Enums.MessageCode.CannotAddBatchSerialNumberRowWithQuantity0, "", Function.ToString(dtLine.Rows[j][FieldLineNum]));
					return null;
				}
				continue;
			}
			DataRow[] array2 = _batchAndSerialData.Select($"BaseLineNumber = {Function.ParseInt(dtLine.Rows[j][FieldLineNum])} and TranType = 1");
			if (GlobalData.LoginSap && (TransType == Enums.BatchSerialTransType.Allocate || TransType == Enums.BatchSerialTransType.InAllocate || TransType == Enums.BatchSerialTransType.OutAlocate) && _oldBatchSerialLine.IsNotNull())
			{
				int num = Function.ParseInt(dtLine.Rows[j][FieldLineNum]);
				DataRow[] lstOld = _oldBatchSerialLine.Select($"BaseLineNumber = {num} and TranType = 1 and isnull(AbsEntry,0) <> 0");
				if (lstOld.Length != 0 && (!Function.ToString(lstOld[0]["ItemCode"]).Equals(Function.ToString(dtLine.Rows[j][FieldItemCode])) || !Function.ToString(lstOld[0]["WhsCode"]).Equals(Function.ToString(dtLine.Rows[j][FieldWhsCode]))))
				{
					continue;
				}
				int j2;
				for (j2 = 0; j2 < lstOld.Length; j2++)
				{
					if (array2.Length == 0 || array2.FirstOrDefault((DataRow t) => Function.ParseInt(t["AbsEntry"]) == Function.ParseInt(lstOld[j2]["AbsEntry"])) == null)
					{
						DataRow dataRow = dataTable.NewRow();
						dataRow.ItemArray = lstOld[j2].ItemArray;
						dataRow["TranType"] = -1;
						dataRow["LogQty"] = 0;
						dataTable.Rows.Add(dataRow);
					}
				}
			}
			if (array2.Length == 0)
			{
				if (!((TransType == Enums.BatchSerialTransType.OutAlocate || TransType == Enums.BatchSerialTransType.InAllocate) | isDraft) && (!CheckNoInvtryMv || !Function.ToString(dtLine.Rows[j]["NoInvtryMv"]).Equals("Y")))
				{
					WinCommons.Warning(Enums.MessageCode.CannotAddRowWithoutCompleteSelectionBatchSerialNumber, "", Function.ToString(dtLine.Rows[j][FieldLineNum]));
					return null;
				}
				continue;
			}
			if (CheckNoInvtryMv && Function.ToString(dtLine.Rows[j]["NoInvtryMv"]).Equals("Y"))
			{
				WinCommons.Warning(Enums.MessageCode.CannotAddBatchSerialRowWithoutInventoryMovement, "", Function.ToString(dtLine.Rows[j][FieldLineNum]));
				return null;
			}
			double num2 = array2.Sum((DataRow t) => Function.ParseDouble(t[gc_Batch_Qty.FieldName])) - Math.Abs(Function.ParseDouble(dtLine.Rows[j][FieldQuantity]));
			if ((TransType == Enums.BatchSerialTransType.OutAlocate || TransType == Enums.BatchSerialTransType.InAllocate) | isDraft)
			{
				if (num2 > 1E-06)
				{
					WinCommons.Warning(Enums.MessageCode.CannotAddRowQuantityMustGreaterThanOrEqualRequiredBatchSerialNumber, "", Function.ToString(dtLine.Rows[j][FieldLineNum]));
					return null;
				}
				continue;
			}
			if (array2.Length == 0 || Math.Abs(num2) > 1E-06)
			{
				WinCommons.Warning(Enums.MessageCode.CannotAddRowWithoutCompleteSelectionBatchSerialNumber, "", Function.ToString(dtLine.Rows[j][FieldLineNum]));
				return null;
			}
			string whsCode = Function.ToString(dtLine.Rows[j][FieldWhsCode]);
			if (CheckWhsManBinLocation(whsCode))
			{
				DataRow dataRow2 = array2.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["BinAbs"]) == 0);
				if (dataRow2 != null)
				{
					WinCommons.Warning(Enums.MessageCode.ItemLineMustBeFullyAllocatedToBinLocationWhs, "", Function.ToString(dataRow2["ItemCode"]), Function.ToString(dataRow2["BaseLineNumber"]));
					return null;
				}
			}
			if (FormObjectType != ApzObjectTypes.oStockTransfer)
			{
				continue;
			}
			string whsCode2 = Function.ToString(dtLine.Rows[j][FieldToTranferWhsCode]);
			if (CheckWhsManBinLocation(whsCode2))
			{
				DataRow[] array3 = _batchAndSerialData.Select($"BaseLineNumber = {Function.ParseInt(dtLine.Rows[j][FieldLineNum])} and TranType = 2");
				if (array3.Length == 0)
				{
					WinCommons.Warning(Enums.MessageCode.ItemLineMustBeFullyAllocatedToBinLocationToWhs, "", Function.ToString(dtLine.Rows[j]["ItemCode"]), Function.ToString(dtLine.Rows[j][FieldLineNum]));
					return null;
				}
				if (Math.Abs(Function.ParseDouble(array3.CopyToDataTable().Compute($"Sum({gc_Batch_Qty.FieldName})", "")) - Math.Abs(Function.ParseDouble(dtLine.Rows[j][FieldQuantity]))) > 1E-06)
				{
					WinCommons.Warning(Enums.MessageCode.ItemLineMustBeFullyAllocatedToBinLocationToWhs, "", Function.ToString(dtLine.Rows[j]["ItemCode"]), Function.ToString(dtLine.Rows[j][FieldLineNum]));
					return null;
				}
			}
		}
		dataTable.AcceptChanges();
		return dataTable;
	}

	public DataTable CheckDataBinAllowcationLine(DataTable dtLine, bool isDraft)
	{
		if (TransType != Enums.BatchSerialTransType.In && TransType != Enums.BatchSerialTransType.Out)
		{
			return new DataTable();
		}
		if (FinishEditorBatchAndSerial(isDraft) != Enums.MessageCode.Success)
		{
			return null;
		}
		if (_binAllowData == null || _binAllowData.Columns.Count == 0 || dtLine == null || dtLine.Rows.Count == 0)
		{
			return new DataTable();
		}
		DataRow[] array = _binAllowData.Select($"Isnull({gcBinAllowBinCode.FieldName},'') = ''");
		for (int i = 0; i < array.Length; i++)
		{
			_binAllowData.Rows.Remove(array[i]);
		}
		_binAllowData.AcceptChanges();
		DataTable dataTable = _binAllowData.Copy();
		int j;
		for (j = 0; j < dtLine.Rows.Count; j++)
		{
			if (Function.ParseInt(dtLine.Rows[j][FieldItemType]) != 4)
			{
				continue;
			}
			string text = ((!string.IsNullOrEmpty(FieldLineType)) ? Function.ToString(dtLine.Rows[j][FieldLineType]) : "R");
			string whsCode = Function.ToString(dtLine.Rows[j][FieldWhsCode]);
			string text2 = (string.IsNullOrEmpty(FieldToTranferWhsCode) ? "" : Function.ToString(dtLine.Rows[j][FieldToTranferWhsCode]));
			string text3 = Function.ToString(dtLine.Rows[j][FieldType]);
			ApzObjectTypes apzObjectTypes = ((!string.IsNullOrEmpty(FieldBaseObject)) ? ((ApzObjectTypes)Function.ParseInt(dtLine.Rows[j][FieldBaseObject])) : ApzObjectTypes.oNone);
			if (FromReserve)
			{
				switch (apzObjectTypes)
				{
				case ApzObjectTypes.oInvoices:
					apzObjectTypes = ApzObjectTypes.oReserveInvoices;
					break;
				case ApzObjectTypes.oPurchaseInvoices:
					apzObjectTypes = ApzObjectTypes.oAPReserveInvoice;
					break;
				}
			}
			if (text != "R" || text3.Equals("B") || text3.Equals("S") || string.IsNullOrEmpty(Function.ToString(dtLine.Rows[j][FieldItemCode])) || (CopyObjectNotBatchAndSerial.ContainsKey(FormObjectType) && CopyObjectNotBatchAndSerial[FormObjectType].Contains(apzObjectTypes)))
			{
				continue;
			}
			if (Math.Abs(Function.ParseDouble(dtLine.Rows[j][FieldQuantity])) < 1E-07)
			{
				if (_batchAndSerialData.AsEnumerable().Any((DataRow t) => Function.ParseInt(t["BaseLineNumber"]) == Function.ParseInt(dtLine.Rows[j][FieldLineNum])))
				{
					WinCommons.Warning(Enums.MessageCode.CannotAddBinAllocationToRowQuantity0, "", Function.ToString(dtLine.Rows[j][FieldLineNum]));
					return null;
				}
				continue;
			}
			if (CheckWhsManBinLocation(whsCode))
			{
				DataRow[] array2 = _binAllowData.Select($"BaseLineNumber = {Function.ParseInt(dtLine.Rows[j][FieldLineNum])} and TranType = 1");
				if (array2.Length == 0)
				{
					if (!isDraft && (!CheckNoInvtryMv || !Function.ToString(dtLine.Rows[j]["NoInvtryMv"]).Equals("Y")))
					{
						WinCommons.Warning(Enums.MessageCode.CannotAddRowWithoutCopleteSelectionBinAllocation, "", Function.ToString(dtLine.Rows[j][FieldLineNum]));
						return null;
					}
					continue;
				}
				if (CheckNoInvtryMv && Function.ToString(dtLine.Rows[j]["NoInvtryMv"]).Equals("Y"))
				{
					WinCommons.Warning(Enums.MessageCode.CannotAddBinAllocationRowWithoutInventoryMovement, "", Function.ToString(dtLine.Rows[j][FieldLineNum]));
					return null;
				}
				double value = Function.ParseDouble(array2.CopyToDataTable().Compute($"Sum({gcBinAllowQty.FieldName})", "")) - Math.Abs(Function.ParseDouble(dtLine.Rows[j][FieldQuantity]));
				if (isDraft)
				{
					if (Math.Abs(value) > 0.0)
					{
						WinCommons.Warning(Enums.MessageCode.CannotAddRowQuantityMustBeGreaterThanOrEqualRequiredBinAllocation, "", Function.ToString(dtLine.Rows[j][FieldLineNum]));
						return null;
					}
					continue;
				}
				if (array2.Length == 0 || Math.Abs(value) > 1E-06)
				{
					WinCommons.Warning(Enums.MessageCode.CannotAddRowWithoutCopleteSelectionBinAllocation, "", Function.ToString(dtLine.Rows[j][FieldLineNum]));
					return null;
				}
			}
			if (FormObjectType == ApzObjectTypes.oStockTransfer && CheckWhsManBinLocation(text2) && !_isBin[text2].Code.Equals("Y"))
			{
				DataRow[] array3 = _binAllowData.Select($"BaseLineNumber = {Function.ParseInt(dtLine.Rows[j][FieldLineNum])} and TranType = 2");
				if (array3.Length == 0)
				{
					WinCommons.Warning(Enums.MessageCode.ItemLineMustBeFullyAllocatedToBinLocationToWhs, "", Function.ToString(dtLine.Rows[j]["ItemCode"]), Function.ToString(dtLine.Rows[j][FieldLineNum]));
					return null;
				}
				if (Math.Abs(Function.ParseDouble(array3.CopyToDataTable().Compute($"Sum({gcBinAllowQty.FieldName})", "")) - Math.Abs(Function.ParseDouble(dtLine.Rows[j][FieldQuantity]))) > 1E-06)
				{
					WinCommons.Warning(Enums.MessageCode.ItemLineMustBeFullyAllocatedToBinLocationToWhs, "", Function.ToString(dtLine.Rows[j]["ItemCode"]), Function.ToString(dtLine.Rows[j][FieldLineNum]));
					return null;
				}
			}
		}
		dataTable.AcceptChanges();
		return dataTable;
	}

	private Enums.MessageCode UpdateBatchEditor(bool isDraft = false)
	{
		DataTable dataSourceTable = grdBatches.DataSourceTable;
		if (dataSourceTable == null || dataSourceTable.Rows.Count < 2)
		{
			return Enums.MessageCode.Success;
		}
		DataRow[] array = GvLineItem.GridControl.DatasourceTable().Select(string.Format("{0} = {1} and ({3} = 4) and isnull({2},'') <> ''", FieldLineNum, Function.ParseInt(dataSourceTable.Rows[0]["BaseLineNumber"]), FieldItemCode, FieldItemType));
		if (array.Length > 1)
		{
			return Enums.MessageCode.LineNumberIsDuplicate;
		}
		if (array.Length == 0)
		{
			return Enums.MessageCode.LineDoesNotExists;
		}
		string arg = ((grdBatches.MainView == gvBatches) ? gc_Batch_Batch.FieldName : gcBinAllowBinCode.FieldName);
		bool flag = grdBatches.MainView == gvBatches;
		double num = Math.Abs(Function.ParseDouble(array[0][FieldQuantity])) - dataSourceTable.AsEnumerable().Sum((DataRow t) => Function.ParseDouble(t[gc_Batch_Qty.FieldName]));
		int num2 = dataSourceTable.Select($"isnull({gc_Batch_Qty.FieldName},0) = 0 and Isnull({arg},'') <> ''").Length;
		if (num < -1E-06 || (Math.Abs(num) < 1E-06 && num2 > 0))
		{
			return Enums.MessageCode.QuantityIsGreaterThanRequired;
		}
		if (num2 == 0)
		{
			if (num > 1E-06 && (TransType == Enums.BatchSerialTransType.Out || TransType == Enums.BatchSerialTransType.In) && ((!isDraft & flag) || !flag))
			{
				return Enums.MessageCode.QuantityIsSmallerThanRequired;
			}
			return Enums.MessageCode.Success;
		}
		for (int num3 = 0; num3 < dataSourceTable.Rows.Count; num3++)
		{
			if (!string.IsNullOrEmpty(Function.ToString(dataSourceTable.Rows[num3][gc_Batch_Batch.FieldName])) && Function.ParseDouble(dataSourceTable.Rows[num3][gc_Batch_Qty.FieldName]) <= 0.0)
			{
				dataSourceTable.Rows[num3][gc_Batch_Qty.FieldName] = num / (double)num2;
			}
		}
		dataSourceTable.AcceptChanges();
		grdBatches.DataSource = dataSourceTable;
		grdBatches.RefreshDataSource();
		return Enums.MessageCode.Success;
	}

	public void EnableDisableControls(bool isEnable)
	{
		try
		{
			txtBatchNo.ReadOnly = !isEnable;
			ToolStripMenuItem toolStripMenuItem = batcheDeleteRowToolStripMenuItem;
			ToolStripMenuItem toolStripMenuItem2 = automaticCreationToolStripMenuItem;
			ToolStripMenuItem toolStripMenuItem3 = autoSelectToolStripMenuItem;
			bool flag = (batchDuplicateRowToolStripMenuItem.Enabled = isEnable);
			bool flag3 = (toolStripMenuItem3.Enabled = flag);
			bool enabled = (toolStripMenuItem2.Enabled = flag3);
			toolStripMenuItem.Enabled = enabled;
			gvBatches.ReadOnly = !isEnable;
			rBatch_Cal.Buttons[0].Visible = isEnable;
			rBatch_Date.Buttons[0].Visible = isEnable;
			rBatch_Status.Buttons[0].Visible = isEnable;
			enabled = (rBatchNo.Buttons[0].Visible = isEnable && (TransType == Enums.BatchSerialTransType.Out || TransType == Enums.BatchSerialTransType.OutAlocate || InBatchFromWhs));
			_chooseBatchSerial = enabled;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public void SetFormViewMode(Enums.FormViewModeType value)
	{
		_formView = value;
		switch (value)
		{
		case Enums.FormViewModeType.Add:
			EnableDisableControls(isEnable: true);
			rBinAllowBinCode.Buttons[0].Visible = true;
			binAllowDeleteRowToolStripMenuItem.Enabled = true;
			gvBinAllowcate.ReadOnly = false;
			break;
		case Enums.FormViewModeType.Update:
			EnableDisableControls(TransType == Enums.BatchSerialTransType.Allocate || TransType == Enums.BatchSerialTransType.InAllocate || TransType == Enums.BatchSerialTransType.OutAlocate);
			binAllowDeleteRowToolStripMenuItem.Enabled = false;
			gvBinAllowcate.ReadOnly = true;
			rBinAllowBinCode.Buttons[0].Visible = false;
			break;
		case Enums.FormViewModeType.View:
			EnableDisableControls(TransType == Enums.BatchSerialTransType.Allocate || TransType == Enums.BatchSerialTransType.InAllocate || TransType == Enums.BatchSerialTransType.OutAlocate);
			binAllowDeleteRowToolStripMenuItem.Enabled = false;
			gvBinAllowcate.ReadOnly = true;
			rBinAllowBinCode.Buttons[0].Visible = false;
			FromReserve = false;
			break;
		case Enums.FormViewModeType.Busy:
			EnableDisableControls(isEnable: false);
			binAllowDeleteRowToolStripMenuItem.Enabled = false;
			gvBinAllowcate.ReadOnly = true;
			rBinAllowBinCode.Buttons[0].Visible = false;
			break;
		case Enums.FormViewModeType.Find:
			break;
		}
	}

	public void DeleteBatchAndSerialByLineNum(int lineDelete = -2)
	{
		try
		{
			if (lineDelete != -2)
			{
				DataRow[] array = _batchAndSerialData.Select($"BaseLineNumber = {lineDelete}");
				for (int i = 0; i < array.Length; i++)
				{
					_batchAndSerialData.Rows.Remove(array[i]);
				}
				DataRow[] array2 = _binAllowData.Select($"BaseLineNumber = {lineDelete}");
				for (int j = 0; j < array2.Length; j++)
				{
					_binAllowData.Rows.Remove(array2[j]);
				}
				_batchAndSerialData.AcceptChanges();
				_binAllowData.AcceptChanges();
			}
			else
			{
				if (!_batchAndSerialData.IsNotNull())
				{
					return;
				}
				int[] selectedRows = GvLineItem.GetSelectedRows();
				for (int k = 0; k < selectedRows.Length; k++)
				{
					int num = Function.ParseInt(GvLineItem.GetRowCellValue(selectedRows[k], FieldLineNum));
					DataRow[] array3 = _batchAndSerialData.Select($"BaseLineNumber = {num}");
					for (int l = 0; l < array3.Length; l++)
					{
						_batchAndSerialData.Rows.Remove(array3[l]);
					}
					DataRow[] array4 = _binAllowData.Select($"BaseLineNumber = {num}");
					for (int m = 0; m < array4.Length; m++)
					{
						_binAllowData.Rows.Remove(array4[m]);
					}
				}
				_batchAndSerialData.AcceptChanges();
				_binAllowData.AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void DeleteEmptyRow(DataTable dt)
	{
		DataRow[] array;
		if (Function.ToString(GvLineItem.GetFocusedRowCellValue(FieldType)).Equals("B"))
		{
			array = dt.Select("isnull(DistNumber, '') = ''");
		}
		else
		{
			array = (Function.ToString(GvLineItem.GetFocusedRowCellValue(FieldType)).Equals("S") ? dt.Select("isnull(DistNumber, '') = '' and isnull(MnfSerial, '') = '' and isnull(LotNumber, '') = ''") : dt.Select("isnull(BinAbs, 0) = 0"));
		}
		for (int i = 0; i < array.Length; i++)
		{
			dt.Rows.Remove(array[i]);
		}
		dt.AcceptChanges();
	}

	private void AutoAllocateToWhs(DataRow rowNew)
	{
		if (FormObjectType != ApzObjectTypes.oStockTransfer)
		{
			return;
		}
		string text = Function.ToString(GvLineItem.GetFocusedRowCellValue(FieldToTranferWhsCode));
		string itemCode = Function.ToString(GvLineItem.GetFocusedRowCellValue(FieldItemCode));
		if (!CheckWhsManBinLocation(text))
		{
			return;
		}
		int binAbs = -1;
		string value = "";
		if (_isBin[text].Name.Equals("Y") && TransType == Enums.BatchSerialTransType.In)
		{
			DataTable binDefault = _httpClient.BinLocMasterDataClient.GetBinDefault("I", text, itemCode, 0.0);
			if (binDefault.IsNotNull())
			{
				binAbs = Function.ParseInt(binDefault.Rows[0]["AbsEntry"]);
				value = Function.ToString(binDefault.Rows[0]["BinCode"]);
			}
		}
		double num = Function.ParseDouble(grdBatches.DataSourceTable.Compute("Sum(LogQty)", string.Format("AbsEntry = {0} and TranType = 1 and BaseLineNumber = {1}", Function.ParseInt(rowNew["AbsEntry"]), Function.ParseInt(rowNew["BaseLineNumber"]))));
		double num2 = Function.ParseDouble(_batchAndSerialData.Compute("Sum(LogQty)", string.Format("AbsEntry = {0} and TranType = 2 and BaseLineNumber = {1}", Function.ParseInt(rowNew["AbsEntry"]), Function.ParseInt(rowNew["BaseLineNumber"]))));
		DataRow dataRow = _batchAndSerialData.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["AbsEntry"]) == Function.ParseInt(rowNew["AbsEntry"]) && Function.ParseInt(t["BaseLineNumber"]) == Function.ParseInt(rowNew["BaseLineNumber"]) && Function.ParseInt(t["TranType"]) == 2 && Function.ParseInt(t["BinAbs"]) == binAbs);
		if (num > num2)
		{
			if (dataRow != null)
			{
				dataRow["LogQty"] = Function.ParseDouble(dataRow["LogQty"]) + num - num2;
			}
			else
			{
				DataRow dataRow2 = _batchAndSerialData.NewRow();
				dataRow2.ItemArray = rowNew.ItemArray;
				dataRow2["TranType"] = 2;
				dataRow2["BinAbs"] = binAbs;
				dataRow2["BinCode"] = value;
				_batchAndSerialData.Rows.Add(dataRow2);
			}
		}
		else if (_batchAndSerialData.AsEnumerable().Count((DataRow t) => Function.ParseInt(t["AbsEntry"]) == Function.ParseInt(rowNew["AbsEntry"]) && Function.ParseInt(t["BaseLineNumber"]) == Function.ParseInt(rowNew["BaseLineNumber"]) && Function.ParseInt(t["TranType"]) == 2) == 1 && dataRow != null)
		{
			dataRow["LogQty"] = num;
		}
		_batchAndSerialData.AcceptChanges();
	}

	private void AllowcateDeleteBatch(DataRow rowDelete)
	{
		if (FormObjectType == ApzObjectTypes.oStockTransfer)
		{
			string whsCode = Function.ToString(GvLineItem.GetFocusedRowCellValue(FieldToTranferWhsCode));
			string itemCode = Function.ToString(GvLineItem.GetFocusedRowCellValue(FieldItemCode));
			if (!CheckWhsManBinLocation(whsCode))
			{
				return;
			}
			int num = -1;
			string value = "";
			DataTable binDefault = _httpClient.BinLocMasterDataClient.GetBinDefault("I", whsCode, itemCode, 0.0);
			if (binDefault.IsNotNull())
			{
				num = Function.ParseInt(binDefault.Rows[0]["AbsEntry"]);
				value = Function.ToString(binDefault.Rows[0]["BinCode"]);
			}
			DataRow[] array = _batchAndSerialData.Select(string.Format("AbsEntry = {0} and TranType = 2 and BaseLineNumber = {1}", Function.ParseInt(rowDelete["AbsEntry"]), Function.ParseInt(rowDelete["BaseLineNumber"])));
			for (int i = 0; i < array.Length; i++)
			{
				_batchAndSerialData.Rows.Remove(array[i]);
			}
			double num2 = Function.ParseDouble(grdBatches.DataSourceTable.Compute("Sum(LogQty)", string.Format("AbsEntry = {0} and TranType = 1 and BaseLineNumber = {1}", Function.ParseInt(rowDelete["AbsEntry"]), Function.ParseInt(rowDelete["BaseLineNumber"])))) - Function.ParseDouble(rowDelete["LogQty"]);
			if (num2 > 0.0)
			{
				DataRow dataRow = _batchAndSerialData.NewRow();
				dataRow.ItemArray = rowDelete.ItemArray;
				dataRow["TranType"] = 2;
				dataRow["LogQty"] = num2;
				dataRow["BinAbs"] = num;
				dataRow["BinCode"] = value;
				_batchAndSerialData.Rows.Add(dataRow);
			}
		}
		_batchAndSerialData.AcceptChanges();
	}

	public void BeginUpdateLine()
	{
		_isLoadLine = true;
	}

	public void EndUpdateLine()
	{
		_isLoadLine = false;
	}

	private void ClearAllocateBinToWhs(int lineNum)
	{
		DataRow[] array = (from t in _batchAndSerialData.AsEnumerable()
			where Function.ParseInt(t["BaseLineNumber"]) == lineNum && Function.ParseInt(t["TranType"]) == 2
			select t).ToArray();
		for (int num = 0; num < array.Length; num++)
		{
			_batchAndSerialData.Rows.Remove(array[num]);
		}
		_batchAndSerialData.AcceptChanges();
	}

	private void LoadDataBinAllowcation(int rowHandle)
	{
		string binCode = Function.ToString(gvBinAllowcate.GetRowCellValue(rowHandle, gcBinAllowBinCode));
		string whsCode = ((_tranBinAllowcateType == 1) ? Function.ToString(GvLineItem.GetFocusedRowCellValue(FieldWhsCode)) : Function.ToString(GvLineItem.GetFocusedRowCellValue(FieldToTranferWhsCode)));
		DataTable dataBinLocation = _httpClient.BinLocMasterDataClient.GetDataBinLocation(binCode, whsCode);
		DataRow dataRow = gvBinAllowcate.GetDataRow(rowHandle);
		if (dataBinLocation.IsNotNull())
		{
			DataTable dataSourceTable = grdBatches.DataSourceTable;
			dataRow["BinCode"] = dataBinLocation.Rows[0]["BinCode"];
			dataRow["BinAbs"] = dataBinLocation.Rows[0]["AbsEntry"];
			double num = Math.Abs(Function.ParseDouble(GvLineItem.GetFocusedRowCellValue(FieldQuantity))) + Function.ParseDouble(dataRow[gcBinAllowQty.FieldName]) - Function.ParseDouble(dataSourceTable.Compute($"sum({gcBinAllowQty.FieldName})", "isnull(BinCode, '') <> ''"));
			dataRow["LogQty"] = ((num > 0.0) ? num : 0.0);
			dataRow[gcBinAllowDetail.FieldName] = dataBinLocation.Rows[0]["Descr"];
			dataRow[gcBinAllowCreateDate.FieldName] = dataBinLocation.Rows[0][gcBinAllowCreateDate.FieldName];
		}
		else
		{
			dataRow["BinCode"] = null;
			dataRow["BinAbs"] = DBNull.Value;
			WinCommons.Warning(Enums.MessageCode.BinLocationDoesNotExistsInWhs, "");
		}
		grdBatches.DataSourceTable.AcceptChanges();
	}

	public void AutoAllowcateBinToWhs(int rowHandle, bool openDraft = false)
	{
		string text = Function.ToString(GvLineItem.GetRowCellValue(rowHandle, FieldType));
		if (text.Equals("B") || text.Equals("S") || (TransType != Enums.BatchSerialTransType.In && TransType != Enums.BatchSerialTransType.Out))
		{
			return;
		}
		string text2 = Function.ToString(GvLineItem.GetRowCellValue(rowHandle, FieldItemCode));
		if (string.IsNullOrEmpty(text2))
		{
			return;
		}
		if (TransType == Enums.BatchSerialTransType.In)
		{
			string whsCode = Function.ToString(GvLineItem.GetRowCellValue(rowHandle, FieldWhsCode));
			if (CheckWhsManBinLocation(whsCode))
			{
				int binAbs = -1;
				string value = "";
				int lineNum = Function.ParseInt(GvLineItem.GetRowCellValue(rowHandle, FieldLineNum));
				if (openDraft && _binAllowData.Select($"TranType = 1 and BaseLineNumber = {lineNum}").Any())
				{
					return;
				}
				DataTable binDefault = _httpClient.BinLocMasterDataClient.GetBinDefault("I", whsCode, text2, 0.0);
				if (binDefault.IsNotNull())
				{
					binAbs = Function.ParseInt(binDefault.Rows[0]["AbsEntry"]);
					value = Function.ToString(binDefault.Rows[0]["BinCode"]);
				}
				double num = Math.Abs(Function.ParseDouble(GvLineItem.GetRowCellValue(rowHandle, FieldQuantity)));
				double num2 = Function.ParseDouble(_binAllowData.Compute("Sum(LogQty)", $"TranType = 1 and BaseLineNumber = {lineNum}"));
				if (num > num2)
				{
					DataRow dataRow = _binAllowData.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["BaseLineNumber"]) == lineNum && Function.ParseInt(t["TranType"]) == 1 && Function.ParseInt(t["BinAbs"]) == binAbs);
					if (dataRow != null)
					{
						dataRow["LogQty"] = Function.ParseDouble(dataRow["LogQty"]) + num - num2;
					}
					else if (binAbs > 0)
					{
						DataRow dataRow2 = _binAllowData.NewRow();
						dataRow2["LogQty"] = num - num2;
						dataRow2["TranType"] = 1;
						dataRow2["BaseLineNumber"] = lineNum;
						dataRow2["BinAbs"] = binAbs;
						dataRow2["BinCode"] = value;
						_binAllowData.Rows.Add(dataRow2);
					}
				}
				_binAllowData.AcceptChanges();
			}
		}
		else if (TransType == Enums.BatchSerialTransType.Out)
		{
			string whsCode2 = Function.ToString(GvLineItem.GetRowCellValue(rowHandle, FieldWhsCode));
			if (CheckWhsManBinLocation(whsCode2))
			{
				int lineNum2 = Function.ParseInt(GvLineItem.GetRowCellValue(rowHandle, FieldLineNum));
				double qty = Math.Abs(Function.ParseDouble(GvLineItem.GetRowCellValue(rowHandle, FieldQuantity)));
				List<DataRow> list = (from t in _binAllowData.AsEnumerable()
					where Function.ParseInt(t["BaseLineNumber"]) == lineNum2 && Function.ParseInt(t["TranType"]) == 1
					select t).ToList();
				if (openDraft && list.Count != 0)
				{
					return;
				}
				DataTable binDefault2 = _httpClient.BinLocMasterDataClient.GetBinDefault("O", whsCode2, text2, qty);
				if (list.Any())
				{
					for (int num3 = 0; num3 < list.Count; num3++)
					{
						_binAllowData.Rows.Remove(list[num3]);
					}
				}
				if (binDefault2.IsNotNull())
				{
					for (int num4 = 0; num4 < binDefault2.Rows.Count; num4++)
					{
						DataRow dataRow3 = _binAllowData.NewRow();
						dataRow3["LogQty"] = binDefault2.Rows[num4]["Qty"];
						dataRow3["SelectedQty"] = binDefault2.Rows[num4]["OnHand"];
						dataRow3["TranType"] = 1;
						dataRow3["BaseLineNumber"] = lineNum2;
						dataRow3["BinAbs"] = binDefault2.Rows[num4]["AbsEntry"];
						dataRow3["BinCode"] = binDefault2.Rows[num4]["BinCode"];
						_binAllowData.Rows.Add(dataRow3);
					}
					_binAllowData.AcceptChanges();
				}
			}
		}
		if (FormObjectType != ApzObjectTypes.oStockTransfer)
		{
			return;
		}
		string whsCode3 = Function.ToString(GvLineItem.GetRowCellValue(rowHandle, FieldToTranferWhsCode));
		if (!CheckWhsManBinLocation(whsCode3))
		{
			return;
		}
		int binAbs2 = -1;
		string value2 = "";
		int lineNum3 = Function.ParseInt(GvLineItem.GetRowCellValue(rowHandle, FieldLineNum));
		if (openDraft && _binAllowData.Select($"TranType = 2 and BaseLineNumber = {lineNum3}").Any())
		{
			return;
		}
		DataTable binDefault3 = _httpClient.BinLocMasterDataClient.GetBinDefault("I", whsCode3, text2, 0.0);
		if (binDefault3.IsNotNull())
		{
			binAbs2 = Function.ParseInt(binDefault3.Rows[0]["AbsEntry"]);
			value2 = Function.ToString(binDefault3.Rows[0]["BinCode"]);
		}
		decimal num5 = Math.Abs(Function.ParseDecimal(GvLineItem.GetRowCellValue(rowHandle, FieldQuantity)));
		List<DataRow> list2 = (from t in _binAllowData.AsEnumerable()
			where Function.ParseInt(t["TranType"]) == 2 && Function.ParseInt(t["BaseLineNumber"]) == lineNum3
			select t).ToList();
		decimal num6 = (list2.Any() ? list2.Sum((DataRow t) => Function.ParseDecimal(t["LogQty"])) : 0m);
		if (num5 > num6)
		{
			DataRow dataRow4 = _binAllowData.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["BaseLineNumber"]) == lineNum3 && Function.ParseInt(t["TranType"]) == 2 && Function.ParseInt(t["BinAbs"]) == binAbs2);
			if (dataRow4 != null)
			{
				dataRow4["LogQty"] = Function.ParseDecimal(dataRow4["LogQty"]) + num5 - num6;
			}
			else if (binAbs2 > 0)
			{
				DataRow dataRow5 = _binAllowData.NewRow();
				dataRow5["LogQty"] = num5 - num6;
				dataRow5["TranType"] = 2;
				dataRow5["BaseLineNumber"] = lineNum3;
				dataRow5["BinAbs"] = binAbs2;
				dataRow5["BinCode"] = value2;
				_binAllowData.Rows.Add(dataRow5);
			}
		}
		else if (num5 < num6 && list2.Count == 1)
		{
			list2[0]["LogQty"] = num5;
		}
		_binAllowData.AcceptChanges();
	}

	private void ClearAllocateBinForLine(int lineNum)
	{
		DataRow[] array = (from t in _binAllowData.AsEnumerable()
			where Function.ParseInt(t["BaseLineNumber"]) == lineNum
			select t).ToArray();
		for (int num = 0; num < array.Length; num++)
		{
			_binAllowData.Rows.Remove(array[num]);
		}
		_binAllowData.AcceptChanges();
	}

	public bool CheckWhsManBinLocation(string whsCode)
	{
		if (string.IsNullOrEmpty(whsCode))
		{
			return false;
		}
		if (!_isBin.Keys.Contains(whsCode))
		{
			_isBin.Add(whsCode, _httpClient.Warehouse.WhsCheckFollowBin(whsCode));
		}
		return _isBin[whsCode].Code.Equals("Y");
	}

	private async void DoAutoSelect()
	{
		try
		{
			string type = Function.ToString(GvLineItem.GetFocusedRowCellValue(FieldType));
			string itemCode = Function.ToString(GvLineItem.GetFocusedRowCellValue(FieldItemCode));
			double genQty = Math.Abs(Function.ParseDouble(GvLineItem.GetFocusedRowCellValue(FieldQuantity))) - Function.ParseDouble(grdBatches.DataSourceTable.Compute($"sum({gc_Batch_Qty.FieldName})", ""));
			DataTable dtCons = new DataTable();
			dtCons.Columns.Add("ItemCode", typeof(string));
			dtCons.Columns.Add("AbsEntry", typeof(int));
			dtCons.Columns.Add("SysNumber", typeof(int));
			dtCons.Columns.Add("DistNumber", typeof(string));
			dtCons.Columns.Add("LogQty", typeof(double));
			dtCons.Columns.Add("BinAbs", typeof(int));
			DataTable dtExistBatch = (grdBatches.DataSourceTable.IsNotNull() ? (from row in grdBatches.DataSourceTable.AsEnumerable().Union(_batchAndSerialData.AsEnumerable()).AsEnumerable()
				group row by new
				{
					absEntry = Function.ParseInt(row["AbsEntry"]),
					sysNumber = Function.ParseInt(row["SysNumber"]),
					ItemCode = Function.ToString(row["ItemCode"]),
					disNumber = (grdBatches.DatasourceTable().ExistsColumn("DistNumber") ? Function.ToString(row["DistNumber"]) : ""),
					BinAbs = Function.ParseInt(row["BinAbs"])
				} into g
				select dtCons.LoadDataRow(new object[6]
				{
					g.Key.ItemCode,
					g.Key.absEntry,
					g.Key.sysNumber,
					g.Key.disNumber,
					Function.ParseDouble(g.Sum((DataRow r) => Function.ParseDouble(r["LogQty"]))),
					g.Key.BinAbs
				}, fAcceptChanges: false)).CopyToDataTable() : dtCons);
			string whsCode = Function.ToString(GvLineItem.GetFocusedRowCellValue(FieldWhsCode));
			DataTable dataTable = await _httpClient.BatchDetails.GetDataAutoSelect(type, (TransType == Enums.BatchSerialTransType.Out && CheckWhsManBinLocation(whsCode)) ? "Y" : "N", itemCode, Function.ToString(GvLineItem.GetFocusedRowCellValue(FieldWhsCode)), genQty, dtExistBatch);
			if (dataTable.IsNotNull())
			{
				DataTable dataSourceTable = grdBatches.DataSourceTable;
				DeleteEmptyRow(dataSourceTable);
				for (int num = 0; num < dataTable.Rows.Count; num++)
				{
					DataRow[] array = dataSourceTable.Select(string.Format("isnull(AbsEntry,0) = {0} and isnull(SysNumber,0) = {1} and isnull(BinAbs,0) = {2}", Function.ParseInt(dataTable.Rows[num]["AbsEntry"]), Function.ParseInt(dataTable.Rows[num]["SysNumber"]), Function.ParseInt(dataTable.Rows[num]["BinAbs"])));
					if (array.Length != 0)
					{
						array[0][gc_Batch_Qty.FieldName] = Function.ParseDouble(array[0][gc_Batch_Qty.FieldName]) + Function.ParseDouble(dataTable.Rows[num]["InStock"]);
						continue;
					}
					DataRow dataRow = dataSourceTable.NewRow();
					for (int num2 = 0; num2 < dataTable.Columns.Count; num2++)
					{
						string columnName = dataTable.Columns[num2].ColumnName;
						if (dataSourceTable.Columns.Contains(columnName))
						{
							dataRow[columnName] = dataTable.Rows[num][num2];
						}
					}
					dataRow["BaseLineNumber"] = Function.ParseInt(GvLineItem.GetFocusedRowCellValue(FieldLineNum));
					dataRow["ItemCode"] = Function.ToString(GvLineItem.GetFocusedRowCellValue(FieldItemCode));
					dataRow["ItemName"] = Function.ToString(GvLineItem.GetFocusedRowCellValue(FieldItemName));
					dataRow["UserSign"] = Function.ParseInt(GlobalData.UserSignLogin);
					dataRow[gc_Batch_Status.FieldName] = "0";
					dataRow["IsExits"] = "Y";
					dataRow["TranType"] = 1;
					dataRow[gc_Batch_Qty.FieldName] = Function.ParseDouble(dataTable.Rows[num]["InStock"]);
					dataRow["ObjType"] = (type.Equals("S") ? Function.ToString(10000045) : Function.ToString(10000044));
					dataSourceTable.Rows.Add(dataRow);
					dataSourceTable.AcceptChanges();
					grdBatches.DataSource = dataSourceTable;
					AutoAllocateToWhs(dataRow);
				}
				if (DBatchSerialChanged != null)
				{
					DBatchSerialChanged();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		AddNewRowBatch();
	}

	public void CalculateOpenQty()
	{
		decimal num = Math.Abs(Function.ParseDecimal(GvLineItem.GetFocusedRowCellValue(FieldQuantity)));
		decimal num2 = (from t in grdBatches.DataSourceTable.AsEnumerable()
			where (grdBatches.MainView == gvBatches && !string.IsNullOrWhiteSpace(Function.ToString(t["DistNumber"]))) || (grdBatches.MainView == gvBinAllowcate && !string.IsNullOrWhiteSpace(Function.ToString(t["BinCode"])))
			select t).Sum((DataRow t) => Function.ParseDecimal(t["LogQty"]));
		grdBatches.DataSourceTable().AcceptChanges();
		decimal num3 = num - num2;
		txtOpenQty.EditValue = num3;
		if (num3 != 0m)
		{
			txtOpenQty.Properties.AppearanceReadOnly.BackColor = Color.Red;
			txtOpenQty.Properties.AppearanceReadOnly.Options.UseBackColor = true;
		}
		else
		{
			txtOpenQty.Properties.AppearanceReadOnly.Options.UseBackColor = false;
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.UserControls.BatchAndSerial.ucBatchAndSerial));
		DevExpress.Utils.SerializableAppearanceObject appearance4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled4 = new DevExpress.Utils.SerializableAppearanceObject();
		this.rBatchNo = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.rBatch_Cal = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.rBatch_Status = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.rBatch_Date = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.txtBatchNo = new Apzon.Controls.Editor.TextEditEx();
		this.lbBatchNo = new Apzon.Controls.Controls.LabelLine();
		this.grdBatches = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip_Batch = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.automaticCreationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.autoSelectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.batchDuplicateRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.allowcateSerialBatchToWhsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.batcheDeleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gvBatches = new Apzon.Controls.Grid.GridViewEx();
		this.gc_Batch_Batch = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Batch_Qty = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Batch_VaribleQty = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Bath_MnfSerial = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Batch_LotNumber = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_BinLocation = new Apzon.Controls.Grid.GridColumnEx();
		this.rBinCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gc_Batch_Status = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Batch_ExpDate = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Batch_MnfDate = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Batch_InDate = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Batch_Location = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Batch_Notes = new Apzon.Controls.Grid.GridColumnEx();
		this.rBinAllowBinCode = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gvBinAllowcate = new Apzon.Controls.Grid.GridViewEx();
		this.gcBinAllowBinCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gcBinAllowQty = new Apzon.Controls.Grid.GridColumnEx();
		this.gcBinAllowAvailQty = new Apzon.Controls.Grid.GridColumnEx();
		this.gcBinAllowCreateDate = new Apzon.Controls.Grid.GridColumnEx();
		this.gcBinAllowDetail = new Apzon.Controls.Grid.GridColumnEx();
		this.btnRotate = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnHide = new Apzon.Controls.Controls.SimpleButtonEx();
		this.contextMenuStrip_Bin = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.autoSelectToolBin = new System.Windows.Forms.ToolStripMenuItem();
		this.binAllowDeleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.lbTitle = new Apzon.Controls.Controls.LabelLine();
		this.lbOpenQty = new Apzon.Controls.Controls.LabelLine();
		this.txtOpenQty = new Apzon.Controls.Editor.CalcEditEx();
		((System.ComponentModel.ISupportInitialize)this.rBatchNo).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rBatch_Cal).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rBatch_Status).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rBatch_Date).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rBatch_Date.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtBatchNo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grdBatches).BeginInit();
		this.contextMenuStrip_Batch.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gvBatches).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rBinCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rBinAllowBinCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gvBinAllowcate).BeginInit();
		this.contextMenuStrip_Bin.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txtOpenQty.Properties).BeginInit();
		base.SuspendLayout();
		this.rBatchNo.AutoHeight = false;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.rBatchNo.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rBatchNo.Name = "rBatchNo";
		this.rBatchNo.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rBatchNo_ButtonClick);
		this.rBatch_Cal.AutoHeight = false;
		this.rBatch_Cal.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rBatch_Cal.Name = "rBatch_Cal";
		this.rBatch_Status.AutoHeight = false;
		this.rBatch_Status.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rBatch_Status.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name1"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Name2", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rBatch_Status.DisplayMember = "Name";
		this.rBatch_Status.Name = "rBatch_Status";
		this.rBatch_Status.NullText = "";
		this.rBatch_Status.ShowFooter = false;
		this.rBatch_Status.ShowHeader = false;
		this.rBatch_Status.ValueMember = "Code";
		this.rBatch_Date.AutoHeight = false;
		this.rBatch_Date.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rBatch_Date.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rBatch_Date.Name = "rBatch_Date";
		this.txtBatchNo.EditControl = null;
		this.txtBatchNo.FieldName = "txtBatchNo";
		this.txtBatchNo.IsSearchControl = false;
		this.txtBatchNo.Location = new System.Drawing.Point(113, 28);
		this.txtBatchNo.Name = "txtBatchNo";
		this.txtBatchNo.SettingInVisible = false;
		this.txtBatchNo.SettingReadonly = false;
		this.txtBatchNo.Size = new System.Drawing.Size(170, 20);
		this.txtBatchNo.TabIndex = 9;
		this.txtBatchNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtBatchNo_KeyDown);
		this.lbBatchNo.EditControl = null;
		this.lbBatchNo.EditValue = "Batch No";
		this.lbBatchNo.FieldName = null;
		this.lbBatchNo.HideWhenEmpty = false;
		this.lbBatchNo.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbBatchNo.Location = new System.Drawing.Point(34, 31);
		this.lbBatchNo.Name = "lbBatchNo";
		this.lbBatchNo.ReadOnly = false;
		this.lbBatchNo.SettingInVisible = false;
		this.lbBatchNo.SettingReadonly = false;
		this.lbBatchNo.ShowUnderLine = true;
		this.lbBatchNo.Size = new System.Drawing.Size(80, 20);
		this.lbBatchNo.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbBatchNo.TabIndex = 7;
		this.lbBatchNo.Utext = "Batch No";
		this.grdBatches.AllowPasteExcel = false;
		this.grdBatches.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grdBatches.ClipboardData = null;
		this.grdBatches.ContextMenuStrip = this.contextMenuStrip_Batch;
		this.grdBatches.Location = new System.Drawing.Point(0, 56);
		this.grdBatches.MainView = this.gvBatches;
		this.grdBatches.Name = "grdBatches";
		this.grdBatches.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[6] { this.rBatch_Status, this.rBatch_Cal, this.rBatch_Date, this.rBatchNo, this.rBinCode, this.rBinAllowBinCode });
		this.grdBatches.Size = new System.Drawing.Size(915, 224);
		this.grdBatches.TabIndex = 8;
		this.grdBatches.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[2] { this.gvBatches, this.gvBinAllowcate });
		this.grdBatches.EditorKeyDown += new System.Windows.Forms.KeyEventHandler(this.grdBatches_EditorKeyDown);
		this.contextMenuStrip_Batch.Items.AddRange(new System.Windows.Forms.ToolStripItem[5] { this.automaticCreationToolStripMenuItem, this.autoSelectToolStripMenuItem, this.batchDuplicateRowToolStripMenuItem, this.allowcateSerialBatchToWhsToolStripMenuItem, this.batcheDeleteRowToolStripMenuItem });
		this.contextMenuStrip_Batch.Name = "contextMenuStrip_Batch";
		this.contextMenuStrip_Batch.Size = new System.Drawing.Size(231, 114);
		this.contextMenuStrip_Batch.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Batch_Opening);
		this.automaticCreationToolStripMenuItem.Name = "automaticCreationToolStripMenuItem";
		this.automaticCreationToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
		this.automaticCreationToolStripMenuItem.Text = "Automatic Creation";
		this.automaticCreationToolStripMenuItem.Click += new System.EventHandler(this.automaticCreationToolStripMenuItem_Click);
		this.autoSelectToolStripMenuItem.Name = "autoSelectToolStripMenuItem";
		this.autoSelectToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
		this.autoSelectToolStripMenuItem.Text = "Auto Select";
		this.autoSelectToolStripMenuItem.Click += new System.EventHandler(this.autoSelectToolStripMenuItem_Click);
		this.batchDuplicateRowToolStripMenuItem.Name = "batchDuplicateRowToolStripMenuItem";
		this.batchDuplicateRowToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
		this.batchDuplicateRowToolStripMenuItem.Text = "Duplicate Row";
		this.batchDuplicateRowToolStripMenuItem.Click += new System.EventHandler(this.batchDuplicateRowToolStripMenuItem_Click);
		this.allowcateSerialBatchToWhsToolStripMenuItem.Name = "allowcateSerialBatchToWhsToolStripMenuItem";
		this.allowcateSerialBatchToWhsToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
		this.allowcateSerialBatchToWhsToolStripMenuItem.Text = "Allowcate Serial/Batch to whs";
		this.allowcateSerialBatchToWhsToolStripMenuItem.Visible = false;
		this.allowcateSerialBatchToWhsToolStripMenuItem.Click += new System.EventHandler(this.allowcateSerialBatchToWhsToolStripMenuItem_Click);
		this.batcheDeleteRowToolStripMenuItem.Name = "batcheDeleteRowToolStripMenuItem";
		this.batcheDeleteRowToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
		this.batcheDeleteRowToolStripMenuItem.Text = "Delete Row";
		this.batcheDeleteRowToolStripMenuItem.Click += new System.EventHandler(this.batcheDeleteRowToolStripMenuItem_Click);
		this.gvBatches.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvBatches.ColumnKey = "DistNumber";
		this.gvBatches.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[12]
		{
			this.gc_Batch_Batch, this.gc_Batch_Qty, this.gc_Batch_VaribleQty, this.gc_Bath_MnfSerial, this.gc_Batch_LotNumber, this.gc_BinLocation, this.gc_Batch_Status, this.gc_Batch_ExpDate, this.gc_Batch_MnfDate, this.gc_Batch_InDate,
			this.gc_Batch_Location, this.gc_Batch_Notes
		});
		this.gvBatches.DisableFormSetting = false;
		this.gvBatches.DisplayException = false;
		this.gvBatches.EditColumn = null;
		this.gvBatches.FieldCurrency = null;
		this.gvBatches.FieldRate = null;
		this.gvBatches.FormId = null;
		this.gvBatches.GridControl = this.grdBatches;
		this.gvBatches.IgnoreFilter = false;
		this.gvBatches.IndexEditRow = -1;
		this.gvBatches.IndicatorWidth = 50;
		this.gvBatches.IsApzTable = false;
		this.gvBatches.IsInsertNewRow = false;
		this.gvBatches.IsLoadingData = false;
		this.gvBatches.IsMappingContextAction = false;
		this.gvBatches.ItemId = "OBTN";
		this.gvBatches.LstColumnAllowBatch = null;
		this.gvBatches.LstColumnNotAllowBatch = null;
		this.gvBatches.LstColumnNotEditable = null;
		this.gvBatches.LstColumNotSetting = null;
		this.gvBatches.Name = "gvBatches";
		this.gvBatches.OptionsSelection.MultiSelect = true;
		this.gvBatches.OptionsView.ColumnAutoWidth = false;
		this.gvBatches.OptionsView.ShowGroupPanel = false;
		this.gvBatches.UseBatchMod = false;
		this.gvBatches.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gvBatches_RowCellStyle);
		this.gvBatches.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gvBatches_ShowingEditor);
		this.gvBatches.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvBatches_CellValueChanged);
		this.gvBatches.ValidatingEditor += new DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventHandler(this.gvBatches_ValidatingEditor);
		this.gc_Batch_Batch.Caption = "Batch";
		this.gc_Batch_Batch.ColumnEdit = this.rBatchNo;
		this.gc_Batch_Batch.CurrencyDateField = null;
		this.gc_Batch_Batch.CurrencyField = null;
		this.gc_Batch_Batch.CurrencyFixed = null;
		this.gc_Batch_Batch.CurrencyFixedRate = 0.0;
		this.gc_Batch_Batch.CurrencyRateField = null;
		this.gc_Batch_Batch.FieldName = "DistNumber";
		this.gc_Batch_Batch.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gc_Batch_Batch.Name = "gc_Batch_Batch";
		this.gc_Batch_Batch.PrimaryKey = true;
		this.gc_Batch_Batch.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gc_Batch_Batch.UnitField = null;
		this.gc_Batch_Batch.Visible = true;
		this.gc_Batch_Batch.VisibleIndex = 0;
		this.gc_Batch_Batch.Width = 95;
		this.gc_Batch_Qty.Caption = "Qty";
		this.gc_Batch_Qty.ColumnEdit = this.rBatch_Cal;
		this.gc_Batch_Qty.CurrencyDateField = null;
		this.gc_Batch_Qty.CurrencyField = null;
		this.gc_Batch_Qty.CurrencyFixed = null;
		this.gc_Batch_Qty.CurrencyFixedRate = 0.0;
		this.gc_Batch_Qty.CurrencyRateField = null;
		this.gc_Batch_Qty.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gc_Batch_Qty.FieldName = "LogQty";
		this.gc_Batch_Qty.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gc_Batch_Qty.Name = "gc_Batch_Qty";
		this.gc_Batch_Qty.PrimaryKey = false;
		this.gc_Batch_Qty.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gc_Batch_Qty.UnitField = null;
		this.gc_Batch_Qty.Visible = true;
		this.gc_Batch_Qty.VisibleIndex = 1;
		this.gc_Batch_VaribleQty.Caption = "Available Qty";
		this.gc_Batch_VaribleQty.ColumnEdit = this.rBatch_Cal;
		this.gc_Batch_VaribleQty.CurrencyDateField = null;
		this.gc_Batch_VaribleQty.CurrencyField = null;
		this.gc_Batch_VaribleQty.CurrencyFixed = null;
		this.gc_Batch_VaribleQty.CurrencyFixedRate = 0.0;
		this.gc_Batch_VaribleQty.CurrencyRateField = null;
		this.gc_Batch_VaribleQty.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gc_Batch_VaribleQty.FieldName = "SelectedQty";
		this.gc_Batch_VaribleQty.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gc_Batch_VaribleQty.Name = "gc_Batch_VaribleQty";
		this.gc_Batch_VaribleQty.OptionsColumn.ReadOnly = true;
		this.gc_Batch_VaribleQty.PrimaryKey = false;
		this.gc_Batch_VaribleQty.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gc_Batch_VaribleQty.UnitField = null;
		this.gc_Bath_MnfSerial.Caption = "Batch Attribute 1";
		this.gc_Bath_MnfSerial.CurrencyDateField = null;
		this.gc_Bath_MnfSerial.CurrencyField = null;
		this.gc_Bath_MnfSerial.CurrencyFixed = null;
		this.gc_Bath_MnfSerial.CurrencyFixedRate = 0.0;
		this.gc_Bath_MnfSerial.CurrencyRateField = null;
		this.gc_Bath_MnfSerial.FieldName = "MnfSerial";
		this.gc_Bath_MnfSerial.Name = "gc_Bath_MnfSerial";
		this.gc_Bath_MnfSerial.PrimaryKey = false;
		this.gc_Bath_MnfSerial.UnitField = null;
		this.gc_Bath_MnfSerial.Visible = true;
		this.gc_Bath_MnfSerial.VisibleIndex = 2;
		this.gc_Bath_MnfSerial.Width = 103;
		this.gc_Batch_LotNumber.Caption = "Batch Attribute 2";
		this.gc_Batch_LotNumber.CurrencyDateField = null;
		this.gc_Batch_LotNumber.CurrencyField = null;
		this.gc_Batch_LotNumber.CurrencyFixed = null;
		this.gc_Batch_LotNumber.CurrencyFixedRate = 0.0;
		this.gc_Batch_LotNumber.CurrencyRateField = null;
		this.gc_Batch_LotNumber.FieldName = "LotNumber";
		this.gc_Batch_LotNumber.Name = "gc_Batch_LotNumber";
		this.gc_Batch_LotNumber.PrimaryKey = false;
		this.gc_Batch_LotNumber.UnitField = null;
		this.gc_Batch_LotNumber.Visible = true;
		this.gc_Batch_LotNumber.VisibleIndex = 3;
		this.gc_Batch_LotNumber.Width = 111;
		this.gc_BinLocation.Caption = "Bin Location";
		this.gc_BinLocation.ColumnEdit = this.rBinCode;
		this.gc_BinLocation.CurrencyDateField = null;
		this.gc_BinLocation.CurrencyField = null;
		this.gc_BinLocation.CurrencyFixed = null;
		this.gc_BinLocation.CurrencyFixedRate = 0.0;
		this.gc_BinLocation.CurrencyRateField = null;
		this.gc_BinLocation.FieldName = "BinCode";
		this.gc_BinLocation.Name = "gc_BinLocation";
		this.gc_BinLocation.PrimaryKey = false;
		this.gc_BinLocation.UnitField = null;
		this.gc_BinLocation.Visible = true;
		this.gc_BinLocation.VisibleIndex = 4;
		this.rBinCode.AutoHeight = false;
		editorButtonImageOptions2.Image = Apzon.Forms.Properties.Resources.right;
		this.rBinCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rBinCode.Name = "rBinCode";
		this.rBinCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rBinCode_ButtonClick);
		this.gc_Batch_Status.Caption = "Status";
		this.gc_Batch_Status.ColumnEdit = this.rBatch_Status;
		this.gc_Batch_Status.CurrencyDateField = null;
		this.gc_Batch_Status.CurrencyField = null;
		this.gc_Batch_Status.CurrencyFixed = null;
		this.gc_Batch_Status.CurrencyFixedRate = 0.0;
		this.gc_Batch_Status.CurrencyRateField = null;
		this.gc_Batch_Status.FieldName = "Status";
		this.gc_Batch_Status.Name = "gc_Batch_Status";
		this.gc_Batch_Status.PrimaryKey = false;
		this.gc_Batch_Status.UnitField = null;
		this.gc_Batch_Status.Visible = true;
		this.gc_Batch_Status.VisibleIndex = 5;
		this.gc_Batch_ExpDate.Caption = "Expiration Date";
		this.gc_Batch_ExpDate.ColumnEdit = this.rBatch_Date;
		this.gc_Batch_ExpDate.CurrencyDateField = null;
		this.gc_Batch_ExpDate.CurrencyField = null;
		this.gc_Batch_ExpDate.CurrencyFixed = null;
		this.gc_Batch_ExpDate.CurrencyFixedRate = 0.0;
		this.gc_Batch_ExpDate.CurrencyRateField = null;
		this.gc_Batch_ExpDate.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gc_Batch_ExpDate.FieldName = "ExpDate";
		this.gc_Batch_ExpDate.Name = "gc_Batch_ExpDate";
		this.gc_Batch_ExpDate.PrimaryKey = false;
		this.gc_Batch_ExpDate.UnitField = null;
		this.gc_Batch_ExpDate.Visible = true;
		this.gc_Batch_ExpDate.VisibleIndex = 6;
		this.gc_Batch_ExpDate.Width = 92;
		this.gc_Batch_MnfDate.Caption = "Mfr Date";
		this.gc_Batch_MnfDate.ColumnEdit = this.rBatch_Date;
		this.gc_Batch_MnfDate.CurrencyDateField = null;
		this.gc_Batch_MnfDate.CurrencyField = null;
		this.gc_Batch_MnfDate.CurrencyFixed = null;
		this.gc_Batch_MnfDate.CurrencyFixedRate = 0.0;
		this.gc_Batch_MnfDate.CurrencyRateField = null;
		this.gc_Batch_MnfDate.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gc_Batch_MnfDate.FieldName = "MnfDate";
		this.gc_Batch_MnfDate.Name = "gc_Batch_MnfDate";
		this.gc_Batch_MnfDate.PrimaryKey = false;
		this.gc_Batch_MnfDate.UnitField = null;
		this.gc_Batch_MnfDate.Visible = true;
		this.gc_Batch_MnfDate.VisibleIndex = 7;
		this.gc_Batch_InDate.Caption = "Admission Date";
		this.gc_Batch_InDate.ColumnEdit = this.rBatch_Date;
		this.gc_Batch_InDate.CurrencyDateField = null;
		this.gc_Batch_InDate.CurrencyField = null;
		this.gc_Batch_InDate.CurrencyFixed = null;
		this.gc_Batch_InDate.CurrencyFixedRate = 0.0;
		this.gc_Batch_InDate.CurrencyRateField = null;
		this.gc_Batch_InDate.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gc_Batch_InDate.FieldName = "InDate";
		this.gc_Batch_InDate.Name = "gc_Batch_InDate";
		this.gc_Batch_InDate.PrimaryKey = false;
		this.gc_Batch_InDate.UnitField = null;
		this.gc_Batch_InDate.Visible = true;
		this.gc_Batch_InDate.VisibleIndex = 8;
		this.gc_Batch_InDate.Width = 88;
		this.gc_Batch_Location.Caption = "Location";
		this.gc_Batch_Location.CurrencyDateField = null;
		this.gc_Batch_Location.CurrencyField = null;
		this.gc_Batch_Location.CurrencyFixed = null;
		this.gc_Batch_Location.CurrencyFixedRate = 0.0;
		this.gc_Batch_Location.CurrencyRateField = null;
		this.gc_Batch_Location.FieldName = "Location";
		this.gc_Batch_Location.Name = "gc_Batch_Location";
		this.gc_Batch_Location.PrimaryKey = false;
		this.gc_Batch_Location.UnitField = null;
		this.gc_Batch_Location.Visible = true;
		this.gc_Batch_Location.VisibleIndex = 9;
		this.gc_Batch_Notes.Caption = "Details";
		this.gc_Batch_Notes.CurrencyDateField = null;
		this.gc_Batch_Notes.CurrencyField = null;
		this.gc_Batch_Notes.CurrencyFixed = null;
		this.gc_Batch_Notes.CurrencyFixedRate = 0.0;
		this.gc_Batch_Notes.CurrencyRateField = null;
		this.gc_Batch_Notes.FieldName = "Notes";
		this.gc_Batch_Notes.Name = "gc_Batch_Notes";
		this.gc_Batch_Notes.PrimaryKey = false;
		this.gc_Batch_Notes.UnitField = null;
		this.gc_Batch_Notes.Visible = true;
		this.gc_Batch_Notes.VisibleIndex = 10;
		this.rBinAllowBinCode.AutoHeight = false;
		this.rBinAllowBinCode.AutoLoadDataSource = true;
		editorButtonImageOptions3.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions4.Image");
		this.rBinAllowBinCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance4, appearanceHovered4, appearancePressed4, appearanceDisabled4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rBinAllowBinCode.ContainerControl = this;
		this.rBinAllowBinCode.DisplayMember = "BinCode";
		this.rBinAllowBinCode.DisplayMember2 = "";
		this.rBinAllowBinCode.LoadDataSourceWhenEnter = false;
		this.rBinAllowBinCode.Name = "rBinAllowBinCode";
		this.rBinAllowBinCode.SeparateCharacterForMulti = ';';
		this.rBinAllowBinCode.SeparateString = ", ";
		this.rBinAllowBinCode.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.GetDataBin;
		this.rBinAllowBinCode.UsingAutoCompleteBox = false;
		this.rBinAllowBinCode.UsingDataSource = true;
		this.rBinAllowBinCode.ValidateWhenNoDataSource = false;
		this.rBinAllowBinCode.ValueMember = "BinCode";
		this.gvBinAllowcate.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvBinAllowcate.ColumnKey = "BinCode";
		this.gvBinAllowcate.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[5] { this.gcBinAllowBinCode, this.gcBinAllowQty, this.gcBinAllowAvailQty, this.gcBinAllowCreateDate, this.gcBinAllowDetail });
		this.gvBinAllowcate.DisableFormSetting = false;
		this.gvBinAllowcate.DisplayException = false;
		this.gvBinAllowcate.EditColumn = null;
		this.gvBinAllowcate.FieldCurrency = null;
		this.gvBinAllowcate.FieldRate = null;
		this.gvBinAllowcate.FormId = null;
		this.gvBinAllowcate.GridControl = this.grdBatches;
		this.gvBinAllowcate.IgnoreFilter = false;
		this.gvBinAllowcate.IndexEditRow = -1;
		this.gvBinAllowcate.IndicatorWidth = 50;
		this.gvBinAllowcate.IsApzTable = false;
		this.gvBinAllowcate.IsInsertNewRow = false;
		this.gvBinAllowcate.IsLoadingData = false;
		this.gvBinAllowcate.IsMappingContextAction = false;
		this.gvBinAllowcate.ItemId = "oBinAllowcation";
		this.gvBinAllowcate.LstColumnAllowBatch = null;
		this.gvBinAllowcate.LstColumnNotAllowBatch = null;
		this.gvBinAllowcate.LstColumnNotEditable = null;
		this.gvBinAllowcate.LstColumNotSetting = null;
		this.gvBinAllowcate.Name = "gvBinAllowcate";
		this.gvBinAllowcate.OptionsSelection.MultiSelect = true;
		this.gvBinAllowcate.OptionsView.ColumnAutoWidth = false;
		this.gvBinAllowcate.OptionsView.ShowGroupPanel = false;
		this.gvBinAllowcate.UseBatchMod = false;
		this.gvBinAllowcate.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gvBinAllowcate_ShowingEditor);
		this.gvBinAllowcate.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvBinAllowcate_CellValueChanged);
		this.gcBinAllowBinCode.Caption = "Bin Code";
		this.gcBinAllowBinCode.ColumnEdit = this.rBinAllowBinCode;
		this.gcBinAllowBinCode.CurrencyDateField = null;
		this.gcBinAllowBinCode.CurrencyField = null;
		this.gcBinAllowBinCode.CurrencyFixed = null;
		this.gcBinAllowBinCode.CurrencyFixedRate = 0.0;
		this.gcBinAllowBinCode.CurrencyRateField = null;
		this.gcBinAllowBinCode.FieldName = "BinCode";
		this.gcBinAllowBinCode.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gcBinAllowBinCode.Name = "gcBinAllowBinCode";
		this.gcBinAllowBinCode.PrimaryKey = false;
		this.gcBinAllowBinCode.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcBinAllowBinCode.UnitField = null;
		this.gcBinAllowBinCode.Visible = true;
		this.gcBinAllowBinCode.VisibleIndex = 0;
		this.gcBinAllowBinCode.Width = 95;
		this.gcBinAllowQty.Caption = "Qty";
		this.gcBinAllowQty.ColumnEdit = this.rBatch_Cal;
		this.gcBinAllowQty.CurrencyDateField = null;
		this.gcBinAllowQty.CurrencyField = null;
		this.gcBinAllowQty.CurrencyFixed = null;
		this.gcBinAllowQty.CurrencyFixedRate = 0.0;
		this.gcBinAllowQty.CurrencyRateField = null;
		this.gcBinAllowQty.FieldName = "LogQty";
		this.gcBinAllowQty.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gcBinAllowQty.Name = "gcBinAllowQty";
		this.gcBinAllowQty.PrimaryKey = false;
		this.gcBinAllowQty.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gcBinAllowQty.UnitField = null;
		this.gcBinAllowQty.Visible = true;
		this.gcBinAllowQty.VisibleIndex = 1;
		this.gcBinAllowAvailQty.Caption = "Available Qty";
		this.gcBinAllowAvailQty.ColumnEdit = this.rBatch_Cal;
		this.gcBinAllowAvailQty.CurrencyDateField = null;
		this.gcBinAllowAvailQty.CurrencyField = null;
		this.gcBinAllowAvailQty.CurrencyFixed = null;
		this.gcBinAllowAvailQty.CurrencyFixedRate = 0.0;
		this.gcBinAllowAvailQty.CurrencyRateField = null;
		this.gcBinAllowAvailQty.FieldName = "SelectedQty";
		this.gcBinAllowAvailQty.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gcBinAllowAvailQty.Name = "gcBinAllowAvailQty";
		this.gcBinAllowAvailQty.OptionsColumn.ReadOnly = true;
		this.gcBinAllowAvailQty.PrimaryKey = false;
		this.gcBinAllowAvailQty.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gcBinAllowAvailQty.UnitField = null;
		this.gcBinAllowAvailQty.Visible = true;
		this.gcBinAllowAvailQty.VisibleIndex = 2;
		this.gcBinAllowCreateDate.Caption = "Create date";
		this.gcBinAllowCreateDate.ColumnEdit = this.rBatch_Date;
		this.gcBinAllowCreateDate.CurrencyDateField = null;
		this.gcBinAllowCreateDate.CurrencyField = null;
		this.gcBinAllowCreateDate.CurrencyFixed = null;
		this.gcBinAllowCreateDate.CurrencyFixedRate = 0.0;
		this.gcBinAllowCreateDate.CurrencyRateField = null;
		this.gcBinAllowCreateDate.FieldName = "CreateDate";
		this.gcBinAllowCreateDate.Name = "gcBinAllowCreateDate";
		this.gcBinAllowCreateDate.PrimaryKey = false;
		this.gcBinAllowCreateDate.UnitField = null;
		this.gcBinAllowCreateDate.Visible = true;
		this.gcBinAllowCreateDate.VisibleIndex = 3;
		this.gcBinAllowCreateDate.Width = 92;
		this.gcBinAllowDetail.Caption = "Details";
		this.gcBinAllowDetail.CurrencyDateField = null;
		this.gcBinAllowDetail.CurrencyField = null;
		this.gcBinAllowDetail.CurrencyFixed = null;
		this.gcBinAllowDetail.CurrencyFixedRate = 0.0;
		this.gcBinAllowDetail.CurrencyRateField = null;
		this.gcBinAllowDetail.FieldName = "BinDescription";
		this.gcBinAllowDetail.Name = "gcBinAllowDetail";
		this.gcBinAllowDetail.PrimaryKey = false;
		this.gcBinAllowDetail.UnitField = null;
		this.gcBinAllowDetail.Visible = true;
		this.gcBinAllowDetail.VisibleIndex = 4;
		this.btnRotate.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btnRotate.EditControl = null;
		this.btnRotate.EditValue = "";
		this.btnRotate.FieldName = null;
		this.btnRotate.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnRotate.ImageOptions.Image");
		this.btnRotate.IsSearchControl = true;
		this.btnRotate.Location = new System.Drawing.Point(887, 2);
		this.btnRotate.Name = "btnRotate";
		this.btnRotate.ReadOnly = false;
		this.btnRotate.SettingInVisible = false;
		this.btnRotate.SettingReadonly = false;
		this.btnRotate.Size = new System.Drawing.Size(24, 23);
		this.btnRotate.TabIndex = 10;
		this.btnRotate.Click += new System.EventHandler(this.btnRotate_Click);
		this.btnHide.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btnHide.EditControl = null;
		this.btnHide.EditValue = "";
		this.btnHide.FieldName = null;
		this.btnHide.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnHide.ImageOptions.Image");
		this.btnHide.IsSearchControl = true;
		this.btnHide.Location = new System.Drawing.Point(857, 2);
		this.btnHide.Name = "btnHide";
		this.btnHide.ReadOnly = false;
		this.btnHide.SettingInVisible = false;
		this.btnHide.SettingReadonly = false;
		this.btnHide.Size = new System.Drawing.Size(25, 23);
		this.btnHide.TabIndex = 11;
		this.btnHide.Click += new System.EventHandler(this.btnHide_Click);
		this.contextMenuStrip_Bin.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.autoSelectToolBin, this.binAllowDeleteRowToolStripMenuItem });
		this.contextMenuStrip_Bin.Name = "contextMenuStrip_Bin";
		this.contextMenuStrip_Bin.Size = new System.Drawing.Size(135, 48);
		this.contextMenuStrip_Bin.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Bin_Opening);
		this.autoSelectToolBin.Name = "autoSelectToolBin";
		this.autoSelectToolBin.Size = new System.Drawing.Size(134, 22);
		this.autoSelectToolBin.Text = "Auto Select";
		this.autoSelectToolBin.Click += new System.EventHandler(this.autoSelectToolBin_Click);
		this.binAllowDeleteRowToolStripMenuItem.Name = "binAllowDeleteRowToolStripMenuItem";
		this.binAllowDeleteRowToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
		this.binAllowDeleteRowToolStripMenuItem.Text = "Delete Row";
		this.binAllowDeleteRowToolStripMenuItem.Click += new System.EventHandler(this.deleteRowToolStripMenuItem_Click);
		this.lbTitle.EditControl = null;
		this.lbTitle.EditValue = "labelControl1";
		this.lbTitle.FieldName = null;
		this.lbTitle.HideWhenEmpty = false;
		this.lbTitle.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbTitle.Location = new System.Drawing.Point(4, 4);
		this.lbTitle.Name = "lbTitle";
		this.lbTitle.ReadOnly = false;
		this.lbTitle.SettingInVisible = false;
		this.lbTitle.SettingReadonly = false;
		this.lbTitle.ShowUnderLine = true;
		this.lbTitle.Size = new System.Drawing.Size(241, 20);
		this.lbTitle.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbTitle.TabIndex = 0;
		this.lbTitle.Utext = "labelControl1";
		this.lbOpenQty.EditControl = null;
		this.lbOpenQty.EditValue = "Open Quantity";
		this.lbOpenQty.FieldName = null;
		this.lbOpenQty.HideWhenEmpty = false;
		this.lbOpenQty.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbOpenQty.Location = new System.Drawing.Point(306, 31);
		this.lbOpenQty.Name = "lbOpenQty";
		this.lbOpenQty.ReadOnly = false;
		this.lbOpenQty.SettingInVisible = false;
		this.lbOpenQty.SettingReadonly = false;
		this.lbOpenQty.ShowUnderLine = true;
		this.lbOpenQty.Size = new System.Drawing.Size(80, 20);
		this.lbOpenQty.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbOpenQty.TabIndex = 12;
		this.lbOpenQty.Utext = "Open Quantity";
		this.txtOpenQty.EditControl = null;
		this.txtOpenQty.FieldName = "txtOpenQty";
		this.txtOpenQty.IsSearchControl = false;
		this.txtOpenQty.Location = new System.Drawing.Point(387, 28);
		this.txtOpenQty.Name = "txtOpenQty";
		this.txtOpenQty.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtOpenQty.Properties.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.txtOpenQty.Properties.IsSearchControl = false;
		this.txtOpenQty.Properties.ReadOnly = true;
		this.txtOpenQty.SettingInVisible = false;
		this.txtOpenQty.SettingReadonly = false;
		this.txtOpenQty.Size = new System.Drawing.Size(170, 20);
		this.txtOpenQty.TabIndex = 13;
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
		base.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		base.Controls.Add(this.txtOpenQty);
		base.Controls.Add(this.lbOpenQty);
		base.Controls.Add(this.lbTitle);
		base.Controls.Add(this.btnHide);
		base.Controls.Add(this.btnRotate);
		base.Controls.Add(this.txtBatchNo);
		base.Controls.Add(this.lbBatchNo);
		base.Controls.Add(this.grdBatches);
		base.Name = "ucBatchAndSerial";
		base.Size = new System.Drawing.Size(915, 280);
		base.Load += new System.EventHandler(this.ucBatchAndSerial_Load);
		((System.ComponentModel.ISupportInitialize)this.rBatchNo).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rBatch_Cal).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rBatch_Status).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rBatch_Date.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rBatch_Date).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtBatchNo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grdBatches).EndInit();
		this.contextMenuStrip_Batch.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gvBatches).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rBinCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rBinAllowBinCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gvBinAllowcate).EndInit();
		this.contextMenuStrip_Bin.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txtOpenQty.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
