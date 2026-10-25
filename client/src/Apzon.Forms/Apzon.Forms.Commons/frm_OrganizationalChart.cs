using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Helper;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Form;
using Apzon.Forms.Helper;
using DevExpress.Diagram.Core;
using DevExpress.Utils;
using DevExpress.XtraDiagram;

namespace Apzon.Forms.Commons;

public class frm_OrganizationalChart : XtraFormEx
{
	public string CurrentKeyVal;

	private IContainer components;

	private DiagramControl diagramControl1;

	private DiagramOrgChartController diagramOrgChartController1;

	private SimpleButtonEx btn_OK;

	private DiagramConnector diagramConnector1;

	private DiagramContainer diagramContainer1;

	private DiagramShape diagramShape1;

	private DiagramShape diagramShape2;

	private DiagramShape diagramShape3;

	private DiagramShape diagramShape4;

	private DiagramShape diagramShape5;

	private DiagramShape diagramShape6;

	public string FormCaption { get; set; }

	public string Key { get; set; }

	public string Father { get; set; }

	public DataTable Source { get; set; }

	public frm_OrganizationalChart()
	{
		InitializeComponent();
		diagramControl1.DoubleClick += DiagramControl1OnDoubleClick;
		diagramControl1.CustomDrawItem += DiagramControl1OnCustomDrawItem;
	}

	private void DiagramControl1OnCustomDrawItem(object sender, CustomDrawItemEventArgs e)
	{
		if (e.Item.DataContext is Employee employee && employee.empID == Function.ParseInt(CurrentKeyVal))
		{
			e.Appearance.BackColor = Color.Yellow;
		}
	}

	private void DiagramControl1OnDoubleClick(object sender, EventArgs e)
	{
		DiagramItem diagramItem = diagramControl1.CalcHitItem(((MouseEventArgs)e).Location);
		if (diagramItem != null && diagramItem.DataContext is Employee employee)
		{
			this.DrillDown(ApzObjectTypes.oEmployeesInfo, ApzObjectTypes.oEmployeesInfo, Function.ToString(employee.empID));
		}
	}

	public async Task PRINTLAYOUT()
	{
		try
		{
			diagramControl1.ShowPrintPreview();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task PRINT()
	{
		try
		{
			diagramControl1.Print();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void frm_OrganizationalChart_Load(object sender, EventArgs e)
	{
		try
		{
			diagramOrgChartController1.ExpandSubordinatesButtonMode = ExpandSubordinatesButtonMode.LookupChildrenInSource;
			diagramOrgChartController1.KeyMember = Key;
			diagramOrgChartController1.ParentMember = Father;
			if (!Source.IsNotNull())
			{
				return;
			}
			Text = FormCaption;
			List<Employee> list = new List<Employee>();
			foreach (DataRow row in Source.Rows)
			{
				Employee employee = new Employee();
				employee.empID = Function.ParseInt(row["empID"]);
				employee.manager = Function.ParseInt(row["manager"]);
				employee.firstName = Function.ToString(row["firstName"]);
				employee.lastName = Function.ToString(row["lastName"]);
				employee.BranchName = Function.ToString(row["BranchName"]);
				employee.Department = Function.ToString(row["Department"]);
				employee.PositionName = Function.ToString(row["PositionName"]);
				employee.jobTitle = Function.ToString(row["jobTitle"]);
				employee.ExtEmpNo = Function.ToString(row["ExtEmpNo"]);
				list.Add(employee);
			}
			diagramOrgChartController1.DataSource = list;
			diagramOrgChartController1.Refresh();
			diagramControl1.FitToDrawing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_OK_Click(object sender, EventArgs e)
	{
		Close();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Commons.frm_OrganizationalChart));
		this.diagramControl1 = new DevExpress.XtraDiagram.DiagramControl();
		this.diagramOrgChartController1 = new DevExpress.XtraDiagram.DiagramOrgChartController(this.components);
		this.diagramConnector1 = new DevExpress.XtraDiagram.DiagramConnector();
		this.diagramContainer1 = new DevExpress.XtraDiagram.DiagramContainer();
		this.diagramShape1 = new DevExpress.XtraDiagram.DiagramShape();
		this.diagramShape2 = new DevExpress.XtraDiagram.DiagramShape();
		this.diagramShape3 = new DevExpress.XtraDiagram.DiagramShape();
		this.diagramShape4 = new DevExpress.XtraDiagram.DiagramShape();
		this.diagramShape5 = new DevExpress.XtraDiagram.DiagramShape();
		this.diagramShape6 = new DevExpress.XtraDiagram.DiagramShape();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.diagramControl1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.diagramOrgChartController1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.diagramOrgChartController1.TemplateDiagram).BeginInit();
		base.SuspendLayout();
		this.diagramControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.diagramControl1.Location = new System.Drawing.Point(1, 1);
		this.diagramControl1.Margin = new System.Windows.Forms.Padding(0);
		this.diagramControl1.Name = "diagramControl1";
		this.diagramControl1.OptionsBehavior.SelectedStencils = new DevExpress.Diagram.Core.StencilCollection("BasicShapes", "BasicFlowchartShapes");
		this.diagramControl1.OptionsView.BringIntoViewMargin = 1f;
		this.diagramControl1.OptionsView.FitToDrawingMargin = new System.Windows.Forms.Padding(0);
		this.diagramControl1.OptionsView.PageMargin = new System.Windows.Forms.Padding(0);
		this.diagramControl1.OptionsView.PaperKind = System.Drawing.Printing.PaperKind.Letter;
		this.diagramControl1.OptionsView.ScrollMargin = new System.Windows.Forms.Padding(0);
		this.diagramControl1.OptionsView.ShowGrid = false;
		this.diagramControl1.OptionsView.ShowPageBreaks = false;
		this.diagramControl1.OptionsView.ShowRulers = false;
		this.diagramControl1.OptionsView.ToolboxVisibility = DevExpress.Diagram.Core.ToolboxVisibility.Closed;
		this.diagramControl1.Size = new System.Drawing.Size(880, 532);
		this.diagramControl1.TabIndex = 0;
		this.diagramControl1.Text = "diagramControl1";
		this.diagramOrgChartController1.Diagram = this.diagramControl1;
		this.diagramOrgChartController1.ExpandSubordinatesButtonMode = DevExpress.Diagram.Core.ExpandSubordinatesButtonMode.LookupChildrenInSource;
		this.diagramOrgChartController1.KeyMember = "LineNum";
		this.diagramOrgChartController1.ParentMember = "FatherNum";
		this.diagramOrgChartController1.TemplateDiagram.Items.AddRange(this.diagramConnector1, this.diagramContainer1);
		this.diagramOrgChartController1.TemplateDiagram.Location = new System.Drawing.Point(0, 0);
		this.diagramOrgChartController1.TemplateDiagram.Name = "";
		this.diagramOrgChartController1.TemplateDiagram.OptionsBehavior.SelectedStencils = new DevExpress.Diagram.Core.StencilCollection("TemplateDesigner");
		this.diagramOrgChartController1.TemplateDiagram.OptionsView.CanvasSizeMode = DevExpress.Diagram.Core.CanvasSizeMode.Fill;
		this.diagramOrgChartController1.TemplateDiagram.OptionsView.PageSize = new System.Drawing.SizeF(313f, 258f);
		this.diagramOrgChartController1.TemplateDiagram.OptionsView.PaperKind = System.Drawing.Printing.PaperKind.Custom;
		this.diagramOrgChartController1.TemplateDiagram.OptionsView.ShowPageBreaks = false;
		this.diagramOrgChartController1.TemplateDiagram.TabIndex = 0;
		this.diagramConnector1.Appearance.ContentBackground = System.Drawing.Color.White;
		this.diagramConnector1.BeginPoint = new DevExpress.Utils.PointFloat(199f, 144f);
		this.diagramConnector1.CanChangeRoute = false;
		this.diagramConnector1.CanDragBeginPoint = false;
		this.diagramConnector1.CanDragEndPoint = false;
		this.diagramConnector1.EndPoint = new DevExpress.Utils.PointFloat(289f, 234f);
		this.diagramConnector1.Points = new DevExpress.XtraDiagram.PointCollection(new DevExpress.Utils.PointFloat[1]
		{
			new DevExpress.Utils.PointFloat(289f, 144f)
		});
		this.diagramConnector1.ThemeStyleId = DevExpress.Diagram.Core.DiagramShapeStyleId.Refined1;
		this.diagramContainer1.Anchors = DevExpress.Diagram.Core.Sides.Left | DevExpress.Diagram.Core.Sides.Top;
		this.diagramContainer1.CanCopyWithoutParent = true;
		this.diagramContainer1.ConnectionPoints = new DevExpress.XtraDiagram.PointCollection(new DevExpress.Utils.PointFloat[4]
		{
			new DevExpress.Utils.PointFloat(0.5f, 0f),
			new DevExpress.Utils.PointFloat(1f, 0.5f),
			new DevExpress.Utils.PointFloat(0.5f, 1f),
			new DevExpress.Utils.PointFloat(0f, 0.5f)
		});
		this.diagramContainer1.DragMode = DevExpress.Diagram.Core.ContainerDragMode.ByAnyPoint;
		this.diagramContainer1.Header = "";
		this.diagramContainer1.Items.AddRange(this.diagramShape1, this.diagramShape2, this.diagramShape3, this.diagramShape4, this.diagramShape5, this.diagramShape6);
		this.diagramContainer1.ItemsCanAttachConnectorBeginPoint = false;
		this.diagramContainer1.ItemsCanAttachConnectorEndPoint = false;
		this.diagramContainer1.ItemsCanCopyWithoutParent = false;
		this.diagramContainer1.ItemsCanDeleteWithoutParent = false;
		this.diagramContainer1.ItemsCanMove = false;
		this.diagramContainer1.ItemsCanResize = false;
		this.diagramContainer1.ItemsCanRotate = false;
		this.diagramContainer1.ItemsCanSelect = false;
		this.diagramContainer1.ItemsCanSnapToOtherItems = false;
		this.diagramContainer1.ItemsCanSnapToThisItem = false;
		this.diagramContainer1.Position = new DevExpress.Utils.PointFloat(24f, -15f);
		this.diagramContainer1.Shape = DevExpress.Diagram.Core.StandardContainers.Alternating;
		this.diagramContainer1.Size = new System.Drawing.SizeF(191f, 100f);
		this.diagramContainer1.ThemeStyleId = DevExpress.Diagram.Core.DiagramShapeStyleId.Balanced1;
		this.diagramShape1.Anchors = DevExpress.Diagram.Core.Sides.Left | DevExpress.Diagram.Core.Sides.Top;
		this.diagramShape1.Appearance.BorderColor = System.Drawing.Color.FromArgb(0, 255, 255, 255);
		this.diagramShape1.Appearance.BorderSize = 0;
		this.diagramShape1.Appearance.Font = new System.Drawing.Font("Tahoma", 12f, System.Drawing.FontStyle.Bold);
		this.diagramShape1.Appearance.ForeColor = System.Drawing.Color.FromArgb(0, 0, 0);
		this.diagramShape1.BackgroundId = DevExpress.Diagram.Core.DiagramThemeColorId.Accent1;
		this.diagramShape1.Bindings.Add(new DevExpress.Diagram.Core.DiagramBinding("Content", "lastName + ',' + firstName"));
		this.diagramShape1.CanAttachConnectorBeginPoint = false;
		this.diagramShape1.CanAttachConnectorEndPoint = false;
		this.diagramShape1.CanChangeParent = false;
		this.diagramShape1.CanCopy = false;
		this.diagramShape1.CanCopyWithoutParent = false;
		this.diagramShape1.CanDelete = false;
		this.diagramShape1.CanDeleteWithoutParent = false;
		this.diagramShape1.CanEdit = false;
		this.diagramShape1.CanMove = false;
		this.diagramShape1.CanResize = false;
		this.diagramShape1.CanRotate = false;
		this.diagramShape1.CanSelect = false;
		this.diagramShape1.Size = new System.Drawing.SizeF(190f, 20f);
		this.diagramShape2.Anchors = DevExpress.Diagram.Core.Sides.Left | DevExpress.Diagram.Core.Sides.Top;
		this.diagramShape2.Appearance.BackColor = System.Drawing.Color.FromArgb(0, 255, 255, 255);
		this.diagramShape2.Appearance.BorderColor = System.Drawing.Color.FromArgb(0, 255, 255, 255);
		this.diagramShape2.Appearance.BorderSize = 0;
		this.diagramShape2.Appearance.ForeColor = System.Drawing.Color.FromArgb(0, 0, 0);
		this.diagramShape2.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.diagramShape2.Bindings.Add(new DevExpress.Diagram.Core.DiagramBinding("Content", "jobTitle", DevExpress.Diagram.Core.DiagramBindingMode.TwoWay));
		this.diagramShape2.CanAttachConnectorBeginPoint = false;
		this.diagramShape2.CanAttachConnectorEndPoint = false;
		this.diagramShape2.CanChangeParent = false;
		this.diagramShape2.CanCopy = false;
		this.diagramShape2.CanCopyWithoutParent = false;
		this.diagramShape2.CanDelete = false;
		this.diagramShape2.CanDeleteWithoutParent = false;
		this.diagramShape2.CanEdit = false;
		this.diagramShape2.CanMove = false;
		this.diagramShape2.CanResize = false;
		this.diagramShape2.CanRotate = false;
		this.diagramShape2.CanSelect = false;
		this.diagramShape2.Position = new DevExpress.Utils.PointFloat(6f, 20f);
		this.diagramShape2.Size = new System.Drawing.SizeF(185f, 20f);
		this.diagramShape3.Anchors = DevExpress.Diagram.Core.Sides.Left | DevExpress.Diagram.Core.Sides.Top;
		this.diagramShape3.Appearance.BackColor = System.Drawing.Color.FromArgb(0, 255, 255, 255);
		this.diagramShape3.Appearance.BorderColor = System.Drawing.Color.FromArgb(0, 255, 255, 255);
		this.diagramShape3.Appearance.BorderSize = 0;
		this.diagramShape3.Appearance.ForeColor = System.Drawing.Color.FromArgb(0, 0, 0);
		this.diagramShape3.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.diagramShape3.Bindings.Add(new DevExpress.Diagram.Core.DiagramBinding("Content", "PositionName"));
		this.diagramShape3.CanAttachConnectorBeginPoint = false;
		this.diagramShape3.CanAttachConnectorEndPoint = false;
		this.diagramShape3.CanChangeParent = false;
		this.diagramShape3.CanCopy = false;
		this.diagramShape3.CanCopyWithoutParent = false;
		this.diagramShape3.CanDelete = false;
		this.diagramShape3.CanDeleteWithoutParent = false;
		this.diagramShape3.CanEdit = false;
		this.diagramShape3.CanMove = false;
		this.diagramShape3.CanResize = false;
		this.diagramShape3.CanRotate = false;
		this.diagramShape3.CanSelect = false;
		this.diagramShape3.Position = new DevExpress.Utils.PointFloat(6f, 35f);
		this.diagramShape3.Size = new System.Drawing.SizeF(185f, 20f);
		this.diagramShape4.Anchors = DevExpress.Diagram.Core.Sides.Left | DevExpress.Diagram.Core.Sides.Top;
		this.diagramShape4.Appearance.BackColor = System.Drawing.Color.FromArgb(0, 255, 255, 255);
		this.diagramShape4.Appearance.BorderColor = System.Drawing.Color.FromArgb(0, 255, 255, 255);
		this.diagramShape4.Appearance.BorderSize = 0;
		this.diagramShape4.Appearance.ForeColor = System.Drawing.Color.FromArgb(0, 0, 0);
		this.diagramShape4.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.diagramShape4.Bindings.Add(new DevExpress.Diagram.Core.DiagramBinding("Content", "Department"));
		this.diagramShape4.CanAttachConnectorBeginPoint = false;
		this.diagramShape4.CanAttachConnectorEndPoint = false;
		this.diagramShape4.CanChangeParent = false;
		this.diagramShape4.CanCopy = false;
		this.diagramShape4.CanCopyWithoutParent = false;
		this.diagramShape4.CanDelete = false;
		this.diagramShape4.CanDeleteWithoutParent = false;
		this.diagramShape4.CanEdit = false;
		this.diagramShape4.CanMove = false;
		this.diagramShape4.CanResize = false;
		this.diagramShape4.CanRotate = false;
		this.diagramShape4.CanSelect = false;
		this.diagramShape4.Position = new DevExpress.Utils.PointFloat(6f, 50f);
		this.diagramShape4.Size = new System.Drawing.SizeF(185f, 20f);
		this.diagramShape5.Anchors = DevExpress.Diagram.Core.Sides.Left | DevExpress.Diagram.Core.Sides.Top;
		this.diagramShape5.Appearance.BackColor = System.Drawing.Color.FromArgb(0, 255, 255, 255);
		this.diagramShape5.Appearance.BorderColor = System.Drawing.Color.FromArgb(0, 255, 255, 255);
		this.diagramShape5.Appearance.BorderSize = 0;
		this.diagramShape5.Appearance.ForeColor = System.Drawing.Color.FromArgb(0, 0, 0);
		this.diagramShape5.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.diagramShape5.Bindings.Add(new DevExpress.Diagram.Core.DiagramBinding("Content", "BranchName"));
		this.diagramShape5.CanAttachConnectorBeginPoint = false;
		this.diagramShape5.CanAttachConnectorEndPoint = false;
		this.diagramShape5.CanChangeParent = false;
		this.diagramShape5.CanCopy = false;
		this.diagramShape5.CanCopyWithoutParent = false;
		this.diagramShape5.CanDelete = false;
		this.diagramShape5.CanDeleteWithoutParent = false;
		this.diagramShape5.CanEdit = false;
		this.diagramShape5.CanMove = false;
		this.diagramShape5.CanResize = false;
		this.diagramShape5.CanRotate = false;
		this.diagramShape5.CanSelect = false;
		this.diagramShape5.Position = new DevExpress.Utils.PointFloat(6f, 65f);
		this.diagramShape5.Size = new System.Drawing.SizeF(185f, 20f);
		this.diagramShape6.Anchors = DevExpress.Diagram.Core.Sides.Left | DevExpress.Diagram.Core.Sides.Top;
		this.diagramShape6.Appearance.BackColor = System.Drawing.Color.FromArgb(0, 255, 255, 255);
		this.diagramShape6.Appearance.BorderColor = System.Drawing.Color.FromArgb(0, 255, 255, 255);
		this.diagramShape6.Appearance.BorderSize = 0;
		this.diagramShape6.Appearance.ForeColor = System.Drawing.Color.FromArgb(0, 0, 0);
		this.diagramShape6.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.diagramShape6.Bindings.Add(new DevExpress.Diagram.Core.DiagramBinding("Content", "ExtEmpNo", DevExpress.Diagram.Core.DiagramBindingMode.TwoWay));
		this.diagramShape6.CanAttachConnectorBeginPoint = false;
		this.diagramShape6.CanAttachConnectorEndPoint = false;
		this.diagramShape6.CanChangeParent = false;
		this.diagramShape6.CanCopy = false;
		this.diagramShape6.CanCopyWithoutParent = false;
		this.diagramShape6.CanDelete = false;
		this.diagramShape6.CanDeleteWithoutParent = false;
		this.diagramShape6.CanEdit = false;
		this.diagramShape6.CanMove = false;
		this.diagramShape6.CanResize = false;
		this.diagramShape6.CanRotate = false;
		this.diagramShape6.CanSelect = false;
		this.diagramShape6.Position = new DevExpress.Utils.PointFloat(7.05664f, 80f);
		this.diagramShape6.Size = new System.Drawing.SizeF(183.0628f, 20f);
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.FieldName = null;
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(1, 539);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.Size = new System.Drawing.Size(75, 23);
		this.btn_OK.TabIndex = 1;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(882, 568);
		base.Controls.Add(this.btn_OK);
		base.Controls.Add(this.diagramControl1);
		base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
		base.Name = "frm_OrganizationalChart";
		this.Text = "frm_ProjectManagerOrgChart";
		base.Load += new System.EventHandler(this.frm_OrganizationalChart_Load);
		((System.ComponentModel.ISupportInitialize)this.diagramControl1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.diagramOrgChartController1.TemplateDiagram).EndInit();
		((System.ComponentModel.ISupportInitialize)this.diagramOrgChartController1).EndInit();
		base.ResumeLayout(false);
	}
}
