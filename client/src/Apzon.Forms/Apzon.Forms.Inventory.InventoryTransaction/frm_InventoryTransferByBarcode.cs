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
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Forms.Properties;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Menu;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using SAPbobsCOM;

namespace Apzon.Forms.Inventory.InventoryTransaction;

public class frm_InventoryTransferByBarcode : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private List<UserFieldsImpl> _lstUDFLine;

	private List<UserFieldsImpl> _lstUDFHeader;

	private IContainer components;

	private TextEditEx txt_User;

	private LabelLine labelLine2;

	private LabelLine labelLine3;

	private DateEditEx date_Doc;

	private LabelLine BIN;

	private LabelLine labelLine4;

	private TextEditEx txt_Batch;

	private GridControlEx grd_Counting;

	private SimpleButtonEx btn_OK;

	private ButtonEditEx bte_BIN;

	private SimpleButtonEx btn_Clear;

	private BackgroundWorker backgroundWorker1;

	private GridViewEx gv_Counting;

	private SimpleButtonEx btn_Cancel;

	private ButtonEditEx bte_FromBin;

	private LabelLine labelLine1;

	private LabelLine labelLine5;

	private TextEditEx txt_TransferRequest;

	public DateTime GetDate { get; set; }

	public frm_InventoryTransferByBarcode()
	{
		InitializeComponent();
	}

	private async void frm_InventoryTransferByBarcode_Load(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			DateTime today = DateTime.Today;
			date_Doc.EditValue = today;
			GetDate = today;
			txt_User.Text = GlobalData.UserName;
			_lstUDFHeader = await _httpClient.Udf.GetListField("OWTR");
			_lstUDFLine = await _httpClient.Udf.GetListField("WTR1");
			ResetData();
			bte_BIN.Focus();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void txt_Batch_KeyDown(object sender, KeyEventArgs e)
	{
		_ = 1;
		try
		{
			if (e.KeyCode != Keys.Return || string.IsNullOrEmpty(txt_Batch.Text))
			{
				return;
			}
			enableControls(isEnable: false);
			WinCommons._OpenProcessing();
			string batchNo = Function.ToString(txt_Batch.Text);
			string whs = Function.ToString(bte_BIN.Text);
			string fromWhs = Function.ToString(bte_FromBin.Text);
			int requestId = Function.ParseInt(txt_TransferRequest.Text);
			DataSetResult dataSetResult = await _httpClient.InventoryCountingBin.GetDataByBarcode(batchNo, whs, fromWhs, requestId);
			if (!string.IsNullOrEmpty(dataSetResult.Message))
			{
				WinCommons.Error(Enums.MessageCode.None, dataSetResult.Message);
				enableControls(isEnable: true);
				txt_Batch.Text = string.Empty;
				txt_Batch.Focus();
				return;
			}
			DataSet ds = dataSetResult.Data;
			if (ds.IsHasStructure())
			{
				if (gv_Counting.Columns.Count <= 0)
				{
					await GridViewHelper.GenerateGrid(grd_Counting, gv_Counting, ds);
					if (!ds.Tables[1].IsNotNull())
					{
						WinCommons.Warning(Enums.MessageCode.None, "Data is Empty!");
					}
				}
				else
				{
					DataTable dataSourceTable = grd_Counting.DataSourceTable;
					if (dataSourceTable.IsNotNull() && ds.Tables[1].IsNotNull())
					{
						foreach (DataRow row in ds.Tables[1].Rows)
						{
							string text = Function.ToString(row["ItemCode"]);
							string text2 = Function.ToString(row["DistNumber"]);
							string text3 = Function.ToString(row["TempKey"]);
							string text4 = Function.ToString(row["Whs"]);
							if (dataSourceTable.Select($"ItemCode='{text}' AND DistNumber='{text2}' AND Whs='{text4}' AND TempKey='{text3}'").Length != 0)
							{
								WinCommons.Error(Enums.MessageCode.None, "Tem đã được bắn!");
								break;
							}
							DataRow dataRow2 = dataSourceTable.NewRow();
							dataRow2.ItemArray = row.ItemArray;
							dataSourceTable.Rows.Add(dataRow2);
						}
						gv_Counting.FocusedRowHandle = dataSourceTable.Rows.Count - 1;
					}
					else if (ds.Tables[1].IsNotNull())
					{
						grd_Counting.DataSource = ds.Tables[1];
						grd_Counting.RefreshDataSource();
					}
					else
					{
						WinCommons.Warning(Enums.MessageCode.None, "Data is Empty!");
					}
				}
				btn_OK.Text = "Transfer";
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, "Không tìm thấy bản ghi nào.");
			}
			WinCommons._CloseProcessing();
			ds.Tables.Clear();
			ds.Dispose();
			enableControls(isEnable: true);
			txt_Batch.Text = string.Empty;
			txt_Batch.Focus();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			enableControls(isEnable: true);
			txt_Batch.Text = string.Empty;
			txt_Batch.Focus();
		}
	}

	private void enableControls(bool isEnable)
	{
		bte_FromBin.Enabled = isEnable;
		bte_BIN.Enabled = isEnable;
		txt_TransferRequest.Enabled = isEnable;
		date_Doc.Enabled = isEnable;
		btn_Clear.Enabled = isEnable;
		btn_OK.Enabled = isEnable;
		btn_Cancel.Enabled = isEnable;
		grd_Counting.Enabled = isEnable;
		txt_Batch.Enabled = isEnable;
	}

	private void txt_Bin_KeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.Return)
			{
				txt_Batch.Enabled = true;
				txt_Batch.Focus();
				txt_Batch.Text = string.Empty;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private void bte_BIN_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.Bin, multiSelect: false, Function.ToString(bte_BIN.EditValue));
			frm_ChooseFromList2.ShowDialog(this);
			if (frm_ChooseFromList2.ChooseItem != null)
			{
				string editValue = Function.ToString(((DataRowView)frm_ChooseFromList2.ChooseItem)["BinCode"]);
				bte_BIN.EditValue = editValue;
				bte_BIN.Focus();
				SendKeys.Send("{Enter}");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Transfer_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (XtraMessageBox.Show(Apzon.Commons.Properties.Resources.xtraMsg_SaveChange, Apzon.Commons.Properties.Resources.Msg, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
			{
				return;
			}
			btn_OK.Enabled = false;
			WinCommons._OpenProcessing();
			DataTable dataTable = grd_Counting.DatasourceTable();
			if (!dataTable.IsNotNull())
			{
				XtraMessageBox.Show("Data is Empty!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
			DocumentsImpl documentsImpl = new DocumentsImpl();
			DateTime dateTime = (documentsImpl.DocDate = Function.ParseDateTimes(date_Doc.EditValue));
			date_Doc.EditValue = dateTime;
			documentsImpl.TaxDate = dateTime;
			documentsImpl.DocDueDate = dateTime;
			documentsImpl.Lines = new List<Document_LinesImpl>();
			documentsImpl.UserFields = new List<UserFieldsImpl>();
			var enumerable = from t in dataTable.AsEnumerable()
				group t by new
				{
					ItemCode = Function.ToString(t["ItemCode"]),
					Whs = Function.ToString(t["Whs"]),
					ToWhs = Function.ToString(t["ToWhs"])
				} into g
				select new
				{
					Key = g.Key,
					Items = g
				};
			int num = 0;
			List<DataColumn> list = (from DataColumn t in dataTable.Columns
				where t.ColumnName.StartsWith("U_")
				select (t)).ToList();
			if (list.Count > 0)
			{
				foreach (DataColumn udfCol in list)
				{
					UserFieldsImpl userFieldsImpl = _lstUDFHeader.Where((UserFieldsImpl m) => "U_" + m.Name == udfCol.ColumnName).FirstOrDefault();
					if (userFieldsImpl != null)
					{
						userFieldsImpl.Value = dataTable.Rows[0][udfCol.ColumnName];
						documentsImpl.UserFields.Add(userFieldsImpl);
					}
				}
			}
			foreach (var item in enumerable)
			{
				Document_LinesImpl document_LinesImpl = new Document_LinesImpl();
				document_LinesImpl.ItemCode = Function.ToString(item.Key.ItemCode);
				document_LinesImpl.FromWhsCod = Function.ToString(item.Key.Whs).ToUpper();
				if (Function.ParseInt(item.Items.First()["BaseEntry"]) > 0)
				{
					document_LinesImpl.BaseType = 1250000001;
					document_LinesImpl.BaseEntry = Function.ParseInt(item.Items.First()["BaseEntry"]);
					document_LinesImpl.BaseLine = Function.ParseInt(item.Items.First()["BaseLine"]);
				}
				document_LinesImpl.Quantity = 0.0;
				if (string.IsNullOrEmpty(documentsImpl.Filler))
				{
					documentsImpl.Filler = document_LinesImpl.FromWhsCod;
				}
				document_LinesImpl.WhsCode = Function.ToString(item.Key.ToWhs).ToUpper();
				if (string.IsNullOrEmpty(documentsImpl.ToWhsCode))
				{
					documentsImpl.ToWhsCode = document_LinesImpl.WhsCode;
				}
				int num2 = 0;
				document_LinesImpl.BatchNumbers = new List<BatchNumbersImpl>();
				document_LinesImpl.SerialNumbers = new List<SerialNumbersImpl>();
				document_LinesImpl.BinAllocations = new List<DocumentLinesBinAllocationsImpl>();
				document_LinesImpl.UserFields = new List<UserFieldsImpl>();
				if (list.Count > 0)
				{
					foreach (DataColumn udfCol2 in list)
					{
						UserFieldsImpl userFieldsImpl2 = _lstUDFLine.Where((UserFieldsImpl m) => "U_" + m.Name == udfCol2.ColumnName).FirstOrDefault();
						if (userFieldsImpl2 != null)
						{
							userFieldsImpl2.Value = item.Items.First()[udfCol2.ColumnName];
							document_LinesImpl.UserFields.Add(userFieldsImpl2);
						}
					}
				}
				foreach (var item2 in from g in item.Items
					group g by new
					{
						DistNumber = Function.ToString(g["DistNumber"]),
						ObjType = Function.ToString(g["ObjType"]),
						SystemNumber = Function.ParseInt(g["SysNumber"]),
						BinId = Function.ParseInt(g["BinId"]),
						ToBinId = Function.ParseInt(g["ToBinId"])
					} into g
					select new
					{
						Key = g.Key,
						Quantity = g.Sum((DataRow dat) => Function.ParseDouble(dat["Quantity"])),
						Bins = g
					})
				{
					int binId = item2.Key.BinId;
					int toBinId = item2.Key.ToBinId;
					if (binId == 0 || toBinId == 0 || !binId.Equals(toBinId))
					{
						double quantity = item2.Quantity;
						document_LinesImpl.Quantity += quantity;
						string objType = item2.Key.ObjType;
						if (objType.Equals("10000044"))
						{
							BatchNumbersImpl batchNumbersImpl = new BatchNumbersImpl();
							batchNumbersImpl.BatchNumber = item2.Key.DistNumber;
							batchNumbersImpl.Quantity = quantity;
							batchNumbersImpl.BaseLineNumber = num;
							document_LinesImpl.BatchNumbers.Add(batchNumbersImpl);
						}
						else if (objType.Equals("10000045"))
						{
							SerialNumbersImpl serialNumbersImpl = new SerialNumbersImpl();
							serialNumbersImpl.BatchID = item2.Key.DistNumber;
							serialNumbersImpl.SystemSerialNumber = item2.Key.SystemNumber;
							serialNumbersImpl.BaseLineNumber = document_LinesImpl.LineNum;
							serialNumbersImpl.Quantity = quantity;
							document_LinesImpl.SerialNumbers.Add(serialNumbersImpl);
						}
						if (binId > 0)
						{
							DocumentLinesBinAllocationsImpl documentLinesBinAllocationsImpl = new DocumentLinesBinAllocationsImpl();
							documentLinesBinAllocationsImpl.BinAbsEntry = binId;
							documentLinesBinAllocationsImpl.BaseLineNumber = num;
							documentLinesBinAllocationsImpl.BinActionType = BinActionTypeEnum.batFromWarehouse;
							documentLinesBinAllocationsImpl.Quantity = quantity;
							documentLinesBinAllocationsImpl.SerialAndBatchNumbersBaseLine = num2;
							document_LinesImpl.BinAllocations.Add(documentLinesBinAllocationsImpl);
						}
						if (toBinId > 0)
						{
							DocumentLinesBinAllocationsImpl documentLinesBinAllocationsImpl2 = new DocumentLinesBinAllocationsImpl();
							documentLinesBinAllocationsImpl2.BinAbsEntry = toBinId;
							documentLinesBinAllocationsImpl2.BaseLineNumber = num;
							documentLinesBinAllocationsImpl2.BinActionType = BinActionTypeEnum.batToWarehouse;
							documentLinesBinAllocationsImpl2.Quantity = quantity;
							documentLinesBinAllocationsImpl2.SerialAndBatchNumbersBaseLine = num2;
							document_LinesImpl.BinAllocations.Add(documentLinesBinAllocationsImpl2);
						}
						num2++;
					}
				}
				document_LinesImpl.LineNum = num;
				documentsImpl.Lines.Add(document_LinesImpl);
				num++;
			}
			if (documentsImpl.Lines.Count == 0)
			{
				WinCommons.Success(Enums.MessageCode.Success, Apzon.Commons.Properties.Resources.Msg_SaveSuccess);
				grd_Counting.DataSource = null;
				grd_Counting.RefreshDataSource();
				Logging.Write("WATCH", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Không có dữ liệu chuyển BIN");
			}
			else
			{
				HttpResult httpResult = await _httpClient.StockTransfer.SaveDoc(documentsImpl);
				if (!string.IsNullOrEmpty(httpResult.Message))
				{
					XtraMessageBox.Show(httpResult.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				}
				else
				{
					WinCommons.Success(Enums.MessageCode.Success, $"{Apzon.Commons.Properties.Resources.Msg_Success} - Doc No. - {Function.ParseInt(httpResult.Content)}");
					grd_Counting.DataSource = null;
					grd_Counting.RefreshDataSource();
					await WinFuntions.PrintLayout("", Function.ParseInt(httpResult.Content), "WTR1");
					txt_TransferRequest.Text = "";
					bte_BIN.Text = "";
					bte_BIN.Focus();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			btn_OK.Enabled = true;
		}
		WinCommons._CloseProcessing();
	}

	private void simpleButton2_Click(object sender, EventArgs e)
	{
		try
		{
			grd_Counting.DataSource = null;
			bte_FromBin.Text = "";
			bte_BIN.EditValue = "";
			txt_Batch.Text = "";
			bte_BIN.Focus();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ResetData()
	{
		try
		{
			bte_FromBin.Text = "";
			txt_Batch.Text = "";
			btn_OK.Text = Apzon.Commons.Properties.Resources.ButtonOK;
			bte_BIN.Text = "";
			bte_BIN.Focus();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Counting_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
	{
		try
		{
			GridViewMenu gridViewMenu = new GridViewMenu((GridView)sender);
			DXMenuItem dXMenuItem = new DXMenuItem();
			dXMenuItem.Caption = "Remove Row";
			dXMenuItem.Click += gv_CountingMenu_Click;
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
			WinCommons.Error(Enums.MessageCode.None, ex.Message);
		}
	}

	private void gv_CountingMenu_Click(object sender, EventArgs e)
	{
		try
		{
			if ((sender as DXMenuItem).Caption.Equals("Remove Row") && gv_Counting.FocusedRowHandle > -1)
			{
				gv_Counting.DeleteRow(gv_Counting.FocusedRowHandle);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void simpleButtonEx1_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void bte_FromBin_KeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.Return)
			{
				bte_BIN.Focus();
				bte_BIN.Text = string.Empty;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private async void txt_TransferRequest_KeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode != Keys.Return || string.IsNullOrEmpty(txt_TransferRequest.Text))
			{
				return;
			}
			WinCommons._OpenProcessing();
			DataTableResult dataTableResult = await _httpClient.InventoryCountingBin.GetWarehouseRequest(txt_TransferRequest.Text);
			if (!string.IsNullOrEmpty(dataTableResult.Message))
			{
				WinCommons.Error(Enums.MessageCode.None, dataTableResult.Message);
				txt_Batch.Text = string.Empty;
				txt_Batch.Enabled = true;
				txt_Batch.Focus();
				return;
			}
			DataTable data = dataTableResult.Data;
			if (data.IsNotNull())
			{
				bte_FromBin.Text = Function.ToString(data.Rows[0]["Filler"]);
				bte_BIN.Text = Function.ToString(data.Rows[0]["ToWhsCode"]);
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, "Không tìm thấy bản ghi nào.");
			}
			WinCommons._CloseProcessing();
			txt_Batch.Text = string.Empty;
			txt_Batch.Enabled = true;
			txt_Batch.Focus();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			txt_Batch.Text = string.Empty;
			txt_Batch.Enabled = true;
			txt_Batch.Focus();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Inventory.InventoryTransaction.frm_InventoryTransferByBarcode));
		this.txt_User = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.date_Doc = new Apzon.Controls.Editor.DateEditEx();
		this.BIN = new Apzon.Controls.Controls.LabelLine();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.txt_Batch = new Apzon.Controls.Editor.TextEditEx();
		this.grd_Counting = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Counting = new Apzon.Controls.Grid.GridViewEx();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.bte_BIN = new Apzon.Controls.Editor.ButtonEditEx();
		this.btn_Clear = new Apzon.Controls.Controls.SimpleButtonEx();
		this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.bte_FromBin = new Apzon.Controls.Editor.ButtonEditEx();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine5 = new Apzon.Controls.Controls.LabelLine();
		this.txt_TransferRequest = new Apzon.Controls.Editor.TextEditEx();
		((System.ComponentModel.ISupportInitialize)this.txt_User.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_Doc.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_Doc.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Batch.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Counting).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Counting).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bte_BIN.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bte_FromBin.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_TransferRequest.Properties).BeginInit();
		base.SuspendLayout();
		this.txt_User.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txt_User.EditControl = null;
		this.txt_User.FieldName = null;
		this.txt_User.IsSearchControl = false;
		this.txt_User.Location = new System.Drawing.Point(734, 6);
		this.txt_User.Name = "txt_User";
		this.txt_User.Properties.ReadOnly = true;
		this.txt_User.SettingInVisible = false;
		this.txt_User.SettingReadonly = false;
		this.txt_User.Size = new System.Drawing.Size(142, 20);
		this.txt_User.TabIndex = 3;
		this.labelLine2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "User";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(655, 8);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(80, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 2;
		this.labelLine2.Utext = "User";
		this.labelLine3.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "Doc Date";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(655, 35);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(80, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 4;
		this.labelLine3.Utext = "Doc Date";
		this.date_Doc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.date_Doc.EditControl = null;
		this.date_Doc.EditValue = null;
		this.date_Doc.FieldName = null;
		this.date_Doc.Location = new System.Drawing.Point(734, 32);
		this.date_Doc.Name = "date_Doc";
		this.date_Doc.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_Doc.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_Doc.Properties.IsSearchControl = false;
		this.date_Doc.SettingInVisible = false;
		this.date_Doc.SettingReadonly = false;
		this.date_Doc.Size = new System.Drawing.Size(142, 20);
		this.date_Doc.TabIndex = 5;
		this.BIN.EditControl = null;
		this.BIN.EditValue = "To Warehouse";
		this.BIN.FieldName = null;
		this.BIN.HideWhenEmpty = false;
		this.BIN.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.BIN.Location = new System.Drawing.Point(5, 33);
		this.BIN.Name = "BIN";
		this.BIN.ReadOnly = false;
		this.BIN.SettingInVisible = false;
		this.BIN.SettingReadonly = false;
		this.BIN.ShowUnderLine = true;
		this.BIN.Size = new System.Drawing.Size(105, 20);
		this.BIN.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.BIN.TabIndex = 9;
		this.BIN.Utext = "To Warehouse";
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "QR Code";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(5, 59);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(105, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 11;
		this.labelLine4.Utext = "QR Code";
		this.txt_Batch.EditControl = null;
		this.txt_Batch.FieldName = null;
		this.txt_Batch.IsSearchControl = false;
		this.txt_Batch.Location = new System.Drawing.Point(110, 57);
		this.txt_Batch.Name = "txt_Batch";
		this.txt_Batch.SettingInVisible = false;
		this.txt_Batch.SettingReadonly = false;
		this.txt_Batch.Size = new System.Drawing.Size(241, 20);
		this.txt_Batch.TabIndex = 10;
		this.txt_Batch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_Batch_KeyDown);
		this.grd_Counting.AllowPasteExcel = true;
		this.grd_Counting.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Counting.ClipboardData = null;
		this.grd_Counting.Location = new System.Drawing.Point(0, 83);
		this.grd_Counting.MainView = this.gv_Counting;
		this.grd_Counting.Name = "grd_Counting";
		this.grd_Counting.Size = new System.Drawing.Size(876, 347);
		this.grd_Counting.TabIndex = 12;
		this.grd_Counting.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Counting });
		this.gv_Counting.Appearance.FocusedRow.BackColor = System.Drawing.Color.Transparent;
		this.gv_Counting.Appearance.FocusedRow.ForeColor = System.Drawing.Color.Transparent;
		this.gv_Counting.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Counting.Appearance.FocusedRow.Options.UseForeColor = true;
		this.gv_Counting.Appearance.SelectedRow.BackColor = System.Drawing.Color.Transparent;
		this.gv_Counting.Appearance.SelectedRow.ForeColor = System.Drawing.Color.Transparent;
		this.gv_Counting.Appearance.SelectedRow.Options.UseBackColor = true;
		this.gv_Counting.Appearance.SelectedRow.Options.UseForeColor = true;
		this.gv_Counting.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Counting.ColumnKey = null;
		this.gv_Counting.DisableFormSetting = false;
		this.gv_Counting.DisplayException = false;
		this.gv_Counting.EditColumn = null;
		this.gv_Counting.FieldCurrency = null;
		this.gv_Counting.FieldRate = null;
		this.gv_Counting.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFullFocus;
		this.gv_Counting.FormId = null;
		this.gv_Counting.GridControl = this.grd_Counting;
		this.gv_Counting.IgnoreFilter = false;
		this.gv_Counting.IndexEditRow = -1;
		this.gv_Counting.IndicatorWidth = 35;
		this.gv_Counting.IsApzTable = false;
		this.gv_Counting.IsInsertNewRow = false;
		this.gv_Counting.IsLoadingData = false;
		this.gv_Counting.IsMappingContextAction = false;
		this.gv_Counting.ItemId = null;
		this.gv_Counting.LstColumnAllowBatch = null;
		this.gv_Counting.LstColumnNotAllowBatch = null;
		this.gv_Counting.LstColumnNotEditable = null;
		this.gv_Counting.LstColumNotSetting = null;
		this.gv_Counting.Name = "gv_Counting";
		this.gv_Counting.OptionsView.ShowGroupPanel = false;
		this.gv_Counting.UseBatchMod = false;
		this.gv_Counting.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(this.gv_Counting_PopupMenuShowing);
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.EditControl = null;
		this.btn_OK.EditValue = "Transfer";
		this.btn_OK.FieldName = null;
		this.btn_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_OK.ImageOptions.Image");
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(12, 434);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.ReadOnly = false;
		this.btn_OK.SettingInVisible = false;
		this.btn_OK.SettingReadonly = false;
		this.btn_OK.Size = new System.Drawing.Size(75, 23);
		this.btn_OK.TabIndex = 15;
		this.btn_OK.Text = "Transfer";
		this.btn_OK.Click += new System.EventHandler(this.btn_Transfer_Click);
		this.bte_BIN.EditControl = null;
		this.bte_BIN.FieldName = null;
		this.bte_BIN.IsSearchControl = false;
		this.bte_BIN.Location = new System.Drawing.Point(110, 32);
		this.bte_BIN.Name = "bte_BIN";
		this.bte_BIN.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.bte_BIN.SettingInVisible = false;
		this.bte_BIN.SettingReadonly = false;
		this.bte_BIN.Size = new System.Drawing.Size(241, 20);
		this.bte_BIN.TabIndex = 18;
		this.bte_BIN.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.bte_BIN_ButtonClick);
		this.bte_BIN.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_Bin_KeyDown);
		this.btn_Clear.EditControl = null;
		this.btn_Clear.EditValue = "Clear";
		this.btn_Clear.FieldName = null;
		this.btn_Clear.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Clear.ImageOptions.Image");
		this.btn_Clear.IsSearchControl = false;
		this.btn_Clear.Location = new System.Drawing.Point(357, 57);
		this.btn_Clear.Name = "btn_Clear";
		this.btn_Clear.ReadOnly = false;
		this.btn_Clear.SettingInVisible = false;
		this.btn_Clear.SettingReadonly = false;
		this.btn_Clear.Size = new System.Drawing.Size(75, 20);
		this.btn_Clear.TabIndex = 19;
		this.btn_Clear.Text = "Clear";
		this.btn_Clear.Click += new System.EventHandler(this.simpleButton2_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = Apzon.Forms.Properties.Resources.cancel_16x16;
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(110, 434);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 20;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.ToolTip = "Thoát";
		this.btn_Cancel.Click += new System.EventHandler(this.simpleButtonEx1_Click);
		this.bte_FromBin.EditControl = null;
		this.bte_FromBin.FieldName = null;
		this.bte_FromBin.IsSearchControl = false;
		this.bte_FromBin.Location = new System.Drawing.Point(110, 5);
		this.bte_FromBin.Name = "bte_FromBin";
		this.bte_FromBin.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.bte_FromBin.SettingInVisible = false;
		this.bte_FromBin.SettingReadonly = false;
		this.bte_FromBin.Size = new System.Drawing.Size(241, 20);
		this.bte_FromBin.TabIndex = 22;
		this.bte_FromBin.KeyDown += new System.Windows.Forms.KeyEventHandler(this.bte_FromBin_KeyDown);
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "From Warehouse";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(5, 6);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(105, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 21;
		this.labelLine1.Utext = "From Warehouse";
		this.labelLine5.EditControl = null;
		this.labelLine5.EditValue = "Transfer Request";
		this.labelLine5.FieldName = null;
		this.labelLine5.HideWhenEmpty = false;
		this.labelLine5.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine5.Location = new System.Drawing.Point(357, 5);
		this.labelLine5.Name = "labelLine5";
		this.labelLine5.ReadOnly = false;
		this.labelLine5.SettingInVisible = false;
		this.labelLine5.SettingReadonly = false;
		this.labelLine5.ShowUnderLine = true;
		this.labelLine5.Size = new System.Drawing.Size(105, 20);
		this.labelLine5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine5.TabIndex = 24;
		this.labelLine5.Utext = "Transfer Request";
		this.txt_TransferRequest.EditControl = null;
		this.txt_TransferRequest.FieldName = null;
		this.txt_TransferRequest.IsSearchControl = false;
		this.txt_TransferRequest.Location = new System.Drawing.Point(462, 3);
		this.txt_TransferRequest.Name = "txt_TransferRequest";
		this.txt_TransferRequest.SettingInVisible = false;
		this.txt_TransferRequest.SettingReadonly = false;
		this.txt_TransferRequest.Size = new System.Drawing.Size(104, 20);
		this.txt_TransferRequest.TabIndex = 23;
		this.txt_TransferRequest.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_TransferRequest_KeyDown);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(877, 460);
		base.Controls.Add(this.labelLine5);
		base.Controls.Add(this.txt_TransferRequest);
		base.Controls.Add(this.bte_FromBin);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_Clear);
		base.Controls.Add(this.bte_BIN);
		base.Controls.Add(this.btn_OK);
		base.Controls.Add(this.grd_Counting);
		base.Controls.Add(this.labelLine4);
		base.Controls.Add(this.txt_Batch);
		base.Controls.Add(this.BIN);
		base.Controls.Add(this.date_Doc);
		base.Controls.Add(this.labelLine3);
		base.Controls.Add(this.txt_User);
		base.Controls.Add(this.labelLine2);
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_InventoryTransferByBarcode";
		base.ShowInTaskbar = false;
		this.Text = "Inventory Transfer";
		base.Load += new System.EventHandler(this.frm_InventoryTransferByBarcode_Load);
		((System.ComponentModel.ISupportInitialize)this.txt_User.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_Doc.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_Doc.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Batch.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Counting).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Counting).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bte_BIN.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bte_FromBin.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_TransferRequest.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
