using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.DMS;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.XtraBars.Docking;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraMap;
using DevExpress.XtraVerticalGrid.Rows;
using Newtonsoft.Json;

namespace Apzon.Forms.DMS.Report;

public class frm_ReportSalesCoverage : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private Image selectedPushpin;

	private Image defaultPushpin;

	private MapPushpin currentPushpin;

	private IContainer components;

	private MapControl map;

	private VectorItemsLayer vectorItemsLayer1;

	private MapItemStorage mapItemStorage1;

	private DockManager dockManager1;

	private PictureBox pictureBox2;

	private Panel panel1;

	private SimpleButtonEx btn_ExpandInfoWindow;

	private SimpleButtonEx btn_CloseInfoWindow;

	private PictureBox pictureBox1;

	private VGridControlEx vGridControlEx1;

	private CategoryRow category;

	private EditorRow row;

	private EditorRow row1;

	private EditorRow row2;

	private EditorRow row3;

	private CategoryRow category1;

	private EditorRow row4;

	private EditorRow row5;

	private EditorRow row6;

	public frm_ReportSalesCoverage()
	{
		InitializeComponent();
	}

	private void frm_ReportSalesCoverage_Load(object sender, EventArgs e)
	{
		try
		{
			ImageLayer imageLayer = new ImageLayer();
			map.Layers.Add(imageLayer);
			BingMapDataProvider bingMapDataProvider = new BingMapDataProvider();
			bingMapDataProvider.BingKey = "KBocJH7Kv5av6y4qpdOv~yhdsUTSx_d_oFAbVQk8v2A~AnyRBjLGaxzZDqKgYY8DGWmvHdCluecHPprHh_BxT5CR0arcXoFFH4cF2U2Q2REd";
			bingMapDataProvider.Kind = BingMapKind.Road;
			imageLayer.DataProvider = bingMapDataProvider;
			map.Zoom(10.0);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void OnProviderWebRequest(object sender, MapWebRequestEventArgs e)
	{
		e.UserAgent = "apzon.com";
	}

	private async void frm_ReportSalesCoverage_Shown(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			List<MapMarker> markers = new List<MapMarker>();
			if (GlobalData.MapMarkers != null)
			{
				markers = GlobalData.MapMarkers;
			}
			else
			{
				DataTable dataTable = await _httpClient.DMSReport.GetMarkers();
				if (dataTable.IsNotNull())
				{
					markers = (from t in dataTable.AsEnumerable()
						select new MapMarker(t)).ToList();
					if (markers != null)
					{
						GlobalData.MapMarkers = markers;
					}
				}
			}
			MapMarker storeMarker = markers.FirstOrDefault((MapMarker t) => t.Code == "circle");
			defaultPushpin = storeMarker.ImageData;
			selectedPushpin = Function.ConvertSVGtoPNG(storeMarker.SvgData, 16, 16, "#EB7D16");
			HttpResult httpResult = await _httpClient.DMSReport.SalesCoverage();
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				WinCommons.Error(Enums.MessageCode.Error, httpResult.Message);
				return;
			}
			DataTable dataTable2 = JsonConvert.DeserializeObject<DataTable>(Function.ToString(httpResult.Content));
			if (dataTable2.IsNotNull())
			{
				for (int num = 0; num < dataTable2.Rows.Count; num++)
				{
					MapPushpin mapPushpin = new MapPushpin();
					mapPushpin.Image = storeMarker.ImageData;
					mapPushpin.Location = new GeoPoint(Function.ParseDouble(dataTable2.Rows[num]["U_Latitude"]), Function.ParseDouble(dataTable2.Rows[num]["U_Longitude"]));
					mapPushpin.Information = dataTable2.Rows[num];
					mapItemStorage1.Items.Add(mapPushpin);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void map_SelectionChanged(object sender, MapSelectionChangedEventArgs e)
	{
		try
		{
			if (e.Selection.Count != 1 || !(e.Selection[0] is MapPushpin))
			{
				return;
			}
			MapPushpin mapPushpin = (MapPushpin)e.Selection[0];
			if (mapPushpin != null)
			{
				mapPushpin.Image = selectedPushpin;
				currentPushpin = mapPushpin;
				if (!panel1.Visible)
				{
					panel1.Visible = true;
				}
				DataRow dataRow = (DataRow)mapPushpin.Information;
				string imageLocation = Function.ToString(dataRow["LinkDownload"]);
				pictureBox1.ImageLocation = imageLocation;
				vGridControlEx1.DataSource = new DataRow[1] { dataRow }.CopyToDataTable();
				vGridControlEx1.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void map_SelectionChanging(object sender, MapSelectionChangingEventArgs e)
	{
		try
		{
			if (currentPushpin != null)
			{
				currentPushpin.Image = defaultPushpin;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_CloseInfoWindow_Click(object sender, EventArgs e)
	{
		try
		{
			panel1.Visible = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.DMS.Report.frm_ReportSalesCoverage));
		this.map = new DevExpress.XtraMap.MapControl();
		this.vectorItemsLayer1 = new DevExpress.XtraMap.VectorItemsLayer();
		this.mapItemStorage1 = new DevExpress.XtraMap.MapItemStorage();
		this.dockManager1 = new DevExpress.XtraBars.Docking.DockManager(this.components);
		this.pictureBox2 = new System.Windows.Forms.PictureBox();
		this.panel1 = new System.Windows.Forms.Panel();
		this.vGridControlEx1 = new Apzon.Controls.Grid.VGridControlEx();
		this.category = new DevExpress.XtraVerticalGrid.Rows.CategoryRow();
		this.row = new DevExpress.XtraVerticalGrid.Rows.EditorRow();
		this.row1 = new DevExpress.XtraVerticalGrid.Rows.EditorRow();
		this.row2 = new DevExpress.XtraVerticalGrid.Rows.EditorRow();
		this.row3 = new DevExpress.XtraVerticalGrid.Rows.EditorRow();
		this.category1 = new DevExpress.XtraVerticalGrid.Rows.CategoryRow();
		this.row4 = new DevExpress.XtraVerticalGrid.Rows.EditorRow();
		this.row5 = new DevExpress.XtraVerticalGrid.Rows.EditorRow();
		this.row6 = new DevExpress.XtraVerticalGrid.Rows.EditorRow();
		this.pictureBox1 = new System.Windows.Forms.PictureBox();
		this.btn_ExpandInfoWindow = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_CloseInfoWindow = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.map).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dockManager1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.pictureBox2).BeginInit();
		this.panel1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.vGridControlEx1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.pictureBox1).BeginInit();
		base.SuspendLayout();
		this.map.CenterPoint = new DevExpress.XtraMap.GeoPoint(20.977512, 105.7836002);
		this.map.Dock = System.Windows.Forms.DockStyle.Fill;
		this.map.Layers.Add(this.vectorItemsLayer1);
		this.map.Location = new System.Drawing.Point(0, 0);
		this.map.Name = "map";
		this.map.Size = new System.Drawing.Size(1301, 748);
		this.map.TabIndex = 1;
		this.map.SelectionChanging += new DevExpress.XtraMap.MapSelectionChangingEventHandler(this.map_SelectionChanging);
		this.map.SelectionChanged += new DevExpress.XtraMap.MapSelectionChangedEventHandler(this.map_SelectionChanged);
		this.vectorItemsLayer1.Data = this.mapItemStorage1;
		this.dockManager1.Form = this;
		this.dockManager1.TopZIndexControls.AddRange(new string[10] { "DevExpress.XtraBars.BarDockControl", "DevExpress.XtraBars.StandaloneBarDockControl", "System.Windows.Forms.StatusBar", "System.Windows.Forms.MenuStrip", "System.Windows.Forms.StatusStrip", "DevExpress.XtraBars.Ribbon.RibbonStatusBar", "DevExpress.XtraBars.Ribbon.RibbonControl", "DevExpress.XtraBars.Navigation.OfficeNavigationBar", "DevExpress.XtraBars.Navigation.TileNavPane", "DevExpress.XtraBars.TabFormControl" });
		this.pictureBox2.Anchor = System.Windows.Forms.AnchorStyles.Top;
		this.pictureBox2.Location = new System.Drawing.Point(0, 28);
		this.pictureBox2.Name = "pictureBox2";
		this.pictureBox2.Size = new System.Drawing.Size(234, 178);
		this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		this.pictureBox2.TabIndex = 0;
		this.pictureBox2.TabStop = false;
		this.panel1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.panel1.Controls.Add(this.vGridControlEx1);
		this.panel1.Controls.Add(this.pictureBox1);
		this.panel1.Controls.Add(this.btn_ExpandInfoWindow);
		this.panel1.Controls.Add(this.btn_CloseInfoWindow);
		this.panel1.Location = new System.Drawing.Point(1028, 12);
		this.panel1.Name = "panel1";
		this.panel1.Size = new System.Drawing.Size(261, 503);
		this.panel1.TabIndex = 2;
		this.panel1.Visible = false;
		this.vGridControlEx1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.vGridControlEx1.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.vGridControlEx1.Appearance.FocusedRow.Options.UseBackColor = true;
		this.vGridControlEx1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.vGridControlEx1.Cursor = System.Windows.Forms.Cursors.SizeWE;
		this.vGridControlEx1.FormId = null;
		this.vGridControlEx1.ItemId = null;
		this.vGridControlEx1.Location = new System.Drawing.Point(0, 259);
		this.vGridControlEx1.Name = "vGridControlEx1";
		this.vGridControlEx1.ObjectType = Apzon.Commons.ApzObjectTypes.oNone;
		this.vGridControlEx1.RecordWidth = 150;
		this.vGridControlEx1.RowHeaderWidth = 108;
		this.vGridControlEx1.Rows.AddRange(new DevExpress.XtraVerticalGrid.Rows.BaseRow[9] { this.category, this.row, this.row1, this.row2, this.row3, this.category1, this.row4, this.row5, this.row6 });
		this.vGridControlEx1.Size = new System.Drawing.Size(261, 244);
		this.vGridControlEx1.TabIndex = 3;
		this.category.Height = 25;
		this.category.Name = "category";
		this.category.Properties.Caption = "Thông tin chung";
		this.row.Height = 25;
		this.row.Name = "row";
		this.row.Properties.Caption = "Mã cửa hàng";
		this.row.Properties.FieldName = "CardCode";
		this.row1.Height = 25;
		this.row1.Name = "row1";
		this.row1.Properties.Caption = "Tên cửa hàng";
		this.row1.Properties.FieldName = "CardName";
		this.row2.Height = 25;
		this.row2.Name = "row2";
		this.row2.Properties.Caption = "Địa chỉ";
		this.row2.Properties.FieldName = "Address";
		this.row3.Height = 25;
		this.row3.Name = "row3";
		this.row3.Properties.Caption = "Số điện thoại";
		this.row3.Properties.FieldName = "Cellular";
		this.category1.Height = 25;
		this.category1.Name = "category1";
		this.category1.Properties.Caption = "Thông tin chủ cửa hàng";
		this.row4.Height = 25;
		this.row4.Name = "row4";
		this.row4.Properties.Caption = "Tên chủ cửa hàng";
		this.row4.Properties.FieldName = "CntctPrsn";
		this.row5.Height = 25;
		this.row5.Name = "row5";
		this.row5.Properties.Caption = "Số điện thoại";
		this.row5.Properties.FieldName = "Tel1";
		this.row6.Height = 25;
		this.row6.Name = "row6";
		this.row6.Properties.Caption = "Ngày sinh";
		this.row6.Properties.FieldName = "BirthDate";
		this.pictureBox1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.pictureBox1.Location = new System.Drawing.Point(0, 30);
		this.pictureBox1.Name = "pictureBox1";
		this.pictureBox1.Size = new System.Drawing.Size(261, 228);
		this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		this.pictureBox1.TabIndex = 2;
		this.pictureBox1.TabStop = false;
		this.btn_ExpandInfoWindow.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
		this.btn_ExpandInfoWindow.FieldName = null;
		this.btn_ExpandInfoWindow.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_ExpandInfoWindow.ImageOptions.Image");
		this.btn_ExpandInfoWindow.IsSearchControl = false;
		this.btn_ExpandInfoWindow.Location = new System.Drawing.Point(206, 3);
		this.btn_ExpandInfoWindow.Name = "btn_ExpandInfoWindow";
		this.btn_ExpandInfoWindow.Size = new System.Drawing.Size(23, 23);
		this.btn_ExpandInfoWindow.TabIndex = 1;
		this.btn_ExpandInfoWindow.ToolTip = "Expand info window";
		this.btn_ExpandInfoWindow.Visible = false;
		this.btn_CloseInfoWindow.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
		this.btn_CloseInfoWindow.FieldName = null;
		this.btn_CloseInfoWindow.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_CloseInfoWindow.ImageOptions.Image");
		this.btn_CloseInfoWindow.IsSearchControl = false;
		this.btn_CloseInfoWindow.Location = new System.Drawing.Point(235, 3);
		this.btn_CloseInfoWindow.Name = "btn_CloseInfoWindow";
		this.btn_CloseInfoWindow.Size = new System.Drawing.Size(23, 23);
		this.btn_CloseInfoWindow.TabIndex = 0;
		this.btn_CloseInfoWindow.ToolTip = "Close info window";
		this.btn_CloseInfoWindow.Click += new System.EventHandler(this.btn_CloseInfoWindow_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1301, 748);
		base.Controls.Add(this.panel1);
		base.Controls.Add(this.map);
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_ReportSalesCoverage.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_ReportSalesCoverage";
		base.ShowInTaskbar = false;
		this.Text = "Báo cáo độ phủ";
		base.Load += new System.EventHandler(this.frm_ReportSalesCoverage_Load);
		base.Shown += new System.EventHandler(this.frm_ReportSalesCoverage_Shown);
		((System.ComponentModel.ISupportInitialize)this.map).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dockManager1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.pictureBox2).EndInit();
		this.panel1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.vGridControlEx1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.pictureBox1).EndInit();
		base.ResumeLayout(false);
	}
}
