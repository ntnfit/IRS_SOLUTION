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
using Apzon.Controls.Editor.TextEditExPrice;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Administrator;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Forms.ItemsMaster;
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

namespace Apzon.Forms.Inventory.PriceLists;

public class frm_PeriodAndVolumeDiscounts : XtraFormEx
{
	private string _formController = "SpecialBp";

	private DataSet _dsConfigPrice = new DataSet();

	private IContainer components;

	private GridControlEx grd_Doc;

	private GridViewEx gv_Doc;

	private SimpleButtonEx btn_Add;

	private SimpleButtonEx btn_Cancel;

	private LookUpEdit luk_PriceList;

	private LabelLine labelLine4;

	private TextEditEx txt_Find;

	private LabelLine labelLine1;

	private GridColumnEx gc_ItemCode;

	private GridColumnEx gc_ItemName;

	private GridColumnEx gc_PrimarCurPrice;

	private GridColumnEx gc_AddCur2Price;

	private GridColumnEx gcAddCur1Price;

	private GridColumnEx gcSourcePrice;

	private GridColumnEx gc_UoM;

	private RepositoryItemButtonEdit rep_bte_ItemCode;

	private RepositoryItemLookUpEdit rep_luk_SourcePrice;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem deleteRowToolStripMenuItem;

	private RepositoryItemTextEditExPrice repositoryItemTextEditExPrice1;

	private RepositoryItemTextEditExPrice repositoryItemTextEditExPrice2;

	private RepositoryItemTextEditExPrice repositoryItemTextEditExPrice3;

	private GridColumnEx gcPriceBefDis;

	public bool _cellItemFocused { get; set; }

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Update:
			btn_Add.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btn_Add.Text = Apzon.Commons.Properties.Resources.ButtonOK;
			break;
		}
	}

	public frm_PeriodAndVolumeDiscounts()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_PeriodAndVolumeDiscounts"))
		{
			InitializeComponent();
			return;
		}
		throw new Exception(Apzon.Commons.Properties.Resources.Msg_FormAccessDenied);
	}

	private async void frm_PeriodAndVolumeDiscounts_Load(object sender, EventArgs e)
	{
		try
		{
			await LoadComboBox();
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

	private async void btn_Add_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
				return;
			}
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
			{
				return;
			}
			WinCommons._OpenProcessing();
			grd_Doc.Enabled = false;
			btn_Add.Enabled = false;
			using HttpClientHelper<DataSet> hp = new HttpClientHelper<DataSet>(Configuration.ApiServer, _formController);
			string text = await hp.PostAsJsonAsync("SaveDoc", _dsConfigPrice);
			if (string.IsNullOrWhiteSpace(text))
			{
				WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
				FormMode = Enums.FormViewModeType.View;
				await LoadItem();
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
		btn_Add.Enabled = true;
		grd_Doc.Enabled = true;
		WinCommons._CloseProcessing();
	}

	private void btn_AddItems_Click(object sender, EventArgs e)
	{
	}

	private async void luk_PriceList_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			FormMode = Enums.FormViewModeType.View;
			await LoadItem();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rep_bte_ItemCode_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				ChooseItemList();
			}
			else if (e.Button.Index == 1)
			{
				ExtenseionMethods.LoadBoMFromGridItem(gv_Doc.GetFocusedRowCellValue("ItemCode").ToString(), MdiParent);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_DoubleClick(object sender, EventArgs e)
	{
		try
		{
			if (!gv_Doc.CalcHitInfo(grd_Doc.PointToClient(Control.MousePosition)).InRow)
			{
				return;
			}
			DataRow focusedDataRow = gv_Doc.GetFocusedDataRow();
			string itemCode = Function.ToString(focusedDataRow["ItemCode"]);
			int priceList = Function.ParseInt(luk_PriceList.EditValue);
			string cardCode = "*" + priceList;
			double priceItem = Function.ParseDouble(focusedDataRow["Price"]);
			string currency = Function.ToString(focusedDataRow["Currency"]);
			DataTable dataTable = _dsConfigPrice.Tables[2];
			DataTable dataTable2 = _dsConfigPrice.Tables[3];
			DataTable table = dataTable.Clone();
			DataTable table2 = dataTable2.Clone();
			List<DataRow> list = new List<DataRow>();
			List<DataRow> list2 = new List<DataRow>();
			if (dataTable.IsNotNull())
			{
				list = (from t in dataTable.AsEnumerable()
					where Function.ToString(t["ItemCode"]).Equals(itemCode)
					select t).ToList();
				if (list.Any())
				{
					table = list.CopyToDataTable();
					if (dataTable2.IsNotNull())
					{
						list2 = (from t in dataTable2.AsEnumerable()
							where Function.ToString(t["ItemCode"]).Equals(itemCode)
							select t).ToList();
						if (list2.Any())
						{
							table2 = list2.CopyToDataTable();
						}
					}
				}
			}
			DataSet dataSet = new DataSet();
			dataSet.Tables.Add(table);
			dataSet.Tables.Add(table2);
			frm_SpecialforBP_LineSPP1 frm_SpecialforBP_LineSPP3 = new frm_SpecialforBP_LineSPP1();
			frm_SpecialforBP_LineSPP3._cardCode = cardCode;
			frm_SpecialforBP_LineSPP3._itemCode = itemCode;
			frm_SpecialforBP_LineSPP3.DsPriceConfig = dataSet;
			frm_SpecialforBP_LineSPP3.priceItem = priceItem;
			frm_SpecialforBP_LineSPP3._currency = currency;
			frm_SpecialforBP_LineSPP3.PriceList = priceList;
			frm_SpecialforBP_LineSPP3.ShowDialog(this);
			if (frm_SpecialforBP_LineSPP3.DialogResult != DialogResult.OK)
			{
				return;
			}
			focusedDataRow["IsUpdate"] = "Y";
			FormMode = Enums.FormViewModeType.Update;
			for (int num = 0; num < list.Count; num++)
			{
				_dsConfigPrice.Tables[2].Rows.Remove(list[num]);
			}
			for (int num2 = 0; num2 < list2.Count; num2++)
			{
				_dsConfigPrice.Tables[3].Rows.Remove(list2[num2]);
			}
			for (int num3 = 0; num3 < frm_SpecialforBP_LineSPP3.DsPriceConfig.Tables[0].Rows.Count; num3++)
			{
				if (Function.ParseInt(frm_SpecialforBP_LineSPP3.DsPriceConfig.Tables[0].Rows[num3]["ListNum"]) == 0)
				{
					_dsConfigPrice.Tables[2].ImportRow(frm_SpecialforBP_LineSPP3.DsPriceConfig.Tables[0].Rows[num3]);
				}
				else if (Function.ParseDecimal(frm_SpecialforBP_LineSPP3.DsPriceConfig.Tables[0].Rows[num3]["Discount"]) != 0m)
				{
					_dsConfigPrice.Tables[2].ImportRow(frm_SpecialforBP_LineSPP3.DsPriceConfig.Tables[0].Rows[num3]);
				}
			}
			for (int num4 = 0; num4 < frm_SpecialforBP_LineSPP3.DsPriceConfig.Tables[1].Rows.Count; num4++)
			{
				_dsConfigPrice.Tables[3].ImportRow(frm_SpecialforBP_LineSPP3.DsPriceConfig.Tables[1].Rows[num4]);
			}
			_dsConfigPrice.Tables[2].AcceptChanges();
			_dsConfigPrice.Tables[3].AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rep_bte_ItemCode_Enter(object sender, EventArgs e)
	{
		_cellItemFocused = true;
	}

	private void rep_bte_ItemCode_Leave(object sender, EventArgs e)
	{
		_cellItemFocused = false;
	}

	private void gv_Doc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			gv_Doc.GetDataRow(e.RowHandle)["IsUpdate"] = "Y";
			FormMode = Enums.FormViewModeType.Update;
			DataTable dataSourceTable = grd_Doc.DataSourceTable;
			if (string.IsNullOrWhiteSpace(Function.ToString(e.Value)) || e.Column != gv_Doc.Columns["ItemCode"])
			{
				return;
			}
			string itemCode = Function.ToString(e.Value);
			if (!dataSourceTable.IsNotNull() || (from t in dataSourceTable.AsEnumerable()
				where Function.ToString(t["ItemCode"]).Equals(itemCode)
				select t).Count() == 1)
			{
				int num = Function.ParseInt(luk_PriceList.EditValue);
				DataTable basePrice = HttpClient.PriceListSetup.GetBasePrice(num, itemCode);
				if (basePrice.IsNotNull())
				{
					DataRow dataRow = gv_Doc.GetDataRow(e.RowHandle);
					dataRow["SrcPrice"] = 0;
					dataRow["Discount"] = 0;
					dataRow["AutoUpdt"] = "Y";
					dataRow["EXPAND"] = "N";
					dataRow["UserSign"] = GlobalData.UserID;
					dataRow["UserSign2"] = GlobalData.UserID;
					dataRow["Valid"] = "Y";
					dataRow["LogInstanc"] = 0;
					dataRow["CardCode"] = "*" + num;
					dataRow["ListNum"] = num;
					gv_Doc.SetRowCellValue(e.RowHandle, "ItemName", basePrice.Rows[0]["ItemName"]);
					gv_Doc.SetRowCellValue(e.RowHandle, "Price", basePrice.Rows[0]["Price"]);
					gv_Doc.SetRowCellValue(e.RowHandle, "AddPrice1", basePrice.Rows[0]["AddPrice1"]);
					gv_Doc.SetRowCellValue(e.RowHandle, "AddPrice2", basePrice.Rows[0]["AddPrice2"]);
					dataRow["Currency"] = basePrice.Rows[0]["Currency"];
					gv_Doc.SetRowCellValue(e.RowHandle, "UomName", basePrice.Rows[0]["UomName"]);
					AddNewRow();
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.ItemCodeDoesNotExists, "");
				}
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.ItemCodeExists, "", itemCode);
				gv_Doc.SetRowCellValue(e.RowHandle, "ItemCode", "");
			}
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
			if (gv_Doc.DataRowCount <= 1)
			{
				return;
			}
			FormMode = Enums.FormViewModeType.Update;
			List<DataRow> listDataSourceSelectedRow = gv_Doc.GetListDataSourceSelectedRow();
			IEnumerable<string> lstItem = listDataSourceSelectedRow.Select((DataRow t) => Function.ToString(t["ItemCode"]));
			if (_dsConfigPrice.Tables[2].IsNotNull())
			{
				List<DataRow> list = (from t in _dsConfigPrice.Tables[2].AsEnumerable()
					where lstItem.Contains(Function.ToString(t["ItemCode"]))
					select t).ToList();
				for (int num = 0; num < list.Count; num++)
				{
					_dsConfigPrice.Tables[2].Rows.Remove(list[num]);
				}
			}
			if (_dsConfigPrice.Tables[3].IsNotNull())
			{
				List<DataRow> list2 = (from t in _dsConfigPrice.Tables[3].AsEnumerable()
					where lstItem.Contains(Function.ToString(t["ItemCode"]))
					select t).ToList();
				for (int num2 = 0; num2 < list2.Count; num2++)
				{
					_dsConfigPrice.Tables[3].Rows.Remove(list2[num2]);
				}
			}
			gv_Doc.DeleteSelectedRows();
			_dsConfigPrice.Tables[1].AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadComboBox()
	{
		try
		{
			using (HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "PriceList"))
			{
				DataTable dataSource = await hp.GetAsync("GetGroupNumPriceList");
				luk_PriceList.Properties.DataSource = dataSource;
			}
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add(new DataColumn("Code", typeof(int)));
			dataTable.Columns.Add(new DataColumn("Name", typeof(string)));
			DataRow dataRow = dataTable.NewRow();
			dataRow["Code"] = 0;
			dataRow["Name"] = "Unit Price - Pri. Curr.";
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			DataRow dataRow2 = dataTable.NewRow();
			dataRow2["Code"] = 1;
			dataRow2["Name"] = "Unit Price - Add. Curr. 1";
			dataTable.Rows.Add(dataRow2);
			dataTable.AcceptChanges();
			DataRow dataRow3 = dataTable.NewRow();
			dataRow3["Code"] = 2;
			dataRow3["Name"] = "Unit Price - Add. Curr. 2";
			dataTable.Rows.Add(dataRow3);
			dataTable.AcceptChanges();
			rep_luk_SourcePrice.BestFitMode = BestFitMode.BestFitResizePopup;
			rep_luk_SourcePrice.DataSource = dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void AddNewRow()
	{
		try
		{
			DataTable dataSourceTable = grd_Doc.DataSourceTable;
			if (!dataSourceTable.IsNotNull() || !dataSourceTable.AsEnumerable().Any((DataRow t) => string.IsNullOrEmpty(Function.ToString(t["ItemCode"]))))
			{
				DataRow dataRow = dataSourceTable.NewRow();
				dataRow["SrcPrice"] = 0;
				dataRow["CardCode"] = "*" + Function.ParseInt(luk_PriceList.EditValue);
				dataRow["ListNum"] = Function.ParseInt(luk_PriceList.EditValue);
				dataRow["IsUpdate"] = "Y";
				dataSourceTable.Rows.Add(dataRow);
				dataSourceTable.AcceptChanges();
				grd_Doc.DataSource = dataSourceTable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadItem()
	{
		try
		{
			int listNum = ((luk_PriceList.EditValue != null) ? Function.ParseInt(luk_PriceList.EditValue) : 0);
			using HttpClientHelper<DataSet> hp = new HttpClientHelper<DataSet>(Configuration.ApiServer, _formController);
			DataSet dataSet = await hp.GetAsync("Search", new
			{
				cardCode = "*" + listNum
			});
			if (dataSet.Tables.Count != 4)
			{
				WinCommons.Error(Enums.MessageCode.CannotGetData, "");
				return;
			}
			dataSet.Tables[0].Rows.Clear();
			DataRow dataRow = dataSet.Tables[0].NewRow();
			dataRow["CardCode"] = "*" + listNum;
			dataRow["IsSpecial"] = "N";
			dataSet.Tables[0].Rows.Add(dataRow);
			grd_Doc.DataSource = dataSet.Tables[1];
			grd_Doc.RefreshDataSource();
			_dsConfigPrice = dataSet;
			AddNewRow();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		try
		{
			if ((msg.Msg == 256 || msg.Msg == 260) && keyData == Keys.Tab && _cellItemFocused && string.IsNullOrWhiteSpace(Function.ToString(gv_Doc.GetFocusedRowCellValue("ItemCode"))))
			{
				ChooseItemList();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return base.ProcessCmdKey(ref msg, keyData);
	}

	private void ChooseItemList()
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.ListItems, true, string.Empty, "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItems == null || frm_CFL2.ChooseItems.Count <= 0)
			{
				return;
			}
			int num = gv_Doc.FocusedRowHandle;
			int num2 = Function.ParseInt(luk_PriceList.EditValue);
			DataTable dataSourceTable = grd_Doc.DataSourceTable;
			for (int i = 0; i < frm_CFL2.ChooseItems.Count; i++)
			{
				DataRow dataRow = frm_CFL2.ChooseItems[i];
				string itemCode = Function.ToString(dataRow["ItemCode"]);
				if (!dataSourceTable.IsNotNull() || !dataSourceTable.AsEnumerable().Any((DataRow t) => Function.ToString(t["ItemCode"]).Equals(itemCode)))
				{
					if (i > 0)
					{
						DataRow dataRow2 = dataSourceTable.NewRow();
						dataRow2["ListNum"] = num2;
						dataSourceTable.Rows.InsertAt(dataRow2, num);
						num = gv_Doc.FindRowHandleByDataRow(dataRow2);
					}
					gv_Doc.SetRowCellValue(num, "ItemCode", itemCode);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_Find_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			DataTable dataSourceTable = grd_Doc.DataSourceTable;
			if (dataSourceTable.IsNotNull())
			{
				string itemCode = txt_Find.EditValue.ToString().Trim().ToLower();
				DataRow row = dataSourceTable.AsEnumerable().FirstOrDefault((DataRow r) => Function.ToString(r["ItemCode"]).ToLower() == itemCode);
				int num = gv_Doc.GetRowHandle(dataSourceTable.Rows.IndexOf(row));
				if (num < 0)
				{
					num = 0;
				}
				gv_Doc.FocusedRowHandle = num;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void frm_PeriodAndVolumeDiscounts_FormClosing(object sender, FormClosingEventArgs e)
	{
		if ((FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCloseForm, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
		{
			e.Cancel = true;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Inventory.PriceLists.frm_PeriodAndVolumeDiscounts));
		this.repositoryItemTextEditExPrice1 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.repositoryItemTextEditExPrice2 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.repositoryItemTextEditExPrice3 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.gc_ItemCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_bte_ItemCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gc_ItemName = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_PrimarCurPrice = new Apzon.Controls.Grid.GridColumnEx();
		this.gcAddCur1Price = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_AddCur2Price = new Apzon.Controls.Grid.GridColumnEx();
		this.gcSourcePrice = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_luk_SourcePrice = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gc_UoM = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_Add = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.luk_PriceList = new DevExpress.XtraEditors.LookUpEdit();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.txt_Find = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.gcPriceBefDis = new Apzon.Controls.Grid.GridColumnEx();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_bte_ItemCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_luk_SourcePrice).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_PriceList.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Find.Properties).BeginInit();
		base.SuspendLayout();
		this.repositoryItemTextEditExPrice1.Name = "repositoryItemTextEditExPrice1";
		this.repositoryItemTextEditExPrice2.Name = "repositoryItemTextEditExPrice2";
		this.repositoryItemTextEditExPrice3.Name = "repositoryItemTextEditExPrice3";
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_Doc.Location = new System.Drawing.Point(12, 60);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.rep_bte_ItemCode, this.rep_luk_SourcePrice });
		this.grd_Doc.Size = new System.Drawing.Size(483, 270);
		this.grd_Doc.TabIndex = 0;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.deleteRowToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(134, 26);
		this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
		this.deleteRowToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
		this.deleteRowToolStripMenuItem.Text = "Delete Row";
		this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.deleteRowToolStripMenuItem_Click);
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = null;
		this.gv_Doc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[8] { this.gc_ItemCode, this.gc_ItemName, this.gc_PrimarCurPrice, this.gcAddCur1Price, this.gc_AddCur2Price, this.gcSourcePrice, this.gc_UoM, this.gcPriceBefDis });
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = "oPeriodAndVolumeDisCount";
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.IgnoreFilter = false;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 35;
		this.gv_Doc.IsApzTable = false;
		this.gv_Doc.IsInsertNewRow = false;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.IsMappingContextAction = false;
		this.gv_Doc.ItemId = "OITM";
		this.gv_Doc.LstColumnAllowBatch = null;
		this.gv_Doc.LstColumnNotAllowBatch = null;
		this.gv_Doc.LstColumnNotEditable = null;
		this.gv_Doc.LstColumNotSetting = null;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsView.ColumnAutoWidth = false;
		this.gv_Doc.OptionsView.ShowGroupPanel = false;
		this.gv_Doc.UseBatchMod = false;
		this.gv_Doc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Doc_CellValueChanged);
		this.gv_Doc.DoubleClick += new System.EventHandler(this.gv_Doc_DoubleClick);
		this.gc_ItemCode.Caption = "Item No.";
		this.gc_ItemCode.ColumnEdit = this.rep_bte_ItemCode;
		this.gc_ItemCode.CurrencyDateField = null;
		this.gc_ItemCode.CurrencyField = null;
		this.gc_ItemCode.CurrencyFixed = null;
		this.gc_ItemCode.CurrencyFixedRate = 0.0;
		this.gc_ItemCode.CurrencyRateField = null;
		this.gc_ItemCode.FieldName = "ItemCode";
		this.gc_ItemCode.Name = "gc_ItemCode";
		this.gc_ItemCode.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gc_ItemCode.PrimaryKey = true;
		this.gc_ItemCode.UnitField = null;
		this.gc_ItemCode.Visible = true;
		this.gc_ItemCode.VisibleIndex = 0;
		this.rep_bte_ItemCode.AutoHeight = false;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_bte_ItemCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_bte_ItemCode.Name = "rep_bte_ItemCode";
		this.rep_bte_ItemCode.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_bte_ItemCode_ButtonPressed);
		this.rep_bte_ItemCode.Enter += new System.EventHandler(this.rep_bte_ItemCode_Enter);
		this.rep_bte_ItemCode.Leave += new System.EventHandler(this.rep_bte_ItemCode_Leave);
		this.gc_ItemName.Caption = "Item Description";
		this.gc_ItemName.CurrencyDateField = null;
		this.gc_ItemName.CurrencyField = null;
		this.gc_ItemName.CurrencyFixed = null;
		this.gc_ItemName.CurrencyFixedRate = 0.0;
		this.gc_ItemName.CurrencyRateField = null;
		this.gc_ItemName.FieldName = "ItemName";
		this.gc_ItemName.Name = "gc_ItemName";
		this.gc_ItemName.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gc_ItemName.PrimaryKey = false;
		this.gc_ItemName.UnitField = null;
		this.gc_ItemName.Visible = true;
		this.gc_ItemName.VisibleIndex = 1;
		this.gc_ItemName.Width = 88;
		this.gc_PrimarCurPrice.Caption = "Unit Price - Primary Currency";
		this.gc_PrimarCurPrice.ColumnEdit = this.repositoryItemTextEditExPrice1;
		this.gc_PrimarCurPrice.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gc_PrimarCurPrice.CurrencyDateField = null;
		this.gc_PrimarCurPrice.CurrencyField = "Currency";
		this.gc_PrimarCurPrice.CurrencyFixed = null;
		this.gc_PrimarCurPrice.CurrencyFixedRate = 0.0;
		this.gc_PrimarCurPrice.CurrencyRateField = null;
		this.gc_PrimarCurPrice.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gc_PrimarCurPrice.FieldName = "Price";
		this.gc_PrimarCurPrice.Name = "gc_PrimarCurPrice";
		this.gc_PrimarCurPrice.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gc_PrimarCurPrice.OptionsColumn.ReadOnly = true;
		this.gc_PrimarCurPrice.PrimaryKey = false;
		this.gc_PrimarCurPrice.UnitField = null;
		this.gc_PrimarCurPrice.Visible = true;
		this.gc_PrimarCurPrice.VisibleIndex = 2;
		this.gc_PrimarCurPrice.Width = 148;
		this.gcAddCur1Price.Caption = "Unit Price - Additonal Currency 1";
		this.gcAddCur1Price.ColumnEdit = this.repositoryItemTextEditExPrice2;
		this.gcAddCur1Price.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gcAddCur1Price.CurrencyDateField = null;
		this.gcAddCur1Price.CurrencyField = "Currency";
		this.gcAddCur1Price.CurrencyFixed = null;
		this.gcAddCur1Price.CurrencyFixedRate = 0.0;
		this.gcAddCur1Price.CurrencyRateField = null;
		this.gcAddCur1Price.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gcAddCur1Price.FieldName = "AddPrice1";
		this.gcAddCur1Price.Name = "gcAddCur1Price";
		this.gcAddCur1Price.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcAddCur1Price.OptionsColumn.ReadOnly = true;
		this.gcAddCur1Price.PrimaryKey = false;
		this.gcAddCur1Price.UnitField = null;
		this.gcAddCur1Price.Visible = true;
		this.gcAddCur1Price.VisibleIndex = 3;
		this.gcAddCur1Price.Width = 166;
		this.gc_AddCur2Price.Caption = "Unit Price - Additonal Currency 2";
		this.gc_AddCur2Price.ColumnEdit = this.repositoryItemTextEditExPrice3;
		this.gc_AddCur2Price.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gc_AddCur2Price.CurrencyDateField = null;
		this.gc_AddCur2Price.CurrencyField = "Currency";
		this.gc_AddCur2Price.CurrencyFixed = null;
		this.gc_AddCur2Price.CurrencyFixedRate = 0.0;
		this.gc_AddCur2Price.CurrencyRateField = null;
		this.gc_AddCur2Price.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gc_AddCur2Price.FieldName = "AddPrice2";
		this.gc_AddCur2Price.Name = "gc_AddCur2Price";
		this.gc_AddCur2Price.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gc_AddCur2Price.OptionsColumn.ReadOnly = true;
		this.gc_AddCur2Price.PrimaryKey = false;
		this.gc_AddCur2Price.UnitField = null;
		this.gc_AddCur2Price.Visible = true;
		this.gc_AddCur2Price.VisibleIndex = 4;
		this.gc_AddCur2Price.Width = 166;
		this.gcSourcePrice.Caption = "Source Price";
		this.gcSourcePrice.ColumnEdit = this.rep_luk_SourcePrice;
		this.gcSourcePrice.CurrencyDateField = null;
		this.gcSourcePrice.CurrencyField = null;
		this.gcSourcePrice.CurrencyFixed = null;
		this.gcSourcePrice.CurrencyFixedRate = 0.0;
		this.gcSourcePrice.CurrencyRateField = null;
		this.gcSourcePrice.FieldName = "SrcPrice";
		this.gcSourcePrice.Name = "gcSourcePrice";
		this.gcSourcePrice.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcSourcePrice.PrimaryKey = false;
		this.gcSourcePrice.UnitField = null;
		this.gcSourcePrice.Visible = true;
		this.gcSourcePrice.VisibleIndex = 5;
		this.rep_luk_SourcePrice.AutoHeight = false;
		this.rep_luk_SourcePrice.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_luk_SourcePrice.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.rep_luk_SourcePrice.DisplayMember = "Name";
		this.rep_luk_SourcePrice.Name = "rep_luk_SourcePrice";
		this.rep_luk_SourcePrice.NullText = "";
		this.rep_luk_SourcePrice.ValueMember = "Code";
		this.gc_UoM.Caption = "UoM";
		this.gc_UoM.CurrencyDateField = null;
		this.gc_UoM.CurrencyField = null;
		this.gc_UoM.CurrencyFixed = null;
		this.gc_UoM.CurrencyFixedRate = 0.0;
		this.gc_UoM.CurrencyRateField = null;
		this.gc_UoM.FieldName = "IUoMEntry";
		this.gc_UoM.Name = "gc_UoM";
		this.gc_UoM.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gc_UoM.OptionsColumn.ReadOnly = true;
		this.gc_UoM.PrimaryKey = false;
		this.gc_UoM.UnitField = null;
		this.gc_UoM.Visible = true;
		this.gc_UoM.VisibleIndex = 6;
		this.btn_Add.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Add.EditControl = null;
		this.btn_Add.EditValue = "OK";
		this.btn_Add.FieldName = null;
		this.btn_Add.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Add.ImageOptions.Image");
		this.btn_Add.IsSearchControl = false;
		this.btn_Add.Location = new System.Drawing.Point(12, 336);
		this.btn_Add.Name = "btn_Add";
		this.btn_Add.ReadOnly = false;
		this.btn_Add.SettingInVisible = false;
		this.btn_Add.SettingReadonly = false;
		this.btn_Add.Size = new System.Drawing.Size(75, 23);
		this.btn_Add.TabIndex = 1;
		this.btn_Add.Text = "OK";
		this.btn_Add.Click += new System.EventHandler(this.btn_Add_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(90, 336);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 2;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.luk_PriceList.Location = new System.Drawing.Point(123, 9);
		this.luk_PriceList.Name = "luk_PriceList";
		this.luk_PriceList.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_PriceList.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ListName", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ListNum", "ListNum", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.luk_PriceList.Properties.DisplayMember = "ListName";
		this.luk_PriceList.Properties.NullText = "";
		this.luk_PriceList.Properties.ValueMember = "ListNum";
		this.luk_PriceList.Size = new System.Drawing.Size(204, 20);
		this.luk_PriceList.TabIndex = 21;
		this.luk_PriceList.EditValueChanged += new System.EventHandler(this.luk_PriceList_EditValueChanged);
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "Price Lists";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(12, 12);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(110, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 20;
		this.labelLine4.Utext = "Price Lists";
		this.txt_Find.EditControl = null;
		this.txt_Find.FieldName = null;
		this.txt_Find.IsSearchControl = false;
		this.txt_Find.Location = new System.Drawing.Point(123, 33);
		this.txt_Find.Name = "txt_Find";
		this.txt_Find.SettingInVisible = false;
		this.txt_Find.SettingReadonly = false;
		this.txt_Find.Size = new System.Drawing.Size(204, 20);
		this.txt_Find.TabIndex = 25;
		this.txt_Find.EditValueChanged += new System.EventHandler(this.txt_Find_EditValueChanged);
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Find";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(12, 36);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(111, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 26;
		this.labelLine1.Utext = "Find";
		this.gcPriceBefDis.Caption = "Unit Price - Primary Currency";
		this.gcPriceBefDis.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gcPriceBefDis.CurrencyDateField = null;
		this.gcPriceBefDis.CurrencyField = "Currency";
		this.gcPriceBefDis.CurrencyFixed = null;
		this.gcPriceBefDis.CurrencyFixedRate = 0.0;
		this.gcPriceBefDis.CurrencyRateField = null;
		this.gcPriceBefDis.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gcPriceBefDis.FieldName = "PriceBefDis";
		this.gcPriceBefDis.Name = "gcPriceBefDis";
		this.gcPriceBefDis.PrimaryKey = false;
		this.gcPriceBefDis.UnitField = null;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(507, 364);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.txt_Find);
		base.Controls.Add(this.luk_PriceList);
		base.Controls.Add(this.labelLine4);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_Add);
		base.Controls.Add(this.grd_Doc);
		base.FormId = "oPeriodAndVolumeDisCount";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_PeriodAndVolumeDiscounts";
		this.Text = "Period And Volume Discounts";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_PeriodAndVolumeDiscounts_FormClosing);
		base.Load += new System.EventHandler(this.frm_PeriodAndVolumeDiscounts_Load);
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_bte_ItemCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_luk_SourcePrice).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_PriceList.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Find.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
