using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Grid;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Tile;

namespace Apzon.Forms.UserControls.POS_UI;

public class ucRefund : XtraUserControl
{
	public Action ForceOk;

	public DataTable DtRefund;

	private double _vatPrcnt;

	private double _invoiceTotal;

	private double _subTotal;

	private double _invoiceDiscount;

	private double _discPrcnt;

	private double _vatSum;

	public string CardCode;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private readonly Hashtable _table = new Hashtable();

	private IContainer components;

	private ucBarcode ucBarcode1;

	private ucRetailGrid ucRetailGrid1;

	private CheckEditEx checkEditEx1;

	private ucPaymentTotal ucPaymentTotal1;

	public ucRefund()
	{
		InitializeComponent();
		Load += OnLoad;
		ucRetailGrid1.RefundMode = true;
	}

	public void SUBMIT()
	{
		try
		{
			dynamic val = ucRetailGrid1.grd_Contents.MainView as GridViewEx;
			List<DataRow> source;
			if (val == null)
			{
				val = ucRetailGrid1.grd_Contents.MainView as TileView;
				if (val == null)
				{
					return;
				}
				source = GetListTileViewSelectedRow();
			}
			else
			{
				source = val.GetListDataSourceSelectedRow();
			}
			if (source.Any())
			{
				DtRefund = source.CopyToDataTable();
			}
			if (DtRefund.IsNotNull())
			{
				for (int i = 0; i < DtRefund.Rows.Count; i++)
				{
					DtRefund.Rows[i]["Quantity"] = Function.ParseDouble(DtRefund.Rows[i]["Quantity"]) * -1.0;
					double qty = Function.ParseDouble(DtRefund.Rows[i]["Quantity"]);
					double price = Function.ParseDouble(DtRefund.Rows[i]["PriceBefDi"]);
					double disc = Function.ParseDouble(DtRefund.Rows[i]["DiscPrcnt"]);
					double num = Function.ParseDouble(DtRefund.Rows[i]["LineVat"]);
					DtRefund.Rows[i]["LineVat"] = num * -1.0;
					double num2 = CalcLineTotal(qty, price, disc);
					DtRefund.Rows[i]["LineTotal"] = num2;
				}
			}
			ForceOk?.Invoke();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public List<DataRow> GetListTileViewSelectedRow()
	{
		List<DataRow> list = new List<DataRow>();
		try
		{
			int[] checkedRows = ucRetailGrid1.tileView1.GetCheckedRows();
			foreach (int num in checkedRows)
			{
				if (num >= 0)
				{
					DataRow dataRow = ucRetailGrid1.tileView1.GetDataRow(num);
					if (dataRow != null)
					{
						list.Add(dataRow);
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return list;
	}

	public double CalcLineTotal(double qty, double price, double disc)
	{
		try
		{
			double num = qty * price;
			return num - num * disc / 100.0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return 0.0;
	}

	private void OnLoad(object sender, EventArgs eventArgs)
	{
		try
		{
			ucRetailGrid1.tileView1.CustomUnboundColumnData += (object o, CustomColumnDataEventArgs e) =>
			{
				if (e.Column.FieldName == "Total")
				{
					int listSourceRowIndex = e.ListSourceRowIndex;
					double num = Function.ParseDouble(ucRetailGrid1.tileView1.GetListSourceRowCellValue(listSourceRowIndex, "LineTotal"));
					double num2 = Function.ParseDouble(ucRetailGrid1.tileView1.GetListSourceRowCellValue(listSourceRowIndex, "LineVat"));
					e.Value = num + num2;
				}
				else
				{
					if (e.IsSetData)
					{
						if (_table.ContainsKey(e.Row))
						{
							_table[e.Row] = e.Value;
						}
						else
						{
							_table.Add(e.Row, e.Value);
						}
					}
					if (e.IsGetData && _table.ContainsKey(e.Row))
					{
						e.Value = (bool)_table[e.Row];
					}
				}
			};
			ucRetailGrid1.tileView1.ItemClick += (object o, TileViewItemClickEventArgs e) =>
			{
				e.Item.Checked = !e.Item.Checked;
				ReCalcData();
			};
			ucRetailGrid1.tileView1.ItemCheckedChanged += (object o, TileViewItemClickEventArgs e) =>
			{
				ReCalcData();
			};
			ButtonEditEx barCode = ucBarcode1.btnBarCode;
			barCode.KeyDown += BtnBarCodeOnKeyDown;
			TileView view = ucRetailGrid1.grd_Contents.MainView as TileView;
			barCode.ButtonClick += async (object o, ButtonPressedEventArgs args) =>
			{
				if (!string.IsNullOrEmpty(barCode.Text))
				{
					string barcode = Function.ToString(barCode.Text);
					if (view != null)
					{
						if (ucRetailGrid1.grd_Contents.DataSource is DataTable dt)
						{
							DataTable dtDoc = new DataTable();
							DataSet dataSet = await _httpClient.RetailChooseFromList.ListItems(GlobalData.UserName, GlobalData.StoreCode, "", Function.ToString(CardCode), barcode);
							if (dataSet.IsNotNull() && dataSet.Tables.Count > 1)
							{
								dtDoc = dataSet.Tables[1];
							}
							foreach (DataRow row2 in dtDoc.Rows)
							{
								string itemCode = Function.ToString(row2["ItemCode"]);
								EnumerableRowCollection<DataRow> source = from t in dt.AsEnumerable()
									where Function.ToString(t["ItemCode"]).ToUpper() == itemCode.ToUpper()
									select t;
								if (source.Any())
								{
									for (int num = 0; num < source.ToArray().Length; num++)
									{
										DataRow row = source.ToArray()[num];
										if (!Function.ParseBool(view.GetRowCellValue(dt.Rows.IndexOf(row), view.ColumnSet.CheckedColumn)))
										{
											view.SetRowCellValue(dt.Rows.IndexOf(row), view.ColumnSet.CheckedColumn, true);
											ReCalcData();
											break;
										}
									}
								}
								else
								{
									WinCommons.Error(Enums.MessageCode.None, PoinOfSale.msg_ItemNotInRefund);
								}
							}
						}
						barCode.Text = string.Empty;
						barCode.Enabled = true;
						barCode.Focus();
					}
				}
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public void SetDataSource(DataSet ds)
	{
		try
		{
			DataTable dataTable = ds.Tables[0];
			DataTable dataTable2 = ds.Tables[1];
			if (dataTable2 != null)
			{
				ucRetailGrid1.grd_Contents.DataSource = dataTable2;
				_discPrcnt = Function.ParseDouble(dataTable.Rows[0]["DiscPrcnt"]);
			}
			_subTotal = 0.0;
			_invoiceDiscount = 0.0;
			_vatSum = 0.0;
			_invoiceTotal = 0.0;
			ucPaymentTotal1?.SetText(new Tuple<double, double, double, double>(_subTotal, _invoiceTotal, _vatSum, _invoiceDiscount));
			checkEditEx1.Checked = false;
			ucBarcode1.btnBarCode.EditValue = null;
			ucBarcode1.btnBarCode.Focus();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ReCalcData()
	{
		try
		{
			if (!(ucRetailGrid1.grd_Contents.MainView is TileView tileView))
			{
				_subTotal = 0.0;
				_invoiceDiscount = 0.0;
				_vatSum = 0.0;
				_invoiceTotal = 0.0;
				ucPaymentTotal1?.SetText(new Tuple<double, double, double, double>(_subTotal, _invoiceTotal, _vatSum, _invoiceDiscount));
				return;
			}
			int[] checkedRows = tileView.GetCheckedRows();
			if (ucRetailGrid1.grd_Contents.DataSource is DataTable dataTable)
			{
				DataTable dataTable2 = dataTable.Clone();
				int[] array = checkedRows;
				foreach (int rowHandle in array)
				{
					int dataSourceRowIndex = tileView.GetDataSourceRowIndex(rowHandle);
					dataTable2.ImportRow(dataTable.Rows[dataSourceRowIndex]);
				}
				double num = dataTable2.AsEnumerable().Sum((DataRow t) => Function.ParseDouble(t["LineTotal"]));
				_vatSum = dataTable2.AsEnumerable().Sum((DataRow t) => Function.ParseDouble(t["LineVat"]));
				if (_discPrcnt > 0.0)
				{
					_invoiceDiscount = num * _discPrcnt / 100.0;
				}
				else
				{
					_invoiceDiscount = _discPrcnt;
				}
				_subTotal = num;
			}
			_invoiceTotal = _subTotal + _vatSum - _invoiceDiscount;
			ucPaymentTotal1?.SetText(new Tuple<double, double, double, double>(_subTotal, _invoiceTotal, _vatSum, _invoiceDiscount));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void BtnBarCodeOnKeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			TileView view = ucRetailGrid1.grd_Contents.MainView as TileView;
			ButtonEditEx barCode = ucBarcode1.btnBarCode;
			if (e.KeyCode != Keys.Return || string.IsNullOrEmpty(barCode.Text))
			{
				return;
			}
			string barcode = Function.ToString(barCode.Text);
			if (view == null)
			{
				return;
			}
			if (ucRetailGrid1.grd_Contents.DataSource is DataTable dt)
			{
				DataTable dtDoc = new DataTable();
				DataSet dataSet = await _httpClient.RetailChooseFromList.ListItems(GlobalData.UserName, GlobalData.StoreCode, "", Function.ToString(CardCode), barcode);
				if (dataSet.IsNotNull() && dataSet.Tables.Count > 1)
				{
					dtDoc = dataSet.Tables[1];
				}
				foreach (DataRow row2 in dtDoc.Rows)
				{
					string itemCode = Function.ToString(row2["ItemCode"]);
					EnumerableRowCollection<DataRow> source = from t in dt.AsEnumerable()
						where Function.ToString(t["ItemCode"]).ToUpper() == itemCode.ToUpper()
						select t;
					if (source.Any())
					{
						for (int num = 0; num < source.ToArray().Length; num++)
						{
							DataRow row = source.ToArray()[num];
							if (!Function.ParseBool(view.GetRowCellValue(dt.Rows.IndexOf(row), view.ColumnSet.CheckedColumn)))
							{
								view.SetRowCellValue(dt.Rows.IndexOf(row), view.ColumnSet.CheckedColumn, true);
								ReCalcData();
								break;
							}
						}
					}
					else
					{
						WinCommons.Error(Enums.MessageCode.None, PoinOfSale.msg_ItemNotInRefund);
					}
				}
			}
			barCode.Text = string.Empty;
			barCode.Enabled = true;
			barCode.Focus();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void checkEditEx1_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			if (ucRetailGrid1.grd_Contents.MainView is TileView tileView)
			{
				for (int i = 0; i < tileView.DataRowCount; i++)
				{
					tileView.SetRowCellValue(i, tileView.ColumnSet.CheckedColumn, checkEditEx1.Checked);
				}
			}
			ReCalcData();
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
		this.ucBarcode1 = new Apzon.Forms.UserControls.POS_UI.ucBarcode();
		this.ucRetailGrid1 = new Apzon.Forms.UserControls.POS_UI.ucRetailGrid();
		this.checkEditEx1 = new Apzon.Controls.Editor.CheckEditEx();
		this.ucPaymentTotal1 = new Apzon.Forms.UserControls.POS_UI.ucPaymentTotal();
		((System.ComponentModel.ISupportInitialize)this.checkEditEx1.Properties).BeginInit();
		base.SuspendLayout();
		this.ucBarcode1.Location = new System.Drawing.Point(0, 467);
		this.ucBarcode1.Name = "ucBarcode1";
		this.ucBarcode1.RetailGrid = null;
		this.ucBarcode1.Size = new System.Drawing.Size(570, 43);
		this.ucBarcode1.TabIndex = 22;
		this.ucRetailGrid1.Location = new System.Drawing.Point(0, 0);
		this.ucRetailGrid1.Name = "ucRetailGrid1";
		this.ucRetailGrid1.Size = new System.Drawing.Size(775, 387);
		this.ucRetailGrid1.TabIndex = 23;
		this.checkEditEx1.FieldName = null;
		this.checkEditEx1.IsSearchControl = false;
		this.checkEditEx1.Location = new System.Drawing.Point(591, 473);
		this.checkEditEx1.Name = "checkEditEx1";
		this.checkEditEx1.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 15.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.checkEditEx1.Properties.Appearance.Options.UseFont = true;
		this.checkEditEx1.Properties.Caption = "Chọn tất cả";
		this.checkEditEx1.Size = new System.Drawing.Size(181, 29);
		this.checkEditEx1.TabIndex = 24;
		this.checkEditEx1.CheckedChanged += new System.EventHandler(this.checkEditEx1_CheckedChanged);
		this.ucPaymentTotal1.Location = new System.Drawing.Point(0, 383);
		this.ucPaymentTotal1.Name = "ucPaymentTotal1";
		this.ucPaymentTotal1.Size = new System.Drawing.Size(775, 84);
		this.ucPaymentTotal1.TabIndex = 25;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.ucPaymentTotal1);
		base.Controls.Add(this.checkEditEx1);
		base.Controls.Add(this.ucRetailGrid1);
		base.Controls.Add(this.ucBarcode1);
		base.Name = "ucRefund";
		base.Size = new System.Drawing.Size(775, 512);
		((System.ComponentModel.ISupportInitialize)this.checkEditEx1.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
