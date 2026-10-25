using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Forms.Commons;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.TableLayout;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Tile;

namespace Apzon.Forms.DMS;

public class frm_Outlet : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private IContainer components;

	private SplitContainerControl splitContainerControl1;

	private SimpleButton btn_OK;

	private GridControlEx grd_List;

	private TileView tileView_List;

	private TileViewColumn col_Image;

	private TileViewColumn col_CardCode;

	private TileViewColumn col_CardName;

	private TileViewColumn col_Address;

	private ButtonEditEx btn_Search;

	private LabelLine labelLine1;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
		{
			WinFuntions.DisableAllControl(Controls);
			return;
		}
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Update:
			btn_OK.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btn_OK.Text = Resources.ButtonOK;
			break;
		}
	}

	public frm_Outlet()
	{
		InitializeComponent();
	}

	private async Task frm_Outlet_Load(object sender, EventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			tileView_List.BeginUpdate();
			DataTable dataTable = await _httpClient.Outlet.GetList();
			if (dataTable != null)
			{
				dataTable.BeginInit();
				dataTable.Columns.Add("Image", typeof(Image));
				dataTable.EndInit();
				grd_List.DataSource = dataTable;
				grd_List.RefreshDataSource();
				if (dataTable.IsNotNull())
				{
					for (int i = 0; i < dataTable.Rows.Count; i++)
					{
						try
						{
							string text = Function.ToString(dataTable.Rows[i]["ImageBase64"]);
							if (!string.IsNullOrEmpty(text))
							{
								byte[] array = Convert.FromBase64String(text);
								Image value;
								using (MemoryStream memoryStream = new MemoryStream(array, 0, array.Length))
								{
									memoryStream.Write(array, 0, array.Length);
									value = Image.FromStream(memoryStream);
								}
								dataTable.Rows[i]["Image"] = value;
							}
						}
						catch (Exception ex)
						{
							Logging.Write("ERROR", ex);
						}
					}
				}
			}
			WinCommons._CloseProcessing();
		}
		catch (Exception value2)
		{
			Console.WriteLine(value2);
		}
		finally
		{
			tileView_List.EndUpdate();
		}
	}

	private void btn_OK_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void btn_Search_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			DataTable dataSourceTable = grd_List.DataSourceTable;
			string text = "";
			foreach (GridColumn column in tileView_List.Columns)
			{
				if (column.Visible && dataSourceTable.ExistsColumn(column.FieldName))
				{
					text += $"OR {column.FieldName} LIKE '%{btn_Search.Text}%' ";
				}
			}
			text = ((text.Length > 2) ? text.Substring(2) : "");
			tileView_List.ActiveFilterString = text;
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition2 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableRowDefinition tableRowDefinition = new DevExpress.XtraEditors.TableLayout.TableRowDefinition();
		DevExpress.XtraEditors.TableLayout.TableRowDefinition tableRowDefinition2 = new DevExpress.XtraEditors.TableLayout.TableRowDefinition();
		DevExpress.XtraEditors.TableLayout.TableRowDefinition tableRowDefinition3 = new DevExpress.XtraEditors.TableLayout.TableRowDefinition();
		DevExpress.XtraEditors.TableLayout.TableSpan tableSpan = new DevExpress.XtraEditors.TableLayout.TableSpan();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement2 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement3 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement4 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.DMS.frm_Outlet));
		this.col_Image = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.col_CardName = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.col_CardCode = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.col_Address = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.btn_Search = new Apzon.Controls.Editor.ButtonEditEx();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.grd_List = new Apzon.Controls.Grid.GridControlEx();
		this.tileView_List = new DevExpress.XtraGrid.Views.Tile.TileView();
		this.btn_OK = new DevExpress.XtraEditors.SimpleButton();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.btn_Search.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_List).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tileView_List).BeginInit();
		base.SuspendLayout();
		this.col_Image.Caption = "Image";
		this.col_Image.FieldName = "Image";
		this.col_Image.Name = "col_Image";
		this.col_Image.Visible = true;
		this.col_Image.VisibleIndex = 0;
		this.col_CardName.Caption = "CardName";
		this.col_CardName.FieldName = "CardName";
		this.col_CardName.Name = "col_CardName";
		this.col_CardName.Visible = true;
		this.col_CardName.VisibleIndex = 2;
		this.col_CardCode.Caption = "CardCode";
		this.col_CardCode.FieldName = "CardCode";
		this.col_CardCode.Name = "col_CardCode";
		this.col_CardCode.Visible = true;
		this.col_CardCode.VisibleIndex = 1;
		this.col_Address.Caption = "Address";
		this.col_Address.FieldName = "Address";
		this.col_Address.Name = "col_Address";
		this.col_Address.Visible = true;
		this.col_Address.VisibleIndex = 3;
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Location = new System.Drawing.Point(2, 0);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.btn_Search);
		this.splitContainerControl1.Panel1.Controls.Add(this.labelLine1);
		this.splitContainerControl1.Panel1.Controls.Add(this.grd_List);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(984, 672);
		this.splitContainerControl1.SplitterPosition = 457;
		this.splitContainerControl1.TabIndex = 0;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.btn_Search.FieldName = null;
		this.btn_Search.IsSearchControl = false;
		this.btn_Search.Location = new System.Drawing.Point(89, 8);
		this.btn_Search.Name = "btn_Search";
		this.btn_Search.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, true, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_Search.Size = new System.Drawing.Size(368, 20);
		this.btn_Search.TabIndex = 3;
		this.btn_Search.EditValueChanged += new System.EventHandler(this.btn_Search_EditValueChanged);
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(10, 11);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(80, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 2;
		this.labelLine1.Utext = "Search";
		this.grd_List.AllowPasteExcel = false;
		this.grd_List.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_List.ClipboardData = null;
		this.grd_List.Location = new System.Drawing.Point(0, 34);
		this.grd_List.MainView = this.tileView_List;
		this.grd_List.Name = "grd_List";
		this.grd_List.Size = new System.Drawing.Size(457, 638);
		this.grd_List.TabIndex = 0;
		this.grd_List.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.tileView_List });
		this.tileView_List.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[4] { this.col_Image, this.col_CardCode, this.col_CardName, this.col_Address });
		this.tileView_List.GridControl = this.grd_List;
		this.tileView_List.Name = "tileView_List";
		this.tileView_List.OptionsTiles.IndentBetweenGroups = 0;
		this.tileView_List.OptionsTiles.IndentBetweenItems = 0;
		this.tileView_List.OptionsTiles.ItemSize = new System.Drawing.Size(480, 100);
		this.tileView_List.OptionsTiles.LayoutMode = DevExpress.XtraGrid.Views.Tile.TileViewLayoutMode.List;
		this.tileView_List.OptionsTiles.Orientation = System.Windows.Forms.Orientation.Vertical;
		this.tileView_List.OptionsTiles.Padding = new System.Windows.Forms.Padding(0);
		this.tileView_List.OptionsTiles.RowCount = 0;
		tableColumnDefinition.Length.Value = 135.0;
		tableColumnDefinition2.Length.Value = 685.0;
		this.tileView_List.TileColumns.Add(tableColumnDefinition);
		this.tileView_List.TileColumns.Add(tableColumnDefinition2);
		tableRowDefinition.Length.Value = 22.0;
		tableRowDefinition2.Length.Value = 22.0;
		tableRowDefinition3.Length.Value = 40.0;
		this.tileView_List.TileRows.Add(tableRowDefinition);
		this.tileView_List.TileRows.Add(tableRowDefinition2);
		this.tileView_List.TileRows.Add(tableRowDefinition3);
		tableSpan.RowSpan = 3;
		this.tileView_List.TileSpans.Add(tableSpan);
		tileViewItemElement.Column = this.col_Image;
		tileViewItemElement.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement.ImageOptions.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement.Text = "col_Image";
		tileViewItemElement.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement2.Column = this.col_CardName;
		tileViewItemElement2.ColumnIndex = 1;
		tileViewItemElement2.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement2.ImageOptions.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement2.Text = "col_CardName";
		tileViewItemElement2.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleLeft;
		tileViewItemElement2.TextLocation = new System.Drawing.Point(15, 0);
		tileViewItemElement3.Column = this.col_CardCode;
		tileViewItemElement3.ColumnIndex = 1;
		tileViewItemElement3.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement3.ImageOptions.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement3.RowIndex = 1;
		tileViewItemElement3.Text = "col_CardCode";
		tileViewItemElement3.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleLeft;
		tileViewItemElement3.TextLocation = new System.Drawing.Point(15, 0);
		tileViewItemElement4.Column = this.col_Address;
		tileViewItemElement4.ColumnIndex = 1;
		tileViewItemElement4.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement4.ImageOptions.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement4.RowIndex = 2;
		tileViewItemElement4.Text = "col_Address";
		tileViewItemElement4.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleLeft;
		tileViewItemElement4.TextLocation = new System.Drawing.Point(15, 0);
		this.tileView_List.TileTemplate.Add(tileViewItemElement);
		this.tileView_List.TileTemplate.Add(tileViewItemElement2);
		this.tileView_List.TileTemplate.Add(tileViewItemElement3);
		this.tileView_List.TileTemplate.Add(tileViewItemElement4);
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_OK.ImageOptions.Image");
		this.btn_OK.Location = new System.Drawing.Point(12, 680);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.Size = new System.Drawing.Size(75, 23);
		this.btn_OK.TabIndex = 1;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(987, 711);
		base.Controls.Add(this.btn_OK);
		base.Controls.Add(this.splitContainerControl1);
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_Outlet.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_Outlet";
		base.ShowInTaskbar = false;
		this.Text = "Outlet";
		base.OnLoadAsync += new Apzon.Controls.Form.LoadAsyncEventHandler(this.frm_Outlet_Load);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.btn_Search.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_List).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tileView_List).EndInit();
		base.ResumeLayout(false);
	}
}
