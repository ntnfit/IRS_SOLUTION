using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Inventory;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using Newtonsoft.Json;

namespace Apzon.Forms.Inventory.InventoryTransaction;

public class frm_InventoryRevaluation : XtraFormEx
{
	public delegate void ShowForms(XtraFormEx frm);

	public ShowForms frmShowChildrenForm;

	private int newDocEntry;

	private int docEntry;

	private IContainer components;

	private PanelControl panelControl1;

	private PanelControl panelControl2;

	private PanelControl panelControl3;

	private SimpleButtonEx btOK;

	private SimpleButtonEx btCancel;

	private LabelLine labelLine4;

	private LabelLine labelLine3;

	private LabelLine labelLine2;

	private LabelLine labelLine1;

	private DateEditEx dtDocDate;

	private DateEditEx dtPostingDate;

	private ComboBoxEditEx cbType;

	private TextEditEx txtDocEntry;

	private SplitContainerControl splitContainerControl1;

	private GridControlEx grdDoc;

	private GridViewEx gvDoc;

	private GroupControl groupControl1;

	private GridControlEx grdLine;

	private GridViewEx gvLine;

	private GridColumnEx gcItemCode;

	private GridColumnEx gcDscription;

	private TextEditEx txtJrnlMemo;

	private MemoEditEx txtComments;

	private LabelLine labelLine6;

	private LabelLine labelLine5;

	private GridColumnEx gcCurrentCost;

	private RepositoryItemCalcEdit repositoryItemCalcEdit3;

	private GridColumnEx gcNewCost;

	private RepositoryItemCalcEdit repositoryItemCalcEdit4;

	private GridColumnEx gcQuantity;

	private RepositoryItemCalcEdit repositoryItemCalcEdit5;

	private GridColumnEx gcDebitCredit;

	private RepositoryItemCalcEdit repositoryItemCalcEdit6;

	private GridColumnEx gcInStock;

	private RepositoryItemCalcEdit repositoryItemCalcEdit7;

	private GridColumnEx gcGLIncreaseAcc;

	private GridColumnEx gcGLDecreaseAcc;

	private GridColumnEx gcWarehouse;

	private GridColumnEx gcEvalSystem;

	private GridColumnEx gcDocEntry;

	private GridColumnEx gcLineNum;

	private GridColumnEx gcDocNo;

	private GridColumnEx gcEntryDate;

	private RepositoryItemDateEdit rEntryDate;

	private GridColumnEx gc2CurrentCost;

	private RepositoryItemCalcEdit repositoryItemCalcEdit1;

	private GridColumnEx gc2Price;

	private RepositoryItemCalcEdit repositoryItemCalcEdit2;

	private GridColumnEx gcOpenQty;

	private GridColumnEx gc2DocEntry;

	private GridColumnEx gc2LineNum;

	private GridColumnEx gc2Quantity;

	private RepositoryItemCalcEdit repositoryItemCalcEdit8;

	private GridColumnEx gc2DebitCredit;

	private RepositoryItemCalcEdit repositoryItemCalcEdit9;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem chooseItemToolStripMenuItem;

	private ToolStripMenuItem deleteRowToolStripMenuItem;

	public frm_InventoryRevaluation()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_InventoryRevaluation"))
		{
			InitializeComponent();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_InventoryRevaluation" && x.authoriza == "F"))
			{
				WinFuntions.DisableAllControl(Controls);
				btCancel.Enabled = true;
			}
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	public async Task MODE_FIND()
	{
		btOK.Text = "Find";
		dtDocDate.Text = string.Empty;
		dtPostingDate.Text = string.Empty;
		txtDocEntry.Text = string.Empty;
		List<InventoryRevaluation_LineMRV1Impl> dataSource = new List<InventoryRevaluation_LineMRV1Impl>();
		grdDoc.DataSource = dataSource;
		grdDoc.RefreshDataSource();
		new List<InventoryRevaluation_LineMRV2Impl>();
		grdLine.DataSource = dataSource;
		grdLine.RefreshDataSource();
	}

	public async Task REFRESH()
	{
		btOK.Text = "Add";
		RefreshData();
	}

	private void frm_InventoryRevaluation_Load(object sender, EventArgs e)
	{
		RefreshData();
	}

	private void RefreshData()
	{
		GetNewDocEntry();
		cbType.SelectedIndex = 0;
		dtDocDate.DateTime = DateTime.Today;
		dtPostingDate.DateTime = DateTime.Today;
		txtJrnlMemo.Text = "Inventory Revaluation";
		Reset();
	}

	private void Reset()
	{
		List<InventoryRevaluation_LineMRV1Impl> dataSource = new List<InventoryRevaluation_LineMRV1Impl>();
		grdDoc.DataSource = dataSource;
		grdDoc.RefreshDataSource();
		List<InventoryRevaluation_LineMRV2Impl> dataSource2 = new List<InventoryRevaluation_LineMRV2Impl>();
		grdLine.DataSource = dataSource2;
		grdLine.RefreshDataSource();
	}

	private async void GetNewDocEntry()
	{
		using HttpClient client = new HttpClient();
		string apiUri = Function.GetApiUri(Configuration.ApiServer, "InventoryRevaluation", "GetMaxDocEntry");
		using HttpResponseMessage response = await client.GetAsync(apiUri);
		if (response.IsSuccessStatusCode)
		{
			newDocEntry = JsonConvert.DeserializeObject<int>(await response.Content.ReadAsStringAsync()) + 1;
			if (btOK.Text.Equals("Add"))
			{
				txtDocEntry.Text = Function.ParseInt(newDocEntry).ToString();
			}
			else
			{
				txtDocEntry.Text = string.Empty;
			}
		}
	}

	private void gvDoc_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
	{
		contextMenuStrip1.Show(grdDoc, e.Point);
	}

	private void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (gvDoc.DataRowCount > 0 && XtraMessageBox.Show("Are  you sure want to delete?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				int num = Function.ParseInt(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, gcDocEntry));
				int lineNum = Function.ParseInt(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, gcLineNum));
				DeleteMRV1(num, lineNum);
				gvDoc.DeleteSelectedRows();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private async void DeleteMRV1(int docEntry, int lineNum)
	{
		_ = 1;
		try
		{
			_ = grdDoc.DataSource;
			using HttpClient client = new HttpClient();
			string apiUri = Function.GetApiUri(Configuration.ApiServer, "InventoryRevaluation", "GetDelete", new { docEntry, lineNum });
			using HttpResponseMessage response = await client.GetAsync(apiUri);
			if (response.IsSuccessStatusCode)
			{
				if ((await response.Content.ReadAsStringAsync()).ToString() == "null")
				{
					WinCommons.writeStatus(Enums.MessageCode.DeleteSuccess, Enums.MessageType.Success, Msg_Time._TimeLong, "");
				}
				else
				{
					WinCommons.writeStatus(Enums.MessageCode.Error, Enums.MessageType.Error, Msg_Time._TimeMedium, "");
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private void chooseItemToolStripMenuItem_Click(object sender, EventArgs e)
	{
		List<InventoryRevaluation_LineMRV1Impl> list = grdDoc.DataSource as List<InventoryRevaluation_LineMRV1Impl>;
		frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.Items, multiSelect: true, string.Empty);
		frm_ChooseFromList2.ShowDialog(this);
		if (frm_ChooseFromList2.ChooseItems == null)
		{
			return;
		}
		List<dynamic> chooseItems = frm_ChooseFromList2.ChooseItems;
		int num = 0;
		num = ((list != null && gvDoc.RowCount > 0) ? list.Max((InventoryRevaluation_LineMRV1Impl x) => x.LineNum) : 0);
		foreach (dynamic item in chooseItems)
		{
			InventoryRevaluation_LineMRV1Impl inventoryRevaluation_LineMRV1Impl = new InventoryRevaluation_LineMRV1Impl();
			inventoryRevaluation_LineMRV1Impl.ItemCode = item.ItemCode;
			inventoryRevaluation_LineMRV1Impl.Dscription = item.ItemName;
			inventoryRevaluation_LineMRV1Impl.RActPrice = item.Price;
			inventoryRevaluation_LineMRV1Impl.Price = 0.0;
			inventoryRevaluation_LineMRV1Impl.ROnHand = item.QuantityOnStock;
			inventoryRevaluation_LineMRV1Impl.Quantity = 1.0;
			inventoryRevaluation_LineMRV1Impl.Price = item.Price;
			inventoryRevaluation_LineMRV1Impl.WhsCode = item.DefaultWarehouse;
			inventoryRevaluation_LineMRV1Impl.LineTotal = inventoryRevaluation_LineMRV1Impl.Quantity * inventoryRevaluation_LineMRV1Impl.Price;
			inventoryRevaluation_LineMRV1Impl.EvalSystem = item.EvalSystem;
			inventoryRevaluation_LineMRV1Impl.DocEntry = Function.ParseInt(txtDocEntry.Text);
			num = (inventoryRevaluation_LineMRV1Impl.LineNum = num + 1);
			list.Add(inventoryRevaluation_LineMRV1Impl);
		}
		grdDoc.DataSource = list;
		grdDoc.RefreshDataSource();
	}

	private async void Save()
	{
		try
		{
			InventoryRevaluationImpl inventoryRevaluationImpl = new InventoryRevaluationImpl();
			InventoryRevaluation_LineMRV1Impl inventoryRevaluation_LineMRV1Impl = new InventoryRevaluation_LineMRV1Impl();
			List<InventoryRevaluation_LineMRV1Impl> list = (List<InventoryRevaluation_LineMRV1Impl>)grdDoc.DataSource;
			list.FirstOrDefault((InventoryRevaluation_LineMRV1Impl p) => p.DocEntry == newDocEntry);
			int focusedLine = Function.ParseInt(gvDoc.GetFocusedRowCellValue(gcLineNum));
			list.FirstOrDefault((InventoryRevaluation_LineMRV1Impl p) => p.DocEntry == docEntry && p.LineNum == focusedLine);
			int docNum = (inventoryRevaluationImpl.DocEntry = Function.ParseInt(txtDocEntry.Text.ToString().Trim()));
			inventoryRevaluationImpl.DocNum = docNum;
			inventoryRevaluationImpl.DocDate = DateTime.Today;
			inventoryRevaluationImpl.TaxDate = DateTime.Today;
			inventoryRevaluationImpl.Comments = txtComments.Text.ToString().Trim();
			inventoryRevaluationImpl.JrnlMemo = txtJrnlMemo.Text.ToString().Trim();
			inventoryRevaluationImpl.RevalType = cbType.Text;
			inventoryRevaluationImpl.DataSource = "I";
			inventoryRevaluationImpl.Series = 22;
			inventoryRevaluationImpl.UserSign = 3;
			inventoryRevaluationImpl.ObjType = "162";
			inventoryRevaluation_LineMRV1Impl.ItemCode = Function.ToString(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, gcItemCode));
			inventoryRevaluation_LineMRV1Impl.Dscription = Function.ToString(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, gcDscription));
			inventoryRevaluation_LineMRV1Impl.RActPrice = Function.ParseDouble(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, gcCurrentCost));
			inventoryRevaluation_LineMRV1Impl.Price = Function.ParseDouble(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, gcNewCost));
			inventoryRevaluation_LineMRV1Impl.Quantity = Function.ParseDouble(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, gcQuantity));
			inventoryRevaluation_LineMRV1Impl.ROnHand = Function.ParseDouble(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, gcInStock));
			inventoryRevaluation_LineMRV1Impl.RIncmAcct = Function.ToString(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, gcGLIncreaseAcc));
			inventoryRevaluation_LineMRV1Impl.RDcrmAcct = Function.ToString(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, gcGLDecreaseAcc));
			inventoryRevaluation_LineMRV1Impl.WhsCode = Function.ToString(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, gcWarehouse));
			inventoryRevaluation_LineMRV1Impl.EvalSystem = Function.ToString(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, gcEvalSystem));
			inventoryRevaluation_LineMRV1Impl.LineNum = Function.ParseInt(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, gcLineNum));
			inventoryRevaluation_LineMRV1Impl.DocEntry = newDocEntry;
			inventoryRevaluationImpl.LinesMRV1 = list;
			if (gvDoc.RowCount > 0 && XtraMessageBox.Show(Resources.xtraMsg_Save, "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				await Save(inventoryRevaluationImpl);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private async Task<string> Save(InventoryRevaluationImpl doc)
	{
		HttpRequestMessage httpRequestMessage = new HttpRequestMessage();
		new MultipartFormDataContent();
		httpRequestMessage.Content = new ObjectContent<InventoryRevaluationImpl>(doc, new JsonMediaTypeFormatter());
		httpRequestMessage.Method = HttpMethod.Post;
		string apiUri = Function.GetApiUri(Configuration.ApiServer, "InventoryRevaluation", "SaveDoc");
		httpRequestMessage.RequestUri = new Uri(apiUri);
		new List<InventoryRevaluationImpl>();
		List<InventoryRevaluation_LineMRV1Impl> lstLine = new List<InventoryRevaluation_LineMRV1Impl>();
		await new HttpClient().SendAsync(httpRequestMessage).ContinueWith((Task<HttpResponseMessage> task) =>
		{
			if (task.Result.IsSuccessStatusCode)
			{
				Task<string> task2 = task.Result.Content.ReadAsStringAsync();
				if (task2 != null && string.IsNullOrEmpty(task2.Result))
				{
					return task2.Result;
				}
				BeginInvoke((MethodInvoker)(() =>
				{
					WinCommons.writeStatus(Enums.MessageCode.Success, Enums.MessageType.Success, Msg_Time._TimeMedium, "");
					grdDoc.DataSource = lstLine;
					grdDoc.RefreshDataSource();
					if (btOK.Text.Equals("Update"))
					{
						btOK.Text = "Add";
						EnabledDoc();
					}
					if (btOK.Text.Equals("Add"))
					{
						EnabledDoc_True();
					}
					GetNewDocEntry();
				}));
			}
			return (string)null;
		});
		return null;
	}

	private async void Search()
	{
		InventoryRevaluationImpl inventoryRevaluationImpl = new InventoryRevaluationImpl();
		if (string.IsNullOrEmpty(txtDocEntry.Text.ToString().Trim()))
		{
			ChooseListInventoryRevaluation();
			return;
		}
		try
		{
			inventoryRevaluationImpl.DocEntry = Function.ParseInt(txtDocEntry.Text);
			inventoryRevaluationImpl.DocDate = dtDocDate.DateTime;
			inventoryRevaluationImpl.TaxDate = dtPostingDate.DateTime;
			HttpRequestMessage httpRequestMessage = new HttpRequestMessage();
			httpRequestMessage.Content = new ObjectContent<InventoryRevaluationImpl>(inventoryRevaluationImpl, new JsonMediaTypeFormatter());
			httpRequestMessage.Method = HttpMethod.Post;
			httpRequestMessage.RequestUri = new Uri(Function.GetApiUri(Configuration.ApiServer, "InventoryRevaluation", "Search"));
			List<InventoryRevaluationImpl> lst = new List<InventoryRevaluationImpl>();
			new List<InventoryRevaluation_LineMRV1Impl>();
			new List<InventoryRevaluation_LineMRV2Impl>();
			await new HttpClient().SendAsync(httpRequestMessage).ContinueWith((Task<HttpResponseMessage> task) =>
			{
				if (task.Result.IsSuccessStatusCode)
				{
					Task<string> task2 = task.Result.Content.ReadAsStringAsync();
					if (task2 != null && !string.IsNullOrEmpty(task2.Result))
					{
						lst = JsonConvert.DeserializeObject<List<InventoryRevaluationImpl>>(task2.Result).ToList();
						if (lst != null)
						{
							InventoryRevaluationImpl obj = new InventoryRevaluationImpl();
							obj = lst.FirstOrDefault();
							BeginInvoke((MethodInvoker)(() =>
							{
								if (obj != null)
								{
									txtDocEntry.Text = Function.ToString(obj.DocEntry);
									dtDocDate.DateTime = obj.DocDate;
									dtPostingDate.DateTime = obj.TaxDate;
									cbType.Text = obj.RevalType;
									txtComments.Text = obj.Comments;
									txtJrnlMemo.Text = obj.JrnlMemo;
									lst.Add(obj);
									SearchLineMRV1(obj.DocEntry);
									SearchLineMRV2(obj.DocEntry);
									btOK.Text = "Update";
								}
								else
								{
									BeginInvoke((MethodInvoker)(() =>
									{
										btOK.Text = "Find";
									}));
								}
							}));
						}
					}
				}
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private async void SearchLineMRV2(int idDocEntry)
	{
		_ = 1;
		try
		{
			using HttpClient client = new HttpClient();
			string apiUri = Function.GetApiUri(Configuration.ApiServer, "InventoryRevaluation", "GetLineMRV2", new { idDocEntry });
			using HttpResponseMessage response = await client.GetAsync(apiUri);
			if (response.IsSuccessStatusCode)
			{
				List<InventoryRevaluation_LineMRV2Impl> dataSource = JsonConvert.DeserializeObject<InventoryRevaluation_LineMRV2Impl[]>(await response.Content.ReadAsStringAsync()).ToList();
				grdLine.DataSource = dataSource;
				grdLine.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private void ChooseListInventoryRevaluation()
	{
	}

	private void EnabledDoc_True()
	{
		txtDocEntry.Enabled = true;
	}

	private void EnabledDoc()
	{
		txtDocEntry.Enabled = false;
	}

	private async void SearchLineMRV1(int idDocEntry)
	{
		_ = 1;
		try
		{
			using HttpClient client = new HttpClient();
			string apiUri = Function.GetApiUri(Configuration.ApiServer, "InventoryRevaluation", "GetLineMRV1", new { idDocEntry });
			using HttpResponseMessage response = await client.GetAsync(apiUri);
			if (response.IsSuccessStatusCode)
			{
				List<InventoryRevaluation_LineMRV1Impl> dataSource = JsonConvert.DeserializeObject<InventoryRevaluation_LineMRV1Impl[]>(await response.Content.ReadAsStringAsync()).ToList();
				grdDoc.DataSource = dataSource;
				grdDoc.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private void EnabledContextMenuStrip()
	{
		deleteRowToolStripMenuItem.Enabled = false;
	}

	private void EnabledTrueContextMenuStrip()
	{
		deleteRowToolStripMenuItem.Enabled = true;
	}

	private void btOK_Click(object sender, EventArgs e)
	{
		if (btOK.Text.Equals("Add") || btOK.Text.Equals("Update"))
		{
			Save();
		}
		if (btOK.Text.Equals("Find"))
		{
			Search();
		}
	}

	private void btCancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void gvDoc_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void gvLine_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Inventory.InventoryTransaction.frm_InventoryRevaluation));
		this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
		this.dtDocDate = new Apzon.Controls.Editor.DateEditEx();
		this.dtPostingDate = new Apzon.Controls.Editor.DateEditEx();
		this.cbType = new Apzon.Controls.Controls.ComboBoxEditEx();
		this.txtDocEntry = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
		this.txtJrnlMemo = new Apzon.Controls.Editor.TextEditEx();
		this.txtComments = new Apzon.Controls.Editor.MemoEditEx();
		this.btOK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.labelLine6 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine5 = new Apzon.Controls.Controls.LabelLine();
		this.panelControl3 = new DevExpress.XtraEditors.PanelControl();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.grdDoc = new Apzon.Controls.Grid.GridControlEx();
		this.gvDoc = new Apzon.Controls.Grid.GridViewEx();
		this.gcItemCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDscription = new Apzon.Controls.Grid.GridColumnEx();
		this.gcCurrentCost = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEdit3 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gcNewCost = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEdit4 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gcQuantity = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEdit5 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gcDebitCredit = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEdit6 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gcInStock = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEdit7 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gcGLIncreaseAcc = new Apzon.Controls.Grid.GridColumnEx();
		this.gcGLDecreaseAcc = new Apzon.Controls.Grid.GridColumnEx();
		this.gcWarehouse = new Apzon.Controls.Grid.GridColumnEx();
		this.gcEvalSystem = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDocEntry = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineNum = new Apzon.Controls.Grid.GridColumnEx();
		this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
		this.grdLine = new Apzon.Controls.Grid.GridControlEx();
		this.gvLine = new Apzon.Controls.Grid.GridViewEx();
		this.gcDocNo = new Apzon.Controls.Grid.GridColumnEx();
		this.gcEntryDate = new Apzon.Controls.Grid.GridColumnEx();
		this.rEntryDate = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.gc2CurrentCost = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gc2Price = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gc2Quantity = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEdit8 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gc2DebitCredit = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEdit9 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gcOpenQty = new Apzon.Controls.Grid.GridColumnEx();
		this.gc2DocEntry = new Apzon.Controls.Grid.GridColumnEx();
		this.gc2LineNum = new Apzon.Controls.Grid.GridColumnEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.chooseItemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		((System.ComponentModel.ISupportInitialize)this.panelControl1).BeginInit();
		this.panelControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.dtDocDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dtDocDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dtPostingDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dtPostingDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbType.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtDocEntry.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl2).BeginInit();
		this.panelControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txtJrnlMemo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtComments.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl3).BeginInit();
		this.panelControl3.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit6).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit7).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.groupControl1).BeginInit();
		this.groupControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdLine).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gvLine).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rEntryDate).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rEntryDate.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit8).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit9).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		base.SuspendLayout();
		this.panelControl1.Controls.Add(this.dtDocDate);
		this.panelControl1.Controls.Add(this.dtPostingDate);
		this.panelControl1.Controls.Add(this.cbType);
		this.panelControl1.Controls.Add(this.txtDocEntry);
		this.panelControl1.Controls.Add(this.labelLine4);
		this.panelControl1.Controls.Add(this.labelLine3);
		this.panelControl1.Controls.Add(this.labelLine2);
		this.panelControl1.Controls.Add(this.labelLine1);
		this.panelControl1.Dock = System.Windows.Forms.DockStyle.Top;
		this.panelControl1.Location = new System.Drawing.Point(0, 0);
		this.panelControl1.Name = "panelControl1";
		this.panelControl1.Size = new System.Drawing.Size(787, 84);
		this.panelControl1.TabIndex = 0;
		this.dtDocDate.EditControl = null;
		this.dtDocDate.EditValue = null;
		this.dtDocDate.FieldName = null;
		this.dtDocDate.Location = new System.Drawing.Point(401, 49);
		this.dtDocDate.Name = "dtDocDate";
		this.dtDocDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dtDocDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dtDocDate.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.dtDocDate.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.dtDocDate.Properties.EditFormat.FormatString = "dd/MM/yyyy";
		this.dtDocDate.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.dtDocDate.Properties.IsSearchControl = false;
		this.dtDocDate.Properties.Mask.EditMask = "dd/MM/yyyy";
		this.dtDocDate.SettingInVisible = false;
		this.dtDocDate.SettingReadonly = false;
		this.dtDocDate.Size = new System.Drawing.Size(149, 20);
		this.dtDocDate.TabIndex = 3;
		this.dtPostingDate.EditControl = null;
		this.dtPostingDate.EditValue = null;
		this.dtPostingDate.FieldName = null;
		this.dtPostingDate.Location = new System.Drawing.Point(401, 12);
		this.dtPostingDate.Name = "dtPostingDate";
		this.dtPostingDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dtPostingDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dtPostingDate.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.dtPostingDate.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.dtPostingDate.Properties.EditFormat.FormatString = "dd/MM/yyyy";
		this.dtPostingDate.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.dtPostingDate.Properties.IsSearchControl = false;
		this.dtPostingDate.Properties.Mask.EditMask = "dd/MM/yyyy";
		this.dtPostingDate.SettingInVisible = false;
		this.dtPostingDate.SettingReadonly = false;
		this.dtPostingDate.Size = new System.Drawing.Size(149, 20);
		this.dtPostingDate.TabIndex = 3;
		this.cbType.EditControl = null;
		this.cbType.FieldName = null;
		this.cbType.IsSearchControl = false;
		this.cbType.Location = new System.Drawing.Point(105, 49);
		this.cbType.Name = "cbType";
		this.cbType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbType.Properties.Items.AddRange(new object[2] { "Price Change", "Inventory Debit/Credit" });
		this.cbType.SettingInVisible = false;
		this.cbType.SettingReadonly = false;
		this.cbType.Size = new System.Drawing.Size(131, 20);
		this.cbType.TabIndex = 2;
		this.txtDocEntry.EditControl = null;
		this.txtDocEntry.FieldName = null;
		this.txtDocEntry.IsSearchControl = false;
		this.txtDocEntry.Location = new System.Drawing.Point(105, 12);
		this.txtDocEntry.Name = "txtDocEntry";
		this.txtDocEntry.SettingInVisible = false;
		this.txtDocEntry.SettingReadonly = false;
		this.txtDocEntry.Size = new System.Drawing.Size(131, 20);
		this.txtDocEntry.TabIndex = 1;
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "Document Date";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(314, 49);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(93, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 0;
		this.labelLine4.Utext = "Document Date";
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "Posting Date";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(315, 12);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(92, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 0;
		this.labelLine3.Utext = "Posting Date";
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "Inventory Type";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(19, 49);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(94, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 0;
		this.labelLine2.Utext = "Inventory Type";
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Number";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(19, 12);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(94, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 0;
		this.labelLine1.Utext = "Number";
		this.panelControl2.Controls.Add(this.txtJrnlMemo);
		this.panelControl2.Controls.Add(this.txtComments);
		this.panelControl2.Controls.Add(this.btOK);
		this.panelControl2.Controls.Add(this.btCancel);
		this.panelControl2.Controls.Add(this.labelLine6);
		this.panelControl2.Controls.Add(this.labelLine5);
		this.panelControl2.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.panelControl2.Location = new System.Drawing.Point(0, 419);
		this.panelControl2.Name = "panelControl2";
		this.panelControl2.Size = new System.Drawing.Size(787, 95);
		this.panelControl2.TabIndex = 1;
		this.txtJrnlMemo.EditControl = null;
		this.txtJrnlMemo.FieldName = null;
		this.txtJrnlMemo.IsSearchControl = false;
		this.txtJrnlMemo.Location = new System.Drawing.Point(105, 44);
		this.txtJrnlMemo.Name = "txtJrnlMemo";
		this.txtJrnlMemo.SettingInVisible = false;
		this.txtJrnlMemo.SettingReadonly = false;
		this.txtJrnlMemo.Size = new System.Drawing.Size(670, 20);
		this.txtJrnlMemo.TabIndex = 43;
		this.txtComments.EditControl = null;
		this.txtComments.FieldName = null;
		this.txtComments.IsSearchControl = false;
		this.txtComments.Location = new System.Drawing.Point(105, 7);
		this.txtComments.Name = "txtComments";
		this.txtComments.SettingInVisible = false;
		this.txtComments.SettingReadonly = false;
		this.txtComments.Size = new System.Drawing.Size(670, 34);
		this.txtComments.TabIndex = 42;
		this.btOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btOK.ButtonKind = Apzon.Commons.Enums.Enums.SimpleButtonType.Add;
		this.btOK.EditControl = null;
		this.btOK.EditValue = "Add";
		this.btOK.FieldName = null;
		this.btOK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btOK.ImageOptions.Image");
		this.btOK.IsSearchControl = false;
		this.btOK.Location = new System.Drawing.Point(0, 70);
		this.btOK.Name = "btOK";
		this.btOK.ReadOnly = false;
		this.btOK.SettingInVisible = false;
		this.btOK.SettingReadonly = false;
		this.btOK.Size = new System.Drawing.Size(99, 20);
		this.btOK.TabIndex = 40;
		this.btOK.Text = "Add";
		this.btOK.Click += new System.EventHandler(this.btOK_Click);
		this.btCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btCancel.EditControl = null;
		this.btCancel.EditValue = "Cancel";
		this.btCancel.FieldName = null;
		this.btCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btCancel.ImageOptions.Image");
		this.btCancel.IsSearchControl = false;
		this.btCancel.Location = new System.Drawing.Point(105, 70);
		this.btCancel.Name = "btCancel";
		this.btCancel.ReadOnly = false;
		this.btCancel.SettingInVisible = false;
		this.btCancel.SettingReadonly = false;
		this.btCancel.Size = new System.Drawing.Size(99, 20);
		this.btCancel.TabIndex = 41;
		this.btCancel.Text = "Cancel";
		this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
		this.labelLine6.EditControl = null;
		this.labelLine6.EditValue = "Journal Remark";
		this.labelLine6.FieldName = null;
		this.labelLine6.HideWhenEmpty = false;
		this.labelLine6.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine6.Location = new System.Drawing.Point(2, 44);
		this.labelLine6.Name = "labelLine6";
		this.labelLine6.ReadOnly = false;
		this.labelLine6.SettingInVisible = false;
		this.labelLine6.SettingReadonly = false;
		this.labelLine6.ShowUnderLine = true;
		this.labelLine6.Size = new System.Drawing.Size(111, 20);
		this.labelLine6.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine6.TabIndex = 0;
		this.labelLine6.Utext = "Journal Remark";
		this.labelLine5.EditControl = null;
		this.labelLine5.EditValue = "Remark";
		this.labelLine5.FieldName = null;
		this.labelLine5.HideWhenEmpty = false;
		this.labelLine5.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine5.Location = new System.Drawing.Point(4, 5);
		this.labelLine5.Name = "labelLine5";
		this.labelLine5.ReadOnly = false;
		this.labelLine5.SettingInVisible = false;
		this.labelLine5.SettingReadonly = false;
		this.labelLine5.ShowUnderLine = true;
		this.labelLine5.Size = new System.Drawing.Size(109, 20);
		this.labelLine5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine5.TabIndex = 0;
		this.labelLine5.Utext = "Remark";
		this.panelControl3.Controls.Add(this.splitContainerControl1);
		this.panelControl3.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panelControl3.Location = new System.Drawing.Point(0, 84);
		this.panelControl3.Name = "panelControl3";
		this.panelControl3.Size = new System.Drawing.Size(787, 335);
		this.panelControl3.TabIndex = 2;
		this.splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(2, 2);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.grdDoc);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.groupControl1);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.RightToLeft = System.Windows.Forms.RightToLeft.No;
		this.splitContainerControl1.Size = new System.Drawing.Size(783, 331);
		this.splitContainerControl1.SplitterPosition = 157;
		this.splitContainerControl1.TabIndex = 0;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.grdDoc.AllowPasteExcel = true;
		this.grdDoc.ClipboardData = null;
		this.grdDoc.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grdDoc.Location = new System.Drawing.Point(0, 0);
		this.grdDoc.MainView = this.gvDoc;
		this.grdDoc.Name = "grdDoc";
		this.grdDoc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[5] { this.repositoryItemCalcEdit3, this.repositoryItemCalcEdit4, this.repositoryItemCalcEdit5, this.repositoryItemCalcEdit6, this.repositoryItemCalcEdit7 });
		this.grdDoc.Size = new System.Drawing.Size(783, 157);
		this.grdDoc.TabIndex = 0;
		this.grdDoc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvDoc });
		this.gvDoc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvDoc.ColumnKey = null;
		this.gvDoc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[13]
		{
			this.gcItemCode, this.gcDscription, this.gcCurrentCost, this.gcNewCost, this.gcQuantity, this.gcDebitCredit, this.gcInStock, this.gcGLIncreaseAcc, this.gcGLDecreaseAcc, this.gcWarehouse,
			this.gcEvalSystem, this.gcDocEntry, this.gcLineNum
		});
		this.gvDoc.DisableFormSetting = false;
		this.gvDoc.DisplayException = false;
		this.gvDoc.EditColumn = null;
		this.gvDoc.FieldCurrency = null;
		this.gvDoc.FieldRate = null;
		this.gvDoc.FormId = "oInventoryRevaluation";
		this.gvDoc.GridControl = this.grdDoc;
		this.gvDoc.IgnoreFilter = false;
		this.gvDoc.IndexEditRow = -1;
		this.gvDoc.IndicatorWidth = 35;
		this.gvDoc.IsApzTable = false;
		this.gvDoc.IsInsertNewRow = false;
		this.gvDoc.IsLoadingData = false;
		this.gvDoc.IsMappingContextAction = false;
		this.gvDoc.ItemId = "ODOC";
		this.gvDoc.LstColumnAllowBatch = null;
		this.gvDoc.LstColumnNotAllowBatch = null;
		this.gvDoc.LstColumnNotEditable = null;
		this.gvDoc.LstColumNotSetting = null;
		this.gvDoc.Name = "gvDoc";
		this.gvDoc.OptionsView.ColumnAutoWidth = false;
		this.gvDoc.OptionsView.ShowFooter = true;
		this.gvDoc.OptionsView.ShowGroupPanel = false;
		this.gvDoc.UseBatchMod = false;
		this.gvDoc.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gvDoc_CustomDrawRowIndicator);
		this.gvDoc.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(this.gvDoc_PopupMenuShowing);
		this.gcItemCode.Caption = "Item Code";
		this.gcItemCode.CurrencyDateField = null;
		this.gcItemCode.CurrencyField = null;
		this.gcItemCode.CurrencyFixed = null;
		this.gcItemCode.CurrencyFixedRate = 0.0;
		this.gcItemCode.CurrencyRateField = null;
		this.gcItemCode.FieldName = "ItemCode";
		this.gcItemCode.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gcItemCode.Name = "gcItemCode";
		this.gcItemCode.PrimaryKey = true;
		this.gcItemCode.UnitField = null;
		this.gcItemCode.Visible = true;
		this.gcItemCode.VisibleIndex = 0;
		this.gcItemCode.Width = 102;
		this.gcDscription.Caption = "Dscription";
		this.gcDscription.CurrencyDateField = null;
		this.gcDscription.CurrencyField = null;
		this.gcDscription.CurrencyFixed = null;
		this.gcDscription.CurrencyFixedRate = 0.0;
		this.gcDscription.CurrencyRateField = null;
		this.gcDscription.FieldName = "Dscription";
		this.gcDscription.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gcDscription.Name = "gcDscription";
		this.gcDscription.PrimaryKey = false;
		this.gcDscription.UnitField = null;
		this.gcDscription.Visible = true;
		this.gcDscription.VisibleIndex = 1;
		this.gcDscription.Width = 175;
		this.gcCurrentCost.Caption = "Current Cost";
		this.gcCurrentCost.ColumnEdit = this.repositoryItemCalcEdit3;
		this.gcCurrentCost.CurrencyDateField = null;
		this.gcCurrentCost.CurrencyField = null;
		this.gcCurrentCost.CurrencyFixed = null;
		this.gcCurrentCost.CurrencyFixedRate = 0.0;
		this.gcCurrentCost.CurrencyRateField = null;
		this.gcCurrentCost.DisplayFormat.FormatString = "n0";
		this.gcCurrentCost.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gcCurrentCost.FieldName = "RActPrice";
		this.gcCurrentCost.Name = "gcCurrentCost";
		this.gcCurrentCost.PrimaryKey = false;
		this.gcCurrentCost.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[1]
		{
			new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "RActPrice", "{0:n0}")
		});
		this.gcCurrentCost.UnitField = null;
		this.gcCurrentCost.Visible = true;
		this.gcCurrentCost.VisibleIndex = 2;
		this.gcCurrentCost.Width = 150;
		this.repositoryItemCalcEdit3.AutoHeight = false;
		this.repositoryItemCalcEdit3.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEdit3.Name = "repositoryItemCalcEdit3";
		this.gcNewCost.Caption = "New Cost";
		this.gcNewCost.ColumnEdit = this.repositoryItemCalcEdit4;
		this.gcNewCost.CurrencyDateField = null;
		this.gcNewCost.CurrencyField = null;
		this.gcNewCost.CurrencyFixed = null;
		this.gcNewCost.CurrencyFixedRate = 0.0;
		this.gcNewCost.CurrencyRateField = null;
		this.gcNewCost.DisplayFormat.FormatString = "n0";
		this.gcNewCost.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gcNewCost.FieldName = "Price";
		this.gcNewCost.Name = "gcNewCost";
		this.gcNewCost.PrimaryKey = false;
		this.gcNewCost.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[1]
		{
			new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum)
		});
		this.gcNewCost.UnitField = null;
		this.gcNewCost.Visible = true;
		this.gcNewCost.VisibleIndex = 3;
		this.gcNewCost.Width = 150;
		this.repositoryItemCalcEdit4.AutoHeight = false;
		this.repositoryItemCalcEdit4.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEdit4.Name = "repositoryItemCalcEdit4";
		this.gcQuantity.Caption = "Quantity";
		this.gcQuantity.ColumnEdit = this.repositoryItemCalcEdit5;
		this.gcQuantity.CurrencyDateField = null;
		this.gcQuantity.CurrencyField = null;
		this.gcQuantity.CurrencyFixed = null;
		this.gcQuantity.CurrencyFixedRate = 0.0;
		this.gcQuantity.CurrencyRateField = null;
		this.gcQuantity.DisplayFormat.FormatString = "n0";
		this.gcQuantity.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gcQuantity.FieldName = "Quantity";
		this.gcQuantity.Name = "gcQuantity";
		this.gcQuantity.PrimaryKey = false;
		this.gcQuantity.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[1]
		{
			new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum)
		});
		this.gcQuantity.UnitField = null;
		this.gcQuantity.Visible = true;
		this.gcQuantity.VisibleIndex = 6;
		this.gcQuantity.Width = 150;
		this.repositoryItemCalcEdit5.AutoHeight = false;
		this.repositoryItemCalcEdit5.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEdit5.Name = "repositoryItemCalcEdit5";
		this.gcDebitCredit.Caption = "Debit/Credit";
		this.gcDebitCredit.ColumnEdit = this.repositoryItemCalcEdit6;
		this.gcDebitCredit.CurrencyDateField = null;
		this.gcDebitCredit.CurrencyField = null;
		this.gcDebitCredit.CurrencyFixed = null;
		this.gcDebitCredit.CurrencyFixedRate = 0.0;
		this.gcDebitCredit.CurrencyRateField = null;
		this.gcDebitCredit.Name = "gcDebitCredit";
		this.gcDebitCredit.PrimaryKey = false;
		this.gcDebitCredit.UnitField = null;
		this.gcDebitCredit.Visible = true;
		this.gcDebitCredit.VisibleIndex = 4;
		this.gcDebitCredit.Width = 150;
		this.repositoryItemCalcEdit6.AutoHeight = false;
		this.repositoryItemCalcEdit6.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEdit6.Name = "repositoryItemCalcEdit6";
		this.gcInStock.Caption = "InStock";
		this.gcInStock.ColumnEdit = this.repositoryItemCalcEdit7;
		this.gcInStock.CurrencyDateField = null;
		this.gcInStock.CurrencyField = null;
		this.gcInStock.CurrencyFixed = null;
		this.gcInStock.CurrencyFixedRate = 0.0;
		this.gcInStock.CurrencyRateField = null;
		this.gcInStock.DisplayFormat.FormatString = "n0";
		this.gcInStock.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gcInStock.FieldName = "ROnHand";
		this.gcInStock.Name = "gcInStock";
		this.gcInStock.PrimaryKey = false;
		this.gcInStock.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[1]
		{
			new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum)
		});
		this.gcInStock.UnitField = null;
		this.gcInStock.Visible = true;
		this.gcInStock.VisibleIndex = 5;
		this.gcInStock.Width = 150;
		this.repositoryItemCalcEdit7.AutoHeight = false;
		this.repositoryItemCalcEdit7.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEdit7.Name = "repositoryItemCalcEdit7";
		this.gcGLIncreaseAcc.Caption = "G/L Increase Acc";
		this.gcGLIncreaseAcc.CurrencyDateField = null;
		this.gcGLIncreaseAcc.CurrencyField = null;
		this.gcGLIncreaseAcc.CurrencyFixed = null;
		this.gcGLIncreaseAcc.CurrencyFixedRate = 0.0;
		this.gcGLIncreaseAcc.CurrencyRateField = null;
		this.gcGLIncreaseAcc.FieldName = "RIncmAcct";
		this.gcGLIncreaseAcc.Name = "gcGLIncreaseAcc";
		this.gcGLIncreaseAcc.PrimaryKey = false;
		this.gcGLIncreaseAcc.UnitField = null;
		this.gcGLIncreaseAcc.Visible = true;
		this.gcGLIncreaseAcc.VisibleIndex = 7;
		this.gcGLIncreaseAcc.Width = 150;
		this.gcGLDecreaseAcc.Caption = "G/L DecreaseAcc";
		this.gcGLDecreaseAcc.CurrencyDateField = null;
		this.gcGLDecreaseAcc.CurrencyField = null;
		this.gcGLDecreaseAcc.CurrencyFixed = null;
		this.gcGLDecreaseAcc.CurrencyFixedRate = 0.0;
		this.gcGLDecreaseAcc.CurrencyRateField = null;
		this.gcGLDecreaseAcc.FieldName = "RDcrmAcct";
		this.gcGLDecreaseAcc.Name = "gcGLDecreaseAcc";
		this.gcGLDecreaseAcc.PrimaryKey = false;
		this.gcGLDecreaseAcc.UnitField = null;
		this.gcGLDecreaseAcc.Visible = true;
		this.gcGLDecreaseAcc.VisibleIndex = 8;
		this.gcGLDecreaseAcc.Width = 150;
		this.gcWarehouse.Caption = "Warehouse";
		this.gcWarehouse.CurrencyDateField = null;
		this.gcWarehouse.CurrencyField = null;
		this.gcWarehouse.CurrencyFixed = null;
		this.gcWarehouse.CurrencyFixedRate = 0.0;
		this.gcWarehouse.CurrencyRateField = null;
		this.gcWarehouse.FieldName = "WhsCode";
		this.gcWarehouse.Name = "gcWarehouse";
		this.gcWarehouse.PrimaryKey = false;
		this.gcWarehouse.UnitField = null;
		this.gcWarehouse.Visible = true;
		this.gcWarehouse.VisibleIndex = 9;
		this.gcWarehouse.Width = 150;
		this.gcEvalSystem.Caption = "EvalSystem";
		this.gcEvalSystem.CurrencyDateField = null;
		this.gcEvalSystem.CurrencyField = null;
		this.gcEvalSystem.CurrencyFixed = null;
		this.gcEvalSystem.CurrencyFixedRate = 0.0;
		this.gcEvalSystem.CurrencyRateField = null;
		this.gcEvalSystem.FieldName = "EvalSystem";
		this.gcEvalSystem.Name = "gcEvalSystem";
		this.gcEvalSystem.PrimaryKey = false;
		this.gcEvalSystem.UnitField = null;
		this.gcEvalSystem.Visible = true;
		this.gcEvalSystem.VisibleIndex = 10;
		this.gcEvalSystem.Width = 150;
		this.gcDocEntry.Caption = "DocEntry";
		this.gcDocEntry.CurrencyDateField = null;
		this.gcDocEntry.CurrencyField = null;
		this.gcDocEntry.CurrencyFixed = null;
		this.gcDocEntry.CurrencyFixedRate = 0.0;
		this.gcDocEntry.CurrencyRateField = null;
		this.gcDocEntry.FieldName = "DocEntry";
		this.gcDocEntry.Name = "gcDocEntry";
		this.gcDocEntry.PrimaryKey = false;
		this.gcDocEntry.UnitField = null;
		this.gcDocEntry.Visible = true;
		this.gcDocEntry.VisibleIndex = 11;
		this.gcDocEntry.Width = 150;
		this.gcLineNum.Caption = "LineNum";
		this.gcLineNum.CurrencyDateField = null;
		this.gcLineNum.CurrencyField = null;
		this.gcLineNum.CurrencyFixed = null;
		this.gcLineNum.CurrencyFixedRate = 0.0;
		this.gcLineNum.CurrencyRateField = null;
		this.gcLineNum.FieldName = "LineNum";
		this.gcLineNum.Name = "gcLineNum";
		this.gcLineNum.PrimaryKey = false;
		this.gcLineNum.UnitField = null;
		this.gcLineNum.Visible = true;
		this.gcLineNum.VisibleIndex = 12;
		this.gcLineNum.Width = 150;
		this.groupControl1.Controls.Add(this.grdLine);
		this.groupControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.groupControl1.Location = new System.Drawing.Point(0, 0);
		this.groupControl1.Name = "groupControl1";
		this.groupControl1.Size = new System.Drawing.Size(783, 164);
		this.groupControl1.TabIndex = 0;
		this.groupControl1.Text = "FIFO Layer";
		this.grdLine.AllowPasteExcel = true;
		this.grdLine.ClipboardData = null;
		this.grdLine.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grdLine.Location = new System.Drawing.Point(2, 23);
		this.grdLine.MainView = this.gvLine;
		this.grdLine.Name = "grdLine";
		this.grdLine.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[5] { this.rEntryDate, this.repositoryItemCalcEdit1, this.repositoryItemCalcEdit2, this.repositoryItemCalcEdit8, this.repositoryItemCalcEdit9 });
		this.grdLine.Size = new System.Drawing.Size(779, 139);
		this.grdLine.TabIndex = 0;
		this.grdLine.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvLine });
		this.gvLine.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvLine.ColumnKey = null;
		this.gvLine.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[9] { this.gcDocNo, this.gcEntryDate, this.gc2CurrentCost, this.gc2Price, this.gc2Quantity, this.gc2DebitCredit, this.gcOpenQty, this.gc2DocEntry, this.gc2LineNum });
		this.gvLine.DisableFormSetting = false;
		this.gvLine.DisplayException = false;
		this.gvLine.EditColumn = null;
		this.gvLine.FieldCurrency = null;
		this.gvLine.FieldRate = null;
		this.gvLine.FormId = "oInventoryRevaluation";
		this.gvLine.GridControl = this.grdLine;
		this.gvLine.IgnoreFilter = false;
		this.gvLine.IndexEditRow = -1;
		this.gvLine.IndicatorWidth = 35;
		this.gvLine.IsApzTable = false;
		this.gvLine.IsInsertNewRow = false;
		this.gvLine.IsLoadingData = false;
		this.gvLine.IsMappingContextAction = false;
		this.gvLine.ItemId = "OLINE";
		this.gvLine.LstColumnAllowBatch = null;
		this.gvLine.LstColumnNotAllowBatch = null;
		this.gvLine.LstColumnNotEditable = null;
		this.gvLine.LstColumNotSetting = null;
		this.gvLine.Name = "gvLine";
		this.gvLine.OptionsView.ColumnAutoWidth = false;
		this.gvLine.OptionsView.ShowFooter = true;
		this.gvLine.OptionsView.ShowGroupPanel = false;
		this.gvLine.UseBatchMod = false;
		this.gvLine.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gvLine_CustomDrawRowIndicator);
		this.gcDocNo.Caption = "Doc. No";
		this.gcDocNo.CurrencyDateField = null;
		this.gcDocNo.CurrencyField = null;
		this.gcDocNo.CurrencyFixed = null;
		this.gcDocNo.CurrencyFixedRate = 0.0;
		this.gcDocNo.CurrencyRateField = null;
		this.gcDocNo.FieldName = "INMCreatBy";
		this.gcDocNo.Name = "gcDocNo";
		this.gcDocNo.PrimaryKey = true;
		this.gcDocNo.UnitField = null;
		this.gcDocNo.Visible = true;
		this.gcDocNo.VisibleIndex = 0;
		this.gcEntryDate.Caption = "Entry Date";
		this.gcEntryDate.ColumnEdit = this.rEntryDate;
		this.gcEntryDate.CurrencyDateField = null;
		this.gcEntryDate.CurrencyField = null;
		this.gcEntryDate.CurrencyFixed = null;
		this.gcEntryDate.CurrencyFixedRate = 0.0;
		this.gcEntryDate.CurrencyRateField = null;
		this.gcEntryDate.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.gcEntryDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.gcEntryDate.FieldName = "INMDocDate";
		this.gcEntryDate.Name = "gcEntryDate";
		this.gcEntryDate.PrimaryKey = false;
		this.gcEntryDate.UnitField = null;
		this.gcEntryDate.Visible = true;
		this.gcEntryDate.VisibleIndex = 1;
		this.rEntryDate.AutoHeight = false;
		this.rEntryDate.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rEntryDate.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rEntryDate.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.rEntryDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.rEntryDate.EditFormat.FormatString = "dd/MM/yyyy";
		this.rEntryDate.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.rEntryDate.Mask.EditMask = "dd/MM/yyyy";
		this.rEntryDate.Name = "rEntryDate";
		this.gc2CurrentCost.Caption = "Current Cost";
		this.gc2CurrentCost.ColumnEdit = this.repositoryItemCalcEdit1;
		this.gc2CurrentCost.CurrencyDateField = null;
		this.gc2CurrentCost.CurrencyField = null;
		this.gc2CurrentCost.CurrencyFixed = null;
		this.gc2CurrentCost.CurrencyFixedRate = 0.0;
		this.gc2CurrentCost.CurrencyRateField = null;
		this.gc2CurrentCost.DisplayFormat.FormatString = "n0";
		this.gc2CurrentCost.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gc2CurrentCost.FieldName = "RActPrice";
		this.gc2CurrentCost.Name = "gc2CurrentCost";
		this.gc2CurrentCost.PrimaryKey = false;
		this.gc2CurrentCost.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[1]
		{
			new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum)
		});
		this.gc2CurrentCost.UnitField = null;
		this.gc2CurrentCost.Visible = true;
		this.gc2CurrentCost.VisibleIndex = 2;
		this.repositoryItemCalcEdit1.AutoHeight = false;
		this.repositoryItemCalcEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEdit1.Name = "repositoryItemCalcEdit1";
		this.gc2Price.Caption = "New Price";
		this.gc2Price.ColumnEdit = this.repositoryItemCalcEdit2;
		this.gc2Price.CurrencyDateField = null;
		this.gc2Price.CurrencyField = null;
		this.gc2Price.CurrencyFixed = null;
		this.gc2Price.CurrencyFixedRate = 0.0;
		this.gc2Price.CurrencyRateField = null;
		this.gc2Price.DisplayFormat.FormatString = "n0";
		this.gc2Price.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gc2Price.FieldName = "Price";
		this.gc2Price.Name = "gc2Price";
		this.gc2Price.PrimaryKey = false;
		this.gc2Price.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[1]
		{
			new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum)
		});
		this.gc2Price.UnitField = null;
		this.gc2Price.Visible = true;
		this.gc2Price.VisibleIndex = 3;
		this.repositoryItemCalcEdit2.AutoHeight = false;
		this.repositoryItemCalcEdit2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEdit2.Name = "repositoryItemCalcEdit2";
		this.gc2Quantity.Caption = "Quantity";
		this.gc2Quantity.ColumnEdit = this.repositoryItemCalcEdit8;
		this.gc2Quantity.CurrencyDateField = null;
		this.gc2Quantity.CurrencyField = null;
		this.gc2Quantity.CurrencyFixed = null;
		this.gc2Quantity.CurrencyFixedRate = 0.0;
		this.gc2Quantity.CurrencyRateField = null;
		this.gc2Quantity.DisplayFormat.FormatString = "n0";
		this.gc2Quantity.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gc2Quantity.FieldName = "Quantity";
		this.gc2Quantity.Name = "gc2Quantity";
		this.gc2Quantity.PrimaryKey = false;
		this.gc2Quantity.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[1]
		{
			new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum)
		});
		this.gc2Quantity.UnitField = null;
		this.gc2Quantity.Visible = true;
		this.gc2Quantity.VisibleIndex = 4;
		this.repositoryItemCalcEdit8.AutoHeight = false;
		this.repositoryItemCalcEdit8.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEdit8.Name = "repositoryItemCalcEdit8";
		this.gc2DebitCredit.Caption = "Debit/Credit";
		this.gc2DebitCredit.ColumnEdit = this.repositoryItemCalcEdit9;
		this.gc2DebitCredit.CurrencyDateField = null;
		this.gc2DebitCredit.CurrencyField = null;
		this.gc2DebitCredit.CurrencyFixed = null;
		this.gc2DebitCredit.CurrencyFixedRate = 0.0;
		this.gc2DebitCredit.CurrencyRateField = null;
		this.gc2DebitCredit.DisplayFormat.FormatString = "n0";
		this.gc2DebitCredit.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gc2DebitCredit.Name = "gc2DebitCredit";
		this.gc2DebitCredit.PrimaryKey = false;
		this.gc2DebitCredit.UnitField = null;
		this.gc2DebitCredit.Visible = true;
		this.gc2DebitCredit.VisibleIndex = 5;
		this.repositoryItemCalcEdit9.AutoHeight = false;
		this.repositoryItemCalcEdit9.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEdit9.Name = "repositoryItemCalcEdit9";
		this.gcOpenQty.Caption = "Open Qty";
		this.gcOpenQty.CurrencyDateField = null;
		this.gcOpenQty.CurrencyField = null;
		this.gcOpenQty.CurrencyFixed = null;
		this.gcOpenQty.CurrencyFixedRate = 0.0;
		this.gcOpenQty.CurrencyRateField = null;
		this.gcOpenQty.DisplayFormat.FormatString = "n0";
		this.gcOpenQty.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gcOpenQty.FieldName = "INMOpenQty";
		this.gcOpenQty.Name = "gcOpenQty";
		this.gcOpenQty.PrimaryKey = false;
		this.gcOpenQty.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[1]
		{
			new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum)
		});
		this.gcOpenQty.UnitField = null;
		this.gcOpenQty.Visible = true;
		this.gcOpenQty.VisibleIndex = 6;
		this.gc2DocEntry.Caption = "DocEntry";
		this.gc2DocEntry.CurrencyDateField = null;
		this.gc2DocEntry.CurrencyField = null;
		this.gc2DocEntry.CurrencyFixed = null;
		this.gc2DocEntry.CurrencyFixedRate = 0.0;
		this.gc2DocEntry.CurrencyRateField = null;
		this.gc2DocEntry.FieldName = "DocEntry";
		this.gc2DocEntry.Name = "gc2DocEntry";
		this.gc2DocEntry.PrimaryKey = false;
		this.gc2DocEntry.UnitField = null;
		this.gc2DocEntry.Visible = true;
		this.gc2DocEntry.VisibleIndex = 7;
		this.gc2LineNum.Caption = "LineNum";
		this.gc2LineNum.CurrencyDateField = null;
		this.gc2LineNum.CurrencyField = null;
		this.gc2LineNum.CurrencyFixed = null;
		this.gc2LineNum.CurrencyFixedRate = 0.0;
		this.gc2LineNum.CurrencyRateField = null;
		this.gc2LineNum.FieldName = "LineNum";
		this.gc2LineNum.Name = "gc2LineNum";
		this.gc2LineNum.PrimaryKey = false;
		this.gc2LineNum.UnitField = null;
		this.gc2LineNum.Visible = true;
		this.gc2LineNum.VisibleIndex = 8;
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.chooseItemToolStripMenuItem, this.deleteRowToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(142, 48);
		this.chooseItemToolStripMenuItem.Name = "chooseItemToolStripMenuItem";
		this.chooseItemToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
		this.chooseItemToolStripMenuItem.Text = "Choose Item";
		this.chooseItemToolStripMenuItem.Click += new System.EventHandler(this.chooseItemToolStripMenuItem_Click);
		this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
		this.deleteRowToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
		this.deleteRowToolStripMenuItem.Text = "Delete Row";
		this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.deleteRowToolStripMenuItem_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(787, 514);
		base.Controls.Add(this.panelControl3);
		base.Controls.Add(this.panelControl2);
		base.Controls.Add(this.panelControl1);
		base.FormId = "oInventoryRevaluation";
		base.Name = "frm_InventoryRevaluation";
		this.Text = "Inventory Revaluation";
		base.Load += new System.EventHandler(this.frm_InventoryRevaluation_Load);
		((System.ComponentModel.ISupportInitialize)this.panelControl1).EndInit();
		this.panelControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.dtDocDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dtDocDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dtPostingDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dtPostingDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbType.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtDocEntry.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl2).EndInit();
		this.panelControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txtJrnlMemo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtComments.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl3).EndInit();
		this.panelControl3.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit6).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit7).EndInit();
		((System.ComponentModel.ISupportInitialize)this.groupControl1).EndInit();
		this.groupControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdLine).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gvLine).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rEntryDate.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rEntryDate).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit8).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit9).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
