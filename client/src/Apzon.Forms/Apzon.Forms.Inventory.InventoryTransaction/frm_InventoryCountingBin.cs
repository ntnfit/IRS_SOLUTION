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
using Apzon.Entities.Models.Documents;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Menu;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using SAPbobsCOM;

namespace Apzon.Forms.Inventory.InventoryTransaction;

public class frm_InventoryCountingBin : XtraFormEx
{
	private CountingBinType _type;

	private const string UdoCodeTransfer = "OINC";

	public string UdoTableNameTransfer = "@OINC";

	public DataTable UdoTableTransfer = new DataTable();

	public string FormController = "InventoryCountingBin";

	private const string UdoCodeCounting = "OINB";

	public string UdoTableNameCounting = "@OINB";

	public DataTable UdoTableCounting = new DataTable();

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	public string BinLocation = "";

	private IContainer components;

	private LabelLine labelLine1;

	private TextEditEx txt_DocNo;

	private TextEditEx txt_User;

	private LabelLine labelLine2;

	private LabelLine labelLine3;

	private DateEditEx date_Doc;

	private LabelLine BIN;

	private LabelLine labelLine4;

	private TextEditEx txt_Batch;

	private GridControlEx grd_Counting;

	private SimpleButtonEx btn_OK;

	private SimpleButtonEx btn_Print;

	private SimpleButtonEx btn_Transfer;

	private ButtonEditEx bte_BIN;

	private SimpleButtonEx simpleButton2;

	private BackgroundWorker backgroundWorker1;

	private RadioGroupEx rdo_CountinBin;

	private LabelLine lb_Period;

	private LookUpEdit look_Period;

	private GridViewEx gv_Counting;

	public DateTime GetDate { get; set; }

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
		{
			WinFuntions.DisableAllControl(Controls);
			return;
		}
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Add:
			btn_OK.Text = Resources.ButtonAdd;
			break;
		case Enums.FormViewModeType.Update:
			btn_OK.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btn_OK.Text = Resources.ButtonOK;
			break;
		}
	}

	public frm_InventoryCountingBin()
	{
		InitializeComponent();
	}

	private async void frm_InventoryCountingBin_Load(object sender, EventArgs e)
	{
		_ = 3;
		try
		{
			UdoTableCounting = await _httpClient.Udo.GetUdoByCode(new Common.Table_Code
			{
				Name = "OUDO",
				Code = "OINB"
			});
			int num = await _httpClient.Udo.GetMaxId("OINC");
			txt_DocNo.Text = Function.ToString(num + 1);
			DateTime dateTime = await _httpClient.Commons.GetServerDate();
			date_Doc.EditValue = dateTime;
			GetDate = dateTime;
			txt_User.Text = GlobalData.UserName;
			ResetData();
			DataTable dataSource = await _httpClient.InventoryCountingBin.GetPeriods();
			look_Period.Properties.DataSource = dataSource;
			look_Period.Properties.DisplayMember = "PeriodName";
			look_Period.Properties.ValueMember = "PeriodCode";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void txt_Batch_KeyDown(object sender, KeyEventArgs e)
	{
		_ = 3;
		try
		{
			if (e.KeyCode != Keys.Return || string.IsNullOrEmpty(txt_Batch.Text))
			{
				return;
			}
			string text = Function.ToString(txt_Batch.Text);
			string text2 = Function.ToString(bte_BIN.Text);
			if (_type == CountingBinType.Transfer)
			{
				if (grd_Counting.DataSource != null)
				{
					DataTable dataTable = grd_Counting.DataSource as DataTable;
					if (dataTable.IsNotNull() && dataTable.Select($"U_DistNumber='{text}'").Length != 0)
					{
						WinCommons.Error(Enums.MessageCode.None, "Đã tồn tại \"Batch\" trong lưới dữ liệu");
						txt_Batch.Text = "";
						txt_Batch.Focus();
						return;
					}
				}
				DataSet dataSet = await _httpClient.InventoryCountingBin.TransferGet(text, text2);
				if (dataSet.IsHasValidValue())
				{
					int num = 0;
					if (dataSet.Tables[1].IsNotNull())
					{
						num = Function.ParseInt(dataSet.Tables[1].Rows[0]["PwNo"]);
					}
					if (num > 0 && !GlobalData.UserName.ToUpper().StartsWith("PR"))
					{
						WinCommons.Error(Enums.MessageCode.None, $"\"Batch No\" đã được chọn trong phiếu tính giấy: {num}. Yêu cầu bộ phận SX hủy phiếu!");
						txt_Batch.Text = string.Empty;
						txt_Batch.Focus();
						return;
					}
					if (gv_Counting.Columns.Count <= 0)
					{
						await GridViewHelper.GenerateGrid(grd_Counting, gv_Counting, dataSet.Tables[0], dataSet.Tables[1], dataSet.Tables[2]);
						gv_Counting.Columns[0].Visible = true;
					}
					else if (grd_Counting.DataSource != null && grd_Counting.DataSourceTable.IsNotNull() && grd_Counting.DataSourceTable.Rows.Count > 0)
					{
						DataTable dataTable2 = (DataTable)grd_Counting.DataSource;
						foreach (DataRow row in dataSet.Tables[1].Rows)
						{
							DataRow dataRow2 = dataTable2.NewRow();
							dataRow2.ItemArray = row.ItemArray;
							dataTable2.Rows.Add(dataRow2);
							gv_Counting.FocusedRowHandle = dataTable2.Rows.IndexOf(dataRow2);
						}
						dataTable2.AcceptChanges();
					}
					else
					{
						grd_Counting.DataSource = dataSet.Tables[1];
						grd_Counting.RefreshDataSource();
					}
					gv_Counting.SelectAll();
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, "Không tìm thấy bản ghi nào.");
				}
				txt_Batch.Text = string.Empty;
				txt_Batch.Focus();
			}
			else if (_type == CountingBinType.Counting)
			{
				string period = Function.ToString(look_Period.EditValue);
				int id = 0;
				DataTable dt = grd_Counting.DataSourceTable;
				if (dt.IsNotNull())
				{
					if (FormMode == Enums.FormViewModeType.View || FormMode == Enums.FormViewModeType.Update)
					{
						btn_OK.Text = Resources.ButtonUpdate;
					}
					else
					{
						btn_OK.Text = Resources.ButtonAdd;
					}
					DataRow dataRow3 = dt.Select(string.Format("U_DistNumber='{0}' AND (U_LogicBin='{1}' OR (ISNULL(U_LogicBin,'')='' AND U_LogicWhs='{1}')) AND (U_Status='{2}' OR U_Status='{3}' OR U_Status='{4}')", text, text2, CountingBinStatus.T2.ToString(), CountingBinStatus.SB.ToString(), CountingBinStatus.T1.ToString())).FirstOrDefault();
					if (dataRow3 != null && dataRow3.IsExistsColumn("U_Status"))
					{
						dataRow3["U_Status"] = CountingBinStatus.OK.ToString();
						dataRow3["Checked"] = true;
						id = Function.ParseInt(dataRow3["ID"]);
						dt.AcceptChanges();
					}
					else
					{
						DataRow dataRow4 = dt.Select(string.Format("U_DistNumber='{0}' AND (U_LogicBin='{1}' OR U_LogicWhs='{1}')", text, text2)).FirstOrDefault();
						if (dataRow4 != null)
						{
							string text3 = Function.ToString(dataRow4["U_Status"]);
							if (!text3.Equals(CountingBinStatus.T2.ToString()) && !text3.Equals(CountingBinStatus.T1.ToString()) && !text3.Equals(CountingBinStatus.SB.ToString()) && XtraMessageBox.Show("\"Batch No\" Đã được kiểm kê trước đó. Bạn có muốn tiếp tục thêm vào danh sách?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
							{
								DataRow dataRow5 = dt.NewRow();
								dataRow5.ItemArray = dataRow4.ItemArray;
								dataRow5["ID"] = (from x in dt.AsEnumerable()
									where x.RowState != DataRowState.Deleted
									select x).Max((DataRow x) => Function.ParseInt(x["ID"])) + 1;
								dataRow5["U_Status"] = CountingBinStatus.T1.ToString();
								dataRow5["Checked"] = true;
								id = Function.ParseInt(dataRow5["ID"]);
								dt.Rows.Add(dataRow5);
								dt.AcceptChanges();
							}
						}
						else
						{
							DataTable dataTable3 = await _httpClient.InventoryCountingBin.CheckBatch(text, text2, period);
							if (dataTable3.IsNotNull())
							{
								DataRow dataRow6 = dataTable3.Rows[0];
								if (Function.ToString(dataRow6["U_Status"]).Equals("##"))
								{
									if (XtraMessageBox.Show("\"Batch No\" Đã được kiểm kê trước đó. Bạn có muốn tiếp tục thêm vào danh sách?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
									{
										DataRow dataRow7 = dt.NewRow();
										dataRow7.ItemArray = dataRow6.ItemArray;
										dataRow7["ID"] = (from x in dt.AsEnumerable()
											where x.RowState != DataRowState.Deleted
											select x).Max((DataRow x) => Function.ParseInt(x["ID"])) + 1;
										dataRow7["U_Status"] = CountingBinStatus.T1.ToString();
										dataRow7["Checked"] = true;
										id = Function.ParseInt(dataRow7["ID"]);
										dt.Rows.Add(dataRow7);
										dt.AcceptChanges();
									}
								}
								else
								{
									DataRow dataRow8 = dt.NewRow();
									dataRow8.ItemArray = dataRow6.ItemArray;
									dataRow8["ID"] = (from x in dt.AsEnumerable()
										where x.RowState != DataRowState.Deleted
										select x).Max((DataRow x) => Function.ParseInt(x["ID"])) + 1;
									dataRow8["Checked"] = true;
									dataRow8["U_Status"] = CountingBinStatus.T1.ToString();
									id = Function.ParseInt(dataRow8["ID"]);
									dt.Rows.Add(dataRow8);
									dt.AcceptChanges();
								}
							}
						}
					}
				}
				else if (dt != null && dt.ExistsColumn("ID") && dt.ExistsColumn("Checked"))
				{
					DataRow dataRow9 = dt.NewRow();
					dataRow9["ID"] = (from x in dt.AsEnumerable()
						where x.RowState != DataRowState.Deleted
						select x).Max((DataRow x) => Function.ParseInt(x["ID"])) + 1;
					dataRow9["Checked"] = true;
					id = Function.ParseInt(dataRow9["ID"]);
					dt.Rows.Add(dataRow9);
					dt.AcceptChanges();
				}
				else
				{
					DataTable dataTable4 = await _httpClient.InventoryCountingBin.CheckBatch(text, text2, period);
					if (dataTable4.IsNotNull())
					{
						DataRow dataRow10 = dataTable4.Rows[0];
						if (Function.ToString(dataRow10["U_Status"]).Equals("##"))
						{
							if (XtraMessageBox.Show("\"Batch No\" Đã được kiểm kê trước đó. Bạn có muốn tiếp tục thêm vào danh sách?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
							{
								DataRow dataRow11 = dt.NewRow();
								dataRow11.ItemArray = dataRow10.ItemArray;
								dataRow11["ID"] = (from x in dt.AsEnumerable()
									where x.RowState != DataRowState.Deleted
									select x).Max((DataRow x) => Function.ParseInt(x["ID"])) + 1;
								dataRow11["U_Status"] = CountingBinStatus.T1.ToString();
								dataRow11["Checked"] = true;
								id = Function.ParseInt(dataRow11["ID"]);
								dt.Rows.Add(dataRow11);
								dt.AcceptChanges();
							}
						}
						else
						{
							dataRow10["ID"] = 1;
							dataRow10["Checked"] = true;
							id = 1;
							dataRow10["U_Status"] = CountingBinStatus.T1.ToString();
							grd_Counting.DataSource = dataTable4;
							grd_Counting.RefreshDataSource();
						}
					}
				}
				int num2 = gv_Counting.LocateByValue("ID", Function.ParseInt(id));
				if (num2 != int.MinValue)
				{
					gv_Counting.FocusedRowHandle = num2;
					gv_Counting.MakeRowVisible(num2);
				}
				txt_Batch.Text = "";
				txt_Batch.Focus();
				txt_Batch.Text = "";
				txt_Batch.Focus();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.InvalidCountingType, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void txt_Bin_KeyDown(object sender, KeyEventArgs e)
	{
		_ = 1;
		try
		{
			if (e.KeyCode == Keys.Return)
			{
				txt_Batch.Enabled = true;
				txt_Batch.Focus();
				txt_Batch.Text = string.Empty;
				if (_type == CountingBinType.Counting)
				{
					WinCommons._OpenProcessing();
					DataSet ds = await _httpClient.InventoryCountingBin.GetBinInstock(Function.ToString(bte_BIN.EditValue), Function.ToString(look_Period.EditValue));
					if (ds.IsHasValidValue())
					{
						await GridViewHelper.GenerateGrid(grd_Counting, gv_Counting, ds.Tables[0], ds.Tables[1], ds.Tables[2]);
						gv_Counting.Columns[0].Visible = false;
						if (ds.Tables[1].IsNotNull() && ds.Tables[1].ExistsColumn("DocEntry"))
						{
							int num = ds.Tables[1].AsEnumerable().Max((DataRow x) => Function.ParseInt(x["DocEntry"]));
							if (num > 0)
							{
								FormMode = Enums.FormViewModeType.Update;
								txt_DocNo.Text = num.ToString();
							}
							else
							{
								FormMode = Enums.FormViewModeType.Add;
							}
						}
					}
					else
					{
						WinCommons.Error(Enums.MessageCode.None, "Không tìm thấy dữ liệu");
					}
				}
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

	private void bte_FromBin_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
	}

	private void bte_FromBin_EditValueChanged(object sender, EventArgs e)
	{
	}

	private async void btn_Transfer_Click(object sender, EventArgs e)
	{
		_ = 2;
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
			{
				return;
			}
			btn_Transfer.Enabled = false;
			WinCommons._OpenProcessing();
			DataRow[] array = grd_Counting.DataSourceTable.Select("Checked = 'True'");
			if (array != null && array.Length != 0)
			{
				DocumentsImpl doc = new DocumentsImpl();
				DataRow[] array2 = (from t in array.AsEnumerable()
					where Function.ToString(t["U_Status"]) != "NE"
					select t).ToArray();
				if (array2.Length != 0)
				{
					DataTable source = array2.CopyToDataTable();
					DateTime dateTime = (doc.DocDate = await _httpClient.Commons.GetServerDate());
					date_Doc.EditValue = dateTime;
					doc.TaxDate = dateTime;
					doc.DocDueDate = dateTime;
					doc.Lines = new List<Document_LinesImpl>();
					doc.BinAllocations = new List<DocumentLinesBinAllocationsImpl>();
					var enumerable = from t in source.AsEnumerable()
						group t by new
						{
							ItemCode = Function.ToString(t["U_ItemCode"]),
							Whs = Function.ToString(t["U_Whs"]),
							ToWhs = Function.ToString(t["U_ToWhs"])
						} into g
						select new
						{
							Key = g.Key,
							Items = g
						};
					int num = 0;
					foreach (var item in enumerable)
					{
						Document_LinesImpl document_LinesImpl = new Document_LinesImpl();
						document_LinesImpl.ItemCode = Function.ToString(item.Key.ItemCode);
						document_LinesImpl.FromWhsCod = Function.ToString(item.Key.Whs).ToUpper();
						if (string.IsNullOrEmpty(doc.Filler))
						{
							doc.Filler = document_LinesImpl.FromWhsCod;
						}
						document_LinesImpl.WhsCode = Function.ToString(item.Key.ToWhs).ToUpper();
						if (string.IsNullOrEmpty(doc.ToWhsCode))
						{
							doc.ToWhsCode = document_LinesImpl.WhsCode;
						}
						int num2 = 0;
						double num3 = 0.0;
						document_LinesImpl.BatchNumbers = new List<BatchNumbersImpl>();
						document_LinesImpl.BinAllocations = new List<DocumentLinesBinAllocationsImpl>();
						foreach (DataRow item2 in item.Items)
						{
							if (Function.ToString(item2["U_Status"]).Equals("NE"))
							{
								continue;
							}
							int num4 = Function.ParseInt(item2["U_BinId"]);
							int num5 = Function.ParseInt(item2["U_ToBinId"]);
							if ((num4 == 0 || num5 == 0 || !num4.Equals(num5)) && (num4 != 0 || num5 != 0 || !document_LinesImpl.FromWhsCod.Equals(doc.ToWhsCode)))
							{
								double num6 = Function.ParseDouble(item2["U_Qty"]);
								num3 += num6;
								BatchNumbersImpl batchNumbersImpl = new BatchNumbersImpl();
								batchNumbersImpl.BatchNumber = Function.ToString(item2["U_DistNumber"]);
								batchNumbersImpl.Quantity = num6;
								batchNumbersImpl.BaseLineNumber = num;
								document_LinesImpl.BatchNumbers.Add(batchNumbersImpl);
								DocumentLinesBinAllocationsImpl documentLinesBinAllocationsImpl = new DocumentLinesBinAllocationsImpl();
								documentLinesBinAllocationsImpl.BinAbsEntry = Function.ParseInt(item2["U_BinId"]);
								documentLinesBinAllocationsImpl.BaseLineNumber = num;
								documentLinesBinAllocationsImpl.BinActionType = BinActionTypeEnum.batFromWarehouse;
								documentLinesBinAllocationsImpl.Quantity = num6;
								documentLinesBinAllocationsImpl.SerialAndBatchNumbersBaseLine = num2;
								if (documentLinesBinAllocationsImpl.BinAbsEntry > 0)
								{
									document_LinesImpl.BinAllocations.Add(documentLinesBinAllocationsImpl);
								}
								DocumentLinesBinAllocationsImpl documentLinesBinAllocationsImpl2 = new DocumentLinesBinAllocationsImpl();
								documentLinesBinAllocationsImpl2.BinAbsEntry = Function.ParseInt(item2["U_ToBinId"]);
								documentLinesBinAllocationsImpl2.BaseLineNumber = num;
								documentLinesBinAllocationsImpl2.BinActionType = BinActionTypeEnum.batToWarehouse;
								documentLinesBinAllocationsImpl2.Quantity = num6;
								documentLinesBinAllocationsImpl2.SerialAndBatchNumbersBaseLine = num2;
								if (documentLinesBinAllocationsImpl2.BinAbsEntry > 0)
								{
									document_LinesImpl.BinAllocations.Add(documentLinesBinAllocationsImpl2);
								}
								num2++;
							}
						}
						document_LinesImpl.Quantity = num3;
						document_LinesImpl.LineNum = num;
						doc.Lines.Add(document_LinesImpl);
						num++;
					}
					DataSet ds = new DataSet();
					if (doc.Lines.Count == 0 && !ds.IsNotNull())
					{
						WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
						grd_Counting.DataSource = null;
						grd_Counting.RefreshDataSource();
						Logging.Write("WATCH", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Không có dữ liệu chuyển BIN");
					}
					else
					{
						HttpResult check = await _httpClient.StockTransfer.SaveTransferBin(doc, ds);
						if (check.Content == null)
						{
							XtraMessageBox.Show(check.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
						}
						else
						{
							WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
							grd_Counting.DataSource = null;
							grd_Counting.RefreshDataSource();
							await WinFuntions.PrintLayout("", Function.ParseInt(check.Content), "OINC");
							txt_DocNo.Text = (Function.ParseInt(check.Content) + 1).ToString();
						}
					}
				}
			}
			else
			{
				XtraMessageBox.Show("Chọn dòng để chuyển bin", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
		btn_Transfer.Enabled = true;
		bte_BIN.Text = "";
		bte_BIN.Focus();
	}

	private void bte_FromBin_KeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.Return)
			{
				bte_BIN.Text = "";
				bte_BIN.Focus();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void bte_BIN_EditValueChanging(object sender, ChangingEventArgs e)
	{
	}

	private void simpleButton2_Click(object sender, EventArgs e)
	{
		try
		{
			grd_Counting.DataSource = null;
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

	private void bte_BIN_EditValueChanged(object sender, EventArgs e)
	{
	}

	private void gv_Counting_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (_type == CountingBinType.Transfer)
			{
				GridColumn focusedColumn = gv_Counting.FocusedColumn;
				string text = Function.ToString(gv_Counting.GetFocusedRowCellValue("U_Status"));
				if (!focusedColumn.FieldName.Equals("U_Remark"))
				{
					if (focusedColumn.FieldName.Equals("U_Status"))
					{
						e.Cancel = text.Equals("NE");
					}
					else
					{
						e.Cancel = !text.Equals("NE");
					}
				}
				return;
			}
			GridColumn focusedColumn2 = gv_Counting.FocusedColumn;
			string text2 = Function.ToString(gv_Counting.GetFocusedRowCellValue("U_Status"));
			if (!focusedColumn2.FieldName.Equals("U_Remark"))
			{
				if (focusedColumn2.FieldName.Equals("U_Status"))
				{
					e.Cancel = text2.Equals("T1");
				}
				else
				{
					e.Cancel = !text2.Equals("T1");
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Counting_RowStyle(object sender, RowStyleEventArgs e)
	{
		try
		{
			if (Function.ToString(gv_Counting.GetRowCellValue(e.RowHandle, "U_Status")).Equals("NE"))
			{
				e.Appearance.BackColor = Color.Tomato;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void rdo_CountinBin_EditValueChanging(object sender, ChangingEventArgs e)
	{
		_ = 1;
		try
		{
			if (grd_Counting.DataSourceTable.IsNotNull() && ((_type == CountingBinType.Counting && Function.ParseInt(e.NewValue) == 0) || (_type == CountingBinType.Transfer && Function.ParseInt(e.NewValue) == 1)) && _type == CountingBinType.Transfer && Function.ParseInt(e.NewValue) == 1 && grd_Counting.DataSource != null && XtraMessageBox.Show("Chuyển chức năng sẽ mất thông tin hiện tại. Tiếp tục?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
			{
				e.Cancel = true;
				return;
			}
			grd_Counting.DataSource = null;
			grd_Counting.RefreshDataSource();
			gv_Counting.Columns.Clear();
			_type = ((_type == CountingBinType.Transfer) ? CountingBinType.Counting : CountingBinType.Transfer);
			btn_Transfer.Visible = _type == CountingBinType.Transfer;
			LabelLine labelLine = lb_Period;
			bool visible = (look_Period.Visible = _type == CountingBinType.Counting);
			labelLine.Visible = visible;
			gv_Counting.OptionsSelection.MultiSelectMode = ((_type == CountingBinType.Transfer) ? GridMultiSelectMode.CheckBoxRowSelect : GridMultiSelectMode.RowSelect);
			if (_type == CountingBinType.Transfer)
			{
				bte_BIN.Enabled = true;
				int num = await _httpClient.Udo.GetMaxId("OINC");
				txt_DocNo.Text = Function.ToString(num + 1);
			}
			else
			{
				bte_BIN.Enabled = !string.IsNullOrEmpty(Function.ToString(look_Period.EditValue));
				int num2 = await _httpClient.Udo.GetMaxId("OINB");
				txt_DocNo.Text = Function.ToString(num2 + 1);
			}
			bte_BIN.Text = "";
			bte_BIN.Focus();
			txt_Batch.Text = "";
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
			bte_BIN.Text = "";
			txt_Batch.Text = "";
			txt_Batch.Enabled = false;
			btn_Print.Enabled = false;
			btn_OK.Text = Resources.ButtonOK;
			bte_BIN.Focus();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void txt_Batch_EditValueChanged(object sender, EventArgs e)
	{
	}

	private void look_Period_EditValueChanged(object sender, EventArgs e)
	{
		bte_BIN.Enabled = true;
		bte_BIN.Text = "";
		bte_BIN.Focus();
		txt_Batch.Text = "";
	}

	private async void btn_OK_Click(object sender, EventArgs e)
	{
		try
		{
			if (btn_OK.Text.Equals(Resources.ButtonOK) || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
			{
				return;
			}
			WinCommons._OpenProcessing();
			DataTable dataSourceTable = grd_Counting.DataSourceTable;
			DataSet dataSet = new DataSet();
			if (dataSourceTable.IsNotNull())
			{
				if (UdoTableCounting == null || UdoTableCounting.Rows.Count == 0)
				{
					WinCommons.Error(Enums.MessageCode.UdoTableIsInvalid, "");
					return;
				}
				string tableName = Function.ToString(UdoTableCounting.Rows[0]["TableName"]);
				dataSet.Tables.Add(UdoTableCounting.Copy());
				DataTable dataTable = new DataTable(tableName);
				dataTable.Columns.Add("DocEntry", typeof(int));
				dataTable.Columns.Add("Remark", typeof(string));
				dataTable.Columns.Add("U_BinCode", typeof(string));
				dataTable.Columns.Add("U_DocDate", typeof(DateTime));
				dataTable.Columns.Add("U_CountPeriod", typeof(string));
				DataRow dataRow = dataTable.NewRow();
				dataRow["DocEntry"] = 0;
				dataRow["U_CountPeriod"] = Function.ToString(look_Period.EditValue);
				dataRow["U_BinCode"] = Function.ToString(bte_BIN.EditValue);
				dataRow["U_DocDate"] = ((!Function.ParseDateTime(date_Doc.EditValue).HasValue) ? GetDate : Convert.ToDateTime(date_Doc.EditValue));
				dataTable.Rows.Add(dataRow);
				dataSet.Tables.Add(dataTable);
				DataTable dataTable2 = dataSourceTable.Copy();
				dataTable2.TableName = "INB1";
				if (dataTable2.IsNotNull() && dataTable2.ExistsColumn("DocEntry"))
				{
					int num = (from x in dataTable2.AsEnumerable()
						where x.RowState != DataRowState.Deleted
						select x).Max((DataRow x) => Function.ParseInt(x["DocEntry"]));
					dataTable.Rows[0]["DocEntry"] = num;
				}
				foreach (string item in (from DataColumn t in dataTable2.Columns
					where !t.ColumnName.StartsWith("U_") && !t.ColumnName.Equals("DocEntry") && !t.ColumnName.Equals("LineId")
					select t.ColumnName).ToList())
				{
					dataTable2.Columns.Remove(item);
				}
				dataTable2.TableName = "INB1";
				dataSet.Tables.Add(dataTable2.Copy());
				HttpResult httpResult = await _httpClient.Udo.Save(dataSet);
				if (!string.IsNullOrEmpty(httpResult.Message))
				{
					XtraMessageBox.Show(httpResult.Message, "Error", MessageBoxButtons.OK);
				}
				else
				{
					bte_BIN.Text = "";
					bte_BIN.Focus();
					txt_Batch.Text = "";
					txt_Batch.Enabled = false;
					grd_Counting.DataSource = null;
					gv_Counting.Columns.Clear();
					WinCommons.Success(Enums.MessageCode.Success, "");
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_NEXT()
	{
		try
		{
			int type = 3;
			int currentId = Function.ParseInt(txt_DocNo.EditValue);
			await Navigate(currentId, type);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task MOVE_FIRST()
	{
		try
		{
			int type = 1;
			int currentId = Function.ParseInt(txt_DocNo.EditValue);
			await Navigate(currentId, type);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task MOVE_PREVIOUS()
	{
		try
		{
			int type = 2;
			int currentId = Function.ParseInt(txt_DocNo.EditValue);
			await Navigate(currentId, type);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task MOVE_LAST()
	{
		try
		{
			int type = 4;
			int currentId = Function.ParseInt(txt_DocNo.EditValue);
			await Navigate(currentId, type);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task Navigate(int currentId, int type)
	{
		if (_type == CountingBinType.Transfer)
		{
			return;
		}
		WinCommons._OpenProcessing();
		try
		{
			int num = await GetPage(currentId, type);
			DataTable dataTable = await _httpClient.Udo.GetByDocEntry(new Common.Table_Code
			{
				Code = num.ToString(),
				Name = UdoTableNameCounting
			});
			if (dataTable.IsNotNull())
			{
				txt_DocNo.EditValue = Function.ToString(dataTable.Rows[0]["DocEntry"]);
				look_Period.EditValue = Function.ToString(dataTable.Rows[0]["U_CountPeriod"]);
				date_Doc.EditValue = Function.ToString(dataTable.Rows[0]["U_DocDate"]);
				bte_BIN.EditValue = Function.ToString(dataTable.Rows[0]["U_BinCode"]);
				btn_OK.Text = Resources.ButtonOK;
				DataSet dataSet = await GetCountingBin();
				if (dataSet.IsHasValidValue())
				{
					if (grd_Counting.DataSourceTable.Rows.Count <= 0)
					{
						await GridViewHelper.GenerateGrid(grd_Counting, gv_Counting, dataSet.Tables[0], dataSet.Tables[1], dataSet.Tables[2], null, expand: true);
					}
					else
					{
						grd_Counting.DataSource = dataSet.Tables[1];
						grd_Counting.RefreshDataSource();
					}
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, "Không tìm thấy dữ liệu");
				}
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private async Task<DataSet> GetCountingBin()
	{
		try
		{
			int id = Function.ParseInt(txt_DocNo.EditValue);
			return await _httpClient.InventoryCountingBin.GetById(id);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataSet();
	}

	private async Task<int> GetPage(int current, int type)
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "InventoryCountingBin");
			return await helper.GetIntAsync("GetPage", new { current, type });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return 0;
	}

	public async void PRINTLAYOUT()
	{
		try
		{
			int num = Function.ParseInt(txt_DocNo.Text) - 1;
			await WinFuntions.PrintLayout("", num, "OINC");
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
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Inventory.InventoryTransaction.frm_InventoryCountingBin));
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.txt_DocNo = new Apzon.Controls.Editor.TextEditEx();
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
		this.btn_Print = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Transfer = new Apzon.Controls.Controls.SimpleButtonEx();
		this.bte_BIN = new Apzon.Controls.Editor.ButtonEditEx();
		this.simpleButton2 = new Apzon.Controls.Controls.SimpleButtonEx();
		this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
		this.rdo_CountinBin = new Apzon.Controls.Editor.RadioGroupEx();
		this.lb_Period = new Apzon.Controls.Controls.LabelLine();
		this.look_Period = new DevExpress.XtraEditors.LookUpEdit();
		((System.ComponentModel.ISupportInitialize)this.txt_DocNo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_User.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_Doc.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_Doc.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Batch.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Counting).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Counting).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bte_BIN.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rdo_CountinBin.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.look_Period.Properties).BeginInit();
		base.SuspendLayout();
		this.labelLine1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Doc No.";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(647, 12);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(80, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 0;
		this.labelLine1.Utext = "Doc No.";
		this.txt_DocNo.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txt_DocNo.EditControl = null;
		this.txt_DocNo.FieldName = null;
		this.txt_DocNo.IsSearchControl = false;
		this.txt_DocNo.Location = new System.Drawing.Point(726, 10);
		this.txt_DocNo.Name = "txt_DocNo";
		this.txt_DocNo.SettingInVisible = false;
		this.txt_DocNo.SettingReadonly = false;
		this.txt_DocNo.Size = new System.Drawing.Size(142, 20);
		this.txt_DocNo.TabIndex = 1;
		this.txt_User.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txt_User.EditControl = null;
		this.txt_User.FieldName = null;
		this.txt_User.IsSearchControl = false;
		this.txt_User.Location = new System.Drawing.Point(726, 34);
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
		this.labelLine2.Location = new System.Drawing.Point(647, 36);
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
		this.labelLine3.Location = new System.Drawing.Point(647, 63);
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
		this.date_Doc.Enabled = false;
		this.date_Doc.FieldName = null;
		this.date_Doc.Location = new System.Drawing.Point(726, 60);
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
		this.BIN.EditValue = "BIN";
		this.BIN.FieldName = null;
		this.BIN.HideWhenEmpty = false;
		this.BIN.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.BIN.Location = new System.Drawing.Point(12, 34);
		this.BIN.Name = "BIN";
		this.BIN.ReadOnly = false;
		this.BIN.SettingInVisible = false;
		this.BIN.SettingReadonly = false;
		this.BIN.ShowUnderLine = true;
		this.BIN.Size = new System.Drawing.Size(105, 20);
		this.BIN.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.BIN.TabIndex = 9;
		this.BIN.Utext = "BIN";
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "Batch (Mã hàng)";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(12, 60);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(105, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 11;
		this.labelLine4.Utext = "Batch (Mã hàng)";
		this.txt_Batch.EditControl = null;
		this.txt_Batch.FieldName = null;
		this.txt_Batch.IsSearchControl = false;
		this.txt_Batch.Location = new System.Drawing.Point(117, 58);
		this.txt_Batch.Name = "txt_Batch";
		this.txt_Batch.SettingInVisible = false;
		this.txt_Batch.SettingReadonly = false;
		this.txt_Batch.Size = new System.Drawing.Size(241, 20);
		this.txt_Batch.TabIndex = 10;
		this.txt_Batch.EditValueChanged += new System.EventHandler(this.txt_Batch_EditValueChanged);
		this.txt_Batch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_Batch_KeyDown);
		this.grd_Counting.AllowPasteExcel = true;
		this.grd_Counting.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Counting.ClipboardData = null;
		this.grd_Counting.Location = new System.Drawing.Point(0, 89);
		this.grd_Counting.MainView = this.gv_Counting;
		this.grd_Counting.Name = "grd_Counting";
		this.grd_Counting.Size = new System.Drawing.Size(876, 341);
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
		this.gv_Counting.FormId = "oInventoryCoutingBin";
		this.gv_Counting.GridControl = this.grd_Counting;
		this.gv_Counting.IgnoreFilter = false;
		this.gv_Counting.IndexEditRow = -1;
		this.gv_Counting.IndicatorWidth = 35;
		this.gv_Counting.IsApzTable = false;
		this.gv_Counting.IsInsertNewRow = false;
		this.gv_Counting.IsLoadingData = false;
		this.gv_Counting.IsMappingContextAction = false;
		this.gv_Counting.ItemId = "OICB";
		this.gv_Counting.LstColumnAllowBatch = null;
		this.gv_Counting.LstColumnNotAllowBatch = null;
		this.gv_Counting.LstColumnNotEditable = null;
		this.gv_Counting.LstColumNotSetting = null;
		this.gv_Counting.Name = "gv_Counting";
		this.gv_Counting.OptionsView.ShowGroupPanel = false;
		this.gv_Counting.UseBatchMod = false;
		this.gv_Counting.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.gv_Counting_RowStyle);
		this.gv_Counting.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(this.gv_Counting_PopupMenuShowing);
		this.gv_Counting.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_Counting_ShowingEditor);
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.EditControl = null;
		this.btn_OK.EditValue = "OK";
		this.btn_OK.FieldName = null;
		this.btn_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_OK.ImageOptions.Image");
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(1, 437);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.ReadOnly = false;
		this.btn_OK.SettingInVisible = false;
		this.btn_OK.SettingReadonly = false;
		this.btn_OK.Size = new System.Drawing.Size(75, 20);
		this.btn_OK.TabIndex = 13;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.btn_Print.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Print.EditControl = null;
		this.btn_Print.EditValue = "Print";
		this.btn_Print.Enabled = false;
		this.btn_Print.FieldName = null;
		this.btn_Print.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Print.ImageOptions.Image");
		this.btn_Print.IsSearchControl = false;
		this.btn_Print.Location = new System.Drawing.Point(78, 437);
		this.btn_Print.Name = "btn_Print";
		this.btn_Print.ReadOnly = false;
		this.btn_Print.SettingInVisible = false;
		this.btn_Print.SettingReadonly = false;
		this.btn_Print.Size = new System.Drawing.Size(75, 20);
		this.btn_Print.TabIndex = 14;
		this.btn_Print.Text = "Print";
		this.btn_Transfer.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Transfer.EditControl = null;
		this.btn_Transfer.EditValue = "Transfer";
		this.btn_Transfer.FieldName = null;
		this.btn_Transfer.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Transfer.ImageOptions.Image");
		this.btn_Transfer.IsSearchControl = false;
		this.btn_Transfer.Location = new System.Drawing.Point(801, 436);
		this.btn_Transfer.Name = "btn_Transfer";
		this.btn_Transfer.ReadOnly = false;
		this.btn_Transfer.SettingInVisible = false;
		this.btn_Transfer.SettingReadonly = false;
		this.btn_Transfer.Size = new System.Drawing.Size(75, 20);
		this.btn_Transfer.TabIndex = 15;
		this.btn_Transfer.Text = "Transfer";
		this.btn_Transfer.Click += new System.EventHandler(this.btn_Transfer_Click);
		this.bte_BIN.EditControl = null;
		this.bte_BIN.FieldName = null;
		this.bte_BIN.IsSearchControl = false;
		this.bte_BIN.Location = new System.Drawing.Point(117, 33);
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
		this.bte_BIN.EditValueChanged += new System.EventHandler(this.bte_BIN_EditValueChanged);
		this.bte_BIN.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.bte_BIN_EditValueChanging);
		this.bte_BIN.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_Bin_KeyDown);
		this.simpleButton2.EditControl = null;
		this.simpleButton2.EditValue = "Clear";
		this.simpleButton2.FieldName = null;
		this.simpleButton2.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("simpleButton2.ImageOptions.Image");
		this.simpleButton2.IsSearchControl = false;
		this.simpleButton2.Location = new System.Drawing.Point(364, 58);
		this.simpleButton2.Name = "simpleButton2";
		this.simpleButton2.ReadOnly = false;
		this.simpleButton2.SettingInVisible = false;
		this.simpleButton2.SettingReadonly = false;
		this.simpleButton2.Size = new System.Drawing.Size(75, 20);
		this.simpleButton2.TabIndex = 19;
		this.simpleButton2.Text = "Clear";
		this.simpleButton2.Click += new System.EventHandler(this.simpleButton2_Click);
		this.rdo_CountinBin.EditControl = null;
		this.rdo_CountinBin.EditValue = "0";
		this.rdo_CountinBin.FieldName = null;
		this.rdo_CountinBin.IsSearchControl = false;
		this.rdo_CountinBin.Location = new System.Drawing.Point(7, 2);
		this.rdo_CountinBin.Name = "rdo_CountinBin";
		this.rdo_CountinBin.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.rdo_CountinBin.Properties.Appearance.Options.UseBackColor = true;
		this.rdo_CountinBin.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.rdo_CountinBin.Properties.Columns = 2;
		this.rdo_CountinBin.Properties.GlyphAlignment = DevExpress.Utils.HorzAlignment.Default;
		this.rdo_CountinBin.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[2]
		{
			new DevExpress.XtraEditors.Controls.RadioGroupItem("0", "Chuyển Bin"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem("1", "Kiểm kê")
		});
		this.rdo_CountinBin.SettingInVisible = false;
		this.rdo_CountinBin.SettingReadonly = false;
		this.rdo_CountinBin.Size = new System.Drawing.Size(169, 30);
		this.rdo_CountinBin.TabIndex = 24;
		this.rdo_CountinBin.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.rdo_CountinBin_EditValueChanging);
		this.lb_Period.EditControl = null;
		this.lb_Period.EditValue = "Kỳ kiểm kê";
		this.lb_Period.FieldName = null;
		this.lb_Period.HideWhenEmpty = false;
		this.lb_Period.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Period.Location = new System.Drawing.Point(160, 7);
		this.lb_Period.Name = "lb_Period";
		this.lb_Period.ReadOnly = false;
		this.lb_Period.SettingInVisible = false;
		this.lb_Period.SettingReadonly = false;
		this.lb_Period.ShowUnderLine = true;
		this.lb_Period.Size = new System.Drawing.Size(80, 20);
		this.lb_Period.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Period.TabIndex = 25;
		this.lb_Period.Utext = "Kỳ kiểm kê";
		this.lb_Period.Visible = false;
		this.look_Period.Location = new System.Drawing.Point(236, 4);
		this.look_Period.Name = "look_Period";
		this.look_Period.Properties.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
		this.look_Period.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.look_Period.Properties.NullText = "";
		this.look_Period.Size = new System.Drawing.Size(122, 20);
		this.look_Period.TabIndex = 26;
		this.look_Period.Visible = false;
		this.look_Period.EditValueChanged += new System.EventHandler(this.look_Period_EditValueChanged);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(877, 460);
		base.Controls.Add(this.look_Period);
		base.Controls.Add(this.lb_Period);
		base.Controls.Add(this.rdo_CountinBin);
		base.Controls.Add(this.simpleButton2);
		base.Controls.Add(this.bte_BIN);
		base.Controls.Add(this.btn_Transfer);
		base.Controls.Add(this.btn_Print);
		base.Controls.Add(this.btn_OK);
		base.Controls.Add(this.grd_Counting);
		base.Controls.Add(this.labelLine4);
		base.Controls.Add(this.txt_Batch);
		base.Controls.Add(this.BIN);
		base.Controls.Add(this.date_Doc);
		base.Controls.Add(this.labelLine3);
		base.Controls.Add(this.txt_User);
		base.Controls.Add(this.labelLine2);
		base.Controls.Add(this.txt_DocNo);
		base.Controls.Add(this.labelLine1);
		base.FormId = "oInventoryCoutingBin";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_InventoryCountingBin";
		base.ShowInTaskbar = false;
		this.Text = "Inventory Counting Bin";
		base.Load += new System.EventHandler(this.frm_InventoryCountingBin_Load);
		((System.ComponentModel.ISupportInitialize)this.txt_DocNo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_User.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_Doc.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_Doc.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Batch.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Counting).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Counting).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bte_BIN.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rdo_CountinBin.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.look_Period.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
