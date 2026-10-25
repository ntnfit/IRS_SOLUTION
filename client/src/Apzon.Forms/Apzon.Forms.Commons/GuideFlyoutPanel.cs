using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Controls.Controls;
using DevExpress.Utils;
using DevExpress.XtraBars.Docking2010;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;

namespace Apzon.Forms.Commons;

public class GuideFlyoutPanel : System.Windows.Forms.UserControl
{
	private int curentLessonIdCore;

	private IContainer components;

	private LayoutControl layoutControl;

	private LayoutControlGroup layoutControlGroup;

	private WindowsUIButtonPanel navigator;

	private LayoutControlItemEx navigatorItem;

	private SimpleButtonEx backButton;

	private LayoutControlItemEx backItem;

	private SimpleButtonEx nextButton;

	private LayoutControlItemEx nextItem;

	private SimpleButtonEx skipButton;

	private LayoutControlItemEx shipItem;

	private EmptySpaceItem emptyItem;

	private LabelControl label;

	private LayoutControlItemEx labelItem;

	public string LabelText
	{
		get
		{
			return label.Text;
		}
		set
		{
			label.Text = value;
		}
	}

	public int CurrentLessonIndex => curentLessonIdCore;

	public GuideFlyoutPanel()
	{
	}

	public GuideFlyoutPanel(int lessonCount)
	{
		InitializeComponent();
		curentLessonIdCore = 0;
		InitializeNavigator(lessonCount);
	}

	private void InitializeNavigator(int count)
	{
		for (int i = 0; i < count; i++)
		{
			navigator.Buttons.Add(new WindowsUIButton("Button", GetResourceImage(), 0, DevExpress.XtraBars.Docking2010.ImageLocation.Default, ButtonStyle.CheckButton, "", useCaption: false, -1, enabled: true, null, useImage: true, i == curentLessonIdCore, visible: true, null, null, 1, isLeft: false, enableImageTransparency: false));
		}
	}

	private Image GetResourceImage()
	{
		return ResourceImageHelperCore.CreateImageFromResourcesEx("Apzon.Forms.Resources.navigationButton.png", typeof(GuideFlyoutPanel).Assembly);
	}

	private void OnNavigatorButtonChecked(object sender, ButtonEventArgs e)
	{
		curentLessonIdCore = navigator.Buttons.IndexOf(e.Button);
	}

	private void OnSkipButtonClick(object sender, EventArgs e)
	{
	}

	private void OnBackButtonClick(object sender, EventArgs e)
	{
		curentLessonIdCore--;
		if (curentLessonIdCore < 0)
		{
			curentLessonIdCore = navigator.Buttons.Count - 1;
		}
		navigator.Buttons[curentLessonIdCore].Properties.Checked = true;
	}

	private void OnNextButtonClick(object sender, EventArgs e)
	{
		curentLessonIdCore++;
		if (curentLessonIdCore > navigator.Buttons.Count - 1)
		{
			curentLessonIdCore = 0;
		}
		navigator.Buttons[curentLessonIdCore].Properties.Checked = true;
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
		this.layoutControl = new DevExpress.XtraLayout.LayoutControl();
		this.label = new DevExpress.XtraEditors.LabelControl();
		this.skipButton = new Apzon.Controls.Controls.SimpleButtonEx();
		this.nextButton = new Apzon.Controls.Controls.SimpleButtonEx();
		this.backButton = new Apzon.Controls.Controls.SimpleButtonEx();
		this.navigator = new DevExpress.XtraBars.Docking2010.WindowsUIButtonPanel();
		this.layoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
		this.nextItem = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.backItem = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.shipItem = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.navigatorItem = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.emptyItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.labelItem = new Apzon.Controls.Controls.LayoutControlItemEx();
		((System.ComponentModel.ISupportInitialize)this.layoutControl).BeginInit();
		this.layoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nextItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.backItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.shipItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.navigatorItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptyItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.labelItem).BeginInit();
		base.SuspendLayout();
		this.layoutControl.Controls.Add(this.label);
		this.layoutControl.Controls.Add(this.skipButton);
		this.layoutControl.Controls.Add(this.nextButton);
		this.layoutControl.Controls.Add(this.backButton);
		this.layoutControl.Controls.Add(this.navigator);
		this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layoutControl.Location = new System.Drawing.Point(0, 0);
		this.layoutControl.Name = "layoutControl";
		this.layoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(954, 91, 697, 350);
		this.layoutControl.Root = this.layoutControlGroup;
		this.layoutControl.Size = new System.Drawing.Size(234, 148);
		this.layoutControl.TabIndex = 0;
		this.layoutControl.Text = "layoutControl1";
		this.label.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.label.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
		this.label.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
		this.label.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
		this.label.Location = new System.Drawing.Point(12, 12);
		this.label.Name = "label";
		this.label.Size = new System.Drawing.Size(210, 74);
		this.label.StyleController = this.layoutControl;
		this.label.TabIndex = 0;
		this.label.Text = "Empty";
		this.skipButton.Location = new System.Drawing.Point(12, 114);
		this.skipButton.Name = "skipButton";
		this.skipButton.Size = new System.Drawing.Size(46, 22);
		this.skipButton.StyleController = this.layoutControl;
		this.skipButton.TabIndex = 7;
		this.skipButton.Text = "Skip";
		this.skipButton.Click += new System.EventHandler(this.OnSkipButtonClick);
		this.nextButton.Location = new System.Drawing.Point(176, 114);
		this.nextButton.Name = "nextButton";
		this.nextButton.Size = new System.Drawing.Size(46, 22);
		this.nextButton.StyleController = this.layoutControl;
		this.nextButton.TabIndex = 6;
		this.nextButton.Text = "Next";
		this.nextButton.Click += new System.EventHandler(this.OnNextButtonClick);
		this.backButton.Location = new System.Drawing.Point(126, 114);
		this.backButton.Name = "backButton";
		this.backButton.Size = new System.Drawing.Size(46, 22);
		this.backButton.StyleController = this.layoutControl;
		this.backButton.TabIndex = 5;
		this.backButton.Text = "Back";
		this.backButton.Click += new System.EventHandler(this.OnBackButtonClick);
		this.navigator.AppearanceButton.Hovered.ForeColor = System.Drawing.Color.Gray;
		this.navigator.AppearanceButton.Hovered.Options.UseForeColor = true;
		this.navigator.AppearanceButton.Normal.ForeColor = System.Drawing.Color.Silver;
		this.navigator.AppearanceButton.Normal.Options.UseForeColor = true;
		this.navigator.AppearanceButton.Pressed.BackColor = System.Drawing.Color.Gray;
		this.navigator.AppearanceButton.Pressed.Options.UseBackColor = true;
		this.navigator.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
		this.navigator.ButtonInterval = 5;
		this.navigator.Location = new System.Drawing.Point(12, 90);
		this.navigator.Name = "navigator";
		this.navigator.Size = new System.Drawing.Size(210, 20);
		this.navigator.TabIndex = 4;
		this.navigator.Text = "windowsUIButtonPanel1";
		this.navigator.UseButtonBackgroundImages = false;
		this.navigator.ButtonChecked += new DevExpress.XtraBars.Docking2010.ButtonEventHandler(this.OnNavigatorButtonChecked);
		this.layoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup.GroupBordersVisible = false;
		this.layoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[6] { this.nextItem, this.backItem, this.shipItem, this.navigatorItem, this.emptyItem, this.labelItem });
		this.layoutControlGroup.Location = new System.Drawing.Point(0, 0);
		this.layoutControlGroup.Name = "Root";
		this.layoutControlGroup.Size = new System.Drawing.Size(234, 148);
		this.layoutControlGroup.TextVisible = false;
		this.nextItem.Control = this.nextButton;
		this.nextItem.Location = new System.Drawing.Point(164, 102);
		this.nextItem.MaxSize = new System.Drawing.Size(50, 26);
		this.nextItem.MinSize = new System.Drawing.Size(50, 26);
		this.nextItem.Name = "nextItem";
		this.nextItem.Size = new System.Drawing.Size(50, 26);
		this.nextItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.nextItem.TextSize = new System.Drawing.Size(0, 0);
		this.nextItem.TextVisible = false;
		this.backItem.Control = this.backButton;
		this.backItem.Location = new System.Drawing.Point(114, 102);
		this.backItem.MaxSize = new System.Drawing.Size(50, 26);
		this.backItem.MinSize = new System.Drawing.Size(50, 26);
		this.backItem.Name = "backItem";
		this.backItem.Size = new System.Drawing.Size(50, 26);
		this.backItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.backItem.TextSize = new System.Drawing.Size(0, 0);
		this.backItem.TextVisible = false;
		this.shipItem.Control = this.skipButton;
		this.shipItem.Location = new System.Drawing.Point(0, 102);
		this.shipItem.MaxSize = new System.Drawing.Size(50, 26);
		this.shipItem.MinSize = new System.Drawing.Size(50, 26);
		this.shipItem.Name = "shipItem";
		this.shipItem.Size = new System.Drawing.Size(50, 26);
		this.shipItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.shipItem.TextSize = new System.Drawing.Size(0, 0);
		this.shipItem.TextVisible = false;
		this.navigatorItem.Control = this.navigator;
		this.navigatorItem.Location = new System.Drawing.Point(0, 78);
		this.navigatorItem.MaxSize = new System.Drawing.Size(0, 24);
		this.navigatorItem.MinSize = new System.Drawing.Size(1, 24);
		this.navigatorItem.Name = "navigatorItem";
		this.navigatorItem.Size = new System.Drawing.Size(214, 24);
		this.navigatorItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.navigatorItem.TextSize = new System.Drawing.Size(0, 0);
		this.navigatorItem.TextVisible = false;
		this.emptyItem.AllowHotTrack = false;
		this.emptyItem.Location = new System.Drawing.Point(50, 102);
		this.emptyItem.Name = "emptyItem";
		this.emptyItem.Size = new System.Drawing.Size(64, 26);
		this.emptyItem.TextSize = new System.Drawing.Size(0, 0);
		this.labelItem.Control = this.label;
		this.labelItem.Location = new System.Drawing.Point(0, 0);
		this.labelItem.MinSize = new System.Drawing.Size(14, 17);
		this.labelItem.Name = "labelItem";
		this.labelItem.Size = new System.Drawing.Size(214, 78);
		this.labelItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.labelItem.TextSize = new System.Drawing.Size(0, 0);
		this.labelItem.TextVisible = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.layoutControl);
		base.Name = "GuideFlyoutPanel";
		base.Size = new System.Drawing.Size(234, 148);
		((System.ComponentModel.ISupportInitialize)this.layoutControl).EndInit();
		this.layoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nextItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.backItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.shipItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.navigatorItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptyItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.labelItem).EndInit();
		base.ResumeLayout(false);
	}
}
