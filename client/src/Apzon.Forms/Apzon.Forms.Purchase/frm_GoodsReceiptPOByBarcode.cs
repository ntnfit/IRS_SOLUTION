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
using Apzon.Entities.Models;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;
using Apzon.Entities.Models.UserFields;
using Apzon.Forms.Commons;
using Apzon.Forms.Helper;
using Apzon.Forms.Properties;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Menu;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using SAPbobsCOM;

namespace Apzon.Forms.Purchase;

public class frm_GoodsReceiptPOByBarcode : XtraFormEx
{
	private const string FormController = "GoodReceiptIt";

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private List<UserFieldsImpl> lstUdf = new List<UserFieldsImpl>();

	private DataTable dtCache = new DataTable();

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

	private DateEditEx date_Doc;

	private LabelLine labelLine3;

	private SimpleButtonEx btn_Refresh;

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
		case Enums.FormViewModeType.Add:
			btn_Update.Text = Apzon.Commons.Properties.Resources.ButtonAdd;
			break;
		case Enums.FormViewModeType.Update:
			btn_Update.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btn_Update.Text = Apzon.Commons.Properties.Resources.ButtonOK;
			break;
		}
	}

	public frm_GoodsReceiptPOByBarcode()
	{
		InitializeComponent();
	}

	private async void frm_GoodsReceiptPOByBarcode_Load(object sender, EventArgs e)
	{
		try
		{
			FormMode = Enums.FormViewModeType.Add;
			date_Doc.EditValue = DateTime.Today;
			lstUdf = await _httpClient.Udf.GetListField("OPDN");
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
			if (e.KeyCode != Keys.Return || string.IsNullOrEmpty(txt_BarCode.Text) || FormMode != Enums.FormViewModeType.Add)
			{
				return;
			}
			txt_BarCode.Enabled = false;
			WinCommons._OpenProcessing();
			DataSetResult dataSetResult = await _httpClient.GoodsReceiptPOByBarcodeClient.GetDataByBarcode(txt_BarCode.Text, GlobalData.UserName, GlobalData.DefaultBranch);
			if (!string.IsNullOrEmpty(dataSetResult.Message))
			{
				WinCommons.Error(Enums.MessageCode.None, dataSetResult.Message);
				txt_BarCode.Text = string.Empty;
				txt_BarCode.Enabled = true;
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
						XtraMessageBox.Show("Data is Empty!", "System", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
					}
					else
					{
						dtCache = ds.Tables[1].Copy();
						FormMode = Enums.FormViewModeType.Add;
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
							int num2 = Function.ParseInt(row["POLine"]);
							if (dtCache.Select($"ItemCode='{text}' AND DistNumber='{text2}' AND POEntry={num} AND POLine='{num2}' AND TempKey='{text3}'").Length != 0)
							{
								WinCommons.Error(Enums.MessageCode.None, "Tem đã được bắn!");
								break;
							}
							DataRow dataRow2 = dataSourceTable.NewRow();
							dataRow2.ItemArray = row.ItemArray;
							dataSourceTable.Rows.Add(dataRow2);
							DataRow dataRow3 = dtCache.NewRow();
							dataRow3.ItemArray = row.ItemArray;
							dtCache.Rows.Add(dataRow3);
						}
						gv_Line.FocusedRowHandle = dataSourceTable.Rows.Count - 1;
						FormMode = Enums.FormViewModeType.Add;
					}
					else if (ds.Tables[1].IsNotNull())
					{
						grd_Line.DataSource = ds.Tables[1];
						grd_Line.RefreshDataSource();
						FormMode = Enums.FormViewModeType.Add;
						dtCache = ds.Tables[1].Copy();
					}
					else
					{
						XtraMessageBox.Show("Data is Empty!", "System", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
					}
				}
				WinCommons._CloseProcessing();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, "Data is not found!");
			}
			ds.Tables.Clear();
			ds.Dispose();
			txt_BarCode.Text = string.Empty;
			txt_BarCode.Enabled = true;
			txt_BarCode.Focus();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btn_Update_Click(object sender, EventArgs e)
	{
		_ = 2;
		try
		{
			if (FormMode == Enums.FormViewModeType.View || XtraMessageBox.Show(Apzon.Commons.Properties.Resources.xtraMsg_SaveChange, Apzon.Commons.Properties.Resources.Msg, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
			{
				return;
			}
			WinCommons._OpenProcessing();
			DataTable dt = grd_Line.DataSourceTable;
			List<string> lstBasePO = new List<string>();
			if (!dt.IsNotNull())
			{
				WinCommons.Error(Enums.MessageCode.None, "Data is Empty!");
				return;
			}
			DocumentsImpl doc = new DocumentsImpl
			{
				BPLId = GlobalData.DefaultBranch,
				Comments = "GRPO",
				DocDate = Function.ParseDateTimes(date_Doc.EditValue),
				ObjType = 20,
				Lines = new List<Document_LinesImpl>()
			};
			var enumerable = from t in dt.AsEnumerable()
				group t by new
				{
					POEntry = Function.ToString(t["POEntry"]),
					POLine = Function.ToString(t["POLine"])
				} into g
				select new
				{
					Key = g.Key,
					Items = g
				};
			int j = 0;
			foreach (var grp in enumerable)
			{
				Document_LinesImpl line = new Document_LinesImpl
				{
					BatchNumbers = new List<BatchNumbersImpl>(),
					BinAllocations = new List<DocumentLinesBinAllocationsImpl>()
				};
				if (doc.DocRate <= 0.0)
				{
					DataTable dataTable = new DataTable();
					dataTable.Columns.Add(new DataColumn("poEntry", typeof(int)));
					dataTable.Columns.Add(new DataColumn("docDate", typeof(DateTime)));
					DataRow dataRow = dataTable.NewRow();
					dataRow["poEntry"] = Function.ParseInt(dt.Rows[0]["POEntry"]);
					dataRow["docDate"] = (string.IsNullOrEmpty(date_Doc.Text) ? DateTime.Today : Convert.ToDateTime(date_Doc.EditValue));
					dataTable.Rows.Add(dataRow);
					HttpResult httpResult = await _httpClient.GoodsReceiptPOByBarcodeClient.GetCurrentRate(dataTable);
					if (string.IsNullOrEmpty(httpResult.Message))
					{
						doc.DocRate = Function.ParseDouble(httpResult.Content);
					}
				}
				line.LineNum = j;
				double num = 0.0;
				List<DataColumn> list = (from DataColumn t in dt.Columns
					where t.ColumnName.StartsWith("U_")
					select (t)).ToList();
				foreach (DataRow item in grp.Items)
				{
					if (string.IsNullOrEmpty(doc.CardCode))
					{
						doc.CardCode = Function.ToString(item["CardCode"]);
					}
					if (string.IsNullOrEmpty(line.ItemCode))
					{
						line.ItemCode = Function.ToString(item["ItemCode"]);
					}
					if (item.IsExistsColumn("WhsCode") && string.IsNullOrEmpty(line.WhsCode))
					{
						line.WhsCode = Function.ToString(item["WhsCode"]);
					}
					if (item.IsExistsColumn("Price"))
					{
						line.Price = Function.ParseDouble(item["Price"]);
					}
					if (item.IsExistsColumn("Currency"))
					{
						line.Currency = Function.ToString(item["Currency"]);
					}
					if (item.IsExistsColumn("AcctCode"))
					{
						line.AcctCode = Function.ToString(item["AcctCode"]);
					}
					if (item.IsExistsColumn("POEntry") && Function.ParseInt(item["POEntry"]) > 0)
					{
						line.BaseEntry = Function.ParseInt(item["POEntry"]);
						line.BaseRef = Function.ParseInt(item["POEntry"]);
						line.BaseType = 22;
						lstBasePO.Add(line.BaseEntry.ToString());
					}
					if (item.IsExistsColumn("POLine") && Function.ParseInt(item["POLine"]) > 0)
					{
						line.BaseLine = Function.ParseInt(item["POLine"]);
					}
					BatchNumbersImpl batchNumbersImpl = new BatchNumbersImpl
					{
						Quantity = Function.ParseDouble(item["Quantity"]),
						BatchNumber = Function.ToString(item["DistNumber"]),
						AddmisionDate = DateTime.Today,
						BaseLineNumber = j
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
							userFieldsImpl.Value = item[list[num2]];
							batchNumbersImpl.UserFields.Add(userFieldsImpl);
						}
					}
					num += batchNumbersImpl.Quantity;
					line.BatchNumbers.Add(batchNumbersImpl);
				}
				line.Quantity = num;
				doc.Lines.Add(line);
				j++;
			}
			DataTable dataTable2 = _httpClient.Commons.CreateFreightGridDatasource("O");
			foreach (DataRow row in _httpClient.Commons.GetFreight("POR3", string.Join(",", lstBasePO)).Rows)
			{
				if (!Function.ToString(row["Status"]).Equals("C"))
				{
					DataRow dataRow2 = dataTable2.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["ExpnsCode"]) == Function.ParseInt(row["ExpnsCode"]));
					if (dataRow2 != null)
					{
						dataRow2["LineTotal"] = Function.ParseDouble(row["LineTotal"]);
						dataRow2["TotalFrgn"] = Function.ParseDouble(row["TotalFrgn"]);
						dataRow2["Comments"] = Function.ToString(row["Comments"]);
						dataRow2["VatGroup"] = Function.ToString(row["VatGroup"]);
						dataRow2["VatPrcnt"] = Function.ParseDouble(row["VatPrcnt"]);
						dataRow2["VatSum"] = Function.ParseDouble(row["VatSum"]);
						dataRow2["DistrbMthd"] = Function.ToString(row["DistrbMthd"]);
						dataRow2["GrsAmount"] = Function.ParseDouble(row["GrsAmount"]);
						dataRow2["OcrCode"] = Function.ToString(row["OcrCode"]);
						dataRow2["OcrCode2"] = Function.ToString(row["OcrCode2"]);
						dataRow2["OcrCode3"] = Function.ToString(row["OcrCode3"]);
						dataRow2["OcrCode4"] = Function.ToString(row["OcrCode4"]);
						dataRow2["OcrCode5"] = Function.ToString(row["OcrCode5"]);
						dataRow2["Project"] = Function.ToString(row["Project"]);
						dataRow2["BaseType"] = Function.ParseInt(row["BaseType"]);
						dataRow2["BaseAbsEnt"] = Function.ParseInt(row["BaseAbsEnt"]);
						dataRow2["BaseLnNum"] = Function.ParseInt(row["BaseLnNum"]);
						dataRow2["BaseRef"] = Function.ParseInt(row["BaseRef"]);
					}
				}
			}
			dataTable2.AcceptChanges();
			DataRow[] array = (from t in dataTable2.Copy().AsEnumerable()
				where !string.IsNullOrEmpty(Function.ToString(t["ExpnsCode"])) && (Function.ParseDouble(t["LineTotal"]) != 0.0 || Function.ParseDouble(t["TotalFrgn"]) != 0.0)
				select t).ToArray();
			if (array.Length != 0)
			{
				doc.Expenses = array.CopyToDataTable().ToDocumentsAdditional();
			}
			HttpResult httpResult2 = await _httpClient.GoodsReceiptPOByBarcodeClient.AddNew(doc);
			if (!string.IsNullOrEmpty(httpResult2.Message))
			{
				XtraMessageBox.Show(httpResult2.Message, "Error", MessageBoxButtons.OK);
			}
			else
			{
				WinCommons.Success(Enums.MessageCode.Success, Apzon.Commons.Properties.Resources.Msg_SaveSuccess);
				grd_Line.DataSource = null;
				grd_Line.RefreshDataSource();
				dtCache = new DataTable();
				await WinFuntions.PrintLayout("", Function.ParseInt(httpResult2.Content), "PDN2");
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
		Close();
	}

	private void gv_Line_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.View)
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
			date_Doc.EditValue = DateTime.Today;
			FormMode = Enums.FormViewModeType.Add;
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

	private void CreateFreightGridDatasource()
	{
		try
		{
			string vatType = "O";
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons");
			httpClientHelper.Get("CreateFreightGridDatasource", new { vatType });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Purchase.frm_GoodsReceiptPOByBarcode));
		this.btn_Update = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_Line = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Line = new Apzon.Controls.Grid.GridViewEx();
		this.txt_BarCode = new Apzon.Controls.Editor.TextEditEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.lbl_BarCode = new Apzon.Controls.Controls.LabelLine();
		this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
		this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
		this.btn_Refresh = new Apzon.Controls.Controls.SimpleButtonEx();
		this.date_Doc = new Apzon.Controls.Editor.DateEditEx();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.panelControl3 = new DevExpress.XtraEditors.PanelControl();
		((System.ComponentModel.ISupportInitialize)this.grd_Line).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Line).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BarCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl1).BeginInit();
		this.panelControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.panelControl2).BeginInit();
		this.panelControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.date_Doc.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_Doc.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl3).BeginInit();
		this.panelControl3.SuspendLayout();
		base.SuspendLayout();
		this.btn_Update.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Update.EditControl = null;
		this.btn_Update.EditValue = "OK";
		this.btn_Update.FieldName = null;
		this.btn_Update.ImageOptions.Image = Apzon.Forms.Properties.Resources.apply_16x16;
		this.btn_Update.IsSearchControl = false;
		this.btn_Update.Location = new System.Drawing.Point(5, 7);
		this.btn_Update.Name = "btn_Update";
		this.btn_Update.ReadOnly = false;
		this.btn_Update.SettingInVisible = false;
		this.btn_Update.SettingReadonly = false;
		this.btn_Update.Size = new System.Drawing.Size(75, 23);
		this.btn_Update.TabIndex = 0;
		this.btn_Update.Text = "OK";
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
		this.txt_BarCode.TabIndex = 1;
		this.txt_BarCode.ToolTip = "Bắn barcode tem cần packing";
		this.txt_BarCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_BarCode_KeyDown);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = Apzon.Forms.Properties.Resources.cancel_16x16;
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(83, 7);
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
		this.lbl_BarCode.Location = new System.Drawing.Point(11, 8);
		this.lbl_BarCode.Name = "lbl_BarCode";
		this.lbl_BarCode.ReadOnly = false;
		this.lbl_BarCode.SettingInVisible = false;
		this.lbl_BarCode.SettingReadonly = false;
		this.lbl_BarCode.ShowUnderLine = true;
		this.lbl_BarCode.Size = new System.Drawing.Size(89, 20);
		this.lbl_BarCode.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_BarCode.TabIndex = 12;
		this.lbl_BarCode.Utext = "QR Code";
		this.panelControl1.Controls.Add(this.btn_Cancel);
		this.panelControl1.Controls.Add(this.btn_Update);
		this.panelControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.panelControl1.Location = new System.Drawing.Point(0, 425);
		this.panelControl1.Name = "panelControl1";
		this.panelControl1.Size = new System.Drawing.Size(801, 35);
		this.panelControl1.TabIndex = 22;
		this.panelControl2.Controls.Add(this.btn_Refresh);
		this.panelControl2.Controls.Add(this.date_Doc);
		this.panelControl2.Controls.Add(this.labelLine3);
		this.panelControl2.Controls.Add(this.txt_BarCode);
		this.panelControl2.Controls.Add(this.lbl_BarCode);
		this.panelControl2.Dock = System.Windows.Forms.DockStyle.Top;
		this.panelControl2.Location = new System.Drawing.Point(0, 0);
		this.panelControl2.Name = "panelControl2";
		this.panelControl2.Size = new System.Drawing.Size(801, 38);
		this.panelControl2.TabIndex = 23;
		this.btn_Refresh.EditControl = null;
		this.btn_Refresh.EditValue = "Clear";
		this.btn_Refresh.FieldName = null;
		this.btn_Refresh.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Refresh.ImageOptions.Image");
		this.btn_Refresh.IsSearchControl = false;
		this.btn_Refresh.Location = new System.Drawing.Point(346, 5);
		this.btn_Refresh.Name = "btn_Refresh";
		this.btn_Refresh.ReadOnly = false;
		this.btn_Refresh.SettingInVisible = false;
		this.btn_Refresh.SettingReadonly = false;
		this.btn_Refresh.Size = new System.Drawing.Size(75, 20);
		this.btn_Refresh.TabIndex = 2;
		this.btn_Refresh.Text = "Clear";
		this.btn_Refresh.ToolTip = "Làm mới";
		this.btn_Refresh.Click += new System.EventHandler(this.btn_Refresh_Click);
		this.date_Doc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.date_Doc.EditControl = null;
		this.date_Doc.EditValue = null;
		this.date_Doc.FieldName = null;
		this.date_Doc.Location = new System.Drawing.Point(679, 5);
		this.date_Doc.Name = "date_Doc";
		this.date_Doc.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_Doc.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_Doc.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.date_Doc.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_Doc.Properties.EditFormat.FormatString = "dd/MM/yyyy";
		this.date_Doc.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_Doc.Properties.IsSearchControl = false;
		this.date_Doc.Properties.Mask.EditMask = "dd/MM/yyyy";
		this.date_Doc.SettingInVisible = false;
		this.date_Doc.SettingReadonly = false;
		this.date_Doc.Size = new System.Drawing.Size(117, 20);
		this.date_Doc.TabIndex = 0;
		this.date_Doc.ToolTip = "Ngày tạo";
		this.labelLine3.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "Doc Date";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(593, 8);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(89, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 16;
		this.labelLine3.Utext = "Doc Date";
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
		base.Name = "frm_GoodsReceiptPOByBarcode";
		base.ShowInTaskbar = false;
		this.Text = "Goods Receipt PO";
		base.Load += new System.EventHandler(this.frm_GoodsReceiptPOByBarcode_Load);
		((System.ComponentModel.ISupportInitialize)this.grd_Line).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Line).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BarCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl1).EndInit();
		this.panelControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.panelControl2).EndInit();
		this.panelControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.date_Doc.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_Doc.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl3).EndInit();
		this.panelControl3.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
