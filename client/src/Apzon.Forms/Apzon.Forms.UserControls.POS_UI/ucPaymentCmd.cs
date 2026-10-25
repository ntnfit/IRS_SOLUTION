using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace Apzon.Forms.UserControls.POS_UI;

public class ucPaymentCmd : XtraUserControl
{
	private IContainer components;

	public TileControl tileControl1;

	public TileGroup tileGroup2;

	public TileItem tileItem_Cash;

	public TileItem tileItem_CreditCard;

	public TileItem tileItem_Points;

	public TileItem tileItem1;

	public TileItem tileItem4;

	public TileItem tileItem6;

	public TileItem tileItem8;

	public TileItem tileItem9;

	public ucPaymentCmd()
	{
		InitializeComponent();
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
		this.tileControl1 = new DevExpress.XtraEditors.TileControl();
		this.tileGroup2 = new DevExpress.XtraEditors.TileGroup();
		this.tileItem_Cash = new DevExpress.XtraEditors.TileItem();
		this.tileItem_CreditCard = new DevExpress.XtraEditors.TileItem();
		this.tileItem_Points = new DevExpress.XtraEditors.TileItem();
		this.tileItem1 = new DevExpress.XtraEditors.TileItem();
		this.tileItem4 = new DevExpress.XtraEditors.TileItem();
		this.tileItem6 = new DevExpress.XtraEditors.TileItem();
		this.tileItem8 = new DevExpress.XtraEditors.TileItem();
		this.tileItem9 = new DevExpress.XtraEditors.TileItem();
		base.SuspendLayout();
		this.tileControl1.ColumnCount = 3;
		this.tileControl1.Cursor = System.Windows.Forms.Cursors.Default;
		this.tileControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tileControl1.Groups.Add(this.tileGroup2);
		this.tileControl1.IndentBetweenGroups = 0;
		this.tileControl1.IndentBetweenItems = 5;
		this.tileControl1.ItemPadding = new System.Windows.Forms.Padding(0);
		this.tileControl1.LayoutMode = DevExpress.XtraEditors.TileControlLayoutMode.Adaptive;
		this.tileControl1.Location = new System.Drawing.Point(0, 0);
		this.tileControl1.Margin = new System.Windows.Forms.Padding(0);
		this.tileControl1.MaxId = 19;
		this.tileControl1.Name = "tileControl1";
		this.tileControl1.OptionsAdaptiveLayout.ItemMinSize = new System.Drawing.Size(20, 20);
		this.tileControl1.Orientation = System.Windows.Forms.Orientation.Vertical;
		this.tileControl1.Padding = new System.Windows.Forms.Padding(0);
		this.tileControl1.RowCount = 3;
		this.tileControl1.ScrollMode = DevExpress.XtraEditors.TileControlScrollMode.ScrollBar;
		this.tileControl1.Size = new System.Drawing.Size(600, 450);
		this.tileControl1.TabIndex = 14;
		this.tileControl1.Text = "tileControl1";
		this.tileGroup2.Items.Add(this.tileItem_Cash);
		this.tileGroup2.Items.Add(this.tileItem_CreditCard);
		this.tileGroup2.Items.Add(this.tileItem_Points);
		this.tileGroup2.Name = "tileGroup2";
		this.tileItem_Cash.AppearanceItem.Normal.BackColor = System.Drawing.Color.FromArgb(155, 187, 89);
		this.tileItem_Cash.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 11.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.tileItem_Cash.AppearanceItem.Normal.ForeColor = System.Drawing.SystemColors.WindowText;
		this.tileItem_Cash.AppearanceItem.Normal.Options.UseBackColor = true;
		this.tileItem_Cash.AppearanceItem.Normal.Options.UseFont = true;
		this.tileItem_Cash.AppearanceItem.Normal.Options.UseForeColor = true;
		this.tileItem_Cash.BorderVisibility = DevExpress.XtraEditors.TileItemBorderVisibility.Never;
		tileItemElement.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileItemElement.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Right;
		tileItemElement.Text = "Tiền mặt";
		this.tileItem_Cash.Elements.Add(tileItemElement);
		this.tileItem_Cash.Id = 5;
		this.tileItem_Cash.ItemSize = DevExpress.XtraEditors.TileItemSize.Small;
		this.tileItem_Cash.Name = "tileItem_Cash";
		this.tileItem_Cash.Tag = "F3";
		this.tileItem_CreditCard.AppearanceItem.Hovered.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem_CreditCard.AppearanceItem.Hovered.Options.UseFont = true;
		this.tileItem_CreditCard.AppearanceItem.Normal.BackColor = System.Drawing.Color.FromArgb(0, 176, 240);
		this.tileItem_CreditCard.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 11.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.tileItem_CreditCard.AppearanceItem.Normal.ForeColor = System.Drawing.SystemColors.WindowText;
		this.tileItem_CreditCard.AppearanceItem.Normal.Options.UseBackColor = true;
		this.tileItem_CreditCard.AppearanceItem.Normal.Options.UseFont = true;
		this.tileItem_CreditCard.AppearanceItem.Normal.Options.UseForeColor = true;
		this.tileItem_CreditCard.AppearanceItem.Pressed.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem_CreditCard.AppearanceItem.Pressed.Options.UseFont = true;
		this.tileItem_CreditCard.AppearanceItem.Selected.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem_CreditCard.AppearanceItem.Selected.Options.UseFont = true;
		this.tileItem_CreditCard.BorderVisibility = DevExpress.XtraEditors.TileItemBorderVisibility.Never;
		tileItemElement2.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileItemElement2.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Right;
		tileItemElement2.Text = "Credit Card";
		this.tileItem_CreditCard.Elements.Add(tileItemElement2);
		this.tileItem_CreditCard.Id = 7;
		this.tileItem_CreditCard.ItemSize = DevExpress.XtraEditors.TileItemSize.Small;
		this.tileItem_CreditCard.Name = "tileItem_CreditCard";
		this.tileItem_CreditCard.Tag = "F4";
		this.tileItem_Points.AppearanceItem.Hovered.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem_Points.AppearanceItem.Hovered.Options.UseFont = true;
		this.tileItem_Points.AppearanceItem.Normal.BackColor = System.Drawing.Color.FromArgb(228, 108, 10);
		this.tileItem_Points.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 11.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.tileItem_Points.AppearanceItem.Normal.ForeColor = System.Drawing.SystemColors.WindowText;
		this.tileItem_Points.AppearanceItem.Normal.Options.UseBackColor = true;
		this.tileItem_Points.AppearanceItem.Normal.Options.UseFont = true;
		this.tileItem_Points.AppearanceItem.Normal.Options.UseForeColor = true;
		this.tileItem_Points.AppearanceItem.Pressed.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem_Points.AppearanceItem.Pressed.Options.UseFont = true;
		this.tileItem_Points.AppearanceItem.Selected.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem_Points.AppearanceItem.Selected.Options.UseFont = true;
		this.tileItem_Points.BorderVisibility = DevExpress.XtraEditors.TileItemBorderVisibility.Never;
		tileItemElement3.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileItemElement3.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Right;
		tileItemElement3.Text = "Điểm tích lũy";
		this.tileItem_Points.Elements.Add(tileItemElement3);
		this.tileItem_Points.Id = 12;
		this.tileItem_Points.ItemSize = DevExpress.XtraEditors.TileItemSize.Small;
		this.tileItem_Points.Name = "tileItem_Points";
		this.tileItem1.AppearanceItem.Hovered.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem1.AppearanceItem.Hovered.Options.UseFont = true;
		this.tileItem1.AppearanceItem.Normal.BackColor = System.Drawing.Color.Teal;
		this.tileItem1.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem1.AppearanceItem.Normal.Options.UseBackColor = true;
		this.tileItem1.AppearanceItem.Normal.Options.UseFont = true;
		this.tileItem1.AppearanceItem.Pressed.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem1.AppearanceItem.Pressed.Options.UseFont = true;
		this.tileItem1.AppearanceItem.Selected.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem1.AppearanceItem.Selected.Options.UseFont = true;
		tileItemElement4.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileItemElement4.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Right;
		tileItemElement4.Text = "Trống";
		this.tileItem1.Elements.Add(tileItemElement4);
		this.tileItem1.Id = 11;
		this.tileItem1.ItemSize = DevExpress.XtraEditors.TileItemSize.Medium;
		this.tileItem1.Name = "tileItem1";
		this.tileItem4.AppearanceItem.Hovered.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem4.AppearanceItem.Hovered.Options.UseFont = true;
		this.tileItem4.AppearanceItem.Normal.BackColor = System.Drawing.Color.Teal;
		this.tileItem4.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem4.AppearanceItem.Normal.Options.UseBackColor = true;
		this.tileItem4.AppearanceItem.Normal.Options.UseFont = true;
		this.tileItem4.AppearanceItem.Pressed.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem4.AppearanceItem.Pressed.Options.UseFont = true;
		this.tileItem4.AppearanceItem.Selected.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem4.AppearanceItem.Selected.Options.UseFont = true;
		tileItemElement5.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileItemElement5.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Right;
		tileItemElement5.Text = "Trống";
		this.tileItem4.Elements.Add(tileItemElement5);
		this.tileItem4.Id = 11;
		this.tileItem4.ItemSize = DevExpress.XtraEditors.TileItemSize.Medium;
		this.tileItem4.Name = "tileItem4";
		this.tileItem6.AppearanceItem.Hovered.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem6.AppearanceItem.Hovered.Options.UseFont = true;
		this.tileItem6.AppearanceItem.Normal.BackColor = System.Drawing.Color.Teal;
		this.tileItem6.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem6.AppearanceItem.Normal.Options.UseBackColor = true;
		this.tileItem6.AppearanceItem.Normal.Options.UseFont = true;
		this.tileItem6.AppearanceItem.Pressed.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem6.AppearanceItem.Pressed.Options.UseFont = true;
		this.tileItem6.AppearanceItem.Selected.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem6.AppearanceItem.Selected.Options.UseFont = true;
		tileItemElement6.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileItemElement6.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Right;
		tileItemElement6.Text = "Trống";
		this.tileItem6.Elements.Add(tileItemElement6);
		this.tileItem6.Id = 11;
		this.tileItem6.ItemSize = DevExpress.XtraEditors.TileItemSize.Medium;
		this.tileItem6.Name = "tileItem6";
		this.tileItem8.AppearanceItem.Hovered.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem8.AppearanceItem.Hovered.Options.UseFont = true;
		this.tileItem8.AppearanceItem.Normal.BackColor = System.Drawing.Color.Teal;
		this.tileItem8.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem8.AppearanceItem.Normal.Options.UseBackColor = true;
		this.tileItem8.AppearanceItem.Normal.Options.UseFont = true;
		this.tileItem8.AppearanceItem.Pressed.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem8.AppearanceItem.Pressed.Options.UseFont = true;
		this.tileItem8.AppearanceItem.Selected.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem8.AppearanceItem.Selected.Options.UseFont = true;
		tileItemElement7.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileItemElement7.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Right;
		tileItemElement7.Text = "Trống";
		this.tileItem8.Elements.Add(tileItemElement7);
		this.tileItem8.Id = 11;
		this.tileItem8.ItemSize = DevExpress.XtraEditors.TileItemSize.Medium;
		this.tileItem8.Name = "tileItem8";
		this.tileItem9.AppearanceItem.Hovered.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem9.AppearanceItem.Hovered.Options.UseFont = true;
		this.tileItem9.AppearanceItem.Normal.BackColor = System.Drawing.Color.Teal;
		this.tileItem9.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem9.AppearanceItem.Normal.Options.UseBackColor = true;
		this.tileItem9.AppearanceItem.Normal.Options.UseFont = true;
		this.tileItem9.AppearanceItem.Pressed.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem9.AppearanceItem.Pressed.Options.UseFont = true;
		this.tileItem9.AppearanceItem.Selected.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem9.AppearanceItem.Selected.Options.UseFont = true;
		tileItemElement8.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileItemElement8.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Right;
		tileItemElement8.Text = "Trống";
		this.tileItem9.Elements.Add(tileItemElement8);
		this.tileItem9.Id = 11;
		this.tileItem9.ItemSize = DevExpress.XtraEditors.TileItemSize.Medium;
		this.tileItem9.Name = "tileItem9";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.tileControl1);
		base.Name = "ucPaymentCmd";
		base.Size = new System.Drawing.Size(600, 450);
		base.ResumeLayout(false);
	}
}
