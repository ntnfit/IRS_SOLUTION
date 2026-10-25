using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Forms.Helper;
using DevExpress.Diagram.Core;
using DevExpress.Utils;
using DevExpress.XtraDiagram;

namespace Apzon.Forms.Widget;

public class ucSalesProcessModel : System.Windows.Forms.UserControl
{
	private IContainer components;

	private DiagramControlEx diagramControl1;

	private DiagramShape diagramShape1;

	private DiagramShape diagramShape2;

	private DiagramShape diagramShape3;

	private DiagramShape diagramShape4;

	private DiagramShape diagramShape5;

	private DiagramShape diagramShape6;

	private DiagramShape diagramShape7;

	private DiagramConnector diagramConnector1;

	private DiagramConnector diagramConnector2;

	private DiagramConnector diagramConnector3;

	private DiagramConnector diagramConnector4;

	private DiagramConnector diagramConnector5;

	private DiagramConnector diagramConnector6;

	private DiagramConnector diagramConnector7;

	private DiagramImageEx diagramImage1;

	private DiagramImageEx diagramImage2;

	private DiagramImageEx diagramImage3;

	private DiagramImageEx diagramImage4;

	private DiagramImageEx diagramImage5;

	private DiagramImageEx diagramImage6;

	private DiagramImageEx diagramImage7;

	private DiagramShape diagramShape8;

	private DiagramShape diagramShape9;

	private DiagramShape diagramShape10;

	private DiagramShape diagramShape11;

	private DiagramShape diagramShape12;

	private DiagramShape diagramShape13;

	private DiagramShape diagramShape14;

	private DiagramImageEx diagramImage8;

	private DiagramShape diagramShape15;

	public ucSalesProcessModel()
	{
		InitializeComponent();
	}

	private void Form1_Load(object sender, EventArgs e)
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

	private void Form1_Resize(object sender, EventArgs e)
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

	private void diagramControl1_MouseHover(object sender, EventArgs e)
	{
	}

	private void diagramControl1_MouseMove(object sender, MouseEventArgs e)
	{
		try
		{
			diagramControl1.CalcHitInfo(e.Location);
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
			if (diagramControl1.CalcHitItem(e.Location) is DiagramImageEx diagramImageEx && string.IsNullOrEmpty(diagramImageEx.ObjectGroup))
			{
				this.ShowDocumentForm(diagramImageEx.ObjType, checkMaximized: true);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Widget.ucSalesProcessModel));
		this.diagramControl1 = new Apzon.Forms.Widget.DiagramControlEx();
		this.diagramShape1 = new DevExpress.XtraDiagram.DiagramShape();
		this.diagramShape2 = new DevExpress.XtraDiagram.DiagramShape();
		this.diagramShape3 = new DevExpress.XtraDiagram.DiagramShape();
		this.diagramShape4 = new DevExpress.XtraDiagram.DiagramShape();
		this.diagramShape5 = new DevExpress.XtraDiagram.DiagramShape();
		this.diagramShape6 = new DevExpress.XtraDiagram.DiagramShape();
		this.diagramShape7 = new DevExpress.XtraDiagram.DiagramShape();
		this.diagramConnector1 = new DevExpress.XtraDiagram.DiagramConnector();
		this.diagramConnector2 = new DevExpress.XtraDiagram.DiagramConnector();
		this.diagramConnector3 = new DevExpress.XtraDiagram.DiagramConnector();
		this.diagramConnector4 = new DevExpress.XtraDiagram.DiagramConnector();
		this.diagramConnector5 = new DevExpress.XtraDiagram.DiagramConnector();
		this.diagramConnector6 = new DevExpress.XtraDiagram.DiagramConnector();
		this.diagramConnector7 = new DevExpress.XtraDiagram.DiagramConnector();
		this.diagramImage1 = new Apzon.Forms.Widget.DiagramImageEx();
		this.diagramImage2 = new Apzon.Forms.Widget.DiagramImageEx();
		this.diagramImage3 = new Apzon.Forms.Widget.DiagramImageEx();
		this.diagramImage4 = new Apzon.Forms.Widget.DiagramImageEx();
		this.diagramImage5 = new Apzon.Forms.Widget.DiagramImageEx();
		this.diagramImage6 = new Apzon.Forms.Widget.DiagramImageEx();
		this.diagramImage7 = new Apzon.Forms.Widget.DiagramImageEx();
		this.diagramShape8 = new DevExpress.XtraDiagram.DiagramShape();
		this.diagramShape9 = new DevExpress.XtraDiagram.DiagramShape();
		this.diagramShape10 = new DevExpress.XtraDiagram.DiagramShape();
		this.diagramShape11 = new DevExpress.XtraDiagram.DiagramShape();
		this.diagramShape12 = new DevExpress.XtraDiagram.DiagramShape();
		this.diagramShape13 = new DevExpress.XtraDiagram.DiagramShape();
		this.diagramShape14 = new DevExpress.XtraDiagram.DiagramShape();
		this.diagramImage8 = new Apzon.Forms.Widget.DiagramImageEx();
		this.diagramShape15 = new DevExpress.XtraDiagram.DiagramShape();
		((System.ComponentModel.ISupportInitialize)this.diagramControl1).BeginInit();
		base.SuspendLayout();
		this.diagramControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.diagramControl1.Items.AddRange(this.diagramShape1, this.diagramShape2, this.diagramShape3, this.diagramShape4, this.diagramShape5, this.diagramShape6, this.diagramShape7, this.diagramConnector1, this.diagramConnector2, this.diagramConnector3, this.diagramConnector4, this.diagramConnector5, this.diagramConnector6, this.diagramConnector7, this.diagramImage1, this.diagramImage2, this.diagramImage3, this.diagramImage4, this.diagramImage5, this.diagramImage6, this.diagramImage7, this.diagramShape8, this.diagramShape9, this.diagramShape10, this.diagramShape11, this.diagramShape12, this.diagramShape13, this.diagramShape14, this.diagramImage8, this.diagramShape15);
		this.diagramControl1.Location = new System.Drawing.Point(0, 0);
		this.diagramControl1.Margin = new System.Windows.Forms.Padding(0);
		this.diagramControl1.Name = "diagramControl1";
		this.diagramControl1.OptionsBehavior.ResizingMode = DevExpress.Diagram.Core.ResizingMode.Preview;
		this.diagramControl1.OptionsBehavior.SelectedStencils = new DevExpress.Diagram.Core.StencilCollection("BasicShapes", "BasicFlowchartShapes", "ArrowShapes", "SDLDiagramShapes", "SoftwareIcons", "DecorativeShapes");
		this.diagramControl1.OptionsView.AllowShapeShadows = true;
		this.diagramControl1.OptionsView.CanvasSizeMode = DevExpress.Diagram.Core.CanvasSizeMode.Fill;
		this.diagramControl1.OptionsView.PageMargin = new System.Windows.Forms.Padding(0);
		this.diagramControl1.OptionsView.PaperKind = System.Drawing.Printing.PaperKind.Letter;
		this.diagramControl1.OptionsView.ScrollMargin = new System.Windows.Forms.Padding(2000000000);
		this.diagramControl1.OptionsView.ShowGrid = false;
		this.diagramControl1.OptionsView.ShowPageBreaks = false;
		this.diagramControl1.OptionsView.ShowRulers = false;
		this.diagramControl1.Size = new System.Drawing.Size(1340, 763);
		this.diagramControl1.TabIndex = 0;
		this.diagramControl1.Text = "diagramControl1";
		this.diagramControl1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.diagramControl1_MouseClick);
		this.diagramControl1.MouseHover += new System.EventHandler(this.diagramControl1_MouseHover);
		this.diagramControl1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.diagramControl1_MouseMove);
		this.diagramShape1.AffectedByLayoutAlgorithms = false;
		this.diagramShape1.CanCopy = false;
		this.diagramShape1.CanCopyWithoutParent = false;
		this.diagramShape1.CanDelete = false;
		this.diagramShape1.CanDeleteWithoutParent = false;
		this.diagramShape1.CanMove = false;
		this.diagramShape1.CanResize = false;
		this.diagramShape1.CanRotate = false;
		this.diagramShape1.Position = new DevExpress.Utils.PointFloat(30f, 20f);
		this.diagramShape1.Shape = DevExpress.Diagram.Core.BasicFlowchartShapes.OnPageReference;
		this.diagramShape1.Size = new System.Drawing.SizeF(100f, 100f);
		this.diagramShape1.ThemeStyleId = DevExpress.Diagram.Core.DiagramShapeStyleId.Moderate6;
		this.diagramShape2.AffectedByLayoutAlgorithms = false;
		this.diagramShape2.CanCopy = false;
		this.diagramShape2.CanCopyWithoutParent = false;
		this.diagramShape2.CanDelete = false;
		this.diagramShape2.CanDeleteWithoutParent = false;
		this.diagramShape2.CanMove = false;
		this.diagramShape2.CanResize = false;
		this.diagramShape2.CanRotate = false;
		this.diagramShape2.Position = new DevExpress.Utils.PointFloat(240f, 170f);
		this.diagramShape2.Shape = DevExpress.Diagram.Core.BasicFlowchartShapes.OnPageReference;
		this.diagramShape2.Size = new System.Drawing.SizeF(100f, 100f);
		this.diagramShape2.ThemeStyleId = DevExpress.Diagram.Core.DiagramShapeStyleId.Moderate5;
		this.diagramShape3.AffectedByLayoutAlgorithms = false;
		this.diagramShape3.CanCopy = false;
		this.diagramShape3.CanCopyWithoutParent = false;
		this.diagramShape3.CanDelete = false;
		this.diagramShape3.CanDeleteWithoutParent = false;
		this.diagramShape3.CanMove = false;
		this.diagramShape3.CanResize = false;
		this.diagramShape3.CanRotate = false;
		this.diagramShape3.Position = new DevExpress.Utils.PointFloat(30f, 340f);
		this.diagramShape3.Shape = DevExpress.Diagram.Core.BasicFlowchartShapes.OnPageReference;
		this.diagramShape3.Size = new System.Drawing.SizeF(100f, 100f);
		this.diagramShape4.AffectedByLayoutAlgorithms = false;
		this.diagramShape4.CanCopy = false;
		this.diagramShape4.CanCopyWithoutParent = false;
		this.diagramShape4.CanDelete = false;
		this.diagramShape4.CanDeleteWithoutParent = false;
		this.diagramShape4.CanMove = false;
		this.diagramShape4.CanResize = false;
		this.diagramShape4.CanRotate = false;
		this.diagramShape4.Position = new DevExpress.Utils.PointFloat(460f, 340f);
		this.diagramShape4.Shape = DevExpress.Diagram.Core.BasicFlowchartShapes.OnPageReference;
		this.diagramShape4.Size = new System.Drawing.SizeF(100f, 100f);
		this.diagramShape4.ThemeStyleId = DevExpress.Diagram.Core.DiagramShapeStyleId.Moderate7;
		this.diagramShape5.AffectedByLayoutAlgorithms = false;
		this.diagramShape5.CanCopy = false;
		this.diagramShape5.CanCopyWithoutParent = false;
		this.diagramShape5.CanDelete = false;
		this.diagramShape5.CanDeleteWithoutParent = false;
		this.diagramShape5.CanMove = false;
		this.diagramShape5.CanResize = false;
		this.diagramShape5.CanRotate = false;
		this.diagramShape5.Position = new DevExpress.Utils.PointFloat(769.1363f, 334.5681f);
		this.diagramShape5.Shape = DevExpress.Diagram.Core.BasicFlowchartShapes.OnPageReference;
		this.diagramShape5.Size = new System.Drawing.SizeF(100f, 100f);
		this.diagramShape5.ThemeStyleId = DevExpress.Diagram.Core.DiagramShapeStyleId.Intense5;
		this.diagramShape6.AffectedByLayoutAlgorithms = false;
		this.diagramShape6.CanCopy = false;
		this.diagramShape6.CanCopyWithoutParent = false;
		this.diagramShape6.CanDelete = false;
		this.diagramShape6.CanDeleteWithoutParent = false;
		this.diagramShape6.CanMove = false;
		this.diagramShape6.CanResize = false;
		this.diagramShape6.CanRotate = false;
		this.diagramShape6.Position = new DevExpress.Utils.PointFloat(460f, 600f);
		this.diagramShape6.Shape = DevExpress.Diagram.Core.BasicFlowchartShapes.OnPageReference;
		this.diagramShape6.Size = new System.Drawing.SizeF(100f, 100f);
		this.diagramShape6.ThemeStyleId = DevExpress.Diagram.Core.DiagramShapeStyleId.Balanced7;
		this.diagramShape7.AffectedByLayoutAlgorithms = false;
		this.diagramShape7.CanCopy = false;
		this.diagramShape7.CanCopyWithoutParent = false;
		this.diagramShape7.CanDelete = false;
		this.diagramShape7.CanDeleteWithoutParent = false;
		this.diagramShape7.CanMove = false;
		this.diagramShape7.CanResize = false;
		this.diagramShape7.CanRotate = false;
		this.diagramShape7.Position = new DevExpress.Utils.PointFloat(769.1363f, 594.5681f);
		this.diagramShape7.Shape = DevExpress.Diagram.Core.BasicFlowchartShapes.OnPageReference;
		this.diagramShape7.Size = new System.Drawing.SizeF(100f, 100f);
		this.diagramShape7.ThemeStyleId = DevExpress.Diagram.Core.DiagramShapeStyleId.Balanced5;
		this.diagramConnector1.AffectedByLayoutAlgorithms = false;
		this.diagramConnector1.Appearance.ContentBackground = System.Drawing.Color.White;
		this.diagramConnector1.BeginItem = this.diagramShape1;
		this.diagramConnector1.BeginItemPointIndex = 2;
		this.diagramConnector1.BeginPoint = new DevExpress.Utils.PointFloat(80f, 120f);
		this.diagramConnector1.CanCopy = false;
		this.diagramConnector1.CanCopyWithoutParent = false;
		this.diagramConnector1.CanDelete = false;
		this.diagramConnector1.CanDeleteWithoutParent = false;
		this.diagramConnector1.CanMove = false;
		this.diagramConnector1.CanSelect = false;
		this.diagramConnector1.EndItem = this.diagramShape3;
		this.diagramConnector1.EndPoint = new DevExpress.Utils.PointFloat(80f, 340f);
		this.diagramConnector1.Points = new DevExpress.XtraDiagram.PointCollection(new DevExpress.Utils.PointFloat[0]);
		this.diagramConnector1.Type = DevExpress.Diagram.Core.ConnectorType.Straight;
		this.diagramConnector2.AffectedByLayoutAlgorithms = false;
		this.diagramConnector2.Appearance.ContentBackground = System.Drawing.Color.White;
		this.diagramConnector2.BeginItem = this.diagramShape3;
		this.diagramConnector2.BeginItemPointIndex = 1;
		this.diagramConnector2.BeginPoint = new DevExpress.Utils.PointFloat(130f, 390f);
		this.diagramConnector2.CanCopy = false;
		this.diagramConnector2.CanCopyWithoutParent = false;
		this.diagramConnector2.CanDelete = false;
		this.diagramConnector2.CanDeleteWithoutParent = false;
		this.diagramConnector2.CanMove = false;
		this.diagramConnector2.CanSelect = false;
		this.diagramConnector2.EndItem = this.diagramShape2;
		this.diagramConnector2.EndItemPointIndex = 2;
		this.diagramConnector2.EndPoint = new DevExpress.Utils.PointFloat(290f, 270f);
		this.diagramConnector2.Points = new DevExpress.XtraDiagram.PointCollection(new DevExpress.Utils.PointFloat[0]);
		this.diagramConnector2.Type = DevExpress.Diagram.Core.ConnectorType.Straight;
		this.diagramConnector3.AffectedByLayoutAlgorithms = false;
		this.diagramConnector3.Appearance.ContentBackground = System.Drawing.Color.White;
		this.diagramConnector3.BeginItem = this.diagramShape4;
		this.diagramConnector3.BeginItemPointIndex = 1;
		this.diagramConnector3.BeginPoint = new DevExpress.Utils.PointFloat(560f, 390f);
		this.diagramConnector3.CanCopy = false;
		this.diagramConnector3.CanCopyWithoutParent = false;
		this.diagramConnector3.CanDelete = false;
		this.diagramConnector3.CanDeleteWithoutParent = false;
		this.diagramConnector3.CanMove = false;
		this.diagramConnector3.CanSelect = false;
		this.diagramConnector3.EndItem = this.diagramShape5;
		this.diagramConnector3.EndPoint = new DevExpress.Utils.PointFloat(769.1473f, 385.616f);
		this.diagramConnector3.Points = new DevExpress.XtraDiagram.PointCollection(new DevExpress.Utils.PointFloat[0]);
		this.diagramConnector3.Type = DevExpress.Diagram.Core.ConnectorType.Straight;
		this.diagramConnector4.AffectedByLayoutAlgorithms = false;
		this.diagramConnector4.Appearance.ContentBackground = System.Drawing.Color.White;
		this.diagramConnector4.BeginItem = this.diagramShape4;
		this.diagramConnector4.BeginItemPointIndex = 2;
		this.diagramConnector4.BeginPoint = new DevExpress.Utils.PointFloat(510f, 440f);
		this.diagramConnector4.CanCopy = false;
		this.diagramConnector4.CanCopyWithoutParent = false;
		this.diagramConnector4.CanDelete = false;
		this.diagramConnector4.CanDeleteWithoutParent = false;
		this.diagramConnector4.CanMove = false;
		this.diagramConnector4.CanSelect = false;
		this.diagramConnector4.EndItem = this.diagramShape6;
		this.diagramConnector4.EndPoint = new DevExpress.Utils.PointFloat(510f, 600f);
		this.diagramConnector4.Points = new DevExpress.XtraDiagram.PointCollection(new DevExpress.Utils.PointFloat[0]);
		this.diagramConnector4.Type = DevExpress.Diagram.Core.ConnectorType.Straight;
		this.diagramConnector5.AffectedByLayoutAlgorithms = false;
		this.diagramConnector5.Appearance.ContentBackground = System.Drawing.Color.White;
		this.diagramConnector5.BeginItem = this.diagramShape5;
		this.diagramConnector5.BeginItemPointIndex = 2;
		this.diagramConnector5.BeginPoint = new DevExpress.Utils.PointFloat(819.1363f, 434.5681f);
		this.diagramConnector5.CanCopy = false;
		this.diagramConnector5.CanCopyWithoutParent = false;
		this.diagramConnector5.CanDelete = false;
		this.diagramConnector5.CanDeleteWithoutParent = false;
		this.diagramConnector5.CanMove = false;
		this.diagramConnector5.CanSelect = false;
		this.diagramConnector5.EndItem = this.diagramShape7;
		this.diagramConnector5.EndItemPointIndex = 0;
		this.diagramConnector5.EndPoint = new DevExpress.Utils.PointFloat(819.1363f, 594.5681f);
		this.diagramConnector5.Points = new DevExpress.XtraDiagram.PointCollection(new DevExpress.Utils.PointFloat[0]);
		this.diagramConnector5.Type = DevExpress.Diagram.Core.ConnectorType.Straight;
		this.diagramConnector6.AffectedByLayoutAlgorithms = false;
		this.diagramConnector6.Appearance.ContentBackground = System.Drawing.Color.White;
		this.diagramConnector6.BeginItem = this.diagramShape2;
		this.diagramConnector6.BeginItemPointIndex = 2;
		this.diagramConnector6.BeginPoint = new DevExpress.Utils.PointFloat(290f, 270f);
		this.diagramConnector6.CanCopy = false;
		this.diagramConnector6.CanCopyWithoutParent = false;
		this.diagramConnector6.CanDelete = false;
		this.diagramConnector6.CanDeleteWithoutParent = false;
		this.diagramConnector6.CanMove = false;
		this.diagramConnector6.CanSelect = false;
		this.diagramConnector6.EndItem = this.diagramShape4;
		this.diagramConnector6.EndItemPointIndex = 3;
		this.diagramConnector6.EndPoint = new DevExpress.Utils.PointFloat(460f, 390f);
		this.diagramConnector6.Points = new DevExpress.XtraDiagram.PointCollection(new DevExpress.Utils.PointFloat[0]);
		this.diagramConnector6.Type = DevExpress.Diagram.Core.ConnectorType.Straight;
		this.diagramConnector7.AffectedByLayoutAlgorithms = false;
		this.diagramConnector7.Appearance.ContentBackground = System.Drawing.Color.White;
		this.diagramConnector7.BeginItem = this.diagramShape3;
		this.diagramConnector7.BeginItemPointIndex = 1;
		this.diagramConnector7.BeginPoint = new DevExpress.Utils.PointFloat(130f, 390f);
		this.diagramConnector7.CanCopy = false;
		this.diagramConnector7.CanCopyWithoutParent = false;
		this.diagramConnector7.CanDelete = false;
		this.diagramConnector7.CanDeleteWithoutParent = false;
		this.diagramConnector7.CanMove = false;
		this.diagramConnector7.CanSelect = false;
		this.diagramConnector7.EndItem = this.diagramShape4;
		this.diagramConnector7.EndItemPointIndex = 3;
		this.diagramConnector7.EndPoint = new DevExpress.Utils.PointFloat(460f, 390f);
		this.diagramConnector7.Points = new DevExpress.XtraDiagram.PointCollection(new DevExpress.Utils.PointFloat[0]);
		this.diagramConnector7.Type = DevExpress.Diagram.Core.ConnectorType.Straight;
		this.diagramImage1.AffectedByLayoutAlgorithms = false;
		this.diagramImage1.CanCopy = false;
		this.diagramImage1.CanCopyWithoutParent = false;
		this.diagramImage1.CanDelete = false;
		this.diagramImage1.CanDeleteWithoutParent = false;
		this.diagramImage1.CanMove = false;
		this.diagramImage1.CanSelect = false;
		this.diagramImage1.Image = resources.GetObject("diagramImage1.Image");
		this.diagramImage1.ObjectGroup = null;
		this.diagramImage1.ObjType = Apzon.Commons.ApzObjectTypes.oQuotations;
		this.diagramImage1.Position = new DevExpress.Utils.PointFloat(55.43185f, 45.43185f);
		this.diagramImage1.Size = new System.Drawing.SizeF(49.13629f, 49.13629f);
		this.diagramImage1.StretchMode = DevExpress.Diagram.Core.StretchMode.Stretch;
		this.diagramImage2.AffectedByLayoutAlgorithms = false;
		this.diagramImage2.CanCopy = false;
		this.diagramImage2.CanCopyWithoutParent = false;
		this.diagramImage2.CanDelete = false;
		this.diagramImage2.CanDeleteWithoutParent = false;
		this.diagramImage2.CanMove = false;
		this.diagramImage2.CanSelect = false;
		this.diagramImage2.Image = resources.GetObject("diagramImage2.Image");
		this.diagramImage2.ObjectGroup = null;
		this.diagramImage2.ObjType = Apzon.Commons.ApzObjectTypes.oOrders;
		this.diagramImage2.Position = new DevExpress.Utils.PointFloat(55.43185f, 365.4319f);
		this.diagramImage2.Size = new System.Drawing.SizeF(49.13629f, 49.13629f);
		this.diagramImage2.StretchMode = DevExpress.Diagram.Core.StretchMode.Stretch;
		this.diagramImage3.AffectedByLayoutAlgorithms = false;
		this.diagramImage3.CanCopy = false;
		this.diagramImage3.CanCopyWithoutParent = false;
		this.diagramImage3.CanDelete = false;
		this.diagramImage3.CanDeleteWithoutParent = false;
		this.diagramImage3.CanMove = false;
		this.diagramImage3.CanSelect = false;
		this.diagramImage3.Image = resources.GetObject("diagramImage3.Image");
		this.diagramImage3.ObjectGroup = null;
		this.diagramImage3.ObjType = Apzon.Commons.ApzObjectTypes.oReserveInvoices;
		this.diagramImage3.Position = new DevExpress.Utils.PointFloat(265.4319f, 195.4319f);
		this.diagramImage3.Size = new System.Drawing.SizeF(49.13629f, 49.13629f);
		this.diagramImage3.StretchMode = DevExpress.Diagram.Core.StretchMode.Stretch;
		this.diagramImage4.AffectedByLayoutAlgorithms = false;
		this.diagramImage4.CanCopy = false;
		this.diagramImage4.CanCopyWithoutParent = false;
		this.diagramImage4.CanDelete = false;
		this.diagramImage4.CanDeleteWithoutParent = false;
		this.diagramImage4.CanMove = false;
		this.diagramImage4.CanSelect = false;
		this.diagramImage4.Image = resources.GetObject("diagramImage4.Image");
		this.diagramImage4.ObjectGroup = null;
		this.diagramImage4.ObjType = Apzon.Commons.ApzObjectTypes.oDeliveryNotes;
		this.diagramImage4.Position = new DevExpress.Utils.PointFloat(485.4319f, 365.4319f);
		this.diagramImage4.Size = new System.Drawing.SizeF(49.13629f, 49.13629f);
		this.diagramImage4.StretchMode = DevExpress.Diagram.Core.StretchMode.Stretch;
		this.diagramImage5.AffectedByLayoutAlgorithms = false;
		this.diagramImage5.CanCopy = false;
		this.diagramImage5.CanCopyWithoutParent = false;
		this.diagramImage5.CanDelete = false;
		this.diagramImage5.CanDeleteWithoutParent = false;
		this.diagramImage5.CanMove = false;
		this.diagramImage5.CanSelect = false;
		this.diagramImage5.Image = resources.GetObject("diagramImage5.Image");
		this.diagramImage5.ObjectGroup = null;
		this.diagramImage5.ObjType = Apzon.Commons.ApzObjectTypes.oInvoices;
		this.diagramImage5.Position = new DevExpress.Utils.PointFloat(800f, 360f);
		this.diagramImage5.Size = new System.Drawing.SizeF(49.13629f, 49.13629f);
		this.diagramImage5.StretchMode = DevExpress.Diagram.Core.StretchMode.Stretch;
		this.diagramImage6.AffectedByLayoutAlgorithms = false;
		this.diagramImage6.CanCopy = false;
		this.diagramImage6.CanCopyWithoutParent = false;
		this.diagramImage6.CanDelete = false;
		this.diagramImage6.CanDeleteWithoutParent = false;
		this.diagramImage6.CanMove = false;
		this.diagramImage6.CanSelect = false;
		this.diagramImage6.FlipMode = DevExpress.Diagram.Core.ImageFlipMode.Horizontal;
		this.diagramImage6.Image = resources.GetObject("diagramImage6.Image");
		this.diagramImage6.ObjectGroup = null;
		this.diagramImage6.ObjType = Apzon.Commons.ApzObjectTypes.oReturns;
		this.diagramImage6.Position = new DevExpress.Utils.PointFloat(485.4319f, 625.4319f);
		this.diagramImage6.Size = new System.Drawing.SizeF(49.13629f, 49.13629f);
		this.diagramImage6.StretchMode = DevExpress.Diagram.Core.StretchMode.Stretch;
		this.diagramImage7.AffectedByLayoutAlgorithms = false;
		this.diagramImage7.CanCopy = false;
		this.diagramImage7.CanCopyWithoutParent = false;
		this.diagramImage7.CanDelete = false;
		this.diagramImage7.CanDeleteWithoutParent = false;
		this.diagramImage7.CanMove = false;
		this.diagramImage7.CanSelect = false;
		this.diagramImage7.Image = resources.GetObject("diagramImage7.Image");
		this.diagramImage7.ObjType = Apzon.Commons.ApzObjectTypes.oCreditNotes;
		this.diagramImage7.Position = new DevExpress.Utils.PointFloat(794.5682f, 620f);
		this.diagramImage7.Size = new System.Drawing.SizeF(49.13629f, 49.13629f);
		this.diagramImage7.StretchMode = DevExpress.Diagram.Core.StretchMode.Stretch;
		this.diagramShape8.AffectedByLayoutAlgorithms = false;
		this.diagramShape8.Appearance.Font = new System.Drawing.Font("Segoe UI", 12f, System.Drawing.FontStyle.Bold);
		this.diagramShape8.CanCopy = false;
		this.diagramShape8.CanCopyWithoutParent = false;
		this.diagramShape8.CanDelete = false;
		this.diagramShape8.CanDeleteWithoutParent = false;
		this.diagramShape8.CanMove = false;
		this.diagramShape8.CanSelect = false;
		this.diagramShape8.Content = "Quotations";
		this.diagramShape8.Position = new DevExpress.Utils.PointFloat(125f, 82.5f);
		this.diagramShape8.Shape = DevExpress.Diagram.Core.BasicFlowchartShapes.StartEnd;
		this.diagramShape8.Size = new System.Drawing.SizeF(100f, 37.5f);
		this.diagramShape9.AffectedByLayoutAlgorithms = false;
		this.diagramShape9.Appearance.Font = new System.Drawing.Font("Segoe UI", 12f, System.Drawing.FontStyle.Bold);
		this.diagramShape9.CanCopy = false;
		this.diagramShape9.CanCopyWithoutParent = false;
		this.diagramShape9.CanDelete = false;
		this.diagramShape9.CanDeleteWithoutParent = false;
		this.diagramShape9.CanMove = false;
		this.diagramShape9.CanSelect = false;
		this.diagramShape9.Content = "Orders";
		this.diagramShape9.Position = new DevExpress.Utils.PointFloat(125f, 402.5f);
		this.diagramShape9.Shape = DevExpress.Diagram.Core.BasicFlowchartShapes.StartEnd;
		this.diagramShape9.Size = new System.Drawing.SizeF(100f, 37.5f);
		this.diagramShape10.AffectedByLayoutAlgorithms = false;
		this.diagramShape10.Appearance.Font = new System.Drawing.Font("Segoe UI", 12f, System.Drawing.FontStyle.Bold);
		this.diagramShape10.CanCopy = false;
		this.diagramShape10.CanCopyWithoutParent = false;
		this.diagramShape10.CanDelete = false;
		this.diagramShape10.CanDeleteWithoutParent = false;
		this.diagramShape10.CanMove = false;
		this.diagramShape10.CanSelect = false;
		this.diagramShape10.Content = "Reserve Invoices";
		this.diagramShape10.Position = new DevExpress.Utils.PointFloat(350f, 232.5f);
		this.diagramShape10.Shape = DevExpress.Diagram.Core.BasicFlowchartShapes.StartEnd;
		this.diagramShape10.Size = new System.Drawing.SizeF(150f, 37.5f);
		this.diagramShape11.AffectedByLayoutAlgorithms = false;
		this.diagramShape11.Appearance.Font = new System.Drawing.Font("Segoe UI", 12f, System.Drawing.FontStyle.Bold);
		this.diagramShape11.CanCopy = false;
		this.diagramShape11.CanCopyWithoutParent = false;
		this.diagramShape11.CanDelete = false;
		this.diagramShape11.CanDeleteWithoutParent = false;
		this.diagramShape11.CanMove = false;
		this.diagramShape11.CanSelect = false;
		this.diagramShape11.Content = "Deliveries";
		this.diagramShape11.Position = new DevExpress.Utils.PointFloat(575f, 402.5f);
		this.diagramShape11.Shape = DevExpress.Diagram.Core.BasicFlowchartShapes.StartEnd;
		this.diagramShape11.Size = new System.Drawing.SizeF(100f, 37.5f);
		this.diagramShape12.AffectedByLayoutAlgorithms = false;
		this.diagramShape12.Appearance.Font = new System.Drawing.Font("Segoe UI", 12f, System.Drawing.FontStyle.Bold);
		this.diagramShape12.CanCopy = false;
		this.diagramShape12.CanCopyWithoutParent = false;
		this.diagramShape12.CanDelete = false;
		this.diagramShape12.CanDeleteWithoutParent = false;
		this.diagramShape12.CanMove = false;
		this.diagramShape12.CanSelect = false;
		this.diagramShape12.Content = "Returns";
		this.diagramShape12.Position = new DevExpress.Utils.PointFloat(575f, 657.0681f);
		this.diagramShape12.Shape = DevExpress.Diagram.Core.BasicFlowchartShapes.StartEnd;
		this.diagramShape12.Size = new System.Drawing.SizeF(100f, 37.5f);
		this.diagramShape13.AffectedByLayoutAlgorithms = false;
		this.diagramShape13.Appearance.Font = new System.Drawing.Font("Segoe UI", 12f, System.Drawing.FontStyle.Bold);
		this.diagramShape13.CanCopy = false;
		this.diagramShape13.CanCopyWithoutParent = false;
		this.diagramShape13.CanDelete = false;
		this.diagramShape13.CanDeleteWithoutParent = false;
		this.diagramShape13.CanMove = false;
		this.diagramShape13.CanSelect = false;
		this.diagramShape13.Content = "Invoices";
		this.diagramShape13.Position = new DevExpress.Utils.PointFloat(875f, 402.5f);
		this.diagramShape13.Shape = DevExpress.Diagram.Core.BasicFlowchartShapes.StartEnd;
		this.diagramShape13.Size = new System.Drawing.SizeF(100f, 37.5f);
		this.diagramShape14.AffectedByLayoutAlgorithms = false;
		this.diagramShape14.Appearance.Font = new System.Drawing.Font("Segoe UI", 12f, System.Drawing.FontStyle.Bold);
		this.diagramShape14.CanCopy = false;
		this.diagramShape14.CanCopyWithoutParent = false;
		this.diagramShape14.CanDelete = false;
		this.diagramShape14.CanDeleteWithoutParent = false;
		this.diagramShape14.CanMove = false;
		this.diagramShape14.CanSelect = false;
		this.diagramShape14.Content = "Credit Memos";
		this.diagramShape14.Position = new DevExpress.Utils.PointFloat(875f, 657.0681f);
		this.diagramShape14.Shape = DevExpress.Diagram.Core.BasicFlowchartShapes.StartEnd;
		this.diagramShape14.Size = new System.Drawing.SizeF(125f, 37.5f);
		this.diagramImage8.AffectedByLayoutAlgorithms = false;
		this.diagramImage8.CanCopy = false;
		this.diagramImage8.CanCopyWithoutParent = false;
		this.diagramImage8.CanDelete = false;
		this.diagramImage8.CanDeleteWithoutParent = false;
		this.diagramImage8.CanMove = false;
		this.diagramImage8.CanResize = false;
		this.diagramImage8.CanRotate = false;
		this.diagramImage8.CanSelect = false;
		this.diagramImage8.Image = resources.GetObject("diagramImage8.Image");
		this.diagramImage8.ObjectGroup = "SalesReport";
		this.diagramImage8.ObjType = Apzon.Commons.ApzObjectTypes.oNone;
		this.diagramImage8.Position = new DevExpress.Utils.PointFloat(769.1363f, 20f);
		this.diagramImage8.Size = new System.Drawing.SizeF(100f, 100f);
		this.diagramImage8.StretchMode = DevExpress.Diagram.Core.StretchMode.Stretch;
		this.diagramShape15.AffectedByLayoutAlgorithms = false;
		this.diagramShape15.Appearance.Font = new System.Drawing.Font("Segoe UI", 12f, System.Drawing.FontStyle.Bold);
		this.diagramShape15.CanCopy = false;
		this.diagramShape15.CanCopyWithoutParent = false;
		this.diagramShape15.CanDelete = false;
		this.diagramShape15.CanDeleteWithoutParent = false;
		this.diagramShape15.CanMove = false;
		this.diagramShape15.CanSelect = false;
		this.diagramShape15.Content = "Report";
		this.diagramShape15.Position = new DevExpress.Utils.PointFloat(875f, 82.5f);
		this.diagramShape15.Shape = DevExpress.Diagram.Core.BasicFlowchartShapes.StartEnd;
		this.diagramShape15.Size = new System.Drawing.SizeF(100f, 37.5f);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.diagramControl1);
		base.Name = "ucSalesProcessModel";
		base.Size = new System.Drawing.Size(1340, 763);
		base.Load += new System.EventHandler(this.Form1_Load);
		base.Resize += new System.EventHandler(this.Form1_Resize);
		((System.ComponentModel.ISupportInitialize)this.diagramControl1).EndInit();
		base.ResumeLayout(false);
	}
}
