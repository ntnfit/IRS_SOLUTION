using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Controls;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraBars.Docking2010;
using DevExpress.XtraBars.Docking2010.Views;
using DevExpress.XtraBars.Docking2010.Views.Tabbed;
using DevExpress.XtraBars.Docking2010.Views.Widget;
using DevExpress.XtraEditors;

namespace Apzon.Forms.Widget;

public class UcWidgetViewer : XtraUserControl
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private const string TableHeaderName = "APZ_OWID";

	private const string TableDocumentsName = "APZ_WID1";

	private IContainer components;

	private DocumentManager documentManager1;

	private WidgetView widgetView1;

	private StackGroup stackGroup1;

	private StackGroup stackGroup2;

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

	public int WidgetEntry { get; set; }

	public Action<DocumentEventArgs> DocumentClosed { get; set; }

	public Action<object> ControlResized { get; set; }

	private DataTable DtDocuments { get; set; }

	private DataTable DtHeader { get; set; }

	private DataTable DtChooseItem { get; set; }

	public WidgetView WidgetView => widgetView1;

	public UcWidgetViewer()
	{
		InitializeComponent();
		widgetView1.QueryControl += widget_QueryControl;
		widgetView1.DocumentClosed += WidgetView1_DocumentClosed;
		widgetView1.BeginSizing += WidgetView1_BeginSizing;
	}

	private void WidgetView1_BeginSizing(object sender, LayoutBeginSizingEventArgs e)
	{
	}

	private void WidgetView1_DocumentClosed(object sender, DocumentEventArgs e)
	{
		try
		{
			if (DocumentClosed != null)
			{
				DocumentClosed(e);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void widget_QueryControl(object sender, QueryControlEventArgs e)
	{
		WinCommons._OpenProcessing();
		try
		{
			if (!string.IsNullOrEmpty(e.Document.ControlTypeName))
			{
				if (Function.ParseInt(e.Document.ControlTypeName) == 7000030)
				{
					UcDashboardViewer control = new UcDashboardViewer(Function.ParseInt(e.Document.ControlName));
					e.Control = control;
				}
				else if (Type.GetType(e.Document.ControlTypeName) != null && Activator.CreateInstance(Type.GetType(e.Document.ControlTypeName)) is Control control2)
				{
					e.Control = control2;
				}
				if (e.Document is DevExpress.XtraBars.Docking2010.Views.Widget.Document)
				{
					if (((DevExpress.XtraBars.Docking2010.Views.Widget.Document)e.Document).Width == 0)
					{
						((DevExpress.XtraBars.Docking2010.Views.Widget.Document)e.Document).Width = 400;
					}
					if (((DevExpress.XtraBars.Docking2010.Views.Widget.Document)e.Document).Height == 0)
					{
						((DevExpress.XtraBars.Docking2010.Views.Widget.Document)e.Document).Height = 400;
					}
				}
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
		WinCommons._CloseProcessing();
	}

	private async void UcWidgetViewer_Load(object sender, EventArgs e)
	{
		_ = 2;
		try
		{
			WidgetEntry = -1;
			if (WidgetEntry != 0)
			{
				DtHeader = await _httpClient.Widget.GetHeader(WidgetEntry);
				if (DtHeader.IsNotNull())
				{
					Text = Function.ToString(DtHeader.Rows[0]["Caption"]);
					widgetView1.LayoutMode = (LayoutMode)Function.ParseInt(DtHeader.Rows[0]["LayoutMode"]);
				}
				DtDocuments = await _httpClient.Widget.GetDocument(WidgetEntry);
				if (DtDocuments.IsNotNull())
				{
					for (int i = 0; i < DtDocuments.Rows.Count; i++)
					{
						DevExpress.XtraBars.Docking2010.Views.Widget.Document document = new DevExpress.XtraBars.Docking2010.Views.Widget.Document();
						document.Caption = Function.ToString(DtDocuments.Rows[i]["Caption"]);
						document.ColumnIndex = Function.ParseInt(DtDocuments.Rows[i]["ColumnIndex"]);
						document.ColumnSpan = Function.ParseInt(DtDocuments.Rows[i]["ColumnSpan"]);
						document.ControlName = Function.ToString(DtDocuments.Rows[i]["ObjCode"]);
						document.ControlTypeName = Function.ToString(DtDocuments.Rows[i]["ObjType"]);
						document.RowIndex = Function.ParseInt(DtDocuments.Rows[i]["RowIndex"]);
						document.RowSpan = Function.ParseInt(DtDocuments.Rows[i]["RowSpan"]);
						document.Height = Function.ParseInt(DtDocuments.Rows[i]["Height"]);
						document.Width = Function.ParseInt(DtDocuments.Rows[i]["Width"]);
						document.FreeLayoutHeight.UnitType = LengthUnitType.Star;
						document.FreeLayoutHeight.UnitValue = Function.ParseDouble(DtDocuments.Rows[i]["FreeLayoutHeight"]);
						document.FreeLayoutWidth.UnitType = LengthUnitType.Star;
						document.FreeLayoutWidth.UnitValue = Function.ParseDouble(DtDocuments.Rows[i]["FreeLayoutWidth"]);
						document.ToolTip = Function.ToString(DtDocuments.Rows[i]["ToolTip"]);
						DevExpress.XtraBars.Docking2010.Views.Widget.Document document2 = document;
						widgetView1.Documents.Add(document2);
						int num = Function.ParseInt(DtDocuments.Rows[i]["StackGroup"]);
						if (widgetView1.StackGroups != null && widgetView1.StackGroups.Count > num)
						{
							widgetView1.Controller.Dock(document2, widgetView1.StackGroups[num]);
						}
					}
				}
			}
			DtChooseItem = await _httpClient.Widget.GetItems(0);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void UcWidgetViewer_ControlRemoved(object sender, ControlEventArgs e)
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
		DevExpress.XtraBars.Docking2010.Views.Widget.WidgetDockingContainer widgetDockingContainer = new DevExpress.XtraBars.Docking2010.Views.Widget.WidgetDockingContainer();
		this.documentManager1 = new DevExpress.XtraBars.Docking2010.DocumentManager(this.components);
		this.widgetView1 = new DevExpress.XtraBars.Docking2010.Views.Widget.WidgetView(this.components);
		this.columnDefinition1 = new DevExpress.XtraBars.Docking2010.Views.Widget.ColumnDefinition();
		this.columnDefinition2 = new DevExpress.XtraBars.Docking2010.Views.Widget.ColumnDefinition();
		this.columnDefinition3 = new DevExpress.XtraBars.Docking2010.Views.Widget.ColumnDefinition();
		this.columnDefinition4 = new DevExpress.XtraBars.Docking2010.Views.Widget.ColumnDefinition();
		this.columnDefinition5 = new DevExpress.XtraBars.Docking2010.Views.Widget.ColumnDefinition();
		this.columnDefinition6 = new DevExpress.XtraBars.Docking2010.Views.Widget.ColumnDefinition();
		this.columnDefinition7 = new DevExpress.XtraBars.Docking2010.Views.Widget.ColumnDefinition();
		this.columnDefinition8 = new DevExpress.XtraBars.Docking2010.Views.Widget.ColumnDefinition();
		this.rowDefinition1 = new DevExpress.XtraBars.Docking2010.Views.Widget.RowDefinition();
		this.rowDefinition2 = new DevExpress.XtraBars.Docking2010.Views.Widget.RowDefinition();
		this.rowDefinition3 = new DevExpress.XtraBars.Docking2010.Views.Widget.RowDefinition();
		this.rowDefinition4 = new DevExpress.XtraBars.Docking2010.Views.Widget.RowDefinition();
		this.rowDefinition5 = new DevExpress.XtraBars.Docking2010.Views.Widget.RowDefinition();
		this.rowDefinition6 = new DevExpress.XtraBars.Docking2010.Views.Widget.RowDefinition();
		this.rowDefinition7 = new DevExpress.XtraBars.Docking2010.Views.Widget.RowDefinition();
		this.rowDefinition8 = new DevExpress.XtraBars.Docking2010.Views.Widget.RowDefinition();
		this.stackGroup1 = new DevExpress.XtraBars.Docking2010.Views.Widget.StackGroup(this.components);
		this.stackGroup2 = new DevExpress.XtraBars.Docking2010.Views.Widget.StackGroup(this.components);
		((System.ComponentModel.ISupportInitialize)this.documentManager1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.widgetView1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition6).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition7).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition8).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition6).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition7).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition8).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.stackGroup1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.stackGroup2).BeginInit();
		base.SuspendLayout();
		this.documentManager1.ContainerControl = this;
		this.documentManager1.View = this.widgetView1;
		this.documentManager1.ViewCollection.AddRange(new DevExpress.XtraBars.Docking2010.Views.BaseView[1] { this.widgetView1 });
		this.widgetView1.Columns.AddRange(new DevExpress.XtraBars.Docking2010.Views.Widget.ColumnDefinition[8] { this.columnDefinition1, this.columnDefinition2, this.columnDefinition3, this.columnDefinition4, this.columnDefinition5, this.columnDefinition6, this.columnDefinition7, this.columnDefinition8 });
		this.widgetView1.DocumentProperties.ShowCloseButton = false;
		this.widgetView1.DocumentProperties.ShowMaximizeButton = false;
		this.widgetView1.LayoutMode = DevExpress.XtraBars.Docking2010.Views.Widget.LayoutMode.TableLayout;
		this.widgetView1.LoadingIndicatorProperties.Caption = "Loading ...";
		this.widgetView1.LoadingIndicatorProperties.Description = "Please Wait...";
		this.widgetView1.RootContainer.Nodes.AddRange(new DevExpress.XtraBars.Docking2010.Views.Tabbed.DockingContainer[1] { widgetDockingContainer });
		this.widgetView1.RootContainer.Orientation = System.Windows.Forms.Orientation.Vertical;
		this.widgetView1.Rows.AddRange(new DevExpress.XtraBars.Docking2010.Views.Widget.RowDefinition[8] { this.rowDefinition1, this.rowDefinition2, this.rowDefinition3, this.rowDefinition4, this.rowDefinition5, this.rowDefinition6, this.rowDefinition7, this.rowDefinition8 });
		this.widgetView1.StackGroups.AddRange(new DevExpress.XtraBars.Docking2010.Views.Widget.StackGroup[2] { this.stackGroup1, this.stackGroup2 });
		this.widgetView1.UseDocumentSelector = DevExpress.Utils.DefaultBoolean.False;
		this.widgetView1.UseLoadingIndicator = DevExpress.Utils.DefaultBoolean.True;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Name = "UcWidgetViewer";
		base.Size = new System.Drawing.Size(733, 565);
		base.Load += new System.EventHandler(this.UcWidgetViewer_Load);
		base.ControlRemoved += new System.Windows.Forms.ControlEventHandler(this.UcWidgetViewer_ControlRemoved);
		((System.ComponentModel.ISupportInitialize)this.documentManager1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.widgetView1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition6).EndInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition7).EndInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition8).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition6).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition7).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition8).EndInit();
		((System.ComponentModel.ISupportInitialize)this.stackGroup1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.stackGroup2).EndInit();
		base.ResumeLayout(false);
	}
}
