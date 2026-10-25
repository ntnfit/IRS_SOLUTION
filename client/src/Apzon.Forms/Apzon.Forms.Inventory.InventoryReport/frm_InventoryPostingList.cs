using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
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
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.Inventory.InventoryReport;

public class frm_InventoryPostingList : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	public DateTime DateFrom = DateTime.Today;

	public DateTime DateTo = DateTime.Today.AddDays(Configuration.SearchDate);

	private IContainer components;

	private LabelLine labelControl1;

	private ButtonEditEx txt_ItemCodeFrom;

	private ButtonEditEx txt_ItemTo;

	private LabelLine labelLine1;

	private ButtonEditEx txt_BpTo;

	private LabelLine labelLine2;

	private ButtonEditEx txt_BpFrom;

	private LabelLine labelLine3;

	private LabelLine labelLine4;

	private LabelLine labelLine5;

	private DateEditEx date_From;

	private DateEditEx date_To;

	private LabelLine labelLine6;

	private CheckedComboBoxEdit chkcbx_WhsCode;

	private GridControlEx grd_Data;

	private GridViewEx gv_Data;

	private SimpleButtonEx btn_OK;

	private SimpleButtonEx btn_Search;

	private GridColumnEx gridColumn1;

	private GridColumnEx gridColumn2;

	private GridColumnEx gridColumn3;

	private GridColumnEx gridColumn4;

	private RepositoryItemButtonEdit rep_Document;

	private GridColumnEx gridColumn5;

	private GridColumnEx gridColumn6;

	private GridColumnEx gridColumn7;

	private GridColumnEx gridColumn8;

	private GridColumnEx gridColumn9;

	private GridColumnEx gridColumn10;

	private GridColumnEx gridColumn11;

	private GridColumnEx gridColumn12;

	private GridColumnEx gridColumn13;

	private GridColumnEx gridColumn14;

	private RepositoryItemButtonEdit rep_Item;

	private RepositoryItemLookUpEdit look_TransType;

	private RepositoryItemButtonEdit rep_CardCode;

	public string ItemFrom { get; set; }

	public string ItemTo { get; set; }

	public string BpFrom { get; set; }

	public string BpTo { get; set; }

	public string WhsCode { get; set; }

	public bool GroupByItem { get; set; }

	public bool GroupByBp { get; set; }

	public frm_InventoryPostingList()
	{
		InitializeComponent();
	}

	private async void SetDataWhs()
	{
		try
		{
			RepositoryItemCheckedComboBoxEdit properties = chkcbx_WhsCode.Properties;
			properties.DataSource = await GetListWhs();
			chkcbx_WhsCode.EditValue = "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task<List<string>> GetListWhs()
	{
		List<string> lst = new List<string>();
		try
		{
			DataTable dataTable = await _httpClient.Warehouse.GetAllCode();
			if (dataTable.IsNotNull())
			{
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					lst.Add(Function.ToString(dataTable.Rows[i]["WhsCode"]));
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return lst;
	}

	private string ChooseFromListItems(string itemcode)
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.ListItems, false, itemcode, "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem != null && frm_CFL2.ChooseItem.IsExistsColumn("ItemCode"))
			{
				return Function.ToString(frm_CFL2.ChooseItem["ItemCode"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return "";
	}

	private string ChooseFromListBp(string cardcode)
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.Customer, false, cardcode, "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem != null && frm_CFL2.ChooseItem.IsExistsColumn("CardCode"))
			{
				return Function.ToString(frm_CFL2.ChooseItem["CardCode"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return "";
	}

	private void txt_ItemCodeFrom_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			string itemcode = Function.ToString(txt_ItemCodeFrom.EditValue);
			txt_ItemCodeFrom.EditValue = ChooseFromListItems(itemcode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_ItemTo_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			string itemcode = Function.ToString(txt_ItemTo.EditValue);
			txt_ItemTo.EditValue = ChooseFromListItems(itemcode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_ItemCodeFrom_KeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.Tab)
			{
				string itemcode = Function.ToString(txt_ItemCodeFrom.EditValue);
				txt_ItemCodeFrom.EditValue = ChooseFromListItems(itemcode);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_ItemTo_KeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.Tab)
			{
				string itemcode = Function.ToString(txt_ItemTo.EditValue);
				txt_ItemTo.EditValue = ChooseFromListItems(itemcode);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_OK_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void txt_BpFrom_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			string cardcode = Function.ToString(txt_BpFrom.EditValue);
			txt_BpFrom.EditValue = ChooseFromListBp(cardcode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_BpTo_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			string cardcode = Function.ToString(txt_BpTo.EditValue);
			txt_BpTo.EditValue = ChooseFromListBp(cardcode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_BpFrom_KeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.Tab)
			{
				string cardcode = Function.ToString(txt_BpFrom.EditValue);
				txt_BpFrom.EditValue = ChooseFromListBp(cardcode);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_BpTo_KeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.Tab)
			{
				string cardcode = Function.ToString(txt_BpTo.EditValue);
				txt_BpTo.EditValue = ChooseFromListBp(cardcode);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void frm_InventoryPostingList_Load(object sender, EventArgs e)
	{
		try
		{
			SetDataWhs();
			txt_ItemCodeFrom.EditValue = ItemFrom;
			txt_ItemTo.EditValue = ItemTo;
			txt_BpFrom.EditValue = BpFrom;
			txt_BpTo.EditValue = BpTo;
			date_From.EditValue = DateFrom;
			date_To.EditValue = DateTo;
			chkcbx_WhsCode.EditValue = WhsCode;
			if (GroupByItem)
			{
				gv_Data.Columns["ItemCode"].GroupIndex = 0;
			}
			if (GroupByBp)
			{
				gv_Data.Columns["ItemCode"].GroupIndex = 1;
			}
			gv_Data.Columns["DocDate"].DisplayFormat.FormatType = FormatType.DateTime;
			gv_Data.Columns["DocDate"].DisplayFormat.FormatString = GlobalData.DateTimeFormat;
			gv_Data.Columns["Quantity"].DisplayFormat.FormatType = FormatType.Numeric;
			gv_Data.Columns["Quantity"].DisplayFormat.FormatString = "n" + GlobalData.QuantitiesFormat.NumberDecimalDigits;
			gv_Data.Columns["Balance"].DisplayFormat.FormatType = FormatType.Numeric;
			gv_Data.Columns["Balance"].DisplayFormat.FormatString = "n" + GlobalData.QuantitiesFormat.NumberDecimalDigits;
			gv_Data.Columns["Price"].DisplayFormat.FormatType = FormatType.Numeric;
			gv_Data.Columns["Price"].DisplayFormat.FormatString = "n" + GlobalData.UnitPriceFormat.CurrencyDecimalDigits;
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Code", typeof(string));
			dataTable.Columns.Add("Name", typeof(string));
			DataRow dataRow = dataTable.NewRow();
			dataRow["Code"] = "15";
			dataRow["Name"] = "Delivery";
			dataTable.Rows.Add(dataRow);
			dataRow = dataTable.NewRow();
			dataRow["Code"] = "16";
			dataRow["Name"] = "Returns";
			dataTable.Rows.Add(dataRow);
			dataRow = dataTable.NewRow();
			dataRow["Code"] = "13";
			dataRow["Name"] = "A/R Invoice";
			dataTable.Rows.Add(dataRow);
			dataRow = dataTable.NewRow();
			dataRow["Code"] = "14";
			dataRow["Name"] = "A/R Credit Memo";
			dataTable.Rows.Add(dataRow);
			dataRow = dataTable.NewRow();
			dataRow["Code"] = "132";
			dataRow["Name"] = "Correction Invoice";
			dataTable.Rows.Add(dataRow);
			dataRow = dataTable.NewRow();
			dataRow["Code"] = "20";
			dataRow["Name"] = "Goods Receipt PO";
			dataTable.Rows.Add(dataRow);
			dataRow = dataTable.NewRow();
			dataRow["Code"] = "21";
			dataRow["Name"] = "Goods Return";
			dataTable.Rows.Add(dataRow);
			dataRow = dataTable.NewRow();
			dataRow["Code"] = "18";
			dataRow["Name"] = "A/P Invoice";
			dataTable.Rows.Add(dataRow);
			dataRow = dataTable.NewRow();
			dataRow["Code"] = "19";
			dataRow["Name"] = "A/P Credit Memo";
			dataTable.Rows.Add(dataRow);
			dataRow = dataTable.NewRow();
			dataRow["Code"] = "58";
			dataRow["Name"] = "Inventory Update";
			dataTable.Rows.Add(dataRow);
			dataRow = dataTable.NewRow();
			dataRow["Code"] = "59";
			dataRow["Name"] = "Goods Receipt";
			dataTable.Rows.Add(dataRow);
			dataRow = dataTable.NewRow();
			dataRow["Code"] = "60";
			dataRow["Name"] = "Goods Issue";
			dataTable.Rows.Add(dataRow);
			dataRow = dataTable.NewRow();
			dataRow["Code"] = "67";
			dataRow["Name"] = "Inventory Transfer";
			dataTable.Rows.Add(dataRow);
			dataRow = dataTable.NewRow();
			dataRow["Code"] = "-1";
			dataRow["Name"] = "All Transaction";
			dataTable.Rows.Add(dataRow);
			dataRow = dataTable.NewRow();
			dataRow["Code"] = "68";
			dataRow["Name"] = "Work Instructions";
			dataTable.Rows.Add(dataRow);
			dataRow = dataTable.NewRow();
			dataRow["Code"] = "162";
			dataRow["Name"] = "Inventory Revaluation";
			dataTable.Rows.Add(dataRow);
			dataRow = dataTable.NewRow();
			dataRow["Code"] = "69";
			dataRow["Name"] = "Landed Cost";
			dataTable.Rows.Add(dataRow);
			dataRow = dataTable.NewRow();
			dataRow["Code"] = "310000001";
			dataRow["Name"] = "Initial Quantity";
			dataTable.Rows.Add(dataRow);
			dataRow = dataTable.NewRow();
			dataRow["Code"] = "10000071";
			dataRow["Name"] = "Inventory Posting";
			dataTable.Rows.Add(dataRow);
			look_TransType.DataSource = dataTable;
			btn_Search_Click(null, null);
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
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("ItemFrom", typeof(string));
			dataTable.Columns.Add("ItemTo", typeof(string));
			dataTable.Columns.Add("BpFrom", typeof(string));
			dataTable.Columns.Add("BpTo", typeof(string));
			dataTable.Columns.Add("DateFrom", typeof(DateTime));
			dataTable.Columns.Add("DateTo", typeof(DateTime));
			dataTable.Columns.Add("WhsCode", typeof(string));
			DataRow dataRow = dataTable.NewRow();
			dataRow["ItemFrom"] = Function.ToString(txt_ItemCodeFrom.EditValue);
			dataRow["ItemTo"] = Function.ToString(txt_ItemTo.EditValue);
			dataRow["BpFrom"] = Function.ToString(txt_BpFrom.EditValue);
			dataRow["BpTo"] = Function.ToString(txt_BpTo.EditValue);
			dataRow["DateFrom"] = Function.ParseDateTime(date_From.EditValue) ?? new DateTime(1900, 1, 1);
			dataRow["DateTo"] = Function.ParseDateTime(date_To.EditValue) ?? DateTime.Today;
			dataRow["WhsCode"] = Function.ToString(chkcbx_WhsCode.EditValue);
			dataTable.Rows.Add(dataRow);
			DataTable dataSource = await _httpClient.InventoryPostingList.GetData(dataTable);
			grd_Data.DataSource = dataSource;
			grd_Data.RefreshDataSource();
			gv_Data.ExpandAllGroups();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rep_Document_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			string id = Function.ToString(gv_Data.GetFocusedRowCellValue("BASE_REF"));
			int num = Function.ParseInt(gv_Data.GetFocusedRowCellValue("TransType"));
			this.DrillDown((ApzObjectTypes)num, (ApzObjectTypes)num, id);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rep_Item_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			string id = Function.ToString(gv_Data.GetFocusedRowCellValue("ItemCode"));
			this.DrillDown(ApzObjectTypes.oItems, ApzObjectTypes.oItems, id);
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
			string id = Function.ToString(gv_Data.GetFocusedRowCellValue("BPCardCode"));
			this.DrillDown(ApzObjectTypes.oBusinessPartners, ApzObjectTypes.oBusinessPartners, id);
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Inventory.InventoryReport.frm_InventoryPostingList));
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
		this.labelControl1 = new Apzon.Controls.Controls.LabelLine();
		this.txt_ItemCodeFrom = new Apzon.Controls.Editor.ButtonEditEx();
		this.txt_ItemTo = new Apzon.Controls.Editor.ButtonEditEx();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.txt_BpTo = new Apzon.Controls.Editor.ButtonEditEx();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.txt_BpFrom = new Apzon.Controls.Editor.ButtonEditEx();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine5 = new Apzon.Controls.Controls.LabelLine();
		this.date_From = new Apzon.Controls.Editor.DateEditEx();
		this.date_To = new Apzon.Controls.Editor.DateEditEx();
		this.labelLine6 = new Apzon.Controls.Controls.LabelLine();
		this.chkcbx_WhsCode = new DevExpress.XtraEditors.CheckedComboBoxEdit();
		this.grd_Data = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Data = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Item = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gridColumn2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn3 = new Apzon.Controls.Grid.GridColumnEx();
		this.look_TransType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumn4 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Document = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gridColumn5 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn6 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_CardCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gridColumn7 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn8 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn9 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn10 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn11 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn12 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn13 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn14 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Search = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.txt_ItemCodeFrom.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ItemTo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BpTo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BpFrom.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_From.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_From.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_To.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_To.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chkcbx_WhsCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Data).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Data).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Item).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.look_TransType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Document).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CardCode).BeginInit();
		base.SuspendLayout();
		this.labelControl1.EditControl = null;
		this.labelControl1.EditValue = "Items - From";
		this.labelControl1.FieldName = null;
		this.labelControl1.HideWhenEmpty = false;
		this.labelControl1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelControl1.Location = new System.Drawing.Point(13, 13);
		this.labelControl1.Name = "labelControl1";
		this.labelControl1.ReadOnly = false;
		this.labelControl1.SettingInVisible = false;
		this.labelControl1.SettingReadonly = false;
		this.labelControl1.ShowUnderLine = true;
		this.labelControl1.Size = new System.Drawing.Size(127, 20);
		this.labelControl1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelControl1.TabIndex = 0;
		this.labelControl1.Utext = "Items - From";
		this.txt_ItemCodeFrom.EditControl = null;
		this.txt_ItemCodeFrom.FieldName = null;
		this.txt_ItemCodeFrom.IsSearchControl = false;
		this.txt_ItemCodeFrom.Location = new System.Drawing.Point(137, 11);
		this.txt_ItemCodeFrom.Name = "txt_ItemCodeFrom";
		this.txt_ItemCodeFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.txt_ItemCodeFrom.Properties.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txt_ItemCodeFrom_Properties_ButtonClick);
		this.txt_ItemCodeFrom.Properties.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_ItemCodeFrom_KeyDown);
		this.txt_ItemCodeFrom.SettingInVisible = false;
		this.txt_ItemCodeFrom.SettingReadonly = false;
		this.txt_ItemCodeFrom.Size = new System.Drawing.Size(142, 20);
		this.txt_ItemCodeFrom.TabIndex = 0;
		this.txt_ItemCodeFrom.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_ItemCodeFrom_KeyDown);
		this.txt_ItemTo.EditControl = null;
		this.txt_ItemTo.FieldName = null;
		this.txt_ItemTo.IsSearchControl = false;
		this.txt_ItemTo.Location = new System.Drawing.Point(360, 11);
		this.txt_ItemTo.Name = "txt_ItemTo";
		this.txt_ItemTo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.txt_ItemTo.Properties.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txt_ItemTo_Properties_ButtonClick);
		this.txt_ItemTo.Properties.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_ItemTo_KeyDown);
		this.txt_ItemTo.SettingInVisible = false;
		this.txt_ItemTo.SettingReadonly = false;
		this.txt_ItemTo.Size = new System.Drawing.Size(142, 20);
		this.txt_ItemTo.TabIndex = 1;
		this.txt_ItemTo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_ItemTo_KeyDown);
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "To";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(281, 13);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(80, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 2;
		this.labelLine1.Utext = "To";
		this.txt_BpTo.EditControl = null;
		this.txt_BpTo.FieldName = null;
		this.txt_BpTo.IsSearchControl = false;
		this.txt_BpTo.Location = new System.Drawing.Point(359, 36);
		this.txt_BpTo.Name = "txt_BpTo";
		this.txt_BpTo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.txt_BpTo.Properties.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txt_BpTo_Properties_ButtonClick);
		this.txt_BpTo.Properties.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_BpTo_KeyDown);
		this.txt_BpTo.SettingInVisible = false;
		this.txt_BpTo.SettingReadonly = false;
		this.txt_BpTo.Size = new System.Drawing.Size(142, 20);
		this.txt_BpTo.TabIndex = 3;
		this.txt_BpTo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_BpTo_KeyDown);
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "To";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(280, 39);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(80, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 6;
		this.labelLine2.Utext = "To";
		this.txt_BpFrom.EditControl = null;
		this.txt_BpFrom.FieldName = null;
		this.txt_BpFrom.IsSearchControl = false;
		this.txt_BpFrom.Location = new System.Drawing.Point(137, 36);
		this.txt_BpFrom.Name = "txt_BpFrom";
		this.txt_BpFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.txt_BpFrom.Properties.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txt_BpFrom_Properties_ButtonClick);
		this.txt_BpFrom.Properties.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_BpFrom_KeyDown);
		this.txt_BpFrom.SettingInVisible = false;
		this.txt_BpFrom.SettingReadonly = false;
		this.txt_BpFrom.Size = new System.Drawing.Size(142, 20);
		this.txt_BpFrom.TabIndex = 2;
		this.txt_BpFrom.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_BpFrom_KeyDown);
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "Business Partner - From";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(12, 39);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(128, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 4;
		this.labelLine3.Utext = "Business Partner - From";
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "To";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(280, 65);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(80, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 9;
		this.labelLine4.Utext = "To";
		this.labelLine5.EditControl = null;
		this.labelLine5.EditValue = "Date - From";
		this.labelLine5.FieldName = null;
		this.labelLine5.HideWhenEmpty = false;
		this.labelLine5.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine5.Location = new System.Drawing.Point(12, 65);
		this.labelLine5.Name = "labelLine5";
		this.labelLine5.ReadOnly = false;
		this.labelLine5.SettingInVisible = false;
		this.labelLine5.SettingReadonly = false;
		this.labelLine5.ShowUnderLine = true;
		this.labelLine5.Size = new System.Drawing.Size(128, 20);
		this.labelLine5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine5.TabIndex = 8;
		this.labelLine5.Utext = "Date - From";
		this.date_From.EditControl = null;
		this.date_From.EditValue = null;
		this.date_From.FieldName = null;
		this.date_From.Location = new System.Drawing.Point(137, 63);
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
		this.date_From.Size = new System.Drawing.Size(142, 20);
		this.date_From.TabIndex = 4;
		this.date_To.EditControl = null;
		this.date_To.EditValue = null;
		this.date_To.FieldName = null;
		this.date_To.Location = new System.Drawing.Point(359, 63);
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
		this.date_To.Size = new System.Drawing.Size(142, 20);
		this.date_To.TabIndex = 5;
		this.labelLine6.EditControl = null;
		this.labelLine6.EditValue = "Warehouse";
		this.labelLine6.FieldName = null;
		this.labelLine6.HideWhenEmpty = false;
		this.labelLine6.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine6.Location = new System.Drawing.Point(507, 63);
		this.labelLine6.Name = "labelLine6";
		this.labelLine6.ReadOnly = false;
		this.labelLine6.SettingInVisible = false;
		this.labelLine6.SettingReadonly = false;
		this.labelLine6.ShowUnderLine = true;
		this.labelLine6.Size = new System.Drawing.Size(127, 20);
		this.labelLine6.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine6.TabIndex = 12;
		this.labelLine6.Utext = "Warehouse";
		this.chkcbx_WhsCode.Location = new System.Drawing.Point(633, 60);
		this.chkcbx_WhsCode.Name = "chkcbx_WhsCode";
		this.chkcbx_WhsCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.chkcbx_WhsCode.Size = new System.Drawing.Size(142, 20);
		this.chkcbx_WhsCode.TabIndex = 6;
		this.grd_Data.AllowPasteExcel = true;
		this.grd_Data.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Data.ClipboardData = null;
		this.grd_Data.Location = new System.Drawing.Point(0, 92);
		this.grd_Data.MainView = this.gv_Data;
		this.grd_Data.Name = "grd_Data";
		this.grd_Data.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[4] { this.rep_Document, this.rep_Item, this.look_TransType, this.rep_CardCode });
		this.grd_Data.Size = new System.Drawing.Size(884, 435);
		this.grd_Data.TabIndex = 14;
		this.grd_Data.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Data });
		this.gv_Data.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Data.ColumnKey = null;
		this.gv_Data.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[14]
		{
			this.gridColumn1, this.gridColumn2, this.gridColumn3, this.gridColumn4, this.gridColumn5, this.gridColumn6, this.gridColumn7, this.gridColumn8, this.gridColumn9, this.gridColumn10,
			this.gridColumn11, this.gridColumn12, this.gridColumn13, this.gridColumn14
		});
		this.gv_Data.DisableFormSetting = false;
		this.gv_Data.DisplayException = false;
		this.gv_Data.EditColumn = null;
		this.gv_Data.FieldCurrency = null;
		this.gv_Data.FieldRate = null;
		this.gv_Data.FormId = "oInventoryPostingList";
		this.gv_Data.GridControl = this.grd_Data;
		this.gv_Data.IgnoreFilter = false;
		this.gv_Data.IndexEditRow = -1;
		this.gv_Data.IndicatorWidth = 35;
		this.gv_Data.IsApzTable = false;
		this.gv_Data.IsInsertNewRow = false;
		this.gv_Data.IsLoadingData = false;
		this.gv_Data.IsMappingContextAction = false;
		this.gv_Data.ItemId = "ODATA";
		this.gv_Data.LstColumnAllowBatch = null;
		this.gv_Data.LstColumnNotAllowBatch = null;
		this.gv_Data.LstColumnNotEditable = null;
		this.gv_Data.LstColumNotSetting = null;
		this.gv_Data.Name = "gv_Data";
		this.gv_Data.OptionsView.ColumnAutoWidth = false;
		this.gv_Data.UseBatchMod = false;
		this.gridColumn1.Caption = "Item Code";
		this.gridColumn1.ColumnEdit = this.rep_Item;
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "ItemCode";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.OptionsColumn.ReadOnly = true;
		this.gridColumn1.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.gridColumn1.PrimaryKey = true;
		this.gridColumn1.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridColumn1.UnitField = null;
		this.gridColumn1.Visible = true;
		this.gridColumn1.VisibleIndex = 0;
		this.gridColumn1.Width = 100;
		this.rep_Item.AutoHeight = false;
		editorButtonImageOptions.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions1.Image");
		this.rep_Item.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_Item.Name = "rep_Item";
		this.rep_Item.ReadOnly = true;
		this.rep_Item.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_Item_ButtonClick);
		this.gridColumn2.Caption = "Posting Date";
		this.gridColumn2.CurrencyDateField = null;
		this.gridColumn2.CurrencyField = null;
		this.gridColumn2.CurrencyFixed = null;
		this.gridColumn2.CurrencyFixedRate = 0.0;
		this.gridColumn2.CurrencyRateField = null;
		this.gridColumn2.FieldName = "DocDate";
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.OptionsColumn.ReadOnly = true;
		this.gridColumn2.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.gridColumn2.PrimaryKey = false;
		this.gridColumn2.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridColumn2.UnitField = null;
		this.gridColumn2.Visible = true;
		this.gridColumn2.VisibleIndex = 1;
		this.gridColumn2.Width = 100;
		this.gridColumn3.Caption = "Trans. Type";
		this.gridColumn3.ColumnEdit = this.look_TransType;
		this.gridColumn3.CurrencyDateField = null;
		this.gridColumn3.CurrencyField = null;
		this.gridColumn3.CurrencyFixed = null;
		this.gridColumn3.CurrencyFixedRate = 0.0;
		this.gridColumn3.CurrencyRateField = null;
		this.gridColumn3.FieldName = "TransType";
		this.gridColumn3.Name = "gridColumn3";
		this.gridColumn3.OptionsColumn.ReadOnly = true;
		this.gridColumn3.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.gridColumn3.PrimaryKey = false;
		this.gridColumn3.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridColumn3.UnitField = null;
		this.gridColumn3.Visible = true;
		this.gridColumn3.VisibleIndex = 2;
		this.gridColumn3.Width = 100;
		this.look_TransType.AutoHeight = false;
		this.look_TransType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.look_TransType.DisplayMember = "Name";
		this.look_TransType.Name = "look_TransType";
		this.look_TransType.NullText = "";
		this.look_TransType.ValueMember = "Code";
		this.gridColumn4.Caption = "Document";
		this.gridColumn4.ColumnEdit = this.rep_Document;
		this.gridColumn4.CurrencyDateField = null;
		this.gridColumn4.CurrencyField = null;
		this.gridColumn4.CurrencyFixed = null;
		this.gridColumn4.CurrencyFixedRate = 0.0;
		this.gridColumn4.CurrencyRateField = null;
		this.gridColumn4.FieldName = "BASE_REF";
		this.gridColumn4.Name = "gridColumn4";
		this.gridColumn4.OptionsColumn.ReadOnly = true;
		this.gridColumn4.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.gridColumn4.PrimaryKey = false;
		this.gridColumn4.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridColumn4.UnitField = null;
		this.gridColumn4.Visible = true;
		this.gridColumn4.VisibleIndex = 3;
		this.gridColumn4.Width = 100;
		this.rep_Document.AutoHeight = false;
		editorButtonImageOptions2.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions2.Image");
		this.rep_Document.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_Document.Name = "rep_Document";
		this.rep_Document.ReadOnly = true;
		this.rep_Document.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_Document_ButtonClick);
		this.gridColumn5.Caption = "Doc. Row";
		this.gridColumn5.CurrencyDateField = null;
		this.gridColumn5.CurrencyField = null;
		this.gridColumn5.CurrencyFixed = null;
		this.gridColumn5.CurrencyFixedRate = 0.0;
		this.gridColumn5.CurrencyRateField = null;
		this.gridColumn5.FieldName = "DocLineNum";
		this.gridColumn5.Name = "gridColumn5";
		this.gridColumn5.OptionsColumn.ReadOnly = true;
		this.gridColumn5.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.gridColumn5.PrimaryKey = false;
		this.gridColumn5.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridColumn5.UnitField = null;
		this.gridColumn5.Visible = true;
		this.gridColumn5.VisibleIndex = 4;
		this.gridColumn5.Width = 100;
		this.gridColumn6.Caption = "BP Code";
		this.gridColumn6.ColumnEdit = this.rep_CardCode;
		this.gridColumn6.CurrencyDateField = null;
		this.gridColumn6.CurrencyField = null;
		this.gridColumn6.CurrencyFixed = null;
		this.gridColumn6.CurrencyFixedRate = 0.0;
		this.gridColumn6.CurrencyRateField = null;
		this.gridColumn6.FieldName = "BPCardCode";
		this.gridColumn6.Name = "gridColumn6";
		this.gridColumn6.OptionsColumn.ReadOnly = true;
		this.gridColumn6.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.gridColumn6.PrimaryKey = false;
		this.gridColumn6.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridColumn6.UnitField = null;
		this.gridColumn6.Visible = true;
		this.gridColumn6.VisibleIndex = 5;
		this.gridColumn6.Width = 100;
		this.rep_CardCode.AutoHeight = false;
		editorButtonImageOptions3.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions3.Image");
		this.rep_CardCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_CardCode.Name = "rep_CardCode";
		this.rep_CardCode.ReadOnly = true;
		this.rep_CardCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_CardCode_ButtonClick);
		this.gridColumn7.Caption = "BP Name";
		this.gridColumn7.CurrencyDateField = null;
		this.gridColumn7.CurrencyField = null;
		this.gridColumn7.CurrencyFixed = null;
		this.gridColumn7.CurrencyFixedRate = 0.0;
		this.gridColumn7.CurrencyRateField = null;
		this.gridColumn7.FieldName = "CardName";
		this.gridColumn7.Name = "gridColumn7";
		this.gridColumn7.OptionsColumn.ReadOnly = true;
		this.gridColumn7.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.gridColumn7.PrimaryKey = false;
		this.gridColumn7.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridColumn7.UnitField = null;
		this.gridColumn7.Visible = true;
		this.gridColumn7.VisibleIndex = 6;
		this.gridColumn7.Width = 100;
		this.gridColumn8.Caption = "Inventory UoM";
		this.gridColumn8.CurrencyDateField = null;
		this.gridColumn8.CurrencyField = null;
		this.gridColumn8.CurrencyFixed = null;
		this.gridColumn8.CurrencyFixedRate = 0.0;
		this.gridColumn8.CurrencyRateField = null;
		this.gridColumn8.FieldName = "InvUoM";
		this.gridColumn8.Name = "gridColumn8";
		this.gridColumn8.OptionsColumn.ReadOnly = true;
		this.gridColumn8.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.gridColumn8.PrimaryKey = false;
		this.gridColumn8.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridColumn8.UnitField = null;
		this.gridColumn8.Visible = true;
		this.gridColumn8.VisibleIndex = 7;
		this.gridColumn8.Width = 100;
		this.gridColumn9.Caption = "Quantity";
		this.gridColumn9.CurrencyDateField = null;
		this.gridColumn9.CurrencyField = null;
		this.gridColumn9.CurrencyFixed = null;
		this.gridColumn9.CurrencyFixedRate = 0.0;
		this.gridColumn9.CurrencyRateField = null;
		this.gridColumn9.FieldName = "Quantity";
		this.gridColumn9.Name = "gridColumn9";
		this.gridColumn9.OptionsColumn.ReadOnly = true;
		this.gridColumn9.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.gridColumn9.PrimaryKey = false;
		this.gridColumn9.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridColumn9.UnitField = null;
		this.gridColumn9.Visible = true;
		this.gridColumn9.VisibleIndex = 8;
		this.gridColumn9.Width = 100;
		this.gridColumn10.Caption = "Price after Discount";
		this.gridColumn10.CurrencyDateField = null;
		this.gridColumn10.CurrencyField = null;
		this.gridColumn10.CurrencyFixed = null;
		this.gridColumn10.CurrencyFixedRate = 0.0;
		this.gridColumn10.CurrencyRateField = null;
		this.gridColumn10.FieldName = "Price";
		this.gridColumn10.Name = "gridColumn10";
		this.gridColumn10.OptionsColumn.ReadOnly = true;
		this.gridColumn10.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.gridColumn10.PrimaryKey = false;
		this.gridColumn10.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridColumn10.UnitField = null;
		this.gridColumn10.Visible = true;
		this.gridColumn10.VisibleIndex = 9;
		this.gridColumn10.Width = 100;
		this.gridColumn11.Caption = "Balance";
		this.gridColumn11.CurrencyDateField = null;
		this.gridColumn11.CurrencyField = null;
		this.gridColumn11.CurrencyFixed = null;
		this.gridColumn11.CurrencyFixedRate = 0.0;
		this.gridColumn11.CurrencyRateField = null;
		this.gridColumn11.FieldName = "Balance";
		this.gridColumn11.Name = "gridColumn11";
		this.gridColumn11.OptionsColumn.ReadOnly = true;
		this.gridColumn11.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.gridColumn11.PrimaryKey = false;
		this.gridColumn11.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridColumn11.UnitField = null;
		this.gridColumn11.Visible = true;
		this.gridColumn11.VisibleIndex = 10;
		this.gridColumn11.Width = 100;
		this.gridColumn12.Caption = "Remarks";
		this.gridColumn12.CurrencyDateField = null;
		this.gridColumn12.CurrencyField = null;
		this.gridColumn12.CurrencyFixed = null;
		this.gridColumn12.CurrencyFixedRate = 0.0;
		this.gridColumn12.CurrencyRateField = null;
		this.gridColumn12.FieldName = "Comment";
		this.gridColumn12.Name = "gridColumn12";
		this.gridColumn12.OptionsColumn.ReadOnly = true;
		this.gridColumn12.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.gridColumn12.PrimaryKey = false;
		this.gridColumn12.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridColumn12.UnitField = null;
		this.gridColumn12.Visible = true;
		this.gridColumn12.VisibleIndex = 11;
		this.gridColumn12.Width = 100;
		this.gridColumn13.Caption = "Ref. 1";
		this.gridColumn13.CurrencyDateField = null;
		this.gridColumn13.CurrencyField = null;
		this.gridColumn13.CurrencyFixed = null;
		this.gridColumn13.CurrencyFixedRate = 0.0;
		this.gridColumn13.CurrencyRateField = null;
		this.gridColumn13.FieldName = "Ref1";
		this.gridColumn13.Name = "gridColumn13";
		this.gridColumn13.OptionsColumn.ReadOnly = true;
		this.gridColumn13.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.gridColumn13.PrimaryKey = false;
		this.gridColumn13.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridColumn13.UnitField = null;
		this.gridColumn13.Visible = true;
		this.gridColumn13.VisibleIndex = 12;
		this.gridColumn13.Width = 100;
		this.gridColumn14.Caption = "Ref. 2";
		this.gridColumn14.CurrencyDateField = null;
		this.gridColumn14.CurrencyField = null;
		this.gridColumn14.CurrencyFixed = null;
		this.gridColumn14.CurrencyFixedRate = 0.0;
		this.gridColumn14.CurrencyRateField = null;
		this.gridColumn14.FieldName = "Ref2";
		this.gridColumn14.Name = "gridColumn14";
		this.gridColumn14.OptionsColumn.ReadOnly = true;
		this.gridColumn14.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.gridColumn14.PrimaryKey = false;
		this.gridColumn14.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridColumn14.UnitField = null;
		this.gridColumn14.Visible = true;
		this.gridColumn14.VisibleIndex = 13;
		this.gridColumn14.Width = 100;
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.EditControl = null;
		this.btn_OK.EditValue = "OK";
		this.btn_OK.FieldName = null;
		this.btn_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_OK.ImageOptions.Image");
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(1, 532);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.ReadOnly = false;
		this.btn_OK.SettingInVisible = false;
		this.btn_OK.SettingReadonly = false;
		this.btn_OK.Size = new System.Drawing.Size(75, 23);
		this.btn_OK.TabIndex = 8;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.btn_Search.EditControl = null;
		this.btn_Search.EditValue = "Search";
		this.btn_Search.FieldName = null;
		this.btn_Search.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Search.ImageOptions.Image");
		this.btn_Search.IsSearchControl = false;
		this.btn_Search.Location = new System.Drawing.Point(781, 60);
		this.btn_Search.Name = "btn_Search";
		this.btn_Search.ReadOnly = false;
		this.btn_Search.SettingInVisible = false;
		this.btn_Search.SettingReadonly = false;
		this.btn_Search.Size = new System.Drawing.Size(91, 20);
		this.btn_Search.TabIndex = 7;
		this.btn_Search.Text = "Search";
		this.btn_Search.Click += new System.EventHandler(this.btn_Search_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(884, 561);
		base.Controls.Add(this.btn_Search);
		base.Controls.Add(this.btn_OK);
		base.Controls.Add(this.grd_Data);
		base.Controls.Add(this.chkcbx_WhsCode);
		base.Controls.Add(this.labelLine6);
		base.Controls.Add(this.date_To);
		base.Controls.Add(this.date_From);
		base.Controls.Add(this.labelLine4);
		base.Controls.Add(this.labelLine5);
		base.Controls.Add(this.txt_BpTo);
		base.Controls.Add(this.labelLine2);
		base.Controls.Add(this.txt_BpFrom);
		base.Controls.Add(this.labelLine3);
		base.Controls.Add(this.txt_ItemTo);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.txt_ItemCodeFrom);
		base.Controls.Add(this.labelControl1);
		base.FormId = "oInventoryPostingList";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_InventoryPostingList";
		this.Text = "Inventory Posting List";
		base.Load += new System.EventHandler(this.frm_InventoryPostingList_Load);
		((System.ComponentModel.ISupportInitialize)this.txt_ItemCodeFrom.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ItemTo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BpTo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BpFrom.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_From.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_From.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_To.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_To.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chkcbx_WhsCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Data).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Data).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Item).EndInit();
		((System.ComponentModel.ISupportInitialize)this.look_TransType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Document).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CardCode).EndInit();
		base.ResumeLayout(false);
	}
}
