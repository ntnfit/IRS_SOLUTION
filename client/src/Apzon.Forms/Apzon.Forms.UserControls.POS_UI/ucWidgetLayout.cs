using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Controls;
using Apzon.Entities.Models.Common;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraBars.Docking2010;
using DevExpress.XtraBars.Docking2010.Views;
using DevExpress.XtraBars.Docking2010.Views.Widget;
using DevExpress.XtraEditors;

namespace Apzon.Forms.UserControls.POS_UI;

public class ucWidgetLayout : XtraUserControl
{
	private int WidgetEntry = -100;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private DataTable _dtDocuments;

	private DataTable _dtHeader;

	private List<Common.Table_ID> _lstGroup = new List<Common.Table_ID>
	{
		new Common.Table_ID
		{
			ID = 1,
			Name = "Main"
		},
		new Common.Table_ID
		{
			ID = 2,
			Name = "Others"
		},
		new Common.Table_ID
		{
			ID = 3,
			Name = "Operation"
		}
	};

	private IContainer components;

	public DocumentManager documentManager1;

	public WidgetView widgetView1;

	public Document docStatusBar;

	public Document docGridCanvas;

	public Document docPaymentTotal;

	public Document docBarcode;

	public Document docDisplayScreen;

	public Document docCommandPanel;

	public Document docCalculator;

	private ColumnDefinition columnDefinition1;

	private ColumnDefinition columnDefinition2;

	private ColumnDefinition columnDefinition3;

	private ColumnDefinition columnDefinition4;

	private ColumnDefinition columnDefinition5;

	private ColumnDefinition columnDefinition6;

	private ColumnDefinition columnDefinition7;

	private ColumnDefinition columnDefinition8;

	private ColumnDefinition columnDefinition9;

	private ColumnDefinition columnDefinition10;

	private ColumnDefinition columnDefinition11;

	private ColumnDefinition columnDefinition12;

	private ColumnDefinition columnDefinition13;

	private ColumnDefinition columnDefinition14;

	private ColumnDefinition columnDefinition15;

	private ColumnDefinition columnDefinition16;

	private ColumnDefinition columnDefinition17;

	private ColumnDefinition columnDefinition18;

	private ColumnDefinition columnDefinition19;

	private ColumnDefinition columnDefinition20;

	private ColumnDefinition columnDefinition21;

	private ColumnDefinition columnDefinition22;

	private ColumnDefinition columnDefinition23;

	private ColumnDefinition columnDefinition24;

	private ColumnDefinition columnDefinition25;

	private ColumnDefinition columnDefinition26;

	private ColumnDefinition columnDefinition27;

	private ColumnDefinition columnDefinition28;

	private ColumnDefinition columnDefinition29;

	private ColumnDefinition columnDefinition30;

	private ColumnDefinition columnDefinition31;

	private ColumnDefinition columnDefinition32;

	private ColumnDefinition columnDefinition33;

	private ColumnDefinition columnDefinition34;

	private ColumnDefinition columnDefinition35;

	private RowDefinition rowDefinition1;

	private RowDefinition rowDefinition2;

	private RowDefinition rowDefinition3;

	private RowDefinition rowDefinition4;

	private RowDefinition rowDefinition5;

	private RowDefinition rowDefinition6;

	private RowDefinition rowDefinition7;

	private RowDefinition rowDefinition8;

	private RowDefinition rowDefinition9;

	private RowDefinition rowDefinition10;

	private RowDefinition rowDefinition11;

	private RowDefinition rowDefinition12;

	private RowDefinition rowDefinition13;

	private RowDefinition rowDefinition14;

	private RowDefinition rowDefinition15;

	private RowDefinition rowDefinition16;

	private RowDefinition rowDefinition17;

	private RowDefinition rowDefinition18;

	private RowDefinition rowDefinition19;

	private RowDefinition rowDefinition20;

	private RowDefinition rowDefinition21;

	private RowDefinition rowDefinition22;

	private RowDefinition rowDefinition23;

	private RowDefinition rowDefinition24;

	private RowDefinition rowDefinition25;

	private RowDefinition rowDefinition26;

	private RowDefinition rowDefinition27;

	private RowDefinition rowDefinition28;

	private RowDefinition rowDefinition29;

	private RowDefinition rowDefinition30;

	private RowDefinition rowDefinition31;

	private RowDefinition rowDefinition32;

	private RowDefinition rowDefinition33;

	private RowDefinition rowDefinition34;

	private RowDefinition rowDefinition35;

	public ucWidgetLayout()
	{
		InitializeComponent();
		Load += OnLoad;
		HideCloseButton();
		HideMaximizeButton();
	}

	private void OnLoad(object sender, EventArgs eventArgs)
	{
		LoadMainLayout();
	}

	private async void widgetView1_QueryControl(object sender, QueryControlEventArgs e)
	{
		try
		{
			Type type = Type.GetType(e.Document.ControlTypeName + "." + e.Document.ControlName);
			if (!(type != null))
			{
				return;
			}
			Control control = Activator.CreateInstance(type) as Control;
			if (control != null)
			{
				e.Control = control;
			}
			ucMainCmd mainCmd = ((control is ucCommandPanel ucCommandPanel2) ? ucCommandPanel2.ucMainCmd1 : null);
			TileControl tileControl = mainCmd?.tile_Commands;
			if (tileControl == null)
			{
				return;
			}
			DataTable dataTable = await _httpClient.Retail.LoadCmdConfig();
			if (dataTable.IsNotNull())
			{
				TileItemElementCollection elements = tileControl.Groups[0].Items[0].Elements;
				tileControl.Groups.Clear();
				foreach (int grCode in (from t in dataTable.AsEnumerable()
					select Function.ParseInt(t["MnGroup"])).Distinct().ToList())
				{
					DataRow[] source = dataTable.Select($"MnGroup = {grCode}");
					if (!source.Any())
					{
						continue;
					}
					DataTable dataTable2 = source.CopyToDataTable();
					List<TileItem> list = new List<TileItem>();
					TileGroup tileGroup = new TileGroup();
					for (int num = 0; num < dataTable2.Rows.Count; num++)
					{
						DataRow dataRow = dataTable2.Rows[num];
						TileItem tileItem = new TileItem
						{
							ItemSize = TileItemSize.Small
						};
						tileItem.AppearanceItem.Normal.Font = new Font(new FontFamily("Tahoma"), 11.25f, FontStyle.Bold);
						tileItem.Elements.Assign(elements);
						tileItem.AppearanceItem.Normal.BackColor = Color.FromArgb(Function.ParseInt(dataRow["BackColor"]));
						tileItem.AppearanceItem.Normal.ForeColor = Color.FromArgb(Function.ParseInt(dataRow["TextColor"]));
						tileItem.BorderVisibility = TileItemBorderVisibility.Never;
						string tag = Function.ToString(dataRow["Hotkey"]);
						tileItem.Text = Function.ToString(dataRow["Text"]);
						tileItem.Tag = tag;
						tileItem.Visible = Function.ToString(dataRow["Visible"]) == "Y";
						tileItem.Name = "tileItem_" + Function.ToString(dataRow["CmdName"]);
						list.Add(tileItem);
					}
					tileGroup.Items.Clear();
					foreach (TileItem item in list)
					{
						tileGroup.Items.Add(item);
					}
					tileGroup.Tag = grCode;
					Common.Table_ID table_ID = _lstGroup.AsEnumerable().FirstOrDefault((Common.Table_ID t) => Function.ParseInt(t.ID) == grCode);
					if (table_ID != null)
					{
						tileGroup.Name = "group" + table_ID.Name;
					}
					tileControl.Groups.Add(tileGroup);
				}
			}
			for (int num2 = 0; num2 < tileControl.Groups.Count; num2++)
			{
				TileGroup tileGroup2 = tileControl.Groups[num2];
				for (int num3 = 0; num3 < tileGroup2.Items.Count; num3++)
				{
					TileItem tileItem2 = tileGroup2.Items[num3];
					string text = Function.ToString(tileItem2.Tag);
					if (!string.IsNullOrEmpty(text))
					{
						tileItem2.Text = tileItem2.Text + " (" + text.Replace(",", "+") + ")";
					}
				}
			}
			TileControl tileControl2 = mainCmd?.tile_Menu;
			if (tileControl2 == null)
			{
				return;
			}
			for (int num4 = 0; num4 < tileControl2.Groups[0].Items.Count; num4++)
			{
				TileItem it = tileControl2.Groups[0].Items[num4];
				it.ItemClick += (object o, TileItemEventArgs args) =>
				{
					VisibleGroup(it.Name.Substring(9, it.Name.Length - 9), tileControl);
				};
			}
			if (tileControl2.Groups[0].Items.Count > 0)
			{
				tileControl2.Groups[0].Items[0].PerformItemClick();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void VisibleGroup(string groupName, TileControl tileControl)
	{
		try
		{
			for (int i = 0; i < tileControl.Groups.Count; i++)
			{
				TileGroup tileGroup = tileControl.Groups[i];
				tileGroup.Visible = tileGroup.Name == groupName;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadMainLayout()
	{
		try
		{
			WidgetView widgetView = widgetView1;
			widgetView.BeginUpdateAnimation();
			if (WidgetEntry != 0)
			{
				_dtHeader = Task.Run(() => _httpClient.Widget.GetHeader(WidgetEntry)).Result;
				if (_dtHeader.IsNotNull())
				{
					Text = Function.ToString(_dtHeader.Rows[0]["Caption"]);
					widgetView.LayoutMode = (LayoutMode)Function.ParseInt(_dtHeader.Rows[0]["LayoutMode"]);
				}
				_dtDocuments = Task.Run(() => _httpClient.Widget.GetDocument(WidgetEntry)).Result;
				if (_dtDocuments.IsNotNull())
				{
					widgetView.Documents.Clear();
					for (int num = 0; num < _dtDocuments.Rows.Count; num++)
					{
						Document document = new Document();
						document.Caption = Function.ToString(_dtDocuments.Rows[num]["Caption"]);
						document.ColumnIndex = Function.ParseInt(_dtDocuments.Rows[num]["ColumnIndex"]);
						document.ColumnSpan = Function.ParseInt(_dtDocuments.Rows[num]["ColumnSpan"]);
						document.ControlName = Function.ToString(_dtDocuments.Rows[num]["ObjCode"]);
						document.ControlTypeName = Function.ToString(_dtDocuments.Rows[num]["ObjType"]);
						document.RowIndex = Function.ParseInt(_dtDocuments.Rows[num]["RowIndex"]);
						document.RowSpan = Function.ParseInt(_dtDocuments.Rows[num]["RowSpan"]);
						document.Height = Function.ParseInt(_dtDocuments.Rows[num]["Height"]);
						document.Width = Function.ParseInt(_dtDocuments.Rows[num]["Width"]);
						document.FreeLayoutHeight.UnitType = LengthUnitType.Star;
						document.FreeLayoutHeight.UnitValue = Function.ParseDouble(_dtDocuments.Rows[num]["FreeLayoutHeight"]);
						document.FreeLayoutWidth.UnitType = LengthUnitType.Star;
						document.FreeLayoutWidth.UnitValue = Function.ParseDouble(_dtDocuments.Rows[num]["FreeLayoutWidth"]);
						document.ToolTip = Function.ToString(_dtDocuments.Rows[num]["ToolTip"]);
						Document document2 = document;
						widgetView.Documents.Add(document2);
						int num2 = Function.ParseInt(_dtDocuments.Rows[num]["StackGroup"]);
						if (widgetView.StackGroups != null && widgetView.StackGroups.Count > num2)
						{
							widgetView.Controller.Dock(document2, widgetView.StackGroups[num2]);
						}
					}
				}
			}
			widgetView.EndUpdateAnimation();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
	}

	public void HideCaption()
	{
		try
		{
			foreach (Document item in (IEnumerable<BaseDocument>)widgetView1.Documents)
			{
				item.Properties.ShowBorders = DefaultBoolean.False;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void HideCloseButton()
	{
		try
		{
			foreach (Document item in (IEnumerable<BaseDocument>)widgetView1.Documents)
			{
				item.Properties.ShowCloseButton = DefaultBoolean.False;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void HideMaximizeButton()
	{
		try
		{
			foreach (Document item in (IEnumerable<BaseDocument>)widgetView1.Documents)
			{
				item.Properties.ShowMaximizeButton = DefaultBoolean.False;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
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
		this.documentManager1 = new DevExpress.XtraBars.Docking2010.DocumentManager(this.components);
		this.widgetView1 = new DevExpress.XtraBars.Docking2010.Views.Widget.WidgetView(this.components);
		this.docStatusBar = new DevExpress.XtraBars.Docking2010.Views.Widget.Document(this.components);
		this.docGridCanvas = new DevExpress.XtraBars.Docking2010.Views.Widget.Document(this.components);
		this.docPaymentTotal = new DevExpress.XtraBars.Docking2010.Views.Widget.Document(this.components);
		this.docBarcode = new DevExpress.XtraBars.Docking2010.Views.Widget.Document(this.components);
		this.docDisplayScreen = new DevExpress.XtraBars.Docking2010.Views.Widget.Document(this.components);
		this.docCommandPanel = new DevExpress.XtraBars.Docking2010.Views.Widget.Document(this.components);
		this.docCalculator = new DevExpress.XtraBars.Docking2010.Views.Widget.Document(this.components);
		this.rowDefinition1 = new DevExpress.XtraBars.Docking2010.Views.Widget.RowDefinition();
		this.rowDefinition2 = new DevExpress.XtraBars.Docking2010.Views.Widget.RowDefinition();
		this.rowDefinition3 = new DevExpress.XtraBars.Docking2010.Views.Widget.RowDefinition();
		this.rowDefinition4 = new DevExpress.XtraBars.Docking2010.Views.Widget.RowDefinition();
		this.rowDefinition5 = new DevExpress.XtraBars.Docking2010.Views.Widget.RowDefinition();
		this.rowDefinition6 = new DevExpress.XtraBars.Docking2010.Views.Widget.RowDefinition();
		this.rowDefinition7 = new DevExpress.XtraBars.Docking2010.Views.Widget.RowDefinition();
		this.rowDefinition8 = new DevExpress.XtraBars.Docking2010.Views.Widget.RowDefinition();
		this.rowDefinition9 = new DevExpress.XtraBars.Docking2010.Views.Widget.RowDefinition();
		this.rowDefinition10 = new DevExpress.XtraBars.Docking2010.Views.Widget.RowDefinition();
		this.rowDefinition11 = new DevExpress.XtraBars.Docking2010.Views.Widget.RowDefinition();
		this.rowDefinition12 = new DevExpress.XtraBars.Docking2010.Views.Widget.RowDefinition();
		this.rowDefinition13 = new DevExpress.XtraBars.Docking2010.Views.Widget.RowDefinition();
		this.rowDefinition14 = new DevExpress.XtraBars.Docking2010.Views.Widget.RowDefinition();
		this.rowDefinition15 = new DevExpress.XtraBars.Docking2010.Views.Widget.RowDefinition();
		this.rowDefinition16 = new DevExpress.XtraBars.Docking2010.Views.Widget.RowDefinition();
		this.rowDefinition17 = new DevExpress.XtraBars.Docking2010.Views.Widget.RowDefinition();
		this.rowDefinition18 = new DevExpress.XtraBars.Docking2010.Views.Widget.RowDefinition();
		this.rowDefinition19 = new DevExpress.XtraBars.Docking2010.Views.Widget.RowDefinition();
		this.rowDefinition20 = new DevExpress.XtraBars.Docking2010.Views.Widget.RowDefinition();
		this.rowDefinition21 = new DevExpress.XtraBars.Docking2010.Views.Widget.RowDefinition();
		this.rowDefinition22 = new DevExpress.XtraBars.Docking2010.Views.Widget.RowDefinition();
		this.rowDefinition23 = new DevExpress.XtraBars.Docking2010.Views.Widget.RowDefinition();
		this.rowDefinition24 = new DevExpress.XtraBars.Docking2010.Views.Widget.RowDefinition();
		this.rowDefinition25 = new DevExpress.XtraBars.Docking2010.Views.Widget.RowDefinition();
		this.rowDefinition26 = new DevExpress.XtraBars.Docking2010.Views.Widget.RowDefinition();
		this.rowDefinition27 = new DevExpress.XtraBars.Docking2010.Views.Widget.RowDefinition();
		this.rowDefinition28 = new DevExpress.XtraBars.Docking2010.Views.Widget.RowDefinition();
		this.rowDefinition29 = new DevExpress.XtraBars.Docking2010.Views.Widget.RowDefinition();
		this.rowDefinition30 = new DevExpress.XtraBars.Docking2010.Views.Widget.RowDefinition();
		this.rowDefinition31 = new DevExpress.XtraBars.Docking2010.Views.Widget.RowDefinition();
		this.rowDefinition32 = new DevExpress.XtraBars.Docking2010.Views.Widget.RowDefinition();
		this.rowDefinition33 = new DevExpress.XtraBars.Docking2010.Views.Widget.RowDefinition();
		this.rowDefinition34 = new DevExpress.XtraBars.Docking2010.Views.Widget.RowDefinition();
		this.rowDefinition35 = new DevExpress.XtraBars.Docking2010.Views.Widget.RowDefinition();
		this.columnDefinition1 = new DevExpress.XtraBars.Docking2010.Views.Widget.ColumnDefinition();
		this.columnDefinition2 = new DevExpress.XtraBars.Docking2010.Views.Widget.ColumnDefinition();
		this.columnDefinition3 = new DevExpress.XtraBars.Docking2010.Views.Widget.ColumnDefinition();
		this.columnDefinition4 = new DevExpress.XtraBars.Docking2010.Views.Widget.ColumnDefinition();
		this.columnDefinition5 = new DevExpress.XtraBars.Docking2010.Views.Widget.ColumnDefinition();
		this.columnDefinition6 = new DevExpress.XtraBars.Docking2010.Views.Widget.ColumnDefinition();
		this.columnDefinition7 = new DevExpress.XtraBars.Docking2010.Views.Widget.ColumnDefinition();
		this.columnDefinition8 = new DevExpress.XtraBars.Docking2010.Views.Widget.ColumnDefinition();
		this.columnDefinition9 = new DevExpress.XtraBars.Docking2010.Views.Widget.ColumnDefinition();
		this.columnDefinition10 = new DevExpress.XtraBars.Docking2010.Views.Widget.ColumnDefinition();
		this.columnDefinition11 = new DevExpress.XtraBars.Docking2010.Views.Widget.ColumnDefinition();
		this.columnDefinition12 = new DevExpress.XtraBars.Docking2010.Views.Widget.ColumnDefinition();
		this.columnDefinition13 = new DevExpress.XtraBars.Docking2010.Views.Widget.ColumnDefinition();
		this.columnDefinition14 = new DevExpress.XtraBars.Docking2010.Views.Widget.ColumnDefinition();
		this.columnDefinition15 = new DevExpress.XtraBars.Docking2010.Views.Widget.ColumnDefinition();
		this.columnDefinition16 = new DevExpress.XtraBars.Docking2010.Views.Widget.ColumnDefinition();
		this.columnDefinition17 = new DevExpress.XtraBars.Docking2010.Views.Widget.ColumnDefinition();
		this.columnDefinition18 = new DevExpress.XtraBars.Docking2010.Views.Widget.ColumnDefinition();
		this.columnDefinition19 = new DevExpress.XtraBars.Docking2010.Views.Widget.ColumnDefinition();
		this.columnDefinition20 = new DevExpress.XtraBars.Docking2010.Views.Widget.ColumnDefinition();
		this.columnDefinition21 = new DevExpress.XtraBars.Docking2010.Views.Widget.ColumnDefinition();
		this.columnDefinition22 = new DevExpress.XtraBars.Docking2010.Views.Widget.ColumnDefinition();
		this.columnDefinition23 = new DevExpress.XtraBars.Docking2010.Views.Widget.ColumnDefinition();
		this.columnDefinition24 = new DevExpress.XtraBars.Docking2010.Views.Widget.ColumnDefinition();
		this.columnDefinition25 = new DevExpress.XtraBars.Docking2010.Views.Widget.ColumnDefinition();
		this.columnDefinition26 = new DevExpress.XtraBars.Docking2010.Views.Widget.ColumnDefinition();
		this.columnDefinition27 = new DevExpress.XtraBars.Docking2010.Views.Widget.ColumnDefinition();
		this.columnDefinition28 = new DevExpress.XtraBars.Docking2010.Views.Widget.ColumnDefinition();
		this.columnDefinition29 = new DevExpress.XtraBars.Docking2010.Views.Widget.ColumnDefinition();
		this.columnDefinition30 = new DevExpress.XtraBars.Docking2010.Views.Widget.ColumnDefinition();
		this.columnDefinition31 = new DevExpress.XtraBars.Docking2010.Views.Widget.ColumnDefinition();
		this.columnDefinition32 = new DevExpress.XtraBars.Docking2010.Views.Widget.ColumnDefinition();
		this.columnDefinition33 = new DevExpress.XtraBars.Docking2010.Views.Widget.ColumnDefinition();
		this.columnDefinition34 = new DevExpress.XtraBars.Docking2010.Views.Widget.ColumnDefinition();
		this.columnDefinition35 = new DevExpress.XtraBars.Docking2010.Views.Widget.ColumnDefinition();
		((System.ComponentModel.ISupportInitialize)this.documentManager1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.widgetView1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.docStatusBar).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.docGridCanvas).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.docPaymentTotal).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.docBarcode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.docDisplayScreen).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.docCommandPanel).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.docCalculator).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition6).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition7).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition8).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition9).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition10).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition11).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition12).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition13).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition14).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition15).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition16).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition17).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition18).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition19).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition20).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition21).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition22).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition23).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition24).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition25).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition26).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition27).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition28).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition29).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition30).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition31).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition32).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition33).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition34).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition35).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition6).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition7).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition8).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition9).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition10).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition11).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition12).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition13).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition14).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition15).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition16).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition17).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition18).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition19).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition20).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition21).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition22).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition23).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition24).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition25).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition26).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition27).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition28).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition29).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition30).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition31).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition32).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition33).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition34).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition35).BeginInit();
		base.SuspendLayout();
		this.documentManager1.ContainerControl = this;
		this.documentManager1.View = this.widgetView1;
		this.documentManager1.ViewCollection.AddRange(new DevExpress.XtraBars.Docking2010.Views.BaseView[1] { this.widgetView1 });
		this.widgetView1.Columns.AddRange(new DevExpress.XtraBars.Docking2010.Views.Widget.ColumnDefinition[35]
		{
			this.columnDefinition1, this.columnDefinition2, this.columnDefinition3, this.columnDefinition4, this.columnDefinition5, this.columnDefinition6, this.columnDefinition7, this.columnDefinition8, this.columnDefinition9, this.columnDefinition10,
			this.columnDefinition11, this.columnDefinition12, this.columnDefinition13, this.columnDefinition14, this.columnDefinition15, this.columnDefinition16, this.columnDefinition17, this.columnDefinition18, this.columnDefinition19, this.columnDefinition20,
			this.columnDefinition21, this.columnDefinition22, this.columnDefinition23, this.columnDefinition24, this.columnDefinition25, this.columnDefinition26, this.columnDefinition27, this.columnDefinition28, this.columnDefinition29, this.columnDefinition30,
			this.columnDefinition31, this.columnDefinition32, this.columnDefinition33, this.columnDefinition34, this.columnDefinition35
		});
		this.widgetView1.Documents.AddRange(new DevExpress.XtraBars.Docking2010.Views.BaseDocument[7] { this.docStatusBar, this.docGridCanvas, this.docPaymentTotal, this.docBarcode, this.docDisplayScreen, this.docCommandPanel, this.docCalculator });
		this.widgetView1.LayoutMode = DevExpress.XtraBars.Docking2010.Views.Widget.LayoutMode.TableLayout;
		this.widgetView1.RootContainer.Element = null;
		this.widgetView1.RootContainer.Orientation = System.Windows.Forms.Orientation.Vertical;
		this.widgetView1.Rows.AddRange(new DevExpress.XtraBars.Docking2010.Views.Widget.RowDefinition[35]
		{
			this.rowDefinition1, this.rowDefinition2, this.rowDefinition3, this.rowDefinition4, this.rowDefinition5, this.rowDefinition6, this.rowDefinition7, this.rowDefinition8, this.rowDefinition9, this.rowDefinition10,
			this.rowDefinition11, this.rowDefinition12, this.rowDefinition13, this.rowDefinition14, this.rowDefinition15, this.rowDefinition16, this.rowDefinition17, this.rowDefinition18, this.rowDefinition19, this.rowDefinition20,
			this.rowDefinition21, this.rowDefinition22, this.rowDefinition23, this.rowDefinition24, this.rowDefinition25, this.rowDefinition26, this.rowDefinition27, this.rowDefinition28, this.rowDefinition29, this.rowDefinition30,
			this.rowDefinition31, this.rowDefinition32, this.rowDefinition33, this.rowDefinition34, this.rowDefinition35
		});
		this.widgetView1.QueryControl += new DevExpress.XtraBars.Docking2010.Views.QueryControlEventHandler(this.widgetView1_QueryControl);
		this.docStatusBar.Caption = "Status Bar";
		this.docStatusBar.ColumnSpan = 35;
		this.docStatusBar.ControlName = "ucStatusBar";
		this.docStatusBar.ControlTypeName = "Apzon.Forms.UserControls.POS_UI";
		this.docStatusBar.Height = 27;
		this.docStatusBar.RowSpan = 2;
		this.docStatusBar.Width = 1196;
		this.docGridCanvas.Caption = "Retail Grid";
		this.docGridCanvas.ColumnSpan = 21;
		this.docGridCanvas.ControlName = "ucGridCanvas";
		this.docGridCanvas.ControlTypeName = "Apzon.Forms.UserControls.POS_UI";
		this.docGridCanvas.Height = 636;
		this.docGridCanvas.RowIndex = 2;
		this.docGridCanvas.RowSpan = 26;
		this.docGridCanvas.Width = 761;
		this.docPaymentTotal.Caption = "Payment Total";
		this.docPaymentTotal.ColumnSpan = 21;
		this.docPaymentTotal.ControlName = "ucPaymentTotal";
		this.docPaymentTotal.ControlTypeName = "Apzon.Forms.UserControls.POS_UI";
		this.docPaymentTotal.Height = 120;
		this.docPaymentTotal.RowIndex = 28;
		this.docPaymentTotal.RowSpan = 5;
		this.docPaymentTotal.Width = 759;
		this.docBarcode.Caption = "Barcode";
		this.docBarcode.ColumnSpan = 21;
		this.docBarcode.ControlName = "ucBarcode";
		this.docBarcode.ControlTypeName = "Apzon.Forms.UserControls.POS_UI";
		this.docBarcode.Height = 49;
		this.docBarcode.RowIndex = 33;
		this.docBarcode.RowSpan = 2;
		this.docBarcode.Width = 763;
		this.docDisplayScreen.Caption = "Scanned Item";
		this.docDisplayScreen.ColumnIndex = 21;
		this.docDisplayScreen.ColumnSpan = 14;
		this.docDisplayScreen.ControlName = "ucDisplayScreen";
		this.docDisplayScreen.ControlTypeName = "Apzon.Forms.UserControls.POS_UI";
		this.docDisplayScreen.RowIndex = 2;
		this.docDisplayScreen.RowSpan = 4;
		this.docCommandPanel.Caption = "Command Panel";
		this.docCommandPanel.ColumnIndex = 21;
		this.docCommandPanel.ColumnSpan = 14;
		this.docCommandPanel.ControlName = "ucCommandPanel";
		this.docCommandPanel.ControlTypeName = "Apzon.Forms.UserControls.POS_UI";
		this.docCommandPanel.RowIndex = 6;
		this.docCommandPanel.RowSpan = 17;
		this.docCalculator.Caption = "NumPad";
		this.docCalculator.ColumnIndex = 23;
		this.docCalculator.ColumnSpan = 12;
		this.docCalculator.ControlName = "ucNumPad";
		this.docCalculator.ControlTypeName = "Apzon.Forms.UserControls.POS_UI";
		this.docCalculator.Height = 289;
		this.docCalculator.RowIndex = 23;
		this.docCalculator.RowSpan = 12;
		this.docCalculator.Width = 428;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Name = "ucWidgetLayout";
		base.Size = new System.Drawing.Size(1211, 708);
		((System.ComponentModel.ISupportInitialize)this.documentManager1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.widgetView1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.docStatusBar).EndInit();
		((System.ComponentModel.ISupportInitialize)this.docGridCanvas).EndInit();
		((System.ComponentModel.ISupportInitialize)this.docPaymentTotal).EndInit();
		((System.ComponentModel.ISupportInitialize)this.docBarcode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.docDisplayScreen).EndInit();
		((System.ComponentModel.ISupportInitialize)this.docCommandPanel).EndInit();
		((System.ComponentModel.ISupportInitialize)this.docCalculator).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition6).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition7).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition8).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition9).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition10).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition11).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition12).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition13).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition14).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition15).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition16).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition17).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition18).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition19).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition20).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition21).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition22).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition23).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition24).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition25).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition26).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition27).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition28).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition29).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition30).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition31).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition32).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition33).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition34).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rowDefinition35).EndInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition6).EndInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition7).EndInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition8).EndInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition9).EndInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition10).EndInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition11).EndInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition12).EndInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition13).EndInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition14).EndInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition15).EndInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition16).EndInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition17).EndInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition18).EndInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition19).EndInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition20).EndInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition21).EndInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition22).EndInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition23).EndInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition24).EndInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition25).EndInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition26).EndInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition27).EndInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition28).EndInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition29).EndInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition30).EndInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition31).EndInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition32).EndInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition33).EndInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition34).EndInit();
		((System.ComponentModel.ISupportInitialize)this.columnDefinition35).EndInit();
		base.ResumeLayout(false);
	}
}
