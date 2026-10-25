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
using Apzon.Controls.Editor.MyLookUpEdit;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Forms.UserControls;
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

namespace Apzon.Forms.Inventory.InventoryTransaction;

public class frm_InventoryCounting : XtraFormEx
{
	private const string OkString = "OK";

	private const string UpdateString = "Update";

	private const string AddString = "Add";

	private DataTable _dtDoc;

	private DataTable _dtLine;

	private DataTable _dtBatch;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private bool _isLoading;

	private DataTable dtApplicantInterview;

	private readonly RepositoryItemLookUpEdit _typeP = new RepositoryItemLookUpEdit();

	private readonly RepositoryItemLookUpEdit _typeC = new RepositoryItemLookUpEdit();

	private RepositoryItemButtonEditCFL user_btn = new RepositoryItemButtonEditCFL();

	private RepositoryItemButtonEditCFL emp_btn = new RepositoryItemButtonEditCFL();

	private readonly Dictionary<string, string> _lstValidA = new Dictionary<string, string>
	{
		{ "A", "Approved" },
		{ "D", "Draft" }
	};

	private readonly Dictionary<string, string> _lstValidC = new Dictionary<string, string>
	{
		{ "A", "Approved" },
		{ "D", "Draft" },
		{ "L", "Closed" },
		{ "C", "Canceled" }
	};

	private readonly Dictionary<string, Common.Table_Code> _isBin = new Dictionary<string, Common.Table_Code>();

	private const string KeyDefaultBin = "APZBINDEF";

	private IContainer components;

	public SimpleButtonEx btn_Ok;

	public LabelLine lb_DocStatus;

	public LabelLine lb_Whs;

	public LabelLine lb_DocNo;

	public LabelLine lb_ToDate;

	public LabelLine lb_FromDate;

	public SimpleButtonEx btnCancel;

	public SimpleButtonEx btn_Search;

	public TextEditEx txt_No;

	public DateEditEx txtToDate;

	public DateEditEx txtFromDate;

	protected SplitContainerControl split_Main;

	private GridControlEx grd_Doc;

	private GridViewEx gv_Doc;

	private GridColumnEx gridColumnEx1;

	private GridColumnEx gridColumnEx2;

	private GridColumnEx gridColumnEx3;

	private GridColumnEx gridColumnEx4;

	private RepositoryItemButtonEditCFL btn_WhsDoc;

	private GridColumnEx gridColumnEx5;

	private RepositoryItemLookUpEditSimple luk_StatusDoc;

	private GridColumnEx gridColumnEx6;

	private SplitContainerControl split_LineBatchAndSerial;

	private XtraTabControlEx xtraTabControl1;

	private XtraTabPageEx xtra_Item;

	private GridControlEx grd_line;

	private GridViewEx gv_line;

	private GridColumnEx gridColumnEx7;

	private GridColumnEx gridColumnEx8;

	private GridColumnEx gridColumnEx9;

	private RepositoryItemButtonEditCFL btn_ItemCode;

	private GridColumnEx gridColumnEx10;

	private GridColumnEx gridColumnEx11;

	private RepositoryItemButtonEditCFL btn_WhsLine;

	private GridColumnEx gridColumnEx12;

	private GridColumnEx gridColumnEx13;

	private XtraTabControlEx xtraTabControl2;

	private XtraTabPageEx xtra_Batch;

	private GridControlEx grd_batch;

	private GridViewEx gv_batch;

	private GridColumnEx gridColumnEx14;

	private GridColumnEx gridColumnEx15;

	private GridColumnEx gridColumnEx16;

	private GridColumnEx gridColumnEx17;

	private GridColumnEx gridColumnEx18;

	private GridColumnEx gb_DistNumber;

	private GridColumnEx gb_itemCode;

	private GridColumnEx gb_BinAbs;

	private GridColumnEx gb_BinCode;

	private GridColumnEx gb_Qty;

	private ButtonEditCFL btn_Whs;

	private RepositoryItemButtonEditCFL btn_DisNumber;

	private LookUpEditSimple luk_Status;

	private RepositoryItemButtonEditCFL repositoryItemButtonEditCFL1;

	private RepositoryItemButtonEditCFL btn_BinCode;

	private GridColumnEx gridColumnEx24;

	private RepositoryItemButtonEditCFL btn_RequestNo;

	private GridColumnEx gridColumnEx19;

	private ContextMenuStrip ctm_Doc;

	private ToolStripMenuItem addToolStripMenuItem;

	private ToolStripMenuItem duplicateToolStripMenuItem;

	private ToolStripMenuItem closeToolStripMenuItem;

	private ToolStripMenuItem cancelToolStripMenuItem;

	private ContextMenuStrip ctm_Line;

	private ToolStripMenuItem addRowToolStripMenuItem;

	private ToolStripMenuItem deleteRowToolStripMenuItem;

	private ToolStripMenuItem duplicateRowToolStripMenuItem;

	private ContextMenuStrip ctm_Batch;

	private ToolStripMenuItem addRowToolStripMenuItem1;

	private ToolStripMenuItem deleteRowToolStripMenuItem1;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx1;

	private GridColumnEx gridColumnEx20;

	private GridColumnEx gridColumnEx21;

	private GridColumnEx gridColumnEx22;

	private GridColumnEx gridColumnEx23;

	private GridColumnEx gridColumnEx25;

	private RepositoryItemButtonEditCFL btn_UomCode;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx2;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx3;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx4;

	private GridColumnEx gridColumnEx26;

	private GridColumnEx gridColumnEx27;

	private RepositoryItemButtonEditCFL btn_BinItem;

	private GridColumnEx gc_Taker1Type;

	private GridColumnEx gc_Taker1Id;

	private RepositoryItemLookUpEditSimple luk_Taker1Type;

	private GridColumnEx gc_CountType;

	private RepositoryItemLookUpEditSimple luk_CountType;

	public int Id { get; set; }

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		try
		{
			if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
			{
				WinFuntions.DisableAllControl(Controls);
				btnCancel.Enabled = true;
				return;
			}
			_formMode = value;
			switch (value)
			{
			case Enums.FormViewModeType.Add:
				btn_Ok.Text = "Add";
				addRowToolStripMenuItem.Enabled = true;
				deleteRowToolStripMenuItem.Enabled = true;
				duplicateRowToolStripMenuItem.Enabled = true;
				addRowToolStripMenuItem1.Enabled = true;
				deleteRowToolStripMenuItem1.Enabled = true;
				break;
			case Enums.FormViewModeType.Update:
				btn_Ok.Text = "Update";
				deleteRowToolStripMenuItem.Enabled = true;
				duplicateRowToolStripMenuItem.Enabled = false;
				deleteRowToolStripMenuItem1.Enabled = false;
				break;
			case Enums.FormViewModeType.View:
				btn_Ok.Text = "OK";
				deleteRowToolStripMenuItem.Enabled = true;
				duplicateRowToolStripMenuItem.Enabled = false;
				deleteRowToolStripMenuItem1.Enabled = false;
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public frm_InventoryCounting()
	{
		InitializeComponent();
		OnLoadAsync += frm_InventoryCounting_Load;
		RepositoryItemButtonEditCFL repositoryItemButtonEditCFL = btn_WhsDoc;
		repositoryItemButtonEditCFL.AfterShowCFL = (Action<frm_CFL>)Delegate.Combine(repositoryItemButtonEditCFL.AfterShowCFL, new Action<frm_CFL>(AfterShow));
		RepositoryItemButtonEditCFL repositoryItemButtonEditCFL2 = btn_WhsLine;
		repositoryItemButtonEditCFL2.AfterShowCFL = (Action<frm_CFL>)Delegate.Combine(repositoryItemButtonEditCFL2.AfterShowCFL, new Action<frm_CFL>(AfterShow));
	}

	private void AfterShow(frm_CFL frm)
	{
		try
		{
			DataTable dataTable = frm.gv_CFL.GridControl.DataSource as DataTable;
			if (dataTable.Rows.Count > 0 && dataTable != null)
			{
				DataRow[] source = (from s in dataTable.AsEnumerable()
					where Function.ToString(s["DropShips"]) == "N"
					select s).ToArray();
				if (source.Count() > 0)
				{
					frm.gv_CFL.GridControl.DataSource = source.CopyToDataTable();
					frm.gv_CFL.ExpandAllGroups();
				}
				else
				{
					dataTable.Clear();
					dataTable.AcceptChanges();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Ok_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			switch (FormMode)
			{
			case Enums.FormViewModeType.Add:
				await Save(1);
				break;
			case Enums.FormViewModeType.Update:
				await Save(2);
				break;
			case Enums.FormViewModeType.View:
				Close();
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task frm_InventoryCounting_Load(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			_isLoading = true;
			FormMode = Enums.FormViewModeType.View;
			LoadInItForm();
			luk_StatusDoc.SetDataSource(_lstValidC);
			luk_Status.SetDataSource(_lstValidC);
			btn_Whs.Properties.ValueMember = "WhsCode";
			btn_Whs.Properties.DisplayMember = "WhsCode";
			btn_Whs.Properties.TypeCFL = Enums.TypeCFL.GetListWareHouse;
			RepositoryItemButtonEditCFL properties = btn_Whs.Properties;
			properties.DataSourceLoaded = (Action<DataTable>)Delegate.Combine(properties.DataSourceLoaded, (Action<DataTable>)((DataTable dtData) =>
			{
				btn_WhsDoc.DataSource = dtData.Copy();
				btn_WhsLine.DataSource = dtData.Copy();
			}));
			btn_Whs.Properties.RefreshDataSource();
			btn_Whs.Properties.TypeDrillDown = ApzObjectTypes.oWarehouses;
			btn_WhsDoc.ValueMember = "WhsCode";
			btn_WhsDoc.DisplayMember = "WhsCode";
			btn_WhsDoc.TypeCFL = Enums.TypeCFL.GetListWareHouse;
			btn_WhsDoc.TypeDrillDown = ApzObjectTypes.oWarehouses;
			btn_WhsDoc.RefreshDataSource();
			btn_WhsLine.ValueMember = "WhsCode";
			btn_WhsLine.DisplayMember = "WhsCode";
			btn_WhsLine.TypeCFL = Enums.TypeCFL.GetListWareHouse;
			btn_WhsLine.TypeDrillDown = ApzObjectTypes.oWarehouses;
			btn_WhsLine.RefreshDataSource();
			btn_DisNumber.ValueMember = "DistNumber";
			btn_DisNumber.DisplayMember = "DistNumber";
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL = btn_DisNumber;
			repositoryItemButtonEditCFL.BeforeShowCFL = (Action<frm_CFL>)Delegate.Combine(repositoryItemButtonEditCFL.BeforeShowCFL, (Action<frm_CFL>)((frm_CFL frm) =>
			{
				frm.ItemCode = Function.ToString(gv_line.GetFocusedRowCellValue("ItemCode"));
				frm.WhsCode = Function.ToString(gv_line.GetFocusedRowCellValue("WhsCode"));
			}));
			btn_DisNumber.FindCFLBy(Enums.TypeCFL.InventoryCountingBatchSerial, true, "", Enums.ItemObjectType.NoItemType, null);
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL2 = btn_DisNumber;
			repositoryItemButtonEditCFL2.HandleChooseItem = (Action<DataRow>)Delegate.Combine(repositoryItemButtonEditCFL2.HandleChooseItem, new Action<DataRow>(DistNumber_HandleChooseItem));
			btn_DisNumber.HandleChooseItemMulti = DistNumber_ChooseMulti;
			btn_DisNumber.RefreshDataSource();
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL3 = btn_DisNumber;
			repositoryItemButtonEditCFL3.DrillDownMethod = (Action<string>)Delegate.Combine(repositoryItemButtonEditCFL3.DrillDownMethod, (Action<string>)((string editorValue) =>
			{
				string text = Function.ToString(gv_batch.GetFocusedRowCellValue("IsBatch"));
				ApzObjectTypes type = ApzObjectTypes.oNone;
				if (!(text == "Y"))
				{
					if (text == "N")
					{
						type = ApzObjectTypes.oSerialNumberDetails;
					}
				}
				else
				{
					type = ApzObjectTypes.oBatchDetails;
				}
				this.DrillDown(type, editorValue);
			}));
			btn_ItemCode.ValueMember = "ItemCode";
			btn_ItemCode.FieldNameInDataRow("ItemCode", "ItemName");
			btn_ItemCode.ListMandatoryField = "ItemName,ManBatchSerial,UgpCode,OnWhs";
			btn_ItemCode.FieldNameInGrid("ItemCode", "Dscription");
			btn_ItemCode.FindCFLBy(Enums.TypeCFL.ListItems, true, "", Enums.ItemObjectType.NoItemType, null);
			btn_ItemCode.TypeDrillDown = ApzObjectTypes.oItems;
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL4 = btn_ItemCode;
			repositoryItemButtonEditCFL4.HandleChooseItem = (Action<DataRow>)Delegate.Combine(repositoryItemButtonEditCFL4.HandleChooseItem, new Action<DataRow>(ItemCode_HandleChooseItem));
			btn_ItemCode.HandleChooseItemMulti = ChooseItemReceiptMulti;
			btn_ItemCode.RefreshDataSource();
			btn_BinCode.ValueMember = "BinCode";
			btn_BinCode.FieldNameInDataRow("AbsEntry", "BinCode");
			btn_BinCode.FieldNameInGrid("BinAbs", "BinCode");
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL5 = btn_BinCode;
			repositoryItemButtonEditCFL5.BeforeShowCFL = (Action<frm_CFL>)Delegate.Combine(repositoryItemButtonEditCFL5.BeforeShowCFL, (Action<frm_CFL>)((frm_CFL frm) =>
			{
				frm.WhsCode = Function.ToString(gv_line.GetFocusedRowCellValue("WhsCode"));
			}));
			btn_BinCode.TypeCFL = Enums.TypeCFL.GetDataBin;
			btn_BinCode.RefreshDataSource();
			btn_BinItem.ValueMember = "BinCode";
			btn_BinItem.FieldNameInDataRow("AbsEntry", "BinCode");
			btn_BinItem.FieldNameInGrid("BinAbs", "BinCode");
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL6 = btn_BinItem;
			repositoryItemButtonEditCFL6.BeforeShowCFL = (Action<frm_CFL>)Delegate.Combine(repositoryItemButtonEditCFL6.BeforeShowCFL, (Action<frm_CFL>)((frm_CFL frm) =>
			{
				frm.WhsCode = Function.ToString(gv_line.GetFocusedRowCellValue("WhsCode"));
			}));
			btn_BinItem.TypeCFL = Enums.TypeCFL.GetDataBin;
			btn_BinItem.RefreshDataSource();
			btn_RequestNo.ValueMember = "DocEntry";
			btn_RequestNo.TypeCFL = Enums.TypeCFL.InventoryCountingRequest;
			btn_RequestNo.TypeDrillDown = ApzObjectTypes.oInventoryCountingRequest;
			btn_RequestNo.RefreshDataSource();
			btn_UomCode.ValueMember = "UomCode";
			btn_UomCode.DisplayMember = "UomName";
			btn_UomCode.TypeCFL = Enums.TypeCFL.ListUoMByUoMGroupItemMaster;
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL7 = btn_UomCode;
			repositoryItemButtonEditCFL7.BeforeShowCFL = (Action<frm_CFL>)Delegate.Combine(repositoryItemButtonEditCFL7.BeforeShowCFL, (Action<frm_CFL>)((frm_CFL fr) =>
			{
				fr.DataInt1 = Function.ParseInt(gv_line.GetFocusedRowCellValue("UgpEntry"));
				fr.DataString2 = "I";
			}));
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL8 = btn_UomCode;
			repositoryItemButtonEditCFL8.HandleChooseItem = (Action<DataRow>)Delegate.Combine(repositoryItemButtonEditCFL8.HandleChooseItem, new Action<DataRow>(Uom_HandleChooseItem));
			btn_UomCode.RefreshDataSource();
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Code", typeof(int));
			dataTable.Columns.Add("Name", typeof(string));
			dataTable.Rows.Add(12, "User");
			dataTable.Rows.Add(171, "Employee");
			luk_Taker1Type.DataSource = dataTable;
			DataTable dataTable2 = new DataTable();
			dataTable2.Columns.Add("Code", typeof(string));
			dataTable2.Columns.Add("Name", typeof(string));
			dataTable2.Rows.Add("1", "Single Counter");
			dataTable2.Rows.Add("2", "Multiple Counter");
			luk_CountType.DataSource = dataTable2;
			user_btn.ValueMember = "USERID";
			user_btn.DisplayMember = "EmployeeName";
			user_btn.TypeCFL = Enums.TypeCFL.GetUserList;
			user_btn.MultiSelectMode = true;
			user_btn.RefreshDataSource();
			emp_btn.ValueMember = "empId";
			emp_btn.DisplayMember = "fullName";
			emp_btn.TypeCFL = Enums.TypeCFL.Employees;
			emp_btn.MultiSelectMode = true;
			emp_btn.RefreshDataSource();
			await Task.WhenAll(WinFuntions.LoadUdfColumns(gv_Doc, "APZ_OINC"), WinFuntions.LoadUdfColumns(gv_line, "APZ_INC1"), WinFuntions.LoadUdfColumns(gv_batch, "APZ_INC2"));
			txtFromDate.EditValue = DateTime.Today.AddDays(-Configuration.SearchDate);
			txtToDate.EditValue = DateTime.Today.AddDays(Configuration.SearchDate);
			string startFrom = (Function.ParseDateTime(txtFromDate.EditValue).HasValue ? ((DateTime)txtFromDate.EditValue).ToString("yyyyMMdd") : "");
			string startTo = (Function.ParseDateTime(txtToDate.EditValue).HasValue ? ((DateTime)txtToDate.EditValue).ToString("yyyyMMdd") : "");
			if (Id > 0)
			{
				txt_No.EditValue = Id;
			}
			DataTable dataTable3 = await LoadData(Function.ParseInt(txt_No.EditValue), "", startFrom, startTo, "");
			Id = 0;
			grd_Doc.DataSource = dataTable3;
			grd_Doc.DatasourceTable().AcceptChanges();
			if (!dataTable3.IsNotNull())
			{
				gv_Doc_FocusedRowChanged(null, null);
			}
			if (dataTable3.Rows.Count == 0)
			{
				split_LineBatchAndSerial.PanelVisibility = SplitPanelVisibility.Panel1;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		_isLoading = false;
	}

	private async void btn_Search_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			string whs = Function.ToString(btn_Whs.EditValue);
			string status = Function.ToString(luk_Status.EditValue);
			string startFrom = (Function.ParseDateTime(txtFromDate.EditValue).HasValue ? ((DateTime)txtFromDate.EditValue).ToString("yyyyMMdd") : "");
			string startTo = (Function.ParseDateTime(txtToDate.EditValue).HasValue ? ((DateTime)txtToDate.EditValue).ToString("yyyyMMdd") : "");
			if (Id > 0)
			{
				txt_No.EditValue = Id;
			}
			DataTable dataSource = await LoadData(Function.ParseInt(txt_No), whs, startFrom, startTo, status);
			Id = 0;
			if (!_isLoading && ((FormMode != Enums.FormViewModeType.Add && FormMode != Enums.FormViewModeType.Update) || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes))
			{
				FormMode = Enums.FormViewModeType.View;
				grd_line.DataSourceTable().Rows.Clear();
				grd_batch.DataSourceTable().Rows.Clear();
				grd_Doc.DataSource = dataSource;
				grd_Doc.DatasourceTable().AcceptChanges();
				int id = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("DocEntry"));
				await LoadLineById(id);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private async void gv_Doc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			_dtDoc = grd_Doc.DataSourceTable;
			_dtLine = grd_line.DataSourceTable;
			_dtBatch = grd_batch.DataSourceTable;
			int docEntry = Function.ParseInt(gv_Doc.GetRowCellValue(e.RowHandle, "DocEntry"));
			if (e.Column.FieldName == "WhsCode")
			{
				if (_dtLine != null && _dtLine.Rows.Count > 0 && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdateDataToExistsLine, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, Function.ToString(e.Value)) == DialogResult.Yes)
				{
					for (int i = 0; i < _dtLine.Rows.Count; i++)
					{
						int lineNum = Function.ParseInt(_dtLine.Rows[i]["LineNum"]);
						_dtLine.Rows[i]["WhsCode"] = Function.ToString(e.Value);
						DataTable dataTable = await GetBatchByLineNum(lineNum, docEntry);
						if (dataTable != null && dataTable.Rows.Count > 0)
						{
							for (int j = 0; j < dataTable.Rows.Count; j++)
							{
								dataTable.Rows[j]["BinAbs"] = 0;
								dataTable.Rows[j]["BinCode"] = "";
							}
						}
					}
				}
			}
			else if (e.Column.FieldName == "Taker1Type")
			{
				if (e.Value != null)
				{
					if (Function.ParseInt(e.Value) == 12)
					{
						gc_Taker1Id.ColumnEdit = user_btn;
					}
					else
					{
						gc_Taker1Id.ColumnEdit = emp_btn;
					}
					gv_Doc.SetRowCellValue(e.RowHandle, "CountType", "1");
					gv_Doc.SetRowCellValue(e.RowHandle, "Taker1Id", "");
				}
			}
			else if (e.Column.FieldName == "CountType" && e.Value != null)
			{
				if (Function.ParseInt(gv_Doc.GetRowCellValue(e.RowHandle, "Taker1Type")) == 12)
				{
					if (Function.ToString(e.Value).Equals("1"))
					{
						user_btn.MultiSelectMode = false;
					}
					else
					{
						user_btn.MultiSelectMode = true;
					}
				}
				else if (Function.ToString(e.Value).Equals("1"))
				{
					emp_btn.MultiSelectMode = false;
				}
				else
				{
					emp_btn.MultiSelectMode = true;
				}
				gv_Doc.SetRowCellValue(e.RowHandle, "Taker1Id", "");
			}
			DataChanged();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_BeforeLeaveRow(object sender, RowAllowEventArgs e)
	{
		try
		{
			if (!_isLoading && FormMode == Enums.FormViewModeType.Update)
			{
				DialogResult dialogResult = XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
				if (dialogResult == DialogResult.Yes && dialogResult == DialogResult.Yes)
				{
					grd_Doc.DataSourceTable.RejectChanges();
					FormMode = Enums.FormViewModeType.View;
				}
				if (dialogResult == DialogResult.No)
				{
					e.Allow = false;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void gv_Doc_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Add)
			{
				return;
			}
			int id = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("DocEntry"));
			await LoadLineById(id);
			string text = Function.ToString(gv_Doc.GetFocusedRowCellValue("DocStatus"));
			if (!(text == "D"))
			{
				if (text == "A")
				{
					addRowToolStripMenuItem.Enabled = false;
					addRowToolStripMenuItem1.Enabled = false;
				}
			}
			else
			{
				addRowToolStripMenuItem.Enabled = true;
				addRowToolStripMenuItem1.Enabled = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void gv_line_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		_ = 2;
		try
		{
			_dtLine = grd_line.DataSourceTable;
			_dtBatch = grd_batch.DataSourceTable;
			int lineNum = Function.ParseInt(gv_line.GetRowCellValue(e.RowHandle, "LineNum"));
			int docEntry = Function.ParseInt(gv_line.GetRowCellValue(e.RowHandle, "DocEntry"));
			if (e.Column.FieldName == "ItemCode")
			{
				grd_batch.DataSourceTable.Rows.Clear();
				grd_batch.DataSourceTable.AcceptChanges();
				gv_line.SetRowCellValue(e.RowHandle, "Qty", 0);
				_dtLine.AcceptChanges();
				string itemCode = Function.ToString(e.Value);
				DataRow itemInfor = (from t in btn_ItemCode.DataSource.AsEnumerable()
					where Function.ToString(t["ItemCode"]) == itemCode
					select t).FirstOrDefault();
				string whsCode = Function.ToString(gv_line.GetRowCellValue(e.RowHandle, "WhsCode"));
				if (_dtLine.Select($"ItemCode='{itemCode}' and WhsCode='{whsCode}'").ToArray().Count() > 1)
				{
					WinCommons.Warning(Enums.MessageCode.ItemInWhsExistsInGrid, "", itemCode, whsCode);
				}
				if (string.Equals(await CheckBatchSerNum(itemCode), "Y"))
				{
					split_LineBatchAndSerial.PanelVisibility = SplitPanelVisibility.Both;
					if ((from t in _dtBatch.AsEnumerable()
						where Function.ToString(t["DistNumber"]) == ""
						select t).ToList().Count() == 0)
					{
						grd_batch.DataSourceTable().Rows.Clear();
						int num = Function.ParseInt(_dtBatch.Compute("MAX(LineId)", ""));
						DataRow dataRow = _dtBatch.NewRow();
						dataRow["DocEntry"] = docEntry;
						dataRow["BaseLine"] = lineNum;
						dataRow["LineId"] = num + 1;
						_dtBatch.Rows.Add(dataRow);
						grd_batch.DataSource = _dtBatch;
					}
				}
				else
				{
					split_LineBatchAndSerial.PanelVisibility = SplitPanelVisibility.Panel1;
				}
				if (itemInfor != null)
				{
					gv_line.SetRowCellValue(e.RowHandle, "Dscription", Function.ToString(itemInfor["ItemName"]));
					gv_line.SetRowCellValue(e.RowHandle, "TranType", Function.ToString(itemInfor["ManBatchSerial"]));
					gv_line.SetRowCellValue(e.RowHandle, "UgpEntry", Function.ParseInt(itemInfor["UgpCode"]));
					gv_line.SetRowCellValue(e.RowHandle, "UomEntry", 0);
					gv_line.SetRowCellValue(e.RowHandle, "UomCode", "");
					gv_line.SetRowCellValue(e.RowHandle, "NumPerMsr", 0);
					if (!string.IsNullOrWhiteSpace(whsCode))
					{
						DataTable dataTable = await _httpClient.InventoryCounting.GetOnHandByItem(itemCode, whsCode);
						gv_line.SetRowCellValue(e.RowHandle, "InvQty", Function.ParseDouble(dataTable.Rows[0][0]));
					}
					else
					{
						gv_line.SetRowCellValue(e.RowHandle, "InvQty", Function.ParseDouble(itemInfor["OnWhs"]));
					}
					if (Function.ParseInt(itemInfor["UgpCode"]) == -1)
					{
						gv_line.SetRowCellValue(e.RowHandle, "UomEntry", -1);
						gv_line.SetRowCellValue(e.RowHandle, "UomCode", "Manual");
						gv_line.SetRowCellValue(e.RowHandle, "NumPerMsr", 1);
					}
					if (lineNum == 0 || string.IsNullOrEmpty(Function.ToString(lineNum)))
					{
						gv_line.SetRowCellValue(e.RowHandle, "LineNum", Function.ParseInt(_dtLine.Compute("MAX(LineNum)", "")) + 1);
					}
				}
			}
			if (e.Column.FieldName == "WhsCode")
			{
				_dtLine.AcceptChanges();
				string text = Function.ToString(e.Value);
				string text2 = Function.ToString(gv_line.GetRowCellValue(e.RowHandle, "ItemCode"));
				if (_dtLine.Select($"ItemCode='{text2}' and WhsCode='{text}'").ToArray().Count() > 1)
				{
					WinCommons.Warning(Enums.MessageCode.ItemInWhsExistsInGrid, "", text2, text);
				}
				gv_line.SetRowCellValue(e.RowHandle, "BinAbs", 0);
				gv_line.SetRowCellValue(e.RowHandle, "BinCode", "");
				if (!string.IsNullOrWhiteSpace(text2))
				{
					DataTable dataTable2 = await _httpClient.InventoryCounting.GetOnHandByItem(text2, text);
					gv_line.SetRowCellValue(e.RowHandle, "InvQty", Function.ParseDouble(dataTable2.Rows[0][0]));
				}
				else
				{
					gv_line.SetRowCellValue(e.RowHandle, "InvQty", 0);
				}
				if (_dtBatch != null && _dtBatch.Rows.Count > 0)
				{
					for (int num2 = 0; num2 < _dtBatch.Rows.Count; num2++)
					{
						_dtBatch.Rows[num2]["BinAbs"] = 0;
						_dtBatch.Rows[num2]["BinCode"] = "";
					}
				}
			}
			if (e.Column.FieldName == "BinCode")
			{
				_dtLine.AcceptChanges();
				string text3 = Function.ToString(e.Value);
				string text4 = Function.ToString(gv_line.GetRowCellValue(e.RowHandle, "ItemCode"));
				string text5 = Function.ToString(gv_line.GetRowCellValue(e.RowHandle, "WhsCode"));
				if (_dtLine.Select($"ItemCode='{text4}' and WhsCode='{text5}' and BinCode='{text3}'").ToArray().Count() > 1)
				{
					WinCommons.Warning(Enums.MessageCode.ItemBinCodeWhsExistsInGrid, "", text4, text3, text5);
				}
			}
			if (e.Column.FieldName == "Qty")
			{
				double num3 = Function.ParseDouble(e.Value);
				double num4 = Function.ParseDouble(gv_line.GetFocusedRowCellValue("InvQty"));
				gv_line.SetFocusedRowCellValue("Variance", num3 - num4);
				if (num4 == 0.0)
				{
					gv_line.SetFocusedRowCellValue("Variance%", 100);
				}
				else
				{
					gv_line.SetFocusedRowCellValue("Variance%", (num3 - num4) / num4 * 100.0);
				}
			}
			if (e.Column.FieldName == "NumPerMsr")
			{
				Function.ParseDouble(e.Value);
				Function.ParseDouble(gv_line.GetRowCellValue(e.RowHandle, "Qty"));
			}
			if ((from t in _dtLine.AsEnumerable()
				where Function.ToString(t["ItemCode"]) == ""
				select t).ToList().Count() == 0)
			{
				NewItem();
			}
			DataChanged();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_batch_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			DataTable dataSourceTable = grd_batch.DataSourceTable;
			_dtLine = grd_line.DataSourceTable;
			if (e.Column.FieldName == "DistNumber" && Function.ToString(gv_line.GetRowCellValue(e.RowHandle, "TranType")).Equals("S"))
			{
				gv_batch.SetRowCellValue(e.RowHandle, "Qty", 1);
			}
			if (e.Column.FieldName == "Qty")
			{
				double num = dataSourceTable.AsEnumerable().Sum((DataRow t) => Function.ParseDouble(t["Qty"]));
				gv_line.SetFocusedRowCellValue("Qty", num);
			}
			if (FormMode == Enums.FormViewModeType.Add && (from t in dataSourceTable.AsEnumerable()
				where Function.ToString(t["DistNumber"]) == ""
				select t).ToList().Count() == 0)
			{
				NewBatch();
			}
			DataChanged();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_line_BeforeLeaveRow(object sender, RowAllowEventArgs e)
	{
		try
		{
			BatchAndSerial();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_CustomRowCellEditForEditing(object sender, CustomRowCellEditEventArgs e)
	{
		try
		{
			if (e.Column.FieldName != "DocStatus")
			{
				return;
			}
			string text = Function.ToString(gv_Doc.GetRowCellValue(e.RowHandle, "DocStatus"));
			if (!string.IsNullOrWhiteSpace(text))
			{
				switch (text)
				{
				case "A":
					e.RepositoryItem = _typeP;
					break;
				case "D":
					e.RepositoryItem = _typeP;
					break;
				case "C":
					e.RepositoryItem = _typeC;
					break;
				case "L":
					e.RepositoryItem = _typeC;
					break;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
	{
		try
		{
			if (e.Column.FieldName != "Taker1Id")
			{
				return;
			}
			if (Function.ParseInt(gv_Doc.GetRowCellValue(e.RowHandle, "Taker1Type")) == 12)
			{
				if (Function.ToString(gv_Doc.GetRowCellValue(e.RowHandle, "CountType")).Equals("1"))
				{
					user_btn.MultiSelectMode = false;
				}
				else
				{
					user_btn.MultiSelectMode = true;
				}
				e.RepositoryItem = user_btn;
			}
			else
			{
				if (Function.ParseInt(gv_Doc.GetRowCellValue(e.RowHandle, "CountType")) == 1)
				{
					emp_btn.MultiSelectMode = false;
				}
				else
				{
					emp_btn.MultiSelectMode = true;
				}
				e.RepositoryItem = emp_btn;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void gv_line_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		_ = 2;
		try
		{
			if (gv_line.IsInsertNewRow)
			{
				return;
			}
			int lineNum = Function.ParseInt(gv_line.GetRowCellValue(e.RowHandle, "LineNum"));
			int docEntry = Function.ParseInt(gv_line.GetRowCellValue(e.RowHandle, "DocEntry"));
			string itemCode = Function.ToString(gv_line.GetRowCellValue(e.RowHandle, "ItemCode"));
			Function.ToString(gv_line.GetRowCellValue(e.RowHandle, "WhsCode"));
			if (string.Equals(await CheckBatchSerNum(itemCode), "Y"))
			{
				split_LineBatchAndSerial.PanelVisibility = SplitPanelVisibility.Both;
				if (lineNum > 0)
				{
					DataTable dataTable = await GetBatchByLineNum(lineNum, docEntry);
					if (dataTable.Rows.Count > 0)
					{
						grd_batch.DataSource = dataTable;
						return;
					}
					_dtBatch = grd_batch.DataSourceTable;
					DataRow row = _dtBatch.NewRow();
					DataRow dataRow = row;
					dataRow["DocEntry"] = await GetMaxDocNum();
					row["BaseLine"] = lineNum;
					_dtBatch.Rows.Add(row);
					grd_batch.DataSource = _dtBatch;
				}
			}
			else
			{
				split_LineBatchAndSerial.PanelVisibility = SplitPanelVisibility.Panel1;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_batch_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Add)
			{
				gv_batch.FocusedColumn.OptionsColumn.ReadOnly = true;
			}
			if (!Function.ToString(gv_line.GetFocusedRowCellValue("TranType")).Equals("B") && gv_batch.FocusedColumn.FieldName.Equals("Qty"))
			{
				e.Cancel = true;
			}
			if (Function.ToString(gv_line.GetFocusedRowCellValue("TranType")).Equals("B") && !gv_batch.FocusedColumn.FieldName.Equals("DistNumber") && string.IsNullOrEmpty(Function.ToString(gv_batch.GetFocusedRowCellValue("DistNumber"))))
			{
				WinCommons.Warning(Enums.MessageCode.YouMustEnterBatchNo, "");
				gv_batch.Focus();
				gv_batch.FocusedColumn = gb_DistNumber;
				gv_batch.ShowEditor();
				e.Cancel = true;
			}
			else if (Function.ToString(gv_line.GetFocusedRowCellValue("TranType")).Equals("S") && !gv_batch.FocusedColumn.FieldName.Equals("DistNumber") && string.IsNullOrEmpty(Function.ToString(gv_batch.GetFocusedRowCellValue("DistNumber"))))
			{
				WinCommons.Warning(Enums.MessageCode.YouMustEnterSerialNo, "");
				gv_batch.Focus();
				gv_batch.FocusedColumn = gb_DistNumber;
				gv_batch.ShowEditor();
				e.Cancel = true;
			}
			else if (gv_batch.FocusedColumn.FieldName.Equals("BinCode"))
			{
				string text = Function.ToString(gv_line.GetFocusedRowCellValue("WhsCode"));
				if (!_isBin.Keys.Contains(text))
				{
					_isBin.Add(text, _httpClient.Warehouse.WhsCheckFollowBin(text));
				}
				if (_isBin[text].Code.Equals("N"))
				{
					WinCommons.Warning(Enums.MessageCode.WarehouseNotSetupFollowBin, "", text);
					e.Cancel = true;
				}
			}
			else
			{
				string a = Function.ToString(gv_Doc.GetFocusedRowCellValue("DocStatus"));
				if ((gv_batch.FocusedColumn.FieldName == "DistNumber" || gv_batch.FocusedColumn.FieldName == "BinCode" || gv_batch.FocusedColumn.FieldName == "Qty") && (string.Equals(a, "C") || string.Equals(a, "L")))
				{
					gv_batch.FocusedColumn.OptionsColumn.ReadOnly = true;
					return;
				}
				gv_batch.Columns["DistNumber"].OptionsColumn.ReadOnly = false;
				gv_batch.Columns["BinCode"].OptionsColumn.ReadOnly = false;
				gv_batch.Columns["Qty"].OptionsColumn.ReadOnly = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void addToolStripMenuItem_Click(object sender, EventArgs e)
	{
		REFRESH();
	}

	private async void duplicateToolStripMenuItem_Click(object sender, EventArgs e)
	{
		WinCommons._OpenProcessing();
		try
		{
			int focusedRowHandle = gv_Doc.FocusedRowHandle;
			if (gv_Doc.IsRowSelected(focusedRowHandle))
			{
				int focusedEntry = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("DocEntry"));
				gv_Doc.GetFocusedDataRow();
				DataTable dataSourceTable = grd_Doc.DataSourceTable;
				DataRow[] dr = (from t in dataSourceTable.AsEnumerable()
					where Function.ParseInt(t["DocEntry"]) == focusedEntry
					select t).ToArray();
				int maxDocEntry = Function.ParseInt(await GetMaxDocNum()) + 1;
				dr[0]["DocEntry"] = maxDocEntry;
				if (dr != null && dr.Length != 0)
				{
					DataTable dataSource = dr.CopyToDataTable();
					grd_Doc.DataSource = null;
					grd_Doc.DataSource = dataSource;
				}
				grd_Doc.DataSourceTable.AcceptChanges();
				DataSet dataSet = await _httpClient.InventoryCounting.GetDataById(focusedEntry);
				if (dataSet != null && dataSet.Tables.Count > 0)
				{
					DataTable dataTable = dataSet.Tables[0];
					foreach (DataRow row in dataTable.Rows)
					{
						row["DocEntry"] = maxDocEntry;
					}
					grd_line.DataSource = dataTable;
					grd_line.RefreshDataSource();
				}
			}
			else
			{
				WinCommons.writeStatus(Enums.MessageCode.ChooseRow, Enums.MessageType.Error, Msg_Time._TimeMedium, "");
			}
			FormMode = Enums.FormViewModeType.Add;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private async void closeToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			int num = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("DocEntry"));
			string text = "L";
			if (num > 0 && !string.IsNullOrEmpty(text) && string.IsNullOrEmpty((await _httpClient.InventoryCounting.UpdateStatus(num, text)).Message))
			{
				btn_Search_Click(null, null);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void cancelToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			int num = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("DocEntry"));
			string text = "C";
			if (num > 0 && !string.IsNullOrEmpty(text) && string.IsNullOrEmpty((await _httpClient.InventoryCounting.UpdateStatus(num, text)).Message))
			{
				btn_Search_Click(null, null);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void addRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		NewItem();
	}

	private void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			_dtBatch = grd_batch.DataSourceTable();
			_ = gv_line.FocusedRowHandle;
			int num = Function.ParseInt(gv_line.GetFocusedRowCellValue("LineNum"));
			int num2 = Function.ParseInt(gv_line.GetFocusedRowCellValue("DocEntry"));
			if (_dtBatch.Rows.Count > 0 && num > 0)
			{
				DataRow[] array = _dtBatch.Select($"DocEntry={num2} and BaseLine = {num}");
				if (array.Count() > 0)
				{
					for (int i = 0; i < array.Count(); i++)
					{
						array[i].Delete();
					}
					_dtBatch.AcceptChanges();
					grd_batch.DataSource = _dtBatch;
					grd_batch.DatasourceTable().AcceptChanges();
				}
			}
			grd_line.BeginUpdate();
			gv_line.DeleteSelectedRows();
			grd_line.EndUpdate();
			grd_line.DatasourceTable().AcceptChanges();
			grd_line.RefreshDataSource();
			gv_line.FocusedRowHandle = 0;
			DataChanged();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
			WinCommons.Error(Enums.MessageCode.None, ex.Message);
		}
	}

	private void duplicateRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			int lineNum = Function.ParseInt(gv_line.GetRowCellValue(gv_line.FocusedRowHandle, "LineNum"));
			DataTable dataSourceTable = grd_line.DataSourceTable;
			DataRow[] array = (from t in dataSourceTable.AsEnumerable()
				where Function.ParseInt(t["LineNum"]) == lineNum
				select t).ToArray();
			Function.ParseInt(dataSourceTable.Compute("MAX(lineNum)", ""));
			if (array != null && array.Length != 0)
			{
				DataRow dataRow = dataSourceTable.NewRow();
				dataRow.ItemArray = array[0].ItemArray;
				dataRow["LineNum"] = lineNum + 1;
				dataSourceTable.Rows.Add(dataRow);
				grd_line.DataSource = dataSourceTable;
				grd_line.DatasourceTable().AcceptChanges();
				gv_line.FocusedRowHandle = gv_line.FindRowHandleByDataRow(dataRow);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void addRowToolStripMenuItem1_Click(object sender, EventArgs e)
	{
		try
		{
			NewBatch();
			DataChanged();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void deleteRowToolStripMenuItem1_Click(object sender, EventArgs e)
	{
		try
		{
			_dtBatch = grd_batch.DataSourceTable;
			grd_batch.BeginUpdate();
			gv_batch.DeleteSelectedRows();
			grd_batch.EndUpdate();
			grd_batch.DatasourceTable().AcceptChanges();
			DataChanged();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			_ = gv_Doc.FocusedRowHandle;
			string a = Function.ToString(gv_Doc.GetRowCellValue(e.RowHandle, "DocStatus"));
			if ((e.Column.FieldName == "DocDate" || e.Column.FieldName == "WhsCode" || e.Column.FieldName == "DocStatus" || e.Column.FieldName == "ReqNo") && (string.Equals(a, "C") || string.Equals(a, "L")))
			{
				e.Appearance.BackColor = GridColor.GridReadOnly;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			_ = gv_Doc.FocusedRowHandle;
			string a = Function.ToString(gv_Doc.GetFocusedRowCellValue("DocStatus"));
			if ((gv_Doc.FocusedColumn.FieldName == "DocDate" || gv_Doc.FocusedColumn.FieldName == "WhsCode" || gv_Doc.FocusedColumn.FieldName == "DocStatus" || gv_Doc.FocusedColumn.FieldName == "ReqNo") && (string.Equals(a, "C") || string.Equals(a, "L")))
			{
				gv_Doc.FocusedColumn.OptionsColumn.ReadOnly = true;
				gv_Doc.Columns["Remark"].OptionsColumn.ReadOnly = false;
			}
			else
			{
				gv_Doc.FocusedColumn.OptionsColumn.ReadOnly = false;
				gv_Doc.Columns["DocEntry"].OptionsColumn.ReadOnly = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_line_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			string a = Function.ToString(gv_Doc.GetFocusedRowCellValue("DocStatus"));
			if (string.Equals(a, "C") || string.Equals(a, "L"))
			{
				e.Appearance.BackColor = GridColor.GridReadOnly;
			}
			int num = Function.ParseInt(gv_line.GetRowCellValue(e.RowHandle, "UgpEntry"));
			if (e.Column.FieldName == "UomCode" && num == -1)
			{
				e.Appearance.BackColor = GridColor.GridReadOnly;
			}
			if (e.Column.FieldName == "InvQty")
			{
				e.Appearance.BackColor = GridColor.GridReadOnly;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_line_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			string a = Function.ToString(gv_Doc.GetFocusedRowCellValue("DocStatus"));
			int num = Function.ParseInt(gv_line.GetFocusedRowCellValue("UgpEntry"));
			if ((gv_line.FocusedColumn.FieldName == "ItemCode" || gv_line.FocusedColumn.FieldName == "Dscription" || gv_line.FocusedColumn.FieldName == "WhsCode" || gv_line.FocusedColumn.FieldName == "Qty" || gv_line.FocusedColumn.FieldName == "Remarks" || gv_line.FocusedColumn.FieldName == "UomCode") && (string.Equals(a, "C") || string.Equals(a, "L")))
			{
				gv_line.FocusedColumn.OptionsColumn.ReadOnly = true;
				gv_line.Columns["Remarks"].OptionsColumn.ReadOnly = false;
			}
			else
			{
				gv_line.FocusedColumn.OptionsColumn.ReadOnly = false;
				gv_line.Columns["DocEntry"].OptionsColumn.ReadOnly = true;
				gv_line.Columns["LineNum"].OptionsColumn.ReadOnly = true;
			}
			if (gv_line.FocusedColumn.FieldName == "UomCode" && num == -1)
			{
				gv_line.Columns["UomCode"].OptionsColumn.ReadOnly = true;
			}
			if (FormMode != Enums.FormViewModeType.Add)
			{
				gv_line.FocusedColumn.OptionsColumn.ReadOnly = true;
			}
			if (gv_line.FocusedColumn.FieldName.Equals("BinCode"))
			{
				string text = Function.ToString(gv_line.GetFocusedRowCellValue("WhsCode"));
				if (!_isBin.Keys.Contains(text))
				{
					_isBin.Add(text, _httpClient.Warehouse.WhsCheckFollowBin(text));
				}
				if (_isBin[text].Code.Equals("N"))
				{
					WinCommons.Warning(Enums.MessageCode.WarehouseNotSetupFollowBin, "", text);
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

	private void gv_batch_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			string a = Function.ToString(gv_Doc.GetFocusedRowCellValue("DocStatus"));
			if (string.Equals(a, "C") || string.Equals(a, "L"))
			{
				e.Appearance.BackColor = GridColor.GridReadOnly;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_batch_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
	{
		try
		{
			if (gv_batch == null)
			{
				return;
			}
			_dtBatch = grd_batch.DatasourceTable();
			if (gv_batch.FocusedColumn.FieldName == "DistNumber" && !string.IsNullOrEmpty(Function.ToString(e.Value)))
			{
				string text = Function.ToString(e.Value);
				Function.ParseInt(gv_batch.GetFocusedRowCellValue("LineNum"));
				Function.ParseInt(gv_batch.GetFocusedRowCellValue("DocEntry"));
				if (_dtBatch != null && _dtBatch.Rows.Count > 0)
				{
					if (_dtBatch.Select($"DistNumber='{text}'").Count() > 1)
					{
						e.Valid = false;
						e.ErrorText = "";
						if (Function.ToString(gv_line.GetFocusedRowCellValue("TranType")).Equals("B"))
						{
							WinCommons.Warning(Enums.MessageCode.TheBatchAlreadyExists, "", text);
						}
						else
						{
							WinCommons.Warning(Enums.MessageCode.TheSerialAlreadyExists, "", text);
						}
						return;
					}
					if (string.IsNullOrEmpty(text))
					{
						e.Valid = false;
						e.ErrorText = "";
						if (Function.ToString(gv_line.GetFocusedRowCellValue("TranType")).Equals("B"))
						{
							WinCommons.Warning(Enums.MessageCode.TheBatchDoesNotBeNull, "");
						}
						else
						{
							WinCommons.Warning(Enums.MessageCode.TheSerialDoesNotBeNull, "");
						}
						return;
					}
				}
			}
			if (gv_batch.FocusedColumn.FieldName == "Qty")
			{
				_dtBatch.AcceptChanges();
				Function.ParseDouble(gv_line.GetFocusedRowCellValue("Qty"));
				Function.ParseDouble(_dtBatch.Compute("SUM(Qty)", ""));
				if (Function.ParseDouble(e.Value) <= 0.0)
				{
					e.Valid = false;
					e.ErrorText = "";
					WinCommons.Warning(Enums.MessageCode.InQtyValueGreater0, "");
					return;
				}
			}
			if (gv_batch.FocusedColumn.FieldName == "BinCode")
			{
				_dtBatch.AcceptChanges();
				if (string.IsNullOrEmpty(Function.ToString(e.Value)))
				{
					e.Valid = false;
					e.ErrorText = "";
					WinCommons.Warning(Enums.MessageCode.BinCodeCannotBeNull, "");
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_batch_InvalidValueException(object sender, InvalidValueExceptionEventArgs e)
	{
		try
		{
			if (sender is ColumnView columnView)
			{
				e.ExceptionMode = ExceptionMode.DisplayError;
				e.WindowCaption = "Input Error";
				columnView.HideEditor();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
	}

	private async void ItemCode_HandleChooseItem(DataRow dr)
	{
	}

	private void ChooseItemReceiptMulti(List<DataRow> rows)
	{
		try
		{
			_dtLine = grd_line.DataSourceTable();
			if (_dtLine == null || _dtLine.Rows.Count <= 0 || rows.Count <= 0)
			{
				return;
			}
			gv_line.SetFocusedRowCellValue("Dscription", Function.ToString(rows[0]["ItemName"]));
			gv_line.SetFocusedRowCellValue("TranType", Function.ToString(rows[0]["ManBatchSerial"]));
			gv_line.SetFocusedRowCellValue("ItemCode", Function.ToString(rows[0]["ItemCode"]));
			gv_line.SetFocusedRowCellValue("UgpEntry", Function.ParseInt(rows[0]["UgpCode"]));
			gv_line.SetFocusedRowCellValue("UomEntry", 0);
			gv_line.SetFocusedRowCellValue("UomCode", "");
			gv_line.SetFocusedRowCellValue("NumPerMsr", 0);
			gv_line.SetFocusedRowCellValue("InvQty", Function.ParseDouble(rows[0]["OnWhs"]));
			if (Function.ParseInt(rows[0]["UgpCode"]) == -1)
			{
				gv_line.SetFocusedRowCellValue("UomEntry", -1);
				gv_line.SetFocusedRowCellValue("UomCode", "Manual");
				gv_line.SetFocusedRowCellValue("NumPerMsr", 1);
			}
			for (int i = 1; i < rows.Count; i++)
			{
				DataRow dataRow = (from t in _dtLine.AsEnumerable()
					where Function.ToString(t["ItemCode"]) == ""
					select t).FirstOrDefault();
				if (dataRow != null)
				{
					dataRow["ItemCode"] = Function.ToString(rows[i]["ItemCode"]);
					dataRow["Dscription"] = Function.ToString(rows[i]["ItemName"]);
					dataRow["TranType"] = Function.ToString(rows[i]["ManBatchSerial"]);
					dataRow["UgpEntry"] = Function.ParseInt(rows[i]["UgpCode"]);
					dataRow["UomEntry"] = 0;
					dataRow["UomCode"] = "";
					dataRow["NumPerMsr"] = 0;
					dataRow["InvQty"] = 0;
					if (Function.ParseInt(rows[i]["UgpCode"]) == -1)
					{
						dataRow["UomEntry"] = -1;
						dataRow["UomCode"] = "Manual";
						dataRow["NumPerMsr"] = 1;
					}
				}
				NewItem();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void DistNumber_HandleChooseItem(DataRow dr)
	{
	}

	private void DistNumber_ChooseMulti(List<DataRow> rows)
	{
		try
		{
			_dtBatch = grd_batch.DataSourceTable();
			if (_dtBatch == null || _dtBatch.Rows.Count <= 0 || rows.Count <= 0)
			{
				return;
			}
			string text = Function.ToString(gv_line.GetFocusedRowCellValue("TranType"));
			if (text.Equals("B"))
			{
				gv_batch.SetFocusedRowCellValue("IsBatch", "Y");
			}
			else
			{
				gv_batch.SetFocusedRowCellValue("IsBatch", "N");
			}
			gv_batch.SetFocusedRowCellValue("MdAbsEntry", Function.ParseInt(rows[0]["AbsEntry"]));
			gv_batch.SetFocusedRowCellValue("SysNumber", Function.ParseInt(rows[0]["SysNumber"]));
			gv_batch.SetFocusedRowCellValue("ItemCode", Function.ToString(rows[0]["ItemCode"]));
			gv_batch.SetFocusedRowCellValue("DistNumber", Function.ToString(rows[0]["DistNumber"]));
			for (int i = 1; i < rows.Count; i++)
			{
				DataRow dataRow = (from t in _dtBatch.AsEnumerable()
					where Function.ToString(t["DistNumber"]) == ""
					select t).FirstOrDefault();
				if (dataRow != null)
				{
					dataRow["MdAbsEntry"] = Function.ToString(rows[i]["AbsEntry"]);
					dataRow["SysNumber"] = Function.ToString(rows[i]["SysNumber"]);
					dataRow["ItemCode"] = Function.ToString(rows[i]["ItemCode"]);
					dataRow["DistNumber"] = Function.ToString(rows[i]["DistNumber"]);
					if (text.Equals("B"))
					{
						dataRow["IsBatch"] = "Y";
					}
					else
					{
						dataRow["IsBatch"] = "N";
					}
					continue;
				}
				NewBatch();
				DataRow dataRow2 = (from t in _dtBatch.AsEnumerable()
					where Function.ToString(t["DistNumber"]) == ""
					select t).FirstOrDefault();
				dataRow2["MdAbsEntry"] = Function.ToString(rows[i]["AbsEntry"]);
				dataRow2["SysNumber"] = Function.ToString(rows[i]["SysNumber"]);
				dataRow2["ItemCode"] = Function.ToString(rows[i]["ItemCode"]);
				dataRow2["DistNumber"] = Function.ToString(rows[i]["DistNumber"]);
				if (text.Equals("B"))
				{
					dataRow2["IsBatch"] = "Y";
				}
				else
				{
					dataRow2["IsBatch"] = "N";
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void Uom_HandleChooseItem(DataRow dr)
	{
		try
		{
			if (dr != null)
			{
				double num = Function.ParseDouble(gv_line.GetFocusedRowCellValue("Qty"));
				double num2 = 0.0;
				if (Function.ParseDouble(dr["AltQty"]) > 0.0)
				{
					num2 = Math.Round(Function.ParseDouble(dr["BaseQty"]) / Function.ParseDouble(dr["AltQty"]));
				}
				gv_line.SetFocusedRowCellValue("UomEntry", Function.ParseInt(dr["UomEntry"]));
				gv_line.SetFocusedRowCellValue("UomCode", Function.ToString(dr["UomCode"]));
				gv_line.SetFocusedRowCellValue("NumPerMsr", num2);
				gv_line.SetFocusedRowCellValue("InvQty", Math.Round(num * num2, 6));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<DataTable> LoadData(int id, string whs, string startFrom, string startTo, string status)
	{
		try
		{
			_isLoading = true;
			DataTable result = await _httpClient.InventoryCounting.LoadData(id, whs.Trim(), startFrom, startTo, status);
			_isLoading = false;
			return result;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	private async Task LoadLineById(int id)
	{
		try
		{
			WinCommons._OpenProcessing();
			_isLoading = true;
			DataSet dataSet = await _httpClient.InventoryCounting.GetDataById(id);
			if (dataSet.IsNotNull() && dataSet.Tables.Count > 0)
			{
				grd_line.DataSource = dataSet.Tables[0];
				dtApplicantInterview = dataSet.Tables[1].Copy();
				grd_batch.DataSource = dataSet.Tables[1].Copy();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
		_isLoading = false;
	}

	private async Task<DataTable> GetBatchByLineNum(int lineNum, int docEntry)
	{
		try
		{
			DataTable dataTable = grd_batch.DataSourceTable().Clone();
			if (dtApplicantInterview.Rows.Count > 0 && lineNum > 0)
			{
				DataRow[] source = dtApplicantInterview.Select($"DocEntry={docEntry} and BaseLine = {lineNum}");
				if (source.Count() > 0)
				{
					dataTable = source.CopyToDataTable();
				}
				else
				{
					DataRow dataRow = dataTable.NewRow();
					dataRow["DocEntry"] = docEntry;
					dataRow["BaseLine"] = lineNum;
					dataTable.Rows.Add(dataRow);
					dataTable.AcceptChanges();
				}
			}
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	public void BatchAndSerial()
	{
		try
		{
			DataTable dataSourceTable = grd_batch.DataSourceTable;
			if (dataSourceTable != null && dataSourceTable.Rows.Count > 0)
			{
				DataRow[] array = dtApplicantInterview.Select(string.Format("BaseLine = {0} ", Function.ParseInt(dataSourceTable.Rows[0]["BaseLine"])));
				for (int i = 0; i < array.Length; i++)
				{
					dtApplicantInterview.Rows.Remove(array[i]);
				}
				for (int j = 0; j < dataSourceTable.Rows.Count; j++)
				{
					DataRow dataRow = dtApplicantInterview.NewRow();
					dataRow.ItemArray = dataSourceTable.Rows[j].ItemArray;
					dtApplicantInterview.Rows.Add(dataRow);
				}
				dtApplicantInterview.AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<string> CheckBatchSerNum(string itemCode)
	{
		try
		{
			return await _httpClient.InventoryCounting.CheckBatchSerNum(itemCode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
			return "";
		}
	}

	public async Task REFRESH()
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View || (FormMode != Enums.FormViewModeType.Update && FormMode != Enums.FormViewModeType.Add) || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) != DialogResult.No)
			{
				FormMode = Enums.FormViewModeType.Add;
				grd_Doc.DataSourceTable.Rows.Clear();
				grd_line.DataSourceTable().Rows.Clear();
				grd_batch.DataSourceTable.Rows.Clear();
				_dtDoc = grd_Doc.DataSourceTable;
				if (_dtDoc == null || _dtDoc.Columns.Count == 0)
				{
					WinCommons.Error(Enums.MessageCode.InfoCannotLoadData, "");
				}
				int num = await GetMaxDocNum();
				DataRow dataRow = _dtDoc.NewRow();
				dataRow["DocDate"] = DateTime.Now.Date;
				dataRow["DocStatus"] = "D";
				dataRow["DocEntry"] = num + 1;
				dataRow["Taker1Type"] = 12;
				dataRow["CountType"] = 1;
				_dtDoc.Rows.Add(dataRow);
				grd_Doc.DataSource = _dtDoc;
				NewItem();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			btn_Search_Click(null, null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void NewItem()
	{
		try
		{
			_dtLine = grd_line.DataSourceTable();
			if (_dtLine == null || _dtLine.Columns.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.InfoCannotLoadData, "");
				return;
			}
			int num = Function.ParseInt(_dtLine.Compute("MAX(LineNum)", ""));
			string value = Function.ToString(gv_Doc.GetFocusedRowCellValue("WhsCode"));
			DataRow dataRow = _dtLine.NewRow();
			dataRow["DocEntry"] = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("DocEntry"));
			dataRow["LineNum"] = num + 1;
			dataRow["WhsCode"] = value;
			_dtLine.Rows.Add(dataRow);
			_dtLine.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void NewBatch()
	{
		try
		{
			int num = Function.ParseInt(gv_line.GetFocusedRowCellValue("LineNum"));
			int num2 = Function.ParseInt(gv_line.GetFocusedRowCellValue("DocEntry"));
			_dtBatch = grd_batch.DataSourceTable;
			int num3 = Function.ParseInt(_dtBatch.Compute("MAX(LineId)", ""));
			DataRow dataRow = _dtBatch.NewRow();
			dataRow["DocEntry"] = num2;
			dataRow["BaseLine"] = num;
			dataRow["LineId"] = num3 + 1;
			_dtBatch.Rows.Add(dataRow);
			_dtBatch.AcceptChanges();
			grd_batch.DataSource = _dtBatch;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void DataChanged()
	{
		try
		{
			if (!_isLoading && FormMode == Enums.FormViewModeType.View)
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

	private void LoadInItForm()
	{
		try
		{
			List<Common.Table_Code> dataSource = _lstValidA.Select((KeyValuePair<string, string> item) => new Common.Table_Code
			{
				Code = item.Key,
				Name = item.Value
			}).ToList();
			_typeP.DataSource = dataSource;
			_typeP.DisplayMember = "Name";
			_typeP.ValueMember = "Code";
			_typeP.NullText = string.Empty;
			_lstValidC.Select((KeyValuePair<string, string> item) => new Common.Table_Code
			{
				Code = item.Key,
				Name = item.Value
			}).ToList();
			_typeC.DataSource = _lstValidC;
			_typeC.DisplayMember = "Name";
			_typeC.ValueMember = "Code";
			_typeC.NullText = string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<int> Save(int type)
	{
		_ = 1;
		try
		{
			_dtDoc = grd_Doc.DatasourceTable();
			_dtLine = grd_line.DataSourceTable;
			BatchAndSerial();
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				WinCommons._OpenProcessing();
				DataSet dataSet = new DataSet();
				if (type == 2)
				{
					DataTable dataSourceTable = grd_Doc.DataSourceTable;
					dataSourceTable.AcceptChanges();
					DataRow[] array = dataSourceTable.Select("DocEntry=" + Function.ParseInt(gv_Doc.GetFocusedRowCellValue("DocEntry")));
					if (array.Length != 0)
					{
						_dtDoc = array.CopyToDataTable();
					}
				}
				else
				{
					_dtDoc = grd_Doc.DataSourceTable;
				}
				DataTable dataTable = _dtLine.Copy();
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					if (Function.ToString(dataTable.Rows[i]["ItemCode"]) == "")
					{
						dataTable.Rows.RemoveAt(i--);
					}
				}
				dataTable.AcceptChanges();
				_dtBatch = dtApplicantInterview;
				(from DataRow t in _dtBatch.Rows
					where string.IsNullOrEmpty(Function.ToString(t["DistNumber"]))
					select t).ToList().ForEach((DataRow r) =>
				{
					r.Delete();
				});
				_dtBatch.AcceptChanges();
				(from DataRow t in _dtBatch.Rows
					where string.IsNullOrEmpty(Function.ToString(t["Qty"]))
					select t).ToList().ForEach((DataRow r) =>
				{
					r.Delete();
				});
				_dtBatch.AcceptChanges();
				Enums.MessageCode messageCode = UpdateBatchEditor();
				if (messageCode != Enums.MessageCode.Success)
				{
					WinCommons.Warning(messageCode, "");
					return 1;
				}
				_dtDoc.TableName = "dtDoc";
				dataTable.TableName = "dtLine";
				_dtBatch.TableName = "dtBatch";
				dataSet.Tables.Add(_dtDoc.Copy());
				dataSet.Tables.Add(dataTable.Copy());
				dataSet.Tables.Add(_dtBatch.Copy());
				HttpResult httpResult = ((type != 2) ? (await _httpClient.InventoryCounting.Add(dataSet)) : (await _httpClient.InventoryCounting.Update(dataSet)));
				if (string.IsNullOrEmpty(httpResult.Message))
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
					_isLoading = true;
					FormMode = Enums.FormViewModeType.View;
					if (type == 2)
					{
						grd_Doc.DatasourceTable().AcceptChanges();
						grd_line.DatasourceTable().AcceptChanges();
						grd_batch.DatasourceTable().AcceptChanges();
					}
					else
					{
						btn_Search_Click(null, null);
					}
					_isLoading = false;
					return 0;
				}
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				return 1;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
			return 1;
		}
		WinCommons._CloseProcessing();
		return 1;
	}

	private Enums.MessageCode UpdateBatchEditor()
	{
		DataTable dataSourceTable = grd_batch.DataSourceTable;
		if (dataSourceTable == null || dataSourceTable.Rows.Count == 0)
		{
			return Enums.MessageCode.Success;
		}
		DataRow[] array = grd_line.DatasourceTable().Select(string.Format("{0} = {1} and isnull({2},'') <> ''", "LineNum", Function.ParseInt(dataSourceTable.Rows[0]["BaseLine"]), "ItemCode"));
		if (array.Length > 1)
		{
			return Enums.MessageCode.LineNumberIsDuplicate;
		}
		if (array.Length == 0)
		{
			return Enums.MessageCode.LineDoesNotExists;
		}
		double num = Function.ParseDouble(array[0]["Qty"]) - Function.ParseDouble(dataSourceTable.Compute($"Sum({gb_Qty.FieldName})", ""));
		int num2 = dataSourceTable.Select($"isnull({gb_Qty.FieldName},0) = 0 and Isnull(DistNumber,'') <> ''").Length;
		if (num != 0.0 && num2 > 0)
		{
			return Enums.MessageCode.QuantityIsGreaterThanRequired;
		}
		for (int i = 0; i < dataSourceTable.Rows.Count; i++)
		{
			if (!string.IsNullOrEmpty(Function.ToString(dataSourceTable.Rows[i][gb_DistNumber.FieldName])) && Function.ParseDouble(dataSourceTable.Rows[i][gb_Qty.FieldName]) <= 0.0)
			{
				dataSourceTable.Rows[i][gb_Qty.FieldName] = num / (double)num2;
			}
		}
		var list = (from row in grd_line.DataSourceTable.Copy().AsEnumerable()
			group row by new
			{
				ItemCode = row.Field<string>("ItemCode"),
				WhsCode = row.Field<string>("WhsCode")
			} into gr
			where gr.Count() > 1
			select gr).ToList();
		if (list != null && list.Count() > 0)
		{
			return Enums.MessageCode.ThisItemCodeInWhsAlreadyExistsInFollowingGrid;
		}
		dataSourceTable.AcceptChanges();
		grd_batch.DataSource = dataSourceTable;
		grd_batch.RefreshDataSource();
		return Enums.MessageCode.Success;
	}

	private async Task<int> GetMaxDocNum()
	{
		try
		{
			return await _httpClient.InventoryCounting.GetMaxDocEntry();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
			return 0;
		}
	}

	public async Task MOVE_FIRST()
	{
		WinCommons._OpenProcessing();
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			if (await FindNextLastFrist(Enums.NavigateType.First) == 0)
			{
				FormMode = Enums.FormViewModeType.View;
				WinCommons._CloseProcessing();
				return;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_NEXT()
	{
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			WinCommons._OpenProcessing();
			if (await FindNextLastFrist(Enums.NavigateType.Next) == 0)
			{
				FormMode = Enums.FormViewModeType.View;
				WinCommons._CloseProcessing();
				return;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_LAST()
	{
		WinCommons._OpenProcessing();
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			if (await FindNextLastFrist(Enums.NavigateType.Last) == 0)
			{
				FormMode = Enums.FormViewModeType.View;
				WinCommons._CloseProcessing();
				return;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_PREVIOUS()
	{
		WinCommons._OpenProcessing();
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			if (await FindNextLastFrist(Enums.NavigateType.Previous) == 0)
			{
				FormMode = Enums.FormViewModeType.View;
				WinCommons._CloseProcessing();
				return;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	public async Task<int> FindNextLastFrist(Enums.NavigateType type)
	{
		_ = 1;
		try
		{
			DataTable dataTable = await _httpClient.InventoryCounting.FindNextLastFrist(Function.ParseInt(txt_No.EditValue), (int)type);
			if (dataTable.IsNotNull())
			{
				grd_Doc.DataSource = dataTable;
				grd_Doc.DatasourceTable().AcceptChanges();
				int num = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("DocEntry"));
				txt_No.EditValue = num;
				await LoadLineById(num);
				FormMode = Enums.FormViewModeType.View;
				return 1;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		return 0;
	}

	private async void gv_line_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
	{
		try
		{
			if (gv_line == null)
			{
				return;
			}
			_dtLine = grd_line.DatasourceTable();
			_dtBatch = grd_batch.DataSourceTable;
			if (gv_line.FocusedColumn.FieldName == "Qty" && string.Equals(await CheckBatchSerNum(Function.ToString(gv_line.GetFocusedRowCellValue("ItemCode"))), "Y"))
			{
				_dtLine.AcceptChanges();
				if (_dtBatch != null && _dtBatch.Rows.Count > 0)
				{
					double num = Function.ParseDouble(e.Value) - Function.ParseDouble(_dtBatch.Compute("SUM(Qty)", ""));
					if (Function.ParseDouble(e.Value) < 0.0)
					{
						e.Valid = false;
						e.ErrorText = "";
						WinCommons.Warning(Enums.MessageCode.InQtyValueGreater0, "");
						return;
					}
					if (num != 0.0)
					{
						e.Valid = false;
						e.ErrorText = "";
						WinCommons.Warning(Enums.MessageCode.AllocateBatchSerialQtyEquaItemQuantity, "");
						return;
					}
				}
			}
			if (gv_line.FocusedColumn.FieldName == "WhsCode")
			{
				_dtLine.AcceptChanges();
				string text = Function.ToString(gv_line.GetFocusedRowCellValue("ItemCode"));
				string text2 = Function.ToString(e.Value);
				if (_dtLine.Select($"ItemCode='{text}' and WhsCode='{text2}'").ToArray().Count() > 1)
				{
					e.Valid = false;
					e.ErrorText = "";
					WinCommons.Warning(Enums.MessageCode.ThisItemCodeInWhsAlreadyExistsInFollowingGrid, "", text, text2);
					return;
				}
			}
			if (gv_line.FocusedColumn.FieldName == "ItemCode")
			{
				_dtLine.AcceptChanges();
				string text3 = Function.ToString(gv_line.GetFocusedRowCellValue("WhsCode"));
				string text4 = Function.ToString(e.Value);
				if (_dtLine.Select($"ItemCode='{text4}' and WhsCode='{text3}'").ToArray().Count() > 1)
				{
					e.Valid = false;
					e.ErrorText = "";
					WinCommons.Warning(Enums.MessageCode.ItemInWhsExistsInGrid, "", text4, text3);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
			WinCommons.Error(Enums.MessageCode.None, ex.Message);
		}
	}

	private void gv_line_InvalidValueException(object sender, InvalidValueExceptionEventArgs e)
	{
		try
		{
			if (sender is ColumnView columnView)
			{
				e.ExceptionMode = ExceptionMode.DisplayError;
				e.WindowCaption = "Input Error";
				columnView.HideEditor();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
			WinCommons.Error(Enums.MessageCode.None, ex.Message);
		}
	}

	private void btn_ItemCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				_ = gv_line.Columns["ItemCode"].OptionsColumn.ReadOnly;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void frm_InventoryCounting_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if ((FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCloseForm, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
			{
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Inventory.InventoryTransaction.frm_InventoryCounting));
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions4 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions5 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions6 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions4 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions7 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance11 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered11 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed11 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled11 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions5 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance12 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered12 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed12 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled12 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions8 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance13 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered13 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed13 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled13 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions6 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance14 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered14 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed14 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled14 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions9 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance15 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered15 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed15 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled15 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions10 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance16 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered16 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed16 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled16 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions11 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance17 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered17 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed17 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled17 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions12 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance18 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered18 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed18 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled18 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions13 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance19 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered19 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed19 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled19 = new DevExpress.Utils.SerializableAppearanceObject();
		this.repositoryItemCalcEditEx2 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx3 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx4 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx1 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.btn_Ok = new Apzon.Controls.Controls.SimpleButtonEx();
		this.lb_DocStatus = new Apzon.Controls.Controls.LabelLine();
		this.lb_Whs = new Apzon.Controls.Controls.LabelLine();
		this.lb_DocNo = new Apzon.Controls.Controls.LabelLine();
		this.lb_ToDate = new Apzon.Controls.Controls.LabelLine();
		this.lb_FromDate = new Apzon.Controls.Controls.LabelLine();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Search = new Apzon.Controls.Controls.SimpleButtonEx();
		this.txt_No = new Apzon.Controls.Editor.TextEditEx();
		this.txtToDate = new Apzon.Controls.Editor.DateEditEx();
		this.txtFromDate = new Apzon.Controls.Editor.DateEditEx();
		this.split_Main = new DevExpress.XtraEditors.SplitContainerControl();
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.ctm_Doc = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.duplicateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.cancelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_RequestNo = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx4 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_WhsDoc = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx5 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_StatusDoc = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.gridColumnEx6 = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_CountType = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_CountType = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.gc_Taker1Type = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_Taker1Type = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.gc_Taker1Id = new Apzon.Controls.Grid.GridColumnEx();
		this.split_LineBatchAndSerial = new DevExpress.XtraEditors.SplitContainerControl();
		this.xtraTabControl1 = new Apzon.Controls.Controls.XtraTabControlEx();
		this.xtra_Item = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_line = new Apzon.Controls.Grid.GridControlEx();
		this.ctm_Line = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.duplicateRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_line = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx7 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx8 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx9 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_ItemCode = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx10 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx11 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_WhsLine = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx12 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx13 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx19 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx20 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx21 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx22 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_UomCode = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx23 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx25 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx26 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx27 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_BinItem = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.xtraTabControl2 = new Apzon.Controls.Controls.XtraTabControlEx();
		this.xtra_Batch = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_batch = new Apzon.Controls.Grid.GridControlEx();
		this.ctm_Batch = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addRowToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteRowToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_batch = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx14 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx15 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx16 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx17 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx18 = new Apzon.Controls.Grid.GridColumnEx();
		this.gb_DistNumber = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_DisNumber = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gb_itemCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gb_BinAbs = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemButtonEditCFL1 = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gb_BinCode = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_BinCode = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gb_Qty = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx24 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_Whs = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.luk_Status = new Apzon.Controls.Editor.MyLookUpEdit.LookUpEditSimple();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_No.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtToDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtToDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtFromDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtFromDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.split_Main).BeginInit();
		this.split_Main.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		this.ctm_Doc.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_RequestNo).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_WhsDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_StatusDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_CountType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Taker1Type).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.split_LineBatchAndSerial).BeginInit();
		this.split_LineBatchAndSerial.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).BeginInit();
		this.xtraTabControl1.SuspendLayout();
		this.xtra_Item.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_line).BeginInit();
		this.ctm_Line.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_line).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_ItemCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_WhsLine).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_UomCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_BinItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl2).BeginInit();
		this.xtraTabControl2.SuspendLayout();
		this.xtra_Batch.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_batch).BeginInit();
		this.ctm_Batch.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_batch).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_DisNumber).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemButtonEditCFL1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_BinCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_Whs.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Status.Properties).BeginInit();
		base.SuspendLayout();
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
		this.repositoryItemCalcEditEx1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx1.IsSearchControl = false;
		this.repositoryItemCalcEditEx1.Name = "repositoryItemCalcEditEx1";
		this.btn_Ok.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Ok.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Ok.Appearance.Options.UseBackColor = true;
		this.btn_Ok.ButtonKind = Apzon.Commons.Enums.Enums.SimpleButtonType.Save;
		this.btn_Ok.EditControl = null;
		this.btn_Ok.EditValue = "OK";
		this.btn_Ok.FieldName = null;
		this.btn_Ok.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Ok.ImageOptions.Image");
		this.btn_Ok.IsSearchControl = false;
		this.btn_Ok.Location = new System.Drawing.Point(2, 521);
		this.btn_Ok.Name = "btn_Ok";
		this.btn_Ok.ReadOnly = false;
		this.btn_Ok.SettingInVisible = false;
		this.btn_Ok.SettingReadonly = false;
		this.btn_Ok.Size = new System.Drawing.Size(99, 20);
		this.btn_Ok.TabIndex = 80;
		this.btn_Ok.Text = "OK";
		this.btn_Ok.Click += new System.EventHandler(this.btn_Ok_Click);
		this.lb_DocStatus.EditControl = null;
		this.lb_DocStatus.EditValue = "Status";
		this.lb_DocStatus.FieldName = null;
		this.lb_DocStatus.HideWhenEmpty = false;
		this.lb_DocStatus.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_DocStatus.Location = new System.Drawing.Point(461, 25);
		this.lb_DocStatus.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_DocStatus.Name = "lb_DocStatus";
		this.lb_DocStatus.ReadOnly = false;
		this.lb_DocStatus.SettingInVisible = false;
		this.lb_DocStatus.SettingReadonly = false;
		this.lb_DocStatus.ShowUnderLine = true;
		this.lb_DocStatus.Size = new System.Drawing.Size(61, 20);
		this.lb_DocStatus.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_DocStatus.TabIndex = 77;
		this.lb_DocStatus.Utext = "Status";
		this.lb_Whs.EditControl = null;
		this.lb_Whs.EditValue = "Warehouse";
		this.lb_Whs.FieldName = null;
		this.lb_Whs.HideWhenEmpty = false;
		this.lb_Whs.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Whs.Location = new System.Drawing.Point(227, 1);
		this.lb_Whs.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_Whs.Name = "lb_Whs";
		this.lb_Whs.ReadOnly = false;
		this.lb_Whs.SettingInVisible = false;
		this.lb_Whs.SettingReadonly = false;
		this.lb_Whs.ShowUnderLine = true;
		this.lb_Whs.Size = new System.Drawing.Size(88, 20);
		this.lb_Whs.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Whs.TabIndex = 76;
		this.lb_Whs.Utext = "Warehouse";
		this.lb_DocNo.EditControl = null;
		this.lb_DocNo.EditValue = "Doc. No";
		this.lb_DocNo.FieldName = null;
		this.lb_DocNo.HideWhenEmpty = false;
		this.lb_DocNo.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_DocNo.Location = new System.Drawing.Point(227, 26);
		this.lb_DocNo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_DocNo.Name = "lb_DocNo";
		this.lb_DocNo.ReadOnly = false;
		this.lb_DocNo.SettingInVisible = false;
		this.lb_DocNo.SettingReadonly = false;
		this.lb_DocNo.ShowUnderLine = true;
		this.lb_DocNo.Size = new System.Drawing.Size(88, 20);
		this.lb_DocNo.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_DocNo.TabIndex = 75;
		this.lb_DocNo.Utext = "Doc. No";
		this.lb_ToDate.EditControl = null;
		this.lb_ToDate.EditValue = "To Date";
		this.lb_ToDate.FieldName = null;
		this.lb_ToDate.HideWhenEmpty = false;
		this.lb_ToDate.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_ToDate.Location = new System.Drawing.Point(1, 26);
		this.lb_ToDate.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_ToDate.Name = "lb_ToDate";
		this.lb_ToDate.ReadOnly = false;
		this.lb_ToDate.SettingInVisible = false;
		this.lb_ToDate.SettingReadonly = false;
		this.lb_ToDate.ShowUnderLine = true;
		this.lb_ToDate.Size = new System.Drawing.Size(80, 20);
		this.lb_ToDate.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_ToDate.TabIndex = 74;
		this.lb_ToDate.Utext = "To Date";
		this.lb_FromDate.EditControl = null;
		this.lb_FromDate.EditValue = "From Date";
		this.lb_FromDate.FieldName = null;
		this.lb_FromDate.HideWhenEmpty = false;
		this.lb_FromDate.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_FromDate.Location = new System.Drawing.Point(1, 1);
		this.lb_FromDate.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_FromDate.Name = "lb_FromDate";
		this.lb_FromDate.ReadOnly = false;
		this.lb_FromDate.SettingInVisible = false;
		this.lb_FromDate.SettingReadonly = false;
		this.lb_FromDate.ShowUnderLine = true;
		this.lb_FromDate.Size = new System.Drawing.Size(80, 20);
		this.lb_FromDate.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_FromDate.TabIndex = 73;
		this.lb_FromDate.Utext = "From Date";
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnCancel.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btnCancel.Appearance.Options.UseBackColor = true;
		this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btnCancel.EditControl = null;
		this.btnCancel.EditValue = "Cancel";
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(104, 521);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(99, 20);
		this.btnCancel.TabIndex = 72;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.btn_Search.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Search.Appearance.Options.UseBackColor = true;
		this.btn_Search.EditControl = null;
		this.btn_Search.EditValue = "Search";
		this.btn_Search.FieldName = null;
		this.btn_Search.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Search.ImageOptions.Image");
		this.btn_Search.IsSearchControl = false;
		this.btn_Search.Location = new System.Drawing.Point(636, 24);
		this.btn_Search.Name = "btn_Search";
		this.btn_Search.ReadOnly = false;
		this.btn_Search.SettingInVisible = false;
		this.btn_Search.SettingReadonly = false;
		this.btn_Search.Size = new System.Drawing.Size(99, 20);
		this.btn_Search.TabIndex = 71;
		this.btn_Search.Text = "Search";
		this.btn_Search.Click += new System.EventHandler(this.btn_Search_Click);
		this.txt_No.EditControl = null;
		this.txt_No.FieldName = null;
		this.txt_No.IsSearchControl = false;
		this.txt_No.Location = new System.Drawing.Point(315, 25);
		this.txt_No.Name = "txt_No";
		this.txt_No.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 6.5f);
		this.txt_No.Properties.Appearance.Options.UseFont = true;
		this.txt_No.SettingInVisible = false;
		this.txt_No.SettingReadonly = false;
		this.txt_No.Size = new System.Drawing.Size(140, 18);
		this.txt_No.TabIndex = 69;
		this.txtToDate.EditControl = null;
		this.txtToDate.EditValue = null;
		this.txtToDate.FieldName = null;
		this.txtToDate.Location = new System.Drawing.Point(81, 25);
		this.txtToDate.Name = "txtToDate";
		this.txtToDate.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 6.5f);
		this.txtToDate.Properties.Appearance.Options.UseFont = true;
		this.txtToDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtToDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtToDate.Properties.IsSearchControl = false;
		this.txtToDate.SettingInVisible = false;
		this.txtToDate.SettingReadonly = false;
		this.txtToDate.Size = new System.Drawing.Size(140, 18);
		this.txtToDate.TabIndex = 67;
		this.txtFromDate.EditControl = null;
		this.txtFromDate.EditValue = null;
		this.txtFromDate.FieldName = null;
		this.txtFromDate.Location = new System.Drawing.Point(81, 1);
		this.txtFromDate.Name = "txtFromDate";
		this.txtFromDate.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 6.5f);
		this.txtFromDate.Properties.Appearance.Options.UseFont = true;
		this.txtFromDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtFromDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtFromDate.Properties.IsSearchControl = false;
		this.txtFromDate.SettingInVisible = false;
		this.txtFromDate.SettingReadonly = false;
		this.txtFromDate.Size = new System.Drawing.Size(140, 18);
		this.txtFromDate.TabIndex = 66;
		this.split_Main.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.split_Main.Horizontal = false;
		this.split_Main.Location = new System.Drawing.Point(1, 51);
		this.split_Main.Name = "split_Main";
		this.split_Main.Panel1.Controls.Add(this.grd_Doc);
		this.split_Main.Panel1.Text = "Panel1";
		this.split_Main.Panel2.Controls.Add(this.split_LineBatchAndSerial);
		this.split_Main.Panel2.Text = "Panel2";
		this.split_Main.Size = new System.Drawing.Size(844, 464);
		this.split_Main.SplitterPosition = 162;
		this.split_Main.TabIndex = 81;
		this.split_Main.Text = "splitContainerControl1";
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.ContextMenuStrip = this.ctm_Doc;
		this.grd_Doc.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Doc.Location = new System.Drawing.Point(0, 0);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[5] { this.btn_WhsDoc, this.luk_StatusDoc, this.btn_RequestNo, this.luk_Taker1Type, this.luk_CountType });
		this.grd_Doc.Size = new System.Drawing.Size(844, 162);
		this.grd_Doc.TabIndex = 0;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.ctm_Doc.Items.AddRange(new System.Windows.Forms.ToolStripItem[4] { this.addToolStripMenuItem, this.duplicateToolStripMenuItem, this.closeToolStripMenuItem, this.cancelToolStripMenuItem });
		this.ctm_Doc.Name = "ctm_Doc";
		this.ctm_Doc.Size = new System.Drawing.Size(125, 92);
		this.addToolStripMenuItem.Name = "addToolStripMenuItem";
		this.addToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
		this.addToolStripMenuItem.Text = "Add New";
		this.addToolStripMenuItem.Click += new System.EventHandler(this.addToolStripMenuItem_Click);
		this.duplicateToolStripMenuItem.Name = "duplicateToolStripMenuItem";
		this.duplicateToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
		this.duplicateToolStripMenuItem.Text = "Duplicate";
		this.duplicateToolStripMenuItem.Click += new System.EventHandler(this.duplicateToolStripMenuItem_Click);
		this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
		this.closeToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
		this.closeToolStripMenuItem.Text = "Close";
		this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
		this.cancelToolStripMenuItem.Name = "cancelToolStripMenuItem";
		this.cancelToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
		this.cancelToolStripMenuItem.Text = "Cancel";
		this.cancelToolStripMenuItem.Click += new System.EventHandler(this.cancelToolStripMenuItem_Click);
		this.gv_Doc.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Doc.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = null;
		this.gv_Doc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[9] { this.gridColumnEx1, this.gridColumnEx2, this.gridColumnEx3, this.gridColumnEx4, this.gridColumnEx5, this.gridColumnEx6, this.gc_CountType, this.gc_Taker1Type, this.gc_Taker1Id });
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = "oInventoryCounting";
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.IgnoreFilter = false;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 35;
		this.gv_Doc.IsApzTable = false;
		this.gv_Doc.IsInsertNewRow = false;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.IsMappingContextAction = false;
		this.gv_Doc.ItemId = "APZ_OINC";
		this.gv_Doc.LstColumnAllowBatch = null;
		this.gv_Doc.LstColumnNotAllowBatch = null;
		this.gv_Doc.LstColumnNotEditable = null;
		this.gv_Doc.LstColumNotSetting = null;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Doc.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Doc.OptionsView.ColumnAutoWidth = false;
		this.gv_Doc.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Doc.OptionsView.ShowGroupPanel = false;
		this.gv_Doc.UseBatchMod = false;
		this.gv_Doc.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Doc_RowCellStyle);
		this.gv_Doc.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gv_Doc_CustomRowCellEdit);
		this.gv_Doc.CustomRowCellEditForEditing += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gv_Doc_CustomRowCellEditForEditing);
		this.gv_Doc.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_Doc_ShowingEditor);
		this.gv_Doc.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gv_Doc_FocusedRowChanged);
		this.gv_Doc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Doc_CellValueChanged);
		this.gv_Doc.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Doc_CellValueChanging);
		this.gv_Doc.BeforeLeaveRow += new DevExpress.XtraGrid.Views.Base.RowAllowEventHandler(this.gv_Doc_BeforeLeaveRow);
		this.gridColumnEx1.Caption = "Doc. No";
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "DocEntry";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.OptionsColumn.ReadOnly = true;
		this.gridColumnEx1.PrimaryKey = true;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx1.Visible = true;
		this.gridColumnEx1.VisibleIndex = 0;
		this.gridColumnEx2.Caption = "Req. No";
		this.gridColumnEx2.ColumnEdit = this.btn_RequestNo;
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.FieldName = "ReqNo";
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.UnitField = null;
		this.gridColumnEx2.Visible = true;
		this.gridColumnEx2.VisibleIndex = 1;
		this.btn_RequestNo.AutoHeight = false;
		this.btn_RequestNo.AutoLoadDataSource = true;
		editorButtonImageOptions.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions2.Image");
		this.btn_RequestNo.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_RequestNo.ContainerControl = this;
		this.btn_RequestNo.DisplayMember = "DocEntry";
		this.btn_RequestNo.DisplayMember2 = "";
		this.btn_RequestNo.LoadDataSourceWhenEnter = false;
		this.btn_RequestNo.Name = "btn_RequestNo";
		this.btn_RequestNo.SeparateCharacterForMulti = ';';
		this.btn_RequestNo.SeparateString = ", ";
		this.btn_RequestNo.UsingAutoCompleteBox = false;
		this.btn_RequestNo.UsingDataSource = true;
		this.btn_RequestNo.ValidateWhenNoDataSource = false;
		this.btn_RequestNo.ValueMember = "DocEntry";
		this.gridColumnEx3.Caption = "Counting Date";
		this.gridColumnEx3.CurrencyDateField = null;
		this.gridColumnEx3.CurrencyField = null;
		this.gridColumnEx3.CurrencyFixed = null;
		this.gridColumnEx3.CurrencyFixedRate = 0.0;
		this.gridColumnEx3.CurrencyRateField = null;
		this.gridColumnEx3.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gridColumnEx3.FieldName = "DocDate";
		this.gridColumnEx3.Name = "gridColumnEx3";
		this.gridColumnEx3.PrimaryKey = false;
		this.gridColumnEx3.UnitField = null;
		this.gridColumnEx3.Visible = true;
		this.gridColumnEx3.VisibleIndex = 2;
		this.gridColumnEx3.Width = 120;
		this.gridColumnEx4.Caption = "Warehouse";
		this.gridColumnEx4.ColumnEdit = this.btn_WhsDoc;
		this.gridColumnEx4.CurrencyDateField = null;
		this.gridColumnEx4.CurrencyField = null;
		this.gridColumnEx4.CurrencyFixed = null;
		this.gridColumnEx4.CurrencyFixedRate = 0.0;
		this.gridColumnEx4.CurrencyRateField = null;
		this.gridColumnEx4.FieldName = "WhsCode";
		this.gridColumnEx4.Name = "gridColumnEx4";
		this.gridColumnEx4.PrimaryKey = false;
		this.gridColumnEx4.UnitField = null;
		this.gridColumnEx4.Visible = true;
		this.gridColumnEx4.VisibleIndex = 3;
		this.btn_WhsDoc.AutoHeight = false;
		this.btn_WhsDoc.AutoLoadDataSource = true;
		this.btn_WhsDoc.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, imageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance4, appearanceHovered4, appearancePressed4, appearanceDisabled4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_WhsDoc.ContainerControl = this;
		this.btn_WhsDoc.DisplayMember = "WhsCode";
		this.btn_WhsDoc.DisplayMember2 = "";
		this.btn_WhsDoc.LoadDataSourceWhenEnter = false;
		this.btn_WhsDoc.Name = "btn_WhsDoc";
		this.btn_WhsDoc.SeparateCharacterForMulti = ';';
		this.btn_WhsDoc.SeparateString = ", ";
		this.btn_WhsDoc.UsingAutoCompleteBox = false;
		this.btn_WhsDoc.UsingDataSource = true;
		this.btn_WhsDoc.ValidateWhenNoDataSource = false;
		this.btn_WhsDoc.ValueMember = "WhsCode";
		this.gridColumnEx5.Caption = "Status";
		this.gridColumnEx5.ColumnEdit = this.luk_StatusDoc;
		this.gridColumnEx5.CurrencyDateField = null;
		this.gridColumnEx5.CurrencyField = null;
		this.gridColumnEx5.CurrencyFixed = null;
		this.gridColumnEx5.CurrencyFixedRate = 0.0;
		this.gridColumnEx5.CurrencyRateField = null;
		this.gridColumnEx5.FieldName = "DocStatus";
		this.gridColumnEx5.Name = "gridColumnEx5";
		this.gridColumnEx5.PrimaryKey = false;
		this.gridColumnEx5.UnitField = null;
		this.gridColumnEx5.Visible = true;
		this.gridColumnEx5.VisibleIndex = 4;
		this.luk_StatusDoc.AutoHeight = false;
		this.luk_StatusDoc.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_StatusDoc.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_StatusDoc.IsSearchControl = false;
		this.luk_StatusDoc.Name = "luk_StatusDoc";
		this.luk_StatusDoc.NullText = "";
		this.luk_StatusDoc.ShowFooter = false;
		this.luk_StatusDoc.ShowHeader = false;
		this.gridColumnEx6.Caption = "Remarks";
		this.gridColumnEx6.CurrencyDateField = null;
		this.gridColumnEx6.CurrencyField = null;
		this.gridColumnEx6.CurrencyFixed = null;
		this.gridColumnEx6.CurrencyFixedRate = 0.0;
		this.gridColumnEx6.CurrencyRateField = null;
		this.gridColumnEx6.FieldName = "Remarks";
		this.gridColumnEx6.Name = "gridColumnEx6";
		this.gridColumnEx6.PrimaryKey = false;
		this.gridColumnEx6.UnitField = null;
		this.gridColumnEx6.Visible = true;
		this.gridColumnEx6.VisibleIndex = 5;
		this.gc_CountType.Caption = "Counting Type";
		this.gc_CountType.ColumnEdit = this.luk_CountType;
		this.gc_CountType.CurrencyDateField = null;
		this.gc_CountType.CurrencyField = null;
		this.gc_CountType.CurrencyFixed = null;
		this.gc_CountType.CurrencyFixedRate = 0.0;
		this.gc_CountType.CurrencyRateField = null;
		this.gc_CountType.FieldName = "CountType";
		this.gc_CountType.MinWidth = 75;
		this.gc_CountType.Name = "gc_CountType";
		this.gc_CountType.PrimaryKey = false;
		this.gc_CountType.UnitField = null;
		this.gc_CountType.Visible = true;
		this.gc_CountType.VisibleIndex = 6;
		this.gc_CountType.Width = 100;
		this.luk_CountType.AutoHeight = false;
		this.luk_CountType.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_CountType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_CountType.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.luk_CountType.DisplayMember = "Name";
		this.luk_CountType.IsSearchControl = false;
		this.luk_CountType.Name = "luk_CountType";
		this.luk_CountType.NullText = "";
		this.luk_CountType.ShowFooter = false;
		this.luk_CountType.ShowHeader = false;
		this.luk_CountType.ValueMember = "Code";
		this.gc_Taker1Type.Caption = "Counter Type";
		this.gc_Taker1Type.ColumnEdit = this.luk_Taker1Type;
		this.gc_Taker1Type.CurrencyDateField = null;
		this.gc_Taker1Type.CurrencyField = null;
		this.gc_Taker1Type.CurrencyFixed = null;
		this.gc_Taker1Type.CurrencyFixedRate = 0.0;
		this.gc_Taker1Type.CurrencyRateField = null;
		this.gc_Taker1Type.FieldName = "Taker1Type";
		this.gc_Taker1Type.MinWidth = 75;
		this.gc_Taker1Type.Name = "gc_Taker1Type";
		this.gc_Taker1Type.PrimaryKey = false;
		this.gc_Taker1Type.UnitField = null;
		this.gc_Taker1Type.Visible = true;
		this.gc_Taker1Type.VisibleIndex = 7;
		this.gc_Taker1Type.Width = 100;
		this.luk_Taker1Type.AutoHeight = false;
		this.luk_Taker1Type.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_Taker1Type.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Taker1Type.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.luk_Taker1Type.DisplayMember = "Name";
		this.luk_Taker1Type.IsSearchControl = false;
		this.luk_Taker1Type.Name = "luk_Taker1Type";
		this.luk_Taker1Type.NullText = "";
		this.luk_Taker1Type.ShowFooter = false;
		this.luk_Taker1Type.ShowHeader = false;
		this.luk_Taker1Type.ValueMember = "Code";
		this.gc_Taker1Id.Caption = "Counter";
		this.gc_Taker1Id.CurrencyDateField = null;
		this.gc_Taker1Id.CurrencyField = null;
		this.gc_Taker1Id.CurrencyFixed = null;
		this.gc_Taker1Id.CurrencyFixedRate = 0.0;
		this.gc_Taker1Id.CurrencyRateField = null;
		this.gc_Taker1Id.FieldName = "Taker1Id";
		this.gc_Taker1Id.MinWidth = 150;
		this.gc_Taker1Id.Name = "gc_Taker1Id";
		this.gc_Taker1Id.PrimaryKey = false;
		this.gc_Taker1Id.UnitField = null;
		this.gc_Taker1Id.Visible = true;
		this.gc_Taker1Id.VisibleIndex = 8;
		this.gc_Taker1Id.Width = 150;
		this.split_LineBatchAndSerial.Dock = System.Windows.Forms.DockStyle.Fill;
		this.split_LineBatchAndSerial.Horizontal = false;
		this.split_LineBatchAndSerial.Location = new System.Drawing.Point(0, 0);
		this.split_LineBatchAndSerial.Name = "split_LineBatchAndSerial";
		this.split_LineBatchAndSerial.Panel1.Controls.Add(this.xtraTabControl1);
		this.split_LineBatchAndSerial.Panel1.Text = "Panel1";
		this.split_LineBatchAndSerial.Panel2.Controls.Add(this.xtraTabControl2);
		this.split_LineBatchAndSerial.Panel2.Text = "Panel2";
		this.split_LineBatchAndSerial.Size = new System.Drawing.Size(844, 292);
		this.split_LineBatchAndSerial.SplitterPosition = 148;
		this.split_LineBatchAndSerial.TabIndex = 13;
		this.split_LineBatchAndSerial.Text = "splitContainerControl1";
		this.xtraTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.xtraTabControl1.IsSearchControl = false;
		this.xtraTabControl1.Location = new System.Drawing.Point(0, 0);
		this.xtraTabControl1.Name = "xtraTabControl1";
		this.xtraTabControl1.SelectedTabPage = this.xtra_Item;
		this.xtraTabControl1.Size = new System.Drawing.Size(844, 148);
		this.xtraTabControl1.TabIndex = 1;
		this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[1] { this.xtra_Item });
		this.xtra_Item.Controls.Add(this.grd_line);
		this.xtra_Item.EditControl = null;
		this.xtra_Item.EditValue = "Items";
		this.xtra_Item.FieldName = null;
		this.xtra_Item.IsSearchControl = false;
		this.xtra_Item.Name = "xtra_Item";
		this.xtra_Item.ReadOnly = false;
		this.xtra_Item.SettingInVisible = false;
		this.xtra_Item.SettingReadonly = false;
		this.xtra_Item.Size = new System.Drawing.Size(842, 123);
		this.xtra_Item.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtra_Item.Text = "Items";
		this.grd_line.AllowPasteExcel = true;
		this.grd_line.ClipboardData = null;
		this.grd_line.ContextMenuStrip = this.ctm_Line;
		this.grd_line.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_line.Location = new System.Drawing.Point(0, 0);
		this.grd_line.MainView = this.gv_line;
		this.grd_line.Name = "grd_line";
		this.grd_line.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[4] { this.btn_ItemCode, this.btn_WhsLine, this.btn_UomCode, this.btn_BinItem });
		this.grd_line.Size = new System.Drawing.Size(842, 123);
		this.grd_line.TabIndex = 0;
		this.grd_line.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_line });
		this.ctm_Line.Items.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.addRowToolStripMenuItem, this.deleteRowToolStripMenuItem, this.duplicateRowToolStripMenuItem });
		this.ctm_Line.Name = "ctm_Line";
		this.ctm_Line.Size = new System.Drawing.Size(151, 70);
		this.addRowToolStripMenuItem.Name = "addRowToolStripMenuItem";
		this.addRowToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
		this.addRowToolStripMenuItem.Text = "Add Row";
		this.addRowToolStripMenuItem.Click += new System.EventHandler(this.addRowToolStripMenuItem_Click);
		this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
		this.deleteRowToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
		this.deleteRowToolStripMenuItem.Text = "Delete Row";
		this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.deleteRowToolStripMenuItem_Click);
		this.duplicateRowToolStripMenuItem.Name = "duplicateRowToolStripMenuItem";
		this.duplicateRowToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
		this.duplicateRowToolStripMenuItem.Text = "Duplicate Row";
		this.duplicateRowToolStripMenuItem.Click += new System.EventHandler(this.duplicateRowToolStripMenuItem_Click);
		this.gv_line.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_line.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_line.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_line.ColumnKey = "ItemCode";
		this.gv_line.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[15]
		{
			this.gridColumnEx7, this.gridColumnEx8, this.gridColumnEx9, this.gridColumnEx10, this.gridColumnEx11, this.gridColumnEx12, this.gridColumnEx13, this.gridColumnEx19, this.gridColumnEx20, this.gridColumnEx21,
			this.gridColumnEx22, this.gridColumnEx23, this.gridColumnEx25, this.gridColumnEx26, this.gridColumnEx27
		});
		this.gv_line.DisableFormSetting = false;
		this.gv_line.DisplayException = false;
		this.gv_line.EditColumn = null;
		this.gv_line.FieldCurrency = null;
		this.gv_line.FieldRate = null;
		this.gv_line.FormId = "oInventoryCounting";
		this.gv_line.GridControl = this.grd_line;
		this.gv_line.IgnoreFilter = false;
		this.gv_line.IndexEditRow = -1;
		this.gv_line.IndicatorWidth = 35;
		this.gv_line.IsApzTable = false;
		this.gv_line.IsInsertNewRow = false;
		this.gv_line.IsLoadingData = false;
		this.gv_line.IsMappingContextAction = false;
		this.gv_line.ItemId = "APZ_INC1";
		this.gv_line.LstColumnAllowBatch = null;
		this.gv_line.LstColumnNotAllowBatch = null;
		this.gv_line.LstColumnNotEditable = null;
		this.gv_line.LstColumNotSetting = null;
		this.gv_line.Name = "gv_line";
		this.gv_line.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_line.OptionsView.BestFitMaxRowCount = 100;
		this.gv_line.OptionsView.ColumnAutoWidth = false;
		this.gv_line.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_line.OptionsView.ShowGroupPanel = false;
		this.gv_line.UseBatchMod = false;
		this.gv_line.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_line_RowCellStyle);
		this.gv_line.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_line_ShowingEditor);
		this.gv_line.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_line_FocusedRowObjectChanged);
		this.gv_line.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_line_CellValueChanged);
		this.gv_line.BeforeLeaveRow += new DevExpress.XtraGrid.Views.Base.RowAllowEventHandler(this.gv_line_BeforeLeaveRow);
		this.gv_line.ValidatingEditor += new DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventHandler(this.gv_line_ValidatingEditor);
		this.gv_line.InvalidValueException += new DevExpress.XtraEditors.Controls.InvalidValueExceptionEventHandler(this.gv_line_InvalidValueException);
		this.gridColumnEx7.Caption = "DocEntry";
		this.gridColumnEx7.CurrencyDateField = null;
		this.gridColumnEx7.CurrencyField = null;
		this.gridColumnEx7.CurrencyFixed = null;
		this.gridColumnEx7.CurrencyFixedRate = 0.0;
		this.gridColumnEx7.CurrencyRateField = null;
		this.gridColumnEx7.FieldName = "DocEntry";
		this.gridColumnEx7.Name = "gridColumnEx7";
		this.gridColumnEx7.OptionsColumn.ReadOnly = true;
		this.gridColumnEx7.PrimaryKey = true;
		this.gridColumnEx7.UnitField = null;
		this.gridColumnEx8.Caption = "LineNum";
		this.gridColumnEx8.CurrencyDateField = null;
		this.gridColumnEx8.CurrencyField = null;
		this.gridColumnEx8.CurrencyFixed = null;
		this.gridColumnEx8.CurrencyFixedRate = 0.0;
		this.gridColumnEx8.CurrencyRateField = null;
		this.gridColumnEx8.FieldName = "LineNum";
		this.gridColumnEx8.Name = "gridColumnEx8";
		this.gridColumnEx8.OptionsColumn.ReadOnly = true;
		this.gridColumnEx8.PrimaryKey = false;
		this.gridColumnEx8.UnitField = null;
		this.gridColumnEx9.Caption = "Item Code";
		this.gridColumnEx9.ColumnEdit = this.btn_ItemCode;
		this.gridColumnEx9.CurrencyDateField = null;
		this.gridColumnEx9.CurrencyField = null;
		this.gridColumnEx9.CurrencyFixed = null;
		this.gridColumnEx9.CurrencyFixedRate = 0.0;
		this.gridColumnEx9.CurrencyRateField = null;
		this.gridColumnEx9.FieldName = "ItemCode";
		this.gridColumnEx9.Name = "gridColumnEx9";
		this.gridColumnEx9.PrimaryKey = true;
		this.gridColumnEx9.UnitField = null;
		this.gridColumnEx9.Visible = true;
		this.gridColumnEx9.VisibleIndex = 0;
		this.btn_ItemCode.AutoHeight = false;
		this.btn_ItemCode.AutoLoadDataSource = true;
		editorButtonImageOptions2.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions8.Image");
		this.btn_ItemCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance5, appearanceHovered5, appearancePressed5, appearanceDisabled5, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance6, appearanceHovered6, appearancePressed6, appearanceDisabled6, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_ItemCode.ContainerControl = this;
		this.btn_ItemCode.DisplayMember = "ItemCode";
		this.btn_ItemCode.DisplayMember2 = "";
		this.btn_ItemCode.LoadDataSourceWhenEnter = false;
		this.btn_ItemCode.Name = "btn_ItemCode";
		this.btn_ItemCode.SeparateCharacterForMulti = ';';
		this.btn_ItemCode.SeparateString = ", ";
		this.btn_ItemCode.UsingAutoCompleteBox = false;
		this.btn_ItemCode.UsingDataSource = true;
		this.btn_ItemCode.ValidateWhenNoDataSource = false;
		this.btn_ItemCode.ValueMember = "ItemCode";
		this.btn_ItemCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btn_ItemCode_ButtonClick);
		this.gridColumnEx10.Caption = "Item Name";
		this.gridColumnEx10.CurrencyDateField = null;
		this.gridColumnEx10.CurrencyField = null;
		this.gridColumnEx10.CurrencyFixed = null;
		this.gridColumnEx10.CurrencyFixedRate = 0.0;
		this.gridColumnEx10.CurrencyRateField = null;
		this.gridColumnEx10.FieldName = "Dscription";
		this.gridColumnEx10.Name = "gridColumnEx10";
		this.gridColumnEx10.PrimaryKey = false;
		this.gridColumnEx10.UnitField = null;
		this.gridColumnEx10.Visible = true;
		this.gridColumnEx10.VisibleIndex = 1;
		this.gridColumnEx11.Caption = "WhsCode";
		this.gridColumnEx11.ColumnEdit = this.btn_WhsLine;
		this.gridColumnEx11.CurrencyDateField = null;
		this.gridColumnEx11.CurrencyField = null;
		this.gridColumnEx11.CurrencyFixed = null;
		this.gridColumnEx11.CurrencyFixedRate = 0.0;
		this.gridColumnEx11.CurrencyRateField = null;
		this.gridColumnEx11.FieldName = "WhsCode";
		this.gridColumnEx11.Name = "gridColumnEx11";
		this.gridColumnEx11.PrimaryKey = false;
		this.gridColumnEx11.UnitField = null;
		this.gridColumnEx11.Visible = true;
		this.gridColumnEx11.VisibleIndex = 2;
		this.btn_WhsLine.AutoHeight = false;
		this.btn_WhsLine.AutoLoadDataSource = true;
		editorButtonImageOptions3.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions10.Image");
		this.btn_WhsLine.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions5, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance7, appearanceHovered7, appearancePressed7, appearanceDisabled7, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance8, appearanceHovered8, appearancePressed8, appearanceDisabled8, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_WhsLine.ContainerControl = this;
		this.btn_WhsLine.DisplayMember = "WhsCode";
		this.btn_WhsLine.DisplayMember2 = "";
		this.btn_WhsLine.LoadDataSourceWhenEnter = false;
		this.btn_WhsLine.Name = "btn_WhsLine";
		this.btn_WhsLine.SeparateCharacterForMulti = ';';
		this.btn_WhsLine.SeparateString = ", ";
		this.btn_WhsLine.UsingAutoCompleteBox = false;
		this.btn_WhsLine.UsingDataSource = true;
		this.btn_WhsLine.ValidateWhenNoDataSource = false;
		this.btn_WhsLine.ValueMember = "WhsCode";
		this.gridColumnEx12.Caption = "Counting Quantity";
		this.gridColumnEx12.ColumnEdit = this.repositoryItemCalcEditEx1;
		this.gridColumnEx12.CurrencyDateField = null;
		this.gridColumnEx12.CurrencyField = null;
		this.gridColumnEx12.CurrencyFixed = null;
		this.gridColumnEx12.CurrencyFixedRate = 0.0;
		this.gridColumnEx12.CurrencyRateField = null;
		this.gridColumnEx12.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gridColumnEx12.FieldName = "Qty";
		this.gridColumnEx12.Name = "gridColumnEx12";
		this.gridColumnEx12.PrimaryKey = false;
		this.gridColumnEx12.UnitField = null;
		this.gridColumnEx12.Visible = true;
		this.gridColumnEx12.VisibleIndex = 3;
		this.gridColumnEx12.Width = 120;
		this.gridColumnEx13.Caption = "Remarks";
		this.gridColumnEx13.CurrencyDateField = null;
		this.gridColumnEx13.CurrencyField = null;
		this.gridColumnEx13.CurrencyFixed = null;
		this.gridColumnEx13.CurrencyFixedRate = 0.0;
		this.gridColumnEx13.CurrencyRateField = null;
		this.gridColumnEx13.FieldName = "Remarks";
		this.gridColumnEx13.Name = "gridColumnEx13";
		this.gridColumnEx13.PrimaryKey = false;
		this.gridColumnEx13.UnitField = null;
		this.gridColumnEx13.Visible = true;
		this.gridColumnEx13.VisibleIndex = 7;
		this.gridColumnEx19.Caption = "TranType";
		this.gridColumnEx19.CurrencyDateField = null;
		this.gridColumnEx19.CurrencyField = null;
		this.gridColumnEx19.CurrencyFixed = null;
		this.gridColumnEx19.CurrencyFixedRate = 0.0;
		this.gridColumnEx19.CurrencyRateField = null;
		this.gridColumnEx19.FieldName = "TranType";
		this.gridColumnEx19.Name = "gridColumnEx19";
		this.gridColumnEx19.OptionsColumn.ReadOnly = true;
		this.gridColumnEx19.PrimaryKey = false;
		this.gridColumnEx19.UnitField = null;
		this.gridColumnEx20.Caption = "UpgEntry";
		this.gridColumnEx20.CurrencyDateField = null;
		this.gridColumnEx20.CurrencyField = null;
		this.gridColumnEx20.CurrencyFixed = null;
		this.gridColumnEx20.CurrencyFixedRate = 0.0;
		this.gridColumnEx20.CurrencyRateField = null;
		this.gridColumnEx20.FieldName = "UgpEntry";
		this.gridColumnEx20.Name = "gridColumnEx20";
		this.gridColumnEx20.OptionsColumn.AllowEdit = false;
		this.gridColumnEx20.OptionsColumn.ReadOnly = true;
		this.gridColumnEx20.PrimaryKey = false;
		this.gridColumnEx20.UnitField = null;
		this.gridColumnEx21.Caption = "UoMEntry";
		this.gridColumnEx21.CurrencyDateField = null;
		this.gridColumnEx21.CurrencyField = null;
		this.gridColumnEx21.CurrencyFixed = null;
		this.gridColumnEx21.CurrencyFixedRate = 0.0;
		this.gridColumnEx21.CurrencyRateField = null;
		this.gridColumnEx21.FieldName = "UomEntry";
		this.gridColumnEx21.Name = "gridColumnEx21";
		this.gridColumnEx21.OptionsColumn.AllowEdit = false;
		this.gridColumnEx21.OptionsColumn.ReadOnly = true;
		this.gridColumnEx21.PrimaryKey = false;
		this.gridColumnEx21.UnitField = null;
		this.gridColumnEx22.Caption = "UoM Code";
		this.gridColumnEx22.ColumnEdit = this.btn_UomCode;
		this.gridColumnEx22.CurrencyDateField = null;
		this.gridColumnEx22.CurrencyField = null;
		this.gridColumnEx22.CurrencyFixed = null;
		this.gridColumnEx22.CurrencyFixedRate = 0.0;
		this.gridColumnEx22.CurrencyRateField = null;
		this.gridColumnEx22.FieldName = "UomCode";
		this.gridColumnEx22.Name = "gridColumnEx22";
		this.gridColumnEx22.PrimaryKey = false;
		this.gridColumnEx22.UnitField = null;
		this.gridColumnEx22.Visible = true;
		this.gridColumnEx22.VisibleIndex = 4;
		this.btn_UomCode.AutoHeight = false;
		this.btn_UomCode.AutoLoadDataSource = true;
		editorButtonImageOptions4.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions12.Image");
		this.btn_UomCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions6, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance9, appearanceHovered9, appearancePressed9, appearanceDisabled9, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, false, true, editorButtonImageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance10, appearanceHovered10, appearancePressed10, appearanceDisabled10, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_UomCode.ContainerControl = this;
		this.btn_UomCode.DisplayMember = "UomName";
		this.btn_UomCode.DisplayMember2 = "";
		this.btn_UomCode.LoadDataSourceWhenEnter = false;
		this.btn_UomCode.Name = "btn_UomCode";
		this.btn_UomCode.SeparateCharacterForMulti = ';';
		this.btn_UomCode.SeparateString = ", ";
		this.btn_UomCode.UsingAutoCompleteBox = false;
		this.btn_UomCode.UsingDataSource = true;
		this.btn_UomCode.ValidateWhenNoDataSource = false;
		this.btn_UomCode.ValueMember = "UomCode";
		this.gridColumnEx23.Caption = "Item Per Unit";
		this.gridColumnEx23.ColumnEdit = this.repositoryItemCalcEditEx2;
		this.gridColumnEx23.CurrencyDateField = null;
		this.gridColumnEx23.CurrencyField = null;
		this.gridColumnEx23.CurrencyFixed = null;
		this.gridColumnEx23.CurrencyFixedRate = 0.0;
		this.gridColumnEx23.CurrencyRateField = null;
		this.gridColumnEx23.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Rate;
		this.gridColumnEx23.FieldName = "NumPerMsr";
		this.gridColumnEx23.Name = "gridColumnEx23";
		this.gridColumnEx23.PrimaryKey = false;
		this.gridColumnEx23.UnitField = null;
		this.gridColumnEx23.Visible = true;
		this.gridColumnEx23.VisibleIndex = 5;
		this.gridColumnEx23.Width = 88;
		this.gridColumnEx25.Caption = "Inventory Quantity";
		this.gridColumnEx25.ColumnEdit = this.repositoryItemCalcEditEx3;
		this.gridColumnEx25.CurrencyDateField = null;
		this.gridColumnEx25.CurrencyField = null;
		this.gridColumnEx25.CurrencyFixed = null;
		this.gridColumnEx25.CurrencyFixedRate = 0.0;
		this.gridColumnEx25.CurrencyRateField = null;
		this.gridColumnEx25.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gridColumnEx25.FieldName = "InvQty";
		this.gridColumnEx25.Name = "gridColumnEx25";
		this.gridColumnEx25.OptionsColumn.ReadOnly = true;
		this.gridColumnEx25.PrimaryKey = false;
		this.gridColumnEx25.UnitField = null;
		this.gridColumnEx25.Visible = true;
		this.gridColumnEx25.VisibleIndex = 6;
		this.gridColumnEx25.Width = 115;
		this.gridColumnEx26.Caption = "gridColumnEx26";
		this.gridColumnEx26.CurrencyDateField = null;
		this.gridColumnEx26.CurrencyField = null;
		this.gridColumnEx26.CurrencyFixed = null;
		this.gridColumnEx26.CurrencyFixedRate = 0.0;
		this.gridColumnEx26.CurrencyRateField = null;
		this.gridColumnEx26.FieldName = "BinAbs";
		this.gridColumnEx26.Name = "gridColumnEx26";
		this.gridColumnEx26.OptionsColumn.ReadOnly = true;
		this.gridColumnEx26.PrimaryKey = false;
		this.gridColumnEx26.UnitField = null;
		this.gridColumnEx27.Caption = "Bin Code";
		this.gridColumnEx27.ColumnEdit = this.btn_BinItem;
		this.gridColumnEx27.CurrencyDateField = null;
		this.gridColumnEx27.CurrencyField = null;
		this.gridColumnEx27.CurrencyFixed = null;
		this.gridColumnEx27.CurrencyFixedRate = 0.0;
		this.gridColumnEx27.CurrencyRateField = null;
		this.gridColumnEx27.FieldName = "BinCode";
		this.gridColumnEx27.Name = "gridColumnEx27";
		this.gridColumnEx27.PrimaryKey = false;
		this.gridColumnEx27.UnitField = null;
		this.gridColumnEx27.Visible = true;
		this.gridColumnEx27.VisibleIndex = 8;
		this.btn_BinItem.AutoHeight = false;
		this.btn_BinItem.AutoLoadDataSource = true;
		editorButtonImageOptions5.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions14.Image");
		this.btn_BinItem.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions7, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance11, appearanceHovered11, appearancePressed11, appearanceDisabled11, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions5, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance12, appearanceHovered12, appearancePressed12, appearanceDisabled12, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_BinItem.ContainerControl = this;
		this.btn_BinItem.DisplayMember = "";
		this.btn_BinItem.DisplayMember2 = "";
		this.btn_BinItem.LoadDataSourceWhenEnter = false;
		this.btn_BinItem.Name = "btn_BinItem";
		this.btn_BinItem.SeparateCharacterForMulti = ';';
		this.btn_BinItem.SeparateString = ", ";
		this.btn_BinItem.UsingAutoCompleteBox = false;
		this.btn_BinItem.UsingDataSource = true;
		this.btn_BinItem.ValidateWhenNoDataSource = false;
		this.btn_BinItem.ValueMember = "";
		this.xtraTabControl2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.xtraTabControl2.IsSearchControl = false;
		this.xtraTabControl2.Location = new System.Drawing.Point(0, 0);
		this.xtraTabControl2.Name = "xtraTabControl2";
		this.xtraTabControl2.SelectedTabPage = this.xtra_Batch;
		this.xtraTabControl2.Size = new System.Drawing.Size(844, 134);
		this.xtraTabControl2.TabIndex = 1;
		this.xtraTabControl2.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[1] { this.xtra_Batch });
		this.xtra_Batch.Controls.Add(this.grd_batch);
		this.xtra_Batch.EditControl = null;
		this.xtra_Batch.EditValue = "Batches / Serials Number";
		this.xtra_Batch.FieldName = null;
		this.xtra_Batch.IsSearchControl = false;
		this.xtra_Batch.Name = "xtra_Batch";
		this.xtra_Batch.ReadOnly = false;
		this.xtra_Batch.SettingInVisible = false;
		this.xtra_Batch.SettingReadonly = false;
		this.xtra_Batch.Size = new System.Drawing.Size(842, 109);
		this.xtra_Batch.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtra_Batch.Text = "Batches / Serials Number";
		this.grd_batch.AllowPasteExcel = true;
		this.grd_batch.ClipboardData = null;
		this.grd_batch.ContextMenuStrip = this.ctm_Batch;
		this.grd_batch.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_batch.Location = new System.Drawing.Point(0, 0);
		this.grd_batch.MainView = this.gv_batch;
		this.grd_batch.Name = "grd_batch";
		this.grd_batch.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.btn_DisNumber, this.repositoryItemButtonEditCFL1, this.btn_BinCode });
		this.grd_batch.Size = new System.Drawing.Size(842, 109);
		this.grd_batch.TabIndex = 0;
		this.grd_batch.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_batch });
		this.ctm_Batch.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addRowToolStripMenuItem1, this.deleteRowToolStripMenuItem1 });
		this.ctm_Batch.Name = "ctm_Batch";
		this.ctm_Batch.Size = new System.Drawing.Size(134, 48);
		this.addRowToolStripMenuItem1.Name = "addRowToolStripMenuItem1";
		this.addRowToolStripMenuItem1.Size = new System.Drawing.Size(133, 22);
		this.addRowToolStripMenuItem1.Text = "Add Row";
		this.addRowToolStripMenuItem1.Click += new System.EventHandler(this.addRowToolStripMenuItem1_Click);
		this.deleteRowToolStripMenuItem1.Name = "deleteRowToolStripMenuItem1";
		this.deleteRowToolStripMenuItem1.Size = new System.Drawing.Size(133, 22);
		this.deleteRowToolStripMenuItem1.Text = "Delete Row";
		this.deleteRowToolStripMenuItem1.Click += new System.EventHandler(this.deleteRowToolStripMenuItem1_Click);
		this.gv_batch.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_batch.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_batch.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_batch.ColumnKey = null;
		this.gv_batch.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[11]
		{
			this.gridColumnEx14, this.gridColumnEx15, this.gridColumnEx16, this.gridColumnEx17, this.gridColumnEx18, this.gb_DistNumber, this.gb_itemCode, this.gb_BinAbs, this.gb_BinCode, this.gb_Qty,
			this.gridColumnEx24
		});
		this.gv_batch.DisableFormSetting = false;
		this.gv_batch.DisplayException = false;
		this.gv_batch.EditColumn = null;
		this.gv_batch.FieldCurrency = null;
		this.gv_batch.FieldRate = null;
		this.gv_batch.FormId = "oInventoryCounting";
		this.gv_batch.GridControl = this.grd_batch;
		this.gv_batch.IgnoreFilter = false;
		this.gv_batch.IndexEditRow = -1;
		this.gv_batch.IndicatorWidth = 35;
		this.gv_batch.IsApzTable = false;
		this.gv_batch.IsInsertNewRow = false;
		this.gv_batch.IsLoadingData = false;
		this.gv_batch.IsMappingContextAction = false;
		this.gv_batch.ItemId = "APZ_INC2";
		this.gv_batch.LstColumnAllowBatch = null;
		this.gv_batch.LstColumnNotAllowBatch = null;
		this.gv_batch.LstColumnNotEditable = null;
		this.gv_batch.LstColumNotSetting = null;
		this.gv_batch.Name = "gv_batch";
		this.gv_batch.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_batch.OptionsView.BestFitMaxRowCount = 100;
		this.gv_batch.OptionsView.ColumnAutoWidth = false;
		this.gv_batch.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_batch.OptionsView.ShowGroupPanel = false;
		this.gv_batch.UseBatchMod = false;
		this.gv_batch.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_batch_RowCellStyle);
		this.gv_batch.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_batch_ShowingEditor);
		this.gv_batch.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_batch_CellValueChanged);
		this.gv_batch.ValidatingEditor += new DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventHandler(this.gv_batch_ValidatingEditor);
		this.gv_batch.InvalidValueException += new DevExpress.XtraEditors.Controls.InvalidValueExceptionEventHandler(this.gv_batch_InvalidValueException);
		this.gridColumnEx14.Caption = "DocEntry";
		this.gridColumnEx14.CurrencyDateField = null;
		this.gridColumnEx14.CurrencyField = null;
		this.gridColumnEx14.CurrencyFixed = null;
		this.gridColumnEx14.CurrencyFixedRate = 0.0;
		this.gridColumnEx14.CurrencyRateField = null;
		this.gridColumnEx14.FieldName = "DocEntry";
		this.gridColumnEx14.Name = "gridColumnEx14";
		this.gridColumnEx14.OptionsColumn.ReadOnly = true;
		this.gridColumnEx14.PrimaryKey = false;
		this.gridColumnEx14.UnitField = null;
		this.gridColumnEx15.Caption = "BaseLine";
		this.gridColumnEx15.CurrencyDateField = null;
		this.gridColumnEx15.CurrencyField = null;
		this.gridColumnEx15.CurrencyFixed = null;
		this.gridColumnEx15.CurrencyFixedRate = 0.0;
		this.gridColumnEx15.CurrencyRateField = null;
		this.gridColumnEx15.FieldName = "BaseLine";
		this.gridColumnEx15.Name = "gridColumnEx15";
		this.gridColumnEx15.OptionsColumn.ReadOnly = true;
		this.gridColumnEx15.PrimaryKey = false;
		this.gridColumnEx15.UnitField = null;
		this.gridColumnEx16.Caption = "LineId";
		this.gridColumnEx16.CurrencyDateField = null;
		this.gridColumnEx16.CurrencyField = null;
		this.gridColumnEx16.CurrencyFixed = null;
		this.gridColumnEx16.CurrencyFixedRate = 0.0;
		this.gridColumnEx16.CurrencyRateField = null;
		this.gridColumnEx16.FieldName = "LineId";
		this.gridColumnEx16.Name = "gridColumnEx16";
		this.gridColumnEx16.OptionsColumn.ReadOnly = true;
		this.gridColumnEx16.PrimaryKey = false;
		this.gridColumnEx16.UnitField = null;
		this.gridColumnEx17.Caption = "MdAbsEntry";
		this.gridColumnEx17.CurrencyDateField = null;
		this.gridColumnEx17.CurrencyField = null;
		this.gridColumnEx17.CurrencyFixed = null;
		this.gridColumnEx17.CurrencyFixedRate = 0.0;
		this.gridColumnEx17.CurrencyRateField = null;
		this.gridColumnEx17.FieldName = "MdAbsEntry";
		this.gridColumnEx17.Name = "gridColumnEx17";
		this.gridColumnEx17.OptionsColumn.ReadOnly = true;
		this.gridColumnEx17.PrimaryKey = false;
		this.gridColumnEx17.UnitField = null;
		this.gridColumnEx18.Caption = "SysNumber";
		this.gridColumnEx18.CurrencyDateField = null;
		this.gridColumnEx18.CurrencyField = null;
		this.gridColumnEx18.CurrencyFixed = null;
		this.gridColumnEx18.CurrencyFixedRate = 0.0;
		this.gridColumnEx18.CurrencyRateField = null;
		this.gridColumnEx18.FieldName = "SysNumber";
		this.gridColumnEx18.Name = "gridColumnEx18";
		this.gridColumnEx18.OptionsColumn.ReadOnly = true;
		this.gridColumnEx18.PrimaryKey = false;
		this.gridColumnEx18.UnitField = null;
		this.gb_DistNumber.Caption = "Dist Number";
		this.gb_DistNumber.ColumnEdit = this.btn_DisNumber;
		this.gb_DistNumber.CurrencyDateField = null;
		this.gb_DistNumber.CurrencyField = null;
		this.gb_DistNumber.CurrencyFixed = null;
		this.gb_DistNumber.CurrencyFixedRate = 0.0;
		this.gb_DistNumber.CurrencyRateField = null;
		this.gb_DistNumber.FieldName = "DistNumber";
		this.gb_DistNumber.Name = "gb_DistNumber";
		this.gb_DistNumber.PrimaryKey = false;
		this.gb_DistNumber.UnitField = null;
		this.gb_DistNumber.Visible = true;
		this.gb_DistNumber.VisibleIndex = 0;
		this.gb_DistNumber.Width = 120;
		this.btn_DisNumber.AutoHeight = false;
		this.btn_DisNumber.AutoLoadDataSource = true;
		editorButtonImageOptions6.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions16.Image");
		this.btn_DisNumber.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions8, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance13, appearanceHovered13, appearancePressed13, appearanceDisabled13, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions6, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance14, appearanceHovered14, appearancePressed14, appearanceDisabled14, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_DisNumber.ContainerControl = this;
		this.btn_DisNumber.DisplayMember = "DistNumber";
		this.btn_DisNumber.DisplayMember2 = "";
		this.btn_DisNumber.LoadDataSourceWhenEnter = false;
		this.btn_DisNumber.Name = "btn_DisNumber";
		this.btn_DisNumber.SeparateCharacterForMulti = ';';
		this.btn_DisNumber.SeparateString = ", ";
		this.btn_DisNumber.UsingAutoCompleteBox = false;
		this.btn_DisNumber.UsingDataSource = true;
		this.btn_DisNumber.ValidateWhenNoDataSource = false;
		this.btn_DisNumber.ValueMember = "DistNumber";
		this.gb_itemCode.Caption = "ItemCode";
		this.gb_itemCode.CurrencyDateField = null;
		this.gb_itemCode.CurrencyField = null;
		this.gb_itemCode.CurrencyFixed = null;
		this.gb_itemCode.CurrencyFixedRate = 0.0;
		this.gb_itemCode.CurrencyRateField = null;
		this.gb_itemCode.FieldName = "ItemCode";
		this.gb_itemCode.Name = "gb_itemCode";
		this.gb_itemCode.OptionsColumn.ReadOnly = true;
		this.gb_itemCode.PrimaryKey = false;
		this.gb_itemCode.UnitField = null;
		this.gb_BinAbs.Caption = "BinAbs";
		this.gb_BinAbs.ColumnEdit = this.repositoryItemButtonEditCFL1;
		this.gb_BinAbs.CurrencyDateField = null;
		this.gb_BinAbs.CurrencyField = null;
		this.gb_BinAbs.CurrencyFixed = null;
		this.gb_BinAbs.CurrencyFixedRate = 0.0;
		this.gb_BinAbs.CurrencyRateField = null;
		this.gb_BinAbs.FieldName = "BinAbs";
		this.gb_BinAbs.Name = "gb_BinAbs";
		this.gb_BinAbs.OptionsColumn.ReadOnly = true;
		this.gb_BinAbs.PrimaryKey = false;
		this.gb_BinAbs.UnitField = null;
		this.repositoryItemButtonEditCFL1.AutoHeight = false;
		this.repositoryItemButtonEditCFL1.AutoLoadDataSource = true;
		this.repositoryItemButtonEditCFL1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, false, false, imageOptions9, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance15, appearanceHovered15, appearancePressed15, appearanceDisabled15, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, imageOptions10, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance16, appearanceHovered16, appearancePressed16, appearanceDisabled16, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.repositoryItemButtonEditCFL1.ContainerControl = this;
		this.repositoryItemButtonEditCFL1.DisplayMember = "";
		this.repositoryItemButtonEditCFL1.DisplayMember2 = "";
		this.repositoryItemButtonEditCFL1.LoadDataSourceWhenEnter = false;
		this.repositoryItemButtonEditCFL1.Name = "repositoryItemButtonEditCFL1";
		this.repositoryItemButtonEditCFL1.ReadOnly = true;
		this.repositoryItemButtonEditCFL1.SeparateCharacterForMulti = ';';
		this.repositoryItemButtonEditCFL1.SeparateString = ", ";
		this.repositoryItemButtonEditCFL1.UsingAutoCompleteBox = false;
		this.repositoryItemButtonEditCFL1.UsingDataSource = true;
		this.repositoryItemButtonEditCFL1.ValidateWhenNoDataSource = false;
		this.repositoryItemButtonEditCFL1.ValueMember = "";
		this.gb_BinCode.Caption = "Bin Code";
		this.gb_BinCode.ColumnEdit = this.btn_BinCode;
		this.gb_BinCode.CurrencyDateField = null;
		this.gb_BinCode.CurrencyField = null;
		this.gb_BinCode.CurrencyFixed = null;
		this.gb_BinCode.CurrencyFixedRate = 0.0;
		this.gb_BinCode.CurrencyRateField = null;
		this.gb_BinCode.FieldName = "BinCode";
		this.gb_BinCode.Name = "gb_BinCode";
		this.gb_BinCode.PrimaryKey = false;
		this.gb_BinCode.UnitField = null;
		this.gb_BinCode.Visible = true;
		this.gb_BinCode.VisibleIndex = 2;
		this.btn_BinCode.AutoHeight = false;
		this.btn_BinCode.AutoLoadDataSource = true;
		this.btn_BinCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions11, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance17, appearanceHovered17, appearancePressed17, appearanceDisabled17, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_BinCode.ContainerControl = this;
		this.btn_BinCode.DisplayMember = "BinCode";
		this.btn_BinCode.DisplayMember2 = "";
		this.btn_BinCode.LoadDataSourceWhenEnter = false;
		this.btn_BinCode.Name = "btn_BinCode";
		this.btn_BinCode.SeparateCharacterForMulti = ';';
		this.btn_BinCode.SeparateString = ", ";
		this.btn_BinCode.UsingAutoCompleteBox = false;
		this.btn_BinCode.UsingDataSource = true;
		this.btn_BinCode.ValidateWhenNoDataSource = false;
		this.btn_BinCode.ValueMember = "BinCode";
		this.gb_Qty.Caption = "Quantity";
		this.gb_Qty.ColumnEdit = this.repositoryItemCalcEditEx4;
		this.gb_Qty.CurrencyDateField = null;
		this.gb_Qty.CurrencyField = null;
		this.gb_Qty.CurrencyFixed = null;
		this.gb_Qty.CurrencyFixedRate = 0.0;
		this.gb_Qty.CurrencyRateField = null;
		this.gb_Qty.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gb_Qty.FieldName = "Qty";
		this.gb_Qty.Name = "gb_Qty";
		this.gb_Qty.PrimaryKey = false;
		this.gb_Qty.UnitField = null;
		this.gb_Qty.Visible = true;
		this.gb_Qty.VisibleIndex = 1;
		this.gridColumnEx24.Caption = "IsBatch";
		this.gridColumnEx24.CurrencyDateField = null;
		this.gridColumnEx24.CurrencyField = null;
		this.gridColumnEx24.CurrencyFixed = null;
		this.gridColumnEx24.CurrencyFixedRate = 0.0;
		this.gridColumnEx24.CurrencyRateField = null;
		this.gridColumnEx24.FieldName = "IsBatch";
		this.gridColumnEx24.Name = "gridColumnEx24";
		this.gridColumnEx24.OptionsColumn.ReadOnly = true;
		this.gridColumnEx24.PrimaryKey = false;
		this.gridColumnEx24.UnitField = null;
		this.btn_Whs.EditControl = null;
		this.btn_Whs.FieldName = null;
		this.btn_Whs.IsSearchControl = false;
		this.btn_Whs.Location = new System.Drawing.Point(315, 1);
		this.btn_Whs.Name = "btn_Whs";
		this.btn_Whs.Properties.AutoLoadDataSource = true;
		this.btn_Whs.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions12, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance18, appearanceHovered18, appearancePressed18, appearanceDisabled18, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, imageOptions13, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance19, appearanceHovered19, appearancePressed19, appearanceDisabled19, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_Whs.Properties.ContainerControl = null;
		this.btn_Whs.Properties.DisplayMember = "WhsCode";
		this.btn_Whs.Properties.DisplayMember2 = "";
		this.btn_Whs.Properties.LoadDataSourceWhenEnter = false;
		this.btn_Whs.Properties.SeparateCharacterForMulti = ';';
		this.btn_Whs.Properties.SeparateString = ", ";
		this.btn_Whs.Properties.UsingAutoCompleteBox = false;
		this.btn_Whs.Properties.UsingDataSource = true;
		this.btn_Whs.Properties.ValidateWhenNoDataSource = false;
		this.btn_Whs.Properties.ValueMember = "WhsCode";
		this.btn_Whs.SettingInVisible = false;
		this.btn_Whs.SettingReadonly = false;
		this.btn_Whs.Size = new System.Drawing.Size(140, 22);
		this.btn_Whs.TabIndex = 82;
		this.luk_Status.EditControl = null;
		this.luk_Status.FieldName = null;
		this.luk_Status.IsSearchControl = false;
		this.luk_Status.Location = new System.Drawing.Point(514, 21);
		this.luk_Status.Name = "luk_Status";
		this.luk_Status.Properties.AutoHeight = false;
		this.luk_Status.Properties.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_Status.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Status.Properties.IsSearchControl = false;
		this.luk_Status.Properties.NullText = "";
		this.luk_Status.Properties.ShowFooter = false;
		this.luk_Status.Properties.ShowHeader = false;
		this.luk_Status.SettingInVisible = false;
		this.luk_Status.SettingReadonly = false;
		this.luk_Status.Size = new System.Drawing.Size(100, 20);
		this.luk_Status.TabIndex = 83;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(850, 544);
		base.Controls.Add(this.luk_Status);
		base.Controls.Add(this.btn_Whs);
		base.Controls.Add(this.btn_Ok);
		base.Controls.Add(this.lb_DocStatus);
		base.Controls.Add(this.lb_Whs);
		base.Controls.Add(this.lb_DocNo);
		base.Controls.Add(this.lb_ToDate);
		base.Controls.Add(this.lb_FromDate);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.btn_Search);
		base.Controls.Add(this.txt_No);
		base.Controls.Add(this.txtToDate);
		base.Controls.Add(this.txtFromDate);
		base.Controls.Add(this.split_Main);
		base.FormId = "oInventoryCounting";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_InventoryCounting";
		this.Text = "Inventory Counting";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_InventoryCounting_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_No.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtToDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtToDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtFromDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtFromDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.split_Main).EndInit();
		this.split_Main.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		this.ctm_Doc.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_RequestNo).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_WhsDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_StatusDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_CountType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Taker1Type).EndInit();
		((System.ComponentModel.ISupportInitialize)this.split_LineBatchAndSerial).EndInit();
		this.split_LineBatchAndSerial.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).EndInit();
		this.xtraTabControl1.ResumeLayout(false);
		this.xtra_Item.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_line).EndInit();
		this.ctm_Line.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_line).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_ItemCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_WhsLine).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_UomCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_BinItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl2).EndInit();
		this.xtraTabControl2.ResumeLayout(false);
		this.xtra_Batch.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_batch).EndInit();
		this.ctm_Batch.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_batch).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_DisNumber).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemButtonEditCFL1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_BinCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_Whs.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Status.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
