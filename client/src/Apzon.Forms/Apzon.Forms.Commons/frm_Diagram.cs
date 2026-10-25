using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Form;
using Apzon.Forms.Helper;
using Apzon.HttpClientHelper;
using DevExpress.Diagram.Core;
using DevExpress.Utils;
using DevExpress.XtraDiagram;
using DevExpress.XtraEditors;

namespace Apzon.Forms.Commons;

public class frm_Diagram : XtraFormEx
{
	public List<DiagramConnectorEx> Connectors = new List<DiagramConnectorEx>();

	public DiagramShapeEx Root;

	public List<DiagramShapeEx> ShapeExs = new List<DiagramShapeEx>();

	public Dictionary<string, PointFloat> ObjectLastPoint = new Dictionary<string, PointFloat>();

	private int _layout;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private IContainer components;

	private DiagramControl dig_Main;

	private SimpleButtonEx btn_OK;

	private MarqueeProgressBarControl marquee_Wait;

	private LabelControl lb_Wait;

	private SimpleButtonEx btnChangeLayout;

	public ApzObjectTypes ObjectType { get; set; }

	public string ObjectId { get; set; }

	public frm_Diagram()
	{
		InitializeComponent();
	}

	private void btn_OK_Click(object sender, EventArgs e)
	{
		Close();
	}

	private async void frm_Diagram_Load(object sender, EventArgs e)
	{
		lb_Wait.Visible = true;
		marquee_Wait.Visible = true;
		try
		{
			DataTable dataTable = new DataTable();
			if (!string.IsNullOrEmpty(ObjectId))
			{
				dataTable = await _httpClient.Diagram.GetDiagram(ObjectType, ObjectId);
			}
			if (dataTable.IsNotNull())
			{
				Text = Text + " " + GetObjectString(((int)ObjectType).ToString(), ObjectType) + " - " + ObjectId;
				if (ObjectType == ApzObjectTypes.oProductTrees)
				{
					for (int i = 0; i < dataTable.Rows.Count; i++)
					{
						InitDiagramProductTree(dataTable.Rows[i], dataTable, null);
					}
					dig_Main.ApplyTipOverTreeLayout();
					_layout = 4;
				}
				else
				{
					for (int j = 0; j < dataTable.Rows.Count; j++)
					{
						InitDiagram(dataTable.Rows[j], dataTable, null);
					}
					if (ObjectType != ApzObjectTypes.oProductionOrders)
					{
						dig_Main.ApplyMindMapTreeLayout();
						_layout = 1;
					}
					else
					{
						dig_Main.ApplySugiyamaLayout();
						_layout = 3;
					}
				}
				btnChangeLayout.Enabled = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		lb_Wait.Visible = false;
		marquee_Wait.Visible = false;
	}

	private void InitDiagramProductTree(DataRow root, DataTable data, DiagramShapeEx baseShape)
	{
		string code = Function.ToString(root["Code"]);
		DiagramShapeEx diagramShapeEx = ShapeExs.FirstOrDefault((DiagramShapeEx t) => t.ObjectNum == code);
		if (diagramShapeEx == null)
		{
			diagramShapeEx = new DiagramShapeEx(4.ToString(), Function.ToString(root["Code"]), Function.ToString(root["Name"]), Function.ToString(root["Code"]) == ObjectId);
			DiagramShapeEx diagramShapeEx2 = diagramShapeEx;
			PointFloat position;
			if (baseShape != null)
			{
				position = new PointFloat(baseShape.X - 10f - baseShape.Width, baseShape.Position.Y + baseShape.Height + 50f);
			}
			else
			{
				position = ((Root == null) ? new PointFloat(100f, 200f) : new PointFloat(Root.X + 50f + Root.Width, Root.Y));
			}
			diagramShapeEx2.Position = position;
			dig_Main.Items.Add(diagramShapeEx);
			ShapeExs.Add(diagramShapeEx);
			if (baseShape == null)
			{
				Root = diagramShapeEx;
			}
		}
		List<DataRow> list = (from t in data.AsEnumerable()
			where Function.ToString(t["Father"]) == code
			select t).ToList();
		int num = 0;
		foreach (DataRow dr in list)
		{
			DiagramShapeEx diagramShapeEx3 = ShapeExs.FirstOrDefault((DiagramShapeEx t) => t.ObjectNum == Function.ToString(dr["Code"]));
			if (diagramShapeEx3 == null)
			{
				diagramShapeEx3 = new DiagramShapeEx(4.ToString(), Function.ToString(dr["Code"]), Function.ToString(dr["Name"]), Function.ToString(dr["Code"]) == ObjectId);
				diagramShapeEx3.Position = new PointFloat(diagramShapeEx.X + (float)num * diagramShapeEx.Width + (float)((num != 0) ? 100 : 0), diagramShapeEx.Position.Y + diagramShapeEx.Height + 50f);
				dig_Main.Items.Add(diagramShapeEx3);
				ShapeExs.Add(diagramShapeEx3);
			}
			if (Connectors.FirstOrDefault((DiagramConnectorEx t) => t.Begin == Function.ToString(root["Code"]) && t.End == Function.ToString(dr["Code"])) == null)
			{
				DiagramConnectorEx item = new DiagramConnectorEx(diagramShapeEx, diagramShapeEx3, 2, 0, ConnectorType.Curved);
				Connectors.Add(item);
				dig_Main.Items.Add(item);
			}
			InitDiagramProductTree(dr, data, diagramShapeEx3);
			num++;
		}
	}

	private void InitDiagram(DataRow root, DataTable data, DiagramShapeEx baseShape)
	{
		string id = Function.ToString(root["Id"]);
		DiagramShapeEx shape = ShapeExs.FirstOrDefault((DiagramShapeEx t) => t.Id == id);
		if (shape == null)
		{
			string key = Function.ToString(root["ObjType"]);
			shape = new DiagramShapeEx(Function.ToString(root["Id"]), Function.ToString(root["ObjType"]), Function.ToString(root["ObjectId"]), Function.ToString(root["ObjectNum"]), Function.ToString(root["Type"]), Function.ToString(root["ObjectCode"]), Function.ToString(root["Status"]), Function.ParseDateTime(root["DocDate"]) ?? new DateTime(1900, 1, 1), Function.ToString(root["FatherId"]), Function.ToString(root["RelatedTyp"]), Function.ParseInt(root["RelatedEnt"]), (Function.ParseInt(root["ObjType"]) == (int)ObjectType || (ObjectType == ApzObjectTypes.oReserveInvoices && Function.ToString(root["isIns"]) == "Y")) && Function.ToString(root["ObjectId"]) == ObjectId, Function.ParseDouble(root["DocTotal"]), Function.ToString(root["DocCur"]), Function.ToString(root["isIns"]), ObjectType);
			float num;
			if (ObjectLastPoint.ContainsKey(key))
			{
				num = ObjectLastPoint[key].X;
			}
			else
			{
				num = ((baseShape == null) ? 100f : (baseShape.Width + 250f));
			}
			float num2 = (ObjectLastPoint.ContainsKey(key) ? (ObjectLastPoint[key].Y + 90f) : (baseShape?.Position.Y ?? 200f));
			shape.Position = ((baseShape == null) ? new PointFloat(num, num2) : new PointFloat(num, num2));
			dig_Main.Items.Add(shape);
			ShapeExs.Add(shape);
			ObjectLastPoint[key] = shape.Position;
		}
		int num3 = 0;
		List<DataRow> list = (from t in data.AsEnumerable()
			where Function.ToString(t["FatherId"]) == id
			select t).ToList();
		foreach (DataRow dr in list)
		{
			DiagramShapeEx child = ShapeExs.FirstOrDefault((DiagramShapeEx t) => t.Id == Function.ToString(dr["Id"]));
			if (child == null)
			{
				string key2 = Function.ToString(dr["ObjType"]);
				child = new DiagramShapeEx(Function.ToString(dr["Id"]), Function.ToString(dr["ObjType"]), Function.ToString(dr["ObjectId"]), Function.ToString(dr["ObjectNum"]), Function.ToString(dr["Type"]), Function.ToString(dr["ObjectCode"]), Function.ToString(dr["Status"]), Function.ParseDateTime(dr["DocDate"]) ?? new DateTime(1900, 1, 1), Function.ToString(dr["FatherId"]), Function.ToString(dr["RelatedTyp"]), Function.ParseInt(dr["RelatedEnt"]), Function.ParseInt(dr["ObjType"]) == (int)ObjectType && Function.ToString(dr["ObjectId"]) == ObjectId, Function.ParseDouble(dr["DocTotal"]), Function.ToString(dr["DocCur"]), Function.ToString(dr["isIns"]), ObjectType);
				ShapeExs.Add(child);
				float num4 = (ObjectLastPoint.ContainsKey(key2) ? ObjectLastPoint[key2].X : (shape.Position.X + 250f + (float)((num3 <= list.Count / 2) ? (num3 * 10) : ((list.Count - num3) * 10))));
				float num5 = (ObjectLastPoint.ContainsKey(key2) ? (ObjectLastPoint[key2].Y + 90f) : (shape.Position.Y - (float)((list.Count > 1) ? 90 : 0) + (float)num3 * (child.Height + 5f)));
				child.Position = new PointFloat(num4, num5);
				dig_Main.Items.Add(child);
				InitDiagram(dr, data, child);
				ObjectLastPoint[key2] = child.Position;
			}
			if (Connectors.FirstOrDefault((DiagramConnectorEx t) => t.Begin == Function.ToString(shape.Id) && t.End == Function.ToString(child.Id)) == null)
			{
				child.Position = new PointFloat(shape.Position.X + 250f + (float)((num3 <= list.Count / 2) ? (num3 * 10) : ((list.Count - num3) * 10)), shape.Position.Y - (float)((list.Count > 1) ? 90 : 0) + (float)num3 * (child.Height + 5f));
				DiagramConnectorEx diagramConnectorEx = new DiagramConnectorEx(shape, child, 1, 3, ConnectorType.Straight);
				Connectors.Add(diagramConnectorEx);
				dig_Main.Items.AddRange(diagramConnectorEx);
			}
			DiagramShapeEx related = ShapeExs.FirstOrDefault((DiagramShapeEx t) => t.ObjectType == Function.ToString(dr["RelatedTyp"]) && t.ObjectId == Function.ToString(dr["RelatedEnt"]));
			if (related != null && Connectors.FirstOrDefault((DiagramConnectorEx t) => t.Begin == Function.ToString(related.Id) && t.End == Function.ToString(child.Id)) == null)
			{
				DiagramConnectorEx item = new DiagramConnectorEx(related, child, 1, 1, ConnectorType.RightAngle);
				dig_Main.Items.Add(item);
				Connectors.Add(item);
			}
			num3++;
		}
	}

	private void dig_Main_ShowingEditor(object sender, DiagramShowingEditorEventArgs e)
	{
		try
		{
			if (e.Item is DiagramShapeEx diagramShapeEx)
			{
				ApzObjectTypes apzObjectTypes = (ApzObjectTypes)Function.ParseInt(diagramShapeEx.ObjectType);
				string objectId = diagramShapeEx.ObjectId;
				bool reserve = diagramShapeEx.IsIns == "Y";
				this.DrillDown(apzObjectTypes, apzObjectTypes, objectId, reserve, ObjectType == ApzObjectTypes.oProductionOrders, objectId = diagramShapeEx.ObjectId);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		e.Cancel = true;
	}

	public static string GetObjectString(string type, ApzObjectTypes roottype, string isIns = "N")
	{
		switch ((ApzObjectTypes)Function.ParseInt(type))
		{
		case ApzObjectTypes.oProductionOrders:
			return "Production Order";
		case ApzObjectTypes.oInventoryGenEntry:
			if (roottype != ApzObjectTypes.oProductionOrders)
			{
				return "Goods Receipt";
			}
			return "Receipt from production";
		case ApzObjectTypes.oInventoryGenExit:
			if (roottype != ApzObjectTypes.oProductionOrders)
			{
				return "Goods Issue";
			}
			return "Issue for production";
		case ApzObjectTypes.oDeliveryNotes:
			return "Delivery";
		case ApzObjectTypes.oOrders:
			return "Sales Orders";
		case ApzObjectTypes.oQuotations:
			return "Sales Quotation";
		case ApzObjectTypes.oInvoices:
			if (!(isIns == "Y"))
			{
				return "A/R Invoice";
			}
			return "A/R Reserve Invoice";
		case ApzObjectTypes.oReserveInvoices:
			return "A/R Reserve Invoice";
		case ApzObjectTypes.oReturns:
			return "Return";
		case ApzObjectTypes.oCreditNotes:
			return "A/R Credit Memo";
		case ApzObjectTypes.oPurchaseRequest:
			return "Purchase Request";
		case ApzObjectTypes.oPurchaseQuotations:
			return "Purchase Quotation";
		case ApzObjectTypes.oPurchaseOrders:
			return "Purchase Order";
		case ApzObjectTypes.oPurchaseDeliveryNotes:
			return "Goods Receipt P/O";
		case ApzObjectTypes.oPurchaseInvoices:
			if (!(isIns == "Y"))
			{
				return "A/P Invoice";
			}
			return "A/P Reserve Invoice";
		case ApzObjectTypes.oAPReserveInvoice:
			return "A/P Reserve Invoice";
		case ApzObjectTypes.oGoodsReturnRequest:
			return "Goods Return Request";
		case ApzObjectTypes.oPurchaseReturns:
			return "Goods Return";
		case ApzObjectTypes.oPurchaseCreditNotes:
			return "A/P Credit Memo";
		case ApzObjectTypes.oProductTrees:
			return "Bill of material";
		case ApzObjectTypes.oProjectManager:
			return "Project manager";
		case ApzObjectTypes.oReportCompletion:
			return "Project report completion";
		case ApzObjectTypes.oProjectConfirm:
			return "Project confirm";
		case ApzObjectTypes.oPurchaseDownPayments:
			return "A/P Down Payment Request";
		case ApzObjectTypes.oPurchaseLandedCost:
			return "Landed Costs";
		case ApzObjectTypes.oInventoryTransferRequest:
			return "Transfer Request";
		case ApzObjectTypes.oStockTransfer:
			return "Inventory Transfer";
		case ApzObjectTypes.oReturnRequest:
			return "Return Request";
		default:
			return "";
		}
	}

	public static string GetTypeString(string type)
	{
		return type switch
		{
			"S" => "Standard", 
			"D" => "Disassembly", 
			"P" => "Special", 
			_ => "        ", 
		};
	}

	public static string GetStatusString(string status)
	{
		return status switch
		{
			"O" => "Open", 
			"L" => "Closed", 
			"C" => "Canceled", 
			"P" => "Planned", 
			"R" => "Released", 
			_ => "", 
		};
	}

	private void btnChangeLayout_Click(object sender, EventArgs e)
	{
		try
		{
			btnChangeLayout.Enabled = false;
			_layout++;
			if (_layout == 6)
			{
				_layout = 0;
			}
			switch (_layout)
			{
			case 0:
				dig_Main.ApplyCircularLayout();
				break;
			case 1:
				dig_Main.ApplyMindMapTreeLayout();
				break;
			case 2:
				dig_Main.ApplyOrgChartLayout();
				break;
			case 3:
				dig_Main.ApplySugiyamaLayout();
				break;
			case 4:
				dig_Main.ApplyTipOverTreeLayout();
				break;
			case 5:
				dig_Main.ApplyTreeLayout();
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex.ToString());
			WinCommons.Error(Enums.MessageCode.Error, "");
		}
		finally
		{
			btnChangeLayout.Enabled = true;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Commons.frm_Diagram));
		this.dig_Main = new DevExpress.XtraDiagram.DiagramControl();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.marquee_Wait = new DevExpress.XtraEditors.MarqueeProgressBarControl();
		this.lb_Wait = new DevExpress.XtraEditors.LabelControl();
		this.btnChangeLayout = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.dig_Main).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.marquee_Wait.Properties).BeginInit();
		base.SuspendLayout();
		this.dig_Main.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.dig_Main.Location = new System.Drawing.Point(2, 1);
		this.dig_Main.Name = "dig_Main";
		this.dig_Main.OptionsBehavior.SelectedStencils = new DevExpress.Diagram.Core.StencilCollection("BasicShapes", "BasicFlowchartShapes");
		this.dig_Main.OptionsConnector.LineJumpStyle = DevExpress.Diagram.Core.LineJumpDescriptions.Square;
		this.dig_Main.OptionsView.AllowShapeShadows = true;
		this.dig_Main.OptionsView.BringIntoViewMargin = 2f;
		this.dig_Main.OptionsView.CanvasSizeMode = DevExpress.Diagram.Core.CanvasSizeMode.Fill;
		this.dig_Main.OptionsView.PaperKind = System.Drawing.Printing.PaperKind.Letter;
		this.dig_Main.OptionsView.ShowGrid = false;
		this.dig_Main.OptionsView.ShowPageBreaks = false;
		this.dig_Main.OptionsView.ShowRulers = false;
		this.dig_Main.OptionsView.ToolboxVisibility = DevExpress.Diagram.Core.ToolboxVisibility.Closed;
		this.dig_Main.Size = new System.Drawing.Size(1132, 631);
		this.dig_Main.TabIndex = 0;
		this.dig_Main.Text = "diagramControl1";
		this.dig_Main.ShowingEditor += new System.EventHandler<DevExpress.XtraDiagram.DiagramShowingEditorEventArgs>(this.dig_Main_ShowingEditor);
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.EditControl = null;
		this.btn_OK.EditValue = "OK";
		this.btn_OK.FieldName = null;
		this.btn_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_OK.ImageOptions.Image");
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(2, 638);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.ReadOnly = false;
		this.btn_OK.SettingInVisible = false;
		this.btn_OK.SettingReadonly = false;
		this.btn_OK.Size = new System.Drawing.Size(75, 23);
		this.btn_OK.TabIndex = 1;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.marquee_Wait.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.marquee_Wait.EditValue = 0;
		this.marquee_Wait.Location = new System.Drawing.Point(879, 640);
		this.marquee_Wait.Name = "marquee_Wait";
		this.marquee_Wait.Size = new System.Drawing.Size(246, 18);
		this.marquee_Wait.TabIndex = 2;
		this.lb_Wait.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.lb_Wait.Appearance.ForeColor = System.Drawing.Color.DarkBlue;
		this.lb_Wait.Appearance.Options.UseForeColor = true;
		this.lb_Wait.Location = new System.Drawing.Point(683, 643);
		this.lb_Wait.Name = "lb_Wait";
		this.lb_Wait.Size = new System.Drawing.Size(176, 13);
		this.lb_Wait.TabIndex = 3;
		this.lb_Wait.Text = "Please wait, the diagram is initializing";
		this.btnChangeLayout.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnChangeLayout.EditControl = null;
		this.btnChangeLayout.EditValue = "";
		this.btnChangeLayout.Enabled = false;
		this.btnChangeLayout.FieldName = null;
		this.btnChangeLayout.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnChangeLayout.ImageOptions.Image");
		this.btnChangeLayout.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
		this.btnChangeLayout.IsSearchControl = false;
		this.btnChangeLayout.Location = new System.Drawing.Point(110, 637);
		this.btnChangeLayout.Name = "btnChangeLayout";
		this.btnChangeLayout.ReadOnly = false;
		this.btnChangeLayout.SettingInVisible = false;
		this.btnChangeLayout.SettingReadonly = false;
		this.btnChangeLayout.Size = new System.Drawing.Size(55, 22);
		this.btnChangeLayout.TabIndex = 4;
		this.btnChangeLayout.ToolTip = "Change Layout";
		this.btnChangeLayout.Click += new System.EventHandler(this.btnChangeLayout_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1137, 663);
		base.Controls.Add(this.btnChangeLayout);
		base.Controls.Add(this.lb_Wait);
		base.Controls.Add(this.marquee_Wait);
		base.Controls.Add(this.btn_OK);
		base.Controls.Add(this.dig_Main);
		base.DisableContextAction = true;
		base.DisableLoadScript = true;
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_Diagram.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_Diagram";
		base.ShowInTaskbar = false;
		this.Text = "Diagram";
		base.Load += new System.EventHandler(this.frm_Diagram_Load);
		((System.ComponentModel.ISupportInitialize)this.dig_Main).EndInit();
		((System.ComponentModel.ISupportInitialize)this.marquee_Wait.Properties).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
