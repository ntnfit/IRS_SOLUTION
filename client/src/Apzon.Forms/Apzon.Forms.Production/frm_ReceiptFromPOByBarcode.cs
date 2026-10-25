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
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;
using Apzon.Entities.Models.UserFields;
using Apzon.Forms.Commons;
using Apzon.Forms.Helper;
using Apzon.Forms.Properties;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Menu;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using SAPbobsCOM;

namespace Apzon.Forms.Production;

public class frm_ReceiptFromPOByBarcode : XtraFormEx
{
	private const string FormController = "ReceiptProduction";

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private List<UserFieldsImpl> lstUdf = new List<UserFieldsImpl>();

	public DataTable UdoTable = new DataTable();

	private IContainer components;

	private SimpleButtonEx btn_Update;

	private TextEditEx txt_BarCode;

	private LabelLine lbl_BarCode;

	private SimpleButtonEx btn_Cancel;

	private GridControlEx grd_Line;

	private PanelControl panelControl1;

	private PanelControl panelControl2;

	private PanelControl panelControl3;

	private GridViewEx gv_Line;

	private SimpleButtonEx btn_AddPacking;

	private SimpleButtonEx btn_Packing;

	public frm_ReceiptFromPOByBarcode()
	{
		InitializeComponent();
	}

	private async void frm_ReceiptFromPOByBarcode_Load(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			lstUdf = await _httpClient.Udf.GetListField("OWOR");
			UdoTable = await _httpClient.Udo.GetUdoByCode(new Common.Table_Code
			{
				Name = "OUDO",
				Code = "OPKB"
			});
			txt_BarCode.Focus();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void txt_BarCode_KeyDown(object sender, KeyEventArgs e)
	{
		_ = 1;
		try
		{
			if (e.KeyCode != Keys.Return || string.IsNullOrEmpty(txt_BarCode.Text))
			{
				return;
			}
			txt_BarCode.Enabled = false;
			btn_AddPacking.Enabled = false;
			btn_Cancel.Enabled = false;
			grd_Line.Enabled = false;
			WinCommons._OpenProcessing();
			DataSetResult dataSetResult = await _httpClient.ProductionOrders.GetDataByBarcode(txt_BarCode.Text, GlobalData.UserName, GlobalData.DefaultBranch);
			if (!string.IsNullOrEmpty(dataSetResult.Message))
			{
				WinCommons.Error(Enums.MessageCode.None, dataSetResult.Message);
				btn_AddPacking.Enabled = true;
				btn_Cancel.Enabled = true;
				txt_BarCode.Text = string.Empty;
				txt_BarCode.Enabled = true;
				grd_Line.Enabled = true;
				txt_BarCode.Focus();
				return;
			}
			DataSet ds = dataSetResult.Data;
			if (ds.IsHasStructure())
			{
				if (gv_Line.Columns.Count <= 0)
				{
					await GridViewHelper.GenerateGrid(grd_Line, gv_Line, ds, lstUdf);
					if (!ds.Tables[1].IsNotNull())
					{
						WinCommons.Warning(Enums.MessageCode.None, "Data is Empty!");
					}
				}
				else
				{
					DataTable dataSourceTable = grd_Line.DataSourceTable;
					if (dataSourceTable.IsNotNull() && ds.Tables[1].IsNotNull())
					{
						foreach (DataRow row in ds.Tables[1].Rows)
						{
							string text = Function.ToString(row["ItemCode"]);
							string text2 = Function.ToString(row["DistNumber"]);
							string text3 = Function.ToString(row["TempKey"]);
							int num = Function.ParseInt(row["POEntry"]);
							if (dataSourceTable.Select($"ItemCode <> '{text}'").Length != 0)
							{
								WinCommons.Error(Enums.MessageCode.None, "Không được nhập 2 mã hàng khác nhau!");
								break;
							}
							if (grd_Line.DatasourceTable().Select($"ItemCode='{text}' AND DistNumber='{text2}' AND POEntry={num} AND TempKey='{text3}'").Length != 0)
							{
								WinCommons.Error(Enums.MessageCode.None, "Tem đã được bắn!");
								break;
							}
							dataSourceTable.Select($"ItemCode='{text}' AND DistNumber='{text2}' AND POEntry={num}");
							DataRow dataRow2 = dataSourceTable.NewRow();
							dataRow2.ItemArray = row.ItemArray;
							dataSourceTable.Rows.Add(dataRow2);
						}
						gv_Line.FocusedRowHandle = dataSourceTable.Rows.Count - 1;
					}
					else if (ds.Tables[1].IsNotNull())
					{
						grd_Line.DataSource = ds.Tables[1];
						grd_Line.RefreshDataSource();
					}
					else
					{
						WinCommons.Warning(Enums.MessageCode.None, "Data is Empty!");
					}
				}
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, "Data is not found!");
			}
			ds.Tables.Clear();
			ds.Dispose();
			WinCommons._CloseProcessing();
			txt_BarCode.Text = string.Empty;
			txt_BarCode.Enabled = true;
			btn_AddPacking.Enabled = true;
			btn_Cancel.Enabled = true;
			grd_Line.Enabled = true;
			txt_BarCode.Focus();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			WinCommons._CloseProcessing();
			txt_BarCode.Text = string.Empty;
			txt_BarCode.Enabled = true;
			btn_AddPacking.Enabled = true;
			btn_Cancel.Enabled = true;
			grd_Line.Enabled = true;
			txt_BarCode.Focus();
		}
	}

	private async void btn_Update_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (XtraMessageBox.Show(Apzon.Commons.Properties.Resources.xtraMsg_SaveChange, Apzon.Commons.Properties.Resources.Msg, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
			{
				return;
			}
			WinCommons._OpenProcessing();
			DataTable dataSourceTable = grd_Line.DataSourceTable;
			new DataSet().Tables.Clear();
			if (!dataSourceTable.IsNotNull())
			{
				WinCommons.Error(Enums.MessageCode.None, "Data is Empty!");
				return;
			}
			DocumentsImpl documentsImpl = new DocumentsImpl
			{
				BPLId = GlobalData.DefaultBranch,
				Comments = "IRS: " + string.Join(",", (from x in dataSourceTable.AsEnumerable()
					select Function.ToString(x.Field<string>("TempKey"))).ToArray()),
				DocDate = DateTime.Today,
				Lines = new List<Document_LinesImpl>()
			};
			var enumerable = from t in dataSourceTable.AsEnumerable()
				group t by new
				{
					ItemCode = Function.ToString(t["POEntry"])
				} into g
				select new
				{
					Key = g.Key,
					Items = g
				};
			int num = 0;
			foreach (var item in enumerable)
			{
				Document_LinesImpl document_LinesImpl = new Document_LinesImpl
				{
					BatchNumbers = new List<BatchNumbersImpl>(),
					SerialNumbers = new List<SerialNumbersImpl>(),
					BaseType = 202,
					Quantity = 0.0,
					LineNum = num
				};
				List<DataColumn> list = (from DataColumn t in dataSourceTable.Columns
					where t.ColumnName.StartsWith("U_")
					select (t)).ToList();
				foreach (DataRow item2 in item.Items)
				{
					if (string.IsNullOrEmpty(document_LinesImpl.ItemCode))
					{
						document_LinesImpl.ItemCode = Function.ToString(item2["ItemCode"]);
					}
					if (item2.IsExistsColumn("WhsCode") && string.IsNullOrEmpty(document_LinesImpl.WhsCode))
					{
						document_LinesImpl.WhsCode = Function.ToString(item2["WhsCode"]);
					}
					if (item2.IsExistsColumn("POEntry") && Function.ParseInt(item2["POEntry"]) > 0)
					{
						document_LinesImpl.BaseEntry = Function.ParseInt(item2["POEntry"]);
						document_LinesImpl.BaseRef = Function.ParseInt(item2["POEntry"]);
						document_LinesImpl.BaseType = 202;
					}
					document_LinesImpl.BaseLine = -1;
					string text = "10000044";
					if (item2.IsExistsColumn("ObjType"))
					{
						text = Function.ToString(item2["ObjType"]);
					}
					if (text.Equals("10000044"))
					{
						BatchNumbersImpl batchNumbersImpl = new BatchNumbersImpl
						{
							Quantity = Function.ParseDouble(item2["Quantity"]),
							BatchNumber = Function.ToString(item2["DistNumber"]),
							AddmisionDate = DateTime.Today,
							BaseLineNumber = num
						};
						if (list.Count > 0)
						{
							batchNumbersImpl.UserFields = new List<UserFieldsImpl>();
							for (int num2 = 0; num2 < list.Count; num2++)
							{
								UserFieldsImpl userFieldsImpl = new UserFieldsImpl
								{
									Name = list[num2].ColumnName
								};
								if (list[num2].DataType == typeof(DateTime))
								{
									userFieldsImpl.Type = BoFieldTypes.db_Date;
								}
								else if (list[num2].DataType == typeof(double) || list[num2].DataType == typeof(decimal))
								{
									userFieldsImpl.Type = BoFieldTypes.db_Numeric;
									userFieldsImpl.SubType = BoFldSubTypes.st_Quantity;
								}
								else if (list[num2].DataType == typeof(int) || list[num2].DataType == typeof(short) || list[num2].DataType == typeof(long))
								{
									userFieldsImpl.Type = BoFieldTypes.db_Numeric;
								}
								else
								{
									userFieldsImpl.Type = BoFieldTypes.db_Alpha;
								}
								userFieldsImpl.Value = item2[list[num2]];
								batchNumbersImpl.UserFields.Add(userFieldsImpl);
							}
						}
						document_LinesImpl.Quantity += batchNumbersImpl.Quantity;
						document_LinesImpl.BatchNumbers.Add(batchNumbersImpl);
					}
					else
					{
						if (!text.Equals("10000045"))
						{
							continue;
						}
						SerialNumbersImpl serialNumbersImpl = new SerialNumbersImpl();
						serialNumbersImpl.BatchID = Function.ToString(item2["DistNumber"]);
						serialNumbersImpl.InternalSerialNumber = Function.ToString(item2["DistNumber"]);
						serialNumbersImpl.SystemSerialNumber = Function.ParseInt(item2["SysNumber"]);
						serialNumbersImpl.BaseLineNumber = document_LinesImpl.LineNum;
						serialNumbersImpl.Quantity = Function.ParseDouble(item2["Quantity"]);
						if (list.Count > 0)
						{
							serialNumbersImpl.UserFields = new List<UserFieldsImpl>();
							for (int num3 = 0; num3 < list.Count; num3++)
							{
								UserFieldsImpl userFieldsImpl2 = new UserFieldsImpl
								{
									Name = list[num3].ColumnName
								};
								if (list[num3].DataType == typeof(DateTime))
								{
									userFieldsImpl2.Type = BoFieldTypes.db_Date;
								}
								else if (list[num3].DataType == typeof(double) || list[num3].DataType == typeof(decimal))
								{
									userFieldsImpl2.Type = BoFieldTypes.db_Numeric;
									userFieldsImpl2.SubType = BoFldSubTypes.st_Quantity;
								}
								else if (list[num3].DataType == typeof(int) || list[num3].DataType == typeof(short) || list[num3].DataType == typeof(long))
								{
									userFieldsImpl2.Type = BoFieldTypes.db_Numeric;
								}
								else
								{
									userFieldsImpl2.Type = BoFieldTypes.db_Alpha;
								}
								userFieldsImpl2.Value = item2[list[num3]];
								serialNumbersImpl.UserFields.Add(userFieldsImpl2);
							}
						}
						document_LinesImpl.Quantity += serialNumbersImpl.Quantity;
						document_LinesImpl.SerialNumbers.Add(serialNumbersImpl);
					}
				}
				documentsImpl.Lines.Add(document_LinesImpl);
				num++;
			}
			HttpResult httpResult = await _httpClient.GoodsReceipt.AddNew(documentsImpl);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				XtraMessageBox.Show(httpResult.Message, "Error", MessageBoxButtons.OK);
			}
			else
			{
				WinCommons.Success(Enums.MessageCode.Success, Apzon.Commons.Properties.Resources.Msg_SaveSuccess);
				grd_Line.DataSource = null;
				grd_Line.RefreshDataSource();
				await WinFuntions.PrintLayout("", Function.ParseInt(httpResult.Content), "PDN2");
				txt_BarCode.Text = "";
				txt_BarCode.Focus();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private async void btn_AddPacking_Click(object sender, EventArgs e)
	{
		try
		{
			if (XtraMessageBox.Show(Apzon.Commons.Properties.Resources.xtraMsg_SaveChange, Apzon.Commons.Properties.Resources.Msg, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
			{
				return;
			}
			WinCommons._OpenProcessing();
			DataTable dataSourceTable = grd_Line.DataSourceTable;
			if (!dataSourceTable.IsNotNull())
			{
				WinCommons.Error(Enums.MessageCode.None, "Data is Empty!");
				return;
			}
			if (UdoTable == null || UdoTable.Rows.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.None, "UDO Table is invalid.");
				return;
			}
			DataSet dataSet = new DataSet();
			string tableName = Function.ToString(UdoTable.Rows[0]["TableName"]);
			dataSet.Tables.Add(UdoTable.Copy());
			DataTable dataTable = new DataTable(tableName);
			dataTable.Columns.Add("DocEntry", typeof(int));
			dataTable.Columns.Add("CreateDate", typeof(DateTime));
			dataTable.Columns.Add("U_BoxCode", typeof(string));
			DataRow dataRow = dataTable.NewRow();
			dataRow["DocEntry"] = 0;
			dataRow["CreateDate"] = DateTime.Today;
			dataRow["U_BoxCode"] = "";
			dataTable.Rows.Add(dataRow);
			dataSet.Tables.Add(dataTable);
			DataTable dataTable2 = new DataTable();
			dataTable2.TableName = "PKB1";
			dataTable2.Columns.Add("DocEntry", typeof(int));
			dataTable2.Columns.Add("LineId", typeof(int));
			dataTable2.Columns.Add("U_ItemCode", typeof(string));
			dataTable2.Columns.Add("U_ItemName", typeof(string));
			dataTable2.Columns.Add("U_LotNo", typeof(string));
			dataTable2.Columns.Add("U_Quantity", typeof(double));
			dataTable2.Columns.Add("U_BagCode", typeof(string));
			dataTable2.Columns.Add("U_TemKey", typeof(string));
			foreach (DataRow row in dataSourceTable.Rows)
			{
				DataRow dataRow3 = dataTable2.NewRow();
				dataRow3["DocEntry"] = 0;
				dataRow3["LineId"] = 0;
				dataRow3["U_ItemCode"] = Function.ToString(row["ItemCode"]);
				dataRow3["U_ItemName"] = Function.ToString(row["ItemDescription"]);
				dataRow3["U_LotNo"] = Function.ToString(row["DistNumber"]);
				dataRow3["U_Quantity"] = Function.ParseDouble(row["Quantity"]);
				dataRow3["U_BagCode"] = Function.ToString(row["BagCode"]);
				dataRow3["U_TemKey"] = Function.ToString(row["TempKey"]);
				dataTable2.Rows.Add(dataRow3);
			}
			dataSet.Tables.Add(dataTable2);
			DocumentsImpl documentsImpl = new DocumentsImpl
			{
				BPLId = GlobalData.DefaultBranch,
				Comments = "IRS: " + string.Join(",", (from x in dataSourceTable.AsEnumerable()
					select Function.ToString(x.Field<string>("TempKey"))).ToArray()),
				DocDate = DateTime.Today,
				Lines = new List<Document_LinesImpl>()
			};
			var enumerable = from t in dataSourceTable.AsEnumerable()
				group t by new
				{
					ItemCode = Function.ToString(t["POEntry"])
				} into g
				select new
				{
					Key = g.Key,
					Items = g
				};
			int num = 0;
			foreach (var item in enumerable)
			{
				Document_LinesImpl document_LinesImpl = new Document_LinesImpl
				{
					BatchNumbers = new List<BatchNumbersImpl>(),
					BinAllocations = new List<DocumentLinesBinAllocationsImpl>(),
					BaseType = 202,
					LineNum = num
				};
				double num2 = 0.0;
				List<DataColumn> list = (from DataColumn t in dataSourceTable.Columns
					where t.ColumnName.StartsWith("U_")
					select (t)).ToList();
				foreach (DataRow item2 in item.Items)
				{
					if (string.IsNullOrEmpty(document_LinesImpl.ItemCode))
					{
						document_LinesImpl.ItemCode = Function.ToString(item2["ItemCode"]);
					}
					if (item2.IsExistsColumn("WhsCode") && string.IsNullOrEmpty(document_LinesImpl.WhsCode))
					{
						document_LinesImpl.WhsCode = Function.ToString(item2["WhsCode"]);
					}
					if (item2.IsExistsColumn("POEntry") && Function.ParseInt(item2["POEntry"]) > 0)
					{
						document_LinesImpl.BaseEntry = Function.ParseInt(item2["POEntry"]);
						document_LinesImpl.BaseRef = Function.ParseInt(item2["POEntry"]);
						document_LinesImpl.BaseType = 202;
					}
					document_LinesImpl.BaseLine = -1;
					BatchNumbersImpl batchNumbersImpl = new BatchNumbersImpl
					{
						Quantity = Function.ParseDouble(item2["Quantity"]),
						BatchNumber = Function.ToString(item2["DistNumber"]),
						AddmisionDate = DateTime.Today,
						BaseLineNumber = num
					};
					if (list.Count > 0)
					{
						batchNumbersImpl.UserFields = new List<UserFieldsImpl>();
						for (int num3 = 0; num3 < list.Count; num3++)
						{
							UserFieldsImpl userFieldsImpl = new UserFieldsImpl
							{
								Name = list[num3].ColumnName
							};
							if (list[num3].DataType == typeof(DateTime))
							{
								userFieldsImpl.Type = BoFieldTypes.db_Date;
							}
							else if (list[num3].DataType == typeof(double) || list[num3].DataType == typeof(decimal))
							{
								userFieldsImpl.Type = BoFieldTypes.db_Numeric;
								userFieldsImpl.SubType = BoFldSubTypes.st_Quantity;
							}
							else if (list[num3].DataType == typeof(int) || list[num3].DataType == typeof(short) || list[num3].DataType == typeof(long))
							{
								userFieldsImpl.Type = BoFieldTypes.db_Numeric;
							}
							else
							{
								userFieldsImpl.Type = BoFieldTypes.db_Alpha;
							}
							userFieldsImpl.Value = item2[list[num3]];
							batchNumbersImpl.UserFields.Add(userFieldsImpl);
						}
					}
					num2 += batchNumbersImpl.Quantity;
					document_LinesImpl.BatchNumbers.Add(batchNumbersImpl);
				}
				document_LinesImpl.Quantity = num2;
				documentsImpl.Lines.Add(document_LinesImpl);
				num++;
			}
			HttpResult httpResult = await _httpClient.GoodsReceipt.SaveDocWithUdo(documentsImpl, dataSet);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				XtraMessageBox.Show("Receipt from Production error: " + httpResult.Message, "Error", MessageBoxButtons.OK);
			}
			else
			{
				WinCommons.Success(Enums.MessageCode.Success, Apzon.Commons.Properties.Resources.Msg_SaveSuccess);
				grd_Line.DataSource = null;
				grd_Line.RefreshDataSource();
				txt_BarCode.Text = "";
				txt_BarCode.Focus();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		if (XtraMessageBox.Show(Apzon.Commons.Properties.Resources.xtraMsg_Cancel, Apzon.Commons.Properties.Resources.Msg, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.No)
		{
			Close();
		}
	}

	private void gv_Line_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
	{
		try
		{
			GridViewMenu gridViewMenu = new GridViewMenu((GridView)sender);
			DXMenuItem dXMenuItem = new DXMenuItem
			{
				Caption = "Remove Line"
			};
			dXMenuItem.Click += lineMenu_Click;
			gridViewMenu.Items.Add(dXMenuItem);
			if (e.MenuType == GridMenuType.User || e.MenuType == GridMenuType.Row)
			{
				e.Allow = true;
				gridViewMenu.Show(e.Point);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void lineMenu_Click(object sender, EventArgs e)
	{
		try
		{
			if ((sender as DXMenuItem).Caption.Equals("Remove Line"))
			{
				GridView gridView = (GridView)grd_Line.MainView;
				if (gridView.FocusedRowHandle > -1)
				{
					gridView.DeleteRow(gridView.FocusedRowHandle);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_Refresh_Click(object sender, EventArgs e)
	{
		try
		{
			if ((grd_Line.DataSource as DataTable).IsNotNull() && FormMode == Enums.FormViewModeType.Add && XtraMessageBox.Show("Unsaved data will be lost. Do you want to continue without saving?", Apzon.Commons.Properties.Resources.Msg, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
			{
				txt_BarCode.Focus();
				return;
			}
			grd_Line.DataSource = new DataTable();
			grd_Line.RefreshDataSource();
			txt_BarCode.Text = "";
			txt_BarCode.Focus();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Packing_Click(object sender, EventArgs e)
	{
		try
		{
			if (XtraMessageBox.Show(Apzon.Commons.Properties.Resources.xtraMsg_SaveChange, Apzon.Commons.Properties.Resources.Msg, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
			{
				return;
			}
			WinCommons._OpenProcessing();
			DataTable dataSourceTable = grd_Line.DataSourceTable;
			if (!dataSourceTable.IsNotNull())
			{
				WinCommons.Error(Enums.MessageCode.None, "Data is Empty!");
				return;
			}
			if (UdoTable == null || UdoTable.Rows.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.None, "UDO Table is invalid.");
				return;
			}
			DataSet dataSet = new DataSet();
			string tableName = Function.ToString(UdoTable.Rows[0]["TableName"]);
			dataSet.Tables.Add(UdoTable.Copy());
			DataTable dataTable = new DataTable(tableName);
			dataTable.Columns.Add("DocEntry", typeof(int));
			dataTable.Columns.Add("CreateDate", typeof(DateTime));
			dataTable.Columns.Add("U_BoxCode", typeof(string));
			DataRow dataRow = dataTable.NewRow();
			dataRow["DocEntry"] = 0;
			dataRow["CreateDate"] = DateTime.Today;
			dataRow["U_BoxCode"] = "";
			dataTable.Rows.Add(dataRow);
			dataSet.Tables.Add(dataTable);
			DataTable dataTable2 = new DataTable();
			dataTable2.TableName = "PKB1";
			dataTable2.Columns.Add("DocEntry", typeof(int));
			dataTable2.Columns.Add("LineId", typeof(int));
			dataTable2.Columns.Add("U_ItemCode", typeof(string));
			dataTable2.Columns.Add("U_ItemName", typeof(string));
			dataTable2.Columns.Add("U_LotNo", typeof(string));
			dataTable2.Columns.Add("U_Quantity", typeof(double));
			dataTable2.Columns.Add("U_BagCode", typeof(string));
			dataTable2.Columns.Add("U_TemKey", typeof(string));
			foreach (DataRow row in dataSourceTable.Rows)
			{
				DataRow dataRow3 = dataTable2.NewRow();
				dataRow3["DocEntry"] = 0;
				dataRow3["LineId"] = 0;
				dataRow3["U_ItemCode"] = Function.ToString(row["ItemCode"]);
				dataRow3["U_ItemName"] = Function.ToString(row["ItemDescription"]);
				dataRow3["U_LotNo"] = Function.ToString(row["DistNumber"]);
				dataRow3["U_Quantity"] = Function.ParseDouble(row["Quantity"]);
				dataRow3["U_BagCode"] = Function.ToString(row["BagCode"]);
				dataRow3["U_TemKey"] = Function.ToString(row["TempKey"]);
				dataTable2.Rows.Add(dataRow3);
			}
			dataSet.Tables.Add(dataTable2);
			HttpResult httpResult = await _httpClient.Udo.Save(dataSet);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				XtraMessageBox.Show("Add Packing Box error: " + httpResult.Message, "Error", MessageBoxButtons.OK);
			}
			WinCommons.Success(Enums.MessageCode.Success, Apzon.Commons.Properties.Resources.Msg_SaveSuccess);
			grd_Line.DataSource = null;
			grd_Line.RefreshDataSource();
			txt_BarCode.Text = "";
			txt_BarCode.Focus();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Production.frm_ReceiptFromPOByBarcode));
		this.btn_Update = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_Line = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Line = new Apzon.Controls.Grid.GridViewEx();
		this.txt_BarCode = new Apzon.Controls.Editor.TextEditEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.lbl_BarCode = new Apzon.Controls.Controls.LabelLine();
		this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
		this.btn_Packing = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_AddPacking = new Apzon.Controls.Controls.SimpleButtonEx();
		this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
		this.panelControl3 = new DevExpress.XtraEditors.PanelControl();
		((System.ComponentModel.ISupportInitialize)this.grd_Line).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Line).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BarCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl1).BeginInit();
		this.panelControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.panelControl2).BeginInit();
		this.panelControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.panelControl3).BeginInit();
		this.panelControl3.SuspendLayout();
		base.SuspendLayout();
		this.btn_Update.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Update.EditControl = null;
		this.btn_Update.EditValue = "OK";
		this.btn_Update.FieldName = null;
		this.btn_Update.ImageOptions.Image = Apzon.Forms.Properties.Resources.apply_16x16;
		this.btn_Update.IsSearchControl = false;
		this.btn_Update.Location = new System.Drawing.Point(714, 7);
		this.btn_Update.Name = "btn_Update";
		this.btn_Update.ReadOnly = false;
		this.btn_Update.SettingInVisible = false;
		this.btn_Update.SettingReadonly = false;
		this.btn_Update.Size = new System.Drawing.Size(75, 23);
		this.btn_Update.TabIndex = 18;
		this.btn_Update.Text = "OK";
		this.btn_Update.Visible = false;
		this.btn_Update.Click += new System.EventHandler(this.btn_Update_Click);
		this.grd_Line.AllowPasteExcel = true;
		this.grd_Line.ClipboardData = null;
		this.grd_Line.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Line.Location = new System.Drawing.Point(2, 2);
		this.grd_Line.MainView = this.gv_Line;
		this.grd_Line.Name = "grd_Line";
		this.grd_Line.Size = new System.Drawing.Size(797, 383);
		this.grd_Line.TabIndex = 0;
		this.grd_Line.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Line });
		this.gv_Line.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Line.ColumnKey = null;
		this.gv_Line.DisableFormSetting = false;
		this.gv_Line.DisplayException = false;
		this.gv_Line.EditColumn = null;
		this.gv_Line.FieldCurrency = null;
		this.gv_Line.FieldRate = null;
		this.gv_Line.FormId = null;
		this.gv_Line.GridControl = this.grd_Line;
		this.gv_Line.IgnoreFilter = false;
		this.gv_Line.IndexEditRow = -1;
		this.gv_Line.IndicatorWidth = 35;
		this.gv_Line.IsApzTable = false;
		this.gv_Line.IsInsertNewRow = false;
		this.gv_Line.IsLoadingData = false;
		this.gv_Line.IsMappingContextAction = false;
		this.gv_Line.ItemId = null;
		this.gv_Line.LstColumnAllowBatch = null;
		this.gv_Line.LstColumnNotAllowBatch = null;
		this.gv_Line.LstColumnNotEditable = null;
		this.gv_Line.LstColumNotSetting = null;
		this.gv_Line.Name = "gv_Line";
		this.gv_Line.OptionsView.ShowGroupPanel = false;
		this.gv_Line.UseBatchMod = false;
		this.gv_Line.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(this.gv_Line_PopupMenuShowing);
		this.txt_BarCode.EditControl = null;
		this.txt_BarCode.FieldName = null;
		this.txt_BarCode.IsSearchControl = false;
		this.txt_BarCode.Location = new System.Drawing.Point(98, 5);
		this.txt_BarCode.Name = "txt_BarCode";
		this.txt_BarCode.SettingInVisible = false;
		this.txt_BarCode.SettingReadonly = false;
		this.txt_BarCode.Size = new System.Drawing.Size(245, 20);
		this.txt_BarCode.TabIndex = 0;
		this.txt_BarCode.ToolTip = "Bắn barcode tem cần packing";
		this.txt_BarCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_BarCode_KeyDown);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = Apzon.Forms.Properties.Resources.cancel_16x16;
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(127, 7);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 1;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.ToolTip = "Thoát";
		this.btn_Cancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.lbl_BarCode.EditControl = null;
		this.lbl_BarCode.EditValue = "QR Code";
		this.lbl_BarCode.FieldName = null;
		this.lbl_BarCode.HideWhenEmpty = false;
		this.lbl_BarCode.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_BarCode.Location = new System.Drawing.Point(11, 7);
		this.lbl_BarCode.Name = "lbl_BarCode";
		this.lbl_BarCode.ReadOnly = false;
		this.lbl_BarCode.SettingInVisible = false;
		this.lbl_BarCode.SettingReadonly = false;
		this.lbl_BarCode.ShowUnderLine = true;
		this.lbl_BarCode.Size = new System.Drawing.Size(89, 20);
		this.lbl_BarCode.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_BarCode.TabIndex = 12;
		this.lbl_BarCode.Utext = "QR Code";
		this.panelControl1.Controls.Add(this.btn_Packing);
		this.panelControl1.Controls.Add(this.btn_AddPacking);
		this.panelControl1.Controls.Add(this.btn_Cancel);
		this.panelControl1.Controls.Add(this.btn_Update);
		this.panelControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.panelControl1.Location = new System.Drawing.Point(0, 425);
		this.panelControl1.Name = "panelControl1";
		this.panelControl1.Size = new System.Drawing.Size(801, 35);
		this.panelControl1.TabIndex = 22;
		this.btn_Packing.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Packing.EditControl = null;
		this.btn_Packing.EditValue = "Packing";
		this.btn_Packing.FieldName = null;
		this.btn_Packing.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Packing.ImageOptions.Image");
		this.btn_Packing.IsSearchControl = false;
		this.btn_Packing.Location = new System.Drawing.Point(633, 7);
		this.btn_Packing.Name = "btn_Packing";
		this.btn_Packing.ReadOnly = false;
		this.btn_Packing.SettingInVisible = false;
		this.btn_Packing.SettingReadonly = false;
		this.btn_Packing.Size = new System.Drawing.Size(75, 23);
		this.btn_Packing.TabIndex = 21;
		this.btn_Packing.Text = "Packing";
		this.btn_Packing.Visible = false;
		this.btn_Packing.Click += new System.EventHandler(this.btn_Packing_Click);
		this.btn_AddPacking.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_AddPacking.EditControl = null;
		this.btn_AddPacking.EditValue = "Add && Packing";
		this.btn_AddPacking.FieldName = null;
		this.btn_AddPacking.ImageOptions.Image = Apzon.Forms.Properties.Resources.apply_16x16;
		this.btn_AddPacking.IsSearchControl = false;
		this.btn_AddPacking.Location = new System.Drawing.Point(11, 7);
		this.btn_AddPacking.Name = "btn_AddPacking";
		this.btn_AddPacking.ReadOnly = false;
		this.btn_AddPacking.SettingInVisible = false;
		this.btn_AddPacking.SettingReadonly = false;
		this.btn_AddPacking.Size = new System.Drawing.Size(110, 23);
		this.btn_AddPacking.TabIndex = 0;
		this.btn_AddPacking.Text = "Add && Packing";
		this.btn_AddPacking.Click += new System.EventHandler(this.btn_AddPacking_Click);
		this.panelControl2.Controls.Add(this.txt_BarCode);
		this.panelControl2.Controls.Add(this.lbl_BarCode);
		this.panelControl2.Dock = System.Windows.Forms.DockStyle.Top;
		this.panelControl2.Location = new System.Drawing.Point(0, 0);
		this.panelControl2.Name = "panelControl2";
		this.panelControl2.Size = new System.Drawing.Size(801, 38);
		this.panelControl2.TabIndex = 23;
		this.panelControl3.Controls.Add(this.grd_Line);
		this.panelControl3.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panelControl3.Location = new System.Drawing.Point(0, 38);
		this.panelControl3.Name = "panelControl3";
		this.panelControl3.Size = new System.Drawing.Size(801, 387);
		this.panelControl3.TabIndex = 24;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(801, 460);
		base.Controls.Add(this.panelControl3);
		base.Controls.Add(this.panelControl2);
		base.Controls.Add(this.panelControl1);
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_ReceiptFromPOByBarcode";
		base.ShowInTaskbar = false;
		this.Text = "Receipt From Production";
		base.Load += new System.EventHandler(this.frm_ReceiptFromPOByBarcode_Load);
		((System.ComponentModel.ISupportInitialize)this.grd_Line).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Line).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BarCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl1).EndInit();
		this.panelControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.panelControl2).EndInit();
		this.panelControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.panelControl3).EndInit();
		this.panelControl3.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
