using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Form;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.TableLayout;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Tile;

namespace Apzon.Forms.Widget;

public class frm_WidgetAddItem : XtraFormEx
{
	private readonly int _widgetEntry;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private readonly bool _multiselect;

	private readonly Enums.WidgetItemType _widgetItemType;

	private IContainer components;

	private GridControl grd_Main;

	private TileView titleView_Main;

	private TileViewColumn col_Image;

	private TileViewColumn col_Name;

	private TileViewColumn col_Category;

	private TileViewColumn col_Checked;

	private ImageCollection imageCollection1;

	private SimpleButtonEx btn_OK;

	private SimpleButtonEx btn_Cancel;

	private TileViewColumn col_ObjType;

	public Action<string, string, bool> UpdateDocument { get; set; }

	public Action<DataRow[]> SelectItem { get; set; }

	public DataTable WidgetSource { get; set; }

	public frm_WidgetAddItem()
	{
		InitializeComponent();
	}

	public frm_WidgetAddItem(int widgetEntry)
	{
		InitializeComponent();
		_widgetEntry = widgetEntry;
	}

	public frm_WidgetAddItem(bool multiselect, Enums.WidgetItemType widgetItemType)
	{
		InitializeComponent();
		_multiselect = multiselect;
		_widgetItemType = widgetItemType;
	}

	private async void frm_WidgetAddItem_Load(object sender, EventArgs e)
	{
		try
		{
			DataTable dataTable = await _httpClient.Widget.GetItems(_widgetEntry);
			DataRow[] source = (from t in dataTable.AsEnumerable()
				where !string.IsNullOrEmpty(Function.ToString(t["Name"]))
				select t).ToArray();
			if (!source.Any())
			{
				return;
			}
			DataTable dataTable2 = source.CopyToDataTable();
			DataTable ret = dataTable.Clone();
			if (WidgetSource.IsNotNull())
			{
				IEnumerable<DataRow> enumerable = from a in dataTable2.AsEnumerable()
					join b in WidgetSource.AsEnumerable() on new
					{
						ObjType = Function.ToString(a["ObjType"]),
						ObjCode = Function.ToString(a["ObjCode"])
					} equals new
					{
						ObjType = Function.ToString(b["ObjType"]),
						ObjCode = Function.ToString(b["ObjCode"])
					} into grp
					from item in grp.DefaultIfEmpty()
					select ret.LoadDataRow(new object[6]
					{
						Function.ToString(a["ObjCode"]),
						Function.ToString(a["Name"]),
						Function.ToString(a["Category"]),
						Function.ToString(a["ImageData"]),
						Function.ToString(a["ObjType"]),
						item != null
					}, fAcceptChanges: true);
				IList<DataRow> source2 = (enumerable as IList<DataRow>) ?? enumerable.ToList();
				if (source2.Any())
				{
					ret = source2.CopyToDataTable();
				}
			}
			else
			{
				ret = dataTable2.Copy();
			}
			grd_Main.DataSource = ret;
			titleView_Main.OptionsSelection.MultiSelect = _multiselect;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void titleView_Main_ItemCustomize(object sender, TileViewItemCustomizeEventArgs e)
	{
		try
		{
			if (!(e.Item.Elements[0] is TileViewItemElement tileViewItemElement) || Function.ParseInt(titleView_Main.GetRowCellValue(e.Item.RowHandle, col_ObjType)) != 7000030)
			{
				return;
			}
			XElement xElement = XDocument.Parse(Function.ToString(tileViewItemElement)).Descendants("Items").Elements()
				.FirstOrDefault();
			if (xElement != null)
			{
				switch (xElement.Name.LocalName.ToLower())
				{
				case "grid":
				case "pivot":
					e.Item.ImageIndex = 0;
					break;
				case "chart":
					e.Item.ImageIndex = 1;
					break;
				case "scatterchart":
					e.Item.ImageIndex = 2;
					break;
				case "pie":
					e.Item.ImageIndex = 3;
					break;
				case "gauge":
					e.Item.ImageIndex = 4;
					break;
				case "card":
					e.Item.ImageIndex = 5;
					break;
				case "piemap":
				case "geopointmap":
				case "bubblemap":
				case "choroplethmap":
					e.Item.ImageIndex = 6;
					break;
				default:
					e.Item.ImageIndex = 1;
					break;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void titleView_Main_ItemClick(object sender, TileViewItemClickEventArgs e)
	{
		try
		{
			e.Item.Checked = !e.Item.Checked;
			if (UpdateDocument != null && e.Item.Elements[0] is TileViewItemElement)
			{
				string arg = Function.ToString(titleView_Main.GetRowCellValue(e.Item.RowHandle, col_ObjType));
				string arg2 = Function.ToString(titleView_Main.GetRowCellValue(e.Item.RowHandle, "ObjCode"));
				bool arg3 = e.Item.Checked;
				UpdateDocument(arg, arg2, arg3);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_OK_Click(object sender, EventArgs e)
	{
		try
		{
			if (SelectItem != null && titleView_Main.DataSource is DataView dataView)
			{
				DataTable dataTable = dataView.ToTable();
				if (dataTable.IsNotNull())
				{
					EnumerableRowCollection<DataRow> source = from t in dataTable.AsEnumerable()
						where Function.ParseBool(t["Checked"])
						select t;
					SelectItem(source.ToArray());
				}
			}
			Close();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		try
		{
			Close();
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
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableRowDefinition tableRowDefinition = new DevExpress.XtraEditors.TableLayout.TableRowDefinition();
		DevExpress.XtraEditors.TableLayout.TableRowDefinition tableRowDefinition2 = new DevExpress.XtraEditors.TableLayout.TableRowDefinition();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement2 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Widget.frm_WidgetAddItem));
		this.col_Image = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.col_Name = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.grd_Main = new DevExpress.XtraGrid.GridControl();
		this.titleView_Main = new DevExpress.XtraGrid.Views.Tile.TileView();
		this.col_Category = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.col_Checked = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.col_ObjType = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.imageCollection1 = new DevExpress.Utils.ImageCollection(this.components);
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.grd_Main).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.titleView_Main).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.imageCollection1).BeginInit();
		base.SuspendLayout();
		this.col_Image.Caption = "Image";
		this.col_Image.FieldName = "ImageData";
		this.col_Image.Name = "col_Image";
		this.col_Image.Visible = true;
		this.col_Image.VisibleIndex = 0;
		this.col_Name.Caption = "Name";
		this.col_Name.FieldName = "Name";
		this.col_Name.Name = "col_Name";
		this.col_Name.Visible = true;
		this.col_Name.VisibleIndex = 1;
		this.grd_Main.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Main.Location = new System.Drawing.Point(0, 0);
		this.grd_Main.MainView = this.titleView_Main;
		this.grd_Main.Name = "grd_Main";
		this.grd_Main.Size = new System.Drawing.Size(823, 622);
		this.grd_Main.TabIndex = 0;
		this.grd_Main.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.titleView_Main });
		this.titleView_Main.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
		this.titleView_Main.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[5] { this.col_Image, this.col_Name, this.col_Category, this.col_Checked, this.col_ObjType });
		this.titleView_Main.ColumnSet.CheckedColumn = this.col_Checked;
		this.titleView_Main.ColumnSet.GroupColumn = this.col_Category;
		this.titleView_Main.GridControl = this.grd_Main;
		this.titleView_Main.Images = this.imageCollection1;
		this.titleView_Main.Name = "titleView_Main";
		this.titleView_Main.OptionsBehavior.AllowSmoothScrolling = true;
		this.titleView_Main.OptionsFind.AlwaysVisible = true;
		this.titleView_Main.OptionsTiles.AllowItemHover = true;
		this.titleView_Main.OptionsTiles.ColumnCount = 4;
		this.titleView_Main.OptionsTiles.HorizontalContentAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.titleView_Main.OptionsTiles.ItemBorderVisibility = DevExpress.XtraEditors.TileItemBorderVisibility.Never;
		this.titleView_Main.OptionsTiles.ItemSize = new System.Drawing.Size(175, 175);
		this.titleView_Main.OptionsTiles.RowCount = 4;
		this.titleView_Main.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[1]
		{
			new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.col_Category, DevExpress.Data.ColumnSortOrder.Ascending)
		});
		tableColumnDefinition.Length.Value = 163.0;
		this.titleView_Main.TileColumns.Add(tableColumnDefinition);
		tableRowDefinition.Length.Value = 120.0;
		tableRowDefinition2.Length.Value = 36.0;
		this.titleView_Main.TileRows.Add(tableRowDefinition);
		this.titleView_Main.TileRows.Add(tableRowDefinition2);
		tileViewItemElement.Column = this.col_Image;
		tileViewItemElement.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.NoScale;
		tileViewItemElement.Text = "col_Image";
		tileViewItemElement.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement.TextVisible = false;
		tileViewItemElement2.Column = this.col_Name;
		tileViewItemElement2.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement2.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement2.RowIndex = 1;
		tileViewItemElement2.Text = "col_Name";
		tileViewItemElement2.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		this.titleView_Main.TileTemplate.Add(tileViewItemElement);
		this.titleView_Main.TileTemplate.Add(tileViewItemElement2);
		this.titleView_Main.ItemClick += new DevExpress.XtraGrid.Views.Tile.TileViewItemClickEventHandler(this.titleView_Main_ItemClick);
		this.titleView_Main.ItemCustomize += new DevExpress.XtraGrid.Views.Tile.TileViewItemCustomizeEventHandler(this.titleView_Main_ItemCustomize);
		this.col_Category.Caption = "Category";
		this.col_Category.FieldName = "Category";
		this.col_Category.Name = "col_Category";
		this.col_Category.Visible = true;
		this.col_Category.VisibleIndex = 2;
		this.col_Checked.Caption = "Checked";
		this.col_Checked.FieldName = "Checked";
		this.col_Checked.Name = "col_Checked";
		this.col_Checked.Visible = true;
		this.col_Checked.VisibleIndex = 3;
		this.col_ObjType.Caption = "Object Type";
		this.col_ObjType.FieldName = "ObjType";
		this.col_ObjType.Name = "col_ObjType";
		this.col_ObjType.Visible = true;
		this.col_ObjType.VisibleIndex = 4;
		this.imageCollection1.ImageSize = new System.Drawing.Size(100, 100);
		this.imageCollection1.ImageStream = (DevExpress.Utils.ImageCollectionStreamer)resources.GetObject("imageCollection1.ImageStream");
		this.imageCollection1.Images.SetKeyName(0, "icon-grid");
		this.imageCollection1.Images.SetKeyName(1, "icon-chart");
		this.imageCollection1.Images.SetKeyName(2, "icon-scatter");
		this.imageCollection1.Images.SetKeyName(3, "icon-pies");
		this.imageCollection1.Images.SetKeyName(4, "icon-gauges");
		this.imageCollection1.Images.SetKeyName(5, "icon-card");
		this.imageCollection1.Images.SetKeyName(6, "icon-map");
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_OK.ImageOptions.Image");
		this.btn_OK.Location = new System.Drawing.Point(2, 630);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.Size = new System.Drawing.Size(75, 23);
		this.btn_OK.TabIndex = 1;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.Location = new System.Drawing.Point(82, 630);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 2;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(823, 660);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_OK);
		base.Controls.Add(this.grd_Main);
		base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
		base.Name = "frm_WidgetAddItem";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		this.Text = "Widget Item";
		base.Load += new System.EventHandler(this.frm_WidgetAddItem_Load);
		((System.ComponentModel.ISupportInitialize)this.grd_Main).EndInit();
		((System.ComponentModel.ISupportInitialize)this.titleView_Main).EndInit();
		((System.ComponentModel.ISupportInitialize)this.imageCollection1).EndInit();
		base.ResumeLayout(false);
	}
}
