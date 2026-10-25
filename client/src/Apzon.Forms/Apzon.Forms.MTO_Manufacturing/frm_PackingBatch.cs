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
using Apzon.Entities.Models.Production;
using Apzon.Entities.Models.UserFields;
using Apzon.Forms.Commons;
using Apzon.Forms.Helper;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTab;
using SAPbobsCOM;

namespace Apzon.Forms.MTO_Manufacturing;

public class frm_PackingBatch : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private List<UserFieldsImpl> lstUdf = new List<UserFieldsImpl>();

	private IContainer components;

	private LabelLine labelLine1;

	private TextEditEx txt_BatchNo;

	private SplitContainerControl splitContainerControl1;

	private XtraTabControlEx xtraTabControl1;

	private XtraTabPageEx xtraTabPage1;

	private GridControlEx grd_ListOfBatch;

	private GridViewEx gv_ListOfBatch;

	private XtraTabControlEx xtraTabControl2;

	private XtraTabPageEx xtraTabPage2;

	private GridControlEx grd_Result;

	private GridViewEx gv_Result;

	private SimpleButtonEx btn_Packing;

	private CalcEditEx cal_Qty;

	private LabelLine labelLine2;

	private SimpleButtonEx btn_Print;

	private SimpleButtonEx btn_Clear;

	private TextEditEx txt_DocNo;

	private LabelLine labelLine3;

	private SimpleButtonEx btn_Confirm;

	private CheckEditEx chk_PackLast;

	private CheckEditEx chk_PackQty;

	public int PwNo { get; set; }

	public frm_PackingBatch()
	{
		InitializeComponent();
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

	private async void txt_DocNo_KeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.Return && !string.IsNullOrEmpty(txt_DocNo.Text))
			{
				int type = 5;
				int num = Function.ParseInt(txt_DocNo.EditValue);
				if (num > 0)
				{
					await Navigate(num, type);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task Navigate(int currentId, int type)
	{
		WinCommons._OpenProcessing();
		try
		{
			int num = ((type != 5) ? (await GetPage(currentId, type)) : currentId);
			int id = num;
			txt_DocNo.EditValue = id;
			DataSet ds = await _httpClient.PackingBatch.GetIssue(id);
			if (ds.IsHasValidValue())
			{
				await GridViewHelper.GenerateGrid(grd_ListOfBatch, gv_ListOfBatch, ds, lstUdf);
			}
			ds.Tables.Clear();
			ds.Dispose();
			DataSet dsResult = await _httpClient.PackingBatch.GetReceipt(id);
			if (dsResult.IsHasValidValue())
			{
				await GridViewHelper.GenerateGrid(grd_Result, gv_Result, dsResult, lstUdf);
			}
			dsResult.Tables.Clear();
			dsResult.Dispose();
			btn_Packing.Enabled = false;
			btn_Confirm.Enabled = false;
			txt_BatchNo.Text = "";
			txt_BatchNo.Focus();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private async Task<int> GetPage(int current, int type)
	{
		try
		{
			return await _httpClient.PackingBatch.GetPage(current, type, GlobalData.UserName, GlobalData.DefaultBranch);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return 0;
	}

	private async void frm_PackingBatch_Load(object sender, EventArgs e)
	{
		try
		{
			lstUdf = await _httpClient.Udf.GetListField("OBTN");
			btn_Packing.Enabled = false;
			btn_Confirm.Enabled = false;
			txt_BatchNo.Text = "";
			txt_BatchNo.Focus();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void txt_BatchNo_KeyDown(object sender, KeyEventArgs e)
	{
		_ = 2;
		try
		{
			if (e.KeyCode != Keys.Return)
			{
				return;
			}
			WinCommons._OpenProcessing();
			if (!string.IsNullOrEmpty(Function.ToString(txt_DocNo.Text)))
			{
				txt_DocNo.Text = "";
				grd_ListOfBatch.DataSource = null;
				grd_ListOfBatch.RefreshDataSource();
				grd_Result.DataSource = null;
				grd_Result.RefreshDataSource();
			}
			string text = Function.ToString(txt_BatchNo.EditValue);
			if (grd_ListOfBatch.DataSource != null)
			{
				DataTable dataTable = grd_ListOfBatch.DataSource as DataTable;
				if (dataTable.ExistsColumn("U_DistNumber") && dataTable != null && dataTable.Select($"U_DistNumber='{text}'").Length != 0)
				{
					WinCommons.Error(Enums.MessageCode.None, "Đã tồn tại \"Batch\" trong lưới dữ liệu");
					txt_BatchNo.Text = "";
					txt_BatchNo.Focus();
					return;
				}
			}
			DataSet ds = await _httpClient.PackingBatch.GetBatch(text);
			if (ds.IsHasValidValue())
			{
				if (gv_ListOfBatch.Columns.Count <= 0)
				{
					await GridViewHelper.GenerateGrid(grd_ListOfBatch, gv_ListOfBatch, ds, lstUdf);
					await GridViewHelper.GenerateGrid(grd_Result, gv_Result, ds.Tables[0], ds.Tables[1].Clone(), ds.Tables[2], lstUdf);
				}
				else if (grd_ListOfBatch.DataSource != null && grd_ListOfBatch.DataSourceTable.IsNotNull() && grd_ListOfBatch.DataSourceTable.Rows.Count > 0)
				{
					DataTable dataTable2 = (DataTable)grd_ListOfBatch.DataSource;
					DataTable dataTable3 = ds.Tables[1];
					if (dataTable3.IsNotNull())
					{
						string text2 = Function.ToString(dataTable3.Rows[0]["U_ItemCode"]);
						string text3 = Function.ToString(dataTable3.Rows[0]["U_UnitUoM"]);
						decimal num = Function.ParseDecimal(dataTable3.Rows[0]["U_Width"]);
						decimal num2 = Function.ParseDecimal(dataTable3.Rows[0]["U_Length"]);
						string text4 = Function.ToString(dataTable2.Rows[0]["U_ItemCode"]);
						string text5 = Function.ToString(dataTable2.Rows[0]["U_UnitUoM"]);
						decimal num3 = Function.ParseDecimal(dataTable2.Rows[0]["U_Width"]);
						decimal num4 = Function.ParseDecimal(dataTable2.Rows[0]["U_Length"]);
						if (!text2.Equals(text4))
						{
							XtraMessageBox.Show($"Mã hàng không hợp lệ. Mã hàng hiện tại là {text4}", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Hand);
							txt_BatchNo.Text = "";
							txt_BatchNo.Focus();
							return;
						}
						if (!text3.Equals(text5))
						{
							XtraMessageBox.Show($"Đơn vị tính không hợp lệ. Đơn vị hiện tại là {text5}", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Hand);
							txt_BatchNo.Text = "";
							txt_BatchNo.Focus();
							return;
						}
						if (text3.Equals("C") && num != num3)
						{
							XtraMessageBox.Show($"Khổ không hợp lệ. Khổ hiện tại là {num3}", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Hand);
							txt_BatchNo.Text = "";
							txt_BatchNo.Focus();
							return;
						}
						if (text3.Equals("T") && ((num != num3 && num != num4 * 100m) || (num2 != num4 && num2 * 100m != num3)))
						{
							XtraMessageBox.Show($"Quy cách không hợp lệ. Quy cách hiện tại là {num3} x {num4}", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Hand);
							txt_BatchNo.Text = "";
							txt_BatchNo.Focus();
							return;
						}
					}
					foreach (DataRow row in ds.Tables[1].Rows)
					{
						DataRow dataRow2 = dataTable2.NewRow();
						dataRow2.ItemArray = row.ItemArray;
						dataTable2.Rows.Add(dataRow2);
					}
					dataTable2.AcceptChanges();
				}
				else
				{
					grd_ListOfBatch.DataSource = ds.Tables[1];
					grd_ListOfBatch.RefreshDataSource();
				}
				btn_Packing.Enabled = true;
				btn_Confirm.Enabled = false;
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, "Không có dữ liệu");
			}
			txt_BatchNo.Text = "";
			txt_BatchNo.Focus();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Packing_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataSourceTable = grd_ListOfBatch.DataSourceTable;
			if (!dataSourceTable.IsNotNull())
			{
				return;
			}
			WinCommons._OpenProcessing();
			grd_Result.DataSource = null;
			grd_Result.RefreshDataSource();
			if (Function.ToString(dataSourceTable.Rows[0]["U_UnitUoM"]).ToUpper().Equals("T") || chk_PackQty.Checked)
			{
				int num = Function.ParseInt(cal_Qty.EditValue);
				if (num <= 0)
				{
					XtraMessageBox.Show("Chọn số lượng \"Packing Quantity\" lớn hơn 0");
					cal_Qty.EditValue = null;
					cal_Qty.Focus();
					return;
				}
				int num2 = Function.ParseInt(dataSourceTable.Compute("Sum(U_QtyUoM)", ""));
				if (num > 0)
				{
					DataRow dataRow = dataSourceTable.Rows[0];
					DataTable dataTable = grd_Result.DataSourceTable;
					if (!dataTable.IsNotNull())
					{
						dataTable = grd_ListOfBatch.DataSourceTable.Clone();
					}
					int num3 = 0;
					while (num2 > 0)
					{
						num = ((num > num2) ? num2 : num);
						num2 -= num;
						if (num2 < num && chk_PackLast.Checked)
						{
							num = num2 + num;
							num2 = 0;
						}
						DataRow dataRow2 = dataTable.NewRow();
						dataRow2.ItemArray = dataRow.ItemArray;
						dataRow2["U_QtyUoM"] = num;
						dataRow2["U_DistNumber"] = string.Format("{0}-{1}", Function.ToString(dataRow["U_DistNumber"]), num3);
						num3++;
						dataRow2["U_Qty"] = Math.Round(Function.ParseDouble(dataRow["U_Width"]) * Function.ParseDouble(dataRow["U_Length"]) * (double)num / 100.0, 3, MidpointRounding.AwayFromZero);
						dataTable.Rows.Add(dataRow2);
					}
					grd_Result.DataSource = dataTable;
					grd_Result.RefreshDataSource();
				}
			}
			else
			{
				double num4 = Function.ParseDouble(cal_Qty.EditValue);
				double num5 = Function.ParseDouble(dataSourceTable.Compute("Sum(U_Length)", ""));
				DataTable dataTable2 = grd_Result.DataSourceTable;
				if (!dataTable2.IsNotNull())
				{
					dataTable2 = grd_ListOfBatch.DataSourceTable.Clone();
				}
				DataRow dataRow3 = dataSourceTable.Rows[0];
				if (num4 <= 0.0 || num4 > num5)
				{
					DataRow dataRow4 = dataTable2.NewRow();
					dataRow4.ItemArray = dataRow3.ItemArray;
					dataRow4["U_DistNumber"] = string.Format("{0}-{1}", Function.ToString(dataRow3["U_DistNumber"]), 0);
					dataRow4["U_Length"] = num5;
					dataRow4["U_Qty"] = Math.Round(Function.ParseDouble(dataRow3["U_Width"]) * Function.ParseDouble(dataRow3["U_QtyUoM"]) * num5 / 100.0, 3, MidpointRounding.AwayFromZero);
					dataTable2.Rows.Add(dataRow4);
				}
				else
				{
					int num6 = 0;
					while (num5 > 0.0)
					{
						num4 = ((num4 > num5) ? num5 : num4);
						num5 -= num4;
						if (num5 < num4 && chk_PackLast.Checked)
						{
							num4 = num5 + num4;
							num5 = 0.0;
						}
						DataRow dataRow5 = dataTable2.NewRow();
						dataRow5.ItemArray = dataRow3.ItemArray;
						dataRow5["U_DistNumber"] = string.Format("{0}-{1}", Function.ToString(dataRow3["U_DistNumber"]), num6);
						num6++;
						dataRow5["U_Length"] = num4;
						dataRow5["U_Qty"] = Math.Round(Function.ParseDouble(dataRow3["U_Width"]) * Function.ParseDouble(dataRow3["U_QtyUoM"]) * num4 / 100.0, 3, MidpointRounding.AwayFromZero);
						dataTable2.Rows.Add(dataRow5);
					}
				}
				grd_Result.DataSource = dataTable2;
				grd_Result.RefreshDataSource();
			}
			btn_Confirm.Enabled = true;
			btn_Confirm.Focus();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Confirm_Click(object sender, EventArgs e)
	{
		_ = 2;
		try
		{
			DataTable dtIssue = grd_ListOfBatch.DataSourceTable;
			DataTable dtReceipt = grd_Result.DataSourceTable;
			if ((!dtIssue.IsNotNull() && !dtReceipt.IsNotNull()) || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No || (dtReceipt.Select("(U_UnitUoM = 'T' AND U_QtyUoM <= 10) OR (U_UnitUoM = 'C' AND U_Length <= 10 OR U_Qty < 2)").Length != 0 && XtraMessageBox.Show("Có batch số lượng quá ít. Bạn có chắc chắn nhập?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No))
			{
				return;
			}
			btn_Confirm.Enabled = false;
			WinCommons._OpenProcessing();
			string value = string.Join(";", from t in dtIssue.AsEnumerable()
				select Function.ToString(t["U_DistNumber"]));
			string value2 = string.Join(";", from t in dtReceipt.AsEnumerable()
				select Function.ToString(t["U_DistNumber"]));
			string itemcode = Function.ToString(dtIssue.Rows[0]["U_ItemCode"]);
			string whs = Function.ToString(dtIssue.Rows[0]["U_Whs"]);
			double planqty = dtIssue.AsEnumerable().Sum((DataRow t) => Function.ParseDouble(t["U_Qty"]));
			ProductionOder_Impl_OWOR production = new ProductionOder_Impl_OWOR();
			production.DueDate = DateTime.Today;
			production.ItemCode = itemcode;
			production.PlannedQty = planqty;
			production.Warehouse = whs;
			production.OriginAbs = Function.ParseInt(dtIssue.Rows[0]["U_DocEntry"]);
			production.OriginType = "S";
			production.UserFields = new List<UserFieldsImpl>();
			production.UserFields.Add(new UserFieldsImpl
			{
				Name = "U_IssBatchP",
				Value = value
			});
			production.UserFields.Add(new UserFieldsImpl
			{
				Name = "U_ReBatch",
				Value = value2
			});
			production.UserFields.Add(new UserFieldsImpl
			{
				Name = "U_Packing",
				Value = "Y"
			});
			production.UserFields.Add(new UserFieldsImpl
			{
				Name = "U_IssuedQty",
				Type = BoFieldTypes.db_Numeric,
				Value = planqty
			});
			production.Line_WOR1 = new List<ProductionOder_LineImpl_WOR1>();
			DataTable dataTable = await _httpClient.BillOfMasterials.GetLineByFather(itemcode, GlobalData.UserName, GlobalData.DefaultBranch);
			if (dataTable.IsNotNull())
			{
				for (int num = 0; num < dataTable.Rows.Count; num++)
				{
					ProductionOder_LineImpl_WOR1 productionOder_LineImpl_WOR = new ProductionOder_LineImpl_WOR1();
					productionOder_LineImpl_WOR.ItemType = Function.ToString(dataTable.Rows[num]["Type"]);
					productionOder_LineImpl_WOR.ItemCode = Function.ToString(dataTable.Rows[num]["Code"]);
					productionOder_LineImpl_WOR.wareHouse = Function.ToString(dataTable.Rows[num]["Warehouse"]);
					productionOder_LineImpl_WOR.PlannedQty = Function.ParseDouble(dataTable.Rows[num]["RateQty"]) * production.PlannedQty;
					production.Line_WOR1.Add(productionOder_LineImpl_WOR);
				}
			}
			DocumentsImpl documentsImpl = new DocumentsImpl();
			documentsImpl.BPLId = Function.ParseInt(GlobalData.DefaultBranch);
			documentsImpl.Lines = new List<Document_LinesImpl>();
			Document_LinesImpl document_LinesImpl = new Document_LinesImpl();
			document_LinesImpl.ItemCode = itemcode;
			document_LinesImpl.Quantity = planqty;
			document_LinesImpl.WhsCode = whs;
			document_LinesImpl.BatchNumbers = new List<BatchNumbersImpl>();
			document_LinesImpl.BinAllocations = new List<DocumentLinesBinAllocationsImpl>();
			for (int num2 = 0; num2 < dtIssue.Rows.Count; num2++)
			{
				BatchNumbersImpl batchNumbersImpl = new BatchNumbersImpl();
				batchNumbersImpl.Quantity = Function.ParseDouble(dtIssue.Rows[num2]["U_Qty"]);
				batchNumbersImpl.BatchNumber = Function.ToString(dtIssue.Rows[num2]["U_DistNumber"]);
				document_LinesImpl.BatchNumbers.Add(batchNumbersImpl);
				DocumentLinesBinAllocationsImpl documentLinesBinAllocationsImpl = new DocumentLinesBinAllocationsImpl();
				documentLinesBinAllocationsImpl.BaseLineNumber = 0;
				documentLinesBinAllocationsImpl.BinAbsEntry = Function.ParseInt(dtIssue.Rows[num2]["U_BinId"]);
				documentLinesBinAllocationsImpl.Quantity = batchNumbersImpl.Quantity;
				documentLinesBinAllocationsImpl.SerialAndBatchNumbersBaseLine = num2;
				if (documentLinesBinAllocationsImpl.BinAbsEntry > 0)
				{
					document_LinesImpl.BinAllocations.Add(documentLinesBinAllocationsImpl);
				}
			}
			documentsImpl.Lines.Add(document_LinesImpl);
			DocumentsImpl documentsImpl2 = new DocumentsImpl();
			documentsImpl2.BPLId = Function.ParseInt(GlobalData.DefaultBranch);
			if (dtReceipt.IsNotNull())
			{
				documentsImpl2.Lines = new List<Document_LinesImpl>();
				Document_LinesImpl document_LinesImpl2 = new Document_LinesImpl();
				double num3 = 0.0;
				document_LinesImpl2.WhsCode = Function.ToString(dtReceipt.Rows[0]["U_Whs"]);
				document_LinesImpl2.BaseType = 202;
				document_LinesImpl2.BaseLine = -1;
				document_LinesImpl2.BatchNumbers = new List<BatchNumbersImpl>();
				document_LinesImpl2.BinAllocations = new List<DocumentLinesBinAllocationsImpl>();
				for (int num4 = 0; num4 < dtReceipt.Rows.Count; num4++)
				{
					List<UserFieldsImpl> list = new List<UserFieldsImpl>();
					foreach (UserFieldsImpl item in lstUdf)
					{
						string text = "U_" + item.Name;
						if (dtReceipt.Rows[num4].IsExistsColumn(text))
						{
							UserFieldsImpl userFieldsImpl = new UserFieldsImpl();
							userFieldsImpl.Name = item.Name;
							userFieldsImpl.Type = item.Type;
							userFieldsImpl.SubType = item.SubType;
							userFieldsImpl.Value = dtReceipt.Rows[num4][text];
							list.Add(userFieldsImpl);
						}
					}
					BatchNumbersImpl batchNumbersImpl2 = new BatchNumbersImpl();
					batchNumbersImpl2.UserFields = list;
					batchNumbersImpl2.Quantity = Function.ParseDouble(dtReceipt.Rows[num4]["U_Qty"]);
					num3 += batchNumbersImpl2.Quantity;
					batchNumbersImpl2.BatchNumber = Function.ToString(dtReceipt.Rows[num4]["U_DistNumber"]);
					batchNumbersImpl2.InternalSerialNumber = Function.ToString(dtReceipt.Rows[num4]["LotNumber"]);
					batchNumbersImpl2.AddmisionDate = Function.ParseDateTime(dtReceipt.Rows[num4]["InDate"]);
					document_LinesImpl2.BatchNumbers.Add(batchNumbersImpl2);
					DocumentLinesBinAllocationsImpl documentLinesBinAllocationsImpl2 = new DocumentLinesBinAllocationsImpl();
					documentLinesBinAllocationsImpl2.BaseLineNumber = 0;
					documentLinesBinAllocationsImpl2.BinAbsEntry = Function.ParseInt(dtReceipt.Rows[num4]["U_BinId"]);
					documentLinesBinAllocationsImpl2.Quantity = batchNumbersImpl2.Quantity;
					documentLinesBinAllocationsImpl2.SerialAndBatchNumbersBaseLine = num4;
					if (documentLinesBinAllocationsImpl2.BinAbsEntry > 0)
					{
						document_LinesImpl2.BinAllocations.Add(documentLinesBinAllocationsImpl2);
					}
				}
				document_LinesImpl2.Quantity = num3;
				documentsImpl2.Lines.Add(document_LinesImpl2);
			}
			HttpResult httpResult = await _httpClient.ProductionOrders.PackingBatch(new Tuple<DocumentsImpl, DocumentsImpl, ProductionOder_Impl_OWOR>(documentsImpl, documentsImpl2, production));
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				btn_Confirm.Enabled = true;
				return;
			}
			string[] array = Function.ToString(httpResult.Content).Split(',');
			if (array.Length != 0)
			{
				PwNo = Function.ParseInt(array[0]);
			}
			txt_DocNo.Text = PwNo.ToString();
			WinCommons.Success(Enums.MessageCode.Success, "");
			await PRINT();
			cal_Qty.EditValue = null;
			grd_ListOfBatch.DataSource = null;
			grd_ListOfBatch.RefreshDataSource();
			grd_Result.DataSource = null;
			grd_Result.RefreshDataSource();
			btn_Packing.Enabled = false;
			btn_Confirm.Enabled = false;
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			btn_Confirm.Enabled = true;
		}
	}

	private async void btn_Print_Click(object sender, EventArgs e)
	{
		await PRINT();
	}

	private void btn_Clear_Click(object sender, EventArgs e)
	{
		try
		{
			txt_BatchNo.Text = string.Empty;
			cal_Qty.EditValue = null;
			grd_ListOfBatch.DataSource = null;
			grd_ListOfBatch.RefreshDataSource();
			grd_Result.DataSource = null;
			grd_Result.RefreshDataSource();
			txt_BatchNo.Focus();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Result_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if ("U_Width".Equals(e.Column.FieldName) || "U_Length".Equals(e.Column.FieldName) || "U_QtyUoM".Equals(e.Column.FieldName))
			{
				double num = Function.ParseDouble(gv_Result.GetRowCellValue(e.RowHandle, "U_Width"));
				double num2 = Function.ParseDouble(gv_Result.GetRowCellValue(e.RowHandle, "U_Length"));
				double num3 = Function.ParseDouble(gv_Result.GetRowCellValue(e.RowHandle, "U_QtyUoM"));
				double num4 = Math.Round(num * num2 * num3 / 100.0, 3, MidpointRounding.AwayFromZero);
				gv_Result.SetRowCellValue(e.RowHandle, "U_Qty", num4);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_ListOfBatch_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if ("U_Width".Equals(e.Column.FieldName) || "U_Length".Equals(e.Column.FieldName) || "U_QtyUoM".Equals(e.Column.FieldName))
			{
				double num = Function.ParseDouble(gv_ListOfBatch.GetRowCellValue(e.RowHandle, "U_Width"));
				double num2 = Function.ParseDouble(gv_ListOfBatch.GetRowCellValue(e.RowHandle, "U_Length"));
				double num3 = Function.ParseDouble(gv_ListOfBatch.GetRowCellValue(e.RowHandle, "U_QtyUoM"));
				double num4 = Math.Round(num * num2 * num3 / 100.0, 3, MidpointRounding.AwayFromZero);
				gv_ListOfBatch.SetRowCellValue(e.RowHandle, "U_Qty", num4);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async void PRINTLAYOUT()
	{
		try
		{
			int num = ((PwNo > 0) ? PwNo : Function.ParseInt(txt_DocNo.Text));
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
		try
		{
			int num = ((PwNo > 0) ? PwNo : Function.ParseInt(txt_DocNo.Text));
			if (num > 0)
			{
				await WinFuntions.Print("", num, "WOR1");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Result_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
	{
	}

	private void docMenu_Click(object sender, EventArgs e)
	{
		try
		{
			if ((sender as DXMenuItem).Caption.Equals("Remove Row") && gv_Result.FocusedRowHandle > -1 && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				WinCommons._OpenProcessing();
				gv_Result.DeleteRow(gv_Result.FocusedRowHandle);
				WinCommons._CloseProcessing();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void cal_Qty_KeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.Return)
			{
				btn_Packing_Click(null, null);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.MTO_Manufacturing.frm_PackingBatch));
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.txt_BatchNo = new Apzon.Controls.Editor.TextEditEx();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.xtraTabControl1 = new Apzon.Controls.Controls.XtraTabControlEx();
		this.xtraTabPage1 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_ListOfBatch = new Apzon.Controls.Grid.GridControlEx();
		this.gv_ListOfBatch = new Apzon.Controls.Grid.GridViewEx();
		this.xtraTabControl2 = new Apzon.Controls.Controls.XtraTabControlEx();
		this.xtraTabPage2 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_Result = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Result = new Apzon.Controls.Grid.GridViewEx();
		this.btn_Packing = new Apzon.Controls.Controls.SimpleButtonEx();
		this.cal_Qty = new Apzon.Controls.Editor.CalcEditEx();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.btn_Print = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Clear = new Apzon.Controls.Controls.SimpleButtonEx();
		this.txt_DocNo = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.btn_Confirm = new Apzon.Controls.Controls.SimpleButtonEx();
		this.chk_PackLast = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_PackQty = new Apzon.Controls.Editor.CheckEditEx();
		((System.ComponentModel.ISupportInitialize)this.txt_BatchNo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).BeginInit();
		this.xtraTabControl1.SuspendLayout();
		this.xtraTabPage1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_ListOfBatch).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_ListOfBatch).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl2).BeginInit();
		this.xtraTabControl2.SuspendLayout();
		this.xtraTabPage2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Result).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Result).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cal_Qty.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_DocNo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_PackLast.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_PackQty.Properties).BeginInit();
		base.SuspendLayout();
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Batch No.";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(13, 15);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(138, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 0;
		this.labelLine1.Utext = "Batch No.";
		this.txt_BatchNo.EditControl = null;
		this.txt_BatchNo.FieldName = null;
		this.txt_BatchNo.IsSearchControl = false;
		this.txt_BatchNo.Location = new System.Drawing.Point(148, 13);
		this.txt_BatchNo.Name = "txt_BatchNo";
		this.txt_BatchNo.SettingInVisible = false;
		this.txt_BatchNo.SettingReadonly = false;
		this.txt_BatchNo.Size = new System.Drawing.Size(338, 20);
		this.txt_BatchNo.TabIndex = 1;
		this.txt_BatchNo.ToolTip = "Bắn barcode cuộn hàng cần packing";
		this.txt_BatchNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_BatchNo_KeyDown);
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(5, 65);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.xtraTabControl1);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.xtraTabControl2);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(960, 555);
		this.splitContainerControl1.SplitterPosition = 244;
		this.splitContainerControl1.TabIndex = 4;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.xtraTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.xtraTabControl1.IsSearchControl = false;
		this.xtraTabControl1.Location = new System.Drawing.Point(0, 0);
		this.xtraTabControl1.Name = "xtraTabControl1";
		this.xtraTabControl1.SelectedTabPage = this.xtraTabPage1;
		this.xtraTabControl1.Size = new System.Drawing.Size(960, 244);
		this.xtraTabControl1.TabIndex = 0;
		this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[1] { this.xtraTabPage1 });
		this.xtraTabPage1.Controls.Add(this.grd_ListOfBatch);
		this.xtraTabPage1.EditControl = null;
		this.xtraTabPage1.EditValue = "List Of Batch";
		this.xtraTabPage1.FieldName = null;
		this.xtraTabPage1.IsSearchControl = false;
		this.xtraTabPage1.Name = "xtraTabPage1";
		this.xtraTabPage1.ReadOnly = false;
		this.xtraTabPage1.SettingInVisible = false;
		this.xtraTabPage1.SettingReadonly = false;
		this.xtraTabPage1.Size = new System.Drawing.Size(958, 219);
		this.xtraTabPage1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPage1.Text = "List Of Batch";
		this.grd_ListOfBatch.AllowPasteExcel = true;
		this.grd_ListOfBatch.ClipboardData = null;
		this.grd_ListOfBatch.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_ListOfBatch.Location = new System.Drawing.Point(0, 0);
		this.grd_ListOfBatch.MainView = this.gv_ListOfBatch;
		this.grd_ListOfBatch.Name = "grd_ListOfBatch";
		this.grd_ListOfBatch.Size = new System.Drawing.Size(958, 219);
		this.grd_ListOfBatch.TabIndex = 0;
		this.grd_ListOfBatch.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_ListOfBatch });
		this.gv_ListOfBatch.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_ListOfBatch.ColumnKey = null;
		this.gv_ListOfBatch.DisableFormSetting = false;
		this.gv_ListOfBatch.DisplayException = false;
		this.gv_ListOfBatch.EditColumn = null;
		this.gv_ListOfBatch.FieldCurrency = null;
		this.gv_ListOfBatch.FieldRate = null;
		this.gv_ListOfBatch.FormId = "oPackingBatch";
		this.gv_ListOfBatch.GridControl = this.grd_ListOfBatch;
		this.gv_ListOfBatch.IgnoreFilter = false;
		this.gv_ListOfBatch.IndexEditRow = -1;
		this.gv_ListOfBatch.IndicatorWidth = 35;
		this.gv_ListOfBatch.IsApzTable = false;
		this.gv_ListOfBatch.IsInsertNewRow = false;
		this.gv_ListOfBatch.IsLoadingData = false;
		this.gv_ListOfBatch.IsMappingContextAction = false;
		this.gv_ListOfBatch.ItemId = "OLOB";
		this.gv_ListOfBatch.LstColumnAllowBatch = null;
		this.gv_ListOfBatch.LstColumnNotAllowBatch = null;
		this.gv_ListOfBatch.LstColumnNotEditable = null;
		this.gv_ListOfBatch.LstColumNotSetting = null;
		this.gv_ListOfBatch.Name = "gv_ListOfBatch";
		this.gv_ListOfBatch.OptionsSelection.MultiSelect = true;
		this.gv_ListOfBatch.UseBatchMod = false;
		this.gv_ListOfBatch.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_ListOfBatch_CellValueChanged);
		this.xtraTabControl2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.xtraTabControl2.IsSearchControl = false;
		this.xtraTabControl2.Location = new System.Drawing.Point(0, 0);
		this.xtraTabControl2.Name = "xtraTabControl2";
		this.xtraTabControl2.SelectedTabPage = this.xtraTabPage2;
		this.xtraTabControl2.Size = new System.Drawing.Size(960, 301);
		this.xtraTabControl2.TabIndex = 1;
		this.xtraTabControl2.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[1] { this.xtraTabPage2 });
		this.xtraTabPage2.Controls.Add(this.grd_Result);
		this.xtraTabPage2.EditControl = null;
		this.xtraTabPage2.EditValue = "Result";
		this.xtraTabPage2.FieldName = null;
		this.xtraTabPage2.IsSearchControl = false;
		this.xtraTabPage2.Name = "xtraTabPage2";
		this.xtraTabPage2.ReadOnly = false;
		this.xtraTabPage2.SettingInVisible = false;
		this.xtraTabPage2.SettingReadonly = false;
		this.xtraTabPage2.Size = new System.Drawing.Size(958, 276);
		this.xtraTabPage2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPage2.Text = "Result";
		this.grd_Result.AllowPasteExcel = true;
		this.grd_Result.ClipboardData = null;
		this.grd_Result.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Result.Location = new System.Drawing.Point(0, 0);
		this.grd_Result.MainView = this.gv_Result;
		this.grd_Result.Name = "grd_Result";
		this.grd_Result.Size = new System.Drawing.Size(958, 276);
		this.grd_Result.TabIndex = 0;
		this.grd_Result.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Result });
		this.gv_Result.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Result.ColumnKey = null;
		this.gv_Result.DisableFormSetting = false;
		this.gv_Result.DisplayException = false;
		this.gv_Result.EditColumn = null;
		this.gv_Result.FieldCurrency = null;
		this.gv_Result.FieldRate = null;
		this.gv_Result.FormId = "oPackingBatch";
		this.gv_Result.GridControl = this.grd_Result;
		this.gv_Result.IgnoreFilter = false;
		this.gv_Result.IndexEditRow = -1;
		this.gv_Result.IndicatorWidth = 35;
		this.gv_Result.IsApzTable = false;
		this.gv_Result.IsInsertNewRow = false;
		this.gv_Result.IsLoadingData = false;
		this.gv_Result.IsMappingContextAction = false;
		this.gv_Result.ItemId = "LOB1";
		this.gv_Result.LstColumnAllowBatch = null;
		this.gv_Result.LstColumnNotAllowBatch = null;
		this.gv_Result.LstColumnNotEditable = null;
		this.gv_Result.LstColumNotSetting = null;
		this.gv_Result.Name = "gv_Result";
		this.gv_Result.UseBatchMod = false;
		this.gv_Result.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(this.gv_Result_PopupMenuShowing);
		this.gv_Result.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Result_CellValueChanged);
		this.btn_Packing.EditControl = null;
		this.btn_Packing.EditValue = "Packing";
		this.btn_Packing.FieldName = null;
		this.btn_Packing.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Packing.ImageOptions.Image");
		this.btn_Packing.IsSearchControl = false;
		this.btn_Packing.Location = new System.Drawing.Point(492, 33);
		this.btn_Packing.Name = "btn_Packing";
		this.btn_Packing.ReadOnly = false;
		this.btn_Packing.SettingInVisible = false;
		this.btn_Packing.SettingReadonly = false;
		this.btn_Packing.Size = new System.Drawing.Size(84, 23);
		this.btn_Packing.TabIndex = 5;
		this.btn_Packing.Text = "Packing";
		this.btn_Packing.ToolTip = "Tính toán kết quả";
		this.btn_Packing.Click += new System.EventHandler(this.btn_Packing_Click);
		this.cal_Qty.EditControl = null;
		this.cal_Qty.FieldName = null;
		this.cal_Qty.IsSearchControl = false;
		this.cal_Qty.Location = new System.Drawing.Point(148, 36);
		this.cal_Qty.Name = "cal_Qty";
		this.cal_Qty.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cal_Qty.Properties.IsSearchControl = false;
		this.cal_Qty.SettingInVisible = false;
		this.cal_Qty.SettingReadonly = false;
		this.cal_Qty.Size = new System.Drawing.Size(338, 20);
		this.cal_Qty.TabIndex = 7;
		this.cal_Qty.ToolTip = "Số lượng tờ 1 gói";
		this.cal_Qty.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cal_Qty_KeyDown);
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "Package Quantity";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(13, 39);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(138, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 6;
		this.labelLine2.Utext = "Package Quantity";
		this.btn_Print.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Print.EditControl = null;
		this.btn_Print.EditValue = "Print";
		this.btn_Print.FieldName = null;
		this.btn_Print.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Print.ImageOptions.Image");
		this.btn_Print.IsSearchControl = false;
		this.btn_Print.Location = new System.Drawing.Point(822, 35);
		this.btn_Print.Name = "btn_Print";
		this.btn_Print.ReadOnly = false;
		this.btn_Print.SettingInVisible = false;
		this.btn_Print.SettingReadonly = false;
		this.btn_Print.Size = new System.Drawing.Size(68, 20);
		this.btn_Print.TabIndex = 8;
		this.btn_Print.Text = "Print";
		this.btn_Print.ToolTip = "In lại tem thành phẩm theo số lệnh sản xuất";
		this.btn_Print.Click += new System.EventHandler(this.btn_Print_Click);
		this.btn_Clear.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Clear.EditControl = null;
		this.btn_Clear.EditValue = "Clear";
		this.btn_Clear.FieldName = null;
		this.btn_Clear.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Clear.ImageOptions.Image");
		this.btn_Clear.IsSearchControl = false;
		this.btn_Clear.Location = new System.Drawing.Point(896, 36);
		this.btn_Clear.Name = "btn_Clear";
		this.btn_Clear.ReadOnly = false;
		this.btn_Clear.SettingInVisible = false;
		this.btn_Clear.SettingReadonly = false;
		this.btn_Clear.Size = new System.Drawing.Size(64, 20);
		this.btn_Clear.TabIndex = 9;
		this.btn_Clear.Text = "Clear";
		this.btn_Clear.ToolTip = "Làm mới";
		this.btn_Clear.Click += new System.EventHandler(this.btn_Clear_Click);
		this.txt_DocNo.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txt_DocNo.EditControl = null;
		this.txt_DocNo.FieldName = null;
		this.txt_DocNo.IsSearchControl = false;
		this.txt_DocNo.Location = new System.Drawing.Point(822, 13);
		this.txt_DocNo.Name = "txt_DocNo";
		this.txt_DocNo.SettingInVisible = false;
		this.txt_DocNo.SettingReadonly = false;
		this.txt_DocNo.Size = new System.Drawing.Size(138, 20);
		this.txt_DocNo.TabIndex = 11;
		this.txt_DocNo.ToolTip = "Nhập số lệnh sản xuất cần in tem";
		this.txt_DocNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_DocNo_KeyDown);
		this.labelLine3.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "DocNo";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(774, 15);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(53, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 10;
		this.labelLine3.Utext = "DocNo";
		this.btn_Confirm.EditControl = null;
		this.btn_Confirm.EditValue = "Confirm";
		this.btn_Confirm.FieldName = null;
		this.btn_Confirm.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Confirm.ImageOptions.Image");
		this.btn_Confirm.IsSearchControl = false;
		this.btn_Confirm.Location = new System.Drawing.Point(582, 33);
		this.btn_Confirm.Name = "btn_Confirm";
		this.btn_Confirm.ReadOnly = false;
		this.btn_Confirm.SettingInVisible = false;
		this.btn_Confirm.SettingReadonly = false;
		this.btn_Confirm.Size = new System.Drawing.Size(75, 23);
		this.btn_Confirm.TabIndex = 12;
		this.btn_Confirm.Text = "Confirm";
		this.btn_Confirm.ToolTip = "Báo cáo packing";
		this.btn_Confirm.Click += new System.EventHandler(this.btn_Confirm_Click);
		this.chk_PackLast.EditControl = null;
		this.chk_PackLast.FieldName = null;
		this.chk_PackLast.IsSearchControl = false;
		this.chk_PackLast.Location = new System.Drawing.Point(492, 12);
		this.chk_PackLast.Name = "chk_PackLast";
		this.chk_PackLast.Properties.Caption = "Ghép batch cuối";
		this.chk_PackLast.SettingInVisible = false;
		this.chk_PackLast.SettingReadonly = false;
		this.chk_PackLast.Size = new System.Drawing.Size(115, 20);
		this.chk_PackLast.TabIndex = 13;
		this.chk_PackLast.ToolTip = "Tùy chọn nhập số lẻ vào gói cuối cùng";
		this.chk_PackQty.EditControl = null;
		this.chk_PackQty.FieldName = null;
		this.chk_PackQty.IsSearchControl = false;
		this.chk_PackQty.Location = new System.Drawing.Point(613, 12);
		this.chk_PackQty.Name = "chk_PackQty";
		this.chk_PackQty.Properties.Caption = "Ghép số cuộn";
		this.chk_PackQty.SettingInVisible = false;
		this.chk_PackQty.SettingReadonly = false;
		this.chk_PackQty.Size = new System.Drawing.Size(102, 20);
		this.chk_PackQty.TabIndex = 14;
		this.chk_PackQty.ToolTip = "Tùy chọn Ghép số cuộn (áp dụng với giấy nhiệt)";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(965, 625);
		base.Controls.Add(this.chk_PackQty);
		base.Controls.Add(this.chk_PackLast);
		base.Controls.Add(this.btn_Confirm);
		base.Controls.Add(this.txt_DocNo);
		base.Controls.Add(this.labelLine3);
		base.Controls.Add(this.btn_Clear);
		base.Controls.Add(this.btn_Print);
		base.Controls.Add(this.cal_Qty);
		base.Controls.Add(this.labelLine2);
		base.Controls.Add(this.btn_Packing);
		base.Controls.Add(this.splitContainerControl1);
		base.Controls.Add(this.txt_BatchNo);
		base.Controls.Add(this.labelLine1);
		base.FormId = "oPackingBatch";
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_PackingBatch.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_PackingBatch";
		base.ShowInTaskbar = false;
		this.Text = "Packing Batch";
		base.Load += new System.EventHandler(this.frm_PackingBatch_Load);
		((System.ComponentModel.ISupportInitialize)this.txt_BatchNo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).EndInit();
		this.xtraTabControl1.ResumeLayout(false);
		this.xtraTabPage1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_ListOfBatch).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_ListOfBatch).EndInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl2).EndInit();
		this.xtraTabControl2.ResumeLayout(false);
		this.xtraTabPage2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Result).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Result).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cal_Qty.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_DocNo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_PackLast.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_PackQty.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
