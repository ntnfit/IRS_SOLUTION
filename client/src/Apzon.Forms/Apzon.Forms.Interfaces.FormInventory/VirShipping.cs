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
using Apzon.Forms.Interfaces.ChooseFromList;
using Apzon.Forms.Inventory.Pick_and_Pack;
using Apzon.Forms.Properties;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTab;

namespace Apzon.Forms.Interfaces.FormInventory;

public class VirShipping : XtraFormEx
{
	protected readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	protected List<GridColumnEx> ReadOnlyLineCols = new List<GridColumnEx>();

	protected List<GridColumnEx> ReadOnlyDocCols = new List<GridColumnEx>();

	private bool isLoading;

	protected SplitContainerControl splitContainerControl1;

	protected LabelLine labelLine1;

	protected DateEditEx date_From;

	protected DateEditEx date_To;

	protected LabelLine labelLine2;

	protected LabelLine labelLine3;

	protected VirChooseFromList txt_CardCode;

	protected LabelLine lbl_DocNum;

	protected TextEditEx txt_DocNum;

	protected SimpleButtonEx btn_Search;

	protected GridControlEx grd_Doc;

	protected GridViewEx gv_Doc;

	protected XtraTabControlEx xtraTabControl1;

	protected XtraTabPageEx tabpage_Contents;

	protected GridControlEx grd_Line;

	protected GridViewEx gv_Line;

	protected SimpleButtonEx btn_OK;

	protected SimpleButtonEx btn_Cancel;

	protected GridColumnEx col_DocEntry;

	protected GridColumnEx gridColumnEx2;

	protected GridColumnEx gridColumnEx3;

	protected GridColumnEx gridColumnEx4;

	protected GridColumnEx gridColumnEx5;

	protected GridColumnEx gridColumnEx6;

	protected GridColumnEx gridColumnEx7;

	protected GridColumnEx gridColumnEx8;

	protected GridColumnEx gridColumnEx9;

	protected GridColumnEx gridColumnEx10;

	protected GridColumnEx gridColumnEx11;

	protected GridColumnEx gridColumnEx12;

	protected GridColumnEx gridColumnEx13;

	public DropDownButtonEx drlCopyTo;

	public DropDownButtonEx drlCopyFrom;

	protected GridColumnEx gridColumnEx14;

	protected GridColumnEx gridColumnEx15;

	protected GridColumnEx gridColumnEx16;

	protected GridColumnEx gridColumnEx17;

	protected GridColumnEx gridColumnEx18;

	protected GridColumnEx gridColumnEx19;

	protected GridColumnEx gridColumnEx20;

	protected GridColumnEx gridColumnEx21;

	protected GridColumnEx gridColumnEx22;

	protected GridColumnEx gridColumnEx23;

	protected GridColumnEx gridColumnEx24;

	protected GridColumnEx gridColumnEx25;

	protected GridColumnEx gridColumnEx26;

	protected GridColumnEx gridColumnEx27;

	protected GridColumnEx gridColumnEx28;

	protected LookUpEdit look_Status;

	protected LabelLine labelLine5;

	protected ContextMenuStrip ctx_Doc;

	protected ToolStripMenuItem addToolStripMenuItem;

	protected ToolStripSeparator toolStripSeparator2;

	protected ToolStripMenuItem duplicateToolStripMenuItem;

	protected ToolStripSeparator toolStripSeparator1;

	protected ToolStripMenuItem closeToolStripMenuItem;

	protected ToolStripMenuItem cancelToolStripMenuItem;

	protected ContextMenuStrip ctx_Line;

	protected ToolStripMenuItem addRowToolStripMenuItem;

	protected ToolStripSeparator toolStripSeparator3;

	protected ToolStripMenuItem duplicateRowToolStripMenuItem;

	protected ToolStripSeparator toolStripSeparator4;

	protected ToolStripMenuItem deleteRowToolStripMenuItem;

	protected ToolStripSeparator toolStripSeparator5;

	protected ToolStripMenuItem baseDocumentToolStripMenuItem;

	protected ToolStripMenuItem targetDocumentToolStripMenuItem;

	protected RepositoryItemLookUpEdit rep_DocStatus;

	protected RepositoryItemButtonEdit rep_CardCode;

	protected RepositoryItemButtonEdit rep_Vehicle;

	protected RepositoryItemButtonEdit rep_BaseNum;

	protected RepositoryItemButtonEdit rep_PickNo;

	protected RepositoryItemButtonEdit rep_ItemCode;

	protected RepositoryItemButtonEdit rep_BatchNumber;

	protected RepositoryItemButtonEdit rep_WhsCode;

	protected GridColumnEx gridColumnEx29;

	protected ToolStripMenuItem insertRowToolStripMenuItem;

	private IContainer components;

	private RepositoryItemLookUpEdit rep_TransType;

	private RepositoryItemButtonEdit rep_TransNo;

	private GridColumnEx gridColumnEx1;

	private GridColumnEx gridColumnEx30;

	private RepositoryItemLookUpEdit rep_UseBaseUn;

	private GridColumnEx gridColumnEx31;

	private GridColumnEx gridColumnEx32;

	private GridColumnEx gridColumnEx33;

	private GridColumnEx gridColumnEx34;

	private GridColumnEx gridColumnEx35;

	private GridColumnEx gridColumnEx36;

	private GridColumnEx gridColumnEx37;

	private GridColumnEx gridColumnEx38;

	private GridColumnEx gridColumnEx39;

	private GridColumnEx gridColumnEx40;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx1;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx2;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx3;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx4;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx5;

	public int DocEntry { get; set; }

	private DataTable StructureDoc { get; set; }

	private DataTable StructureLine { get; set; }

	private DataTable TableSearch { get; set; }

	protected string TableDoc { get; set; }

	protected string TableLine { get; set; }

	public List<ApzObjectTypes> PopCopyTo { get; set; }

	public List<ApzObjectTypes> PopCopyFrom { get; set; }

	public ApzObjectTypes FromType { get; set; }

	public int FromEntry { get; set; }

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		try
		{
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
				drlCopyFrom.Enabled = true;
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
				drlCopyFrom.Enabled = false;
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
				drlCopyFrom.Enabled = false;
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
				drlCopyFrom.Enabled = false;
				drlCopyTo.Enabled = false;
				break;
			}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public VirShipping()
	{
		InitializeComponent();
	}

	protected virtual async Task VirShipping_Load(object sender, EventArgs e)
	{
		_ = 3;
		try
		{
			FormMode = Enums.FormViewModeType.View;
			GridViewEx gridViewEx = gv_Doc;
			GridViewEx gridViewEx2 = gv_Line;
			string text = (FormId = FormObjectType.ToString());
			string formId = (gridViewEx2.FormId = text);
			gridViewEx.FormId = formId;
			gv_Doc.ItemId = TableDoc;
			gv_Line.ItemId = TableLine;
			txt_CardCode.TypeCfl = Enums.TypeCFL.Customer;
			Dictionary<string, GridViewEx> dictionary = new Dictionary<string, GridViewEx>();
			dictionary[TableDoc] = gv_Doc;
			dictionary[TableLine] = gv_Line;
			await Task.WhenAll(WinFuntions.LoadUdfColumns(gv_Doc, gv_Doc.ItemId, dictionary), WinFuntions.LoadUdfColumns(gv_Line, gv_Line.ItemId, dictionary));
			TableSearch = new DataTable();
			TableSearch.Columns.Add("FromDate", typeof(DateTime));
			TableSearch.Columns.Add("ToDate", typeof(DateTime));
			TableSearch.Columns.Add("CardCode", typeof(string));
			TableSearch.Columns.Add("DocEntry", typeof(int));
			TableSearch.Columns.Add("DocStatus", typeof(string));
			DataRow dataRow = TableSearch.NewRow();
			dataRow["DocEntry"] = -1;
			TableSearch.Rows.Add(dataRow);
			FormId = ApzObjectTypes.oPacking.ToString();
			gv_Doc.FormId = FormId;
			gv_Doc.ItemId = TableDoc;
			gv_Line.FormId = FormId;
			gv_Line.ItemId = TableLine;
			Dictionary<string, string> dic = new Dictionary<string, string>
			{
				{ "O", "Open" },
				{ "L", "Closed" },
				{ "C", "Canceled" },
				{ "", "All" }
			};
			look_Status.SetDataSource(dic);
			look_Status.EditValue = "O";
			rep_DocStatus.SetDataSource(dic);
			rep_UseBaseUn.SetDataSource(new Dictionary<string, string>
			{
				{ "Y", "Yes" },
				{ "N", "No" }
			});
			date_From.EditValue = DateTime.Today.AddDays(-Configuration.SearchDate);
			date_To.EditValue = DateTime.Today.AddDays(Configuration.SearchDate);
			if (PopCopyTo != null && PopCopyTo.Count > 0)
			{
				DXPopupMenu dXPopupMenu = new DXPopupMenu();
				foreach (ApzObjectTypes item in PopCopyTo)
				{
					DXMenuItem dXMenuItem = new DXMenuItem();
					switch (item)
					{
					case ApzObjectTypes.oDeliveryNotes:
						dXMenuItem.Caption = MenuCaption.Delivery;
						break;
					case ApzObjectTypes.oInvoices:
						dXMenuItem.Caption = MenuCaption.ARInvoices;
						break;
					case ApzObjectTypes.oShipping:
						dXMenuItem.Caption = MenuCaption.Shipping;
						break;
					case ApzObjectTypes.oPacking:
						dXMenuItem.Caption = MenuCaption.Packing;
						break;
					}
					ApzObjectTypes objtype = item;
					dXMenuItem.Click += (object s, EventArgs events) =>
					{
						try
						{
							CopyToShipping(FormObjectType, objtype, Function.ParseInt(gv_Doc.GetFocusedRowCellValue("DocEntry")));
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
			if (PopCopyFrom != null && PopCopyFrom.Count > 0)
			{
				DXPopupMenu dXPopupMenu2 = new DXPopupMenu();
				foreach (ApzObjectTypes item2 in PopCopyFrom)
				{
					DXMenuItem dXMenuItem2 = new DXMenuItem();
					switch (item2)
					{
					case ApzObjectTypes.oPickLists:
						dXMenuItem2.Caption = MenuCaption.PickingDocument;
						break;
					case ApzObjectTypes.oPacking:
						dXMenuItem2.Caption = MenuCaption.Packing;
						break;
					case ApzObjectTypes.oOrders:
						dXMenuItem2.Caption = MenuCaption.SalesOrder;
						break;
					}
					ApzObjectTypes objtype2 = item2;
					dXMenuItem2.Click += (object s, EventArgs events) =>
					{
						try
						{
							CopyFromShipping(objtype2, FormObjectType);
						}
						catch (Exception ex2)
						{
							Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
							WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex2.Message);
						}
					};
					dXPopupMenu2.Items.Add(dXMenuItem2);
				}
				drlCopyFrom.DropDownControl = dXPopupMenu2;
			}
			foreach (GridColumnEx item3 in gv_Line.Columns.Where((GridColumn t) => t.OptionsColumn.ReadOnly).Cast<GridColumnEx>())
			{
				ReadOnlyLineCols.Add(item3);
			}
			foreach (GridColumnEx item4 in gv_Doc.Columns.Where((GridColumn t) => t.OptionsColumn.ReadOnly).Cast<GridColumnEx>())
			{
				ReadOnlyDocCols.Add(item4);
			}
			rep_TransType.SetDataSourceObject();
			if (FromEntry == 0)
			{
				await Search();
			}
			else
			{
				isLoading = true;
				await NewDoc();
				await SetDataCopy(FromType, FormObjectType, new List<int> { FromEntry });
				isLoading = false;
			}
			gv_Doc.CellValueChanged += ChangeViewMode;
			gv_Line.CellValueChanged += ChangeViewMode;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			isLoading = false;
		}
	}

	protected virtual async Task<DataTable> GetDoc(DataTable dt)
	{
		return await _httpClient.Packing.GetDoc(dt);
	}

	protected virtual async Task<DataTable> GetLines(int docentry)
	{
		return await _httpClient.Packing.GetLines(docentry);
	}

	protected virtual Task<int> GetNextId()
	{
		return _httpClient.Packing.GetNextId();
	}

	protected virtual async Task<HttpResult> Close(int docentry)
	{
		return await _httpClient.Packing.Close(docentry);
	}

	protected virtual async Task<HttpResult> Cancel(int docentry)
	{
		return await _httpClient.Packing.Cancel(docentry);
	}

	protected virtual async Task<HttpResult> Save(DataSet ds)
	{
		return await _httpClient.Packing.Save(ds);
	}

	public async Task REFRESH()
	{
		await NewDoc();
	}

	public async Task REFRESH_DATA()
	{
		await Search();
	}

	private void CopyToShipping(ApzObjectTypes fromtype, ApzObjectTypes totype, int fromentry)
	{
		try
		{
			string text = Function.ToString(gv_Doc.GetFocusedRowCellValue("DocStatus"));
			if (!(text != "O") || string.IsNullOrEmpty(text))
			{
				if (totype == ApzObjectTypes.oPacking)
				{
					frm_Packing frm_Packing2 = new frm_Packing();
					frm_Packing2.FromType = fromtype;
					frm_Packing2.MdiParent = MdiParent;
					frm_Packing2.FromEntry = fromentry;
					frm_Packing2.Show();
				}
				else
				{
					this.DrillDown(fromtype, totype, fromentry.ToString());
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void CopyFromShipping(ApzObjectTypes fromtype, ApzObjectTypes totype)
	{
		try
		{
			string text = Function.ToString(gv_Doc.GetFocusedRowCellValue("CardCode"));
			if (string.IsNullOrEmpty(text))
			{
				WinCommons.Error(Enums.MessageCode.PleaseChooseCustommer, "");
				return;
			}
			Enums.TypeCFL type = Enums.TypeCFL.Packing;
			switch (fromtype)
			{
			case ApzObjectTypes.oPickLists:
				type = Enums.TypeCFL.ListOfPick;
				break;
			case ApzObjectTypes.oPacking:
				type = Enums.TypeCFL.ListOfPack;
				break;
			}
			frm_CFL frm_CFL2 = new frm_CFL(type, true, "", "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.BpCode = text;
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItems != null && frm_CFL2.ChooseItems.Count > 0)
			{
				new List<int>();
				List<int> lst = ((fromtype != ApzObjectTypes.oPickLists) ? frm_CFL2.ChooseItems.Select((DataRow t) => Function.ParseInt(t["DocEntry"])).ToList() : frm_CFL2.ChooseItems.Select((DataRow t) => Function.ParseInt(t["AbsEntry"])).ToList());
				await SetDataCopy(fromtype, totype, lst);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task SetDataCopy(ApzObjectTypes fromtype, ApzObjectTypes totype, List<int> lst)
	{
		DataTable dataTable = await _httpClient.Shipping.CopyHeader(fromtype, totype, lst);
		if (dataTable.IsNotNull())
		{
			DataTable dataSourceTable = grd_Doc.DataSourceTable;
			if (dataSourceTable.IsNotNull() && dataTable.Rows.Count == 1)
			{
				foreach (DataColumn item in from DataColumn t in dataTable.Columns
					where t.ColumnName.StartsWith("U_")
					select t)
				{
					if (dataSourceTable.ExistsColumn(item.ColumnName))
					{
						dataSourceTable.Rows[0][item.ColumnName] = dataTable.Rows[0][item.ColumnName];
					}
				}
				dataSourceTable.AcceptChanges();
			}
			DataTable dataTable2 = await _httpClient.Shipping.Copy(fromtype, totype, lst);
			if (dataTable2.IsNotNull())
			{
				grd_Line.DataSource = dataTable2;
				grd_Line.RefreshDataSource();
				NewLineBlank(-1);
				gv_Doc.SetFocusedRowCellValue("CardCode", dataTable2.Rows[0]["CardCode"]);
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.NoMatchingRecordFound, "");
			}
		}
		else
		{
			WinCommons.Error(Enums.MessageCode.NoMatchingRecordFound, "");
		}
	}

	private void ChangeViewMode(object sender, CellValueChangedEventArgs e)
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
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
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

	private async void btn_Search_Click(object sender, EventArgs e)
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
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task Search()
	{
		gv_Doc.ShowLoadingPanel();
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			DataTable dataTable = TableSearch.Clone();
			DataRow dataRow = dataTable.NewRow();
			dataRow["FromDate"] = Function.ParseDateTime(date_From.EditValue);
			dataRow["ToDate"] = Function.ParseDateTime(date_To.EditValue);
			dataRow["CardCode"] = Function.ToString(txt_CardCode.EditValue);
			dataRow["DocEntry"] = ((DocEntry == 0) ? Function.ParseInt(txt_DocNum.EditValue) : DocEntry);
			dataRow["DocStatus"] = Function.ToString(look_Status.EditValue);
			dataTable.Rows.Add(dataRow);
			DataTable dataTable2 = await GetDoc(dataTable);
			StructureDoc = dataTable2.Clone();
			grd_Doc.DataSource = dataTable2;
			grd_Doc.RefreshDataSource();
			if (!dataTable2.IsNotNull())
			{
				grd_Line.DataSource = StructureLine;
				grd_Line.RefreshDataSource();
			}
			DocEntry = 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.View;
		gv_Doc.HideLoadingPanel();
	}

	private void NewLineBlank(int position)
	{
		try
		{
			DataTable dataSourceTable = grd_Line.DataSourceTable;
			DataRow dataRow = dataSourceTable.NewRow();
			dataRow["DocEntry"] = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("DocEntry"));
			dataRow["LineNum"] = -1;
			dataRow["LineStatus"] = "O";
			if (position == -1)
			{
				if (!dataSourceTable.AsEnumerable().Any((DataRow t) => string.IsNullOrEmpty(Function.ToString(t["ItemCode"]))))
				{
					dataSourceTable.Rows.Add(dataRow);
				}
			}
			else
			{
				dataSourceTable.Rows.InsertAt(dataRow, position);
			}
			dataSourceTable.AcceptChanges();
			grd_Line.DataSource = dataSourceTable;
			grd_Line.RefreshDataSource();
			if (FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private Task<DataTable> GetStructureDoc()
	{
		if (!StructureDoc.IsNotNull())
		{
			return GetDoc(TableSearch);
		}
		return Task<DataTable>.Factory.StartNew(() => StructureDoc);
	}

	private Task<DataTable> GetStructureLine()
	{
		if (!StructureDoc.IsNotNull())
		{
			return GetLines(-1);
		}
		return Task<DataTable>.Factory.StartNew(() => StructureLine);
	}

	private async Task NewDoc()
	{
		_ = 1;
		try
		{
			if (FormMode == Enums.FormViewModeType.View || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
			{
				Task<DataTable> structureDoc = GetStructureDoc();
				Task<int> nextId = GetNextId();
				DataTable newDoc = (await structureDoc).Clone();
				DataRow dr = newDoc.NewRow();
				DataRow dataRow = dr;
				dataRow["DocEntry"] = await nextId;
				dr["DocDate"] = DateTime.Today;
				dr["DocStatus"] = "O";
				newDoc.Rows.Add(dr);
				grd_Doc.DataSource = newDoc;
				grd_Doc.RefreshDataSource();
				FormMode = Enums.FormViewModeType.Add;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task gv_Doc_FocusedRowObjectChangedAsync(FocusedRowObjectChangedEventArgs e)
	{
		if (!isLoading)
		{
			gv_Line.ShowLoadingPanel();
			try
			{
				int num = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("DocEntry"));
				string text = Function.ToString(gv_Doc.GetFocusedRowCellValue("DocStatus"));
				drlCopyTo.Enabled = num > 0 && text == "O";
				drlCopyFrom.Enabled = false;
				DataTable dataTable = await GetLines(num);
				StructureLine = dataTable.Clone();
				dataTable.Rows.Add(dataTable.NewRow());
				grd_Line.DataSource = dataTable;
				grd_Line.RefreshDataSource();
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
				WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			}
			gv_Line.HideLoadingPanel();
		}
	}

	private async void addToolStripMenuItem_Click(object sender, EventArgs e)
	{
		await NewDoc();
	}

	private async Task gv_Line_CellValueChangedAsync(CellValueChangedEventArgs e)
	{
		_ = 1;
		try
		{
			if (!(e.Column.FieldName == "PickNo") && !(e.Column.FieldName == "PackNo"))
			{
				return;
			}
			int num = Function.ParseInt(gv_Line.GetFocusedRowCellValue(e.Column));
			if (num == 0)
			{
				return;
			}
			if (FormObjectType == ApzObjectTypes.oPacking && e.Column.FieldName == "PackNo" && (FormMode == Enums.FormViewModeType.Update || FormMode == Enums.FormViewModeType.View))
			{
				int num2 = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("DocEntry"));
				if (num == num2)
				{
					WinCommons.Error(Enums.MessageCode.CouldNotBeAddedAddingContentWouldCauseCirularDependency, "", e.Column.Caption);
					grd_Line.DataSourceTable.Rows[gv_Line.GetFocusedDataSourceRowIndex()]["PackNo"] = DBNull.Value;
					return;
				}
			}
			DataTable dataTable = new DataTable();
			Enums.TypeCFL type = Enums.TypeCFL.PickList;
			string fieldName = e.Column.FieldName;
			if (!(fieldName == "PickNo"))
			{
				if (fieldName == "PackNo")
				{
					dataTable = await _httpClient.Packing.GetLineByPackNo(num);
					type = Enums.TypeCFL.Packing;
				}
			}
			else
			{
				dataTable = await _httpClient.Packing.GetLineByPickNo(num);
			}
			if (dataTable.IsNotNull())
			{
				if (dataTable.Rows.Count > 1)
				{
					frm_CFL frm_CFL2 = new frm_CFL(type, true, "", "", Enums.ItemObjectType.NoItemType, null, "");
					DataSet dataSet = new DataSet();
					dataSet.Tables.Add(dataTable.Copy());
					frm_CFL2.DataSource = dataSet;
					frm_CFL2.ShowDialog(this);
					if (frm_CFL2.ChooseItems != null && frm_CFL2.ChooseItems.Count > 0)
					{
						BindDataItem(e.RowHandle, frm_CFL2.ChooseItems);
					}
					else
					{
						gv_Line.SetFocusedRowCellValue(e.Column, DBNull.Value);
					}
				}
				else if (dataTable.Rows.Count == 1)
				{
					BindDataItem(e.RowHandle, dataTable.Rows.Cast<DataRow>().ToList());
				}
				else
				{
					gv_Line.SetFocusedRowCellValue(e.Column, DBNull.Value);
				}
			}
			else
			{
				grd_Line.DataSourceTable.Rows[gv_Line.GetFocusedDataSourceRowIndex()][e.Column.FieldName] = DBNull.Value;
				WinCommons.Error(Enums.MessageCode.NoMatchingRecordFound, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void BindDataItem(int rowHandle, IReadOnlyList<DataRow> rows)
	{
		DataTable dataSourceTable = grd_Line.DataSourceTable;
		int dataSourceRowIndex = gv_Line.GetDataSourceRowIndex(rowHandle);
		if (rows == null || rows.Count <= 0)
		{
			return;
		}
		dataSourceTable.Rows.RemoveAt(dataSourceRowIndex);
		dataSourceTable.AcceptChanges();
		int num = 0;
		if (dataSourceTable.IsNotNull())
		{
			num = dataSourceTable.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["LineNum"])) + 1;
		}
		for (int num2 = 0; num2 < rows.Count; num2++)
		{
			DataRow dataRow = dataSourceTable.NewRow();
			string text = Function.ToString(rows[num2]["ManBatchSerial"]);
			if (text == "B" || text == "S")
			{
				dataRow["ItemCode"] = Function.ToString(rows[num2]["ItemCode"]);
				dataRow["Dscription"] = Function.ToString(rows[num2]["Dscription"]);
				dataRow["DistNumber"] = Function.ToString(rows[num2]["DistNumber"]);
				dataRow["Quantity"] = Function.ParseDouble(rows[num2]["Quantity"]);
				dataRow["UoMCode"] = Function.ToString(rows[num2]["UoMCode"]);
				dataRow["UoMName"] = Function.ToString(rows[num2]["UoMName"]);
				dataRow["NumPerMsr"] = Function.ParseInt(rows[num2]["NumPerMsr"]);
				dataRow["WhsCode"] = Function.ToString(rows[num2]["WhsCode"]);
				dataRow["TransType"] = Function.ToString(rows[num2]["TransType"]);
				dataRow["TransNo"] = Function.ParseInt(rows[num2]["TransNo"]);
				dataRow["TransRow"] = Function.ToString(rows[num2]["TransRow"]);
				int num3 = Function.ParseInt(rows[num2]["PickNo"]);
				if (num3 == 0)
				{
					dataRow["PickNo"] = DBNull.Value;
				}
				else
				{
					dataRow["PickNo"] = num3;
				}
				int num4 = Function.ParseInt(rows[num2]["PackNo"]);
				int num5 = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("DocEntry"));
				if (num4 == num5)
				{
					WinCommons.Error(Enums.MessageCode.CouldNotBeAddedAddingContentWouldCauseCirularDependency, "", gv_Line.Columns["PackNo"].Caption);
					grd_Line.DataSourceTable.Rows[gv_Line.GetFocusedDataSourceRowIndex()]["PackNo"] = DBNull.Value;
					return;
				}
				if (num4 == 0)
				{
					dataRow["PackNo"] = DBNull.Value;
				}
				else
				{
					dataRow["PackNo"] = num4;
				}
				dataRow["LineNum"] = num + num2;
				dataRow["LineStatus"] = "O";
				dataSourceTable.Rows.InsertAt(dataRow, dataSourceRowIndex);
			}
			else
			{
				gv_Line.SetFocusedRowCellValue("ItemCode", Function.ToString(rows[num2]["ItemCode"]));
			}
			NewLineBlank(-1);
		}
		dataSourceTable.AcceptChanges();
		FormMode = ((FormMode == Enums.FormViewModeType.View) ? Enums.FormViewModeType.Update : Enums.FormViewModeType.Add);
	}

	private void gv_Line_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			string text = Function.ToString(gv_Doc.GetFocusedRowCellValue("DocStatus"));
			if (text != "O" && !string.IsNullOrEmpty(text))
			{
				e.Appearance.BackColor = GridColor.GridReadOnly;
				foreach (GridColumn item in gv_Line.Columns.Where((GridColumn t) => !t.OptionsColumn.ReadOnly))
				{
					item.OptionsColumn.ReadOnly = true;
				}
				rep_BaseNum.Buttons[0].Visible = false;
				rep_PickNo.Buttons[0].Visible = false;
				rep_ItemCode.Buttons[0].Visible = false;
				rep_BatchNumber.Buttons[0].Visible = false;
				return;
			}
			foreach (GridColumn item2 in gv_Line.Columns.Where((GridColumn t) => !ReadOnlyLineCols.Contains(t)))
			{
				item2.OptionsColumn.ReadOnly = false;
			}
			if (e.Column.OptionsColumn.ReadOnly)
			{
				e.Appearance.BackColor = GridColor.GridReadOnly;
				return;
			}
			rep_BaseNum.Buttons[0].Visible = true;
			rep_PickNo.Buttons[0].Visible = true;
			rep_ItemCode.Buttons[0].Visible = true;
			rep_BatchNumber.Buttons[0].Visible = true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rep_ItemCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index != 0 && e.Button.Index == 1)
			{
				string id = Function.ToString(gv_Line.GetFocusedRowCellValue("ItemCode"));
				this.DrillDown(ApzObjectTypes.oItems, ApzObjectTypes.oItems, id);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rep_WhsCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			this.DrillDown(ApzObjectTypes.oWarehouses, ApzObjectTypes.oWarehouses, Function.ToString(gv_Line.GetFocusedRowCellValue("WhsCode")));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rep_BatchNumber_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 1)
			{
				this.DrillDown(ApzObjectTypes.oBatchDetails, ApzObjectTypes.oBatchDetails, Function.ToString(gv_Line.GetFocusedRowCellValue("BatchSerialEntry")));
			}
			else if (e.Button.Index == 0)
			{
				string txtSeach = Function.ToString(gv_Line.GetFocusedRowCellValue("DistNumber"));
				frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.PickingBatch, true, txtSeach, "", Enums.ItemObjectType.NoItemType, null, "");
				frm_CFL2.ShowDialog(this);
				BindDataItem(gv_Line.FocusedRowHandle, frm_CFL2.ChooseItems);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rep_PickNo_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				string txtSeach = Function.ToString(gv_Line.GetFocusedRowCellValue("PickNo"));
				frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.PickList, true, txtSeach, "", Enums.ItemObjectType.NoItemType, null, "");
				frm_CFL2.ShowDialog(this);
				BindDataItem(gv_Line.FocusedRowHandle, frm_CFL2.ChooseItems);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rep_CardCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			string text = Function.ToString(gv_Doc.GetFocusedRowCellValue("CardCode"));
			if (e.Button.Index == 0 && "O" == Function.ToString(gv_Doc.GetFocusedRowCellValue("DocStatus")))
			{
				frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.Customer, false, text, "", Enums.ItemObjectType.NoItemType, null, "");
				frm_CFL2.ShowDialog(this);
				if (frm_CFL2.ChooseItem != null)
				{
					gv_Doc.SetFocusedRowCellValue("CardCode", Function.ToString(frm_CFL2.ChooseItem["CardCode"]));
				}
			}
			else if (e.Button.Index == 1)
			{
				this.DrillDown(ApzObjectTypes.oBusinessPartners, ApzObjectTypes.oBusinessPartners, text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task gv_Doc_CellValueChangedAsync(CellValueChangedEventArgs e)
	{
		try
		{
			if (!(e.Column.FieldName == "CardCode"))
			{
				return;
			}
			string text = Function.ToString(gv_Doc.GetFocusedRowCellValue("CardCode"));
			if (!string.IsNullOrEmpty(text))
			{
				DataTable dataTable = await _httpClient.BusinessMaster.GetADoc(text);
				if (dataTable.IsNotNull())
				{
					DataTable dataSourceTable = grd_Doc.DataSourceTable;
					int focusedDataSourceRowIndex = gv_Doc.GetFocusedDataSourceRowIndex();
					dataSourceTable.Rows[focusedDataSourceRowIndex]["CardName"] = Function.ToString(dataTable.Rows[0]["CardName"]);
					dataSourceTable.Rows[focusedDataSourceRowIndex]["ShipTo"] = Function.ToString(dataTable.Rows[0]["ShipToDef"]);
					dataSourceTable.AcceptChanges();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btn_OK_Click(object sender, EventArgs e)
	{
		Enums.FormViewModeType currentView = FormMode;
		try
		{
			if (btn_OK.Text == Apzon.Commons.Properties.Resources.ButtonOK && FormMode == Enums.FormViewModeType.View)
			{
				Close();
			}
			else
			{
				FormMode = Enums.FormViewModeType.Busy;
				DataRow dataRow = gv_Doc.GetDataRow(gv_Doc.GetFocusedDataSourceRowIndex());
				if (dataRow != null)
				{
					DataTable dataTable = grd_Doc.DataSourceTable.Clone();
					dataTable.ImportRow(dataRow);
					if (dataTable.IsNotNull())
					{
						DataTable dataSourceTable = grd_Line.DataSourceTable;
						dataSourceTable.TableName = TableLine;
						DataSet dataSet = new DataSet();
						DataTable dataTable2 = dataTable.Copy();
						if (currentView == Enums.FormViewModeType.Add)
						{
							dataTable2.Rows[0]["DocEntry"] = 0;
						}
						dataTable2.TableName = TableDoc;
						dataSet.Tables.Add(dataTable2);
						dataSet.Tables.Add(dataSourceTable.Copy());
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
						}
						else
						{
							WinCommons.Success(Enums.MessageCode.SuccessWithDocNo, "", Function.ToString(httpResult.Content));
							await Search();
						}
						return;
					}
					Logging.Write("ERROR", "Could not get header data. Data is empty");
				}
				else
				{
					Logging.Write("ERROR", "Could not get header data");
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = currentView;
	}

	private void rep_BaseNum_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			string text = Function.ToString(gv_Line.GetFocusedRowCellValue("PackNo"));
			if (e.Button.Index == 0)
			{
				frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.Packing, true, text, "", Enums.ItemObjectType.NoItemType, null, "");
				frm_CFL2.ShowDialog(this);
				BindDataItem(gv_Line.FocusedRowHandle, frm_CFL2.ChooseItems);
			}
			else if (e.Button.Index == 1)
			{
				this.DrillDown(ApzObjectTypes.oPacking, text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void addRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			NewLineBlank(-1);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void duplicateRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			DataRow dataRow = gv_Line.GetDataRow(gv_Line.GetFocusedDataSourceRowIndex());
			if (dataRow != null)
			{
				DataTable dataSourceTable = grd_Line.DataSourceTable;
				if (dataSourceTable.IsNotNull())
				{
					DataRow dataRow2 = dataSourceTable.NewRow();
					dataRow2.ItemArray = dataRow.ItemArray;
					dataSourceTable.Rows.InsertAt(dataRow2, gv_Line.FocusedRowHandle);
					dataSourceTable.AcceptChanges();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void insertRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		NewLineBlank(gv_Line.FocusedRowHandle);
	}

	private void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			gv_Line.DeleteSelectedRows();
			grd_Line.DataSourceTable.AcceptChanges();
			if (FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void duplicateToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable line = grd_Line.DataSourceTable.Copy();
			await NewDoc();
			for (int i = 0; i < line.Rows.Count; i++)
			{
				line.Rows[i]["DocEntry"] = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("DocEntry"));
				line.Rows[i]["LineStatus"] = 0;
				line.Rows[i]["OpenQty"] = Function.ParseDouble(line.Rows[i]["Quantity"]);
			}
			line.AcceptChanges();
			grd_Line.DataSource = line;
			grd_Line.RefreshDataSource();
			FormMode = Enums.FormViewModeType.Add;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void closeToolStripMenuItem_Click(object sender, EventArgs e)
	{
		WinCommons._OpenProcessing();
		Enums.FormViewModeType currentMode = FormMode;
		try
		{
			int num = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("DocEntry"));
			if (num > 0)
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCloseItem, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
				{
					return;
				}
				FormMode = Enums.FormViewModeType.Busy;
				HttpResult httpResult = await Close(num);
				if (!string.IsNullOrEmpty(httpResult.Message))
				{
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				}
				else
				{
					WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
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

	private void ctx_Doc_Opening(object sender, CancelEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Add)
			{
				addToolStripMenuItem.Enabled = false;
				closeToolStripMenuItem.Enabled = false;
				cancelToolStripMenuItem.Enabled = false;
				duplicateToolStripMenuItem.Enabled = false;
			}
			else if (FormMode == Enums.FormViewModeType.Update || FormMode == Enums.FormViewModeType.View)
			{
				string text = Function.ToString(gv_Doc.GetFocusedRowCellValue("DocStatus"));
				addToolStripMenuItem.Enabled = true;
				closeToolStripMenuItem.Enabled = text == "O";
				cancelToolStripMenuItem.Enabled = text == "O";
				duplicateToolStripMenuItem.Enabled = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void cancelToolStripMenuItem_Click(object sender, EventArgs e)
	{
		WinCommons._OpenProcessing();
		Enums.FormViewModeType currentMode = FormMode;
		try
		{
			int num = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("DocEntry"));
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

	private void gv_Doc_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			string text = Function.ToString(gv_Doc.GetRowCellValue(e.RowHandle, "DocStatus"));
			if (text != "O" && !string.IsNullOrEmpty(text))
			{
				e.Appearance.BackColor = GridColor.GridReadOnly;
				{
					foreach (GridColumn item in gv_Doc.Columns.Where((GridColumn t) => !t.OptionsColumn.ReadOnly))
					{
						item.OptionsColumn.ReadOnly = true;
					}
					return;
				}
			}
			foreach (GridColumn item2 in gv_Doc.Columns.Where((GridColumn t) => !ReadOnlyDocCols.Contains(t)))
			{
				item2.OptionsColumn.ReadOnly = false;
			}
			if (e.Column.OptionsColumn.ReadOnly)
			{
				e.Appearance.BackColor = GridColor.GridReadOnly;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ctx_Line_Opening(object sender, CancelEventArgs e)
	{
		try
		{
			baseDocumentToolStripMenuItem.Enabled = Function.ParseInt(gv_Line.GetFocusedRowCellValue("BaseType")) > 0 && Function.ParseInt(gv_Line.GetFocusedRowCellValue("BaseEntry")) > 0;
			string text = Function.ToString(gv_Doc.GetFocusedRowCellValue("DocStatus"));
			addRowToolStripMenuItem.Enabled = text == "O";
			insertRowToolStripMenuItem.Enabled = text == "O";
			duplicateRowToolStripMenuItem.Enabled = text == "O";
			deleteRowToolStripMenuItem.Enabled = text == "O";
			string text2 = Function.ToString(gv_Line.GetFocusedRowCellValue("LineStatus"));
			deleteRowToolStripMenuItem.Enabled = text2 == "O";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void baseDocumentToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			int num = Function.ParseInt(gv_Line.GetFocusedRowCellValue("BaseType"));
			if (num > 0)
			{
				this.DrillDown((ApzObjectTypes)num, Function.ParseInt(gv_Line.GetFocusedRowCellValue("BaseEntry")).ToString());
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rep_TransNo_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			int num = Function.ParseInt(gv_Line.GetFocusedRowCellValue("TransType"));
			if (num > 0)
			{
				int num2 = Function.ParseInt(gv_Line.GetFocusedRowCellValue("TransNo"));
				if (num2 > 0)
				{
					this.DrillDown((ApzObjectTypes)num, num2.ToString());
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.NoMatchingRecordFound, "");
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rep_Vehicle_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0 && "O" == Function.ToString(gv_Doc.GetFocusedRowCellValue("DocStatus")))
			{
				frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.Vehicle, false, "", "", Enums.ItemObjectType.NoItemType, null, "");
				frm_CFL2.ShowDialog(this);
				if (frm_CFL2.ChooseItem != null)
				{
					gv_Doc.SetFocusedRowCellValue("Vehicle", Function.ToString(frm_CFL2.ChooseItem["Code"]));
				}
			}
			else if (e.Button.Index == 1)
			{
				this.DrillDown(ApzObjectTypes.oVehicle, Function.ToString(gv_Doc.GetFocusedRowCellValue("Vehicle")));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
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

	protected void InitializeComponent()
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Interfaces.FormInventory.VirShipping));
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
		this.repositoryItemCalcEditEx1 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx2 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx3 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx4 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx5 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.ctx_Doc = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
		this.duplicateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
		this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.cancelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.col_DocEntry = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx28 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_DocStatus = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx3 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_CardCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gridColumnEx4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx5 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx6 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx7 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Vehicle = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gridColumnEx40 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx39 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx8 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx9 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx10 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx11 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx12 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx13 = new Apzon.Controls.Grid.GridColumnEx();
		this.xtraTabControl1 = new Apzon.Controls.Controls.XtraTabControlEx();
		this.tabpage_Contents = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_Line = new Apzon.Controls.Grid.GridControlEx();
		this.ctx_Line = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.insertRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
		this.duplicateRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
		this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
		this.baseDocumentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.targetDocumentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Line = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx14 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_BaseNum = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gridColumnEx15 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_PickNo = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gridColumnEx16 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_ItemCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gridColumnEx17 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_BatchNumber = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gridColumnEx18 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx19 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx23 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_WhsCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx20 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx21 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx22 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx30 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_UseBaseUn = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx24 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_TransType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx25 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_TransNo = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gridColumnEx26 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx27 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx29 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx31 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx32 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx33 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx34 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx35 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx36 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx37 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx38 = new Apzon.Controls.Grid.GridColumnEx();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.date_From = new Apzon.Controls.Editor.DateEditEx();
		this.date_To = new Apzon.Controls.Editor.DateEditEx();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.txt_CardCode = new Apzon.Forms.Interfaces.ChooseFromList.VirChooseFromList();
		this.lbl_DocNum = new Apzon.Controls.Controls.LabelLine();
		this.txt_DocNum = new Apzon.Controls.Editor.TextEditEx();
		this.btn_Search = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.drlCopyTo = new Apzon.Controls.Controls.DropDownButtonEx();
		this.drlCopyFrom = new Apzon.Controls.Controls.DropDownButtonEx();
		this.look_Status = new DevExpress.XtraEditors.LookUpEdit();
		this.labelLine5 = new Apzon.Controls.Controls.LabelLine();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		this.ctx_Doc.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_DocStatus).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CardCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Vehicle).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).BeginInit();
		this.xtraTabControl1.SuspendLayout();
		this.tabpage_Contents.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Line).BeginInit();
		this.ctx_Line.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Line).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_BaseNum).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_PickNo).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ItemCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_BatchNumber).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_WhsCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_UseBaseUn).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_TransType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_TransNo).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_From.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_From.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_To.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_To.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_CardCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_DocNum.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.look_Status.Properties).BeginInit();
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
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(2, 58);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.grd_Doc);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.xtraTabControl1);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(978, 477);
		this.splitContainerControl1.SplitterPosition = 160;
		this.splitContainerControl1.TabIndex = 0;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.ContextMenuStrip = this.ctx_Doc;
		this.grd_Doc.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Doc.Location = new System.Drawing.Point(0, 0);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.rep_DocStatus, this.rep_CardCode, this.rep_Vehicle });
		this.grd_Doc.Size = new System.Drawing.Size(978, 160);
		this.grd_Doc.TabIndex = 0;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.ctx_Doc.Items.AddRange(new System.Windows.Forms.ToolStripItem[6] { this.addToolStripMenuItem, this.toolStripSeparator2, this.duplicateToolStripMenuItem, this.toolStripSeparator1, this.closeToolStripMenuItem, this.cancelToolStripMenuItem });
		this.ctx_Doc.Name = "ctx_Doc";
		this.ctx_Doc.Size = new System.Drawing.Size(125, 104);
		this.ctx_Doc.Opening += new System.ComponentModel.CancelEventHandler(this.ctx_Doc_Opening);
		this.addToolStripMenuItem.Name = "addToolStripMenuItem";
		this.addToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
		this.addToolStripMenuItem.Text = "Add";
		this.addToolStripMenuItem.Click += new System.EventHandler(this.addToolStripMenuItem_Click);
		this.toolStripSeparator2.Name = "toolStripSeparator2";
		this.toolStripSeparator2.Size = new System.Drawing.Size(121, 6);
		this.duplicateToolStripMenuItem.Name = "duplicateToolStripMenuItem";
		this.duplicateToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
		this.duplicateToolStripMenuItem.Text = "Duplicate";
		this.duplicateToolStripMenuItem.Click += new System.EventHandler(this.duplicateToolStripMenuItem_Click);
		this.toolStripSeparator1.Name = "toolStripSeparator1";
		this.toolStripSeparator1.Size = new System.Drawing.Size(121, 6);
		this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
		this.closeToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
		this.closeToolStripMenuItem.Text = "Close";
		this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
		this.cancelToolStripMenuItem.Name = "cancelToolStripMenuItem";
		this.cancelToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
		this.cancelToolStripMenuItem.Text = "Cancel";
		this.cancelToolStripMenuItem.Click += new System.EventHandler(this.cancelToolStripMenuItem_Click);
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = null;
		this.gv_Doc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[16]
		{
			this.col_DocEntry, this.gridColumnEx2, this.gridColumnEx28, this.gridColumnEx3, this.gridColumnEx4, this.gridColumnEx5, this.gridColumnEx6, this.gridColumnEx7, this.gridColumnEx40, this.gridColumnEx39,
			this.gridColumnEx8, this.gridColumnEx9, this.gridColumnEx10, this.gridColumnEx11, this.gridColumnEx12, this.gridColumnEx13
		});
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = "oPacking";
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.IgnoreFilter = false;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 40;
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
		this.gv_Doc.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Doc.OptionsView.ShowGroupPanel = false;
		this.gv_Doc.UseBatchMod = false;
		this.gv_Doc.FocusedRowObjectChangedAsync += new Apzon.Controls.Grid.RaiseFocusedRowObjectChangedAsync(this.gv_Doc_FocusedRowObjectChangedAsync);
		this.gv_Doc.CellValueChangedAsync += new Apzon.Controls.Grid.CellValueChangedAsync(this.gv_Doc_CellValueChangedAsync);
		this.gv_Doc.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Doc_RowCellStyle);
		this.gv_Doc.BeforeLeaveRow += new DevExpress.XtraGrid.Views.Base.RowAllowEventHandler(this.gv_Doc_BeforeLeaveRow);
		this.col_DocEntry.Caption = "Package No.";
		this.col_DocEntry.CurrencyDateField = null;
		this.col_DocEntry.CurrencyField = null;
		this.col_DocEntry.CurrencyFixed = null;
		this.col_DocEntry.CurrencyFixedRate = 0.0;
		this.col_DocEntry.CurrencyRateField = null;
		this.col_DocEntry.FieldName = "DocEntry";
		this.col_DocEntry.Name = "col_DocEntry";
		this.col_DocEntry.OptionsColumn.ReadOnly = true;
		this.col_DocEntry.PrimaryKey = true;
		this.col_DocEntry.UnitField = null;
		this.col_DocEntry.Visible = true;
		this.col_DocEntry.VisibleIndex = 0;
		this.col_DocEntry.Width = 90;
		this.gridColumnEx2.Caption = "Document Date";
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.FieldName = "DocDate";
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.UnitField = null;
		this.gridColumnEx2.Visible = true;
		this.gridColumnEx2.VisibleIndex = 1;
		this.gridColumnEx2.Width = 101;
		this.gridColumnEx28.Caption = "Status";
		this.gridColumnEx28.ColumnEdit = this.rep_DocStatus;
		this.gridColumnEx28.CurrencyDateField = null;
		this.gridColumnEx28.CurrencyField = null;
		this.gridColumnEx28.CurrencyFixed = null;
		this.gridColumnEx28.CurrencyFixedRate = 0.0;
		this.gridColumnEx28.CurrencyRateField = null;
		this.gridColumnEx28.FieldName = "DocStatus";
		this.gridColumnEx28.Name = "gridColumnEx28";
		this.gridColumnEx28.OptionsColumn.AllowEdit = false;
		this.gridColumnEx28.PrimaryKey = false;
		this.gridColumnEx28.UnitField = null;
		this.gridColumnEx28.Visible = true;
		this.gridColumnEx28.VisibleIndex = 2;
		this.rep_DocStatus.AutoHeight = false;
		this.rep_DocStatus.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_DocStatus.Name = "rep_DocStatus";
		this.rep_DocStatus.NullText = "";
		this.gridColumnEx3.Caption = "BP Code";
		this.gridColumnEx3.ColumnEdit = this.rep_CardCode;
		this.gridColumnEx3.CurrencyDateField = null;
		this.gridColumnEx3.CurrencyField = null;
		this.gridColumnEx3.CurrencyFixed = null;
		this.gridColumnEx3.CurrencyFixedRate = 0.0;
		this.gridColumnEx3.CurrencyRateField = null;
		this.gridColumnEx3.FieldName = "CardCode";
		this.gridColumnEx3.Name = "gridColumnEx3";
		this.gridColumnEx3.PrimaryKey = false;
		this.gridColumnEx3.UnitField = null;
		this.gridColumnEx3.Visible = true;
		this.gridColumnEx3.VisibleIndex = 3;
		this.gridColumnEx3.Width = 121;
		this.rep_CardCode.AutoHeight = false;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_CardCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_CardCode.Name = "rep_CardCode";
		this.rep_CardCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_CardCode_ButtonClick);
		this.gridColumnEx4.Caption = "BP Name";
		this.gridColumnEx4.CurrencyDateField = null;
		this.gridColumnEx4.CurrencyField = null;
		this.gridColumnEx4.CurrencyFixed = null;
		this.gridColumnEx4.CurrencyFixedRate = 0.0;
		this.gridColumnEx4.CurrencyRateField = null;
		this.gridColumnEx4.FieldName = "CardName";
		this.gridColumnEx4.Name = "gridColumnEx4";
		this.gridColumnEx4.OptionsColumn.ReadOnly = true;
		this.gridColumnEx4.PrimaryKey = false;
		this.gridColumnEx4.UnitField = null;
		this.gridColumnEx4.Visible = true;
		this.gridColumnEx4.VisibleIndex = 4;
		this.gridColumnEx4.Width = 171;
		this.gridColumnEx5.Caption = "Ship To";
		this.gridColumnEx5.CurrencyDateField = null;
		this.gridColumnEx5.CurrencyField = null;
		this.gridColumnEx5.CurrencyFixed = null;
		this.gridColumnEx5.CurrencyFixedRate = 0.0;
		this.gridColumnEx5.CurrencyRateField = null;
		this.gridColumnEx5.FieldName = "ShipTo";
		this.gridColumnEx5.Name = "gridColumnEx5";
		this.gridColumnEx5.PrimaryKey = false;
		this.gridColumnEx5.UnitField = null;
		this.gridColumnEx5.Visible = true;
		this.gridColumnEx5.VisibleIndex = 5;
		this.gridColumnEx5.Width = 100;
		this.gridColumnEx6.Caption = "ToGate";
		this.gridColumnEx6.CurrencyDateField = null;
		this.gridColumnEx6.CurrencyField = null;
		this.gridColumnEx6.CurrencyFixed = null;
		this.gridColumnEx6.CurrencyFixedRate = 0.0;
		this.gridColumnEx6.CurrencyRateField = null;
		this.gridColumnEx6.FieldName = "ToGate";
		this.gridColumnEx6.Name = "gridColumnEx6";
		this.gridColumnEx6.PrimaryKey = false;
		this.gridColumnEx6.UnitField = null;
		this.gridColumnEx6.Visible = true;
		this.gridColumnEx6.VisibleIndex = 6;
		this.gridColumnEx6.Width = 100;
		this.gridColumnEx7.Caption = "Vehicle";
		this.gridColumnEx7.ColumnEdit = this.rep_Vehicle;
		this.gridColumnEx7.CurrencyDateField = null;
		this.gridColumnEx7.CurrencyField = null;
		this.gridColumnEx7.CurrencyFixed = null;
		this.gridColumnEx7.CurrencyFixedRate = 0.0;
		this.gridColumnEx7.CurrencyRateField = null;
		this.gridColumnEx7.FieldName = "Vehicle";
		this.gridColumnEx7.Name = "gridColumnEx7";
		this.gridColumnEx7.PrimaryKey = false;
		this.gridColumnEx7.UnitField = null;
		this.gridColumnEx7.Visible = true;
		this.gridColumnEx7.VisibleIndex = 9;
		this.gridColumnEx7.Width = 100;
		this.rep_Vehicle.AutoHeight = false;
		editorButtonImageOptions2.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_Vehicle.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_Vehicle.Name = "rep_Vehicle";
		this.rep_Vehicle.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_Vehicle_ButtonClick);
		this.gridColumnEx40.Caption = "Container No.";
		this.gridColumnEx40.CurrencyDateField = null;
		this.gridColumnEx40.CurrencyField = null;
		this.gridColumnEx40.CurrencyFixed = null;
		this.gridColumnEx40.CurrencyFixedRate = 0.0;
		this.gridColumnEx40.CurrencyRateField = null;
		this.gridColumnEx40.FieldName = "ContainerNo";
		this.gridColumnEx40.Name = "gridColumnEx40";
		this.gridColumnEx40.PrimaryKey = false;
		this.gridColumnEx40.UnitField = null;
		this.gridColumnEx40.Visible = true;
		this.gridColumnEx40.VisibleIndex = 7;
		this.gridColumnEx39.Caption = "Container Id";
		this.gridColumnEx39.CurrencyDateField = null;
		this.gridColumnEx39.CurrencyField = null;
		this.gridColumnEx39.CurrencyFixed = null;
		this.gridColumnEx39.CurrencyFixedRate = 0.0;
		this.gridColumnEx39.CurrencyRateField = null;
		this.gridColumnEx39.FieldName = "Container";
		this.gridColumnEx39.Name = "gridColumnEx39";
		this.gridColumnEx39.PrimaryKey = false;
		this.gridColumnEx39.UnitField = null;
		this.gridColumnEx39.Visible = true;
		this.gridColumnEx39.VisibleIndex = 8;
		this.gridColumnEx8.Caption = "P.I.C";
		this.gridColumnEx8.CurrencyDateField = null;
		this.gridColumnEx8.CurrencyField = null;
		this.gridColumnEx8.CurrencyFixed = null;
		this.gridColumnEx8.CurrencyFixedRate = 0.0;
		this.gridColumnEx8.CurrencyRateField = null;
		this.gridColumnEx8.FieldName = "PIC";
		this.gridColumnEx8.Name = "gridColumnEx8";
		this.gridColumnEx8.PrimaryKey = false;
		this.gridColumnEx8.UnitField = null;
		this.gridColumnEx8.Visible = true;
		this.gridColumnEx8.VisibleIndex = 10;
		this.gridColumnEx8.Width = 100;
		this.gridColumnEx9.Caption = "Route ID";
		this.gridColumnEx9.CurrencyDateField = null;
		this.gridColumnEx9.CurrencyField = null;
		this.gridColumnEx9.CurrencyFixed = null;
		this.gridColumnEx9.CurrencyFixedRate = 0.0;
		this.gridColumnEx9.CurrencyRateField = null;
		this.gridColumnEx9.FieldName = "RouteId";
		this.gridColumnEx9.Name = "gridColumnEx9";
		this.gridColumnEx9.PrimaryKey = false;
		this.gridColumnEx9.UnitField = null;
		this.gridColumnEx9.Visible = true;
		this.gridColumnEx9.VisibleIndex = 11;
		this.gridColumnEx9.Width = 100;
		this.gridColumnEx10.Caption = "Route order";
		this.gridColumnEx10.CurrencyDateField = null;
		this.gridColumnEx10.CurrencyField = null;
		this.gridColumnEx10.CurrencyFixed = null;
		this.gridColumnEx10.CurrencyFixedRate = 0.0;
		this.gridColumnEx10.CurrencyRateField = null;
		this.gridColumnEx10.FieldName = "RouteOrder";
		this.gridColumnEx10.Name = "gridColumnEx10";
		this.gridColumnEx10.PrimaryKey = false;
		this.gridColumnEx10.UnitField = null;
		this.gridColumnEx10.Visible = true;
		this.gridColumnEx10.VisibleIndex = 12;
		this.gridColumnEx10.Width = 100;
		this.gridColumnEx11.Caption = "Route Time";
		this.gridColumnEx11.CurrencyDateField = null;
		this.gridColumnEx11.CurrencyField = null;
		this.gridColumnEx11.CurrencyFixed = null;
		this.gridColumnEx11.CurrencyFixedRate = 0.0;
		this.gridColumnEx11.CurrencyRateField = null;
		this.gridColumnEx11.FieldName = "RouteTime";
		this.gridColumnEx11.Name = "gridColumnEx11";
		this.gridColumnEx11.PrimaryKey = false;
		this.gridColumnEx11.UnitField = null;
		this.gridColumnEx11.Visible = true;
		this.gridColumnEx11.VisibleIndex = 13;
		this.gridColumnEx11.Width = 100;
		this.gridColumnEx12.Caption = "Route Distance";
		this.gridColumnEx12.CurrencyDateField = null;
		this.gridColumnEx12.CurrencyField = null;
		this.gridColumnEx12.CurrencyFixed = null;
		this.gridColumnEx12.CurrencyFixedRate = 0.0;
		this.gridColumnEx12.CurrencyRateField = null;
		this.gridColumnEx12.FieldName = "RouteDist";
		this.gridColumnEx12.Name = "gridColumnEx12";
		this.gridColumnEx12.PrimaryKey = false;
		this.gridColumnEx12.UnitField = null;
		this.gridColumnEx12.Visible = true;
		this.gridColumnEx12.VisibleIndex = 14;
		this.gridColumnEx12.Width = 100;
		this.gridColumnEx13.Caption = "Comments";
		this.gridColumnEx13.CurrencyDateField = null;
		this.gridColumnEx13.CurrencyField = null;
		this.gridColumnEx13.CurrencyFixed = null;
		this.gridColumnEx13.CurrencyFixedRate = 0.0;
		this.gridColumnEx13.CurrencyRateField = null;
		this.gridColumnEx13.FieldName = "Comments";
		this.gridColumnEx13.Name = "gridColumnEx13";
		this.gridColumnEx13.PrimaryKey = false;
		this.gridColumnEx13.UnitField = null;
		this.gridColumnEx13.Visible = true;
		this.gridColumnEx13.VisibleIndex = 15;
		this.gridColumnEx13.Width = 100;
		this.xtraTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.xtraTabControl1.IsSearchControl = false;
		this.xtraTabControl1.Location = new System.Drawing.Point(0, 0);
		this.xtraTabControl1.Name = "xtraTabControl1";
		this.xtraTabControl1.SelectedTabPage = this.tabpage_Contents;
		this.xtraTabControl1.Size = new System.Drawing.Size(978, 307);
		this.xtraTabControl1.TabIndex = 0;
		this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[1] { this.tabpage_Contents });
		this.tabpage_Contents.Controls.Add(this.grd_Line);
		this.tabpage_Contents.EditControl = null;
		this.tabpage_Contents.EditValue = "Contents";
		this.tabpage_Contents.FieldName = null;
		this.tabpage_Contents.IsSearchControl = false;
		this.tabpage_Contents.Name = "tabpage_Contents";
		this.tabpage_Contents.ReadOnly = false;
		this.tabpage_Contents.SettingInVisible = false;
		this.tabpage_Contents.SettingReadonly = false;
		this.tabpage_Contents.Size = new System.Drawing.Size(976, 282);
		this.tabpage_Contents.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tabpage_Contents.Text = "Contents";
		this.grd_Line.AllowPasteExcel = true;
		this.grd_Line.ClipboardData = null;
		this.grd_Line.ContextMenuStrip = this.ctx_Line;
		this.grd_Line.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Line.Location = new System.Drawing.Point(0, 0);
		this.grd_Line.MainView = this.gv_Line;
		this.grd_Line.Name = "grd_Line";
		this.grd_Line.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[8] { this.rep_BaseNum, this.rep_PickNo, this.rep_ItemCode, this.rep_BatchNumber, this.rep_WhsCode, this.rep_TransType, this.rep_TransNo, this.rep_UseBaseUn });
		this.grd_Line.Size = new System.Drawing.Size(976, 282);
		this.grd_Line.TabIndex = 1;
		this.grd_Line.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Line });
		this.ctx_Line.Items.AddRange(new System.Windows.Forms.ToolStripItem[9] { this.addRowToolStripMenuItem, this.insertRowToolStripMenuItem, this.toolStripSeparator3, this.duplicateRowToolStripMenuItem, this.toolStripSeparator4, this.deleteRowToolStripMenuItem, this.toolStripSeparator5, this.baseDocumentToolStripMenuItem, this.targetDocumentToolStripMenuItem });
		this.ctx_Line.Name = "ctx_Line";
		this.ctx_Line.Size = new System.Drawing.Size(166, 154);
		this.ctx_Line.Opening += new System.ComponentModel.CancelEventHandler(this.ctx_Line_Opening);
		this.addRowToolStripMenuItem.Name = "addRowToolStripMenuItem";
		this.addRowToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
		this.addRowToolStripMenuItem.Text = "Add Row";
		this.addRowToolStripMenuItem.Click += new System.EventHandler(this.addRowToolStripMenuItem_Click);
		this.insertRowToolStripMenuItem.Name = "insertRowToolStripMenuItem";
		this.insertRowToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
		this.insertRowToolStripMenuItem.Text = "Insert Row";
		this.insertRowToolStripMenuItem.Click += new System.EventHandler(this.insertRowToolStripMenuItem_Click);
		this.toolStripSeparator3.Name = "toolStripSeparator3";
		this.toolStripSeparator3.Size = new System.Drawing.Size(162, 6);
		this.duplicateRowToolStripMenuItem.Name = "duplicateRowToolStripMenuItem";
		this.duplicateRowToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
		this.duplicateRowToolStripMenuItem.Text = "Duplicate Row";
		this.duplicateRowToolStripMenuItem.Click += new System.EventHandler(this.duplicateRowToolStripMenuItem_Click);
		this.toolStripSeparator4.Name = "toolStripSeparator4";
		this.toolStripSeparator4.Size = new System.Drawing.Size(162, 6);
		this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
		this.deleteRowToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
		this.deleteRowToolStripMenuItem.Text = "Delete Row";
		this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.deleteRowToolStripMenuItem_Click);
		this.toolStripSeparator5.Name = "toolStripSeparator5";
		this.toolStripSeparator5.Size = new System.Drawing.Size(162, 6);
		this.baseDocumentToolStripMenuItem.Name = "baseDocumentToolStripMenuItem";
		this.baseDocumentToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
		this.baseDocumentToolStripMenuItem.Text = "Base Document";
		this.baseDocumentToolStripMenuItem.Click += new System.EventHandler(this.baseDocumentToolStripMenuItem_Click);
		this.targetDocumentToolStripMenuItem.Name = "targetDocumentToolStripMenuItem";
		this.targetDocumentToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
		this.targetDocumentToolStripMenuItem.Text = "Target Document";
		this.gv_Line.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Line.ColumnKey = null;
		this.gv_Line.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[25]
		{
			this.gridColumnEx14, this.gridColumnEx15, this.gridColumnEx16, this.gridColumnEx17, this.gridColumnEx18, this.gridColumnEx19, this.gridColumnEx23, this.gridColumnEx1, this.gridColumnEx20, this.gridColumnEx21,
			this.gridColumnEx22, this.gridColumnEx30, this.gridColumnEx24, this.gridColumnEx25, this.gridColumnEx26, this.gridColumnEx27, this.gridColumnEx29, this.gridColumnEx31, this.gridColumnEx32, this.gridColumnEx33,
			this.gridColumnEx34, this.gridColumnEx35, this.gridColumnEx36, this.gridColumnEx37, this.gridColumnEx38
		});
		this.gv_Line.DisableFormSetting = false;
		this.gv_Line.DisplayException = false;
		this.gv_Line.EditColumn = null;
		this.gv_Line.FieldCurrency = null;
		this.gv_Line.FieldRate = null;
		this.gv_Line.FormId = "oPacking";
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
		this.gv_Line.CellValueChangedAsync += new Apzon.Controls.Grid.CellValueChangedAsync(this.gv_Line_CellValueChangedAsync);
		this.gv_Line.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Line_RowCellStyle);
		this.gridColumnEx14.Caption = "Package No.";
		this.gridColumnEx14.ColumnEdit = this.rep_BaseNum;
		this.gridColumnEx14.CurrencyDateField = null;
		this.gridColumnEx14.CurrencyField = null;
		this.gridColumnEx14.CurrencyFixed = null;
		this.gridColumnEx14.CurrencyFixedRate = 0.0;
		this.gridColumnEx14.CurrencyRateField = null;
		this.gridColumnEx14.FieldName = "PackNo";
		this.gridColumnEx14.Name = "gridColumnEx14";
		this.gridColumnEx14.PrimaryKey = true;
		this.gridColumnEx14.UnitField = null;
		this.gridColumnEx14.Visible = true;
		this.gridColumnEx14.VisibleIndex = 0;
		this.gridColumnEx14.Width = 100;
		this.rep_BaseNum.AutoHeight = false;
		editorButtonImageOptions3.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions3.Image");
		this.rep_BaseNum.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_BaseNum.Name = "rep_BaseNum";
		this.rep_BaseNum.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_BaseNum_ButtonClick);
		this.gridColumnEx15.Caption = "Picked No.";
		this.gridColumnEx15.ColumnEdit = this.rep_PickNo;
		this.gridColumnEx15.CurrencyDateField = null;
		this.gridColumnEx15.CurrencyField = null;
		this.gridColumnEx15.CurrencyFixed = null;
		this.gridColumnEx15.CurrencyFixedRate = 0.0;
		this.gridColumnEx15.CurrencyRateField = null;
		this.gridColumnEx15.FieldName = "PickNo";
		this.gridColumnEx15.Name = "gridColumnEx15";
		this.gridColumnEx15.PrimaryKey = true;
		this.gridColumnEx15.UnitField = null;
		this.gridColumnEx15.Visible = true;
		this.gridColumnEx15.VisibleIndex = 1;
		this.gridColumnEx15.Width = 100;
		this.rep_PickNo.AutoHeight = false;
		editorButtonImageOptions4.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions4.Image");
		this.rep_PickNo.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance4, appearanceHovered4, appearancePressed4, appearanceDisabled4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_PickNo.Name = "rep_PickNo";
		this.rep_PickNo.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_PickNo_ButtonClick);
		this.gridColumnEx16.Caption = "Item Code";
		this.gridColumnEx16.ColumnEdit = this.rep_ItemCode;
		this.gridColumnEx16.CurrencyDateField = null;
		this.gridColumnEx16.CurrencyField = null;
		this.gridColumnEx16.CurrencyFixed = null;
		this.gridColumnEx16.CurrencyFixedRate = 0.0;
		this.gridColumnEx16.CurrencyRateField = null;
		this.gridColumnEx16.FieldName = "ItemCode";
		this.gridColumnEx16.Name = "gridColumnEx16";
		this.gridColumnEx16.PrimaryKey = true;
		this.gridColumnEx16.UnitField = null;
		this.gridColumnEx16.Visible = true;
		this.gridColumnEx16.VisibleIndex = 2;
		this.gridColumnEx16.Width = 150;
		this.rep_ItemCode.AutoHeight = false;
		editorButtonImageOptions5.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions5.Image");
		this.rep_ItemCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions5, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance5, appearanceHovered5, appearancePressed5, appearanceDisabled5, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_ItemCode.Name = "rep_ItemCode";
		this.rep_ItemCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_ItemCode_ButtonClick);
		this.gridColumnEx17.Caption = "Batch/Serial";
		this.gridColumnEx17.ColumnEdit = this.rep_BatchNumber;
		this.gridColumnEx17.CurrencyDateField = null;
		this.gridColumnEx17.CurrencyField = null;
		this.gridColumnEx17.CurrencyFixed = null;
		this.gridColumnEx17.CurrencyFixedRate = 0.0;
		this.gridColumnEx17.CurrencyRateField = null;
		this.gridColumnEx17.FieldName = "DistNumber";
		this.gridColumnEx17.Name = "gridColumnEx17";
		this.gridColumnEx17.PrimaryKey = false;
		this.gridColumnEx17.UnitField = null;
		this.gridColumnEx17.Visible = true;
		this.gridColumnEx17.VisibleIndex = 3;
		this.gridColumnEx17.Width = 150;
		this.rep_BatchNumber.AutoHeight = false;
		editorButtonImageOptions6.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions6.Image");
		this.rep_BatchNumber.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions6, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance6, appearanceHovered6, appearancePressed6, appearanceDisabled6, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_BatchNumber.Name = "rep_BatchNumber";
		this.rep_BatchNumber.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_BatchNumber_ButtonClick);
		this.gridColumnEx18.Caption = "Sys. Number";
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
		this.gridColumnEx18.Width = 100;
		this.gridColumnEx19.Caption = "Inventory Quantity";
		this.gridColumnEx19.ColumnEdit = this.repositoryItemCalcEditEx1;
		this.gridColumnEx19.CurrencyDateField = null;
		this.gridColumnEx19.CurrencyField = null;
		this.gridColumnEx19.CurrencyFixed = null;
		this.gridColumnEx19.CurrencyFixedRate = 0.0;
		this.gridColumnEx19.CurrencyRateField = null;
		this.gridColumnEx19.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gridColumnEx19.FieldName = "Quantity";
		this.gridColumnEx19.Name = "gridColumnEx19";
		this.gridColumnEx19.PrimaryKey = false;
		this.gridColumnEx19.UnitField = null;
		this.gridColumnEx19.Visible = true;
		this.gridColumnEx19.VisibleIndex = 4;
		this.gridColumnEx19.Width = 114;
		this.gridColumnEx23.Caption = "Whs. Code";
		this.gridColumnEx23.ColumnEdit = this.rep_WhsCode;
		this.gridColumnEx23.CurrencyDateField = null;
		this.gridColumnEx23.CurrencyField = null;
		this.gridColumnEx23.CurrencyFixed = null;
		this.gridColumnEx23.CurrencyFixedRate = 0.0;
		this.gridColumnEx23.CurrencyRateField = null;
		this.gridColumnEx23.FieldName = "WhsCode";
		this.gridColumnEx23.Name = "gridColumnEx23";
		this.gridColumnEx23.OptionsColumn.ReadOnly = true;
		this.gridColumnEx23.PrimaryKey = false;
		this.gridColumnEx23.UnitField = null;
		this.gridColumnEx23.Visible = true;
		this.gridColumnEx23.VisibleIndex = 5;
		this.gridColumnEx23.Width = 100;
		this.rep_WhsCode.AutoHeight = false;
		editorButtonImageOptions7.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions7.Image");
		this.rep_WhsCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions7, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance7, appearanceHovered7, appearancePressed7, appearanceDisabled7, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_WhsCode.Name = "rep_WhsCode";
		this.rep_WhsCode.ReadOnly = true;
		this.rep_WhsCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_WhsCode_ButtonClick);
		this.gridColumnEx1.Caption = "Quantity UoM";
		this.gridColumnEx1.ColumnEdit = this.repositoryItemCalcEditEx2;
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gridColumnEx1.FieldName = "gridColumnEx1";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.OptionsColumn.ReadOnly = true;
		this.gridColumnEx1.PrimaryKey = false;
		this.gridColumnEx1.UnboundExpression = "Iif([UseBaseUn] = 'N', [Quantity], [Quantity] * [NumPerMsr])";
		this.gridColumnEx1.UnboundType = DevExpress.Data.UnboundColumnType.Decimal;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx1.Visible = true;
		this.gridColumnEx1.VisibleIndex = 6;
		this.gridColumnEx20.Caption = "UoM Code";
		this.gridColumnEx20.CurrencyDateField = null;
		this.gridColumnEx20.CurrencyField = null;
		this.gridColumnEx20.CurrencyFixed = null;
		this.gridColumnEx20.CurrencyFixedRate = 0.0;
		this.gridColumnEx20.CurrencyRateField = null;
		this.gridColumnEx20.FieldName = "UoMCode";
		this.gridColumnEx20.Name = "gridColumnEx20";
		this.gridColumnEx20.OptionsColumn.ReadOnly = true;
		this.gridColumnEx20.PrimaryKey = false;
		this.gridColumnEx20.UnitField = null;
		this.gridColumnEx20.Visible = true;
		this.gridColumnEx20.VisibleIndex = 7;
		this.gridColumnEx20.Width = 100;
		this.gridColumnEx21.Caption = "UoM Name";
		this.gridColumnEx21.CurrencyDateField = null;
		this.gridColumnEx21.CurrencyField = null;
		this.gridColumnEx21.CurrencyFixed = null;
		this.gridColumnEx21.CurrencyFixedRate = 0.0;
		this.gridColumnEx21.CurrencyRateField = null;
		this.gridColumnEx21.FieldName = "UoMName";
		this.gridColumnEx21.Name = "gridColumnEx21";
		this.gridColumnEx21.OptionsColumn.ReadOnly = true;
		this.gridColumnEx21.PrimaryKey = false;
		this.gridColumnEx21.UnitField = null;
		this.gridColumnEx21.Visible = true;
		this.gridColumnEx21.VisibleIndex = 8;
		this.gridColumnEx21.Width = 100;
		this.gridColumnEx22.Caption = "Items per Unit";
		this.gridColumnEx22.ColumnEdit = this.repositoryItemCalcEditEx3;
		this.gridColumnEx22.CurrencyDateField = null;
		this.gridColumnEx22.CurrencyField = null;
		this.gridColumnEx22.CurrencyFixed = null;
		this.gridColumnEx22.CurrencyFixedRate = 0.0;
		this.gridColumnEx22.CurrencyRateField = null;
		this.gridColumnEx22.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gridColumnEx22.FieldName = "NumPerMsr";
		this.gridColumnEx22.Name = "gridColumnEx22";
		this.gridColumnEx22.OptionsColumn.ReadOnly = true;
		this.gridColumnEx22.PrimaryKey = false;
		this.gridColumnEx22.UnitField = null;
		this.gridColumnEx22.Visible = true;
		this.gridColumnEx22.VisibleIndex = 9;
		this.gridColumnEx22.Width = 100;
		this.gridColumnEx30.Caption = "Inventory UoM";
		this.gridColumnEx30.ColumnEdit = this.rep_UseBaseUn;
		this.gridColumnEx30.CurrencyDateField = null;
		this.gridColumnEx30.CurrencyField = null;
		this.gridColumnEx30.CurrencyFixed = null;
		this.gridColumnEx30.CurrencyFixedRate = 0.0;
		this.gridColumnEx30.CurrencyRateField = null;
		this.gridColumnEx30.FieldName = "UseBaseUn";
		this.gridColumnEx30.Name = "gridColumnEx30";
		this.gridColumnEx30.OptionsColumn.ReadOnly = true;
		this.gridColumnEx30.PrimaryKey = false;
		this.gridColumnEx30.UnitField = null;
		this.gridColumnEx30.Visible = true;
		this.gridColumnEx30.VisibleIndex = 10;
		this.rep_UseBaseUn.AutoHeight = false;
		this.rep_UseBaseUn.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_UseBaseUn.Name = "rep_UseBaseUn";
		this.rep_UseBaseUn.NullText = "";
		this.gridColumnEx24.Caption = "Trans. Type";
		this.gridColumnEx24.ColumnEdit = this.rep_TransType;
		this.gridColumnEx24.CurrencyDateField = null;
		this.gridColumnEx24.CurrencyField = null;
		this.gridColumnEx24.CurrencyFixed = null;
		this.gridColumnEx24.CurrencyFixedRate = 0.0;
		this.gridColumnEx24.CurrencyRateField = null;
		this.gridColumnEx24.FieldName = "TransType";
		this.gridColumnEx24.Name = "gridColumnEx24";
		this.gridColumnEx24.OptionsColumn.ReadOnly = true;
		this.gridColumnEx24.PrimaryKey = false;
		this.gridColumnEx24.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gridColumnEx24.UnitField = null;
		this.gridColumnEx24.Visible = true;
		this.gridColumnEx24.VisibleIndex = 11;
		this.gridColumnEx24.Width = 100;
		this.rep_TransType.AutoHeight = false;
		this.rep_TransType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_TransType.Name = "rep_TransType";
		this.rep_TransType.NullText = "";
		this.gridColumnEx25.Caption = "Doc. No";
		this.gridColumnEx25.ColumnEdit = this.rep_TransNo;
		this.gridColumnEx25.CurrencyDateField = null;
		this.gridColumnEx25.CurrencyField = null;
		this.gridColumnEx25.CurrencyFixed = null;
		this.gridColumnEx25.CurrencyFixedRate = 0.0;
		this.gridColumnEx25.CurrencyRateField = null;
		this.gridColumnEx25.FieldName = "TransNo";
		this.gridColumnEx25.Name = "gridColumnEx25";
		this.gridColumnEx25.OptionsColumn.ReadOnly = true;
		this.gridColumnEx25.PrimaryKey = false;
		this.gridColumnEx25.UnitField = null;
		this.gridColumnEx25.Visible = true;
		this.gridColumnEx25.VisibleIndex = 12;
		this.gridColumnEx25.Width = 100;
		this.rep_TransNo.AutoHeight = false;
		editorButtonImageOptions8.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_TransNo.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, false, false, editorButtonImageOptions8, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance8, appearanceHovered8, appearancePressed8, appearanceDisabled8, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_TransNo.Name = "rep_TransNo";
		this.rep_TransNo.ReadOnly = true;
		this.rep_TransNo.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_TransNo_ButtonClick);
		this.gridColumnEx26.Caption = "Doc. Row";
		this.gridColumnEx26.CurrencyDateField = null;
		this.gridColumnEx26.CurrencyField = null;
		this.gridColumnEx26.CurrencyFixed = null;
		this.gridColumnEx26.CurrencyFixedRate = 0.0;
		this.gridColumnEx26.CurrencyRateField = null;
		this.gridColumnEx26.FieldName = "TransRow";
		this.gridColumnEx26.Name = "gridColumnEx26";
		this.gridColumnEx26.OptionsColumn.ReadOnly = true;
		this.gridColumnEx26.PrimaryKey = false;
		this.gridColumnEx26.UnitField = null;
		this.gridColumnEx26.Visible = true;
		this.gridColumnEx26.VisibleIndex = 13;
		this.gridColumnEx26.Width = 100;
		this.gridColumnEx27.Caption = "FreeText";
		this.gridColumnEx27.CurrencyDateField = null;
		this.gridColumnEx27.CurrencyField = null;
		this.gridColumnEx27.CurrencyFixed = null;
		this.gridColumnEx27.CurrencyFixedRate = 0.0;
		this.gridColumnEx27.CurrencyRateField = null;
		this.gridColumnEx27.FieldName = "FreeTxt";
		this.gridColumnEx27.Name = "gridColumnEx27";
		this.gridColumnEx27.PrimaryKey = false;
		this.gridColumnEx27.UnitField = null;
		this.gridColumnEx27.Visible = true;
		this.gridColumnEx27.VisibleIndex = 14;
		this.gridColumnEx27.Width = 100;
		this.gridColumnEx29.Caption = "BatchSerialEntry";
		this.gridColumnEx29.CurrencyDateField = null;
		this.gridColumnEx29.CurrencyField = null;
		this.gridColumnEx29.CurrencyFixed = null;
		this.gridColumnEx29.CurrencyFixedRate = 0.0;
		this.gridColumnEx29.CurrencyRateField = null;
		this.gridColumnEx29.FieldName = "BatchSerialEntry";
		this.gridColumnEx29.Name = "gridColumnEx29";
		this.gridColumnEx29.OptionsColumn.ReadOnly = true;
		this.gridColumnEx29.PrimaryKey = false;
		this.gridColumnEx29.UnitField = null;
		this.gridColumnEx31.Caption = "Open Quantity Uom";
		this.gridColumnEx31.ColumnEdit = this.repositoryItemCalcEditEx4;
		this.gridColumnEx31.CurrencyDateField = null;
		this.gridColumnEx31.CurrencyField = null;
		this.gridColumnEx31.CurrencyFixed = null;
		this.gridColumnEx31.CurrencyFixedRate = 0.0;
		this.gridColumnEx31.CurrencyRateField = null;
		this.gridColumnEx31.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gridColumnEx31.FieldName = "OpenQtyUom";
		this.gridColumnEx31.Name = "gridColumnEx31";
		this.gridColumnEx31.OptionsColumn.ReadOnly = true;
		this.gridColumnEx31.PrimaryKey = false;
		this.gridColumnEx31.UnitField = null;
		this.gridColumnEx31.Visible = true;
		this.gridColumnEx31.VisibleIndex = 15;
		this.gridColumnEx32.Caption = "Open Quantity";
		this.gridColumnEx32.ColumnEdit = this.repositoryItemCalcEditEx5;
		this.gridColumnEx32.CurrencyDateField = null;
		this.gridColumnEx32.CurrencyField = null;
		this.gridColumnEx32.CurrencyFixed = null;
		this.gridColumnEx32.CurrencyFixedRate = 0.0;
		this.gridColumnEx32.CurrencyRateField = null;
		this.gridColumnEx32.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gridColumnEx32.FieldName = "OpenQty";
		this.gridColumnEx32.Name = "gridColumnEx32";
		this.gridColumnEx32.OptionsColumn.ReadOnly = true;
		this.gridColumnEx32.PrimaryKey = false;
		this.gridColumnEx32.UnitField = null;
		this.gridColumnEx32.Visible = true;
		this.gridColumnEx32.VisibleIndex = 16;
		this.gridColumnEx33.Caption = "Ship To";
		this.gridColumnEx33.CurrencyDateField = null;
		this.gridColumnEx33.CurrencyField = null;
		this.gridColumnEx33.CurrencyFixed = null;
		this.gridColumnEx33.CurrencyFixedRate = 0.0;
		this.gridColumnEx33.CurrencyRateField = null;
		this.gridColumnEx33.FieldName = "ShipTo";
		this.gridColumnEx33.Name = "gridColumnEx33";
		this.gridColumnEx33.PrimaryKey = false;
		this.gridColumnEx33.UnitField = null;
		this.gridColumnEx33.Visible = true;
		this.gridColumnEx33.VisibleIndex = 17;
		this.gridColumnEx34.Caption = "Stage";
		this.gridColumnEx34.CurrencyDateField = null;
		this.gridColumnEx34.CurrencyField = null;
		this.gridColumnEx34.CurrencyFixed = null;
		this.gridColumnEx34.CurrencyFixedRate = 0.0;
		this.gridColumnEx34.CurrencyRateField = null;
		this.gridColumnEx34.FieldName = "Stage";
		this.gridColumnEx34.Name = "gridColumnEx34";
		this.gridColumnEx34.PrimaryKey = false;
		this.gridColumnEx34.UnitField = null;
		this.gridColumnEx34.Visible = true;
		this.gridColumnEx34.VisibleIndex = 18;
		this.gridColumnEx35.Caption = "Vehicle";
		this.gridColumnEx35.CurrencyDateField = null;
		this.gridColumnEx35.CurrencyField = null;
		this.gridColumnEx35.CurrencyFixed = null;
		this.gridColumnEx35.CurrencyFixedRate = 0.0;
		this.gridColumnEx35.CurrencyRateField = null;
		this.gridColumnEx35.FieldName = "Vehicle";
		this.gridColumnEx35.Name = "gridColumnEx35";
		this.gridColumnEx35.PrimaryKey = false;
		this.gridColumnEx35.UnitField = null;
		this.gridColumnEx35.Visible = true;
		this.gridColumnEx35.VisibleIndex = 19;
		this.gridColumnEx36.Caption = "P.I.C";
		this.gridColumnEx36.CurrencyDateField = null;
		this.gridColumnEx36.CurrencyField = null;
		this.gridColumnEx36.CurrencyFixed = null;
		this.gridColumnEx36.CurrencyFixedRate = 0.0;
		this.gridColumnEx36.CurrencyRateField = null;
		this.gridColumnEx36.FieldName = "PIC";
		this.gridColumnEx36.Name = "gridColumnEx36";
		this.gridColumnEx36.PrimaryKey = false;
		this.gridColumnEx36.UnitField = null;
		this.gridColumnEx36.Visible = true;
		this.gridColumnEx36.VisibleIndex = 20;
		this.gridColumnEx37.Caption = "Customer Code";
		this.gridColumnEx37.CurrencyDateField = null;
		this.gridColumnEx37.CurrencyField = null;
		this.gridColumnEx37.CurrencyFixed = null;
		this.gridColumnEx37.CurrencyFixedRate = 0.0;
		this.gridColumnEx37.CurrencyRateField = null;
		this.gridColumnEx37.FieldName = "CardCode";
		this.gridColumnEx37.Name = "gridColumnEx37";
		this.gridColumnEx37.OptionsColumn.ReadOnly = true;
		this.gridColumnEx37.PrimaryKey = false;
		this.gridColumnEx37.UnitField = null;
		this.gridColumnEx37.Visible = true;
		this.gridColumnEx37.VisibleIndex = 21;
		this.gridColumnEx38.Caption = "Customer Name";
		this.gridColumnEx38.CurrencyDateField = null;
		this.gridColumnEx38.CurrencyField = null;
		this.gridColumnEx38.CurrencyFixed = null;
		this.gridColumnEx38.CurrencyFixedRate = 0.0;
		this.gridColumnEx38.CurrencyRateField = null;
		this.gridColumnEx38.FieldName = "CardName";
		this.gridColumnEx38.Name = "gridColumnEx38";
		this.gridColumnEx38.OptionsColumn.ReadOnly = true;
		this.gridColumnEx38.PrimaryKey = false;
		this.gridColumnEx38.UnitField = null;
		this.gridColumnEx38.Visible = true;
		this.gridColumnEx38.VisibleIndex = 22;
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "From Date";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(12, 12);
		this.labelLine1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(80, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 1;
		this.labelLine1.Utext = "From Date";
		this.date_From.EditControl = null;
		this.date_From.EditValue = null;
		this.date_From.FieldName = null;
		this.date_From.Location = new System.Drawing.Point(88, 9);
		this.date_From.Name = "date_From";
		this.date_From.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_From.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_From.Properties.IsSearchControl = false;
		this.date_From.SettingInVisible = false;
		this.date_From.SettingReadonly = false;
		this.date_From.Size = new System.Drawing.Size(144, 20);
		this.date_From.TabIndex = 2;
		this.date_To.EditControl = null;
		this.date_To.EditValue = null;
		this.date_To.FieldName = null;
		this.date_To.Location = new System.Drawing.Point(88, 32);
		this.date_To.Name = "date_To";
		this.date_To.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_To.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_To.Properties.IsSearchControl = false;
		this.date_To.SettingInVisible = false;
		this.date_To.SettingReadonly = false;
		this.date_To.Size = new System.Drawing.Size(144, 20);
		this.date_To.TabIndex = 4;
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "To Date";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(12, 35);
		this.labelLine2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(80, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 3;
		this.labelLine2.Utext = "To Date";
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "Business Partner ";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(239, 12);
		this.labelLine3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(94, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 5;
		this.labelLine3.Utext = "Business Partner ";
		this.txt_CardCode.EditControl = null;
		this.txt_CardCode.FieldName = null;
		this.txt_CardCode.IsSearchControl = false;
		this.txt_CardCode.Location = new System.Drawing.Point(330, 9);
		this.txt_CardCode.Name = "txt_CardCode";
		this.txt_CardCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.txt_CardCode.SettingInVisible = false;
		this.txt_CardCode.SettingReadonly = false;
		this.txt_CardCode.Size = new System.Drawing.Size(144, 20);
		this.txt_CardCode.TabIndex = 6;
		this.lbl_DocNum.EditControl = null;
		this.lbl_DocNum.EditValue = "Package No.";
		this.lbl_DocNum.FieldName = null;
		this.lbl_DocNum.HideWhenEmpty = false;
		this.lbl_DocNum.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_DocNum.Location = new System.Drawing.Point(239, 35);
		this.lbl_DocNum.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lbl_DocNum.Name = "lbl_DocNum";
		this.lbl_DocNum.ReadOnly = false;
		this.lbl_DocNum.SettingInVisible = false;
		this.lbl_DocNum.SettingReadonly = false;
		this.lbl_DocNum.ShowUnderLine = true;
		this.lbl_DocNum.Size = new System.Drawing.Size(94, 20);
		this.lbl_DocNum.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_DocNum.TabIndex = 7;
		this.lbl_DocNum.Utext = "Package No.";
		this.txt_DocNum.EditControl = null;
		this.txt_DocNum.FieldName = null;
		this.txt_DocNum.IsSearchControl = false;
		this.txt_DocNum.Location = new System.Drawing.Point(330, 32);
		this.txt_DocNum.Name = "txt_DocNum";
		this.txt_DocNum.SettingInVisible = false;
		this.txt_DocNum.SettingReadonly = false;
		this.txt_DocNum.Size = new System.Drawing.Size(144, 20);
		this.txt_DocNum.TabIndex = 8;
		this.btn_Search.EditControl = null;
		this.btn_Search.EditValue = "Search";
		this.btn_Search.FieldName = null;
		this.btn_Search.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Search.ImageOptions.Image");
		this.btn_Search.IsSearchControl = false;
		this.btn_Search.Location = new System.Drawing.Point(724, 30);
		this.btn_Search.Name = "btn_Search";
		this.btn_Search.ReadOnly = false;
		this.btn_Search.SettingInVisible = false;
		this.btn_Search.SettingReadonly = false;
		this.btn_Search.Size = new System.Drawing.Size(99, 20);
		this.btn_Search.TabIndex = 9;
		this.btn_Search.Text = "Search";
		this.btn_Search.Click += new System.EventHandler(this.btn_Search_Click);
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.EditControl = null;
		this.btn_OK.EditValue = "OK";
		this.btn_OK.FieldName = null;
		this.btn_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_OK.ImageOptions.Image");
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(3, 542);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.ReadOnly = false;
		this.btn_OK.SettingInVisible = false;
		this.btn_OK.SettingReadonly = false;
		this.btn_OK.Size = new System.Drawing.Size(99, 20);
		this.btn_OK.TabIndex = 10;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(105, 542);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(99, 20);
		this.btn_Cancel.TabIndex = 11;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.drlCopyTo.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.drlCopyTo.Appearance.Options.UseBackColor = true;
		this.drlCopyTo.EditControl = null;
		this.drlCopyTo.EditValue = "Copy to";
		this.drlCopyTo.Enabled = false;
		this.drlCopyTo.FieldName = null;
		this.drlCopyTo.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("drlCopyTo.ImageOptions.Image");
		this.drlCopyTo.IsSearchControl = false;
		this.drlCopyTo.ItemId = null;
		this.drlCopyTo.Location = new System.Drawing.Point(857, 542);
		this.drlCopyTo.Name = "drlCopyTo";
		this.drlCopyTo.ReadOnly = false;
		this.drlCopyTo.SettingInVisible = false;
		this.drlCopyTo.SettingReadonly = false;
		this.drlCopyTo.Size = new System.Drawing.Size(116, 20);
		this.drlCopyTo.TabIndex = 27;
		this.drlCopyTo.Text = "Copy to";
		this.drlCopyFrom.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.drlCopyFrom.Appearance.Options.UseBackColor = true;
		this.drlCopyFrom.EditControl = null;
		this.drlCopyFrom.EditValue = "Copy From";
		this.drlCopyFrom.Enabled = false;
		this.drlCopyFrom.FieldName = null;
		this.drlCopyFrom.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("drlCopyFrom.ImageOptions.Image");
		this.drlCopyFrom.IsSearchControl = false;
		this.drlCopyFrom.ItemId = null;
		this.drlCopyFrom.Location = new System.Drawing.Point(741, 542);
		this.drlCopyFrom.Name = "drlCopyFrom";
		this.drlCopyFrom.ReadOnly = false;
		this.drlCopyFrom.SettingInVisible = false;
		this.drlCopyFrom.SettingReadonly = false;
		this.drlCopyFrom.Size = new System.Drawing.Size(110, 20);
		this.drlCopyFrom.TabIndex = 26;
		this.drlCopyFrom.Text = "Copy From";
		this.look_Status.Location = new System.Drawing.Point(574, 30);
		this.look_Status.Name = "look_Status";
		this.look_Status.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.look_Status.Properties.NullText = "";
		this.look_Status.Size = new System.Drawing.Size(144, 20);
		this.look_Status.TabIndex = 28;
		this.labelLine5.EditControl = null;
		this.labelLine5.EditValue = "Status";
		this.labelLine5.FieldName = null;
		this.labelLine5.HideWhenEmpty = false;
		this.labelLine5.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine5.Location = new System.Drawing.Point(480, 32);
		this.labelLine5.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine5.Name = "labelLine5";
		this.labelLine5.ReadOnly = false;
		this.labelLine5.SettingInVisible = false;
		this.labelLine5.SettingReadonly = false;
		this.labelLine5.ShowUnderLine = true;
		this.labelLine5.Size = new System.Drawing.Size(94, 20);
		this.labelLine5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine5.TabIndex = 29;
		this.labelLine5.Utext = "Status";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(979, 568);
		base.Controls.Add(this.labelLine5);
		base.Controls.Add(this.look_Status);
		base.Controls.Add(this.drlCopyTo);
		base.Controls.Add(this.drlCopyFrom);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_OK);
		base.Controls.Add(this.btn_Search);
		base.Controls.Add(this.txt_DocNum);
		base.Controls.Add(this.lbl_DocNum);
		base.Controls.Add(this.txt_CardCode);
		base.Controls.Add(this.labelLine3);
		base.Controls.Add(this.date_To);
		base.Controls.Add(this.labelLine2);
		base.Controls.Add(this.date_From);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.splitContainerControl1);
		base.FormId = "oPacking";
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("VirShipping.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "VirShipping";
		base.ShowInTaskbar = false;
		this.Text = "Packing";
		base.OnLoadAsync += new Apzon.Controls.Form.LoadAsyncEventHandler(this.VirShipping_Load);
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		this.ctx_Doc.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_DocStatus).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CardCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Vehicle).EndInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).EndInit();
		this.xtraTabControl1.ResumeLayout(false);
		this.tabpage_Contents.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Line).EndInit();
		this.ctx_Line.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Line).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_BaseNum).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_PickNo).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ItemCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_BatchNumber).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_WhsCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_UseBaseUn).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_TransType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_TransNo).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_From.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_From.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_To.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_To.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_CardCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_DocNum.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.look_Status.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
