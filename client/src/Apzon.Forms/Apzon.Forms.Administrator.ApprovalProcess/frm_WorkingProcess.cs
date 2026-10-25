using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Editor.TextEditExPrice;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;
using Apzon.Entities.Models.Production;
using Apzon.Forms.Commons;
using Apzon.Forms.Helper;
using Apzon.Forms.Properties;
using Apzon.Forms.UserControls;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Tile;
using DevExpress.XtraNavBar;
using DevExpress.XtraNavBar.ViewInfo;

namespace Apzon.Forms.Administrator.ApprovalProcess;

public class frm_WorkingProcess : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private readonly Font _defaultFont = new Font("Segoe UI", 8.25f, FontStyle.Regular);

	private int _defaultUser = -1;

	private DataTable _dtSource = new DataTable();

	private DataTable _dtFilterConds = new DataTable();

	private DataTable _dtListFilter = new DataTable();

	private string _draftNoText = string.Empty;

	private string _draftTypeText = string.Empty;

	private string _docNoText = string.Empty;

	private string _docTypeText = string.Empty;

	public string CurrentRequest = string.Empty;

	public DataTable DtFormSetting = new DataTable();

	private IContainer components;

	private SplitContainerControl splitContainerControl1;

	private PanelControl panelControl2;

	private SplitContainerControl splitContainerControl2;

	private DateEditEx dtp_CreateDate;

	private ButtonEditCFL btn_DocNo;

	private LabelLine lbl_DraftType;

	private LabelLine lbl_DraftNo;

	private LabelLine lbl_Template;

	private LabelLine lbl_Currency;

	private LabelLine lbl_CreateDate;

	private LabelLine lbl_UserRequest;

	private GroupControl grp_ApprovalStages;

	private GridControlEx grd_TemplateDetails;

	private GridViewEx gv_TemplatesDetail;

	private PanelControl pnl_Function;

	private SimpleButtonEx btn_Approval;

	private PanelControl panelControl3;

	private GridColumnEx gc_WtmCode;

	private GridColumnEx gc_AuthorizerID;

	private GridColumnEx gc_AuthorizerName;

	private GridColumnEx gc_Status;

	private GridColumnEx gc_DateUpdate;

	private GridColumnEx gc_WstCode;

	private GridColumnEx gc_WstName;

	private GridColumnEx gc_TimeUpdate;

	private GridColumnEx gc_Remarks;

	private SimpleButtonEx btn_Reject;

	private RepositoryItemTextEdit rep_StatusApproved;

	private GridColumnEx gc_WddCode;

	private RepositoryItemButtonEditCFL rep_DateRequest;

	private TextEditExPrice txt_Total;

	private LabelLine lbl_Remarks;

	private NavBarControl nav_Menu;

	private NavBarGroup nbg_Status;

	private NavBarItem nbi_All;

	private NavBarGroup nbg_Process;

	private NavBarGroup nbg_Document;

	private TextEditExPrice txt_Price;

	private TextEditEx txt_TemplateName;

	private TextEditEx txt_DocType;

	private TextEditEx txt_Currency;

	private TextEditEx txt_UserRequest;

	private MemoExEditEx txt_Remarks;

	private NavBarGroup nbg_ApprovalDocs;

	private NavBarItem nbi_ApprovalRequest;

	private NavBarItem nbi_MyRequest;

	private NavBarItem nbi_Approved;

	private NavBarItem nbi_Rejected;

	private NavBarItem nbi_Pending;

	private SimpleButtonEx btn_Save;

	private ButtonEditEx btn_DocNo1;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx2;

	private ImageCollection img_Lists;

	private TileViewPagingEx uc_Templates;

	private NavBarItem nbi_Created;

	private GridColumnEx gc_SortId;

	private LabelLine lbl_Total;

	private LabelLine lbl_DocType;

	private LabelLine lbl_DocNo;

	private GridColumnEx gc_Pending;

	private GridColumnEx gc_Approved;

	private GridColumnEx gc_Rejected;

	private GridColumnEx gc_NotStarted;

	private NavBarItem navBarItem1;

	public frm_WorkingProcess()
	{
		InitializeComponent();
		WindowState = FormWindowState.Maximized;
		OnLoadAsync += frm_WorkingProcess_Load;
		uc_Templates.TileView.FocusedRowChanged += TileView_FocusedRowChanged;
		uc_Templates.RequestTypeControl.EditValueChanged += RequestTypeSearch_EditValueChanged;
		uc_Templates.TileView.ItemClick += TileView_ItemClick;
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			uc_Templates.RequestTypeControl.ItemIndex = 0;
			uc_Templates.SearchTypeControl.ItemIndex = 0;
			uc_Templates.TextSearchControl.Text = string.Empty;
			SetDefaultFormatMenu("A");
			LoadGeneral("P");
			if (!uc_Templates.GridControl.DataSourceTable.IsNotNull())
			{
				LoadGeneral("M");
			}
			ReloadFormSetting(DtFormSetting);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public new async Task FORMSETTING()
	{
		await ShowFormSetting(this);
	}

	private async Task ShowFormSetting(XtraFormEx form)
	{
		try
		{
			string value = Function.ToString(ApzObjectTypes.oWorkingProcess);
			frm_FormSetting frm_FormSetting2 = new frm_FormSetting
			{
				BaseForm = this
			};
			List<GridColumn> list = new List<GridColumn>
			{
				uc_Templates.Columns["ItemCheck"],
				uc_Templates.Columns["WddCode"],
				uc_Templates.Columns["WtmCode"],
				uc_Templates.Columns["WtmName"],
				uc_Templates.Columns["UserID"],
				uc_Templates.Columns["CardCode"],
				uc_Templates.Columns["DocCur"],
				uc_Templates.Columns["UpdateTime"]
			};
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("FormId", typeof(string));
			dataTable.Columns.Add("ItemId", typeof(string));
			dataTable.Columns.Add("ColumnId", typeof(string));
			dataTable.Columns.Add("ColumnName", typeof(string));
			dataTable.Columns.Add("UserSign", typeof(string));
			dataTable.Columns.Add("Width", typeof(int));
			dataTable.Columns.Add("Visible", typeof(bool));
			dataTable.Columns.Add("VisIndex", typeof(int));
			dataTable.Columns.Add("Editable", typeof(bool));
			dataTable.Columns.Add("IsReadOnly", typeof(bool));
			dataTable.Columns.Add("DataType", typeof(string));
			dataTable.Columns.Add("Fixed", typeof(string));
			dataTable.Columns.Add("AllowChSet", typeof(bool));
			dataTable.Columns.Add("VisSetting", typeof(bool));
			dataTable.Columns.Add("IsSystem", typeof(string));
			dataTable.Columns.Add("LngCode", typeof(string));
			dataTable.Columns.Add("NeverEditable", typeof(bool));
			dataTable.Columns.Add("GroupIndex", typeof(int));
			dataTable.Columns.Add("CurrField", typeof(string));
			foreach (NavBarGroup group in nav_Menu.Groups)
			{
				if (group.Name.StartsWith("nbg_"))
				{
					DataRow dataRow = dataTable.NewRow();
					dataRow["FormId"] = form.FormId;
					dataRow["ItemId"] = value;
					dataRow["ColumnId"] = group.Name;
					dataRow["ColumnName"] = group.Caption;
					dataRow["UserSign"] = GlobalData.UserName;
					dataRow["Width"] = 75;
					dataRow["Visible"] = group.Visible;
					dataRow["VisIndex"] = 0;
					dataRow["Editable"] = false;
					dataRow["NeverEditable"] = true;
					dataRow["IsReadOnly"] = true;
					dataRow["DataType"] = "string";
					dataRow["Fixed"] = "N";
					dataRow["AllowChSet"] = 1;
					dataRow["VisSetting"] = true;
					dataRow["IsSystem"] = "Y";
					dataRow["LngCode"] = "-";
					dataRow["GroupIndex"] = -1;
					dataTable.Rows.Add(dataRow);
				}
			}
			foreach (NavBarGroup group2 in nav_Menu.Groups)
			{
				if (group2.Name.Equals(nbg_Process.Name) || group2.Name.Equals(nbg_Document.Name))
				{
					continue;
				}
				foreach (NavBarItemLink itemLink in group2.ItemLinks)
				{
					DataRow dataRow2 = dataTable.NewRow();
					dataRow2["FormId"] = form.FormId;
					dataRow2["ItemId"] = value;
					dataRow2["ColumnId"] = itemLink.Item.Name;
					dataRow2["ColumnName"] = itemLink.Item.Caption;
					dataRow2["UserSign"] = GlobalData.UserName;
					dataRow2["Width"] = 75;
					dataRow2["Visible"] = itemLink.Item.Visible;
					dataRow2["VisIndex"] = 0;
					dataRow2["Editable"] = false;
					dataRow2["NeverEditable"] = true;
					dataRow2["IsReadOnly"] = true;
					dataRow2["DataType"] = "string";
					dataRow2["Fixed"] = "N";
					dataRow2["AllowChSet"] = 1;
					dataRow2["VisSetting"] = true;
					dataRow2["IsSystem"] = "Y";
					dataRow2["LngCode"] = "-";
					dataRow2["GroupIndex"] = -1;
					dataTable.Rows.Add(dataRow2);
				}
			}
			foreach (TileViewColumn column in uc_Templates.Columns)
			{
				if (!list.Contains(column))
				{
					DataRow dataRow3 = dataTable.NewRow();
					dataRow3["FormId"] = form.FormId;
					dataRow3["ItemId"] = value;
					dataRow3["ColumnId"] = column.Name;
					dataRow3["ColumnName"] = column.Caption;
					dataRow3["UserSign"] = GlobalData.UserName;
					dataRow3["Width"] = column.Width;
					dataRow3["Visible"] = column.Visible;
					dataRow3["VisIndex"] = column.AbsoluteIndex;
					dataRow3["Editable"] = false;
					dataRow3["NeverEditable"] = true;
					dataRow3["IsReadOnly"] = column.OptionsColumn.ReadOnly;
					dataRow3["DataType"] = Function.ToString(column.ColumnType);
					DataRow dataRow4 = dataRow3;
					string value2;
					if (column.Fixed == FixedStyle.Left)
					{
						value2 = "L";
					}
					else
					{
						value2 = ((column.Fixed == FixedStyle.Right) ? "R" : "N");
					}
					dataRow4["Fixed"] = value2;
					dataRow3["AllowChSet"] = 1;
					dataRow3["VisSetting"] = true;
					dataRow3["IsSystem"] = "Y";
					dataRow3["LngCode"] = "-";
					dataRow3["GroupIndex"] = -1;
					dataTable.Rows.Add(dataRow3);
				}
			}
			foreach (Control control3 in splitContainerControl2.Panel1.Controls)
			{
				if (control3 is LabelLine)
				{
					LabelLine labelLine = control3 as LabelLine;
					DataRow dataRow5 = dataTable.NewRow();
					dataRow5["FormId"] = form.FormId;
					dataRow5["ItemId"] = value;
					dataRow5["ColumnId"] = labelLine.Name;
					dataRow5["ColumnName"] = labelLine.Utext;
					dataRow5["UserSign"] = GlobalData.UserName;
					dataRow5["Width"] = labelLine.Width;
					dataRow5["Visible"] = labelLine.Visible;
					dataRow5["Editable"] = false;
					dataRow5["NeverEditable"] = true;
					dataRow5["IsReadOnly"] = true;
					dataRow5["DataType"] = "string";
					dataRow5["Fixed"] = "N";
					dataRow5["AllowChSet"] = 1;
					dataRow5["VisSetting"] = true;
					dataRow5["IsSystem"] = "Y";
					dataRow5["LngCode"] = "-";
					dataRow5["GroupIndex"] = -1;
					dataTable.Rows.Add(dataRow5);
				}
			}
			foreach (Control control4 in pnl_Function.Controls)
			{
				if (control4 is SimpleButtonEx)
				{
					SimpleButtonEx simpleButtonEx = control4 as SimpleButtonEx;
					DataRow dataRow6 = dataTable.NewRow();
					dataRow6["FormId"] = form.FormId;
					dataRow6["ItemId"] = value;
					dataRow6["ColumnId"] = simpleButtonEx.Name;
					dataRow6["ColumnName"] = simpleButtonEx.Text;
					dataRow6["UserSign"] = GlobalData.UserName;
					dataRow6["Width"] = simpleButtonEx.Width;
					dataRow6["Visible"] = simpleButtonEx.Visible;
					dataRow6["Editable"] = false;
					dataRow6["NeverEditable"] = true;
					dataRow6["IsReadOnly"] = true;
					dataRow6["DataType"] = "string";
					dataRow6["Fixed"] = "N";
					dataRow6["AllowChSet"] = 1;
					dataRow6["VisSetting"] = true;
					dataRow6["IsSystem"] = "Y";
					dataRow6["LngCode"] = "-";
					dataRow6["GroupIndex"] = -1;
					dataTable.Rows.Add(dataRow6);
				}
			}
			DataRow dataRow7 = dataTable.NewRow();
			dataRow7["FormId"] = form.FormId;
			dataRow7["ItemId"] = value;
			dataRow7["ColumnId"] = grp_ApprovalStages.Name;
			dataRow7["ColumnName"] = grp_ApprovalStages.Text;
			dataRow7["UserSign"] = GlobalData.UserName;
			dataRow7["Width"] = grp_ApprovalStages.Width;
			dataRow7["Visible"] = grp_ApprovalStages.Visible;
			dataRow7["Editable"] = false;
			dataRow7["NeverEditable"] = true;
			dataRow7["IsReadOnly"] = true;
			dataRow7["DataType"] = "string";
			dataRow7["Fixed"] = "N";
			dataRow7["AllowChSet"] = 1;
			dataRow7["VisSetting"] = true;
			dataRow7["IsSystem"] = "Y";
			dataRow7["LngCode"] = "-";
			dataRow7["GroupIndex"] = -1;
			dataTable.Rows.Add(dataRow7);
			foreach (GridColumnEx item in gv_TemplatesDetail.ColumnsEx)
			{
				if (item.Name.Equals(gc_Approved.Name) || item.Name.Equals(gc_Rejected.Name) || item.Name.Equals(gc_Pending.Name) || item.Name.Equals(gc_NotStarted.Name))
				{
					DataRow dataRow8 = dataTable.NewRow();
					dataRow8["FormId"] = form.FormId;
					dataRow8["ItemId"] = value;
					dataRow8["ColumnId"] = item.Name;
					dataRow8["ColumnName"] = item.Caption;
					dataRow8["UserSign"] = GlobalData.UserName;
					dataRow8["Width"] = item.Width;
					dataRow8["Visible"] = "True";
					dataRow8["VisIndex"] = item.AbsoluteIndex;
					dataRow8["Editable"] = false;
					dataRow8["NeverEditable"] = true;
					dataRow8["IsReadOnly"] = item.OptionsColumn.ReadOnly;
					dataRow8["DataType"] = Function.ToString(item.ColumnType);
					DataRow dataRow9 = dataRow8;
					string value3;
					if (item.Fixed == FixedStyle.Left)
					{
						value3 = "L";
					}
					else
					{
						value3 = ((item.Fixed == FixedStyle.Right) ? "R" : "N");
					}
					dataRow9["Fixed"] = value3;
					dataRow8["AllowChSet"] = 1;
					dataRow8["VisSetting"] = true;
					dataRow8["IsSystem"] = "Y";
					dataRow8["LngCode"] = "-";
					dataRow8["GroupIndex"] = -1;
					dataTable.Rows.Add(dataRow8);
				}
			}
			dataTable.AcceptChanges();
			frm_FormSetting2.DtSource = dataTable;
			frm_FormSetting2.FormSettingId = form.FormId;
			frm_FormSetting2.ShowInTaskbar = true;
			frm_FormSetting2.ShowDialog(this);
			ReloadFormSetting(DtFormSetting = await _httpClient.FormSettingClient.GetFormSetting(FormId, FormId, GlobalData.UserCodeLogin, GlobalData.CURRENTLANGUAGE));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void LoadGeneral(string type)
	{
		try
		{
			int index = ((!type.Equals("P")) ? 1 : 0);
			nbi_All.Appearance.FontStyleDelta = FontStyle.Underline;
			nbg_ApprovalDocs.ItemLinks[index].PerformClick();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadItemNavBarGroup(NavBarGroup navBarGroup, DataTable dt)
	{
		try
		{
			navBarGroup.ItemLinks.Clear();
			NavBarItem item = nav_Menu.Items.Add(isSeparator: true);
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				NavBarItem navBarItem = new NavBarItem(Function.ToString(dt.Rows[i]["DocType"]));
				navBarItem.Appearance.Font = _defaultFont;
				navBarItem.Appearance.Options.UseFont = true;
				navBarItem.AppearanceHotTracked.Font = _defaultFont;
				navBarItem.AppearanceHotTracked.Options.UseFont = true;
				navBarItem.AppearancePressed.Font = _defaultFont;
				navBarItem.AppearancePressed.Options.UseFont = true;
				navBarItem.AppearanceHotTracked.FontStyleDelta = FontStyle.Underline;
				navBarGroup.ItemLinks.Add(navBarItem);
				if (i >= 0 && i < dt.Rows.Count - 1)
				{
					navBarGroup.ItemLinks.Add(item);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public void SettingMenu(DataTable dt)
	{
		try
		{
			if (!dt.IsNotNull())
			{
				ResetMenu();
				return;
			}
			List<DataRow> list = (from row in dt.AsEnumerable()
				where Function.ToString(row["MnuType"]).Equals("D")
				select row).ToList();
			List<DataRow> list2 = (from row in dt.AsEnumerable()
				where Function.ToString(row["MnuType"]).Equals("P")
				select row).ToList();
			if (list.Count > 0)
			{
				LoadItemNavBarGroup(nbg_Document, list.CopyToDataTable());
			}
			if (list2.Count > 0)
			{
				LoadItemNavBarGroup(nbg_Process, list2.CopyToDataTable());
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task SaveTemplateApproval(List<DataRow> lstItems, ApprovalType type)
	{
		try
		{
			if (lstItems.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.NoDocumentSelected, "");
				return;
			}
			WinCommons._OpenProcessing();
			EnableDisableControls(isEnable: false);
			foreach (DataRow lstItem in lstItems)
			{
				DataTable dataTable = new DataTable();
				dataTable.Columns.Add(new DataColumn("WddCode", typeof(int)));
				dataTable.Columns.Add(new DataColumn("Password", typeof(string)));
				dataTable.Columns.Add(new DataColumn("status", typeof(string)));
				dataTable.Columns.Add(new DataColumn("Remarks", typeof(string)));
				DataRow dataRow = dataTable.NewRow();
				dataRow["WddCode"] = lstItem["WddCode"];
				dataRow["Password"] = GlobalData.OldPassword;
				dataRow["status"] = ((type == ApprovalType.Approval) ? "Y" : "N");
				dataRow["Remarks"] = txt_Remarks.Text.Trim();
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
				string text = await _httpClient.WorkingTable.SetStatus(dataTable);
				if (!string.IsNullOrEmpty(text))
				{
					WinCommons.Error(Enums.MessageCode.None, text);
					EnableDisableControls(isEnable: true);
					return;
				}
				WinCommons.Success(Enums.MessageCode.Success, "");
			}
			LoadGeneral(CurrentRequest);
			EnableDisableControls(isEnable: true);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		EnableDisableControls(isEnable: true);
		WinCommons._CloseProcessing();
	}

	private async Task CreateDocuments(List<DataRow> lstItems)
	{
		_ = 2;
		try
		{
			WinCommons._OpenProcessing();
			int num = 0;
			foreach (DataRow lstItem in lstItems)
			{
				int wddCode = Function.ParseInt(lstItem["WddCode"]);
				int draftEntry = Function.ParseInt(lstItem["DocNo"]);
				int objType = Function.ParseInt(lstItem["ObjType"]);
				bool flag = _httpClient.WorkingProcessClient.CheckDocumentCompleteApproval(wddCode);
				if (num == draftEntry)
				{
					continue;
				}
				if (!flag)
				{
					WinCommons.Error(Enums.MessageCode.CannotCreateDocumentThatHasNotApprovedWithDraftKey, "", Function.ToString(draftEntry));
					continue;
				}
				EnableDisableControls(isEnable: false);
				HttpResult httpResult;
				if (new ApzObjectTypes[2]
				{
					ApzObjectTypes.oIncomingPayment,
					ApzObjectTypes.oVendorPayments
				}.Contains((ApzObjectTypes)objType))
				{
					using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "IncomingPayment");
					httpResult = await helper.PostAsync("SaveDraftToDocument", draftEntry);
				}
				else if (new int[2] { 1250000001, 67 }.Contains(objType))
				{
					using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "InventoryTranfer");
					httpResult = await helper.PostAsync("SaveDraftToDocument", draftEntry);
				}
				else
				{
					using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "WorkingProcess");
					httpResult = await helper.PostAsync("SaveDraftToDocument", new DocumentsImpl
					{
						draftKey = draftEntry
					});
				}
				if (httpResult.MessageCode != Enums.MessageCode.Success)
				{
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message + " (" + draftEntry + " - " + objType + ")");
					EnableDisableControls(isEnable: true);
					return;
				}
				WinCommons.Success(Enums.MessageCode.None, Apzon.Commons.Properties.Resources.Msg_Success + " Add draft to document " + httpResult.Content?.ToString() + "-" + draftEntry + " - " + objType);
				num = draftEntry;
			}
			LoadGeneral(CurrentRequest);
			EnableDisableControls(isEnable: true);
			uc_Templates.ClearItemsSelected();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		EnableDisableControls(isEnable: true);
		WinCommons._CloseProcessing();
	}

	private async Task LoadSetting(int userId)
	{
		try
		{
			DataSet setting = _httpClient.WorkingProcessClient.GetSetting(userId);
			if (setting.Tables.Count > 0)
			{
				SettingMenu(setting.Tables[0]);
			}
			DataTable dt = await _httpClient.FormSettingClient.GetFormSetting(FormId, FormId, GlobalData.UserCodeLogin, GlobalData.CURRENTLANGUAGE);
			uc_Templates.SettingTileViewItem(dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
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
			return EntitiesHelper<AttachmentFile_Impl>.ToEntities(dataTable);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		return new List<AttachmentFile_Impl>();
	}

	public void VisibleControls(string requestType)
	{
		if (requestType.Equals("M"))
		{
			btn_Save.Visible = true;
			btn_Approval.Visible = false;
			btn_Reject.Visible = false;
		}
		else
		{
			btn_Save.Visible = false;
			btn_Approval.Visible = true;
			btn_Reject.Visible = true;
		}
	}

	public void EnableDisableControls(bool isEnable)
	{
		btn_Save.Enabled = isEnable;
		btn_Approval.Enabled = isEnable;
		btn_Reject.Enabled = isEnable;
		pnl_Function.Refresh();
	}

	private void SetDefaultFormatMenu(string type)
	{
		try
		{
			if (type.Equals("A") || type.Equals("G"))
			{
				foreach (NavBarItemLink itemLink in nbg_ApprovalDocs.ItemLinks)
				{
					itemLink.Item.Appearance.FontStyleDelta = FontStyle.Regular;
				}
			}
			if (!type.Equals("A") && !type.Equals("S"))
			{
				return;
			}
			foreach (NavBarItemLink itemLink2 in nbg_Status.ItemLinks)
			{
				itemLink2.Item.Appearance.FontStyleDelta = FontStyle.Regular;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void SetValueControls(DataTable dt)
	{
		try
		{
			string text = Function.ToString(uc_Templates.TileView.GetRowCellValue(uc_Templates.TileView.FocusedRowHandle, "IsDraft"));
			int num = -1;
			if (text.Equals("N"))
			{
				num = Function.ParseInt(Function.ToString(uc_Templates.TileView.GetRowCellValue(uc_Templates.TileView.FocusedRowHandle, "DocNo")));
			}
			bool flag = dt.IsNotNull();
			bool flag2 = uc_Templates.IsDocumentCreated();
			if (flag2)
			{
				lbl_DraftNo.Utext = (flag2 ? _docNoText : _draftNoText);
				lbl_DraftType.Utext = (flag2 ? _docTypeText : _draftTypeText);
			}
			txt_UserRequest.EditValue = (flag ? dt.Rows[0]["UserName"] : string.Empty);
			dtp_CreateDate.EditValue = (flag ? Function.ToString(dt.Rows[0]["CreateDate"]) : DateTime.Now.ToString("dd.MM.yyyy"));
			ButtonEditEx buttonEditEx = btn_DocNo1;
			string editValue;
			if (flag)
			{
				editValue = (text.Equals("N") ? Function.ToString(num) : Function.ToString(dt.Rows[0]["DocNo"]));
			}
			else
			{
				editValue = string.Empty;
			}
			buttonEditEx.EditValue = editValue;
			txt_DocType.EditValue = (flag ? Function.ToString(dt.Rows[0]["DocType"]) : string.Empty);
			txt_Currency.EditValue = (flag ? Function.ToString(dt.Rows[0]["Currency"]) : string.Empty);
			txt_TemplateName.EditValue = (flag ? Function.ToString(dt.Rows[0]["WtmName"]) : string.Empty);
			txt_Price.Currency = (flag ? Function.ToString(dt.Rows[0]["Currency"]) : string.Empty);
			txt_Price.EditValue = (flag ? Function.ParseDecimal(dt.Rows[0]["Total"]) : 0m);
			grd_TemplateDetails.DataSource = dt;
			grd_TemplateDetails.RefreshDataSource();
			gv_TemplatesDetail.FocusedRowHandle = 0;
			if (DtFormSetting.IsNotNull())
			{
				List<DataRow> list = (from r in DtFormSetting.Copy().AsEnumerable()
					where Function.ToString(r["ColumnId"]).StartsWith("gc_")
					select r).ToList();
				if (list.Count > 0)
				{
					ReloadGridDetailSetting(list);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadTemplateDetails(int wddCode)
	{
		try
		{
			DataTable[] array = await Task.WhenAll<DataTable>(Task.Run(() => _httpClient.WorkingProcessClient.GetTemplateDetails(wddCode)));
			if (array.Length != 0)
			{
				SetValueControls(array[0]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private string GetValueOfItemSelected(NavBarGroup group)
	{
		try
		{
			string result = string.Empty;
			foreach (NavBarItemLink itemLink in group.ItemLinks)
			{
				NavBarItem item = itemLink.Item;
				if (group == nbg_ApprovalDocs)
				{
					if (item.Name.Equals("nbi_ApprovalRequest") && item.Appearance.FontStyleDelta == FontStyle.Underline)
					{
						result = "P";
						break;
					}
					if (item.Name.Equals("nbi_MyRequest") && item.Appearance.FontStyleDelta == FontStyle.Underline)
					{
						result = "M";
						break;
					}
				}
				else if (group == nbg_Status && item.Appearance.FontStyleDelta == FontStyle.Underline)
				{
					switch (item.Name)
					{
					case "nbi_All":
						result = "A";
						break;
					case "nbi_Approved":
						result = "Y";
						break;
					case "nbi_Rejected":
						result = "N";
						break;
					case "nbi_Pending":
						result = "W";
						break;
					case "nbi_Created":
						result = "C";
						break;
					}
					break;
				}
			}
			return result;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return string.Empty;
		}
	}

	private void GetProcessDocumentSelected(NavBarGroup group)
	{
		try
		{
			if (group != nbg_Process && group != nbg_Document)
			{
				return;
			}
			foreach (NavBarItemLink itemLink in group.ItemLinks)
			{
				NavBarItem item = itemLink.Item;
				if (item.Appearance.FontStyleDelta == FontStyle.Underline)
				{
					DataRow dataRow = _dtListFilter.NewRow();
					dataRow["Name"] = item.Caption;
					dataRow["Type"] = ((group == nbg_Process) ? "P" : "D");
					_dtListFilter.Rows.Add(dataRow);
				}
			}
			_dtListFilter.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task FilterData()
	{
		_ = 1;
		try
		{
			DataSet dataSet = new DataSet();
			WinCommons._OpenProcessing();
			_dtFilterConds.Rows.Clear();
			_dtListFilter.Rows.Clear();
			DataRow dataRow = _dtFilterConds.NewRow();
			dataRow["UserId"] = GlobalData.UserID;
			dataRow["Status"] = "A";
			dataRow["RequestType"] = GetValueOfItemSelected(nbg_ApprovalDocs);
			_dtFilterConds.Rows.Add(dataRow);
			_dtFilterConds.AcceptChanges();
			GetProcessDocumentSelected(nbg_Process);
			GetProcessDocumentSelected(nbg_Document);
			uc_Templates._dtSource = (_dtSource = _httpClient.WorkingProcessClient.GetTemplateData(GlobalData.UserID, GetValueOfItemSelected(nbg_Status), CurrentRequest));
			dataSet.Tables.Add(_dtSource.Copy());
			dataSet.Tables.Add(_dtFilterConds.Copy());
			dataSet.Tables.Add(_dtListFilter.Copy());
			dataSet.AcceptChanges();
			DataTable dataTable = _httpClient.WorkingProcessClient.FilterMenu(new Tuple<DataSet>(dataSet));
			if (!_dtListFilter.IsNotNull())
			{
				SaveMenu(dataTable);
			}
			uc_Templates._dtSourceTmp = dataTable.Copy();
			uc_Templates.SetDataSource(dataTable);
			if (!uc_Templates.GridControl.DataSourceTable.IsNotNull())
			{
				await LoadTemplateDetails(-1);
			}
			else
			{
				uc_Templates.TileView.FocusedRowHandle = 0;
				await LoadTemplateDetails(Function.ParseInt(uc_Templates.TileView.GetRowCellValue(uc_Templates.TileView.FocusedRowHandle, "WddCode")));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	private void ResetMenu()
	{
		nbg_Process.ItemLinks.Clear();
		nbg_Document.ItemLinks.Clear();
	}

	private void SaveMenu(DataTable dtSource)
	{
		try
		{
			if (!dtSource.IsNotNull())
			{
				return;
			}
			DataView dataView = new DataView(dtSource);
			DataTable dataTable = dataView.ToTable(true, "WtmName");
			DataTable dataTable2 = dataView.ToTable(true, "DocType");
			DataTable tableStructure = WinFuntions.GetTableStructure("APZ_WPS1");
			foreach (DataRow row in dataTable.Rows)
			{
				DataRow dataRow2 = tableStructure.NewRow();
				dataRow2["UserId"] = GlobalData.UserID;
				dataRow2["DocType"] = Function.ToString(row["WtmName"]);
				dataRow2["MnuType"] = "P";
				dataRow2["Visible"] = "Y";
				tableStructure.Rows.Add(dataRow2);
			}
			foreach (DataRow row2 in dataTable2.Rows)
			{
				DataRow dataRow4 = tableStructure.NewRow();
				dataRow4["UserId"] = GlobalData.UserID;
				dataRow4["DocType"] = Function.ToString(row2["DocType"]);
				dataRow4["MnuType"] = "D";
				dataRow4["Visible"] = "Y";
				tableStructure.Rows.Add(dataRow4);
			}
			if (!string.IsNullOrEmpty(_httpClient.WorkingProcessClient.SaveMenu(new Tuple<int, DataTable>(GlobalData.UserID, tableStructure)).Message))
			{
				Logging.Write("ERROR", "Save menu failed");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void SetItemUnderline(NavBarLinkEventArgs e, bool isUnderline)
	{
		e.Link.Item.Appearance.FontStyleDelta = (isUnderline ? FontStyle.Underline : FontStyle.Regular);
	}

	private void ReloadFormSetting(DataTable dtSource)
	{
		try
		{
			if (!dtSource.IsNotNull())
			{
				return;
			}
			List<DataRow> list = (from r in dtSource.Copy().AsEnumerable()
				where Function.ToString(r["ColumnId"]).StartsWith("nbg_") || Function.ToString(r["ColumnId"]).StartsWith("nbi_")
				select r).ToList();
			List<DataRow> list2 = (from r in dtSource.Copy().AsEnumerable()
				where Function.ToString(r["ColumnId"]).StartsWith("tc_")
				select r).ToList();
			List<DataRow> list3 = (from r in dtSource.Copy().AsEnumerable()
				where Function.ToString(r["ColumnId"]).StartsWith("lbl_")
				select r).ToList();
			List<DataRow> list4 = (from r in dtSource.Copy().AsEnumerable()
				where Function.ToString(r["ColumnId"]).StartsWith("btn_")
				select r).ToList();
			List<DataRow> list5 = (from r in dtSource.Copy().AsEnumerable()
				where Function.ToString(r["ColumnId"]).StartsWith("grp_")
				select r).ToList();
			List<DataRow> list6 = (from r in dtSource.Copy().AsEnumerable()
				where Function.ToString(r["ColumnId"]).StartsWith("gc_")
				select r).ToList();
			if (list.Count > 0)
			{
				foreach (DataRow row in list.CopyToDataTable().Rows)
				{
					string text = Function.ToString(row["ColumnId"]);
					string caption = Function.ToString(row["ColumnName"]);
					switch (text)
					{
					case "nbg_ApprovalDocs":
						nbg_ApprovalDocs.Caption = caption;
						break;
					case "nbg_Status":
						nbg_Status.Caption = caption;
						break;
					case "nbg_Process":
						nbg_Process.Caption = caption;
						break;
					case "nbg_Document":
						nbg_Document.Caption = caption;
						break;
					case "nbi_ApprovalRequest":
						nbi_ApprovalRequest.Caption = caption;
						break;
					case "nbi_MyRequest":
						nbi_MyRequest.Caption = caption;
						break;
					case "nbi_All":
						nbi_All.Caption = caption;
						break;
					case "nbi_Approved":
						nbi_Approved.Caption = caption;
						break;
					case "nbi_Rejected":
						nbi_Rejected.Caption = caption;
						break;
					case "nbi_Pending":
						nbi_Pending.Caption = caption;
						break;
					case "nbi_Created":
						nbi_Created.Caption = caption;
						break;
					}
				}
			}
			if (list2.Count > 0)
			{
				uc_Templates.SettingTileViewItem(list2.CopyToDataTable());
			}
			if (list3.Count > 0)
			{
				DataTable dataTable = list3.CopyToDataTable();
				foreach (DataRow row2 in dataTable.Rows)
				{
					string text2 = Function.ToString(row2["ColumnId"]);
					string utext = Function.ToString(row2["ColumnName"]);
					switch (text2)
					{
					case "lbl_DocType":
						_draftTypeText = Function.ToString(dataTable.Rows[0]["ColumnName"]);
						_docTypeText = Function.ToString(dataTable.Rows[9]["ColumnName"]);
						lbl_DraftType.Utext = utext;
						break;
					case "lbl_DocNo":
						_draftNoText = Function.ToString(dataTable.Rows[1]["ColumnName"]);
						_docNoText = Function.ToString(dataTable.Rows[8]["ColumnName"]);
						lbl_DraftNo.Utext = utext;
						break;
					case "lbl_Remarks":
						lbl_Remarks.Utext = utext;
						break;
					case "lbl_Template":
						lbl_Template.Utext = utext;
						break;
					case "lbl_Total":
						lbl_Total.Utext = utext;
						break;
					case "lbl_Currency":
						lbl_Currency.Utext = utext;
						break;
					case "lbl_CreateDate":
						lbl_CreateDate.Utext = utext;
						break;
					case "lbl_UserRequest":
						lbl_UserRequest.Utext = utext;
						break;
					}
				}
			}
			if (list4.Count > 0)
			{
				foreach (DataRow row3 in list4.CopyToDataTable().Rows)
				{
					switch (Function.ToString(row3["ColumnId"]))
					{
					case "btn_Approval":
						btn_Approval.Text = Function.ToString(row3["ColumnName"]);
						break;
					case "btn_Reject":
						btn_Reject.Text = Function.ToString(row3["ColumnName"]);
						break;
					case "btn_Save":
						btn_Save.Text = Function.ToString(row3["ColumnName"]);
						break;
					}
				}
			}
			if (list5.Count > 0)
			{
				grp_ApprovalStages.Text = Function.ToString(list5.CopyToDataTable().Rows[0]["ColumnName"]);
			}
			if (list6.Count > 0)
			{
				ReloadGridDetailSetting(list6.ToList());
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ReloadGridDetailSetting(List<DataRow> lst)
	{
		try
		{
			string text = Function.ToString(lst[0]["ColumnName"]);
			string text2 = Function.ToString(lst[1]["ColumnName"]);
			string text3 = Function.ToString(lst[2]["ColumnName"]);
			string text4 = Function.ToString(lst[3]["ColumnName"]);
			foreach (DataRow row in grd_TemplateDetails.DataSourceTable.Rows)
			{
				switch (Function.ToString(row["StatusCode"]))
				{
				case "W":
					row["Status"] = text;
					gc_Pending.Caption = text;
					break;
				case "Y":
					row["Status"] = text2;
					gc_Approved.Caption = text2;
					break;
				case "N":
					row["Status"] = text3;
					gc_Rejected.Caption = text3;
					break;
				case "S":
					row["Status"] = text4;
					gc_NotStarted.Caption = text4;
					break;
				}
			}
			grd_TemplateDetails.DataSourceTable.AcceptChanges();
			grd_TemplateDetails.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void Btn_DocNum_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				WinCommons._OpenProcessing();
				string id = Function.ToString(btn_DocNo1.Text);
				int num = Function.ParseInt(uc_Templates.TileView.GetRowCellValue(uc_Templates.TileView.FocusedRowHandle, "ObjType"));
				object rowCellValue = uc_Templates.TileView.GetRowCellValue(uc_Templates.TileView.FocusedRowHandle, "IsDraft");
				ApzObjectTypes fromType = ApzObjectTypes.oDrafts;
				ApzObjectTypes toType = ApzObjectTypes.oDrafts;
				if (rowCellValue.Equals("N"))
				{
					fromType = (ApzObjectTypes)num;
					toType = (ApzObjectTypes)num;
				}
				this.DrillDown(fromType, toType, id);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	private async Task frm_WorkingProcess_Load(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			_defaultUser = GlobalData.UserID;
			_dtFilterConds.Columns.Add("UserId", typeof(int));
			_dtFilterConds.Columns.Add("Status", typeof(string));
			_dtFilterConds.Columns.Add("RequestType", typeof(string));
			_dtListFilter.Columns.Add("Name", typeof(string));
			_dtListFilter.Columns.Add("Type", typeof(string));
			DtFormSetting = await _httpClient.FormSettingClient.GetFormSetting(FormId, FormId, GlobalData.UserCodeLogin, GlobalData.CURRENTLANGUAGE);
			await REFRESH_DATA();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_ApprovalStages_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (string.Equals(e.Column.FieldName, gc_Status.FieldName))
			{
				switch (Function.ToString(gv_TemplatesDetail.GetRowCellValue(e.RowHandle, "StatusCode")))
				{
				case "Y":
					e.Appearance.ForeColor = Color.Green;
					break;
				case "N":
					e.Appearance.ForeColor = Color.Red;
					break;
				case "W":
					e.Appearance.ForeColor = Color.DarkBlue;
					break;
				case "S":
					e.Appearance.ForeColor = Color.Gray;
					break;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btn_Approval_Click(object sender, EventArgs e)
	{
		try
		{
			if (XtraMessageBoxEx.Show((uc_Templates.ItemsSelected.Count == 1) ? Enums.MessageCode.DoYouWantToApprovalThisDocument : Enums.MessageCode.DoYouWantToApprovalTheSelectedDocuments, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				await SaveTemplateApproval(uc_Templates.ItemsSelected, ApprovalType.Approval);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btn_Rejected_Click(object sender, EventArgs e)
	{
		try
		{
			List<DataRow> itemsSelected = uc_Templates.ItemsSelected;
			if (XtraMessageBoxEx.Show((itemsSelected.Count == 1) ? Enums.MessageCode.DoYouWantToRejectThisDocument : Enums.MessageCode.DoYouWantToRejectTheSelectedDocuments, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				await SaveTemplateApproval(itemsSelected, ApprovalType.Reject);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void nav_Menu_CustomDrawGroupCaption(object sender, CustomDrawNavBarElementEventArgs e)
	{
		try
		{
			NavGroupInfoArgs navGroupInfoArgs = e.ObjectInfo as NavGroupInfoArgs;
			if (!(navGroupInfoArgs.Group.Name == nbg_ApprovalDocs.Name))
			{
				return;
			}
			e.Handled = true;
			Point location = new Point(navGroupInfoArgs.ButtonBounds.X + 8, navGroupInfoArgs.ButtonBounds.Y - 7);
			navGroupInfoArgs.ImageBounds = new Rectangle(location, new Size(16, 16));
			PropertyInfo property = nav_Menu.GetType().GetProperty("GroupPainter", BindingFlags.Instance | BindingFlags.NonPublic);
			if (property != null && property.GetValue(nav_Menu, null) is BaseNavGroupPainter baseNavGroupPainter)
			{
				baseNavGroupPainter.NavBar.OptionsNavPane.ShowExpandButton = false;
				MethodInfo method = baseNavGroupPainter.GetType().GetMethod("DrawGroupCaptionCore", BindingFlags.Instance | BindingFlags.NonPublic);
				if (method != null)
				{
					method.Invoke(baseNavGroupPainter, new object[2] { navGroupInfoArgs, e });
					e.Graphics.DrawImage(img_Lists.Images[4], new Point(navGroupInfoArgs.ButtonBounds.X, navGroupInfoArgs.ButtonBounds.Y));
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void nav_Menu_MouseClick(object sender, MouseEventArgs e)
	{
		try
		{
			if (e.Button != MouseButtons.Left || !(sender is NavBarControl navBarControl))
			{
				return;
			}
			NavBarHitInfo navBarHitInfo = navBarControl.CalcHitInfo(new Point(e.X, e.Y));
			if (navBarHitInfo.Group != null && navBarHitInfo.InGroupButton && navBarHitInfo.Group.Expanded && Function.ToString(navBarHitInfo.Group.Name).Equals("nbg_ApprovalDocs"))
			{
				await uc_Templates.ShowWorkingProcessSetting("P");
				if (uc_Templates.IsSettingChanged)
				{
					SettingMenu(uc_Templates.dtMenu);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void Nav_Menu_LinkClicked(object sender, NavBarLinkEventArgs e)
	{
		try
		{
			if (e.Link.Group == nbg_ApprovalDocs)
			{
				SetDefaultFormatMenu("G");
				SetDefaultFormatMenu("S");
				SetItemUnderline(e, isUnderline: true);
				CurrentRequest = (e.Link.Item.Name.Equals("nbi_ApprovalRequest") ? "P" : "M");
				if (CurrentRequest == "P")
				{
					nbi_Pending.Appearance.FontStyleDelta = FontStyle.Underline;
				}
				else
				{
					nbi_Approved.Appearance.FontStyleDelta = FontStyle.Underline;
				}
				uc_Templates.CurrentRequest = CurrentRequest;
			}
			else if (e.Link.Group == nbg_Status)
			{
				SetDefaultFormatMenu("S");
				SetItemUnderline(e, isUnderline: true);
			}
			else
			{
				bool isUnderline = e.Link.Item.Appearance.FontStyleDelta == FontStyle.Regular;
				SetItemUnderline(e, isUnderline);
			}
			FilterData();
			if (e.Link.Group == nbg_ApprovalDocs || e.Link.Group == nbg_Status)
			{
				if (uc_Templates.GridControl.DataSourceTable.IsNotNull())
				{
					uc_Templates.TileView.FocusedRowHandle = 0;
					LoadSetting(_defaultUser);
				}
				else
				{
					ResetMenu();
				}
			}
			VisibleControls(Function.ToString(_dtFilterConds.Rows[0]["RequestType"]));
			uc_Templates.ClearItemsSelected();
			if (uc_Templates.GridControl.DataSourceTable.IsNotNull())
			{
				string text = Function.ToString(uc_Templates.TileView.GetRowCellValue(uc_Templates.TileView.FocusedRowHandle, "Status"));
				string text2 = Function.ToString(uc_Templates.TileView.GetRowCellValue(uc_Templates.TileView.FocusedRowHandle, "IsDraft"));
				if (CurrentRequest.Equals("M") && text2.Equals("N"))
				{
					EnableDisableControls(isEnable: false);
				}
				else if (text.Equals("N"))
				{
					EnableDisableControls(isEnable: false);
				}
			}
			else
			{
				EnableDisableControls(isEnable: false);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btn_Save_Click(object sender, EventArgs e)
	{
		try
		{
			if (uc_Templates.GridControl.DataSourceTable.IsNotNull())
			{
				List<DataRow> itemsSelected = uc_Templates.ItemsSelected;
				if (itemsSelected.Count == 0)
				{
					WinCommons.Error(Enums.MessageCode.NoDocumentSelected, "");
				}
				else if (XtraMessageBoxEx.Show((itemsSelected.Count == 1 || itemsSelected.Count == 0) ? Enums.MessageCode.ConfirmCreateDocument : Enums.MessageCode.ConfirmCreateMultipleDocument, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					await CreateDocuments(itemsSelected);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void TileView_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
	{
		try
		{
			int focusedRowHandle = e.FocusedRowHandle;
			if (focusedRowHandle >= 0)
			{
				await LoadTemplateDetails(Function.ParseInt(uc_Templates.TileView.GetRowCellValue(focusedRowHandle, "WddCode")));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void RequestTypeSearch_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			string type = ((uc_Templates.RequestTypeControl.ItemIndex == 0) ? "P" : "M");
			SetDefaultFormatMenu("A");
			LoadGeneral(type);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void TileView_ItemClick(object sender, TileViewItemClickEventArgs e)
	{
		_ = 1;
		try
		{
			int rowHandle = uc_Templates.TileView.FocusedRowHandle;
			int wddCode = Function.ParseInt(uc_Templates.TileView.GetRowCellValue(rowHandle, "WddCode"));
			Function.ParseInt(uc_Templates.TileView.GetRowCellValue(rowHandle, "WtmCode"));
			string status = Function.ToString(uc_Templates.TileView.GetRowCellValue(rowHandle, "Status"));
			bool userLoginAppr = await _httpClient.WorkingProcessClient.IsUserApproved(wddCode, GlobalData.UserID);
			string text = Function.ToString(uc_Templates.TileView.GetRowCellValue(rowHandle, "IsDraft"));
			string text2 = Function.ToString(uc_Templates.TileView.GetRowCellValue(rowHandle, "ItemCheck"));
			int num = Function.ParseInt(uc_Templates.TileView.GetRowCellValue(rowHandle, "OwnerID"));
			int num2 = Function.ParseInt(uc_Templates.TileView.GetRowCellValue(rowHandle, "UserId"));
			bool isEnable = uc_Templates.ItemsSelected.Count > 0;
			if (text.Equals("N"))
			{
				e.Item.Enabled = false;
				uc_Templates.SetItemCheckValue(rowHandle, "False");
				uc_Templates.AllowCheckItem = false;
			}
			else if (num == GlobalData.UserID || num2 == GlobalData.UserID || uc_Templates.CurrentRequest.Equals("M"))
			{
				if (status.Equals("Y"))
				{
					VisibleControls("M");
					EnableDisableControls(isEnable);
				}
				else if (await _httpClient.WorkingProcessClient.AllowLastGroupUserCreatedDoc(wddCode, num))
				{
					VisibleControls("M");
					EnableDisableControls(isEnable);
				}
				else
				{
					VisibleControls(CurrentRequest);
					EnableDisableControls(isEnable && !userLoginAppr);
				}
			}
			else if (num2 != GlobalData.UserID)
			{
				VisibleControls(CurrentRequest);
				if (text2.Equals("False"))
				{
					EnableDisableControls(isEnable);
				}
				else if (status.Equals("Y") | userLoginAppr)
				{
					EnableDisableControls(isEnable: false);
				}
				else
				{
					EnableDisableControls(isEnable);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_TemplatesDetail_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
	{
		try
		{
			if (string.Equals(e.Column.FieldName, gc_Status.FieldName))
			{
				RepositoryItemTextEdit repositoryItemTextEdit = new RepositoryItemTextEdit();
				switch (Function.ToString(gv_TemplatesDetail.GetRowCellValue(e.RowHandle, "StatusCode")))
				{
				case "Y":
					repositoryItemTextEdit.Appearance.ForeColor = Color.Green;
					repositoryItemTextEdit.ContextImage = img_Lists.Images[0];
					break;
				case "N":
					repositoryItemTextEdit.Appearance.ForeColor = Color.Red;
					repositoryItemTextEdit.ContextImage = img_Lists.Images[1];
					break;
				case "W":
					repositoryItemTextEdit.Appearance.ForeColor = Color.DarkBlue;
					repositoryItemTextEdit.ContextImage = img_Lists.Images[2];
					break;
				case "S":
					repositoryItemTextEdit.Appearance.ForeColor = Color.Gray;
					repositoryItemTextEdit.ContextImage = img_Lists.Images[2];
					break;
				}
				grd_TemplateDetails.RepositoryItems.Add(repositoryItemTextEdit);
				e.RepositoryItem = repositoryItemTextEdit;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Administrator.ApprovalProcess.frm_WorkingProcess));
		this.repositoryItemCalcEditEx2 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.uc_Templates = new Apzon.Forms.UserControls.TileViewPagingEx();
		this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
		this.splitContainerControl2 = new DevExpress.XtraEditors.SplitContainerControl();
		this.btn_DocNo1 = new Apzon.Controls.Editor.ButtonEditEx();
		this.txt_Remarks = new Apzon.Controls.Editor.MemoExEditEx();
		this.txt_TemplateName = new Apzon.Controls.Editor.TextEditEx();
		this.txt_DocType = new Apzon.Controls.Editor.TextEditEx();
		this.txt_Currency = new Apzon.Controls.Editor.TextEditEx();
		this.txt_UserRequest = new Apzon.Controls.Editor.TextEditEx();
		this.txt_Price = new Apzon.Controls.Editor.TextEditExPrice.TextEditExPrice();
		this.dtp_CreateDate = new Apzon.Controls.Editor.DateEditEx();
		this.lbl_DraftType = new Apzon.Controls.Controls.LabelLine();
		this.lbl_DraftNo = new Apzon.Controls.Controls.LabelLine();
		this.lbl_Remarks = new Apzon.Controls.Controls.LabelLine();
		this.lbl_Template = new Apzon.Controls.Controls.LabelLine();
		this.lbl_Total = new Apzon.Controls.Controls.LabelLine();
		this.lbl_Currency = new Apzon.Controls.Controls.LabelLine();
		this.lbl_CreateDate = new Apzon.Controls.Controls.LabelLine();
		this.lbl_UserRequest = new Apzon.Controls.Controls.LabelLine();
		this.lbl_DocNo = new Apzon.Controls.Controls.LabelLine();
		this.lbl_DocType = new Apzon.Controls.Controls.LabelLine();
		this.grp_ApprovalStages = new DevExpress.XtraEditors.GroupControl();
		this.grd_TemplateDetails = new Apzon.Controls.Grid.GridControlEx();
		this.gv_TemplatesDetail = new Apzon.Controls.Grid.GridViewEx();
		this.gc_WddCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_WtmCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_WstCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_WstName = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_AuthorizerID = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_AuthorizerName = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Status = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_StatusApproved = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.gc_DateUpdate = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_TimeUpdate = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Remarks = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_SortId = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Pending = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Approved = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Rejected = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_NotStarted = new Apzon.Controls.Grid.GridColumnEx();
		this.pnl_Function = new DevExpress.XtraEditors.PanelControl();
		this.btn_Reject = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Approval = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.txt_Total = new Apzon.Controls.Editor.TextEditExPrice.TextEditExPrice();
		this.panelControl3 = new DevExpress.XtraEditors.PanelControl();
		this.nav_Menu = new DevExpress.XtraNavBar.NavBarControl();
		this.nbg_ApprovalDocs = new DevExpress.XtraNavBar.NavBarGroup();
		this.nbi_ApprovalRequest = new DevExpress.XtraNavBar.NavBarItem();
		this.nbi_MyRequest = new DevExpress.XtraNavBar.NavBarItem();
		this.nbg_Status = new DevExpress.XtraNavBar.NavBarGroup();
		this.nbi_All = new DevExpress.XtraNavBar.NavBarItem();
		this.nbi_Approved = new DevExpress.XtraNavBar.NavBarItem();
		this.nbi_Rejected = new DevExpress.XtraNavBar.NavBarItem();
		this.nbi_Pending = new DevExpress.XtraNavBar.NavBarItem();
		this.navBarItem1 = new DevExpress.XtraNavBar.NavBarItem();
		this.nbi_Created = new DevExpress.XtraNavBar.NavBarItem();
		this.nbg_Process = new DevExpress.XtraNavBar.NavBarGroup();
		this.nbg_Document = new DevExpress.XtraNavBar.NavBarGroup();
		this.img_Lists = new DevExpress.Utils.ImageCollection(this.components);
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.panelControl2).BeginInit();
		this.panelControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl2).BeginInit();
		this.splitContainerControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.btn_DocNo1.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Remarks.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_TemplateName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_DocType.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Currency.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_UserRequest.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Price.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dtp_CreateDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dtp_CreateDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grp_ApprovalStages).BeginInit();
		this.grp_ApprovalStages.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_TemplateDetails).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_TemplatesDetail).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_StatusApproved).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.pnl_Function).BeginInit();
		this.pnl_Function.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txt_Total.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl3).BeginInit();
		this.panelControl3.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.nav_Menu).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.img_Lists).BeginInit();
		base.SuspendLayout();
		this.repositoryItemCalcEditEx2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx2.IsSearchControl = false;
		this.repositoryItemCalcEditEx2.Name = "repositoryItemCalcEditEx2";
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Location = new System.Drawing.Point(247, 0);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Appearance.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.splitContainerControl1.Panel1.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.splitContainerControl1.Panel1.Appearance.Options.UseBackColor = true;
		this.splitContainerControl1.Panel1.Appearance.Options.UseFont = true;
		this.splitContainerControl1.Panel1.Controls.Add(this.uc_Templates);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.panelControl2);
		this.splitContainerControl1.Panel2.Controls.Add(this.pnl_Function);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(1185, 759);
		this.splitContainerControl1.SplitterPosition = 521;
		this.splitContainerControl1.TabIndex = 2;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.uc_Templates.Dock = System.Windows.Forms.DockStyle.Fill;
		this.uc_Templates.Location = new System.Drawing.Point(0, 0);
		this.uc_Templates.Name = "uc_Templates";
		this.uc_Templates.Size = new System.Drawing.Size(521, 759);
		this.uc_Templates.TabIndex = 7;
		this.panelControl2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.panelControl2.Controls.Add(this.splitContainerControl2);
		this.panelControl2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panelControl2.Location = new System.Drawing.Point(0, 35);
		this.panelControl2.Name = "panelControl2";
		this.panelControl2.Size = new System.Drawing.Size(654, 724);
		this.panelControl2.TabIndex = 1;
		this.splitContainerControl2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainerControl2.Horizontal = false;
		this.splitContainerControl2.Location = new System.Drawing.Point(0, 0);
		this.splitContainerControl2.Name = "splitContainerControl2";
		this.splitContainerControl2.Panel1.Controls.Add(this.btn_DocNo1);
		this.splitContainerControl2.Panel1.Controls.Add(this.txt_Remarks);
		this.splitContainerControl2.Panel1.Controls.Add(this.txt_TemplateName);
		this.splitContainerControl2.Panel1.Controls.Add(this.txt_DocType);
		this.splitContainerControl2.Panel1.Controls.Add(this.txt_Currency);
		this.splitContainerControl2.Panel1.Controls.Add(this.txt_UserRequest);
		this.splitContainerControl2.Panel1.Controls.Add(this.txt_Price);
		this.splitContainerControl2.Panel1.Controls.Add(this.dtp_CreateDate);
		this.splitContainerControl2.Panel1.Controls.Add(this.lbl_DraftType);
		this.splitContainerControl2.Panel1.Controls.Add(this.lbl_DraftNo);
		this.splitContainerControl2.Panel1.Controls.Add(this.lbl_Remarks);
		this.splitContainerControl2.Panel1.Controls.Add(this.lbl_Template);
		this.splitContainerControl2.Panel1.Controls.Add(this.lbl_Total);
		this.splitContainerControl2.Panel1.Controls.Add(this.lbl_Currency);
		this.splitContainerControl2.Panel1.Controls.Add(this.lbl_CreateDate);
		this.splitContainerControl2.Panel1.Controls.Add(this.lbl_UserRequest);
		this.splitContainerControl2.Panel1.Controls.Add(this.lbl_DocNo);
		this.splitContainerControl2.Panel1.Controls.Add(this.lbl_DocType);
		this.splitContainerControl2.Panel1.Text = "Panel1";
		this.splitContainerControl2.Panel2.Controls.Add(this.grp_ApprovalStages);
		this.splitContainerControl2.Panel2.Text = "Panel2";
		this.splitContainerControl2.Size = new System.Drawing.Size(654, 724);
		this.splitContainerControl2.SplitterPosition = 96;
		this.splitContainerControl2.TabIndex = 0;
		this.splitContainerControl2.Text = "splitContainerControl2";
		this.btn_DocNo1.EditControl = null;
		this.btn_DocNo1.FieldName = null;
		this.btn_DocNo1.IsSearchControl = false;
		this.btn_DocNo1.Location = new System.Drawing.Point(100, 27);
		this.btn_DocNo1.Name = "btn_DocNo1";
		this.btn_DocNo1.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btn_DocNo1.Properties.Appearance.Options.UseFont = true;
		this.btn_DocNo1.Properties.AppearanceFocused.BackColor = System.Drawing.Color.FromArgb(250, 249, 215);
		this.btn_DocNo1.Properties.AppearanceFocused.BackColor2 = System.Drawing.Color.FromArgb(250, 249, 215);
		this.btn_DocNo1.Properties.AppearanceFocused.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btn_DocNo1.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.btn_DocNo1.Properties.AppearanceFocused.Options.UseFont = true;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.btn_DocNo1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_DocNo1.Properties.ReadOnly = true;
		this.btn_DocNo1.Properties.UseReadOnlyAppearance = false;
		this.btn_DocNo1.SettingInVisible = false;
		this.btn_DocNo1.SettingReadonly = false;
		this.btn_DocNo1.Size = new System.Drawing.Size(157, 20);
		this.btn_DocNo1.TabIndex = 10;
		this.btn_DocNo1.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.Btn_DocNum_ButtonClick);
		this.txt_Remarks.EditControl = null;
		this.txt_Remarks.FieldName = null;
		this.txt_Remarks.IsSearchControl = false;
		this.txt_Remarks.Location = new System.Drawing.Point(372, 71);
		this.txt_Remarks.Name = "txt_Remarks";
		this.txt_Remarks.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_Remarks.Properties.ShowIcon = false;
		this.txt_Remarks.SettingInVisible = false;
		this.txt_Remarks.SettingReadonly = false;
		this.txt_Remarks.Size = new System.Drawing.Size(157, 20);
		this.txt_Remarks.TabIndex = 15;
		this.txt_TemplateName.EditControl = null;
		this.txt_TemplateName.FieldName = null;
		this.txt_TemplateName.IsSearchControl = false;
		this.txt_TemplateName.Location = new System.Drawing.Point(372, 49);
		this.txt_TemplateName.Name = "txt_TemplateName";
		this.txt_TemplateName.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txt_TemplateName.Properties.Appearance.Options.UseFont = true;
		this.txt_TemplateName.Properties.AppearanceFocused.BackColor = System.Drawing.Color.FromArgb(250, 249, 215);
		this.txt_TemplateName.Properties.AppearanceFocused.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txt_TemplateName.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.txt_TemplateName.Properties.AppearanceFocused.Options.UseFont = true;
		this.txt_TemplateName.Properties.ReadOnly = true;
		this.txt_TemplateName.Properties.UseReadOnlyAppearance = false;
		this.txt_TemplateName.SettingInVisible = false;
		this.txt_TemplateName.SettingReadonly = false;
		this.txt_TemplateName.Size = new System.Drawing.Size(157, 20);
		this.txt_TemplateName.TabIndex = 13;
		this.txt_DocType.EditControl = null;
		this.txt_DocType.FieldName = null;
		this.txt_DocType.IsSearchControl = false;
		this.txt_DocType.Location = new System.Drawing.Point(372, 27);
		this.txt_DocType.Name = "txt_DocType";
		this.txt_DocType.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txt_DocType.Properties.Appearance.Options.UseFont = true;
		this.txt_DocType.Properties.AppearanceFocused.BackColor = System.Drawing.Color.FromArgb(250, 249, 215);
		this.txt_DocType.Properties.AppearanceFocused.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txt_DocType.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.txt_DocType.Properties.AppearanceFocused.Options.UseFont = true;
		this.txt_DocType.Properties.ReadOnly = true;
		this.txt_DocType.Properties.UseReadOnlyAppearance = false;
		this.txt_DocType.SettingInVisible = false;
		this.txt_DocType.SettingReadonly = false;
		this.txt_DocType.Size = new System.Drawing.Size(157, 20);
		this.txt_DocType.TabIndex = 11;
		this.txt_Currency.EditControl = null;
		this.txt_Currency.FieldName = null;
		this.txt_Currency.IsSearchControl = false;
		this.txt_Currency.Location = new System.Drawing.Point(100, 49);
		this.txt_Currency.Name = "txt_Currency";
		this.txt_Currency.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txt_Currency.Properties.Appearance.Options.UseFont = true;
		this.txt_Currency.Properties.AppearanceFocused.BackColor = System.Drawing.Color.FromArgb(250, 249, 215);
		this.txt_Currency.Properties.AppearanceFocused.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txt_Currency.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.txt_Currency.Properties.AppearanceFocused.Options.UseFont = true;
		this.txt_Currency.Properties.ReadOnly = true;
		this.txt_Currency.Properties.UseReadOnlyAppearance = false;
		this.txt_Currency.SettingInVisible = false;
		this.txt_Currency.SettingReadonly = false;
		this.txt_Currency.Size = new System.Drawing.Size(157, 20);
		this.txt_Currency.TabIndex = 12;
		this.txt_UserRequest.EditControl = null;
		this.txt_UserRequest.FieldName = null;
		this.txt_UserRequest.IsSearchControl = false;
		this.txt_UserRequest.Location = new System.Drawing.Point(100, 5);
		this.txt_UserRequest.Name = "txt_UserRequest";
		this.txt_UserRequest.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txt_UserRequest.Properties.Appearance.Options.UseFont = true;
		this.txt_UserRequest.Properties.AppearanceFocused.BackColor = System.Drawing.Color.FromArgb(250, 249, 215);
		this.txt_UserRequest.Properties.AppearanceFocused.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txt_UserRequest.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.txt_UserRequest.Properties.AppearanceFocused.Options.UseFont = true;
		this.txt_UserRequest.Properties.ReadOnly = true;
		this.txt_UserRequest.Properties.UseReadOnlyAppearance = false;
		this.txt_UserRequest.SettingInVisible = false;
		this.txt_UserRequest.SettingReadonly = false;
		this.txt_UserRequest.Size = new System.Drawing.Size(157, 20);
		this.txt_UserRequest.TabIndex = 8;
		this.txt_Price.Currency = null;
		this.txt_Price.EditControl = null;
		this.txt_Price.FieldName = null;
		this.txt_Price.IsSearchControl = false;
		this.txt_Price.Location = new System.Drawing.Point(100, 71);
		this.txt_Price.Name = "txt_Price";
		this.txt_Price.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txt_Price.Properties.Appearance.Options.UseFont = true;
		this.txt_Price.Properties.Appearance.Options.UseTextOptions = true;
		this.txt_Price.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.txt_Price.Properties.AppearanceFocused.BackColor = System.Drawing.Color.FromArgb(250, 249, 215);
		this.txt_Price.Properties.AppearanceFocused.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txt_Price.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.txt_Price.Properties.AppearanceFocused.Options.UseFont = true;
		this.txt_Price.Properties.ReadOnly = true;
		this.txt_Price.Properties.UseReadOnlyAppearance = false;
		this.txt_Price.SettingInVisible = false;
		this.txt_Price.SettingReadonly = false;
		this.txt_Price.Size = new System.Drawing.Size(157, 20);
		this.txt_Price.TabIndex = 14;
		this.dtp_CreateDate.EditControl = null;
		this.dtp_CreateDate.EditValue = null;
		this.dtp_CreateDate.FieldName = "CreateDate";
		this.dtp_CreateDate.Location = new System.Drawing.Point(372, 5);
		this.dtp_CreateDate.Name = "dtp_CreateDate";
		this.dtp_CreateDate.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.dtp_CreateDate.Properties.Appearance.Options.UseFont = true;
		this.dtp_CreateDate.Properties.AppearanceDropDown.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.dtp_CreateDate.Properties.AppearanceDropDown.Options.UseFont = true;
		this.dtp_CreateDate.Properties.AppearanceFocused.BackColor = System.Drawing.Color.FromArgb(250, 249, 215);
		this.dtp_CreateDate.Properties.AppearanceFocused.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.dtp_CreateDate.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.dtp_CreateDate.Properties.AppearanceFocused.Options.UseFont = true;
		this.dtp_CreateDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dtp_CreateDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dtp_CreateDate.Properties.DisplayFormat.FormatString = "dd.MM.yyyy";
		this.dtp_CreateDate.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
		this.dtp_CreateDate.Properties.EditFormat.FormatString = "dd.MM.yyyy";
		this.dtp_CreateDate.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
		this.dtp_CreateDate.Properties.IsSearchControl = false;
		this.dtp_CreateDate.Properties.ReadOnly = true;
		this.dtp_CreateDate.Properties.UseReadOnlyAppearance = false;
		this.dtp_CreateDate.SettingInVisible = false;
		this.dtp_CreateDate.SettingReadonly = false;
		this.dtp_CreateDate.Size = new System.Drawing.Size(157, 20);
		this.dtp_CreateDate.TabIndex = 9;
		this.lbl_DraftType.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lbl_DraftType.Appearance.Options.UseFont = true;
		this.lbl_DraftType.EditControl = null;
		this.lbl_DraftType.EditValue = "Draft Type";
		this.lbl_DraftType.FieldName = null;
		this.lbl_DraftType.HideWhenEmpty = false;
		this.lbl_DraftType.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_DraftType.Location = new System.Drawing.Point(279, 30);
		this.lbl_DraftType.Name = "lbl_DraftType";
		this.lbl_DraftType.ReadOnly = false;
		this.lbl_DraftType.SettingInVisible = false;
		this.lbl_DraftType.SettingReadonly = false;
		this.lbl_DraftType.ShowUnderLine = true;
		this.lbl_DraftType.Size = new System.Drawing.Size(94, 20);
		this.lbl_DraftType.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_DraftType.TabIndex = 7;
		this.lbl_DraftType.Utext = "Draft Type";
		this.lbl_DraftNo.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lbl_DraftNo.Appearance.Options.UseFont = true;
		this.lbl_DraftNo.EditControl = null;
		this.lbl_DraftNo.EditValue = "Draft No";
		this.lbl_DraftNo.FieldName = null;
		this.lbl_DraftNo.HideWhenEmpty = false;
		this.lbl_DraftNo.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_DraftNo.Location = new System.Drawing.Point(6, 30);
		this.lbl_DraftNo.Name = "lbl_DraftNo";
		this.lbl_DraftNo.ReadOnly = false;
		this.lbl_DraftNo.SettingInVisible = false;
		this.lbl_DraftNo.SettingReadonly = false;
		this.lbl_DraftNo.ShowUnderLine = true;
		this.lbl_DraftNo.Size = new System.Drawing.Size(94, 20);
		this.lbl_DraftNo.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_DraftNo.TabIndex = 7;
		this.lbl_DraftNo.Utext = "Draft No";
		this.lbl_Remarks.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lbl_Remarks.Appearance.Options.UseFont = true;
		this.lbl_Remarks.EditControl = null;
		this.lbl_Remarks.EditValue = "Remarks";
		this.lbl_Remarks.FieldName = null;
		this.lbl_Remarks.HideWhenEmpty = false;
		this.lbl_Remarks.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_Remarks.Location = new System.Drawing.Point(279, 74);
		this.lbl_Remarks.Name = "lbl_Remarks";
		this.lbl_Remarks.ReadOnly = false;
		this.lbl_Remarks.SettingInVisible = false;
		this.lbl_Remarks.SettingReadonly = false;
		this.lbl_Remarks.ShowUnderLine = true;
		this.lbl_Remarks.Size = new System.Drawing.Size(94, 20);
		this.lbl_Remarks.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_Remarks.TabIndex = 7;
		this.lbl_Remarks.Utext = "Remarks";
		this.lbl_Template.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lbl_Template.Appearance.Options.UseFont = true;
		this.lbl_Template.EditControl = null;
		this.lbl_Template.EditValue = "Template";
		this.lbl_Template.FieldName = null;
		this.lbl_Template.HideWhenEmpty = false;
		this.lbl_Template.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_Template.Location = new System.Drawing.Point(279, 52);
		this.lbl_Template.Name = "lbl_Template";
		this.lbl_Template.ReadOnly = false;
		this.lbl_Template.SettingInVisible = false;
		this.lbl_Template.SettingReadonly = false;
		this.lbl_Template.ShowUnderLine = true;
		this.lbl_Template.Size = new System.Drawing.Size(94, 20);
		this.lbl_Template.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_Template.TabIndex = 7;
		this.lbl_Template.Utext = "Template";
		this.lbl_Total.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lbl_Total.Appearance.Options.UseFont = true;
		this.lbl_Total.EditControl = null;
		this.lbl_Total.EditValue = "Total";
		this.lbl_Total.FieldName = null;
		this.lbl_Total.HideWhenEmpty = false;
		this.lbl_Total.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_Total.Location = new System.Drawing.Point(6, 74);
		this.lbl_Total.Name = "lbl_Total";
		this.lbl_Total.ReadOnly = false;
		this.lbl_Total.SettingInVisible = false;
		this.lbl_Total.SettingReadonly = false;
		this.lbl_Total.ShowUnderLine = true;
		this.lbl_Total.Size = new System.Drawing.Size(94, 20);
		this.lbl_Total.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_Total.TabIndex = 7;
		this.lbl_Total.Utext = "Total";
		this.lbl_Currency.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lbl_Currency.Appearance.Options.UseFont = true;
		this.lbl_Currency.EditControl = null;
		this.lbl_Currency.EditValue = "Currency";
		this.lbl_Currency.FieldName = null;
		this.lbl_Currency.HideWhenEmpty = false;
		this.lbl_Currency.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_Currency.Location = new System.Drawing.Point(6, 52);
		this.lbl_Currency.Name = "lbl_Currency";
		this.lbl_Currency.ReadOnly = false;
		this.lbl_Currency.SettingInVisible = false;
		this.lbl_Currency.SettingReadonly = false;
		this.lbl_Currency.ShowUnderLine = true;
		this.lbl_Currency.Size = new System.Drawing.Size(94, 20);
		this.lbl_Currency.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_Currency.TabIndex = 7;
		this.lbl_Currency.Utext = "Currency";
		this.lbl_CreateDate.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lbl_CreateDate.Appearance.Options.UseFont = true;
		this.lbl_CreateDate.EditControl = null;
		this.lbl_CreateDate.EditValue = "Create Date";
		this.lbl_CreateDate.FieldName = null;
		this.lbl_CreateDate.HideWhenEmpty = false;
		this.lbl_CreateDate.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_CreateDate.Location = new System.Drawing.Point(279, 8);
		this.lbl_CreateDate.Name = "lbl_CreateDate";
		this.lbl_CreateDate.ReadOnly = false;
		this.lbl_CreateDate.SettingInVisible = false;
		this.lbl_CreateDate.SettingReadonly = false;
		this.lbl_CreateDate.ShowUnderLine = true;
		this.lbl_CreateDate.Size = new System.Drawing.Size(94, 20);
		this.lbl_CreateDate.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_CreateDate.TabIndex = 7;
		this.lbl_CreateDate.Utext = "Create Date";
		this.lbl_UserRequest.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lbl_UserRequest.Appearance.Options.UseFont = true;
		this.lbl_UserRequest.EditControl = null;
		this.lbl_UserRequest.EditValue = "User Request";
		this.lbl_UserRequest.FieldName = null;
		this.lbl_UserRequest.HideWhenEmpty = false;
		this.lbl_UserRequest.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_UserRequest.Location = new System.Drawing.Point(6, 8);
		this.lbl_UserRequest.Name = "lbl_UserRequest";
		this.lbl_UserRequest.ReadOnly = false;
		this.lbl_UserRequest.SettingInVisible = false;
		this.lbl_UserRequest.SettingReadonly = false;
		this.lbl_UserRequest.ShowUnderLine = true;
		this.lbl_UserRequest.Size = new System.Drawing.Size(94, 20);
		this.lbl_UserRequest.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_UserRequest.TabIndex = 7;
		this.lbl_UserRequest.Utext = "User Request";
		this.lbl_DocNo.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lbl_DocNo.Appearance.Options.UseFont = true;
		this.lbl_DocNo.EditControl = null;
		this.lbl_DocNo.EditValue = "Doc No";
		this.lbl_DocNo.FieldName = null;
		this.lbl_DocNo.HideWhenEmpty = false;
		this.lbl_DocNo.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_DocNo.Location = new System.Drawing.Point(6, 27);
		this.lbl_DocNo.Name = "lbl_DocNo";
		this.lbl_DocNo.ReadOnly = false;
		this.lbl_DocNo.SettingInVisible = false;
		this.lbl_DocNo.SettingReadonly = false;
		this.lbl_DocNo.ShowUnderLine = true;
		this.lbl_DocNo.Size = new System.Drawing.Size(94, 20);
		this.lbl_DocNo.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_DocNo.TabIndex = 17;
		this.lbl_DocNo.Utext = "Doc No";
		this.lbl_DocNo.Visible = false;
		this.lbl_DocType.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lbl_DocType.Appearance.Options.UseFont = true;
		this.lbl_DocType.EditControl = null;
		this.lbl_DocType.EditValue = "Doc Type";
		this.lbl_DocType.FieldName = null;
		this.lbl_DocType.HideWhenEmpty = false;
		this.lbl_DocType.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_DocType.Location = new System.Drawing.Point(279, 27);
		this.lbl_DocType.Name = "lbl_DocType";
		this.lbl_DocType.ReadOnly = false;
		this.lbl_DocType.SettingInVisible = false;
		this.lbl_DocType.SettingReadonly = false;
		this.lbl_DocType.ShowUnderLine = true;
		this.lbl_DocType.Size = new System.Drawing.Size(94, 20);
		this.lbl_DocType.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_DocType.TabIndex = 16;
		this.lbl_DocType.Utext = "Doc Type";
		this.lbl_DocType.Visible = false;
		this.grp_ApprovalStages.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grp_ApprovalStages.Appearance.Font = new System.Drawing.Font("Segoe UI", 18f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.grp_ApprovalStages.Appearance.Options.UseFont = true;
		this.grp_ApprovalStages.Controls.Add(this.grd_TemplateDetails);
		this.grp_ApprovalStages.Location = new System.Drawing.Point(0, 0);
		this.grp_ApprovalStages.Name = "grp_ApprovalStages";
		this.grp_ApprovalStages.Size = new System.Drawing.Size(646, 620);
		this.grp_ApprovalStages.TabIndex = 0;
		this.grp_ApprovalStages.Text = "Approval Stages";
		this.grd_TemplateDetails.AllowPasteExcel = true;
		this.grd_TemplateDetails.ClipboardData = null;
		this.grd_TemplateDetails.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_TemplateDetails.Location = new System.Drawing.Point(2, 23);
		this.grd_TemplateDetails.MainView = this.gv_TemplatesDetail;
		this.grd_TemplateDetails.Name = "grd_TemplateDetails";
		this.grd_TemplateDetails.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.rep_StatusApproved });
		this.grd_TemplateDetails.Size = new System.Drawing.Size(642, 595);
		this.grd_TemplateDetails.TabIndex = 16;
		this.grd_TemplateDetails.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_TemplatesDetail });
		this.gv_TemplatesDetail.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_TemplatesDetail.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_TemplatesDetail.Appearance.GroupRow.BackColor = System.Drawing.SystemColors.ControlLight;
		this.gv_TemplatesDetail.Appearance.GroupRow.Font = new System.Drawing.Font("Segoe UI Semibold", 9f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.gv_TemplatesDetail.Appearance.GroupRow.ForeColor = System.Drawing.Color.Gray;
		this.gv_TemplatesDetail.Appearance.GroupRow.Options.UseBackColor = true;
		this.gv_TemplatesDetail.Appearance.GroupRow.Options.UseFont = true;
		this.gv_TemplatesDetail.Appearance.GroupRow.Options.UseForeColor = true;
		this.gv_TemplatesDetail.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_TemplatesDetail.ColumnKey = null;
		this.gv_TemplatesDetail.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[15]
		{
			this.gc_WddCode, this.gc_WtmCode, this.gc_WstCode, this.gc_WstName, this.gc_AuthorizerID, this.gc_AuthorizerName, this.gc_Status, this.gc_DateUpdate, this.gc_TimeUpdate, this.gc_Remarks,
			this.gc_SortId, this.gc_Pending, this.gc_Approved, this.gc_Rejected, this.gc_NotStarted
		});
		this.gv_TemplatesDetail.DisableFormSetting = true;
		this.gv_TemplatesDetail.DisplayException = false;
		this.gv_TemplatesDetail.FieldCurrency = null;
		this.gv_TemplatesDetail.FieldRate = null;
		this.gv_TemplatesDetail.FormId = "oWorkingProcess";
		this.gv_TemplatesDetail.GridControl = this.grd_TemplateDetails;
		this.gv_TemplatesDetail.GroupCount = 1;
		this.gv_TemplatesDetail.GroupFormat = "[#image] {1}{2}";
		this.gv_TemplatesDetail.GroupRowHeight = 20;
		this.gv_TemplatesDetail.IndicatorWidth = 35;
		this.gv_TemplatesDetail.IsApzTable = false;
		this.gv_TemplatesDetail.IsInsertNewRow = false;
		this.gv_TemplatesDetail.IsLoadingData = false;
		this.gv_TemplatesDetail.ItemId = null;
		this.gv_TemplatesDetail.LstColumnAllowBatch = null;
		this.gv_TemplatesDetail.LstColumnNotAllowBatch = null;
		this.gv_TemplatesDetail.LstColumnNotEditable = null;
		this.gv_TemplatesDetail.LstColumNotSetting = null;
		this.gv_TemplatesDetail.Name = "gv_TemplatesDetail";
		this.gv_TemplatesDetail.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_TemplatesDetail.OptionsBehavior.AutoExpandAllGroups = true;
		this.gv_TemplatesDetail.OptionsBehavior.Editable = false;
		this.gv_TemplatesDetail.OptionsBehavior.ReadOnly = true;
		this.gv_TemplatesDetail.OptionsPrint.AutoWidth = false;
		this.gv_TemplatesDetail.OptionsPrint.UsePrintStyles = false;
		this.gv_TemplatesDetail.OptionsView.BestFitMaxRowCount = 100;
		this.gv_TemplatesDetail.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_TemplatesDetail.OptionsView.ShowColumnHeaders = false;
		this.gv_TemplatesDetail.OptionsView.ShowGroupPanel = false;
		this.gv_TemplatesDetail.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[2]
		{
			new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.gc_WstName, DevExpress.Data.ColumnSortOrder.Ascending),
			new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.gc_SortId, DevExpress.Data.ColumnSortOrder.Ascending)
		});
		this.gv_TemplatesDetail.UseBatchMod = false;
		this.gv_TemplatesDetail.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_ApprovalStages_RowCellStyle);
		this.gv_TemplatesDetail.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gv_TemplatesDetail_CustomRowCellEdit);
		this.gc_WddCode.Caption = "Wdd Code";
		this.gc_WddCode.CurrencyDateField = null;
		this.gc_WddCode.CurrencyField = null;
		this.gc_WddCode.CurrencyFixed = null;
		this.gc_WddCode.CurrencyFixedRate = 0.0;
		this.gc_WddCode.CurrencyRateField = null;
		this.gc_WddCode.FieldName = "WddCode";
		this.gc_WddCode.Name = "gc_WddCode";
		this.gc_WddCode.OptionsEditForm.Visible = DevExpress.Utils.DefaultBoolean.False;
		this.gc_WddCode.PrimaryKey = true;
		this.gc_WddCode.UnitField = null;
		this.gc_WtmCode.Caption = "Wtm Code";
		this.gc_WtmCode.CurrencyDateField = null;
		this.gc_WtmCode.CurrencyField = null;
		this.gc_WtmCode.CurrencyFixed = null;
		this.gc_WtmCode.CurrencyFixedRate = 0.0;
		this.gc_WtmCode.CurrencyRateField = null;
		this.gc_WtmCode.FieldName = "WtmCode";
		this.gc_WtmCode.Name = "gc_WtmCode";
		this.gc_WtmCode.OptionsEditForm.Visible = DevExpress.Utils.DefaultBoolean.False;
		this.gc_WtmCode.PrimaryKey = false;
		this.gc_WtmCode.UnitField = null;
		this.gc_WstCode.Caption = "Wst Code";
		this.gc_WstCode.CurrencyDateField = null;
		this.gc_WstCode.CurrencyField = null;
		this.gc_WstCode.CurrencyFixed = null;
		this.gc_WstCode.CurrencyFixedRate = 0.0;
		this.gc_WstCode.CurrencyRateField = null;
		this.gc_WstCode.FieldName = "WstCode";
		this.gc_WstCode.Name = "gc_WstCode";
		this.gc_WstCode.OptionsEditForm.Visible = DevExpress.Utils.DefaultBoolean.False;
		this.gc_WstCode.PrimaryKey = false;
		this.gc_WstCode.UnitField = null;
		this.gc_WstName.CurrencyDateField = null;
		this.gc_WstName.CurrencyField = null;
		this.gc_WstName.CurrencyFixed = null;
		this.gc_WstName.CurrencyFixedRate = 0.0;
		this.gc_WstName.CurrencyRateField = null;
		this.gc_WstName.FieldName = "WstName";
		this.gc_WstName.Name = "gc_WstName";
		this.gc_WstName.OptionsColumn.AllowGroup = DevExpress.Utils.DefaultBoolean.True;
		this.gc_WstName.OptionsColumn.ShowCaption = false;
		this.gc_WstName.OptionsEditForm.Visible = DevExpress.Utils.DefaultBoolean.False;
		this.gc_WstName.PrimaryKey = false;
		this.gc_WstName.UnitField = null;
		this.gc_AuthorizerID.Caption = "UserID";
		this.gc_AuthorizerID.CurrencyDateField = null;
		this.gc_AuthorizerID.CurrencyField = null;
		this.gc_AuthorizerID.CurrencyFixed = null;
		this.gc_AuthorizerID.CurrencyFixedRate = 0.0;
		this.gc_AuthorizerID.CurrencyRateField = null;
		this.gc_AuthorizerID.FieldName = "AuthID";
		this.gc_AuthorizerID.Name = "gc_AuthorizerID";
		this.gc_AuthorizerID.OptionsEditForm.Visible = DevExpress.Utils.DefaultBoolean.False;
		this.gc_AuthorizerID.PrimaryKey = false;
		this.gc_AuthorizerID.UnitField = null;
		this.gc_AuthorizerName.Caption = "Authorizer";
		this.gc_AuthorizerName.CurrencyDateField = null;
		this.gc_AuthorizerName.CurrencyField = null;
		this.gc_AuthorizerName.CurrencyFixed = null;
		this.gc_AuthorizerName.CurrencyFixedRate = 0.0;
		this.gc_AuthorizerName.CurrencyRateField = null;
		this.gc_AuthorizerName.FieldName = "AuthName";
		this.gc_AuthorizerName.Name = "gc_AuthorizerName";
		this.gc_AuthorizerName.OptionsColumn.FixedWidth = true;
		this.gc_AuthorizerName.PrimaryKey = false;
		this.gc_AuthorizerName.UnitField = null;
		this.gc_AuthorizerName.Visible = true;
		this.gc_AuthorizerName.VisibleIndex = 0;
		this.gc_AuthorizerName.Width = 166;
		this.gc_Status.AppearanceCell.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.gc_Status.AppearanceCell.Options.UseFont = true;
		this.gc_Status.Caption = "Status";
		this.gc_Status.ColumnEdit = this.rep_StatusApproved;
		this.gc_Status.CurrencyDateField = null;
		this.gc_Status.CurrencyField = null;
		this.gc_Status.CurrencyFixed = null;
		this.gc_Status.CurrencyFixedRate = 0.0;
		this.gc_Status.CurrencyRateField = null;
		this.gc_Status.FieldName = "Status";
		this.gc_Status.Name = "gc_Status";
		this.gc_Status.OptionsColumn.FixedWidth = true;
		this.gc_Status.PrimaryKey = false;
		this.gc_Status.UnitField = null;
		this.gc_Status.Visible = true;
		this.gc_Status.VisibleIndex = 1;
		this.gc_Status.Width = 115;
		this.rep_StatusApproved.AutoHeight = false;
		this.rep_StatusApproved.Name = "rep_StatusApproved";
		this.gc_DateUpdate.Caption = "Date Updated ";
		this.gc_DateUpdate.CurrencyDateField = null;
		this.gc_DateUpdate.CurrencyField = null;
		this.gc_DateUpdate.CurrencyFixed = null;
		this.gc_DateUpdate.CurrencyFixedRate = 0.0;
		this.gc_DateUpdate.CurrencyRateField = null;
		this.gc_DateUpdate.DisplayFormat.FormatString = "dd.MM.yyyy";
		this.gc_DateUpdate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
		this.gc_DateUpdate.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gc_DateUpdate.FieldName = "UpdateDate";
		this.gc_DateUpdate.Name = "gc_DateUpdate";
		this.gc_DateUpdate.OptionsColumn.FixedWidth = true;
		this.gc_DateUpdate.PrimaryKey = false;
		this.gc_DateUpdate.UnitField = null;
		this.gc_DateUpdate.Visible = true;
		this.gc_DateUpdate.VisibleIndex = 2;
		this.gc_DateUpdate.Width = 83;
		this.gc_TimeUpdate.Caption = "Time Updated";
		this.gc_TimeUpdate.CurrencyDateField = null;
		this.gc_TimeUpdate.CurrencyField = null;
		this.gc_TimeUpdate.CurrencyFixed = null;
		this.gc_TimeUpdate.CurrencyFixedRate = 0.0;
		this.gc_TimeUpdate.CurrencyRateField = null;
		this.gc_TimeUpdate.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Time;
		this.gc_TimeUpdate.FieldName = "UpdateTime";
		this.gc_TimeUpdate.Name = "gc_TimeUpdate";
		this.gc_TimeUpdate.OptionsColumn.FixedWidth = true;
		this.gc_TimeUpdate.PrimaryKey = false;
		this.gc_TimeUpdate.UnitField = null;
		this.gc_TimeUpdate.Visible = true;
		this.gc_TimeUpdate.VisibleIndex = 3;
		this.gc_TimeUpdate.Width = 67;
		this.gc_Remarks.Caption = "Remarks";
		this.gc_Remarks.CurrencyDateField = null;
		this.gc_Remarks.CurrencyField = null;
		this.gc_Remarks.CurrencyFixed = null;
		this.gc_Remarks.CurrencyFixedRate = 0.0;
		this.gc_Remarks.CurrencyRateField = null;
		this.gc_Remarks.FieldName = "AuthRemarks";
		this.gc_Remarks.Name = "gc_Remarks";
		this.gc_Remarks.PrimaryKey = false;
		this.gc_Remarks.UnitField = null;
		this.gc_Remarks.Visible = true;
		this.gc_Remarks.VisibleIndex = 4;
		this.gc_Remarks.Width = 165;
		this.gc_SortId.Caption = "SortId";
		this.gc_SortId.CurrencyDateField = null;
		this.gc_SortId.CurrencyField = null;
		this.gc_SortId.CurrencyFixed = null;
		this.gc_SortId.CurrencyFixedRate = 0.0;
		this.gc_SortId.CurrencyRateField = null;
		this.gc_SortId.FieldName = "SortId";
		this.gc_SortId.Name = "gc_SortId";
		this.gc_SortId.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
		this.gc_SortId.OptionsColumn.ReadOnly = true;
		this.gc_SortId.PrimaryKey = false;
		this.gc_SortId.UnitField = null;
		this.gc_Pending.Caption = "Pending";
		this.gc_Pending.CurrencyDateField = null;
		this.gc_Pending.CurrencyField = null;
		this.gc_Pending.CurrencyFixed = null;
		this.gc_Pending.CurrencyFixedRate = 0.0;
		this.gc_Pending.CurrencyRateField = null;
		this.gc_Pending.Name = "gc_Pending";
		this.gc_Pending.PrimaryKey = false;
		this.gc_Pending.UnitField = null;
		this.gc_Approved.Caption = "Approved";
		this.gc_Approved.CurrencyDateField = null;
		this.gc_Approved.CurrencyField = null;
		this.gc_Approved.CurrencyFixed = null;
		this.gc_Approved.CurrencyFixedRate = 0.0;
		this.gc_Approved.CurrencyRateField = null;
		this.gc_Approved.Name = "gc_Approved";
		this.gc_Approved.PrimaryKey = false;
		this.gc_Approved.UnitField = null;
		this.gc_Rejected.Caption = "Rejected";
		this.gc_Rejected.CurrencyDateField = null;
		this.gc_Rejected.CurrencyField = null;
		this.gc_Rejected.CurrencyFixed = null;
		this.gc_Rejected.CurrencyFixedRate = 0.0;
		this.gc_Rejected.CurrencyRateField = null;
		this.gc_Rejected.Name = "gc_Rejected";
		this.gc_Rejected.PrimaryKey = false;
		this.gc_Rejected.UnitField = null;
		this.gc_NotStarted.Caption = "Not started";
		this.gc_NotStarted.CurrencyDateField = null;
		this.gc_NotStarted.CurrencyField = null;
		this.gc_NotStarted.CurrencyFixed = null;
		this.gc_NotStarted.CurrencyFixedRate = 0.0;
		this.gc_NotStarted.CurrencyRateField = null;
		this.gc_NotStarted.Name = "gc_NotStarted";
		this.gc_NotStarted.PrimaryKey = false;
		this.gc_NotStarted.UnitField = null;
		this.gc_NotStarted.Width = 68;
		this.pnl_Function.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.pnl_Function.Controls.Add(this.btn_Reject);
		this.pnl_Function.Controls.Add(this.btn_Approval);
		this.pnl_Function.Controls.Add(this.btn_Save);
		this.pnl_Function.Dock = System.Windows.Forms.DockStyle.Top;
		this.pnl_Function.Location = new System.Drawing.Point(0, 0);
		this.pnl_Function.Name = "pnl_Function";
		this.pnl_Function.Size = new System.Drawing.Size(654, 35);
		this.pnl_Function.TabIndex = 0;
		this.btn_Reject.Appearance.BackColor = System.Drawing.Color.White;
		this.btn_Reject.Appearance.Font = new System.Drawing.Font("Segoe UI", 9f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.btn_Reject.Appearance.ForeColor = System.Drawing.Color.Red;
		this.btn_Reject.Appearance.Options.UseBackColor = true;
		this.btn_Reject.Appearance.Options.UseFont = true;
		this.btn_Reject.Appearance.Options.UseForeColor = true;
		this.btn_Reject.Appearance.Options.UseTextOptions = true;
		this.btn_Reject.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
		this.btn_Reject.Cursor = System.Windows.Forms.Cursors.Arrow;
		this.btn_Reject.EditControl = null;
		this.btn_Reject.EditValue = "Reject";
		this.btn_Reject.Enabled = false;
		this.btn_Reject.FieldName = null;
		this.btn_Reject.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Reject.ImageOptions.Image");
		this.btn_Reject.IsSearchControl = false;
		this.btn_Reject.Location = new System.Drawing.Point(89, 6);
		this.btn_Reject.Name = "btn_Reject";
		this.btn_Reject.ReadOnly = false;
		this.btn_Reject.SettingInVisible = false;
		this.btn_Reject.SettingReadonly = false;
		this.btn_Reject.Size = new System.Drawing.Size(83, 23);
		this.btn_Reject.TabIndex = 6;
		this.btn_Reject.Text = "Reject";
		this.btn_Reject.Click += new System.EventHandler(this.btn_Rejected_Click);
		this.btn_Approval.Appearance.BackColor = System.Drawing.Color.White;
		this.btn_Approval.Appearance.Font = new System.Drawing.Font("Segoe UI", 9f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.btn_Approval.Appearance.ForeColor = System.Drawing.Color.Green;
		this.btn_Approval.Appearance.Options.UseBackColor = true;
		this.btn_Approval.Appearance.Options.UseFont = true;
		this.btn_Approval.Appearance.Options.UseForeColor = true;
		this.btn_Approval.Appearance.Options.UseTextOptions = true;
		this.btn_Approval.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
		this.btn_Approval.Cursor = System.Windows.Forms.Cursors.Default;
		this.btn_Approval.EditControl = null;
		this.btn_Approval.EditValue = "Approval";
		this.btn_Approval.Enabled = false;
		this.btn_Approval.FieldName = null;
		this.btn_Approval.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Approval.ImageOptions.Image");
		this.btn_Approval.IsSearchControl = false;
		this.btn_Approval.Location = new System.Drawing.Point(3, 6);
		this.btn_Approval.Name = "btn_Approval";
		this.btn_Approval.ReadOnly = false;
		this.btn_Approval.SettingInVisible = false;
		this.btn_Approval.SettingReadonly = false;
		this.btn_Approval.Size = new System.Drawing.Size(83, 23);
		this.btn_Approval.TabIndex = 5;
		this.btn_Approval.Text = "Approval";
		this.btn_Approval.Click += new System.EventHandler(this.btn_Approval_Click);
		this.btn_Save.Appearance.Font = new System.Drawing.Font("Segoe UI", 9f, System.Drawing.FontStyle.Bold);
		this.btn_Save.Appearance.ForeColor = System.Drawing.Color.Black;
		this.btn_Save.Appearance.Options.UseFont = true;
		this.btn_Save.Appearance.Options.UseForeColor = true;
		this.btn_Save.ButtonKind = Apzon.Commons.Enums.Enums.SimpleButtonType.Save;
		this.btn_Save.EditControl = null;
		this.btn_Save.EditValue = "Save";
		this.btn_Save.Enabled = false;
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(3, 6);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.ReadOnly = false;
		this.btn_Save.SettingInVisible = false;
		this.btn_Save.SettingReadonly = false;
		this.btn_Save.Size = new System.Drawing.Size(83, 23);
		this.btn_Save.TabIndex = 7;
		this.btn_Save.Text = "Save";
		this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
		this.txt_Total.Currency = null;
		this.txt_Total.EditControl = null;
		this.txt_Total.FieldName = null;
		this.txt_Total.IsSearchControl = false;
		this.txt_Total.Location = new System.Drawing.Point(0, 0);
		this.txt_Total.Name = "txt_Total";
		this.txt_Total.Properties.Appearance.Options.UseTextOptions = true;
		this.txt_Total.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.txt_Total.SettingInVisible = false;
		this.txt_Total.SettingReadonly = false;
		this.txt_Total.Size = new System.Drawing.Size(100, 20);
		this.txt_Total.TabIndex = 0;
		this.panelControl3.Controls.Add(this.nav_Menu);
		this.panelControl3.Dock = System.Windows.Forms.DockStyle.Left;
		this.panelControl3.Location = new System.Drawing.Point(0, 0);
		this.panelControl3.Name = "panelControl3";
		this.panelControl3.Size = new System.Drawing.Size(241, 759);
		this.panelControl3.TabIndex = 0;
		this.nav_Menu.ActiveGroup = this.nbg_ApprovalDocs;
		this.nav_Menu.Dock = System.Windows.Forms.DockStyle.Fill;
		this.nav_Menu.DragDropFlags = DevExpress.XtraNavBar.NavBarDragDrop.None;
		this.nav_Menu.Font = new System.Drawing.Font("Segoe UI", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.nav_Menu.Groups.AddRange(new DevExpress.XtraNavBar.NavBarGroup[4] { this.nbg_ApprovalDocs, this.nbg_Status, this.nbg_Process, this.nbg_Document });
		this.nav_Menu.Items.AddRange(new DevExpress.XtraNavBar.NavBarItem[8] { this.nbi_All, this.nbi_ApprovalRequest, this.nbi_MyRequest, this.nbi_Approved, this.nbi_Rejected, this.nbi_Pending, this.nbi_Created, this.navBarItem1 });
		this.nav_Menu.Location = new System.Drawing.Point(2, 2);
		this.nav_Menu.Margin = new System.Windows.Forms.Padding(3, 3, 3, 100);
		this.nav_Menu.Name = "nav_Menu";
		this.nav_Menu.OptionsNavPane.ExpandedWidth = 237;
		this.nav_Menu.Padding = new System.Windows.Forms.Padding(0, 50, 50, 50);
		this.nav_Menu.PaintStyleKind = DevExpress.XtraNavBar.NavBarViewKind.ExplorerBar;
		this.nav_Menu.ShowGroupHint = false;
		this.nav_Menu.ShowIcons = DevExpress.Utils.DefaultBoolean.True;
		this.nav_Menu.ShowLinkHint = false;
		this.nav_Menu.Size = new System.Drawing.Size(237, 755);
		this.nav_Menu.SkinExplorerBarViewScrollStyle = DevExpress.XtraNavBar.SkinExplorerBarViewScrollStyle.ScrollBar;
		this.nav_Menu.StoreDefaultPaintStyleName = true;
		this.nav_Menu.TabIndex = 1;
		this.nav_Menu.Text = "navBarControl1";
		this.nav_Menu.CustomDrawGroupCaption += new DevExpress.XtraNavBar.ViewInfo.CustomDrawNavBarElementEventHandler(this.nav_Menu_CustomDrawGroupCaption);
		this.nav_Menu.LinkClicked += new DevExpress.XtraNavBar.NavBarLinkEventHandler(this.Nav_Menu_LinkClicked);
		this.nav_Menu.MouseClick += new System.Windows.Forms.MouseEventHandler(this.nav_Menu_MouseClick);
		this.nbg_ApprovalDocs.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 9f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.nbg_ApprovalDocs.Appearance.Options.UseFont = true;
		this.nbg_ApprovalDocs.Caption = "General";
		this.nbg_ApprovalDocs.Expanded = true;
		this.nbg_ApprovalDocs.ItemLinks.AddRange(new DevExpress.XtraNavBar.NavBarItemLink[2]
		{
			new DevExpress.XtraNavBar.NavBarItemLink(this.nbi_ApprovalRequest),
			new DevExpress.XtraNavBar.NavBarItemLink(this.nbi_MyRequest)
		});
		this.nbg_ApprovalDocs.Name = "nbg_ApprovalDocs";
		this.nbi_ApprovalRequest.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.nbi_ApprovalRequest.Appearance.Options.UseFont = true;
		this.nbi_ApprovalRequest.AppearanceHotTracked.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.nbi_ApprovalRequest.AppearanceHotTracked.FontStyleDelta = System.Drawing.FontStyle.Underline;
		this.nbi_ApprovalRequest.AppearanceHotTracked.Options.UseFont = true;
		this.nbi_ApprovalRequest.AppearancePressed.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.nbi_ApprovalRequest.AppearancePressed.Options.UseFont = true;
		this.nbi_ApprovalRequest.Caption = "Approval Request";
		this.nbi_ApprovalRequest.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("nbi_ApprovalRequest.ImageOptions.LargeImage");
		this.nbi_ApprovalRequest.ImageOptions.SmallImage = (System.Drawing.Image)resources.GetObject("nbi_ApprovalRequest.ImageOptions.SmallImage");
		this.nbi_ApprovalRequest.ImageOptions.SmallImageSize = new System.Drawing.Size(16, 16);
		this.nbi_ApprovalRequest.Name = "nbi_ApprovalRequest";
		this.nbi_MyRequest.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.nbi_MyRequest.Appearance.Options.UseFont = true;
		this.nbi_MyRequest.AppearanceHotTracked.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.nbi_MyRequest.AppearanceHotTracked.FontStyleDelta = System.Drawing.FontStyle.Underline;
		this.nbi_MyRequest.AppearanceHotTracked.Options.UseFont = true;
		this.nbi_MyRequest.AppearancePressed.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.nbi_MyRequest.AppearancePressed.Options.UseFont = true;
		this.nbi_MyRequest.Caption = "My Request";
		this.nbi_MyRequest.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("nbi_MyRequest.ImageOptions.LargeImage");
		this.nbi_MyRequest.ImageOptions.SmallImage = (System.Drawing.Image)resources.GetObject("nbi_MyRequest.ImageOptions.SmallImage");
		this.nbi_MyRequest.Name = "nbi_MyRequest";
		this.nbg_Status.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 9f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.nbg_Status.Appearance.Options.UseFont = true;
		this.nbg_Status.Caption = "Status";
		this.nbg_Status.Expanded = true;
		this.nbg_Status.ItemLinks.AddRange(new DevExpress.XtraNavBar.NavBarItemLink[6]
		{
			new DevExpress.XtraNavBar.NavBarItemLink(this.nbi_All),
			new DevExpress.XtraNavBar.NavBarItemLink(this.nbi_Approved),
			new DevExpress.XtraNavBar.NavBarItemLink(this.nbi_Rejected),
			new DevExpress.XtraNavBar.NavBarItemLink(this.nbi_Pending),
			new DevExpress.XtraNavBar.NavBarItemLink(this.navBarItem1),
			new DevExpress.XtraNavBar.NavBarItemLink(this.nbi_Created)
		});
		this.nbg_Status.Name = "nbg_Status";
		this.nbi_All.Appearance.Font = new System.Drawing.Font("Segoe UI", 9f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.nbi_All.Appearance.ForeColor = System.Drawing.Color.Brown;
		this.nbi_All.Appearance.Options.UseFont = true;
		this.nbi_All.Appearance.Options.UseForeColor = true;
		this.nbi_All.AppearanceDisabled.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.nbi_All.AppearanceDisabled.Options.UseFont = true;
		this.nbi_All.AppearanceHotTracked.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.nbi_All.AppearanceHotTracked.FontStyleDelta = System.Drawing.FontStyle.Underline;
		this.nbi_All.AppearanceHotTracked.ForeColor = System.Drawing.Color.Brown;
		this.nbi_All.AppearanceHotTracked.Options.UseFont = true;
		this.nbi_All.AppearanceHotTracked.Options.UseForeColor = true;
		this.nbi_All.AppearancePressed.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Bold);
		this.nbi_All.AppearancePressed.ForeColor = System.Drawing.Color.DimGray;
		this.nbi_All.AppearancePressed.Options.UseFont = true;
		this.nbi_All.AppearancePressed.Options.UseForeColor = true;
		this.nbi_All.Caption = "All";
		this.nbi_All.Name = "nbi_All";
		this.nbi_Approved.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.nbi_Approved.Appearance.ForeColor = System.Drawing.Color.Green;
		this.nbi_Approved.Appearance.Options.UseFont = true;
		this.nbi_Approved.Appearance.Options.UseForeColor = true;
		this.nbi_Approved.AppearanceHotTracked.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.nbi_Approved.AppearanceHotTracked.FontStyleDelta = System.Drawing.FontStyle.Underline;
		this.nbi_Approved.AppearanceHotTracked.ForeColor = System.Drawing.Color.Green;
		this.nbi_Approved.AppearanceHotTracked.Options.UseFont = true;
		this.nbi_Approved.AppearanceHotTracked.Options.UseForeColor = true;
		this.nbi_Approved.AppearancePressed.Font = new System.Drawing.Font("Segoe UI", 8.25f);
		this.nbi_Approved.AppearancePressed.ForeColor = System.Drawing.Color.Green;
		this.nbi_Approved.AppearancePressed.Options.UseFont = true;
		this.nbi_Approved.AppearancePressed.Options.UseForeColor = true;
		this.nbi_Approved.Caption = "Approved";
		this.nbi_Approved.ImageOptions.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
		this.nbi_Approved.ImageOptions.SmallImage = (System.Drawing.Image)resources.GetObject("nbi_Approved.ImageOptions.SmallImage");
		this.nbi_Approved.Name = "nbi_Approved";
		this.nbi_Rejected.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25f);
		this.nbi_Rejected.Appearance.ForeColor = System.Drawing.Color.Red;
		this.nbi_Rejected.Appearance.Options.UseFont = true;
		this.nbi_Rejected.Appearance.Options.UseForeColor = true;
		this.nbi_Rejected.AppearanceHotTracked.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.nbi_Rejected.AppearanceHotTracked.FontStyleDelta = System.Drawing.FontStyle.Underline;
		this.nbi_Rejected.AppearanceHotTracked.ForeColor = System.Drawing.Color.Red;
		this.nbi_Rejected.AppearanceHotTracked.Options.UseFont = true;
		this.nbi_Rejected.AppearanceHotTracked.Options.UseForeColor = true;
		this.nbi_Rejected.AppearancePressed.Font = new System.Drawing.Font("Segoe UI", 8.25f);
		this.nbi_Rejected.AppearancePressed.ForeColor = System.Drawing.Color.Red;
		this.nbi_Rejected.AppearancePressed.Options.UseFont = true;
		this.nbi_Rejected.AppearancePressed.Options.UseForeColor = true;
		this.nbi_Rejected.Caption = "Rejected";
		this.nbi_Rejected.ImageOptions.SmallImage = (System.Drawing.Image)resources.GetObject("nbi_Rejected.ImageOptions.SmallImage");
		this.nbi_Rejected.Name = "nbi_Rejected";
		this.nbi_Pending.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25f);
		this.nbi_Pending.Appearance.ForeColor = System.Drawing.Color.DarkBlue;
		this.nbi_Pending.Appearance.Options.UseFont = true;
		this.nbi_Pending.Appearance.Options.UseForeColor = true;
		this.nbi_Pending.AppearanceHotTracked.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.nbi_Pending.AppearanceHotTracked.FontStyleDelta = System.Drawing.FontStyle.Underline;
		this.nbi_Pending.AppearanceHotTracked.ForeColor = System.Drawing.Color.DarkBlue;
		this.nbi_Pending.AppearanceHotTracked.Options.UseFont = true;
		this.nbi_Pending.AppearanceHotTracked.Options.UseForeColor = true;
		this.nbi_Pending.AppearancePressed.Font = new System.Drawing.Font("Segoe UI", 8.25f);
		this.nbi_Pending.AppearancePressed.ForeColor = System.Drawing.Color.DarkBlue;
		this.nbi_Pending.AppearancePressed.Options.UseFont = true;
		this.nbi_Pending.AppearancePressed.Options.UseForeColor = true;
		this.nbi_Pending.Caption = "Pending";
		this.nbi_Pending.ImageOptions.SmallImage = (System.Drawing.Image)resources.GetObject("nbi_Pending.ImageOptions.SmallImage");
		this.nbi_Pending.Name = "nbi_Pending";
		this.navBarItem1.Caption = "";
		this.navBarItem1.Enabled = false;
		this.navBarItem1.Name = "navBarItem1";
		this.nbi_Created.Appearance.Font = new System.Drawing.Font("Segoe UI", 9f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.nbi_Created.Appearance.Options.UseFont = true;
		this.nbi_Created.Appearance.Options.UseForeColor = true;
		this.nbi_Created.Appearance.Options.UseTextOptions = true;
		this.nbi_Created.AppearanceHotTracked.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25f, System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline);
		this.nbi_Created.AppearanceHotTracked.FontStyleDelta = System.Drawing.FontStyle.Underline;
		this.nbi_Created.AppearanceHotTracked.Options.UseFont = true;
		this.nbi_Created.AppearancePressed.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25f, System.Drawing.FontStyle.Bold);
		this.nbi_Created.AppearancePressed.Options.UseFont = true;
		this.nbi_Created.Caption = "Created";
		this.nbi_Created.Name = "nbi_Created";
		this.nbg_Process.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 9f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.nbg_Process.Appearance.Options.UseFont = true;
		this.nbg_Process.Caption = "Process";
		this.nbg_Process.Expanded = true;
		this.nbg_Process.Name = "nbg_Process";
		this.nbg_Document.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 9f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.nbg_Document.Appearance.Options.UseFont = true;
		this.nbg_Document.AppearanceBackground.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.nbg_Document.AppearanceBackground.Options.UseFont = true;
		this.nbg_Document.Caption = "Document";
		this.nbg_Document.Expanded = true;
		this.nbg_Document.GroupStyle = DevExpress.XtraNavBar.NavBarGroupStyle.SmallIconsText;
		this.nbg_Document.Name = "nbg_Document";
		this.img_Lists.ImageStream = (DevExpress.Utils.ImageCollectionStreamer)resources.GetObject("img_Lists.ImageStream");
		this.img_Lists.Images.SetKeyName(0, "approved.png");
		this.img_Lists.Images.SetKeyName(1, "reject.png");
		this.img_Lists.Images.SetKeyName(2, "pending.png");
		this.img_Lists.Images.SetKeyName(3, "cancel.png");
		this.img_Lists.Images.SetKeyName(4, "setting.png");
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1432, 759);
		base.Controls.Add(this.panelControl3);
		base.Controls.Add(this.splitContainerControl1);
		base.DisableContextAction = true;
		base.DisableLoadScript = true;
		base.FormId = "oWorkingProcess";
		base.FormObjectType = Apzon.Commons.ApzObjectTypes.oWorkingProcess;
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_WorkingProcess.IconOptions.Icon");
		base.IconOptions.Image = (System.Drawing.Image)resources.GetObject("frm_WorkingProcess.IconOptions.Image");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_WorkingProcess";
		this.Text = "Working Process";
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.panelControl2).EndInit();
		this.panelControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl2).EndInit();
		this.splitContainerControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.btn_DocNo1.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Remarks.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_TemplateName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_DocType.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Currency.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_UserRequest.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Price.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dtp_CreateDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dtp_CreateDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grp_ApprovalStages).EndInit();
		this.grp_ApprovalStages.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_TemplateDetails).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_TemplatesDetail).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_StatusApproved).EndInit();
		((System.ComponentModel.ISupportInitialize)this.pnl_Function).EndInit();
		this.pnl_Function.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txt_Total.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl3).EndInit();
		this.panelControl3.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.nav_Menu).EndInit();
		((System.ComponentModel.ISupportInitialize)this.img_Lists).EndInit();
		base.ResumeLayout(false);
	}
}
