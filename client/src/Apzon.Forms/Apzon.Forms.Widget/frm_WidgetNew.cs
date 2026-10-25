using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraBars.Docking2010;
using DevExpress.XtraBars.Docking2010.Views;
using DevExpress.XtraBars.Docking2010.Views.NoDocuments;
using DevExpress.XtraBars.Docking2010.Views.Tabbed;
using DevExpress.XtraBars.Navigation;

namespace Apzon.Forms.Widget;

public class frm_WidgetNew : Form
{
	private IContainer components;

	private NavigationFrame navigationFrame1;

	private NavigationPage navigationPage1;

	private NavigationPage navigationPage2;

	private DocumentManager documentManager1;

	private NoDocumentsView noDocumentsView1;

	private TabbedView tabbedView1;

	public frm_WidgetNew()
	{
		InitializeComponent();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Widget.frm_WidgetNew));
		this.navigationFrame1 = new DevExpress.XtraBars.Navigation.NavigationFrame();
		this.navigationPage1 = new DevExpress.XtraBars.Navigation.NavigationPage();
		this.navigationPage2 = new DevExpress.XtraBars.Navigation.NavigationPage();
		this.documentManager1 = new DevExpress.XtraBars.Docking2010.DocumentManager(this.components);
		this.tabbedView1 = new DevExpress.XtraBars.Docking2010.Views.Tabbed.TabbedView(this.components);
		this.noDocumentsView1 = new DevExpress.XtraBars.Docking2010.Views.NoDocuments.NoDocumentsView(this.components);
		((System.ComponentModel.ISupportInitialize)this.navigationFrame1).BeginInit();
		this.navigationFrame1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.documentManager1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tabbedView1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.noDocumentsView1).BeginInit();
		base.SuspendLayout();
		this.navigationFrame1.Controls.Add(this.navigationPage1);
		this.navigationFrame1.Controls.Add(this.navigationPage2);
		this.navigationFrame1.Location = new System.Drawing.Point(4, 36);
		this.navigationFrame1.Name = "navigationFrame1";
		this.navigationFrame1.Pages.AddRange(new DevExpress.XtraBars.Navigation.NavigationPageBase[2] { this.navigationPage1, this.navigationPage2 });
		this.navigationFrame1.SelectedPage = this.navigationPage2;
		this.navigationFrame1.Size = new System.Drawing.Size(918, 638);
		this.navigationFrame1.TabIndex = 0;
		this.navigationFrame1.Text = "navigationFrame1";
		this.navigationPage1.Caption = "navigationPage1";
		this.navigationPage1.Name = "navigationPage1";
		this.navigationPage1.Size = new System.Drawing.Size(918, 638);
		this.navigationPage2.Caption = "navigationPage2";
		this.navigationPage2.Name = "navigationPage2";
		this.navigationPage2.Size = new System.Drawing.Size(918, 638);
		this.documentManager1.ContainerControl = this;
		this.documentManager1.View = this.tabbedView1;
		this.documentManager1.ViewCollection.AddRange(new DevExpress.XtraBars.Docking2010.Views.BaseView[2] { this.tabbedView1, this.noDocumentsView1 });
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(920, 673);
		base.Controls.Add(this.navigationFrame1);
		base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
		base.Name = "frm_WidgetNew";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		this.Text = "Create New Wiget";
		((System.ComponentModel.ISupportInitialize)this.navigationFrame1).EndInit();
		this.navigationFrame1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.documentManager1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tabbedView1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.noDocumentsView1).EndInit();
		base.ResumeLayout(false);
	}
}
