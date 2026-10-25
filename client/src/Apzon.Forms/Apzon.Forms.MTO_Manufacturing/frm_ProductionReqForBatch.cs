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
using Apzon.Controls.Tree;
using Apzon.Entities.Models;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;
using Apzon.Entities.Models.UserFields;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraTab;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using SAPbobsCOM;

namespace Apzon.Forms.MTO_Manufacturing;

public class frm_ProductionReqForBatch : XtraFormEx
{
	private DataTable dtSearch;

	private const string FormController = "ProductionForBatch";

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private List<UserFieldsImpl> lstUdf = new List<UserFieldsImpl>();

	private IContainer components;

	private LookUpEdit look_SalesEmp;

	private ButtonEditEx bte_BPCode;

	private LabelLine labelLine1;

	private LabelLine lb_SalesEmp;

	private SimpleButtonEx btn_Search;

	private ButtonEditEx bte_SoNo;

	private LabelLine labelLine2;

	private ButtonEditEx bte_ItemCode;

	private LabelLine labelLine3;

	private XtraTabControlEx xtraTab_ProductionControl;

	private XtraTabPageEx tab_Request;

	private TreeEx tv_Request;

	private SimpleButtonEx btn_ProductionPlan;

	private SimpleButtonEx btn_ClearAll;

	private SimpleButtonEx btn_SelectAll;

	private CheckEditEx chk_Auto;

	private SpinEdit spin_Auto;

	private Timer timer_AutoSearch;

	private XtraTabPageEx tab_Report;

	private GridControlEx grd_Planning;

	private GridViewEx gv_Planning;

	private SimpleButtonEx btn_ViewDetail;

	public LabelLine lb_ToDate;

	public LabelLine lb_FromDate;

	private XtraTabPageEx tab_ListPro;

	private TreeEx tv_ListPO;

	private SimpleButtonEx btn_DelRequest;

	private SimpleButtonEx btn_ViewDetail2;

	private SimpleButtonEx btn_Adjust;

	private SimpleButtonEx btn_PrintTem;

	private DateEditEx txtFromDate;

	private DateEditEx txtToDate;

	private CheckedComboBoxEdit chb_Whs;

	public LabelLine labelLine4;

	private SimpleButton btn_Reject;

	public frm_ProductionReqForBatch()
	{
		InitializeComponent();
	}

	private async void frm_ProductionReqForBatch_Load(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			timer_AutoSearch.Enabled = chk_Auto.Checked;
			timer_AutoSearch.Interval = Function.ParseInt(spin_Auto.EditValue) * 60000;
			txtFromDate.EditValue = DateTime.Today;
			txtToDate.EditValue = DateTime.Today;
			await LoadWhs();
			lstUdf = await _httpClient.Udf.GetListField("RDR1");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void bte_BPCode_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.Bp, false, bte_BPCode.Text, "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			DataRow chooseItem = frm_CFL2.ChooseItem;
			if (chooseItem != null && chooseItem.IsExistsColumn("CardCode"))
			{
				bte_BPCode.Text = Function.ToString(chooseItem["CardCode"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Search_Click(object sender, EventArgs e)
	{
		await LoadData();
	}

	private async Task LoadData()
	{
		WinCommons._OpenProcessing();
		try
		{
			btn_Search.Enabled = false;
			btn_ProductionPlan.Enabled = false;
			btn_DelRequest.Enabled = false;
			btn_Adjust.Enabled = false;
			CreateDataSearch();
			DataRow dataRow = dtSearch.NewRow();
			dataRow["so"] = Function.ParseInt(bte_SoNo.EditValue);
			dataRow["itemcode"] = Function.ToString(bte_ItemCode.EditValue);
			dataRow["slp"] = Function.ParseInt(look_SalesEmp.EditValue);
			dataRow["cardcode"] = Function.ToString(bte_BPCode.EditValue);
			dataRow["branch"] = GlobalData.DefaultBranch;
			dataRow["fromdate"] = Function.ParseDateTime(txtFromDate.EditValue);
			dataRow["todate"] = Function.ParseDateTime(txtToDate.EditValue);
			dataRow["whs"] = Function.ToString(chb_Whs.EditValue);
			dtSearch.Rows.Add(dataRow);
			string name = xtraTab_ProductionControl.SelectedTabPage.Name;
			if (!(name == "tab_ListPro"))
			{
				if (name == "tab_Report")
				{
					DataSet dsPlanning = await SearchPlanning();
					await GridViewHelper.GenerateGrid(grd_Planning, gv_Planning, dsPlanning, lstUdf, expand: true);
					dsPlanning.Tables.Clear();
					dsPlanning.Dispose();
				}
				else
				{
					DataSet ds = await SearchData();
					await TreeListHelper.GenerateTreeList(tv_Request, ds, lstUdf, expand: true);
					ds.Tables.Clear();
					ds.Dispose();
				}
			}
			else
			{
				DataSet dsPro = await SearchProductionOrder();
				await TreeListHelper.GenerateTreeList(tv_ListPO, dsPro, lstUdf, expand: true);
				dsPro.Tables.Clear();
				dsPro.Dispose();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		btn_Search.Enabled = true;
		btn_ProductionPlan.Enabled = true;
		btn_DelRequest.Enabled = true;
		btn_Adjust.Enabled = true;
		WinCommons._CloseProcessing();
	}

	private async Task<DataSet> SearchPlanning()
	{
		try
		{
			using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "ProductionForBatch");
			return await helper.PostAsync("SearchPlanning", dtSearch);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataSet();
	}

	private async Task<DataSet> SearchData()
	{
		try
		{
			using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "ProductionForBatch");
			return await helper.PostAsync("Search", dtSearch);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataSet();
	}

	private async Task<DataSet> SearchProductionOrder()
	{
		try
		{
			using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "ProductionForBatch");
			return await helper.PostAsync("SearchProductionOrder", dtSearch);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataSet();
	}

	private void tv_Request_CellValueChanged(object sender, DevExpress.XtraTreeList.CellValueChangedEventArgs e)
	{
	}

	private void tv_Request_CellValueChanging(object sender, DevExpress.XtraTreeList.CellValueChangedEventArgs e)
	{
		try
		{
			if (e.Column.FieldName.Equals("Checked") && Function.ParseInt(tv_Request.FocusedNode.GetValue("FatherId")) == -1 && e.Node.Nodes != null && Function.ParseBool(e.Value))
			{
				e.Node.SetValue("Checked", e.Value);
				string arg = Function.ToString(e.Node.GetValue("U_ItemCode"));
				DataTable dataTable = tv_Request.DataSourceTable();
				if (dataTable.ExistsColumn("U_ItemCode"))
				{
					DataRow[] array = dataTable.Select($"U_ItemCode='{arg}'");
					foreach (DataRow dataRow in array)
					{
						if (!Function.ParseInt(dataRow["ID"]).Equals(e.Node.Id))
						{
							dataRow["Checked"] = e.Value;
						}
					}
				}
				dataTable.AcceptChanges();
			}
			btn_ProductionPlan.Enabled = true;
			_formMode = Enums.FormViewModeType.Update;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_ProductionPlan_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataTable = tv_Request.DataSourceTable();
			if (!dataTable.IsNotNull() || !dataTable.ExistsColumn("Checked"))
			{
				return;
			}
			DataRow[] array = dataTable.Select("Checked=true And FatherId<>-1");
			if (array.IsNotNull())
			{
				DataTable dataTable2 = array.CopyToDataTable();
				if ((from m in dataTable2.AsEnumerable()
					select m["U_ItemCode"]).Distinct().ToList().Count > 1)
				{
					WinCommons.Error(Enums.MessageCode.None, "Không thể chọn 2 mã hàng khác nhau!");
					return;
				}
				frm_ProductionForBatch frm_ProductionForBatch2 = new frm_ProductionForBatch(dataTable2);
				frm_ProductionForBatch2.MdiParent = MdiParent;
				frm_ProductionForBatch2.Show();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.PleaseChooseItem, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_SelectAll_Click(object sender, EventArgs e)
	{
		CheckAll(value: true);
	}

	private void CheckAll(bool value)
	{
		try
		{
			if (tv_Request.Nodes != null && tv_Request.Nodes.Count > 0)
			{
				for (int i = 0; i < tv_Request.Nodes.Count; i++)
				{
					CheckAllNode(tv_Request.Nodes[i], value);
				}
				btn_ProductionPlan.Enabled = true;
				_formMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void CheckAllNode(TreeListNode node, bool value)
	{
		try
		{
			node.SetValue("Checked", value);
			if (node.Nodes != null && node.Nodes.Count > 0)
			{
				for (int i = 0; i < node.Nodes.Count; i++)
				{
					CheckAllNode(node.Nodes[i], value);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_ClearAll_Click(object sender, EventArgs e)
	{
		CheckAll(value: false);
	}

	private void chk_Auto_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			spin_Auto.Enabled = chk_Auto.Checked;
			timer_AutoSearch.Enabled = chk_Auto.Checked;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void spin_Auto_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			timer_AutoSearch.Interval = Function.ParseInt(spin_Auto.EditValue) * 60000;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void timer_AutoSearch_Tick(object sender, EventArgs e)
	{
		try
		{
			btn_Search_Click(null, null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void frm_ProductionReqForBatch_Shown(object sender, EventArgs e)
	{
		try
		{
			btn_Search_Click(null, null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_ViewDetail_Click(object sender, EventArgs e)
	{
		try
		{
			int num = Function.ParseInt(gv_Planning.GetFocusedRowCellValue("DocEntry"));
			if (num > 0)
			{
				frm_ProductionForBatch frm_ProductionForBatch2 = new frm_ProductionForBatch();
				frm_ProductionForBatch2.PlanId = num;
				frm_ProductionForBatch2.MdiParent = MdiParent;
				frm_ProductionForBatch2.Show();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.NoMatchingRecordFoundWithDocEntry, "", Function.ToString(num));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_ViewDetail2_Click(object sender, EventArgs e)
	{
		try
		{
			DataRow row = ((DataRowView)tv_ListPO.GetDataRecordByNode(tv_ListPO.FocusedNode)).Row;
			if (row.IsExistsColumn("TinhGiayID"))
			{
				int num = Function.ParseInt(row["TinhGiayID"]);
				if (num > 0)
				{
					frm_ProductionForBatch frm_ProductionForBatch2 = new frm_ProductionForBatch();
					frm_ProductionForBatch2.PlanId = num;
					frm_ProductionForBatch2.MdiParent = MdiParent;
					frm_ProductionForBatch2.Show();
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.NoMatchingRecordFoundWithDocEntry, "", Function.ToString(num));
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void xtraTab_ProductionControl_SelectedPageChanged(object sender, TabPageChangedEventArgs e)
	{
	}

	private async Task LoadWhs()
	{
		try
		{
			DataTable dataTable = new DataTable();
			using (HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ProductionForBatch"))
			{
				dataTable = await helper.GetAsync("LoadWhs", new
				{
					userName = GlobalData.UserName,
					branch = GlobalData.DefaultBranch
				});
			}
			if (dataTable.IsNotNull())
			{
				chb_Whs.Properties.DataSource = dataTable;
				chb_Whs.Properties.DisplayMember = "Display";
				chb_Whs.Properties.ValueMember = "Value";
			}
			foreach (DataRow row in dataTable.Rows)
			{
				if (Function.ParseBool(row["Checked"]))
				{
					CheckedComboBoxEdit checkedComboBoxEdit = chb_Whs;
					CheckedComboBoxEdit checkedComboBoxEdit2 = chb_Whs;
					object editValue = (checkedComboBoxEdit2.EditValue = checkedComboBoxEdit2.EditValue?.ToString() + "," + Function.ToString(row["Value"]));
					checkedComboBoxEdit.EditValue = editValue;
				}
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
			dtSearch.Columns.Add(new DataColumn("so", typeof(int)));
			dtSearch.Columns.Add(new DataColumn("itemcode", typeof(string)));
			dtSearch.Columns.Add(new DataColumn("slp", typeof(int)));
			dtSearch.Columns.Add(new DataColumn("cardcode", typeof(string)));
			dtSearch.Columns.Add(new DataColumn("branch", typeof(string)));
			dtSearch.Columns.Add(new DataColumn("fromdate", typeof(DateTime)));
			dtSearch.Columns.Add(new DataColumn("todate", typeof(DateTime)));
			dtSearch.Columns.Add(new DataColumn("whs", typeof(string)));
		}
		else
		{
			dtSearch.Rows.Clear();
		}
	}

	private void tv_ListPO_CellValueChanging(object sender, DevExpress.XtraTreeList.CellValueChangedEventArgs e)
	{
		try
		{
			if (!e.Column.FieldName.Equals("Checked"))
			{
				return;
			}
			int num = Function.ParseInt(tv_ListPO.FocusedNode.GetValue("ParentID"));
			int num2 = Function.ParseInt(tv_ListPO.FocusedNode.GetValue("ID"));
			if (num == num2 && e.Node.Nodes != null && Function.ParseBool(e.Value))
			{
				e.Node.SetValue("Checked", e.Value);
				int num3 = Function.ParseInt(e.Node.GetValue("U_RefNo"));
				DataTable dataTable = tv_ListPO.DataSourceTable();
				DataRow[] array = dataTable.Select($"U_RefNo='{num3}' AND PwStatus = 'L' AND BatchNo <> ''");
				for (int i = 0; i < array.Length; i++)
				{
					array[i]["Checked"] = e.Value;
				}
				dataTable.AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_DelRequest_Click(object sender, EventArgs e)
	{
		try
		{
			DataRow[] array = tv_ListPO.DataSourceTable().Select($"Checked='{true}' AND BatchNo <> ''");
			if (array != null && array.Length != 0)
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
				{
					return;
				}
				btn_DelRequest.Enabled = false;
				WinCommons._OpenProcessing();
				string text = await UpdateDelRequestBatch(array.CopyToDataTable());
				if (string.IsNullOrEmpty(text))
				{
					WinCommons.writeStatus(Enums.MessageCode.SaveSuccess, Enums.MessageType.Success, Msg_Time._TimeMedium, "");
				}
				else
				{
					WinCommons.writeStatus(Enums.MessageCode.None, Enums.MessageType.Error, Msg_Time._TimeMedium, text);
				}
				WinCommons._CloseProcessing();
			}
			else
			{
				MessageBox.Show("Chưa chọn hàng!", "System", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		btn_DelRequest.Enabled = true;
	}

	private async Task<string> UpdateDelRequestBatch(DataTable dtBatch)
	{
		try
		{
			using HttpClientHelper<DataTable> _help = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ProductionForBatch");
			return await _help.PostAsJsonAsync("UpdateDelRequestBatch", dtBatch);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private void tv_ListPO_NodeCellStyle(object sender, GetCustomNodeCellStyleEventArgs e)
	{
		try
		{
			string text = e.Column.FieldName.ToUpper();
			if ((text.Equals("AdjustQty".ToUpper()) || text.Equals("AdjustWidth".ToUpper()) || text.Equals("AdjustLength".ToUpper()) || text.Equals("AdjustQtyUoM".ToUpper())) && Math.Abs(Function.ParseDouble(e.Node.GetValue(e.Column))) > 0.0)
			{
				e.Appearance.ForeColor = Color.Red;
				e.Appearance.Font = new Font(e.Appearance.Font.FontFamily, e.Appearance.Font.Size, FontStyle.Bold);
			}
		}
		catch (Exception)
		{
		}
	}

	private async void btn_Adjust_Click(object sender, EventArgs e)
	{
		_ = 2;
		try
		{
			btn_Adjust.Enabled = false;
			string batch = "";
			DataRow[] array = (from t in tv_ListPO.DataSourceTable().AsEnumerable()
				where Math.Abs(Function.ParseDouble(t["AdjustQty"])) > 0.0
				select t).ToArray();
			if (array.Length == 0)
			{
				return;
			}
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				btn_Adjust.Enabled = true;
				return;
			}
			DocumentsImpl documentsImpl = new DocumentsImpl();
			DocumentsImpl documentsImpl2 = new DocumentsImpl();
			DataRow[] array2 = array.Where((DataRow t) => Function.ParseDouble(t["AdjustQty"]) < 0.0).ToArray();
			DataRow[] array3 = array.Where((DataRow t) => Function.ParseDouble(t["AdjustQty"]) > 0.0).ToArray();
			if (array2.IsNotNull())
			{
				documentsImpl.BPLId = Function.ParseInt(GlobalData.DefaultBranch);
				documentsImpl.UserFields = new List<UserFieldsImpl>
				{
					new UserFieldsImpl
					{
						Name = "U_paymentEntry",
						Value = 60,
						Type = BoFieldTypes.db_Numeric
					},
					new UserFieldsImpl
					{
						Name = "U_PlanId",
						Value = Function.ParseInt(array2[0]["PwNo"]),
						Type = BoFieldTypes.db_Numeric
					}
				};
				string text = string.Join(",", (from x in array2
					where !string.IsNullOrEmpty(Function.ToString(x["AdjustRemark"]))
					select Function.ToString(x["AdjustRemark"])).ToArray());
				if (string.IsNullOrEmpty(text))
				{
					XtraMessageBox.Show("Chưa nhập ghi chú điều chỉnh!", "System", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
					return;
				}
				documentsImpl.Comments = "Adjust Batches - " + text;
				documentsImpl.Lines = new List<Document_LinesImpl>();
				int num = 0;
				DataRow[] array4 = array2;
				foreach (DataRow dataRow in array4)
				{
					Document_LinesImpl document_LinesImpl = new Document_LinesImpl
					{
						ItemCode = Function.ToString(dataRow["ItemCode"]),
						WhsCode = Function.ToString(dataRow["WhsCode"]),
						LineNum = num,
						BatchNumbers = new List<BatchNumbersImpl>(),
						BinAllocations = new List<DocumentLinesBinAllocationsImpl>(),
						UserFields = new List<UserFieldsImpl>
						{
							new UserFieldsImpl
							{
								Name = "U_Width",
								Value = Function.ParseDouble(dataRow["AdjustWidth"]),
								Type = BoFieldTypes.db_Numeric,
								SubType = BoFldSubTypes.st_Quantity
							},
							new UserFieldsImpl
							{
								Name = "U_Length",
								Value = Function.ParseDouble(dataRow["AdjustLength"]),
								Type = BoFieldTypes.db_Numeric,
								SubType = BoFldSubTypes.st_Quantity
							},
							new UserFieldsImpl
							{
								Name = "U_QtyUoM",
								Value = Function.ParseDouble(dataRow["AdjustQtyUoM"]),
								Type = BoFieldTypes.db_Numeric,
								SubType = BoFldSubTypes.st_Quantity
							},
							new UserFieldsImpl
							{
								Name = "U_UnitUoM",
								Value = Function.ToString(dataRow["U_UnitUoM"]),
								Type = BoFieldTypes.db_Alpha
							}
						}
					};
					BatchNumbersImpl batchNumbersImpl = new BatchNumbersImpl
					{
						Quantity = Math.Abs(Function.ParseDouble(dataRow["AdjustQty"])),
						BatchNumber = Function.ToString(dataRow["BatchNo"]),
						BaseLineNumber = num
					};
					batch = batch + batchNumbersImpl.BatchNumber + ",";
					document_LinesImpl.BatchNumbers.Add(batchNumbersImpl);
					DocumentLinesBinAllocationsImpl documentLinesBinAllocationsImpl = new DocumentLinesBinAllocationsImpl
					{
						BaseLineNumber = num,
						BinAbsEntry = Function.ParseInt(dataRow["BinId"]),
						Quantity = batchNumbersImpl.Quantity,
						SerialAndBatchNumbersBaseLine = 0
					};
					if (documentLinesBinAllocationsImpl.BinAbsEntry > 0)
					{
						document_LinesImpl.BinAllocations.Add(documentLinesBinAllocationsImpl);
					}
					document_LinesImpl.Quantity = batchNumbersImpl.Quantity;
					documentsImpl.Lines.Add(document_LinesImpl);
					num++;
				}
			}
			if (array3.IsNotNull())
			{
				documentsImpl2.BPLId = Function.ParseInt(GlobalData.DefaultBranch);
				documentsImpl2.UserFields = new List<UserFieldsImpl>
				{
					new UserFieldsImpl
					{
						Name = "U_paymentEntry",
						Value = 59,
						Type = BoFieldTypes.db_Numeric
					},
					new UserFieldsImpl
					{
						Name = "U_PlanId",
						Value = Function.ParseInt(array3[0]["PwNo"]),
						Type = BoFieldTypes.db_Numeric
					}
				};
				string text2 = string.Join(",", (from x in array3
					where !string.IsNullOrEmpty(Function.ToString(x["AdjustRemark"]))
					select Function.ToString(x["AdjustRemark"])).ToArray());
				if (string.IsNullOrEmpty(text2))
				{
					XtraMessageBox.Show("Chưa nhập ghi chú điều chỉnh!", "System", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
					return;
				}
				documentsImpl2.Comments = "Adjust Batches - " + text2;
				documentsImpl2.Lines = new List<Document_LinesImpl>();
				int num3 = 0;
				DataRow[] array4 = array3;
				foreach (DataRow dataRow2 in array4)
				{
					Document_LinesImpl document_LinesImpl2 = new Document_LinesImpl
					{
						ItemCode = Function.ToString(dataRow2["ItemCode"]),
						WhsCode = Function.ToString(dataRow2["WhsCode"]),
						Price = Function.ParseDouble(dataRow2["Cost"]),
						BatchNumbers = new List<BatchNumbersImpl>(),
						BinAllocations = new List<DocumentLinesBinAllocationsImpl>(),
						UserFields = new List<UserFieldsImpl>
						{
							new UserFieldsImpl
							{
								Name = "U_Width",
								Value = Function.ParseDouble(dataRow2["AdjustWidth"]),
								Type = BoFieldTypes.db_Numeric,
								SubType = BoFldSubTypes.st_Quantity
							},
							new UserFieldsImpl
							{
								Name = "U_Length",
								Value = Function.ParseDouble(dataRow2["AdjustLength"]),
								Type = BoFieldTypes.db_Numeric,
								SubType = BoFldSubTypes.st_Quantity
							},
							new UserFieldsImpl
							{
								Name = "U_QtyUoM",
								Value = Function.ParseDouble(dataRow2["AdjustQtyUoM"]),
								Type = BoFieldTypes.db_Numeric,
								SubType = BoFldSubTypes.st_Quantity
							},
							new UserFieldsImpl
							{
								Name = "U_UnitUoM",
								Value = Function.ToString(dataRow2["U_UnitUoM"]),
								Type = BoFieldTypes.db_Alpha
							}
						}
					};
					BatchNumbersImpl batchNumbersImpl2 = new BatchNumbersImpl
					{
						Quantity = Math.Abs(Function.ParseDouble(dataRow2["AdjustQty"])),
						BatchNumber = Function.ToString(dataRow2["BatchNo"]),
						BaseLineNumber = num3
					};
					batch = batch + batchNumbersImpl2.BatchNumber + ",";
					document_LinesImpl2.BatchNumbers.Add(batchNumbersImpl2);
					DocumentLinesBinAllocationsImpl documentLinesBinAllocationsImpl2 = new DocumentLinesBinAllocationsImpl
					{
						BaseLineNumber = num3,
						BinAbsEntry = Function.ParseInt(dataRow2["BinId"]),
						Quantity = batchNumbersImpl2.Quantity,
						SerialAndBatchNumbersBaseLine = 0
					};
					if (documentLinesBinAllocationsImpl2.BinAbsEntry > 0)
					{
						document_LinesImpl2.BinAllocations.Add(documentLinesBinAllocationsImpl2);
					}
					document_LinesImpl2.Quantity = batchNumbersImpl2.Quantity;
					documentsImpl2.Lines.Add(document_LinesImpl2);
					num3++;
				}
			}
			WinCommons._OpenProcessing();
			HttpResult httpResult;
			using (HttpClientHelper<HttpResult> help = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "ProductionForBatch"))
			{
				httpResult = await help.PostAsync("Adjust", new DocumentsImpl[2] { documentsImpl, documentsImpl2 });
			}
			if (httpResult != null)
			{
				if (!string.IsNullOrEmpty(httpResult.Message))
				{
					XtraMessageBox.Show(httpResult.Message, "System", MessageBoxButtons.OK, MessageBoxIcon.Hand);
					return;
				}
				WinCommons.Success(Enums.MessageCode.Success, "");
				await PrintTem(batch);
				await LoadData();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			btn_Adjust.Enabled = true;
			WinCommons._CloseProcessing();
		}
	}

	private void tv_ListPO_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (tv_ListPO.FocusedColumn.FieldName.StartsWith("Adjust"))
			{
				e.Cancel = string.IsNullOrEmpty(Function.ToString(tv_ListPO.FocusedNode.GetValue("BatchNo")));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void tv_ListPO_CellValueChanged(object sender, DevExpress.XtraTreeList.CellValueChangedEventArgs e)
	{
		try
		{
			if (e.Column.FieldName.ToUpper().Equals("ADJUSTWIDTH") || e.Column.FieldName.ToUpper().Equals("ADJUSTLENGTH") || e.Column.FieldName.ToUpper().Equals("ADJUSTQTYUOM"))
			{
				TreeListNode focusedNode = tv_ListPO.FocusedNode;
				if (Function.ParseDouble(focusedNode.GetValue("AdjustWidth")).Equals(0.0))
				{
					focusedNode.SetValue("AdjustWidth", Function.ParseDouble(focusedNode.GetValue("U_Width")));
				}
				if (Function.ParseDouble(focusedNode.GetValue("AdjustLength")).Equals(0.0))
				{
					focusedNode.SetValue("AdjustLength", Function.ParseDouble(focusedNode.GetValue("U_Length")));
				}
				if (Function.ParseDouble(focusedNode.GetValue("AdjustQtyUoM")).Equals(0.0))
				{
					focusedNode.SetValue("AdjustQtyUoM", Function.ParseDouble(focusedNode.GetValue("U_QtyUoM")));
				}
				double num = Math.Abs(Function.ParseDouble(focusedNode.GetValue("AdjustWidth")) * Function.ParseDouble(focusedNode.GetValue("AdjustLength")) * Function.ParseDouble(focusedNode.GetValue("AdjustQtyUoM")) / 100.0);
				double num2 = Math.Abs(Function.ParseDouble(focusedNode.GetValue("U_Width")) * Function.ParseDouble(focusedNode.GetValue("U_Length")) * Function.ParseDouble(focusedNode.GetValue("U_QtyUoM")) / 100.0);
				focusedNode.SetValue("AdjustQty", Math.Round(num - num2, 3, MidpointRounding.AwayFromZero));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_PrintTem_Click(object sender, EventArgs e)
	{
		try
		{
			string text = "";
			DataRow[] array = tv_ListPO.DataSourceTable().Select($"Checked='{true}' AND BatchNo <> ''");
			if (array.Length != 0)
			{
				DataRow[] array2 = array;
				foreach (DataRow dataRow in array2)
				{
					text = text + Function.ToString(dataRow["BatchNo"]) + ",";
				}
			}
			await PrintTem(text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task PrintTem(string batch)
	{
		if (!string.IsNullOrEmpty(batch))
		{
			if (batch.EndsWith(","))
			{
				batch = batch.Substring(0, batch.Length - 1);
			}
			List<Common.ParameterValue> lstParam = new List<Common.ParameterValue>
			{
				new Common.ParameterValue
				{
					Name = "batch",
					Value = batch
				}
			};
			string text = await WinFuntions.ReportViewer(Application.StartupPath + "\\Report\\Tem TP theo Batch.rpt", lstParam);
			if (!string.IsNullOrEmpty(text))
			{
				WinCommons.Error(Enums.MessageCode.None, text);
			}
		}
	}

	protected override void NoAuthSetControlSearch()
	{
		btn_Search.Enabled = true;
		txtFromDate.ReadOnly = false;
		txtToDate.ReadOnly = false;
	}

	private async void btn_Reject_Click(object sender, EventArgs e)
	{
		_ = 5;
		try
		{
			WinCommons._OpenProcessing();
			DataTable dataTable = tv_Request.DataSourceTable();
			if (!dataTable.IsNotNull())
			{
				return;
			}
			DataRow[] array = dataTable.Select("Checked=true And FatherId<>-1");
			if (array == null)
			{
				return;
			}
			string text = (array[0].IsExistsColumn("Stage1") ? Function.ToString(array[0]["Stage1"]) : "");
			if (text.Equals("N"))
			{
				return;
			}
			int punEntry = (array[0].IsExistsColumn("PunEntry") ? Function.ParseInt(array[0]["PunEntry"]) : (-1));
			if (punEntry <= 0)
			{
				return;
			}
			int soNo = (array[0].IsExistsColumn("U_SoNo") ? Function.ParseInt(array[0]["U_SoNo"]) : (-1));
			int soLine = (array[0].IsExistsColumn("U_SoLine") ? Function.ParseInt(array[0]["U_SoLine"]) : (-1));
			string remark1 = (array[0].IsExistsColumn("U_Remark1") ? Function.ToString(array[0]["U_Remark1"]) : "");
			string remark2 = (array[0].IsExistsColumn("U_Remark2") ? Function.ToString(array[0]["U_Remark2"]) : "");
			if (string.IsNullOrEmpty(text) || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
			{
				return;
			}
			new List<DocumentsImpl>();
			DocumentsImpl documentsImpl = new DocumentsImpl();
			documentsImpl.DocEntry = soNo;
			documentsImpl.UserName = "manager";
			DataTable tableDoc = await LoadOrder(documentsImpl);
			DocumentLineTable dt = await LoadOrderLines(soNo);
			new DocumentsImpl();
			DocumentsImpl documentsImpl2 = tableDoc.Rows[0].ToDocument();
			documentsImpl2.Lines = new List<Document_LinesImpl>();
			documentsImpl2.Lines = dt.ToDocumentLine(lstUdf);
			foreach (Document_LinesImpl line in documentsImpl2.Lines)
			{
				line.WhsCode = ((GlobalData.DefaultBranch == 1) ? "SW05" : "HW05");
			}
			string value = await ExtenseionMethods.SaveDocument(new Tuple<DocumentsImpl, ApzObjectTypes>(documentsImpl2, ApzObjectTypes.oOrders));
			if (!string.IsNullOrEmpty(value))
			{
				XtraMessageBox.Show(value, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			DataTable dataTable2 = await _httpClient.Udo.GetUdoByCode(new Common.Table_Code
			{
				Name = "OUDO",
				Code = "DPUN"
			});
			if (!dataTable2.IsNotNull())
			{
				WinCommons.Error(Enums.MessageCode.UdoTableIsInvalid, "");
				return;
			}
			DataSet ds = new DataSet();
			string tableName = Function.ToString(dataTable2.Rows[0]["TableName"]);
			ds.Tables.Add(dataTable2.Copy());
			DataTable dataTable3 = new DataTable(tableName);
			dataTable3.Columns.Add("DocEntry", typeof(int));
			dataTable3.Columns.Add("U_Stage1", typeof(string));
			dataTable3.Columns.Add("U_Stage2", typeof(string));
			dataTable3.Columns.Add("U_SoNo", typeof(string));
			dataTable3.Columns.Add("U_SoLine", typeof(string));
			dataTable3.Columns.Add("U_Remark1", typeof(string));
			dataTable3.Columns.Add("U_Remark2", typeof(string));
			dataTable3.Columns.Add("U_UserSign2", typeof(string));
			DataRow dataRow = dataTable3.NewRow();
			dataRow["DocEntry"] = punEntry;
			dataRow["U_Stage1"] = "Y";
			dataRow["U_Stage2"] = "N";
			dataRow["U_SoNo"] = soNo;
			dataRow["U_SoLine"] = soLine;
			dataRow["U_Remark1"] = remark1;
			dataRow["U_Remark2"] = remark2;
			dataTable3.Rows.Add(dataRow);
			ds.Tables.Add(dataTable3);
			HttpResult httpResult = await _httpClient.Udo.Save(ds);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
			}
			else
			{
				WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
				await LoadData();
			}
			ds.DisposeDataSet();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			WinCommons._CloseProcessing();
		}
	}

	private async Task<DocumentLineTable> LoadOrderLines(int docEntry)
	{
		DocumentLineTable lst = new DocumentLineTable();
		try
		{
			using HttpClientHelper<DocumentLineTable> docLineTableHelper = new HttpClientHelper<DocumentLineTable>(Configuration.ApiServer, "Sales");
			lst = await docLineTableHelper.GetAsync("GetLine", new
			{
				id = docEntry
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return lst;
	}

	private async Task<DataTable> LoadOrder(DocumentsImpl value)
	{
		DataTable dt = new DataTable();
		try
		{
			using HttpClientHelper<DataTable> docLineTableHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Sales");
			dt = await docLineTableHelper.PostAsync("Search", value);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return dt;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.MTO_Manufacturing.frm_ProductionReqForBatch));
		this.look_SalesEmp = new DevExpress.XtraEditors.LookUpEdit();
		this.bte_BPCode = new Apzon.Controls.Editor.ButtonEditEx();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.lb_SalesEmp = new Apzon.Controls.Controls.LabelLine();
		this.btn_Search = new Apzon.Controls.Controls.SimpleButtonEx();
		this.bte_SoNo = new Apzon.Controls.Editor.ButtonEditEx();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.bte_ItemCode = new Apzon.Controls.Editor.ButtonEditEx();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.xtraTab_ProductionControl = new Apzon.Controls.Controls.XtraTabControlEx();
		this.tab_Request = new Apzon.Controls.Controls.XtraTabPageEx();
		this.btn_Reject = new DevExpress.XtraEditors.SimpleButton();
		this.spin_Auto = new DevExpress.XtraEditors.SpinEdit();
		this.btn_ClearAll = new Apzon.Controls.Controls.SimpleButtonEx();
		this.chk_Auto = new Apzon.Controls.Editor.CheckEditEx();
		this.btn_SelectAll = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_ProductionPlan = new Apzon.Controls.Controls.SimpleButtonEx();
		this.tv_Request = new Apzon.Controls.Tree.TreeEx();
		this.tab_Report = new Apzon.Controls.Controls.XtraTabPageEx();
		this.btn_ViewDetail = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_Planning = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Planning = new Apzon.Controls.Grid.GridViewEx();
		this.tab_ListPro = new Apzon.Controls.Controls.XtraTabPageEx();
		this.btn_PrintTem = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Adjust = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_ViewDetail2 = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_DelRequest = new Apzon.Controls.Controls.SimpleButtonEx();
		this.tv_ListPO = new Apzon.Controls.Tree.TreeEx();
		this.timer_AutoSearch = new System.Windows.Forms.Timer(this.components);
		this.lb_ToDate = new Apzon.Controls.Controls.LabelLine();
		this.lb_FromDate = new Apzon.Controls.Controls.LabelLine();
		this.txtFromDate = new Apzon.Controls.Editor.DateEditEx();
		this.txtToDate = new Apzon.Controls.Editor.DateEditEx();
		this.chb_Whs = new DevExpress.XtraEditors.CheckedComboBoxEdit();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		((System.ComponentModel.ISupportInitialize)this.look_SalesEmp.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bte_BPCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bte_SoNo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bte_ItemCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTab_ProductionControl).BeginInit();
		this.xtraTab_ProductionControl.SuspendLayout();
		this.tab_Request.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.spin_Auto.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Auto.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tv_Request).BeginInit();
		this.tab_Report.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Planning).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Planning).BeginInit();
		this.tab_ListPro.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.tv_ListPO).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtFromDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtFromDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtToDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtToDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chb_Whs.Properties).BeginInit();
		base.SuspendLayout();
		this.look_SalesEmp.Location = new System.Drawing.Point(86, 29);
		this.look_SalesEmp.Name = "look_SalesEmp";
		this.look_SalesEmp.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.look_SalesEmp.Properties.NullText = "";
		this.look_SalesEmp.Size = new System.Drawing.Size(107, 20);
		this.look_SalesEmp.TabIndex = 46;
		this.bte_BPCode.FieldName = null;
		this.bte_BPCode.IsSearchControl = false;
		this.bte_BPCode.Location = new System.Drawing.Point(274, 31);
		this.bte_BPCode.Name = "bte_BPCode";
		this.bte_BPCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.bte_BPCode.Properties.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.bte_BPCode_Properties_ButtonClick);
		this.bte_BPCode.Size = new System.Drawing.Size(107, 20);
		this.bte_BPCode.TabIndex = 42;
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(199, 32);
		this.labelLine1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(76, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 41;
		this.labelLine1.Utext = "BP Code";
		this.lb_SalesEmp.FieldName = null;
		this.lb_SalesEmp.HideWhenEmpty = false;
		this.lb_SalesEmp.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_SalesEmp.Location = new System.Drawing.Point(7, 32);
		this.lb_SalesEmp.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_SalesEmp.Name = "lb_SalesEmp";
		this.lb_SalesEmp.ShowUnderLine = true;
		this.lb_SalesEmp.Size = new System.Drawing.Size(80, 20);
		this.lb_SalesEmp.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_SalesEmp.TabIndex = 40;
		this.lb_SalesEmp.Utext = "Sales Emp";
		this.btn_Search.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Search.Appearance.Options.UseBackColor = true;
		this.btn_Search.FieldName = null;
		this.btn_Search.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Search.ImageOptions.Image");
		this.btn_Search.IsSearchControl = false;
		this.btn_Search.Location = new System.Drawing.Point(667, 28);
		this.btn_Search.Name = "btn_Search";
		this.btn_Search.Size = new System.Drawing.Size(100, 23);
		this.btn_Search.TabIndex = 39;
		this.btn_Search.Text = "Search";
		this.btn_Search.Click += new System.EventHandler(this.btn_Search_Click);
		this.bte_SoNo.FieldName = null;
		this.bte_SoNo.IsSearchControl = false;
		this.bte_SoNo.Location = new System.Drawing.Point(86, 3);
		this.bte_SoNo.Name = "bte_SoNo";
		this.bte_SoNo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.bte_SoNo.Properties.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.bte_BPCode_Properties_ButtonClick);
		this.bte_SoNo.Size = new System.Drawing.Size(107, 20);
		this.bte_SoNo.TabIndex = 48;
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(7, 6);
		this.labelLine2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(80, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 47;
		this.labelLine2.Utext = "SO No.";
		this.bte_ItemCode.FieldName = null;
		this.bte_ItemCode.IsSearchControl = false;
		this.bte_ItemCode.Location = new System.Drawing.Point(274, 6);
		this.bte_ItemCode.Name = "bte_ItemCode";
		this.bte_ItemCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.bte_ItemCode.Properties.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.bte_BPCode_Properties_ButtonClick);
		this.bte_ItemCode.Size = new System.Drawing.Size(107, 20);
		this.bte_ItemCode.TabIndex = 50;
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(199, 6);
		this.labelLine3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(76, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 49;
		this.labelLine3.Utext = "ItemCode";
		this.xtraTab_ProductionControl.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.xtraTab_ProductionControl.IsSearchControl = false;
		this.xtraTab_ProductionControl.Location = new System.Drawing.Point(-1, 59);
		this.xtraTab_ProductionControl.Name = "xtraTab_ProductionControl";
		this.xtraTab_ProductionControl.SelectedTabPage = this.tab_Request;
		this.xtraTab_ProductionControl.Size = new System.Drawing.Size(882, 443);
		this.xtraTab_ProductionControl.TabIndex = 51;
		this.xtraTab_ProductionControl.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[3] { this.tab_Request, this.tab_Report, this.tab_ListPro });
		this.xtraTab_ProductionControl.SelectedPageChanged += new DevExpress.XtraTab.TabPageChangedEventHandler(this.xtraTab_ProductionControl_SelectedPageChanged);
		this.tab_Request.Controls.Add(this.btn_Reject);
		this.tab_Request.Controls.Add(this.spin_Auto);
		this.tab_Request.Controls.Add(this.btn_ClearAll);
		this.tab_Request.Controls.Add(this.chk_Auto);
		this.tab_Request.Controls.Add(this.btn_SelectAll);
		this.tab_Request.Controls.Add(this.btn_ProductionPlan);
		this.tab_Request.Controls.Add(this.tv_Request);
		this.tab_Request.FieldName = null;
		this.tab_Request.IsSearchControl = false;
		this.tab_Request.Name = "tab_Request";
		this.tab_Request.Size = new System.Drawing.Size(880, 414);
		this.tab_Request.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Request.Text = "Production Request";
		this.btn_Reject.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Reject.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Reject.ImageOptions.Image");
		this.btn_Reject.Location = new System.Drawing.Point(301, 388);
		this.btn_Reject.Name = "btn_Reject";
		this.btn_Reject.Size = new System.Drawing.Size(154, 23);
		this.btn_Reject.TabIndex = 55;
		this.btn_Reject.Text = "Reject Fast Response";
		this.btn_Reject.Click += new System.EventHandler(this.btn_Reject_Click);
		this.spin_Auto.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.spin_Auto.EditValue = new decimal(new int[4] { 10, 0, 0, 0 });
		this.spin_Auto.Location = new System.Drawing.Point(555, 389);
		this.spin_Auto.Name = "spin_Auto";
		this.spin_Auto.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.spin_Auto.Properties.DisplayFormat.FormatString = "{0:n0} Minute";
		this.spin_Auto.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
		this.spin_Auto.Properties.MaxValue = new decimal(new int[4] { 60, 0, 0, 0 });
		this.spin_Auto.Properties.MinValue = new decimal(new int[4] { 1, 0, 0, 0 });
		this.spin_Auto.Size = new System.Drawing.Size(91, 20);
		this.spin_Auto.TabIndex = 53;
		this.spin_Auto.EditValueChanged += new System.EventHandler(this.spin_Auto_EditValueChanged);
		this.btn_ClearAll.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_ClearAll.FieldName = null;
		this.btn_ClearAll.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_ClearAll.ImageOptions.Image");
		this.btn_ClearAll.IsSearchControl = false;
		this.btn_ClearAll.Location = new System.Drawing.Point(769, 389);
		this.btn_ClearAll.Name = "btn_ClearAll";
		this.btn_ClearAll.Size = new System.Drawing.Size(104, 20);
		this.btn_ClearAll.TabIndex = 54;
		this.btn_ClearAll.Text = "Clear All";
		this.btn_ClearAll.Click += new System.EventHandler(this.btn_ClearAll_Click);
		this.chk_Auto.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.chk_Auto.FieldName = null;
		this.chk_Auto.IsSearchControl = false;
		this.chk_Auto.Location = new System.Drawing.Point(461, 390);
		this.chk_Auto.Name = "chk_Auto";
		this.chk_Auto.Properties.Caption = "Auto Refresh";
		this.chk_Auto.Size = new System.Drawing.Size(88, 20);
		this.chk_Auto.TabIndex = 52;
		this.chk_Auto.CheckedChanged += new System.EventHandler(this.chk_Auto_CheckedChanged);
		this.btn_SelectAll.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_SelectAll.FieldName = null;
		this.btn_SelectAll.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_SelectAll.ImageOptions.Image");
		this.btn_SelectAll.IsSearchControl = false;
		this.btn_SelectAll.Location = new System.Drawing.Point(659, 389);
		this.btn_SelectAll.Name = "btn_SelectAll";
		this.btn_SelectAll.Size = new System.Drawing.Size(104, 20);
		this.btn_SelectAll.TabIndex = 53;
		this.btn_SelectAll.Text = "Select All";
		this.btn_SelectAll.Click += new System.EventHandler(this.btn_SelectAll_Click);
		this.btn_ProductionPlan.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_ProductionPlan.Enabled = false;
		this.btn_ProductionPlan.FieldName = null;
		this.btn_ProductionPlan.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_ProductionPlan.ImageOptions.Image");
		this.btn_ProductionPlan.IsSearchControl = false;
		this.btn_ProductionPlan.Location = new System.Drawing.Point(3, 392);
		this.btn_ProductionPlan.Name = "btn_ProductionPlan";
		this.btn_ProductionPlan.Size = new System.Drawing.Size(104, 20);
		this.btn_ProductionPlan.TabIndex = 52;
		this.btn_ProductionPlan.Text = "Production Plan";
		this.btn_ProductionPlan.Click += new System.EventHandler(this.btn_ProductionPlan_Click);
		this.tv_Request.AllowPasteExcel = false;
		this.tv_Request.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.tv_Request.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.tv_Request.ColumnKey = null;
		this.tv_Request.DisableFormSetting = false;
		this.tv_Request.EditColumn = null;
		this.tv_Request.FieldCurrency = null;
		this.tv_Request.FieldRate = null;
		this.tv_Request.FormId = "oProductionControl";
		this.tv_Request.IndexEditRow = -1;
		this.tv_Request.IndicatorWidth = 40;
		this.tv_Request.IsApzTable = false;
		this.tv_Request.IsLoadingData = false;
		this.tv_Request.ItemId = "OREQUEST";
		this.tv_Request.Location = new System.Drawing.Point(0, 0);
		this.tv_Request.LstColumnNotEditable = null;
		this.tv_Request.LstColumNotSetting = null;
		this.tv_Request.Name = "tv_Request";
		this.tv_Request.OptionsFilter.ShowAllValuesInFilterPopup = true;
		this.tv_Request.OptionsView.ShowAutoFilterRow = true;
		this.tv_Request.Size = new System.Drawing.Size(876, 386);
		this.tv_Request.TabIndex = 0;
		this.tv_Request.CellValueChanging += new DevExpress.XtraTreeList.CellValueChangedEventHandler(this.tv_Request_CellValueChanging);
		this.tv_Request.CellValueChanged += new DevExpress.XtraTreeList.CellValueChangedEventHandler(this.tv_Request_CellValueChanged);
		this.tab_Report.Controls.Add(this.btn_ViewDetail);
		this.tab_Report.Controls.Add(this.grd_Planning);
		this.tab_Report.FieldName = null;
		this.tab_Report.IsSearchControl = false;
		this.tab_Report.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.tab_Report.Name = "tab_Report";
		this.tab_Report.Size = new System.Drawing.Size(880, 414);
		this.tab_Report.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Report.Text = "List of Production Planning";
		this.btn_ViewDetail.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_ViewDetail.FieldName = null;
		this.btn_ViewDetail.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_ViewDetail.ImageOptions.Image");
		this.btn_ViewDetail.IsSearchControl = false;
		this.btn_ViewDetail.Location = new System.Drawing.Point(781, 389);
		this.btn_ViewDetail.Name = "btn_ViewDetail";
		this.btn_ViewDetail.Size = new System.Drawing.Size(92, 23);
		this.btn_ViewDetail.TabIndex = 1;
		this.btn_ViewDetail.Text = "View Details";
		this.btn_ViewDetail.Click += new System.EventHandler(this.btn_ViewDetail_Click);
		this.grd_Planning.AllowPasteExcel = false;
		this.grd_Planning.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Planning.ClipboardData = null;
		this.grd_Planning.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grd_Planning.Location = new System.Drawing.Point(0, 0);
		this.grd_Planning.MainView = this.gv_Planning;
		this.grd_Planning.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grd_Planning.Name = "grd_Planning";
		this.grd_Planning.Size = new System.Drawing.Size(876, 384);
		this.grd_Planning.TabIndex = 0;
		this.grd_Planning.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Planning });
		this.gv_Planning.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Planning.ColumnKey = null;
		this.gv_Planning.DisableFormSetting = false;
		this.gv_Planning.DisplayException = false;
		this.gv_Planning.EditColumn = null;
		this.gv_Planning.FieldCurrency = null;
		this.gv_Planning.FieldRate = null;
		this.gv_Planning.FormId = "oProductionControl";
		this.gv_Planning.GridControl = this.grd_Planning;
		this.gv_Planning.IndexEditRow = -1;
		this.gv_Planning.IndicatorWidth = 35;
		this.gv_Planning.IsApzTable = false;
		this.gv_Planning.IsInsertNewRow = false;
		this.gv_Planning.IsLoadingData = false;
		this.gv_Planning.ItemId = "OPLANNING";
		this.gv_Planning.LstColumnAllowBatch = null;
		this.gv_Planning.LstColumnNotAllowBatch = null;
		this.gv_Planning.LstColumnNotEditable = null;
		this.gv_Planning.LstColumNotSetting = null;
		this.gv_Planning.Name = "gv_Planning";
		this.gv_Planning.UseBatchMod = false;
		this.tab_ListPro.Controls.Add(this.btn_PrintTem);
		this.tab_ListPro.Controls.Add(this.btn_Adjust);
		this.tab_ListPro.Controls.Add(this.btn_ViewDetail2);
		this.tab_ListPro.Controls.Add(this.btn_DelRequest);
		this.tab_ListPro.Controls.Add(this.tv_ListPO);
		this.tab_ListPro.FieldName = null;
		this.tab_ListPro.IsSearchControl = false;
		this.tab_ListPro.Name = "tab_ListPro";
		this.tab_ListPro.Size = new System.Drawing.Size(880, 414);
		this.tab_ListPro.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_ListPro.Text = "List of Production Orders";
		this.btn_PrintTem.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_PrintTem.FieldName = null;
		this.btn_PrintTem.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_PrintTem.ImageOptions.Image");
		this.btn_PrintTem.IsSearchControl = false;
		this.btn_PrintTem.Location = new System.Drawing.Point(681, 390);
		this.btn_PrintTem.Name = "btn_PrintTem";
		this.btn_PrintTem.Size = new System.Drawing.Size(75, 23);
		this.btn_PrintTem.TabIndex = 5;
		this.btn_PrintTem.Text = "In Tem";
		this.btn_PrintTem.Click += new System.EventHandler(this.btn_PrintTem_Click);
		this.btn_Adjust.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Adjust.FieldName = null;
		this.btn_Adjust.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Adjust.ImageOptions.Image");
		this.btn_Adjust.IsSearchControl = false;
		this.btn_Adjust.Location = new System.Drawing.Point(583, 390);
		this.btn_Adjust.Name = "btn_Adjust";
		this.btn_Adjust.Size = new System.Drawing.Size(92, 23);
		this.btn_Adjust.TabIndex = 4;
		this.btn_Adjust.Text = "Adjust";
		this.btn_Adjust.Click += new System.EventHandler(this.btn_Adjust_Click);
		this.btn_ViewDetail2.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_ViewDetail2.FieldName = null;
		this.btn_ViewDetail2.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_ViewDetail2.ImageOptions.Image");
		this.btn_ViewDetail2.IsSearchControl = false;
		this.btn_ViewDetail2.Location = new System.Drawing.Point(487, 390);
		this.btn_ViewDetail2.Name = "btn_ViewDetail2";
		this.btn_ViewDetail2.Size = new System.Drawing.Size(92, 23);
		this.btn_ViewDetail2.TabIndex = 3;
		this.btn_ViewDetail2.Text = "View Details";
		this.btn_ViewDetail2.Click += new System.EventHandler(this.btn_ViewDetail2_Click);
		this.btn_DelRequest.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_DelRequest.FieldName = null;
		this.btn_DelRequest.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_DelRequest.ImageOptions.Image");
		this.btn_DelRequest.IsSearchControl = false;
		this.btn_DelRequest.Location = new System.Drawing.Point(762, 389);
		this.btn_DelRequest.Name = "btn_DelRequest";
		this.btn_DelRequest.Size = new System.Drawing.Size(111, 23);
		this.btn_DelRequest.TabIndex = 2;
		this.btn_DelRequest.Text = "Delivery Request";
		this.btn_DelRequest.Click += new System.EventHandler(this.btn_DelRequest_Click);
		this.tv_ListPO.AllowPasteExcel = false;
		this.tv_ListPO.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.tv_ListPO.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.tv_ListPO.ColumnKey = null;
		this.tv_ListPO.DisableFormSetting = false;
		this.tv_ListPO.EditColumn = null;
		this.tv_ListPO.FieldCurrency = null;
		this.tv_ListPO.FieldRate = null;
		this.tv_ListPO.FormId = "oProductionControl";
		this.tv_ListPO.IndexEditRow = -1;
		this.tv_ListPO.IndicatorWidth = 40;
		this.tv_ListPO.IsApzTable = false;
		this.tv_ListPO.IsLoadingData = false;
		this.tv_ListPO.ItemId = "OPRODUCTION";
		this.tv_ListPO.Location = new System.Drawing.Point(-1, 3);
		this.tv_ListPO.LstColumnNotEditable = null;
		this.tv_ListPO.LstColumNotSetting = null;
		this.tv_ListPO.Name = "tv_ListPO";
		this.tv_ListPO.Size = new System.Drawing.Size(877, 380);
		this.tv_ListPO.TabIndex = 1;
		this.tv_ListPO.NodeCellStyle += new DevExpress.XtraTreeList.GetCustomNodeCellStyleEventHandler(this.tv_ListPO_NodeCellStyle);
		this.tv_ListPO.CellValueChanging += new DevExpress.XtraTreeList.CellValueChangedEventHandler(this.tv_ListPO_CellValueChanging);
		this.tv_ListPO.CellValueChanged += new DevExpress.XtraTreeList.CellValueChangedEventHandler(this.tv_ListPO_CellValueChanged);
		this.tv_ListPO.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.tv_ListPO_ShowingEditor);
		this.timer_AutoSearch.Enabled = true;
		this.timer_AutoSearch.Tick += new System.EventHandler(this.timer_AutoSearch_Tick);
		this.lb_ToDate.FieldName = null;
		this.lb_ToDate.HideWhenEmpty = false;
		this.lb_ToDate.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_ToDate.Location = new System.Drawing.Point(395, 31);
		this.lb_ToDate.Name = "lb_ToDate";
		this.lb_ToDate.ShowUnderLine = true;
		this.lb_ToDate.Size = new System.Drawing.Size(80, 20);
		this.lb_ToDate.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_ToDate.TabIndex = 55;
		this.lb_ToDate.Utext = "To Date";
		this.lb_FromDate.FieldName = null;
		this.lb_FromDate.HideWhenEmpty = false;
		this.lb_FromDate.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_FromDate.Location = new System.Drawing.Point(395, 6);
		this.lb_FromDate.Name = "lb_FromDate";
		this.lb_FromDate.ShowUnderLine = true;
		this.lb_FromDate.Size = new System.Drawing.Size(80, 20);
		this.lb_FromDate.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_FromDate.TabIndex = 54;
		this.lb_FromDate.Utext = "From Date";
		this.txtFromDate.EditValue = null;
		this.txtFromDate.FieldName = null;
		this.txtFromDate.Location = new System.Drawing.Point(475, 4);
		this.txtFromDate.Name = "txtFromDate";
		this.txtFromDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtFromDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtFromDate.Properties.IsSearchControl = false;
		this.txtFromDate.Size = new System.Drawing.Size(117, 20);
		this.txtFromDate.TabIndex = 56;
		this.txtToDate.EditValue = null;
		this.txtToDate.FieldName = null;
		this.txtToDate.Location = new System.Drawing.Point(475, 29);
		this.txtToDate.Name = "txtToDate";
		this.txtToDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtToDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtToDate.Properties.IsSearchControl = false;
		this.txtToDate.Size = new System.Drawing.Size(117, 20);
		this.txtToDate.TabIndex = 57;
		this.chb_Whs.Location = new System.Drawing.Point(667, 4);
		this.chb_Whs.Name = "chb_Whs";
		this.chb_Whs.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.chb_Whs.Size = new System.Drawing.Size(209, 20);
		this.chb_Whs.TabIndex = 60;
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(598, 3);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(63, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 61;
		this.labelLine4.Utext = "Warehouse";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(882, 503);
		base.Controls.Add(this.labelLine4);
		base.Controls.Add(this.chb_Whs);
		base.Controls.Add(this.txtToDate);
		base.Controls.Add(this.txtFromDate);
		base.Controls.Add(this.lb_ToDate);
		base.Controls.Add(this.lb_FromDate);
		base.Controls.Add(this.xtraTab_ProductionControl);
		base.Controls.Add(this.bte_ItemCode);
		base.Controls.Add(this.labelLine3);
		base.Controls.Add(this.bte_SoNo);
		base.Controls.Add(this.labelLine2);
		base.Controls.Add(this.look_SalesEmp);
		base.Controls.Add(this.bte_BPCode);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.lb_SalesEmp);
		base.Controls.Add(this.btn_Search);
		base.FormId = "oProductionControl";
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_ProductionReqForBatch.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_ProductionReqForBatch";
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Production Controls";
		base.Load += new System.EventHandler(this.frm_ProductionReqForBatch_Load);
		base.Shown += new System.EventHandler(this.frm_ProductionReqForBatch_Shown);
		((System.ComponentModel.ISupportInitialize)this.look_SalesEmp.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bte_BPCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bte_SoNo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bte_ItemCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTab_ProductionControl).EndInit();
		this.xtraTab_ProductionControl.ResumeLayout(false);
		this.tab_Request.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.spin_Auto.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Auto.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tv_Request).EndInit();
		this.tab_Report.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Planning).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Planning).EndInit();
		this.tab_ListPro.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.tv_ListPO).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtFromDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtFromDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtToDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtToDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chb_Whs.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
