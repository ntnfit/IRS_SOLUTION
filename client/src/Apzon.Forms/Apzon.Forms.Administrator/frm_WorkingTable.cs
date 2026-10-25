using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;
using Apzon.Entities.Models.Production;
using Apzon.Forms.Commons;
using Apzon.Forms.Helper;
using Apzon.Forms.Properties;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTab;

namespace Apzon.Forms.Administrator;

public class frm_WorkingTable : XtraFormEx
{
	private string _FormController = "WorkingTable";

	private bool _isLoading;

	private RepositoryItemButtonEdit _bteDocEntryA;

	private RepositoryItemButtonEdit _bteDocEntryM;

	private RepositoryItemButtonEdit _bteDocEntryP;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private IContainer components;

	private GridControlEx grd_MyRequest;

	private GridControlEx grd_Line;

	private XtraTabControlEx xtraTabControl1;

	private XtraTabPageEx tabPg_Pending;

	private GridControlEx grd_Pending;

	private XtraTabPageEx tabPg_MyRQ;

	private XtraTabPageEx tabPg_AllRequest;

	private GridControlEx grd_MyRQLine;

	private GridControlEx grd_AllRequest;

	private SimpleButtonEx btn_Approve;

	private SimpleButtonEx btn_Rejected;

	private SimpleButtonEx btn_Close;

	private SimpleButtonEx btn_Save;

	private GridViewEx gv_MyRequest;

	private GridViewEx gv_Line;

	private GridViewEx gv_Pending;

	private GridViewEx gv_MyRQLine;

	private GridViewEx gv_AllRequest;

	private SplitContainerControl splitContainerControl1;

	private SplitContainerControl splitContainerControl2;

	private MemoEdit memo_Remarks;

	private Label label1;

	private SplitContainerControl splitContainerControl3;

	private GridControlEx grd_PendingStatus;

	private GridViewEx gv_PendingStatus;

	private SimpleButtonEx simpleButtonEx1;

	public frm_WorkingTable()
	{
		InitializeComponent();
		CreateRepositoryDrillDown();
	}

	public void CreateRepositoryDrillDown()
	{
		try
		{
			_bteDocEntryA = new RepositoryItemButtonEdit();
			_bteDocEntryA.Buttons[0].Kind = ButtonPredefines.Glyph;
			_bteDocEntryA.Buttons[0].IsLeft = true;
			_bteDocEntryA.Buttons[0].ImageLocation = ImageLocation.MiddleCenter;
			_bteDocEntryA.Buttons[0].Image = Apzon.Forms.Properties.Resources.right;
			_bteDocEntryA.Name = "_bte_DocEntryA";
			_bteDocEntryA.ButtonPressed += _bte_DocEntry_ButtonPressed;
			grd_AllRequest.RepositoryItems.Add(_bteDocEntryA);
			_bteDocEntryP = new RepositoryItemButtonEdit();
			_bteDocEntryP.Buttons[0].Kind = ButtonPredefines.Glyph;
			_bteDocEntryP.Buttons[0].IsLeft = true;
			_bteDocEntryP.Buttons[0].ImageLocation = ImageLocation.MiddleCenter;
			_bteDocEntryP.Buttons[0].Image = Apzon.Forms.Properties.Resources.right;
			_bteDocEntryP.Name = "_bte_DocEntryP";
			_bteDocEntryP.ButtonPressed += _bte_DocEntryP_ButtonPressed;
			grd_Pending.RepositoryItems.Add(_bteDocEntryP);
			_bteDocEntryM = new RepositoryItemButtonEdit();
			_bteDocEntryM.Buttons[0].Kind = ButtonPredefines.Glyph;
			_bteDocEntryM.Buttons[0].IsLeft = true;
			_bteDocEntryM.Buttons[0].ImageLocation = ImageLocation.MiddleCenter;
			_bteDocEntryM.Buttons[0].Image = Apzon.Forms.Properties.Resources.right;
			_bteDocEntryM.Name = "_bte_DocEntryM";
			_bteDocEntryM.ButtonPressed += _bte_DocEntryM_ButtonPressed;
			grd_MyRequest.RepositoryItems.Add(_bteDocEntryM);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			await LoadData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void frm_WorkingTable_Load(object sender, EventArgs e)
	{
		try
		{
			await LoadData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void gv_MyRequest_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		_ = 1;
		try
		{
			if (Function.ParseInt(gv_MyRequest.GetFocusedRowCellValue("WddCode")) > 0 && !_isLoading)
			{
				await LoadDataDetails();
				string text = Function.ToString(gv_MyRequest.GetFocusedRowCellValue("WasRead"));
				if (!string.IsNullOrEmpty(text) && text == "N")
				{
					int code = Function.ParseInt(gv_MyRequest.GetFocusedRowCellValue("AlertCode"));
					await SaveReadMessageChanged(code, "Y");
					gv_MyRequest.SetFocusedRowCellValue("WasRead", "Y");
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void gv_Pending_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		_ = 1;
		try
		{
			if (!string.IsNullOrWhiteSpace(Function.ToString(gv_Pending.GetFocusedRowCellValue("WddCode"))) && !_isLoading)
			{
				await LoadDataDetails();
				string text = Function.ToString(gv_Pending.GetFocusedRowCellValue("WasRead"));
				if (!string.IsNullOrEmpty(text) && text == "N")
				{
					int code = Function.ParseInt(gv_Pending.GetFocusedRowCellValue("AlertCode"));
					await SaveReadMessageChanged(code, "Y");
					gv_Pending.SetFocusedRowCellValue("WasRead", "Y");
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task SaveReadMessageChanged(int code, string wasread)
	{
		try
		{
			Tuple<int, string, string> pr = new Tuple<int, string, string>(code, wasread, GlobalData.UserName);
			using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "AlertOverview");
			await helper.PostAsync("SaveReadMessageChanged", pr);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void gv_AllRequest_FocusedRowObjectChanged_1(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			if (!string.IsNullOrWhiteSpace(Function.ToString(gv_AllRequest.GetFocusedRowCellValue("WddCode"))) && !_isLoading)
			{
				await LoadDataDetails();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_Close_Click(object sender, EventArgs e)
	{
		Close();
	}

	private async void btn_Approve_Click(object sender, EventArgs e)
	{
		await UpdateStatus("Y");
	}

	private async void btn_Save_Click(object sender, EventArgs e)
	{
		_ = 3;
		try
		{
			btn_Save.Enabled = false;
			int[] selected = gv_MyRequest.GetSelectedRows();
			if (selected.Length == 0)
			{
				WinCommons.Error(Enums.MessageCode.NoDocumentSelected, "");
				btn_Save.Enabled = true;
				return;
			}
			for (int i = 0; i < selected.Length; i++)
			{
				int entry = Function.ParseInt(gv_MyRequest.GetRowCellValue(selected[i], "DocNo"));
				int objType = Function.ParseInt(gv_MyRequest.GetRowCellValue(selected[i], "ObjType"));
				if (Function.ToString(gv_MyRequest.GetRowCellValue(selected[i], "Status")) != "Approved")
				{
					WinCommons.Error(Enums.MessageCode.CannotCreateDocumentThatHasNotApprovedWithDraftKey, "", Function.ToString(entry));
					continue;
				}
				WinCommons._OpenProcessing();
				List<Document_LinesImpl> lstLineDraft = new List<Document_LinesImpl>();
				DocumentsImpl documentsImpl;
				if (new int[2] { 1250000001, 67 }.Contains(objType))
				{
					documentsImpl = new DocumentsImpl
					{
						draftKey = entry,
						DocObjectCode = (ApzObjectTypes)objType
					};
				}
				else
				{
					IHttpClient httpClient = HttpClientHelperFactory.GetHttpClient((ApzObjectTypes)objType);
					DocumentTable tableDocDraft = await httpClient.Documents.CopyHeader(ApzObjectTypes.oDrafts, (ApzObjectTypes)objType, entry.ToString());
					DocumentLineTable documentLineTable = await httpClient.Documents.Copy(ApzObjectTypes.oDrafts, (ApzObjectTypes)objType, entry.ToString());
					DataTable dataTable = httpClient.Commons.CreateFreightGridDatasource("I");
					foreach (DataRow row in httpClient.Commons.GetFreight("DRF3", entry.ToString()).Rows)
					{
						DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["ExpnsCode"]) == Function.ParseInt(row["ExpnsCode"]));
						if (dataRow != null)
						{
							dataRow["LineTotal"] = Function.ParseDouble(row["LineTotal"]);
							dataRow["TotalFrgn"] = Function.ParseDouble(row["TotalFrgn"]);
							dataRow["Comments"] = Function.ToString(row["Comments"]);
							dataRow["VatGroup"] = Function.ToString(row["VatGroup"]);
							dataRow["VatPrcnt"] = Function.ParseDouble(row["VatPrcnt"]);
							dataRow["VatSum"] = Function.ParseDouble(row["VatSum"]);
							dataRow["DistrbMthd"] = Function.ToString(row["DistrbMthd"]);
							dataRow["GrsAmount"] = Function.ParseDouble(row["GrsAmount"]);
							dataRow["OcrCode"] = Function.ToString(row["OcrCode"]);
							dataRow["OcrCode2"] = Function.ToString(row["OcrCode2"]);
							dataRow["OcrCode3"] = Function.ToString(row["OcrCode3"]);
							dataRow["OcrCode4"] = Function.ToString(row["OcrCode4"]);
							dataRow["OcrCode5"] = Function.ToString(row["OcrCode5"]);
							dataRow["Project"] = Function.ToString(row["Project"]);
							dataRow["BaseType"] = Function.ParseInt(row["BaseType"]);
							dataRow["BaseAbsEnt"] = Function.ParseInt(row["BaseAbsEnt"]);
							dataRow["BaseLnNum"] = Function.ParseInt(row["BaseLnNum"]);
						}
					}
					dataTable.AcceptChanges();
					if (tableDocDraft != null && tableDocDraft.Rows.Count > 0)
					{
						documentsImpl = tableDocDraft[0].ToDocument();
						DataRow[] array = (from t in dataTable.Copy().AsEnumerable()
							where !string.IsNullOrEmpty(Function.ToString(t["ExpnsCode"])) && (Function.ParseDouble(t["LineTotal"]) != 0.0 || Function.ParseDouble(t["TotalFrgn"]) != 0.0)
							select t).ToArray();
						if (array.Length != 0)
						{
							documentsImpl.Expenses = array.CopyToDataTable().ToDocumentsAdditional();
						}
					}
					else
					{
						documentsImpl = null;
					}
					if (documentLineTable != null && documentLineTable.Rows.Count > 0)
					{
						Dictionary<string, string> docLineUdfNameAndTypeDictionary = WinFuntions.GetDocLineUdfNameAndTypeDictionary();
						lstLineDraft = documentLineTable.ToDocumentLine(docLineUdfNameAndTypeDictionary, priceSpecified: true);
						lstLineDraft = lstLineDraft.Where((Document_LinesImpl x) => !string.IsNullOrEmpty(x.ItemCode)).ToList();
						if (lstLineDraft.Count > 0)
						{
							DataTable dataTable2 = httpClient.Documents.CopyBatchSerial(entry.ToString(), ApzObjectTypes.oDrafts);
							if (dataTable2.IsNotNull())
							{
								DataTable dataTable3 = DocumentsHelper.BatchSerialNumbersStruct.Copy();
								string[] columnNames = (from DataColumn t in dataTable3.Columns
									select t.ColumnName).ToArray();
								dataTable2 = dataTable2.DefaultView.ToTable("Source", distinct: false, columnNames);
								DataRow dataRow2 = dataTable3.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["DistNumber"]).Equals(""));
								if (dataRow2 != null)
								{
									dataTable3.Rows.Remove(dataRow2);
									dataTable3.AcceptChanges();
								}
								foreach (DataRow row2 in dataTable2.Rows)
								{
									DataRow dataRow4 = dataTable3.NewRow();
									dataRow4.ItemArray = row2.ItemArray;
									dataTable3.Rows.Add(dataRow4);
								}
							}
							foreach (Document_LinesImpl line in lstLineDraft)
							{
								DataRow[] array2 = (from t in dataTable2.AsEnumerable()
									where Function.ParseInt(t["BaseLineNumber"]) == line.LineNum
									select t).ToArray();
								if (array2.Length != 0)
								{
									line.LoadDataBatch(array2.CopyToDataTable(), (ApzObjectTypes)objType);
								}
							}
						}
					}
				}
				if (documentsImpl != null)
				{
					documentsImpl.Lines = lstLineDraft;
					documentsImpl.DocObjectCode = (ApzObjectTypes)objType;
					documentsImpl.ObjType = objType;
					documentsImpl.AttachmentFiles = LoadAttachments(documentsImpl.AtcEntry);
					HttpResult httpResult;
					using (HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, _FormController))
					{
						httpResult = await helper.PostAsync("SaveDraftToDocument", documentsImpl);
					}
					if (!string.IsNullOrEmpty(httpResult.Message))
					{
						WinCommons.Error(Enums.MessageCode.None, httpResult.Message + entry + " - " + objType);
						continue;
					}
					WinCommons.Success(Enums.MessageCode.None, Apzon.Commons.Properties.Resources.Msg_Success + " Add draft to document " + httpResult.Content?.ToString() + "-" + entry + " - " + objType);
				}
			}
			await LoadData();
			btn_Save.Enabled = true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			btn_Save.Enabled = true;
		}
		WinCommons._CloseProcessing();
	}

	private List<AttachmentFile_Impl> LoadAttachments(int absEntry)
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "IssueForProduction");
			DataTable dataTable = httpClientHelper.Get("LoadAttachFiles", new { absEntry });
			if (!dataTable.IsNotNull())
			{
				dataTable = WinFuntions.GetTableStructure("ATC1");
			}
			dataTable.AcceptChanges();
			return dataTable.ToAttachmentFile();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		return new List<AttachmentFile_Impl>();
	}

	private async void btn_Rejected_Click(object sender, EventArgs e)
	{
		await UpdateStatus("N");
	}

	private async void xtraTabControl1_SelectedPageChanged(object sender, TabPageChangedEventArgs e)
	{
		try
		{
			await LoadDataDetails();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Pending_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
	}

	private void gv_MyRequest_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
	}

	private void _bte_DocEntry_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			int toType = Function.ParseInt(gv_AllRequest.GetFocusedRowCellValue("ObjType"));
			string id = Function.ToString(gv_AllRequest.GetFocusedRowCellValue("DocNo"));
			this.DrillDown(ApzObjectTypes.oDrafts, (ApzObjectTypes)toType, id);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void _bte_DocEntryM_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			int toType = Function.ParseInt(gv_MyRequest.GetFocusedRowCellValue("ObjType"));
			string id = Function.ToString(gv_MyRequest.GetFocusedRowCellValue("DocNo"));
			this.DrillDown(ApzObjectTypes.oDrafts, (ApzObjectTypes)toType, id);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void _bte_DocEntryP_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			int toType = Function.ParseInt(gv_Pending.GetFocusedRowCellValue("ObjType"));
			string id = Function.ToString(gv_Pending.GetFocusedRowCellValue("DocNo"));
			this.DrillDown(ApzObjectTypes.oDrafts, (ApzObjectTypes)toType, id);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadData()
	{
		_ = 2;
		try
		{
			_isLoading = true;
			WinCommons._OpenProcessing();
			await LoadDataMyRq();
			await LoadDataPending();
			await LoadDataAllRq();
			WinCommons._CloseProcessing();
			_isLoading = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			_isLoading = false;
		}
	}

	private async Task LoadDataMyRq()
	{
		_ = 2;
		try
		{
			using (HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, _FormController))
			{
				DataSet dataSet = await helper.GetAsync("GetWorkingTableDoc", new
				{
					type = "M"
				});
				if (dataSet.IsHasStructure())
				{
					await GridViewHelper.GenerateGrid(grd_MyRequest, gv_MyRequest, dataSet);
				}
				else if (dataSet.Tables.Count > 0)
				{
					grd_MyRequest.DataSource = null;
					grd_MyRequest.RefreshDataSource();
					gv_MyRequest.PopulateColumns();
				}
				if (gv_MyRequest.Columns["DocNo"] != null)
				{
					gv_MyRequest.Columns["DocNo"].ColumnEdit = _bteDocEntryM;
				}
			}
			await LoadDataDetails();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadDataPending()
	{
		_ = 2;
		try
		{
			using (HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, _FormController))
			{
				DataSet dataSet = await helper.GetAsync("GetWorkingTableDoc", new
				{
					type = "P"
				});
				if (dataSet.IsHasStructure())
				{
					await GridViewHelper.GenerateGrid(grd_Pending, gv_Pending, dataSet);
				}
				else if (dataSet.Tables.Count > 0)
				{
					grd_Pending.DataSource = null;
					grd_Pending.RefreshDataSource();
					gv_Pending.PopulateColumns();
				}
				if (gv_Pending.Columns["DocNo"] != null)
				{
					gv_Pending.Columns["DocNo"].ColumnEdit = _bteDocEntryP;
				}
			}
			await LoadDataDetails();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadDataAllRq()
	{
		_ = 2;
		try
		{
			using (HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, _FormController))
			{
				DataSet ds = await helper.GetAsync("GetWorkingTableDoc", new
				{
					type = "A"
				});
				if (ds.IsHasStructure())
				{
					await GridViewHelper.GenerateGrid(grd_AllRequest, gv_AllRequest, ds);
				}
				else
				{
					grd_AllRequest.DataSource = null;
					grd_AllRequest.RefreshDataSource();
					gv_AllRequest.PopulateColumns();
				}
				if (gv_AllRequest.Columns["DocNo"] != null)
				{
					gv_AllRequest.Columns["DocNo"].ColumnEdit = _bteDocEntryA;
				}
			}
			await LoadDataDetails();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			_isLoading = false;
		}
	}

	private async Task LoadDataDetails()
	{
		_ = 4;
		try
		{
			int id = 0;
			int entry = 0;
			switch (xtraTabControl1.SelectedTabPage.Name)
			{
			case "tabPg_MyRQ":
				id = Function.ParseInt(gv_MyRequest.GetFocusedRowCellValue("WddCode"));
				entry = Function.ParseInt(gv_MyRequest.GetFocusedRowCellValue("DocNo"));
				await LoadDataStepStatus(id);
				break;
			case "tabPg_Pending":
				id = Function.ParseInt(gv_Pending.GetFocusedRowCellValue("WddCode"));
				entry = Function.ParseInt(gv_Pending.GetFocusedRowCellValue("DocNo"));
				await LoadDataStepStatus(id, isPendingTab: true);
				break;
			case "tabPg_AllRequest":
				id = Function.ParseInt(gv_AllRequest.GetFocusedRowCellValue("WddCode"));
				entry = Function.ParseInt(gv_AllRequest.GetFocusedRowCellValue("DocNo"));
				break;
			}
			using (HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, _FormController))
			{
				DataSet dataSet = await helper.GetAsync("GetWorkingTableLine", new { id });
				if (dataSet != null && dataSet.Tables.Count > 1)
				{
					await GridViewHelper.GenerateGrid(grd_Line, gv_Line, dataSet.Tables[0], dataSet.Tables[1]);
				}
				else
				{
					grd_Line.DataSource = null;
					grd_Line.RefreshDataSource();
					gv_Line.PopulateColumns();
				}
			}
			await MarkReadMessage(Function.ParseInt(entry));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadDataStepStatus(int id, bool isPendingTab = false)
	{
		_ = 2;
		try
		{
			using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, _FormController);
			DataSet dataSet = await helper.GetAsync("GetDataStepStatus", new { id });
			if (dataSet != null && dataSet.Tables.Count > 0)
			{
				if (isPendingTab)
				{
					await GridViewHelper.GenerateGrid(grd_PendingStatus, gv_PendingStatus, dataSet.Tables[0], dataSet.Tables[1]);
				}
				else
				{
					await GridViewHelper.GenerateGrid(grd_MyRQLine, gv_MyRQLine, dataSet.Tables[0], dataSet.Tables[1]);
				}
			}
			else if (!isPendingTab)
			{
				grd_MyRQLine.DataSource = null;
				grd_MyRQLine.RefreshDataSource();
				gv_MyRQLine.PopulateColumns();
			}
			else
			{
				grd_PendingStatus.DataSource = null;
				grd_PendingStatus.RefreshDataSource();
				gv_PendingStatus.PopulateColumns();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void splitContainerControl2_Resize(object sender, EventArgs e)
	{
		splitContainerControl2.SplitterPosition = splitContainerControl2.Width * 2 / 3;
	}

	private async void simpleButtonEx1_Click(object sender, EventArgs e)
	{
		try
		{
			Dictionary<string, Stream> listAttachFile = new Dictionary<string, Stream>();
			SmtpMailClient smtpMailClient = new SmtpMailClient();
			List<string> listTo = new List<string> { "duonglh@apzon.com" };
			string body = File.ReadAllText("C:\\test.html");
			await smtpMailClient.SendEmail(listTo, "Yêu cầu phê duyệt", body, "Luyện Huy Dương", null, null, listAttachFile);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task UpdateStatus(string status)
	{
		_ = 1;
		try
		{
			btn_Approve.Enabled = false;
			btn_Rejected.Enabled = false;
			int[] selected = gv_Pending.GetSelectedRows();
			if (selected.Length == 0)
			{
				WinCommons.Error(Enums.MessageCode.NoDocumentSelected, "");
				btn_Approve.Enabled = true;
				btn_Rejected.Enabled = true;
				return;
			}
			WinCommons._OpenProcessing();
			for (int i = 0; i < selected.Length; i++)
			{
				DataTable dataTable = new DataTable();
				dataTable.Columns.Add(new DataColumn("WddCode", typeof(int)));
				dataTable.Columns.Add(new DataColumn("Password", typeof(string)));
				dataTable.Columns.Add(new DataColumn("status", typeof(string)));
				dataTable.Columns.Add(new DataColumn("Remarks", typeof(string)));
				DataRow dataRow = dataTable.NewRow();
				dataRow["WddCode"] = Function.ParseInt(gv_Pending.GetRowCellValue(selected[i], "WddCode"));
				dataRow["Password"] = GlobalData.OldPassword;
				dataRow["status"] = status;
				dataRow["Remarks"] = memo_Remarks.Text.Trim();
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
				string text = await _httpClient.WorkingTable.SetStatus(dataTable);
				if (string.IsNullOrWhiteSpace(text))
				{
					WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, text);
				}
			}
			await LoadData();
			btn_Approve.Enabled = true;
			btn_Rejected.Enabled = true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
			btn_Approve.Enabled = true;
			btn_Rejected.Enabled = true;
		}
		WinCommons._CloseProcessing();
	}

	private async Task MarkReadMessage(int docEntry)
	{
		try
		{
			using HttpClientHelper<string> helper = new HttpClientHelper<string>(Configuration.ApiServer, _FormController);
			string text = await helper.GetAsync("MarkReadMessage", new { docEntry });
			if (!string.IsNullOrWhiteSpace(text))
			{
				WinCommons.Error(Enums.MessageCode.None, text);
			}
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Administrator.frm_WorkingTable));
		this.grd_MyRequest = new Apzon.Controls.Grid.GridControlEx();
		this.gv_MyRequest = new Apzon.Controls.Grid.GridViewEx();
		this.grd_Line = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Line = new Apzon.Controls.Grid.GridViewEx();
		this.xtraTabControl1 = new Apzon.Controls.Controls.XtraTabControlEx();
		this.tabPg_Pending = new Apzon.Controls.Controls.XtraTabPageEx();
		this.splitContainerControl3 = new DevExpress.XtraEditors.SplitContainerControl();
		this.grd_Pending = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Pending = new Apzon.Controls.Grid.GridViewEx();
		this.grd_PendingStatus = new Apzon.Controls.Grid.GridControlEx();
		this.gv_PendingStatus = new Apzon.Controls.Grid.GridViewEx();
		this.memo_Remarks = new DevExpress.XtraEditors.MemoEdit();
		this.btn_Approve = new Apzon.Controls.Controls.SimpleButtonEx();
		this.label1 = new System.Windows.Forms.Label();
		this.btn_Rejected = new Apzon.Controls.Controls.SimpleButtonEx();
		this.tabPg_MyRQ = new Apzon.Controls.Controls.XtraTabPageEx();
		this.splitContainerControl2 = new DevExpress.XtraEditors.SplitContainerControl();
		this.grd_MyRQLine = new Apzon.Controls.Grid.GridControlEx();
		this.gv_MyRQLine = new Apzon.Controls.Grid.GridViewEx();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.tabPg_AllRequest = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_AllRequest = new Apzon.Controls.Grid.GridControlEx();
		this.gv_AllRequest = new Apzon.Controls.Grid.GridViewEx();
		this.btn_Close = new Apzon.Controls.Controls.SimpleButtonEx();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.simpleButtonEx1 = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.grd_MyRequest).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_MyRequest).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Line).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Line).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).BeginInit();
		this.xtraTabControl1.SuspendLayout();
		this.tabPg_Pending.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl3).BeginInit();
		this.splitContainerControl3.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Pending).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Pending).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_PendingStatus).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_PendingStatus).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.memo_Remarks.Properties).BeginInit();
		this.tabPg_MyRQ.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl2).BeginInit();
		this.splitContainerControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_MyRQLine).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_MyRQLine).BeginInit();
		this.tabPg_AllRequest.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_AllRequest).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_AllRequest).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		base.SuspendLayout();
		this.grd_MyRequest.AllowPasteExcel = false;
		this.grd_MyRequest.ClipboardData = null;
		this.grd_MyRequest.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_MyRequest.Location = new System.Drawing.Point(0, 0);
		this.grd_MyRequest.MainView = this.gv_MyRequest;
		this.grd_MyRequest.Name = "grd_MyRequest";
		this.grd_MyRequest.Size = new System.Drawing.Size(608, 187);
		this.grd_MyRequest.TabIndex = 1;
		this.grd_MyRequest.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_MyRequest });
		this.gv_MyRequest.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_MyRequest.ColumnKey = null;
		this.gv_MyRequest.DisableFormSetting = false;
		this.gv_MyRequest.DisplayException = false;
		this.gv_MyRequest.EditColumn = null;
		this.gv_MyRequest.FieldCurrency = null;
		this.gv_MyRequest.FieldRate = null;
		this.gv_MyRequest.FormId = "oWorkingTable";
		this.gv_MyRequest.GridControl = this.grd_MyRequest;
		this.gv_MyRequest.IgnoreFilter = false;
		this.gv_MyRequest.IndexEditRow = -1;
		this.gv_MyRequest.IndicatorWidth = 35;
		this.gv_MyRequest.IsApzTable = false;
		this.gv_MyRequest.IsInsertNewRow = false;
		this.gv_MyRequest.IsLoadingData = false;
		this.gv_MyRequest.IsMappingContextAction = false;
		this.gv_MyRequest.ItemId = "OMYREQUEST";
		this.gv_MyRequest.LstColumnAllowBatch = null;
		this.gv_MyRequest.LstColumnNotAllowBatch = null;
		this.gv_MyRequest.LstColumnNotEditable = null;
		this.gv_MyRequest.LstColumNotSetting = null;
		this.gv_MyRequest.Name = "gv_MyRequest";
		this.gv_MyRequest.OptionsSelection.MultiSelect = true;
		this.gv_MyRequest.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
		this.gv_MyRequest.OptionsView.ShowGroupPanel = false;
		this.gv_MyRequest.UseBatchMod = false;
		this.gv_MyRequest.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_MyRequest_RowCellStyle);
		this.gv_MyRequest.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_MyRequest_FocusedRowObjectChanged);
		this.grd_Line.AllowPasteExcel = false;
		this.grd_Line.ClipboardData = null;
		this.grd_Line.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Line.Location = new System.Drawing.Point(0, 0);
		this.grd_Line.MainView = this.gv_Line;
		this.grd_Line.Name = "grd_Line";
		this.grd_Line.Size = new System.Drawing.Size(1078, 272);
		this.grd_Line.TabIndex = 2;
		this.grd_Line.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Line });
		this.gv_Line.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Line.ColumnKey = null;
		this.gv_Line.DisableFormSetting = false;
		this.gv_Line.DisplayException = false;
		this.gv_Line.EditColumn = null;
		this.gv_Line.FieldCurrency = null;
		this.gv_Line.FieldRate = null;
		this.gv_Line.FormId = "oWorkingTable";
		this.gv_Line.GridControl = this.grd_Line;
		this.gv_Line.IgnoreFilter = false;
		this.gv_Line.IndexEditRow = -1;
		this.gv_Line.IndicatorWidth = 35;
		this.gv_Line.IsApzTable = false;
		this.gv_Line.IsInsertNewRow = false;
		this.gv_Line.IsLoadingData = false;
		this.gv_Line.IsMappingContextAction = false;
		this.gv_Line.ItemId = "OLINE";
		this.gv_Line.LstColumnAllowBatch = null;
		this.gv_Line.LstColumnNotAllowBatch = null;
		this.gv_Line.LstColumnNotEditable = null;
		this.gv_Line.LstColumNotSetting = null;
		this.gv_Line.Name = "gv_Line";
		this.gv_Line.OptionsView.ShowGroupPanel = false;
		this.gv_Line.UseBatchMod = false;
		this.xtraTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.xtraTabControl1.IsSearchControl = false;
		this.xtraTabControl1.Location = new System.Drawing.Point(0, 0);
		this.xtraTabControl1.Name = "xtraTabControl1";
		this.xtraTabControl1.SelectedTabPage = this.tabPg_Pending;
		this.xtraTabControl1.Size = new System.Drawing.Size(1078, 212);
		this.xtraTabControl1.TabIndex = 3;
		this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[3] { this.tabPg_Pending, this.tabPg_MyRQ, this.tabPg_AllRequest });
		this.xtraTabControl1.SelectedPageChanged += new DevExpress.XtraTab.TabPageChangedEventHandler(this.xtraTabControl1_SelectedPageChanged);
		this.tabPg_Pending.Controls.Add(this.splitContainerControl3);
		this.tabPg_Pending.EditControl = null;
		this.tabPg_Pending.EditValue = "PENDING";
		this.tabPg_Pending.FieldName = null;
		this.tabPg_Pending.IsSearchControl = false;
		this.tabPg_Pending.Name = "tabPg_Pending";
		this.tabPg_Pending.ReadOnly = false;
		this.tabPg_Pending.SettingInVisible = false;
		this.tabPg_Pending.SettingReadonly = false;
		this.tabPg_Pending.Size = new System.Drawing.Size(1076, 187);
		this.tabPg_Pending.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tabPg_Pending.Text = "PENDING";
		this.splitContainerControl3.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainerControl3.Location = new System.Drawing.Point(0, 0);
		this.splitContainerControl3.Name = "splitContainerControl3";
		this.splitContainerControl3.Panel1.Controls.Add(this.grd_Pending);
		this.splitContainerControl3.Panel1.Text = "Panel1";
		this.splitContainerControl3.Panel2.Controls.Add(this.grd_PendingStatus);
		this.splitContainerControl3.Panel2.Controls.Add(this.memo_Remarks);
		this.splitContainerControl3.Panel2.Controls.Add(this.btn_Approve);
		this.splitContainerControl3.Panel2.Controls.Add(this.label1);
		this.splitContainerControl3.Panel2.Controls.Add(this.btn_Rejected);
		this.splitContainerControl3.Panel2.Text = "Panel2";
		this.splitContainerControl3.Size = new System.Drawing.Size(1076, 187);
		this.splitContainerControl3.SplitterPosition = 488;
		this.splitContainerControl3.TabIndex = 9;
		this.splitContainerControl3.Text = "splitContainerControl3";
		this.grd_Pending.AllowPasteExcel = true;
		this.grd_Pending.ClipboardData = null;
		this.grd_Pending.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Pending.Location = new System.Drawing.Point(0, 0);
		this.grd_Pending.MainView = this.gv_Pending;
		this.grd_Pending.Name = "grd_Pending";
		this.grd_Pending.Size = new System.Drawing.Size(488, 187);
		this.grd_Pending.TabIndex = 2;
		this.grd_Pending.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Pending });
		this.gv_Pending.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Pending.ColumnKey = null;
		this.gv_Pending.DisableFormSetting = false;
		this.gv_Pending.DisplayException = false;
		this.gv_Pending.EditColumn = null;
		this.gv_Pending.FieldCurrency = null;
		this.gv_Pending.FieldRate = null;
		this.gv_Pending.FormId = "oWorkingTable";
		this.gv_Pending.GridControl = this.grd_Pending;
		this.gv_Pending.IgnoreFilter = false;
		this.gv_Pending.IndexEditRow = -1;
		this.gv_Pending.IndicatorWidth = 35;
		this.gv_Pending.IsApzTable = false;
		this.gv_Pending.IsInsertNewRow = false;
		this.gv_Pending.IsLoadingData = false;
		this.gv_Pending.IsMappingContextAction = false;
		this.gv_Pending.ItemId = "OPENDING";
		this.gv_Pending.LstColumnAllowBatch = null;
		this.gv_Pending.LstColumnNotAllowBatch = null;
		this.gv_Pending.LstColumnNotEditable = null;
		this.gv_Pending.LstColumNotSetting = null;
		this.gv_Pending.Name = "gv_Pending";
		this.gv_Pending.OptionsSelection.MultiSelect = true;
		this.gv_Pending.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
		this.gv_Pending.OptionsView.ShowGroupPanel = false;
		this.gv_Pending.UseBatchMod = false;
		this.gv_Pending.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Pending_RowCellStyle);
		this.gv_Pending.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_Pending_FocusedRowObjectChanged);
		this.grd_PendingStatus.AllowPasteExcel = true;
		this.grd_PendingStatus.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_PendingStatus.ClipboardData = null;
		this.grd_PendingStatus.Location = new System.Drawing.Point(2, 2);
		this.grd_PendingStatus.MainView = this.gv_PendingStatus;
		this.grd_PendingStatus.Name = "grd_PendingStatus";
		this.grd_PendingStatus.Size = new System.Drawing.Size(387, 184);
		this.grd_PendingStatus.TabIndex = 9;
		this.grd_PendingStatus.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_PendingStatus });
		this.gv_PendingStatus.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_PendingStatus.ColumnKey = null;
		this.gv_PendingStatus.DisableFormSetting = false;
		this.gv_PendingStatus.DisplayException = false;
		this.gv_PendingStatus.EditColumn = null;
		this.gv_PendingStatus.FieldCurrency = null;
		this.gv_PendingStatus.FieldRate = null;
		this.gv_PendingStatus.FormId = "oWorkingTable";
		this.gv_PendingStatus.GridControl = this.grd_PendingStatus;
		this.gv_PendingStatus.IgnoreFilter = false;
		this.gv_PendingStatus.IndexEditRow = -1;
		this.gv_PendingStatus.IndicatorWidth = 35;
		this.gv_PendingStatus.IsApzTable = false;
		this.gv_PendingStatus.IsInsertNewRow = false;
		this.gv_PendingStatus.IsLoadingData = false;
		this.gv_PendingStatus.IsMappingContextAction = false;
		this.gv_PendingStatus.ItemId = "OPENDINGSTATUS";
		this.gv_PendingStatus.LstColumnAllowBatch = null;
		this.gv_PendingStatus.LstColumnNotAllowBatch = null;
		this.gv_PendingStatus.LstColumnNotEditable = null;
		this.gv_PendingStatus.LstColumNotSetting = null;
		this.gv_PendingStatus.Name = "gv_PendingStatus";
		this.gv_PendingStatus.OptionsView.ColumnAutoWidth = false;
		this.gv_PendingStatus.OptionsView.ShowGroupPanel = false;
		this.gv_PendingStatus.UseBatchMod = false;
		this.memo_Remarks.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.memo_Remarks.EditValue = "";
		this.memo_Remarks.Location = new System.Drawing.Point(395, 31);
		this.memo_Remarks.Name = "memo_Remarks";
		this.memo_Remarks.Size = new System.Drawing.Size(181, 118);
		this.memo_Remarks.TabIndex = 8;
		this.btn_Approve.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Approve.EditControl = null;
		this.btn_Approve.EditValue = "Approved";
		this.btn_Approve.FieldName = null;
		this.btn_Approve.IsSearchControl = false;
		this.btn_Approve.Location = new System.Drawing.Point(395, 155);
		this.btn_Approve.Name = "btn_Approve";
		this.btn_Approve.ReadOnly = false;
		this.btn_Approve.SettingInVisible = false;
		this.btn_Approve.SettingReadonly = false;
		this.btn_Approve.Size = new System.Drawing.Size(75, 23);
		this.btn_Approve.TabIndex = 4;
		this.btn_Approve.Text = "Approved";
		this.btn_Approve.Click += new System.EventHandler(this.btn_Approve_Click);
		this.label1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.label1.AutoSize = true;
		this.label1.Location = new System.Drawing.Point(392, 2);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(48, 13);
		this.label1.TabIndex = 7;
		this.label1.Text = "Remarks";
		this.btn_Rejected.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Rejected.EditControl = null;
		this.btn_Rejected.EditValue = "Rejected";
		this.btn_Rejected.FieldName = null;
		this.btn_Rejected.IsSearchControl = false;
		this.btn_Rejected.Location = new System.Drawing.Point(476, 155);
		this.btn_Rejected.Name = "btn_Rejected";
		this.btn_Rejected.ReadOnly = false;
		this.btn_Rejected.SettingInVisible = false;
		this.btn_Rejected.SettingReadonly = false;
		this.btn_Rejected.Size = new System.Drawing.Size(75, 23);
		this.btn_Rejected.TabIndex = 6;
		this.btn_Rejected.Text = "Rejected";
		this.btn_Rejected.Click += new System.EventHandler(this.btn_Rejected_Click);
		this.tabPg_MyRQ.Controls.Add(this.splitContainerControl2);
		this.tabPg_MyRQ.EditControl = null;
		this.tabPg_MyRQ.EditValue = "MY REQUEST - PENDING";
		this.tabPg_MyRQ.FieldName = null;
		this.tabPg_MyRQ.IsSearchControl = false;
		this.tabPg_MyRQ.Name = "tabPg_MyRQ";
		this.tabPg_MyRQ.ReadOnly = false;
		this.tabPg_MyRQ.SettingInVisible = false;
		this.tabPg_MyRQ.SettingReadonly = false;
		this.tabPg_MyRQ.Size = new System.Drawing.Size(1076, 187);
		this.tabPg_MyRQ.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tabPg_MyRQ.Text = "MY REQUEST - PENDING";
		this.splitContainerControl2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainerControl2.Location = new System.Drawing.Point(0, 0);
		this.splitContainerControl2.Name = "splitContainerControl2";
		this.splitContainerControl2.Panel1.Controls.Add(this.grd_MyRequest);
		this.splitContainerControl2.Panel1.Text = "Panel1";
		this.splitContainerControl2.Panel2.Controls.Add(this.grd_MyRQLine);
		this.splitContainerControl2.Panel2.Controls.Add(this.btn_Save);
		this.splitContainerControl2.Panel2.Text = "Panel2";
		this.splitContainerControl2.Size = new System.Drawing.Size(1076, 187);
		this.splitContainerControl2.SplitterPosition = 608;
		this.splitContainerControl2.TabIndex = 9;
		this.splitContainerControl2.Text = "splitContainerControl2";
		this.splitContainerControl2.Resize += new System.EventHandler(this.splitContainerControl2_Resize);
		this.grd_MyRQLine.AllowPasteExcel = false;
		this.grd_MyRQLine.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_MyRQLine.ClipboardData = null;
		this.grd_MyRQLine.Location = new System.Drawing.Point(1, 0);
		this.grd_MyRQLine.MainView = this.gv_MyRQLine;
		this.grd_MyRQLine.Name = "grd_MyRQLine";
		this.grd_MyRQLine.Size = new System.Drawing.Size(394, 184);
		this.grd_MyRQLine.TabIndex = 2;
		this.grd_MyRQLine.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_MyRQLine });
		this.gv_MyRQLine.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_MyRQLine.ColumnKey = null;
		this.gv_MyRQLine.DisableFormSetting = false;
		this.gv_MyRQLine.DisplayException = false;
		this.gv_MyRQLine.EditColumn = null;
		this.gv_MyRQLine.FieldCurrency = null;
		this.gv_MyRQLine.FieldRate = null;
		this.gv_MyRQLine.FormId = "oWorkingTable";
		this.gv_MyRQLine.GridControl = this.grd_MyRQLine;
		this.gv_MyRQLine.IgnoreFilter = false;
		this.gv_MyRQLine.IndexEditRow = -1;
		this.gv_MyRQLine.IndicatorWidth = 35;
		this.gv_MyRQLine.IsApzTable = false;
		this.gv_MyRQLine.IsInsertNewRow = false;
		this.gv_MyRQLine.IsLoadingData = false;
		this.gv_MyRQLine.IsMappingContextAction = false;
		this.gv_MyRQLine.ItemId = "OMYRQLINE";
		this.gv_MyRQLine.LstColumnAllowBatch = null;
		this.gv_MyRQLine.LstColumnNotAllowBatch = null;
		this.gv_MyRQLine.LstColumnNotEditable = null;
		this.gv_MyRQLine.LstColumNotSetting = null;
		this.gv_MyRQLine.Name = "gv_MyRQLine";
		this.gv_MyRQLine.OptionsView.ColumnAutoWidth = false;
		this.gv_MyRQLine.OptionsView.ShowGroupPanel = false;
		this.gv_MyRQLine.UseBatchMod = false;
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Right;
		this.btn_Save.ButtonKind = Apzon.Commons.Enums.Enums.SimpleButtonType.Save;
		this.btn_Save.EditControl = null;
		this.btn_Save.EditValue = "Save";
		this.btn_Save.FieldName = null;
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(401, 75);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.ReadOnly = false;
		this.btn_Save.SettingInVisible = false;
		this.btn_Save.SettingReadonly = false;
		this.btn_Save.Size = new System.Drawing.Size(55, 23);
		this.btn_Save.TabIndex = 8;
		this.btn_Save.Text = "Save";
		this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
		this.tabPg_AllRequest.Controls.Add(this.grd_AllRequest);
		this.tabPg_AllRequest.EditControl = null;
		this.tabPg_AllRequest.EditValue = "ALL REQUEST";
		this.tabPg_AllRequest.FieldName = null;
		this.tabPg_AllRequest.IsSearchControl = false;
		this.tabPg_AllRequest.Name = "tabPg_AllRequest";
		this.tabPg_AllRequest.ReadOnly = false;
		this.tabPg_AllRequest.SettingInVisible = false;
		this.tabPg_AllRequest.SettingReadonly = false;
		this.tabPg_AllRequest.Size = new System.Drawing.Size(1076, 187);
		this.tabPg_AllRequest.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tabPg_AllRequest.Text = "ALL REQUEST";
		this.grd_AllRequest.AllowPasteExcel = false;
		this.grd_AllRequest.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_AllRequest.ClipboardData = null;
		this.grd_AllRequest.Location = new System.Drawing.Point(3, 0);
		this.grd_AllRequest.MainView = this.gv_AllRequest;
		this.grd_AllRequest.Name = "grd_AllRequest";
		this.grd_AllRequest.Size = new System.Drawing.Size(1066, 181);
		this.grd_AllRequest.TabIndex = 3;
		this.grd_AllRequest.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_AllRequest });
		this.gv_AllRequest.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_AllRequest.ColumnKey = null;
		this.gv_AllRequest.DisableFormSetting = false;
		this.gv_AllRequest.DisplayException = false;
		this.gv_AllRequest.EditColumn = null;
		this.gv_AllRequest.FieldCurrency = null;
		this.gv_AllRequest.FieldRate = null;
		this.gv_AllRequest.FormId = "oWorkingTable";
		this.gv_AllRequest.GridControl = this.grd_AllRequest;
		this.gv_AllRequest.IgnoreFilter = false;
		this.gv_AllRequest.IndexEditRow = -1;
		this.gv_AllRequest.IndicatorWidth = 35;
		this.gv_AllRequest.IsApzTable = false;
		this.gv_AllRequest.IsInsertNewRow = false;
		this.gv_AllRequest.IsLoadingData = false;
		this.gv_AllRequest.IsMappingContextAction = false;
		this.gv_AllRequest.ItemId = "OALLREQUEST";
		this.gv_AllRequest.LstColumnAllowBatch = null;
		this.gv_AllRequest.LstColumnNotAllowBatch = null;
		this.gv_AllRequest.LstColumnNotEditable = null;
		this.gv_AllRequest.LstColumNotSetting = null;
		this.gv_AllRequest.Name = "gv_AllRequest";
		this.gv_AllRequest.OptionsView.ShowGroupPanel = false;
		this.gv_AllRequest.UseBatchMod = false;
		this.gv_AllRequest.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_AllRequest_FocusedRowObjectChanged_1);
		this.btn_Close.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Close.EditControl = null;
		this.btn_Close.EditValue = "Close";
		this.btn_Close.FieldName = null;
		this.btn_Close.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Close.ImageOptions.Image");
		this.btn_Close.IsSearchControl = false;
		this.btn_Close.Location = new System.Drawing.Point(1, 501);
		this.btn_Close.Name = "btn_Close";
		this.btn_Close.ReadOnly = false;
		this.btn_Close.SettingInVisible = false;
		this.btn_Close.SettingReadonly = false;
		this.btn_Close.Size = new System.Drawing.Size(75, 23);
		this.btn_Close.TabIndex = 7;
		this.btn_Close.Text = "Close";
		this.btn_Close.Click += new System.EventHandler(this.btn_Close_Click);
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(1, 1);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.xtraTabControl1);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.grd_Line);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(1078, 494);
		this.splitContainerControl1.SplitterPosition = 212;
		this.splitContainerControl1.TabIndex = 8;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.simpleButtonEx1.EditControl = null;
		this.simpleButtonEx1.EditValue = "Send Email";
		this.simpleButtonEx1.FieldName = null;
		this.simpleButtonEx1.IsSearchControl = false;
		this.simpleButtonEx1.Location = new System.Drawing.Point(126, 502);
		this.simpleButtonEx1.Name = "simpleButtonEx1";
		this.simpleButtonEx1.ReadOnly = false;
		this.simpleButtonEx1.SettingInVisible = false;
		this.simpleButtonEx1.SettingReadonly = false;
		this.simpleButtonEx1.Size = new System.Drawing.Size(110, 23);
		this.simpleButtonEx1.TabIndex = 9;
		this.simpleButtonEx1.Text = "Send Email";
		this.simpleButtonEx1.Visible = false;
		this.simpleButtonEx1.Click += new System.EventHandler(this.simpleButtonEx1_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1079, 525);
		base.Controls.Add(this.simpleButtonEx1);
		base.Controls.Add(this.splitContainerControl1);
		base.Controls.Add(this.btn_Close);
		base.FormId = "oWorkingTable";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_WorkingTable";
		this.Text = "Working Table";
		base.Load += new System.EventHandler(this.frm_WorkingTable_Load);
		((System.ComponentModel.ISupportInitialize)this.grd_MyRequest).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_MyRequest).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Line).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Line).EndInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).EndInit();
		this.xtraTabControl1.ResumeLayout(false);
		this.tabPg_Pending.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl3).EndInit();
		this.splitContainerControl3.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Pending).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Pending).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_PendingStatus).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_PendingStatus).EndInit();
		((System.ComponentModel.ISupportInitialize)this.memo_Remarks.Properties).EndInit();
		this.tabPg_MyRQ.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl2).EndInit();
		this.splitContainerControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_MyRQLine).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_MyRQLine).EndInit();
		this.tabPg_AllRequest.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_AllRequest).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_AllRequest).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
