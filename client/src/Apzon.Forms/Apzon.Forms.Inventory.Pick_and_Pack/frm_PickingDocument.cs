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
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Forms.Properties;
using Apzon.Forms.UserControls;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Menu;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTab;

namespace Apzon.Forms.Inventory.Pick_and_Pack;

public class frm_PickingDocument : XtraFormEx
{
	private readonly DataTable _dtRequest = new DataTable();

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient(ApzObjectTypes.oOrders);

	private DataTable _tableSearch;

	protected DataTable BatchData;

	protected DataTable ConfimedData;

	protected List<GridColumnEx> ReadOnlyBatchCols = new List<GridColumnEx>();

	protected List<GridColumnEx> ReadOnlyConfirmCols = new List<GridColumnEx>();

	protected List<GridColumnEx> ReadOnlyLineCols = new List<GridColumnEx>();

	protected string TableBatch = "APZ_PKL3";

	protected string TableDoc = "APZ_OPKL";

	protected string TableLine = "APZ_PKL1";

	protected string TableBatchConfirm = "APZ_PKL2";

	private IContainer components;

	private SplitContainerControl split_main;

	private SplitContainerControl split_Picked;

	private GridControlEx grd_Doc;

	private GridViewEx gv_Doc;

	private GridControlEx grd_Line;

	private GridViewEx gv_Line;

	private GridControlEx grd_Batch;

	private GridViewEx gv_Batch;

	private SimpleButtonEx btn_Cancel;

	private PanelControl panelControl1;

	private PanelControl panelControl2;

	private DateEditEx date_ToDate;

	private LabelLine labelLine1;

	private DateEditEx date_FromDate;

	private LabelLine labelLine2;

	private ButtonEditEx txt_BpCode;

	private LabelLine labelLine4;

	private LabelLine labelLine3;

	private SimpleButtonEx btn_Search;

	private TextEditEx txt_DocNo;

	private LabelLine labelLine6;

	private SimpleButtonEx btn_OK;

	public DropDownButtonEx drlCopyTo;

	private GridColumnEx gridColumnEx1;

	private GridColumnEx gridColumnEx2;

	private GridColumnEx gridColumnEx3;

	private GridColumnEx gridColumnEx4;

	private GridColumnEx gridColumnEx6;

	private RepositoryItemLookUpEdit rep_Status;

	private GridColumnEx gridColumnEx5;

	private GridColumnEx gridColumnEx8;

	private GridColumnEx gridColumnEx9;

	private GridColumnEx gridColumnEx10;

	private GridColumnEx gridColumnEx11;

	private GridColumnEx gridColumnEx12;

	private GridColumnEx gridColumnEx13;

	private GridColumnEx gridColumnEx14;

	private GridColumnEx gridColumnEx17;

	private GridColumnEx gridColumnEx18;

	private GridColumnEx gridColumnEx19;

	private GridColumnEx gridColumnEx20;

	private GridColumnEx gridColumnEx21;

	private GridColumnEx gridColumnEx22;

	private GridColumnEx gridColumnEx23;

	private GridColumnEx gridColumnEx24;

	private GridColumnEx gridColumnEx25;

	private RepositoryItemButtonEdit rep_BatchSerial;

	private RepositoryItemButtonEdit rep_ItemCode;

	private RepositoryItemButtonEdit rep_LineItem;

	private RepositoryItemButtonEdit rep_Whs;

	private RepositoryItemButtonEdit rep_Customer;

	private LookUpEdit look_Status;

	private GridColumnEx gridColumnEx7;

	private GridColumnEx gridColumnEx15;

	private RepositoryItemLookUpEdit rep_ManBatchSerial;

	private ContextMenuStrip ctx_Line;

	private ToolStripMenuItem removeRowToolStripMenuItem;

	private ContextMenuStrip ctx_BatchSerial;

	private ToolStripMenuItem removeRowToolStripMenuItem1;

	private RepositoryItemCheckEdit repositoryItemCheckEdit1;

	private RepositoryItemButtonEditCFL rep_Owner;

	private GridColumnEx gridColumnEx16;

	private RepositoryItemButtonEdit rep_Order;

	private GridColumnEx gridColumnEx26;

	private GridColumnEx gridColumnEx27;

	private RepositoryItemLookUpEdit rep_BatchPickStatus;

	private RepositoryItemLookUpEdit rep_LineStatus;

	private GridColumnEx gridColumnEx28;

	private SimpleButtonEx btn_AutoSelect;

	private GridColumnEx gridColumnEx29;

	private GridColumnEx gridColumnEx30;

	private GridColumnEx gridColumnEx31;

	private GridColumnEx gridColumnEx32;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem cancelToolStripMenuItem;

	private XtraTabControlEx xtraTabControl1;

	private XtraTabPageEx xtraTabPage2;

	private GridControlEx grd_Confirmed;

	private GridViewEx gv_Confirmed;

	private GridColumnEx gridColumnEx33;

	private RepositoryItemButtonEdit rep_ConfirmedBatch;

	private GridColumnEx gridColumnEx34;

	private RepositoryItemButtonEdit rep_ConfirmedItemCode;

	private GridColumnEx gridColumnEx35;

	private GridColumnEx gridColumnEx36;

	private GridColumnEx gridColumnEx37;

	private GridColumnEx gridColumnEx38;

	private GridColumnEx gridColumnEx39;

	private GridColumnEx gridColumnEx40;

	private GridColumnEx gridColumnEx41;

	private GridColumnEx gridColumnEx42;

	private RepositoryItemCheckEdit rep_ConfirmedChecked;

	private GridColumnEx gridColumnEx43;

	private RepositoryItemLookUpEdit rep_ConfirmedStatus;

	private GridColumnEx gridColumnEx44;

	private GridColumnEx gridColumnEx45;

	private GridColumnEx gridColumnEx46;

	private XtraTabPageEx xtraTabPage1;

	private ContextMenuStrip contextMenuStrip2;

	private ToolStripMenuItem toolStripMenuItem1;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx1;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx2;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx3;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx4;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx5;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx6;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx7;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx8;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx9;

	public List<ApzObjectTypes> PopCopyFrom { get; set; }

	public List<ApzObjectTypes> PopCopyTo { get; set; }

	private DataTable StructureDoc { get; set; }

	private DataTable StructureLine { get; set; }

	private DataTable StructureBatch { get; set; }

	public frm_PickingDocument()
	{
		InitializeComponent();
	}

	public frm_PickingDocument(DataTable dtrequest = null)
	{
		InitializeComponent();
		if (dtrequest.IsNotNull())
		{
			_dtRequest = dtrequest;
		}
	}

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		try
		{
			if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
			{
				WinFuntions.DisableAllControl(Controls);
				btn_Cancel.Enabled = true;
				return;
			}
			_formMode = value;
			switch (_formMode)
			{
			case Enums.FormViewModeType.Add:
			{
				SimpleButtonEx simpleButtonEx7 = btn_OK;
				SimpleButtonEx simpleButtonEx8 = btn_Cancel;
				bool flag = (btn_Search.Enabled = true);
				bool enabled = (simpleButtonEx8.Enabled = flag);
				simpleButtonEx7.Enabled = enabled;
				btn_OK.Text = Apzon.Commons.Properties.Resources.ButtonAdd;
				drlCopyTo.Enabled = false;
				break;
			}
			case Enums.FormViewModeType.Update:
			{
				SimpleButtonEx simpleButtonEx5 = btn_OK;
				SimpleButtonEx simpleButtonEx6 = btn_Cancel;
				bool flag = (btn_Search.Enabled = true);
				bool enabled = (simpleButtonEx6.Enabled = flag);
				simpleButtonEx5.Enabled = enabled;
				btn_OK.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
				drlCopyTo.Enabled = true;
				break;
			}
			case Enums.FormViewModeType.View:
			{
				SimpleButtonEx simpleButtonEx3 = btn_OK;
				SimpleButtonEx simpleButtonEx4 = btn_Cancel;
				bool flag = (btn_Search.Enabled = true);
				bool enabled = (simpleButtonEx4.Enabled = flag);
				simpleButtonEx3.Enabled = enabled;
				btn_OK.Text = Apzon.Commons.Properties.Resources.ButtonOK;
				drlCopyTo.Enabled = true;
				break;
			}
			case Enums.FormViewModeType.Busy:
			{
				SimpleButtonEx simpleButtonEx = btn_OK;
				SimpleButtonEx simpleButtonEx2 = btn_Cancel;
				bool flag = (btn_Search.Enabled = false);
				bool enabled = (simpleButtonEx2.Enabled = flag);
				simpleButtonEx.Enabled = enabled;
				drlCopyTo.Enabled = false;
				break;
			}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task frm_PickingDocument_Load(object sender, EventArgs e)
	{
		_ = 4;
		try
		{
			FormObjectType = ApzObjectTypes.oPickLists;
			GridViewEx gridViewEx = gv_Batch;
			GridViewEx gridViewEx2 = gv_Line;
			GridViewEx gridViewEx3 = gv_Doc;
			GridViewEx gridViewEx4 = gv_Confirmed;
			string text = (FormId = FormObjectType.ToString());
			string text3 = (gridViewEx4.FormId = text);
			string text5 = (gridViewEx3.FormId = text3);
			string formId = (gridViewEx2.FormId = text5);
			gridViewEx.FormId = formId;
			gv_Doc.ItemId = TableDoc;
			gv_Line.ItemId = TableLine;
			gv_Batch.ItemId = TableBatch;
			gv_Confirmed.ItemId = TableBatchConfirm;
			Dictionary<string, GridViewEx> dictionary = new Dictionary<string, GridViewEx>();
			dictionary[TableDoc] = gv_Doc;
			dictionary[TableLine] = gv_Line;
			dictionary[TableBatch] = gv_Batch;
			dictionary[TableBatchConfirm] = gv_Confirmed;
			await Task.WhenAll(WinFuntions.LoadUdfColumns(gv_Doc, TableDoc, dictionary), WinFuntions.LoadUdfColumns(gv_Line, TableLine, dictionary), WinFuntions.LoadUdfColumns(gv_Batch, TableBatch, dictionary));
			PopCopyFrom = new List<ApzObjectTypes> { ApzObjectTypes.oOrders };
			PopCopyTo = new List<ApzObjectTypes>
			{
				ApzObjectTypes.oPacking,
				ApzObjectTypes.oShipping,
				ApzObjectTypes.oDeliveryNotes,
				ApzObjectTypes.oInvoices
			};
			if (PopCopyTo != null && PopCopyTo.Count > 0)
			{
				DXPopupMenu dXPopupMenu = new DXPopupMenu();
				foreach (ApzObjectTypes item in PopCopyTo)
				{
					DXMenuItem dXMenuItem = new DXMenuItem();
					switch (item)
					{
					case ApzObjectTypes.oPacking:
						dXMenuItem.Caption = MenuCaption.Packing;
						break;
					case ApzObjectTypes.oShipping:
						dXMenuItem.Caption = MenuCaption.ShipDoc;
						break;
					case ApzObjectTypes.oDeliveryNotes:
						dXMenuItem.Caption = MenuCaption.Delivery;
						break;
					case ApzObjectTypes.oInvoices:
						dXMenuItem.Caption = MenuCaption.ARInvoices;
						break;
					}
					ApzObjectTypes objtype = item;
					dXMenuItem.Click += (object s, EventArgs events) =>
					{
						try
						{
							CopyToShipping(FormObjectType, objtype, Function.ParseInt(gv_Doc.GetFocusedRowCellValue("AbsEntry")));
						}
						catch (Exception ex2)
						{
							Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
							WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex2.Message);
						}
					};
					dXPopupMenu.Items.Add(dXMenuItem);
				}
				drlCopyTo.DropDownControl = dXPopupMenu;
			}
			Dictionary<string, string> dic = new Dictionary<string, string>
			{
				{ "O", "Open" },
				{ "L", "Closed" },
				{ "C", "Canceled" },
				{ "P", "Confirmed" },
				{ "", "All" }
			};
			rep_Status.SetDataSource(dic);
			look_Status.SetDataSource(dic);
			Dictionary<string, string> dic2 = new Dictionary<string, string>
			{
				{ "O", "Open" },
				{ "C", "Closed" }
			};
			rep_BatchPickStatus.SetDataSource(dic2);
			rep_LineStatus.SetDataSource(dic2);
			rep_ConfirmedStatus.SetDataSource(dic2);
			rep_ManBatchSerial.SetDataSource(new Dictionary<string, string>
			{
				{ "B", "Batches" },
				{ "S", "Serial Numbers" },
				{ "N", "None" }
			});
			split_main.PanelVisibility = SplitPanelVisibility.Panel1;
			_tableSearch = new DataTable();
			_tableSearch.Columns.Add(new DataColumn("docentry", typeof(string)));
			_tableSearch.Columns.Add(new DataColumn("whs", typeof(string)));
			_tableSearch.Columns.Add(new DataColumn("fromDate", typeof(DateTime)));
			_tableSearch.Columns.Add(new DataColumn("toDate", typeof(DateTime)));
			_tableSearch.Columns.Add(new DataColumn("docStatus", typeof(string)));
			_tableSearch.Columns.Add(new DataColumn("bpCode", typeof(string)));
			_tableSearch.Columns.Add(new DataColumn("userName", typeof(string)));
			_tableSearch.Columns.Add(new DataColumn("branch", typeof(int)));
			DataRow dataRow = _tableSearch.NewRow();
			dataRow["docentry"] = -1;
			_tableSearch.Rows.Add(dataRow);
			BatchData = await GetStructureBatch(0, -1);
			ConfimedData = BatchData.Clone();
			StructureBatch = BatchData.Copy();
			foreach (GridColumnEx item2 in gv_Line.Columns.Where((GridColumn t) => t.OptionsColumn.ReadOnly).Cast<GridColumnEx>())
			{
				ReadOnlyLineCols.Add(item2);
			}
			foreach (GridColumnEx item3 in gv_Batch.Columns.Where((GridColumn t) => t.OptionsColumn.ReadOnly).Cast<GridColumnEx>())
			{
				ReadOnlyBatchCols.Add(item3);
			}
			foreach (GridColumnEx item4 in gv_Confirmed.Columns.Where((GridColumn t) => t.OptionsColumn.ReadOnly).Cast<GridColumnEx>())
			{
				ReadOnlyConfirmCols.Add(item4);
			}
			if (!_dtRequest.IsNotNull())
			{
				await Search();
			}
			else
			{
				await NewDoc();
				DataTable dataTable = grd_Line.DataSourceTable;
				if (!dataTable.IsNotNull())
				{
					dataTable = await GetLines(0);
				}
				dataTable.Rows.Clear();
				for (int num = 0; num < _dtRequest.Rows.Count; num++)
				{
					DataRow dataRow2 = dataTable.NewRow();
					WinFuntions.DataRowToDataRow(_dtRequest.Rows[num], dataRow2);
					dataRow2["PickEntry"] = num;
					dataRow2["AbsEntry"] = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("AbsEntry"));
					dataRow2["PickStatus"] = "O";
					dataTable.Rows.Add(dataRow2);
				}
				grd_Line.DataSource = dataTable;
				NewBatchBlank(-1, grd_Batch);
				NewBatchBlank(-1, grd_Confirmed);
				FormMode = Enums.FormViewModeType.Add;
			}
			rep_Owner.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task REFRESH_DATA()
	{
		await Search();
	}

	private void CopyToShipping(ApzObjectTypes formObjectType, ApzObjectTypes objtype, int fromentry)
	{
		try
		{
			this.DrillDown(formObjectType, objtype, fromentry.ToString());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void NewBatchBlank(int position, GridControlEx grd)
	{
		try
		{
			DataTable dataTable = grd.DataSourceTable;
			if (!dataTable.IsNotNull())
			{
				dataTable = BatchData.Clone();
			}
			DataRow dataRow = dataTable.NewRow();
			dataRow["AbsEntry"] = Function.ParseInt(gv_Line.GetFocusedRowCellValue("AbsEntry"));
			dataRow["PickEntry"] = Function.ParseInt(gv_Line.GetFocusedRowCellValue("PickEntry"));
			if (position == -1)
			{
				if (!dataTable.AsEnumerable().Any((DataRow t) => string.IsNullOrEmpty(Function.ToString(t["DistNumber"]))))
				{
					dataTable.Rows.Add(dataRow);
				}
			}
			else
			{
				dataTable.Rows.InsertAt(dataRow, position);
			}
			dataTable.AcceptChanges();
			grd.DataSource = dataTable;
			grd.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void txt_BpCode_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.Bp, false, "", "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem != null)
			{
				txt_BpCode.EditValue = Function.ToString(frm_CFL2.ChooseItem["CardCode"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Line_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
	{
		try
		{
			GridViewMenu gridViewMenu = new GridViewMenu((GridViewEx)sender);
			DXMenuItem dXMenuItem = new DXMenuItem();
			dXMenuItem.Caption = "Batch/Serial Numbers";
			dXMenuItem.Click += LineBatchMenu_Click;
			gridViewMenu.Items.Add(dXMenuItem);
			if (e.MenuType == GridMenuType.User || e.MenuType == GridMenuType.Row)
			{
				e.Allow = true;
				gridViewMenu.Show(e.Point);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LineBatchMenu_Click(object sender, EventArgs e)
	{
		try
		{
			split_main.PanelVisibility = SplitPanelVisibility.Both;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Line_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			InsertDataToTableBatch(grd_Batch.DataSourceTable.Copy());
			InsertDataToTableConfirmed(grd_Confirmed.DataSourceTable.Copy());
			SetFocusLine();
			string value = Function.ToString(gv_Doc.GetFocusedRowCellValue("Status"));
			string text = Function.ToString(gv_Line.GetFocusedRowCellValue("PickStatus"));
			if (new string[2] { "C", "L" }.Contains(value) || text != "O")
			{
				btn_AutoSelect.Enabled = false;
			}
			else
			{
				btn_AutoSelect.Enabled = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void SetFocusLine()
	{
		grd_Batch.DataSourceTable.Rows.Clear();
		grd_Batch.DataSourceTable.AcceptChanges();
		grd_Confirmed.DataSourceTable.Rows.Clear();
		grd_Confirmed.DataSourceTable.AcceptChanges();
		string text = Function.ToString(gv_Line.GetFocusedRowCellValue("ManBatchSerial"));
		if (!string.IsNullOrEmpty(Function.ToString(gv_Line.GetFocusedRowCellValue("ItemCode"))) && !text.Equals("N"))
		{
			split_main.PanelVisibility = SplitPanelVisibility.Both;
			LoadBatchNumber(Function.ParseInt(gv_Line.GetFocusedRowCellValue("PickEntry")));
		}
		else
		{
			split_main.PanelVisibility = SplitPanelVisibility.Panel1;
		}
	}

	private async Task NewDoc()
	{
		gv_Doc.IsLoadingData = true;
		try
		{
			Task<DataTable> structureDoc = GetStructureDoc();
			Task<int> nextId = GetNextId();
			DataTable newDoc = (await structureDoc).Clone();
			DataRow dr = newDoc.NewRow();
			DataRow dataRow = dr;
			dataRow["AbsEntry"] = await nextId;
			dr["PickDate"] = DateTime.Today;
			dr["Status"] = "O";
			newDoc.Rows.Add(dr);
			grd_Doc.DataSource = newDoc;
			grd_Doc.RefreshDataSource();
			FormMode = Enums.FormViewModeType.Add;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		gv_Doc.IsLoadingData = false;
	}

	private Task<int> GetNextId()
	{
		return _httpClient.Picking.GetNextId();
	}

	private Task<DataTable> GetStructureDoc()
	{
		if (!StructureDoc.IsNotNull())
		{
			return GetDoc(_tableSearch);
		}
		return Task<DataTable>.Factory.StartNew(() => StructureDoc);
	}

	protected virtual async Task<DataTable> GetDoc(DataTable dt)
	{
		return await _httpClient.Picking.GetDoc(dt);
	}

	private void LoadBatchNumber(int docLine)
	{
		if (BatchData == null || BatchData.Columns.Count == 0)
		{
			WinCommons.Error(Enums.MessageCode.CanNotGetDataBatchNumber, "");
			return;
		}
		DataTable dataTable = BatchData.Clone();
		if (string.IsNullOrEmpty(Function.ToString(gv_Line.GetFocusedRowCellValue("ItemCode"))))
		{
			grd_Batch.DataSource = dataTable;
			grd_Batch.RefreshDataSource();
			grd_Confirmed.DataSource = dataTable;
			grd_Confirmed.RefreshDataSource();
			return;
		}
		DataRow[] array = BatchData.Select($"PickEntry = {docLine}");
		for (int i = 0; i < array.Length; i++)
		{
			DataRow dataRow = dataTable.NewRow();
			dataRow.ItemArray = array[i].ItemArray;
			dataTable.Rows.Add(dataRow);
		}
		BatchData.AcceptChanges();
		dataTable.AcceptChanges();
		grd_Batch.DataSource = dataTable;
		grd_Batch.RefreshDataSource();
		NewBatchBlank(-1, grd_Batch);
		DataTable dataTable2 = ConfimedData.Clone();
		DataRow[] array2 = ConfimedData.Select($"PickEntry = {docLine}");
		for (int j = 0; j < array2.Length; j++)
		{
			DataRow dataRow2 = dataTable2.NewRow();
			dataRow2.ItemArray = array2[j].ItemArray;
			dataTable2.Rows.Add(dataRow2);
		}
		ConfimedData.AcceptChanges();
		dataTable2.AcceptChanges();
		grd_Confirmed.DataSource = dataTable2;
		grd_Confirmed.RefreshDataSource();
		NewBatchBlank(-1, grd_Confirmed);
	}

	private Task<DataTable> GetStructureBatch(int absEntry, int pickentry)
	{
		if (!StructureBatch.IsNotNull())
		{
			return GetBatch(absEntry, pickentry);
		}
		return Task<DataTable>.Factory.StartNew(() => StructureBatch);
	}

	private async Task<DataTable> GetBatch(int absEntry, int pickentry)
	{
		return await _httpClient.Picking.GetBatch(absEntry, pickentry);
	}

	private void InsertDataToTableBatch(DataTable dt)
	{
		if (BatchData == null || BatchData.Columns.Count == 0)
		{
			WinCommons.Error(Enums.MessageCode.CanNotGetDataBatchNumber, "");
		}
		else
		{
			if (dt == null || dt.Rows.Count == 0)
			{
				return;
			}
			DataRow[] array = BatchData.Select("PickEntry = " + Function.ParseInt(dt.Rows[0]["PickEntry"]) + " AND AbsEntry=" + Function.ParseInt(dt.Rows[0]["AbsEntry"]));
			for (int i = 0; i < array.Length; i++)
			{
				BatchData.Rows.Remove(array[i]);
			}
			for (int j = 0; j < dt.Rows.Count; j++)
			{
				if (Function.ParseInt(dt.Rows[j]["MdAbsEntry"]) > 0 && Function.ParseDouble(dt.Rows[j]["Quantity"]) != 0.0)
				{
					DataRow dataRow = BatchData.NewRow();
					dataRow.ItemArray = dt.Rows[j].ItemArray;
					BatchData.Rows.Add(dataRow);
				}
			}
			BatchData.AcceptChanges();
		}
	}

	private void InsertDataToTableConfirmed(DataTable dt)
	{
		if (ConfimedData == null || ConfimedData.Columns.Count == 0)
		{
			WinCommons.Error(Enums.MessageCode.CanNotGetDataBatchNumber, "");
		}
		else
		{
			if (dt == null || dt.Rows.Count == 0)
			{
				return;
			}
			DataRow[] array = ConfimedData.Select("PickEntry = " + Function.ParseInt(dt.Rows[0]["PickEntry"]) + " AND AbsEntry=" + Function.ParseInt(dt.Rows[0]["AbsEntry"]));
			for (int i = 0; i < array.Length; i++)
			{
				ConfimedData.Rows.Remove(array[i]);
			}
			for (int j = 0; j < dt.Rows.Count; j++)
			{
				if (Function.ParseInt(dt.Rows[j]["MdAbsEntry"]) > 0 && Function.ParseDouble(dt.Rows[j]["Quantity"]) != 0.0)
				{
					DataRow dataRow = ConfimedData.NewRow();
					dataRow.ItemArray = dt.Rows[j].ItemArray;
					ConfimedData.Rows.Add(dataRow);
				}
			}
			ConfimedData.AcceptChanges();
		}
	}

	private void gv_Line_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			ChangeViewMode();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ChangeViewMode()
	{
		try
		{
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

	private async void btn_OK_Click(object sender, EventArgs e)
	{
		Enums.FormViewModeType currentView = FormMode;
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
				return;
			}
			FormMode = Enums.FormViewModeType.Busy;
			DataRow dataRow = gv_Doc.GetDataRow(gv_Doc.GetFocusedDataSourceRowIndex());
			if (dataRow != null)
			{
				DataSet dataSet = new DataSet();
				DataTable dataTable = grd_Doc.DataSourceTable.Clone();
				dataTable.ImportRow(dataRow);
				if (dataTable.IsNotNull())
				{
					DataTable dataSourceTable = grd_Line.DataSourceTable;
					DataRow[] source = (from t in dataSourceTable.AsEnumerable()
						where !string.IsNullOrEmpty(Function.ToString(t["ItemCode"]))
						select (t)).ToArray();
					if (source.Any())
					{
						dataSourceTable = source.CopyToDataTable();
						dataSourceTable.TableName = TableLine;
						DataTable dataTable2 = dataTable.Copy();
						if (currentView == Enums.FormViewModeType.Add)
						{
							dataTable2.Rows[0]["AbsEntry"] = 0;
						}
						dataTable2.TableName = TableDoc;
						DataTable dataTable3 = CheckDataBatch(dataSourceTable);
						if (dataTable3 == null)
						{
							FormMode = currentView;
							return;
						}
						dataTable3.TableName = TableBatch;
						DataTable dataTable4 = CheckDataConfirm(dataSourceTable);
						dataTable4.TableName = TableBatchConfirm;
						dataSet.Tables.AddRange(new DataTable[4]
						{
							dataTable2,
							dataSourceTable.Copy(),
							dataTable3.Copy(),
							dataTable4.Copy()
						});
						if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
						{
							FormMode = currentView;
							return;
						}
						HttpResult httpResult = await Save(dataSet);
						if (!string.IsNullOrEmpty(httpResult.Message))
						{
							WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
							FormMode = currentView;
							return;
						}
						WinCommons.Success(Enums.MessageCode.SuccessWithDocNo, "", Function.ToString(httpResult.Content));
						FormMode = Enums.FormViewModeType.View;
						await Search();
					}
					else
					{
						FormMode = currentView;
						WinCommons.Error(Enums.MessageCode.ContentIsEmpty, "");
					}
				}
				else
				{
					Logging.Write("ERROR", "Could not get header data. Data is empty");
				}
			}
			else
			{
				Logging.Write("ERROR", "Could not get header data");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
			FormMode = currentView;
		}
	}

	private DataTable CheckDataBatch(DataTable dtLine)
	{
		InsertDataToTableBatch(grd_Batch.DataSourceTable);
		if (BatchData == null || BatchData.Columns.Count == 0 || dtLine == null || dtLine.Rows.Count == 0)
		{
			return new DataTable();
		}
		return BatchData.Copy();
	}

	private DataTable CheckDataConfirm(DataTable dtLine)
	{
		InsertDataToTableConfirmed(grd_Confirmed.DataSourceTable);
		if (ConfimedData == null || ConfimedData.Columns.Count == 0 || dtLine == null || dtLine.Rows.Count == 0)
		{
			return new DataTable();
		}
		return ConfimedData.Copy();
	}

	protected virtual async Task<HttpResult> Save(DataSet ds)
	{
		return await _httpClient.Picking.Save(ds);
	}

	private async void btn_search_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
			{
				await Search();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task Search()
	{
		int num = Function.ParseInt(txt_DocNo.EditValue);
		DateTime? dateTime = Function.ParseDateTime(date_FromDate.EditValue);
		DateTime? dateTime2 = Function.ParseDateTime(date_ToDate.EditValue);
		string value = Function.ToString(txt_BpCode.EditValue);
		string empty = string.Empty;
		DataTable dataTable = _tableSearch.Clone();
		DataRow dataRow = dataTable.NewRow();
		dataRow["docentry"] = num;
		dataRow["whs"] = empty;
		if (dateTime.HasValue)
		{
			dataRow["fromDate"] = dateTime;
		}
		else
		{
			dataRow["fromDate"] = DBNull.Value;
		}
		if (dateTime2.HasValue)
		{
			dataRow["toDate"] = dateTime2;
		}
		else
		{
			dataRow["toDate"] = DBNull.Value;
		}
		dataRow["docStatus"] = Function.ToString(look_Status.EditValue);
		dataRow["bpCode"] = value;
		dataRow["userName"] = GlobalData.UserName;
		dataRow["branch"] = GlobalData.DefaultBranch;
		dataTable.Rows.Add(dataRow);
		DataTable dataSource = await _httpClient.Picking.GetDoc(dataTable);
		grd_Doc.DataSource = dataSource;
		grd_Doc.RefreshDataSource();
		FormMode = Enums.FormViewModeType.View;
	}

	private async void gv_Picking_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		gv_Line.ShowLoadingPanel();
		try
		{
			int docEntry = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("AbsEntry"));
			string text = Function.ToString(gv_Doc.GetFocusedRowCellValue("Status"));
			drlCopyTo.Enabled = docEntry > 0 && text == "O";
			DataTable ret = await GetLines(docEntry);
			StructureLine = ret.Clone();
			if (ret.Rows != null)
			{
				ret.Rows.Add(ret.NewRow());
			}
			grd_Batch.DataSourceTable.Rows.Clear();
			BatchData.Rows.Clear();
			BatchData = await GetBatch(docEntry, 0);
			grd_Confirmed.DataSourceTable.Rows.Clear();
			ConfimedData.Rows.Clear();
			ConfimedData = await GetConfirmed(docEntry, 0);
			grd_Line.DataSource = ret;
			grd_Line.RefreshDataSource();
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		gv_Line.HideLoadingPanel();
	}

	private async Task<DataTable> GetConfirmed(int docEntry, int i)
	{
		return await _httpClient.Picking.GetConfirmed(docEntry, i);
	}

	protected virtual async Task<DataTable> GetLines(int docentry)
	{
		return await _httpClient.Picking.GetLines(docentry);
	}

	private async void rep_BatchSerial_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		await BatchButtonClick(gv_Batch, e);
	}

	private async Task BatchButtonClick(GridViewEx gv, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 1)
			{
				this.DrillDown(ApzObjectTypes.oBatchDetails, Function.ToString(gv.GetFocusedRowCellValue("MdAbsEntry")));
			}
			else if (e.Button.Index == 0)
			{
				if (!(gv.GridControl is GridControlEx grd))
				{
					return;
				}
				string text = Function.ToString(gv_Line.GetFocusedRowCellValue("ItemCode"));
				string text2 = Function.ToString(gv_Line.GetFocusedRowCellValue("WhsCode"));
				if (string.IsNullOrEmpty(text))
				{
					WinCommons.Warning(Enums.MessageCode.YouMustChooseItemCodeBeforeChooseBatch, "");
					gv_Line.Focus();
					gv_Line.FocusedColumn = gv_Line.Columns["ItemCode"];
					gv_Line.ShowEditor();
					return;
				}
				if (string.IsNullOrEmpty(text2))
				{
					WinCommons.Warning(Enums.MessageCode.YouMustChooseWhsBeforeChooseBatch, "");
					gv_Line.Focus();
					gv_Line.FocusedColumn = gv_Line.Columns["WhsCode"];
					gv_Line.ShowEditor();
					return;
				}
				int docEntry = ((FormMode != Enums.FormViewModeType.Add) ? Function.ParseInt(gv_Doc.GetFocusedRowCellValue("AbsEntry")) : 0);
				int orderNum = Function.ParseInt(gv_Line.GetFocusedRowCellValue("OrderEntry"));
				int orderLine = Function.ParseInt(gv_Line.GetFocusedRowCellValue("OrderLine"));
				WinCommons._OpenProcessing();
				DataSet dataSource = await _httpClient.Picking.GetListBatchSerial(text, text2, Function.ParseInt(gv_Line.GetFocusedRowCellValue("PickEntry")), docEntry, orderNum, orderLine);
				WinCommons._CloseProcessing();
				frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.UserDefinedData, true, Function.ToString(gv.GetFocusedRowCellValue("DistNumber")), "", Enums.ItemObjectType.NoItemType, null, "");
				frm_CFL2.DataSource = dataSource;
				frm_CFL2.ShowDialog(this);
				if (frm_CFL2.ChooseItems == null || frm_CFL2.ChooseItems.Count == 0)
				{
					return;
				}
				int focusedRowHandle = gv.FocusedRowHandle;
				int focusedDataSourceRowIndex = gv.GetFocusedDataSourceRowIndex();
				DataTable dataSourceTable = grd.DataSourceTable;
				for (int num = frm_CFL2.ChooseItems.Count - 1; num >= 0; num--)
				{
					if (frm_CFL2.ChooseItems[num] != null)
					{
						gv.BeginInsertRow();
						if (num != frm_CFL2.ChooseItems.Count - 1)
						{
							DataRow row = dataSourceTable.NewRow();
							dataSourceTable.Rows.InsertAt(row, focusedDataSourceRowIndex);
							dataSourceTable.AcceptChanges();
							grd.DataSource = dataSourceTable;
							gv.FocusedRowHandle = focusedRowHandle;
						}
						gv.SetFocusedRowCellValue("MdAbsEntry", frm_CFL2.ChooseItems[num]["MdAbsEntry"]);
						gv.EndInsertRow();
					}
				}
				grd.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		gv.EndInsertRow();
		WinCommons._OpenProcessing();
	}

	private void rep_ItemCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		DrillDownItemCode(Function.ToString(gv_Batch.GetFocusedRowCellValue("ItemCode")));
	}

	private void DrillDownItemCode(string itemCode)
	{
		try
		{
			this.DrillDown(ApzObjectTypes.oItems, itemCode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rep_LineItem_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			this.DrillDown(ApzObjectTypes.oItems, Function.ToString(gv_Line.GetFocusedRowCellValue("ItemCode")));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rep_Whs_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			this.DrillDown(ApzObjectTypes.oWarehouses, Function.ToString(gv_Line.GetFocusedRowCellValue("WhsCode")));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rep_Customer_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			this.DrillDown(ApzObjectTypes.oBusinessPartners, Function.ToString(gv_Line.GetFocusedRowCellValue("CardCode")));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void gv_Line_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			string value = Function.ToString(gv_Doc.GetFocusedRowCellValue("Status"));
			if (new string[2] { "C", "L" }.Contains(value) && !string.IsNullOrEmpty(value))
			{
				e.Appearance.BackColor = GridColor.GridReadOnly;
				{
					foreach (GridColumn item in gv_Line.Columns.Where((GridColumn t) => !t.OptionsColumn.ReadOnly))
					{
						item.OptionsColumn.ReadOnly = true;
					}
					return;
				}
			}
			foreach (GridColumn item2 in gv_Line.Columns.Where((GridColumn t) => !ReadOnlyLineCols.Contains(t)))
			{
				item2.OptionsColumn.ReadOnly = false;
			}
			if (e.Column.OptionsColumn.ReadOnly)
			{
				e.Appearance.BackColor = ((e.RowHandle == gv_Line.FocusedRowHandle) ? GridColor.GridEditting : GridColor.GridReadOnly);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Batch_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		RowCellStyle(sender, e, ReadOnlyBatchCols);
	}

	private void RowCellStyle(object sender, RowCellStyleEventArgs e, List<GridColumnEx> readonlyCols)
	{
		try
		{
			if (!(sender is GridViewEx gridViewEx))
			{
				return;
			}
			string value = Function.ToString(gv_Doc.GetFocusedRowCellValue("Status"));
			string text = Function.ToString(gv_Line.GetFocusedRowCellValue("PickStatus"));
			string text2 = Function.ToString(gridViewEx.GetFocusedRowCellValue("PickStatus"));
			if ((new string[2] { "L", "C" }.Contains(value) && !string.IsNullOrEmpty(value)) || text == "C" || text2 == "C")
			{
				e.Appearance.BackColor = GridColor.GridReadOnly;
				{
					foreach (GridColumn item in gridViewEx.Columns.Where((GridColumn t) => !t.OptionsColumn.ReadOnly))
					{
						item.OptionsColumn.ReadOnly = true;
					}
					return;
				}
			}
			foreach (GridColumn item2 in gridViewEx.Columns.Where((GridColumn t) => !readonlyCols.Contains(t)))
			{
				item2.OptionsColumn.ReadOnly = false;
			}
			if (e.Column.OptionsColumn.ReadOnly)
			{
				e.Appearance.BackColor = ((e.RowHandle == gridViewEx.FocusedRowHandle) ? GridColor.GridEditting : GridColor.GridReadOnly);
				e.Appearance.BackColor = ((e.RowHandle == gridViewEx.FocusedRowHandle) ? GridColor.GridEditting : GridColor.GridReadOnly);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Batch_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		BatchCellValueChanged(gv_Batch, e);
	}

	private void BatchCellValueChanged(GridViewEx gv, CellValueChangedEventArgs e)
	{
		try
		{
			if (!(gv.GridControl is GridControlEx gridControlEx))
			{
				return;
			}
			ChangeViewMode();
			DataTable dataSourceTable = gridControlEx.DataSourceTable;
			int dataSourceRowIndex = gv.GetDataSourceRowIndex(e.RowHandle);
			if (e.Column.FieldName == "Quantity")
			{
				if (Function.ParseDouble(e.Value) <= 0.0 || Function.ParseDouble(gv_Line.GetFocusedRowCellValue("PickQtty")) < Function.ParseDouble(e.Value))
				{
					WinCommons.Error(Enums.MessageCode.InQuantityColumnEnterGreaterThan0, "");
					gv.FocusedColumn = gv.Columns["Quantity"];
					gv.ShowEditor();
					return;
				}
			}
			else if (e.Column.FieldName == "DistNumber")
			{
				if (dataSourceTable.Select(string.Format("{0} = '{1}'", "DistNumber", Function.ToString(e.Value))).Length > 1)
				{
					WinCommons.Error(Enums.MessageCode.BatchSerialNumberAlreadyExistsWithCodeItemAndLine, "", Function.ToString(e.Value), Function.ToString(gv_Line.GetFocusedRowCellValue("ItemCode")), Function.ToString(gv_Line.FocusedRowHandle + 1));
					gv.FocusedColumn = gv.Columns["DistNumber"];
					gv.ShowEditor();
				}
				string disNumber = Function.ToString(e.Value);
				Function.ToString(gv.GetFocusedRowCellValue("MnfSerial"));
				Function.ToString(gv.GetFocusedRowCellValue("LotNumber"));
				string text = Function.ToString(gv_Line.GetFocusedRowCellValue("ItemCode"));
				string whsCode = Function.ToString(gv_Line.GetFocusedRowCellValue("WhsCode"));
				DataTable dtBatch = _httpClient.Commons.GetDataBatchAndSerial(-1, disNumber, text, whsCode);
				if (dtBatch.IsNotNull() && dtBatch.Rows.Count == 1)
				{
					double num = Function.ParseDouble(gv_Line.GetFocusedRowCellValue("PickQtty")) - Function.ParseDouble(dataSourceTable.Compute(string.Format("sum({0})", "Quantity"), ""));
					if (num <= 0.0)
					{
						WinCommons.Warning(Enums.MessageCode.CannotAddBatchSerialQuantityMustBeGreaterOrEqualRequiredBatchSerialNumber, "", Function.ToString(dtBatch.Rows[0]["DistNumber"]));
						return;
					}
					double num2 = Function.ParseDouble(dtBatch.Rows[0]["InStock"]) - Function.ParseDouble(dtBatch.Rows[0]["CommitQty"]);
					double num3 = ((num >= num2) ? num2 : num);
					if (num3 <= 0.0)
					{
						WinCommons.Warning(Enums.MessageCode.InQtyValueGreater0, "");
						return;
					}
					if (dataSourceTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["SysNumber"]) == Function.ParseInt(dtBatch.Rows[0]["SysNumber"])) != null)
					{
						WinCommons.Warning(Enums.MessageCode.TheBatchSerialNumberAlreadyExists, "", Function.ToString(dtBatch.Rows[0]["DistNumber"]));
						return;
					}
					dataSourceTable.Rows[dataSourceRowIndex]["AbsEntry"] = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("AbsEntry"));
					dataSourceTable.Rows[dataSourceRowIndex]["PickEntry"] = Function.ParseInt(gv_Line.GetFocusedRowCellValue("PickEntry"));
					dataSourceTable.Rows[dataSourceRowIndex]["MdAbsEntry"] = Function.ToString(dtBatch.Rows[0]["AbsEntry"]);
					dataSourceTable.Rows[dataSourceRowIndex]["DistNumber"] = Function.ToString(dtBatch.Rows[0]["DistNumber"]);
					dataSourceTable.Rows[dataSourceRowIndex]["ItemCode"] = text;
					dataSourceTable.Rows[dataSourceRowIndex]["Quantity"] = num3;
					dataSourceTable.Rows[dataSourceRowIndex]["SysNumber"] = Function.ParseInt(dtBatch.Rows[0]["SysNumber"]);
					dataSourceTable.Rows[dataSourceRowIndex]["LotNumber"] = Function.ToString(dtBatch.Rows[0]["LotNumber"]);
					dataSourceTable.Rows[dataSourceRowIndex]["MnfSerial"] = Function.ParseInt(dtBatch.Rows[0]["MnfSerial"]);
					dataSourceTable.Rows[dataSourceRowIndex]["BinAbs"] = Function.ParseInt(dtBatch.Rows[0]["BinAbs"]);
					dataSourceTable.Rows[dataSourceRowIndex]["BinCode"] = Function.ToString(dtBatch.Rows[0]["BinCode"]);
					dataSourceTable.Rows[dataSourceRowIndex]["BinDescription"] = Function.ToString(dtBatch.Rows[0]["BinDescription"]);
					dataSourceTable.Rows[dataSourceRowIndex]["PickStatus"] = "O";
					dataSourceTable.Rows[dataSourceRowIndex]["Confirmed"] = ((gv.Name == gv_Confirmed.Name) ? "Y" : "N");
					dataSourceTable.AcceptChanges();
					NewBatchBlank(-1, gridControlEx);
				}
			}
			else if (e.Column.FieldName == "MdAbsEntry")
			{
				string text2 = Function.ToString(gv_Line.GetFocusedRowCellValue("ManBatchSerial"));
				int absEntry = Function.ParseInt(gv.GetFocusedRowCellValue("MdAbsEntry"));
				DataTable dtBatch2 = new DataTable();
				if (text2 == "B")
				{
					dtBatch2 = _httpClient.BatchDetails.GetByAbsEntry(absEntry);
				}
				else if (text2 == "S")
				{
					dtBatch2 = _httpClient.SerialDetails.GetByAbsEntry(absEntry);
				}
				if (dtBatch2.IsNotNull())
				{
					double num4 = Function.ParseDouble(gv_Line.GetFocusedRowCellValue("PickQtty")) - Function.ParseDouble(dataSourceTable.Compute(string.Format("sum({0})", "Quantity"), ""));
					if (num4 <= 0.0)
					{
						WinCommons.Warning(Enums.MessageCode.CannotAddBatchSerialQuantityMustBeGreaterOrEqualRequiredBatchSerialNumber, "", Function.ToString(dtBatch2.Rows[0]["DistNumber"]));
						return;
					}
					double num5 = Function.ParseDouble(dtBatch2.Rows[0]["LogQty"]);
					double num6 = ((num4 >= num5) ? num5 : num4);
					if (num6 <= 0.0)
					{
						WinCommons.Warning(Enums.MessageCode.InQuantityColumnEnterGreaterThan0, "");
						return;
					}
					if (dataSourceTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["SysNumber"]) == Function.ParseInt(dtBatch2.Rows[0]["SysNumber"])) != null)
					{
						WinCommons.Warning(Enums.MessageCode.TheBatchSerialNumberAlreadyExists, "", Function.ToString(dtBatch2.Rows[0]["DistNumber"]));
						return;
					}
					dataSourceTable.Rows[dataSourceRowIndex]["AbsEntry"] = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("AbsEntry"));
					dataSourceTable.Rows[dataSourceRowIndex]["PickEntry"] = Function.ParseInt(gv_Line.GetFocusedRowCellValue("PickEntry"));
					dataSourceTable.Rows[dataSourceRowIndex]["DistNumber"] = Function.ToString(dtBatch2.Rows[0]["DistNumber"]);
					dataSourceTable.Rows[dataSourceRowIndex]["ItemCode"] = Function.ToString(dtBatch2.Rows[0]["ItemCode"]);
					dataSourceTable.Rows[dataSourceRowIndex]["Quantity"] = num6;
					dataSourceTable.Rows[dataSourceRowIndex]["SysNumber"] = Function.ParseInt(dtBatch2.Rows[0]["SysNumber"]);
					dataSourceTable.Rows[dataSourceRowIndex]["LotNumber"] = Function.ToString(dtBatch2.Rows[0]["LotNumber"]);
					dataSourceTable.Rows[dataSourceRowIndex]["MnfSerial"] = Function.ParseInt(dtBatch2.Rows[0]["MnfSerial"]);
					dataSourceTable.Rows[dataSourceRowIndex]["BinAbs"] = Function.ParseInt(dtBatch2.Rows[0]["BinAbs"]);
					dataSourceTable.Rows[dataSourceRowIndex]["BinCode"] = Function.ToString(dtBatch2.Rows[0]["BinCode"]);
					dataSourceTable.Rows[dataSourceRowIndex]["BinDescription"] = Function.ToString(dtBatch2.Rows[0]["BinDescription"]);
					dataSourceTable.Rows[dataSourceRowIndex]["PickStatus"] = "O";
					dataSourceTable.Rows[dataSourceRowIndex]["Confirmed"] = ((gv.Name == gv_Confirmed.Name) ? "Y" : "N");
					dataSourceTable.AcceptChanges();
					NewBatchBlank(-1, gridControlEx);
					gv.FocusedRowHandle = e.RowHandle;
					gv.FocusedColumn = gv.Columns["Quantity"];
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.NoMatchingRecordFound, "");
				}
			}
			dataSourceTable.AcceptChanges();
			gridControlEx.DataSource = dataSourceTable;
			gridControlEx.DataSourceTable.AcceptChanges();
			gridControlEx.RefreshDataSource();
			NewBatchBlank(-1, gridControlEx);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Batch_ShowingEditor(object sender, CancelEventArgs e)
	{
		BatchShowingEditor(gv_Batch, e);
	}

	private void BatchShowingEditor(GridViewEx gv, CancelEventArgs e)
	{
		try
		{
			if (gv.FocusedColumn.FieldName == "Quantity" && Function.ToString(gv_Line.GetFocusedRowCellValue("ManBatchSerial")) == "S")
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

	private void removeRowToolStripMenuItem1_Click(object sender, EventArgs e)
	{
		try
		{
			if (Function.ToString(gv_Batch.GetFocusedRowCellValue("Confirmed")) == "Y")
			{
				WinCommons.Error(Enums.MessageCode.CannotRemoveConfirmedLine, "");
				return;
			}
			gv_Batch.DeleteSelectedRows();
			grd_Batch.DatasourceTable().AcceptChanges();
			InsertDataToTableBatch(grd_Batch.DataSourceTable);
			ChangeViewMode();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Doc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		ChangeViewMode();
	}

	private void rep_Order_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			string num = Function.ToString(gv_Line.GetFocusedRowCellValue("OrderEntry"));
			this.DrillDown(ApzObjectTypes.oOrders, num);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btn_AutoSelect_Click(object sender, EventArgs e)
	{
		Enums.FormViewModeType currentMode = FormMode;
		try
		{
			DataTable dtLine = grd_Line.DataSourceTable;
			if (dtLine.IsNotNull())
			{
				if (BatchData.IsNotNull())
				{
					if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmReplaceBatchSerial, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
					{
						return;
					}
					BatchData.Clear();
				}
				FormMode = Enums.FormViewModeType.Busy;
				WinCommons._OpenProcessing();
				for (int i = 0; i < dtLine.Rows.Count; i++)
				{
					string itemcode = Function.ToString(dtLine.Rows[i]["ItemCode"]);
					if (string.IsNullOrEmpty(itemcode))
					{
						continue;
					}
					string whsCode = Function.ToString(dtLine.Rows[i]["WhsCode"]);
					double quantity = Function.ParseDouble(dtLine.Rows[i]["PickQtty"]);
					int linenum = Function.ParseInt(dtLine.Rows[i]["PickEntry"]);
					int docEntry = ((currentMode != Enums.FormViewModeType.Add) ? Function.ParseInt(gv_Doc.GetFocusedRowCellValue("AbsEntry")) : 0);
					int orderNum = Function.ParseInt(dtLine.Rows[i]["OrderEntry"]);
					int orderLine = Function.ParseInt(dtLine.Rows[i]["OrderLine"]);
					DataTable dataTable = await _httpClient.Picking.AutoSelectBatch(itemcode, whsCode, quantity, linenum, docEntry, orderNum, orderLine);
					if (dataTable.IsNotNull())
					{
						var list = (from batch in dataTable.AsEnumerable()
							join used in from t in BatchData.AsEnumerable()
								where Function.ToString(t["ItemCode"]) == itemcode
								select t on new
							{
								ItemCode = Function.ToString(batch["ItemCode"]),
								DistNumber = Function.ToString(batch["DistNumber"])
							} equals new
							{
								ItemCode = Function.ToString(used["ItemCode"]),
								DistNumber = Function.ToString(used["DistNumber"])
							} into grp
							from item2 in grp.DefaultIfEmpty()
							let item = item2
							where Function.ParseDouble(batch["Quantity"]) - ((item != null) ? Function.ParseDouble(item["Quantity"]) : 0.0) > 0.0
							select new
							{
								MdAbsEntry = batch["MdAbsEntry"],
								ItemCode = batch["ItemCode"],
								DistNumber = batch["DistNumber"],
								SysNumber = batch["SysNumber"],
								MnfSerial = batch["MnfSerial"],
								LotNumber = batch["LotNumber"],
								BinAbs = batch["BinAbs"],
								BinCode = batch["BinCode"],
								BinDescription = batch["BinDescription"],
								Quantity = ((Function.ParseDouble(batch["Quantity"]) - ((item != null) ? Function.ParseDouble(item["Quantity"]) : 0.0) > 0.0) ? (Function.ParseDouble(batch["Quantity"]) - ((item != null) ? Function.ParseDouble(item["Quantity"]) : 0.0)) : Function.ParseDouble(batch["Quantity"]))
							}).ToList();
						if (list.Count > 0)
						{
							double num = 0.0;
							DataTable dataTable2 = BatchData.Clone();
							if (Function.ToString(gv_Batch.GetFocusedRowCellValue("ItemCode")) == itemcode)
							{
								dataTable2 = grd_Batch.DataSourceTable;
							}
							else
							{
								DataRow[] array = (from t in BatchData.AsEnumerable()
									where Function.ToString(t["ItemCode"]) == itemcode
									select t).ToArray();
								if (array.Length != 0)
								{
									dataTable2 = array.CopyToDataTable();
								}
							}
							DataTable dataTable3 = dataTable2.Clone();
							foreach (var item in list)
							{
								double quantity2 = item.Quantity;
								DataRow dataRow = dataTable3.NewRow();
								DataRow dataRow2 = dataTable2.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["SysNumber"]) == Function.ParseInt(item.SysNumber));
								if (dataRow2 != null)
								{
									if (Function.ParseDouble(dataRow2["Quantity"]) == quantity2)
									{
										dataTable3.ImportRow(dataRow2);
										num += item.Quantity;
										if (num >= quantity)
										{
											break;
										}
										continue;
									}
									if (num + item.Quantity > quantity)
									{
										dataRow2["Quantity"] = quantity - num;
									}
									else
									{
										dataRow2["Quantity"] = item.Quantity;
									}
									dataTable3.ImportRow(dataRow2);
									num += item.Quantity;
									if (num >= quantity)
									{
										break;
									}
									continue;
								}
								if (num + item.Quantity > quantity)
								{
									dataRow["Quantity"] = quantity - num;
								}
								else
								{
									dataRow["Quantity"] = item.Quantity;
								}
								dataRow["MdAbsEntry"] = item.MdAbsEntry;
								dataRow["ItemCode"] = item.ItemCode;
								dataRow["DistNumber"] = item.DistNumber;
								dataRow["SysNumber"] = item.SysNumber;
								dataRow["MnfSerial"] = item.MnfSerial;
								dataRow["LotNumber"] = item.LotNumber;
								dataRow["AbsEntry"] = docEntry;
								dataRow["PickEntry"] = linenum;
								dataRow["Confirmed"] = "N";
								dataRow["PickStatus"] = "O";
								dataRow["BinAbs"] = item.BinAbs;
								dataRow["BinCode"] = item.BinCode;
								dataRow["BinDescription"] = item.BinDescription;
								dataTable3.Rows.Add(dataRow);
								num += item.Quantity;
								if (num >= quantity)
								{
									break;
								}
							}
							InsertDataToTableBatch(dataTable3);
							if (currentMode != Enums.FormViewModeType.Add)
							{
								currentMode = Enums.FormViewModeType.Update;
							}
						}
						else
						{
							WinCommons.Error(Enums.MessageCode.CannotCheckBatchSerialResult, "");
						}
					}
					else
					{
						WinCommons.Warning(Enums.MessageCode.NoDataBatchSerialFoundWithLine, "", Function.ToString(linenum));
					}
				}
				SetFocusLine();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			SetFocusLine();
		}
		WinCommons._CloseProcessing();
		FormMode = currentMode;
	}

	private void removeRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
	}

	private async void cancelToolStripMenuItem_Click(object sender, EventArgs e)
	{
		WinCommons._OpenProcessing();
		Enums.FormViewModeType currentMode = FormMode;
		try
		{
			int num = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("AbsEntry"));
			if (num > 0)
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCancelItem, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
				{
					return;
				}
				FormMode = Enums.FormViewModeType.Busy;
				HttpResult httpResult = await Cancel(num);
				if (!string.IsNullOrEmpty(httpResult.Message))
				{
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				}
				else
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
					await Search();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = currentMode;
		WinCommons._CloseProcessing();
	}

	private async Task<HttpResult> Cancel(int absEntry)
	{
		return await _httpClient.Picking.Cancel(absEntry);
	}

	private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
	{
		try
		{
			string text = Function.ToString(gv_Doc.GetFocusedRowCellValue("Status"));
			bool flag = grd_Batch.DataSourceTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["Confirmed"]) == "Y") != null;
			if (!flag && BatchData.IsNotNull())
			{
				flag = BatchData.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["Confirmed"]) == "Y") != null;
			}
			cancelToolStripMenuItem.Visible = text == "O" && !flag;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Doc_BeforeLeaveRow(object sender, RowAllowEventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.View)
			{
				e.Allow = XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rep_ConfirmedItemCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		DrillDownItemCode(Function.ToString(gv_Confirmed.GetFocusedRowCellValue("ItemCode")));
	}

	private void gv_Confirmed_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		RowCellStyle(sender, e, ReadOnlyConfirmCols);
	}

	private async void rep_ConfirmedBatch_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		await BatchButtonClick(gv_Confirmed, e);
	}

	private void gv_Confirmed_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		BatchCellValueChanged(gv_Confirmed, e);
	}

	private void gv_Confirmed_ShowingEditor(object sender, CancelEventArgs e)
	{
		BatchShowingEditor(gv_Confirmed, e);
	}

	private void toolStripMenuItem1_Click(object sender, EventArgs e)
	{
		try
		{
			gv_Confirmed.DeleteSelectedRows();
			grd_Confirmed.DatasourceTable().AcceptChanges();
			InsertDataToTableConfirmed(grd_Confirmed.DataSourceTable);
			ChangeViewMode();
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Inventory.Pick_and_Pack.frm_PickingDocument));
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions4 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions5 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions6 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions7 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions8 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions9 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled9 = new DevExpress.Utils.SerializableAppearanceObject();
		this.repositoryItemCalcEditEx1 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx2 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx3 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx4 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx5 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx6 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx7 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx8 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx9 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.split_main = new DevExpress.XtraEditors.SplitContainerControl();
		this.split_Picked = new DevExpress.XtraEditors.SplitContainerControl();
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.cancelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx3 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Owner = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx6 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Status = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx5 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx28 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx32 = new Apzon.Controls.Grid.GridColumnEx();
		this.grd_Line = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Line = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx8 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_LineItem = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gridColumnEx9 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx10 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Whs = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gridColumnEx11 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Customer = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gridColumnEx12 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx13 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx14 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_LineStatus = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx15 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_ManBatchSerial = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx16 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Order = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gridColumnEx26 = new Apzon.Controls.Grid.GridColumnEx();
		this.xtraTabControl1 = new Apzon.Controls.Controls.XtraTabControlEx();
		this.xtraTabPage1 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_Batch = new Apzon.Controls.Grid.GridControlEx();
		this.ctx_BatchSerial = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.removeRowToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Batch = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx17 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_BatchSerial = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gridColumnEx18 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_ItemCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gridColumnEx19 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx20 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx21 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx22 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx23 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx24 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx25 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx7 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gridColumnEx27 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_BatchPickStatus = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx29 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx30 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx31 = new Apzon.Controls.Grid.GridColumnEx();
		this.xtraTabPage2 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_Confirmed = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Confirmed = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx33 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_ConfirmedBatch = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gridColumnEx34 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_ConfirmedItemCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gridColumnEx35 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx36 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx37 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx38 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx39 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx40 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx41 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx42 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_ConfirmedChecked = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gridColumnEx43 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_ConfirmedStatus = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx44 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx45 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx46 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
		this.btn_AutoSelect = new Apzon.Controls.Controls.SimpleButtonEx();
		this.drlCopyTo = new Apzon.Controls.Controls.DropDownButtonEx();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
		this.look_Status = new DevExpress.XtraEditors.LookUpEdit();
		this.txt_DocNo = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine6 = new Apzon.Controls.Controls.LabelLine();
		this.date_ToDate = new Apzon.Controls.Editor.DateEditEx();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.date_FromDate = new Apzon.Controls.Editor.DateEditEx();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.txt_BpCode = new Apzon.Controls.Editor.ButtonEditEx();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.btn_Search = new Apzon.Controls.Controls.SimpleButtonEx();
		this.ctx_Line = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.removeRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx6).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx7).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx8).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx9).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.split_main).BeginInit();
		this.split_main.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.split_Picked).BeginInit();
		this.split_Picked.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Owner).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Status).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Line).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Line).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_LineItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Whs).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Customer).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_LineStatus).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ManBatchSerial).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Order).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).BeginInit();
		this.xtraTabControl1.SuspendLayout();
		this.xtraTabPage1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Batch).BeginInit();
		this.ctx_BatchSerial.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Batch).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_BatchSerial).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ItemCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_BatchPickStatus).BeginInit();
		this.xtraTabPage2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Confirmed).BeginInit();
		this.contextMenuStrip2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Confirmed).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ConfirmedBatch).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ConfirmedItemCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ConfirmedChecked).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ConfirmedStatus).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl1).BeginInit();
		this.panelControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.panelControl2).BeginInit();
		this.panelControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.look_Status.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_DocNo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_ToDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_ToDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_FromDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_FromDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BpCode.Properties).BeginInit();
		this.ctx_Line.SuspendLayout();
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
		this.repositoryItemCalcEditEx9.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx9.IsSearchControl = false;
		this.repositoryItemCalcEditEx9.Name = "repositoryItemCalcEditEx9";
		this.split_main.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.split_main.Horizontal = false;
		this.split_main.Location = new System.Drawing.Point(0, 38);
		this.split_main.Name = "split_main";
		this.split_main.Panel1.Controls.Add(this.split_Picked);
		this.split_main.Panel1.Text = "Panel1";
		this.split_main.Panel2.Controls.Add(this.xtraTabControl1);
		this.split_main.Panel2.Text = "Panel2";
		this.split_main.Size = new System.Drawing.Size(1071, 517);
		this.split_main.SplitterPosition = 335;
		this.split_main.TabIndex = 20;
		this.split_main.Text = "splitContainerControl1";
		this.split_Picked.Dock = System.Windows.Forms.DockStyle.Fill;
		this.split_Picked.Horizontal = false;
		this.split_Picked.Location = new System.Drawing.Point(0, 0);
		this.split_Picked.Name = "split_Picked";
		this.split_Picked.Panel1.Controls.Add(this.grd_Doc);
		this.split_Picked.Panel1.Text = "Panel1";
		this.split_Picked.Panel2.Controls.Add(this.grd_Line);
		this.split_Picked.Panel2.Text = "Panel2";
		this.split_Picked.Size = new System.Drawing.Size(1071, 335);
		this.split_Picked.SplitterPosition = 181;
		this.split_Picked.TabIndex = 1;
		this.split_Picked.Text = "splitContainerControl3";
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_Doc.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Doc.Location = new System.Drawing.Point(0, 0);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.rep_Status, this.rep_Owner });
		this.grd_Doc.Size = new System.Drawing.Size(1071, 181);
		this.grd_Doc.TabIndex = 0;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.cancelToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(111, 26);
		this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
		this.cancelToolStripMenuItem.Name = "cancelToolStripMenuItem";
		this.cancelToolStripMenuItem.Size = new System.Drawing.Size(110, 22);
		this.cancelToolStripMenuItem.Text = "Cancel";
		this.cancelToolStripMenuItem.Click += new System.EventHandler(this.cancelToolStripMenuItem_Click);
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = null;
		this.gv_Doc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[8] { this.gridColumnEx1, this.gridColumnEx2, this.gridColumnEx3, this.gridColumnEx4, this.gridColumnEx6, this.gridColumnEx5, this.gridColumnEx28, this.gridColumnEx32 });
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = "oPickingDocument";
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.IgnoreFilter = false;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 35;
		this.gv_Doc.IsApzTable = false;
		this.gv_Doc.IsInsertNewRow = false;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.IsMappingContextAction = false;
		this.gv_Doc.ItemId = "ODOC";
		this.gv_Doc.LstColumnAllowBatch = null;
		this.gv_Doc.LstColumnNotAllowBatch = null;
		this.gv_Doc.LstColumnNotEditable = null;
		this.gv_Doc.LstColumNotSetting = null;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsView.ColumnAutoWidth = false;
		this.gv_Doc.OptionsView.ShowGroupPanel = false;
		this.gv_Doc.UseBatchMod = false;
		this.gv_Doc.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_Picking_FocusedRowObjectChanged);
		this.gv_Doc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Doc_CellValueChanged);
		this.gv_Doc.BeforeLeaveRow += new DevExpress.XtraGrid.Views.Base.RowAllowEventHandler(this.gv_Doc_BeforeLeaveRow);
		this.gridColumnEx1.Caption = "Pick No";
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "AbsEntry";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.PrimaryKey = true;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx1.Visible = true;
		this.gridColumnEx1.VisibleIndex = 0;
		this.gridColumnEx1.Width = 100;
		this.gridColumnEx2.Caption = "Name";
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.FieldName = "Name";
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.UnitField = null;
		this.gridColumnEx2.Visible = true;
		this.gridColumnEx2.VisibleIndex = 1;
		this.gridColumnEx2.Width = 100;
		this.gridColumnEx3.Caption = "Owner";
		this.gridColumnEx3.ColumnEdit = this.rep_Owner;
		this.gridColumnEx3.CurrencyDateField = null;
		this.gridColumnEx3.CurrencyField = null;
		this.gridColumnEx3.CurrencyFixed = null;
		this.gridColumnEx3.CurrencyFixedRate = 0.0;
		this.gridColumnEx3.CurrencyRateField = null;
		this.gridColumnEx3.FieldName = "OwnerCode";
		this.gridColumnEx3.Name = "gridColumnEx3";
		this.gridColumnEx3.PrimaryKey = false;
		this.gridColumnEx3.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridColumnEx3.UnitField = null;
		this.gridColumnEx3.Visible = true;
		this.gridColumnEx3.VisibleIndex = 2;
		this.gridColumnEx3.Width = 100;
		this.rep_Owner.AutoHeight = false;
		this.rep_Owner.AutoLoadDataSource = true;
		editorButtonImageOptions.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions1.Image");
		this.rep_Owner.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_Owner.ContainerControl = this;
		this.rep_Owner.DisplayMember = "firstName";
		this.rep_Owner.DisplayMember2 = "lastName";
		this.rep_Owner.GridToRefreshDisplayMember = this.grd_Doc;
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
		this.gridColumnEx4.Caption = "Pick Date";
		this.gridColumnEx4.CurrencyDateField = null;
		this.gridColumnEx4.CurrencyField = null;
		this.gridColumnEx4.CurrencyFixed = null;
		this.gridColumnEx4.CurrencyFixedRate = 0.0;
		this.gridColumnEx4.CurrencyRateField = null;
		this.gridColumnEx4.FieldName = "PickDate";
		this.gridColumnEx4.Name = "gridColumnEx4";
		this.gridColumnEx4.PrimaryKey = false;
		this.gridColumnEx4.UnitField = null;
		this.gridColumnEx4.Visible = true;
		this.gridColumnEx4.VisibleIndex = 3;
		this.gridColumnEx4.Width = 100;
		this.gridColumnEx6.Caption = "Status";
		this.gridColumnEx6.ColumnEdit = this.rep_Status;
		this.gridColumnEx6.CurrencyDateField = null;
		this.gridColumnEx6.CurrencyField = null;
		this.gridColumnEx6.CurrencyFixed = null;
		this.gridColumnEx6.CurrencyFixedRate = 0.0;
		this.gridColumnEx6.CurrencyRateField = null;
		this.gridColumnEx6.FieldName = "Status";
		this.gridColumnEx6.Name = "gridColumnEx6";
		this.gridColumnEx6.OptionsColumn.ReadOnly = true;
		this.gridColumnEx6.PrimaryKey = false;
		this.gridColumnEx6.UnitField = null;
		this.gridColumnEx6.Visible = true;
		this.gridColumnEx6.VisibleIndex = 4;
		this.gridColumnEx6.Width = 100;
		this.rep_Status.AutoHeight = false;
		this.rep_Status.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Status.Name = "rep_Status";
		this.rep_Status.NullText = "";
		this.gridColumnEx5.Caption = "Remarks";
		this.gridColumnEx5.CurrencyDateField = null;
		this.gridColumnEx5.CurrencyField = null;
		this.gridColumnEx5.CurrencyFixed = null;
		this.gridColumnEx5.CurrencyFixedRate = 0.0;
		this.gridColumnEx5.CurrencyRateField = null;
		this.gridColumnEx5.FieldName = "Remarks";
		this.gridColumnEx5.Name = "gridColumnEx5";
		this.gridColumnEx5.PrimaryKey = false;
		this.gridColumnEx5.UnitField = null;
		this.gridColumnEx5.Visible = true;
		this.gridColumnEx5.VisibleIndex = 5;
		this.gridColumnEx5.Width = 100;
		this.gridColumnEx28.Caption = "To Gate";
		this.gridColumnEx28.CurrencyDateField = null;
		this.gridColumnEx28.CurrencyField = null;
		this.gridColumnEx28.CurrencyFixed = null;
		this.gridColumnEx28.CurrencyFixedRate = 0.0;
		this.gridColumnEx28.CurrencyRateField = null;
		this.gridColumnEx28.FieldName = "ToGate";
		this.gridColumnEx28.Name = "gridColumnEx28";
		this.gridColumnEx28.PrimaryKey = false;
		this.gridColumnEx28.UnitField = null;
		this.gridColumnEx28.Visible = true;
		this.gridColumnEx28.VisibleIndex = 6;
		this.gridColumnEx28.Width = 132;
		this.gridColumnEx32.Caption = "Container No.";
		this.gridColumnEx32.CurrencyDateField = null;
		this.gridColumnEx32.CurrencyField = null;
		this.gridColumnEx32.CurrencyFixed = null;
		this.gridColumnEx32.CurrencyFixedRate = 0.0;
		this.gridColumnEx32.CurrencyRateField = null;
		this.gridColumnEx32.FieldName = "ContainerNo";
		this.gridColumnEx32.Name = "gridColumnEx32";
		this.gridColumnEx32.PrimaryKey = false;
		this.gridColumnEx32.UnitField = null;
		this.gridColumnEx32.Visible = true;
		this.gridColumnEx32.VisibleIndex = 7;
		this.grd_Line.AllowPasteExcel = true;
		this.grd_Line.ClipboardData = null;
		this.grd_Line.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Line.Location = new System.Drawing.Point(0, 0);
		this.grd_Line.MainView = this.gv_Line;
		this.grd_Line.Name = "grd_Line";
		this.grd_Line.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[6] { this.rep_LineItem, this.rep_Whs, this.rep_Customer, this.rep_ManBatchSerial, this.rep_Order, this.rep_LineStatus });
		this.grd_Line.Size = new System.Drawing.Size(1071, 144);
		this.grd_Line.TabIndex = 1;
		this.grd_Line.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Line });
		this.gv_Line.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Line.ColumnKey = null;
		this.gv_Line.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[10] { this.gridColumnEx8, this.gridColumnEx9, this.gridColumnEx10, this.gridColumnEx11, this.gridColumnEx12, this.gridColumnEx13, this.gridColumnEx14, this.gridColumnEx15, this.gridColumnEx16, this.gridColumnEx26 });
		this.gv_Line.DisableFormSetting = false;
		this.gv_Line.DisplayException = false;
		this.gv_Line.EditColumn = null;
		this.gv_Line.FieldCurrency = null;
		this.gv_Line.FieldRate = null;
		this.gv_Line.FormId = "oPickingDocument";
		this.gv_Line.GridControl = this.grd_Line;
		this.gv_Line.IgnoreFilter = false;
		this.gv_Line.IndexEditRow = -1;
		this.gv_Line.IndicatorWidth = 35;
		this.gv_Line.IsApzTable = false;
		this.gv_Line.IsInsertNewRow = false;
		this.gv_Line.IsLoadingData = false;
		this.gv_Line.IsMappingContextAction = false;
		this.gv_Line.ItemId = "OLINE";
		this.gv_Line.LstColumnAllowBatch = null;
		this.gv_Line.LstColumnNotAllowBatch = null;
		this.gv_Line.LstColumnNotEditable = null;
		this.gv_Line.LstColumNotSetting = null;
		this.gv_Line.Name = "gv_Line";
		this.gv_Line.OptionsView.ColumnAutoWidth = false;
		this.gv_Line.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Line.OptionsView.ShowGroupPanel = false;
		this.gv_Line.UseBatchMod = false;
		this.gv_Line.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Line_RowCellStyle);
		this.gv_Line.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(this.gv_Line_PopupMenuShowing);
		this.gv_Line.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_Line_FocusedRowObjectChanged);
		this.gv_Line.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Line_CellValueChanged);
		this.gridColumnEx8.Caption = "Item Code";
		this.gridColumnEx8.ColumnEdit = this.rep_LineItem;
		this.gridColumnEx8.CurrencyDateField = null;
		this.gridColumnEx8.CurrencyField = null;
		this.gridColumnEx8.CurrencyFixed = null;
		this.gridColumnEx8.CurrencyFixedRate = 0.0;
		this.gridColumnEx8.CurrencyRateField = null;
		this.gridColumnEx8.FieldName = "ItemCode";
		this.gridColumnEx8.Name = "gridColumnEx8";
		this.gridColumnEx8.OptionsColumn.ReadOnly = true;
		this.gridColumnEx8.PrimaryKey = true;
		this.gridColumnEx8.UnitField = null;
		this.gridColumnEx8.Visible = true;
		this.gridColumnEx8.VisibleIndex = 0;
		this.gridColumnEx8.Width = 100;
		this.rep_LineItem.AutoHeight = false;
		editorButtonImageOptions2.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_LineItem.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_LineItem.Name = "rep_LineItem";
		this.rep_LineItem.ReadOnly = true;
		this.rep_LineItem.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_LineItem_ButtonClick);
		this.gridColumnEx9.Caption = "Item Name";
		this.gridColumnEx9.CurrencyDateField = null;
		this.gridColumnEx9.CurrencyField = null;
		this.gridColumnEx9.CurrencyFixed = null;
		this.gridColumnEx9.CurrencyFixedRate = 0.0;
		this.gridColumnEx9.CurrencyRateField = null;
		this.gridColumnEx9.FieldName = "ItemName";
		this.gridColumnEx9.Name = "gridColumnEx9";
		this.gridColumnEx9.OptionsColumn.ReadOnly = true;
		this.gridColumnEx9.PrimaryKey = false;
		this.gridColumnEx9.UnitField = null;
		this.gridColumnEx9.Visible = true;
		this.gridColumnEx9.VisibleIndex = 1;
		this.gridColumnEx9.Width = 100;
		this.gridColumnEx10.Caption = "Whs. Code";
		this.gridColumnEx10.ColumnEdit = this.rep_Whs;
		this.gridColumnEx10.CurrencyDateField = null;
		this.gridColumnEx10.CurrencyField = null;
		this.gridColumnEx10.CurrencyFixed = null;
		this.gridColumnEx10.CurrencyFixedRate = 0.0;
		this.gridColumnEx10.CurrencyRateField = null;
		this.gridColumnEx10.FieldName = "WhsCode";
		this.gridColumnEx10.Name = "gridColumnEx10";
		this.gridColumnEx10.OptionsColumn.ReadOnly = true;
		this.gridColumnEx10.PrimaryKey = false;
		this.gridColumnEx10.UnitField = null;
		this.gridColumnEx10.Visible = true;
		this.gridColumnEx10.VisibleIndex = 2;
		this.gridColumnEx10.Width = 100;
		this.rep_Whs.AutoHeight = false;
		editorButtonImageOptions3.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_Whs.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_Whs.Name = "rep_Whs";
		this.rep_Whs.ReadOnly = true;
		this.rep_Whs.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_Whs_ButtonClick);
		this.gridColumnEx11.Caption = "Customer Code";
		this.gridColumnEx11.ColumnEdit = this.rep_Customer;
		this.gridColumnEx11.CurrencyDateField = null;
		this.gridColumnEx11.CurrencyField = null;
		this.gridColumnEx11.CurrencyFixed = null;
		this.gridColumnEx11.CurrencyFixedRate = 0.0;
		this.gridColumnEx11.CurrencyRateField = null;
		this.gridColumnEx11.FieldName = "CardCode";
		this.gridColumnEx11.Name = "gridColumnEx11";
		this.gridColumnEx11.OptionsColumn.ReadOnly = true;
		this.gridColumnEx11.PrimaryKey = false;
		this.gridColumnEx11.UnitField = null;
		this.gridColumnEx11.Visible = true;
		this.gridColumnEx11.VisibleIndex = 3;
		this.gridColumnEx11.Width = 100;
		this.rep_Customer.AutoHeight = false;
		editorButtonImageOptions4.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_Customer.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance4, appearanceHovered4, appearancePressed4, appearanceDisabled4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_Customer.Name = "rep_Customer";
		this.rep_Customer.ReadOnly = true;
		this.rep_Customer.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_Customer_ButtonClick);
		this.gridColumnEx12.Caption = "Customer Name";
		this.gridColumnEx12.CurrencyDateField = null;
		this.gridColumnEx12.CurrencyField = null;
		this.gridColumnEx12.CurrencyFixed = null;
		this.gridColumnEx12.CurrencyFixedRate = 0.0;
		this.gridColumnEx12.CurrencyRateField = null;
		this.gridColumnEx12.FieldName = "CardName";
		this.gridColumnEx12.Name = "gridColumnEx12";
		this.gridColumnEx12.OptionsColumn.ReadOnly = true;
		this.gridColumnEx12.PrimaryKey = false;
		this.gridColumnEx12.UnitField = null;
		this.gridColumnEx12.Visible = true;
		this.gridColumnEx12.VisibleIndex = 4;
		this.gridColumnEx12.Width = 100;
		this.gridColumnEx13.Caption = "Pick Quantity";
		this.gridColumnEx13.ColumnEdit = this.repositoryItemCalcEditEx1;
		this.gridColumnEx13.CurrencyDateField = null;
		this.gridColumnEx13.CurrencyField = null;
		this.gridColumnEx13.CurrencyFixed = null;
		this.gridColumnEx13.CurrencyFixedRate = 0.0;
		this.gridColumnEx13.CurrencyRateField = null;
		this.gridColumnEx13.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gridColumnEx13.FieldName = "PickQtty";
		this.gridColumnEx13.Name = "gridColumnEx13";
		this.gridColumnEx13.PrimaryKey = false;
		this.gridColumnEx13.UnitField = null;
		this.gridColumnEx13.Visible = true;
		this.gridColumnEx13.VisibleIndex = 5;
		this.gridColumnEx13.Width = 100;
		this.gridColumnEx14.Caption = "Pick Status";
		this.gridColumnEx14.ColumnEdit = this.rep_LineStatus;
		this.gridColumnEx14.CurrencyDateField = null;
		this.gridColumnEx14.CurrencyField = null;
		this.gridColumnEx14.CurrencyFixed = null;
		this.gridColumnEx14.CurrencyFixedRate = 0.0;
		this.gridColumnEx14.CurrencyRateField = null;
		this.gridColumnEx14.FieldName = "PickStatus";
		this.gridColumnEx14.Name = "gridColumnEx14";
		this.gridColumnEx14.OptionsColumn.ReadOnly = true;
		this.gridColumnEx14.PrimaryKey = false;
		this.gridColumnEx14.UnitField = null;
		this.gridColumnEx14.Visible = true;
		this.gridColumnEx14.VisibleIndex = 6;
		this.gridColumnEx14.Width = 100;
		this.rep_LineStatus.AutoHeight = false;
		this.rep_LineStatus.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_LineStatus.Name = "rep_LineStatus";
		this.rep_LineStatus.NullText = "";
		this.gridColumnEx15.Caption = "Manage Item by";
		this.gridColumnEx15.ColumnEdit = this.rep_ManBatchSerial;
		this.gridColumnEx15.CurrencyDateField = null;
		this.gridColumnEx15.CurrencyField = null;
		this.gridColumnEx15.CurrencyFixed = null;
		this.gridColumnEx15.CurrencyFixedRate = 0.0;
		this.gridColumnEx15.CurrencyRateField = null;
		this.gridColumnEx15.FieldName = "ManBatchSerial";
		this.gridColumnEx15.Name = "gridColumnEx15";
		this.gridColumnEx15.OptionsColumn.ReadOnly = true;
		this.gridColumnEx15.PrimaryKey = false;
		this.gridColumnEx15.UnitField = null;
		this.gridColumnEx15.Visible = true;
		this.gridColumnEx15.VisibleIndex = 7;
		this.gridColumnEx15.Width = 150;
		this.rep_ManBatchSerial.AutoHeight = false;
		this.rep_ManBatchSerial.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_ManBatchSerial.Name = "rep_ManBatchSerial";
		this.rep_ManBatchSerial.NullText = "";
		this.gridColumnEx16.Caption = "Order";
		this.gridColumnEx16.ColumnEdit = this.rep_Order;
		this.gridColumnEx16.CurrencyDateField = null;
		this.gridColumnEx16.CurrencyField = null;
		this.gridColumnEx16.CurrencyFixed = null;
		this.gridColumnEx16.CurrencyFixedRate = 0.0;
		this.gridColumnEx16.CurrencyRateField = null;
		this.gridColumnEx16.FieldName = "OrderEntry";
		this.gridColumnEx16.Name = "gridColumnEx16";
		this.gridColumnEx16.OptionsColumn.ReadOnly = true;
		this.gridColumnEx16.PrimaryKey = false;
		this.gridColumnEx16.UnitField = null;
		this.gridColumnEx16.Visible = true;
		this.gridColumnEx16.VisibleIndex = 8;
		this.rep_Order.AutoHeight = false;
		editorButtonImageOptions5.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_Order.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions5, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance5, appearanceHovered5, appearancePressed5, appearanceDisabled5, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_Order.Name = "rep_Order";
		this.rep_Order.ReadOnly = true;
		this.rep_Order.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_Order_ButtonClick);
		this.gridColumnEx26.Caption = "Order Line";
		this.gridColumnEx26.CurrencyDateField = null;
		this.gridColumnEx26.CurrencyField = null;
		this.gridColumnEx26.CurrencyFixed = null;
		this.gridColumnEx26.CurrencyFixedRate = 0.0;
		this.gridColumnEx26.CurrencyRateField = null;
		this.gridColumnEx26.FieldName = "OrderLine";
		this.gridColumnEx26.Name = "gridColumnEx26";
		this.gridColumnEx26.OptionsColumn.ReadOnly = true;
		this.gridColumnEx26.PrimaryKey = false;
		this.gridColumnEx26.UnitField = null;
		this.gridColumnEx26.Visible = true;
		this.gridColumnEx26.VisibleIndex = 9;
		this.xtraTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.xtraTabControl1.IsSearchControl = false;
		this.xtraTabControl1.Location = new System.Drawing.Point(0, 0);
		this.xtraTabControl1.Name = "xtraTabControl1";
		this.xtraTabControl1.SelectedTabPage = this.xtraTabPage1;
		this.xtraTabControl1.Size = new System.Drawing.Size(1071, 172);
		this.xtraTabControl1.TabIndex = 3;
		this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[2] { this.xtraTabPage1, this.xtraTabPage2 });
		this.xtraTabPage1.Controls.Add(this.grd_Batch);
		this.xtraTabPage1.EditControl = null;
		this.xtraTabPage1.EditValue = "Planned";
		this.xtraTabPage1.FieldName = null;
		this.xtraTabPage1.IsSearchControl = false;
		this.xtraTabPage1.Name = "xtraTabPage1";
		this.xtraTabPage1.ReadOnly = false;
		this.xtraTabPage1.SettingInVisible = false;
		this.xtraTabPage1.SettingReadonly = false;
		this.xtraTabPage1.Size = new System.Drawing.Size(1069, 147);
		this.xtraTabPage1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPage1.Text = "Planned";
		this.grd_Batch.AllowPasteExcel = true;
		this.grd_Batch.ClipboardData = null;
		this.grd_Batch.ContextMenuStrip = this.ctx_BatchSerial;
		this.grd_Batch.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Batch.Location = new System.Drawing.Point(0, 0);
		this.grd_Batch.MainView = this.gv_Batch;
		this.grd_Batch.Name = "grd_Batch";
		this.grd_Batch.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[4] { this.rep_BatchSerial, this.rep_ItemCode, this.repositoryItemCheckEdit1, this.rep_BatchPickStatus });
		this.grd_Batch.Size = new System.Drawing.Size(1069, 147);
		this.grd_Batch.TabIndex = 2;
		this.grd_Batch.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Batch });
		this.ctx_BatchSerial.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.removeRowToolStripMenuItem1 });
		this.ctx_BatchSerial.Name = "ctx_BatchSerial";
		this.ctx_BatchSerial.Size = new System.Drawing.Size(144, 26);
		this.removeRowToolStripMenuItem1.Name = "removeRowToolStripMenuItem1";
		this.removeRowToolStripMenuItem1.Size = new System.Drawing.Size(143, 22);
		this.removeRowToolStripMenuItem1.Text = "Remove Row";
		this.removeRowToolStripMenuItem1.Click += new System.EventHandler(this.removeRowToolStripMenuItem1_Click);
		this.gv_Batch.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Batch.ColumnKey = "DistNumber";
		this.gv_Batch.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[14]
		{
			this.gridColumnEx17, this.gridColumnEx18, this.gridColumnEx19, this.gridColumnEx20, this.gridColumnEx21, this.gridColumnEx22, this.gridColumnEx23, this.gridColumnEx24, this.gridColumnEx25, this.gridColumnEx7,
			this.gridColumnEx27, this.gridColumnEx29, this.gridColumnEx30, this.gridColumnEx31
		});
		this.gv_Batch.DisableFormSetting = false;
		this.gv_Batch.DisplayException = false;
		this.gv_Batch.EditColumn = null;
		this.gv_Batch.FieldCurrency = null;
		this.gv_Batch.FieldRate = null;
		this.gv_Batch.FormId = "oPickingDocument";
		this.gv_Batch.GridControl = this.grd_Batch;
		this.gv_Batch.IgnoreFilter = false;
		this.gv_Batch.IndexEditRow = -1;
		this.gv_Batch.IndicatorWidth = 35;
		this.gv_Batch.IsApzTable = false;
		this.gv_Batch.IsInsertNewRow = false;
		this.gv_Batch.IsLoadingData = false;
		this.gv_Batch.IsMappingContextAction = false;
		this.gv_Batch.ItemId = "OBATCH";
		this.gv_Batch.LstColumnAllowBatch = null;
		this.gv_Batch.LstColumnNotAllowBatch = null;
		this.gv_Batch.LstColumnNotEditable = null;
		this.gv_Batch.LstColumNotSetting = null;
		this.gv_Batch.Name = "gv_Batch";
		this.gv_Batch.OptionsSelection.MultiSelect = true;
		this.gv_Batch.OptionsView.ColumnAutoWidth = false;
		this.gv_Batch.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Batch.OptionsView.ShowGroupPanel = false;
		this.gv_Batch.UseBatchMod = false;
		this.gv_Batch.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Batch_RowCellStyle);
		this.gv_Batch.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_Batch_ShowingEditor);
		this.gv_Batch.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Batch_CellValueChanged);
		this.gridColumnEx17.Caption = "Batch/Serial Number";
		this.gridColumnEx17.ColumnEdit = this.rep_BatchSerial;
		this.gridColumnEx17.CurrencyDateField = null;
		this.gridColumnEx17.CurrencyField = null;
		this.gridColumnEx17.CurrencyFixed = null;
		this.gridColumnEx17.CurrencyFixedRate = 0.0;
		this.gridColumnEx17.CurrencyRateField = null;
		this.gridColumnEx17.FieldName = "DistNumber";
		this.gridColumnEx17.Name = "gridColumnEx17";
		this.gridColumnEx17.PrimaryKey = true;
		this.gridColumnEx17.UnitField = null;
		this.gridColumnEx17.Visible = true;
		this.gridColumnEx17.VisibleIndex = 0;
		this.gridColumnEx17.Width = 178;
		this.rep_BatchSerial.AutoHeight = false;
		editorButtonImageOptions6.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_BatchSerial.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions6, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance6, appearanceHovered6, appearancePressed6, appearanceDisabled6, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_BatchSerial.Name = "rep_BatchSerial";
		this.rep_BatchSerial.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_BatchSerial_ButtonClick);
		this.gridColumnEx18.Caption = "Item Code";
		this.gridColumnEx18.ColumnEdit = this.rep_ItemCode;
		this.gridColumnEx18.CurrencyDateField = null;
		this.gridColumnEx18.CurrencyField = null;
		this.gridColumnEx18.CurrencyFixed = null;
		this.gridColumnEx18.CurrencyFixedRate = 0.0;
		this.gridColumnEx18.CurrencyRateField = null;
		this.gridColumnEx18.FieldName = "ItemCode";
		this.gridColumnEx18.Name = "gridColumnEx18";
		this.gridColumnEx18.OptionsColumn.ReadOnly = true;
		this.gridColumnEx18.PrimaryKey = false;
		this.gridColumnEx18.UnitField = null;
		this.gridColumnEx18.Visible = true;
		this.gridColumnEx18.VisibleIndex = 1;
		this.gridColumnEx18.Width = 130;
		this.rep_ItemCode.AutoHeight = false;
		editorButtonImageOptions7.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_ItemCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions7, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance7, appearanceHovered7, appearancePressed7, appearanceDisabled7, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_ItemCode.Name = "rep_ItemCode";
		this.rep_ItemCode.ReadOnly = true;
		this.rep_ItemCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_ItemCode_ButtonClick);
		this.gridColumnEx19.Caption = "Order Entry";
		this.gridColumnEx19.CurrencyDateField = null;
		this.gridColumnEx19.CurrencyField = null;
		this.gridColumnEx19.CurrencyFixed = null;
		this.gridColumnEx19.CurrencyFixedRate = 0.0;
		this.gridColumnEx19.CurrencyRateField = null;
		this.gridColumnEx19.FieldName = "OrderEntry";
		this.gridColumnEx19.Name = "gridColumnEx19";
		this.gridColumnEx19.OptionsColumn.ReadOnly = true;
		this.gridColumnEx19.PrimaryKey = false;
		this.gridColumnEx19.UnitField = null;
		this.gridColumnEx19.Width = 100;
		this.gridColumnEx20.Caption = "Order Line";
		this.gridColumnEx20.CurrencyDateField = null;
		this.gridColumnEx20.CurrencyField = null;
		this.gridColumnEx20.CurrencyFixed = null;
		this.gridColumnEx20.CurrencyFixedRate = 0.0;
		this.gridColumnEx20.CurrencyRateField = null;
		this.gridColumnEx20.FieldName = "OrderLine";
		this.gridColumnEx20.Name = "gridColumnEx20";
		this.gridColumnEx20.OptionsColumn.ReadOnly = true;
		this.gridColumnEx20.PrimaryKey = false;
		this.gridColumnEx20.UnitField = null;
		this.gridColumnEx20.Width = 100;
		this.gridColumnEx21.Caption = "Quantity";
		this.gridColumnEx21.ColumnEdit = this.repositoryItemCalcEditEx2;
		this.gridColumnEx21.CurrencyDateField = null;
		this.gridColumnEx21.CurrencyField = null;
		this.gridColumnEx21.CurrencyFixed = null;
		this.gridColumnEx21.CurrencyFixedRate = 0.0;
		this.gridColumnEx21.CurrencyRateField = null;
		this.gridColumnEx21.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gridColumnEx21.FieldName = "Quantity";
		this.gridColumnEx21.Name = "gridColumnEx21";
		this.gridColumnEx21.PrimaryKey = false;
		this.gridColumnEx21.UnitField = null;
		this.gridColumnEx21.Visible = true;
		this.gridColumnEx21.VisibleIndex = 2;
		this.gridColumnEx21.Width = 120;
		this.gridColumnEx22.Caption = "Allocated Qty";
		this.gridColumnEx22.ColumnEdit = this.repositoryItemCalcEditEx3;
		this.gridColumnEx22.CurrencyDateField = null;
		this.gridColumnEx22.CurrencyField = null;
		this.gridColumnEx22.CurrencyFixed = null;
		this.gridColumnEx22.CurrencyFixedRate = 0.0;
		this.gridColumnEx22.CurrencyRateField = null;
		this.gridColumnEx22.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gridColumnEx22.FieldName = "AllocQty";
		this.gridColumnEx22.Name = "gridColumnEx22";
		this.gridColumnEx22.OptionsColumn.ReadOnly = true;
		this.gridColumnEx22.PrimaryKey = false;
		this.gridColumnEx22.UnitField = null;
		this.gridColumnEx22.Width = 100;
		this.gridColumnEx23.Caption = "Release Qty";
		this.gridColumnEx23.ColumnEdit = this.repositoryItemCalcEditEx4;
		this.gridColumnEx23.CurrencyDateField = null;
		this.gridColumnEx23.CurrencyField = null;
		this.gridColumnEx23.CurrencyFixed = null;
		this.gridColumnEx23.CurrencyFixedRate = 0.0;
		this.gridColumnEx23.CurrencyRateField = null;
		this.gridColumnEx23.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gridColumnEx23.FieldName = "ReleaseQty";
		this.gridColumnEx23.Name = "gridColumnEx23";
		this.gridColumnEx23.OptionsColumn.ReadOnly = true;
		this.gridColumnEx23.PrimaryKey = false;
		this.gridColumnEx23.UnitField = null;
		this.gridColumnEx23.Width = 100;
		this.gridColumnEx24.Caption = "Picked Qty";
		this.gridColumnEx24.ColumnEdit = this.repositoryItemCalcEditEx5;
		this.gridColumnEx24.CurrencyDateField = null;
		this.gridColumnEx24.CurrencyField = null;
		this.gridColumnEx24.CurrencyFixed = null;
		this.gridColumnEx24.CurrencyFixedRate = 0.0;
		this.gridColumnEx24.CurrencyRateField = null;
		this.gridColumnEx24.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gridColumnEx24.FieldName = "PickedQty";
		this.gridColumnEx24.Name = "gridColumnEx24";
		this.gridColumnEx24.OptionsColumn.ReadOnly = true;
		this.gridColumnEx24.PrimaryKey = false;
		this.gridColumnEx24.UnitField = null;
		this.gridColumnEx24.Width = 100;
		this.gridColumnEx25.Caption = "Batch/Serial Entry";
		this.gridColumnEx25.CurrencyDateField = null;
		this.gridColumnEx25.CurrencyField = null;
		this.gridColumnEx25.CurrencyFixed = null;
		this.gridColumnEx25.CurrencyFixedRate = 0.0;
		this.gridColumnEx25.CurrencyRateField = null;
		this.gridColumnEx25.FieldName = "MdAbsEntry";
		this.gridColumnEx25.Name = "gridColumnEx25";
		this.gridColumnEx25.OptionsColumn.ReadOnly = true;
		this.gridColumnEx25.PrimaryKey = true;
		this.gridColumnEx25.UnitField = null;
		this.gridColumnEx25.Width = 100;
		this.gridColumnEx7.Caption = "Confirmed";
		this.gridColumnEx7.ColumnEdit = this.repositoryItemCheckEdit1;
		this.gridColumnEx7.CurrencyDateField = null;
		this.gridColumnEx7.CurrencyField = null;
		this.gridColumnEx7.CurrencyFixed = null;
		this.gridColumnEx7.CurrencyFixedRate = 0.0;
		this.gridColumnEx7.CurrencyRateField = null;
		this.gridColumnEx7.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gridColumnEx7.FieldName = "Confirmed";
		this.gridColumnEx7.Name = "gridColumnEx7";
		this.gridColumnEx7.PrimaryKey = false;
		this.gridColumnEx7.UnitField = null;
		this.gridColumnEx7.Width = 90;
		this.repositoryItemCheckEdit1.AutoHeight = false;
		this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
		this.repositoryItemCheckEdit1.ValueChecked = "Y";
		this.repositoryItemCheckEdit1.ValueGrayed = "N";
		this.repositoryItemCheckEdit1.ValueUnchecked = "N";
		this.gridColumnEx27.Caption = "Status";
		this.gridColumnEx27.ColumnEdit = this.rep_BatchPickStatus;
		this.gridColumnEx27.CurrencyDateField = null;
		this.gridColumnEx27.CurrencyField = null;
		this.gridColumnEx27.CurrencyFixed = null;
		this.gridColumnEx27.CurrencyFixedRate = 0.0;
		this.gridColumnEx27.CurrencyRateField = null;
		this.gridColumnEx27.FieldName = "PickStatus";
		this.gridColumnEx27.Name = "gridColumnEx27";
		this.gridColumnEx27.OptionsColumn.ReadOnly = true;
		this.gridColumnEx27.PrimaryKey = false;
		this.gridColumnEx27.UnitField = null;
		this.rep_BatchPickStatus.AutoHeight = false;
		this.rep_BatchPickStatus.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_BatchPickStatus.Name = "rep_BatchPickStatus";
		this.rep_BatchPickStatus.NullText = "";
		this.gridColumnEx29.Caption = "BinAbs";
		this.gridColumnEx29.CurrencyDateField = null;
		this.gridColumnEx29.CurrencyField = null;
		this.gridColumnEx29.CurrencyFixed = null;
		this.gridColumnEx29.CurrencyFixedRate = 0.0;
		this.gridColumnEx29.CurrencyRateField = null;
		this.gridColumnEx29.FieldName = "BinAbs";
		this.gridColumnEx29.Name = "gridColumnEx29";
		this.gridColumnEx29.OptionsColumn.ReadOnly = true;
		this.gridColumnEx29.PrimaryKey = false;
		this.gridColumnEx29.UnitField = null;
		this.gridColumnEx29.Visible = true;
		this.gridColumnEx29.VisibleIndex = 3;
		this.gridColumnEx30.Caption = "Bin Code";
		this.gridColumnEx30.CurrencyDateField = null;
		this.gridColumnEx30.CurrencyField = null;
		this.gridColumnEx30.CurrencyFixed = null;
		this.gridColumnEx30.CurrencyFixedRate = 0.0;
		this.gridColumnEx30.CurrencyRateField = null;
		this.gridColumnEx30.FieldName = "BinCode";
		this.gridColumnEx30.Name = "gridColumnEx30";
		this.gridColumnEx30.OptionsColumn.ReadOnly = true;
		this.gridColumnEx30.PrimaryKey = false;
		this.gridColumnEx30.UnitField = null;
		this.gridColumnEx30.Visible = true;
		this.gridColumnEx30.VisibleIndex = 4;
		this.gridColumnEx30.Width = 98;
		this.gridColumnEx31.Caption = "Bin Description";
		this.gridColumnEx31.CurrencyDateField = null;
		this.gridColumnEx31.CurrencyField = null;
		this.gridColumnEx31.CurrencyFixed = null;
		this.gridColumnEx31.CurrencyFixedRate = 0.0;
		this.gridColumnEx31.CurrencyRateField = null;
		this.gridColumnEx31.FieldName = "BinDescription";
		this.gridColumnEx31.Name = "gridColumnEx31";
		this.gridColumnEx31.OptionsColumn.ReadOnly = true;
		this.gridColumnEx31.PrimaryKey = false;
		this.gridColumnEx31.UnitField = null;
		this.gridColumnEx31.Visible = true;
		this.gridColumnEx31.VisibleIndex = 5;
		this.gridColumnEx31.Width = 164;
		this.xtraTabPage2.Controls.Add(this.grd_Confirmed);
		this.xtraTabPage2.EditControl = null;
		this.xtraTabPage2.EditValue = "Confirmed";
		this.xtraTabPage2.FieldName = null;
		this.xtraTabPage2.IsSearchControl = false;
		this.xtraTabPage2.Name = "xtraTabPage2";
		this.xtraTabPage2.ReadOnly = false;
		this.xtraTabPage2.SettingInVisible = false;
		this.xtraTabPage2.SettingReadonly = false;
		this.xtraTabPage2.Size = new System.Drawing.Size(1069, 147);
		this.xtraTabPage2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPage2.Text = "Confirmed";
		this.grd_Confirmed.AllowPasteExcel = true;
		this.grd_Confirmed.ClipboardData = null;
		this.grd_Confirmed.ContextMenuStrip = this.contextMenuStrip2;
		this.grd_Confirmed.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Confirmed.Location = new System.Drawing.Point(0, 0);
		this.grd_Confirmed.MainView = this.gv_Confirmed;
		this.grd_Confirmed.Name = "grd_Confirmed";
		this.grd_Confirmed.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[4] { this.rep_ConfirmedBatch, this.rep_ConfirmedItemCode, this.rep_ConfirmedChecked, this.rep_ConfirmedStatus });
		this.grd_Confirmed.Size = new System.Drawing.Size(1069, 147);
		this.grd_Confirmed.TabIndex = 3;
		this.grd_Confirmed.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Confirmed });
		this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.toolStripMenuItem1 });
		this.contextMenuStrip2.Name = "ctx_BatchSerial";
		this.contextMenuStrip2.Size = new System.Drawing.Size(144, 26);
		this.toolStripMenuItem1.Name = "toolStripMenuItem1";
		this.toolStripMenuItem1.Size = new System.Drawing.Size(143, 22);
		this.toolStripMenuItem1.Text = "Remove Row";
		this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
		this.gv_Confirmed.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Confirmed.ColumnKey = "DistNumber";
		this.gv_Confirmed.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[14]
		{
			this.gridColumnEx33, this.gridColumnEx34, this.gridColumnEx35, this.gridColumnEx36, this.gridColumnEx37, this.gridColumnEx38, this.gridColumnEx39, this.gridColumnEx40, this.gridColumnEx41, this.gridColumnEx42,
			this.gridColumnEx43, this.gridColumnEx44, this.gridColumnEx45, this.gridColumnEx46
		});
		this.gv_Confirmed.DisableFormSetting = false;
		this.gv_Confirmed.DisplayException = false;
		this.gv_Confirmed.EditColumn = null;
		this.gv_Confirmed.FieldCurrency = null;
		this.gv_Confirmed.FieldRate = null;
		this.gv_Confirmed.FormId = "oPickingDocument";
		this.gv_Confirmed.GridControl = this.grd_Confirmed;
		this.gv_Confirmed.IgnoreFilter = false;
		this.gv_Confirmed.IndexEditRow = -1;
		this.gv_Confirmed.IndicatorWidth = 35;
		this.gv_Confirmed.IsApzTable = false;
		this.gv_Confirmed.IsInsertNewRow = false;
		this.gv_Confirmed.IsLoadingData = false;
		this.gv_Confirmed.IsMappingContextAction = false;
		this.gv_Confirmed.ItemId = "OCONFIRM";
		this.gv_Confirmed.LstColumnAllowBatch = null;
		this.gv_Confirmed.LstColumnNotAllowBatch = null;
		this.gv_Confirmed.LstColumnNotEditable = null;
		this.gv_Confirmed.LstColumNotSetting = null;
		this.gv_Confirmed.Name = "gv_Confirmed";
		this.gv_Confirmed.OptionsSelection.MultiSelect = true;
		this.gv_Confirmed.OptionsView.ColumnAutoWidth = false;
		this.gv_Confirmed.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Confirmed.OptionsView.ShowGroupPanel = false;
		this.gv_Confirmed.UseBatchMod = false;
		this.gv_Confirmed.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Confirmed_RowCellStyle);
		this.gv_Confirmed.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_Confirmed_ShowingEditor);
		this.gv_Confirmed.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Confirmed_CellValueChanged);
		this.gridColumnEx33.Caption = "Batch/Serial Number";
		this.gridColumnEx33.ColumnEdit = this.rep_ConfirmedBatch;
		this.gridColumnEx33.CurrencyDateField = null;
		this.gridColumnEx33.CurrencyField = null;
		this.gridColumnEx33.CurrencyFixed = null;
		this.gridColumnEx33.CurrencyFixedRate = 0.0;
		this.gridColumnEx33.CurrencyRateField = null;
		this.gridColumnEx33.FieldName = "DistNumber";
		this.gridColumnEx33.Name = "gridColumnEx33";
		this.gridColumnEx33.PrimaryKey = true;
		this.gridColumnEx33.UnitField = null;
		this.gridColumnEx33.Visible = true;
		this.gridColumnEx33.VisibleIndex = 0;
		this.gridColumnEx33.Width = 139;
		this.rep_ConfirmedBatch.AutoHeight = false;
		editorButtonImageOptions8.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_ConfirmedBatch.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions8, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance8, appearanceHovered8, appearancePressed8, appearanceDisabled8, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_ConfirmedBatch.Name = "rep_ConfirmedBatch";
		this.rep_ConfirmedBatch.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_ConfirmedBatch_ButtonClick);
		this.gridColumnEx34.Caption = "Item Code";
		this.gridColumnEx34.ColumnEdit = this.rep_ConfirmedItemCode;
		this.gridColumnEx34.CurrencyDateField = null;
		this.gridColumnEx34.CurrencyField = null;
		this.gridColumnEx34.CurrencyFixed = null;
		this.gridColumnEx34.CurrencyFixedRate = 0.0;
		this.gridColumnEx34.CurrencyRateField = null;
		this.gridColumnEx34.FieldName = "ItemCode";
		this.gridColumnEx34.Name = "gridColumnEx34";
		this.gridColumnEx34.OptionsColumn.ReadOnly = true;
		this.gridColumnEx34.PrimaryKey = false;
		this.gridColumnEx34.UnitField = null;
		this.gridColumnEx34.Visible = true;
		this.gridColumnEx34.VisibleIndex = 1;
		this.gridColumnEx34.Width = 100;
		this.rep_ConfirmedItemCode.AutoHeight = false;
		editorButtonImageOptions9.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_ConfirmedItemCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions9, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance9, appearanceHovered9, appearancePressed9, appearanceDisabled9, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_ConfirmedItemCode.Name = "rep_ConfirmedItemCode";
		this.rep_ConfirmedItemCode.ReadOnly = true;
		this.rep_ConfirmedItemCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_ConfirmedItemCode_ButtonClick);
		this.gridColumnEx35.Caption = "Order Entry";
		this.gridColumnEx35.CurrencyDateField = null;
		this.gridColumnEx35.CurrencyField = null;
		this.gridColumnEx35.CurrencyFixed = null;
		this.gridColumnEx35.CurrencyFixedRate = 0.0;
		this.gridColumnEx35.CurrencyRateField = null;
		this.gridColumnEx35.FieldName = "OrderEntry";
		this.gridColumnEx35.Name = "gridColumnEx35";
		this.gridColumnEx35.OptionsColumn.ReadOnly = true;
		this.gridColumnEx35.PrimaryKey = false;
		this.gridColumnEx35.UnitField = null;
		this.gridColumnEx35.Width = 100;
		this.gridColumnEx36.Caption = "Order Line";
		this.gridColumnEx36.CurrencyDateField = null;
		this.gridColumnEx36.CurrencyField = null;
		this.gridColumnEx36.CurrencyFixed = null;
		this.gridColumnEx36.CurrencyFixedRate = 0.0;
		this.gridColumnEx36.CurrencyRateField = null;
		this.gridColumnEx36.FieldName = "OrderLine";
		this.gridColumnEx36.Name = "gridColumnEx36";
		this.gridColumnEx36.OptionsColumn.ReadOnly = true;
		this.gridColumnEx36.PrimaryKey = false;
		this.gridColumnEx36.UnitField = null;
		this.gridColumnEx36.Width = 100;
		this.gridColumnEx37.Caption = "Quantity";
		this.gridColumnEx37.ColumnEdit = this.repositoryItemCalcEditEx6;
		this.gridColumnEx37.CurrencyDateField = null;
		this.gridColumnEx37.CurrencyField = null;
		this.gridColumnEx37.CurrencyFixed = null;
		this.gridColumnEx37.CurrencyFixedRate = 0.0;
		this.gridColumnEx37.CurrencyRateField = null;
		this.gridColumnEx37.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gridColumnEx37.FieldName = "Quantity";
		this.gridColumnEx37.Name = "gridColumnEx37";
		this.gridColumnEx37.PrimaryKey = false;
		this.gridColumnEx37.UnitField = null;
		this.gridColumnEx37.Visible = true;
		this.gridColumnEx37.VisibleIndex = 2;
		this.gridColumnEx37.Width = 100;
		this.gridColumnEx38.Caption = "Allocated Qty";
		this.gridColumnEx38.ColumnEdit = this.repositoryItemCalcEditEx7;
		this.gridColumnEx38.CurrencyDateField = null;
		this.gridColumnEx38.CurrencyField = null;
		this.gridColumnEx38.CurrencyFixed = null;
		this.gridColumnEx38.CurrencyFixedRate = 0.0;
		this.gridColumnEx38.CurrencyRateField = null;
		this.gridColumnEx38.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gridColumnEx38.FieldName = "AllocQty";
		this.gridColumnEx38.Name = "gridColumnEx38";
		this.gridColumnEx38.OptionsColumn.ReadOnly = true;
		this.gridColumnEx38.PrimaryKey = false;
		this.gridColumnEx38.UnitField = null;
		this.gridColumnEx38.Width = 100;
		this.gridColumnEx39.Caption = "Release Qty";
		this.gridColumnEx39.ColumnEdit = this.repositoryItemCalcEditEx8;
		this.gridColumnEx39.CurrencyDateField = null;
		this.gridColumnEx39.CurrencyField = null;
		this.gridColumnEx39.CurrencyFixed = null;
		this.gridColumnEx39.CurrencyFixedRate = 0.0;
		this.gridColumnEx39.CurrencyRateField = null;
		this.gridColumnEx39.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gridColumnEx39.FieldName = "ReleaseQty";
		this.gridColumnEx39.Name = "gridColumnEx39";
		this.gridColumnEx39.OptionsColumn.ReadOnly = true;
		this.gridColumnEx39.PrimaryKey = false;
		this.gridColumnEx39.UnitField = null;
		this.gridColumnEx39.Width = 100;
		this.gridColumnEx40.Caption = "Picked Qty";
		this.gridColumnEx40.ColumnEdit = this.repositoryItemCalcEditEx9;
		this.gridColumnEx40.CurrencyDateField = null;
		this.gridColumnEx40.CurrencyField = null;
		this.gridColumnEx40.CurrencyFixed = null;
		this.gridColumnEx40.CurrencyFixedRate = 0.0;
		this.gridColumnEx40.CurrencyRateField = null;
		this.gridColumnEx40.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gridColumnEx40.FieldName = "PickedQty";
		this.gridColumnEx40.Name = "gridColumnEx40";
		this.gridColumnEx40.OptionsColumn.ReadOnly = true;
		this.gridColumnEx40.PrimaryKey = false;
		this.gridColumnEx40.UnitField = null;
		this.gridColumnEx40.Width = 100;
		this.gridColumnEx41.Caption = "Batch/Serial Entry";
		this.gridColumnEx41.CurrencyDateField = null;
		this.gridColumnEx41.CurrencyField = null;
		this.gridColumnEx41.CurrencyFixed = null;
		this.gridColumnEx41.CurrencyFixedRate = 0.0;
		this.gridColumnEx41.CurrencyRateField = null;
		this.gridColumnEx41.FieldName = "MdAbsEntry";
		this.gridColumnEx41.Name = "gridColumnEx41";
		this.gridColumnEx41.OptionsColumn.ReadOnly = true;
		this.gridColumnEx41.PrimaryKey = true;
		this.gridColumnEx41.UnitField = null;
		this.gridColumnEx41.Width = 100;
		this.gridColumnEx42.Caption = "Confirmed";
		this.gridColumnEx42.ColumnEdit = this.rep_ConfirmedChecked;
		this.gridColumnEx42.CurrencyDateField = null;
		this.gridColumnEx42.CurrencyField = null;
		this.gridColumnEx42.CurrencyFixed = null;
		this.gridColumnEx42.CurrencyFixedRate = 0.0;
		this.gridColumnEx42.CurrencyRateField = null;
		this.gridColumnEx42.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gridColumnEx42.FieldName = "Confirmed";
		this.gridColumnEx42.Name = "gridColumnEx42";
		this.gridColumnEx42.OptionsColumn.ReadOnly = true;
		this.gridColumnEx42.PrimaryKey = false;
		this.gridColumnEx42.UnitField = null;
		this.gridColumnEx42.Visible = true;
		this.gridColumnEx42.VisibleIndex = 3;
		this.gridColumnEx42.Width = 90;
		this.rep_ConfirmedChecked.AutoHeight = false;
		this.rep_ConfirmedChecked.Name = "rep_ConfirmedChecked";
		this.rep_ConfirmedChecked.ValueChecked = "Y";
		this.rep_ConfirmedChecked.ValueGrayed = "N";
		this.rep_ConfirmedChecked.ValueUnchecked = "N";
		this.gridColumnEx43.Caption = "Status";
		this.gridColumnEx43.ColumnEdit = this.rep_ConfirmedStatus;
		this.gridColumnEx43.CurrencyDateField = null;
		this.gridColumnEx43.CurrencyField = null;
		this.gridColumnEx43.CurrencyFixed = null;
		this.gridColumnEx43.CurrencyFixedRate = 0.0;
		this.gridColumnEx43.CurrencyRateField = null;
		this.gridColumnEx43.FieldName = "PickStatus";
		this.gridColumnEx43.Name = "gridColumnEx43";
		this.gridColumnEx43.OptionsColumn.ReadOnly = true;
		this.gridColumnEx43.PrimaryKey = false;
		this.gridColumnEx43.UnitField = null;
		this.gridColumnEx43.Visible = true;
		this.gridColumnEx43.VisibleIndex = 4;
		this.rep_ConfirmedStatus.AutoHeight = false;
		this.rep_ConfirmedStatus.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_ConfirmedStatus.Name = "rep_ConfirmedStatus";
		this.rep_ConfirmedStatus.NullText = "";
		this.gridColumnEx44.Caption = "BinAbs";
		this.gridColumnEx44.CurrencyDateField = null;
		this.gridColumnEx44.CurrencyField = null;
		this.gridColumnEx44.CurrencyFixed = null;
		this.gridColumnEx44.CurrencyFixedRate = 0.0;
		this.gridColumnEx44.CurrencyRateField = null;
		this.gridColumnEx44.FieldName = "BinAbs";
		this.gridColumnEx44.Name = "gridColumnEx44";
		this.gridColumnEx44.OptionsColumn.ReadOnly = true;
		this.gridColumnEx44.PrimaryKey = false;
		this.gridColumnEx44.UnitField = null;
		this.gridColumnEx44.Visible = true;
		this.gridColumnEx44.VisibleIndex = 5;
		this.gridColumnEx45.Caption = "Bin Code";
		this.gridColumnEx45.CurrencyDateField = null;
		this.gridColumnEx45.CurrencyField = null;
		this.gridColumnEx45.CurrencyFixed = null;
		this.gridColumnEx45.CurrencyFixedRate = 0.0;
		this.gridColumnEx45.CurrencyRateField = null;
		this.gridColumnEx45.FieldName = "BinCode";
		this.gridColumnEx45.Name = "gridColumnEx45";
		this.gridColumnEx45.PrimaryKey = false;
		this.gridColumnEx45.UnitField = null;
		this.gridColumnEx45.Visible = true;
		this.gridColumnEx45.VisibleIndex = 6;
		this.gridColumnEx46.Caption = "Bin Description";
		this.gridColumnEx46.CurrencyDateField = null;
		this.gridColumnEx46.CurrencyField = null;
		this.gridColumnEx46.CurrencyFixed = null;
		this.gridColumnEx46.CurrencyFixedRate = 0.0;
		this.gridColumnEx46.CurrencyRateField = null;
		this.gridColumnEx46.FieldName = "BinDescription";
		this.gridColumnEx46.Name = "gridColumnEx46";
		this.gridColumnEx46.OptionsColumn.ReadOnly = true;
		this.gridColumnEx46.PrimaryKey = false;
		this.gridColumnEx46.UnitField = null;
		this.gridColumnEx46.Visible = true;
		this.gridColumnEx46.VisibleIndex = 7;
		this.gridColumnEx46.Width = 128;
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(88, 11);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 21;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.panelControl1.Controls.Add(this.btn_AutoSelect);
		this.panelControl1.Controls.Add(this.drlCopyTo);
		this.panelControl1.Controls.Add(this.btn_OK);
		this.panelControl1.Controls.Add(this.btn_Cancel);
		this.panelControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.panelControl1.Location = new System.Drawing.Point(0, 552);
		this.panelControl1.Name = "panelControl1";
		this.panelControl1.Size = new System.Drawing.Size(1071, 41);
		this.panelControl1.TabIndex = 25;
		this.btn_AutoSelect.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_AutoSelect.EditControl = null;
		this.btn_AutoSelect.EditValue = "Auto Select Batch/Serial";
		this.btn_AutoSelect.FieldName = null;
		this.btn_AutoSelect.IsSearchControl = false;
		this.btn_AutoSelect.Location = new System.Drawing.Point(809, 10);
		this.btn_AutoSelect.Name = "btn_AutoSelect";
		this.btn_AutoSelect.ReadOnly = false;
		this.btn_AutoSelect.SettingInVisible = false;
		this.btn_AutoSelect.SettingReadonly = false;
		this.btn_AutoSelect.Size = new System.Drawing.Size(136, 23);
		this.btn_AutoSelect.TabIndex = 30;
		this.btn_AutoSelect.Text = "Auto Select Batch/Serial";
		this.btn_AutoSelect.Click += new System.EventHandler(this.btn_AutoSelect_Click);
		this.drlCopyTo.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.drlCopyTo.Appearance.Options.UseBackColor = true;
		this.drlCopyTo.EditControl = null;
		this.drlCopyTo.EditValue = "Copy to";
		this.drlCopyTo.Enabled = false;
		this.drlCopyTo.FieldName = null;
		this.drlCopyTo.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("drlCopyTo.ImageOptions.Image");
		this.drlCopyTo.IsSearchControl = false;
		this.drlCopyTo.ItemId = null;
		this.drlCopyTo.Location = new System.Drawing.Point(948, 10);
		this.drlCopyTo.Name = "drlCopyTo";
		this.drlCopyTo.ReadOnly = false;
		this.drlCopyTo.SettingInVisible = false;
		this.drlCopyTo.SettingReadonly = false;
		this.drlCopyTo.Size = new System.Drawing.Size(116, 23);
		this.drlCopyTo.TabIndex = 29;
		this.drlCopyTo.Text = "Copy to";
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.EditControl = null;
		this.btn_OK.EditValue = "OK";
		this.btn_OK.FieldName = null;
		this.btn_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_OK.ImageOptions.Image");
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(10, 11);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.ReadOnly = false;
		this.btn_OK.SettingInVisible = false;
		this.btn_OK.SettingReadonly = false;
		this.btn_OK.Size = new System.Drawing.Size(75, 23);
		this.btn_OK.TabIndex = 25;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.panelControl2.Controls.Add(this.look_Status);
		this.panelControl2.Controls.Add(this.txt_DocNo);
		this.panelControl2.Controls.Add(this.labelLine6);
		this.panelControl2.Controls.Add(this.date_ToDate);
		this.panelControl2.Controls.Add(this.labelLine1);
		this.panelControl2.Controls.Add(this.date_FromDate);
		this.panelControl2.Controls.Add(this.labelLine2);
		this.panelControl2.Controls.Add(this.txt_BpCode);
		this.panelControl2.Controls.Add(this.labelLine4);
		this.panelControl2.Controls.Add(this.labelLine3);
		this.panelControl2.Controls.Add(this.btn_Search);
		this.panelControl2.Dock = System.Windows.Forms.DockStyle.Top;
		this.panelControl2.Location = new System.Drawing.Point(0, 0);
		this.panelControl2.Name = "panelControl2";
		this.panelControl2.Size = new System.Drawing.Size(1071, 31);
		this.panelControl2.TabIndex = 26;
		this.look_Status.Location = new System.Drawing.Point(762, 3);
		this.look_Status.Name = "look_Status";
		this.look_Status.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.look_Status.Properties.NullText = "";
		this.look_Status.Size = new System.Drawing.Size(117, 20);
		this.look_Status.TabIndex = 21;
		this.txt_DocNo.EditControl = null;
		this.txt_DocNo.FieldName = null;
		this.txt_DocNo.IsSearchControl = false;
		this.txt_DocNo.Location = new System.Drawing.Point(92, 5);
		this.txt_DocNo.Name = "txt_DocNo";
		this.txt_DocNo.SettingInVisible = false;
		this.txt_DocNo.SettingReadonly = false;
		this.txt_DocNo.Size = new System.Drawing.Size(102, 20);
		this.txt_DocNo.TabIndex = 20;
		this.labelLine6.EditControl = null;
		this.labelLine6.EditValue = "Doc No";
		this.labelLine6.FieldName = null;
		this.labelLine6.HideWhenEmpty = false;
		this.labelLine6.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine6.Location = new System.Drawing.Point(32, 7);
		this.labelLine6.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine6.Name = "labelLine6";
		this.labelLine6.ReadOnly = false;
		this.labelLine6.SettingInVisible = false;
		this.labelLine6.SettingReadonly = false;
		this.labelLine6.ShowUnderLine = true;
		this.labelLine6.Size = new System.Drawing.Size(63, 20);
		this.labelLine6.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine6.TabIndex = 19;
		this.labelLine6.Utext = "Doc No";
		this.date_ToDate.EditControl = null;
		this.date_ToDate.EditValue = null;
		this.date_ToDate.FieldName = null;
		this.date_ToDate.Location = new System.Drawing.Point(590, 4);
		this.date_ToDate.Name = "date_ToDate";
		this.date_ToDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_ToDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_ToDate.Properties.IsSearchControl = false;
		this.date_ToDate.SettingInVisible = false;
		this.date_ToDate.SettingReadonly = false;
		this.date_ToDate.Size = new System.Drawing.Size(102, 20);
		this.date_ToDate.TabIndex = 15;
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "From Date";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(360, 6);
		this.labelLine1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(63, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 10;
		this.labelLine1.Utext = "From Date";
		this.date_FromDate.EditControl = null;
		this.date_FromDate.EditValue = null;
		this.date_FromDate.FieldName = null;
		this.date_FromDate.Location = new System.Drawing.Point(423, 4);
		this.date_FromDate.Name = "date_FromDate";
		this.date_FromDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_FromDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_FromDate.Properties.IsSearchControl = false;
		this.date_FromDate.SettingInVisible = false;
		this.date_FromDate.SettingReadonly = false;
		this.date_FromDate.Size = new System.Drawing.Size(102, 20);
		this.date_FromDate.TabIndex = 14;
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "To Date";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(531, 6);
		this.labelLine2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(63, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 11;
		this.labelLine2.Utext = "To Date";
		this.txt_BpCode.EditControl = null;
		this.txt_BpCode.FieldName = null;
		this.txt_BpCode.IsSearchControl = false;
		this.txt_BpCode.Location = new System.Drawing.Point(252, 5);
		this.txt_BpCode.Name = "txt_BpCode";
		this.txt_BpCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.txt_BpCode.Properties.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txt_BpCode_Properties_ButtonClick);
		this.txt_BpCode.SettingInVisible = false;
		this.txt_BpCode.SettingReadonly = false;
		this.txt_BpCode.Size = new System.Drawing.Size(102, 20);
		this.txt_BpCode.TabIndex = 16;
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "Bp Code";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(200, 7);
		this.labelLine4.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(63, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 12;
		this.labelLine4.Utext = "Bp Code";
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "Status";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(698, 6);
		this.labelLine3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(63, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 13;
		this.labelLine3.Utext = "Status";
		this.btn_Search.EditControl = null;
		this.btn_Search.EditValue = "Search";
		this.btn_Search.FieldName = null;
		this.btn_Search.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Search.ImageOptions.Image");
		this.btn_Search.IsSearchControl = false;
		this.btn_Search.Location = new System.Drawing.Point(891, 2);
		this.btn_Search.Name = "btn_Search";
		this.btn_Search.ReadOnly = false;
		this.btn_Search.SettingInVisible = false;
		this.btn_Search.SettingReadonly = false;
		this.btn_Search.Size = new System.Drawing.Size(75, 23);
		this.btn_Search.TabIndex = 17;
		this.btn_Search.Text = "Search";
		this.btn_Search.Click += new System.EventHandler(this.btn_search_Click);
		this.ctx_Line.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.removeRowToolStripMenuItem });
		this.ctx_Line.Name = "ctx_Line";
		this.ctx_Line.Size = new System.Drawing.Size(144, 26);
		this.removeRowToolStripMenuItem.Name = "removeRowToolStripMenuItem";
		this.removeRowToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
		this.removeRowToolStripMenuItem.Text = "Remove Row";
		this.removeRowToolStripMenuItem.Click += new System.EventHandler(this.removeRowToolStripMenuItem_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1071, 593);
		base.Controls.Add(this.panelControl2);
		base.Controls.Add(this.panelControl1);
		base.Controls.Add(this.split_main);
		base.FormId = "oPickingDocument";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_PickingDocument";
		base.ShowInTaskbar = false;
		this.Text = "Picking Document";
		base.OnLoadAsync += new Apzon.Controls.Form.LoadAsyncEventHandler(this.frm_PickingDocument_Load);
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx6).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx7).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx8).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx9).EndInit();
		((System.ComponentModel.ISupportInitialize)this.split_main).EndInit();
		this.split_main.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.split_Picked).EndInit();
		this.split_Picked.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Owner).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Status).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Line).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Line).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_LineItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Whs).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Customer).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_LineStatus).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ManBatchSerial).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Order).EndInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).EndInit();
		this.xtraTabControl1.ResumeLayout(false);
		this.xtraTabPage1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Batch).EndInit();
		this.ctx_BatchSerial.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Batch).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_BatchSerial).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ItemCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_BatchPickStatus).EndInit();
		this.xtraTabPage2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Confirmed).EndInit();
		this.contextMenuStrip2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Confirmed).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ConfirmedBatch).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ConfirmedItemCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ConfirmedChecked).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ConfirmedStatus).EndInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl1).EndInit();
		this.panelControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.panelControl2).EndInit();
		this.panelControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.look_Status.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_DocNo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_ToDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_ToDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_FromDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_FromDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BpCode.Properties).EndInit();
		this.ctx_Line.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
