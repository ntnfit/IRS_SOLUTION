using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Controls.Form;
using Apzon.Forms.Administrator;
using Apzon.Forms.Helper;
using Apzon.Forms.HumanResources;
using Apzon.Forms.HumanResources.Salary;
using Apzon.Forms.HumanResources.TimeSheeting;
using Apzon.Forms.Inventory.InventoryTransaction;
using Apzon.Forms.Inventory.PriceLists;
using Apzon.Forms.ItemsMaster;
using Apzon.Forms.Mrps;
using Apzon.Forms.PM;
using Apzon.Forms.Production;
using Apzon.Forms.SalesOppotunity;
using Apzon.Forms.Service;
using DevExpress.Diagram.Core;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraDiagram;

namespace Apzon.Forms.Widget;

public class ucMainProcessModel : System.Windows.Forms.UserControl
{
	private IContainer components;

	private DiagramControlEx diagramControl1;

	private RadialMenu radialMenu_Sales;

	private BarLargeButtonItem barLargeButtonItem1;

	private BarManager barManager1;

	private BarDockControl barDockControlTop;

	private BarDockControl barDockControlBottom;

	private BarDockControl barDockControlLeft;

	private BarDockControl barDockControlRight;

	private BarLargeButtonItem barLargeButtonItem2;

	private BarLargeButtonItem barButtonItem1;

	private BarLargeButtonItem barButtonItem2;

	private BarLargeButtonItem barButtonItem3;

	private BarLargeButtonItem barButtonItem4;

	private BarLargeButtonItem barButtonItem5;

	private BarLargeButtonItem barButtonItem6;

	private BarLargeButtonItem barLargeButtonItem3;

	private BarLargeButtonItem barLargeButtonItem4;

	private BarLargeButtonItem barLargeButtonItem5;

	private BarLargeButtonItem barLargeButtonItem6;

	private BarLargeButtonItem barLargeButtonItem7;

	private BarLargeButtonItem barLargeButtonItem8;

	private BarLargeButtonItem barLargeButtonItem9;

	private BarLargeButtonItem barLargeButtonItem10;

	private BarLargeButtonItem barLargeButtonItem11;

	private RadialMenu radialMenu_Purchase;

	private BarLargeButtonItem barLargeButtonItem12;

	private BarLargeButtonItem barLargeButtonItem13;

	private BarLargeButtonItem barLargeButtonItem14;

	private BarLargeButtonItem barLargeButtonItem15;

	private BarLargeButtonItem barLargeButtonItem16;

	private BarLargeButtonItem barLargeButtonItem17;

	private BarLargeButtonItem barLargeButtonItem18;

	private RadialMenu radialMenu_Inventory;

	private BarLargeButtonItem barLargeButtonItem19;

	private BarLargeButtonItem barLargeButtonItem20;

	private BarLargeButtonItem barLargeButtonItem21;

	private BarLargeButtonItem barLargeButtonItem22;

	private RadialMenu radialMenu_Manufacturing;

	private BarLargeButtonItem barLargeButtonItem23;

	private BarLargeButtonItem barLargeButtonItem24;

	private BarLargeButtonItem barLargeButtonItem25;

	private RadialMenu radialMenu_Opportunity;

	private BarLargeButtonItem barLargeButtonItem26;

	private BarLargeButtonItem barLargeButtonItem27;

	private BarLargeButtonItem barLargeButtonItem28;

	private BarLargeButtonItem barLargeButtonItem29;

	private BarLargeButtonItem barLargeButtonItem30;

	private RadialMenu radialMenu_HRM;

	private BarLargeButtonItem barLargeButtonItem31;

	private BarLargeButtonItem barLargeButtonItem32;

	private BarLargeButtonItem barLargeButtonItem33;

	private RadialMenu radialMenu_Project;

	private BarLargeButtonItem barLargeButtonItem34;

	private BarLargeButtonItem barLargeButtonItem35;

	private BarLargeButtonItem barLargeButtonItem36;

	private BarLargeButtonItem barLargeButtonItem37;

	private BarLargeButtonItem barLargeButtonItem38;

	private BarLargeButtonItem barLargeButtonItem39;

	private RadialMenu radialMenu_CRM;

	private BarLargeButtonItem barLargeButtonItem40;

	private DiagramImageEx diagramImageEx1;

	private DiagramImageEx diagramImageEx2;

	private DiagramImageEx diagramImageEx3;

	private DiagramImageEx diagramImageEx4;

	private DiagramImageEx diagramImageEx5;

	private DiagramImageEx diagramImageEx6;

	private DiagramImageEx diagramImageEx7;

	private DiagramShape diagramShape1;

	private DiagramShape diagramShape2;

	private DiagramShape diagramShape3;

	private DiagramShape diagramShape4;

	private DiagramShape diagramShape5;

	private DiagramShape diagramShape6;

	private DiagramShape diagramShape7;

	private DiagramShape diagramShape8;

	private DiagramImageEx diagramImageEx8;

	private DiagramShape diagramShape9;

	private DiagramShape diagramShape10;

	private DiagramShape diagramShape11;

	private DiagramShape diagramShape12;

	private DiagramShape diagramShape13;

	private DiagramShape diagramShape14;

	private DiagramShape diagramShape15;

	private DiagramShape diagramShape16;

	private DiagramImage diagramImage1;

	public ucMainProcessModel()
	{
		InitializeComponent();
	}

	private void ucMainProcessModel_Load(object sender, EventArgs e)
	{
		try
		{
			foreach (DiagramItem item in diagramControl1.Items)
			{
				if (item is DiagramShape diagramShape && !string.IsNullOrEmpty(diagramShape.Content))
				{
					diagramShape.Appearance.BackColor = Color.Transparent;
					diagramShape.Appearance.BorderColor = Color.Transparent;
					diagramShape.Appearance.ForeColor = Color.Black;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ucMainProcessModel_Resize(object sender, EventArgs e)
	{
		try
		{
			diagramControl1.FitToDrawing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void diagramControl1_ShowingEditor(object sender, DiagramShowingEditorEventArgs e)
	{
		try
		{
			_ = e.Item is DiagramShape;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void diagramControl1_MouseClick(object sender, MouseEventArgs e)
	{
		try
		{
			if (!(diagramControl1.CalcHitItem(e.Location) is DiagramImageEx diagramImageEx))
			{
				return;
			}
			XtraFormEx xtraFormEx = FindForm() as XtraFormEx;
			switch (diagramImageEx.ObjectGroup)
			{
			case "Sales":
			{
				if ((e.Button & MouseButtons.Right) != 0)
				{
					radialMenu_Sales.ShowPopup(Cursor.Position, expanded: true);
					break;
				}
				frm_Widget frm_Widget2 = new frm_Widget();
				frm_Widget2.WidgetEntry = -1;
				if (xtraFormEx != null)
				{
					frm_Widget2.MdiParent = xtraFormEx.MdiParent;
				}
				frm_Widget2.Show();
				break;
			}
			case "Purchase":
				if ((e.Button & MouseButtons.Right) != 0)
				{
					radialMenu_Purchase.ShowPopup(Cursor.Position, expanded: true);
				}
				break;
			case "Inventory":
				if ((e.Button & MouseButtons.Right) != 0)
				{
					radialMenu_Inventory.ShowPopup(Cursor.Position, expanded: true);
				}
				break;
			case "Manufacturing":
				if ((e.Button & MouseButtons.Right) != 0)
				{
					radialMenu_Manufacturing.ShowPopup(Cursor.Position, expanded: true);
				}
				break;
			case "HRM":
				if ((e.Button & MouseButtons.Right) != 0)
				{
					radialMenu_HRM.ShowPopup(Cursor.Position, expanded: true);
				}
				break;
			case "CRM":
				if ((e.Button & MouseButtons.Right) != 0)
				{
					radialMenu_CRM.ShowPopup(Cursor.Position, expanded: true);
				}
				break;
			case "Project":
				if ((e.Button & MouseButtons.Right) != 0)
				{
					radialMenu_Project.ShowPopup(Cursor.Position, expanded: true);
				}
				break;
			case "Opportunity":
				if ((e.Button & MouseButtons.Right) != 0)
				{
					radialMenu_Opportunity.ShowPopup(Cursor.Position, expanded: true);
				}
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void barManager1_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			if (!(e.Item is BarLargeButtonItem))
			{
				return;
			}
			string text = Function.ToString(((BarLargeButtonItem)e.Item).Tag);
			XtraFormEx xtraFormEx = FindForm() as XtraFormEx;
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			switch (text)
			{
			case "23":
				this.ShowDocumentForm(ApzObjectTypes.oQuotations);
				break;
			case "17":
				this.ShowDocumentForm(ApzObjectTypes.oOrders);
				break;
			case "13Y":
				this.ShowDocumentForm(ApzObjectTypes.oReserveInvoices);
				break;
			case "15":
				this.ShowDocumentForm(ApzObjectTypes.oDeliveryNotes);
				break;
			case "16":
				this.ShowDocumentForm(ApzObjectTypes.oReturns);
				break;
			case "13":
				this.ShowDocumentForm(ApzObjectTypes.oInvoices);
				break;
			case "14":
				this.ShowDocumentForm(ApzObjectTypes.oCreditNotes);
				break;
			case "1470000113":
				this.ShowDocumentForm(ApzObjectTypes.oPurchaseRequest);
				break;
			case "540000006":
				this.ShowDocumentForm(ApzObjectTypes.oPurchaseQuotations);
				break;
			case "22":
				this.ShowDocumentForm(ApzObjectTypes.oPurchaseOrders);
				break;
			case "20":
				this.ShowDocumentForm(ApzObjectTypes.oPurchaseDeliveryNotes);
				break;
			case "21":
				this.ShowDocumentForm(ApzObjectTypes.oPurchaseReturns);
				break;
			case "18":
				this.ShowDocumentForm(ApzObjectTypes.oPurchaseInvoices);
				break;
			case "19":
				this.ShowDocumentForm(ApzObjectTypes.oPurchaseCreditNotes);
				break;
			case "18Y":
				this.ShowDocumentForm(ApzObjectTypes.oAPReserveInvoice);
				break;
			case "4":
			{
				frm_ItemMasterData frm_ItemMasterData2 = new frm_ItemMasterData();
				if (xtraFormEx != null)
				{
					frm_ItemMasterData2.MdiParent = xtraFormEx.MdiParent;
				}
				frm_ItemMasterData2.Show();
				break;
			}
			case "PriceLists":
			{
				frm_STDPriceLists frm_STDPriceLists2 = new frm_STDPriceLists();
				if (xtraFormEx != null)
				{
					frm_STDPriceLists2.MdiParent = xtraFormEx.MdiParent;
				}
				frm_STDPriceLists2.Show();
				break;
			}
			case "1250000001":
			{
				frm_InventoryTranferRequest frm_InventoryTranferRequest2 = new frm_InventoryTranferRequest();
				if (xtraFormEx != null)
				{
					frm_InventoryTranferRequest2.MdiParent = xtraFormEx.MdiParent;
				}
				frm_InventoryTranferRequest2.Show();
				break;
			}
			case "67":
			{
				frm_InventoryTranfer frm_InventoryTranfer2 = new frm_InventoryTranfer();
				if (xtraFormEx != null)
				{
					frm_InventoryTranfer2.MdiParent = xtraFormEx.MdiParent;
				}
				frm_InventoryTranfer2.Show();
				break;
			}
			case "59":
			{
				frm_GoodsReceipt frm_GoodsReceipt2 = new frm_GoodsReceipt();
				if (xtraFormEx != null)
				{
					frm_GoodsReceipt2.MdiParent = xtraFormEx.MdiParent;
				}
				frm_GoodsReceipt2.Show();
				break;
			}
			case "60":
			{
				frm_GoodsIssue frm_GoodsIssue2 = new frm_GoodsIssue();
				if (xtraFormEx != null)
				{
					frm_GoodsIssue2.MdiParent = xtraFormEx.MdiParent;
				}
				frm_GoodsIssue2.Show();
				break;
			}
			case "66":
			{
				frm_BillOfMaterials frm_BillOfMaterials2 = new frm_BillOfMaterials();
				if (xtraFormEx != null)
				{
					frm_BillOfMaterials2.MdiParent = xtraFormEx.MdiParent;
				}
				frm_BillOfMaterials2.Show();
				break;
			}
			case "202":
			{
				frm_ProductionOrder frm_ProductionOrder2 = new frm_ProductionOrder();
				if (xtraFormEx != null)
				{
					frm_ProductionOrder2.MdiParent = xtraFormEx.MdiParent;
				}
				frm_ProductionOrder2.Show();
				break;
			}
			case "8000005":
			{
				frm_AdvanceForecast frm_AdvanceForecast2 = new frm_AdvanceForecast();
				if (xtraFormEx != null)
				{
					frm_AdvanceForecast2.MdiParent = xtraFormEx.MdiParent;
				}
				frm_AdvanceForecast2.Show();
				break;
			}
			case "700003":
			{
				frm_MRP frm_MRP2 = new frm_MRP();
				if (xtraFormEx != null)
				{
					frm_MRP2.MdiParent = xtraFormEx.MdiParent;
				}
				frm_MRP2.Show();
				break;
			}
			case "800004":
			{
				frm_Schedule frm_Schedule2 = new frm_Schedule();
				if (xtraFormEx != null)
				{
					frm_Schedule2.MdiParent = xtraFormEx.MdiParent;
				}
				frm_Schedule2.Show();
				break;
			}
			case "97":
			{
				frm_SalesOpportunity frm_SalesOpportunity2 = new frm_SalesOpportunity();
				if (xtraFormEx != null)
				{
					frm_SalesOpportunity2.MdiParent = xtraFormEx.MdiParent;
				}
				frm_SalesOpportunity2.Show();
				break;
			}
			case "171":
			{
				frm_HumanResources frm_HumanResources2 = new frm_HumanResources();
				if (xtraFormEx != null)
				{
					frm_HumanResources2.MdiParent = xtraFormEx.MdiParent;
				}
				frm_HumanResources2.Show();
				break;
			}
			case "700012":
			{
				frm_JobVacancy frm_JobVacancy2 = new frm_JobVacancy();
				if (xtraFormEx != null)
				{
					frm_JobVacancy2.MdiParent = xtraFormEx.MdiParent;
				}
				frm_JobVacancy2.Show();
				break;
			}
			case "19000090":
			{
				frm_TimeSheeting frm_TimeSheeting2 = new frm_TimeSheeting();
				if (xtraFormEx != null)
				{
					frm_TimeSheeting2.MdiParent = xtraFormEx.MdiParent;
				}
				frm_TimeSheeting2.Show();
				break;
			}
			case "810008":
			{
				frm_SalaryCalculation frm_SalaryCalculation2 = new frm_SalaryCalculation();
				if (xtraFormEx != null)
				{
					frm_SalaryCalculation2.MdiParent = xtraFormEx.MdiParent;
				}
				frm_SalaryCalculation2.Show();
				break;
			}
			case "1470000132":
			{
				frm_ProjectManager frm_ProjectManager2 = new frm_ProjectManager();
				if (xtraFormEx != null)
				{
					frm_ProjectManager2.MdiParent = xtraFormEx.MdiParent;
				}
				frm_ProjectManager2.Show();
				break;
			}
			case "1470000130":
			{
				frm_TrackingIssue frm_TrackingIssue2 = new frm_TrackingIssue();
				if (xtraFormEx != null)
				{
					frm_TrackingIssue2.MdiParent = xtraFormEx.MdiParent;
				}
				frm_TrackingIssue2.Show();
				break;
			}
			case "1470000128":
			{
				frm_TaskManagement frm_TaskManagement2 = new frm_TaskManagement();
				if (xtraFormEx != null)
				{
					frm_TaskManagement2.MdiParent = xtraFormEx.MdiParent;
				}
				frm_TaskManagement2.Show();
				break;
			}
			case "1470000129":
			{
				frm_EventManagement frm_EventManagement2 = new frm_EventManagement();
				if (xtraFormEx != null)
				{
					frm_EventManagement2.MdiParent = xtraFormEx.MdiParent;
				}
				frm_EventManagement2.Show();
				break;
			}
			case "19000102":
			{
				frm_NoteManagement frm_NoteManagement2 = new frm_NoteManagement();
				if (xtraFormEx != null)
				{
					frm_NoteManagement2.MdiParent = xtraFormEx.MdiParent;
				}
				frm_NoteManagement2.Show();
				break;
			}
			case "Calendar":
			{
				frm_ActivityCalendar frm_ActivityCalendar2 = new frm_ActivityCalendar();
				if (xtraFormEx != null)
				{
					frm_ActivityCalendar2.MdiParent = xtraFormEx.MdiParent;
				}
				frm_ActivityCalendar2.Show();
				break;
			}
			case "189":
			{
				frm_Solution frm_Solution2 = new frm_Solution();
				if (xtraFormEx != null)
				{
					frm_Solution2.MdiParent = xtraFormEx.MdiParent;
				}
				frm_Solution2.Show();
				break;
			}
			case "SalesReport":
				break;
			case "PurchasingReport":
				break;
			case "ProductionReports":
				break;
			case "OpportunitiesReports":
				break;
			case "HRMReports":
				break;
			case "ProjectReports":
				break;
			case "CRMReports":
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
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
		this.components = new System.ComponentModel.Container();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Widget.ucMainProcessModel));
		this.diagramControl1 = new Apzon.Forms.Widget.DiagramControlEx();
		this.radialMenu_Sales = new DevExpress.XtraBars.Ribbon.RadialMenu(this.components);
		this.barLargeButtonItem1 = new DevExpress.XtraBars.BarLargeButtonItem();
		this.barLargeButtonItem2 = new DevExpress.XtraBars.BarLargeButtonItem();
		this.barButtonItem1 = new DevExpress.XtraBars.BarLargeButtonItem();
		this.barButtonItem2 = new DevExpress.XtraBars.BarLargeButtonItem();
		this.barButtonItem3 = new DevExpress.XtraBars.BarLargeButtonItem();
		this.barButtonItem4 = new DevExpress.XtraBars.BarLargeButtonItem();
		this.barButtonItem5 = new DevExpress.XtraBars.BarLargeButtonItem();
		this.barButtonItem6 = new DevExpress.XtraBars.BarLargeButtonItem();
		this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
		this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
		this.barLargeButtonItem3 = new DevExpress.XtraBars.BarLargeButtonItem();
		this.barLargeButtonItem4 = new DevExpress.XtraBars.BarLargeButtonItem();
		this.barLargeButtonItem5 = new DevExpress.XtraBars.BarLargeButtonItem();
		this.barLargeButtonItem6 = new DevExpress.XtraBars.BarLargeButtonItem();
		this.barLargeButtonItem7 = new DevExpress.XtraBars.BarLargeButtonItem();
		this.barLargeButtonItem8 = new DevExpress.XtraBars.BarLargeButtonItem();
		this.barLargeButtonItem9 = new DevExpress.XtraBars.BarLargeButtonItem();
		this.barLargeButtonItem10 = new DevExpress.XtraBars.BarLargeButtonItem();
		this.barLargeButtonItem11 = new DevExpress.XtraBars.BarLargeButtonItem();
		this.barLargeButtonItem12 = new DevExpress.XtraBars.BarLargeButtonItem();
		this.barLargeButtonItem13 = new DevExpress.XtraBars.BarLargeButtonItem();
		this.barLargeButtonItem14 = new DevExpress.XtraBars.BarLargeButtonItem();
		this.barLargeButtonItem15 = new DevExpress.XtraBars.BarLargeButtonItem();
		this.barLargeButtonItem16 = new DevExpress.XtraBars.BarLargeButtonItem();
		this.barLargeButtonItem17 = new DevExpress.XtraBars.BarLargeButtonItem();
		this.barLargeButtonItem18 = new DevExpress.XtraBars.BarLargeButtonItem();
		this.barLargeButtonItem19 = new DevExpress.XtraBars.BarLargeButtonItem();
		this.barLargeButtonItem20 = new DevExpress.XtraBars.BarLargeButtonItem();
		this.barLargeButtonItem21 = new DevExpress.XtraBars.BarLargeButtonItem();
		this.barLargeButtonItem22 = new DevExpress.XtraBars.BarLargeButtonItem();
		this.barLargeButtonItem23 = new DevExpress.XtraBars.BarLargeButtonItem();
		this.barLargeButtonItem24 = new DevExpress.XtraBars.BarLargeButtonItem();
		this.barLargeButtonItem25 = new DevExpress.XtraBars.BarLargeButtonItem();
		this.barLargeButtonItem26 = new DevExpress.XtraBars.BarLargeButtonItem();
		this.barLargeButtonItem27 = new DevExpress.XtraBars.BarLargeButtonItem();
		this.barLargeButtonItem28 = new DevExpress.XtraBars.BarLargeButtonItem();
		this.barLargeButtonItem29 = new DevExpress.XtraBars.BarLargeButtonItem();
		this.barLargeButtonItem30 = new DevExpress.XtraBars.BarLargeButtonItem();
		this.barLargeButtonItem31 = new DevExpress.XtraBars.BarLargeButtonItem();
		this.barLargeButtonItem32 = new DevExpress.XtraBars.BarLargeButtonItem();
		this.barLargeButtonItem33 = new DevExpress.XtraBars.BarLargeButtonItem();
		this.barLargeButtonItem34 = new DevExpress.XtraBars.BarLargeButtonItem();
		this.barLargeButtonItem35 = new DevExpress.XtraBars.BarLargeButtonItem();
		this.barLargeButtonItem36 = new DevExpress.XtraBars.BarLargeButtonItem();
		this.barLargeButtonItem37 = new DevExpress.XtraBars.BarLargeButtonItem();
		this.barLargeButtonItem38 = new DevExpress.XtraBars.BarLargeButtonItem();
		this.barLargeButtonItem39 = new DevExpress.XtraBars.BarLargeButtonItem();
		this.barLargeButtonItem40 = new DevExpress.XtraBars.BarLargeButtonItem();
		this.radialMenu_Purchase = new DevExpress.XtraBars.Ribbon.RadialMenu(this.components);
		this.radialMenu_Inventory = new DevExpress.XtraBars.Ribbon.RadialMenu(this.components);
		this.radialMenu_Manufacturing = new DevExpress.XtraBars.Ribbon.RadialMenu(this.components);
		this.radialMenu_Opportunity = new DevExpress.XtraBars.Ribbon.RadialMenu(this.components);
		this.radialMenu_HRM = new DevExpress.XtraBars.Ribbon.RadialMenu(this.components);
		this.radialMenu_Project = new DevExpress.XtraBars.Ribbon.RadialMenu(this.components);
		this.radialMenu_CRM = new DevExpress.XtraBars.Ribbon.RadialMenu(this.components);
		this.diagramImageEx1 = new Apzon.Forms.Widget.DiagramImageEx();
		this.diagramImageEx2 = new Apzon.Forms.Widget.DiagramImageEx();
		this.diagramImageEx3 = new Apzon.Forms.Widget.DiagramImageEx();
		this.diagramImageEx4 = new Apzon.Forms.Widget.DiagramImageEx();
		this.diagramImageEx5 = new Apzon.Forms.Widget.DiagramImageEx();
		this.diagramImageEx6 = new Apzon.Forms.Widget.DiagramImageEx();
		this.diagramImageEx7 = new Apzon.Forms.Widget.DiagramImageEx();
		this.diagramShape1 = new DevExpress.XtraDiagram.DiagramShape();
		this.diagramShape2 = new DevExpress.XtraDiagram.DiagramShape();
		this.diagramShape3 = new DevExpress.XtraDiagram.DiagramShape();
		this.diagramShape4 = new DevExpress.XtraDiagram.DiagramShape();
		this.diagramShape5 = new DevExpress.XtraDiagram.DiagramShape();
		this.diagramShape6 = new DevExpress.XtraDiagram.DiagramShape();
		this.diagramShape7 = new DevExpress.XtraDiagram.DiagramShape();
		this.diagramShape8 = new DevExpress.XtraDiagram.DiagramShape();
		this.diagramImageEx8 = new Apzon.Forms.Widget.DiagramImageEx();
		this.diagramShape9 = new DevExpress.XtraDiagram.DiagramShape();
		this.diagramShape10 = new DevExpress.XtraDiagram.DiagramShape();
		this.diagramShape11 = new DevExpress.XtraDiagram.DiagramShape();
		this.diagramShape12 = new DevExpress.XtraDiagram.DiagramShape();
		this.diagramShape13 = new DevExpress.XtraDiagram.DiagramShape();
		this.diagramShape14 = new DevExpress.XtraDiagram.DiagramShape();
		this.diagramShape15 = new DevExpress.XtraDiagram.DiagramShape();
		this.diagramShape16 = new DevExpress.XtraDiagram.DiagramShape();
		this.diagramImage1 = new DevExpress.XtraDiagram.DiagramImage();
		((System.ComponentModel.ISupportInitialize)this.diagramControl1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.radialMenu_Sales).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.barManager1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.radialMenu_Purchase).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.radialMenu_Inventory).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.radialMenu_Manufacturing).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.radialMenu_Opportunity).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.radialMenu_HRM).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.radialMenu_Project).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.radialMenu_CRM).BeginInit();
		base.SuspendLayout();
		this.diagramControl1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
		this.diagramControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.diagramControl1.Items.AddRange(this.diagramImageEx1, this.diagramImageEx2, this.diagramImageEx3, this.diagramImageEx4, this.diagramImageEx5, this.diagramImageEx6, this.diagramImageEx7, this.diagramShape1, this.diagramShape2, this.diagramShape3, this.diagramShape4, this.diagramShape5, this.diagramShape6, this.diagramShape7, this.diagramShape8, this.diagramImageEx8, this.diagramShape9, this.diagramShape10, this.diagramShape11, this.diagramShape12, this.diagramShape13, this.diagramShape14, this.diagramShape15, this.diagramShape16, this.diagramImage1);
		this.diagramControl1.Location = new System.Drawing.Point(0, 0);
		this.diagramControl1.Margin = new System.Windows.Forms.Padding(0);
		this.diagramControl1.Name = "diagramControl1";
		this.diagramControl1.OptionsBehavior.ResizingMode = DevExpress.Diagram.Core.ResizingMode.Preview;
		this.diagramControl1.OptionsBehavior.SelectedStencils = new DevExpress.Diagram.Core.StencilCollection("BasicShapes", "BasicFlowchartShapes", "ArrowShapes", "SDLDiagramShapes", "SoftwareIcons", "DecorativeShapes");
		this.diagramControl1.OptionsView.AllowShapeShadows = true;
		this.diagramControl1.OptionsView.CanvasSizeMode = DevExpress.Diagram.Core.CanvasSizeMode.Fill;
		this.diagramControl1.OptionsView.PageMargin = new System.Windows.Forms.Padding(0);
		this.diagramControl1.OptionsView.PageSize = new System.Drawing.SizeF(1123f, 794f);
		this.diagramControl1.OptionsView.PaperKind = System.Drawing.Printing.PaperKind.A4;
		this.diagramControl1.OptionsView.ShowGrid = false;
		this.diagramControl1.OptionsView.ShowPageBreaks = false;
		this.diagramControl1.OptionsView.ShowRulers = false;
		this.diagramControl1.OptionsView.Theme = DevExpress.Diagram.Core.DiagramThemes.Parallel;
		this.diagramControl1.Size = new System.Drawing.Size(1273, 734);
		this.diagramControl1.TabIndex = 1;
		this.diagramControl1.Text = "diagramControl1";
		this.diagramControl1.ShowingEditor += new System.EventHandler<DevExpress.XtraDiagram.DiagramShowingEditorEventArgs>(this.diagramControl1_ShowingEditor);
		this.diagramControl1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.diagramControl1_MouseClick);
		this.radialMenu_Sales.AutoExpand = true;
		this.radialMenu_Sales.Glyph = (System.Drawing.Image)resources.GetObject("radialMenu_Sales.Glyph");
		this.radialMenu_Sales.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[8]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.barLargeButtonItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.barLargeButtonItem2),
			new DevExpress.XtraBars.LinkPersistInfo(this.barButtonItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.barButtonItem2),
			new DevExpress.XtraBars.LinkPersistInfo(this.barButtonItem3),
			new DevExpress.XtraBars.LinkPersistInfo(this.barButtonItem4),
			new DevExpress.XtraBars.LinkPersistInfo(this.barButtonItem5),
			new DevExpress.XtraBars.LinkPersistInfo(this.barButtonItem6)
		});
		this.radialMenu_Sales.Manager = this.barManager1;
		this.radialMenu_Sales.MenuColor = System.Drawing.Color.Teal;
		this.radialMenu_Sales.Name = "radialMenu_Sales";
		this.radialMenu_Sales.SubMenuHoverColor = System.Drawing.Color.Olive;
		this.barLargeButtonItem1.Caption = "Quotations";
		this.barLargeButtonItem1.Id = 0;
		this.barLargeButtonItem1.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("barLargeButtonItem1.ImageOptions.Image");
		this.barLargeButtonItem1.Name = "barLargeButtonItem1";
		this.barLargeButtonItem1.Tag = "23";
		this.barLargeButtonItem2.Caption = "Orders";
		this.barLargeButtonItem2.Id = 1;
		this.barLargeButtonItem2.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("barLargeButtonItem2.ImageOptions.Image");
		this.barLargeButtonItem2.Name = "barLargeButtonItem2";
		this.barLargeButtonItem2.Tag = "17";
		this.barButtonItem1.Caption = "Reserve Invoices";
		this.barButtonItem1.Id = 2;
		this.barButtonItem1.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("barButtonItem1.ImageOptions.Image");
		this.barButtonItem1.Name = "barButtonItem1";
		this.barButtonItem1.Tag = "13Y";
		this.barButtonItem2.Caption = "Deliveries";
		this.barButtonItem2.Id = 3;
		this.barButtonItem2.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("barButtonItem2.ImageOptions.Image");
		this.barButtonItem2.Name = "barButtonItem2";
		this.barButtonItem2.Tag = "15";
		this.barButtonItem3.Caption = "Returns";
		this.barButtonItem3.Id = 4;
		this.barButtonItem3.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("barButtonItem3.ImageOptions.Image");
		this.barButtonItem3.Name = "barButtonItem3";
		this.barButtonItem3.Tag = "16";
		this.barButtonItem4.Caption = "Invoices";
		this.barButtonItem4.Id = 5;
		this.barButtonItem4.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("barButtonItem4.ImageOptions.Image");
		this.barButtonItem4.Name = "barButtonItem4";
		this.barButtonItem4.Tag = "13";
		this.barButtonItem5.Caption = "Credit Memos";
		this.barButtonItem5.Id = 6;
		this.barButtonItem5.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("barButtonItem5.ImageOptions.Image");
		this.barButtonItem5.Name = "barButtonItem5";
		this.barButtonItem5.Tag = "14";
		this.barButtonItem6.Caption = "Reports";
		this.barButtonItem6.Id = 7;
		this.barButtonItem6.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("barButtonItem6.ImageOptions.Image");
		this.barButtonItem6.Name = "barButtonItem6";
		this.barButtonItem6.Tag = "SalesReport";
		this.barManager1.DockControls.Add(this.barDockControlTop);
		this.barManager1.DockControls.Add(this.barDockControlBottom);
		this.barManager1.DockControls.Add(this.barDockControlLeft);
		this.barManager1.DockControls.Add(this.barDockControlRight);
		this.barManager1.Form = this;
		this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[46]
		{
			this.barLargeButtonItem1, this.barLargeButtonItem2, this.barButtonItem1, this.barButtonItem2, this.barButtonItem3, this.barButtonItem4, this.barButtonItem5, this.barButtonItem6, this.barLargeButtonItem3, this.barLargeButtonItem4,
			this.barLargeButtonItem5, this.barLargeButtonItem6, this.barLargeButtonItem7, this.barLargeButtonItem8, this.barLargeButtonItem9, this.barLargeButtonItem10, this.barLargeButtonItem11, this.barLargeButtonItem12, this.barLargeButtonItem13, this.barLargeButtonItem14,
			this.barLargeButtonItem15, this.barLargeButtonItem16, this.barLargeButtonItem17, this.barLargeButtonItem18, this.barLargeButtonItem19, this.barLargeButtonItem20, this.barLargeButtonItem21, this.barLargeButtonItem22, this.barLargeButtonItem23, this.barLargeButtonItem24,
			this.barLargeButtonItem25, this.barLargeButtonItem26, this.barLargeButtonItem27, this.barLargeButtonItem28, this.barLargeButtonItem29, this.barLargeButtonItem30, this.barLargeButtonItem31, this.barLargeButtonItem32, this.barLargeButtonItem33, this.barLargeButtonItem34,
			this.barLargeButtonItem35, this.barLargeButtonItem36, this.barLargeButtonItem37, this.barLargeButtonItem38, this.barLargeButtonItem39, this.barLargeButtonItem40
		});
		this.barManager1.MaxItemId = 46;
		this.barManager1.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barManager1_ItemClick);
		this.barDockControlTop.CausesValidation = false;
		this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
		this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
		this.barDockControlTop.Manager = this.barManager1;
		this.barDockControlTop.Size = new System.Drawing.Size(1273, 0);
		this.barDockControlBottom.CausesValidation = false;
		this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.barDockControlBottom.Location = new System.Drawing.Point(0, 734);
		this.barDockControlBottom.Manager = this.barManager1;
		this.barDockControlBottom.Size = new System.Drawing.Size(1273, 0);
		this.barDockControlLeft.CausesValidation = false;
		this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
		this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
		this.barDockControlLeft.Manager = this.barManager1;
		this.barDockControlLeft.Size = new System.Drawing.Size(0, 734);
		this.barDockControlRight.CausesValidation = false;
		this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
		this.barDockControlRight.Location = new System.Drawing.Point(1273, 0);
		this.barDockControlRight.Manager = this.barManager1;
		this.barDockControlRight.Size = new System.Drawing.Size(0, 734);
		this.barLargeButtonItem3.Caption = "Purchase Request";
		this.barLargeButtonItem3.Id = 8;
		this.barLargeButtonItem3.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("barLargeButtonItem3.ImageOptions.Image");
		this.barLargeButtonItem3.Name = "barLargeButtonItem3";
		this.barLargeButtonItem3.Tag = "1470000113";
		this.barLargeButtonItem4.Caption = "Purchase Quotations";
		this.barLargeButtonItem4.Id = 9;
		this.barLargeButtonItem4.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("barLargeButtonItem4.ImageOptions.Image");
		this.barLargeButtonItem4.Name = "barLargeButtonItem4";
		this.barLargeButtonItem4.Tag = "540000006";
		this.barLargeButtonItem5.Caption = "Purchase Orders";
		this.barLargeButtonItem5.Id = 10;
		this.barLargeButtonItem5.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("barLargeButtonItem5.ImageOptions.Image");
		this.barLargeButtonItem5.Name = "barLargeButtonItem5";
		this.barLargeButtonItem5.Tag = "22";
		this.barLargeButtonItem6.Caption = "Goods Receipts PO";
		this.barLargeButtonItem6.Id = 11;
		this.barLargeButtonItem6.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("barLargeButtonItem6.ImageOptions.Image");
		this.barLargeButtonItem6.Name = "barLargeButtonItem6";
		this.barLargeButtonItem6.Tag = "20";
		this.barLargeButtonItem7.Caption = "Goods Returns";
		this.barLargeButtonItem7.Id = 12;
		this.barLargeButtonItem7.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("barLargeButtonItem7.ImageOptions.Image");
		this.barLargeButtonItem7.Name = "barLargeButtonItem7";
		this.barLargeButtonItem7.Tag = "21";
		this.barLargeButtonItem8.Caption = "A/P Invoices";
		this.barLargeButtonItem8.Id = 13;
		this.barLargeButtonItem8.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("barLargeButtonItem8.ImageOptions.Image");
		this.barLargeButtonItem8.Name = "barLargeButtonItem8";
		this.barLargeButtonItem8.Tag = "18";
		this.barLargeButtonItem9.Caption = "A/P Credit Memos";
		this.barLargeButtonItem9.Id = 14;
		this.barLargeButtonItem9.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("barLargeButtonItem9.ImageOptions.Image");
		this.barLargeButtonItem9.Name = "barLargeButtonItem9";
		this.barLargeButtonItem9.Tag = "19";
		this.barLargeButtonItem10.Caption = "Reports";
		this.barLargeButtonItem10.Id = 15;
		this.barLargeButtonItem10.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("barLargeButtonItem10.ImageOptions.Image");
		this.barLargeButtonItem10.Name = "barLargeButtonItem10";
		this.barLargeButtonItem10.Tag = "PurchasingReport";
		this.barLargeButtonItem11.Caption = "A/P Reserve Invoices";
		this.barLargeButtonItem11.Id = 16;
		this.barLargeButtonItem11.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("barLargeButtonItem11.ImageOptions.Image");
		this.barLargeButtonItem11.Name = "barLargeButtonItem11";
		this.barLargeButtonItem11.Tag = "18Y";
		this.barLargeButtonItem12.Caption = "Items";
		this.barLargeButtonItem12.Id = 17;
		this.barLargeButtonItem12.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("barLargeButtonItem12.ImageOptions.Image");
		this.barLargeButtonItem12.Name = "barLargeButtonItem12";
		this.barLargeButtonItem12.Tag = "4";
		this.barLargeButtonItem13.Caption = "Price Lists";
		this.barLargeButtonItem13.Id = 18;
		this.barLargeButtonItem13.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("barLargeButtonItem13.ImageOptions.Image");
		this.barLargeButtonItem13.Name = "barLargeButtonItem13";
		this.barLargeButtonItem13.Tag = "PriceLists";
		this.barLargeButtonItem14.Caption = "Transfer Request";
		this.barLargeButtonItem14.Id = 19;
		this.barLargeButtonItem14.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("barLargeButtonItem14.ImageOptions.Image");
		this.barLargeButtonItem14.Name = "barLargeButtonItem14";
		this.barLargeButtonItem14.Tag = "1250000001";
		this.barLargeButtonItem15.Caption = "Transfer";
		this.barLargeButtonItem15.Id = 20;
		this.barLargeButtonItem15.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("barLargeButtonItem15.ImageOptions.Image");
		this.barLargeButtonItem15.Name = "barLargeButtonItem15";
		this.barLargeButtonItem15.Tag = "67";
		this.barLargeButtonItem16.Caption = "Goods Receipt";
		this.barLargeButtonItem16.Id = 21;
		this.barLargeButtonItem16.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("barLargeButtonItem16.ImageOptions.Image");
		this.barLargeButtonItem16.Name = "barLargeButtonItem16";
		this.barLargeButtonItem16.Tag = "59";
		this.barLargeButtonItem17.Caption = "Goods Issue";
		this.barLargeButtonItem17.Id = 22;
		this.barLargeButtonItem17.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("barLargeButtonItem17.ImageOptions.Image");
		this.barLargeButtonItem17.Name = "barLargeButtonItem17";
		this.barLargeButtonItem17.Tag = "60";
		this.barLargeButtonItem18.Caption = "Reports";
		this.barLargeButtonItem18.Id = 23;
		this.barLargeButtonItem18.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("barLargeButtonItem18.ImageOptions.Image");
		this.barLargeButtonItem18.Name = "barLargeButtonItem18";
		this.barLargeButtonItem18.Tag = "InventoryReports";
		this.barLargeButtonItem19.Caption = "BOM";
		this.barLargeButtonItem19.Id = 24;
		this.barLargeButtonItem19.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("barLargeButtonItem19.ImageOptions.Image");
		this.barLargeButtonItem19.Name = "barLargeButtonItem19";
		this.barLargeButtonItem19.Tag = "66";
		this.barLargeButtonItem20.Caption = "Production Orders";
		this.barLargeButtonItem20.Id = 25;
		this.barLargeButtonItem20.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("barLargeButtonItem20.ImageOptions.Image");
		this.barLargeButtonItem20.Name = "barLargeButtonItem20";
		this.barLargeButtonItem20.Tag = "202";
		this.barLargeButtonItem21.Caption = "Forecasts";
		this.barLargeButtonItem21.Id = 26;
		this.barLargeButtonItem21.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("barLargeButtonItem21.ImageOptions.Image");
		this.barLargeButtonItem21.Name = "barLargeButtonItem21";
		this.barLargeButtonItem21.Tag = "8000005";
		this.barLargeButtonItem22.Caption = "MRP";
		this.barLargeButtonItem22.Id = 27;
		this.barLargeButtonItem22.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("barLargeButtonItem22.ImageOptions.Image");
		this.barLargeButtonItem22.Name = "barLargeButtonItem22";
		this.barLargeButtonItem22.Tag = "700003";
		this.barLargeButtonItem23.Caption = "Reports";
		this.barLargeButtonItem23.Id = 28;
		this.barLargeButtonItem23.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("barLargeButtonItem23.ImageOptions.Image");
		this.barLargeButtonItem23.Name = "barLargeButtonItem23";
		this.barLargeButtonItem23.Tag = "ProductionReports";
		this.barLargeButtonItem24.Caption = "Opportunities";
		this.barLargeButtonItem24.Id = 29;
		this.barLargeButtonItem24.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("barLargeButtonItem24.ImageOptions.Image");
		this.barLargeButtonItem24.Name = "barLargeButtonItem24";
		this.barLargeButtonItem24.Tag = "97";
		this.barLargeButtonItem25.Caption = "Reports";
		this.barLargeButtonItem25.Id = 30;
		this.barLargeButtonItem25.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("barLargeButtonItem25.ImageOptions.Image");
		this.barLargeButtonItem25.Name = "barLargeButtonItem25";
		this.barLargeButtonItem25.Tag = "OpportunitiesReports";
		this.barLargeButtonItem26.Caption = "Employees";
		this.barLargeButtonItem26.Id = 31;
		this.barLargeButtonItem26.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("barLargeButtonItem26.ImageOptions.Image");
		this.barLargeButtonItem26.Name = "barLargeButtonItem26";
		this.barLargeButtonItem26.Tag = "171";
		this.barLargeButtonItem27.Caption = "Job Vacancy";
		this.barLargeButtonItem27.Id = 32;
		this.barLargeButtonItem27.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("barLargeButtonItem27.ImageOptions.Image");
		this.barLargeButtonItem27.Name = "barLargeButtonItem27";
		this.barLargeButtonItem27.Tag = "700012";
		this.barLargeButtonItem28.Caption = "Time Sheeting";
		this.barLargeButtonItem28.Id = 33;
		this.barLargeButtonItem28.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("barLargeButtonItem28.ImageOptions.Image");
		this.barLargeButtonItem28.Name = "barLargeButtonItem28";
		this.barLargeButtonItem28.Tag = "19000090";
		this.barLargeButtonItem29.Caption = "Salaries";
		this.barLargeButtonItem29.Id = 34;
		this.barLargeButtonItem29.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("barLargeButtonItem29.ImageOptions.Image");
		this.barLargeButtonItem29.Name = "barLargeButtonItem29";
		this.barLargeButtonItem29.Tag = "810008";
		this.barLargeButtonItem30.Caption = "Reports";
		this.barLargeButtonItem30.Id = 35;
		this.barLargeButtonItem30.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("barLargeButtonItem30.ImageOptions.Image");
		this.barLargeButtonItem30.Name = "barLargeButtonItem30";
		this.barLargeButtonItem30.Tag = "HRMReports";
		this.barLargeButtonItem31.Caption = "Project Management";
		this.barLargeButtonItem31.Id = 36;
		this.barLargeButtonItem31.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("barLargeButtonItem31.ImageOptions.Image");
		this.barLargeButtonItem31.Name = "barLargeButtonItem31";
		this.barLargeButtonItem31.Tag = "1470000132";
		this.barLargeButtonItem32.Caption = "Tracking Issues";
		this.barLargeButtonItem32.Id = 37;
		this.barLargeButtonItem32.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("barLargeButtonItem32.ImageOptions.Image");
		this.barLargeButtonItem32.Name = "barLargeButtonItem32";
		this.barLargeButtonItem32.Tag = "1470000130";
		this.barLargeButtonItem33.Caption = "Reports";
		this.barLargeButtonItem33.Id = 38;
		this.barLargeButtonItem33.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("barLargeButtonItem33.ImageOptions.Image");
		this.barLargeButtonItem33.Name = "barLargeButtonItem33";
		this.barLargeButtonItem33.Tag = "ProjectReports";
		this.barLargeButtonItem34.Caption = "Task List";
		this.barLargeButtonItem34.Id = 39;
		this.barLargeButtonItem34.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("barLargeButtonItem34.ImageOptions.Image");
		this.barLargeButtonItem34.Name = "barLargeButtonItem34";
		this.barLargeButtonItem34.Tag = "1470000128";
		this.barLargeButtonItem35.Caption = "Events";
		this.barLargeButtonItem35.Id = 40;
		this.barLargeButtonItem35.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("barLargeButtonItem35.ImageOptions.Image");
		this.barLargeButtonItem35.Name = "barLargeButtonItem35";
		this.barLargeButtonItem35.Tag = "1470000129";
		this.barLargeButtonItem36.Caption = "Notes";
		this.barLargeButtonItem36.Id = 41;
		this.barLargeButtonItem36.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("barLargeButtonItem36.ImageOptions.Image");
		this.barLargeButtonItem36.Name = "barLargeButtonItem36";
		this.barLargeButtonItem36.Tag = "19000102";
		this.barLargeButtonItem37.Caption = "Calendar";
		this.barLargeButtonItem37.Id = 42;
		this.barLargeButtonItem37.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("barLargeButtonItem37.ImageOptions.Image");
		this.barLargeButtonItem37.Name = "barLargeButtonItem37";
		this.barLargeButtonItem37.Tag = "Calendar";
		this.barLargeButtonItem38.Caption = "Solutions";
		this.barLargeButtonItem38.Id = 43;
		this.barLargeButtonItem38.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("barLargeButtonItem38.ImageOptions.Image");
		this.barLargeButtonItem38.Name = "barLargeButtonItem38";
		this.barLargeButtonItem38.Tag = "189";
		this.barLargeButtonItem39.Caption = "Reports";
		this.barLargeButtonItem39.Id = 44;
		this.barLargeButtonItem39.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("barLargeButtonItem39.ImageOptions.Image");
		this.barLargeButtonItem39.Name = "barLargeButtonItem39";
		this.barLargeButtonItem39.Tag = "CRMReports";
		this.barLargeButtonItem40.Caption = "Schedule";
		this.barLargeButtonItem40.Id = 45;
		this.barLargeButtonItem40.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("barLargeButtonItem40.ImageOptions.Image");
		this.barLargeButtonItem40.Name = "barLargeButtonItem40";
		this.barLargeButtonItem40.Tag = "800004";
		this.radialMenu_Purchase.AutoExpand = true;
		this.radialMenu_Purchase.Glyph = (System.Drawing.Image)resources.GetObject("radialMenu_Purchase.Glyph");
		this.radialMenu_Purchase.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[9]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.barLargeButtonItem3),
			new DevExpress.XtraBars.LinkPersistInfo(this.barLargeButtonItem4),
			new DevExpress.XtraBars.LinkPersistInfo(this.barLargeButtonItem5),
			new DevExpress.XtraBars.LinkPersistInfo(this.barLargeButtonItem11),
			new DevExpress.XtraBars.LinkPersistInfo(this.barLargeButtonItem6),
			new DevExpress.XtraBars.LinkPersistInfo(this.barLargeButtonItem7),
			new DevExpress.XtraBars.LinkPersistInfo(this.barLargeButtonItem8),
			new DevExpress.XtraBars.LinkPersistInfo(this.barLargeButtonItem9),
			new DevExpress.XtraBars.LinkPersistInfo(this.barLargeButtonItem10)
		});
		this.radialMenu_Purchase.Manager = this.barManager1;
		this.radialMenu_Purchase.MenuColor = System.Drawing.Color.CornflowerBlue;
		this.radialMenu_Purchase.Name = "radialMenu_Purchase";
		this.radialMenu_Purchase.SubMenuHoverColor = System.Drawing.Color.FromArgb(128, 128, 255);
		this.radialMenu_Inventory.Glyph = (System.Drawing.Image)resources.GetObject("radialMenu_Inventory.Glyph");
		this.radialMenu_Inventory.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[7]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.barLargeButtonItem12),
			new DevExpress.XtraBars.LinkPersistInfo(this.barLargeButtonItem13),
			new DevExpress.XtraBars.LinkPersistInfo(this.barLargeButtonItem14),
			new DevExpress.XtraBars.LinkPersistInfo(this.barLargeButtonItem15),
			new DevExpress.XtraBars.LinkPersistInfo(this.barLargeButtonItem16),
			new DevExpress.XtraBars.LinkPersistInfo(this.barLargeButtonItem17),
			new DevExpress.XtraBars.LinkPersistInfo(this.barLargeButtonItem18)
		});
		this.radialMenu_Inventory.Manager = this.barManager1;
		this.radialMenu_Inventory.MenuColor = System.Drawing.Color.CadetBlue;
		this.radialMenu_Inventory.Name = "radialMenu_Inventory";
		this.radialMenu_Manufacturing.Glyph = (System.Drawing.Image)resources.GetObject("radialMenu_Manufacturing.Glyph");
		this.radialMenu_Manufacturing.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[6]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.barLargeButtonItem19),
			new DevExpress.XtraBars.LinkPersistInfo(this.barLargeButtonItem21),
			new DevExpress.XtraBars.LinkPersistInfo(this.barLargeButtonItem22),
			new DevExpress.XtraBars.LinkPersistInfo(this.barLargeButtonItem40),
			new DevExpress.XtraBars.LinkPersistInfo(this.barLargeButtonItem20),
			new DevExpress.XtraBars.LinkPersistInfo(this.barLargeButtonItem23)
		});
		this.radialMenu_Manufacturing.Manager = this.barManager1;
		this.radialMenu_Manufacturing.Name = "radialMenu_Manufacturing";
		this.radialMenu_Opportunity.Glyph = (System.Drawing.Image)resources.GetObject("radialMenu_Opportunity.Glyph");
		this.radialMenu_Opportunity.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[2]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.barLargeButtonItem24),
			new DevExpress.XtraBars.LinkPersistInfo(this.barLargeButtonItem25)
		});
		this.radialMenu_Opportunity.Manager = this.barManager1;
		this.radialMenu_Opportunity.MenuColor = System.Drawing.Color.Olive;
		this.radialMenu_Opportunity.Name = "radialMenu_Opportunity";
		this.radialMenu_HRM.Glyph = (System.Drawing.Image)resources.GetObject("radialMenu_HRM.Glyph");
		this.radialMenu_HRM.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[5]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.barLargeButtonItem26),
			new DevExpress.XtraBars.LinkPersistInfo(this.barLargeButtonItem27),
			new DevExpress.XtraBars.LinkPersistInfo(this.barLargeButtonItem28),
			new DevExpress.XtraBars.LinkPersistInfo(this.barLargeButtonItem29),
			new DevExpress.XtraBars.LinkPersistInfo(this.barLargeButtonItem30)
		});
		this.radialMenu_HRM.Manager = this.barManager1;
		this.radialMenu_HRM.MenuColor = System.Drawing.Color.DarkSlateBlue;
		this.radialMenu_HRM.Name = "radialMenu_HRM";
		this.radialMenu_Project.Glyph = (System.Drawing.Image)resources.GetObject("radialMenu_Project.Glyph");
		this.radialMenu_Project.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[3]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.barLargeButtonItem31),
			new DevExpress.XtraBars.LinkPersistInfo(this.barLargeButtonItem32),
			new DevExpress.XtraBars.LinkPersistInfo(this.barLargeButtonItem33)
		});
		this.radialMenu_Project.Manager = this.barManager1;
		this.radialMenu_Project.MenuColor = System.Drawing.Color.ForestGreen;
		this.radialMenu_Project.Name = "radialMenu_Project";
		this.radialMenu_CRM.Glyph = (System.Drawing.Image)resources.GetObject("radialMenu_CRM.Glyph");
		this.radialMenu_CRM.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[6]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.barLargeButtonItem34),
			new DevExpress.XtraBars.LinkPersistInfo(this.barLargeButtonItem35),
			new DevExpress.XtraBars.LinkPersistInfo(this.barLargeButtonItem36),
			new DevExpress.XtraBars.LinkPersistInfo(this.barLargeButtonItem37),
			new DevExpress.XtraBars.LinkPersistInfo(this.barLargeButtonItem38),
			new DevExpress.XtraBars.LinkPersistInfo(this.barLargeButtonItem39)
		});
		this.radialMenu_CRM.Manager = this.barManager1;
		this.radialMenu_CRM.MenuColor = System.Drawing.Color.LightSalmon;
		this.radialMenu_CRM.Name = "radialMenu_CRM";
		this.diagramImageEx1.AffectedByLayoutAlgorithms = false;
		this.diagramImageEx1.CanChangeParent = false;
		this.diagramImageEx1.CanCopy = false;
		this.diagramImageEx1.CanCopyWithoutParent = false;
		this.diagramImageEx1.CanDelete = false;
		this.diagramImageEx1.CanDeleteWithoutParent = false;
		this.diagramImageEx1.CanMove = false;
		this.diagramImageEx1.CanResize = false;
		this.diagramImageEx1.CanRotate = false;
		this.diagramImageEx1.Image = resources.GetObject("diagramImageEx1.Image");
		this.diagramImageEx1.ObjectGroup = "Opportunity";
		this.diagramImageEx1.ObjType = Apzon.Commons.ApzObjectTypes.oNone;
		this.diagramImageEx1.Position = new DevExpress.Utils.PointFloat(-144.5065f, -260f);
		this.diagramImageEx1.Size = new System.Drawing.SizeF(136.5452f, 136.5452f);
		this.diagramImageEx1.StretchMode = DevExpress.Diagram.Core.StretchMode.Stretch;
		this.diagramImageEx2.AffectedByLayoutAlgorithms = false;
		this.diagramImageEx2.CanChangeParent = false;
		this.diagramImageEx2.CanCopy = false;
		this.diagramImageEx2.CanCopyWithoutParent = false;
		this.diagramImageEx2.CanDelete = false;
		this.diagramImageEx2.CanDeleteWithoutParent = false;
		this.diagramImageEx2.CanMove = false;
		this.diagramImageEx2.CanResize = false;
		this.diagramImageEx2.CanRotate = false;
		this.diagramImageEx2.Image = resources.GetObject("diagramImageEx2.Image");
		this.diagramImageEx2.ObjectGroup = "Sales";
		this.diagramImageEx2.ObjType = Apzon.Commons.ApzObjectTypes.oNone;
		this.diagramImageEx2.Position = new DevExpress.Utils.PointFloat(191.7274f, -120f);
		this.diagramImageEx2.Size = new System.Drawing.SizeF(156.5452f, 156.5452f);
		this.diagramImageEx2.StretchMode = DevExpress.Diagram.Core.StretchMode.Stretch;
		this.diagramImageEx3.AffectedByLayoutAlgorithms = false;
		this.diagramImageEx3.CanChangeParent = false;
		this.diagramImageEx3.CanCopy = false;
		this.diagramImageEx3.CanCopyWithoutParent = false;
		this.diagramImageEx3.CanDelete = false;
		this.diagramImageEx3.CanDeleteWithoutParent = false;
		this.diagramImageEx3.CanMove = false;
		this.diagramImageEx3.CanResize = false;
		this.diagramImageEx3.CanRotate = false;
		this.diagramImageEx3.Image = resources.GetObject("diagramImageEx3.Image");
		this.diagramImageEx3.ObjectGroup = "Purchase";
		this.diagramImageEx3.ObjType = Apzon.Commons.ApzObjectTypes.oNone;
		this.diagramImageEx3.Position = new DevExpress.Utils.PointFloat(-454.3062f, -100f);
		this.diagramImageEx3.Size = new System.Drawing.SizeF(136.5452f, 136.5452f);
		this.diagramImageEx3.StretchMode = DevExpress.Diagram.Core.StretchMode.Stretch;
		this.diagramImageEx4.AffectedByLayoutAlgorithms = false;
		this.diagramImageEx4.CanChangeParent = false;
		this.diagramImageEx4.CanCopy = false;
		this.diagramImageEx4.CanCopyWithoutParent = false;
		this.diagramImageEx4.CanDelete = false;
		this.diagramImageEx4.CanDeleteWithoutParent = false;
		this.diagramImageEx4.CanMove = false;
		this.diagramImageEx4.CanResize = false;
		this.diagramImageEx4.CanRotate = false;
		this.diagramImageEx4.Image = resources.GetObject("diagramImageEx4.Image");
		this.diagramImageEx4.ObjectGroup = "Inventory";
		this.diagramImageEx4.ObjType = Apzon.Commons.ApzObjectTypes.oNone;
		this.diagramImageEx4.Position = new DevExpress.Utils.PointFloat(300f, 159.2755f);
		this.diagramImageEx4.Size = new System.Drawing.SizeF(136.5452f, 136.5452f);
		this.diagramImageEx4.StretchMode = DevExpress.Diagram.Core.StretchMode.Stretch;
		this.diagramImageEx5.AffectedByLayoutAlgorithms = false;
		this.diagramImageEx5.CanChangeParent = false;
		this.diagramImageEx5.CanCopy = false;
		this.diagramImageEx5.CanCopyWithoutParent = false;
		this.diagramImageEx5.CanDelete = false;
		this.diagramImageEx5.CanDeleteWithoutParent = false;
		this.diagramImageEx5.CanMove = false;
		this.diagramImageEx5.CanResize = false;
		this.diagramImageEx5.CanRotate = false;
		this.diagramImageEx5.Image = resources.GetObject("diagramImageEx5.Image");
		this.diagramImageEx5.ObjectGroup = "Manufacturing";
		this.diagramImageEx5.ObjType = Apzon.Commons.ApzObjectTypes.oNone;
		this.diagramImageEx5.Position = new DevExpress.Utils.PointFloat(211.7274f, 480f);
		this.diagramImageEx5.Size = new System.Drawing.SizeF(136.5452f, 136.5452f);
		this.diagramImageEx5.StretchMode = DevExpress.Diagram.Core.StretchMode.Stretch;
		this.diagramImageEx6.AffectedByLayoutAlgorithms = false;
		this.diagramImageEx6.CanChangeParent = false;
		this.diagramImageEx6.CanCopy = false;
		this.diagramImageEx6.CanCopyWithoutParent = false;
		this.diagramImageEx6.CanDelete = false;
		this.diagramImageEx6.CanDeleteWithoutParent = false;
		this.diagramImageEx6.CanMove = false;
		this.diagramImageEx6.CanResize = false;
		this.diagramImageEx6.CanRotate = false;
		this.diagramImageEx6.Image = resources.GetObject("diagramImageEx6.Image");
		this.diagramImageEx6.ObjectGroup = "CRM";
		this.diagramImageEx6.ObjType = Apzon.Commons.ApzObjectTypes.oNone;
		this.diagramImageEx6.Position = new DevExpress.Utils.PointFloat(-436.7274f, 500.4774f);
		this.diagramImageEx6.Size = new System.Drawing.SizeF(136.5452f, 136.5452f);
		this.diagramImageEx6.StretchMode = DevExpress.Diagram.Core.StretchMode.Stretch;
		this.diagramImageEx7.AffectedByLayoutAlgorithms = false;
		this.diagramImageEx7.CanChangeParent = false;
		this.diagramImageEx7.CanCopy = false;
		this.diagramImageEx7.CanCopyWithoutParent = false;
		this.diagramImageEx7.CanDelete = false;
		this.diagramImageEx7.CanDeleteWithoutParent = false;
		this.diagramImageEx7.CanMove = false;
		this.diagramImageEx7.CanResize = false;
		this.diagramImageEx7.CanRotate = false;
		this.diagramImageEx7.Image = resources.GetObject("diagramImageEx7.Image");
		this.diagramImageEx7.ObjectGroup = "HRM";
		this.diagramImageEx7.ObjType = Apzon.Commons.ApzObjectTypes.oNone;
		this.diagramImageEx7.Position = new DevExpress.Utils.PointFloat(-580f, 159.2755f);
		this.diagramImageEx7.Size = new System.Drawing.SizeF(136.5452f, 136.5452f);
		this.diagramImageEx7.StretchMode = DevExpress.Diagram.Core.StretchMode.Stretch;
		this.diagramShape1.Appearance.Font = new System.Drawing.Font("Segoe UI", 12f);
		this.diagramShape1.CanChangeParent = false;
		this.diagramShape1.CanCopy = false;
		this.diagramShape1.CanCopyWithoutParent = false;
		this.diagramShape1.CanDelete = false;
		this.diagramShape1.CanDeleteWithoutParent = false;
		this.diagramShape1.CanMove = false;
		this.diagramShape1.CanResize = false;
		this.diagramShape1.CanRotate = false;
		this.diagramShape1.Content = "Purchasing";
		this.diagramShape1.Position = new DevExpress.Utils.PointFloat(-427.2442f, 60.615f);
		this.diagramShape1.Shape = DevExpress.Diagram.Core.BasicFlowchartShapes.StartEnd;
		this.diagramShape1.Size = new System.Drawing.SizeF(100f, 37.5f);
		this.diagramShape2.Appearance.Font = new System.Drawing.Font("Segoe UI", 12f);
		this.diagramShape2.CanChangeParent = false;
		this.diagramShape2.CanCopy = false;
		this.diagramShape2.CanCopyWithoutParent = false;
		this.diagramShape2.CanDelete = false;
		this.diagramShape2.CanDeleteWithoutParent = false;
		this.diagramShape2.CanMove = false;
		this.diagramShape2.CanResize = false;
		this.diagramShape2.CanRotate = false;
		this.diagramShape2.Content = "Opportunity";
		this.diagramShape2.Position = new DevExpress.Utils.PointFloat(-123.1994f, -120f);
		this.diagramShape2.Shape = DevExpress.Diagram.Core.BasicFlowchartShapes.StartEnd;
		this.diagramShape2.Size = new System.Drawing.SizeF(100f, 37.5f);
		this.diagramShape3.Appearance.Font = new System.Drawing.Font("Segoe UI", 12f);
		this.diagramShape3.CanChangeParent = false;
		this.diagramShape3.CanCopy = false;
		this.diagramShape3.CanCopyWithoutParent = false;
		this.diagramShape3.CanDelete = false;
		this.diagramShape3.CanDeleteWithoutParent = false;
		this.diagramShape3.CanMove = false;
		this.diagramShape3.CanResize = false;
		this.diagramShape3.CanRotate = false;
		this.diagramShape3.Content = "Sales A/R";
		this.diagramShape3.Position = new DevExpress.Utils.PointFloat(210f, 60.615f);
		this.diagramShape3.Shape = DevExpress.Diagram.Core.BasicFlowchartShapes.StartEnd;
		this.diagramShape3.Size = new System.Drawing.SizeF(100f, 37.5f);
		this.diagramShape4.Appearance.Font = new System.Drawing.Font("Segoe UI", 12f);
		this.diagramShape4.CanChangeParent = false;
		this.diagramShape4.CanCopy = false;
		this.diagramShape4.CanCopyWithoutParent = false;
		this.diagramShape4.CanDelete = false;
		this.diagramShape4.CanDeleteWithoutParent = false;
		this.diagramShape4.CanMove = false;
		this.diagramShape4.CanResize = false;
		this.diagramShape4.CanRotate = false;
		this.diagramShape4.Content = "Inventory";
		this.diagramShape4.Position = new DevExpress.Utils.PointFloat(318.2726f, 320f);
		this.diagramShape4.Shape = DevExpress.Diagram.Core.BasicFlowchartShapes.StartEnd;
		this.diagramShape4.Size = new System.Drawing.SizeF(100f, 37.5f);
		this.diagramShape5.Appearance.Font = new System.Drawing.Font("Segoe UI", 12f);
		this.diagramShape5.CanChangeParent = false;
		this.diagramShape5.CanCopy = false;
		this.diagramShape5.CanCopyWithoutParent = false;
		this.diagramShape5.CanDelete = false;
		this.diagramShape5.CanDeleteWithoutParent = false;
		this.diagramShape5.CanMove = false;
		this.diagramShape5.CanResize = false;
		this.diagramShape5.CanRotate = false;
		this.diagramShape5.Content = "Manufacturing";
		this.diagramShape5.Position = new DevExpress.Utils.PointFloat(229.3062f, 620f);
		this.diagramShape5.Shape = DevExpress.Diagram.Core.BasicFlowchartShapes.StartEnd;
		this.diagramShape5.Size = new System.Drawing.SizeF(118.9664f, 37.5f);
		this.diagramShape6.Appearance.Font = new System.Drawing.Font("Segoe UI", 12f);
		this.diagramShape6.CanChangeParent = false;
		this.diagramShape6.CanCopy = false;
		this.diagramShape6.CanCopyWithoutParent = false;
		this.diagramShape6.CanDelete = false;
		this.diagramShape6.CanDeleteWithoutParent = false;
		this.diagramShape6.CanMove = false;
		this.diagramShape6.CanResize = false;
		this.diagramShape6.CanRotate = false;
		this.diagramShape6.Content = "CRM";
		this.diagramShape6.Position = new DevExpress.Utils.PointFloat(-436.7274f, 640f);
		this.diagramShape6.Shape = DevExpress.Diagram.Core.BasicFlowchartShapes.StartEnd;
		this.diagramShape6.Size = new System.Drawing.SizeF(118.9664f, 37.5f);
		this.diagramShape7.Appearance.Font = new System.Drawing.Font("Segoe UI", 12f);
		this.diagramShape7.CanChangeParent = false;
		this.diagramShape7.CanCopy = false;
		this.diagramShape7.CanCopyWithoutParent = false;
		this.diagramShape7.CanDelete = false;
		this.diagramShape7.CanDeleteWithoutParent = false;
		this.diagramShape7.CanMove = false;
		this.diagramShape7.CanResize = false;
		this.diagramShape7.CanRotate = false;
		this.diagramShape7.Content = "Human Resources";
		this.diagramShape7.Position = new DevExpress.Utils.PointFloat(-598.4548f, 320f);
		this.diagramShape7.Shape = DevExpress.Diagram.Core.BasicFlowchartShapes.StartEnd;
		this.diagramShape7.Size = new System.Drawing.SizeF(155f, 37.5f);
		this.diagramShape8.Appearance.Font = new System.Drawing.Font("Segoe UI", 12f);
		this.diagramShape8.CanChangeParent = false;
		this.diagramShape8.CanCopy = false;
		this.diagramShape8.CanCopyWithoutParent = false;
		this.diagramShape8.CanDelete = false;
		this.diagramShape8.CanDeleteWithoutParent = false;
		this.diagramShape8.CanMove = false;
		this.diagramShape8.CanResize = false;
		this.diagramShape8.CanRotate = false;
		this.diagramShape8.Content = "Project Management";
		this.diagramShape8.Position = new DevExpress.Utils.PointFloat(-159.7446f, 720f);
		this.diagramShape8.Shape = DevExpress.Diagram.Core.BasicFlowchartShapes.StartEnd;
		this.diagramShape8.Size = new System.Drawing.SizeF(180.3122f, 37.5f);
		this.diagramImageEx8.CanCopy = false;
		this.diagramImageEx8.CanCopyWithoutParent = false;
		this.diagramImageEx8.CanDelete = false;
		this.diagramImageEx8.CanDeleteWithoutParent = false;
		this.diagramImageEx8.CanMove = false;
		this.diagramImageEx8.CanResize = false;
		this.diagramImageEx8.CanRotate = false;
		this.diagramImageEx8.Image = resources.GetObject("diagramImageEx8.Image");
		this.diagramImageEx8.ObjectGroup = "Project";
		this.diagramImageEx8.ObjType = Apzon.Commons.ApzObjectTypes.oNone;
		this.diagramImageEx8.Position = new DevExpress.Utils.PointFloat(-141.472f, 570.4774f);
		this.diagramImageEx8.Size = new System.Drawing.SizeF(136.5452f, 136.5452f);
		this.diagramImageEx8.StretchMode = DevExpress.Diagram.Core.StretchMode.Stretch;
		this.diagramShape9.Angle = 135f;
		this.diagramShape9.Position = new DevExpress.Utils.PointFloat(-320f, 80f);
		this.diagramShape9.Shape = DevExpress.Diagram.Core.ArrowShapes.StripedArrow;
		this.diagramShape9.Size = new System.Drawing.SizeF(202.028f, 35.78428f);
		this.diagramShape9.ThemeStyleId = DevExpress.Diagram.Core.DiagramShapeStyleId.Subtle1;
		this.diagramShape10.Angle = 90f;
		this.diagramShape10.Position = new DevExpress.Utils.PointFloat(-170.6025f, 23.12184f);
		this.diagramShape10.Shape = DevExpress.Diagram.Core.ArrowShapes.StripedArrow;
		this.diagramShape10.Size = new System.Drawing.SizeF(202.028f, 35.78428f);
		this.diagramShape10.ThemeStyleId = DevExpress.Diagram.Core.DiagramShapeStyleId.Subtle1;
		this.diagramShape11.Angle = 45f;
		this.diagramShape11.Position = new DevExpress.Utils.PointFloat(-16.93466f, 80f);
		this.diagramShape11.Shape = DevExpress.Diagram.Core.ArrowShapes.StripedArrow;
		this.diagramShape11.Size = new System.Drawing.SizeF(202.028f, 35.78428f);
		this.diagramShape11.ThemeStyleId = DevExpress.Diagram.Core.DiagramShapeStyleId.Subtle1;
		this.diagramShape12.Position = new DevExpress.Utils.PointFloat(40f, 215.4286f);
		this.diagramShape12.Shape = DevExpress.Diagram.Core.ArrowShapes.StripedArrow;
		this.diagramShape12.Size = new System.Drawing.SizeF(202.028f, 35.78428f);
		this.diagramShape12.ThemeStyleId = DevExpress.Diagram.Core.DiagramShapeStyleId.Subtle1;
		this.diagramShape13.Angle = -45f;
		this.diagramShape13.Position = new DevExpress.Utils.PointFloat(-16.93466f, 366.1872f);
		this.diagramShape13.Shape = DevExpress.Diagram.Core.ArrowShapes.StripedArrow;
		this.diagramShape13.Size = new System.Drawing.SizeF(202.028f, 35.78428f);
		this.diagramShape13.ThemeStyleId = DevExpress.Diagram.Core.DiagramShapeStyleId.Subtle1;
		this.diagramShape14.Angle = -90f;
		this.diagramShape14.Position = new DevExpress.Utils.PointFloat(-170.6025f, 423.1219f);
		this.diagramShape14.Shape = DevExpress.Diagram.Core.ArrowShapes.StripedArrow;
		this.diagramShape14.Size = new System.Drawing.SizeF(202.028f, 35.78428f);
		this.diagramShape14.ThemeStyleId = DevExpress.Diagram.Core.DiagramShapeStyleId.Subtle1;
		this.diagramShape15.Angle = -135f;
		this.diagramShape15.Position = new DevExpress.Utils.PointFloat(-320f, 366.1872f);
		this.diagramShape15.Shape = DevExpress.Diagram.Core.ArrowShapes.StripedArrow;
		this.diagramShape15.Size = new System.Drawing.SizeF(202.028f, 35.78428f);
		this.diagramShape15.ThemeStyleId = DevExpress.Diagram.Core.DiagramShapeStyleId.Subtle1;
		this.diagramShape16.Angle = -180f;
		this.diagramShape16.Position = new DevExpress.Utils.PointFloat(-380f, 215.4286f);
		this.diagramShape16.Shape = DevExpress.Diagram.Core.ArrowShapes.StripedArrow;
		this.diagramShape16.Size = new System.Drawing.SizeF(202.028f, 35.78428f);
		this.diagramShape16.ThemeStyleId = DevExpress.Diagram.Core.DiagramShapeStyleId.Subtle1;
		this.diagramImage1.AffectedByLayoutAlgorithms = false;
		this.diagramImage1.CanAttachConnectorBeginPoint = false;
		this.diagramImage1.CanAttachConnectorEndPoint = false;
		this.diagramImage1.CanChangeParent = false;
		this.diagramImage1.CanCopy = false;
		this.diagramImage1.CanCopyWithoutParent = false;
		this.diagramImage1.CanDelete = false;
		this.diagramImage1.CanDeleteWithoutParent = false;
		this.diagramImage1.CanMove = false;
		this.diagramImage1.CanResize = false;
		this.diagramImage1.CanRotate = false;
		this.diagramImage1.CanSnapToOtherItems = false;
		this.diagramImage1.CanSnapToThisItem = false;
		this.diagramImage1.Image = resources.GetObject("diagramImage1.Image");
		this.diagramImage1.Position = new DevExpress.Utils.PointFloat(-163.3555f, 165.8472f);
		this.diagramImage1.Size = new System.Drawing.SizeF(180.3122f, 134.947f);
		this.diagramImage1.StretchMode = DevExpress.Diagram.Core.StretchMode.Stretch;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.SystemColors.Control;
		base.Controls.Add(this.diagramControl1);
		base.Controls.Add(this.barDockControlLeft);
		base.Controls.Add(this.barDockControlRight);
		base.Controls.Add(this.barDockControlBottom);
		base.Controls.Add(this.barDockControlTop);
		base.Name = "ucMainProcessModel";
		base.Size = new System.Drawing.Size(1273, 734);
		base.Load += new System.EventHandler(this.ucMainProcessModel_Load);
		base.Resize += new System.EventHandler(this.ucMainProcessModel_Resize);
		((System.ComponentModel.ISupportInitialize)this.diagramControl1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.radialMenu_Sales).EndInit();
		((System.ComponentModel.ISupportInitialize)this.barManager1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.radialMenu_Purchase).EndInit();
		((System.ComponentModel.ISupportInitialize)this.radialMenu_Inventory).EndInit();
		((System.ComponentModel.ISupportInitialize)this.radialMenu_Manufacturing).EndInit();
		((System.ComponentModel.ISupportInitialize)this.radialMenu_Opportunity).EndInit();
		((System.ComponentModel.ISupportInitialize)this.radialMenu_HRM).EndInit();
		((System.ComponentModel.ISupportInitialize)this.radialMenu_Project).EndInit();
		((System.ComponentModel.ISupportInitialize)this.radialMenu_CRM).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
