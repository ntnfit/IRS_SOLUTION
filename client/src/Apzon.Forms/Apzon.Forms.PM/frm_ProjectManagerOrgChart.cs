using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Helper;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Form;
using Apzon.Entities.Models.PM;
using DevExpress.Diagram.Core;
using DevExpress.Utils;
using DevExpress.XtraDiagram;

namespace Apzon.Forms.PM;

public class frm_ProjectManagerOrgChart : XtraFormEx
{
	private IContainer components;

	private DiagramControl diagramControl1;

	private DiagramOrgChartController diagramOrgChartController1;

	private SimpleButtonEx btn_OK;

	private DiagramConnector diagramConnector1;

	private DiagramContainer diagramContainer1;

	private DiagramShape diagramShape1;

	public DataTable Source { get; set; }

	public DataRow Project { get; set; }

	public frm_ProjectManagerOrgChart()
	{
		InitializeComponent();
	}

	private void frm_ProjectManagerOrgChart_Load(object sender, EventArgs e)
	{
		try
		{
			if (!Source.IsNotNull() || Project == null)
			{
				return;
			}
			List<ProjectManagementImpl> list = new List<ProjectManagementImpl>();
			ProjectManagementImpl projectManagementImpl = new ProjectManagementImpl();
			projectManagementImpl.LineNum = 0;
			projectManagementImpl.FatherNum = -1;
			projectManagementImpl.TaskCode = Function.ToString(Project["Code"]);
			projectManagementImpl.TaskName = Function.ToString(Project["Name"]);
			Text = $"{projectManagementImpl.TaskName} - Work Breakdown Structure";
			list.Add(projectManagementImpl);
			foreach (DataRow row in Source.Rows)
			{
				ProjectManagementImpl projectManagementImpl2 = new ProjectManagementImpl();
				projectManagementImpl2.LineNum = Function.ParseInt(row["LineNum"]);
				projectManagementImpl2.FatherNum = Function.ParseInt(row["FatherNum"]);
				projectManagementImpl2.TaskCode = Function.ToString(row["TaskCode"]);
				string text = Function.ToString(row["TaskName"]);
				projectManagementImpl2.Wbs = Function.ToString(row["Wbs"]);
				projectManagementImpl2.TaskName = $"{projectManagementImpl2.Wbs} - {(string.IsNullOrEmpty(text) ? projectManagementImpl2.TaskCode : text)}";
				list.Add(projectManagementImpl2);
			}
			diagramOrgChartController1.DataSource = list;
			diagramOrgChartController1.Refresh();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.PM.frm_ProjectManagerOrgChart));
		this.diagramControl1 = new DevExpress.XtraDiagram.DiagramControl();
		this.diagramOrgChartController1 = new DevExpress.XtraDiagram.DiagramOrgChartController(this.components);
		this.diagramConnector1 = new DevExpress.XtraDiagram.DiagramConnector();
		this.diagramContainer1 = new DevExpress.XtraDiagram.DiagramContainer();
		this.diagramShape1 = new DevExpress.XtraDiagram.DiagramShape();
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
		this.diagramOrgChartController1.TemplateDiagram.OptionsView.PageSize = new System.Drawing.SizeF(763f, 238f);
		this.diagramOrgChartController1.TemplateDiagram.OptionsView.PaperKind = System.Drawing.Printing.PaperKind.Custom;
		this.diagramOrgChartController1.TemplateDiagram.OptionsView.ShowPageBreaks = false;
		this.diagramOrgChartController1.TemplateDiagram.TabIndex = 0;
		this.diagramConnector1.Appearance.ContentBackground = System.Drawing.Color.White;
		this.diagramConnector1.BeginPoint = new DevExpress.Utils.PointFloat(400f, -875f);
		this.diagramConnector1.CanChangeRoute = false;
		this.diagramConnector1.CanDragBeginPoint = false;
		this.diagramConnector1.CanDragEndPoint = false;
		this.diagramConnector1.EndPoint = new DevExpress.Utils.PointFloat(490f, -785f);
		this.diagramConnector1.Points = new DevExpress.XtraDiagram.PointCollection(new DevExpress.Utils.PointFloat[1]
		{
			new DevExpress.Utils.PointFloat(490f, -875f)
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
		this.diagramContainer1.Items.AddRange(this.diagramShape1);
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
		this.diagramContainer1.Position = new DevExpress.Utils.PointFloat(225f, -975f);
		this.diagramContainer1.Shape = DevExpress.Diagram.Core.StandardContainers.Alternating;
		this.diagramContainer1.Size = new System.Drawing.SizeF(125f, 50f);
		this.diagramContainer1.ThemeStyleId = DevExpress.Diagram.Core.DiagramShapeStyleId.Balanced1;
		this.diagramShape1.Anchors = DevExpress.Diagram.Core.Sides.Left | DevExpress.Diagram.Core.Sides.Top;
		this.diagramShape1.Appearance.BackColor = System.Drawing.Color.FromArgb(0, 255, 255, 255);
		this.diagramShape1.Appearance.BorderColor = System.Drawing.Color.FromArgb(0, 255, 255, 255);
		this.diagramShape1.Appearance.BorderSize = 0;
		this.diagramShape1.Appearance.Font = new System.Drawing.Font("Tahoma", 12f);
		this.diagramShape1.Appearance.ForeColor = System.Drawing.Color.FromArgb(0, 0, 0);
		this.diagramShape1.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
		this.diagramShape1.Bindings.Add(new DevExpress.Diagram.Core.DiagramBinding("Content", "TaskName", DevExpress.Diagram.Core.DiagramBindingMode.TwoWay));
		this.diagramShape1.CanAttachConnectorBeginPoint = false;
		this.diagramShape1.CanAttachConnectorEndPoint = false;
		this.diagramShape1.CanChangeParent = false;
		this.diagramShape1.CanCopyWithoutParent = false;
		this.diagramShape1.CanDeleteWithoutParent = false;
		this.diagramShape1.CanEdit = false;
		this.diagramShape1.CanMove = false;
		this.diagramShape1.CanResize = false;
		this.diagramShape1.CanRotate = false;
		this.diagramShape1.CanSelect = false;
		this.diagramShape1.Size = new System.Drawing.SizeF(125f, 50f);
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
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
		base.Name = "frm_ProjectManagerOrgChart";
		this.Text = "frm_ProjectManagerOrgChart";
		base.Load += new System.EventHandler(this.frm_ProjectManagerOrgChart_Load);
		((System.ComponentModel.ISupportInitialize)this.diagramControl1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.diagramOrgChartController1.TemplateDiagram).EndInit();
		((System.ComponentModel.ISupportInitialize)this.diagramOrgChartController1).EndInit();
		base.ResumeLayout(false);
	}
}
