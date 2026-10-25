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
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Controls.Tree;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;
using Apzon.Entities.Models.Production;
using Apzon.Entities.Models.UserFields;
using Apzon.Forms.Commons;
using Apzon.Forms.Helper;
using Apzon.Forms.Interfaces.ChooseFromList.RepositoryItem;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTab;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using SAPbobsCOM;

namespace Apzon.Forms.MTO_Manufacturing;

public class frm_ProductionForBatch : XtraFormEx
{
	private const string FormController = "ProductionForBatch";

	private const string TinhGiayController = "TinhGiay";

	private readonly DataTable _dtRequest = new DataTable();

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private List<UserFieldsImpl> lstUdf = new List<UserFieldsImpl>();

	public string UdoCode = "OPWB";

	public string UdoTableName = "@OPWB";

	public DataTable UdoTable = new DataTable();

	private const string ColId = "U_ID";

	private const string ColFather = "U_FatherId";

	private readonly Dictionary<string, TreeList> TblFormular = new Dictionary<string, TreeList>();

	private DataTable dtSelectedBatch;

	private bool userEdit = true;

	private IContainer components;

	private LookUpEdit look_Status;

	private LabelLine labelLine3;

	private TextEdit txt_Remark;

	private LabelLine labelLine2;

	private TextEdit txt_DocNo;

	private LabelLine labelLine1;

	private XtraTabControlEx tab_Main;

	private XtraTabPageEx xtraTabPage1;

	private SimpleButton btn_OK;

	private SplitContainerControl split_Main;

	private GroupControl groupControl1;

	private GridControlEx grd_ProductionRequest;

	private XtraTabPageEx tabPage_Planned;

	private SimpleButton btn_Release;

	private GridControlEx grd_Planned;

	private XtraTabPageEx tabPage_Released;

	private SimpleButton btn_Close;

	private GridControlEx grd_Released;

	private XtraTabPageEx tabPage_Holding;

	private GridControlEx grd_Holding;

	private XtraTabPageEx tabPage_Closed;

	private GridControlEx grd_Closed;

	private XtraTabPageEx tabPage_Canceled;

	private GridControlEx grd_Canceled;

	private XtraTabControlEx tabControl_Planning;

	private XtraTabPageEx xtraTabPage2;

	private XtraTabPageEx tab_Planning;

	private SplitContainerControl splitContainerControl1;

	private TreeEx tv_ProductionPlanning;

	private GridControlEx grd_Operation;

	private PanelControl panelControl1;

	private SimpleButton btn_TinhGiay;

	private PanelControl panelControl2;

	private SimpleButton btn_ChooseBatch;

	private GridViewEx gv_ProductionRequest;

	private GridViewEx gv_Planned;

	private GridViewEx gv_Released;

	private GridViewEx gv_Holding;

	private GridViewEx gv_Closed;

	private GridViewEx gv_Canceled;

	private GridViewEx gv_Operation;

	private ContextMenuStrip ctx_Request;

	private ToolStripMenuItem removeRowsToolStripMenuItem;

	private SimpleButton btn_GenerateProductionOrder;

	private ContextMenuStrip ctx_Planning;

	private ToolStripMenuItem removeRowsToolStripMenuItem1;

	private ToolStripSeparator toolStripSeparator1;

	private ToolStripMenuItem copyOperationToolStripMenuItem;

	private ToolStripMenuItem pasteOperationToolStripMenuItem;

	private RepCflWarehouse rep_PlanWhs;

	private RepCflWarehouse rep_Whs;

	private TreeEx tre_DSCuon;

	private ContextMenuStrip ctm_Operation;

	private ToolStripMenuItem removeRowToolStripMenuItem;

	private ContextMenuStrip ctx_Main;

	private ToolStripMenuItem menuMainClose;

	private ToolStripMenuItem menuMainCancel;

	private TextEdit txt_StockRequest;

	private LabelLine labelLine4;

	private SimpleButton btn_AutoCalc;

	private SimpleButton btn_AutoXa;

	private ToolStripMenuItem menu_plan_CancelPO;

	private TextEdit txt_Creator;

	private LabelLine labelLine5;

	private ToolStripMenuItem menu_plan_UnReleasePO;

	private CheckedComboBoxEdit chb_StatusQC;

	private LabelLine lb_BusinessPartner;

	private TextEdit txt_Pono;

	private LabelLine labelLine6;

	private TextEdit txt_BatchNo;

	private LabelLine labelLine7;

	private TextEdit txt_SoNo;

	private LabelLine labelLine8;

	private SimpleButton btn_ConfirmProduction;

	public DataTable DtCopyOperation { get; set; }

	public int PlanId { get; set; }

	public frm_ProductionForBatch(DataTable dtrequest = null)
	{
		InitializeComponent();
		if (dtrequest.IsNotNull())
		{
			_dtRequest = dtrequest;
		}
		try
		{
			rep_Whs.GridView = gv_Operation;
			rep_Whs.FieldName = "U_WhsCode";
			rep_PlanWhs.TreeList = tv_ProductionPlanning;
			rep_PlanWhs.FieldName = "U_WhsCode";
			FormId = "oProductionForBatch";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	public frm_ProductionForBatch()
	{
		InitializeComponent();
		rep_Whs.GridView = gv_Operation;
		rep_Whs.FieldName = "U_WhsCode";
		rep_PlanWhs.TreeList = tv_ProductionPlanning;
		rep_PlanWhs.FieldName = "U_WhsCode";
		FormId = "oProductionForBatch";
	}

	private async void frm_ProductionForBatch_Load(object sender, EventArgs e)
	{
		_ = 9;
		try
		{
			await LoadBatchStatusQC();
			await LoadDocStatus();
			UdoTable = await _httpClient.Udo.GetUdoByCode(new Common.Table_Code
			{
				Name = "OUDO",
				Code = "OPWB"
			});
			DataSet dataSet = await GetProductionRequest();
			if (_dtRequest.IsNotNull())
			{
				if (dataSet.IsNotNull())
				{
					await GridViewHelper.GenerateGrid(grd_ProductionRequest, gv_ProductionRequest, dataSet.Tables[0], _dtRequest, null, null, expand: true);
				}
				else
				{
					grd_ProductionRequest.DataSource = _dtRequest;
					grd_ProductionRequest.RefreshDataSource();
					gv_ProductionRequest.PopulateColumns();
				}
				gv_ProductionRequest.ExpandAllGroups();
				gv_ProductionRequest.SelectRow(0);
				string text = Function.ToString(_dtRequest.Rows[0]["StatusQC"]);
				if (!string.IsNullOrEmpty(text))
				{
					chb_StatusQC.SetEditValue(text);
				}
				await LoadTonKho();
				btn_OK.Text = Resources.ButtonAdd;
				btn_OK.Enabled = false;
				btn_AutoCalc.Enabled = false;
				btn_AutoXa.Enabled = false;
				FormMode = Enums.FormViewModeType.View;
			}
			List<Formular> formulars = await WinFuntions.GetFormulas(UdoCode);
			TblFormular["@PWB2"] = tv_ProductionPlanning;
			tv_ProductionPlanning.TblFormular = TblFormular;
			tv_ProductionPlanning.Formulars = formulars;
			DataSet ds = await GetPlanning("", 0);
			if (ds.IsHasValidValue())
			{
				TreeListHelper.fixedColor = false;
				await TreeListHelper.GenerateTreeList(tv_ProductionPlanning, ds, new List<UserFieldsImpl>(), expand: true);
				TreeListHelper.fixedColor = true;
			}
			tv_ProductionPlanning.FatherColumn = "U_FatherId";
			if (PlanId > 0)
			{
				await Navigate(PlanId, 5, GlobalData.UserName);
			}
			gv_Operation.OptionsView.NewItemRowPosition = NewItemRowPosition.Bottom;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void tv_ProductionPlanning_FocusedNodeChanged(object sender, FocusedNodeChangedEventArgs e)
	{
		_ = 1;
		try
		{
			object dataRecordByNode = tv_ProductionPlanning.GetDataRecordByNode(tv_ProductionPlanning.FocusedNode);
			if (dataRecordByNode == null)
			{
				return;
			}
			DataRowView planning = (DataRowView)dataRecordByNode;
			if (planning == null || planning.Row == null)
			{
				return;
			}
			DataTable dataSourceTable = grd_Operation.DataSourceTable;
			int id = Function.ParseInt(planning.Row["U_ID"]);
			gv_Operation.ActiveFilterString = $"U_PlanId='{id}'";
			gv_Operation.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
			if (dataSourceTable.IsNotNull())
			{
				if (dataSourceTable.Select($"U_PlanId='{id}'").Length == 0)
				{
					DataRow dataRow = dataSourceTable.NewRow();
					dataRow["U_ItemCode"] = planning.Row["U_ItemCode"];
					dataRow["U_Width"] = planning.Row["U_Width"];
					dataRow["U_Length"] = planning.Row["U_Length"];
					dataRow["U_UnitUoM"] = planning.Row["U_UnitUoM"];
					dataRow["U_QtyUoM"] = planning.Row["U_QtyUoM"];
					dataRow["U_Quantity"] = planning.Row["U_Quantity"];
					dataRow["U_PlanId"] = id;
					dataRow["U_WhsCode"] = GlobalData.UserDefaultGroup.Warehouse;
					dataRow["U_DelStatus"] = "N";
					dataRow["U_StatusQC"] = "OK";
					dataSourceTable.Rows.Add(dataRow);
				}
				return;
			}
			DataSet dataSet = await GetOperation(0, -1);
			if (dataSet.IsHasStructure())
			{
				DataTable dataTable = dataSet.Tables[1].Clone();
				DataRow dataRow2 = dataTable.NewRow();
				dataRow2["U_ItemCode"] = planning.Row["U_ItemCode"];
				dataRow2["U_Width"] = planning.Row["U_Width"];
				dataRow2["U_Length"] = planning.Row["U_Length"];
				dataRow2["U_UnitUoM"] = planning.Row["U_UnitUoM"];
				dataRow2["U_QtyUoM"] = planning.Row["U_QtyUoM"];
				dataRow2["U_Quantity"] = planning.Row["U_Quantity"];
				dataRow2["U_WhsCode"] = GlobalData.UserDefaultGroup.Warehouse;
				dataRow2["U_DelStatus"] = "N";
				dataRow2["U_StatusQC"] = "OK";
				dataRow2["U_PlanId"] = id;
				dataTable.Rows.Add(dataRow2);
				await GridViewHelper.GenerateGrid(grd_Operation, gv_Operation, dataSet.Tables[0], dataTable, dataSet.Tables[2], new List<UserFieldsImpl>(), expand: true);
				gv_Operation.OptionsView.NewItemRowPosition = NewItemRowPosition.Bottom;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<DataSet> GetProductionRequest()
	{
		try
		{
			int id = Function.ParseInt(txt_DocNo.EditValue);
			using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "ProductionForBatch");
			return await helper.GetAsync("GetProductionRequest", new { id });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataSet();
	}

	private async Task LoadDocStatus()
	{
		try
		{
			RepositoryItemLookUpEdit properties = look_Status.Properties;
			properties.DataSource = await _httpClient.Udo.GetStatus();
			look_Status.Properties.DisplayMember = "Name";
			look_Status.Properties.ValueMember = "Code";
			look_Status.ReadOnly = true;
			look_Status.EditValue = "O";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadBatchStatusQC()
	{
		try
		{
			DataTable dataTable = new DataTable();
			using (HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ProductionForBatch"))
			{
				dataTable = await helper.GetAsync("LoadBatchStatusQC");
			}
			if (dataTable.IsNotNull())
			{
				chb_StatusQC.Properties.DataSource = dataTable;
				chb_StatusQC.Properties.DisplayMember = "Display";
				chb_StatusQC.Properties.ValueMember = "Value";
			}
			else
			{
				chb_StatusQC.Properties.Items.Clear();
				chb_StatusQC.Properties.Items.Add("OK", "Hàng tốt");
				chb_StatusQC.Properties.Items.Add("NG", "Hàng hỏng");
				chb_StatusQC.Properties.Items.Add("NA", "Chờ xác nhận lỗi");
				chb_StatusQC.Properties.Items.Add("HG", "Hàng khách gửi");
				chb_StatusQC.Properties.Items.Add("CT", "Hàng khách gửi chờ trả khách");
				chb_StatusQC.Properties.Items.Add("HD", "Hàng hợp đồng");
				chb_StatusQC.Properties.Items.Add("CL", "Chờ Claim nhà cung cấp");
				chb_StatusQC.Properties.Items.Add("ST", "Sai TEM");
			}
			chb_StatusQC.SetEditValue("OK,NG,NA");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<DataSet> GetPlanning(string batch, int docEntry)
	{
		try
		{
			using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "ProductionForBatch");
			return await helper.GetAsync("GetPlanning", new { batch, docEntry });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataSet();
	}

	private async Task<string> ChooseBatch(List<DataRow> rows)
	{
		_ = 1;
		try
		{
			WinCommons._OpenProcessing();
			if (rows != null && rows.Count > 0 && tv_ProductionPlanning.DataSource != null)
			{
				if (tv_ProductionPlanning.DataSource == null)
				{
					DataSet ds = await GetPlanning("", 0);
					if (ds.IsHasValidValue())
					{
						TreeListHelper.fixedColor = false;
						await TreeListHelper.GenerateTreeList(tv_ProductionPlanning, ds, null, expand: true);
						TreeListHelper.fixedColor = true;
					}
				}
				DataTable dataTable = tv_ProductionPlanning.DataSourceTable();
				foreach (DataRow row in rows)
				{
					if (dataTable.Select(string.Format("U_BatchNo='{0}'", Function.ToString(row["U_BatchNo"]))).Length != 0)
					{
						WinCommons.Warning(Enums.MessageCode.None, string.Format("\"Batch No.\" {0} Đã tồn tại trên lưới dữ liệu", Function.ToString(row["U_BatchNo"])));
						continue;
					}
					int num = ((!dataTable.AsEnumerable().Any()) ? 1 : (dataTable.AsEnumerable().Max((DataRow x) => Function.ParseInt(x["U_ID"])) + 1));
					DataRow dataRow = dataTable.NewRow();
					foreach (DataColumn item in row.Table.Columns.Cast<DataColumn>())
					{
						if (dataRow.Table.Columns.Contains(item.ColumnName))
						{
							dataRow[item.ColumnName] = row[item.ColumnName];
						}
					}
					dataRow["U_ID"] = num;
					dataRow["U_FatherId"] = -1;
					dataRow["U_WhsCode"] = GlobalData.UserDefaultGroup.Warehouse;
					dataTable.Rows.Add(dataRow);
				}
				dataTable.AcceptChanges();
				int[] selectedRows = gv_ProductionRequest.GetSelectedRows();
				string text = string.Join(",", rows.Select((DataRow t) => Function.ToString(t["U_BatchNo"])).ToArray());
				for (int num2 = 0; num2 < selectedRows.Length; num2++)
				{
					string text2 = Function.ToString(gv_ProductionRequest.GetRowCellValue(num2, "U_BatchNo"));
					gv_ProductionRequest.SetRowCellValue(num2, "U_BatchNo", string.Join(";", text, text2));
				}
				tabControl_Planning.SelectedTabPage = tab_Planning;
			}
			WinCommons._CloseProcessing();
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return ex.Message;
		}
	}

	private async Task<DataSet> GetOperation(int docEntry, int fatherId)
	{
		try
		{
			using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "ProductionForBatch");
			return await helper.GetAsync("GetOperation", new { docEntry, fatherId });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataSet();
	}

	public async Task MOVE_NEXT()
	{
		try
		{
			int type = 3;
			int currentId = Function.ParseInt(txt_DocNo.EditValue);
			await Navigate(currentId, type, GlobalData.UserName);
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
			await Navigate(currentId, type, GlobalData.UserName);
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
			await Navigate(currentId, type, GlobalData.UserName);
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
			await Navigate(currentId, type, GlobalData.UserName);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task MODE_FIND()
	{
		try
		{
			txt_DocNo.ReadOnly = false;
			FormMode = Enums.FormViewModeType.Find;
			btn_OK.Text = Resources.ButtonFind;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			int currentId = Function.ParseInt(txt_DocNo.EditValue);
			await Navigate(currentId, 5, GlobalData.UserName);
			FormMode = Enums.FormViewModeType.View;
			btn_OK.Text = Resources.ButtonOK;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task Navigate(int currentId, int type, string username)
	{
		WinCommons._OpenProcessing();
		try
		{
			int num = ((type != 5) ? (await GetPage(currentId, type, username, GlobalData.DefaultBranch)) : currentId);
			int id = num;
			DataTable dataTable = await _httpClient.Udo.GetByDocEntry(new Common.Table_Code
			{
				Code = id.ToString(),
				Name = UdoTableName
			});
			if (dataTable.IsNotNull())
			{
				txt_DocNo.EditValue = Function.ToString(dataTable.Rows[0]["DocEntry"]);
				if ("C".Equals(Function.ToString(dataTable.Rows[0]["Status"])) && "N".Equals(Function.ToString(dataTable.Rows[0]["Canceled"])))
				{
					look_Status.EditValue = "L";
				}
				else
				{
					look_Status.EditValue = Function.ToString(dataTable.Rows[0]["Status"]);
				}
				txt_Remark.Text = Function.ToString(dataTable.Rows[0]["Remark"]);
				txt_Creator.Text = (dataTable.Rows[0].IsExistsColumn("Creator") ? Function.ToString(dataTable.Rows[0]["Creator"]) : "");
				txt_StockRequest.Text = (dataTable.Rows[0].IsExistsColumn("U_TranId") ? Function.ToString(dataTable.Rows[0]["U_TranId"]) : "");
				DataSet ds = await GetProductionRequest();
				if (ds.IsHasStructure())
				{
					if (grd_ProductionRequest.DataSourceTable.Rows.Count <= 0)
					{
						await GridViewHelper.GenerateGrid(grd_ProductionRequest, gv_ProductionRequest, ds.Tables[0], ds.Tables[1], null, null, expand: true);
					}
					else
					{
						grd_ProductionRequest.DataSource = ds.Tables[1];
						grd_ProductionRequest.RefreshDataSource();
						gv_ProductionRequest.ExpandAllGroups();
					}
				}
				DataSet dsOperation = await GetOperation(id, id);
				if (dsOperation.IsHasValidValue())
				{
					await GridViewHelper.GenerateGrid(grd_Operation, gv_Operation, dsOperation, new List<UserFieldsImpl>(), expand: true);
				}
				DataSet dsplan = await GetPlanning("", id);
				if (dsplan.IsHasValidValue())
				{
					await TreeListHelper.GenerateTreeList(tv_ProductionPlanning, dsplan, new List<UserFieldsImpl>(), expand: true);
					tv_ProductionPlanning.Columns["U_WhsCode"].ColumnEdit = rep_PlanWhs;
					btn_GenerateProductionOrder.Enabled = true;
				}
				ds.Tables.Clear();
				ds.Dispose();
				dsOperation.Tables.Clear();
				dsOperation.Dispose();
				dsplan.Tables.Clear();
				dsplan.Dispose();
				tv_ProductionPlanning.ExpandAll();
				txt_Pono.Text = "";
				txt_BatchNo.Text = "";
				txt_SoNo.Text = "";
				FormMode = Enums.FormViewModeType.Update;
				tre_DSCuon.DataSourceTable().Rows.Clear();
				tabControl_Planning.SelectedTabPage = tab_Planning;
				gv_Operation.OptionsView.NewItemRowPosition = NewItemRowPosition.Bottom;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private async Task<int> GetPage(int current, int type, string username, int branch)
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ProductionForBatch");
			return await helper.GetIntAsync("GetPage", new { current, type, username, branch });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return 0;
	}

	private void FormValueChanged()
	{
		try
		{
			btn_GenerateProductionOrder.Enabled = true;
			btn_ConfirmProduction.Enabled = true;
			btn_OK.Enabled = true;
			btn_AutoCalc.Enabled = true;
			btn_AutoXa.Enabled = true;
			txt_DocNo.ReadOnly = true;
			if (FormMode == Enums.FormViewModeType.Update)
			{
				btn_OK.Text = Resources.ButtonUpdate;
			}
			else
			{
				btn_OK.Text = Resources.ButtonAdd;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
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

	private async void btn_ChooseBatch_Click(object sender, EventArgs e)
	{
		try
		{
			if (gv_ProductionRequest.GetSelectedRows().Length == 0)
			{
				XtraMessageBox.Show("Select request row");
				return;
			}
			new List<DataRow>();
			DataTable dt = tre_DSCuon.DataSourceTable();
			DataRow[] array = dt.Select("Checked = 'True' AND (Status = 'NG' OR Status = 'ST' OR Status = 'P2')");
			if (array != null && array.Length != 0 && XtraMessageBox.Show("Chọn vào hàng lỗi. Bạn có muốn tiếp tục?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
			{
				return;
			}
			List<DataRow> rows = dt.Select("Checked = 'True'").ToList();
			if (dtSelectedBatch.IsNotNull())
			{
				foreach (DataRow item in rows)
				{
					DataRow dataRow = dtSelectedBatch.NewRow();
					WinFuntions.DataRowToDataRow(item, dataRow);
					dtSelectedBatch.Rows.Add(dataRow);
				}
			}
			else
			{
				dtSelectedBatch = rows.CopyToDataTable();
			}
			int num = 0;
			int index = 0;
			List<DataRow> list = new List<DataRow>();
			for (int i = 0; i < rows.Count; i++)
			{
				int num2 = Function.ParseInt(rows[i]["ParentID"]);
				if (num2 == num && num > 0 && i > 0)
				{
					int num3 = Function.ParseInt(rows[index]["U_QtyUoM"]);
					int num4 = Function.ParseInt(rows[i]["U_QtyUoM"]);
					double num5 = Function.ParseDouble(rows[index]["U_Quantity"]);
					double num6 = Function.ParseDouble(rows[i]["U_Quantity"]);
					rows[index]["U_QtyUoM"] = num3 + num4;
					rows[index]["U_Quantity"] = Math.Round(num5 + num6, 3, MidpointRounding.AwayFromZero);
					DataRow dataRow2 = rows[index];
					dataRow2["U_BatchNo"] = dataRow2["U_BatchNo"]?.ToString() + ";" + Function.ToString(rows[i]["U_BatchNo"]);
					list.Add(rows[i]);
				}
				else
				{
					index = i;
					num = num2;
				}
			}
			foreach (DataRow item2 in list)
			{
				rows.Remove(item2);
				dt.Rows.Remove(item2);
			}
			if (!string.IsNullOrEmpty(await ChooseBatch(rows)))
			{
				return;
			}
			list = new List<DataRow>();
			for (int j = 0; j < rows.Count; j++)
			{
				list.Add(rows[j]);
			}
			foreach (DataRow item3 in list)
			{
				dt.Rows.Remove(item3);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_TinhGiay_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (tre_DSCuon.DataSource == null || !tre_DSCuon.DataSourceTable().IsNotNull())
			{
				await LoadTonKho();
			}
			else
			{
				await LoadTonKho();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadTonKho()
	{
		WinCommons._OpenProcessing();
		string text = "";
		for (int i = 0; i < chb_StatusQC.Properties.Items.Count; i++)
		{
			if (chb_StatusQC.Properties.Items[i].CheckState == CheckState.Checked)
			{
				text = text + "," + chb_StatusQC.Properties.Items[i].Value;
			}
		}
		text = ((!string.IsNullOrEmpty(text) && text.StartsWith(",")) ? text.Substring(1, text.Length - 1) : "");
		DataTable dataTable = CreateTableTinhGiay();
		DataTable dataSourceTable = grd_ProductionRequest.DataSourceTable;
		for (int j = 0; j < dataSourceTable.Rows.Count; j++)
		{
			DataRow dataRow = dataTable.NewRow();
			dataRow["ID"] = j;
			dataRow["ItemCode"] = (dataSourceTable.Rows[j].IsExistsColumn("U_ItemCode") ? dataSourceTable.Rows[j]["U_ItemCode"] : "");
			dataRow["ItemName"] = (dataSourceTable.Rows[j].IsExistsColumn("U_ItemName") ? dataSourceTable.Rows[j]["U_ItemName"] : "");
			dataRow["Batch"] = (dataSourceTable.Rows[j].IsExistsColumn("Batch") ? dataSourceTable.Rows[j]["Batch"] : "");
			dataRow["Width"] = (dataSourceTable.Rows[j].IsExistsColumn("U_SONo") ? dataSourceTable.Rows[j]["U_SONo"] : ((object)0));
			dataRow["Length"] = (dataSourceTable.Rows[j].IsExistsColumn("U_SoLine") ? dataSourceTable.Rows[j]["U_SoLine"] : ((object)0));
			dataRow["Qty"] = (dataSourceTable.Rows[j].IsExistsColumn("U_Quantity") ? dataSourceTable.Rows[j]["U_Quantity"] : ((object)0));
			dataRow["QtyPcs"] = (dataSourceTable.Rows[j].IsExistsColumn("QtyPcs") ? dataSourceTable.Rows[j]["QtyPcs"] : ((object)0));
			dataRow["Uom"] = (dataSourceTable.Rows[j].IsExistsColumn("U_UnitUoM") ? dataSourceTable.Rows[j]["U_UnitUoM"] : "");
			dataRow["XoayKho"] = (dataSourceTable.Rows[j].IsExistsColumn("XoayKho") ? dataSourceTable.Rows[j]["XoayKho"] : ((object)false));
			dataRow["H1"] = (dataSourceTable.Rows[j].IsExistsColumn("H1") ? dataSourceTable.Rows[j]["H1"] : ((object)false));
			dataRow["H2"] = (dataSourceTable.Rows[j].IsExistsColumn("H2") ? dataSourceTable.Rows[j]["H2"] : ((object)false));
			dataRow["Hxa"] = (dataSourceTable.Rows[j].IsExistsColumn("Hxa") ? dataSourceTable.Rows[j]["Hxa"] : ((object)false));
			dataRow["GhepKho"] = (dataSourceTable.Rows[j].IsExistsColumn("GhepKho") ? dataSourceTable.Rows[j]["GhepKho"] : ((object)false));
			dataRow["KhongGhep"] = false;
			if (!dataSourceTable.Rows[j].IsExistsColumn("Joins") || string.IsNullOrEmpty(Function.ToString(dataSourceTable.Rows[j]["Joins"])))
			{
				dataRow["Joins"] = ((Function.ParseInt(dataSourceTable.Rows[j]["U_SONo"]) == 0) ? 1 : 0);
			}
			else
			{
				dataRow["Joins"] = dataSourceTable.Rows[j]["Joins"];
			}
			dataTable.Rows.Add(dataRow);
		}
		DataSet ds = new DataSet();
		ds.Tables.Clear();
		ds = await XemTonKho(dataTable, GlobalData.DefaultBranch.ToString(), text);
		if (ds.IsHasStructure())
		{
			List<UserFieldsImpl> lstUDF = await LoadUDF("QUT1");
			await TreeListHelper.GenerateTreeList(tre_DSCuon, ds, lstUDF, expand: false, filterRow: true);
		}
		else
		{
			tre_DSCuon.DataSource = new DataTable();
			tre_DSCuon.RefreshDataSource();
		}
		WinCommons._CloseProcessing();
	}

	private void tre_DSCuon_CellValueChanged(object sender, DevExpress.XtraTreeList.CellValueChangedEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			if (tre_DSCuon.FocusedColumn.FieldName.Equals("ChooseQty"))
			{
				DataTable dataTable = tre_DSCuon.DataSourceTable();
				DataRow row = ((DataRowView)tre_DSCuon.GetDataRecordByNode(e.Node)).Row;
				int num = Function.ParseInt(row["ChooseQty"]);
				int num2 = Function.ParseInt(row["TotalQty"]);
				int num3 = Function.ParseInt(row["U_ID"]);
				if (num > num2)
				{
					row["ChooseQty"] = 0;
					XtraMessageBox.Show("Số lượng chọn vượt quá số lượng tồn kho!", "System", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
					return;
				}
				DataRow[] array = dataTable.Select("ParentID = " + num3);
				if (array != null && array.Length != 0)
				{
					for (int i = 0; i < array.Length; i++)
					{
						if (num > 0)
						{
							int num4 = Function.ParseInt(array[i]["U_QtyUoM"]);
							array[i]["Checked"] = true;
							num -= num4;
						}
						else
						{
							array[i]["Checked"] = false;
						}
					}
				}
				e.Node.Expanded = true;
			}
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private DataTable CreateTableTinhGiay()
	{
		DataTable dataTable = new DataTable();
		try
		{
			dataTable.Columns.Add("ID", typeof(int));
			dataTable.Columns.Add("ItemCode", typeof(string));
			dataTable.Columns.Add("ItemName", typeof(string));
			dataTable.Columns.Add("Batch", typeof(string));
			dataTable.Columns.Add("Qty", typeof(double));
			dataTable.Columns.Add("Width", typeof(double));
			dataTable.Columns.Add("Length", typeof(double));
			dataTable.Columns.Add("QtyPcs", typeof(double));
			dataTable.Columns.Add("Uom", typeof(string));
			dataTable.Columns.Add("XoayKho", typeof(bool));
			dataTable.Columns.Add("H1", typeof(bool));
			dataTable.Columns.Add("H2", typeof(bool));
			dataTable.Columns.Add("Joins", typeof(double));
			dataTable.Columns.Add("KhongGhep", typeof(bool));
			dataTable.Columns.Add("Hxa", typeof(bool));
			dataTable.Columns.Add("GhepKho", typeof(bool));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return dataTable;
	}

	private async Task<DataSet> XemTonKho(DataTable dtGhepKho, string branch, string statusQC)
	{
		try
		{
			using HttpClientHelper<DataSet> _helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "TinhGiay");
			return await _helper.PostAsync("XemTonKho", new Tuple<DataTable, string, string>(dtGhepKho, branch, statusQC));
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataSet();
	}

	private async Task<List<UserFieldsImpl>> LoadUDF(string tableName)
	{
		List<UserFieldsImpl> lst = new List<UserFieldsImpl>();
		try
		{
			using HttpClientHelper<UserFieldsImpl> helper = new HttpClientHelper<UserFieldsImpl>(Configuration.ApiServer, "UDF");
			lst = await helper.GetListAsyncByPr("GetListField", new { tableName });
			if (lst != null && lst.Count > 0)
			{
				return lst;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return lst;
	}

	private async void btn_OK_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (btn_OK.Text.Equals(Resources.ButtonOK))
			{
				Close();
			}
			else if (btn_OK.Text.Equals(Resources.ButtonFind))
			{
				int currentId = Function.ParseInt(txt_DocNo.EditValue);
				await Navigate(currentId, 5, GlobalData.UserName);
			}
			else if (btn_OK.Text.Equals(Resources.ButtonAdd) || btn_OK.Text.Equals(Resources.ButtonUpdate))
			{
				DataTable dataTable = tv_ProductionPlanning.DataSourceTable();
				if (!dataTable.IsNotNull())
				{
					XtraMessageBox.Show("Chưa chọn hàng để sản xuất!", "System", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
					return;
				}
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					double num = Math.Round(Function.ParseDouble(dataTable.Rows[i]["U_Quantity"]), 3, MidpointRounding.AwayFromZero);
					double num2 = Math.Round(Function.ParseDouble(dataTable.Rows[i]["U_LossQty"]), 3, MidpointRounding.AwayFromZero);
					if (((num2 > 0.0 && num2 > num * 0.1) || (num2 < 0.0 && 0.0 - num2 > num * 0.1)) && XtraMessageBox.Show("Hao hụt quá nhiều. Bạn có muốn tiếp tục lưu?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
					{
						return;
					}
				}
				DataTable dataSourceTable = grd_Operation.DataSourceTable;
				if (dataSourceTable.Select("U_DelStatus = 'Y' AND (U_SONo IS NULL OR U_SONo = 0)").IsNotNull())
				{
					XtraMessageBox.Show("Chưa nhập SoNo cho hàng giao khách!", "System", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
					return;
				}
				List<DataRow> list = (from t in grd_ProductionRequest.DataSourceTable.AsEnumerable()
					where Function.ParseInt(t["U_SoNo"]) > 0
					select t).ToList();
				List<DataRow> list2 = (from t in dataTable.AsEnumerable()
					where "OP_GN".Equals(Function.ToString(t["U_OpCode"]))
					select t).ToList();
				if (!dataSourceTable.Select("U_SONo > 0").IsNotNull() && list2.Count <= 0 && list.Count > 0)
				{
					XtraMessageBox.Show("Chưa nhập SoNo cho phiếu tính giấy!", "System", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
					return;
				}
				if (dataTable.Select("U_OpCode = 'OP_GN' AND (U_SONo IS NULL OR U_SONo = 0)").Length != 0)
				{
					XtraMessageBox.Show("Chưa nhập SoNo cho phiếu giao nguyên!", "System", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
					return;
				}
				if (dataTable.Select("U_OpCode = 'OP_GN' AND U_FromWhsCode = U_WhsCode").Length != 0)
				{
					XtraMessageBox.Show("Chọn kho SX chưa đúng!", "System", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
					return;
				}
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
				{
					return;
				}
				btn_OK.Enabled = false;
				btn_GenerateProductionOrder.Enabled = false;
				WinCommons._OpenProcessing();
				await SaveUdo();
				WinCommons._CloseProcessing();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		btn_OK.Enabled = true;
		btn_GenerateProductionOrder.Enabled = true;
	}

	private async Task SaveUdo()
	{
		try
		{
			if (UdoTable == null || UdoTable.Rows.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.UdoTableIsInvalid, "");
				return;
			}
			string tableName = Function.ToString(UdoTable.Rows[0]["TableName"]);
			DataSet ds = new DataSet
			{
				Tables = { UdoTable.Copy() }
			};
			DataTable dataTable = new DataTable(tableName);
			dataTable.Columns.Add("DocEntry", typeof(int));
			dataTable.Columns.Add("Remark", typeof(string));
			dataTable.Columns.Add("U_TranId", typeof(int));
			dataTable.Columns.Add("U_UserSign2", typeof(string));
			DataRow dataRow = dataTable.NewRow();
			dataRow["DocEntry"] = ((FormMode != Enums.FormViewModeType.Add) ? Function.ParseInt(txt_DocNo.EditValue) : 0);
			dataRow["Remark"] = Function.ToString(txt_Remark.EditValue);
			dataRow["U_TranId"] = Function.ParseInt(txt_StockRequest.EditValue);
			dataTable.Rows.Add(dataRow);
			ds.Tables.Add(dataTable);
			DataTable dataSourceTable = grd_ProductionRequest.DataSourceTable;
			dataSourceTable.TableName = "PWB1";
			DataTable dataTable2 = dataSourceTable.Copy();
			if (!dataTable2.IsExistsColumn("DocEntry"))
			{
				dataTable2.Columns.Add("DocEntry", typeof(int));
			}
			if (!dataTable2.IsExistsColumn("LineNum"))
			{
				dataTable2.Columns.Add("LineNum", typeof(int));
			}
			foreach (string item in (from DataColumn t in dataTable2.Columns
				where !t.ColumnName.Equals("DocEntry") && !t.ColumnName.Equals("LineNum") && !t.ColumnName.StartsWith("U_")
				select t.ColumnName).ToList())
			{
				dataTable2.Columns.Remove(item);
			}
			ds.Tables.Add(dataTable2.Copy());
			DataTable dataTable3 = tv_ProductionPlanning.DataSourceTable();
			DataTable dataTable4 = dataTable3.Copy();
			dataTable4.TableName = "PWB2";
			if (!dataTable4.ExistsColumn("DocEntry"))
			{
				dataTable4.Columns.Add("DocEntry", typeof(int));
			}
			if (!dataTable4.ExistsColumn("LineNum"))
			{
				dataTable4.Columns.Add("LineNum", typeof(int));
			}
			foreach (string item2 in (from DataColumn t in dataTable4.Columns
				where !t.ColumnName.Equals("DocEntry") && !t.ColumnName.Equals("LineNum") && !t.ColumnName.StartsWith("U_")
				select t.ColumnName).ToList())
			{
				dataTable4.Columns.Remove(item2);
			}
			ds.Tables.Add(dataTable4.Copy());
			DataTable dataSourceTable2 = grd_Operation.DataSourceTable;
			DataRow[] array = dataSourceTable2.Select(string.Format("U_UnitUoM='{0}' and U_DelStatus='{1}' AND U_QtyUoM > 1", "C", "N"));
			if (array != null && array.Length != 0)
			{
				for (int num = 0; num < array.Length; num++)
				{
					DataRow[] array2 = dataTable3.Select("U_ID = " + Function.ParseInt(array[num]["U_PlanId"]));
					if (array2.Length != 0 && "OP_GN".Equals(Function.ToString(array2[0]["U_OpCode"])))
					{
						continue;
					}
					int num2 = Function.ParseInt(array[num]["U_QtyUoM"]);
					int num3 = Function.ParseInt(array[num]["U_PackQty"]);
					if (num3 == 0)
					{
						num3 = 1;
					}
					if (num2 > num3)
					{
						array[num]["U_QtyUoM"] = num3;
						array[num]["U_PackQty"] = num3;
						array[num]["U_Quantity"] = Math.Round(Function.ParseDouble(array[num]["U_Width"]) * Function.ParseDouble(array[num]["U_Length"]) * (double)num3 / 100.0, 3, MidpointRounding.AwayFromZero);
						num2 -= num3;
						while (num2 > 0)
						{
							num3 = ((num3 > num2) ? num2 : num3);
							num2 -= num3;
							DataRow dataRow2 = dataSourceTable2.NewRow();
							dataRow2.ItemArray = array[num].ItemArray;
							dataRow2["U_BatchNo"] = "";
							dataRow2["U_QtyUoM"] = num3;
							dataRow2["U_Quantity"] = Math.Round(Function.ParseDouble(array[num]["U_Width"]) * Function.ParseDouble(array[num]["U_Length"]) * (double)num3 / 100.0, 3, MidpointRounding.AwayFromZero);
							dataSourceTable2.Rows.Add(dataRow2);
						}
					}
				}
				if (dataSourceTable2.Rows.Count > 50 && XtraMessageBox.Show("Thông tin thu hồi lớn hơn 50 dòng. Bạn có muốn tiếp tục lưu?", "Cảnh báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
				{
					return;
				}
			}
			DataTable dataTable5 = dataSourceTable2.Copy();
			dataTable5.TableName = "PWB4";
			if (!dataTable5.ExistsColumn("DocEntry"))
			{
				dataTable5.Columns.Add("DocEntry", typeof(int));
			}
			if (!dataTable5.ExistsColumn("LineNum"))
			{
				dataTable5.Columns.Add("LineNum", typeof(int));
			}
			foreach (string item3 in (from DataColumn t in dataTable5.Columns
				where !t.ColumnName.Equals("DocEntry") && !t.ColumnName.Equals("LineNum") && !t.ColumnName.StartsWith("U_")
				select t.ColumnName).ToList())
			{
				dataTable5.Columns.Remove(item3);
			}
			ds.Tables.Add(dataTable5.Copy());
			HttpResult httpResult = await Save(ds);
			ds.DisposeDataSet();
			if (string.IsNullOrEmpty(httpResult.Message))
			{
				if (FormMode != Enums.FormViewModeType.Update)
				{
					WinCommons.writeStatus(Enums.MessageCode.SaveSuccessWithDocEntry, Enums.MessageType.Success, Msg_Time._TimeMedium, "", Function.ToString(httpResult.Content));
					txt_DocNo.EditValue = httpResult.Content;
				}
				btn_OK.Text = Resources.ButtonOK;
				FormMode = Enums.FormViewModeType.Update;
			}
			else if (httpResult.Message.Equals("INVALIDSESSION") || httpResult.Message.Equals("Invalid Session ID"))
			{
				XtraMessageBox.Show("User bị mất Session. Thực hiện lại thao tác hoặc đăng nhập lại!", "System", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
			}
			else
			{
				XtraMessageBox.Show(httpResult.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<HttpResult> Save(DataSet ds)
	{
		return await _httpClient.Udo.Save(ds);
	}

	private async void btn_GenerateProductionOrder_Click(object sender, EventArgs e)
	{
		_ = 3;
		try
		{
			if (string.IsNullOrEmpty(txt_DocNo.Text))
			{
				XtraMessageBox.Show("Chưa Add phiếu tính giấy", "System", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
				WinCommons._CloseProcessing();
				return;
			}
			DataTable dt = tv_ProductionPlanning.DataSourceTable();
			if (dt.IsNotNull())
			{
				List<ProductionOder_Impl_OWOR> oDocs = new List<ProductionOder_Impl_OWOR>();
				DataRowCollection rows = dt.Rows;
				if ((from t in dt.AsEnumerable()
					where string.IsNullOrEmpty(Function.ToString(t["U_Resource"])) && !"OP_GN".Equals(Function.ToString(t["U_OpCode"]))
					select t).ToList().Count > 0)
				{
					XtraMessageBox.Show("Chưa chọn \"Resource\"", "System", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
					gv_Planned.ExpandAllGroups();
					WinCommons._CloseProcessing();
					return;
				}
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
				{
					return;
				}
				WinCommons._OpenProcessing();
				btn_GenerateProductionOrder.Enabled = false;
				DataTable dataSourceTable = grd_ProductionRequest.DataSourceTable;
				string itemcode = Function.ToString(dataSourceTable.Rows[0]["U_ItemCode"]);
				string batchPlan = "";
				int idx = 0;
				for (int i = 0; i < rows.Count; i++)
				{
					string value = Function.ToString(rows[i]["U_OpCode"]);
					if ("OP_GN".Equals(value))
					{
						continue;
					}
					int planid = Function.ParseInt(rows[i]["U_ID"]);
					DataTable opreationSource = grd_Operation.DataSourceTable;
					string text = Function.ToString(rows[i]["PwStatus"]);
					if (text.Equals("R") || text.Equals("L"))
					{
						continue;
					}
					DataRow[] array = opreationSource.Select(string.Format("U_PlanId='{0}' and U_DelStatus='{1}'", planid, "Y"));
					ProductionOder_Impl_OWOR item = new ProductionOder_Impl_OWOR
					{
						ItemCode = itemcode
					};
					if (string.IsNullOrEmpty(item.ItemCode))
					{
						item.ItemCode = Function.ToString(rows[0]["U_ItemCode"]);
					}
					item.DocEntry = Function.ParseInt(rows[i]["U_PwNo"]);
					item.PlannedQty = Function.ParseDouble(rows[i]["U_Quantity"]);
					item.Warehouse = Function.ToString(rows[i]["U_WhsCode"]);
					item.DueDate = Function.ParseDateTime(rows[i]["U_DueDate"]);
					item.OriginType = "S";
					item.UserFields = new List<UserFieldsImpl>();
					UserFieldsImpl userFieldsImpl = new UserFieldsImpl();
					userFieldsImpl.Type = BoFieldTypes.db_Alpha;
					userFieldsImpl.Name = "U_IssBatchP";
					userFieldsImpl.Value = Function.ToString(rows[i]["U_BatchNo"]);
					item.UserFields.Add(userFieldsImpl);
					if (array.Length != 0)
					{
						if (item.DocEntry <= 0)
						{
							item.OriginAbs = Function.ParseInt(array[0]["U_SoNo"]);
						}
						UserFieldsImpl userFieldsImpl2 = new UserFieldsImpl();
						userFieldsImpl2.Type = BoFieldTypes.db_Alpha;
						userFieldsImpl2.Name = "U_SOLine";
						userFieldsImpl2.Value = Function.ToString(array[0]["U_SoLine"]);
						item.UserFields.Add(userFieldsImpl2);
					}
					int num = ((!string.IsNullOrEmpty(Function.ToString(rows[i]["DocEntry"]))) ? Function.ParseInt(rows[i]["DocEntry"]) : Function.ParseInt(txt_DocNo.EditValue));
					item.UserFields.Add(new UserFieldsImpl
					{
						Type = BoFieldTypes.db_Numeric,
						Name = "U_PWBEntry",
						Value = num
					});
					item.UserFields.Add(new UserFieldsImpl
					{
						Type = BoFieldTypes.db_Numeric,
						Name = "U_PWBLine",
						Value = Function.ParseInt(rows[i]["U_ID"])
					});
					item.Line_WOR1 = new List<ProductionOder_LineImpl_WOR1>();
					DataTable dataTable = await _httpClient.BillOfMasterials.GetLineByFather(item.ItemCode, "", GlobalData.DefaultBranch);
					if (dataTable.IsNotNull())
					{
						for (int num2 = 0; num2 < dataTable.Rows.Count; num2++)
						{
							ProductionOder_LineImpl_WOR1 productionOder_LineImpl_WOR = new ProductionOder_LineImpl_WOR1();
							productionOder_LineImpl_WOR.ItemType = Function.ToString(dataTable.Rows[num2]["Type"]);
							productionOder_LineImpl_WOR.ItemCode = Function.ToString(rows[i]["U_Resource"]);
							productionOder_LineImpl_WOR.wareHouse = Function.ToString(dataTable.Rows[num2]["Warehouse"]);
							productionOder_LineImpl_WOR.PlannedQty = Function.ParseDouble(dataTable.Rows[num2]["RateQty"]) * item.PlannedQty;
							item.Line_WOR1.Add(productionOder_LineImpl_WOR);
						}
					}
					else
					{
						ProductionOder_LineImpl_WOR1 productionOder_LineImpl_WOR2 = new ProductionOder_LineImpl_WOR1();
						productionOder_LineImpl_WOR2.ItemType = Function.ToString(290);
						productionOder_LineImpl_WOR2.ItemCode = Function.ToString(rows[i]["U_Resource"]);
						productionOder_LineImpl_WOR2.wareHouse = Function.ToString(rows[i]["U_WhsCode"]);
						productionOder_LineImpl_WOR2.PlannedQty = item.PlannedQty;
						if (!string.IsNullOrEmpty(productionOder_LineImpl_WOR2.ItemCode))
						{
							item.Line_WOR1.Add(productionOder_LineImpl_WOR2);
						}
					}
					oDocs.Add(item);
					DataRow[] array2 = opreationSource.Select(string.Format("U_PlanId='{0}' and U_DelStatus='{1}'", planid, "N"));
					if (array2.Length == 0)
					{
						continue;
					}
					for (int num3 = 0; num3 < array2.Length; num3++)
					{
						double num4 = Function.ParseDouble(array2[num3]["U_QtyUoM"]);
						double num5 = Function.ParseDouble(array2[num3]["U_PackQty"]);
						if (num5 == 0.0)
						{
							num5 = num4;
						}
						if (Function.ToString(array2[num3]["U_UnitUoM"]).Equals("C") && num4 > 1.0 && num4 > num5)
						{
							array2[num3]["U_QtyUoM"] = num5;
							array2[num3]["U_PackQty"] = num5;
							array2[num3]["U_Quantity"] = Math.Round(Function.ParseDouble(array2[num3]["U_Width"]) * Function.ParseDouble(array2[num3]["U_Length"]) * num5 / 100.0, 3, MidpointRounding.AwayFromZero);
							num4 -= num5;
							while (num4 > 0.0)
							{
								num5 = ((num5 > num4) ? num4 : num5);
								num4 -= num5;
								DataRow dataRow = opreationSource.NewRow();
								dataRow.ItemArray = array2[num3].ItemArray;
								dataRow["U_BatchNo"] = "";
								dataRow["U_QtyUoM"] = num5;
								dataRow["U_Quantity"] = Math.Round(Function.ParseDouble(array2[num3]["U_Width"]) * Function.ParseDouble(array2[num3]["U_Length"]) * num5 / 100.0, 3, MidpointRounding.AwayFromZero);
								opreationSource.Rows.Add(dataRow);
							}
						}
						else if (Function.ToString(array2[num3]["U_UnitUoM"]).Equals("T") && num4 > num5)
						{
							double num6 = Function.ParseDouble(array2[num3]["U_Width"]);
							double num7 = Function.ParseDouble(array2[num3]["U_length"]);
							array2[num3]["U_QtyUoM"] = num5;
							array2[num3]["U_Quantity"] = Math.Round(num6 * num7 * num5 / 100.0, 3, MidpointRounding.AwayFromZero);
							num4 -= num5;
							while (num4 > 0.0)
							{
								num5 = ((num5 > num4) ? num4 : num5);
								num4 -= num5;
								DataRow dataRow2 = opreationSource.NewRow();
								dataRow2.ItemArray = array2[num3].ItemArray;
								dataRow2["U_QtyUoM"] = num5;
								dataRow2["U_Quantity"] = Math.Round(num6 * num7 * num5 / 100.0, 3, MidpointRounding.AwayFromZero);
								opreationSource.Rows.Add(dataRow2);
							}
						}
					}
					opreationSource.AcceptChanges();
					string text2 = Function.ToString(rows[i]["U_BatchNo"]);
					if (!string.IsNullOrEmpty(text2) && !batchPlan.Equals(text2))
					{
						batchPlan = text2;
					}
					string[] array3 = batchPlan.Split(';');
					if (array3 != null && array3.Length != 0)
					{
						batchPlan = array3[0];
					}
					if (batchPlan.Length > 25)
					{
						string[] array4 = text2.Split('.');
						if (array4.Length > 2)
						{
							batchPlan = $"{array4[0]}.{array4[1]}\\{array4[2]}";
						}
					}
					if (string.IsNullOrEmpty(batchPlan) || batchPlan.Length < 8)
					{
						batchPlan = DateTime.Now.ToString("yyMMddhhmmss");
					}
					DataRow[] array5 = opreationSource.Select(string.Format("U_PlanId='{0}' and U_DelStatus='{1}'", planid, "N"));
					for (int num8 = 0; num8 < array5.Length; num8++)
					{
						string text3 = Function.ToString(array5[num8]["U_BatchNo"]);
						if (string.IsNullOrEmpty(text3))
						{
							array5[num8]["U_BatchNo"] = $"{batchPlan}.{idx}";
							idx++;
						}
						else if (text3.Equals($"{batchPlan}.{idx}"))
						{
							idx++;
						}
					}
					grd_Operation.DataSourceTable.AcceptChanges();
				}
				DocumentsImpl tran = new DocumentsImpl();
				string filler = Function.ToString(rows[0]["U_FromWhsCode"]);
				string toWhsCode = Function.ToString(rows[0]["U_WhsCode"]);
				tran.UserFields = new List<UserFieldsImpl>
				{
					new UserFieldsImpl
					{
						Name = "U_PlanId",
						Type = BoFieldTypes.db_Numeric,
						Value = Function.ParseInt(txt_DocNo.EditValue)
					}
				};
				tran.Filler = filler;
				tran.ToWhsCode = toWhsCode;
				tran.Comments = Function.ToString(txt_Remark.Text);
				tran.Lines = new List<Document_LinesImpl>();
				int num9 = 0;
				for (int num10 = 0; num10 < rows.Count; num10++)
				{
					if (string.IsNullOrEmpty(Function.ToString(rows[num10]["U_BatchNo"])))
					{
						continue;
					}
					string[] array6 = Function.ToString(rows[num10]["U_BatchNo"]).Split(';');
					if (array6.Length == 0)
					{
						continue;
					}
					string[] array7 = array6;
					foreach (string text4 in array7)
					{
						Document_LinesImpl document_LinesImpl = new Document_LinesImpl();
						document_LinesImpl.ItemCode = Function.ToString(rows[num10]["U_ItemCode"]);
						if (string.IsNullOrEmpty(document_LinesImpl.ItemCode))
						{
							document_LinesImpl.ItemCode = Function.ToString(rows[0]["U_ItemCode"]);
						}
						document_LinesImpl.FromWhsCod = Function.ToString(rows[num10]["U_FromWhsCode"]);
						document_LinesImpl.WhsCode = Function.ToString(rows[num10]["U_WhsCode"]);
						document_LinesImpl.UserFields = new List<UserFieldsImpl>();
						document_LinesImpl.UserFields.Add(new UserFieldsImpl
						{
							Name = "U_DistNumber",
							Type = BoFieldTypes.db_Alpha,
							Value = Function.ToString(text4)
						});
						document_LinesImpl.Quantity = Function.ParseDouble(rows[num10]["U_Quantity"]) / (double)array6.Length;
						string value2 = Function.ToString(rows[num10]["PwStatus"]);
						if (!document_LinesImpl.FromWhsCod.ToUpper().Equals(document_LinesImpl.WhsCode.ToUpper()) && string.IsNullOrEmpty(value2))
						{
							if (string.IsNullOrEmpty(Function.ToString(rows[num10]["U_RequireTime"])))
							{
								string text5 = DateTime.Now.AddMinutes(20.0).Hour.ToString("00");
								string text6 = DateTime.Now.AddMinutes(20.0).Minute.ToString("00");
								string arg = text5 + text6;
								rows[num10]["U_RequireTime"] = $"{arg}";
							}
							if (rows[num10].IsExistsColumn("U_RequireDate") && string.IsNullOrEmpty(Function.ToString(rows[num10]["U_RequireDate"])))
							{
								rows[num10]["U_RequireDate"] = DateTime.Now.AddMinutes(20.0).Date;
							}
							document_LinesImpl.LineNum = num9;
							tran.Lines.Add(document_LinesImpl);
							num9++;
						}
					}
				}
				HttpResult ret = await _httpClient.ProductionOrders.SaveProductionPlan(oDocs, tran, grd_Operation.DataSourceTable);
				if (ret != null && !string.IsNullOrEmpty(ret.Message))
				{
					if (ret.Message.Equals("INVALIDSESSION") || ret.Message.Equals("Invalid Session ID"))
					{
						XtraMessageBox.Show("User bị mất Session. Thực hiện lại thao tác hoặc đăng nhập lại!", "System", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
					}
					else
					{
						WinCommons.Error(Enums.MessageCode.None, ret.Message);
					}
					DataRow[] array8 = grd_Operation.DataSourceTable.Select("U_DelStatus = 'N' AND U_BatchNo <> ''");
					if (array8 != null && array8.Length != 0)
					{
						DataRow[] array9 = array8;
						for (int num11 = 0; num11 < array9.Length; num11++)
						{
							array9[num11]["U_BatchNo"] = "";
						}
					}
				}
				else if (ret != null)
				{
					List<DataRow> list = (from t in dt.AsEnumerable()
						where !Function.ToString(t["PwStatus"]).Equals("R") && !Function.ToString(t["PwStatus"]).Equals("L") && !Function.ToString(t["U_OpCode"]).Equals("OP_GN")
						select t).ToList();
					List<string> list2 = Function.ToString(ret.Content).Split(',').ToList();
					int num12 = list2.Count;
					if (tran != null && tran.Lines != null && tran.Lines.Count > 0 && num12 > list.Count)
					{
						txt_StockRequest.Text = list2[num12 - 1];
						num12--;
					}
					if (num12 > 0 && list.Count == num12)
					{
						for (int num13 = 0; num13 < num12; num13++)
						{
							list[num13]["U_PwNo"] = Function.ParseInt(list2[num13]);
							list[num13]["PwStatus"] = "P";
						}
						dt.AcceptChanges();
						await SaveUdo();
					}
					WinCommons.Success(Enums.MessageCode.SuccessWithDocNo, "", Function.ToString(ret.Content));
					await PRINTLAYOUT();
				}
				else
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		btn_GenerateProductionOrder.Enabled = true;
	}

	private async void btn_ConfirmProduction_Click(object sender, EventArgs e)
	{
		_ = 4;
		try
		{
			btn_ConfirmProduction.Enabled = false;
			int suffix = 1;
			int suffixTH = 0;
			string issueBatch = "";
			if (string.IsNullOrEmpty(txt_DocNo.Text))
			{
				XtraMessageBox.Show("Chưa Add phiếu tính giấy", "System", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
				return;
			}
			DataTable dtIssue = tv_ProductionPlanning.DataSourceTable();
			if (!dtIssue.IsNotNull())
			{
				return;
			}
			new List<ProductionOder_Impl_OWOR>();
			_ = dtIssue.Rows;
			if ((from t in dtIssue.AsEnumerable()
				where string.IsNullOrEmpty(Function.ToString(t["U_Resource"]))
				select t).ToList().Count > 0)
			{
				XtraMessageBox.Show("Chưa chọn \"Resource\"", "System", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
				gv_Planned.ExpandAllGroups();
			}
			else if ((from t in dtIssue.AsEnumerable()
				where Function.ParseInt(t["U_PwNo"]) > 0
				select t).ToList().Count > 0)
			{
				XtraMessageBox.Show("Phiếu tính giấy đã có lệnh SX không thể báo cáo nhanh!", "System", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
				gv_Planned.ExpandAllGroups();
			}
			else
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
				{
					return;
				}
				WinCommons._OpenProcessing();
				DataTable dataSourceTable = grd_ProductionRequest.DataSourceTable;
				string itemcode = Function.ToString(dataSourceTable.Rows[0]["U_ItemCode"]);
				DataTable dtReceipt = grd_Operation.DatasourceTable();
				string value = string.Join(";", from t in dtIssue.AsEnumerable()
					select Function.ToString(t["U_BatchNo"]));
				string whs = Function.ToString(dtIssue.Rows[0]["U_WhsCode"]);
				double planqty = Math.Round(dtIssue.AsEnumerable().Sum((DataRow t) => Function.ParseDouble(t["U_Quantity"])), 3, MidpointRounding.AwayFromZero);
				if (dtReceipt.AsEnumerable().Sum((DataRow t) => Function.ParseDouble(t["U_Quantity"])) > planqty && XtraMessageBox.Show("Số lượng nhập lớn hơn số lượng hàng xuất! Bạn có muốn tiếp tục?", "System", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
				{
					return;
				}
				double receiptQtyGH = 0.0;
				double receiptQtyTH = 0.0;
				DataRow[] array = dtReceipt.Select(string.Format("U_DelStatus='{0}'", "Y"));
				ProductionOder_Impl_OWOR production = new ProductionOder_Impl_OWOR
				{
					DueDate = DateTime.Today,
					ItemCode = itemcode,
					PlannedQty = planqty,
					Warehouse = whs
				};
				if (array != null && array.Length != 0)
				{
					production.OriginAbs = Function.ParseInt(array[0]["U_SoNo"]);
				}
				production.OriginType = "S";
				production.UserFields = new List<UserFieldsImpl>();
				production.UserFields.Add(new UserFieldsImpl
				{
					Name = "U_IssBatchP",
					Value = value
				});
				int num = ((!string.IsNullOrEmpty(Function.ToString(dtIssue.Rows[0]["DocEntry"]))) ? Function.ParseInt(dtIssue.Rows[0]["DocEntry"]) : Function.ParseInt(txt_DocNo.EditValue));
				production.UserFields.Add(new UserFieldsImpl
				{
					Type = BoFieldTypes.db_Numeric,
					Name = "U_PWBEntry",
					Value = num
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
					for (int num2 = 0; num2 < dataTable.Rows.Count; num2++)
					{
						ProductionOder_LineImpl_WOR1 productionOder_LineImpl_WOR = new ProductionOder_LineImpl_WOR1();
						productionOder_LineImpl_WOR.ItemType = Function.ToString(dataTable.Rows[num2]["Type"]);
						productionOder_LineImpl_WOR.ItemCode = Function.ToString(dtIssue.Rows[0]["U_Resource"]);
						productionOder_LineImpl_WOR.wareHouse = Function.ToString(dataTable.Rows[num2]["Warehouse"]);
						productionOder_LineImpl_WOR.PlannedQty = Function.ParseDouble(dataTable.Rows[num2]["RateQty"]) * production.PlannedQty;
						production.Line_WOR1.Add(productionOder_LineImpl_WOR);
					}
				}
				DocumentsImpl docIssue = new DocumentsImpl
				{
					BPLId = Function.ParseInt(GlobalData.DefaultBranch),
					Lines = new List<Document_LinesImpl>()
				};
				Document_LinesImpl document_LinesImpl = new Document_LinesImpl();
				document_LinesImpl.ItemCode = itemcode;
				document_LinesImpl.Quantity = planqty;
				document_LinesImpl.WhsCode = Function.ToString(dtIssue.Rows[0]["U_FromWhsCode"]);
				document_LinesImpl.BatchNumbers = new List<BatchNumbersImpl>();
				document_LinesImpl.BinAllocations = new List<DocumentLinesBinAllocationsImpl>();
				for (int num3 = 0; num3 < dtIssue.Rows.Count; num3++)
				{
					string[] array2 = Function.ToString(dtIssue.Rows[num3]["U_BatchNo"]).Split(';');
					string[] array3 = array2;
					foreach (string batchNumber in array3)
					{
						BatchNumbersImpl batchNumbersImpl = new BatchNumbersImpl();
						batchNumbersImpl.Quantity = Math.Round(Function.ParseDouble(dtIssue.Rows[num3]["U_Quantity"]) / (double)array2.Length, 3, MidpointRounding.AwayFromZero);
						batchNumbersImpl.BatchNumber = batchNumber;
						document_LinesImpl.BatchNumbers.Add(batchNumbersImpl);
					}
					DocumentLinesBinAllocationsImpl documentLinesBinAllocationsImpl = new DocumentLinesBinAllocationsImpl();
					documentLinesBinAllocationsImpl.BaseLineNumber = num3;
					documentLinesBinAllocationsImpl.BinAbsEntry = Function.ParseInt(dtIssue.Rows[num3]["U_BinAbs"]);
					documentLinesBinAllocationsImpl.Quantity = document_LinesImpl.Quantity;
					documentLinesBinAllocationsImpl.SerialAndBatchNumbersBaseLine = num3;
					if (documentLinesBinAllocationsImpl.BinAbsEntry > 0)
					{
						document_LinesImpl.BinAllocations.Add(documentLinesBinAllocationsImpl);
					}
				}
				docIssue.Lines.Add(document_LinesImpl);
				if (lstUdf == null || lstUdf.Count == 0)
				{
					lstUdf = await _httpClient.Udf.GetListField("OBTN");
				}
				DocumentsImpl documentsImpl = new DocumentsImpl();
				documentsImpl.BPLId = Function.ParseInt(GlobalData.DefaultBranch);
				if (dtReceipt.IsNotNull())
				{
					documentsImpl.Lines = new List<Document_LinesImpl>();
					Document_LinesImpl document_LinesImpl2 = new Document_LinesImpl();
					document_LinesImpl2.WhsCode = Function.ToString(dtReceipt.Rows[0]["U_WhsCode"]);
					if (whs.ToUpper().Equals("SW07"))
					{
						document_LinesImpl2.WhsCode = "SW09";
					}
					if (whs.ToUpper().Equals("SW11"))
					{
						document_LinesImpl2.WhsCode = "SW12";
					}
					document_LinesImpl2.BaseType = 202;
					document_LinesImpl2.BaseLine = -1;
					document_LinesImpl2.BatchNumbers = new List<BatchNumbersImpl>();
					document_LinesImpl2.BinAllocations = new List<DocumentLinesBinAllocationsImpl>();
					Document_LinesImpl document_LinesImpl3 = new Document_LinesImpl();
					document_LinesImpl3.WhsCode = Function.ToString(dtReceipt.Rows[0]["U_WhsCode"]);
					if (whs.ToUpper().Equals("SW07"))
					{
						document_LinesImpl3.WhsCode = "SW07";
					}
					if (whs.ToUpper().Equals("SW11"))
					{
						document_LinesImpl3.WhsCode = "SW11";
					}
					document_LinesImpl3.BaseType = 202;
					document_LinesImpl3.BaseLine = -1;
					document_LinesImpl3.BatchNumbers = new List<BatchNumbersImpl>();
					document_LinesImpl3.BinAllocations = new List<DocumentLinesBinAllocationsImpl>();
					for (int num5 = 0; num5 < dtReceipt.Rows.Count; num5++)
					{
						string text = Function.ToString(dtReceipt.Rows[num5]["U_UnitUoM"]);
						int num6 = Function.ParseInt(dtReceipt.Rows[num5]["U_PackQty"]);
						int num7 = Function.ParseInt(dtReceipt.Rows[num5]["U_QtyUoM"]);
						if (Math.Round(Function.ParseDouble(dtReceipt.Rows[num5]["U_Quantity"]), 3, MidpointRounding.AwayFromZero) <= 0.0)
						{
							XtraMessageBox.Show("Nhập số lượng báo cáo!", "System", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
							clearBatch();
							return;
						}
						if (num6 <= 0)
						{
							XtraMessageBox.Show("Nhập số lượng packing!", "System", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
							clearBatch();
							return;
						}
						if (text.Equals("T") && num6 < 100 && (double)num6 < (double)num7 * 0.8 && XtraMessageBox.Show("Số lượng packing quá thấp. Bạn có muốn tiếp tục?", "System", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
						{
							clearBatch();
							return;
						}
						DataRow dataRow = dtIssue.Select(string.Format("U_ID={0}", Function.ParseInt(dtReceipt.Rows[num5]["U_PlanId"])))[0];
						string text2 = Function.ToString(dataRow["U_BatchNo"]).Split(';')[0];
						if (!text2.Equals(issueBatch))
						{
							issueBatch = text2;
							suffix = 1;
							suffixTH = 0;
						}
						string delsta = Function.ToString(dtReceipt.Rows[num5]["U_DelStatus"]);
						DataRow dataRow2;
						while (num6 <= num7)
						{
							List<UserFieldsImpl> list = new List<UserFieldsImpl>();
							foreach (UserFieldsImpl item in lstUdf)
							{
								UserFieldsImpl newUdf = getNewUdf(item, dtReceipt.Rows[num5], delsta, num6);
								if (newUdf != null)
								{
									list.Add(newUdf);
								}
							}
							BatchNumbersImpl batchNumbersImpl2 = new BatchNumbersImpl();
							batchNumbersImpl2.UserFields = list;
							double num8 = Function.ParseInt(dataRow["U_ReNo"]);
							if (num8 > 0.0)
							{
								batchNumbersImpl2.AddmisionDate = DateTime.Today.AddDays(0.0 - num8);
							}
							batchNumbersImpl2.Quantity = Math.Round(Function.ParseDouble(dtReceipt.Rows[num5]["U_Width"]) * Function.ParseDouble(dtReceipt.Rows[num5]["U_Length"]) / 100.0 * (double)num6, 3, MidpointRounding.AwayFromZero);
							if (string.IsNullOrEmpty(Function.ToString(dtReceipt.Rows[num5]["U_BatchNo"])) && Function.ToString(dtReceipt.Rows[num5]["U_DelStatus"]).Equals("N"))
							{
								batchNumbersImpl2.BatchNumber = CreateAutoBatchTH(issueBatch, suffixTH);
								suffixTH++;
								dataRow2 = dtReceipt.Rows[num5];
								dataRow2["U_BatchNo"] = dataRow2["U_BatchNo"]?.ToString() + ";" + batchNumbersImpl2.BatchNumber;
								dtReceipt.Rows[0]["U_WhsCode"] = "SW07";
								document_LinesImpl3.BatchNumbers.Add(batchNumbersImpl2);
								receiptQtyTH += batchNumbersImpl2.Quantity;
							}
							if (string.IsNullOrEmpty(batchNumbersImpl2.BatchNumber) || batchNumbersImpl2.BatchNumber.Length < 4)
							{
								batchNumbersImpl2.BatchNumber = CreateAutoBatch(issueBatch, suffix);
								suffix++;
								dataRow2 = dtReceipt.Rows[num5];
								dataRow2["U_BatchNo"] = dataRow2["U_BatchNo"]?.ToString() + ";" + batchNumbersImpl2.BatchNumber;
								dtReceipt.Rows[0]["U_WhsCode"] = "SW09";
								document_LinesImpl2.BatchNumbers.Add(batchNumbersImpl2);
								receiptQtyGH += batchNumbersImpl2.Quantity;
							}
							if (dtReceipt.Rows[num5].IsExistsColumn("Notes"))
							{
								batchNumbersImpl2.Notes = Function.ToString(dtReceipt.Rows[num5]["Notes"]);
							}
							if (dataRow.IsExistsColumn("U_LotNumber"))
							{
								batchNumbersImpl2.InternalSerialNumber = Function.ToString(dataRow["U_LotNumber"]);
							}
							num7 -= num6;
						}
						if (num7 <= 0)
						{
							continue;
						}
						List<UserFieldsImpl> list2 = new List<UserFieldsImpl>();
						foreach (UserFieldsImpl item2 in lstUdf)
						{
							UserFieldsImpl newUdf2 = getNewUdf(item2, dtReceipt.Rows[num5], delsta, num7);
							if (newUdf2 != null)
							{
								list2.Add(newUdf2);
							}
						}
						BatchNumbersImpl batchNumbersImpl3 = new BatchNumbersImpl();
						batchNumbersImpl3.UserFields = list2;
						batchNumbersImpl3.Quantity = Math.Round(Function.ParseDouble(dtReceipt.Rows[num5]["U_Width"]) * Function.ParseDouble(dtReceipt.Rows[num5]["U_Length"]) / 100.0 * (double)num7, 3, MidpointRounding.AwayFromZero);
						batchNumbersImpl3.BatchNumber = CreateAutoBatch(issueBatch, suffix);
						suffix++;
						dataRow2 = dtReceipt.Rows[num5];
						dataRow2["U_BatchNo"] = dataRow2["U_BatchNo"]?.ToString() + ";" + batchNumbersImpl3.BatchNumber;
						if (dtReceipt.Rows[num5].IsExistsColumn("Notes"))
						{
							batchNumbersImpl3.Notes = Function.ToString(dtReceipt.Rows[num5]["Notes"]);
						}
						if (dtReceipt.Rows[num5].IsExistsColumn("U_LotNumber"))
						{
							batchNumbersImpl3.InternalSerialNumber = Function.ToString(dtReceipt.Rows[num5]["U_LotNumber"]);
						}
						receiptQtyGH += batchNumbersImpl3.Quantity;
						document_LinesImpl2.BatchNumbers.Add(batchNumbersImpl3);
					}
					document_LinesImpl2.Quantity = receiptQtyGH;
					document_LinesImpl3.Quantity = receiptQtyTH;
					if (document_LinesImpl2.Quantity > 0.0)
					{
						documentsImpl.Lines.Add(document_LinesImpl2);
					}
					if (document_LinesImpl3.Quantity > 0.0)
					{
						documentsImpl.Lines.Add(document_LinesImpl3);
					}
					if (documentsImpl.Lines.Count > 1)
					{
						for (int num9 = 0; num9 < documentsImpl.Lines.Count; num9++)
						{
							documentsImpl.Lines[num9].LineNum = num9;
							foreach (BatchNumbersImpl batchNumber2 in documentsImpl.Lines[num9].BatchNumbers)
							{
								batchNumber2.BaseLineNumber = num9;
							}
						}
					}
					string text3 = string.Join("", from t in dtReceipt.AsEnumerable()
						select Function.ToString(t["U_BatchNo"]));
					text3 = (text3.StartsWith(";") ? text3.Substring(1) : text3);
					production.UserFields.Add(new UserFieldsImpl
					{
						Name = "U_ReBatch",
						Value = text3
					});
				}
				HttpResult httpResult = await _httpClient.ProductionOrders.ConfirmBatch(new Tuple<DocumentsImpl, DocumentsImpl, ProductionOder_Impl_OWOR>(docIssue, documentsImpl, production));
				if (!string.IsNullOrEmpty(httpResult.Message))
				{
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
					clearBatch();
					return;
				}
				WinCommons.Success(Enums.MessageCode.Success, "");
				int docEntry = Function.ParseInt(httpResult.Content);
				if (docEntry <= 0)
				{
					return;
				}
				foreach (DataRow row in dtIssue.Rows)
				{
					row["U_PwNo"] = docEntry;
					row["PwStatus"] = "L";
				}
				dtIssue.AcceptChanges();
				await SaveUdo();
				await WinFuntions.Print("", docEntry, "WOR1");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			WinCommons._CloseProcessing();
			btn_ConfirmProduction.Enabled = true;
		}
	}

	private UserFieldsImpl getNewUdf(UserFieldsImpl udf, DataRow row, string delsta, int qtyuom)
	{
		UserFieldsImpl userFieldsImpl = new UserFieldsImpl();
		if (udf.Name.Equals("QtyUoM"))
		{
			userFieldsImpl.Name = udf.Name;
			userFieldsImpl.Type = udf.Type;
			userFieldsImpl.SubType = udf.SubType;
			userFieldsImpl.Value = qtyuom;
		}
		else if (udf.Name.Equals("DelRequest") && delsta.Equals("Y"))
		{
			userFieldsImpl.Name = udf.Name;
			userFieldsImpl.Type = udf.Type;
			userFieldsImpl.SubType = udf.SubType;
			userFieldsImpl.Value = 'Y';
		}
		else if (udf.Name.Equals("ObjType") && delsta.Equals("Y"))
		{
			userFieldsImpl.Name = udf.Name;
			userFieldsImpl.Type = udf.Type;
			userFieldsImpl.SubType = udf.SubType;
			userFieldsImpl.Value = 17;
		}
		else if (udf.Name.Equals("DocEntry") && delsta.Equals("Y"))
		{
			userFieldsImpl.Name = udf.Name;
			userFieldsImpl.Type = udf.Type;
			userFieldsImpl.SubType = udf.SubType;
			userFieldsImpl.Value = row["U_SoNo"];
		}
		else if (udf.Name.Equals("LineID") && delsta.Equals("Y"))
		{
			userFieldsImpl.Name = udf.Name;
			userFieldsImpl.Type = udf.Type;
			userFieldsImpl.SubType = udf.SubType;
			userFieldsImpl.Value = row["U_SoLine"];
		}
		else
		{
			string text = "U_" + udf.Name;
			if (!row.IsExistsColumn(text))
			{
				return null;
			}
			userFieldsImpl.Name = udf.Name;
			userFieldsImpl.Type = udf.Type;
			userFieldsImpl.SubType = udf.SubType;
			userFieldsImpl.Value = row[text];
		}
		return userFieldsImpl;
	}

	private void clearBatch()
	{
		DataRow[] array = grd_Operation.DataSourceTable.Select("U_BatchNo <> ''");
		if (array != null && array.Length != 0)
		{
			DataRow[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i]["U_BatchNo"] = "";
			}
		}
	}

	private void txt_DocNo_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			btn_GenerateProductionOrder.Enabled = Function.ParseInt(txt_DocNo.EditValue) > 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_AutoCalc_Click(object sender, EventArgs e)
	{
		try
		{
			DataRow row = ((DataRowView)tv_ProductionPlanning.GetDataRecordByNode(tv_ProductionPlanning.FocusedNode)).Row;
			double num = Function.ParseDouble(row["U_Width"]);
			double num2 = Function.ParseDouble(row["U_Length"]);
			double num3 = Function.ParseDouble(row["U_QtyUoM"]);
			string text = Function.ToString(row["U_UnitUoM"]);
			DataTable dataSourceTable = grd_Operation.DataSourceTable;
			int num4 = Function.ParseInt(row["U_ID"]);
			DataRow[] array = dataSourceTable.Select($"U_PlanId='{num4}'");
			if (array != null && array.Length != 0)
			{
				DataRow[] array2 = array;
				foreach (DataRow row2 in array2)
				{
					dataSourceTable.Rows.Remove(row2);
				}
			}
			ReloadPlannedRequest();
			double num5 = num;
			double num6 = num2;
			double num7 = num3;
			string value = text;
			if (gv_ProductionRequest.GetSelectedRows() != null && gv_ProductionRequest.GetSelectedRows().Length != 0)
			{
				for (int j = 0; j < gv_ProductionRequest.GetSelectedRows().Length; j++)
				{
					int num8 = gv_ProductionRequest.GetSelectedRows()[j];
					if (num8 < 0)
					{
						continue;
					}
					DataRow dataRow = gv_ProductionRequest.GetDataRow(num8);
					double num9 = Function.ParseDouble(dataRow["U_Width"]);
					double num10 = Function.ParseDouble(dataRow["U_Length"]);
					double num11 = Function.ParseDouble(dataRow["U_Remain"]);
					string value2 = Function.ToString(dataRow["U_UnitUoM"]);
					if (!(num11 > 0.0) || ((!(num5 >= num9) || !(num6 >= num10)) && (!(num5 >= num10 * 100.0) || num5 % (num10 * 100.0) != 0.0) && !(num6 * Math.Floor(num5 / num9) > num10)))
					{
						continue;
					}
					DataRow dataRow2 = dataSourceTable.NewRow();
					dataRow2["U_ItemCode"] = dataRow["U_ItemCode"];
					dataRow2["U_PlanId"] = num4;
					dataRow2["U_WhsCode"] = GlobalData.UserDefaultGroup.Warehouse;
					dataRow2["U_DelStatus"] = "Y";
					dataRow2["U_StatusQC"] = "OK";
					dataRow2["U_SoNo"] = dataRow["U_SoNo"];
					dataRow2["U_SoLine"] = dataRow["U_SoLine"];
					if ("C".Equals(text) && "C".Equals(value2))
					{
						dataRow2["U_Width"] = num9;
						dataRow2["U_Length"] = num10;
						double num12 = Math.Floor(num5 / num9) * Math.Floor(num6 / num10) * num7;
						double num13 = ((num12 >= num11) ? num11 : num12);
						dataRow2["U_QtyUoM"] = num13;
						dataRow2["U_UnitUoM"] = value2;
						dataRow2["U_Quantity"] = num9 * num10 * num13 / 100.0;
						double num14 = ((num2 > num10) ? Math.Floor(num2 / num10) : 1.0);
						num5 = (double)((decimal)num5 - (decimal)(num9 * num13 / num14));
						num12 = Math.Floor(num5 / num);
						num7 = ((num12 > 0.0) ? num12 : 1.0);
					}
					else if ("C".Equals(text) && "T".Equals(value2))
					{
						if (num5 >= num10 * 100.0 && num5 % (num10 * 100.0) == 0.0)
						{
							double num15 = num9;
							num9 = num10 * 100.0;
							num10 = (double)(decimal)(num15 / 100.0);
						}
						dataRow2["U_Width"] = num9;
						dataRow2["U_Length"] = num6;
						double num16 = Math.Floor(num5 / num9);
						double num17 = ((num16 >= num11) ? num11 : num16);
						dataRow2["U_QtyUoM"] = num17;
						dataRow2["U_UnitUoM"] = value;
						dataRow2["U_Quantity"] = num9 * num6 * num17 / 100.0;
						num5 = (double)((decimal)(num5 * num7) - (decimal)(num9 * num17));
						num16 = Math.Floor(num5 / num);
						num7 = ((num16 > 0.0) ? num16 : 1.0);
						dataRow2["U_WhsCode"] = GlobalData.UserDefaultGroup.Warehouse;
						dataRow2["U_DelStatus"] = "N";
						dataRow2["U_StatusQC"] = "OK";
						dataRow2["U_SoNo"] = 0;
						dataRow2["U_SoLine"] = 0;
					}
					if (!string.IsNullOrEmpty(Function.ToString(dataRow2["U_Width"])))
					{
						dataSourceTable.Rows.Add(dataRow2);
					}
				}
			}
			if (num5 > 0.0 && num6 > 2.0 && num7 > 0.0)
			{
				DataRow dataRow3 = dataSourceTable.NewRow();
				dataRow3["U_ItemCode"] = row["U_ItemCode"];
				dataRow3["U_Width"] = Math.Round(num5, 2);
				dataRow3["U_Length"] = Math.Round(num6, 2) - 2.0;
				dataRow3["U_QtyUoM"] = num7;
				dataRow3["U_UnitUoM"] = text;
				dataRow3["U_Quantity"] = num5 * num6 * num7 / 100.0;
				dataRow3["U_PlanId"] = num4;
				dataRow3["U_WhsCode"] = GlobalData.UserDefaultGroup.Warehouse;
				dataRow3["U_DelStatus"] = "N";
				dataRow3["U_StatusQC"] = "OK";
				dataSourceTable.Rows.Add(dataRow3);
			}
			dataSourceTable.AcceptChanges();
			grd_Operation.RefreshDataSource();
			ReLoadLossQty(dataSourceTable, num4);
			ReloadPlannedRequest();
			FormValueChanged();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_AutoXa_Click(object sender, EventArgs e)
	{
		try
		{
			DataRow row = ((DataRowView)tv_ProductionPlanning.GetDataRecordByNode(tv_ProductionPlanning.FocusedNode)).Row;
			double num = Function.ParseDouble(row["U_Width"]);
			double num2 = Function.ParseDouble(row["U_Length"]);
			double num3 = Function.ParseDouble(row["U_QtyUoM"]);
			string value = Function.ToString(row["U_UnitUoM"]);
			DataTable dataSourceTable = grd_Operation.DataSourceTable;
			int num4 = Function.ParseInt(row["U_ID"]);
			DataRow[] array = dataSourceTable.Select($"U_PlanId='{num4}'");
			if (array != null && array.Length != 0)
			{
				DataRow[] array2 = array;
				foreach (DataRow row2 in array2)
				{
					dataSourceTable.Rows.Remove(row2);
				}
			}
			ReloadPlannedRequest();
			double num5 = num;
			double num6 = num2;
			double num7 = num3;
			if (gv_ProductionRequest.GetSelectedRows() != null && gv_ProductionRequest.GetSelectedRows().Length != 0)
			{
				for (int j = 0; j < gv_ProductionRequest.GetSelectedRows().Length; j++)
				{
					int num8 = gv_ProductionRequest.GetSelectedRows()[j];
					if (num8 < 0)
					{
						continue;
					}
					DataRow dataRow = gv_ProductionRequest.GetDataRow(num8);
					double num9 = Function.ParseDouble(dataRow["U_Width"]);
					double num10 = Function.ParseDouble(dataRow["U_Length"]);
					double num11 = Function.ParseDouble(dataRow["U_Remain"]);
					string value2 = Function.ToString(dataRow["U_UnitUoM"]);
					if (!(num11 > 0.0) || ((!(num5 >= num9) || !(num6 >= num10)) && (!(num5 >= num10 * 100.0) || num5 % (num10 * 100.0) != 0.0)))
					{
						continue;
					}
					DataRow dataRow2 = dataSourceTable.NewRow();
					dataRow2["U_ItemCode"] = dataRow["U_ItemCode"];
					dataRow2["U_PlanId"] = num4;
					dataRow2["U_WhsCode"] = GlobalData.UserDefaultGroup.Warehouse;
					dataRow2["U_DelStatus"] = "Y";
					dataRow2["U_StatusQC"] = "OK";
					dataRow2["U_SoNo"] = dataRow["U_SoNo"];
					dataRow2["U_SoLine"] = dataRow["U_SoLine"];
					if ("C".Equals(value) && "C".Equals(value2))
					{
						dataRow2["U_Width"] = num9;
						dataRow2["U_Length"] = num10;
						double num12 = Math.Floor(num5 / num9) * Math.Floor(num6 / num10);
						double num13 = ((num12 >= num11) ? num11 : num12);
						dataRow2["U_QtyUoM"] = num13;
						dataRow2["U_UnitUoM"] = value2;
						dataRow2["U_Quantity"] = num9 * num10 * num13 / 100.0;
						double num14 = ((num2 > num10) ? Math.Floor(num2 / num10) : 1.0);
						num5 = (double)((decimal)num5 - (decimal)(num9 * num13 / num14));
						num12 = Math.Floor(num5 / num);
						num7 = ((num12 > 0.0) ? num12 : 1.0);
					}
					else if ("C".Equals(value) && "T".Equals(value2))
					{
						if (num5 >= num10 * 100.0 && num5 % (num10 * 100.0) == 0.0)
						{
							double num15 = num9;
							num9 = num10 * 100.0;
							num10 = (double)(decimal)(num15 / 100.0);
						}
						dataRow2["U_Width"] = num9;
						dataRow2["U_Length"] = num10;
						double num16 = Math.Floor(num5 / num9 * num3) * Math.Floor(num6 / num10);
						double num17 = ((num16 >= num11) ? num11 : num16);
						dataRow2["U_QtyUoM"] = num17;
						dataRow2["U_UnitUoM"] = value2;
						dataRow2["U_Quantity"] = num9 * num10 * (double)((decimal)num17 / 100m);
						num6 = Math.Floor(Math.Floor((num16 - num11) * num10) / Math.Floor(num5 / num9));
						if (num6 > num2 * Math.Floor(num / num9))
						{
							num6 = (num16 - num11) * num10 / Math.Floor(num5 / num9);
						}
						num16 = Math.Floor(num5 / num);
						num7 = ((num16 > 0.0) ? num16 : 1.0);
					}
					if (!string.IsNullOrEmpty(Function.ToString(dataRow2["U_Width"])))
					{
						dataSourceTable.Rows.Add(dataRow2);
					}
				}
			}
			if (num5 > 0.0 && num6 > 1.0 && num7 > 0.0)
			{
				if (num6 > num2)
				{
					double num18 = num6 - num2 * Math.Floor(num6 / num2) * Math.Floor(num5 / num);
					DataRow dataRow3 = dataSourceTable.NewRow();
					dataRow3["U_ItemCode"] = row["U_ItemCode"];
					dataRow3["U_Width"] = num5;
					dataRow3["U_Length"] = Math.Floor(num18);
					dataRow3["U_QtyUoM"] = 1;
					dataRow3["U_UnitUoM"] = value;
					dataRow3["U_Quantity"] = num5 * (double)((decimal)num18 / 100m);
					dataRow3["U_PlanId"] = num4;
					dataRow3["U_WhsCode"] = GlobalData.UserDefaultGroup.Warehouse;
					dataRow3["U_DelStatus"] = "N";
					dataRow3["U_StatusQC"] = "OK";
					dataSourceTable.Rows.Add(dataRow3);
					num7 = (int)(num6 / num2);
					num6 = (num6 - num18) / num7;
				}
				DataRow dataRow4 = dataSourceTable.NewRow();
				dataRow4["U_ItemCode"] = row["U_ItemCode"];
				dataRow4["U_Width"] = num5;
				dataRow4["U_Length"] = Math.Floor(num6) - 1.0;
				dataRow4["U_QtyUoM"] = num7;
				dataRow4["U_UnitUoM"] = value;
				dataRow4["U_Quantity"] = num5 * num6 * (double)((decimal)num7 / 100m);
				dataRow4["U_PlanId"] = num4;
				dataRow4["U_WhsCode"] = GlobalData.UserDefaultGroup.Warehouse;
				dataRow4["U_DelStatus"] = "N";
				dataRow4["U_StatusQC"] = "OK";
				dataSourceTable.Rows.Add(dataRow4);
			}
			dataSourceTable.AcceptChanges();
			grd_Operation.RefreshDataSource();
			ReLoadLossQty(dataSourceTable, num4);
			ReloadPlannedRequest();
			FormValueChanged();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected override void NoAuthSetControlSearch()
	{
		txt_BatchNo.ReadOnly = false;
		txt_DocNo.ReadOnly = false;
		txt_Pono.ReadOnly = false;
		txt_SoNo.ReadOnly = false;
	}

	private async Task<string> OperatorDoc(DataSet ds, string command)
	{
		try
		{
			using HttpClientHelper<DataSet> _help = new HttpClientHelper<DataSet>(Configuration.ApiServer, "UDO");
			return await _help.PostAsJsonAsync(command, ds);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return "";
	}

	private async void menuMainCancel_Click(object sender, EventArgs e)
	{
		try
		{
			if (string.IsNullOrEmpty(txt_DocNo.Text) || Function.ParseInt(txt_DocNo.EditValue) <= 0)
			{
				return;
			}
			int num = Function.ParseInt(txt_DocNo.EditValue);
			DataRow[] array = tv_ProductionPlanning.DataSourceTable().Select("U_PwNo > 0 AND PwStatus <> 'C'");
			if (array != null && array.Length != 0)
			{
				XtraMessageBox.Show("Không thể hủy khi có lệnh sản xuất!", "System", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
			}
			else if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCancel, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				DataSet ds = new DataSet();
				ds.Tables.Clear();
				ds.Tables.Add(UdoTable);
				DataTable dataTable = new DataTable();
				dataTable.Columns.Add("DocEntry", typeof(int));
				DataRow dataRow = dataTable.NewRow();
				dataRow["DocEntry"] = num;
				dataTable.Rows.Add(dataRow);
				ds.Tables.Add(dataTable);
				string text = await OperatorDoc(ds, "CancelDoc");
				ds.Tables.Clear();
				ds.Dispose();
				if (string.IsNullOrEmpty(text))
				{
					txt_DocNo.Text = "";
					txt_Remark.Text = "";
					txt_StockRequest.Text = "";
					grd_ProductionRequest.DataSourceTable.Rows.Clear();
					tv_ProductionPlanning.DataSourceTable().Rows.Clear();
					grd_Operation.DataSourceTable.Rows.Clear();
					WinCommons.writeStatus(Enums.MessageCode.CancelSuccess, Enums.MessageType.Success, Msg_Time._TimeMedium, "");
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, text);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void menuMainClose_Click(object sender, EventArgs e)
	{
		try
		{
			if (string.IsNullOrEmpty(txt_DocNo.Text) || Function.ParseInt(txt_DocNo.EditValue) <= 0)
			{
				return;
			}
			int num = Function.ParseInt(txt_DocNo.EditValue);
			DataRow[] array = tv_ProductionPlanning.DataSourceTable().Select("U_PwNo > 0 AND PwStatus <> 'L' AND PwStatus <> 'C'");
			if (array != null && array.Length != 0)
			{
				XtraMessageBox.Show("Không thể đóng khi có lệnh sản xuất chưa hoàn thành!", "System", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
			}
			else if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmClose, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				DataSet ds = new DataSet();
				ds.Tables.Clear();
				ds.Tables.Add(UdoTable);
				DataTable dataTable = new DataTable();
				dataTable.Columns.Add("DocEntry", typeof(int));
				DataRow dataRow = dataTable.NewRow();
				dataRow["DocEntry"] = num;
				dataTable.Rows.Add(dataRow);
				ds.Tables.Add(dataTable);
				string text = await OperatorDoc(ds, "CloseDoc");
				ds.Tables.Clear();
				ds.Dispose();
				if (string.IsNullOrEmpty(text))
				{
					txt_DocNo.Text = "";
					txt_Remark.Text = "";
					txt_StockRequest.Text = "";
					grd_ProductionRequest.DataSourceTable.Rows.Clear();
					tv_ProductionPlanning.DataSourceTable().Rows.Clear();
					grd_Operation.DataSourceTable.Rows.Clear();
					WinCommons.writeStatus(Enums.MessageCode.CloseSuccess, Enums.MessageType.Success, Msg_Time._TimeMedium, "");
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, text);
				}
			}
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
			if (e.KeyCode == Keys.Return && !string.IsNullOrEmpty(txt_DocNo.Text) && btn_OK.Text.Equals(Resources.ButtonFind))
			{
				int type = 5;
				int num = Function.ParseInt(txt_DocNo.EditValue);
				if (num > 0)
				{
					await Navigate(num, type, GlobalData.UserName);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void txt_Pono_KeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.Return && !string.IsNullOrEmpty(txt_Pono.Text) && btn_OK.Text.Equals(Resources.ButtonFind))
			{
				int type = 6;
				int num = Function.ParseInt(txt_Pono.EditValue);
				if (num > 0)
				{
					await Navigate(num, type, GlobalData.UserName);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void txt_BatchNo_KeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.Return && !string.IsNullOrEmpty(txt_BatchNo.Text) && btn_OK.Text.Equals(Resources.ButtonFind))
			{
				int type = 7;
				string text = Function.ToString(txt_BatchNo.EditValue);
				if (!string.IsNullOrEmpty(text))
				{
					await Navigate(0, type, text);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void txt_SoNo_KeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.Return && !string.IsNullOrEmpty(txt_SoNo.Text) && btn_OK.Text.Equals(Resources.ButtonFind))
			{
				int type = 8;
				int num = Function.ParseInt(txt_SoNo.EditValue);
				if (num > 0)
				{
					await Navigate(num, type, GlobalData.UserName);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_ProductionRequest_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
	{
		try
		{
			FormValueChanged();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_Request_RemoveRow_Click(object sender, EventArgs e)
	{
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDeleteThisRow, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				gv_ProductionRequest.DeleteSelectedRows();
				grd_ProductionRequest.DataSourceTable.AcceptChanges();
				FormValueChanged();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ReloadPlannedRequest()
	{
		DataTable dataSourceTable = grd_ProductionRequest.DataSourceTable;
		DataTable dataSourceTable2 = grd_Operation.DataSourceTable;
		if (!dataSourceTable.IsNotNull())
		{
			return;
		}
		foreach (DataRow row in dataSourceTable.Rows)
		{
			double num = Function.ParseDouble(row["U_Width"]);
			double num2 = Function.ParseDouble(row["U_Length"]);
			double num3 = Function.ParseDouble(row["U_QtyUoM"]);
			string text = Function.ToString(row["U_UnitUoM"]);
			int num4 = Function.ParseInt(row["U_SoNo"]);
			int num5 = Function.ParseInt(row["U_SoLine"]);
			string filterExpression = $"U_SoNo='{num4}' AND U_SoLine='{num5}' AND U_Width='{num}'";
			if (text.ToUpper().Equals("T"))
			{
				filterExpression = $"U_SoNo={num4} AND U_SoLine={num5} AND (U_Width={num} AND U_Length={num2}) OR (U_Width={(decimal)(num2 * 100.0)} AND U_Length={(decimal)(num / 100.0)})";
			}
			DataRow[] array = dataSourceTable2.Select(filterExpression);
			if (array.Length != 0)
			{
				double num6 = array.Sum((DataRow t) => Function.ParseDouble(t["U_QtyUoM"]));
				row["U_Planned"] = num6;
				row["U_Remain"] = num3 - num6;
			}
			else
			{
				row["U_Planned"] = 0;
				row["U_Remain"] = num3;
			}
			dataSourceTable.AcceptChanges();
		}
	}

	private void ctx_Planning_Opening(object sender, CancelEventArgs e)
	{
		try
		{
			string text = Function.ToString(tv_ProductionPlanning.FocusedNode.GetValue("PwStatus"));
			if (!string.IsNullOrEmpty(text) && (text.ToUpper().Equals("L") || text.ToUpper().Equals("R")))
			{
				removeRowsToolStripMenuItem.Visible = false;
			}
			else
			{
				removeRowsToolStripMenuItem.Visible = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menu_plan_RemoveRow_Click(object sender, EventArgs e)
	{
		try
		{
			if (tv_ProductionPlanning.FocusedNode == null)
			{
				return;
			}
			string text = Function.ToString(tv_ProductionPlanning.FocusedNode.GetValue("PwStatus"));
			if (!string.IsNullOrEmpty(text) && (text.ToUpper().Equals("L") || text.ToUpper().Equals("R")))
			{
				WinCommons.Error(Enums.MessageCode.None, "Không thể xóa lệnh sản xuất đã thực hiện!");
			}
			else if (!string.IsNullOrEmpty(text) && text.ToUpper().Equals("P"))
			{
				WinCommons.Error(Enums.MessageCode.None, "Không thể xóa lệnh sản xuất chưa hủy!");
			}
			else
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDeleteThisRow, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, DefaultBoolean.True) != DialogResult.Yes)
				{
					return;
				}
				DataRow row = ((DataRowView)tv_ProductionPlanning.GetDataRecordByNode(tv_ProductionPlanning.FocusedNode)).Row;
				if (row != null)
				{
					DataTable dataSourceTable = grd_Operation.DataSourceTable;
					int num = Function.ParseInt(row["U_ID"]);
					DataRow[] array = dataSourceTable.Select($"U_PlanId='{num}'");
					if (array != null && array.Length != 0)
					{
						DataRow[] array2 = array;
						foreach (DataRow row2 in array2)
						{
							dataSourceTable.Rows.Remove(row2);
						}
					}
					string text2 = Function.ToString(row["U_BatchNo"]);
					if (!string.IsNullOrEmpty(text2))
					{
						foreach (DataRow row3 in grd_ProductionRequest.DataSourceTable.Rows)
						{
							row3["U_BatchNo"] = Function.ToString(row3["U_BatchNo"]).Replace(text2, "");
						}
						if (dtSelectedBatch.IsNotNull() && tre_DSCuon.DataSourceTable().IsNotNull())
						{
							string[] array3 = text2.Split(';');
							string text3 = "U_BatchNo IN (";
							for (int j = 0; j < array3.Length; j++)
							{
								text3 += $"'{array3[j]}',";
							}
							text3 = ((!string.IsNullOrEmpty(text3)) ? (text3.Remove(text3.Length - 1, 1) + ")") : "");
							DataRow[] array4 = dtSelectedBatch.Select(text3);
							DataTable dataTable = tre_DSCuon.DataSourceTable();
							dataTable.AcceptChanges();
							DataRow[] array2 = array4;
							foreach (DataRow dataRow2 in array2)
							{
								DataRow dataRow3 = dataTable.NewRow();
								WinFuntions.DataRowToDataRow(dataRow2, dataRow3);
								dataRow3["Checked"] = false;
								dataTable.Rows.Add(dataRow3);
								dtSelectedBatch.Rows.Remove(dataRow2);
							}
							dtSelectedBatch.AcceptChanges();
						}
					}
					FormValueChanged();
					tv_ProductionPlanning.DeleteNode(tv_ProductionPlanning.FocusedNode);
					tv_ProductionPlanning.DataSourceTable().AcceptChanges();
					grd_ProductionRequest.DataSourceTable.AcceptChanges();
				}
				ReloadPlannedRequest();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void tv_ProductionPlanning_PopupMenuShowing(object sender, DevExpress.XtraTreeList.PopupMenuShowingEventArgs e)
	{
		try
		{
			pasteOperationToolStripMenuItem.Enabled = DtCopyOperation.IsNotNull();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void tv_ProductionPlanning_CellValueChanged(object sender, DevExpress.XtraTreeList.CellValueChangedEventArgs e)
	{
		try
		{
			FormValueChanged();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void tv_ProductionPlanning_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			string text = Function.ToString(tv_ProductionPlanning.FocusedNode.GetValue("PwStatus"));
			if (!string.IsNullOrEmpty(text) && (text.ToUpper().Equals("L") || text.ToUpper().Equals("R")))
			{
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ReLoadLossQty(DataTable dtOperation, int planId, double extraQty = 0.0)
	{
		DataRow[] array = dtOperation.Select("U_PlanId=" + planId);
		if (array.Length != 0)
		{
			double num = array.Sum((DataRow t) => Function.ParseDouble(t["U_Quantity"]));
			double num2 = Math.Round(Function.ParseDouble(tv_ProductionPlanning.FocusedNode.GetValue("U_Quantity")) - num - extraQty, 3, MidpointRounding.AwayFromZero);
			tv_ProductionPlanning.FocusedNode.SetValue("U_LossQty", num2);
		}
	}

	private async void menu_plan_CancelPO_Click(object sender, EventArgs e)
	{
		try
		{
			if (tv_ProductionPlanning.FocusedNode == null)
			{
				return;
			}
			string text = Function.ToString(tv_ProductionPlanning.FocusedNode.GetValue("PwStatus"));
			if (!string.IsNullOrEmpty(text) && (text.ToUpper().Equals("L") || text.ToUpper().Equals("R")))
			{
				WinCommons.Error(Enums.MessageCode.None, "Không thể hủy lệnh sản xuất đã thực hiện");
			}
			else
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCancelProductionOrder, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, DefaultBoolean.False) != DialogResult.Yes)
				{
					return;
				}
				WinCommons._OpenProcessing();
				DataRow row = ((DataRowView)tv_ProductionPlanning.GetDataRecordByNode(tv_ProductionPlanning.FocusedNode)).Row;
				if (row != null)
				{
					int num = Function.ParseInt(row["U_PwNo"]);
					if (num > 0)
					{
						string text2 = await UpdateStatusPO(num, "C");
						if (!string.IsNullOrEmpty(text2))
						{
							WinCommons.Error(Enums.MessageCode.None, text2);
						}
						else
						{
							tv_ProductionPlanning.FocusedNode.SetValue("PwStatus", "C");
							btn_OK.Text = Resources.ButtonUpdate;
							txt_DocNo.ReadOnly = true;
							WinCommons.writeStatus(Enums.MessageCode.SaveSuccess, Enums.MessageType.Success, Msg_Time._TimeMedium, "");
						}
					}
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

	private async void menu_plan_UnReleasePO_Click(object sender, EventArgs e)
	{
		try
		{
			if (tv_ProductionPlanning.FocusedNode == null)
			{
				return;
			}
			string text = Function.ToString(tv_ProductionPlanning.FocusedNode.GetValue("PwStatus"));
			if ((!string.IsNullOrEmpty(text) && !text.ToUpper().Equals("R")) || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUnReleaseProductionOrder, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, DefaultBoolean.False) != DialogResult.Yes)
			{
				return;
			}
			WinCommons._OpenProcessing();
			DataRow row = ((DataRowView)tv_ProductionPlanning.GetDataRecordByNode(tv_ProductionPlanning.FocusedNode)).Row;
			if (row != null)
			{
				int num = Function.ParseInt(row["U_PwNo"]);
				if (num > 0)
				{
					string text2 = await UpdateStatusPO(num, "P");
					if (!string.IsNullOrEmpty(text2))
					{
						WinCommons.Error(Enums.MessageCode.None, text2);
					}
					else
					{
						WinCommons.writeStatus(Enums.MessageCode.SaveSuccess, Enums.MessageType.Success, Msg_Time._TimeMedium, "");
						tv_ProductionPlanning.FocusedNode.SetValue("PwStatus", "P");
					}
					btn_OK.Text = Resources.ButtonUpdate;
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

	private void gv_Operation_InitNewRow(object sender, InitNewRowEventArgs e)
	{
		try
		{
			int rowHandle = e.RowHandle;
			gv_Operation.SetRowCellValue(rowHandle, "U_WhsCode", GlobalData.UserDefaultGroup.Warehouse);
			gv_Operation.SetRowCellValue(rowHandle, "U_ItemCode", Function.ToString(gv_Operation.GetRowCellValue(rowHandle - 1, "U_ItemCode")));
			gv_Operation.SetRowCellValue(rowHandle, "U_DelStatus", "N");
			gv_Operation.SetRowCellValue(rowHandle, "U_StatusQC", "OK");
			gv_Operation.SetRowCellValue(rowHandle, "U_UnitUoM", "C");
			int num = Function.ParseInt(tv_ProductionPlanning.FocusedNode.GetValue("U_ID"));
			gv_Operation.SetRowCellValue(rowHandle, "U_PlanId", num);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Operation_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
	{
		try
		{
			if (!userEdit)
			{
				return;
			}
			userEdit = false;
			FormValueChanged();
			int rowHandle = e.RowHandle;
			string text = e.Column.FieldName.ToUpper();
			DataTable dataSourceTable = grd_Operation.DataSourceTable;
			if (!e.Column.FieldName.ToUpper().Equals("U_PLANID"))
			{
				int num = Function.ParseInt(tv_ProductionPlanning.FocusedNode.GetValue("U_ID"));
				gv_Operation.SetRowCellValue(rowHandle, "U_PlanId", num);
			}
			DataRowView dataRowView = (DataRowView)tv_ProductionPlanning.GetDataRecordByNode(tv_ProductionPlanning.FocusedNode);
			double num2 = Function.ParseDouble(dataRowView.Row["U_Width"]);
			double num3 = Function.ParseDouble(dataRowView.Row["U_Length"]);
			double num4 = Function.ParseDouble(dataRowView.Row["U_QtyUoM"]);
			Function.ToString(dataRowView.Row["U_UnitUoM"]);
			Function.ParseDouble(dataRowView["U_Quantity"]);
			string text2 = Function.ToString(gv_Operation.GetRowCellValue(rowHandle, "U_UnitUoM"));
			double num5 = Function.ParseDouble(gv_Operation.GetRowCellValue(rowHandle, "U_Length"));
			double num6 = Function.ParseDouble(gv_Operation.GetRowCellValue(rowHandle, "U_Width"));
			double num7 = Function.ParseDouble(gv_Operation.GetRowCellValue(rowHandle, "U_QtyUoM"));
			int planId = Function.ParseInt(gv_Operation.GetRowCellValue(rowHandle, "U_PlanId"));
			double num8 = num5 * num6 * num7 / 100.0;
			if (text.Equals("U_WIDTH") || text.Equals("U_LENGTH") || text.Equals("U_QTYUOM") || text.Equals("U_SONO") || text.Equals("U_SOLINE"))
			{
				DataRow[] array = dataSourceTable.Select("U_PlanId=" + Function.ParseInt(gv_Operation.GetRowCellValue(rowHandle, "U_PlanId")));
				if ((text2.ToUpper().Equals("T") || num5 < 2.0) && text.Equals("U_LENGTH"))
				{
					double num9 = (double)(int)(Math.Floor(num2 / num6) * num3 / num5) * num4;
					if (array.Length != 0)
					{
						gv_Operation.SetRowCellValue(rowHandle, "U_QtyUoM", num9);
						gv_Operation.SetRowCellValue(rowHandle, "U_Quantity", Math.Round(num5 * num6 * num9 / 100.0, 3, MidpointRounding.AwayFromZero));
					}
				}
				else if (array.Length != 0)
				{
					gv_Operation.SetRowCellValue(rowHandle, "U_Quantity", num8);
				}
				ReloadPlannedRequest();
			}
			if ((text.Equals("U_WIDTH") || text.Equals("U_LENGTH") || text.Equals("U_QTYUOM")) && num6 > 0.0 && num5 > 0.0 && num7 > 0.0)
			{
				if (rowHandle < 0)
				{
					double extraQty = num6 * num5 * num7 / 100.0;
					ReLoadLossQty(dataSourceTable, planId, extraQty);
				}
				else
				{
					ReLoadLossQty(dataSourceTable, planId);
				}
			}
			if (text.Equals("U_LENGTH"))
			{
				if (num5 < 2.0)
				{
					gv_Operation.SetRowCellValue(rowHandle, "U_UnitUoM", "T");
				}
				else
				{
					gv_Operation.SetRowCellValue(rowHandle, "U_UnitUoM", "C");
				}
			}
			if (text.Equals("U_QTYUOM"))
			{
				if ("C".Equals(text2))
				{
					gv_Operation.SetRowCellValue(rowHandle, "U_PackQty", 1);
				}
				else
				{
					gv_Operation.SetRowCellValue(rowHandle, "U_PackQty", num7);
				}
			}
			if (text.Equals("U_DELSTATUS"))
			{
				string text3 = Function.ToString(gv_Operation.GetRowCellValue(rowHandle, "U_DelStatus"));
				int num10 = Function.ParseInt(tv_ProductionPlanning.FocusedNode.GetValue("U_ID"));
				if (!string.IsNullOrEmpty(text3) && text3.Equals("P"))
				{
					DataTable dataTable = tv_ProductionPlanning.DataSourceTable();
					int num11 = ((!dataTable.AsEnumerable().Any()) ? 1 : (dataTable.AsEnumerable().Max((DataRow x) => Function.ParseInt(x["U_ID"])) + 1));
					DataRow dataRow = dataTable.NewRow();
					DataRow dataRow2 = gv_Operation.GetDataRow(rowHandle);
					for (int num12 = 0; num12 < dataRow2.ItemArray.Length; num12++)
					{
						if (dataRow.IsExistsColumn(dataRow2.Table.Columns[num12].ColumnName))
						{
							dataRow[dataRow2.Table.Columns[num12].ColumnName] = dataRow2[dataRow2.Table.Columns[num12].ColumnName];
						}
					}
					gv_Operation.SetRowCellValue(rowHandle, "U_ID", num11);
					dataRow["U_ID"] = num11;
					dataRow["U_FatherId"] = num10;
					dataRow["U_WhsCode"] = GlobalData.UserDefaultGroup.Warehouse;
					dataRow["U_DueDate"] = DateTime.Today.AddDays(1.0);
					dataTable.Rows.Add(dataRow);
					dataTable.AcceptChanges();
					dataRow2["U_WhsCode"] = GlobalData.UserDefaultGroup.Warehouse;
					dataRow2["U_SoNo"] = 0;
					dataRow2["U_SoLine"] = 0;
				}
				else if (!string.IsNullOrEmpty(text3) && text3.Equals("Y"))
				{
					DataTable dataSourceTable2 = grd_ProductionRequest.DataSourceTable;
					if ("C".Equals(text2))
					{
						DataRow dataRow3 = dataSourceTable2.Select($"U_UnitUoM = '{text2}' AND U_Width = {num6} AND U_Remain > 0").FirstOrDefault();
						if (dataRow3 != null)
						{
							DataRow dataRow4 = gv_Operation.GetDataRow(rowHandle);
							dataRow4["U_SoNo"] = dataRow3["U_SoNo"];
							dataRow4["U_SoLine"] = dataRow3["U_SoLine"];
							dataRow4["U_WhsCode"] = GlobalData.UserDefaultGroup.Warehouse;
						}
					}
					else if ("T".Equals(text2))
					{
						DataRow dataRow5 = dataSourceTable2.Select($"U_UnitUoM = '{text2}' AND (U_Width = {num6} OR U_Width = {num5 * 100.0}) AND U_Remain > 0").FirstOrDefault();
						if (dataRow5 != null)
						{
							DataRow dataRow6 = gv_Operation.GetDataRow(rowHandle);
							dataRow6["U_SoNo"] = dataRow5["U_SoNo"];
							dataRow6["U_SoLine"] = dataRow5["U_SoLine"];
							dataRow6["U_WhsCode"] = GlobalData.UserDefaultGroup.Warehouse;
						}
					}
				}
			}
			userEdit = true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Operation_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (tv_ProductionPlanning.FocusedNode != null)
			{
				string text = Function.ToString(tv_ProductionPlanning.FocusedNode.GetValue("PwStatus"));
				if (!string.IsNullOrEmpty(text) && (text.ToUpper().Equals("L") || text.ToUpper().Equals("R")))
				{
					e.Cancel = true;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ctm_Operation_Opening(object sender, CancelEventArgs e)
	{
		try
		{
			string text = Function.ToString(tv_ProductionPlanning.FocusedNode.GetValue("PwStatus"));
			if (!string.IsNullOrEmpty(text) && (text.ToUpper().Equals("L") || text.ToUpper().Equals("R")))
			{
				removeRowToolStripMenuItem.Visible = false;
			}
			else
			{
				removeRowToolStripMenuItem.Visible = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void copyOperationToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			DtCopyOperation = grd_Operation.DataSourceTable.Copy();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void pasteOperationToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (Function.ParseInt(tv_ProductionPlanning.FocusedNode.GetValue("U_PwNo")) > 0 || !DtCopyOperation.IsNotNull())
			{
				return;
			}
			DataTable dataSourceTable = grd_Operation.DataSourceTable;
			foreach (TreeListNode item in tv_ProductionPlanning.Selection)
			{
				int id = Function.ParseInt(item.GetValue("U_ID"));
				foreach (DataRow item2 in from t in dataSourceTable.AsEnumerable()
					where Function.ParseInt(t["U_PlanId"]) == id
					select t)
				{
					dataSourceTable.Rows.Remove(item2);
				}
				for (int num = 0; num < DtCopyOperation.Rows.Count; num++)
				{
					DataRow dataRow = dataSourceTable.NewRow();
					dataRow.ItemArray = DtCopyOperation.Rows[num].ItemArray;
					dataRow["U_PlanId"] = id;
					dataSourceTable.Rows.Add(dataRow);
				}
				ReLoadLossQty(dataSourceTable, id);
			}
			dataSourceTable.AcceptChanges();
			ReloadPlannedRequest();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void removeRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			try
			{
				string text = Function.ToString(tv_ProductionPlanning.FocusedNode.GetValue("PwStatus"));
				if (!string.IsNullOrEmpty(text) && (text.ToUpper().Equals("L") || text.ToUpper().Equals("R")))
				{
					WinCommons.Error(Enums.MessageCode.None, "Không thể xóa lệnh sản xuất đã thực hiện");
				}
				else if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDeleteThisRow, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					int planId = Function.ParseInt(gv_Operation.GetFocusedRowCellValue("U_PlanId"));
					gv_Operation.DeleteSelectedRows();
					grd_Operation.DataSourceTable.AcceptChanges();
					ReLoadLossQty(grd_Operation.DataSourceTable, planId);
					ReloadPlannedRequest();
				}
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
				WinCommons.ErrorLineNumber(ex);
			}
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
			WinCommons.ErrorLineNumber(ex2);
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

	private string CreateAutoBatchTH(string issBatch, int suffix)
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
			return $"{text}.{suffix}";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return "";
	}

	public async Task PRINTLAYOUT()
	{
		try
		{
			int num = Function.ParseInt(txt_DocNo.EditValue);
			if (num > 0)
			{
				await WinFuntions.PrintLayout("", num, "OPWB");
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
			int num = Function.ParseInt(txt_DocNo.EditValue);
			if (num > 0)
			{
				await WinFuntions.Print("", num, "OPWB");
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.MTO_Manufacturing.frm_ProductionForBatch));
		this.look_Status = new DevExpress.XtraEditors.LookUpEdit();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.txt_Remark = new DevExpress.XtraEditors.TextEdit();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.txt_DocNo = new DevExpress.XtraEditors.TextEdit();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.tab_Main = new Apzon.Controls.Controls.XtraTabControlEx();
		this.xtraTabPage1 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.btn_ConfirmProduction = new DevExpress.XtraEditors.SimpleButton();
		this.btn_AutoXa = new DevExpress.XtraEditors.SimpleButton();
		this.btn_AutoCalc = new DevExpress.XtraEditors.SimpleButton();
		this.btn_GenerateProductionOrder = new DevExpress.XtraEditors.SimpleButton();
		this.btn_OK = new DevExpress.XtraEditors.SimpleButton();
		this.split_Main = new DevExpress.XtraEditors.SplitContainerControl();
		this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
		this.grd_ProductionRequest = new Apzon.Controls.Grid.GridControlEx();
		this.ctx_Request = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.removeRowsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_ProductionRequest = new Apzon.Controls.Grid.GridViewEx();
		this.tabControl_Planning = new Apzon.Controls.Controls.XtraTabControlEx();
		this.xtraTabPage2 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
		this.chb_StatusQC = new DevExpress.XtraEditors.CheckedComboBoxEdit();
		this.lb_BusinessPartner = new Apzon.Controls.Controls.LabelLine();
		this.btn_ChooseBatch = new DevExpress.XtraEditors.SimpleButton();
		this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
		this.tre_DSCuon = new Apzon.Controls.Tree.TreeEx();
		this.btn_TinhGiay = new DevExpress.XtraEditors.SimpleButton();
		this.tab_Planning = new Apzon.Controls.Controls.XtraTabPageEx();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.tv_ProductionPlanning = new Apzon.Controls.Tree.TreeEx();
		this.ctx_Planning = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.removeRowsToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.menu_plan_UnReleasePO = new System.Windows.Forms.ToolStripMenuItem();
		this.menu_plan_CancelPO = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
		this.copyOperationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.pasteOperationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.rep_PlanWhs = new Apzon.Forms.Interfaces.ChooseFromList.RepositoryItem.RepCflWarehouse();
		this.grd_Operation = new Apzon.Controls.Grid.GridControlEx();
		this.ctm_Operation = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.removeRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Operation = new Apzon.Controls.Grid.GridViewEx();
		this.rep_Whs = new Apzon.Forms.Interfaces.ChooseFromList.RepositoryItem.RepCflWarehouse();
		this.tabPage_Planned = new Apzon.Controls.Controls.XtraTabPageEx();
		this.btn_Release = new DevExpress.XtraEditors.SimpleButton();
		this.grd_Planned = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Planned = new Apzon.Controls.Grid.GridViewEx();
		this.tabPage_Released = new Apzon.Controls.Controls.XtraTabPageEx();
		this.btn_Close = new DevExpress.XtraEditors.SimpleButton();
		this.grd_Released = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Released = new Apzon.Controls.Grid.GridViewEx();
		this.tabPage_Holding = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_Holding = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Holding = new Apzon.Controls.Grid.GridViewEx();
		this.tabPage_Closed = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_Closed = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Closed = new Apzon.Controls.Grid.GridViewEx();
		this.tabPage_Canceled = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_Canceled = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Canceled = new Apzon.Controls.Grid.GridViewEx();
		this.ctx_Main = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.menuMainClose = new System.Windows.Forms.ToolStripMenuItem();
		this.menuMainCancel = new System.Windows.Forms.ToolStripMenuItem();
		this.txt_StockRequest = new DevExpress.XtraEditors.TextEdit();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.txt_Creator = new DevExpress.XtraEditors.TextEdit();
		this.labelLine5 = new Apzon.Controls.Controls.LabelLine();
		this.txt_Pono = new DevExpress.XtraEditors.TextEdit();
		this.labelLine6 = new Apzon.Controls.Controls.LabelLine();
		this.txt_BatchNo = new DevExpress.XtraEditors.TextEdit();
		this.labelLine7 = new Apzon.Controls.Controls.LabelLine();
		this.txt_SoNo = new DevExpress.XtraEditors.TextEdit();
		this.labelLine8 = new Apzon.Controls.Controls.LabelLine();
		((System.ComponentModel.ISupportInitialize)this.look_Status.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Remark.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_DocNo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tab_Main).BeginInit();
		this.tab_Main.SuspendLayout();
		this.xtraTabPage1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.split_Main).BeginInit();
		this.split_Main.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.groupControl1).BeginInit();
		this.groupControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_ProductionRequest).BeginInit();
		this.ctx_Request.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_ProductionRequest).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tabControl_Planning).BeginInit();
		this.tabControl_Planning.SuspendLayout();
		this.xtraTabPage2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.panelControl1).BeginInit();
		this.panelControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.chb_StatusQC.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl2).BeginInit();
		this.panelControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.tre_DSCuon).BeginInit();
		this.tab_Planning.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.tv_ProductionPlanning).BeginInit();
		this.ctx_Planning.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.rep_PlanWhs).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Operation).BeginInit();
		this.ctm_Operation.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Operation).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Whs).BeginInit();
		this.tabPage_Planned.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Planned).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Planned).BeginInit();
		this.tabPage_Released.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Released).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Released).BeginInit();
		this.tabPage_Holding.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Holding).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Holding).BeginInit();
		this.tabPage_Closed.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Closed).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Closed).BeginInit();
		this.tabPage_Canceled.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Canceled).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Canceled).BeginInit();
		this.ctx_Main.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txt_StockRequest.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Creator.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Pono.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BatchNo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_SoNo.Properties).BeginInit();
		base.SuspendLayout();
		this.look_Status.Location = new System.Drawing.Point(529, 1);
		this.look_Status.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.look_Status.Name = "look_Status";
		this.look_Status.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.look_Status.Properties.NullText = "";
		this.look_Status.Size = new System.Drawing.Size(70, 20);
		this.look_Status.TabIndex = 13;
		this.look_Status.ToolTip = "Trạng thái phiếu tính giấy";
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "Status";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(491, 4);
		this.labelLine3.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(47, 21);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 12;
		this.labelLine3.Utext = "Status";
		this.txt_Remark.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.txt_Remark.Location = new System.Drawing.Point(918, 1);
		this.txt_Remark.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.txt_Remark.Name = "txt_Remark";
		this.txt_Remark.Size = new System.Drawing.Size(144, 20);
		this.txt_Remark.TabIndex = 11;
		this.txt_Remark.ToolTip = "Ghi chú tìm hàng";
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "Ghi chú tìm hàng";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(831, 4);
		this.labelLine2.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(86, 21);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 10;
		this.labelLine2.Utext = "Ghi chú tìm hàng";
		this.txt_DocNo.Location = new System.Drawing.Point(48, 1);
		this.txt_DocNo.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.txt_DocNo.Name = "txt_DocNo";
		this.txt_DocNo.Properties.DisplayFormat.FormatString = "n0";
		this.txt_DocNo.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.txt_DocNo.Properties.EditFormat.FormatString = "n0";
		this.txt_DocNo.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.txt_DocNo.Properties.Mask.EditMask = "n0";
		this.txt_DocNo.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
		this.txt_DocNo.Properties.ReadOnly = true;
		this.txt_DocNo.Size = new System.Drawing.Size(60, 20);
		this.txt_DocNo.TabIndex = 9;
		this.txt_DocNo.ToolTip = "Số phiếu tính giấy";
		this.txt_DocNo.EditValueChanged += new System.EventHandler(this.txt_DocNo_EditValueChanged);
		this.txt_DocNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_DocNo_KeyDown);
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Doc No";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(4, 4);
		this.labelLine1.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(47, 21);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 8;
		this.labelLine1.Utext = "Doc No";
		this.tab_Main.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.tab_Main.IsSearchControl = false;
		this.tab_Main.Location = new System.Drawing.Point(0, 32);
		this.tab_Main.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.tab_Main.Name = "tab_Main";
		this.tab_Main.SelectedTabPage = this.xtraTabPage1;
		this.tab_Main.Size = new System.Drawing.Size(1066, 659);
		this.tab_Main.TabIndex = 14;
		this.tab_Main.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[6] { this.xtraTabPage1, this.tabPage_Planned, this.tabPage_Released, this.tabPage_Holding, this.tabPage_Closed, this.tabPage_Canceled });
		this.xtraTabPage1.Controls.Add(this.btn_ConfirmProduction);
		this.xtraTabPage1.Controls.Add(this.btn_AutoXa);
		this.xtraTabPage1.Controls.Add(this.btn_AutoCalc);
		this.xtraTabPage1.Controls.Add(this.btn_GenerateProductionOrder);
		this.xtraTabPage1.Controls.Add(this.btn_OK);
		this.xtraTabPage1.Controls.Add(this.split_Main);
		this.xtraTabPage1.EditControl = null;
		this.xtraTabPage1.EditValue = "Production Planning";
		this.xtraTabPage1.FieldName = "";
		this.xtraTabPage1.IsSearchControl = false;
		this.xtraTabPage1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.xtraTabPage1.Name = "xtraTabPage1";
		this.xtraTabPage1.ReadOnly = false;
		this.xtraTabPage1.SettingInVisible = false;
		this.xtraTabPage1.SettingReadonly = false;
		this.xtraTabPage1.Size = new System.Drawing.Size(1064, 634);
		this.xtraTabPage1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPage1.Text = "Production Planning";
		this.xtraTabPage1.Tooltip = "Lập lệnh sản xuất";
		this.btn_ConfirmProduction.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_ConfirmProduction.Enabled = false;
		this.btn_ConfirmProduction.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_ConfirmProduction.ImageOptions.Image");
		this.btn_ConfirmProduction.Location = new System.Drawing.Point(719, 605);
		this.btn_ConfirmProduction.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.btn_ConfirmProduction.Name = "btn_ConfirmProduction";
		this.btn_ConfirmProduction.Size = new System.Drawing.Size(168, 21);
		this.btn_ConfirmProduction.TabIndex = 9;
		this.btn_ConfirmProduction.Text = "&Confirm Production Order";
		this.btn_ConfirmProduction.Click += new System.EventHandler(this.btn_ConfirmProduction_Click);
		this.btn_AutoXa.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_AutoXa.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_AutoXa.ImageOptions.Image");
		this.btn_AutoXa.Location = new System.Drawing.Point(158, 606);
		this.btn_AutoXa.Name = "btn_AutoXa";
		this.btn_AutoXa.Size = new System.Drawing.Size(75, 21);
		this.btn_AutoXa.TabIndex = 8;
		this.btn_AutoXa.Text = "Auto Xả";
		this.btn_AutoXa.Click += new System.EventHandler(this.btn_AutoXa_Click);
		this.btn_AutoCalc.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_AutoCalc.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_AutoCalc.ImageOptions.Image");
		this.btn_AutoCalc.Location = new System.Drawing.Point(79, 606);
		this.btn_AutoCalc.Name = "btn_AutoCalc";
		this.btn_AutoCalc.Size = new System.Drawing.Size(75, 21);
		this.btn_AutoCalc.TabIndex = 7;
		this.btn_AutoCalc.Text = "Auto Chia";
		this.btn_AutoCalc.Click += new System.EventHandler(this.btn_AutoCalc_Click);
		this.btn_GenerateProductionOrder.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_GenerateProductionOrder.Enabled = false;
		this.btn_GenerateProductionOrder.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_GenerateProductionOrder.ImageOptions.Image");
		this.btn_GenerateProductionOrder.Location = new System.Drawing.Point(891, 605);
		this.btn_GenerateProductionOrder.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.btn_GenerateProductionOrder.Name = "btn_GenerateProductionOrder";
		this.btn_GenerateProductionOrder.Size = new System.Drawing.Size(168, 21);
		this.btn_GenerateProductionOrder.TabIndex = 6;
		this.btn_GenerateProductionOrder.Text = "&Generate Production Order";
		this.btn_GenerateProductionOrder.Click += new System.EventHandler(this.btn_GenerateProductionOrder_Click);
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_OK.ImageOptions.Image");
		this.btn_OK.Location = new System.Drawing.Point(2, 606);
		this.btn_OK.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.Size = new System.Drawing.Size(74, 21);
		this.btn_OK.TabIndex = 5;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.split_Main.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.split_Main.Horizontal = false;
		this.split_Main.Location = new System.Drawing.Point(0, 0);
		this.split_Main.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.split_Main.Name = "split_Main";
		this.split_Main.Panel1.Controls.Add(this.groupControl1);
		this.split_Main.Panel1.Text = "Panel1";
		this.split_Main.Panel2.Controls.Add(this.tabControl_Planning);
		this.split_Main.Panel2.Text = "Panel2";
		this.split_Main.Size = new System.Drawing.Size(1060, 603);
		this.split_Main.SplitterPosition = 168;
		this.split_Main.TabIndex = 4;
		this.split_Main.Text = "splitContainerControl1";
		this.groupControl1.AppearanceCaption.Options.UseTextOptions = true;
		this.groupControl1.AppearanceCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.groupControl1.Controls.Add(this.grd_ProductionRequest);
		this.groupControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.groupControl1.Location = new System.Drawing.Point(0, 0);
		this.groupControl1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.groupControl1.Name = "groupControl1";
		this.groupControl1.Size = new System.Drawing.Size(1060, 168);
		this.groupControl1.TabIndex = 1;
		this.groupControl1.Text = "Production Request";
		this.grd_ProductionRequest.AllowPasteExcel = true;
		this.grd_ProductionRequest.ClipboardData = null;
		this.grd_ProductionRequest.ContextMenuStrip = this.ctx_Request;
		this.grd_ProductionRequest.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_ProductionRequest.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.grd_ProductionRequest.Location = new System.Drawing.Point(2, 23);
		this.grd_ProductionRequest.MainView = this.gv_ProductionRequest;
		this.grd_ProductionRequest.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.grd_ProductionRequest.Name = "grd_ProductionRequest";
		this.grd_ProductionRequest.Size = new System.Drawing.Size(1056, 143);
		this.grd_ProductionRequest.TabIndex = 0;
		this.grd_ProductionRequest.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_ProductionRequest });
		this.ctx_Request.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.removeRowsToolStripMenuItem });
		this.ctx_Request.Name = "ctx_Request";
		this.ctx_Request.Size = new System.Drawing.Size(157, 26);
		this.removeRowsToolStripMenuItem.Name = "removeRowsToolStripMenuItem";
		this.removeRowsToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
		this.removeRowsToolStripMenuItem.Text = "Remove Row(s)";
		this.removeRowsToolStripMenuItem.Click += new System.EventHandler(this.menu_Request_RemoveRow_Click);
		this.gv_ProductionRequest.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_ProductionRequest.ColumnKey = null;
		this.gv_ProductionRequest.DisableFormSetting = false;
		this.gv_ProductionRequest.DisplayException = false;
		this.gv_ProductionRequest.EditColumn = null;
		this.gv_ProductionRequest.FieldCurrency = null;
		this.gv_ProductionRequest.FieldRate = null;
		this.gv_ProductionRequest.FormId = "oProductionForBatch";
		this.gv_ProductionRequest.GridControl = this.grd_ProductionRequest;
		this.gv_ProductionRequest.IgnoreFilter = false;
		this.gv_ProductionRequest.IndexEditRow = -1;
		this.gv_ProductionRequest.IndicatorWidth = 35;
		this.gv_ProductionRequest.IsApzTable = false;
		this.gv_ProductionRequest.IsInsertNewRow = false;
		this.gv_ProductionRequest.IsLoadingData = false;
		this.gv_ProductionRequest.IsMappingContextAction = false;
		this.gv_ProductionRequest.ItemId = "OREQUEST";
		this.gv_ProductionRequest.LstColumnAllowBatch = null;
		this.gv_ProductionRequest.LstColumnNotAllowBatch = null;
		this.gv_ProductionRequest.LstColumnNotEditable = null;
		this.gv_ProductionRequest.LstColumNotSetting = null;
		this.gv_ProductionRequest.Name = "gv_ProductionRequest";
		this.gv_ProductionRequest.OptionsSelection.CheckBoxSelectorColumnWidth = 25;
		this.gv_ProductionRequest.OptionsSelection.MultiSelect = true;
		this.gv_ProductionRequest.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
		this.gv_ProductionRequest.OptionsView.ColumnAutoWidth = false;
		this.gv_ProductionRequest.OptionsView.ShowGroupPanel = false;
		this.gv_ProductionRequest.UseBatchMod = false;
		this.gv_ProductionRequest.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_ProductionRequest_CellValueChanged);
		this.tabControl_Planning.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tabControl_Planning.IsSearchControl = false;
		this.tabControl_Planning.Location = new System.Drawing.Point(0, 0);
		this.tabControl_Planning.Name = "tabControl_Planning";
		this.tabControl_Planning.SelectedTabPage = this.xtraTabPage2;
		this.tabControl_Planning.Size = new System.Drawing.Size(1060, 425);
		this.tabControl_Planning.TabIndex = 0;
		this.tabControl_Planning.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[2] { this.xtraTabPage2, this.tab_Planning });
		this.xtraTabPage2.Controls.Add(this.panelControl1);
		this.xtraTabPage2.EditControl = null;
		this.xtraTabPage2.EditValue = "Select Batch Issue";
		this.xtraTabPage2.FieldName = "";
		this.xtraTabPage2.IsSearchControl = false;
		this.xtraTabPage2.Name = "xtraTabPage2";
		this.xtraTabPage2.ReadOnly = false;
		this.xtraTabPage2.SettingInVisible = false;
		this.xtraTabPage2.SettingReadonly = false;
		this.xtraTabPage2.Size = new System.Drawing.Size(1058, 400);
		this.xtraTabPage2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPage2.Text = "Select Batch Issue";
		this.panelControl1.Controls.Add(this.chb_StatusQC);
		this.panelControl1.Controls.Add(this.lb_BusinessPartner);
		this.panelControl1.Controls.Add(this.btn_ChooseBatch);
		this.panelControl1.Controls.Add(this.panelControl2);
		this.panelControl1.Controls.Add(this.btn_TinhGiay);
		this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panelControl1.Location = new System.Drawing.Point(0, 0);
		this.panelControl1.Name = "panelControl1";
		this.panelControl1.Size = new System.Drawing.Size(1058, 400);
		this.panelControl1.TabIndex = 0;
		this.chb_StatusQC.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.chb_StatusQC.Location = new System.Drawing.Point(920, 9);
		this.chb_StatusQC.Name = "chb_StatusQC";
		this.chb_StatusQC.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.chb_StatusQC.Size = new System.Drawing.Size(133, 20);
		this.chb_StatusQC.TabIndex = 59;
		this.lb_BusinessPartner.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lb_BusinessPartner.EditControl = null;
		this.lb_BusinessPartner.EditValue = "Tình trạng hàng";
		this.lb_BusinessPartner.FieldName = null;
		this.lb_BusinessPartner.HideWhenEmpty = false;
		this.lb_BusinessPartner.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_BusinessPartner.Location = new System.Drawing.Point(840, 12);
		this.lb_BusinessPartner.Name = "lb_BusinessPartner";
		this.lb_BusinessPartner.ReadOnly = false;
		this.lb_BusinessPartner.SettingInVisible = false;
		this.lb_BusinessPartner.SettingReadonly = false;
		this.lb_BusinessPartner.ShowUnderLine = true;
		this.lb_BusinessPartner.Size = new System.Drawing.Size(83, 20);
		this.lb_BusinessPartner.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_BusinessPartner.TabIndex = 58;
		this.lb_BusinessPartner.Utext = "Tình trạng hàng";
		this.btn_ChooseBatch.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_ChooseBatch.ImageOptions.Image");
		this.btn_ChooseBatch.Location = new System.Drawing.Point(96, 6);
		this.btn_ChooseBatch.Name = "btn_ChooseBatch";
		this.btn_ChooseBatch.Size = new System.Drawing.Size(88, 23);
		this.btn_ChooseBatch.TabIndex = 4;
		this.btn_ChooseBatch.Text = "Chọn Giấy";
		this.btn_ChooseBatch.Click += new System.EventHandler(this.btn_ChooseBatch_Click);
		this.panelControl2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.panelControl2.Controls.Add(this.tre_DSCuon);
		this.panelControl2.Location = new System.Drawing.Point(2, 35);
		this.panelControl2.Name = "panelControl2";
		this.panelControl2.Size = new System.Drawing.Size(1054, 363);
		this.panelControl2.TabIndex = 3;
		this.tre_DSCuon.AllowPasteExcel = false;
		this.tre_DSCuon.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.tre_DSCuon.ColumnKey = null;
		this.tre_DSCuon.DisableFormSetting = false;
		this.tre_DSCuon.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tre_DSCuon.EditColumn = null;
		this.tre_DSCuon.FieldCurrency = null;
		this.tre_DSCuon.FieldRate = null;
		this.tre_DSCuon.FormId = "oProductionForBatch";
		this.tre_DSCuon.IndexEditRow = 0;
		this.tre_DSCuon.IndicatorWidth = 40;
		this.tre_DSCuon.IsApzTable = false;
		this.tre_DSCuon.IsLoadingData = false;
		this.tre_DSCuon.ItemId = "OSELECTBATCH";
		this.tre_DSCuon.Location = new System.Drawing.Point(2, 2);
		this.tre_DSCuon.LstColumnNotEditable = null;
		this.tre_DSCuon.LstColumNotSetting = null;
		this.tre_DSCuon.Name = "tre_DSCuon";
		this.tre_DSCuon.OptionsFind.FindNullPrompt = "Nhập để tìm kiếm...";
		this.tre_DSCuon.Size = new System.Drawing.Size(1050, 359);
		this.tre_DSCuon.TabIndex = 1;
		this.tre_DSCuon.CellValueChanged += new DevExpress.XtraTreeList.CellValueChangedEventHandler(this.tre_DSCuon_CellValueChanged);
		this.btn_TinhGiay.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_TinhGiay.ImageOptions.Image");
		this.btn_TinhGiay.Location = new System.Drawing.Point(4, 6);
		this.btn_TinhGiay.Name = "btn_TinhGiay";
		this.btn_TinhGiay.Size = new System.Drawing.Size(85, 23);
		this.btn_TinhGiay.TabIndex = 2;
		this.btn_TinhGiay.Text = "Xem tồn kho";
		this.btn_TinhGiay.Click += new System.EventHandler(this.btn_TinhGiay_Click);
		this.tab_Planning.Controls.Add(this.splitContainerControl1);
		this.tab_Planning.EditControl = null;
		this.tab_Planning.EditValue = "Production Planning";
		this.tab_Planning.FieldName = "";
		this.tab_Planning.IsSearchControl = false;
		this.tab_Planning.Name = "tab_Planning";
		this.tab_Planning.ReadOnly = false;
		this.tab_Planning.SettingInVisible = false;
		this.tab_Planning.SettingReadonly = false;
		this.tab_Planning.Size = new System.Drawing.Size(1058, 400);
		this.tab_Planning.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Planning.Text = "Production Planning";
		this.splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(0, 0);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.tv_ProductionPlanning);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.grd_Operation);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(1058, 400);
		this.splitContainerControl1.SplitterPosition = 171;
		this.splitContainerControl1.TabIndex = 0;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.tv_ProductionPlanning.AllowPasteExcel = false;
		this.tv_ProductionPlanning.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.tv_ProductionPlanning.ColumnKey = null;
		this.tv_ProductionPlanning.ContextMenuStrip = this.ctx_Planning;
		this.tv_ProductionPlanning.DisableFormSetting = false;
		this.tv_ProductionPlanning.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tv_ProductionPlanning.EditColumn = null;
		this.tv_ProductionPlanning.FieldCurrency = null;
		this.tv_ProductionPlanning.FieldRate = null;
		this.tv_ProductionPlanning.FormId = "oProductionForBatch";
		this.tv_ProductionPlanning.IndexEditRow = 0;
		this.tv_ProductionPlanning.IndicatorWidth = 40;
		this.tv_ProductionPlanning.IsApzTable = false;
		this.tv_ProductionPlanning.IsLoadingData = false;
		this.tv_ProductionPlanning.ItemId = "OPRODUCTIONPLANNING";
		this.tv_ProductionPlanning.Location = new System.Drawing.Point(0, 0);
		this.tv_ProductionPlanning.LstColumnNotEditable = null;
		this.tv_ProductionPlanning.LstColumNotSetting = null;
		this.tv_ProductionPlanning.Name = "tv_ProductionPlanning";
		this.tv_ProductionPlanning.OptionsSelection.MultiSelect = true;
		this.tv_ProductionPlanning.ParentFieldName = "U_FatherId";
		this.tv_ProductionPlanning.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.rep_PlanWhs });
		this.tv_ProductionPlanning.Size = new System.Drawing.Size(1058, 171);
		this.tv_ProductionPlanning.TabIndex = 0;
		this.tv_ProductionPlanning.CellValueChanged += new DevExpress.XtraTreeList.CellValueChangedEventHandler(this.tv_ProductionPlanning_CellValueChanged);
		this.tv_ProductionPlanning.FocusedNodeChanged += new DevExpress.XtraTreeList.FocusedNodeChangedEventHandler(this.tv_ProductionPlanning_FocusedNodeChanged);
		this.tv_ProductionPlanning.PopupMenuShowing += new DevExpress.XtraTreeList.PopupMenuShowingEventHandler(this.tv_ProductionPlanning_PopupMenuShowing);
		this.tv_ProductionPlanning.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.tv_ProductionPlanning_ShowingEditor);
		this.ctx_Planning.Items.AddRange(new System.Windows.Forms.ToolStripItem[6] { this.removeRowsToolStripMenuItem1, this.menu_plan_UnReleasePO, this.menu_plan_CancelPO, this.toolStripSeparator1, this.copyOperationToolStripMenuItem, this.pasteOperationToolStripMenuItem });
		this.ctx_Planning.Name = "ctx_Planning";
		this.ctx_Planning.Size = new System.Drawing.Size(224, 120);
		this.ctx_Planning.Opening += new System.ComponentModel.CancelEventHandler(this.ctx_Planning_Opening);
		this.removeRowsToolStripMenuItem1.Name = "removeRowsToolStripMenuItem1";
		this.removeRowsToolStripMenuItem1.Size = new System.Drawing.Size(223, 22);
		this.removeRowsToolStripMenuItem1.Text = "Remove Row";
		this.removeRowsToolStripMenuItem1.Click += new System.EventHandler(this.menu_plan_RemoveRow_Click);
		this.menu_plan_UnReleasePO.Name = "menu_plan_UnReleasePO";
		this.menu_plan_UnReleasePO.Size = new System.Drawing.Size(223, 22);
		this.menu_plan_UnReleasePO.Text = "UnRelease Production Order";
		this.menu_plan_UnReleasePO.Click += new System.EventHandler(this.menu_plan_UnReleasePO_Click);
		this.menu_plan_CancelPO.Name = "menu_plan_CancelPO";
		this.menu_plan_CancelPO.Size = new System.Drawing.Size(223, 22);
		this.menu_plan_CancelPO.Text = "Cancel Production Order";
		this.menu_plan_CancelPO.Click += new System.EventHandler(this.menu_plan_CancelPO_Click);
		this.toolStripSeparator1.Name = "toolStripSeparator1";
		this.toolStripSeparator1.Size = new System.Drawing.Size(220, 6);
		this.copyOperationToolStripMenuItem.Name = "copyOperationToolStripMenuItem";
		this.copyOperationToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
		this.copyOperationToolStripMenuItem.Text = "Copy Operation";
		this.copyOperationToolStripMenuItem.Click += new System.EventHandler(this.copyOperationToolStripMenuItem_Click);
		this.pasteOperationToolStripMenuItem.Name = "pasteOperationToolStripMenuItem";
		this.pasteOperationToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
		this.pasteOperationToolStripMenuItem.Text = "Paste Operation";
		this.pasteOperationToolStripMenuItem.Click += new System.EventHandler(this.pasteOperationToolStripMenuItem_Click);
		this.rep_PlanWhs.AutoHeight = false;
		this.rep_PlanWhs.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.rep_PlanWhs.FieldName = null;
		this.rep_PlanWhs.GridView = null;
		this.rep_PlanWhs.IsSearchControl = false;
		this.rep_PlanWhs.Name = "rep_PlanWhs";
		this.rep_PlanWhs.TreeList = null;
		this.grd_Operation.AllowPasteExcel = false;
		this.grd_Operation.ClipboardData = null;
		this.grd_Operation.ContextMenuStrip = this.ctm_Operation;
		this.grd_Operation.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Operation.Location = new System.Drawing.Point(0, 0);
		this.grd_Operation.MainView = this.gv_Operation;
		this.grd_Operation.Name = "grd_Operation";
		this.grd_Operation.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.rep_Whs });
		this.grd_Operation.Size = new System.Drawing.Size(1058, 219);
		this.grd_Operation.TabIndex = 0;
		this.grd_Operation.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Operation });
		this.ctm_Operation.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.removeRowToolStripMenuItem });
		this.ctm_Operation.Name = "ctm_Operation";
		this.ctm_Operation.Size = new System.Drawing.Size(144, 26);
		this.ctm_Operation.Opening += new System.ComponentModel.CancelEventHandler(this.ctm_Operation_Opening);
		this.removeRowToolStripMenuItem.Name = "removeRowToolStripMenuItem";
		this.removeRowToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
		this.removeRowToolStripMenuItem.Text = "Remove Row";
		this.removeRowToolStripMenuItem.Click += new System.EventHandler(this.removeRowToolStripMenuItem_Click);
		this.gv_Operation.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Operation.ColumnKey = null;
		this.gv_Operation.DisableFormSetting = false;
		this.gv_Operation.DisplayException = false;
		this.gv_Operation.EditColumn = null;
		this.gv_Operation.FieldCurrency = null;
		this.gv_Operation.FieldRate = null;
		this.gv_Operation.FormId = "oProductionForBatch";
		this.gv_Operation.GridControl = this.grd_Operation;
		this.gv_Operation.IgnoreFilter = false;
		this.gv_Operation.IndexEditRow = -1;
		this.gv_Operation.IndicatorWidth = 35;
		this.gv_Operation.IsApzTable = false;
		this.gv_Operation.IsInsertNewRow = false;
		this.gv_Operation.IsLoadingData = false;
		this.gv_Operation.IsMappingContextAction = false;
		this.gv_Operation.ItemId = "OOPERATION";
		this.gv_Operation.LstColumnAllowBatch = null;
		this.gv_Operation.LstColumnNotAllowBatch = null;
		this.gv_Operation.LstColumnNotEditable = null;
		this.gv_Operation.LstColumNotSetting = null;
		this.gv_Operation.Name = "gv_Operation";
		this.gv_Operation.OptionsCustomization.AllowSort = false;
		this.gv_Operation.UseBatchMod = false;
		this.gv_Operation.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_Operation_ShowingEditor);
		this.gv_Operation.InitNewRow += new DevExpress.XtraGrid.Views.Grid.InitNewRowEventHandler(this.gv_Operation_InitNewRow);
		this.gv_Operation.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Operation_CellValueChanged);
		this.rep_Whs.AutoHeight = false;
		this.rep_Whs.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.rep_Whs.FieldName = null;
		this.rep_Whs.GridView = null;
		this.rep_Whs.IsSearchControl = false;
		this.rep_Whs.Name = "rep_Whs";
		this.rep_Whs.TreeList = null;
		this.tabPage_Planned.Controls.Add(this.btn_Release);
		this.tabPage_Planned.Controls.Add(this.grd_Planned);
		this.tabPage_Planned.EditControl = null;
		this.tabPage_Planned.EditValue = "Planned";
		this.tabPage_Planned.FieldName = "";
		this.tabPage_Planned.IsSearchControl = false;
		this.tabPage_Planned.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.tabPage_Planned.Name = "tabPage_Planned";
		this.tabPage_Planned.PageVisible = false;
		this.tabPage_Planned.ReadOnly = false;
		this.tabPage_Planned.SettingInVisible = false;
		this.tabPage_Planned.SettingReadonly = false;
		this.tabPage_Planned.Size = new System.Drawing.Size(1064, 634);
		this.tabPage_Planned.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tabPage_Planned.Text = "Planned";
		this.btn_Release.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Release.Enabled = false;
		this.btn_Release.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Release.ImageOptions.Image");
		this.btn_Release.Location = new System.Drawing.Point(2, 448);
		this.btn_Release.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.btn_Release.Name = "btn_Release";
		this.btn_Release.Size = new System.Drawing.Size(74, 21);
		this.btn_Release.TabIndex = 6;
		this.btn_Release.Text = "Release";
		this.grd_Planned.AllowPasteExcel = false;
		this.grd_Planned.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Planned.ClipboardData = null;
		this.grd_Planned.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.grd_Planned.Location = new System.Drawing.Point(-214, -46);
		this.grd_Planned.MainView = this.gv_Planned;
		this.grd_Planned.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.grd_Planned.Name = "grd_Planned";
		this.grd_Planned.Size = new System.Drawing.Size(922, 442);
		this.grd_Planned.TabIndex = 0;
		this.grd_Planned.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Planned });
		this.gv_Planned.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Planned.ColumnKey = null;
		this.gv_Planned.DisableFormSetting = false;
		this.gv_Planned.DisplayException = false;
		this.gv_Planned.EditColumn = null;
		this.gv_Planned.FieldCurrency = null;
		this.gv_Planned.FieldRate = null;
		this.gv_Planned.FormId = "oProductionForBatch";
		this.gv_Planned.GridControl = this.grd_Planned;
		this.gv_Planned.IgnoreFilter = false;
		this.gv_Planned.IndexEditRow = -1;
		this.gv_Planned.IndicatorWidth = 35;
		this.gv_Planned.IsApzTable = false;
		this.gv_Planned.IsInsertNewRow = false;
		this.gv_Planned.IsLoadingData = false;
		this.gv_Planned.IsMappingContextAction = false;
		this.gv_Planned.ItemId = "OPLANNED";
		this.gv_Planned.LstColumnAllowBatch = null;
		this.gv_Planned.LstColumnNotAllowBatch = null;
		this.gv_Planned.LstColumnNotEditable = null;
		this.gv_Planned.LstColumNotSetting = null;
		this.gv_Planned.Name = "gv_Planned";
		this.gv_Planned.OptionsSelection.MultiSelect = true;
		this.gv_Planned.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
		this.gv_Planned.UseBatchMod = false;
		this.tabPage_Released.Controls.Add(this.btn_Close);
		this.tabPage_Released.Controls.Add(this.grd_Released);
		this.tabPage_Released.EditControl = null;
		this.tabPage_Released.EditValue = "Released";
		this.tabPage_Released.FieldName = "";
		this.tabPage_Released.IsSearchControl = false;
		this.tabPage_Released.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.tabPage_Released.Name = "tabPage_Released";
		this.tabPage_Released.PageVisible = false;
		this.tabPage_Released.ReadOnly = false;
		this.tabPage_Released.SettingInVisible = false;
		this.tabPage_Released.SettingReadonly = false;
		this.tabPage_Released.Size = new System.Drawing.Size(1064, 634);
		this.tabPage_Released.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tabPage_Released.Text = "Released";
		this.tabPage_Released.Tooltip = "Các lệnh sản xuất đang chạy";
		this.btn_Close.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Close.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Close.ImageOptions.Image");
		this.btn_Close.Location = new System.Drawing.Point(2, 456);
		this.btn_Close.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.btn_Close.Name = "btn_Close";
		this.btn_Close.Size = new System.Drawing.Size(74, 21);
		this.btn_Close.TabIndex = 8;
		this.btn_Close.Text = "Close";
		this.grd_Released.AllowPasteExcel = false;
		this.grd_Released.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Released.ClipboardData = null;
		this.grd_Released.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.grd_Released.Location = new System.Drawing.Point(-1, 0);
		this.grd_Released.MainView = this.gv_Released;
		this.grd_Released.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.grd_Released.Name = "grd_Released";
		this.grd_Released.Size = new System.Drawing.Size(925, 450);
		this.grd_Released.TabIndex = 7;
		this.grd_Released.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Released });
		this.gv_Released.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Released.ColumnKey = null;
		this.gv_Released.DisableFormSetting = false;
		this.gv_Released.DisplayException = false;
		this.gv_Released.EditColumn = null;
		this.gv_Released.FieldCurrency = null;
		this.gv_Released.FieldRate = null;
		this.gv_Released.FormId = "oProductionForBatch";
		this.gv_Released.GridControl = this.grd_Released;
		this.gv_Released.IgnoreFilter = false;
		this.gv_Released.IndexEditRow = -1;
		this.gv_Released.IndicatorWidth = 35;
		this.gv_Released.IsApzTable = false;
		this.gv_Released.IsInsertNewRow = false;
		this.gv_Released.IsLoadingData = false;
		this.gv_Released.IsMappingContextAction = false;
		this.gv_Released.ItemId = "ORELEASE";
		this.gv_Released.LstColumnAllowBatch = null;
		this.gv_Released.LstColumnNotAllowBatch = null;
		this.gv_Released.LstColumnNotEditable = null;
		this.gv_Released.LstColumNotSetting = null;
		this.gv_Released.Name = "gv_Released";
		this.gv_Released.OptionsSelection.MultiSelect = true;
		this.gv_Released.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
		this.gv_Released.UseBatchMod = false;
		this.tabPage_Holding.Controls.Add(this.grd_Holding);
		this.tabPage_Holding.EditControl = null;
		this.tabPage_Holding.EditValue = "Pending";
		this.tabPage_Holding.FieldName = "";
		this.tabPage_Holding.IsSearchControl = false;
		this.tabPage_Holding.Name = "tabPage_Holding";
		this.tabPage_Holding.PageVisible = false;
		this.tabPage_Holding.ReadOnly = false;
		this.tabPage_Holding.SettingInVisible = false;
		this.tabPage_Holding.SettingReadonly = false;
		this.tabPage_Holding.Size = new System.Drawing.Size(1064, 634);
		this.tabPage_Holding.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tabPage_Holding.Text = "Pending";
		this.tabPage_Holding.Tooltip = "Các lệnh sản xuất đã tạm dừng";
		this.grd_Holding.AllowPasteExcel = false;
		this.grd_Holding.ClipboardData = null;
		this.grd_Holding.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Holding.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.grd_Holding.Location = new System.Drawing.Point(0, 0);
		this.grd_Holding.MainView = this.gv_Holding;
		this.grd_Holding.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.grd_Holding.Name = "grd_Holding";
		this.grd_Holding.Size = new System.Drawing.Size(1064, 634);
		this.grd_Holding.TabIndex = 8;
		this.grd_Holding.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Holding });
		this.gv_Holding.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Holding.ColumnKey = null;
		this.gv_Holding.DisableFormSetting = false;
		this.gv_Holding.DisplayException = false;
		this.gv_Holding.EditColumn = null;
		this.gv_Holding.FieldCurrency = null;
		this.gv_Holding.FieldRate = null;
		this.gv_Holding.FormId = "oProductionForBatch";
		this.gv_Holding.GridControl = this.grd_Holding;
		this.gv_Holding.IgnoreFilter = false;
		this.gv_Holding.IndexEditRow = -1;
		this.gv_Holding.IndicatorWidth = 35;
		this.gv_Holding.IsApzTable = false;
		this.gv_Holding.IsInsertNewRow = false;
		this.gv_Holding.IsLoadingData = false;
		this.gv_Holding.IsMappingContextAction = false;
		this.gv_Holding.ItemId = "OHOLDING";
		this.gv_Holding.LstColumnAllowBatch = null;
		this.gv_Holding.LstColumnNotAllowBatch = null;
		this.gv_Holding.LstColumnNotEditable = null;
		this.gv_Holding.LstColumNotSetting = null;
		this.gv_Holding.Name = "gv_Holding";
		this.gv_Holding.UseBatchMod = false;
		this.tabPage_Closed.Controls.Add(this.grd_Closed);
		this.tabPage_Closed.EditControl = null;
		this.tabPage_Closed.EditValue = "Closed";
		this.tabPage_Closed.FieldName = "";
		this.tabPage_Closed.IsSearchControl = false;
		this.tabPage_Closed.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.tabPage_Closed.Name = "tabPage_Closed";
		this.tabPage_Closed.PageVisible = false;
		this.tabPage_Closed.ReadOnly = false;
		this.tabPage_Closed.SettingInVisible = false;
		this.tabPage_Closed.SettingReadonly = false;
		this.tabPage_Closed.Size = new System.Drawing.Size(1064, 634);
		this.tabPage_Closed.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tabPage_Closed.Text = "Closed";
		this.tabPage_Closed.Tooltip = "Các lệnh sản xuất đã hoàn thành";
		this.grd_Closed.AllowPasteExcel = false;
		this.grd_Closed.ClipboardData = null;
		this.grd_Closed.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Closed.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.grd_Closed.Location = new System.Drawing.Point(0, 0);
		this.grd_Closed.MainView = this.gv_Closed;
		this.grd_Closed.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.grd_Closed.Name = "grd_Closed";
		this.grd_Closed.Size = new System.Drawing.Size(1064, 634);
		this.grd_Closed.TabIndex = 7;
		this.grd_Closed.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Closed });
		this.gv_Closed.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Closed.ColumnKey = null;
		this.gv_Closed.DisableFormSetting = false;
		this.gv_Closed.DisplayException = false;
		this.gv_Closed.EditColumn = null;
		this.gv_Closed.FieldCurrency = null;
		this.gv_Closed.FieldRate = null;
		this.gv_Closed.FormId = "oProductionForBatch";
		this.gv_Closed.GridControl = this.grd_Closed;
		this.gv_Closed.IgnoreFilter = false;
		this.gv_Closed.IndexEditRow = -1;
		this.gv_Closed.IndicatorWidth = 35;
		this.gv_Closed.IsApzTable = false;
		this.gv_Closed.IsInsertNewRow = false;
		this.gv_Closed.IsLoadingData = false;
		this.gv_Closed.IsMappingContextAction = false;
		this.gv_Closed.ItemId = "OCLOSED";
		this.gv_Closed.LstColumnAllowBatch = null;
		this.gv_Closed.LstColumnNotAllowBatch = null;
		this.gv_Closed.LstColumnNotEditable = null;
		this.gv_Closed.LstColumNotSetting = null;
		this.gv_Closed.Name = "gv_Closed";
		this.gv_Closed.UseBatchMod = false;
		this.tabPage_Canceled.Controls.Add(this.grd_Canceled);
		this.tabPage_Canceled.EditControl = null;
		this.tabPage_Canceled.EditValue = "Canceled";
		this.tabPage_Canceled.FieldName = "";
		this.tabPage_Canceled.IsSearchControl = false;
		this.tabPage_Canceled.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.tabPage_Canceled.Name = "tabPage_Canceled";
		this.tabPage_Canceled.PageVisible = false;
		this.tabPage_Canceled.ReadOnly = false;
		this.tabPage_Canceled.SettingInVisible = false;
		this.tabPage_Canceled.SettingReadonly = false;
		this.tabPage_Canceled.Size = new System.Drawing.Size(1064, 634);
		this.tabPage_Canceled.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tabPage_Canceled.Text = "Canceled";
		this.grd_Canceled.AllowPasteExcel = false;
		this.grd_Canceled.ClipboardData = null;
		this.grd_Canceled.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Canceled.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.grd_Canceled.Location = new System.Drawing.Point(0, 0);
		this.grd_Canceled.MainView = this.gv_Canceled;
		this.grd_Canceled.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.grd_Canceled.Name = "grd_Canceled";
		this.grd_Canceled.Size = new System.Drawing.Size(1064, 634);
		this.grd_Canceled.TabIndex = 7;
		this.grd_Canceled.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Canceled });
		this.gv_Canceled.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Canceled.ColumnKey = null;
		this.gv_Canceled.DisableFormSetting = false;
		this.gv_Canceled.DisplayException = false;
		this.gv_Canceled.EditColumn = null;
		this.gv_Canceled.FieldCurrency = null;
		this.gv_Canceled.FieldRate = null;
		this.gv_Canceled.FormId = "oProductionForBatch";
		this.gv_Canceled.GridControl = this.grd_Canceled;
		this.gv_Canceled.IgnoreFilter = false;
		this.gv_Canceled.IndexEditRow = -1;
		this.gv_Canceled.IndicatorWidth = 35;
		this.gv_Canceled.IsApzTable = false;
		this.gv_Canceled.IsInsertNewRow = false;
		this.gv_Canceled.IsLoadingData = false;
		this.gv_Canceled.IsMappingContextAction = false;
		this.gv_Canceled.ItemId = "OCANCELED";
		this.gv_Canceled.LstColumnAllowBatch = null;
		this.gv_Canceled.LstColumnNotAllowBatch = null;
		this.gv_Canceled.LstColumnNotEditable = null;
		this.gv_Canceled.LstColumNotSetting = null;
		this.gv_Canceled.Name = "gv_Canceled";
		this.gv_Canceled.UseBatchMod = false;
		this.ctx_Main.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.menuMainClose, this.menuMainCancel });
		this.ctx_Main.Name = "ctx_Main";
		this.ctx_Main.Size = new System.Drawing.Size(111, 48);
		this.menuMainClose.Name = "menuMainClose";
		this.menuMainClose.Size = new System.Drawing.Size(110, 22);
		this.menuMainClose.Text = "Close";
		this.menuMainClose.Click += new System.EventHandler(this.menuMainClose_Click);
		this.menuMainCancel.Name = "menuMainCancel";
		this.menuMainCancel.Size = new System.Drawing.Size(110, 22);
		this.menuMainCancel.Text = "Cancel";
		this.menuMainCancel.Click += new System.EventHandler(this.menuMainCancel_Click);
		this.txt_StockRequest.Location = new System.Drawing.Point(677, 1);
		this.txt_StockRequest.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.txt_StockRequest.Name = "txt_StockRequest";
		this.txt_StockRequest.Size = new System.Drawing.Size(57, 20);
		this.txt_StockRequest.TabIndex = 16;
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "Stock Request";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(601, 4);
		this.labelLine4.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(80, 21);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 15;
		this.labelLine4.Utext = "Stock Request";
		this.txt_Creator.Location = new System.Drawing.Point(785, 1);
		this.txt_Creator.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.txt_Creator.Name = "txt_Creator";
		this.txt_Creator.Properties.ReadOnly = true;
		this.txt_Creator.Size = new System.Drawing.Size(44, 20);
		this.txt_Creator.TabIndex = 18;
		this.txt_Creator.ToolTip = "Người lập phiếu";
		this.labelLine5.EditControl = null;
		this.labelLine5.EditValue = "Creator";
		this.labelLine5.FieldName = null;
		this.labelLine5.HideWhenEmpty = false;
		this.labelLine5.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine5.Location = new System.Drawing.Point(735, 4);
		this.labelLine5.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
		this.labelLine5.Name = "labelLine5";
		this.labelLine5.ReadOnly = false;
		this.labelLine5.SettingInVisible = false;
		this.labelLine5.SettingReadonly = false;
		this.labelLine5.ShowUnderLine = true;
		this.labelLine5.Size = new System.Drawing.Size(49, 21);
		this.labelLine5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine5.TabIndex = 17;
		this.labelLine5.Utext = "Creator";
		this.txt_Pono.Location = new System.Drawing.Point(255, 1);
		this.txt_Pono.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.txt_Pono.Name = "txt_Pono";
		this.txt_Pono.Size = new System.Drawing.Size(60, 20);
		this.txt_Pono.TabIndex = 20;
		this.txt_Pono.ToolTip = "Tìm kiếm theo lệnh sản xuất";
		this.txt_Pono.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_Pono_KeyDown);
		this.labelLine6.EditControl = null;
		this.labelLine6.EditValue = "Po No";
		this.labelLine6.FieldName = null;
		this.labelLine6.HideWhenEmpty = false;
		this.labelLine6.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine6.Location = new System.Drawing.Point(214, 4);
		this.labelLine6.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
		this.labelLine6.Name = "labelLine6";
		this.labelLine6.ReadOnly = false;
		this.labelLine6.SettingInVisible = false;
		this.labelLine6.SettingReadonly = false;
		this.labelLine6.ShowUnderLine = true;
		this.labelLine6.Size = new System.Drawing.Size(40, 21);
		this.labelLine6.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine6.TabIndex = 19;
		this.labelLine6.Utext = "Po No";
		this.txt_BatchNo.Location = new System.Drawing.Point(370, 1);
		this.txt_BatchNo.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.txt_BatchNo.Name = "txt_BatchNo";
		this.txt_BatchNo.Size = new System.Drawing.Size(119, 20);
		this.txt_BatchNo.TabIndex = 22;
		this.txt_BatchNo.ToolTip = "Tìm kiếm theo mã batch";
		this.txt_BatchNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_BatchNo_KeyDown);
		this.labelLine7.EditControl = null;
		this.labelLine7.EditValue = "Batch No";
		this.labelLine7.FieldName = null;
		this.labelLine7.HideWhenEmpty = false;
		this.labelLine7.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine7.Location = new System.Drawing.Point(319, 4);
		this.labelLine7.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
		this.labelLine7.Name = "labelLine7";
		this.labelLine7.ReadOnly = false;
		this.labelLine7.SettingInVisible = false;
		this.labelLine7.SettingReadonly = false;
		this.labelLine7.ShowUnderLine = true;
		this.labelLine7.Size = new System.Drawing.Size(55, 21);
		this.labelLine7.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine7.TabIndex = 21;
		this.labelLine7.Utext = "Batch No";
		this.txt_SoNo.Location = new System.Drawing.Point(147, 1);
		this.txt_SoNo.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.txt_SoNo.Name = "txt_SoNo";
		this.txt_SoNo.Size = new System.Drawing.Size(63, 20);
		this.txt_SoNo.TabIndex = 24;
		this.txt_SoNo.ToolTip = "Tìm kiếm theo đơn hàng";
		this.txt_SoNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_SoNo_KeyDown);
		this.labelLine8.EditControl = null;
		this.labelLine8.EditValue = "SO No";
		this.labelLine8.FieldName = null;
		this.labelLine8.HideWhenEmpty = false;
		this.labelLine8.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine8.Location = new System.Drawing.Point(112, 4);
		this.labelLine8.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
		this.labelLine8.Name = "labelLine8";
		this.labelLine8.ReadOnly = false;
		this.labelLine8.SettingInVisible = false;
		this.labelLine8.SettingReadonly = false;
		this.labelLine8.ShowUnderLine = true;
		this.labelLine8.Size = new System.Drawing.Size(33, 21);
		this.labelLine8.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine8.TabIndex = 23;
		this.labelLine8.Utext = "SO No";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1067, 690);
		this.ContextMenuStrip = this.ctx_Main;
		base.Controls.Add(this.txt_SoNo);
		base.Controls.Add(this.labelLine8);
		base.Controls.Add(this.txt_BatchNo);
		base.Controls.Add(this.labelLine7);
		base.Controls.Add(this.txt_Pono);
		base.Controls.Add(this.labelLine6);
		base.Controls.Add(this.txt_Creator);
		base.Controls.Add(this.labelLine5);
		base.Controls.Add(this.txt_StockRequest);
		base.Controls.Add(this.labelLine4);
		base.Controls.Add(this.tab_Main);
		base.Controls.Add(this.look_Status);
		base.Controls.Add(this.labelLine3);
		base.Controls.Add(this.txt_Remark);
		base.Controls.Add(this.labelLine2);
		base.Controls.Add(this.txt_DocNo);
		base.Controls.Add(this.labelLine1);
		base.FormId = "oProductionForBatch";
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_ProductionForBatch.IconOptions.Icon");
		base.Name = "frm_ProductionForBatch";
		this.Text = "Production For Batch";
		base.Load += new System.EventHandler(this.frm_ProductionForBatch_Load);
		((System.ComponentModel.ISupportInitialize)this.look_Status.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Remark.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_DocNo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tab_Main).EndInit();
		this.tab_Main.ResumeLayout(false);
		this.xtraTabPage1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.split_Main).EndInit();
		this.split_Main.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.groupControl1).EndInit();
		this.groupControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_ProductionRequest).EndInit();
		this.ctx_Request.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_ProductionRequest).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tabControl_Planning).EndInit();
		this.tabControl_Planning.ResumeLayout(false);
		this.xtraTabPage2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.panelControl1).EndInit();
		this.panelControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.chb_StatusQC.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl2).EndInit();
		this.panelControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.tre_DSCuon).EndInit();
		this.tab_Planning.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.tv_ProductionPlanning).EndInit();
		this.ctx_Planning.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.rep_PlanWhs).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Operation).EndInit();
		this.ctm_Operation.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Operation).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Whs).EndInit();
		this.tabPage_Planned.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Planned).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Planned).EndInit();
		this.tabPage_Released.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Released).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Released).EndInit();
		this.tabPage_Holding.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Holding).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Holding).EndInit();
		this.tabPage_Closed.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Closed).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Closed).EndInit();
		this.tabPage_Canceled.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Canceled).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Canceled).EndInit();
		this.ctx_Main.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txt_StockRequest.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Creator.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Pono.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BatchNo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_SoNo.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
