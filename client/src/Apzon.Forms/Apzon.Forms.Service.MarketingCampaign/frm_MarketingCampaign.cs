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
using Apzon.Controls.Editor.MyLookUpEdit;
using Apzon.Controls.Editor.TextEditExPrice;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Forms.Properties;
using Apzon.Forms.SalesOppotunity;
using Apzon.Forms.UserControls;
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
using DevExpress.XtraTab;

namespace Apzon.Forms.Service.MarketingCampaign;

public class frm_MarketingCampaign : XtraFormEx
{
	private const string OkString = "OK";

	private const string UpdateString = "Update";

	private const string AddString = "Add";

	private DataTable _dtDoc;

	private DataTable _dtLine;

	private DataTable _dtItem;

	private DataTable _dtPartner;

	private DataTable _dtmember;

	private DataTable _dtCost;

	private DataTable _dtAttactment;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private bool _isLoading;

	public int CampaignId;

	private Dictionary<int, string> dictionEmployee = new Dictionary<int, string>();

	private Dictionary<int, string> dictionUser = new Dictionary<int, string>();

	private IContainer components;

	private SplitContainerControl splitContainerControl1;

	private GridControlEx grd_Doc;

	private GridViewEx gv_Doc;

	private XtraTabControlEx xtraTabControlEx1;

	private XtraTabPageEx xtra_Bp;

	private GridControlEx grd_Bp;

	private GridViewEx gv_Bp;

	private XtraTabPageEx xtra_Item;

	private GridControlEx grd_Item;

	private GridViewEx gv_Item;

	private XtraTabPageEx xtra_Partner;

	private XtraTabPageEx xtra_Member;

	private GridControlEx grd_Member;

	private GridViewEx gv_Member;

	private XtraTabPageEx xtra_Atta;

	private ucAttachment ucAttachment1;

	private XtraTabPageEx xtra_Cost;

	private GridControlEx grd_Cost;

	private GridViewEx gv_Cost;

	private XtraTabPageEx xtra_Task;

	private LabelLine labelLine1;

	private LabelLine labelLine2;

	private LabelLine labelLine3;

	private LabelLine labelLine4;

	private TextEditEx txt_No;

	private LookUpEditSimple luk_StatusSearch;

	private DateEditEx date_FromDate;

	private DateEditEx date_ToDate;

	private SimpleButtonEx btn_Search;

	private SimpleButton btn_Ok;

	private SimpleButton btn_Cancel;

	private ContextMenuStrip ctm_Doc;

	private ToolStripMenuItem addNewToolStripMenuItem;

	private ToolStripMenuItem deleteToolStripMenuItem;

	private ContextMenuStrip ctm_Bp;

	private ToolStripMenuItem addRow_Bp;

	private ToolStripMenuItem deleteRow_Bp;

	private ContextMenuStrip ctm_Member;

	private ToolStripMenuItem addRow_member;

	private ToolStripMenuItem deleteRow_member;

	private ContextMenuStrip ctm_Cost;

	private ToolStripMenuItem addRow_Cost;

	private ToolStripMenuItem deleteRow_Cost;

	private GridColumnEx gridColumnEx1;

	private GridColumnEx gridColumnEx2;

	private GridColumnEx gridColumnEx3;

	private GridColumnEx gridColumnEx4;

	private GridColumnEx gridColumnEx5;

	private GridColumnEx gridColumnEx6;

	private GridColumnEx gridColumnEx7;

	private GridColumnEx gridColumnEx8;

	private GridColumnEx gridColumnEx9;

	private GridColumnEx gridColumnEx10;

	private GridColumnEx gridColumnEx11;

	private GridColumnEx gridColumnEx12;

	private GridColumnEx gridColumnEx13;

	private GridColumnEx gridColumnEx14;

	private GridColumnEx gridColumnEx15;

	private GridColumnEx gridColumnEx16;

	private GridColumnEx gridColumnEx17;

	private GridColumnEx gridColumnEx18;

	private GridColumnEx gridColumnEx19;

	private GridColumnEx gridColumnEx20;

	private GridColumnEx gridColumnEx21;

	private GridColumnEx gridColumnEx22;

	private GridColumnEx gridColumnEx23;

	private GridColumnEx gridColumnEx24;

	private GridColumnEx gridColumnEx25;

	private GridColumnEx gridColumnEx26;

	private GridColumnEx gridColumnEx27;

	private GridColumnEx gridColumnEx28;

	private GridColumnEx gridColumnEx29;

	private GridColumnEx gridColumnEx30;

	private GridColumnEx gridColumnEx31;

	private GridColumnEx gridColumnEx32;

	private GridColumnEx gridColumnEx33;

	private GridColumnEx gridColumnEx34;

	private GridColumnEx gridColumnEx35;

	private GridColumnEx gridColumnEx36;

	private GridColumnEx gridColumnEx37;

	private GridColumnEx gridColumnEx38;

	private GridColumnEx gridColumnEx39;

	private GridColumnEx gridColumnEx40;

	private GridColumnEx gridColumnEx41;

	private GridColumnEx gridColumnEx42;

	private GridColumnEx gridColumnEx43;

	private GridColumnEx gridColumnEx44;

	private GridColumnEx gridColumnEx45;

	private GridColumnEx gridColumnEx46;

	private RepositoryItemButtonEditCFL btn_ItemCost;

	private GridColumnEx gridColumnEx47;

	private GridColumnEx gridColumnEx48;

	private GridColumnEx gridColumnEx49;

	private GridColumnEx gridColumnEx50;

	private GridColumnEx gridColumnEx51;

	private GridColumnEx gridColumnEx52;

	private GridColumnEx gridColumnEx53;

	private GridColumnEx gridColumnEx54;

	private GridColumnEx gridColumnEx55;

	private GridControlEx grd_ListCompleted;

	private GridViewEx gv_ListCompleted;

	private GridColumnEx gridColumnEx56;

	private GridColumnEx gc_Closed_Completed;

	private RepositoryItemButtonEdit rep_bte_Closed;

	private GridColumnEx gridColumnEx57;

	private GridColumnEx gc_DueDate_Completed;

	private GridColumnEx gc_ProjectEntry_Completed;

	private GridColumnEx gc_ProjectWBS_Completed;

	private GridColumnEx gridColumnEx58;

	private GridColumnEx gridColumnEx59;

	private GridColumnEx gridColumnEx60;

	private GridColumnEx gc_AssignTo_Completed;

	private GridColumnEx gc_CloseDate_Completed;

	private GridColumnEx gc_Priority_Completed;

	private RepositoryItemButtonEdit rep_bte_Open;

	private RepositoryItemLookUpEditSimple luk_TargetType;

	private RepositoryItemButtonEditCFL btn_OwnerPer;

	private RepositoryItemButtonEditCFL btn_BpCode;

	private RepositoryItemLookUpEditSimple luk_StatusBp;

	private RepositoryItemButtonEditCFL btn_EmployeeNo;

	private RepositoryItemLookUpEditDefineNew luk_BranchMember;

	private RepositoryItemLookUpEditDefineNew luk_DepartmentMember;

	private ContextMenuStrip ctm_Task;

	private ToolStripMenuItem newTaskToolStripMenuItem;

	private RepositoryItemLookUpEditSimple luk_StatusDoc;

	private RepositoryItemLookUpEditSimple luk_Qualified;

	private TextEditEx txt_Id;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx1;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx2;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx3;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx4;

	private RepositoryItemTextEditExPrice repositoryItemTextEditExPrice1;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx5;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx6;

	private RepositoryItemTextEditExPrice repositoryItemTextEditExPrice2;

	private RepositoryItemTextEditExPrice repositoryItemTextEditExPrice3;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx7;

	private GridColumnEx gridColumnEx61;

	private RepositoryItemButtonEditCFL btn_ProjectId;

	private RepositoryItemLookUpEditDefineNew luk_Role;

	private RepositoryItemLookUpEditDefineNew luk_CampaignType;

	private RepositoryItemLookUpEditDefineNew luk_Stage;

	private RepositoryItemButtonEditCFL btn_ParentCampaign;

	private RepositoryItemButtonEditCFL btn_BpCustomer;

	private RepositoryItemButtonEditCFL btn_BpVender;

	private GridColumnEx gridColumnEx62;

	private GridColumnEx gridColumnEx63;

	private GridColumnEx gridColumnEx64;

	private RepositoryItemButtonEditCFL btn_ItemCode;

	private GridColumnEx gridColumnEx65;

	private GridColumnEx gridColumnEx66;

	private GridColumnEx gridColumnEx67;

	private GridColumnEx gridColumnEx68;

	private GridColumnEx gridColumnEx69;

	private GridControlEx grd_Partner;

	private GridViewEx gv_Partner;

	private GridColumnEx gcPartnerName;

	private GridColumnEx gcPartnerRelationShip;

	private GridColumnEx gcPartnerRelatedBP;

	private GridColumnEx gcPartnerRemark;

	private GridColumnEx gcDocEntry;

	private GridColumnEx gcPartnerLine;

	private XtraTabPageEx xtra_Sub;

	private GridControlEx grd_Sub;

	private GridViewEx gv_Sub;

	private GridColumnEx gridColumnEx70;

	private GridColumnEx gridColumnEx71;

	private RepositoryItemLookUpEditSimple luk_TargetTypeSub;

	private GridColumnEx gridColumnEx72;

	private GridColumnEx gridColumnEx73;

	private GridColumnEx gridColumnEx74;

	private RepositoryItemLookUpEditDefineNew luk_TypeSub;

	private GridColumnEx gridColumnEx75;

	private RepositoryItemButtonEditCFL btn_EmployeeSub;

	private GridColumnEx gridColumnEx76;

	private RepositoryItemLookUpEditSimple luk_StatusSub;

	private GridColumnEx gridColumnEx77;

	private RepositoryItemLookUpEditDefineNew luk_TargetGroupSub;

	private GridColumnEx gridColumnEx78;

	private GridColumnEx gridColumnEx79;

	private GridColumnEx gridColumnEx80;

	private RepositoryItemButtonEditCFL btn_ParentSub;

	private GridColumnEx gridColumnEx81;

	private RepositoryItemLookUpEditDefineNew luk_StageSub;

	private GridColumnEx gridColumnEx82;

	private GridColumnEx gridColumnEx83;

	private GridColumnEx gridColumnEx84;

	private GridColumnEx gridColumnEx85;

	private GridColumnEx gridColumnEx86;

	private GridColumnEx gridColumnEx87;

	private GridColumnEx gridColumnEx88;

	private GridColumnEx gridColumnEx89;

	private RepositoryItemButtonEditCFL btn_ProjectSub;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx8;

	private RepositoryItemTextEditExPrice repositoryItemTextEditExPrice4;

	private RepositoryItemTextEditExPrice repositoryItemTextEditExPrice5;

	private RepositoryItemLookUpEditDefineNew luk_PartnerId;

	private RepositoryItemLookUpEditDefineNew luk_Orl;

	private RepositoryItemButtonEditCFL btn_RelaPartner;

	private RepositoryItemLookUpEditDefineNew luk_BpGroup;

	private RepositoryItemLookUpEditDefineNew luk_ItemGroup;

	private GridColumnEx gridColumnEx90;

	private RepositoryItemLookUpEditDefineNew luk_ItmGrpCost;

	private RepositoryItemLookUpEdit luk_TargetGroup;

	private ContextMenuStrip ctm_Item;

	private ToolStripMenuItem addRow_Item;

	private ToolStripMenuItem deleteRow_Item;

	private ContextMenuStrip ctm_Partner;

	private ToolStripMenuItem addRow_Partner;

	private ToolStripMenuItem deleteRow_Partner;

	private XtraTabPageEx xtra_Document;

	private GridControlEx grd_LineDocument;

	private GridViewEx gv_LineDocument;

	private GridColumnEx gridColumnEx91;

	private GridColumnEx gridColumnEx92;

	private GridColumnEx gridColumnEx93;

	private GridColumnEx gridColumnEx94;

	private RepositoryItemButtonEditCFL btn_DocNumDo;

	private GridColumnEx gridColumnEx95;

	private GridColumnEx gridColumnEx96;

	private GridColumnEx gridColumnEx97;

	private GridColumnEx gridColumnEx98;

	private RepositoryItemLookUpEdit luk_CurrencyLineDu;

	private RepositoryItemButtonEditCFL btn_Territory;

	private GridColumnEx gridColumnEx99;

	private RepositoryItemLookUpEditDefineNew luk_Industry;

	private ToolStripMenuItem addSubToolStripMenuItem;

	private LabelLine labelLine5;

	private RepositoryItemButtonEditCFL btn_DocEntrySub;

	private GridColumnEx gridColumnEx100;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx9;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx10;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx11;

	private GridColumnEx gridColumnEx101;

	private GridColumnEx gridColumnEx102;

	private GridColumnEx gridColumnEx103;

	private GridColumnEx gridColumnEx104;

	private RepositoryItemCheckEdit cke_ShowBPDoc;

	private RepositoryItemLookUpEditSimple luk_DocType;

	private RepositoryItemButtonEdit btn_DocNum;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		try
		{
			_formMode = value;
			switch (value)
			{
			case Enums.FormViewModeType.Add:
				btn_Ok.Text = "Add";
				newTaskToolStripMenuItem.Visible = false;
				addSubToolStripMenuItem.Visible = false;
				break;
			case Enums.FormViewModeType.Update:
				btn_Ok.Text = "Update";
				newTaskToolStripMenuItem.Visible = true;
				addSubToolStripMenuItem.Visible = true;
				break;
			case Enums.FormViewModeType.View:
				btn_Ok.Text = "OK";
				newTaskToolStripMenuItem.Visible = true;
				addSubToolStripMenuItem.Visible = true;
				break;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public frm_MarketingCampaign()
	{
		InitializeComponent();
		OnLoadAsync += frm_MarketingCampaign_Load;
		RepositoryItemButtonEditCFL repositoryItemButtonEditCFL = btn_BpCode;
		repositoryItemButtonEditCFL.AfterShowCFL = (Action<frm_CFL>)Delegate.Combine(repositoryItemButtonEditCFL.AfterShowCFL, (Action<frm_CFL>)((frm_CFL frm) =>
		{
			try
			{
				_ = frm.gv_CFL.GridControl.DataSource;
				string text = Function.ToString(gv_Doc.GetFocusedRowCellValue("TargetType"));
				if (!string.IsNullOrWhiteSpace(text))
				{
					if (text == "C")
					{
						frm.gv_CFL.GridControl.DataSource = btn_BpCustomer.DataSource;
					}
					else if (text == "V")
					{
						frm.gv_CFL.GridControl.DataSource = btn_BpVender.DataSource;
					}
				}
			}
			catch (Exception ex)
			{
				WinCommons.ErrorLineNumber(ex);
			}
		}));
	}

	private async Task frm_MarketingCampaign_Load(object sender, EventArgs e)
	{
		_ = 3;
		try
		{
			_isLoading = true;
			FormMode = Enums.FormViewModeType.View;
			LoadRepostoryCFLDataSource();
			LoadTargetGroup();
			luk_BranchMember.ValueMember = "Code";
			luk_BranchMember.DisplayMember = "Name";
			luk_BranchMember.FormProperties.TableName = "OUBR";
			luk_DepartmentMember.ValueMember = "Code";
			luk_DepartmentMember.DisplayMember = "Name";
			luk_DepartmentMember.FormProperties.TableName = "OUDP";
			luk_Role.ValueMember = "typeID";
			luk_Role.DisplayMember = "name";
			luk_Role.FormProperties.TableName = "ohty";
			luk_CampaignType.ValueMember = "Code";
			luk_CampaignType.DisplayMember = "Name";
			luk_CampaignType.FormProperties.TableName = "APZ_SMCT";
			luk_TypeSub.ValueMember = "Code";
			luk_TypeSub.DisplayMember = "Name";
			luk_TypeSub.FormProperties.TableName = "APZ_SMCT";
			luk_Stage.ValueMember = "Code";
			luk_Stage.DisplayMember = "Name";
			luk_Stage.FormProperties.TableName = "APZ_SMCS";
			luk_StageSub.ValueMember = "Code";
			luk_StageSub.DisplayMember = "Name";
			luk_StageSub.FormProperties.TableName = "APZ_SMCS";
			luk_PartnerId.ValueMember = "PrtId";
			luk_PartnerId.DisplayMember = "Name";
			luk_PartnerId.FormProperties.TableName = "OPRT";
			luk_PartnerId.RefreshDataSource();
			luk_Orl.ValueMember = "OrlCode";
			luk_Orl.DisplayMember = "OrlDesc";
			luk_Orl.FormProperties.TableName = "OORL";
			luk_Orl.RefreshDataSource();
			luk_BpGroup.ValueMember = "GroupCode";
			luk_BpGroup.DisplayMember = "GroupName";
			luk_BpGroup.FormProperties.TableName = "OCRG";
			luk_BpGroup.RefreshDataSource();
			luk_ItemGroup.ValueMember = "ItmsGrpCod";
			luk_ItemGroup.DisplayMember = "ItmsGrpNam";
			luk_ItemGroup.FormProperties.TableName = "OITB";
			luk_ItemGroup.RefreshDataSource();
			luk_ItmGrpCost.ValueMember = "ItmsGrpCod";
			luk_ItmGrpCost.DisplayMember = "ItmsGrpNam";
			luk_ItmGrpCost.FormProperties.TableName = "OITB";
			luk_ItmGrpCost.RefreshDataSource();
			luk_Industry.ValueMember = "IndCode";
			luk_Industry.DisplayMember = "IndName";
			luk_Industry.FormProperties.TableName = "OOND";
			luk_Industry.RefreshDataSource();
			await Task.WhenAll(luk_BranchMember.RefreshDataSourceAsync(), luk_DepartmentMember.RefreshDataSourceAsync(), luk_Role.RefreshDataSourceAsync(), luk_CampaignType.RefreshDataSourceAsync(), luk_TypeSub.RefreshDataSourceAsync(), luk_Stage.RefreshDataSourceAsync(), luk_StageSub.RefreshDataSourceAsync());
			luk_TargetType.SetDataSource(new Dictionary<string, string>
			{
				{ "C", "Customer/Lead" },
				{ "V", "Vendor" }
			});
			luk_TargetTypeSub.SetDataSource(new Dictionary<string, string>
			{
				{ "C", "Customer/Lead" },
				{ "V", "Vendor" }
			});
			luk_StatusDoc.SetDataSource(new Dictionary<string, string>
			{
				{ "O", "Open" },
				{ "C", "Closed" }
			});
			luk_StatusSearch.SetDataSource(new Dictionary<string, string>
			{
				{ "", "" },
				{ "O", "Open" },
				{ "C", "Closed" }
			});
			luk_StatusSub.SetDataSource(new Dictionary<string, string>
			{
				{ "O", "Open" },
				{ "C", "Closed" }
			});
			luk_Qualified.SetDataSource(new Dictionary<string, string>
			{
				{ "Y", "Yes" },
				{ "N", "No" }
			});
			luk_StatusBp.SetDataSource(new Dictionary<string, string>
			{
				{ "O", "Open" },
				{ "S", "Send" }
			});
			luk_DocType.NullValuePrompt = "Document Type...";
			luk_DocType.Properties.NullValuePromptShowForEmptyValue = true;
			btn_DocNum.NullValuePrompt = "Document Number...";
			btn_OwnerPer.ValueMember = "EmpId";
			btn_OwnerPer.DisplayMember = "lastName";
			btn_OwnerPer.DisplayMember2 = "firstName";
			btn_OwnerPer.SeparateString = " ";
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL = btn_OwnerPer;
			repositoryItemButtonEditCFL.DataSourceLoaded = (Action<DataTable>)Delegate.Combine(repositoryItemButtonEditCFL.DataSourceLoaded, (Action<DataTable>)((DataTable dtData) =>
			{
				btn_EmployeeNo.DataSource = dtData.Copy();
			}));
			btn_OwnerPer.TypeCFL = Enums.TypeCFL.EmployeeListForSalesEmpSetup;
			btn_OwnerPer.RefreshDataSource();
			btn_OwnerPer.TypeDrillDown = ApzObjectTypes.oEmployeesInfo;
			btn_EmployeeSub.ValueMember = "EmpId";
			btn_EmployeeSub.DisplayMember = "lastName";
			btn_EmployeeSub.DisplayMember2 = "firstName";
			btn_EmployeeSub.SeparateString = " ";
			btn_EmployeeSub.TypeCFL = Enums.TypeCFL.EmployeeListForSalesEmpSetup;
			btn_EmployeeSub.RefreshDataSource();
			btn_EmployeeSub.TypeDrillDown = ApzObjectTypes.oEmployeesInfo;
			btn_EmployeeNo.ValueMember = "empID";
			btn_EmployeeNo.DisplayMember = "empID";
			btn_EmployeeNo.SeparateString = " ";
			btn_EmployeeNo.FindCFLBy(Enums.TypeCFL.EmployeeListForSalesEmpSetup, true, "", Enums.ItemObjectType.NoItemType, null);
			btn_EmployeeNo.HandleChooseItemMulti = ChooseEmployeeNoReceiptMulti;
			btn_EmployeeNo.RefreshDataSource();
			btn_EmployeeNo.TypeDrillDown = ApzObjectTypes.oEmployeesInfo;
			btn_ProjectId.ValueMember = "PrjCode";
			btn_ProjectId.TypeCFL = Enums.TypeCFL.Project;
			btn_ProjectId.RefreshDataSource();
			btn_ProjectId.TypeDrillDown = ApzObjectTypes.oProjectCodes;
			btn_ProjectSub.ValueMember = "PrjCode";
			btn_ProjectSub.TypeCFL = Enums.TypeCFL.Project;
			btn_ProjectSub.RefreshDataSource();
			btn_ProjectSub.TypeDrillDown = ApzObjectTypes.oProjectCodes;
			btn_ParentCampaign.ValueMember = "DocEntry";
			btn_ParentCampaign.DisplayMember = "CampaignNo";
			btn_ParentCampaign.TypeCFL = Enums.TypeCFL.MarketingCampaign;
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL2 = btn_ParentCampaign;
			repositoryItemButtonEditCFL2.BeforeShowCFL = (Action<frm_CFL>)Delegate.Combine(repositoryItemButtonEditCFL2.BeforeShowCFL, (Action<frm_CFL>)((frm_CFL frm) =>
			{
				frm.campaignId = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("DocEntry"));
			}));
			btn_ParentCampaign.RefreshDataSource();
			btn_ParentCampaign.TypeDrillDown = ApzObjectTypes.oMarketingCampaign;
			btn_ParentSub.ValueMember = "DocEntry";
			btn_ParentSub.DisplayMember = "CampaignNo";
			btn_ParentSub.TypeCFL = Enums.TypeCFL.MarketingCampaign;
			btn_ParentSub.RefreshDataSource();
			btn_ParentSub.TypeDrillDown = ApzObjectTypes.oMarketingCampaign;
			btn_DocEntrySub.ValueMember = "DocEntry";
			btn_DocEntrySub.TypeCFL = Enums.TypeCFL.MarketingCampaign;
			btn_DocEntrySub.RefreshDataSource();
			btn_DocEntrySub.TypeDrillDown = ApzObjectTypes.oMarketingCampaign;
			btn_ItemCost.ValueMember = "ItemCode";
			btn_ItemCost.FindCFLBy(Enums.TypeCFL.ListItems, true, "", Enums.ItemObjectType.NoItemType, null);
			btn_ItemCost.TypeDrillDown = ApzObjectTypes.oItems;
			btn_ItemCost.ListMandatoryField = "ItmsGrpCod,ItemName";
			btn_ItemCost.HandleChooseItemMulti = ChooseItemReceiptMulti;
			btn_ItemCost.RefreshDataSource();
			btn_ItemCode.ValueMember = "ItemCode";
			btn_ItemCode.FindCFLBy(Enums.TypeCFL.ListItems, true, "", Enums.ItemObjectType.NoItemType, null);
			btn_ItemCode.TypeDrillDown = ApzObjectTypes.oItems;
			btn_ItemCode.ListMandatoryField = "InvntryUom,ItmsGrpCod,ItemName";
			btn_ItemCode.HandleChooseItemMulti = ChooseItemCodeReceiptMulti;
			btn_ItemCode.RefreshDataSource();
			btn_RelaPartner.ValueMember = "CardCode";
			btn_RelaPartner.TypeCFL = Enums.TypeCFL.Customer;
			btn_RelaPartner.RefreshDataSource();
			btn_RelaPartner.TypeDrillDown = ApzObjectTypes.oBusinessPartners;
			btn_Territory.ValueMember = "territryID";
			btn_Territory.DisplayMember = "descript";
			btn_Territory.TypeCFL = Enums.TypeCFL.Territory;
			btn_Territory.RefreshDataSource();
			btn_BpCode.ValueMember = "CardCode";
			btn_BpCode.FindCFLBy(Enums.TypeCFL.Bp, true, "", Enums.ItemObjectType.NoItemType, null);
			btn_BpCode.HandleChooseItemMulti = ChooseBpReceiptMulti;
			btn_BpCode.RefreshDataSource();
			btn_BpCode.TypeDrillDown = ApzObjectTypes.oBusinessPartners;
			btn_DocNum.Buttons[0].Width = 10;
			btn_DocNum.Buttons[0].Kind = ButtonPredefines.Search;
			btn_DocNum.Buttons[0].Click += btn_DocNum_Pressed;
			btn_DocNumDo.ValueMember = "DocNum";
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL3 = btn_DocNumDo;
			repositoryItemButtonEditCFL3.DrillDownMethod = (Action<string>)Delegate.Combine(repositoryItemButtonEditCFL3.DrillDownMethod, (Action<string>)((string editorValue) =>
			{
				string text = Function.ToString(gv_LineDocument.GetFocusedRowCellValue("ObjType"));
				ApzObjectTypes type = ApzObjectTypes.oNone;
				switch (text)
				{
				case "23":
					type = ApzObjectTypes.oQuotations;
					break;
				case "17":
					type = ApzObjectTypes.oOrders;
					break;
				case "1470000113":
					type = ApzObjectTypes.oPurchaseRequest;
					break;
				case "20":
					type = ApzObjectTypes.oPurchaseDeliveryNotes;
					break;
				case "22":
					type = ApzObjectTypes.oPurchaseOrders;
					break;
				case "15":
					type = ApzObjectTypes.oDeliveryNotes;
					break;
				case "18":
					type = ApzObjectTypes.oPurchaseInvoices;
					break;
				case "13":
					type = ApzObjectTypes.oInvoices;
					break;
				case "540000006":
					type = ApzObjectTypes.oPurchaseQuotations;
					break;
				case "203":
					type = ApzObjectTypes.oDownPayments;
					break;
				case "204":
					type = ApzObjectTypes.oPurchaseDownPayments;
					break;
				}
				this.DrillDown(type, editorValue);
			}));
			btn_DocNumDo.RefreshDataSource();
			DataSet dataSet = await _httpClient.Cfl.GetUserList();
			new DataTable();
			DataTable dataTable = ((!dataSet.IsHasStructure()) ? dataSet.Tables[0] : dataSet.Tables[1]);
			if (dataTable.IsNotNull() && dataTable.ExistsColumn("USERID") && dataTable.ExistsColumn("U_NAME"))
			{
				dictionUser = dataTable.AsEnumerable().ToDictionary((DataRow s) => Function.ParseInt(s["USERID"]), (DataRow s) => Function.ToString(s["U_NAME"]));
			}
			DataSet employeeListForSalesEmpSetup = _httpClient.Cfl.GetEmployeeListForSalesEmpSetup();
			new DataTable();
			DataTable dataTable2 = ((!employeeListForSalesEmpSetup.IsHasStructure()) ? employeeListForSalesEmpSetup.Tables[0] : employeeListForSalesEmpSetup.Tables[1]);
			if (dataTable2.IsNotNull() && dataTable2.ExistsColumn("empID") && dataTable2.ExistsColumn("lastName") && dataTable2.ExistsColumn("firstName"))
			{
				dictionEmployee = dataTable2.AsEnumerable().ToDictionary((DataRow s) => Function.ParseInt(s["empID"]), (DataRow s) => Function.ToString(s["firstName"]) + "," + Function.ToString(s["lastName"]));
			}
			await Task.WhenAll(WinFuntions.LoadUdfColumns(gv_Doc, "APZ_OSMC"), WinFuntions.LoadUdfColumns(gv_Bp, "APZ_SMC1"), WinFuntions.LoadUdfColumns(gv_Item, "APZ_SMC2"), WinFuntions.LoadUdfColumns(gv_Partner, "APZ_SMC3"), WinFuntions.LoadUdfColumns(gv_Member, "APZ_SMC4"), WinFuntions.LoadUdfColumns(gv_Cost, "APZ_SMC5"), WinFuntions.LoadUdfColumns(gv_ListCompleted, "OCLG"), WinFuntions.LoadUdfColumns(gv_Sub, "APZ_OSMC"));
			new DataTable();
			string code = Function.ToString(txt_No.EditValue);
			string status = Function.ToString(luk_StatusSearch.EditValue);
			date_FromDate.EditValue = DateTime.Today.AddDays(-Configuration.SearchDate);
			date_ToDate.EditValue = DateTime.Today.AddDays(Configuration.SearchDate);
			string startFrom = (Function.ParseDateTime(date_FromDate.EditValue).HasValue ? ((DateTime)date_FromDate.EditValue).ToString("yyyyMMdd") : "");
			string startTo = (Function.ParseDateTime(date_ToDate.EditValue).HasValue ? ((DateTime)date_ToDate.EditValue).ToString("yyyyMMdd") : "");
			if (CampaignId > 0)
			{
				txt_Id.EditValue = CampaignId;
			}
			DataTable dataTable3 = await LoadData(Function.ParseInt(txt_Id.EditValue), code, startFrom, startTo, status);
			CampaignId = 0;
			grd_Doc.DataSource = dataTable3;
			grd_Doc.DatasourceTable().AcceptChanges();
			if (!dataTable3.IsNotNull())
			{
				gv_Doc_FocusedRowObjectChanged(null, null);
			}
			gv_Doc.FocusedRowHandle = 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Search_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			string code = Function.ToString(txt_No.EditValue);
			string status = Function.ToString(luk_StatusSearch.EditValue);
			string startFrom = (Function.ParseDateTime(date_FromDate.EditValue).HasValue ? ((DateTime)date_FromDate.EditValue).ToString("yyyyMMdd") : "");
			string startTo = (Function.ParseDateTime(date_ToDate.EditValue).HasValue ? ((DateTime)date_ToDate.EditValue).ToString("yyyyMMdd") : "");
			if (CampaignId > 0)
			{
				txt_Id.EditValue = CampaignId;
			}
			DataTable dataSource = await LoadData(Function.ParseInt(txt_Id.EditValue), code, startFrom, startTo, status);
			CampaignId = 0;
			if (!_isLoading && ((FormMode != Enums.FormViewModeType.Add && FormMode != Enums.FormViewModeType.Update) || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes))
			{
				FormMode = Enums.FormViewModeType.View;
				grd_Bp.DataSourceTable().Rows.Clear();
				grd_Member.DataSourceTable().Rows.Clear();
				grd_Cost.DataSourceTable.Rows.Clear();
				grd_Doc.DataSource = dataSource;
				grd_Doc.DatasourceTable().AcceptChanges();
				int id = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("DocEntry"));
				await LoadLineById(id);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Ok_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			switch (FormMode)
			{
			case Enums.FormViewModeType.Add:
				await Save(1);
				break;
			case Enums.FormViewModeType.Update:
				await Save(2);
				break;
			case Enums.FormViewModeType.View:
				Close();
				break;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private async void gv_Doc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		DataChanged();
		if (e.Column.FieldName == "TargetType")
		{
			for (int i = 0; i < gv_Bp.RowCount; i++)
			{
				gv_Bp.SetRowCellValue(i, "DocType", "");
			}
			SetSourceLukDocType();
		}
		else if (e.Column.FieldName == "TargetGroup")
		{
			if (string.IsNullOrWhiteSpace(Function.ToString(e.Value)))
			{
				return;
			}
			if (XtraMessageBoxEx.Show(Enums.MessageCode.SpecifiedNewTargetGroup, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				DataTable dataTable = await _httpClient.MarketingCampaign.LoadTargetGroupDetails(Function.ToString(e.Value));
				if (dataTable.IsNotNull())
				{
					grd_Bp.DataSourceTable.Rows.Clear();
					grd_Bp.DataSourceTable.AcceptChanges();
					for (int j = 0; j < dataTable.Rows.Count; j++)
					{
						NewBp();
						gv_Bp.MoveNext();
						gv_Bp.SetFocusedRowCellValue("BpCode", dataTable.Rows[j]["CardCode"]);
					}
					NewBp();
				}
			}
			else
			{
				gv_Doc.SetFocusedRowCellValue("TargetGroup", string.Empty);
			}
		}
		else
		{
			if (!(e.Column.FieldName == "ParentCampaign") || Function.ParseInt(e.Value) == 0)
			{
				return;
			}
			DataTable dataTable2 = await _httpClient.MarketingCampaign.GetDataTargetBPById(Function.ParseInt(e.Value));
			if (!dataTable2.IsNotNull())
			{
				return;
			}
			if (CheckDataTargetBP(grd_Bp.DatasourceTable()))
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDeleteItemWhenExistsChild, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
				{
					int num = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("DocEntry"));
					for (int k = 0; k < dataTable2.Rows.Count; k++)
					{
						dataTable2.Rows[k]["DocEntry"] = num;
					}
					grd_Bp.DataSourceTable.Clear();
					grd_Bp.DataSource = dataTable2;
				}
				else
				{
					DataTable dataTable3 = (from r in grd_Bp.DataSourceTable.AsEnumerable()
						where !string.IsNullOrEmpty(Function.ToString(r["BpCode"]))
						select r).CopyToDataTable();
					int num2 = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("DocEntry"));
					int num3 = Function.ParseInt(dataTable3.Compute("MAX(LineNum)", ""));
					for (int num4 = 0; num4 < dataTable2.Rows.Count; num4++)
					{
						num3++;
						dataTable2.Rows[num4]["LineNum"] = num3;
						dataTable2.Rows[num4]["DocEntry"] = num2;
						dataTable3.ImportRow(dataTable2.Rows[num4]);
					}
					grd_Bp.DataSourceTable.Clear();
					grd_Bp.DataSource = dataTable3;
				}
			}
			else
			{
				int num5 = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("DocEntry"));
				for (int num6 = 0; num6 < dataTable2.Rows.Count; num6++)
				{
					dataTable2.Rows[num6]["DocEntry"] = num5;
				}
				grd_Bp.DataSourceTable.Clear();
				grd_Bp.DataSource = dataTable2;
			}
			NewBp();
		}
	}

	private async void gv_Doc_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Add)
			{
				int id = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("DocEntry"));
				await LoadLineById(id);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_BeforeLeaveRow(object sender, RowAllowEventArgs e)
	{
		try
		{
			if (!_isLoading && FormMode == Enums.FormViewModeType.Update)
			{
				DialogResult dialogResult = XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
				if (dialogResult == DialogResult.Yes && dialogResult == DialogResult.Yes)
				{
					grd_Doc.DataSourceTable.RejectChanges();
					FormMode = Enums.FormViewModeType.View;
				}
				if (dialogResult == DialogResult.No)
				{
					e.Allow = false;
				}
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_ListCompleted_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == "Closed")
			{
				string text = Function.ToString(gv_ListCompleted.GetRowCellValue(e.RowHandle, e.Column));
				if (text == "Y")
				{
					e.RepositoryItem = rep_bte_Closed;
				}
				else if (text == "N")
				{
					e.RepositoryItem = rep_bte_Open;
				}
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_ListCompleted_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
	{
		try
		{
			if (!(sender is GridView gridView) || e.Column.FieldName != "AssignTo" || !e.IsGetData)
			{
				return;
			}
			int listSourceRowIndex = e.ListSourceRowIndex;
			string text = Function.ToString(gridView.GetListSourceRowCellValue(listSourceRowIndex, "GroupUser"));
			string text2 = Function.ToString(gridView.GetListSourceRowCellValue(listSourceRowIndex, "GroupEmployee"));
			string text3 = "";
			if (!string.IsNullOrEmpty(text) && string.IsNullOrEmpty(text2))
			{
				foreach (string item in text.Split(new char[1] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList())
				{
					int key = Function.ParseInt(item);
					if (dictionUser.ContainsKey(key))
					{
						text3 = text3 + dictionUser[key] + "; ";
					}
				}
			}
			if (!string.IsNullOrEmpty(text2))
			{
				foreach (string item2 in text2.Split(new char[1] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList())
				{
					int key2 = Function.ParseInt(item2);
					if (dictionEmployee.ContainsKey(key2))
					{
						text3 = text3 + dictionEmployee[key2] + "; ";
					}
				}
			}
			if (text3.Length > 2)
			{
				text3 = text3.Remove(text3.Length - 2);
			}
			e.Value = text3;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_ListCompleted_DoubleClick(object sender, EventArgs e)
	{
		try
		{
			if (gv_ListCompleted.FocusedRowHandle >= 0)
			{
				int num = Function.ParseInt(gv_ListCompleted.GetFocusedRowCellValue("ClgCode"));
				if (num > 0)
				{
					DataTable dataTable = new DataTable
					{
						Columns = { 
						{
							"ClgCode",
							typeof(int)
						} }
					};
					DataRow dataRow = dataTable.NewRow();
					dataRow["ClgCode"] = num;
					dataTable.Rows.Add(dataRow);
					frm_TaskManagement frm_TaskManagement2 = new frm_TaskManagement
					{
						MdiParent = MdiParent,
						CallFromCal = true,
						DataRowFromCal = dataRow
					};
					frm_TaskManagement2.ReloadCalendar = (Func<Task>)Delegate.Combine(frm_TaskManagement2.ReloadCalendar, new Func<Task>(ReloadTaskList));
					frm_TaskManagement2.ReloadAfterRemove = (Func<int, Task>)Delegate.Combine(frm_TaskManagement2.ReloadAfterRemove, new Func<int, Task>(ReloadTaskList));
					frm_TaskManagement2.Show();
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.NoMatchingRecordFound, "");
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_ListCompleted_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == "AssignTo")
			{
				string value = Function.ToString(gv_ListCompleted.GetRowCellValue(e.RowHandle, "GroupUser"));
				string value2 = Function.ToString(gv_ListCompleted.GetRowCellValue(e.RowHandle, "GroupEmployee"));
				if (string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(value2))
				{
					e.Appearance.ForeColor = Color.FromArgb(125, 88, 0);
				}
			}
			string a = Function.ToString(gv_Doc.GetFocusedRowCellValue("Status"));
			if (string.Equals(a, "C") || string.Equals(a, "L"))
			{
				e.Appearance.BackColor = GridColor.GridReadOnly;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task ReloadTaskList()
	{
		try
		{
			DataTable dataSource = await _httpClient.SubActivities.LoadTaskByObjectType(GlobalData.UserCodeLogin, ApzObjectTypes.oMarketingCampaign, Function.ToString(gv_Doc.GetFocusedRowCellValue("DocEntry")));
			grd_ListCompleted.DataSource = dataSource;
			grd_ListCompleted.RefreshDataSource();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task ReloadTaskList(int clgCode)
	{
		try
		{
			DataTable dataSource = await _httpClient.SubActivities.LoadTaskByObjectType(GlobalData.UserCodeLogin, ApzObjectTypes.oMarketingCampaign, Function.ToString(gv_Doc.GetFocusedRowCellValue("DocEntry")));
			grd_ListCompleted.DataSource = dataSource;
			grd_ListCompleted.RefreshDataSource();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void newTaskToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			frm_TaskManagement frm_TaskManagement2 = new frm_TaskManagement
			{
				ClgCode = 0,
				StartPosition = FormStartPosition.CenterScreen,
				MdiParent = MdiParent,
				CallFromCal = true
			};
			int num = 1470000145;
			int num2 = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("DocEntry"));
			DataTable tableStructure = WinFuntions.GetTableStructure("APZ_OCLG");
			DataRow dataRow = tableStructure.NewRow();
			dataRow["GroupUser"] = GlobalData.UserID;
			if (Function.ToString(dataRow["Recontact"]) == "")
			{
				dataRow["Recontact"] = DateTime.Today;
				string text = DateTime.Now.Minute.ToString();
				dataRow["BeginTime"] = DateTime.Now.Hour + ((text.Length == 2) ? text : ("0" + text));
			}
			dataRow["Closed"] = "N";
			dataRow["DocType"] = num;
			dataRow["DocEntry"] = num2;
			dataRow["DocNum"] = num2;
			tableStructure.Rows.Add(dataRow);
			tableStructure.AcceptChanges();
			frm_TaskManagement2.DataRowFromCal = dataRow;
			frm_TaskManagement2.ReloadCalendar = (Func<Task>)Delegate.Combine(frm_TaskManagement2.ReloadCalendar, new Func<Task>(ReloadTaskList));
			frm_TaskManagement2.ReloadAfterRemove = (Func<int, Task>)Delegate.Combine(frm_TaskManagement2.ReloadAfterRemove, new Func<int, Task>(ReloadTaskList));
			frm_TaskManagement2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Cost_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == "Qty")
			{
				double num = Function.ParseDouble(e.Value);
				double num2 = Function.ParseDouble(gv_Cost.GetRowCellValue(e.RowHandle, "RefPrice"));
				double num3 = Math.Round(num * num2, 6);
				gv_Cost.SetRowCellValue(e.RowHandle, "BudgetValue", num3);
			}
			if (e.Column.FieldName == "RefPrice")
			{
				double num4 = Function.ParseDouble(gv_Cost.GetRowCellValue(e.RowHandle, "Qty"));
				double num5 = Function.ParseDouble(e.Value);
				double num6 = Math.Round(num4 * num5, 6);
				gv_Cost.SetRowCellValue(e.RowHandle, "BudgetValue", num6);
			}
			if (e.Column.FieldName == "BudgetValue")
			{
				double num7 = Function.ParseDouble(gv_Cost.GetRowCellValue(e.RowHandle, "ActualCost"));
				double num8 = Function.ParseDouble(e.Value);
				double num9 = Math.Round(num7 * 100.0 / num8, 6);
				gv_Cost.SetRowCellValue(e.RowHandle, "Diff", num9);
			}
			if (e.Column.FieldName == "ActualCost")
			{
				double num10 = Function.ParseDouble(e.Value);
				double num11 = Function.ParseDouble(gv_Cost.GetRowCellValue(e.RowHandle, "BudgetValue"));
				double num12 = Math.Round(num10 * 100.0 / num11, 6);
				gv_Cost.SetRowCellValue(e.RowHandle, "Diff", num12);
			}
			if (e.Column.FieldName == "ActualQty")
			{
				double num13 = Function.ParseDouble(e.Value);
				double num14 = Function.ParseDouble(gv_Cost.GetRowCellValue(e.RowHandle, "ActualPrice"));
				double num15 = Math.Round(num13 * num14, 6);
				gv_Cost.SetRowCellValue(e.RowHandle, "ActualCost", num15);
			}
			if (e.Column.FieldName == "ActualPrice")
			{
				double num16 = Function.ParseDouble(e.Value);
				double num17 = Math.Round(Function.ParseDouble(gv_Cost.GetRowCellValue(e.RowHandle, "ActualQty")) * num16, 6);
				gv_Cost.SetRowCellValue(e.RowHandle, "ActualCost", num17);
			}
			if (e.Column.FieldName == "ItemCode")
			{
				string itemCode = Function.ToString(e.Value);
				(from t in btn_ItemCost.DataSource.AsEnumerable()
					where Function.ToString(t["ItemCode"]) == itemCode
					select t).FirstOrDefault();
			}
			_dtCost = grd_Cost.DataSourceTable;
			if ((from t in _dtCost.AsEnumerable()
				where string.IsNullOrWhiteSpace(Function.ToString(t["ItemCode"]))
				select t).ToList().Count <= 0)
			{
				NewCost();
			}
			DataChanged();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Item_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			_dtItem = grd_Item.DatasourceTable();
			if (e.Column.FieldName == "Qty")
			{
				double num = Function.ParseDouble(e.Value);
				double num2 = Function.ParseDouble(gv_Item.GetRowCellValue(e.RowHandle, "RefPrice"));
				double num3 = num * num2;
				gv_Item.SetRowCellValue(e.RowHandle, "RevenueValues", num3);
			}
			if (e.Column.FieldName == "RefPrice")
			{
				double num4 = Function.ParseDouble(gv_Item.GetRowCellValue(e.RowHandle, "Qty"));
				double num5 = Function.ParseDouble(e.Value);
				double num6 = num4 * num5;
				gv_Item.SetRowCellValue(e.RowHandle, "RevenueValues", num6);
			}
			if (e.Column.FieldName == "ItemCode")
			{
				string itemCode = Function.ToString(e.Value);
				DataRow dataRow = (from t in btn_ItemCode.DataSource.AsEnumerable()
					where Function.ToString(t["ItemCode"]) == itemCode
					select t).FirstOrDefault();
				if (dataRow != null)
				{
					gv_Item.SetRowCellValue(e.RowHandle, "ItemName", Function.ToString(dataRow["ItemName"]));
					gv_Item.SetRowCellValue(e.RowHandle, "ItemGroup", Function.ParseInt(dataRow["ItmsGrpCod"]));
					gv_Item.SetRowCellValue(e.RowHandle, "UoM", Function.ToString(dataRow["InvntryUom"]));
				}
			}
			if (e.Column.FieldName == "RevenueValues")
			{
				double num7 = _dtItem.AsEnumerable().Sum((DataRow t) => Function.ParseDouble(t["RevenueValues"]));
				gv_Doc.SetFocusedRowCellValue("TotalPlRevenue", num7);
			}
			_dtItem = grd_Item.DataSourceTable;
			if ((from t in _dtItem.AsEnumerable()
				where string.IsNullOrWhiteSpace(Function.ToString(t["ItemCode"]))
				select t).ToList().Count <= 0)
			{
				NewItem();
			}
			DataChanged();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void addNewToolStripMenuItem_Click(object sender, EventArgs e)
	{
		await REFRESH();
	}

	private async void deleteToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (gv_Doc.GetSelectedRows() == null && gv_Doc.GetSelectedRows().Count() == 0)
			{
				WinCommons.Error(Enums.MessageCode.InfoNoRowSelected, "");
				return;
			}
			Function.ParseInt(gv_Doc.GetFocusedRowCellValue("DocEntry"));
			string text = Function.ToString(gv_Doc.GetFocusedRowCellValue("CampaignNo"));
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmRemoveCampaign, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, text) == DialogResult.Yes)
			{
				string id = Function.ToString(gv_Doc.GetFocusedRowCellValue("DocEntry"));
				string text2 = await _httpClient.MarketingCampaign.DeleteDocs(id);
				if (string.IsNullOrEmpty(text2))
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
					btn_Search_Click(null, null);
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, text2);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void addRow_Bp_Click(object sender, EventArgs e)
	{
		NewBp();
	}

	private void deleteRow_Bp_Click(object sender, EventArgs e)
	{
		gv_Bp.DeleteSelectedRows();
		grd_Bp.DatasourceTable().AcceptChanges();
		DataChanged();
	}

	private void addRow_member_Click(object sender, EventArgs e)
	{
		NewMember();
	}

	private void deleteRow_member_Click(object sender, EventArgs e)
	{
		gv_Member.DeleteSelectedRows();
		grd_Member.DatasourceTable().AcceptChanges();
		DataChanged();
	}

	private void addRow_Cost_Click(object sender, EventArgs e)
	{
		NewCost();
	}

	private void deleteRow_Cost_Click(object sender, EventArgs e)
	{
		gv_Cost.DeleteSelectedRows();
		grd_Cost.DatasourceTable().AcceptChanges();
		DataChanged();
	}

	private void addRow_Item_Click(object sender, EventArgs e)
	{
		NewItem();
	}

	private void deleteRow_Item_Click(object sender, EventArgs e)
	{
		gv_Item.DeleteSelectedRows();
		grd_Item.DatasourceTable().AcceptChanges();
		DataChanged();
	}

	private void addRow_Partner_Click(object sender, EventArgs e)
	{
		NewPartner();
	}

	private void deleteRow_Partner_Click(object sender, EventArgs e)
	{
		gv_Partner.BeginUpdate();
		gv_Partner.DeleteSelectedRows();
		grd_Partner.DatasourceTable().AcceptChanges();
		gv_Partner.EndUpdate();
		DataChanged();
	}

	private async void gv_Bp_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == "BpCode")
			{
				string code = Function.ToString(e.Value);
				Function.ToString(gv_Doc.GetFocusedRowCellValue("TargetType"));
				DataRow itemInfor = (from t in btn_BpCode.DataSource.AsEnumerable()
					where Function.ToString(t["CardCode"]) == code
					select t).FirstOrDefault();
				if (itemInfor != null)
				{
					DataTable dataTable = await _httpClient.BusinessMaster.GetADoc(Function.ToString(itemInfor["CardCode"]));
					if (dataTable != null && dataTable.Rows.Count > 0)
					{
						gv_Bp.SetRowCellValue(e.RowHandle, "BpName", Function.ToString(itemInfor["CardName"]));
						gv_Bp.SetRowCellValue(e.RowHandle, "BpGroup", Function.ParseInt(dataTable.Rows[0]["GroupCode"]));
						gv_Bp.SetRowCellValue(e.RowHandle, "BpAddress", Function.ToString(dataTable.Rows[0]["Address"]));
						gv_Bp.SetRowCellValue(e.RowHandle, "Industry", Function.ParseInt(dataTable.Rows[0]["IndustryC"]));
						gv_Bp.SetRowCellValue(e.RowHandle, "Territory", Function.ParseInt(dataTable.Rows[0]["Territory"]));
					}
					DataTable dataTable2 = _httpClient.BusinessMaster.ShowContactPerson(Function.ToString(itemInfor["CardCode"]));
					if (dataTable2 != null && dataTable2.Rows.Count > 0)
					{
						string value = Function.ToString(dataTable.Rows[0]["CntctPrsn"]);
						DataRow dataRow = dataTable2.AsEnumerable().FirstOrDefault((DataRow t) => string.Equals(Function.ToString(t["Name"]), value));
						if (dataRow != null)
						{
							gv_Bp.SetRowCellValue(e.RowHandle, "ContactName", Function.ToString(dataRow["Name"]));
							gv_Bp.SetRowCellValue(e.RowHandle, "ContactEmail", Function.ToString(dataRow["E_MailL"]));
							gv_Bp.SetRowCellValue(e.RowHandle, "ContactPhone", Function.ToString(dataRow["Cellolar"]));
							gv_Bp.SetRowCellValue(e.RowHandle, "ContactPosition", Function.ToString(dataRow["Position"]));
						}
					}
				}
			}
			if (e.Column.FieldName == "DocType")
			{
				gv_Bp.SetRowCellValue(gv_Bp.FocusedRowHandle, "DocNumber", "");
			}
			_dtLine = grd_Bp.DataSourceTable;
			if ((from t in _dtLine.AsEnumerable()
				where string.IsNullOrWhiteSpace(Function.ToString(t["BpCode"]))
				select t).ToList().Count <= 0)
			{
				NewBp();
			}
			DataChanged();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Partner_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			_dtPartner = grd_Partner.DataSourceTable;
			if ((from t in _dtPartner.AsEnumerable()
				where string.IsNullOrWhiteSpace(Function.ToString(t["ParterId"]))
				select t).ToList().Count <= 0)
			{
				NewPartner();
			}
			DataChanged();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Member_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == "EmployeeNo")
			{
				string code = Function.ToString(e.Value);
				DataRow dataRow = (from t in btn_EmployeeNo.DataSource.AsEnumerable()
					where Function.ToString(t["empID"]) == code
					select t).FirstOrDefault();
				if (dataRow != null)
				{
					gv_Member.SetRowCellValue(e.RowHandle, "EmployeeName", Function.ToString(dataRow["lastName"]) + " " + Function.ToString(dataRow["firstName"]));
					gv_Member.SetRowCellValue(e.RowHandle, "Branch", Function.ParseInt(dataRow["branch"]));
					gv_Member.SetRowCellValue(e.RowHandle, "Department", Function.ParseInt(dataRow["dept"]));
					gv_Member.SetRowCellValue(e.RowHandle, "Email", Function.ToString(dataRow["email"]));
					gv_Member.SetRowCellValue(e.RowHandle, "MobileNumber", Function.ToString(dataRow["mobile"]));
				}
			}
			_dtmember = grd_Member.DataSourceTable;
			if ((from t in _dtmember.AsEnumerable()
				where string.IsNullOrWhiteSpace(Function.ToString(t["EmployeeNo"]))
				select t).ToList().Count <= 0)
			{
				NewMember();
			}
			DataChanged();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		if (e.Column.FieldName == "TotalPlRevenue" || e.Column.FieldName == "Opportunities" || e.Column.FieldName == "TotalQuotation" || e.Column.FieldName == "QuotationValue" || e.Column.FieldName == "TotalOrder" || e.Column.FieldName == "OrderValue")
		{
			e.Appearance.BackColor = GridColor.GridReadOnly;
		}
	}

	private void gv_Item_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		if (e.Column.FieldName == "ItemGroup" || e.Column.FieldName == "UoM")
		{
			e.Appearance.BackColor = GridColor.GridReadOnly;
		}
	}

	private void gv_Cost_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		if (e.Column.FieldName == "ItemGroup" || e.Column.FieldName == "UoM")
		{
			e.Appearance.BackColor = GridColor.GridReadOnly;
		}
	}

	private void addSubToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			FormMode = Enums.FormViewModeType.Add;
			int num = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("DocEntry"));
			grd_Doc.DataSourceTable.Rows.Clear();
			grd_Bp.DataSourceTable().Rows.Clear();
			grd_Item.DatasourceTable().Rows.Clear();
			grd_Partner.DatasourceTable().Rows.Clear();
			grd_Sub.DatasourceTable().Rows.Clear();
			grd_Cost.DataSourceTable.Rows.Clear();
			grd_Member.DataSourceTable().Rows.Clear();
			ucAttachment1.grd_File.DataSourceTable.Clear();
			_dtDoc = grd_Doc.DataSourceTable;
			if (_dtDoc == null || _dtDoc.Columns.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.InfoCannotLoadData, "");
			}
			DataRow dataRow = _dtDoc.NewRow();
			dataRow["StartDate"] = DateTime.Now.Date;
			dataRow["Status"] = "O";
			dataRow["ParentCampaign"] = num;
			_dtDoc.Rows.Add(dataRow);
			grd_Doc.DataSource = _dtDoc;
			NewBp();
			NewMember();
			NewCost();
			NewItem();
			NewPartner();
			_dtAttactment = ucAttachment1.grd_File.DataSourceTable;
			if (_dtAttactment == null || _dtAttactment.Columns.Count == 0)
			{
				XtraMessageBoxEx.Show(Enums.MessageCode.InfoCannotLoadData, Enums.MessageCode.SystemMessage, MessageBoxButtons.OK);
			}
			ucAttachment1.DataSource = _dtAttactment;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ucAttachment1_ButtonBrowse_Click(object sender, EventArgs e)
	{
		DataChanged();
	}

	private void ucAttachment1_ButtonDelete_Click(object sender, EventArgs e)
	{
		DataChanged();
	}

	private void gv_Bp_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == "Territory" && Function.ParseInt(gv_Bp.GetRowCellValue(e.ListSourceRowIndex, "Territory")) == 0)
			{
				e.DisplayText = "";
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_DocNum_Pressed(object sender, EventArgs e)
	{
		try
		{
			string FindCardCode = Function.ToString(gv_Bp.GetFocusedRowCellValue("BpCode"));
			if (string.IsNullOrWhiteSpace(FindCardCode))
			{
				WinCommons.Error(Enums.MessageCode.BpCodeCannotBeEmpty, "Business Partner code cannot be empty");
				return;
			}
			BeginInvoke((Action)(() =>
			{
				ShowCFL(FindCardCode);
			}));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task MOVE_FIRST()
	{
		WinCommons._OpenProcessing();
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			if (await FindNextLastFrist(Enums.NavigateType.First) == 0)
			{
				FormMode = Enums.FormViewModeType.View;
				WinCommons._CloseProcessing();
				return;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_NEXT()
	{
		WinCommons._OpenProcessing();
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			if (await FindNextLastFrist(Enums.NavigateType.Next) == 0)
			{
				FormMode = Enums.FormViewModeType.View;
				WinCommons._CloseProcessing();
				return;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_LAST()
	{
		WinCommons._OpenProcessing();
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			if (await FindNextLastFrist(Enums.NavigateType.Last) == 0)
			{
				FormMode = Enums.FormViewModeType.View;
				WinCommons._CloseProcessing();
				return;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_PREVIOUS()
	{
		WinCommons._OpenProcessing();
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			if (await FindNextLastFrist(Enums.NavigateType.Previous) == 0)
			{
				FormMode = Enums.FormViewModeType.View;
				WinCommons._CloseProcessing();
				return;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	public async Task<int> FindNextLastFrist(Enums.NavigateType type)
	{
		_ = 1;
		try
		{
			DataTable dataTable = await _httpClient.MarketingCampaign.FindNextLastFrist(Function.ParseInt(txt_Id.EditValue), (int)type);
			if (dataTable.IsNotNull())
			{
				grd_Doc.DataSource = dataTable;
				grd_Doc.DatasourceTable().AcceptChanges();
				int num = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("DocEntry"));
				txt_Id.EditValue = num;
				await LoadLineById(num);
				FormMode = Enums.FormViewModeType.View;
				return 1;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		return 0;
	}

	private async Task<DataTable> LoadData(int id, string code, string startFrom, string startTo, string status)
	{
		try
		{
			_isLoading = true;
			DataTable dataTable = await _httpClient.MarketingCampaign.LoadData(id, code.Trim(), startFrom, startTo, status);
			if (dataTable != null)
			{
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					int num = Function.ParseInt(dataTable.Rows[i]["ParentCampaign"]);
					dataTable.Rows[i]["ParentCampaign"] = ((num > 0) ? dataTable.Rows[i]["ParentCampaign"] : DBNull.Value);
				}
				_isLoading = false;
			}
			return dataTable;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	private async Task LoadLineById(int id)
	{
		_ = 2;
		try
		{
			grd_Bp.DataSourceTable.Rows.Clear();
			grd_Item.DataSourceTable.Rows.Clear();
			grd_Partner.DataSourceTable.Rows.Clear();
			grd_Sub.DataSourceTable.Rows.Clear();
			grd_Cost.DataSourceTable.Rows.Clear();
			grd_Member.DataSourceTable.Rows.Clear();
			DataSet dataSet = await _httpClient.MarketingCampaign.GetDataById(id);
			if (dataSet != null)
			{
				grd_Bp.DataSource = dataSet.Tables[0];
				SetSourceLukDocType();
				grd_Item.DataSource = dataSet.Tables[1];
				grd_Partner.DataSource = dataSet.Tables[2];
				grd_Member.DataSource = dataSet.Tables[3];
				grd_Cost.DataSource = dataSet.Tables[4];
				grd_Sub.DataSource = dataSet.Tables[5];
				grd_LineDocument.DataSource = dataSet.Tables[6];
				int absEntry = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("AtcEntry"));
				await ucAttachment1.LoadAttachments(absEntry);
			}
			await ReloadTaskList();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		_isLoading = false;
	}

	private void DataChanged()
	{
		try
		{
			if (!_isLoading && FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task REFRESH()
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View || (FormMode != Enums.FormViewModeType.Update && FormMode != Enums.FormViewModeType.Add) || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.No)
			{
				FormMode = Enums.FormViewModeType.Add;
				grd_Doc.DataSourceTable.Rows.Clear();
				grd_Bp.DataSourceTable().Rows.Clear();
				grd_Item.DatasourceTable().Rows.Clear();
				grd_Partner.DatasourceTable().Rows.Clear();
				grd_Sub.DatasourceTable().Rows.Clear();
				grd_Cost.DataSourceTable.Rows.Clear();
				grd_Member.DataSourceTable().Rows.Clear();
				ucAttachment1.grd_File.DataSourceTable.Clear();
				_dtDoc = grd_Doc.DataSourceTable;
				if (_dtDoc == null || _dtDoc.Columns.Count == 0)
				{
					WinCommons.Error(Enums.MessageCode.InfoCannotLoadData, "");
				}
				DataRow dataRow = _dtDoc.NewRow();
				dataRow["StartDate"] = DateTime.Now.Date;
				dataRow["Status"] = "O";
				_dtDoc.Rows.Add(dataRow);
				grd_Doc.DataSource = _dtDoc;
				NewBp();
				NewMember();
				NewCost();
				NewItem();
				NewPartner();
				_dtAttactment = ucAttachment1.grd_File.DataSourceTable;
				if (_dtAttactment == null || _dtAttactment.Columns.Count == 0)
				{
					XtraMessageBoxEx.Show(Enums.MessageCode.InfoCannotLoadData, Enums.MessageCode.SystemMessage, MessageBoxButtons.OK);
				}
				ucAttachment1.DataSource = _dtAttactment;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void NewBp()
	{
		try
		{
			_dtLine = grd_Bp.DataSourceTable();
			if (_dtLine == null || _dtLine.Columns.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.InfoCannotLoadData, "");
				return;
			}
			DataRow dataRow = _dtLine.NewRow();
			dataRow["DocEntry"] = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("DocEntry"));
			int num = Function.ParseInt(_dtLine.Compute("MAX(LineNum)", ""));
			dataRow["LineNum"] = num + 1;
			dataRow["Status"] = "O";
			_dtLine.Rows.Add(dataRow);
			_dtLine.AcceptChanges();
			DataChanged();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void NewMember()
	{
		try
		{
			_dtmember = grd_Member.DataSourceTable();
			if (_dtmember == null || _dtmember.Columns.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.InfoCannotLoadData, "");
				return;
			}
			DataRow dataRow = _dtmember.NewRow();
			dataRow["DocEntry"] = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("DocEntry"));
			int num = Function.ParseInt(_dtmember.Compute("MAX(LineNum)", ""));
			dataRow["LineNum"] = num + 1;
			_dtmember.Rows.Add(dataRow);
			_dtmember.AcceptChanges();
			DataChanged();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void NewItem()
	{
		try
		{
			_dtItem = grd_Item.DataSourceTable();
			if (_dtItem == null || _dtItem.Columns.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.InfoCannotLoadData, "");
				return;
			}
			DataRow dataRow = _dtItem.NewRow();
			dataRow["DocEntry"] = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("DocEntry"));
			int num = Function.ParseInt(_dtItem.Compute("MAX(LineNum)", ""));
			dataRow["LineNum"] = num + 1;
			_dtItem.Rows.Add(dataRow);
			_dtItem.AcceptChanges();
			DataChanged();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void NewPartner()
	{
		try
		{
			_dtPartner = grd_Partner.DataSourceTable();
			if (_dtPartner == null || _dtPartner.Columns.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.InfoCannotLoadData, "");
				return;
			}
			DataRow dataRow = _dtPartner.NewRow();
			dataRow["DocEntry"] = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("DocEntry"));
			int num = Function.ParseInt(_dtPartner.Compute("MAX(LineNum)", ""));
			dataRow["LineNum"] = num + 1;
			_dtPartner.Rows.Add(dataRow);
			_dtPartner.AcceptChanges();
			DataChanged();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void NewCost()
	{
		try
		{
			_dtCost = grd_Cost.DataSourceTable();
			if (_dtCost == null || _dtCost.Columns.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.InfoCannotLoadData, "");
				return;
			}
			DataRow dataRow = _dtCost.NewRow();
			dataRow["DocEntry"] = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("DocEntry"));
			int num = Function.ParseInt(_dtCost.Compute("MAX(LineNum)", ""));
			dataRow["LineNum"] = num + 1;
			_dtCost.Rows.Add(dataRow);
			_dtCost.AcceptChanges();
			DataChanged();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			btn_Search_Click(null, null);
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<int> Save(int type)
	{
		_ = 1;
		try
		{
			_dtLine = grd_Bp.DataSourceTable().Copy();
			_dtLine.Copy();
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				WinCommons._OpenProcessing();
				DataSet dataSet = new DataSet();
				DataTable dataTable = grd_Doc.DatasourceTable().Copy();
				dataTable.AcceptChanges();
				if (type == 2)
				{
					DataTable dataTable2 = dataTable.Copy();
					dataTable2.AcceptChanges();
					DataRow[] array = dataTable2.Select("DocEntry=" + Function.ParseInt(gv_Doc.GetFocusedRowCellValue("DocEntry")));
					if (array.Length != 0)
					{
						_dtDoc = array.CopyToDataTable();
					}
				}
				else
				{
					_dtDoc = dataTable.Copy();
				}
				if (Function.ToString(_dtDoc.Rows[0]["ParentCampaign"]) == "")
				{
					_dtDoc.Rows[0]["ParentCampaign"] = 0;
				}
				(from DataRow t in _dtLine.Rows
					where string.IsNullOrEmpty(Function.ToString(t["BpCode"]))
					select t).ToList().ForEach((DataRow r) =>
				{
					r.Delete();
				});
				_dtLine.AcceptChanges();
				if (ucAttachment1.DataSource != null)
				{
					_dtAttactment = ucAttachment1.UploadFileNoSAP();
					if (!_dtAttactment.IsNotNull() && ucAttachment1.DataSource.IsNotNull())
					{
						return 1;
					}
				}
				_dtItem = grd_Item.DataSourceTable().Copy();
				(from DataRow t in _dtItem.Rows
					where string.IsNullOrEmpty(Function.ToString(t["ItemCode"]))
					select t).ToList().ForEach((DataRow r) =>
				{
					r.Delete();
				});
				_dtItem.AcceptChanges();
				_dtPartner = grd_Partner.DataSourceTable().Copy();
				(from DataRow t in _dtPartner.Rows
					where string.IsNullOrEmpty(Function.ToString(t["ParterId"]))
					select t).ToList().ForEach((DataRow r) =>
				{
					r.Delete();
				});
				_dtPartner.AcceptChanges();
				_dtmember = grd_Member.DataSourceTable().Copy();
				(from DataRow t in _dtmember.Rows
					where string.IsNullOrEmpty(Function.ToString(t["EmployeeNo"]))
					select t).ToList().ForEach((DataRow r) =>
				{
					r.Delete();
				});
				_dtmember.AcceptChanges();
				_dtCost = grd_Cost.DataSourceTable().Copy();
				(from DataRow t in _dtCost.Rows
					where string.IsNullOrEmpty(Function.ToString(t["ItemCode"]))
					select t).ToList().ForEach((DataRow r) =>
				{
					r.Delete();
				});
				_dtCost.AcceptChanges();
				_dtDoc.TableName = "dtDoc";
				_dtLine.TableName = "dtLine";
				_dtItem.TableName = "dtItem";
				_dtPartner.TableName = "dtPartner";
				_dtmember.TableName = "dtMember";
				_dtCost.TableName = "dtCost";
				_dtAttactment.TableName = "Attactment";
				dataSet.Tables.Add(_dtDoc.Copy());
				dataSet.Tables.Add(_dtLine.Copy());
				dataSet.Tables.Add(_dtItem.Copy());
				dataSet.Tables.Add(_dtPartner.Copy());
				dataSet.Tables.Add(_dtmember.Copy());
				dataSet.Tables.Add(_dtCost.Copy());
				dataSet.Tables.Add(_dtAttactment.Copy());
				HttpResult httpResult = ((type != 2) ? (await _httpClient.MarketingCampaign.Add(dataSet)) : (await _httpClient.MarketingCampaign.Update(dataSet)));
				if (string.IsNullOrEmpty(httpResult.Message))
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
					_isLoading = true;
					FormMode = Enums.FormViewModeType.View;
					if (type == 2)
					{
						grd_Doc.DatasourceTable().AcceptChanges();
						grd_Bp.DatasourceTable().AcceptChanges();
						grd_Item.DatasourceTable().AcceptChanges();
						grd_Cost.DatasourceTable().AcceptChanges();
						grd_Partner.DatasourceTable().AcceptChanges();
						grd_Member.DatasourceTable().AcceptChanges();
					}
					else
					{
						btn_Search_Click(null, null);
					}
					_isLoading = false;
					return 0;
				}
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				return 1;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
			return 1;
		}
		WinCommons._CloseProcessing();
		return 1;
	}

	private void LoadRepostoryCFLDataSource()
	{
		try
		{
			btn_BpCustomer = new RepositoryItemButtonEditCFL();
			btn_BpCustomer.ValueMember = "CardCode";
			btn_BpCustomer.FindCFLBy(Enums.TypeCFL.CustomerLead, true, "", Enums.ItemObjectType.NoItemType, null);
			btn_BpCustomer.HandleChooseItemMulti = ChooseBpReceiptMulti;
			btn_BpCustomer.RefreshDataSource();
			btn_BpCustomer.TypeDrillDown = ApzObjectTypes.oBusinessPartners;
			btn_BpVender = new RepositoryItemButtonEditCFL();
			btn_BpVender.ValueMember = "CardCode";
			btn_BpVender.FindCFLBy(Enums.TypeCFL.Vendor, true, "", Enums.ItemObjectType.NoItemType, null);
			btn_BpVender.HandleChooseItemMulti = ChooseBpReceiptMulti;
			btn_BpVender.RefreshDataSource();
			btn_BpVender.TypeDrillDown = ApzObjectTypes.oBusinessPartners;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void LoadTargetGroup()
	{
		try
		{
			DataTable dataTable = await _httpClient.MarketingCampaign.LoadTargetGroup();
			if (dataTable == null || dataTable.Columns.Count <= 0)
			{
				dataTable = new DataTable();
				dataTable.Columns.Add("TargetCode", typeof(string));
				dataTable.Columns.Add("TargetName", typeof(string));
			}
			DataRow dataRow = dataTable.NewRow();
			dataRow["TargetCode"] = "";
			dataRow["TargetName"] = "";
			dataTable.Rows.InsertAt(dataRow, 0);
			dataTable.AcceptChanges();
			luk_TargetGroup.DataSource = null;
			luk_TargetGroup.DataSource = dataTable.Copy();
			luk_TargetGroupSub.DataSource = null;
			luk_TargetGroupSub.DataSource = dataTable.Copy();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ChooseItemCodeReceiptMulti(List<DataRow> rows)
	{
		try
		{
			_dtItem = grd_Item.DataSourceTable();
			if (_dtItem == null || _dtItem.Rows.Count <= 0 || rows.Count <= 0)
			{
				return;
			}
			gv_Item.SetFocusedRowCellValue("ItemName", Function.ToString(rows[0]["ItemName"]));
			gv_Item.SetFocusedRowCellValue("ItemCode", Function.ToString(rows[0]["ItemCode"]));
			gv_Item.SetFocusedRowCellValue("ItemGroup", Function.ToString(rows[0]["ItmsGrpCod"]));
			gv_Item.SetFocusedRowCellValue("UoM", Function.ToString(rows[0]["InvntryUom"]));
			for (int i = 1; i < rows.Count; i++)
			{
				DataRow dataRow = (from t in _dtItem.AsEnumerable()
					where Function.ToString(t["ItemCode"]) == ""
					select t).FirstOrDefault();
				if (dataRow != null)
				{
					dataRow["ItemCode"] = Function.ToString(rows[i]["ItemCode"]);
					dataRow["ItemName"] = Function.ToString(rows[i]["ItemName"]);
					dataRow["ItemGroup"] = Function.ParseInt(rows[i]["ItmsGrpCod"]);
					dataRow["UoM"] = Function.ToString(rows[i]["InvntryUom"]);
				}
				NewItem();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ChooseItemReceiptMulti(List<DataRow> rows)
	{
		try
		{
			_dtCost = grd_Cost.DataSourceTable();
			if (_dtCost == null || _dtCost.Rows.Count <= 0 || rows.Count <= 0)
			{
				return;
			}
			gv_Cost.SetFocusedRowCellValue("ItemName", Function.ToString(rows[0]["ItemName"]));
			gv_Cost.SetFocusedRowCellValue("ItemCode", Function.ToString(rows[0]["ItemCode"]));
			gv_Cost.SetFocusedRowCellValue("ItemGroup", Function.ParseInt(rows[0]["ItmsGrpCod"]));
			for (int i = 1; i < rows.Count; i++)
			{
				DataRow dataRow = (from t in _dtCost.AsEnumerable()
					where Function.ToString(t["ItemCode"]) == ""
					select t).FirstOrDefault();
				if (dataRow != null)
				{
					dataRow["ItemCode"] = Function.ToString(rows[i]["ItemCode"]);
					dataRow["ItemName"] = Function.ToString(rows[i]["ItemName"]);
					dataRow["ItemGroup"] = Function.ParseInt(rows[i]["ItmsGrpCod"]);
				}
				NewCost();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ChooseEmployeeNoReceiptMulti(List<DataRow> rows)
	{
		try
		{
			_dtmember = grd_Member.DataSourceTable();
			if (_dtmember == null || _dtmember.Rows.Count <= 0 || rows.Count <= 0)
			{
				return;
			}
			gv_Member.SetFocusedRowCellValue("EmployeeNo", Function.ToString(rows[0]["empID"]));
			gv_Member.SetFocusedRowCellValue("EmployeeName", Function.ToString(rows[0]["lastName"]) + " " + Function.ToString(rows[0]["firstName"]));
			gv_Member.SetFocusedRowCellValue("Branch", Function.ParseInt(rows[0]["branch"]));
			gv_Member.SetFocusedRowCellValue("Department", Function.ParseInt(rows[0]["dept"]));
			gv_Member.SetFocusedRowCellValue("Email", Function.ToString(rows[0]["email"]));
			gv_Member.SetFocusedRowCellValue("MobileNumber", Function.ToString(rows[0]["mobile"]));
			for (int i = 1; i < rows.Count; i++)
			{
				DataRow dataRow = (from t in _dtmember.AsEnumerable()
					where Function.ToString(t["EmployeeNo"]) == ""
					select t).FirstOrDefault();
				if (dataRow != null)
				{
					dataRow["EmployeeNo"] = Function.ToString(rows[i]["empID"]);
					dataRow["EmployeeName"] = Function.ToString(rows[i]["lastName"]) + " " + Function.ToString(rows[i]["firstName"]);
					dataRow["Branch"] = Function.ParseInt(rows[i]["branch"]);
					dataRow["Department"] = Function.ParseInt(rows[i]["dept"]);
					dataRow["Email"] = Function.ToString(rows[i]["email"]);
					dataRow["MobileNumber"] = Function.ToString(rows[i]["mobile"]);
				}
				NewMember();
			}
			grd_Member.DataSourceTable.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void ChooseBpReceiptMulti(List<DataRow> rows)
	{
		_ = 1;
		try
		{
			_dtLine = grd_Bp.DataSourceTable();
			if (_dtLine == null || _dtLine.Rows.Count <= 0 || rows.Count <= 0)
			{
				return;
			}
			if (!string.IsNullOrEmpty(Function.ToString(rows[0]["CardCode"])))
			{
				gv_Bp.SetFocusedRowCellValue("BpCode", Function.ToString(rows[0]["CardCode"]));
				gv_Bp.SetFocusedRowCellValue("BpName", Function.ToString(rows[0]["CardName"]));
				DataTable dataTable = await _httpClient.BusinessMaster.GetADoc(Function.ToString(rows[0]["CardCode"]));
				if (dataTable != null && dataTable.Rows.Count > 0)
				{
					if (Function.ParseInt(dataTable.Rows[0]["GroupCode"]) > 0)
					{
						gv_Bp.SetFocusedRowCellValue("BpGroup", Function.ParseInt(dataTable.Rows[0]["GroupCode"]));
					}
					else
					{
						gv_Bp.SetFocusedRowCellValue("BpGroup", DBNull.Value);
					}
					gv_Bp.SetFocusedRowCellValue("BpAddress", Function.ToString(dataTable.Rows[0]["Address"]));
					if (Function.ParseInt(dataTable.Rows[0]["IndustryC"]) > 0)
					{
						gv_Bp.SetFocusedRowCellValue("Industry", Function.ParseInt(dataTable.Rows[0]["IndustryC"]));
					}
					else
					{
						gv_Bp.SetFocusedRowCellValue("Industry", DBNull.Value);
					}
					if (Function.ParseInt(dataTable.Rows[0]["Territory"]) > 0)
					{
						gv_Bp.SetFocusedRowCellValue("Territory", Function.ParseInt(dataTable.Rows[0]["Territory"]));
					}
					else
					{
						gv_Bp.SetFocusedRowCellValue("Territory", DBNull.Value);
					}
				}
				DataTable dataTable2 = _httpClient.BusinessMaster.ShowContactPerson(Function.ToString(rows[0]["CardCode"]));
				if (dataTable2 != null && dataTable2.Rows.Count > 0)
				{
					string code = Function.ToString(dataTable.Rows[0]["CntctPrsn"]);
					DataRow dataRow = dataTable2.AsEnumerable().FirstOrDefault((DataRow t) => string.Equals(Function.ToString(t["Name"]), code));
					if (dataRow != null)
					{
						gv_Bp.SetFocusedRowCellValue("ContactName", Function.ToString(dataRow["Name"]));
						gv_Bp.SetFocusedRowCellValue("ContactEmail", Function.ToString(dataRow["E_MailL"]));
						gv_Bp.SetFocusedRowCellValue("ContactPhone", Function.ToString(dataRow["Cellolar"]));
						gv_Bp.SetFocusedRowCellValue("ContactPosition", Function.ToString(dataRow["Position"]));
					}
				}
			}
			for (int i = 1; i < rows.Count; i++)
			{
				DataRow rowfocused = (from t in _dtLine.AsEnumerable()
					where Function.ToString(t["BpCode"]) == ""
					select t).FirstOrDefault();
				if (rowfocused != null)
				{
					rowfocused["BpCode"] = Function.ToString(rows[i]["CardCode"]);
					rowfocused["BpName"] = Function.ToString(rows[i]["CardName"]);
					DataTable dataTable3 = await _httpClient.BusinessMaster.GetADoc(Function.ToString(rows[i]["CardCode"]));
					if (dataTable3 != null && dataTable3.Rows.Count > 0)
					{
						if (Function.ParseInt(dataTable3.Rows[0]["GroupCode"]) > 0)
						{
							rowfocused["BpGroup"] = Function.ParseInt(dataTable3.Rows[0]["GroupCode"]);
						}
						else
						{
							rowfocused["BpGroup"] = DBNull.Value;
						}
						rowfocused["BpAddress"] = Function.ToString(dataTable3.Rows[0]["Address"]);
						if (Function.ParseInt(dataTable3.Rows[0]["IndustryC"]) > 0)
						{
							rowfocused["Industry"] = Function.ParseInt(dataTable3.Rows[0]["IndustryC"]);
						}
						else
						{
							rowfocused["Industry"] = DBNull.Value;
						}
						if (Function.ParseInt(dataTable3.Rows[0]["Territory"]) > 0)
						{
							rowfocused["Territory"] = Function.ParseInt(dataTable3.Rows[0]["Territory"]);
						}
						else
						{
							rowfocused["Territory"] = DBNull.Value;
						}
					}
					DataTable dataTable4 = _httpClient.BusinessMaster.ShowContactPerson(Function.ToString(rows[i]["CardCode"]));
					if (dataTable4 != null && dataTable4.Rows.Count > 0)
					{
						string code2 = Function.ToString(dataTable3.Rows[0]["CntctPrsn"]);
						DataRow dataRow2 = dataTable4.AsEnumerable().FirstOrDefault((DataRow t) => string.Equals(Function.ToString(t["Name"]), code2));
						if (dataRow2 != null)
						{
							rowfocused["ContactName"] = Function.ToString(dataRow2["Name"]);
							rowfocused["ContactEmail"] = Function.ToString(dataRow2["E_MailL"]);
							rowfocused["ContactPhone"] = Function.ToString(dataRow2["Cellolar"]);
							rowfocused["ContactPosition"] = Function.ToString(dataRow2["Position"]);
						}
					}
				}
				_dtLine = grd_Bp.DataSourceTable;
				if ((from t in _dtLine.AsEnumerable()
					where string.IsNullOrWhiteSpace(Function.ToString(t["BpCode"]))
					select t).ToList().Count <= 0)
				{
					NewBp();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ShowCFL(string CardCode = "")
	{
		frm_List_Document frm_List_Document2 = new frm_List_Document((ApzObjectTypes)Function.ParseInt(gv_Bp.GetFocusedRowCellValue("DocType")), multiSelect: false, CardCode, isNoStatus: true);
		frm_List_Document2.ShowDialog(FindForm());
		if (frm_List_Document2.ChooseItem != null)
		{
			DocumentsImpl chooseItem = frm_List_Document2.ChooseItem;
			gv_Bp.SetRowCellValue(gv_Bp.FocusedRowHandle, "DocNumber", chooseItem.DocNum);
		}
	}

	private void SetSourceLukDocType()
	{
		if (!string.IsNullOrWhiteSpace(Function.ToString(gv_Doc.GetFocusedRowCellValue("TargetType"))))
		{
			if (gv_Doc.GetFocusedRowCellValue("TargetType").Equals("C"))
			{
				luk_DocType.SetDataSource(new Dictionary<string, string>
				{
					{
						Function.ToString(97),
						"Sales Opportunities"
					},
					{
						Function.ToString(23),
						"Sales Quotations"
					},
					{
						Function.ToString(17),
						"Sales Orders"
					},
					{
						Function.ToString(15),
						"Deliveries"
					},
					{
						Function.ToString(13),
						"A/R Invoices"
					},
					{
						Function.ToString(1250000025),
						"Sales Blanket"
					}
				});
			}
			else
			{
				luk_DocType.SetDataSource(new Dictionary<string, string>
				{
					{
						Function.ToString(540000006),
						"Purchase Quotations"
					},
					{
						Function.ToString(22),
						"Purchase Orders"
					},
					{
						Function.ToString(20),
						"Good Receipt POs"
					},
					{
						Function.ToString(18),
						"A/P Invoices"
					},
					{
						Function.ToString(1250000026),
						"Purchase Blanket"
					}
				});
			}
		}
	}

	private bool CheckDataTargetBP(DataTable dt)
	{
		return dt.AsEnumerable().Any((DataRow r) => !string.IsNullOrEmpty(Function.ToString(r["BpCode"])));
	}

	private void gv_Doc_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
	{
		try
		{
			if (sender is GridViewEx gridViewEx && gridViewEx.FocusedColumn.FieldName == "EndDate" && Function.ParseDateTime(gv_Doc.GetFocusedRowCellValue("StartDate")) > Function.ParseDateTime(e.Value))
			{
				e.Valid = false;
				e.ErrorText = "";
				WinCommons.Error(Enums.MessageCode.StartDateShouldNotGreaterThanEndDate, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_InvalidValueException(object sender, InvalidValueExceptionEventArgs e)
	{
		try
		{
			if (sender is GridViewEx)
			{
				e.ExceptionMode = ExceptionMode.DisplayError;
				e.WindowCaption = "Input Error";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void frm_MarketingCampaign_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if ((FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
			{
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
			WinCommons.Error(Enums.MessageCode.None, ex.Message);
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Service.MarketingCampaign.frm_MarketingCampaign));
		DevExpress.Utils.SerializableAppearanceObject appearance2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions4 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions4 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions5 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions6 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions5 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance11 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered11 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed11 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled11 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions7 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance12 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered12 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed12 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled12 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions6 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance13 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered13 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed13 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled13 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions8 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance14 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered14 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed14 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled14 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions7 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance15 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered15 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed15 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled15 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions9 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance16 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered16 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed16 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled16 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions8 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance17 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered17 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed17 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled17 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions10 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance18 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered18 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed18 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled18 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions9 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance19 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered19 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed19 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled19 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions11 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance20 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered20 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed20 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled20 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions10 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance21 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered21 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed21 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled21 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraGrid.GridFormatRule gridFormatRule = new DevExpress.XtraGrid.GridFormatRule();
		DevExpress.XtraEditors.FormatConditionRuleIconSet formatConditionRuleIconSet = new DevExpress.XtraEditors.FormatConditionRuleIconSet();
		DevExpress.XtraEditors.FormatConditionIconSet formatConditionIconSet = new DevExpress.XtraEditors.FormatConditionIconSet();
		DevExpress.XtraEditors.FormatConditionIconSetIcon formatConditionIconSetIcon = new DevExpress.XtraEditors.FormatConditionIconSetIcon();
		DevExpress.XtraEditors.FormatConditionIconSetIcon formatConditionIconSetIcon2 = new DevExpress.XtraEditors.FormatConditionIconSetIcon();
		DevExpress.XtraEditors.FormatConditionIconSetIcon formatConditionIconSetIcon3 = new DevExpress.XtraEditors.FormatConditionIconSetIcon();
		DevExpress.XtraEditors.FormatConditionIconSetIcon formatConditionIconSetIcon4 = new DevExpress.XtraEditors.FormatConditionIconSetIcon();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions11 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance22 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered22 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed22 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled22 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions12 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance23 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered23 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed23 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled23 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions12 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance24 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered24 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed24 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled24 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions13 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance25 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered25 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed25 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled25 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions13 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance26 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered26 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed26 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled26 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions14 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance27 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered27 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed27 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled27 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions14 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance28 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered28 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed28 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled28 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions15 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance29 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered29 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed29 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled29 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions15 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance30 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered30 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed30 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled30 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions16 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance31 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered31 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed31 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled31 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions17 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance32 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered32 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed32 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled32 = new DevExpress.Utils.SerializableAppearanceObject();
		this.gc_Priority_Completed = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEditEx9 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx10 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx11 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx8 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemTextEditExPrice4 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.repositoryItemTextEditExPrice5 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.repositoryItemCalcEditEx1 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx2 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx3 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx4 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemTextEditExPrice1 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.repositoryItemCalcEditEx5 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx6 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemTextEditExPrice2 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.repositoryItemTextEditExPrice3 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.repositoryItemCalcEditEx7 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.ctm_Doc = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addNewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.addSubToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_TargetType = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.gridColumnEx3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx5 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_CampaignType = new Apzon.Forms.UserControls.RepositoryItemLookUpEditDefineNew();
		this.gridColumnEx6 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_OwnerPer = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx7 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_StatusDoc = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.gridColumnEx8 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_TargetGroup = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx9 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx10 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx11 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_ParentCampaign = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx12 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_Stage = new Apzon.Forms.UserControls.RepositoryItemLookUpEditDefineNew();
		this.gridColumnEx13 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx14 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx15 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx16 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx17 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx18 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx19 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx61 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_ProjectId = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx100 = new Apzon.Controls.Grid.GridColumnEx();
		this.xtraTabControlEx1 = new Apzon.Controls.Controls.XtraTabControlEx();
		this.xtra_Bp = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_Bp = new Apzon.Controls.Grid.GridControlEx();
		this.ctm_Bp = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addRow_Bp = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteRow_Bp = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Bp = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx20 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx21 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx22 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_BpCode = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx23 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx24 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_BpGroup = new Apzon.Forms.UserControls.RepositoryItemLookUpEditDefineNew();
		this.gridColumnEx25 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx26 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_Territory = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx27 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx28 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx29 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx30 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx31 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_Qualified = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.gridColumnEx32 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx33 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_StatusBp = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.gridColumnEx99 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_Industry = new Apzon.Forms.UserControls.RepositoryItemLookUpEditDefineNew();
		this.gridColumnEx102 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_DocType = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.gridColumnEx103 = new Apzon.Controls.Grid.GridColumnEx();
		this.cke_ShowBPDoc = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gridColumnEx104 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_DocNum = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.btn_BpCustomer = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.btn_BpVender = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.xtra_Item = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_Item = new Apzon.Controls.Grid.GridControlEx();
		this.ctm_Item = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addRow_Item = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteRow_Item = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Item = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx62 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx63 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx64 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_ItemCode = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx65 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx66 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_ItemGroup = new Apzon.Forms.UserControls.RepositoryItemLookUpEditDefineNew();
		this.gridColumnEx67 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx68 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx69 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx101 = new Apzon.Controls.Grid.GridColumnEx();
		this.xtra_Partner = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_Partner = new Apzon.Controls.Grid.GridControlEx();
		this.ctm_Partner = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addRow_Partner = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteRow_Partner = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Partner = new Apzon.Controls.Grid.GridViewEx();
		this.gcPartnerName = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_PartnerId = new Apzon.Forms.UserControls.RepositoryItemLookUpEditDefineNew();
		this.gcPartnerRelationShip = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_Orl = new Apzon.Forms.UserControls.RepositoryItemLookUpEditDefineNew();
		this.gcPartnerRelatedBP = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_RelaPartner = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gcPartnerRemark = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDocEntry = new Apzon.Controls.Grid.GridColumnEx();
		this.gcPartnerLine = new Apzon.Controls.Grid.GridColumnEx();
		this.xtra_Member = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_Member = new Apzon.Controls.Grid.GridControlEx();
		this.ctm_Member = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addRow_member = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteRow_member = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Member = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx34 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx35 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx36 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_EmployeeNo = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx37 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx38 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_BranchMember = new Apzon.Forms.UserControls.RepositoryItemLookUpEditDefineNew();
		this.gridColumnEx39 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_DepartmentMember = new Apzon.Forms.UserControls.RepositoryItemLookUpEditDefineNew();
		this.gridColumnEx40 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx41 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx42 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_Role = new Apzon.Forms.UserControls.RepositoryItemLookUpEditDefineNew();
		this.gridColumnEx43 = new Apzon.Controls.Grid.GridColumnEx();
		this.xtra_Atta = new Apzon.Controls.Controls.XtraTabPageEx();
		this.ucAttachment1 = new Apzon.Forms.UserControls.ucAttachment();
		this.xtra_Cost = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_Cost = new Apzon.Controls.Grid.GridControlEx();
		this.ctm_Cost = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addRow_Cost = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteRow_Cost = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Cost = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx44 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx45 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx46 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_ItemCost = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx47 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx48 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx49 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx50 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx51 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx52 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx53 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx54 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx55 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx90 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_ItmGrpCost = new Apzon.Forms.UserControls.RepositoryItemLookUpEditDefineNew();
		this.xtra_Task = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_ListCompleted = new Apzon.Controls.Grid.GridControlEx();
		this.ctm_Task = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.newTaskToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_ListCompleted = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx56 = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Closed_Completed = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_bte_Closed = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gridColumnEx57 = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_DueDate_Completed = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_ProjectEntry_Completed = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_ProjectWBS_Completed = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx58 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx59 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx60 = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_AssignTo_Completed = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_CloseDate_Completed = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_bte_Open = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.xtra_Sub = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_Sub = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Sub = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx70 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_DocEntrySub = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx71 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_TargetTypeSub = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.gridColumnEx72 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx73 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx74 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_TypeSub = new Apzon.Forms.UserControls.RepositoryItemLookUpEditDefineNew();
		this.gridColumnEx75 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_EmployeeSub = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx76 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_StatusSub = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.gridColumnEx77 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_TargetGroupSub = new Apzon.Forms.UserControls.RepositoryItemLookUpEditDefineNew();
		this.gridColumnEx78 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx79 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx80 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_ParentSub = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx81 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_StageSub = new Apzon.Forms.UserControls.RepositoryItemLookUpEditDefineNew();
		this.gridColumnEx82 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx83 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx84 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx85 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx86 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx87 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx88 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx89 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_ProjectSub = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.xtra_Document = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_LineDocument = new Apzon.Controls.Grid.GridControlEx();
		this.gv_LineDocument = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx91 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx92 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx93 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx94 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_DocNumDo = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx95 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx96 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx97 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx98 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_CurrencyLineDu = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.txt_No = new Apzon.Controls.Editor.TextEditEx();
		this.luk_StatusSearch = new Apzon.Controls.Editor.MyLookUpEdit.LookUpEditSimple();
		this.date_FromDate = new Apzon.Controls.Editor.DateEditEx();
		this.date_ToDate = new Apzon.Controls.Editor.DateEditEx();
		this.btn_Search = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Ok = new DevExpress.XtraEditors.SimpleButton();
		this.btn_Cancel = new DevExpress.XtraEditors.SimpleButton();
		this.txt_Id = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine5 = new Apzon.Controls.Controls.LabelLine();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx9).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx10).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx11).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx8).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx6).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx7).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		this.ctm_Doc.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_TargetType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_CampaignType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_OwnerPer).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_StatusDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_TargetGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_ParentCampaign).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Stage).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_ProjectId).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControlEx1).BeginInit();
		this.xtraTabControlEx1.SuspendLayout();
		this.xtra_Bp.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Bp).BeginInit();
		this.ctm_Bp.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Bp).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_BpCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_BpGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_Territory).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Qualified).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_StatusBp).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Industry).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_DocType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cke_ShowBPDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_DocNum).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_BpCustomer).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_BpVender).BeginInit();
		this.xtra_Item.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Item).BeginInit();
		this.ctm_Item.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Item).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_ItemCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_ItemGroup).BeginInit();
		this.xtra_Partner.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Partner).BeginInit();
		this.ctm_Partner.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Partner).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_PartnerId).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Orl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_RelaPartner).BeginInit();
		this.xtra_Member.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Member).BeginInit();
		this.ctm_Member.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Member).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_EmployeeNo).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_BranchMember).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_DepartmentMember).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Role).BeginInit();
		this.xtra_Atta.SuspendLayout();
		this.xtra_Cost.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Cost).BeginInit();
		this.ctm_Cost.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Cost).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_ItemCost).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_ItmGrpCost).BeginInit();
		this.xtra_Task.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_ListCompleted).BeginInit();
		this.ctm_Task.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_ListCompleted).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_bte_Closed).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_bte_Open).BeginInit();
		this.xtra_Sub.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Sub).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Sub).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_DocEntrySub).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_TargetTypeSub).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_TypeSub).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_EmployeeSub).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_StatusSub).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_TargetGroupSub).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_ParentSub).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_StageSub).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_ProjectSub).BeginInit();
		this.xtra_Document.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_LineDocument).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_LineDocument).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_DocNumDo).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_CurrencyLineDu).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_No.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_StatusSearch.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_FromDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_FromDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_ToDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_ToDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Id.Properties).BeginInit();
		base.SuspendLayout();
		this.gc_Priority_Completed.Caption = " Priority";
		this.gc_Priority_Completed.CurrencyDateField = null;
		this.gc_Priority_Completed.CurrencyField = null;
		this.gc_Priority_Completed.CurrencyFixed = null;
		this.gc_Priority_Completed.CurrencyFixedRate = 0.0;
		this.gc_Priority_Completed.CurrencyRateField = null;
		this.gc_Priority_Completed.FieldName = "Priority";
		this.gc_Priority_Completed.Name = "gc_Priority_Completed";
		this.gc_Priority_Completed.OptionsColumn.FixedWidth = true;
		this.gc_Priority_Completed.PrimaryKey = false;
		this.gc_Priority_Completed.UnitField = null;
		this.gc_Priority_Completed.Width = 24;
		this.repositoryItemCalcEditEx9.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx9.IsSearchControl = false;
		this.repositoryItemCalcEditEx9.Name = "repositoryItemCalcEditEx9";
		this.repositoryItemCalcEditEx10.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx10.IsSearchControl = false;
		this.repositoryItemCalcEditEx10.Name = "repositoryItemCalcEditEx10";
		this.repositoryItemCalcEditEx11.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx11.IsSearchControl = false;
		this.repositoryItemCalcEditEx11.Name = "repositoryItemCalcEditEx11";
		this.repositoryItemCalcEditEx8.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx8.IsSearchControl = false;
		this.repositoryItemCalcEditEx8.Name = "repositoryItemCalcEditEx8";
		this.repositoryItemTextEditExPrice4.Name = "repositoryItemTextEditExPrice4";
		this.repositoryItemTextEditExPrice5.Name = "repositoryItemTextEditExPrice5";
		this.repositoryItemCalcEditEx1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx1.IsSearchControl = false;
		this.repositoryItemCalcEditEx1.Name = "repositoryItemCalcEditEx1";
		this.repositoryItemCalcEditEx2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx2.IsSearchControl = false;
		this.repositoryItemCalcEditEx2.Name = "repositoryItemCalcEditEx2";
		this.repositoryItemCalcEditEx3.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx3.IsSearchControl = false;
		this.repositoryItemCalcEditEx3.Name = "repositoryItemCalcEditEx3";
		this.repositoryItemCalcEditEx4.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx4.IsSearchControl = false;
		this.repositoryItemCalcEditEx4.Name = "repositoryItemCalcEditEx4";
		this.repositoryItemTextEditExPrice1.Name = "repositoryItemTextEditExPrice1";
		this.repositoryItemCalcEditEx5.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx5.IsSearchControl = false;
		this.repositoryItemCalcEditEx5.Name = "repositoryItemCalcEditEx5";
		this.repositoryItemCalcEditEx6.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx6.IsSearchControl = false;
		this.repositoryItemCalcEditEx6.Name = "repositoryItemCalcEditEx6";
		this.repositoryItemTextEditExPrice2.Name = "repositoryItemTextEditExPrice2";
		this.repositoryItemTextEditExPrice3.Name = "repositoryItemTextEditExPrice3";
		this.repositoryItemCalcEditEx7.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx7.IsSearchControl = false;
		this.repositoryItemCalcEditEx7.Name = "repositoryItemCalcEditEx7";
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(1, 89);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.grd_Doc);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.xtraTabControlEx1);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(973, 402);
		this.splitContainerControl1.SplitterPosition = 150;
		this.splitContainerControl1.TabIndex = 0;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.ContextMenuStrip = this.ctm_Doc;
		this.grd_Doc.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Doc.Location = new System.Drawing.Point(0, 0);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[8] { this.luk_TargetType, this.btn_OwnerPer, this.luk_StatusDoc, this.btn_ProjectId, this.luk_CampaignType, this.luk_Stage, this.btn_ParentCampaign, this.luk_TargetGroup });
		this.grd_Doc.Size = new System.Drawing.Size(973, 150);
		this.grd_Doc.TabIndex = 0;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.ctm_Doc.Items.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.addNewToolStripMenuItem, this.deleteToolStripMenuItem, this.addSubToolStripMenuItem });
		this.ctm_Doc.Name = "ctm_Doc";
		this.ctm_Doc.Size = new System.Drawing.Size(124, 70);
		this.addNewToolStripMenuItem.Name = "addNewToolStripMenuItem";
		this.addNewToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
		this.addNewToolStripMenuItem.Text = "Add New";
		this.addNewToolStripMenuItem.Click += new System.EventHandler(this.addNewToolStripMenuItem_Click);
		this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
		this.deleteToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
		this.deleteToolStripMenuItem.Text = "Remove";
		this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
		this.addSubToolStripMenuItem.Name = "addSubToolStripMenuItem";
		this.addSubToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
		this.addSubToolStripMenuItem.Text = "Add Sub";
		this.addSubToolStripMenuItem.Click += new System.EventHandler(this.addSubToolStripMenuItem_Click);
		this.gv_Doc.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Doc.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = null;
		this.gv_Doc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[21]
		{
			this.gridColumnEx1, this.gridColumnEx2, this.gridColumnEx3, this.gridColumnEx4, this.gridColumnEx5, this.gridColumnEx6, this.gridColumnEx7, this.gridColumnEx8, this.gridColumnEx9, this.gridColumnEx10,
			this.gridColumnEx11, this.gridColumnEx12, this.gridColumnEx13, this.gridColumnEx14, this.gridColumnEx15, this.gridColumnEx16, this.gridColumnEx17, this.gridColumnEx18, this.gridColumnEx19, this.gridColumnEx61,
			this.gridColumnEx100
		});
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = "oMarketingCampaign";
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.IgnoreFilter = false;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 35;
		this.gv_Doc.IsApzTable = false;
		this.gv_Doc.IsInsertNewRow = false;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.IsMappingContextAction = false;
		this.gv_Doc.ItemId = "APZ_OSMC";
		this.gv_Doc.LstColumnAllowBatch = null;
		this.gv_Doc.LstColumnNotAllowBatch = null;
		this.gv_Doc.LstColumnNotEditable = null;
		this.gv_Doc.LstColumNotSetting = null;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Doc.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Doc.OptionsView.ColumnAutoWidth = false;
		this.gv_Doc.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Doc.OptionsView.ShowGroupPanel = false;
		this.gv_Doc.UseBatchMod = false;
		this.gv_Doc.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Doc_RowCellStyle);
		this.gv_Doc.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_Doc_FocusedRowObjectChanged);
		this.gv_Doc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Doc_CellValueChanged);
		this.gv_Doc.BeforeLeaveRow += new DevExpress.XtraGrid.Views.Base.RowAllowEventHandler(this.gv_Doc_BeforeLeaveRow);
		this.gv_Doc.ValidatingEditor += new DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventHandler(this.gv_Doc_ValidatingEditor);
		this.gv_Doc.InvalidValueException += new DevExpress.XtraEditors.Controls.InvalidValueExceptionEventHandler(this.gv_Doc_InvalidValueException);
		this.gridColumnEx1.Caption = "DocEntry";
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "DocEntry";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.OptionsColumn.AllowEdit = false;
		this.gridColumnEx1.OptionsColumn.ReadOnly = true;
		this.gridColumnEx1.PrimaryKey = true;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx2.Caption = "Target Type";
		this.gridColumnEx2.ColumnEdit = this.luk_TargetType;
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.FieldName = "TargetType";
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.UnitField = null;
		this.gridColumnEx2.Visible = true;
		this.gridColumnEx2.VisibleIndex = 0;
		this.luk_TargetType.AutoHeight = false;
		this.luk_TargetType.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_TargetType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_TargetType.IsSearchControl = false;
		this.luk_TargetType.Name = "luk_TargetType";
		this.luk_TargetType.NullText = "";
		this.luk_TargetType.ShowFooter = false;
		this.luk_TargetType.ShowHeader = false;
		this.gridColumnEx3.Caption = "Campaign No";
		this.gridColumnEx3.CurrencyDateField = null;
		this.gridColumnEx3.CurrencyField = null;
		this.gridColumnEx3.CurrencyFixed = null;
		this.gridColumnEx3.CurrencyFixedRate = 0.0;
		this.gridColumnEx3.CurrencyRateField = null;
		this.gridColumnEx3.FieldName = "CampaignNo";
		this.gridColumnEx3.Name = "gridColumnEx3";
		this.gridColumnEx3.PrimaryKey = false;
		this.gridColumnEx3.UnitField = null;
		this.gridColumnEx3.Visible = true;
		this.gridColumnEx3.VisibleIndex = 1;
		this.gridColumnEx4.Caption = "Campaign Name";
		this.gridColumnEx4.CurrencyDateField = null;
		this.gridColumnEx4.CurrencyField = null;
		this.gridColumnEx4.CurrencyFixed = null;
		this.gridColumnEx4.CurrencyFixedRate = 0.0;
		this.gridColumnEx4.CurrencyRateField = null;
		this.gridColumnEx4.FieldName = "CampaignName";
		this.gridColumnEx4.Name = "gridColumnEx4";
		this.gridColumnEx4.PrimaryKey = false;
		this.gridColumnEx4.UnitField = null;
		this.gridColumnEx4.Visible = true;
		this.gridColumnEx4.VisibleIndex = 2;
		this.gridColumnEx4.Width = 96;
		this.gridColumnEx5.Caption = "Campaign Type";
		this.gridColumnEx5.ColumnEdit = this.luk_CampaignType;
		this.gridColumnEx5.CurrencyDateField = null;
		this.gridColumnEx5.CurrencyField = null;
		this.gridColumnEx5.CurrencyFixed = null;
		this.gridColumnEx5.CurrencyFixedRate = 0.0;
		this.gridColumnEx5.CurrencyRateField = null;
		this.gridColumnEx5.FieldName = "CampaignType";
		this.gridColumnEx5.Name = "gridColumnEx5";
		this.gridColumnEx5.PrimaryKey = false;
		this.gridColumnEx5.UnitField = null;
		this.gridColumnEx5.Visible = true;
		this.gridColumnEx5.VisibleIndex = 3;
		this.gridColumnEx5.Width = 102;
		this.luk_CampaignType.AutoHeight = false;
		this.luk_CampaignType.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
		this.luk_CampaignType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_CampaignType.CheckFields = null;
		this.luk_CampaignType.CheckTables = null;
		this.luk_CampaignType.Name = "luk_CampaignType";
		this.luk_CampaignType.NullText = "";
		this.luk_CampaignType.ShowFooter = false;
		this.luk_CampaignType.ShowHeader = false;
		this.gridColumnEx6.Caption = "Owner Person";
		this.gridColumnEx6.ColumnEdit = this.btn_OwnerPer;
		this.gridColumnEx6.CurrencyDateField = null;
		this.gridColumnEx6.CurrencyField = null;
		this.gridColumnEx6.CurrencyFixed = null;
		this.gridColumnEx6.CurrencyFixedRate = 0.0;
		this.gridColumnEx6.CurrencyRateField = null;
		this.gridColumnEx6.FieldName = "EmployeeNo";
		this.gridColumnEx6.Name = "gridColumnEx6";
		this.gridColumnEx6.PrimaryKey = false;
		this.gridColumnEx6.UnitField = null;
		this.gridColumnEx6.Visible = true;
		this.gridColumnEx6.VisibleIndex = 4;
		this.gridColumnEx6.Width = 97;
		this.btn_OwnerPer.AutoHeight = false;
		this.btn_OwnerPer.AutoLoadDataSource = true;
		editorButtonImageOptions.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions34.Image");
		this.btn_OwnerPer.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_OwnerPer.ContainerControl = this;
		this.btn_OwnerPer.DisplayMember = "lastName";
		this.btn_OwnerPer.DisplayMember2 = "firstName";
		this.btn_OwnerPer.LoadDataSourceWhenEnter = false;
		this.btn_OwnerPer.Name = "btn_OwnerPer";
		this.btn_OwnerPer.SeparateCharacterForMulti = ';';
		this.btn_OwnerPer.SeparateString = ", ";
		this.btn_OwnerPer.UsingAutoCompleteBox = false;
		this.btn_OwnerPer.UsingDataSource = true;
		this.btn_OwnerPer.ValidateWhenNoDataSource = false;
		this.btn_OwnerPer.ValueMember = "EmpId";
		this.gridColumnEx7.Caption = "Status";
		this.gridColumnEx7.ColumnEdit = this.luk_StatusDoc;
		this.gridColumnEx7.CurrencyDateField = null;
		this.gridColumnEx7.CurrencyField = null;
		this.gridColumnEx7.CurrencyFixed = null;
		this.gridColumnEx7.CurrencyFixedRate = 0.0;
		this.gridColumnEx7.CurrencyRateField = null;
		this.gridColumnEx7.FieldName = "Status";
		this.gridColumnEx7.Name = "gridColumnEx7";
		this.gridColumnEx7.PrimaryKey = false;
		this.gridColumnEx7.UnitField = null;
		this.gridColumnEx7.Visible = true;
		this.gridColumnEx7.VisibleIndex = 5;
		this.luk_StatusDoc.AutoHeight = false;
		this.luk_StatusDoc.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_StatusDoc.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_StatusDoc.IsSearchControl = false;
		this.luk_StatusDoc.Name = "luk_StatusDoc";
		this.luk_StatusDoc.NullText = "";
		this.luk_StatusDoc.ShowFooter = false;
		this.luk_StatusDoc.ShowHeader = false;
		this.gridColumnEx8.Caption = "Target Group";
		this.gridColumnEx8.ColumnEdit = this.luk_TargetGroup;
		this.gridColumnEx8.CurrencyDateField = null;
		this.gridColumnEx8.CurrencyField = null;
		this.gridColumnEx8.CurrencyFixed = null;
		this.gridColumnEx8.CurrencyFixedRate = 0.0;
		this.gridColumnEx8.CurrencyRateField = null;
		this.gridColumnEx8.FieldName = "TargetGroup";
		this.gridColumnEx8.Name = "gridColumnEx8";
		this.gridColumnEx8.PrimaryKey = false;
		this.gridColumnEx8.UnitField = null;
		this.gridColumnEx8.Visible = true;
		this.gridColumnEx8.VisibleIndex = 6;
		this.luk_TargetGroup.AutoHeight = false;
		this.luk_TargetGroup.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_TargetGroup.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("TargetCode", "Code", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("TargetName", "Name")
		});
		this.luk_TargetGroup.DisplayMember = "TargetName";
		this.luk_TargetGroup.Name = "luk_TargetGroup";
		this.luk_TargetGroup.NullText = "";
		this.luk_TargetGroup.ValueMember = "TargetCode";
		this.gridColumnEx9.Caption = "Start Date";
		this.gridColumnEx9.CurrencyDateField = null;
		this.gridColumnEx9.CurrencyField = null;
		this.gridColumnEx9.CurrencyFixed = null;
		this.gridColumnEx9.CurrencyFixedRate = 0.0;
		this.gridColumnEx9.CurrencyRateField = null;
		this.gridColumnEx9.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gridColumnEx9.FieldName = "StartDate";
		this.gridColumnEx9.Name = "gridColumnEx9";
		this.gridColumnEx9.PrimaryKey = false;
		this.gridColumnEx9.UnitField = null;
		this.gridColumnEx9.Visible = true;
		this.gridColumnEx9.VisibleIndex = 7;
		this.gridColumnEx10.Caption = "End Date";
		this.gridColumnEx10.CurrencyDateField = null;
		this.gridColumnEx10.CurrencyField = null;
		this.gridColumnEx10.CurrencyFixed = null;
		this.gridColumnEx10.CurrencyFixedRate = 0.0;
		this.gridColumnEx10.CurrencyRateField = null;
		this.gridColumnEx10.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gridColumnEx10.FieldName = "EndDate";
		this.gridColumnEx10.Name = "gridColumnEx10";
		this.gridColumnEx10.PrimaryKey = false;
		this.gridColumnEx10.UnitField = null;
		this.gridColumnEx10.Visible = true;
		this.gridColumnEx10.VisibleIndex = 8;
		this.gridColumnEx11.Caption = "Parent Campaign";
		this.gridColumnEx11.ColumnEdit = this.btn_ParentCampaign;
		this.gridColumnEx11.CurrencyDateField = null;
		this.gridColumnEx11.CurrencyField = null;
		this.gridColumnEx11.CurrencyFixed = null;
		this.gridColumnEx11.CurrencyFixedRate = 0.0;
		this.gridColumnEx11.CurrencyRateField = null;
		this.gridColumnEx11.FieldName = "ParentCampaign";
		this.gridColumnEx11.Name = "gridColumnEx11";
		this.gridColumnEx11.PrimaryKey = false;
		this.gridColumnEx11.UnitField = null;
		this.gridColumnEx11.Visible = true;
		this.gridColumnEx11.VisibleIndex = 9;
		this.gridColumnEx11.Width = 111;
		this.btn_ParentCampaign.AutoHeight = false;
		this.btn_ParentCampaign.AutoLoadDataSource = true;
		editorButtonImageOptions2.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions2.Image");
		this.btn_ParentCampaign.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance4, appearanceHovered4, appearancePressed4, appearanceDisabled4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_ParentCampaign.ContainerControl = this;
		this.btn_ParentCampaign.DisplayMember = "CampaignNo";
		this.btn_ParentCampaign.DisplayMember2 = "";
		this.btn_ParentCampaign.LoadDataSourceWhenEnter = false;
		this.btn_ParentCampaign.Name = "btn_ParentCampaign";
		this.btn_ParentCampaign.SeparateCharacterForMulti = ';';
		this.btn_ParentCampaign.SeparateString = ", ";
		this.btn_ParentCampaign.UsingAutoCompleteBox = false;
		this.btn_ParentCampaign.UsingDataSource = true;
		this.btn_ParentCampaign.ValidateWhenNoDataSource = false;
		this.btn_ParentCampaign.ValueMember = "DocEntry";
		this.gridColumnEx12.Caption = "Stage";
		this.gridColumnEx12.ColumnEdit = this.luk_Stage;
		this.gridColumnEx12.CurrencyDateField = null;
		this.gridColumnEx12.CurrencyField = null;
		this.gridColumnEx12.CurrencyFixed = null;
		this.gridColumnEx12.CurrencyFixedRate = 0.0;
		this.gridColumnEx12.CurrencyRateField = null;
		this.gridColumnEx12.FieldName = "Stage";
		this.gridColumnEx12.Name = "gridColumnEx12";
		this.gridColumnEx12.PrimaryKey = false;
		this.gridColumnEx12.UnitField = null;
		this.gridColumnEx12.Visible = true;
		this.gridColumnEx12.VisibleIndex = 10;
		this.luk_Stage.AutoHeight = false;
		this.luk_Stage.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
		this.luk_Stage.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Stage.CheckFields = null;
		this.luk_Stage.CheckTables = null;
		this.luk_Stage.Name = "luk_Stage";
		this.luk_Stage.NullText = "";
		this.luk_Stage.ShowFooter = false;
		this.luk_Stage.ShowHeader = false;
		this.gridColumnEx13.Caption = "Remark";
		this.gridColumnEx13.CurrencyDateField = null;
		this.gridColumnEx13.CurrencyField = null;
		this.gridColumnEx13.CurrencyFixed = null;
		this.gridColumnEx13.CurrencyFixedRate = 0.0;
		this.gridColumnEx13.CurrencyRateField = null;
		this.gridColumnEx13.FieldName = "Remark";
		this.gridColumnEx13.Name = "gridColumnEx13";
		this.gridColumnEx13.PrimaryKey = false;
		this.gridColumnEx13.UnitField = null;
		this.gridColumnEx13.Visible = true;
		this.gridColumnEx13.VisibleIndex = 11;
		this.gridColumnEx14.Caption = "Total Plan Revenue";
		this.gridColumnEx14.ColumnEdit = this.repositoryItemCalcEditEx1;
		this.gridColumnEx14.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.MainCurrency;
		this.gridColumnEx14.CurrencyDateField = null;
		this.gridColumnEx14.CurrencyField = null;
		this.gridColumnEx14.CurrencyFixed = null;
		this.gridColumnEx14.CurrencyFixedRate = 0.0;
		this.gridColumnEx14.CurrencyRateField = null;
		this.gridColumnEx14.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gridColumnEx14.FieldName = "TotalPlRevenue";
		this.gridColumnEx14.Name = "gridColumnEx14";
		this.gridColumnEx14.OptionsColumn.ReadOnly = true;
		this.gridColumnEx14.PrimaryKey = false;
		this.gridColumnEx14.UnitField = null;
		this.gridColumnEx14.Visible = true;
		this.gridColumnEx14.VisibleIndex = 12;
		this.gridColumnEx14.Width = 114;
		this.gridColumnEx15.Caption = "Opportunities";
		this.gridColumnEx15.ColumnEdit = this.repositoryItemCalcEditEx9;
		this.gridColumnEx15.CurrencyDateField = null;
		this.gridColumnEx15.CurrencyField = null;
		this.gridColumnEx15.CurrencyFixed = null;
		this.gridColumnEx15.CurrencyFixedRate = 0.0;
		this.gridColumnEx15.CurrencyRateField = null;
		this.gridColumnEx15.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gridColumnEx15.FieldName = "Opportunities";
		this.gridColumnEx15.Name = "gridColumnEx15";
		this.gridColumnEx15.OptionsColumn.ReadOnly = true;
		this.gridColumnEx15.PrimaryKey = false;
		this.gridColumnEx15.UnitField = null;
		this.gridColumnEx15.Visible = true;
		this.gridColumnEx15.VisibleIndex = 13;
		this.gridColumnEx16.Caption = "Total Quotation";
		this.gridColumnEx16.ColumnEdit = this.repositoryItemCalcEditEx2;
		this.gridColumnEx16.CurrencyDateField = null;
		this.gridColumnEx16.CurrencyField = null;
		this.gridColumnEx16.CurrencyFixed = null;
		this.gridColumnEx16.CurrencyFixedRate = 0.0;
		this.gridColumnEx16.CurrencyRateField = null;
		this.gridColumnEx16.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gridColumnEx16.FieldName = "TotalQuotation";
		this.gridColumnEx16.Name = "gridColumnEx16";
		this.gridColumnEx16.OptionsColumn.ReadOnly = true;
		this.gridColumnEx16.PrimaryKey = false;
		this.gridColumnEx16.UnitField = null;
		this.gridColumnEx16.Visible = true;
		this.gridColumnEx16.VisibleIndex = 14;
		this.gridColumnEx16.Width = 108;
		this.gridColumnEx17.Caption = "Quotation Value";
		this.gridColumnEx17.ColumnEdit = this.repositoryItemCalcEditEx10;
		this.gridColumnEx17.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.MainCurrency;
		this.gridColumnEx17.CurrencyDateField = null;
		this.gridColumnEx17.CurrencyField = null;
		this.gridColumnEx17.CurrencyFixed = null;
		this.gridColumnEx17.CurrencyFixedRate = 0.0;
		this.gridColumnEx17.CurrencyRateField = null;
		this.gridColumnEx17.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gridColumnEx17.FieldName = "QuotationValue";
		this.gridColumnEx17.Name = "gridColumnEx17";
		this.gridColumnEx17.OptionsColumn.ReadOnly = true;
		this.gridColumnEx17.PrimaryKey = false;
		this.gridColumnEx17.UnitField = null;
		this.gridColumnEx17.Visible = true;
		this.gridColumnEx17.VisibleIndex = 15;
		this.gridColumnEx17.Width = 110;
		this.gridColumnEx18.Caption = "Total Order";
		this.gridColumnEx18.ColumnEdit = this.repositoryItemCalcEditEx3;
		this.gridColumnEx18.CurrencyDateField = null;
		this.gridColumnEx18.CurrencyField = null;
		this.gridColumnEx18.CurrencyFixed = null;
		this.gridColumnEx18.CurrencyFixedRate = 0.0;
		this.gridColumnEx18.CurrencyRateField = null;
		this.gridColumnEx18.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gridColumnEx18.FieldName = "TotalOrder";
		this.gridColumnEx18.Name = "gridColumnEx18";
		this.gridColumnEx18.OptionsColumn.ReadOnly = true;
		this.gridColumnEx18.PrimaryKey = false;
		this.gridColumnEx18.UnitField = null;
		this.gridColumnEx18.Visible = true;
		this.gridColumnEx18.VisibleIndex = 16;
		this.gridColumnEx19.Caption = "Order Value";
		this.gridColumnEx19.ColumnEdit = this.repositoryItemCalcEditEx11;
		this.gridColumnEx19.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.MainCurrency;
		this.gridColumnEx19.CurrencyDateField = null;
		this.gridColumnEx19.CurrencyField = null;
		this.gridColumnEx19.CurrencyFixed = null;
		this.gridColumnEx19.CurrencyFixedRate = 0.0;
		this.gridColumnEx19.CurrencyRateField = null;
		this.gridColumnEx19.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gridColumnEx19.FieldName = "OrderValue";
		this.gridColumnEx19.Name = "gridColumnEx19";
		this.gridColumnEx19.OptionsColumn.ReadOnly = true;
		this.gridColumnEx19.PrimaryKey = false;
		this.gridColumnEx19.UnitField = null;
		this.gridColumnEx19.Visible = true;
		this.gridColumnEx19.VisibleIndex = 17;
		this.gridColumnEx61.Caption = "Project";
		this.gridColumnEx61.ColumnEdit = this.btn_ProjectId;
		this.gridColumnEx61.CurrencyDateField = null;
		this.gridColumnEx61.CurrencyField = null;
		this.gridColumnEx61.CurrencyFixed = null;
		this.gridColumnEx61.CurrencyFixedRate = 0.0;
		this.gridColumnEx61.CurrencyRateField = null;
		this.gridColumnEx61.FieldName = "ProjectId";
		this.gridColumnEx61.Name = "gridColumnEx61";
		this.gridColumnEx61.PrimaryKey = false;
		this.gridColumnEx61.UnitField = null;
		this.gridColumnEx61.Visible = true;
		this.gridColumnEx61.VisibleIndex = 18;
		this.btn_ProjectId.AutoHeight = false;
		this.btn_ProjectId.AutoLoadDataSource = true;
		editorButtonImageOptions3.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions4.Image");
		this.btn_ProjectId.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance5, appearanceHovered5, appearancePressed5, appearanceDisabled5, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance6, appearanceHovered6, appearancePressed6, appearanceDisabled6, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_ProjectId.ContainerControl = this;
		this.btn_ProjectId.DisplayMember = "PrjCode";
		this.btn_ProjectId.DisplayMember2 = "";
		this.btn_ProjectId.LoadDataSourceWhenEnter = false;
		this.btn_ProjectId.Name = "btn_ProjectId";
		this.btn_ProjectId.SeparateCharacterForMulti = ';';
		this.btn_ProjectId.SeparateString = ", ";
		this.btn_ProjectId.UsingAutoCompleteBox = false;
		this.btn_ProjectId.UsingDataSource = true;
		this.btn_ProjectId.ValidateWhenNoDataSource = false;
		this.btn_ProjectId.ValueMember = "PrjCode";
		this.gridColumnEx100.Caption = "AtcEntry";
		this.gridColumnEx100.CurrencyDateField = null;
		this.gridColumnEx100.CurrencyField = null;
		this.gridColumnEx100.CurrencyFixed = null;
		this.gridColumnEx100.CurrencyFixedRate = 0.0;
		this.gridColumnEx100.CurrencyRateField = null;
		this.gridColumnEx100.FieldName = "AtcEntry";
		this.gridColumnEx100.Name = "gridColumnEx100";
		this.gridColumnEx100.OptionsColumn.ReadOnly = true;
		this.gridColumnEx100.PrimaryKey = false;
		this.gridColumnEx100.UnitField = null;
		this.xtraTabControlEx1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.xtraTabControlEx1.IsSearchControl = false;
		this.xtraTabControlEx1.Location = new System.Drawing.Point(0, 0);
		this.xtraTabControlEx1.Name = "xtraTabControlEx1";
		this.xtraTabControlEx1.SelectedTabPage = this.xtra_Bp;
		this.xtraTabControlEx1.Size = new System.Drawing.Size(973, 242);
		this.xtraTabControlEx1.TabIndex = 0;
		this.xtraTabControlEx1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[9] { this.xtra_Bp, this.xtra_Item, this.xtra_Partner, this.xtra_Member, this.xtra_Atta, this.xtra_Cost, this.xtra_Task, this.xtra_Sub, this.xtra_Document });
		this.xtra_Bp.Controls.Add(this.grd_Bp);
		this.xtra_Bp.EditControl = null;
		this.xtra_Bp.EditValue = "Target BP";
		this.xtra_Bp.FieldName = "";
		this.xtra_Bp.IsSearchControl = false;
		this.xtra_Bp.Name = "xtra_Bp";
		this.xtra_Bp.ReadOnly = false;
		this.xtra_Bp.SettingInVisible = false;
		this.xtra_Bp.SettingReadonly = false;
		this.xtra_Bp.Size = new System.Drawing.Size(971, 217);
		this.xtra_Bp.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtra_Bp.Text = "Target BP";
		this.grd_Bp.AllowPasteExcel = true;
		this.grd_Bp.ClipboardData = null;
		this.grd_Bp.ContextMenuStrip = this.ctm_Bp;
		this.grd_Bp.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Bp.Location = new System.Drawing.Point(0, 0);
		this.grd_Bp.MainView = this.gv_Bp;
		this.grd_Bp.Name = "grd_Bp";
		this.grd_Bp.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[11]
		{
			this.btn_BpCode, this.luk_StatusBp, this.luk_Qualified, this.btn_BpCustomer, this.btn_BpVender, this.luk_BpGroup, this.luk_Industry, this.btn_Territory, this.cke_ShowBPDoc, this.luk_DocType,
			this.btn_DocNum
		});
		this.grd_Bp.Size = new System.Drawing.Size(971, 217);
		this.grd_Bp.TabIndex = 0;
		this.grd_Bp.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Bp });
		this.ctm_Bp.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addRow_Bp, this.deleteRow_Bp });
		this.ctm_Bp.Name = "ctm_Bp";
		this.ctm_Bp.Size = new System.Drawing.Size(134, 48);
		this.addRow_Bp.Name = "addRow_Bp";
		this.addRow_Bp.Size = new System.Drawing.Size(133, 22);
		this.addRow_Bp.Text = "Add Row";
		this.addRow_Bp.Click += new System.EventHandler(this.addRow_Bp_Click);
		this.deleteRow_Bp.Name = "deleteRow_Bp";
		this.deleteRow_Bp.Size = new System.Drawing.Size(133, 22);
		this.deleteRow_Bp.Text = "Delete Row";
		this.deleteRow_Bp.Click += new System.EventHandler(this.deleteRow_Bp_Click);
		this.gv_Bp.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Bp.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Bp.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Bp.ColumnKey = null;
		this.gv_Bp.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[18]
		{
			this.gridColumnEx20, this.gridColumnEx21, this.gridColumnEx22, this.gridColumnEx23, this.gridColumnEx24, this.gridColumnEx25, this.gridColumnEx26, this.gridColumnEx27, this.gridColumnEx28, this.gridColumnEx29,
			this.gridColumnEx30, this.gridColumnEx31, this.gridColumnEx32, this.gridColumnEx33, this.gridColumnEx99, this.gridColumnEx102, this.gridColumnEx103, this.gridColumnEx104
		});
		this.gv_Bp.DisableFormSetting = false;
		this.gv_Bp.DisplayException = false;
		this.gv_Bp.EditColumn = null;
		this.gv_Bp.FieldCurrency = null;
		this.gv_Bp.FieldRate = null;
		this.gv_Bp.FormId = "oMarketingCampaign";
		this.gv_Bp.GridControl = this.grd_Bp;
		this.gv_Bp.IgnoreFilter = false;
		this.gv_Bp.IndexEditRow = -1;
		this.gv_Bp.IndicatorWidth = 35;
		this.gv_Bp.IsApzTable = false;
		this.gv_Bp.IsInsertNewRow = false;
		this.gv_Bp.IsLoadingData = false;
		this.gv_Bp.IsMappingContextAction = false;
		this.gv_Bp.ItemId = "APZ_SMC1";
		this.gv_Bp.LstColumnAllowBatch = null;
		this.gv_Bp.LstColumnNotAllowBatch = null;
		this.gv_Bp.LstColumnNotEditable = null;
		this.gv_Bp.LstColumNotSetting = null;
		this.gv_Bp.Name = "gv_Bp";
		this.gv_Bp.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Bp.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Bp.OptionsView.ColumnAutoWidth = false;
		this.gv_Bp.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Bp.OptionsView.ShowGroupPanel = false;
		this.gv_Bp.UseBatchMod = false;
		this.gv_Bp.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Bp_CellValueChanged);
		this.gv_Bp.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.gv_Bp_CustomColumnDisplayText);
		this.gridColumnEx20.Caption = "DocEntry";
		this.gridColumnEx20.CurrencyDateField = null;
		this.gridColumnEx20.CurrencyField = null;
		this.gridColumnEx20.CurrencyFixed = null;
		this.gridColumnEx20.CurrencyFixedRate = 0.0;
		this.gridColumnEx20.CurrencyRateField = null;
		this.gridColumnEx20.FieldName = "DocEntry";
		this.gridColumnEx20.Name = "gridColumnEx20";
		this.gridColumnEx20.OptionsColumn.AllowEdit = false;
		this.gridColumnEx20.OptionsColumn.ReadOnly = true;
		this.gridColumnEx20.PrimaryKey = true;
		this.gridColumnEx20.UnitField = null;
		this.gridColumnEx21.Caption = "LineNum";
		this.gridColumnEx21.CurrencyDateField = null;
		this.gridColumnEx21.CurrencyField = null;
		this.gridColumnEx21.CurrencyFixed = null;
		this.gridColumnEx21.CurrencyFixedRate = 0.0;
		this.gridColumnEx21.CurrencyRateField = null;
		this.gridColumnEx21.FieldName = "LineNum";
		this.gridColumnEx21.Name = "gridColumnEx21";
		this.gridColumnEx21.OptionsColumn.ReadOnly = true;
		this.gridColumnEx21.PrimaryKey = false;
		this.gridColumnEx21.UnitField = null;
		this.gridColumnEx22.Caption = "Bp Code";
		this.gridColumnEx22.ColumnEdit = this.btn_BpCode;
		this.gridColumnEx22.CurrencyDateField = null;
		this.gridColumnEx22.CurrencyField = null;
		this.gridColumnEx22.CurrencyFixed = null;
		this.gridColumnEx22.CurrencyFixedRate = 0.0;
		this.gridColumnEx22.CurrencyRateField = null;
		this.gridColumnEx22.FieldName = "BpCode";
		this.gridColumnEx22.Name = "gridColumnEx22";
		this.gridColumnEx22.PrimaryKey = true;
		this.gridColumnEx22.UnitField = null;
		this.gridColumnEx22.Visible = true;
		this.gridColumnEx22.VisibleIndex = 0;
		this.gridColumnEx22.Width = 121;
		this.btn_BpCode.AutoHeight = false;
		this.btn_BpCode.AutoLoadDataSource = true;
		editorButtonImageOptions4.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions6.Image");
		this.btn_BpCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance7, appearanceHovered7, appearancePressed7, appearanceDisabled7, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance8, appearanceHovered8, appearancePressed8, appearanceDisabled8, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_BpCode.ContainerControl = this;
		this.btn_BpCode.DisplayMember = "";
		this.btn_BpCode.DisplayMember2 = "";
		this.btn_BpCode.LoadDataSourceWhenEnter = false;
		this.btn_BpCode.Name = "btn_BpCode";
		this.btn_BpCode.SeparateCharacterForMulti = ';';
		this.btn_BpCode.SeparateString = ", ";
		this.btn_BpCode.UsingAutoCompleteBox = false;
		this.btn_BpCode.UsingDataSource = true;
		this.btn_BpCode.ValidateWhenNoDataSource = false;
		this.btn_BpCode.ValueMember = "";
		this.gridColumnEx23.Caption = "Bp Name";
		this.gridColumnEx23.CurrencyDateField = null;
		this.gridColumnEx23.CurrencyField = null;
		this.gridColumnEx23.CurrencyFixed = null;
		this.gridColumnEx23.CurrencyFixedRate = 0.0;
		this.gridColumnEx23.CurrencyRateField = null;
		this.gridColumnEx23.FieldName = "BpName";
		this.gridColumnEx23.Name = "gridColumnEx23";
		this.gridColumnEx23.PrimaryKey = false;
		this.gridColumnEx23.UnitField = null;
		this.gridColumnEx23.Visible = true;
		this.gridColumnEx23.VisibleIndex = 1;
		this.gridColumnEx23.Width = 121;
		this.gridColumnEx24.Caption = "Bp Group";
		this.gridColumnEx24.ColumnEdit = this.luk_BpGroup;
		this.gridColumnEx24.CurrencyDateField = null;
		this.gridColumnEx24.CurrencyField = null;
		this.gridColumnEx24.CurrencyFixed = null;
		this.gridColumnEx24.CurrencyFixedRate = 0.0;
		this.gridColumnEx24.CurrencyRateField = null;
		this.gridColumnEx24.FieldName = "BpGroup";
		this.gridColumnEx24.Name = "gridColumnEx24";
		this.gridColumnEx24.PrimaryKey = false;
		this.gridColumnEx24.UnitField = null;
		this.gridColumnEx24.Visible = true;
		this.gridColumnEx24.VisibleIndex = 2;
		this.gridColumnEx24.Width = 125;
		this.luk_BpGroup.AutoHeight = false;
		this.luk_BpGroup.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
		this.luk_BpGroup.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_BpGroup.CheckFields = null;
		this.luk_BpGroup.CheckTables = null;
		this.luk_BpGroup.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("GroupCode", "Code"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("GroupName", "Name")
		});
		this.luk_BpGroup.DisplayMember = "GroupName";
		this.luk_BpGroup.Name = "luk_BpGroup";
		this.luk_BpGroup.NullText = "";
		this.luk_BpGroup.ShowFooter = false;
		this.luk_BpGroup.ShowHeader = false;
		this.luk_BpGroup.ValueMember = "GroupCode";
		this.gridColumnEx25.Caption = "Bp Address";
		this.gridColumnEx25.CurrencyDateField = null;
		this.gridColumnEx25.CurrencyField = null;
		this.gridColumnEx25.CurrencyFixed = null;
		this.gridColumnEx25.CurrencyFixedRate = 0.0;
		this.gridColumnEx25.CurrencyRateField = null;
		this.gridColumnEx25.FieldName = "BpAddress";
		this.gridColumnEx25.Name = "gridColumnEx25";
		this.gridColumnEx25.PrimaryKey = false;
		this.gridColumnEx25.UnitField = null;
		this.gridColumnEx25.Visible = true;
		this.gridColumnEx25.VisibleIndex = 3;
		this.gridColumnEx25.Width = 171;
		this.gridColumnEx26.Caption = "Territory";
		this.gridColumnEx26.ColumnEdit = this.btn_Territory;
		this.gridColumnEx26.CurrencyDateField = null;
		this.gridColumnEx26.CurrencyField = null;
		this.gridColumnEx26.CurrencyFixed = null;
		this.gridColumnEx26.CurrencyFixedRate = 0.0;
		this.gridColumnEx26.CurrencyRateField = null;
		this.gridColumnEx26.FieldName = "Territory";
		this.gridColumnEx26.Name = "gridColumnEx26";
		this.gridColumnEx26.PrimaryKey = false;
		this.gridColumnEx26.UnitField = null;
		this.gridColumnEx26.Visible = true;
		this.gridColumnEx26.VisibleIndex = 5;
		this.gridColumnEx26.Width = 135;
		this.btn_Territory.AutoHeight = false;
		this.btn_Territory.AutoLoadDataSource = true;
		this.btn_Territory.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions5, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance9, appearanceHovered9, appearancePressed9, appearanceDisabled9, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_Territory.ContainerControl = this;
		this.btn_Territory.DisplayMember = "descript";
		this.btn_Territory.DisplayMember2 = "";
		this.btn_Territory.LoadDataSourceWhenEnter = false;
		this.btn_Territory.Name = "btn_Territory";
		this.btn_Territory.SeparateCharacterForMulti = ';';
		this.btn_Territory.SeparateString = ", ";
		this.btn_Territory.UsingAutoCompleteBox = false;
		this.btn_Territory.UsingDataSource = true;
		this.btn_Territory.ValidateWhenNoDataSource = false;
		this.btn_Territory.ValueMember = "territryID";
		this.gridColumnEx27.Caption = "Contact Name";
		this.gridColumnEx27.CurrencyDateField = null;
		this.gridColumnEx27.CurrencyField = null;
		this.gridColumnEx27.CurrencyFixed = null;
		this.gridColumnEx27.CurrencyFixedRate = 0.0;
		this.gridColumnEx27.CurrencyRateField = null;
		this.gridColumnEx27.FieldName = "ContactName";
		this.gridColumnEx27.Name = "gridColumnEx27";
		this.gridColumnEx27.PrimaryKey = false;
		this.gridColumnEx27.UnitField = null;
		this.gridColumnEx27.Visible = true;
		this.gridColumnEx27.VisibleIndex = 6;
		this.gridColumnEx27.Width = 125;
		this.gridColumnEx28.Caption = "Contact Email";
		this.gridColumnEx28.CurrencyDateField = null;
		this.gridColumnEx28.CurrencyField = null;
		this.gridColumnEx28.CurrencyFixed = null;
		this.gridColumnEx28.CurrencyFixedRate = 0.0;
		this.gridColumnEx28.CurrencyRateField = null;
		this.gridColumnEx28.FieldName = "ContactEmail";
		this.gridColumnEx28.Name = "gridColumnEx28";
		this.gridColumnEx28.PrimaryKey = false;
		this.gridColumnEx28.UnitField = null;
		this.gridColumnEx28.Visible = true;
		this.gridColumnEx28.VisibleIndex = 7;
		this.gridColumnEx28.Width = 177;
		this.gridColumnEx29.Caption = "Contact Phone";
		this.gridColumnEx29.CurrencyDateField = null;
		this.gridColumnEx29.CurrencyField = null;
		this.gridColumnEx29.CurrencyFixed = null;
		this.gridColumnEx29.CurrencyFixedRate = 0.0;
		this.gridColumnEx29.CurrencyRateField = null;
		this.gridColumnEx29.FieldName = "ContactPhone";
		this.gridColumnEx29.Name = "gridColumnEx29";
		this.gridColumnEx29.PrimaryKey = false;
		this.gridColumnEx29.UnitField = null;
		this.gridColumnEx29.Visible = true;
		this.gridColumnEx29.VisibleIndex = 8;
		this.gridColumnEx29.Width = 116;
		this.gridColumnEx30.Caption = "Contact Position";
		this.gridColumnEx30.CurrencyDateField = null;
		this.gridColumnEx30.CurrencyField = null;
		this.gridColumnEx30.CurrencyFixed = null;
		this.gridColumnEx30.CurrencyFixedRate = 0.0;
		this.gridColumnEx30.CurrencyRateField = null;
		this.gridColumnEx30.FieldName = "ContactPosition";
		this.gridColumnEx30.Name = "gridColumnEx30";
		this.gridColumnEx30.PrimaryKey = false;
		this.gridColumnEx30.UnitField = null;
		this.gridColumnEx30.Visible = true;
		this.gridColumnEx30.VisibleIndex = 9;
		this.gridColumnEx30.Width = 111;
		this.gridColumnEx31.Caption = "Qualified";
		this.gridColumnEx31.ColumnEdit = this.luk_Qualified;
		this.gridColumnEx31.CurrencyDateField = null;
		this.gridColumnEx31.CurrencyField = null;
		this.gridColumnEx31.CurrencyFixed = null;
		this.gridColumnEx31.CurrencyFixedRate = 0.0;
		this.gridColumnEx31.CurrencyRateField = null;
		this.gridColumnEx31.FieldName = "Qualified";
		this.gridColumnEx31.Name = "gridColumnEx31";
		this.gridColumnEx31.PrimaryKey = false;
		this.gridColumnEx31.UnitField = null;
		this.gridColumnEx31.Visible = true;
		this.gridColumnEx31.VisibleIndex = 10;
		this.luk_Qualified.AutoHeight = false;
		this.luk_Qualified.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_Qualified.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Qualified.IsSearchControl = false;
		this.luk_Qualified.Name = "luk_Qualified";
		this.luk_Qualified.NullText = "";
		this.luk_Qualified.ShowFooter = false;
		this.luk_Qualified.ShowHeader = false;
		this.gridColumnEx32.Caption = "Qualified Date";
		this.gridColumnEx32.CurrencyDateField = null;
		this.gridColumnEx32.CurrencyField = null;
		this.gridColumnEx32.CurrencyFixed = null;
		this.gridColumnEx32.CurrencyFixedRate = 0.0;
		this.gridColumnEx32.CurrencyRateField = null;
		this.gridColumnEx32.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gridColumnEx32.FieldName = "QualifiedDate";
		this.gridColumnEx32.Name = "gridColumnEx32";
		this.gridColumnEx32.PrimaryKey = false;
		this.gridColumnEx32.UnitField = null;
		this.gridColumnEx32.Visible = true;
		this.gridColumnEx32.VisibleIndex = 11;
		this.gridColumnEx32.Width = 96;
		this.gridColumnEx33.Caption = "Status";
		this.gridColumnEx33.ColumnEdit = this.luk_StatusBp;
		this.gridColumnEx33.CurrencyDateField = null;
		this.gridColumnEx33.CurrencyField = null;
		this.gridColumnEx33.CurrencyFixed = null;
		this.gridColumnEx33.CurrencyFixedRate = 0.0;
		this.gridColumnEx33.CurrencyRateField = null;
		this.gridColumnEx33.FieldName = "Status";
		this.gridColumnEx33.Name = "gridColumnEx33";
		this.gridColumnEx33.PrimaryKey = false;
		this.gridColumnEx33.UnitField = null;
		this.gridColumnEx33.Visible = true;
		this.gridColumnEx33.VisibleIndex = 12;
		this.luk_StatusBp.AutoHeight = false;
		this.luk_StatusBp.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_StatusBp.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_StatusBp.IsSearchControl = false;
		this.luk_StatusBp.Name = "luk_StatusBp";
		this.luk_StatusBp.NullText = "";
		this.luk_StatusBp.ShowFooter = false;
		this.luk_StatusBp.ShowHeader = false;
		this.gridColumnEx99.Caption = "Industry";
		this.gridColumnEx99.ColumnEdit = this.luk_Industry;
		this.gridColumnEx99.CurrencyDateField = null;
		this.gridColumnEx99.CurrencyField = null;
		this.gridColumnEx99.CurrencyFixed = null;
		this.gridColumnEx99.CurrencyFixedRate = 0.0;
		this.gridColumnEx99.CurrencyRateField = null;
		this.gridColumnEx99.FieldName = "Industry";
		this.gridColumnEx99.Name = "gridColumnEx99";
		this.gridColumnEx99.PrimaryKey = false;
		this.gridColumnEx99.UnitField = null;
		this.gridColumnEx99.Visible = true;
		this.gridColumnEx99.VisibleIndex = 4;
		this.gridColumnEx99.Width = 104;
		this.luk_Industry.AutoHeight = false;
		this.luk_Industry.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
		this.luk_Industry.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Industry.CheckFields = null;
		this.luk_Industry.CheckTables = null;
		this.luk_Industry.Name = "luk_Industry";
		this.luk_Industry.NullText = "";
		this.luk_Industry.ShowFooter = false;
		this.luk_Industry.ShowHeader = false;
		this.gridColumnEx102.Caption = "Document Type";
		this.gridColumnEx102.ColumnEdit = this.luk_DocType;
		this.gridColumnEx102.CurrencyDateField = null;
		this.gridColumnEx102.CurrencyField = null;
		this.gridColumnEx102.CurrencyFixed = null;
		this.gridColumnEx102.CurrencyFixedRate = 0.0;
		this.gridColumnEx102.CurrencyRateField = null;
		this.gridColumnEx102.FieldName = "DocType";
		this.gridColumnEx102.Name = "gridColumnEx102";
		this.gridColumnEx102.PrimaryKey = false;
		this.gridColumnEx102.UnitField = null;
		this.gridColumnEx102.Visible = true;
		this.gridColumnEx102.VisibleIndex = 13;
		this.gridColumnEx102.Width = 125;
		this.luk_DocType.AutoHeight = false;
		this.luk_DocType.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_DocType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_DocType.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "")
		});
		this.luk_DocType.IsSearchControl = false;
		this.luk_DocType.Name = "luk_DocType";
		this.luk_DocType.NullText = "";
		this.luk_DocType.ShowFooter = false;
		this.luk_DocType.ShowHeader = false;
		this.gridColumnEx103.Caption = "Show BP Documents";
		this.gridColumnEx103.ColumnEdit = this.cke_ShowBPDoc;
		this.gridColumnEx103.CurrencyDateField = null;
		this.gridColumnEx103.CurrencyField = null;
		this.gridColumnEx103.CurrencyFixed = null;
		this.gridColumnEx103.CurrencyFixedRate = 0.0;
		this.gridColumnEx103.CurrencyRateField = null;
		this.gridColumnEx103.FieldName = "ShowBPDoc";
		this.gridColumnEx103.Name = "gridColumnEx103";
		this.gridColumnEx103.PrimaryKey = false;
		this.gridColumnEx103.UnitField = null;
		this.gridColumnEx103.Visible = true;
		this.gridColumnEx103.VisibleIndex = 14;
		this.gridColumnEx103.Width = 125;
		this.cke_ShowBPDoc.AutoHeight = false;
		this.cke_ShowBPDoc.Name = "cke_ShowBPDoc";
		this.cke_ShowBPDoc.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.cke_ShowBPDoc.ValueChecked = "Y";
		this.cke_ShowBPDoc.ValueUnchecked = "N";
		this.gridColumnEx104.Caption = "Document Number";
		this.gridColumnEx104.ColumnEdit = this.btn_DocNum;
		this.gridColumnEx104.CurrencyDateField = null;
		this.gridColumnEx104.CurrencyField = null;
		this.gridColumnEx104.CurrencyFixed = null;
		this.gridColumnEx104.CurrencyFixedRate = 0.0;
		this.gridColumnEx104.CurrencyRateField = null;
		this.gridColumnEx104.FieldName = "DocNumber";
		this.gridColumnEx104.Name = "gridColumnEx104";
		this.gridColumnEx104.PrimaryKey = false;
		this.gridColumnEx104.UnitField = null;
		this.gridColumnEx104.Visible = true;
		this.gridColumnEx104.VisibleIndex = 15;
		this.gridColumnEx104.Width = 125;
		this.btn_DocNum.AutoHeight = false;
		this.btn_DocNum.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph)
		});
		this.btn_DocNum.Name = "btn_DocNum";
		this.btn_BpCustomer.AutoHeight = false;
		this.btn_BpCustomer.AutoLoadDataSource = true;
		editorButtonImageOptions5.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions9.Image");
		this.btn_BpCustomer.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions6, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance10, appearanceHovered10, appearancePressed10, appearanceDisabled10, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions5, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance11, appearanceHovered11, appearancePressed11, appearanceDisabled11, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_BpCustomer.ContainerControl = this;
		this.btn_BpCustomer.DisplayMember = "";
		this.btn_BpCustomer.DisplayMember2 = "";
		this.btn_BpCustomer.LoadDataSourceWhenEnter = false;
		this.btn_BpCustomer.Name = "btn_BpCustomer";
		this.btn_BpCustomer.SeparateCharacterForMulti = ';';
		this.btn_BpCustomer.SeparateString = ", ";
		this.btn_BpCustomer.UsingAutoCompleteBox = false;
		this.btn_BpCustomer.UsingDataSource = true;
		this.btn_BpCustomer.ValidateWhenNoDataSource = false;
		this.btn_BpCustomer.ValueMember = "";
		this.btn_BpVender.AutoHeight = false;
		this.btn_BpVender.AutoLoadDataSource = true;
		editorButtonImageOptions6.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions11.Image");
		this.btn_BpVender.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions7, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance12, appearanceHovered12, appearancePressed12, appearanceDisabled12, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions6, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance13, appearanceHovered13, appearancePressed13, appearanceDisabled13, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_BpVender.ContainerControl = this;
		this.btn_BpVender.DisplayMember = "";
		this.btn_BpVender.DisplayMember2 = "";
		this.btn_BpVender.LoadDataSourceWhenEnter = false;
		this.btn_BpVender.Name = "btn_BpVender";
		this.btn_BpVender.SeparateCharacterForMulti = ';';
		this.btn_BpVender.SeparateString = ", ";
		this.btn_BpVender.UsingAutoCompleteBox = false;
		this.btn_BpVender.UsingDataSource = true;
		this.btn_BpVender.ValidateWhenNoDataSource = false;
		this.btn_BpVender.ValueMember = "";
		this.xtra_Item.Controls.Add(this.grd_Item);
		this.xtra_Item.EditControl = null;
		this.xtra_Item.EditValue = "Items";
		this.xtra_Item.FieldName = null;
		this.xtra_Item.IsSearchControl = false;
		this.xtra_Item.Name = "xtra_Item";
		this.xtra_Item.ReadOnly = false;
		this.xtra_Item.SettingInVisible = false;
		this.xtra_Item.SettingReadonly = false;
		this.xtra_Item.Size = new System.Drawing.Size(971, 217);
		this.xtra_Item.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtra_Item.Text = "Items";
		this.grd_Item.AllowPasteExcel = true;
		this.grd_Item.ClipboardData = null;
		this.grd_Item.ContextMenuStrip = this.ctm_Item;
		this.grd_Item.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Item.Location = new System.Drawing.Point(0, 0);
		this.grd_Item.MainView = this.gv_Item;
		this.grd_Item.Name = "grd_Item";
		this.grd_Item.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.btn_ItemCode, this.luk_ItemGroup });
		this.grd_Item.Size = new System.Drawing.Size(971, 217);
		this.grd_Item.TabIndex = 0;
		this.grd_Item.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Item });
		this.ctm_Item.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addRow_Item, this.deleteRow_Item });
		this.ctm_Item.Name = "ctm_Item";
		this.ctm_Item.Size = new System.Drawing.Size(134, 48);
		this.addRow_Item.Name = "addRow_Item";
		this.addRow_Item.Size = new System.Drawing.Size(133, 22);
		this.addRow_Item.Text = "Add Row";
		this.addRow_Item.Click += new System.EventHandler(this.addRow_Item_Click);
		this.deleteRow_Item.Name = "deleteRow_Item";
		this.deleteRow_Item.Size = new System.Drawing.Size(133, 22);
		this.deleteRow_Item.Text = "Delete Row";
		this.deleteRow_Item.Click += new System.EventHandler(this.deleteRow_Item_Click);
		this.gv_Item.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Item.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Item.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Item.ColumnKey = null;
		this.gv_Item.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[9] { this.gridColumnEx62, this.gridColumnEx63, this.gridColumnEx64, this.gridColumnEx65, this.gridColumnEx66, this.gridColumnEx67, this.gridColumnEx68, this.gridColumnEx69, this.gridColumnEx101 });
		this.gv_Item.DisableFormSetting = false;
		this.gv_Item.DisplayException = false;
		this.gv_Item.EditColumn = null;
		this.gv_Item.FieldCurrency = null;
		this.gv_Item.FieldRate = null;
		this.gv_Item.FormId = "oMarketingCampaign";
		this.gv_Item.GridControl = this.grd_Item;
		this.gv_Item.IgnoreFilter = false;
		this.gv_Item.IndexEditRow = -1;
		this.gv_Item.IndicatorWidth = 35;
		this.gv_Item.IsApzTable = false;
		this.gv_Item.IsInsertNewRow = false;
		this.gv_Item.IsLoadingData = false;
		this.gv_Item.IsMappingContextAction = false;
		this.gv_Item.ItemId = "APZ_SMC2";
		this.gv_Item.LstColumnAllowBatch = null;
		this.gv_Item.LstColumnNotAllowBatch = null;
		this.gv_Item.LstColumnNotEditable = null;
		this.gv_Item.LstColumNotSetting = null;
		this.gv_Item.Name = "gv_Item";
		this.gv_Item.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Item.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Item.OptionsView.ColumnAutoWidth = false;
		this.gv_Item.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Item.OptionsView.ShowGroupPanel = false;
		this.gv_Item.UseBatchMod = false;
		this.gv_Item.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Item_RowCellStyle);
		this.gv_Item.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Item_CellValueChanged);
		this.gridColumnEx62.Caption = "DocEntry";
		this.gridColumnEx62.CurrencyDateField = null;
		this.gridColumnEx62.CurrencyField = null;
		this.gridColumnEx62.CurrencyFixed = null;
		this.gridColumnEx62.CurrencyFixedRate = 0.0;
		this.gridColumnEx62.CurrencyRateField = null;
		this.gridColumnEx62.FieldName = "DocEntry";
		this.gridColumnEx62.Name = "gridColumnEx62";
		this.gridColumnEx62.OptionsColumn.AllowEdit = false;
		this.gridColumnEx62.OptionsColumn.ReadOnly = true;
		this.gridColumnEx62.PrimaryKey = false;
		this.gridColumnEx62.UnitField = null;
		this.gridColumnEx63.Caption = "LineNum";
		this.gridColumnEx63.CurrencyDateField = null;
		this.gridColumnEx63.CurrencyField = null;
		this.gridColumnEx63.CurrencyFixed = null;
		this.gridColumnEx63.CurrencyFixedRate = 0.0;
		this.gridColumnEx63.CurrencyRateField = null;
		this.gridColumnEx63.FieldName = "LineNum";
		this.gridColumnEx63.Name = "gridColumnEx63";
		this.gridColumnEx63.OptionsColumn.ReadOnly = true;
		this.gridColumnEx63.PrimaryKey = false;
		this.gridColumnEx63.UnitField = null;
		this.gridColumnEx64.Caption = "Item Code";
		this.gridColumnEx64.ColumnEdit = this.btn_ItemCode;
		this.gridColumnEx64.CurrencyDateField = null;
		this.gridColumnEx64.CurrencyField = null;
		this.gridColumnEx64.CurrencyFixed = null;
		this.gridColumnEx64.CurrencyFixedRate = 0.0;
		this.gridColumnEx64.CurrencyRateField = null;
		this.gridColumnEx64.FieldName = "ItemCode";
		this.gridColumnEx64.Name = "gridColumnEx64";
		this.gridColumnEx64.PrimaryKey = true;
		this.gridColumnEx64.UnitField = null;
		this.gridColumnEx64.Visible = true;
		this.gridColumnEx64.VisibleIndex = 0;
		this.btn_ItemCode.AutoHeight = false;
		this.btn_ItemCode.AutoLoadDataSource = true;
		editorButtonImageOptions7.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions13.Image");
		this.btn_ItemCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions8, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance14, appearanceHovered14, appearancePressed14, appearanceDisabled14, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions7, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance15, appearanceHovered15, appearancePressed15, appearanceDisabled15, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_ItemCode.ContainerControl = this;
		this.btn_ItemCode.DisplayMember = "ItemCode";
		this.btn_ItemCode.DisplayMember2 = "";
		this.btn_ItemCode.LoadDataSourceWhenEnter = false;
		this.btn_ItemCode.Name = "btn_ItemCode";
		this.btn_ItemCode.SeparateCharacterForMulti = ';';
		this.btn_ItemCode.SeparateString = ", ";
		this.btn_ItemCode.UsingAutoCompleteBox = false;
		this.btn_ItemCode.UsingDataSource = true;
		this.btn_ItemCode.ValidateWhenNoDataSource = false;
		this.btn_ItemCode.ValueMember = "ItemCode";
		this.gridColumnEx65.Caption = "Item Name";
		this.gridColumnEx65.CurrencyDateField = null;
		this.gridColumnEx65.CurrencyField = null;
		this.gridColumnEx65.CurrencyFixed = null;
		this.gridColumnEx65.CurrencyFixedRate = 0.0;
		this.gridColumnEx65.CurrencyRateField = null;
		this.gridColumnEx65.FieldName = "ItemName";
		this.gridColumnEx65.Name = "gridColumnEx65";
		this.gridColumnEx65.PrimaryKey = false;
		this.gridColumnEx65.UnitField = null;
		this.gridColumnEx65.Visible = true;
		this.gridColumnEx65.VisibleIndex = 1;
		this.gridColumnEx66.Caption = "Group";
		this.gridColumnEx66.ColumnEdit = this.luk_ItemGroup;
		this.gridColumnEx66.CurrencyDateField = null;
		this.gridColumnEx66.CurrencyField = null;
		this.gridColumnEx66.CurrencyFixed = null;
		this.gridColumnEx66.CurrencyFixedRate = 0.0;
		this.gridColumnEx66.CurrencyRateField = null;
		this.gridColumnEx66.FieldName = "ItemGroup";
		this.gridColumnEx66.Name = "gridColumnEx66";
		this.gridColumnEx66.OptionsColumn.ReadOnly = true;
		this.gridColumnEx66.PrimaryKey = false;
		this.gridColumnEx66.UnitField = null;
		this.gridColumnEx66.Visible = true;
		this.gridColumnEx66.VisibleIndex = 2;
		this.luk_ItemGroup.AutoHeight = false;
		this.luk_ItemGroup.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
		this.luk_ItemGroup.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_ItemGroup.CheckFields = null;
		this.luk_ItemGroup.CheckTables = null;
		this.luk_ItemGroup.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ItmsGrpCod", "Code"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ItmsGrpNam", "Name")
		});
		this.luk_ItemGroup.DisplayMember = "ItmsGrpNam";
		this.luk_ItemGroup.Name = "luk_ItemGroup";
		this.luk_ItemGroup.NullText = "";
		this.luk_ItemGroup.ShowFooter = false;
		this.luk_ItemGroup.ShowHeader = false;
		this.luk_ItemGroup.ValueMember = "ItmsGrpCod";
		this.gridColumnEx67.Caption = "Qty";
		this.gridColumnEx67.ColumnEdit = this.repositoryItemCalcEditEx8;
		this.gridColumnEx67.CurrencyDateField = null;
		this.gridColumnEx67.CurrencyField = null;
		this.gridColumnEx67.CurrencyFixed = null;
		this.gridColumnEx67.CurrencyFixedRate = 0.0;
		this.gridColumnEx67.CurrencyRateField = null;
		this.gridColumnEx67.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gridColumnEx67.FieldName = "Qty";
		this.gridColumnEx67.Name = "gridColumnEx67";
		this.gridColumnEx67.PrimaryKey = false;
		this.gridColumnEx67.UnitField = null;
		this.gridColumnEx67.Visible = true;
		this.gridColumnEx67.VisibleIndex = 3;
		this.gridColumnEx68.Caption = "Ref Price";
		this.gridColumnEx68.ColumnEdit = this.repositoryItemTextEditExPrice4;
		this.gridColumnEx68.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.MainCurrency;
		this.gridColumnEx68.CurrencyDateField = null;
		this.gridColumnEx68.CurrencyField = null;
		this.gridColumnEx68.CurrencyFixed = null;
		this.gridColumnEx68.CurrencyFixedRate = 0.0;
		this.gridColumnEx68.CurrencyRateField = null;
		this.gridColumnEx68.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gridColumnEx68.FieldName = "RefPrice";
		this.gridColumnEx68.Name = "gridColumnEx68";
		this.gridColumnEx68.PrimaryKey = false;
		this.gridColumnEx68.UnitField = null;
		this.gridColumnEx68.Visible = true;
		this.gridColumnEx68.VisibleIndex = 4;
		this.gridColumnEx69.Caption = "Revenue Values";
		this.gridColumnEx69.ColumnEdit = this.repositoryItemTextEditExPrice5;
		this.gridColumnEx69.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.MainCurrency;
		this.gridColumnEx69.CurrencyDateField = null;
		this.gridColumnEx69.CurrencyField = null;
		this.gridColumnEx69.CurrencyFixed = null;
		this.gridColumnEx69.CurrencyFixedRate = 0.0;
		this.gridColumnEx69.CurrencyRateField = null;
		this.gridColumnEx69.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gridColumnEx69.FieldName = "RevenueValues";
		this.gridColumnEx69.Name = "gridColumnEx69";
		this.gridColumnEx69.PrimaryKey = false;
		this.gridColumnEx69.UnitField = null;
		this.gridColumnEx69.Visible = true;
		this.gridColumnEx69.VisibleIndex = 5;
		this.gridColumnEx69.Width = 119;
		this.gridColumnEx101.Caption = "UoM";
		this.gridColumnEx101.CurrencyDateField = null;
		this.gridColumnEx101.CurrencyField = null;
		this.gridColumnEx101.CurrencyFixed = null;
		this.gridColumnEx101.CurrencyFixedRate = 0.0;
		this.gridColumnEx101.CurrencyRateField = null;
		this.gridColumnEx101.FieldName = "UoM";
		this.gridColumnEx101.Name = "gridColumnEx101";
		this.gridColumnEx101.OptionsColumn.ReadOnly = true;
		this.gridColumnEx101.PrimaryKey = false;
		this.gridColumnEx101.UnitField = null;
		this.gridColumnEx101.Visible = true;
		this.gridColumnEx101.VisibleIndex = 6;
		this.xtra_Partner.Controls.Add(this.grd_Partner);
		this.xtra_Partner.EditControl = null;
		this.xtra_Partner.EditValue = "Partners";
		this.xtra_Partner.FieldName = null;
		this.xtra_Partner.IsSearchControl = false;
		this.xtra_Partner.Name = "xtra_Partner";
		this.xtra_Partner.ReadOnly = false;
		this.xtra_Partner.SettingInVisible = false;
		this.xtra_Partner.SettingReadonly = false;
		this.xtra_Partner.Size = new System.Drawing.Size(971, 217);
		this.xtra_Partner.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtra_Partner.Text = "Partners";
		this.grd_Partner.AllowPasteExcel = true;
		this.grd_Partner.ClipboardData = null;
		this.grd_Partner.ContextMenuStrip = this.ctm_Partner;
		this.grd_Partner.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Partner.Location = new System.Drawing.Point(0, 0);
		this.grd_Partner.MainView = this.gv_Partner;
		this.grd_Partner.Name = "grd_Partner";
		this.grd_Partner.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.luk_PartnerId, this.luk_Orl, this.btn_RelaPartner });
		this.grd_Partner.Size = new System.Drawing.Size(971, 217);
		this.grd_Partner.TabIndex = 19;
		this.grd_Partner.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Partner });
		this.ctm_Partner.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addRow_Partner, this.deleteRow_Partner });
		this.ctm_Partner.Name = "ctm_Partner";
		this.ctm_Partner.Size = new System.Drawing.Size(134, 48);
		this.addRow_Partner.Name = "addRow_Partner";
		this.addRow_Partner.Size = new System.Drawing.Size(133, 22);
		this.addRow_Partner.Text = "Add Row";
		this.addRow_Partner.Click += new System.EventHandler(this.addRow_Partner_Click);
		this.deleteRow_Partner.Name = "deleteRow_Partner";
		this.deleteRow_Partner.Size = new System.Drawing.Size(133, 22);
		this.deleteRow_Partner.Text = "Delete Row";
		this.deleteRow_Partner.Click += new System.EventHandler(this.deleteRow_Partner_Click);
		this.gv_Partner.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Partner.ColumnKey = null;
		this.gv_Partner.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[6] { this.gcPartnerName, this.gcPartnerRelationShip, this.gcPartnerRelatedBP, this.gcPartnerRemark, this.gcDocEntry, this.gcPartnerLine });
		this.gv_Partner.DisableFormSetting = false;
		this.gv_Partner.DisplayException = false;
		this.gv_Partner.EditColumn = null;
		this.gv_Partner.FieldCurrency = null;
		this.gv_Partner.FieldRate = null;
		this.gv_Partner.FormId = "oMarketingCampaign";
		this.gv_Partner.GridControl = this.grd_Partner;
		this.gv_Partner.IgnoreFilter = false;
		this.gv_Partner.IndexEditRow = -1;
		this.gv_Partner.IndicatorWidth = 35;
		this.gv_Partner.IsApzTable = false;
		this.gv_Partner.IsInsertNewRow = false;
		this.gv_Partner.IsLoadingData = false;
		this.gv_Partner.IsMappingContextAction = false;
		this.gv_Partner.ItemId = "APZ_SMC3";
		this.gv_Partner.LanguageSource = Apzon.Commons.Enums.Enums.LanguageSource.Documents;
		this.gv_Partner.LstColumnAllowBatch = null;
		this.gv_Partner.LstColumnNotAllowBatch = null;
		this.gv_Partner.LstColumnNotEditable = null;
		this.gv_Partner.LstColumNotSetting = null;
		this.gv_Partner.Name = "gv_Partner";
		this.gv_Partner.OptionsSelection.CheckBoxSelectorColumnWidth = 30;
		this.gv_Partner.OptionsSelection.MultiSelect = true;
		this.gv_Partner.OptionsView.ColumnAutoWidth = false;
		this.gv_Partner.OptionsView.ShowGroupPanel = false;
		this.gv_Partner.UseBatchMod = false;
		this.gv_Partner.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Partner_CellValueChanged);
		this.gcPartnerName.Caption = "Name";
		this.gcPartnerName.ColumnEdit = this.luk_PartnerId;
		this.gcPartnerName.CurrencyDateField = null;
		this.gcPartnerName.CurrencyField = null;
		this.gcPartnerName.CurrencyFixed = null;
		this.gcPartnerName.CurrencyFixedRate = 0.0;
		this.gcPartnerName.CurrencyRateField = null;
		this.gcPartnerName.FieldName = "ParterId";
		this.gcPartnerName.Name = "gcPartnerName";
		this.gcPartnerName.PrimaryKey = false;
		this.gcPartnerName.UnitField = null;
		this.gcPartnerName.Visible = true;
		this.gcPartnerName.VisibleIndex = 0;
		this.gcPartnerName.Width = 200;
		this.luk_PartnerId.AutoHeight = false;
		this.luk_PartnerId.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
		this.luk_PartnerId.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_PartnerId.CheckFields = null;
		this.luk_PartnerId.CheckTables = null;
		this.luk_PartnerId.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("PrtId", "Code"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.luk_PartnerId.DisplayMember = "Name";
		this.luk_PartnerId.Name = "luk_PartnerId";
		this.luk_PartnerId.NullText = "";
		this.luk_PartnerId.ShowFooter = false;
		this.luk_PartnerId.ShowHeader = false;
		this.luk_PartnerId.ValueMember = "PrtId";
		this.gcPartnerRelationShip.Caption = "Relation ship";
		this.gcPartnerRelationShip.ColumnEdit = this.luk_Orl;
		this.gcPartnerRelationShip.CurrencyDateField = null;
		this.gcPartnerRelationShip.CurrencyField = null;
		this.gcPartnerRelationShip.CurrencyFixed = null;
		this.gcPartnerRelationShip.CurrencyFixedRate = 0.0;
		this.gcPartnerRelationShip.CurrencyRateField = null;
		this.gcPartnerRelationShip.FieldName = "OrlCode";
		this.gcPartnerRelationShip.Name = "gcPartnerRelationShip";
		this.gcPartnerRelationShip.PrimaryKey = false;
		this.gcPartnerRelationShip.UnitField = null;
		this.gcPartnerRelationShip.Visible = true;
		this.gcPartnerRelationShip.VisibleIndex = 1;
		this.gcPartnerRelationShip.Width = 200;
		this.luk_Orl.AutoHeight = false;
		this.luk_Orl.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
		this.luk_Orl.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Orl.CheckFields = null;
		this.luk_Orl.CheckTables = null;
		this.luk_Orl.Name = "luk_Orl";
		this.luk_Orl.NullText = "";
		this.luk_Orl.ShowFooter = false;
		this.luk_Orl.ShowHeader = false;
		this.gcPartnerRelatedBP.Caption = "Related BP";
		this.gcPartnerRelatedBP.ColumnEdit = this.btn_RelaPartner;
		this.gcPartnerRelatedBP.CurrencyDateField = null;
		this.gcPartnerRelatedBP.CurrencyField = null;
		this.gcPartnerRelatedBP.CurrencyFixed = null;
		this.gcPartnerRelatedBP.CurrencyFixedRate = 0.0;
		this.gcPartnerRelatedBP.CurrencyRateField = null;
		this.gcPartnerRelatedBP.FieldName = "RelatCard";
		this.gcPartnerRelatedBP.Name = "gcPartnerRelatedBP";
		this.gcPartnerRelatedBP.PrimaryKey = false;
		this.gcPartnerRelatedBP.UnitField = null;
		this.gcPartnerRelatedBP.Visible = true;
		this.gcPartnerRelatedBP.VisibleIndex = 2;
		this.gcPartnerRelatedBP.Width = 200;
		this.btn_RelaPartner.AutoHeight = false;
		this.btn_RelaPartner.AutoLoadDataSource = true;
		editorButtonImageOptions8.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions15.Image");
		this.btn_RelaPartner.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions9, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance16, appearanceHovered16, appearancePressed16, appearanceDisabled16, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions8, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance17, appearanceHovered17, appearancePressed17, appearanceDisabled17, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_RelaPartner.ContainerControl = this;
		this.btn_RelaPartner.DisplayMember = "CardCode";
		this.btn_RelaPartner.DisplayMember2 = "";
		this.btn_RelaPartner.LoadDataSourceWhenEnter = false;
		this.btn_RelaPartner.Name = "btn_RelaPartner";
		this.btn_RelaPartner.SeparateCharacterForMulti = ';';
		this.btn_RelaPartner.SeparateString = ", ";
		this.btn_RelaPartner.UsingAutoCompleteBox = false;
		this.btn_RelaPartner.UsingDataSource = true;
		this.btn_RelaPartner.ValidateWhenNoDataSource = false;
		this.btn_RelaPartner.ValueMember = "CardCode";
		this.gcPartnerRemark.Caption = "Remark";
		this.gcPartnerRemark.CurrencyDateField = null;
		this.gcPartnerRemark.CurrencyField = null;
		this.gcPartnerRemark.CurrencyFixed = null;
		this.gcPartnerRemark.CurrencyFixedRate = 0.0;
		this.gcPartnerRemark.CurrencyRateField = null;
		this.gcPartnerRemark.FieldName = "Memo";
		this.gcPartnerRemark.Name = "gcPartnerRemark";
		this.gcPartnerRemark.PrimaryKey = false;
		this.gcPartnerRemark.UnitField = null;
		this.gcPartnerRemark.Visible = true;
		this.gcPartnerRemark.VisibleIndex = 3;
		this.gcPartnerRemark.Width = 200;
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
		this.gcPartnerLine.Caption = "LineNum";
		this.gcPartnerLine.CurrencyDateField = null;
		this.gcPartnerLine.CurrencyField = null;
		this.gcPartnerLine.CurrencyFixed = null;
		this.gcPartnerLine.CurrencyFixedRate = 0.0;
		this.gcPartnerLine.CurrencyRateField = null;
		this.gcPartnerLine.FieldName = "LineNum";
		this.gcPartnerLine.Name = "gcPartnerLine";
		this.gcPartnerLine.PrimaryKey = false;
		this.gcPartnerLine.UnitField = null;
		this.xtra_Member.Controls.Add(this.grd_Member);
		this.xtra_Member.EditControl = null;
		this.xtra_Member.EditValue = "Campaign Members";
		this.xtra_Member.FieldName = null;
		this.xtra_Member.IsSearchControl = false;
		this.xtra_Member.Name = "xtra_Member";
		this.xtra_Member.ReadOnly = false;
		this.xtra_Member.SettingInVisible = false;
		this.xtra_Member.SettingReadonly = false;
		this.xtra_Member.Size = new System.Drawing.Size(971, 217);
		this.xtra_Member.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtra_Member.Text = "Campaign Members";
		this.grd_Member.AllowPasteExcel = true;
		this.grd_Member.ClipboardData = null;
		this.grd_Member.ContextMenuStrip = this.ctm_Member;
		this.grd_Member.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Member.Location = new System.Drawing.Point(0, 0);
		this.grd_Member.MainView = this.gv_Member;
		this.grd_Member.Name = "grd_Member";
		this.grd_Member.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[4] { this.btn_EmployeeNo, this.luk_BranchMember, this.luk_DepartmentMember, this.luk_Role });
		this.grd_Member.Size = new System.Drawing.Size(971, 217);
		this.grd_Member.TabIndex = 0;
		this.grd_Member.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Member });
		this.ctm_Member.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addRow_member, this.deleteRow_member });
		this.ctm_Member.Name = "ctm_Member";
		this.ctm_Member.Size = new System.Drawing.Size(134, 48);
		this.addRow_member.Name = "addRow_member";
		this.addRow_member.Size = new System.Drawing.Size(133, 22);
		this.addRow_member.Text = "Add Row";
		this.addRow_member.Click += new System.EventHandler(this.addRow_member_Click);
		this.deleteRow_member.Name = "deleteRow_member";
		this.deleteRow_member.Size = new System.Drawing.Size(133, 22);
		this.deleteRow_member.Text = "Delete Row";
		this.deleteRow_member.Click += new System.EventHandler(this.deleteRow_member_Click);
		this.gv_Member.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Member.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Member.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Member.ColumnKey = null;
		this.gv_Member.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[10] { this.gridColumnEx34, this.gridColumnEx35, this.gridColumnEx36, this.gridColumnEx37, this.gridColumnEx38, this.gridColumnEx39, this.gridColumnEx40, this.gridColumnEx41, this.gridColumnEx42, this.gridColumnEx43 });
		this.gv_Member.DisableFormSetting = false;
		this.gv_Member.DisplayException = false;
		this.gv_Member.EditColumn = null;
		this.gv_Member.FieldCurrency = null;
		this.gv_Member.FieldRate = null;
		this.gv_Member.FormId = "oMarketingCampaign";
		this.gv_Member.GridControl = this.grd_Member;
		this.gv_Member.IgnoreFilter = false;
		this.gv_Member.IndexEditRow = -1;
		this.gv_Member.IndicatorWidth = 35;
		this.gv_Member.IsApzTable = false;
		this.gv_Member.IsInsertNewRow = false;
		this.gv_Member.IsLoadingData = false;
		this.gv_Member.IsMappingContextAction = false;
		this.gv_Member.ItemId = "APZ_SMC4";
		this.gv_Member.LstColumnAllowBatch = null;
		this.gv_Member.LstColumnNotAllowBatch = null;
		this.gv_Member.LstColumnNotEditable = null;
		this.gv_Member.LstColumNotSetting = null;
		this.gv_Member.Name = "gv_Member";
		this.gv_Member.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Member.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Member.OptionsView.ColumnAutoWidth = false;
		this.gv_Member.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Member.OptionsView.ShowGroupPanel = false;
		this.gv_Member.UseBatchMod = false;
		this.gv_Member.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Member_CellValueChanged);
		this.gridColumnEx34.Caption = "DocEntry";
		this.gridColumnEx34.CurrencyDateField = null;
		this.gridColumnEx34.CurrencyField = null;
		this.gridColumnEx34.CurrencyFixed = null;
		this.gridColumnEx34.CurrencyFixedRate = 0.0;
		this.gridColumnEx34.CurrencyRateField = null;
		this.gridColumnEx34.FieldName = "DocEntry";
		this.gridColumnEx34.Name = "gridColumnEx34";
		this.gridColumnEx34.OptionsColumn.AllowEdit = false;
		this.gridColumnEx34.OptionsColumn.ReadOnly = true;
		this.gridColumnEx34.PrimaryKey = false;
		this.gridColumnEx34.UnitField = null;
		this.gridColumnEx35.Caption = "LineNum";
		this.gridColumnEx35.CurrencyDateField = null;
		this.gridColumnEx35.CurrencyField = null;
		this.gridColumnEx35.CurrencyFixed = null;
		this.gridColumnEx35.CurrencyFixedRate = 0.0;
		this.gridColumnEx35.CurrencyRateField = null;
		this.gridColumnEx35.FieldName = "LineNum";
		this.gridColumnEx35.Name = "gridColumnEx35";
		this.gridColumnEx35.OptionsColumn.ReadOnly = true;
		this.gridColumnEx35.PrimaryKey = false;
		this.gridColumnEx35.UnitField = null;
		this.gridColumnEx36.Caption = "Employee No";
		this.gridColumnEx36.ColumnEdit = this.btn_EmployeeNo;
		this.gridColumnEx36.CurrencyDateField = null;
		this.gridColumnEx36.CurrencyField = null;
		this.gridColumnEx36.CurrencyFixed = null;
		this.gridColumnEx36.CurrencyFixedRate = 0.0;
		this.gridColumnEx36.CurrencyRateField = null;
		this.gridColumnEx36.FieldName = "EmployeeNo";
		this.gridColumnEx36.Name = "gridColumnEx36";
		this.gridColumnEx36.PrimaryKey = true;
		this.gridColumnEx36.UnitField = null;
		this.gridColumnEx36.Visible = true;
		this.gridColumnEx36.VisibleIndex = 0;
		this.gridColumnEx36.Width = 118;
		this.btn_EmployeeNo.AutoHeight = false;
		this.btn_EmployeeNo.AutoLoadDataSource = true;
		editorButtonImageOptions9.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions17.Image");
		this.btn_EmployeeNo.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions10, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance18, appearanceHovered18, appearancePressed18, appearanceDisabled18, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions9, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance19, appearanceHovered19, appearancePressed19, appearanceDisabled19, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_EmployeeNo.ContainerControl = this;
		this.btn_EmployeeNo.DisplayMember = "empID";
		this.btn_EmployeeNo.DisplayMember2 = "";
		this.btn_EmployeeNo.LoadDataSourceWhenEnter = false;
		this.btn_EmployeeNo.Name = "btn_EmployeeNo";
		this.btn_EmployeeNo.SeparateCharacterForMulti = ';';
		this.btn_EmployeeNo.SeparateString = ", ";
		this.btn_EmployeeNo.UsingAutoCompleteBox = false;
		this.btn_EmployeeNo.UsingDataSource = true;
		this.btn_EmployeeNo.ValidateWhenNoDataSource = false;
		this.btn_EmployeeNo.ValueMember = "empID";
		this.gridColumnEx37.Caption = "Employee Name";
		this.gridColumnEx37.CurrencyDateField = null;
		this.gridColumnEx37.CurrencyField = null;
		this.gridColumnEx37.CurrencyFixed = null;
		this.gridColumnEx37.CurrencyFixedRate = 0.0;
		this.gridColumnEx37.CurrencyRateField = null;
		this.gridColumnEx37.FieldName = "EmployeeName";
		this.gridColumnEx37.Name = "gridColumnEx37";
		this.gridColumnEx37.PrimaryKey = false;
		this.gridColumnEx37.UnitField = null;
		this.gridColumnEx37.Visible = true;
		this.gridColumnEx37.VisibleIndex = 1;
		this.gridColumnEx37.Width = 132;
		this.gridColumnEx38.Caption = "Branch";
		this.gridColumnEx38.ColumnEdit = this.luk_BranchMember;
		this.gridColumnEx38.CurrencyDateField = null;
		this.gridColumnEx38.CurrencyField = null;
		this.gridColumnEx38.CurrencyFixed = null;
		this.gridColumnEx38.CurrencyFixedRate = 0.0;
		this.gridColumnEx38.CurrencyRateField = null;
		this.gridColumnEx38.FieldName = "Branch";
		this.gridColumnEx38.Name = "gridColumnEx38";
		this.gridColumnEx38.PrimaryKey = false;
		this.gridColumnEx38.UnitField = null;
		this.gridColumnEx38.Visible = true;
		this.gridColumnEx38.VisibleIndex = 2;
		this.luk_BranchMember.AutoHeight = false;
		this.luk_BranchMember.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
		this.luk_BranchMember.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_BranchMember.CheckFields = null;
		this.luk_BranchMember.CheckTables = null;
		this.luk_BranchMember.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.luk_BranchMember.DisplayMember = "Name";
		this.luk_BranchMember.Name = "luk_BranchMember";
		this.luk_BranchMember.NullText = "";
		this.luk_BranchMember.ShowFooter = false;
		this.luk_BranchMember.ShowHeader = false;
		this.luk_BranchMember.ValueMember = "Code";
		this.gridColumnEx39.Caption = "Department";
		this.gridColumnEx39.ColumnEdit = this.luk_DepartmentMember;
		this.gridColumnEx39.CurrencyDateField = null;
		this.gridColumnEx39.CurrencyField = null;
		this.gridColumnEx39.CurrencyFixed = null;
		this.gridColumnEx39.CurrencyFixedRate = 0.0;
		this.gridColumnEx39.CurrencyRateField = null;
		this.gridColumnEx39.FieldName = "Department";
		this.gridColumnEx39.Name = "gridColumnEx39";
		this.gridColumnEx39.PrimaryKey = false;
		this.gridColumnEx39.UnitField = null;
		this.gridColumnEx39.Visible = true;
		this.gridColumnEx39.VisibleIndex = 3;
		this.gridColumnEx39.Width = 100;
		this.luk_DepartmentMember.AutoHeight = false;
		this.luk_DepartmentMember.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
		this.luk_DepartmentMember.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_DepartmentMember.CheckFields = null;
		this.luk_DepartmentMember.CheckTables = null;
		this.luk_DepartmentMember.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.luk_DepartmentMember.DisplayMember = "Name";
		this.luk_DepartmentMember.Name = "luk_DepartmentMember";
		this.luk_DepartmentMember.NullText = "";
		this.luk_DepartmentMember.ShowFooter = false;
		this.luk_DepartmentMember.ShowHeader = false;
		this.luk_DepartmentMember.ValueMember = "Code";
		this.gridColumnEx40.Caption = "Email";
		this.gridColumnEx40.CurrencyDateField = null;
		this.gridColumnEx40.CurrencyField = null;
		this.gridColumnEx40.CurrencyFixed = null;
		this.gridColumnEx40.CurrencyFixedRate = 0.0;
		this.gridColumnEx40.CurrencyRateField = null;
		this.gridColumnEx40.FieldName = "Email";
		this.gridColumnEx40.Name = "gridColumnEx40";
		this.gridColumnEx40.PrimaryKey = false;
		this.gridColumnEx40.UnitField = null;
		this.gridColumnEx40.Visible = true;
		this.gridColumnEx40.VisibleIndex = 4;
		this.gridColumnEx41.Caption = "Mobile Number";
		this.gridColumnEx41.CurrencyDateField = null;
		this.gridColumnEx41.CurrencyField = null;
		this.gridColumnEx41.CurrencyFixed = null;
		this.gridColumnEx41.CurrencyFixedRate = 0.0;
		this.gridColumnEx41.CurrencyRateField = null;
		this.gridColumnEx41.FieldName = "MobileNumber";
		this.gridColumnEx41.Name = "gridColumnEx41";
		this.gridColumnEx41.PrimaryKey = false;
		this.gridColumnEx41.UnitField = null;
		this.gridColumnEx41.Visible = true;
		this.gridColumnEx41.VisibleIndex = 5;
		this.gridColumnEx41.Width = 125;
		this.gridColumnEx42.Caption = "Member Role";
		this.gridColumnEx42.ColumnEdit = this.luk_Role;
		this.gridColumnEx42.CurrencyDateField = null;
		this.gridColumnEx42.CurrencyField = null;
		this.gridColumnEx42.CurrencyFixed = null;
		this.gridColumnEx42.CurrencyFixedRate = 0.0;
		this.gridColumnEx42.CurrencyRateField = null;
		this.gridColumnEx42.FieldName = "Role";
		this.gridColumnEx42.Name = "gridColumnEx42";
		this.gridColumnEx42.PrimaryKey = false;
		this.gridColumnEx42.UnitField = null;
		this.gridColumnEx42.Visible = true;
		this.gridColumnEx42.VisibleIndex = 6;
		this.luk_Role.AutoHeight = false;
		this.luk_Role.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
		this.luk_Role.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Role.CheckFields = null;
		this.luk_Role.CheckTables = null;
		this.luk_Role.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("typeID", "Code"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("name", "Name")
		});
		this.luk_Role.DisplayMember = "name";
		this.luk_Role.Name = "luk_Role";
		this.luk_Role.NullText = "";
		this.luk_Role.ShowFooter = false;
		this.luk_Role.ShowHeader = false;
		this.luk_Role.ValueMember = "typeID";
		this.gridColumnEx43.Caption = "Remark";
		this.gridColumnEx43.CurrencyDateField = null;
		this.gridColumnEx43.CurrencyField = null;
		this.gridColumnEx43.CurrencyFixed = null;
		this.gridColumnEx43.CurrencyFixedRate = 0.0;
		this.gridColumnEx43.CurrencyRateField = null;
		this.gridColumnEx43.FieldName = "Remark";
		this.gridColumnEx43.Name = "gridColumnEx43";
		this.gridColumnEx43.PrimaryKey = false;
		this.gridColumnEx43.UnitField = null;
		this.gridColumnEx43.Visible = true;
		this.gridColumnEx43.VisibleIndex = 7;
		this.xtra_Atta.Controls.Add(this.ucAttachment1);
		this.xtra_Atta.EditControl = null;
		this.xtra_Atta.EditValue = "Attached Files";
		this.xtra_Atta.FieldName = null;
		this.xtra_Atta.IsSearchControl = false;
		this.xtra_Atta.Name = "xtra_Atta";
		this.xtra_Atta.ReadOnly = false;
		this.xtra_Atta.SettingInVisible = false;
		this.xtra_Atta.SettingReadonly = false;
		this.xtra_Atta.Size = new System.Drawing.Size(971, 217);
		this.xtra_Atta.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtra_Atta.Text = "Attached Files";
		this.ucAttachment1.DataSource = null;
		this.ucAttachment1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.ucAttachment1.FormId = "oMarketingCampaign";
		this.ucAttachment1.ItemId = "ATC1";
		this.ucAttachment1.Location = new System.Drawing.Point(0, 0);
		this.ucAttachment1.Name = "ucAttachment1";
		this.ucAttachment1.Size = new System.Drawing.Size(971, 217);
		this.ucAttachment1.TabIndex = 0;
		this.ucAttachment1.ButtonBrowse_Click += new System.EventHandler(this.ucAttachment1_ButtonDelete_Click);
		this.ucAttachment1.ButtonDelete_Click += new System.EventHandler(this.ucAttachment1_ButtonDelete_Click);
		this.xtra_Cost.Controls.Add(this.grd_Cost);
		this.xtra_Cost.EditControl = null;
		this.xtra_Cost.EditValue = "Campaign Cost";
		this.xtra_Cost.FieldName = null;
		this.xtra_Cost.IsSearchControl = false;
		this.xtra_Cost.Name = "xtra_Cost";
		this.xtra_Cost.ReadOnly = false;
		this.xtra_Cost.SettingInVisible = false;
		this.xtra_Cost.SettingReadonly = false;
		this.xtra_Cost.Size = new System.Drawing.Size(971, 217);
		this.xtra_Cost.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtra_Cost.Text = "Campaign Cost";
		this.grd_Cost.AllowPasteExcel = true;
		this.grd_Cost.ClipboardData = null;
		this.grd_Cost.ContextMenuStrip = this.ctm_Cost;
		this.grd_Cost.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Cost.Location = new System.Drawing.Point(0, 0);
		this.grd_Cost.MainView = this.gv_Cost;
		this.grd_Cost.Name = "grd_Cost";
		this.grd_Cost.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.btn_ItemCost, this.luk_ItmGrpCost });
		this.grd_Cost.Size = new System.Drawing.Size(971, 217);
		this.grd_Cost.TabIndex = 0;
		this.grd_Cost.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Cost });
		this.ctm_Cost.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addRow_Cost, this.deleteRow_Cost });
		this.ctm_Cost.Name = "ctm_Cost";
		this.ctm_Cost.Size = new System.Drawing.Size(134, 48);
		this.addRow_Cost.Name = "addRow_Cost";
		this.addRow_Cost.Size = new System.Drawing.Size(133, 22);
		this.addRow_Cost.Text = "Add Row";
		this.addRow_Cost.Click += new System.EventHandler(this.addRow_Cost_Click);
		this.deleteRow_Cost.Name = "deleteRow_Cost";
		this.deleteRow_Cost.Size = new System.Drawing.Size(133, 22);
		this.deleteRow_Cost.Text = "Delete Row";
		this.deleteRow_Cost.Click += new System.EventHandler(this.deleteRow_Cost_Click);
		this.gv_Cost.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Cost.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Cost.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Cost.ColumnKey = null;
		this.gv_Cost.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[13]
		{
			this.gridColumnEx44, this.gridColumnEx45, this.gridColumnEx46, this.gridColumnEx47, this.gridColumnEx48, this.gridColumnEx49, this.gridColumnEx50, this.gridColumnEx51, this.gridColumnEx52, this.gridColumnEx53,
			this.gridColumnEx54, this.gridColumnEx55, this.gridColumnEx90
		});
		this.gv_Cost.DisableFormSetting = false;
		this.gv_Cost.DisplayException = false;
		this.gv_Cost.EditColumn = null;
		this.gv_Cost.FieldCurrency = null;
		this.gv_Cost.FieldRate = null;
		this.gv_Cost.FormId = "oMarketingCampaign";
		this.gv_Cost.GridControl = this.grd_Cost;
		this.gv_Cost.IgnoreFilter = false;
		this.gv_Cost.IndexEditRow = -1;
		this.gv_Cost.IndicatorWidth = 35;
		this.gv_Cost.IsApzTable = false;
		this.gv_Cost.IsInsertNewRow = false;
		this.gv_Cost.IsLoadingData = false;
		this.gv_Cost.IsMappingContextAction = false;
		this.gv_Cost.ItemId = "APZ_SMC5";
		this.gv_Cost.LstColumnAllowBatch = null;
		this.gv_Cost.LstColumnNotAllowBatch = null;
		this.gv_Cost.LstColumnNotEditable = null;
		this.gv_Cost.LstColumNotSetting = null;
		this.gv_Cost.Name = "gv_Cost";
		this.gv_Cost.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Cost.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Cost.OptionsView.ColumnAutoWidth = false;
		this.gv_Cost.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Cost.OptionsView.ShowGroupPanel = false;
		this.gv_Cost.UseBatchMod = false;
		this.gv_Cost.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Cost_RowCellStyle);
		this.gv_Cost.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Cost_CellValueChanged);
		this.gridColumnEx44.Caption = "DocEntry";
		this.gridColumnEx44.CurrencyDateField = null;
		this.gridColumnEx44.CurrencyField = null;
		this.gridColumnEx44.CurrencyFixed = null;
		this.gridColumnEx44.CurrencyFixedRate = 0.0;
		this.gridColumnEx44.CurrencyRateField = null;
		this.gridColumnEx44.FieldName = "DocEntry";
		this.gridColumnEx44.Name = "gridColumnEx44";
		this.gridColumnEx44.OptionsColumn.AllowEdit = false;
		this.gridColumnEx44.OptionsColumn.ReadOnly = true;
		this.gridColumnEx44.PrimaryKey = false;
		this.gridColumnEx44.UnitField = null;
		this.gridColumnEx45.Caption = "LineNum";
		this.gridColumnEx45.CurrencyDateField = null;
		this.gridColumnEx45.CurrencyField = null;
		this.gridColumnEx45.CurrencyFixed = null;
		this.gridColumnEx45.CurrencyFixedRate = 0.0;
		this.gridColumnEx45.CurrencyRateField = null;
		this.gridColumnEx45.FieldName = "LineNum";
		this.gridColumnEx45.Name = "gridColumnEx45";
		this.gridColumnEx45.OptionsColumn.ReadOnly = true;
		this.gridColumnEx45.PrimaryKey = false;
		this.gridColumnEx45.UnitField = null;
		this.gridColumnEx46.Caption = "Item Code";
		this.gridColumnEx46.ColumnEdit = this.btn_ItemCost;
		this.gridColumnEx46.CurrencyDateField = null;
		this.gridColumnEx46.CurrencyField = null;
		this.gridColumnEx46.CurrencyFixed = null;
		this.gridColumnEx46.CurrencyFixedRate = 0.0;
		this.gridColumnEx46.CurrencyRateField = null;
		this.gridColumnEx46.FieldName = "ItemCode";
		this.gridColumnEx46.Name = "gridColumnEx46";
		this.gridColumnEx46.PrimaryKey = true;
		this.gridColumnEx46.UnitField = null;
		this.gridColumnEx46.Visible = true;
		this.gridColumnEx46.VisibleIndex = 0;
		this.gridColumnEx46.Width = 111;
		this.btn_ItemCost.AutoHeight = false;
		this.btn_ItemCost.AutoLoadDataSource = true;
		editorButtonImageOptions10.Image = Apzon.Forms.Properties.Resources.right;
		this.btn_ItemCost.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions11, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance20, appearanceHovered20, appearancePressed20, appearanceDisabled20, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions10, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance21, appearanceHovered21, appearancePressed21, appearanceDisabled21, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_ItemCost.ContainerControl = this;
		this.btn_ItemCost.DisplayMember = "ItemCode";
		this.btn_ItemCost.DisplayMember2 = "";
		this.btn_ItemCost.LoadDataSourceWhenEnter = false;
		this.btn_ItemCost.Name = "btn_ItemCost";
		this.btn_ItemCost.SeparateCharacterForMulti = ';';
		this.btn_ItemCost.SeparateString = ", ";
		this.btn_ItemCost.UsingAutoCompleteBox = false;
		this.btn_ItemCost.UsingDataSource = true;
		this.btn_ItemCost.ValidateWhenNoDataSource = false;
		this.btn_ItemCost.ValueMember = "ItemCode";
		this.gridColumnEx47.Caption = "Item Name";
		this.gridColumnEx47.CurrencyDateField = null;
		this.gridColumnEx47.CurrencyField = null;
		this.gridColumnEx47.CurrencyFixed = null;
		this.gridColumnEx47.CurrencyFixedRate = 0.0;
		this.gridColumnEx47.CurrencyRateField = null;
		this.gridColumnEx47.FieldName = "ItemName";
		this.gridColumnEx47.Name = "gridColumnEx47";
		this.gridColumnEx47.PrimaryKey = false;
		this.gridColumnEx47.UnitField = null;
		this.gridColumnEx47.Visible = true;
		this.gridColumnEx47.VisibleIndex = 1;
		this.gridColumnEx47.Width = 153;
		this.gridColumnEx48.Caption = "Qty";
		this.gridColumnEx48.ColumnEdit = this.repositoryItemCalcEditEx4;
		this.gridColumnEx48.CurrencyDateField = null;
		this.gridColumnEx48.CurrencyField = null;
		this.gridColumnEx48.CurrencyFixed = null;
		this.gridColumnEx48.CurrencyFixedRate = 0.0;
		this.gridColumnEx48.CurrencyRateField = null;
		this.gridColumnEx48.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gridColumnEx48.FieldName = "Qty";
		this.gridColumnEx48.Name = "gridColumnEx48";
		this.gridColumnEx48.PrimaryKey = false;
		this.gridColumnEx48.UnitField = null;
		this.gridColumnEx48.Visible = true;
		this.gridColumnEx48.VisibleIndex = 3;
		this.gridColumnEx49.Caption = "Ref Price";
		this.gridColumnEx49.ColumnEdit = this.repositoryItemTextEditExPrice1;
		this.gridColumnEx49.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.MainCurrency;
		this.gridColumnEx49.CurrencyDateField = null;
		this.gridColumnEx49.CurrencyField = null;
		this.gridColumnEx49.CurrencyFixed = null;
		this.gridColumnEx49.CurrencyFixedRate = 0.0;
		this.gridColumnEx49.CurrencyRateField = null;
		this.gridColumnEx49.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gridColumnEx49.FieldName = "RefPrice";
		this.gridColumnEx49.Name = "gridColumnEx49";
		this.gridColumnEx49.PrimaryKey = false;
		this.gridColumnEx49.UnitField = null;
		this.gridColumnEx49.Visible = true;
		this.gridColumnEx49.VisibleIndex = 4;
		this.gridColumnEx50.Caption = "Budget Value";
		this.gridColumnEx50.ColumnEdit = this.repositoryItemCalcEditEx5;
		this.gridColumnEx50.CurrencyDateField = null;
		this.gridColumnEx50.CurrencyField = null;
		this.gridColumnEx50.CurrencyFixed = null;
		this.gridColumnEx50.CurrencyFixedRate = 0.0;
		this.gridColumnEx50.CurrencyRateField = null;
		this.gridColumnEx50.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gridColumnEx50.FieldName = "BudgetValue";
		this.gridColumnEx50.Name = "gridColumnEx50";
		this.gridColumnEx50.PrimaryKey = false;
		this.gridColumnEx50.UnitField = null;
		this.gridColumnEx50.Visible = true;
		this.gridColumnEx50.VisibleIndex = 5;
		this.gridColumnEx50.Width = 113;
		this.gridColumnEx51.Caption = "Actual Qty";
		this.gridColumnEx51.ColumnEdit = this.repositoryItemCalcEditEx6;
		this.gridColumnEx51.CurrencyDateField = null;
		this.gridColumnEx51.CurrencyField = null;
		this.gridColumnEx51.CurrencyFixed = null;
		this.gridColumnEx51.CurrencyFixedRate = 0.0;
		this.gridColumnEx51.CurrencyRateField = null;
		this.gridColumnEx51.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gridColumnEx51.FieldName = "ActualQty";
		this.gridColumnEx51.Name = "gridColumnEx51";
		this.gridColumnEx51.PrimaryKey = false;
		this.gridColumnEx51.UnitField = null;
		this.gridColumnEx51.Visible = true;
		this.gridColumnEx51.VisibleIndex = 6;
		this.gridColumnEx51.Width = 126;
		this.gridColumnEx52.Caption = "Actual Price";
		this.gridColumnEx52.ColumnEdit = this.repositoryItemTextEditExPrice2;
		this.gridColumnEx52.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.MainCurrency;
		this.gridColumnEx52.CurrencyDateField = null;
		this.gridColumnEx52.CurrencyField = null;
		this.gridColumnEx52.CurrencyFixed = null;
		this.gridColumnEx52.CurrencyFixedRate = 0.0;
		this.gridColumnEx52.CurrencyRateField = null;
		this.gridColumnEx52.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gridColumnEx52.FieldName = "ActualPrice";
		this.gridColumnEx52.Name = "gridColumnEx52";
		this.gridColumnEx52.PrimaryKey = false;
		this.gridColumnEx52.UnitField = null;
		this.gridColumnEx52.Visible = true;
		this.gridColumnEx52.VisibleIndex = 7;
		this.gridColumnEx52.Width = 119;
		this.gridColumnEx53.Caption = "Actual Cost";
		this.gridColumnEx53.ColumnEdit = this.repositoryItemTextEditExPrice3;
		this.gridColumnEx53.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.MainCurrency;
		this.gridColumnEx53.CurrencyDateField = null;
		this.gridColumnEx53.CurrencyField = null;
		this.gridColumnEx53.CurrencyFixed = null;
		this.gridColumnEx53.CurrencyFixedRate = 0.0;
		this.gridColumnEx53.CurrencyRateField = null;
		this.gridColumnEx53.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gridColumnEx53.FieldName = "ActualCost";
		this.gridColumnEx53.Name = "gridColumnEx53";
		this.gridColumnEx53.PrimaryKey = false;
		this.gridColumnEx53.UnitField = null;
		this.gridColumnEx53.Visible = true;
		this.gridColumnEx53.VisibleIndex = 8;
		this.gridColumnEx53.Width = 101;
		this.gridColumnEx54.Caption = "Diff %";
		this.gridColumnEx54.ColumnEdit = this.repositoryItemCalcEditEx7;
		this.gridColumnEx54.CurrencyDateField = null;
		this.gridColumnEx54.CurrencyField = null;
		this.gridColumnEx54.CurrencyFixed = null;
		this.gridColumnEx54.CurrencyFixedRate = 0.0;
		this.gridColumnEx54.CurrencyRateField = null;
		this.gridColumnEx54.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Percent;
		this.gridColumnEx54.FieldName = "Diff";
		this.gridColumnEx54.Name = "gridColumnEx54";
		this.gridColumnEx54.PrimaryKey = false;
		this.gridColumnEx54.UnitField = null;
		this.gridColumnEx54.Visible = true;
		this.gridColumnEx54.VisibleIndex = 9;
		this.gridColumnEx55.Caption = "Remark";
		this.gridColumnEx55.CurrencyDateField = null;
		this.gridColumnEx55.CurrencyField = null;
		this.gridColumnEx55.CurrencyFixed = null;
		this.gridColumnEx55.CurrencyFixedRate = 0.0;
		this.gridColumnEx55.CurrencyRateField = null;
		this.gridColumnEx55.FieldName = "Remark";
		this.gridColumnEx55.Name = "gridColumnEx55";
		this.gridColumnEx55.PrimaryKey = false;
		this.gridColumnEx55.UnitField = null;
		this.gridColumnEx55.Visible = true;
		this.gridColumnEx55.VisibleIndex = 10;
		this.gridColumnEx90.Caption = "Item Group";
		this.gridColumnEx90.ColumnEdit = this.luk_ItmGrpCost;
		this.gridColumnEx90.CurrencyDateField = null;
		this.gridColumnEx90.CurrencyField = null;
		this.gridColumnEx90.CurrencyFixed = null;
		this.gridColumnEx90.CurrencyFixedRate = 0.0;
		this.gridColumnEx90.CurrencyRateField = null;
		this.gridColumnEx90.FieldName = "ItemGroup";
		this.gridColumnEx90.Name = "gridColumnEx90";
		this.gridColumnEx90.OptionsColumn.ReadOnly = true;
		this.gridColumnEx90.PrimaryKey = false;
		this.gridColumnEx90.UnitField = null;
		this.gridColumnEx90.Visible = true;
		this.gridColumnEx90.VisibleIndex = 2;
		this.gridColumnEx90.Width = 105;
		this.luk_ItmGrpCost.AutoHeight = false;
		this.luk_ItmGrpCost.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
		this.luk_ItmGrpCost.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_ItmGrpCost.CheckFields = null;
		this.luk_ItmGrpCost.CheckTables = null;
		this.luk_ItmGrpCost.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ItmsGrpCod", "Code"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ItmsGrpNam", "Name")
		});
		this.luk_ItmGrpCost.DisplayMember = "ItmsGrpNam";
		this.luk_ItmGrpCost.Name = "luk_ItmGrpCost";
		this.luk_ItmGrpCost.NullText = "";
		this.luk_ItmGrpCost.ShowFooter = false;
		this.luk_ItmGrpCost.ShowHeader = false;
		this.luk_ItmGrpCost.ValueMember = "ItmsGrpCod";
		this.xtra_Task.Controls.Add(this.grd_ListCompleted);
		this.xtra_Task.EditControl = null;
		this.xtra_Task.EditValue = "Related Tasks";
		this.xtra_Task.FieldName = null;
		this.xtra_Task.IsSearchControl = false;
		this.xtra_Task.Name = "xtra_Task";
		this.xtra_Task.ReadOnly = false;
		this.xtra_Task.SettingInVisible = false;
		this.xtra_Task.SettingReadonly = false;
		this.xtra_Task.Size = new System.Drawing.Size(971, 217);
		this.xtra_Task.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtra_Task.Text = "Related Tasks";
		this.grd_ListCompleted.AllowPasteExcel = false;
		this.grd_ListCompleted.ClipboardData = null;
		this.grd_ListCompleted.ContextMenuStrip = this.ctm_Task;
		this.grd_ListCompleted.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_ListCompleted.Location = new System.Drawing.Point(0, 0);
		this.grd_ListCompleted.LookAndFeel.UseDefaultLookAndFeel = false;
		this.grd_ListCompleted.MainView = this.gv_ListCompleted;
		this.grd_ListCompleted.Name = "grd_ListCompleted";
		this.grd_ListCompleted.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.rep_bte_Open, this.rep_bte_Closed });
		this.grd_ListCompleted.Size = new System.Drawing.Size(971, 217);
		this.grd_ListCompleted.TabIndex = 20;
		this.grd_ListCompleted.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_ListCompleted });
		this.ctm_Task.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.newTaskToolStripMenuItem });
		this.ctm_Task.Name = "ctm_Task";
		this.ctm_Task.Size = new System.Drawing.Size(124, 26);
		this.newTaskToolStripMenuItem.Name = "newTaskToolStripMenuItem";
		this.newTaskToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
		this.newTaskToolStripMenuItem.Text = "New Task";
		this.newTaskToolStripMenuItem.Click += new System.EventHandler(this.newTaskToolStripMenuItem_Click);
		this.gv_ListCompleted.Appearance.FocusedRow.BackColor = System.Drawing.Color.DarkSeaGreen;
		this.gv_ListCompleted.Appearance.FocusedRow.BorderColor = System.Drawing.Color.DarkSeaGreen;
		this.gv_ListCompleted.Appearance.Row.ForeColor = System.Drawing.Color.FromArgb(47, 46, 68);
		this.gv_ListCompleted.Appearance.Row.Options.UseForeColor = true;
		this.gv_ListCompleted.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.gv_ListCompleted.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_ListCompleted.ColumnKey = null;
		this.gv_ListCompleted.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[12]
		{
			this.gridColumnEx56, this.gc_Closed_Completed, this.gridColumnEx57, this.gc_DueDate_Completed, this.gc_ProjectEntry_Completed, this.gc_ProjectWBS_Completed, this.gridColumnEx58, this.gridColumnEx59, this.gridColumnEx60, this.gc_AssignTo_Completed,
			this.gc_CloseDate_Completed, this.gc_Priority_Completed
		});
		this.gv_ListCompleted.DisableFormSetting = false;
		this.gv_ListCompleted.DisplayException = false;
		this.gv_ListCompleted.EditColumn = null;
		this.gv_ListCompleted.FieldCurrency = null;
		this.gv_ListCompleted.FieldRate = null;
		gridFormatRule.Column = this.gc_Priority_Completed;
		gridFormatRule.ColumnApplyTo = this.gc_Priority_Completed;
		gridFormatRule.Name = "Format0";
		formatConditionIconSet.CategoryName = "Shapes";
		formatConditionIconSetIcon.PredefinedName = "RedToBlack4_1.png";
		formatConditionIconSetIcon.Value = new decimal(new int[4] { 3, 0, 0, 0 });
		formatConditionIconSetIcon.ValueComparison = DevExpress.XtraEditors.FormatConditionComparisonType.GreaterOrEqual;
		formatConditionIconSetIcon2.PredefinedName = "TrafficLights23_2.png";
		formatConditionIconSetIcon2.Value = new decimal(new int[4] { 2, 0, 0, 0 });
		formatConditionIconSetIcon2.ValueComparison = DevExpress.XtraEditors.FormatConditionComparisonType.GreaterOrEqual;
		formatConditionIconSetIcon3.PredefinedName = "RedToBlack4_3.png";
		formatConditionIconSetIcon3.Value = new decimal(new int[4] { 1, 0, 0, 0 });
		formatConditionIconSetIcon3.ValueComparison = DevExpress.XtraEditors.FormatConditionComparisonType.GreaterOrEqual;
		formatConditionIconSetIcon4.PredefinedName = "Signs3_1.png";
		formatConditionIconSetIcon4.ValueComparison = DevExpress.XtraEditors.FormatConditionComparisonType.GreaterOrEqual;
		formatConditionIconSet.Icons.Add(formatConditionIconSetIcon);
		formatConditionIconSet.Icons.Add(formatConditionIconSetIcon2);
		formatConditionIconSet.Icons.Add(formatConditionIconSetIcon3);
		formatConditionIconSet.Icons.Add(formatConditionIconSetIcon4);
		formatConditionIconSet.Name = "RedToBlack";
		formatConditionRuleIconSet.IconSet = formatConditionIconSet;
		gridFormatRule.Rule = formatConditionRuleIconSet;
		this.gv_ListCompleted.FormatRules.Add(gridFormatRule);
		this.gv_ListCompleted.FormId = "oMarketingCampaign";
		this.gv_ListCompleted.GridControl = this.grd_ListCompleted;
		this.gv_ListCompleted.IgnoreFilter = false;
		this.gv_ListCompleted.IndexEditRow = -1;
		this.gv_ListCompleted.IndicatorWidth = 40;
		this.gv_ListCompleted.IsApzTable = false;
		this.gv_ListCompleted.IsInsertNewRow = false;
		this.gv_ListCompleted.IsLoadingData = false;
		this.gv_ListCompleted.IsMappingContextAction = false;
		this.gv_ListCompleted.ItemId = "APZ_OCLG";
		this.gv_ListCompleted.LstColumnAllowBatch = null;
		this.gv_ListCompleted.LstColumnNotAllowBatch = null;
		this.gv_ListCompleted.LstColumnNotEditable = null;
		this.gv_ListCompleted.LstColumNotSetting = null;
		this.gv_ListCompleted.Name = "gv_ListCompleted";
		this.gv_ListCompleted.NewItemRowText = "New Task";
		this.gv_ListCompleted.OptionsBehavior.Editable = false;
		this.gv_ListCompleted.OptionsView.BestFitMaxRowCount = 100;
		this.gv_ListCompleted.OptionsView.ColumnAutoWidth = false;
		this.gv_ListCompleted.OptionsView.ShowGroupPanel = false;
		this.gv_ListCompleted.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.True;
		this.gv_ListCompleted.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.False;
		this.gv_ListCompleted.UseBatchMod = false;
		this.gv_ListCompleted.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_ListCompleted_RowCellStyle);
		this.gv_ListCompleted.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gv_ListCompleted_CustomRowCellEdit);
		this.gv_ListCompleted.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gv_ListCompleted_CustomUnboundColumnData);
		this.gv_ListCompleted.DoubleClick += new System.EventHandler(this.gv_ListCompleted_DoubleClick);
		this.gridColumnEx56.Caption = "Code";
		this.gridColumnEx56.CurrencyDateField = null;
		this.gridColumnEx56.CurrencyField = null;
		this.gridColumnEx56.CurrencyFixed = null;
		this.gridColumnEx56.CurrencyFixedRate = 0.0;
		this.gridColumnEx56.CurrencyRateField = null;
		this.gridColumnEx56.FieldName = "ClgCode";
		this.gridColumnEx56.Name = "gridColumnEx56";
		this.gridColumnEx56.PrimaryKey = false;
		this.gridColumnEx56.UnitField = null;
		this.gc_Closed_Completed.Caption = " Closed";
		this.gc_Closed_Completed.ColumnEdit = this.rep_bte_Closed;
		this.gc_Closed_Completed.CurrencyDateField = null;
		this.gc_Closed_Completed.CurrencyField = null;
		this.gc_Closed_Completed.CurrencyFixed = null;
		this.gc_Closed_Completed.CurrencyFixedRate = 0.0;
		this.gc_Closed_Completed.CurrencyRateField = null;
		this.gc_Closed_Completed.FieldName = "Closed";
		this.gc_Closed_Completed.Name = "gc_Closed_Completed";
		this.gc_Closed_Completed.OptionsColumn.AllowSize = false;
		this.gc_Closed_Completed.OptionsColumn.FixedWidth = true;
		this.gc_Closed_Completed.OptionsColumn.ShowCaption = false;
		this.gc_Closed_Completed.OptionsColumn.TabStop = false;
		this.gc_Closed_Completed.PrimaryKey = false;
		this.gc_Closed_Completed.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gc_Closed_Completed.UnitField = null;
		this.gc_Closed_Completed.Visible = true;
		this.gc_Closed_Completed.VisibleIndex = 0;
		this.gc_Closed_Completed.Width = 24;
		this.rep_bte_Closed.AutoHeight = false;
		editorButtonImageOptions11.Image = Apzon.Forms.Properties.Resources.completed;
		this.rep_bte_Closed.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, editorButtonImageOptions11, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance22, appearanceHovered22, appearancePressed22, appearanceDisabled22, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_bte_Closed.Name = "rep_bte_Closed";
		this.rep_bte_Closed.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
		this.gridColumnEx57.Caption = "Details";
		this.gridColumnEx57.CurrencyDateField = null;
		this.gridColumnEx57.CurrencyField = null;
		this.gridColumnEx57.CurrencyFixed = null;
		this.gridColumnEx57.CurrencyFixedRate = 0.0;
		this.gridColumnEx57.CurrencyRateField = null;
		this.gridColumnEx57.FieldName = "Details";
		this.gridColumnEx57.Name = "gridColumnEx57";
		this.gridColumnEx57.PrimaryKey = false;
		this.gridColumnEx57.UnitField = null;
		this.gridColumnEx57.Visible = true;
		this.gridColumnEx57.VisibleIndex = 1;
		this.gridColumnEx57.Width = 677;
		this.gc_DueDate_Completed.AppearanceCell.Options.UseTextOptions = true;
		this.gc_DueDate_Completed.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.gc_DueDate_Completed.Caption = "Due Date";
		this.gc_DueDate_Completed.CurrencyDateField = null;
		this.gc_DueDate_Completed.CurrencyField = null;
		this.gc_DueDate_Completed.CurrencyFixed = null;
		this.gc_DueDate_Completed.CurrencyFixedRate = 0.0;
		this.gc_DueDate_Completed.CurrencyRateField = null;
		this.gc_DueDate_Completed.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gc_DueDate_Completed.FieldName = "endDate";
		this.gc_DueDate_Completed.Name = "gc_DueDate_Completed";
		this.gc_DueDate_Completed.OptionsColumn.AllowEdit = false;
		this.gc_DueDate_Completed.OptionsColumn.AllowFocus = false;
		this.gc_DueDate_Completed.OptionsColumn.FixedWidth = true;
		this.gc_DueDate_Completed.OptionsColumn.ReadOnly = true;
		this.gc_DueDate_Completed.OptionsColumn.TabStop = false;
		this.gc_DueDate_Completed.PrimaryKey = false;
		this.gc_DueDate_Completed.UnitField = null;
		this.gc_DueDate_Completed.Visible = true;
		this.gc_DueDate_Completed.VisibleIndex = 3;
		this.gc_DueDate_Completed.Width = 60;
		this.gc_ProjectEntry_Completed.AppearanceCell.Options.UseTextOptions = true;
		this.gc_ProjectEntry_Completed.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.gc_ProjectEntry_Completed.Caption = "Project ";
		this.gc_ProjectEntry_Completed.CurrencyDateField = null;
		this.gc_ProjectEntry_Completed.CurrencyField = null;
		this.gc_ProjectEntry_Completed.CurrencyFixed = null;
		this.gc_ProjectEntry_Completed.CurrencyFixedRate = 0.0;
		this.gc_ProjectEntry_Completed.CurrencyRateField = null;
		this.gc_ProjectEntry_Completed.FieldName = "ProjectEntry";
		this.gc_ProjectEntry_Completed.Name = "gc_ProjectEntry_Completed";
		this.gc_ProjectEntry_Completed.OptionsColumn.FixedWidth = true;
		this.gc_ProjectEntry_Completed.PrimaryKey = false;
		this.gc_ProjectEntry_Completed.UnitField = null;
		this.gc_ProjectWBS_Completed.AppearanceCell.Options.UseTextOptions = true;
		this.gc_ProjectWBS_Completed.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.gc_ProjectWBS_Completed.Caption = "WBS";
		this.gc_ProjectWBS_Completed.CurrencyDateField = null;
		this.gc_ProjectWBS_Completed.CurrencyField = null;
		this.gc_ProjectWBS_Completed.CurrencyFixed = null;
		this.gc_ProjectWBS_Completed.CurrencyFixedRate = 0.0;
		this.gc_ProjectWBS_Completed.CurrencyRateField = null;
		this.gc_ProjectWBS_Completed.FieldName = "ProjectWBS";
		this.gc_ProjectWBS_Completed.Name = "gc_ProjectWBS_Completed";
		this.gc_ProjectWBS_Completed.OptionsColumn.FixedWidth = true;
		this.gc_ProjectWBS_Completed.PrimaryKey = false;
		this.gc_ProjectWBS_Completed.UnitField = null;
		this.gc_ProjectWBS_Completed.Width = 100;
		this.gridColumnEx58.Caption = "OrderLine";
		this.gridColumnEx58.CurrencyDateField = null;
		this.gridColumnEx58.CurrencyField = null;
		this.gridColumnEx58.CurrencyFixed = null;
		this.gridColumnEx58.CurrencyFixedRate = 0.0;
		this.gridColumnEx58.CurrencyRateField = null;
		this.gridColumnEx58.FieldName = "OrderLine";
		this.gridColumnEx58.Name = "gridColumnEx58";
		this.gridColumnEx58.PrimaryKey = false;
		this.gridColumnEx58.UnitField = null;
		this.gridColumnEx59.Caption = "Create Date";
		this.gridColumnEx59.CurrencyDateField = null;
		this.gridColumnEx59.CurrencyField = null;
		this.gridColumnEx59.CurrencyFixed = null;
		this.gridColumnEx59.CurrencyFixedRate = 0.0;
		this.gridColumnEx59.CurrencyRateField = null;
		this.gridColumnEx59.FieldName = "CntctDate";
		this.gridColumnEx59.Name = "gridColumnEx59";
		this.gridColumnEx59.PrimaryKey = false;
		this.gridColumnEx59.UnitField = null;
		this.gridColumnEx60.Caption = "End Time";
		this.gridColumnEx60.CurrencyDateField = null;
		this.gridColumnEx60.CurrencyField = null;
		this.gridColumnEx60.CurrencyFixed = null;
		this.gridColumnEx60.CurrencyFixedRate = 0.0;
		this.gridColumnEx60.CurrencyRateField = null;
		this.gridColumnEx60.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Time;
		this.gridColumnEx60.FieldName = "ENDTime";
		this.gridColumnEx60.Name = "gridColumnEx60";
		this.gridColumnEx60.PrimaryKey = false;
		this.gridColumnEx60.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridColumnEx60.UnitField = null;
		this.gridColumnEx60.Visible = true;
		this.gridColumnEx60.VisibleIndex = 4;
		this.gridColumnEx60.Width = 178;
		this.gc_AssignTo_Completed.Caption = "AssignTo";
		this.gc_AssignTo_Completed.CurrencyDateField = null;
		this.gc_AssignTo_Completed.CurrencyField = null;
		this.gc_AssignTo_Completed.CurrencyFixed = null;
		this.gc_AssignTo_Completed.CurrencyFixedRate = 0.0;
		this.gc_AssignTo_Completed.CurrencyRateField = null;
		this.gc_AssignTo_Completed.FieldName = "AssignTo";
		this.gc_AssignTo_Completed.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gc_AssignTo_Completed.Name = "gc_AssignTo_Completed";
		this.gc_AssignTo_Completed.OptionsColumn.FixedWidth = true;
		this.gc_AssignTo_Completed.PrimaryKey = false;
		this.gc_AssignTo_Completed.UnboundType = DevExpress.Data.UnboundColumnType.String;
		this.gc_AssignTo_Completed.UnitField = null;
		this.gc_AssignTo_Completed.Visible = true;
		this.gc_AssignTo_Completed.VisibleIndex = 2;
		this.gc_AssignTo_Completed.Width = 120;
		this.gc_CloseDate_Completed.Caption = "gridColumnEx27";
		this.gc_CloseDate_Completed.CurrencyDateField = null;
		this.gc_CloseDate_Completed.CurrencyField = null;
		this.gc_CloseDate_Completed.CurrencyFixed = null;
		this.gc_CloseDate_Completed.CurrencyFixedRate = 0.0;
		this.gc_CloseDate_Completed.CurrencyRateField = null;
		this.gc_CloseDate_Completed.FieldName = "CloseDate";
		this.gc_CloseDate_Completed.Name = "gc_CloseDate_Completed";
		this.gc_CloseDate_Completed.PrimaryKey = false;
		this.gc_CloseDate_Completed.UnitField = null;
		this.rep_bte_Open.AutoHeight = false;
		editorButtonImageOptions12.Image = Apzon.Forms.Properties.Resources.incompleted;
		this.rep_bte_Open.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, editorButtonImageOptions12, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance23, appearanceHovered23, appearancePressed23, appearanceDisabled23, "Open", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_bte_Open.Name = "rep_bte_Open";
		this.rep_bte_Open.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
		this.xtra_Sub.Controls.Add(this.grd_Sub);
		this.xtra_Sub.EditControl = null;
		this.xtra_Sub.EditValue = "Sub Campaign";
		this.xtra_Sub.FieldName = null;
		this.xtra_Sub.IsSearchControl = false;
		this.xtra_Sub.Name = "xtra_Sub";
		this.xtra_Sub.ReadOnly = false;
		this.xtra_Sub.SettingInVisible = false;
		this.xtra_Sub.SettingReadonly = false;
		this.xtra_Sub.Size = new System.Drawing.Size(971, 217);
		this.xtra_Sub.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtra_Sub.Text = "Sub Campaign";
		this.grd_Sub.AllowPasteExcel = false;
		this.grd_Sub.ClipboardData = null;
		this.grd_Sub.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Sub.Location = new System.Drawing.Point(0, 0);
		this.grd_Sub.MainView = this.gv_Sub;
		this.grd_Sub.Name = "grd_Sub";
		this.grd_Sub.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[9] { this.luk_TargetTypeSub, this.btn_EmployeeSub, this.luk_StatusSub, this.btn_ProjectSub, this.luk_TypeSub, this.luk_StageSub, this.btn_ParentSub, this.luk_TargetGroupSub, this.btn_DocEntrySub });
		this.grd_Sub.Size = new System.Drawing.Size(971, 217);
		this.grd_Sub.TabIndex = 1;
		this.grd_Sub.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Sub });
		this.gv_Sub.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Sub.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Sub.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Sub.ColumnKey = null;
		this.gv_Sub.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[20]
		{
			this.gridColumnEx70, this.gridColumnEx71, this.gridColumnEx72, this.gridColumnEx73, this.gridColumnEx74, this.gridColumnEx75, this.gridColumnEx76, this.gridColumnEx77, this.gridColumnEx78, this.gridColumnEx79,
			this.gridColumnEx80, this.gridColumnEx81, this.gridColumnEx82, this.gridColumnEx83, this.gridColumnEx84, this.gridColumnEx85, this.gridColumnEx86, this.gridColumnEx87, this.gridColumnEx88, this.gridColumnEx89
		});
		this.gv_Sub.DisableFormSetting = false;
		this.gv_Sub.DisplayException = false;
		this.gv_Sub.EditColumn = null;
		this.gv_Sub.FieldCurrency = null;
		this.gv_Sub.FieldRate = null;
		this.gv_Sub.FormId = "oMarketingCampaign";
		this.gv_Sub.GridControl = this.grd_Sub;
		this.gv_Sub.IgnoreFilter = false;
		this.gv_Sub.IndexEditRow = -1;
		this.gv_Sub.IndicatorWidth = 35;
		this.gv_Sub.IsApzTable = false;
		this.gv_Sub.IsInsertNewRow = false;
		this.gv_Sub.IsLoadingData = false;
		this.gv_Sub.IsMappingContextAction = false;
		this.gv_Sub.ItemId = "APZ_SMC";
		this.gv_Sub.LstColumnAllowBatch = null;
		this.gv_Sub.LstColumnNotAllowBatch = null;
		this.gv_Sub.LstColumnNotEditable = null;
		this.gv_Sub.LstColumNotSetting = null;
		this.gv_Sub.Name = "gv_Sub";
		this.gv_Sub.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Sub.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Sub.OptionsView.ColumnAutoWidth = false;
		this.gv_Sub.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Sub.OptionsView.ShowGroupPanel = false;
		this.gv_Sub.UseBatchMod = false;
		this.gridColumnEx70.Caption = "Doc Entry";
		this.gridColumnEx70.ColumnEdit = this.btn_DocEntrySub;
		this.gridColumnEx70.CurrencyDateField = null;
		this.gridColumnEx70.CurrencyField = null;
		this.gridColumnEx70.CurrencyFixed = null;
		this.gridColumnEx70.CurrencyFixedRate = 0.0;
		this.gridColumnEx70.CurrencyRateField = null;
		this.gridColumnEx70.FieldName = "DocEntry";
		this.gridColumnEx70.Name = "gridColumnEx70";
		this.gridColumnEx70.OptionsColumn.ReadOnly = true;
		this.gridColumnEx70.PrimaryKey = false;
		this.gridColumnEx70.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridColumnEx70.UnitField = null;
		this.gridColumnEx70.Visible = true;
		this.gridColumnEx70.VisibleIndex = 0;
		this.btn_DocEntrySub.AutoHeight = false;
		this.btn_DocEntrySub.AutoLoadDataSource = true;
		editorButtonImageOptions13.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions36.Image");
		this.btn_DocEntrySub.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, false, false, imageOptions12, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance24, appearanceHovered24, appearancePressed24, appearanceDisabled24, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions13, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance25, appearanceHovered25, appearancePressed25, appearanceDisabled25, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_DocEntrySub.ContainerControl = this;
		this.btn_DocEntrySub.DisplayMember = "DocEntry";
		this.btn_DocEntrySub.DisplayMember2 = "";
		this.btn_DocEntrySub.LoadDataSourceWhenEnter = false;
		this.btn_DocEntrySub.Name = "btn_DocEntrySub";
		this.btn_DocEntrySub.ReadOnly = true;
		this.btn_DocEntrySub.SeparateCharacterForMulti = ';';
		this.btn_DocEntrySub.SeparateString = ", ";
		this.btn_DocEntrySub.UsingAutoCompleteBox = false;
		this.btn_DocEntrySub.UsingDataSource = true;
		this.btn_DocEntrySub.ValidateWhenNoDataSource = false;
		this.btn_DocEntrySub.ValueMember = "DocEntry";
		this.gridColumnEx71.Caption = "Target Type";
		this.gridColumnEx71.ColumnEdit = this.luk_TargetTypeSub;
		this.gridColumnEx71.CurrencyDateField = null;
		this.gridColumnEx71.CurrencyField = null;
		this.gridColumnEx71.CurrencyFixed = null;
		this.gridColumnEx71.CurrencyFixedRate = 0.0;
		this.gridColumnEx71.CurrencyRateField = null;
		this.gridColumnEx71.FieldName = "TargetType";
		this.gridColumnEx71.Name = "gridColumnEx71";
		this.gridColumnEx71.PrimaryKey = false;
		this.gridColumnEx71.UnitField = null;
		this.gridColumnEx71.Visible = true;
		this.gridColumnEx71.VisibleIndex = 1;
		this.luk_TargetTypeSub.AutoHeight = false;
		this.luk_TargetTypeSub.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_TargetTypeSub.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_TargetTypeSub.IsSearchControl = false;
		this.luk_TargetTypeSub.Name = "luk_TargetTypeSub";
		this.luk_TargetTypeSub.NullText = "";
		this.luk_TargetTypeSub.ShowFooter = false;
		this.luk_TargetTypeSub.ShowHeader = false;
		this.gridColumnEx72.Caption = "Campaign No";
		this.gridColumnEx72.CurrencyDateField = null;
		this.gridColumnEx72.CurrencyField = null;
		this.gridColumnEx72.CurrencyFixed = null;
		this.gridColumnEx72.CurrencyFixedRate = 0.0;
		this.gridColumnEx72.CurrencyRateField = null;
		this.gridColumnEx72.FieldName = "CampaignNo";
		this.gridColumnEx72.Name = "gridColumnEx72";
		this.gridColumnEx72.PrimaryKey = false;
		this.gridColumnEx72.UnitField = null;
		this.gridColumnEx72.Visible = true;
		this.gridColumnEx72.VisibleIndex = 2;
		this.gridColumnEx73.Caption = "Campaign Name";
		this.gridColumnEx73.CurrencyDateField = null;
		this.gridColumnEx73.CurrencyField = null;
		this.gridColumnEx73.CurrencyFixed = null;
		this.gridColumnEx73.CurrencyFixedRate = 0.0;
		this.gridColumnEx73.CurrencyRateField = null;
		this.gridColumnEx73.FieldName = "CampaignName";
		this.gridColumnEx73.Name = "gridColumnEx73";
		this.gridColumnEx73.PrimaryKey = false;
		this.gridColumnEx73.UnitField = null;
		this.gridColumnEx73.Visible = true;
		this.gridColumnEx73.VisibleIndex = 3;
		this.gridColumnEx73.Width = 118;
		this.gridColumnEx74.Caption = "Campaign Type";
		this.gridColumnEx74.ColumnEdit = this.luk_TypeSub;
		this.gridColumnEx74.CurrencyDateField = null;
		this.gridColumnEx74.CurrencyField = null;
		this.gridColumnEx74.CurrencyFixed = null;
		this.gridColumnEx74.CurrencyFixedRate = 0.0;
		this.gridColumnEx74.CurrencyRateField = null;
		this.gridColumnEx74.FieldName = "CampaignType";
		this.gridColumnEx74.Name = "gridColumnEx74";
		this.gridColumnEx74.PrimaryKey = false;
		this.gridColumnEx74.UnitField = null;
		this.gridColumnEx74.Visible = true;
		this.gridColumnEx74.VisibleIndex = 4;
		this.gridColumnEx74.Width = 126;
		this.luk_TypeSub.AutoHeight = false;
		this.luk_TypeSub.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
		this.luk_TypeSub.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_TypeSub.CheckFields = null;
		this.luk_TypeSub.CheckTables = null;
		this.luk_TypeSub.Name = "luk_TypeSub";
		this.luk_TypeSub.NullText = "";
		this.luk_TypeSub.ShowFooter = false;
		this.luk_TypeSub.ShowHeader = false;
		this.gridColumnEx75.Caption = "Owner Person";
		this.gridColumnEx75.ColumnEdit = this.btn_EmployeeSub;
		this.gridColumnEx75.CurrencyDateField = null;
		this.gridColumnEx75.CurrencyField = null;
		this.gridColumnEx75.CurrencyFixed = null;
		this.gridColumnEx75.CurrencyFixedRate = 0.0;
		this.gridColumnEx75.CurrencyRateField = null;
		this.gridColumnEx75.FieldName = "EmployeeNo";
		this.gridColumnEx75.Name = "gridColumnEx75";
		this.gridColumnEx75.PrimaryKey = false;
		this.gridColumnEx75.UnitField = null;
		this.gridColumnEx75.Visible = true;
		this.gridColumnEx75.VisibleIndex = 5;
		this.gridColumnEx75.Width = 120;
		this.btn_EmployeeSub.AutoHeight = false;
		this.btn_EmployeeSub.AutoLoadDataSource = true;
		editorButtonImageOptions14.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions25.Image");
		this.btn_EmployeeSub.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions13, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance26, appearanceHovered26, appearancePressed26, appearanceDisabled26, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions14, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance27, appearanceHovered27, appearancePressed27, appearanceDisabled27, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_EmployeeSub.ContainerControl = this;
		this.btn_EmployeeSub.DisplayMember = "lastName";
		this.btn_EmployeeSub.DisplayMember2 = "firstName";
		this.btn_EmployeeSub.LoadDataSourceWhenEnter = false;
		this.btn_EmployeeSub.Name = "btn_EmployeeSub";
		this.btn_EmployeeSub.SeparateCharacterForMulti = ';';
		this.btn_EmployeeSub.SeparateString = ", ";
		this.btn_EmployeeSub.UsingAutoCompleteBox = false;
		this.btn_EmployeeSub.UsingDataSource = true;
		this.btn_EmployeeSub.ValidateWhenNoDataSource = false;
		this.btn_EmployeeSub.ValueMember = "EmpId";
		this.gridColumnEx76.Caption = "Status";
		this.gridColumnEx76.ColumnEdit = this.luk_StatusSub;
		this.gridColumnEx76.CurrencyDateField = null;
		this.gridColumnEx76.CurrencyField = null;
		this.gridColumnEx76.CurrencyFixed = null;
		this.gridColumnEx76.CurrencyFixedRate = 0.0;
		this.gridColumnEx76.CurrencyRateField = null;
		this.gridColumnEx76.FieldName = "Status";
		this.gridColumnEx76.Name = "gridColumnEx76";
		this.gridColumnEx76.PrimaryKey = false;
		this.gridColumnEx76.UnitField = null;
		this.gridColumnEx76.Visible = true;
		this.gridColumnEx76.VisibleIndex = 6;
		this.luk_StatusSub.AutoHeight = false;
		this.luk_StatusSub.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_StatusSub.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_StatusSub.IsSearchControl = false;
		this.luk_StatusSub.Name = "luk_StatusSub";
		this.luk_StatusSub.NullText = "";
		this.luk_StatusSub.ShowFooter = false;
		this.luk_StatusSub.ShowHeader = false;
		this.gridColumnEx77.Caption = "Target Group";
		this.gridColumnEx77.ColumnEdit = this.luk_TargetGroupSub;
		this.gridColumnEx77.CurrencyDateField = null;
		this.gridColumnEx77.CurrencyField = null;
		this.gridColumnEx77.CurrencyFixed = null;
		this.gridColumnEx77.CurrencyFixedRate = 0.0;
		this.gridColumnEx77.CurrencyRateField = null;
		this.gridColumnEx77.FieldName = "TargetGroup";
		this.gridColumnEx77.Name = "gridColumnEx77";
		this.gridColumnEx77.PrimaryKey = false;
		this.gridColumnEx77.UnitField = null;
		this.gridColumnEx77.Visible = true;
		this.gridColumnEx77.VisibleIndex = 7;
		this.luk_TargetGroupSub.AutoHeight = false;
		this.luk_TargetGroupSub.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
		this.luk_TargetGroupSub.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_TargetGroupSub.CheckFields = null;
		this.luk_TargetGroupSub.CheckTables = null;
		this.luk_TargetGroupSub.Name = "luk_TargetGroupSub";
		this.luk_TargetGroupSub.NullText = "";
		this.luk_TargetGroupSub.ShowFooter = false;
		this.luk_TargetGroupSub.ShowHeader = false;
		this.gridColumnEx78.Caption = "Start Date";
		this.gridColumnEx78.CurrencyDateField = null;
		this.gridColumnEx78.CurrencyField = null;
		this.gridColumnEx78.CurrencyFixed = null;
		this.gridColumnEx78.CurrencyFixedRate = 0.0;
		this.gridColumnEx78.CurrencyRateField = null;
		this.gridColumnEx78.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gridColumnEx78.FieldName = "StartDate";
		this.gridColumnEx78.Name = "gridColumnEx78";
		this.gridColumnEx78.PrimaryKey = false;
		this.gridColumnEx78.UnitField = null;
		this.gridColumnEx78.Visible = true;
		this.gridColumnEx78.VisibleIndex = 8;
		this.gridColumnEx79.Caption = "End Date";
		this.gridColumnEx79.CurrencyDateField = null;
		this.gridColumnEx79.CurrencyField = null;
		this.gridColumnEx79.CurrencyFixed = null;
		this.gridColumnEx79.CurrencyFixedRate = 0.0;
		this.gridColumnEx79.CurrencyRateField = null;
		this.gridColumnEx79.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gridColumnEx79.FieldName = "EndDate";
		this.gridColumnEx79.Name = "gridColumnEx79";
		this.gridColumnEx79.PrimaryKey = false;
		this.gridColumnEx79.UnitField = null;
		this.gridColumnEx79.Visible = true;
		this.gridColumnEx79.VisibleIndex = 9;
		this.gridColumnEx80.Caption = "Parent Campaign";
		this.gridColumnEx80.ColumnEdit = this.btn_ParentSub;
		this.gridColumnEx80.CurrencyDateField = null;
		this.gridColumnEx80.CurrencyField = null;
		this.gridColumnEx80.CurrencyFixed = null;
		this.gridColumnEx80.CurrencyFixedRate = 0.0;
		this.gridColumnEx80.CurrencyRateField = null;
		this.gridColumnEx80.FieldName = "ParentCampaign";
		this.gridColumnEx80.Name = "gridColumnEx80";
		this.gridColumnEx80.PrimaryKey = false;
		this.gridColumnEx80.UnitField = null;
		this.gridColumnEx80.Visible = true;
		this.gridColumnEx80.VisibleIndex = 10;
		this.gridColumnEx80.Width = 99;
		this.btn_ParentSub.AutoHeight = false;
		this.btn_ParentSub.AutoLoadDataSource = true;
		editorButtonImageOptions15.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions27.Image");
		this.btn_ParentSub.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions14, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance28, appearanceHovered28, appearancePressed28, appearanceDisabled28, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions15, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance29, appearanceHovered29, appearancePressed29, appearanceDisabled29, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_ParentSub.ContainerControl = this;
		this.btn_ParentSub.DisplayMember = "";
		this.btn_ParentSub.DisplayMember2 = "";
		this.btn_ParentSub.LoadDataSourceWhenEnter = false;
		this.btn_ParentSub.Name = "btn_ParentSub";
		this.btn_ParentSub.SeparateCharacterForMulti = ';';
		this.btn_ParentSub.SeparateString = ", ";
		this.btn_ParentSub.UsingAutoCompleteBox = false;
		this.btn_ParentSub.UsingDataSource = true;
		this.btn_ParentSub.ValidateWhenNoDataSource = false;
		this.btn_ParentSub.ValueMember = "";
		this.gridColumnEx81.Caption = "Stage";
		this.gridColumnEx81.ColumnEdit = this.luk_StageSub;
		this.gridColumnEx81.CurrencyDateField = null;
		this.gridColumnEx81.CurrencyField = null;
		this.gridColumnEx81.CurrencyFixed = null;
		this.gridColumnEx81.CurrencyFixedRate = 0.0;
		this.gridColumnEx81.CurrencyRateField = null;
		this.gridColumnEx81.FieldName = "Stage";
		this.gridColumnEx81.Name = "gridColumnEx81";
		this.gridColumnEx81.PrimaryKey = false;
		this.gridColumnEx81.UnitField = null;
		this.gridColumnEx81.Visible = true;
		this.gridColumnEx81.VisibleIndex = 11;
		this.luk_StageSub.AutoHeight = false;
		this.luk_StageSub.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
		this.luk_StageSub.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_StageSub.CheckFields = null;
		this.luk_StageSub.CheckTables = null;
		this.luk_StageSub.Name = "luk_StageSub";
		this.luk_StageSub.NullText = "";
		this.luk_StageSub.ShowFooter = false;
		this.luk_StageSub.ShowHeader = false;
		this.gridColumnEx82.Caption = "Remark";
		this.gridColumnEx82.CurrencyDateField = null;
		this.gridColumnEx82.CurrencyField = null;
		this.gridColumnEx82.CurrencyFixed = null;
		this.gridColumnEx82.CurrencyFixedRate = 0.0;
		this.gridColumnEx82.CurrencyRateField = null;
		this.gridColumnEx82.FieldName = "Remark";
		this.gridColumnEx82.Name = "gridColumnEx82";
		this.gridColumnEx82.PrimaryKey = false;
		this.gridColumnEx82.UnitField = null;
		this.gridColumnEx82.Visible = true;
		this.gridColumnEx82.VisibleIndex = 12;
		this.gridColumnEx83.Caption = "Total Plan Revenue";
		this.gridColumnEx83.ColumnEdit = this.repositoryItemCalcEditEx1;
		this.gridColumnEx83.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.MainCurrency;
		this.gridColumnEx83.CurrencyDateField = null;
		this.gridColumnEx83.CurrencyField = null;
		this.gridColumnEx83.CurrencyFixed = null;
		this.gridColumnEx83.CurrencyFixedRate = 0.0;
		this.gridColumnEx83.CurrencyRateField = null;
		this.gridColumnEx83.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gridColumnEx83.FieldName = "TotalPlRevenue";
		this.gridColumnEx83.Name = "gridColumnEx83";
		this.gridColumnEx83.PrimaryKey = false;
		this.gridColumnEx83.UnitField = null;
		this.gridColumnEx83.Visible = true;
		this.gridColumnEx83.VisibleIndex = 13;
		this.gridColumnEx83.Width = 106;
		this.gridColumnEx84.Caption = "Opportunities";
		this.gridColumnEx84.CurrencyDateField = null;
		this.gridColumnEx84.CurrencyField = null;
		this.gridColumnEx84.CurrencyFixed = null;
		this.gridColumnEx84.CurrencyFixedRate = 0.0;
		this.gridColumnEx84.CurrencyRateField = null;
		this.gridColumnEx84.FieldName = "Opportunities";
		this.gridColumnEx84.Name = "gridColumnEx84";
		this.gridColumnEx84.PrimaryKey = false;
		this.gridColumnEx84.UnitField = null;
		this.gridColumnEx84.Visible = true;
		this.gridColumnEx84.VisibleIndex = 14;
		this.gridColumnEx85.Caption = "Total Quotation";
		this.gridColumnEx85.ColumnEdit = this.repositoryItemCalcEditEx2;
		this.gridColumnEx85.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.MainCurrency;
		this.gridColumnEx85.CurrencyDateField = null;
		this.gridColumnEx85.CurrencyField = null;
		this.gridColumnEx85.CurrencyFixed = null;
		this.gridColumnEx85.CurrencyFixedRate = 0.0;
		this.gridColumnEx85.CurrencyRateField = null;
		this.gridColumnEx85.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gridColumnEx85.FieldName = "TotalQuotation";
		this.gridColumnEx85.Name = "gridColumnEx85";
		this.gridColumnEx85.PrimaryKey = false;
		this.gridColumnEx85.UnitField = null;
		this.gridColumnEx85.Visible = true;
		this.gridColumnEx85.VisibleIndex = 15;
		this.gridColumnEx85.Width = 103;
		this.gridColumnEx86.Caption = "Quotation Value";
		this.gridColumnEx86.CurrencyDateField = null;
		this.gridColumnEx86.CurrencyField = null;
		this.gridColumnEx86.CurrencyFixed = null;
		this.gridColumnEx86.CurrencyFixedRate = 0.0;
		this.gridColumnEx86.CurrencyRateField = null;
		this.gridColumnEx86.FieldName = "QuotationValue";
		this.gridColumnEx86.Name = "gridColumnEx86";
		this.gridColumnEx86.PrimaryKey = false;
		this.gridColumnEx86.UnitField = null;
		this.gridColumnEx86.Visible = true;
		this.gridColumnEx86.VisibleIndex = 16;
		this.gridColumnEx86.Width = 96;
		this.gridColumnEx87.Caption = "Total Order";
		this.gridColumnEx87.ColumnEdit = this.repositoryItemCalcEditEx3;
		this.gridColumnEx87.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.MainCurrency;
		this.gridColumnEx87.CurrencyDateField = null;
		this.gridColumnEx87.CurrencyField = null;
		this.gridColumnEx87.CurrencyFixed = null;
		this.gridColumnEx87.CurrencyFixedRate = 0.0;
		this.gridColumnEx87.CurrencyRateField = null;
		this.gridColumnEx87.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gridColumnEx87.FieldName = "TotalOrder";
		this.gridColumnEx87.Name = "gridColumnEx87";
		this.gridColumnEx87.PrimaryKey = false;
		this.gridColumnEx87.UnitField = null;
		this.gridColumnEx87.Visible = true;
		this.gridColumnEx87.VisibleIndex = 17;
		this.gridColumnEx88.Caption = "Order Value";
		this.gridColumnEx88.CurrencyDateField = null;
		this.gridColumnEx88.CurrencyField = null;
		this.gridColumnEx88.CurrencyFixed = null;
		this.gridColumnEx88.CurrencyFixedRate = 0.0;
		this.gridColumnEx88.CurrencyRateField = null;
		this.gridColumnEx88.FieldName = "OrderValue";
		this.gridColumnEx88.Name = "gridColumnEx88";
		this.gridColumnEx88.PrimaryKey = false;
		this.gridColumnEx88.UnitField = null;
		this.gridColumnEx88.Visible = true;
		this.gridColumnEx88.VisibleIndex = 18;
		this.gridColumnEx89.Caption = "Project";
		this.gridColumnEx89.ColumnEdit = this.btn_ProjectSub;
		this.gridColumnEx89.CurrencyDateField = null;
		this.gridColumnEx89.CurrencyField = null;
		this.gridColumnEx89.CurrencyFixed = null;
		this.gridColumnEx89.CurrencyFixedRate = 0.0;
		this.gridColumnEx89.CurrencyRateField = null;
		this.gridColumnEx89.FieldName = "ProjectId";
		this.gridColumnEx89.Name = "gridColumnEx89";
		this.gridColumnEx89.PrimaryKey = false;
		this.gridColumnEx89.UnitField = null;
		this.gridColumnEx89.Visible = true;
		this.gridColumnEx89.VisibleIndex = 19;
		this.btn_ProjectSub.AutoHeight = false;
		this.btn_ProjectSub.AutoLoadDataSource = true;
		editorButtonImageOptions16.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions29.Image");
		this.btn_ProjectSub.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions15, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance30, appearanceHovered30, appearancePressed30, appearanceDisabled30, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions16, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance31, appearanceHovered31, appearancePressed31, appearanceDisabled31, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_ProjectSub.ContainerControl = this;
		this.btn_ProjectSub.DisplayMember = "PrjCode";
		this.btn_ProjectSub.DisplayMember2 = "";
		this.btn_ProjectSub.LoadDataSourceWhenEnter = false;
		this.btn_ProjectSub.Name = "btn_ProjectSub";
		this.btn_ProjectSub.SeparateCharacterForMulti = ';';
		this.btn_ProjectSub.SeparateString = ", ";
		this.btn_ProjectSub.UsingAutoCompleteBox = false;
		this.btn_ProjectSub.UsingDataSource = true;
		this.btn_ProjectSub.ValidateWhenNoDataSource = false;
		this.btn_ProjectSub.ValueMember = "PrjCode";
		this.xtra_Document.Controls.Add(this.grd_LineDocument);
		this.xtra_Document.EditControl = null;
		this.xtra_Document.EditValue = "Documents";
		this.xtra_Document.FieldName = null;
		this.xtra_Document.IsSearchControl = false;
		this.xtra_Document.Name = "xtra_Document";
		this.xtra_Document.ReadOnly = false;
		this.xtra_Document.SettingInVisible = false;
		this.xtra_Document.SettingReadonly = false;
		this.xtra_Document.Size = new System.Drawing.Size(971, 217);
		this.xtra_Document.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtra_Document.Text = "Documents";
		this.grd_LineDocument.AllowPasteExcel = false;
		this.grd_LineDocument.ClipboardData = null;
		this.grd_LineDocument.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_LineDocument.Location = new System.Drawing.Point(0, 0);
		this.grd_LineDocument.MainView = this.gv_LineDocument;
		this.grd_LineDocument.Name = "grd_LineDocument";
		this.grd_LineDocument.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.btn_DocNumDo, this.luk_CurrencyLineDu });
		this.grd_LineDocument.Size = new System.Drawing.Size(971, 217);
		this.grd_LineDocument.TabIndex = 1;
		this.grd_LineDocument.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_LineDocument });
		this.gv_LineDocument.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_LineDocument.ColumnKey = null;
		this.gv_LineDocument.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[8] { this.gridColumnEx91, this.gridColumnEx92, this.gridColumnEx93, this.gridColumnEx94, this.gridColumnEx95, this.gridColumnEx96, this.gridColumnEx97, this.gridColumnEx98 });
		this.gv_LineDocument.DisableFormSetting = false;
		this.gv_LineDocument.DisplayException = false;
		this.gv_LineDocument.EditColumn = null;
		this.gv_LineDocument.FieldCurrency = null;
		this.gv_LineDocument.FieldRate = null;
		this.gv_LineDocument.FormId = "oProjectManagement";
		this.gv_LineDocument.GridControl = this.grd_LineDocument;
		this.gv_LineDocument.IgnoreFilter = false;
		this.gv_LineDocument.IndexEditRow = -1;
		this.gv_LineDocument.IndicatorWidth = 35;
		this.gv_LineDocument.IsApzTable = false;
		this.gv_LineDocument.IsInsertNewRow = false;
		this.gv_LineDocument.IsLoadingData = false;
		this.gv_LineDocument.IsMappingContextAction = false;
		this.gv_LineDocument.ItemId = "APZ_PRG5";
		this.gv_LineDocument.LstColumnAllowBatch = null;
		this.gv_LineDocument.LstColumnNotAllowBatch = null;
		this.gv_LineDocument.LstColumnNotEditable = null;
		this.gv_LineDocument.LstColumNotSetting = null;
		this.gv_LineDocument.Name = "gv_LineDocument";
		this.gv_LineDocument.OptionsView.BestFitMaxRowCount = 100;
		this.gv_LineDocument.OptionsView.ColumnAutoWidth = false;
		this.gv_LineDocument.OptionsView.ShowGroupPanel = false;
		this.gv_LineDocument.UseBatchMod = false;
		this.gridColumnEx91.Caption = "AbsEntry";
		this.gridColumnEx91.CurrencyDateField = null;
		this.gridColumnEx91.CurrencyField = null;
		this.gridColumnEx91.CurrencyFixed = null;
		this.gridColumnEx91.CurrencyFixedRate = 0.0;
		this.gridColumnEx91.CurrencyRateField = null;
		this.gridColumnEx91.FieldName = "AbsEntry";
		this.gridColumnEx91.Name = "gridColumnEx91";
		this.gridColumnEx91.OptionsColumn.ReadOnly = true;
		this.gridColumnEx91.PrimaryKey = false;
		this.gridColumnEx91.UnitField = null;
		this.gridColumnEx92.Caption = "Doc Type";
		this.gridColumnEx92.CurrencyDateField = null;
		this.gridColumnEx92.CurrencyField = null;
		this.gridColumnEx92.CurrencyFixed = null;
		this.gridColumnEx92.CurrencyFixedRate = 0.0;
		this.gridColumnEx92.CurrencyRateField = null;
		this.gridColumnEx92.FieldName = "ObjTypeName";
		this.gridColumnEx92.Name = "gridColumnEx92";
		this.gridColumnEx92.OptionsColumn.ReadOnly = true;
		this.gridColumnEx92.PrimaryKey = false;
		this.gridColumnEx92.UnitField = null;
		this.gridColumnEx92.Visible = true;
		this.gridColumnEx92.VisibleIndex = 1;
		this.gridColumnEx93.Caption = "Doc Entry";
		this.gridColumnEx93.CurrencyDateField = null;
		this.gridColumnEx93.CurrencyField = null;
		this.gridColumnEx93.CurrencyFixed = null;
		this.gridColumnEx93.CurrencyFixedRate = 0.0;
		this.gridColumnEx93.CurrencyRateField = null;
		this.gridColumnEx93.FieldName = "DocEntry";
		this.gridColumnEx93.Name = "gridColumnEx93";
		this.gridColumnEx93.OptionsColumn.ReadOnly = true;
		this.gridColumnEx93.PrimaryKey = false;
		this.gridColumnEx93.UnitField = null;
		this.gridColumnEx93.Visible = true;
		this.gridColumnEx93.VisibleIndex = 2;
		this.gridColumnEx94.Caption = "Doc Num";
		this.gridColumnEx94.ColumnEdit = this.btn_DocNumDo;
		this.gridColumnEx94.CurrencyDateField = null;
		this.gridColumnEx94.CurrencyField = null;
		this.gridColumnEx94.CurrencyFixed = null;
		this.gridColumnEx94.CurrencyFixedRate = 0.0;
		this.gridColumnEx94.CurrencyRateField = null;
		this.gridColumnEx94.FieldName = "DocNum";
		this.gridColumnEx94.Name = "gridColumnEx94";
		this.gridColumnEx94.OptionsColumn.ReadOnly = true;
		this.gridColumnEx94.PrimaryKey = false;
		this.gridColumnEx94.UnitField = null;
		this.gridColumnEx94.Visible = true;
		this.gridColumnEx94.VisibleIndex = 0;
		this.btn_DocNumDo.AutoHeight = false;
		this.btn_DocNumDo.AutoLoadDataSource = true;
		editorButtonImageOptions17.Image = Apzon.Forms.Properties.Resources.right;
		this.btn_DocNumDo.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions17, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance32, appearanceHovered32, appearancePressed32, appearanceDisabled32, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_DocNumDo.ContainerControl = this;
		this.btn_DocNumDo.DisplayMember = "";
		this.btn_DocNumDo.DisplayMember2 = "";
		this.btn_DocNumDo.LoadDataSourceWhenEnter = false;
		this.btn_DocNumDo.Name = "btn_DocNumDo";
		this.btn_DocNumDo.ReadOnly = true;
		this.btn_DocNumDo.SeparateCharacterForMulti = ';';
		this.btn_DocNumDo.SeparateString = ", ";
		this.btn_DocNumDo.UsingAutoCompleteBox = false;
		this.btn_DocNumDo.UsingDataSource = true;
		this.btn_DocNumDo.ValidateWhenNoDataSource = false;
		this.btn_DocNumDo.ValueMember = "DocNum";
		this.gridColumnEx95.Caption = "ObjType";
		this.gridColumnEx95.CurrencyDateField = null;
		this.gridColumnEx95.CurrencyField = null;
		this.gridColumnEx95.CurrencyFixed = null;
		this.gridColumnEx95.CurrencyFixedRate = 0.0;
		this.gridColumnEx95.CurrencyRateField = null;
		this.gridColumnEx95.FieldName = "ObjType";
		this.gridColumnEx95.Name = "gridColumnEx95";
		this.gridColumnEx95.OptionsColumn.ReadOnly = true;
		this.gridColumnEx95.PrimaryKey = false;
		this.gridColumnEx95.UnitField = null;
		this.gridColumnEx96.Caption = "Total";
		this.gridColumnEx96.ColumnEdit = this.repositoryItemCalcEditEx4;
		this.gridColumnEx96.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gridColumnEx96.CurrencyDateField = null;
		this.gridColumnEx96.CurrencyField = "Currency";
		this.gridColumnEx96.CurrencyFixed = null;
		this.gridColumnEx96.CurrencyFixedRate = 0.0;
		this.gridColumnEx96.CurrencyRateField = null;
		this.gridColumnEx96.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gridColumnEx96.FieldName = "Total";
		this.gridColumnEx96.Name = "gridColumnEx96";
		this.gridColumnEx96.OptionsColumn.ReadOnly = true;
		this.gridColumnEx96.PrimaryKey = false;
		this.gridColumnEx96.UnitField = null;
		this.gridColumnEx96.Visible = true;
		this.gridColumnEx96.VisibleIndex = 3;
		this.gridColumnEx97.Caption = "Remark";
		this.gridColumnEx97.CurrencyDateField = null;
		this.gridColumnEx97.CurrencyField = null;
		this.gridColumnEx97.CurrencyFixed = null;
		this.gridColumnEx97.CurrencyFixedRate = 0.0;
		this.gridColumnEx97.CurrencyRateField = null;
		this.gridColumnEx97.FieldName = "Remark";
		this.gridColumnEx97.Name = "gridColumnEx97";
		this.gridColumnEx97.OptionsColumn.ReadOnly = true;
		this.gridColumnEx97.PrimaryKey = false;
		this.gridColumnEx97.UnitField = null;
		this.gridColumnEx97.Visible = true;
		this.gridColumnEx97.VisibleIndex = 4;
		this.gridColumnEx98.Caption = "Currency";
		this.gridColumnEx98.ColumnEdit = this.luk_CurrencyLineDu;
		this.gridColumnEx98.CurrencyDateField = null;
		this.gridColumnEx98.CurrencyField = null;
		this.gridColumnEx98.CurrencyFixed = null;
		this.gridColumnEx98.CurrencyFixedRate = 0.0;
		this.gridColumnEx98.CurrencyRateField = null;
		this.gridColumnEx98.FieldName = "Currency";
		this.gridColumnEx98.Name = "gridColumnEx98";
		this.gridColumnEx98.OptionsColumn.ReadOnly = true;
		this.gridColumnEx98.PrimaryKey = false;
		this.gridColumnEx98.UnitField = null;
		this.luk_CurrencyLineDu.AutoHeight = false;
		this.luk_CurrencyLineDu.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_CurrencyLineDu.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("CurrCode", "Code")
		});
		this.luk_CurrencyLineDu.DisplayMember = "CurrCode";
		this.luk_CurrencyLineDu.Name = "luk_CurrencyLineDu";
		this.luk_CurrencyLineDu.NullText = "";
		this.luk_CurrencyLineDu.ValueMember = "CurrCode";
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Campaign. No";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(1, 13);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(80, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 1;
		this.labelLine1.Utext = "Campaign. No";
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "Status";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(2, 53);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(80, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 2;
		this.labelLine2.Utext = "Status";
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "From Date";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(203, 11);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(80, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 3;
		this.labelLine3.Utext = "From Date";
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "To Date";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(203, 51);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(80, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 4;
		this.labelLine4.Utext = "To Date";
		this.txt_No.EditControl = null;
		this.txt_No.FieldName = null;
		this.txt_No.IsSearchControl = true;
		this.txt_No.Location = new System.Drawing.Point(78, 10);
		this.txt_No.Name = "txt_No";
		this.txt_No.SettingInVisible = false;
		this.txt_No.SettingReadonly = false;
		this.txt_No.Size = new System.Drawing.Size(100, 20);
		this.txt_No.TabIndex = 5;
		this.luk_StatusSearch.EditControl = null;
		this.luk_StatusSearch.FieldName = null;
		this.luk_StatusSearch.IsSearchControl = true;
		this.luk_StatusSearch.Location = new System.Drawing.Point(78, 49);
		this.luk_StatusSearch.Name = "luk_StatusSearch";
		this.luk_StatusSearch.Properties.AutoHeight = false;
		this.luk_StatusSearch.Properties.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_StatusSearch.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_StatusSearch.Properties.IsSearchControl = false;
		this.luk_StatusSearch.Properties.NullText = "";
		this.luk_StatusSearch.Properties.ShowFooter = false;
		this.luk_StatusSearch.Properties.ShowHeader = false;
		this.luk_StatusSearch.SettingInVisible = false;
		this.luk_StatusSearch.SettingReadonly = false;
		this.luk_StatusSearch.Size = new System.Drawing.Size(100, 20);
		this.luk_StatusSearch.TabIndex = 6;
		this.date_FromDate.EditControl = null;
		this.date_FromDate.EditValue = null;
		this.date_FromDate.FieldName = null;
		this.date_FromDate.Location = new System.Drawing.Point(281, 8);
		this.date_FromDate.Name = "date_FromDate";
		this.date_FromDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_FromDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_FromDate.Properties.IsSearchControl = true;
		this.date_FromDate.SettingInVisible = false;
		this.date_FromDate.SettingReadonly = false;
		this.date_FromDate.Size = new System.Drawing.Size(100, 20);
		this.date_FromDate.TabIndex = 7;
		this.date_ToDate.EditControl = null;
		this.date_ToDate.EditValue = null;
		this.date_ToDate.FieldName = null;
		this.date_ToDate.Location = new System.Drawing.Point(281, 48);
		this.date_ToDate.Name = "date_ToDate";
		this.date_ToDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_ToDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_ToDate.Properties.IsSearchControl = true;
		this.date_ToDate.SettingInVisible = false;
		this.date_ToDate.SettingReadonly = false;
		this.date_ToDate.Size = new System.Drawing.Size(100, 20);
		this.date_ToDate.TabIndex = 8;
		this.btn_Search.EditControl = null;
		this.btn_Search.EditValue = "Search";
		this.btn_Search.FieldName = null;
		this.btn_Search.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Search.ImageOptions.Image");
		this.btn_Search.IsSearchControl = true;
		this.btn_Search.Location = new System.Drawing.Point(497, 47);
		this.btn_Search.Name = "btn_Search";
		this.btn_Search.ReadOnly = false;
		this.btn_Search.SettingInVisible = false;
		this.btn_Search.SettingReadonly = false;
		this.btn_Search.Size = new System.Drawing.Size(75, 23);
		this.btn_Search.TabIndex = 9;
		this.btn_Search.Text = "Search";
		this.btn_Search.Click += new System.EventHandler(this.btn_Search_Click);
		this.btn_Ok.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Ok.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Ok.ImageOptions.Image");
		this.btn_Ok.Location = new System.Drawing.Point(1, 497);
		this.btn_Ok.Name = "btn_Ok";
		this.btn_Ok.Size = new System.Drawing.Size(75, 23);
		this.btn_Ok.TabIndex = 10;
		this.btn_Ok.Text = "OK";
		this.btn_Ok.Click += new System.EventHandler(this.btn_Ok_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.Location = new System.Drawing.Point(82, 497);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 11;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.txt_Id.EditControl = null;
		this.txt_Id.FieldName = null;
		this.txt_Id.IsSearchControl = true;
		this.txt_Id.Location = new System.Drawing.Point(495, 10);
		this.txt_Id.Name = "txt_Id";
		this.txt_Id.SettingInVisible = false;
		this.txt_Id.SettingReadonly = false;
		this.txt_Id.Size = new System.Drawing.Size(100, 20);
		this.txt_Id.TabIndex = 14;
		this.labelLine5.EditControl = null;
		this.labelLine5.EditValue = "Campaign Id";
		this.labelLine5.FieldName = null;
		this.labelLine5.HideWhenEmpty = false;
		this.labelLine5.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine5.Location = new System.Drawing.Point(413, 12);
		this.labelLine5.Name = "labelLine5";
		this.labelLine5.ReadOnly = false;
		this.labelLine5.SettingInVisible = false;
		this.labelLine5.SettingReadonly = false;
		this.labelLine5.ShowUnderLine = true;
		this.labelLine5.Size = new System.Drawing.Size(80, 20);
		this.labelLine5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine5.TabIndex = 15;
		this.labelLine5.Utext = "Campaign Id";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(975, 523);
		base.Controls.Add(this.labelLine5);
		base.Controls.Add(this.txt_Id);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_Ok);
		base.Controls.Add(this.btn_Search);
		base.Controls.Add(this.date_ToDate);
		base.Controls.Add(this.date_FromDate);
		base.Controls.Add(this.luk_StatusSearch);
		base.Controls.Add(this.txt_No);
		base.Controls.Add(this.labelLine4);
		base.Controls.Add(this.labelLine3);
		base.Controls.Add(this.labelLine2);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.splitContainerControl1);
		base.FormId = "oMarketingCampaign";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_MarketingCampaign";
		this.Text = "Marketing Campaign";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_MarketingCampaign_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx9).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx10).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx11).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx8).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx6).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx7).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		this.ctm_Doc.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_TargetType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_CampaignType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_OwnerPer).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_StatusDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_TargetGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_ParentCampaign).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Stage).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_ProjectId).EndInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControlEx1).EndInit();
		this.xtraTabControlEx1.ResumeLayout(false);
		this.xtra_Bp.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Bp).EndInit();
		this.ctm_Bp.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Bp).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_BpCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_BpGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_Territory).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Qualified).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_StatusBp).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Industry).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_DocType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cke_ShowBPDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_DocNum).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_BpCustomer).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_BpVender).EndInit();
		this.xtra_Item.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Item).EndInit();
		this.ctm_Item.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Item).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_ItemCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_ItemGroup).EndInit();
		this.xtra_Partner.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Partner).EndInit();
		this.ctm_Partner.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Partner).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_PartnerId).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Orl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_RelaPartner).EndInit();
		this.xtra_Member.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Member).EndInit();
		this.ctm_Member.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Member).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_EmployeeNo).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_BranchMember).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_DepartmentMember).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Role).EndInit();
		this.xtra_Atta.ResumeLayout(false);
		this.xtra_Cost.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Cost).EndInit();
		this.ctm_Cost.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Cost).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_ItemCost).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_ItmGrpCost).EndInit();
		this.xtra_Task.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_ListCompleted).EndInit();
		this.ctm_Task.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_ListCompleted).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_bte_Closed).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_bte_Open).EndInit();
		this.xtra_Sub.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Sub).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Sub).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_DocEntrySub).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_TargetTypeSub).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_TypeSub).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_EmployeeSub).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_StatusSub).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_TargetGroupSub).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_ParentSub).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_StageSub).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_ProjectSub).EndInit();
		this.xtra_Document.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_LineDocument).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_LineDocument).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_DocNumDo).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_CurrencyLineDu).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_No.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_StatusSearch.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_FromDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_FromDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_ToDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_ToDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Id.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
