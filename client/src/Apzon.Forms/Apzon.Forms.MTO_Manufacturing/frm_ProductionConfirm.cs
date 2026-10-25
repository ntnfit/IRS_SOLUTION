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
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Menu;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraTab;

namespace Apzon.Forms.MTO_Manufacturing;

public class frm_ProductionConfirm : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private const string FormController = "ProductionConfirm";

	private int poRelease;

	private string issueBatch = "";

	private List<UserFieldsImpl> lstUdf = new List<UserFieldsImpl>();

	private int printDoc;

	private string receiptDoc = "";

	private bool isLoadForm = true;

	private int suffix;

	private DataTable dtSearch;

	private IContainer components;

	private PanelControl panelControl1;

	private PanelControl panelControl2;

	private TextEditEx txt_Barcode;

	private LabelLine labelLine1;

	private SimpleButtonEx btn_Confirm;

	private XtraTabControlEx xtraTabControl1;

	private XtraTabPageEx tab_ProductionControl;

	private GridControlEx grd_ProductionControl;

	private LabelLine labelLine3;

	private DateEditEx date_ToDate;

	private LabelLine labelLine2;

	private SimpleButtonEx btn_UnRelease;

	private SimpleButtonEx btn_Cancel;

	private SplitContainerControl splitContainerControl1;

	private XtraTabControlEx xtraTabControl2;

	private XtraTabPageEx xtraTabPage1;

	private GridControlEx grd_ReceiptBatch;

	private SimpleButtonEx btn_Pending;

	private SimpleButtonEx btn_Refresh;

	private Timer timer_LoadForm;

	private SpinEdit spin_Timer;

	private CheckEditEx chk_Timer;

	private ToolTipController ttc_ProductionControl;

	private GridViewEx gv_ProductionControl;

	private GridViewEx gv_ReceiptBatch;

	private LabelLine lb_BusinessPartner;

	private CheckedComboBoxEdit chb_Status;

	private SimpleButtonEx btn_ClosePO;

	private ButtonEditEx bte_WorkCenter;

	private LabelLine labelLine4;

	private MemoExEdit txt_IssueBatch;

	private SimpleButtonEx btn_CheckAll;

	private LabelLine labelLine5;

	private DateEditEx date_FromDate;

	public frm_ProductionConfirm()
	{
		InitializeComponent();
	}

	private async void frm_ProductionConfirm_Load(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			isLoadForm = true;
			btn_Confirm.Enabled = false;
			btn_UnRelease.Enabled = false;
			btn_Pending.Enabled = false;
			bte_WorkCenter.Text = GlobalData.UserName;
			date_ToDate.EditValue = DateTime.Today;
			date_FromDate.EditValue = DateTime.Today;
			chb_Status.Properties.Items.Clear();
			chb_Status.Properties.Items.Add("P", "Planned");
			chb_Status.Properties.Items.Add("R", "Released");
			chb_Status.Properties.Items.Add("PD", "Pending");
			chb_Status.Properties.Items.Add("L", "Closed");
			chb_Status.Properties.Items.Add("C", "Canceled");
			chb_Status.SetEditValue("P,R,PD");
			timer_LoadForm.Enabled = chk_Timer.Checked;
			timer_LoadForm.Interval = Function.ParseInt(spin_Timer.EditValue) * 60000;
			isLoadForm = false;
			lstUdf = await _httpClient.Udf.GetListField("OBTN");
			await LoadProductionExecuting();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void txt_Barcode_KeyDown(object sender, KeyEventArgs e)
	{
		_ = 3;
		try
		{
			if (e.KeyCode == Keys.Return && !string.IsNullOrEmpty(txt_Barcode.Text))
			{
				WinCommons._OpenProcessing();
				EnableForm(enable: false);
				string batch = Function.ToString(txt_Barcode.Text);
				DataTable dt = (DataTable)grd_ProductionControl.DataSource;
				if (poRelease > 0)
				{
					DataRow[] array = dt.Select($"U_PwNo = '{poRelease}'");
					if (array != null && array.Length != 0)
					{
						DataRow[] array2 = array;
						foreach (DataRow dataRow in array2)
						{
							if (Function.ToString(dataRow["U_BatchNo"]).Split(';').Any((string m) => m.Equals(batch)) && !issueBatch.Split(';').Any((string m) => m.Equals(batch)) && !string.IsNullOrEmpty(issueBatch))
							{
								batch = $"{issueBatch};{batch}";
								issueBatch = batch;
								txt_IssueBatch.Text = batch;
								txt_Barcode.Text = "";
								txt_Barcode.Focus();
								gv_ProductionControl.FocusedRowHandle = dt.Rows.IndexOf(dataRow);
								WinCommons._CloseProcessing();
								EnableForm(enable: true);
								return;
							}
						}
					}
				}
				foreach (DataRow arrayRow in dt.Rows)
				{
					if (!Function.ToString(arrayRow["U_BatchNo"]).Split(';').Any((string m) => m.Equals(batch)))
					{
						continue;
					}
					string text = Function.ToString(arrayRow["WhsCode"]);
					string value = Function.ToString(arrayRow["U_WhsCode"]);
					if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(value) && !text.Equals(value))
					{
						WinCommons._CloseProcessing();
						EnableForm(enable: true);
						XtraMessageBox.Show("Kho hiện tại không đúng với kho sản xuất. Yêu cầu BP kho xuất hàng!", "System", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
						return;
					}
					int docEntry = Function.ParseInt(arrayRow["U_PwNo"]);
					string text2 = Function.ToString(arrayRow["Status"]);
					string text3 = "";
					if (docEntry > 0 && (text2.Equals("P") || text2.Equals("R") || text2.Equals("PD")))
					{
						if (text2.Equals("PD"))
						{
							poRelease = docEntry;
							issueBatch = batch;
							text3 = await UpdateStatusPO(poRelease, "ND");
						}
						else if (text2.Equals("P"))
						{
							text3 = await UpdateStatusPO(docEntry, "R");
						}
						if (string.IsNullOrEmpty(text3))
						{
							poRelease = docEntry;
							issueBatch = batch;
							txt_IssueBatch.Text = batch;
							btn_Confirm.Enabled = true;
							btn_UnRelease.Enabled = true;
							btn_Pending.Enabled = true;
							arrayRow["Status"] = "R";
							txt_Barcode.Text = "";
							txt_Barcode.Focus();
							gv_ProductionControl.FocusedRowHandle = dt.Rows.IndexOf(arrayRow);
						}
						else
						{
							WinCommons.Error(Enums.MessageCode.None, text3);
						}
					}
					WinCommons._CloseProcessing();
					EnableForm(enable: true);
					return;
				}
				string text4 = await GetWorkCenter(batch);
				WinCommons._CloseProcessing();
				if (!string.IsNullOrEmpty(text4) && text4.Equals(bte_WorkCenter.Text))
				{
					XtraMessageBox.Show($"Cuộn hàng phải sản xuất ở máy hiện tại: {text4}. Kiểm tra lại tiêu chí tìm kiếm!, ", "System", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
				}
				else if (!string.IsNullOrEmpty(text4))
				{
					switch (XtraMessageBox.Show(string.Format("Cuộn hàng phải sản xuất ở máy: {0}. Nhấn \"Yes\" để thực hiện lệnh, Nhấn \"No\" để chuyển sang màn hình máy {0}, Nhấn \"Cancel\" để hủy!, ", text4), "System", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button3))
					{
					case DialogResult.Yes:
						isLoadForm = true;
						bte_WorkCenter.Text = text4;
						isLoadForm = false;
						WinCommons._OpenProcessing();
						await LoadProductionExecuting();
						txt_Barcode.Text = batch;
						WinCommons._CloseProcessing();
						txt_Barcode_KeyDown(sender, e);
						break;
					case DialogResult.No:
						bte_WorkCenter.Text = text4;
						break;
					}
				}
				else
				{
					XtraMessageBox.Show("Không tìm thấy cuộn hàng phù hợp!", "System", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
				}
				txt_Barcode.Text = "";
				txt_Barcode.Focus();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		EnableForm(enable: true);
	}

	private void gv_ProductionControl_RowStyle(object sender, RowStyleEventArgs e)
	{
		try
		{
			if (e.RowHandle > -1 && gv_ProductionControl.IsExistColumn("Status") && gv_ProductionControl.IsExistColumn("U_PwNo"))
			{
				string text = gv_ProductionControl.GetRowCellValue(e.RowHandle, "Status").ToString();
				int num = Function.ParseInt(gv_ProductionControl.GetRowCellValue(e.RowHandle, "U_PwNo"));
				if (num > 0 && poRelease == num)
				{
					e.Appearance.BackColor = Color.Orange;
				}
				if (text.Equals("L"))
				{
					e.Appearance.BackColor = Color.Gray;
				}
				else if (text.Equals("PD"))
				{
					e.Appearance.BackColor = Color.Aquamarine;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_UnRelease_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (poRelease > 0)
			{
				WinCommons._OpenProcessing();
				string text = await UpdateStatusPO(poRelease, "P");
				if (string.IsNullOrEmpty(text))
				{
					poRelease = 0;
					btn_Confirm.Enabled = false;
					btn_UnRelease.Enabled = false;
					btn_Pending.Enabled = false;
					txt_IssueBatch.Text = "";
					await LoadProductionExecuting();
					WinCommons.writeStatus(Enums.MessageCode.SaveSuccess, Enums.MessageType.Success, Msg_Time._TimeMedium, "");
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, text);
				}
				WinCommons._CloseProcessing();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Confirm_Click(object sender, EventArgs e)
	{
		_ = 3;
		try
		{
			DataTable dataSourceTable = grd_ProductionControl.DataSourceTable;
			if (!dataSourceTable.IsNotNull())
			{
				XtraMessageBox.Show("Không có thông tin lệnh sản xuất!", "System", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
			}
			else
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
				{
					return;
				}
				WinCommons._OpenProcessing();
				EnableForm(enable: false);
				if (poRelease > 0)
				{
					DataRow dataRow = dataSourceTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["U_PwNo"]) == poRelease && "R".Equals(Function.ToString(t["Status"])));
					if (dataRow == null)
					{
						return;
					}
					double num = Math.Round(Function.ParseDouble(dataRow["U_Quantity"]), 3, MidpointRounding.AwayFromZero);
					double num2 = Function.ParseDouble(dataRow["CmpltQty"]);
					string item = Function.ToString(dataRow["Remark"]);
					DocumentsImpl documentsImpl = new DocumentsImpl();
					documentsImpl.BPLId = Function.ParseInt(GlobalData.DefaultBranch);
					documentsImpl.Ref2 = poRelease.ToString();
					documentsImpl.Comments = poRelease.ToString();
					documentsImpl.Lines = new List<Document_LinesImpl>();
					Document_LinesImpl document_LinesImpl = new Document_LinesImpl();
					document_LinesImpl.ItemCode = Function.ToString(dataRow["U_ItemCode"]);
					document_LinesImpl.Quantity = Math.Round(Function.ParseDouble(dataRow["U_Width"]) * Function.ParseDouble(dataRow["U_Length"]) * Function.ParseDouble(dataRow["U_QtyUoM"]) / 100.0, 3, MidpointRounding.AwayFromZero);
					document_LinesImpl.WhsCode = Function.ToString(dataRow["U_WhsCode"]);
					document_LinesImpl.BatchNumbers = new List<BatchNumbersImpl>();
					DataTable dtReceipt = grd_ReceiptBatch.DataSourceTable;
					DocumentsImpl documentsImpl2 = new DocumentsImpl();
					documentsImpl2.BPLId = Function.ParseInt(GlobalData.DefaultBranch);
					documentsImpl2.Ref2 = poRelease.ToString();
					documentsImpl2.Comments = poRelease.ToString();
					if (!dtReceipt.IsNotNull())
					{
						EnableForm(enable: true);
						WinCommons._CloseProcessing();
						return;
					}
					if (Function.ParseInt(dtReceipt.Rows[0]["U_PwNo"]) != poRelease)
					{
						WinCommons._CloseProcessing();
						EnableForm(enable: true);
						XtraMessageBox.Show("Thông tin hàng thu hồi không khớp với lệnh SX!", "System", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
						return;
					}
					DataRow[] array = dtReceipt.Select("U_DelStatus = 'N' AND U_UnitUoM = 'C' AND U_QtyUoM > 1");
					if (array != null && array.Length != 0)
					{
						WinCommons._CloseProcessing();
						if (XtraMessageBox.Show("Có hàng cuộn Packing lớn hơn 1. Bạn có chắc chắn?", "System", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
						{
							EnableForm(enable: true);
							return;
						}
						WinCommons._OpenProcessing();
					}
					DataRow[] array2 = dtReceipt.Select("U_NoteQC ='' AND (U_StatusQC = 'NG' OR U_StatusQC = 'ST' OR U_StatusQC = 'NA')");
					if (array2 != null && array2.Length != 0)
					{
						WinCommons._CloseProcessing();
						EnableForm(enable: true);
						XtraMessageBox.Show("Cần thêm ghi chú hàng không tốt!", "System", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
						return;
					}
					if (dtReceipt.Rows[0].IsExistsColumn("Status") && dtReceipt.Rows[0].IsExistsColumn("Checked") && dtReceipt.Select("Checked='True'").Length == 0)
					{
						WinCommons._CloseProcessing();
						EnableForm(enable: true);
						XtraMessageBox.Show("Chưa chọn batch thành phẩm!", "System", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
						return;
					}
					btn_Confirm.Enabled = false;
					documentsImpl2.Lines = new List<Document_LinesImpl>();
					Document_LinesImpl document_LinesImpl2 = new Document_LinesImpl();
					double num3 = 0.0;
					document_LinesImpl2.WhsCode = ((GlobalData.DefaultBranch == 1) ? "SW06" : "HW06");
					if (document_LinesImpl.WhsCode.ToUpper().Equals("SW07"))
					{
						document_LinesImpl2.WhsCode = "SW09";
					}
					if (document_LinesImpl.WhsCode.ToUpper().Equals("SW11"))
					{
						document_LinesImpl2.WhsCode = "SW12";
					}
					document_LinesImpl2.BaseType = 202;
					document_LinesImpl2.BaseEntry = poRelease;
					document_LinesImpl2.BaseLine = -1;
					document_LinesImpl2.BaseRef = poRelease;
					document_LinesImpl2.BatchNumbers = new List<BatchNumbersImpl>();
					Document_LinesImpl document_LinesImpl3 = new Document_LinesImpl();
					double num4 = 0.0;
					document_LinesImpl3.WhsCode = Function.ToString(dtReceipt.Rows[0]["U_WhsCode"]);
					if (document_LinesImpl.WhsCode.ToUpper().Equals("SW07"))
					{
						document_LinesImpl3.WhsCode = "SW07";
					}
					if (document_LinesImpl.WhsCode.ToUpper().Equals("SW11"))
					{
						document_LinesImpl3.WhsCode = "SW11";
					}
					document_LinesImpl3.BaseType = 202;
					document_LinesImpl3.BaseEntry = poRelease;
					document_LinesImpl3.BaseLine = -1;
					document_LinesImpl3.BaseRef = poRelease;
					document_LinesImpl3.BatchNumbers = new List<BatchNumbersImpl>();
					Document_LinesImpl document_LinesImpl4 = new Document_LinesImpl();
					double num5 = 0.0;
					document_LinesImpl4.WhsCode = ((GlobalData.DefaultBranch == 1) ? "SW00" : "HW00");
					if (document_LinesImpl.WhsCode.ToUpper().Equals("SW07"))
					{
						document_LinesImpl4.WhsCode = "SW00";
					}
					if (document_LinesImpl.WhsCode.ToUpper().Equals("SW11"))
					{
						document_LinesImpl4.WhsCode = "SW00";
					}
					document_LinesImpl4.BaseType = 202;
					document_LinesImpl4.BaseEntry = poRelease;
					document_LinesImpl4.BaseLine = -1;
					document_LinesImpl4.BaseRef = poRelease;
					document_LinesImpl4.BatchNumbers = new List<BatchNumbersImpl>();
					suffix = 1;
					string text = Function.ToString(dataRow["U_ReBatch"]);
					string value = issueBatch.Split(';')[0];
					string[] array3 = text.Split(';');
					if (array3 != null && array3.Length != 0)
					{
						string[] array4 = array3;
						foreach (string text2 in array4)
						{
							string[] array5 = text2.Split('/');
							if (array5 != null && array5.Length > 1 && text2.StartsWith(value))
							{
								int num7 = Function.ParseInt(array5[array5.Length - 1]);
								if (num7 >= suffix)
								{
									suffix = num7 + 1;
								}
							}
						}
					}
					for (int num8 = 0; num8 < dtReceipt.Rows.Count; num8++)
					{
						if (!Function.ToString(dtReceipt.Rows[num8]["Checked"]).Equals("True"))
						{
							continue;
						}
						string text3 = Function.ToString(dtReceipt.Rows[num8]["U_UnitUoM"]);
						int num9 = Function.ParseInt(dtReceipt.Rows[num8]["PackNo"]);
						int num10 = Function.ParseInt(dtReceipt.Rows[num8]["U_QtyUoM"]);
						int num11 = Function.ParseInt(dtReceipt.Rows[num8]["Confirmed"]);
						int num12 = Function.ParseInt(dtReceipt.Rows[num8]["ConfirmReport"]);
						if (num12 <= 0)
						{
							WinCommons._CloseProcessing();
							EnableForm(enable: true);
							XtraMessageBox.Show("Nhập số lượng báo cáo!", "System", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
							btn_Confirm.Enabled = true;
							return;
						}
						if (text3.Equals("T") && num9 < 100 && (double)num9 < (double)num10 * 0.8)
						{
							if (GlobalData.UserName.ToUpper().StartsWith("PR") && XtraMessageBox.Show("Số lượng packing quá thấp. Bạn có muốn tiếp tục!", "System", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
							{
								WinCommons._CloseProcessing();
								btn_Confirm.Enabled = true;
								return;
							}
							if (!GlobalData.UserName.ToUpper().StartsWith("PR"))
							{
								WinCommons._CloseProcessing();
								EnableForm(enable: true);
								XtraMessageBox.Show("Số lượng packing quá thấp. Cần xác nhận của ca trưởng!", "System", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
								btn_Confirm.Enabled = true;
								return;
							}
						}
						if (num12 + num11 > num10 && "C".Equals(text3) && !GlobalData.UserName.ToUpper().StartsWith("PR"))
						{
							WinCommons._CloseProcessing();
							EnableForm(enable: true);
							XtraMessageBox.Show("Nhập quá số lượng báo cáo!", "System", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
							btn_Confirm.Enabled = true;
							return;
						}
						num10 = num12;
						DataRow dataRow2;
						while (num9 <= num10)
						{
							List<UserFieldsImpl> list = new List<UserFieldsImpl>();
							foreach (UserFieldsImpl item2 in lstUdf)
							{
								if (item2.Name.Equals("QtyUoM"))
								{
									UserFieldsImpl userFieldsImpl = new UserFieldsImpl();
									userFieldsImpl.Name = item2.Name;
									userFieldsImpl.Type = item2.Type;
									userFieldsImpl.SubType = item2.SubType;
									userFieldsImpl.Value = num9;
									list.Add(userFieldsImpl);
									continue;
								}
								string text4 = "U_" + item2.Name;
								if (dtReceipt.Rows[num8].IsExistsColumn(text4))
								{
									UserFieldsImpl userFieldsImpl2 = new UserFieldsImpl();
									userFieldsImpl2.Name = item2.Name;
									userFieldsImpl2.Type = item2.Type;
									userFieldsImpl2.SubType = item2.SubType;
									userFieldsImpl2.Value = dtReceipt.Rows[num8][text4];
									list.Add(userFieldsImpl2);
								}
							}
							BatchNumbersImpl batchNumbersImpl = new BatchNumbersImpl();
							batchNumbersImpl.UserFields = list;
							batchNumbersImpl.AddmisionDate = Function.ParseDateTime(dtReceipt.Rows[num8]["InDate"]);
							batchNumbersImpl.Quantity = Math.Round(Function.ParseDouble(dtReceipt.Rows[num8]["U_Width"]) * Function.ParseDouble(dtReceipt.Rows[num8]["U_Length"]) / 100.0 * (double)num9, 3, MidpointRounding.AwayFromZero);
							string text5 = Function.ToString(dtReceipt.Rows[num8]["U_BatchNo"]);
							if (!string.IsNullOrEmpty(text5) && Function.ToString(dtReceipt.Rows[num8]["U_DelStatus"]).Equals("N") && text5.Length > 8)
							{
								batchNumbersImpl.BatchNumber = Function.ToString(dtReceipt.Rows[num8]["U_BatchNo"]).Split(';')[0];
							}
							if (string.IsNullOrEmpty(batchNumbersImpl.BatchNumber) || batchNumbersImpl.BatchNumber.Length < 4)
							{
								batchNumbersImpl.BatchNumber = CreateAutoBatch(issueBatch, suffix);
								suffix++;
								dataRow2 = dtReceipt.Rows[num8];
								dataRow2["U_BatchNo"] = dataRow2["U_BatchNo"]?.ToString() + ";" + batchNumbersImpl.BatchNumber;
							}
							if (dtReceipt.Rows[num8].IsExistsColumn("Notes"))
							{
								batchNumbersImpl.Notes = Function.ToString(dtReceipt.Rows[num8]["Notes"]);
							}
							if (dtReceipt.Rows[num8].IsExistsColumn("LotNumber"))
							{
								batchNumbersImpl.InternalSerialNumber = Function.ToString(dtReceipt.Rows[num8]["LotNumber"]);
							}
							if (Function.ToString(dtReceipt.Rows[num8]["U_StatusQC"]).Equals("TM"))
							{
								document_LinesImpl4.BatchNumbers.Add(batchNumbersImpl);
								num5 += batchNumbersImpl.Quantity;
							}
							else if (Function.ToString(dtReceipt.Rows[num8]["U_DelStatus"]).Equals("Y"))
							{
								document_LinesImpl2.BatchNumbers.Add(batchNumbersImpl);
								num3 += batchNumbersImpl.Quantity;
							}
							else
							{
								document_LinesImpl3.BatchNumbers.Add(batchNumbersImpl);
								num4 += batchNumbersImpl.Quantity;
							}
							num10 -= num9;
						}
						if (num10 <= 0)
						{
							continue;
						}
						List<UserFieldsImpl> list2 = new List<UserFieldsImpl>();
						foreach (UserFieldsImpl item3 in lstUdf)
						{
							if (item3.Name.Equals("QtyUoM"))
							{
								UserFieldsImpl userFieldsImpl3 = new UserFieldsImpl();
								userFieldsImpl3.Name = item3.Name;
								userFieldsImpl3.Type = item3.Type;
								userFieldsImpl3.SubType = item3.SubType;
								userFieldsImpl3.Value = num10;
								list2.Add(userFieldsImpl3);
								continue;
							}
							if (bte_WorkCenter.Text.ToUpper().StartsWith("DE") && item3.Name.Equals("U_DelRequest"))
							{
								UserFieldsImpl userFieldsImpl4 = new UserFieldsImpl();
								userFieldsImpl4.Name = item3.Name;
								userFieldsImpl4.Type = item3.Type;
								userFieldsImpl4.SubType = item3.SubType;
								userFieldsImpl4.Value = 'Y';
								list2.Add(userFieldsImpl4);
								continue;
							}
							string text6 = "U_" + item3.Name;
							if (dtReceipt.Rows[num8].IsExistsColumn(text6))
							{
								UserFieldsImpl userFieldsImpl5 = new UserFieldsImpl();
								userFieldsImpl5.Name = item3.Name;
								userFieldsImpl5.Type = item3.Type;
								userFieldsImpl5.SubType = item3.SubType;
								userFieldsImpl5.Value = dtReceipt.Rows[num8][text6];
								list2.Add(userFieldsImpl5);
							}
						}
						BatchNumbersImpl batchNumbersImpl2 = new BatchNumbersImpl();
						batchNumbersImpl2.UserFields = list2;
						batchNumbersImpl2.Quantity = Math.Round(Function.ParseDouble(dtReceipt.Rows[num8]["U_Width"]) * Function.ParseDouble(dtReceipt.Rows[num8]["U_Length"]) / 100.0 * (double)num10, 3, MidpointRounding.AwayFromZero);
						batchNumbersImpl2.BatchNumber = CreateAutoBatch(issueBatch, suffix);
						dataRow2 = dtReceipt.Rows[num8];
						dataRow2["U_BatchNo"] = dataRow2["U_BatchNo"]?.ToString() + ";" + batchNumbersImpl2.BatchNumber;
						if (dtReceipt.Rows[num8].IsExistsColumn("Notes"))
						{
							batchNumbersImpl2.Notes = Function.ToString(dtReceipt.Rows[num8]["Notes"]);
						}
						if (dtReceipt.Rows[num8].IsExistsColumn("LotNumber"))
						{
							batchNumbersImpl2.InternalSerialNumber = Function.ToString(dtReceipt.Rows[num8]["LotNumber"]);
						}
						if (Function.ToString(dtReceipt.Rows[num8]["U_StatusQC"]).Equals("TM"))
						{
							document_LinesImpl4.BatchNumbers.Add(batchNumbersImpl2);
							num5 += batchNumbersImpl2.Quantity;
						}
						else if (Function.ToString(dtReceipt.Rows[num8]["U_DelStatus"]).Equals("Y"))
						{
							document_LinesImpl2.BatchNumbers.Add(batchNumbersImpl2);
							num3 += batchNumbersImpl2.Quantity;
						}
						else
						{
							document_LinesImpl3.BatchNumbers.Add(batchNumbersImpl2);
							num4 += batchNumbersImpl2.Quantity;
						}
						suffix++;
					}
					document_LinesImpl2.Quantity = Math.Round(num3, 3, MidpointRounding.AwayFromZero);
					document_LinesImpl3.Quantity = Math.Round(num4, 3, MidpointRounding.AwayFromZero);
					document_LinesImpl4.Quantity = Math.Round(num5, 3, MidpointRounding.AwayFromZero);
					if (document_LinesImpl2.BatchNumbers.Count > 0)
					{
						documentsImpl2.Lines.Add(document_LinesImpl2);
					}
					if (document_LinesImpl3.BatchNumbers.Count > 0)
					{
						documentsImpl2.Lines.Add(document_LinesImpl3);
					}
					if (document_LinesImpl4.BatchNumbers.Count > 0)
					{
						documentsImpl2.Lines.Add(document_LinesImpl4);
					}
					if (documentsImpl2.Lines.Count > 1)
					{
						for (int num13 = 0; num13 < documentsImpl2.Lines.Count; num13++)
						{
							documentsImpl2.Lines[num13].LineNum = num13;
							foreach (BatchNumbersImpl batchNumber2 in documentsImpl2.Lines[num13].BatchNumbers)
							{
								batchNumber2.BaseLineNumber = num13;
							}
						}
					}
					if (num * 1.2 < num2 + num3 + num4 + num5 && !GlobalData.UserName.ToUpper().StartsWith("PR"))
					{
						WinCommons._CloseProcessing();
						EnableForm(enable: true);
						XtraMessageBox.Show("Số lượng báo cáo chênh lệch quá nhiều!", "System", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
						btn_Confirm.Enabled = true;
						return;
					}
					if (num - (num2 + num3 + num4 + num5) < 1.0 && num - (num2 + num3 + num4 + num5) > -1.0)
					{
						issueBatch = Function.ToString(dataRow["U_BatchNo"]);
					}
					string[] array6 = issueBatch.Split(';');
					if (array6 != null && array6.Length != 0)
					{
						double num14 = 0.0;
						document_LinesImpl.Quantity = Function.ParseDouble(dataRow["U_Width"]) * Function.ParseDouble(dataRow["U_Length"]) * Function.ParseDouble(dataRow["U_QtyUoM"]) / (double)Function.ToString(dataRow["U_BatchNo"]).Split(';').Length * (double)array6.Length / 100.0;
						string[] array4 = array6;
						foreach (string batchNumber in array4)
						{
							BatchNumbersImpl batchNumbersImpl3 = new BatchNumbersImpl();
							batchNumbersImpl3.Quantity = Math.Round(document_LinesImpl.Quantity / (double)array6.Length, 3, MidpointRounding.AwayFromZero);
							batchNumbersImpl3.BatchNumber = batchNumber;
							document_LinesImpl.BatchNumbers.Add(batchNumbersImpl3);
							num14 += Math.Round(document_LinesImpl.Quantity / (double)array6.Length, 3, MidpointRounding.AwayFromZero);
						}
						document_LinesImpl.Quantity = Math.Round(num14, 3, MidpointRounding.AwayFromZero);
					}
					documentsImpl.Lines.Add(document_LinesImpl);
					HttpResult httpResult = await ReceiptForBatch(new Tuple<int, DocumentsImpl, DocumentsImpl, string, string, DataTable, string>(poRelease, documentsImpl, documentsImpl2, bte_WorkCenter.Text, txt_IssueBatch.Text, dtReceipt, item));
					if (!string.IsNullOrEmpty(httpResult.Message))
					{
						if (httpResult.Message.Equals("INVALIDSESSION") || httpResult.Message.Equals("Invalid Session ID"))
						{
							XtraMessageBox.Show("User bị mất Session. Thực hiện lại thao tác hoặc đăng nhập lại!", "System", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
						}
						else
						{
							WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
						}
						DataRow[] array7 = dtReceipt.Select("U_DelStatus <> 'N' AND U_BatchNo <> ''");
						if (array7 != null && array7.Length != 0)
						{
							DataRow[] array8 = array7;
							for (int num6 = 0; num6 < array8.Length; num6++)
							{
								array8[num6]["U_BatchNo"] = "";
							}
						}
						btn_Confirm.Enabled = true;
						EnableForm(enable: true);
						return;
					}
					printDoc = poRelease;
					poRelease = 0;
					btn_Confirm.Enabled = false;
					btn_UnRelease.Enabled = false;
					btn_Pending.Enabled = false;
					txt_IssueBatch.Text = "";
					txt_Barcode.Focus();
					if (httpResult.Content != null && !string.IsNullOrEmpty(httpResult.Content.ToString()))
					{
						string[] array9 = httpResult.Content.ToString().Split(',');
						if (array9 != null && array9.Length != 0)
						{
							int num15 = array9.Length - 1;
							receiptDoc = array9[0];
							if (printDoc == Function.ParseInt(array9[num15]))
							{
								num15--;
							}
							for (int num16 = 1; num16 <= num15; num16++)
							{
								receiptDoc = receiptDoc + "," + array9[num16];
							}
						}
					}
					if (!GlobalData.UserName.ToUpper().StartsWith("WH") && !GlobalData.UserName.ToUpper().StartsWith("DE"))
					{
						await PrintTem();
					}
					else
					{
						await PRINTLAYOUT();
					}
					await LoadProductionExecuting();
					WinCommons.writeStatus(Enums.MessageCode.SaveSuccess, Enums.MessageType.Success, Msg_Time._TimeMedium, "");
				}
				WinCommons._CloseProcessing();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			EnableForm(enable: true);
		}
	}

	private async void btn_Pending_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
			{
				return;
			}
			WinCommons._OpenProcessing();
			if (poRelease > 0)
			{
				string text = await UpdateStatusPO(poRelease, "D");
				if (!string.IsNullOrEmpty(text))
				{
					WinCommons.Error(Enums.MessageCode.None, text);
				}
				else
				{
					poRelease = 0;
					btn_Confirm.Enabled = false;
					btn_UnRelease.Enabled = false;
					btn_Pending.Enabled = false;
					await LoadProductionExecuting();
					WinCommons.writeStatus(Enums.MessageCode.SaveSuccess, Enums.MessageType.Success, Msg_Time._TimeMedium, "");
				}
			}
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Refresh_Click(object sender, EventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			await LoadProductionExecuting();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_CheckAll_Click(object sender, EventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			DataTable dataSourceTable = grd_ReceiptBatch.DataSourceTable;
			if (dataSourceTable.IsNotNull())
			{
				foreach (DataRow row in dataSourceTable.Rows)
				{
					int num = Function.ParseInt(row["Confirmed"]);
					int num2 = Function.ParseInt(row["U_QtyUoM"]);
					row["ConfirmReport"] = num2 - num;
					row["Checked"] = true;
				}
			}
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void gv_ProductionControl_RowClick(object sender, RowClickEventArgs e)
	{
		_ = 1;
		try
		{
			if (((e.RowHandle >= 0 && grd_ProductionControl.DataSourceTable.Rows.Count == 1) || grd_ReceiptBatch.DataSourceTable.Rows.Count == 0) && gv_ProductionControl.IsExistColumn("U_PwNo"))
			{
				int num = Function.ParseInt(gv_ProductionControl.GetRowCellValue(e.RowHandle, "U_PwNo"));
				if (num > 0)
				{
					DataSet dsProductionDetail = await GetProductionDetail(num.ToString(), GlobalData.UserName, GlobalData.DefaultBranch.ToString());
					await GridViewHelper.GenerateGrid(grd_ReceiptBatch, gv_ReceiptBatch, dsProductionDetail, lstUdf, expand: true);
					dsProductionDetail.Tables.Clear();
					dsProductionDetail.Dispose();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void date_CurrentDate_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (date_ToDate.EditValue != null)
			{
				date_FromDate.Properties.MaxValue = (DateTime)date_ToDate.EditValue;
			}
			else if (date_ToDate.EditValue == null || string.IsNullOrEmpty(date_ToDate.Text))
			{
				date_FromDate.Properties.MaxValue = DateTime.Today.AddMonths(1);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void date_FromDate_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (date_FromDate.EditValue != null)
			{
				date_ToDate.Properties.MinValue = (DateTime)date_FromDate.EditValue;
			}
			else if (date_FromDate.EditValue == null || string.IsNullOrEmpty(date_FromDate.Text))
			{
				date_ToDate.Properties.MinValue = default;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void chb_Status_EditValueChanged(object sender, EventArgs e)
	{
	}

	private void bte_WorkCenter_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.UserDefinedCfl, false, bte_WorkCenter.Text, "Apz_Cfl_GetResource", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			DataRow chooseItem = frm_CFL2.ChooseItem;
			if (chooseItem != null && chooseItem.IsExistsColumn("ResCode"))
			{
				bte_WorkCenter.Text = Function.ToString(chooseItem["ResCode"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void bte_WorkCenter_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!isLoadForm)
			{
				WinCommons._OpenProcessing();
				await LoadProductionExecuting();
				WinCommons._CloseProcessing();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void gv_ProductionControl_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (gv_ProductionControl.IsExistColumn("U_Width") && gv_ProductionControl.IsExistColumn("U_Length") && gv_ProductionControl.IsExistColumn("U_QtyUoM") && gv_ProductionControl.IsExistColumn("U_Quantity") && ("U_Width".Equals(e.Column.FieldName) || "U_Length".Equals(e.Column.FieldName)))
			{
				double num = Function.ParseDouble(gv_ProductionControl.GetRowCellValue(e.RowHandle, "U_Width"));
				double num2 = Function.ParseDouble(gv_ProductionControl.GetRowCellValue(e.RowHandle, "U_Length"));
				double num3 = Function.ParseDouble(gv_ProductionControl.GetRowCellValue(e.RowHandle, "U_QtyUoM"));
				double num4 = num * num2 * num3 / 100.0;
				gv_ProductionControl.SetRowCellValue(e.RowHandle, "U_Quantity", num4);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_ReceiptBatch_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (gv_ReceiptBatch.IsExistColumn("U_Width") && gv_ReceiptBatch.IsExistColumn("U_Length") && gv_ReceiptBatch.IsExistColumn("U_QtyUoM") && gv_ReceiptBatch.IsExistColumn("U_Quantity") && ("U_Width".Equals(e.Column.FieldName) || "U_Length".Equals(e.Column.FieldName) || "U_QtyUoM".Equals(e.Column.FieldName)))
			{
				double num = Function.ParseDouble(gv_ReceiptBatch.GetRowCellValue(e.RowHandle, "U_Width"));
				double num2 = Function.ParseDouble(gv_ReceiptBatch.GetRowCellValue(e.RowHandle, "U_Length"));
				double num3 = Function.ParseDouble(gv_ReceiptBatch.GetRowCellValue(e.RowHandle, "U_QtyUoM"));
				double num4 = num * num2 * num3 / 100.0;
				gv_ReceiptBatch.SetRowCellValue(e.RowHandle, "U_Quantity", num4);
			}
			if (!(bool)gv_ReceiptBatch.GetRowCellValue(e.RowHandle, "Checked") && !"Checked".Equals(e.Column.FieldName))
			{
				gv_ReceiptBatch.SetRowCellValue(e.RowHandle, "Checked", true);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_ReceiptBatch_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			GridViewEx gridViewEx = sender as GridViewEx;
			if (gridViewEx.IsExistColumn("Status"))
			{
				e.Cancel = !"N".Equals(Function.ToString(gridViewEx.GetRowCellValue(gridViewEx.FocusedRowHandle, "Status")));
			}
			if (gridViewEx.IsExistColumn("U_UnitUoM"))
			{
				string value = Function.ToString(gridViewEx.GetRowCellValue(gridViewEx.FocusedRowHandle, "U_UnitUoM"));
				if ("C".Equals(value) && gridViewEx.FocusedColumn.FieldName == "U_QtyUoM")
				{
					e.Cancel = true;
					return;
				}
				if ("T".Equals(value) && gridViewEx.FocusedColumn.FieldName == "U_Length")
				{
					e.Cancel = false;
					return;
				}
				if ("C".Equals(value) && gridViewEx.FocusedColumn.FieldName == "PackNo")
				{
					e.Cancel = true;
					return;
				}
				if (gridViewEx.FocusedColumn.FieldName == "U_Width" && !GlobalData.UserName.ToUpper().StartsWith("PR"))
				{
					e.Cancel = true;
					return;
				}
				string value2 = Function.ToString(gv_ReceiptBatch.GetRowCellValue(gv_ReceiptBatch.FocusedRowHandle, "NextWorkcenter"));
				if (gridViewEx.FocusedColumn.FieldName == "U_BatchNo" && !string.IsNullOrEmpty(value2))
				{
					XtraMessageBox.Show("Cuộn hàng tiếp phiếu. Không thể chỉnh sửa!", "System", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
					e.Cancel = true;
					return;
				}
			}
			if (gridViewEx.IsExistColumn("ID") && gridViewEx.FocusedColumn.FieldName.Equals("U_DelStatus") && !GlobalData.UserName.ToUpper().StartsWith("PR"))
			{
				e.Cancel = Function.ParseInt(gridViewEx.GetRowCellValue(gridViewEx.FocusedRowHandle, "ID")) != 0;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_ReceiptBatch_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
	{
		try
		{
			if (GlobalData.UserName.ToUpper().StartsWith("PR") || GlobalData.UserName.ToLower().Equals("manager"))
			{
				GridViewMenu gridViewMenu = new GridViewMenu((GridViewEx)sender);
				DXMenuItem dXMenuItem = new DXMenuItem();
				dXMenuItem.Caption = "Remove Row";
				dXMenuItem.Click += docMenu_Click;
				gridViewMenu.Items.Add(dXMenuItem);
				DXMenuItem dXMenuItem2 = new DXMenuItem();
				dXMenuItem2.Caption = "Add New Row";
				dXMenuItem2.Click += docMenu_Click;
				gridViewMenu.Items.Add(dXMenuItem2);
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

	private void docMenu_Click(object sender, EventArgs e)
	{
		try
		{
			DXMenuItem dXMenuItem = sender as DXMenuItem;
			if (gv_ProductionControl.FocusedRowHandle > -1)
			{
				string value = Function.ToString(gv_ProductionControl.GetRowCellValue(gv_ProductionControl.FocusedRowHandle, "Status"));
				if ("L".Equals(value))
				{
					return;
				}
			}
			if (dXMenuItem.Caption.Equals("Remove Row"))
			{
				if (gv_ReceiptBatch.FocusedRowHandle > -1)
				{
					string value2 = Function.ToString(gv_ReceiptBatch.GetRowCellValue(gv_ReceiptBatch.FocusedRowHandle, "Status"));
					if ("Y".Equals(value2))
					{
						XtraMessageBox.Show("Không thể xóa hàng đã sản xuất!", "System", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
					}
					else if (!string.IsNullOrEmpty(Function.ToString(gv_ReceiptBatch.GetRowCellValue(gv_ReceiptBatch.FocusedRowHandle, "NextWorkcenter"))))
					{
						XtraMessageBox.Show("Cuộn hàng tiếp phiếu. Không thể xóa!", "System", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
					}
					else if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
					{
						WinCommons._OpenProcessing();
						gv_ReceiptBatch.DeleteRow(gv_ReceiptBatch.FocusedRowHandle);
						WinCommons._CloseProcessing();
					}
				}
			}
			else
			{
				if (!dXMenuItem.Caption.Equals("Add New Row") || gv_ReceiptBatch.FocusedRowHandle <= -1)
				{
					return;
				}
				if (!string.IsNullOrEmpty(Function.ToString(gv_ReceiptBatch.GetRowCellValue(gv_ReceiptBatch.FocusedRowHandle, "NextWorkcenter"))))
				{
					XtraMessageBox.Show("Cuộn hàng tiếp phiếu. Không thể thêm dòng!", "System", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
					return;
				}
				WinCommons._OpenProcessing();
				DataTable dataTable = (DataTable)grd_ReceiptBatch.DataSource;
				if (!dataTable.IsNotNull())
				{
					return;
				}
				DataRow dataRow = gv_ReceiptBatch.GetDataRow(gv_ReceiptBatch.FocusedRowHandle);
				DataRow dataRow2 = dataTable.NewRow();
				WinFuntions.DataRowToDataRow(dataRow, dataRow2);
				string text = Function.ToString(dataTable.Rows[dataTable.Rows.Count - 1]["U_BatchNo"]);
				if (string.IsNullOrEmpty(text))
				{
					text = Function.ToString(dataRow["U_BatchNo"]);
				}
				string[] array = ((!string.IsNullOrEmpty(text)) ? text.Split('/') : null);
				if (array != null && array.Length > 1)
				{
					int num = Function.ParseInt(array[array.Length - 1]);
					string text2 = "";
					string text3 = "";
					for (int i = 0; i < array.Length - 1; i++)
					{
						text2 = text2 + "/" + array[i];
					}
					text2 = text2.Substring(1, text2.Length - 1);
					if (num < 10)
					{
						text3 = "00" + (num + 1);
					}
					else
					{
						text3 = ((num >= 100) ? (num + 1).ToString() : ("0" + (num + 1)));
					}
					if (dataRow2.IsExistsColumn("U_BatchNo"))
					{
						dataRow2["U_BatchNo"] = text2 + "/" + text3;
					}
				}
				else if (array != null && array.Length == 1)
				{
					string text4 = (Function.ParseInt(array[0].Substring(array[0].Length - 2)) + 1).ToString();
					dataRow2["U_BatchNo"] = text.Substring(0, text.Length - 2) + text4;
				}
				if (dataRow2.IsExistsColumn("ID"))
				{
					dataRow2["ID"] = 0;
				}
				if (dataRow2.IsExistsColumn("U_DelStatus"))
				{
					dataRow2["U_DelStatus"] = "N";
				}
				dataTable.Rows.Add(dataRow2);
				grd_ReceiptBatch.RefreshDataSource();
				WinCommons._CloseProcessing();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_ClosePO_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (!grd_ProductionControl.DataSourceTable.IsNotNull())
			{
				XtraMessageBox.Show("Không có thông tin lệnh sản xuất!", "System", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
			}
			else
			{
				if (gv_ProductionControl.FocusedRowHandle < 0 || !GlobalData.UserName.ToUpper().StartsWith("PR") || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
				{
					return;
				}
				DataRow dataRow = gv_ProductionControl.GetDataRow(gv_ProductionControl.FocusedRowHandle);
				if (dataRow == null || !"R".Equals(Function.ToString(dataRow["Status"])))
				{
					return;
				}
				WinCommons._OpenProcessing();
				int docEntry = Function.ParseInt(dataRow["U_PwNo"]);
				double num = Function.ParseDouble(dataRow["U_Quantity"]);
				if (Function.ParseDouble(dataRow["CmpltQty"]) <= num * 0.8 && !GlobalData.UserName.ToUpper().StartsWith("PR"))
				{
					WinCommons._CloseProcessing();
					XtraMessageBox.Show("Lệnh sản xuất chưa báo cáo đủ!", "System", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
					return;
				}
				string text = await UpdateStatusPO(docEntry, "L");
				if (!string.IsNullOrEmpty(text))
				{
					WinCommons.Error(Enums.MessageCode.None, text);
				}
				else
				{
					btn_Confirm.Enabled = false;
					btn_UnRelease.Enabled = false;
					btn_Pending.Enabled = false;
					await LoadProductionExecuting();
					WinCommons.writeStatus(Enums.MessageCode.SaveSuccess, Enums.MessageType.Success, Msg_Time._TimeMedium, "");
				}
				WinCommons._CloseProcessing();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void gv_ProductionControl_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
	{
		_ = 1;
		try
		{
			if (e.FocusedRowHandle >= 0 && gv_ProductionControl.IsExistColumn("U_PwNo"))
			{
				int num = Function.ParseInt(gv_ProductionControl.GetRowCellValue(gv_ProductionControl.FocusedRowHandle, "U_PwNo"));
				if (num > 0)
				{
					DataSet dsProductionDetail = await GetProductionDetail(num.ToString(), GlobalData.UserName, GlobalData.DefaultBranch.ToString());
					await GridViewHelper.GenerateGrid(grd_ReceiptBatch, gv_ReceiptBatch, dsProductionDetail, lstUdf, expand: true);
					dsProductionDetail.Tables.Clear();
					dsProductionDetail.Dispose();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void chk_Timer_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			spin_Timer.Enabled = chk_Timer.Checked;
			timer_LoadForm.Enabled = chk_Timer.Checked;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void spin_Timer_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			timer_LoadForm.Interval = Function.ParseInt(spin_Timer.EditValue) * 60000;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void timer_LoadForm_Tick(object sender, EventArgs e)
	{
		try
		{
			if (!isLoadForm)
			{
				await LoadProductionExecuting();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private string CreateAutoBatch(string issBatch, int suffix)
	{
		try
		{
			string text = issBatch.Split(';')[0];
			if (text.Length > 25)
			{
				string[] array = text.Split('.');
				if (array.Length > 3)
				{
					text = string.Format("{0}.{1}.{2}\\{3}", array[0], array[1], DateTime.Now.ToString("hhmmss"), array[3]);
				}
			}
			if (suffix < 10)
			{
				return $"{text}/0{suffix}";
			}
			_ = 100;
			return $"{text}/{suffix}";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return "";
	}

	private async Task LoadProductionExecuting()
	{
		_ = 3;
		try
		{
			EnableForm(enable: false);
			gv_ProductionControl.OptionsView.ShowFilterPanelMode = ShowFilterPanelMode.Default;
			string text = "";
			for (int i = 0; i < chb_Status.Properties.Items.Count; i++)
			{
				if (chb_Status.Properties.Items[i].CheckState == CheckState.Checked)
				{
					text = text + "," + chb_Status.Properties.Items[i].Value;
				}
			}
			text = ((!string.IsNullOrEmpty(text) && text.StartsWith(",")) ? text.Substring(1, text.Length - 1) : "");
			CreateDataSearch();
			DataRow dataRow = dtSearch.NewRow();
			dataRow["status"] = text;
			dataRow["fromDate"] = ((date_FromDate.EditValue != null) ? Convert.ToDateTime(date_FromDate.EditValue) : DateTime.Today);
			dataRow["toDate"] = ((date_ToDate.EditValue != null) ? Convert.ToDateTime(date_ToDate.EditValue) : DateTime.Today);
			dataRow["username"] = ((!string.IsNullOrEmpty(bte_WorkCenter.Text)) ? bte_WorkCenter.Text : GlobalData.UserName);
			dataRow["branch"] = GlobalData.DefaultBranch.ToString();
			dtSearch.Rows.Add(dataRow);
			DataSet dsProductionControl = await GetProductionControl();
			await GridViewHelper.GenerateGrid(grd_ProductionControl, gv_ProductionControl, dsProductionControl, lstUdf, expand: true);
			dsProductionControl.Tables.Clear();
			dsProductionControl.Dispose();
			if (poRelease > 0)
			{
				DataSet dsProductionDetail = await GetProductionDetail(poRelease.ToString(), GlobalData.UserName, GlobalData.DefaultBranch.ToString());
				await GridViewHelper.GenerateGrid(grd_ReceiptBatch, gv_ReceiptBatch, dsProductionDetail, lstUdf, expand: true);
				dsProductionDetail.Tables.Clear();
				dsProductionDetail.Dispose();
			}
			else
			{
				grd_ReceiptBatch.DataSourceTable.Rows.Clear();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		EnableForm(enable: true);
		txt_Barcode.Text = "";
		txt_Barcode.Focus();
	}

	private async Task<DataSet> GetProductionControl()
	{
		try
		{
			using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "ProductionConfirm");
			return await helper.PostAsync("GetProductionControl", dtSearch);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataSet();
	}

	private void CreateDataSearch()
	{
		if (!dtSearch.IsNotNull() || dtSearch.Columns.Count == 0)
		{
			dtSearch = new DataTable();
			dtSearch.Columns.Add(new DataColumn("status", typeof(string)));
			dtSearch.Columns.Add(new DataColumn("fromDate", typeof(DateTime)));
			dtSearch.Columns.Add(new DataColumn("toDate", typeof(DateTime)));
			dtSearch.Columns.Add(new DataColumn("username", typeof(string)));
			dtSearch.Columns.Add(new DataColumn("branch", typeof(string)));
		}
		else
		{
			dtSearch.Rows.Clear();
		}
	}

	private async Task<DataSet> GetProductionDetail(string docEntry, string username, string branch)
	{
		try
		{
			using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "ProductionConfirm");
			return await helper.GetAsync("GetProductionDetail", new { docEntry, username, branch });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataSet();
	}

	private async Task<string> UpdateStatusPO(int docEntry, string status)
	{
		try
		{
			using HttpClientHelper<string> helper = new HttpClientHelper<string>(Configuration.ApiServer, "ProductionOder");
			return await helper.GetAsync("UpdateStatusByDI", new { docEntry, status });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public async Task<HttpResult> ReceiptForBatch(Tuple<int, DocumentsImpl, DocumentsImpl, string, string, DataTable, string> model)
	{
		try
		{
			using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "ProductionOder");
			return await helper.PostAsync("ConfirmProduction", model);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task<string> GetWorkCenter(string batchNo)
	{
		try
		{
			using HttpClientHelper<string> helper = new HttpClientHelper<string>(Configuration.ApiServer, "ProductionConfirm");
			return await helper.GetAsync("GetWorkCenter", new { batchNo });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private void EnableForm(bool enable)
	{
		txt_Barcode.Enabled = enable;
		chb_Status.Enabled = enable;
		bte_WorkCenter.Enabled = enable;
		date_FromDate.Enabled = enable;
		date_ToDate.Enabled = enable;
		btn_Refresh.Enabled = enable;
		btn_ClosePO.Enabled = enable;
		btn_CheckAll.Enabled = enable;
		grd_ProductionControl.Enabled = enable;
		grd_ReceiptBatch.Enabled = enable;
		txt_Barcode.Focus();
	}

	public async Task PRINTLAYOUT()
	{
		try
		{
			int num = 0;
			if (printDoc > 0)
			{
				num = printDoc;
			}
			else if (gv_ProductionControl.IsExistColumn("U_PwNo"))
			{
				num = Function.ParseInt(gv_ProductionControl.GetFocusedRowCellValue("U_PwNo"));
			}
			if (num > 0)
			{
				await WinFuntions.PrintLayout("", num, "WOR1");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task PRINT()
	{
		_ = 1;
		try
		{
			if (!GlobalData.UserName.StartsWith("PR"))
			{
				return;
			}
			int docEntry = 0;
			if (!string.IsNullOrEmpty(receiptDoc))
			{
				string[] array = receiptDoc.Split(',');
				if (array != null && array.Length != 0)
				{
					string[] array2 = array;
					foreach (string obj in array2)
					{
						await WinFuntions.Print("", Function.ParseInt(obj), "IGN2");
					}
				}
			}
			else
			{
				docEntry = Function.ParseInt(gv_ProductionControl.GetFocusedRowCellValue("U_PwNo"));
			}
			if (docEntry > 0)
			{
				await WinFuntions.Print("", docEntry, "WOR1");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task PrintTem()
	{
		_ = 1;
		try
		{
			int docEntry = 0;
			if (!string.IsNullOrEmpty(receiptDoc))
			{
				string[] array = receiptDoc.Split(',');
				if (array != null && array.Length != 0)
				{
					string[] array2 = array;
					foreach (string obj in array2)
					{
						await WinFuntions.Print("", Function.ParseInt(obj), "IGN2");
					}
				}
			}
			else
			{
				docEntry = Function.ParseInt(gv_ProductionControl.GetFocusedRowCellValue("U_PwNo"));
			}
			if (docEntry > 0)
			{
				await WinFuntions.Print("", docEntry, "WOR1");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ttc_ProductionControl_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
	{
		try
		{
			if (e.SelectedControl != grd_ProductionControl)
			{
				return;
			}
			ToolTipControlInfo toolTipControlInfo = null;
			if (!(grd_ProductionControl.GetViewAt(e.ControlMousePosition) is GridViewEx gridViewEx))
			{
				return;
			}
			GridHitInfo gridHitInfo = gridViewEx.CalcHitInfo(e.ControlMousePosition);
			if (gridHitInfo.HitTest == GridHitTest.RowCell && gridHitInfo.RowHandle >= 0)
			{
				string text = "";
				string text2 = gv_ProductionControl.GetRowCellValue(gridHitInfo.RowHandle, "Status").ToString();
				int num = Function.ParseInt(gv_ProductionControl.GetRowCellValue(gridHitInfo.RowHandle, "U_PwNo"));
				if (num > 0 && poRelease == num)
				{
					Text = "Lệnh sản xuất Đang thực hiện";
				}
				else if (text2.Equals("P"))
				{
					text = "Lệnh sản xuất chưa thực hiện";
				}
				else if (text2.Equals("L"))
				{
					text = "Lệnh sản xuất Đã hoàn thành";
				}
				else if (text2.Equals("PD"))
				{
					text = "Lệnh sản xuất Đang tạm dừng";
				}
				toolTipControlInfo = new ToolTipControlInfo(gridHitInfo.HitTest + gridHitInfo.RowHandle, text);
			}
			if (toolTipControlInfo != null)
			{
				e.Info = toolTipControlInfo;
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
		this.components = new System.ComponentModel.Container();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.MTO_Manufacturing.frm_ProductionConfirm));
		this.xtraTabControl1 = new Apzon.Controls.Controls.XtraTabControlEx();
		this.tab_ProductionControl = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_ProductionControl = new Apzon.Controls.Grid.GridControlEx();
		this.gv_ProductionControl = new Apzon.Controls.Grid.GridViewEx();
		this.ttc_ProductionControl = new DevExpress.Utils.ToolTipController(this.components);
		this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
		this.date_ToDate = new Apzon.Controls.Editor.DateEditEx();
		this.date_FromDate = new Apzon.Controls.Editor.DateEditEx();
		this.labelLine5 = new Apzon.Controls.Controls.LabelLine();
		this.txt_IssueBatch = new DevExpress.XtraEditors.MemoExEdit();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.bte_WorkCenter = new Apzon.Controls.Editor.ButtonEditEx();
		this.chb_Status = new DevExpress.XtraEditors.CheckedComboBoxEdit();
		this.lb_BusinessPartner = new Apzon.Controls.Controls.LabelLine();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.txt_Barcode = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.spin_Timer = new DevExpress.XtraEditors.SpinEdit();
		this.chk_Timer = new Apzon.Controls.Editor.CheckEditEx();
		this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
		this.btn_CheckAll = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_ClosePO = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Refresh = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Pending = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_UnRelease = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Confirm = new Apzon.Controls.Controls.SimpleButtonEx();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.xtraTabControl2 = new Apzon.Controls.Controls.XtraTabControlEx();
		this.xtraTabPage1 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_ReceiptBatch = new Apzon.Controls.Grid.GridControlEx();
		this.gv_ReceiptBatch = new Apzon.Controls.Grid.GridViewEx();
		this.timer_LoadForm = new System.Windows.Forms.Timer(this.components);
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).BeginInit();
		this.xtraTabControl1.SuspendLayout();
		this.tab_ProductionControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_ProductionControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_ProductionControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl1).BeginInit();
		this.panelControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.date_ToDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_ToDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_FromDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_FromDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_IssueBatch.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bte_WorkCenter.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chb_Status.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Barcode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.spin_Timer.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Timer.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl2).BeginInit();
		this.panelControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl2).BeginInit();
		this.xtraTabControl2.SuspendLayout();
		this.xtraTabPage1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_ReceiptBatch).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_ReceiptBatch).BeginInit();
		base.SuspendLayout();
		this.xtraTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.xtraTabControl1.IsSearchControl = false;
		this.xtraTabControl1.Location = new System.Drawing.Point(0, 0);
		this.xtraTabControl1.Name = "xtraTabControl1";
		this.xtraTabControl1.SelectedTabPage = this.tab_ProductionControl;
		this.xtraTabControl1.Size = new System.Drawing.Size(922, 195);
		this.xtraTabControl1.TabIndex = 0;
		this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[1] { this.tab_ProductionControl });
		this.tab_ProductionControl.Controls.Add(this.grd_ProductionControl);
		this.tab_ProductionControl.EditControl = null;
		this.tab_ProductionControl.EditValue = "Production Confirmation";
		this.tab_ProductionControl.FieldName = null;
		this.tab_ProductionControl.IsSearchControl = false;
		this.tab_ProductionControl.Name = "tab_ProductionControl";
		this.tab_ProductionControl.ReadOnly = false;
		this.tab_ProductionControl.SettingInVisible = false;
		this.tab_ProductionControl.SettingReadonly = false;
		this.tab_ProductionControl.Size = new System.Drawing.Size(920, 170);
		this.tab_ProductionControl.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_ProductionControl.Text = "Production Confirmation";
		this.grd_ProductionControl.AllowPasteExcel = true;
		this.grd_ProductionControl.ClipboardData = null;
		this.grd_ProductionControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_ProductionControl.Location = new System.Drawing.Point(0, 0);
		this.grd_ProductionControl.MainView = this.gv_ProductionControl;
		this.grd_ProductionControl.Name = "grd_ProductionControl";
		this.grd_ProductionControl.Size = new System.Drawing.Size(920, 170);
		this.grd_ProductionControl.TabIndex = 0;
		this.grd_ProductionControl.ToolTipController = this.ttc_ProductionControl;
		this.grd_ProductionControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_ProductionControl });
		this.gv_ProductionControl.Appearance.FocusedRow.BackColor = System.Drawing.Color.Transparent;
		this.gv_ProductionControl.Appearance.FocusedRow.BackColor2 = System.Drawing.Color.Transparent;
		this.gv_ProductionControl.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_ProductionControl.Appearance.SelectedRow.BackColor = System.Drawing.Color.Transparent;
		this.gv_ProductionControl.Appearance.SelectedRow.BackColor2 = System.Drawing.Color.Transparent;
		this.gv_ProductionControl.Appearance.SelectedRow.Options.UseBackColor = true;
		this.gv_ProductionControl.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_ProductionControl.ColumnKey = null;
		this.gv_ProductionControl.DisableFormSetting = false;
		this.gv_ProductionControl.DisplayException = false;
		this.gv_ProductionControl.EditColumn = null;
		this.gv_ProductionControl.FieldCurrency = null;
		this.gv_ProductionControl.FieldRate = null;
		this.gv_ProductionControl.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFullFocus;
		this.gv_ProductionControl.FormId = "oProductionConfirm";
		this.gv_ProductionControl.GridControl = this.grd_ProductionControl;
		this.gv_ProductionControl.IgnoreFilter = false;
		this.gv_ProductionControl.IndexEditRow = -1;
		this.gv_ProductionControl.IndicatorWidth = 35;
		this.gv_ProductionControl.IsApzTable = false;
		this.gv_ProductionControl.IsInsertNewRow = false;
		this.gv_ProductionControl.IsLoadingData = false;
		this.gv_ProductionControl.IsMappingContextAction = false;
		this.gv_ProductionControl.ItemId = "OWOR";
		this.gv_ProductionControl.LstColumnAllowBatch = null;
		this.gv_ProductionControl.LstColumnNotAllowBatch = null;
		this.gv_ProductionControl.LstColumnNotEditable = null;
		this.gv_ProductionControl.LstColumNotSetting = null;
		this.gv_ProductionControl.Name = "gv_ProductionControl";
		this.gv_ProductionControl.UseBatchMod = false;
		this.gv_ProductionControl.RowClick += new DevExpress.XtraGrid.Views.Grid.RowClickEventHandler(this.gv_ProductionControl_RowClick);
		this.gv_ProductionControl.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.gv_ProductionControl_RowStyle);
		this.gv_ProductionControl.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gv_ProductionControl_FocusedRowChanged);
		this.gv_ProductionControl.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_ProductionControl_CellValueChanged);
		this.ttc_ProductionControl.GetActiveObjectInfo += new DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventHandler(this.ttc_ProductionControl_GetActiveObjectInfo);
		this.panelControl1.Controls.Add(this.date_ToDate);
		this.panelControl1.Controls.Add(this.date_FromDate);
		this.panelControl1.Controls.Add(this.labelLine5);
		this.panelControl1.Controls.Add(this.txt_IssueBatch);
		this.panelControl1.Controls.Add(this.labelLine4);
		this.panelControl1.Controls.Add(this.bte_WorkCenter);
		this.panelControl1.Controls.Add(this.chb_Status);
		this.panelControl1.Controls.Add(this.lb_BusinessPartner);
		this.panelControl1.Controls.Add(this.labelLine3);
		this.panelControl1.Controls.Add(this.labelLine2);
		this.panelControl1.Controls.Add(this.txt_Barcode);
		this.panelControl1.Controls.Add(this.labelLine1);
		this.panelControl1.Dock = System.Windows.Forms.DockStyle.Top;
		this.panelControl1.Location = new System.Drawing.Point(0, 0);
		this.panelControl1.Name = "panelControl1";
		this.panelControl1.Size = new System.Drawing.Size(922, 56);
		this.panelControl1.TabIndex = 1;
		this.date_ToDate.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.date_ToDate.EditControl = null;
		this.date_ToDate.EditValue = null;
		this.date_ToDate.FieldName = null;
		this.date_ToDate.Location = new System.Drawing.Point(816, 28);
		this.date_ToDate.Name = "date_ToDate";
		this.date_ToDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_ToDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_ToDate.Properties.IsSearchControl = false;
		this.date_ToDate.SettingInVisible = false;
		this.date_ToDate.SettingReadonly = false;
		this.date_ToDate.Size = new System.Drawing.Size(101, 20);
		this.date_ToDate.TabIndex = 65;
		this.date_ToDate.EditValueChanged += new System.EventHandler(this.date_CurrentDate_EditValueChanged);
		this.date_FromDate.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.date_FromDate.EditControl = null;
		this.date_FromDate.EditValue = null;
		this.date_FromDate.FieldName = null;
		this.date_FromDate.Location = new System.Drawing.Point(619, 27);
		this.date_FromDate.Name = "date_FromDate";
		this.date_FromDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_FromDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_FromDate.Properties.IsSearchControl = false;
		this.date_FromDate.SettingInVisible = false;
		this.date_FromDate.SettingReadonly = false;
		this.date_FromDate.Size = new System.Drawing.Size(111, 20);
		this.date_FromDate.TabIndex = 64;
		this.date_FromDate.EditValueChanged += new System.EventHandler(this.date_FromDate_EditValueChanged);
		this.labelLine5.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.labelLine5.EditControl = null;
		this.labelLine5.EditValue = "From Date";
		this.labelLine5.FieldName = null;
		this.labelLine5.HideWhenEmpty = false;
		this.labelLine5.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine5.Location = new System.Drawing.Point(539, 29);
		this.labelLine5.Name = "labelLine5";
		this.labelLine5.ReadOnly = false;
		this.labelLine5.SettingInVisible = false;
		this.labelLine5.SettingReadonly = false;
		this.labelLine5.ShowUnderLine = true;
		this.labelLine5.Size = new System.Drawing.Size(80, 20);
		this.labelLine5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine5.TabIndex = 63;
		this.labelLine5.Utext = "From Date";
		this.txt_IssueBatch.Location = new System.Drawing.Point(301, 27);
		this.txt_IssueBatch.Name = "txt_IssueBatch";
		this.txt_IssueBatch.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_IssueBatch.Properties.ReadOnly = true;
		this.txt_IssueBatch.Properties.ShowIcon = false;
		this.txt_IssueBatch.Size = new System.Drawing.Size(212, 20);
		this.txt_IssueBatch.TabIndex = 61;
		this.txt_IssueBatch.ToolTip = "Barcode đã bắn";
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "Issue Batch";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(241, 30);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(62, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 59;
		this.labelLine4.Utext = "Issue Batch";
		this.bte_WorkCenter.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.bte_WorkCenter.EditControl = null;
		this.bte_WorkCenter.FieldName = null;
		this.bte_WorkCenter.IsSearchControl = false;
		this.bte_WorkCenter.Location = new System.Drawing.Point(816, 5);
		this.bte_WorkCenter.Name = "bte_WorkCenter";
		this.bte_WorkCenter.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.bte_WorkCenter.Properties.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.bte_WorkCenter_Properties_ButtonClick);
		this.bte_WorkCenter.SettingInVisible = false;
		this.bte_WorkCenter.SettingReadonly = false;
		this.bte_WorkCenter.Size = new System.Drawing.Size(101, 20);
		this.bte_WorkCenter.TabIndex = 58;
		this.bte_WorkCenter.ToolTip = "Máy Sản Xuất";
		this.bte_WorkCenter.EditValueChanged += new System.EventHandler(this.bte_WorkCenter_EditValueChanged);
		this.chb_Status.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.chb_Status.Location = new System.Drawing.Point(619, 4);
		this.chb_Status.Name = "chb_Status";
		this.chb_Status.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.chb_Status.Size = new System.Drawing.Size(111, 20);
		this.chb_Status.TabIndex = 57;
		this.chb_Status.ToolTip = "Trạng thái lệnh sản xuất";
		this.chb_Status.EditValueChanged += new System.EventHandler(this.chb_Status_EditValueChanged);
		this.lb_BusinessPartner.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lb_BusinessPartner.EditControl = null;
		this.lb_BusinessPartner.EditValue = "Status";
		this.lb_BusinessPartner.FieldName = null;
		this.lb_BusinessPartner.HideWhenEmpty = false;
		this.lb_BusinessPartner.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_BusinessPartner.Location = new System.Drawing.Point(540, 6);
		this.lb_BusinessPartner.Name = "lb_BusinessPartner";
		this.lb_BusinessPartner.ReadOnly = false;
		this.lb_BusinessPartner.SettingInVisible = false;
		this.lb_BusinessPartner.SettingReadonly = false;
		this.lb_BusinessPartner.ShowUnderLine = true;
		this.lb_BusinessPartner.Size = new System.Drawing.Size(80, 20);
		this.lb_BusinessPartner.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_BusinessPartner.TabIndex = 56;
		this.lb_BusinessPartner.Utext = "Status";
		this.labelLine3.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "To Date";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(736, 28);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(80, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 7;
		this.labelLine3.Utext = "To Date";
		this.labelLine2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "Work Center";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(736, 6);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(80, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 5;
		this.labelLine2.Utext = "Work Center";
		this.txt_Barcode.EditControl = null;
		this.txt_Barcode.FieldName = null;
		this.txt_Barcode.IsSearchControl = false;
		this.txt_Barcode.Location = new System.Drawing.Point(12, 28);
		this.txt_Barcode.Name = "txt_Barcode";
		this.txt_Barcode.SettingInVisible = false;
		this.txt_Barcode.SettingReadonly = false;
		this.txt_Barcode.Size = new System.Drawing.Size(223, 20);
		this.txt_Barcode.TabIndex = 1;
		this.txt_Barcode.ToolTip = "Barcode";
		this.txt_Barcode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_Barcode_KeyDown);
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Barcode";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(12, 5);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(45, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 0;
		this.labelLine1.Utext = "Barcode";
		this.spin_Timer.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.spin_Timer.EditValue = new decimal(new int[4] { 10, 0, 0, 0 });
		this.spin_Timer.Location = new System.Drawing.Point(352, 9);
		this.spin_Timer.Name = "spin_Timer";
		this.spin_Timer.Properties.AllowMouseWheel = false;
		this.spin_Timer.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.spin_Timer.Properties.DisplayFormat.FormatString = "{0:n0} Minute";
		this.spin_Timer.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
		this.spin_Timer.Properties.EditFormat.FormatString = "{0:n0} Minute";
		this.spin_Timer.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
		this.spin_Timer.Properties.Mask.EditMask = "n0";
		this.spin_Timer.Properties.MaxValue = new decimal(new int[4] { 60, 0, 0, 0 });
		this.spin_Timer.Properties.MinValue = new decimal(new int[4] { 1, 0, 0, 0 });
		this.spin_Timer.Properties.NullText = "0";
		this.spin_Timer.Size = new System.Drawing.Size(82, 20);
		this.spin_Timer.TabIndex = 55;
		this.spin_Timer.ToolTip = "Thời gian tự động làm mới dữ liệu";
		this.spin_Timer.Visible = false;
		this.spin_Timer.EditValueChanged += new System.EventHandler(this.spin_Timer_EditValueChanged);
		this.chk_Timer.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.chk_Timer.EditControl = null;
		this.chk_Timer.Enabled = false;
		this.chk_Timer.FieldName = null;
		this.chk_Timer.IsSearchControl = false;
		this.chk_Timer.Location = new System.Drawing.Point(269, 8);
		this.chk_Timer.Name = "chk_Timer";
		this.chk_Timer.Properties.Caption = "Auto Refresh";
		this.chk_Timer.SettingInVisible = false;
		this.chk_Timer.SettingReadonly = false;
		this.chk_Timer.Size = new System.Drawing.Size(89, 20);
		this.chk_Timer.TabIndex = 54;
		this.chk_Timer.ToolTip = "Tự động làm mới dữ liệu sau 1 khoảng thời gian tùy chọn";
		this.chk_Timer.Visible = false;
		this.chk_Timer.CheckedChanged += new System.EventHandler(this.chk_Timer_CheckedChanged);
		this.panelControl2.Controls.Add(this.btn_CheckAll);
		this.panelControl2.Controls.Add(this.btn_ClosePO);
		this.panelControl2.Controls.Add(this.btn_Refresh);
		this.panelControl2.Controls.Add(this.btn_Pending);
		this.panelControl2.Controls.Add(this.spin_Timer);
		this.panelControl2.Controls.Add(this.chk_Timer);
		this.panelControl2.Controls.Add(this.btn_Cancel);
		this.panelControl2.Controls.Add(this.btn_UnRelease);
		this.panelControl2.Controls.Add(this.btn_Confirm);
		this.panelControl2.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.panelControl2.Location = new System.Drawing.Point(0, 461);
		this.panelControl2.Name = "panelControl2";
		this.panelControl2.Size = new System.Drawing.Size(922, 33);
		this.panelControl2.TabIndex = 2;
		this.btn_CheckAll.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_CheckAll.EditControl = null;
		this.btn_CheckAll.EditValue = "Auto Check";
		this.btn_CheckAll.FieldName = null;
		this.btn_CheckAll.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_CheckAll.ImageOptions.Image");
		this.btn_CheckAll.IsSearchControl = false;
		this.btn_CheckAll.Location = new System.Drawing.Point(662, 6);
		this.btn_CheckAll.Name = "btn_CheckAll";
		this.btn_CheckAll.ReadOnly = false;
		this.btn_CheckAll.SettingInVisible = false;
		this.btn_CheckAll.SettingReadonly = false;
		this.btn_CheckAll.Size = new System.Drawing.Size(82, 23);
		this.btn_CheckAll.TabIndex = 1;
		this.btn_CheckAll.Text = "Auto Check";
		this.btn_CheckAll.ToolTip = "Tự động check và nhập số lượng báo cáo mặc định";
		this.btn_CheckAll.Click += new System.EventHandler(this.btn_CheckAll_Click);
		this.btn_ClosePO.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_ClosePO.EditControl = null;
		this.btn_ClosePO.EditValue = "Close PO";
		this.btn_ClosePO.FieldName = null;
		this.btn_ClosePO.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_ClosePO.ImageOptions.Image");
		this.btn_ClosePO.IsSearchControl = false;
		this.btn_ClosePO.Location = new System.Drawing.Point(750, 6);
		this.btn_ClosePO.Name = "btn_ClosePO";
		this.btn_ClosePO.ReadOnly = false;
		this.btn_ClosePO.SettingInVisible = false;
		this.btn_ClosePO.SettingReadonly = false;
		this.btn_ClosePO.Size = new System.Drawing.Size(86, 23);
		this.btn_ClosePO.TabIndex = 6;
		this.btn_ClosePO.Text = "Close PO";
		this.btn_ClosePO.ToolTip = "Đóng lệnh sản xuất bằng tay. Chỉ tài khoản PR mới thực hiện được chức năng này";
		this.btn_ClosePO.ToolTipIconType = DevExpress.Utils.ToolTipIconType.Information;
		this.btn_ClosePO.ToolTipTitle = "Đóng lệnh sản xuất";
		this.btn_ClosePO.Click += new System.EventHandler(this.btn_ClosePO_Click);
		this.btn_Refresh.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Refresh.EditControl = null;
		this.btn_Refresh.EditValue = "Refresh";
		this.btn_Refresh.FieldName = null;
		this.btn_Refresh.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Refresh.ImageOptions.Image");
		this.btn_Refresh.IsSearchControl = false;
		this.btn_Refresh.Location = new System.Drawing.Point(188, 6);
		this.btn_Refresh.Name = "btn_Refresh";
		this.btn_Refresh.ReadOnly = false;
		this.btn_Refresh.SettingInVisible = false;
		this.btn_Refresh.SettingReadonly = false;
		this.btn_Refresh.Size = new System.Drawing.Size(75, 23);
		this.btn_Refresh.TabIndex = 5;
		this.btn_Refresh.Text = "Refresh";
		this.btn_Refresh.ToolTip = "Làm mới form";
		this.btn_Refresh.Click += new System.EventHandler(this.btn_Refresh_Click);
		this.btn_Pending.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Pending.EditControl = null;
		this.btn_Pending.EditValue = "Pending";
		this.btn_Pending.FieldName = null;
		this.btn_Pending.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Pending.ImageOptions.Image");
		this.btn_Pending.IsSearchControl = false;
		this.btn_Pending.Location = new System.Drawing.Point(570, 6);
		this.btn_Pending.Name = "btn_Pending";
		this.btn_Pending.ReadOnly = false;
		this.btn_Pending.SettingInVisible = false;
		this.btn_Pending.SettingReadonly = false;
		this.btn_Pending.Size = new System.Drawing.Size(86, 23);
		this.btn_Pending.TabIndex = 4;
		this.btn_Pending.Text = "Pending";
		this.btn_Pending.ToolTip = "Tạm dừng sản xuất";
		this.btn_Pending.Visible = false;
		this.btn_Pending.Click += new System.EventHandler(this.btn_Pending_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(842, 6);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 3;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.ToolTip = "Thoát";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.btn_UnRelease.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_UnRelease.EditControl = null;
		this.btn_UnRelease.EditValue = "UnRelease";
		this.btn_UnRelease.FieldName = null;
		this.btn_UnRelease.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_UnRelease.ImageOptions.Image");
		this.btn_UnRelease.IsSearchControl = false;
		this.btn_UnRelease.Location = new System.Drawing.Point(96, 6);
		this.btn_UnRelease.Name = "btn_UnRelease";
		this.btn_UnRelease.ReadOnly = false;
		this.btn_UnRelease.SettingInVisible = false;
		this.btn_UnRelease.SettingReadonly = false;
		this.btn_UnRelease.Size = new System.Drawing.Size(86, 23);
		this.btn_UnRelease.TabIndex = 2;
		this.btn_UnRelease.Text = "UnRelease";
		this.btn_UnRelease.ToolTip = "Hủy sản xuất";
		this.btn_UnRelease.Click += new System.EventHandler(this.btn_UnRelease_Click);
		this.btn_Confirm.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Confirm.EditControl = null;
		this.btn_Confirm.EditValue = "Confirm";
		this.btn_Confirm.FieldName = null;
		this.btn_Confirm.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Confirm.ImageOptions.Image");
		this.btn_Confirm.IsSearchControl = false;
		this.btn_Confirm.Location = new System.Drawing.Point(5, 6);
		this.btn_Confirm.Name = "btn_Confirm";
		this.btn_Confirm.ReadOnly = false;
		this.btn_Confirm.SettingInVisible = false;
		this.btn_Confirm.SettingReadonly = false;
		this.btn_Confirm.Size = new System.Drawing.Size(86, 23);
		this.btn_Confirm.TabIndex = 0;
		this.btn_Confirm.Text = "Confirm";
		this.btn_Confirm.ToolTip = "Báo cáo sản xuất";
		this.btn_Confirm.Click += new System.EventHandler(this.btn_Confirm_Click);
		this.splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(0, 56);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.xtraTabControl1);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.xtraTabControl2);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(922, 405);
		this.splitContainerControl1.SplitterPosition = 195;
		this.splitContainerControl1.TabIndex = 3;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.xtraTabControl2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.xtraTabControl2.IsSearchControl = false;
		this.xtraTabControl2.Location = new System.Drawing.Point(0, 0);
		this.xtraTabControl2.Name = "xtraTabControl2";
		this.xtraTabControl2.SelectedTabPage = this.xtraTabPage1;
		this.xtraTabControl2.Size = new System.Drawing.Size(922, 200);
		this.xtraTabControl2.TabIndex = 1;
		this.xtraTabControl2.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[1] { this.xtraTabPage1 });
		this.xtraTabPage1.Controls.Add(this.grd_ReceiptBatch);
		this.xtraTabPage1.EditControl = null;
		this.xtraTabPage1.EditValue = "Receipt Batch";
		this.xtraTabPage1.FieldName = null;
		this.xtraTabPage1.IsSearchControl = false;
		this.xtraTabPage1.Name = "xtraTabPage1";
		this.xtraTabPage1.ReadOnly = false;
		this.xtraTabPage1.SettingInVisible = false;
		this.xtraTabPage1.SettingReadonly = false;
		this.xtraTabPage1.Size = new System.Drawing.Size(920, 175);
		this.xtraTabPage1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPage1.Text = "Receipt Batch";
		this.grd_ReceiptBatch.AllowPasteExcel = true;
		this.grd_ReceiptBatch.ClipboardData = null;
		this.grd_ReceiptBatch.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_ReceiptBatch.Location = new System.Drawing.Point(0, 0);
		this.grd_ReceiptBatch.MainView = this.gv_ReceiptBatch;
		this.grd_ReceiptBatch.Name = "grd_ReceiptBatch";
		this.grd_ReceiptBatch.Size = new System.Drawing.Size(920, 175);
		this.grd_ReceiptBatch.TabIndex = 0;
		this.grd_ReceiptBatch.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_ReceiptBatch });
		this.gv_ReceiptBatch.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_ReceiptBatch.ColumnKey = null;
		this.gv_ReceiptBatch.DisableFormSetting = false;
		this.gv_ReceiptBatch.DisplayException = false;
		this.gv_ReceiptBatch.EditColumn = null;
		this.gv_ReceiptBatch.FieldCurrency = null;
		this.gv_ReceiptBatch.FieldRate = null;
		this.gv_ReceiptBatch.FormId = "oProductionConfirm";
		this.gv_ReceiptBatch.GridControl = this.grd_ReceiptBatch;
		this.gv_ReceiptBatch.IgnoreFilter = false;
		this.gv_ReceiptBatch.IndexEditRow = -1;
		this.gv_ReceiptBatch.IndicatorWidth = 35;
		this.gv_ReceiptBatch.IsApzTable = false;
		this.gv_ReceiptBatch.IsInsertNewRow = false;
		this.gv_ReceiptBatch.IsLoadingData = false;
		this.gv_ReceiptBatch.IsMappingContextAction = false;
		this.gv_ReceiptBatch.ItemId = "OBTN";
		this.gv_ReceiptBatch.LstColumnAllowBatch = null;
		this.gv_ReceiptBatch.LstColumnNotAllowBatch = null;
		this.gv_ReceiptBatch.LstColumnNotEditable = null;
		this.gv_ReceiptBatch.LstColumNotSetting = null;
		this.gv_ReceiptBatch.Name = "gv_ReceiptBatch";
		this.gv_ReceiptBatch.UseBatchMod = false;
		this.gv_ReceiptBatch.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(this.gv_ReceiptBatch_PopupMenuShowing);
		this.gv_ReceiptBatch.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_ReceiptBatch_ShowingEditor);
		this.gv_ReceiptBatch.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_ReceiptBatch_CellValueChanged);
		this.timer_LoadForm.Enabled = true;
		this.timer_LoadForm.Interval = 1000;
		this.timer_LoadForm.Tick += new System.EventHandler(this.timer_LoadForm_Tick);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(922, 494);
		base.Controls.Add(this.splitContainerControl1);
		base.Controls.Add(this.panelControl2);
		base.Controls.Add(this.panelControl1);
		base.FormId = "oProductionConfirm";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_ProductionConfirm";
		base.ShowInTaskbar = false;
		this.Text = "Production Confirm";
		base.Load += new System.EventHandler(this.frm_ProductionConfirm_Load);
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).EndInit();
		this.xtraTabControl1.ResumeLayout(false);
		this.tab_ProductionControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_ProductionControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_ProductionControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl1).EndInit();
		this.panelControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.date_ToDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_ToDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_FromDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_FromDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_IssueBatch.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bte_WorkCenter.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chb_Status.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Barcode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.spin_Timer.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Timer.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl2).EndInit();
		this.panelControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl2).EndInit();
		this.xtraTabControl2.ResumeLayout(false);
		this.xtraTabPage1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_ReceiptBatch).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_ReceiptBatch).EndInit();
		base.ResumeLayout(false);
	}
}
