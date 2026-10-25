using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;

namespace Apzon.Forms.UserControls.POS_UI;

public class ucMainCmd : XtraUserControl
{
	private IContainer components;

	public TileControl tile_Commands;

	public TileGroup groupMain;

	public TileItem tileItem_Payment;

	public TileItem tileItem_Refund;

	public TileItem tileItem_CustomerSearch;

	public TileItem tileItem_Cancel;

	public TileItem tileItem_Hold;

	public TileItem tileItem_RemoveItem;

	public TileItem tileItem_ScanCustomer;

	public TileItem tileItem_ItemSearch;

	public TileItem tileItem_Recall;

	public TileItem tileItem_EditLine;

	public TileItem tileItem_OpenDrawer;

	public TileItem tileItem_ShutDown;

	public TileItem tileItem_EditOrder;

	public TileControl tile_Menu;

	public TileGroup menuGroup;

	public TileItem tileItem_groupMain;

	public TileItem tileItem_groupOthers;

	public TileItem tileItem_groupOperation;

	private TableLayoutPanel tableLayoutPanel1;

	private TileGroup groupOthers;

	private TileGroup groupOperation;

	private TileItem tileItem_Exit;

	private TileItem tileItem_CashIn;

	private TileItem tileItem_CashOut;

	private TileItem tileItem_SignOut;

	private TileItem tileItem_ReprintBill;

	private TileItem tileItem_Promotions;

	public ucMainCmd()
	{
		InitializeComponent();
	}

	private void ucMainCmd_Load(object sender, EventArgs e)
	{
		try
		{
			for (int i = 0; i < menuGroup.Items.Count; i++)
			{
				menuGroup.Items[i].ItemClick += ItemOnItemClick;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ItemOnItemClick(object sender, TileItemEventArgs e)
	{
		try
		{
			for (int i = 0; i < menuGroup.Items.Count; i++)
			{
				TileItem tileItem = menuGroup.Items[i];
				tileItem.Checked = tileItem == e.Item;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
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
		DevExpress.XtraEditors.TileItemElement tileItemElement = new DevExpress.XtraEditors.TileItemElement();
		DevExpress.XtraEditors.TileItemElement tileItemElement2 = new DevExpress.XtraEditors.TileItemElement();
		DevExpress.XtraEditors.TileItemElement tileItemElement3 = new DevExpress.XtraEditors.TileItemElement();
		DevExpress.XtraEditors.TileItemElement tileItemElement4 = new DevExpress.XtraEditors.TileItemElement();
		DevExpress.XtraEditors.TileItemElement tileItemElement5 = new DevExpress.XtraEditors.TileItemElement();
		DevExpress.XtraEditors.TileItemElement tileItemElement6 = new DevExpress.XtraEditors.TileItemElement();
		DevExpress.XtraEditors.TileItemElement tileItemElement7 = new DevExpress.XtraEditors.TileItemElement();
		DevExpress.XtraEditors.TileItemElement tileItemElement8 = new DevExpress.XtraEditors.TileItemElement();
		DevExpress.XtraEditors.TileItemElement tileItemElement9 = new DevExpress.XtraEditors.TileItemElement();
		DevExpress.XtraEditors.TileItemElement tileItemElement10 = new DevExpress.XtraEditors.TileItemElement();
		DevExpress.XtraEditors.TileItemElement tileItemElement11 = new DevExpress.XtraEditors.TileItemElement();
		DevExpress.XtraEditors.TileItemElement tileItemElement12 = new DevExpress.XtraEditors.TileItemElement();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.UserControls.POS_UI.ucMainCmd));
		DevExpress.XtraEditors.TileItemElement tileItemElement13 = new DevExpress.XtraEditors.TileItemElement();
		DevExpress.XtraEditors.TileItemElement tileItemElement14 = new DevExpress.XtraEditors.TileItemElement();
		DevExpress.XtraEditors.TileItemElement tileItemElement15 = new DevExpress.XtraEditors.TileItemElement();
		DevExpress.XtraEditors.TileItemElement tileItemElement16 = new DevExpress.XtraEditors.TileItemElement();
		DevExpress.XtraEditors.TileItemElement tileItemElement17 = new DevExpress.XtraEditors.TileItemElement();
		DevExpress.XtraEditors.TileItemElement tileItemElement18 = new DevExpress.XtraEditors.TileItemElement();
		DevExpress.XtraEditors.TileItemElement tileItemElement19 = new DevExpress.XtraEditors.TileItemElement();
		DevExpress.XtraEditors.TileItemElement tileItemElement20 = new DevExpress.XtraEditors.TileItemElement();
		DevExpress.XtraEditors.TileItemElement tileItemElement21 = new DevExpress.XtraEditors.TileItemElement();
		new DevExpress.XtraEditors.TileItemElement();
		DevExpress.XtraEditors.TileItemElement tileItemElement22 = new DevExpress.XtraEditors.TileItemElement();
		this.tile_Commands = new DevExpress.XtraEditors.TileControl();
		this.groupMain = new DevExpress.XtraEditors.TileGroup();
		this.tileItem_ItemSearch = new DevExpress.XtraEditors.TileItem();
		this.tileItem_CustomerSearch = new DevExpress.XtraEditors.TileItem();
		this.tileItem_ScanCustomer = new DevExpress.XtraEditors.TileItem();
		this.tileItem_EditLine = new DevExpress.XtraEditors.TileItem();
		this.tileItem_RemoveItem = new DevExpress.XtraEditors.TileItem();
		this.tileItem_Cancel = new DevExpress.XtraEditors.TileItem();
		this.tileItem_EditOrder = new DevExpress.XtraEditors.TileItem();
		this.tileItem_Payment = new DevExpress.XtraEditors.TileItem();
		this.groupOthers = new DevExpress.XtraEditors.TileGroup();
		this.tileItem_Hold = new DevExpress.XtraEditors.TileItem();
		this.tileItem_Recall = new DevExpress.XtraEditors.TileItem();
		this.tileItem_Refund = new DevExpress.XtraEditors.TileItem();
		this.tileItem_ReprintBill = new DevExpress.XtraEditors.TileItem();
		this.tileItem_CashIn = new DevExpress.XtraEditors.TileItem();
		this.tileItem_CashOut = new DevExpress.XtraEditors.TileItem();
		this.groupOperation = new DevExpress.XtraEditors.TileGroup();
		this.tileItem_OpenDrawer = new DevExpress.XtraEditors.TileItem();
		this.tileItem_Exit = new DevExpress.XtraEditors.TileItem();
		this.tileItem_ShutDown = new DevExpress.XtraEditors.TileItem();
		this.tileItem_SignOut = new DevExpress.XtraEditors.TileItem();
		this.tile_Menu = new DevExpress.XtraEditors.TileControl();
		this.menuGroup = new DevExpress.XtraEditors.TileGroup();
		this.tileItem_groupMain = new DevExpress.XtraEditors.TileItem();
		this.tileItem_groupOthers = new DevExpress.XtraEditors.TileItem();
		this.tileItem_groupOperation = new DevExpress.XtraEditors.TileItem();
		this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
		this.tileItem_Promotions = new DevExpress.XtraEditors.TileItem();
		this.tableLayoutPanel1.SuspendLayout();
		base.SuspendLayout();
		this.tile_Commands.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
		this.tile_Commands.ColumnCount = 4;
		this.tile_Commands.Cursor = System.Windows.Forms.Cursors.Default;
		this.tile_Commands.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tile_Commands.Groups.Add(this.groupMain);
		this.tile_Commands.Groups.Add(this.groupOthers);
		this.tile_Commands.Groups.Add(this.groupOperation);
		this.tile_Commands.IndentBetweenGroups = 0;
		this.tile_Commands.IndentBetweenItems = 5;
		this.tile_Commands.ItemPadding = new System.Windows.Forms.Padding(0);
		this.tile_Commands.LayoutMode = DevExpress.XtraEditors.TileControlLayoutMode.Adaptive;
		this.tile_Commands.Location = new System.Drawing.Point(0, 0);
		this.tile_Commands.Margin = new System.Windows.Forms.Padding(0);
		this.tile_Commands.MaxId = 37;
		this.tile_Commands.Name = "tile_Commands";
		this.tile_Commands.OptionsAdaptiveLayout.ItemMinSize = new System.Drawing.Size(20, 20);
		this.tile_Commands.Orientation = System.Windows.Forms.Orientation.Vertical;
		this.tile_Commands.Padding = new System.Windows.Forms.Padding(0);
		this.tile_Commands.RowCount = 3;
		this.tile_Commands.ScrollMode = DevExpress.XtraEditors.TileControlScrollMode.TouchScrollBar;
		this.tile_Commands.Size = new System.Drawing.Size(851, 365);
		this.tile_Commands.TabIndex = 2;
		this.tile_Commands.Text = "tileControl1";
		this.groupMain.Items.Add(this.tileItem_ItemSearch);
		this.groupMain.Items.Add(this.tileItem_CustomerSearch);
		this.groupMain.Items.Add(this.tileItem_ScanCustomer);
		this.groupMain.Items.Add(this.tileItem_EditLine);
		this.groupMain.Items.Add(this.tileItem_RemoveItem);
		this.groupMain.Items.Add(this.tileItem_Cancel);
		this.groupMain.Items.Add(this.tileItem_EditOrder);
		this.groupMain.Items.Add(this.tileItem_Payment);
		this.groupMain.Items.Add(this.tileItem_Promotions);
		this.groupMain.Name = "groupMain";
		this.groupMain.Tag = "1";
		this.tileItem_ItemSearch.AppearanceItem.Normal.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
		this.tileItem_ItemSearch.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 11.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.tileItem_ItemSearch.AppearanceItem.Normal.ForeColor = System.Drawing.Color.FromArgb(0, 64, 64);
		this.tileItem_ItemSearch.AppearanceItem.Normal.Options.UseBackColor = true;
		this.tileItem_ItemSearch.AppearanceItem.Normal.Options.UseFont = true;
		this.tileItem_ItemSearch.AppearanceItem.Normal.Options.UseForeColor = true;
		this.tileItem_ItemSearch.BorderVisibility = DevExpress.XtraEditors.TileItemBorderVisibility.Never;
		tileItemElement.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileItemElement.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Right;
		tileItemElement.Text = "Tìm mặt hàng";
		this.tileItem_ItemSearch.Elements.Add(tileItemElement);
		this.tileItem_ItemSearch.Id = 22;
		this.tileItem_ItemSearch.ItemSize = DevExpress.XtraEditors.TileItemSize.Small;
		this.tileItem_ItemSearch.Name = "tileItem_ItemSearch";
		this.tileItem_ItemSearch.Tag = "F7";
		this.tileItem_CustomerSearch.AppearanceItem.Hovered.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem_CustomerSearch.AppearanceItem.Hovered.Options.UseFont = true;
		this.tileItem_CustomerSearch.AppearanceItem.Normal.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
		this.tileItem_CustomerSearch.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 11.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.tileItem_CustomerSearch.AppearanceItem.Normal.ForeColor = System.Drawing.Color.FromArgb(0, 64, 64);
		this.tileItem_CustomerSearch.AppearanceItem.Normal.Options.UseBackColor = true;
		this.tileItem_CustomerSearch.AppearanceItem.Normal.Options.UseFont = true;
		this.tileItem_CustomerSearch.AppearanceItem.Normal.Options.UseForeColor = true;
		this.tileItem_CustomerSearch.AppearanceItem.Pressed.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem_CustomerSearch.AppearanceItem.Pressed.Options.UseFont = true;
		this.tileItem_CustomerSearch.AppearanceItem.Selected.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem_CustomerSearch.AppearanceItem.Selected.Options.UseFont = true;
		this.tileItem_CustomerSearch.BorderVisibility = DevExpress.XtraEditors.TileItemBorderVisibility.Never;
		tileItemElement2.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileItemElement2.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Right;
		tileItemElement2.Text = "Tìm Thành Viên";
		this.tileItem_CustomerSearch.Elements.Add(tileItemElement2);
		this.tileItem_CustomerSearch.Elements.Add(tileItemElement2);
		this.tileItem_CustomerSearch.Id = 6;
		this.tileItem_CustomerSearch.ItemSize = DevExpress.XtraEditors.TileItemSize.Small;
		this.tileItem_CustomerSearch.Name = "tileItem_CustomerSearch";
		this.tileItem_CustomerSearch.Tag = "F6";
		this.tileItem_ScanCustomer.AppearanceItem.Normal.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
		this.tileItem_ScanCustomer.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 11.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.tileItem_ScanCustomer.AppearanceItem.Normal.ForeColor = System.Drawing.Color.FromArgb(0, 64, 64);
		this.tileItem_ScanCustomer.AppearanceItem.Normal.Options.UseBackColor = true;
		this.tileItem_ScanCustomer.AppearanceItem.Normal.Options.UseFont = true;
		this.tileItem_ScanCustomer.AppearanceItem.Normal.Options.UseForeColor = true;
		this.tileItem_ScanCustomer.BorderVisibility = DevExpress.XtraEditors.TileItemBorderVisibility.Never;
		tileItemElement3.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileItemElement3.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Right;
		tileItemElement3.Text = "Quét mã thành viên";
		this.tileItem_ScanCustomer.Elements.Add(tileItemElement3);
		this.tileItem_ScanCustomer.Id = 21;
		this.tileItem_ScanCustomer.ItemSize = DevExpress.XtraEditors.TileItemSize.Small;
		this.tileItem_ScanCustomer.Name = "tileItem_ScanCustomer";
		this.tileItem_ScanCustomer.Tag = "F9";
		this.tileItem_EditLine.AppearanceItem.Normal.BackColor = System.Drawing.Color.FromArgb(255, 192, 128);
		this.tileItem_EditLine.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 11.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.tileItem_EditLine.AppearanceItem.Normal.ForeColor = System.Drawing.Color.Black;
		this.tileItem_EditLine.AppearanceItem.Normal.Options.UseBackColor = true;
		this.tileItem_EditLine.AppearanceItem.Normal.Options.UseFont = true;
		this.tileItem_EditLine.AppearanceItem.Normal.Options.UseForeColor = true;
		tileItemElement4.Text = "Sửa dòng";
		tileItemElement4.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		this.tileItem_EditLine.Elements.Add(tileItemElement4);
		this.tileItem_EditLine.Id = 25;
		this.tileItem_EditLine.ItemSize = DevExpress.XtraEditors.TileItemSize.Small;
		this.tileItem_EditLine.Name = "tileItem_EditLine";
		this.tileItem_EditLine.Tag = "F1";
		this.tileItem_RemoveItem.AppearanceItem.Hovered.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem_RemoveItem.AppearanceItem.Hovered.Options.UseFont = true;
		this.tileItem_RemoveItem.AppearanceItem.Normal.BackColor = System.Drawing.Color.FromArgb(255, 192, 192);
		this.tileItem_RemoveItem.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 11.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.tileItem_RemoveItem.AppearanceItem.Normal.ForeColor = System.Drawing.Color.FromArgb(0, 64, 64);
		this.tileItem_RemoveItem.AppearanceItem.Normal.Options.UseBackColor = true;
		this.tileItem_RemoveItem.AppearanceItem.Normal.Options.UseFont = true;
		this.tileItem_RemoveItem.AppearanceItem.Normal.Options.UseForeColor = true;
		this.tileItem_RemoveItem.AppearanceItem.Pressed.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem_RemoveItem.AppearanceItem.Pressed.Options.UseFont = true;
		this.tileItem_RemoveItem.AppearanceItem.Selected.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem_RemoveItem.AppearanceItem.Selected.Options.UseFont = true;
		this.tileItem_RemoveItem.BorderVisibility = DevExpress.XtraEditors.TileItemBorderVisibility.Never;
		tileItemElement5.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileItemElement5.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Right;
		tileItemElement5.Text = "Hủy dòng";
		this.tileItem_RemoveItem.Elements.Add(tileItemElement5);
		this.tileItem_RemoveItem.Id = 15;
		this.tileItem_RemoveItem.ItemSize = DevExpress.XtraEditors.TileItemSize.Small;
		this.tileItem_RemoveItem.Name = "tileItem_RemoveItem";
		this.tileItem_RemoveItem.Tag = "F10";
		this.tileItem_Cancel.AppearanceItem.Hovered.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem_Cancel.AppearanceItem.Hovered.Options.UseFont = true;
		this.tileItem_Cancel.AppearanceItem.Normal.BackColor = System.Drawing.Color.FromArgb(255, 192, 192);
		this.tileItem_Cancel.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 11.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.tileItem_Cancel.AppearanceItem.Normal.ForeColor = System.Drawing.Color.FromArgb(0, 64, 64);
		this.tileItem_Cancel.AppearanceItem.Normal.Options.UseBackColor = true;
		this.tileItem_Cancel.AppearanceItem.Normal.Options.UseFont = true;
		this.tileItem_Cancel.AppearanceItem.Normal.Options.UseForeColor = true;
		this.tileItem_Cancel.AppearanceItem.Pressed.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem_Cancel.AppearanceItem.Pressed.Options.UseFont = true;
		this.tileItem_Cancel.AppearanceItem.Selected.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem_Cancel.AppearanceItem.Selected.Options.UseFont = true;
		this.tileItem_Cancel.BorderVisibility = DevExpress.XtraEditors.TileItemBorderVisibility.Never;
		tileItemElement6.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileItemElement6.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Right;
		tileItemElement6.Text = "Hủy phiếu";
		this.tileItem_Cancel.Elements.Add(tileItemElement6);
		this.tileItem_Cancel.Id = 9;
		this.tileItem_Cancel.ItemSize = DevExpress.XtraEditors.TileItemSize.Small;
		this.tileItem_Cancel.Name = "tileItem_Cancel";
		this.tileItem_Cancel.Tag = "F8";
		this.tileItem_EditOrder.AppearanceItem.Normal.BackColor = System.Drawing.Color.FromArgb(255, 192, 128);
		this.tileItem_EditOrder.AppearanceItem.Normal.ForeColor = System.Drawing.Color.Black;
		this.tileItem_EditOrder.AppearanceItem.Normal.Options.UseBackColor = true;
		this.tileItem_EditOrder.AppearanceItem.Normal.Options.UseForeColor = true;
		tileItemElement7.Appearance.Normal.Font = new System.Drawing.Font("Tahoma", 11.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		tileItemElement7.Appearance.Normal.Options.UseFont = true;
		tileItemElement7.Text = "Điêu chỉnh đơn hàng";
		tileItemElement7.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		this.tileItem_EditOrder.Elements.Add(tileItemElement7);
		this.tileItem_EditOrder.Id = 28;
		this.tileItem_EditOrder.ItemSize = DevExpress.XtraEditors.TileItemSize.Small;
		this.tileItem_EditOrder.Name = "tileItem_EditOrder";
		this.tileItem_EditOrder.Tag = "F3";
		this.tileItem_Payment.AppearanceItem.Normal.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
		this.tileItem_Payment.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 11.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.tileItem_Payment.AppearanceItem.Normal.ForeColor = System.Drawing.Color.Teal;
		this.tileItem_Payment.AppearanceItem.Normal.Options.UseBackColor = true;
		this.tileItem_Payment.AppearanceItem.Normal.Options.UseFont = true;
		this.tileItem_Payment.AppearanceItem.Normal.Options.UseForeColor = true;
		this.tileItem_Payment.BorderVisibility = DevExpress.XtraEditors.TileItemBorderVisibility.Never;
		tileItemElement8.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileItemElement8.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Right;
		tileItemElement8.Text = "Thanh toán";
		this.tileItem_Payment.Elements.Add(tileItemElement8);
		this.tileItem_Payment.Id = 5;
		this.tileItem_Payment.ItemSize = DevExpress.XtraEditors.TileItemSize.Small;
		this.tileItem_Payment.Name = "tileItem_Payment";
		this.tileItem_Payment.Tag = "F4";
		this.groupOthers.Items.Add(this.tileItem_Hold);
		this.groupOthers.Items.Add(this.tileItem_Recall);
		this.groupOthers.Items.Add(this.tileItem_Refund);
		this.groupOthers.Items.Add(this.tileItem_ReprintBill);
		this.groupOthers.Items.Add(this.tileItem_CashIn);
		this.groupOthers.Items.Add(this.tileItem_CashOut);
		this.groupOthers.Name = "groupOthers";
		this.groupOthers.Tag = "2";
		this.tileItem_Hold.AppearanceItem.Hovered.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem_Hold.AppearanceItem.Hovered.Options.UseFont = true;
		this.tileItem_Hold.AppearanceItem.Normal.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
		this.tileItem_Hold.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 11.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.tileItem_Hold.AppearanceItem.Normal.ForeColor = System.Drawing.Color.FromArgb(0, 64, 64);
		this.tileItem_Hold.AppearanceItem.Normal.Options.UseBackColor = true;
		this.tileItem_Hold.AppearanceItem.Normal.Options.UseFont = true;
		this.tileItem_Hold.AppearanceItem.Normal.Options.UseForeColor = true;
		this.tileItem_Hold.AppearanceItem.Pressed.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem_Hold.AppearanceItem.Pressed.Options.UseFont = true;
		this.tileItem_Hold.AppearanceItem.Selected.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem_Hold.AppearanceItem.Selected.Options.UseFont = true;
		this.tileItem_Hold.BorderVisibility = DevExpress.XtraEditors.TileItemBorderVisibility.Never;
		tileItemElement9.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileItemElement9.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Right;
		tileItemElement9.Text = "Tạm dừng";
		this.tileItem_Hold.Elements.Add(tileItemElement9);
		this.tileItem_Hold.Id = 12;
		this.tileItem_Hold.ItemSize = DevExpress.XtraEditors.TileItemSize.Small;
		this.tileItem_Hold.Name = "tileItem_Hold";
		this.tileItem_Hold.Tag = "F2";
		this.tileItem_Recall.AppearanceItem.Normal.BackColor = System.Drawing.Color.FromArgb(128, 255, 128);
		this.tileItem_Recall.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 11.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.tileItem_Recall.AppearanceItem.Normal.ForeColor = System.Drawing.Color.FromArgb(0, 64, 64);
		this.tileItem_Recall.AppearanceItem.Normal.Options.UseBackColor = true;
		this.tileItem_Recall.AppearanceItem.Normal.Options.UseFont = true;
		this.tileItem_Recall.AppearanceItem.Normal.Options.UseForeColor = true;
		this.tileItem_Recall.BorderVisibility = DevExpress.XtraEditors.TileItemBorderVisibility.Never;
		tileItemElement10.Text = "Gọi lại";
		tileItemElement10.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		this.tileItem_Recall.Elements.Add(tileItemElement10);
		this.tileItem_Recall.Id = 24;
		this.tileItem_Recall.ItemSize = DevExpress.XtraEditors.TileItemSize.Small;
		this.tileItem_Recall.Name = "tileItem_Recall";
		this.tileItem_Refund.AppearanceItem.Hovered.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem_Refund.AppearanceItem.Hovered.Options.UseFont = true;
		this.tileItem_Refund.AppearanceItem.Normal.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
		this.tileItem_Refund.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 11.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.tileItem_Refund.AppearanceItem.Normal.ForeColor = System.Drawing.Color.Teal;
		this.tileItem_Refund.AppearanceItem.Normal.Options.UseBackColor = true;
		this.tileItem_Refund.AppearanceItem.Normal.Options.UseFont = true;
		this.tileItem_Refund.AppearanceItem.Normal.Options.UseForeColor = true;
		this.tileItem_Refund.AppearanceItem.Pressed.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem_Refund.AppearanceItem.Pressed.Options.UseFont = true;
		this.tileItem_Refund.AppearanceItem.Selected.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem_Refund.AppearanceItem.Selected.Options.UseFont = true;
		this.tileItem_Refund.BorderVisibility = DevExpress.XtraEditors.TileItemBorderVisibility.Never;
		tileItemElement11.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileItemElement11.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Right;
		tileItemElement11.Text = "Đổi hàng";
		this.tileItem_Refund.Elements.Add(tileItemElement11);
		this.tileItem_Refund.Id = 8;
		this.tileItem_Refund.ItemSize = DevExpress.XtraEditors.TileItemSize.Small;
		this.tileItem_Refund.Name = "tileItem_Refund";
		this.tileItem_Refund.Tag = "F5";
		this.tileItem_ReprintBill.AppearanceItem.Normal.BackColor = System.Drawing.Color.FromArgb(128, 255, 255);
		this.tileItem_ReprintBill.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 11.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.tileItem_ReprintBill.AppearanceItem.Normal.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.tileItem_ReprintBill.AppearanceItem.Normal.Options.UseBackColor = true;
		this.tileItem_ReprintBill.AppearanceItem.Normal.Options.UseFont = true;
		this.tileItem_ReprintBill.AppearanceItem.Normal.Options.UseForeColor = true;
		tileItemElement12.Image = (System.Drawing.Image)resources.GetObject("tileItemElement13.Image");
		tileItemElement12.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileItemElement12.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Left;
		tileItemElement12.Text = "In lại hóa đơn";
		tileItemElement12.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		this.tileItem_ReprintBill.Elements.Add(tileItemElement12);
		this.tileItem_ReprintBill.Id = 35;
		this.tileItem_ReprintBill.ItemSize = DevExpress.XtraEditors.TileItemSize.Small;
		this.tileItem_ReprintBill.Name = "tileItem_ReprintBill";
		this.tileItem_CashIn.AppearanceItem.Normal.BackColor = System.Drawing.Color.FromArgb(255, 255, 128);
		this.tileItem_CashIn.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 11.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.tileItem_CashIn.AppearanceItem.Normal.ForeColor = System.Drawing.Color.Teal;
		this.tileItem_CashIn.AppearanceItem.Normal.Options.UseBackColor = true;
		this.tileItem_CashIn.AppearanceItem.Normal.Options.UseFont = true;
		this.tileItem_CashIn.AppearanceItem.Normal.Options.UseForeColor = true;
		tileItemElement13.Text = "Thu tiền khác";
		tileItemElement13.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		this.tileItem_CashIn.Elements.Add(tileItemElement13);
		this.tileItem_CashIn.Id = 32;
		this.tileItem_CashIn.ItemSize = DevExpress.XtraEditors.TileItemSize.Small;
		this.tileItem_CashIn.Name = "tileItem_CashIn";
		this.tileItem_CashOut.AppearanceItem.Normal.BackColor = System.Drawing.Color.FromArgb(255, 255, 128);
		this.tileItem_CashOut.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 11.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.tileItem_CashOut.AppearanceItem.Normal.ForeColor = System.Drawing.Color.Teal;
		this.tileItem_CashOut.AppearanceItem.Normal.Options.UseBackColor = true;
		this.tileItem_CashOut.AppearanceItem.Normal.Options.UseFont = true;
		this.tileItem_CashOut.AppearanceItem.Normal.Options.UseForeColor = true;
		tileItemElement14.Text = "Chi tiền khác";
		tileItemElement14.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		this.tileItem_CashOut.Elements.Add(tileItemElement14);
		this.tileItem_CashOut.Id = 33;
		this.tileItem_CashOut.ItemSize = DevExpress.XtraEditors.TileItemSize.Small;
		this.tileItem_CashOut.Name = "tileItem_CashOut";
		this.groupOperation.Items.Add(this.tileItem_OpenDrawer);
		this.groupOperation.Items.Add(this.tileItem_Exit);
		this.groupOperation.Items.Add(this.tileItem_ShutDown);
		this.groupOperation.Items.Add(this.tileItem_SignOut);
		this.groupOperation.Name = "groupOperation";
		this.groupOperation.Tag = "3";
		this.tileItem_OpenDrawer.AppearanceItem.Normal.BackColor = System.Drawing.Color.FromArgb(255, 192, 128);
		this.tileItem_OpenDrawer.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 11.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.tileItem_OpenDrawer.AppearanceItem.Normal.ForeColor = System.Drawing.Color.Black;
		this.tileItem_OpenDrawer.AppearanceItem.Normal.Options.UseBackColor = true;
		this.tileItem_OpenDrawer.AppearanceItem.Normal.Options.UseFont = true;
		this.tileItem_OpenDrawer.AppearanceItem.Normal.Options.UseForeColor = true;
		tileItemElement15.Text = "Mở ngăn đựng tiền";
		tileItemElement15.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		this.tileItem_OpenDrawer.Elements.Add(tileItemElement15);
		this.tileItem_OpenDrawer.Id = 26;
		this.tileItem_OpenDrawer.ItemSize = DevExpress.XtraEditors.TileItemSize.Small;
		this.tileItem_OpenDrawer.Name = "tileItem_OpenDrawer";
		this.tileItem_Exit.AppearanceItem.Normal.BackColor = System.Drawing.Color.FromArgb(255, 255, 128);
		this.tileItem_Exit.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 11.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.tileItem_Exit.AppearanceItem.Normal.ForeColor = System.Drawing.Color.Teal;
		this.tileItem_Exit.AppearanceItem.Normal.Options.UseBackColor = true;
		this.tileItem_Exit.AppearanceItem.Normal.Options.UseFont = true;
		this.tileItem_Exit.AppearanceItem.Normal.Options.UseForeColor = true;
		tileItemElement16.Text = "Thoát hệ thống";
		tileItemElement16.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		this.tileItem_Exit.Elements.Add(tileItemElement16);
		this.tileItem_Exit.Id = 31;
		this.tileItem_Exit.ItemSize = DevExpress.XtraEditors.TileItemSize.Small;
		this.tileItem_Exit.Name = "tileItem_Exit";
		this.tileItem_ShutDown.AppearanceItem.Normal.BackColor = System.Drawing.Color.FromArgb(255, 192, 128);
		this.tileItem_ShutDown.AppearanceItem.Normal.ForeColor = System.Drawing.Color.Black;
		this.tileItem_ShutDown.AppearanceItem.Normal.Options.UseBackColor = true;
		this.tileItem_ShutDown.AppearanceItem.Normal.Options.UseForeColor = true;
		tileItemElement17.Appearance.Normal.Font = new System.Drawing.Font("Tahoma", 11.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		tileItemElement17.Appearance.Normal.Options.UseFont = true;
		tileItemElement17.Text = "Kết thúc bán hàng";
		tileItemElement17.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		this.tileItem_ShutDown.Elements.Add(tileItemElement17);
		this.tileItem_ShutDown.Id = 27;
		this.tileItem_ShutDown.ItemSize = DevExpress.XtraEditors.TileItemSize.Small;
		this.tileItem_ShutDown.Name = "tileItem_ShutDown";
		this.tileItem_SignOut.AppearanceItem.Normal.BackColor = System.Drawing.Color.FromArgb(128, 128, 255);
		this.tileItem_SignOut.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 11.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.tileItem_SignOut.AppearanceItem.Normal.ForeColor = System.Drawing.Color.White;
		this.tileItem_SignOut.AppearanceItem.Normal.Options.UseBackColor = true;
		this.tileItem_SignOut.AppearanceItem.Normal.Options.UseFont = true;
		this.tileItem_SignOut.AppearanceItem.Normal.Options.UseForeColor = true;
		tileItemElement18.Text = "Đăng xuất";
		tileItemElement18.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		this.tileItem_SignOut.Elements.Add(tileItemElement18);
		this.tileItem_SignOut.Id = 34;
		this.tileItem_SignOut.ItemSize = DevExpress.XtraEditors.TileItemSize.Small;
		this.tileItem_SignOut.Name = "tileItem_SignOut";
		this.tile_Menu.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
		this.tile_Menu.ColumnCount = 4;
		this.tile_Menu.Cursor = System.Windows.Forms.Cursors.Default;
		this.tile_Menu.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tile_Menu.Groups.Add(this.menuGroup);
		this.tile_Menu.IndentBetweenGroups = 0;
		this.tile_Menu.IndentBetweenItems = 5;
		this.tile_Menu.ItemPadding = new System.Windows.Forms.Padding(0);
		this.tile_Menu.LayoutMode = DevExpress.XtraEditors.TileControlLayoutMode.Adaptive;
		this.tile_Menu.Location = new System.Drawing.Point(0, 365);
		this.tile_Menu.Margin = new System.Windows.Forms.Padding(0);
		this.tile_Menu.MaxId = 30;
		this.tile_Menu.Name = "tile_Menu";
		this.tile_Menu.OptionsAdaptiveLayout.ItemMinSize = new System.Drawing.Size(60, 60);
		this.tile_Menu.Orientation = System.Windows.Forms.Orientation.Vertical;
		this.tile_Menu.Padding = new System.Windows.Forms.Padding(0);
		this.tile_Menu.RowCount = 3;
		this.tile_Menu.ScrollMode = DevExpress.XtraEditors.TileControlScrollMode.TouchScrollBar;
		this.tile_Menu.Size = new System.Drawing.Size(851, 65);
		this.tile_Menu.TabIndex = 3;
		this.tile_Menu.Text = "tileControl2";
		this.menuGroup.Items.Add(this.tileItem_groupMain);
		this.menuGroup.Items.Add(this.tileItem_groupOthers);
		this.menuGroup.Items.Add(this.tileItem_groupOperation);
		this.menuGroup.Name = "menuGroup";
		this.tileItem_groupMain.AppearanceItem.Normal.BackColor = System.Drawing.Color.Gray;
		this.tileItem_groupMain.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 11.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.tileItem_groupMain.AppearanceItem.Normal.Options.UseBackColor = true;
		this.tileItem_groupMain.AppearanceItem.Normal.Options.UseFont = true;
		tileItemElement19.Text = "Chính";
		tileItemElement19.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		this.tileItem_groupMain.Elements.Add(tileItemElement19);
		this.tileItem_groupMain.Id = 26;
		this.tileItem_groupMain.ItemSize = DevExpress.XtraEditors.TileItemSize.Small;
		this.tileItem_groupMain.Name = "tileItem_groupMain";
		this.tileItem_groupOthers.AppearanceItem.Normal.BackColor = System.Drawing.Color.Gray;
		this.tileItem_groupOthers.AppearanceItem.Normal.Options.UseBackColor = true;
		tileItemElement20.Appearance.Normal.Font = new System.Drawing.Font("Tahoma", 11.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		tileItemElement20.Appearance.Normal.Options.UseFont = true;
		tileItemElement20.Text = "Các nghiệp vụ khác";
		tileItemElement20.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		this.tileItem_groupOthers.Elements.Add(tileItemElement20);
		this.tileItem_groupOthers.Id = 27;
		this.tileItem_groupOthers.ItemSize = DevExpress.XtraEditors.TileItemSize.Small;
		this.tileItem_groupOthers.Name = "tileItem_groupOthers";
		this.tileItem_groupOperation.AppearanceItem.Normal.BackColor = System.Drawing.Color.Gray;
		this.tileItem_groupOperation.AppearanceItem.Normal.Options.UseBackColor = true;
		tileItemElement21.Appearance.Normal.Font = new System.Drawing.Font("Tahoma", 11.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		tileItemElement21.Appearance.Normal.Options.UseFont = true;
		tileItemElement21.Text = "Vận hành";
		tileItemElement21.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		this.tileItem_groupOperation.Elements.Add(tileItemElement21);
		this.tileItem_groupOperation.Id = 28;
		this.tileItem_groupOperation.ItemSize = DevExpress.XtraEditors.TileItemSize.Small;
		this.tileItem_groupOperation.Name = "tileItem_groupOperation";
		this.tableLayoutPanel1.ColumnCount = 1;
		this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100f));
		this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20f));
		this.tableLayoutPanel1.Controls.Add(this.tile_Menu, 0, 1);
		this.tableLayoutPanel1.Controls.Add(this.tile_Commands, 0, 0);
		this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
		this.tableLayoutPanel1.Name = "tableLayoutPanel1";
		this.tableLayoutPanel1.RowCount = 2;
		this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 85f));
		this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 15f));
		this.tableLayoutPanel1.Size = new System.Drawing.Size(851, 430);
		this.tableLayoutPanel1.TabIndex = 4;
		this.tileItem_Promotions.AppearanceItem.Normal.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
		this.tileItem_Promotions.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 11.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.tileItem_Promotions.AppearanceItem.Normal.ForeColor = System.Drawing.Color.Teal;
		this.tileItem_Promotions.AppearanceItem.Normal.Options.UseBackColor = true;
		this.tileItem_Promotions.AppearanceItem.Normal.Options.UseFont = true;
		this.tileItem_Promotions.AppearanceItem.Normal.Options.UseForeColor = true;
		this.tileItem_Promotions.BorderVisibility = DevExpress.XtraEditors.TileItemBorderVisibility.Never;
		tileItemElement22.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileItemElement22.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Right;
		tileItemElement22.Text = "Chương trình KM";
		this.tileItem_Promotions.Elements.Add(tileItemElement22);
		this.tileItem_Promotions.Id = 36;
		this.tileItem_Promotions.ItemSize = DevExpress.XtraEditors.TileItemSize.Small;
		this.tileItem_Promotions.Name = "tileItem_Promotions";
		this.tileItem_Promotions.Tag = "";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.tableLayoutPanel1);
		base.Name = "ucMainCmd";
		base.Size = new System.Drawing.Size(851, 430);
		base.Load += new System.EventHandler(this.ucMainCmd_Load);
		this.tableLayoutPanel1.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
