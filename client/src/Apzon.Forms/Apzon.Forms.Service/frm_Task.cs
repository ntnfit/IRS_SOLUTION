using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Navigation;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.TableLayout;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Tile;

namespace Apzon.Forms.Service;

public class frm_Task : XtraFormEx
{
	private Color UnreadTextColor = Color.FromArgb(248, 124, 50);

	private IContainer components;

	private BarSubItem barSubItem2;

	private BarCheckItem bcFilterYesterday;

	private BarSubItem barSubItem1;

	private BarButtonItem biSortDate;

	private BarButtonItem biSortFrom;

	private BarButtonItem biSortRead;

	private BarButtonItem biSortSubject;

	private BarButtonItem biSortImportance;

	private BarCheckItem bcFilterToday;

	private BarDockControl barDockControlLeft;

	private BarManager barManager1;

	private BarDockControl barDockControlTop;

	private BarDockControl barDockControlBottom;

	private BarDockControl barDockControlRight;

	private PanelControl panel1;

	private DropDownButton labelControl1;

	private PopupMenu popupMenu1;

	private BarCheckItem bcFilterAll;

	private BarCheckItem bcFilterUnread;

	private BarCheckItem bcFilterImportance;

	private GridControl gridControl1;

	private TileView tileView1;

	private TileViewColumn colDate;

	private TileViewColumn colFrom;

	private TileViewColumn colSubject;

	private TileViewColumn colPlainText;

	private TileViewColumn colSubjectDisplayText;

	private TileViewColumn colRead;

	private TileViewColumn colPriority;

	private ImageCollection imageCollection2;

	private BarButtonItem barButtonItem1;

	private ImageCollection imageCollection1;

	private ButtonEditEx buttonEdit1;

	private AccordionControl accordionControl1;

	private AccordionControlElement accordionControlElement1;

	private AccordionControlElement accordionControlElement2;

	private AccordionControlElement accordionControlElement3;

	private AccordionControlElement accordionControlElement4;

	private AccordionControlElement accordionControlElement5;

	private AccordionControlElement accordionControlElement6;

	private AccordionControlElement accordionControlElement7;

	private AccordionControlElement accordionControlElement8;

	private AccordionControlElement accordionControlElement9;

	private AccordionControlElement accordionControlElement10;

	private AccordionControlElement accordionControlElement11;

	private AccordionControlElement accordionControlElement12;

	private AccordionControlElement accordionControlElement13;

	private AccordionControlElement accordionControlElement14;

	private AccordionControlElement accordionControlElement15;

	private SplitContainerControl splitContainerControl1;

	private RepositoryItemMemoExEdit repositoryItemMemoExEdit1;

	public frm_Task()
	{
		InitializeComponent();
	}

	private void frm_Task_Load(object sender, EventArgs e)
	{
		try
		{
			gridControl1.DataSource = DataHelper.Messages;
			colDate.SortOrder = ColumnSortOrder.Descending;
			tileView1.Appearance.ItemFocused.BackColor = Color.FromArgb(40, UnreadTextColor);
			tileView1.Appearance.ItemHovered.BackColor = Color.FromArgb(40, UnreadTextColor);
			tileView1.FocusedRowHandle = 2;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void buttonEdit2_EditValueChanged(object sender, EventArgs e)
	{
	}

	private void panel1_Paint(object sender, PaintEventArgs e)
	{
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
		DevExpress.Utils.ContextButton contextButton = new DevExpress.Utils.ContextButton();
		DevExpress.Utils.ContextButton contextButton2 = new DevExpress.Utils.ContextButton();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Service.frm_Task));
		DevExpress.Utils.ContextButton contextButton3 = new DevExpress.Utils.ContextButton();
		DevExpress.Utils.ContextButton contextButton4 = new DevExpress.Utils.ContextButton();
		DevExpress.Utils.ContextButton contextButton5 = new DevExpress.Utils.ContextButton();
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
		DevExpress.XtraEditors.TableLayout.TableRowDefinition obj6 = new DevExpress.XtraEditors.TableLayout.TableRowDefinition();
		DevExpress.XtraEditors.TableLayout.TableSpan tableSpan2 = new DevExpress.XtraEditors.TableLayout.TableSpan();
		DevExpress.XtraEditors.TableLayout.TableSpan tableSpan3 = new DevExpress.XtraEditors.TableLayout.TableSpan();
		DevExpress.XtraEditors.TableLayout.TableSpan tableSpan4 = new DevExpress.XtraEditors.TableLayout.TableSpan();
		DevExpress.XtraEditors.TableLayout.TableSpan tableSpan5 = new DevExpress.XtraEditors.TableLayout.TableSpan();
		DevExpress.XtraEditors.TableLayout.TableSpan tableSpan6 = new DevExpress.XtraEditors.TableLayout.TableSpan();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement5 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement6 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement7 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement8 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement9 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		this.colDate = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.colSubjectDisplayText = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.colFrom = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.colRead = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.colPlainText = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.repositoryItemMemoExEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit();
		this.barSubItem2 = new DevExpress.XtraBars.BarSubItem();
		this.bcFilterYesterday = new DevExpress.XtraBars.BarCheckItem();
		this.barSubItem1 = new DevExpress.XtraBars.BarSubItem();
		this.biSortDate = new DevExpress.XtraBars.BarButtonItem();
		this.biSortFrom = new DevExpress.XtraBars.BarButtonItem();
		this.biSortRead = new DevExpress.XtraBars.BarButtonItem();
		this.biSortSubject = new DevExpress.XtraBars.BarButtonItem();
		this.biSortImportance = new DevExpress.XtraBars.BarButtonItem();
		this.bcFilterToday = new DevExpress.XtraBars.BarCheckItem();
		this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
		this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
		this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
		this.imageCollection2 = new DevExpress.Utils.ImageCollection(this.components);
		this.barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
		this.bcFilterAll = new DevExpress.XtraBars.BarCheckItem();
		this.bcFilterUnread = new DevExpress.XtraBars.BarCheckItem();
		this.bcFilterImportance = new DevExpress.XtraBars.BarCheckItem();
		this.panel1 = new DevExpress.XtraEditors.PanelControl();
		this.buttonEdit1 = new Apzon.Controls.Editor.ButtonEditEx();
		this.labelControl1 = new DevExpress.XtraEditors.DropDownButton();
		this.popupMenu1 = new DevExpress.XtraBars.PopupMenu(this.components);
		this.colPriority = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.imageCollection1 = new DevExpress.Utils.ImageCollection(this.components);
		this.gridControl1 = new DevExpress.XtraGrid.GridControl();
		this.tileView1 = new DevExpress.XtraGrid.Views.Tile.TileView();
		this.colSubject = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.accordionControl1 = new DevExpress.XtraBars.Navigation.AccordionControl();
		this.accordionControlElement1 = new DevExpress.XtraBars.Navigation.AccordionControlElement();
		this.accordionControlElement2 = new DevExpress.XtraBars.Navigation.AccordionControlElement();
		this.accordionControlElement3 = new DevExpress.XtraBars.Navigation.AccordionControlElement();
		this.accordionControlElement4 = new DevExpress.XtraBars.Navigation.AccordionControlElement();
		this.accordionControlElement5 = new DevExpress.XtraBars.Navigation.AccordionControlElement();
		this.accordionControlElement6 = new DevExpress.XtraBars.Navigation.AccordionControlElement();
		this.accordionControlElement7 = new DevExpress.XtraBars.Navigation.AccordionControlElement();
		this.accordionControlElement8 = new DevExpress.XtraBars.Navigation.AccordionControlElement();
		this.accordionControlElement9 = new DevExpress.XtraBars.Navigation.AccordionControlElement();
		this.accordionControlElement10 = new DevExpress.XtraBars.Navigation.AccordionControlElement();
		this.accordionControlElement11 = new DevExpress.XtraBars.Navigation.AccordionControlElement();
		this.accordionControlElement12 = new DevExpress.XtraBars.Navigation.AccordionControlElement();
		this.accordionControlElement13 = new DevExpress.XtraBars.Navigation.AccordionControlElement();
		this.accordionControlElement14 = new DevExpress.XtraBars.Navigation.AccordionControlElement();
		this.accordionControlElement15 = new DevExpress.XtraBars.Navigation.AccordionControlElement();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.barManager1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.imageCollection2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.panel1).BeginInit();
		this.panel1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.buttonEdit1.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.popupMenu1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.imageCollection1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridControl1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tileView1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.accordionControl1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
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
		this.repositoryItemMemoExEdit1.AutoHeight = false;
		this.repositoryItemMemoExEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemMemoExEdit1.Name = "repositoryItemMemoExEdit1";
		this.barSubItem2.Caption = "Show As";
		this.barSubItem2.Id = 10;
		this.barSubItem2.Name = "barSubItem2";
		this.bcFilterYesterday.Caption = "Yesterday";
		this.bcFilterYesterday.GroupIndex = 22;
		this.bcFilterYesterday.Id = 5;
		this.bcFilterYesterday.Name = "bcFilterYesterday";
		this.barSubItem1.Caption = "Sort By";
		this.barSubItem1.Id = 6;
		this.barSubItem1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[5]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.biSortDate),
			new DevExpress.XtraBars.LinkPersistInfo(this.biSortFrom),
			new DevExpress.XtraBars.LinkPersistInfo(this.biSortRead),
			new DevExpress.XtraBars.LinkPersistInfo(this.biSortSubject),
			new DevExpress.XtraBars.LinkPersistInfo(this.biSortImportance)
		});
		this.barSubItem1.Name = "barSubItem1";
		this.biSortDate.Caption = "Date";
		this.biSortDate.Id = 7;
		this.biSortDate.Name = "biSortDate";
		this.biSortFrom.Caption = "From";
		this.biSortFrom.Id = 8;
		this.biSortFrom.Name = "biSortFrom";
		this.biSortRead.Caption = "To";
		this.biSortRead.Id = 9;
		this.biSortRead.Name = "biSortRead";
		this.biSortSubject.Caption = "Subject";
		this.biSortSubject.Id = 11;
		this.biSortSubject.Name = "biSortSubject";
		this.biSortImportance.Caption = "Importance";
		this.biSortImportance.Id = 12;
		this.biSortImportance.Name = "biSortImportance";
		this.bcFilterToday.Caption = "Today";
		this.bcFilterToday.GroupIndex = 22;
		this.bcFilterToday.Id = 4;
		this.bcFilterToday.Name = "bcFilterToday";
		this.barDockControlLeft.CausesValidation = false;
		this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
		this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
		this.barDockControlLeft.Manager = this.barManager1;
		this.barDockControlLeft.Size = new System.Drawing.Size(0, 662);
		this.barManager1.AllowGlyphSkinning = true;
		this.barManager1.DockControls.Add(this.barDockControlTop);
		this.barManager1.DockControls.Add(this.barDockControlBottom);
		this.barManager1.DockControls.Add(this.barDockControlLeft);
		this.barManager1.DockControls.Add(this.barDockControlRight);
		this.barManager1.Form = this;
		this.barManager1.Images = this.imageCollection2;
		this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[13]
		{
			this.barButtonItem1, this.bcFilterAll, this.bcFilterUnread, this.bcFilterImportance, this.bcFilterToday, this.bcFilterYesterday, this.barSubItem1, this.biSortDate, this.biSortFrom, this.biSortRead,
			this.barSubItem2, this.biSortSubject, this.biSortImportance
		});
		this.barManager1.MaxItemId = 13;
		this.barDockControlTop.CausesValidation = false;
		this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
		this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
		this.barDockControlTop.Manager = this.barManager1;
		this.barDockControlTop.Size = new System.Drawing.Size(1480, 0);
		this.barDockControlBottom.CausesValidation = false;
		this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.barDockControlBottom.Location = new System.Drawing.Point(0, 662);
		this.barDockControlBottom.Manager = this.barManager1;
		this.barDockControlBottom.Size = new System.Drawing.Size(1480, 0);
		this.barDockControlRight.CausesValidation = false;
		this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
		this.barDockControlRight.Location = new System.Drawing.Point(1480, 0);
		this.barDockControlRight.Manager = this.barManager1;
		this.barDockControlRight.Size = new System.Drawing.Size(0, 662);
		this.imageCollection2.ImageStream = (DevExpress.Utils.ImageCollectionStreamer)resources.GetObject("imageCollection2.ImageStream");
		this.barButtonItem1.Caption = "barButtonItem1";
		this.barButtonItem1.Id = 0;
		this.barButtonItem1.Name = "barButtonItem1";
		this.bcFilterAll.BindableChecked = true;
		this.bcFilterAll.Caption = "All";
		this.bcFilterAll.Checked = true;
		this.bcFilterAll.GroupIndex = 22;
		this.bcFilterAll.Id = 1;
		this.bcFilterAll.Name = "bcFilterAll";
		this.bcFilterUnread.Caption = "Unread";
		this.bcFilterUnread.GroupIndex = 22;
		this.bcFilterUnread.Id = 2;
		this.bcFilterUnread.Name = "bcFilterUnread";
		this.bcFilterImportance.Caption = "Important";
		this.bcFilterImportance.GroupIndex = 22;
		this.bcFilterImportance.Id = 3;
		this.bcFilterImportance.Name = "bcFilterImportance";
		this.panel1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.panel1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.panel1.Controls.Add(this.buttonEdit1);
		this.panel1.Controls.Add(this.labelControl1);
		this.panel1.Location = new System.Drawing.Point(8, 3);
		this.panel1.Name = "panel1";
		this.panel1.Padding = new System.Windows.Forms.Padding(28, 0, 28, 0);
		this.panel1.Size = new System.Drawing.Size(514, 56);
		this.panel1.TabIndex = 9;
		this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
		this.buttonEdit1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.buttonEdit1.FieldName = null;
		this.buttonEdit1.Location = new System.Drawing.Point(0, 13);
		this.buttonEdit1.MenuManager = this.barManager1;
		this.buttonEdit1.Name = "buttonEdit1";
		this.buttonEdit1.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 14.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.buttonEdit1.Properties.Appearance.Options.UseFont = true;
		this.buttonEdit1.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.buttonEdit1.Size = new System.Drawing.Size(423, 30);
		this.buttonEdit1.TabIndex = 2;
		this.labelControl1.AllowFocus = false;
		this.labelControl1.Appearance.Font = new System.Drawing.Font("Segoe UI", 14.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.labelControl1.Appearance.Options.UseFont = true;
		this.labelControl1.AppearanceHovered.Font = new System.Drawing.Font("Segoe UI", 14.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.labelControl1.AppearanceHovered.Options.UseFont = true;
		this.labelControl1.AppearancePressed.Font = new System.Drawing.Font("Segoe UI", 14.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.labelControl1.AppearancePressed.Options.UseFont = true;
		this.labelControl1.AutoSize = true;
		this.labelControl1.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.labelControl1.Dock = System.Windows.Forms.DockStyle.Right;
		this.labelControl1.DropDownArrowStyle = DevExpress.XtraEditors.DropDownArrowStyle.Show;
		this.labelControl1.DropDownControl = this.popupMenu1;
		this.labelControl1.Location = new System.Drawing.Point(429, 0);
		this.labelControl1.Name = "labelControl1";
		this.labelControl1.Size = new System.Drawing.Size(57, 56);
		this.labelControl1.TabIndex = 1;
		this.labelControl1.Text = "Filter";
		this.popupMenu1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[6]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.bcFilterAll),
			new DevExpress.XtraBars.LinkPersistInfo(this.bcFilterUnread),
			new DevExpress.XtraBars.LinkPersistInfo(this.bcFilterImportance),
			new DevExpress.XtraBars.LinkPersistInfo(this.bcFilterToday),
			new DevExpress.XtraBars.LinkPersistInfo(this.bcFilterYesterday),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSubItem1)
		});
		this.popupMenu1.Manager = this.barManager1;
		this.popupMenu1.MenuCaption = "Filter";
		this.popupMenu1.Name = "popupMenu1";
		this.popupMenu1.ShowCaption = true;
		this.colPriority.Caption = "colPriority";
		this.colPriority.FieldName = "Priority";
		this.colPriority.Name = "colPriority";
		this.colPriority.Visible = true;
		this.colPriority.VisibleIndex = 6;
		this.imageCollection1.ImageStream = (DevExpress.Utils.ImageCollectionStreamer)resources.GetObject("imageCollection1.ImageStream");
		this.gridControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.gridControl1.Location = new System.Drawing.Point(8, 65);
		this.gridControl1.MainView = this.tileView1;
		this.gridControl1.Name = "gridControl1";
		this.gridControl1.Size = new System.Drawing.Size(514, 597);
		this.gridControl1.TabIndex = 7;
		this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.tileView1 });
		this.tileView1.Appearance.GroupText.Font = new System.Drawing.Font("Segoe UI", 9f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.tileView1.Appearance.GroupText.ForeColor = System.Drawing.Color.FromArgb(248, 124, 50);
		this.tileView1.Appearance.GroupText.Options.UseFont = true;
		this.tileView1.Appearance.GroupText.Options.UseForeColor = true;
		this.tileView1.Appearance.ItemNormal.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.tileView1.Appearance.ItemNormal.Options.UseFont = true;
		this.tileView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[7] { this.colDate, this.colFrom, this.colSubject, this.colPlainText, this.colSubjectDisplayText, this.colRead, this.colPriority });
		this.tileView1.ColumnSet.GroupColumn = this.colDate;
		this.tileView1.ContextButtonOptions.AllowHtmlText = true;
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
		contextButton.Visibility = DevExpress.Utils.ContextItemVisibility.Visible;
		contextButton2.Alignment = DevExpress.Utils.ContextItemAlignment.TopFar;
		contextButton2.Id = new System.Guid("07c1aaac-55bd-4ebc-b850-93a79dede0db");
		contextButton2.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("resource.Image");
		contextButton2.Name = "cbRemove";
		contextButton2.Visibility = DevExpress.Utils.ContextItemVisibility.Visible;
		contextButton3.Alignment = DevExpress.Utils.ContextItemAlignment.TopFar;
		contextButton3.Id = new System.Guid("b5ddbc13-0cf7-4468-afe5-8d6d6df06d77");
		contextButton3.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("resource.Image1");
		contextButton3.Name = "cbEdit";
		contextButton3.Visibility = DevExpress.Utils.ContextItemVisibility.Visible;
		contextButton4.Alignment = DevExpress.Utils.ContextItemAlignment.TopFar;
		contextButton4.Id = new System.Guid("83ef3e55-2c44-470e-acca-c4d796b1ecbd");
		contextButton4.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("resource.Image2");
		contextButton4.Name = "cbShow";
		contextButton4.Visibility = DevExpress.Utils.ContextItemVisibility.Visible;
		contextButton5.Alignment = DevExpress.Utils.ContextItemAlignment.TopFar;
		contextButton5.Id = new System.Guid("214e7575-143e-49d2-9ab8-9aa7f7a88580");
		contextButton5.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("resource.Image3");
		contextButton5.Name = "cbDone";
		contextButton5.Visibility = DevExpress.Utils.ContextItemVisibility.Visible;
		this.tileView1.ContextButtons.Add(contextButton);
		this.tileView1.ContextButtons.Add(contextButton2);
		this.tileView1.ContextButtons.Add(contextButton3);
		this.tileView1.ContextButtons.Add(contextButton4);
		this.tileView1.ContextButtons.Add(contextButton5);
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
		this.tileView1.OptionsTiles.ItemSize = new System.Drawing.Size(560, 80);
		this.tileView1.OptionsTiles.LayoutMode = DevExpress.XtraGrid.Views.Tile.TileViewLayoutMode.List;
		this.tileView1.OptionsTiles.Orientation = System.Windows.Forms.Orientation.Vertical;
		this.tileView1.OptionsTiles.Padding = new System.Windows.Forms.Padding(0);
		this.tileView1.OptionsTiles.ScrollMode = DevExpress.XtraEditors.TileControlScrollMode.TouchScrollBar;
		this.tileView1.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[1]
		{
			new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.colDate, DevExpress.Data.ColumnSortOrder.Ascending)
		});
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
		this.tileView1.TileRows.Add(obj6);
		tableSpan2.ColumnIndex = 1;
		tableSpan2.ColumnSpan = 2;
		tableSpan2.RowIndex = 1;
		tableSpan3.ColumnIndex = 1;
		tableSpan3.ColumnSpan = 3;
		tableSpan4.RowSpan = 3;
		tableSpan5.ColumnIndex = 1;
		tableSpan5.ColumnSpan = 3;
		tableSpan5.RowIndex = 2;
		tableSpan6.ColumnIndex = 3;
		tableSpan6.ColumnSpan = 2;
		tableSpan6.RowIndex = 1;
		this.tileView1.TileSpans.Add(tableSpan2);
		this.tileView1.TileSpans.Add(tableSpan3);
		this.tileView1.TileSpans.Add(tableSpan4);
		this.tileView1.TileSpans.Add(tableSpan5);
		this.tileView1.TileSpans.Add(tableSpan6);
		tileViewItemElement5.Appearance.Normal.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		tileViewItemElement5.Appearance.Normal.ForeColor = System.Drawing.Color.FromArgb(145, 154, 154);
		tileViewItemElement5.Appearance.Normal.Options.UseFont = true;
		tileViewItemElement5.Appearance.Normal.Options.UseForeColor = true;
		tileViewItemElement5.Appearance.Normal.Options.UseTextOptions = true;
		tileViewItemElement5.Appearance.Normal.TextOptions.WordWrap = DevExpress.Utils.WordWrap.NoWrap;
		tileViewItemElement5.Column = this.colDate;
		tileViewItemElement5.ColumnIndex = 4;
		tileViewItemElement5.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement5.RowIndex = 1;
		tileViewItemElement5.Text = "colDate";
		tileViewItemElement5.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleRight;
		tileViewItemElement6.Appearance.Normal.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		tileViewItemElement6.Appearance.Normal.Options.UseFont = true;
		tileViewItemElement6.Appearance.Normal.Options.UseTextOptions = true;
		tileViewItemElement6.Appearance.Normal.TextOptions.WordWrap = DevExpress.Utils.WordWrap.NoWrap;
		tileViewItemElement6.Column = this.colSubjectDisplayText;
		tileViewItemElement6.ColumnIndex = 1;
		tileViewItemElement6.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement6.RowIndex = 1;
		tileViewItemElement6.Text = "colSubjectDisplayText";
		tileViewItemElement6.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleLeft;
		tileViewItemElement7.Appearance.Normal.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		tileViewItemElement7.Appearance.Normal.ForeColor = System.Drawing.Color.FromArgb(145, 154, 154);
		tileViewItemElement7.Appearance.Normal.Options.UseFont = true;
		tileViewItemElement7.Appearance.Normal.Options.UseForeColor = true;
		tileViewItemElement7.Appearance.Normal.Options.UseTextOptions = true;
		tileViewItemElement7.Appearance.Normal.TextOptions.WordWrap = DevExpress.Utils.WordWrap.NoWrap;
		tileViewItemElement7.Column = this.colPlainText;
		tileViewItemElement7.ColumnIndex = 1;
		tileViewItemElement7.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement7.RowIndex = 2;
		tileViewItemElement7.Text = "colPlainText";
		tileViewItemElement7.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleLeft;
		tileViewItemElement8.Appearance.Normal.Font = new System.Drawing.Font("Segoe UI", 12.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		tileViewItemElement8.Appearance.Normal.Options.UseFont = true;
		tileViewItemElement8.Appearance.Normal.Options.UseTextOptions = true;
		tileViewItemElement8.Appearance.Normal.TextOptions.WordWrap = DevExpress.Utils.WordWrap.NoWrap;
		tileViewItemElement8.Column = this.colFrom;
		tileViewItemElement8.ColumnIndex = 2;
		tileViewItemElement8.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement8.Text = "colFrom";
		tileViewItemElement8.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleLeft;
		tileViewItemElement9.Column = this.colRead;
		tileViewItemElement9.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement9.Name = "readElement";
		tileViewItemElement9.RowIndex = 1;
		tileViewItemElement9.StretchVertical = true;
		tileViewItemElement9.Text = "colRead";
		tileViewItemElement9.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleLeft;
		tileViewItemElement9.TextVisible = false;
		tileViewItemElement9.Width = 3;
		this.tileView1.TileTemplate.Add(tileViewItemElement5);
		this.tileView1.TileTemplate.Add(tileViewItemElement6);
		this.tileView1.TileTemplate.Add(tileViewItemElement7);
		this.tileView1.TileTemplate.Add(tileViewItemElement8);
		this.tileView1.TileTemplate.Add(tileViewItemElement9);
		this.colSubject.Caption = "colSubject";
		this.colSubject.ColumnEdit = this.repositoryItemMemoExEdit1;
		this.colSubject.FieldName = "Subject";
		this.colSubject.Name = "colSubject";
		this.colSubject.Visible = true;
		this.colSubject.VisibleIndex = 2;
		this.accordionControl1.AnimationType = DevExpress.XtraBars.Navigation.AnimationType.Office2016;
		this.accordionControl1.Dock = System.Windows.Forms.DockStyle.Left;
		this.accordionControl1.Elements.AddRange(new DevExpress.XtraBars.Navigation.AccordionControlElement[4] { this.accordionControlElement1, this.accordionControlElement4, this.accordionControlElement8, this.accordionControlElement11 });
		this.accordionControl1.Location = new System.Drawing.Point(0, 0);
		this.accordionControl1.LookAndFeel.SkinName = "Office 2016 Colorful";
		this.accordionControl1.Name = "accordionControl1";
		this.accordionControl1.ScrollBarMode = DevExpress.XtraBars.Navigation.ScrollBarMode.Auto;
		this.accordionControl1.ShowFilterControl = DevExpress.XtraBars.Navigation.ShowFilterControl.Always;
		this.accordionControl1.ShowGroupExpandButtons = false;
		this.accordionControl1.Size = new System.Drawing.Size(233, 662);
		this.accordionControl1.TabIndex = 14;
		this.accordionControl1.Text = "Advanced Filter";
		this.accordionControlElement1.Elements.AddRange(new DevExpress.XtraBars.Navigation.AccordionControlElement[2] { this.accordionControlElement2, this.accordionControlElement3 });
		this.accordionControlElement1.Name = "accordionControlElement1";
		this.accordionControlElement1.Text = "View";
		this.accordionControlElement2.Name = "accordionControlElement2";
		this.accordionControlElement2.Style = DevExpress.XtraBars.Navigation.ElementStyle.Item;
		this.accordionControlElement2.Text = "My Task";
		this.accordionControlElement3.Name = "accordionControlElement3";
		this.accordionControlElement3.Style = DevExpress.XtraBars.Navigation.ElementStyle.Item;
		this.accordionControlElement3.Text = "All";
		this.accordionControlElement4.Elements.AddRange(new DevExpress.XtraBars.Navigation.AccordionControlElement[3] { this.accordionControlElement5, this.accordionControlElement6, this.accordionControlElement7 });
		this.accordionControlElement4.Expanded = true;
		this.accordionControlElement4.Name = "accordionControlElement4";
		this.accordionControlElement4.Text = "Status";
		this.accordionControlElement5.Name = "accordionControlElement5";
		this.accordionControlElement5.Style = DevExpress.XtraBars.Navigation.ElementStyle.Item;
		this.accordionControlElement5.Text = "Not Started";
		this.accordionControlElement6.Name = "accordionControlElement6";
		this.accordionControlElement6.Style = DevExpress.XtraBars.Navigation.ElementStyle.Item;
		this.accordionControlElement6.Text = "In Process";
		this.accordionControlElement7.Name = "accordionControlElement7";
		this.accordionControlElement7.Style = DevExpress.XtraBars.Navigation.ElementStyle.Item;
		this.accordionControlElement7.Text = "Completed";
		this.accordionControlElement8.Elements.AddRange(new DevExpress.XtraBars.Navigation.AccordionControlElement[2] { this.accordionControlElement9, this.accordionControlElement10 });
		this.accordionControlElement8.Expanded = true;
		this.accordionControlElement8.Name = "accordionControlElement8";
		this.accordionControlElement8.Text = "Business Partner";
		this.accordionControlElement9.Name = "accordionControlElement9";
		this.accordionControlElement9.Style = DevExpress.XtraBars.Navigation.ElementStyle.Item;
		this.accordionControlElement9.Text = "SYSTECH";
		this.accordionControlElement10.Name = "accordionControlElement10";
		this.accordionControlElement10.Style = DevExpress.XtraBars.Navigation.ElementStyle.Item;
		this.accordionControlElement10.Text = "VHM";
		this.accordionControlElement11.Elements.AddRange(new DevExpress.XtraBars.Navigation.AccordionControlElement[4] { this.accordionControlElement12, this.accordionControlElement13, this.accordionControlElement14, this.accordionControlElement15 });
		this.accordionControlElement11.Expanded = true;
		this.accordionControlElement11.Name = "accordionControlElement11";
		this.accordionControlElement11.Text = "User";
		this.accordionControlElement12.Name = "accordionControlElement12";
		this.accordionControlElement12.Style = DevExpress.XtraBars.Navigation.ElementStyle.Item;
		this.accordionControlElement12.Text = "duonglh";
		this.accordionControlElement13.HeaderTemplate.AddRange(new DevExpress.XtraBars.Navigation.HeaderElementInfo[4]
		{
			new DevExpress.XtraBars.Navigation.HeaderElementInfo(DevExpress.XtraBars.Navigation.HeaderElementType.ContextButtons, DevExpress.XtraBars.Navigation.HeaderElementAlignment.Left),
			new DevExpress.XtraBars.Navigation.HeaderElementInfo(DevExpress.XtraBars.Navigation.HeaderElementType.Text),
			new DevExpress.XtraBars.Navigation.HeaderElementInfo(DevExpress.XtraBars.Navigation.HeaderElementType.Image),
			new DevExpress.XtraBars.Navigation.HeaderElementInfo(DevExpress.XtraBars.Navigation.HeaderElementType.HeaderControl)
		});
		this.accordionControlElement13.Name = "accordionControlElement13";
		this.accordionControlElement13.Style = DevExpress.XtraBars.Navigation.ElementStyle.Item;
		this.accordionControlElement13.Text = "nghilq";
		this.accordionControlElement14.Name = "accordionControlElement14";
		this.accordionControlElement14.Style = DevExpress.XtraBars.Navigation.ElementStyle.Item;
		this.accordionControlElement14.Text = "quandn";
		this.accordionControlElement15.Name = "accordionControlElement15";
		this.accordionControlElement15.Style = DevExpress.XtraBars.Navigation.ElementStyle.Item;
		this.accordionControlElement15.Text = "thanhnv";
		this.splitContainerControl1.Location = new System.Drawing.Point(239, 0);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.gridControl1);
		this.splitContainerControl1.Panel1.Controls.Add(this.panel1);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(1239, 662);
		this.splitContainerControl1.SplitterPosition = 522;
		this.splitContainerControl1.TabIndex = 15;
		this.splitContainerControl1.Text = "splitContainerControl1";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1480, 662);
		base.Controls.Add(this.splitContainerControl1);
		base.Controls.Add(this.accordionControl1);
		base.Controls.Add(this.barDockControlLeft);
		base.Controls.Add(this.barDockControlRight);
		base.Controls.Add(this.barDockControlBottom);
		base.Controls.Add(this.barDockControlTop);
		base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
		base.Name = "frm_Task";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		this.Text = "Task";
		base.Load += new System.EventHandler(this.frm_Task_Load);
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.barManager1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.imageCollection2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.panel1).EndInit();
		this.panel1.ResumeLayout(false);
		this.panel1.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.buttonEdit1.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.popupMenu1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.imageCollection1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridControl1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tileView1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.accordionControl1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
