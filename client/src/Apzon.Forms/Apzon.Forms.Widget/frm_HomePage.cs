using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Controls.Form;
using DevExpress.Utils;
using DevExpress.XtraBars.Docking2010;
using DevExpress.XtraBars.Docking2010.Views;
using DevExpress.XtraBars.Docking2010.Views.Widget;
using DevExpress.XtraEditors;

namespace Apzon.Forms.Widget;

public class frm_HomePage : XtraFormEx
{
	private IContainer components;

	private DocumentManager documentManager1;

	private WidgetView widget_Home;

	private Document document_SalesWidget;

	private ColumnDefinition columnDefinition1;

	private ColumnDefinition columnDefinition2;

	private ColumnDefinition columnDefinition3;

	private ColumnDefinition columnDefinition4;

	private ColumnDefinition columnDefinition5;

	private ColumnDefinition columnDefinition6;

	private ColumnDefinition columnDefinition7;

	private ColumnDefinition columnDefinition8;

	private RowDefinition rowDefinition1;

	private RowDefinition rowDefinition2;

	private RowDefinition rowDefinition3;

	private RowDefinition rowDefinition4;

	private RowDefinition rowDefinition5;

	private RowDefinition rowDefinition6;

	private RowDefinition rowDefinition7;

	private RowDefinition rowDefinition8;

	private Document document_MainModules;

	public frm_HomePage()
	{
		InitializeComponent();
		widget_Home.QueryControl += widget_Home_QueryControl;
	}

	private void widget_Home_QueryControl(object sender, QueryControlEventArgs e)
	{
		try
		{
			if (!string.IsNullOrEmpty(e.Document.ControlTypeName))
			{
				Control control = Activator.CreateInstance(Type.GetType(e.Document.ControlTypeName)) as Control;
				e.Control = control;
			}
			else
			{
				e.Control = new System.Windows.Forms.UserControl();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			e.Control = new System.Windows.Forms.UserControl();
		}
	}

	private void frm_HomePage_Load(object sender, EventArgs e)
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Widget.frm_HomePage));
		this.documentManager1 = new DevExpress.XtraBars.Docking2010.DocumentManager(this.components);
		this.widget_Home = new DevExpress.XtraBars.Docking2010.Views.Widget.WidgetView(this.components);
		this.columnDefinition1 = new DevExpress.XtraBars.Docking2010.Views.Widget.ColumnDefinition();
		this.columnDefinition2 = new DevExpress.XtraBars.Docking2010.Views.Widget.ColumnDefinition();
		this.columnDefinition3 = new DevExpress.XtraBars.Docking2010.Views.Widget.ColumnDefinition();
		this.columnDefinition4 = new DevExpress.XtraBars.Docking2010.Views.Widget.ColumnDefinition();
		this.columnDefinition5 = new DevExpress.XtraBars.Docking2010.Views.Widget.ColumnDefinition();
		this.columnDefinition6 = new DevExpress.XtraBars.Docking2010.Views.Widget.ColumnDefinition();
		this.columnDefinition7 = new DevExpress.XtraBars.Docking2010.Views.Widget.ColumnDefinition();
		this.columnDefinition8 = new DevExpress.XtraBars.Docking2010.Views.Widget.ColumnDefinition();
		this.document_MainModules = new DevExpress.XtraBars.Docking2010.Views.Widget.Document(this.components);
		this.rowDefinition1 = new DevExpress.XtraBars.Docking2010.Views.Widget.RowDefinition();
		this.rowDefinition2 = new DevExpress.XtraBars.Docking2010.Views.Widget.RowDefinition();
		this.rowDefinition3 = new DevExpress.XtraBars.Docking2010.Views.Widget.RowDefinition();
		this.rowDefinition4 = new DevExpress.XtraBars.Docking2010.Views.Widget.RowDefinition();
		this.rowDefinition5 = new DevExpress.XtraBars.Docking2010.Views.Widget.RowDefinition();
		this.rowDefinition6 = new DevExpress.XtraBars.Docking2010.Views.Widget.RowDefinition();
		this.rowDefinition7 = new DevExpress.XtraBars.Docking2010.Views.Widget.RowDefinition();
		this.rowDefinition8 = new DevExpress.XtraBars.Docking2010.Views.Widget.RowDefinition();
		this.document_SalesWidget = new DevExpress.XtraBars.Docking2010.Views.Widget.Document(this.components);
		((System.ComponentModel.ISupportInitialize)this.documentManager1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.widget_Home).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition6).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition7).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition8).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.document_MainModules).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition6).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition7).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition8).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.document_SalesWidget).BeginInit();
		base.SuspendLayout();
		this.documentManager1.ContainerControl = this;
		this.documentManager1.View = this.widget_Home;
		this.documentManager1.ViewCollection.AddRange(new DevExpress.XtraBars.Docking2010.Views.BaseView[1] { this.widget_Home });
		this.widget_Home.AllowStartupAnimation = DevExpress.Utils.DefaultBoolean.False;
		this.widget_Home.Columns.AddRange(new DevExpress.XtraBars.Docking2010.Views.Widget.ColumnDefinition[8] { this.columnDefinition1, this.columnDefinition2, this.columnDefinition3, this.columnDefinition4, this.columnDefinition5, this.columnDefinition6, this.columnDefinition7, this.columnDefinition8 });
		this.widget_Home.Documents.AddRange(new DevExpress.XtraBars.Docking2010.Views.BaseDocument[1] { this.document_MainModules });
		this.widget_Home.LayoutMode = DevExpress.XtraBars.Docking2010.Views.Widget.LayoutMode.TableLayout;
		this.widget_Home.RootContainer.Element = null;
		this.widget_Home.RootContainer.Orientation = System.Windows.Forms.Orientation.Vertical;
		this.widget_Home.Rows.AddRange(new DevExpress.XtraBars.Docking2010.Views.Widget.RowDefinition[8] { this.rowDefinition1, this.rowDefinition2, this.rowDefinition3, this.rowDefinition4, this.rowDefinition5, this.rowDefinition6, this.rowDefinition7, this.rowDefinition8 });
		this.document_MainModules.Caption = "Main Modules";
		this.document_MainModules.ColumnSpan = 8;
		this.document_MainModules.ControlName = "ucMainProcessModel";
		this.document_MainModules.ControlTypeName = "Apzon.Forms.Widget.ucMainProcessModel";
		this.document_MainModules.Properties.AllowClose = DevExpress.Utils.DefaultBoolean.False;
		this.document_MainModules.Properties.ShowBorders = DevExpress.Utils.DefaultBoolean.False;
		this.document_MainModules.Properties.ShowMaximizeButton = DevExpress.Utils.DefaultBoolean.False;
		this.document_MainModules.RowSpan = 8;
		this.document_SalesWidget.Caption = "Sales A/R Process Model";
		this.document_SalesWidget.ColumnIndex = 5;
		this.document_SalesWidget.ColumnSpan = 3;
		this.document_SalesWidget.ControlName = "ucSalesProcessModel";
		this.document_SalesWidget.ControlTypeName = "Apzon.Forms.Widget.ucSalesProcessModel";
		this.document_SalesWidget.RowSpan = 3;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1085, 695);
		base.FormBorderEffect = DevExpress.XtraEditors.FormBorderEffect.None;
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
		base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
		base.Name = "frm_HomePage";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		this.Text = "Main Modules";
		base.Load += new System.EventHandler(this.frm_HomePage_Load);
		((System.ComponentModel.ISupportInitialize)this.documentManager1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.widget_Home).EndInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition6).EndInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition7).EndInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition8).EndInit();
		((System.ComponentModel.ISupportInitialize)this.document_MainModules).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition6).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition7).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition8).EndInit();
		((System.ComponentModel.ISupportInitialize)this.document_SalesWidget).EndInit();
		base.ResumeLayout(false);
	}
}
