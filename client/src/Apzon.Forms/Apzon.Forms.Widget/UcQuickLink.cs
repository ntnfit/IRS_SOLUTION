using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Controls.Form;
using Apzon.Forms.Sales;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.TableLayout;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Tile;

namespace Apzon.Forms.Widget;

public class UcQuickLink : XtraUserControl
{
	private readonly Color _unreadTextColor = Color.FromArgb(248, 124, 50);

	private IContainer components;

	private GridControl gridControl1;

	private TileView tileView1;

	private TileViewColumn colDate;

	private TileViewColumn colFrom;

	private TileViewColumn colSubject;

	private TileViewColumn colPlainText;

	private TileViewColumn colSubjectDisplayText;

	private TileViewColumn colRead;

	private TileViewColumn colPriority;

	public UcQuickLink()
	{
		InitializeComponent();
	}

	private void gridControl1_Load(object sender, EventArgs e)
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Date", typeof(DateTime));
			dataTable.Columns.Add("From", typeof(string));
			dataTable.Columns.Add("Subject", typeof(string));
			dataTable.Columns.Add("PlanText", typeof(string));
			dataTable.Columns.Add("SubjectDisplayText", typeof(string));
			dataTable.Columns.Add("Read", typeof(bool));
			dataTable.Columns.Add("Priority", typeof(int));
			dataTable.Columns.Add("ObjType", typeof(string));
			dataTable.Columns.Add("ObjCode", typeof(string));
			DataRow dataRow = dataTable.NewRow();
			dataRow["Date"] = DateTime.Today;
			dataRow["From"] = "Create New Sales Order";
			dataRow["Subject"] = "Create New Sales Order";
			dataRow["PlanText"] = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Duis luctus auctor orci, a porta urna bibendum et. In quis semper diam. Suspendisse at cursus neque";
			dataRow["SubjectDisplayText"] = "Vivamus tincidunt diam enim, sit amet volutpat tortor venenatis dictum. Quisque venenatis, lectus sed facilisis lacinia, odio ipsum blandit nulla";
			dataTable.Rows.Add(dataRow);
			dataRow = dataTable.NewRow();
			dataRow["Date"] = DateTime.Today;
			dataRow["From"] = "List Of Delivery";
			dataRow["Subject"] = "List Of Delivery";
			dataRow["PlanText"] = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Duis luctus auctor orci, a porta urna bibendum et. In quis semper diam. Suspendisse at cursus neque";
			dataRow["SubjectDisplayText"] = "Vivamus tincidunt diam enim, sit amet volutpat tortor venenatis dictum. Quisque venenatis, lectus sed facilisis lacinia, odio ipsum blandit nulla";
			dataTable.Rows.Add(dataRow);
			dataRow = dataTable.NewRow();
			dataRow["Date"] = DateTime.Today;
			dataRow["From"] = "Open Invoices";
			dataRow["Subject"] = "Open Invoices";
			dataRow["PlanText"] = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Duis luctus auctor orci, a porta urna bibendum et. In quis semper diam. Suspendisse at cursus neque";
			dataRow["SubjectDisplayText"] = "Vivamus tincidunt diam enim, sit amet volutpat tortor venenatis dictum. Quisque venenatis, lectus sed facilisis lacinia, odio ipsum blandit nulla";
			dataTable.Rows.Add(dataRow);
			gridControl1.DataSource = dataTable;
			gridControl1.RefreshDataSource();
			tileView1.Appearance.ItemFocused.BackColor = Color.FromArgb(40, _unreadTextColor);
			tileView1.Appearance.ItemHovered.BackColor = Color.FromArgb(40, _unreadTextColor);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void tileView1_ItemClick(object sender, TileViewItemClickEventArgs e)
	{
		try
		{
			frm_SalesOrder frm_SalesOrder2 = new frm_SalesOrder();
			if (FindForm() is XtraFormEx xtraFormEx)
			{
				frm_SalesOrder2.MdiParent = xtraFormEx.MdiParent;
			}
			frm_SalesOrder2.Show();
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
		DevExpress.Utils.ContextButton contextButton = new DevExpress.Utils.ContextButton();
		DevExpress.Utils.ContextButton contextButton2 = new DevExpress.Utils.ContextButton();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Widget.UcQuickLink));
		DevExpress.Utils.ContextButton contextButton3 = new DevExpress.Utils.ContextButton();
		DevExpress.Utils.ContextButton contextButton4 = new DevExpress.Utils.ContextButton();
		DevExpress.XtraGrid.Views.Tile.ItemTemplate itemTemplate = new DevExpress.XtraGrid.Views.Tile.ItemTemplate();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition2 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement2 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement3 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement4 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraEditors.TableLayout.TableRowDefinition obj = new DevExpress.XtraEditors.TableLayout.TableRowDefinition();
		DevExpress.XtraEditors.TableLayout.TableRowDefinition obj2 = new DevExpress.XtraEditors.TableLayout.TableRowDefinition();
		DevExpress.XtraEditors.TableLayout.TableRowDefinition obj3 = new DevExpress.XtraEditors.TableLayout.TableRowDefinition();
		DevExpress.XtraEditors.TableLayout.TableSpan tableSpan = new DevExpress.XtraEditors.TableLayout.TableSpan();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition3 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition4 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition5 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition6 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition7 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableRowDefinition obj4 = new DevExpress.XtraEditors.TableLayout.TableRowDefinition();
		DevExpress.XtraEditors.TableLayout.TableRowDefinition obj5 = new DevExpress.XtraEditors.TableLayout.TableRowDefinition();
		DevExpress.XtraEditors.TableLayout.TableSpan tableSpan2 = new DevExpress.XtraEditors.TableLayout.TableSpan();
		DevExpress.XtraEditors.TableLayout.TableSpan tableSpan3 = new DevExpress.XtraEditors.TableLayout.TableSpan();
		DevExpress.XtraEditors.TableLayout.TableSpan tableSpan4 = new DevExpress.XtraEditors.TableLayout.TableSpan();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement5 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement6 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement7 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		this.colDate = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.colSubjectDisplayText = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.colFrom = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.colRead = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.colPlainText = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.gridControl1 = new DevExpress.XtraGrid.GridControl();
		this.tileView1 = new DevExpress.XtraGrid.Views.Tile.TileView();
		this.colSubject = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.colPriority = new DevExpress.XtraGrid.Columns.TileViewColumn();
		((System.ComponentModel.ISupportInitialize)this.gridControl1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tileView1).BeginInit();
		base.SuspendLayout();
		this.colDate.Caption = "colDate";
		this.colDate.FieldName = "Date";
		this.colDate.GroupInterval = DevExpress.XtraGrid.ColumnGroupInterval.DateRange;
		this.colDate.Name = "colDate";
		this.colDate.Visible = true;
		this.colDate.VisibleIndex = 0;
		this.colSubjectDisplayText.Caption = "colSubjectDisplayText";
		this.colSubjectDisplayText.FieldName = "SubjectDisplayText";
		this.colSubjectDisplayText.Name = "colSubjectDisplayText";
		this.colSubjectDisplayText.Visible = true;
		this.colSubjectDisplayText.VisibleIndex = 4;
		this.colFrom.Caption = "colFrom";
		this.colFrom.FieldName = "From";
		this.colFrom.Name = "colFrom";
		this.colFrom.Visible = true;
		this.colFrom.VisibleIndex = 1;
		this.colRead.Caption = "colRead";
		this.colRead.FieldName = "Read";
		this.colRead.Name = "colRead";
		this.colRead.Visible = true;
		this.colRead.VisibleIndex = 5;
		this.colPlainText.Caption = "colPlainText";
		this.colPlainText.FieldName = "PlainText";
		this.colPlainText.Name = "colPlainText";
		this.colPlainText.Visible = true;
		this.colPlainText.VisibleIndex = 3;
		this.gridControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.gridControl1.Location = new System.Drawing.Point(0, 0);
		this.gridControl1.MainView = this.tileView1;
		this.gridControl1.Name = "gridControl1";
		this.gridControl1.Size = new System.Drawing.Size(681, 451);
		this.gridControl1.TabIndex = 1;
		this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.tileView1 });
		this.gridControl1.Load += new System.EventHandler(this.gridControl1_Load);
		this.tileView1.Appearance.GroupText.Font = new System.Drawing.Font("Segoe UI", 9f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.tileView1.Appearance.GroupText.ForeColor = System.Drawing.Color.FromArgb(248, 124, 50);
		this.tileView1.Appearance.GroupText.Options.UseFont = true;
		this.tileView1.Appearance.GroupText.Options.UseForeColor = true;
		this.tileView1.Appearance.ItemNormal.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.tileView1.Appearance.ItemNormal.Options.UseFont = true;
		this.tileView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[7] { this.colDate, this.colFrom, this.colSubject, this.colPlainText, this.colSubjectDisplayText, this.colRead, this.colPriority });
		this.tileView1.ContextButtonOptions.Indent = 6;
		this.tileView1.ContextButtonOptions.TopPanelPadding = new System.Windows.Forms.Padding(5, 10, 20, 5);
		contextButton.Alignment = DevExpress.Utils.ContextItemAlignment.TopFar;
		contextButton.AppearanceHover.Font = new System.Drawing.Font("Segoe UI", 14.25f, System.Drawing.FontStyle.Bold);
		contextButton.AppearanceHover.ForeColor = System.Drawing.Color.Red;
		contextButton.AppearanceHover.Options.UseFont = true;
		contextButton.AppearanceHover.Options.UseForeColor = true;
		contextButton.AppearanceNormal.Font = new System.Drawing.Font("Segoe UI", 14.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		contextButton.AppearanceNormal.FontStyleDelta = System.Drawing.FontStyle.Bold;
		contextButton.AppearanceNormal.ForeColor = System.Drawing.Color.Red;
		contextButton.AppearanceNormal.Options.UseFont = true;
		contextButton.AppearanceNormal.Options.UseForeColor = true;
		contextButton.Caption = "!";
		contextButton.Id = new System.Guid("46c5197d-a771-45bb-9de8-afed5491a3c5");
		contextButton.Name = "cbExclamation";
		contextButton.Padding = new System.Windows.Forms.Padding(0, -1, 0, 0);
		contextButton.Visibility = DevExpress.Utils.ContextItemVisibility.Hidden;
		contextButton2.Alignment = DevExpress.Utils.ContextItemAlignment.TopFar;
		contextButton2.Id = new System.Guid("07c1aaac-55bd-4ebc-b850-93a79dede0db");
		contextButton2.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("resource.Image");
		contextButton2.Name = "cbRemove";
		contextButton2.Visibility = DevExpress.Utils.ContextItemVisibility.Hidden;
		contextButton3.Alignment = DevExpress.Utils.ContextItemAlignment.TopFar;
		contextButton3.Id = new System.Guid("b5ddbc13-0cf7-4468-afe5-8d6d6df06d77");
		contextButton3.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("resource.Image1");
		contextButton3.Name = "cbEdit";
		contextButton3.Visibility = DevExpress.Utils.ContextItemVisibility.Hidden;
		contextButton4.Alignment = DevExpress.Utils.ContextItemAlignment.TopFar;
		contextButton4.Id = new System.Guid("83ef3e55-2c44-470e-acca-c4d796b1ecbd");
		contextButton4.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("resource.Image2");
		contextButton4.Name = "cbShow";
		contextButton4.Visibility = DevExpress.Utils.ContextItemVisibility.Hidden;
		this.tileView1.ContextButtons.Add(contextButton);
		this.tileView1.ContextButtons.Add(contextButton2);
		this.tileView1.ContextButtons.Add(contextButton3);
		this.tileView1.ContextButtons.Add(contextButton4);
		this.tileView1.FocusBorderColor = System.Drawing.Color.Transparent;
		this.tileView1.GridControl = this.gridControl1;
		this.tileView1.Name = "tileView1";
		this.tileView1.OptionsBehavior.AllowSmoothScrolling = true;
		this.tileView1.OptionsTiles.AllowItemHover = true;
		this.tileView1.OptionsTiles.AllowPressAnimation = false;
		this.tileView1.OptionsTiles.GroupTextOffset = 30;
		this.tileView1.OptionsTiles.GroupTextToItemsIndent = 10;
		this.tileView1.OptionsTiles.IndentBetweenGroups = 10;
		this.tileView1.OptionsTiles.ItemPadding = new System.Windows.Forms.Padding(0, 8, 12, 8);
		this.tileView1.OptionsTiles.ItemSize = new System.Drawing.Size(450, 50);
		this.tileView1.OptionsTiles.LayoutMode = DevExpress.XtraGrid.Views.Tile.TileViewLayoutMode.List;
		this.tileView1.OptionsTiles.Orientation = System.Windows.Forms.Orientation.Vertical;
		this.tileView1.OptionsTiles.Padding = new System.Windows.Forms.Padding(0);
		this.tileView1.OptionsTiles.ScrollMode = DevExpress.XtraEditors.TileControlScrollMode.TouchScrollBar;
		tableColumnDefinition.Length.Type = DevExpress.XtraEditors.TableLayout.TableDefinitionLengthType.Pixel;
		tableColumnDefinition.Length.Value = 28.0;
		tableColumnDefinition2.Length.Value = 214.0;
		itemTemplate.Columns.Add(tableColumnDefinition);
		itemTemplate.Columns.Add(tableColumnDefinition2);
		tileViewItemElement.Appearance.Normal.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		tileViewItemElement.Appearance.Normal.ForeColor = System.Drawing.Color.FromArgb(145, 154, 154);
		tileViewItemElement.Appearance.Normal.Options.UseFont = true;
		tileViewItemElement.Appearance.Normal.Options.UseForeColor = true;
		tileViewItemElement.Appearance.Normal.Options.UseTextOptions = true;
		tileViewItemElement.Appearance.Normal.TextOptions.WordWrap = DevExpress.Utils.WordWrap.NoWrap;
		tileViewItemElement.Column = this.colDate;
		tileViewItemElement.ColumnIndex = 1;
		tileViewItemElement.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement.RowIndex = 2;
		tileViewItemElement.Text = "colDate";
		tileViewItemElement.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleLeft;
		tileViewItemElement2.Appearance.Normal.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		tileViewItemElement2.Appearance.Normal.Options.UseFont = true;
		tileViewItemElement2.Appearance.Normal.Options.UseTextOptions = true;
		tileViewItemElement2.Appearance.Normal.TextOptions.WordWrap = DevExpress.Utils.WordWrap.NoWrap;
		tileViewItemElement2.Column = this.colSubjectDisplayText;
		tileViewItemElement2.ColumnIndex = 1;
		tileViewItemElement2.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement2.RowIndex = 1;
		tileViewItemElement2.Text = "colSubjectDisplayText";
		tileViewItemElement2.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleLeft;
		tileViewItemElement3.Appearance.Normal.Font = new System.Drawing.Font("Segoe UI", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		tileViewItemElement3.Appearance.Normal.Options.UseFont = true;
		tileViewItemElement3.Appearance.Normal.Options.UseTextOptions = true;
		tileViewItemElement3.Appearance.Normal.TextOptions.WordWrap = DevExpress.Utils.WordWrap.NoWrap;
		tileViewItemElement3.Column = this.colFrom;
		tileViewItemElement3.ColumnIndex = 1;
		tileViewItemElement3.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement3.Text = "colFrom";
		tileViewItemElement3.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleLeft;
		tileViewItemElement4.Column = this.colRead;
		tileViewItemElement4.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement4.Name = "readElement";
		tileViewItemElement4.RowIndex = 1;
		tileViewItemElement4.StretchVertical = true;
		tileViewItemElement4.Text = "colRead";
		tileViewItemElement4.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleLeft;
		tileViewItemElement4.TextVisible = false;
		tileViewItemElement4.Width = 3;
		itemTemplate.Elements.Add(tileViewItemElement);
		itemTemplate.Elements.Add(tileViewItemElement2);
		itemTemplate.Elements.Add(tileViewItemElement3);
		itemTemplate.Elements.Add(tileViewItemElement4);
		itemTemplate.Name = "smallsize";
		itemTemplate.Rows.Add(obj);
		itemTemplate.Rows.Add(obj2);
		itemTemplate.Rows.Add(obj3);
		tableSpan.RowSpan = 3;
		itemTemplate.Spans.Add(tableSpan);
		this.tileView1.Templates.Add(itemTemplate);
		tableColumnDefinition3.Length.Type = DevExpress.XtraEditors.TableLayout.TableDefinitionLengthType.Pixel;
		tableColumnDefinition3.Length.Value = 28.0;
		tableColumnDefinition4.Length.Value = 214.0;
		tableColumnDefinition5.Length.Value = 171.0;
		tableColumnDefinition6.Length.Type = DevExpress.XtraEditors.TableLayout.TableDefinitionLengthType.Pixel;
		tableColumnDefinition6.Length.Value = 45.0;
		tableColumnDefinition7.Length.Type = DevExpress.XtraEditors.TableLayout.TableDefinitionLengthType.Pixel;
		tableColumnDefinition7.Length.Value = 45.0;
		this.tileView1.TileColumns.Add(tableColumnDefinition3);
		this.tileView1.TileColumns.Add(tableColumnDefinition4);
		this.tileView1.TileColumns.Add(tableColumnDefinition5);
		this.tileView1.TileColumns.Add(tableColumnDefinition6);
		this.tileView1.TileColumns.Add(tableColumnDefinition7);
		this.tileView1.TileRows.Add(obj4);
		this.tileView1.TileRows.Add(obj5);
		tableSpan2.ColumnIndex = 1;
		tableSpan2.ColumnSpan = 3;
		tableSpan3.RowSpan = 3;
		tableSpan4.ColumnIndex = 1;
		tableSpan4.ColumnSpan = 4;
		tableSpan4.RowIndex = 1;
		this.tileView1.TileSpans.Add(tableSpan2);
		this.tileView1.TileSpans.Add(tableSpan3);
		this.tileView1.TileSpans.Add(tableSpan4);
		tileViewItemElement5.Appearance.Normal.Font = new System.Drawing.Font("Segoe UI", 8.25f);
		tileViewItemElement5.Appearance.Normal.ForeColor = System.Drawing.Color.Gray;
		tileViewItemElement5.Appearance.Normal.Options.UseFont = true;
		tileViewItemElement5.Appearance.Normal.Options.UseForeColor = true;
		tileViewItemElement5.Appearance.Normal.Options.UseTextOptions = true;
		tileViewItemElement5.Appearance.Normal.TextOptions.WordWrap = DevExpress.Utils.WordWrap.NoWrap;
		tileViewItemElement5.Column = this.colSubjectDisplayText;
		tileViewItemElement5.ColumnIndex = 1;
		tileViewItemElement5.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement5.RowIndex = 1;
		tileViewItemElement5.Text = "colSubjectDisplayText";
		tileViewItemElement5.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleLeft;
		tileViewItemElement6.Appearance.Normal.Font = new System.Drawing.Font("Segoe UI", 9.25f);
		tileViewItemElement6.Appearance.Normal.Options.UseFont = true;
		tileViewItemElement6.Appearance.Normal.Options.UseTextOptions = true;
		tileViewItemElement6.Appearance.Normal.TextOptions.WordWrap = DevExpress.Utils.WordWrap.NoWrap;
		tileViewItemElement6.Column = this.colFrom;
		tileViewItemElement6.ColumnIndex = 2;
		tileViewItemElement6.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement6.Text = "colFrom";
		tileViewItemElement6.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleLeft;
		tileViewItemElement7.Column = this.colRead;
		tileViewItemElement7.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement7.Name = "readElement";
		tileViewItemElement7.RowIndex = 1;
		tileViewItemElement7.StretchVertical = true;
		tileViewItemElement7.Text = "colRead";
		tileViewItemElement7.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleLeft;
		tileViewItemElement7.TextVisible = false;
		tileViewItemElement7.Width = 3;
		this.tileView1.TileTemplate.Add(tileViewItemElement5);
		this.tileView1.TileTemplate.Add(tileViewItemElement6);
		this.tileView1.TileTemplate.Add(tileViewItemElement7);
		this.tileView1.ItemClick += new DevExpress.XtraGrid.Views.Tile.TileViewItemClickEventHandler(this.tileView1_ItemClick);
		this.colSubject.Caption = "colSubject";
		this.colSubject.FieldName = "Subject";
		this.colSubject.Name = "colSubject";
		this.colSubject.Visible = true;
		this.colSubject.VisibleIndex = 2;
		this.colPriority.Caption = "colPriority";
		this.colPriority.FieldName = "Priority";
		this.colPriority.Name = "colPriority";
		this.colPriority.Visible = true;
		this.colPriority.VisibleIndex = 6;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.gridControl1);
		base.Name = "UcQuickLink";
		base.Size = new System.Drawing.Size(681, 451);
		((System.ComponentModel.ISupportInitialize)this.gridControl1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tileView1).EndInit();
		base.ResumeLayout(false);
	}
}
