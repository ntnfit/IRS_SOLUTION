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
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
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
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Inventory.Item_Management.Item_Serial_Numbers;

public class frm_SerialNumberTransaction : XtraFormEx
{
	public string _itemCode = string.Empty;

	public int _sysNumber;

	private Dictionary<string, string> lstStatus = new Dictionary<string, string>
	{
		{ "0", "Available" },
		{ "1", "Unavailable" },
		{ "2", "Allocated" }
	};

	private DataTable dtDoc = new DataTable();

	private DataTable dtDocs = new DataTable();

	private IContainer components;

	private SplitContainerControl splitContainerControl1;

	private GroupControl groupControl2;

	private GridControlEx grd_SerialNumber;

	private GridViewEx gv_SerialNumber;

	private GridColumnEx gridColumn9;

	private GridColumnEx gridColumn8;

	private GridColumnEx gridColumn11;

	private GridColumnEx gridColumn12;

	private GridColumnEx gridColumn10;

	private GridColumnEx gridColumn15;

	private RepositoryItemDateEdit rep_Date;

	private GridColumnEx gridColumn14;

	private RepositoryItemDateEdit rep_MDate;

	private GridColumnEx gridColumn13;

	private RepositoryItemDateEdit rep_EDate;

	private GridColumnEx gridColumn16;

	private RepositoryItemDateEdit rep_Start;

	private GridColumnEx gridColumn17;

	private RepositoryItemDateEdit rep_End;

	private GridColumnEx gridColumn18;

	private GridColumnEx gridColumn19;

	private GridColumnEx gridColumn40;

	private RepositoryItemLookUpEdit rep_Status;

	private GridColumnEx gridColumn21;

	private GridColumnEx gridColumn28;

	private RepositoryItemButtonEdit rep_ItemCode;

	private RepositoryItemButtonEdit rep_WhsCode;

	private GroupControl groupControl1;

	private GridControlEx grd_BatchTransaction;

	private GridViewEx gv_BatchTransaction;

	private GridColumnEx gridColumn2;

	private RepositoryItemLookUpEdit rep_Type;

	private GridColumnEx Document;

	private RepositoryItemButtonEdit rep_Document;

	private GridColumnEx DocLine;

	private GridColumnEx DocDate;

	private RepositoryItemDateEdit rep_DocDate;

	private GridColumnEx LocCode;

	private RepositoryItemButtonEdit rep_Warehorse;

	private GridColumnEx BinLocation;

	private GridColumnEx gridColumn1;

	private GridColumnEx CardName;

	private GridColumnEx AllocQty;

	private RepositoryItemCalcEdit rep_Qty;

	private GridColumnEx Direction;

	private ButtonEditCFL btn_ItemCodeTo;

	private ButtonEditCFL btn_ItemCodeFrom;

	private TextEditEx txt_DistNumber;

	private LabelLine labelLine4;

	private LookUpEditEx look_Status;

	private LabelLine labelLine2;

	private SimpleButtonEx btn_Search;

	private LabelLine labelLine3;

	private LabelLine labelLine1;

	public frm_SerialNumberTransaction()
	{
		InitializeComponent();
	}

	private async void frm_SerialNumberTransaction_Load(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			await WinFuntions.LoadUdfColumns(gv_SerialNumber, "OSRN");
			if (!string.IsNullOrEmpty(_itemCode) || _sysNumber > 0)
			{
				DataTable dataSource = await GetSerialNumbers(_itemCode, _sysNumber);
				grd_SerialNumber.DataSource = dataSource;
				grd_SerialNumber.RefreshDataSource();
			}
			List<Common.Table_Code> dataSource2 = CommonList.oDocumentNumberingCode.Select((KeyValuePair<string, string> item) => new Common.Table_Code
			{
				Code = item.Key,
				Name = item.Value
			}).ToList();
			rep_Type.DataSource = dataSource2;
			List<Common.Table_Code> dataSource3 = lstStatus.Select((KeyValuePair<string, string> item) => new Common.Table_Code
			{
				Code = item.Key,
				Name = item.Value
			}).ToList();
			rep_Status.DataSource = dataSource3;
			rep_Date.Mask.MaskType = MaskType.DateTime;
			rep_Date.Mask.EditMask = GlobalData.DateTimeFormat;
			rep_Date.DisplayFormat.FormatType = FormatType.DateTime;
			rep_Date.DisplayFormat.FormatString = GlobalData.DateTimeFormat;
			rep_Date.EditFormat.FormatType = FormatType.DateTime;
			rep_Date.EditFormat.FormatString = GlobalData.DateTimeFormat;
			rep_DocDate.Mask.MaskType = MaskType.DateTime;
			rep_DocDate.Mask.EditMask = GlobalData.DateTimeFormat;
			rep_DocDate.DisplayFormat.FormatType = FormatType.DateTime;
			rep_DocDate.DisplayFormat.FormatString = GlobalData.DateTimeFormat;
			rep_DocDate.EditFormat.FormatType = FormatType.DateTime;
			rep_DocDate.EditFormat.FormatString = GlobalData.DateTimeFormat;
			rep_EDate.Mask.MaskType = MaskType.DateTime;
			rep_EDate.Mask.EditMask = GlobalData.DateTimeFormat;
			rep_EDate.DisplayFormat.FormatType = FormatType.DateTime;
			rep_EDate.DisplayFormat.FormatString = GlobalData.DateTimeFormat;
			rep_EDate.EditFormat.FormatType = FormatType.DateTime;
			rep_EDate.EditFormat.FormatString = GlobalData.DateTimeFormat;
			rep_MDate.Mask.MaskType = MaskType.DateTime;
			rep_MDate.Mask.EditMask = GlobalData.DateTimeFormat;
			rep_MDate.DisplayFormat.FormatType = FormatType.DateTime;
			rep_MDate.DisplayFormat.FormatString = GlobalData.DateTimeFormat;
			rep_MDate.EditFormat.FormatType = FormatType.DateTime;
			rep_MDate.EditFormat.FormatString = GlobalData.DateTimeFormat;
			rep_Start.Mask.MaskType = MaskType.DateTime;
			rep_Start.Mask.EditMask = GlobalData.DateTimeFormat;
			rep_Start.DisplayFormat.FormatType = FormatType.DateTime;
			rep_Start.DisplayFormat.FormatString = GlobalData.DateTimeFormat;
			rep_Start.EditFormat.FormatType = FormatType.DateTime;
			rep_Start.EditFormat.FormatString = GlobalData.DateTimeFormat;
			rep_End.Mask.MaskType = MaskType.DateTime;
			rep_End.Mask.EditMask = GlobalData.DateTimeFormat;
			rep_End.DisplayFormat.FormatType = FormatType.DateTime;
			rep_End.DisplayFormat.FormatString = GlobalData.DateTimeFormat;
			rep_End.EditFormat.FormatType = FormatType.DateTime;
			rep_End.EditFormat.FormatString = GlobalData.DateTimeFormat;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<DataTable> GetSerialNumbers(string itemCode, int sysNumber)
	{
		try
		{
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "SerialNumberTransaction");
			return await _helper.GetDataTableAsync("GetSerialNumbers", new { itemCode, sysNumber });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	private void rep_ItemCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			string text = Function.ToString(gv_SerialNumber.GetFocusedRowCellValue("ItemCode"));
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

	private void rep_WhsCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			string whsSearchCode = Function.ToString(gv_SerialNumber.GetFocusedRowCellValue("WhsCode"));
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

	private async void gv_SerialNumber_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			string whsCode = Function.ToString(gv_SerialNumber.GetFocusedRowCellValue("WhsCode"));
			_itemCode = Function.ToString(gv_SerialNumber.GetFocusedRowCellValue("ItemCode"));
			_sysNumber = Function.ParseInt(gv_SerialNumber.GetFocusedRowCellValue("SysNumber"));
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "SerialNumberTransaction");
			DataTable dataSource = await _helper.GetDataTableAsync("GetTransactions", new
			{
				itemCode = _itemCode,
				sysNumber = _sysNumber,
				whsCode = whsCode
			});
			grd_BatchTransaction.DataSource = dataSource;
			grd_BatchTransaction.RefreshDataSource();
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
			ApzObjectTypes apzObjectTypes = (ApzObjectTypes)Function.ParseInt(gv_BatchTransaction.GetFocusedRowCellValue("DocType"));
			string id = Function.ToString(gv_BatchTransaction.GetFocusedRowCellValue("DocEntry"));
			this.DrillDown(apzObjectTypes, apzObjectTypes, id);
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

	private void gv_BatchTransaction_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void gv_SerialNumber_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		try
		{
			if (e.Info.IsRowIndicator)
			{
				e.Info.DisplayText = (e.RowHandle + 1).ToString();
			}
		}
		catch (Exception)
		{
		}
	}

	private void gv_BatchTransaction_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (gv_BatchTransaction.FocusedColumn.FieldName == "DocType")
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

	private async void btn_Search_Click(object sender, EventArgs e)
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

	private async Task LoadData()
	{
		gv_SerialNumber.ShowLoadingPanel();
		try
		{
			dtDocs = await GetDocs();
			grd_SerialNumber.DataSource = dtDocs;
			grd_SerialNumber.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		gv_SerialNumber.HideLoadingPanel();
	}

	private async Task<DataTable> GetDocs()
	{
		try
		{
			string distNumber = Function.ToString(txt_DistNumber.EditValue);
			string itemfrom = Function.ToString(btn_ItemCodeFrom.EditValue);
			string itemto = Function.ToString(btn_ItemCodeTo.EditValue);
			string status = Function.ToString(look_Status.EditValue);
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "SerialNumberTransaction");
			return await _helper.GetDataTableAsync("GetDocs", new { distNumber, itemfrom, itemto, status });
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
		DevExpress.Utils.SerializableAppearanceObject appearance4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions4 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions5 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Inventory.Item_Management.Item_Serial_Numbers.frm_SerialNumberTransaction));
		DevExpress.Utils.SerializableAppearanceObject appearance8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions4 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions6 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled10 = new DevExpress.Utils.SerializableAppearanceObject();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.groupControl2 = new DevExpress.XtraEditors.GroupControl();
		this.grd_SerialNumber = new Apzon.Controls.Grid.GridControlEx();
		this.gv_SerialNumber = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn9 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_ItemCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gridColumn28 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn8 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn11 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn12 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn10 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn13 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_EDate = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.gridColumn14 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_MDate = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.gridColumn15 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Date = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.gridColumn16 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Start = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.gridColumn17 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_End = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.gridColumn18 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_WhsCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gridColumn19 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn40 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Status = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumn21 = new Apzon.Controls.Grid.GridColumnEx();
		this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
		this.grd_BatchTransaction = new Apzon.Controls.Grid.GridControlEx();
		this.gv_BatchTransaction = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn2 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Type = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.Document = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Document = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.DocLine = new Apzon.Controls.Grid.GridColumnEx();
		this.DocDate = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_DocDate = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.LocCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Warehorse = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.BinLocation = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.CardName = new Apzon.Controls.Grid.GridColumnEx();
		this.AllocQty = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Qty = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.Direction = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_ItemCodeTo = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.btn_ItemCodeFrom = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.txt_DistNumber = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.look_Status = new Apzon.Controls.Editor.LookUpEditEx();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.btn_Search = new Apzon.Controls.Controls.SimpleButtonEx();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.groupControl2).BeginInit();
		this.groupControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_SerialNumber).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_SerialNumber).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ItemCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_EDate).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_EDate.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_MDate).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_MDate.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Date).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Date.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Start).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Start.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_End).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_End.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_WhsCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Status).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.groupControl1).BeginInit();
		this.groupControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_BatchTransaction).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_BatchTransaction).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Type).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Document).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_DocDate).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_DocDate.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Warehorse).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Qty).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_ItemCodeTo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_ItemCodeFrom.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_DistNumber.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.look_Status.Properties).BeginInit();
		base.SuspendLayout();
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(0, 64);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.groupControl2);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.groupControl1);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(1054, 579);
		this.splitContainerControl1.SplitterPosition = 164;
		this.splitContainerControl1.TabIndex = 0;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.groupControl2.AppearanceCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.groupControl2.AppearanceCaption.Options.UseFont = true;
		this.groupControl2.Controls.Add(this.grd_SerialNumber);
		this.groupControl2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.groupControl2.Location = new System.Drawing.Point(0, 0);
		this.groupControl2.Name = "groupControl2";
		this.groupControl2.Size = new System.Drawing.Size(1054, 164);
		this.groupControl2.TabIndex = 3;
		this.groupControl2.Text = "Serial Numbers";
		this.grd_SerialNumber.AllowPasteExcel = true;
		this.grd_SerialNumber.ClipboardData = null;
		this.grd_SerialNumber.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_SerialNumber.Location = new System.Drawing.Point(2, 23);
		this.grd_SerialNumber.MainView = this.gv_SerialNumber;
		this.grd_SerialNumber.Name = "grd_SerialNumber";
		this.grd_SerialNumber.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[8] { this.rep_Status, this.rep_Date, this.rep_MDate, this.rep_EDate, this.rep_Start, this.rep_End, this.rep_ItemCode, this.rep_WhsCode });
		this.grd_SerialNumber.Size = new System.Drawing.Size(1050, 139);
		this.grd_SerialNumber.TabIndex = 1;
		this.grd_SerialNumber.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_SerialNumber });
		this.gv_SerialNumber.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_SerialNumber.ColumnKey = null;
		this.gv_SerialNumber.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[15]
		{
			this.gridColumn9, this.gridColumn28, this.gridColumn8, this.gridColumn11, this.gridColumn12, this.gridColumn10, this.gridColumn13, this.gridColumn14, this.gridColumn15, this.gridColumn16,
			this.gridColumn17, this.gridColumn18, this.gridColumn19, this.gridColumn40, this.gridColumn21
		});
		this.gv_SerialNumber.DisableFormSetting = false;
		this.gv_SerialNumber.DisplayException = false;
		this.gv_SerialNumber.EditColumn = null;
		this.gv_SerialNumber.FieldCurrency = null;
		this.gv_SerialNumber.FieldRate = null;
		this.gv_SerialNumber.FormId = "oSerialNumberTransaction";
		this.gv_SerialNumber.GridControl = this.grd_SerialNumber;
		this.gv_SerialNumber.IgnoreFilter = false;
		this.gv_SerialNumber.IndexEditRow = -1;
		this.gv_SerialNumber.IndicatorWidth = 35;
		this.gv_SerialNumber.IsApzTable = false;
		this.gv_SerialNumber.IsInsertNewRow = false;
		this.gv_SerialNumber.IsLoadingData = false;
		this.gv_SerialNumber.IsMappingContextAction = false;
		this.gv_SerialNumber.ItemId = "OSRN";
		this.gv_SerialNumber.LstColumnAllowBatch = null;
		this.gv_SerialNumber.LstColumnNotAllowBatch = null;
		this.gv_SerialNumber.LstColumnNotEditable = null;
		this.gv_SerialNumber.LstColumNotSetting = null;
		this.gv_SerialNumber.Name = "gv_SerialNumber";
		this.gv_SerialNumber.OptionsBehavior.ReadOnly = true;
		this.gv_SerialNumber.OptionsSelection.CheckBoxSelectorColumnWidth = 35;
		this.gv_SerialNumber.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
		this.gv_SerialNumber.OptionsView.ColumnAutoWidth = false;
		this.gv_SerialNumber.OptionsView.ShowGroupPanel = false;
		this.gv_SerialNumber.UseBatchMod = false;
		this.gv_SerialNumber.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_SerialNumber_CustomDrawRowIndicator);
		this.gv_SerialNumber.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_SerialNumber_FocusedRowObjectChanged);
		this.gridColumn9.Caption = "Item No.";
		this.gridColumn9.ColumnEdit = this.rep_ItemCode;
		this.gridColumn9.CurrencyDateField = null;
		this.gridColumn9.CurrencyField = null;
		this.gridColumn9.CurrencyFixed = null;
		this.gridColumn9.CurrencyFixedRate = 0.0;
		this.gridColumn9.CurrencyRateField = null;
		this.gridColumn9.FieldName = "ItemCode";
		this.gridColumn9.Name = "gridColumn9";
		this.gridColumn9.OptionsColumn.ReadOnly = true;
		this.gridColumn9.PrimaryKey = true;
		this.gridColumn9.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridColumn9.UnitField = null;
		this.gridColumn9.Visible = true;
		this.gridColumn9.VisibleIndex = 0;
		this.gridColumn9.Width = 109;
		this.rep_ItemCode.AutoHeight = false;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_ItemCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_ItemCode.Name = "rep_ItemCode";
		this.rep_ItemCode.ReadOnly = true;
		this.rep_ItemCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_ItemCode_ButtonClick);
		this.gridColumn28.Caption = "Item Description";
		this.gridColumn28.CurrencyDateField = null;
		this.gridColumn28.CurrencyField = null;
		this.gridColumn28.CurrencyFixed = null;
		this.gridColumn28.CurrencyFixedRate = 0.0;
		this.gridColumn28.CurrencyRateField = null;
		this.gridColumn28.FieldName = "itemName";
		this.gridColumn28.Name = "gridColumn28";
		this.gridColumn28.OptionsColumn.ReadOnly = true;
		this.gridColumn28.PrimaryKey = false;
		this.gridColumn28.UnitField = null;
		this.gridColumn28.Visible = true;
		this.gridColumn28.VisibleIndex = 1;
		this.gridColumn28.Width = 189;
		this.gridColumn8.Caption = "Mfr Serial No.";
		this.gridColumn8.CurrencyDateField = null;
		this.gridColumn8.CurrencyField = null;
		this.gridColumn8.CurrencyFixed = null;
		this.gridColumn8.CurrencyFixedRate = 0.0;
		this.gridColumn8.CurrencyRateField = null;
		this.gridColumn8.FieldName = "MnfSerial";
		this.gridColumn8.Name = "gridColumn8";
		this.gridColumn8.OptionsColumn.ReadOnly = true;
		this.gridColumn8.PrimaryKey = false;
		this.gridColumn8.UnitField = null;
		this.gridColumn8.Visible = true;
		this.gridColumn8.VisibleIndex = 2;
		this.gridColumn8.Width = 116;
		this.gridColumn11.Caption = "Serial Number";
		this.gridColumn11.CurrencyDateField = null;
		this.gridColumn11.CurrencyField = null;
		this.gridColumn11.CurrencyFixed = null;
		this.gridColumn11.CurrencyFixedRate = 0.0;
		this.gridColumn11.CurrencyRateField = null;
		this.gridColumn11.FieldName = "DistNumber";
		this.gridColumn11.Name = "gridColumn11";
		this.gridColumn11.OptionsColumn.ReadOnly = true;
		this.gridColumn11.PrimaryKey = false;
		this.gridColumn11.UnitField = null;
		this.gridColumn11.Visible = true;
		this.gridColumn11.VisibleIndex = 3;
		this.gridColumn11.Width = 118;
		this.gridColumn12.Caption = "Lot Number";
		this.gridColumn12.CurrencyDateField = null;
		this.gridColumn12.CurrencyField = null;
		this.gridColumn12.CurrencyFixed = null;
		this.gridColumn12.CurrencyFixedRate = 0.0;
		this.gridColumn12.CurrencyRateField = null;
		this.gridColumn12.FieldName = "LotNumber";
		this.gridColumn12.Name = "gridColumn12";
		this.gridColumn12.OptionsColumn.ReadOnly = true;
		this.gridColumn12.PrimaryKey = false;
		this.gridColumn12.UnitField = null;
		this.gridColumn12.Visible = true;
		this.gridColumn12.VisibleIndex = 4;
		this.gridColumn12.Width = 111;
		this.gridColumn10.Caption = "System No.";
		this.gridColumn10.CurrencyDateField = null;
		this.gridColumn10.CurrencyField = null;
		this.gridColumn10.CurrencyFixed = null;
		this.gridColumn10.CurrencyFixedRate = 0.0;
		this.gridColumn10.CurrencyRateField = null;
		this.gridColumn10.FieldName = "SysNumber";
		this.gridColumn10.Name = "gridColumn10";
		this.gridColumn10.OptionsColumn.ReadOnly = true;
		this.gridColumn10.PrimaryKey = false;
		this.gridColumn10.UnitField = null;
		this.gridColumn10.Width = 65;
		this.gridColumn13.Caption = "Expiration Date";
		this.gridColumn13.ColumnEdit = this.rep_EDate;
		this.gridColumn13.CurrencyDateField = null;
		this.gridColumn13.CurrencyField = null;
		this.gridColumn13.CurrencyFixed = null;
		this.gridColumn13.CurrencyFixedRate = 0.0;
		this.gridColumn13.CurrencyRateField = null;
		this.gridColumn13.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gridColumn13.FieldName = "ExpDate";
		this.gridColumn13.Name = "gridColumn13";
		this.gridColumn13.OptionsColumn.ReadOnly = true;
		this.gridColumn13.PrimaryKey = false;
		this.gridColumn13.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gridColumn13.UnitField = null;
		this.gridColumn13.Visible = true;
		this.gridColumn13.VisibleIndex = 5;
		this.gridColumn13.Width = 87;
		this.rep_EDate.AutoHeight = false;
		this.rep_EDate.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_EDate.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_EDate.Name = "rep_EDate";
		this.gridColumn14.Caption = "Mfr Date";
		this.gridColumn14.ColumnEdit = this.rep_MDate;
		this.gridColumn14.CurrencyDateField = null;
		this.gridColumn14.CurrencyField = null;
		this.gridColumn14.CurrencyFixed = null;
		this.gridColumn14.CurrencyFixedRate = 0.0;
		this.gridColumn14.CurrencyRateField = null;
		this.gridColumn14.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gridColumn14.FieldName = "MnfDate";
		this.gridColumn14.Name = "gridColumn14";
		this.gridColumn14.OptionsColumn.ReadOnly = true;
		this.gridColumn14.PrimaryKey = false;
		this.gridColumn14.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gridColumn14.UnitField = null;
		this.gridColumn14.Visible = true;
		this.gridColumn14.VisibleIndex = 6;
		this.gridColumn14.Width = 92;
		this.rep_MDate.AutoHeight = false;
		this.rep_MDate.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_MDate.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_MDate.Name = "rep_MDate";
		this.gridColumn15.Caption = "Admission Date";
		this.gridColumn15.ColumnEdit = this.rep_Date;
		this.gridColumn15.CurrencyDateField = null;
		this.gridColumn15.CurrencyField = null;
		this.gridColumn15.CurrencyFixed = null;
		this.gridColumn15.CurrencyFixedRate = 0.0;
		this.gridColumn15.CurrencyRateField = null;
		this.gridColumn15.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gridColumn15.FieldName = "InDate";
		this.gridColumn15.Name = "gridColumn15";
		this.gridColumn15.OptionsColumn.ReadOnly = true;
		this.gridColumn15.PrimaryKey = false;
		this.gridColumn15.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gridColumn15.UnitField = null;
		this.gridColumn15.Visible = true;
		this.gridColumn15.VisibleIndex = 7;
		this.gridColumn15.Width = 88;
		this.rep_Date.AutoHeight = false;
		this.rep_Date.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Date.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Date.Name = "rep_Date";
		this.gridColumn16.Caption = "Mfr Warranty Start";
		this.gridColumn16.ColumnEdit = this.rep_Start;
		this.gridColumn16.CurrencyDateField = null;
		this.gridColumn16.CurrencyField = null;
		this.gridColumn16.CurrencyFixed = null;
		this.gridColumn16.CurrencyFixedRate = 0.0;
		this.gridColumn16.CurrencyRateField = null;
		this.gridColumn16.FieldName = "GrntStart";
		this.gridColumn16.Name = "gridColumn16";
		this.gridColumn16.OptionsColumn.ReadOnly = true;
		this.gridColumn16.PrimaryKey = false;
		this.gridColumn16.UnitField = null;
		this.gridColumn16.Visible = true;
		this.gridColumn16.VisibleIndex = 8;
		this.gridColumn16.Width = 102;
		this.rep_Start.AutoHeight = false;
		this.rep_Start.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Start.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Start.Name = "rep_Start";
		this.gridColumn17.Caption = "Mfr Warranty End";
		this.gridColumn17.ColumnEdit = this.rep_End;
		this.gridColumn17.CurrencyDateField = null;
		this.gridColumn17.CurrencyField = null;
		this.gridColumn17.CurrencyFixed = null;
		this.gridColumn17.CurrencyFixedRate = 0.0;
		this.gridColumn17.CurrencyRateField = null;
		this.gridColumn17.FieldName = "GrntExp";
		this.gridColumn17.Name = "gridColumn17";
		this.gridColumn17.OptionsColumn.ReadOnly = true;
		this.gridColumn17.PrimaryKey = false;
		this.gridColumn17.UnitField = null;
		this.gridColumn17.Visible = true;
		this.gridColumn17.VisibleIndex = 9;
		this.gridColumn17.Width = 95;
		this.rep_End.AutoHeight = false;
		this.rep_End.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_End.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_End.Name = "rep_End";
		this.gridColumn18.Caption = "Whse Code";
		this.gridColumn18.ColumnEdit = this.rep_WhsCode;
		this.gridColumn18.CurrencyDateField = null;
		this.gridColumn18.CurrencyField = null;
		this.gridColumn18.CurrencyFixed = null;
		this.gridColumn18.CurrencyFixedRate = 0.0;
		this.gridColumn18.CurrencyRateField = null;
		this.gridColumn18.FieldName = "WhsCode";
		this.gridColumn18.Name = "gridColumn18";
		this.gridColumn18.OptionsColumn.ReadOnly = true;
		this.gridColumn18.PrimaryKey = false;
		this.gridColumn18.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridColumn18.UnitField = null;
		this.gridColumn18.Visible = true;
		this.gridColumn18.VisibleIndex = 10;
		this.gridColumn18.Width = 84;
		this.rep_WhsCode.AutoHeight = false;
		editorButtonImageOptions2.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_WhsCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_WhsCode.Name = "rep_WhsCode";
		this.rep_WhsCode.ReadOnly = true;
		this.rep_WhsCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_WhsCode_ButtonClick);
		this.gridColumn19.Caption = "Location";
		this.gridColumn19.CurrencyDateField = null;
		this.gridColumn19.CurrencyField = null;
		this.gridColumn19.CurrencyFixed = null;
		this.gridColumn19.CurrencyFixedRate = 0.0;
		this.gridColumn19.CurrencyRateField = null;
		this.gridColumn19.FieldName = "Location";
		this.gridColumn19.Name = "gridColumn19";
		this.gridColumn19.OptionsColumn.ReadOnly = true;
		this.gridColumn19.PrimaryKey = false;
		this.gridColumn19.UnitField = null;
		this.gridColumn19.Visible = true;
		this.gridColumn19.VisibleIndex = 11;
		this.gridColumn19.Width = 65;
		this.gridColumn40.Caption = "Status";
		this.gridColumn40.ColumnEdit = this.rep_Status;
		this.gridColumn40.CurrencyDateField = null;
		this.gridColumn40.CurrencyField = null;
		this.gridColumn40.CurrencyFixed = null;
		this.gridColumn40.CurrencyFixedRate = 0.0;
		this.gridColumn40.CurrencyRateField = null;
		this.gridColumn40.FieldName = "Status";
		this.gridColumn40.Name = "gridColumn40";
		this.gridColumn40.OptionsColumn.ReadOnly = true;
		this.gridColumn40.PrimaryKey = false;
		this.gridColumn40.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gridColumn40.UnitField = null;
		this.gridColumn40.Visible = true;
		this.gridColumn40.VisibleIndex = 12;
		this.gridColumn40.Width = 89;
		this.rep_Status.AutoHeight = false;
		this.rep_Status.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
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
		this.gridColumn21.Caption = "Details";
		this.gridColumn21.CurrencyDateField = null;
		this.gridColumn21.CurrencyField = null;
		this.gridColumn21.CurrencyFixed = null;
		this.gridColumn21.CurrencyFixedRate = 0.0;
		this.gridColumn21.CurrencyRateField = null;
		this.gridColumn21.FieldName = "Notes";
		this.gridColumn21.Name = "gridColumn21";
		this.gridColumn21.OptionsColumn.ReadOnly = true;
		this.gridColumn21.PrimaryKey = false;
		this.gridColumn21.UnitField = null;
		this.gridColumn21.Visible = true;
		this.gridColumn21.VisibleIndex = 13;
		this.gridColumn21.Width = 103;
		this.groupControl1.AppearanceCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.groupControl1.AppearanceCaption.Options.UseFont = true;
		this.groupControl1.Controls.Add(this.grd_BatchTransaction);
		this.groupControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.groupControl1.Location = new System.Drawing.Point(0, 0);
		this.groupControl1.Name = "groupControl1";
		this.groupControl1.Size = new System.Drawing.Size(1054, 405);
		this.groupControl1.TabIndex = 1;
		this.groupControl1.Text = "Transactions for Serial Number";
		this.grd_BatchTransaction.AllowPasteExcel = true;
		this.grd_BatchTransaction.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_BatchTransaction.ClipboardData = null;
		this.grd_BatchTransaction.Location = new System.Drawing.Point(2, 21);
		this.grd_BatchTransaction.MainView = this.gv_BatchTransaction;
		this.grd_BatchTransaction.Name = "grd_BatchTransaction";
		this.grd_BatchTransaction.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[5] { this.rep_Document, this.rep_DocDate, this.rep_Warehorse, this.rep_Type, this.rep_Qty });
		this.grd_BatchTransaction.Size = new System.Drawing.Size(1050, 387);
		this.grd_BatchTransaction.TabIndex = 0;
		this.grd_BatchTransaction.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_BatchTransaction });
		this.gv_BatchTransaction.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_BatchTransaction.ColumnKey = null;
		this.gv_BatchTransaction.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[10] { this.gridColumn2, this.Document, this.DocLine, this.DocDate, this.LocCode, this.BinLocation, this.gridColumn1, this.CardName, this.AllocQty, this.Direction });
		this.gv_BatchTransaction.DisableFormSetting = false;
		this.gv_BatchTransaction.DisplayException = false;
		this.gv_BatchTransaction.EditColumn = null;
		this.gv_BatchTransaction.FieldCurrency = null;
		this.gv_BatchTransaction.FieldRate = null;
		this.gv_BatchTransaction.FormId = "oSerialNumberTransaction";
		this.gv_BatchTransaction.GridControl = this.grd_BatchTransaction;
		this.gv_BatchTransaction.IgnoreFilter = false;
		this.gv_BatchTransaction.IndexEditRow = -1;
		this.gv_BatchTransaction.IndicatorWidth = 35;
		this.gv_BatchTransaction.IsApzTable = false;
		this.gv_BatchTransaction.IsInsertNewRow = false;
		this.gv_BatchTransaction.IsLoadingData = false;
		this.gv_BatchTransaction.IsMappingContextAction = false;
		this.gv_BatchTransaction.ItemId = "Transaction";
		this.gv_BatchTransaction.LstColumnAllowBatch = null;
		this.gv_BatchTransaction.LstColumnNotAllowBatch = null;
		this.gv_BatchTransaction.LstColumnNotEditable = null;
		this.gv_BatchTransaction.LstColumNotSetting = null;
		this.gv_BatchTransaction.Name = "gv_BatchTransaction";
		this.gv_BatchTransaction.OptionsBehavior.ReadOnly = true;
		this.gv_BatchTransaction.OptionsView.ColumnAutoWidth = false;
		this.gv_BatchTransaction.OptionsView.ShowGroupPanel = false;
		this.gv_BatchTransaction.UseBatchMod = false;
		this.gv_BatchTransaction.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_BatchTransaction_CustomDrawRowIndicator);
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
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.PrimaryKey = true;
		this.gridColumn2.UnitField = null;
		this.gridColumn2.Visible = true;
		this.gridColumn2.VisibleIndex = 0;
		this.gridColumn2.Width = 129;
		this.rep_Type.AutoHeight = false;
		this.rep_Type.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo, "", -1, false, true, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
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
		this.Document.Name = "Document";
		this.Document.OptionsColumn.ReadOnly = true;
		this.Document.PrimaryKey = false;
		this.Document.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.Document.UnitField = null;
		this.Document.Visible = true;
		this.Document.VisibleIndex = 1;
		this.Document.Width = 98;
		this.rep_Document.AutoHeight = false;
		editorButtonImageOptions3.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_Document.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance4, appearanceHovered4, appearancePressed4, appearanceDisabled4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
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
		this.DocDate.ColumnEdit = this.rep_DocDate;
		this.DocDate.CurrencyDateField = null;
		this.DocDate.CurrencyField = null;
		this.DocDate.CurrencyFixed = null;
		this.DocDate.CurrencyFixedRate = 0.0;
		this.DocDate.CurrencyRateField = null;
		this.DocDate.FieldName = "DocDate";
		this.DocDate.Name = "DocDate";
		this.DocDate.OptionsColumn.ReadOnly = true;
		this.DocDate.PrimaryKey = false;
		this.DocDate.UnitField = null;
		this.DocDate.Visible = true;
		this.DocDate.VisibleIndex = 3;
		this.DocDate.Width = 92;
		this.rep_DocDate.AutoHeight = false;
		this.rep_DocDate.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_DocDate.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_DocDate.Name = "rep_DocDate";
		this.LocCode.Caption = "Whse";
		this.LocCode.ColumnEdit = this.rep_Warehorse;
		this.LocCode.CurrencyDateField = null;
		this.LocCode.CurrencyField = null;
		this.LocCode.CurrencyFixed = null;
		this.LocCode.CurrencyFixedRate = 0.0;
		this.LocCode.CurrencyRateField = null;
		this.LocCode.FieldName = "LocCode";
		this.LocCode.Name = "LocCode";
		this.LocCode.OptionsColumn.ReadOnly = true;
		this.LocCode.PrimaryKey = false;
		this.LocCode.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.LocCode.UnitField = null;
		this.LocCode.Visible = true;
		this.LocCode.VisibleIndex = 4;
		this.LocCode.Width = 126;
		this.rep_Warehorse.AutoHeight = false;
		editorButtonImageOptions4.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_Warehorse.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance5, appearanceHovered5, appearancePressed5, appearanceDisabled5, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_Warehorse.Name = "rep_Warehorse";
		this.rep_Warehorse.ReadOnly = true;
		this.rep_Warehorse.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_Warehorse_ButtonClick);
		this.BinLocation.Caption = "Bin Location";
		this.BinLocation.CurrencyDateField = null;
		this.BinLocation.CurrencyField = null;
		this.BinLocation.CurrencyFixed = null;
		this.BinLocation.CurrencyFixedRate = 0.0;
		this.BinLocation.CurrencyRateField = null;
		this.BinLocation.FieldName = "DftBinAbs";
		this.BinLocation.Name = "BinLocation";
		this.BinLocation.OptionsColumn.ReadOnly = true;
		this.BinLocation.PrimaryKey = false;
		this.BinLocation.UnitField = null;
		this.BinLocation.Visible = true;
		this.BinLocation.VisibleIndex = 5;
		this.BinLocation.Width = 107;
		this.gridColumn1.Caption = "G/L Acct/BP Code";
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "CardCode";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.PrimaryKey = false;
		this.gridColumn1.UnitField = null;
		this.gridColumn1.Visible = true;
		this.gridColumn1.VisibleIndex = 6;
		this.gridColumn1.Width = 117;
		this.CardName.Caption = "G/L Acct/BP Name";
		this.CardName.CurrencyDateField = null;
		this.CardName.CurrencyField = null;
		this.CardName.CurrencyFixed = null;
		this.CardName.CurrencyFixedRate = 0.0;
		this.CardName.CurrencyRateField = null;
		this.CardName.FieldName = "CardName";
		this.CardName.Name = "CardName";
		this.CardName.OptionsColumn.ReadOnly = true;
		this.CardName.PrimaryKey = false;
		this.CardName.UnitField = null;
		this.CardName.Visible = true;
		this.CardName.VisibleIndex = 7;
		this.CardName.Width = 301;
		this.AllocQty.Caption = "Allocated";
		this.AllocQty.ColumnEdit = this.rep_Qty;
		this.AllocQty.CurrencyDateField = null;
		this.AllocQty.CurrencyField = null;
		this.AllocQty.CurrencyFixed = null;
		this.AllocQty.CurrencyFixedRate = 0.0;
		this.AllocQty.CurrencyRateField = null;
		this.AllocQty.FieldName = "AllocQty";
		this.AllocQty.Name = "AllocQty";
		this.AllocQty.OptionsColumn.ReadOnly = true;
		this.AllocQty.PrimaryKey = false;
		this.AllocQty.UnitField = null;
		this.AllocQty.Visible = true;
		this.AllocQty.VisibleIndex = 8;
		this.rep_Qty.AutoHeight = false;
		this.rep_Qty.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo, "", -1, false, true, false, imageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance6, appearanceHovered6, appearancePressed6, appearanceDisabled6, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_Qty.Name = "rep_Qty";
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
		this.Direction.VisibleIndex = 9;
		this.Direction.Width = 100;
		this.btn_ItemCodeTo.EditControl = null;
		this.btn_ItemCodeTo.FieldName = "ToItem";
		this.btn_ItemCodeTo.IsSearchControl = true;
		this.btn_ItemCodeTo.Location = new System.Drawing.Point(101, 35);
		this.btn_ItemCodeTo.Name = "btn_ItemCodeTo";
		this.btn_ItemCodeTo.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions5.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions8.Image");
		this.btn_ItemCodeTo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance7, appearanceHovered7, appearancePressed7, appearanceDisabled7, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions5, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance8, appearanceHovered8, appearancePressed8, appearanceDisabled8, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
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
		this.btn_ItemCodeTo.TabIndex = 38;
		this.btn_ItemCodeFrom.EditControl = null;
		this.btn_ItemCodeFrom.FieldName = "FromItem";
		this.btn_ItemCodeFrom.IsSearchControl = true;
		this.btn_ItemCodeFrom.Location = new System.Drawing.Point(101, 10);
		this.btn_ItemCodeFrom.Name = "btn_ItemCodeFrom";
		this.btn_ItemCodeFrom.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions6.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions10.Image");
		this.btn_ItemCodeFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance9, appearanceHovered9, appearancePressed9, appearanceDisabled9, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions6, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance10, appearanceHovered10, appearancePressed10, appearanceDisabled10, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
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
		this.btn_ItemCodeFrom.TabIndex = 37;
		this.txt_DistNumber.EditControl = null;
		this.txt_DistNumber.FieldName = "DistNumber";
		this.txt_DistNumber.IsSearchControl = true;
		this.txt_DistNumber.Location = new System.Drawing.Point(311, 9);
		this.txt_DistNumber.Name = "txt_DistNumber";
		this.txt_DistNumber.SettingInVisible = false;
		this.txt_DistNumber.SettingReadonly = false;
		this.txt_DistNumber.Size = new System.Drawing.Size(147, 20);
		this.txt_DistNumber.TabIndex = 36;
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "Serial Number";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(235, 12);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(76, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 35;
		this.labelLine4.Utext = "Serial Number";
		this.look_Status.EditControl = null;
		this.look_Status.FieldName = "Status";
		this.look_Status.IsSearchControl = false;
		this.look_Status.Location = new System.Drawing.Point(311, 35);
		this.look_Status.Name = "look_Status";
		this.look_Status.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
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
		this.look_Status.TabIndex = 34;
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "Status";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(235, 38);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(76, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 33;
		this.labelLine2.Utext = "Status";
		this.btn_Search.EditControl = null;
		this.btn_Search.EditValue = "Search";
		this.btn_Search.FieldName = "btnSearch";
		this.btn_Search.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Search.ImageOptions.Image");
		this.btn_Search.IsSearchControl = true;
		this.btn_Search.Location = new System.Drawing.Point(464, 35);
		this.btn_Search.Name = "btn_Search";
		this.btn_Search.ReadOnly = false;
		this.btn_Search.SettingInVisible = false;
		this.btn_Search.SettingReadonly = false;
		this.btn_Search.Size = new System.Drawing.Size(81, 20);
		this.btn_Search.TabIndex = 32;
		this.btn_Search.Text = "Search";
		this.btn_Search.Click += new System.EventHandler(this.btn_Search_Click);
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "Item Code To";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(12, 38);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(90, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 31;
		this.labelLine3.Utext = "Item Code To";
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Item Code From";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(12, 12);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(90, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 30;
		this.labelLine1.Utext = "Item Code From";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1055, 642);
		base.Controls.Add(this.btn_ItemCodeTo);
		base.Controls.Add(this.btn_ItemCodeFrom);
		base.Controls.Add(this.txt_DistNumber);
		base.Controls.Add(this.labelLine4);
		base.Controls.Add(this.look_Status);
		base.Controls.Add(this.labelLine2);
		base.Controls.Add(this.btn_Search);
		base.Controls.Add(this.labelLine3);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.splitContainerControl1);
		base.FormId = "oSerialNumberTransaction";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_SerialNumberTransaction";
		this.Text = "Serial Number Transactions Report";
		base.Load += new System.EventHandler(this.frm_SerialNumberTransaction_Load);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.groupControl2).EndInit();
		this.groupControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_SerialNumber).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_SerialNumber).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ItemCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_EDate.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_EDate).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_MDate.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_MDate).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Date.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Date).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Start.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Start).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_End.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_End).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_WhsCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Status).EndInit();
		((System.ComponentModel.ISupportInitialize)this.groupControl1).EndInit();
		this.groupControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_BatchTransaction).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_BatchTransaction).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Type).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Document).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_DocDate.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_DocDate).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Warehorse).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Qty).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_ItemCodeTo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_ItemCodeFrom.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_DistNumber.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.look_Status.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
