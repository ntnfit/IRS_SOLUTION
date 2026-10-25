using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Controls.Form;
using Apzon.Forms.UserControls.POS_UI;
using DevExpress.XtraBars.Docking;
using DevExpress.XtraBars.Docking.Helpers;
using DevExpress.XtraBars.Navigation;
using DevExpress.XtraEditors.Controls;

namespace Apzon.Forms.Retail;

public class PosTest : XtraFormEx
{
	private IContainer components;

	private DockManager dockManager1;

	private DockPanel dockPanel4;

	private ControlContainer dockPanel4_Container;

	private DockPanel dockPanel3;

	private ControlContainer dockPanel3_Container;

	private DockPanel dockPanel2;

	private ControlContainer dockPanel2_Container;

	private DockPanel dockPanel1;

	private ControlContainer dockPanel1_Container;

	private NavigationFrame navigationFrame2;

	private NavigationPage navigationPage5;

	private ucMemberBarcode ucMemberBarcode1;

	private NavigationPage navigationPage6;

	private ucMemberDetail ucMemberDetail2;

	private NavigationPage navigationPage7;

	private ucPaymentScreen ucPaymentScreen2;

	private NavigationPage navigationPage8;

	private ucBarcode ucBarcode2;

	private ucRetailGrid ucRetailGrid2;

	private ucPaymentTotal ucPaymentTotal1;

	private NavigationFrame navigationFrame1;

	private NavigationPage navigationPage1;

	private ucScanItem ucScanItem1;

	private NavigationPage navigationPage2;

	private ucCustomerPay ucCustomerPay1;

	private ucCalculator ucCalculator1;

	private NavigationFrame navigationFrame3;

	private NavigationPage navigationPage3;

	private ucPaymentCmd ucPaymentCmd1;

	private NavigationPage navigationPage4;

	private ucCommandPanel ucCommandPanel1;

	private DockPanel dockPanel5;

	private ControlContainer dockPanel5_Container;

	private ucStatusBar ucStatusBar1;

	private KeyboardSimpleButton btn_Minus;

	private DockPanel dockPanel9;

	private ControlContainer dockPanel9_Container;

	private ucPowQuantity ucPowQuantity1;

	private DockPanel panelContainer1;

	private DockPanel dockPanel6;

	private ControlContainer dockPanel6_Container;

	public PosTest()
	{
		InitializeComponent();
	}

	private void PosTest_Load(object sender, EventArgs e)
	{
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
		new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Retail.PosTest));
		this.dockManager1 = new DevExpress.XtraBars.Docking.DockManager(this.components);
		this.dockPanel5 = new DevExpress.XtraBars.Docking.DockPanel();
		this.dockPanel5_Container = new DevExpress.XtraBars.Docking.ControlContainer();
		this.ucStatusBar1 = new Apzon.Forms.UserControls.POS_UI.ucStatusBar();
		this.dockPanel1 = new DevExpress.XtraBars.Docking.DockPanel();
		this.dockPanel1_Container = new DevExpress.XtraBars.Docking.ControlContainer();
		this.navigationFrame2 = new DevExpress.XtraBars.Navigation.NavigationFrame();
		this.navigationPage5 = new DevExpress.XtraBars.Navigation.NavigationPage();
		this.ucMemberBarcode1 = new Apzon.Forms.UserControls.POS_UI.ucMemberBarcode();
		this.navigationPage6 = new DevExpress.XtraBars.Navigation.NavigationPage();
		this.ucMemberDetail2 = new Apzon.Forms.UserControls.POS_UI.ucMemberDetail();
		this.navigationPage7 = new DevExpress.XtraBars.Navigation.NavigationPage();
		this.ucPaymentScreen2 = new Apzon.Forms.UserControls.POS_UI.ucPaymentScreen();
		this.navigationPage8 = new DevExpress.XtraBars.Navigation.NavigationPage();
		this.ucPaymentTotal1 = new Apzon.Forms.UserControls.POS_UI.ucPaymentTotal();
		this.ucBarcode2 = new Apzon.Forms.UserControls.POS_UI.ucBarcode();
		this.ucRetailGrid2 = new Apzon.Forms.UserControls.POS_UI.ucRetailGrid();
		this.dockPanel9 = new DevExpress.XtraBars.Docking.DockPanel();
		this.dockPanel9_Container = new DevExpress.XtraBars.Docking.ControlContainer();
		this.ucPowQuantity1 = new Apzon.Forms.UserControls.POS_UI.ucPowQuantity();
		this.dockPanel2 = new DevExpress.XtraBars.Docking.DockPanel();
		this.dockPanel2_Container = new DevExpress.XtraBars.Docking.ControlContainer();
		this.navigationFrame1 = new DevExpress.XtraBars.Navigation.NavigationFrame();
		this.navigationPage1 = new DevExpress.XtraBars.Navigation.NavigationPage();
		this.btn_Minus = new Apzon.Forms.UserControls.POS_UI.KeyboardSimpleButton();
		this.ucScanItem1 = new Apzon.Forms.UserControls.POS_UI.ucScanItem();
		this.navigationPage2 = new DevExpress.XtraBars.Navigation.NavigationPage();
		this.ucCustomerPay1 = new Apzon.Forms.UserControls.POS_UI.ucCustomerPay();
		this.dockPanel4 = new DevExpress.XtraBars.Docking.DockPanel();
		this.dockPanel4_Container = new DevExpress.XtraBars.Docking.ControlContainer();
		this.ucCalculator1 = new Apzon.Forms.UserControls.POS_UI.ucCalculator();
		this.dockPanel3 = new DevExpress.XtraBars.Docking.DockPanel();
		this.dockPanel3_Container = new DevExpress.XtraBars.Docking.ControlContainer();
		this.navigationFrame3 = new DevExpress.XtraBars.Navigation.NavigationFrame();
		this.navigationPage3 = new DevExpress.XtraBars.Navigation.NavigationPage();
		this.ucPaymentCmd1 = new Apzon.Forms.UserControls.POS_UI.ucPaymentCmd();
		this.navigationPage4 = new DevExpress.XtraBars.Navigation.NavigationPage();
		this.ucCommandPanel1 = new Apzon.Forms.UserControls.POS_UI.ucCommandPanel();
		this.dockPanel6 = new DevExpress.XtraBars.Docking.DockPanel();
		this.dockPanel6_Container = new DevExpress.XtraBars.Docking.ControlContainer();
		this.panelContainer1 = new DevExpress.XtraBars.Docking.DockPanel();
		((System.ComponentModel.ISupportInitialize)this.dockManager1).BeginInit();
		this.dockPanel5.SuspendLayout();
		this.dockPanel5_Container.SuspendLayout();
		this.dockPanel1.SuspendLayout();
		this.dockPanel1_Container.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.navigationFrame2).BeginInit();
		this.navigationFrame2.SuspendLayout();
		this.navigationPage5.SuspendLayout();
		this.navigationPage6.SuspendLayout();
		this.navigationPage7.SuspendLayout();
		this.navigationPage8.SuspendLayout();
		this.dockPanel9.SuspendLayout();
		this.dockPanel9_Container.SuspendLayout();
		this.dockPanel2.SuspendLayout();
		this.dockPanel2_Container.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.navigationFrame1).BeginInit();
		this.navigationFrame1.SuspendLayout();
		this.navigationPage1.SuspendLayout();
		this.navigationPage2.SuspendLayout();
		this.dockPanel4.SuspendLayout();
		this.dockPanel4_Container.SuspendLayout();
		this.dockPanel3.SuspendLayout();
		this.dockPanel3_Container.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.navigationFrame3).BeginInit();
		this.navigationFrame3.SuspendLayout();
		this.navigationPage3.SuspendLayout();
		this.navigationPage4.SuspendLayout();
		this.dockPanel6.SuspendLayout();
		this.panelContainer1.SuspendLayout();
		base.SuspendLayout();
		this.dockManager1.DockMode = DevExpress.XtraBars.Docking.Helpers.DockMode.Standard;
		this.dockManager1.Form = this;
		this.dockManager1.RootPanels.AddRange(new DevExpress.XtraBars.Docking.DockPanel[6] { this.dockPanel5, this.dockPanel1, this.dockPanel9, this.dockPanel2, this.panelContainer1, this.dockPanel3 });
		this.dockManager1.TopZIndexControls.AddRange(new string[10] { "DevExpress.XtraBars.BarDockControl", "DevExpress.XtraBars.StandaloneBarDockControl", "System.Windows.Forms.StatusBar", "System.Windows.Forms.MenuStrip", "System.Windows.Forms.StatusStrip", "DevExpress.XtraBars.Ribbon.RibbonStatusBar", "DevExpress.XtraBars.Ribbon.RibbonControl", "DevExpress.XtraBars.Navigation.OfficeNavigationBar", "DevExpress.XtraBars.Navigation.TileNavPane", "DevExpress.XtraBars.TabFormControl" });
		this.dockPanel5.Controls.Add(this.dockPanel5_Container);
		this.dockPanel5.Dock = DevExpress.XtraBars.Docking.DockingStyle.Top;
		this.dockPanel5.FloatVertical = true;
		this.dockPanel5.ID = new System.Guid("6019094b-29af-4426-b495-b86d3982d157");
		this.dockPanel5.Location = new System.Drawing.Point(0, 0);
		this.dockPanel5.Name = "dockPanel5";
		this.dockPanel5.OriginalSize = new System.Drawing.Size(200, 51);
		this.dockPanel5.Size = new System.Drawing.Size(1168, 51);
		this.dockPanel5.Text = "dockPanel5";
		this.dockPanel5_Container.Controls.Add(this.ucStatusBar1);
		this.dockPanel5_Container.Location = new System.Drawing.Point(4, 23);
		this.dockPanel5_Container.Name = "dockPanel5_Container";
		this.dockPanel5_Container.Size = new System.Drawing.Size(1160, 23);
		this.dockPanel5_Container.TabIndex = 0;
		this.ucStatusBar1.Appearance.BackColor = System.Drawing.SystemColors.Control;
		this.ucStatusBar1.Appearance.Options.UseBackColor = true;
		this.ucStatusBar1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.ucStatusBar1.Location = new System.Drawing.Point(0, 0);
		this.ucStatusBar1.Name = "ucStatusBar1";
		this.ucStatusBar1.Size = new System.Drawing.Size(1160, 23);
		this.ucStatusBar1.TabIndex = 0;
		this.dockPanel1.Controls.Add(this.dockPanel1_Container);
		this.dockPanel1.Dock = DevExpress.XtraBars.Docking.DockingStyle.Left;
		this.dockPanel1.ID = new System.Guid("a707cf03-4c79-48f6-a437-90c5fcda3ff2");
		this.dockPanel1.Location = new System.Drawing.Point(0, 51);
		this.dockPanel1.Name = "dockPanel1";
		this.dockPanel1.OriginalSize = new System.Drawing.Size(565, 200);
		this.dockPanel1.Size = new System.Drawing.Size(565, 656);
		this.dockPanel1.Text = "dockPanel1";
		this.dockPanel1_Container.Controls.Add(this.navigationFrame2);
		this.dockPanel1_Container.Location = new System.Drawing.Point(4, 23);
		this.dockPanel1_Container.Name = "dockPanel1_Container";
		this.dockPanel1_Container.Size = new System.Drawing.Size(556, 629);
		this.dockPanel1_Container.TabIndex = 0;
		this.navigationFrame2.Controls.Add(this.navigationPage5);
		this.navigationFrame2.Controls.Add(this.navigationPage6);
		this.navigationFrame2.Controls.Add(this.navigationPage7);
		this.navigationFrame2.Controls.Add(this.navigationPage8);
		this.navigationFrame2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.navigationFrame2.Location = new System.Drawing.Point(0, 0);
		this.navigationFrame2.Name = "navigationFrame2";
		this.navigationFrame2.Pages.AddRange(new DevExpress.XtraBars.Navigation.NavigationPageBase[4] { this.navigationPage8, this.navigationPage5, this.navigationPage6, this.navigationPage7 });
		this.navigationFrame2.SelectedPage = this.navigationPage8;
		this.navigationFrame2.Size = new System.Drawing.Size(556, 629);
		this.navigationFrame2.TabIndex = 1;
		this.navigationFrame2.Text = "navigationFrame2";
		this.navigationPage5.Caption = "navigationPage5";
		this.navigationPage5.Controls.Add(this.ucMemberBarcode1);
		this.navigationPage5.Name = "navigationPage5";
		this.navigationPage5.Size = new System.Drawing.Size(556, 629);
		this.ucMemberBarcode1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.ucMemberBarcode1.Location = new System.Drawing.Point(0, 0);
		this.ucMemberBarcode1.Name = "ucMemberBarcode1";
		this.ucMemberBarcode1.Size = new System.Drawing.Size(556, 629);
		this.ucMemberBarcode1.TabIndex = 1;
		this.navigationPage6.Caption = "navigationPage6";
		this.navigationPage6.Controls.Add(this.ucMemberDetail2);
		this.navigationPage6.Name = "navigationPage6";
		this.navigationPage6.Size = new System.Drawing.Size(556, 629);
		this.ucMemberDetail2.Appearance.BackColor = System.Drawing.SystemColors.Control;
		this.ucMemberDetail2.Appearance.Options.UseBackColor = true;
		this.ucMemberDetail2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.ucMemberDetail2.Location = new System.Drawing.Point(0, 0);
		this.ucMemberDetail2.Name = "ucMemberDetail2";
		this.ucMemberDetail2.Size = new System.Drawing.Size(556, 629);
		this.ucMemberDetail2.TabIndex = 0;
		this.navigationPage7.Caption = "navigationPage7";
		this.navigationPage7.Controls.Add(this.ucPaymentScreen2);
		this.navigationPage7.Name = "navigationPage7";
		this.navigationPage7.Size = new System.Drawing.Size(556, 629);
		this.ucPaymentScreen2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.ucPaymentScreen2.Location = new System.Drawing.Point(0, 0);
		this.ucPaymentScreen2.Name = "ucPaymentScreen2";
		this.ucPaymentScreen2.Size = new System.Drawing.Size(556, 629);
		this.ucPaymentScreen2.TabIndex = 0;
		this.navigationPage8.Caption = "navigationPage8";
		this.navigationPage8.Controls.Add(this.ucPaymentTotal1);
		this.navigationPage8.Controls.Add(this.ucBarcode2);
		this.navigationPage8.Controls.Add(this.ucRetailGrid2);
		this.navigationPage8.Name = "navigationPage8";
		this.navigationPage8.Size = new System.Drawing.Size(556, 629);
		this.ucPaymentTotal1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.ucPaymentTotal1.Location = new System.Drawing.Point(3, 503);
		this.ucPaymentTotal1.Name = "ucPaymentTotal1";
		this.ucPaymentTotal1.Size = new System.Drawing.Size(550, 77);
		this.ucPaymentTotal1.TabIndex = 2;
		this.ucBarcode2.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.ucBarcode2.Location = new System.Drawing.Point(1, 586);
		this.ucBarcode2.Name = "ucBarcode2";
		this.ucBarcode2.RetailGrid = null;
		this.ucBarcode2.Size = new System.Drawing.Size(553, 40);
		this.ucBarcode2.TabIndex = 1;
		this.ucRetailGrid2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.ucRetailGrid2.Location = new System.Drawing.Point(3, 3);
		this.ucRetailGrid2.Name = "ucRetailGrid2";
		this.ucRetailGrid2.Size = new System.Drawing.Size(551, 494);
		this.ucRetailGrid2.TabIndex = 0;
		this.dockPanel9.Controls.Add(this.dockPanel9_Container);
		this.dockPanel9.Dock = DevExpress.XtraBars.Docking.DockingStyle.Left;
		this.dockPanel9.ID = new System.Guid("cc072f0c-bb7c-4b64-bdc2-973b58e70db4");
		this.dockPanel9.Location = new System.Drawing.Point(565, 51);
		this.dockPanel9.Name = "dockPanel9";
		this.dockPanel9.OriginalSize = new System.Drawing.Size(82, 161);
		this.dockPanel9.Size = new System.Drawing.Size(82, 656);
		this.dockPanel9.Text = "dockPanel9";
		this.dockPanel9_Container.Controls.Add(this.ucPowQuantity1);
		this.dockPanel9_Container.Location = new System.Drawing.Point(4, 23);
		this.dockPanel9_Container.Name = "dockPanel9_Container";
		this.dockPanel9_Container.Size = new System.Drawing.Size(73, 629);
		this.dockPanel9_Container.TabIndex = 0;
		this.ucPowQuantity1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.ucPowQuantity1.Location = new System.Drawing.Point(0, 0);
		this.ucPowQuantity1.Name = "ucPowQuantity1";
		this.ucPowQuantity1.Size = new System.Drawing.Size(73, 629);
		this.ucPowQuantity1.TabIndex = 10;
		this.dockPanel2.Controls.Add(this.dockPanel2_Container);
		this.dockPanel2.Dock = DevExpress.XtraBars.Docking.DockingStyle.Top;
		this.dockPanel2.ID = new System.Guid("d2742df8-9705-40f7-bf37-8f6017f4ff2c");
		this.dockPanel2.Location = new System.Drawing.Point(647, 51);
		this.dockPanel2.Name = "dockPanel2";
		this.dockPanel2.OriginalSize = new System.Drawing.Size(200, 101);
		this.dockPanel2.Size = new System.Drawing.Size(521, 101);
		this.dockPanel2.Text = "dockPanel2";
		this.dockPanel2_Container.Controls.Add(this.navigationFrame1);
		this.dockPanel2_Container.Location = new System.Drawing.Point(4, 23);
		this.dockPanel2_Container.Name = "dockPanel2_Container";
		this.dockPanel2_Container.Size = new System.Drawing.Size(513, 73);
		this.dockPanel2_Container.TabIndex = 0;
		this.navigationFrame1.Controls.Add(this.navigationPage1);
		this.navigationFrame1.Controls.Add(this.navigationPage2);
		this.navigationFrame1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.navigationFrame1.Location = new System.Drawing.Point(0, 0);
		this.navigationFrame1.Name = "navigationFrame1";
		this.navigationFrame1.Pages.AddRange(new DevExpress.XtraBars.Navigation.NavigationPageBase[2] { this.navigationPage1, this.navigationPage2 });
		this.navigationFrame1.SelectedPage = this.navigationPage1;
		this.navigationFrame1.Size = new System.Drawing.Size(513, 73);
		this.navigationFrame1.TabIndex = 0;
		this.navigationFrame1.Text = "navigationFrame1";
		this.navigationPage1.Caption = "navigationPage1";
		this.navigationPage1.Controls.Add(this.btn_Minus);
		this.navigationPage1.Controls.Add(this.ucScanItem1);
		this.navigationPage1.Name = "navigationPage1";
		this.navigationPage1.Size = new System.Drawing.Size(513, 73);
		this.btn_Minus.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Minus.Appearance.BackColor = System.Drawing.Color.FromArgb(255, 192, 0);
		this.btn_Minus.Appearance.Font = new System.Drawing.Font("Tahoma", 11.48141f, System.Drawing.FontStyle.Bold);
		this.btn_Minus.Appearance.ForeColor = System.Drawing.Color.Black;
		this.btn_Minus.Appearance.Options.UseBackColor = true;
		this.btn_Minus.Appearance.Options.UseFont = true;
		this.btn_Minus.Appearance.Options.UseForeColor = true;
		this.btn_Minus.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Flat;
		this.btn_Minus.Location = new System.Drawing.Point(427, 1);
		this.btn_Minus.Name = "btn_Minus";
		this.btn_Minus.Size = new System.Drawing.Size(86, 72);
		this.btn_Minus.TabIndex = 36;
		this.btn_Minus.Text = "Kết thúc";
		this.ucScanItem1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.ucScanItem1.Location = new System.Drawing.Point(0, 0);
		this.ucScanItem1.Name = "ucScanItem1";
		this.ucScanItem1.Size = new System.Drawing.Size(421, 73);
		this.ucScanItem1.TabIndex = 0;
		this.navigationPage2.Caption = "navigationPage2";
		this.navigationPage2.Controls.Add(this.ucCustomerPay1);
		this.navigationPage2.Name = "navigationPage2";
		this.navigationPage2.Size = new System.Drawing.Size(513, 73);
		this.ucCustomerPay1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.ucCustomerPay1.Location = new System.Drawing.Point(0, 0);
		this.ucCustomerPay1.Name = "ucCustomerPay1";
		this.ucCustomerPay1.Size = new System.Drawing.Size(513, 73);
		this.ucCustomerPay1.TabIndex = 0;
		this.dockPanel4.Controls.Add(this.dockPanel4_Container);
		this.dockPanel4.Dock = DevExpress.XtraBars.Docking.DockingStyle.Fill;
		this.dockPanel4.FloatVertical = true;
		this.dockPanel4.ID = new System.Guid("5f048c97-b230-4f9d-a42a-fd2c326a9593");
		this.dockPanel4.Location = new System.Drawing.Point(297, 0);
		this.dockPanel4.Name = "dockPanel4";
		this.dockPanel4.OriginalSize = new System.Drawing.Size(200, 200);
		this.dockPanel4.Size = new System.Drawing.Size(224, 238);
		this.dockPanel4_Container.Controls.Add(this.ucCalculator1);
		this.dockPanel4_Container.Location = new System.Drawing.Point(4, 24);
		this.dockPanel4_Container.Name = "dockPanel4_Container";
		this.dockPanel4_Container.Size = new System.Drawing.Size(216, 210);
		this.dockPanel4_Container.TabIndex = 0;
		this.ucCalculator1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.ucCalculator1.Location = new System.Drawing.Point(-5, 0);
		this.ucCalculator1.Name = "ucCalculator1";
		this.ucCalculator1.Size = new System.Drawing.Size(218, 207);
		this.ucCalculator1.TabIndex = 0;
		this.dockPanel3.Controls.Add(this.dockPanel3_Container);
		this.dockPanel3.Dock = DevExpress.XtraBars.Docking.DockingStyle.Top;
		this.dockPanel3.ID = new System.Guid("857a1ad3-af2b-4dbb-9022-5b71ada850f2");
		this.dockPanel3.Location = new System.Drawing.Point(647, 152);
		this.dockPanel3.Name = "dockPanel3";
		this.dockPanel3.OriginalSize = new System.Drawing.Size(200, 317);
		this.dockPanel3.Size = new System.Drawing.Size(521, 317);
		this.dockPanel3.Text = "dockPanel3";
		this.dockPanel3_Container.Controls.Add(this.navigationFrame3);
		this.dockPanel3_Container.Location = new System.Drawing.Point(4, 23);
		this.dockPanel3_Container.Name = "dockPanel3_Container";
		this.dockPanel3_Container.Size = new System.Drawing.Size(513, 289);
		this.dockPanel3_Container.TabIndex = 0;
		this.navigationFrame3.Controls.Add(this.navigationPage3);
		this.navigationFrame3.Controls.Add(this.navigationPage4);
		this.navigationFrame3.Dock = System.Windows.Forms.DockStyle.Fill;
		this.navigationFrame3.Location = new System.Drawing.Point(0, 0);
		this.navigationFrame3.Name = "navigationFrame3";
		this.navigationFrame3.Pages.AddRange(new DevExpress.XtraBars.Navigation.NavigationPageBase[2] { this.navigationPage4, this.navigationPage3 });
		this.navigationFrame3.SelectedPage = this.navigationPage4;
		this.navigationFrame3.Size = new System.Drawing.Size(513, 289);
		this.navigationFrame3.TabIndex = 0;
		this.navigationFrame3.Text = "navigationFrame3";
		this.navigationPage3.Caption = "navigationPage3";
		this.navigationPage3.Controls.Add(this.ucPaymentCmd1);
		this.navigationPage3.Name = "navigationPage3";
		this.navigationPage3.Size = new System.Drawing.Size(513, 289);
		this.ucPaymentCmd1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.ucPaymentCmd1.Location = new System.Drawing.Point(0, 0);
		this.ucPaymentCmd1.Name = "ucPaymentCmd1";
		this.ucPaymentCmd1.Size = new System.Drawing.Size(513, 289);
		this.ucPaymentCmd1.TabIndex = 0;
		this.navigationPage4.Caption = "navigationPage4";
		this.navigationPage4.Controls.Add(this.ucCommandPanel1);
		this.navigationPage4.Name = "navigationPage4";
		this.navigationPage4.Size = new System.Drawing.Size(513, 289);
		this.ucCommandPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.ucCommandPanel1.Location = new System.Drawing.Point(0, 0);
		this.ucCommandPanel1.Name = "ucCommandPanel1";
		this.ucCommandPanel1.Size = new System.Drawing.Size(513, 289);
		this.ucCommandPanel1.TabIndex = 0;
		this.dockPanel6.Controls.Add(this.dockPanel6_Container);
		this.dockPanel6.Dock = DevExpress.XtraBars.Docking.DockingStyle.Fill;
		this.dockPanel6.ID = new System.Guid("500246ac-2631-48fc-bc8e-ba75b370e176");
		this.dockPanel6.Location = new System.Drawing.Point(0, 0);
		this.dockPanel6.Name = "dockPanel6";
		this.dockPanel6.OriginalSize = new System.Drawing.Size(200, 51);
		this.dockPanel6.Size = new System.Drawing.Size(297, 238);
		this.dockPanel6.Text = "dockPanel6";
		this.dockPanel6_Container.Location = new System.Drawing.Point(4, 24);
		this.dockPanel6_Container.Name = "dockPanel6_Container";
		this.dockPanel6_Container.Size = new System.Drawing.Size(288, 210);
		this.dockPanel6_Container.TabIndex = 0;
		this.panelContainer1.Controls.Add(this.dockPanel6);
		this.panelContainer1.Controls.Add(this.dockPanel4);
		this.panelContainer1.Dock = DevExpress.XtraBars.Docking.DockingStyle.Bottom;
		this.panelContainer1.FloatVertical = true;
		this.panelContainer1.ID = new System.Guid("84b76cd8-c0ee-44d9-b672-c8c47969509e");
		this.panelContainer1.Location = new System.Drawing.Point(647, 469);
		this.panelContainer1.Name = "panelContainer1";
		this.panelContainer1.OriginalSize = new System.Drawing.Size(200, 238);
		this.panelContainer1.Size = new System.Drawing.Size(521, 238);
		this.panelContainer1.Text = "panelContainer1";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1168, 707);
		base.Controls.Add(this.dockPanel3);
		base.Controls.Add(this.panelContainer1);
		base.Controls.Add(this.dockPanel2);
		base.Controls.Add(this.dockPanel9);
		base.Controls.Add(this.dockPanel1);
		base.Controls.Add(this.dockPanel5);
		base.Name = "PosTest";
		this.Text = "PosTest";
		base.Load += new System.EventHandler(this.PosTest_Load);
		((System.ComponentModel.ISupportInitialize)this.dockManager1).EndInit();
		this.dockPanel5.ResumeLayout(false);
		this.dockPanel5_Container.ResumeLayout(false);
		this.dockPanel1.ResumeLayout(false);
		this.dockPanel1_Container.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.navigationFrame2).EndInit();
		this.navigationFrame2.ResumeLayout(false);
		this.navigationPage5.ResumeLayout(false);
		this.navigationPage6.ResumeLayout(false);
		this.navigationPage7.ResumeLayout(false);
		this.navigationPage8.ResumeLayout(false);
		this.dockPanel9.ResumeLayout(false);
		this.dockPanel9_Container.ResumeLayout(false);
		this.dockPanel2.ResumeLayout(false);
		this.dockPanel2_Container.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.navigationFrame1).EndInit();
		this.navigationFrame1.ResumeLayout(false);
		this.navigationPage1.ResumeLayout(false);
		this.navigationPage2.ResumeLayout(false);
		this.dockPanel4.ResumeLayout(false);
		this.dockPanel4_Container.ResumeLayout(false);
		this.dockPanel3.ResumeLayout(false);
		this.dockPanel3_Container.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.navigationFrame3).EndInit();
		this.navigationFrame3.ResumeLayout(false);
		this.navigationPage3.ResumeLayout(false);
		this.navigationPage4.ResumeLayout(false);
		this.dockPanel6.ResumeLayout(false);
		this.panelContainer1.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
