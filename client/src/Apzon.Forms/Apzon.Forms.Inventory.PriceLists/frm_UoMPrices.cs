using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
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
using Apzon.Entities.Models.MasterData;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Inventory.PriceLists;

public class frm_UoMPrices : XtraFormEx
{
	public string _ItemCode;

	public int _PriceList;

	public int _UgpEntry;

	private string _formController = "ItemsMaster";

	private string currency = "";

	private string currency1 = "";

	private string currency2 = "";

	private bool _isLoading;

	private DataTable _dtPriceList = new DataTable();

	private bool _isChangedData;

	private IContainer components;

	private TextEditEx txt_Find;

	private LabelLine lb_Find;

	private SimpleButtonEx btn_Cancel;

	private SimpleButtonEx bt_OK;

	private GridControlEx grd_Doc;

	private RepositoryItemLookUpEdit rep_UomEntry;

	private RepositoryItemCheckEdit rep_AutoUpdate;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem deleteRowToolStripMenuItem;

	private BandedGridViewEx gv_Doc;

	private BandedGridColumnEx gridColumn1;

	private BandedGridColumnEx gridColumn2;

	private BandedGridColumnEx gcUomCode;

	private BandedGridColumnEx gcFactor;

	private BandedGridColumnEx gcPrice;

	private BandedGridColumnEx gcCurrency;

	private BandedGridColumnEx gcAutoUpdate;

	private BandedGridColumnEx gcPrice1;

	private BandedGridColumnEx gcCurrency1;

	private BandedGridColumnEx gcPrice2;

	private BandedGridColumnEx gcCurrency2;

	private BandedGridColumnEx gridColumn12;

	private BandedGridColumnEx gridColumn13;

	private BandedGridColumnEx gcFactor1;

	private BandedGridColumnEx gcFactor2;

	private BandedGridColumnEx gridColumn16;

	private BandedGridColumnEx gcUomName;

	private BandedGridColumnEx gcBasePrice;

	private BandedGridColumnEx gcBasePrice1;

	private BandedGridColumnEx gcBasePrice2;

	private RepositoryItemTextEditExPrice rPrice;

	private GridBand gridBandUom;

	private GridBand gridBandPrimary;

	private GridBand gridBandAdditional1;

	private GridBand gridBandAdditional2;

	private GridBand gridBandAuto;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx1;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx2;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx3;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
		{
			WinFuntions.DisableAllControl(Controls);
			btn_Cancel.Enabled = true;
			return;
		}
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Update:
			bt_OK.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			bt_OK.Text = Resources.ButtonOK;
			break;
		}
	}

	public frm_UoMPrices()
	{
		InitializeComponent();
	}

	private void frm_UoMPrices_Load(object sender, EventArgs e)
	{
		DataTable dataTable = LoadPriceList();
		if (dataTable.IsNotNull())
		{
			Text = Function.ToString(dataTable.Rows[0]["ListName"]) + Text;
		}
		LoadComBo();
		LoadData();
	}

	private void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (grd_Doc.DataSource is DataTable dataTable)
			{
				int[] selectedRows = gv_Doc.GetSelectedRows();
				if (selectedRows != null && selectedRows.Length != 0 && gv_Doc.GetFocusedDataSourceRowIndex() > 0)
				{
					dataTable.Rows.RemoveAt(gv_Doc.GetFocusedDataSourceRowIndex());
					dataTable.AcceptChanges();
					grd_Doc.DataSource = dataTable;
					grd_Doc.RefreshDataSource();
					ChangeButtonOKText();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void bt_OK_Click(object sender, EventArgs e)
	{
		bt_OK.Enabled = false;
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
				return;
			}
			if (grd_Doc.DataSource is DataTable dataTable && dataTable.Rows.Count > 0)
			{
				List<Items_UoMPricesImpl> list = new List<Items_UoMPricesImpl>();
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					DataRow dataRow = dataTable.Rows[i];
					if (Function.ToString(dataRow["IsBase"]).Equals("N") && Function.ParseInt(dataRow["UomEntry"]) > 0)
					{
						Items_UoMPricesImpl items_UoMPricesImpl = new Items_UoMPricesImpl();
						items_UoMPricesImpl.ItemCode = _ItemCode;
						short? num = Function.ParseShort(_PriceList);
						if (num.HasValue)
						{
							items_UoMPricesImpl.PriceList = num.Value;
						}
						items_UoMPricesImpl.UomEntry = Function.ParseInt(dataRow["UomEntry"]);
						items_UoMPricesImpl.Factor = Function.ParseDouble(dataRow["Factor"]);
						items_UoMPricesImpl.Price = Function.ParseDouble(dataRow["Price"]);
						items_UoMPricesImpl.Currency = Function.ToString(dataRow["Currency"]);
						items_UoMPricesImpl.Factor1 = Function.ParseDouble(dataRow["Factor1"]);
						items_UoMPricesImpl.AddPrice1 = Function.ParseDouble(dataRow["AddPrice1"]);
						items_UoMPricesImpl.Currency1 = Function.ToString(dataRow["Currency1"]);
						items_UoMPricesImpl.Factor2 = Function.ParseDouble(dataRow["Factor2"]);
						items_UoMPricesImpl.AddPrice2 = Function.ParseDouble(dataRow["AddPrice2"]);
						items_UoMPricesImpl.Currency2 = Function.ToString(dataRow["Currency2"]);
						items_UoMPricesImpl.AutoUpdate = ((Function.ToString(dataRow["AutoUpdate"]) == "Y") ? "" : "N");
						list.Add(items_UoMPricesImpl);
					}
				}
				if (list.Count == 0)
				{
					list.Add(new Items_UoMPricesImpl
					{
						PriceList = Function.ParseShort(_PriceList).GetValueOrDefault(),
						ItemCode = _ItemCode
					});
				}
				using HttpClientHelper<string> httpClientHelper = new HttpClientHelper<string>(Configuration.ApiServer, _formController);
				string text = httpClientHelper.Post("UpdateUoMPrices", list);
				if (!string.IsNullOrEmpty(text))
				{
					WinCommons.Error(Enums.MessageCode.None, text);
				}
				else
				{
					WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
					FormMode = Enums.FormViewModeType.View;
				}
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.InfoNoRowSelected, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		bt_OK.Enabled = true;
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void gv_Doc_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == "UomName" || e.Column.FieldName == "BasePrice" || e.Column.FieldName == "BasePrice1" || e.Column.FieldName == "BasePrice2" || e.RowHandle == 0)
			{
				e.Appearance.BackColor = Color.Silver;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Doc_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (gv_Doc.FocusedRowHandle == 0)
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

	private void gv_Doc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (_isChangedData)
			{
				return;
			}
			ChangeButtonOKText();
			if (e.Column.FieldName.Equals("Factor"))
			{
				_isChangedData = true;
				double num = Function.ParseDouble(gv_Doc.GetFocusedRowCellValue("BasePrice"));
				double num2 = Function.ParseDouble(gv_Doc.GetFocusedRowCellValue("Factor"));
				gv_Doc.SetRowCellValue(e.RowHandle, "Currency", gv_Doc.GetRowCellValue(e.RowHandle, "BaseCurrency"));
				gv_Doc.SetRowCellValue(e.RowHandle, "Price", num * (1.0 - num2 / 100.0));
				_isChangedData = false;
			}
			else if (e.Column.FieldName.Equals("Price"))
			{
				if (!Function.ToString(gv_Doc.GetRowCellValue(e.RowHandle, "Currency")).Equals(Function.ToString(gv_Doc.GetRowCellValue(e.RowHandle, "BaseCurrency"))))
				{
					WinCommons.Error(Enums.MessageCode.CurrencyDoesNotMatchPriceList, "");
					_isChangedData = true;
					gv_Doc.SetRowCellValue(e.RowHandle, "Currency", gv_Doc.GetRowCellValue(e.RowHandle, "BaseCurrency"));
					gv_Doc.SetRowCellValue(e.RowHandle, "Price", 0);
					gv_Doc.SetRowCellValue(e.RowHandle, "Factor", 0);
					_isChangedData = false;
					return;
				}
				_isChangedData = true;
				double num3 = Function.ParseDouble(gv_Doc.GetFocusedRowCellValue("BasePrice"));
				if (Math.Abs(num3) > 1E-07)
				{
					gv_Doc.SetRowCellValue(e.RowHandle, "Factor", 100.0 * (1.0 - Function.ParseDouble(e.Value) / num3));
				}
				_isChangedData = false;
			}
			else if (e.Column.FieldName.Equals("Factor1"))
			{
				_isChangedData = true;
				double num4 = Function.ParseDouble(gv_Doc.GetFocusedRowCellValue("BasePrice1"));
				double num5 = Function.ParseDouble(gv_Doc.GetFocusedRowCellValue("Factor1"));
				gv_Doc.SetRowCellValue(e.RowHandle, "Currency1", gv_Doc.GetRowCellValue(e.RowHandle, "BaseCurrency1"));
				gv_Doc.SetRowCellValue(e.RowHandle, "Price1", num4 * (1.0 - num5 / 100.0));
				_isChangedData = false;
			}
			else if (e.Column.FieldName.Equals("Price1"))
			{
				if (!Function.ToString(gv_Doc.GetRowCellValue(e.RowHandle, "Currency1")).Equals(Function.ToString(gv_Doc.GetRowCellValue(e.RowHandle, "BaseCurrency1"))))
				{
					WinCommons.Error(Enums.MessageCode.CurrencyDoesNotMatchPriceList, "");
					_isChangedData = true;
					gv_Doc.SetRowCellValue(e.RowHandle, "Currency1", gv_Doc.GetRowCellValue(e.RowHandle, "BaseCurrency1"));
					gv_Doc.SetRowCellValue(e.RowHandle, "Price1", 0);
					gv_Doc.SetRowCellValue(e.RowHandle, "Factor1", 0);
					_isChangedData = false;
					return;
				}
				_isChangedData = true;
				double num6 = Function.ParseDouble(gv_Doc.GetFocusedRowCellValue("BasePrice1"));
				if (Math.Abs(num6) > 1E-07)
				{
					gv_Doc.SetRowCellValue(e.RowHandle, "Factor1", 100.0 * (1.0 - Function.ParseDouble(e.Value) / num6));
				}
				_isChangedData = false;
			}
			else if (e.Column.FieldName.Equals("Factor2"))
			{
				_isChangedData = true;
				double num7 = Function.ParseDouble(gv_Doc.GetFocusedRowCellValue("BasePrice2"));
				double num8 = Function.ParseDouble(gv_Doc.GetFocusedRowCellValue("Factor2"));
				gv_Doc.SetRowCellValue(e.RowHandle, "Currency2", gv_Doc.GetRowCellValue(e.RowHandle, "BaseCurrency2"));
				gv_Doc.SetRowCellValue(e.RowHandle, "Price2", num7 * (1.0 - num8 / 100.0));
				_isChangedData = false;
			}
			else if (e.Column.FieldName.Equals("Price2"))
			{
				if (!Function.ToString(gv_Doc.GetRowCellValue(e.RowHandle, "Currency2")).Equals(Function.ToString(gv_Doc.GetRowCellValue(e.RowHandle, "BaseCurrency2"))))
				{
					WinCommons.Error(Enums.MessageCode.CurrencyDoesNotMatchPriceList, "");
					_isChangedData = true;
					gv_Doc.SetRowCellValue(e.RowHandle, "Currency2", gv_Doc.GetRowCellValue(e.RowHandle, "BaseCurrency2"));
					gv_Doc.SetRowCellValue(e.RowHandle, "Price2", 0);
					gv_Doc.SetRowCellValue(e.RowHandle, "Factor2", 0);
					_isChangedData = false;
					return;
				}
				_isChangedData = true;
				double num9 = Function.ParseDouble(gv_Doc.GetFocusedRowCellValue("BasePrice2"));
				if (Math.Abs(num9) > 1E-07)
				{
					gv_Doc.SetRowCellValue(e.RowHandle, "Factor2", 100.0 * (1.0 - Function.ParseDouble(e.Value) / num9));
				}
				_isChangedData = false;
			}
			AddNewRow();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		_isChangedData = false;
	}

	private void rep_UomEntry_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (!(grd_Doc.DataSource is DataTable source))
			{
				return;
			}
			string[] value = (from t in source.AsEnumerable()
				select Function.ToString(t["UomEntry"])).ToArray();
			string dataString = string.Join(",", value);
			frm_CFL frm = new frm_CFL(Enums.TypeCFL.ListUoMForUoMPrices, false, string.Empty, "", Enums.ItemObjectType.NoItemType, null, "")
			{
				DataString1 = _ItemCode,
				DataString2 = dataString,
				DataInt1 = _PriceList,
				UgpEntry = _UgpEntry
			};
			frm.ShowDialog(this);
			if (frm.ChooseItem != null && source.AsEnumerable().All((DataRow t) => Function.ParseInt(t["UomEntry"]) != Function.ParseInt(frm.ChooseItem["UomEntry"])))
			{
				gv_Doc.SetFocusedRowCellValue("UomEntry", Function.ParseInt(frm.ChooseItem["UomEntry"]));
				gv_Doc.SetFocusedRowCellValue("UomName", Function.ToString(frm.ChooseItem["UomName"]));
				DataRow dataRow = source.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["IsBase"]).Equals("Y"));
				if (dataRow != null)
				{
					double num = Function.ParseDouble(dataRow["BasePrice"]) * Function.ParseDouble(frm.ChooseItem["ItemPerUnit"]) / Function.ParseDouble(dataRow["ItemPerUnit"]);
					gv_Doc.SetFocusedRowCellValue("BasePrice", num);
					gv_Doc.SetFocusedRowCellValue("Price", num);
					double num2 = Function.ParseDouble(dataRow["BasePrice1"]) * Function.ParseDouble(frm.ChooseItem["ItemPerUnit"]) / Function.ParseDouble(dataRow["ItemPerUnit"]);
					gv_Doc.SetFocusedRowCellValue("BasePrice1", num2);
					gv_Doc.SetFocusedRowCellValue("AddPrice1", num2);
					double num3 = Function.ParseDouble(dataRow["BasePrice2"]) * Function.ParseDouble(frm.ChooseItem["ItemPerUnit"]) / Function.ParseDouble(dataRow["ItemPerUnit"]);
					gv_Doc.SetFocusedRowCellValue("BasePrice2", num3);
					gv_Doc.SetFocusedRowCellValue("AddPrice2", num3);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rep_UomEntry_QueryPopUp(object sender, CancelEventArgs e)
	{
		e.Cancel = true;
	}

	private void LoadData()
	{
		_isLoading = true;
		try
		{
			grd_Doc.DataSource = HttpClient.Items.GetUoMPricesByItmCodePrList(_PriceList, _ItemCode, _UgpEntry);
			AddNewRow();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		_isLoading = false;
	}

	private void AddNewRow()
	{
		try
		{
			if (grd_Doc.DataSource is DataTable dataTable && !dataTable.AsEnumerable().Any((DataRow t) => Function.ParseInt(t["UomEntry"]) <= 0))
			{
				string value = "";
				string value2 = "";
				string value3 = "";
				DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["IsBase"]).Equals("Y"));
				if (dataRow != null)
				{
					value = Function.ToString(dataRow["BaseCurrency"]);
					value2 = Function.ToString(dataRow["BaseCurrency1"]);
					value3 = Function.ToString(dataRow["BaseCurrency2"]);
				}
				DataRow dataRow2 = dataTable.NewRow();
				dataRow2["IsBase"] = "N";
				dataRow2["AutoUpdate"] = "Y";
				dataRow2["ItemCode"] = _ItemCode;
				dataRow2["PriceList"] = _PriceList;
				dataRow2["Currency"] = value;
				dataRow2["Currency1"] = value2;
				dataRow2["Currency2"] = value3;
				dataRow2["BaseCurrency"] = value;
				dataRow2["BaseCurrency1"] = value2;
				dataRow2["BaseCurrency2"] = value3;
				dataTable.Rows.Add(dataRow2);
				dataTable.AcceptChanges();
				grd_Doc.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadComBo()
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, _formController);
			DataTable dataSource = httpClientHelper.Get("UoMPricesLoadComBo", new
			{
				itemCode = _ItemCode
			});
			rep_UomEntry.Columns.Clear();
			rep_UomEntry.Columns.Add(new LookUpColumnInfo
			{
				Caption = "Code",
				FieldName = "UomCode"
			});
			rep_UomEntry.DisplayMember = "UomCode";
			rep_UomEntry.ValueMember = "UomEntry";
			rep_UomEntry.NullText = "";
			rep_UomEntry.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private DataTable LoadPriceList()
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, _formController);
			return httpClientHelper.Get("UoMPricesLoadPriceList", new
			{
				listNum = _PriceList
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return new DataTable();
		}
	}

	private void ChangeButtonOKText()
	{
		try
		{
			if (!_isLoading)
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Inventory.PriceLists.frm_UoMPrices));
		this.repositoryItemCalcEditEx1 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx2 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx3 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.txt_Find = new Apzon.Controls.Editor.TextEditEx();
		this.lb_Find = new Apzon.Controls.Controls.LabelLine();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.bt_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Doc = new Apzon.Controls.Grid.BandedGridViewEx();
		this.gridBandUom = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
		this.gridColumn1 = new Apzon.Controls.Grid.BandedGridColumnEx();
		this.gridColumn2 = new Apzon.Controls.Grid.BandedGridColumnEx();
		this.gcUomCode = new Apzon.Controls.Grid.BandedGridColumnEx();
		this.rep_UomEntry = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcCurrency = new Apzon.Controls.Grid.BandedGridColumnEx();
		this.gcCurrency1 = new Apzon.Controls.Grid.BandedGridColumnEx();
		this.gcCurrency2 = new Apzon.Controls.Grid.BandedGridColumnEx();
		this.gridColumn12 = new Apzon.Controls.Grid.BandedGridColumnEx();
		this.gridColumn13 = new Apzon.Controls.Grid.BandedGridColumnEx();
		this.gridColumn16 = new Apzon.Controls.Grid.BandedGridColumnEx();
		this.gcUomName = new Apzon.Controls.Grid.BandedGridColumnEx();
		this.gridBandPrimary = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
		this.gcBasePrice = new Apzon.Controls.Grid.BandedGridColumnEx();
		this.rPrice = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.gcFactor = new Apzon.Controls.Grid.BandedGridColumnEx();
		this.gcPrice = new Apzon.Controls.Grid.BandedGridColumnEx();
		this.gridBandAdditional1 = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
		this.gcBasePrice1 = new Apzon.Controls.Grid.BandedGridColumnEx();
		this.gcFactor1 = new Apzon.Controls.Grid.BandedGridColumnEx();
		this.gcPrice1 = new Apzon.Controls.Grid.BandedGridColumnEx();
		this.gridBandAdditional2 = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
		this.gcBasePrice2 = new Apzon.Controls.Grid.BandedGridColumnEx();
		this.gcFactor2 = new Apzon.Controls.Grid.BandedGridColumnEx();
		this.gcPrice2 = new Apzon.Controls.Grid.BandedGridColumnEx();
		this.gridBandAuto = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
		this.gcAutoUpdate = new Apzon.Controls.Grid.BandedGridColumnEx();
		this.rep_AutoUpdate = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Find.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_UomEntry).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rPrice).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_AutoUpdate).BeginInit();
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
		this.txt_Find.EditControl = null;
		this.txt_Find.FieldName = null;
		this.txt_Find.IsSearchControl = false;
		this.txt_Find.Location = new System.Drawing.Point(114, 7);
		this.txt_Find.Name = "txt_Find";
		this.txt_Find.SettingInVisible = false;
		this.txt_Find.SettingReadonly = false;
		this.txt_Find.Size = new System.Drawing.Size(130, 20);
		this.txt_Find.TabIndex = 10;
		this.lb_Find.EditControl = null;
		this.lb_Find.EditValue = "Item";
		this.lb_Find.FieldName = null;
		this.lb_Find.HideWhenEmpty = false;
		this.lb_Find.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Find.Location = new System.Drawing.Point(12, 10);
		this.lb_Find.Name = "lb_Find";
		this.lb_Find.ReadOnly = false;
		this.lb_Find.SettingInVisible = false;
		this.lb_Find.SettingReadonly = false;
		this.lb_Find.ShowUnderLine = true;
		this.lb_Find.Size = new System.Drawing.Size(106, 20);
		this.lb_Find.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Find.TabIndex = 9;
		this.lb_Find.Utext = "Item";
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(82, 373);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 12;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.bt_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.bt_OK.EditControl = null;
		this.bt_OK.EditValue = "OK";
		this.bt_OK.FieldName = null;
		this.bt_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("bt_OK.ImageOptions.Image");
		this.bt_OK.IsSearchControl = false;
		this.bt_OK.Location = new System.Drawing.Point(4, 373);
		this.bt_OK.Name = "bt_OK";
		this.bt_OK.ReadOnly = false;
		this.bt_OK.SettingInVisible = false;
		this.bt_OK.SettingReadonly = false;
		this.bt_OK.Size = new System.Drawing.Size(75, 23);
		this.bt_OK.TabIndex = 11;
		this.bt_OK.Text = "OK";
		this.bt_OK.Click += new System.EventHandler(this.bt_OK_Click);
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_Doc.Location = new System.Drawing.Point(-1, 35);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.rep_UomEntry, this.rep_AutoUpdate, this.rPrice });
		this.grd_Doc.Size = new System.Drawing.Size(1028, 332);
		this.grd_Doc.TabIndex = 13;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.deleteRowToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(134, 26);
		this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
		this.deleteRowToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
		this.deleteRowToolStripMenuItem.Text = "Delete Row";
		this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.deleteRowToolStripMenuItem_Click);
		this.gv_Doc.Bands.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[5] { this.gridBandUom, this.gridBandPrimary, this.gridBandAdditional1, this.gridBandAdditional2, this.gridBandAuto });
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.Columns.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn[20]
		{
			this.gridColumn1, this.gridColumn2, this.gcUomCode, this.gcUomName, this.gcBasePrice, this.gcFactor, this.gcPrice, this.gcCurrency, this.gcBasePrice1, this.gcFactor1,
			this.gcPrice1, this.gcCurrency1, this.gcBasePrice2, this.gcFactor2, this.gcPrice2, this.gcCurrency2, this.gridColumn12, this.gridColumn13, this.gridColumn16, this.gcAutoUpdate
		});
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FormId = "oUomPrice";
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 60;
		this.gv_Doc.IsApzTable = false;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.ItemId = "OPRICE";
		this.gv_Doc.LstColumnNotEditable = null;
		this.gv_Doc.LstColumNotSetting = null;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsView.ColumnAutoWidth = false;
		this.gv_Doc.OptionsView.ShowGroupPanel = false;
		this.gv_Doc.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Doc_RowCellStyle);
		this.gv_Doc.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_Doc_ShowingEditor);
		this.gv_Doc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Doc_CellValueChanged);
		this.gridBandUom.Columns.Add(this.gridColumn1);
		this.gridBandUom.Columns.Add(this.gridColumn2);
		this.gridBandUom.Columns.Add(this.gcUomCode);
		this.gridBandUom.Columns.Add(this.gcCurrency);
		this.gridBandUom.Columns.Add(this.gcCurrency1);
		this.gridBandUom.Columns.Add(this.gcCurrency2);
		this.gridBandUom.Columns.Add(this.gridColumn12);
		this.gridBandUom.Columns.Add(this.gridColumn13);
		this.gridBandUom.Columns.Add(this.gridColumn16);
		this.gridBandUom.Columns.Add(this.gcUomName);
		this.gridBandUom.Name = "gridBandUom";
		this.gridBandUom.VisibleIndex = 0;
		this.gridBandUom.Width = 150;
		this.gridColumn1.Caption = "ItemCode";
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "ItemCode";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gridColumn2.Caption = "PriceList";
		this.gridColumn2.CurrencyField = null;
		this.gridColumn2.CurrencyFixed = null;
		this.gridColumn2.CurrencyFixedRate = 0.0;
		this.gridColumn2.CurrencyRateField = null;
		this.gridColumn2.FieldName = "PriceList";
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcUomCode.Caption = "UomEntry";
		this.gcUomCode.ColumnEdit = this.rep_UomEntry;
		this.gcUomCode.CurrencyField = null;
		this.gcUomCode.CurrencyFixed = null;
		this.gcUomCode.CurrencyFixedRate = 0.0;
		this.gcUomCode.CurrencyRateField = null;
		this.gcUomCode.FieldName = "UomEntry";
		this.gcUomCode.Name = "gcUomCode";
		this.gcUomCode.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcUomCode.Visible = true;
		this.rep_UomEntry.AutoHeight = false;
		this.rep_UomEntry.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rep_UomEntry.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("UomCode", "Name")
		});
		this.rep_UomEntry.DisplayMember = "UomCode";
		this.rep_UomEntry.Name = "rep_UomEntry";
		this.rep_UomEntry.NullText = "";
		this.rep_UomEntry.ValueMember = "UomEntry";
		this.rep_UomEntry.QueryPopUp += new System.ComponentModel.CancelEventHandler(this.rep_UomEntry_QueryPopUp);
		this.rep_UomEntry.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_UomEntry_ButtonPressed);
		this.gcCurrency.Caption = "Currency";
		this.gcCurrency.CurrencyField = null;
		this.gcCurrency.CurrencyFixed = null;
		this.gcCurrency.CurrencyFixedRate = 0.0;
		this.gcCurrency.CurrencyRateField = null;
		this.gcCurrency.FieldName = "Currency";
		this.gcCurrency.Name = "gcCurrency";
		this.gcCurrency.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcCurrency1.Caption = "Currency1";
		this.gcCurrency1.CurrencyField = null;
		this.gcCurrency1.CurrencyFixed = null;
		this.gcCurrency1.CurrencyFixedRate = 0.0;
		this.gcCurrency1.CurrencyRateField = null;
		this.gcCurrency1.FieldName = "Currency1";
		this.gcCurrency1.Name = "gcCurrency1";
		this.gcCurrency1.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcCurrency2.Caption = "Currency2";
		this.gcCurrency2.CurrencyField = null;
		this.gcCurrency2.CurrencyFixed = null;
		this.gcCurrency2.CurrencyFixedRate = 0.0;
		this.gcCurrency2.CurrencyRateField = null;
		this.gcCurrency2.FieldName = "Currency2";
		this.gcCurrency2.Name = "gcCurrency2";
		this.gcCurrency2.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gridColumn12.Caption = "LogInstanc";
		this.gridColumn12.CurrencyField = null;
		this.gridColumn12.CurrencyFixed = null;
		this.gridColumn12.CurrencyFixedRate = 0.0;
		this.gridColumn12.CurrencyRateField = null;
		this.gridColumn12.FieldName = "LogInstanc";
		this.gridColumn12.Name = "gridColumn12";
		this.gridColumn12.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gridColumn13.Caption = "ObjType";
		this.gridColumn13.CurrencyField = null;
		this.gridColumn13.CurrencyFixed = null;
		this.gridColumn13.CurrencyFixedRate = 0.0;
		this.gridColumn13.CurrencyRateField = null;
		this.gridColumn13.FieldName = "ObjType";
		this.gridColumn13.Name = "gridColumn13";
		this.gridColumn13.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gridColumn16.Caption = "UpdateDate";
		this.gridColumn16.CurrencyField = null;
		this.gridColumn16.CurrencyFixed = null;
		this.gridColumn16.CurrencyFixedRate = 0.0;
		this.gridColumn16.CurrencyRateField = null;
		this.gridColumn16.FieldName = "UpdateDate";
		this.gridColumn16.Name = "gridColumn16";
		this.gridColumn16.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcUomName.AppearanceCell.BackColor = System.Drawing.Color.Silver;
		this.gcUomName.AppearanceCell.Options.UseBackColor = true;
		this.gcUomName.Caption = "Uom Name";
		this.gcUomName.CurrencyField = null;
		this.gcUomName.CurrencyFixed = null;
		this.gcUomName.CurrencyFixedRate = 0.0;
		this.gcUomName.CurrencyRateField = null;
		this.gcUomName.FieldName = "UomName";
		this.gcUomName.Name = "gcUomName";
		this.gcUomName.OptionsColumn.AllowEdit = false;
		this.gcUomName.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcUomName.OptionsColumn.ReadOnly = true;
		this.gcUomName.Visible = true;
		this.gridBandPrimary.Caption = "Primary currency";
		this.gridBandPrimary.Columns.Add(this.gcBasePrice);
		this.gridBandPrimary.Columns.Add(this.gcFactor);
		this.gridBandPrimary.Columns.Add(this.gcPrice);
		this.gridBandPrimary.Name = "gridBandPrimary";
		this.gridBandPrimary.VisibleIndex = 1;
		this.gridBandPrimary.Width = 225;
		this.gcBasePrice.AppearanceCell.BackColor = System.Drawing.Color.Silver;
		this.gcBasePrice.AppearanceCell.Options.UseBackColor = true;
		this.gcBasePrice.Caption = "Base Price";
		this.gcBasePrice.ColumnEdit = this.rPrice;
		this.gcBasePrice.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gcBasePrice.CurrencyField = "BaseCurrency";
		this.gcBasePrice.CurrencyFixed = null;
		this.gcBasePrice.CurrencyFixedRate = 0.0;
		this.gcBasePrice.CurrencyRateField = null;
		this.gcBasePrice.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gcBasePrice.FieldName = "BasePrice";
		this.gcBasePrice.Name = "gcBasePrice";
		this.gcBasePrice.OptionsColumn.AllowEdit = false;
		this.gcBasePrice.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcBasePrice.OptionsColumn.ReadOnly = true;
		this.gcBasePrice.Visible = true;
		this.rPrice.AutoHeight = false;
		this.rPrice.Name = "rPrice";
		this.gcFactor.Caption = "Reduce By %";
		this.gcFactor.ColumnEdit = this.repositoryItemCalcEditEx1;
		this.gcFactor.CurrencyField = null;
		this.gcFactor.CurrencyFixed = null;
		this.gcFactor.CurrencyFixedRate = 0.0;
		this.gcFactor.CurrencyRateField = null;
		this.gcFactor.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Percent;
		this.gcFactor.FieldName = "Factor";
		this.gcFactor.Name = "gcFactor";
		this.gcFactor.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcFactor.Visible = true;
		this.gcPrice.Caption = "Price";
		this.gcPrice.ColumnEdit = this.rPrice;
		this.gcPrice.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gcPrice.CurrencyField = "Currency";
		this.gcPrice.CurrencyFixed = null;
		this.gcPrice.CurrencyFixedRate = 0.0;
		this.gcPrice.CurrencyRateField = null;
		this.gcPrice.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gcPrice.FieldName = "Price";
		this.gcPrice.Name = "gcPrice";
		this.gcPrice.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcPrice.Visible = true;
		this.gridBandAdditional1.Caption = "Additional Currency 1";
		this.gridBandAdditional1.Columns.Add(this.gcBasePrice1);
		this.gridBandAdditional1.Columns.Add(this.gcFactor1);
		this.gridBandAdditional1.Columns.Add(this.gcPrice1);
		this.gridBandAdditional1.Name = "gridBandAdditional1";
		this.gridBandAdditional1.VisibleIndex = 2;
		this.gridBandAdditional1.Width = 254;
		this.gcBasePrice1.AppearanceCell.BackColor = System.Drawing.Color.Silver;
		this.gcBasePrice1.AppearanceCell.Options.UseBackColor = true;
		this.gcBasePrice1.Caption = "Base Price";
		this.gcBasePrice1.ColumnEdit = this.rPrice;
		this.gcBasePrice1.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gcBasePrice1.CurrencyField = "BaseCurrency1";
		this.gcBasePrice1.CurrencyFixed = null;
		this.gcBasePrice1.CurrencyFixedRate = 0.0;
		this.gcBasePrice1.CurrencyRateField = null;
		this.gcBasePrice1.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gcBasePrice1.FieldName = "BasePrice1";
		this.gcBasePrice1.Name = "gcBasePrice1";
		this.gcBasePrice1.OptionsColumn.AllowEdit = false;
		this.gcBasePrice1.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcBasePrice1.OptionsColumn.ReadOnly = true;
		this.gcBasePrice1.Visible = true;
		this.gcFactor1.Caption = "Reduce By %";
		this.gcFactor1.ColumnEdit = this.repositoryItemCalcEditEx2;
		this.gcFactor1.CurrencyField = null;
		this.gcFactor1.CurrencyFixed = null;
		this.gcFactor1.CurrencyFixedRate = 0.0;
		this.gcFactor1.CurrencyRateField = null;
		this.gcFactor1.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Percent;
		this.gcFactor1.FieldName = "Factor1";
		this.gcFactor1.Name = "gcFactor1";
		this.gcFactor1.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcFactor1.Visible = true;
		this.gcFactor1.Width = 104;
		this.gcPrice1.Caption = "Price";
		this.gcPrice1.ColumnEdit = this.rPrice;
		this.gcPrice1.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gcPrice1.CurrencyField = "Currency1";
		this.gcPrice1.CurrencyFixed = null;
		this.gcPrice1.CurrencyFixedRate = 0.0;
		this.gcPrice1.CurrencyRateField = null;
		this.gcPrice1.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gcPrice1.FieldName = "AddPrice1";
		this.gcPrice1.Name = "gcPrice1";
		this.gcPrice1.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcPrice1.Visible = true;
		this.gridBandAdditional2.Caption = "Additional Currency 2";
		this.gridBandAdditional2.Columns.Add(this.gcBasePrice2);
		this.gridBandAdditional2.Columns.Add(this.gcFactor2);
		this.gridBandAdditional2.Columns.Add(this.gcPrice2);
		this.gridBandAdditional2.Name = "gridBandAdditional2";
		this.gridBandAdditional2.VisibleIndex = 3;
		this.gridBandAdditional2.Width = 259;
		this.gcBasePrice2.AppearanceCell.BackColor = System.Drawing.Color.Silver;
		this.gcBasePrice2.AppearanceCell.Options.UseBackColor = true;
		this.gcBasePrice2.Caption = "Base Price";
		this.gcBasePrice2.ColumnEdit = this.rPrice;
		this.gcBasePrice2.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gcBasePrice2.CurrencyField = "BaseCurrency2";
		this.gcBasePrice2.CurrencyFixed = null;
		this.gcBasePrice2.CurrencyFixedRate = 0.0;
		this.gcBasePrice2.CurrencyRateField = null;
		this.gcBasePrice2.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gcBasePrice2.FieldName = "BasePrice2";
		this.gcBasePrice2.Name = "gcBasePrice2";
		this.gcBasePrice2.OptionsColumn.AllowEdit = false;
		this.gcBasePrice2.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcBasePrice2.OptionsColumn.ReadOnly = true;
		this.gcBasePrice2.Visible = true;
		this.gcFactor2.Caption = "Reduce By %";
		this.gcFactor2.ColumnEdit = this.repositoryItemCalcEditEx3;
		this.gcFactor2.CurrencyField = null;
		this.gcFactor2.CurrencyFixed = null;
		this.gcFactor2.CurrencyFixedRate = 0.0;
		this.gcFactor2.CurrencyRateField = null;
		this.gcFactor2.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Percent;
		this.gcFactor2.FieldName = "Factor2";
		this.gcFactor2.Name = "gcFactor2";
		this.gcFactor2.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcFactor2.Visible = true;
		this.gcFactor2.Width = 109;
		this.gcPrice2.Caption = "Price";
		this.gcPrice2.ColumnEdit = this.rPrice;
		this.gcPrice2.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gcPrice2.CurrencyField = "Currency2";
		this.gcPrice2.CurrencyFixed = null;
		this.gcPrice2.CurrencyFixedRate = 0.0;
		this.gcPrice2.CurrencyRateField = null;
		this.gcPrice2.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gcPrice2.FieldName = "AddPrice2";
		this.gcPrice2.Name = "gcPrice2";
		this.gcPrice2.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcPrice2.Visible = true;
		this.gridBandAuto.Columns.Add(this.gcAutoUpdate);
		this.gridBandAuto.Name = "gridBandAuto";
		this.gridBandAuto.VisibleIndex = 4;
		this.gridBandAuto.Width = 75;
		this.gcAutoUpdate.Caption = "AutoUpdate";
		this.gcAutoUpdate.ColumnEdit = this.rep_AutoUpdate;
		this.gcAutoUpdate.CurrencyField = null;
		this.gcAutoUpdate.CurrencyFixed = null;
		this.gcAutoUpdate.CurrencyFixedRate = 0.0;
		this.gcAutoUpdate.CurrencyRateField = null;
		this.gcAutoUpdate.FieldName = "AutoUpdate";
		this.gcAutoUpdate.Name = "gcAutoUpdate";
		this.gcAutoUpdate.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcAutoUpdate.Visible = true;
		this.rep_AutoUpdate.AutoHeight = false;
		this.rep_AutoUpdate.Name = "rep_AutoUpdate";
		this.rep_AutoUpdate.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.rep_AutoUpdate.ValueChecked = "Y";
		this.rep_AutoUpdate.ValueGrayed = "N";
		this.rep_AutoUpdate.ValueUnchecked = "N";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1030, 400);
		base.Controls.Add(this.grd_Doc);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.bt_OK);
		base.Controls.Add(this.txt_Find);
		base.Controls.Add(this.lb_Find);
		base.FormId = "oUomPrice";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_UoMPrices";
		this.Text = "- UoM Prices";
		base.Load += new System.EventHandler(this.frm_UoMPrices_Load);
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Find.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_UomEntry).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rPrice).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_AutoUpdate).EndInit();
		base.ResumeLayout(false);
	}
}
