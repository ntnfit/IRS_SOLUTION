using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
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
using Apzon.Entities.Models.Common;
using Apzon.Forms.BusinessMaster;
using Apzon.Forms.Commons;
using Apzon.Forms.Helper;
using Apzon.Forms.Properties;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.Sales;

public class frm_ListDocumentDraft : XtraFormEx
{
	public delegate void ShowForms(XtraFormEx frm);

	public string FilterObject = "ODRF";

	public string FilterString = "";

	public ShowForms frmShowChildrenForm;

	private DataTable dtObject = new DataTable();

	private DataTable dt = new DataTable();

	private readonly DataTable _dtDeleteCloseDocs = new DataTable();

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient(ApzObjectTypes.oDrafts);

	private IContainer components;

	private GridControlEx grdDocumentDraft;

	private GridColumnEx gcDocument;

	private GridColumnEx gcDocEntry;

	private GridColumnEx gcDocNum;

	private GridColumnEx gcPostingDate;

	private GridColumnEx gcBPCode;

	private GridColumnEx gcTotal;

	private GridColumnEx gcComments;

	private SimpleButtonEx btBack;

	private RepositoryItemButtonEdit rIBt_DocNum;

	private RepositoryItemButtonEdit rIBt_CardCode;

	private GridColumnEx gcReqName;

	private GridColumnEx gcRequester;

	private GridColumnEx gcUserSign;

	private GridColumnEx gcObjType;

	private GridColumnEx gcRequriedDate;

	private GridColumnEx gcDocTotalSy;

	private GridColumnEx gcDocTotalFc;

	private GridColumnEx gcDocCurrency;

	private RepositoryItemDateEdit repositoryItemDateEdit1;

	private GridColumnEx gcDocDueDate;

	private GridColumnEx gcCreateDate;

	private GridColumnEx gcDateUpdate;

	private GridColumnEx gcDocumentDate;

	private GridViewEx gvDocumentDraft;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem closeToolStripMenuItem;

	private ToolStripMenuItem removeToolStripMenuItem;

	private SimpleButtonEx btOK;

	private SimpleButtonEx btCancel;

	private GridColumnEx gcDocStatus;

	private GridColumnEx gridColumn1;

	private GridColumnEx gridColumn2;

	private GridColumnEx gc_BPName;

	private GridColumnEx gc_ReqByDept;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx4;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx1;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx2;

	public DataTable dtSearch { get; set; }

	public frm_ListDocumentDraft(DataTable dtSearch)
	{
		InitializeComponent();
		SettingLaguage();
		dt = dtSearch;
		WinFuntions.SetFormatControlValue(this);
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
			btOK.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btOK.Text = Apzon.Commons.Properties.Resources.ButtonOK;
			break;
		}
	}

	private async void frm_ListDocumentDraft_Load(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			gvDocumentDraft.ShowLoadingPanel();
			_dtDeleteCloseDocs.Columns.Add(new DataColumn("UpdateType", typeof(string)));
			_dtDeleteCloseDocs.Columns.Add(new DataColumn("Id", typeof(int)));
			WinFuntions.LoadUdfColumns(gvDocumentDraft, "ODRF");
			FilterString = await WinFuntions.ShowFilterSetting(FilterObject);
			await Search(dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			gvDocumentDraft.HideLoadingPanel();
		}
	}

	private async Task Search(DataTable dt)
	{
		try
		{
			if (!dt.IsExistsColumn("FilterString"))
			{
				dt.Columns.Add("FilterString", typeof(string));
				if (dt.IsNotNull())
				{
					dt.Rows[0]["FilterString"] = FilterString;
				}
			}
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "DocumentDraft");
			dtObject = await helper.PostAsync("SearchDoc", dt);
			if (dtObject != null && dtObject.Rows.Count > 0)
			{
				dtObject.PrimaryKey = new DataColumn[1] { dtObject.Columns["DocEntry"] };
				grdDocumentDraft.DataSource = dtObject;
				grdDocumentDraft.DataSourceTable().AcceptChanges();
				grdDocumentDraft.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btOK_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				CopyFrom();
			}
			else if (FormMode == Enums.FormViewModeType.Update && _dtDeleteCloseDocs.Rows.Count > 0)
			{
				await Save();
				_dtDeleteCloseDocs.Rows.Clear();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btBack_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void btCancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void gvDocumentDraft_DoubleClick(object sender, EventArgs e)
	{
		if (gvDocumentDraft.CalcHitInfo(grdDocumentDraft.PointToClient(Control.MousePosition)).InRow)
		{
			CopyFrom();
		}
	}

	private async void removeToolStripMenuItem_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			int num = Function.ParseInt(gvDocumentDraft.GetFocusedRowCellValue("DocEntry"));
			if (num > 0 && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmRemoveItem, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				HttpResult httpResult = await _httpClient.Documents.Remove(num);
				if (!string.IsNullOrEmpty(httpResult.Message))
				{
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
					return;
				}
				WinCommons.Success(Enums.MessageCode.Success, "");
				await Search(dt);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void closeToolStripMenuItem_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			int num = Function.ParseInt(gvDocumentDraft.GetFocusedRowCellValue("DocEntry"));
			if (num > 0 && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCloseItem, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				HttpResult httpResult = await _httpClient.Documents.Close(num);
				if (!string.IsNullOrEmpty(httpResult.Message))
				{
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
					return;
				}
				WinCommons.Success(Enums.MessageCode.Success, "");
				await Search(dt);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
	{
		try
		{
			if (((gvDocumentDraft.GetFocusedRowCellValue("DocStatus") == null || gvDocumentDraft.GetFocusedRowCellValue("DocStatus") == DBNull.Value) ? "C" : Function.ToString(gvDocumentDraft.GetFocusedRowCellValue("DocStatus"))).ToUpper() == "O")
			{
				closeToolStripMenuItem.Visible = true;
			}
			else
			{
				closeToolStripMenuItem.Visible = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rIBt_CardCode_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_BusinessMasterData frm_BusinessMasterData2 = new frm_BusinessMasterData();
			frm_BusinessMasterData2._copyF_CodeCustomer = Function.ToString(gvDocumentDraft.GetFocusedRowCellValue("CardCode"));
			frm_BusinessMasterData2.Show();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rIBt_DocNum_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		CopyFrom();
	}

	private async Task Save()
	{
		try
		{
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "DocumentDraft");
			string text = await hp.PostAsJsonAsync("RemoveAndClose", _dtDeleteCloseDocs);
			if (string.IsNullOrWhiteSpace(text))
			{
				WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
				FormMode = Enums.FormViewModeType.View;
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void CopyFrom()
	{
		dt = new DataTable();
		dt.Columns.Add("DocNum", typeof(string));
		dt.Columns.Add("DocEntry", typeof(string));
		DataRow dataRow = dt.NewRow();
		dataRow["DocNum"] = Function.ToString(gvDocumentDraft.GetFocusedRowCellValue("DocNum"));
		dataRow["DocEntry"] = Function.ToString(gvDocumentDraft.GetFocusedRowCellValue("DocEntry"));
		dt.Rows.Add(dataRow);
		CopyFromDocumentDraft();
	}

	private void CopyFromDocumentDraft()
	{
		try
		{
			string docName = Function.ToString(gvDocumentDraft.GetFocusedRowCellValue("Document"));
			int num = Function.ParseInt(gvDocumentDraft.GetFocusedRowCellValue("DocEntry"));
			this.DrillDown(ApzObjectTypes.oDrafts, GetDocTypeByName(docName), num.ToString());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private ApzObjectTypes GetDocTypeByName(string docName)
	{
		try
		{
			ApzObjectTypes result = ApzObjectTypes.oDrafts;
			switch (docName)
			{
			case "Sales Quotation":
				result = ApzObjectTypes.oQuotations;
				break;
			case "Sales Order":
				result = ApzObjectTypes.oOrders;
				break;
			case "Delivery":
				result = ApzObjectTypes.oDeliveryNotes;
				break;
			case "Return":
				result = ApzObjectTypes.oReturns;
				break;
			case "A/R Invoices":
				result = ApzObjectTypes.oInvoices;
				break;
			case "A/R Reserve Invoices":
				result = ApzObjectTypes.oReserveInvoices;
				break;
			case "A/R Credit Memos":
				result = ApzObjectTypes.oCreditNotes;
				break;
			case "A/R DownPayment":
				result = ApzObjectTypes.oDownPayments;
				break;
			case "Purchase Request":
				result = ApzObjectTypes.oPurchaseRequest;
				break;
			case "Purchase Quotation":
				result = ApzObjectTypes.oPurchaseQuotations;
				break;
			case "Purchase Order":
				result = ApzObjectTypes.oPurchaseOrders;
				break;
			case "Goods Receipt P/O":
				result = ApzObjectTypes.oPurchaseDeliveryNotes;
				break;
			case "Goods Return":
				result = ApzObjectTypes.oPurchaseReturns;
				break;
			case "A/P Invoices":
				result = ApzObjectTypes.oPurchaseInvoices;
				break;
			case "A/P Credit Memos":
				result = ApzObjectTypes.oPurchaseCreditNotes;
				break;
			case "Landed Costs":
				result = ApzObjectTypes.oLandedCostsCodes;
				break;
			case "A/P Down Payment":
				result = ApzObjectTypes.oPurchaseDownPayments;
				break;
			case "Incoming Payments":
				result = ApzObjectTypes.oIncomingPayments;
				break;
			case "Outgoing Payments":
				result = ApzObjectTypes.oVendorPayments;
				break;
			case "Inventory Transfer Request":
				result = ApzObjectTypes.oInventoryTransferRequest;
				break;
			case "Inventory Transfer":
				result = ApzObjectTypes.oStockTransfer;
				break;
			case "Goods Issue":
				result = ApzObjectTypes.oInventoryGenExit;
				break;
			case "Goods Receipt":
				result = ApzObjectTypes.oInventoryGenEntry;
				break;
			case "Inventory Revaluation":
				result = ApzObjectTypes.oMaterialRevaluation;
				break;
			case "Production Orders":
				result = ApzObjectTypes.oProductionOrders;
				break;
			case "Blanket Agreements":
				result = ApzObjectTypes.oBlanketAgreement;
				break;
			}
			return result;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return ApzObjectTypes.oDrafts;
		}
	}

	private void SettingLaguage()
	{
		btCancel.Text = Apzon.Commons.Properties.Resources.ButtonCancel;
		FormMode = Enums.FormViewModeType.View;
		gcPostingDate.Caption = Apzon.Commons.Properties.Resources.Doc_gvDoc_gcPostingDate;
		gcDocDueDate.Caption = Apzon.Commons.Properties.Resources.Doc_gvDoc_gcDocDueDate;
		gcCreateDate.Caption = Apzon.Commons.Properties.Resources.Doc_gvDoc_CreateDate;
		gcBPCode.Caption = Apzon.Commons.Properties.Resources.Doc_gvDoc_gcBPCode;
		gcComments.Caption = Apzon.Commons.Properties.Resources.Doc_gvDoc_Remark;
		gcDocCurrency.Caption = Apzon.Commons.Properties.Resources.Doc_gvDoc_gcDocCurrency;
		gcReqName.Caption = Apzon.Commons.Properties.Resources.Doc_gvDoc_ReqName;
		gcRequester.Caption = Apzon.Commons.Properties.Resources.Doc_gvDoc_gcRequester;
		gcDocTotalFc.Caption = Apzon.Commons.Properties.Resources.Doc_gvDoc_gcDocTotalFc;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Sales.frm_ListDocumentDraft));
		this.repositoryItemCalcEditEx4 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx1 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx2 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.grdDocumentDraft = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gvDocumentDraft = new Apzon.Controls.Grid.GridViewEx();
		this.gcDocument = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDocEntry = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDocNum = new Apzon.Controls.Grid.GridColumnEx();
		this.rIBt_DocNum = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcPostingDate = new Apzon.Controls.Grid.GridColumnEx();
		this.gcBPCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rIBt_CardCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcTotal = new Apzon.Controls.Grid.GridColumnEx();
		this.gcComments = new Apzon.Controls.Grid.GridColumnEx();
		this.gcReqName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcRequester = new Apzon.Controls.Grid.GridColumnEx();
		this.gcUserSign = new Apzon.Controls.Grid.GridColumnEx();
		this.gcObjType = new Apzon.Controls.Grid.GridColumnEx();
		this.gcRequriedDate = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDocTotalSy = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDocTotalFc = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDocCurrency = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDocDueDate = new Apzon.Controls.Grid.GridColumnEx();
		this.gcCreateDate = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDateUpdate = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDocumentDate = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDocStatus = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_BPName = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_ReqByDept = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemDateEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.btOK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btBack = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grdDocumentDraft).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gvDocumentDraft).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rIBt_DocNum).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rIBt_CardCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit1.CalendarTimeProperties).BeginInit();
		base.SuspendLayout();
		this.repositoryItemCalcEditEx4.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx4.IsSearchControl = false;
		this.repositoryItemCalcEditEx4.Name = "repositoryItemCalcEditEx4";
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
		this.grdDocumentDraft.AllowPasteExcel = true;
		this.grdDocumentDraft.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grdDocumentDraft.ClipboardData = null;
		this.grdDocumentDraft.ContextMenuStrip = this.contextMenuStrip1;
		this.grdDocumentDraft.Location = new System.Drawing.Point(2, 2);
		this.grdDocumentDraft.MainView = this.gvDocumentDraft;
		this.grdDocumentDraft.Name = "grdDocumentDraft";
		this.grdDocumentDraft.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.rIBt_DocNum, this.rIBt_CardCode, this.repositoryItemDateEdit1 });
		this.grdDocumentDraft.Size = new System.Drawing.Size(604, 298);
		this.grdDocumentDraft.TabIndex = 0;
		this.grdDocumentDraft.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvDocumentDraft });
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.closeToolStripMenuItem, this.removeToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(118, 48);
		this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
		this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
		this.closeToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
		this.closeToolStripMenuItem.Text = "Close";
		this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
		this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
		this.removeToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
		this.removeToolStripMenuItem.Text = "Remove";
		this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
		this.gvDocumentDraft.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvDocumentDraft.ColumnKey = null;
		this.gvDocumentDraft.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[24]
		{
			this.gcDocument, this.gcDocEntry, this.gcDocNum, this.gcPostingDate, this.gcBPCode, this.gcTotal, this.gcComments, this.gcReqName, this.gcRequester, this.gcUserSign,
			this.gcObjType, this.gcRequriedDate, this.gcDocTotalSy, this.gcDocTotalFc, this.gcDocCurrency, this.gcDocDueDate, this.gcCreateDate, this.gcDateUpdate, this.gcDocumentDate, this.gcDocStatus,
			this.gridColumn1, this.gridColumn2, this.gc_BPName, this.gc_ReqByDept
		});
		this.gvDocumentDraft.DisableFormSetting = false;
		this.gvDocumentDraft.DisplayException = false;
		this.gvDocumentDraft.EditColumn = null;
		this.gvDocumentDraft.FieldCurrency = null;
		this.gvDocumentDraft.FieldRate = null;
		this.gvDocumentDraft.FormId = "oDocumentDraft";
		this.gvDocumentDraft.GridControl = this.grdDocumentDraft;
		this.gvDocumentDraft.IgnoreFilter = false;
		this.gvDocumentDraft.IndexEditRow = -1;
		this.gvDocumentDraft.IndicatorWidth = 35;
		this.gvDocumentDraft.IsApzTable = false;
		this.gvDocumentDraft.IsInsertNewRow = false;
		this.gvDocumentDraft.IsLoadingData = false;
		this.gvDocumentDraft.IsMappingContextAction = false;
		this.gvDocumentDraft.ItemId = "ODRF";
		this.gvDocumentDraft.LstColumnAllowBatch = null;
		this.gvDocumentDraft.LstColumnNotAllowBatch = null;
		this.gvDocumentDraft.LstColumnNotEditable = null;
		this.gvDocumentDraft.LstColumNotSetting = null;
		this.gvDocumentDraft.Name = "gvDocumentDraft";
		this.gvDocumentDraft.OptionsBehavior.ReadOnly = true;
		this.gvDocumentDraft.OptionsView.ColumnAutoWidth = false;
		this.gvDocumentDraft.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gvDocumentDraft.OptionsView.ShowGroupPanel = false;
		this.gvDocumentDraft.UseBatchMod = false;
		this.gvDocumentDraft.DoubleClick += new System.EventHandler(this.gvDocumentDraft_DoubleClick);
		this.gcDocument.Caption = "Document";
		this.gcDocument.CurrencyDateField = null;
		this.gcDocument.CurrencyField = null;
		this.gcDocument.CurrencyFixed = null;
		this.gcDocument.CurrencyFixedRate = 0.0;
		this.gcDocument.CurrencyRateField = null;
		this.gcDocument.FieldName = "Document";
		this.gcDocument.Name = "gcDocument";
		this.gcDocument.PrimaryKey = true;
		this.gcDocument.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcDocument.UnitField = null;
		this.gcDocument.Visible = true;
		this.gcDocument.VisibleIndex = 0;
		this.gcDocument.Width = 150;
		this.gcDocEntry.Caption = "DocEntry";
		this.gcDocEntry.CurrencyDateField = null;
		this.gcDocEntry.CurrencyField = null;
		this.gcDocEntry.CurrencyFixed = null;
		this.gcDocEntry.CurrencyFixedRate = 0.0;
		this.gcDocEntry.CurrencyRateField = null;
		this.gcDocEntry.FieldName = "DocEntry";
		this.gcDocEntry.Name = "gcDocEntry";
		this.gcDocEntry.PrimaryKey = false;
		this.gcDocEntry.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcDocEntry.UnitField = null;
		this.gcDocEntry.Visible = true;
		this.gcDocEntry.VisibleIndex = 1;
		this.gcDocEntry.Width = 100;
		this.gcDocNum.Caption = "Document No";
		this.gcDocNum.ColumnEdit = this.rIBt_DocNum;
		this.gcDocNum.CurrencyDateField = null;
		this.gcDocNum.CurrencyField = null;
		this.gcDocNum.CurrencyFixed = null;
		this.gcDocNum.CurrencyFixedRate = 0.0;
		this.gcDocNum.CurrencyRateField = null;
		this.gcDocNum.FieldName = "DocNum";
		this.gcDocNum.Name = "gcDocNum";
		this.gcDocNum.PrimaryKey = false;
		this.gcDocNum.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcDocNum.UnitField = null;
		this.gcDocNum.Visible = true;
		this.gcDocNum.VisibleIndex = 2;
		this.gcDocNum.Width = 100;
		this.rIBt_DocNum.AutoHeight = false;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.rIBt_DocNum.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rIBt_DocNum.Name = "rIBt_DocNum";
		this.rIBt_DocNum.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rIBt_DocNum_ButtonPressed);
		this.gcPostingDate.Caption = "Posting Date";
		this.gcPostingDate.CurrencyDateField = null;
		this.gcPostingDate.CurrencyField = null;
		this.gcPostingDate.CurrencyFixed = null;
		this.gcPostingDate.CurrencyFixedRate = 0.0;
		this.gcPostingDate.CurrencyRateField = null;
		this.gcPostingDate.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gcPostingDate.FieldName = "DocDate";
		this.gcPostingDate.Name = "gcPostingDate";
		this.gcPostingDate.PrimaryKey = false;
		this.gcPostingDate.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcPostingDate.UnitField = null;
		this.gcPostingDate.Visible = true;
		this.gcPostingDate.VisibleIndex = 3;
		this.gcPostingDate.Width = 90;
		this.gcBPCode.Caption = "BP Code";
		this.gcBPCode.ColumnEdit = this.rIBt_CardCode;
		this.gcBPCode.CurrencyDateField = null;
		this.gcBPCode.CurrencyField = null;
		this.gcBPCode.CurrencyFixed = null;
		this.gcBPCode.CurrencyFixedRate = 0.0;
		this.gcBPCode.CurrencyRateField = null;
		this.gcBPCode.FieldName = "CardCode";
		this.gcBPCode.Name = "gcBPCode";
		this.gcBPCode.PrimaryKey = false;
		this.gcBPCode.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcBPCode.UnitField = null;
		this.gcBPCode.Visible = true;
		this.gcBPCode.VisibleIndex = 8;
		this.gcBPCode.Width = 90;
		this.rIBt_CardCode.AutoHeight = false;
		editorButtonImageOptions2.Image = Apzon.Forms.Properties.Resources.right;
		this.rIBt_CardCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rIBt_CardCode.Name = "rIBt_CardCode";
		this.rIBt_CardCode.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rIBt_CardCode_ButtonPressed);
		this.gcTotal.Caption = "Total";
		this.gcTotal.ColumnEdit = this.repositoryItemCalcEditEx4;
		this.gcTotal.CurrencyDateField = null;
		this.gcTotal.CurrencyField = "DocCurrency";
		this.gcTotal.CurrencyFixed = null;
		this.gcTotal.CurrencyFixedRate = 0.0;
		this.gcTotal.CurrencyRateField = null;
		this.gcTotal.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gcTotal.FieldName = "DocTotal";
		this.gcTotal.Name = "gcTotal";
		this.gcTotal.PrimaryKey = false;
		this.gcTotal.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcTotal.UnitField = null;
		this.gcTotal.Visible = true;
		this.gcTotal.VisibleIndex = 10;
		this.gcTotal.Width = 100;
		this.gcComments.Caption = "Remarks";
		this.gcComments.CurrencyDateField = null;
		this.gcComments.CurrencyField = null;
		this.gcComments.CurrencyFixed = null;
		this.gcComments.CurrencyFixedRate = 0.0;
		this.gcComments.CurrencyRateField = null;
		this.gcComments.FieldName = "Comments";
		this.gcComments.Name = "gcComments";
		this.gcComments.PrimaryKey = false;
		this.gcComments.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcComments.UnitField = null;
		this.gcComments.Visible = true;
		this.gcComments.VisibleIndex = 11;
		this.gcComments.Width = 150;
		this.gcReqName.Caption = "ReqName";
		this.gcReqName.CurrencyDateField = null;
		this.gcReqName.CurrencyField = null;
		this.gcReqName.CurrencyFixed = null;
		this.gcReqName.CurrencyFixedRate = 0.0;
		this.gcReqName.CurrencyRateField = null;
		this.gcReqName.FieldName = "ReqName";
		this.gcReqName.Name = "gcReqName";
		this.gcReqName.PrimaryKey = false;
		this.gcReqName.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcReqName.UnitField = null;
		this.gcReqName.Visible = true;
		this.gcReqName.VisibleIndex = 12;
		this.gcReqName.Width = 100;
		this.gcRequester.Caption = "Requester";
		this.gcRequester.CurrencyDateField = null;
		this.gcRequester.CurrencyField = null;
		this.gcRequester.CurrencyFixed = null;
		this.gcRequester.CurrencyFixedRate = 0.0;
		this.gcRequester.CurrencyRateField = null;
		this.gcRequester.FieldName = "Requester";
		this.gcRequester.Name = "gcRequester";
		this.gcRequester.PrimaryKey = false;
		this.gcRequester.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcRequester.UnitField = null;
		this.gcRequester.Visible = true;
		this.gcRequester.VisibleIndex = 13;
		this.gcRequester.Width = 100;
		this.gcUserSign.Caption = "UserSign";
		this.gcUserSign.CurrencyDateField = null;
		this.gcUserSign.CurrencyField = null;
		this.gcUserSign.CurrencyFixed = null;
		this.gcUserSign.CurrencyFixedRate = 0.0;
		this.gcUserSign.CurrencyRateField = null;
		this.gcUserSign.FieldName = "UserSign";
		this.gcUserSign.Name = "gcUserSign";
		this.gcUserSign.PrimaryKey = false;
		this.gcUserSign.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcUserSign.UnitField = null;
		this.gcUserSign.Visible = true;
		this.gcUserSign.VisibleIndex = 19;
		this.gcUserSign.Width = 90;
		this.gcObjType.Caption = "ObjType";
		this.gcObjType.CurrencyDateField = null;
		this.gcObjType.CurrencyField = null;
		this.gcObjType.CurrencyFixed = null;
		this.gcObjType.CurrencyFixedRate = 0.0;
		this.gcObjType.CurrencyRateField = null;
		this.gcObjType.FieldName = "ObjType";
		this.gcObjType.Name = "gcObjType";
		this.gcObjType.PrimaryKey = false;
		this.gcObjType.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcObjType.UnitField = null;
		this.gcObjType.Width = 100;
		this.gcRequriedDate.Caption = "RequriedDate";
		this.gcRequriedDate.CurrencyDateField = null;
		this.gcRequriedDate.CurrencyField = null;
		this.gcRequriedDate.CurrencyFixed = null;
		this.gcRequriedDate.CurrencyFixedRate = 0.0;
		this.gcRequriedDate.CurrencyRateField = null;
		this.gcRequriedDate.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gcRequriedDate.FieldName = "RequriedDate";
		this.gcRequriedDate.Name = "gcRequriedDate";
		this.gcRequriedDate.PrimaryKey = false;
		this.gcRequriedDate.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcRequriedDate.UnitField = null;
		this.gcRequriedDate.Visible = true;
		this.gcRequriedDate.VisibleIndex = 18;
		this.gcRequriedDate.Width = 90;
		this.gcDocTotalSy.Caption = "DocTotalSy";
		this.gcDocTotalSy.ColumnEdit = this.repositoryItemCalcEditEx1;
		this.gcDocTotalSy.CurrencyDateField = null;
		this.gcDocTotalSy.CurrencyField = null;
		this.gcDocTotalSy.CurrencyFixed = null;
		this.gcDocTotalSy.CurrencyFixedRate = 0.0;
		this.gcDocTotalSy.CurrencyRateField = null;
		this.gcDocTotalSy.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gcDocTotalSy.FieldName = "DocTotalSy";
		this.gcDocTotalSy.Name = "gcDocTotalSy";
		this.gcDocTotalSy.PrimaryKey = false;
		this.gcDocTotalSy.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcDocTotalSy.UnitField = null;
		this.gcDocTotalSy.Visible = true;
		this.gcDocTotalSy.VisibleIndex = 14;
		this.gcDocTotalFc.Caption = "DocTotalFc";
		this.gcDocTotalFc.ColumnEdit = this.repositoryItemCalcEditEx2;
		this.gcDocTotalFc.CurrencyDateField = null;
		this.gcDocTotalFc.CurrencyField = null;
		this.gcDocTotalFc.CurrencyFixed = null;
		this.gcDocTotalFc.CurrencyFixedRate = 0.0;
		this.gcDocTotalFc.CurrencyRateField = null;
		this.gcDocTotalFc.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gcDocTotalFc.FieldName = "DocTotalFc";
		this.gcDocTotalFc.Name = "gcDocTotalFc";
		this.gcDocTotalFc.PrimaryKey = false;
		this.gcDocTotalFc.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcDocTotalFc.UnitField = null;
		this.gcDocTotalFc.Visible = true;
		this.gcDocTotalFc.VisibleIndex = 15;
		this.gcDocCurrency.Caption = "DocCurrency";
		this.gcDocCurrency.CurrencyDateField = null;
		this.gcDocCurrency.CurrencyField = null;
		this.gcDocCurrency.CurrencyFixed = null;
		this.gcDocCurrency.CurrencyFixedRate = 0.0;
		this.gcDocCurrency.CurrencyRateField = null;
		this.gcDocCurrency.FieldName = "DocCurrency";
		this.gcDocCurrency.Name = "gcDocCurrency";
		this.gcDocCurrency.PrimaryKey = false;
		this.gcDocCurrency.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcDocCurrency.UnitField = null;
		this.gcDocCurrency.Visible = true;
		this.gcDocCurrency.VisibleIndex = 16;
		this.gcDocDueDate.Caption = "Due Date";
		this.gcDocDueDate.CurrencyDateField = null;
		this.gcDocDueDate.CurrencyField = null;
		this.gcDocDueDate.CurrencyFixed = null;
		this.gcDocDueDate.CurrencyFixedRate = 0.0;
		this.gcDocDueDate.CurrencyRateField = null;
		this.gcDocDueDate.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gcDocDueDate.FieldName = "DocDueDate";
		this.gcDocDueDate.Name = "gcDocDueDate";
		this.gcDocDueDate.PrimaryKey = false;
		this.gcDocDueDate.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcDocDueDate.UnitField = null;
		this.gcDocDueDate.Visible = true;
		this.gcDocDueDate.VisibleIndex = 4;
		this.gcCreateDate.Caption = "Creation Date";
		this.gcCreateDate.CurrencyDateField = null;
		this.gcCreateDate.CurrencyField = null;
		this.gcCreateDate.CurrencyFixed = null;
		this.gcCreateDate.CurrencyFixedRate = 0.0;
		this.gcCreateDate.CurrencyRateField = null;
		this.gcCreateDate.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gcCreateDate.FieldName = "CreateDate";
		this.gcCreateDate.Name = "gcCreateDate";
		this.gcCreateDate.PrimaryKey = false;
		this.gcCreateDate.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcCreateDate.UnitField = null;
		this.gcCreateDate.Visible = true;
		this.gcCreateDate.VisibleIndex = 5;
		this.gcCreateDate.Width = 77;
		this.gcDateUpdate.Caption = "Date of Update";
		this.gcDateUpdate.CurrencyDateField = null;
		this.gcDateUpdate.CurrencyField = null;
		this.gcDateUpdate.CurrencyFixed = null;
		this.gcDateUpdate.CurrencyFixedRate = 0.0;
		this.gcDateUpdate.CurrencyRateField = null;
		this.gcDateUpdate.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gcDateUpdate.FieldName = "UpdateDate";
		this.gcDateUpdate.Name = "gcDateUpdate";
		this.gcDateUpdate.PrimaryKey = false;
		this.gcDateUpdate.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcDateUpdate.UnitField = null;
		this.gcDateUpdate.Visible = true;
		this.gcDateUpdate.VisibleIndex = 6;
		this.gcDateUpdate.Width = 93;
		this.gcDocumentDate.Caption = "Document Date";
		this.gcDocumentDate.CurrencyDateField = null;
		this.gcDocumentDate.CurrencyField = null;
		this.gcDocumentDate.CurrencyFixed = null;
		this.gcDocumentDate.CurrencyFixedRate = 0.0;
		this.gcDocumentDate.CurrencyRateField = null;
		this.gcDocumentDate.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gcDocumentDate.FieldName = "TaxDate";
		this.gcDocumentDate.Name = "gcDocumentDate";
		this.gcDocumentDate.PrimaryKey = false;
		this.gcDocumentDate.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcDocumentDate.UnitField = null;
		this.gcDocumentDate.Visible = true;
		this.gcDocumentDate.VisibleIndex = 7;
		this.gcDocumentDate.Width = 89;
		this.gcDocStatus.Caption = "Doc Status";
		this.gcDocStatus.CurrencyDateField = null;
		this.gcDocStatus.CurrencyField = null;
		this.gcDocStatus.CurrencyFixed = null;
		this.gcDocStatus.CurrencyFixedRate = 0.0;
		this.gcDocStatus.CurrencyRateField = null;
		this.gcDocStatus.FieldName = "DocStatus";
		this.gcDocStatus.Name = "gcDocStatus";
		this.gcDocStatus.PrimaryKey = false;
		this.gcDocStatus.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcDocStatus.UnitField = null;
		this.gcDocStatus.Visible = true;
		this.gcDocStatus.VisibleIndex = 17;
		this.gridColumn1.Caption = "ListNum";
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "ListNum";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.PrimaryKey = false;
		this.gridColumn1.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridColumn1.UnitField = null;
		this.gridColumn2.Caption = "ListName";
		this.gridColumn2.CurrencyDateField = null;
		this.gridColumn2.CurrencyField = null;
		this.gridColumn2.CurrencyFixed = null;
		this.gridColumn2.CurrencyFixedRate = 0.0;
		this.gridColumn2.CurrencyRateField = null;
		this.gridColumn2.FieldName = "ListName";
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.PrimaryKey = false;
		this.gridColumn2.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridColumn2.UnitField = null;
		this.gc_BPName.Caption = "BP Name";
		this.gc_BPName.CurrencyDateField = null;
		this.gc_BPName.CurrencyField = null;
		this.gc_BPName.CurrencyFixed = null;
		this.gc_BPName.CurrencyFixedRate = 0.0;
		this.gc_BPName.CurrencyRateField = null;
		this.gc_BPName.FieldName = "CardName";
		this.gc_BPName.Name = "gc_BPName";
		this.gc_BPName.PrimaryKey = false;
		this.gc_BPName.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gc_BPName.UnitField = null;
		this.gc_BPName.Visible = true;
		this.gc_BPName.VisibleIndex = 9;
		this.gc_BPName.Width = 168;
		this.gc_ReqByDept.Caption = "Req By Dept";
		this.gc_ReqByDept.CurrencyDateField = null;
		this.gc_ReqByDept.CurrencyField = null;
		this.gc_ReqByDept.CurrencyFixed = null;
		this.gc_ReqByDept.CurrencyFixedRate = 0.0;
		this.gc_ReqByDept.CurrencyRateField = null;
		this.gc_ReqByDept.FieldName = "U_dept";
		this.gc_ReqByDept.Name = "gc_ReqByDept";
		this.gc_ReqByDept.PrimaryKey = false;
		this.gc_ReqByDept.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gc_ReqByDept.UnitField = null;
		this.gc_ReqByDept.Visible = true;
		this.gc_ReqByDept.VisibleIndex = 20;
		this.repositoryItemDateEdit1.AutoHeight = false;
		this.repositoryItemDateEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEdit1.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEdit1.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.repositoryItemDateEdit1.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.repositoryItemDateEdit1.EditFormat.FormatString = "dd/MM/yyyy";
		this.repositoryItemDateEdit1.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.repositoryItemDateEdit1.Mask.EditMask = "dd/MM/yyyy";
		this.repositoryItemDateEdit1.Name = "repositoryItemDateEdit1";
		this.btOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btOK.EditControl = null;
		this.btOK.EditValue = "OK";
		this.btOK.FieldName = null;
		this.btOK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btOK.ImageOptions.Image");
		this.btOK.IsSearchControl = false;
		this.btOK.Location = new System.Drawing.Point(38, 306);
		this.btOK.Name = "btOK";
		this.btOK.ReadOnly = false;
		this.btOK.SettingInVisible = false;
		this.btOK.SettingReadonly = false;
		this.btOK.Size = new System.Drawing.Size(75, 25);
		this.btOK.TabIndex = 1;
		this.btOK.Text = "OK";
		this.btOK.Click += new System.EventHandler(this.btOK_Click);
		this.btCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btCancel.EditControl = null;
		this.btCancel.EditValue = "Cancel";
		this.btCancel.FieldName = null;
		this.btCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btCancel.ImageOptions.Image");
		this.btCancel.IsSearchControl = false;
		this.btCancel.Location = new System.Drawing.Point(119, 306);
		this.btCancel.Name = "btCancel";
		this.btCancel.ReadOnly = false;
		this.btCancel.SettingInVisible = false;
		this.btCancel.SettingReadonly = false;
		this.btCancel.Size = new System.Drawing.Size(75, 25);
		this.btCancel.TabIndex = 2;
		this.btCancel.Text = "Cancel";
		this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
		this.btBack.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btBack.EditControl = null;
		this.btBack.EditValue = "";
		this.btBack.FieldName = null;
		this.btBack.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btBack.ImageOptions.Image");
		this.btBack.IsSearchControl = false;
		this.btBack.Location = new System.Drawing.Point(5, 306);
		this.btBack.Name = "btBack";
		this.btBack.ReadOnly = false;
		this.btBack.SettingInVisible = false;
		this.btBack.SettingReadonly = false;
		this.btBack.Size = new System.Drawing.Size(27, 25);
		this.btBack.TabIndex = 3;
		this.btBack.Click += new System.EventHandler(this.btBack_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(606, 335);
		base.Controls.Add(this.btBack);
		base.Controls.Add(this.btCancel);
		base.Controls.Add(this.btOK);
		base.Controls.Add(this.grdDocumentDraft);
		base.FormId = "oDocumentDraft";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_ListDocumentDraft";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Document Draft";
		base.Load += new System.EventHandler(this.frm_ListDocumentDraft_Load);
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grdDocumentDraft).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gvDocumentDraft).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rIBt_DocNum).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rIBt_CardCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit1.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit1).EndInit();
		base.ResumeLayout(false);
	}
}
