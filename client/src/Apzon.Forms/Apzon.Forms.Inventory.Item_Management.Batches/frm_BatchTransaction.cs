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
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Administrator;
using Apzon.Forms.Commons;
using Apzon.Forms.Helper;
using Apzon.Forms.Properties;
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

namespace Apzon.Forms.Inventory.Item_Management.Batches;

public class frm_BatchTransaction : XtraFormEx
{
	private readonly Dictionary<string, string> lstStatus = new Dictionary<string, string>
	{
		{ "", " " },
		{ "0", "Released" },
		{ "1", "Not Accessible" },
		{ "2", "Locked" }
	};

	private DataTable dtDoc = new DataTable();

	private DataTable dtDocs = new DataTable();

	private IContainer components;

	private GridControlEx grd_Batch;

	private GridColumnEx DistNumber;

	private GridColumnEx MnfSerial;

	private GridColumnEx LotNumber;

	private GridColumnEx InDate;

	private RepositoryItemDateEdit rep_DatePicker;

	private GridColumnEx MnfDate;

	private GridColumnEx ExpDate;

	private GridColumnEx Notes;

	private GridColumnEx SysNumber;

	private GridColumnEx Status;

	private GridColumnEx AbsEntry;

	private GroupControl groupControl1;

	private GroupControl groupControl2;

	private SplitContainerControl splitContainerControl1;

	private LabelLine labelLine3;

	private LabelLine labelLine1;

	private GridColumnEx WhsCode;

	private GridColumnEx Quantity;

	private GridColumnEx Allocated;

	private GridControlEx grd_BatchTransaction;

	private GridColumnEx Document;

	private GridColumnEx DocLine;

	private GridColumnEx DocDate;

	private GridColumnEx LocCode;

	private GridColumnEx CardName;

	private GridColumnEx Qty;

	private GridColumnEx AllocQty;

	private GridColumnEx Direction;

	private LabelLine labelLine2;

	private GridColumnEx ItemCode;

	private GridColumnEx ItemName;

	private GridColumnEx LogEntry;

	private GridViewEx gv_Batch;

	private SimpleButtonEx btn_Search;

	private GridViewEx gv_BatchTransaction;

	private GridColumnEx gridColumn1;

	private RepositoryItemButtonEdit rep_ItemNo;

	private RepositoryItemButtonEdit rep_Whse;

	private RepositoryItemButtonEdit rep_Document;

	private RepositoryItemDateEdit rep_Date;

	private RepositoryItemButtonEdit rep_Warehorse;

	private GridColumnEx gridColumn2;

	private RepositoryItemLookUpEdit rep_Type;

	private LookUpEditEx look_Status;

	private RepositoryItemLookUpEdit rep_Status;

	private RepositoryItemCalcEdit rep_Quantity;

	private RepositoryItemCalcEdit rep_Qty;

	private LabelLine labelLine4;

	private TextEditEx txt_DistNumber;

	private ButtonEditCFL btn_ItemCodeFrom;

	private ButtonEditCFL btn_ItemCodeTo;

	private RepositoryItemButtonEditCFL rDisNumber;

	public frm_BatchTransaction()
	{
		InitializeComponent();
		Quantity.SummaryItem.DisplayFormat = "{0:n" + GlobalData.QuantitiesFormat.NumberDecimalDigits + "}";
		Allocated.SummaryItem.DisplayFormat = "{0:n" + GlobalData.QuantitiesFormat.NumberDecimalDigits + "}";
		Qty.SummaryItem.DisplayFormat = "{0:n" + GlobalData.QuantitiesFormat.NumberDecimalDigits + "}";
		AllocQty.SummaryItem.DisplayFormat = "{0:n" + GlobalData.QuantitiesFormat.NumberDecimalDigits + "}";
	}

	public frm_BatchTransaction(string _distNumber)
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_BatchDetail"))
		{
			InitializeComponent();
			txt_DistNumber.EditValue = _distNumber;
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_BatchDetail" && x.authoriza == "F"))
			{
				WinFuntions.DisableAllControl(Controls);
				btn_Search.Enabled = true;
			}
			return;
		}
		throw new Exception(Apzon.Commons.Properties.Resources.Msg_FormAccessDenied);
	}

	public async Task REFRESH_DATA()
	{
		await LoadData();
	}

	public void REFRESH()
	{
		try
		{
			btn_ItemCodeFrom.EditValue = "";
			btn_ItemCodeTo.EditValue = "";
			txt_DistNumber.EditValue = "";
			look_Status.EditValue = "";
			btn_Search.Focus();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void frm_BatchTransaction_Load(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			await WinFuntions.LoadUdfColumns(gv_Batch, "OBTN");
			string value = Function.ToString(txt_DistNumber.EditValue);
			string value2 = Function.ToString(btn_ItemCodeFrom.EditValue);
			string value3 = Function.ToString(btn_ItemCodeTo.EditValue);
			string value4 = Function.ToString(look_Status.EditValue);
			if (!string.IsNullOrEmpty(value) || !string.IsNullOrEmpty(value2) || !string.IsNullOrEmpty(value3) || !string.IsNullOrEmpty(value4))
			{
				await LoadData();
			}
			List<Common.Table_Code> list = CommonList.oDocumentNumberingCode.Select((KeyValuePair<string, string> item) => new Common.Table_Code
			{
				Code = item.Key,
				Name = item.Value
			}).ToList();
			list.Add(new Common.Table_Code("-1", "Issue for Production"));
			list.Add(new Common.Table_Code("-2", "Receipt from Production"));
			rep_Type.DataSource = list;
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL = rDisNumber;
			repositoryItemButtonEditCFL.DrillDownMethodWithGridView = (Action<string, GridView>)Delegate.Combine(repositoryItemButtonEditCFL.DrillDownMethodWithGridView, (Action<string, GridView>)((string input, GridView view) =>
			{
				this.DrillDown(ApzObjectTypes.oBatchDetails, Function.ToString(view.GetFocusedRowCellValue("AbsEntry")));
			}));
			look_Status.SetDataSource(lstStatus);
			rep_Status.SetDataSource(lstStatus);
			btn_ItemCodeFrom.Properties.RefreshDataSource();
			btn_ItemCodeTo.Properties.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task btn_Search_Click(object sender, EventArgs e)
	{
		try
		{
			await LoadData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rep_ItemNo_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			string text = Function.ToString(gv_Batch.GetFocusedRowCellValue("ItemCode"));
			if (!string.IsNullOrEmpty(text))
			{
				ExtenseionMethods.LoadBoMFromGridItem(text, MdiParent);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rep_Whse_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			this.DrillDown(ApzObjectTypes.oWarehouses, Function.ToString(gv_Batch.GetFocusedRowCellValue("WhsCode")));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void gv_Batch_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			await LoadDoc();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_BatchTransaction_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == "Direction")
			{
				if (gv_BatchTransaction.GetRowCellValue(e.RowHandle, "Direction").Equals("IN"))
				{
					e.Appearance.BackColor = Color.CadetBlue;
				}
				else if (gv_BatchTransaction.GetRowCellValue(e.RowHandle, "Direction").Equals("OUT"))
				{
					e.Appearance.BackColor = Color.Tomato;
				}
				else
				{
					e.Appearance.BackColor = Color.Gold;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_BatchTransaction_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (gv_BatchTransaction.FocusedColumn.FieldName == "DocType" || gv_BatchTransaction.FocusedColumn.FieldName == "Direction")
			{
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rep_Document_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			ApzObjectTypes apzObjectTypes = Function.ParseInt(gv_BatchTransaction.GetFocusedRowCellValue("DocType")) switch
			{
				-2 => ApzObjectTypes.oInventoryGenEntry, 
				-1 => ApzObjectTypes.oInventoryGenExit, 
				_ => (ApzObjectTypes)Function.ParseInt(gv_BatchTransaction.GetFocusedRowCellValue("DocType")), 
			};
			string id = Function.ToString(gv_BatchTransaction.GetFocusedRowCellValue("DocEntry"));
			this.DrillDown(apzObjectTypes, apzObjectTypes, id, reserve: false, Function.ParseInt(gv_BatchTransaction.GetFocusedRowCellValue("BaseType")) == 202);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rep_Warehorse_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			string whsSearchCode = Function.ToString(gv_BatchTransaction.GetFocusedRowCellValue("LocCode"));
			frm_WareHouses frm_WareHouses2 = new frm_WareHouses();
			frm_WareHouses2._whsSearchCode = whsSearchCode;
			frm_WareHouses2.ShowDialog(this);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void SetEnableControlSearch(bool isEnable)
	{
		SimpleButtonEx simpleButtonEx = btn_Search;
		ButtonEditCFL buttonEditCFL = btn_ItemCodeFrom;
		ButtonEditCFL buttonEditCFL2 = btn_ItemCodeTo;
		TextEditEx textEditEx = txt_DistNumber;
		bool flag = (look_Status.Enabled = isEnable);
		bool flag3 = (textEditEx.Enabled = flag);
		bool flag5 = (buttonEditCFL2.Enabled = flag3);
		bool enabled = (buttonEditCFL.Enabled = flag5);
		simpleButtonEx.Enabled = enabled;
	}

	private async Task LoadData()
	{
		SetEnableControlSearch(isEnable: false);
		gv_Batch.ShowLoadingPanel();
		try
		{
			grd_Batch.DataSource = null;
			grd_BatchTransaction.DataSource = null;
			dtDocs = await GetDocs();
			grd_Batch.DataSource = dtDocs;
			grd_Batch.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		gv_Batch.HideLoadingPanel();
		SetEnableControlSearch(isEnable: true);
	}

	private async Task<DataTable> GetDocs()
	{
		try
		{
			string distNumber = Function.ToString(txt_DistNumber.EditValue);
			string itemfrom = Function.ToString(btn_ItemCodeFrom.EditValue);
			string itemto = Function.ToString(btn_ItemCodeTo.EditValue);
			string status = Function.ToString(look_Status.EditValue);
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "BatchTransaction");
			return await _helper.GetDataTableAsync("GetDocs", new { distNumber, itemfrom, itemto, status });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	private async Task LoadDoc()
	{
		grd_BatchTransaction.DataSource = null;
		gv_BatchTransaction.ShowLoadingPanel();
		try
		{
			string itemCode = Function.ToString(gv_Batch.GetRowCellValue(gv_Batch.FocusedRowHandle, "ItemCode"));
			string sysNumber = Function.ToString(gv_Batch.GetRowCellValue(gv_Batch.FocusedRowHandle, "SysNumber"));
			string locCode = Function.ToString(gv_Batch.GetRowCellValue(gv_Batch.FocusedRowHandle, "LocCode"));
			dtDoc = await GetDoc(itemCode, sysNumber, locCode);
			grd_BatchTransaction.DataSource = dtDoc;
			grd_BatchTransaction.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		gv_BatchTransaction.HideLoadingPanel();
	}

	private async Task<DataTable> GetDoc(string itemCode, string sysNumber, string locCode)
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "BatchTransaction");
			return await helper.GetAsync("GetDoc", new { itemCode, sysNumber, locCode });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Inventory.Item_Management.Batches.frm_BatchTransaction));
		DevExpress.Utils.SerializableAppearanceObject appearance3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions4 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions4 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions5 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions6 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance12 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered12 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed12 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled12 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions7 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance13 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered13 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed13 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled13 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions7 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance14 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered14 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed14 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled14 = new DevExpress.Utils.SerializableAppearanceObject();
		this.grd_Batch = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Batch = new Apzon.Controls.Grid.GridViewEx();
		this.ItemCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_ItemNo = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.ItemName = new Apzon.Controls.Grid.GridColumnEx();
		this.DistNumber = new Apzon.Controls.Grid.GridColumnEx();
		this.rDisNumber = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.WhsCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Whse = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.Quantity = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Quantity = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.Allocated = new Apzon.Controls.Grid.GridColumnEx();
		this.MnfSerial = new Apzon.Controls.Grid.GridColumnEx();
		this.LotNumber = new Apzon.Controls.Grid.GridColumnEx();
		this.Status = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Status = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.ExpDate = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_DatePicker = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.MnfDate = new Apzon.Controls.Grid.GridColumnEx();
		this.InDate = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.SysNumber = new Apzon.Controls.Grid.GridColumnEx();
		this.Notes = new Apzon.Controls.Grid.GridColumnEx();
		this.AbsEntry = new Apzon.Controls.Grid.GridColumnEx();
		this.LogEntry = new Apzon.Controls.Grid.GridColumnEx();
		this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
		this.grd_BatchTransaction = new Apzon.Controls.Grid.GridControlEx();
		this.gv_BatchTransaction = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn2 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Type = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.Document = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Document = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.DocLine = new Apzon.Controls.Grid.GridColumnEx();
		this.DocDate = new Apzon.Controls.Grid.GridColumnEx();
		this.LocCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Warehorse = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.CardName = new Apzon.Controls.Grid.GridColumnEx();
		this.Qty = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Qty = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.AllocQty = new Apzon.Controls.Grid.GridColumnEx();
		this.Direction = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Date = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.groupControl2 = new DevExpress.XtraEditors.GroupControl();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.btn_Search = new Apzon.Controls.Controls.SimpleButtonEx();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.look_Status = new Apzon.Controls.Editor.LookUpEditEx();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.txt_DistNumber = new Apzon.Controls.Editor.TextEditEx();
		this.btn_ItemCodeFrom = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.btn_ItemCodeTo = new Apzon.Forms.UserControls.ButtonEditCFL();
		((System.ComponentModel.ISupportInitialize)this.grd_Batch).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Batch).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ItemNo).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rDisNumber).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Whse).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Quantity).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Status).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_DatePicker).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_DatePicker.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.groupControl1).BeginInit();
		this.groupControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_BatchTransaction).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_BatchTransaction).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Type).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Document).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Warehorse).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Qty).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Date).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Date.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.groupControl2).BeginInit();
		this.groupControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.look_Status.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_DistNumber.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_ItemCodeFrom.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_ItemCodeTo.Properties).BeginInit();
		base.SuspendLayout();
		this.grd_Batch.AllowPasteExcel = true;
		this.grd_Batch.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Batch.ClipboardData = null;
		this.grd_Batch.Location = new System.Drawing.Point(2, 21);
		this.grd_Batch.MainView = this.gv_Batch;
		this.grd_Batch.Name = "grd_Batch";
		this.grd_Batch.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[6] { this.rep_DatePicker, this.rep_ItemNo, this.rep_Whse, this.rep_Status, this.rep_Quantity, this.rDisNumber });
		this.grd_Batch.Size = new System.Drawing.Size(981, 165);
		this.grd_Batch.TabIndex = 7;
		this.grd_Batch.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Batch });
		this.gv_Batch.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Batch.ColumnKey = null;
		this.gv_Batch.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[17]
		{
			this.ItemCode, this.ItemName, this.DistNumber, this.WhsCode, this.Quantity, this.Allocated, this.MnfSerial, this.LotNumber, this.Status, this.ExpDate,
			this.MnfDate, this.InDate, this.gridColumn1, this.SysNumber, this.Notes, this.AbsEntry, this.LogEntry
		});
		this.gv_Batch.DisableFormSetting = false;
		this.gv_Batch.DisplayException = false;
		this.gv_Batch.EditColumn = null;
		this.gv_Batch.FieldCurrency = null;
		this.gv_Batch.FieldRate = null;
		this.gv_Batch.FormId = "oBatchTransaction";
		this.gv_Batch.GridControl = this.grd_Batch;
		this.gv_Batch.IgnoreFilter = false;
		this.gv_Batch.IndexEditRow = -1;
		this.gv_Batch.IndicatorWidth = 40;
		this.gv_Batch.IsApzTable = false;
		this.gv_Batch.IsInsertNewRow = false;
		this.gv_Batch.IsLoadingData = false;
		this.gv_Batch.IsMappingContextAction = false;
		this.gv_Batch.ItemId = "OBTN";
		this.gv_Batch.LstColumnAllowBatch = null;
		this.gv_Batch.LstColumnNotAllowBatch = null;
		this.gv_Batch.LstColumnNotEditable = null;
		this.gv_Batch.LstColumNotSetting = null;
		this.gv_Batch.Name = "gv_Batch";
		this.gv_Batch.OptionsBehavior.ReadOnly = true;
		this.gv_Batch.OptionsSelection.CheckBoxSelectorColumnWidth = 35;
		this.gv_Batch.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
		this.gv_Batch.OptionsView.ColumnAutoWidth = false;
		this.gv_Batch.OptionsView.ShowFooter = true;
		this.gv_Batch.OptionsView.ShowGroupPanel = false;
		this.gv_Batch.UseBatchMod = false;
		this.gv_Batch.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_Batch_FocusedRowObjectChanged);
		this.ItemCode.Caption = "Item No.";
		this.ItemCode.ColumnEdit = this.rep_ItemNo;
		this.ItemCode.CurrencyDateField = null;
		this.ItemCode.CurrencyField = null;
		this.ItemCode.CurrencyFixed = null;
		this.ItemCode.CurrencyFixedRate = 0.0;
		this.ItemCode.CurrencyRateField = null;
		this.ItemCode.FieldName = "ItemCode";
		this.ItemCode.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.ItemCode.Name = "ItemCode";
		this.ItemCode.PrimaryKey = true;
		this.ItemCode.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.ItemCode.UnitField = null;
		this.ItemCode.Visible = true;
		this.ItemCode.VisibleIndex = 0;
		this.ItemCode.Width = 108;
		this.rep_ItemNo.AutoHeight = false;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_ItemNo.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_ItemNo.Name = "rep_ItemNo";
		this.rep_ItemNo.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_ItemNo_ButtonClick);
		this.ItemName.Caption = "Item Description";
		this.ItemName.CurrencyDateField = null;
		this.ItemName.CurrencyField = null;
		this.ItemName.CurrencyFixed = null;
		this.ItemName.CurrencyFixedRate = 0.0;
		this.ItemName.CurrencyRateField = null;
		this.ItemName.FieldName = "itemName";
		this.ItemName.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.ItemName.Name = "ItemName";
		this.ItemName.PrimaryKey = false;
		this.ItemName.UnitField = null;
		this.ItemName.Visible = true;
		this.ItemName.VisibleIndex = 1;
		this.ItemName.Width = 175;
		this.DistNumber.Caption = "Batch";
		this.DistNumber.ColumnEdit = this.rDisNumber;
		this.DistNumber.CurrencyDateField = null;
		this.DistNumber.CurrencyField = null;
		this.DistNumber.CurrencyFixed = null;
		this.DistNumber.CurrencyFixedRate = 0.0;
		this.DistNumber.CurrencyRateField = null;
		this.DistNumber.FieldName = "DistNumber";
		this.DistNumber.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.DistNumber.Name = "DistNumber";
		this.DistNumber.OptionsColumn.ReadOnly = true;
		this.DistNumber.PrimaryKey = false;
		this.DistNumber.UnitField = null;
		this.DistNumber.Visible = true;
		this.DistNumber.VisibleIndex = 2;
		this.DistNumber.Width = 88;
		this.rDisNumber.AutoHeight = false;
		this.rDisNumber.AutoLoadDataSource = true;
		editorButtonImageOptions2.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions3.Image");
		this.rDisNumber.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, false, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rDisNumber.ContainerControl = this;
		this.rDisNumber.DisplayMember = "";
		this.rDisNumber.DisplayMember2 = "";
		this.rDisNumber.LoadDataSourceWhenEnter = false;
		this.rDisNumber.Name = "rDisNumber";
		this.rDisNumber.ReadOnly = true;
		this.rDisNumber.SeparateCharacterForMulti = ';';
		this.rDisNumber.SeparateString = ", ";
		this.rDisNumber.TypeDrillDown = Apzon.Commons.ApzObjectTypes.oBatchDetails;
		this.rDisNumber.UsingAutoCompleteBox = false;
		this.rDisNumber.UsingDataSource = true;
		this.rDisNumber.ValidateWhenNoDataSource = false;
		this.rDisNumber.ValueMember = "";
		this.WhsCode.Caption = "Whse";
		this.WhsCode.ColumnEdit = this.rep_Whse;
		this.WhsCode.CurrencyDateField = null;
		this.WhsCode.CurrencyField = null;
		this.WhsCode.CurrencyFixed = null;
		this.WhsCode.CurrencyFixedRate = 0.0;
		this.WhsCode.CurrencyRateField = null;
		this.WhsCode.FieldName = "LocCode";
		this.WhsCode.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.WhsCode.Name = "WhsCode";
		this.WhsCode.PrimaryKey = false;
		this.WhsCode.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.WhsCode.UnitField = null;
		this.WhsCode.Visible = true;
		this.WhsCode.VisibleIndex = 3;
		this.WhsCode.Width = 85;
		this.rep_Whse.AutoHeight = false;
		editorButtonImageOptions3.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_Whse.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance4, appearanceHovered4, appearancePressed4, appearanceDisabled4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_Whse.Name = "rep_Whse";
		this.rep_Whse.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_Whse_ButtonClick);
		this.Quantity.Caption = "Quantity";
		this.Quantity.ColumnEdit = this.rep_Quantity;
		this.Quantity.CurrencyDateField = null;
		this.Quantity.CurrencyField = null;
		this.Quantity.CurrencyFixed = null;
		this.Quantity.CurrencyFixedRate = 0.0;
		this.Quantity.CurrencyRateField = null;
		this.Quantity.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.Quantity.FieldName = "Quantity";
		this.Quantity.Name = "Quantity";
		this.Quantity.PrimaryKey = false;
		this.Quantity.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.Quantity.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[1]
		{
			new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum)
		});
		this.Quantity.UnitField = null;
		this.Quantity.Visible = true;
		this.Quantity.VisibleIndex = 4;
		this.Quantity.Width = 89;
		this.rep_Quantity.AutoHeight = false;
		this.rep_Quantity.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo, "", -1, false, true, false, imageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance5, appearanceHovered5, appearancePressed5, appearanceDisabled5, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_Quantity.Name = "rep_Quantity";
		this.Allocated.Caption = "Allocated";
		this.Allocated.ColumnEdit = this.rep_Quantity;
		this.Allocated.CurrencyDateField = null;
		this.Allocated.CurrencyField = null;
		this.Allocated.CurrencyFixed = null;
		this.Allocated.CurrencyFixedRate = 0.0;
		this.Allocated.CurrencyRateField = null;
		this.Allocated.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.Allocated.FieldName = "Allocated";
		this.Allocated.Name = "Allocated";
		this.Allocated.PrimaryKey = false;
		this.Allocated.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.Allocated.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[1]
		{
			new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum)
		});
		this.Allocated.UnitField = null;
		this.Allocated.Visible = true;
		this.Allocated.VisibleIndex = 5;
		this.Allocated.Width = 63;
		this.MnfSerial.Caption = "Batch Attribute 1";
		this.MnfSerial.CurrencyDateField = null;
		this.MnfSerial.CurrencyField = null;
		this.MnfSerial.CurrencyFixed = null;
		this.MnfSerial.CurrencyFixedRate = 0.0;
		this.MnfSerial.CurrencyRateField = null;
		this.MnfSerial.FieldName = "MnfSerial";
		this.MnfSerial.Name = "MnfSerial";
		this.MnfSerial.PrimaryKey = false;
		this.MnfSerial.UnitField = null;
		this.MnfSerial.Visible = true;
		this.MnfSerial.VisibleIndex = 6;
		this.MnfSerial.Width = 92;
		this.LotNumber.Caption = "Batch Attribute 2";
		this.LotNumber.CurrencyDateField = null;
		this.LotNumber.CurrencyField = null;
		this.LotNumber.CurrencyFixed = null;
		this.LotNumber.CurrencyFixedRate = 0.0;
		this.LotNumber.CurrencyRateField = null;
		this.LotNumber.FieldName = "LotNumber";
		this.LotNumber.Name = "LotNumber";
		this.LotNumber.PrimaryKey = false;
		this.LotNumber.UnitField = null;
		this.LotNumber.Visible = true;
		this.LotNumber.VisibleIndex = 7;
		this.LotNumber.Width = 98;
		this.Status.Caption = "Status";
		this.Status.ColumnEdit = this.rep_Status;
		this.Status.CurrencyDateField = null;
		this.Status.CurrencyField = null;
		this.Status.CurrencyFixed = null;
		this.Status.CurrencyFixedRate = 0.0;
		this.Status.CurrencyRateField = null;
		this.Status.FieldName = "Status";
		this.Status.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.Status.Name = "Status";
		this.Status.OptionsColumn.ReadOnly = true;
		this.Status.PrimaryKey = false;
		this.Status.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.Status.UnitField = null;
		this.Status.Visible = true;
		this.Status.VisibleIndex = 8;
		this.Status.Width = 65;
		this.rep_Status.AutoHeight = false;
		this.rep_Status.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo, "", -1, false, true, false, imageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance6, appearanceHovered6, appearancePressed6, appearanceDisabled6, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_Status.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.rep_Status.DisplayMember = "Name";
		this.rep_Status.Name = "rep_Status";
		this.rep_Status.NullText = "";
		this.rep_Status.ShowFooter = false;
		this.rep_Status.ShowHeader = false;
		this.rep_Status.ShowLines = false;
		this.rep_Status.ValueMember = "Code";
		this.ExpDate.Caption = "Expiration Date";
		this.ExpDate.ColumnEdit = this.rep_DatePicker;
		this.ExpDate.CurrencyDateField = null;
		this.ExpDate.CurrencyField = null;
		this.ExpDate.CurrencyFixed = null;
		this.ExpDate.CurrencyFixedRate = 0.0;
		this.ExpDate.CurrencyRateField = null;
		this.ExpDate.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.ExpDate.FieldName = "ExpDate";
		this.ExpDate.Name = "ExpDate";
		this.ExpDate.PrimaryKey = false;
		this.ExpDate.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.ExpDate.UnitField = null;
		this.ExpDate.Visible = true;
		this.ExpDate.VisibleIndex = 9;
		this.ExpDate.Width = 87;
		this.rep_DatePicker.AutoHeight = false;
		this.rep_DatePicker.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_DatePicker.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_DatePicker.DisplayFormat.FormatString = "";
		this.rep_DatePicker.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.rep_DatePicker.EditFormat.FormatString = "";
		this.rep_DatePicker.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.rep_DatePicker.Name = "rep_DatePicker";
		this.MnfDate.Caption = "Manufacturring Date";
		this.MnfDate.ColumnEdit = this.rep_DatePicker;
		this.MnfDate.CurrencyDateField = null;
		this.MnfDate.CurrencyField = null;
		this.MnfDate.CurrencyFixed = null;
		this.MnfDate.CurrencyFixedRate = 0.0;
		this.MnfDate.CurrencyRateField = null;
		this.MnfDate.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.MnfDate.FieldName = "MnfDate";
		this.MnfDate.Name = "MnfDate";
		this.MnfDate.PrimaryKey = false;
		this.MnfDate.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.MnfDate.UnitField = null;
		this.MnfDate.Visible = true;
		this.MnfDate.VisibleIndex = 10;
		this.MnfDate.Width = 111;
		this.InDate.Caption = "Admission Date";
		this.InDate.ColumnEdit = this.rep_DatePicker;
		this.InDate.CurrencyDateField = null;
		this.InDate.CurrencyField = null;
		this.InDate.CurrencyFixed = null;
		this.InDate.CurrencyFixedRate = 0.0;
		this.InDate.CurrencyRateField = null;
		this.InDate.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.InDate.FieldName = "InDate";
		this.InDate.Name = "InDate";
		this.InDate.PrimaryKey = false;
		this.InDate.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.InDate.UnitField = null;
		this.InDate.Visible = true;
		this.InDate.VisibleIndex = 11;
		this.InDate.Width = 87;
		this.gridColumn1.Caption = "Location";
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "Location";
		this.gridColumn1.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.PrimaryKey = false;
		this.gridColumn1.UnitField = null;
		this.gridColumn1.Visible = true;
		this.gridColumn1.VisibleIndex = 12;
		this.gridColumn1.Width = 61;
		this.SysNumber.Caption = "System number";
		this.SysNumber.CurrencyDateField = null;
		this.SysNumber.CurrencyField = null;
		this.SysNumber.CurrencyFixed = null;
		this.SysNumber.CurrencyFixedRate = 0.0;
		this.SysNumber.CurrencyRateField = null;
		this.SysNumber.FieldName = "SysNumber";
		this.SysNumber.Name = "SysNumber";
		this.SysNumber.OptionsColumn.ReadOnly = true;
		this.SysNumber.PrimaryKey = false;
		this.SysNumber.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.SysNumber.UnitField = null;
		this.SysNumber.Visible = true;
		this.SysNumber.VisibleIndex = 13;
		this.SysNumber.Width = 88;
		this.Notes.Caption = "Details";
		this.Notes.CurrencyDateField = null;
		this.Notes.CurrencyField = null;
		this.Notes.CurrencyFixed = null;
		this.Notes.CurrencyFixedRate = 0.0;
		this.Notes.CurrencyRateField = null;
		this.Notes.FieldName = "Notes";
		this.Notes.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.Notes.Name = "Notes";
		this.Notes.PrimaryKey = false;
		this.Notes.UnitField = null;
		this.Notes.Visible = true;
		this.Notes.VisibleIndex = 14;
		this.Notes.Width = 63;
		this.AbsEntry.Caption = "AbsEntry";
		this.AbsEntry.CurrencyDateField = null;
		this.AbsEntry.CurrencyField = null;
		this.AbsEntry.CurrencyFixed = null;
		this.AbsEntry.CurrencyFixedRate = 0.0;
		this.AbsEntry.CurrencyRateField = null;
		this.AbsEntry.FieldName = "AbsEntry";
		this.AbsEntry.Name = "AbsEntry";
		this.AbsEntry.PrimaryKey = false;
		this.AbsEntry.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.AbsEntry.UnitField = null;
		this.AbsEntry.Width = 100;
		this.LogEntry.Caption = "LogEntry";
		this.LogEntry.CurrencyDateField = null;
		this.LogEntry.CurrencyField = null;
		this.LogEntry.CurrencyFixed = null;
		this.LogEntry.CurrencyFixedRate = 0.0;
		this.LogEntry.CurrencyRateField = null;
		this.LogEntry.FieldName = "LogEntry";
		this.LogEntry.Name = "LogEntry";
		this.LogEntry.PrimaryKey = false;
		this.LogEntry.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.LogEntry.UnitField = null;
		this.groupControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.groupControl1.AppearanceCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.groupControl1.AppearanceCaption.Options.UseFont = true;
		this.groupControl1.Controls.Add(this.grd_BatchTransaction);
		this.groupControl1.Location = new System.Drawing.Point(0, 0);
		this.groupControl1.Name = "groupControl1";
		this.groupControl1.Size = new System.Drawing.Size(985, 359);
		this.groupControl1.TabIndex = 0;
		this.groupControl1.Text = "Batch Transaction";
		this.grd_BatchTransaction.AllowPasteExcel = true;
		this.grd_BatchTransaction.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_BatchTransaction.ClipboardData = null;
		this.grd_BatchTransaction.Location = new System.Drawing.Point(2, 21);
		this.grd_BatchTransaction.MainView = this.gv_BatchTransaction;
		this.grd_BatchTransaction.Name = "grd_BatchTransaction";
		this.grd_BatchTransaction.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[5] { this.rep_Document, this.rep_Date, this.rep_Warehorse, this.rep_Type, this.rep_Qty });
		this.grd_BatchTransaction.Size = new System.Drawing.Size(981, 336);
		this.grd_BatchTransaction.TabIndex = 0;
		this.grd_BatchTransaction.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_BatchTransaction });
		this.gv_BatchTransaction.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_BatchTransaction.ColumnKey = null;
		this.gv_BatchTransaction.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[9] { this.gridColumn2, this.Document, this.DocLine, this.DocDate, this.LocCode, this.CardName, this.Qty, this.AllocQty, this.Direction });
		this.gv_BatchTransaction.DisableFormSetting = false;
		this.gv_BatchTransaction.DisplayException = false;
		this.gv_BatchTransaction.EditColumn = null;
		this.gv_BatchTransaction.FieldCurrency = null;
		this.gv_BatchTransaction.FieldRate = null;
		this.gv_BatchTransaction.FormId = "oBatchTransaction";
		this.gv_BatchTransaction.GridControl = this.grd_BatchTransaction;
		this.gv_BatchTransaction.IgnoreFilter = false;
		this.gv_BatchTransaction.IndexEditRow = -1;
		this.gv_BatchTransaction.IndicatorWidth = 35;
		this.gv_BatchTransaction.IsApzTable = false;
		this.gv_BatchTransaction.IsInsertNewRow = false;
		this.gv_BatchTransaction.IsLoadingData = false;
		this.gv_BatchTransaction.IsMappingContextAction = false;
		this.gv_BatchTransaction.ItemId = "OBTT";
		this.gv_BatchTransaction.LstColumnAllowBatch = null;
		this.gv_BatchTransaction.LstColumnNotAllowBatch = null;
		this.gv_BatchTransaction.LstColumnNotEditable = null;
		this.gv_BatchTransaction.LstColumNotSetting = null;
		this.gv_BatchTransaction.Name = "gv_BatchTransaction";
		this.gv_BatchTransaction.OptionsBehavior.ReadOnly = true;
		this.gv_BatchTransaction.OptionsView.ColumnAutoWidth = false;
		this.gv_BatchTransaction.OptionsView.ShowFooter = true;
		this.gv_BatchTransaction.OptionsView.ShowGroupPanel = false;
		this.gv_BatchTransaction.UseBatchMod = false;
		this.gv_BatchTransaction.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_BatchTransaction_RowCellStyle);
		this.gv_BatchTransaction.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_BatchTransaction_ShowingEditor);
		this.gridColumn2.Caption = "Doc Type";
		this.gridColumn2.ColumnEdit = this.rep_Type;
		this.gridColumn2.CurrencyDateField = null;
		this.gridColumn2.CurrencyField = null;
		this.gridColumn2.CurrencyFixed = null;
		this.gridColumn2.CurrencyFixedRate = 0.0;
		this.gridColumn2.CurrencyRateField = null;
		this.gridColumn2.FieldName = "DocType";
		this.gridColumn2.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.PrimaryKey = true;
		this.gridColumn2.UnitField = null;
		this.gridColumn2.Visible = true;
		this.gridColumn2.VisibleIndex = 0;
		this.gridColumn2.Width = 129;
		this.rep_Type.AutoHeight = false;
		this.rep_Type.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo, "", -1, false, true, false, imageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance7, appearanceHovered7, appearancePressed7, appearanceDisabled7, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_Type.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rep_Type.DisplayMember = "Name";
		this.rep_Type.Name = "rep_Type";
		this.rep_Type.NullText = "";
		this.rep_Type.ShowFooter = false;
		this.rep_Type.ShowHeader = false;
		this.rep_Type.ShowLines = false;
		this.rep_Type.ValueMember = "Code";
		this.Document.Caption = "Doc No.";
		this.Document.ColumnEdit = this.rep_Document;
		this.Document.CurrencyDateField = null;
		this.Document.CurrencyField = null;
		this.Document.CurrencyFixed = null;
		this.Document.CurrencyFixedRate = 0.0;
		this.Document.CurrencyRateField = null;
		this.Document.FieldName = "Document";
		this.Document.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.Document.Name = "Document";
		this.Document.OptionsColumn.ReadOnly = true;
		this.Document.PrimaryKey = false;
		this.Document.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.Document.UnitField = null;
		this.Document.Visible = true;
		this.Document.VisibleIndex = 1;
		this.Document.Width = 98;
		this.rep_Document.AutoHeight = false;
		editorButtonImageOptions4.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_Document.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance8, appearanceHovered8, appearancePressed8, appearanceDisabled8, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_Document.Name = "rep_Document";
		this.rep_Document.ReadOnly = true;
		this.rep_Document.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_Document_ButtonClick);
		this.DocLine.Caption = "Doc.Row";
		this.DocLine.CurrencyDateField = null;
		this.DocLine.CurrencyField = null;
		this.DocLine.CurrencyFixed = null;
		this.DocLine.CurrencyFixedRate = 0.0;
		this.DocLine.CurrencyRateField = null;
		this.DocLine.FieldName = "DocLine";
		this.DocLine.Name = "DocLine";
		this.DocLine.OptionsColumn.ReadOnly = true;
		this.DocLine.PrimaryKey = false;
		this.DocLine.UnitField = null;
		this.DocLine.Visible = true;
		this.DocLine.VisibleIndex = 2;
		this.DocLine.Width = 79;
		this.DocDate.Caption = "Date";
		this.DocDate.CurrencyDateField = null;
		this.DocDate.CurrencyField = null;
		this.DocDate.CurrencyFixed = null;
		this.DocDate.CurrencyFixedRate = 0.0;
		this.DocDate.CurrencyRateField = null;
		this.DocDate.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.DocDate.FieldName = "DocDate";
		this.DocDate.Name = "DocDate";
		this.DocDate.OptionsColumn.ReadOnly = true;
		this.DocDate.PrimaryKey = false;
		this.DocDate.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.DocDate.UnitField = null;
		this.DocDate.Visible = true;
		this.DocDate.VisibleIndex = 3;
		this.DocDate.Width = 92;
		this.LocCode.Caption = "Whse";
		this.LocCode.ColumnEdit = this.rep_Warehorse;
		this.LocCode.CurrencyDateField = null;
		this.LocCode.CurrencyField = null;
		this.LocCode.CurrencyFixed = null;
		this.LocCode.CurrencyFixedRate = 0.0;
		this.LocCode.CurrencyRateField = null;
		this.LocCode.FieldName = "LocCode";
		this.LocCode.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.LocCode.Name = "LocCode";
		this.LocCode.OptionsColumn.ReadOnly = true;
		this.LocCode.PrimaryKey = false;
		this.LocCode.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.LocCode.UnitField = null;
		this.LocCode.Visible = true;
		this.LocCode.VisibleIndex = 4;
		this.LocCode.Width = 126;
		this.rep_Warehorse.AutoHeight = false;
		editorButtonImageOptions5.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_Warehorse.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions5, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance9, appearanceHovered9, appearancePressed9, appearanceDisabled9, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_Warehorse.Name = "rep_Warehorse";
		this.rep_Warehorse.ReadOnly = true;
		this.rep_Warehorse.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_Warehorse_ButtonClick);
		this.CardName.Caption = "G/L Acct/BP Name";
		this.CardName.CurrencyDateField = null;
		this.CardName.CurrencyField = null;
		this.CardName.CurrencyFixed = null;
		this.CardName.CurrencyFixedRate = 0.0;
		this.CardName.CurrencyRateField = null;
		this.CardName.FieldName = "CardName";
		this.CardName.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.CardName.Name = "CardName";
		this.CardName.OptionsColumn.ReadOnly = true;
		this.CardName.PrimaryKey = false;
		this.CardName.UnitField = null;
		this.CardName.Visible = true;
		this.CardName.VisibleIndex = 5;
		this.CardName.Width = 301;
		this.Qty.Caption = "Qty";
		this.Qty.ColumnEdit = this.rep_Qty;
		this.Qty.CurrencyDateField = null;
		this.Qty.CurrencyField = null;
		this.Qty.CurrencyFixed = null;
		this.Qty.CurrencyFixedRate = 0.0;
		this.Qty.CurrencyRateField = null;
		this.Qty.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.Qty.FieldName = "Qty";
		this.Qty.Name = "Qty";
		this.Qty.OptionsColumn.ReadOnly = true;
		this.Qty.PrimaryKey = false;
		this.Qty.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[1]
		{
			new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum)
		});
		this.Qty.UnitField = null;
		this.Qty.Visible = true;
		this.Qty.VisibleIndex = 6;
		this.Qty.Width = 107;
		this.rep_Qty.AutoHeight = false;
		this.rep_Qty.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo, "", -1, false, true, false, imageOptions5, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance10, appearanceHovered10, appearancePressed10, appearanceDisabled10, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_Qty.Name = "rep_Qty";
		this.AllocQty.Caption = "Allocated";
		this.AllocQty.ColumnEdit = this.rep_Qty;
		this.AllocQty.CurrencyDateField = null;
		this.AllocQty.CurrencyField = null;
		this.AllocQty.CurrencyFixed = null;
		this.AllocQty.CurrencyFixedRate = 0.0;
		this.AllocQty.CurrencyRateField = null;
		this.AllocQty.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.AllocQty.FieldName = "AllocQty";
		this.AllocQty.Name = "AllocQty";
		this.AllocQty.OptionsColumn.ReadOnly = true;
		this.AllocQty.PrimaryKey = false;
		this.AllocQty.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[1]
		{
			new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum)
		});
		this.AllocQty.UnitField = null;
		this.AllocQty.Visible = true;
		this.AllocQty.VisibleIndex = 7;
		this.AllocQty.Width = 101;
		this.Direction.Caption = "Direction";
		this.Direction.CurrencyDateField = null;
		this.Direction.CurrencyField = null;
		this.Direction.CurrencyFixed = null;
		this.Direction.CurrencyFixedRate = 0.0;
		this.Direction.CurrencyRateField = null;
		this.Direction.FieldName = "Direction";
		this.Direction.Name = "Direction";
		this.Direction.OptionsColumn.ReadOnly = true;
		this.Direction.PrimaryKey = false;
		this.Direction.UnitField = null;
		this.Direction.Visible = true;
		this.Direction.VisibleIndex = 8;
		this.Direction.Width = 100;
		this.rep_Date.AutoHeight = false;
		this.rep_Date.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Date.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Date.DisplayFormat.FormatString = "";
		this.rep_Date.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.rep_Date.EditFormat.FormatString = "";
		this.rep_Date.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.rep_Date.Name = "rep_Date";
		this.groupControl2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.groupControl2.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.groupControl2.Appearance.Options.UseFont = true;
		this.groupControl2.AppearanceCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.groupControl2.AppearanceCaption.Options.UseFont = true;
		this.groupControl2.Controls.Add(this.grd_Batch);
		this.groupControl2.Location = new System.Drawing.Point(0, 0);
		this.groupControl2.Name = "groupControl2";
		this.groupControl2.Size = new System.Drawing.Size(985, 188);
		this.groupControl2.TabIndex = 0;
		this.groupControl2.Text = "Batches";
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(2, 63);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.groupControl2);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.groupControl1);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(985, 552);
		this.splitContainerControl1.SplitterPosition = 188;
		this.splitContainerControl1.TabIndex = 20;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.btn_Search.EditControl = null;
		this.btn_Search.EditValue = "Search";
		this.btn_Search.FieldName = "btnSearch";
		this.btn_Search.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Search.ImageOptions.Image");
		this.btn_Search.IsSearchControl = true;
		this.btn_Search.Location = new System.Drawing.Point(456, 37);
		this.btn_Search.Name = "btn_Search";
		this.btn_Search.ReadOnly = false;
		this.btn_Search.SettingInVisible = false;
		this.btn_Search.SettingReadonly = false;
		this.btn_Search.Size = new System.Drawing.Size(81, 20);
		this.btn_Search.TabIndex = 23;
		this.btn_Search.Text = "Search";
		this.btn_Search.OnClickAsync += new Apzon.Controls.Controls.OnClickAsync(this.btn_Search_Click);
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "Item Code To";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(4, 40);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(90, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 18;
		this.labelLine3.Utext = "Item Code To";
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Item Code From";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(4, 14);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(90, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 16;
		this.labelLine1.Utext = "Item Code From";
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "Status";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(227, 40);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(76, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 24;
		this.labelLine2.Utext = "Status";
		this.look_Status.EditControl = null;
		this.look_Status.FieldName = "Status";
		this.look_Status.IsSearchControl = false;
		this.look_Status.Location = new System.Drawing.Point(303, 37);
		this.look_Status.Name = "look_Status";
		this.look_Status.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.look_Status.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.look_Status.Properties.DisplayMember = "Name";
		this.look_Status.Properties.NullText = "";
		this.look_Status.Properties.ShowFooter = false;
		this.look_Status.Properties.ShowHeader = false;
		this.look_Status.Properties.ShowLines = false;
		this.look_Status.Properties.ValueMember = "Code";
		this.look_Status.SettingInVisible = false;
		this.look_Status.SettingReadonly = false;
		this.look_Status.Size = new System.Drawing.Size(147, 20);
		this.look_Status.TabIndex = 25;
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "DistNumber";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(227, 14);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(76, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 26;
		this.labelLine4.Utext = "DistNumber";
		this.txt_DistNumber.EditControl = null;
		this.txt_DistNumber.FieldName = "DistNumber";
		this.txt_DistNumber.IsSearchControl = true;
		this.txt_DistNumber.Location = new System.Drawing.Point(303, 11);
		this.txt_DistNumber.Name = "txt_DistNumber";
		this.txt_DistNumber.SettingInVisible = false;
		this.txt_DistNumber.SettingReadonly = false;
		this.txt_DistNumber.Size = new System.Drawing.Size(147, 20);
		this.txt_DistNumber.TabIndex = 27;
		this.btn_ItemCodeFrom.EditControl = null;
		this.btn_ItemCodeFrom.FieldName = "FromItem";
		this.btn_ItemCodeFrom.IsSearchControl = true;
		this.btn_ItemCodeFrom.Location = new System.Drawing.Point(93, 12);
		this.btn_ItemCodeFrom.Name = "btn_ItemCodeFrom";
		this.btn_ItemCodeFrom.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions6.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions7.Image");
		this.btn_ItemCodeFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions6, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance11, appearanceHovered11, appearancePressed11, appearanceDisabled11, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions6, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance12, appearanceHovered12, appearancePressed12, appearanceDisabled12, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_ItemCodeFrom.Properties.ContainerControl = null;
		this.btn_ItemCodeFrom.Properties.DisplayMember = "ItemCode";
		this.btn_ItemCodeFrom.Properties.DisplayMember2 = "";
		this.btn_ItemCodeFrom.Properties.LoadDataSourceWhenEnter = false;
		this.btn_ItemCodeFrom.Properties.SeparateCharacterForMulti = ';';
		this.btn_ItemCodeFrom.Properties.SeparateString = ", ";
		this.btn_ItemCodeFrom.Properties.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.ListItems;
		this.btn_ItemCodeFrom.Properties.TypeDrillDown = Apzon.Commons.ApzObjectTypes.oItems;
		this.btn_ItemCodeFrom.Properties.UsingAutoCompleteBox = false;
		this.btn_ItemCodeFrom.Properties.UsingDataSource = true;
		this.btn_ItemCodeFrom.Properties.ValidateWhenNoDataSource = false;
		this.btn_ItemCodeFrom.Properties.ValueMember = "ItemCode";
		this.btn_ItemCodeFrom.SettingInVisible = false;
		this.btn_ItemCodeFrom.SettingReadonly = false;
		this.btn_ItemCodeFrom.Size = new System.Drawing.Size(128, 20);
		this.btn_ItemCodeFrom.TabIndex = 28;
		this.btn_ItemCodeTo.EditControl = null;
		this.btn_ItemCodeTo.FieldName = "ToItem";
		this.btn_ItemCodeTo.IsSearchControl = true;
		this.btn_ItemCodeTo.Location = new System.Drawing.Point(93, 37);
		this.btn_ItemCodeTo.Name = "btn_ItemCodeTo";
		this.btn_ItemCodeTo.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions7.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions5.Image");
		this.btn_ItemCodeTo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions7, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance13, appearanceHovered13, appearancePressed13, appearanceDisabled13, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions7, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance14, appearanceHovered14, appearancePressed14, appearanceDisabled14, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_ItemCodeTo.Properties.ContainerControl = null;
		this.btn_ItemCodeTo.Properties.DisplayMember = "ItemCode";
		this.btn_ItemCodeTo.Properties.DisplayMember2 = "";
		this.btn_ItemCodeTo.Properties.LoadDataSourceWhenEnter = false;
		this.btn_ItemCodeTo.Properties.SeparateCharacterForMulti = ';';
		this.btn_ItemCodeTo.Properties.SeparateString = ", ";
		this.btn_ItemCodeTo.Properties.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.ListItems;
		this.btn_ItemCodeTo.Properties.TypeDrillDown = Apzon.Commons.ApzObjectTypes.oItems;
		this.btn_ItemCodeTo.Properties.UsingAutoCompleteBox = false;
		this.btn_ItemCodeTo.Properties.UsingDataSource = true;
		this.btn_ItemCodeTo.Properties.ValidateWhenNoDataSource = false;
		this.btn_ItemCodeTo.Properties.ValueMember = "ItemCode";
		this.btn_ItemCodeTo.SettingInVisible = false;
		this.btn_ItemCodeTo.SettingReadonly = false;
		this.btn_ItemCodeTo.Size = new System.Drawing.Size(128, 20);
		this.btn_ItemCodeTo.TabIndex = 29;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(989, 616);
		base.Controls.Add(this.btn_ItemCodeTo);
		base.Controls.Add(this.btn_ItemCodeFrom);
		base.Controls.Add(this.txt_DistNumber);
		base.Controls.Add(this.labelLine4);
		base.Controls.Add(this.look_Status);
		base.Controls.Add(this.labelLine2);
		base.Controls.Add(this.splitContainerControl1);
		base.Controls.Add(this.btn_Search);
		base.Controls.Add(this.labelLine3);
		base.Controls.Add(this.labelLine1);
		base.FormId = "oBatchTransaction";
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_BatchTransaction.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_BatchTransaction";
		this.Text = "Batch Transaction";
		base.Load += new System.EventHandler(this.frm_BatchTransaction_Load);
		((System.ComponentModel.ISupportInitialize)this.grd_Batch).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Batch).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ItemNo).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rDisNumber).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Whse).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Quantity).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Status).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_DatePicker.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_DatePicker).EndInit();
		((System.ComponentModel.ISupportInitialize)this.groupControl1).EndInit();
		this.groupControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_BatchTransaction).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_BatchTransaction).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Type).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Document).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Warehorse).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Qty).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Date.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Date).EndInit();
		((System.ComponentModel.ISupportInitialize)this.groupControl2).EndInit();
		this.groupControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.look_Status.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_DistNumber.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_ItemCodeFrom.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_ItemCodeTo.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
