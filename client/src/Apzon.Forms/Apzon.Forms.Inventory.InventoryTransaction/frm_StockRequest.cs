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
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;
using Apzon.Forms.Commons;
using Apzon.Forms.Helper;
using Apzon.Forms.MTO_Manufacturing;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTab;
using SAPbobsCOM;

namespace Apzon.Forms.Inventory.InventoryTransaction;

public class frm_StockRequest : XtraFormEx
{
	private const string FormController = "StockRequest";

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private DataTable dtSearch;

	private IContainer components;

	private XtraTabControlEx xtraTab_StockRequest;

	private XtraTabPageEx tab_StockRequest;

	private SimpleButtonEx btn_Print;

	private SplitContainerControl splitContainerControl1;

	private GridControlEx grd_Doc;

	private ContextMenuStrip ctm_Doc;

	private ToolStripMenuItem printToolStripMenuItem;

	private GridViewEx gv_Doc;

	private GridControlEx grd_Line;

	private GridViewEx gv_Line;

	private XtraTabPageEx tab_Transfer;

	private GridControlEx grd_Transfer;

	private GridViewEx gv_Transfer;

	private LabelLine labelLine1;

	private TextEditEx txt_DocNo;

	private SimpleButtonEx btn_PrintTransfer;

	private TextEditEx txt_BatchNo;

	private LabelLine labelLine2;

	private SimpleButtonEx btn_Clear;

	private SimpleButtonEx btn_Transfer;

	private Timer timer_LoadForm;

	private SimpleButtonEx btn_Refresh;

	private SpinEdit spin_Timer;

	private CheckEditEx chk_Timer;

	private DateEditEx date_fromDate;

	private TextEditEx txt_DocTranfer;

	private LabelLine labelLine3;

	public int TransferId { get; set; }

	public frm_StockRequest()
	{
		InitializeComponent();
	}

	private void frm_StockRequest_Load(object sender, EventArgs e)
	{
		try
		{
			timer_LoadForm.Enabled = chk_Timer.Checked;
			timer_LoadForm.Interval = Function.ParseInt(spin_Timer.EditValue) * 60000;
			date_fromDate.EditValue = DateTime.Today;
			txt_DocNo.Focus();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadData()
	{
		EnableControl(isEnable: false);
		try
		{
			WinCommons._OpenProcessing();
			DataSet dataSet = new DataSet();
			DateTime? dateTime = Function.ParseDateTime(date_fromDate.EditValue);
			if (!dateTime.HasValue)
			{
				grd_Doc.DataSourceTable().Clear();
				grd_Line.DataSourceTable().Clear();
				grd_Transfer.DataSourceTable().Clear();
				return;
			}
			CreateDataSearch();
			DataRow dataRow = dtSearch.NewRow();
			dataRow["username"] = GlobalData.UserName;
			dataRow["branch"] = GlobalData.DefaultBranch;
			dataRow["fromdate"] = dateTime;
			dtSearch.Rows.Add(dataRow);
			using (HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "StockRequest"))
			{
				dataSet = await helper.PostAsync("GetAll", dtSearch);
			}
			if (dataSet.IsHasStructure())
			{
				if (gv_Doc.Columns.Count <= 0)
				{
					await GridViewHelper.GenerateGrid(grd_Doc, gv_Doc, dataSet);
					return;
				}
				grd_Doc.DataSource = dataSet.Tables[1];
				grd_Doc.RefreshDataSource();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.NoMatchingRecordFound, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			EnableControl(isEnable: true);
			WinCommons._CloseProcessing();
		}
	}

	private async void gv_Doc_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		_ = 1;
		try
		{
			WinCommons._OpenProcessing();
			EnableControl(isEnable: false);
			int docentry = Function.ParseInt(gv_Doc.GetRowCellValue(e.FocusedRowHandle, "DocEntry"));
			DataSet dataSet = new DataSet();
			using (HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "StockRequest"))
			{
				dataSet = await helper.GetAsync("GetDetail", new { docentry });
			}
			if (dataSet.IsHasValidValue())
			{
				if (gv_Line.Columns.Count <= 0)
				{
					await GridViewHelper.GenerateGrid(grd_Line, gv_Line, dataSet);
					return;
				}
				grd_Line.DataSource = dataSet.Tables[1];
				grd_Line.RefreshDataSource();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.NoMatchingRecordFound, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			EnableControl(isEnable: true);
			WinCommons._CloseProcessing();
		}
	}

	private async void txt_DocNo_KeyDown(object sender, KeyEventArgs e)
	{
		_ = 1;
		try
		{
			if (e.KeyCode != Keys.Return)
			{
				return;
			}
			int num = Function.ParseInt(txt_DocNo.EditValue);
			if (num > 0)
			{
				WinCommons._OpenProcessing();
				DataSet dataSet = await GetDataByDoc(num);
				if (dataSet.IsHasValidValue())
				{
					if (gv_Transfer.Columns.Count <= 0)
					{
						await GridViewHelper.GenerateGrid(grd_Transfer, gv_Transfer, dataSet);
					}
					else if (dataSet.Tables[1].IsNotNull())
					{
						if (grd_Transfer.DataSourceTable.IsNotNull())
						{
							if (XtraMessageBox.Show("Tồn tại dữ liệu đang kiểm tra.Bạn có muốn tiếp tục thêm dòng?", Text, MessageBoxButtons.YesNo) == DialogResult.Yes)
							{
								DataTable dataSourceTable = grd_Transfer.DataSourceTable;
								for (int i = 0; i < dataSet.Tables[1].Rows.Count; i++)
								{
									DataRow dataRow = dataSourceTable.NewRow();
									dataRow.ItemArray = dataSet.Tables[1].Rows[i].ItemArray;
									dataSourceTable.Rows.Add(dataRow);
								}
								dataSourceTable.AcceptChanges();
							}
							else
							{
								grd_Transfer.DataSource = dataSet.Tables[1];
								grd_Transfer.RefreshDataSource();
							}
						}
						else
						{
							grd_Transfer.DataSource = dataSet.Tables[1];
							grd_Transfer.RefreshDataSource();
						}
					}
					WinCommons._CloseProcessing();
					txt_DocTranfer.Text = "";
					txt_DocNo.Text = "";
					txt_BatchNo.Enabled = true;
					txt_BatchNo.Focus();
					return;
				}
				WinCommons.Error(Enums.MessageCode.NoMatchingRecordFound, "");
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, "Thông tin không hợp lệ");
			}
			txt_DocNo.Text = "";
			txt_DocNo.Focus();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<DataSet> GetDataByDoc(int docno)
	{
		try
		{
			using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "StockRequest");
			return await helper.GetAsync("GetDetail", new
			{
				docentry = docno
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataSet();
	}

	private void txt_BatchNo_KeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode != Keys.Return)
			{
				return;
			}
			string text = Function.ToString(txt_BatchNo.EditValue);
			DataTable dataSourceTable = grd_Transfer.DataSourceTable;
			if (!string.IsNullOrEmpty(text) && dataSourceTable.IsNotNull())
			{
				DataRow[] array = dataSourceTable.Select($"BatchNo='{text}'");
				if (array.Length != 0)
				{
					if (Function.ParseBool(array[0]["Checked"]))
					{
						WinCommons.Error(Enums.MessageCode.None, "\"Batch\" đã tồn tại trên lưới dữ liệu");
					}
					else
					{
						array[0]["Checked"] = true;
						dataSourceTable.AcceptChanges();
					}
				}
				else
				{
					DataRow dataRow = dataSourceTable.NewRow();
					dataRow["U_BatchNo"] = text;
					dataRow["Status"] = "N";
					dataSourceTable.Rows.Add(dataRow);
				}
			}
			txt_BatchNo.Text = "";
			txt_BatchNo.Focus();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Clear_Click(object sender, EventArgs e)
	{
		try
		{
			txt_DocTranfer.Text = "";
			txt_BatchNo.Text = "";
			txt_BatchNo.Enabled = false;
			txt_DocNo.Text = "";
			grd_Transfer.DataSource = null;
			grd_Transfer.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Print_Click_1(object sender, EventArgs e)
	{
		try
		{
			int docForBatch = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("PwNo"));
			if ((await CheckRequest(docForBatch, GlobalData.UserName, GlobalData.DefaultBranch)).IsNotNull())
			{
				WinCommons._OpenProcessing();
				frm_ProductionConfirm frm_ProductionConfirm2 = new frm_ProductionConfirm();
				frm_ProductionConfirm2.MdiParent = MdiParent;
				frm_ProductionConfirm2.Show();
				WinCommons._CloseProcessing();
			}
			PRINTLAYOUT();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_PrintTransfer_Click(object sender, EventArgs e)
	{
		try
		{
			await PreviewTransfer();
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
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
			{
				return;
			}
			WinCommons._OpenProcessing();
			EnableControl(isEnable: false);
			DocumentsImpl documentsImpl = new DocumentsImpl();
			documentsImpl.DocDate = DateTime.Today;
			documentsImpl.TaxDate = documentsImpl.DocDate;
			documentsImpl.DocDueDate = documentsImpl.DocDate;
			DataTable dataSourceTable = grd_Transfer.DataSourceTable;
			if (!dataSourceTable.IsNotNull())
			{
				return;
			}
			DataRow[] array = dataSourceTable.Select("Checked='TRUE'");
			if (array.Length == 0)
			{
				XtraMessageBox.Show("Chưa kiểm tra các dòng");
				WinCommons._CloseProcessing();
				return;
			}
			documentsImpl.Lines = new List<Document_LinesImpl>();
			int num = 0;
			for (int i = 0; i < array.Length; i++)
			{
				Function.ParseInt(array[i]["BinAbs"]);
				Document_LinesImpl document_LinesImpl = new Document_LinesImpl();
				document_LinesImpl.ItemCode = Function.ToString(array[i]["ItemCode"]);
				document_LinesImpl.FromWhsCod = Function.ToString(array[i]["WhsCode"]);
				if (string.IsNullOrEmpty(documentsImpl.Filler))
				{
					documentsImpl.Filler = document_LinesImpl.FromWhsCod;
				}
				document_LinesImpl.WhsCode = Function.ToString(array[i]["ToWhsCode"]);
				if (string.IsNullOrEmpty(documentsImpl.ToWhsCode))
				{
					documentsImpl.ToWhsCode = document_LinesImpl.WhsCode;
				}
				document_LinesImpl.Quantity = Function.ParseDouble(array[i]["Quantity"]);
				document_LinesImpl.BatchNumbers = new List<BatchNumbersImpl>();
				BatchNumbersImpl batchNumbersImpl = new BatchNumbersImpl();
				batchNumbersImpl.BatchNumber = Function.ToString(array[i]["BatchNo"]);
				batchNumbersImpl.Quantity = document_LinesImpl.Quantity;
				batchNumbersImpl.BaseLineNumber = num;
				document_LinesImpl.BatchNumbers.Add(batchNumbersImpl);
				document_LinesImpl.LineNum = num;
				document_LinesImpl.BaseType = 1250000001;
				document_LinesImpl.BaseEntry = Function.ParseInt(array[i]["DocEntry"]);
				document_LinesImpl.BaseLine = Function.ParseInt(array[i]["LineNum"]);
				documentsImpl.Lines.Add(document_LinesImpl);
				document_LinesImpl.BinAllocations = new List<DocumentLinesBinAllocationsImpl>();
				DocumentLinesBinAllocationsImpl documentLinesBinAllocationsImpl = new DocumentLinesBinAllocationsImpl();
				documentLinesBinAllocationsImpl.BinAbsEntry = Function.ParseInt(array[i]["BinAbs"]);
				documentLinesBinAllocationsImpl.BaseLineNumber = num;
				documentLinesBinAllocationsImpl.BinActionType = BinActionTypeEnum.batFromWarehouse;
				documentLinesBinAllocationsImpl.Quantity = document_LinesImpl.Quantity;
				documentLinesBinAllocationsImpl.SerialAndBatchNumbersBaseLine = 0;
				if (documentLinesBinAllocationsImpl.BinAbsEntry > 0)
				{
					document_LinesImpl.BinAllocations.Add(documentLinesBinAllocationsImpl);
				}
				DocumentLinesBinAllocationsImpl documentLinesBinAllocationsImpl2 = new DocumentLinesBinAllocationsImpl();
				documentLinesBinAllocationsImpl2.BinAbsEntry = Function.ParseInt(array[i]["ToBin"]);
				documentLinesBinAllocationsImpl2.BaseLineNumber = num;
				documentLinesBinAllocationsImpl2.BinActionType = BinActionTypeEnum.batToWarehouse;
				documentLinesBinAllocationsImpl2.Quantity = document_LinesImpl.Quantity;
				documentLinesBinAllocationsImpl2.SerialAndBatchNumbersBaseLine = 0;
				if (documentLinesBinAllocationsImpl2.BinAbsEntry > 0)
				{
					document_LinesImpl.BinAllocations.Add(documentLinesBinAllocationsImpl2);
				}
				num++;
			}
			HttpResult httpResult = await _httpClient.StockTransfer.SaveDoc(documentsImpl);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				XtraMessageBox.Show(httpResult.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			WinCommons.Success(Enums.MessageCode.SuccessWithContent, "", Function.ToString(httpResult.Content));
			TransferId = Function.ParseInt(httpResult.Content);
			txt_DocTranfer.Text = TransferId.ToString();
			await PrinTransfer();
			grd_Transfer.DataSource = null;
			grd_Transfer.RefreshDataSource();
			txt_DocNo.Text = "";
			txt_DocNo.Focus();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			EnableControl(isEnable: true);
			WinCommons._CloseProcessing();
		}
	}

	private async void btn_Refresh_Click(object sender, EventArgs e)
	{
		try
		{
			await LoadData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void date_fromDate_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			await LoadData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_RowStyle(object sender, RowStyleEventArgs e)
	{
		try
		{
			if (e.RowHandle > -1 && gv_Doc.IsExistColumn("PwbStatus"))
			{
				int num = Function.ParseInt(gv_Doc.GetRowCellValue(e.RowHandle, "Ref2"));
				string value = Function.ToString(gv_Doc.GetRowCellValue(e.RowHandle, "PwbStatus"));
				int num2 = Function.ParseInt(gv_Doc.GetRowCellValue(e.RowHandle, "Timer"));
				if (num > 0)
				{
					e.Appearance.BackColor = Color.Aquamarine;
				}
				else
				{
					e.Appearance.BackColor = Color.FromArgb(255, 255, 51);
				}
				if (num2 > 30)
				{
					e.Appearance.BackColor = Color.Red;
				}
				if ("C".Equals(value))
				{
					e.Appearance.BackColor = Color.DarkGray;
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
			await LoadData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async void PRINTLAYOUT()
	{
		_ = 2;
		try
		{
			int docEntry = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("DocEntry"));
			if (docEntry <= 0)
			{
				return;
			}
			PrintLayout printLayout = await WinFuntions.GetDefaultPrintLayout(GlobalData.UserName, "WTQ1", "");
			List<Common.ParameterValue> lstParam = new List<Common.ParameterValue>
			{
				new Common.ParameterValue
				{
					Name = "DOCKEY@",
					Value = docEntry.ToString()
				}
			};
			if (printLayout != null && !string.IsNullOrEmpty(printLayout.FileName))
			{
				string text = await WinFuntions.ReportViewerListParam($"{Application.StartupPath}\\Report\\{GlobalData.DatabaseName}\\{printLayout.FileName}", lstParam);
				if (!string.IsNullOrEmpty(text))
				{
					WinCommons.Error(Enums.MessageCode.None, text);
				}
			}
			int count = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("Ref2")) + 1;
			string text2 = await UpdatePrintCount(docEntry.ToString(), "OWTQ", count, GlobalData.UserName);
			if (!string.IsNullOrEmpty(text2))
			{
				WinCommons.Error(Enums.MessageCode.None, text2);
			}
			else
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
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
			int docEntry = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("DocEntry"));
			if (docEntry <= 0)
			{
				return;
			}
			await WinFuntions.Print("", docEntry, "WTR1");
			int num = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("Ref2")) + 1;
			DocumentsImpl documentsImpl = new DocumentsImpl();
			documentsImpl.DocEntry = docEntry;
			documentsImpl.Ref2 = num.ToString();
			using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "TranferRequest");
			HttpResult httpResult = await helper.PostAsync("SaveDoc", documentsImpl);
			if (httpResult.MessageCode != Enums.MessageCode.Success)
			{
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
			}
			else
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async void PREVIEWLAYOUTS()
	{
		try
		{
			xtraTab_StockRequest.SelectedTabPage = tab_Transfer;
			int num = ((Function.ParseInt(txt_DocTranfer.Text) == 0) ? TransferId : Function.ParseInt(txt_DocTranfer.Text));
			if (num > 0)
			{
				await WinFuntions.PreviewLayout("", num, "WTR1", this);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task PrinTransfer()
	{
		_ = 1;
		try
		{
			int docEntry = ((Function.ParseInt(txt_DocTranfer.Text) == 0) ? TransferId : Function.ParseInt(txt_DocTranfer.Text));
			if (docEntry > 0)
			{
				await WinFuntions.Print("", docEntry, "WTR1");
				await WinFuntions.UpdatePrintedLayout(docEntry.ToString(), "OWTR", "Y");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task PreviewTransfer()
	{
		try
		{
			int num = ((Function.ParseInt(txt_DocTranfer.Text) == 0) ? TransferId : Function.ParseInt(txt_DocTranfer.Text));
			if (num > 0)
			{
				await WinFuntions.PrintLayout("", num, "WTR1");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void CreateDataSearch()
	{
		if (!dtSearch.IsNotNull() || dtSearch.Columns.Count == 0)
		{
			dtSearch = new DataTable();
			dtSearch.Columns.Add(new DataColumn("username", typeof(string)));
			dtSearch.Columns.Add(new DataColumn("branch", typeof(int)));
			dtSearch.Columns.Add(new DataColumn("fromdate", typeof(DateTime)));
		}
		else
		{
			dtSearch.Rows.Clear();
		}
	}

	private async Task<DataTable> CheckRequest(int docForBatch, string username, int branch)
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "StockRequest");
			return await helper.GetAsync("CheckRequest", new { docForBatch, username, branch });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	private async Task<string> UpdatePrintCount(string docentry, string table, int count, string userName = "")
	{
		try
		{
			using HttpClientHelper<string> helper = new HttpClientHelper<string>(Configuration.ApiServer, "StockRequest");
			return await helper.GetAsync("UpdatePrintCount", new { docentry, table, count, userName });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private void EnableControl(bool isEnable)
	{
		grd_Doc.Enabled = isEnable;
		grd_Line.Enabled = isEnable;
		grd_Transfer.Enabled = isEnable;
		btn_Print.Enabled = isEnable;
		btn_Refresh.Enabled = isEnable;
		btn_PrintTransfer.Enabled = isEnable;
		btn_Transfer.Enabled = isEnable;
		btn_Clear.Enabled = isEnable;
		txt_DocNo.Enabled = isEnable;
		txt_DocTranfer.Enabled = isEnable;
		txt_BatchNo.Enabled = isEnable;
	}

	public Color ChangeColorBrightness(Color color, float correctionFactor)
	{
		try
		{
			float num = (int)color.R;
			float num2 = (int)color.G;
			float num3 = (int)color.B;
			if (correctionFactor < 0f)
			{
				correctionFactor = 1f + correctionFactor;
				num *= correctionFactor;
				num2 *= correctionFactor;
				num3 *= correctionFactor;
			}
			else
			{
				num = (255f - num) * correctionFactor + num;
				num2 = (255f - num2) * correctionFactor + num2;
				num3 = (255f - num3) * correctionFactor + num3;
			}
			return Color.FromArgb(color.A, (int)num, (int)num2, (int)num3);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return Color.White;
	}

	private void xtraTabControl1_SelectedPageChanged(object sender, TabPageChangedEventArgs e)
	{
		try
		{
			if (xtraTab_StockRequest.SelectedTabPage.Name == "tab_Transfer")
			{
				txt_DocNo.Focus();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Inventory.InventoryTransaction.frm_StockRequest));
		this.xtraTab_StockRequest = new Apzon.Controls.Controls.XtraTabControlEx();
		this.tab_StockRequest = new Apzon.Controls.Controls.XtraTabPageEx();
		this.date_fromDate = new Apzon.Controls.Editor.DateEditEx();
		this.btn_Refresh = new Apzon.Controls.Controls.SimpleButtonEx();
		this.spin_Timer = new DevExpress.XtraEditors.SpinEdit();
		this.chk_Timer = new Apzon.Controls.Editor.CheckEditEx();
		this.btn_Print = new Apzon.Controls.Controls.SimpleButtonEx();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.ctm_Doc = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.printToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.grd_Line = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Line = new Apzon.Controls.Grid.GridViewEx();
		this.tab_Transfer = new Apzon.Controls.Controls.XtraTabPageEx();
		this.txt_DocTranfer = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.btn_Transfer = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Clear = new Apzon.Controls.Controls.SimpleButtonEx();
		this.txt_BatchNo = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.btn_PrintTransfer = new Apzon.Controls.Controls.SimpleButtonEx();
		this.txt_DocNo = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.grd_Transfer = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Transfer = new Apzon.Controls.Grid.GridViewEx();
		this.timer_LoadForm = new System.Windows.Forms.Timer(this.components);
		((System.ComponentModel.ISupportInitialize)this.xtraTab_StockRequest).BeginInit();
		this.xtraTab_StockRequest.SuspendLayout();
		this.tab_StockRequest.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.date_fromDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_fromDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.spin_Timer.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Timer.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		this.ctm_Doc.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Line).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Line).BeginInit();
		this.tab_Transfer.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txt_DocTranfer.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BatchNo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_DocNo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Transfer).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Transfer).BeginInit();
		base.SuspendLayout();
		this.xtraTab_StockRequest.Dock = System.Windows.Forms.DockStyle.Fill;
		this.xtraTab_StockRequest.IsSearchControl = false;
		this.xtraTab_StockRequest.Location = new System.Drawing.Point(0, 0);
		this.xtraTab_StockRequest.Name = "xtraTab_StockRequest";
		this.xtraTab_StockRequest.SelectedTabPage = this.tab_StockRequest;
		this.xtraTab_StockRequest.Size = new System.Drawing.Size(1029, 649);
		this.xtraTab_StockRequest.TabIndex = 0;
		this.xtraTab_StockRequest.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[2] { this.tab_StockRequest, this.tab_Transfer });
		this.xtraTab_StockRequest.SelectedPageChanged += new DevExpress.XtraTab.TabPageChangedEventHandler(this.xtraTabControl1_SelectedPageChanged);
		this.tab_StockRequest.Controls.Add(this.date_fromDate);
		this.tab_StockRequest.Controls.Add(this.btn_Refresh);
		this.tab_StockRequest.Controls.Add(this.spin_Timer);
		this.tab_StockRequest.Controls.Add(this.chk_Timer);
		this.tab_StockRequest.Controls.Add(this.btn_Print);
		this.tab_StockRequest.Controls.Add(this.splitContainerControl1);
		this.tab_StockRequest.EditControl = null;
		this.tab_StockRequest.EditValue = "List Of Stock Request";
		this.tab_StockRequest.FieldName = null;
		this.tab_StockRequest.IsSearchControl = false;
		this.tab_StockRequest.Name = "tab_StockRequest";
		this.tab_StockRequest.ReadOnly = false;
		this.tab_StockRequest.SettingInVisible = false;
		this.tab_StockRequest.SettingReadonly = false;
		this.tab_StockRequest.Size = new System.Drawing.Size(1027, 624);
		this.tab_StockRequest.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_StockRequest.Text = "List Of Stock Request";
		this.date_fromDate.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.date_fromDate.EditControl = null;
		this.date_fromDate.EditValue = null;
		this.date_fromDate.FieldName = null;
		this.date_fromDate.Location = new System.Drawing.Point(183, 598);
		this.date_fromDate.Name = "date_fromDate";
		this.date_fromDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_fromDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_fromDate.Properties.IsSearchControl = false;
		this.date_fromDate.SettingInVisible = false;
		this.date_fromDate.SettingReadonly = false;
		this.date_fromDate.Size = new System.Drawing.Size(100, 20);
		this.date_fromDate.TabIndex = 59;
		this.date_fromDate.EditValueChanged += new System.EventHandler(this.date_fromDate_EditValueChanged);
		this.btn_Refresh.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Refresh.EditControl = null;
		this.btn_Refresh.EditValue = "Refresh";
		this.btn_Refresh.FieldName = null;
		this.btn_Refresh.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Refresh.ImageOptions.Image");
		this.btn_Refresh.IsSearchControl = false;
		this.btn_Refresh.Location = new System.Drawing.Point(90, 596);
		this.btn_Refresh.Name = "btn_Refresh";
		this.btn_Refresh.ReadOnly = false;
		this.btn_Refresh.SettingInVisible = false;
		this.btn_Refresh.SettingReadonly = false;
		this.btn_Refresh.Size = new System.Drawing.Size(75, 23);
		this.btn_Refresh.TabIndex = 56;
		this.btn_Refresh.Text = "Refresh";
		this.btn_Refresh.ToolTip = "Làm mới form";
		this.btn_Refresh.Click += new System.EventHandler(this.btn_Refresh_Click);
		this.spin_Timer.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.spin_Timer.EditValue = new decimal(new int[4] { 5, 0, 0, 0 });
		this.spin_Timer.Location = new System.Drawing.Point(934, 597);
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
		this.spin_Timer.TabIndex = 58;
		this.spin_Timer.ToolTip = "Thời gian tự động load trang";
		this.chk_Timer.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.chk_Timer.EditControl = null;
		this.chk_Timer.EditValue = true;
		this.chk_Timer.FieldName = null;
		this.chk_Timer.IsSearchControl = false;
		this.chk_Timer.Location = new System.Drawing.Point(848, 598);
		this.chk_Timer.Name = "chk_Timer";
		this.chk_Timer.Properties.Caption = "Auto Refresh";
		this.chk_Timer.SettingInVisible = false;
		this.chk_Timer.SettingReadonly = false;
		this.chk_Timer.Size = new System.Drawing.Size(89, 20);
		this.chk_Timer.TabIndex = 57;
		this.btn_Print.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Print.EditControl = null;
		this.btn_Print.EditValue = "Print";
		this.btn_Print.FieldName = null;
		this.btn_Print.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Print.ImageOptions.Image");
		this.btn_Print.IsSearchControl = false;
		this.btn_Print.Location = new System.Drawing.Point(3, 595);
		this.btn_Print.Name = "btn_Print";
		this.btn_Print.ReadOnly = false;
		this.btn_Print.SettingInVisible = false;
		this.btn_Print.SettingReadonly = false;
		this.btn_Print.Size = new System.Drawing.Size(75, 23);
		this.btn_Print.TabIndex = 5;
		this.btn_Print.Text = "Print";
		this.btn_Print.Click += new System.EventHandler(this.btn_Print_Click_1);
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(0, 0);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.grd_Doc);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.grd_Line);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(1023, 590);
		this.splitContainerControl1.SplitterPosition = 266;
		this.splitContainerControl1.TabIndex = 3;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.ContextMenuStrip = this.ctm_Doc;
		this.grd_Doc.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Doc.Location = new System.Drawing.Point(0, 0);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.Size = new System.Drawing.Size(1023, 266);
		this.grd_Doc.TabIndex = 0;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.ctm_Doc.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.printToolStripMenuItem });
		this.ctm_Doc.Name = "ctm_Doc";
		this.ctm_Doc.Size = new System.Drawing.Size(100, 26);
		this.printToolStripMenuItem.Name = "printToolStripMenuItem";
		this.printToolStripMenuItem.Size = new System.Drawing.Size(99, 22);
		this.printToolStripMenuItem.Text = "Print";
		this.gv_Doc.Appearance.FocusedRow.BackColor = System.Drawing.Color.Transparent;
		this.gv_Doc.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Doc.Appearance.SelectedRow.BackColor = System.Drawing.Color.Transparent;
		this.gv_Doc.Appearance.SelectedRow.Options.UseBackColor = true;
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = null;
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFullFocus;
		this.gv_Doc.FormId = "oStockRequest";
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.IgnoreFilter = false;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 35;
		this.gv_Doc.IsApzTable = false;
		this.gv_Doc.IsInsertNewRow = false;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.IsMappingContextAction = false;
		this.gv_Doc.ItemId = "OSTR";
		this.gv_Doc.LstColumnAllowBatch = null;
		this.gv_Doc.LstColumnNotAllowBatch = null;
		this.gv_Doc.LstColumnNotEditable = null;
		this.gv_Doc.LstColumNotSetting = null;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.UseBatchMod = false;
		this.gv_Doc.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.gv_Doc_RowStyle);
		this.gv_Doc.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_Doc_FocusedRowObjectChanged);
		this.grd_Line.AllowPasteExcel = true;
		this.grd_Line.ClipboardData = null;
		this.grd_Line.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Line.Location = new System.Drawing.Point(0, 0);
		this.grd_Line.MainView = this.gv_Line;
		this.grd_Line.Name = "grd_Line";
		this.grd_Line.Size = new System.Drawing.Size(1023, 314);
		this.grd_Line.TabIndex = 1;
		this.grd_Line.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Line });
		this.gv_Line.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Line.ColumnKey = null;
		this.gv_Line.DisableFormSetting = false;
		this.gv_Line.DisplayException = false;
		this.gv_Line.EditColumn = null;
		this.gv_Line.FieldCurrency = null;
		this.gv_Line.FieldRate = null;
		this.gv_Line.FormId = "oStockRequest";
		this.gv_Line.GridControl = this.grd_Line;
		this.gv_Line.IgnoreFilter = false;
		this.gv_Line.IndexEditRow = -1;
		this.gv_Line.IndicatorWidth = 35;
		this.gv_Line.IsApzTable = false;
		this.gv_Line.IsInsertNewRow = false;
		this.gv_Line.IsLoadingData = false;
		this.gv_Line.IsMappingContextAction = false;
		this.gv_Line.ItemId = "STR1";
		this.gv_Line.LstColumnAllowBatch = null;
		this.gv_Line.LstColumnNotAllowBatch = null;
		this.gv_Line.LstColumnNotEditable = null;
		this.gv_Line.LstColumNotSetting = null;
		this.gv_Line.Name = "gv_Line";
		this.gv_Line.UseBatchMod = false;
		this.tab_Transfer.Controls.Add(this.txt_DocTranfer);
		this.tab_Transfer.Controls.Add(this.labelLine3);
		this.tab_Transfer.Controls.Add(this.btn_Transfer);
		this.tab_Transfer.Controls.Add(this.btn_Clear);
		this.tab_Transfer.Controls.Add(this.txt_BatchNo);
		this.tab_Transfer.Controls.Add(this.labelLine2);
		this.tab_Transfer.Controls.Add(this.btn_PrintTransfer);
		this.tab_Transfer.Controls.Add(this.txt_DocNo);
		this.tab_Transfer.Controls.Add(this.labelLine1);
		this.tab_Transfer.Controls.Add(this.grd_Transfer);
		this.tab_Transfer.EditControl = null;
		this.tab_Transfer.EditValue = "Transfer";
		this.tab_Transfer.FieldName = null;
		this.tab_Transfer.IsSearchControl = false;
		this.tab_Transfer.Name = "tab_Transfer";
		this.tab_Transfer.ReadOnly = false;
		this.tab_Transfer.SettingInVisible = false;
		this.tab_Transfer.SettingReadonly = false;
		this.tab_Transfer.Size = new System.Drawing.Size(1027, 624);
		this.tab_Transfer.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Transfer.Text = "Transfer";
		this.txt_DocTranfer.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txt_DocTranfer.EditControl = null;
		this.txt_DocTranfer.FieldName = null;
		this.txt_DocTranfer.IsSearchControl = false;
		this.txt_DocTranfer.Location = new System.Drawing.Point(934, 7);
		this.txt_DocTranfer.Name = "txt_DocTranfer";
		this.txt_DocTranfer.SettingInVisible = false;
		this.txt_DocTranfer.SettingReadonly = false;
		this.txt_DocTranfer.Size = new System.Drawing.Size(86, 20);
		this.txt_DocTranfer.TabIndex = 9;
		this.labelLine3.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "Phiếu chuyển kho:";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(834, 9);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(104, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 8;
		this.labelLine3.Utext = "Phiếu chuyển kho:";
		this.btn_Transfer.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Transfer.EditControl = null;
		this.btn_Transfer.EditValue = "Transfer";
		this.btn_Transfer.FieldName = null;
		this.btn_Transfer.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Transfer.ImageOptions.Image");
		this.btn_Transfer.IsSearchControl = false;
		this.btn_Transfer.Location = new System.Drawing.Point(945, 591);
		this.btn_Transfer.Name = "btn_Transfer";
		this.btn_Transfer.ReadOnly = false;
		this.btn_Transfer.SettingInVisible = false;
		this.btn_Transfer.SettingReadonly = false;
		this.btn_Transfer.Size = new System.Drawing.Size(75, 23);
		this.btn_Transfer.TabIndex = 7;
		this.btn_Transfer.Text = "Transfer";
		this.btn_Transfer.Click += new System.EventHandler(this.btn_Transfer_Click);
		this.btn_Clear.EditControl = null;
		this.btn_Clear.EditValue = "Clear";
		this.btn_Clear.FieldName = null;
		this.btn_Clear.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Clear.ImageOptions.Image");
		this.btn_Clear.IsSearchControl = false;
		this.btn_Clear.Location = new System.Drawing.Point(351, 30);
		this.btn_Clear.Name = "btn_Clear";
		this.btn_Clear.ReadOnly = false;
		this.btn_Clear.SettingInVisible = false;
		this.btn_Clear.SettingReadonly = false;
		this.btn_Clear.Size = new System.Drawing.Size(76, 20);
		this.btn_Clear.TabIndex = 6;
		this.btn_Clear.Text = "Clear";
		this.btn_Clear.Click += new System.EventHandler(this.btn_Clear_Click);
		this.txt_BatchNo.EditControl = null;
		this.txt_BatchNo.Enabled = false;
		this.txt_BatchNo.FieldName = null;
		this.txt_BatchNo.IsSearchControl = false;
		this.txt_BatchNo.Location = new System.Drawing.Point(80, 30);
		this.txt_BatchNo.Name = "txt_BatchNo";
		this.txt_BatchNo.SettingInVisible = false;
		this.txt_BatchNo.SettingReadonly = false;
		this.txt_BatchNo.Size = new System.Drawing.Size(265, 20);
		this.txt_BatchNo.TabIndex = 5;
		this.txt_BatchNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_BatchNo_KeyDown);
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "Batch No.";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(0, 33);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(80, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 4;
		this.labelLine2.Utext = "Batch No.";
		this.btn_PrintTransfer.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btn_PrintTransfer.EditControl = null;
		this.btn_PrintTransfer.EditValue = "Print";
		this.btn_PrintTransfer.FieldName = null;
		this.btn_PrintTransfer.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_PrintTransfer.ImageOptions.Image");
		this.btn_PrintTransfer.IsSearchControl = false;
		this.btn_PrintTransfer.Location = new System.Drawing.Point(945, 33);
		this.btn_PrintTransfer.Name = "btn_PrintTransfer";
		this.btn_PrintTransfer.ReadOnly = false;
		this.btn_PrintTransfer.SettingInVisible = false;
		this.btn_PrintTransfer.SettingReadonly = false;
		this.btn_PrintTransfer.Size = new System.Drawing.Size(75, 23);
		this.btn_PrintTransfer.TabIndex = 3;
		this.btn_PrintTransfer.Text = "Print";
		this.btn_PrintTransfer.Click += new System.EventHandler(this.btn_PrintTransfer_Click);
		this.txt_DocNo.EditControl = null;
		this.txt_DocNo.FieldName = null;
		this.txt_DocNo.IsSearchControl = false;
		this.txt_DocNo.Location = new System.Drawing.Point(80, 4);
		this.txt_DocNo.Name = "txt_DocNo";
		this.txt_DocNo.SettingInVisible = false;
		this.txt_DocNo.SettingReadonly = false;
		this.txt_DocNo.Size = new System.Drawing.Size(265, 20);
		this.txt_DocNo.TabIndex = 2;
		this.txt_DocNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_DocNo_KeyDown);
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Phiếu tìm hàng:";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(0, 7);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(80, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 1;
		this.labelLine1.Utext = "Phiếu tìm hàng:";
		this.grd_Transfer.AllowPasteExcel = false;
		this.grd_Transfer.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Transfer.ClipboardData = null;
		this.grd_Transfer.Location = new System.Drawing.Point(3, 59);
		this.grd_Transfer.MainView = this.gv_Transfer;
		this.grd_Transfer.Name = "grd_Transfer";
		this.grd_Transfer.Size = new System.Drawing.Size(1020, 526);
		this.grd_Transfer.TabIndex = 0;
		this.grd_Transfer.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Transfer });
		this.gv_Transfer.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Transfer.ColumnKey = null;
		this.gv_Transfer.DisableFormSetting = false;
		this.gv_Transfer.DisplayException = false;
		this.gv_Transfer.EditColumn = null;
		this.gv_Transfer.FieldCurrency = null;
		this.gv_Transfer.FieldRate = null;
		this.gv_Transfer.FormId = "oStockRequest";
		this.gv_Transfer.GridControl = this.grd_Transfer;
		this.gv_Transfer.IgnoreFilter = false;
		this.gv_Transfer.IndexEditRow = -1;
		this.gv_Transfer.IndicatorWidth = 35;
		this.gv_Transfer.IsApzTable = false;
		this.gv_Transfer.IsInsertNewRow = false;
		this.gv_Transfer.IsLoadingData = false;
		this.gv_Transfer.IsMappingContextAction = false;
		this.gv_Transfer.ItemId = "STR2";
		this.gv_Transfer.LstColumnAllowBatch = null;
		this.gv_Transfer.LstColumnNotAllowBatch = null;
		this.gv_Transfer.LstColumnNotEditable = null;
		this.gv_Transfer.LstColumNotSetting = null;
		this.gv_Transfer.Name = "gv_Transfer";
		this.gv_Transfer.UseBatchMod = false;
		this.timer_LoadForm.Interval = 10000;
		this.timer_LoadForm.Tick += new System.EventHandler(this.timer_LoadForm_Tick);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1029, 649);
		base.Controls.Add(this.xtraTab_StockRequest);
		base.FormId = "oStockRequest";
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_StockRequest.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_StockRequest";
		base.ShowInTaskbar = false;
		this.Text = "Stock Request";
		base.Load += new System.EventHandler(this.frm_StockRequest_Load);
		((System.ComponentModel.ISupportInitialize)this.xtraTab_StockRequest).EndInit();
		this.xtraTab_StockRequest.ResumeLayout(false);
		this.tab_StockRequest.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.date_fromDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_fromDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.spin_Timer.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Timer.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		this.ctm_Doc.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Line).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Line).EndInit();
		this.tab_Transfer.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txt_DocTranfer.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BatchNo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_DocNo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Transfer).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Transfer).EndInit();
		base.ResumeLayout(false);
	}
}
